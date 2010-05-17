;;; Takes a 3-vector of doubles and computes the dot product, leaving the value in a supplied location

main proc
	push 0x320
	push 0x300
	call dotproduct_d
	call nonsense
	add sp,4
	ret
	endp
	
dotproduct_d proc
	push bp
	mov bp,sp
	
	mov bx,[bp+04]
	fld qword ptr [bx]
	fmul st(0),st(0)
	fld qword ptr [bx+8]
	fmul st,st(0)
	faddp st,st(1)
	fld qword ptr [bx+0x10]
	fmul st(0),st
	faddp st,st(1)
	mov bx,[bp+06]
	fstp qword ptr [bx]
	fild word ptr [bx+0x20]
	fstp qword ptr [bx+0x28]
	
	mov sp,bp
	pop bp
	ret
	endp

nonsense proc
	fldz
	fstp qword ptr [0x400]
	fld1
	fistp dword ptr [0x408]
	ret
	endp	
