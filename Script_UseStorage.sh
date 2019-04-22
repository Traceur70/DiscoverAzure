export STORAGEACCOUNT="naddatest001"
export RESOURCEGROUP="66db4b3a-582c-42d4-8757-972c36ec356b"
export APPLOCATION="westeurope"
export STORAGEACCOUNT_RESTENDPOINT_BLOB="https://$STORAGEACCOUNT.blob.core.windows.net/"
export STORAGEACCOUNT_RESTENDPOINT_QUEUE="https://$STORAGEACCOUNT.queue.core.windows.net/"
export STORAGEACCOUNT_RESTENDPOINT_TABLE="https://$STORAGEACCOUNT.table.core.windows.net/"
export STORAGEACCOUNT_RESTENDPOINT_FILE="https://$STORAGEACCOUNT.file.core.windows.net/"
export APPNAME="ConsoleApp01"

#create storage
az storage account create \
    -n $STORAGEACCOUNT \
    -g $RESOURCEGROUP \
    -l $APPLOCATION \
    --kind StorageV2 \
    --access-tier Hot \
    --sku Standard_LRS 

dotnet new console --name $APPNAME #Create & run an MVC ASP.NET Core project
cd $APPNAME
dotnet add package WindowsAzure.Storage #Add package to call storage account
dotnet run #Run the project
#Display connection string
export STORAGE_CONNSTRING="$(az storage account show-connection-string \
    --resource-group $RESOURCEGROUP \
    --name $STORAGEACCOUNT \
    --query connectionString)"
touch appsettings.json #Create file 'appsettings.json'
echo "{\"StorageAccountConnectionString\": $STORAGE_CONNSTRING}" >> appsettings.json  #Specify conn string in the appsettings

##############
##Add these tags in ....csproj : 
##    Project->ItemGroup->   <None Update="appsettings.json"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>
##    Project->PropertyGroup-> <LangVersion>7.1</LangVersion>
##############

dotnet add package Microsoft.Extensions.Configuration.Json
echo "using Microsoft.WindowsAzure.Storage;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApp01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(\"appsettings.json\");

            var configuration = builder.Build();
            var connectionString = configuration[\"StorageAccountConnectionString\"];

            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount))
            {
                Console.WriteLine(\"Unable to parse connection string\");
            }
            else
            {
                var blobClient = storageAccount.CreateCloudBlobClient();
                var blobContainer = blobClient.GetContainerReference(\"photoblobs\");
                bool created = await blobContainer.CreateIfNotExistsAsync();
                Console.WriteLine(created ? \"Created the Blob container\" : \"Blob container already exists.\");
            }
        }
    }
}">Program.cs

dotnet run #Test the program


################# Develop a site to access blobs storage
#Create storage
export STORAGENAME="naddastoragetst01"
export APPNAME="naddastoragetst01-webapp"
export LOCATION="westeurope"
export RESGROUPID="1d4e5a2d-24c2-4ffe-afd5-316c6db201d3"
az storage account create \
  --kind StorageV2 \
  --resource-group $RESGROUPID \
  --location $LOCATION \
  --name $STORAGENAME

#Get sources
git clone https://github.com/MicrosoftDocs/mslearn-store-data-in-azure.git
cd mslearn-store-data-in-azure/store-app-data-with-azure-blob-storage/src/final
dotnet add package WindowsAzure.Storage
dotnet restore

#Configure new webapp
az appservice plan create --name blob-exercise-plan --resource-group $RESGROUPID
az webapp create --name $APPNAME --plan blob-exercise-plan --resource-group $RESGROUPID
CONNECTIONSTRING=$(az storage account show-connection-string --name $STORAGENAME --output tsv)
az webapp config appsettings set --name $APPNAME --resource-group $RESGROUPID --settings AzureStorageConfig:ConnectionString=$CONNECTIONSTRING AzureStorageConfig:FileContainerName=files

#Publish Webapp
dotnet publish -o pub
cd pub
zip -r ../site.zip *
az webapp deployment source config-zip --src ../site.zip --name $APPNAME --resource-group $RESGROUPID