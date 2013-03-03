	.i86
foo proc
	mov bp,sp 
	mov	ax,[bp+06]
	or ax,ax
   jnz none 
   mov ax,-1
none:
	mov sp,bp
	ret
	
