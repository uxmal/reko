;;; while_break.asm
;;; A while loop with a break innit

.i86

	xor ax,ax
	mov bx,[0x200]
lupe:
	or bx,bx
	jz done
	
	mov [0x204],bx
	cmp word ptr [bx],-1
	je  done		;; this is the break
	
	mov bx,[bx+04]
	inc ax
	jmp lupe
	
done:
	mov [0x300],ax
	ret
