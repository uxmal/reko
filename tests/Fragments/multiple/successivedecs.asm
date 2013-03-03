;;; Common pattern in hand written assembler. Net result is that of a switch statement.
.i86

main proc
	mov ax,[0x300]
	call foo
	mov [0x302],ax
	ret
	endp
	
foo proc
	or ax,ax
	jnz foo_1
	mov ax,0x123
	ret
foo_1:
	dec ax
	jnz foo_2
	mov ax,0x125
	ret
foo_2
	dec ax
	jnz foo_default
	mov ax,0x127
	ret
foo_default:
	xor ax,ax
	ret
	endp
	