	.i86
	mov	ax,[bx+4]
	imul ax,ax
	mov [bx+6],ax

	lea bx,[si+2]
	mov [bx+1234],bx

	xor ax,ax		;; so ax and flags are not live out
	ret
