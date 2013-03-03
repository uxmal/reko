		.i86
foo		proc
		push	cs
		pop		ds
		mov	bl,[si]
		cmp bl,0x2
		ja default
		
		xor bh,bh
		add bx,bx
		jmp	[bx+jmptable]

jmptable:
		dw	offset one
		dw	offset two
		dw	offset three
one:
		mov	ax,1
		ret
two:
		mov ax,2
		ret
three:
		mov ax,3
		ret
default:
		mov ax,0
		ret
foo		endp
