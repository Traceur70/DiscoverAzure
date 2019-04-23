
Set-ExecutionPolicy RemoteSigned
Install-Module -Name Az -AllowClobber #Install Azure
Import-Module Az


###PARAMS
$vmAdmCredentials = Get-Credential
$vmName="testazps-vm01"
$vmImageName="WindowsServer"
$subscriptionId="282ed50a-a15b-4e58-856c-02d8d4571fc8"
$groupName = "testazps"
$groupLocation = "westeurope"
$planName = "testazps-plan01"
$appName = "testazps-app01"
$appRepoUrl = "https://github.com/Azure-Samples/php-docs-hello-world"

Connect-AzAccount
Select-AzSubscription -Subscription $subscriptionId
Get-AzResourceGroup
Get-AzResourceGroup | Format-Table
New-AzResourceGroup -Name $groupName -Location $groupLocation
New-AzVm `
       -ResourceGroupName $groupName `
       -Name $vmName `
       -Credential $vmAdmCredentials `
       -Location $groupLocation 
Remove-AzVM ...
Start-AzVM ...
Stop-AzVM ...
Restart-AzVM ...
Update-AzVM ...
Get-AzVM  -Name $vmName -ResourceGroupName $groupName


#Sample create & use a VM
$vmAdmCredentials = Get-Credential
$vmName = "testvm-weu-nadda-01"
$groupName = "08a3a9fe-8d2e-4187-99ec-527441baeb2e"
$groupLocation = "westeurope"
$imageName="UbuntuLTS"
New-AzVm -ResourceGroupName $groupName -Name $vmName -Credential $vmAdmCredentials -Location $groupLocation -Image $imageName -OpenPorts 22
Get-AzResource -ResourceGroupName $vm.ResourceGroupName | ft
$vm = Get-AzVM -Name $vmName -ResourceGroupName $groupName
$vmIp=($vm | Get-AzPublicIpAddress).IpAddress
ssh "$($vmAdmCredentials.UserName)@$vmIp"

#Sample delete a VM
Stop-AzVM -Name $vm.Name -ResourceGroup $vm.ResourceGroupName
Remove-AzVM -Name $vm.Name -ResourceGroup $vm.ResourceGroupName
$vm | Remove-AzNetworkInterface –Force
Get-AzDisk -ResourceGroupName $vm.ResourceGroupName -DiskName $vm.StorageProfile.OSDisk.Name | Remove-AzDisk -Force
Get-AzVirtualNetwork -ResourceGroup $vm.ResourceGroupName | Remove-AzVirtualNetwork -Force
Get-AzNetworkSecurityGroup -ResourceGroup $vm.ResourceGroupName | Remove-AzNetworkSecurityGroup -Force
Get-AzPublicIpAddress -ResourceGroup $vm.ResourceGroupName | Remove-AzPublicIpAddress -Force