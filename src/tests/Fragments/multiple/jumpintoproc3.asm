	.i386

main proc
	mov ebx,3
	call blob
	push 9
	call foo
	pop ecx
	ret

foo proc
	jmp foo_core

blob proc
	push ebx
	call foo_core
	pop ecx
	ret

foo_core proc
	mov eax,[esp+4]
	mov [0x00123400],eax
	ret
