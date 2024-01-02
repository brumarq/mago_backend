# Define your variables
$resourceGroupName = "test-mago-backend"
$location = 'westeurope'
$functionAppName = "ProcessAggregationLogsTimer"
$subscriptionId = '8e7c7eb8-572a-4e7b-9a57-9efbf2706e4a'
$storageAccountName = 'testmagostorageacc'
$serverFarmName = 'test-mago-server-farm'
$keyVaultName = 'test-mago-secret-vault'

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


az functionapp config appsettings set --name $functionAppName --resource-group $resourceGroupName --settings METRICS_DB_CONNECTION_STRING_PYODBC=$secretValue
#func azure functionapp publish "ProcessAggregationLogsTimer" --no-build


# Zip the Python function app
$functionAppFolder = "../ProcessAggregationLogsTimer"

$publishZip = "publish.zip"
Compress-Archive -Path $functionAppFolder -DestinationPath $publishZip -Force

#Deploy
az functionapp deployment source config-zip --resource-group $resourceGroupName --name $functionAppName --src $publishZip

# Clean up - remove the temporary zip file (if any)
Remove-Item $publishZip -Force