@echo off
rem Copies .NET assemblies and other shared libraries from the Reko solution into
rem the `generated\assemblies` folder so they are visible to the Electron application.

xcopy /d /y ..\Electron.Adapter\bin\debug\Reko.Gui.Electron.Adapter.dll generated\assemblies
xcopy /d /y ..\..\Drivers\CmdLine\bin\debug\*.dll generated\assemblies
xcopy /d /y ..\..\Drivers\CmdLine\bin\debug\*.xml generated\assemblies
xcopy /d /y ..\..\Drivers\CmdLine\bin\debug\*.config generated\assemblies
xcopy /d /y ..\..\Drivers\WindowsDecompiler\bin\Debug\Reko.Gui.dll generated\assemblies