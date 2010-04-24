	.i86
	mov		cx,[bx]
	add		cx,2
	mov		ax,cx
	mov		cl,[bx+2]
	xor		cl,0x11
	or		cl,[bx+3]
	or		al,cl
	mov		bx,ax
