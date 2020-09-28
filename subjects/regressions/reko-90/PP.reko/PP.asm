;;; Segment 0800 (0800:0000)
0800:0000 BA 83 14 2E 89 16 5A 02 B4 30 CD 21 8B 2E 02 00 ......Z..0.!....
0800:0010 8B 1E 2C 00 8E DA A3 7D 00 8C 06 7B 00 89 1E 77 ..,....}...{...w
0800:0020 00 89 2E 91 00 E8 4E 01 C4 3E 75 00 8B C7 8B D8 ......N..>u.....
0800:0030 B9 FF 7F FC F2 AE E3 43 43 26 38 05 75 F6 80 CD .......CC&8.u...
0800:0040 80 F7 D9 89 0E 75 00 B9 02 00 D3 E3 83 C3 10 83 .....u..........
0800:0050 E3 F0 89 1E 79 00 8C D2 2B EA BF 83 14 8E C7 26 ....y...+......&
0800:0060 8B 3E FC 25 81 FF 00 02 73 08 BF 00 02 26 89 3E .>.%....s....&.>
0800:0070 FC 25 B1 04 D3 EF 47 3B EF 73 03 E9 C7 01 8B DF .%....G;.s......
0800:0080 03 DA 89 1E 89 00 89 1E 8D 00 A1 7B 00 2B D8 8E ...........{.+..
0800:0090 C0 B4 4A 57 CD 21 5F D3 E7 FA 8E D2 8B E7 FB B8 ..JW.!_.........
0800:00A0 83 14 8E C0 26 89 3E FC 25 33 C0 2E 8E 06 5A 02 ....&.>.%3....Z.
0800:00B0 BF 14 27 B9 F4 4E 2B CF FC F3 AA 83 3E E8 24 14 ..'..N+.....>.$.
0800:00C0 76 47 80 3E 7D 00 03 72 40 77 07 80 3E 7E 00 1E vG.>}..r@w..>~..
0800:00D0 72 37 B8 01 58 BB 02 00 CD 21 72 2A B4 67 8B 1E r7..X....!r*.g..
0800:00E0 E8 24 CD 21 72 20 B4 48 BB 01 00 CD 21 72 17 40 .$.!r .H....!r.@
0800:00F0 A3 91 00 48 8E C0 B4 49 CD 21 72 0A B8 01 58 BB ...H...I.!r...X.
0800:0100 00 00 CD 21 73 03 E9 3C 01 B4 00 CD 1A 89 16 81 ...!s..<........
0800:0110 00 89 0E 83 00 0A C0 74 0C B8 40 00 8E C0 BB 70 .......t..@....p
0800:0120 00 26 C6 07 01 33 ED 2E 8E 06 5A 02 BE FC 26 BF .&...3....Z...&.
0800:0130 14 27 E8 B1 00 FF 36 73 00 FF 36 71 00 FF 36 6F .'....6s..6q..6o
0800:0140 00 FF 36 6D 00 FF 36 6B 00 E8 12 01 50 E8 0A 8A ..6m..6k....P...

;; fn0800_0150: 0800:0150
;;   Called from:
;;     0800:8B2F (in fn0800_8B0D)
fn0800_0150 proc
	mov	es,cs:[025A]
	push	si
	push	di
	mov	si,2714
	mov	di,2714
	call	01E6
	pop	di
	pop	si
	ret

;; fn0800_0163: 0800:0163
;;   Called from:
;;     0800:8B39 (in fn0800_8B0D)
fn0800_0163 proc
	ret

;; fn0800_0164: 0800:0164
;;   Called from:
;;     0800:8B51 (in fn0800_8B0D)
fn0800_0164 proc
	mov	bp,sp
	mov	ah,4C
	mov	al,[bp+02]
	int	21
0800:016D                                        B9 0E 00              ...
0800:0170 BA 2F 00 E9 D5 00                               ./....         

;; fn0800_0176: 0800:0176
fn0800_0176 proc
	push	ds
	mov	ax,3500
	int	21
	mov	[005B],bx
	mov	[005D],es
	mov	ax,3504
	int	21
	mov	[005F],bx
	mov	[0061],es
	mov	ax,3505
	int	21
	mov	[0063],bx
	mov	[0065],es
	mov	ax,3506
	int	21
	mov	[0067],bx
	mov	[0069],es
	mov	ax,2500
	mov	dx,cs
	mov	ds,dx
	mov	dx,016D
	int	21
	pop	ds
	ret

;; fn0800_01B9: 0800:01B9
;;   Called from:
;;     0800:8B36 (in fn0800_8B0D)
fn0800_01B9 proc
	push	ds
	mov	ax,2500
	lds	dx,[005B]
	int	21
	pop	ds
	push	ds
	mov	ax,2504
	lds	dx,[005F]
	int	21
	pop	ds
	push	ds
	mov	ax,2505
	lds	dx,[0063]
	int	21
	pop	ds
	push	ds
	mov	ax,2506
	lds	dx,[0067]
	int	21
	pop	ds
	ret

;; fn0800_01E6: 0800:01E6
;;   Called from:
;;     0800:015D (in fn0800_0150)
fn0800_01E6 proc
	cmp	si,26FC
	jz	01F0

l0800_01EC:
	xor	ah,ah
	jmp	01F2

l0800_01F0:
	mov	ah,FF

l0800_01F2:
	mov	dx,di
	mov	bx,si

l0800_01F6:
	cmp	bx,di
	jz	021D

l0800_01FA:
	cmp	byte ptr es:[bx],FF
	jz	0218

l0800_0200:
	cmp	si,26FC
	jz	020C

l0800_0206:
	cmp	ah,es:[bx+01]
	jmp	0210

l0800_020C:
	cmp	es:[bx+01],ah

l0800_0210:
	ja	0218

l0800_0212:
	mov	ah,es:[bx+01]
	mov	dx,bx

l0800_0218:
	add	bx,06
	jmp	01F6

l0800_021D:
	cmp	dx,di
	jz	023C

l0800_0221:
	mov	bx,dx
	cmp	byte ptr es:[bx],00
	mov	byte ptr es:[bx],FF
	push	es
	jz	0235

l0800_022E:
	call	dword ptr es:[bx+02]
	pop	es
	jmp	01E6

l0800_0235:
	call	word ptr es:[bx+02]
	pop	es
	jmp	01E6

l0800_023C:
	ret

;; fn0800_023D: 0800:023D
fn0800_023D proc
	mov	ah,40
	mov	bx,0002
	int	21
	ret
0800:0245                B9 1E 00 BA 3D 00 2E 8E 1E 5A 02      ....=....Z.
0800:0250 E8 EA FF B8 03 00 50 E8 0F 89 00 00 03 40       ......P......@ 

;; main: 0800:025E
main proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	mov	word ptr [2A27],0001
	mov	word ptr [2E4D],0000
	mov	word ptr [2A1F],0000
	mov	word ptr [2E4F],0000
	mov	word ptr [2A1D],0000
	mov	word ptr [2A17],0000
	mov	word ptr [2A1B],0000
	mov	word ptr [2A11],0000
	mov	word ptr [2A0F],0000
	mov	word ptr [2A0D],0000
	mov	word ptr [2A0B],3000
	mov	word ptr [2E31],8000
	mov	word ptr [2E2F],1000
	mov	word ptr [2A21],0001
	mov	word ptr [29F5],0000
	mov	word ptr [29F3],0000
	mov	word ptr [29F1],0000
	mov	word ptr [29EF],0000
	mov	word ptr [2A13],0000
	mov	word ptr [2A23],0001
	call	2C9A
	push	ds
	mov	ax,0094
	push	ax
	push	ds
	mov	ax,07E8
	push	ax
	call	B2EF
	add	sp,08
	call	0402
	call	0541
	mov	ax,0001
	push	ax
	push	ax
	call	2DBF
	add	sp,04
	xor	ax,ax
	push	ax
	push	ax
	call	9764
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	bx,[2A25]
	cmp	bx,08
	ja	0338

l0800_031A:
	shl	bx,01
	jmp	word ptr cs:[bx+03F0]

l0800_0321:
	call	0DE8
	jmp	0338

l0800_0326:
	call	12E2
	jmp	0338

l0800_032B:
	call	18D9
	jmp	0338

l0800_0330:
	call	112D
	jmp	0338

l0800_0335:
	call	19EE

l0800_0338:
	xor	ax,ax
	push	ax
	push	ax
	call	9764
	add	sp,04
	sub	ax,[bp-04]
	sbb	dx,[bp-02]
	mov	[bp-02],dx
	mov	[bp-04],ax
	push	word ptr [29F1]
	push	word ptr [29EF]
	push	word ptr [29F5]
	push	word ptr [29F3]
	call	0B79
	add	sp,08
	mov	si,ax
	xor	ax,ax
	mov	dx,003C
	push	ax
	push	dx
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	8BCA
	push	dx
	push	ax
	xor	ax,ax
	mov	dx,003C
	push	ax
	push	dx
	mov	dx,0E10
	push	ax
	push	dx
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	8BCA
	push	dx
	push	ax
	call	8BBB
	push	dx
	push	ax
	xor	ax,ax
	mov	dx,0E10
	push	ax
	push	dx
	mov	dx,5180
	push	ax
	push	dx
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	8BCA
	push	dx
	push	ax
	call	8BBB
	push	dx
	push	ax
	mov	ax,si
	mov	bx,0064
	xor	dx,dx
	div	bx
	push	dx
	mov	ax,si
	xor	dx,dx
	div	bx
	push	ax
	push	word ptr [29F1]
	push	word ptr [29EF]
	push	word ptr [29F5]
	push	word ptr [29F3]
	push	ds
	mov	ax,05DC
	push	ax
	mov	ax,0008
	push	ax
	push	word ptr [2A13]
	push	ds
	mov	ax,07EB
	push	ax
	call	B2EF
	add	sp,24
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret
l0800_03F0	dw	0x0321
l0800_03F2	dw	0x0330
l0800_03F4	dw	0x0330
l0800_03F6	dw	0x0326
l0800_03F8	dw	0x0326
l0800_03FA	dw	0x032B
l0800_03FC	dw	0x0335
l0800_03FE	dw	0x0335
l0800_0400	dw	0x0335

;; fn0800_0402: 0800:0402
;;   Called from:
;;     0800:02F0 (in main)
fn0800_0402 proc
	push	si
	push	di
	mov	ax,[2A27]
	cmp	ax,[269A]
	jnz	0410

l0800_040D:
	call	0DCE

l0800_0410:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	push	word ptr es:[bx+02]
	push	word ptr es:[bx]
	call	BFC7
	add	sp,04
	cmp	ax,0001
	jbe	044D

l0800_042F:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	push	word ptr es:[bx+02]
	push	word ptr es:[bx]
	mov	ax,0001
	push	ax
	call	0D24
	add	sp,06

l0800_044D:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	les	bx,es:[bx]
	mov	al,es:[bx]
	push	ax
	push	ds
	mov	ax,0829
	push	ax
	call	0C29
	add	sp,06
	mov	[2A25],ax
	cmp	ax,0009
	jl	0492

l0800_0474:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	push	word ptr es:[bx+02]
	push	word ptr es:[bx]
	mov	ax,0001
	push	ax
	call	0D24
	add	sp,06

l0800_0492:
	inc	word ptr [2A27]
	cmp	word ptr [2A25],02
	jg	04A0

l0800_049D:
	jmp	053E

l0800_04A0:
	mov	ax,[2A27]
	cmp	ax,[269A]
	jnz	04AC

l0800_04A9:
	call	0DCE

l0800_04AC:
	mov	ax,[2A27]
	inc	word ptr [2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	push	word ptr es:[bx+02]
	push	word ptr es:[bx]
	push	ds
	mov	ax,4348
	push	ax
	call	BF9E
	add	sp,08
	push	ds
	mov	ax,4348
	push	ax
	call	0C6C
	add	sp,04
	push	ds
	pop	es
	mov	di,4348
	xor	ax,ax
	mov	cx,FFFF

l0800_04E4:
	repne scasb

l0800_04E6:
	not	cx
	mov	ax,002E
	dec	di
	std

l0800_04ED:
	repne scasb

l0800_04EF:
	jz	04F8

l0800_04F1:
	mov	di,FFFF
	xor	ax,ax
	mov	es,ax

l0800_04F8:
	inc	di
	cld
	mov	ax,es
	push	ds
	pop	es
	push	di
	mov	di,4348
	xor	ax,ax
	mov	cx,FFFF

l0800_0507:
	repne scasb

l0800_0509:
	not	cx
	mov	ax,005C
	sub	di,cx

l0800_0510:
	repne scasb

l0800_0512:
	jz	051B

l0800_0514:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_051B:
	dec	di
	mov	ax,es
	pop	ax
	cmp	ax,di
	ja	0538

l0800_0523:
	push	ds
	pop	es
	mov	di,4348
	mov	si,0833
	mov	cx,FFFF
	xor	ax,ax

l0800_0530:
	repne scasb

l0800_0532:
	dec	di
	mov	cx,0005

l0800_0536:
	rep movsb

l0800_0538:
	mov	word ptr [2A19],0001

l0800_053E:
	pop	di
	pop	si
	ret

;; fn0800_0541: 0800:0541
;;   Called from:
;;     0800:02F3 (in main)
fn0800_0541 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	di
	mov	ax,[2A27]
	cmp	ax,[269A]
	jnz	0555

l0800_0552:
	jmp	0987

l0800_0555:
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	les	bx,es:[bx]
	cmp	byte ptr es:[bx],2D
	jnz	056B

l0800_0568:
	jmp	086D

l0800_056B:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	les	bx,es:[bx]
	cmp	byte ptr es:[bx],2F
	jnz	0584

l0800_0581:
	jmp	086D

l0800_0584:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_058A:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	les	bx,es:[bx]
	mov	al,es:[bx+01]
	push	ax
	push	ds
	mov	ax,0838
	push	ax
	call	0C29
	add	sp,06
	mov	si,ax
	cmp	ax,000B
	jl	05CF

l0800_05B1:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	push	word ptr es:[bx+02]
	push	word ptr es:[bx]
	mov	ax,0002
	push	ax
	call	0D24
	add	sp,06

l0800_05CF:
	cmp	si,06
	jl	05D7

l0800_05D4:
	jmp	0659

l0800_05D7:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	push	word ptr es:[bx+02]
	push	word ptr es:[bx]
	call	BFC7
	add	sp,04
	cmp	ax,0002
	jnz	0622

l0800_05F6:
	inc	word ptr [2A27]
	mov	ax,[2A27]
	cmp	ax,[269A]
	jnz	0606

l0800_0603:
	call	0DCE

l0800_0606:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	mov	ax,es:[bx+02]
	mov	dx,es:[bx]
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	063F

l0800_0622:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	mov	ax,es:[bx+02]
	mov	dx,es:[bx]
	add	dx,02
	mov	[bp-02],ax
	mov	[bp-04],dx

l0800_063F:
	les	bx,[bp-04]
	mov	al,es:[bx]
	cbw
	or	ax,ax
	jnz	0659

l0800_064A:
	push	ds
	mov	ax,05DC
	push	ax
	mov	ax,0002
	push	ax
	call	0D24
	add	sp,06

l0800_0659:
	mov	bx,si
	cmp	bx,0A
	jbe	0663

l0800_0660:
	jmp	0869

l0800_0663:
	shl	bx,01
	jmp	word ptr cs:[bx+098D]

l0800_066A:
	mov	word ptr [2A1F],0001
	mov	word ptr [2E4F],0000
	jmp	0869

l0800_0679:
	mov	word ptr [2A1D],0001
	jmp	0869

l0800_0682:
	mov	word ptr [2A1B],0001
	jmp	0869

l0800_068B:
	mov	word ptr [2A17],0001
	jmp	0869

l0800_0694:
	mov	word ptr [2A15],0001
	jmp	0869

l0800_069D:
	cmp	word ptr [2A19],00
	jz	06A7

l0800_06A4:
	jmp	0869

l0800_06A7:
	les	di,[bp-04]
	xor	ax,ax
	mov	cx,FFFF

l0800_06AF:
	repne scasb

l0800_06B1:
	not	cx
	dec	cx
	cmp	cx,01
	jbe	06C9

l0800_06B9:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	mov	ax,0003
	push	ax
	call	0D24
	add	sp,06

l0800_06C9:
	les	bx,[bp-04]
	mov	al,es:[bx]
	push	ax
	push	ds
	mov	ax,0844
	push	ax
	call	0C29
	add	sp,06
	mov	[2A23],ax
	cmp	ax,0006
	jl	06F3

l0800_06E3:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	mov	ax,0003
	push	ax
	call	0D24
	add	sp,06

l0800_06F3:
	mov	bx,[2A23]
	shl	bx,01
	shl	bx,01
	push	word ptr [bx+0547]
	push	word ptr [bx+0545]
	push	ds
	mov	ax,084B
	push	ax
	push	ds
	mov	ax,4271
	push	ax
	call	BEA2
	add	sp,0C
	jmp	0869

l0800_0716:
	push	ds
	mov	ax,2E4F
	push	ax
	push	ds
	mov	ax,084F
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	BF18
	add	sp,0C
	cmp	word ptr [2E4F],00
	jnz	0743

l0800_0733:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	mov	ax,0004
	push	ax
	call	0D24
	add	sp,06

l0800_0743:
	mov	word ptr [2A1F],0000
	jmp	0869

l0800_074C:
	push	ds
	mov	ax,2A0F
	push	ax
	push	ds
	mov	ax,0853
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	BF18
	add	sp,0C
	jmp	0869

l0800_0765:
	push	ds
	mov	ax,2A21
	push	ax
	push	ds
	mov	ax,0856
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	BF18
	add	sp,0C
	cmp	word ptr [2A21],02
	jg	0796

l0800_0782:
	cmp	word ptr [2A21],00
	jz	078C

l0800_0789:
	jmp	0869

l0800_078C:
	cmp	word ptr [2A25],00
	jz	0796

l0800_0793:
	jmp	0869

l0800_0796:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	mov	ax,0005
	push	ax
	call	0D24
	add	sp,06
	jmp	0869

l0800_07A9:
	push	ds
	pop	es
	mov	di,42E3
	push	es
	mov	es,[bp-02]
	push	di
	mov	di,[bp-04]
	xor	ax,ax
	mov	cx,FFFF

l0800_07BB:
	repne scasb

l0800_07BD:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_07CC:
	rep movsw

l0800_07CE:
	adc	cx,cx

l0800_07D0:
	rep movsb

l0800_07D2:
	pop	ds
	push	ds
	mov	ax,42E3
	push	ax
	call	0C6C
	add	sp,04
	push	ds
	pop	es
	mov	di,42E3
	xor	ax,ax
	mov	cx,FFFF

l0800_07E8:
	repne scasb

l0800_07EA:
	not	cx
	dec	cx
	dec	cx
	mov	bx,cx
	cmp	byte ptr [bx+42E3],5C
	jz	0869

l0800_07F7:
	mov	di,42E3
	mov	si,0859
	mov	cx,FFFF

l0800_0800:
	repne scasb

l0800_0802:
	dec	di
	mov	cx,0002

l0800_0806:
	rep movsb

l0800_0808:
	jmp	0869

l0800_080A:
	push	ds
	pop	es
	mov	di,427E
	push	es
	mov	es,[bp-02]
	push	di
	mov	di,[bp-04]
	xor	ax,ax
	mov	cx,FFFF

l0800_081C:
	repne scasb

l0800_081E:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_082D:
	rep movsw

l0800_082F:
	adc	cx,cx

l0800_0831:
	rep movsb

l0800_0833:
	pop	ds
	push	ds
	mov	ax,427E
	push	ax
	call	0C6C
	add	sp,04
	push	ds
	pop	es
	mov	di,427E
	xor	ax,ax
	mov	cx,FFFF

l0800_0849:
	repne scasb

l0800_084B:
	not	cx
	dec	cx
	dec	cx
	mov	bx,cx
	cmp	byte ptr [bx+427E],5C
	jz	0869

l0800_0858:
	mov	di,427E
	mov	si,0859
	mov	cx,FFFF

l0800_0861:
	repne scasb

l0800_0863:
	dec	di
	mov	cx,0002

l0800_0867:
	rep movsb

l0800_0869:
	inc	word ptr [2A27]

l0800_086D:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	les	bx,es:[bx]
	cmp	byte ptr es:[bx],2D
	jnz	0886

l0800_0883:
	jmp	058A

l0800_0886:
	mov	ax,[2A27]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	les	bx,es:[bx]
	cmp	byte ptr es:[bx],2F
	jnz	089F

l0800_089C:
	jmp	058A

l0800_089F:
	mov	ax,[2A21]
	cmp	ax,0001
	jz	08AE

l0800_08A7:
	cmp	ax,0002
	jz	08D9

l0800_08AC:
	jmp	0902

l0800_08AE:
	push	ds
	pop	es
	mov	di,4271
	mov	si,085B
	mov	cx,FFFF
	xor	ax,ax

l0800_08BB:
	repne scasb

l0800_08BD:
	dec	di
	mov	cx,0002

l0800_08C1:
	rep movsb

l0800_08C3:
	cmp	word ptr [2E31],8000
	jbe	08D1

l0800_08CB:
	mov	word ptr [2E31],8000

l0800_08D1:
	mov	word ptr [2E2F],1000
	jmp	0902

l0800_08D9:
	push	ds
	pop	es
	mov	di,4271
	mov	si,085D
	mov	cx,FFFF
	xor	ax,ax

l0800_08E6:
	repne scasb

l0800_08E8:
	dec	di
	mov	cx,0002

l0800_08EC:
	rep movsb

l0800_08EE:
	cmp	word ptr [2E31],1000
	jbe	08FC

l0800_08F6:
	mov	word ptr [2E31],1000

l0800_08FC:
	mov	word ptr [2E2F],00FF

l0800_0902:
	cmp	word ptr [2A23],02
	jnz	0911

l0800_0909:
	xor	ax,ax
	mov	[2A1D],ax
	mov	[2E4F],ax

l0800_0911:
	cmp	word ptr [2E4F],00
	jz	092D

l0800_0918:
	push	ds
	pop	es
	mov	di,4271
	mov	si,0653
	mov	cx,FFFF
	xor	ax,ax

l0800_0925:
	repne scasb

l0800_0927:
	dec	di
	mov	cx,0002

l0800_092B:
	rep movsb

l0800_092D:
	cmp	word ptr [2A1D],00
	jz	0949

l0800_0934:
	push	ds
	pop	es
	mov	di,4271
	mov	si,085F
	mov	cx,FFFF
	xor	ax,ax

l0800_0941:
	repne scasb

l0800_0943:
	dec	di
	mov	cx,0002

l0800_0947:
	rep movsb

l0800_0949:
	push	ds
	pop	es
	mov	di,4271
	mov	si,0861
	mov	cx,FFFF
	xor	ax,ax

l0800_0956:
	repne scasb

l0800_0958:
	dec	di
	mov	cx,0005

l0800_095C:
	rep movsb

l0800_095E:
	cmp	word ptr [2A25],00
	jnz	0987

l0800_0965:
	mov	bx,[2A23]
	shl	bx,01
	shl	bx,01
	les	bx,[bx+0545]
	cmp	byte ptr es:[bx],00
	jz	0987

l0800_0977:
	push	ds
	mov	ax,2E75
	push	ax
	push	ds
	mov	ax,4271
	push	ax
	call	09A3
	add	sp,08

l0800_0987:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret
l0800_098D	dw	0x069D
l0800_098F	dw	0x0716
l0800_0991	dw	0x0765
l0800_0993	dw	0x080A
l0800_0995	dw	0x07A9
l0800_0997	dw	0x074C
l0800_0999	dw	0x066A
l0800_099B	dw	0x0682
l0800_099D	dw	0x0679
l0800_099F	dw	0x068B
l0800_09A1	dw	0x0694

;; fn0800_09A3: 0800:09A3
;;   Called from:
;;     0800:0981 (in fn0800_0541)
;;     0800:5EBD (in fn0800_5E64)
;;     0800:670F (in fn0800_669C)
fn0800_09A3 proc
	push	bp
	mov	bp,sp
	sub	sp,08
	push	si
	push	di
	mov	al,[0A72]
	cbw
	mov	cl,08
	shl	ax,cl
	mov	dl,[0A73]
	mov	dh,00
	add	ax,dx
	xor	dx,dx
	push	ax
	mov	al,[0A74]
	cbw
	shl	ax,cl
	mov	bl,[0A75]
	mov	bh,00
	add	ax,bx
	add	dx,ax
	pop	ax
	adc	ax,0000
	add	dx,20
	adc	ax,0000
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [bp-02]
	push	ax
	push	ds
	mov	ax,0A6E
	push	ax
	nop
	push	cs
	call	867A
	add	sp,08
	jmp	0A4F

l0800_09FF:
	les	di,[bp-08]
	xor	ax,ax
	mov	cx,FFFF

l0800_0A07:
	repne scasb

l0800_0A09:
	not	cx
	dec	cx
	mov	ax,[bp-08]
	add	ax,cx
	mov	bx,ax
	mov	al,es:[bx+01]
	cbw
	mov	cl,08
	shl	ax,cl
	mov	di,[bp-08]
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_0A25:
	repne scasb

l0800_0A27:
	not	cx
	dec	cx
	mov	ax,[bp-08]
	add	ax,cx
	mov	bx,ax
	mov	al,es:[bx+02]
	mov	ah,00
	pop	dx
	add	dx,ax
	mov	di,[bp-08]
	xor	ax,ax
	mov	cx,FFFF

l0800_0A42:
	repne scasb

l0800_0A44:
	not	cx
	dec	cx
	add	dx,cx
	add	dx,03
	add	[bp-08],dx

l0800_0A4F:
	mov	si,[bp+04]
	push	ds
	mov	ds,[bp+06]
	les	di,[bp-08]
	xor	ax,ax
	mov	cx,FFFF

l0800_0A5E:
	repne scasb

l0800_0A60:
	not	cx
	sub	di,cx

l0800_0A64:
	rep cmpsb

l0800_0A66:
	jz	0A6D

l0800_0A68:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_0A6D:
	pop	ds
	or	ax,ax
	jnz	09FF

l0800_0A72:
	les	di,[bp-08]
	xor	ax,ax
	mov	cx,FFFF

l0800_0A7A:
	repne scasb

l0800_0A7C:
	not	cx
	dec	cx
	inc	cx
	add	[bp-08],cx
	les	bx,[bp-08]
	mov	al,es:[bx]
	cbw
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+01]
	mov	dh,00
	add	ax,dx
	add	ax,0002
	push	ax
	push	word ptr [bp-06]
	push	bx
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	B0F3
	add	sp,0A
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	4346
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_0ABC: 0800:0ABC
;;   Called from:
;;     0800:10EE (in fn0800_0DE8)
;;     0800:12BC (in fn0800_112D)
;;     0800:1869 (in fn0800_12E2)
;;     0800:19B6 (in fn0800_18D9)
;;     0800:1CAE (in fn0800_19EE)
fn0800_0ABC proc
	push	bp
	mov	bp,sp
	sub	sp,0E
	push	si
	mov	si,[bp+04]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	ss
	lea	ax,[bp-0E]
	push	ax
	call	35A3
	add	sp,08
	push	word ptr [2A05]
	push	word ptr [2A03]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	0B79
	add	sp,08
	mov	cx,ax
	mov	bx,0064
	xor	dx,dx
	div	bx
	push	dx
	mov	ax,cx
	xor	dx,dx
	div	bx
	push	ax
	push	word ptr [2A05]
	push	word ptr [2A03]
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	ss
	lea	ax,[bp-0E]
	push	ax
	mov	ax,0014
	push	ax
	push	ax
	push	ds
	mov	ax,0866
	push	ax
	call	B2EF
	add	sp,18
	cmp	word ptr [2A25],02
	jz	0B2E

l0800_0B2A:
	or	si,si
	jz	0B47

l0800_0B2E:
	mov	bx,si
	shl	bx,01
	shl	bx,01
	push	word ptr [bx+059B]
	push	word ptr [bx+0599]
	push	ds
	mov	ax,0889
	push	ax
	call	B2EF
	add	sp,08

l0800_0B47:
	push	ds
	mov	ax,0827
	push	ax
	call	B2EF
	add	sp,04
	mov	ax,[2A09]
	mov	dx,[2A07]
	add	[29F3],dx
	adc	[29F5],ax
	mov	ax,[2A05]
	mov	dx,[2A03]
	add	[29EF],dx
	adc	[29F1],ax
	inc	word ptr [2A13]
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_0B79: 0800:0B79
;;   Called from:
;;     0800:035E (in main)
;;     0800:0AE7 (in fn0800_0ABC)
fn0800_0B79 proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+04]
	or	ax,[bp+06]
	jz	0B9E

l0800_0B84:
	mov	ax,[bp+08]
	or	ax,[bp+0A]
	jz	0B9E

l0800_0B8C:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	cmp	ax,[bp+0A]
	ja	0BCE

l0800_0B97:
	jnz	0B9E

l0800_0B99:
	cmp	dx,[bp+08]
	ja	0BCE

l0800_0B9E:
	xor	ax,ax
	pop	bp
	ret

l0800_0BA2:
	xor	ax,ax
	mov	dx,000A
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BC2
	mov	[bp+06],dx
	mov	[bp+04],ax
	xor	ax,ax
	mov	dx,000A
	push	ax
	push	dx
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	8BC2
	mov	[bp+0A],dx
	mov	[bp+08],ax

l0800_0BCE:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	sub	dx,[bp+08]
	sbb	ax,[bp+0A]
	cmp	ax,0006
	ja	0BA2

l0800_0BDF:
	jnz	0BE7

l0800_0BE1:
	cmp	dx,8DB8
	ja	0BA2

l0800_0BE7:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	mov	cx,[bp+06]
	mov	bx,[bp+04]
	sub	bx,[bp+08]
	sbb	cx,[bp+0A]
	xor	dx,dx
	mov	ax,2710
	call	8F18
	push	dx
	push	ax
	call	8BC2
	pop	bp
	ret

;; fn0800_0C08: 0800:0C08
;;   Called from:
;;     0800:832A (in fn0800_831D)
;;     0800:8418 (in fn0800_8407)
fn0800_0C08 proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+04]
	mov	cx,0001
	jmp	0C15

l0800_0C14:
	inc	cx

l0800_0C15:
	mov	bx,0002
	mov	ax,si
	xor	dx,dx
	div	bx
	mov	si,ax
	or	ax,ax
	jnz	0C14

l0800_0C24:
	mov	ax,cx
	pop	si
	pop	bp
	ret

;; fn0800_0C29: 0800:0C29
;;   Called from:
;;     0800:0466 (in fn0800_0402)
;;     0800:05A4 (in fn0800_0541)
;;     0800:06D5 (in fn0800_0541)
fn0800_0C29 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	mov	cl,[bp+08]
	mov	al,cl
	cbw
	push	ax
	call	97CC
	add	sp,02
	mov	cl,al
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	0C4F

l0800_0C4C:
	inc	word ptr [bp-04]

l0800_0C4F:
	les	bx,[bp-04]
	cmp	byte ptr es:[bx],00
	jz	0C5D

l0800_0C58:
	cmp	es:[bx],cl
	jnz	0C4C

l0800_0C5D:
	mov	ax,[bp-04]
	xor	dx,dx
	sub	ax,[bp+04]
	sbb	dx,00
	mov	sp,bp
	pop	bp
	ret

;; fn0800_0C6C: 0800:0C6C
;;   Called from:
;;     0800:04D4 (in fn0800_0402)
;;     0800:07D8 (in fn0800_0541)
;;     0800:0839 (in fn0800_0541)
;;     0800:2EBD (in fn0800_2DE2)
;;     0800:2FA2 (in fn0800_2DE2)
;;     0800:3141 (in fn0800_2DE2)
fn0800_0C6C proc
	push	bp
	mov	bp,sp
	jmp	0C88

l0800_0C71:
	les	bx,[bp+04]
	mov	al,es:[bx]
	cbw
	push	ax
	call	97CC
	add	sp,02
	les	bx,[bp+04]
	mov	es:[bx],al
	inc	word ptr [bp+04]

l0800_0C88:
	les	bx,[bp+04]
	cmp	byte ptr es:[bx],00
	jnz	0C71

l0800_0C91:
	pop	bp
	ret

;; fn0800_0C93: 0800:0C93
;;   Called from:
;;     0800:0E8E (in fn0800_0DE8)
;;     0800:11A4 (in fn0800_112D)
;;     0800:15E0 (in fn0800_12E2)
;;     0800:1945 (in fn0800_18D9)
;;     0800:1AB6 (in fn0800_19EE)
fn0800_0C93 proc
	push	bp
	mov	bp,sp
	sub	sp,66
	push	si
	push	di
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	ss
	lea	ax,[bp-66]
	push	ax
	call	3509
	add	sp,08
	mov	si,2714
	push	ss
	pop	es
	lea	di,[bp-66]
	xor	ax,ax
	mov	cx,FFFF

l0800_0CB9:
	repne scasb

l0800_0CBB:
	not	cx
	sub	di,cx

l0800_0CBF:
	rep cmpsb

l0800_0CC1:
	jz	0CC8

l0800_0CC3:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_0CC8:
	or	ax,ax
	jz	0D0D

l0800_0CCC:
	push	ss
	lea	ax,[bp-66]
	push	ax
	push	ds
	mov	ax,088F
	push	ax
	call	B2EF
	add	sp,08
	push	ss
	pop	es
	lea	di,[bp-66]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,2714
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_0CF5:
	repne scasb

l0800_0CF7:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_0D06:
	rep movsw

l0800_0D08:
	adc	cx,cx

l0800_0D0A:
	rep movsb

l0800_0D0C:
	pop	ds

l0800_0D0D:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	ds
	mov	ax,089E
	push	ax
	call	B2EF
	add	sp,08
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_0D24: 0800:0D24
;;   Called from:
;;     0800:0447 (in fn0800_0402)
;;     0800:048C (in fn0800_0402)
;;     0800:05C9 (in fn0800_0541)
;;     0800:0653 (in fn0800_0541)
;;     0800:06C3 (in fn0800_0541)
;;     0800:06ED (in fn0800_0541)
;;     0800:073D (in fn0800_0541)
;;     0800:07A0 (in fn0800_0541)
;;     0800:1368 (in fn0800_12E2)
;;     0800:1924 (in fn0800_18D9)
;;     0800:1A3A (in fn0800_19EE)
;;     0800:1D63 (in fn0800_1CF6)
;;     0800:1DA4 (in fn0800_1CF6)
;;     0800:1E53 (in fn0800_1CF6)
;;     0800:2605 (in fn0800_24FE)
;;     0800:3718 (in fn0800_3678)
;;     0800:3881 (in fn0800_37DF)
;;     0800:3DAC (in fn0800_3C99)
;;     0800:3E19 (in fn0800_3DCF)
;;     0800:3EE4 (in fn0800_3E9A)
;;     0800:4093 (in fn0800_4047)
;;     0800:414A (in fn0800_4110)
;;     0800:418C (in fn0800_4152)
;;     0800:4261 (in fn0800_4234)
;;     0800:4336 (in fn0800_4311)
fn0800_0D24 proc
	push	bp
	mov	bp,sp
	push	ds
	mov	ax,4477
	push	ax
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	0DA9
	add	sp,08
	push	ds
	mov	ax,4412
	push	ax
	push	word ptr [29D9]
	push	word ptr [29D7]
	call	0DA9
	add	sp,08
	push	ds
	mov	ax,43AD
	push	ax
	push	word ptr [29D5]
	push	word ptr [29D3]
	call	0DA9
	add	sp,08
	mov	bx,[bp+04]
	shl	bx,01
	shl	bx,01
	push	word ptr [bx+055F]
	push	word ptr [bx+055D]
	push	ds
	mov	ax,08A2
	push	ax
	call	B2EF
	add	sp,08
	les	bx,[bp+06]
	cmp	byte ptr es:[bx],00
	jz	0D92

l0800_0D83:
	push	word ptr [bp+08]
	push	bx
	push	ds
	mov	ax,08A6
	push	ax
	call	B2EF
	add	sp,08

l0800_0D92:
	push	ds
	mov	ax,0827
	push	ax
	call	B2EF
	add	sp,04
	mov	ax,0001
	push	ax
	call	8B5A
	add	sp,02
	pop	bp
	ret

;; fn0800_0DA9: 0800:0DA9
;;   Called from:
;;     0800:0D34 (in fn0800_0D24)
;;     0800:0D47 (in fn0800_0D24)
;;     0800:0D5A (in fn0800_0D24)
fn0800_0DA9 proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+04]
	or	ax,[bp+06]
	jz	0DCC

l0800_0DB4:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A614
	add	sp,04
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	8F7F
	add	sp,04

l0800_0DCC:
	pop	bp
	ret

;; fn0800_0DCE: 0800:0DCE
;;   Called from:
;;     0800:040D (in fn0800_0402)
;;     0800:04A9 (in fn0800_0402)
;;     0800:0603 (in fn0800_0541)
fn0800_0DCE proc
	push	ds
	mov	ax,0121
	push	ax
	push	ds
	mov	ax,07E8
	push	ax
	call	B2EF
	add	sp,08
	xor	ax,ax
	push	ax
	call	8B5A
	add	sp,02
	ret

;; fn0800_0DE8: 0800:0DE8
;;   Called from:
;;     0800:0321 (in main)
fn0800_0DE8 proc
	push	si
	mov	bx,[2A23]
	shl	bx,01
	shl	bx,01
	push	word ptr [bx+052F]
	push	word ptr [bx+052D]
	mov	bx,[2A25]
	shl	bx,01
	shl	bx,01
	push	word ptr [bx+050B]
	push	word ptr [bx+0509]
	push	ds
	mov	ax,08AC
	push	ax
	call	B2EF
	add	sp,0C
	cmp	word ptr [2E4F],00
	jz	0E2A

l0800_0E1B:
	push	word ptr [2E4F]
	push	ds
	mov	ax,08BA
	push	ax
	call	B2EF
	add	sp,06

l0800_0E2A:
	cmp	word ptr [2A1F],00
	jz	0E3C

l0800_0E31:
	push	ds
	mov	ax,08CF
	push	ax
	call	B2EF
	add	sp,04

l0800_0E3C:
	push	ds
	mov	ax,08DC
	push	ax
	call	B2EF
	add	sp,04
	push	ds
	mov	ax,427E
	push	ax
	call	3678
	add	sp,04
	push	ds
	mov	ax,08DF
	push	ax
	push	ds
	mov	ax,4477
	push	ax
	call	37BE
	add	sp,08
	push	ds
	mov	ax,08EC
	push	ax
	push	ds
	mov	ax,4477
	push	ax
	call	4234
	add	sp,08
	mov	[29DD],dx
	mov	[29DB],ax
	jmp	10F4

l0800_0E7C:
	call	3764
	call	388C
	or	ax,ax
	jnz	0E89

l0800_0E86:
	jmp	10F4

l0800_0E89:
	push	ds
	mov	ax,4541
	push	ax
	call	0C93
	add	sp,04
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	4194
	add	sp,04
	mov	[2A09],dx
	mov	[2A07],ax
	mov	[2A05],dx
	mov	[2A03],ax
	xor	si,si
	cmp	word ptr [2A09],00
	ja	0ECF

l0800_0EB9:
	jnz	0EC2

l0800_0EBB:
	cmp	word ptr [2A07],12
	ja	0ECF

l0800_0EC2:
	cmp	word ptr [2A15],00
	jnz	0ECF

l0800_0EC9:
	mov	si,0003
	jmp	105C

l0800_0ECF:
	call	3992
	or	ax,ax
	jz	0EDC

l0800_0ED6:
	mov	si,000E
	jmp	105C

l0800_0EDC:
	mov	bx,[2A23]
	cmp	bx,05
	jbe	0EE8

l0800_0EE5:
	jmp	1007

l0800_0EE8:
	shl	bx,01
	jmp	word ptr cs:[bx+1121]

l0800_0EEF:
	cmp	word ptr [2A09],00
	jc	0F1F

l0800_0EF6:
	jnz	0EFF

l0800_0EF8:
	cmp	word ptr [2A07],12
	jc	0F1F

l0800_0EFF:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F58
	add	sp,04
	mov	cl,08
	call	8CAA
	cmp	dx,52
	jnz	0F1F

l0800_0F17:
	cmp	ax,4E43
	jnz	0F1F

l0800_0F1C:
	mov	si,0004

l0800_0F1F:
	or	si,si
	jz	0F26

l0800_0F23:
	jmp	1007

l0800_0F26:
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	75EA
	add	sp,08
	mov	ax,[2A05]
	mov	dx,[2A03]
	cmp	ax,[2A09]
	jnc	0F4C

l0800_0F49:
	jmp	1007

l0800_0F4C:
	jnz	0F57

l0800_0F4E:
	cmp	dx,[2A07]
	jnc	0F57

l0800_0F54:
	jmp	1007

l0800_0F57:
	cmp	word ptr [2A15],00
	jnz	0F61

l0800_0F5E:
	jmp	1007

l0800_0F61:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	BA67
	add	sp,04
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	BA67
	add	sp,04
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,524E
	mov	dx,4300
	push	ax
	push	dx
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	409C
	add	sp,08
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	mov	si,0002
	jmp	1007

l0800_0FCC:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F0A
	add	sp,04
	cmp	ax,4D5A
	jnz	0FE6

l0800_0FDF:
	call	5E64
	mov	si,ax
	jmp	1007

l0800_0FE6:
	call	669C
	mov	si,ax
	jmp	1007

l0800_0FED:
	call	67BF
	mov	si,ax
	jmp	1007

l0800_0FF4:
	call	6AD4
	mov	si,ax
	jmp	1007

l0800_0FFB:
	call	73AC
	mov	si,ax
	jmp	1007

l0800_1002:
	call	741D
	mov	si,ax

l0800_1007:
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	4194
	add	sp,04
	mov	[2A05],dx
	mov	[2A03],ax
	mov	ax,[2A05]
	mov	dx,[2A03]
	cmp	ax,[2A09]
	jc	1049

l0800_1029:
	jnz	1031

l0800_102B:
	cmp	dx,[2A07]
	jc	1049

l0800_1031:
	cmp	word ptr [2A15],00
	jnz	1049

l0800_1038:
	mov	ax,[2A09]
	mov	dx,[2A07]
	mov	[2A05],ax
	mov	[2A03],dx
	mov	si,0003

l0800_1049:
	cmp	si,04
	jnz	105C

l0800_104E:
	mov	ax,[2A09]
	mov	dx,[2A07]
	mov	[2A05],ax
	mov	[2A03],dx

l0800_105C:
	cmp	byte ptr [427E],00
	jz	10B7

l0800_1063:
	cmp	si,03
	jz	106D

l0800_1068:
	cmp	si,04
	jnz	10B7

l0800_106D:
	mov	ax,[2A09]
	mov	dx,[2A07]
	mov	[2A05],ax
	mov	[2A03],dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	BA67
	add	sp,04
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	BA67
	add	sp,04
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	xor	si,si

l0800_10B7:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	A614
	add	sp,04
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	A614
	add	sp,04
	cmp	si,02
	jle	10E5

l0800_10D8:
	push	ds
	mov	ax,44DC
	push	ax
	call	8F7F
	add	sp,04
	jmp	10E8

l0800_10E5:
	call	37DF

l0800_10E8:
	push	ds
	mov	ax,4541
	push	ax
	push	si
	call	0ABC
	add	sp,06

l0800_10F4:
	push	ds
	mov	ax,4541
	push	ax
	call	2DE2
	add	sp,04
	or	ax,ax
	jz	1106

l0800_1103:
	jmp	0E7C

l0800_1106:
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	A614
	add	sp,04
	push	ds
	mov	ax,4477
	push	ax
	call	8F7F
	add	sp,04
	pop	si
	ret
l0800_1121	dw	0x0FF4
l0800_1123	dw	0x0EEF
l0800_1125	dw	0x1002
l0800_1127	dw	0x0FFB
l0800_1129	dw	0x0FCC
l0800_112B	dw	0x0FED

;; fn0800_112D: 0800:112D
;;   Called from:
;;     0800:0330 (in main)
fn0800_112D proc
	push	si
	mov	bx,[2A23]
	shl	bx,01
	shl	bx,01
	push	word ptr [bx+052F]
	push	word ptr [bx+052D]
	mov	bx,[2A25]
	shl	bx,01
	shl	bx,01
	push	word ptr [bx+050B]
	push	word ptr [bx+0509]
	push	ds
	mov	ax,08F0
	push	ax
	call	B2EF
	add	sp,0C
	cmp	word ptr [2E4F],00
	jz	116F

l0800_1160:
	push	word ptr [2E4F]
	push	ds
	mov	ax,08FE
	push	ax
	call	B2EF
	add	sp,06

l0800_116F:
	push	ds
	mov	ax,0913
	push	ax
	call	B2EF
	add	sp,04
	cmp	word ptr [2A25],01
	jz	1184

l0800_1181:
	jmp	12C2

l0800_1184:
	push	ds
	mov	ax,427E
	push	ax
	call	3678
	add	sp,04
	jmp	12C2

l0800_1192:
	call	3764
	call	388C
	or	ax,ax
	jnz	119F

l0800_119C:
	jmp	12C2

l0800_119F:
	push	ds
	mov	ax,4541
	push	ax
	call	0C93
	add	sp,04
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	4194
	add	sp,04
	mov	[2A05],dx
	mov	[2A03],ax
	or	dx,dx
	ja	11CF

l0800_11C3:
	jc	11CA

l0800_11C5:
	cmp	ax,0012
	jnc	11CF

l0800_11CA:
	mov	si,0007
	jmp	1243

l0800_11CF:
	mov	bx,[2A23]
	cmp	bx,05
	ja	1243

l0800_11D8:
	shl	bx,01
	jmp	word ptr cs:[bx+12D6]

l0800_11DF:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F58
	add	sp,04
	mov	cl,08
	call	8CAA
	cmp	dx,52
	jnz	1203

l0800_11F7:
	cmp	ax,4E43
	jnz	1203

l0800_11FC:
	call	5374
	mov	si,ax
	jmp	1243

l0800_1203:
	mov	si,0007
	jmp	1243

l0800_1208:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F0A
	add	sp,04
	cmp	ax,4D5A
	jnz	1222

l0800_121B:
	call	46FE
	mov	si,ax
	jmp	1243

l0800_1222:
	call	4B97
	mov	si,ax
	jmp	1243

l0800_1229:
	call	4BB1
	mov	si,ax
	jmp	1243

l0800_1230:
	call	4C55
	mov	si,ax
	jmp	1243

l0800_1237:
	call	518F
	mov	si,ax
	jmp	1243

l0800_123E:
	call	51A9
	mov	si,ax

l0800_1243:
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	4194
	add	sp,04
	mov	[2A09],dx
	mov	[2A07],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	4194
	add	sp,04
	mov	[2A05],dx
	mov	[2A03],ax
	or	si,si
	jz	127F

l0800_1271:
	mov	ax,[2A05]
	mov	dx,[2A03]
	mov	[2A09],ax
	mov	[2A07],dx

l0800_127F:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	A614
	add	sp,04
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	A614
	add	sp,04
	cmp	word ptr [2A25],02
	jz	12A6

l0800_12A2:
	or	si,si
	jz	12B3

l0800_12A6:
	push	ds
	mov	ax,44DC
	push	ax
	call	8F7F
	add	sp,04
	jmp	12B6

l0800_12B3:
	call	37DF

l0800_12B6:
	push	ds
	mov	ax,4541
	push	ax
	push	si
	call	0ABC
	add	sp,06

l0800_12C2:
	push	ds
	mov	ax,4541
	push	ax
	call	2DE2
	add	sp,04
	or	ax,ax
	jz	12D4

l0800_12D1:
	jmp	1192

l0800_12D4:
	pop	si
	ret
l0800_12D6	dw	0x1230
l0800_12D8	dw	0x11DF
l0800_12DA	dw	0x123E
l0800_12DC	dw	0x1237
l0800_12DE	dw	0x1208
l0800_12E0	dw	0x1229

;; fn0800_12E2: 0800:12E2
;;   Called from:
;;     0800:0326 (in main)
fn0800_12E2 proc
	push	bp
	mov	bp,sp
	sub	sp,0084
	push	si
	push	di
	push	ds
	mov	ax,4348
	push	ax
	mov	bx,[2A25]
	shl	bx,01
	shl	bx,01
	push	word ptr [bx+050B]
	push	word ptr [bx+0509]
	push	ds
	mov	ax,0916
	push	ax
	call	B2EF
	add	sp,0C
	cmp	word ptr [2E4F],00
	jz	1321

l0800_1312:
	push	word ptr [2E4F]
	push	ds
	mov	ax,0925
	push	ax
	call	B2EF
	add	sp,06

l0800_1321:
	push	ds
	mov	ax,093A
	push	ax
	call	B2EF
	add	sp,04
	push	ds
	mov	ax,093D
	push	ax
	push	ds
	mov	ax,4348
	push	ax
	call	AA7E
	add	sp,08
	mov	[29D1],dx
	mov	[29CF],ax
	mov	ax,[2A25]
	cmp	ax,0003
	jz	1353

l0800_134B:
	cmp	ax,0004
	jz	1371

l0800_1350:
	jmp	1478

l0800_1353:
	mov	ax,[29CF]
	or	ax,[29D1]
	jz	135F

l0800_135C:
	jmp	1478

l0800_135F:
	push	ds
	mov	ax,4348
	push	ax
	mov	ax,0007
	push	ax
	call	0D24
	add	sp,06
	jmp	1478

l0800_1371:
	mov	ax,[29CF]
	or	ax,[29D1]
	jz	13C6

l0800_137A:
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3E27
	add	sp,04
	cmp	ax,4D5A
	jnz	13BA

l0800_138D:
	mov	ax,0002
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	ACB3
	add	sp,0A
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	AD2F
	add	sp,04
	mov	[29ED],dx
	mov	[29EB],ax
	jmp	13C6

l0800_13BA:
	mov	word ptr [29D1],0000
	mov	word ptr [29CF],0000

l0800_13C6:
	mov	ax,[29CF]
	or	ax,[29D1]
	jnz	13F2

l0800_13CF:
	push	ds
	mov	ax,0941
	push	ax
	push	ds
	mov	ax,4348
	push	ax
	call	4234
	add	sp,08
	mov	[29D1],dx
	mov	[29CF],ax
	mov	word ptr [29ED],0000
	mov	word ptr [29EB],0000

l0800_13F2:
	mov	byte ptr [bp-6E],52
	mov	byte ptr [bp-6D],4E
	mov	byte ptr [bp-6C],43
	mov	byte ptr [bp-6B],41
	mov	byte ptr [bp-6A],00
	mov	byte ptr [bp-69],0C
	mov	byte ptr [bp-66],00
	mov	byte ptr [bp-65],0C
	mov	byte ptr [bp-64],00
	mov	byte ptr [bp-63],00
	xor	ax,ax
	push	ax
	mov	ax,0004
	push	ax
	push	ss
	lea	ax,[bp-66]
	push	ax
	call	2CCF
	add	sp,08
	mov	cl,08
	shr	ax,cl
	mov	[bp-68],al
	xor	ax,ax
	push	ax
	mov	ax,0004
	push	ax
	push	ss
	lea	ax,[bp-66]
	push	ax
	call	2CCF
	add	sp,08
	mov	[bp-67],al
	push	word ptr [29D1]
	push	word ptr [29CF]
	xor	ax,ax
	mov	dx,000C
	push	ax
	push	dx
	push	ss
	lea	ax,[bp-6E]
	push	ax
	call	4152
	add	sp,0C
	push	word ptr [29D1]
	push	word ptr [29CF]
	push	word ptr [29ED]
	push	word ptr [29EB]
	call	409C
	add	sp,08

l0800_1478:
	xor	ax,ax
	push	ax
	call	1CF6
	add	sp,02
	xor	si,si
	jmp	1527

l0800_1486:
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp-6E]
	push	ax
	call	3509
	add	sp,08
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp+FF7C]
	push	ax
	call	35A3
	add	sp,08
	push	ss
	lea	ax,[bp-6E]
	push	ax
	call	283D
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,dx
	jnz	14CD

l0800_14BC:
	push	ss
	lea	ax,[bp-6E]
	push	ax
	call	1F5C
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax

l0800_14CD:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	ss
	lea	ax,[bp+FF7C]
	push	ax
	call	2931
	add	sp,08
	mov	[bp-06],dx
	mov	[bp-08],ax
	or	ax,dx
	jnz	14FD

l0800_14E9:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	ss
	lea	ax,[bp+FF7C]
	push	ax
	call	2085
	add	sp,08
	jmp	1509

l0800_14FD:
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	23EC
	add	sp,04

l0800_1509:
	mov	si,0001
	les	bx,[2E51]
	mov	al,es:[bx+04]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+05]
	mov	dh,00
	add	ax,dx
	cmp	ax,FF8C
	jnc	1539

l0800_1527:
	push	ds
	mov	ax,4541
	push	ax
	call	2DE2
	add	sp,04
	or	ax,ax
	jz	1539

l0800_1536:
	jmp	1486

l0800_1539:
	or	si,si
	jnz	1551

l0800_153D:
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	A614
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_1551:
	call	1E5E
	mov	ax,0001
	push	ax
	call	1CF6
	add	sp,02
	mov	ax,0001
	push	ax
	xor	ax,ax
	push	ax
	call	2DBF
	add	sp,04
	mov	ax,[29D1]
	mov	dx,[29CF]
	mov	[29E1],ax
	mov	[29DF],dx
	push	ds
	mov	ax,0945
	push	ax
	push	ds
	mov	ax,4477
	push	ax
	call	37BE
	add	sp,08
	push	ds
	mov	ax,0941
	push	ax
	push	ds
	mov	ax,4477
	push	ax
	call	4234
	add	sp,08
	mov	[29DD],dx
	mov	[29DB],ax
	jmp	1897

l0800_15A3:
	push	ds
	mov	ax,093D
	push	ax
	push	ds
	mov	ax,4541
	push	ax
	call	4234
	add	sp,08
	mov	[29E5],dx
	mov	[29E3],ax
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp-6E]
	push	ax
	call	3509
	add	sp,08
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp+FF7C]
	push	ax
	call	35A3
	add	sp,08
	push	ds
	mov	ax,4541
	push	ax
	call	0C93
	add	sp,04
	push	ss
	lea	ax,[bp-6E]
	push	ax
	call	283D
	add	sp,04
	push	dx
	push	ax
	push	ss
	lea	ax,[bp+FF7C]
	push	ax
	call	2931
	add	sp,08
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	4194
	add	sp,04
	sub	ax,0004
	sbb	dx,00
	mov	[29E9],dx
	mov	[29E7],ax
	mov	ax,[29E9]
	cwd
	mov	cl,08
	call	8C8A
	les	di,[bp-08]
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_1632:
	repne scasb

l0800_1634:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_163A:
	repne scasb

l0800_163C:
	jz	1645

l0800_163E:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1645:
	dec	di
	mov	ax,es
	mov	es,ax
	pop	ax
	mov	es:[di+01],al
	mov	ax,[29E9]
	cwd
	les	di,[bp-08]
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_165C:
	repne scasb

l0800_165E:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1664:
	repne scasb

l0800_1666:
	jz	166F

l0800_1668:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_166F:
	dec	di
	mov	ax,es
	mov	es,ax
	pop	ax
	mov	es:[di+02],al
	mov	dx,[29E9]
	mov	ax,[29E7]
	mov	cl,08
	call	8C8A
	les	di,[bp-08]
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_168E:
	repne scasb

l0800_1690:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1696:
	repne scasb

l0800_1698:
	jz	16A1

l0800_169A:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_16A1:
	dec	di
	mov	ax,es
	mov	es,ax
	pop	ax
	mov	es:[di+03],al
	les	di,[bp-08]
	xor	ax,ax
	mov	cx,FFFF

l0800_16B3:
	repne scasb

l0800_16B5:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_16BB:
	repne scasb

l0800_16BD:
	jz	16C6

l0800_16BF:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_16C6:
	dec	di
	mov	ax,es
	mov	dl,[29E7]
	mov	es,ax
	mov	es:[di+04],dl
	mov	di,0001
	xor	ax,ax
	mov	word ptr [2A05],0000
	mov	[2A03],ax
	mov	si,ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	4194
	add	sp,04
	mov	[2A09],dx
	mov	[2A07],ax
	xor	ax,ax
	push	ax
	push	word ptr [29E9]
	push	word ptr [29E7]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	cmp	word ptr [2A09],00
	jnc	171B

l0800_1718:
	jmp	17A2

l0800_171B:
	ja	1724

l0800_171D:
	cmp	word ptr [2A07],12
	jbe	17A2

l0800_1724:
	cmp	word ptr [2A21],00
	jz	17A2

l0800_172B:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F58
	add	sp,04
	mov	cl,08
	call	8CAA
	cmp	dx,52
	jnz	1748

l0800_1743:
	cmp	ax,4E43
	jz	17A2

l0800_1748:
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	75EA
	add	sp,08
	mov	ax,[2A05]
	mov	dx,[2A03]
	cmp	ax,[2A09]
	jc	1778

l0800_176B:
	jnz	1773

l0800_176D:
	cmp	dx,[2A07]
	jc	1778

l0800_1773:
	mov	ax,0001
	jmp	177A

l0800_1778:
	xor	ax,ax

l0800_177A:
	mov	di,ax
	or	ax,ax
	jz	17A2

l0800_1780:
	mov	ax,0001
	push	ax
	mov	ax,[2A05]
	mov	dx,[2A03]
	neg	ax
	neg	dx
	sbb	ax,0000
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A

l0800_17A2:
	or	di,di
	jnz	17A9

l0800_17A6:
	jmp	183F

l0800_17A9:
	mov	ax,[2A09]
	mov	dx,[2A07]
	mov	[2A05],ax
	mov	[2A03],dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	BA67
	add	sp,04
	cmp	word ptr [2A09],00
	jc	17F2

l0800_17CC:
	jnz	17D5

l0800_17CE:
	cmp	word ptr [2A07],12
	jc	17F2

l0800_17D5:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F58
	add	sp,04
	mov	cl,08
	call	8CAA
	cmp	dx,52
	jnz	17F2

l0800_17ED:
	cmp	ax,4E43
	jz	181E

l0800_17F2:
	push	word ptr [29D1]
	push	word ptr [29CF]
	mov	ax,524E
	mov	dx,4300
	push	ax
	push	dx
	call	409C
	add	sp,08
	push	word ptr [29D1]
	push	word ptr [29CF]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	409C
	add	sp,08

l0800_181E:
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [29D1]
	push	word ptr [29CF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	mov	si,0002

l0800_183F:
	push	word ptr [29D1]
	push	word ptr [29CF]
	push	word ptr [29ED]
	push	word ptr [29EB]
	call	409C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	A614
	add	sp,04
	push	ds
	mov	ax,4541
	push	ax
	push	si
	call	0ABC
	add	sp,06
	les	bx,[2E51]
	mov	al,es:[bx+04]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+05]
	mov	dh,00
	add	ax,dx
	cmp	ax,FF8C
	jc	1897

l0800_188A:
	push	ds
	mov	ax,0952
	push	ax
	call	B2EF
	add	sp,04
	jmp	18A9

l0800_1897:
	push	ds
	mov	ax,4541
	push	ax
	call	2DE2
	add	sp,04
	or	ax,ax
	jz	18A9

l0800_18A6:
	jmp	15A3

l0800_18A9:
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	A614
	add	sp,04
	push	ds
	mov	ax,4477
	push	ax
	call	8F7F
	add	sp,04
	call	1E5E
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	A614
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_18D9: 0800:18D9
;;   Called from:
;;     0800:032B (in main)
fn0800_18D9 proc
	push	bp
	mov	bp,sp
	sub	sp,6E
	push	di
	push	ds
	mov	ax,4348
	push	ax
	mov	bx,[2A25]
	shl	bx,01
	shl	bx,01
	push	word ptr [bx+050B]
	push	word ptr [bx+0509]
	push	ds
	mov	ax,096C
	push	ax
	call	B2EF
	add	sp,0C
	push	ds
	mov	ax,097D
	push	ax
	push	ds
	mov	ax,4348
	push	ax
	call	AA7E
	add	sp,08
	mov	[29D1],dx
	mov	[29CF],ax
	or	ax,dx
	jnz	192A

l0800_191B:
	push	ds
	mov	ax,4348
	push	ax
	mov	ax,0007
	push	ax
	call	0D24
	add	sp,06

l0800_192A:
	mov	ax,0001
	push	ax
	call	1CF6
	add	sp,02
	xor	ax,ax
	push	ax
	push	ax
	call	2DBF
	add	sp,04
	jmp	19BC

l0800_1940:
	push	ss
	lea	ax,[bp-6E]
	push	ax
	call	0C93
	add	sp,04
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	23EC
	add	sp,04
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	22FE
	add	sp,04
	les	di,[bp-04]
	add	di,02
	xor	ax,ax
	mov	cx,FFFF

l0800_196E:
	repne scasb

l0800_1970:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1976:
	repne scasb

l0800_1978:
	jz	1981

l0800_197A:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1981:
	dec	di
	mov	ax,es
	mov	es,ax
	cmp	byte ptr es:[di+01],00
	jnz	19AD

l0800_198D:
	mov	ax,[2E53]
	mov	dx,[2E51]
	add	dx,08
	cmp	ax,[bp-02]
	jnz	19A1

l0800_199C:
	cmp	dx,[bp-04]
	jz	19AD

l0800_19A1:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	2201
	add	sp,04

l0800_19AD:
	push	ss
	lea	ax,[bp-6E]
	push	ax
	mov	ax,000D
	push	ax
	call	0ABC
	add	sp,06

l0800_19BC:
	push	ss
	lea	ax,[bp-6E]
	push	ax
	push	ss
	lea	ax,[bp-08]
	push	ax
	push	ss
	lea	ax,[bp-04]
	push	ax
	call	29C5
	add	sp,0C
	or	ax,ax
	jz	19D8

l0800_19D5:
	jmp	1940

l0800_19D8:
	call	1E5E
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	A614
	add	sp,04
	pop	di
	mov	sp,bp
	pop	bp
	ret

;; fn0800_19EE: 0800:19EE
;;   Called from:
;;     0800:0335 (in main)
fn0800_19EE proc
	push	bp
	mov	bp,sp
	sub	sp,72
	push	si
	push	di
	push	ds
	mov	ax,4348
	push	ax
	mov	bx,[2A25]
	shl	bx,01
	shl	bx,01
	push	word ptr [bx+050B]
	push	word ptr [bx+0509]
	push	ds
	mov	ax,0986
	push	ax
	call	B2EF
	add	sp,0C
	push	ds
	mov	ax,0997
	push	ax
	push	ds
	mov	ax,4348
	push	ax
	call	AA7E
	add	sp,08
	mov	[29D1],dx
	mov	[29CF],ax
	or	ax,dx
	jnz	1A40

l0800_1A31:
	push	ds
	mov	ax,4348
	push	ax
	mov	ax,0007
	push	ax
	call	0D24
	add	sp,06

l0800_1A40:
	mov	ax,[2A27]
	cmp	ax,[269A]
	jnz	1A7D

l0800_1A49:
	mov	ax,[0984]
	mov	dx,[0982]
	mov	[bp-0A],ax
	mov	[bp-0C],dx
	mov	ax,[bp-0C]
	mov	dx,0004
	les	bx,[269C]
	lea	ax,[bp-0C]
	mov	es:[bx+06],ss
	mov	es:[bx+04],ax
	mov	word ptr [2A27],0001
	mov	word ptr [269A],0002
	mov	word ptr [2A1B],0001

l0800_1A7D:
	xor	ax,ax
	push	ax
	mov	ax,0001
	push	ax
	call	2DBF
	add	sp,04
	mov	ax,0001
	push	ax
	call	1CF6
	add	sp,02
	cmp	word ptr [2A25],07
	jz	1A9E

l0800_1A9B:
	jmp	1CB4

l0800_1A9E:
	push	ds
	mov	ax,099B
	push	ax
	push	ds
	mov	ax,44DC
	push	ax
	call	37BE
	add	sp,08
	jmp	1CB4

l0800_1AB1:
	push	ss
	lea	ax,[bp-72]
	push	ax
	call	0C93
	add	sp,04
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	24FE
	add	sp,04
	xor	ax,ax
	push	ax
	les	di,[bp-08]
	mov	cx,FFFF

l0800_1AD1:
	repne scasb

l0800_1AD3:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1AD9:
	repne scasb

l0800_1ADB:
	jz	1AE4

l0800_1ADD:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1AE4:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+01]
	cbw
	mov	cl,08
	shl	ax,cl
	les	di,[bp-08]
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_1AFB:
	repne scasb

l0800_1AFD:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1B03:
	repne scasb

l0800_1B05:
	jz	1B0E

l0800_1B07:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1B0E:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+02]
	mov	ah,00
	pop	dx
	add	dx,ax
	xor	ax,ax
	les	di,[bp-08]
	push	dx
	push	ax
	mov	cx,FFFF

l0800_1B26:
	repne scasb

l0800_1B28:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1B2E:
	repne scasb

l0800_1B30:
	jz	1B39

l0800_1B32:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1B39:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+03]
	cbw
	mov	cl,08
	shl	ax,cl
	les	di,[bp-08]
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_1B50:
	repne scasb

l0800_1B52:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1B58:
	repne scasb

l0800_1B5A:
	jz	1B63

l0800_1B5C:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1B63:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+04]
	mov	ah,00
	pop	dx
	add	dx,ax
	pop	ax
	add	ax,dx
	pop	dx
	adc	dx,00
	push	dx
	push	ax
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	ACB3
	add	sp,0A
	cmp	word ptr [2A25],07
	jnz	1B92

l0800_1B8F:
	jmp	1C61

l0800_1B92:
	push	ds
	pop	es
	mov	di,44DC
	push	es
	push	di
	mov	di,427E
	xor	ax,ax
	mov	cx,FFFF

l0800_1BA1:
	repne scasb

l0800_1BA3:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_1BB2:
	rep movsw

l0800_1BB4:
	adc	cx,cx

l0800_1BB6:
	rep movsb

l0800_1BB8:
	pop	ds
	cmp	word ptr [2A25],08
	jnz	1C29

l0800_1BC0:
	cmp	byte ptr [427E],00
	jz	1BFC

l0800_1BC7:
	cmp	byte ptr [bp-72],5C
	jnz	1BFC

l0800_1BCD:
	push	ss
	lea	ax,[bp-71]
	push	ax
	push	ds
	pop	es
	mov	di,44DC
	xor	ax,ax
	mov	cx,FFFF

l0800_1BDC:
	repne scasb

l0800_1BDE:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1BE4:
	repne scasb

l0800_1BE6:
	jz	1BEF

l0800_1BE8:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1BEF:
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	3509
	add	sp,08
	jmp	1C29

l0800_1BFC:
	push	ss
	lea	ax,[bp-72]
	push	ax
	push	ds
	pop	es
	mov	di,44DC
	xor	ax,ax
	mov	cx,FFFF

l0800_1C0B:
	repne scasb

l0800_1C0D:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1C13:
	repne scasb

l0800_1C15:
	jz	1C1E

l0800_1C17:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1C1E:
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	3509
	add	sp,08

l0800_1C29:
	push	ds
	mov	ax,44DC
	push	ax
	call	3678
	add	sp,04
	push	ss
	lea	ax,[bp-72]
	push	ax
	push	ds
	pop	es
	mov	di,44DC
	xor	ax,ax
	mov	cx,FFFF

l0800_1C43:
	repne scasb

l0800_1C45:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1C4B:
	repne scasb

l0800_1C4D:
	jz	1C56

l0800_1C4F:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1C56:
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	35A3
	add	sp,08

l0800_1C61:
	push	ds
	mov	ax,09A8
	push	ax
	push	ds
	mov	ax,44DC
	push	ax
	call	4234
	add	sp,08
	mov	[29E1],dx
	mov	[29DF],ax
	mov	ax,[29D1]
	mov	dx,[29CF]
	mov	[29E5],ax
	mov	[29E3],dx
	call	5374
	mov	si,ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	A614
	add	sp,04
	or	si,si
	jz	1CA8

l0800_1C9D:
	push	ds
	mov	ax,44DC
	push	ax
	call	8F7F
	add	sp,04

l0800_1CA8:
	push	ss
	lea	ax,[bp-72]
	push	ax
	push	si
	call	0ABC
	add	sp,06

l0800_1CB4:
	push	ss
	lea	ax,[bp-72]
	push	ax
	push	ss
	lea	ax,[bp-08]
	push	ax
	push	ss
	lea	ax,[bp-04]
	push	ax
	call	29C5
	add	sp,0C
	or	ax,ax
	jz	1CD0

l0800_1CCD:
	jmp	1AB1

l0800_1CD0:
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	A614
	add	sp,04
	cmp	word ptr [2A25],07
	jnz	1CF0

l0800_1CE5:
	push	ds
	mov	ax,44DC
	push	ax
	call	8F7F
	add	sp,04

l0800_1CF0:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_1CF6: 0800:1CF6
;;   Called from:
;;     0800:147B (in fn0800_12E2)
;;     0800:1558 (in fn0800_12E2)
;;     0800:192E (in fn0800_18D9)
;;     0800:1A8E (in fn0800_19EE)
fn0800_1CF6 proc
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	xor	ax,ax
	xor	dx,dx
	mov	[45A8],ax
	mov	[45A6],dx
	mov	[29ED],ax
	mov	[29EB],dx
	mov	ax,0002
	push	ax
	mov	ax,FFFF
	mov	dx,FFFC
	push	ax
	push	dx
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	ACB3
	add	sp,0A
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3E5D
	add	sp,04
	mov	[29ED],dx
	mov	[29EB],ax
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	4194
	add	sp,04
	cmp	dx,[29ED]
	ja	1D69

l0800_1D52:
	jc	1D5A

l0800_1D54:
	cmp	ax,[29EB]
	jnc	1D69

l0800_1D5A:
	push	ds
	mov	ax,4348
	push	ax
	mov	ax,000B
	push	ax
	call	0D24
	add	sp,06

l0800_1D69:
	xor	ax,ax
	push	ax
	push	word ptr [29ED]
	push	word ptr [29EB]
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	ACB3
	add	sp,0A
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3E5D
	add	sp,04
	cmp	dx,524E
	jnz	1D9B

l0800_1D96:
	cmp	ax,4341
	jz	1DAA

l0800_1D9B:
	push	ds
	mov	ax,4348
	push	ax
	mov	ax,000B
	push	ax
	call	0D24
	add	sp,06

l0800_1DAA:
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3E27
	add	sp,04
	mov	si,ax
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3E27
	add	sp,04
	mov	[bp-02],ax
	cmp	word ptr [bp+04],00
	jz	1DE4

l0800_1DD1:
	xor	ax,ax
	push	ax
	push	si
	call	4311
	add	sp,04
	mov	[2E53],dx
	mov	[2E51],ax
	jmp	1DF8

l0800_1DE4:
	xor	ax,ax
	mov	dx,FFF0
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[2E53],dx
	mov	[2E51],ax

l0800_1DF8:
	xor	ax,ax
	push	ax
	push	word ptr [29ED]
	push	word ptr [29EB]
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	ACB3
	add	sp,0A
	push	word ptr [29D1]
	push	word ptr [29CF]
	xor	ax,ax
	push	ax
	push	si
	push	word ptr [2E53]
	push	word ptr [2E51]
	call	4110
	add	sp,0C
	xor	ax,ax
	push	ax
	mov	ax,si
	sub	ax,0008
	push	ax
	mov	ax,[2E51]
	add	ax,0008
	push	word ptr [2E53]
	push	ax
	call	2CCF
	add	sp,08
	cmp	ax,[bp-02]
	jz	1E59

l0800_1E4A:
	push	ds
	mov	ax,4348
	push	ax
	mov	ax,000D
	push	ax
	call	0D24
	add	sp,06

l0800_1E59:
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_1E5E: 0800:1E5E
;;   Called from:
;;     0800:1551 (in fn0800_12E2)
;;     0800:18C2 (in fn0800_12E2)
;;     0800:19D8 (in fn0800_18D9)
fn0800_1E5E proc
	push	si
	les	bx,[2E51]
	mov	al,es:[bx+04]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+05]
	mov	dh,00
	add	ax,dx
	mov	si,ax
	xor	ax,ax
	push	ax
	mov	ax,si
	sub	ax,0008
	push	ax
	mov	ax,[2E51]
	add	ax,0008
	push	word ptr [2E53]
	push	ax
	call	2CCF
	add	sp,08
	mov	dx,ax
	mov	cl,08
	shr	ax,cl
	les	bx,[2E51]
	mov	es:[bx+06],al
	mov	es:[bx+07],dl
	xor	ax,ax
	push	ax
	mov	ax,[29ED]
	mov	dx,[29EB]
	add	dx,04
	adc	ax,0000
	push	ax
	push	dx
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	ACB3
	add	sp,0A
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3E27
	add	sp,04
	mov	dx,ax
	cmp	si,dx
	jbe	1EF7

l0800_1ED7:
	mov	ax,si
	sub	ax,dx
	xor	dx,dx
	push	dx
	push	ax
	push	word ptr [29ED]
	push	word ptr [29EB]
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3BC3
	add	sp,0C
	jmp	1F19

l0800_1EF7:
	cmp	si,dx
	jnc	1F19

l0800_1EFB:
	mov	ax,dx
	sub	ax,si
	xor	dx,dx
	push	dx
	push	ax
	push	word ptr [29ED]
	push	word ptr [29EB]
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3C99
	add	sp,0C

l0800_1F19:
	xor	ax,ax
	push	ax
	push	word ptr [29ED]
	push	word ptr [29EB]
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	ACB3
	add	sp,0A
	push	word ptr [29D1]
	push	word ptr [29CF]
	xor	ax,ax
	push	ax
	push	si
	push	word ptr [2E53]
	push	word ptr [2E51]
	call	4152
	add	sp,0C
	push	word ptr [2E53]
	push	word ptr [2E51]
	call	4346
	add	sp,04
	pop	si
	ret

;; fn0800_1F5C: 0800:1F5C
;;   Called from:
;;     0800:14C1 (in fn0800_12E2)
fn0800_1F5C proc
	push	bp
	mov	bp,sp
	sub	sp,08
	push	si
	push	di
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_1F6C:
	repne scasb

l0800_1F6E:
	not	cx
	mov	ax,003A
	sub	di,cx

l0800_1F75:
	repne scasb

l0800_1F77:
	jz	1F80

l0800_1F79:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1F80:
	dec	di
	mov	ax,es
	or	di,ax
	jz	1FAD

l0800_1F87:
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_1F8F:
	repne scasb

l0800_1F91:
	not	cx
	mov	ax,003A
	sub	di,cx

l0800_1F98:
	repne scasb

l0800_1F9A:
	jz	1FA3

l0800_1F9C:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_1FA3:
	dec	di
	mov	ax,es
	inc	di
	mov	[bp+06],ax
	mov	[bp+04],di

l0800_1FAD:
	les	bx,[2E51]
	mov	al,es:[bx+04]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+05]
	mov	dh,00
	add	ax,dx
	mov	[bp-06],ax
	mov	ax,[2E53]
	mov	dx,[2E51]
	add	dx,[bp-06]
	mov	[bp-02],ax
	mov	[bp-04],dx
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_1FDE:
	repne scasb

l0800_1FE0:
	not	cx
	dec	cx
	add	cx,04
	mov	[bp-08],cx
	mov	ax,[bp-04]
	xor	dx,dx
	sub	ax,[2E51]
	sbb	dx,00
	add	ax,[bp-08]
	adc	dx,00
	mov	cl,08
	call	8C8A
	les	bx,[bp-04]
	mov	es:[bx],al
	mov	ax,[bp-04]
	xor	dx,dx
	sub	ax,[2E51]
	sbb	dx,00
	add	al,[bp-08]
	mov	es:[bx+01],al
	mov	di,[bp-04]
	add	di,02
	push	es
	mov	es,[bp+06]
	push	di
	mov	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_202C:
	repne scasb

l0800_202E:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_203D:
	rep movsw

l0800_203F:
	adc	cx,cx

l0800_2041:
	rep movsb

l0800_2043:
	pop	ds
	mov	es,[bp-02]
	add	bx,[bp-08]
	mov	byte ptr es:[bx-01],00
	mov	ax,[bp-06]
	add	ax,[bp-08]
	mov	cl,08
	shr	ax,cl
	les	bx,[2E51]
	mov	es:[bx+04],al
	mov	al,[bp-06]
	add	al,[bp-08]
	mov	es:[bx+05],al
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [bp-08]
	call	2688
	add	sp,08
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_2085: 0800:2085
;;   Called from:
;;     0800:14F5 (in fn0800_12E2)
fn0800_2085 proc
	push	bp
	mov	bp,sp
	sub	sp,0C
	push	si
	push	di
	les	bx,[2E51]
	mov	al,es:[bx+04]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+05]
	mov	dh,00
	add	ax,dx
	mov	[bp-0A],ax
	mov	ax,[2E53]
	mov	dx,[2E51]
	add	dx,[bp-0A]
	mov	[bp-06],ax
	mov	[bp-08],dx
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_20BE:
	repne scasb

l0800_20C0:
	not	cx
	dec	cx
	add	cx,05
	mov	[bp-0C],cx
	les	bx,[bp+08]
	mov	al,es:[bx]
	cbw
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+01]
	mov	dh,00
	add	ax,dx
	mov	dx,[2E53]
	mov	bx,[2E51]
	add	bx,ax
	dec	bx
	mov	[bp-02],dx
	mov	[bp-04],bx
	mov	ax,[bp-08]
	xor	dx,dx
	sub	ax,[bp-04]
	sbb	dx,00
	push	ax
	push	word ptr [2E53]
	push	bx
	mov	ax,[bp-04]
	add	ax,[bp-0C]
	push	word ptr [2E53]
	push	ax
	call	B0F3
	add	sp,0A
	les	di,[bp-04]
	push	es
	mov	es,[bp+06]
	push	di
	mov	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_211F:
	repne scasb

l0800_2121:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_2130:
	rep movsw

l0800_2132:
	adc	cx,cx

l0800_2134:
	rep movsb

l0800_2136:
	pop	ds
	les	di,[bp-04]
	xor	ax,ax
	mov	cx,FFFF

l0800_213F:
	repne scasb

l0800_2141:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2147:
	repne scasb

l0800_2149:
	jz	2152

l0800_214B:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2152:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di+01],00
	les	di,[bp-04]
	xor	ax,ax
	mov	cx,FFFF

l0800_2164:
	repne scasb

l0800_2166:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_216C:
	repne scasb

l0800_216E:
	jz	2177

l0800_2170:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2177:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di+02],00
	les	di,[bp-04]
	xor	ax,ax
	mov	cx,FFFF

l0800_2189:
	repne scasb

l0800_218B:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2191:
	repne scasb

l0800_2193:
	jz	219C

l0800_2195:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_219C:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di+03],00
	les	di,[bp-04]
	xor	ax,ax
	mov	cx,FFFF

l0800_21AE:
	repne scasb

l0800_21B0:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_21B6:
	repne scasb

l0800_21B8:
	jz	21C1

l0800_21BA:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_21C1:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di+04],00
	mov	ax,[bp-0A]
	add	ax,[bp-0C]
	mov	cl,08
	shr	ax,cl
	les	bx,[2E51]
	mov	es:[bx+04],al
	mov	al,[bp-0A]
	add	al,[bp-0C]
	mov	es:[bx+05],al
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [bp-0C]
	call	2688
	add	sp,08
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_2201: 0800:2201
;;   Called from:
;;     0800:19A7 (in fn0800_18D9)
fn0800_2201 proc
	push	bp
	mov	bp,sp
	sub	sp,0A
	push	di
	les	bx,[2E51]
	mov	al,es:[bx+04]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+05]
	mov	dh,00
	add	ax,dx
	mov	[bp-06],ax
	mov	ax,[2E53]
	mov	dx,[2E51]
	add	dx,[bp-06]
	mov	[bp-02],ax
	mov	[bp-04],dx
	les	bx,[bp+04]
	mov	al,es:[bx]
	cbw
	shl	ax,cl
	mov	dl,es:[bx+01]
	mov	dh,00
	add	ax,dx
	mov	dx,[2E51]
	add	dx,ax
	xor	ax,ax
	sub	dx,[bp+04]
	sbb	ax,0000
	mov	[bp-08],dx
	mov	ax,[bp+04]
	add	ax,[bp-08]
	mov	[bp-0A],ax
	mov	dx,[bp-04]
	xor	bx,bx
	sub	dx,ax
	sbb	bx,00
	push	dx
	push	word ptr [bp+06]
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	B0F3
	add	sp,0A
	mov	ax,[bp-06]
	sub	ax,[bp-08]
	mov	cl,08
	shr	ax,cl
	les	bx,[2E51]
	mov	es:[bx+04],al
	mov	al,[bp-06]
	sub	al,[bp-08]
	mov	es:[bx+05],al
	xor	ax,ax
	push	ax
	push	ax
	mov	ax,[bp-08]
	xor	dx,dx
	neg	dx
	neg	ax
	sbb	dx,00
	push	dx
	push	ax
	call	2688
	add	sp,08
	mov	ax,[45B0]
	mov	dx,[45AE]
	cmp	ax,[bp+06]
	jnz	22EA

l0800_22B7:
	cmp	dx,[bp+04]
	jnz	22EA

l0800_22BC:
	les	di,[bp+04]
	add	di,02
	xor	ax,ax
	mov	cx,FFFF

l0800_22C7:
	repne scasb

l0800_22C9:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_22CF:
	repne scasb

l0800_22D1:
	jz	22DA

l0800_22D3:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_22DA:
	dec	di
	mov	ax,es
	inc	di
	mov	[45AC],ax
	mov	[45AA],di
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_22EA:
	mov	ax,[45AE]
	cmp	ax,[bp+04]
	jbe	22F9

l0800_22F2:
	mov	ax,[bp-08]
	sub	[45AA],ax

l0800_22F9:
	pop	di
	mov	sp,bp
	pop	bp
	ret

;; fn0800_22FE: 0800:22FE
;;   Called from:
;;     0800:195D (in fn0800_18D9)
fn0800_22FE proc
	push	bp
	mov	bp,sp
	sub	sp,0A
	push	di
	les	bx,[2E51]
	mov	al,es:[bx+04]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+05]
	mov	dh,00
	add	ax,dx
	mov	[bp-06],ax
	mov	ax,[2E53]
	mov	dx,[2E51]
	add	dx,[bp-06]
	mov	[bp-02],ax
	mov	[bp-04],dx
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_2336:
	repne scasb

l0800_2338:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_233E:
	repne scasb

l0800_2340:
	jz	2349

l0800_2342:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2349:
	dec	di
	mov	ax,es
	add	di,05
	xor	ax,ax
	sub	di,[bp+04]
	sbb	ax,0000
	mov	[bp-08],di
	mov	ax,[bp+04]
	add	ax,[bp-08]
	mov	[bp-0A],ax
	mov	dx,[bp-04]
	xor	bx,bx
	sub	dx,ax
	sbb	bx,00
	push	dx
	push	word ptr [bp+06]
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	B0F3
	add	sp,0A
	mov	ax,[bp-06]
	sub	ax,[bp-08]
	mov	cl,08
	shr	ax,cl
	les	bx,[2E51]
	mov	es:[bx+04],al
	mov	al,[bp-06]
	sub	al,[bp-08]
	mov	es:[bx+05],al
	xor	ax,ax
	push	ax
	push	ax
	mov	ax,[bp-08]
	xor	dx,dx
	neg	dx
	neg	ax
	sbb	dx,00
	push	dx
	push	ax
	call	2688
	add	sp,08
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	add	dx,[bp-08]
	mov	[45AC],ax
	mov	[45AA],dx
	or	dx,ax
	jz	23D8

l0800_23C6:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	mov	[45AC],ax
	mov	[45AA],dx
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_23D8:
	mov	ax,[45AA]
	cmp	ax,[bp+04]
	jbe	23E7

l0800_23E0:
	mov	ax,[bp-08]
	sub	[45AA],ax

l0800_23E7:
	pop	di
	mov	sp,bp
	pop	bp
	ret

;; fn0800_23EC: 0800:23EC
;;   Called from:
;;     0800:1503 (in fn0800_12E2)
;;     0800:1951 (in fn0800_18D9)
fn0800_23EC proc
	push	bp
	mov	bp,sp
	sub	sp,08
	push	di
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	24FE
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_240D:
	repne scasb

l0800_240F:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2415:
	repne scasb

l0800_2417:
	jz	2420

l0800_2419:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2420:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+01]
	cbw
	mov	cl,08
	shl	ax,cl
	les	di,[bp+04]
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_2437:
	repne scasb

l0800_2439:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_243F:
	repne scasb

l0800_2441:
	jz	244A

l0800_2443:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_244A:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+02]
	mov	ah,00
	pop	dx
	add	dx,ax
	xor	ax,ax
	les	di,[bp+04]
	push	dx
	push	ax
	mov	cx,FFFF

l0800_2462:
	repne scasb

l0800_2464:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_246A:
	repne scasb

l0800_246C:
	jz	2475

l0800_246E:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2475:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+03]
	cbw
	mov	cl,08
	shl	ax,cl
	les	di,[bp+04]
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_248C:
	repne scasb

l0800_248E:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2494:
	repne scasb

l0800_2496:
	jz	249F

l0800_2498:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_249F:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+04]
	mov	ah,00
	pop	dx
	add	dx,ax
	pop	ax
	add	ax,dx
	pop	dx
	adc	dx,00
	mov	[bp-02],dx
	mov	[bp-04],ax
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	sub	dx,[45A6]
	sbb	ax,[45A8]
	push	ax
	push	dx
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3C99
	add	sp,0C
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	neg	ax
	neg	dx
	sbb	ax,0000
	push	ax
	push	dx
	call	2688
	add	sp,08
	pop	di
	mov	sp,bp
	pop	bp
	ret

;; fn0800_24FE: 0800:24FE
;;   Called from:
;;     0800:1AC2 (in fn0800_19EE)
;;     0800:23F9 (in fn0800_23EC)
fn0800_24FE proc
	push	bp
	mov	bp,sp
	sub	sp,0C
	push	di
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_250D:
	repne scasb

l0800_250F:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2515:
	repne scasb

l0800_2517:
	jz	2520

l0800_2519:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2520:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+01]
	cbw
	mov	cl,08
	shl	ax,cl
	les	di,[bp+04]
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_2537:
	repne scasb

l0800_2539:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_253F:
	repne scasb

l0800_2541:
	jz	254A

l0800_2543:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_254A:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+02]
	mov	ah,00
	pop	dx
	add	dx,ax
	xor	ax,ax
	les	di,[bp+04]
	push	dx
	push	ax
	mov	cx,FFFF

l0800_2562:
	repne scasb

l0800_2564:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_256A:
	repne scasb

l0800_256C:
	jz	2575

l0800_256E:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2575:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+03]
	cbw
	mov	cl,08
	shl	ax,cl
	les	di,[bp+04]
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_258C:
	repne scasb

l0800_258E:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2594:
	repne scasb

l0800_2596:
	jz	259F

l0800_2598:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_259F:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+04]
	mov	ah,00
	pop	dx
	add	dx,ax
	pop	ax
	add	ax,dx
	pop	dx
	adc	dx,00
	sub	ax,[45A6]
	sbb	dx,[45A8]
	mov	[bp-02],dx
	mov	[bp-04],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	ACB3
	add	sp,0A
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3E5D
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	mov	cl,08
	call	8C8A
	cmp	dx,52
	jnz	25FC

l0800_25F7:
	cmp	ax,4E43
	jz	260B

l0800_25FC:
	push	ds
	mov	ax,4348
	push	ax
	mov	ax,000C
	push	ax
	call	0D24
	add	sp,06

l0800_260B:
	mov	ax,[bp-08]
	and	ax,00FF
	or	ax,0000
	jz	2655

l0800_2616:
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3E5D
	add	sp,04
	mov	[2A09],dx
	mov	[2A07],ax
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3E5D
	add	sp,04
	mov	[2A05],dx
	mov	[2A03],ax
	mov	ax,[2A05]
	mov	dx,[2A03]
	add	dx,12
	adc	ax,0000
	mov	[bp-0A],ax
	mov	[bp-0C],dx
	jmp	267D

l0800_2655:
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	3E5D
	add	sp,04
	mov	[2A05],dx
	mov	[2A03],ax
	mov	[2A09],dx
	mov	[2A07],ax
	add	ax,0008
	adc	dx,00
	mov	[bp-0A],dx
	mov	[bp-0C],ax

l0800_267D:
	mov	dx,[bp-0A]
	mov	ax,[bp-0C]
	pop	di
	mov	sp,bp
	pop	bp
	ret

;; fn0800_2688: 0800:2688
;;   Called from:
;;     0800:2073 (in fn0800_1F5C)
;;     0800:21EF (in fn0800_2085)
;;     0800:22A5 (in fn0800_2201)
;;     0800:23AC (in fn0800_22FE)
;;     0800:24F3 (in fn0800_23EC)
fn0800_2688 proc
	push	bp
	mov	bp,sp
	sub	sp,10
	les	bx,[2E51]
	mov	al,es:[bx+04]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+05]
	mov	dh,00
	add	ax,dx
	mov	dx,[2E53]
	add	bx,ax
	mov	[bp-06],dx
	mov	[bp-08],bx
	mov	ax,[2E53]
	mov	dx,[2E51]
	add	dx,08
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	27B3

l0800_26C3:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	mov	[bp-0A],ax
	mov	[bp-0C],dx
	add	word ptr [bp-04],02

l0800_26D3:
	les	bx,[bp-04]
	inc	word ptr [bp-04]
	cmp	byte ptr es:[bx],00
	jnz	26D3

l0800_26DF:
	jmp	277D

l0800_26E2:
	les	bx,[bp-04]
	inc	word ptr [bp-04]
	cmp	byte ptr es:[bx],00
	jnz	26E2

l0800_26EE:
	les	bx,[bp-04]
	mov	al,es:[bx]
	cbw
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+01]
	mov	dh,00
	add	ax,dx
	xor	dx,dx
	push	ax
	mov	al,es:[bx+02]
	cbw
	shl	ax,cl
	mov	bl,es:[bx+03]
	mov	bh,00
	add	ax,bx
	add	dx,ax
	pop	ax
	adc	ax,0000
	mov	[bp-0E],ax
	mov	[bp-10],dx
	mov	ax,[bp-0E]
	cmp	ax,[bp+0A]
	jl	2779

l0800_2727:
	jnz	272E

l0800_2729:
	cmp	dx,[bp+08]
	jc	2779

l0800_272E:
	mov	ax,[bp-0E]
	mov	dx,[bp-10]
	add	dx,[bp+04]
	adc	ax,[bp+06]
	cwd
	mov	cl,08
	call	8C8A
	les	bx,[bp-04]
	mov	es:[bx],al
	mov	ax,[bp-0E]
	mov	dx,[bp-10]
	add	dx,[bp+04]
	adc	ax,[bp+06]
	cwd
	mov	es:[bx+01],al
	mov	dx,[bp-0E]
	mov	ax,[bp-10]
	add	ax,[bp+04]
	adc	dx,[bp+06]
	mov	cl,08
	call	8C8A
	les	bx,[bp-04]
	mov	es:[bx+02],al
	mov	al,[bp-10]
	add	al,[bp+04]
	mov	es:[bx+03],al

l0800_2779:
	add	word ptr [bp-04],04

l0800_277D:
	les	bx,[bp-04]
	cmp	byte ptr es:[bx],00
	jz	2789

l0800_2786:
	jmp	26E2

l0800_2789:
	inc	word ptr [bp-04]
	mov	ax,[bp-04]
	xor	dx,dx
	sub	ax,[2E51]
	sbb	dx,00
	mov	cl,08
	call	8C8A
	les	bx,[bp-0C]
	mov	es:[bx],al
	mov	ax,[bp-04]
	xor	dx,dx
	sub	ax,[2E51]
	sbb	dx,00
	mov	es:[bx+01],al

l0800_27B3:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	cmp	ax,[bp-06]
	jz	27C1

l0800_27BE:
	jmp	26C3

l0800_27C1:
	cmp	dx,[bp-08]
	jz	27C9

l0800_27C6:
	jmp	26C3

l0800_27C9:
	mov	ax,[bp+08]
	or	ax,[bp+0A]
	jnz	27DF

l0800_27D1:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	add	[45A6],dx
	adc	[45A8],ax

l0800_27DF:
	mov	sp,bp
	pop	bp
	ret
0800:27E3          55 8B EC 83 EC 6A FF 76 06 FF 76 04 16    U....j.v..v..
0800:27F0 8D 46 96 50 E8 12 0D 83 C4 08 16 8D 46 96 50 E8 .F.P........F.P.
0800:2800 3B 00 83 C4 04 89 56 FE 89 46 FC 0B C2 75 08 33 ;.....V..F...u.3
0800:2810 D2 33 C0 8B E5 5D C3 FF 76 06 FF 76 04 16 8D 46 .3...]..v..v...F
0800:2820 96 50 E8 7E 0D 83 C4 08 FF 76 FE FF 76 FC 16 8D .P.~.....v..v...
0800:2830 46 96 50 E8 FB 00 83 C4 08 8B E5 5D C3          F.P........].  

;; fn0800_283D: 0800:283D
;;   Called from:
;;     0800:14AC (in fn0800_12E2)
;;     0800:15EB (in fn0800_12E2)
fn0800_283D proc
	push	bp
	mov	bp,sp
	sub	sp,08
	push	si
	push	di
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_284D:
	repne scasb

l0800_284F:
	not	cx
	mov	ax,003A
	sub	di,cx

l0800_2856:
	repne scasb

l0800_2858:
	jz	2861

l0800_285A:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2861:
	dec	di
	mov	ax,es
	or	di,ax
	jz	288E

l0800_2868:
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_2870:
	repne scasb

l0800_2872:
	not	cx
	mov	ax,003A
	sub	di,cx

l0800_2879:
	repne scasb

l0800_287B:
	jz	2884

l0800_287D:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2884:
	dec	di
	mov	ax,es
	inc	di
	mov	[bp+06],ax
	mov	[bp+04],di

l0800_288E:
	les	bx,[2E51]
	mov	al,es:[bx+04]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+05]
	mov	dh,00
	add	ax,dx
	mov	dx,[2E53]
	add	bx,ax
	mov	[bp-02],dx
	mov	[bp-04],bx
	mov	ax,[2E53]
	mov	dx,[2E51]
	add	dx,08
	mov	[bp-06],ax
	mov	[bp-08],dx
	jmp	2917

l0800_28C2:
	mov	si,[bp-08]
	add	si,02
	push	ds
	mov	ds,[bp-06]
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_28D4:
	repne scasb

l0800_28D6:
	not	cx
	sub	di,cx

l0800_28DA:
	rep cmpsb

l0800_28DC:
	jz	28E3

l0800_28DE:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_28E3:
	pop	ds
	or	ax,ax
	jnz	28F4

l0800_28E8:
	mov	dx,[bp-06]
	mov	ax,[bp-08]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_28F4:
	les	bx,[bp-08]
	mov	al,es:[bx]
	cbw
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+01]
	mov	dh,00
	add	ax,dx
	mov	dx,[2E53]
	mov	bx,[2E51]
	add	bx,ax
	mov	[bp-06],dx
	mov	[bp-08],bx

l0800_2917:
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	cmp	ax,[bp-02]
	jnz	28C2

l0800_2922:
	cmp	dx,[bp-04]
	jnz	28C2

l0800_2927:
	xor	dx,dx
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_2931: 0800:2931
;;   Called from:
;;     0800:14D9 (in fn0800_12E2)
;;     0800:15F9 (in fn0800_12E2)
fn0800_2931 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	les	di,[bp+08]
	add	di,02
	xor	ax,ax
	mov	cx,FFFF

l0800_2941:
	repne scasb

l0800_2943:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2949:
	repne scasb

l0800_294B:
	jz	2954

l0800_294D:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2954:
	dec	di
	mov	ax,es
	inc	di
	mov	[bp+0A],ax
	mov	[bp+08],di
	jmp	29B4

l0800_2960:
	mov	si,[bp+08]
	push	ds
	mov	ds,[bp+0A]
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_296F:
	repne scasb

l0800_2971:
	not	cx
	sub	di,cx

l0800_2975:
	rep cmpsb

l0800_2977:
	jz	297E

l0800_2979:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_297E:
	pop	ds
	or	ax,ax
	jnz	298D

l0800_2983:
	mov	dx,[bp+0A]
	mov	ax,[bp+08]
	pop	di
	pop	si
	pop	bp
	ret

l0800_298D:
	les	di,[bp+08]
	xor	ax,ax
	mov	cx,FFFF

l0800_2995:
	repne scasb

l0800_2997:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_299D:
	repne scasb

l0800_299F:
	jz	29A8

l0800_29A1:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_29A8:
	dec	di
	mov	ax,es
	add	di,05
	mov	[bp+0A],ax
	mov	[bp+08],di

l0800_29B4:
	les	bx,[bp+08]
	cmp	byte ptr es:[bx],00
	jnz	2960

l0800_29BD:
	xor	dx,dx
	xor	ax,ax
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_29C5: 0800:29C5
;;   Called from:
;;     0800:19CB (in fn0800_18D9)
;;     0800:1CC3 (in fn0800_19EE)
;;     0800:2C8E (in fn0800_29C5)
fn0800_29C5 proc
	push	bp
	mov	bp,sp
	sub	sp,78
	push	si
	push	di
	mov	ax,[45AE]
	or	ax,[45B0]
	jnz	2A07

l0800_29D6:
	mov	ax,[45AA]
	or	ax,[45AC]
	jnz	2A07

l0800_29DF:
	push	ds
	mov	ax,4541
	push	ax
	call	2DE2
	add	sp,04
	or	ax,ax
	jnz	29F6

l0800_29EE:
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_29F6:
	mov	ax,[2E53]
	mov	dx,[2E51]
	add	dx,08
	mov	[45B0],ax
	mov	[45AE],dx

l0800_2A07:
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	3509
	add	sp,08
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp-78]
	push	ax
	call	35A3
	add	sp,08
	les	bx,[2E51]
	mov	al,es:[bx+04]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+05]
	mov	dh,00
	add	ax,dx
	mov	dx,[2E53]
	add	bx,ax
	mov	[bp-02],dx
	mov	[bp-04],bx
	jmp	2C53

l0800_2A4C:
	mov	ax,[45AA]
	or	ax,[45AC]
	jnz	2A58

l0800_2A55:
	jmp	2C12

l0800_2A58:
	jmp	2B54

l0800_2A5B:
	push	ss
	lea	ax,[bp-78]
	push	ax
	push	word ptr [45AC]
	push	word ptr [45AA]
	call	4357
	add	sp,08
	or	ax,ax
	jnz	2A75

l0800_2A72:
	jmp	2B2B

l0800_2A75:
	les	bx,[bp+04]
	mov	ax,[45B0]
	mov	dx,[45AE]
	mov	es:[bx+02],ax
	mov	es:[bx],dx
	les	bx,[bp+08]
	mov	ax,[45AC]
	mov	dx,[45AA]
	mov	es:[bx+02],ax
	mov	es:[bx],dx
	les	di,[45AE]
	add	di,02
	push	es
	mov	es,[bp+0E]
	push	di
	mov	di,[bp+0C]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_2AB3:
	repne scasb

l0800_2AB5:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_2AC4:
	rep movsw

l0800_2AC6:
	adc	cx,cx

l0800_2AC8:
	rep movsb

l0800_2ACA:
	pop	ds
	les	di,[bp+0C]
	push	es
	mov	es,[45AC]
	push	di
	mov	di,[45AA]
	xor	ax,ax
	mov	cx,FFFF

l0800_2ADD:
	repne scasb

l0800_2ADF:
	not	cx
	sub	di,cx
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	mov	dx,ds
	mov	ds,ax
	push	cx
	mov	cx,FFFF
	xor	ax,ax

l0800_2AF3:
	repne scasb

l0800_2AF5:
	dec	di
	pop	cx

l0800_2AF7:
	rep movsb

l0800_2AF9:
	mov	ds,dx
	les	di,[45AA]
	mov	cx,FFFF

l0800_2B02:
	repne scasb

l0800_2B04:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2B0A:
	repne scasb

l0800_2B0C:
	jz	2B15

l0800_2B0E:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2B15:
	dec	di
	mov	ax,es
	add	di,05
	mov	[45AC],ax
	mov	[45AA],di
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_2B2B:
	les	di,[45AA]
	xor	ax,ax
	mov	cx,FFFF

l0800_2B34:
	repne scasb

l0800_2B36:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2B3C:
	repne scasb

l0800_2B3E:
	jz	2B47

l0800_2B40:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2B47:
	dec	di
	mov	ax,es
	add	di,05
	mov	[45AC],ax
	mov	[45AA],di

l0800_2B54:
	les	bx,[45AA]
	cmp	byte ptr es:[bx],00
	jz	2B61

l0800_2B5E:
	jmp	2A5B

l0800_2B61:
	les	bx,[45AE]
	mov	al,es:[bx]
	cbw
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+01]
	mov	dh,00
	add	ax,dx
	mov	dx,[2E53]
	mov	bx,[2E51]
	add	bx,ax
	mov	[45B0],dx
	mov	[45AE],bx
	jmp	2C12

l0800_2B8A:
	cmp	word ptr [2A1B],00
	jz	2BBB

l0800_2B91:
	push	ss
	pop	es
	lea	di,[bp-6A]
	xor	ax,ax
	mov	cx,FFFF

l0800_2B9B:
	repne scasb

l0800_2B9D:
	not	cx
	dec	cx
	push	cx
	push	ss
	lea	ax,[bp-6A]
	push	ax
	mov	ax,[45AE]
	add	ax,0002
	push	word ptr [45B0]
	push	ax
	call	BFE6
	add	sp,0A
	or	ax,ax
	jz	2C29

l0800_2BBB:
	cmp	word ptr [2A1B],00
	jnz	2BEC

l0800_2BC2:
	mov	si,[45AE]
	add	si,02
	push	ds
	mov	ds,[45B0]
	push	ss
	pop	es
	lea	di,[bp-6A]
	xor	ax,ax
	mov	cx,FFFF

l0800_2BD8:
	repne scasb

l0800_2BDA:
	not	cx
	sub	di,cx

l0800_2BDE:
	rep cmpsb

l0800_2BE0:
	jz	2BE7

l0800_2BE2:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_2BE7:
	pop	ds
	or	ax,ax
	jz	2C29

l0800_2BEC:
	les	bx,[45AE]
	mov	al,es:[bx]
	cbw
	mov	cl,08
	shl	ax,cl
	mov	dl,es:[bx+01]
	mov	dh,00
	add	ax,dx
	mov	dx,[2E53]
	mov	bx,[2E51]
	add	bx,ax
	mov	[45B0],dx
	mov	[45AE],bx

l0800_2C12:
	mov	ax,[45B0]
	mov	dx,[45AE]
	cmp	ax,[bp-02]
	jz	2C21

l0800_2C1E:
	jmp	2B8A

l0800_2C21:
	cmp	dx,[bp-04]
	jz	2C29

l0800_2C26:
	jmp	2B8A

l0800_2C29:
	les	di,[45AE]
	add	di,02
	xor	ax,ax
	mov	cx,FFFF

l0800_2C35:
	repne scasb

l0800_2C37:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2C3D:
	repne scasb

l0800_2C3F:
	jz	2C48

l0800_2C41:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2C48:
	dec	di
	mov	ax,es
	inc	di
	mov	[45AC],ax
	mov	[45AA],di

l0800_2C53:
	mov	ax,[45B0]
	mov	dx,[45AE]
	cmp	ax,[bp-02]
	jz	2C62

l0800_2C5F:
	jmp	2A4C

l0800_2C62:
	cmp	dx,[bp-04]
	jz	2C6A

l0800_2C67:
	jmp	2A4C

l0800_2C6A:
	xor	ax,ax
	xor	dx,dx
	mov	[45AC],ax
	mov	[45AA],dx
	mov	[45B0],ax
	mov	[45AE],dx
	push	word ptr [bp+0E]
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	29C5
	add	sp,0C
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_2C9A: 0800:2C9A
;;   Called from:
;;     0800:02DD (in main)
fn0800_2C9A proc
	push	si
	xor	cx,cx
	mov	si,2A29

l0800_2CA0:
	mov	dx,cx
	mov	bx,0008
	jmp	2CBD

l0800_2CA7:
	test	dx,0001
	jz	2CB6

l0800_2CAD:
	mov	ax,dx
	shr	ax,01
	xor	ax,A001
	jmp	2CBA

l0800_2CB6:
	mov	ax,dx
	shr	ax,01

l0800_2CBA:
	mov	dx,ax
	dec	bx

l0800_2CBD:
	or	bx,bx
	jnz	2CA7

l0800_2CC1:
	mov	[si],dx
	add	si,02
	inc	cx
	cmp	cx,0100
	jc	2CA0

l0800_2CCD:
	pop	si
	ret

;; fn0800_2CCF: 0800:2CCF
;;   Called from:
;;     0800:1426 (in fn0800_12E2)
;;     0800:143F (in fn0800_12E2)
;;     0800:1E3F (in fn0800_1CF6)
;;     0800:1E8B (in fn0800_1E5E)
;;     0800:2D7B (in fn0800_2D0A)
fn0800_2CCF proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+08]
	jmp	2CFD

l0800_2CD8:
	les	bx,[bp+04]
	mov	al,[bp+0A]
	xor	al,es:[bx]
	mov	ah,00
	and	ax,00FF
	shl	ax,01
	mov	bx,ax
	mov	ax,[bx+2A29]
	mov	dx,[bp+0A]
	mov	cl,08
	shr	dx,cl
	xor	ax,dx
	mov	[bp+0A],ax
	inc	word ptr [bp+04]

l0800_2CFD:
	mov	ax,si
	dec	si
	or	ax,ax
	jnz	2CD8

l0800_2D04:
	mov	ax,[bp+0A]
	pop	si
	pop	bp
	ret

;; fn0800_2D0A: 0800:2D0A
;;   Called from:
;;     0800:5444 (in fn0800_5374)
fn0800_2D0A proc
	push	bp
	mov	bp,sp
	sub	sp,0C
	push	si
	xor	ax,ax
	mov	dx,FFF0
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AD2F
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	xor	si,si
	jmp	2D8F

l0800_2D3A:
	cmp	word ptr [bp+0A],00
	jc	2D4F

l0800_2D40:
	ja	2D48

l0800_2D42:
	cmp	word ptr [bp+08],F0
	jbe	2D4F

l0800_2D48:
	xor	dx,dx
	mov	ax,FFF0
	jmp	2D55

l0800_2D4F:
	mov	dx,[bp+0A]
	mov	ax,[bp+08]

l0800_2D55:
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [bp-06]
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	4110
	add	sp,0C
	push	si
	push	word ptr [bp-08]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	2CCF
	add	sp,08
	mov	si,ax
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	sub	[bp+08],dx
	sbb	[bp+0A],ax

l0800_2D8F:
	mov	ax,[bp+08]
	or	ax,[bp+0A]
	jnz	2D3A

l0800_2D97:
	xor	ax,ax
	push	ax
	push	word ptr [bp-0A]
	push	word ptr [bp-0C]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	4346
	add	sp,04
	mov	ax,si
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_2DBF: 0800:2DBF
;;   Called from:
;;     0800:02FB (in main)
;;     0800:1565 (in fn0800_12E2)
;;     0800:1938 (in fn0800_18D9)
;;     0800:1A84 (in fn0800_19EE)
fn0800_2DBF proc
	push	bp
	mov	bp,sp
	mov	word ptr [4656],FFFF
	mov	word ptr [4654],0000
	mov	ax,[2A27]
	mov	[4652],ax
	mov	ax,[bp+04]
	mov	[4650],ax
	mov	ax,[bp+06]
	mov	[464E],ax
	pop	bp
	ret

;; fn0800_2DE2: 0800:2DE2
;;   Called from:
;;     0800:10F9 (in fn0800_0DE8)
;;     0800:12C7 (in fn0800_112D)
;;     0800:152C (in fn0800_12E2)
;;     0800:189C (in fn0800_12E2)
;;     0800:29E4 (in fn0800_29C5)
;;     0800:2E99 (in fn0800_2DE2)
;;     0800:2F45 (in fn0800_2DE2)
;;     0800:2FFB (in fn0800_2DE2)
;;     0800:30E4 (in fn0800_2DE2)
;;     0800:319B (in fn0800_2DE2)
fn0800_2DE2 proc
	push	bp
	mov	bp,sp
	sub	sp,10
	push	si
	push	di
	cmp	word ptr [4654],00
	jnz	2E11

l0800_2DF1:
	mov	ax,[4652]
	cmp	ax,[4656]
	jnz	2E11

l0800_2DFA:
	cmp	word ptr [464E],00
	jz	2E11

l0800_2E01:
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10]
	push	ax
	call	3479
	add	sp,06
	mov	si,ax

l0800_2E11:
	cmp	word ptr [4654],00
	jnz	2E1B

l0800_2E18:
	jmp	2F51

l0800_2E1B:
	mov	word ptr [4617],0000
	push	word ptr [461D]
	push	word ptr [461B]
	mov	ax,0065
	push	ax
	push	ds
	mov	ax,45B2
	push	ax
	call	A77D
	add	sp,0A
	or	ax,dx
	jnz	2E3F

l0800_2E3C:
	jmp	2F1B

l0800_2E3F:
	push	ds
	pop	es
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF

l0800_2E49:
	repne scasb

l0800_2E4B:
	not	cx
	mov	ax,000D
	sub	di,cx

l0800_2E52:
	repne scasb

l0800_2E54:
	jz	2E5D

l0800_2E56:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2E5D:
	dec	di
	mov	ax,es
	or	di,ax
	jz	2E8B

l0800_2E64:
	push	ds
	pop	es
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF

l0800_2E6E:
	repne scasb

l0800_2E70:
	not	cx
	mov	ax,000D
	sub	di,cx

l0800_2E77:
	repne scasb

l0800_2E79:
	jz	2E82

l0800_2E7B:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_2E82:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di],00

l0800_2E8B:
	mov	al,[45B2]
	cbw
	or	ax,ax
	jnz	2EA5

l0800_2E93:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	2DE2
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_2EA5:
	push	ds
	pop	es
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF

l0800_2EAF:
	repne scasb

l0800_2EB1:
	not	cx
	dec	cx
	mov	[4619],cx
	push	ds
	mov	ax,45B2
	push	ax
	call	0C6C
	add	sp,04
	cmp	word ptr [464E],00
	jnz	2EFE

l0800_2ECA:
	les	di,[bp+04]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF

l0800_2ED9:
	repne scasb

l0800_2EDB:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_2EEA:
	rep movsw

l0800_2EEC:
	adc	cx,cx

l0800_2EEE:
	rep movsb

l0800_2EF0:
	pop	ds
	inc	word ptr [4617]
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_2EFE:
	push	ds
	mov	ax,45B2
	push	ax
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10]
	push	ax
	call	33CD
	add	sp,0A
	mov	si,ax
	mov	word ptr [4654],0000
	jmp	2F51

l0800_2F1B:
	push	word ptr [461D]
	push	word ptr [461B]
	call	A614
	add	sp,04
	mov	word ptr [4654],0000
	mov	word ptr [461D],0000
	mov	word ptr [461B],0000
	inc	word ptr [4652]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	2DE2
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_2F51:
	mov	ax,[4652]
	cmp	ax,[4656]
	jnz	2F5D

l0800_2F5A:
	jmp	3066

l0800_2F5D:
	cmp	ax,[269A]
	jnz	2F6B

l0800_2F63:
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_2F6B:
	mov	ax,[4652]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	push	word ptr es:[bx+02]
	push	word ptr es:[bx]
	push	ds
	mov	ax,45B2
	push	ax
	call	BF9E
	add	sp,08
	push	ds
	pop	es
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF

l0800_2F94:
	repne scasb

l0800_2F96:
	not	cx
	dec	cx
	mov	[4619],cx
	push	ds
	mov	ax,45B2
	push	ax
	call	0C6C
	add	sp,04
	mov	ax,[4652]
	mov	[4656],ax
	mov	word ptr [4617],0000
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	les	bx,es:[bx]
	cmp	byte ptr es:[bx],40
	jnz	3007

l0800_2FC7:
	push	ds
	mov	ax,0A13
	push	ax
	mov	ax,[4652]
	shl	ax,01
	shl	ax,01
	les	bx,[269C]
	add	bx,ax
	mov	ax,es:[bx]
	inc	ax
	push	word ptr es:[bx+02]
	push	ax
	call	4234
	add	sp,08
	mov	[461D],dx
	mov	[461B],ax
	mov	word ptr [4654],0001
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	2DE2
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_3007:
	push	ds
	mov	ax,45B2
	push	ax
	call	335C
	add	sp,04
	cmp	word ptr [464E],00
	jnz	3051

l0800_3019:
	les	di,[bp+04]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF

l0800_3028:
	repne scasb

l0800_302A:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_3039:
	rep movsw

l0800_303B:
	adc	cx,cx

l0800_303D:
	rep movsb

l0800_303F:
	pop	ds
	inc	word ptr [4617]
	inc	word ptr [4652]
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_3051:
	push	ds
	mov	ax,45B2
	push	ax
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10]
	push	ax
	call	33CD
	add	sp,0A
	mov	si,ax

l0800_3066:
	or	si,si
	jnz	306D

l0800_306A:
	jmp	30F0

l0800_306D:
	cmp	word ptr [2A1B],00
	jz	309E

l0800_3074:
	jmp	309A

l0800_3076:
	push	ds
	mov	ax,45B2
	push	ax
	call	31B4
	add	sp,04
	or	ax,ax
	jz	309E

l0800_3085:
	push	ds
	mov	ax,45B2
	push	ax
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10]
	push	ax
	call	33CD
	add	sp,0A
	mov	si,ax

l0800_309A:
	or	si,si
	jnz	3076

l0800_309E:
	or	si,si
	jz	30F0

l0800_30A2:
	mov	bx,[4619]
	mov	byte ptr [bx+45B2],00
	cmp	word ptr [4617],00
	jnz	30C9

l0800_30B2:
	cmp	word ptr [4650],00
	jz	30C9

l0800_30B9:
	push	ds
	mov	ax,45B2
	push	ax
	push	ds
	mov	ax,0A17
	push	ax
	call	B2EF
	add	sp,08

l0800_30C9:
	mov	ax,[461B]
	or	ax,[461D]
	jz	30DA

l0800_30D2:
	mov	word ptr [4654],0001
	jmp	30DE

l0800_30DA:
	inc	word ptr [4652]

l0800_30DE:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	2DE2
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_30F0:
	push	ds
	mov	ax,45B2
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3509
	add	sp,08
	push	ss
	pop	es
	lea	di,[bp-10]
	push	es
	mov	es,[bp+06]
	push	di
	mov	di,[bp+04]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_311B:
	repne scasb

l0800_311D:
	not	cx
	sub	di,cx
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	mov	[bp-02],ds
	mov	ds,ax
	push	cx
	mov	cx,FFFF
	xor	ax,ax

l0800_3132:
	repne scasb

l0800_3134:
	dec	di
	pop	cx

l0800_3136:
	rep movsb

l0800_3138:
	mov	ds,[bp-02]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	0C6C
	add	sp,04
	mov	si,[bp+04]
	mov	cx,[bp+06]
	push	ds
	pop	es
	mov	di,4348
	push	ds
	mov	ds,cx
	xor	ax,ax
	mov	cx,FFFF

l0800_315A:
	repne scasb

l0800_315C:
	not	cx
	sub	di,cx

l0800_3160:
	rep cmpsb

l0800_3162:
	jz	3169

l0800_3164:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_3169:
	pop	ds
	or	ax,ax
	jz	3195

l0800_316E:
	mov	si,[bp+04]
	mov	cx,[bp+06]
	push	ds
	pop	es
	mov	di,4477
	push	ds
	mov	ds,cx
	xor	ax,ax
	mov	cx,FFFF

l0800_3181:
	repne scasb

l0800_3183:
	not	cx
	sub	di,cx

l0800_3187:
	rep cmpsb

l0800_3189:
	jz	3190

l0800_318B:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_3190:
	pop	ds
	or	ax,ax
	jnz	31A7

l0800_3195:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	2DE2
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_31A7:
	inc	word ptr [4617]
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_31B4: 0800:31B4
;;   Called from:
;;     0800:307B (in fn0800_2DE2)
fn0800_31B4 proc
	push	bp
	mov	bp,sp
	sub	sp,0082
	push	si
	push	di

l0800_31BD:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	ss
	lea	ax,[bp+FF7E]
	push	ax
	call	3509
	add	sp,08
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	ss
	lea	ax,[bp-0E]
	push	ax
	call	35A3
	add	sp,08
	mov	bx,[09AC]
	shl	bx,01
	push	word ptr [bx+09AE]
	push	ss
	lea	ax,[bp+FF7E]
	push	ax
	push	ss
	lea	ax,[bp-1C]
	push	ax
	call	32CD
	add	sp,0A
	or	ax,ax
	jnz	323E

l0800_31FF:
	push	ss
	lea	ax,[bp-0E]
	push	ax
	push	ds
	mov	ax,0A30
	push	ax
	push	ss
	lea	ax,[bp-1C]
	push	ax
	push	ss
	lea	ax,[bp+FF7E]
	push	ax
	push	ds
	mov	ax,0A27
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	BEA2
	add	sp,18
	inc	word ptr [09AC]
	mov	bx,[09AC]
	shl	bx,01
	mov	word ptr [bx+09AE],0000
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_323E:
	cmp	word ptr [09AC],00
	jnz	3253

l0800_3245:
	mov	word ptr [09AE],0000
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_3253:
	push	ss
	pop	es
	lea	di,[bp+FF7E]
	xor	ax,ax
	mov	cx,FFFF

l0800_325E:
	repne scasb

l0800_3260:
	not	cx
	dec	cx
	dec	cx
	lea	ax,[bp+FF7E]
	add	cx,ax
	mov	bx,cx
	mov	byte ptr ss:[bx],00
	push	ss
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3509
	add	sp,08
	push	ss
	pop	es
	lea	di,[bp-0E]
	push	es
	mov	es,[bp+06]
	push	di
	mov	di,[bp+04]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_3298:
	repne scasb

l0800_329A:
	not	cx
	sub	di,cx
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	mov	dx,ds
	mov	ds,ax
	push	cx
	mov	cx,FFFF
	xor	ax,ax

l0800_32AE:
	repne scasb

l0800_32B0:
	dec	di
	pop	cx

l0800_32B2:
	rep movsb

l0800_32B4:
	mov	ds,dx
	dec	word ptr [09AC]
	mov	bx,[09AC]
	shl	bx,01
	inc	word ptr [bx+09AE]
	jmp	31BD
0800:32C7                      5F 5E 8B E5 5D C3                 _^..].  

;; fn0800_32CD: 0800:32CD
;;   Called from:
;;     0800:31F5 (in fn0800_31B4)
fn0800_32CD proc
	push	bp
	mov	bp,sp
	sub	sp,66
	push	si
	push	di
	push	ss
	pop	es
	lea	di,[bp-66]
	push	es
	mov	es,[bp+0A]
	push	di
	mov	di,[bp+08]
	xor	ax,ax
	mov	cx,FFFF

l0800_32E7:
	repne scasb

l0800_32E9:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_32F8:
	rep movsw

l0800_32FA:
	adc	cx,cx

l0800_32FC:
	rep movsb

l0800_32FE:
	pop	ds
	push	ss
	pop	es
	lea	di,[bp-66]
	mov	si,0A32
	mov	cx,FFFF
	xor	ax,ax

l0800_330C:
	repne scasb

l0800_330E:
	dec	di
	mov	cx,0004

l0800_3312:
	rep movsb

l0800_3314:
	push	ss
	lea	ax,[bp-66]
	push	ax
	mov	ax,0010
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	33CD
	add	sp,0A
	or	ax,ax
	jz	334D

l0800_332D:
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_3336:
	mov	ax,0010
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3479
	add	sp,06
	or	ax,ax
	jnz	3353

l0800_334A:
	dec	word ptr [bp+0C]

l0800_334D:
	cmp	word ptr [bp+0C],00
	jnz	3336

l0800_3353:
	mov	ax,[bp+0C]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_335C: 0800:335C
;;   Called from:
;;     0800:300C (in fn0800_2DE2)
fn0800_335C proc
	push	bp
	mov	bp,sp
	sub	sp,1C
	push	si
	push	di
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	mov	ax,0010
	push	ax
	push	ss
	lea	ax,[bp-0E]
	push	ax
	call	33CD
	add	sp,0A
	or	ax,ax
	jnz	33C7

l0800_337D:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	ss
	lea	ax,[bp-1C]
	push	ax
	call	35A3
	add	sp,08
	mov	ax,ss
	lea	si,[bp-0E]
	push	ds
	mov	ds,ax
	push	ss
	pop	es
	lea	di,[bp-1C]
	xor	ax,ax
	mov	cx,FFFF

l0800_33A0:
	repne scasb

l0800_33A2:
	not	cx
	sub	di,cx

l0800_33A6:
	rep cmpsb

l0800_33A8:
	jz	33AF

l0800_33AA:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_33AF:
	pop	ds
	or	ax,ax
	jnz	33C7

l0800_33B4:
	les	di,[bp+04]
	mov	si,0A36
	mov	cx,FFFF
	xor	ax,ax

l0800_33BF:
	repne scasb

l0800_33C1:
	dec	di
	mov	cx,0005

l0800_33C5:
	rep movsb

l0800_33C7:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_33CD: 0800:33CD
;;   Called from:
;;     0800:2F0B (in fn0800_2DE2)
;;     0800:305E (in fn0800_2DE2)
;;     0800:3092 (in fn0800_2DE2)
;;     0800:3323 (in fn0800_32CD)
;;     0800:3373 (in fn0800_335C)
;;     0800:36F4 (in fn0800_3678)
fn0800_33CD proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	push	word ptr [bp+08]
	push	ds
	mov	ax,4623
	push	ax
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	call	A817
	add	sp,0A
	or	ax,ax
	jz	33F1

l0800_33EA:
	mov	ax,0001
	pop	di
	pop	si
	pop	bp
	ret

l0800_33F1:
	push	ds
	pop	es
	mov	di,0A3B
	mov	si,4641
	mov	cx,0002
	xor	ax,ax

l0800_33FE:
	rep cmpsb

l0800_3400:
	jz	3407

l0800_3402:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_3407:
	or	ax,ax
	jz	3431

l0800_340B:
	push	ds
	pop	es
	mov	di,0A3D
	mov	si,4641
	mov	cx,0003
	xor	ax,ax

l0800_3418:
	rep cmpsb

l0800_341A:
	jz	3421

l0800_341C:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_3421:
	or	ax,ax
	jz	3431

l0800_3425:
	mov	al,[4638]
	cbw
	and	ax,[bp+08]
	cmp	ax,[bp+08]
	jz	3444

l0800_3431:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3479
	add	sp,06
	pop	di
	pop	si
	pop	bp
	ret

l0800_3444:
	mov	ax,[bp+04]
	or	ax,[bp+06]
	jz	3473

l0800_344C:
	les	di,[bp+04]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,4641
	xor	ax,ax
	mov	cx,FFFF

l0800_345B:
	repne scasb

l0800_345D:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_346C:
	rep movsw

l0800_346E:
	adc	cx,cx

l0800_3470:
	rep movsb

l0800_3472:
	pop	ds

l0800_3473:
	xor	ax,ax
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_3479: 0800:3479
;;   Called from:
;;     0800:2E09 (in fn0800_2DE2)
;;     0800:3340 (in fn0800_32CD)
;;     0800:343A (in fn0800_33CD)
fn0800_3479 proc
	push	bp
	mov	bp,sp
	push	si
	push	di

l0800_347E:
	push	ds
	mov	ax,4623
	push	ax
	call	A84A
	add	sp,04
	or	ax,ax
	jz	3494

l0800_348D:
	mov	ax,0001
	pop	di
	pop	si
	pop	bp
	ret

l0800_3494:
	push	ds
	pop	es
	mov	di,0A3B
	mov	si,4641
	mov	cx,0002
	xor	ax,ax

l0800_34A1:
	rep cmpsb

l0800_34A3:
	jz	34AA

l0800_34A5:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_34AA:
	or	ax,ax
	jz	347E

l0800_34AE:
	push	ds
	pop	es
	mov	di,0A3D
	mov	si,4641
	mov	cx,0003
	xor	ax,ax

l0800_34BB:
	rep cmpsb

l0800_34BD:
	jz	34C4

l0800_34BF:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_34C4:
	or	ax,ax
	jz	347E

l0800_34C8:
	mov	al,[4638]
	cbw
	and	ax,[bp+08]
	cmp	ax,[bp+08]
	jnz	347E

l0800_34D4:
	mov	ax,[bp+04]
	or	ax,[bp+06]
	jz	3503

l0800_34DC:
	les	di,[bp+04]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,4641
	xor	ax,ax
	mov	cx,FFFF

l0800_34EB:
	repne scasb

l0800_34ED:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_34FC:
	rep movsw

l0800_34FE:
	adc	cx,cx

l0800_3500:
	rep movsb

l0800_3502:
	pop	ds

l0800_3503:
	xor	ax,ax
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_3509: 0800:3509
;;   Called from:
;;     0800:0CA6 (in fn0800_0C93)
;;     0800:1490 (in fn0800_12E2)
;;     0800:15C4 (in fn0800_12E2)
;;     0800:1BF4 (in fn0800_19EE)
;;     0800:1C23 (in fn0800_19EE)
;;     0800:2A11 (in fn0800_29C5)
;;     0800:30FB (in fn0800_2DE2)
;;     0800:31C9 (in fn0800_31B4)
;;     0800:3278 (in fn0800_31B4)
;;     0800:37A0 (in fn0800_3764)
;;     0800:37F0 (in fn0800_37DF)
;;     0800:39BF (in fn0800_3992)
;;     0800:3CAB (in fn0800_3C99)
fn0800_3509 proc
	push	bp
	mov	bp,sp
	sub	sp,58
	push	si
	push	di
	push	ss
	lea	ax,[bp-14]
	push	ax
	push	ss
	lea	ax,[bp-0E]
	push	ax
	push	ss
	lea	ax,[bp-58]
	push	ax
	push	ss
	lea	ax,[bp-04]
	push	ax
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	BE3B
	add	sp,14
	push	ss
	pop	es
	lea	di,[bp-04]
	push	es
	mov	es,[bp+06]
	push	di
	mov	di,[bp+04]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_354B:
	repne scasb

l0800_354D:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_355C:
	rep movsw

l0800_355E:
	adc	cx,cx

l0800_3560:
	rep movsb

l0800_3562:
	pop	ds
	push	ss
	pop	es
	lea	di,[bp-58]
	push	es
	mov	es,[bp+06]
	push	di
	mov	di,[bp+04]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_357D:
	repne scasb

l0800_357F:
	not	cx
	sub	di,cx
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	mov	[bp-16],ds
	mov	ds,ax
	push	cx
	mov	cx,FFFF
	xor	ax,ax

l0800_3594:
	repne scasb

l0800_3596:
	dec	di
	pop	cx

l0800_3598:
	rep movsb

l0800_359A:
	mov	ds,[bp-16]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_35A3: 0800:35A3
;;   Called from:
;;     0800:0AD1 (in fn0800_0ABC)
;;     0800:14A1 (in fn0800_12E2)
;;     0800:15D5 (in fn0800_12E2)
;;     0800:1C5B (in fn0800_19EE)
;;     0800:2A21 (in fn0800_29C5)
;;     0800:31DA (in fn0800_31B4)
;;     0800:3388 (in fn0800_335C)
;;     0800:381D (in fn0800_37DF)
;;     0800:39EC (in fn0800_3992)
fn0800_35A3 proc
	push	bp
	mov	bp,sp
	sub	sp,58
	push	si
	push	di
	push	ss
	lea	ax,[bp-14]
	push	ax
	push	ss
	lea	ax,[bp-0E]
	push	ax
	push	ss
	lea	ax,[bp-58]
	push	ax
	push	ss
	lea	ax,[bp-04]
	push	ax
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	BE3B
	add	sp,14
	push	ss
	pop	es
	lea	di,[bp-0E]
	push	es
	mov	es,[bp+06]
	push	di
	mov	di,[bp+04]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_35E5:
	repne scasb

l0800_35E7:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_35F6:
	rep movsw

l0800_35F8:
	adc	cx,cx

l0800_35FA:
	rep movsb

l0800_35FC:
	pop	ds
	push	ss
	pop	es
	lea	di,[bp-14]
	push	es
	mov	es,[bp+06]
	push	di
	mov	di,[bp+04]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,FFFF

l0800_3617:
	repne scasb

l0800_3619:
	not	cx
	sub	di,cx
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	mov	[bp-16],ds
	mov	ds,ax
	push	cx
	mov	cx,FFFF
	xor	ax,ax

l0800_362E:
	repne scasb

l0800_3630:
	dec	di
	pop	cx

l0800_3632:
	rep movsb

l0800_3634:
	mov	ds,[bp-16]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_363D: 0800:363D
;;   Called from:
;;     0800:38CA (in fn0800_388C)
;;     0800:38F9 (in fn0800_388C)
;;     0800:3AA0 (in fn0800_3992)
;;     0800:3AB0 (in fn0800_3992)
fn0800_363D proc
	push	bp
	mov	bp,sp
	sub	sp,2C
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-2C]
	push	ax
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	A817
	add	sp,0A
	or	ax,ax
	jz	3662

l0800_365B:
	mov	ax,0001
	mov	sp,bp
	pop	bp
	ret

l0800_3662:
	les	bx,[bp+04]
	mov	ax,[bp-16]
	mov	es:[bx],ax
	mov	ax,[bp-14]
	mov	es:[bx+02],ax
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3678: 0800:3678
;;   Called from:
;;     0800:0E4C (in fn0800_0DE8)
;;     0800:1189 (in fn0800_112D)
;;     0800:1C2E (in fn0800_19EE)
fn0800_3678 proc
	push	bp
	mov	bp,sp
	sub	sp,08
	push	di
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	mov	[bp-02],ax
	mov	[bp-04],dx
	les	di,[bp+04]
	xor	ax,ax
	mov	cx,FFFF

l0800_3693:
	repne scasb

l0800_3695:
	not	cx
	mov	ax,005C
	sub	di,cx

l0800_369C:
	repne scasb

l0800_369E:
	jz	36A7

l0800_36A0:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_36A7:
	dec	di
	mov	ax,es
	mov	[bp-06],ax
	mov	[bp-08],di
	mov	dx,di
	or	dx,ax
	jz	3732

l0800_36B6:
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	cmp	ax,[bp+06]
	jnz	36C6

l0800_36C1:
	cmp	dx,[bp+04]
	jz	36D0

l0800_36C6:
	les	bx,[bp-08]
	cmp	byte ptr es:[bx-01],3A
	jnz	3732

l0800_36D0:
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	inc	dx
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	3732

l0800_36DF:
	les	bx,[bp-08]
	mov	byte ptr es:[bx],00
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	mov	ax,0010
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	call	33CD
	add	sp,0A
	or	ax,ax
	jz	371E

l0800_36FE:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8E52
	add	sp,04
	or	ax,ax
	jz	371E

l0800_370E:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	mov	ax,000E
	push	ax
	call	0D24
	add	sp,06

l0800_371E:
	les	bx,[bp-08]
	mov	byte ptr es:[bx],5C
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	inc	dx
	mov	[bp-02],ax
	mov	[bp-04],dx

l0800_3732:
	les	di,[bp-04]
	xor	ax,ax
	mov	cx,FFFF

l0800_373A:
	repne scasb

l0800_373C:
	not	cx
	mov	ax,005C
	sub	di,cx

l0800_3743:
	repne scasb

l0800_3745:
	jz	374E

l0800_3747:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_374E:
	dec	di
	mov	ax,es
	mov	[bp-06],ax
	mov	[bp-08],di
	mov	dx,di
	or	dx,ax
	jnz	36DF

l0800_375D:
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3764: 0800:3764
;;   Called from:
;;     0800:0E7C (in fn0800_0DE8)
;;     0800:1192 (in fn0800_112D)
fn0800_3764 proc
	push	si
	push	di
	cmp	byte ptr [427E],00
	jz	3796

l0800_376D:
	push	ds
	pop	es
	mov	di,44DC
	push	es
	push	di
	mov	di,427E
	xor	ax,ax
	mov	cx,FFFF

l0800_377C:
	repne scasb

l0800_377E:
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax

l0800_378D:
	rep movsw

l0800_378F:
	adc	cx,cx

l0800_3791:
	rep movsb

l0800_3793:
	pop	ds
	jmp	37A6

l0800_3796:
	push	ds
	mov	ax,4541
	push	ax
	push	ds
	mov	ax,44DC
	push	ax
	call	3509
	add	sp,08

l0800_37A6:
	push	ds
	pop	es
	mov	di,44DC
	mov	si,0A40
	mov	cx,FFFF
	xor	ax,ax

l0800_37B3:
	repne scasb

l0800_37B5:
	dec	di
	mov	cx,000D

l0800_37B9:
	rep movsb

l0800_37BB:
	pop	di
	pop	si
	ret

;; fn0800_37BE: 0800:37BE
;;   Called from:
;;     0800:0E5C (in fn0800_0DE8)
;;     0800:1583 (in fn0800_12E2)
;;     0800:1AA8 (in fn0800_19EE)
;;     0800:3BD3 (in fn0800_3BC3)
fn0800_37BE proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ds
	mov	ax,42E3
	push	ax
	push	ds
	mov	ax,0A2B
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	BEA2
	add	sp,10
	pop	bp
	ret

;; fn0800_37DF: 0800:37DF
;;   Called from:
;;     0800:10E5 (in fn0800_0DE8)
;;     0800:12B3 (in fn0800_112D)
fn0800_37DF proc
	push	bp
	mov	bp,sp
	sub	sp,6A
	push	di
	push	ds
	mov	ax,44DC
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	3509
	add	sp,08
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	pop	es
	lea	di,[bp-6A]
	xor	ax,ax
	mov	cx,FFFF

l0800_3805:
	repne scasb

l0800_3807:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_380D:
	repne scasb

l0800_380F:
	jz	3818

l0800_3811:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_3818:
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	35A3
	add	sp,08
	push	ds
	mov	ax,0A4D
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	AA7E
	add	sp,08
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,dx
	jz	3863

l0800_383D:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	A614
	add	sp,04
	mov	ax,0180
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	A4F6
	add	sp,06
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	8F7F
	add	sp,04

l0800_3863:
	push	ss
	lea	ax,[bp-6A]
	push	ax
	push	ds
	mov	ax,44DC
	push	ax
	call	BA4A
	add	sp,08
	cmp	ax,FFFF
	jnz	3887

l0800_3878:
	push	ds
	mov	ax,44DC
	push	ax
	mov	ax,000A
	push	ax
	call	0D24
	add	sp,06

l0800_3887:
	pop	di
	mov	sp,bp
	pop	bp
	ret

;; fn0800_388C: 0800:388C
;;   Called from:
;;     0800:0E7F (in fn0800_0DE8)
;;     0800:1195 (in fn0800_112D)
fn0800_388C proc
	push	bp
	mov	bp,sp
	sub	sp,08
	push	ds
	mov	ax,0A13
	push	ax
	push	ds
	mov	ax,4541
	push	ax
	call	4234
	add	sp,08
	mov	[29E5],dx
	mov	[29E3],ax
	push	ds
	mov	ax,0A50
	push	ax
	push	ds
	mov	ax,44DC
	push	ax
	call	4234
	add	sp,08
	mov	[29E1],dx
	mov	[29DF],ax
	push	ds
	mov	ax,44DC
	push	ax
	push	ss
	lea	ax,[bp-04]
	push	ax
	call	363D
	add	sp,08
	cmp	byte ptr [0A12],00
	jnz	38EF

l0800_38D7:
	mov	ax,[bp-02]
	mov	[4621],ax
	mov	ax,[bp-04]
	mov	[461F],ax
	mov	byte ptr [0A12],01
	mov	ax,0001
	mov	sp,bp
	pop	bp
	ret

l0800_38EF:
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp-08]
	push	ax
	call	363D
	add	sp,08
	push	ds
	mov	ax,461F
	push	ax
	push	ss
	lea	ax,[bp-08]
	push	ax
	call	395B
	add	sp,08
	or	ax,ax
	jl	3927

l0800_3913:
	push	ss
	lea	ax,[bp-04]
	push	ax
	push	ss
	lea	ax,[bp-08]
	push	ax
	call	395B
	add	sp,08
	or	ax,ax
	jle	392E

l0800_3927:
	mov	ax,0001
	mov	sp,bp
	pop	bp
	ret

l0800_392E:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	A614
	add	sp,04
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	A614
	add	sp,04
	push	ds
	mov	ax,44DC
	push	ax
	call	8F7F
	add	sp,04
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret

;; fn0800_395B: 0800:395B
;;   Called from:
;;     0800:3909 (in fn0800_388C)
;;     0800:391D (in fn0800_388C)
;;     0800:3AC0 (in fn0800_3992)
fn0800_395B proc
	push	bp
	mov	bp,sp
	sub	sp,02
	les	bx,[bp+04]
	mov	ax,es:[bx+02]
	mov	[bp-02],ax
	les	bx,[bp+08]
	cmp	ax,es:[bx+02]
	jnz	3984

l0800_3974:
	les	bx,[bp+04]
	mov	ax,es:[bx]
	les	bx,[bp+08]
	sub	ax,es:[bx]
	mov	sp,bp
	pop	bp
	ret

l0800_3984:
	mov	ax,[bp-02]
	les	bx,[bp+08]
	sub	ax,es:[bx+02]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3992: 0800:3992
;;   Called from:
;;     0800:0ECF (in fn0800_0DE8)
fn0800_3992 proc
	push	bp
	mov	bp,sp
	sub	sp,72
	push	di
	cmp	byte ptr [427E],00
	jz	39AE

l0800_39A0:
	cmp	word ptr [2A23],01
	jnz	39AE

l0800_39A7:
	cmp	word ptr [2A17],00
	jnz	39B5

l0800_39AE:
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_39B5:
	push	ds
	mov	ax,44DC
	push	ax
	push	ss
	lea	ax,[bp-72]
	push	ax
	call	3509
	add	sp,08
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	pop	es
	lea	di,[bp-72]
	xor	ax,ax
	mov	cx,FFFF

l0800_39D4:
	repne scasb

l0800_39D6:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_39DC:
	repne scasb

l0800_39DE:
	jz	39E7

l0800_39E0:
	mov	di,0001
	xor	ax,ax
	mov	es,ax

l0800_39E7:
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	35A3
	add	sp,08
	push	ds
	mov	ax,0A4D
	push	ax
	push	ss
	lea	ax,[bp-72]
	push	ax
	call	AA7E
	add	sp,08
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,dx
	jnz	3A13

l0800_3A0C:
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_3A13:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	4194
	add	sp,04
	or	dx,dx
	ja	3A3D

l0800_3A23:
	jnz	3A2A

l0800_3A25:
	cmp	ax,0012
	ja	3A3D

l0800_3A2A:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	A614
	add	sp,04
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_3A3D:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	3E5D
	add	sp,04
	mov	cl,08
	call	8CAA
	cmp	dx,52
	jnz	3A58

l0800_3A53:
	cmp	ax,4E43
	jz	3A6B

l0800_3A58:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	A614
	add	sp,04
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_3A6B:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	3E5D
	add	sp,04
	cmp	dx,[2A09]
	jnz	3A83

l0800_3A7D:
	cmp	ax,[2A07]
	jz	3A96

l0800_3A83:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	A614
	add	sp,04
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_3A96:
	push	ss
	lea	ax,[bp-72]
	push	ax
	push	ss
	lea	ax,[bp-0C]
	push	ax
	call	363D
	add	sp,08
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp-08]
	push	ax
	call	363D
	add	sp,08
	push	ss
	lea	ax,[bp-08]
	push	ax
	push	ss
	lea	ax,[bp-0C]
	push	ax
	call	395B
	add	sp,08
	or	ax,ax
	jge	3ADD

l0800_3ACA:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	A614
	add	sp,04
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_3ADD:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	3E5D
	add	sp,04
	add	ax,0012
	adc	dx,00
	mov	[2A05],dx
	mov	[2A03],ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	A614
	add	sp,04
	mov	ax,0001
	pop	di
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3B0A: 0800:3B0A
;;   Called from:
;;     0800:0FC1 (in fn0800_0DE8)
;;     0800:10AF (in fn0800_0DE8)
;;     0800:1836 (in fn0800_12E2)
;;     0800:3C31 (in fn0800_3BC3)
;;     0800:3C76 (in fn0800_3BC3)
;;     0800:3CFF (in fn0800_3C99)
;;     0800:3D54 (in fn0800_3C99)
;;     0800:4B15 (in fn0800_46FE)
;;     0800:4C0A (in fn0800_4BB1)
;;     0800:4FFD (in fn0800_4F2C)
;;     0800:50AB (in fn0800_4F2C)
;;     0800:52D0 (in fn0800_51A9)
;;     0800:5337 (in fn0800_51A9)
;;     0800:53D4 (in fn0800_5374)
;;     0800:6423 (in fn0800_5E64)
;;     0800:6869 (in fn0800_67BF)
;;     0800:7166 (in fn0800_6F20)
;;     0800:722E (in fn0800_6F20)
;;     0800:7542 (in fn0800_741D)
;;     0800:75BB (in fn0800_741D)
fn0800_3B0A proc
	push	bp
	mov	bp,sp
	sub	sp,08
	mov	ax,[bp+0C]
	or	ax,[bp+0E]
	jnz	3B1B

l0800_3B18:
	jmp	3BBF

l0800_3B1B:
	cmp	word ptr [bp+0E],00
	jl	3B31

l0800_3B21:
	jg	3B2A

l0800_3B23:
	cmp	word ptr [bp+0C],FDE8
	jbe	3B31

l0800_3B2A:
	xor	dx,dx
	mov	ax,FDE8
	jmp	3B37

l0800_3B31:
	mov	dx,[bp+0E]
	mov	ax,[bp+0C]

l0800_3B37:
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [bp-06]
	push	ax
	call	4311
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	jmp	3BAB

l0800_3B4F:
	cmp	word ptr [bp+0E],00
	jl	3B65

l0800_3B55:
	jg	3B5E

l0800_3B57:
	cmp	word ptr [bp+0C],FDE8
	jbe	3B65

l0800_3B5E:
	xor	dx,dx
	mov	ax,FDE8
	jmp	3B6B

l0800_3B65:
	mov	dx,[bp+0E]
	mov	ax,[bp+0C]

l0800_3B6B:
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [bp-06]
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	4110
	add	sp,0C
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	4152
	add	sp,0C
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	sub	[bp+0C],dx
	sbb	[bp+0E],ax

l0800_3BAB:
	mov	ax,[bp+0C]
	or	ax,[bp+0E]
	jnz	3B4F

l0800_3BB3:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	4346
	add	sp,04

l0800_3BBF:
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3BC3: 0800:3BC3
;;   Called from:
;;     0800:1EEF (in fn0800_1E5E)
fn0800_3BC3 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	ds
	mov	ax,0A54
	push	ax
	push	ds
	mov	ax,43AD
	push	ax
	call	37BE
	add	sp,08
	push	ds
	mov	ax,0A61
	push	ax
	push	ds
	mov	ax,43AD
	push	ax
	call	4234
	add	sp,08
	mov	[29D5],dx
	mov	[29D3],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	4194
	add	sp,04
	sub	ax,[bp+08]
	sbb	dx,[bp+0A]
	mov	[bp-02],dx
	mov	[bp-04],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29D5]
	push	word ptr [29D3]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3B0A
	add	sp,0C
	push	word ptr [29D5]
	push	word ptr [29D3]
	call	BA67
	add	sp,04
	xor	ax,ax
	push	ax
	mov	ax,[bp+0A]
	mov	dx,[bp+08]
	add	dx,[bp+0C]
	adc	ax,[bp+0E]
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [29D5]
	push	word ptr [29D3]
	call	3B0A
	add	sp,0C
	push	word ptr [29D5]
	push	word ptr [29D3]
	call	A614
	add	sp,04
	push	ds
	mov	ax,43AD
	push	ax
	call	8F7F
	add	sp,04
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3C99: 0800:3C99
;;   Called from:
;;     0800:1F13 (in fn0800_1E5E)
;;     0800:24D8 (in fn0800_23EC)
fn0800_3C99 proc
	push	bp
	mov	bp,sp
	sub	sp,6A
	push	si
	push	di
	push	ds
	mov	ax,4348
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	3509
	add	sp,08
	push	ss
	pop	es
	lea	di,[bp-6A]
	mov	si,0A54
	mov	cx,FFFF
	xor	ax,ax

l0800_3CBE:
	repne scasb

l0800_3CC0:
	dec	di
	mov	cx,000D

l0800_3CC4:
	rep movsb

l0800_3CC6:
	push	ds
	mov	ax,0A61
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	4234
	add	sp,08
	mov	[bp-02],dx
	mov	[bp-04],ax
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3B0A
	add	sp,0C
	xor	ax,ax
	push	ax
	mov	ax,[bp+0A]
	mov	dx,[bp+08]
	add	dx,[bp+0C]
	adc	ax,[bp+0E]
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	4194
	add	sp,04
	mov	bx,[bp+0A]
	mov	cx,[bp+08]
	add	cx,[bp+0C]
	adc	bx,[bp+0E]
	sub	ax,cx
	sbb	dx,bx
	mov	[bp+0E],dx
	mov	[bp+0C],ax
	push	word ptr [bp+0E]
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3B0A
	add	sp,0C
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	A614
	add	sp,04
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	A614
	add	sp,04
	mov	ax,0180
	push	ax
	push	ds
	mov	ax,4348
	push	ax
	call	A4F6
	add	sp,06
	push	ds
	mov	ax,4348
	push	ax
	call	8F7F
	add	sp,04
	push	ds
	mov	ax,4348
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	BA4A
	add	sp,08
	cmp	ax,FFFF
	jnz	3DB2

l0800_3DA3:
	push	ss
	lea	ax,[bp-6A]
	push	ax
	mov	ax,000A
	push	ax
	call	0D24
	add	sp,06

l0800_3DB2:
	push	ds
	mov	ax,0A65
	push	ax
	push	ds
	mov	ax,4348
	push	ax
	call	4234
	add	sp,08
	mov	[29D1],dx
	mov	[29CF],ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3DCF: 0800:3DCF
;;   Called from:
;;     0800:3E33 (in fn0800_3E27)
;;     0800:3E44 (in fn0800_3E27)
;;     0800:3F16 (in fn0800_3F0A)
;;     0800:3F27 (in fn0800_3F0A)
;;     0800:3FB9 (in fn0800_3FAD)
;;     0800:3FCA (in fn0800_3FAD)
;;     0800:49BD (in fn0800_46FE)
;;     0800:49EB (in fn0800_46FE)
;;     0800:51E5 (in fn0800_51A9)
;;     0800:52EC (in fn0800_51A9)
;;     0800:7459 (in fn0800_741D)
;;     0800:757A (in fn0800_741D)
;;     0800:8367 (in fn0800_8359)
fn0800_3DCF proc
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	les	bx,[bp+04]
	dec	word ptr es:[bx]
	jl	3DF3

l0800_3DDE:
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	mov	es,ax
	mov	al,es:[si]
	mov	ah,00
	jmp	3DFF

l0800_3DF3:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AEC2
	add	sp,04

l0800_3DFF:
	mov	[bp-02],ax
	cmp	ax,FFFF
	jnz	3E1F

l0800_3E07:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	4271
	add	sp,04
	push	dx
	push	ax
	mov	ax,0008
	push	ax
	call	0D24
	add	sp,06

l0800_3E1F:
	mov	al,[bp-02]
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3E27: 0800:3E27
;;   Called from:
;;     0800:1382 (in fn0800_12E2)
;;     0800:1DB2 (in fn0800_1CF6)
;;     0800:1DC2 (in fn0800_1CF6)
;;     0800:1ECB (in fn0800_1E5E)
;;     0800:3E69 (in fn0800_3E5D)
;;     0800:3E7D (in fn0800_3E5D)
;;     0800:3F64 (in fn0800_3F58)
;;     0800:3F78 (in fn0800_3F58)
;;     0800:4C44 (in fn0800_4BB1)
;;     0800:540C (in fn0800_5374)
;;     0800:541D (in fn0800_5374)
;;     0800:542E (in fn0800_5374)
;;     0800:5E07 (in fn0800_5DCE)
;;     0800:5E18 (in fn0800_5DCE)
;;     0800:6895 (in fn0800_67BF)
;;     0800:6A10 (in fn0800_67BF)
;;     0800:7AD3 (in fn0800_7A02)
;;     0800:7B67 (in fn0800_7A02)
;;     0800:7B78 (in fn0800_7A02)
;;     0800:7CB4 (in fn0800_7C78)
;;     0800:7CE1 (in fn0800_7C78)
;;     0800:7CF2 (in fn0800_7C78)
fn0800_3E27 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3DCF
	add	sp,04
	mov	ah,00
	mov	[bp-02],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3DCF
	add	sp,04
	mov	ah,00
	mov	[bp-04],ax
	mov	ax,[bp-02]
	mov	cl,08
	shl	ax,cl
	add	ax,[bp-04]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3E5D: 0800:3E5D
;;   Called from:
;;     0800:1D31 (in fn0800_1CF6)
;;     0800:1D8A (in fn0800_1CF6)
;;     0800:25E1 (in fn0800_24FE)
;;     0800:261E (in fn0800_24FE)
;;     0800:2633 (in fn0800_24FE)
;;     0800:265D (in fn0800_24FE)
;;     0800:3A43 (in fn0800_3992)
;;     0800:3A71 (in fn0800_3992)
;;     0800:3AE3 (in fn0800_3992)
;;     0800:4C63 (in fn0800_4C55)
;;     0800:4C96 (in fn0800_4C55)
;;     0800:4CCC (in fn0800_4C55)
;;     0800:4D04 (in fn0800_4C55)
;;     0800:4D20 (in fn0800_4C55)
;;     0800:4D34 (in fn0800_4C55)
;;     0800:4DCE (in fn0800_4C55)
;;     0800:4E0D (in fn0800_4C55)
;;     0800:4E49 (in fn0800_4C55)
;;     0800:4F3A (in fn0800_4F2C)
;;     0800:4F75 (in fn0800_4F2C)
;;     0800:4F89 (in fn0800_4F2C)
;;     0800:503C (in fn0800_4F2C)
;;     0800:506A (in fn0800_4F2C)
;;     0800:5205 (in fn0800_51A9)
;;     0800:5397 (in fn0800_5374)
;;     0800:53AB (in fn0800_5374)
;;     0800:53F7 (in fn0800_5374)
;;     0800:6A26 (in fn0800_67BF)
;;     0800:6A36 (in fn0800_67BF)
;;     0800:6A50 (in fn0800_67BF)
;;     0800:6B0C (in fn0800_6AD4)
;;     0800:6B40 (in fn0800_6AD4)
;;     0800:6B76 (in fn0800_6AD4)
;;     0800:6BAE (in fn0800_6AD4)
;;     0800:6BCA (in fn0800_6AD4)
;;     0800:6BDE (in fn0800_6AD4)
;;     0800:6C99 (in fn0800_6AD4)
;;     0800:6CEC (in fn0800_6AD4)
;;     0800:6F2E (in fn0800_6F20)
;;     0800:6F8E (in fn0800_6F20)
;;     0800:7061 (in fn0800_6F20)
;;     0800:71BF (in fn0800_6F20)
;;     0800:71ED (in fn0800_6F20)
;;     0800:7479 (in fn0800_741D)
fn0800_3E5D proc
	push	bp
	mov	bp,sp
	sub	sp,08
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3E27
	add	sp,04
	mov	word ptr [bp-02],0000
	mov	[bp-04],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3E27
	add	sp,04
	mov	word ptr [bp-06],0000
	mov	[bp-08],ax
	mov	dx,[bp-04]
	xor	ax,ax
	add	ax,[bp-08]
	adc	dx,00
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3E9A: 0800:3E9A
;;   Called from:
;;     0800:51BB (in fn0800_51A9)
;;     0800:742F (in fn0800_741D)
fn0800_3E9A proc
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	les	bx,[bp+04]
	dec	word ptr es:[bx]
	jl	3EBE

l0800_3EA9:
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	mov	es,ax
	mov	al,es:[si]
	mov	ah,00
	jmp	3ECA

l0800_3EBE:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AEC2
	add	sp,04

l0800_3ECA:
	mov	[bp-02],ax
	cmp	ax,FFFF
	jnz	3EEA

l0800_3ED2:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	4271
	add	sp,04
	push	dx
	push	ax
	mov	ax,0008
	push	ax
	call	0D24
	add	sp,06

l0800_3EEA:
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFFF
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	mov	al,[bp-02]
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3F0A: 0800:3F0A
;;   Called from:
;;     0800:0FD4 (in fn0800_0DE8)
;;     0800:1210 (in fn0800_112D)
;;     0800:682B (in fn0800_67BF)
fn0800_3F0A proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3DCF
	add	sp,04
	mov	ah,00
	mov	[bp-02],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3DCF
	add	sp,04
	mov	ah,00
	mov	[bp-04],ax
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFFE
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	mov	ax,[bp-02]
	mov	cl,08
	shl	ax,cl
	add	ax,[bp-04]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3F58: 0800:3F58
;;   Called from:
;;     0800:0F07 (in fn0800_0DE8)
;;     0800:11E7 (in fn0800_112D)
;;     0800:1733 (in fn0800_12E2)
;;     0800:17DD (in fn0800_12E2)
;;     0800:4783 (in fn0800_46FE)
;;     0800:4F9D (in fn0800_4F2C)
;;     0800:50C4 (in fn0800_4F2C)
;;     0800:5240 (in fn0800_51A9)
;;     0800:69D3 (in fn0800_67BF)
;;     0800:6FD4 (in fn0800_6F20)
;;     0800:7174 (in fn0800_6F20)
fn0800_3F58 proc
	push	bp
	mov	bp,sp
	sub	sp,08
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3E27
	add	sp,04
	mov	word ptr [bp-02],0000
	mov	[bp-04],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3E27
	add	sp,04
	mov	word ptr [bp-06],0000
	mov	[bp-08],ax
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFFC
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	mov	dx,[bp-04]
	xor	ax,ax
	add	ax,[bp-08]
	adc	dx,[bp-06]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3FAD: 0800:3FAD
;;   Called from:
;;     0800:473D (in fn0800_46FE)
;;     0800:47D3 (in fn0800_46FE)
;;     0800:47E3 (in fn0800_46FE)
;;     0800:4826 (in fn0800_46FE)
;;     0800:4834 (in fn0800_46FE)
;;     0800:4845 (in fn0800_46FE)
;;     0800:4856 (in fn0800_46FE)
;;     0800:4867 (in fn0800_46FE)
;;     0800:4878 (in fn0800_46FE)
;;     0800:4889 (in fn0800_46FE)
;;     0800:489A (in fn0800_46FE)
;;     0800:48AB (in fn0800_46FE)
;;     0800:49D3 (in fn0800_46FE)
;;     0800:521B (in fn0800_51A9)
;;     0800:5F28 (in fn0800_5E64)
;;     0800:5F39 (in fn0800_5E64)
;;     0800:5F86 (in fn0800_5E64)
;;     0800:5F97 (in fn0800_5E64)
;;     0800:5FA8 (in fn0800_5E64)
;;     0800:5FB9 (in fn0800_5E64)
;;     0800:5FCA (in fn0800_5E64)
;;     0800:5FDB (in fn0800_5E64)
;;     0800:5FEC (in fn0800_5E64)
;;     0800:5FFA (in fn0800_5E64)
;;     0800:600B (in fn0800_5E64)
;;     0800:601C (in fn0800_5E64)
;;     0800:602D (in fn0800_5E64)
;;     0800:609E (in fn0800_5E64)
;;     0800:60AF (in fn0800_5E64)
;;     0800:748F (in fn0800_741D)
fn0800_3FAD proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3DCF
	add	sp,04
	mov	ah,00
	mov	[bp-02],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3DCF
	add	sp,04
	mov	ah,00
	mov	[bp-04],ax
	mov	cl,08
	shl	ax,cl
	add	ax,[bp-02]
	mov	sp,bp
	pop	bp
	ret
0800:3FE0 55 8B EC 8B 46 04 0B 46 06 74 1B FF 76 0A FF 76 U...F..F.t..v..v
0800:3FF0 08 E8 DB FD 83 C4 04 C4 5E 04 FF 46 04 26 88 07 ........^..F.&..
0800:4000 0A C0 75 E7 EB 10 FF 76 0A FF 76 08 E8 C0 FD 83 ..u....v..v.....
0800:4010 C4 04 0A C0 75 F0 8B 56 06 8B 46 04 5D C3       ....u..V..F.]. 

;; fn0800_401E: 0800:401E
;;   Called from:
;;     0800:40A8 (in fn0800_409C)
;;     0800:40B7 (in fn0800_409C)
;;     0800:48C0 (in fn0800_46FE)
;;     0800:4939 (in fn0800_46FE)
;;     0800:498F (in fn0800_46FE)
;;     0800:4BED (in fn0800_4BB1)
;;     0800:4C4B (in fn0800_4BB1)
;;     0800:689C (in fn0800_67BF)
;;     0800:74A8 (in fn0800_741D)
;;     0800:77C2 (in fn0800_75EA)
;;     0800:77D3 (in fn0800_75EA)
;;     0800:77E4 (in fn0800_75EA)
;;     0800:7936 (in fn0800_75EA)
;;     0800:7948 (in fn0800_75EA)
;;     0800:8350 (in fn0800_831D)
fn0800_401E proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	mov	ax,[bp+04]
	mov	cl,08
	shr	ax,cl
	push	ax
	call	4047
	add	sp,06
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	mov	al,[bp+04]
	push	ax
	call	4047
	add	sp,06
	pop	bp
	ret

;; fn0800_4047: 0800:4047
;;   Called from:
;;     0800:402F (in fn0800_401E)
;;     0800:403F (in fn0800_401E)
;;     0800:40CE (in fn0800_40BF)
;;     0800:40E4 (in fn0800_40BF)
;;     0800:4A7B (in fn0800_46FE)
;;     0800:51EC (in fn0800_51A9)
;;     0800:52F3 (in fn0800_51A9)
;;     0800:612B (in fn0800_5E64)
;;     0800:61A5 (in fn0800_5E64)
;;     0800:61E7 (in fn0800_5E64)
;;     0800:622B (in fn0800_5E64)
;;     0800:6258 (in fn0800_5E64)
;;     0800:697A (in fn0800_67BF)
;;     0800:72B6 (in fn0800_6F20)
;;     0800:7460 (in fn0800_741D)
;;     0800:7581 (in fn0800_741D)
;;     0800:795A (in fn0800_75EA)
;;     0800:796C (in fn0800_75EA)
;;     0800:8648 (in fn0800_8624)
fn0800_4047 proc
	push	bp
	mov	bp,sp
	push	si
	mov	cl,[bp+04]
	les	bx,[bp+06]
	inc	word ptr es:[bx]
	jge	406F

l0800_4056:
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	mov	dl,cl
	mov	es,ax
	mov	es:[si],dl
	mov	al,dl
	mov	ah,00
	jmp	407C

l0800_406F:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	cx
	call	B30A
	add	sp,06

l0800_407C:
	cmp	ax,FFFF
	jnz	4099

l0800_4081:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	call	4271
	add	sp,04
	push	dx
	push	ax
	mov	ax,0009
	push	ax
	call	0D24
	add	sp,06

l0800_4099:
	pop	si
	pop	bp
	ret

;; fn0800_409C: 0800:409C
;;   Called from:
;;     0800:0F8D (in fn0800_0DE8)
;;     0800:0FA3 (in fn0800_0DE8)
;;     0800:1472 (in fn0800_12E2)
;;     0800:1802 (in fn0800_12E2)
;;     0800:1818 (in fn0800_12E2)
;;     0800:184F (in fn0800_12E2)
;;     0800:48D2 (in fn0800_46FE)
;;     0800:48E4 (in fn0800_46FE)
;;     0800:4C1C (in fn0800_4BB1)
;;     0800:4C2E (in fn0800_4BB1)
;;     0800:4C88 (in fn0800_4C55)
;;     0800:4CB4 (in fn0800_4C55)
;;     0800:4CD4 (in fn0800_4C55)
;;     0800:4D12 (in fn0800_4C55)
;;     0800:4D60 (in fn0800_4C55)
;;     0800:4D7C (in fn0800_4C55)
;;     0800:4DD6 (in fn0800_4C55)
;;     0800:4E61 (in fn0800_4C55)
;;     0800:4FCA (in fn0800_4F2C)
;;     0800:502E (in fn0800_4F2C)
;;     0800:5054 (in fn0800_4F2C)
;;     0800:5088 (in fn0800_4F2C)
;;     0800:50FB (in fn0800_4F2C)
;;     0800:513C (in fn0800_4F2C)
;;     0800:520D (in fn0800_51A9)
;;     0800:6901 (in fn0800_67BF)
;;     0800:6994 (in fn0800_67BF)
;;     0800:69EF (in fn0800_67BF)
;;     0800:6AC8 (in fn0800_67BF)
;;     0800:6B32 (in fn0800_6AD4)
;;     0800:6B5E (in fn0800_6AD4)
;;     0800:6B7E (in fn0800_6AD4)
;;     0800:6BBC (in fn0800_6AD4)
;;     0800:6C10 (in fn0800_6AD4)
;;     0800:6C2C (in fn0800_6AD4)
;;     0800:6C60 (in fn0800_6AD4)
;;     0800:6CA1 (in fn0800_6AD4)
;;     0800:7012 (in fn0800_6F20)
;;     0800:7090 (in fn0800_6F20)
;;     0800:70A4 (in fn0800_6F20)
;;     0800:70B8 (in fn0800_6F20)
;;     0800:71B1 (in fn0800_6F20)
;;     0800:71D7 (in fn0800_6F20)
;;     0800:720B (in fn0800_6F20)
;;     0800:7323 (in fn0800_6F20)
;;     0800:7381 (in fn0800_6F20)
;;     0800:73FE (in fn0800_73AC)
;;     0800:7481 (in fn0800_741D)
;;     0800:7789 (in fn0800_75EA)
;;     0800:779F (in fn0800_75EA)
;;     0800:77B1 (in fn0800_75EA)
;;     0800:7924 (in fn0800_75EA)
fn0800_409C proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	call	401E
	add	sp,06
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+04]
	call	401E
	add	sp,06
	pop	bp
	ret

;; fn0800_40BF: 0800:40BF
;;   Called from:
;;     0800:48F5 (in fn0800_46FE)
;;     0800:4906 (in fn0800_46FE)
;;     0800:4917 (in fn0800_46FE)
;;     0800:4928 (in fn0800_46FE)
;;     0800:494A (in fn0800_46FE)
;;     0800:495B (in fn0800_46FE)
;;     0800:496D (in fn0800_46FE)
;;     0800:497E (in fn0800_46FE)
;;     0800:49FE (in fn0800_46FE)
;;     0800:4A0F (in fn0800_46FE)
;;     0800:4B56 (in fn0800_46FE)
;;     0800:4B67 (in fn0800_46FE)
;;     0800:4B78 (in fn0800_46FE)
;;     0800:4B89 (in fn0800_46FE)
;;     0800:5232 (in fn0800_51A9)
;;     0800:5297 (in fn0800_51A9)
;;     0800:6145 (in fn0800_5E64)
;;     0800:62CC (in fn0800_5E64)
;;     0800:62DD (in fn0800_5E64)
;;     0800:62EE (in fn0800_5E64)
;;     0800:62FF (in fn0800_5E64)
;;     0800:6310 (in fn0800_5E64)
;;     0800:633E (in fn0800_5E64)
;;     0800:635D (in fn0800_5E64)
;;     0800:6467 (in fn0800_5E64)
;;     0800:6476 (in fn0800_5E64)
;;     0800:64A4 (in fn0800_5E64)
;;     0800:64B3 (in fn0800_5E64)
;;     0800:64EC (in fn0800_5E64)
;;     0800:6513 (in fn0800_5E64)
;;     0800:6524 (in fn0800_5E64)
;;     0800:65C8 (in fn0800_5E64)
;;     0800:65E7 (in fn0800_5E64)
;;     0800:6612 (in fn0800_5E64)
;;     0800:6646 (in fn0800_5E64)
;;     0800:74F4 (in fn0800_741D)
fn0800_40BF proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	mov	al,[bp+04]
	and	al,FF
	push	ax
	call	4047
	add	sp,06
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	mov	ax,[bp+04]
	mov	cl,08
	shr	ax,cl
	and	al,FF
	push	ax
	call	4047
	add	sp,06
	pop	bp
	ret
0800:40EC                                     55 8B EC FF             U...
0800:40F0 76 0A FF 76 08 C4 5E 04 26 8A 07 50 E8 48 FF 83 v..v..^.&..P.H..
0800:4100 C4 06 C4 5E 04 FF 46 04 26 80 3F 00 75 E1 5D C3 ...^..F.&.?.u.].

;; fn0800_4110: 0800:4110
;;   Called from:
;;     0800:1E25 (in fn0800_1CF6)
;;     0800:2D6B (in fn0800_2D0A)
;;     0800:3B81 (in fn0800_3B0A)
;;     0800:5B9E (in fn0800_5B15)
;;     0800:5BE4 (in fn0800_5B15)
;;     0800:80A8 (in fn0800_7FDC)
fn0800_4110 proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+0E]
	push	word ptr [bp+0C]
	push	word ptr [bp+08]
	mov	ax,0001
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ABA3
	add	sp,0C
	xor	dx,dx
	cmp	dx,[bp+0A]
	jnz	4138

l0800_4133:
	cmp	ax,[bp+08]
	jz	4150

l0800_4138:
	push	word ptr [bp+0E]
	push	word ptr [bp+0C]
	call	4271
	add	sp,04
	push	dx
	push	ax
	mov	ax,0008
	push	ax
	call	0D24
	add	sp,06

l0800_4150:
	pop	bp
	ret

;; fn0800_4152: 0800:4152
;;   Called from:
;;     0800:145C (in fn0800_12E2)
;;     0800:1F46 (in fn0800_1E5E)
;;     0800:3B99 (in fn0800_3B0A)
;;     0800:578F (in fn0800_55E8)
;;     0800:5936 (in fn0800_579B)
;;     0800:5D68 (in fn0800_5D2F)
;;     0800:6055 (in fn0800_5E64)
;;     0800:6737 (in fn0800_669C)
;;     0800:6803 (in fn0800_67BF)
;;     0800:6CD9 (in fn0800_6AD4)
;;     0800:73CE (in fn0800_73AC)
fn0800_4152 proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+0E]
	push	word ptr [bp+0C]
	push	word ptr [bp+08]
	mov	ax,0001
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AD85
	add	sp,0C
	xor	dx,dx
	cmp	dx,[bp+0A]
	jnz	417A

l0800_4175:
	cmp	ax,[bp+08]
	jz	4192

l0800_417A:
	push	word ptr [bp+0E]
	push	word ptr [bp+0C]
	call	4271
	add	sp,04
	push	dx
	push	ax
	mov	ax,0009
	push	ax
	call	0D24
	add	sp,06

l0800_4192:
	pop	bp
	ret

;; fn0800_4194: 0800:4194
;;   Called from:
;;     0800:0E9C (in fn0800_0DE8)
;;     0800:100F (in fn0800_0DE8)
;;     0800:11B2 (in fn0800_112D)
;;     0800:124B (in fn0800_112D)
;;     0800:1260 (in fn0800_112D)
;;     0800:160D (in fn0800_12E2)
;;     0800:16EB (in fn0800_12E2)
;;     0800:1D46 (in fn0800_1CF6)
;;     0800:3A19 (in fn0800_3992)
;;     0800:3BF6 (in fn0800_3BC3)
;;     0800:3D28 (in fn0800_3C99)
;;     0800:4AF7 (in fn0800_46FE)
;;     0800:4EF6 (in fn0800_4C55)
;;     0800:5309 (in fn0800_51A9)
;;     0800:5355 (in fn0800_51A9)
;;     0800:5B3F (in fn0800_5B15)
;;     0800:5DD8 (in fn0800_5DCE)
;;     0800:5DF4 (in fn0800_5DCE)
fn0800_4194 proc
	push	bp
	mov	bp,sp
	sub	sp,08
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AD2F
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	ax,0002
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AD2F
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	mov	dx,[bp-06]
	mov	ax,[bp-08]
	mov	sp,bp
	pop	bp
	ret
0800:41F1    55 8B EC 83 EC 08 1E B8 69 0A 50 FF 76 06 FF  U.......i.P.v..
0800:4200 76 04 E8 2F 00 83 C4 08 89 56 FE 89 46 FC FF 76 v../.....V..F..v
0800:4210 FE 50 E8 7F FF 83 C4 04 89 56 FA 89 46 F8 FF 76 .P.......V..F..v
0800:4220 FE FF 76 FC E8 ED 63 83 C4 04 8B 56 FA 8B 46 F8 ..v...c....V..F.
0800:4230 8B E5 5D C3                                     ..].           

;; fn0800_4234: 0800:4234
;;   Called from:
;;     0800:0E6C (in fn0800_0DE8)
;;     0800:13D9 (in fn0800_12E2)
;;     0800:1593 (in fn0800_12E2)
;;     0800:15AD (in fn0800_12E2)
;;     0800:1C6B (in fn0800_19EE)
;;     0800:2FE2 (in fn0800_2DE2)
;;     0800:389C (in fn0800_388C)
;;     0800:38B3 (in fn0800_388C)
;;     0800:3BE3 (in fn0800_3BC3)
;;     0800:3CD0 (in fn0800_3C99)
;;     0800:3DBC (in fn0800_3C99)
fn0800_4234 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AA7E
	add	sp,08
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,[bp-02]
	jnz	4267

l0800_4257:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	mov	ax,0007
	push	ax
	call	0D24
	add	sp,06

l0800_4267:
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_4271: 0800:4271
;;   Called from:
;;     0800:3E0D (in fn0800_3DCF)
;;     0800:3ED8 (in fn0800_3E9A)
;;     0800:4087 (in fn0800_4047)
;;     0800:413E (in fn0800_4110)
;;     0800:4180 (in fn0800_4152)
fn0800_4271 proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	cmp	ax,[29E5]
	jnz	428D

l0800_4280:
	cmp	dx,[29E3]
	jnz	428D

l0800_4286:
	mov	dx,ds
	mov	ax,4541
	pop	bp
	ret

l0800_428D:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	cmp	ax,[29E1]
	jnz	42A6

l0800_4299:
	cmp	dx,[29DF]
	jnz	42A6

l0800_429F:
	mov	dx,ds
	mov	ax,44DC
	pop	bp
	ret

l0800_42A6:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	cmp	ax,[29D1]
	jnz	42BF

l0800_42B2:
	cmp	dx,[29CF]
	jnz	42BF

l0800_42B8:
	mov	dx,ds
	mov	ax,4348
	pop	bp
	ret

l0800_42BF:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	cmp	ax,[29DD]
	jnz	42D8

l0800_42CB:
	cmp	dx,[29DB]
	jnz	42D8

l0800_42D1:
	mov	dx,ds
	mov	ax,4477
	pop	bp
	ret

l0800_42D8:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	cmp	ax,[29D9]
	jnz	42F1

l0800_42E4:
	cmp	dx,[29D7]
	jnz	42F1

l0800_42EA:
	mov	dx,ds
	mov	ax,4412
	pop	bp
	ret

l0800_42F1:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	cmp	ax,[29D5]
	jnz	430A

l0800_42FD:
	cmp	dx,[29D3]
	jnz	430A

l0800_4303:
	mov	dx,ds
	mov	ax,43AD
	pop	bp
	ret

l0800_430A:
	mov	dx,ds
	mov	ax,0A60
	pop	bp
	ret

;; fn0800_4311: 0800:4311
;;   Called from:
;;     0800:09DA (in fn0800_09A3)
;;     0800:1DD5 (in fn0800_1CF6)
;;     0800:1DEB (in fn0800_1CF6)
;;     0800:2D18 (in fn0800_2D0A)
;;     0800:3B41 (in fn0800_3B0A)
;;     0800:5460 (in fn0800_5374)
;;     0800:547F (in fn0800_5374)
;;     0800:606B (in fn0800_5E64)
;;     0800:76B4 (in fn0800_75EA)
;;     0800:76D3 (in fn0800_75EA)
;;     0800:76F2 (in fn0800_75EA)
;;     0800:7711 (in fn0800_75EA)
;;     0800:7730 (in fn0800_75EA)
fn0800_4311 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	9F89
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,dx
	jnz	433C

l0800_432D:
	push	ds
	mov	ax,0A6C
	push	ax
	mov	ax,0006
	push	ax
	call	0D24
	add	sp,06

l0800_433C:
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_4346: 0800:4346
;;   Called from:
;;     0800:0AB0 (in fn0800_09A3)
;;     0800:1F54 (in fn0800_1E5E)
;;     0800:2DB2 (in fn0800_2D0A)
;;     0800:3BB9 (in fn0800_3B0A)
;;     0800:5588 (in fn0800_5374)
;;     0800:5594 (in fn0800_5374)
;;     0800:621A (in fn0800_5E64)
;;     0800:79BC (in fn0800_75EA)
;;     0800:79C8 (in fn0800_75EA)
;;     0800:79D4 (in fn0800_75EA)
;;     0800:79E0 (in fn0800_75EA)
;;     0800:79EC (in fn0800_75EA)
fn0800_4346 proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	9E75
	add	sp,04
	pop	bp
	ret

;; fn0800_4357: 0800:4357
;;   Called from:
;;     0800:2A68 (in fn0800_29C5)
fn0800_4357 proc
	push	bp
	mov	bp,sp
	jmp	43B9

l0800_435C:
	les	bx,[bp+08]
	mov	al,es:[bx]
	mov	dl,al
	cbw
	cmp	ax,002A
	jz	4379

l0800_436A:
	cmp	ax,002E
	jz	439E

l0800_436F:
	cmp	ax,003F
	jz	438A

l0800_4374:
	jmp	43A7

l0800_4376:
	inc	word ptr [bp+04]

l0800_4379:
	les	bx,[bp+04]
	cmp	byte ptr es:[bx],2E
	jz	43B6

l0800_4382:
	cmp	byte ptr es:[bx],00
	jnz	4376

l0800_4388:
	jmp	43B6

l0800_438A:
	les	bx,[bp+04]
	cmp	byte ptr es:[bx],2E
	jz	43B6

l0800_4393:
	cmp	byte ptr es:[bx],00
	jz	43B6

l0800_4399:
	inc	word ptr [bp+04]
	jmp	43B6

l0800_439E:
	les	bx,[bp+04]
	cmp	byte ptr es:[bx],00
	jz	43B6

l0800_43A7:
	les	bx,[bp+04]
	cmp	dl,es:[bx]
	jz	43B3

l0800_43AF:
	xor	ax,ax
	pop	bp
	ret

l0800_43B3:
	inc	word ptr [bp+04]

l0800_43B6:
	inc	word ptr [bp+08]

l0800_43B9:
	les	bx,[bp+08]
	cmp	byte ptr es:[bx],00
	jnz	435C

l0800_43C2:
	les	bx,[bp+04]
	cmp	byte ptr es:[bx],00
	jnz	43D0

l0800_43CB:
	mov	ax,0001
	jmp	43D2

l0800_43D0:
	xor	ax,ax

l0800_43D2:
	pop	bp
	ret

;; fn0800_43D4: 0800:43D4
;;   Called from:
;;     0800:5A32 (in fn0800_5A24)
;;     0800:7A22 (in fn0800_7A02)
;;     0800:7A31 (in fn0800_7A02)
;;     0800:7A40 (in fn0800_7A02)
fn0800_43D4 proc
	push	bp
	mov	bp,sp
	push	si
	mov	cx,[bp+08]
	mov	si,[bp+04]
	mov	ax,cx
	mov	dx,000C
	imul	dx
	add	si,ax
	jmp	440F

l0800_43E9:
	mov	es,[bp+06]
	mov	word ptr es:[si+02],0000
	mov	word ptr es:[si],0000
	mov	word ptr es:[si+04],FFFF
	mov	word ptr es:[si+08],0000
	mov	word ptr es:[si+06],0000
	mov	word ptr es:[si+0A],0000

l0800_440F:
	sub	si,0C
	mov	ax,cx
	dec	cx
	or	ax,ax
	jnz	43E9

l0800_4419:
	pop	si
	pop	bp
	ret

;; fn0800_441C: 0800:441C
;;   Called from:
;;     0800:7A69 (in fn0800_7A02)
;;     0800:7A78 (in fn0800_7A02)
;;     0800:7A87 (in fn0800_7A02)
fn0800_441C proc
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	push	di
	xor	di,di
	mov	cx,di
	mov	si,[bp+04]
	mov	ax,cx
	mov	dx,000C
	imul	dx
	add	si,ax
	cmp	cx,[bp+08]
	jnc	4452

l0800_4439:
	mov	es,[bp+06]
	mov	ax,es:[si]
	or	ax,es:[si+02]
	jz	4449

l0800_4445:
	inc	di
	mov	[bp-02],cx

l0800_4449:
	add	si,0C
	inc	cx
	cmp	cx,[bp+08]
	jc	4439

l0800_4452:
	or	di,di
	jnz	4459

l0800_4456:
	jmp	454A

l0800_4459:
	cmp	di,01
	jz	4461

l0800_445E:
	jmp	4525

l0800_4461:
	mov	ax,[bp-02]
	mov	dx,000C
	imul	dx
	les	bx,[bp+04]
	add	bx,ax
	inc	word ptr es:[bx+0A]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_4478:
	mov	ax,[4658]
	mov	dx,000C
	imul	dx
	mov	cx,ax
	les	bx,[bp+04]
	add	bx,ax
	mov	ax,es:[bx+02]
	mov	dx,es:[bx]
	push	ax
	mov	ax,[465A]
	mov	bx,000C
	push	dx
	imul	bx
	mov	dx,ax
	mov	bx,[bp+04]
	add	bx,ax
	pop	ax
	add	es:[bx],ax
	pop	ax
	adc	es:[bx+02],ax
	mov	bx,[bp+04]
	add	bx,cx
	mov	word ptr es:[bx+02],0000
	mov	word ptr es:[bx],0000
	mov	bx,[bp+04]
	add	bx,dx
	inc	word ptr es:[bx+0A]
	jmp	44DD

l0800_44C3:
	mov	es,[bp+06]
	mov	ax,es:[si+04]
	mov	[465A],ax
	mov	dx,000C
	imul	dx
	mov	dx,ax
	mov	bx,[bp+04]
	add	bx,ax
	inc	word ptr es:[bx+0A]

l0800_44DD:
	les	bx,[bp+04]
	add	bx,dx
	mov	si,bx
	cmp	word ptr es:[bx+04],FF
	jnz	44C3

l0800_44EB:
	mov	ax,[4658]
	mov	es:[si+04],ax
	mov	bx,[bp+04]
	add	bx,cx
	inc	word ptr es:[bx+0A]
	jmp	4517

l0800_44FD:
	mov	es,[bp+06]
	mov	ax,es:[si+04]
	mov	[4658],ax
	mov	dx,000C
	imul	dx
	mov	cx,ax
	mov	bx,[bp+04]
	add	bx,ax
	inc	word ptr es:[bx+0A]

l0800_4517:
	les	bx,[bp+04]
	add	bx,cx
	mov	si,bx
	cmp	word ptr es:[bx+04],FF
	jnz	44FD

l0800_4525:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	463B
	add	sp,06
	or	ax,ax
	jz	453B

l0800_4538:
	jmp	4478

l0800_453B:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	4550
	add	sp,06

l0800_454A:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_4550: 0800:4550
;;   Called from:
;;     0800:4544 (in fn0800_441C)
;;     0800:5A83 (in fn0800_5A24)
fn0800_4550 proc
	push	bp
	mov	bp,sp
	sub	sp,0A
	push	si
	push	di
	mov	word ptr [bp-02],0000
	mov	word ptr [bp-04],0000
	mov	word ptr [bp-06],8000
	mov	word ptr [bp-08],0000
	mov	word ptr [bp-0A],0001
	jmp	45D6

l0800_4573:
	xor	di,di
	mov	si,[bp+04]
	cmp	di,[bp+08]
	jnc	45C3

l0800_457D:
	mov	es,[bp+06]
	mov	ax,es:[si+0A]
	cmp	ax,[bp-0A]
	jnz	45BA

l0800_4589:
	push	word ptr [bp-0A]
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	8BC2
	push	dx
	push	ax
	call	45E2
	add	sp,06
	mov	es,[bp+06]
	mov	es:[si+08],dx
	mov	es:[si+06],ax
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	add	[bp-04],dx
	adc	[bp-02],ax

l0800_45BA:
	add	si,0C
	inc	di
	cmp	di,[bp+08]
	jc	457D

l0800_45C3:
	inc	word ptr [bp-0A]
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	shr	ax,01
	rcr	dx,01
	mov	[bp-06],ax
	mov	[bp-08],dx

l0800_45D6:
	cmp	word ptr [bp-0A],10
	jbe	4573

l0800_45DC:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_45E2: 0800:45E2
;;   Called from:
;;     0800:459D (in fn0800_4550)
fn0800_45E2 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	mov	bx,[bp+08]
	mov	word ptr [bp-02],0000
	mov	word ptr [bp-04],0000
	jmp	462A

l0800_45F7:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	shl	dx,01
	rcl	ax,01
	mov	[bp-02],ax
	mov	[bp-04],dx
	mov	ax,[bp+04]
	and	ax,0001
	or	ax,0000
	jz	461A

l0800_4612:
	or	word ptr [bp-04],01
	or	word ptr [bp-02],00

l0800_461A:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	shr	ax,01
	rcr	dx,01
	mov	[bp+06],ax
	mov	[bp+04],dx

l0800_462A:
	mov	ax,bx
	dec	bx
	or	ax,ax
	jnz	45F7

l0800_4631:
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_463B: 0800:463B
;;   Called from:
;;     0800:452E (in fn0800_441C)
fn0800_463B proc
	push	bp
	mov	bp,sp
	sub	sp,0C
	push	si
	push	di
	mov	di,[bp+08]
	mov	ax,FFFF
	mov	dx,FFFF
	mov	[bp-0A],ax
	mov	[bp-0C],dx
	mov	[bp-06],ax
	mov	[bp-08],dx
	xor	cx,cx
	mov	si,[bp+04]
	cmp	cx,di
	jnc	46D5

l0800_4661:
	mov	es,[bp+06]
	mov	ax,es:[si+02]
	mov	dx,es:[si]
	mov	[bp-02],ax
	mov	[bp-04],dx
	or	dx,ax
	jz	46CD

l0800_4675:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	cmp	ax,[bp-06]
	ja	46AB

l0800_4680:
	jc	4687

l0800_4682:
	cmp	dx,[bp-08]
	jnc	46AB

l0800_4687:
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	mov	[bp-0A],ax
	mov	[bp-0C],dx
	mov	ax,[465A]
	mov	[4658],ax
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	mov	[bp-06],ax
	mov	[bp-08],dx
	mov	[465A],cx
	jmp	46CD

l0800_46AB:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	cmp	ax,[bp-0A]
	ja	46CD

l0800_46B6:
	jc	46BD

l0800_46B8:
	cmp	dx,[bp-0C]
	jnc	46CD

l0800_46BD:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	mov	[bp-0A],ax
	mov	[bp-0C],dx
	mov	[4658],cx

l0800_46CD:
	add	si,0C
	inc	cx
	cmp	cx,di
	jc	4661

l0800_46D5:
	cmp	word ptr [bp-06],FF
	jnz	46E1

l0800_46DB:
	cmp	word ptr [bp-08],FF
	jz	46ED

l0800_46E1:
	cmp	word ptr [bp-0A],FF
	jnz	46F5

l0800_46E7:
	cmp	word ptr [bp-0C],FF
	jnz	46F5

l0800_46ED:
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_46F5:
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_46FE: 0800:46FE
;;   Called from:
;;     0800:121B (in fn0800_112D)
fn0800_46FE proc
	push	bp
	mov	bp,sp
	sub	sp,20
	push	si
	push	di
	cmp	word ptr [2A05],00
	ja	471F

l0800_470D:
	jc	4716

l0800_470F:
	cmp	word ptr [2A03],2A
	jnc	471F

l0800_4716:
	mov	ax,0007
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_471F:
	xor	ax,ax
	push	ax
	mov	dx,0028
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	di,ax
	add	ax,0024
	xor	dx,dx
	cmp	dx,[2A05]
	jc	4761

l0800_4750:
	ja	4758

l0800_4752:
	cmp	ax,[2A03]
	jbe	4761

l0800_4758:
	mov	ax,0007
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_4761:
	xor	ax,ax
	push	ax
	mov	ax,di
	add	ax,0020
	xor	dx,dx
	push	dx
	push	ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F58
	add	sp,04
	mov	cl,08
	call	8CAA
	cmp	dx,52
	jnz	4798

l0800_4793:
	cmp	ax,4E43
	jz	47A1

l0800_4798:
	mov	ax,0007
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_47A1:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	xor	ax,ax
	push	ax
	mov	dx,0002
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	di,ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-0A],ax
	or	di,di
	jz	47F3

l0800_47F0:
	dec	word ptr [bp-0A]

l0800_47F3:
	mov	ax,[bp-0A]
	xor	dx,dx
	mov	cl,09
	call	8C69
	add	ax,di
	adc	dx,00
	mov	[bp-02],dx
	mov	[bp-04],ax
	xor	ax,ax
	push	ax
	mov	dx,0016
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-1C],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-1E],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-10],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-12],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-18],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-1A],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-16],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-14],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,4D5A
	push	ax
	call	401E
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-10]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-12]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-14]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-16]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	call	401E
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-18]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-1A]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,001E
	push	ax
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-1E]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	call	401E
	add	sp,06
	xor	ax,ax
	push	ax
	mov	ax,[bp-1C]
	add	ax,0020
	xor	dx,dx
	push	dx
	push	ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	mov	word ptr [bp-0C],0000

l0800_49B5:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3DCF
	add	sp,04
	mov	ah,00
	mov	di,ax
	or	di,di
	jz	4A22

l0800_49CB:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-20],ax
	xor	si,si
	mov	[bp-0A],di
	jmp	4A15

l0800_49E3:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3DCF
	add	sp,04
	mov	ah,00
	add	si,ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	si
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-20]
	call	40BF
	add	sp,06

l0800_4A15:
	mov	ax,[bp-0A]
	dec	word ptr [bp-0A]
	or	ax,ax
	jnz	49E3

l0800_4A1F:
	add	[bp-0C],di

l0800_4A22:
	or	di,di
	jnz	49B5

l0800_4A26:
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	or	dx,dx
	jg	4A56

l0800_4A38:
	jl	4A3F

l0800_4A3A:
	cmp	ax,0200
	jnc	4A56

l0800_4A3F:
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	dx,0200
	sub	dx,ax
	mov	di,dx
	jmp	4A81

l0800_4A56:
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	dx,0010
	sub	dx,ax
	and	dx,0F
	mov	di,dx
	jmp	4A81

l0800_4A70:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,00
	push	ax
	call	4047
	add	sp,06

l0800_4A81:
	mov	ax,di
	dec	di
	or	ax,ax
	jnz	4A70

l0800_4A88:
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	cl,04
	call	8C8A
	mov	[bp-0E],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	call	5374
	mov	dx,ax
	or	dx,dx
	jz	4AC4

l0800_4ABE:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_4AC4:
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	4194
	add	sp,04
	sub	ax,[bp-04]
	sbb	dx,[bp-02]
	push	dx
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	mov	di,[bp-08]
	and	di,01FF
	mov	dx,[bp-06]
	mov	ax,[bp-08]
	mov	cl,09
	call	8CAA
	mov	[bp-0A],ax
	or	di,di
	jz	4B37

l0800_4B34:
	inc	word ptr [bp-0A]

l0800_4B37:
	xor	ax,ax
	push	ax
	mov	dx,0002
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	di
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0A]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0C]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0E]
	call	40BF
	add	sp,06
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_4B97: 0800:4B97
;;   Called from:
;;     0800:1222 (in fn0800_112D)
fn0800_4B97 proc
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	5DCE
	add	sp,04
	or	ax,ax
	jnz	4BAD

l0800_4BA9:
	mov	ax,0007
	ret

l0800_4BAD:
	call	5374
	ret

;; fn0800_4BB1: 0800:4BB1
;;   Called from:
;;     0800:1229 (in fn0800_112D)
fn0800_4BB1 proc
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	5DCE
	add	sp,04
	or	ax,ax
	jnz	4BC7

l0800_4BC3:
	mov	ax,0007
	ret

l0800_4BC7:
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFEE
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,601A
	push	ax
	call	401E
	add	sp,06
	xor	ax,ax
	mov	dx,0010
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E27
	add	sp,04
	push	ax
	call	401E
	add	sp,06
	call	5374
	ret

;; fn0800_4C55: 0800:4C55
;;   Called from:
;;     0800:1230 (in fn0800_112D)
fn0800_4C55 proc
	push	bp
	mov	bp,sp
	sub	sp,18
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	or	dx,dx
	jnz	4C72

l0800_4C6D:
	cmp	ax,03F3
	jz	4C79

l0800_4C72:
	mov	ax,0007
	mov	sp,bp
	pop	bp
	ret

l0800_4C79:
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	mov	dx,03F3
	push	ax
	push	dx
	call	409C
	add	sp,08

l0800_4C8E:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	mov	[bp-02],dx
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-02]
	push	ax
	call	409C
	add	sp,08
	jmp	4CDA

l0800_4CBC:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	push	dx
	push	ax
	call	409C
	add	sp,08

l0800_4CDA:
	mov	ax,[bp-08]
	mov	dx,[bp-06]
	sub	word ptr [bp-08],01
	sbb	word ptr [bp-06],00
	or	ax,dx
	jnz	4CBC

l0800_4CEC:
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jnz	4C8E

l0800_4CF4:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	sub	ax,0001
	sbb	dx,00
	push	dx
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-0E],dx
	mov	[bp-10],ax
	mov	ax,[bp-0E]
	mov	dx,[bp-10]
	sub	dx,[bp-0C]
	sbb	ax,[bp-0A]
	mov	[bp-12],ax
	mov	[bp-14],dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0A]
	push	word ptr [bp-0C]
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,[bp-0E]
	mov	dx,[bp-10]
	sub	dx,01
	sbb	ax,0000
	push	ax
	push	dx
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	[4E8E],dx
	mov	[4E8C],ax
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,0004
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	mov	ax,[bp-12]
	mov	dx,[bp-14]
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	4DE4

l0800_4DBE:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	push	dx
	push	ax
	call	409C
	add	sp,08
	sub	word ptr [bp-04],01
	sbb	word ptr [bp-02],00

l0800_4DE4:
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jnz	4DBE

l0800_4DEC:
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,0004
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	add	ax,0001
	adc	dx,00
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	ax,0001
	push	ax
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	cl,02
	call	8C69
	push	dx
	push	ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	jmp	4EDE

l0800_4E41:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-06]
	push	ax
	call	409C
	add	sp,08
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	and	ax,3FFF
	mov	[bp-18],dx
	mov	[bp-16],ax
	mov	cx,0004
	mov	bx,4F14

l0800_4E7C:
	mov	ax,cs:[bx]
	cmp	ax,[bp-08]
	jnz	4E8D

l0800_4E84:
	mov	ax,cs:[bx+08]
	cmp	ax,[bp-16]
	jz	4E94

l0800_4E8D:
	add	bx,02
	loop	4E7C

l0800_4E92:
	jmp	4ED7

l0800_4E94:
	jmp	word ptr cs:[bx+10]
0800:4E98                         E8 91 00 8B D0 0B D2 74         .......t
0800:4EA0 3D 8B E5 5D C3 FF 36 E5 29 FF 36 E3 29 E8 AD EF =..]..6.).6.)...
0800:4EB0 83 C4 04 89 56 FE 89 46 FC FF 36 E1 29 FF 36 DF ....V..F..6.).6.
0800:4EC0 29 FF 76 FE 50 E8 D4 F1 83 C4 08 83 06 8C 4E 04 ).v.P.........N.
0800:4ED0 83 16 8E 4E 00 EB 07                            ...N...        

l0800_4ED7:
	mov	ax,0009
	mov	sp,bp
	pop	bp
	ret

l0800_4EDE:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	4194
	add	sp,04
	pop	bx
	cmp	bx,dx
	pop	dx
	jnc	4F05

l0800_4F02:
	jmp	4E41

l0800_4F05:
	jnz	4F0E

l0800_4F07:
	cmp	dx,ax
	jnc	4F0E

l0800_4F0B:
	jmp	4E41

l0800_4F0E:
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret
0800:4F14             E9 03 EA 03 EB 03 F2 03 00 00 00 00     ............
0800:4F20 00 00 00 00 98 4E 98 4E A5 4E DE 4E             .....N.N.N.N   

;; fn0800_4F2C: 0800:4F2C
fn0800_4F2C proc
	push	bp
	mov	bp,sp
	sub	sp,1A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	cl,02
	call	8C69
	mov	[bp-10],dx
	mov	[bp-12],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	mov	[bp-18],dx
	mov	[bp-1A],ax
	cmp	word ptr [bp-10],00
	jc	4F95

l0800_4F65:
	ja	4F6D

l0800_4F67:
	cmp	word ptr [bp-12],12
	jbe	4F95

l0800_4F6D:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-14],dx
	mov	[bp-16],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-0C],dx
	mov	[bp-0E],ax

l0800_4F95:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F58
	add	sp,04
	mov	cl,08
	call	8CAA
	cmp	dx,52
	jnz	4FB5

l0800_4FAD:
	cmp	ax,4E43
	jnz	4FB5

l0800_4FB2:
	jmp	50E6

l0800_4FB5:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	dx,[bp-10]
	mov	ax,[bp-12]
	mov	cl,02
	call	8CAA
	push	dx
	push	ax
	call	409C
	add	sp,08
	xor	ax,ax
	push	ax
	push	word ptr [bp-18]
	push	word ptr [bp-1A]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [bp-10]
	push	word ptr [bp-12]
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	jmp	50BC

l0800_5006:
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,0004
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	mov	dx,03EC
	push	ax
	push	dx
	call	409C
	add	sp,08

l0800_5034:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-04],dx
	mov	[bp-06],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-04]
	push	ax
	call	409C
	add	sp,08
	mov	ax,[bp-06]
	or	ax,[bp-04]
	jz	50B1

l0800_5062:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	sub	ax,0001
	sbb	dx,00
	mov	[bp-08],dx
	mov	[bp-0A],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-08]
	push	ax
	call	409C
	add	sp,08
	mov	dx,[bp-04]
	mov	ax,[bp-06]
	mov	cl,02
	call	8C69
	push	dx
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C

l0800_50B1:
	mov	ax,[bp-06]
	or	ax,[bp-04]
	jz	50BC

l0800_50B9:
	jmp	5034

l0800_50BC:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F58
	add	sp,04
	or	dx,dx
	jnz	50D6

l0800_50CE:
	cmp	ax,03EC
	jnz	50D6

l0800_50D3:
	jmp	5006

l0800_50D6:
	add	word ptr [4E8C],04
	adc	word ptr [4E8E],00
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret

l0800_50E6:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	dx,[bp-14]
	mov	ax,[bp-16]
	mov	cl,02
	call	8CAA
	push	dx
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	[bp-04],dx
	mov	[bp-06],ax
	xor	ax,ax
	push	ax
	push	word ptr [4E8E]
	push	word ptr [4E8C]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0C]
	push	word ptr [bp-0E]
	call	409C
	add	sp,08
	add	word ptr [4E8C],04
	adc	word ptr [4E8E],00
	xor	ax,ax
	push	ax
	push	word ptr [bp-04]
	push	word ptr [bp-06]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	call	5374
	mov	[bp-02],ax
	xor	ax,ax
	push	ax
	mov	ax,[bp-18]
	mov	dx,[bp-1A]
	add	dx,[bp-12]
	adc	ax,[bp-10]
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	mov	ax,[bp-02]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_518F: 0800:518F
;;   Called from:
;;     0800:1237 (in fn0800_112D)
fn0800_518F proc
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	5DCE
	add	sp,04
	or	ax,ax
	jnz	51A5

l0800_51A1:
	mov	ax,0007
	ret

l0800_51A5:
	call	5374
	ret

;; fn0800_51A9: 0800:51A9
;;   Called from:
;;     0800:123E (in fn0800_112D)
fn0800_51A9 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	jmp	533D

l0800_51B3:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E9A
	add	sp,04
	or	ax,ax
	jz	51D5

l0800_51C5:
	cmp	ax,0001
	jz	51F5

l0800_51CA:
	cmp	ax,0003
	jnz	51D2

l0800_51CF:
	jmp	52D8

l0800_51D2:
	jmp	5301

l0800_51D5:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3DCF
	add	sp,04
	push	ax
	call	4047
	add	sp,06
	jmp	533D

l0800_51F5:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	push	dx
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	word ptr [bp-02],0000
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	ax
	call	40BF
	add	sp,06
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F58
	add	sp,04
	mov	cl,08
	call	8CAA
	cmp	dx,52
	jnz	52BA

l0800_5250:
	cmp	ax,4E43
	jnz	52BA

l0800_5255:
	call	5374
	mov	si,ax
	or	si,si
	jz	5263

l0800_525E:
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_5263:
	mov	ax,0001
	push	ax
	mov	ax,[2A09]
	mov	dx,[2A07]
	add	dx,02
	adc	ax,0000
	neg	ax
	neg	dx
	sbb	ax,0000
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A07]
	call	40BF
	add	sp,06
	mov	ax,0001
	push	ax
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	jmp	533D

l0800_52BA:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	jmp	533D

l0800_52D8:
	xor	si,si
	jmp	52FA

l0800_52DC:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3DCF
	add	sp,04
	push	ax
	call	4047
	add	sp,06
	inc	si

l0800_52FA:
	cmp	si,03
	jnz	52DC

l0800_52FF:
	jmp	533D

l0800_5301:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	4194
	add	sp,04
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	pop	bx
	pop	cx
	sub	cx,ax
	sbb	bx,dx
	push	bx
	push	cx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C

l0800_533D:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	4194
	add	sp,04
	pop	bx
	cmp	bx,dx
	pop	dx
	jnc	5364

l0800_5361:
	jmp	51B3

l0800_5364:
	jnz	536D

l0800_5366:
	cmp	dx,ax
	jnc	536D

l0800_536A:
	jmp	51B3

l0800_536D:
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_5374: 0800:5374
;;   Called from:
;;     0800:11FC (in fn0800_112D)
;;     0800:1C86 (in fn0800_19EE)
;;     0800:4AB5 (in fn0800_46FE)
;;     0800:4BAD (in fn0800_4B97)
;;     0800:4C51 (in fn0800_4BB1)
;;     0800:5163 (in fn0800_4F2C)
;;     0800:51A5 (in fn0800_518F)
;;     0800:5255 (in fn0800_51A9)
fn0800_5374 proc
	push	bp
	mov	bp,sp
	sub	sp,0E
	push	si
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	and	ax,0003
	mov	[2A21],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[2A09],dx
	mov	[2A07],ax
	cmp	word ptr [2A21],00
	jnz	53EF

l0800_53BF:
	push	word ptr [2A09]
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	mov	ax,[2A09]
	mov	dx,[2A07]
	mov	[2A05],ax
	mov	[2A03],dx
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_53EF:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[2A05],dx
	mov	[2A03],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E27
	add	sp,04
	mov	[2E4B],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E27
	add	sp,04
	mov	[2E49],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E27
	add	sp,04
	push	word ptr [2A05]
	push	word ptr [2A03]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	2D0A
	add	sp,08
	cmp	ax,[2E49]
	jz	5458

l0800_5450:
	mov	ax,0005
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_5458:
	mov	ax,0001
	mov	dx,000F
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	inc	dx
	mov	[2E73],dx
	mov	word ptr [2E71],0000
	mov	ax,0001
	mov	dx,000F
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	inc	dx
	mov	[2E5B],dx
	mov	word ptr [2E59],0000
	mov	ax,[2E73]
	mov	dx,[2E71]
	add	dx,FD
	mov	[2E6F],ax
	mov	[2E6D],dx
	mov	ax,[2E5B]
	mov	dx,[2E59]
	add	dx,[2E31]
	mov	[2E57],ax
	mov	[2E55],dx
	mov	word ptr [2E4D],0000
	xor	si,si
	mov	word ptr [2E45],0000
	mov	word ptr [29FD],0000
	mov	word ptr [29FB],0000
	mov	word ptr [2A01],0000
	mov	word ptr [29FF],0000
	mov	ax,[2E4F]
	mov	[bp-0E],ax
	mov	ax,0001
	push	ax
	call	5C1A
	add	sp,02
	or	ax,ax
	jz	5504

l0800_54F3:
	cmp	word ptr [2A25],02
	jz	5504

l0800_54FA:
	cmp	word ptr [2A25],07
	jz	5504

l0800_5501:
	mov	si,000A

l0800_5504:
	or	si,si
	jnz	555D

l0800_5508:
	mov	ax,0001
	push	ax
	call	5C1A
	add	sp,02
	or	ax,ax
	jz	5557

l0800_5516:
	cmp	word ptr [2A23],01
	jz	554B

l0800_551D:
	mov	ax,0010
	push	ax
	call	5C1A
	add	sp,02
	mov	dx,ax
	cmp	word ptr [2A25],02
	jnz	553B

l0800_5530:
	cmp	word ptr [2E4F],00
	jnz	553B

l0800_5537:
	mov	[2E4F],dx

l0800_553B:
	cmp	[2E4F],dx
	jz	554B

l0800_5541:
	cmp	word ptr [2E4F],00
	jz	554B

l0800_5548:
	mov	si,000C

l0800_554B:
	cmp	word ptr [2E4F],00
	jnz	555D

l0800_5552:
	mov	si,000B
	jmp	555D

l0800_5557:
	mov	word ptr [2E4F],0000

l0800_555D:
	or	si,si
	jnz	557C

l0800_5561:
	mov	ax,[2A21]
	cmp	ax,0001
	jz	5570

l0800_5569:
	cmp	ax,0002
	jz	5577

l0800_556E:
	jmp	557C

l0800_5570:
	call	55E8
	mov	si,ax
	jmp	557C

l0800_5577:
	call	579B
	mov	si,ax

l0800_557C:
	mov	ax,[bp-0E]
	mov	[2E4F],ax
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	4346
	add	sp,04
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	4346
	add	sp,04
	add	word ptr [2A03],12
	adc	word ptr [2A05],00
	xor	ax,ax
	push	ax
	mov	ax,[bp-0A]
	mov	dx,[bp-0C]
	add	dx,[2A03]
	adc	ax,[2A05]
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	or	si,si
	jz	55D0

l0800_55C9:
	mov	ax,si
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_55D0:
	mov	ax,[2E4D]
	cmp	ax,[2E4B]
	jz	55E1

l0800_55D9:
	mov	ax,0006
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_55E1:
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_55E8: 0800:55E8
;;   Called from:
;;     0800:5570 (in fn0800_5374)
fn0800_55E8 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	jmp	574B

l0800_55F1:
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,290F
	push	ax
	call	5A24
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,284F
	push	ax
	call	5A24
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,278F
	push	ax
	call	5A24
	add	sp,06
	mov	ax,0010
	push	ax
	call	5C39
	add	sp,02
	mov	word ptr [bp-02],0000
	mov	[bp-04],ax
	jmp	5736

l0800_5633:
	push	ds
	mov	ax,290F
	push	ax
	call	5A8D
	add	sp,04
	mov	[2E47],ax
	add	[29FF],ax
	adc	word ptr [2A01],00
	cmp	word ptr [2E47],00
	jnz	5654

l0800_5651:
	jmp	56E6

l0800_5654:
	jmp	5664

l0800_5656:
	call	5B15
	xor	al,[2E4F]
	push	ax
	call	5D2F
	add	sp,02

l0800_5664:
	mov	ax,[2E47]
	dec	word ptr [2E47]
	or	ax,ax
	jnz	5656

l0800_566F:
	test	word ptr [2E4F],0001
	jz	5684

l0800_5677:
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	5688

l0800_5684:
	shr	word ptr [2E4F],01

l0800_5688:
	les	bx,[2E6D]
	mov	al,es:[bx+02]
	mov	ah,00
	push	ax
	xor	ax,ax
	mov	dl,es:[bx+01]
	mov	dh,00
	mov	cl,08
	shl	dx,cl
	add	ax,dx
	pop	dx
	adc	dx,00
	mov	bl,es:[bx]
	mov	bh,00
	push	ax
	mov	ax,bx
	push	dx
	cwd
	mov	bx,dx
	pop	dx
	mov	cx,ax
	pop	ax
	add	ax,cx
	adc	dx,bx
	mov	cl,[2E45]
	call	8C69
	push	ax
	mov	ax,0001
	mov	cl,[2E45]
	shl	ax,cl
	dec	ax
	push	dx
	cwd
	mov	bx,[29FD]
	mov	cx,[29FB]
	and	cx,ax
	and	bx,dx
	pop	ax
	pop	dx
	add	dx,cx
	adc	ax,bx
	mov	[29FD],ax
	mov	[29FB],dx

l0800_56E6:
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jz	574B

l0800_56EE:
	push	ds
	mov	ax,284F
	push	ax
	call	5A8D
	add	sp,04
	inc	ax
	mov	[2E2B],ax
	push	ds
	mov	ax,278F
	push	ax
	call	5A8D
	add	sp,04
	add	ax,0002
	mov	[2E29],ax
	add	[29FF],ax
	adc	word ptr [2A01],00
	jmp	572B

l0800_5719:
	les	bx,[2E55]
	sub	bx,[2E2B]
	mov	al,es:[bx]
	push	ax
	call	5D2F
	add	sp,02

l0800_572B:
	mov	ax,[2E29]
	dec	word ptr [2E29]
	or	ax,ax
	jnz	5719

l0800_5736:
	mov	ax,[bp-04]
	mov	dx,[bp-02]
	sub	word ptr [bp-04],01
	sbb	word ptr [bp-02],00
	or	ax,dx
	jz	574B

l0800_5748:
	jmp	5633

l0800_574B:
	mov	ax,[2A01]
	mov	dx,[29FF]
	cmp	ax,[2A09]
	jnc	575B

l0800_5758:
	jmp	55F1

l0800_575B:
	jnz	5766

l0800_575D:
	cmp	dx,[2A07]
	jnc	5766

l0800_5763:
	jmp	55F1

l0800_5766:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,[2E55]
	xor	dx,dx
	sub	ax,[2E59]
	sbb	dx,00
	sub	ax,[2E31]
	sbb	dx,00
	push	dx
	push	ax
	mov	ax,[2E59]
	add	ax,[2E31]
	push	word ptr [2E5B]
	push	ax
	call	4152
	add	sp,0C
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret

;; fn0800_579B: 0800:579B
;;   Called from:
;;     0800:5577 (in fn0800_5374)
fn0800_579B proc
	jmp	58F2

l0800_579E:
	call	5B15
	xor	al,[2E4F]
	push	ax
	call	5D2F
	add	sp,02
	test	word ptr [2E4F],0001
	jz	57C1

l0800_57B4:
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	57C5

l0800_57C1:
	shr	word ptr [2E4F],01

l0800_57C5:
	add	word ptr [29FF],01
	adc	word ptr [2A01],00

l0800_57CF:
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	or	ax,ax
	jz	579E

l0800_57DD:
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	or	ax,ax
	jz	5866

l0800_57EB:
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	or	ax,ax
	jnz	580A

l0800_57F9:
	mov	word ptr [2E29],0002
	call	5B15
	mov	ah,00
	inc	ax
	mov	[2E2B],ax
	jmp	5838

l0800_580A:
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	or	ax,ax
	jnz	5820

l0800_5818:
	mov	word ptr [2E29],0003
	jmp	5835

l0800_5820:
	call	5B15
	mov	ah,00
	add	ax,0008
	mov	[2E29],ax
	cmp	word ptr [2E29],08
	jnz	5835

l0800_5832:
	jmp	58E8

l0800_5835:
	call	5975

l0800_5838:
	mov	ax,[2E29]
	add	[29FF],ax
	adc	word ptr [2A01],00
	jmp	5858

l0800_5846:
	les	bx,[2E55]
	sub	bx,[2E2B]
	mov	al,es:[bx]
	push	ax
	call	5D2F
	add	sp,02

l0800_5858:
	mov	ax,[2E29]
	dec	word ptr [2E29]
	or	ax,ax
	jnz	5846

l0800_5863:
	jmp	57CF

l0800_5866:
	call	593F
	cmp	word ptr [2E29],09
	jnz	58B7

l0800_5870:
	call	5A0F
	mov	ax,[2E47]
	add	[29FF],ax
	adc	word ptr [2A01],00
	jmp	588F

l0800_5881:
	call	5B15
	xor	al,[2E4F]
	push	ax
	call	5D2F
	add	sp,02

l0800_588F:
	mov	ax,[2E47]
	dec	word ptr [2E47]
	or	ax,ax
	jnz	5881

l0800_589A:
	test	word ptr [2E4F],0001
	jz	58B0

l0800_58A2:
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	57CF

l0800_58B0:
	shr	word ptr [2E4F],01
	jmp	57CF

l0800_58B7:
	call	5975
	mov	ax,[2E29]
	add	[29FF],ax
	adc	word ptr [2A01],00
	jmp	58DA

l0800_58C8:
	les	bx,[2E55]
	sub	bx,[2E2B]
	mov	al,es:[bx]
	push	ax
	call	5D2F
	add	sp,02

l0800_58DA:
	mov	ax,[2E29]
	dec	word ptr [2E29]
	or	ax,ax
	jnz	58C8

l0800_58E5:
	jmp	57CF

l0800_58E8:
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02

l0800_58F2:
	mov	ax,[2A01]
	mov	dx,[29FF]
	cmp	ax,[2A09]
	jnc	5902

l0800_58FF:
	jmp	57CF

l0800_5902:
	jnz	590D

l0800_5904:
	cmp	dx,[2A07]
	jnc	590D

l0800_590A:
	jmp	57CF

l0800_590D:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,[2E55]
	xor	dx,dx
	sub	ax,[2E59]
	sbb	dx,00
	sub	ax,[2E31]
	sbb	dx,00
	push	dx
	push	ax
	mov	ax,[2E59]
	add	ax,[2E31]
	push	word ptr [2E5B]
	push	ax
	call	4152
	add	sp,0C
	xor	ax,ax
	ret

;; fn0800_593F: 0800:593F
;;   Called from:
;;     0800:5866 (in fn0800_579B)
fn0800_593F proc
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	add	ax,0004
	mov	[2E29],ax
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	or	ax,ax
	jz	5974

l0800_595D:
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	mov	dx,[2E29]
	dec	dx
	shl	dx,01
	add	dx,ax
	mov	[2E29],dx

l0800_5974:
	ret

;; fn0800_5975: 0800:5975
;;   Called from:
;;     0800:5835 (in fn0800_579B)
;;     0800:58B7 (in fn0800_579B)
fn0800_5975 proc
	mov	word ptr [2E2B],0000
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	or	ax,ax
	jz	59FA

l0800_5989:
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	mov	[2E2B],ax
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	or	ax,ax
	jz	59E3

l0800_59A4:
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	mov	dx,[2E2B]
	shl	dx,01
	add	dx,ax
	or	dx,04
	mov	[2E2B],dx
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	or	ax,ax
	jnz	59FA

l0800_59CB:
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	mov	dx,[2E2B]
	shl	dx,01
	add	dx,ax
	mov	[2E2B],dx
	jmp	59FA

l0800_59E3:
	cmp	word ptr [2E2B],00
	jnz	59FA

l0800_59EA:
	mov	ax,0001
	push	ax
	call	5CD9
	add	sp,02
	add	ax,0002
	mov	[2E2B],ax

l0800_59FA:
	call	5B15
	mov	ah,00
	mov	dx,[2E2B]
	mov	cl,08
	shl	dx,cl
	add	dx,ax
	inc	dx
	mov	[2E2B],dx
	ret

;; fn0800_5A0F: 0800:5A0F
;;   Called from:
;;     0800:5870 (in fn0800_579B)
fn0800_5A0F proc
	mov	ax,0004
	push	ax
	call	5CD9
	add	sp,02
	shl	ax,01
	shl	ax,01
	add	ax,000C
	mov	[2E47],ax
	ret

;; fn0800_5A24: 0800:5A24
;;   Called from:
;;     0800:55FA (in fn0800_55E8)
;;     0800:5609 (in fn0800_55E8)
;;     0800:5618 (in fn0800_55E8)
fn0800_5A24 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	43D4
	add	sp,06
	mov	ax,0005
	push	ax
	call	5C39
	add	sp,02
	mov	[bp+08],ax
	or	ax,ax
	jz	5A89

l0800_5A49:
	cmp	word ptr [bp+08],10
	jbe	5A54

l0800_5A4F:
	mov	word ptr [bp+08],0010

l0800_5A54:
	xor	di,di
	mov	si,[bp+04]
	add	si,0A
	cmp	di,[bp+08]
	jnc	5A7A

l0800_5A61:
	mov	ax,0004
	push	ax
	call	5C39
	add	sp,02
	mov	es,[bp+06]
	mov	es:[si],ax
	add	si,0C
	inc	di
	cmp	di,[bp+08]
	jc	5A61

l0800_5A7A:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	4550
	add	sp,06

l0800_5A89:
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_5A8D: 0800:5A8D
;;   Called from:
;;     0800:5638 (in fn0800_55E8)
;;     0800:56F3 (in fn0800_55E8)
;;     0800:5702 (in fn0800_55E8)
fn0800_5A8D proc
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	push	di
	mov	word ptr [bp-02],0000
	mov	di,[bp+04]
	jmp	5AA5

l0800_5A9F:
	add	di,0C
	inc	word ptr [bp-02]

l0800_5AA5:
	mov	es,[bp+06]
	cmp	word ptr es:[di+0A],00
	jz	5A9F

l0800_5AAF:
	mov	cl,es:[di+0A]
	mov	ax,0001
	shl	ax,cl
	dec	ax
	cwd
	mov	bx,[29FD]
	mov	cx,[29FB]
	and	cx,ax
	and	bx,dx
	cmp	bx,es:[di+08]
	jnz	5A9F

l0800_5ACC:
	cmp	cx,es:[di+06]
	jnz	5A9F

l0800_5AD2:
	mov	ax,[bp-02]
	mov	dx,000C
	imul	dx
	mov	bx,[bp+04]
	add	bx,ax
	push	word ptr es:[bx+0A]
	call	5C39
	add	sp,02
	cmp	word ptr [bp-02],02
	jnc	5AF8

l0800_5AEF:
	mov	ax,[bp-02]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_5AF8:
	mov	ax,[bp-02]
	dec	ax
	push	ax
	call	5C39
	add	sp,02
	mov	cl,[bp-02]
	dec	cl
	mov	dx,0001
	shl	dx,cl
	or	ax,dx
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_5B15: 0800:5B15
;;   Called from:
;;     0800:5656 (in fn0800_55E8)
;;     0800:579E (in fn0800_579B)
;;     0800:57FF (in fn0800_579B)
;;     0800:5820 (in fn0800_579B)
;;     0800:5881 (in fn0800_579B)
;;     0800:59FA (in fn0800_5975)
;;     0800:5C54 (in fn0800_5C39)
;;     0800:5C5C (in fn0800_5C39)
;;     0800:5CEC (in fn0800_5CD9)
fn0800_5B15 proc
	push	bp
	mov	bp,sp
	sub	sp,08
	mov	ax,[2E73]
	mov	dx,[2E71]
	add	dx,FD
	cmp	ax,[2E6F]
	jz	5B2E

l0800_5B2B:
	jmp	5C0B

l0800_5B2E:
	cmp	dx,[2E6D]
	jz	5B37

l0800_5B34:
	jmp	5C0B

l0800_5B37:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	4194
	add	sp,04
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	pop	bx
	pop	cx
	sub	cx,ax
	sbb	bx,dx
	mov	[bp-02],bx
	mov	[bp-04],cx
	cmp	word ptr [bp-02],00
	jc	5B76

l0800_5B67:
	ja	5B6F

l0800_5B69:
	cmp	word ptr [bp-04],FD
	jbe	5B76

l0800_5B6F:
	xor	dx,dx
	mov	ax,FFFD
	jmp	5B7C

l0800_5B76:
	mov	dx,[bp-02]
	mov	ax,[bp-04]

l0800_5B7C:
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	push	word ptr [bp-06]
	push	ax
	mov	ax,[2E73]
	mov	dx,[2E71]
	mov	[2E6F],ax
	mov	[2E6D],dx
	push	ax
	push	dx
	call	4110
	add	sp,0C
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	sub	[bp-04],dx
	mov	dx,[bp-04]
	sbb	[bp-02],ax
	mov	ax,[bp-02]
	or	ax,ax
	jc	5BCB

l0800_5BBA:
	ja	5BC1

l0800_5BBC:
	cmp	dx,02
	jbe	5BCB

l0800_5BC1:
	mov	word ptr [bp-02],0000
	mov	word ptr [bp-04],0002

l0800_5BCB:
	push	word ptr [29E5]
	push	word ptr [29E3]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	mov	ax,[2E71]
	add	ax,[bp-08]
	push	word ptr [2E73]
	push	ax
	call	4110
	add	sp,0C
	mov	ax,0001
	push	ax
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	neg	ax
	neg	dx
	sbb	ax,0000
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A

l0800_5C0B:
	les	bx,[2E6D]
	inc	word ptr [2E6D]
	mov	al,es:[bx]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_5C1A: 0800:5C1A
;;   Called from:
;;     0800:54E9 (in fn0800_5374)
;;     0800:550C (in fn0800_5374)
;;     0800:5521 (in fn0800_5374)
fn0800_5C1A proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+04]
	cmp	word ptr [2A21],02
	jnz	5C30

l0800_5C27:
	push	ax
	call	5CD9
	add	sp,02
	pop	bp
	ret

l0800_5C30:
	push	ax
	call	5C39
	add	sp,02
	pop	bp
	ret

;; fn0800_5C39: 0800:5C39
;;   Called from:
;;     0800:5622 (in fn0800_55E8)
;;     0800:5A3C (in fn0800_5A24)
;;     0800:5A65 (in fn0800_5A24)
;;     0800:5AE3 (in fn0800_5A8D)
;;     0800:5AFD (in fn0800_5A8D)
;;     0800:5C31 (in fn0800_5C1A)
fn0800_5C39 proc
	push	bp
	mov	bp,sp
	sub	sp,06
	push	si
	push	di
	mov	si,[bp+04]
	xor	di,di
	mov	word ptr [bp-06],0001
	jmp	5CCA

l0800_5C4D:
	cmp	word ptr [2E45],00
	jnz	5CA2

l0800_5C54:
	call	5B15
	mov	ah,00
	mov	[bp-02],ax
	call	5B15
	mov	ah,00
	mov	[bp-04],ax
	les	bx,[2E6D]
	mov	al,es:[bx+01]
	mov	ah,00
	xor	dx,dx
	mov	cl,18
	call	8C69
	les	bx,[2E6D]
	mov	bl,es:[bx]
	mov	bh,00
	add	ax,0000
	adc	dx,bx
	mov	bx,[bp-04]
	mov	cl,08
	shl	bx,cl
	add	ax,bx
	adc	dx,00
	add	ax,[bp-02]
	adc	dx,00
	mov	[29FD],dx
	mov	[29FB],ax
	mov	word ptr [2E45],0010

l0800_5CA2:
	mov	ax,[29FB]
	and	ax,0001
	or	ax,0000
	jz	5CB0

l0800_5CAD:
	or	di,[bp-06]

l0800_5CB0:
	mov	ax,[29FD]
	mov	dx,[29FB]
	shr	ax,01
	rcr	dx,01
	mov	[29FD],ax
	mov	[29FB],dx
	shl	word ptr [bp-06],01
	dec	word ptr [2E45]
	dec	si

l0800_5CCA:
	or	si,si
	jz	5CD1

l0800_5CCE:
	jmp	5C4D

l0800_5CD1:
	mov	ax,di
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_5CD9: 0800:5CD9
;;   Called from:
;;     0800:57D3 (in fn0800_579B)
;;     0800:57E1 (in fn0800_579B)
;;     0800:57EF (in fn0800_579B)
;;     0800:580E (in fn0800_579B)
;;     0800:58EC (in fn0800_579B)
;;     0800:5943 (in fn0800_593F)
;;     0800:5953 (in fn0800_593F)
;;     0800:5961 (in fn0800_593F)
;;     0800:597F (in fn0800_5975)
;;     0800:598D (in fn0800_5975)
;;     0800:599A (in fn0800_5975)
;;     0800:59A8 (in fn0800_5975)
;;     0800:59C1 (in fn0800_5975)
;;     0800:59CF (in fn0800_5975)
;;     0800:59EE (in fn0800_5975)
;;     0800:5A13 (in fn0800_5A0F)
;;     0800:5C28 (in fn0800_5C1A)
fn0800_5CD9 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	di,[bp+04]
	xor	si,si
	jmp	5D25

l0800_5CE5:
	cmp	word ptr [2E45],00
	jnz	5D00

l0800_5CEC:
	call	5B15
	mov	ah,00
	mov	word ptr [29FD],0000
	mov	[29FB],ax
	mov	word ptr [2E45],0008

l0800_5D00:
	shl	si,01
	mov	ax,[29FB]
	and	ax,0080
	or	ax,0000
	jz	5D0E

l0800_5D0D:
	inc	si

l0800_5D0E:
	mov	ax,[29FD]
	mov	dx,[29FB]
	shl	dx,01
	rcl	ax,01
	mov	[29FD],ax
	mov	[29FB],dx
	dec	word ptr [2E45]
	dec	di

l0800_5D25:
	or	di,di
	jnz	5CE5

l0800_5D29:
	mov	ax,si
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_5D2F: 0800:5D2F
;;   Called from:
;;     0800:565E (in fn0800_55E8)
;;     0800:5725 (in fn0800_55E8)
;;     0800:57A6 (in fn0800_579B)
;;     0800:5852 (in fn0800_579B)
;;     0800:5889 (in fn0800_579B)
;;     0800:58D4 (in fn0800_579B)
fn0800_5D2F proc
	push	bp
	mov	bp,sp
	mov	ax,[2E5B]
	mov	dx,[2E59]
	dec	dx
	cmp	ax,[2E57]
	jnz	5D9E

l0800_5D40:
	cmp	dx,[2E55]
	jnz	5D9E

l0800_5D46:
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	mov	dx,FFFF
	sub	dx,[2E31]
	sbb	ax,0000
	push	ax
	push	dx
	mov	ax,[2E59]
	add	ax,[2E31]
	push	word ptr [2E5B]
	push	ax
	call	4152
	add	sp,0C
	push	word ptr [2E31]
	mov	ax,[2E55]
	sub	ax,[2E31]
	push	word ptr [2E57]
	push	ax
	push	word ptr [2E5B]
	push	word ptr [2E59]
	call	B0F3
	add	sp,0A
	mov	ax,[2E5B]
	mov	dx,[2E59]
	add	dx,[2E31]
	mov	[2E57],ax
	mov	[2E55],dx

l0800_5D9E:
	les	bx,[2E55]
	mov	al,[bp+04]
	mov	es:[bx],al
	inc	word ptr [2E55]
	mov	al,[2E4D]
	xor	al,[bp+04]
	mov	ah,00
	and	ax,00FF
	shl	ax,01
	mov	bx,ax
	mov	ax,[bx+2A29]
	mov	dx,[2E4D]
	mov	cl,08
	shr	dx,cl
	xor	ax,dx
	mov	[2E4D],ax
	pop	bp
	ret

;; fn0800_5DCE: 0800:5DCE
;;   Called from:
;;     0800:4B9F (in fn0800_4B97)
;;     0800:4BB9 (in fn0800_4BB1)
;;     0800:5197 (in fn0800_518F)
fn0800_5DCE proc
	push	bp
	mov	bp,sp
	push	si
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	4194
	add	sp,04
	or	dx,dx
	jc	5DEE

l0800_5DE2:
	jnz	5DE9

l0800_5DE4:
	cmp	ax,0400
	jc	5DEE

l0800_5DE9:
	mov	ax,0400
	jmp	5DFD

l0800_5DEE:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	4194
	add	sp,04
	and	ax,FFFC

l0800_5DFD:
	mov	si,ax
	jmp	5E5B

l0800_5E01:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3E27
	add	sp,04
	cmp	ax,524E
	jnz	5E58

l0800_5E12:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	3E27
	add	sp,04
	and	ax,FF00
	cmp	ax,4300
	jnz	5E40

l0800_5E26:
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFFC
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	jmp	5E5F

l0800_5E40:
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFFE
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A

l0800_5E58:
	sub	si,02

l0800_5E5B:
	or	si,si
	jnz	5E01

l0800_5E5F:
	mov	ax,si
	pop	si
	pop	bp
	ret

;; fn0800_5E64: 0800:5E64
;;   Called from:
;;     0800:0FDF (in fn0800_0DE8)
fn0800_5E64 proc
	push	bp
	mov	bp,sp
	sub	sp,38
	push	si
	push	di
	push	ds
	pop	es
	mov	di,2202
	mov	si,4271
	mov	cx,0004
	mov	dx,0003
	cmp	dx,cx
	jnc	5E80

l0800_5E7E:
	mov	cx,dx

l0800_5E80:
	xor	ax,ax

l0800_5E82:
	rep cmpsb

l0800_5E84:
	jz	5E8B

l0800_5E86:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_5E8B:
	or	ax,ax
	jz	5EC3

l0800_5E8F:
	push	ds
	pop	es
	mov	di,4271
	mov	si,2202
	mov	dx,0003
	mov	cx,0004
	sub	dx,cx
	jnc	5EA5

l0800_5EA1:
	add	cx,dx
	xor	dx,dx

l0800_5EA5:
	shr	cx,01

l0800_5EA7:
	rep movsw

l0800_5EA9:
	adc	cx,cx

l0800_5EAB:
	rep movsb

l0800_5EAD:
	mov	cx,dx
	xor	ax,ax

l0800_5EB1:
	rep stosb

l0800_5EB3:
	push	ds
	mov	ax,2E75
	push	ax
	push	ds
	mov	ax,4271
	push	ax
	call	09A3
	add	sp,08

l0800_5EC3:
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	xor	dx,dx
	cmp	dx,[2A09]
	jc	5EED

l0800_5EDC:
	jnz	5EE4

l0800_5EDE:
	cmp	ax,[2A07]
	jc	5EED

l0800_5EE4:
	mov	ax,0003
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_5EED:
	cmp	word ptr [2A0D],00
	jc	5F0A

l0800_5EF4:
	ja	5EFE

l0800_5EF6:
	cmp	word ptr [2A0B],7FF0
	jbe	5F0A

l0800_5EFE:
	mov	word ptr [2A0D],0000
	mov	word ptr [2A0B],7FF0

l0800_5F0A:
	xor	ax,ax
	push	ax
	mov	dx,0002
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-22],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	di,ax
	cmp	word ptr [bp-22],00
	jz	5F48

l0800_5F47:
	dec	di

l0800_5F48:
	mov	ax,di
	xor	dx,dx
	mov	cl,09
	call	8C69
	add	ax,[bp-22]
	adc	dx,00
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	cmp	ax,[2A09]
	jc	5F7E

l0800_5F69:
	ja	5F71

l0800_5F6B:
	cmp	dx,[2A07]
	jbe	5F7E

l0800_5F71:
	mov	ax,[2A09]
	mov	dx,[2A07]
	mov	[bp-02],ax
	mov	[bp-04],dx

l0800_5F7E:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-24],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-26],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-28],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-2A],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-2C],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-2E],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-30],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-32],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-34],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	xor	dx,dx
	push	dx
	push	ax
	push	ds
	mov	ax,2E77
	push	ax
	call	4152
	add	sp,0C
	cmp	word ptr [bp-24],00
	jnz	6064

l0800_6061:
	jmp	6220

l0800_6064:
	xor	ax,ax
	mov	dx,FFFF
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp-34]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	xor	si,si
	mov	ax,[bp-0C]
	mov	[bp-36],ax
	jmp	60D6

l0800_6096:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	[bp-22],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	di,ax
	xor	dx,dx
	mov	cl,04
	call	8C69
	add	ax,[bp-22]
	adc	dx,00
	mov	es,[bp-0A]
	mov	bx,[bp-36]
	mov	es:[bx+02],dx
	mov	es:[bx],ax
	add	word ptr [bp-36],04
	inc	si

l0800_60D6:
	cmp	si,[bp-24]
	jnz	6096

l0800_60DB:
	mov	ax,667B
	push	ax
	mov	ax,0004
	push	ax
	push	word ptr [bp-24]
	push	word ptr [bp-0A]
	push	word ptr [bp-0C]
	call	B95E
	add	sp,0A
	xor	si,si
	mov	di,[bp-0C]
	xor	ax,ax
	adc	ax,0000
	neg	ax
	mov	[bp-38],ax
	jmp	620C

l0800_6104:
	mov	es,[bp-0A]
	mov	ax,es:[di+02]
	mov	dx,es:[di]
	mov	[bp-12],ax
	mov	[bp-14],dx
	and	dx,F0
	and	ax,000F
	mov	[bp-0E],ax
	mov	[bp-10],dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,00
	push	ax
	call	4047
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	dx,[bp-0E]
	mov	ax,[bp-10]
	mov	cl,04
	call	8CAA
	push	ax
	call	40BF
	add	sp,06
	mov	word ptr [bp-22],0000

l0800_6150:
	mov	es,[bp-0A]
	mov	ax,es:[di+02]
	mov	dx,es:[di]
	sub	dx,[bp-10]
	sbb	ax,[bp-0E]
	mov	[bp-16],ax
	mov	[bp-18],dx
	add	[bp-10],dx
	adc	[bp-0E],ax
	cmp	word ptr [bp-16],00
	ja	61B9

l0800_6172:
	jnz	617B

l0800_6174:
	cmp	word ptr [bp-18],00FF
	ja	61B9

l0800_617B:
	mov	ax,[bp-0E]
	mov	dx,[bp-10]
	sub	dx,[bp-14]
	sbb	ax,[bp-12]
	or	ax,ax
	ja	61B9

l0800_618B:
	jc	6192

l0800_618D:
	cmp	dx,F0
	jnc	61B9

l0800_6192:
	cmp	word ptr [bp-22],00FF
	jz	61B9

l0800_6199:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[bp-18]
	push	ax
	call	4047
	add	sp,06
	inc	word ptr [bp-22]
	add	di,04
	inc	si
	mov	ax,si
	cmp	ax,[bp-24]
	jnz	6150

l0800_61B9:
	mov	ax,0001
	push	ax
	mov	ax,[bp-22]
	add	ax,0003
	mov	dx,[bp-38]
	neg	ax
	sbb	dx,00
	push	dx
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[bp-22]
	push	ax
	call	4047
	add	sp,06
	mov	ax,0001
	push	ax
	mov	ax,[bp-22]
	xor	dx,dx
	add	ax,0002
	adc	dx,00
	push	dx
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A

l0800_620C:
	cmp	si,[bp-24]
	jz	6214

l0800_6211:
	jmp	6104

l0800_6214:
	push	word ptr [bp-0A]
	push	word ptr [bp-0C]
	call	4346
	add	sp,04

l0800_6220:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,00
	push	ax
	call	4047
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	add	ax,FFE0
	adc	dx,FF
	mov	[bp-1A],dx
	mov	[bp-1C],ax
	jmp	6266

l0800_624D:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,00
	push	ax
	call	4047
	add	sp,06
	add	word ptr [bp-1C],01
	adc	word ptr [bp-1A],00

l0800_6266:
	mov	ax,[bp-1C]
	and	ax,000F
	or	ax,0000
	jnz	624D

l0800_6271:
	mov	ax,[bp-26]
	mov	cl,04
	shl	ax,cl
	sub	[bp-04],ax
	sbb	word ptr [bp-02],00
	xor	ax,ax
	push	ax
	mov	ax,[bp-26]
	shl	ax,cl
	xor	dx,dx
	push	dx
	push	ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	75EA
	add	sp,08
	xor	ax,ax
	push	ax
	mov	dx,0020
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-30]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-32]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-2E]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-2C]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-1C]
	call	40BF
	add	sp,06
	mov	dx,[2A05]
	mov	ax,[2A03]
	mov	cl,04
	call	8CAA
	mov	[bp-22],ax
	mov	ax,[2A03]
	and	ax,000F
	or	ax,0000
	jz	6333

l0800_6330:
	inc	word ptr [bp-22]

l0800_6333:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-22]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	dx,[bp-1A]
	mov	ax,[bp-1C]
	mov	cl,04
	call	8CAA
	mov	dx,[bp-22]
	add	dx,ax
	push	dx
	call	40BF
	add	sp,06
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	add	dx,[4E88]
	adc	ax,[4E8A]
	add	dx,0080
	adc	ax,0000
	mov	[bp-06],ax
	mov	[bp-08],dx
	cmp	word ptr [2A1D],00
	jz	6398

l0800_638D:
	add	word ptr [bp-08],0200
	adc	word ptr [bp-06],00
	jmp	63A8

l0800_6398:
	cmp	word ptr [2A21],01
	jnz	63A8

l0800_639F:
	add	word ptr [bp-08],0180
	adc	word ptr [bp-06],00

l0800_63A8:
	xor	ax,ax
	push	ax
	mov	ax,[bp-26]
	mov	cl,04
	shl	ax,cl
	mov	dx,[bp-02]
	mov	bx,[bp-04]
	add	bx,ax
	adc	dx,00
	push	dx
	push	bx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	mov	ax,0002
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	[bp-1E],dx
	mov	[bp-20],ax
	mov	ax,[bp-26]
	mov	cl,04
	shl	ax,cl
	mov	dx,[2A09]
	mov	bx,[2A07]
	sub	bx,[bp-04]
	sbb	dx,[bp-02]
	sub	bx,ax
	sbb	dx,00
	push	dx
	push	bx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	mov	ax,[bp-20]
	and	ax,01FF
	mov	[bp-22],ax
	mov	dx,[bp-1E]
	mov	ax,[bp-20]
	mov	cl,09
	call	8CAA
	mov	di,ax
	cmp	word ptr [bp-22],00
	jz	6446

l0800_6445:
	inc	di

l0800_6446:
	xor	ax,ax
	push	ax
	mov	dx,0002
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-22]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	di
	call	40BF
	add	sp,06
	cmp	word ptr [2A1D],00
	jz	64B9

l0800_6483:
	xor	ax,ax
	push	ax
	mov	dx,002E
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-22]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	di
	call	40BF
	add	sp,06

l0800_64B9:
	xor	ax,ax
	push	ax
	mov	dx,0018
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	sub	ax,0020
	push	ax
	call	40BF
	add	sp,06
	xor	ax,ax
	push	ax
	mov	dx,001C
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-28]
	call	40BF
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-2A]
	call	40BF
	add	sp,06
	xor	ax,ax
	push	ax
	mov	dx,000A
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	mov	ax,[bp-28]
	mov	cl,04
	shl	ax,cl
	mov	dx,[bp-02]
	mov	bx,[bp-04]
	add	bx,ax
	adc	dx,00
	mov	ax,[bp-1E]
	mov	cx,[bp-20]
	sub	cx,20
	sbb	ax,0000
	add	cx,[bp-08]
	adc	ax,[bp-06]
	cmp	dx,ax
	jc	659C

l0800_6568:
	ja	656E

l0800_656A:
	cmp	bx,cx
	jbe	659C

l0800_656E:
	mov	ax,[bp-28]
	mov	cl,04
	shl	ax,cl
	mov	dx,[bp-02]
	mov	bx,[bp-04]
	add	bx,ax
	adc	dx,00
	mov	ax,[bp-1E]
	mov	cx,[bp-20]
	sub	cx,20
	sbb	ax,0000
	add	cx,[bp-08]
	adc	ax,[bp-06]
	sub	bx,cx
	sbb	dx,ax
	add	[bp-08],bx
	adc	[bp-06],dx

l0800_659C:
	xor	ax,ax
	mov	dx,0010
	sub	dx,[bp-08]
	sbb	ax,[bp-06]
	and	dx,0F
	add	[bp-08],dx
	adc	word ptr [bp-06],00
	mov	dx,[bp-06]
	mov	ax,[bp-08]
	mov	cl,04
	call	8CAA
	mov	[bp-28],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	ax
	call	40BF
	add	sp,06
	mov	ax,[bp-2A]
	cmp	ax,[bp-28]
	jnc	65DC

l0800_65D6:
	mov	ax,[bp-28]
	mov	[bp-2A],ax

l0800_65DC:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-2A]
	call	40BF
	add	sp,06
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	add	ax,[4E88]
	adc	dx,[4E8A]
	add	ax,0020
	adc	dx,00
	mov	cl,04
	call	8CAA
	mov	[bp-2C],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	ax
	call	40BF
	add	sp,06
	mov	ax,[bp-1C]
	add	ax,0080
	mov	[bp-2E],ax
	cmp	word ptr [2A1D],00
	jz	662F

l0800_6628:
	add	word ptr [bp-2E],0200
	jmp	663B

l0800_662F:
	cmp	word ptr [2A21],01
	jnz	663B

l0800_6636:
	add	word ptr [bp-2E],0180

l0800_663B:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-2E]
	call	40BF
	add	sp,06
	mov	ax,[bp-26]
	mov	cl,04
	shl	ax,cl
	mov	dx,[2A09]
	mov	bx,[2A07]
	sub	bx,[bp-04]
	sbb	dx,[bp-02]
	sub	bx,ax
	sbb	dx,00
	or	bx,dx
	jz	6673

l0800_666A:
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_6673:
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret
0800:667B                                  55 8B EC 8B 46            U...F
0800:6680 04 3B 46 08 76 05 B8 01 00 5D C3 8B 46 04 3B 46 .;F.v....]..F.;F
0800:6690 08 73 05 B8 FF FF 5D C3 33 C0 5D C3             .s....].3.].   

;; fn0800_669C: 0800:669C
;;   Called from:
;;     0800:0FE6 (in fn0800_0DE8)
fn0800_669C proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	di
	cmp	word ptr [2A09],00
	jc	66BE

l0800_66AB:
	ja	66B5

l0800_66AD:
	cmp	word ptr [2A07],FEFE
	jbe	66BE

l0800_66B5:
	mov	ax,0003
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_66BE:
	push	ds
	pop	es
	mov	di,2206
	mov	si,4271
	mov	cx,0004
	mov	dx,0003
	cmp	dx,cx
	jnc	66D2

l0800_66D0:
	mov	cx,dx

l0800_66D2:
	xor	ax,ax

l0800_66D4:
	rep cmpsb

l0800_66D6:
	jz	66DD

l0800_66D8:
	sbb	ax,ax
	sbb	ax,FFFF

l0800_66DD:
	or	ax,ax
	jz	6715

l0800_66E1:
	push	ds
	pop	es
	mov	di,4271
	mov	si,2206
	mov	dx,0003
	mov	cx,0004
	sub	dx,cx
	jnc	66F7

l0800_66F3:
	add	cx,dx
	xor	dx,dx

l0800_66F7:
	shr	cx,01

l0800_66F9:
	rep movsw

l0800_66FB:
	adc	cx,cx

l0800_66FD:
	rep movsb

l0800_66FF:
	mov	cx,dx
	xor	ax,ax

l0800_6703:
	rep stosb

l0800_6705:
	push	ds
	mov	ax,2E75
	push	ax
	push	ds
	mov	ax,4271
	push	ax
	call	09A3
	add	sp,08

l0800_6715:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	xor	dx,dx
	push	dx
	push	ax
	push	ds
	mov	ax,2E77
	push	ax
	call	4152
	add	sp,0C
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	mov	dx,[2A09]
	mov	bx,[2A07]
	add	bx,ax
	adc	dx,00
	add	bx,40
	adc	dx,00
	mov	[bp-02],dx
	mov	[bp-04],bx
	cmp	word ptr [2A1D],00
	jz	6779

l0800_676E:
	add	word ptr [bp-04],0200
	adc	word ptr [bp-02],00
	jmp	6789

l0800_6779:
	cmp	word ptr [2A21],01
	jnz	6789

l0800_6780:
	add	word ptr [bp-04],0180
	adc	word ptr [bp-02],00

l0800_6789:
	cmp	word ptr [bp-02],00
	jc	67A1

l0800_678F:
	ja	6798

l0800_6791:
	cmp	word ptr [bp-04],FEFE
	jbe	67A1

l0800_6798:
	mov	ax,0003
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_67A1:
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	75EA
	add	sp,08
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_67BF: 0800:67BF
;;   Called from:
;;     0800:0FED (in fn0800_0DE8)
fn0800_67BF proc
	push	bp
	mov	bp,sp
	sub	sp,0E
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	mov	[bp-0E],ax
	xor	dx,dx
	cmp	dx,[2A09]
	jc	67F0

l0800_67E1:
	jnz	67E9

l0800_67E3:
	cmp	ax,[2A07]
	jc	67F0

l0800_67E9:
	mov	ax,0003
	mov	sp,bp
	pop	bp
	ret

l0800_67F0:
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	word ptr [bp-0E]
	push	ds
	mov	ax,2E77
	push	ax
	call	4152
	add	sp,0C
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFEE
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F0A
	add	sp,04
	cmp	ax,601A
	jz	6839

l0800_6836:
	jmp	68F1

l0800_6839:
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,0002
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	xor	ax,ax
	mov	dx,0010
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	xor	ax,ax
	push	ax
	mov	dx,001A
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E27
	add	sp,04
	push	ax
	call	401E
	add	sp,06
	mov	ax,[2A09]
	mov	dx,[2A07]
	sub	dx,1C
	sbb	ax,0000
	push	ax
	push	dx
	mov	ax,[2A09]
	mov	dx,[2A07]
	sub	dx,1C
	sbb	ax,0000
	push	ax
	push	dx
	call	75EA
	add	sp,08
	mov	ax,[2A09]
	mov	dx,[2A07]
	sub	dx,1C
	sbb	ax,0000
	sub	dx,[2A03]
	sbb	ax,[2A05]
	add	dx,[4E88]
	adc	ax,[4E8A]
	add	dx,0E
	adc	ax,0000
	mov	[bp-06],ax
	mov	[bp-08],dx
	jmp	6959

l0800_68F1:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	409C
	add	sp,08
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,000E
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	75EA
	add	sp,08
	mov	ax,[2A09]
	mov	dx,[2A07]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	add	dx,[4E88]
	adc	ax,[4E8A]
	add	dx,0E
	adc	ax,0000
	mov	[bp-06],ax
	mov	[bp-08],dx

l0800_6959:
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	and	ax,0001
	or	ax,0000
	jz	6988

l0800_696F:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,90
	push	ax
	call	4047
	add	sp,06
	add	word ptr [bp-08],01
	adc	word ptr [bp-06],00

l0800_6988:
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	add	ax,FFE0
	adc	dx,FF
	mov	[2A05],dx
	mov	[2A03],ax
	xor	ax,ax
	push	ax
	mov	dx,0002
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	3F58
	add	sp,04
	sub	[bp-08],ax
	sbb	[bp-06],dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A05]
	push	word ptr [2A03]
	call	409C
	add	sp,08
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E27
	add	sp,04
	cmp	ax,601A
	jz	6A1E

l0800_6A1B:
	jmp	6AA4

l0800_6A1E:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	pop	bx
	pop	cx
	add	cx,ax
	adc	bx,dx
	mov	[bp-02],bx
	mov	[bp-04],cx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	add	dx,[bp-0C]
	adc	ax,[bp-0A]
	mov	bx,[2A05]
	mov	cx,[2A03]
	add	cx,[bp-08]
	adc	bx,[bp-06]
	cmp	ax,bx
	jc	6AA4

l0800_6A7A:
	ja	6A80

l0800_6A7C:
	cmp	dx,cx
	jbe	6AA4

l0800_6A80:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	add	dx,[bp-0C]
	adc	ax,[bp-0A]
	mov	bx,[2A05]
	mov	cx,[2A03]
	add	cx,[bp-08]
	adc	bx,[bp-06]
	sub	dx,cx
	sbb	ax,bx
	add	[bp-08],dx
	adc	[bp-06],ax

l0800_6AA4:
	xor	ax,ax
	push	ax
	mov	dx,000A
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	409C
	add	sp,08
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret

;; fn0800_6AD4: 0800:6AD4
;;   Called from:
;;     0800:0FF4 (in fn0800_0DE8)
fn0800_6AD4 proc
	push	bp
	mov	bp,sp
	sub	sp,18
	push	si
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	xor	dx,dx
	cmp	dx,[2A09]
	jc	6B04

l0800_6AF4:
	jnz	6AFC

l0800_6AF6:
	cmp	ax,[2A07]
	jc	6B04

l0800_6AFC:
	mov	ax,0003
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_6B04:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	or	dx,dx
	jnz	6B1B

l0800_6B16:
	cmp	ax,03F3
	jz	6B23

l0800_6B1B:
	mov	ax,0003
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_6B23:
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	mov	dx,03F3
	push	ax
	push	dx
	call	409C
	add	sp,08

l0800_6B38:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	mov	[bp-02],dx
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-02]
	push	ax
	call	409C
	add	sp,08
	jmp	6B84

l0800_6B66:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	push	dx
	push	ax
	call	409C
	add	sp,08

l0800_6B84:
	mov	ax,[bp-08]
	mov	dx,[bp-06]
	sub	word ptr [bp-08],01
	sbb	word ptr [bp-06],00
	or	ax,dx
	jnz	6B66

l0800_6B96:
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jnz	6B38

l0800_6B9E:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	add	ax,0001
	adc	dx,00
	push	dx
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-0E],dx
	mov	[bp-10],ax
	mov	ax,[bp-0E]
	mov	dx,[bp-10]
	sub	dx,[bp-0C]
	sbb	ax,[bp-0A]
	add	dx,01
	adc	ax,0000
	mov	[bp-12],ax
	mov	[bp-14],dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0A]
	push	word ptr [bp-0C]
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,[bp-0E]
	mov	dx,[bp-10]
	add	dx,01
	adc	ax,0000
	push	ax
	push	dx
	call	409C
	add	sp,08
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	sub	ax,000C
	shr	ax,01
	shr	ax,01
	mov	word ptr [bp-02],0000
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	word ptr [bp-04]
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	[4E8E],dx
	mov	[4E8C],ax
	mov	ax,[bp-12]
	mov	dx,[bp-14]
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	6CAF

l0800_6C89:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	push	dx
	push	ax
	call	409C
	add	sp,08
	sub	word ptr [bp-04],01
	sbb	word ptr [bp-02],00

l0800_6CAF:
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jnz	6C89

l0800_6CB7:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	xor	dx,dx
	push	dx
	push	ax
	push	ds
	mov	ax,2E77
	push	ax
	call	4152
	add	sp,0C
	xor	si,si
	jmp	6E93

l0800_6CE4:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	and	ax,3FFF
	mov	[bp-18],dx
	mov	[bp-16],ax
	mov	cx,0007
	mov	bx,6EBC

l0800_6D0D:
	mov	ax,cs:[bx]
	cmp	ax,[bp-08]
	jnz	6D1E

l0800_6D15:
	mov	ax,cs:[bx+0E]
	cmp	ax,[bp-16]
	jz	6D26

l0800_6D1E:
	add	bx,02
	loop	6D0D

l0800_6D23:
	jmp	6E8B

l0800_6D26:
	jmp	word ptr cs:[bx+1C]
0800:6D2A                               1E B8 0A 22 50 56           ..."PV
0800:6D30 E8 B3 01 83 C4 06 FF 76 FA FF 76 F8 E8 E1 01 83 .......v..v.....
0800:6D40 C4 04 56 E8 B9 01 83 C4 02 46 E9 46 01 1E B8 0F ..V......F.F....
0800:6D50 22 50 56 E8 90 01 83 C4 06 FF 76 FA FF 76 F8 E8 "PV.......v..v..
0800:6D60 BE 01 83 C4 04 56 E8 96 01 83 C4 02 46 E9 23 01 .....V......F.#.
0800:6D70 1E B8 14 22 50 56 E8 6D 01 83 C4 06 FF 36 E1 29 ..."PV.m.....6.)
0800:6D80 FF 36 DF 29 33 C0 BA EB 03 50 52 E8 0E D3 83 C4 .6.)3....PR.....
0800:6D90 08 FF 36 E5 29 FF 36 E3 29 E8 C1 D0 83 C4 04 89 ..6.).6.).......
0800:6DA0 56 FE 89 46 FC FF 36 E1 29 FF 36 DF 29 FF 76 FE V..F..6.).6.).v.
0800:6DB0 50 E8 E8 D2 83 C4 08 83 06 8C 4E 04 83 16 8E 4E P.........N....N
0800:6DC0 00 56 E8 3A 01 83 C4 02 E9 C8 00 1E B8 19 22 50 .V.:.........."P
0800:6DD0 56 E8 12 01 83 C4 06 FF 36 E5 29 FF 36 E3 29 E8 V.......6.).6.).
0800:6DE0 7B D0 83 C4 04 B1 02 E8 7F 1E 89 56 FE 89 46 FC {..........V..F.
0800:6DF0 0B 46 FE 74 20 B8 01 00 50 8B 46 FE 8B 56 FC 83 .F.t ...P.F..V..
0800:6E00 C2 04 15 00 00 50 52 FF 36 E5 29 FF 36 E3 29 E8 .....PR.6.).6.).
0800:6E10 A1 3E 83 C4 0A 8B 46 FC 0B 46 FE 75 BA 56 E8 DE .>....F..F.u.V..
0800:6E20 00 83 C4 02 EB 6D 1E B8 1E 22 50 56 E8 B7 00 83 .....m..."PV....
0800:6E30 C4 06 FF 36 E5 29 FF 36 E3 29 E8 20 D0 83 C4 04 ...6.).6.). ....
0800:6E40 B1 02 E8 24 1E 89 56 FE 89 46 FC B8 01 00 50 FF ...$..V..F....P.
0800:6E50 76 FE FF 76 FC FF 36 E5 29 FF 36 E3 29 E8 53 3E v..v..6.).6.).S>
0800:6E60 83 C4 0A 56 E8 98 00 83 C4 02 EB 27 B8 08 00 5E ...V.......'...^
0800:6E70 8B E5 5D C3 FF 36 E1 29 FF 36 DF 29 33 C0 BA F2 ..]..6.).6.)3...
0800:6E80 03 50 52 E8 16 D2 83 C4 08 EB 08                .PR........    

l0800_6E8B:
	mov	ax,0009
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_6E93:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	cmp	dx,[2A09]
	jnc	6EAA

l0800_6EA7:
	jmp	6CE4

l0800_6EAA:
	jnz	6EB5

l0800_6EAC:
	cmp	ax,[2A07]
	jnc	6EB5

l0800_6EB2:
	jmp	6CE4

l0800_6EB5:
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret
0800:6EBC                                     E9 03 EA 03             ....
0800:6EC0 EB 03 F0 03 F1 03 F2 03 F5 03 00 00 00 00 00 00 ................
0800:6ED0 00 00 00 00 00 00 00 00 2A 6D 4D 6D 70 6D CB 6D ........*mMmpm.m
0800:6EE0 26 6E 74 6E 6C 6E                               &ntnln         

;; fn0800_6EE6: 0800:6EE6
fn0800_6EE6 proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	ds
	mov	ax,2223
	push	ax
	call	B2EF
	add	sp,0A
	pop	bp
	ret

;; fn0800_6EFF: 0800:6EFF
fn0800_6EFF proc
	push	bp
	mov	bp,sp
	push	ds
	mov	ax,2231
	push	ax
	call	B2EF
	add	sp,04
	cmp	word ptr [bp+04],09
	jbe	6F1E

l0800_6F13:
	push	ds
	mov	ax,223E
	push	ax
	call	B2EF
	add	sp,04

l0800_6F1E:
	pop	bp
	ret

;; fn0800_6F20: 0800:6F20
fn0800_6F20 proc
	push	bp
	mov	bp,sp
	sub	sp,1A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	cl,02
	call	8C69
	mov	[bp-0E],dx
	mov	[bp-10],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	mov	[bp-12],dx
	mov	[bp-14],ax
	mov	ax,0001
	push	ax
	push	word ptr [bp-0E]
	push	word ptr [bp-10]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	jmp	6FCC

l0800_6F6D:
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,0004
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A

l0800_6F86:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	cl,02
	call	8C69
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,[bp-02]
	jz	6FC4

l0800_6FA4:
	mov	ax,0001
	push	ax
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	add	dx,04
	adc	ax,0000
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A

l0800_6FC4:
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jnz	6F86

l0800_6FCC:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F58
	add	sp,04
	or	dx,dx
	jnz	6FE3

l0800_6FDE:
	cmp	ax,03EC
	jz	6F6D

l0800_6FE3:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	sub	ax,[bp-14]
	sbb	dx,[bp-12]
	mov	[bp-16],dx
	mov	[bp-18],ax
	mov	[2A05],dx
	mov	[2A03],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	cmp	word ptr [bp-16],00
	jnc	7035

l0800_7032:
	jmp	70F9

l0800_7035:
	ja	7040

l0800_7037:
	cmp	word ptr [bp-18],12
	ja	7040

l0800_703D:
	jmp	70F9

l0800_7040:
	xor	ax,ax
	push	ax
	push	word ptr [4E8E]
	push	word ptr [4E8C]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	3E5D
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0E]
	push	word ptr [bp-10]
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0A]
	push	word ptr [bp-0C]
	call	409C
	add	sp,08
	xor	ax,ax
	push	ax
	push	word ptr [bp-12]
	push	word ptr [bp-14]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	mov	ax,[bp-16]
	mov	dx,[bp-18]
	sub	dx,08
	sbb	ax,0000
	push	ax
	push	dx
	push	word ptr [bp-16]
	push	word ptr [bp-18]
	call	75EA
	add	sp,08
	add	word ptr [2A03],08
	adc	word ptr [2A05],00

l0800_70F9:
	mov	ax,[2A05]
	mov	dx,[2A03]
	cmp	ax,[bp-16]
	jnc	7108

l0800_7105:
	jmp	724D

l0800_7108:
	jnz	7112

l0800_710A:
	cmp	dx,[bp-18]
	jnc	7112

l0800_710F:
	jmp	724D

l0800_7112:
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	xor	ax,ax
	push	ax
	mov	ax,[bp-12]
	mov	dx,[bp-14]
	sub	dx,04
	sbb	ax,0000
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	mov	ax,[bp-0E]
	mov	dx,[bp-10]
	add	dx,04
	adc	ax,0000
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3F58
	add	sp,04
	or	dx,dx
	jz	7181

l0800_717E:
	jmp	723F

l0800_7181:
	cmp	ax,03EC
	jz	7189

l0800_7186:
	jmp	723F

l0800_7189:
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,0004
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	mov	dx,03EC
	push	ax
	push	dx
	call	409C
	add	sp,08

l0800_71B7:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-02]
	push	ax
	call	409C
	add	sp,08
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jz	7234

l0800_71E5:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	add	ax,0001
	adc	dx,00
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-06]
	push	ax
	call	409C
	add	sp,08
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	cl,02
	call	8C69
	push	dx
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C

l0800_7234:
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jz	723F

l0800_723C:
	jmp	71B7

l0800_723F:
	add	word ptr [4E8C],04
	adc	word ptr [4E8E],00
	mov	sp,bp
	pop	bp
	ret

l0800_724D:
	mov	ax,[bp-16]
	mov	dx,[bp-18]
	add	dx,[4E88]
	adc	ax,[4E8A]
	mov	[bp-02],ax
	mov	[bp-04],dx
	mov	ax,[2A03]
	and	ax,0003
	or	ax,0000
	jz	7274

l0800_726C:
	add	word ptr [bp-04],02
	adc	word ptr [bp-02],00

l0800_7274:
	mov	ax,[bp-04]
	and	ax,0003
	or	ax,0000
	jz	7298

l0800_727F:
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	cl,02
	call	8CAA
	add	ax,0001
	adc	dx,00
	mov	[bp-02],dx
	mov	[bp-04],ax
	jmp	72C6

l0800_7298:
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	cl,02
	call	8CAA
	mov	[bp-02],dx
	mov	[bp-04],ax
	jmp	72C6

l0800_72AB:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,00
	push	ax
	call	4047
	add	sp,06
	add	word ptr [2A03],01
	adc	word ptr [2A05],00

l0800_72C6:
	mov	ax,[2A03]
	and	ax,0003
	or	ax,0000
	jnz	72AB

l0800_72D1:
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	mov	ax,0001
	push	ax
	mov	ax,[2A05]
	mov	dx,[2A03]
	neg	ax
	neg	dx
	sbb	ax,0000
	sub	dx,04
	sbb	ax,0000
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	dx,[2A05]
	mov	ax,[2A03]
	mov	cl,02
	call	8CAA
	push	dx
	push	ax
	call	409C
	add	sp,08
	mov	ax,[bp-0A]
	mov	dx,[bp-0C]
	and	ax,3FFF
	mov	[bp-1A],ax
	cmp	ax,[bp-02]
	jc	734D

l0800_733A:
	ja	7341

l0800_733C:
	cmp	dx,[bp-04]
	jbe	734D

l0800_7341:
	mov	ax,[bp-0C]
	mov	dx,[bp-1A]
	mov	[bp-02],dx
	mov	[bp-04],ax

l0800_734D:
	mov	ax,[bp-0A]
	and	ax,C000
	or	word ptr [bp-04],00
	or	[bp-02],ax
	xor	ax,ax
	push	ax
	push	word ptr [4E8E]
	push	word ptr [4E8C]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	409C
	add	sp,08
	xor	ax,ax
	push	ax
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	add	word ptr [4E8C],04
	adc	word ptr [4E8E],00
	mov	sp,bp
	pop	bp
	ret

;; fn0800_73AC: 0800:73AC
;;   Called from:
;;     0800:0FFB (in fn0800_0DE8)
fn0800_73AC proc
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	xor	dx,dx
	push	dx
	push	ax
	push	ds
	mov	ax,2E77
	push	ax
	call	4152
	add	sp,0C
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFFC
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A11]
	push	word ptr [2A0F]
	call	409C
	add	sp,08
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	75EA
	add	sp,08
	xor	ax,ax
	ret

;; fn0800_741D: 0800:741D
;;   Called from:
;;     0800:1002 (in fn0800_0DE8)
fn0800_741D proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	jmp	75C1

l0800_7427:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E9A
	add	sp,04
	or	ax,ax
	jz	7449

l0800_7439:
	cmp	ax,0001
	jz	7469

l0800_743E:
	cmp	ax,0003
	jnz	7446

l0800_7443:
	jmp	7566

l0800_7446:
	jmp	758F

l0800_7449:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3DCF
	add	sp,04
	push	ax
	call	4047
	add	sp,06
	jmp	75C1

l0800_7469:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3E5D
	add	sp,04
	push	dx
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3FAD
	add	sp,04
	mov	word ptr [bp-02],0000
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	call	401E
	add	sp,06
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	75EA
	add	sp,08
	mov	ax,0001
	push	ax
	mov	ax,[2A05]
	mov	dx,[2A03]
	add	dx,02
	adc	ax,0000
	neg	ax
	neg	dx
	sbb	ax,0000
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A03]
	call	40BF
	add	sp,06
	mov	ax,[2A05]
	mov	dx,[2A03]
	cmp	ax,[bp-02]
	jnz	754A

l0800_7506:
	cmp	dx,[bp-04]
	jnz	754A

l0800_750B:
	mov	ax,0001
	push	ax
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	neg	ax
	neg	dx
	sbb	ax,0000
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C
	jmp	75C1

l0800_754A:
	mov	ax,0001
	push	ax
	push	word ptr [2A05]
	push	word ptr [2A03]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	jmp	75C1

l0800_7566:
	xor	si,si
	jmp	7588

l0800_756A:
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3DCF
	add	sp,04
	push	ax
	call	4047
	add	sp,06
	inc	si

l0800_7588:
	cmp	si,03
	jnz	756A

l0800_758D:
	jmp	75C1

l0800_758F:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	mov	bx,[2A09]
	mov	cx,[2A07]
	sub	cx,ax
	sbb	bx,dx
	push	bx
	push	cx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3B0A
	add	sp,0C

l0800_75C1:
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	cmp	dx,[2A09]
	jnc	75D8

l0800_75D5:
	jmp	7427

l0800_75D8:
	jnz	75E3

l0800_75DA:
	cmp	ax,[2A07]
	jnc	75E3

l0800_75E0:
	jmp	7427

l0800_75E3:
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_75EA: 0800:75EA
;;   Called from:
;;     0800:0F36 (in fn0800_0DE8)
;;     0800:1758 (in fn0800_12E2)
;;     0800:62A5 (in fn0800_5E64)
;;     0800:67B1 (in fn0800_669C)
;;     0800:68C0 (in fn0800_67BF)
;;     0800:6930 (in fn0800_67BF)
;;     0800:70E9 (in fn0800_6F20)
;;     0800:7414 (in fn0800_73AC)
;;     0800:74BA (in fn0800_741D)
fn0800_75EA proc
	push	bp
	mov	bp,sp
	sub	sp,16
	push	si
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	mov	[4680],ax
	mov	[467E],dx
	mov	[2A05],ax
	mov	[2A03],dx
	mov	[4678],ax
	mov	[4676],dx
	mov	ax,[bp+0A]
	mov	dx,[bp+08]
	sub	dx,12
	sbb	ax,0000
	mov	[4E86],ax
	mov	[4E84],dx
	cmp	word ptr [4680],00
	ja	7634

l0800_7626:
	jz	762B

l0800_7628:
	jmp	79FD

l0800_762B:
	cmp	word ptr [bp+04],12
	ja	7634

l0800_7631:
	jmp	79FD

l0800_7634:
	mov	word ptr [2E4B],0000
	mov	word ptr [2E49],0000
	mov	word ptr [465C],0000
	mov	word ptr [4668],0000
	mov	word ptr [4666],0000
	mov	word ptr [2A05],0000
	mov	word ptr [2A03],0000
	mov	word ptr [2A01],0000
	mov	word ptr [29FF],0000
	mov	word ptr [467C],0000
	mov	word ptr [467A],0000
	mov	word ptr [4674],0000
	mov	word ptr [4672],0000
	mov	word ptr [2E43],0000
	mov	word ptr [2E45],0000
	mov	word ptr [4E82],0000
	mov	word ptr [4E8A],0000
	mov	word ptr [4E88],0000
	mov	word ptr [29F9],0000
	mov	word ptr [29F7],0000
	mov	ax,0001
	mov	dx,000F
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[bp-14],dx
	mov	[bp-16],ax
	inc	dx
	mov	[2E73],dx
	mov	word ptr [2E71],0000
	mov	ax,0001
	mov	dx,0010
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[bp-10],dx
	mov	[bp-12],ax
	inc	dx
	mov	[2E39],dx
	mov	word ptr [2E37],0000
	mov	ax,0001
	mov	dx,0010
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[bp-0C],dx
	mov	[bp-0E],ax
	inc	dx
	mov	[2E35],dx
	mov	word ptr [2E33],0000
	mov	ax,0001
	mov	dx,0010
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[bp-08],dx
	mov	[bp-0A],ax
	inc	dx
	mov	[2E41],dx
	mov	word ptr [2E3F],0000
	mov	ax,0001
	mov	dx,0010
	push	ax
	push	dx
	call	4311
	add	sp,04
	mov	[bp-04],dx
	mov	[bp-06],ax
	inc	dx
	mov	[2E3D],dx
	mov	word ptr [2E3B],0000
	call	87F8
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	mov	[4664],dx
	mov	[4662],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	mov	[4660],dx
	mov	[465E],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,[2A21]
	cwd
	add	ax,4300
	adc	dx,524E
	push	dx
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [4680]
	push	word ptr [467E]
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	ax
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	call	401E
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	call	401E
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	call	401E
	add	sp,06
	push	ds
	mov	ax,2240
	push	ax
	call	B2EF
	add	sp,04
	mov	ax,[2E4F]
	mov	[bp-02],ax
	mov	ax,0001
	push	ax
	cmp	word ptr [2A1F],00
	jz	7808

l0800_7806:
	jmp	780A

l0800_7808:
	xor	ax,ax

l0800_780A:
	push	ax
	call	8465
	add	sp,04
	mov	ax,0001
	push	ax
	cmp	word ptr [2E4F],00
	jz	781E

l0800_781C:
	jmp	7820

l0800_781E:
	xor	ax,ax

l0800_7820:
	push	ax
	call	8465
	add	sp,04
	cmp	word ptr [2A23],01
	jz	7843

l0800_782E:
	cmp	word ptr [2E4F],00
	jz	7843

l0800_7835:
	mov	ax,0010
	push	ax
	push	word ptr [2E4F]
	call	8465
	add	sp,04

l0800_7843:
	mov	ax,[2A21]
	cmp	ax,0001
	jz	7852

l0800_784B:
	cmp	ax,0002
	jz	7857

l0800_7850:
	jmp	785A

l0800_7852:
	call	7A02
	jmp	785A

l0800_7857:
	call	7C78

l0800_785A:
	xor	si,si
	jmp	7870

l0800_785E:
	mov	bx,si
	inc	si
	mov	al,[bx+4682]
	push	ax
	call	8624
	add	sp,02
	dec	word ptr [4E82]

l0800_7870:
	cmp	word ptr [4E82],00
	jnz	785E

l0800_7877:
	mov	ax,[bp-02]
	mov	[2E4F],ax
	mov	ax,[4680]
	mov	dx,[467E]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	cmp	ax,[4E8A]
	ja	78B3

l0800_7892:
	jc	789A

l0800_7894:
	cmp	dx,[4E88]
	jnc	78B3

l0800_789A:
	mov	ax,[4680]
	mov	dx,[467E]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	sub	[4E88],dx
	sbb	[4E8A],ax
	jmp	78BF

l0800_78B3:
	mov	word ptr [4E8A],0000
	mov	word ptr [4E88],0000

l0800_78BF:
	cmp	word ptr [2A21],02
	jnz	78D0

l0800_78C6:
	add	word ptr [4E88],02
	adc	word ptr [4E8A],00

l0800_78D0:
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	AD2F
	add	sp,04
	sub	ax,[465E]
	sbb	dx,[4660]
	mov	[2A05],dx
	mov	[2A03],ax
	xor	ax,ax
	push	ax
	mov	ax,[4660]
	mov	dx,[465E]
	add	dx,08
	adc	ax,0000
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,[2A05]
	mov	dx,[2A03]
	sub	dx,12
	sbb	ax,0000
	push	ax
	push	dx
	call	409C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2E4B]
	call	401E
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2E49]
	call	401E
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[4E88]
	push	ax
	call	4047
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[29F7]
	push	ax
	call	4047
	add	sp,06
	xor	ax,ax
	push	ax
	mov	ax,[4660]
	mov	dx,[465E]
	add	dx,[2A03]
	adc	ax,[2A05]
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	ACB3
	add	sp,0A
	xor	ax,ax
	push	ax
	mov	ax,[4664]
	mov	dx,[4662]
	add	dx,[467E]
	adc	ax,[4680]
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	push	word ptr [bp-14]
	push	word ptr [bp-16]
	call	4346
	add	sp,04
	push	word ptr [bp-10]
	push	word ptr [bp-12]
	call	4346
	add	sp,04
	push	word ptr [bp-0C]
	push	word ptr [bp-0E]
	call	4346
	add	sp,04
	push	word ptr [bp-08]
	push	word ptr [bp-0A]
	call	4346
	add	sp,04
	push	word ptr [bp-04]
	push	word ptr [bp-06]
	call	4346
	add	sp,04
	push	ds
	mov	ax,223C
	push	ax
	call	B2EF
	add	sp,04

l0800_79FD:
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_7A02: 0800:7A02
;;   Called from:
;;     0800:7852 (in fn0800_75EA)
fn0800_7A02 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	mov	ax,[4664]
	mov	dx,[4662]
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	7C1A

l0800_7A19:
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,284F
	push	ax
	call	43D4
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,278F
	push	ax
	call	43D4
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,290F
	push	ax
	call	43D4
	add	sp,06
	call	7FDC
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,290F
	push	ax
	call	441C
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,284F
	push	ax
	call	441C
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,278F
	push	ax
	call	441C
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,290F
	push	ax
	call	83A1
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,284F
	push	ax
	call	83A1
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,278F
	push	ax
	call	83A1
	add	sp,06
	mov	ax,0010
	push	ax
	push	word ptr [466A]
	call	8489
	add	sp,04
	jmp	7BC0

l0800_7ACB:
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	3E27
	add	sp,04
	mov	[2E47],ax
	add	[467A],ax
	adc	word ptr [467C],00
	push	ds
	mov	ax,290F
	push	ax
	push	word ptr [2E47]
	call	8407
	add	sp,06
	cmp	word ptr [2E47],00
	jz	7B56

l0800_7AFB:
	cmp	word ptr [2E45],00
	jz	7B32

l0800_7B02:
	jmp	7B17

l0800_7B04:
	call	8359
	xor	al,[2E4F]
	mov	bx,[4E82]
	mov	[bx+4682],al
	inc	word ptr [4E82]

l0800_7B17:
	mov	ax,[2E47]
	dec	word ptr [2E47]
	or	ax,ax
	jnz	7B04

l0800_7B22:
	jmp	7B3D

l0800_7B24:
	call	8359
	xor	al,[2E4F]
	push	ax
	call	8624
	add	sp,02

l0800_7B32:
	mov	ax,[2E47]
	dec	word ptr [2E47]
	or	ax,ax
	jnz	7B24

l0800_7B3D:
	test	word ptr [2E4F],0001
	jz	7B52

l0800_7B45:
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	7B56

l0800_7B52:
	shr	word ptr [2E4F],01

l0800_7B56:
	mov	ax,[466A]
	or	ax,[466C]
	jz	7BC0

l0800_7B5F:
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	3E27
	add	sp,04
	mov	[2E29],ax
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	3E27
	add	sp,04
	mov	[2E2B],ax
	push	ds
	mov	ax,284F
	push	ax
	push	word ptr [2E2B]
	call	8407
	add	sp,06
	push	ds
	mov	ax,278F
	push	ax
	push	word ptr [2E29]
	call	8407
	add	sp,06
	add	word ptr [2E29],02
	mov	ax,[2E29]
	add	[467A],ax
	adc	word ptr [467C],00
	jmp	7BB5

l0800_7BB2:
	call	8359

l0800_7BB5:
	mov	ax,[2E29]
	dec	word ptr [2E29]
	or	ax,ax
	jnz	7BB2

l0800_7BC0:
	mov	ax,[466A]
	mov	dx,[466C]
	sub	word ptr [466A],01
	sbb	word ptr [466C],00
	or	ax,dx
	jz	7BD8

l0800_7BD5:
	jmp	7ACB

l0800_7BD8:
	cmp	word ptr [2E45],00
	jnz	7BFC

l0800_7BDF:
	xor	si,si
	jmp	7BF5

l0800_7BE3:
	mov	bx,si
	inc	si
	mov	al,[bx+4682]
	push	ax
	call	8624
	add	sp,02
	dec	word ptr [4E82]

l0800_7BF5:
	cmp	word ptr [4E82],00
	jnz	7BE3

l0800_7BFC:
	add	word ptr [29F7],01
	adc	word ptr [29F9],00
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax

l0800_7C1A:
	mov	ax,[467C]
	mov	dx,[467A]
	cmp	ax,[4680]
	jnc	7C2A

l0800_7C27:
	jmp	7A19

l0800_7C2A:
	jnz	7C35

l0800_7C2C:
	cmp	dx,[467E]
	jnc	7C35

l0800_7C32:
	jmp	7A19

l0800_7C35:
	mov	cl,10
	sub	cl,[2E45]
	shr	word ptr [2E43],cl
	cmp	word ptr [2E45],00
	jnz	7C4D

l0800_7C46:
	cmp	word ptr [4E82],00
	jz	7C57

l0800_7C4D:
	mov	al,[2E43]
	push	ax
	call	8624
	add	sp,02

l0800_7C57:
	cmp	word ptr [2E45],08
	ja	7C65

l0800_7C5E:
	cmp	word ptr [4E82],00
	jz	7C73

l0800_7C65:
	mov	ax,[2E43]
	mov	cl,08
	shr	ax,cl
	push	ax
	call	8624
	add	sp,02

l0800_7C73:
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_7C78: 0800:7C78
;;   Called from:
;;     0800:7857 (in fn0800_75EA)
fn0800_7C78 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	mov	ax,[4664]
	mov	dx,[4662]
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	7E6D

l0800_7C8F:
	call	7FDC
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	jmp	7DCD

l0800_7CAC:
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	3E27
	add	sp,04
	mov	[2E47],ax
	add	[467A],ax
	adc	word ptr [467C],00
	push	ax
	call	7EAF
	add	sp,02
	mov	ax,[466A]
	or	ax,[466C]
	jnz	7CD9

l0800_7CD6:
	jmp	7DCD

l0800_7CD9:
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	3E27
	add	sp,04
	mov	[2E29],ax
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	3E27
	add	sp,04
	mov	[2E2B],ax
	cmp	word ptr [2E29],00
	jnz	7D1D

l0800_7D02:
	mov	ax,0003
	push	ax
	mov	ax,0006
	push	ax
	call	854B
	add	sp,04
	mov	al,[2E2B]
	push	ax
	call	8600
	add	sp,02
	jmp	7DAC

l0800_7D1D:
	cmp	word ptr [2E29],07
	jnc	7D68

l0800_7D24:
	mov	bx,[2E29]
	mov	al,[bx+21DB]
	mov	ah,00
	push	ax
	mov	al,[bx+21D4]
	mov	ah,00
	push	ax
	call	854B
	add	sp,04
	mov	bx,[2E2B]
	mov	cl,08
	shr	bx,cl
	mov	si,bx
	mov	al,[bx+21F2]
	mov	ah,00
	push	ax
	mov	al,[si+21E2]
	mov	ah,00
	push	ax
	call	854B
	add	sp,04
	mov	al,[2E2B]
	and	al,FF
	push	ax
	call	8600
	add	sp,02
	jmp	7DAC

l0800_7D68:
	mov	ax,0004
	push	ax
	mov	ax,000F
	push	ax
	call	854B
	add	sp,04
	mov	al,[2E29]
	sub	al,06
	push	ax
	call	8600
	add	sp,02
	mov	bx,[2E2B]
	mov	cl,08
	shr	bx,cl
	mov	si,bx
	mov	al,[bx+21F2]
	mov	ah,00
	push	ax
	mov	al,[si+21E2]
	mov	ah,00
	push	ax
	call	854B
	add	sp,04
	mov	al,[2E2B]
	and	al,FF
	push	ax
	call	8600
	add	sp,02

l0800_7DAC:
	add	word ptr [2E29],02
	mov	ax,[2E29]
	add	[467A],ax
	adc	word ptr [467C],00
	jmp	7DC2

l0800_7DBF:
	call	8359

l0800_7DC2:
	mov	ax,[2E29]
	dec	word ptr [2E29]
	or	ax,ax
	jnz	7DBF

l0800_7DCD:
	mov	ax,[466A]
	mov	dx,[466C]
	sub	word ptr [466A],01
	sbb	word ptr [466C],00
	or	ax,dx
	jz	7DE5

l0800_7DE2:
	jmp	7CAC

l0800_7DE5:
	mov	ax,0004
	push	ax
	mov	ax,000F
	push	ax
	call	854B
	add	sp,04
	mov	al,00
	push	ax
	call	8600
	add	sp,02
	mov	ax,[467C]
	mov	dx,[467A]
	cmp	ax,[4680]
	ja	7E1E

l0800_7E09:
	jc	7E11

l0800_7E0B:
	cmp	dx,[467E]
	jnc	7E1E

l0800_7E11:
	mov	ax,0001
	push	ax
	push	ax
	call	854B
	add	sp,04
	jmp	7E2B

l0800_7E1E:
	mov	ax,0001
	push	ax
	xor	ax,ax
	push	ax
	call	854B
	add	sp,04

l0800_7E2B:
	cmp	word ptr [2E45],00
	jnz	7E4F

l0800_7E32:
	xor	si,si
	jmp	7E48

l0800_7E36:
	mov	bx,si
	inc	si
	mov	al,[bx+4682]
	push	ax
	call	8624
	add	sp,02
	dec	word ptr [4E82]

l0800_7E48:
	cmp	word ptr [4E82],00
	jnz	7E36

l0800_7E4F:
	add	word ptr [29F7],01
	adc	word ptr [29F9],00
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	AD2F
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax

l0800_7E6D:
	mov	ax,[467C]
	mov	dx,[467A]
	cmp	ax,[4680]
	jnc	7E7D

l0800_7E7A:
	jmp	7C8F

l0800_7E7D:
	jnz	7E88

l0800_7E7F:
	cmp	dx,[467E]
	jnc	7E88

l0800_7E85:
	jmp	7C8F

l0800_7E88:
	mov	cl,08
	sub	cl,[2E45]
	shl	word ptr [2E43],cl
	cmp	word ptr [2E45],00
	jnz	7EA0

l0800_7E99:
	cmp	word ptr [4E82],00
	jz	7EAA

l0800_7EA0:
	mov	al,[2E43]
	push	ax
	call	8624
	add	sp,02

l0800_7EAA:
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_7EAF: 0800:7EAF
;;   Called from:
;;     0800:7CC7 (in fn0800_7C78)
fn0800_7EAF proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	si,[bp+04]
	jmp	7FD1

l0800_7EBA:
	cmp	si,0C
	jnc	7F32

l0800_7EBF:
	jmp	7EF6

l0800_7EC1:
	mov	ax,0001
	push	ax
	xor	ax,ax
	push	ax
	call	854B
	add	sp,04
	call	8359
	xor	al,[2E4F]
	push	ax
	call	8600
	add	sp,02
	test	word ptr [2E4F],0001
	jz	7EF1

l0800_7EE4:
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	7EF5

l0800_7EF1:
	shr	word ptr [2E4F],01

l0800_7EF5:
	dec	si

l0800_7EF6:
	or	si,si
	jnz	7EC1

l0800_7EFA:
	jmp	7FD1

l0800_7EFD:
	mov	ax,0001
	push	ax
	xor	ax,ax
	push	ax
	call	854B
	add	sp,04
	call	8359
	xor	al,[2E4F]
	push	ax
	call	8600
	add	sp,02
	test	word ptr [2E4F],0001
	jz	7F2D

l0800_7F20:
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	7F31

l0800_7F2D:
	shr	word ptr [2E4F],01

l0800_7F31:
	dec	si

l0800_7F32:
	test	si,0003
	jnz	7EFD

l0800_7F38:
	mov	ax,0005
	push	ax
	mov	ax,0017
	push	ax
	call	854B
	add	sp,04
	cmp	si,48
	jc	7F8F

l0800_7F4B:
	mov	ax,0004
	push	ax
	mov	ax,000F
	push	ax
	call	854B
	add	sp,04
	xor	di,di
	jmp	7F6C

l0800_7F5D:
	call	8359
	xor	al,[2E4F]
	push	ax
	call	8600
	add	sp,02
	inc	di

l0800_7F6C:
	cmp	di,48
	jnz	7F5D

l0800_7F71:
	test	word ptr [2E4F],0001
	jz	7F86

l0800_7F79:
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	7F8A

l0800_7F86:
	shr	word ptr [2E4F],01

l0800_7F8A:
	sub	si,48
	jmp	7FD1

l0800_7F8F:
	mov	ax,0004
	push	ax
	mov	ax,si
	sub	ax,000C
	shr	ax,01
	shr	ax,01
	push	ax
	call	854B
	add	sp,04
	jmp	7FB4

l0800_7FA5:
	call	8359
	xor	al,[2E4F]
	push	ax
	call	8600
	add	sp,02
	dec	si

l0800_7FB4:
	or	si,si
	jnz	7FA5

l0800_7FB8:
	test	word ptr [2E4F],0001
	jz	7FCD

l0800_7FC0:
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	7FD1

l0800_7FCD:
	shr	word ptr [2E4F],01

l0800_7FD1:
	or	si,si
	jz	7FD8

l0800_7FD5:
	jmp	7EBA

l0800_7FD8:
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_7FDC: 0800:7FDC
;;   Called from:
;;     0800:7A46 (in fn0800_7A02)
;;     0800:7C8F (in fn0800_7C78)
fn0800_7FDC proc
	push	bp
	mov	bp,sp
	sub	sp,04
	mov	word ptr [466C],0000
	mov	word ptr [466A],0000
	mov	word ptr [2E47],0000
	mov	ax,[2A0D]
	mov	dx,[2A0B]
	mov	[4670],ax
	mov	[466E],dx
	xor	ax,ax
	push	ax
	mov	ax,[4664]
	mov	dx,[4662]
	add	dx,[467A]
	adc	ax,[467C]
	add	dx,[4672]
	adc	ax,[4674]
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	ACB3
	add	sp,0A
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	ACB3
	add	sp,0A
	jmp	82A1

l0800_8042:
	xor	ax,ax
	mov	dx,FFFF
	sub	dx,[2E31]
	sbb	ax,0000
	sub	dx,[4672]
	sbb	ax,[4674]
	mov	[bp-02],ax
	mov	[bp-04],dx
	mov	ax,[4678]
	mov	dx,[4676]
	cmp	ax,[bp-02]
	ja	807C

l0800_8068:
	jc	806F

l0800_806A:
	cmp	dx,[bp-04]
	jnc	807C

l0800_806F:
	mov	ax,[4678]
	mov	dx,[4676]
	mov	[bp-02],ax
	mov	[bp-04],dx

l0800_807C:
	mov	ax,[2E73]
	mov	dx,[2E71]
	add	dx,[2E31]
	mov	[2E6F],ax
	mov	[2E6D],dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	mov	ax,[2E6D]
	add	ax,[4672]
	push	word ptr [2E73]
	push	ax
	call	4110
	add	sp,0C
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	sub	[4676],dx
	sbb	[4678],ax
	add	[4672],dx
	adc	[4674],ax
	mov	ax,[2E6F]
	mov	dx,[2E6D]
	add	dx,[4672]
	mov	[2E67],ax
	mov	[2E65],dx
	mov	[2E6B],ax
	mov	[2E69],dx
	mov	ax,[4670]
	mov	dx,[466E]
	cmp	ax,[4674]
	jbe	80ED

l0800_80EA:
	jmp	81E9

l0800_80ED:
	jc	80F8

l0800_80EF:
	cmp	dx,[4672]
	jc	80F8

l0800_80F5:
	jmp	81E9

l0800_80F8:
	mov	ax,[2E6F]
	mov	dx,[2E6D]
	add	dx,[466E]
	mov	[2E67],ax
	mov	[2E65],dx
	jmp	81E9

l0800_810D:
	call	8832
	cmp	word ptr [2E29],02
	jc	8193

l0800_8117:
	mov	ax,[2E6D]
	add	ax,[2E29]
	cmp	ax,[2E65]
	jbe	813F

l0800_8124:
	mov	ax,[466A]
	or	ax,[466C]
	jz	8130

l0800_812D:
	jmp	8209

l0800_8130:
	mov	ax,[2E65]
	xor	dx,dx
	sub	ax,[2E6D]
	sbb	dx,00
	mov	[2E29],ax

l0800_813F:
	push	ds
	mov	ax,290F
	push	ax
	push	word ptr [2E47]
	call	831D
	add	sp,06
	push	ds
	mov	ax,278F
	push	ax
	mov	ax,[2E29]
	sub	ax,0002
	push	ax
	call	831D
	add	sp,06
	push	ds
	mov	ax,284F
	push	ax
	mov	ax,[2E2B]
	dec	ax
	push	ax
	call	831D
	add	sp,06
	push	word ptr [2E29]
	call	89A8
	add	sp,02
	add	word ptr [466A],01
	adc	word ptr [466C],00
	mov	word ptr [2E47],0000
	mov	ax,[2E29]
	add	[465C],ax
	jmp	81A5

l0800_8193:
	mov	ax,0001
	push	ax
	call	89A8
	add	sp,02
	inc	word ptr [2E47]
	inc	word ptr [465C]

l0800_81A5:
	cmp	word ptr [465C],0400
	jc	81E9

l0800_81AD:
	mov	ax,[465C]
	add	[4666],ax
	adc	word ptr [4668],00
	push	word ptr [4680]
	push	word ptr [467E]
	mov	cx,[4668]
	mov	bx,[4666]
	xor	dx,dx
	mov	ax,0063
	call	8F18
	push	dx
	push	ax
	call	8BC2
	push	dx
	push	ax
	push	ds
	mov	ax,2244
	push	ax
	call	B2EF
	add	sp,08
	mov	word ptr [465C],0000

l0800_81E9:
	mov	ax,[2E65]
	dec	ax
	cmp	ax,[2E6D]
	jbe	8209

l0800_81F3:
	cmp	word ptr [466C],00
	jnc	81FD

l0800_81FA:
	jmp	810D

l0800_81FD:
	jnz	8209

l0800_81FF:
	cmp	word ptr [466A],FE
	jnc	8209

l0800_8206:
	jmp	810D

l0800_8209:
	mov	ax,[2E69]
	xor	dx,dx
	sub	ax,[2E6D]
	sbb	dx,00
	mov	[4674],dx
	mov	[4672],ax
	mov	ax,[2E6D]
	xor	dx,dx
	sub	ax,[2E71]
	sbb	dx,00
	sub	ax,[2E31]
	sbb	dx,00
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	ax,[2E71]
	add	ax,[bp-04]
	mov	cx,ax
	mov	dx,[2E6D]
	xor	bx,bx
	sub	dx,ax
	sbb	bx,00
	add	dx,[4672]
	push	dx
	push	word ptr [2E73]
	push	ax
	push	word ptr [2E73]
	push	word ptr [2E71]
	call	B0F3
	add	sp,0A
	mov	ax,[2E65]
	cmp	ax,[2E69]
	jc	82B9

l0800_8269:
	mov	ax,[2E67]
	mov	dx,[2E65]
	cmp	ax,[2E6B]
	jnz	8285

l0800_8276:
	cmp	dx,[2E69]
	jnz	8285

l0800_827C:
	mov	ax,[4676]
	or	ax,[4678]
	jz	82B9

l0800_8285:
	cmp	word ptr [466C],00
	jnz	8293

l0800_828C:
	cmp	word ptr [466A],FE
	jz	82B9

l0800_8293:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	sub	[466E],dx
	sbb	[4670],ax

l0800_82A1:
	mov	ax,[4676]
	or	ax,[4678]
	jz	82AD

l0800_82AA:
	jmp	8042

l0800_82AD:
	mov	ax,[4672]
	or	ax,[4674]
	jz	82B9

l0800_82B6:
	jmp	8042

l0800_82B9:
	mov	ax,[2E67]
	mov	dx,[2E65]
	cmp	ax,[2E6B]
	jnz	82ED

l0800_82C6:
	cmp	dx,[2E69]
	jnz	82ED

l0800_82CC:
	mov	ax,[4676]
	or	ax,[4678]
	jnz	82ED

l0800_82D5:
	cmp	word ptr [466C],00
	jnz	82E3

l0800_82DC:
	cmp	word ptr [466A],FE
	jz	82ED

l0800_82E3:
	mov	ax,[2E47]
	add	ax,[4672]
	mov	[2E47],ax

l0800_82ED:
	push	ds
	mov	ax,290F
	push	ax
	push	word ptr [2E47]
	call	831D
	add	sp,06
	add	word ptr [466A],01
	adc	word ptr [466C],00
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	ACB3
	add	sp,0A
	mov	sp,bp
	pop	bp
	ret

;; fn0800_831D: 0800:831D
;;   Called from:
;;     0800:8148 (in fn0800_7FDC)
;;     0800:815A (in fn0800_7FDC)
;;     0800:816A (in fn0800_7FDC)
;;     0800:82F6 (in fn0800_7FDC)
fn0800_831D proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+04]
	cmp	si,01
	jbe	8332

l0800_8329:
	push	si
	call	0C08
	add	sp,02
	jmp	8334

l0800_8332:
	mov	ax,si

l0800_8334:
	mov	dx,000C
	imul	dx
	les	bx,[bp+06]
	add	bx,ax
	add	word ptr es:[bx],01
	adc	word ptr es:[bx+02],00
	push	word ptr [29DD]
	push	word ptr [29DB]
	push	si
	call	401E
	add	sp,06
	pop	si
	pop	bp
	ret

;; fn0800_8359: 0800:8359
;;   Called from:
;;     0800:7B04 (in fn0800_7A02)
;;     0800:7B24 (in fn0800_7A02)
;;     0800:7BB2 (in fn0800_7A02)
;;     0800:7DBF (in fn0800_7C78)
;;     0800:7ECE (in fn0800_7EAF)
;;     0800:7F0A (in fn0800_7EAF)
;;     0800:7F5D (in fn0800_7EAF)
;;     0800:7FA5 (in fn0800_7EAF)
fn0800_8359 proc
	push	bp
	mov	bp,sp
	sub	sp,02
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	3DCF
	add	sp,04
	mov	[bp-01],al
	mov	al,[2E4B]
	xor	al,[bp-01]
	mov	ah,00
	and	ax,00FF
	shl	ax,01
	mov	bx,ax
	mov	ax,[bx+2A29]
	mov	dx,[2E4B]
	mov	cl,08
	shr	dx,cl
	xor	ax,dx
	mov	[2E4B],ax
	add	word ptr [29FF],01
	adc	word ptr [2A01],00
	mov	al,[bp-01]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_83A1: 0800:83A1
;;   Called from:
;;     0800:7A96 (in fn0800_7A02)
;;     0800:7AA5 (in fn0800_7A02)
;;     0800:7AB4 (in fn0800_7A02)
fn0800_83A1 proc
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	push	di
	mov	di,[bp+08]
	mov	si,di
	jmp	83C4

l0800_83B0:
	mov	ax,si
	mov	dx,000C
	imul	dx
	les	bx,[bp+04]
	add	bx,ax
	cmp	word ptr es:[bx+0A],00
	jnz	83CB

l0800_83C3:
	dec	di

l0800_83C4:
	mov	ax,si
	dec	si
	or	ax,ax
	jnz	83B0

l0800_83CB:
	mov	ax,0005
	push	ax
	push	di
	call	8489
	add	sp,04
	xor	si,si
	mov	ax,[bp+04]
	add	ax,000A
	mov	[bp-02],ax
	cmp	si,di
	jnc	8401

l0800_83E5:
	mov	ax,0004
	push	ax
	mov	es,[bp+06]
	mov	bx,[bp-02]
	push	word ptr es:[bx]
	call	8489
	add	sp,04
	add	word ptr [bp-02],0C
	inc	si
	cmp	si,di
	jc	83E5

l0800_8401:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_8407: 0800:8407
;;   Called from:
;;     0800:7AEE (in fn0800_7A02)
;;     0800:7B8A (in fn0800_7A02)
;;     0800:7B99 (in fn0800_7A02)
fn0800_8407 proc
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	push	di
	mov	si,[bp+04]
	cmp	si,01
	jbe	8420

l0800_8417:
	push	si
	call	0C08
	add	sp,02
	jmp	8422

l0800_8420:
	mov	ax,si

l0800_8422:
	mov	[bp-02],ax
	mov	dx,000C
	imul	dx
	les	bx,[bp+06]
	add	bx,ax
	mov	di,bx
	push	word ptr es:[bx+0A]
	push	word ptr es:[di+06]
	call	8489
	add	sp,04
	cmp	word ptr [bp-02],01
	jbe	845F

l0800_8445:
	mov	ax,[bp-02]
	dec	ax
	push	ax
	mov	cl,[bp-02]
	dec	cl
	mov	ax,0001
	shl	ax,cl
	mov	dx,si
	sub	dx,ax
	push	dx
	call	8489
	add	sp,04

l0800_845F:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_8465: 0800:8465
;;   Called from:
;;     0800:780B (in fn0800_75EA)
;;     0800:7821 (in fn0800_75EA)
;;     0800:783D (in fn0800_75EA)
fn0800_8465 proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+04]
	mov	dx,[bp+06]
	cmp	word ptr [2A21],02
	jnz	847F

l0800_8475:
	push	dx
	push	ax
	call	854B
	add	sp,04
	pop	bp
	ret

l0800_847F:
	push	dx
	push	ax
	call	8489
	add	sp,04
	pop	bp
	ret

;; fn0800_8489: 0800:8489
;;   Called from:
;;     0800:7AC2 (in fn0800_7A02)
;;     0800:83D0 (in fn0800_83A1)
;;     0800:83F2 (in fn0800_83A1)
;;     0800:8439 (in fn0800_8407)
;;     0800:8459 (in fn0800_8407)
;;     0800:8481 (in fn0800_8465)
fn0800_8489 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	di,[bp+04]
	jmp	853A

l0800_8494:
	shr	word ptr [2E43],01
	test	di,0001
	jz	84A4

l0800_849E:
	or	word ptr [2E43],8000

l0800_84A4:
	shr	di,01
	inc	word ptr [2E45]
	mov	ax,[2E45]
	cmp	ax,0010
	jz	84B5

l0800_84B2:
	jmp	853A

l0800_84B5:
	mov	al,[2E43]
	push	ax
	call	8624
	add	sp,02
	mov	ax,[2E43]
	mov	cl,08
	shr	ax,cl
	push	ax
	call	8624
	add	sp,02
	xor	si,si
	jmp	84E3

l0800_84D1:
	mov	bx,si
	inc	si
	mov	al,[bx+4682]
	push	ax
	call	8624
	add	sp,02
	dec	word ptr [4E82]

l0800_84E3:
	cmp	word ptr [4E82],00
	jnz	84D1

l0800_84EA:
	mov	ax,[2A01]
	mov	dx,[29FF]
	cmp	ax,[2A05]
	jc	8532

l0800_84F7:
	ja	84FF

l0800_84F9:
	cmp	dx,[2A03]
	jbe	8532

l0800_84FF:
	mov	ax,[2A01]
	mov	dx,[29FF]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	cmp	ax,[4E8A]
	jc	8532

l0800_8514:
	ja	851C

l0800_8516:
	cmp	dx,[4E88]
	jbe	8532

l0800_851C:
	mov	ax,[2A01]
	mov	dx,[29FF]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	mov	[4E8A],ax
	mov	[4E88],dx

l0800_8532:
	xor	ax,ax
	mov	[2E45],ax
	mov	[2E43],ax

l0800_853A:
	mov	ax,[bp+06]
	dec	word ptr [bp+06]
	or	ax,ax
	jz	8547

l0800_8544:
	jmp	8494

l0800_8547:
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_854B: 0800:854B
;;   Called from:
;;     0800:7D0A (in fn0800_7C78)
;;     0800:7D36 (in fn0800_7C78)
;;     0800:7D54 (in fn0800_7C78)
;;     0800:7D70 (in fn0800_7C78)
;;     0800:7D9A (in fn0800_7C78)
;;     0800:7DED (in fn0800_7C78)
;;     0800:7E16 (in fn0800_7C78)
;;     0800:7E25 (in fn0800_7C78)
;;     0800:7EC8 (in fn0800_7EAF)
;;     0800:7F04 (in fn0800_7EAF)
;;     0800:7F40 (in fn0800_7EAF)
;;     0800:7F53 (in fn0800_7EAF)
;;     0800:7F9D (in fn0800_7EAF)
;;     0800:8477 (in fn0800_8465)
fn0800_854B proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	cl,[bp+06]
	dec	cl
	mov	di,0001
	shl	di,cl
	jmp	85EF

l0800_855D:
	shl	word ptr [2E43],01
	test	[bp+04],di
	jz	856A

l0800_8566:
	inc	word ptr [2E43]

l0800_856A:
	shr	di,01
	inc	word ptr [2E45]
	mov	ax,[2E45]
	cmp	ax,0008
	jnz	85EF

l0800_8578:
	mov	al,[2E43]
	push	ax
	call	8624
	add	sp,02
	xor	si,si
	jmp	8598

l0800_8586:
	mov	bx,si
	inc	si
	mov	al,[bx+4682]
	push	ax
	call	8624
	add	sp,02
	dec	word ptr [4E82]

l0800_8598:
	cmp	word ptr [4E82],00
	jnz	8586

l0800_859F:
	mov	ax,[2A01]
	mov	dx,[29FF]
	cmp	ax,[2A05]
	jc	85E7

l0800_85AC:
	ja	85B4

l0800_85AE:
	cmp	dx,[2A03]
	jbe	85E7

l0800_85B4:
	mov	ax,[2A01]
	mov	dx,[29FF]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	cmp	ax,[4E8A]
	jc	85E7

l0800_85C9:
	ja	85D1

l0800_85CB:
	cmp	dx,[4E88]
	jbe	85E7

l0800_85D1:
	mov	ax,[2A01]
	mov	dx,[29FF]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	mov	[4E8A],ax
	mov	[4E88],dx

l0800_85E7:
	xor	ax,ax
	mov	[2E45],ax
	mov	[2E43],ax

l0800_85EF:
	mov	ax,[bp+06]
	dec	word ptr [bp+06]
	or	ax,ax
	jz	85FC

l0800_85F9:
	jmp	855D

l0800_85FC:
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_8600: 0800:8600
;;   Called from:
;;     0800:7D14 (in fn0800_7C78)
;;     0800:7D60 (in fn0800_7C78)
;;     0800:7D7C (in fn0800_7C78)
;;     0800:7DA6 (in fn0800_7C78)
;;     0800:7DF6 (in fn0800_7C78)
;;     0800:7ED6 (in fn0800_7EAF)
;;     0800:7F12 (in fn0800_7EAF)
;;     0800:7F65 (in fn0800_7EAF)
;;     0800:7FAD (in fn0800_7EAF)
fn0800_8600 proc
	push	bp
	mov	bp,sp
	mov	dl,[bp+04]
	cmp	word ptr [2E45],00
	jz	861B

l0800_860D:
	mov	bx,[4E82]
	mov	[bx+4682],dl
	inc	word ptr [4E82]
	pop	bp
	ret

l0800_861B:
	push	dx
	call	8624
	add	sp,02
	pop	bp
	ret

;; fn0800_8624: 0800:8624
;;   Called from:
;;     0800:7866 (in fn0800_75EA)
;;     0800:7B2C (in fn0800_7A02)
;;     0800:7BEB (in fn0800_7A02)
;;     0800:7C51 (in fn0800_7A02)
;;     0800:7C6D (in fn0800_7A02)
;;     0800:7E3E (in fn0800_7C78)
;;     0800:7EA4 (in fn0800_7C78)
;;     0800:84B9 (in fn0800_8489)
;;     0800:84C7 (in fn0800_8489)
;;     0800:84D9 (in fn0800_8489)
;;     0800:857C (in fn0800_854B)
;;     0800:858E (in fn0800_854B)
;;     0800:861C (in fn0800_8600)
fn0800_8624 proc
	push	bp
	mov	bp,sp
	mov	ax,[2A05]
	mov	dx,[2A03]
	cmp	ax,[4E86]
	ja	8678

l0800_8634:
	jc	863C

l0800_8636:
	cmp	dx,[4E84]
	jnc	8678

l0800_863C:
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[bp+04]
	push	ax
	call	4047
	add	sp,06
	mov	al,[2E49]
	xor	al,[bp+04]
	mov	ah,00
	and	ax,00FF
	shl	ax,01
	mov	bx,ax
	mov	ax,[bx+2A29]
	mov	dx,[2E49]
	mov	cl,08
	shr	dx,cl
	xor	ax,dx
	mov	[2E49],ax
	add	word ptr [2A03],01
	adc	word ptr [2A05],00

l0800_8678:
	pop	bp
	ret

;; fn0800_867A: 0800:867A
;;   Called from:
;;     0800:09F6 (in fn0800_09A3)
fn0800_867A proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	push	ds
	cld
	mov	si,[bp+06]
	mov	ds,[bp+08]
	mov	di,[bp+0A]
	mov	es,[bp+0C]
	add	si,04
	call	87EF
	push	ax
	push	bx
	add	si,0A
	stc
	lodsb
	adc	al,al
	add	al,al
	jmp	8745

l0800_86A1:
	lodsb
	adc	al,al
	jmp	86AD
0800:86A6                   90                                  .        

l0800_86A7:
	mov	cl,04

l0800_86A9:
	add	al,al
	jz	86A1

l0800_86AD:
	adc	bh,bh
	loop	86A9

l0800_86B1:
	mov	cl,03
	add	cl,bh
	add	cl,cl

l0800_86B7:
	rep movsw

l0800_86B9:
	jmp	8745

l0800_86BC:
	lodsb
	adc	al,al
	jmp	86EA
0800:86C1    90                                            .             

l0800_86C2:
	lodsb
	adc	al,al
	jmp	86F0
0800:86C7                      90                                .       

l0800_86C8:
	lodsb
	adc	al,al
	jmp	86F9
0800:86CD                                        90                    . 

l0800_86CE:
	lodsb
	adc	al,al
	jmp	8705
0800:86D3          90                                        .           

l0800_86D4:
	lodsb
	adc	al,al
	jmp	870E
0800:86D9                            90                            .     

l0800_86DA:
	lodsb
	adc	al,al
	jmp	8714
0800:86DF                                              90                .

l0800_86E0:
	lodsb
	adc	al,al
	jmp	8726
0800:86E5                90                                    .         

l0800_86E6:
	add	al,al
	jz	86BC

l0800_86EA:
	adc	cl,cl
	add	al,al
	jz	86C2

l0800_86F0:
	jnc	8701

l0800_86F2:
	nop
	nop
	nop
	add	al,al
	jz	86C8

l0800_86F9:
	dec	cx
	adc	cl,cl
	cmp	cl,09
	jz	86A7

l0800_8701:
	add	al,al
	jz	86CE

l0800_8705:
	jnc	8728

l0800_8707:
	nop
	nop
	nop
	add	al,al
	jz	86D4

l0800_870E:
	adc	bh,bh
	add	al,al
	jz	86DA

l0800_8714:
	jc	8784

l0800_8716:
	nop
	nop
	nop
	or	bh,bh
	jnz	8728

l0800_871D:
	nop
	nop
	nop
	inc	bh

l0800_8722:
	add	al,al
	jz	86E0

l0800_8726:
	adc	bh,bh

l0800_8728:
	mov	bl,[si]
	inc	si
	mov	bp,si
	mov	si,di
	dec	si
	sub	si,bx
	cli

l0800_8733:
	rep movsb

l0800_8736:
	sti
	mov	si,bp
	jmp	8745
0800:873B                                  90                        .   

l0800_873C:
	lodsb
	adc	al,al
	jc	8753

l0800_8741:
	nop
	nop
	nop

l0800_8744:
	movsb

l0800_8745:
	add	al,al
	jc	8751

l0800_8749:
	nop
	nop
	nop
	movsb
	add	al,al
	jnc	8744

l0800_8751:
	jz	873C

l0800_8753:
	mov	cx,0002
	sub	bh,bh
	add	al,al
	jz	879B

l0800_875C:
	nop
	nop
	nop

l0800_875F:
	jnc	86E6

l0800_8761:
	add	al,al
	jz	87A0

l0800_8765:
	nop
	nop
	nop

l0800_8768:
	jnc	8728

l0800_876A:
	inc	cx
	add	al,al
	jz	87A5

l0800_876F:
	nop
	nop
	nop

l0800_8772:
	jnc	8701

l0800_8774:
	mov	cl,[si]
	inc	si
	or	cl,cl
	jz	87B4

l0800_877B:
	nop
	nop
	nop
	add	cx,08
	jmp	8701

l0800_8784:
	add	al,al
	jz	87AA

l0800_8788:
	nop
	nop
	nop

l0800_878B:
	adc	bh,bh
	or	bh,04
	add	al,al
	jz	87AF

l0800_8794:
	nop
	nop
	nop

l0800_8797:
	jc	8728

l0800_8799:
	jmp	8722

l0800_879B:
	lodsb
	adc	al,al
	jmp	875F

l0800_87A0:
	lodsb
	adc	al,al
	jmp	8768

l0800_87A5:
	lodsb
	adc	al,al
	jmp	8772

l0800_87AA:
	lodsb
	adc	al,al
	jmp	878B

l0800_87AF:
	lodsb
	adc	al,al
	jmp	8797

l0800_87B4:
	push	ax
	mov	bx,di
	and	di,0F
	add	di,8000
	mov	cl,04
	shr	bx,cl
	mov	ax,es
	add	ax,bx
	sub	ax,0800
	mov	es,ax
	mov	bx,si
	and	si,0F
	shr	bx,cl
	mov	ax,ds
	add	ax,bx
	mov	ds,ax
	pop	ax
	add	al,al
	jnz	87E3

l0800_87DD:
	nop
	nop
	nop
	lodsb
	adc	al,al

l0800_87E3:
	jnc	87E8

l0800_87E5:
	jmp	8745

l0800_87E8:
	pop	dx
	pop	ax
	pop	ds
	pop	di
	pop	si
	pop	bp
	retf

;; fn0800_87EF: 0800:87EF
;;   Called from:
;;     0800:8690 (in fn0800_867A)
fn0800_87EF proc
	call	87F4
	mov	bx,ax

;; fn0800_87F4: 0800:87F4
;;   Called from:
;;     0800:87EF (in fn0800_87EF)
;;     0800:87F2 (in fn0800_87EF)
fn0800_87F4 proc
	lodsw
	xchg	al,ah
	ret

;; fn0800_87F8: 0800:87F8
;;   Called from:
;;     0800:7747 (in fn0800_75EA)
fn0800_87F8 proc
	push	di
	cld
	les	di,[2E37]
	mov	ax,[2E31]
	mov	cx,8000

l0800_8804:
	rep stosw

l0800_8806:
	les	di,[2E33]
	mov	ax,[2E31]
	mov	cx,8000

l0800_8810:
	rep stosw

l0800_8812:
	les	di,[2E3B]
	mov	cx,[2E31]
	xor	ax,ax

l0800_881C:
	rep stosw

l0800_881E:
	les	di,[2E3F]
	xor	ax,ax
	mov	cx,[2E31]

l0800_8828:
	stosw
	inc	ax
	loop	8828

l0800_882C:
	mov	[2E2D],cx
	pop	di
	ret

;; fn0800_8832: 0800:8832
;;   Called from:
;;     0800:810D (in fn0800_7FDC)
fn0800_8832 proc
	push	si
	push	di
	call	889A
	cmp	word ptr [2E29],02
	jc	8897

l0800_883E:
	nop
	nop
	nop
	mov	ax,[2E65]
	sub	ax,[2E6D]
	cmp	ax,0003
	jc	8897

l0800_884D:
	nop
	nop
	nop
	mov	si,[2E29]
	mov	di,[2E2B]
	mov	ax,[2E2D]
	push	ax
	inc	ax
	cmp	ax,[2E31]
	jnz	8865

l0800_8863:
	xor	ax,ax

l0800_8865:
	mov	[2E2D],ax
	inc	word ptr [2E6D]
	call	889A
	dec	word ptr [2E6D]
	pop	word ptr [2E2D]
	cmp	[2E29],si
	jbe	888F

l0800_887D:
	nop
	nop
	nop
	mov	word ptr [2E29],0001
	mov	word ptr [2E2B],0000
	jmp	8897
0800:888E                                           90                  .

l0800_888F:
	mov	[2E29],si
	mov	[2E2B],di

l0800_8897:
	pop	di
	pop	si
	ret

;; fn0800_889A: 0800:889A
;;   Called from:
;;     0800:8834 (in fn0800_8832)
;;     0800:886C (in fn0800_8832)
fn0800_889A proc
	push	si
	push	di
	cld
	mov	word ptr [2E2B],0000
	mov	word ptr [2E29],0001
	les	di,[2E6D]
	mov	ax,es:[di]
	mov	[4E90],ax
	inc	di
	mov	dx,[2E69]
	sub	dx,di
	mov	cx,dx

l0800_88BC:
	rep scasb

l0800_88BE:
	jnz	88C1

l0800_88C0:
	dec	cx

l0800_88C1:
	sub	dx,cx
	les	di,[2E6D]
	mov	ax,[2E69]
	sub	ax,di
	mov	[4E94],ax
	mov	di,[4E90]
	shl	di,01
	mov	es,[2E39]
	mov	ax,es:[di]

l0800_88DC:
	cmp	ax,[2E31]
	jnz	88E5

l0800_88E2:
	jmp	8984

l0800_88E5:
	mov	di,ax
	shl	di,01
	mov	es,[2E41]
	mov	bx,es:[di]
	mov	[4E92],bx
	mov	bx,[2E2D]
	cmp	bx,ax
	ja	8903

l0800_88FC:
	nop
	nop
	nop
	add	bx,[2E31]

l0800_8903:
	sub	bx,ax
	les	si,[2E6D]
	sub	si,bx
	mov	ax,es:[si]
	cmp	[4E90],ax
	jnz	897E

l0800_8914:
	nop
	nop
	nop
	mov	es,[2E3D]
	mov	cx,es:[di]
	cmp	cx,bx
	jbe	892D

l0800_8922:
	nop
	nop
	nop
	mov	bx,0001
	mov	cx,dx
	jmp	8963
0800:892C                                     90                      .  

l0800_892D:
	cmp	cx,dx
	jbe	893A

l0800_8931:
	nop
	nop
	nop
	sub	cx,dx
	sub	bx,cx
	mov	cx,dx

l0800_893A:
	cmp	cx,dx
	jnz	8963

l0800_893E:
	nop
	nop
	nop
	les	di,[2E6D]
	add	di,cx
	mov	si,di
	sub	si,bx
	mov	ax,[4E94]
	sub	ax,cx
	mov	cx,ax
	push	ds
	mov	ds,[2E6F]

l0800_8957:
	rep cmpsb

l0800_8959:
	jz	895C

l0800_895B:
	inc	cx

l0800_895C:
	pop	ds
	sub	ax,cx
	mov	cx,dx
	add	cx,ax

l0800_8963:
	cmp	cx,[2E2F]
	jbe	896D

l0800_8969:
	mov	cx,[2E2F]

l0800_896D:
	cmp	cx,[2E29]
	jc	897E

l0800_8973:
	nop
	nop
	nop
	mov	[2E29],cx
	mov	[2E2B],bx

l0800_897E:
	mov	ax,[4E92]
	jmp	88DC

l0800_8984:
	cmp	word ptr [2E29],02
	jnz	89A5

l0800_898B:
	nop
	nop
	nop
	cmp	word ptr [2E2B],0100
	jbe	89A5

l0800_8996:
	nop
	nop
	nop
	mov	word ptr [2E29],0001
	mov	word ptr [2E2B],0000

l0800_89A5:
	pop	di
	pop	si
	ret

;; fn0800_89A8: 0800:89A8
;;   Called from:
;;     0800:8174 (in fn0800_7FDC)
;;     0800:8197 (in fn0800_7FDC)
fn0800_89A8 proc
	push	bp
	mov	bp,sp
	push	di
	mov	dx,[2E31]
	cld

l0800_89B1:
	mov	di,[2E2D]
	shl	di,01
	mov	es,[2E41]
	mov	ax,es:[di]
	mov	es:[di],dx
	cmp	[2E2D],ax
	jz	89EA

l0800_89C7:
	nop
	nop
	nop
	les	di,[2E6D]
	sub	di,dx
	mov	di,es:[di]
	shl	di,01
	mov	es,[2E39]
	mov	es:[di],ax
	cmp	ax,dx
	jnz	89EA

l0800_89E0:
	nop
	nop
	nop
	mov	es,[2E35]
	mov	es:[di],dx

l0800_89EA:
	les	di,[2E6D]
	mov	di,es:[di]
	shl	di,01
	mov	ax,[2E2D]
	mov	es,[2E39]
	cmp	es:[di],dx
	jnz	8A08

l0800_89FF:
	nop
	nop
	nop
	mov	es:[di],ax
	jmp	8A18
0800:8A07                      90                                .       

l0800_8A08:
	mov	es,[2E35]
	mov	bx,es:[di]
	shl	bx,01
	mov	es,[2E41]
	mov	es:[bx],ax

l0800_8A18:
	mov	es,[2E35]
	mov	es:[di],ax
	les	di,[2E6D]
	mov	al,es:[di]
	inc	di
	mov	bx,[2E69]
	sub	bx,di
	mov	cx,bx

l0800_8A2F:
	rep scasb

l0800_8A31:
	jnz	8A34

l0800_8A33:
	dec	cx

l0800_8A34:
	sub	bx,cx
	mov	di,[2E2D]
	shl	di,01
	mov	es,[2E3D]
	mov	es:[di],bx
	jmp	8A86
0800:8A45                90                                    .         

l0800_8A46:
	mov	di,[2E2D]
	shl	di,01
	mov	es,[2E3D]
	mov	es:[di],bx
	mov	es,[2E41]
	mov	ax,[2E2D]
	xchg	es:[di],ax
	cmp	[2E2D],ax
	jz	8A86

l0800_8A63:
	nop
	nop
	nop
	les	di,[2E6D]
	sub	di,dx
	mov	di,es:[di]
	shl	di,01
	mov	es,[2E39]
	mov	es:[di],ax
	cmp	ax,dx
	jnz	8A86

l0800_8A7C:
	nop
	nop
	nop
	mov	es,[2E35]
	mov	es:[di],dx

l0800_8A86:
	mov	ax,[2E2D]
	inc	ax
	cmp	ax,dx
	jnz	8A90

l0800_8A8E:
	xor	ax,ax

l0800_8A90:
	mov	[2E2D],ax
	inc	word ptr [2E6D]
	dec	word ptr [bp+04]
	jz	8AA8

l0800_8A9C:
	nop
	nop
	nop
	dec	bx
	cmp	bx,01
	ja	8A46

l0800_8AA5:
	jmp	89B1

l0800_8AA8:
	pop	di
	pop	bp
	ret
0800:8AAB                                  55 8B EC 83 3E            U...>
0800:8AB0 4E 22 20 75 05 B8 01 00 EB 13 8B 1E 4E 22 D1 E3 N" u........N"..
0800:8AC0 8B 46 04 89 87 96 4E FF 06 4E 22 33 C0 5D C3    .F....N..N"3.].

;; fn0800_8ACF: 0800:8ACF
fn0800_8ACF proc
	push	bp
	mov	bp,sp
	push	ds
	mov	ah,43
	xor	al,al
	lds	dx,[bp+04]
	int	21
	pop	ds
	jc	8AE9

l0800_8ADF:
	les	bx,[bp+08]
	mov	es:[bx],cx
	xor	ax,ax
	jmp	8AED

l0800_8AE9:
	push	ax
	call	8D64

l0800_8AED:
	pop	bp
	ret
0800:8AEF                                              55                U
0800:8AF0 8B EC 1E B4 43 B0 01 C5 56 04 8B 4E 08 CD 21 1F ....C...V..N..!.
0800:8B00 72 04 33 C0 EB 04 50 E8 5A 02 5D C3 C3          r.3...P.Z.]..  

;; fn0800_8B0D: 0800:8B0D
;;   Called from:
;;     0800:8B64 (in fn0800_8B5A)
;;     0800:8B76 (in fn0800_8B69)
fn0800_8B0D proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+08]
	or	si,si
	jnz	8B36

l0800_8B18:
	jmp	8B28

l0800_8B1A:
	dec	word ptr [224E]
	mov	bx,[224E]
	shl	bx,01
	call	word ptr [bx+4E96]

l0800_8B28:
	cmp	word ptr [224E],00
	jnz	8B1A

l0800_8B2F:
	call	0150
	call	word ptr [2352]

l0800_8B36:
	call	01B9
	call	0163
	cmp	word ptr [bp+06],00
	jnz	8B55

l0800_8B42:
	or	si,si
	jnz	8B4E

l0800_8B46:
	call	word ptr [2354]
	call	word ptr [2356]

l0800_8B4E:
	push	word ptr [bp+04]
	call	0164
	pop	cx

l0800_8B55:
	pop	si
	pop	bp
	ret	0006

;; fn0800_8B5A: 0800:8B5A
;;   Called from:
;;     0800:0DA1 (in fn0800_0D24)
;;     0800:0DE1 (in fn0800_0DCE)
fn0800_8B5A proc
	push	bp
	mov	bp,sp
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+04]
	call	8B0D
	pop	bp
	ret

;; fn0800_8B69: 0800:8B69
fn0800_8B69 proc
	push	bp
	mov	bp,sp
	mov	ax,0001
	push	ax
	xor	ax,ax
	push	ax
	push	word ptr [bp+04]
	call	8B0D
	pop	bp
	ret
0800:8B7B                                  33 C0 50 B8 01            3.P..
0800:8B80 00 50 33 C0 50 E8 85 FF C3 B8 01 00 50 50 33 C0 .P3.P.......PP3.
0800:8B90 50 E8 79 FF C3                                  P.y..          

;; fn0800_8B95: 0800:8B95
;;   Called from:
;;     0800:976F (in fn0800_9764)
fn0800_8B95 proc
	push	bp
	mov	bp,sp
	mov	ah,2A
	int	21
	les	bx,[bp+04]
	mov	es:[bx],cx
	mov	es:[bx+02],dx
	pop	bp
	ret

;; fn0800_8BA8: 0800:8BA8
;;   Called from:
;;     0800:9779 (in fn0800_9764)
fn0800_8BA8 proc
	push	bp
	mov	bp,sp
	mov	ah,2C
	int	21
	les	bx,[bp+04]
	mov	es:[bx],cx
	mov	es:[bx+02],dx
	pop	bp
	ret

;; fn0800_8BBB: 0800:8BBB
;;   Called from:
;;     0800:038F (in main)
;;     0800:03AB (in main)
;;     0800:B904 (in fn0800_B6D6)
;;     0800:B925 (in fn0800_B6D6)
;;     0800:C1BD (in fn0800_C177)
;;     0800:C1E9 (in fn0800_C177)
;;     0800:C1FF (in fn0800_C177)
;;     0800:C250 (in fn0800_C177)
;;     0800:C294 (in fn0800_C177)
;;     0800:C2D8 (in fn0800_C177)
fn0800_8BBB proc
	pop	cx
	push	cs
	push	cx
	xor	cx,cx
	jmp	8BD8

;; fn0800_8BC2: 0800:8BC2
;;   Called from:
;;     0800:0BAF (in fn0800_0B79)
;;     0800:0BC5 (in fn0800_0B79)
;;     0800:0C03 (in fn0800_0B79)
;;     0800:4598 (in fn0800_4550)
;;     0800:81D3 (in fn0800_7FDC)
fn0800_8BC2 proc
	pop	cx
	push	cs
	push	cx
	mov	cx,0001
	jmp	8BD8

;; fn0800_8BCA: 0800:8BCA
;;   Called from:
;;     0800:0373 (in main)
;;     0800:038A (in main)
;;     0800:03A6 (in main)
;;     0800:C1A6 (in fn0800_C177)
;;     0800:C1D3 (in fn0800_C177)
;;     0800:C21C (in fn0800_C177)
;;     0800:C266 (in fn0800_C177)
;;     0800:C283 (in fn0800_C177)
;;     0800:C2C1 (in fn0800_C177)
fn0800_8BCA proc
	pop	cx
	push	cs
	push	cx
	mov	cx,0002
	jmp	8BD8
0800:8BD2       59 0E 51 B9 03 00                           Y.Q...       

;; fn0800_8BD8: 0800:8BD8
;;   Called from:
;;     0800:8BC0 (in fn0800_8BBB)
;;     0800:8BC8 (in fn0800_8BC2)
;;     0800:8BD0 (in fn0800_8BCA)
fn0800_8BD8 proc
	push	bp
	push	si
	push	di
	mov	bp,sp
	mov	di,cx
	mov	ax,[bp+0A]
	mov	dx,[bp+0C]
	mov	bx,[bp+0E]
	mov	cx,[bp+10]
	or	cx,cx
	jnz	8BF7

l0800_8BEF:
	or	dx,dx
	jz	8C5C

l0800_8BF3:
	or	bx,bx
	jz	8C5C

l0800_8BF7:
	test	di,0001
	jnz	8C19

l0800_8BFD:
	or	dx,dx
	jns	8C0B

l0800_8C01:
	neg	dx
	neg	ax
	sbb	dx,00
	or	di,0C

l0800_8C0B:
	or	cx,cx
	jns	8C19

l0800_8C0F:
	neg	cx
	neg	bx
	sbb	cx,00
	xor	di,04

l0800_8C19:
	mov	bp,cx
	mov	cx,0020
	push	di
	xor	di,di
	xor	si,si

l0800_8C23:
	shl	ax,01
	rcl	dx,01
	rcl	si,01
	rcl	di,01
	cmp	di,bp
	jc	8C3A

l0800_8C2F:
	ja	8C35

l0800_8C31:
	cmp	si,bx
	jc	8C3A

l0800_8C35:
	sub	si,bx
	sbb	di,bp
	inc	ax

l0800_8C3A:
	loop	8C23

l0800_8C3C:
	pop	bx
	test	bx,0002
	jz	8C49

l0800_8C43:
	mov	ax,si
	mov	dx,di
	shr	bx,01

l0800_8C49:
	test	bx,0004
	jz	8C56

l0800_8C4F:
	neg	dx
	neg	ax
	sbb	dx,00

l0800_8C56:
	pop	di
	pop	si
	pop	bp
	retf	0008

l0800_8C5C:
	div	bx
	test	di,0002
	jz	8C65

l0800_8C64:
	xchg	ax,dx

l0800_8C65:
	xor	dx,dx
	jmp	8C56

;; fn0800_8C69: 0800:8C69
;;   Called from:
;;     0800:47FA (in fn0800_46FE)
;;     0800:4E2B (in fn0800_4C55)
;;     0800:4F42 (in fn0800_4F2C)
;;     0800:5096 (in fn0800_4F2C)
;;     0800:56BD (in fn0800_55E8)
;;     0800:5C72 (in fn0800_5C39)
;;     0800:5F4E (in fn0800_5E64)
;;     0800:60BB (in fn0800_5E64)
;;     0800:6F36 (in fn0800_6F20)
;;     0800:6F96 (in fn0800_6F20)
;;     0800:7219 (in fn0800_6F20)
;;     0800:A222 (in fn0800_A215)
fn0800_8C69 proc
	pop	bx
	push	cs
	push	bx
	cmp	cl,10
	jnc	8C81

l0800_8C71:
	mov	bx,ax
	shl	ax,cl
	shl	dx,cl
	neg	cl
	add	cl,10
	shr	bx,cl
	or	dx,bx
	retf

l0800_8C81:
	sub	cl,10
	xchg	ax,dx
	xor	ax,ax
	shl	dx,cl
	retf

;; fn0800_8C8A: 0800:8C8A
;;   Called from:
;;     0800:1626 (in fn0800_12E2)
;;     0800:1682 (in fn0800_12E2)
;;     0800:1FFD (in fn0800_1F5C)
;;     0800:25EF (in fn0800_24FE)
;;     0800:273D (in fn0800_2688)
;;     0800:2765 (in fn0800_2688)
;;     0800:279A (in fn0800_2688)
;;     0800:4A98 (in fn0800_46FE)
fn0800_8C8A proc
	pop	bx
	push	cs
	push	bx
	cmp	cl,10
	jnc	8CA2

l0800_8C92:
	mov	bx,dx
	shr	ax,cl
	sar	dx,cl
	neg	cl
	add	cl,10
	shl	bx,cl
	or	ax,bx
	retf

l0800_8CA2:
	sub	cl,10
	xchg	ax,dx
	cwd
	sar	ax,cl
	retf

;; fn0800_8CAA: 0800:8CAA
;;   Called from:
;;     0800:0F0F (in fn0800_0DE8)
;;     0800:11EF (in fn0800_112D)
;;     0800:173B (in fn0800_12E2)
;;     0800:17E5 (in fn0800_12E2)
;;     0800:3A4B (in fn0800_3992)
;;     0800:478B (in fn0800_46FE)
;;     0800:4B2A (in fn0800_46FE)
;;     0800:4FA5 (in fn0800_4F2C)
;;     0800:4FC5 (in fn0800_4F2C)
;;     0800:50F6 (in fn0800_4F2C)
;;     0800:5248 (in fn0800_51A9)
;;     0800:6141 (in fn0800_5E64)
;;     0800:631F (in fn0800_5E64)
;;     0800:6354 (in fn0800_5E64)
;;     0800:643A (in fn0800_5E64)
;;     0800:65B9 (in fn0800_5E64)
;;     0800:6603 (in fn0800_5E64)
;;     0800:7287 (in fn0800_6F20)
;;     0800:72A0 (in fn0800_6F20)
;;     0800:731E (in fn0800_6F20)
fn0800_8CAA proc
	pop	bx
	push	cs
	push	bx
	cmp	cl,10
	jnc	8CC2

l0800_8CB2:
	mov	bx,dx
	shr	ax,cl
	shr	dx,cl
	neg	cl
	add	cl,10
	shl	bx,cl
	or	ax,bx
	retf

l0800_8CC2:
	sub	cl,10
	xchg	ax,dx
	xor	dx,dx
	shr	ax,cl
	retf

;; fn0800_8CCB: 0800:8CCB
;;   Called from:
;;     0800:A253 (in fn0800_A215)
;;     0800:AC02 (in fn0800_ABA3)
;;     0800:ADF8 (in fn0800_AD85)
fn0800_8CCB proc
	pop	es
	push	cs
	push	es
	or	cx,cx
	jge	8CDE

l0800_8CD2:
	not	bx
	not	cx
	add	bx,01
	adc	cx,00
	jmp	8D0D

l0800_8CDE:
	add	ax,bx
	jnc	8CE6

l0800_8CE2:
	add	dx,1000

l0800_8CE6:
	mov	ch,cl
	mov	cl,04
	shl	ch,cl
	add	dh,ch
	mov	ch,al
	shr	ax,cl
	add	dx,ax
	mov	al,ch
	and	ax,000F
	retf
0800:8CFA                               07 0E 06 0B C9 7D           .....}
0800:8D00 0C F7 D3 F7 D1 83 C3 01 83 D1 00 EB D1          .............  

l0800_8D0D:
	sub	ax,bx
	jnc	8D15

l0800_8D11:
	sub	dx,1000

l0800_8D15:
	mov	bh,cl
	mov	cl,04
	shl	bh,cl
	xor	bl,bl
	sub	dx,bx
	mov	ch,al
	shr	ax,cl
	add	dx,ax
	mov	al,ch
	and	ax,000F
	retf

;; fn0800_8D2B: 0800:8D2B
;;   Called from:
;;     0800:8D6C (in fn0800_8D64)
;;     0800:8E4C (in fn0800_8E29)
;;     0800:8E65 (in fn0800_8E52)
;;     0800:8F7A (in fn0800_8F50)
;;     0800:8F92 (in fn0800_8F7F)
;;     0800:980B (in fn0800_97F8)
;;     0800:A2C7 (in fn0800_A2A3)
;;     0800:A417 (in fn0800_A401)
;;     0800:A554 (in fn0800_A53C)
;;     0800:A569 (in fn0800_A559)
;;     0800:A598 (in fn0800_A57F)
;;     0800:A60D (in fn0800_A59D)
;;     0800:A845 (in fn0800_A817)
;;     0800:A872 (in fn0800_A84A)
;;     0800:B127 (in fn0800_B113)
;;     0800:B183 (in fn0800_B140)
;;     0800:B197 (in fn0800_B140)
;;     0800:B2E8 (in fn0800_B2A0)
;;     0800:B994 (in fn0800_B97F)
;;     0800:BA61 (in fn0800_BA4A)
;;     0800:C648 (in fn0800_C632)
;;     0800:C7B0 (in fn0800_C779)
fn0800_8D2B proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+04]
	or	si,si
	jl	8D4B

l0800_8D36:
	cmp	si,58
	jle	8D3E

l0800_8D3B:
	mov	si,0057

l0800_8D3E:
	mov	[2516],si
	mov	al,[si+2518]
	cbw
	mov	si,ax
	jmp	8D58

l0800_8D4B:
	neg	si
	cmp	si,23
	jg	8D3B

l0800_8D52:
	mov	word ptr [2516],FFFF

l0800_8D58:
	mov	[007F],si
	mov	ax,FFFF
	pop	si
	pop	bp
	ret	0002

;; fn0800_8D64: 0800:8D64
;;   Called from:
;;     0800:8AEA (in fn0800_8ACF)
fn0800_8D64 proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+04]
	push	si
	call	8D2B
	mov	ax,si
	pop	si
	pop	bp
	ret	0002

;; fn0800_8D76: 0800:8D76
;;   Called from:
;;     0800:A9D6 (in fn0800_A96D)
fn0800_8D76 proc
	push	bp
	mov	bp,sp
	mov	ax,4400
	mov	bx,[bp+04]
	int	21
	xchg	ax,dx
	and	ax,0080
	pop	bp
	ret

;; fn0800_8D87: 0800:8D87
;;   Called from:
;;     0800:8E22 (in fn0800_8E09)
;;     0800:9A43 (in fn0800_9828)
fn0800_8D87 proc
	push	bp
	mov	bp,sp
	sub	sp,22
	push	si
	push	di
	push	es
	les	di,[bp+0A]
	mov	bx,[bp+08]
	cmp	bx,24
	ja	8DF7

l0800_8D9B:
	cmp	bl,02
	jc	8DF7

l0800_8DA0:
	mov	ax,[bp+0E]
	mov	cx,[bp+10]
	or	cx,cx
	jge	8DBC

l0800_8DAA:
	cmp	byte ptr [bp+06],00
	jz	8DBC

l0800_8DB0:
	mov	byte ptr es:[di],2D
	inc	di
	neg	cx
	neg	ax
	sbb	cx,00

l0800_8DBC:
	lea	si,[bp-22]
	jcxz	8DD1

l0800_8DC1:
	xchg	ax,cx
	sub	dx,dx
	div	bx
	xchg	ax,cx
	div	bx
	mov	ss:[si],dl
	inc	si
	jcxz	8DD9

l0800_8DCF:
	jmp	8DC1

l0800_8DD1:
	sub	dx,dx
	div	bx
	mov	ss:[si],dl
	inc	si

l0800_8DD9:
	or	ax,ax
	jnz	8DD1

l0800_8DDD:
	lea	cx,[bp-22]
	neg	cx
	add	cx,si
	cld

l0800_8DE5:
	dec	si
	mov	al,ss:[si]
	sub	al,0A
	jnc	8DF1

l0800_8DED:
	add	al,3A
	jmp	8DF4

l0800_8DF1:
	add	al,[bp+04]

l0800_8DF4:
	stosb
	loop	8DE5

l0800_8DF7:
	mov	al,00
	stosb
	pop	es
	mov	dx,[bp+0C]
	mov	ax,[bp+0A]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	000E

;; fn0800_8E09: 0800:8E09
;;   Called from:
;;     0800:8EA5 (in fn0800_8E6A)
fn0800_8E09 proc
	push	bp
	mov	bp,sp
	xor	ax,ax
	push	ax
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	mov	ax,000A
	push	ax
	mov	al,00
	push	ax
	mov	al,61
	push	ax
	call	8D87
	pop	bp
	ret	0006

;; fn0800_8E29: 0800:8E29
;;   Called from:
;;     0800:97C4 (in fn0800_97B6)
;;     0800:AD15 (in fn0800_ACB3)
;;     0800:B446 (in fn0800_B324)
;;     0800:B55A (in fn0800_B4BE)
;;     0800:B600 (in fn0800_B4BE)
;;     0800:BA2F (in fn0800_B97F)
;;     0800:C671 (in fn0800_C632)
fn0800_8E29 proc
	push	bp
	mov	bp,sp
	mov	bx,[bp+04]
	shl	bx,01
	and	word ptr [bx+24EA],FDFF
	mov	ah,42
	mov	al,[bp+0A]
	mov	bx,[bp+04]
	mov	cx,[bp+08]
	mov	dx,[bp+06]
	int	21
	jc	8E4B

l0800_8E49:
	jmp	8E50

l0800_8E4B:
	push	ax
	call	8D2B
	cwd

l0800_8E50:
	pop	bp
	ret

;; fn0800_8E52: 0800:8E52
;;   Called from:
;;     0800:3704 (in fn0800_3678)
fn0800_8E52 proc
	push	bp
	mov	bp,sp
	push	ds
	mov	ah,39
	lds	dx,[bp+04]
	int	21
	pop	ds
	jc	8E64

l0800_8E60:
	xor	ax,ax
	jmp	8E68

l0800_8E64:
	push	ax
	call	8D2B

l0800_8E68:
	pop	bp
	ret

;; fn0800_8E6A: 0800:8E6A
;;   Called from:
;;     0800:A69F (in fn0800_A614)
fn0800_8E6A proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+0A]
	or	ax,[bp+0C]
	jnz	8E7D

l0800_8E75:
	mov	[bp+0C],ds
	mov	word ptr [bp+0A],4ED6

l0800_8E7D:
	push	word ptr [bp+04]
	mov	ax,[bp+06]
	or	ax,[bp+08]
	jnz	8E8F

l0800_8E88:
	mov	dx,ds
	mov	ax,2572
	jmp	8E95

l0800_8E8F:
	mov	dx,[bp+08]
	mov	ax,[bp+06]

l0800_8E95:
	push	dx
	push	ax
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	call	9CE6
	add	sp,08
	push	dx
	push	ax
	call	8E09
	push	ds
	mov	ax,2576
	push	ax
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	call	BF5F
	add	sp,08
	mov	dx,[bp+0C]
	mov	ax,[bp+0A]
	pop	bp
	ret	000A
0800:8EC3          55 8B EC 83 EC 02 FF 76 0A FF 76 08 33    U......v..v.3
0800:8ED0 C0 50 50 C4 5E 04 26 83 3F FF 75 05 B8 02 00 EB .PP.^.&.?.u.....
0800:8EE0 03 B8 01 00 C4 5E 04 26 01 07 26 8B 07 50 E8 79 .....^.&..&..P.y
0800:8EF0 FF 89 56 0A 89 46 08 16 8D 46 FE 50 FF 76 0A FF ..V..F...F.P.v..
0800:8F00 76 08 E8 CA FB 83 C4 08 0B C0 74 BD 8B 56 0A 8B v.........t..V..
0800:8F10 46 08 8B E5 5D C2 08 00                         F...]...       

;; fn0800_8F18: 0800:8F18
;;   Called from:
;;     0800:0BFE (in fn0800_0B79)
;;     0800:81CE (in fn0800_7FDC)
;;     0800:ABBF (in fn0800_ABA3)
;;     0800:AD9D (in fn0800_AD85)
;;     0800:C088 (in fn0800_C04F)
;;     0800:C0A1 (in fn0800_C04F)
;;     0800:C134 (in fn0800_C04F)
;;     0800:C14F (in fn0800_C04F)
;;     0800:C4B5 (in fn0800_C379)
fn0800_8F18 proc
	push	si
	xchg	ax,si
	xchg	ax,dx
	test	ax,ax
	jz	8F21

l0800_8F1F:
	mul	bx

l0800_8F21:
	jcxz	8F28

l0800_8F23:
	xchg	ax,cx
	mul	si
	add	ax,cx

l0800_8F28:
	xchg	ax,si
	mul	bx
	add	dx,si
	pop	si
	ret

;; fn0800_8F2F: 0800:8F2F
;;   Called from:
;;     0800:A1E7 (in fn0800_A1D6)
;;     0800:A1FA (in fn0800_A1D6)
;;     0800:A267 (in fn0800_A215)
;;     0800:A27A (in fn0800_A215)
;;     0800:B0B3 (in fn0800_B0A1)
fn0800_8F2F proc
	push	cx
	mov	ch,al
	mov	cl,04
	shr	ax,cl
	add	dx,ax
	mov	al,ch
	mov	ah,bl
	shr	bx,cl
	pop	cx
	add	cx,bx
	mov	bl,ah
	and	ax,000F
	and	bx,0F
	cmp	dx,cx
	jnz	8F4F

l0800_8F4D:
	cmp	ax,bx

l0800_8F4F:
	ret

;; fn0800_8F50: 0800:8F50
;;   Called from:
;;     0800:AB27 (in fn0800_AAB3)
;;     0800:B9C1 (in fn0800_B97F)
;;     0800:BA0C (in fn0800_B97F)
fn0800_8F50 proc
	push	bp
	mov	bp,sp
	mov	bx,[bp+04]
	shl	bx,01
	test	word ptr [bx+24EA],0002
	jz	8F66

l0800_8F60:
	mov	ax,0005
	push	ax
	jmp	8F7A

l0800_8F66:
	push	ds
	mov	ah,3F
	mov	bx,[bp+04]
	mov	cx,[bp+0A]
	lds	dx,[bp+06]
	int	21
	pop	ds
	jc	8F79

l0800_8F77:
	jmp	8F7D

l0800_8F79:
	push	ax

l0800_8F7A:
	call	8D2B

l0800_8F7D:
	pop	bp
	ret

;; fn0800_8F7F: 0800:8F7F
;;   Called from:
;;     0800:0DC6 (in fn0800_0DA9)
;;     0800:10DD (in fn0800_0DE8)
;;     0800:1119 (in fn0800_0DE8)
;;     0800:12AB (in fn0800_112D)
;;     0800:18BC (in fn0800_12E2)
;;     0800:1CA2 (in fn0800_19EE)
;;     0800:1CEA (in fn0800_19EE)
;;     0800:385D (in fn0800_37DF)
;;     0800:394F (in fn0800_388C)
;;     0800:3C8F (in fn0800_3BC3)
;;     0800:3D88 (in fn0800_3C99)
fn0800_8F7F proc
	push	bp
	mov	bp,sp
	push	ds
	mov	ah,41
	lds	dx,[bp+04]
	int	21
	pop	ds
	jc	8F91

l0800_8F8D:
	xor	ax,ax
	jmp	8F95

l0800_8F91:
	push	ax
	call	8D2B

l0800_8F95:
	pop	bp
	ret

;; fn0800_8F97: 0800:8F97
;;   Called from:
;;     0800:BF33 (in fn0800_BF18)
fn0800_8F97 proc
	push	bp
	mov	bp,sp
	sub	sp,2A
	push	si
	push	di
	mov	word ptr [bp-04],0000
	mov	word ptr [bp-06],0000
	jmp	8FC6

;; fn0800_8FAB: 0800:8FAB
;;   Called from:
;;     0800:9110 (in fn0800_8F97)
;;     0800:91D3 (in fn0800_8F97)
;;     0800:9275 (in fn0800_8F97)
;;     0800:9327 (in fn0800_8F97)
fn0800_8FAB proc
	les	di,[bp+10]
	test	byte ptr [bp-01],20
	jz	8FBC

l0800_8FB4:
	les	di,es:[di]
	add	word ptr [bp+10],04
	ret

l0800_8FBC:
	mov	di,es:[di]
	push	ds
	pop	es
	add	word ptr [bp+10],02
	ret

l0800_8FC6:
	push	es
	cld

l0800_8FC8:
	mov	si,[bp+0C]

l0800_8FCB:
	mov	es,[bp+0E]
	lodsb
	or	al,al
	jz	9042

l0800_8FD4:
	cmp	al,25
	jz	9045

l0800_8FD8:
	cbw
	xchg	ax,di
	inc	word ptr [bp-06]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	or	ax,ax
	jl	9016

l0800_8FEC:
	cbw
	or	di,di
	js	902B

l0800_8FF1:
	cmp	byte ptr [di+257C],01
	jnz	902B

l0800_8FF8:
	xchg	ax,bx
	or	bl,bl
	js	9019

l0800_8FFD:
	cmp	byte ptr [bx+257C],01
	jnz	9019

l0800_9004:
	inc	word ptr [bp-06]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	or	ax,ax
	jg	8FF8

l0800_9016:
	jmp	93A6

l0800_9019:
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	bx
	call	word ptr [bp+06]
	add	sp,06
	dec	word ptr [bp-06]
	jmp	8FCB

l0800_902B:
	cmp	ax,di
	jz	8FCB

l0800_902F:
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	dec	word ptr [bp-06]
	jmp	93BE

l0800_9042:
	jmp	93BE

l0800_9045:
	mov	word ptr [bp-0A],FFFF
	mov	es,[bp+0E]
	mov	byte ptr [bp-01],20

l0800_9051:
	lodsb
	cbw
	mov	[bp+0C],si
	xchg	ax,di
	or	di,di
	jl	9075

l0800_905C:
	mov	bl,[di+257C]
	xor	bh,bh
	cmp	bx,15
	jbe	906A

l0800_9067:
	jmp	93A6

l0800_906A:
	shl	bx,01
	jmp	word ptr cs:[bx+9459]

l0800_9071:
	xchg	ax,di
	jmp	8FD8

l0800_9075:
	jmp	93BE

l0800_9078:
	or	byte ptr [bp-01],01
	jmp	9051

l0800_907E:
	sub	di,30
	xchg	[bp-0A],di
	or	di,di
	jl	9051

l0800_9088:
	mov	ax,000A
	mul	di
	add	[bp-0A],ax
	jmp	9051

l0800_9092:
	or	byte ptr [bp-01],08
	jmp	9051

l0800_9098:
	or	byte ptr [bp-01],04
	jmp	9051

l0800_909E:
	or	byte ptr [bp-01],02
	jmp	9051

l0800_90A4:
	and	byte ptr [bp-01],DF
	jmp	9051

l0800_90AA:
	or	byte ptr [bp-01],20
	jmp	9051

l0800_90B0:
	mov	ax,[bp-06]
	sub	dx,dx
	test	byte ptr [bp-01],01
	jz	9110

l0800_90BB:
	jmp	9051

l0800_90BD:
	mov	si,0008
	jmp	90CE

l0800_90C2:
	mov	si,000A
	jmp	90CE

l0800_90C7:
	mov	si,0010
	jmp	90CE

l0800_90CC:
	xor	si,si

l0800_90CE:
	test	di,0020
	jnz	90DD

l0800_90D4:
	cmp	di,58
	jz	90DD

l0800_90D9:
	or	byte ptr [bp-01],04

l0800_90DD:
	push	ss
	lea	ax,[bp-08]
	push	ax
	push	ss
	lea	ax,[bp-06]
	push	ax
	mov	ax,[bp-0A]
	and	ax,7FFF
	push	ax
	push	si
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	94B0
	add	sp,14
	cmp	word ptr [bp-08],00
	jle	911F

l0800_9107:
	test	byte ptr [bp-01],01
	jnz	911C

l0800_910D:
	inc	word ptr [bp-04]

l0800_9110:
	call	8FAB
	stosw
	test	byte ptr [bp-01],04
	jz	911C

l0800_911A:
	xchg	ax,dx
	stosw

l0800_911C:
	jmp	8FC8

l0800_911F:
	jl	9124

l0800_9121:
	jmp	93BE

l0800_9124:
	jmp	93A6

l0800_9127:
	call	912A

;; fn0800_912A: 0800:912A
;;   Called from:
;;     0800:9127 (in fn0800_8F97)
;;     0800:9127 (in fn0800_8F97)
fn0800_912A proc
	jmp	93C5
0800:912D                                        FF 76 0A              .v.
0800:9130 FF 76 08 50 FF 56 06 83 C4 06 FF 4E FA 81 66 F6 .v.P.V.....N..f.
0800:9140 FF 7F E8 00 00                                  .....          

;; fn0800_9145: 0800:9145
fn0800_9145 proc
	jmp	93EF
0800:9148                         52 3C 3A 74 19 0B C0 7E         R<:t...~
0800:9150 10 FF 76 0A FF 76 08 50 FF 56 06 83 C4 06 FF 4E ..v..v.P.V.....N
0800:9160 FA 5A 8C DB EB 1F E8 00 00                      .Z.......      

;; fn0800_9169: 0800:9169
fn0800_9169 proc
	jmp	93EF
0800:916C                                     5B 0B C0 7E             [..~
0800:9170 14 52 53 FF 76 0A FF 76 08 50 FF 56 06 83 C4 06 .RS.v..v.P.V....
0800:9180 FF 4E FA 5B 5A F6 46 FF 01 75 10 E8 1D FE FF 46 .N.[Z.F..u.....F
0800:9190 FC 92 AB F6 46 FF 20 74 02 93 AB E9 2A FE       ....F. t....*. 

l0800_919E:
	jmp	93A6

l0800_91A1:
	push	ss
	lea	ax,[bp-08]
	push	ax
	push	ss
	lea	ax,[bp-06]
	push	ax
	mov	ax,7FFF
	and	ax,[bp-0A]
	push	ax
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A2D0
	add	sp,12
	cmp	word ptr [bp-08],00
	jle	9203

l0800_91CA:
	mov	al,[bp-01]
	cbw
	test	ax,0001
	jnz	91FD

l0800_91D3:
	call	8FAB
	inc	word ptr [bp-04]
	test	byte ptr [bp-01],04
	jz	91E4

l0800_91DF:
	mov	ax,0004
	jmp	91F1

l0800_91E4:
	test	byte ptr [bp-01],08
	jz	91EF

l0800_91EA:
	mov	ax,0008
	jmp	91F1

l0800_91EF:
	xor	ax,ax

l0800_91F1:
	push	ax
	push	es
	push	di
	call	A2D4
	add	sp,06
	jmp	8FC8

l0800_91FD:
	call	A2D8
	jmp	8FC8

l0800_9203:
	call	A2D8
	jl	919E

l0800_9208:
	jmp	93BE

l0800_920B:
	call	920E

;; fn0800_920E: 0800:920E
;;   Called from:
;;     0800:920B (in fn0800_8F97)
;;     0800:920B (in fn0800_8F97)
fn0800_920E proc
	jmp	93C5
0800:9211    F6 46 FF 01 75 06 E8 91 FD FF 46 FC 81 66 F6  .F..u.....F..f.
0800:9220 FF 7F 74 2D F6 46 FF 01 75 01 AA FF 46 FA 06 FF ..t-.F..u...F...
0800:9230 76 0A FF 76 08 FF 56 04 59 59 07 0B C0 7E 12 0A v..v..V.YY...~..
0800:9240 C0 78 09 93 80 BF 7C 25 01 93 7E 05 FF 4E F6 7F .x....|%..~..N..
0800:9250 D3 06 FF 76 0A FF 76 08 50 FF 56 06 83 C4 06 07 ...v..v.P.V.....
0800:9260 FF 4E FA F6 46 FF 01 75 03 B0 00 AA E9 59 FD    .N..F..u.....Y.

l0800_926F:
	test	byte ptr [bp-01],01
	jnz	9278

l0800_9275:
	call	8FAB

l0800_9278:
	mov	si,[bp-0A]
	or	si,si
	jge	9282

l0800_927F:
	mov	si,0001

l0800_9282:
	jz	92A2

l0800_9284:
	inc	word ptr [bp-06]
	push	es
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	pop	es
	or	ax,ax
	jl	92AE

l0800_9298:
	test	byte ptr [bp-01],01
	jnz	929F

l0800_929E:
	stosb

l0800_929F:
	dec	si
	jg	9284

l0800_92A2:
	test	byte ptr [bp-01],01
	jnz	92AB

l0800_92A8:
	inc	word ptr [bp-04]

l0800_92AB:
	jmp	8FC8

l0800_92AE:
	jmp	93A6

l0800_92B1:
	push	es
	sub	ax,ax
	cld
	push	ss
	pop	es
	lea	di,[bp-2A]
	mov	cx,0010

l0800_92BD:
	rep stosw

l0800_92BF:
	pop	es
	lodsb
	and	byte ptr [bp-01],EF
	cmp	al,5E
	jnz	92D0

l0800_92CA:
	or	byte ptr [bp-01],10
	lodsb

l0800_92D0:
	mov	ah,00

l0800_92D2:
	mov	dl,al
	mov	di,ax
	mov	cl,03
	shr	di,cl
	mov	cx,0107
	and	cl,dl
	shl	ch,cl
	or	[bp+di-2A],ch

l0800_92E4:
	lodsb
	cmp	al,00
	jz	9313

l0800_92EA:
	cmp	al,5D
	jz	9316

l0800_92EE:
	cmp	al,2D
	jnz	92D2

l0800_92F2:
	cmp	dl,es:[si]
	ja	92D2

l0800_92F7:
	cmp	byte ptr es:[si],5D
	jz	92D2

l0800_92FD:
	lodsb
	sub	al,dl
	jz	92E4

l0800_9303:
	add	dl,al

l0800_9305:
	rol	ch,01
	adc	di,00
	or	[bp+di-2A],ch
	dec	al
	jnz	9305

l0800_9311:
	jmp	92E4

l0800_9313:
	jmp	93BE

l0800_9316:
	mov	[bp+0C],si
	and	word ptr [bp-0A],7FFF
	mov	si,[bp-0A]
	test	byte ptr [bp-01],01
	jnz	932A

l0800_9327:
	call	8FAB

l0800_932A:
	dec	si
	jl	9385

l0800_932D:
	inc	word ptr [bp-06]
	push	es
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	pop	es
	or	ax,ax
	jl	9394

l0800_9341:
	xchg	ax,si
	mov	bx,si
	mov	cl,03
	shr	si,cl
	mov	cx,0107
	and	cl,bl
	shl	ch,cl
	test	[bp+si-2A],ch
	xchg	ax,si
	xchg	ax,bx
	jz	935E

l0800_9356:
	test	byte ptr [bp-01],10
	jz	9364

l0800_935C:
	jmp	936D

l0800_935E:
	test	byte ptr [bp-01],10
	jz	936D

l0800_9364:
	test	byte ptr [bp-01],01
	jnz	932A

l0800_936A:
	stosb
	jmp	932A

l0800_936D:
	push	es
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	pop	es
	dec	word ptr [bp-06]
	inc	si
	cmp	si,[bp-0A]
	jge	938E

l0800_9385:
	test	byte ptr [bp-01],01
	jnz	9391

l0800_938B:
	inc	word ptr [bp-04]

l0800_938E:
	mov	al,00
	stosb

l0800_9391:
	jmp	8FC8

l0800_9394:
	inc	si
	cmp	si,[bp-0A]
	jge	93A6

l0800_939A:
	test	byte ptr [bp-01],01
	jnz	93A6

l0800_93A0:
	mov	al,00
	stosb
	inc	word ptr [bp-04]

;; fn0800_93A6: 0800:93A6
;;   Called from:
;;     0800:9016 (in fn0800_8F97)
;;     0800:903F (in fn0800_8F97)
;;     0800:9042 (in fn0800_8F97)
;;     0800:9067 (in fn0800_8F97)
;;     0800:906C (in fn0800_8F97)
;;     0800:906C (in fn0800_8F97)
;;     0800:906C (in fn0800_8F97)
;;     0800:9075 (in fn0800_8F97)
;;     0800:9124 (in fn0800_8F97)
;;     0800:919E (in fn0800_8F97)
;;     0800:92AE (in fn0800_8F97)
;;     0800:9398 (in fn0800_8F97)
;;     0800:939E (in fn0800_8F97)
;;     0800:93A3 (in fn0800_8F97)
;;     0800:93ED (in fn0800_93C5)
;;     0800:9450 (in fn0800_93EF)
fn0800_93A6 proc
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	mov	ax,FFFF
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	cmp	word ptr [bp-04],01
	sbb	word ptr [bp-04],00

;; fn0800_93BE: 0800:93BE
;;   Called from:
;;     0800:9121 (in fn0800_8F97)
;;     0800:9208 (in fn0800_8F97)
;;     0800:9313 (in fn0800_8F97)
;;     0800:93BA (in fn0800_93A6)
fn0800_93BE proc
	pop	es
	mov	ax,[bp-04]
	jmp	9453

;; fn0800_93C5: 0800:93C5
;;   Called from:
;;     0800:912A (in fn0800_912A)
;;     0800:920E (in fn0800_920E)
fn0800_93C5 proc
	inc	word ptr [bp-06]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	or	ax,ax
	jle	93EA

l0800_93D7:
	or	al,al
	js	93E4

l0800_93DB:
	xchg	ax,bx
	cmp	byte ptr [bx+257C],01
	xchg	ax,bx
	jz	93C5

l0800_93E4:
	pop	cx
	add	cx,03
	jmp	cx

l0800_93EA:
	jz	93E4

l0800_93EC:
	pop	cx
	jmp	93A6

;; fn0800_93EF: 0800:93EF
;;   Called from:
;;     0800:9145 (in fn0800_9145)
;;     0800:9169 (in fn0800_9169)
fn0800_93EF proc
	sub	dx,dx
	mov	cx,0004

l0800_93F4:
	dec	word ptr [bp-0A]
	jl	9442

l0800_93F9:
	push	dx
	push	cx
	inc	word ptr [bp-06]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	pop	cx
	pop	dx
	or	ax,ax
	jle	9444

l0800_940F:
	dec	cl
	jl	9444

l0800_9413:
	mov	ch,al
	sub	ch,30
	jc	9444

l0800_941A:
	cmp	ch,0A
	jc	9436

l0800_941F:
	sub	ch,11
	jc	9444

l0800_9424:
	cmp	ch,06
	jc	9433

l0800_9429:
	sub	ch,20
	jc	9444

l0800_942E:
	cmp	ch,06
	jnc	9444

l0800_9433:
	add	ch,0A

l0800_9436:
	shl	dx,01
	shl	dx,01
	shl	dx,01
	shl	dx,01
	add	dl,ch
	jmp	93F4

l0800_9442:
	sub	ax,ax

l0800_9444:
	cmp	cl,04
	jz	944F

l0800_9449:
	pop	cx
	add	cx,03
	jmp	cx

l0800_944F:
	pop	cx
	jmp	93A6

l0800_9453:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret
l0800_9459	dw	0x93BE
l0800_945B	dw	0x93BE
l0800_945D	dw	0x93BE
l0800_945F	dw	0x9071
l0800_9461	dw	0x9078
l0800_9463	dw	0x907E
l0800_9465	dw	0x926F
l0800_9467	dw	0x90C2
l0800_9469	dw	0x90C2
l0800_946B	dw	0x90CC
l0800_946D	dw	0x91A1
l0800_946F	dw	0x9092
l0800_9471	dw	0x909E
l0800_9473	dw	0x9098
l0800_9475	dw	0x90BD
l0800_9477	dw	0x920B
l0800_9479	dw	0x92B1
l0800_947B	dw	0x90B0
l0800_947D	dw	0x90C7
l0800_947F	dw	0x9127
l0800_9481	dw	0x90A4
l0800_9483	dw	0x90AA

;; fn0800_9485: 0800:9485
;;   Called from:
;;     0800:95A2 (in fn0800_94B0)
;;     0800:95CC (in fn0800_94B0)
;;     0800:95FC (in fn0800_94B0)
fn0800_9485 proc
	push	bx
	sub	bl,30
	jc	94AD

l0800_948B:
	cmp	bl,09
	jbe	94A2

l0800_9490:
	cmp	bl,2A
	ja	949A

l0800_9495:
	sub	bl,07
	jmp	949D

l0800_949A:
	sub	bl,27

l0800_949D:
	cmp	bl,09
	jbe	94AD

l0800_94A2:
	cmp	bl,cl
	jnc	94AD

l0800_94A6:
	inc	sp
	inc	sp
	clc
	mov	bh,00
	jmp	94AF

l0800_94AD:
	pop	bx
	stc

l0800_94AF:
	ret

;; fn0800_94B0: 0800:94B0
;;   Called from:
;;     0800:90FB (in fn0800_8F97)
fn0800_94B0 proc
	push	bp
	mov	bp,sp
	sub	sp,06
	push	si
	push	di
	mov	byte ptr [bp-01],00
	mov	word ptr [bp-04],0000
	mov	word ptr [bp-06],0001

l0800_94C6:
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	or	ax,ax
	jl	9549

l0800_94D8:
	cbw
	xchg	ax,bx
	test	bl,80
	jnz	94E7

l0800_94DF:
	mov	di,2251
	test	byte ptr [bx+di],01
	jnz	94C6

l0800_94E7:
	xchg	ax,bx
	dec	word ptr [bp+0E]
	jl	9550

l0800_94ED:
	cmp	al,2B
	jz	94F8

l0800_94F1:
	cmp	al,2D
	jnz	950F

l0800_94F5:
	inc	byte ptr [bp-01]

l0800_94F8:
	dec	word ptr [bp+0E]
	jl	9550

l0800_94FD:
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	or	ax,ax
	jl	9549

l0800_950F:
	sub	si,si
	mov	di,si
	mov	cx,[bp+0C]
	jcxz	956E

l0800_9518:
	cmp	cx,24
	ja	9550

l0800_951D:
	cmp	cl,02
	jc	9550

l0800_9522:
	cmp	al,30
	jnz	959E

l0800_9526:
	cmp	cl,10
	jnz	959C

l0800_952B:
	dec	word ptr [bp+0E]
	jl	956B

l0800_9530:
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	cmp	al,78
	jz	959C

l0800_9542:
	cmp	al,58
	jz	959C

l0800_9546:
	jmp	95C8

l0800_9549:
	mov	word ptr [bp-06],FFFF
	jmp	9555

l0800_9550:
	mov	word ptr [bp-06],0000

l0800_9555:
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	dec	word ptr [bp-04]
	sub	ax,ax
	cwd
	jmp	9622

l0800_956B:
	jmp	9612

l0800_956E:
	cmp	al,30
	mov	word ptr [bp+0C],000A
	jnz	959E

l0800_9577:
	dec	word ptr [bp+0E]
	jl	956B

l0800_957C:
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	mov	word ptr [bp+0C],0008
	cmp	al,78
	jz	9597

l0800_9593:
	cmp	al,58
	jnz	95C8

l0800_9597:
	mov	word ptr [bp+0C],0010

l0800_959C:
	jmp	95B5

l0800_959E:
	mov	cx,[bp+0C]
	xchg	ax,bx
	call	9485
	xchg	ax,bx
	jc	9550

l0800_95A8:
	xchg	ax,si
	jmp	95B5

l0800_95AB:
	xchg	ax,si
	mul	word ptr [bp+0C]
	add	si,ax
	adc	di,dx
	jnz	95E5

l0800_95B5:
	dec	word ptr [bp+0E]
	jl	9612

l0800_95BA:
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx

l0800_95C8:
	mov	cx,[bp+0C]
	xchg	ax,bx
	call	9485
	xchg	ax,bx
	jnc	95AB

l0800_95D2:
	jmp	9602

l0800_95D4:
	xchg	ax,si
	mul	cx
	xchg	ax,di
	xchg	dx,cx
	mul	dx
	add	si,di
	adc	ax,cx
	xchg	ax,di
	adc	dl,dh
	jnz	9636

l0800_95E5:
	dec	word ptr [bp+0E]
	jl	9612

l0800_95EA:
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	mov	cx,[bp+0C]
	xchg	ax,bx
	call	9485
	xchg	ax,bx
	jnc	95D4

l0800_9602:
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	dec	word ptr [bp-04]

l0800_9612:
	mov	dx,di
	xchg	ax,si
	cmp	byte ptr [bp-01],00
	jz	9622

l0800_961B:
	neg	dx
	neg	ax
	sbb	dx,00

l0800_9622:
	les	di,[bp+10]
	mov	bx,[bp-04]
	add	es:[di],bx
	les	di,[bp+14]
	mov	bx,[bp-06]
	mov	es:[di],bx
	jmp	964C

l0800_9636:
	mov	ax,FFFF
	mov	dx,7FFF
	add	al,[bp-01]
	adc	ah,00
	adc	dx,00
	mov	word ptr [bp-06],0002
	jmp	9622

l0800_964C:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_9652: 0800:9652
fn0800_9652 proc
	push	bp
	mov	bp,sp
	push	si
	mov	ah,2B
	les	si,[bp+04]
	mov	cx,es:[si]
	mov	dx,es:[si+02]
	int	21
	pop	si
	pop	bp
	ret

;; fn0800_9667: 0800:9667
fn0800_9667 proc
	push	bp
	mov	bp,sp
	push	si
	mov	ah,2D
	les	si,[bp+04]
	mov	cx,es:[si]
	mov	dx,es:[si+02]
	int	21
	pop	si
	pop	bp
	ret
0800:967C                                     B9 05 00 3B             ...;
0800:9680 0E E8 24 73 38 8B D9 D1 E3 C7 87 EA 24 00 00 8B ..$s8.......$...
0800:9690 C1 BA 14 00 F7 EA 8B D8 C6 87 5C 23 FF 8B C1 BA ..........\#....
0800:96A0 14 00 F7 EA 05 58 23 50 8B C1 BA 14 00 F7 EA 8B .....X#P........
0800:96B0 D8 58 89 87 6A 23 41 3B 0E E8 24 72 C8 A0 5C 23 .X..j#A;..$r..\#
0800:96C0 98 50 E8 B1 F6 59 0B C0 75 06 81 26 5A 23 FF FD .P...Y..u..&Z#..
0800:96D0 B8 00 02 50 F7 06 5A 23 00 02 74 05 B8 01 00 EB ...P..Z#..t.....
0800:96E0 02 33 C0 50 33 C0 50 50 1E B8 58 23 50 E8 99 23 .3.P3.PP..X#P..#
0800:96F0 83 C4 0C A0 70 23 98 50 E8 7B F6 59 0B C0 75 06 ....p#.P.{.Y..u.
0800:9700 81 26 6E 23 FF FD B8 00 02 50 F7 06 6E 23 00 02 .&n#.....P..n#..
0800:9710 74 05 B8 02 00 EB 02 33 C0 50 33 C0 50 50 1E B8 t......3.P3.PP..
0800:9720 6C 23 50 E8 63 23 83 C4 0C C3 55 8B EC 83 EC 08 l#P.c#....U.....
0800:9730 16 8D 46 F8 50 16 8D 46 FC 50 C4 5E 04 26 FF 77 ..F.P..F.P.^.&.w
0800:9740 02 26 FF 37 E8 30 2A 83 C4 0C 16 8D 46 FC 50 E8 .&.7.0*.....F.P.
0800:9750 00 FF 59 59 16 8D 46 F8 50 E8 0B FF 59 59 33 C0 ..YY..F.P...YY3.
0800:9760 8B E5 5D C3                                     ..].           

;; fn0800_9764: 0800:9764
;;   Called from:
;;     0800:0305 (in main)
;;     0800:033C (in main)
fn0800_9764 proc
	push	bp
	mov	bp,sp
	sub	sp,0C
	push	ss
	lea	ax,[bp-04]
	push	ax
	call	8B95
	pop	cx
	pop	cx
	push	ss
	lea	ax,[bp-08]
	push	ax
	call	8BA8
	pop	cx
	pop	cx
	push	ss
	lea	ax,[bp-08]
	push	ax
	push	ss
	lea	ax,[bp-04]
	push	ax
	call	C04F
	add	sp,08
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	mov	ax,[bp+04]
	or	ax,[bp+06]
	jz	97AC

l0800_979C:
	les	bx,[bp+04]
	mov	ax,[bp-0A]
	mov	dx,[bp-0C]
	mov	es:[bx+02],ax
	mov	es:[bx],dx

l0800_97AC:
	mov	dx,[bp-0A]
	mov	ax,[bp-0C]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_97B6: 0800:97B6
;;   Called from:
;;     0800:AD3E (in fn0800_AD2F)
fn0800_97B6 proc
	push	bp
	mov	bp,sp
	mov	ax,0001
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+04]
	call	8E29
	add	sp,08
	pop	bp
	ret

;; fn0800_97CC: 0800:97CC
;;   Called from:
;;     0800:0C36 (in fn0800_0C29)
;;     0800:0C79 (in fn0800_0C6C)
fn0800_97CC proc
	push	bp
	mov	bp,sp
	mov	dx,[bp+04]
	cmp	dx,FF
	jnz	97DC

l0800_97D7:
	mov	ax,FFFF
	jmp	97F6

l0800_97DC:
	mov	al,dl
	mov	ah,00
	mov	bx,ax
	test	byte ptr [bx+2251],08
	jz	97F2

l0800_97E9:
	mov	al,dl
	mov	ah,00
	add	ax,FFE0
	jmp	97F6

l0800_97F2:
	mov	al,dl
	mov	ah,00

l0800_97F6:
	pop	bp
	ret

;; fn0800_97F8: 0800:97F8
;;   Called from:
;;     0800:A6A4 (in fn0800_A614)
fn0800_97F8 proc
	push	bp
	mov	bp,sp
	push	ds
	mov	ah,41
	lds	dx,[bp+04]
	int	21
	pop	ds
	jc	980A

l0800_9806:
	xor	ax,ax
	jmp	980E

l0800_980A:
	push	ax
	call	8D2B

l0800_980E:
	pop	bp
	ret

;; fn0800_9810: 0800:9810
;;   Called from:
;;     0800:9A7A (in fn0800_9828)
;;     0800:9A83 (in fn0800_9828)
fn0800_9810 proc
	mov	al,dh
	call	9817
	mov	al,dl

;; fn0800_9817: 0800:9817
;;   Called from:
;;     0800:9812 (in fn0800_9810)
;;     0800:9815 (in fn0800_9810)
fn0800_9817 proc
	aam	10
	xchg	al,ah
	call	9820
	xchg	al,ah

;; fn0800_9820: 0800:9820
;;   Called from:
;;     0800:981B (in fn0800_9817)
;;     0800:981E (in fn0800_9817)
fn0800_9820 proc
	add	al,90
	daa
	adc	al,40
	daa
	stosb
	ret

;; fn0800_9828: 0800:9828
;;   Called from:
;;     0800:B305 (in fn0800_B2EF)
;;     0800:BEBF (in fn0800_BEA2)
fn0800_9828 proc
	push	bp
	mov	bp,sp
	sub	sp,0096
	push	si
	push	di
	mov	word ptr [bp-12],0000
	mov	word ptr [bp-14],0050
	mov	word ptr [bp-16],0000
	jmp	988C

;; fn0800_9842: 0800:9842
;;   Called from:
;;     0800:9AF4 (in fn0800_9828)
;;     0800:9B54 (in fn0800_9828)
;;     0800:9B86 (in fn0800_9828)
fn0800_9842 proc
	push	di
	mov	cx,FFFF
	xor	al,al

l0800_9848:
	repne scasb

l0800_984A:
	not	cx
	dec	cx
	pop	di
	ret

;; fn0800_984F: 0800:984F
;;   Called from:
;;     0800:9BDA (in fn0800_9828)
;;     0800:9BEB (in fn0800_9828)
;;     0800:9BF1 (in fn0800_9828)
;;     0800:9C10 (in fn0800_9828)
;;     0800:9C1B (in fn0800_9828)
;;     0800:9C3E (in fn0800_9828)
;;     0800:9C8D (in fn0800_9828)
fn0800_984F proc
	mov	ss:[di],al
	inc	di
	dec	byte ptr [bp-14]
	jnz	988B

;; fn0800_9858: 0800:9858
;;   Called from:
;;     0800:9856 (in fn0800_984F)
;;     0800:98AE (in fn0800_9828)
;;     0800:9C31 (in fn0800_9828)
;;     0800:9C9C (in fn0800_9828)
fn0800_9858 proc
	push	bx
	push	cx
	push	dx
	push	es
	lea	ax,[bp+FF6A]
	sub	di,ax
	push	ss
	lea	ax,[bp+FF6A]
	push	ax
	push	di
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	call	word ptr [bp+0E]
	or	ax,ax
	jnz	987B

l0800_9876:
	mov	word ptr [bp-16],0001

l0800_987B:
	mov	word ptr [bp-14],0050
	add	[bp-12],di
	lea	di,[bp+FF6A]
	pop	es
	pop	dx
	pop	cx
	pop	bx

l0800_988B:
	ret

l0800_988C:
	push	es
	cld
	lea	di,[bp+FF6A]
	mov	[bp-04],di

l0800_9895:
	mov	di,[bp-04]

l0800_9898:
	les	si,[bp+06]

l0800_989B:
	lodsb
	or	al,al
	jz	98B3

l0800_98A1:
	cmp	al,25
	jz	98B6

l0800_98A5:
	mov	ss:[di],al
	inc	di
	dec	byte ptr [bp-14]
	jg	989B

l0800_98AE:
	call	9858
	jmp	989B

l0800_98B3:
	jmp	9C96

l0800_98B6:
	mov	[bp-10],si
	lodsb
	cmp	al,25
	jz	98A5

l0800_98BF:
	mov	[bp-04],di
	xor	cx,cx
	mov	[bp-0E],cx
	mov	word ptr [bp-02],0020
	mov	[bp-0B],cl
	mov	word ptr [bp-08],FFFF
	mov	word ptr [bp-0A],FFFF
	jmp	98DD

l0800_98DB:
	lodsb

l0800_98DD:
	xor	ah,ah
	mov	dx,ax
	mov	bx,ax
	sub	bl,20
	cmp	bl,60
	jnc	98FE

l0800_98EB:
	mov	bl,[bx+2605]
	cmp	bx,17
	jbe	98F7

l0800_98F4:
	jmp	9C82

l0800_98F7:
	shl	bx,01
	jmp	word ptr cs:[bx+9CB6]

l0800_98FE:
	jmp	9C82

l0800_9901:
	cmp	ch,00
	ja	98FE

l0800_9906:
	or	word ptr [bp-02],01
	jmp	98DB

l0800_990C:
	cmp	ch,00
	ja	98FE

l0800_9911:
	or	word ptr [bp-02],02
	jmp	98DB

l0800_9917:
	cmp	ch,00
	ja	98FE

l0800_991C:
	cmp	byte ptr [bp-0B],2B
	jz	9925

l0800_9922:
	mov	[bp-0B],dl

l0800_9925:
	jmp	98DB

l0800_9927:
	and	word ptr [bp-02],DF
	jmp	9931

l0800_992D:
	or	word ptr [bp-02],20

l0800_9931:
	mov	ch,05
	jmp	98DB

l0800_9935:
	cmp	ch,00
	ja	9987

l0800_993A:
	test	word ptr [bp-02],0002
	jnz	996A

l0800_9941:
	or	word ptr [bp-02],08
	mov	ch,01
	jmp	98DB

l0800_9949:
	jmp	9C82

l0800_994C:
	mov	di,[bp+04]
	mov	ax,ss:[di]
	add	word ptr [bp+04],02
	cmp	ch,02
	jnc	996D

l0800_995B:
	or	ax,ax
	jns	9965

l0800_995F:
	neg	ax
	or	word ptr [bp-02],02

l0800_9965:
	mov	[bp-08],ax
	mov	ch,03

l0800_996A:
	jmp	98DB

l0800_996D:
	cmp	ch,04
	jnz	9949

l0800_9972:
	mov	[bp-0A],ax
	inc	ch
	jmp	98DB

l0800_997A:
	cmp	ch,04
	jnc	9949

l0800_997F:
	mov	ch,04
	inc	word ptr [bp-0A]
	jmp	98DB

l0800_9987:
	xchg	ax,dx
	sub	al,30
	cbw
	cmp	ch,02
	ja	99A9

l0800_9990:
	mov	ch,02
	xchg	[bp-08],ax
	or	ax,ax
	jl	996A

l0800_9999:
	shl	ax,01
	mov	dx,ax
	shl	ax,01
	shl	ax,01
	add	ax,dx
	add	[bp-08],ax
	jmp	98DB

l0800_99A9:
	cmp	ch,04
	jnz	9949

l0800_99AE:
	xchg	[bp-0A],ax
	or	ax,ax
	jz	996A

l0800_99B5:
	shl	ax,01
	mov	dx,ax
	shl	ax,01
	shl	ax,01
	add	ax,dx
	add	[bp-0A],ax
	jmp	98DB

l0800_99C5:
	or	word ptr [bp-02],10
	jmp	9931

l0800_99CC:
	or	word ptr [bp-02],0100

l0800_99D1:
	and	word ptr [bp-02],EF
	jmp	9931

l0800_99D8:
	mov	bh,08
	jmp	99E6

l0800_99DC:
	mov	bh,0A
	jmp	99EA

l0800_99E0:
	mov	bh,10
	mov	bl,E9
	add	bl,dl

l0800_99E6:
	mov	byte ptr [bp-0B],00

l0800_99EA:
	mov	[bp-05],dl
	xor	dx,dx
	mov	[bp-06],dl
	mov	di,[bp+04]
	mov	ax,ss:[di]
	jmp	9A0A

l0800_99FA:
	mov	bh,0A
	mov	byte ptr [bp-06],01
	mov	[bp-05],dl
	mov	di,[bp+04]
	mov	ax,ss:[di]
	cwd

l0800_9A0A:
	inc	di
	inc	di
	mov	[bp+06],si
	test	word ptr [bp-02],0010
	jz	9A1B

l0800_9A16:
	mov	dx,ss:[di]
	inc	di
	inc	di

l0800_9A1B:
	mov	[bp+04],di
	lea	di,[bp-45]
	or	ax,ax
	jnz	9A32

l0800_9A25:
	or	dx,dx
	jnz	9A32

l0800_9A29:
	cmp	word ptr [bp-0A],00
	jnz	9A36

l0800_9A2F:
	jmp	9895

l0800_9A32:
	or	word ptr [bp-02],04

l0800_9A36:
	push	dx
	push	ax
	push	ss
	push	di
	mov	al,bh
	cbw
	push	ax
	mov	al,[bp-06]
	push	ax
	push	bx
	call	8D87
	push	ss
	pop	es
	mov	dx,[bp-0A]
	or	dx,dx
	jge	9A52

l0800_9A4F:
	jmp	9B46

l0800_9A52:
	jmp	9B54

l0800_9A55:
	mov	[bp-05],dl
	mov	[bp+06],si
	lea	di,[bp-46]
	mov	bx,[bp+04]
	push	word ptr ss:[bx]
	inc	bx
	inc	bx
	mov	[bp+04],bx
	test	word ptr [bp-02],0020
	jz	9A80

l0800_9A70:
	mov	dx,ss:[bx]
	inc	bx
	inc	bx
	mov	[bp+04],bx
	push	ss
	pop	es
	call	9810
	mov	al,3A
	stosb

l0800_9A80:
	push	ss
	pop	es
	pop	dx
	call	9810
	mov	byte ptr ss:[di],00
	mov	byte ptr [bp-06],00
	and	word ptr [bp-02],FB
	lea	cx,[bp-46]
	sub	di,cx
	xchg	di,cx
	mov	dx,[bp-0A]
	cmp	dx,cx
	jg	9AA2

l0800_9AA0:
	mov	dx,cx

l0800_9AA2:
	jmp	9B46

l0800_9AA5:
	mov	[bp+06],si
	mov	[bp-05],dl
	mov	di,[bp+04]
	mov	ax,ss:[di]
	add	word ptr [bp+04],02
	push	ss
	pop	es
	lea	di,[bp-45]
	xor	ah,ah
	mov	ss:[di],ax
	mov	cx,0001
	jmp	9B89

l0800_9AC5:
	mov	[bp+06],si
	mov	[bp-05],dl
	mov	di,[bp+04]
	test	word ptr [bp-02],0020
	jnz	9AE2

l0800_9AD5:
	mov	di,ss:[di]
	add	word ptr [bp+04],02
	push	ds
	pop	es
	or	di,di
	jmp	9AED

l0800_9AE2:
	les	di,ss:[di]
	add	word ptr [bp+04],04
	mov	ax,es
	or	ax,di

l0800_9AED:
	jnz	9AF4

l0800_9AEF:
	push	ds
	pop	es
	mov	di,25FE

l0800_9AF4:
	call	9842
	cmp	cx,[bp-0A]
	jbe	9AFF

l0800_9AFC:
	mov	cx,[bp-0A]

l0800_9AFF:
	jmp	9B89

l0800_9B02:
	mov	[bp+06],si
	mov	[bp-05],dl
	mov	di,[bp+04]
	mov	cx,[bp-0A]
	or	cx,cx
	jge	9B15

l0800_9B12:
	mov	cx,0006

l0800_9B15:
	push	ss
	push	di
	push	cx
	push	ss
	lea	bx,[bp-45]
	push	bx
	push	dx
	mov	ax,0001
	and	ax,[bp-02]
	push	ax
	mov	ax,[bp-02]
	test	ax,0100
	jz	9B36

l0800_9B2D:
	mov	ax,0008
	add	word ptr [bp+04],0A
	jmp	9B3D

l0800_9B36:
	add	word ptr [bp+04],08
	mov	ax,0006

l0800_9B3D:
	push	ax
	call	A2CC
	push	ss
	pop	es
	lea	di,[bp-45]

l0800_9B46:
	test	word ptr [bp-02],0008
	jz	9B65

l0800_9B4D:
	mov	dx,[bp-08]
	or	dx,dx
	jle	9B65

l0800_9B54:
	call	9842
	cmp	byte ptr es:[di],2D
	jnz	9B5E

l0800_9B5D:
	dec	cx

l0800_9B5E:
	sub	dx,cx
	jle	9B65

l0800_9B62:
	mov	[bp-0E],dx

l0800_9B65:
	cmp	byte ptr es:[di],2D
	jz	9B76

l0800_9B6B:
	mov	al,[bp-0B]
	or	al,al
	jz	9B86

l0800_9B72:
	dec	di
	mov	es:[di],al

l0800_9B76:
	cmp	word ptr [bp-0E],00
	jle	9B86

l0800_9B7C:
	mov	cx,[bp-0A]
	or	cx,cx
	jge	9B86

l0800_9B83:
	dec	word ptr [bp-0E]

l0800_9B86:
	call	9842

l0800_9B89:
	mov	si,di
	mov	di,[bp-04]
	mov	bx,[bp-08]
	mov	ax,0005
	and	ax,[bp-02]
	cmp	ax,0005
	jnz	9BAF

l0800_9B9C:
	mov	ah,[bp-05]
	cmp	ah,6F
	jnz	9BB1

l0800_9BA4:
	cmp	word ptr [bp-0E],00
	jg	9BAF

l0800_9BAA:
	mov	word ptr [bp-0E],0001

l0800_9BAF:
	jmp	9BCC

l0800_9BB1:
	cmp	ah,78
	jz	9BBB

l0800_9BB6:
	cmp	ah,58
	jnz	9BCC

l0800_9BBB:
	or	word ptr [bp-02],40
	dec	bx
	dec	bx
	sub	word ptr [bp-0E],02
	jge	9BCC

l0800_9BC7:
	mov	word ptr [bp-0E],0000

l0800_9BCC:
	add	cx,[bp-0E]
	test	word ptr [bp-02],0002
	jnz	9BE2

l0800_9BD6:
	jmp	9BDE

l0800_9BD8:
	mov	al,20
	call	984F
	dec	bx

l0800_9BDE:
	cmp	bx,cx
	jg	9BD8

l0800_9BE2:
	test	word ptr [bp-02],0040
	jz	9BF4

l0800_9BE9:
	mov	al,30
	call	984F
	mov	al,[bp-05]
	call	984F

l0800_9BF4:
	mov	dx,[bp-0E]
	or	dx,dx
	jle	9C22

l0800_9BFB:
	sub	cx,dx
	sub	bx,dx
	mov	al,es:[si]
	cmp	al,2D
	jz	9C0E

l0800_9C06:
	cmp	al,20
	jz	9C0E

l0800_9C0A:
	cmp	al,2B
	jnz	9C15

l0800_9C0E:
	lodsb
	call	984F
	dec	cx
	dec	bx

l0800_9C15:
	xchg	dx,cx
	jcxz	9C20

l0800_9C19:
	mov	al,30
	call	984F
	loop	9C19

l0800_9C20:
	xchg	dx,cx

l0800_9C22:
	jcxz	9C36

l0800_9C24:
	sub	bx,cx

l0800_9C26:
	lodsb
	mov	ss:[di],al
	inc	di
	dec	byte ptr [bp-14]
	jg	9C34

l0800_9C31:
	call	9858

l0800_9C34:
	loop	9C26

l0800_9C36:
	or	bx,bx
	jle	9C43

l0800_9C3A:
	mov	cx,bx

l0800_9C3C:
	mov	al,20
	call	984F
	loop	9C3C

l0800_9C43:
	jmp	9898

l0800_9C46:
	mov	[bp+06],si
	mov	di,[bp+04]
	test	word ptr [bp-02],0020
	jnz	9C5E

l0800_9C53:
	mov	di,ss:[di]
	add	word ptr [bp+04],02
	push	ds
	pop	es
	jmp	9C65

l0800_9C5E:
	les	di,ss:[di]
	add	word ptr [bp+04],04

l0800_9C65:
	mov	ax,0050
	sub	al,[bp-14]
	add	ax,[bp-12]
	mov	es:[di],ax
	test	word ptr [bp-02],0010
	jz	9C7F

l0800_9C78:
	inc	di
	inc	di
	mov	word ptr es:[di],0000

l0800_9C7F:
	jmp	9895

l0800_9C82:
	mov	si,[bp-10]
	mov	es,[bp+08]
	mov	di,[bp-04]
	mov	al,25

l0800_9C8D:
	call	984F
	lodsb
	or	al,al
	jnz	9C8D

l0800_9C96:
	cmp	byte ptr [bp-14],50
	jge	9C9F

l0800_9C9C:
	call	9858

l0800_9C9F:
	pop	es
	cmp	word ptr [bp-16],00
	jz	9CAB

l0800_9CA6:
	mov	ax,FFFF
	jmp	9CAE

l0800_9CAB:
	mov	ax,[bp-12]

l0800_9CAE:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	000C
l0800_9CB6	dw	0x9917
l0800_9CB8	dw	0x9901
l0800_9CBA	dw	0x994C
l0800_9CBC	dw	0x990C
l0800_9CBE	dw	0x997A
l0800_9CC0	dw	0x9987
l0800_9CC2	dw	0x99C5
l0800_9CC4	dw	0x99CC
l0800_9CC6	dw	0x99D1
l0800_9CC8	dw	0x9935
l0800_9CCA	dw	0x99FA
l0800_9CCC	dw	0x99D8
l0800_9CCE	dw	0x99DC
l0800_9CD0	dw	0x99E0
l0800_9CD2	dw	0x9A55
l0800_9CD4	dw	0x9B02
l0800_9CD6	dw	0x9AA5
l0800_9CD8	dw	0x9AC5
l0800_9CDA	dw	0x9C46
l0800_9CDC	dw	0x9C82
l0800_9CDE	dw	0x9C82
l0800_9CE0	dw	0x9C82
l0800_9CE2	dw	0x9927
l0800_9CE4	dw	0x992D

;; fn0800_9CE6: 0800:9CE6
;;   Called from:
;;     0800:8E9D (in fn0800_8E6A)
fn0800_9CE6 proc
	push	bp
	mov	bp,sp
	push	si
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	BFC7
	pop	cx
	pop	cx
	mov	si,ax
	inc	ax
	push	ax
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	B03B
	add	sp,0A
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	add	ax,si
	pop	si
	pop	bp
	ret
0800:9D16                   BA 66 26 EB 03 BA 6B 26 B9 05       .f&...k&..
0800:9D20 00 B4 40 BB 02 00 CD 21 B9 27 00 BA 70 26 B4 40 ..@....!.'..p&.@
0800:9D30 CD 21 E9 10 65 00 00 00 00 00 00 00 00 00 00 00 .!..e...........
0800:9D40 00                                              .              

;; fn0800_9D41: 0800:9D41
;;   Called from:
;;     0800:9E8D (in fn0800_9E75)
fn0800_9D41 proc
	cmp	dx,cs:[9D35]
	jz	9D7F

l0800_9D48:
	mov	ds,dx
	mov	ds,[0002]
	cmp	word ptr [0002],00
	jz	9D5C

l0800_9D55:
	mov	cs:[9D37],ds
	jmp	9D94

l0800_9D5C:
	mov	ax,ds
	cmp	ax,cs:[9D35]
	jz	9D7A

l0800_9D65:
	mov	ax,[0008]
	mov	cs:[9D37],ax
	push	ds
	xor	ax,ax
	push	ax
	call	9E15
	mov	ds,cs:[9D3B]
	jmp	9D9D

l0800_9D7A:
	mov	dx,cs:[9D35]

l0800_9D7F:
	mov	word ptr cs:[9D35],0000
	mov	word ptr cs:[9D37],0000
	mov	word ptr cs:[9D39],0000

l0800_9D94:
	mov	ds,cs:[9D3B]
	push	dx
	xor	ax,ax
	push	ax

l0800_9D9D:
	call	A1D6
	add	sp,04
	ret

;; fn0800_9DA4: 0800:9DA4
;;   Called from:
;;     0800:9E92 (in fn0800_9E75)
fn0800_9DA4 proc
	mov	ds,dx
	push	ds
	mov	es,[0002]
	mov	word ptr [0002],0000
	mov	[0008],es
	cmp	dx,cs:[9D35]
	jz	9DEA

l0800_9DBC:
	cmp	word ptr es:[0002],00
	jnz	9DEA

l0800_9DC4:
	mov	ax,[0000]
	pop	bx
	push	es
	add	es:[0000],ax
	mov	cx,es
	add	dx,ax
	mov	es,dx
	cmp	word ptr es:[0002],00
	jnz	9DE3

l0800_9DDC:
	mov	es:[0008],cx
	jmp	9DED

l0800_9DE3:
	mov	es:[0002],cx
	jmp	9DED

l0800_9DEA:
	call	9E3E

l0800_9DED:
	pop	es
	mov	ax,es
	add	ax,es:[0000]
	mov	ds,ax
	cmp	word ptr [0002],00
	jz	9DFF

l0800_9DFE:
	ret

l0800_9DFF:
	mov	ax,[0000]
	add	es:[0000],ax
	mov	ax,es
	mov	bx,ds
	add	bx,[0000]
	mov	es,bx
	mov	es:[0002],ax

;; fn0800_9E15: 0800:9E15
;;   Called from:
;;     0800:9D70 (in fn0800_9D41)
;;     0800:9E11 (in fn0800_9DA4)
;;     0800:9FEF (in fn0800_9F9F)
fn0800_9E15 proc
	mov	bx,ds
	cmp	bx,[0006]
	jz	9E36

l0800_9E1D:
	mov	es,[0006]
	mov	ds,[0004]
	mov	[0006],es
	mov	es:[0004],ds
	mov	cs:[9D39],ds
	mov	ds,bx
	ret

l0800_9E36:
	mov	word ptr cs:[9D39],0000
	ret

;; fn0800_9E3E: 0800:9E3E
;;   Called from:
;;     0800:9DEA (in fn0800_9DA4)
fn0800_9E3E proc
	mov	ax,cs:[9D39]
	or	ax,ax
	jz	9E67

l0800_9E46:
	mov	bx,ss
	pushf
	cli
	mov	ss,ax
	mov	es,ss:[0006]
	mov	ss:[0006],ds
	mov	[0004],ss
	mov	ss,bx
	popf
	mov	es:[0004],ds
	mov	[0006],es
	ret

l0800_9E67:
	mov	cs:[9D39],ds
	mov	[0004],ds
	mov	[0006],ds
	ret

;; fn0800_9E75: 0800:9E75
;;   Called from:
;;     0800:434F (in fn0800_4346)
;;     0800:A075 (in fn0800_A006)
;;     0800:A0BF (in fn0800_A080)
;;     0800:A65A (in fn0800_A614)
;;     0800:BB09 (in fn0800_BA89)
fn0800_9E75 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	cs:[9D3B],ds
	mov	dx,[bp+06]
	or	dx,dx
	jz	9E95

l0800_9E86:
	cmp	dx,cs:[9D37]
	jnz	9E92

l0800_9E8D:
	call	9D41
	jmp	9E95

l0800_9E92:
	call	9DA4

l0800_9E95:
	mov	ds,cs:[9D3B]
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_9E9E: 0800:9E9E
;;   Called from:
;;     0800:9FDE (in fn0800_9F9F)
fn0800_9E9E proc
	push	ax
	mov	ds,cs:[9D3B]
	xor	ax,ax
	push	ax
	push	ax
	call	A215
	add	sp,04
	and	ax,000F
	jz	9EC7

l0800_9EB3:
	mov	dx,0010
	sub	dx,ax
	xor	ax,ax
	mov	ds,cs:[9D3B]
	push	ax
	push	dx
	call	A215
	add	sp,04

l0800_9EC7:
	pop	ax
	push	ax
	xor	bx,bx
	mov	bl,ah
	mov	cl,04
	shr	bx,cl
	shl	ax,cl
	mov	ds,cs:[9D3B]
	push	bx
	push	ax
	call	A215
	add	sp,04
	pop	bx
	cmp	ax,FFFF
	jz	9EFE

l0800_9EE6:
	mov	cs:[9D35],dx
	mov	cs:[9D37],dx
	mov	ds,dx
	mov	[0000],bx
	mov	[0002],dx
	mov	ax,0004
	ret

l0800_9EFE:
	xor	ax,ax
	cwd
	ret

;; fn0800_9F02: 0800:9F02
;;   Called from:
;;     0800:9FD9 (in fn0800_9F9F)
fn0800_9F02 proc
	push	ax
	xor	bx,bx
	mov	bl,ah
	mov	cl,04
	shr	bx,cl
	shl	ax,cl
	mov	ds,cs:[9D3B]
	push	bx
	push	ax
	call	A215
	add	sp,04
	pop	bx
	cmp	ax,FFFF
	jz	9F58

l0800_9F20:
	and	ax,000F
	jnz	9F3D

l0800_9F25:
	mov	cx,cs:[9D37]
	mov	cs:[9D37],dx
	mov	ds,dx
	mov	[0000],bx
	mov	[0002],cx
	mov	ax,0004
	ret

l0800_9F3D:
	push	bx
	push	dx
	neg	ax
	add	ax,0010
	xor	bx,bx
	push	bx
	push	ax
	call	A215
	add	sp,04
	pop	dx
	pop	bx
	cmp	ax,FFFF
	jz	9F58

l0800_9F55:
	inc	dx
	jmp	9F25

l0800_9F58:
	xor	ax,ax
	cwd
	ret

;; fn0800_9F5C: 0800:9F5C
;;   Called from:
;;     0800:9FE3 (in fn0800_9F9F)
fn0800_9F5C proc
	mov	bx,dx
	sub	[0000],ax
	add	dx,[0000]
	mov	ds,dx
	mov	[0000],ax
	mov	[0002],bx
	mov	bx,dx
	add	bx,[0000]
	mov	ds,bx
	mov	[0002],dx
	mov	ax,0004
	ret

;; fn0800_9F7F: 0800:9F7F
;;   Called from:
;;     0800:BB4D (in fn0800_BA89)
fn0800_9F7F proc
	push	bp
	mov	bp,sp
	xor	dx,dx
	mov	ax,[bp+04]
	jmp	9F92

;; fn0800_9F89: 0800:9F89
;;   Called from:
;;     0800:431D (in fn0800_4311)
;;     0800:A013 (in fn0800_A006)
fn0800_9F89 proc
	push	bp
	mov	bp,sp
	mov	dx,[bp+06]
	mov	ax,[bp+04]

;; fn0800_9F92: 0800:9F92
;;   Called from:
;;     0800:9F87 (in fn0800_9F7F)
;;     0800:9F8F (in fn0800_9F89)
fn0800_9F92 proc
	mov	cx,ax
	or	cx,dx
	push	si
	push	di
	mov	cs:[9D3B],ds
	jz	9FFD

;; fn0800_9F9F: 0800:9F9F
;;   Called from:
;;     0800:9F9D (in fn0800_9F92)
;;     0800:9F9D (in fn0800_9F92)
;;     0800:9F9D (in fn0800_9F92)
fn0800_9F9F proc
	add	ax,0013
	adc	dx,00
	jc	9FE8

l0800_9FA7:
	test	dx,FFF0
	jnz	9FE8

l0800_9FAD:
	mov	cl,04
	shr	ax,cl
	shl	dx,cl
	or	ah,dl
	mov	dx,cs:[9D35]
	or	dx,dx
	jz	9FDE

l0800_9FBE:
	mov	dx,cs:[9D39]
	or	dx,dx
	jz	9FD9

l0800_9FC7:
	mov	bx,dx

l0800_9FC9:
	mov	ds,dx
	cmp	[0000],ax
	jnc	9FED

l0800_9FD1:
	mov	dx,[0006]
	cmp	dx,bx
	jnz	9FC9

l0800_9FD9:
	call	9F02
	jmp	9FFD

l0800_9FDE:
	call	9E9E
	jmp	9FFD

l0800_9FE3:
	call	9F5C
	jmp	9FFD

l0800_9FE8:
	xor	ax,ax
	cwd
	jmp	9FFD

l0800_9FED:
	ja	9FE3

l0800_9FEF:
	call	9E15
	mov	bx,[0008]
	mov	[0002],bx
	mov	ax,0004

l0800_9FFD:
	mov	ds,cs:[9D3B]
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_A006: 0800:A006
fn0800_A006 proc
	push	bx
	mov	si,cs:[9D3D]
	push	si
	mov	si,cs:[9D3F]
	push	si
	call	9F89
	add	sp,04
	or	dx,dx
	jnz	A01F

l0800_A01D:
	pop	bx
	ret

l0800_A01F:
	pop	ds
	mov	es,dx
	push	es
	push	ds
	push	bx
	mov	dx,[0000]
	cld
	dec	dx
	mov	di,0004
	mov	si,di
	mov	cx,0006

l0800_A033:
	rep movsw

l0800_A035:
	or	dx,dx
	jz	A070

l0800_A039:
	mov	ax,es
	inc	ax
	mov	es,ax
	mov	ax,ds
	inc	ax
	mov	ds,ax

l0800_A043:
	xor	di,di
	mov	si,di
	mov	cx,dx
	cmp	cx,1000
	jbe	A052

l0800_A04F:
	mov	cx,1000

l0800_A052:
	shl	cx,01
	shl	cx,01
	shl	cx,01

l0800_A058:
	rep movsw

l0800_A05A:
	sub	dx,1000
	jbe	A070

l0800_A060:
	mov	ax,es
	add	ax,1000
	mov	es,ax
	mov	ax,ds
	add	ax,1000
	mov	ds,ax
	jmp	A043

l0800_A070:
	mov	ds,cs:[9D3B]
	call	9E75
	add	sp,04
	pop	dx
	mov	ax,0004
	ret

;; fn0800_A080: 0800:A080
fn0800_A080 proc
	cmp	bx,cs:[9D37]
	jz	A0CB

l0800_A087:
	mov	di,bx
	add	di,ax
	mov	es,di
	mov	si,cx
	sub	si,ax
	mov	es:[0000],si
	mov	es:[0002],bx
	push	es
	push	ax
	mov	es,bx
	mov	es:[0000],ax
	mov	dx,bx
	add	dx,cx
	mov	es,dx
	cmp	word ptr es:[0002],00
	jz	A0B8

l0800_A0B1:
	mov	es:[0002],di
	jmp	A0BD

l0800_A0B8:
	mov	es:[0008],di

l0800_A0BD:
	mov	si,bx
	call	9E75
	add	sp,04
	mov	dx,si
	mov	ax,0004
	ret

l0800_A0CB:
	push	bx
	mov	es,bx
	mov	es:[0000],ax
	add	bx,ax
	push	bx
	xor	ax,ax
	push	ax
	call	A1D6
	add	sp,04
	pop	dx
	mov	ax,0004
	ret
0800:A0E3          55 8B EC 33 D2 EB 06 55 8B EC 8B 56 0A    U..3...U...V.
0800:A0F0 8B 46 08 8B 5E 06 56 57 2E 8C 1E 3B 9D 2E 89 16 .F..^.VW...;....
0800:A100 3D 9D 2E A3 3F 9D 0B DB 74 3A 8B C8 0B CA 74 3E =...?...t:....t>
0800:A110 05 13 00 83 D2 00 72 3E F7 C2 F0 FF 75 38 B1 04 ......r>....u8..
0800:A120 D3 E8 D3 E2 0A E2 8E C3 26 8B 0E 00 00 3B C8 72 ........&....;.r
0800:A130 0E 77 07 8B D3 B8 04 00 EB 1F E8 43 FF EB 1A E8 .w.........C....
0800:A140 C4 FE EB 15 52 50 E8 40 FE 83 C4 04 EB 0B 53 50 ....RP.@......SP
0800:A150 E8 22 FD 83 C4 04 33 C0 99 2E 8E 1E 3B 9D 5F 5E ."....3.....;._^
0800:A160 5D C3                                           ].             

;; fn0800_A162: 0800:A162
;;   Called from:
;;     0800:A205 (in fn0800_A1D6)
;;     0800:A292 (in fn0800_A215)
fn0800_A162 proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+06]
	inc	si
	sub	si,[007B]
	add	si,3F
	mov	cl,06
	shr	si,cl
	cmp	si,[2698]
	jnz	A18D

l0800_A17B:
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	mov	[008D],ax
	mov	[008B],dx
	mov	ax,0001
	jmp	A1D1

l0800_A18D:
	mov	cl,06
	shl	si,cl
	mov	dx,[0091]
	mov	ax,si
	add	ax,[007B]
	cmp	ax,dx
	jbe	A1A5

l0800_A19F:
	mov	si,dx
	sub	si,[007B]

l0800_A1A5:
	push	si
	push	word ptr [007B]
	call	A401
	pop	cx
	pop	cx
	mov	dx,ax
	cmp	dx,FF
	jnz	A1C1

l0800_A1B6:
	mov	ax,si
	mov	cl,06
	shr	ax,cl
	mov	[2698],ax
	jmp	A17B

l0800_A1C1:
	mov	ax,[007B]
	add	ax,dx
	mov	[0091],ax
	mov	word ptr [008F],0000
	xor	ax,ax

l0800_A1D1:
	pop	si
	pop	bp
	ret	0004

;; fn0800_A1D6: 0800:A1D6
;;   Called from:
;;     0800:9D9D (in fn0800_9D41)
;;     0800:A0D8 (in fn0800_A080)
fn0800_A1D6 proc
	push	bp
	mov	bp,sp
	mov	cx,[0089]
	mov	bx,[0087]
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	call	8F2F
	jc	A20C

l0800_A1EC:
	mov	cx,[0091]
	mov	bx,[008F]
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	call	8F2F
	ja	A20C

l0800_A1FF:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A162
	or	ax,ax
	jnz	A211

l0800_A20C:
	mov	ax,FFFF
	jmp	A213

l0800_A211:
	xor	ax,ax

l0800_A213:
	pop	bp
	ret

;; fn0800_A215: 0800:A215
;;   Called from:
;;     0800:9EA8 (in fn0800_9E9E)
;;     0800:9EC1 (in fn0800_9E9E)
;;     0800:9EDA (in fn0800_9E9E)
;;     0800:9F14 (in fn0800_9F02)
;;     0800:9F48 (in fn0800_9F02)
fn0800_A215 proc
	push	bp
	mov	bp,sp
	sub	sp,08
	mov	ax,[008D]
	xor	dx,dx
	mov	cl,04
	call	8C69
	add	ax,[008B]
	adc	dx,00
	add	ax,[bp+04]
	adc	dx,[bp+06]
	cmp	dx,0F
	jl	A246

l0800_A237:
	jg	A23E

l0800_A239:
	cmp	ax,FFFF
	jbe	A246

l0800_A23E:
	mov	dx,FFFF
	mov	ax,FFFF
	jmp	A29F

l0800_A246:
	mov	dx,[008D]
	mov	ax,[008B]
	mov	cx,[bp+06]
	mov	bx,[bp+04]
	call	8CCB
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	cx,[0089]
	mov	bx,[0087]
	mov	dx,[bp-02]
	call	8F2F
	jc	A23E

l0800_A26C:
	mov	cx,[0091]
	mov	bx,[008F]
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	call	8F2F
	ja	A23E

l0800_A27F:
	mov	ax,[008D]
	mov	dx,[008B]
	mov	[bp-06],ax
	mov	[bp-08],dx
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	A162
	or	ax,ax
	jz	A23E

l0800_A299:
	mov	dx,[bp-06]
	mov	ax,[bp-08]

l0800_A29F:
	mov	sp,bp
	pop	bp
	ret

;; fn0800_A2A3: 0800:A2A3
;;   Called from:
;;     0800:B209 (in fn0800_B140)
;;     0800:B22F (in fn0800_B140)
fn0800_A2A3 proc
	push	bp
	mov	bp,sp
	push	ds
	lds	dx,[bp+08]
	mov	ah,44
	mov	al,[bp+06]
	mov	bx,[bp+04]
	mov	cx,[bp+0C]
	int	21
	pop	ds
	jc	A2C6

l0800_A2BA:
	cmp	word ptr [bp+06],00
	jnz	A2C4

l0800_A2C0:
	mov	ax,dx
	jmp	A2CA

l0800_A2C4:
	jmp	A2CA

l0800_A2C6:
	push	ax
	call	8D2B

l0800_A2CA:
	pop	bp
	ret

;; fn0800_A2CC: 0800:A2CC
;;   Called from:
;;     0800:9B3E (in fn0800_9828)
fn0800_A2CC proc
	jmp	word ptr [26F4]

;; fn0800_A2D0: 0800:A2D0
;;   Called from:
;;     0800:91BE (in fn0800_8F97)
fn0800_A2D0 proc
	jmp	word ptr [26F6]

;; fn0800_A2D4: 0800:A2D4
;;   Called from:
;;     0800:91F4 (in fn0800_8F97)
fn0800_A2D4 proc
	jmp	word ptr [26F8]

;; fn0800_A2D8: 0800:A2D8
;;   Called from:
;;     0800:91FD (in fn0800_8F97)
;;     0800:9203 (in fn0800_8F97)
fn0800_A2D8 proc
	jmp	word ptr [26FA]
0800:A2DC                                     00 00 8F 06             ....
0800:A2E0 A0 26 8F 06 A2 26 8F 06 A4 26 2E 8C 1E DC A2 89 .&...&...&......
0800:A2F0 36 A6 26 89 3E A8 26 FC 8E 06 7B 00 BE 80 00 32 6.&.>.&...{....2
0800:A300 E4 26 AC 40 8C C5 87 D6 93 8B 36 75 00 46 46 B9 .&.@......6u.FF.
0800:A310 01 00 80 3E 7D 00 03 72 11 8E 06 77 00 8B FE B1 ...>}..r...w....
0800:A320 7F 32 C0 F2 AE E3 6E 80 F1 7F 50 8B C1 03 C3 40 .2....n...P....@
0800:A330 25 FE FF 8B FC 2B F8 72 5C 8B E7 06 1F 16 07 51 %....+.r\......Q
0800:A340 49 F3 A4 32 C0 AA 8E DD 87 F2 87 D9 8B C3 8B D0 I..2............
0800:A350 43 E8 19 00 77 07 72 40 E8 12 00 77 F9 3C 20 74 C...w.r@...w.< t
0800:A360 08 3C 0D 74 04 3C 09 75 E8 32 C0 EB E4          .<.t.<.u.2...  

;; fn0800_A36D: 0800:A36D
fn0800_A36D proc
	or	ax,ax
	jz	A378

l0800_A371:
	inc	dx
	stosb
	or	al,al
	jnz	A378

l0800_A377:
	inc	bx

l0800_A378:
	xchg	al,ah
	xor	al,al
	stc
	jcxz	A394

l0800_A37F:
	lodsb
	dec	cx
	sub	al,22
	jz	A394

l0800_A385:
	add	al,22
	cmp	al,5C
	jnz	A392

l0800_A38B:
	cmp	byte ptr [si],22
	jnz	A392

l0800_A390:
	lodsb
	dec	cx

l0800_A392:
	or	si,si

l0800_A394:
	ret
0800:A395                E9 AD 5E 59 03 CA 2E 8E 1E DC A2      ..^Y.......
0800:A3A0 89 1E 9A 26 43 03 DB 03 DB 8B F4 8B EC 2B EB 72 ...&C........+.r
0800:A3B0 E4 8B E5 89 2E 9C 26 8C 16 9E 26 E3 11 89 76 00 ......&...&...v.
0800:A3C0 8C 56 02 83 C5 04 36 AC 0A C0 E0 FA 74 ED 33 C0 .V....6.....t.3.
0800:A3D0 89 46 00 89 46 02 2E 8E 1E DC A2 8B 36 A6 26 8B .F..F.......6.&.
0800:A3E0 3E A8 26 FF 36 A4 26 FF 36 A2 26 A1 9A 26 A3 6B >.&.6.&.6.&..&.k
0800:A3F0 00 A1 9E 26 A3 6F 00 A1 9C 26 A3 6D 00 FF 26 A0 ...&.o...&.m..&.
0800:A400 26                                              &              

;; fn0800_A401: 0800:A401
;;   Called from:
;;     0800:A1AA (in fn0800_A162)
fn0800_A401 proc
	push	bp
	mov	bp,sp
	mov	ah,4A
	mov	bx,[bp+06]
	mov	es,[bp+04]
	int	21
	jc	A415

l0800_A410:
	mov	ax,FFFF
	jmp	A41B

l0800_A415:
	push	bx
	push	ax
	call	8D2B
	pop	ax

l0800_A41B:
	pop	bp
	ret
0800:A41D                                        56 57 8E              VW.
0800:A420 06 77 00 33 FF 06 FF 36 79 00 E8 52 FB 5B 8B D8 .w.3...6y..R.[..
0800:A430 07 A3 AA 26 89 16 AC 26 1E 8E DA 0B C2 75 03 E9 ...&...&.....u..
0800:A440 03 5E 33 C0 B9 FF FF 26 80 3D 00 74 0F 89 3F 8C .^3....&.=.t..?.
0800:A450 47 02 83 C3 04 F2 AE 26 38 05 75 F1 89 07 89 47 G......&8.u....G
0800:A460 02 1F 5F 5E A1 AC 26 A3 73 00 A1 AA 26 A3 71 00 .._^..&.s...&.q.
0800:A470 C3                                              .              

;; fn0800_A471: 0800:A471
;;   Called from:
;;     0800:C4A7 (in fn0800_C379)
fn0800_A471 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	push	es
	push	bp
	les	si,[bp+04]
	cld
	sub	ax,ax
	cwd
	mov	cx,000A
	mov	bh,00
	mov	di,2251

l0800_A487:
	mov	bl,es:[si]
	inc	si
	test	byte ptr [bx+di],01
	jnz	A487

l0800_A490:
	mov	bp,0000
	cmp	bl,2B
	jz	A49E

l0800_A498:
	cmp	bl,2D
	jnz	A4A2

l0800_A49D:
	inc	bp

l0800_A49E:
	mov	bl,es:[si]
	inc	si

l0800_A4A2:
	cmp	bl,39
	ja	A4D6

l0800_A4A7:
	sub	bl,30
	jc	A4D6

l0800_A4AC:
	mul	cx
	add	ax,bx
	adc	dl,dh
	jz	A49E

l0800_A4B4:
	jmp	A4C8

l0800_A4B6:
	mov	di,dx
	mov	cx,000A
	mul	cx
	xchg	ax,di
	xchg	cx,dx
	mul	dx
	xchg	ax,dx
	xchg	ax,di
	add	ax,bx
	adc	dx,cx

l0800_A4C8:
	mov	bl,es:[si]
	inc	si
	cmp	bl,39
	ja	A4D6

l0800_A4D1:
	sub	bl,30
	jnc	A4B6

l0800_A4D6:
	dec	bp
	jl	A4E0

l0800_A4D9:
	neg	dx
	neg	ax
	sbb	dx,00

l0800_A4E0:
	pop	bp
	pop	es
	pop	di
	pop	si
	pop	bp
	ret
0800:A4E6                   55 8B EC FF 76 06 FF 76 04 E8       U...v..v..
0800:A4F0 7F FF 59 59 5D C3                               ..YY].         

;; fn0800_A4F6: 0800:A4F6
;;   Called from:
;;     0800:3852 (in fn0800_37DF)
;;     0800:3D7D (in fn0800_3C99)
fn0800_A4F6 proc
	push	bp
	mov	bp,sp
	xor	ax,ax
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A53C
	add	sp,06
	mov	dx,ax
	cmp	dx,FF
	jnz	A513

l0800_A50F:
	mov	ax,dx
	jmp	A53A

l0800_A513:
	and	dx,FE
	test	word ptr [bp+08],0080
	jnz	A520

l0800_A51D:
	or	dx,01

l0800_A520:
	push	dx
	mov	ax,0001
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A53C
	add	sp,08
	mov	dx,ax
	cmp	dx,FF
	jz	A50F

l0800_A538:
	xor	ax,ax

l0800_A53A:
	pop	bp
	ret

;; fn0800_A53C: 0800:A53C
;;   Called from:
;;     0800:A502 (in fn0800_A4F6)
;;     0800:A52B (in fn0800_A4F6)
;;     0800:B165 (in fn0800_B140)
;;     0800:B25F (in fn0800_B140)
fn0800_A53C proc
	push	bp
	mov	bp,sp
	push	ds
	mov	cx,[bp+0A]
	mov	ah,43
	mov	al,[bp+08]
	lds	dx,[bp+04]
	int	21
	pop	ds
	jc	A553

l0800_A550:
	xchg	ax,cx
	jmp	A557

l0800_A553:
	push	ax
	call	8D2B

l0800_A557:
	pop	bp
	ret

;; fn0800_A559: 0800:A559
;;   Called from:
;;     0800:A66F (in fn0800_A614)
fn0800_A559 proc
	push	bp
	mov	bp,sp
	mov	dx,[bp+04]
	cmp	dx,[24E8]
	jc	A56E

l0800_A565:
	mov	ax,0006
	push	ax
	call	8D2B
	jmp	A57D

l0800_A56E:
	mov	bx,dx
	shl	bx,01
	mov	word ptr [bx+24EA],0000
	push	dx
	call	A57F
	pop	cx

l0800_A57D:
	pop	bp
	ret

;; fn0800_A57F: 0800:A57F
;;   Called from:
;;     0800:A579 (in fn0800_A559)
;;     0800:B1D7 (in fn0800_B140)
fn0800_A57F proc
	push	bp
	mov	bp,sp
	mov	ah,3E
	mov	bx,[bp+04]
	int	21
	jc	A597

l0800_A58B:
	shl	bx,01
	mov	word ptr [bx+24EA],0000
	xor	ax,ax
	jmp	A59B

l0800_A597:
	push	ax
	call	8D2B

l0800_A59B:
	pop	bp
	ret

;; fn0800_A59D: 0800:A59D
;;   Called from:
;;     0800:AF76 (in fn0800_AED6)
fn0800_A59D proc
	push	bp
	mov	bp,sp
	sub	sp,04
	mov	ax,[bp+04]
	cmp	ax,[24E8]
	jc	A5B2

l0800_A5AC:
	mov	ax,0006
	push	ax
	jmp	A60D

l0800_A5B2:
	mov	bx,[bp+04]
	shl	bx,01
	test	word ptr [bx+24EA],0200
	jz	A5C4

l0800_A5BF:
	mov	ax,0001
	jmp	A610

l0800_A5C4:
	mov	ax,4400
	mov	bx,[bp+04]
	int	21
	jc	A60C

l0800_A5CE:
	test	dl,80
	jnz	A608

l0800_A5D3:
	mov	ax,4201
	xor	cx,cx
	mov	dx,cx
	int	21
	jc	A60C

l0800_A5DE:
	push	dx
	push	ax
	mov	ax,4202
	xor	cx,cx
	mov	dx,cx
	int	21
	mov	[bp-04],ax
	mov	[bp-02],dx
	pop	dx
	pop	cx
	jc	A60C

l0800_A5F3:
	mov	ax,4200
	int	21
	jc	A60C

l0800_A5FA:
	cmp	dx,[bp-02]
	jc	A608

l0800_A5FF:
	ja	A606

l0800_A601:
	cmp	ax,[bp-04]
	jc	A608

l0800_A606:
	jmp	A5BF

l0800_A608:
	xor	ax,ax
	jmp	A610

l0800_A60C:
	push	ax

l0800_A60D:
	call	8D2B

l0800_A610:
	mov	sp,bp
	pop	bp
	ret

;; fn0800_A614: 0800:A614
;;   Called from:
;;     0800:0DBA (in fn0800_0DA9)
;;     0800:10BF (in fn0800_0DE8)
;;     0800:10CD (in fn0800_0DE8)
;;     0800:110E (in fn0800_0DE8)
;;     0800:1287 (in fn0800_112D)
;;     0800:1295 (in fn0800_112D)
;;     0800:1545 (in fn0800_12E2)
;;     0800:185D (in fn0800_12E2)
;;     0800:18B1 (in fn0800_12E2)
;;     0800:18CD (in fn0800_12E2)
;;     0800:19E3 (in fn0800_18D9)
;;     0800:1C93 (in fn0800_19EE)
;;     0800:1CD8 (in fn0800_19EE)
;;     0800:2F23 (in fn0800_2DE2)
;;     0800:3843 (in fn0800_37DF)
;;     0800:3936 (in fn0800_388C)
;;     0800:3944 (in fn0800_388C)
;;     0800:3A30 (in fn0800_3992)
;;     0800:3A5E (in fn0800_3992)
;;     0800:3A89 (in fn0800_3992)
;;     0800:3AD0 (in fn0800_3992)
;;     0800:3AFC (in fn0800_3992)
;;     0800:3C84 (in fn0800_3BC3)
;;     0800:3D60 (in fn0800_3C99)
;;     0800:3D6E (in fn0800_3C99)
;;     0800:AA18 (in fn0800_A96D)
fn0800_A614 proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,FFFF
	les	bx,[bp+04]
	mov	ax,es:[bx+12]
	cmp	ax,[bp+04]
	jz	A62A

l0800_A627:
	jmp	A6B2

l0800_A62A:
	les	bx,[bp+04]
	cmp	word ptr es:[bx+06],00
	jz	A65F

l0800_A634:
	cmp	word ptr es:[bx],00
	jge	A647

l0800_A63A:
	push	word ptr [bp+06]
	push	bx
	call	A6B7
	pop	cx
	pop	cx
	or	ax,ax
	jnz	A6B2

l0800_A647:
	les	bx,[bp+04]
	test	word ptr es:[bx+02],0004
	jz	A65F

l0800_A652:
	push	word ptr es:[bx+0A]
	push	word ptr es:[bx+08]
	call	9E75
	pop	cx
	pop	cx

l0800_A65F:
	les	bx,[bp+04]
	cmp	byte ptr es:[bx+04],00
	jl	A675

l0800_A669:
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	A559
	pop	cx
	mov	si,ax

l0800_A675:
	les	bx,[bp+04]
	mov	word ptr es:[bx+02],0000
	mov	word ptr es:[bx+06],0000
	mov	word ptr es:[bx],0000
	mov	byte ptr es:[bx+04],FF
	cmp	word ptr es:[bx+10],00
	jz	A6B2

l0800_A695:
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	ax
	push	word ptr es:[bx+10]
	call	8E6A
	push	dx
	push	ax
	call	97F8
	pop	cx
	pop	cx
	les	bx,[bp+04]
	mov	word ptr es:[bx+10],0000

l0800_A6B2:
	mov	ax,si
	pop	si
	pop	bp
	ret

;; fn0800_A6B7: 0800:A6B7
;;   Called from:
;;     0800:A63E (in fn0800_A614)
;;     0800:A89E (in fn0800_A877)
;;     0800:ACC0 (in fn0800_ACB3)
;;     0800:AE37 (in fn0800_AE10)
;;     0800:B374 (in fn0800_B324)
;;     0800:B3C0 (in fn0800_B324)
;;     0800:B40F (in fn0800_B324)
;;     0800:B52C (in fn0800_B4BE)
;;     0800:B5A5 (in fn0800_B4BE)
fn0800_A6B7 proc
	push	bp
	mov	bp,sp
	push	si
	mov	ax,[bp+04]
	or	ax,[bp+06]
	jnz	A6C9

l0800_A6C3:
	call	A877
	jmp	A778

l0800_A6C9:
	les	bx,[bp+04]
	mov	ax,es:[bx+12]
	cmp	ax,[bp+04]
	jz	A6DB

l0800_A6D5:
	mov	ax,FFFF
	jmp	A77A

l0800_A6DB:
	les	bx,[bp+04]
	cmp	word ptr es:[bx],00
	jl	A732

l0800_A6E4:
	test	word ptr es:[bx+02],0008
	jnz	A701

l0800_A6EC:
	mov	ax,es:[bx+0E]
	mov	dx,[bp+04]
	add	dx,05
	cmp	ax,[bp+06]
	jnz	A778

l0800_A6FB:
	cmp	es:[bx+0C],dx
	jnz	A778

l0800_A701:
	les	bx,[bp+04]
	mov	word ptr es:[bx],0000
	mov	ax,es:[bx+0E]
	mov	dx,[bp+04]
	add	dx,05
	cmp	ax,[bp+06]
	jnz	A778

l0800_A718:
	cmp	es:[bx+0C],dx
	jnz	A778

l0800_A71E:
	mov	ax,es:[bx+0A]
	mov	dx,es:[bx+08]
	mov	es:[bx+0E],ax
	mov	es:[bx+0C],dx
	jmp	A778
0800:A730 EB 46                                           .F             

l0800_A732:
	les	bx,[bp+04]
	mov	ax,es:[bx+06]
	add	ax,es:[bx]
	inc	ax
	mov	si,ax
	sub	es:[bx],si
	push	ax
	mov	ax,es:[bx+0A]
	mov	dx,es:[bx+08]
	mov	es:[bx+0E],ax
	mov	es:[bx+0C],dx
	push	ax
	push	dx
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	C632
	add	sp,08
	cmp	ax,si
	jz	A778

l0800_A765:
	les	bx,[bp+04]
	test	word ptr es:[bx+02],0200
	jnz	A778

l0800_A770:
	or	word ptr es:[bx+02],10
	jmp	A6D5

l0800_A778:
	xor	ax,ax

l0800_A77A:
	pop	si
	pop	bp
	ret

;; fn0800_A77D: 0800:A77D
;;   Called from:
;;     0800:2E32 (in fn0800_2DE2)
fn0800_A77D proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	di
	mov	di,[bp+08]
	xor	cx,cx
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	A7A1

l0800_A798:
	les	bx,[bp-04]
	mov	es:[bx],cl
	inc	word ptr [bp-04]

l0800_A7A1:
	cmp	cx,0A
	jz	A7D8

l0800_A7A6:
	dec	di
	jle	A7D8

l0800_A7A9:
	les	bx,[bp+0A]
	dec	word ptr es:[bx]
	jl	A7C6

l0800_A7B1:
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	mov	es,ax
	mov	al,es:[si]
	mov	ah,00
	jmp	A7D1

l0800_A7C6:
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	call	AEC2
	pop	cx
	pop	cx

l0800_A7D1:
	mov	cx,ax
	cmp	ax,FFFF
	jnz	A798

l0800_A7D8:
	cmp	cx,FF
	jnz	A7F3

l0800_A7DD:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	cmp	ax,[bp+06]
	jnz	A7F3

l0800_A7E8:
	cmp	dx,[bp+04]
	jnz	A7F3

l0800_A7ED:
	xor	dx,dx
	xor	ax,ax
	jmp	A811

l0800_A7F3:
	les	bx,[bp-04]
	mov	byte ptr es:[bx],00
	les	bx,[bp+0A]
	test	word ptr es:[bx+02],0010
	jz	A80B

l0800_A805:
	xor	dx,dx
	xor	ax,ax
	jmp	A811

l0800_A80B:
	mov	dx,[bp+06]
	mov	ax,[bp+04]

l0800_A811:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_A817: 0800:A817
;;   Called from:
;;     0800:33E0 (in fn0800_33CD)
;;     0800:3651 (in fn0800_363D)
fn0800_A817 proc
	push	bp
	mov	bp,sp
	push	ds
	mov	ah,2F
	int	21
	push	es
	push	bx
	mov	ah,1A
	lds	dx,[bp+08]
	int	21
	mov	ah,4E
	mov	cx,[bp+0C]
	lds	dx,[bp+04]
	int	21
	pushf
	pop	cx
	xchg	ax,bx
	mov	ah,1A
	pop	dx
	pop	ds
	int	21
	push	cx
	popf
	pop	ds
	jc	A844

l0800_A840:
	xor	ax,ax
	jmp	A848

l0800_A844:
	push	bx
	call	8D2B

l0800_A848:
	pop	bp
	ret

;; fn0800_A84A: 0800:A84A
;;   Called from:
;;     0800:3483 (in fn0800_3479)
fn0800_A84A proc
	push	bp
	mov	bp,sp
	push	ds
	mov	ah,2F
	int	21
	push	es
	push	bx
	mov	ah,1A
	lds	dx,[bp+04]
	int	21
	mov	ah,4F
	int	21
	pushf
	pop	cx
	xchg	ax,bx
	mov	ah,1A
	pop	dx
	pop	ds
	int	21
	push	cx
	popf
	pop	ds
	jc	A871

l0800_A86D:
	xor	ax,ax
	jmp	A875

l0800_A871:
	push	bx
	call	8D2B

l0800_A875:
	pop	bp
	ret

;; fn0800_A877: 0800:A877
;;   Called from:
;;     0800:A6C3 (in fn0800_A6B7)
fn0800_A877 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	di
	xor	di,di
	mov	si,[24E8]
	mov	[bp-02],ds
	mov	word ptr [bp-04],2358
	jmp	A8A8

l0800_A88F:
	les	bx,[bp-04]
	test	word ptr es:[bx+02],0003
	jz	A8A4

l0800_A89A:
	push	word ptr [bp-02]
	push	bx
	call	A6B7
	pop	cx
	pop	cx
	inc	di

l0800_A8A4:
	add	word ptr [bp-04],14

l0800_A8A8:
	mov	ax,si
	dec	si
	or	ax,ax
	jnz	A88F

l0800_A8AF:
	mov	ax,di
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_A8B7: 0800:A8B7
;;   Called from:
;;     0800:A983 (in fn0800_A96D)
fn0800_A8B7 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	xor	di,di
	les	bx,[bp+0C]
	inc	word ptr [bp+0C]
	mov	cl,es:[bx]
	mov	al,cl
	cmp	al,72
	jnz	A8D5

l0800_A8CD:
	mov	dx,0001
	mov	si,0001
	jmp	A8F3

l0800_A8D5:
	cmp	cl,77
	jnz	A8DF

l0800_A8DA:
	mov	dx,0302
	jmp	A8E7

l0800_A8DF:
	cmp	cl,61
	jnz	A8EF

l0800_A8E4:
	mov	dx,0902

l0800_A8E7:
	mov	di,0080
	mov	si,0002
	jmp	A8F3

l0800_A8EF:
	xor	ax,ax
	jmp	A967

l0800_A8F3:
	les	bx,[bp+0C]
	mov	cl,es:[bx]
	inc	word ptr [bp+0C]
	cmp	cl,2B
	jz	A914

l0800_A901:
	les	bx,[bp+0C]
	cmp	byte ptr es:[bx],2B
	jnz	A92B

l0800_A90A:
	cmp	cl,74
	jz	A914

l0800_A90F:
	cmp	cl,62
	jnz	A92B

l0800_A914:
	cmp	cl,2B
	jnz	A91F

l0800_A919:
	les	bx,[bp+0C]
	mov	cl,es:[bx]

l0800_A91F:
	and	dx,FC
	or	dx,04
	mov	di,0180
	mov	si,0003

l0800_A92B:
	cmp	cl,74
	jnz	A936

l0800_A930:
	or	dx,4000
	jmp	A953

l0800_A936:
	cmp	cl,62
	jnz	A941

l0800_A93B:
	or	dx,8000
	jmp	A950

l0800_A941:
	mov	ax,[2512]
	and	ax,C000
	or	dx,ax
	mov	ax,dx
	test	ax,8000
	jz	A953

l0800_A950:
	or	si,40

l0800_A953:
	mov	word ptr [2354],C7B5
	les	bx,[bp+08]
	mov	es:[bx],dx
	les	bx,[bp+04]
	mov	es:[bx],di
	mov	ax,si

l0800_A967:
	pop	di
	pop	si
	pop	bp
	ret	000C

;; fn0800_A96D: 0800:A96D
;;   Called from:
;;     0800:AAAC (in fn0800_AA7E)
fn0800_A96D proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	ss
	lea	ax,[bp-02]
	push	ax
	push	ss
	lea	ax,[bp-04]
	push	ax
	call	A8B7
	les	bx,[bp+0E]
	mov	es:[bx+02],ax
	or	ax,ax
	jz	A9B9

l0800_A991:
	cmp	byte ptr es:[bx+04],00
	jge	A9CD

l0800_A998:
	push	word ptr [bp-04]
	mov	ax,[bp-02]
	or	ax,[bp+04]
	push	ax
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	call	B140
	add	sp,08
	les	bx,[bp+0E]
	mov	es:[bx+04],al
	or	al,al
	jge	A9CD

l0800_A9B9:
	les	bx,[bp+0E]
	mov	byte ptr es:[bx+04],FF
	mov	word ptr es:[bx+02],0000

l0800_A9C7:
	xor	dx,dx
	xor	ax,ax
	jmp	AA2E

l0800_A9CD:
	les	bx,[bp+0E]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	8D76
	pop	cx
	or	ax,ax
	jz	A9E7

l0800_A9DE:
	les	bx,[bp+0E]
	or	word ptr es:[bx+02],0200

l0800_A9E7:
	mov	ax,0200
	push	ax
	les	bx,[bp+0E]
	test	word ptr es:[bx+02],0200
	jz	A9FB

l0800_A9F6:
	mov	ax,0001
	jmp	A9FD

l0800_A9FB:
	xor	ax,ax

l0800_A9FD:
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+10]
	push	word ptr [bp+0E]
	call	BA89
	add	sp,0C
	or	ax,ax
	jz	AA1F

l0800_AA12:
	push	word ptr [bp+10]
	push	word ptr [bp+0E]
	call	A614
	pop	cx
	pop	cx
	jmp	A9C7

l0800_AA1F:
	les	bx,[bp+0E]
	mov	word ptr es:[bx+10],0000
	mov	dx,[bp+10]
	mov	ax,[bp+0E]

l0800_AA2E:
	mov	sp,bp
	pop	bp
	ret	000E

;; fn0800_AA34: 0800:AA34
;;   Called from:
;;     0800:AA84 (in fn0800_AA7E)
fn0800_AA34 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	mov	[bp-02],ds
	mov	word ptr [bp-04],2358

l0800_AA42:
	les	bx,[bp-04]
	cmp	byte ptr es:[bx+04],00
	jl	AA64

l0800_AA4C:
	mov	ax,[bp-04]
	add	word ptr [bp-04],14
	push	ax
	mov	ax,[24E8]
	mov	dx,0014
	imul	dx
	add	ax,2358
	pop	dx
	cmp	dx,ax
	jc	AA42

l0800_AA64:
	les	bx,[bp-04]
	cmp	byte ptr es:[bx+04],00
	jl	AA74

l0800_AA6E:
	xor	dx,dx
	xor	ax,ax
	jmp	AA7A

l0800_AA74:
	mov	dx,[bp-02]
	mov	ax,[bp-04]

l0800_AA7A:
	mov	sp,bp
	pop	bp
	ret

;; fn0800_AA7E: 0800:AA7E
;;   Called from:
;;     0800:1336 (in fn0800_12E2)
;;     0800:190A (in fn0800_18D9)
;;     0800:1A20 (in fn0800_19EE)
;;     0800:382D (in fn0800_37DF)
;;     0800:39FC (in fn0800_3992)
;;     0800:4246 (in fn0800_4234)
fn0800_AA7E proc
	push	bp
	mov	bp,sp
	sub	sp,04
	call	AA34
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,dx
	jnz	AA97

l0800_AA91:
	xor	dx,dx
	xor	ax,ax
	jmp	AAAF

l0800_AA97:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	xor	ax,ax
	push	ax
	call	A96D

l0800_AAAF:
	mov	sp,bp
	pop	bp
	ret

;; fn0800_AAB3: 0800:AAB3
;;   Called from:
;;     0800:ABE2 (in fn0800_ABA3)
;;     0800:AC1F (in fn0800_ABA3)
fn0800_AAB3 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	jmp	AB91

l0800_AABB:
	inc	word ptr [bp+08]
	les	bx,[bp+04]
	mov	ax,es:[bx+06]
	cmp	ax,[bp+08]
	jbe	AACF

l0800_AACA:
	mov	ax,[bp+08]
	jmp	AAD6

l0800_AACF:
	les	bx,[bp+04]
	mov	ax,es:[bx+06]

l0800_AAD6:
	mov	di,ax
	les	bx,[bp+04]
	test	word ptr es:[bx+02],0040
	jz	AB50

l0800_AAE3:
	cmp	word ptr es:[bx+06],00
	jz	AB50

l0800_AAEA:
	mov	ax,es:[bx+06]
	cmp	ax,[bp+08]
	jnc	AB50

l0800_AAF3:
	cmp	word ptr es:[bx],00
	jnz	AB50

l0800_AAF9:
	dec	word ptr [bp+08]
	xor	di,di
	jmp	AB0E

l0800_AB00:
	les	bx,[bp+04]
	add	di,es:[bx+06]
	mov	ax,es:[bx+06]
	sub	[bp+08],ax

l0800_AB0E:
	les	bx,[bp+04]
	mov	ax,es:[bx+06]
	cmp	ax,[bp+08]
	jbe	AB00

l0800_AB1A:
	push	di
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	8F50
	add	sp,08
	mov	dx,ax
	add	[bp+0A],dx
	cmp	dx,di
	jz	AB91

l0800_AB36:
	mov	ax,di
	sub	ax,dx
	add	[bp+08],ax

l0800_AB3D:
	les	bx,[bp+04]
	or	word ptr es:[bx+02],20
	jmp	AB9A

l0800_AB47:
	les	bx,[bp+0A]
	mov	es:[bx],dl
	inc	word ptr [bp+0A]

l0800_AB50:
	dec	word ptr [bp+08]
	mov	ax,[bp+08]
	or	ax,ax
	jz	AB8C

l0800_AB5A:
	dec	di
	jz	AB8C

l0800_AB5D:
	les	bx,[bp+04]
	dec	word ptr es:[bx]
	jl	AB7A

l0800_AB65:
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	mov	es,ax
	mov	al,es:[si]
	mov	ah,00
	jmp	AB85

l0800_AB7A:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AEC2
	pop	cx
	pop	cx

l0800_AB85:
	mov	dx,ax
	cmp	ax,FFFF
	jnz	AB47

l0800_AB8C:
	cmp	dx,FF
	jz	AB3D

l0800_AB91:
	cmp	word ptr [bp+08],00
	jz	AB9A

l0800_AB97:
	jmp	AABB

l0800_AB9A:
	mov	ax,[bp+08]
	pop	di
	pop	si
	pop	bp
	ret	000A

;; fn0800_ABA3: 0800:ABA3
;;   Called from:
;;     0800:4126 (in fn0800_4110)
fn0800_ABA3 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	di
	mov	di,[bp+08]
	or	di,di
	jnz	ABB6

l0800_ABB2:
	xor	ax,ax
	jmp	AC2B

l0800_ABB6:
	mov	bx,di
	xor	cx,cx
	mov	ax,[bp+0A]
	xor	dx,dx
	call	8F18
	mov	[bp-02],dx
	mov	[bp-04],ax
	cmp	dx,01
	ja	ABF2

l0800_ABCD:
	jc	ABD3

l0800_ABCF:
	or	ax,ax
	jnc	ABF2

l0800_ABD3:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [bp-04]
	push	word ptr [bp+0E]
	push	word ptr [bp+0C]
	call	AAB3
	mov	dx,[bp-04]
	sub	dx,ax
	push	dx
	xor	dx,dx
	pop	ax
	div	di
	jmp	AC2B

l0800_ABF2:
	mov	si,[bp+0A]
	inc	si
	jmp	AC0B

l0800_ABF8:
	mov	bx,di
	xor	cx,cx
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	call	8CCB
	mov	[bp+06],dx
	mov	[bp+04],ax

l0800_AC0B:
	dec	si
	mov	ax,si
	or	ax,ax
	jz	AC26

l0800_AC12:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	di
	push	word ptr [bp+0E]
	push	word ptr [bp+0C]
	call	AAB3
	or	ax,ax
	jz	ABF8

l0800_AC26:
	mov	ax,[bp+0A]
	sub	ax,si

l0800_AC2B:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_AC31: 0800:AC31
;;   Called from:
;;     0800:ACE0 (in fn0800_ACB3)
;;     0800:AD5F (in fn0800_AD2F)
;;     0800:AD71 (in fn0800_AD2F)
fn0800_AC31 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	les	bx,[bp+04]
	cmp	word ptr es:[bx],00
	jge	AC4D

l0800_AC41:
	mov	cx,es:[bx+06]
	add	cx,es:[bx]
	inc	cx
	mov	si,cx
	jmp	AC5C

l0800_AC4D:
	les	bx,[bp+04]
	mov	ax,es:[bx]
	cwd
	xor	ax,dx
	sub	ax,dx
	mov	cx,ax
	mov	si,ax

l0800_AC5C:
	les	bx,[bp+04]
	test	word ptr es:[bx+02],0040
	jnz	ACAA

l0800_AC67:
	les	bx,[bp+04]
	mov	ax,es:[bx+0E]
	mov	dx,es:[bx+0C]
	mov	[bp-02],ax
	mov	[bp-04],dx
	cmp	word ptr es:[bx],00
	jge	ACA3

l0800_AC7E:
	jmp	AC8D

l0800_AC80:
	dec	word ptr [bp-04]
	les	bx,[bp-04]
	cmp	byte ptr es:[bx],0A
	jnz	AC8D

l0800_AC8C:
	inc	si

l0800_AC8D:
	mov	ax,cx
	dec	cx
	or	ax,ax
	jnz	AC80

l0800_AC94:
	jmp	ACAA

l0800_AC96:
	les	bx,[bp-04]
	inc	word ptr [bp-04]
	cmp	byte ptr es:[bx],0A
	jnz	ACA3

l0800_ACA2:
	inc	si

l0800_ACA3:
	mov	ax,cx
	dec	cx
	or	ax,ax
	jnz	AC96

l0800_ACAA:
	mov	ax,si
	pop	si
	mov	sp,bp
	pop	bp
	ret	0004

;; fn0800_ACB3: 0800:ACB3
;;   Called from:
;;     0800:139D (in fn0800_12E2)
;;     0800:170B (in fn0800_12E2)
;;     0800:179C (in fn0800_12E2)
;;     0800:1B82 (in fn0800_19EE)
;;     0800:1D23 (in fn0800_1CF6)
;;     0800:1D7C (in fn0800_1CF6)
;;     0800:1E0B (in fn0800_1CF6)
;;     0800:1EBD (in fn0800_1E5E)
;;     0800:1F2C (in fn0800_1E5E)
;;     0800:25D3 (in fn0800_24FE)
;;     0800:2DA6 (in fn0800_2D0A)
;;     0800:3C17 (in fn0800_3BC3)
;;     0800:3C5C (in fn0800_3BC3)
;;     0800:3CE7 (in fn0800_3C99)
;;     0800:3D1C (in fn0800_3C99)
;;     0800:3EFC (in fn0800_3E9A)
;;     0800:3F44 (in fn0800_3F0A)
;;     0800:3F98 (in fn0800_3F58)
;;     0800:41BA (in fn0800_4194)
;;     0800:41E1 (in fn0800_4194)
;;     0800:472F (in fn0800_46FE)
;;     0800:4775 (in fn0800_46FE)
;;     0800:47C5 (in fn0800_46FE)
;;     0800:4818 (in fn0800_46FE)
;;     0800:49AA (in fn0800_46FE)
;;     0800:4AAF (in fn0800_46FE)
;;     0800:4AE9 (in fn0800_46FE)
;;     0800:4B47 (in fn0800_46FE)
;;     0800:4BDB (in fn0800_4BB1)
;;     0800:4DAA (in fn0800_4C55)
;;     0800:4DFF (in fn0800_4C55)
;;     0800:4E38 (in fn0800_4C55)
;;     0800:4FE1 (in fn0800_4F2C)
;;     0800:5019 (in fn0800_4F2C)
;;     0800:5128 (in fn0800_4F2C)
;;     0800:515D (in fn0800_4F2C)
;;     0800:5182 (in fn0800_4F2C)
;;     0800:5285 (in fn0800_51A9)
;;     0800:52B1 (in fn0800_51A9)
;;     0800:55BF (in fn0800_5374)
;;     0800:5C05 (in fn0800_5B15)
;;     0800:5E38 (in fn0800_5DCE)
;;     0800:5E52 (in fn0800_5DCE)
;;     0800:5F1A (in fn0800_5E64)
;;     0800:6086 (in fn0800_5E64)
;;     0800:61D5 (in fn0800_5E64)
;;     0800:6206 (in fn0800_5E64)
;;     0800:6293 (in fn0800_5E64)
;;     0800:62BB (in fn0800_5E64)
;;     0800:63C7 (in fn0800_5E64)
;;     0800:63DD (in fn0800_5E64)
;;     0800:6456 (in fn0800_5E64)
;;     0800:6493 (in fn0800_5E64)
;;     0800:64C9 (in fn0800_5E64)
;;     0800:6502 (in fn0800_5E64)
;;     0800:653A (in fn0800_5E64)
;;     0800:681D (in fn0800_67BF)
;;     0800:684C (in fn0800_67BF)
;;     0800:687F (in fn0800_67BF)
;;     0800:691A (in fn0800_67BF)
;;     0800:69C5 (in fn0800_67BF)
;;     0800:6A02 (in fn0800_67BF)
;;     0800:6AB4 (in fn0800_67BF)
;;     0800:6F65 (in fn0800_6F20)
;;     0800:6F80 (in fn0800_6F20)
;;     0800:6FBE (in fn0800_6F20)
;;     0800:7053 (in fn0800_6F20)
;;     0800:707E (in fn0800_6F20)
;;     0800:70CF (in fn0800_6F20)
;;     0800:7123 (in fn0800_6F20)
;;     0800:7142 (in fn0800_6F20)
;;     0800:719C (in fn0800_6F20)
;;     0800:7307 (in fn0800_6F20)
;;     0800:736D (in fn0800_6F20)
;;     0800:7398 (in fn0800_6F20)
;;     0800:73E8 (in fn0800_73AC)
;;     0800:74E2 (in fn0800_741D)
;;     0800:7526 (in fn0800_741D)
;;     0800:755E (in fn0800_741D)
;;     0800:7907 (in fn0800_75EA)
;;     0800:798E (in fn0800_75EA)
;;     0800:79B0 (in fn0800_75EA)
;;     0800:7A5A (in fn0800_7A02)
;;     0800:7CA3 (in fn0800_7C78)
;;     0800:8026 (in fn0800_7FDC)
;;     0800:8039 (in fn0800_7FDC)
;;     0800:8313 (in fn0800_7FDC)
;;     0800:BA75 (in fn0800_BA67)
;;     0800:BAF0 (in fn0800_BA89)
fn0800_ACB3 proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+0C]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A6B7
	pop	cx
	pop	cx
	or	ax,ax
	jz	ACCE

l0800_ACC9:
	mov	ax,FFFF
	jmp	AD2C

l0800_ACCE:
	cmp	si,01
	jnz	ACEA

l0800_ACD3:
	les	bx,[bp+04]
	cmp	word ptr es:[bx],00
	jle	ACEA

l0800_ACDC:
	push	word ptr [bp+06]
	push	bx
	call	AC31
	cwd
	sub	[bp+08],ax
	sbb	[bp+0A],dx

l0800_ACEA:
	les	bx,[bp+04]
	and	word ptr es:[bx+02],FE5F
	mov	word ptr es:[bx],0000
	mov	ax,es:[bx+0A]
	mov	dx,es:[bx+08]
	mov	es:[bx+0E],ax
	mov	es:[bx+0C],dx
	push	si
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	8E29
	add	sp,08
	cmp	dx,FF
	jnz	AD2A

l0800_AD20:
	cmp	ax,FFFF
	jnz	AD2A

l0800_AD25:
	mov	ax,FFFF
	jmp	AD2C

l0800_AD2A:
	xor	ax,ax

l0800_AD2C:
	pop	si
	pop	bp
	ret

;; fn0800_AD2F: 0800:AD2F
;;   Called from:
;;     0800:13AB (in fn0800_12E2)
;;     0800:2D2A (in fn0800_2D0A)
;;     0800:41A0 (in fn0800_4194)
;;     0800:41C6 (in fn0800_4194)
;;     0800:47A9 (in fn0800_46FE)
;;     0800:4A2E (in fn0800_46FE)
;;     0800:4A47 (in fn0800_46FE)
;;     0800:4A5E (in fn0800_46FE)
;;     0800:4A90 (in fn0800_46FE)
;;     0800:4ACC (in fn0800_46FE)
;;     0800:4D8A (in fn0800_4C55)
;;     0800:4EE6 (in fn0800_4C55)
;;     0800:4F53 (in fn0800_4F2C)
;;     0800:5109 (in fn0800_4F2C)
;;     0800:5319 (in fn0800_51A9)
;;     0800:5345 (in fn0800_51A9)
;;     0800:5383 (in fn0800_5374)
;;     0800:5B4F (in fn0800_5B15)
;;     0800:6239 (in fn0800_5E64)
;;     0800:63EB (in fn0800_5E64)
;;     0800:6961 (in fn0800_67BF)
;;     0800:69A2 (in fn0800_67BF)
;;     0800:6C6E (in fn0800_6AD4)
;;     0800:6E9B (in fn0800_6AD4)
;;     0800:6F47 (in fn0800_6F20)
;;     0800:6FEB (in fn0800_6F20)
;;     0800:7020 (in fn0800_6F20)
;;     0800:72D9 (in fn0800_6F20)
;;     0800:7597 (in fn0800_741D)
;;     0800:75C9 (in fn0800_741D)
;;     0800:7752 (in fn0800_75EA)
;;     0800:7767 (in fn0800_75EA)
;;     0800:78D8 (in fn0800_75EA)
;;     0800:7C0E (in fn0800_7A02)
;;     0800:7E61 (in fn0800_7C78)
fn0800_AD2F proc
	push	bp
	mov	bp,sp
	sub	sp,04
	les	bx,[bp+04]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	97B6
	pop	cx
	mov	[bp-02],dx
	mov	[bp-04],ax
	cmp	dx,FF
	jnz	AD52

l0800_AD4D:
	cmp	ax,FFFF
	jz	AD7B

l0800_AD52:
	les	bx,[bp+04]
	cmp	word ptr es:[bx],00
	jge	AD6B

l0800_AD5B:
	push	word ptr [bp+06]
	push	bx
	call	AC31
	cwd
	add	[bp-04],ax
	adc	[bp-02],dx
	jmp	AD7B

l0800_AD6B:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AC31
	cwd
	sub	[bp-04],ax
	sbb	[bp-02],dx

l0800_AD7B:
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_AD85: 0800:AD85
;;   Called from:
;;     0800:4168 (in fn0800_4152)
fn0800_AD85 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	di
	mov	di,[bp+08]
	or	di,di
	jz	AE07

l0800_AD94:
	mov	bx,di
	xor	cx,cx
	mov	ax,[bp+0A]
	xor	dx,dx
	call	8F18
	mov	[bp-02],dx
	mov	[bp-04],ax
	cmp	dx,01
	ja	ADC9

l0800_ADAB:
	jc	ADB1

l0800_ADAD:
	or	ax,ax
	jnc	ADC9

l0800_ADB1:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [bp-04]
	push	word ptr [bp+0E]
	push	word ptr [bp+0C]
	call	B4BE
	xor	dx,dx
	div	di
	jmp	AE0A

l0800_ADC9:
	xor	si,si
	cmp	si,[bp+0A]
	jnc	AE07

l0800_ADD0:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	di
	push	word ptr [bp+0E]
	push	word ptr [bp+0C]
	call	B4BE
	xor	dx,dx
	or	dx,dx
	jnz	ADEA

l0800_ADE6:
	cmp	ax,di
	jz	ADEE

l0800_ADEA:
	mov	ax,si
	jmp	AE0A

l0800_ADEE:
	mov	bx,di
	xor	cx,cx
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	call	8CCB
	mov	[bp+06],dx
	mov	[bp+04],ax
	inc	si
	cmp	si,[bp+0A]
	jc	ADD0

l0800_AE07:
	mov	ax,[bp+0A]

l0800_AE0A:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_AE10: 0800:AE10
;;   Called from:
;;     0800:AE5A (in fn0800_AE4C)
;;     0800:AF4E (in fn0800_AED6)
fn0800_AE10 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	mov	si,0014
	mov	[bp-02],ds
	mov	word ptr [bp-04],2358
	jmp	AE40

l0800_AE24:
	les	bx,[bp-04]
	mov	ax,es:[bx+02]
	and	ax,0300
	cmp	ax,0300
	jnz	AE3C

l0800_AE33:
	push	word ptr [bp-02]
	push	bx
	call	A6B7
	pop	cx
	pop	cx

l0800_AE3C:
	add	word ptr [bp-04],14

l0800_AE40:
	mov	ax,si
	dec	si
	or	ax,ax
	jnz	AE24

l0800_AE47:
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_AE4C: 0800:AE4C
;;   Called from:
;;     0800:AF38 (in fn0800_AED6)
fn0800_AE4C proc
	push	bp
	mov	bp,sp
	les	bx,[bp+04]
	test	word ptr es:[bx+02],0200
	jz	AE5D

l0800_AE5A:
	call	AE10

l0800_AE5D:
	les	bx,[bp+04]
	push	word ptr es:[bx+06]
	mov	ax,es:[bx+0A]
	mov	dx,es:[bx+08]
	mov	es:[bx+0E],ax
	mov	es:[bx+0C],dx
	push	ax
	push	dx
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	B97F
	add	sp,08
	les	bx,[bp+04]
	mov	es:[bx],ax
	or	ax,ax
	jle	AE95

l0800_AE8C:
	and	word ptr es:[bx+02],DF
	xor	ax,ax
	jmp	AEBE

l0800_AE95:
	les	bx,[bp+04]
	cmp	word ptr es:[bx],00
	jnz	AEAE

l0800_AE9E:
	mov	ax,es:[bx+02]
	and	ax,FE7F
	or	ax,0020
	mov	es:[bx+02],ax
	jmp	AEBB

l0800_AEAE:
	les	bx,[bp+04]
	mov	word ptr es:[bx],0000
	or	word ptr es:[bx+02],10

l0800_AEBB:
	mov	ax,FFFF

l0800_AEBE:
	pop	bp
	ret	0004

;; fn0800_AEC2: 0800:AEC2
;;   Called from:
;;     0800:3DF9 (in fn0800_3DCF)
;;     0800:3EC4 (in fn0800_3E9A)
;;     0800:A7CC (in fn0800_A77D)
;;     0800:AB80 (in fn0800_AAB3)
fn0800_AEC2 proc
	push	bp
	mov	bp,sp
	les	bx,[bp+04]
	inc	word ptr es:[bx]
	push	word ptr [bp+06]
	push	bx
	call	AED6
	pop	cx
	pop	cx
	pop	bp
	ret

;; fn0800_AED6: 0800:AED6
;;   Called from:
;;     0800:AECF (in fn0800_AEC2)
fn0800_AED6 proc
	push	bp
	mov	bp,sp
	push	si
	mov	ax,[bp+04]
	or	ax,[bp+06]
	jnz	AEE8

l0800_AEE2:
	mov	ax,FFFF
	jmp	AFBD

l0800_AEE8:
	les	bx,[bp+04]
	cmp	word ptr es:[bx],00
	jle	AF0B

l0800_AEF1:
	les	bx,[bp+04]
	dec	word ptr es:[bx]
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	mov	es,ax
	mov	al,es:[si]
	jmp	AFBB

l0800_AF0B:
	les	bx,[bp+04]
	cmp	word ptr es:[bx],00
	jl	AF7F

l0800_AF14:
	test	word ptr es:[bx+02],0110
	jnz	AF7F

l0800_AF1C:
	test	word ptr es:[bx+02],0001
	jz	AF7F

l0800_AF24:
	les	bx,[bp+04]
	or	word ptr es:[bx+02],0080
	cmp	word ptr es:[bx+06],00
	jz	AF43

l0800_AF34:
	push	word ptr [bp+06]
	push	bx
	call	AE4C
	or	ax,ax
	jz	AEF1

l0800_AF3F:
	jmp	AEE2
0800:AF41    EB AE                                         ..            

l0800_AF43:
	les	bx,[bp+04]
	test	word ptr es:[bx+02],0200
	jz	AF51

l0800_AF4E:
	call	AE10

l0800_AF51:
	mov	ax,0001
	push	ax
	push	ds
	mov	ax,4EE4
	push	ax
	les	bx,[bp+04]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	B97F
	add	sp,08
	or	ax,ax
	jnz	AF9E

l0800_AF6D:
	les	bx,[bp+04]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	A59D
	pop	cx
	cmp	ax,0001
	jz	AF8A

l0800_AF7F:
	les	bx,[bp+04]
	or	word ptr es:[bx+02],10
	jmp	AEE2

l0800_AF8A:
	les	bx,[bp+04]
	mov	ax,es:[bx+02]
	and	ax,FE7F
	or	ax,0020
	mov	es:[bx+02],ax
	jmp	AEE2

l0800_AF9E:
	cmp	byte ptr [4EE4],0D
	jnz	AFB0

l0800_AFA5:
	les	bx,[bp+04]
	test	word ptr es:[bx+02],0040
	jz	AF43

l0800_AFB0:
	les	bx,[bp+04]
	and	word ptr es:[bx+02],DF
	mov	al,[4EE4]

l0800_AFBB:
	mov	ah,00

l0800_AFBD:
	pop	si
	pop	bp
	ret
0800:AFC0 1E B8 58 23 50 E8 0E FF 59 59 C3                ..X#P...YY.    

;; fn0800_AFCB: 0800:AFCB
;;   Called from:
;;     0800:C385 (in fn0800_C379)
fn0800_AFCB proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	di
	les	di,[bp+04]
	mov	ax,es
	or	ax,di
	jz	AFFC

l0800_AFDC:
	mov	al,00
	mov	ah,es:[di]
	mov	cx,FFFF
	cld

l0800_AFE5:
	repne scasb

l0800_AFE7:
	not	cx
	dec	cx
	jz	AFFC

l0800_AFEC:
	les	di,[26AA]
	mov	[bp-02],es
	mov	bx,es
	or	bx,di
	mov	[bp-04],di
	jnz	B009

l0800_AFFC:
	xor	dx,dx
	xor	ax,ax
	jmp	B035

l0800_B002:
	add	word ptr [bp-04],04
	les	di,[bp-04]

l0800_B009:
	les	di,es:[di]
	mov	bx,es
	or	bx,di
	jz	AFFC

l0800_B012:
	mov	al,es:[di]
	or	al,al
	jz	AFFC

l0800_B019:
	cmp	ah,al
	jnz	B002

l0800_B01D:
	mov	bx,cx
	cmp	byte ptr es:[bx+di],3D
	jnz	B002

l0800_B025:
	push	ds
	lds	si,[bp+04]

l0800_B029:
	rep cmpsb

l0800_B02B:
	pop	ds
	xchg	bx,cx
	jnz	B002

l0800_B030:
	inc	di
	mov	ax,di
	mov	dx,es

l0800_B035:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_B03B: 0800:B03B
;;   Called from:
;;     0800:9D05 (in fn0800_9CE6)
;;     0800:B5C3 (in fn0800_B4BE)
fn0800_B03B proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	dx,ds
	les	di,[bp+04]
	lds	si,[bp+08]
	mov	cx,[bp+0C]
	shr	cx,01
	cld

l0800_B04E:
	rep movsw

l0800_B050:
	jnc	B053

l0800_B052:
	movsb

l0800_B053:
	mov	ds,dx
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_B05F: 0800:B05F
;;   Called from:
;;     0800:B093 (in fn0800_B083)
fn0800_B05F proc
	push	bp
	mov	bp,sp
	push	di
	les	di,[bp+04]
	mov	cx,[bp+08]
	mov	al,[bp+0A]
	mov	ah,al
	cld
	test	di,0001
	jz	B079

l0800_B075:
	jcxz	B080

l0800_B077:
	stosb
	dec	cx

l0800_B079:
	shr	cx,01

l0800_B07B:
	rep stosw

l0800_B07D:
	jnc	B080

l0800_B07F:
	stosb

l0800_B080:
	pop	di
	pop	bp
	ret

;; fn0800_B083: 0800:B083
;;   Called from:
;;     0800:C476 (in fn0800_C379)
fn0800_B083 proc
	push	bp
	mov	bp,sp
	mov	al,[bp+08]
	push	ax
	push	word ptr [bp+0A]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	B05F
	add	sp,08
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	pop	bp
	ret

;; fn0800_B0A1: 0800:B0A1
;;   Called from:
;;     0800:B105 (in fn0800_B0F3)
fn0800_B0A1 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	push	ds
	mov	cx,[bp+0A]
	mov	bx,[bp+08]
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	call	8F2F
	jnc	B0BE

l0800_B0B8:
	std
	mov	ax,0001
	jmp	B0C1

l0800_B0BE:
	cld
	xor	ax,ax

l0800_B0C1:
	lds	si,[bp+04]
	les	di,[bp+08]
	mov	cx,[bp+0C]
	or	ax,ax
	jz	B0D4

l0800_B0CE:
	add	si,cx
	dec	si
	add	di,cx
	dec	di

l0800_B0D4:
	test	di,0001
	jz	B0DE

l0800_B0DA:
	jcxz	B0ED

l0800_B0DC:
	movsb
	dec	cx

l0800_B0DE:
	sub	si,ax
	sub	di,ax
	shr	cx,01

l0800_B0E4:
	rep movsw

l0800_B0E6:
	jnc	B0ED

l0800_B0E8:
	add	si,ax
	add	di,ax
	movsb

l0800_B0ED:
	cld
	pop	ds
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_B0F3: 0800:B0F3
;;   Called from:
;;     0800:0AA4 (in fn0800_09A3)
;;     0800:2109 (in fn0800_2085)
;;     0800:2271 (in fn0800_2201)
;;     0800:2378 (in fn0800_22FE)
;;     0800:5D86 (in fn0800_5D2F)
;;     0800:825A (in fn0800_7FDC)
fn0800_B0F3 proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+0C]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	B0A1
	add	sp,0A
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	pop	bp
	ret

;; fn0800_B113: 0800:B113
;;   Called from:
;;     0800:B1CA (in fn0800_B140)
;;     0800:B1E6 (in fn0800_B140)
fn0800_B113 proc
	push	bp
	mov	bp,sp
	push	ds
	mov	cx,[bp+04]
	mov	ah,3C
	lds	dx,[bp+06]
	int	21
	pop	ds
	jc	B126

l0800_B124:
	jmp	B12A

l0800_B126:
	push	ax
	call	8D2B

l0800_B12A:
	pop	bp
	ret	0006

;; fn0800_B12E: 0800:B12E
;;   Called from:
;;     0800:B23E (in fn0800_B140)
fn0800_B12E proc
	push	bp
	mov	bp,sp
	mov	bx,[bp+04]
	sub	cx,cx
	sub	dx,dx
	mov	ah,40
	int	21
	pop	bp
	ret	0002

;; fn0800_B140: 0800:B140
;;   Called from:
;;     0800:A9A8 (in fn0800_A96D)
fn0800_B140 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	di
	mov	si,[bp+08]
	mov	di,[bp+0A]
	test	si,C000
	jnz	B15C

l0800_B154:
	mov	ax,[2512]
	and	ax,C000
	or	si,ax

l0800_B15C:
	xor	ax,ax
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A53C
	add	sp,06
	mov	[bp-02],ax
	test	si,0100
	jz	B1F2

l0800_B174:
	and	di,[2514]
	mov	ax,di
	test	ax,0180
	jnz	B186

l0800_B17F:
	mov	ax,0001
	push	ax
	call	8D2B

l0800_B186:
	cmp	word ptr [bp-02],FF
	jnz	B1AF

l0800_B18C:
	cmp	word ptr [2516],02
	jz	B19D

l0800_B193:
	push	word ptr [2516]

l0800_B197:
	call	8D2B
	jmp	B29A

l0800_B19D:
	test	di,0080
	jz	B1A7

l0800_B1A3:
	xor	ax,ax
	jmp	B1AA

l0800_B1A7:
	mov	ax,0001

l0800_B1AA:
	mov	[bp-02],ax
	jmp	B1BB

l0800_B1AF:
	test	si,0400
	jz	B1F2

l0800_B1B5:
	mov	ax,0050
	push	ax
	jmp	B197

l0800_B1BB:
	test	si,00F0
	jz	B1DD

l0800_B1C1:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	xor	ax,ax
	push	ax
	call	B113
	mov	di,ax
	or	ax,ax
	jge	B1D6

l0800_B1D3:
	jmp	B298

l0800_B1D6:
	push	di
	call	A57F
	pop	cx
	jmp	B1F2

l0800_B1DD:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [bp-02]
	call	B113
	mov	di,ax
	or	ax,ax
	jge	B265

l0800_B1EF:
	jmp	B298

l0800_B1F2:
	push	si
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	B2A0
	add	sp,06
	mov	di,ax
	or	ax,ax
	jl	B265

l0800_B205:
	xor	ax,ax
	push	ax
	push	di
	call	A2A3
	pop	cx
	pop	cx
	mov	[bp-04],ax
	test	ax,0080
	jz	B237

l0800_B216:
	or	si,2000
	test	si,8000
	jz	B241

l0800_B220:
	and	ax,00FF
	or	ax,0020
	xor	dx,dx
	push	dx
	push	ax
	mov	ax,0001
	push	ax
	push	di
	call	A2A3
	add	sp,08
	jmp	B241

l0800_B237:
	test	si,0200
	jz	B241

l0800_B23D:
	push	di
	call	B12E

l0800_B241:
	test	word ptr [bp-02],0001
	jz	B265

l0800_B248:
	test	si,0100
	jz	B265

l0800_B24E:
	test	si,00F0
	jz	B265

l0800_B254:
	mov	ax,0001
	push	ax
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A53C
	add	sp,08

l0800_B265:
	or	di,di
	jl	B298

l0800_B269:
	test	si,0300
	jz	B274

l0800_B26F:
	mov	ax,1000
	jmp	B276

l0800_B274:
	xor	ax,ax

l0800_B276:
	mov	dx,si
	and	dx,F8FF
	or	dx,ax
	push	dx
	test	word ptr [bp-02],0001
	jz	B28A

l0800_B286:
	xor	ax,ax
	jmp	B28D

l0800_B28A:
	mov	ax,0100

l0800_B28D:
	pop	dx
	or	dx,ax
	mov	bx,di
	shl	bx,01
	mov	[bx+24EA],dx

l0800_B298:
	mov	ax,di

l0800_B29A:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_B2A0: 0800:B2A0
;;   Called from:
;;     0800:B1F9 (in fn0800_B140)
fn0800_B2A0 proc
	push	bp
	mov	bp,sp
	sub	sp,02
	mov	al,01
	mov	cx,[bp+08]
	test	cx,0002
	jnz	B2BB

l0800_B2B1:
	mov	al,02
	test	cx,0004
	jnz	B2BB

l0800_B2B9:
	mov	al,00

l0800_B2BB:
	push	ds
	lds	dx,[bp+04]
	mov	cl,F0
	and	cl,[bp+08]
	or	al,cl
	mov	ah,3D
	int	21
	pop	ds
	jc	B2E7

l0800_B2CD:
	mov	[bp-02],ax
	mov	ax,[bp+08]
	and	ax,B8FF
	or	ax,8000
	mov	bx,[bp-02]
	shl	bx,01
	mov	[bx+24EA],ax
	mov	ax,[bp-02]
	jmp	B2EB

l0800_B2E7:
	push	ax
	call	8D2B

l0800_B2EB:
	mov	sp,bp
	pop	bp
	ret

;; fn0800_B2EF: 0800:B2EF
;;   Called from:
;;     0800:02EA (in main)
;;     0800:03E3 (in main)
;;     0800:0B1D (in fn0800_0ABC)
;;     0800:0B41 (in fn0800_0ABC)
;;     0800:0B4C (in fn0800_0ABC)
;;     0800:0CD6 (in fn0800_0C93)
;;     0800:0D18 (in fn0800_0C93)
;;     0800:0D74 (in fn0800_0D24)
;;     0800:0D8C (in fn0800_0D24)
;;     0800:0D97 (in fn0800_0D24)
;;     0800:0DD8 (in fn0800_0DCE)
;;     0800:0E0E (in fn0800_0DE8)
;;     0800:0E24 (in fn0800_0DE8)
;;     0800:0E36 (in fn0800_0DE8)
;;     0800:0E41 (in fn0800_0DE8)
;;     0800:1153 (in fn0800_112D)
;;     0800:1169 (in fn0800_112D)
;;     0800:1174 (in fn0800_112D)
;;     0800:1305 (in fn0800_12E2)
;;     0800:131B (in fn0800_12E2)
;;     0800:1326 (in fn0800_12E2)
;;     0800:188F (in fn0800_12E2)
;;     0800:18FA (in fn0800_18D9)
;;     0800:1A10 (in fn0800_19EE)
;;     0800:30C3 (in fn0800_2DE2)
;;     0800:6EF7 (in fn0800_6EE6)
;;     0800:6F07 (in fn0800_6EFF)
;;     0800:6F18 (in fn0800_6EFF)
;;     0800:77EF (in fn0800_75EA)
;;     0800:79F7 (in fn0800_75EA)
;;     0800:81DD (in fn0800_7FDC)
fn0800_B2EF proc
	push	bp
	mov	bp,sp
	mov	ax,B4BE
	push	ax
	push	ds
	mov	ax,236C
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	lea	ax,[bp+08]
	push	ax
	call	9828
	pop	bp
	ret

;; fn0800_B30A: 0800:B30A
;;   Called from:
;;     0800:4076 (in fn0800_4047)
;;     0800:B669 (in fn0800_B4BE)
fn0800_B30A proc
	push	bp
	mov	bp,sp
	les	bx,[bp+06]
	dec	word ptr es:[bx]
	push	word ptr [bp+08]
	push	bx
	mov	al,[bp+04]
	cbw
	push	ax
	call	B324
	add	sp,06
	pop	bp
	ret

;; fn0800_B324: 0800:B324
;;   Called from:
;;     0800:B31C (in fn0800_B30A)
;;     0800:B4EA (in fn0800_B4BE)
fn0800_B324 proc
	push	bp
	mov	bp,sp
	push	si
	mov	al,[bp+04]
	mov	[4EE6],al
	les	bx,[bp+06]
	cmp	word ptr es:[bx],FF
	jge	B389

l0800_B337:
	inc	word ptr es:[bx]
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	mov	dl,[4EE6]
	mov	es,ax
	mov	es:[si],dl
	mov	es,[bp+08]
	test	word ptr es:[bx+02],0008
	jnz	B35D

l0800_B35A:
	jmp	B4A3

l0800_B35D:
	cmp	byte ptr [4EE6],0A
	jz	B36E

l0800_B364:
	cmp	byte ptr [4EE6],0D
	jz	B36E

l0800_B36B:
	jmp	B4A3

l0800_B36E:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	call	A6B7
	pop	cx
	pop	cx
	or	ax,ax
	jnz	B380

l0800_B37D:
	jmp	B4A3

l0800_B380:
	mov	ax,FFFF
	jmp	B4A8
0800:B386                   E9 1A 01                            ...      

l0800_B389:
	les	bx,[bp+06]
	test	word ptr es:[bx+02],0090
	jnz	B39C

l0800_B394:
	test	word ptr es:[bx+02],0002
	jnz	B3A6

l0800_B39C:
	les	bx,[bp+06]
	or	word ptr es:[bx+02],10
	jmp	B380

l0800_B3A6:
	les	bx,[bp+06]
	or	word ptr es:[bx+02],0100
	cmp	word ptr es:[bx+06],00
	jz	B421

l0800_B3B6:
	cmp	word ptr es:[bx],00
	jz	B3C9

l0800_B3BC:
	push	word ptr [bp+08]
	push	bx
	call	A6B7
	pop	cx
	pop	cx
	or	ax,ax
	jnz	B380

l0800_B3C9:
	les	bx,[bp+06]
	mov	ax,es:[bx+06]
	neg	ax
	mov	es:[bx],ax
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	mov	dl,[4EE6]
	mov	es,ax
	mov	es:[si],dl
	mov	es,[bp+08]
	test	word ptr es:[bx+02],0008
	jnz	B3F8

l0800_B3F5:
	jmp	B4A3

l0800_B3F8:
	cmp	byte ptr [4EE6],0A
	jz	B409

l0800_B3FF:
	cmp	byte ptr [4EE6],0D
	jz	B409

l0800_B406:
	jmp	B4A3

l0800_B409:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	call	A6B7
	pop	cx
	pop	cx
	or	ax,ax
	jnz	B41B

l0800_B418:
	jmp	B4A3

l0800_B41B:
	jmp	B380
0800:B41E                                           E9 82               ..
0800:B420 00                                              .              

l0800_B421:
	les	bx,[bp+06]
	mov	al,es:[bx+04]
	cbw
	shl	ax,01
	mov	bx,ax
	test	word ptr [bx+24EA],0800
	jz	B44C

l0800_B435:
	mov	ax,0002
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	mov	bx,[bp+06]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	8E29
	add	sp,08

l0800_B44C:
	cmp	byte ptr [4EE6],0A
	jnz	B478

l0800_B453:
	les	bx,[bp+06]
	test	word ptr es:[bx+02],0040
	jnz	B478

l0800_B45E:
	mov	ax,0001
	push	ax
	push	ds
	mov	ax,26AE
	push	ax
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	C779
	add	sp,08
	cmp	ax,0001
	jnz	B495

l0800_B478:
	mov	ax,0001
	push	ax
	push	ds
	mov	ax,4EE6
	push	ax
	les	bx,[bp+06]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	C779
	add	sp,08
	cmp	ax,0001
	jz	B4A3

l0800_B495:
	les	bx,[bp+06]
	test	word ptr es:[bx+02],0200
	jnz	B4A3

l0800_B4A0:
	jmp	B39C

l0800_B4A3:
	mov	al,[4EE6]
	mov	ah,00

l0800_B4A8:
	pop	si
	pop	bp
	ret
0800:B4AB                                  55 8B EC 1E B8            U....
0800:B4B0 6C 23 50 FF 76 04 E8 6B FE 83 C4 06 5D C3       l#P.v..k....]. 

;; fn0800_B4BE: 0800:B4BE
;;   Called from:
;;     0800:ADC0 (in fn0800_AD85)
;;     0800:ADDD (in fn0800_AD85)
fn0800_B4BE proc
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	push	di
	mov	di,[bp+08]
	mov	[bp-02],di
	les	bx,[bp+04]
	test	word ptr es:[bx+02],0008
	jz	B504

l0800_B4D7:
	jmp	B4FA

l0800_B4D9:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	les	bx,[bp+0A]
	inc	word ptr [bp+0A]
	mov	al,es:[bx]
	cbw
	push	ax
	call	B324
	add	sp,06
	cmp	ax,FFFF
	jnz	B4FA

l0800_B4F5:
	xor	ax,ax
	jmp	B6A0

l0800_B4FA:
	mov	ax,di
	dec	di
	or	ax,ax
	jnz	B4D9

l0800_B501:
	jmp	B69D

l0800_B504:
	les	bx,[bp+04]
	test	word ptr es:[bx+02],0040
	jnz	B512

l0800_B50F:
	jmp	B625

l0800_B512:
	cmp	word ptr es:[bx+06],00
	jnz	B51C

l0800_B519:
	jmp	B5DB

l0800_B51C:
	cmp	es:[bx+06],di
	jnc	B583

l0800_B522:
	cmp	word ptr es:[bx],00
	jz	B535

l0800_B528:
	push	word ptr [bp+06]
	push	bx
	call	A6B7
	pop	cx
	pop	cx
	or	ax,ax
	jnz	B4F5

l0800_B535:
	les	bx,[bp+04]
	mov	al,es:[bx+04]
	cbw
	shl	ax,01
	mov	bx,ax
	test	word ptr [bx+24EA],0800
	jz	B560

l0800_B549:
	mov	ax,0002
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	mov	bx,[bp+04]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	8E29
	add	sp,08

l0800_B560:
	push	di
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	les	bx,[bp+04]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	C779
	add	sp,08
	cmp	ax,di
	jc	B57D

l0800_B57A:
	jmp	B69D

l0800_B57D:
	jmp	B4F5
0800:B580 E9 1A 01                                        ...            

l0800_B583:
	les	bx,[bp+04]
	mov	ax,es:[bx]
	add	ax,di
	jl	B5B1

l0800_B58D:
	cmp	word ptr es:[bx],00
	jnz	B59F

l0800_B593:
	mov	ax,FFFF
	sub	ax,es:[bx+06]
	mov	es:[bx],ax
	jmp	B5B1

l0800_B59F:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A6B7
	pop	cx
	pop	cx
	or	ax,ax
	jz	B5B1

l0800_B5AE:
	jmp	B4F5

l0800_B5B1:
	push	di
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	les	bx,[bp+04]
	push	word ptr es:[bx+0E]
	push	word ptr es:[bx+0C]
	call	B03B
	add	sp,0A
	les	bx,[bp+04]
	mov	ax,es:[bx]
	add	ax,di
	mov	es:[bx],ax
	add	es:[bx+0C],di
	jmp	B69D

l0800_B5DB:
	les	bx,[bp+04]
	mov	al,es:[bx+04]
	cbw
	shl	ax,01
	mov	bx,ax
	test	word ptr [bx+24EA],0800
	jz	B606

l0800_B5EF:
	mov	ax,0002
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	mov	bx,[bp+04]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	8E29
	add	sp,08

l0800_B606:
	push	di
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	les	bx,[bp+04]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	C779
	add	sp,08
	cmp	ax,di
	jnc	B69D

l0800_B620:
	jmp	B4F5
0800:B623          EB 78                                     .x          

l0800_B625:
	les	bx,[bp+04]
	cmp	word ptr es:[bx+06],00
	jz	B680

l0800_B62F:
	jmp	B677

l0800_B631:
	les	bx,[bp+04]
	inc	word ptr es:[bx]
	jge	B659

l0800_B639:
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	les	bx,[bp+0A]
	inc	word ptr [bp+0A]
	mov	dl,es:[bx]
	mov	es,ax
	mov	es:[si],dl
	mov	al,dl
	mov	ah,00
	jmp	B66F

l0800_B659:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	les	bx,[bp+0A]
	inc	word ptr [bp+0A]
	mov	al,es:[bx]
	push	ax
	call	B30A
	add	sp,06

l0800_B66F:
	cmp	ax,FFFF
	jnz	B677

l0800_B674:
	jmp	B4F5

l0800_B677:
	mov	ax,di
	dec	di
	or	ax,ax
	jnz	B631

l0800_B67E:
	jmp	B69D

l0800_B680:
	push	di
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	les	bx,[bp+04]
	mov	al,es:[bx+04]
	cbw
	push	ax
	call	C632
	add	sp,08
	cmp	ax,di
	jnc	B69D

l0800_B69A:
	jmp	B4F5

l0800_B69D:
	mov	ax,[bp-02]

l0800_B6A0:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	000A

;; fn0800_B6A8: 0800:B6A8
;;   Called from:
;;     0800:B720 (in fn0800_B6D6)
;;     0800:B772 (in fn0800_B6D6)
;;     0800:B7BD (in fn0800_B6D6)
;;     0800:B7FC (in fn0800_B6D6)
;;     0800:B861 (in fn0800_B6D6)
;;     0800:B8D4 (in fn0800_B6D6)
fn0800_B6A8 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	push	ds
	cld
	mov	cx,[4EE8]
	les	di,[bp+04]
	lds	si,[bp+08]
	shr	cx,01
	jnc	B6C6

l0800_B6BD:
	mov	al,es:[di]
	movsb
	mov	[si-01],al
	jz	B6CF

l0800_B6C6:
	mov	ax,es:[di]
	movsw
	mov	[si-02],ax
	loop	B6C6

l0800_B6CF:
	pop	ds
	pop	di
	pop	si
	pop	bp
	ret	0008

;; fn0800_B6D6: 0800:B6D6
;;   Called from:
;;     0800:B935 (in fn0800_B6D6)
;;     0800:B944 (in fn0800_B6D6)
;;     0800:B97A (in fn0800_B95E)
fn0800_B6D6 proc
	push	bp
	mov	bp,sp
	sub	sp,14
	push	si
	push	di
	mov	si,[bp+04]

l0800_B6E1:
	cmp	si,02
	ja	B726

l0800_B6E6:
	cmp	si,02
	jz	B6EE

l0800_B6EB:
	jmp	B956

l0800_B6EE:
	mov	ax,[bp+08]
	mov	dx,[bp+06]
	add	dx,[4EE8]
	mov	[bp-06],ax
	mov	[bp-08],dx
	push	ax
	push	dx
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	call	word ptr [4EEA]
	add	sp,08
	or	ax,ax
	jg	B714

l0800_B711:
	jmp	B956

l0800_B714:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp-06]
	push	word ptr [bp-08]

l0800_B720:
	call	B6A8
	jmp	B956

l0800_B726:
	mov	ax,si
	dec	ax
	imul	word ptr [4EE8]
	mov	dx,[bp+08]
	mov	bx,[bp+06]
	add	bx,ax
	mov	[bp-06],dx
	mov	[bp-08],bx
	mov	ax,si
	shr	ax,01
	imul	word ptr [4EE8]
	mov	dx,[bp+08]
	mov	bx,[bp+06]
	add	bx,ax
	mov	[bp-02],dx
	mov	[bp-04],bx
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	push	word ptr [bp-02]
	push	bx
	call	word ptr [4EEA]
	add	sp,08
	or	ax,ax
	jle	B775

l0800_B766:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	B6A8

l0800_B775:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	word ptr [4EEA]
	add	sp,08
	or	ax,ax
	jle	B79A

l0800_B78C:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	jmp	B7BD

l0800_B79A:
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	call	word ptr [4EEA]
	add	sp,08
	or	ax,ax
	jle	B7C0

l0800_B7B1:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp-06]
	push	word ptr [bp-08]

l0800_B7BD:
	call	B6A8

l0800_B7C0:
	cmp	si,03
	jnz	B7D4

l0800_B7C5:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	jmp	B720

l0800_B7D4:
	mov	ax,[bp+08]
	mov	dx,[bp+06]
	add	dx,[4EE8]
	mov	[bp-0A],ax
	mov	[bp-0C],dx
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	B813

l0800_B7EC:
	or	di,di
	jnz	B805

l0800_B7F0:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp-0A]
	push	word ptr [bp-0C]
	call	B6A8
	mov	ax,[4EE8]
	add	[bp-0C],ax

l0800_B805:
	mov	ax,[bp-04]
	cmp	ax,[bp-08]
	jnc	B883

l0800_B80D:
	mov	ax,[4EE8]
	add	[bp-04],ax

l0800_B813:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	word ptr [4EEA]
	add	sp,08
	mov	di,ax
	or	ax,ax
	jle	B7EC

l0800_B82C:
	mov	ax,[bp-04]
	cmp	ax,[bp-08]
	jnc	B87B

l0800_B834:
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	call	word ptr [4EEA]
	add	sp,08
	mov	di,ax
	or	ax,ax
	jge	B855

l0800_B84D:
	mov	ax,[4EE8]
	sub	[bp-08],ax
	jmp	B873

l0800_B855:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	B6A8
	or	di,di
	jz	B87B

l0800_B868:
	mov	ax,[4EE8]
	add	[bp-04],ax
	sub	[bp-08],ax
	jmp	B87B

l0800_B873:
	mov	ax,[bp-04]
	cmp	ax,[bp-08]
	jc	B834

l0800_B87B:
	mov	ax,[bp-04]
	cmp	ax,[bp-08]
	jc	B813

l0800_B883:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	word ptr [4EEA]
	add	sp,08
	or	ax,ax
	jg	B8AA

l0800_B89A:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	add	dx,[4EE8]
	mov	[bp-02],ax
	mov	[bp-04],dx

l0800_B8AA:
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	sub	dx,[4EE8]
	mov	[bp-12],ax
	mov	[bp-14],dx
	mov	ax,[bp+08]
	mov	dx,[bp+06]
	mov	[bp-0E],ax
	mov	[bp-10],dx
	jmp	B8E0

l0800_B8C8:
	push	word ptr [bp-0E]
	push	word ptr [bp-10]
	push	word ptr [bp-12]
	push	word ptr [bp-14]
	call	B6A8
	mov	ax,[4EE8]
	add	[bp-10],ax
	sub	[bp-14],ax

l0800_B8E0:
	mov	ax,[bp-10]
	cmp	ax,[bp-0C]
	jnc	B8F0

l0800_B8E8:
	mov	ax,[bp-14]
	cmp	ax,[bp-0C]
	jnc	B8C8

l0800_B8F0:
	xor	ax,ax
	push	ax
	push	word ptr [4EE8]
	mov	ax,[bp-04]
	xor	dx,dx
	sub	ax,[bp-0C]
	sbb	dx,00
	push	dx
	push	ax
	call	8BBB
	mov	di,ax
	xor	ax,ax
	push	ax
	push	word ptr [4EE8]
	mov	ax,si
	imul	word ptr [4EE8]
	mov	dx,[bp+06]
	add	dx,ax
	xor	ax,ax
	sub	dx,[bp-04]
	sbb	ax,0000
	push	ax
	push	dx
	call	8BBB
	mov	si,ax
	cmp	si,di
	jnc	B93D

l0800_B92E:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	ax
	call	B6D6
	mov	si,di
	jmp	B6E1

l0800_B93D:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	di
	call	B6D6
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	mov	[bp+08],ax
	mov	[bp+06],dx
	jmp	B6E1

l0800_B956:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	0006

;; fn0800_B95E: 0800:B95E
;;   Called from:
;;     0800:60EC (in fn0800_5E64)
fn0800_B95E proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+0A]
	mov	[4EE8],ax
	or	ax,ax
	jz	B97D

l0800_B96B:
	mov	ax,[bp+0C]
	mov	[4EEA],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [bp+08]
	call	B6D6

l0800_B97D:
	pop	bp
	ret

;; fn0800_B97F: 0800:B97F
;;   Called from:
;;     0800:AE7C (in fn0800_AE4C)
;;     0800:AF63 (in fn0800_AED6)
fn0800_B97F proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	di
	mov	ax,[bp+04]
	cmp	ax,[24E8]
	jc	B99A

l0800_B990:
	mov	ax,0006
	push	ax
	call	8D2B
	jmp	BA44

l0800_B99A:
	mov	ax,[bp+0A]
	inc	ax
	cmp	ax,0002
	jc	B9B0

l0800_B9A3:
	mov	bx,[bp+04]
	shl	bx,01
	test	word ptr [bx+24EA],0200
	jz	B9B5

l0800_B9B0:
	xor	ax,ax
	jmp	BA44

l0800_B9B5:
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8F50
	add	sp,08
	mov	[bp-02],ax
	inc	ax
	cmp	ax,0002
	jc	B9DD

l0800_B9D0:
	mov	bx,[bp+04]
	shl	bx,01
	test	word ptr [bx+24EA],4000
	jnz	B9E2

l0800_B9DD:
	mov	ax,[bp-02]
	jmp	BA44

l0800_B9E2:
	mov	cx,[bp-02]
	les	si,[bp+06]
	mov	di,si
	mov	bx,si
	cld

l0800_B9ED:
	lodsb
	cmp	al,1A
	jz	BA21

l0800_B9F3:
	cmp	al,0D
	jz	B9FC

l0800_B9F7:
	stosb
	loop	B9ED

l0800_B9FA:
	jmp	BA19

l0800_B9FC:
	loop	B9ED

l0800_B9FE:
	push	es
	push	bx
	mov	ax,0001
	push	ax
	lea	ax,[bp-03]
	push	ss
	push	ax
	push	word ptr [bp+04]
	call	8F50
	add	sp,08
	pop	bx
	pop	es
	cld
	mov	al,[bp-03]
	stosb

l0800_BA19:
	cmp	di,bx
	jnz	BA1F

l0800_BA1D:
	jmp	B9B5

l0800_BA1F:
	jmp	BA41

l0800_BA21:
	push	bx
	mov	ax,0001
	push	ax
	neg	cx
	sbb	ax,ax
	push	ax
	push	cx
	push	word ptr [bp+04]
	call	8E29
	add	sp,08
	mov	bx,[bp+04]
	shl	bx,01
	or	word ptr [bx+24EA],0200
	pop	bx

l0800_BA41:
	sub	di,bx
	xchg	ax,di

l0800_BA44:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_BA4A: 0800:BA4A
;;   Called from:
;;     0800:386D (in fn0800_37DF)
;;     0800:3D98 (in fn0800_3C99)
fn0800_BA4A proc
	push	bp
	mov	bp,sp
	push	di
	push	ds
	mov	ah,56
	lds	dx,[bp+04]
	les	di,[bp+08]
	int	21
	pop	ds
	jc	BA60

l0800_BA5C:
	xor	ax,ax
	jmp	BA64

l0800_BA60:
	push	ax
	call	8D2B

l0800_BA64:
	pop	di
	pop	bp
	ret

;; fn0800_BA67: 0800:BA67
;;   Called from:
;;     0800:0F69 (in fn0800_0DE8)
;;     0800:0F77 (in fn0800_0DE8)
;;     0800:1083 (in fn0800_0DE8)
;;     0800:1091 (in fn0800_0DE8)
;;     0800:17BF (in fn0800_12E2)
;;     0800:3C3F (in fn0800_3BC3)
fn0800_BA67 proc
	push	bp
	mov	bp,sp
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ACB3
	add	sp,0A
	or	ax,ax
	jnz	BA87

l0800_BA7F:
	les	bx,[bp+04]
	and	word ptr es:[bx+02],EF

l0800_BA87:
	pop	bp
	ret

;; fn0800_BA89: 0800:BA89
;;   Called from:
;;     0800:AA08 (in fn0800_A96D)
fn0800_BA89 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	di,[bp+0C]
	mov	si,[bp+0E]
	les	bx,[bp+04]
	mov	ax,es:[bx+12]
	cmp	ax,[bp+04]
	jnz	BAAB

l0800_BAA0:
	cmp	di,02
	jg	BAAB

l0800_BAA5:
	cmp	si,7FFF
	jbe	BAB1

l0800_BAAB:
	mov	ax,FFFF
	jmp	BB94

l0800_BAB1:
	cmp	word ptr [26B2],00
	jnz	BAC7

l0800_BAB8:
	cmp	word ptr [bp+04],236C
	jnz	BAC7

l0800_BABF:
	mov	word ptr [26B2],0001
	jmp	BADB

l0800_BAC7:
	cmp	word ptr [26B0],00
	jnz	BADB

l0800_BACE:
	cmp	word ptr [bp+04],2358
	jnz	BADB

l0800_BAD5:
	mov	word ptr [26B0],0001

l0800_BADB:
	les	bx,[bp+04]
	cmp	word ptr es:[bx],00
	jz	BAF6

l0800_BAE4:
	mov	ax,0001
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+06]
	push	bx
	call	ACB3
	add	sp,0A

l0800_BAF6:
	les	bx,[bp+04]
	test	word ptr es:[bx+02],0004
	jz	BB0E

l0800_BB01:
	push	word ptr es:[bx+0A]
	push	word ptr es:[bx+08]
	call	9E75
	pop	cx
	pop	cx

l0800_BB0E:
	les	bx,[bp+04]
	and	word ptr es:[bx+02],F3
	mov	word ptr es:[bx+06],0000
	mov	ax,[bp+06]
	mov	dx,[bp+04]
	add	dx,05
	mov	es:[bx+0A],ax
	mov	es:[bx+08],dx
	mov	es:[bx+0E],ax
	mov	es:[bx+0C],dx
	cmp	di,02
	jz	BB92

l0800_BB3A:
	or	si,si
	jbe	BB92

l0800_BB3E:
	mov	word ptr [2352],C7F0
	mov	ax,[bp+08]
	or	ax,[bp+0A]
	jnz	BB6B

l0800_BB4C:
	push	si
	call	9F7F
	pop	cx
	mov	[bp+0A],dx
	mov	[bp+08],ax
	or	ax,dx
	jnz	BB5E

l0800_BB5B:
	jmp	BAAB

l0800_BB5E:
	les	bx,[bp+04]
	or	word ptr es:[bx+02],04
	jmp	BB6B
0800:BB68                         E9 40 FF                        .@.    

l0800_BB6B:
	les	bx,[bp+04]
	mov	ax,[bp+0A]
	mov	dx,[bp+08]
	mov	es:[bx+0E],ax
	mov	es:[bx+0C],dx
	mov	es:[bx+0A],ax
	mov	es:[bx+08],dx
	mov	es:[bx+06],si
	cmp	di,01
	jnz	BB92

l0800_BB8D:
	or	word ptr es:[bx+02],08

l0800_BB92:
	xor	ax,ax

l0800_BB94:
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_BB98: 0800:BB98
fn0800_BB98 proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+04]
	mov	ax,[bp+0A]
	or	ax,[bp+0C]
	jz	BBE4

l0800_BBA7:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	call	BFC7
	pop	cx
	pop	cx
	cmp	ax,si
	jc	BBD2

l0800_BBB6:
	push	si
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	call	C01E
	add	sp,0A
	les	bx,[bp+0A]
	mov	byte ptr es:[bx+si],00
	jmp	BBE4

l0800_BBD2:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	call	BF9E
	add	sp,08

l0800_BBE4:
	pop	si
	pop	bp
	ret	000A

;; fn0800_BBE9: 0800:BBE9
fn0800_BBE9 proc
	push	bp
	mov	bp,sp
	sub	sp,02
	les	bx,[bp+04]
	cmp	byte ptr es:[bx-01],2E
	jnz	BBFC

l0800_BBF9:
	dec	word ptr [bp+04]

l0800_BBFC:
	dec	word ptr [bp+04]
	les	bx,[bp+04]
	mov	al,es:[bx]
	cbw
	mov	[bp-02],ax
	mov	cx,0004
	mov	bx,BC39

l0800_BC0F:
	mov	ax,cs:[bx]
	cmp	ax,[bp-02]
	jz	BC1E

l0800_BC17:
	add	bx,02
	loop	BC0F

l0800_BC1C:
	jmp	BC31

l0800_BC1E:
	jmp	word ptr cs:[bx+08]
0800:BC22       C4 5E 04 26 80 7F FE 00 75 05 B8 01 00 EB   .^.&....u.....
0800:BC30 02                                              .              

l0800_BC31:
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret	0004
0800:BC39                            00 00 2F 00 3A 00 5C          ../.:.\
0800:BC40 00 2C BC 2C BC 22 BC 2C BC                      .,.,.".,.      

;; fn0800_BC49: 0800:BC49
;;   Called from:
;;     0800:BE5C (in fn0800_BE3B)
fn0800_BC49 proc
	push	bp
	mov	bp,sp
	sub	sp,58
	push	si
	push	di
	xor	di,di
	mov	ax,[bp+08]
	or	ax,[bp+0A]
	jz	BC62

l0800_BC5B:
	les	bx,[bp+08]
	mov	byte ptr es:[bx],00

l0800_BC62:
	mov	ax,[bp+0C]
	or	ax,[bp+0E]
	jz	BC71

l0800_BC6A:
	les	bx,[bp+0C]
	mov	byte ptr es:[bx],00

l0800_BC71:
	mov	ax,[bp+10]
	or	ax,[bp+12]
	jz	BC80

l0800_BC79:
	les	bx,[bp+10]
	mov	byte ptr es:[bx],00

l0800_BC80:
	mov	ax,[bp+14]
	or	ax,[bp+16]
	jz	BC8F

l0800_BC88:
	les	bx,[bp+14]
	mov	byte ptr es:[bx],00

l0800_BC8F:
	lea	ax,[bp-58]
	mov	[bp-02],ss
	mov	[bp-04],ax
	jmp	BC9D

l0800_BC9A:
	inc	word ptr [bp+04]

l0800_BC9D:
	les	bx,[bp+04]
	cmp	byte ptr es:[bx],20
	jz	BC9A

l0800_BCA6:
	push	word ptr [bp+06]
	push	bx
	call	BFC7
	pop	cx
	pop	cx
	mov	si,ax
	cmp	ax,0050
	jle	BCB9

l0800_BCB6:
	mov	si,0050

l0800_BCB9:
	les	bx,[bp-04]
	mov	byte ptr es:[bx],00
	inc	word ptr [bp-04]
	push	si
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	C01E
	add	sp,0A
	add	[bp-04],si
	les	bx,[bp-04]
	mov	byte ptr es:[bx],00
	xor	si,si

l0800_BCE2:
	dec	word ptr [bp-04]
	les	bx,[bp-04]
	mov	al,es:[bx]
	cbw
	mov	[bp-06],ax
	mov	cx,0007
	mov	bx,BE1F

l0800_BCF5:
	mov	ax,cs:[bx]
	cmp	ax,[bp-06]
	jz	BD04

l0800_BCFD:
	add	bx,02
	loop	BCF5

l0800_BD02:
	jmp	BCE2

l0800_BD04:
	jmp	word ptr cs:[bx+0E]
0800:BD08                         0B F6 75 13 C4 5E FC 26         ..u..^.&
0800:BD10 80 7F 01 00 75 09 FF 76 FE 53 E8 CC FE 8B F0 0B ....u..v.S......
0800:BD20 F6 75 BF F7 C7 02 00 75 B9 83 CF 02 FF 76 16 FF .u.....u.....v..
0800:BD30 76 14 FF 76 FE FF 76 FC B8 04 00 50 E8 59 FE C4 v..v..v....P.Y..
0800:BD40 5E FC 26 C6 07 00 EB 9A 8C D0 8D 56 AA 3B 46 FE ^.&........V.;F.
0800:BD50 75 90 3B 56 FC 75 8B 0B F6 74 2E FF 46 FC C4 5E u.;V.u...t..F..^
0800:BD60 FC 26 80 3F 00 74 03 83 CF 08 FF 76 0E FF 76 0C .&.?.t.....v..v.
0800:BD70 FF 76 FE FF 76 FC B8 41 00 50 E8 1B FE C4 5E FC .v..v..A.P....^.
0800:BD80 26 C6 07 00 FF 4E FC EB 6A 0B F6 74 03 E9 52 FF &....N..j..t..R.
0800:BD90 46 FF 46 FC C4 5E FC 26 80 3F 00 74 03 83 CF 04 F.F..^.&.?.t....
0800:BDA0 FF 76 12 FF 76 10 FF 76 FE FF 76 FC B8 08 00 50 .v..v..v..v....P
0800:BDB0 E8 E5 FD C4 5E FC 26 C6 07 00 FF 4E FC C4 5E FC ....^.&....N..^.
0800:BDC0 26 80 3F 00 74 2D 26 80 3F 3A 74 03 E9 13 FF 8C &.?.t-&.?:t.....
0800:BDD0 D0 8D 56 AA 3B 46 FE 74 03 E9 06 FF 3B 56 FC 74 ..V.;F.t....;V.t
0800:BDE0 03 E9 FE FE EB 0D 0B F6 74 03 E9 F5 FE 83 CF 01 ........t.......
0800:BDF0 E9 EF FE C4 5E FC 26 80 3F 3A 75 1B 80 7E A9 00 ....^.&.?:u..~..
0800:BE00 74 03 83 CF 10 FF 76 0A FF 76 08 16 8D 46 A9 50 t.....v..v...F.P
0800:BE10 B8 02 00 50 E8 81 FD 8B C7 5F 5E 8B E5 5D C3 00 ...P....._^..]..
0800:BE20 00 2A 00 2E 00 2F 00 3A 00 3F 00 5C 00 57 BD E6 .*.../.:.?.\.W..
0800:BE30 BD 08 BD 89 BD 48 BD E6 BD 89 BD                .....H.....    

;; fn0800_BE3B: 0800:BE3B
;;   Called from:
;;     0800:352B (in fn0800_3509)
;;     0800:35C5 (in fn0800_35A3)
fn0800_BE3B proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+16]
	push	word ptr [bp+14]
	push	word ptr [bp+12]
	push	word ptr [bp+10]
	push	word ptr [bp+0E]
	push	word ptr [bp+0C]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	BC49
	add	sp,14
	pop	bp
	ret
0800:BE64             55 8B EC 56 8B 76 08 56 FF 76 0C FF     U..V.v.V.v..
0800:BE70 76 0A C4 5E 04 26 FF 77 02 26 FF 37 E8 BC F1 83 v..^.&.w.&.7....
0800:BE80 C4 0A C4 5E 04 26 01 37 53 06 26 8E 47 02 8C C0 ...^.&.7S.&.G...
0800:BE90 07 5B 26 8B 1F 8E C0 26 C6 07 00 8B C6 5E 5D C2 .[&....&.....^].
0800:BEA0 0A 00                                           ..             

;; fn0800_BEA2: 0800:BEA2
;;   Called from:
;;     0800:070D (in fn0800_0541)
;;     0800:321F (in fn0800_31B4)
;;     0800:37D7 (in fn0800_37BE)
fn0800_BEA2 proc
	push	bp
	mov	bp,sp
	les	bx,[bp+04]
	mov	byte ptr es:[bx],00
	mov	ax,BE64
	push	ax
	push	ss
	lea	ax,[bp+04]
	push	ax
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	lea	ax,[bp+0C]
	push	ax
	call	9828
	pop	bp
	ret
0800:BEC4             55 8B EC C4 5E 04 26 C6 07 00 B8 64     U...^.&....d
0800:BED0 BE 50 16 8D 46 04 50 FF 76 0A FF 76 08 FF 76 0C .P..F.P.v..v..v.
0800:BEE0 E8 45 D9 5D C3 55 8B EC 56 C4 5E 04 26 8B 47 02 .E.].U..V.^.&.G.
0800:BEF0 26 8B 37 26 FF 07 8E C0 26 8A 14 8A C2 0A C0 75 &.7&....&......u
0800:BF00 05 B8 FF FF EB 04 8A C2 B4 00 5E 5D C3 55 8B EC ..........^].U..
0800:BF10 C4 5E 06 26 FF 0F 5D C3                         .^.&..].       

;; fn0800_BF18: 0800:BF18
;;   Called from:
;;     0800:0726 (in fn0800_0541)
;;     0800:075C (in fn0800_0541)
;;     0800:0775 (in fn0800_0541)
fn0800_BF18 proc
	push	bp
	mov	bp,sp
	push	ss
	lea	ax,[bp+0C]
	push	ax
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ss
	lea	ax,[bp+04]
	push	ax
	mov	ax,BF0D
	push	ax
	mov	ax,BEE5
	push	ax
	call	8F97
	add	sp,10
	pop	bp
	ret
0800:BF3B                                  55 8B EC FF 76            U...v
0800:BF40 0E FF 76 0C FF 76 0A FF 76 08 16 8D 46 04 50 B8 ..v..v..v...F.P.
0800:BF50 0D BF 50 B8 E5 BE 50 E8 3D D0 83 C4 10 5D C3    ..P...P.=....].

;; fn0800_BF5F: 0800:BF5F
;;   Called from:
;;     0800:8EB3 (in fn0800_8E6A)
fn0800_BF5F proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	cld
	push	ds
	les	di,[bp+04]
	mov	dx,di
	xor	al,al
	mov	cx,FFFF

l0800_BF70:
	repne scasb

l0800_BF72:
	push	es
	lea	si,[di-01]
	les	di,[bp+08]
	mov	cx,FFFF

l0800_BF7C:
	repne scasb

l0800_BF7E:
	not	cx
	sub	di,cx
	push	es
	pop	ds
	pop	es
	xchg	di,si
	test	si,0001
	jz	BF8F

l0800_BF8D:
	movsb
	dec	cx

l0800_BF8F:
	shr	cx,01

l0800_BF91:
	rep movsw

l0800_BF93:
	jnc	BF96

l0800_BF95:
	movsb

l0800_BF96:
	xchg	ax,dx
	mov	dx,es
	pop	ds
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_BF9E: 0800:BF9E
;;   Called from:
;;     0800:04C9 (in fn0800_0402)
;;     0800:2F84 (in fn0800_2DE2)
;;     0800:BBDE (in fn0800_BB98)
;;     0800:C44B (in fn0800_C379)
;;     0800:C45E (in fn0800_C379)
fn0800_BF9E proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	cld
	les	di,[bp+08]
	mov	si,di
	xor	al,al
	mov	cx,FFFF

l0800_BFAE:
	repne scasb

l0800_BFB0:
	not	cx
	push	ds
	mov	ax,es
	mov	ds,ax
	les	di,[bp+04]

l0800_BFBA:
	rep movsb

l0800_BFBC:
	pop	ds
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_BFC7: 0800:BFC7
;;   Called from:
;;     0800:0424 (in fn0800_0402)
;;     0800:05EB (in fn0800_0541)
;;     0800:9CF0 (in fn0800_9CE6)
;;     0800:BBAD (in fn0800_BB98)
;;     0800:BCAA (in fn0800_BC49)
;;     0800:C39D (in fn0800_C379)
;;     0800:C4E3 (in fn0800_C379)
fn0800_BFC7 proc
	push	bp
	mov	bp,sp
	push	di
	les	di,[bp+04]
	xor	ax,ax
	cmp	ax,[bp+06]
	jnz	BFD9

l0800_BFD5:
	cmp	ax,di
	jz	BFE3

l0800_BFD9:
	cld
	mov	cx,FFFF

l0800_BFDD:
	repne scasb

l0800_BFDF:
	xchg	ax,cx
	not	ax
	dec	ax

l0800_BFE3:
	pop	di
	pop	bp
	ret

;; fn0800_BFE6: 0800:BFE6
;;   Called from:
;;     0800:2BB1 (in fn0800_29C5)
fn0800_BFE6 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	dx,ds
	cld
	les	di,[bp+08]
	mov	si,di
	mov	ax,[bp+0C]
	mov	cx,ax
	jcxz	C018

l0800_BFFA:
	mov	bx,ax
	xor	al,al

l0800_BFFE:
	repne scasb

l0800_C000:
	sub	bx,cx
	mov	cx,bx
	mov	di,si
	lds	si,[bp+04]

l0800_C009:
	rep cmpsb

l0800_C00B:
	mov	al,[si-01]
	mov	bl,es:[di-01]
	xor	ah,ah
	mov	bh,ah
	sub	ax,bx

l0800_C018:
	mov	ds,dx
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_C01E: 0800:C01E
;;   Called from:
;;     0800:BBC3 (in fn0800_BB98)
;;     0800:BCD0 (in fn0800_BC49)
;;     0800:C48E (in fn0800_C379)
;;     0800:C52A (in fn0800_C379)
fn0800_C01E proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	cld
	les	di,[bp+08]
	mov	si,di
	xor	al,al
	mov	bx,[bp+0C]
	mov	cx,bx

l0800_C030:
	repne scasb

l0800_C032:
	sub	bx,cx
	push	ds
	mov	di,es
	mov	ds,di
	les	di,[bp+04]
	xchg	bx,cx

l0800_C03E:
	rep movsb

l0800_C040:
	mov	cx,bx

l0800_C042:
	rep stosb

l0800_C044:
	pop	ds
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_C04F: 0800:C04F
;;   Called from:
;;     0800:9788 (in fn0800_9764)
fn0800_C04F proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	call	C379
	mov	ax,[26E4]
	mov	dx,[26E2]
	add	dx,A600
	adc	ax,12CE
	mov	[bp-02],ax
	mov	[bp-04],dx
	les	bx,[bp+04]
	mov	si,es:[bx]
	add	si,F844
	mov	ax,si
	sar	ax,01
	sar	ax,01
	cwd
	push	ax
	push	dx
	mov	dx,0786
	mov	ax,1F80
	pop	cx
	pop	bx
	call	8F18
	add	[bp-04],ax
	adc	[bp-02],dx
	mov	ax,si
	and	ax,0003
	cwd
	push	ax
	push	dx
	mov	dx,01E1
	mov	ax,3380
	pop	cx
	pop	bx
	call	8F18
	add	[bp-04],ax
	adc	[bp-02],dx
	test	si,0003
	jz	C0B9

l0800_C0B0:
	add	word ptr [bp-04],5180
	adc	word ptr [bp-02],01

l0800_C0B9:
	xor	cx,cx
	les	bx,[bp+04]
	mov	al,es:[bx+03]
	cbw
	dec	ax
	mov	si,ax
	jmp	C0D0

l0800_C0C8:
	dec	si
	mov	al,[si+26B4]
	cbw
	add	cx,ax

l0800_C0D0:
	or	si,si
	jg	C0C8

l0800_C0D4:
	les	bx,[bp+04]
	mov	al,es:[bx+02]
	cbw
	dec	ax
	add	cx,ax
	cmp	byte ptr es:[bx+03],02
	jle	C0EE

l0800_C0E6:
	test	word ptr es:[bx],0003
	jnz	C0EE

l0800_C0ED:
	inc	cx

l0800_C0EE:
	les	bx,[bp+08]
	mov	al,es:[bx+01]
	mov	ah,00
	push	ax
	mov	ax,cx
	mov	dx,0018
	imul	dx
	pop	dx
	add	ax,dx
	mov	si,ax
	cmp	word ptr [26E6],00
	jz	C128

l0800_C10B:
	mov	al,es:[bx+01]
	mov	ah,00
	push	ax
	push	cx
	xor	ax,ax
	push	ax
	les	bx,[bp+04]
	mov	ax,es:[bx]
	add	ax,F84E
	push	ax
	call	C553
	or	ax,ax
	jz	C128

l0800_C127:
	dec	si

l0800_C128:
	mov	ax,si
	cwd
	push	ax
	push	dx
	xor	dx,dx
	mov	ax,0E10
	pop	cx
	pop	bx
	call	8F18
	add	[bp-04],ax
	adc	[bp-02],dx
	les	bx,[bp+08]
	mov	al,es:[bx]
	mov	ah,00
	cwd
	push	ax
	push	dx
	xor	dx,dx
	mov	ax,003C
	pop	cx
	pop	bx
	call	8F18
	les	bx,[bp+08]
	mov	bl,es:[bx+03]
	mov	bh,00
	push	ax
	mov	ax,bx
	push	dx
	cwd
	pop	bx
	pop	cx
	add	cx,ax
	adc	bx,dx
	add	[bp-04],cx
	adc	[bp-02],bx
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_C177: 0800:C177
fn0800_C177 proc
	push	bp
	mov	bp,sp
	call	C379
	mov	ax,[26E4]
	mov	dx,[26E2]
	add	dx,A600
	adc	ax,12CE
	sub	[bp+04],dx
	sbb	[bp+06],ax
	les	bx,[bp+0C]
	mov	byte ptr es:[bx+02],00
	xor	ax,ax
	mov	dx,003C
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BCA
	les	bx,[bp+0C]
	mov	es:[bx+03],al
	xor	ax,ax
	mov	dx,003C
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BBB
	mov	[bp+06],dx
	mov	[bp+04],ax
	xor	ax,ax
	mov	dx,003C
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BCA
	les	bx,[bp+0C]
	mov	es:[bx],al
	xor	ax,ax
	mov	dx,003C
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BBB
	mov	[bp+06],dx
	mov	[bp+04],ax
	xor	ax,ax
	mov	dx,88F8
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BBB
	shl	ax,01
	shl	ax,01
	add	ax,07BC
	les	bx,[bp+08]
	mov	es:[bx],ax
	xor	ax,ax
	mov	dx,88F8
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BCA
	mov	[bp+06],dx
	mov	[bp+04],ax
	cmp	word ptr [bp+06],00
	jl	C26F

l0800_C22B:
	jnz	C234

l0800_C22D:
	cmp	word ptr [bp+04],2250
	jc	C26F

l0800_C234:
	sub	word ptr [bp+04],2250
	sbb	word ptr [bp+06],00
	les	bx,[bp+08]
	inc	word ptr es:[bx]
	xor	ax,ax
	mov	dx,2238
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BBB
	les	bx,[bp+08]
	add	es:[bx],ax
	xor	ax,ax
	mov	dx,2238
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BCA
	mov	[bp+06],dx
	mov	[bp+04],ax

l0800_C26F:
	cmp	word ptr [26E6],00
	jz	C2B4

l0800_C276:
	xor	ax,ax
	mov	dx,0018
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BCA
	push	ax
	xor	ax,ax
	mov	dx,0018
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BBB
	push	ax
	xor	ax,ax
	push	ax
	les	bx,[bp+08]
	mov	ax,es:[bx]
	add	ax,F84E
	push	ax
	call	C553
	or	ax,ax
	jz	C2B4

l0800_C2AC:
	add	word ptr [bp+04],01
	adc	word ptr [bp+06],00

l0800_C2B4:
	xor	ax,ax
	mov	dx,0018
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BCA
	les	bx,[bp+0C]
	mov	es:[bx+01],al
	xor	ax,ax
	mov	dx,0018
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	8BBB
	mov	[bp+06],dx
	mov	[bp+04],ax
	add	word ptr [bp+04],01
	adc	word ptr [bp+06],00
	les	bx,[bp+08]
	test	word ptr es:[bx],0003
	jnz	C326

l0800_C2F3:
	cmp	word ptr [bp+06],00
	jl	C30B

l0800_C2F9:
	jg	C301

l0800_C2FB:
	cmp	word ptr [bp+04],3C
	jbe	C30B

l0800_C301:
	sub	word ptr [bp+04],01
	sbb	word ptr [bp+06],00
	jmp	C326

l0800_C30B:
	cmp	word ptr [bp+06],00
	jnz	C326

l0800_C311:
	cmp	word ptr [bp+04],3C
	jnz	C326

l0800_C317:
	les	bx,[bp+08]
	mov	byte ptr es:[bx+03],02
	mov	byte ptr es:[bx+02],1D
	jmp	C377

l0800_C326:
	les	bx,[bp+08]
	mov	byte ptr es:[bx+03],00
	jmp	C34D

l0800_C330:
	les	bx,[bp+08]
	mov	al,es:[bx+03]
	cbw
	mov	bx,ax
	mov	al,[bx+26B4]
	cbw
	cwd
	sub	[bp+04],ax
	sbb	[bp+06],dx
	mov	bx,[bp+08]
	inc	byte ptr es:[bx+03]

l0800_C34D:
	les	bx,[bp+08]
	mov	al,es:[bx+03]
	cbw
	mov	bx,ax
	mov	al,[bx+26B4]
	cbw
	cwd
	cmp	dx,[bp+06]
	jl	C330

l0800_C362:
	jnz	C369

l0800_C364:
	cmp	ax,[bp+04]
	jc	C330

l0800_C369:
	les	bx,[bp+08]
	inc	byte ptr es:[bx+03]
	mov	al,[bp+04]
	mov	es:[bx+02],al

l0800_C377:
	pop	bp
	ret

;; fn0800_C379: 0800:C379
;;   Called from:
;;     0800:C056 (in fn0800_C04F)
;;     0800:C17A (in fn0800_C177)
fn0800_C379 proc
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	ds
	mov	ax,26E8
	push	ax
	call	AFCB
	pop	cx
	pop	cx
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,dx
	jnz	C397

l0800_C394:
	jmp	C42C

l0800_C397:
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	BFC7
	pop	cx
	pop	cx
	cmp	ax,0004
	jnc	C3AA

l0800_C3A7:
	jmp	C42C

l0800_C3AA:
	les	bx,[bp-04]
	mov	al,es:[bx]
	cbw
	mov	bx,ax
	mov	al,[bx+2251]
	cbw
	test	ax,000C
	jz	C42C

l0800_C3BD:
	mov	bx,[bp-04]
	mov	al,es:[bx+01]
	cbw
	mov	bx,ax
	mov	al,[bx+2251]
	cbw
	test	ax,000C
	jz	C42C

l0800_C3D1:
	mov	bx,[bp-04]
	mov	al,es:[bx+02]
	cbw
	mov	bx,ax
	mov	al,[bx+2251]
	cbw
	test	ax,000C
	jz	C42C

l0800_C3E5:
	mov	bx,[bp-04]
	cmp	byte ptr es:[bx+03],2D
	jz	C404

l0800_C3EF:
	cmp	byte ptr es:[bx+03],2B
	jz	C404

l0800_C3F6:
	mov	al,es:[bx+03]
	cbw
	mov	bx,ax
	test	byte ptr [bx+2251],02
	jz	C42C

l0800_C404:
	les	bx,[bp-04]
	mov	al,es:[bx+03]
	cbw
	mov	bx,ax
	mov	al,[bx+2251]
	cbw
	test	ax,0002
	jnz	C467

l0800_C418:
	mov	bx,[bp-04]
	mov	al,es:[bx+04]
	cbw
	mov	bx,ax
	mov	al,[bx+2251]
	cbw
	test	ax,0002
	jnz	C467

l0800_C42C:
	mov	word ptr [26E6],0001
	mov	word ptr [26E4],0000
	mov	word ptr [26E2],4650
	push	ds
	mov	ax,26EB
	push	ax
	push	word ptr [26DC]
	push	word ptr [26DA]
	call	BF9E
	add	sp,08
	push	ds
	mov	ax,26EF
	push	ax
	push	word ptr [26E0]
	push	word ptr [26DE]
	call	BF9E
	add	sp,08
	jmp	C54E

l0800_C467:
	mov	ax,0004
	push	ax
	xor	ax,ax
	push	ax
	push	word ptr [26E0]
	push	word ptr [26DE]
	call	B083
	add	sp,08
	mov	ax,0003
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [26DC]
	push	word ptr [26DA]
	call	C01E
	add	sp,0A
	les	bx,[26DA]
	mov	byte ptr es:[bx+03],00
	mov	ax,[bp-04]
	add	ax,0003
	push	word ptr [bp-02]
	push	ax
	call	A471
	pop	cx
	pop	cx
	push	ax
	push	dx
	xor	dx,dx
	mov	ax,0E10
	pop	cx
	pop	bx
	call	8F18
	mov	[26E4],dx
	mov	[26E2],ax
	mov	word ptr [26E6],0000
	mov	si,0003
	jmp	C542

l0800_C4CA:
	les	bx,[bp-04]
	mov	al,es:[bx+si]
	cbw
	mov	bx,ax
	test	byte ptr [bx+2251],0C
	jz	C541

l0800_C4DA:
	mov	ax,[bp-04]
	add	ax,si
	push	word ptr [bp-02]
	push	ax
	call	BFC7
	pop	cx
	pop	cx
	cmp	ax,0003
	jc	C54E

l0800_C4ED:
	les	bx,[bp-04]
	mov	al,es:[bx+si+01]
	cbw
	mov	bx,ax
	mov	al,[bx+2251]
	cbw
	test	ax,000C
	jz	C54E

l0800_C501:
	mov	bx,[bp-04]
	mov	al,es:[bx+si+02]
	cbw
	mov	bx,ax
	mov	al,[bx+2251]
	cbw
	test	ax,000C
	jz	C54E

l0800_C515:
	mov	ax,0003
	push	ax
	mov	ax,[bp-04]
	add	ax,si
	push	word ptr [bp-02]
	push	ax
	push	word ptr [26E0]
	push	word ptr [26DE]
	call	C01E
	add	sp,0A
	les	bx,[26DE]
	mov	byte ptr es:[bx+03],00
	mov	word ptr [26E6],0001
	jmp	C54E

l0800_C541:
	inc	si

l0800_C542:
	les	bx,[bp-04]
	cmp	byte ptr es:[bx+si],00
	jz	C54E

l0800_C54B:
	jmp	C4CA

l0800_C54E:
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_C553: 0800:C553
;;   Called from:
;;     0800:C120 (in fn0800_C04F)
;;     0800:C2A5 (in fn0800_C177)
fn0800_C553 proc
	push	bp
	mov	bp,sp
	push	si
	cmp	word ptr [bp+06],00
	jnz	C589

l0800_C55D:
	mov	si,[bp+08]
	cmp	word ptr [bp+08],3B
	jc	C572

l0800_C566:
	mov	ax,[bp+04]
	add	ax,0046
	test	ax,0003
	jnz	C572

l0800_C571:
	dec	si

l0800_C572:
	mov	word ptr [bp+06],0000
	jmp	C57C

l0800_C579:
	inc	word ptr [bp+06]

l0800_C57C:
	mov	bx,[bp+06]
	shl	bx,01
	cmp	[bx+26C0],si
	jbe	C579

l0800_C587:
	jmp	C5AA

l0800_C589:
	cmp	word ptr [bp+06],03
	jc	C59A

l0800_C58F:
	mov	ax,[bp+04]
	add	ax,0046
	test	ax,0003
	jz	C59D

l0800_C59A:
	dec	word ptr [bp+08]

l0800_C59D:
	mov	bx,[bp+06]
	dec	bx
	shl	bx,01
	mov	ax,[bx+26C0]
	add	[bp+08],ax

l0800_C5AA:
	cmp	word ptr [bp+06],04
	jc	C62B

l0800_C5B0:
	jz	C5BA

l0800_C5B2:
	cmp	word ptr [bp+06],0A
	ja	C62B

l0800_C5B8:
	jnz	C626

l0800_C5BA:
	mov	bx,[bp+06]
	shl	bx,01
	cmp	word ptr [bp+04],10
	jle	C5D4

l0800_C5C5:
	cmp	word ptr [bp+06],04
	jnz	C5D4

l0800_C5CB:
	mov	cx,[bx+26BE]
	add	cx,07
	jmp	C5D8

l0800_C5D4:
	mov	cx,[bx+26C0]

l0800_C5D8:
	mov	bx,[bp+04]
	add	bx,07B2
	test	bl,03
	jz	C5E5

l0800_C5E4:
	dec	cx

l0800_C5E5:
	mov	bx,[bp+04]
	inc	bx
	sar	bx,01
	sar	bx,01
	add	bx,cx
	mov	ax,016D
	mul	word ptr [bp+04]
	add	ax,bx
	add	ax,0004
	xor	dx,dx
	mov	bx,0007
	div	bx
	sub	cx,dx
	mov	ax,[bp+08]
	cmp	word ptr [bp+06],04
	jnz	C61A

l0800_C60C:
	cmp	ax,cx
	ja	C626

l0800_C610:
	jnz	C62B

l0800_C612:
	cmp	byte ptr [bp+0A],02
	jc	C62B

l0800_C618:
	jmp	C626

l0800_C61A:
	cmp	ax,cx
	jc	C626

l0800_C61E:
	jnz	C62B

l0800_C620:
	cmp	byte ptr [bp+0A],01
	ja	C62B

l0800_C626:
	mov	ax,0001
	jmp	C62D

l0800_C62B:
	xor	ax,ax

l0800_C62D:
	pop	si
	pop	bp
	ret	0008

;; fn0800_C632: 0800:C632
;;   Called from:
;;     0800:A75B (in fn0800_A6B7)
;;     0800:B690 (in fn0800_B4BE)
fn0800_C632 proc
	push	bp
	mov	bp,sp
	sub	sp,008E
	push	si
	push	di
	mov	di,[bp+04]
	cmp	di,[24E8]
	jc	C64E

l0800_C644:
	mov	ax,0006
	push	ax
	call	8D2B
	jmp	C773

l0800_C64E:
	mov	ax,[bp+0A]
	inc	ax
	cmp	ax,0002
	jnc	C65C

l0800_C657:
	xor	ax,ax
	jmp	C773

l0800_C65C:
	mov	bx,di
	shl	bx,01
	test	word ptr [bx+24EA],0800
	jz	C677

l0800_C668:
	mov	ax,0002
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	di
	call	8E29
	add	sp,08

l0800_C677:
	mov	bx,di
	shl	bx,01
	test	word ptr [bx+24EA],4000
	jnz	C696

l0800_C683:
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	di
	call	C779
	add	sp,08
	jmp	C773

l0800_C696:
	mov	bx,di
	shl	bx,01
	and	word ptr [bx+24EA],FDFF
	mov	ax,[bp+08]
	mov	dx,[bp+06]
	mov	[bp-0A],ax
	mov	[bp-0C],dx
	mov	ax,[bp+0A]
	mov	[bp-06],ax
	jmp	C727

l0800_C6B4:
	dec	word ptr [bp-06]
	les	bx,[bp-0C]
	inc	word ptr [bp-0C]
	mov	al,es:[bx]
	mov	[bp-07],al
	cmp	al,0A
	jnz	C6D1

l0800_C6C7:
	les	bx,[bp-04]
	mov	byte ptr es:[bx],0D
	inc	word ptr [bp-04]

l0800_C6D1:
	les	bx,[bp-04]
	mov	al,[bp-07]
	mov	es:[bx],al
	inc	word ptr [bp-04]
	lea	ax,[bp+FF72]
	mov	dx,[bp-04]
	xor	bx,bx
	sub	dx,ax
	sbb	bx,00
	or	bx,bx
	jl	C731

l0800_C6EF:
	jnz	C6F7

l0800_C6F1:
	cmp	dx,0080
	jc	C731

l0800_C6F7:
	lea	ax,[bp+FF72]
	mov	si,[bp-04]
	xor	dx,dx
	sub	si,ax
	sbb	dx,00
	push	si
	push	ss
	push	ax
	push	di
	call	C779
	add	sp,08
	mov	dx,ax
	cmp	ax,si
	jz	C727

l0800_C715:
	cmp	dx,FF
	jnz	C71F

l0800_C71A:
	mov	ax,FFFF
	jmp	C76E

l0800_C71F:
	mov	ax,[bp+0A]
	sub	ax,[bp-06]
	jmp	C76A

l0800_C727:
	lea	ax,[bp+FF72]
	mov	[bp-02],ss
	mov	[bp-04],ax

l0800_C731:
	cmp	word ptr [bp-06],00
	jz	C73A

l0800_C737:
	jmp	C6B4

l0800_C73A:
	lea	ax,[bp+FF72]
	mov	si,[bp-04]
	xor	dx,dx
	sub	si,ax
	sbb	dx,00
	mov	ax,si
	or	ax,ax
	jbe	C770

l0800_C74E:
	push	si
	push	ss
	lea	ax,[bp+FF72]
	push	ax
	push	di
	call	C779
	add	sp,08
	mov	dx,ax
	cmp	ax,si
	jz	C770

l0800_C762:
	cmp	dx,FF
	jz	C71A

l0800_C767:
	mov	ax,[bp+0A]

l0800_C76A:
	add	ax,dx
	sub	ax,si

l0800_C76E:
	jmp	C773

l0800_C770:
	mov	ax,[bp+0A]

l0800_C773:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_C779: 0800:C779
;;   Called from:
;;     0800:B46D (in fn0800_B324)
;;     0800:B48A (in fn0800_B324)
;;     0800:B570 (in fn0800_B4BE)
;;     0800:B616 (in fn0800_B4BE)
;;     0800:C68D (in fn0800_C632)
;;     0800:C709 (in fn0800_C632)
;;     0800:C756 (in fn0800_C632)
fn0800_C779 proc
	push	bp
	mov	bp,sp
	mov	bx,[bp+04]
	shl	bx,01
	test	word ptr [bx+24EA],0001
	jz	C78F

l0800_C789:
	mov	ax,0005
	push	ax
	jmp	C7B0

l0800_C78F:
	push	ds
	mov	ah,40
	mov	bx,[bp+04]
	mov	cx,[bp+0A]
	lds	dx,[bp+06]
	int	21
	pop	ds
	jc	C7AF

l0800_C7A0:
	push	ax
	mov	bx,[bp+04]
	shl	bx,01
	or	word ptr [bx+24EA],1000
	pop	ax
	jmp	C7B3

l0800_C7AF:
	push	ax

l0800_C7B0:
	call	8D2B

l0800_C7B3:
	pop	bp
	ret
0800:C7B5                55 8B EC 83 EC 04 56 33 F6 8C 5E      U.....V3..^
0800:C7C0 FE C7 46 FC 58 23 3B 36 E8 24 73 1F C4 5E FC 26 ..F.X#;6.$s..^.&
0800:C7D0 F7 47 02 03 00 74 09 FF 76 FE 53 E8 36 DE 59 59 .G...t..v.S.6.YY
0800:C7E0 83 46 FC 14 46 3B 36 E8 24 72 E1 5E 8B E5 5D C3 .F..F;6.$r.^..].
0800:C7F0 55 8B EC 83 EC 04 56 BE 04 00 8C 5E FE C7 46 FC U.....V....^..F.
0800:C800 58 23 EB 19 C4 5E FC 26 F7 47 02 03 00 74 09 FF X#...^.&.G...t..
0800:C810 76 FE 53 E8 A1 DE 59 59 4E 83 46 FC 14 0B F6 75 v.S...YYN.F....u
0800:C820 E3 5E 8B E5 5D C3 00 00 00 00 00 00 00 00 00 00 .^..]...........
;;; Segment 1483 (1483:0000)
1483:0000 00 00 00 00 42 6F 72 6C 61 6E 64 20 43 2B 2B 20 ....Borland C++ 
1483:0010 2D 20 43 6F 70 79 72 69 67 68 74 20 31 39 39 31 - Copyright 1991
1483:0020 20 42 6F 72 6C 61 6E 64 20 49 6E 74 6C 2E 00 44  Borland Intl..D
1483:0030 69 76 69 64 65 20 65 72 72 6F 72 0D 0A 41 62 6E ivide error..Abn
1483:0040 6F 72 6D 61 6C 20 70 72 6F 67 72 61 6D 20 74 65 ormal program te
1483:0050 72 6D 69 6E 61 74 69 6F 6E 0D 0A 00 00 00 00 00 rmination.......
1483:0060 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
1483:0080 00 00 00 00 00 F4 4E 00 00 00 00 00 00 00 00 00 ......N.........
1483:0090 00 00 00 00 50 52 4F 50 41 43 4B 20 28 74 6D 29 ....PROPACK (tm)
1483:00A0 20 32 2E 31 34 20 52 45 47 49 53 54 45 52 45 44  2.14 REGISTERED
1483:00B0 20 54 4F 20 4F 58 46 4F 52 44 20 44 49 47 49 54  TO OXFORD DIGIT
1483:00C0 41 4C 20 45 4E 54 45 52 50 52 49 53 45 53 20 20 AL ENTERPRISES  
1483:00D0 32 39 2D 31 30 2D 39 32 0A 43 6F 70 79 72 69 67 29-10-92.Copyrig
1483:00E0 68 74 20 28 63 29 20 31 39 39 31 2D 39 32 20 52 ht (c) 1991-92 R
1483:00F0 6F 62 20 4E 6F 72 74 68 65 6E 20 43 6F 6D 70 75 ob Northen Compu
1483:0100 74 69 6E 67 2C 20 55 4B 2E 20 41 6C 6C 20 52 69 ting, UK. All Ri
1483:0110 67 68 74 73 20 52 65 73 65 72 76 65 64 2E 0A 0A ghts Reserved...
1483:0120 00 55 53 41 47 45 3A 20 50 50 20 3C 43 4F 4D 4D .USAGE: PP <COMM
1483:0130 41 4E 44 3E 20 5B 2D 4F 50 54 49 4F 4E 28 53 29 AND> [-OPTION(S)
1483:0140 5D 20 5B 3C 46 49 4C 45 28 53 29 3E 20 7C 20 40 ] [<FILE(S)> | @
1483:0150 4C 49 53 54 46 49 4C 45 3E 5D 0A 0A 3C 4E 4F 4E LISTFILE>]..<NON
1483:0160 2D 41 52 43 48 49 56 45 20 43 4F 4D 4D 41 4E 44 -ARCHIVE COMMAND
1483:0170 53 3E 20 20 70 3A 20 50 61 63 6B 20 20 20 75 3A S>  p: Pack   u:
1483:0180 20 55 6E 70 61 63 6B 20 20 20 76 3A 20 56 65 72  Unpack   v: Ver
1483:0190 69 66 79 0A 0A 3C 41 52 43 48 49 56 45 20 43 4F ify..<ARCHIVE CO
1483:01A0 4D 4D 41 4E 44 53 3E 0A 20 61 20 3C 61 72 63 66 MMANDS>. a <arcf
1483:01B0 69 6C 65 3E 3A 20 41 64 64 20 74 6F 20 61 72 63 ile>: Add to arc
1483:01C0 68 69 76 65 20 20 20 20 20 20 20 20 65 20 3C 61 hive        e <a
1483:01D0 72 63 66 69 6C 65 3E 3A 20 45 78 74 72 61 63 74 rcfile>: Extract
1483:01E0 20 66 72 6F 6D 20 61 72 63 68 69 76 65 0A 20 63  from archive. c
1483:01F0 20 3C 61 72 63 66 69 6C 65 3E 3A 20 43 72 65 61  <arcfile>: Crea
1483:0200 74 65 20 61 72 63 68 69 76 65 20 20 20 20 20 20 te archive      
1483:0210 20 20 6C 20 3C 61 72 63 66 69 6C 65 3E 3A 20 4C   l <arcfile>: L
1483:0220 69 73 74 20 26 20 76 65 72 69 66 79 20 61 72 63 ist & verify arc
1483:0230 68 69 76 65 0A 20 64 20 3C 61 72 63 66 69 6C 65 hive. d <arcfile
1483:0240 3E 3A 20 44 65 6C 65 74 65 20 66 72 6F 6D 20 61 >: Delete from a
1483:0250 72 63 68 69 76 65 20 20 20 78 20 3C 61 72 63 66 rchive   x <arcf
1483:0260 69 6C 65 3E 3A 20 65 58 74 72 61 63 74 20 77 69 ile>: eXtract wi
1483:0270 74 68 20 66 75 6C 6C 20 70 61 74 68 6E 61 6D 65 th full pathname
1483:0280 0A 0A 3C 4F 50 54 49 4F 4E 53 3E 0A 2D 66 3A 20 ..<OPTIONS>.-f: 
1483:0290 28 70 75 76 29 20 6E 6F 6E 2D 61 72 63 68 69 76 (puv) non-archiv
1483:02A0 65 20 46 69 6C 65 20 74 79 70 65 20 20 20 20 2D e File type    -
1483:02B0 74 3A 20 28 70 75 65 29 20 54 61 72 67 65 74 2F t: (pue) Target/
1483:02C0 62 61 73 65 20 64 69 72 65 63 74 6F 72 79 0A 20 base directory. 
1483:02D0 20 20 20 20 66 3C 61 7C 64 7C 6C 7C 6D 7C 70 7C     f<a|d|l|m|p|
1483:02E0 73 3E 20 20 20 20 20 20 20 20 20 20 20 20 20 20 s>              
1483:02F0 20 20 20 20 20 20 20 74 3C 64 69 72 3E 0A 2D 6B        t<dir>.-k
1483:0300 3A 20 28 70 75 76 61 63 65 29 20 4B 65 79 20 70 : (puvace) Key p
1483:0310 72 6F 74 65 63 74 20 66 69 6C 65 20 20 20 20 20 rotect file     
1483:0320 20 2D 75 3A 20 28 70 29 20 55 70 64 61 74 65 20  -u: (p) Update 
1483:0330 66 69 6C 65 73 20 69 6E 20 74 61 72 67 65 74 20 files in target 
1483:0340 64 69 72 65 63 74 6F 72 79 0A 20 20 20 20 20 6B directory.     k
1483:0350 3C 6E 3E 20 28 6E 3D 30 2D 30 78 46 46 46 46 29 <n> (n=0-0xFFFF)
1483:0360 20 20 20 20 20 20 20 20 20 20 20 20 20 2D 76 20              -v 
1483:0370 20 28 70 29 20 56 65 72 69 66 79 20 43 52 43 27  (p) Verify CRC'
1483:0380 73 20 64 75 72 69 6E 67 20 73 65 6C 66 2D 65 78 s during self-ex
1483:0390 74 72 61 63 74 0A 2D 6C 3A 20 28 70 61 63 29 20 tract.-l: (pac) 
1483:03A0 6C 6F 63 6B 20 66 69 6C 65 20 20 20 20 20 20 20 lock file       
1483:03B0 20 20 20 20 20 20 20 20 20 2D 77 3A 20 28 61 6C          -w: (al
1483:03C0 6C 29 20 57 6F 72 6B 20 64 69 72 65 63 74 6F 72 l) Work director
1483:03D0 79 0A 2D 6D 3A 20 28 70 61 63 29 20 63 6F 6D 70 y.-m: (pac) comp
1483:03E0 72 65 73 73 69 6F 6E 20 4D 65 74 68 6F 64 20 20 ression Method  
1483:03F0 20 20 20 20 20 20 20 20 20 20 77 3C 64 69 72 3E           w<dir>
1483:0400 0A 20 20 20 20 20 6D 3C 30 7C 31 7C 32 3E 20 20 .     m<0|1|2>  
1483:0410 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20                 
1483:0420 20 20 20 20 2D 78 3A 20 28 70 29 20 36 38 30 30     -x: (p) 6800
1483:0430 30 20 65 58 65 63 20 61 64 64 72 65 73 73 20 6F 0 eXec address o
1483:0440 66 66 73 65 74 0A 2D 72 3A 20 28 61 6C 6C 29 20 ffset.-r: (all) 
1483:0450 52 65 63 75 72 73 65 20 73 75 62 64 69 72 65 63 Recurse subdirec
1483:0460 74 6F 72 69 65 73 20 20 20 20 20 20 20 20 78 3C tories        x<
1483:0470 6E 3E 20 28 6E 3D 30 2D 30 78 46 46 46 46 46 46 n> (n=0-0xFFFFFF
1483:0480 46 46 29 0A 0A 20 20 20 20 20 50 52 4F 50 41 43 FF)..     PROPAC
1483:0490 4B 20 69 73 20 6F 6E 6C 79 20 63 68 61 72 67 65 K is only charge
1483:04A0 61 62 6C 65 20 77 68 65 6E 20 75 73 65 64 20 69 able when used i
1483:04B0 6E 20 61 20 63 6F 6D 6D 65 72 63 69 61 6C 20 70 n a commercial p
1483:04C0 72 6F 64 75 63 74 2E 0A 20 20 20 20 20 20 46 6F roduct..      Fo
1483:04D0 72 20 64 65 74 61 69 6C 73 2C 20 54 65 6C 3A 20 r details, Tel: 
1483:04E0 2B 34 34 20 28 30 29 34 32 38 2D 37 30 37 37 37 +44 (0)428-70777
1483:04F0 31 20 46 61 78 3A 20 2B 34 34 20 28 30 29 34 32 1 Fax: +44 (0)42
1483:0500 38 2D 37 30 37 37 37 32 00 D5 05 83 14 DD 05 83 8-707772........
1483:0510 14 E7 05 83 14 F1 05 83 14 FB 05 83 14 04 06 83 ................
1483:0520 14 12 06 83 14 22 06 83 14 12 06 83 14 2A 06 83 .....".......*..
1483:0530 14 30 06 83 14 35 06 83 14 3A 06 83 14 40 06 83 .0...5...:...@..
1483:0540 14 43 06 83 14 46 06 83 14 DC 05 83 14 DC 05 83 .C...F..........
1483:0550 14 49 06 83 14 4E 06 83 14 43 06 83 14 52 06 83 .I...N...C...R..
1483:0560 14 55 06 83 14 61 06 83 14 6C 06 83 14 7A 06 83 .U...a...l...z..
1483:0570 14 82 06 83 14 8D 06 83 14 A1 06 83 14 AC 06 83 ................
1483:0580 14 B7 06 83 14 C3 06 83 14 D0 06 83 14 E3 06 83 ................
1483:0590 14 F3 06 83 14 07 07 83 14 52 06 83 14 16 07 83 .........R......
1483:05A0 14 27 07 83 14 2F 07 83 14 3A 07 83 14 49 07 83 .'.../...:...I..
1483:05B0 14 5F 07 83 14 77 07 83 14 8E 07 83 14 A6 07 83 ._...w..........
1483:05C0 14 B6 07 83 14 BD 07 83 14 CA 07 83 14 D4 07 83 ................
1483:05D0 14 DC 07 83 14 50 41 43 4B 49 4E 47 00 55 4E 50 .....PACKING.UNP
1483:05E0 41 43 4B 49 4E 47 00 56 45 52 49 46 59 49 4E 47 ACKING.VERIFYING
1483:05F0 00 41 44 44 49 4E 47 20 54 4F 00 43 52 45 41 54 .ADDING TO.CREAT
1483:0600 49 4E 47 00 44 45 4C 45 54 49 4E 47 20 46 52 4F ING.DELETING FRO
1483:0610 4D 00 45 58 54 52 41 43 54 49 4E 47 20 46 52 4F M.EXTRACTING FRO
1483:0620 4D 00 4C 49 53 54 49 4E 47 00 41 4D 49 47 41 00 M.LISTING.AMIGA.
1483:0630 44 41 54 41 00 4C 59 4E 58 00 36 38 30 30 30 00 DATA.LYNX.68000.
1483:0640 50 43 00 53 54 00 41 4D 00 4D 43 36 38 00 45 58 PC.ST.AM.MC68.EX
1483:0650 45 00 4F 4B 00 42 61 64 20 43 6F 6D 6D 61 6E 64 E.OK.Bad Command
1483:0660 00 42 61 64 20 4F 70 74 69 6F 6E 00 42 61 64 20 .Bad Option.Bad 
1483:0670 46 69 6C 65 20 54 79 70 65 00 42 61 64 20 4B 65 File Type.Bad Ke
1483:0680 79 00 42 61 64 20 4D 65 74 68 6F 64 00 49 6E 73 y.Bad Method.Ins
1483:0690 75 66 66 69 63 69 65 6E 74 20 4D 65 6D 6F 72 79 ufficient Memory
1483:06A0 00 43 61 6E 27 74 20 4F 70 65 6E 00 43 61 6E 27 .Can't Open.Can'
1483:06B0 74 20 52 65 61 64 00 43 61 6E 27 74 20 57 72 69 t Read.Can't Wri
1483:06C0 74 65 00 43 61 6E 27 74 20 52 65 6E 61 6D 65 00 te.Can't Rename.
1483:06D0 4E 6F 74 20 61 6E 20 52 4E 43 20 41 72 63 68 69 Not an RNC Archi
1483:06E0 76 65 00 43 6F 72 72 75 70 74 20 41 72 63 68 69 ve.Corrupt Archi
1483:06F0 76 65 00 44 69 72 65 63 74 6F 72 79 20 43 52 43 ve.Directory CRC
1483:0700 20 45 72 72 6F 72 00 43 61 6E 27 74 20 4D 61 6B  Error.Can't Mak
1483:0710 65 20 44 69 72 00 57 41 52 4E 49 4E 47 3A 20 4F e Dir.WARNING: O
1483:0720 56 45 52 4C 41 59 00 53 54 4F 52 49 4E 47 00 43 VERLAY.STORING.C
1483:0730 41 4E 27 54 20 50 41 43 4B 00 41 4C 52 45 41 44 AN'T PACK.ALREAD
1483:0740 59 20 50 41 43 4B 45 44 00 50 41 43 4B 45 44 20 Y PACKED.PACKED 
1483:0750 44 41 54 41 20 43 52 43 20 45 52 52 4F 52 00 55 DATA CRC ERROR.U
1483:0760 4E 50 41 43 4B 45 44 20 44 41 54 41 20 43 52 43 NPACKED DATA CRC
1483:0770 20 45 52 52 4F 52 00 4E 4F 54 20 41 4E 20 52 4E  ERROR.NOT AN RN
1483:0780 43 20 50 41 43 4B 45 44 20 46 49 4C 45 00 43 41 C PACKED FILE.CA
1483:0790 4E 27 54 20 50 41 43 4B 20 4F 56 45 52 4C 41 59 N'T PACK OVERLAY
1483:07A0 20 48 55 4E 4B 00 55 4E 45 58 50 45 43 54 45 44  HUNK.UNEXPECTED
1483:07B0 20 48 55 4E 4B 00 4C 4F 43 4B 45 44 00 4B 45 59  HUNK.LOCKED.KEY
1483:07C0 20 52 45 51 55 49 52 45 44 00 57 52 4F 4E 47 20  REQUIRED.WRONG 
1483:07D0 4B 45 59 00 44 45 4C 45 54 45 44 00 4E 4F 54 20 KEY.DELETED.NOT 
1483:07E0 55 50 44 41 54 45 44 00 25 73 00 0A 25 34 75 20 UPDATED.%s..%4u 
1483:07F0 66 69 6C 65 28 73 29 25 2D 2A 73 20 5B 25 38 6C file(s)%-*s [%8l
1483:0800 75 5D 20 5B 25 38 6C 75 5D 20 5B 25 32 75 2E 25 u] [%8lu] [%2u.%
1483:0810 30 32 75 25 25 5D 20 5B 25 6C 75 68 20 25 6C 75 02u%%] [%luh %lu
1483:0820 6D 20 25 6C 75 73 5D 0A 00 50 55 56 41 43 44 45 m %lus]..PUVACDE
1483:0830 4C 58 00 2E 52 4E 43 00 46 4B 4D 54 57 45 4C 52 LX..RNC.FKMTWELR
1483:0840 56 55 53 00 41 44 4C 4D 50 53 00 25 73 5F 00 25 VUS.ADLMPS.%s_.%
1483:0850 68 69 00 25 49 00 25 69 00 5C 00 31 00 32 00 56 hi.%I.%i.\.1.2.V
1483:0860 00 2E 42 49 4E 00 0D 25 2D 2A 2E 2A 73 20 5B 25 ..BIN..%-*.*s [%
1483:0870 38 6C 75 5D 20 5B 25 38 6C 75 5D 20 5B 25 32 75 8lu] [%8lu] [%2u
1483:0880 2E 25 30 32 75 25 25 5D 00 20 5B 25 73 5D 00 44 .%02u%%]. [%s].D
1483:0890 69 72 65 63 74 6F 72 79 3A 20 25 73 0A 00 25 73 irectory: %s..%s
1483:08A0 20 00 0A 25 73 00 3A 20 25 73 00 00 25 73 20 25  ..%s.: %s..%s %
1483:08B0 73 20 46 49 4C 45 28 53 29 00 20 55 53 49 4E 47 s FILE(S). USING
1483:08C0 20 54 48 45 20 4B 45 59 3A 20 30 78 25 78 00 20  THE KEY: 0x%x. 
1483:08D0 41 4E 44 20 4C 4F 43 4B 49 4E 47 00 0A 0A 00 52 AND LOCKING....R
1483:08E0 4E 43 54 45 4D 50 31 2E 24 24 24 00 77 2B 62 00 NCTEMP1.$$$.w+b.
1483:08F0 25 73 20 25 73 20 46 49 4C 45 28 53 29 00 20 55 %s %s FILE(S). U
1483:0900 53 49 4E 47 20 54 48 45 20 4B 45 59 3A 20 30 78 SING THE KEY: 0x
1483:0910 25 78 00 0A 0A 00 25 73 20 41 52 43 48 49 56 45 %x....%s ARCHIVE
1483:0920 3A 20 25 73 00 20 55 53 49 4E 47 20 54 48 45 20 : %s. USING THE 
1483:0930 4B 45 59 3A 20 30 78 25 78 00 0A 0A 00 72 2B 62 KEY: 0x%x....r+b
1483:0940 00 77 2B 62 00 52 4E 43 54 45 4D 50 31 2E 24 24 .w+b.RNCTEMP1.$$
1483:0950 24 00 41 72 63 68 69 76 65 20 44 69 72 65 63 74 $.Archive Direct
1483:0960 6F 72 79 20 46 75 6C 6C 21 0A 00 00 25 73 20 41 ory Full!...%s A
1483:0970 52 43 48 49 56 45 3A 20 25 73 0A 0A 00 72 2B 62 RCHIVE: %s...r+b
1483:0980 00 00 2A 2E 2A 00 25 73 20 41 52 43 48 49 56 45 ..*.*.%s ARCHIVE
1483:0990 3A 20 25 73 0A 0A 00 72 2B 62 00 52 4E 43 54 45 : %s...r+b.RNCTE
1483:09A0 4D 50 32 2E 24 24 24 00 77 2B 62 00 00 00 00 00 MP2.$$$.w+b.....
1483:09B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
1483:0A10 00 00 00 72 2B 62 00 43 61 6E 27 74 20 46 69 6E ...r+b.Can't Fin
1483:0A20 64 3A 20 25 73 0A 00 25 73 25 73 25 73 25 73 00 d: %s..%s%s%s%s.
1483:0A30 5C 00 2A 2E 2A 00 5C 2A 2E 2A 00 2E 00 2E 2E 00 \.*.*.\*.*......
1483:0A40 52 4E 43 54 45 4D 50 32 2E 24 24 24 00 72 62 00 RNCTEMP2.$$$.rb.
1483:0A50 77 2B 62 00 52 4E 43 54 45 4D 50 33 2E 24 24 24 w+b.RNCTEMP3.$$$
1483:0A60 00 77 2B 62 00 72 2B 62 00 72 62 00 00 00 52 4E .w+b.r+b.rb...RN
1483:0A70 43 02 00 00 64 8A 00 00 17 54 12 1D A7 B8 02 03 C...d....T......
1483:0A80 17 43 3C 4F 4D 5F 31 2E 42 49 4E 00 01 F6 83 EC .C<OM_1.BIN.....
1483:0A90 10 8B EC BE FA 02 FC E8 41 00 05 0F 8B 00 C8 E8 ........A.......
1483:0AA0 39 00 8B D0 03 C6 35 05 06 06 C3 F8 E8 2D 00 AD 9.....5......-..
1483:0AB0 88 66 0A 32 E4 89 76 0C 0D F7 03 C1 11 3B 2E FE .f.2..v......;..
1483:0AC0 76 78 0E FD 57 4E 4F 8B CA F3 A4 47 89 7E 00 5E vx..WNO....G.~.^
1483:0AD0 FC 8B FE 56 BE 56 01 B9 A0 01 10                ...V.V.....    

;; fn1483_0ADB: 1483:0ADB
fn1483_0ADB proc
	ret

;; fn1483_0ADC: 1483:0ADC
fn1483_0ADC proc
	call	0D3F
	fsubr	dword ptr es:[di+E086]
	pusha
	ret
1483:0AE6                   19 02 81 C7 80 64 00 06 04 87       .....d....
1483:0AF0 06 06 8B 01 40 04 89 46 0C BF 80 62 C6 46 0B 00 ....@..F...b.F..
1483:0B00 B0 02 33 E8 AA 10 56 38 05 FA 05 04 38 E8 F4 05 ..3...V8....8...
1483:0B10 06 06 E8 EE 00 E8 93 2B 08 46 08 EB 1C 08 13 57 .......+.F.....W
1483:0B20 00 51 00 14 50 00 83 C1 02 58 40 31 8B D6 7F 2B .Q..P....X@1...+
1483:0B30 F0 88 62 8B F2 06 35 3B 00 E3 25 0B 1B 8A 4E 0B ..b...5;..%...N.
1483:0B40 53 2E 6F D3 7A C0 BA 01 00 D3 E2 4A 21 56 0C 23 S.o.z......J!V.#
1483:0B50 D0 8B 44 02 D3 E3 D3 E0 0B C2 09 5E 0C 4A E1 0E ..D........^.J..
1483:0B60 FF 4E 08 75 B2 FE 4E 0A 75 93 BE 80 78 83 C4 10 .N.u..N.u...x...
1483:0B70 33 C0 FF 30 E6 87 4A 4E 0C AD C0 37 23 D9 AD 3B 3..0..JN...7#..;
1483:0B80 C3 06 75 F6 8B 4C 3C 11 B9 72 8A C5 E8 1A 00 32 ..u..L<..r.....2
1483:0B90 ED 80 F9 02 72 10 FE C9 8A C1 E8 0C 00 BB 54 E1 ....r.........T.
1483:0BA0 E3 0B C3 8B C8 C3 B0 10 51 8A C8 8B 98 4C 8B 52 ........Q....L.R
1483:0BB0 8A 6E 48 0B 6C 23 00 D3 52 2A E9 73 21 02 E9 26 .nH.l#..R*.s!..&
1483:0BC0 86 CD 10 B8 6C D0 D3 CA D3 E8 D3 EB 0B DA 83 C6 ....l...........
1483:0BD0 02 96 16 2A 1B CD B5 10 22 DC 08 1C 93 66 89 46 ...*...."....f.F
1483:0BE0 88 46 07 58 59 C3 57 52 60 7C B0 05 E8 A0 60 FF .F.XY.WR`|....`.
1483:0BF0 64 E3 67 8B FC 35 51 B0 F5 C3 94 FF 36 88 05 47 d.g..5Q.....6..G
1483:0C00 E2 F5 59 56 8B F4 83 48 36 8B 7C 0B 21 01 33 DB ..YV...H6.|.!.3.
1483:0C10 87                                              .              

;; fn1483_0C11: 1483:0C11
;;   Called from:
;;     1483:0C55 (in fn1483_0C55)
fn1483_0C11 proc
	mov	dx,8000
	push	cx
	push	si
	cmp	al,ss:[si]
	jnz	0C4D

l1483_0C1B:
	push	ax
	push	bx
	sbb	al,8C
	mov	ax,1955
	loopne	0C6C

l1483_0C24:
	stosw
	cmpsw
	mov	cl,9B
	illegal
	push	si
	imul	dx,[bx+si],D7
	xchg	cl,dl
	fcmovnbe	st(0),st(1)
	shl	dl,01
	cli
	stosw
	mov	ax,si
	sub	ax,sp
	sub	ax,2EBC
	mov	bh,ss:[di+24]
	mov	es:[di+3C],ax
	pop	cx
	pop	bx
	pop	ax
	add	bx,dx
	inc	si
	loop	0C13

l1483_0C4D:
	pop	si
	pop	cx
	shr	dx,01
	inc	al
	cmp	al,11

;; fn1483_0C55: 1483:0C55
;;   Called from:
;;     1483:0C53 (in fn1483_0C11)
;;     1483:0C96 (in fn1483_0C91)
fn1483_0C55 proc
	jnz	0C11

l1483_0C57:
	pop	si
	test	[si],ax
	pop	dx
	pop	di
	ret
1483:0C5D                                        41 01 4B              A.K
1483:0C60 56 50 03 02 67 6A 86 16 40 03 6E 03             VP..gj..@.n.   

l1483_0C6C:
	cld
	daa
	call	1BB8
	add	di,[bx]
	add	ax,[bx+3303]
	xchg	eax,ebx
	adc	dh,[si+147C]
	sbb	ax,0903
	pop	sp
	add	[bx+di+020E],di
	loopne	0CA0

l1483_0C87:
	or	dx,cx
	mov	ax,[bx]
	push	si
	adc	ch,al
	movsb
	add	ax,ax

;; fn1483_0C91: 1483:0C91
;;   Called from:
;;     1483:0C8F (in fn1483_0C11)
;;     1483:0D04 (in fn1483_0CFC)
;;     1483:0D04 (in fn1483_0CFC)
fn1483_0C91 proc
	pop	es
	adc	dx,ax
	add	al,ch
	jno	0C55

l1483_0C98:
	imul	dx,[bx+si],18
	loope	0CA9

l1483_0C9D:
	add	[bx+si+2203],cx

;; fn1483_0CA0: 1483:0CA0
;;   Called from:
;;     1483:0C85 (in fn1483_0C11)
;;     1483:0C9B (in fn1483_0C91)
;;     1483:0C9D (in fn1483_0C91)
fn1483_0CA0 proc
	and	al,[3101]
	add	ah,[bx+si]
	call	FD4E

l1483_0CA9:
	add	dl,[bx]
	imul	dx,[si+173E],0C62
	add	[bx],dl
	dec	bp
	add	bl,ah
	sub	ax,32AC
	rol	byte ptr [bx+di-56],E2
	stc
	ror	word ptr [bp+7C],01
	adc	[bx],bl
	and	bl,[bx]
	stosb
	sbb	ax,8B1F
	add	[bx],bx
	mov	cx,di
	sub	cx,si
	xchg	[di+14],ax
	call	4FE0
	sub	[F7E3],dx
	sub	ax,sp
	add	[bx+di],bp
	push	si
	push	cx
	sbb	cl,[bp+di+027E]
	push	si
	in	ax,8B

l1483_0CE5:
	mov	ax,bx
1483:0CE7                      B9 08 00 D1 E8 73                 .....s  

;; fn1483_0CED: 1483:0CED
;;   Called from:
;;     1483:0D50 (in fn1483_0D3F)
fn1483_0CED proc
	add	si,[di]
	add	[bx+si+F7E2],sp
	stosw
	inc	bl
	jnz	0CE5

l1483_0CF8:
	pop	cx
	rcr	byte ptr [bx+si],AC

;; fn1483_0CFA: 1483:0CFA
;;   Called from:
;;     1483:0D40 (in fn1483_0D3F)
fn1483_0CFA proc
	sbb	[si+D832],ch

;; fn1483_0CFC: 1483:0CFC
;;   Called from:
;;     1483:0CF9 (in fn1483_0CED)
;;     1483:0CFA (in fn1483_0CFA)
fn1483_0CFC proc
	xor	bl,al
	mov	al,[bx+di]
	mov	word ptr [bp+si],D1FF
	jcxz	0C91

;; fn1483_0D06: 1483:0D06
;;   Called from:
;;     1483:0D02 (in fn1483_0CFC)
;;     1483:0D04 (in fn1483_0CFC)
fn1483_0D06 proc
	sbb	[bx+E209],dx
	sub	ax,5EF1
	cmp	bx,dx
	jz	0D2B

l1483_0D11:
	call	0D14
	pop	dx
	add	dx,0D
	mov	ah,09
	int	21
	mov	ax,4CFF
	add	al,70
	inc	dx
	popa
	and	fs:[bp+di+52],al
	inc	bx
	or	ax,240A

l1483_0D2B:
	ret
1483:0D2C                                     90 94 4A 77             ..Jw
1483:0D30 19 76 04 D3 12 D0 76 08 7D 03 38 5F 7A AE 28    .v....v.}.8_z.(

;; fn1483_0D3F: 1483:0D3F
;;   Called from:
;;     1483:0ADC (in fn1483_0ADC)
fn1483_0D3F proc
	push	ax
	jpe	0CFA

l1483_0D42:
	add	al,ch
	mov	bl,B4
	sub	ch,[bx+si+02]
	adc	ax,FC68
	push	si
	xor	dh,17
	jl	0CED

l1483_0D52:
	add	ah,[di]
	push	0F5F
	push	0C58
	xchg	[bx+si+43],bp
	shl	byte ptr [di],68
	add	sp,12
	clc
	neg	word ptr [bp-06]
	push	es
	into

l1483_0D69:
	mov	[si+5C87],ch
	test	[si],dx
	call	8A82
	pusha
	adc	bx,[bx+di-79]
	adc	[bp+di+F512],dh
	and	[bx+007D],al
	sbb	ax,8747
	adc	al,ch
	xchg	ax,si
	aad	08
	xchg	[si+995D],si
	push	sp
	add	al,2A
	sbb	dh,bh
	add	bx,[bx+si]
	clc
	add	al,ch
	popf
	loope	0D99

l1483_0D97:
	sbb	[bx+di+28],ah

l1483_0D99:
	sub	[bx+si],bl

l1483_0D9A:
	sbb	[si+5A],bh

l1483_0D9B:
	jl	0DF7

l1483_0D9D:
	or	al,3F
	sbb	[di+2D],al
	mov	dl,99
	stosb
	jns	0DB9

l1483_0DA7:
	stosb

l1483_0DA8:
	jns	0DBE

l1483_0DAA:
	clc
	neg	word ptr [bp+si]
	cli
	dec	di
	pop	word ptr [bx+di+45]
	pop	ax
	inc	bp
	add	[bx+si+C079],bp
	dec	bp

l1483_0DB9:
	pop	dx
	shr	byte ptr [bx+di],01
	add	[bx+di+02],cl

l1483_0DBE:
	add	al,[0047]

l1483_0DBF:
	push	es
	inc	di
	add	[03BC],cl
	or	ax,[0E00]
	add	[si+8ED3],cl
	ret
	mov	dx,cs
	mov	ds,dx
1483:0DD2       30 8B 0E 89 8B F1 83 B9 60 EE 02 8B FE D1   0.......`.....
1483:0DE0 E9 FD F3 A5 53 B8 35 00 50 8B 2E 0A 00 03 16 CE ....S.5.P.......
1483:0DF0 CB B8 00                                        ...            

l1483_0DF3:
	adc	[bx+si],bl
	cmp	ax,bp

l1483_0DF7:
	jbe	0E14

l1483_0DF9:
	lds	bp,[bp+di]
	push	es
	call	E62A
	sub	dx,ax
	lds	si,cs:[si]
	mov	cl,03
	ret
1483:0E07                      D3 E0 8B C8 D1 E0 48 48 8B        ......HH.
1483:0E10 F0 8B F8 0B                                     ....           

l1483_0E14:
	xor	[bp+di],cl
	in	ax,dx
	jnz	0DA8

l1483_0E19:
	frndint
	pop	es
	mov	ds,bx
	sub	sp,1C
	mov	bp,sp
	sub	sp,0180
	mov	ax,sp
	inc	bx
	push	bp
	add	ax,8FA0
	inc	si
	or	ah,[bp+si+0C05]
	loope	0DF3

l1483_0E35:
	adc	[bx+si],ax
	lodsb
	mov	[bp+14],al
	xor	di,di
	mov	[bp+15],di
	adc	[bp+0E16],dl
	push	es
	mov	[bp+04],ss
	adc	byte ptr [bp+di+0137],E8
	xor	ah,[bx+si+01]
	push	cs
	sbb	cl,[si+005E]
	xor	[si+6346],cl
	push	si
	or	al,ch
	jz	0ED9

l1483_0E5E:
	push	cs
	lahf
	or	ch,al
	jnz	0E70

l1483_0E64:
	add	ax,E80C
	outsw
	sbb	ax,E014
	sbb	ax,EB12
	and	al,[bp+si+D213]

l1483_0E70:
	adc	dx,dx

l1483_0E72:
	and	bx,[bx+143E]
	retf
1483:0E77                      04 0E 9F 8E 5E 9C 0D 04 00        ....^....
1483:0E80 88 A5 C9 3B B0 55 BE 1A 55 BE 0F 1A 8A 4E 15 06 ...;.U..U....N..
1483:0E90 87 AD 16 C3 05 AD 16 2E 58 18 BC FF 4E 12 75 A4 ........X...N.u.
1483:0EA0 8B DF 83 E7 0F 81 C7 00 80 B1 04 D3 EB 8C C0 03 ................
1483:0EB0 C3 2D 00 08 8E C0 8B DE 83 E6 0F 0F 62 D8 0F 8E .-..........b...
1483:0EC0 D8 E9 FE 4E 14 74 03 E9 59 FF 1F 1E 07 0E 1F 8C ...N.t..Y.......
1483:0ED0 C2 32 ED BE A0 02                               .2....         

l1483_0ED6:
	lodsb
	mov	cl,al

l1483_0ED9:
	jcxz	0EEE

l1483_0EDB:
	lodsw
	add	ax,dx
	mov	ch,2A
	aad	C3
	xor	ah,ah

l1483_0EE4:
	lodsb
	add	di,ax
	add	es:[di],dx
	loop	0EE4

l1483_0EEC:
	jmp	0ED6

l1483_0EEE:
	or	al,20
	mov	si,[9004]
	add	ds:[0300],ax
	cli
	add	[8602],dx
	lodsw
	jmp	far 3024:8E10
1483:0F04             DA 33 26 DB FA 8B 01 E6 8E D7 FB 2E     .3&.........
1483:0F10 FF 2F F3 01 BE 12 10 16 CF BF 04 05 9E 13 7C CF ./............|.
1483:0F20 1E 98 16 18 8B C5 8A 6E 7C 15 38 99 16 18 89 46 .......n|.8....F
1483:0F30 88 BA 46 56 16 06 94 17 9F 4A 17 69 52 17 93 F9 ..FV.....J.iR...
1483:0F40 11 17 3E 16 07 44 4A 19 07 C3 89 CC 5C 35 03 EB ..>..DJ.....\5..
1483:0F50 F8 0C CC 7C 12 11 7C CC 17 3E D0 39 30 11 D0 20 ...|..|..>.90.. 
1483:0F60 1B D0 83 E0 09 D0 C7 46 01 1C 3A 00 8C 4E 1E 1E .......F..:..N..
1483:0F70 AE 4D 70 30 CD 21 3C 03 72 63 8C C0 2C 10 8E D8 .Mp0.!<.rc..,...
1483:0F80 8E 1E 2C 00 33 F6 AC 0A C0 75 FB 04 76 F6 30 00 ..,.3....u..v.0.
1483:0F90 89 76 1C 8C 5E 1E 8B D6 30 B4 3D 28 8B D8 B4 00 .v..^...0.=(....
1483:0FA0 3F B9 20 00 06 1F 33 D2 C0 0C 72 29 81 3E 14 E0 ?. ...3...r).>..
1483:0FB0 D3 75 21 83 30 3E 16 BE 75 1A 2E 06 A1 0E 00 39 .u!.0>..u......9
1483:0FC0 06 F0 33 75 10 09 10 8C 09 04 09 06 30 B4 3E 28 ..3u........0.>(
1483:0FD0 EB 06 BA 01 B5 03 E9 94 02 1F BE 85 11 E8 28 01 ..............(.
1483:0FE0 54 C8 10 E8 77 1F 43 49 8B D3 E8 18 0C 0F 5E 1A T...w.CI......^.
1483:0FF0 BE 45 E8 3E 26 02 12 7E 65 02 55 5F 81 2A 5F 7B .E.>&..~e.U_.*_{
1483:1000 90 5F 75 01 E8 1A F9 02 5F 4A D8 5F 5F D1 12 29 ._u....._J.__..)
1483:1010 5F B6 F7 05 E5 1D 57 AC F2 21 57 61 01 FF 1F 33 _.....W..!Wa...3
1483:1020 F6 8B 4E 0E DD B5 03 8B 46 1A E8 5E 01 E4 01 65 ..N.....F..^...e
1483:1030 CB 03 F9 F7 65 F9 4C 65 0C 50 1E 06 51 53 8B 3F ....e.Le.P..QS.?
1483:1040 7E 08 0D BC 84 17 0F 59 E3 01 42 01 EA 87 26 3A ~......Y..B...&:
1483:1050 88 83 00 FE 00 75 07 8C D8 80 C4 E8 E1 5C E2 63 .....u.......\.c
1483:1060 E4 4A 75 E1 07 1F 58 3B C3 74 46 BA B1 03 0E 1F .Ju...X;.tF.....
1483:1070 A0 97 C5 76 1C 0E 07 07 BF CA 03 AC AA 40 F4 FA ...v.........@..
1483:1080 4F B8 03 0D 0A AB B0 24 AA 0F 1B BA BB 03 01 EF O......$........
1483:1090 B6 74 B2 24 56 B9 4A 69 72 75 73 24 20 43 68 65 .t.$V.Jirus$ Che
1483:10A0 63 6B 20 46 61 69 6C 65 64 3A 20 65 F7 D7 C3 B2 ck Failed: e....
1483:10B0 F5 7A C3 1A FD 27 C3 7C 29 03 AA 5D 73 55 5D 6D .z...'.|)..]sU]m
1483:10C0 27 5D 67 01 73 EE F2 01 5D CA 94 BE 5D C3 12 5F ']g.s...]...].._
1483:10D0 5D A8 51 3F 5D 01 2F 4F 92 F7 EF B5 F5 52 B5 4B ].Q?]./O.....R.K
1483:10E0 3E BF B6 F9 7E AF B6 22 11 EA B6 1E 3A B6 20 FE >...~.."....:. .
1483:10F0 1D 87 B6 1E 8C 5E 20 F4 37 B6 C3 03 25 E9 A2 D5 .....^ .7...%...
1483:1100 B6 36 BA B6                                     .6..           

;; fn1483_1104: 1483:1104
fn1483_1104 proc
	sub	ax,5D97
	mov	dh,26
	aam	B6
	sbb	al,75

l1483_110D:
	mov	dh,34
	std
	adc	[bp+450F],si
	add	ax,bp
	inc	ax
	push	es
	mov	cl,1C
	mov	[bp+si+5E],dx
	sub	word ptr [bx+di+675E],7D
	sbb	ax,CA22
	add	bl,[bp-20]
	push	dx
	pop	si
	fyl2xp1	st(1),st(0)
	adc	bl,[bp+7D]
	mov	si,9F59
	sbb	al,02
	xlat
	les	bx,[si]
	cmc
	add	al,C4
	fscale	st(5),st(0)
	test	sp,99FE
	mov	di,BFC4
	add	[bp+1EC4],bp
	jmp	110D
1483:1149                            D8 FA 0A C4 C9 FE 1C          .......
1483:1150 85 C4 4D 43 36 38 BA C5 32 2E 48 E7 7B FF FF 41 ..MC68..2.H.{..A
1483:1160 FA 02 30 4F EF FE 80 24 4F 20 18 47 E8 00 0A 4B ..0O...$O .G...K
1483:1170 FA FF EA 4D F5 AA 18 0D 49 F3 05 70 00 03 10 2B ...M....I..p...+
1483:1180 FF FE 41 F6 87 0F 08 08 56 05 E1 67 02 52 48 22 ..A.....V..g.RH"
1483:1190 48 B1 CC 63 2C 20 0C 41 0F 04 52 4C BD 11 04 1B H..c, .A..RL....
1483:11A0 72 49 EC FF E0 4C D4 00 FF 48 E0 FF 00 B9 CB 62 rI...L...H.....b
1483:11B0 F0 97 CC D7 C8 C0 63 00 12 2F 09 30 06 3C 00 38 ......c../.0.<.8
1483:11C0 22 D8 01 17 51 C8 FF 1C FA 4E 75 7E 00 1C 2B 00 "...Q....Nu~..+.
1483:11D0 01 E1 5E 1C 13 72 02 61 3C A6 38 20 4A 05 D0 0E ..^..r.a<.8 J...
1483:11E0 41 EA 00 80 61 07 31 EA 01 00 C7 07 C0 00 03 88 A...a.1.........
1483:11F0 53 40 38 00 60 43 1C 19 03 8D 61 42 44 80 43 28 S@8.`C....aBD.C(
1483:1200 9D FF 1F 36 37 1A D9 01 1C 4D FC 3B 60 2A 25 6B ...67....M.;`*%k
1483:1210 1A 1A DB 82 0D 10 17 57 3D 58 10 13 EF A8 72 01 .......W=X....r.
1483:1220 EF 69 53 41 CC 81 8C 80 51 CC FF C0 BB CE 65 9A .iSA....Q.....e.
1483:1230 60 4F 71 F4 30 18 C0 46 90 58 66 F8 12 28 00 3C `Oq.0..F.Xf..(.<
1483:1240 9E 01 6C C1 7D 34 E2 AE 10 8C 0B 3D B0 9F 02 1D ..l.}4.....=....
1483:1250 6D 16 53 51 33 14 00 30 9A 0F 3E 25 8C 1D 16 1D m.SQ3..0..>%....
1483:1260 05 60 C0 AF 70 FF 72 10 99 11 04 B0 11 0F DE 01 .`..p.r.........
1483:1270 EE 01 AE 48 46 58 4B 1C 23 C6 C3 23 09 0D 92 07 ...HFXK.#..#....
1483:1280 7E 10 1D 80 17 72 03 E1 88 10 18 20 51 C9 E3 70 ~....r..... Q..p
1483:1290 07 1F 72 05 61 CA 05 99 7C 34 00 CF 36 00 4F EF ..r.a...|4..6.O.
1483:12A0 FF F0 22 4F 70 0F 72 04 61 B6 12 C0 51 CA FF F6 .."Op.r.a...Q...
1483:12B0 70 01 E2 98 17 A3 74 00 6C 48 E7 07 00 38 03 43 p.....t.lH...8.C
1483:12C0 EF 00 0C B2 19 66 3A 7A 01 E3 6D 53 45 30 C5 2A .....f:z..mSE0.*
1483:12D0 02 48 45 3E 01 53 47 E3 55 E2 56 51 CF 4F 7A 00 .HE>.SG.U.VQ.Oz.
1483:12E0 10 9A 01 EA 6E 30 C6 11 60 41 B7 1A 03 9A 04 33 ....n0..`A.....3
1483:12F0 11 45 B3 7C 00 1F 05 EB 6E 53 46 31 46 CA AF D4 .E.|....nSF1F...
1483:1300 03 E5 E2 88 52 01 B2 00 C9 11 66 AE 4C DF 00 E0 ....R.....f.L...
1483:1310 C6 73 00 10 87 03 20 3A 00 4C 9B C0 1D 0B 01 80 .s.... :.L......
1483:1320 81 89 3A 70 0A 3F 20 1B 27 98 0B 08 08 90 30 8E ..:p.? .'.....0.
1483:1330 DB 07 26 4E D7 81 1D 2E 81 8C 33 FF FF 2F 42 40 ..&N......3../B@
1483:1340 1E 1F 2F 4D 00 00 52 4C EF 7F FF 00 16 E3 45 52 ../M..RL......ER
1483:1350 47 15 65 05 13 3E F5 C9 3E 38 3F 7C 3C 61 3E 3F G.e..>..>8?|<a>?
1483:1360 3A 11 1C 3F B2 03 6A AA E6 84 45 D6 E1 45 CE 38 :..?..j...E..E.8
1483:1370 45 71 C6 03 8E F0 05 45 48 F8 03 45 7C 3C 03 3B Eq.....EH..E|<.;
1483:1380 45 30 03 AB 20 1A DB BB 2D 70 D7 11 F8 E2 7C 5D E0.. ...-p....|]
1483:1390 0F 3A 4B BA 1F 4B 94 F7 1F 4B 40 12 4B A0 92 02 .:K..K...K@.K...
1483:13A0 90 9C 3A 4C 8E 1F 4C 00 0E 00 4C 22 4B 61 00 01 ..:L..L...L"Ka..
1483:13B0 06 DE 72 FF B4 6B 50 67 66 31 3A C3 47 5C 42 E0 ..r..kPgf1:.G\B.
1483:13C0 86 5C 01 2A F8 EB 5C 17 4A 76 00 08 22 03 74 07 .\.*..\.Jv..".t.
1483:13D0 E2 49 64 04 0A 41 A0 01 01 7B 30 C1 52 03 66 EA .Id..A...{0.R.f.
1483:13E0 85 7B 12 19 B3 C3 02 32 02 02 42 00 FF D4 42 34 .{.....2..B...B4
1483:13F0 32 20 30 32 49 0F 53 80 66 54 E8 0B 92 58 9B C0 2 02I.S.fT...X..
1483:1400 86 22 4D 61 C0 72 FE B4 7A 00 54 66 1E 4D 3E 02 ."Ma.r..z.Tf.M>.
1483:1410 00 14 37 9E 1D C4 20 9E E1 9D 84 38 9D 7C 82 17 ..7... ....8.|..
1483:1420 3E 9D D2 01 1F 9D 2E 47 0F 9D 40 7A FF 3A C7 F7 >......G..@z.:..
1483:1430 91 C1 79 91 53 54 6A 47 B9 BC 60 1A 1F 0F 7C 0C ..y.STjG..`...|.
1483:1440 CE A7 42 9C 3F AA 9E 61 EF 95 46 F7 A7 D5 FE 72 ..B.?..a..F....r
1483:1450 89 BA 97 C0 89 24 6F 00 44 22 0D 60 30 0F A4 66 .....$o.D".`0..f
1483:1460 2E 20 01 4D 43 FA 00 8C D1 D9 C0 01 E9 00 04 2C . .MC..........,
1483:1470 07 48 20 18 67 1A 6E 5B C6 F3 D3 91 C1 0E 67 0E .H .g.n[......g.
1483:1480 D3 C0 C1 11 01 66 F2 43 87 1D FD 60 EE 61 C3 60 .....f.C...`.a.`
1483:1490 25 58 B8 A1 03 14 E1 03 1C 25 87 85 08 D2 AA 33 %X.......%.....3
1483:14A0 0F 07 10 8E 07 14 38 07 18 23 3C F8 03 B6 44 4F ......8..#<...DO
1483:14B0 7F B6 04 C6 E3 3F 00 3F 51 1F 4C 4E 41 12 1F B8 .....?.?Q.LNA...
1483:14C0 06 11 9F 13 C6 B1 41 C8 F0 1B C7 AA F8 61 C7 7D ......A......a.}
1483:14D0 47 F7 9F 5D F7 1F D3 39 12 D3 A0 2B 03 1A A8 D4 G..]...9...+....
1483:14E0 7C 8C 15 3E D4 FC 21 C7 E5 94 C3 47 E4 50 EC F7 |..>..!....G.P..
1483:14F0 E5 FA B4 1F 53 C6 03 40 53 C2 66 78 8A 3E 35 6E ....S..@S.fx.>5n
1483:1500 6D 26 47 AD CC 2C 26 E5 25 0E F2 1B 25 F0 FB 21 m&G..,&.%...%..!
1483:1510 5F 6D 88 47 2F 25 4E F7 B7 6D E7 F7 19 E4 6A 19 _m.G/%N..m....j.
1483:1520 41 4D FF 01 18 6B A4 0F 03 E9 E0 14 A6 48 7A 3A AM...k.......Hz:
1483:1530 02 92 52 09 FE 9C 05 0D FF 03 EC 2A 57 4A 95 67 ..R........*WJ.g
1483:1540 03 21 62 2A 55 DB CD 01 01 2E 8D 58 4D 41 ED 8C .!b*U......XMA..
1483:1550 9B 0C 98 52 43 C9 BD E2 4F 27 C8 07 23 CA 20 2B ...RC...O'..#. +
1483:1560 03 FF FA 72 FF B0 42 A4 25 3A 48 66 40 97 2F 0F ...r..B.%:Hf@./.
1483:1570 7C F2 03 FC 2D 0D FC 1D 0C 87 1B 28 4D D9 D4 CA |...-......(M...
1483:1580 07 0B C2 A5 0B E0 39 0B 4E D8 5C 0B D0 03 7C A4 ......9.N.\...|.
1483:1590 05 BE 0B 5E 03 5F 0B 52 03 29 0B 46 76 2B 72 0B ...^._.R.).Fv+r.
1483:15A0 02 DF 9D 05 23 11 A9 53 81 FA 04 2B 20 56 1F E3 ....#..S...+ V..
1483:15B0 CE 5B F8 CC E5 5F CD 01 70 6C DD 22 60 1F 00 FE .[..._..pl."`...
1483:15C0 EC 56 35 27 28 1B D6 91 1B 36 00 38 F2 04 1B C2 .V5'(....6.8....
1483:15D0 E9 1B 11 CB FF F6 36 13 93 1F 36 04 E4 25 1B 04 ......6...6..%..
1483:15E0 9A 7C 03 0B BE 1B CB 44 4E 1B 0C 94 80 4D EC 66 .|.....DN....M.f
1483:15F0 2C 42 06 9C 22 14 67 F2 05 30 20 14 03 41 FA FD ,B..".g..0 ..A..
1483:1600 06 A2 20 50 D1 C8 01 8F 84 A1 24 08 58 82 00 15 .. P......$.X...
1483:1610 D5 B5 08 00 53 81 08 66 F4 60 D6 92 51 FD 80 E1 ....S..f.`..Q...
1483:1620 E5 88 58 80 2F 40 02 40 22 2F 02 04 D0 D7 04 88 ..X./@.@"/......
1483:1630 D7 25 7F FF 04 BB 03 F2 44 AF ED C4 4E 54 1D AF .%......D...NT..
1483:1640 92 09 AF 44 C3 AF 80 E1 05 AF 14 F1 1E AF F1 43 ...D...........C
1483:1650 99 17 8F 4E B8 84 CB D8 E1 9F D0 38 9F 71 C8 03 ...N.......8.q..
1483:1660 9C F0 05 9F 56 F8 03 9F 7C 4A 03 3B 9F 3E 3E F5 ....V...|J.;.>>.
1483:1670 26 07 8F CB 12 87 A5 B4 42 A5 8E 23 A5 86 5F EC &.......B..#.._.
1483:1680 60 A2 7F 04 00 E3 DE 97 E1 0F 61 F0 F0 19 61 CE `.........a...a.
1483:1690 94 01 61 01 C0 22 2F 01 84 D6 BE 7A 84 02 27 61 ..a.."/....z..'a
1483:16A0 13 5F BA 10 B0 70 60 A9 E8 60 7D 9E 0D DF 10 6E ._...p`..`}....n
1483:16B0 23 77 10 D6 DD 03 10 46 F7 47 10 45 CE B3 2A 84 #w.....F.G.E..*.
1483:16C0 76 E6 E1 76 DE 38 76 45 D6 BF 5E 05 0F 76 64 03 v..v.8vE..^..vd.
1483:16D0 87 76 58 C3 03 76 4C E3 2E 76 EE 0D 1C E0 FB F7 .vX..vL..v......
1483:16E0 FD 1C 2C DF 1C 96 19 0F BA 74 1A BC 1C A1 BA 48 ..,......t.....H
1483:16F0 38 BA 74 8F 3E BA 38 0D EF 1B 08 69 BA 1B B4 A1 8.t.>.8....i....
1483:1700 9E D2 38 9E 4E CA 1C 9E C2 03 7C 96 05 3E 9E 50 ..8.N.....|..>.P
1483:1710 03 1F 9E 44 03 0F 9E 38 28 FC B5 EE 0F 10 FB F5 ...D...8(.......
1483:1720 BE 0F FC 19 1F 54 DA 14 70 0F 43 4F 4D 5F 54 32 .....T..p.COM_T2
1483:1730 00 53 01 46 BE 4A 02 FC E8 CC 75 05 0A 8B 00 C8 .S.F.J....u.....
1483:1740 E8 34 00 8B D0 03 C6 32 05 06 06 F8 E1 83 C6 04 .4.....2........
1483:1750 AD 32 E4 8B EE 8B F7 03 C1 8B 0D 3B FE 9E 76 0D .2.........;..v.
1483:1760 FD 57 4E 4F 8B CA F3 A4 47 8B EF 5E FC 8B FE 56 .WNO....G..^...V
1483:1770 BE 4C 01 B9 FA 00 0F                            .L.....        

;; fn1483_1777: 1483:1777
fn1483_1777 proc
	sbb	bl,al
	call	467E
	fsubr	dword ptr [di+61B8]
	xchg	al,ah
	ret
1483:1783          8B F5 F9 AC 12 C0 02 C0 BF 4C E9 8B 00    .........L...
1483:1790 C2 0A EB 31 08 04 32 20 04 33 04 3B 80 04 06 B1 ...1..2 .3.;....
1483:17A0 04 D7 22 1E 74 F5 12 FF E2 F8 B1 03 02 CF 02 C9 ..".t...........
1483:17B0 F3 A5 EB 5E 1C 18 72 6E 16 1C CA 12 C9 05 1C C9 ...^..rn........
1483:17C0 73 0D 05 18 C8 FE C9 0D 80 F9 1C 09 74 CE 0C 18 s...........t...
1483:17D0 C0 73 21 05 75 03 E6 27 38 98 08 72 57 03 0A FF .s!.u..'8..rW...
1483:17E0 75 0B FE C7 C2 01 17 8A 1C 46 00 B0 4E 2B F3 FA u........F..N+..
1483:17F0 F3 1B 26 A4 FB 8E 40 6B 26 0C A4 C7 21 72 05 00 ..&...@k&...!r..
1483:1800 04 73 F6 74 EF B1 02 70 2A 3F 74 8F 38 73 92 05 .s.t...p*?t.8s..
1483:1810 2D 0E 73 CC FE C1 0B 07 2A 73 9D 87 8A 0C 46 0A -.s.....*s....F.
1483:1820 C9 74 26 80 C1 08 EB 91 85 01 50 80 CF 04 82 6B .t&.......P....k
1483:1830 A3 EB 98 08 BC CE 2C 04 D1 14 30 A7 BE E9 33 C0 ......,...0...3.
1483:1840 FF 2B E6 90 4B 51 A1 61 01 A8 BE AC D9 52 3E 3C .+..KQ.a.....R><
1483:1850 52 36 FC 02 52 E8 0F 2A 00 53 50 18 C0 54 4E 01 R6..R..*.SP..TN.
1483:1860 B9 5A 01 FD 06 54 83 1F 5A E8 FE 00 F3 02 5A 8A .Z...T..Z.....Z.
1483:1870 F3 0C 5A 3A F3 10 5A 5D F2 08 5A 0C 32 5A 49 59 ..Z:..Z]..Z.2ZIY
1483:1880 59 CF C7 0B C1 E6 3F 59 B9 B1 FC 03 5A 2C 1F 73 Y.....?Y....Z,.s
1483:1890 CB 41 05 9F 59 83 17 9F 59 CF 09 83 59 8B CF 2B .A..Y...Y...Y..+
1483:18A0 CE 11 F9 04 00 8B 61 56 81 A6 EC 00 02 8B FC 51 ......aV.......Q
1483:18B0 33 DB 8B C3 B9 08 00 D1 E8 73 03 35 01 A0 E2 F7 3........s.5....
1483:18C0 AB FE C3 75 ED 59 18 00 AC 32 D8 8A C7 32 FF D1 ...u.Y...2...2..
1483:18D0 18 E3 8B 19 09 E2 F1 30 81 C4 2D 5E 3B DA B8 E0 .......0..-^;...
1483:18E0 74 1A E8 00 00 5A 83 C2 0D B4 09 CD 21 B8 FF 4C t....Z......!..L
1483:18F0 04 42 61 64 20 01 43 52 43 0D 0A 24 C3 2D B4 2B .Bad .CRC..$.-.+
1483:1900 03 77 01 6A BE 6E E4 34 07 1E 01 F9 0C 07 13 B9 .w.j.n.4........
1483:1910 10 00 04 CF 13 D2 E2 F5 65 BA 9B 64 BA 3C 10 04 ........e..d.<..
1483:1920 3D 04 41 3E 04 45 F8 0D BA A5 26 1C 30 55 FF 03 =.A>.E....&.0U..
1483:1930 60 FE 39 D1 CA EB 63 8C 14 7A E2 21 BF B2 C5 BE `.9...c..z.!....
1483:1940 B2 C5 BD 79 C5 C4 C7 0B B6 E7 0B C5 63 E4 19 C5 ...y........c...
1483:1950 0C 08 26 18 A4 6E 6A 64 CF CB 0B 01 05 0A 73 EA ..&..njd......s.
1483:1960 74 E3 B0 D1 83 73 86 AC D1 BF 96 D1 7E 91 03 D1 t....s......~...
1483:1970 7E 85 0C D1 1F 97 EB 8C 0A 9F D1 A1 05 33 2B A4 ~............3+.
1483:1980 8E 01 CC 3E BE D0 36 5F 2C 7E 0B 27 2C 23 01 EF ...>..6_,~.',#..
1483:1990 F7 7F 10 DF 7F 57 22 51 45 58 45 D7 05 05 03 25 .....W"QEXE....%
1483:19A0 4D 5A 00 12 00 02 51 06 00 16 EF 03 0B 0E 00 00 MZ....Q.........
1483:19B0 0E 8C D3 8E C3 8C CA 0C 8E DA 8B 0E 93 8B 18 F1 ................
1483:19C0 83 EE A2 FE D1 B8 60 E9 FD F3 A5 53 B8 3D 00 50 ......`....S.=.P
1483:19D0 8B 2E 0A 59 16 0C 00 CB 03 B8 00 10 3B C5 76 00 ...Y........;.v.
1483:19E0 1B C5 2B E8 2B D0 2B D8 D8 2E 34 B1 03 B8 65 D3 ..+.+.+...4...e.
1483:19F0 E0 8B C8 D1 E0 48 48 8B F0 8B F8 30 0B CB ED 75 .....HH....0...u
1483:1A00 D9 FC 07 8E DB 2E C7 06 12 00 3E 00 2E 8C 0E 14 ..........>.....
1483:1A10 00 1E 30 37 30 B1 3C 03 72 03 67 8C C0 2C 10 8E ..070.<.r.g..,..
1483:1A20 00 37 1E 2C 00 33 F6 AC 0A 10 C0 75 FB 04 00 F6 .7.,.3.....u....
1483:1A30 83 C6 02 2E 89 36 12 E3 2C 1E 06 2C 8B D6 B4 3D .....6..,..,...=
1483:1A40 2C 00 8B D8 B4 3F B9 20 00 06 18 1F 33 D2 0C 72 ,....?. ....3..r
1483:1A50 29 1C 81 3E 14 C5 0E 75 21 83 3E 00 CB 75 1A 2E )..>...u!.>..u..
1483:1A60 A1 0E 00 39 63 06 E2 75 10 38 09 10 09 04 C3 09 ...9c..u.8......
1483:1A70 06 B4 3E 00 28 EB 06 BA E6 02 E9 D1 18 01 1F BE ..>.(...........
1483:1A80 11 E8 10 06 02 50 53 E8 0B AC 00 C8 8B D3 E8 04 .....PS.........
1483:1A90 02 53 BE C3 5F E8 63 01 82 05 33 FF 06 51 A7 F7 .S.._.c...3..Q..
1483:1AA0 D5 D1 00 50 8B DF 83 E7 0F 81 C7 35 00 80 CA C7 ...P.......5....
1483:1AB0 D3 EB 8C C0 03 C3 2D 00 08 8E C0 8B DE 83 E6 0F ......-.........
1483:1AC0 8C 0F D8 0F 8E 2C D8 58 39 2E 82 1F 38 58 5A 59 .....,.X9...8XZY
1483:1AD0 33 F6 E8 43 00 8C DA 0E 1F 32 ED BE 05 16 8A C8 3..C.....2......
1483:1AE0 06 E3 13 AD 03 C2 32 2E 33 FF 78 32 E4 AC 03 F8 ......2.3.x2....
1483:1AF0 26 01 15 E2 F8 EB E8 8B 36 04 00 8B 3E 06 00 03 &.......6...>...
1483:1B00 FA 01 16 A7 83 EA 31 10 8E 22 DA 33 85 24 DB FA ......1..".3.$..
1483:1B10 8B C3 E6                                        ...            

l1483_1B13:
	mov	ss,di
	sti
	jmp	dword ptr cs:[bx]
1483:1B19                            50 1E 06 16 07 E0 17          P......
1483:1B20 E5 E3 01 7C 42 01 22 E8 26 01 E9 83 FE 00 75 07 ...|B.".&.....u.
1483:1B30 8E 95 80 C4 80 1E E2 E4                         ........       

l1483_1B38:
	dec	dx
	jnz	1B7F

l1483_1B3B:
	loope	1B6D

l1483_1B3D:
	clc
	pop	es
	xchg	ax,si
	cmp	ax,bx
	jz	1B51

l1483_1B44:
	dec	ax
	mov	dx,02E2
	xchg	ax,sp
	adc	al,bh
	jz	1B13

l1483_1B4E:
	add	[si],ah
	push	cs

l1483_1B51:
	pop	es
	mov	di,02FB
	lodsb
	stosb
	call	1B91
	cli
	dec	di
	mov	ax,0A0D
	stosw
	mov	al,24
	popa
	stosb
	sbb	ax,ECBA
	add	dh,bl
	add	[bx+di],bx
	rcl	[di],cl

l1483_1B6D:
	and	al,5C
	push	si
	mov	si,7269
	jnz	1BE8

l1483_1B75:
	and	al,20
	inc	bx
	push	6365
	imul	sp,[bx+si],46
	popa

l1483_1B7F:
	imul	bp,[si+65],3A64
	and	[bp+si],al
	mov	ch,AF
	xor	dh,[bx+di]
	mov	bx,D909
	in	ax,0C
	xor	[2CF2],cl

;; fn1483_1B91: 1483:1B91
;;   Called from:
;;     1483:1B57 (in fn1483_1BB9)
fn1483_1B91 proc
	repne sub	al,28
	xor	ax,2EF9
	sub	al,bh
	not	word ptr [bx+di+10F8]
	xchg	ax,cx
	push	sp
	pop	dx
	pop	ds
	xchg	[bx+di+3201],di
	adc	ax,A587
	in	al,41
	add	al,[ecx]
	sti
	test	ch,ah
	inc	word ptr [FF1F]
	in	al,dx
	sbb	al,F8
	and	al,0A

;; fn1483_1BB8: 1483:1BB8
;;   Called from:
;;     1483:0C6E (in fn1483_0C11)
fn1483_1BB8 proc
	or	al,[si]

;; fn1483_1BB9: 1483:1BB9
;;   Called from:
;;     1483:1BB7 (in fn1483_1B91)
;;     1483:1BB8 (in fn1483_1BB8)
fn1483_1BB9 proc
	add	al,73
	add	bp,cx

l1483_1BBC:
	jmp	1B38

l1483_1BBF:
	mov	ax,[E10D]
	illegal
	cmp	[di],cx
	aas
	mov	di,4D25
	mul	ax
	and	ax,0EBA
	add	bp,cx
	stc
	xchg	ax,di
	shl	word ptr [di],01
	cmp	ah,dh
	and	ax,BE33
	and	ax,972C
	iret
	and	ax,018B
	test	word ptr [bx+38AD],AD8F
	or	bl,bh

l1483_1BE8:
	dec	bp
	sub	ax,85FE
	dec	bp
	or	bh,[bx+03]
	push	es
	iret
1483:1BF2       4D 23 03 09 F9 4D 14 03 FF 24 4D 27 53 54   M#...M...$M'ST
1483:1C00 F0 4C 02 82 60 1A E9 26 7C 76 0C 86 3B FF FF 48 .L..`..&|v..;..H
1483:1C10 E7 03 B8 EC 41 FA 02 64 20 18 47 E8 00 0A 4B FA ....A..d .G...K.
1483:1C20 FF F0 4D F5 CC 0D 49 60 F3 05 70 00 10 2B 0E FF ..M...I`..p..+..
1483:1C30 FE 41 F6 1C 0F 08 08 2D B8 50 67 02 52 48 22 48 .A.....-.Pg.RH"H
1483:1C40 B1 CC 63 2C 20 0C 0F 04 52 6F 4C 11 04 01 1B 49 ..c, ...RoL....I

;; fn1483_1C50: 1483:1C50
fn1483_1C50 proc
	in	al,dx
	jmp	ax
1483:1C53          4C D4 AE 56 18 48 E0 FF 00 B9 CB 62 F0    L..V.H.....b.
1483:1C60 97 CC D7 C8 5D 00 12 00 2F 09 30 3C 00 3F 22 D8 ....].../.0<.?".
1483:1C70 C5 01 51 C8 C3 FF FA 4E 75 7E 80 DE 07 1E 1B DF ..Q....Nu~......
1483:1C80 07 32 05 60 3A BA 05 09 60 30 1E 05 44 03 05 51 .2.`:...`0..D..Q
1483:1C90 46 05 4C E1 09 05 52 7A 03 AE 37 12 67 CA DD 46 F.L...Rz..7.g..F
1483:1CA0 51 CD FF F8 54 46 1A DB 01 01 51 CE FF F6 60 6C Q...TF....Q...`l
1483:1CB0 C1 19 B6 DB 45 01 05 64 0E 53 06 07 B4 0D 66 BA ....E..d.S....f.
1483:1CC0 7B 09 2F E0 0B AE 64 1A 86 05 39 80 05 65 6E 4A {./...d...9..enJ
1483:1CD0 08 46 66 08 52 35 0B A8 11 C7 E1 5E 1C 1B 20 4D .Ff.R5.....^.. M
1483:1CE0 90 C6 53 48 E2 4D 64 02 1A D8 70 3B 1D 0C 1C 1B ..SH.Md...p;....
1483:1CF0 10 1A C6 01 83 67 FA 60 12 6A 13 01 20 09 08 85 .....g.`.j.. ...
1483:1D00 65 6C 0E 71 39 65 40 06 05 64 F4 03 67 EC 7A 02 el.q9e@..d..g.z.
1483:1D10 7C 00 86 49 2C 64 FC 70 7E 07 2A 64 20 B0 52 7D |..I,d.p~.*d .R}
1483:1D20 28 00 64 88 1A 1B 67 34 50 45 70 60 F1 67 20 CC (.d...g4PEp`.g .
1483:1D30 67 00 01 04 E0 09 1C 65 8E 28 60 86 CD CE A2 05 g......e.(`.....
1483:1D40 D0 8A 05 D2 28 05 DA 05 DE C5 25 66 04 00 69 98 ....(.....%f..i.
1483:1D50 60 0C 72 03 E1 04 88 10 18 51 C9 65 3B 20 C3 3A `.r......Q.e; .:
1483:1D60 00 B6 9B C0 24 6F 00 44 22 0D 30 0C 0B A4 66 2E ....$o.D".0...f.
1483:1D70 B7 43 07 FA 00 8C D1 D9 18 01 E9 5D 2C 48 02 20 .C.........],H. 
1483:1D80 18 67 1A 43 F5 31 BF D3 91 80 35 67 0E D3 C0 B0 .g.C.1....5g....
1483:1D90 3C 06 00 01 66 F2 43 1D 1D FD 60 EE 81 8F 60 25 <...f.C...`...`%
1483:1DA0 58 00 0C C7 03 14 00 03 1C 25 41 00 08 D2 73 AA X........%A...s.
1483:1DB0 0F 38 07 10 07 14 E3 07 18 80 23 3C 70 0A 3F 20 .8........#<p.? 
1483:1DC0 EC AD FC E5 0B CB 4F 20 08 90 8E 4E D7 4F EF 01 ......O ...N.O..
1483:1DD0 80 2E 81 4C DF FF FF 3F 00 3F 02 51 4C 4E 41 42 ...L...?.?.QLNAB
1483:1DE0 1E 00 21 2F 4D 00 52 4C 06 EF 7F FF 00 16 21 DB ..!/M.RL......!.
1483:1DF0 09 A1 E2 0A 65 22 8C 7F F4 7C B8 1B 3E 8D 9A 5B ....e"...|..>..[
1483:1E00 1F 8D 46 0C 05 8D 70 0F B1 75 D9 44 C7 1B F4 43 ..F...p..u.D...C
1483:1E10 9D D2 E1 03 9D 56 E5 03 05 58 1E 05 5E 09 05 7C .....V...X..^..|
1483:1E20 64 06 01 9D 10 1B B9 00 1A C0 EE 0A 05 80 AD E6 d...............
1483:1E30 E2 5C 60 71 72 2B A4 50 AF A4 9C 27 AF A2 0E AF .\`qr+.P...'....
1483:1E40 B8 2A 0B 9C 15 AF 9C 03 AF 9C 65 7E E1 01 AF 96 .*........e~....
1483:1E50 F0 1B AF 18 F8 01 AF 54 0E 24 45 1A 77 DD 73 0F .......T.$E.w.s.
1483:1E60 B5 0C 02 04 0B 64 E8 67 E0 E1 BB 34 D1 BB 01 8F .....d.g...4....
1483:1E70 32 64 A4 50 BB 30 E3 0F 7C A1 BD 3A D1 BD 23 19 2d.P.0..|..:..#.
1483:1E80 24 E0 01 BF 20 65 E8 E7 E2 15 74 8A F3 C6 28 05 $... e....t...(.
1483:1E90 C8 05 CA A2 05 D6 51 C9 F0 01 C3 8A F8 BF C3 9D ......Q.........
1483:1EA0 0A AA 12 87 C4 86 C3 15 C4 F4 E0 0C C4 22 4B 60 ............."K`
1483:1EB0 61 C5 D8 72 FF B4 60 6B 72 66 00 02 92 F8 47 D4 a..r..`krf....G.
1483:1EC0 7C 4F F7 7C D4 68 3E D4 4A 04 2E D4 F8 4F EF FE |O.|.h>.J....O..
1483:1ED0 00 20 4F 76 00 22 03 74 07 E2 49 64 04 0A 41 A0 . Ov.".t..Id..A.
1483:1EE0 01 51 CA FF F6 30 C1 52 03 66 EA 74 00 12 19 B3 .Q...0.R.f.t....
1483:1EF0 02 32 02 02 42 60 D4 42 03 34 37 20 00 E0 49 06 .2..B`.B.47 ..I.
1483:1F00 0F 53 80 66 E8 37 2A 02 00 45 12 C2 9B C0 C3 22 .S.f.7*..E....."
1483:1F10 4D 61 B8 72 FE B4 7A 00 BE 66 74 E7 6E 1E 7C 8B Ma.r..z..ft.n.|.
1483:1F20 32 CA 1E 4F 1D 02 DC 1B 97 1D BE CB 11 1D A2 E5 2..O............
1483:1F30 01 1D 5C F2 47 1D 49 FA F7 3E 80 32 8F 80 F7 8F ..\.G.I..>.2....
1483:1F40 E7 04 8C BF A0 36 4E 38 D2 59 01 F8 50 CC 01 7D .....6N8.Y..P..}
1483:1F50 F6 5B 0F AF 32 F7 8F BC 42 B4 3D 48 9B 76 C0 55 .[..2...B.=H.v.U
1483:1F60 7C 3A 03 22 14 3F 19 14 DB 07 26 F4 04 6F 1F 2B |:.".?....&..o.+
1483:1F70 12 89 12 44 04 EC D1 4E 2E AF D2 2C 5B 87 05 38 ...D...N...,[..8
1483:1F80 FF F7 E0 EC 78 B5 F8 42 3B 9F D3 2B 88 87 3C 86 ....x..B;..+..<.
1483:1F90 D8 1B 2D 38 F8 47 4C 7C 42 F7 7D 4C B6 8F 2D 54 ..-8.GL|B.}L..-T
1483:1FA0 03 F8 45 50 66 1A F8 14 96 B7 7E EB 1A 27 96 09 ..EPf.....~..'..
1483:1FB0 95 52 C3 95 50 E9 1B A5 7C 02 47 3F 95 3B F7 BF .R..P...|.G?.;..
1483:1FC0 E8 80 4F A5 C1 47 5F 41 4D F5 02 F4 63 70 0E 03 ..O..G_AM...cp..
1483:1FD0 E9 80 13 99 48 7A 02 20 E8 6A 60 FE 07 FF F2 2A ....Hz. .j`....*
1483:1FE0 57 1D 4A 95 67 48 53 2A 55 DB 60 CD 01 2E 8D 58 W.J.gHS*U.`....X
1483:1FF0 4D 03 41 ED 00 08 0C 98 04 3F 43 02 66 E2 A3 88 M.A......?C.f...
1483:2000 E0 07 84 E6 20 60 2B EA 72 FF B0 42 60 66 0B D0 .... `+.r..B`f..
1483:2010 48 40 3F 33 00 2F 0F F2 E3 03 8E E3 0D 7E E0 0C H@?3./.......~..
1483:2020 7C 28 4D 22 D9 D4 25 6C BE 4A 0D EA 04 AA CA 6E |(M"..%l.J.....n
1483:2030 06 84 54 F4 A3 54 00 0B DA 46 DA 45 53 45 1A 1A ..T..T...F.ESE..
1483:2040 DB B9 2D FC 4E 05 3A 17 F2 B9 D4 2F 8C 64 41 44 ..-.N.:..../.dAD
1483:2050 66 81 09 64 12 53 45 EA 02 15 87 6E B0 96 01 1B f..d.SE....n....
1483:2060 26 44 09 4F 76 45 D2 47 EE 0C 52 46 BE 02 19 05 &D.OvE.G..RF....
1483:2070 11 7E 08 1F 7E 01 0A 6E 1A 0F 45 0C 60 18 01 24 .~..~..n..E.`..$
1483:2080 3D 0E 91 AA AD E2 F4 59 E2 B3 6D FE 62 C3 01 0B =......Y..m.b...
1483:2090 AC 52 45 CE 03 17 70 85 88 24 55 EE 66 E9 02 77 .RE...p..$U.f..w
1483:20A0 44 8E D9 57 19 7A C7 1D 6C 85 01 0F 8E 20 1F 43 D..W.z..l.... .C
1483:20B0 2A 56 A0 2A 5F 66 1D 46 0C 94 80 DD EC 66 00 FE *V.*_f.F.....f..
1483:20C0 30 42 06 9C 22 14 67 F0 05 30 20 14 03 41 FA FE 0B..".g..0 ..A..
1483:20D0 06 10 20 50 D1 C8 01 EC 4F F8 08 24 08 58 82 5C .. P....O..$.X.\
1483:20E0 15 F1 D5 B5 08 00 53 81 66 F4 60 D6 20 3A FD F0 ......S.f.`. :..
1483:20F0 E5 88 58 80 2F 40 00 40 22 2F 6D 58 4F 10 52 7F ..X./@.@"/mXO.R.
1483:2100 7C FF 38 44 C8 11 7A 03 F2 12 7C B5 B2 F0 38 7A |.8D..z...|...8z
1483:2110 07 79 48 7A 01 DE C3 07 7A B6 E3 1E 7A E3 40 64 .yHz....z...z.@d
1483:2120 A1 24 A0 F1 14 54 D5 AB 8D D8 DE FC 49 1D 3E 4A .$...T......I.>J
1483:2130 BA 15 1F 4A 78 23 09 4A 14 C3 4A 12 CA 01 3D 08 ...Jx#.J..J...=.
1483:2140 A8 BF FA 67 E6 F8 07 44 79 68 01 0B B2 F0 05 44 ...g...Dyh.....D
1483:2150 76 E8 44 54 22 3E 44 6C 0F 11 44 80 FF 39 01 22 v.DT">Dl..D..9."
1483:2160 0D 96 3E 42 01 1F 38 72 07 0F 38 52 00 FC 16 1F ..>B..8r..8R....
1483:2170 38 FE 32 0E 1F 38 02 A7 FA 54 76 4C F3 FB 90 B3 8.2..8...TvL....
1483:2180 FB FE F3 07 FB D6 EA 23 76 C4 FA 03 BE 76 AE 44 .......#v....v.D
1483:2190 BF 76 F7 1F 11 09 1F 11 0B 57 54 52 C3 07 1B 32 .v.......WTR...2
1483:21A0 E1 17 1B 12 F5 4E 54 94 4F 57 C4 00 10 E1 57 81 .....NT.OW....W.
1483:21B0 DD CE 7C 00 07 3E 57 D8 66 BF 53 F7 DF B8 2B EF ..|..>W.f.S...+.
1483:21C0 B8 C3 01 59 50 E1 07 59 30 F0 17 59 10 FA 10 9E ...YP..Y0..Y....
1483:21D0 75 03 F2 00 00 0E 08 0A 12 13 16 00 04 04 04 05 u...............
1483:21E0 05 05 00 06 08 09 15 17 1D 1F 28 29 2C 2D 38 39 ..........(),-89
1483:21F0 3C 3D 01 03 04 04 05 05 05 05 06 06 06 06 06 06 <=..............
1483:2200 06 06 45 58 45 00 43 4F 4D 00 43 4F 44 45 00 44 ..EXE.COM.CODE.D
1483:2210 41 54 41 00 42 53 53 20 00 53 59 4D 42 00 44 42 ATA.BSS .SYMB.DB
1483:2220 55 47 00 48 55 4E 4B 20 25 75 20 28 25 73 29 20 UG.HUNK %u (%s) 
1483:2230 00 08 08 08 08 08 08 08 08 08 08 08 08 08 08 00 ................
1483:2240 20 20 20 00 08 08 08 25 32 75 25 25 00 00 00 00    ....%2u%%....
1483:2250 00 20 20 20 20 20 20 20 20 20 21 21 21 21 21 20 .         !!!!! 
1483:2260 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20                 
1483:2270 20 01 40 40 40 40 40 40 40 40 40 40 40 40 40 40  .@@@@@@@@@@@@@@
1483:2280 40 02 02 02 02 02 02 02 02 02 02 40 40 40 40 40 @..........@@@@@
1483:2290 40 40 14 14 14 14 14 14 04 04 04 04 04 04 04 04 @@..............
1483:22A0 04 04 04 04 04 04 04 04 04 04 04 04 40 40 40 40 ............@@@@
1483:22B0 40 40 18 18 18 18 18 18 08 08 08 08 08 08 08 08 @@..............
1483:22C0 08 08 08 08 08 08 08 08 08 08 08 08 40 40 40 40 ............@@@@
1483:22D0 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ...............
1483:22E0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
1483:2350 00 00 0C 8B 0C 8B 0C 8B 00 00 09 02 00 00 00 00 ................
1483:2360 00 00 00 00 00 00 00 00 00 00 58 23 00 00 0A 02 ..........X#....
1483:2370 01 00 00 00 00 00 00 00 00 00 00 00 00 00 6C 23 ..............l#
1483:2380 00 00 02 02 02 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2390 00 00 80 23 00 00 43 02 03 00 00 00 00 00 00 00 ...#..C.........
1483:23A0 00 00 00 00 00 00 94 23 00 00 42 02 04 00 00 00 .......#..B.....
1483:23B0 00 00 00 00 00 00 00 00 00 00 A8 23 00 00 00 00 ...........#....
1483:23C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
1483:24E0 00 00 00 00 00 00 00 00 14 00 01 60 02 60 02 60 ...........`.`.`
1483:24F0 04 A0 02 A0 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2500 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2510 00 00 00 40 FF FF 00 00 00 13 02 02 04 05 06 08 ...@............
1483:2520 08 08 14 15 05 13 FF 16 05 11 02 FF FF FF FF FF ................
1483:2530 FF FF FF FF FF FF FF FF 05 05 FF FF FF FF FF FF ................
1483:2540 FF FF FF FF FF FF FF FF FF FF 0F FF 23 02 FF 0F ............#...
1483:2550 FF FF FF FF 13 FF FF 02 02 05 0F 02 FF FF FF 13 ................
1483:2560 FF FF FF FF FF FF FF FF 23 FF FF FF FF 23 FF 13 ........#....#..
1483:2570 FF 00 54 4D 50 00 2E 24 24 24 00 00 00 02 02 02 ..TMP..$$$......
1483:2580 02 02 02 02 02 01 01 01 01 01 02 02 02 02 02 02 ................
1483:2590 02 02 02 02 02 02 02 02 02 02 02 02 01 02 02 02 ................
1483:25A0 02 03 02 02 02 02 04 02 02 02 02 02 05 05 05 05 ................
1483:25B0 05 05 05 05 05 05 02 02 02 02 02 02 02 02 02 02 ................
1483:25C0 07 0A 15 0A 0C 09 02 02 0B 02 14 0E 02 02 02 02 ................
1483:25D0 02 08 02 02 12 02 02 10 02 10 02 02 02 02 02 06 ................
1483:25E0 07 0A 0A 0A 0C 09 02 02 0D 02 11 0E 13 02 02 0F ................
1483:25F0 02 08 02 02 12 02 02 02 02 02 02 02 00 10 28 6E ..............(n
1483:2600 75 6C 6C 29 00 00 14 14 01 14 15 14 14 14 14 02 ull)............
1483:2610 00 14 03 04 14 09 05 05 05 05 05 05 05 05 05 14 ................
1483:2620 14 14 14 14 14 14 14 14 14 14 0F 17 0F 08 14 14 ................
1483:2630 14 07 14 16 14 14 14 14 14 14 14 14 14 0D 14 14 ................
1483:2640 14 14 14 14 14 14 14 14 10 0A 0F 0F 0F 08 0A 14 ................
1483:2650 14 06 14 12 0B 0E 14 14 11 14 0C 14 14 0D 14 14 ................
1483:2660 14 14 14 14 14 00 70 72 69 6E 74 20 73 63 61 6E ......print scan
1483:2670 66 20 3A 20 66 6C 6F 61 74 69 6E 67 20 70 6F 69 f : floating poi
1483:2680 6E 74 20 66 6F 72 6D 61 74 73 20 6E 6F 74 20 6C nt formats not l
1483:2690 69 6E 6B 65 64 0D 0A 00 00 00 00 00 00 00 00 00 inked...........
1483:26A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0D 00 ................
1483:26B0 00 00 00 00 1F 1C 1F 1E 1F 1E 1F 1F 1E 1F 1E 1F ................
1483:26C0 00 00 1F 00 3B 00 5A 00 78 00 97 00 B5 00 D4 00 ....;.Z.x.......
1483:26D0 F3 00 11 01 30 01 4E 01 6D 01 F0 4E 83 14 EC 4E ....0.N.m..N...N
1483:26E0 83 14 50 46 00 00 01 00 54 5A 00 45 53 54 00 45 ..PF....TZ.EST.E
1483:26F0 44 54 00 00 16 9D 1B 9D 1B 9D 1B 9D 00 02 7C 96 DT............|.
1483:2700 00 00 00 10 DE A2 00 00 00 10 1D A4 00 00 00 1E ................
1483:2710 79 C3 00 08 03 BE 50 00 BF 01 00 B9 39 00 8C CD y.....P.....9...
1483:2720 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
1483:4670 00 00 00 00 00 00 00 00 00 00 00 00 00 00       .............. 

;; fn1483_467E: 1483:467E
;;   Called from:
;;     1483:1779 (in fn1483_1777)
fn1483_467E proc
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
