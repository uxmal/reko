	.i86
	xor ax,ax
	jmp looptest
again:
	mov  si,[0x300]
	or	 si,si
	jz   isnull
	  add  ax,[si]
      jmp  looptest
isnull:
      mov si,[0x302]
      add  ax,[si+04]
looptest:
    cmp	ax,bx
	jl again

	ret
			
