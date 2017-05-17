
.i386
main proc
	mov edi,0x10001212
	fld qword ptr [edi+0x04]
	fld qword ptr [edi+0x0C]
	call dword ptr [_not_found_import]
	mov [edi+0x14],eax
	call dword ptr [_not_found_import]
	mov [edi+0x1C],eax
	ret
main endp

_not_found_import	.import '_not_found_import','msvcrt.dll'
