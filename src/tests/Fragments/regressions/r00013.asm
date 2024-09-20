;;; the infinite loop starting at l016A causes the structuring phase to keel over.

fn1796_0003 proc
	db      0x90
	db      0x90
	db      0x90
	push	ds
	mov	ax,0x0800
	mov	ds,ax
	mov	word ptr cs:[0x0001],ax
	pop	word ptr [0x5422]
	and	esp,0x0000FFFF
	mov	ax,[0x5420]
	mov	word ptr [0x0066],ax
	mov	ah,0x4A
	mov	bx,0xFFFF
	mov	es,[0x5422]
	int	0x21
	mov	ax,[0x5422]
	add	ax,bx
	mov	word ptr [0x53CD],ax
	mov	ah,0x4A
	int	0x21
	mov	word ptr [0x0400],0x1758
	call	fn6CA4
	mov	al,0x24
	push cs
	pop	ds
	mov	dx,offset fn0D15
	mov	ah,0x25
	int	0x21
	push	0x0800
	pop	ds
	call	fn073A
	call	fn05EB
	cmp	word ptr [0x541E],0x00
	jz	l0067

l005C:
	call	fnC489

l0067:
	call	fn2660
	mov	dx,0x53F4
	call	fnC54C

l0078:
	mov	word ptr [0x53FE],0x0001
	call	fn065B
	mov	word ptr [0x53C2],0x4AF3
	lea	si,[0x5447]
	call	fn4AFB
	call	fn172D
	jnc	l009A

l0093:
	call	fn13DA
	jc	long l047A

l009A:
	cmp	word ptr [0x81B0],0x00
	jz	l00A8

l00A1:
	cmp	word ptr [0x5404],0x20
	jl	l009A

l00A8:
	lea	si,[0x5450]
	call	fn4AFB
	mov	word ptr [0x5404],0x0000
	call	fn04BA
	cmp	word ptr [0x53B8],0x04
	jl	long l049E

l00C1:
	mov	bx,[0x53B8]
	dec	bx
	mov	word ptr [0x53BE],bx
	dec	bx
	mov	word ptr [0x53C0],bx
	mov	bx,[0x53C0]
	call	fn0540
	mov	es,[0x53C2]
	xor	di,di
	push	0x6C34
	pop	ds
	xor	si,si
	mov	cx,0x4000

l00E5:
	rep	
	movsd	

l00E8:
	push	0x0800
	pop	ds
	mov	bx,[0x53BE]
	call	fn0540
	mov	es,[0x53C2]
	xor	di,di
	push	0x5D02
	pop	ds
	xor	si,si
	mov	cx,0x4000

l0102:
	rep	
	movsd	

l0105:
	push	0x0800
	pop	ds
	mov	ax,[0x53BE]
	mov	word ptr [0x7E52],ax
	mov	ax,[0x53C2]
	mov	word ptr [0x7E50],ax
	mov	word ptr [0x5380],ax
	mov	ax,[0x53B8]
	sub	ax,0x0002
	mov	word ptr [0x53BC],ax
	call	fn0579
	call	fn2C30
	cmp	word ptr [0x54A6],0x00
	jz	l014A

l012E:
	call	fn2614
	movsx	eax,word ptr [0x5418]
	mov	edx,0x0000F000
	imul	edx
	sub	dword ptr [0x6FF0],eax
	sbb	dword ptr [0x6FF4],edx

l014A:
	call	fn1334

l014D:
	cmp	word ptr [0x81B0],0x00
	jz	l015B

l0154:
	cmp	word ptr [0x5404],0x46
	jl	l014D

l015B:
	mov	dx,0x53E7
	call	fnC54C
	call	fnA3AA
	call	fn2900
	call	fn2A90
	call	fn1760
	call	fn1370

l016A:
	test	word ptr [0x919E],0x01FF
	jz	l01E0

l0182:
	call	fn7B17
	cmp	word ptr [0x0480],0x00
	jz	l0199

l018C:
	call	fnC010

l0197:
	jmp	l019F

