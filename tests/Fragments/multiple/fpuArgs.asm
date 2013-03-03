;;; Test to exercise the passing of arguments on the FPU stack.

.i86

main proc
	fld	dword ptr [0x300]
	fld dword ptr [0x304]
	push 0x308
	call store_em
	
	fldz
	fstp dword ptr [0x30C]
	
	mov ax,[0x308]
	push ax
	call load_it
	fstp dword ptr [0x30C]
	add sp,4
	ret
main endp

store_em proc
	push bp
	mov bp,sp
	
	mov bx,[bp+4]
	fstp dword ptr [bx]
	fstp dword ptr [bx+4]
	
	pop bp
	ret
store_em	endp

load_it proc
	push bp
	mov bp,sp

	mov bx,[bp+4]
	fld dword ptr [bx]
	
	pop bp
	ret
load_it endp
