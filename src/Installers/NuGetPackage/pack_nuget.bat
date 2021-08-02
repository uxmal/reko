@echo off
rem Builds the Reko solution and creates a NuGet package using the nuget
rem spec in this directory.
set CONFIG=Release
if NOT "%1" == "" (
    set CONFIG=%1
)
pushd ..
msbuild /nologo /v:m /m /p:Configuration=%CONFIG% /p:Platform=x64
popd
nuget pack -Properties Configuration=%CONFIG%;Platform=x64
