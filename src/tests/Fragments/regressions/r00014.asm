	.i386
main proc
	sub	esp,0x64
	push	0x64
	lea	eax,[esp+04]
	push	eax
	push	0x09
	call	fn00401000
	push	0x08
	mov	ecx,eax
	call	fn00401000
	add	esp,08
	add	ecx,eax
	push	ecx
	call	fn00401050
	add	esp,0x70
	ret	0010
	endp


fn00401000 proc
	push	esi
	mov	esi,[esp+08]
	cmp	esi,01
	jge	l00401011
	
l0040100A:
	mov	eax,00000001
	pop	esi
	ret	

l00401011:
	lea	eax,[esi-02]
	push	edi
	push	eax
	call	fn00401000
	dec	esi
	push	esi
	mov	edi,eax
	call	fn00401000
	add	esp,08
	add	eax,edi
	pop	edi
	pop	esi
	ret	
	endp

fn00401050 proc
	mov	ecx,[esp+08]
	mov	eax,[esp+0x0C]
	push	edi
	lea	edi,[ecx+eax-01]
	cmp	ecx,edi
	jnc	l00401092

l00401061:
	push	ebx
	push	esi
	mov	esi,[esp+0x10]
	jmp	l00401070

l00401070:
	mov	eax,0xCCCCCCCD
	mul	esi
	shr	edx,03
	lea	eax,[edx+edx*4]
	add	eax,eax
	mov	ebx,eax
	mov	eax,esi
	sub	eax,ebx
	add	al,0x30
	mov	byte ptr [ecx],al
	inc	ecx
	mov	esi,edx
	cmp	ecx,edi
	jc	l00401070

l00401090:
	pop	esi
	pop	ebx

l00401092:
	mov	byte ptr [ecx],00
	pop	edi
	ret
	endp	
