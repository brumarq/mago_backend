# Define your variables
#$resourceGroupName = "mago-backend"
$resourceGroupName = "mago-backend"
$location = 'westeurope'
$functionAppName = "CalculateAveragesFunction"
$subscriptionId = '8e7c7eb8-572a-4e7b-9a57-9efbf2706e4a'
$storageAccountName = 'magostorageacc'
$serverFarmName = 'mago-server-farm'

$bicepTemplatePath = './template.bicep'

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

Write-Host "Deploying resources in $resourceGroupName"

# Set the context to your subscription
az account set --subscription $subscriptionId

# Create new resource group (if does not exist)
az group create -l $location -n $resourceGroupName

# Deploy resources inside resource group
$cmd = "az deployment group create --mode Incremental --resource-group $resourceGroupName --template-file $bicepTemplatePath --parameters $parameters"
Write-Host $cmd
Invoke-Expression $cmd

# Publish Python solution to Azure Function
$functionAppFolder = "./CalculateAveragesFunction"  # Update with the correct path to your Python function app

# Zip the Python function app
$publishZip = "publish.zip"
Compress-Archive -Path $functionAppFolder -DestinationPath $publishZip -Force

# Deploy the Python function app
az functionapp deployment source config-zip --resource-group $resourceGroupName --name $functionAppName --src $publishZip

# Clean up - remove the temporary zip file
Remove-Item $publishZip