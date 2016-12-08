;;; Tests the effects of frames.
.i386

main proc
	mov eax,[0x10003420]
	push eax
	call foo
	add esp,4
	mov [0x10003428],ax
	ret
	endp
	
foo	proc
	push	ebp
	mov	ebp,esp
	push	ecx		; net effect is to subtract 4 from esp, allocating space for a variable
	mov	word ptr [ebp-04],0000
	cmp	dword ptr [ebp+08],00
	jnz	l10001365

l10001360:
	xor	ax,ax
	jmp	l10001391

l10001365:
	movsx	eax,word ptr [ebp-04]
	mov	ecx,[ebp+08]
	movsx	edx,byte ptr [ecx+eax]
	test	edx,edx
	jz	l1000138D

l10001374:
	movsx	eax,word ptr [ebp-04]
	cmp	eax,0x00007FFF
	jge	l1000138D

l1000137F:
	mov	cx,[ebp-04]
	add	cx,01
	mov	word ptr [ebp-04],cx
	jmp	l10001365

l1000138D:
	mov	ax,[ebp-04]

l10001391:
	mov	esp,ebp
	pop	ebp
	ret	
	endp
