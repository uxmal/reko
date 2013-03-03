;;; Tests FPU comparison operations.
.i86
	fld qword ptr [bx]
	fld1
	fcomp
	fstsw ax
	test ax,0x4000
	jnz foo
	
	mov [bx+0x08],0x03
foo:
	ret
