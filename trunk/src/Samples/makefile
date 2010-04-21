all: $(Configuration) $(Configuration)\fib.exe

clean:
	-@del /q $(Configuration)\*

$(Configuration):
	-md $(Configuration)
		
$(Configuration)\fib.exe: fib\fib.c makefile
	cd
	cl /nologo /c /FAc /Fa$(Configuration)\fib.cod /GS- /Ox /Fo$(Configuration)\fib.obj fib\fib.c
	link /nologo /ENTRY:WinMain /NODEFAULTLIB /SUBSYSTEM:WINDOWS /OUT:$(Configuration)\fib.exe $(Configuration)\fib.obj


