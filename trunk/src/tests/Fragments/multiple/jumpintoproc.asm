.i86

main proc 

	mov bx,0x1234
	call the_proc
	mov bx,0x5678
	call inter

main endp



the_proc proc
	cmp word ptr [bx],123
	jle middle
	xor ax,ax
	ret

middle:
	mov ax,[bx]
	inc ax
	ret
the_proc endp

inter proc
	add bx,04
	jmp middle
inter endp
