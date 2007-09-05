.i386
main proc
	mov	eax,[esp+04]
	push	ebx
	mov	ebx,[esp+10]
	push	ebp
	movsx	ecx,word ptr [eax+06]
	movsx	edx,word ptr [eax+02]
	mov	ebp,[esp+0x10]
	sub	ecx,edx
	mov	edx,[esp+0x18]
	mov	dword ptr [ebx],ecx
	movsx	ecx,word ptr [eax+0x04]
	movsx	eax,word ptr [eax]
	sub	ecx,eax
	test	ebp,ebp
	mov	dword ptr [esp+0x14],ecx
	mov	dword ptr [edx],ecx
	jz long	l100202F3

l10020225:
	fild	dword ptr [ebx]
	push	esi
	push	edi
	xor	esi,esi
	mov	edi,0x000186A0
	fstp	dword ptr [esp+0x14]
	fild	dword ptr [esp+0x1C]

l10020238:
	fld	dword ptr [esp+0x14]
	fmul	qword ptr [10033290]
	inc	esi
	fst	dword ptr [esp+0x14]
	call 	_ftol
	mov	ecx,eax
	mov	dword ptr [esp+0x1C],ecx
	fild	dword ptr [esp+0x1C]
	fcomp	dword ptr [esp+0x14]
	fstsw	ax
	test	ah,0x40
	jnz	l1002026E

l10020261:
	inc	ecx
	mov	dword ptr [esp+0x14],ecx
	fild	dword ptr [esp+0x14]
	fstp	dword ptr [esp+0x14]

l1002026E:
	fmul	qword ptr [0x10033290]
	fld	st(0)
	call	_ftol
	mov	ecx,eax
	mov	dword ptr [esp+0x1C],ecx
	fild	dword ptr [esp+0x1C]
	fld	st(1)
	fcompp	
	fstsw	ax
	test	ah,0x40
	jnz	l1002029B

l10020290:
	inc	ecx
	fstp	st(0)
	mov	dword ptr [esp+0x1C],ecx
	fild	dword ptr [esp+0x1C]

l1002029B:
	fld	dword ptr [esp+0x14]
	fcomp	qword ptr [0x10033298]
	fstsw	ax
	test	ah,0x40
	jz	l100202C3

l100202AC:
	fcom	qword ptr [0x10033298]
	fstsw	ax
	test	ah,0x40
	jz	l100202C3

l100202B9:
	cmp	edi,0x000186A0
	jnz	l100202C3

l100202C1:
	mov	edi,esi

l100202C3:
	cmp	esi,ebp
	jl	long l10020238

l100202CB:
	fld	dword ptr [esp+0x14]
	call	_ftol
	mov	dword ptr [ebx],eax
	call	_ftol
	mov	ecx,[esp+0x20]
	cmp	ebp,edi
	pop	edi
	pop	esi
	mov	dword ptr [ecx],eax
	jle	l100202F3

l100202E7:
	mov	dword ptr [ebx],00000000
	mov	dword ptr [ecx],00000000

l100202F3:
	pop	ebp
	pop	ebx
	ret	
	endp
	
_ftol proc
	fstp dword ptr [esp-4]
	mov eax,[esp-4]
	ret
	endp
	