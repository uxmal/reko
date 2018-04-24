call getAssemblies.bat
set MAIN_FILE=.\generated\bridge\index.html
rem start %~dp0\node_modules\.bin\electron %MAIN_FILE%
electron %MAIN_FILE%
