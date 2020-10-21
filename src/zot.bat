@echo off
git clean -Xdf .
nuget restore
msbuild /v:m /t:Rebuild /p:Configuration=Debug /p:Platform=x64 > build.log

