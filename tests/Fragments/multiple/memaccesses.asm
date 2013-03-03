;;; memaccesses.asm 
;;; Tests memory accesses off DS segment registers. Type 
;;; inference should merge all DS-based accesses resulting in
;;; a structure with fields at offsets 0x100, 0x200, 0x300.

.i86

main proc
	call foo
	call bar
	mov word ptr [0x100],0
	endp
	
foo proc
	mov ax,[0x300]
	mov [0x302],ax
	ret
	endp
	
bar proc
	mov ax,[0x200]
	mov [0x202],ax
	ret
	endp
