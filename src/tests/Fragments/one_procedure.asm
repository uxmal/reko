	.i86
;;; [ax] in arg - pointer to some structure
;;; [bp+6] in arg - pointer to some structure.
	push bp
	mov	bp,sp
	mov	si,ax
	mov	ax,[si+04]
	add	ax,[si+06]
	mov si,[bp+06]
	add ax,[si+04]
	add ax,[si+06]
	mov sp,bp
	pop bp
	ret
