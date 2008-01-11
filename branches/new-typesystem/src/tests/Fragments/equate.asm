foo = 0xA3

	mov bx,glob
	mov ax,foo[bx+4]
	mov dst,ax
	ret


glob  dw 0	
dst   dw 0