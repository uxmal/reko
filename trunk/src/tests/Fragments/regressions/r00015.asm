; r00015.asm

fn0800_0A01 proc
	mov	sp,0x06B8
	push	bx
	push	ax
	push	dx
	mov	dx,0x0800
	mov	ds,dx
	mov	dx,0x0887
	mov	ax,0x3D01
	int	0x21
	mov	bx,ax
	pop	ds
	pop	dx
	mov	si,dx
	cld	

l0A1C:
	lodsb	
	cmp	al,00
	jnz	l0A1C

l0800_0A21:
	mov	cx,si
	sub	cx,dx
	dec	cx
	mov	ah,0x40
	int	0x21
	pop	ax

l0A2B:
	push	ax
	mov	ax,0x00
	mov	dx,0x00FF
	call	fn136C
	pop	ax
	mov	ah,0x4C
	int	0x21
	
fn136C proc
	mov [0x300],ax
	mov [0x302],dx
	ret
