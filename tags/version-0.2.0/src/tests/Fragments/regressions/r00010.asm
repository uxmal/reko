;;; r00010.asm 
;;; Incorrect copy propagation.

main proc
	call fn06FE
	ret
	endp
	
fn06FE proc
l06FE:
	mov	al,[si]
	shr	al,01
	shr	al,01
	shr	al,01
	shr	al,01
	add	al,0x30
	cmp	al,0x39
	jle	l0712

	add	al,07

l0712:
	mov	ah,[si]
	and	ah,0x0F
	add	ah,0x30
	cmp	ah,0x39
	jle	l0724

	add	ah,07

l0724:
	mov	word ptr [0x5427],ax
	mov	ah,0x40
	mov	bx,[0x5424]
	mov	cx,0004
	mov	dx,0x5427
	int	0x21
	inc	si
	dec	bp
	jnz	l06FE

	ret	
	endp
	