;;; nestedstructs.asm
;;; This fragments tests the ability to resolve a struct stored in another.
;;; Very useful when dealing with the structure "global memory".

main proc
	mov word ptr [0x300],0000
	mov word ptr [0x302],0202
	mov word ptr [0x304],0404
	
	xor ax,ax
	mov bx,0x302		;; pointer to part of the above struct.
	mov ax,[bx]
	add ax,[bx+02]
	mov [bx+04],ax

	ret
main endp
