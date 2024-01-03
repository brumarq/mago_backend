# Define your variables
$resourceGroupName = "mago-backend"
$location = 'westeurope'
$functionAppName = "mago-azure-functions"
$subscriptionId = '8e7c7eb8-572a-4e7b-9a57-9efbf2706e4a'
$storageAccountName = 'magostorageacc'
$serverFarmName = 'mago-server-farm'
$keyVaultName = 'mago-secret-vault'

$bicepTemplatePath = './azure_function_template.bicep'

$parameters = @{
    storageAccountName = $storageAccountName
    serverFarmName = $serverFarmName
    functionAppName = $functionAppName
}

$parameters = $parameters.Keys.ForEach({"$_=$($parameters[$_])"}) -join ' '

Write-Host "Deploying azure function to $resourceGroupName"

# Set the context to your subscription
az account set --subscription $subscriptionId

# Create new resource group (if does not exist)
az group create -l $location -n $resourceGroupName

# Get secret from Key Vault
$secretValue = az keyvault secret show --vault-name $keyVaultName --name METRICS-DB-CONNECTION-STRING-PYODBC --query 'value' -o tsv
Write-Host $secretValue

# Deploy resources inside resource group
$cmd = "az deployment group create --mode Incremental --resource-group $resourceGroupName --template-file $bicepTemplatePath --parameters $parameters"
Write-Host $cmd
Invoke-Expression $cmd

# Add required settings
$azureFunctionTimezone = "Europe Standard Time"
az functionapp config appsettings set --name $functionAppName --resource-group $resourceGroupName --settings METRICS_DB_CONNECTION_STRING_PYODBC=$secretValue
az functionapp config appsettings set --name $functionAppName --resource-group $resourceGroupName --settings SCM_DO_BUILD_DURING_DEPLOYMENT=true
az functionapp config appsettings set --name $functionAppName --resource-group $resourceGroupName --settings WEBSITE_TIME_ZONE=$azureFunctionTimezone

# Deploy the azure function code
func azure functionapp publish $functionAppName