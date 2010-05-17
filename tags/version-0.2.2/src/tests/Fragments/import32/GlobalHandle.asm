.i386

foo	proc
	push	esi
	mov	esi,[esp+08]
	push	edi
	mov	edi,[_GlobalHandle]
	push	esi
	call	edi
	push	eax
	call	dword ptr [_GlobalUnlock]
	push	esi
	call	edi
	push	eax
	call	dword ptr [_GlobalFree]
	pop	edi
	pop	esi
	ret	
	endp
	
_GlobalHandle	.import 'GlobalHandle','kernel32.dll'
_GlobalUnlock	.import 'GlobalUnlock','kernel32.dll'
_GlobalFree		.import	'GlobalFree',  'kernel32.dll'

