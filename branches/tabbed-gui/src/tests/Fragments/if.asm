;;; A simple if-statement
	.i86
	cmp	ax,0
	jnz	nonzero
	mov	bx,0x1234
	jmp join
nonzero:
	mov bx,0x1211
join:
	mov [si],bx
	ret
