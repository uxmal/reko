main proc
	call foo
	mov [0x300],bx
	call foo
	call bar
	ret
	endp
	
foo proc
	mov ax,42
	mov bx,43
	ret
	endp
	
bar proc
	mov [0x400],ax
	ret
	endp
	