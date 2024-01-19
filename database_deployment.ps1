# Variables for production environment
$location = 'westeurope'
$functionAppName = 'ProcessAggregationLogsTimer'
$resourceGroupNameProduction = 'mago-backend'
$storageAccountNameProduction = 'magostorageacc'
$serverFarmNameProduction = 'mago-server-farm'
$databaseServerNameProduction = 'mago-database-server'
$keyVaultName = 'mago-secret-vault'
$subscriptionId = '8e7c7eb8-572a-4e7b-9a57-9efbf2706e4a'

# Variables for test environment
$resourceGroupNameTest = 'test-mago-backend'
$databaseServerNameTest = 'test-mago-database-server'
$storageAccountNameTest = 'testmagostorageacc'
$serverFarmNameTest = 'test-mago-server-farm'

# Database details
$adminLoginSecret = az keyvault secret show --vault-name $keyVaultName --name SQL-SERVER-ADMIN-USERNAME --query 'value' -o tsv
$adminLoginPasswordSecret = az keyvault secret show --vault-name $keyVaultName --name SQL-SERVER-ADMIN-PASSWORD --query 'value' -o tsv

$administratorLogin = $adminLoginSecret
$administratorLoginPassword = $adminLoginPasswordSecret
$metricsDBName = 'MetricsDB'
$firmwareDBName = 'FirmwareDB'
$notificationsDBName = 'NotificationsDB'
$deviceDBName = 'DeviceDB'


# Set the context to your subscription
az account set --subscription $subscriptionId

# Define bicep path
$bicepTemplatePath = './resource_template.bicep'

# Function to deploy resources
function Deploy-Resources {
    param (
        [string]$rgName,
        [string]$databaseServerName,
        [string]$parameters
    )

    Write-Host "Deploying resources to $rgName"

    az group create -l $location -n $rgName

    $cmd = "az deployment group create --mode Incremental --resource-group $rgName --template-file $bicepTemplatePath --parameters $parameters"
    Write-Host $cmd
    Invoke-Expression $cmd
}

# Apply production parameters for bicep
$parametersProduction = @{
    storageAccountName = $storageAccountNameProduction
    databaseServerName = $databaseServerNameProduction
    serverFarmName = $serverFarmNameProduction
    functionAppName = $functionAppName
    administratorLogin = $administratorLogin
    administratorLoginPassword = $administratorLoginPassword
    metricsDBName = $metricsDBName
    firmwareDBName = $firmwareDBName
    notificationsDBName = $notificationsDBName
    deviceDBName = $deviceDBName
}

$parametersProduction = $parametersProduction.Keys.ForEach({"$_=$($parametersProduction[$_])"}) -join ' '
Deploy-Resources -rgName $resourceGroupNameProduction -databaseServerName $databaseServerNameProduction -parameters $parametersProduction # Execute production deployment


# Apply production parameters for bicep (no azure function needed here)
$parametersTest = @{
    storageAccountName = $storageAccountNameTest
    databaseServerName = $databaseServerNameTest
    serverFarmName = $serverFarmNameTest
    administratorLogin = $administratorLogin
    administratorLoginPassword = $administratorLoginPassword
    metricsDBName = $metricsDBName
    firmwareDBName = $firmwareDBName
    notificationsDBName = $notificationsDBName
    deviceDBName = $deviceDBName
}

$parametersTest = $parametersTest.Keys.ForEach({"$_=$($parametersTest[$_])"}) -join ' '
Deploy-Resources -rgName $resourceGroupNameTest -databaseServerName $databaseServerNameTest -parameters $parametersTest # Execute production deployment