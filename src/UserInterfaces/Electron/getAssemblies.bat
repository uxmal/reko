@echo off
rem Copies .NET assemblies and other shared libraries from the Reko solution into
rem the `generated\assemblies` folder so they are visible to the Electron application.

xcopy /d /y ..\Electron.Adapter\bin\debug\Reko.Gui.Electron.Adapter.dll generated\assemblies
xcopy /d /y ..\..\Core\bin\debug\Reko.Core.dll generated\assemblies
