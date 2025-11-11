# Verify the Release DLL dependencies
$exePath = "C:\Users\serha\source\repos\databento_alt\examples\LiveStreaming.Example\bin\Debug\net8.0"
$dllPath = Join-Path $exePath "databento_native.dll"
$dumpbin = "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Tools\MSVC\14.43.34808\bin\Hostx64\x64\dumpbin.exe"

Write-Host "Checking databento_native.dll in output directory..." -ForegroundColor Cyan
Write-Host ""

$dll = Get-Item $dllPath
$sizeKB = [math]::Round($dll.Length / 1KB, 2)
Write-Host "File size: $sizeKB KB"
Write-Host "Modified: $($dll.LastWriteTime)"
Write-Host ""

Write-Host "Dependencies:" -ForegroundColor Yellow
& $dumpbin /dependents $dllPath | Select-String -Pattern "^\s+\w+.*\.dll$"
