;;; Tests constant propagation.

main proc
	xor ax,ax
	mov [0x300],ax
	mov [0x302],ax
	mov [0x304],ax
	ret
	endp
