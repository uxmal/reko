all: $(Configuration) \
	$(Configuration)\fib.exe \
	$(Configuration)\main.exe \
	$(Configuration)\driver.dll \
	$(Configuration)\delayload.exe \
        $(Configuration)\win32api.exe 

clean:
	-@del /q $(Configuration)\*

$(Configuration):
	-md $(Configuration)
		
$(Configuration)\fib.exe: fib\fib.c makefile
	cd
	cl /nologo /c /FAc /Fa$(Configuration)\fib.cod /GS- /Ox /Fo$(Configuration)\fib.obj fib\fib.c
	link /nologo /ENTRY:WinMain /NODEFAULTLIB /SUBSYSTEM:WINDOWS /OUT:$(Configuration)\fib.exe $(Configuration)\fib.obj

$(Configuration)\driver.dll: exe_dll\driver.cpp exe_dll\types.h 
	cl /nologo /c /FAc /Fa$(Configuration)\driver.cod /GS- /Ox /Fo$(Configuration)\main.obj exe_dll\driver.cpp
	link /nologo /DLL /OUT:$(Configuration)\driver.dll $(Configuration)\main.obj 

$(Configuration)\main.exe: exe_dll\main.cpp exe_dll\types.h $(Configuration)\driver.dll
	cl /nologo /c /FAc /Fa$(Configuration)\main.cod /GS- /Ox /Fo$(Configuration)\main.obj exe_dll\main.cpp
	link /nologo /OUT:$(Configuration)\main.exe $(Configuration)\main.obj $(Configuration)\driver.lib

$(Configuration)\delayload.exe: delayload\delayload.cpp
	cl /nologo /c /FAc /Fa$(Configuration)\delayload.cod /GS- /Ox /Fo$(Configuration)\delayload.obj delayload\delayload.cpp
	link /nologo /NOD /Entry:WinMain /DELAYLOAD:user32.dll delayimp.lib /OUT:$(Configuration)\delayload.exe \
	 $(Configuration)\delayload.obj \
	 kernel32.lib \
	 user32.lib

$(Configuration)\win32api.exe: win32api\win32api.cpp
	cl /nologo /c /FAc /Fa$(Configuration)\win32api.cod /GS- /Ox /Fo$(Configuration)\win32api.obj win32api\win32api.cpp
	link /nologo /NOD /Entry:WinMain /OUT:$(Configuration)\win32api.exe \
	 $(Configuration)\win32api.obj \
	 kernel32.lib \
	 user32.lib
  