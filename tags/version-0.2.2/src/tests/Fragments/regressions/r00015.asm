; r00015: odd stack layout by watcom

fn025C proc 
	push	si

l025D:
	push	di
	push	bp
	mov	bp,sp
	sub	sp,08
	mov	si,ax
	mov	di,dx

l0267:
	xlat	
	mov	word ptr [bp-08],bx

l026A:
	clc	
	mov	word ptr [bp-06],cx
	cmp	byte ptr [si],00
	jz	l0291

l0273:
	mov	al,[si+01]
	xor	ah,ah
	cmp	ax,0x003A
	jnz	l0291

l027D:
	test	dx,dx
	jz	l028C

l0281:
	mov	al,[si]
	mov	byte ptr [di+01],0x3A
	mov	byte ptr [di],al
	mov	byte ptr [di+02],ah

l028C:
	add	si,02
	jmp	l0298

l0291:
	test	di,di
	jz	l0298

l0295:
	mov	byte ptr [di],00

l0298:
	mov	word ptr [bp-02],si
	mov	word ptr [bp-04],si

l029E:
	xor	di,di

l02A0:
	cmp	byte ptr [si],00
	jz	l02DF

l02A5:
	mov	ax,si
	call	fn0E18
	mov	dx,ax
	cmp	ax,0x002E
	jnz	l02B6

l02B1:
	mov	di,si
	inc	si
	jmp	l02A0

l02B6:
	cmp	word ptr [0464],00
	jz	l02CB

l02BD:
	mov	bl,[si]
	xor	bh,bh
	mov	al,[bx+0467]
	and	al,01
	xor	ah,ah
	jmp	l02CD

l02CB:
	xor	ax,ax

l02CD:
	inc	si
	add	si,ax
	cmp	dx,0x5C
	jz	l02DA

l02D5:
	cmp	dx,0x2F
	jnz	l02A0

l02DA:
	mov	word ptr [bp-02],si
	jmp	l029E

l02DF:
	mov	bx,[bp-02]
	mov	cx,0x0081
	mov	dx,[bp-04]
	mov	ax,[bp-08]
	sub	bx,[bp-04]
	call	fn022B
	test	di,di
	jnz	l02F7

l02F5:
	mov	di,si

l02F7:
	mov	cx,0008
	mov	dx,[bp-02]
	mov	bx,di
	mov	ax,[bp-06]
	sub	bx,[bp-02]
	call	fn022B
	mov	cx,0004
	mov	ax,[bp+08]
	mov	bx,si
	mov	dx,di
	sub	bx,di
	call	fn022B
	mov	sp,bp
	pop	bp
	pop	di
	pop	si
	ret	0002

fn022B proc
	mov [0x300],ax
	mov [0x302],bx
	mov [0x304],cx
	mov [0x306],dx
	ret
	
fn0E18 proc
	mov [0x400],ax
	mov ax,0x32
	ret
	