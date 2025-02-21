@description('AI services name')
param aiServicesName string

@description('Model/AI Resource deployment location')
param location string 

@description('The AI Service Account full ARM Resource ID. This is an optional field, and if not provided, the resource will be created.')
param aiServiceAccountResourceId string

@description('Managed identity resource ID')
param managedIdentityResourceId string

@description('Managed identity principal ID')
param managedIdentityPrincipalId string

@description('User principal ID')
param userPrincipalId string

@description('Array of properties for model deployments')
param modelDeploymentsProps array

var aiServiceExists = aiServiceAccountResourceId != ''

var aiServiceParts = split(aiServiceAccountResourceId, '/')

resource existingAIServiceAccount 'Microsoft.CognitiveServices/accounts@2024-10-01' existing = if (aiServiceExists) {
  name: aiServiceParts[8]
  scope: resourceGroup(aiServiceParts[2], aiServiceParts[4])
}

resource aiServices 'Microsoft.CognitiveServices/accounts@2024-10-01' = if(!aiServiceExists) {
  name: aiServicesName
  location: location
  sku: {
    name: 'S0'
  }
  kind: 'AIServices' // or 'OpenAI'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {  
      '${managedIdentityResourceId}': {}  
    }  
  }
  properties: {
    customSubDomainName: toLower('${toLower(aiServicesName)}')
    apiProperties: {
      statisticsEnabled: false
    }
    publicNetworkAccess: 'Enabled'
  }
}

// Cognitive Services OpenAI User. Ability to view files, models, deployments. Readers can't make any changes They can inference and create images
resource cognitiveServicesOpenAIUserRole 'Microsoft.Authorization/roleDefinitions@2022-04-01' existing = {
  name: '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
  scope: resourceGroup()
}

// This role assignment grants the managed identity the required permissions to access the AI services such as Azure OpenAI. 
resource managedIdentityPrincipalOpenAIUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: aiServices
  name: guid(aiServicesName, cognitiveServicesOpenAIUserRole.id, aiServices.id)
  properties: {
    principalId: managedIdentityPrincipalId
    roleDefinitionId: cognitiveServicesOpenAIUserRole.id
    principalType: 'ServicePrincipal'
  }
}

// This role assignment grants the user the required permissions to access the AI services such as Azure OpenAI.
resource userPrincipalOpenAIUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: aiServices
  name: guid(aiServicesName, userPrincipalId, aiServices.id)
  properties: {
    principalId: userPrincipalId
    roleDefinitionId: cognitiveServicesOpenAIUserRole.id
    principalType: 'User'
  }
}

@batchSize(1)
resource modelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01'= [for modelProps in modelDeploymentsProps: if(!aiServiceExists) {
  parent: aiServices
  name: modelProps.name
  sku : {
    capacity: modelProps.capacity
    name: modelProps.skuName
  }
  properties: {
    model:{
      name: modelProps.name
      format: modelProps.format
      version: modelProps.version
    }
  }
}]

 output AZURE_AI_SERVICE_ENDPOINT string = aiServiceExists ? existingAIServiceAccount.properties.endpoints['OpenAI Language Model Instance API'] : aiServices.properties.endpoints['OpenAI Language Model Instance API']