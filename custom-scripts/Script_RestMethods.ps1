[net.servicepointmanager]::SecurityProtocol = [net.securityprotocoltype]::Tls12

$functKey = "jYTHYb4sbWIa3ESZsx/k6BdY2FDKZOwmMoB9o8C9OAzg9smOkXJ7gg=="
$functEndPoint = "https://naddafunct01.azurewebsites.net/api/add-bookmark?code=$functKey"
$funcParams = @{ "id"= "github123"; "url"= "https://www.github123.com" }
Invoke-RestMethod `
    -Headers @{"Content-Type"="application/json"; "x-functions-key"=$functKey }  `
    -Uri $functEndPoint `
    -Body ($funcParams|ConvertTo-Json) `
    -Method "POST"

