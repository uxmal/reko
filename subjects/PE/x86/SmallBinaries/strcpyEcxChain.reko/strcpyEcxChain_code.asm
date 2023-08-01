;;; Segment code (12340000)

;; fn12340000: 12340000
fn12340000 proc
	mov	edi,ecx
	mov	edx,12340034h
	or	ecx,0FFh
	xor	eax,eax
	repne scasb
	not	ecx
	sub	edi,ecx
	mov	esi,edi
	mov	eax,ecx
	mov	edi,edx
	shr	ecx,2h
	rep movsd
	mov	ecx,eax
	and	ecx,3h
	rep movsb
	ret
12340025                CC CC CC CC CC CC CC CC CC CC CC      ...........
12340030 73 31 00 00 73 32                               s1..s2          
