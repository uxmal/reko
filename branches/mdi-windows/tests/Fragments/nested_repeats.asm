;;; Two nested repeat loops that share the same header node.
;;; Adds together all shorts in a 4x4 matrix.
.i86

main	proc

		xor dx,dx
		mov cx,4
		mov di,4
top:
		lodsw
		add dx,ax
		loop top

		mov cx,4
		dec di
		jnz top
		
		mov [0x0300],dx
		ret
main	endp
