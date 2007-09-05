;;; r00009.asm
;;; Tests value propagation errors when stack pushes are involved.
main proc
	mov	bx,[0x9CFD]
	add	bx,bx
	mov	dx,[bx+0x99F8]
	mov	word ptr [0x9CFF],dx
	mov	cx,0x0180
	mov	bx,[0x99D0]
	push	ds
	push	0x6041
	pop	ds
	mov	ah,0x3F
	int	0x21
	pop	ds
	mov	bx,[0x99D0]
	mov	ah,0x3E
	int	0x21
	mov	bx,[0x9CFD]
	add	bx,bx
	mov	ax,[bx+0x542E]
	mov	word ptr [0xC36A],ax
	mov	es,ax
	push	0x6041
	pop	gs
	mov	bx,0x0FFF

	; "tie to memory" to force the segment registers to be live.
	mov di,ds
	mov [0x0100],di
	mov di,gs
	mov [0x0102],di
	ret
	endp
