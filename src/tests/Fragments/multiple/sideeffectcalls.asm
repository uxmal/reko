main proc
	call foo
	mov [0x300],bx
	call foo
	call bar
	ret
	endp
	
	; ax foo(&bx) defines both
foo proc
	mov ax,42
	mov bx,43
	ret
	endp
	
	; bar(ax, bx), uses both
bar proc
	mov [0x400],ax
	mov [0x402],bx
	ret
	endp
	