l0199:
	mov	word ptr [0xC3CF],0x0000

l019F:
	cmp	word ptr [0x542C],0x00
	jnz	l01B7

l01A6:
	call	fn02B9
	call	fn1100

l01B4:
	jmp	l01CB
l01B7:
	mov	dword ptr [0x540C],0x00000000
	call	fn1100

l01CB:
	cmp	word ptr [0xC3CF],0x00
	jz	l01DA

l01D4:
	mov	word ptr [0x0412],0x0000

l01DA:
	mov	word ptr [0x5376],0x0000

l01E0:
	call	fn19A2
	pushf	
	cli	
	call	fn0C81
	mov	bx,0x0000
	xchg	word ptr [0x5404],bx
	popf	
	mov	dx,[0x541A]
	mov	word ptr [0x541A],ax
	sub	ax,dx
	sbb	bx,0x00
	mov	al,ah
	mov	ah,bl
	mov	cx,0x3865
	mul	cx
	mov	word ptr [0x5408],dx
	mov	bx,0x59F0
	mov	si,[bx+0x12]
	test	word ptr [si+0x0094],0xFFFF
	jnz	l0229

l0219:
	mov	bx,0x5BC6
	mov	si,[bx+0x12]
	test	word ptr [si+0x0094],0xFFFF
	jz	l022F

l0229:
	mov	word ptr [0x5408],0x0000

l022F:
	test	word ptr [0x919E],0x02FF
	jnz	l023F

l0239:
	mov	word ptr [0x5408],0x0044

l023F:
	mov	ax,[0x919E]
	and	ax,0x05FF
	cmp	ax,0x0400
	jz	long l016A

l024C:
	cmp	word ptr [0x632E],0x00
	jnz	l025D

l0253:
	cmp	word ptr [0xD10E],0x00
	jz	l025D

l025A:
	call	fnB938

l025D:
	call	fn2AF5
	test	byte ptr [0x6FCA],0x10
	jz	l026D

l0267:
	mov	word ptr [0x5376],0x0001

l026D:
	call	fn7D05
	test	word ptr [0x0412],0xFFFF
	jz	l027D

l027A:
	call	fn03D1

l027D:
	call	fnC721

l0288:
	jmp	l016A


l047A:
l049E:
	mov  al,0x01
	mov  ah,0x4C
	int	 0x21

fn02B9 proc
	ret
	endp
	
fn03D1 proc
	ret
	endp

fn04BA proc
	ret
	endp

fn0540 proc
	mov [0x540],bx
	ret
	endp
	
fn0579 proc
	ret
	endp

fn05EB proc
	ret
	endp

fn065B proc
	ret
	endp


fn073A proc
	ret
	endp


fn0C81 proc
	mov ax,0x42
	ret
	endp


fn1100 proc
	ret
	endp


fn1334 proc
	ret
	endp


fn1370 proc
	ret
	endp


fn13DA proc
	push ax
	mov ax,[0x1234]
	cmp ax,[0x1236]
	ret
	endp


fn172D proc
	push ax
	mov ax,[0x1234]
	cmp ax,[0x1236]
	ret
	endp


fn1760 proc
	ret
	endp


fn19A2 proc
	ret
	endp


fn2614 proc
	ret
	endp


fn2660 proc
	ret
	endp


fn2900 proc
	ret
	endp


fn2A90 proc
	ret
	endp


fn2AF5 proc
	ret
	endp


fn2C30 proc
	ret
	endp


fn4AFB proc
	mov	[0x4AFB],si
	ret
	endp


fn6CA4 proc
	ret
	endp


fn7B17 proc
	ret
	endp


fn7D05 proc
	ret
	endp


fnA3AA proc
	ret
	endp


fnB938 proc
	ret
	endp


fnC010 proc
	ret
	endp


fnC489 proc
	ret
	endp


fnC54C proc
	mov [0xC54C],dx
	ret
	endp


fnC721 proc
	ret
	endp


fn0D15 proc
	ret
	endp
	