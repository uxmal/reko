;;; A while loop, followed by a repeat loop.

	.i86
	xor	ax,ax
	jmp looptest
again:
	add ax,[bx]
	mov bx,[bx+2]
looptest:
	or  bx,bx
	jnz  again
	mov [0x300],ax
	
	xor ax,ax
again2:
	add ax,[si]
	mov si,[si+2]
	or si,si
	jnz  again2
	
	mov [0x302],ax
	ret
