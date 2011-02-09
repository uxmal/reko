;;; intrasegmentfarcall.asm

.i86
main proc
	push 0x0D00
	push 0x0300
	push 0x0001
	push cs
	call far_add
	add sp,6

	push 0x0D00
	push 0x304
	push 0x0002
	call far far_sub
	add sp,6
	ret
	endp

far_add proc
	push bp
	mov bp,sp
	les bx,[bp+6]
	mov ax,[bp+0xA]
	add es:[bx],ax
	pop bp
	retf
	endp

far_sub proc
	push bp
	mov bp,sp
	les bx,[bp+6]
	mov ax,[bp+0xA]
	sub es:[bx],ax
	pop bp
	retf
	endp
