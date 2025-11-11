# Check DLL dependencies using dumpbin if available, or use fallback method
$exePath = "C:\Users\serha\source\repos\databento_alt\examples\LiveStreaming.Example\bin\Debug\net8.0"
$dllPath = Join-Path $exePath "databento_native.dll"

Write-Host "Checking databento_native.dll dependencies..." -ForegroundColor Cyan
Write-Host ""

# Try to find dumpbin in common Visual Studio locations
$vsWhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$dumpbinPath = $null

if (Test-Path $vsWhere) {
    $vsPath = & $vsWhere -latest -property installationPath
    if ($vsPath) {
        # Try to find dumpbin in VS installation
        $possiblePaths = @(
            "$vsPath\VC\Tools\MSVC\*\bin\Hostx64\x64\dumpbin.exe",
            "$vsPath\VC\bin\dumpbin.exe"
        )
        foreach ($pattern in $possiblePaths) {
            $found = Get-ChildItem $pattern -ErrorAction SilentlyContinue | Select-Object -First 1
            if ($found) {
                $dumpbinPath = $found.FullName
                break
            }
        }
    }
}

if ($dumpbinPath) {
    Write-Host "Using dumpbin: $dumpbinPath" -ForegroundColor Green
    Write-Host ""
    & $dumpbinPath /dependents $dllPath
} else {
    Write-Host "dumpbin not found, trying alternative method..." -ForegroundColor Yellow
    Write-Host ""

    # Alternative: Check file size and timestamp
    $dll = Get-Item $dllPath
    Write-Host "File: $($dll.Name)"
    Write-Host "Size: $([math]::Round($dll.Length/1MB, 2)) MB"
    Write-Host "Modified: $($dll.LastWriteTime)"
    Write-Host ""

    # Check if it's a debug build
    Write-Host "Checking for debug build indicators..."
    $bytes = [System.IO.File]::ReadAllBytes($dllPath)
    $hasDebugInfo = $bytes | Select-String -Pattern "RSDS" -SimpleMatch -Quiet
    if ($hasDebugInfo) {
        Write-Host "  [INFO] This appears to be a DEBUG build" -ForegroundColor Yellow
    }

    Write-Host ""
    Write-Host "Common dependencies for databento_native.dll:"
    Write-Host "  - vcruntime140d.dll (Debug C++ Runtime)" -ForegroundColor Yellow
    Write-Host "  - msvcp140d.dll (Debug C++ STL)" -ForegroundColor Yellow
    Write-Host "  - ucrtbased.dll (Debug Universal CRT)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Checking if debug runtimes are available..." -ForegroundColor Cyan

    $debugRuntimes = @(
        "vcruntime140d.dll",
        "vcruntime140_1d.dll",
        "msvcp140d.dll",
        "ucrtbased.dll"
    )

    foreach ($rt in $debugRuntimes) {
        $inOutput = Test-Path (Join-Path $exePath $rt)
        $inSystem = Test-Path (Join-Path $env:SystemRoot "System32\$rt")

        if ($inOutput) {
            Write-Host "  [OK] $rt found in output directory" -ForegroundColor Green
        } elseif ($inSystem) {
            Write-Host "  [OK] $rt found in System32" -ForegroundColor Green
        } else {
            Write-Host "  [MISSING] $rt not found" -ForegroundColor Red
        }
    }
}
