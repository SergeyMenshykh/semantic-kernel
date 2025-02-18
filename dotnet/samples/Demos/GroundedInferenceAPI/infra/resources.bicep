@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}

param groundedInferenceApiExists bool
@secure()
param groundedInferenceApiDefinition object

@description('Id of the user or app to assign application roles')
param principalId string

@description('The AI Service Account full ARM Resource ID. This is an optional field, and if not provided, the resource will be created.')
param aiServiceAccountResourceId string = ''

@description('Azure OpenAI chat model name')
param azureOpenAIChatModelName string

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = uniqueString(subscription().id, resourceGroup().id, location)

// Monitor application with Azure Monitor
module monitoring 'br/public:avm/ptn/azd/monitoring:0.1.0' = {
  name: 'monitoring'
  params: {
    logAnalyticsName: '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
    applicationInsightsName: '${abbrs.insightsComponents}${resourceToken}'
    applicationInsightsDashboardName: '${abbrs.portalDashboards}${resourceToken}'
    location: location
    tags: tags
  }
}

// Container registry
module containerRegistry 'br/public:avm/res/container-registry/registry:0.1.1' = {
  name: 'registry'
  params: {
    name: '${abbrs.containerRegistryRegistries}${resourceToken}'
    location: location
    acrAdminUserEnabled: true
    tags: tags
    publicNetworkAccess: 'Enabled'
    roleAssignments:[
      {
        principalId: groundedInferenceApiIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
      }
    ]
  }
}

// Container apps environment
module containerAppsEnvironment 'br/public:avm/res/app/managed-environment:0.4.5' = {
  name: 'container-apps-environment'
  params: {
    logAnalyticsWorkspaceResourceId: monitoring.outputs.logAnalyticsWorkspaceResourceId
    name: '${abbrs.appManagedEnvironments}${resourceToken}'
    location: location
    zoneRedundant: false
  }
}

module groundedInferenceApiIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.2.1' = {
  name: 'groundedInferenceApiidentity'
  params: {
    name: '${abbrs.managedIdentityUserAssignedIdentities}groundedInferenceApi-${resourceToken}'
    location: location
  }
}

// Create a keyvault to store secrets
module keyVault 'br/public:avm/res/key-vault/vault:0.6.1' = {
  name: 'keyvault'
  params: {
    name: '${abbrs.keyVaultVaults}${resourceToken}'
    location: location
    tags: tags
    enableRbacAuthorization: false
    accessPolicies: [
      {
        objectId: principalId
        permissions: {
          secrets: [ 'get', 'list' ]
        }
      }
      {
        objectId: groundedInferenceApiIdentity.outputs.principalId
        permissions: {
          secrets: [ 'get', 'list' ]
        }
      }
    ]
    secrets: [
        {
            name: 'AIServices--AzureOpenAI--ChatDeploymentName'
            value: azureOpenAIChatModelName
        }
        {
            name: 'AIServices--AzureOpenAI--Endpoint'
            value: aiDependencies.outputs.AZURE_AI_SERVICE_ENDPOINT
        }
    ]
  }
}

module groundedInferenceApiFetchLatestImage './modules/fetch-container-image.bicep' = {
  name: 'groundedInferenceApi-fetch-image'
  params: {
    exists: groundedInferenceApiExists
    name: 'grounded-inference-api'
  }
}

var groundedInferenceApiAppSettingsArray = filter(array(groundedInferenceApiDefinition.settings), i => i.name != '')
var groundedInferenceApiSecrets = map(filter(groundedInferenceApiAppSettingsArray, i => i.?secret != null), i => {
  name: i.name
  value: i.value
  secretRef: i.?secretRef ?? take(replace(replace(toLower(i.name), '_', '-'), '.', '-'), 32)
})
var groundedInferenceApiEnv = map(filter(groundedInferenceApiAppSettingsArray, i => i.?secret == null), i => {
  name: i.name
  value: i.value
})

module groundedInferenceApi 'br/public:avm/res/app/container-app:0.8.0' = {
  name: 'groundedInferenceApi'
  params: {
    name: 'grounded-inference-api'
    ingressTargetPort: 8080
    scaleMinReplicas: 1
    scaleMaxReplicas: 10
    secrets: {
      secureList:  union([
      ],
      map(groundedInferenceApiSecrets, secret => {
        name: secret.secretRef
        value: secret.value
      }))
    }
    containers: [
      {
        image: groundedInferenceApiFetchLatestImage.outputs.?containers[?0].?image ?? 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
        name: 'main'
        resources: {
          cpu: json('0.5')
          memory: '1.0Gi'
        }
        env: union([
          {
            name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
            value: monitoring.outputs.applicationInsightsConnectionString
          }
          {
            name: 'AZURE_CLIENT_ID'
            value: groundedInferenceApiIdentity.outputs.clientId
          }
          {
            name: 'PORT'
            value: '8080'
          }
          {
            name: 'AZURE_KEY_VAULT_ENDPOINT'
            value: keyVault.outputs.uri
          }
        ],
        groundedInferenceApiEnv,
        map(groundedInferenceApiSecrets, secret => {
            name: secret.name
            secretRef: secret.secretRef
        }))
      }
    ]
    managedIdentities:{
      systemAssigned: false
      userAssignedResourceIds: [groundedInferenceApiIdentity.outputs.resourceId]
    }
    registries:[
      {
        server: containerRegistry.outputs.loginServer
        identity: groundedInferenceApiIdentity.outputs.resourceId
      }
    ]
    environmentResourceId: containerAppsEnvironment.outputs.resourceId
    location: location
    tags: union(tags, { 'azd-service-name': 'grounded-inference-api' })
  }
}

// Creare AI services
module aiDependencies './modules/create-azure-ai-service.bicep' = {
  name: 'ai-service'
  params: {
    aiServicesName: '${abbrs.cognitiveServicesAccounts}${resourceToken}'
    aiServiceAccountResourceId: aiServiceAccountResourceId

    // Identity
    managedIdentityResourceId: groundedInferenceApiIdentity.outputs.resourceId
    managedIdentityPrincipalId: groundedInferenceApiIdentity.outputs.principalId
    userPrincipalId: principalId

     // Model deployment parameters
     modelName: azureOpenAIChatModelName
     modelFormat: 'OpenAI'
     modelVersion: '2024-07-18'
     modelSkuName: 'GlobalStandard'
     modelCapacity: 50
     modelLocation: location
  }
}

output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.outputs.loginServer
output AZURE_KEY_VAULT_ENDPOINT string = keyVault.outputs.uri
output AZURE_KEY_VAULT_NAME string = keyVault.outputs.name
output AZURE_RESOURCE_GROUNDED_INFERENCE_API_ID string = groundedInferenceApi.outputs.resourceId
output AZURE_AI_SERVICE_ENDPOINT string = aiDependencies.outputs.AZURE_AI_SERVICE_ENDPOINT
