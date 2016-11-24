;;; r00011.asm
;;; Various nasty bit manipulations, using the high- and low-byte parts
;;; of x86 registers.

main proc
	call foo
	ret
	endp

;;; foo - No registers should be live in
foo proc
	mov	ax,0x4DE1
	mov	es,ax
	mov	bx,0x0FFF

the_loop:
	mov	al,bh
	inc	al
	mov	ah,bl
	and	ah,0x0F
	mul	ah
	shl	ax,0x04
	mov	al,bl
	and	al,0xF0
	and	ah,0x0F
	or	al,ah
	mov	byte ptr es:[bx],al
	dec	bx
	jns	the_loop


	ret
	endp	
