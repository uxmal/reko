;;; leaky_liveness.asm
;;;
;;; Illustrates a procedure (`dequeue`) that leaks
;;; ax as live-out, in additional the carry flag (CF)

.i86

main proc
	call dequeue
	jc   done
	
	add [0x300],ax
done:
	ret
	endp

;;; This procedure has no live-in parameters.
	
dequeue proc
	mov si,[0x100]
	cmp si,[0x102]
	jz q_empty
	
	mov ax,[si]
	inc si
	inc si
	mov [0x100],si
	clc
	ret
q_empty:
	stc
	ret
	endp
