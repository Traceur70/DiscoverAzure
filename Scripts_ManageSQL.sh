az account list #List all subscriptions
az account set --subscription "7ecd65ff-d9dd-48fb-9931-d73f1474d5b2" #Change subscription
az configure --defaults group=44229e73-7cac-414c-bbfd-40464a90189c sql-server="nadda-db01serv" #Load group and sql server name to avoid typing it each time

#MSSQL
az sql db list #List all databases
az sql db list | jq '[.[] | {name: .name}]' #List all databases name
az sql db show --name "nadda-db01" #Display details of a database
az sql db show --name "nadda-db01" | jq '{name: .name, maxSizeBytes: .maxSizeBytes, status: .status}' #Display some details of a database
az sql db show-connection-string --client sqlcmd --name "nadda-db01" #Display connection string of a database
sqlcmd -S tcp:nadda-db01serv.database.windows.net,1433 -d nadda-db01 -U "<user>" -P "<pass>" -N -l 30 #Connection à la base de données
    CREATE TABLE Drivers (DriverID int, LastName varchar(255), FirstName varchar(255), OriginCity varchar(255));
    SELECT name FROM sys.tables;
    INSERT INTO Drivers (DriverID, LastName, FirstName, OriginCity) VALUES (123, 'Zirne', 'Laura', 'Springfield');
    UPDATE Drivers SET OriginCity='Boston' WHERE DriverID=123;
    SELECT DriverID, OriginCity FROM Drivers;
    DELETE FROM Drivers WHERE DriverID=123;
    SELECT COUNT(*) FROM Drivers;

#PostgreSQL : création d'une db 20Go de stockage, capacité de calcul Gen 5 avec 1 cœur virtuel & période de conservation de 15 jours pour les sauvegardes des données
az postgres server create \
    --resource-group "3c9beedd-7cc7-4133-876a-7da4ebf87206" \
    --name "nadda-postgreserv-01" \
    --location "westeurope" \
    --admin-user "nadda" \
    --admin-password "P@ssw0rdP@ssw0rd" \
    --sku-name "B_Gen5_1" \
    --storage-size 20480 \
    --backup-retention 15 \
    --version 10

#PostgreSQL : configuration d'une règle de pare-feu
az postgres server firewall-rule create \
  --resource-group 3c9beedd-7cc7-4133-876a-7da4ebf87206 \
  --server "nadda-postgreserv-01" \
  --name AllowAll \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 255.255.255.255

#PostgreSQL : supression d'une règle de pare-feu
az postgres server firewall-rule delete \
  --name AllowAll \
  --resource-group 3c9beedd-7cc7-4133-876a-7da4ebf87206 \
  --server-name "<server-name>"


psql --host="nadda-postgreserv-01.postgres.database.azure.com" --username="nadda@nadda-postgreserv-01.postgres.database.azure.com" --dbname=postgres #Connect to database
    \l #Show all databases
    CREATE DATABASE "Adventureworks"; #Create database
    \c Adventureworks # Connect to database
    CREATE TABLE PEOPLE(NAME TEXT NOT NULL, AGE INT NOT NULL);
    INSERT INTO PEOPLE(NAME, AGE) VALUES ('Bob', 35);
    INSERT INTO PEOPLE(NAME, AGE) VALUES ('Sarah', 28);
    CREATE TABLE LOCATIONS(CITY TEXT NOT NULL, STATE TEXT NOT NULL);
    INSERT INTO LOCATIONS(CITY, STATE) VALUES ('New York', 'NY');
    INSERT INTO LOCATIONS(CITY, STATE) VALUES ('Flint', 'MI');
    SELECT * FROM PEOPLE;
    SELECT * FROM LOCATIONS;
    \? #pour obtenir de l’aide.
    \dt #pour lister les tables.

#### SECURING DATABASE ####
export ADMINLOGIN='nadda'
export PASSWORD='P@ssw0rdP@ssw0rd'
export SERVERNAME=server$RANDOM
export RESOURCEGROUP=6513caa0-6fae-4f72-a7a3-f776baa5e0a7
export LOCATION=$(az group show --name 6513caa0-6fae-4f72-a7a3-f776baa5e0a7 | jq -r '.location') # Set the location, we'll pull the location from our resource group.
export TCP_SERVERNAME=tcp:$SERVERNAME.database.windows.net,1433
az sql server create \
    --name $SERVERNAME \
    --resource-group $RESOURCEGROUP \
    --location $LOCATION \
    --admin-user $ADMINLOGIN \
    --admin-password "$PASSWORD"
az sql db create --resource-group $RESOURCEGROUP \
    --server $SERVERNAME \
    --name marketplaceDb \
    --sample-name AdventureWorksLT \
    --service-objective Basic
az sql db show-connection-string --client sqlcmd --name marketplaceDb --server $SERVERNAME | jq -r
sqlcmd -S $TCP_SERVERNAME -d marketplaceDb -U $ADMINLOGIN -P $PASSWORD -N -l 30

#Create a VM
az vm create \
  --resource-group $RESOURCEGROUP \
  --name appServer \
  --image UbuntuLTS \
  --size Standard_DS2_v2 \
  --generate-ssh-keys
ssh <X.X.X.X> #Connect to a VM
#Install mssqltoolssur la VM
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bash_profile
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
source ~/.bashrc
curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
curl https://packages.microsoft.com/config/ubuntu/16.04/prod.list | sudo tee /etc/apt/sources.list.d/msprod.list
sudo apt-get update
sudo ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev
sqlcmd -S tcp:server32424.database.windows.net,1433 -d marketplaceDb -U 'nadda' -P 'P@ssw0rdP@ssw0rd' -N -l 30
    EXECUTE sp_set_database_firewall_rule N'My Firewall Rule', '0.0.0.0', '255.255.255.255'
    GO
    EXECUTE sp_delete_database_firewall_rule N'Allow appServer database level rule';
    GO
    CREATE USER ApplicationUser WITH PASSWORD = 'P@ssw0rdP@ssw0rd';
    GO
    ALTER ROLE db_datareader ADD MEMBER ApplicationUser;
    ALTER ROLE db_datawriter ADD MEMBER ApplicationUser;
    GO
    DENY SELECT ON SalesLT.Address TO ApplicationUser;
    GO
    SELECT FirstName, LastName, EmailAddress, Phone FROM SalesLT.Customer;
    GO