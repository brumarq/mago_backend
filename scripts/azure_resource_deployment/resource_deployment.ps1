# Variables for production environment
$location = 'westeurope'
$functionAppName = 'mago-azure-functions'
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

# Function for deploying Azure function
function Deploy-AzureFunction {
    param (
        [string]$functionAppName,
        [string]$resourceGroupName,
        [string]$keyVaultName,
        [string]$azureFunctionTimezone
    )

    # Get secret value from Key Vault
    $connectionString = az keyvault secret show --vault-name $keyVaultName --name METRICS-DB-CONNECTION-STRING-PYODBC --query 'value' -o tsv

    # Set Azure Function app settings
    Set-AzWebApp -ResourceGroupName $resourceGroupName -Name $functionAppName -AppSettings @{
        'METRICS_DB_CONNECTION_STRING_PYODBC' = $connectionString
        'SCM_DO_BUILD_DURING_DEPLOYMENT' = 'true'
        'WEBSITE_TIME_ZONE' = $azureFunctionTimezone
    }

    # Get path to the project directory
    $azureFunctionProjectPath = Join-Path $PSScriptRoot "ProcessAggregationLogsTimer"

    # Publish the Azure Function code
    Publish-AzWebapp -ResourceGroupName $resourceGroupName -Name $functionAppName -ArchivePath $azureFunctionProjectPath
}

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

    # Only if its production, then do the Azure function deployment
    if ($rgName -eq $resourceGroupNameProduction) {
        Deploy-AzureFunction -functionAppName $functionAppName -resourceGroupName $rgName -keyVaultName $keyVaultName -azureFunctionTimezone "Europe Standard Time"
    }
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
Deploy-Resources -rgName $resourceGroupNameTest -databaseServerName $databaseServerNameTest -parameters $parametersTest # Execute test deployment
