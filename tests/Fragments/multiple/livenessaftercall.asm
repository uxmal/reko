;;; livenessaftercall.asm
;;; This test file ensures that a load after a call is live.
.i86
main proc
	call foo
	mov [bx],ax
	ret
	endp
	
foo proc
	mov ax,1
	ret
	endp
