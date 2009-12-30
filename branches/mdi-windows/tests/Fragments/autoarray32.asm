;;; autoarray32.asm
;;; Sets up an automatic array in the stack frame.

.i386

main proc
	sub esp,0x0C
	xor eax,eax
	xor ecx,ecx
	mov dword ptr [esp],1
	mov dword ptr [esp+4],2
	mov dword ptr [esp+8],3
	jmp lupe
loop_body:
	add eax,[esp+ecx*4]
	inc ecx
lupe:
	cmp ecx,3
	jle loop_body
	mov edi,[esp+0x10]
	mov [edi],eax
	add esp,0x0C
	ret
	endp
	