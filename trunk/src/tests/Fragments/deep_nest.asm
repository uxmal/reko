	.i86
	mov si,0x1234
	call level1
	mov [0x5431],ax
	ret

level1 proc
	push si			
	call level2
	pop si
	ret
level1 endp

level2 proc
	mov ax,[si+0x4]
	ret
level2 endp

