# Define your variables
#$resourceGroupName = "mago-backend"
$resourceGroupName = "mago-backend"
$location = 'westeurope'
$functionAppName = "ProcessAggregationLogsTimer"
$subscriptionId = '8e7c7eb8-572a-4e7b-9a57-9efbf2706e4a'
$storageAccountName = 'magostorageacc'
$serverFarmName = 'mago-server-farm'

$bicepTemplatePath = './resource_template.bicep'

# Database details
$databaseServerName = 'mago-database-server'
$administratorLogin = 'MagoAdminDB'
$administratorLoginPassword = 'Test123*'
$metricsDBName = 'MetricsDB'
$firmwareDBName = 'FirmwareDB'
$notificationsDBName = 'NotificationsDB'
$deviceDBName = 'DeviceDB'

$parameters = @{
    storageAccountName = $storageAccountName
    databaseServerName = $databaseServerName
    serverFarmName = $serverFarmName
    functionAppName = $functionAppName
    administratorLogin = $administratorLogin
    administratorLoginPassword = $administratorLoginPassword
    metricsDBName = $metricsDBName
    firmwareDBName = $firmwareDBName
    notificationsDBName = $notificationsDBName
    deviceDBName = $deviceDBName
}

$parameters = $parameters.Keys.ForEach({"$_=$($parameters[$_])"}) -join ' '

Write-Host "Deploying resources to $resourceGroupName"

# Set the context to your subscription
az account set --subscription $subscriptionId

# Create new resource group (if does not exist)
az group create -l $location -n $resourceGroupName

# Deploy resources inside resource group
$cmd = "az deployment group create --mode Incremental --resource-group $resourceGroupName --template-file $bicepTemplatePath --parameters $parameters"
Write-Host $cmd
Invoke-Expression $cmd