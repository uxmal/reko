.i86
main proc
	push 0xD00
	pop ds
	mov dx,0x3000
	mov bx,[0x300]
	mov cx,0x100		; count of bytes
	call read
	or ax,ax
	jge continue
	mov al,1
	call exit
continue:
	mov [0x302],ax		; bytes read
	ret
	endp	


exit proc
	mov ah,0x4C
	int 0x21				; should not fall through: int21/4C terminates an MS-DOS program.
	endp

read proc
	mov ah,0x3F
	int 0x21
	jnc read_done
	mov ax,-1
read_done:
	ret
	endp
	
