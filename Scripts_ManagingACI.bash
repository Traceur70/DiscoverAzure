
#Creating a container
RESGROUP_NAME = "b8497fcf-b470-4d0f-bd94-d6b8280c6cf3"
CONTAINER_NAME = "acr-tasks"
ACR_NAME="NaddaContainerRegistry02"
ACC_NAME="naddacontainerregistry02" #Container registry name in lowercase
az acr create --resource-group $RESGROUP_NAME --name $ACR_NAME --sku Premium #Create container registry
code #Open code, then you have to write this content and save as "Dockerfile"
    # FROM    node:9-alpine
    # ADD     https://raw.githubusercontent.com/Azure-Samples/acr-build-helloworld-node/master/package.json /
    # ADD     https://raw.githubusercontent.com/Azure-Samples/acr-build-helloworld-node/master/server.js /
    # RUN     npm install
    # EXPOSE  80
    # CMD     ["node", "server.js"]
az acr build --registry $ACR_NAME --image helloacrtasks:v1 . #Build the image
az acr repository list --name $ACR_NAME --output table #List the image
az acr update -n $ACC_NAME --admin-enabled true #Add an admin to the container
az acr credential show --name $ACC_NAME #Display admin credentials
#Create container (you have to replace username and password)
az container create \
    --resource-group b8497fcf-b470-4d0f-bd94-d6b8280c6cf3 \
    --name acr-tasks1 \
    --image $ACR_NAME.azurecr.io/helloacrtasks:v1 \
    --registry-login-server $ACR_NAME.azurecr.io \
    --ip-address Public \
    --location eastus \
    --registry-username NaddaContainerRegistry02 \
    --registry-password "GwhygcwDCtR6j4bQzhsekd=Zs4qA5WVE"
az container show --resource-group b8497fcf-b470-4d0f-bd94-d6b8280c6cf3 --name acr-tasks1 --query ipAddress.ip --output table #Display IP adress
az acr replication create --registry $ACR_NAME --location japaneast #Replicate registry
az acr replication list --registry $ACR_NAME --output table #List all replicated registries

#Deploy an container hoosting a webapp
DNS_NAME_LABEL=aci-demo-$RANDOM
#Create container
az container create \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name mycontainer \
  --image microsoft/aci-helloworld \
  --ports 80 \
  --dns-name-label $DNS_NAME_LABEL \
  --location eastus
#Check container state
  az container show \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name mycontainer \
  --query "{FQDN:ipAddress.fqdn,ProvisioningState:provisioningState}" \
  --out table
#Create a container, run the script & shut down. The container is restarted in case of failure
az container create \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name mycontainer-restart-demo \
  --image microsoft/aci-wordcount:latest \
  --restart-policy OnFailure \
  --location eastus
#Check container state
az container show \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name mycontainer-restart-demo \
  --query containers[0].instanceView.currentState.state
  #Check container logs
  az container logs \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name mycontainer-restart-demo

#Deploy Azure Cosmoss db and use it from a container
COSMOS_DB_NAME=aci-cosmos-db-$RANDOM
#Create the db and get the endpoint
COSMOS_DB_ENDPOINT=$(az cosmosdb create \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name $COSMOS_DB_NAME \
  --query documentEndpoint \
  --output tsv)
#Get connexion key
COSMOS_DB_MASTERKEY=$(az cosmosdb list-keys \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name $COSMOS_DB_NAME \
  --query primaryMasterKey \
  --output tsv)
  #Create the container
  az container create \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name aci-demo \
  --image microsoft/azure-vote-front:cosmosdb \
  --ip-address Public \
  --location eastus \
  --environment-variables \
    COSMOS_DB_ENDPOINT=$COSMOS_DB_ENDPOINT \
    COSMOS_DB_MASTERKEY=$COSMOS_DB_MASTERKEY
#Get the public IP
az container show \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name aci-demo \
  --query ipAddress.ip \
  --output tsv
#Display env variable
az container show \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name aci-demo \
  --query containers[0].environmentVariables
#Create the container with secured env variables
az container create \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name aci-demo-secure \
  --image microsoft/azure-vote-front:cosmosdb \
  --ip-address Public \
  --location eastus \
  --secure-environment-variables \
    COSMOS_DB_ENDPOINT=$COSMOS_DB_ENDPOINT \
    COSMOS_DB_MASTERKEY=$COSMOS_DB_MASTERKEY

#Use data volume from container
STORAGE_ACCOUNT_NAME=mystorageaccount$RANDOM
#Create storage account
az storage account create \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name $STORAGE_ACCOUNT_NAME \
  --sku Standard_LRS \
  --location eastus
#Get connection string in an env variable, usable from other CLI's terminals
export AZURE_STORAGE_CONNECTION_STRING=$(az storage account show-connection-string \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name $STORAGE_ACCOUNT_NAME \
  --output tsv)
#Create a file's share in the storage account
az storage share create --name aci-share-demo
#Get key of storage account
STORAGE_KEY=$(az storage account keys list \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --account-name $STORAGE_ACCOUNT_NAME \
  --query "[0].value" \
  --output tsv)
  #Create a container to mount the folder '/aci/logs/' on the storage account
  az container create \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name aci-demo-files \
  --image microsoft/aci-hellofiles \
  --location eastus \
  --ports 80 \
  --ip-address Public \
  --azure-file-volume-account-name $STORAGE_ACCOUNT_NAME \
  --azure-file-volume-account-key $STORAGE_KEY \
  --azure-file-volume-share-name aci-share-demo \
  --azure-file-volume-mount-path /aci/logs/
  #Get the IP of the conatainer
  az container show \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name aci-demo-files \
  --query ipAddress.ip \
  --output tsv
  #List files of file volume 
  az storage file list -s aci-share-demo -o table
  #DL the file in script's storage
  az storage file download -s aci-share-demo -p 1554636877076.txt
  #Display file name
  cat 1554636877076.txt

  #Troubleshoot ACI
  #Create container
  az container create \
  --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 \
  --name mycontainer \
  --image microsoft/sample-aks-helloworld \
  --ports 80 \
  --ip-address Public \
  --location eastus
  #Display container's logs
  az container logs --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 --name mycontainer
  #Attach container (ctrl+C to disconnect)
  az container attach --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 --name mycontainer
  #Work inside container (CMD: ls -> list files, exit -> quit stop session)
  az container exec --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 --name mycontainer --exec-command /bin/sh
  #Get id of container instance
  CONTAINER_ID=$(az container show --resource-group 9784cc94-5c5e-4b83-8766-133a0a44c092 --name mycontainer --query id --output tsv)
  #Get CPU usage
  az monitor metrics list --resource $CONTAINER_ID --output table --metric CPUUsage
  #Get memory usage
  az monitor metrics list --resource $CONTAINER_ID --output table --metric MemoryUsage