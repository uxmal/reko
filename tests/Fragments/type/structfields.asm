;;; This fragment is for testing type inference.
;;; Different fields of a struct should be deduced here

.i386
foo proc
	mov eax,[bx+04]
	mov edx,[bx+08]
	add eax,edx
	mov [bx],eax
	ret
foo endp
