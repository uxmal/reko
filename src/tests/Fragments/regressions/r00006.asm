;;; Structuring this code causes assertions and general death.

	.i386
fn10000000 proc 
	push	ebx
	push	ebp
	push	esi
	push	edi
	mov	di,[esp+0x14]
	xor	bl,bl
	test	di,di
	jnz	l10000021

l100048C0:
	mov	byte ptr [0x1006B8E0],00
	call	fn100325C0
	pop	edi
	pop	esi
	pop	ebp
	pop	ebx
	ret	

l10000021:
	mov	eax,[esp+0x1C]
	cmp	dword ptr [eax],00
	jnz	l1000002C

l100048DA:
	mov	bl,01

l1000002C:
	mov	ebp,[esp+0x20]
	push	0x1000A700
	push	eax
	mov	eax,[esp+0x20]
	push	0x00003158
	mov	ecx,[eax+0x0000D0]
	push	ecx
	push	eax
	push	ebp
	call	fn10000267
	mov	esi,eax
	add	esp,0x18
	test	esi,esi
	jnz	l10000061

l10004906:
	pop	edi
	mov	word ptr [ebp+00],0xFF94
	pop	esi
	pop	ebp
	pop	ebx
	ret	

l10000061:
	cmp	di,01
	jnz	l1000006D

l10000067:
	mov	byte ptr [esi+0x00000204],bl

l1000006D:
	push	esi
	call	fn10000022D
	mov	eax,[esi]
	xor	ebp,ebp
	add	esp,04
	cmp	word ptr [eax],bp
	jnz	long l10004ACF

l10004933:
	cmp	di,bp
	jle	long l10004AAD

l1000493C:
	cmp	di,05
	jg	long l10004AAD

l10004946:
	cmp	di,01
	jnz	long l10004968

l1000494C:
	push	esi
	call	fn10000022D
	mov	edx,[esi+04]
	add	esp,04
	mov	eax,[edx+0x0C]
	mov	ecx,[eax]
	mov	dword ptr [ecx],00000001
	jmp	l10004AB2

l10004968:
	cmp	di,02
	jnz	l100049DA

l1000496E:
	mov	edx,[esi+04]
	push	esi
	mov	dword ptr [edx+0x30],ebp
	call	fn10000022D
	mov	eax,[esi+04]
	push	01
	mov	dword ptr [eax+0x30],ebp
	mov	eax,[esi+04]
	add	eax,16
	mov	cx,[eax+02]
	mov	word ptr [esi+0x00000B4],cx
	mov	dx,[eax]
	mov	word ptr [esi+0x00000B2],dx
	mov	cx,[eax+06]
	mov	word ptr [esi+0x00000B8],cx
	mov	dx,[eax+04]
	mov	word ptr [esi+0x00000B6],dx
	mov	eax,[esi+04]
	add	eax,16
	push	eax
	push	esi
	call	fn1000D580
	push	esi
	call	fn1000D510
	push	esi
	call	fn1000D5E0
	push	esi
	call	fn10005A80
	add	esp,0x1C
	jmp	l10004AB2

l100049DA:
	cmp	di,03
	jnz	l100049EE

l100049E0:
	push	esi
	call	fn10004AE0
	add	esp,04
	jmp	l10004AB2

l100049EE:
	cmp	di,04
	jnz	l10004A9C

l100049F8:
	mov	ecx,[esi+04]
	cmp	dword ptr [ecx+0x00000E0],ebp
	setnz	bl

l10004A04:
	push	esi
	call	fn1000B860
	mov	eax,[esi]
	add	esp,04
	cmp	word ptr [eax],bp
	jz	l10004A1B

l10004A14:
	mov	byte ptr [esi+0xAC],00

l10004A1B:
	cmp	byte ptr [esi+0xAD],01
	jnz	l10004A29

l10004A24:
	cmp	word ptr [eax],bp
	jz	l10004A04

l10004A29:
	cmp	word ptr [eax],bp
	jnz	l10004A37

l10004A2E:
	push	esi
	call	fn10004EA0
	add	esp,04

l10004A37:
	cmp	byte ptr [esi+0xAC],01
	jnz	l10004A5B

l10004A40:
	test	bl,bl
	jz	l10004AB2

l10004A44:
	mov	edx,[esi+04]
	call	dword ptr [edx+0xE0]
	mov	ecx,[esi]
	mov	word ptr [ecx],ax
	mov	edx,[esi]
	cmp	word ptr [edx],bp
	jnz	l10004AB2

