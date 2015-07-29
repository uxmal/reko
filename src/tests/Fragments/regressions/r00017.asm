; r00017.asm: causes stack overflow when generating structired code.
.i386
fn08048980:
	push	ebp
	mov	ebp,esp
	push	ebx
	mov	ebx,offset data1
	cmp	dword ptr [data1],-1
	jz	l080489A0

l08048992:
	mov	esi,esi

l08048994:
	mov	eax,[ebx]
	call	eax
	add	ebx,-4
	cmp	dword ptr [ebx],-1
	jnz	l08048994

l080489A0:
	mov	ebx,[ebp-04]
	mov	esp,ebp
	pop	ebp
	ret
	endp

data1 dd 0
