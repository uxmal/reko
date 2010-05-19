;;; Used to test derivation of integral types.

.i86

	mov eax,[bx]
	mov ecx,[si]
	and eax,0x0FF
	and ecx,0xFFFFFF00
	or  eax,ecx
	mov [bx+04],eax
	
	;;; Now do some character comparisons
	
	mov al,[bx+08]
	cmp al,0x30
	jb  nondigit
	cmp al,0x3A
	jnc nondigit
	mov al,01
	jmp skipit
nondigit:
	xor al,al
skipit:
	mov [bx+09],al
	
	;;; Now do some signed arithmetic
	
	mov ax,[bx+0x0A]
	imul word ptr [bx+0x0C]
	mov [bx+0x0E],ax
	mov [bx+0x10],dx
	ret

	