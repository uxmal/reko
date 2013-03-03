;;; A more complicated decision tree.

.i86

	cmp ax,0
	jl negatory				; ...ol'e buddy
		
		xor cl,cl			; sign is positive.
		cmp ax,12
		jle small
		
			mov ax,12
		
small:
		jmp join
negatory:
		mov cl,01
		cmp ax,-12
		jge negsmall
			
			mov ax,-12
negsmall:
join:
	mov [0x300],ax
	mov [0x302],cl
	ret


		