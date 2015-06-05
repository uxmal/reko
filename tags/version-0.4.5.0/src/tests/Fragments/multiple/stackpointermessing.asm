;;; Although we use and define the stack pointer, it must never be used as an
;;; explicit parameter.

.i86

main proc
	call foo
	mov [0x306],ax
	ret
	endp
	
foo proc
	mov [0x300],sp
	mov [0x302],ss
	; do stuff
	mov [0x308],bx
	mov ax,[0x304]
	; restore ss:sp
	lss sp,[0x300]
	ret
	endp
	