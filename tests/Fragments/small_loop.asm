	.i86
	xor	ax,ax
	mov	cx,[bx]
spin:
	add ax,cx
	add	ax,cx
	loop	spin

	mov [0x300],ax
	ret
