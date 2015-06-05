	.i86
	mov bx,[bx+02]
	cmp bx,ax
	jg zoz
	mov word ptr [bx+04],01
	jmp zaz
zoz: mov word ptr [bx+04],00
	mov ax,[bx+04]
	mov [bx+06],ax
	movzx eax, word ptr [bx+08]
	movsx ecx, word ptr [bx+0x0A]
	add eax,ecx
	mov  [bx+0x0C],eax
zaz:
	ret
