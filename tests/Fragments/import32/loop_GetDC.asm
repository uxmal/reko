;;; Fragments that exhibits the same behaviour as Visual C++ 7.0
;;; The address of an imported function heavily used in a loop
;;; is moved into a register. GetDC cleans up its stack, so we should
;;; get no warning when we leave the function.
.i386
main proc
	mov edi,[g_apFoo]
	mov esi,3
	mov ebx,dword ptr [_GetDC]
	jmp ltest
lbody:
	push dword ptr [edi]
	call ebx
	mov [edi],eax
	push dword ptr 0
	call ebx
	mov [edi+4],eax
	dec esi
	add edi,8
ltest
	or esi,esi
	jnz lbody	
	ret
main endp

g_apFoo dd 0,0,0,0, 0, 0, 0, 0, 0

_GetDC .import 'GetDC','user32.dll'
