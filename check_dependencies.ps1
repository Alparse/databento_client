# Check DLL dependencies
$exePath = "C:\Users\serha\source\repos\databento_alt\examples\LiveStreaming.Example\bin\Debug\net8.0"
$dllPath = Join-Path $exePath "databento_native.dll"

Write-Host "Checking dependencies for: $dllPath" -ForegroundColor Cyan
Write-Host ""

# Check if file exists
if (-not (Test-Path $dllPath)) {
    Write-Host "ERROR: DLL not found!" -ForegroundColor Red
    exit 1
}

# List all DLLs in the directory
Write-Host "DLLs in output directory:" -ForegroundColor Yellow
Get-ChildItem $exePath -Filter "*.dll" | ForEach-Object {
    $size = [math]::Round($_.Length / 1KB, 2)
    Write-Host "  $($_.Name) ($size KB)"
}
Write-Host ""

# Check if vcpkg bin is in PATH
Write-Host "Checking PATH for vcpkg bin..." -ForegroundColor Yellow
$paths = $env:PATH -split ';'
$vcpkgInPath = $paths | Where-Object { $_ -like "*vcpkg*bin*" }
if ($vcpkgInPath) {
    Write-Host "  vcpkg bin found in PATH: $vcpkgInPath" -ForegroundColor Green
} else {
    Write-Host "  vcpkg bin NOT in PATH" -ForegroundColor Red
    Write-Host "  This might be the issue - runtime DLL loading may fail" -ForegroundColor Yellow
}
Write-Host ""

# Check if Visual C++ runtime is available
Write-Host "Checking for Visual C++ Runtime..." -ForegroundColor Yellow
$vcRuntimeDlls = @(
    "vcruntime140.dll",
    "vcruntime140_1.dll",
    "msvcp140.dll"
)

foreach ($vcDll in $vcRuntimeDlls) {
    $found = Get-ChildItem $exePath -Filter $vcDll -ErrorAction SilentlyContinue
    if ($found) {
        Write-Host "  [OK] $vcDll found in output" -ForegroundColor Green
    } else {
        # Check if it's in System32
        $systemDll = Join-Path $env:SystemRoot "System32\$vcDll"
        if (Test-Path $systemDll) {
            Write-Host "  [OK] $vcDll found in System32" -ForegroundColor Green
        } else {
            Write-Host "  [MISSING] $vcDll not found" -ForegroundColor Red
        }
    }
}
