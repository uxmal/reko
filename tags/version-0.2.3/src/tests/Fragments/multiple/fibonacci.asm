	.386p
	
	_arg = 8
fib  proc 
	push	esi
	mov	esi,_arg[esp]
	cmp	esi,01
	jg	l10000012

l10000009:
	mov	eax,0x00000001
	pop	esi
	ret	

l10000012:
	lea	eax,[esi-02]
	push	edi
	push	eax
	call	fib
	dec	esi
	push	esi
	mov	edi,eax
	call	fib
	add	esp,0x08
	add	eax,edi
	pop	edi
	pop	esi
	ret	
	endp

 