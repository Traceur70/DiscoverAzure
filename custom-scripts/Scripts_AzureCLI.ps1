#Download Azure CLI at https://aka.ms/installazurecliwindows

##PARAMS
$groupName = "testazcli"
$groupLocation = "westeurope"
$planName = "testazcli-plan01"
$appName = "testazcli-app01"
$appRepoUrl = "https://github.com/Azure-Samples/php-docs-hello-world"
###

#Generic
az --version #Check version
az find blob #Find commands related to "blob"
az find 'az vm create'#Find commands related to "az vm create"
az storage blob --help #How to use the command "storage blob"
az login #Login to Azure

#Manage groups
az group create --name $groupName --location $groupLocation
az group list
az group list --output table
az group list --query "[?name == '$groupName']"

#Manage services
az appservice plan create --name $planName --resource-group $groupName --location $groupLocation
az appservice plan list --output table
az webapp create --name $appName --resource-group $groupName --plan $planName
az webapp list --output table
az webapp deployment source config --name $appName --resource-group $groupName --repo-url $appRepoUrl --branch master --manual-integration




az webapp create --name popupwebapp-41564 --resource-group 8f51d3ec-63d4-465d-bccc-35bf2feb9160 --plan popupappplan-12345698
az webapp deployment source config --name popupwebapp-41564 --resource-group 8f51d3ec-63d4-465d-bccc-35bf2feb9160 --repo-url "https://github.com/Azure-Samples/php-docs-hello-world" --branch master --manual-integration