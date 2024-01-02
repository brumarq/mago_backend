@description('Location for all resources.')
param location string = resourceGroup().location

@description('The name of the SQL logical server.')
param databaseServerName string

@description('The administrator username of the SQL logical server.')
param administratorLogin string

@description('The administrator password of the SQL logical server.')
param administratorLoginPassword string

// Database names
@description('Name of the metrics database')
param metricsDBName string

@description('Name of the firmware database')
param firmwareDBName string

@description('Name of the notifications database')
param notificationsDBName string

@description('Name of the device database')
param deviceDBName string

param dbNames array = [
  metricsDBName
  firmwareDBName
  notificationsDBName
  deviceDBName
]

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

