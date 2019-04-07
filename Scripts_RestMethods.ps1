[net.servicepointmanager]::SecurityProtocol = [net.securityprotocoltype]::Tls12
Invoke-RestMethod `
    -Headers @{"Content-Type"="application/json"; "x-functions-key"="PDBjK/goWx7pK0RdgIA3yG5uaiegmx4p4XQOBvosLiHsFMykjWb26w==" }  `
    -Uri "https://nadda-funct001.azurewebsites.net/api/find-bookmark2"
    -Body @{"id"= "Bang"} `
    -Method "POST"