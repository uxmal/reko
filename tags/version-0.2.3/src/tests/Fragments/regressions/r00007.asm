main proc
	mov al,32
	call foo
	jc   skip
	mov  [0x300],al
skip:
	ret
	endp
	
foo proc
	cmp al,0x20
	jc  control
	
	inc word ptr[0x201]
control:
	ret
	endp
	