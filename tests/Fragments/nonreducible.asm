;;; A non-reducible graph structure.

	.i86

node1:
	cmp ax,0
	jnz node3
	
node2:
	sub	ax,3
	sar ax,1
	
node3:
	imul ax
	or ax, 0
	jnz node2
	
	mov [0x200],ax	;; ensures AX is live
	ret
