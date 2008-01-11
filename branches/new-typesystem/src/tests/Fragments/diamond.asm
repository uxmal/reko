;;; Simple if-then-else diamond.
.i86
	cmp ax,bx
	jz right
left:
	mov ax,0
	jmp join
right:
	mov ax,1
join:
	mov [100],ax			;; make ax live.
	ret
