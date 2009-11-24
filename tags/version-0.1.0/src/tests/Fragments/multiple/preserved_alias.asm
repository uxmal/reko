;; preserved_alias.asm
;; In this scenario, ax is preserved, we then mangle al, but restore ax before returning. 
.i86
main proc
	mov ax,[0x100]
	call foo
	mov [0x0102],ax
	ret
	endp
	
foo proc
	push ax
	mov al,[0x104]
	mov [0x0105],al
	pop ax;
	ret
	endp
