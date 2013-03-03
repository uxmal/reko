.i86

;;; tests to ensure that fall-throughs to a procedure are handled correctly.
main	proc
	call	fallthru
	push	di
	mov		ax,0x100
	call	trashdi
	mov		[0x300],ax
	pop		di
	endp
	
fallthru	proc
	mov	word ptr [di],0
	ret
	endp
	
trashdi	proc
	xor	di,di
	dec	ax
	ret
	endp

	