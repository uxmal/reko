;;; Tests with a while loop that contains both a 'break' and 'goto'

.i86
main proc
	mov bx,si
	jmp looptest
again:
	mov [di],ax
	or	ax,ax
	jne  ok
	
	mov ax,-1			
	jmp	return			;; this is the 'goto'
ok:
	cmp ax,0x0D			
	jnz looptest
	inc word ptr[0x302]	
	
looptest:
	lodsw
	cmp ax,0x20
	jnz	again
done:
	mov ax,si
	sub ax,bx
return:
	mov [0x300],ax
	ret
	
	endp
