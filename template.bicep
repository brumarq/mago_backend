@description('Location for all resources.')
param location string = resourceGroup().location

@description('The name of the SQL logical server.')
param databaseServerName string

@description('The administrator username of the SQL logical server.')
param administratorLogin string

@description('The administrator password of the SQL logical server.')
param administratorLoginPassword string

@description('Storage account name')
param storageAccountName string

@description('Azure function name')
param functionAppName string

@description('Server farm name')
param serverFarmName string

// Database names
@description('Name of the metrics database')
param metricsDBName string

@description('Name of the firmware database')
param firmwareDBName string

@description('Name of the notifications database')
param notificationsDBName string

@description('Name of the device database')
param deviceDBName string

// Random shit
param tags object = {}
param appInsightsRetention int = 30
param numberOfWorkers int = 1 // why do i need this??

param dbNames array = [
  metricsDBName
  firmwareDBName
  notificationsDBName
  deviceDBName
]

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
    tier: 'Dynamic'
  }
  properties: {}
  kind: 'functionapp'
}

// Resources for azure function
resource functionApp 'Microsoft.Web/sites@2022-09-01' = {
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
      autoHealEnabled: true
      autoHealRules: {
        triggers: {
          privateBytesInKB: 0
          statusCodes: [
            {
              status: 500
              subStatus: 0
              win32Status: 0
              count: 25
              timeInterval: '00:05:00'
            }
          ]
        }
        actions: {
          actionType: 'Recycle'
          minProcessExecutionTime: '00:01:00'
        }
      }
      numberOfWorkers: numberOfWorkers
    }
    clientAffinityEnabled: false
    httpsOnly: true
    containerSize: 1536
    redundancyMode: 'None'
  }

  resource functionAppConfig 'config@2021-03-01' = {
    name: 'appsettings'
    properties: {
      'AzureWebJobsStorage': 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${listKeys(resourceId('Microsoft.Storage/storageAccounts', storageAccount.name), '2021-08-01').keys[0].value};EndpointSuffix=core.windows.net'
      'FUNCTIONS_EXTENSION_VERSION': '~4'
      'FUNCTIONS_WORKER_RUNTIME': 'python' // Set Python as the worker runtime
      'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING': 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${listKeys(resourceId('Microsoft.Storage/storageAccounts', storageAccount.name), '2019-04-01').keys[0].value};EndpointSuffix=core.windows.net'
      'WEBSITE_CONTENTSHARE': replace(toLower(functionApp.name), '-', '')
      // ai settings
      'APPINSIGHTS_INSTRUMENTATIONKEY': reference('Microsoft.Insights/components/${appInsights.name}', '2015-05-01').InstrumentationKey
      'ApplicationInsightsAgent_EXTENSION_VERSION': '~2'
      'InstrumentationEngine_EXTENSION_VERSION': '~1'
      'METRICS_DB_CONNECTION_STRING_PYODBC': 'Driver={ODBC Driver 17 for SQL Server};Server=tcp:mago-database-server.database.windows.net,1433;Database=MetricsDB;Uid=MagoDBAdmin;Pwd=Test123*;Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30;'
      'WEBSITE_RUN_FROM_PACKAGE': 1 // For Python, use package deployment
    }
  }
}

//Resources for database creation
resource sqlServer 'Microsoft.Sql/servers@2022-02-01-preview' = {
  name: databaseServerName
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }
}

resource sqlDatabases 'Microsoft.Sql/servers/databases@2022-02-01-preview' = [for dbName in dbNames: {
  name: dbName
  parent: sqlServer
  location: location
  sku: {
    name: 'Basic'
    size: 'Basic'
    tier: 'Basic'
  }
}]