l10004A59:
	jmp	l10004A04

l10004A5B:
	push	esi
	call	fn1000A700
	push	esi
	call	fn1000D460
	mov	eax,[esi+04]
	push	ebp
	push	ebp
	push	ebp
	add	eax,0x34
	push	ebp
	push	eax
	call	fn10032760
	mov	ecx,[esi+04]
	push	ebp
	push	ebp
	push	ebp
	add	ecx,0x40
	push	ebp
	push	ecx
	call	fn10032760
	mov	edx,[esi+04]
	push	ebp
	push	ebp
	push	ebp
	add	edx,0x60
	push	ebp
	push	edx
	call	fn10032760
	add	esp,0x44
	jmp	l10004AB2

l10004A9C:
	cmp	di,05
	jnz	l10004AB2

l10004AA2:
	push	esi
	call	fn1000A450
	add	esp,04
	jmp	l10004AB2

l10004AAD:
	mov	word ptr [eax],0x8A6C

l10004AB2:
	mov	eax,[esp+0x1C]
	mov	eax,[eax]
	cmp	eax,ebp
	jz	l10004ACF

l10004ABC:
	mov	ecx,[esi+04]
	push	eax
	mov	edx,[ecx+0x00D0]
	push	edx
	call	fn10025160
	add	esp,08

l10004ACF:
	pop	edi
	pop	esi
	pop	ebp
	pop	ebx
	ret	
	endp


fn10004AE0 proc
    mov dword ptr [0x00123400],0x10004AE0
	mov	eax,[esp+0x04]
	ret
	endp

fn10004EA0 proc
    mov dword ptr [0x00123400],0x10004EA0
	mov	eax,[esp+0x04]
	ret
	endp	
	
fn10000022D proc
    mov dword ptr [0x00123400],0x1000022D
	mov	eax,[esp+0x04]
	ret
	endp
	
fn10005A80 proc
    mov dword ptr [0x00123400],0x10005A80
	mov	eax,[esp+0x04]
	ret
	endp
	
fn1000A450 proc
    mov dword ptr [0x00123400],0x1000A450
	mov	eax,[esp+0x04]
	ret
	endp
	
fn1000A700 proc
    mov dword ptr [0x00123400],0x1000A700
	mov	eax,[esp+0x04]
	ret
	endp
	
fn1000B860 proc
    mov dword ptr [0x00123400],0x1000B860
	mov	eax,[esp+0x04]
	ret
	endp
	
fn1000D460 proc
    mov dword ptr [0x00123400],0x1000D460
	mov	eax,[esp+0x04]
	ret
	endp
	
fn1000D510 proc
    mov dword ptr [0x00123400],0x1000D510
	mov	eax,[esp+0x04]
	ret
	endp
	
fn1000D580 proc
    mov dword ptr [0x00123400],0x1000D580
	mov eax,[esp+0x08]
	add eax,[esp+0x04]
	ret
	endp
	
fn1000D5E0 proc
    mov dword ptr [0x00123400],0x1000D5E0
	mov	eax,[esp+0x04]
	ret
	endp
	
fn10025160 proc
    mov dword ptr [0x00123400],0x10025160
	mov eax,[esp+0x08]
	add eax,[esp+0x04]
	ret
	endp
	
fn10000267 proc
    mov dword ptr [0x00123400],0x10000267
	mov edx,[0x20000000]
	mov eax,[esp+0x04]
	mov [edx+0x04],eax
	mov eax,[esp+0x08]
	mov [edx+0x08],eax
	mov eax,[esp+0x0C]
	mov [edx+0x0C],eax
	mov eax,[esp+0x10]
	mov [edx+0x10],eax
	mov eax,[esp+0x14]
	mov [edx+0x14],eax
	mov eax,[esp+0x18]
	mov [edx+0x1C],eax
	ret
	endp
	
fn100325C0 proc
	ret
	endp
	
fn10032760 proc
    mov dword ptr [0x00123400],0x10032760
	mov edx,[0x20000000]
	mov eax,[esp+0x04]
	mov [edx+0x04],eax
	mov eax,[esp+0x08]
	mov [edx+0x08],eax
	mov eax,[esp+0x0C]
	mov [edx+0x0C],eax
	mov eax,[esp+0x10]
	mov [edx+0x10],eax
	mov eax,[esp+0x14]
	mov [edx+0x14],eax
	ret
	endp
	
