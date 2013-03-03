;;; longaddsub.asm

.i86

main proc
	mov ax,[0x0300]
	mov dx,[0x0302]
	add ax,[0x0304]
	adc dx,[0x0306]		; should result in dx_ax += [0x0304:dw]
	
	;; interleaved adds and moves
	
	mov ax,[si+0x0308]
	add ax,cx
	mov dx,[si+0x030A]
	adc dx,bx			; should result in dx_ax = bx_cx + [si+0x0308:dw]
	
	;; Target is memory
	
	add [0x0200],eax
	adc [0x0204],edx
	
	;; sub
	
	mov cx,[si+0x030C]
	sub ax,cx
	mov bx,[si+0x030E]
	sbb dx,bx
	ret
	
	endp
	
	