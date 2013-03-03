;;; Test for a while loop with a large head block; this forces the decompiler to choose a structure like:
;;; while(1) {
;;;		head;
;;;		if (cond)
;;;			break;
;;;		body;
;;;	}
;;;
.x86

main proc
	mov ax,[si]
	jmp head
body:
	push ax
	mov ax,[si+6]
	add [si+6],ax
	pop ax
	dec ax
head:
	mov [si+2],ax
	mov [si+4],ax
	mov word ptr [si+8],03
	cmp ax,0
	jnz body
	
	ret
	endp
