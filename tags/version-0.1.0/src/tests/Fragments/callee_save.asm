	.i86
	call proc1
	mov si,ax
	call proc2
	mov [1024],ax
	ret
	
proc1 proc
	push si			;; callee-save
	mov si,[0122]	
	mov si,[si+04]
	lea ax,[si+08]
	pop si
	ret				;; ax should be live out, but not si
proc1 endp

proc2 proc
	push si			;; si is callee-save...
	mov ax,[si]		;; but also a parameter! so it should be live in!
	pop si
	ret
proc2 endp

