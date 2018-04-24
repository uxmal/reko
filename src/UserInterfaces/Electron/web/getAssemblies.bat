@echo off
rem Copies .NET assemblies and other shared libraries from the Reko solution into
rem the `generated\assemblies` folder so they are visible to the Electron application.

xcopy /d /y ..\..\..\Drivers\CmdLine\bin\x64\Debug\*.dll generated\assemblies
xcopy /d /y ..\..\..\Drivers\CmdLine\bin\x64\Debug\*.xml generated\assemblies
xcopy /d /y ..\..\..\Drivers\CmdLine\bin\x64\Debug\*.config generated\assemblies
xcopy /d /y ..\..\..\Drivers\WindowsDecompiler\bin\x64\Debug\Reko.Gui.dll generated\assemblies
xcopy /d /y ..\..\Electron.Adapter\bin\Debug\Reko.Gui.Electron.Adapter.dll generated\assemblies

rem pause if ran by doubleclick
echo.%cmdcmdline% | find /I "%~0" >nul
if not errorlevel 1 pause
