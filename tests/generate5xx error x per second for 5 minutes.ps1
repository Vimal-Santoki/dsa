# --- CONFIGURATION ---
$baseUrl = "http://localhost:5000"
$tokenEndpoint = "$baseUrl/connect/token"
$targetEndpoint = "$baseUrl/api/sort/BubbleSort"
$payload = '[64, 34, 25, 12, 22, 11, 90]'
$requestPerSecond= 10

# --- SETUP .NET HTTP CLIENT ---
Add-Type -AssemblyName System.Net.Http
$handler = New-Object System.Net.Http.HttpClientHandler
$handler.ServerCertificateCustomValidationCallback = { $true } # Bypass SSL errors
$client = New-Object System.Net.Http.HttpClient($handler)

# --- STEP 1: LOGIN ---
Write-Host "1. Getting Admin Token..." -NoNewline
try {
    $loginJson = '{"username":"admin","password":"password"}'
    $content = New-Object System.Net.Http.StringContent($loginJson, [System.Text.Encoding]::UTF8, "application/json")
    
    $response = $client.PostAsync($tokenEndpoint, $content).GetAwaiter().GetResult()
    
    if (-not $response.IsSuccessStatusCode) {
        throw "Login failed: $($response.StatusCode)"
    }
    
    $jsonString = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    # Parse JSON manually to be safe
    $tokenObj = $jsonString | ConvertFrom-Json
    $token = $tokenObj.accessToken
    
    if ([string]::IsNullOrWhiteSpace($token)) { throw "Token is empty! Response: $jsonString" }

    Write-Host " Success!" -ForegroundColor Green
    Write-Host "   Token: $token" -ForegroundColor DarkGray
}
catch {
    Write-Host " Failed! $($_.Exception.Message)" -ForegroundColor Red
    exit
}

# --- STEP 2: CONFIGURE AUTH HEADER ---
$client.DefaultRequestHeaders.Authorization = 
    New-Object System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $token)

# --- STEP 3: TRAFFIC LOOP ---
Write-Host "`n2. Sending Traffic ($requestPerSecond req/10s)..." -ForegroundColor Cyan

$endTime = (Get-Date).AddMinutes(5)
while ((Get-Date) -lt $endTime) {
    $batchStart = Get-Date
    
    1..$requestPerSecond | ForEach-Object {
        try {
            $content = New-Object System.Net.Http.StringContent($payload, [System.Text.Encoding]::UTF8, "application/json")
            $response = $client.PostAsync($targetEndpoint, $content).GetAwaiter().GetResult()
            
            if ($response.IsSuccessStatusCode) {
                Write-Host "." -NoNewline -ForegroundColor Green
            }
            elseif ($response.StatusCode -eq [System.Net.HttpStatusCode]::Unauthorized) {
                Write-Host "X" -NoNewline -ForegroundColor Red # 401
            }
            elseif ($response.StatusCode -eq [System.Net.HttpStatusCode]::Forbidden) {
                 Write-Host "F" -NoNewline -ForegroundColor Magent # 403
            }
            else {
                Write-Host "!" -NoNewline -ForegroundColor Yellow # Other errors
            }
        }
        catch {
             Write-Host "E" -NoNewline -ForegroundColor Red
        }
    }

    # Wait logic
    $elapsed = ((Get-Date) - $batchStart).TotalSeconds
    $sleep = 10 - $elapsed
    if ($sleep -gt 0) { Start-Sleep -Seconds $sleep }
}
Write-Host "`nDone."