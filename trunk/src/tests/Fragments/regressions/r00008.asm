fn353C_3710:
	mov	word ptr [0x9A2E],0000
	mov	cx,0x0100
	mov	word ptr [0x9A26],cx
	push	0x5CE1
	pop	fs
	mov	si,0000

l353C_3725:
	mov	word ptr fs:[si],0xFFFF
	add	si,0x26
	dec	word ptr [0x9A26]
	jnz	l353C_3725

l353C_3733:
	ret	
	