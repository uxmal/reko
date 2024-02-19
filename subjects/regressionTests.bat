@echo off 
rem This batch file wraps up regressionTests.exe as a convenience for developers
set Cfg=%1
if "%1"=="" (
    set Cfg=Release
)
..\src\tools\regressionTests\bin\%Cfg%\net8.0\regressionTests.exe ..\src -c %Cfg%
