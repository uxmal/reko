;;; Test for calls to _ftol, which pops a float from the FPU stack.

.i386
main proc
	mov edi,0x10001212
	fld qword ptr [edi+0x04]
	fld qword ptr [edi+0x0C]
	call dword ptr [_ftol]
	mov [edi+0x14],eax
	call dword ptr [_ftol]
	mov [edi+0x1C],eax
	ret
main endp

_ftol	.import '_ftol','msvcrt.dll'
