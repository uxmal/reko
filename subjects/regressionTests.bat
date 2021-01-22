@echo off 
set Cfg=%1
if "%1"=="" (
    set Cfg=Release
)
..\src\tools\regressionTests\bin\Debug\netcoreapp3.1\regressionTests.exe ..\src -c %Cfg%
