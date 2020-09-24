;;; negsnots.asm
;;; Test for not and neg instructions, which lead to UnaryExpressions.

	.i86
	
	neg eax
	sbb eax,eax
	mov [0x300],eax

	neg word ptr [0x202]
	
	neg word ptr [0x200]
	sbb ecx,ecx	
	mov [0x308],ecx
	
	not bx
	mov [0x304],bx
	
	not word ptr [0x204]
	
	mov cx,[0x304]
	sub cx,0x3E8
	neg cx
	mov [0x306],cx
	
	mov eax, [0x310]
	neg eax
	sbb eax,eax
	inc eax
	mov [0x30C],eax

	ret
