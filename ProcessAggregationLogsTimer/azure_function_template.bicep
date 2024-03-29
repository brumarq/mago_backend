@description('Location for all resources.')
param location string = resourceGroup().location

@description('Storage account name')
param storageAccountName string

@description('Azure function name')
param functionAppName string

@description('Server farm name')
param serverFarmName string

param tags object = {}
param appInsightsRetention int = 30

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
      supportsHttpsTrafficOnly: true
      allowBlobPublicAccess: true
      minimumTlsVersion: 'TLS1_2'
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: storageAccountName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Bluefield'
    Request_Source: 'rest'
    RetentionInDays: appInsightsRetention
  }
  tags: tags
}

resource serverFarm 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: serverFarmName
  location: location
  tags: tags
  sku: {
    name: 'Y1'
    tier: 'Standard'
  }
  properties: {
    reserved: true
  }
  kind: 'linux'
}

// Resources for azure function
resource functionApp 'Microsoft.Web/sites@2023-01-01' = {
  name: functionAppName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    enabled: true
    serverFarmId: resourceId('Microsoft.Web/serverfarms', serverFarm.name)
    siteConfig: {
      pythonVersion: '3.11'
      linuxFxVersion: 'python|3.11'
    }
    httpsOnly: false
  }

  resource functionAppConfig 'config@2021-03-01' = {
    name: 'appsettings'
    properties: {
      'AzureWebJobsStorage': 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${listKeys(resourceId('Microsoft.Storage/storageAccounts', storageAccount.name), '2021-08-01').keys[0].value};EndpointSuffix=core.windows.net'
      'FUNCTIONS_EXTENSION_VERSION': '~4'
      'FUNCTIONS_WORKER_RUNTIME': 'python' // Set Python as the worker runtime
      'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING': 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${listKeys(resourceId('Microsoft.Storage/storageAccounts', storageAccount.name), '2019-04-01').keys[0].value};EndpointSuffix=core.windows.net'
      'WEBSITE_CONTENTSHARE': replace(toLower(functionApp.name), '-', '')
      'APPINSIGHTS_INSTRUMENTATIONKEY': reference('Microsoft.Insights/components/${appInsights.name}', '2015-05-01').InstrumentationKey
      'AzureWebJobsFeatureFlags': 'EnableWorkerIndexing'
      'ApplicationInsightsAgent_EXTENSION_VERSION': '~2'
      'InstrumentationEngine_EXTENSION_VERSION': '~1'
    }
  }
}
