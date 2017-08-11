.i86

;;; Compound conditions (a && b) in complex situations.

main proc 
	cmp ax,1 
	jne noteq1
	cmp ax,2
	jne noteq1
	cmp ax,3
	jne noteq1
eq1:
	mov ax,1
	jmp test1done
noteq1:
	xor ax,ax
	jmp test1done
test1done:
	mov [0x300],ax
	
	cmp ax,10
	jne noteq2
	cmp ax,11
	jne noteq2
	mov ax,1
	jmp test2done
noteq2:
	xor ax,ax
test2done:
	mov [0x302],ax
	ret
	endp
