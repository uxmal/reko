	.i386

main proc
	mov ebx,3
	call blob
	push 9
	call foo
	pop ecx
	ret

; This trampoline yields control to foo_core, which 
; expects a word on the stack. The jmp will be rewritten
; to a `call` RTL instruction with zero stack depth.
foo proc
	jmp foo_core

; Expects an argument in the ebx register, which is pushed
; onto the stack before calling foo_core.
blob proc
	push ebx
	call foo_core
	pop ecx
	ret

; Expects a single word32 argument at stack offset +[4..7]. The
; return address is at +[0..3]
foo_core proc
	mov eax,[esp+4]
	mov [0x00123400],eax
	ret
