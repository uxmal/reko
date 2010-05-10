	.i386

_malloc .import	'malloc', 'msvcrt.dll'
_free	.import 'free',   'msvcrt.dll'

main proc
	push 8
	call dword ptr [_malloc]
	add esp,4
	
	mov dword ptr [eax],0
	mov dword ptr [eax+4],0x1024
	
	push eax
	call dword ptr [_free]
	add esp,4
	ret
main	endp
