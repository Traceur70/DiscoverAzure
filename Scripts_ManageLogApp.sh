gitRepo=https://github.com/MicrosoftDocs/mslearn-capture-application-logs-app-service
appName="contosofashions$RANDOM"
appPlan="contosofashionsAppPlan"
appLocation=francecentral
resourceGroup=4fba62c3-e859-4cdb-b2cd-69c4456dfd1a
storageAccount=sa$appName
appUserName="naddaApp"
appUserPass="ET3ET2ET1"

az appservice plan create --name $appPlan --resource-group $resourceGroup --location $appLocation --sku FREE #Create a new service plan
az webapp create --name $appName --resource-group $resourceGroup --plan $appPlan --deployment-source-url $gitRepo #Create a new webapp linked to a GIT repo
#Create a storage account
az storage account create \
    -n $storageAccount \
    -g $resourceGroup \
    -l $appLocation \
    --kind StorageV2 \
    --access-tier Hot \
    --sku Standard_LRS 




az webapp log tail --name $appName --resource-group $resourceGroup #Display log in streaming
az webapp deployment user set --user-name $appUserName --password $appUserPass #Create a new user's credential
curl -u $appUserName https://$appName.scm.azurewebsites.net/api/logstream #Get log stream from curl
az webapp log download --log-file "./extract.zip"  --resource-group $resourceGroup --name $appName
unzip -j "./extract.zip" LogFiles/Application/*.txt #Extract specific files
