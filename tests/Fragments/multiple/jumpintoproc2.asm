.86

main proc
	call foo1
	call foo2
	ret
	endp
	
foo1 proc
	push bx
	push cx
	mov bx,[si+02]
	mov cx,[bx]
	add cx,[bx+02]
	mov [bx+04],cx
	jmp  popem
	endp
	
foo2 proc
	push bx
	push cx
	
	mov bx,[si+02]
	mov cx,[bx]
	sub cx,[bx+02]
	mov [bx+04],cx
		
popem:
	pop cx
	pop bx
	ret
	endp
	
	
	