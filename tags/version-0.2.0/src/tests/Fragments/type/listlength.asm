;;; This fragment is for testing type inference.
;;; This should result in a type deduction for a linked list (with an unknown data field).

.i86
length proc
	xor ax,ax
again:
	or bx,bx
	jz done
	
	inc ax
	mov bx,[bx+04]
	jmp again
done:
	mov [0x200],ax		;; otherwise, ax is dead.
	ret
length endp
