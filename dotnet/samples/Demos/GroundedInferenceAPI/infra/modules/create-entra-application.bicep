extension microsoftGraphPreview

@description('The name of the application')
param applicationName string

@description('The display name of the application')
param applicationDisplayName string

@description('The endpoint of the web application')
param webAppEndpoint string

resource appRegistration 'Microsoft.Graph/applications@v1.0' = {
  uniqueName: applicationName
  displayName: applicationDisplayName
  signInAudience: 'AzureADMyOrg'
  web: {
    homePageUrl: webAppEndpoint
    redirectUris: ['${webAppEndpoint}/.auth/login/aad/callback']
    implicitGrantSettings: {enableIdTokenIssuance: true}
  }
  api:{
      requestedAccessTokenVersion: 2
      oauth2PermissionScopes: [{
          adminConsentDescription: 'Allow the application to access grounded-inference-api on behalf of the signed-in user.'
		  adminConsentDisplayName: 'Access grounded-inference-api'
          id: 'da8a48e5-d82f-4eb3-aa98-76dcf1035de2'
          isEnabled: true
          type: 'User'
          userConsentDescription: 'Allow the application to access grounded-inference-api on your behalf.'
          userConsentDisplayName: 'Access grounded-inference-api'
          value: 'user_impersonation'
      }]
  }
  requiredResourceAccess: [
    {
     resourceAppId: '00000003-0000-0000-c000-000000000000'
     resourceAccess: [
       // User.Read
       {id: 'e1fe6dd8-ba31-4d61-89e7-88639da4683d', type: 'Scope'}
     ]
    }
  ]
}

output appId string = appRegistration.appId