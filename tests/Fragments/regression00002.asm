;;; Regression test 00002: caused failure when value numbering.

main	proc
	mov	dword ptr [0x7960],edx
	mov	byte ptr [0x7964],cl
	mov	eax,edx
	neg	eax
	mov	[si+0x00C8],eax
	ret
	endp
