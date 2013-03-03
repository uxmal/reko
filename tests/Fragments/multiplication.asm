;;; Tests for multiplication rewriting.

.i86
	mov al,[0x300]
	mul byte ptr [0x301]
	mov [0x302],ax
	
	mov ax,[0x304]
	mul word ptr[0x0306]
	mov [0x0308],ax
	mov [0x030A],dx
	
	mov cx,[0x30C]
	mov ax,[0x30E]
	imul cx
	mov [0x0310],ax
	mov [0x0312],dx
	
	mov al,[0x300]
	mov cl,[0x301]
	mul cl
	mov [0x302],ax
	
	imul ax,[0x0304],0x24
	imul dx,ax,0x24
	imul ax,[0x0304],0x114
	imul dx,ax,0x114
	imul ax,[0x0304],0x114
	ret
	