[CmdletBinding(PositionalBinding=$false)]
Param(
    [Parameter(Mandatory=$true)]
    [String]$Source,
    [String]$LLCPath = "C:\Users\Patrik\.rustup\toolchains\stable-x86_64-pc-windows-msvc\lib\rustlib\x86_64-pc-windows-msvc\bin\llc.exe",
    [String]$LinkPath = "C:\Program Files\Microsoft Visual Studio\2022\Professional\VC\Tools\MSVC\14.31.31103\bin\Hostx86\x64\link.exe",
    [String]$StdPath = "C:\Users\Patrik\Source\github\patriksvensson\mew\stdlib\target\release",
    [Parameter(ValueFromRemainingArguments)]
    [string[]]$Remaining
)

# Make sure the source file exist
if (-not(Test-Path -Path $Source -PathType Leaf)) {
    Throw "Error: Source file do not exist"
}

$OutputPath = [System.IO.Path]::GetDirectoryName($Source)
$ModuleName = [System.IO.Path]::GetFileNameWithoutExtension($Source)

# Compile
Write-Host ""
Write-Host "Compiling..." -ForegroundColor Yellow
$ByteCodePath = Join-Path $OutputPath "$ModuleName.bc"
dotnet run -- "$Source" -o "$OutputPath" --llvm $Remaining
if($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host ""
    Throw "An error occured during compilation"
}

# Compile LLVM IR
&$LLCPath -filetype=obj "$ByteCodePath"

# Link
Write-Host ""
Write-Host "Linking..." -ForegroundColor Yellow
$ObjPath = Join-Path $OutputPath "$ModuleName.obj"
$ExePath = Join-Path $OutputPath "$ModuleName.exe"
&$LinkPath "$ObjPath" "mewstd.dll.lib" /OUT:"$ExePath" `
    /DEFAULTLIB:libcmt /SUBSYSTEM:Console `
    /LIBPATH:"C:\Program Files\Microsoft Visual Studio\2022\Professional\VC\Tools\MSVC\14.31.31103\lib\x64" `
    /LIBPATH:"C:\Program Files (x86)\Windows Kits\10\Lib\10.0.22000.0\um\x64" `
    /LIBPATH:"C:\Program Files (x86)\Windows Kits\10\Lib\10.0.22000.0\ucrt\x64" `
    /LIBPATH:"$StdPath"

# Execute
Write-Host "Executing " -NoNewline -ForegroundColor Yellow
Write-Host "$ExePath" -ForegroundColor Gray -NoNewline
Write-Host "..." -ForegroundColor Yellow
&"$ExePath"

# Exit code
Write-Host
Write-Host "Exit code:" -ForegroundColor Green
Write-Host $LASTEXITCODE
