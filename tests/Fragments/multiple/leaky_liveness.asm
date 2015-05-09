.i86

main proc
	call dequeue
	jc   done
	
	add [0x300],ax
done:
	ret
	endp
	
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
