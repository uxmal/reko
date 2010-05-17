.i86
	mov	bx,[bx+04]
	mov cx,[bx+02]
	cmp cx,0
	jne zok

	mov word ptr [bx+06],0
	mov cx,[si]
	jmp wol
zok:
	mov word ptr [bx+06],1
	mov cx,[si]
wol:
	mov [0300],cx
