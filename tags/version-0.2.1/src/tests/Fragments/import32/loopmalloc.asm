;;; Fragments that exhibits the same behaviour as Visual C++ 7.0
;;; The address of an imported function heavily used in a loop
;;; is moved into a register.
.i386
main proc
	mov edi,offset g_apFoo
	mov esi,3
	mov ebx,dword ptr [_malloc]
	jmp ltest
lbody:
	push 8
	call ebx
	add esp,4
	mov dword ptr [eax],0
	mov dword ptr [eax+4],0
	mov [edi],eax
	dec esi
	add edi,4
ltest
	or esi,esi
	jnz lbody	
	ret
main endp

g_apFoo dd 0,0,0,0

_malloc	.import 'malloc','msvcrt.dll'
