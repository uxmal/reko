@echo off
mkdir bin
for /r %%i in (.) do if exist %%i\bin\nul call :sub %%i
robocopy bin \code\LaserType\LaserImage\reko-private /s /r:0 /w:0
exit /b /0

:sub
echo ::: %1
if exist %1\bin\debug\net6.0\nul copy /y %1\bin\debug\net6.0 bin
if exist %1\bin\debug\net6.0-windows\nul copy /y %1\bin\debug\net6.0-windows bin
if exist %1\bin\debug\net6.0\runtimes\nul copy /y /s %1\bin\debug\net6.0\runtimes bin\runtimes
exit /b 0

