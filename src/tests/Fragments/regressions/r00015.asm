main proc
	push 0
	push 1
	push 2
	call fn0800_2E62
	ret
	
proc fn0800_2E62
	push	si
	push	di
	push	bp
	mov	bp,sp
	sub	sp,0x08
	mov	si,ax
	mov	word ptr [bp-0x04],bx
	mov	word ptr [bp-0x08],cx
	test	dx,dx
	jnz	l2E7A

l2E76:
	mov	dx,[0x0568]

l2E7A:
	mov	word ptr [bp-0x02],0x0000
	test	dx,dx
	jz	l2E97

l2E83:
	mov	bx,dx
	jmp	l2E92

l2E87:
	mov	ax,[bx]
	call	fn0589
	inc	ax
	inc	bx
	inc	bx
	add	word ptr [bp-0x02],ax

l2E92:
	cmp	word ptr [bx],0x00
	jnz	l2E87

l2E97:
	inc	word ptr [bp-0x02]
	cmp	word ptr [bp+0x0C],0x00
	jz	l2EAB

l2EA0:
	mov	ax,[si]
	call	fn0589
	add	ax,0x0003
	add	word ptr [bp-0x02],ax

l2EAB:
	mov	ax,[0x0574]
	add	word ptr [bp-0x02],0x0F
	mov	word ptr [bp-0x06],ax
	mov	ax,[bp-0x02]
	mov	word ptr [0x0574],0x0010
	call	fn1004
	test	ax,ax
	jnz	l2EE9

l2EC5:
	mov	ax,[bp-0x02]
	call	fn1004
	mov	bx,ax
	test	ax,ax
	jnz	l2EEB

l2ED1:
	mov	ax,0x0005
	call	fn2A7F
	mov	ax,0x0008
	call	fn2A9C
	mov	ax,[bp-0x06]
	mov	word ptr [0x0574],ax

l2EE3:
	mov	ax,0xFFFF
	jmp	l2F84

l2EE9:
	mov	bx,ax

l2EEB:
	mov	ax,[bp-0x06]
	mov	di,[bp-0x04]
	mov	word ptr [0x0574],ax
	mov	word ptr [di],bx
	add	bx,0x0F
	and	bl,0xF0
	mov	cl,0x04
	mov	di,bx
	shr	di,cl
	mov	ax,ds
	mov	cx,di
	mov	di,[bp+0x08]
	add	cx,ax
	mov	word ptr [di],cx
	mov	di,[bp-0x08]
	mov	word ptr [di],bx
	test	dx,dx
	jz	l2F2B

l2F16:
	mov	di,dx
	jmp	l2F26

l2F1A:
	mov	ax,bx
	mov	dx,[di]
	call	fn2E4B
	mov	bx,ax
	inc	di
	inc	di
	inc	bx

l2F26:
	cmp	word ptr [di],0x00
	jnz	l2F1A

l2F2B:
	mov	byte ptr [bx],0x00
	inc	bx
	cmp	word ptr [bp+0x0C],0x00
	jz	l2F3D

l2F35:
	lea	ax,[bx+0x02]
	mov	dx,[si]
	call	fn0517

l2F3D:
	xor	dx,dx
	cmp	word ptr [si],0x00
	jz	l2F5A

l2F44:
	inc	si
	inc	si
	jmp	l2F54

l2F48:
	test	dx,dx
	jz	l2F4D

l2F4C:
	inc	dx

l2F4D:
	call	fn0589
	inc	si
	inc	si
	add	dx,ax

l2F54:
	mov	ax,[si]
	test	ax,ax
	jnz	l2F48

l2F5A:
	cmp	dx,0x7E
	jbe	l2F76

l2F5F:
	mov	ax,0x0002
	call	fn2A7F
	mov	ax,0x000A
	mov	bx,[bp-0x04]
	call	fn2A9C
	mov	ax,[bx]
	call	fn1174
	jmp	ll2EE3

l2F76:
	mov	bx,[bp+0x0A]
	mov	cl,0x04
	mov	ax,[bp-0x02]
	mov	word ptr [bx],0x0090
	shr	ax,cl

l2F84:
	mov	sp,bp
	pop	bp
	pop	di
	pop	si
	ret	0x0006
