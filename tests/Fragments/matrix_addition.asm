;;; Matrix addition: a typical nested loop.

		.i86
mat_add proc
		mov dx,4
outer:
		mov cx,4
inner:
		mov eax,[si]
		add eax,[bx]
		add si,4
		mov [di],eax
		add bx,4		
		add di,4
		loop inner

		dec dx
		jnz outer
		
		ret		
mat_add endp
