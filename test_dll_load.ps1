# Simple test to verify DLL dependencies
Write-Host "Testing Databento.NET DLL Loading..." -ForegroundColor Cyan

$exePath = "C:\Users\serha\source\repos\databento_alt\examples\LiveStreaming.Example\bin\Debug\net8.0"
Write-Host "Checking output directory: $exePath"

$requiredDlls = @(
    "databento_native.dll",
    "libcrypto-3-x64.dll",
    "libssl-3-x64.dll",
    "zstd.dll",
    "zlib1.dll",
    "legacy.dll",
    "Databento.Interop.dll",
    "Databento.Client.dll"
)

$allPresent = $true
foreach ($dll in $requiredDlls) {
    $path = Join-Path $exePath $dll
    if (Test-Path $path) {
        $size = (Get-Item $path).Length
        $sizeKB = [math]::Round($size/1KB, 2)
        Write-Host "[OK] $dll ($sizeKB KB)" -ForegroundColor Green
    } else {
        Write-Host "[MISSING] $dll" -ForegroundColor Red
        $allPresent = $false
    }
}

if ($allPresent) {
    Write-Host "`nAll dependencies present!" -ForegroundColor Green
    Write-Host "You can now run the example from Visual Studio" -ForegroundColor Yellow
} else {
    Write-Host "`nSome dependencies are missing!" -ForegroundColor Red
}
