;;; Regression test 00001: caused failure when value numbering.

.i86
	imul	dx,bx,0x24
	mov	bx,si
	mov	si,[bp+0x08]
	add	si,dx
	mov	ax,[si+0x0C]
	sub	ax,[bp+0xFF7E]
	movsx	eax,ax
	mov	dword ptr [bp+0xFF6A],eax
	mov	cx,0x0005

l0800_09DC:
	mov	edx,[bx+0x10]
	mov	dword ptr [bp+di-0x3C],edx
	mov	eax,[si+0x10]
	sub	eax,edx
	cdq	
	cmp	word ptr [bp+0xFF6A],00
	jz	l0800_09FD

l0800_09F4:
	idiv	dword ptr [bp+0xFF6A]
	mov	dword ptr [bp+di-0x78],eax

l0800_09FD:
	add	di,04
	add	si,04
	add	bx,04
	loop	l0800_09DC

l0800_0A08:
	ret	

