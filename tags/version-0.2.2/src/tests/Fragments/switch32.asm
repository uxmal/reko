;;;  samples of switch statement emitted
;;; by the Microsoft VC compiler
	.i386
foo proc
	mov eax,[esp+04]
	cmp	eax,0000003
	ja	default

	xor	edx,edx
	mov	dl,[eax+bytes]
	jmp	dword ptr [jumps+edx*4]
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
	mov	eax,2
	jmp done
default:
	mov eax,-1
done:
	mov [dummy],eax
	ret
foo endp

dummy dd 0
