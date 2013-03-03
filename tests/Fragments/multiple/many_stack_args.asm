.i86

main proc
	push 3
	push 2
	push 1
	call foo
	add sp,0x6
	mov [0x1300],ax
	ret
	endp
	
foo proc
	push bp
	mov bp,sp
	mov ax,[bp+0x04]
	add ax,[bp+0x06]
	add ax,[bp+0x08]
	pop bp
	ret
	endp
