;;; Tests involving calls to a stack memory allocator.
.i86

main proc
	mov ax,[0x300]
	push ax
	call bar

	call foo
	

	ret
	endp

foo proc
	push bp
	mov bp,sp
	
	mov ax,0x100
	call alloca
	
	push 0x1000
	call baz
	
	mov sp,bp
	pop bp
	ret
	
bar proc
	push bp
	mov bp,sp
	
	mov bx,[bp+04]
	mov ax,[bx+02]
	call alloca				;; Allocate some bytes.
	
	push ax
	call baz
	
	mov sp,bp
	pop bp
	ret
	endp
	
alloca proc
	pop	cx			; return address
	sub sp,ax		; get some stack space
	mov ax,sp		; copy the new stack pointer
	push cx
	ret
	endp
	

baz proc
	push bp
	mov bp,sp
	
	mov bx,[bp+04]
	mov word ptr [bx],0
	
	pop bp
	ret 2
	endp
