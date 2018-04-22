;;;  samples of switch statement emitted
;;; by the Microsoft VC compiler
	.i386
foo proc
	push	ebp
	mov     ebp, esp
	mov	eax, [ebp+08]
	add	eax, 0x7FF8FFFA
	mov	[ebp-04], eax
	cmp	dword ptr [ebp-04],0000003
	ja	default

	mov	edx,[ebp-04]
	xor	ecx,ecx
	mov	cl,[edx+bytes]
	jmp	dword ptr [jumps+ecx*4]
bytes:
	db	01
	db	00
	db	01
	db	02
jumps
	dd	offset jump0
	dd	offset jump1
	dd	offset jump2

jump0:	
	mov eax,0
	jmp done
jump1:
	mov eax,1
	jmp done
jump2
	mov eax,2
	jmp done
default:
	mov eax,-1
done:
	mov [dummy],eax
	pop ebp
	ret
foo endp

dummy dd 0
