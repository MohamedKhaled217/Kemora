# Kemora Secrets Setup Script
# Use this script to securely configure local development secrets without exposing them in Git.

$ProjectDir = "$PSScriptRoot\Kemora.Api"

if (-not (Test-Path $ProjectDir)) {
    Write-Error "Could not find Kemora.Api directory at $ProjectDir"
    exit
}

Write-Host "--- Kemora Local Secret Configuration ---" -ForegroundColor Cyan

# Initialize User Secrets
Push-Location $ProjectDir
dotnet user-secrets init
Pop-Location

# Function to safely set a secret
function Set-KemoraSecret($Key, $Value) {
    if ([string]::IsNullOrWhiteSpace($Value)) {
        Write-Host "Skipping empty value for $Key" -ForegroundColor Yellow
        return
    }
    Push-Location $ProjectDir
    dotnet user-secrets set "$Key" "$Value"
    Pop-Location
}

# Values - REPLACE THESE with the actual keys before sharing this script securely.
$TokenKey = "YOUR_SECURE_TOKEN_KEY"
$GeminiKey = "YOUR_GEMINI_API_KEY"
$SmtpPass = "YOUR_SMTP_PASSWORD"

Set-KemoraSecret "TokenKey" $TokenKey
Set-KemoraSecret "Gemini:ApiKey" $GeminiKey
Set-KemoraSecret "EmailSettings:Password" $SmtpPass

Write-Host "`n[SUCCESS] Secrets have been stored in your machine's secure local store." -ForegroundColor Green
Write-Host "You can now safely delete the values from this script or the script itself." -ForegroundColor Gray
