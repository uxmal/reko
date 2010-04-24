;;; regression 00003. Problems with register flow.

main proc
	call foo
	call bar
	xor ecx,ecx
	mov [0x310],ecx
	ret
	endp
	
foo proc
	ret
	endp
	
bar	proc
	xor ch,ch
	mov cl,[0x300]
	jcxz bar_skip
	mov word ptr [0x302],0

bar_skip:
	ret	
	endp
