	.i86
	push	cx
	mov		cl,3
	xor		ax,ax
loopf:
	add		al,cl
	dec		cl
	jnz		loopf
	pop		cx
	ret
