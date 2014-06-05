; pushed_registers.asm
; Used to test registers being pushed/popped affecting the liveness of said registers.

main proc
	mov cx,42
	push 1
	push 2
	call doesnt_use_cx
	
	xor ax,ax
	push ax
	push ax
	push ax
	push ax
	push ax
	push ax
	call bigstack
	add sp,0xC
	push 1
	push 2
	call does_use_cx
	add sp,4
	ret
	
doesnt_use_cx proc
	push cx
	push bp
	mov bp,sp
	mov cx,[bp+06]
	mov ax,[bp+08]
	add ax,cx
	pop bp
	pop cx
	ret 4
	
bigstack proc
	push	bx
	push	cx
	push	dx
	push	bp
	mov	bp,sp
	lea	bx,[bp+0x0E]

l07BC:
	mov	ax,[bx]
	add	bx,02
	test	ax,ax
	jnz	l07BC

l07C5:
	mov	dx,[bp+0x0C]
	add	bx,02
	mov	ax,[bp+0x0A]
	mov	cx,[bx-02]
	lea	bx,[bp+0x0E]
	call	other_big
	pop	bp
	pop	dx
	pop	cx
	pop	bx
	ret	
		
does_use_cx proc
	push cx
	push bp
	mov bp,sp
	mov ax,[bp+08]
	add ax,cx
	pop bp
	pop cx
	ret

other_big proc
	mov [0x200],bx
	mov [0x300],cx
	ret
	