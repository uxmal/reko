.i86

;;; Tests situations that look like compound procedures but aren't.

main proc

	cmp ax,1
	jz n3
	cmp bx,2
	jz n5
n3:
	cmp cx,3
	jz n5
	mov [0x300],ax
	jmp done
n5:	
	mov [0x302],cx
done:
	ret

main endp
