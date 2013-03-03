.i86

;;; Used to test termination scenarios. For instance, a terminating function has no live-out registers!

main proc 
	mov cx,123
	mov ax,32
	mov bx,0x3212
	call maybeterminate
	mov [0x230],ax
	mov [0x234],cx
	ret
	endp
	
maybeterminate proc
	cmp ax,[bx]
	jc  die
	cmp ax,[bx+2]
	jae die
	add bx,ax
	mov ax,[bx]
	ret
	
die:
	xor cx,cx
	mov [0x0232],cx
	mov ax,0x4C00
	int 0x21
	endp
	
	