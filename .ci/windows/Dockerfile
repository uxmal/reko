# escape=`

FROM cirrusci/windowsservercore:cmake

RUN powershell -Command "`
    choco install -y dotnet-5.0-sdk; `
    REG ADD 'HKLM\SOFTWARE\WOW6432Node\Microsoft\NET Framework Setup\NDP\v3.5' /t REG_DWORD /v Install /d 1; `
    REG ADD 'HKLM\SOFTWARE\WOW6432Node\Microsoft\NET Framework Setup\NDP\v3.5' /t REG_DWORD /v SP /d 1; `
    REG ADD 'HKLM\SOFTWARE\WOW6432Node\Microsoft\NET Framework Setup\NDP\v3.5' /t REG_SZ /v Version /d 3.5.30729.4926; `
    choco install -y -i wixtoolset; `
    Remove-Item $env:TEMP\* -recurse; `
    & $Env:ProgramFiles\dotnet\dotnet nuget locals all --clear; `
    & $Env:ProgramFiles\dotnet\dotnet nuget add source https://api.nuget.org/v3/index.json; `
    # reserved for cache eviction: 0
"
