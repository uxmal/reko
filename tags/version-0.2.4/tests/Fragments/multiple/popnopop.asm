;;; Annoying sequence where BP is sometimes restored and sometimes not.

main proc 
	call foo
	jc main_done
	mov [0x200],bp
main_done:
	ret
	endp
	
foo proc
	push bp
	mov bp,0x300
	jmp foo_test
foo_loop:
	mov bp,[bp]	
foo_test:
	cmp bp,0
	jz foo_null
	cmp word ptr [bp],0
	jnz foo_loop
	add sp,02
	clc
	ret
	
foo_null:
	pop bp
	stc
	ret
	
	endp
