;;; Mutual recursion. A baddie :-)

		.i86
main	proc
		call one
		ret
main	endp

one		proc
		cmp	dx,30
		jnz  not_one
		call	two
		jmp  one_join
not_one:
		dec	dx
one_join:
		mov ax,dx
		ret
one		endp

two		proc
		cmp		dx,20
		jl		not_two
		dec		dx
		call	one
		jmp		two_join
not_two:
		sub		dx,10
		call	one
two_join:
		inc		ax
		ret
two		endp
