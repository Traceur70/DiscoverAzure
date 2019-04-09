#Create & run an MVC ASP.NET Core project
dotnet new mvc --name BestBikeApp
cd BestBikeApp
dotnet run
#Configure & check a git user
git config --global user.name "nadda"
git config --global user.email "traceur_70@hotmail.com"
cat ~/.gitconfig
#Init a GIT repo on current folder
git init
git add .
git commit -m "Initial commit"
git remote add origin https://ftpNadda@nadda-bestbike.scm.azurewebsites.net:443/nadda-bestbike.git #Specify the remote repo
git remote -v #Check the remote repo
git push origin master #Push to master branch


#build a container for an app service
git clone https://github.com/MicrosoftDocs/mslearn-deploy-run-container-app-service.git #Get a sample of app services solution with a docker file
az acr build --registry "naddatst01" --image webimage . #Send the contant of current folder in the specified registry and execute docker file
az acr task create --registry "naddatst01" --name buildwebapp --image webimage --context https://github.com/MicrosoftDocs/mslearn-deploy-run-container-app-service.git --branch master #Create a task to automatically build a new image version when the GitHub repo is updated

#Define the remote repo for production slot
git remote add production https://ftpNadda@nadda-tst01.scm.azurewebsites.net:443/nadda-tst01.git


