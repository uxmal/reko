;;; Sequence taken from code where AH register is preserved by a push AX/pop AX sequence.

.x86

main:
	call	fn06B9
	mov	al,ah
	call	fn06B9
	ret	

fn06B9:
	mov	dx,0331
	push	ax
	xor	ax,ax

l0800_06BF:
	dec	ah
	jz	06C8

l0800_06C3:
	in	al,dx
	test	al,40
	jnz	06BF

l0800_06C8:
	pop	ax
	dec	dx
	out	dx,al
	ret	
