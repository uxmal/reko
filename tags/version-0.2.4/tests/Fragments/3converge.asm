;;; last node has more than two predecessors.

.i86
	cmp	bx,0
	jz	zero
	jg  posi
	mov	ax,-1
	jmp done
posi:
	mov ax,1
	jmp done
zero:
	xor ax,ax
done:
	mov [0x100],ax		;; force AX to be live here.
	ret
