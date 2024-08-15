@echo off
for /r %%i in (.) do if exist %%i\bin\nul call :sub %%i
del /q /s CMakeCache.txt
exit /b /0

:sub
echo ::: %1
rmdir /q /s %1\bin 2> nul
rmdir /q /s %1\obj 2> nul
exit /b 0

