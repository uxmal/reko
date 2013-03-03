;;; test for the SETcc intel instruction -- and branches.

.i86
	xor ecx,ecx
	sub ax,bx
	setz cl
	mov [0x300],ecx
	
	cmp ax,0x30
	jl done
	mov [0x302],ax
done:
	ret
