;;; Void function calling a void function

main proc
	mov si,0x100
	call foo
	
	push 0x100
	call foo_stack
	add sp,2
	ret
main endp

foo proc
	mov ax,[si+02]
	call bar
	ret
foo endp

foo_stack proc
	push bp
	mov bp,sp
	mov bx,[bp+4]
	mov ax,[bx+2]
	push ax
	call bar_stack
	add sp,2
	pop bp
	ret
foo_stack endp

bar proc
	mov [di],ax
	ret
bar endp

bar_stack proc
	push bp
	mov bp,sp
	mov ax,[bp+4]
	mov [di],ax
	pop bp
	ret
bar_stack endp


