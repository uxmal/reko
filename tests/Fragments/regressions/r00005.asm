.i86

driver proc
	call fn7C80
	mov [0x300],bx
	mov [0x302],cl
	ret
	endp
	
fn7C80 proc
	std	
	mov	cx,ds
	mov	es,cx
	lea	di,[0x7E0F]
	mov	cx,0x000C
	xor	al,al

l7C8E:
	rep	
	scasb	

l7C90:
	cld	
	jz	l7CA3

l7C93:
	mov	ah,[di+01]
	bsr	cx,ax
	sub	cx,0x08
	mov	bx,di
	sub	bx,0x7E03
	ret	

l7CA3:
	xor	bx,bx
	xor	cl,cl
	ret	
	endp
