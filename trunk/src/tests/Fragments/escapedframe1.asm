;;; A test involving frames that escape.
.i86
main proc
	push bp
	mov bp,sp
	sub sp,4
	mov word ptr [bp-04],0
	mov word ptr [bp-02],1
	push bp
	call foo
	
	mov sp,bp
	pop bp
	ret
	
foo proc
	push bp
	mov bp,sp
	mov di,[bp+4]
	mov word ptr [di-02],0
	pop bp
	ret
	endp
