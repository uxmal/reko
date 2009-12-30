		.i86
main	proc
		mov dx,30
		call foo
		mov [300],ax
		ret
main	endp

foo		proc
		call bar
		ret
foo		endp

bar		proc
		push si
		xor si,si
		jmp loop_test
not_done:
		add si,dx
		dec dx
loop_test
		cmp dx,0
		jge not_done

		mov ax,si
		pop	si
		ret
bar		endp
