	.i86
	
;;; Another factorial program. This time, parameters are passed in registers,
;;; not on the stack.

	mov cx,5
	call factorial
	mov [0100],ax
	ret

factorial proc
	push si			;; callee-save
	cmp cx,1
	jle basis

	mov si,cx
	dec cx
	call factorial
	imul si
	jmp done
basis:
	mov ax,1
done:
	pop si
	ret				;; returns in ax.
factorial endp
