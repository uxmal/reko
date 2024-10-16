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
	mov	es,cs:[025Ah]
	push	si
	push	di
	mov	si,2714h
	mov	di,2714h
	call	01E6h
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
	mov	ah,4Ch
	mov	al,[bp+2h]
	int	21h
0800:016D                                        B9 0E 00              ...
0800:0170 BA 2F 00 E9 D5 00                               ./....          

;; fn0800_0176: 0800:0176
fn0800_0176 proc
	push	ds
	mov	ax,3500h
	int	21h
	mov	[005Bh],bx
	mov	[005Dh],es
	mov	ax,3504h
	int	21h
	mov	[005Fh],bx
	mov	[0061h],es
	mov	ax,3505h
	int	21h
	mov	[0063h],bx
	mov	[0065h],es
	mov	ax,3506h
	int	21h
	mov	[0067h],bx
	mov	[0069h],es
	mov	ax,2500h
	mov	dx,cs
	mov	ds,dx
	mov	dx,16Dh
	int	21h
	pop	ds
	ret

;; fn0800_01B9: 0800:01B9
;;   Called from:
;;     0800:8B36 (in fn0800_8B0D)
fn0800_01B9 proc
	push	ds
	mov	ax,2500h
	lds	dx,[005Bh]
	int	21h
	pop	ds
	push	ds
	mov	ax,2504h
	lds	dx,[005Fh]
	int	21h
	pop	ds
	push	ds
	mov	ax,2505h
	lds	dx,[0063h]
	int	21h
	pop	ds
	push	ds
	mov	ax,2506h
	lds	dx,[0067h]
	int	21h
	pop	ds
	ret

;; fn0800_01E6: 0800:01E6
;;   Called from:
;;     0800:015D (in fn0800_0150)
fn0800_01E6 proc
	cmp	si,26FCh
	jz	01F0h

l0800_01EC:
	xor	ah,ah
	jmp	01F2h

l0800_01F0:
	mov	ah,0FFh

l0800_01F2:
	mov	dx,di
	mov	bx,si

l0800_01F6:
	cmp	bx,di
	jz	021Dh

l0800_01FA:
	cmp	byte ptr es:[bx],0FFh
	jz	0218h

l0800_0200:
	cmp	si,26FCh
	jz	020Ch

l0800_0206:
	cmp	ah,es:[bx+1h]
	jmp	0210h

l0800_020C:
	cmp	es:[bx+1h],ah

l0800_0210:
	ja	0218h

l0800_0212:
	mov	ah,es:[bx+1h]
	mov	dx,bx

l0800_0218:
	add	bx,6h
	jmp	01F6h

l0800_021D:
	cmp	dx,di
	jz	023Ch

l0800_0221:
	mov	bx,dx
	cmp	byte ptr es:[bx],0h
	mov	byte ptr es:[bx],0FFh
	push	es
	jz	0235h

l0800_022E:
	call	dword ptr es:[bx+2h]
	pop	es
	jmp	01E6h

l0800_0235:
	call	word ptr es:[bx+2h]
	pop	es
	jmp	01E6h

l0800_023C:
	ret

;; fn0800_023D: 0800:023D
fn0800_023D proc
	mov	ah,40h
	mov	bx,2h
	int	21h
	ret
0800:0245                B9 1E 00 BA 3D 00 2E 8E 1E 5A 02      ....=....Z.
0800:0250 E8 EA FF B8 03 00 50 E8 0F 89 00 00 03 40       ......P......@  

;; main: 0800:025E
main proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	push	si
	mov	word ptr [2A27h],1h
	mov	word ptr [2E4Dh],0h
	mov	word ptr [2A1Fh],0h
	mov	word ptr [2E4Fh],0h
	mov	word ptr [2A1Dh],0h
	mov	word ptr [2A17h],0h
	mov	word ptr [2A1Bh],0h
	mov	word ptr [2A11h],0h
	mov	word ptr [2A0Fh],0h
	mov	word ptr [2A0Dh],0h
	mov	word ptr [2A0Bh],3000h
	mov	word ptr [2E31h],8000h
	mov	word ptr [2E2Fh],1000h
	mov	word ptr [2A21h],1h
	mov	word ptr [29F5h],0h
	mov	word ptr [29F3h],0h
	mov	word ptr [29F1h],0h
	mov	word ptr [29EFh],0h
	mov	word ptr [2A13h],0h
	mov	word ptr [2A23h],1h
	call	2C9Ah
	push	ds
	mov	ax,94h
	push	ax
	push	ds
	mov	ax,7E8h
	push	ax
	call	0B2EFh
	add	sp,8h
	call	0402h
	call	0541h
	mov	ax,1h
	push	ax
	push	ax
	call	2DBFh
	add	sp,4h
	xor	ax,ax
	push	ax
	push	ax
	call	9764h
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	mov	bx,[2A25h]
	cmp	bx,8h
	ja	0338h

l0800_031A:
	shl	bx,1h
	jmp	word ptr cs:[bx+3F0h]

l0800_0321:
	call	0DE8h
	jmp	0338h

l0800_0326:
	call	12E2h
	jmp	0338h

l0800_032B:
	call	18D9h
	jmp	0338h

l0800_0330:
	call	112Dh
	jmp	0338h

l0800_0335:
	call	19EEh

l0800_0338:
	xor	ax,ax
	push	ax
	push	ax
	call	9764h
	add	sp,4h
	sub	ax,[bp-4h]
	sbb	dx,[bp-2h]
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	push	word ptr [29F1h]
	push	word ptr [29EFh]
	push	word ptr [29F5h]
	push	word ptr [29F3h]
	call	0B79h
	add	sp,8h
	mov	si,ax
	xor	ax,ax
	mov	dx,3Ch
	push	ax
	push	dx
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	8BCAh
	push	dx
	push	ax
	xor	ax,ax
	mov	dx,3Ch
	push	ax
	push	dx
	mov	dx,0E10h
	push	ax
	push	dx
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	8BCAh
	push	dx
	push	ax
	call	8BBBh
	push	dx
	push	ax
	xor	ax,ax
	mov	dx,0E10h
	push	ax
	push	dx
	mov	dx,5180h
	push	ax
	push	dx
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	8BCAh
	push	dx
	push	ax
	call	8BBBh
	push	dx
	push	ax
	mov	ax,si
	mov	bx,64h
	xor	dx,dx
	div	bx
	push	dx
	mov	ax,si
	xor	dx,dx
	div	bx
	push	ax
	push	word ptr [29F1h]
	push	word ptr [29EFh]
	push	word ptr [29F5h]
	push	word ptr [29F3h]
	push	ds
	mov	ax,5DCh
	push	ax
	mov	ax,8h
	push	ax
	push	word ptr [2A13h]
	push	ds
	mov	ax,7EBh
	push	ax
	call	0B2EFh
	add	sp,24h
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
	mov	ax,[2A27h]
	cmp	ax,[269Ah]
	jnz	0410h

l0800_040D:
	call	0DCEh

l0800_0410:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	push	word ptr es:[bx+2h]
	push	word ptr es:[bx]
	call	0BFC7h
	add	sp,4h
	cmp	ax,1h
	jbe	044Dh

l0800_042F:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	push	word ptr es:[bx+2h]
	push	word ptr es:[bx]
	mov	ax,1h
	push	ax
	call	0D24h
	add	sp,6h

l0800_044D:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	les	bx,es:[bx]
	mov	al,es:[bx]
	push	ax
	push	ds
	mov	ax,829h
	push	ax
	call	0C29h
	add	sp,6h
	mov	[2A25h],ax
	cmp	ax,9h
	jl	0492h

l0800_0474:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	push	word ptr es:[bx+2h]
	push	word ptr es:[bx]
	mov	ax,1h
	push	ax
	call	0D24h
	add	sp,6h

l0800_0492:
	inc	word ptr [2A27h]
	cmp	word ptr [2A25h],2h
	jg	04A0h

l0800_049D:
	jmp	053Eh

l0800_04A0:
	mov	ax,[2A27h]
	cmp	ax,[269Ah]
	jnz	04ACh

l0800_04A9:
	call	0DCEh

l0800_04AC:
	mov	ax,[2A27h]
	inc	word ptr [2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	push	word ptr es:[bx+2h]
	push	word ptr es:[bx]
	push	ds
	mov	ax,4348h
	push	ax
	call	0BF9Eh
	add	sp,8h
	push	ds
	mov	ax,4348h
	push	ax
	call	0C6Ch
	add	sp,4h
	push	ds
	pop	es
	mov	di,4348h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_04E4:
	repne scasb

l0800_04E6:
	not	cx
	mov	ax,2Eh
	dec	di
	std

l0800_04ED:
	repne scasb

l0800_04EF:
	jz	04F8h

l0800_04F1:
	mov	di,0FFFFh
	xor	ax,ax
	mov	es,ax

l0800_04F8:
	inc	di
	cld
	mov	ax,es
	push	ds
	pop	es
	push	di
	mov	di,4348h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_0507:
	repne scasb

l0800_0509:
	not	cx
	mov	ax,5Ch
	sub	di,cx

l0800_0510:
	repne scasb

l0800_0512:
	jz	051Bh

l0800_0514:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_051B:
	dec	di
	mov	ax,es
	pop	ax
	cmp	ax,di
	ja	0538h

l0800_0523:
	push	ds
	pop	es
	mov	di,4348h
	mov	si,833h
	mov	cx,0FFFFh
	xor	ax,ax

l0800_0530:
	repne scasb

l0800_0532:
	dec	di
	mov	cx,5h
	rep movsb

l0800_0538:
	mov	word ptr [2A19h],1h

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
	sub	sp,4h
	push	si
	push	di
	mov	ax,[2A27h]
	cmp	ax,[269Ah]
	jnz	0555h

l0800_0552:
	jmp	0987h

l0800_0555:
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	les	bx,es:[bx]
	cmp	byte ptr es:[bx],2Dh
	jnz	056Bh

l0800_0568:
	jmp	086Dh

l0800_056B:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	les	bx,es:[bx]
	cmp	byte ptr es:[bx],2Fh
	jnz	0584h

l0800_0581:
	jmp	086Dh

l0800_0584:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_058A:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	les	bx,es:[bx]
	mov	al,es:[bx+1h]
	push	ax
	push	ds
	mov	ax,838h
	push	ax
	call	0C29h
	add	sp,6h
	mov	si,ax
	cmp	ax,0Bh
	jl	05CFh

l0800_05B1:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	push	word ptr es:[bx+2h]
	push	word ptr es:[bx]
	mov	ax,2h
	push	ax
	call	0D24h
	add	sp,6h

l0800_05CF:
	cmp	si,6h
	jl	05D7h

l0800_05D4:
	jmp	0659h

l0800_05D7:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	push	word ptr es:[bx+2h]
	push	word ptr es:[bx]
	call	0BFC7h
	add	sp,4h
	cmp	ax,2h
	jnz	0622h

l0800_05F6:
	inc	word ptr [2A27h]
	mov	ax,[2A27h]
	cmp	ax,[269Ah]
	jnz	0606h

l0800_0603:
	call	0DCEh

l0800_0606:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	mov	ax,es:[bx+2h]
	mov	dx,es:[bx]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	jmp	063Fh

l0800_0622:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	mov	ax,es:[bx+2h]
	mov	dx,es:[bx]
	add	dx,2h
	mov	[bp-2h],ax
	mov	[bp-4h],dx

l0800_063F:
	les	bx,[bp-4h]
	mov	al,es:[bx]
	cbw
	or	ax,ax
	jnz	0659h

l0800_064A:
	push	ds
	mov	ax,5DCh
	push	ax
	mov	ax,2h
	push	ax
	call	0D24h
	add	sp,6h

l0800_0659:
	mov	bx,si
	cmp	bx,0Ah
	jbe	0663h

l0800_0660:
	jmp	0869h

l0800_0663:
	shl	bx,1h
	jmp	word ptr cs:[bx+98Dh]

l0800_066A:
	mov	word ptr [2A1Fh],1h
	mov	word ptr [2E4Fh],0h
	jmp	0869h

l0800_0679:
	mov	word ptr [2A1Dh],1h
	jmp	0869h

l0800_0682:
	mov	word ptr [2A1Bh],1h
	jmp	0869h

l0800_068B:
	mov	word ptr [2A17h],1h
	jmp	0869h

l0800_0694:
	mov	word ptr [2A15h],1h
	jmp	0869h

l0800_069D:
	cmp	word ptr [2A19h],0h
	jz	06A7h

l0800_06A4:
	jmp	0869h

l0800_06A7:
	les	di,[bp-4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_06AF:
	repne scasb

l0800_06B1:
	not	cx
	dec	cx
	cmp	cx,1h
	jbe	06C9h

l0800_06B9:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	mov	ax,3h
	push	ax
	call	0D24h
	add	sp,6h

l0800_06C9:
	les	bx,[bp-4h]
	mov	al,es:[bx]
	push	ax
	push	ds
	mov	ax,844h
	push	ax
	call	0C29h
	add	sp,6h
	mov	[2A23h],ax
	cmp	ax,6h
	jl	06F3h

l0800_06E3:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	mov	ax,3h
	push	ax
	call	0D24h
	add	sp,6h

l0800_06F3:
	mov	bx,[2A23h]
	shl	bx,1h
	shl	bx,1h
	push	word ptr [bx+547h]
	push	word ptr [bx+545h]
	push	ds
	mov	ax,84Bh
	push	ax
	push	ds
	mov	ax,4271h
	push	ax
	call	0BEA2h
	add	sp,0Ch
	jmp	0869h

l0800_0716:
	push	ds
	mov	ax,2E4Fh
	push	ax
	push	ds
	mov	ax,84Fh
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0BF18h
	add	sp,0Ch
	cmp	word ptr [2E4Fh],0h
	jnz	0743h

l0800_0733:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	mov	ax,4h
	push	ax
	call	0D24h
	add	sp,6h

l0800_0743:
	mov	word ptr [2A1Fh],0h
	jmp	0869h

l0800_074C:
	push	ds
	mov	ax,2A0Fh
	push	ax
	push	ds
	mov	ax,853h
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0BF18h
	add	sp,0Ch
	jmp	0869h

l0800_0765:
	push	ds
	mov	ax,2A21h
	push	ax
	push	ds
	mov	ax,856h
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0BF18h
	add	sp,0Ch
	cmp	word ptr [2A21h],2h
	jg	0796h

l0800_0782:
	cmp	word ptr [2A21h],0h
	jz	078Ch

l0800_0789:
	jmp	0869h

l0800_078C:
	cmp	word ptr [2A25h],0h
	jz	0796h

l0800_0793:
	jmp	0869h

l0800_0796:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	mov	ax,5h
	push	ax
	call	0D24h
	add	sp,6h
	jmp	0869h

l0800_07A9:
	push	ds
	pop	es
	mov	di,42E3h
	push	es
	mov	es,[bp-2h]
	push	di
	mov	di,[bp-4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_07BB:
	repne scasb

l0800_07BD:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	push	ds
	mov	ax,42E3h
	push	ax
	call	0C6Ch
	add	sp,4h
	push	ds
	pop	es
	mov	di,42E3h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_07E8:
	repne scasb

l0800_07EA:
	not	cx
	dec	cx
	dec	cx
	mov	bx,cx
	cmp	byte ptr [bx+42E3h],5Ch
	jz	0869h

l0800_07F7:
	mov	di,42E3h
	mov	si,859h
	mov	cx,0FFFFh

l0800_0800:
	repne scasb

l0800_0802:
	dec	di
	mov	cx,2h
	rep movsb
	jmp	0869h

l0800_080A:
	push	ds
	pop	es
	mov	di,427Eh
	push	es
	mov	es,[bp-2h]
	push	di
	mov	di,[bp-4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_081C:
	repne scasb

l0800_081E:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	push	ds
	mov	ax,427Eh
	push	ax
	call	0C6Ch
	add	sp,4h
	push	ds
	pop	es
	mov	di,427Eh
	xor	ax,ax
	mov	cx,0FFFFh

l0800_0849:
	repne scasb

l0800_084B:
	not	cx
	dec	cx
	dec	cx
	mov	bx,cx
	cmp	byte ptr [bx+427Eh],5Ch
	jz	0869h

l0800_0858:
	mov	di,427Eh
	mov	si,859h
	mov	cx,0FFFFh

l0800_0861:
	repne scasb

l0800_0863:
	dec	di
	mov	cx,2h
	rep movsb

l0800_0869:
	inc	word ptr [2A27h]

l0800_086D:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	les	bx,es:[bx]
	cmp	byte ptr es:[bx],2Dh
	jnz	0886h

l0800_0883:
	jmp	058Ah

l0800_0886:
	mov	ax,[2A27h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	les	bx,es:[bx]
	cmp	byte ptr es:[bx],2Fh
	jnz	089Fh

l0800_089C:
	jmp	058Ah

l0800_089F:
	mov	ax,[2A21h]
	cmp	ax,1h
	jz	08AEh

l0800_08A7:
	cmp	ax,2h
	jz	08D9h

l0800_08AC:
	jmp	0902h

l0800_08AE:
	push	ds
	pop	es
	mov	di,4271h
	mov	si,85Bh
	mov	cx,0FFFFh
	xor	ax,ax

l0800_08BB:
	repne scasb

l0800_08BD:
	dec	di
	mov	cx,2h
	rep movsb
	cmp	word ptr [2E31h],8000h
	jbe	08D1h

l0800_08CB:
	mov	word ptr [2E31h],8000h

l0800_08D1:
	mov	word ptr [2E2Fh],1000h
	jmp	0902h

l0800_08D9:
	push	ds
	pop	es
	mov	di,4271h
	mov	si,85Dh
	mov	cx,0FFFFh
	xor	ax,ax

l0800_08E6:
	repne scasb

l0800_08E8:
	dec	di
	mov	cx,2h
	rep movsb
	cmp	word ptr [2E31h],1000h
	jbe	08FCh

l0800_08F6:
	mov	word ptr [2E31h],1000h

l0800_08FC:
	mov	word ptr [2E2Fh],0FFh

l0800_0902:
	cmp	word ptr [2A23h],2h
	jnz	0911h

l0800_0909:
	xor	ax,ax
	mov	[2A1Dh],ax
	mov	[2E4Fh],ax

l0800_0911:
	cmp	word ptr [2E4Fh],0h
	jz	092Dh

l0800_0918:
	push	ds
	pop	es
	mov	di,4271h
	mov	si,653h
	mov	cx,0FFFFh
	xor	ax,ax

l0800_0925:
	repne scasb

l0800_0927:
	dec	di
	mov	cx,2h
	rep movsb

l0800_092D:
	cmp	word ptr [2A1Dh],0h
	jz	0949h

l0800_0934:
	push	ds
	pop	es
	mov	di,4271h
	mov	si,85Fh
	mov	cx,0FFFFh
	xor	ax,ax

l0800_0941:
	repne scasb

l0800_0943:
	dec	di
	mov	cx,2h
	rep movsb

l0800_0949:
	push	ds
	pop	es
	mov	di,4271h
	mov	si,861h
	mov	cx,0FFFFh
	xor	ax,ax

l0800_0956:
	repne scasb

l0800_0958:
	dec	di
	mov	cx,5h
	rep movsb
	cmp	word ptr [2A25h],0h
	jnz	0987h

l0800_0965:
	mov	bx,[2A23h]
	shl	bx,1h
	shl	bx,1h
	les	bx,[bx+545h]
	cmp	byte ptr es:[bx],0h
	jz	0987h

l0800_0977:
	push	ds
	mov	ax,2E75h
	push	ax
	push	ds
	mov	ax,4271h
	push	ax
	call	09A3h
	add	sp,8h

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
	sub	sp,8h
	push	si
	push	di
	mov	al,[0A72h]
	cbw
	mov	cl,8h
	shl	ax,cl
	mov	dl,[0A73h]
	mov	dh,0h
	add	ax,dx
	xor	dx,dx
	push	ax
	mov	al,[0A74h]
	cbw
	shl	ax,cl
	mov	bl,[0A75h]
	mov	bh,0h
	add	ax,bx
	add	dx,ax
	pop	ax
	adc	ax,0h
	add	dx,20h
	adc	ax,0h
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	push	word ptr [bp-2h]
	push	ax
	push	ds
	mov	ax,0A6Eh
	push	ax
	nop
	push	cs
	call	867Ah
	add	sp,8h
	jmp	0A4Fh

l0800_09FF:
	les	di,[bp-8h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_0A07:
	repne scasb

l0800_0A09:
	not	cx
	dec	cx
	mov	ax,[bp-8h]
	add	ax,cx
	mov	bx,ax
	mov	al,es:[bx+1h]
	cbw
	mov	cl,8h
	shl	ax,cl
	mov	di,[bp-8h]
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_0A25:
	repne scasb

l0800_0A27:
	not	cx
	dec	cx
	mov	ax,[bp-8h]
	add	ax,cx
	mov	bx,ax
	mov	al,es:[bx+2h]
	mov	ah,0h
	pop	dx
	add	dx,ax
	mov	di,[bp-8h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_0A42:
	repne scasb

l0800_0A44:
	not	cx
	dec	cx
	add	dx,cx
	add	dx,3h
	add	[bp-8h],dx

l0800_0A4F:
	mov	si,[bp+4h]
	push	ds
	mov	ds,[bp+6h]
	les	di,[bp-8h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_0A5E:
	repne scasb

l0800_0A60:
	not	cx
	sub	di,cx
	rep cmpsb
	jz	0A6Dh

l0800_0A68:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_0A6D:
	pop	ds
	or	ax,ax
	jnz	09FFh

l0800_0A72:
	les	di,[bp-8h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_0A7A:
	repne scasb

l0800_0A7C:
	not	cx
	dec	cx
	inc	cx
	add	[bp-8h],cx
	les	bx,[bp-8h]
	mov	al,es:[bx]
	cbw
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+1h]
	mov	dh,0h
	add	ax,dx
	add	ax,2h
	push	ax
	push	word ptr [bp-6h]
	push	bx
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	0B0F3h
	add	sp,0Ah
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	4346h
	add	sp,4h
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
	sub	sp,0Eh
	push	si
	mov	si,[bp+4h]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	ss
	lea	ax,[bp-0Eh]
	push	ax
	call	35A3h
	add	sp,8h
	push	word ptr [2A05h]
	push	word ptr [2A03h]
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	call	0B79h
	add	sp,8h
	mov	cx,ax
	mov	bx,64h
	xor	dx,dx
	div	bx
	push	dx
	mov	ax,cx
	xor	dx,dx
	div	bx
	push	ax
	push	word ptr [2A05h]
	push	word ptr [2A03h]
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	push	ss
	lea	ax,[bp-0Eh]
	push	ax
	mov	ax,14h
	push	ax
	push	ax
	push	ds
	mov	ax,866h
	push	ax
	call	0B2EFh
	add	sp,18h
	cmp	word ptr [2A25h],2h
	jz	0B2Eh

l0800_0B2A:
	or	si,si
	jz	0B47h

l0800_0B2E:
	mov	bx,si
	shl	bx,1h
	shl	bx,1h
	push	word ptr [bx+59Bh]
	push	word ptr [bx+599h]
	push	ds
	mov	ax,889h
	push	ax
	call	0B2EFh
	add	sp,8h

l0800_0B47:
	push	ds
	mov	ax,827h
	push	ax
	call	0B2EFh
	add	sp,4h
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	add	[29F3h],dx
	adc	[29F5h],ax
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	add	[29EFh],dx
	adc	[29F1h],ax
	inc	word ptr [2A13h]
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
	mov	ax,[bp+4h]
	or	ax,[bp+6h]
	jz	0B9Eh

l0800_0B84:
	mov	ax,[bp+8h]
	or	ax,[bp+0Ah]
	jz	0B9Eh

l0800_0B8C:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	cmp	ax,[bp+0Ah]
	ja	0BCEh

l0800_0B97:
	jnz	0B9Eh

l0800_0B99:
	cmp	dx,[bp+8h]
	ja	0BCEh

l0800_0B9E:
	xor	ax,ax
	pop	bp
	ret

l0800_0BA2:
	xor	ax,ax
	mov	dx,0Ah
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BC2h
	mov	[bp+6h],dx
	mov	[bp+4h],ax
	xor	ax,ax
	mov	dx,0Ah
	push	ax
	push	dx
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	8BC2h
	mov	[bp+0Ah],dx
	mov	[bp+8h],ax

l0800_0BCE:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	sub	dx,[bp+8h]
	sbb	ax,[bp+0Ah]
	cmp	ax,6h
	ja	0BA2h

l0800_0BDF:
	jnz	0BE7h

l0800_0BE1:
	cmp	dx,8DB8h
	ja	0BA2h

l0800_0BE7:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	mov	cx,[bp+6h]
	mov	bx,[bp+4h]
	sub	bx,[bp+8h]
	sbb	cx,[bp+0Ah]
	xor	dx,dx
	mov	ax,2710h
	call	8F18h
	push	dx
	push	ax
	call	8BC2h
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
	mov	si,[bp+4h]
	mov	cx,1h
	jmp	0C15h

l0800_0C14:
	inc	cx

l0800_0C15:
	mov	bx,2h
	mov	ax,si
	xor	dx,dx
	div	bx
	mov	si,ax
	or	ax,ax
	jnz	0C14h

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
	sub	sp,4h
	mov	cl,[bp+8h]
	mov	al,cl
	cbw
	push	ax
	call	97CCh
	add	sp,2h
	mov	cl,al
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	jmp	0C4Fh

l0800_0C4C:
	inc	word ptr [bp-4h]

l0800_0C4F:
	les	bx,[bp-4h]
	cmp	byte ptr es:[bx],0h
	jz	0C5Dh

l0800_0C58:
	cmp	es:[bx],cl
	jnz	0C4Ch

l0800_0C5D:
	mov	ax,[bp-4h]
	xor	dx,dx
	sub	ax,[bp+4h]
	sbb	dx,0h
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
	jmp	0C88h

l0800_0C71:
	les	bx,[bp+4h]
	mov	al,es:[bx]
	cbw
	push	ax
	call	97CCh
	add	sp,2h
	les	bx,[bp+4h]
	mov	es:[bx],al
	inc	word ptr [bp+4h]

l0800_0C88:
	les	bx,[bp+4h]
	cmp	byte ptr es:[bx],0h
	jnz	0C71h

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
	sub	sp,66h
	push	si
	push	di
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	ss
	lea	ax,[bp-66h]
	push	ax
	call	3509h
	add	sp,8h
	mov	si,2714h
	push	ss
	pop	es
	lea	di,[bp-66h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_0CB9:
	repne scasb

l0800_0CBB:
	not	cx
	sub	di,cx
	rep cmpsb
	jz	0CC8h

l0800_0CC3:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_0CC8:
	or	ax,ax
	jz	0D0Dh

l0800_0CCC:
	push	ss
	lea	ax,[bp-66h]
	push	ax
	push	ds
	mov	ax,88Fh
	push	ax
	call	0B2EFh
	add	sp,8h
	push	ss
	pop	es
	lea	di,[bp-66h]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,2714h
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_0CF5:
	repne scasb

l0800_0CF7:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds

l0800_0D0D:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	ds
	mov	ax,89Eh
	push	ax
	call	0B2EFh
	add	sp,8h
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
	mov	ax,4477h
	push	ax
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	0DA9h
	add	sp,8h
	push	ds
	mov	ax,4412h
	push	ax
	push	word ptr [29D9h]
	push	word ptr [29D7h]
	call	0DA9h
	add	sp,8h
	push	ds
	mov	ax,43ADh
	push	ax
	push	word ptr [29D5h]
	push	word ptr [29D3h]
	call	0DA9h
	add	sp,8h
	mov	bx,[bp+4h]
	shl	bx,1h
	shl	bx,1h
	push	word ptr [bx+55Fh]
	push	word ptr [bx+55Dh]
	push	ds
	mov	ax,8A2h
	push	ax
	call	0B2EFh
	add	sp,8h
	les	bx,[bp+6h]
	cmp	byte ptr es:[bx],0h
	jz	0D92h

l0800_0D83:
	push	word ptr [bp+8h]
	push	bx
	push	ds
	mov	ax,8A6h
	push	ax
	call	0B2EFh
	add	sp,8h

l0800_0D92:
	push	ds
	mov	ax,827h
	push	ax
	call	0B2EFh
	add	sp,4h
	mov	ax,1h
	push	ax
	call	8B5Ah
	add	sp,2h
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
	mov	ax,[bp+4h]
	or	ax,[bp+6h]
	jz	0DCCh

l0800_0DB4:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0A614h
	add	sp,4h
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	8F7Fh
	add	sp,4h

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
	mov	ax,121h
	push	ax
	push	ds
	mov	ax,7E8h
	push	ax
	call	0B2EFh
	add	sp,8h
	xor	ax,ax
	push	ax
	call	8B5Ah
	add	sp,2h
	ret

;; fn0800_0DE8: 0800:0DE8
;;   Called from:
;;     0800:0321 (in main)
fn0800_0DE8 proc
	push	si
	mov	bx,[2A23h]
	shl	bx,1h
	shl	bx,1h
	push	word ptr [bx+52Fh]
	push	word ptr [bx+52Dh]
	mov	bx,[2A25h]
	shl	bx,1h
	shl	bx,1h
	push	word ptr [bx+50Bh]
	push	word ptr [bx+509h]
	push	ds
	mov	ax,8ACh
	push	ax
	call	0B2EFh
	add	sp,0Ch
	cmp	word ptr [2E4Fh],0h
	jz	0E2Ah

l0800_0E1B:
	push	word ptr [2E4Fh]
	push	ds
	mov	ax,8BAh
	push	ax
	call	0B2EFh
	add	sp,6h

l0800_0E2A:
	cmp	word ptr [2A1Fh],0h
	jz	0E3Ch

l0800_0E31:
	push	ds
	mov	ax,8CFh
	push	ax
	call	0B2EFh
	add	sp,4h

l0800_0E3C:
	push	ds
	mov	ax,8DCh
	push	ax
	call	0B2EFh
	add	sp,4h
	push	ds
	mov	ax,427Eh
	push	ax
	call	3678h
	add	sp,4h
	push	ds
	mov	ax,8DFh
	push	ax
	push	ds
	mov	ax,4477h
	push	ax
	call	37BEh
	add	sp,8h
	push	ds
	mov	ax,8ECh
	push	ax
	push	ds
	mov	ax,4477h
	push	ax
	call	4234h
	add	sp,8h
	mov	[29DDh],dx
	mov	[29DBh],ax
	jmp	10F4h

l0800_0E7C:
	call	3764h
	call	388Ch
	or	ax,ax
	jnz	0E89h

l0800_0E86:
	jmp	10F4h

l0800_0E89:
	push	ds
	mov	ax,4541h
	push	ax
	call	0C93h
	add	sp,4h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	4194h
	add	sp,4h
	mov	[2A09h],dx
	mov	[2A07h],ax
	mov	[2A05h],dx
	mov	[2A03h],ax
	xor	si,si
	cmp	word ptr [2A09h],0h
	ja	0ECFh

l0800_0EB9:
	jnz	0EC2h

l0800_0EBB:
	cmp	word ptr [2A07h],12h
	ja	0ECFh

l0800_0EC2:
	cmp	word ptr [2A15h],0h
	jnz	0ECFh

l0800_0EC9:
	mov	si,3h
	jmp	105Ch

l0800_0ECF:
	call	3992h
	or	ax,ax
	jz	0EDCh

l0800_0ED6:
	mov	si,0Eh
	jmp	105Ch

l0800_0EDC:
	mov	bx,[2A23h]
	cmp	bx,5h
	jbe	0EE8h

l0800_0EE5:
	jmp	1007h

l0800_0EE8:
	shl	bx,1h
	jmp	word ptr cs:[bx+1121h]

l0800_0EEF:
	cmp	word ptr [2A09h],0h
	jc	0F1Fh

l0800_0EF6:
	jnz	0EFFh

l0800_0EF8:
	cmp	word ptr [2A07h],12h
	jc	0F1Fh

l0800_0EFF:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F58h
	add	sp,4h
	mov	cl,8h
	call	8CAAh
	cmp	dx,52h
	jnz	0F1Fh

l0800_0F17:
	cmp	ax,4E43h
	jnz	0F1Fh

l0800_0F1C:
	mov	si,4h

l0800_0F1F:
	or	si,si
	jz	0F26h

l0800_0F23:
	jmp	1007h

l0800_0F26:
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	call	75EAh
	add	sp,8h
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	cmp	ax,[2A09h]
	jnc	0F4Ch

l0800_0F49:
	jmp	1007h

l0800_0F4C:
	jnz	0F57h

l0800_0F4E:
	cmp	dx,[2A07h]
	jnc	0F57h

l0800_0F54:
	jmp	1007h

l0800_0F57:
	cmp	word ptr [2A15h],0h
	jnz	0F61h

l0800_0F5E:
	jmp	1007h

l0800_0F61:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0BA67h
	add	sp,4h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0BA67h
	add	sp,4h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	ax,524Eh
	mov	dx,4300h
	push	ax
	push	dx
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	call	409Ch
	add	sp,8h
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	mov	si,2h
	jmp	1007h

l0800_0FCC:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F0Ah
	add	sp,4h
	cmp	ax,4D5Ah
	jnz	0FE6h

l0800_0FDF:
	call	5E64h
	mov	si,ax
	jmp	1007h

l0800_0FE6:
	call	669Ch
	mov	si,ax
	jmp	1007h

l0800_0FED:
	call	67BFh
	mov	si,ax
	jmp	1007h

l0800_0FF4:
	call	6AD4h
	mov	si,ax
	jmp	1007h

l0800_0FFB:
	call	73ACh
	mov	si,ax
	jmp	1007h

l0800_1002:
	call	741Dh
	mov	si,ax

l0800_1007:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	4194h
	add	sp,4h
	mov	[2A05h],dx
	mov	[2A03h],ax
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	cmp	ax,[2A09h]
	jc	1049h

l0800_1029:
	jnz	1031h

l0800_102B:
	cmp	dx,[2A07h]
	jc	1049h

l0800_1031:
	cmp	word ptr [2A15h],0h
	jnz	1049h

l0800_1038:
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	mov	[2A05h],ax
	mov	[2A03h],dx
	mov	si,3h

l0800_1049:
	cmp	si,4h
	jnz	105Ch

l0800_104E:
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	mov	[2A05h],ax
	mov	[2A03h],dx

l0800_105C:
	cmp	byte ptr [427Eh],0h
	jz	10B7h

l0800_1063:
	cmp	si,3h
	jz	106Dh

l0800_1068:
	cmp	si,4h
	jnz	10B7h

l0800_106D:
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	mov	[2A05h],ax
	mov	[2A03h],dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0BA67h
	add	sp,4h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0BA67h
	add	sp,4h
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	xor	si,si

l0800_10B7:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0A614h
	add	sp,4h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0A614h
	add	sp,4h
	cmp	si,2h
	jle	10E5h

l0800_10D8:
	push	ds
	mov	ax,44DCh
	push	ax
	call	8F7Fh
	add	sp,4h
	jmp	10E8h

l0800_10E5:
	call	37DFh

l0800_10E8:
	push	ds
	mov	ax,4541h
	push	ax
	push	si
	call	0ABCh
	add	sp,6h

l0800_10F4:
	push	ds
	mov	ax,4541h
	push	ax
	call	2DE2h
	add	sp,4h
	or	ax,ax
	jz	1106h

l0800_1103:
	jmp	0E7Ch

l0800_1106:
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	0A614h
	add	sp,4h
	push	ds
	mov	ax,4477h
	push	ax
	call	8F7Fh
	add	sp,4h
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
	mov	bx,[2A23h]
	shl	bx,1h
	shl	bx,1h
	push	word ptr [bx+52Fh]
	push	word ptr [bx+52Dh]
	mov	bx,[2A25h]
	shl	bx,1h
	shl	bx,1h
	push	word ptr [bx+50Bh]
	push	word ptr [bx+509h]
	push	ds
	mov	ax,8F0h
	push	ax
	call	0B2EFh
	add	sp,0Ch
	cmp	word ptr [2E4Fh],0h
	jz	116Fh

l0800_1160:
	push	word ptr [2E4Fh]
	push	ds
	mov	ax,8FEh
	push	ax
	call	0B2EFh
	add	sp,6h

l0800_116F:
	push	ds
	mov	ax,913h
	push	ax
	call	0B2EFh
	add	sp,4h
	cmp	word ptr [2A25h],1h
	jz	1184h

l0800_1181:
	jmp	12C2h

l0800_1184:
	push	ds
	mov	ax,427Eh
	push	ax
	call	3678h
	add	sp,4h
	jmp	12C2h

l0800_1192:
	call	3764h
	call	388Ch
	or	ax,ax
	jnz	119Fh

l0800_119C:
	jmp	12C2h

l0800_119F:
	push	ds
	mov	ax,4541h
	push	ax
	call	0C93h
	add	sp,4h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	4194h
	add	sp,4h
	mov	[2A05h],dx
	mov	[2A03h],ax
	or	dx,dx
	ja	11CFh

l0800_11C3:
	jc	11CAh

l0800_11C5:
	cmp	ax,12h
	jnc	11CFh

l0800_11CA:
	mov	si,7h
	jmp	1243h

l0800_11CF:
	mov	bx,[2A23h]
	cmp	bx,5h
	ja	1243h

l0800_11D8:
	shl	bx,1h
	jmp	word ptr cs:[bx+12D6h]

l0800_11DF:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F58h
	add	sp,4h
	mov	cl,8h
	call	8CAAh
	cmp	dx,52h
	jnz	1203h

l0800_11F7:
	cmp	ax,4E43h
	jnz	1203h

l0800_11FC:
	call	5374h
	mov	si,ax
	jmp	1243h

l0800_1203:
	mov	si,7h
	jmp	1243h

l0800_1208:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F0Ah
	add	sp,4h
	cmp	ax,4D5Ah
	jnz	1222h

l0800_121B:
	call	46FEh
	mov	si,ax
	jmp	1243h

l0800_1222:
	call	4B97h
	mov	si,ax
	jmp	1243h

l0800_1229:
	call	4BB1h
	mov	si,ax
	jmp	1243h

l0800_1230:
	call	4C55h
	mov	si,ax
	jmp	1243h

l0800_1237:
	call	518Fh
	mov	si,ax
	jmp	1243h

l0800_123E:
	call	51A9h
	mov	si,ax

l0800_1243:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	4194h
	add	sp,4h
	mov	[2A09h],dx
	mov	[2A07h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	4194h
	add	sp,4h
	mov	[2A05h],dx
	mov	[2A03h],ax
	or	si,si
	jz	127Fh

l0800_1271:
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	mov	[2A09h],ax
	mov	[2A07h],dx

l0800_127F:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0A614h
	add	sp,4h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0A614h
	add	sp,4h
	cmp	word ptr [2A25h],2h
	jz	12A6h

l0800_12A2:
	or	si,si
	jz	12B3h

l0800_12A6:
	push	ds
	mov	ax,44DCh
	push	ax
	call	8F7Fh
	add	sp,4h
	jmp	12B6h

l0800_12B3:
	call	37DFh

l0800_12B6:
	push	ds
	mov	ax,4541h
	push	ax
	push	si
	call	0ABCh
	add	sp,6h

l0800_12C2:
	push	ds
	mov	ax,4541h
	push	ax
	call	2DE2h
	add	sp,4h
	or	ax,ax
	jz	12D4h

l0800_12D1:
	jmp	1192h

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
	sub	sp,84h
	push	si
	push	di
	push	ds
	mov	ax,4348h
	push	ax
	mov	bx,[2A25h]
	shl	bx,1h
	shl	bx,1h
	push	word ptr [bx+50Bh]
	push	word ptr [bx+509h]
	push	ds
	mov	ax,916h
	push	ax
	call	0B2EFh
	add	sp,0Ch
	cmp	word ptr [2E4Fh],0h
	jz	1321h

l0800_1312:
	push	word ptr [2E4Fh]
	push	ds
	mov	ax,925h
	push	ax
	call	0B2EFh
	add	sp,6h

l0800_1321:
	push	ds
	mov	ax,93Ah
	push	ax
	call	0B2EFh
	add	sp,4h
	push	ds
	mov	ax,93Dh
	push	ax
	push	ds
	mov	ax,4348h
	push	ax
	call	0AA7Eh
	add	sp,8h
	mov	[29D1h],dx
	mov	[29CFh],ax
	mov	ax,[2A25h]
	cmp	ax,3h
	jz	1353h

l0800_134B:
	cmp	ax,4h
	jz	1371h

l0800_1350:
	jmp	1478h

l0800_1353:
	mov	ax,[29CFh]
	or	ax,[29D1h]
	jz	135Fh

l0800_135C:
	jmp	1478h

l0800_135F:
	push	ds
	mov	ax,4348h
	push	ax
	mov	ax,7h
	push	ax
	call	0D24h
	add	sp,6h
	jmp	1478h

l0800_1371:
	mov	ax,[29CFh]
	or	ax,[29D1h]
	jz	13C6h

l0800_137A:
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3E27h
	add	sp,4h
	cmp	ax,4D5Ah
	jnz	13BAh

l0800_138D:
	mov	ax,2h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0AD2Fh
	add	sp,4h
	mov	[29EDh],dx
	mov	[29EBh],ax
	jmp	13C6h

l0800_13BA:
	mov	word ptr [29D1h],0h
	mov	word ptr [29CFh],0h

l0800_13C6:
	mov	ax,[29CFh]
	or	ax,[29D1h]
	jnz	13F2h

l0800_13CF:
	push	ds
	mov	ax,941h
	push	ax
	push	ds
	mov	ax,4348h
	push	ax
	call	4234h
	add	sp,8h
	mov	[29D1h],dx
	mov	[29CFh],ax
	mov	word ptr [29EDh],0h
	mov	word ptr [29EBh],0h

l0800_13F2:
	mov	byte ptr [bp-6Eh],52h
	mov	byte ptr [bp-6Dh],4Eh
	mov	byte ptr [bp-6Ch],43h
	mov	byte ptr [bp-6Bh],41h
	mov	byte ptr [bp-6Ah],0h
	mov	byte ptr [bp-69h],0Ch
	mov	byte ptr [bp-66h],0h
	mov	byte ptr [bp-65h],0Ch
	mov	byte ptr [bp-64h],0h
	mov	byte ptr [bp-63h],0h
	xor	ax,ax
	push	ax
	mov	ax,4h
	push	ax
	push	ss
	lea	ax,[bp-66h]
	push	ax
	call	2CCFh
	add	sp,8h
	mov	cl,8h
	shr	ax,cl
	mov	[bp-68h],al
	xor	ax,ax
	push	ax
	mov	ax,4h
	push	ax
	push	ss
	lea	ax,[bp-66h]
	push	ax
	call	2CCFh
	add	sp,8h
	mov	[bp-67h],al
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	xor	ax,ax
	mov	dx,0Ch
	push	ax
	push	dx
	push	ss
	lea	ax,[bp-6Eh]
	push	ax
	call	4152h
	add	sp,0Ch
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	push	word ptr [29EDh]
	push	word ptr [29EBh]
	call	409Ch
	add	sp,8h

l0800_1478:
	xor	ax,ax
	push	ax
	call	1CF6h
	add	sp,2h
	xor	si,si
	jmp	1527h

l0800_1486:
	push	ds
	mov	ax,4541h
	push	ax
	push	ss
	lea	ax,[bp-6Eh]
	push	ax
	call	3509h
	add	sp,8h
	push	ds
	mov	ax,4541h
	push	ax
	push	ss
	lea	ax,[bp+0FF7Ch]
	push	ax
	call	35A3h
	add	sp,8h
	push	ss
	lea	ax,[bp-6Eh]
	push	ax
	call	283Dh
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	or	ax,dx
	jnz	14CDh

l0800_14BC:
	push	ss
	lea	ax,[bp-6Eh]
	push	ax
	call	1F5Ch
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax

l0800_14CD:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	ss
	lea	ax,[bp+0FF7Ch]
	push	ax
	call	2931h
	add	sp,8h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	or	ax,dx
	jnz	14FDh

l0800_14E9:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	ss
	lea	ax,[bp+0FF7Ch]
	push	ax
	call	2085h
	add	sp,8h
	jmp	1509h

l0800_14FD:
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	call	23ECh
	add	sp,4h

l0800_1509:
	mov	si,1h
	les	bx,[2E51h]
	mov	al,es:[bx+4h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+5h]
	mov	dh,0h
	add	ax,dx
	cmp	ax,0FF8Ch
	jnc	1539h

l0800_1527:
	push	ds
	mov	ax,4541h
	push	ax
	call	2DE2h
	add	sp,4h
	or	ax,ax
	jz	1539h

l0800_1536:
	jmp	1486h

l0800_1539:
	or	si,si
	jnz	1551h

l0800_153D:
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0A614h
	add	sp,4h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_1551:
	call	1E5Eh
	mov	ax,1h
	push	ax
	call	1CF6h
	add	sp,2h
	mov	ax,1h
	push	ax
	xor	ax,ax
	push	ax
	call	2DBFh
	add	sp,4h
	mov	ax,[29D1h]
	mov	dx,[29CFh]
	mov	[29E1h],ax
	mov	[29DFh],dx
	push	ds
	mov	ax,945h
	push	ax
	push	ds
	mov	ax,4477h
	push	ax
	call	37BEh
	add	sp,8h
	push	ds
	mov	ax,941h
	push	ax
	push	ds
	mov	ax,4477h
	push	ax
	call	4234h
	add	sp,8h
	mov	[29DDh],dx
	mov	[29DBh],ax
	jmp	1897h

l0800_15A3:
	push	ds
	mov	ax,93Dh
	push	ax
	push	ds
	mov	ax,4541h
	push	ax
	call	4234h
	add	sp,8h
	mov	[29E5h],dx
	mov	[29E3h],ax
	push	ds
	mov	ax,4541h
	push	ax
	push	ss
	lea	ax,[bp-6Eh]
	push	ax
	call	3509h
	add	sp,8h
	push	ds
	mov	ax,4541h
	push	ax
	push	ss
	lea	ax,[bp+0FF7Ch]
	push	ax
	call	35A3h
	add	sp,8h
	push	ds
	mov	ax,4541h
	push	ax
	call	0C93h
	add	sp,4h
	push	ss
	lea	ax,[bp-6Eh]
	push	ax
	call	283Dh
	add	sp,4h
	push	dx
	push	ax
	push	ss
	lea	ax,[bp+0FF7Ch]
	push	ax
	call	2931h
	add	sp,8h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	4194h
	add	sp,4h
	sub	ax,4h
	sbb	dx,0h
	mov	[29E9h],dx
	mov	[29E7h],ax
	mov	ax,[29E9h]
	cwd
	mov	cl,8h
	call	8C8Ah
	les	di,[bp-8h]
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_1632:
	repne scasb

l0800_1634:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_163A:
	repne scasb

l0800_163C:
	jz	1645h

l0800_163E:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1645:
	dec	di
	mov	ax,es
	mov	es,ax
	pop	ax
	mov	es:[di+1h],al
	mov	ax,[29E9h]
	cwd
	les	di,[bp-8h]
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_165C:
	repne scasb

l0800_165E:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1664:
	repne scasb

l0800_1666:
	jz	166Fh

l0800_1668:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_166F:
	dec	di
	mov	ax,es
	mov	es,ax
	pop	ax
	mov	es:[di+2h],al
	mov	dx,[29E9h]
	mov	ax,[29E7h]
	mov	cl,8h
	call	8C8Ah
	les	di,[bp-8h]
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_168E:
	repne scasb

l0800_1690:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1696:
	repne scasb

l0800_1698:
	jz	16A1h

l0800_169A:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_16A1:
	dec	di
	mov	ax,es
	mov	es,ax
	pop	ax
	mov	es:[di+3h],al
	les	di,[bp-8h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_16B3:
	repne scasb

l0800_16B5:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_16BB:
	repne scasb

l0800_16BD:
	jz	16C6h

l0800_16BF:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_16C6:
	dec	di
	mov	ax,es
	mov	dl,[29E7h]
	mov	es,ax
	mov	es:[di+4h],dl
	mov	di,1h
	xor	ax,ax
	mov	word ptr [2A05h],0h
	mov	[2A03h],ax
	mov	si,ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	4194h
	add	sp,4h
	mov	[2A09h],dx
	mov	[2A07h],ax
	xor	ax,ax
	push	ax
	push	word ptr [29E9h]
	push	word ptr [29E7h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	cmp	word ptr [2A09h],0h
	jnc	171Bh

l0800_1718:
	jmp	17A2h

l0800_171B:
	ja	1724h

l0800_171D:
	cmp	word ptr [2A07h],12h
	jbe	17A2h

l0800_1724:
	cmp	word ptr [2A21h],0h
	jz	17A2h

l0800_172B:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F58h
	add	sp,4h
	mov	cl,8h
	call	8CAAh
	cmp	dx,52h
	jnz	1748h

l0800_1743:
	cmp	ax,4E43h
	jz	17A2h

l0800_1748:
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	call	75EAh
	add	sp,8h
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	cmp	ax,[2A09h]
	jc	1778h

l0800_176B:
	jnz	1773h

l0800_176D:
	cmp	dx,[2A07h]
	jc	1778h

l0800_1773:
	mov	ax,1h
	jmp	177Ah

l0800_1778:
	xor	ax,ax

l0800_177A:
	mov	di,ax
	or	ax,ax
	jz	17A2h

l0800_1780:
	mov	ax,1h
	push	ax
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	neg	ax
	neg	dx
	sbb	ax,0h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah

l0800_17A2:
	or	di,di
	jnz	17A9h

l0800_17A6:
	jmp	183Fh

l0800_17A9:
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	mov	[2A05h],ax
	mov	[2A03h],dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0BA67h
	add	sp,4h
	cmp	word ptr [2A09h],0h
	jc	17F2h

l0800_17CC:
	jnz	17D5h

l0800_17CE:
	cmp	word ptr [2A07h],12h
	jc	17F2h

l0800_17D5:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F58h
	add	sp,4h
	mov	cl,8h
	call	8CAAh
	cmp	dx,52h
	jnz	17F2h

l0800_17ED:
	cmp	ax,4E43h
	jz	181Eh

l0800_17F2:
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	mov	ax,524Eh
	mov	dx,4300h
	push	ax
	push	dx
	call	409Ch
	add	sp,8h
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	call	409Ch
	add	sp,8h

l0800_181E:
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	mov	si,2h

l0800_183F:
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	push	word ptr [29EDh]
	push	word ptr [29EBh]
	call	409Ch
	add	sp,8h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0A614h
	add	sp,4h
	push	ds
	mov	ax,4541h
	push	ax
	push	si
	call	0ABCh
	add	sp,6h
	les	bx,[2E51h]
	mov	al,es:[bx+4h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+5h]
	mov	dh,0h
	add	ax,dx
	cmp	ax,0FF8Ch
	jc	1897h

l0800_188A:
	push	ds
	mov	ax,952h
	push	ax
	call	0B2EFh
	add	sp,4h
	jmp	18A9h

l0800_1897:
	push	ds
	mov	ax,4541h
	push	ax
	call	2DE2h
	add	sp,4h
	or	ax,ax
	jz	18A9h

l0800_18A6:
	jmp	15A3h

l0800_18A9:
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	0A614h
	add	sp,4h
	push	ds
	mov	ax,4477h
	push	ax
	call	8F7Fh
	add	sp,4h
	call	1E5Eh
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0A614h
	add	sp,4h
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
	sub	sp,6Eh
	push	di
	push	ds
	mov	ax,4348h
	push	ax
	mov	bx,[2A25h]
	shl	bx,1h
	shl	bx,1h
	push	word ptr [bx+50Bh]
	push	word ptr [bx+509h]
	push	ds
	mov	ax,96Ch
	push	ax
	call	0B2EFh
	add	sp,0Ch
	push	ds
	mov	ax,97Dh
	push	ax
	push	ds
	mov	ax,4348h
	push	ax
	call	0AA7Eh
	add	sp,8h
	mov	[29D1h],dx
	mov	[29CFh],ax
	or	ax,dx
	jnz	192Ah

l0800_191B:
	push	ds
	mov	ax,4348h
	push	ax
	mov	ax,7h
	push	ax
	call	0D24h
	add	sp,6h

l0800_192A:
	mov	ax,1h
	push	ax
	call	1CF6h
	add	sp,2h
	xor	ax,ax
	push	ax
	push	ax
	call	2DBFh
	add	sp,4h
	jmp	19BCh

l0800_1940:
	push	ss
	lea	ax,[bp-6Eh]
	push	ax
	call	0C93h
	add	sp,4h
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	call	23ECh
	add	sp,4h
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	call	22FEh
	add	sp,4h
	les	di,[bp-4h]
	add	di,2h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_196E:
	repne scasb

l0800_1970:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1976:
	repne scasb

l0800_1978:
	jz	1981h

l0800_197A:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1981:
	dec	di
	mov	ax,es
	mov	es,ax
	cmp	byte ptr es:[di+1h],0h
	jnz	19ADh

l0800_198D:
	mov	ax,[2E53h]
	mov	dx,[2E51h]
	add	dx,8h
	cmp	ax,[bp-2h]
	jnz	19A1h

l0800_199C:
	cmp	dx,[bp-4h]
	jz	19ADh

l0800_19A1:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	2201h
	add	sp,4h

l0800_19AD:
	push	ss
	lea	ax,[bp-6Eh]
	push	ax
	mov	ax,0Dh
	push	ax
	call	0ABCh
	add	sp,6h

l0800_19BC:
	push	ss
	lea	ax,[bp-6Eh]
	push	ax
	push	ss
	lea	ax,[bp-8h]
	push	ax
	push	ss
	lea	ax,[bp-4h]
	push	ax
	call	29C5h
	add	sp,0Ch
	or	ax,ax
	jz	19D8h

l0800_19D5:
	jmp	1940h

l0800_19D8:
	call	1E5Eh
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0A614h
	add	sp,4h
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
	sub	sp,72h
	push	si
	push	di
	push	ds
	mov	ax,4348h
	push	ax
	mov	bx,[2A25h]
	shl	bx,1h
	shl	bx,1h
	push	word ptr [bx+50Bh]
	push	word ptr [bx+509h]
	push	ds
	mov	ax,986h
	push	ax
	call	0B2EFh
	add	sp,0Ch
	push	ds
	mov	ax,997h
	push	ax
	push	ds
	mov	ax,4348h
	push	ax
	call	0AA7Eh
	add	sp,8h
	mov	[29D1h],dx
	mov	[29CFh],ax
	or	ax,dx
	jnz	1A40h

l0800_1A31:
	push	ds
	mov	ax,4348h
	push	ax
	mov	ax,7h
	push	ax
	call	0D24h
	add	sp,6h

l0800_1A40:
	mov	ax,[2A27h]
	cmp	ax,[269Ah]
	jnz	1A7Dh

l0800_1A49:
	mov	ax,[0984h]
	mov	dx,[0982h]
	mov	[bp-0Ah],ax
	mov	[bp-0Ch],dx
	mov	ax,[bp-0Ch]
	mov	dx,4h
	les	bx,[269Ch]
	lea	ax,[bp-0Ch]
	mov	es:[bx+6h],ss
	mov	es:[bx+4h],ax
	mov	word ptr [2A27h],1h
	mov	word ptr [269Ah],2h
	mov	word ptr [2A1Bh],1h

l0800_1A7D:
	xor	ax,ax
	push	ax
	mov	ax,1h
	push	ax
	call	2DBFh
	add	sp,4h
	mov	ax,1h
	push	ax
	call	1CF6h
	add	sp,2h
	cmp	word ptr [2A25h],7h
	jz	1A9Eh

l0800_1A9B:
	jmp	1CB4h

l0800_1A9E:
	push	ds
	mov	ax,99Bh
	push	ax
	push	ds
	mov	ax,44DCh
	push	ax
	call	37BEh
	add	sp,8h
	jmp	1CB4h

l0800_1AB1:
	push	ss
	lea	ax,[bp-72h]
	push	ax
	call	0C93h
	add	sp,4h
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	call	24FEh
	add	sp,4h
	xor	ax,ax
	push	ax
	les	di,[bp-8h]
	mov	cx,0FFFFh

l0800_1AD1:
	repne scasb

l0800_1AD3:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1AD9:
	repne scasb

l0800_1ADB:
	jz	1AE4h

l0800_1ADD:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1AE4:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+1h]
	cbw
	mov	cl,8h
	shl	ax,cl
	les	di,[bp-8h]
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_1AFB:
	repne scasb

l0800_1AFD:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1B03:
	repne scasb

l0800_1B05:
	jz	1B0Eh

l0800_1B07:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1B0E:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+2h]
	mov	ah,0h
	pop	dx
	add	dx,ax
	xor	ax,ax
	les	di,[bp-8h]
	push	dx
	push	ax
	mov	cx,0FFFFh

l0800_1B26:
	repne scasb

l0800_1B28:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1B2E:
	repne scasb

l0800_1B30:
	jz	1B39h

l0800_1B32:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1B39:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+3h]
	cbw
	mov	cl,8h
	shl	ax,cl
	les	di,[bp-8h]
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_1B50:
	repne scasb

l0800_1B52:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1B58:
	repne scasb

l0800_1B5A:
	jz	1B63h

l0800_1B5C:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1B63:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+4h]
	mov	ah,0h
	pop	dx
	add	dx,ax
	pop	ax
	add	ax,dx
	pop	dx
	adc	dx,0h
	push	dx
	push	ax
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0ACB3h
	add	sp,0Ah
	cmp	word ptr [2A25h],7h
	jnz	1B92h

l0800_1B8F:
	jmp	1C61h

l0800_1B92:
	push	ds
	pop	es
	mov	di,44DCh
	push	es
	push	di
	mov	di,427Eh
	xor	ax,ax
	mov	cx,0FFFFh

l0800_1BA1:
	repne scasb

l0800_1BA3:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	cmp	word ptr [2A25h],8h
	jnz	1C29h

l0800_1BC0:
	cmp	byte ptr [427Eh],0h
	jz	1BFCh

l0800_1BC7:
	cmp	byte ptr [bp-72h],5Ch
	jnz	1BFCh

l0800_1BCD:
	push	ss
	lea	ax,[bp-71h]
	push	ax
	push	ds
	pop	es
	mov	di,44DCh
	xor	ax,ax
	mov	cx,0FFFFh

l0800_1BDC:
	repne scasb

l0800_1BDE:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1BE4:
	repne scasb

l0800_1BE6:
	jz	1BEFh

l0800_1BE8:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1BEF:
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	3509h
	add	sp,8h
	jmp	1C29h

l0800_1BFC:
	push	ss
	lea	ax,[bp-72h]
	push	ax
	push	ds
	pop	es
	mov	di,44DCh
	xor	ax,ax
	mov	cx,0FFFFh

l0800_1C0B:
	repne scasb

l0800_1C0D:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1C13:
	repne scasb

l0800_1C15:
	jz	1C1Eh

l0800_1C17:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1C1E:
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	3509h
	add	sp,8h

l0800_1C29:
	push	ds
	mov	ax,44DCh
	push	ax
	call	3678h
	add	sp,4h
	push	ss
	lea	ax,[bp-72h]
	push	ax
	push	ds
	pop	es
	mov	di,44DCh
	xor	ax,ax
	mov	cx,0FFFFh

l0800_1C43:
	repne scasb

l0800_1C45:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_1C4B:
	repne scasb

l0800_1C4D:
	jz	1C56h

l0800_1C4F:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1C56:
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	35A3h
	add	sp,8h

l0800_1C61:
	push	ds
	mov	ax,9A8h
	push	ax
	push	ds
	mov	ax,44DCh
	push	ax
	call	4234h
	add	sp,8h
	mov	[29E1h],dx
	mov	[29DFh],ax
	mov	ax,[29D1h]
	mov	dx,[29CFh]
	mov	[29E5h],ax
	mov	[29E3h],dx
	call	5374h
	mov	si,ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0A614h
	add	sp,4h
	or	si,si
	jz	1CA8h

l0800_1C9D:
	push	ds
	mov	ax,44DCh
	push	ax
	call	8F7Fh
	add	sp,4h

l0800_1CA8:
	push	ss
	lea	ax,[bp-72h]
	push	ax
	push	si
	call	0ABCh
	add	sp,6h

l0800_1CB4:
	push	ss
	lea	ax,[bp-72h]
	push	ax
	push	ss
	lea	ax,[bp-8h]
	push	ax
	push	ss
	lea	ax,[bp-4h]
	push	ax
	call	29C5h
	add	sp,0Ch
	or	ax,ax
	jz	1CD0h

l0800_1CCD:
	jmp	1AB1h

l0800_1CD0:
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0A614h
	add	sp,4h
	cmp	word ptr [2A25h],7h
	jnz	1CF0h

l0800_1CE5:
	push	ds
	mov	ax,44DCh
	push	ax
	call	8F7Fh
	add	sp,4h

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
	sub	sp,2h
	push	si
	xor	ax,ax
	xor	dx,dx
	mov	[45A8h],ax
	mov	[45A6h],dx
	mov	[29EDh],ax
	mov	[29EBh],dx
	mov	ax,2h
	push	ax
	mov	ax,0FFFFh
	mov	dx,0FFFCh
	push	ax
	push	dx
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3E5Dh
	add	sp,4h
	mov	[29EDh],dx
	mov	[29EBh],ax
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	4194h
	add	sp,4h
	cmp	dx,[29EDh]
	ja	1D69h

l0800_1D52:
	jc	1D5Ah

l0800_1D54:
	cmp	ax,[29EBh]
	jnc	1D69h

l0800_1D5A:
	push	ds
	mov	ax,4348h
	push	ax
	mov	ax,0Bh
	push	ax
	call	0D24h
	add	sp,6h

l0800_1D69:
	xor	ax,ax
	push	ax
	push	word ptr [29EDh]
	push	word ptr [29EBh]
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3E5Dh
	add	sp,4h
	cmp	dx,524Eh
	jnz	1D9Bh

l0800_1D96:
	cmp	ax,4341h
	jz	1DAAh

l0800_1D9B:
	push	ds
	mov	ax,4348h
	push	ax
	mov	ax,0Bh
	push	ax
	call	0D24h
	add	sp,6h

l0800_1DAA:
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3E27h
	add	sp,4h
	mov	si,ax
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3E27h
	add	sp,4h
	mov	[bp-2h],ax
	cmp	word ptr [bp+4h],0h
	jz	1DE4h

l0800_1DD1:
	xor	ax,ax
	push	ax
	push	si
	call	4311h
	add	sp,4h
	mov	[2E53h],dx
	mov	[2E51h],ax
	jmp	1DF8h

l0800_1DE4:
	xor	ax,ax
	mov	dx,0FFF0h
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[2E53h],dx
	mov	[2E51h],ax

l0800_1DF8:
	xor	ax,ax
	push	ax
	push	word ptr [29EDh]
	push	word ptr [29EBh]
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	xor	ax,ax
	push	ax
	push	si
	push	word ptr [2E53h]
	push	word ptr [2E51h]
	call	4110h
	add	sp,0Ch
	xor	ax,ax
	push	ax
	mov	ax,si
	sub	ax,8h
	push	ax
	mov	ax,[2E51h]
	add	ax,8h
	push	word ptr [2E53h]
	push	ax
	call	2CCFh
	add	sp,8h
	cmp	ax,[bp-2h]
	jz	1E59h

l0800_1E4A:
	push	ds
	mov	ax,4348h
	push	ax
	mov	ax,0Dh
	push	ax
	call	0D24h
	add	sp,6h

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
	les	bx,[2E51h]
	mov	al,es:[bx+4h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+5h]
	mov	dh,0h
	add	ax,dx
	mov	si,ax
	xor	ax,ax
	push	ax
	mov	ax,si
	sub	ax,8h
	push	ax
	mov	ax,[2E51h]
	add	ax,8h
	push	word ptr [2E53h]
	push	ax
	call	2CCFh
	add	sp,8h
	mov	dx,ax
	mov	cl,8h
	shr	ax,cl
	les	bx,[2E51h]
	mov	es:[bx+6h],al
	mov	es:[bx+7h],dl
	xor	ax,ax
	push	ax
	mov	ax,[29EDh]
	mov	dx,[29EBh]
	add	dx,4h
	adc	ax,0h
	push	ax
	push	dx
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3E27h
	add	sp,4h
	mov	dx,ax
	cmp	si,dx
	jbe	1EF7h

l0800_1ED7:
	mov	ax,si
	sub	ax,dx
	xor	dx,dx
	push	dx
	push	ax
	push	word ptr [29EDh]
	push	word ptr [29EBh]
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3BC3h
	add	sp,0Ch
	jmp	1F19h

l0800_1EF7:
	cmp	si,dx
	jnc	1F19h

l0800_1EFB:
	mov	ax,dx
	sub	ax,si
	xor	dx,dx
	push	dx
	push	ax
	push	word ptr [29EDh]
	push	word ptr [29EBh]
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3C99h
	add	sp,0Ch

l0800_1F19:
	xor	ax,ax
	push	ax
	push	word ptr [29EDh]
	push	word ptr [29EBh]
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	xor	ax,ax
	push	ax
	push	si
	push	word ptr [2E53h]
	push	word ptr [2E51h]
	call	4152h
	add	sp,0Ch
	push	word ptr [2E53h]
	push	word ptr [2E51h]
	call	4346h
	add	sp,4h
	pop	si
	ret

;; fn0800_1F5C: 0800:1F5C
;;   Called from:
;;     0800:14C1 (in fn0800_12E2)
fn0800_1F5C proc
	push	bp
	mov	bp,sp
	sub	sp,8h
	push	si
	push	di
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_1F6C:
	repne scasb

l0800_1F6E:
	not	cx
	mov	ax,3Ah
	sub	di,cx

l0800_1F75:
	repne scasb

l0800_1F77:
	jz	1F80h

l0800_1F79:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1F80:
	dec	di
	mov	ax,es
	or	di,ax
	jz	1FADh

l0800_1F87:
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_1F8F:
	repne scasb

l0800_1F91:
	not	cx
	mov	ax,3Ah
	sub	di,cx

l0800_1F98:
	repne scasb

l0800_1F9A:
	jz	1FA3h

l0800_1F9C:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_1FA3:
	dec	di
	mov	ax,es
	inc	di
	mov	[bp+6h],ax
	mov	[bp+4h],di

l0800_1FAD:
	les	bx,[2E51h]
	mov	al,es:[bx+4h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+5h]
	mov	dh,0h
	add	ax,dx
	mov	[bp-6h],ax
	mov	ax,[2E53h]
	mov	dx,[2E51h]
	add	dx,[bp-6h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_1FDE:
	repne scasb

l0800_1FE0:
	not	cx
	dec	cx
	add	cx,4h
	mov	[bp-8h],cx
	mov	ax,[bp-4h]
	xor	dx,dx
	sub	ax,[2E51h]
	sbb	dx,0h
	add	ax,[bp-8h]
	adc	dx,0h
	mov	cl,8h
	call	8C8Ah
	les	bx,[bp-4h]
	mov	es:[bx],al
	mov	ax,[bp-4h]
	xor	dx,dx
	sub	ax,[2E51h]
	sbb	dx,0h
	add	al,[bp-8h]
	mov	es:[bx+1h],al
	mov	di,[bp-4h]
	add	di,2h
	push	es
	mov	es,[bp+6h]
	push	di
	mov	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_202C:
	repne scasb

l0800_202E:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	mov	es,[bp-2h]
	add	bx,[bp-8h]
	mov	byte ptr es:[bx-1h],0h
	mov	ax,[bp-6h]
	add	ax,[bp-8h]
	mov	cl,8h
	shr	ax,cl
	les	bx,[2E51h]
	mov	es:[bx+4h],al
	mov	al,[bp-6h]
	add	al,[bp-8h]
	mov	es:[bx+5h],al
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [bp-8h]
	call	2688h
	add	sp,8h
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
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
	sub	sp,0Ch
	push	si
	push	di
	les	bx,[2E51h]
	mov	al,es:[bx+4h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+5h]
	mov	dh,0h
	add	ax,dx
	mov	[bp-0Ah],ax
	mov	ax,[2E53h]
	mov	dx,[2E51h]
	add	dx,[bp-0Ah]
	mov	[bp-6h],ax
	mov	[bp-8h],dx
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_20BE:
	repne scasb

l0800_20C0:
	not	cx
	dec	cx
	add	cx,5h
	mov	[bp-0Ch],cx
	les	bx,[bp+8h]
	mov	al,es:[bx]
	cbw
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+1h]
	mov	dh,0h
	add	ax,dx
	mov	dx,[2E53h]
	mov	bx,[2E51h]
	add	bx,ax
	dec	bx
	mov	[bp-2h],dx
	mov	[bp-4h],bx
	mov	ax,[bp-8h]
	xor	dx,dx
	sub	ax,[bp-4h]
	sbb	dx,0h
	push	ax
	push	word ptr [2E53h]
	push	bx
	mov	ax,[bp-4h]
	add	ax,[bp-0Ch]
	push	word ptr [2E53h]
	push	ax
	call	0B0F3h
	add	sp,0Ah
	les	di,[bp-4h]
	push	es
	mov	es,[bp+6h]
	push	di
	mov	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_211F:
	repne scasb

l0800_2121:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	les	di,[bp-4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_213F:
	repne scasb

l0800_2141:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2147:
	repne scasb

l0800_2149:
	jz	2152h

l0800_214B:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2152:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di+1h],0h
	les	di,[bp-4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2164:
	repne scasb

l0800_2166:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_216C:
	repne scasb

l0800_216E:
	jz	2177h

l0800_2170:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2177:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di+2h],0h
	les	di,[bp-4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2189:
	repne scasb

l0800_218B:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2191:
	repne scasb

l0800_2193:
	jz	219Ch

l0800_2195:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_219C:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di+3h],0h
	les	di,[bp-4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_21AE:
	repne scasb

l0800_21B0:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_21B6:
	repne scasb

l0800_21B8:
	jz	21C1h

l0800_21BA:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_21C1:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di+4h],0h
	mov	ax,[bp-0Ah]
	add	ax,[bp-0Ch]
	mov	cl,8h
	shr	ax,cl
	les	bx,[2E51h]
	mov	es:[bx+4h],al
	mov	al,[bp-0Ah]
	add	al,[bp-0Ch]
	mov	es:[bx+5h],al
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [bp-0Ch]
	call	2688h
	add	sp,8h
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
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
	sub	sp,0Ah
	push	di
	les	bx,[2E51h]
	mov	al,es:[bx+4h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+5h]
	mov	dh,0h
	add	ax,dx
	mov	[bp-6h],ax
	mov	ax,[2E53h]
	mov	dx,[2E51h]
	add	dx,[bp-6h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	les	bx,[bp+4h]
	mov	al,es:[bx]
	cbw
	shl	ax,cl
	mov	dl,es:[bx+1h]
	mov	dh,0h
	add	ax,dx
	mov	dx,[2E51h]
	add	dx,ax
	xor	ax,ax
	sub	dx,[bp+4h]
	sbb	ax,0h
	mov	[bp-8h],dx
	mov	ax,[bp+4h]
	add	ax,[bp-8h]
	mov	[bp-0Ah],ax
	mov	dx,[bp-4h]
	xor	bx,bx
	sub	dx,ax
	sbb	bx,0h
	push	dx
	push	word ptr [bp+6h]
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0B0F3h
	add	sp,0Ah
	mov	ax,[bp-6h]
	sub	ax,[bp-8h]
	mov	cl,8h
	shr	ax,cl
	les	bx,[2E51h]
	mov	es:[bx+4h],al
	mov	al,[bp-6h]
	sub	al,[bp-8h]
	mov	es:[bx+5h],al
	xor	ax,ax
	push	ax
	push	ax
	mov	ax,[bp-8h]
	xor	dx,dx
	neg	dx
	neg	ax
	sbb	dx,0h
	push	dx
	push	ax
	call	2688h
	add	sp,8h
	mov	ax,[45B0h]
	mov	dx,[45AEh]
	cmp	ax,[bp+6h]
	jnz	22EAh

l0800_22B7:
	cmp	dx,[bp+4h]
	jnz	22EAh

l0800_22BC:
	les	di,[bp+4h]
	add	di,2h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_22C7:
	repne scasb

l0800_22C9:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_22CF:
	repne scasb

l0800_22D1:
	jz	22DAh

l0800_22D3:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_22DA:
	dec	di
	mov	ax,es
	inc	di
	mov	[45ACh],ax
	mov	[45AAh],di
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_22EA:
	mov	ax,[45AEh]
	cmp	ax,[bp+4h]
	jbe	22F9h

l0800_22F2:
	mov	ax,[bp-8h]
	sub	[45AAh],ax

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
	sub	sp,0Ah
	push	di
	les	bx,[2E51h]
	mov	al,es:[bx+4h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+5h]
	mov	dh,0h
	add	ax,dx
	mov	[bp-6h],ax
	mov	ax,[2E53h]
	mov	dx,[2E51h]
	add	dx,[bp-6h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2336:
	repne scasb

l0800_2338:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_233E:
	repne scasb

l0800_2340:
	jz	2349h

l0800_2342:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2349:
	dec	di
	mov	ax,es
	add	di,5h
	xor	ax,ax
	sub	di,[bp+4h]
	sbb	ax,0h
	mov	[bp-8h],di
	mov	ax,[bp+4h]
	add	ax,[bp-8h]
	mov	[bp-0Ah],ax
	mov	dx,[bp-4h]
	xor	bx,bx
	sub	dx,ax
	sbb	bx,0h
	push	dx
	push	word ptr [bp+6h]
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0B0F3h
	add	sp,0Ah
	mov	ax,[bp-6h]
	sub	ax,[bp-8h]
	mov	cl,8h
	shr	ax,cl
	les	bx,[2E51h]
	mov	es:[bx+4h],al
	mov	al,[bp-6h]
	sub	al,[bp-8h]
	mov	es:[bx+5h],al
	xor	ax,ax
	push	ax
	push	ax
	mov	ax,[bp-8h]
	xor	dx,dx
	neg	dx
	neg	ax
	sbb	dx,0h
	push	dx
	push	ax
	call	2688h
	add	sp,8h
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	add	dx,[bp-8h]
	mov	[45ACh],ax
	mov	[45AAh],dx
	or	dx,ax
	jz	23D8h

l0800_23C6:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	mov	[45ACh],ax
	mov	[45AAh],dx
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_23D8:
	mov	ax,[45AAh]
	cmp	ax,[bp+4h]
	jbe	23E7h

l0800_23E0:
	mov	ax,[bp-8h]
	sub	[45AAh],ax

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
	sub	sp,8h
	push	di
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	24FEh
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_240D:
	repne scasb

l0800_240F:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2415:
	repne scasb

l0800_2417:
	jz	2420h

l0800_2419:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2420:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+1h]
	cbw
	mov	cl,8h
	shl	ax,cl
	les	di,[bp+4h]
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2437:
	repne scasb

l0800_2439:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_243F:
	repne scasb

l0800_2441:
	jz	244Ah

l0800_2443:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_244A:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+2h]
	mov	ah,0h
	pop	dx
	add	dx,ax
	xor	ax,ax
	les	di,[bp+4h]
	push	dx
	push	ax
	mov	cx,0FFFFh

l0800_2462:
	repne scasb

l0800_2464:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_246A:
	repne scasb

l0800_246C:
	jz	2475h

l0800_246E:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2475:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+3h]
	cbw
	mov	cl,8h
	shl	ax,cl
	les	di,[bp+4h]
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_248C:
	repne scasb

l0800_248E:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2494:
	repne scasb

l0800_2496:
	jz	249Fh

l0800_2498:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_249F:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+4h]
	mov	ah,0h
	pop	dx
	add	dx,ax
	pop	ax
	add	ax,dx
	pop	dx
	adc	dx,0h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	sub	dx,[45A6h]
	sbb	ax,[45A8h]
	push	ax
	push	dx
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3C99h
	add	sp,0Ch
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	neg	ax
	neg	dx
	sbb	ax,0h
	push	ax
	push	dx
	call	2688h
	add	sp,8h
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
	sub	sp,0Ch
	push	di
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_250D:
	repne scasb

l0800_250F:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2515:
	repne scasb

l0800_2517:
	jz	2520h

l0800_2519:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2520:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+1h]
	cbw
	mov	cl,8h
	shl	ax,cl
	les	di,[bp+4h]
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2537:
	repne scasb

l0800_2539:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_253F:
	repne scasb

l0800_2541:
	jz	254Ah

l0800_2543:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_254A:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+2h]
	mov	ah,0h
	pop	dx
	add	dx,ax
	xor	ax,ax
	les	di,[bp+4h]
	push	dx
	push	ax
	mov	cx,0FFFFh

l0800_2562:
	repne scasb

l0800_2564:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_256A:
	repne scasb

l0800_256C:
	jz	2575h

l0800_256E:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2575:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+3h]
	cbw
	mov	cl,8h
	shl	ax,cl
	les	di,[bp+4h]
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_258C:
	repne scasb

l0800_258E:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2594:
	repne scasb

l0800_2596:
	jz	259Fh

l0800_2598:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_259F:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	al,es:[di+4h]
	mov	ah,0h
	pop	dx
	add	dx,ax
	pop	ax
	add	ax,dx
	pop	dx
	adc	dx,0h
	sub	ax,[45A6h]
	sbb	dx,[45A8h]
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3E5Dh
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	mov	cl,8h
	call	8C8Ah
	cmp	dx,52h
	jnz	25FCh

l0800_25F7:
	cmp	ax,4E43h
	jz	260Bh

l0800_25FC:
	push	ds
	mov	ax,4348h
	push	ax
	mov	ax,0Ch
	push	ax
	call	0D24h
	add	sp,6h

l0800_260B:
	mov	ax,[bp-8h]
	and	ax,0FFh
	or	ax,0h
	jz	2655h

l0800_2616:
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3E5Dh
	add	sp,4h
	mov	[2A09h],dx
	mov	[2A07h],ax
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3E5Dh
	add	sp,4h
	mov	[2A05h],dx
	mov	[2A03h],ax
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	add	dx,12h
	adc	ax,0h
	mov	[bp-0Ah],ax
	mov	[bp-0Ch],dx
	jmp	267Dh

l0800_2655:
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	3E5Dh
	add	sp,4h
	mov	[2A05h],dx
	mov	[2A03h],ax
	mov	[2A09h],dx
	mov	[2A07h],ax
	add	ax,8h
	adc	dx,0h
	mov	[bp-0Ah],dx
	mov	[bp-0Ch],ax

l0800_267D:
	mov	dx,[bp-0Ah]
	mov	ax,[bp-0Ch]
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
	sub	sp,10h
	les	bx,[2E51h]
	mov	al,es:[bx+4h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+5h]
	mov	dh,0h
	add	ax,dx
	mov	dx,[2E53h]
	add	bx,ax
	mov	[bp-6h],dx
	mov	[bp-8h],bx
	mov	ax,[2E53h]
	mov	dx,[2E51h]
	add	dx,8h
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	jmp	27B3h

l0800_26C3:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	mov	[bp-0Ah],ax
	mov	[bp-0Ch],dx
	add	word ptr [bp-4h],2h

l0800_26D3:
	les	bx,[bp-4h]
	inc	word ptr [bp-4h]
	cmp	byte ptr es:[bx],0h
	jnz	26D3h

l0800_26DF:
	jmp	277Dh

l0800_26E2:
	les	bx,[bp-4h]
	inc	word ptr [bp-4h]
	cmp	byte ptr es:[bx],0h
	jnz	26E2h

l0800_26EE:
	les	bx,[bp-4h]
	mov	al,es:[bx]
	cbw
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+1h]
	mov	dh,0h
	add	ax,dx
	xor	dx,dx
	push	ax
	mov	al,es:[bx+2h]
	cbw
	shl	ax,cl
	mov	bl,es:[bx+3h]
	mov	bh,0h
	add	ax,bx
	add	dx,ax
	pop	ax
	adc	ax,0h
	mov	[bp-0Eh],ax
	mov	[bp-10h],dx
	mov	ax,[bp-0Eh]
	cmp	ax,[bp+0Ah]
	jl	2779h

l0800_2727:
	jnz	272Eh

l0800_2729:
	cmp	dx,[bp+8h]
	jc	2779h

l0800_272E:
	mov	ax,[bp-0Eh]
	mov	dx,[bp-10h]
	add	dx,[bp+4h]
	adc	ax,[bp+6h]
	cwd
	mov	cl,8h
	call	8C8Ah
	les	bx,[bp-4h]
	mov	es:[bx],al
	mov	ax,[bp-0Eh]
	mov	dx,[bp-10h]
	add	dx,[bp+4h]
	adc	ax,[bp+6h]
	cwd
	mov	es:[bx+1h],al
	mov	dx,[bp-0Eh]
	mov	ax,[bp-10h]
	add	ax,[bp+4h]
	adc	dx,[bp+6h]
	mov	cl,8h
	call	8C8Ah
	les	bx,[bp-4h]
	mov	es:[bx+2h],al
	mov	al,[bp-10h]
	add	al,[bp+4h]
	mov	es:[bx+3h],al

l0800_2779:
	add	word ptr [bp-4h],4h

l0800_277D:
	les	bx,[bp-4h]
	cmp	byte ptr es:[bx],0h
	jz	2789h

l0800_2786:
	jmp	26E2h

l0800_2789:
	inc	word ptr [bp-4h]
	mov	ax,[bp-4h]
	xor	dx,dx
	sub	ax,[2E51h]
	sbb	dx,0h
	mov	cl,8h
	call	8C8Ah
	les	bx,[bp-0Ch]
	mov	es:[bx],al
	mov	ax,[bp-4h]
	xor	dx,dx
	sub	ax,[2E51h]
	sbb	dx,0h
	mov	es:[bx+1h],al

l0800_27B3:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	cmp	ax,[bp-6h]
	jz	27C1h

l0800_27BE:
	jmp	26C3h

l0800_27C1:
	cmp	dx,[bp-8h]
	jz	27C9h

l0800_27C6:
	jmp	26C3h

l0800_27C9:
	mov	ax,[bp+8h]
	or	ax,[bp+0Ah]
	jnz	27DFh

l0800_27D1:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	add	[45A6h],dx
	adc	[45A8h],ax

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
	sub	sp,8h
	push	si
	push	di
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_284D:
	repne scasb

l0800_284F:
	not	cx
	mov	ax,3Ah
	sub	di,cx

l0800_2856:
	repne scasb

l0800_2858:
	jz	2861h

l0800_285A:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2861:
	dec	di
	mov	ax,es
	or	di,ax
	jz	288Eh

l0800_2868:
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2870:
	repne scasb

l0800_2872:
	not	cx
	mov	ax,3Ah
	sub	di,cx

l0800_2879:
	repne scasb

l0800_287B:
	jz	2884h

l0800_287D:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2884:
	dec	di
	mov	ax,es
	inc	di
	mov	[bp+6h],ax
	mov	[bp+4h],di

l0800_288E:
	les	bx,[2E51h]
	mov	al,es:[bx+4h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+5h]
	mov	dh,0h
	add	ax,dx
	mov	dx,[2E53h]
	add	bx,ax
	mov	[bp-2h],dx
	mov	[bp-4h],bx
	mov	ax,[2E53h]
	mov	dx,[2E51h]
	add	dx,8h
	mov	[bp-6h],ax
	mov	[bp-8h],dx
	jmp	2917h

l0800_28C2:
	mov	si,[bp-8h]
	add	si,2h
	push	ds
	mov	ds,[bp-6h]
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_28D4:
	repne scasb

l0800_28D6:
	not	cx
	sub	di,cx
	rep cmpsb
	jz	28E3h

l0800_28DE:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_28E3:
	pop	ds
	or	ax,ax
	jnz	28F4h

l0800_28E8:
	mov	dx,[bp-6h]
	mov	ax,[bp-8h]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_28F4:
	les	bx,[bp-8h]
	mov	al,es:[bx]
	cbw
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+1h]
	mov	dh,0h
	add	ax,dx
	mov	dx,[2E53h]
	mov	bx,[2E51h]
	add	bx,ax
	mov	[bp-6h],dx
	mov	[bp-8h],bx

l0800_2917:
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	cmp	ax,[bp-2h]
	jnz	28C2h

l0800_2922:
	cmp	dx,[bp-4h]
	jnz	28C2h

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
	les	di,[bp+8h]
	add	di,2h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2941:
	repne scasb

l0800_2943:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2949:
	repne scasb

l0800_294B:
	jz	2954h

l0800_294D:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2954:
	dec	di
	mov	ax,es
	inc	di
	mov	[bp+0Ah],ax
	mov	[bp+8h],di
	jmp	29B4h

l0800_2960:
	mov	si,[bp+8h]
	push	ds
	mov	ds,[bp+0Ah]
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_296F:
	repne scasb

l0800_2971:
	not	cx
	sub	di,cx
	rep cmpsb
	jz	297Eh

l0800_2979:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_297E:
	pop	ds
	or	ax,ax
	jnz	298Dh

l0800_2983:
	mov	dx,[bp+0Ah]
	mov	ax,[bp+8h]
	pop	di
	pop	si
	pop	bp
	ret

l0800_298D:
	les	di,[bp+8h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2995:
	repne scasb

l0800_2997:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_299D:
	repne scasb

l0800_299F:
	jz	29A8h

l0800_29A1:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_29A8:
	dec	di
	mov	ax,es
	add	di,5h
	mov	[bp+0Ah],ax
	mov	[bp+8h],di

l0800_29B4:
	les	bx,[bp+8h]
	cmp	byte ptr es:[bx],0h
	jnz	2960h

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
	sub	sp,78h
	push	si
	push	di
	mov	ax,[45AEh]
	or	ax,[45B0h]
	jnz	2A07h

l0800_29D6:
	mov	ax,[45AAh]
	or	ax,[45ACh]
	jnz	2A07h

l0800_29DF:
	push	ds
	mov	ax,4541h
	push	ax
	call	2DE2h
	add	sp,4h
	or	ax,ax
	jnz	29F6h

l0800_29EE:
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_29F6:
	mov	ax,[2E53h]
	mov	dx,[2E51h]
	add	dx,8h
	mov	[45B0h],ax
	mov	[45AEh],dx

l0800_2A07:
	push	ds
	mov	ax,4541h
	push	ax
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	call	3509h
	add	sp,8h
	push	ds
	mov	ax,4541h
	push	ax
	push	ss
	lea	ax,[bp-78h]
	push	ax
	call	35A3h
	add	sp,8h
	les	bx,[2E51h]
	mov	al,es:[bx+4h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+5h]
	mov	dh,0h
	add	ax,dx
	mov	dx,[2E53h]
	add	bx,ax
	mov	[bp-2h],dx
	mov	[bp-4h],bx
	jmp	2C53h

l0800_2A4C:
	mov	ax,[45AAh]
	or	ax,[45ACh]
	jnz	2A58h

l0800_2A55:
	jmp	2C12h

l0800_2A58:
	jmp	2B54h

l0800_2A5B:
	push	ss
	lea	ax,[bp-78h]
	push	ax
	push	word ptr [45ACh]
	push	word ptr [45AAh]
	call	4357h
	add	sp,8h
	or	ax,ax
	jnz	2A75h

l0800_2A72:
	jmp	2B2Bh

l0800_2A75:
	les	bx,[bp+4h]
	mov	ax,[45B0h]
	mov	dx,[45AEh]
	mov	es:[bx+2h],ax
	mov	es:[bx],dx
	les	bx,[bp+8h]
	mov	ax,[45ACh]
	mov	dx,[45AAh]
	mov	es:[bx+2h],ax
	mov	es:[bx],dx
	les	di,[45AEh]
	add	di,2h
	push	es
	mov	es,[bp+0Eh]
	push	di
	mov	di,[bp+0Ch]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2AB3:
	repne scasb

l0800_2AB5:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	les	di,[bp+0Ch]
	push	es
	mov	es,[45ACh]
	push	di
	mov	di,[45AAh]
	xor	ax,ax
	mov	cx,0FFFFh

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
	mov	cx,0FFFFh
	xor	ax,ax

l0800_2AF3:
	repne scasb

l0800_2AF5:
	dec	di
	pop	cx
	rep movsb
	mov	ds,dx
	les	di,[45AAh]
	mov	cx,0FFFFh

l0800_2B02:
	repne scasb

l0800_2B04:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2B0A:
	repne scasb

l0800_2B0C:
	jz	2B15h

l0800_2B0E:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2B15:
	dec	di
	mov	ax,es
	add	di,5h
	mov	[45ACh],ax
	mov	[45AAh],di
	mov	ax,1h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_2B2B:
	les	di,[45AAh]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2B34:
	repne scasb

l0800_2B36:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2B3C:
	repne scasb

l0800_2B3E:
	jz	2B47h

l0800_2B40:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2B47:
	dec	di
	mov	ax,es
	add	di,5h
	mov	[45ACh],ax
	mov	[45AAh],di

l0800_2B54:
	les	bx,[45AAh]
	cmp	byte ptr es:[bx],0h
	jz	2B61h

l0800_2B5E:
	jmp	2A5Bh

l0800_2B61:
	les	bx,[45AEh]
	mov	al,es:[bx]
	cbw
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+1h]
	mov	dh,0h
	add	ax,dx
	mov	dx,[2E53h]
	mov	bx,[2E51h]
	add	bx,ax
	mov	[45B0h],dx
	mov	[45AEh],bx
	jmp	2C12h

l0800_2B8A:
	cmp	word ptr [2A1Bh],0h
	jz	2BBBh

l0800_2B91:
	push	ss
	pop	es
	lea	di,[bp-6Ah]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2B9B:
	repne scasb

l0800_2B9D:
	not	cx
	dec	cx
	push	cx
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	mov	ax,[45AEh]
	add	ax,2h
	push	word ptr [45B0h]
	push	ax
	call	0BFE6h
	add	sp,0Ah
	or	ax,ax
	jz	2C29h

l0800_2BBB:
	cmp	word ptr [2A1Bh],0h
	jnz	2BECh

l0800_2BC2:
	mov	si,[45AEh]
	add	si,2h
	push	ds
	mov	ds,[45B0h]
	push	ss
	pop	es
	lea	di,[bp-6Ah]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2BD8:
	repne scasb

l0800_2BDA:
	not	cx
	sub	di,cx
	rep cmpsb
	jz	2BE7h

l0800_2BE2:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_2BE7:
	pop	ds
	or	ax,ax
	jz	2C29h

l0800_2BEC:
	les	bx,[45AEh]
	mov	al,es:[bx]
	cbw
	mov	cl,8h
	shl	ax,cl
	mov	dl,es:[bx+1h]
	mov	dh,0h
	add	ax,dx
	mov	dx,[2E53h]
	mov	bx,[2E51h]
	add	bx,ax
	mov	[45B0h],dx
	mov	[45AEh],bx

l0800_2C12:
	mov	ax,[45B0h]
	mov	dx,[45AEh]
	cmp	ax,[bp-2h]
	jz	2C21h

l0800_2C1E:
	jmp	2B8Ah

l0800_2C21:
	cmp	dx,[bp-4h]
	jz	2C29h

l0800_2C26:
	jmp	2B8Ah

l0800_2C29:
	les	di,[45AEh]
	add	di,2h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2C35:
	repne scasb

l0800_2C37:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_2C3D:
	repne scasb

l0800_2C3F:
	jz	2C48h

l0800_2C41:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2C48:
	dec	di
	mov	ax,es
	inc	di
	mov	[45ACh],ax
	mov	[45AAh],di

l0800_2C53:
	mov	ax,[45B0h]
	mov	dx,[45AEh]
	cmp	ax,[bp-2h]
	jz	2C62h

l0800_2C5F:
	jmp	2A4Ch

l0800_2C62:
	cmp	dx,[bp-4h]
	jz	2C6Ah

l0800_2C67:
	jmp	2A4Ch

l0800_2C6A:
	xor	ax,ax
	xor	dx,dx
	mov	[45ACh],ax
	mov	[45AAh],dx
	mov	[45B0h],ax
	mov	[45AEh],dx
	push	word ptr [bp+0Eh]
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	29C5h
	add	sp,0Ch
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
	mov	si,2A29h

l0800_2CA0:
	mov	dx,cx
	mov	bx,8h
	jmp	2CBDh

l0800_2CA7:
	test	dx,1h
	jz	2CB6h

l0800_2CAD:
	mov	ax,dx
	shr	ax,1h
	xor	ax,0A001h
	jmp	2CBAh

l0800_2CB6:
	mov	ax,dx
	shr	ax,1h

l0800_2CBA:
	mov	dx,ax
	dec	bx

l0800_2CBD:
	or	bx,bx
	jnz	2CA7h

l0800_2CC1:
	mov	[si],dx
	add	si,2h
	inc	cx
	cmp	cx,100h
	jc	2CA0h

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
	mov	si,[bp+8h]
	jmp	2CFDh

l0800_2CD8:
	les	bx,[bp+4h]
	mov	al,[bp+0Ah]
	xor	al,es:[bx]
	mov	ah,0h
	and	ax,0FFh
	shl	ax,1h
	mov	bx,ax
	mov	ax,[bx+2A29h]
	mov	dx,[bp+0Ah]
	mov	cl,8h
	shr	dx,cl
	xor	ax,dx
	mov	[bp+0Ah],ax
	inc	word ptr [bp+4h]

l0800_2CFD:
	mov	ax,si
	dec	si
	or	ax,ax
	jnz	2CD8h

l0800_2D04:
	mov	ax,[bp+0Ah]
	pop	si
	pop	bp
	ret

;; fn0800_2D0A: 0800:2D0A
;;   Called from:
;;     0800:5444 (in fn0800_5374)
fn0800_2D0A proc
	push	bp
	mov	bp,sp
	sub	sp,0Ch
	push	si
	xor	ax,ax
	mov	dx,0FFF0h
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-0Ah],dx
	mov	[bp-0Ch],ax
	xor	si,si
	jmp	2D8Fh

l0800_2D3A:
	cmp	word ptr [bp+0Ah],0h
	jc	2D4Fh

l0800_2D40:
	ja	2D48h

l0800_2D42:
	cmp	word ptr [bp+8h],0F0h
	jbe	2D4Fh

l0800_2D48:
	xor	dx,dx
	mov	ax,0FFF0h
	jmp	2D55h

l0800_2D4F:
	mov	dx,[bp+0Ah]
	mov	ax,[bp+8h]

l0800_2D55:
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	word ptr [bp-6h]
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	4110h
	add	sp,0Ch
	push	si
	push	word ptr [bp-8h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	2CCFh
	add	sp,8h
	mov	si,ax
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	sub	[bp+8h],dx
	sbb	[bp+0Ah],ax

l0800_2D8F:
	mov	ax,[bp+8h]
	or	ax,[bp+0Ah]
	jnz	2D3Ah

l0800_2D97:
	xor	ax,ax
	push	ax
	push	word ptr [bp-0Ah]
	push	word ptr [bp-0Ch]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	4346h
	add	sp,4h
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
	mov	word ptr [4656h],0FFFFh
	mov	word ptr [4654h],0h
	mov	ax,[2A27h]
	mov	[4652h],ax
	mov	ax,[bp+4h]
	mov	[4650h],ax
	mov	ax,[bp+6h]
	mov	[464Eh],ax
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
	sub	sp,10h
	push	si
	push	di
	cmp	word ptr [4654h],0h
	jnz	2E11h

l0800_2DF1:
	mov	ax,[4652h]
	cmp	ax,[4656h]
	jnz	2E11h

l0800_2DFA:
	cmp	word ptr [464Eh],0h
	jz	2E11h

l0800_2E01:
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10h]
	push	ax
	call	3479h
	add	sp,6h
	mov	si,ax

l0800_2E11:
	cmp	word ptr [4654h],0h
	jnz	2E1Bh

l0800_2E18:
	jmp	2F51h

l0800_2E1B:
	mov	word ptr [4617h],0h
	push	word ptr [461Dh]
	push	word ptr [461Bh]
	mov	ax,65h
	push	ax
	push	ds
	mov	ax,45B2h
	push	ax
	call	0A77Dh
	add	sp,0Ah
	or	ax,dx
	jnz	2E3Fh

l0800_2E3C:
	jmp	2F1Bh

l0800_2E3F:
	push	ds
	pop	es
	mov	di,45B2h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2E49:
	repne scasb

l0800_2E4B:
	not	cx
	mov	ax,0Dh
	sub	di,cx

l0800_2E52:
	repne scasb

l0800_2E54:
	jz	2E5Dh

l0800_2E56:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2E5D:
	dec	di
	mov	ax,es
	or	di,ax
	jz	2E8Bh

l0800_2E64:
	push	ds
	pop	es
	mov	di,45B2h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2E6E:
	repne scasb

l0800_2E70:
	not	cx
	mov	ax,0Dh
	sub	di,cx

l0800_2E77:
	repne scasb

l0800_2E79:
	jz	2E82h

l0800_2E7B:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_2E82:
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di],0h

l0800_2E8B:
	mov	al,[45B2h]
	cbw
	or	ax,ax
	jnz	2EA5h

l0800_2E93:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	2DE2h
	add	sp,4h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_2EA5:
	push	ds
	pop	es
	mov	di,45B2h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2EAF:
	repne scasb

l0800_2EB1:
	not	cx
	dec	cx
	mov	[4619h],cx
	push	ds
	mov	ax,45B2h
	push	ax
	call	0C6Ch
	add	sp,4h
	cmp	word ptr [464Eh],0h
	jnz	2EFEh

l0800_2ECA:
	les	di,[bp+4h]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,45B2h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2ED9:
	repne scasb

l0800_2EDB:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	inc	word ptr [4617h]
	mov	ax,1h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_2EFE:
	push	ds
	mov	ax,45B2h
	push	ax
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10h]
	push	ax
	call	33CDh
	add	sp,0Ah
	mov	si,ax
	mov	word ptr [4654h],0h
	jmp	2F51h

l0800_2F1B:
	push	word ptr [461Dh]
	push	word ptr [461Bh]
	call	0A614h
	add	sp,4h
	mov	word ptr [4654h],0h
	mov	word ptr [461Dh],0h
	mov	word ptr [461Bh],0h
	inc	word ptr [4652h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	2DE2h
	add	sp,4h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_2F51:
	mov	ax,[4652h]
	cmp	ax,[4656h]
	jnz	2F5Dh

l0800_2F5A:
	jmp	3066h

l0800_2F5D:
	cmp	ax,[269Ah]
	jnz	2F6Bh

l0800_2F63:
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_2F6B:
	mov	ax,[4652h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	push	word ptr es:[bx+2h]
	push	word ptr es:[bx]
	push	ds
	mov	ax,45B2h
	push	ax
	call	0BF9Eh
	add	sp,8h
	push	ds
	pop	es
	mov	di,45B2h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_2F94:
	repne scasb

l0800_2F96:
	not	cx
	dec	cx
	mov	[4619h],cx
	push	ds
	mov	ax,45B2h
	push	ax
	call	0C6Ch
	add	sp,4h
	mov	ax,[4652h]
	mov	[4656h],ax
	mov	word ptr [4617h],0h
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	les	bx,es:[bx]
	cmp	byte ptr es:[bx],40h
	jnz	3007h

l0800_2FC7:
	push	ds
	mov	ax,0A13h
	push	ax
	mov	ax,[4652h]
	shl	ax,1h
	shl	ax,1h
	les	bx,[269Ch]
	add	bx,ax
	mov	ax,es:[bx]
	inc	ax
	push	word ptr es:[bx+2h]
	push	ax
	call	4234h
	add	sp,8h
	mov	[461Dh],dx
	mov	[461Bh],ax
	mov	word ptr [4654h],1h
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	2DE2h
	add	sp,4h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_3007:
	push	ds
	mov	ax,45B2h
	push	ax
	call	335Ch
	add	sp,4h
	cmp	word ptr [464Eh],0h
	jnz	3051h

l0800_3019:
	les	di,[bp+4h]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,45B2h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_3028:
	repne scasb

l0800_302A:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	inc	word ptr [4617h]
	inc	word ptr [4652h]
	mov	ax,1h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_3051:
	push	ds
	mov	ax,45B2h
	push	ax
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10h]
	push	ax
	call	33CDh
	add	sp,0Ah
	mov	si,ax

l0800_3066:
	or	si,si
	jnz	306Dh

l0800_306A:
	jmp	30F0h

l0800_306D:
	cmp	word ptr [2A1Bh],0h
	jz	309Eh

l0800_3074:
	jmp	309Ah

l0800_3076:
	push	ds
	mov	ax,45B2h
	push	ax
	call	31B4h
	add	sp,4h
	or	ax,ax
	jz	309Eh

l0800_3085:
	push	ds
	mov	ax,45B2h
	push	ax
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10h]
	push	ax
	call	33CDh
	add	sp,0Ah
	mov	si,ax

l0800_309A:
	or	si,si
	jnz	3076h

l0800_309E:
	or	si,si
	jz	30F0h

l0800_30A2:
	mov	bx,[4619h]
	mov	byte ptr [bx+45B2h],0h
	cmp	word ptr [4617h],0h
	jnz	30C9h

l0800_30B2:
	cmp	word ptr [4650h],0h
	jz	30C9h

l0800_30B9:
	push	ds
	mov	ax,45B2h
	push	ax
	push	ds
	mov	ax,0A17h
	push	ax
	call	0B2EFh
	add	sp,8h

l0800_30C9:
	mov	ax,[461Bh]
	or	ax,[461Dh]
	jz	30DAh

l0800_30D2:
	mov	word ptr [4654h],1h
	jmp	30DEh

l0800_30DA:
	inc	word ptr [4652h]

l0800_30DE:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	2DE2h
	add	sp,4h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_30F0:
	push	ds
	mov	ax,45B2h
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3509h
	add	sp,8h
	push	ss
	pop	es
	lea	di,[bp-10h]
	push	es
	mov	es,[bp+6h]
	push	di
	mov	di,[bp+4h]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_311B:
	repne scasb

l0800_311D:
	not	cx
	sub	di,cx
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	mov	[bp-2h],ds
	mov	ds,ax
	push	cx
	mov	cx,0FFFFh
	xor	ax,ax

l0800_3132:
	repne scasb

l0800_3134:
	dec	di
	pop	cx
	rep movsb
	mov	ds,[bp-2h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0C6Ch
	add	sp,4h
	mov	si,[bp+4h]
	mov	cx,[bp+6h]
	push	ds
	pop	es
	mov	di,4348h
	push	ds
	mov	ds,cx
	xor	ax,ax
	mov	cx,0FFFFh

l0800_315A:
	repne scasb

l0800_315C:
	not	cx
	sub	di,cx
	rep cmpsb
	jz	3169h

l0800_3164:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_3169:
	pop	ds
	or	ax,ax
	jz	3195h

l0800_316E:
	mov	si,[bp+4h]
	mov	cx,[bp+6h]
	push	ds
	pop	es
	mov	di,4477h
	push	ds
	mov	ds,cx
	xor	ax,ax
	mov	cx,0FFFFh

l0800_3181:
	repne scasb

l0800_3183:
	not	cx
	sub	di,cx
	rep cmpsb
	jz	3190h

l0800_318B:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_3190:
	pop	ds
	or	ax,ax
	jnz	31A7h

l0800_3195:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	2DE2h
	add	sp,4h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_31A7:
	inc	word ptr [4617h]
	mov	ax,1h
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
	sub	sp,82h
	push	si
	push	di

l0800_31BD:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	ss
	lea	ax,[bp+0FF7Eh]
	push	ax
	call	3509h
	add	sp,8h
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	ss
	lea	ax,[bp-0Eh]
	push	ax
	call	35A3h
	add	sp,8h
	mov	bx,[09ACh]
	shl	bx,1h
	push	word ptr [bx+9AEh]
	push	ss
	lea	ax,[bp+0FF7Eh]
	push	ax
	push	ss
	lea	ax,[bp-1Ch]
	push	ax
	call	32CDh
	add	sp,0Ah
	or	ax,ax
	jnz	323Eh

l0800_31FF:
	push	ss
	lea	ax,[bp-0Eh]
	push	ax
	push	ds
	mov	ax,0A30h
	push	ax
	push	ss
	lea	ax,[bp-1Ch]
	push	ax
	push	ss
	lea	ax,[bp+0FF7Eh]
	push	ax
	push	ds
	mov	ax,0A27h
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0BEA2h
	add	sp,18h
	inc	word ptr [09ACh]
	mov	bx,[09ACh]
	shl	bx,1h
	mov	word ptr [bx+9AEh],0h
	mov	ax,1h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_323E:
	cmp	word ptr [09ACh],0h
	jnz	3253h

l0800_3245:
	mov	word ptr [09AEh],0h
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_3253:
	push	ss
	pop	es
	lea	di,[bp+0FF7Eh]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_325E:
	repne scasb

l0800_3260:
	not	cx
	dec	cx
	dec	cx
	lea	ax,[bp+0FF7Eh]
	add	cx,ax
	mov	bx,cx
	mov	byte ptr ss:[bx],0h
	push	ss
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3509h
	add	sp,8h
	push	ss
	pop	es
	lea	di,[bp-0Eh]
	push	es
	mov	es,[bp+6h]
	push	di
	mov	di,[bp+4h]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

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
	mov	cx,0FFFFh
	xor	ax,ax

l0800_32AE:
	repne scasb

l0800_32B0:
	dec	di
	pop	cx
	rep movsb
	mov	ds,dx
	dec	word ptr [09ACh]
	mov	bx,[09ACh]
	shl	bx,1h
	inc	word ptr [bx+9AEh]
	jmp	31BDh
0800:32C7                      5F 5E 8B E5 5D C3                 _^..].   

;; fn0800_32CD: 0800:32CD
;;   Called from:
;;     0800:31F5 (in fn0800_31B4)
fn0800_32CD proc
	push	bp
	mov	bp,sp
	sub	sp,66h
	push	si
	push	di
	push	ss
	pop	es
	lea	di,[bp-66h]
	push	es
	mov	es,[bp+0Ah]
	push	di
	mov	di,[bp+8h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_32E7:
	repne scasb

l0800_32E9:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	push	ss
	pop	es
	lea	di,[bp-66h]
	mov	si,0A32h
	mov	cx,0FFFFh
	xor	ax,ax

l0800_330C:
	repne scasb

l0800_330E:
	dec	di
	mov	cx,4h
	rep movsb
	push	ss
	lea	ax,[bp-66h]
	push	ax
	mov	ax,10h
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	33CDh
	add	sp,0Ah
	or	ax,ax
	jz	334Dh

l0800_332D:
	mov	ax,1h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_3336:
	mov	ax,10h
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3479h
	add	sp,6h
	or	ax,ax
	jnz	3353h

l0800_334A:
	dec	word ptr [bp+0Ch]

l0800_334D:
	cmp	word ptr [bp+0Ch],0h
	jnz	3336h

l0800_3353:
	mov	ax,[bp+0Ch]
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
	sub	sp,1Ch
	push	si
	push	di
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	mov	ax,10h
	push	ax
	push	ss
	lea	ax,[bp-0Eh]
	push	ax
	call	33CDh
	add	sp,0Ah
	or	ax,ax
	jnz	33C7h

l0800_337D:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	ss
	lea	ax,[bp-1Ch]
	push	ax
	call	35A3h
	add	sp,8h
	mov	ax,ss
	lea	si,[bp-0Eh]
	push	ds
	mov	ds,ax
	push	ss
	pop	es
	lea	di,[bp-1Ch]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_33A0:
	repne scasb

l0800_33A2:
	not	cx
	sub	di,cx
	rep cmpsb
	jz	33AFh

l0800_33AA:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_33AF:
	pop	ds
	or	ax,ax
	jnz	33C7h

l0800_33B4:
	les	di,[bp+4h]
	mov	si,0A36h
	mov	cx,0FFFFh
	xor	ax,ax

l0800_33BF:
	repne scasb

l0800_33C1:
	dec	di
	mov	cx,5h
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
	push	word ptr [bp+8h]
	push	ds
	mov	ax,4623h
	push	ax
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	call	0A817h
	add	sp,0Ah
	or	ax,ax
	jz	33F1h

l0800_33EA:
	mov	ax,1h
	pop	di
	pop	si
	pop	bp
	ret

l0800_33F1:
	push	ds
	pop	es
	mov	di,0A3Bh
	mov	si,4641h
	mov	cx,2h
	xor	ax,ax
	rep cmpsb
	jz	3407h

l0800_3402:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_3407:
	or	ax,ax
	jz	3431h

l0800_340B:
	push	ds
	pop	es
	mov	di,0A3Dh
	mov	si,4641h
	mov	cx,3h
	xor	ax,ax
	rep cmpsb
	jz	3421h

l0800_341C:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_3421:
	or	ax,ax
	jz	3431h

l0800_3425:
	mov	al,[4638h]
	cbw
	and	ax,[bp+8h]
	cmp	ax,[bp+8h]
	jz	3444h

l0800_3431:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3479h
	add	sp,6h
	pop	di
	pop	si
	pop	bp
	ret

l0800_3444:
	mov	ax,[bp+4h]
	or	ax,[bp+6h]
	jz	3473h

l0800_344C:
	les	di,[bp+4h]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,4641h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_345B:
	repne scasb

l0800_345D:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
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
	mov	ax,4623h
	push	ax
	call	0A84Ah
	add	sp,4h
	or	ax,ax
	jz	3494h

l0800_348D:
	mov	ax,1h
	pop	di
	pop	si
	pop	bp
	ret

l0800_3494:
	push	ds
	pop	es
	mov	di,0A3Bh
	mov	si,4641h
	mov	cx,2h
	xor	ax,ax
	rep cmpsb
	jz	34AAh

l0800_34A5:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_34AA:
	or	ax,ax
	jz	347Eh

l0800_34AE:
	push	ds
	pop	es
	mov	di,0A3Dh
	mov	si,4641h
	mov	cx,3h
	xor	ax,ax
	rep cmpsb
	jz	34C4h

l0800_34BF:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_34C4:
	or	ax,ax
	jz	347Eh

l0800_34C8:
	mov	al,[4638h]
	cbw
	and	ax,[bp+8h]
	cmp	ax,[bp+8h]
	jnz	347Eh

l0800_34D4:
	mov	ax,[bp+4h]
	or	ax,[bp+6h]
	jz	3503h

l0800_34DC:
	les	di,[bp+4h]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,4641h
	xor	ax,ax
	mov	cx,0FFFFh

l0800_34EB:
	repne scasb

l0800_34ED:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
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
	sub	sp,58h
	push	si
	push	di
	push	ss
	lea	ax,[bp-14h]
	push	ax
	push	ss
	lea	ax,[bp-0Eh]
	push	ax
	push	ss
	lea	ax,[bp-58h]
	push	ax
	push	ss
	lea	ax,[bp-4h]
	push	ax
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	0BE3Bh
	add	sp,14h
	push	ss
	pop	es
	lea	di,[bp-4h]
	push	es
	mov	es,[bp+6h]
	push	di
	mov	di,[bp+4h]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_354B:
	repne scasb

l0800_354D:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	push	ss
	pop	es
	lea	di,[bp-58h]
	push	es
	mov	es,[bp+6h]
	push	di
	mov	di,[bp+4h]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_357D:
	repne scasb

l0800_357F:
	not	cx
	sub	di,cx
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	mov	[bp-16h],ds
	mov	ds,ax
	push	cx
	mov	cx,0FFFFh
	xor	ax,ax

l0800_3594:
	repne scasb

l0800_3596:
	dec	di
	pop	cx
	rep movsb
	mov	ds,[bp-16h]
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
	sub	sp,58h
	push	si
	push	di
	push	ss
	lea	ax,[bp-14h]
	push	ax
	push	ss
	lea	ax,[bp-0Eh]
	push	ax
	push	ss
	lea	ax,[bp-58h]
	push	ax
	push	ss
	lea	ax,[bp-4h]
	push	ax
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	0BE3Bh
	add	sp,14h
	push	ss
	pop	es
	lea	di,[bp-0Eh]
	push	es
	mov	es,[bp+6h]
	push	di
	mov	di,[bp+4h]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_35E5:
	repne scasb

l0800_35E7:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	push	ss
	pop	es
	lea	di,[bp-14h]
	push	es
	mov	es,[bp+6h]
	push	di
	mov	di,[bp+4h]
	mov	ax,di
	pop	di
	mov	dx,es
	pop	es
	push	dx
	push	ax
	xor	ax,ax
	mov	cx,0FFFFh

l0800_3617:
	repne scasb

l0800_3619:
	not	cx
	sub	di,cx
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	mov	[bp-16h],ds
	mov	ds,ax
	push	cx
	mov	cx,0FFFFh
	xor	ax,ax

l0800_362E:
	repne scasb

l0800_3630:
	dec	di
	pop	cx
	rep movsb
	mov	ds,[bp-16h]
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
	sub	sp,2Ch
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-2Ch]
	push	ax
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	0A817h
	add	sp,0Ah
	or	ax,ax
	jz	3662h

l0800_365B:
	mov	ax,1h
	mov	sp,bp
	pop	bp
	ret

l0800_3662:
	les	bx,[bp+4h]
	mov	ax,[bp-16h]
	mov	es:[bx],ax
	mov	ax,[bp-14h]
	mov	es:[bx+2h],ax
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
	sub	sp,8h
	push	di
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	les	di,[bp+4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_3693:
	repne scasb

l0800_3695:
	not	cx
	mov	ax,5Ch
	sub	di,cx

l0800_369C:
	repne scasb

l0800_369E:
	jz	36A7h

l0800_36A0:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_36A7:
	dec	di
	mov	ax,es
	mov	[bp-6h],ax
	mov	[bp-8h],di
	mov	dx,di
	or	dx,ax
	jz	3732h

l0800_36B6:
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	cmp	ax,[bp+6h]
	jnz	36C6h

l0800_36C1:
	cmp	dx,[bp+4h]
	jz	36D0h

l0800_36C6:
	les	bx,[bp-8h]
	cmp	byte ptr es:[bx-1h],3Ah
	jnz	3732h

l0800_36D0:
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	inc	dx
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	jmp	3732h

l0800_36DF:
	les	bx,[bp-8h]
	mov	byte ptr es:[bx],0h
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	mov	ax,10h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	call	33CDh
	add	sp,0Ah
	or	ax,ax
	jz	371Eh

l0800_36FE:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8E52h
	add	sp,4h
	or	ax,ax
	jz	371Eh

l0800_370E:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	mov	ax,0Eh
	push	ax
	call	0D24h
	add	sp,6h

l0800_371E:
	les	bx,[bp-8h]
	mov	byte ptr es:[bx],5Ch
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	inc	dx
	mov	[bp-2h],ax
	mov	[bp-4h],dx

l0800_3732:
	les	di,[bp-4h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_373A:
	repne scasb

l0800_373C:
	not	cx
	mov	ax,5Ch
	sub	di,cx

l0800_3743:
	repne scasb

l0800_3745:
	jz	374Eh

l0800_3747:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_374E:
	dec	di
	mov	ax,es
	mov	[bp-6h],ax
	mov	[bp-8h],di
	mov	dx,di
	or	dx,ax
	jnz	36DFh

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
	cmp	byte ptr [427Eh],0h
	jz	3796h

l0800_376D:
	push	ds
	pop	es
	mov	di,44DCh
	push	es
	push	di
	mov	di,427Eh
	xor	ax,ax
	mov	cx,0FFFFh

l0800_377C:
	repne scasb

l0800_377E:
	not	cx
	sub	di,cx
	shr	cx,1h
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep movsw
	adc	cx,cx
	rep movsb
	pop	ds
	jmp	37A6h

l0800_3796:
	push	ds
	mov	ax,4541h
	push	ax
	push	ds
	mov	ax,44DCh
	push	ax
	call	3509h
	add	sp,8h

l0800_37A6:
	push	ds
	pop	es
	mov	di,44DCh
	mov	si,0A40h
	mov	cx,0FFFFh
	xor	ax,ax

l0800_37B3:
	repne scasb

l0800_37B5:
	dec	di
	mov	cx,0Dh
	rep movsb
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
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	ds
	mov	ax,42E3h
	push	ax
	push	ds
	mov	ax,0A2Bh
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0BEA2h
	add	sp,10h
	pop	bp
	ret

;; fn0800_37DF: 0800:37DF
;;   Called from:
;;     0800:10E5 (in fn0800_0DE8)
;;     0800:12B3 (in fn0800_112D)
fn0800_37DF proc
	push	bp
	mov	bp,sp
	sub	sp,6Ah
	push	di
	push	ds
	mov	ax,44DCh
	push	ax
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	call	3509h
	add	sp,8h
	push	ds
	mov	ax,4541h
	push	ax
	push	ss
	pop	es
	lea	di,[bp-6Ah]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_3805:
	repne scasb

l0800_3807:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_380D:
	repne scasb

l0800_380F:
	jz	3818h

l0800_3811:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_3818:
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	35A3h
	add	sp,8h
	push	ds
	mov	ax,0A4Dh
	push	ax
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	call	0AA7Eh
	add	sp,8h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	or	ax,dx
	jz	3863h

l0800_383D:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0A614h
	add	sp,4h
	mov	ax,180h
	push	ax
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	call	0A4F6h
	add	sp,6h
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	call	8F7Fh
	add	sp,4h

l0800_3863:
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	push	ds
	mov	ax,44DCh
	push	ax
	call	0BA4Ah
	add	sp,8h
	cmp	ax,0FFFFh
	jnz	3887h

l0800_3878:
	push	ds
	mov	ax,44DCh
	push	ax
	mov	ax,0Ah
	push	ax
	call	0D24h
	add	sp,6h

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
	sub	sp,8h
	push	ds
	mov	ax,0A13h
	push	ax
	push	ds
	mov	ax,4541h
	push	ax
	call	4234h
	add	sp,8h
	mov	[29E5h],dx
	mov	[29E3h],ax
	push	ds
	mov	ax,0A50h
	push	ax
	push	ds
	mov	ax,44DCh
	push	ax
	call	4234h
	add	sp,8h
	mov	[29E1h],dx
	mov	[29DFh],ax
	push	ds
	mov	ax,44DCh
	push	ax
	push	ss
	lea	ax,[bp-4h]
	push	ax
	call	363Dh
	add	sp,8h
	cmp	byte ptr [0A12h],0h
	jnz	38EFh

l0800_38D7:
	mov	ax,[bp-2h]
	mov	[4621h],ax
	mov	ax,[bp-4h]
	mov	[461Fh],ax
	mov	byte ptr [0A12h],1h
	mov	ax,1h
	mov	sp,bp
	pop	bp
	ret

l0800_38EF:
	push	ds
	mov	ax,4541h
	push	ax
	push	ss
	lea	ax,[bp-8h]
	push	ax
	call	363Dh
	add	sp,8h
	push	ds
	mov	ax,461Fh
	push	ax
	push	ss
	lea	ax,[bp-8h]
	push	ax
	call	395Bh
	add	sp,8h
	or	ax,ax
	jl	3927h

l0800_3913:
	push	ss
	lea	ax,[bp-4h]
	push	ax
	push	ss
	lea	ax,[bp-8h]
	push	ax
	call	395Bh
	add	sp,8h
	or	ax,ax
	jle	392Eh

l0800_3927:
	mov	ax,1h
	mov	sp,bp
	pop	bp
	ret

l0800_392E:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0A614h
	add	sp,4h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0A614h
	add	sp,4h
	push	ds
	mov	ax,44DCh
	push	ax
	call	8F7Fh
	add	sp,4h
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
	sub	sp,2h
	les	bx,[bp+4h]
	mov	ax,es:[bx+2h]
	mov	[bp-2h],ax
	les	bx,[bp+8h]
	cmp	ax,es:[bx+2h]
	jnz	3984h

l0800_3974:
	les	bx,[bp+4h]
	mov	ax,es:[bx]
	les	bx,[bp+8h]
	sub	ax,es:[bx]
	mov	sp,bp
	pop	bp
	ret

l0800_3984:
	mov	ax,[bp-2h]
	les	bx,[bp+8h]
	sub	ax,es:[bx+2h]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_3992: 0800:3992
;;   Called from:
;;     0800:0ECF (in fn0800_0DE8)
fn0800_3992 proc
	push	bp
	mov	bp,sp
	sub	sp,72h
	push	di
	cmp	byte ptr [427Eh],0h
	jz	39AEh

l0800_39A0:
	cmp	word ptr [2A23h],1h
	jnz	39AEh

l0800_39A7:
	cmp	word ptr [2A17h],0h
	jnz	39B5h

l0800_39AE:
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_39B5:
	push	ds
	mov	ax,44DCh
	push	ax
	push	ss
	lea	ax,[bp-72h]
	push	ax
	call	3509h
	add	sp,8h
	push	ds
	mov	ax,4541h
	push	ax
	push	ss
	pop	es
	lea	di,[bp-72h]
	xor	ax,ax
	mov	cx,0FFFFh

l0800_39D4:
	repne scasb

l0800_39D6:
	not	cx
	xor	ax,ax
	sub	di,cx

l0800_39DC:
	repne scasb

l0800_39DE:
	jz	39E7h

l0800_39E0:
	mov	di,1h
	xor	ax,ax
	mov	es,ax

l0800_39E7:
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	35A3h
	add	sp,8h
	push	ds
	mov	ax,0A4Dh
	push	ax
	push	ss
	lea	ax,[bp-72h]
	push	ax
	call	0AA7Eh
	add	sp,8h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	or	ax,dx
	jnz	3A13h

l0800_3A0C:
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_3A13:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	4194h
	add	sp,4h
	or	dx,dx
	ja	3A3Dh

l0800_3A23:
	jnz	3A2Ah

l0800_3A25:
	cmp	ax,12h
	ja	3A3Dh

l0800_3A2A:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0A614h
	add	sp,4h
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_3A3D:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	3E5Dh
	add	sp,4h
	mov	cl,8h
	call	8CAAh
	cmp	dx,52h
	jnz	3A58h

l0800_3A53:
	cmp	ax,4E43h
	jz	3A6Bh

l0800_3A58:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0A614h
	add	sp,4h
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_3A6B:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	3E5Dh
	add	sp,4h
	cmp	dx,[2A09h]
	jnz	3A83h

l0800_3A7D:
	cmp	ax,[2A07h]
	jz	3A96h

l0800_3A83:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0A614h
	add	sp,4h
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_3A96:
	push	ss
	lea	ax,[bp-72h]
	push	ax
	push	ss
	lea	ax,[bp-0Ch]
	push	ax
	call	363Dh
	add	sp,8h
	push	ds
	mov	ax,4541h
	push	ax
	push	ss
	lea	ax,[bp-8h]
	push	ax
	call	363Dh
	add	sp,8h
	push	ss
	lea	ax,[bp-8h]
	push	ax
	push	ss
	lea	ax,[bp-0Ch]
	push	ax
	call	395Bh
	add	sp,8h
	or	ax,ax
	jge	3ADDh

l0800_3ACA:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0A614h
	add	sp,4h
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret

l0800_3ADD:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	3E5Dh
	add	sp,4h
	add	ax,12h
	adc	dx,0h
	mov	[2A05h],dx
	mov	[2A03h],ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0A614h
	add	sp,4h
	mov	ax,1h
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
	sub	sp,8h
	mov	ax,[bp+0Ch]
	or	ax,[bp+0Eh]
	jnz	3B1Bh

l0800_3B18:
	jmp	3BBFh

l0800_3B1B:
	cmp	word ptr [bp+0Eh],0h
	jl	3B31h

l0800_3B21:
	jg	3B2Ah

l0800_3B23:
	cmp	word ptr [bp+0Ch],0FDE8h
	jbe	3B31h

l0800_3B2A:
	xor	dx,dx
	mov	ax,0FDE8h
	jmp	3B37h

l0800_3B31:
	mov	dx,[bp+0Eh]
	mov	ax,[bp+0Ch]

l0800_3B37:
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	push	word ptr [bp-6h]
	push	ax
	call	4311h
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	jmp	3BABh

l0800_3B4F:
	cmp	word ptr [bp+0Eh],0h
	jl	3B65h

l0800_3B55:
	jg	3B5Eh

l0800_3B57:
	cmp	word ptr [bp+0Ch],0FDE8h
	jbe	3B65h

l0800_3B5E:
	xor	dx,dx
	mov	ax,0FDE8h
	jmp	3B6Bh

l0800_3B65:
	mov	dx,[bp+0Eh]
	mov	ax,[bp+0Ch]

l0800_3B6B:
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	word ptr [bp-6h]
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	4110h
	add	sp,0Ch
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	4152h
	add	sp,0Ch
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	sub	[bp+0Ch],dx
	sbb	[bp+0Eh],ax

l0800_3BAB:
	mov	ax,[bp+0Ch]
	or	ax,[bp+0Eh]
	jnz	3B4Fh

l0800_3BB3:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	4346h
	add	sp,4h

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
	sub	sp,4h
	push	ds
	mov	ax,0A54h
	push	ax
	push	ds
	mov	ax,43ADh
	push	ax
	call	37BEh
	add	sp,8h
	push	ds
	mov	ax,0A61h
	push	ax
	push	ds
	mov	ax,43ADh
	push	ax
	call	4234h
	add	sp,8h
	mov	[29D5h],dx
	mov	[29D3h],ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	4194h
	add	sp,4h
	sub	ax,[bp+8h]
	sbb	dx,[bp+0Ah]
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [29D5h]
	push	word ptr [29D3h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3B0Ah
	add	sp,0Ch
	push	word ptr [29D5h]
	push	word ptr [29D3h]
	call	0BA67h
	add	sp,4h
	xor	ax,ax
	push	ax
	mov	ax,[bp+0Ah]
	mov	dx,[bp+8h]
	add	dx,[bp+0Ch]
	adc	ax,[bp+0Eh]
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	word ptr [29D5h]
	push	word ptr [29D3h]
	call	3B0Ah
	add	sp,0Ch
	push	word ptr [29D5h]
	push	word ptr [29D3h]
	call	0A614h
	add	sp,4h
	push	ds
	mov	ax,43ADh
	push	ax
	call	8F7Fh
	add	sp,4h
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
	sub	sp,6Ah
	push	si
	push	di
	push	ds
	mov	ax,4348h
	push	ax
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	call	3509h
	add	sp,8h
	push	ss
	pop	es
	lea	di,[bp-6Ah]
	mov	si,0A54h
	mov	cx,0FFFFh
	xor	ax,ax

l0800_3CBE:
	repne scasb

l0800_3CC0:
	dec	di
	mov	cx,0Dh
	rep movsb
	push	ds
	mov	ax,0A61h
	push	ax
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	call	4234h
	add	sp,8h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3B0Ah
	add	sp,0Ch
	xor	ax,ax
	push	ax
	mov	ax,[bp+0Ah]
	mov	dx,[bp+8h]
	add	dx,[bp+0Ch]
	adc	ax,[bp+0Eh]
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	4194h
	add	sp,4h
	mov	bx,[bp+0Ah]
	mov	cx,[bp+8h]
	add	cx,[bp+0Ch]
	adc	bx,[bp+0Eh]
	sub	ax,cx
	sbb	dx,bx
	mov	[bp+0Eh],dx
	mov	[bp+0Ch],ax
	push	word ptr [bp+0Eh]
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3B0Ah
	add	sp,0Ch
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0A614h
	add	sp,4h
	push	word ptr [29D1h]
	push	word ptr [29CFh]
	call	0A614h
	add	sp,4h
	mov	ax,180h
	push	ax
	push	ds
	mov	ax,4348h
	push	ax
	call	0A4F6h
	add	sp,6h
	push	ds
	mov	ax,4348h
	push	ax
	call	8F7Fh
	add	sp,4h
	push	ds
	mov	ax,4348h
	push	ax
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	call	0BA4Ah
	add	sp,8h
	cmp	ax,0FFFFh
	jnz	3DB2h

l0800_3DA3:
	push	ss
	lea	ax,[bp-6Ah]
	push	ax
	mov	ax,0Ah
	push	ax
	call	0D24h
	add	sp,6h

l0800_3DB2:
	push	ds
	mov	ax,0A65h
	push	ax
	push	ds
	mov	ax,4348h
	push	ax
	call	4234h
	add	sp,8h
	mov	[29D1h],dx
	mov	[29CFh],ax
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
	sub	sp,2h
	push	si
	les	bx,[bp+4h]
	dec	word ptr es:[bx]
	jl	3DF3h

l0800_3DDE:
	mov	ax,es:[bx+0Eh]
	mov	si,es:[bx+0Ch]
	inc	word ptr es:[bx+0Ch]
	mov	es,ax
	mov	al,es:[si]
	mov	ah,0h
	jmp	3DFFh

l0800_3DF3:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0AEC2h
	add	sp,4h

l0800_3DFF:
	mov	[bp-2h],ax
	cmp	ax,0FFFFh
	jnz	3E1Fh

l0800_3E07:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	4271h
	add	sp,4h
	push	dx
	push	ax
	mov	ax,8h
	push	ax
	call	0D24h
	add	sp,6h

l0800_3E1F:
	mov	al,[bp-2h]
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
	sub	sp,4h
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3DCFh
	add	sp,4h
	mov	ah,0h
	mov	[bp-2h],ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3DCFh
	add	sp,4h
	mov	ah,0h
	mov	[bp-4h],ax
	mov	ax,[bp-2h]
	mov	cl,8h
	shl	ax,cl
	add	ax,[bp-4h]
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
	sub	sp,8h
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3E27h
	add	sp,4h
	mov	word ptr [bp-2h],0h
	mov	[bp-4h],ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3E27h
	add	sp,4h
	mov	word ptr [bp-6h],0h
	mov	[bp-8h],ax
	mov	dx,[bp-4h]
	xor	ax,ax
	add	ax,[bp-8h]
	adc	dx,0h
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
	sub	sp,2h
	push	si
	les	bx,[bp+4h]
	dec	word ptr es:[bx]
	jl	3EBEh

l0800_3EA9:
	mov	ax,es:[bx+0Eh]
	mov	si,es:[bx+0Ch]
	inc	word ptr es:[bx+0Ch]
	mov	es,ax
	mov	al,es:[si]
	mov	ah,0h
	jmp	3ECAh

l0800_3EBE:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0AEC2h
	add	sp,4h

l0800_3ECA:
	mov	[bp-2h],ax
	cmp	ax,0FFFFh
	jnz	3EEAh

l0800_3ED2:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	4271h
	add	sp,4h
	push	dx
	push	ax
	mov	ax,8h
	push	ax
	call	0D24h
	add	sp,6h

l0800_3EEA:
	mov	ax,1h
	push	ax
	mov	ax,0FFFFh
	mov	dx,0FFFFh
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	mov	al,[bp-2h]
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
	sub	sp,4h
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3DCFh
	add	sp,4h
	mov	ah,0h
	mov	[bp-2h],ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3DCFh
	add	sp,4h
	mov	ah,0h
	mov	[bp-4h],ax
	mov	ax,1h
	push	ax
	mov	ax,0FFFFh
	mov	dx,0FFFEh
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	mov	ax,[bp-2h]
	mov	cl,8h
	shl	ax,cl
	add	ax,[bp-4h]
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
	sub	sp,8h
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3E27h
	add	sp,4h
	mov	word ptr [bp-2h],0h
	mov	[bp-4h],ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3E27h
	add	sp,4h
	mov	word ptr [bp-6h],0h
	mov	[bp-8h],ax
	mov	ax,1h
	push	ax
	mov	ax,0FFFFh
	mov	dx,0FFFCh
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	mov	dx,[bp-4h]
	xor	ax,ax
	add	ax,[bp-8h]
	adc	dx,[bp-6h]
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
	sub	sp,4h
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3DCFh
	add	sp,4h
	mov	ah,0h
	mov	[bp-2h],ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3DCFh
	add	sp,4h
	mov	ah,0h
	mov	[bp-4h],ax
	mov	cl,8h
	shl	ax,cl
	add	ax,[bp-2h]
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
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	mov	ax,[bp+4h]
	mov	cl,8h
	shr	ax,cl
	push	ax
	call	4047h
	add	sp,6h
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	mov	al,[bp+4h]
	push	ax
	call	4047h
	add	sp,6h
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
	mov	cl,[bp+4h]
	les	bx,[bp+6h]
	inc	word ptr es:[bx]
	jge	406Fh

l0800_4056:
	mov	ax,es:[bx+0Eh]
	mov	si,es:[bx+0Ch]
	inc	word ptr es:[bx+0Ch]
	mov	dl,cl
	mov	es,ax
	mov	es:[si],dl
	mov	al,dl
	mov	ah,0h
	jmp	407Ch

l0800_406F:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	cx
	call	0B30Ah
	add	sp,6h

l0800_407C:
	cmp	ax,0FFFFh
	jnz	4099h

l0800_4081:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	call	4271h
	add	sp,4h
	push	dx
	push	ax
	mov	ax,9h
	push	ax
	call	0D24h
	add	sp,6h

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
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	call	401Eh
	add	sp,6h
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+4h]
	call	401Eh
	add	sp,6h
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
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	mov	al,[bp+4h]
	and	al,0FFh
	push	ax
	call	4047h
	add	sp,6h
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	mov	ax,[bp+4h]
	mov	cl,8h
	shr	ax,cl
	and	al,0FFh
	push	ax
	call	4047h
	add	sp,6h
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
	push	word ptr [bp+0Eh]
	push	word ptr [bp+0Ch]
	push	word ptr [bp+8h]
	mov	ax,1h
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ABA3h
	add	sp,0Ch
	xor	dx,dx
	cmp	dx,[bp+0Ah]
	jnz	4138h

l0800_4133:
	cmp	ax,[bp+8h]
	jz	4150h

l0800_4138:
	push	word ptr [bp+0Eh]
	push	word ptr [bp+0Ch]
	call	4271h
	add	sp,4h
	push	dx
	push	ax
	mov	ax,8h
	push	ax
	call	0D24h
	add	sp,6h

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
	push	word ptr [bp+0Eh]
	push	word ptr [bp+0Ch]
	push	word ptr [bp+8h]
	mov	ax,1h
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0AD85h
	add	sp,0Ch
	xor	dx,dx
	cmp	dx,[bp+0Ah]
	jnz	417Ah

l0800_4175:
	cmp	ax,[bp+8h]
	jz	4192h

l0800_417A:
	push	word ptr [bp+0Eh]
	push	word ptr [bp+0Ch]
	call	4271h
	add	sp,4h
	push	dx
	push	ax
	mov	ax,9h
	push	ax
	call	0D24h
	add	sp,6h

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
	sub	sp,8h
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	mov	ax,2h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	mov	dx,[bp-6h]
	mov	ax,[bp-8h]
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
	sub	sp,4h
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0AA7Eh
	add	sp,8h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	or	ax,[bp-2h]
	jnz	4267h

l0800_4257:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	mov	ax,7h
	push	ax
	call	0D24h
	add	sp,6h

l0800_4267:
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
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
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	cmp	ax,[29E5h]
	jnz	428Dh

l0800_4280:
	cmp	dx,[29E3h]
	jnz	428Dh

l0800_4286:
	mov	dx,ds
	mov	ax,4541h
	pop	bp
	ret

l0800_428D:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	cmp	ax,[29E1h]
	jnz	42A6h

l0800_4299:
	cmp	dx,[29DFh]
	jnz	42A6h

l0800_429F:
	mov	dx,ds
	mov	ax,44DCh
	pop	bp
	ret

l0800_42A6:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	cmp	ax,[29D1h]
	jnz	42BFh

l0800_42B2:
	cmp	dx,[29CFh]
	jnz	42BFh

l0800_42B8:
	mov	dx,ds
	mov	ax,4348h
	pop	bp
	ret

l0800_42BF:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	cmp	ax,[29DDh]
	jnz	42D8h

l0800_42CB:
	cmp	dx,[29DBh]
	jnz	42D8h

l0800_42D1:
	mov	dx,ds
	mov	ax,4477h
	pop	bp
	ret

l0800_42D8:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	cmp	ax,[29D9h]
	jnz	42F1h

l0800_42E4:
	cmp	dx,[29D7h]
	jnz	42F1h

l0800_42EA:
	mov	dx,ds
	mov	ax,4412h
	pop	bp
	ret

l0800_42F1:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	cmp	ax,[29D5h]
	jnz	430Ah

l0800_42FD:
	cmp	dx,[29D3h]
	jnz	430Ah

l0800_4303:
	mov	dx,ds
	mov	ax,43ADh
	pop	bp
	ret

l0800_430A:
	mov	dx,ds
	mov	ax,0A60h
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
	sub	sp,4h
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	9F89h
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	or	ax,dx
	jnz	433Ch

l0800_432D:
	push	ds
	mov	ax,0A6Ch
	push	ax
	mov	ax,6h
	push	ax
	call	0D24h
	add	sp,6h

l0800_433C:
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
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
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	9E75h
	add	sp,4h
	pop	bp
	ret

;; fn0800_4357: 0800:4357
;;   Called from:
;;     0800:2A68 (in fn0800_29C5)
fn0800_4357 proc
	push	bp
	mov	bp,sp
	jmp	43B9h

l0800_435C:
	les	bx,[bp+8h]
	mov	al,es:[bx]
	mov	dl,al
	cbw
	cmp	ax,2Ah
	jz	4379h

l0800_436A:
	cmp	ax,2Eh
	jz	439Eh

l0800_436F:
	cmp	ax,3Fh
	jz	438Ah

l0800_4374:
	jmp	43A7h

l0800_4376:
	inc	word ptr [bp+4h]

l0800_4379:
	les	bx,[bp+4h]
	cmp	byte ptr es:[bx],2Eh
	jz	43B6h

l0800_4382:
	cmp	byte ptr es:[bx],0h
	jnz	4376h

l0800_4388:
	jmp	43B6h

l0800_438A:
	les	bx,[bp+4h]
	cmp	byte ptr es:[bx],2Eh
	jz	43B6h

l0800_4393:
	cmp	byte ptr es:[bx],0h
	jz	43B6h

l0800_4399:
	inc	word ptr [bp+4h]
	jmp	43B6h

l0800_439E:
	les	bx,[bp+4h]
	cmp	byte ptr es:[bx],0h
	jz	43B6h

l0800_43A7:
	les	bx,[bp+4h]
	cmp	dl,es:[bx]
	jz	43B3h

l0800_43AF:
	xor	ax,ax
	pop	bp
	ret

l0800_43B3:
	inc	word ptr [bp+4h]

l0800_43B6:
	inc	word ptr [bp+8h]

l0800_43B9:
	les	bx,[bp+8h]
	cmp	byte ptr es:[bx],0h
	jnz	435Ch

l0800_43C2:
	les	bx,[bp+4h]
	cmp	byte ptr es:[bx],0h
	jnz	43D0h

l0800_43CB:
	mov	ax,1h
	jmp	43D2h

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
	mov	cx,[bp+8h]
	mov	si,[bp+4h]
	mov	ax,cx
	mov	dx,0Ch
	imul	dx
	add	si,ax
	jmp	440Fh

l0800_43E9:
	mov	es,[bp+6h]
	mov	word ptr es:[si+2h],0h
	mov	word ptr es:[si],0h
	mov	word ptr es:[si+4h],0FFFFh
	mov	word ptr es:[si+8h],0h
	mov	word ptr es:[si+6h],0h
	mov	word ptr es:[si+0Ah],0h

l0800_440F:
	sub	si,0Ch
	mov	ax,cx
	dec	cx
	or	ax,ax
	jnz	43E9h

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
	sub	sp,2h
	push	si
	push	di
	xor	di,di
	mov	cx,di
	mov	si,[bp+4h]
	mov	ax,cx
	mov	dx,0Ch
	imul	dx
	add	si,ax
	cmp	cx,[bp+8h]
	jnc	4452h

l0800_4439:
	mov	es,[bp+6h]
	mov	ax,es:[si]
	or	ax,es:[si+2h]
	jz	4449h

l0800_4445:
	inc	di
	mov	[bp-2h],cx

l0800_4449:
	add	si,0Ch
	inc	cx
	cmp	cx,[bp+8h]
	jc	4439h

l0800_4452:
	or	di,di
	jnz	4459h

l0800_4456:
	jmp	454Ah

l0800_4459:
	cmp	di,1h
	jz	4461h

l0800_445E:
	jmp	4525h

l0800_4461:
	mov	ax,[bp-2h]
	mov	dx,0Ch
	imul	dx
	les	bx,[bp+4h]
	add	bx,ax
	inc	word ptr es:[bx+0Ah]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_4478:
	mov	ax,[4658h]
	mov	dx,0Ch
	imul	dx
	mov	cx,ax
	les	bx,[bp+4h]
	add	bx,ax
	mov	ax,es:[bx+2h]
	mov	dx,es:[bx]
	push	ax
	mov	ax,[465Ah]
	mov	bx,0Ch
	push	dx
	imul	bx
	mov	dx,ax
	mov	bx,[bp+4h]
	add	bx,ax
	pop	ax
	add	es:[bx],ax
	pop	ax
	adc	es:[bx+2h],ax
	mov	bx,[bp+4h]
	add	bx,cx
	mov	word ptr es:[bx+2h],0h
	mov	word ptr es:[bx],0h
	mov	bx,[bp+4h]
	add	bx,dx
	inc	word ptr es:[bx+0Ah]
	jmp	44DDh

l0800_44C3:
	mov	es,[bp+6h]
	mov	ax,es:[si+4h]
	mov	[465Ah],ax
	mov	dx,0Ch
	imul	dx
	mov	dx,ax
	mov	bx,[bp+4h]
	add	bx,ax
	inc	word ptr es:[bx+0Ah]

l0800_44DD:
	les	bx,[bp+4h]
	add	bx,dx
	mov	si,bx
	cmp	word ptr es:[bx+4h],0FFh
	jnz	44C3h

l0800_44EB:
	mov	ax,[4658h]
	mov	es:[si+4h],ax
	mov	bx,[bp+4h]
	add	bx,cx
	inc	word ptr es:[bx+0Ah]
	jmp	4517h

l0800_44FD:
	mov	es,[bp+6h]
	mov	ax,es:[si+4h]
	mov	[4658h],ax
	mov	dx,0Ch
	imul	dx
	mov	cx,ax
	mov	bx,[bp+4h]
	add	bx,ax
	inc	word ptr es:[bx+0Ah]

l0800_4517:
	les	bx,[bp+4h]
	add	bx,cx
	mov	si,bx
	cmp	word ptr es:[bx+4h],0FFh
	jnz	44FDh

l0800_4525:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	463Bh
	add	sp,6h
	or	ax,ax
	jz	453Bh

l0800_4538:
	jmp	4478h

l0800_453B:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	4550h
	add	sp,6h

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
	sub	sp,0Ah
	push	si
	push	di
	mov	word ptr [bp-2h],0h
	mov	word ptr [bp-4h],0h
	mov	word ptr [bp-6h],8000h
	mov	word ptr [bp-8h],0h
	mov	word ptr [bp-0Ah],1h
	jmp	45D6h

l0800_4573:
	xor	di,di
	mov	si,[bp+4h]
	cmp	di,[bp+8h]
	jnc	45C3h

l0800_457D:
	mov	es,[bp+6h]
	mov	ax,es:[si+0Ah]
	cmp	ax,[bp-0Ah]
	jnz	45BAh

l0800_4589:
	push	word ptr [bp-0Ah]
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	8BC2h
	push	dx
	push	ax
	call	45E2h
	add	sp,6h
	mov	es,[bp+6h]
	mov	es:[si+8h],dx
	mov	es:[si+6h],ax
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	add	[bp-4h],dx
	adc	[bp-2h],ax

l0800_45BA:
	add	si,0Ch
	inc	di
	cmp	di,[bp+8h]
	jc	457Dh

l0800_45C3:
	inc	word ptr [bp-0Ah]
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	shr	ax,1h
	rcr	dx,1h
	mov	[bp-6h],ax
	mov	[bp-8h],dx

l0800_45D6:
	cmp	word ptr [bp-0Ah],10h
	jbe	4573h

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
	sub	sp,4h
	mov	bx,[bp+8h]
	mov	word ptr [bp-2h],0h
	mov	word ptr [bp-4h],0h
	jmp	462Ah

l0800_45F7:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	shl	dx,1h
	rcl	ax,1h
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	mov	ax,[bp+4h]
	and	ax,1h
	or	ax,0h
	jz	461Ah

l0800_4612:
	or	word ptr [bp-4h],1h
	or	word ptr [bp-2h],0h

l0800_461A:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	shr	ax,1h
	rcr	dx,1h
	mov	[bp+6h],ax
	mov	[bp+4h],dx

l0800_462A:
	mov	ax,bx
	dec	bx
	or	ax,ax
	jnz	45F7h

l0800_4631:
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_463B: 0800:463B
;;   Called from:
;;     0800:452E (in fn0800_441C)
fn0800_463B proc
	push	bp
	mov	bp,sp
	sub	sp,0Ch
	push	si
	push	di
	mov	di,[bp+8h]
	mov	ax,0FFFFh
	mov	dx,0FFFFh
	mov	[bp-0Ah],ax
	mov	[bp-0Ch],dx
	mov	[bp-6h],ax
	mov	[bp-8h],dx
	xor	cx,cx
	mov	si,[bp+4h]
	cmp	cx,di
	jnc	46D5h

l0800_4661:
	mov	es,[bp+6h]
	mov	ax,es:[si+2h]
	mov	dx,es:[si]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	or	dx,ax
	jz	46CDh

l0800_4675:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	cmp	ax,[bp-6h]
	ja	46ABh

l0800_4680:
	jc	4687h

l0800_4682:
	cmp	dx,[bp-8h]
	jnc	46ABh

l0800_4687:
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	mov	[bp-0Ah],ax
	mov	[bp-0Ch],dx
	mov	ax,[465Ah]
	mov	[4658h],ax
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	mov	[bp-6h],ax
	mov	[bp-8h],dx
	mov	[465Ah],cx
	jmp	46CDh

l0800_46AB:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	cmp	ax,[bp-0Ah]
	ja	46CDh

l0800_46B6:
	jc	46BDh

l0800_46B8:
	cmp	dx,[bp-0Ch]
	jnc	46CDh

l0800_46BD:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	mov	[bp-0Ah],ax
	mov	[bp-0Ch],dx
	mov	[4658h],cx

l0800_46CD:
	add	si,0Ch
	inc	cx
	cmp	cx,di
	jc	4661h

l0800_46D5:
	cmp	word ptr [bp-6h],0FFh
	jnz	46E1h

l0800_46DB:
	cmp	word ptr [bp-8h],0FFh
	jz	46EDh

l0800_46E1:
	cmp	word ptr [bp-0Ah],0FFh
	jnz	46F5h

l0800_46E7:
	cmp	word ptr [bp-0Ch],0FFh
	jnz	46F5h

l0800_46ED:
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_46F5:
	mov	ax,1h
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
	sub	sp,20h
	push	si
	push	di
	cmp	word ptr [2A05h],0h
	ja	471Fh

l0800_470D:
	jc	4716h

l0800_470F:
	cmp	word ptr [2A03h],2Ah
	jnc	471Fh

l0800_4716:
	mov	ax,7h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_471F:
	xor	ax,ax
	push	ax
	mov	dx,28h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	di,ax
	add	ax,24h
	xor	dx,dx
	cmp	dx,[2A05h]
	jc	4761h

l0800_4750:
	ja	4758h

l0800_4752:
	cmp	ax,[2A03h]
	jbe	4761h

l0800_4758:
	mov	ax,7h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_4761:
	xor	ax,ax
	push	ax
	mov	ax,di
	add	ax,20h
	xor	dx,dx
	push	dx
	push	ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F58h
	add	sp,4h
	mov	cl,8h
	call	8CAAh
	cmp	dx,52h
	jnz	4798h

l0800_4793:
	cmp	ax,4E43h
	jz	47A1h

l0800_4798:
	mov	ax,7h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_47A1:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	xor	ax,ax
	push	ax
	mov	dx,2h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	di,ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-0Ah],ax
	or	di,di
	jz	47F3h

l0800_47F0:
	dec	word ptr [bp-0Ah]

l0800_47F3:
	mov	ax,[bp-0Ah]
	xor	dx,dx
	mov	cl,9h
	call	8C69h
	add	ax,di
	adc	dx,0h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	xor	ax,ax
	push	ax
	mov	dx,16h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-1Ch],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-1Eh],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-10h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-12h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-18h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-1Ah],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-16h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-14h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	ax,4D5Ah
	push	ax
	call	401Eh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-10h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-12h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-14h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-16h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	call	401Eh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-18h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-1Ah]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	ax,1Eh
	push	ax
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-1Eh]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	call	401Eh
	add	sp,6h
	xor	ax,ax
	push	ax
	mov	ax,[bp-1Ch]
	add	ax,20h
	xor	dx,dx
	push	dx
	push	ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	mov	word ptr [bp-0Ch],0h

l0800_49B5:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3DCFh
	add	sp,4h
	mov	ah,0h
	mov	di,ax
	or	di,di
	jz	4A22h

l0800_49CB:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-20h],ax
	xor	si,si
	mov	[bp-0Ah],di
	jmp	4A15h

l0800_49E3:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3DCFh
	add	sp,4h
	mov	ah,0h
	add	si,ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	si
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-20h]
	call	40BFh
	add	sp,6h

l0800_4A15:
	mov	ax,[bp-0Ah]
	dec	word ptr [bp-0Ah]
	or	ax,ax
	jnz	49E3h

l0800_4A1F:
	add	[bp-0Ch],di

l0800_4A22:
	or	di,di
	jnz	49B5h

l0800_4A26:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	or	dx,dx
	jg	4A56h

l0800_4A38:
	jl	4A3Fh

l0800_4A3A:
	cmp	ax,200h
	jnc	4A56h

l0800_4A3F:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	dx,200h
	sub	dx,ax
	mov	di,dx
	jmp	4A81h

l0800_4A56:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	dx,10h
	sub	dx,ax
	and	dx,0Fh
	mov	di,dx
	jmp	4A81h

l0800_4A70:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,0h
	push	ax
	call	4047h
	add	sp,6h

l0800_4A81:
	mov	ax,di
	dec	di
	or	ax,ax
	jnz	4A70h

l0800_4A88:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	cl,4h
	call	8C8Ah
	mov	[bp-0Eh],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	call	5374h
	mov	dx,ax
	or	dx,dx
	jz	4AC4h

l0800_4ABE:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_4AC4:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	4194h
	add	sp,4h
	sub	ax,[bp-4h]
	sbb	dx,[bp-2h]
	push	dx
	push	ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	mov	di,[bp-8h]
	and	di,1FFh
	mov	dx,[bp-6h]
	mov	ax,[bp-8h]
	mov	cl,9h
	call	8CAAh
	mov	[bp-0Ah],ax
	or	di,di
	jz	4B37h

l0800_4B34:
	inc	word ptr [bp-0Ah]

l0800_4B37:
	xor	ax,ax
	push	ax
	mov	dx,2h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	di
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-0Ah]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-0Ch]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-0Eh]
	call	40BFh
	add	sp,6h
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
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	5DCEh
	add	sp,4h
	or	ax,ax
	jnz	4BADh

l0800_4BA9:
	mov	ax,7h
	ret

l0800_4BAD:
	call	5374h
	ret

;; fn0800_4BB1: 0800:4BB1
;;   Called from:
;;     0800:1229 (in fn0800_112D)
fn0800_4BB1 proc
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	5DCEh
	add	sp,4h
	or	ax,ax
	jnz	4BC7h

l0800_4BC3:
	mov	ax,7h
	ret

l0800_4BC7:
	mov	ax,1h
	push	ax
	mov	ax,0FFFFh
	mov	dx,0FFEEh
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	ax,601Ah
	push	ax
	call	401Eh
	add	sp,6h
	xor	ax,ax
	mov	dx,10h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E27h
	add	sp,4h
	push	ax
	call	401Eh
	add	sp,6h
	call	5374h
	ret

;; fn0800_4C55: 0800:4C55
;;   Called from:
;;     0800:1230 (in fn0800_112D)
fn0800_4C55 proc
	push	bp
	mov	bp,sp
	sub	sp,18h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	or	dx,dx
	jnz	4C72h

l0800_4C6D:
	cmp	ax,3F3h
	jz	4C79h

l0800_4C72:
	mov	ax,7h
	mov	sp,bp
	pop	bp
	ret

l0800_4C79:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	mov	dx,3F3h
	push	ax
	push	dx
	call	409Ch
	add	sp,8h

l0800_4C8E:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-2h]
	push	ax
	call	409Ch
	add	sp,8h
	jmp	4CDAh

l0800_4CBC:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	push	dx
	push	ax
	call	409Ch
	add	sp,8h

l0800_4CDA:
	mov	ax,[bp-8h]
	mov	dx,[bp-6h]
	sub	word ptr [bp-8h],1h
	sbb	word ptr [bp-6h],0h
	or	ax,dx
	jnz	4CBCh

l0800_4CEC:
	mov	ax,[bp-4h]
	or	ax,[bp-2h]
	jnz	4C8Eh

l0800_4CF4:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	sub	ax,1h
	sbb	dx,0h
	push	dx
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-0Ah],dx
	mov	[bp-0Ch],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-0Eh],dx
	mov	[bp-10h],ax
	mov	ax,[bp-0Eh]
	mov	dx,[bp-10h]
	sub	dx,[bp-0Ch]
	sbb	ax,[bp-0Ah]
	mov	[bp-12h],ax
	mov	[bp-14h],dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-0Ah]
	push	word ptr [bp-0Ch]
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	ax,[bp-0Eh]
	mov	dx,[bp-10h]
	sub	dx,1h
	sbb	ax,0h
	push	ax
	push	dx
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	[4E8Eh],dx
	mov	[4E8Ch],ax
	mov	ax,1h
	push	ax
	xor	ax,ax
	mov	dx,4h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	mov	ax,[bp-12h]
	mov	dx,[bp-14h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	jmp	4DE4h

l0800_4DBE:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	push	dx
	push	ax
	call	409Ch
	add	sp,8h
	sub	word ptr [bp-4h],1h
	sbb	word ptr [bp-2h],0h

l0800_4DE4:
	mov	ax,[bp-4h]
	or	ax,[bp-2h]
	jnz	4DBEh

l0800_4DEC:
	mov	ax,1h
	push	ax
	xor	ax,ax
	mov	dx,4h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	add	ax,1h
	adc	dx,0h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	mov	ax,1h
	push	ax
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
	mov	cl,2h
	call	8C69h
	push	dx
	push	ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	jmp	4EDEh

l0800_4E41:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-6h]
	push	ax
	call	409Ch
	add	sp,8h
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	and	ax,3FFFh
	mov	[bp-18h],dx
	mov	[bp-16h],ax
	mov	cx,4h
	mov	bx,4F14h

l0800_4E7C:
	mov	ax,cs:[bx]
	cmp	ax,[bp-8h]
	jnz	4E8Dh

l0800_4E84:
	mov	ax,cs:[bx+8h]
	cmp	ax,[bp-16h]
	jz	4E94h

l0800_4E8D:
	add	bx,2h
	loop	4E7Ch

l0800_4E92:
	jmp	4ED7h

l0800_4E94:
	jmp	word ptr cs:[bx+10h]
0800:4E98                         E8 91 00 8B D0 0B D2 74         .......t
0800:4EA0 3D 8B E5 5D C3 FF 36 E5 29 FF 36 E3 29 E8 AD EF =..]..6.).6.)...
0800:4EB0 83 C4 04 89 56 FE 89 46 FC FF 36 E1 29 FF 36 DF ....V..F..6.).6.
0800:4EC0 29 FF 76 FE 50 E8 D4 F1 83 C4 08 83 06 8C 4E 04 ).v.P.........N.
0800:4ED0 83 16 8E 4E 00 EB 07                            ...N...         

l0800_4ED7:
	mov	ax,9h
	mov	sp,bp
	pop	bp
	ret

l0800_4EDE:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	4194h
	add	sp,4h
	pop	bx
	cmp	bx,dx
	pop	dx
	jnc	4F05h

l0800_4F02:
	jmp	4E41h

l0800_4F05:
	jnz	4F0Eh

l0800_4F07:
	cmp	dx,ax
	jnc	4F0Eh

l0800_4F0B:
	jmp	4E41h

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
	sub	sp,1Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	cl,2h
	call	8C69h
	mov	[bp-10h],dx
	mov	[bp-12h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-18h],dx
	mov	[bp-1Ah],ax
	cmp	word ptr [bp-10h],0h
	jc	4F95h

l0800_4F65:
	ja	4F6Dh

l0800_4F67:
	cmp	word ptr [bp-12h],12h
	jbe	4F95h

l0800_4F6D:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-14h],dx
	mov	[bp-16h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-0Ch],dx
	mov	[bp-0Eh],ax

l0800_4F95:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F58h
	add	sp,4h
	mov	cl,8h
	call	8CAAh
	cmp	dx,52h
	jnz	4FB5h

l0800_4FAD:
	cmp	ax,4E43h
	jnz	4FB5h

l0800_4FB2:
	jmp	50E6h

l0800_4FB5:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	dx,[bp-10h]
	mov	ax,[bp-12h]
	mov	cl,2h
	call	8CAAh
	push	dx
	push	ax
	call	409Ch
	add	sp,8h
	xor	ax,ax
	push	ax
	push	word ptr [bp-18h]
	push	word ptr [bp-1Ah]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [bp-10h]
	push	word ptr [bp-12h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	jmp	50BCh

l0800_5006:
	mov	ax,1h
	push	ax
	xor	ax,ax
	mov	dx,4h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	mov	dx,3ECh
	push	ax
	push	dx
	call	409Ch
	add	sp,8h

l0800_5034:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-4h],dx
	mov	[bp-6h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-4h]
	push	ax
	call	409Ch
	add	sp,8h
	mov	ax,[bp-6h]
	or	ax,[bp-4h]
	jz	50B1h

l0800_5062:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	sub	ax,1h
	sbb	dx,0h
	mov	[bp-8h],dx
	mov	[bp-0Ah],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-8h]
	push	ax
	call	409Ch
	add	sp,8h
	mov	dx,[bp-4h]
	mov	ax,[bp-6h]
	mov	cl,2h
	call	8C69h
	push	dx
	push	ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch

l0800_50B1:
	mov	ax,[bp-6h]
	or	ax,[bp-4h]
	jz	50BCh

l0800_50B9:
	jmp	5034h

l0800_50BC:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F58h
	add	sp,4h
	or	dx,dx
	jnz	50D6h

l0800_50CE:
	cmp	ax,3ECh
	jnz	50D6h

l0800_50D3:
	jmp	5006h

l0800_50D6:
	add	word ptr [4E8Ch],4h
	adc	word ptr [4E8Eh],0h
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret

l0800_50E6:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	dx,[bp-14h]
	mov	ax,[bp-16h]
	mov	cl,2h
	call	8CAAh
	push	dx
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-4h],dx
	mov	[bp-6h],ax
	xor	ax,ax
	push	ax
	push	word ptr [4E8Eh]
	push	word ptr [4E8Ch]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-0Ch]
	push	word ptr [bp-0Eh]
	call	409Ch
	add	sp,8h
	add	word ptr [4E8Ch],4h
	adc	word ptr [4E8Eh],0h
	xor	ax,ax
	push	ax
	push	word ptr [bp-4h]
	push	word ptr [bp-6h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	call	5374h
	mov	[bp-2h],ax
	xor	ax,ax
	push	ax
	mov	ax,[bp-18h]
	mov	dx,[bp-1Ah]
	add	dx,[bp-12h]
	adc	ax,[bp-10h]
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	mov	ax,[bp-2h]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_518F: 0800:518F
;;   Called from:
;;     0800:1237 (in fn0800_112D)
fn0800_518F proc
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	5DCEh
	add	sp,4h
	or	ax,ax
	jnz	51A5h

l0800_51A1:
	mov	ax,7h
	ret

l0800_51A5:
	call	5374h
	ret

;; fn0800_51A9: 0800:51A9
;;   Called from:
;;     0800:123E (in fn0800_112D)
fn0800_51A9 proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	push	si
	jmp	533Dh

l0800_51B3:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E9Ah
	add	sp,4h
	or	ax,ax
	jz	51D5h

l0800_51C5:
	cmp	ax,1h
	jz	51F5h

l0800_51CA:
	cmp	ax,3h
	jnz	51D2h

l0800_51CF:
	jmp	52D8h

l0800_51D2:
	jmp	5301h

l0800_51D5:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3DCFh
	add	sp,4h
	push	ax
	call	4047h
	add	sp,6h
	jmp	533Dh

l0800_51F5:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	push	dx
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	word ptr [bp-2h],0h
	mov	[bp-4h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	ax
	call	40BFh
	add	sp,6h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F58h
	add	sp,4h
	mov	cl,8h
	call	8CAAh
	cmp	dx,52h
	jnz	52BAh

l0800_5250:
	cmp	ax,4E43h
	jnz	52BAh

l0800_5255:
	call	5374h
	mov	si,ax
	or	si,si
	jz	5263h

l0800_525E:
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_5263:
	mov	ax,1h
	push	ax
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	add	dx,2h
	adc	ax,0h
	neg	ax
	neg	dx
	sbb	ax,0h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [2A07h]
	call	40BFh
	add	sp,6h
	mov	ax,1h
	push	ax
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	jmp	533Dh

l0800_52BA:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	jmp	533Dh

l0800_52D8:
	xor	si,si
	jmp	52FAh

l0800_52DC:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3DCFh
	add	sp,4h
	push	ax
	call	4047h
	add	sp,6h
	inc	si

l0800_52FA:
	cmp	si,3h
	jnz	52DCh

l0800_52FF:
	jmp	533Dh

l0800_5301:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	4194h
	add	sp,4h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	pop	bx
	pop	cx
	sub	cx,ax
	sbb	bx,dx
	push	bx
	push	cx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch

l0800_533D:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	4194h
	add	sp,4h
	pop	bx
	cmp	bx,dx
	pop	dx
	jnc	5364h

l0800_5361:
	jmp	51B3h

l0800_5364:
	jnz	536Dh

l0800_5366:
	cmp	dx,ax
	jnc	536Dh

l0800_536A:
	jmp	51B3h

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
	sub	sp,0Eh
	push	si
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-0Ah],dx
	mov	[bp-0Ch],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	and	ax,3h
	mov	[2A21h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[2A09h],dx
	mov	[2A07h],ax
	cmp	word ptr [2A21h],0h
	jnz	53EFh

l0800_53BF:
	push	word ptr [2A09h]
	push	ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	mov	[2A05h],ax
	mov	[2A03h],dx
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_53EF:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[2A05h],dx
	mov	[2A03h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E27h
	add	sp,4h
	mov	[2E4Bh],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E27h
	add	sp,4h
	mov	[2E49h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E27h
	add	sp,4h
	push	word ptr [2A05h]
	push	word ptr [2A03h]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	2D0Ah
	add	sp,8h
	cmp	ax,[2E49h]
	jz	5458h

l0800_5450:
	mov	ax,5h
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_5458:
	mov	ax,1h
	mov	dx,0Fh
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	inc	dx
	mov	[2E73h],dx
	mov	word ptr [2E71h],0h
	mov	ax,1h
	mov	dx,0Fh
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	inc	dx
	mov	[2E5Bh],dx
	mov	word ptr [2E59h],0h
	mov	ax,[2E73h]
	mov	dx,[2E71h]
	add	dx,0FDh
	mov	[2E6Fh],ax
	mov	[2E6Dh],dx
	mov	ax,[2E5Bh]
	mov	dx,[2E59h]
	add	dx,[2E31h]
	mov	[2E57h],ax
	mov	[2E55h],dx
	mov	word ptr [2E4Dh],0h
	xor	si,si
	mov	word ptr [2E45h],0h
	mov	word ptr [29FDh],0h
	mov	word ptr [29FBh],0h
	mov	word ptr [2A01h],0h
	mov	word ptr [29FFh],0h
	mov	ax,[2E4Fh]
	mov	[bp-0Eh],ax
	mov	ax,1h
	push	ax
	call	5C1Ah
	add	sp,2h
	or	ax,ax
	jz	5504h

l0800_54F3:
	cmp	word ptr [2A25h],2h
	jz	5504h

l0800_54FA:
	cmp	word ptr [2A25h],7h
	jz	5504h

l0800_5501:
	mov	si,0Ah

l0800_5504:
	or	si,si
	jnz	555Dh

l0800_5508:
	mov	ax,1h
	push	ax
	call	5C1Ah
	add	sp,2h
	or	ax,ax
	jz	5557h

l0800_5516:
	cmp	word ptr [2A23h],1h
	jz	554Bh

l0800_551D:
	mov	ax,10h
	push	ax
	call	5C1Ah
	add	sp,2h
	mov	dx,ax
	cmp	word ptr [2A25h],2h
	jnz	553Bh

l0800_5530:
	cmp	word ptr [2E4Fh],0h
	jnz	553Bh

l0800_5537:
	mov	[2E4Fh],dx

l0800_553B:
	cmp	[2E4Fh],dx
	jz	554Bh

l0800_5541:
	cmp	word ptr [2E4Fh],0h
	jz	554Bh

l0800_5548:
	mov	si,0Ch

l0800_554B:
	cmp	word ptr [2E4Fh],0h
	jnz	555Dh

l0800_5552:
	mov	si,0Bh
	jmp	555Dh

l0800_5557:
	mov	word ptr [2E4Fh],0h

l0800_555D:
	or	si,si
	jnz	557Ch

l0800_5561:
	mov	ax,[2A21h]
	cmp	ax,1h
	jz	5570h

l0800_5569:
	cmp	ax,2h
	jz	5577h

l0800_556E:
	jmp	557Ch

l0800_5570:
	call	55E8h
	mov	si,ax
	jmp	557Ch

l0800_5577:
	call	579Bh
	mov	si,ax

l0800_557C:
	mov	ax,[bp-0Eh]
	mov	[2E4Fh],ax
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	call	4346h
	add	sp,4h
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	4346h
	add	sp,4h
	add	word ptr [2A03h],12h
	adc	word ptr [2A05h],0h
	xor	ax,ax
	push	ax
	mov	ax,[bp-0Ah]
	mov	dx,[bp-0Ch]
	add	dx,[2A03h]
	adc	ax,[2A05h]
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	or	si,si
	jz	55D0h

l0800_55C9:
	mov	ax,si
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_55D0:
	mov	ax,[2E4Dh]
	cmp	ax,[2E4Bh]
	jz	55E1h

l0800_55D9:
	mov	ax,6h
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
	sub	sp,4h
	jmp	574Bh

l0800_55F1:
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,290Fh
	push	ax
	call	5A24h
	add	sp,6h
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,284Fh
	push	ax
	call	5A24h
	add	sp,6h
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,278Fh
	push	ax
	call	5A24h
	add	sp,6h
	mov	ax,10h
	push	ax
	call	5C39h
	add	sp,2h
	mov	word ptr [bp-2h],0h
	mov	[bp-4h],ax
	jmp	5736h

l0800_5633:
	push	ds
	mov	ax,290Fh
	push	ax
	call	5A8Dh
	add	sp,4h
	mov	[2E47h],ax
	add	[29FFh],ax
	adc	word ptr [2A01h],0h
	cmp	word ptr [2E47h],0h
	jnz	5654h

l0800_5651:
	jmp	56E6h

l0800_5654:
	jmp	5664h

l0800_5656:
	call	5B15h
	xor	al,[2E4Fh]
	push	ax
	call	5D2Fh
	add	sp,2h

l0800_5664:
	mov	ax,[2E47h]
	dec	word ptr [2E47h]
	or	ax,ax
	jnz	5656h

l0800_566F:
	test	word ptr [2E4Fh],1h
	jz	5684h

l0800_5677:
	mov	ax,[2E4Fh]
	shr	ax,1h
	or	ax,8000h
	mov	[2E4Fh],ax
	jmp	5688h

l0800_5684:
	shr	word ptr [2E4Fh],1h

l0800_5688:
	les	bx,[2E6Dh]
	mov	al,es:[bx+2h]
	mov	ah,0h
	push	ax
	xor	ax,ax
	mov	dl,es:[bx+1h]
	mov	dh,0h
	mov	cl,8h
	shl	dx,cl
	add	ax,dx
	pop	dx
	adc	dx,0h
	mov	bl,es:[bx]
	mov	bh,0h
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
	mov	cl,[2E45h]
	call	8C69h
	push	ax
	mov	ax,1h
	mov	cl,[2E45h]
	shl	ax,cl
	dec	ax
	push	dx
	cwd
	mov	bx,[29FDh]
	mov	cx,[29FBh]
	and	cx,ax
	and	bx,dx
	pop	ax
	pop	dx
	add	dx,cx
	adc	ax,bx
	mov	[29FDh],ax
	mov	[29FBh],dx

l0800_56E6:
	mov	ax,[bp-4h]
	or	ax,[bp-2h]
	jz	574Bh

l0800_56EE:
	push	ds
	mov	ax,284Fh
	push	ax
	call	5A8Dh
	add	sp,4h
	inc	ax
	mov	[2E2Bh],ax
	push	ds
	mov	ax,278Fh
	push	ax
	call	5A8Dh
	add	sp,4h
	add	ax,2h
	mov	[2E29h],ax
	add	[29FFh],ax
	adc	word ptr [2A01h],0h
	jmp	572Bh

l0800_5719:
	les	bx,[2E55h]
	sub	bx,[2E2Bh]
	mov	al,es:[bx]
	push	ax
	call	5D2Fh
	add	sp,2h

l0800_572B:
	mov	ax,[2E29h]
	dec	word ptr [2E29h]
	or	ax,ax
	jnz	5719h

l0800_5736:
	mov	ax,[bp-4h]
	mov	dx,[bp-2h]
	sub	word ptr [bp-4h],1h
	sbb	word ptr [bp-2h],0h
	or	ax,dx
	jz	574Bh

l0800_5748:
	jmp	5633h

l0800_574B:
	mov	ax,[2A01h]
	mov	dx,[29FFh]
	cmp	ax,[2A09h]
	jnc	575Bh

l0800_5758:
	jmp	55F1h

l0800_575B:
	jnz	5766h

l0800_575D:
	cmp	dx,[2A07h]
	jnc	5766h

l0800_5763:
	jmp	55F1h

l0800_5766:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	ax,[2E55h]
	xor	dx,dx
	sub	ax,[2E59h]
	sbb	dx,0h
	sub	ax,[2E31h]
	sbb	dx,0h
	push	dx
	push	ax
	mov	ax,[2E59h]
	add	ax,[2E31h]
	push	word ptr [2E5Bh]
	push	ax
	call	4152h
	add	sp,0Ch
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret

;; fn0800_579B: 0800:579B
;;   Called from:
;;     0800:5577 (in fn0800_5374)
fn0800_579B proc
	jmp	58F2h

l0800_579E:
	call	5B15h
	xor	al,[2E4Fh]
	push	ax
	call	5D2Fh
	add	sp,2h
	test	word ptr [2E4Fh],1h
	jz	57C1h

l0800_57B4:
	mov	ax,[2E4Fh]
	shr	ax,1h
	or	ax,8000h
	mov	[2E4Fh],ax
	jmp	57C5h

l0800_57C1:
	shr	word ptr [2E4Fh],1h

l0800_57C5:
	add	word ptr [29FFh],1h
	adc	word ptr [2A01h],0h

l0800_57CF:
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	or	ax,ax
	jz	579Eh

l0800_57DD:
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	or	ax,ax
	jz	5866h

l0800_57EB:
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	or	ax,ax
	jnz	580Ah

l0800_57F9:
	mov	word ptr [2E29h],2h
	call	5B15h
	mov	ah,0h
	inc	ax
	mov	[2E2Bh],ax
	jmp	5838h

l0800_580A:
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	or	ax,ax
	jnz	5820h

l0800_5818:
	mov	word ptr [2E29h],3h
	jmp	5835h

l0800_5820:
	call	5B15h
	mov	ah,0h
	add	ax,8h
	mov	[2E29h],ax
	cmp	word ptr [2E29h],8h
	jnz	5835h

l0800_5832:
	jmp	58E8h

l0800_5835:
	call	5975h

l0800_5838:
	mov	ax,[2E29h]
	add	[29FFh],ax
	adc	word ptr [2A01h],0h
	jmp	5858h

l0800_5846:
	les	bx,[2E55h]
	sub	bx,[2E2Bh]
	mov	al,es:[bx]
	push	ax
	call	5D2Fh
	add	sp,2h

l0800_5858:
	mov	ax,[2E29h]
	dec	word ptr [2E29h]
	or	ax,ax
	jnz	5846h

l0800_5863:
	jmp	57CFh

l0800_5866:
	call	593Fh
	cmp	word ptr [2E29h],9h
	jnz	58B7h

l0800_5870:
	call	5A0Fh
	mov	ax,[2E47h]
	add	[29FFh],ax
	adc	word ptr [2A01h],0h
	jmp	588Fh

l0800_5881:
	call	5B15h
	xor	al,[2E4Fh]
	push	ax
	call	5D2Fh
	add	sp,2h

l0800_588F:
	mov	ax,[2E47h]
	dec	word ptr [2E47h]
	or	ax,ax
	jnz	5881h

l0800_589A:
	test	word ptr [2E4Fh],1h
	jz	58B0h

l0800_58A2:
	mov	ax,[2E4Fh]
	shr	ax,1h
	or	ax,8000h
	mov	[2E4Fh],ax
	jmp	57CFh

l0800_58B0:
	shr	word ptr [2E4Fh],1h
	jmp	57CFh

l0800_58B7:
	call	5975h
	mov	ax,[2E29h]
	add	[29FFh],ax
	adc	word ptr [2A01h],0h
	jmp	58DAh

l0800_58C8:
	les	bx,[2E55h]
	sub	bx,[2E2Bh]
	mov	al,es:[bx]
	push	ax
	call	5D2Fh
	add	sp,2h

l0800_58DA:
	mov	ax,[2E29h]
	dec	word ptr [2E29h]
	or	ax,ax
	jnz	58C8h

l0800_58E5:
	jmp	57CFh

l0800_58E8:
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h

l0800_58F2:
	mov	ax,[2A01h]
	mov	dx,[29FFh]
	cmp	ax,[2A09h]
	jnc	5902h

l0800_58FF:
	jmp	57CFh

l0800_5902:
	jnz	590Dh

l0800_5904:
	cmp	dx,[2A07h]
	jnc	590Dh

l0800_590A:
	jmp	57CFh

l0800_590D:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	ax,[2E55h]
	xor	dx,dx
	sub	ax,[2E59h]
	sbb	dx,0h
	sub	ax,[2E31h]
	sbb	dx,0h
	push	dx
	push	ax
	mov	ax,[2E59h]
	add	ax,[2E31h]
	push	word ptr [2E5Bh]
	push	ax
	call	4152h
	add	sp,0Ch
	xor	ax,ax
	ret

;; fn0800_593F: 0800:593F
;;   Called from:
;;     0800:5866 (in fn0800_579B)
fn0800_593F proc
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	add	ax,4h
	mov	[2E29h],ax
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	or	ax,ax
	jz	5974h

l0800_595D:
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	mov	dx,[2E29h]
	dec	dx
	shl	dx,1h
	add	dx,ax
	mov	[2E29h],dx

l0800_5974:
	ret

;; fn0800_5975: 0800:5975
;;   Called from:
;;     0800:5835 (in fn0800_579B)
;;     0800:58B7 (in fn0800_579B)
fn0800_5975 proc
	mov	word ptr [2E2Bh],0h
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	or	ax,ax
	jz	59FAh

l0800_5989:
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	mov	[2E2Bh],ax
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	or	ax,ax
	jz	59E3h

l0800_59A4:
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	mov	dx,[2E2Bh]
	shl	dx,1h
	add	dx,ax
	or	dx,4h
	mov	[2E2Bh],dx
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	or	ax,ax
	jnz	59FAh

l0800_59CB:
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	mov	dx,[2E2Bh]
	shl	dx,1h
	add	dx,ax
	mov	[2E2Bh],dx
	jmp	59FAh

l0800_59E3:
	cmp	word ptr [2E2Bh],0h
	jnz	59FAh

l0800_59EA:
	mov	ax,1h
	push	ax
	call	5CD9h
	add	sp,2h
	add	ax,2h
	mov	[2E2Bh],ax

l0800_59FA:
	call	5B15h
	mov	ah,0h
	mov	dx,[2E2Bh]
	mov	cl,8h
	shl	dx,cl
	add	dx,ax
	inc	dx
	mov	[2E2Bh],dx
	ret

;; fn0800_5A0F: 0800:5A0F
;;   Called from:
;;     0800:5870 (in fn0800_579B)
fn0800_5A0F proc
	mov	ax,4h
	push	ax
	call	5CD9h
	add	sp,2h
	shl	ax,1h
	shl	ax,1h
	add	ax,0Ch
	mov	[2E47h],ax
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
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	43D4h
	add	sp,6h
	mov	ax,5h
	push	ax
	call	5C39h
	add	sp,2h
	mov	[bp+8h],ax
	or	ax,ax
	jz	5A89h

l0800_5A49:
	cmp	word ptr [bp+8h],10h
	jbe	5A54h

l0800_5A4F:
	mov	word ptr [bp+8h],10h

l0800_5A54:
	xor	di,di
	mov	si,[bp+4h]
	add	si,0Ah
	cmp	di,[bp+8h]
	jnc	5A7Ah

l0800_5A61:
	mov	ax,4h
	push	ax
	call	5C39h
	add	sp,2h
	mov	es,[bp+6h]
	mov	es:[si],ax
	add	si,0Ch
	inc	di
	cmp	di,[bp+8h]
	jc	5A61h

l0800_5A7A:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	4550h
	add	sp,6h

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
	sub	sp,2h
	push	si
	push	di
	mov	word ptr [bp-2h],0h
	mov	di,[bp+4h]
	jmp	5AA5h

l0800_5A9F:
	add	di,0Ch
	inc	word ptr [bp-2h]

l0800_5AA5:
	mov	es,[bp+6h]
	cmp	word ptr es:[di+0Ah],0h
	jz	5A9Fh

l0800_5AAF:
	mov	cl,es:[di+0Ah]
	mov	ax,1h
	shl	ax,cl
	dec	ax
	cwd
	mov	bx,[29FDh]
	mov	cx,[29FBh]
	and	cx,ax
	and	bx,dx
	cmp	bx,es:[di+8h]
	jnz	5A9Fh

l0800_5ACC:
	cmp	cx,es:[di+6h]
	jnz	5A9Fh

l0800_5AD2:
	mov	ax,[bp-2h]
	mov	dx,0Ch
	imul	dx
	mov	bx,[bp+4h]
	add	bx,ax
	push	word ptr es:[bx+0Ah]
	call	5C39h
	add	sp,2h
	cmp	word ptr [bp-2h],2h
	jnc	5AF8h

l0800_5AEF:
	mov	ax,[bp-2h]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_5AF8:
	mov	ax,[bp-2h]
	dec	ax
	push	ax
	call	5C39h
	add	sp,2h
	mov	cl,[bp-2h]
	dec	cl
	mov	dx,1h
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
	sub	sp,8h
	mov	ax,[2E73h]
	mov	dx,[2E71h]
	add	dx,0FDh
	cmp	ax,[2E6Fh]
	jz	5B2Eh

l0800_5B2B:
	jmp	5C0Bh

l0800_5B2E:
	cmp	dx,[2E6Dh]
	jz	5B37h

l0800_5B34:
	jmp	5C0Bh

l0800_5B37:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	4194h
	add	sp,4h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	pop	bx
	pop	cx
	sub	cx,ax
	sbb	bx,dx
	mov	[bp-2h],bx
	mov	[bp-4h],cx
	cmp	word ptr [bp-2h],0h
	jc	5B76h

l0800_5B67:
	ja	5B6Fh

l0800_5B69:
	cmp	word ptr [bp-4h],0FDh
	jbe	5B76h

l0800_5B6F:
	xor	dx,dx
	mov	ax,0FFFDh
	jmp	5B7Ch

l0800_5B76:
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]

l0800_5B7C:
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	push	word ptr [bp-6h]
	push	ax
	mov	ax,[2E73h]
	mov	dx,[2E71h]
	mov	[2E6Fh],ax
	mov	[2E6Dh],dx
	push	ax
	push	dx
	call	4110h
	add	sp,0Ch
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	sub	[bp-4h],dx
	mov	dx,[bp-4h]
	sbb	[bp-2h],ax
	mov	ax,[bp-2h]
	or	ax,ax
	jc	5BCBh

l0800_5BBA:
	ja	5BC1h

l0800_5BBC:
	cmp	dx,2h
	jbe	5BCBh

l0800_5BC1:
	mov	word ptr [bp-2h],0h
	mov	word ptr [bp-4h],2h

l0800_5BCB:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	mov	ax,[2E71h]
	add	ax,[bp-8h]
	push	word ptr [2E73h]
	push	ax
	call	4110h
	add	sp,0Ch
	mov	ax,1h
	push	ax
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	neg	ax
	neg	dx
	sbb	ax,0h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah

l0800_5C0B:
	les	bx,[2E6Dh]
	inc	word ptr [2E6Dh]
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
	mov	ax,[bp+4h]
	cmp	word ptr [2A21h],2h
	jnz	5C30h

l0800_5C27:
	push	ax
	call	5CD9h
	add	sp,2h
	pop	bp
	ret

l0800_5C30:
	push	ax
	call	5C39h
	add	sp,2h
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
	sub	sp,6h
	push	si
	push	di
	mov	si,[bp+4h]
	xor	di,di
	mov	word ptr [bp-6h],1h
	jmp	5CCAh

l0800_5C4D:
	cmp	word ptr [2E45h],0h
	jnz	5CA2h

l0800_5C54:
	call	5B15h
	mov	ah,0h
	mov	[bp-2h],ax
	call	5B15h
	mov	ah,0h
	mov	[bp-4h],ax
	les	bx,[2E6Dh]
	mov	al,es:[bx+1h]
	mov	ah,0h
	xor	dx,dx
	mov	cl,18h
	call	8C69h
	les	bx,[2E6Dh]
	mov	bl,es:[bx]
	mov	bh,0h
	add	ax,0h
	adc	dx,bx
	mov	bx,[bp-4h]
	mov	cl,8h
	shl	bx,cl
	add	ax,bx
	adc	dx,0h
	add	ax,[bp-2h]
	adc	dx,0h
	mov	[29FDh],dx
	mov	[29FBh],ax
	mov	word ptr [2E45h],10h

l0800_5CA2:
	mov	ax,[29FBh]
	and	ax,1h
	or	ax,0h
	jz	5CB0h

l0800_5CAD:
	or	di,[bp-6h]

l0800_5CB0:
	mov	ax,[29FDh]
	mov	dx,[29FBh]
	shr	ax,1h
	rcr	dx,1h
	mov	[29FDh],ax
	mov	[29FBh],dx
	shl	word ptr [bp-6h],1h
	dec	word ptr [2E45h]
	dec	si

l0800_5CCA:
	or	si,si
	jz	5CD1h

l0800_5CCE:
	jmp	5C4Dh

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
	mov	di,[bp+4h]
	xor	si,si
	jmp	5D25h

l0800_5CE5:
	cmp	word ptr [2E45h],0h
	jnz	5D00h

l0800_5CEC:
	call	5B15h
	mov	ah,0h
	mov	word ptr [29FDh],0h
	mov	[29FBh],ax
	mov	word ptr [2E45h],8h

l0800_5D00:
	shl	si,1h
	mov	ax,[29FBh]
	and	ax,80h
	or	ax,0h
	jz	5D0Eh

l0800_5D0D:
	inc	si

l0800_5D0E:
	mov	ax,[29FDh]
	mov	dx,[29FBh]
	shl	dx,1h
	rcl	ax,1h
	mov	[29FDh],ax
	mov	[29FBh],dx
	dec	word ptr [2E45h]
	dec	di

l0800_5D25:
	or	di,di
	jnz	5CE5h

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
	mov	ax,[2E5Bh]
	mov	dx,[2E59h]
	dec	dx
	cmp	ax,[2E57h]
	jnz	5D9Eh

l0800_5D40:
	cmp	dx,[2E55h]
	jnz	5D9Eh

l0800_5D46:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	mov	dx,0FFFFh
	sub	dx,[2E31h]
	sbb	ax,0h
	push	ax
	push	dx
	mov	ax,[2E59h]
	add	ax,[2E31h]
	push	word ptr [2E5Bh]
	push	ax
	call	4152h
	add	sp,0Ch
	push	word ptr [2E31h]
	mov	ax,[2E55h]
	sub	ax,[2E31h]
	push	word ptr [2E57h]
	push	ax
	push	word ptr [2E5Bh]
	push	word ptr [2E59h]
	call	0B0F3h
	add	sp,0Ah
	mov	ax,[2E5Bh]
	mov	dx,[2E59h]
	add	dx,[2E31h]
	mov	[2E57h],ax
	mov	[2E55h],dx

l0800_5D9E:
	les	bx,[2E55h]
	mov	al,[bp+4h]
	mov	es:[bx],al
	inc	word ptr [2E55h]
	mov	al,[2E4Dh]
	xor	al,[bp+4h]
	mov	ah,0h
	and	ax,0FFh
	shl	ax,1h
	mov	bx,ax
	mov	ax,[bx+2A29h]
	mov	dx,[2E4Dh]
	mov	cl,8h
	shr	dx,cl
	xor	ax,dx
	mov	[2E4Dh],ax
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
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	4194h
	add	sp,4h
	or	dx,dx
	jc	5DEEh

l0800_5DE2:
	jnz	5DE9h

l0800_5DE4:
	cmp	ax,400h
	jc	5DEEh

l0800_5DE9:
	mov	ax,400h
	jmp	5DFDh

l0800_5DEE:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	4194h
	add	sp,4h
	and	ax,0FFFCh

l0800_5DFD:
	mov	si,ax
	jmp	5E5Bh

l0800_5E01:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3E27h
	add	sp,4h
	cmp	ax,524Eh
	jnz	5E58h

l0800_5E12:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	3E27h
	add	sp,4h
	and	ax,0FF00h
	cmp	ax,4300h
	jnz	5E40h

l0800_5E26:
	mov	ax,1h
	push	ax
	mov	ax,0FFFFh
	mov	dx,0FFFCh
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	jmp	5E5Fh

l0800_5E40:
	mov	ax,1h
	push	ax
	mov	ax,0FFFFh
	mov	dx,0FFFEh
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah

l0800_5E58:
	sub	si,2h

l0800_5E5B:
	or	si,si
	jnz	5E01h

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
	sub	sp,38h
	push	si
	push	di
	push	ds
	pop	es
	mov	di,2202h
	mov	si,4271h
	mov	cx,4h
	mov	dx,3h
	cmp	dx,cx
	jnc	5E80h

l0800_5E7E:
	mov	cx,dx

l0800_5E80:
	xor	ax,ax
	rep cmpsb
	jz	5E8Bh

l0800_5E86:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_5E8B:
	or	ax,ax
	jz	5EC3h

l0800_5E8F:
	push	ds
	pop	es
	mov	di,4271h
	mov	si,2202h
	mov	dx,3h
	mov	cx,4h
	sub	dx,cx
	jnc	5EA5h

l0800_5EA1:
	add	cx,dx
	xor	dx,dx

l0800_5EA5:
	shr	cx,1h
	rep movsw
	adc	cx,cx
	rep movsb
	mov	cx,dx
	xor	ax,ax

l0800_5EB1:
	rep stosb

l0800_5EB3:
	push	ds
	mov	ax,2E75h
	push	ax
	push	ds
	mov	ax,4271h
	push	ax
	call	09A3h
	add	sp,8h

l0800_5EC3:
	mov	al,[2E75h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,[2E76h]
	mov	dh,0h
	add	ax,dx
	xor	dx,dx
	cmp	dx,[2A09h]
	jc	5EEDh

l0800_5EDC:
	jnz	5EE4h

l0800_5EDE:
	cmp	ax,[2A07h]
	jc	5EEDh

l0800_5EE4:
	mov	ax,3h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_5EED:
	cmp	word ptr [2A0Dh],0h
	jc	5F0Ah

l0800_5EF4:
	ja	5EFEh

l0800_5EF6:
	cmp	word ptr [2A0Bh],7FF0h
	jbe	5F0Ah

l0800_5EFE:
	mov	word ptr [2A0Dh],0h
	mov	word ptr [2A0Bh],7FF0h

l0800_5F0A:
	xor	ax,ax
	push	ax
	mov	dx,2h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-22h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	di,ax
	cmp	word ptr [bp-22h],0h
	jz	5F48h

l0800_5F47:
	dec	di

l0800_5F48:
	mov	ax,di
	xor	dx,dx
	mov	cl,9h
	call	8C69h
	add	ax,[bp-22h]
	adc	dx,0h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	cmp	ax,[2A09h]
	jc	5F7Eh

l0800_5F69:
	ja	5F71h

l0800_5F6B:
	cmp	dx,[2A07h]
	jbe	5F7Eh

l0800_5F71:
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx

l0800_5F7E:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-24h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-26h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-28h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-2Ah],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-2Ch],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-2Eh],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-30h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-32h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-34h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,[2E75h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,[2E76h]
	mov	dh,0h
	add	ax,dx
	xor	dx,dx
	push	dx
	push	ax
	push	ds
	mov	ax,2E77h
	push	ax
	call	4152h
	add	sp,0Ch
	cmp	word ptr [bp-24h],0h
	jnz	6064h

l0800_6061:
	jmp	6220h

l0800_6064:
	xor	ax,ax
	mov	dx,0FFFFh
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[bp-0Ah],dx
	mov	[bp-0Ch],ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp-34h]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	xor	si,si
	mov	ax,[bp-0Ch]
	mov	[bp-36h],ax
	jmp	60D6h

l0800_6096:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	[bp-22h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	di,ax
	xor	dx,dx
	mov	cl,4h
	call	8C69h
	add	ax,[bp-22h]
	adc	dx,0h
	mov	es,[bp-0Ah]
	mov	bx,[bp-36h]
	mov	es:[bx+2h],dx
	mov	es:[bx],ax
	add	word ptr [bp-36h],4h
	inc	si

l0800_60D6:
	cmp	si,[bp-24h]
	jnz	6096h

l0800_60DB:
	mov	ax,667Bh
	push	ax
	mov	ax,4h
	push	ax
	push	word ptr [bp-24h]
	push	word ptr [bp-0Ah]
	push	word ptr [bp-0Ch]
	call	0B95Eh
	add	sp,0Ah
	xor	si,si
	mov	di,[bp-0Ch]
	xor	ax,ax
	adc	ax,0h
	neg	ax
	mov	[bp-38h],ax
	jmp	620Ch

l0800_6104:
	mov	es,[bp-0Ah]
	mov	ax,es:[di+2h]
	mov	dx,es:[di]
	mov	[bp-12h],ax
	mov	[bp-14h],dx
	and	dx,0F0h
	and	ax,0Fh
	mov	[bp-0Eh],ax
	mov	[bp-10h],dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,0h
	push	ax
	call	4047h
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	dx,[bp-0Eh]
	mov	ax,[bp-10h]
	mov	cl,4h
	call	8CAAh
	push	ax
	call	40BFh
	add	sp,6h
	mov	word ptr [bp-22h],0h

l0800_6150:
	mov	es,[bp-0Ah]
	mov	ax,es:[di+2h]
	mov	dx,es:[di]
	sub	dx,[bp-10h]
	sbb	ax,[bp-0Eh]
	mov	[bp-16h],ax
	mov	[bp-18h],dx
	add	[bp-10h],dx
	adc	[bp-0Eh],ax
	cmp	word ptr [bp-16h],0h
	ja	61B9h

l0800_6172:
	jnz	617Bh

l0800_6174:
	cmp	word ptr [bp-18h],0FFh
	ja	61B9h

l0800_617B:
	mov	ax,[bp-0Eh]
	mov	dx,[bp-10h]
	sub	dx,[bp-14h]
	sbb	ax,[bp-12h]
	or	ax,ax
	ja	61B9h

l0800_618B:
	jc	6192h

l0800_618D:
	cmp	dx,0F0h
	jnc	61B9h

l0800_6192:
	cmp	word ptr [bp-22h],0FFh
	jz	61B9h

l0800_6199:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,[bp-18h]
	push	ax
	call	4047h
	add	sp,6h
	inc	word ptr [bp-22h]
	add	di,4h
	inc	si
	mov	ax,si
	cmp	ax,[bp-24h]
	jnz	6150h

l0800_61B9:
	mov	ax,1h
	push	ax
	mov	ax,[bp-22h]
	add	ax,3h
	mov	dx,[bp-38h]
	neg	ax
	sbb	dx,0h
	push	dx
	push	ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,[bp-22h]
	push	ax
	call	4047h
	add	sp,6h
	mov	ax,1h
	push	ax
	mov	ax,[bp-22h]
	xor	dx,dx
	add	ax,2h
	adc	dx,0h
	push	dx
	push	ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah

l0800_620C:
	cmp	si,[bp-24h]
	jz	6214h

l0800_6211:
	jmp	6104h

l0800_6214:
	push	word ptr [bp-0Ah]
	push	word ptr [bp-0Ch]
	call	4346h
	add	sp,4h

l0800_6220:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,0h
	push	ax
	call	4047h
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	add	ax,0FFE0h
	adc	dx,0FFh
	mov	[bp-1Ah],dx
	mov	[bp-1Ch],ax
	jmp	6266h

l0800_624D:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,0h
	push	ax
	call	4047h
	add	sp,6h
	add	word ptr [bp-1Ch],1h
	adc	word ptr [bp-1Ah],0h

l0800_6266:
	mov	ax,[bp-1Ch]
	and	ax,0Fh
	or	ax,0h
	jnz	624Dh

l0800_6271:
	mov	ax,[bp-26h]
	mov	cl,4h
	shl	ax,cl
	sub	[bp-4h],ax
	sbb	word ptr [bp-2h],0h
	xor	ax,ax
	push	ax
	mov	ax,[bp-26h]
	shl	ax,cl
	xor	dx,dx
	push	dx
	push	ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	75EAh
	add	sp,8h
	xor	ax,ax
	push	ax
	mov	dx,20h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-30h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-32h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-2Eh]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-2Ch]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-1Ch]
	call	40BFh
	add	sp,6h
	mov	dx,[2A05h]
	mov	ax,[2A03h]
	mov	cl,4h
	call	8CAAh
	mov	[bp-22h],ax
	mov	ax,[2A03h]
	and	ax,0Fh
	or	ax,0h
	jz	6333h

l0800_6330:
	inc	word ptr [bp-22h]

l0800_6333:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-22h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	dx,[bp-1Ah]
	mov	ax,[bp-1Ch]
	mov	cl,4h
	call	8CAAh
	mov	dx,[bp-22h]
	add	dx,ax
	push	dx
	call	40BFh
	add	sp,6h
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	sub	dx,[2A03h]
	sbb	ax,[2A05h]
	add	dx,[4E88h]
	adc	ax,[4E8Ah]
	add	dx,80h
	adc	ax,0h
	mov	[bp-6h],ax
	mov	[bp-8h],dx
	cmp	word ptr [2A1Dh],0h
	jz	6398h

l0800_638D:
	add	word ptr [bp-8h],200h
	adc	word ptr [bp-6h],0h
	jmp	63A8h

l0800_6398:
	cmp	word ptr [2A21h],1h
	jnz	63A8h

l0800_639F:
	add	word ptr [bp-8h],180h
	adc	word ptr [bp-6h],0h

l0800_63A8:
	xor	ax,ax
	push	ax
	mov	ax,[bp-26h]
	mov	cl,4h
	shl	ax,cl
	mov	dx,[bp-2h]
	mov	bx,[bp-4h]
	add	bx,ax
	adc	dx,0h
	push	dx
	push	bx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	mov	ax,2h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-1Eh],dx
	mov	[bp-20h],ax
	mov	ax,[bp-26h]
	mov	cl,4h
	shl	ax,cl
	mov	dx,[2A09h]
	mov	bx,[2A07h]
	sub	bx,[bp-4h]
	sbb	dx,[bp-2h]
	sub	bx,ax
	sbb	dx,0h
	push	dx
	push	bx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	mov	ax,[bp-20h]
	and	ax,1FFh
	mov	[bp-22h],ax
	mov	dx,[bp-1Eh]
	mov	ax,[bp-20h]
	mov	cl,9h
	call	8CAAh
	mov	di,ax
	cmp	word ptr [bp-22h],0h
	jz	6446h

l0800_6445:
	inc	di

l0800_6446:
	xor	ax,ax
	push	ax
	mov	dx,2h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-22h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	di
	call	40BFh
	add	sp,6h
	cmp	word ptr [2A1Dh],0h
	jz	64B9h

l0800_6483:
	xor	ax,ax
	push	ax
	mov	dx,2Eh
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-22h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	di
	call	40BFh
	add	sp,6h

l0800_64B9:
	xor	ax,ax
	push	ax
	mov	dx,18h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,[2E75h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,[2E76h]
	mov	dh,0h
	add	ax,dx
	sub	ax,20h
	push	ax
	call	40BFh
	add	sp,6h
	xor	ax,ax
	push	ax
	mov	dx,1Ch
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-28h]
	call	40BFh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-2Ah]
	call	40BFh
	add	sp,6h
	xor	ax,ax
	push	ax
	mov	dx,0Ah
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	mov	ax,[bp-28h]
	mov	cl,4h
	shl	ax,cl
	mov	dx,[bp-2h]
	mov	bx,[bp-4h]
	add	bx,ax
	adc	dx,0h
	mov	ax,[bp-1Eh]
	mov	cx,[bp-20h]
	sub	cx,20h
	sbb	ax,0h
	add	cx,[bp-8h]
	adc	ax,[bp-6h]
	cmp	dx,ax
	jc	659Ch

l0800_6568:
	ja	656Eh

l0800_656A:
	cmp	bx,cx
	jbe	659Ch

l0800_656E:
	mov	ax,[bp-28h]
	mov	cl,4h
	shl	ax,cl
	mov	dx,[bp-2h]
	mov	bx,[bp-4h]
	add	bx,ax
	adc	dx,0h
	mov	ax,[bp-1Eh]
	mov	cx,[bp-20h]
	sub	cx,20h
	sbb	ax,0h
	add	cx,[bp-8h]
	adc	ax,[bp-6h]
	sub	bx,cx
	sbb	dx,ax
	add	[bp-8h],bx
	adc	[bp-6h],dx

l0800_659C:
	xor	ax,ax
	mov	dx,10h
	sub	dx,[bp-8h]
	sbb	ax,[bp-6h]
	and	dx,0Fh
	add	[bp-8h],dx
	adc	word ptr [bp-6h],0h
	mov	dx,[bp-6h]
	mov	ax,[bp-8h]
	mov	cl,4h
	call	8CAAh
	mov	[bp-28h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	ax
	call	40BFh
	add	sp,6h
	mov	ax,[bp-2Ah]
	cmp	ax,[bp-28h]
	jnc	65DCh

l0800_65D6:
	mov	ax,[bp-28h]
	mov	[bp-2Ah],ax

l0800_65DC:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-2Ah]
	call	40BFh
	add	sp,6h
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
	add	ax,[4E88h]
	adc	dx,[4E8Ah]
	add	ax,20h
	adc	dx,0h
	mov	cl,4h
	call	8CAAh
	mov	[bp-2Ch],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	ax
	call	40BFh
	add	sp,6h
	mov	ax,[bp-1Ch]
	add	ax,80h
	mov	[bp-2Eh],ax
	cmp	word ptr [2A1Dh],0h
	jz	662Fh

l0800_6628:
	add	word ptr [bp-2Eh],200h
	jmp	663Bh

l0800_662F:
	cmp	word ptr [2A21h],1h
	jnz	663Bh

l0800_6636:
	add	word ptr [bp-2Eh],180h

l0800_663B:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-2Eh]
	call	40BFh
	add	sp,6h
	mov	ax,[bp-26h]
	mov	cl,4h
	shl	ax,cl
	mov	dx,[2A09h]
	mov	bx,[2A07h]
	sub	bx,[bp-4h]
	sbb	dx,[bp-2h]
	sub	bx,ax
	sbb	dx,0h
	or	bx,dx
	jz	6673h

l0800_666A:
	mov	ax,1h
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
	sub	sp,4h
	push	si
	push	di
	cmp	word ptr [2A09h],0h
	jc	66BEh

l0800_66AB:
	ja	66B5h

l0800_66AD:
	cmp	word ptr [2A07h],0FEFEh
	jbe	66BEh

l0800_66B5:
	mov	ax,3h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_66BE:
	push	ds
	pop	es
	mov	di,2206h
	mov	si,4271h
	mov	cx,4h
	mov	dx,3h
	cmp	dx,cx
	jnc	66D2h

l0800_66D0:
	mov	cx,dx

l0800_66D2:
	xor	ax,ax
	rep cmpsb
	jz	66DDh

l0800_66D8:
	sbb	ax,ax
	sbb	ax,0FFFFh

l0800_66DD:
	or	ax,ax
	jz	6715h

l0800_66E1:
	push	ds
	pop	es
	mov	di,4271h
	mov	si,2206h
	mov	dx,3h
	mov	cx,4h
	sub	dx,cx
	jnc	66F7h

l0800_66F3:
	add	cx,dx
	xor	dx,dx

l0800_66F7:
	shr	cx,1h
	rep movsw
	adc	cx,cx
	rep movsb
	mov	cx,dx
	xor	ax,ax

l0800_6703:
	rep stosb

l0800_6705:
	push	ds
	mov	ax,2E75h
	push	ax
	push	ds
	mov	ax,4271h
	push	ax
	call	09A3h
	add	sp,8h

l0800_6715:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,[2E75h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,[2E76h]
	mov	dh,0h
	add	ax,dx
	xor	dx,dx
	push	dx
	push	ax
	push	ds
	mov	ax,2E77h
	push	ax
	call	4152h
	add	sp,0Ch
	mov	al,[2E75h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,[2E76h]
	mov	dh,0h
	add	ax,dx
	mov	dx,[2A09h]
	mov	bx,[2A07h]
	add	bx,ax
	adc	dx,0h
	add	bx,40h
	adc	dx,0h
	mov	[bp-2h],dx
	mov	[bp-4h],bx
	cmp	word ptr [2A1Dh],0h
	jz	6779h

l0800_676E:
	add	word ptr [bp-4h],200h
	adc	word ptr [bp-2h],0h
	jmp	6789h

l0800_6779:
	cmp	word ptr [2A21h],1h
	jnz	6789h

l0800_6780:
	add	word ptr [bp-4h],180h
	adc	word ptr [bp-2h],0h

l0800_6789:
	cmp	word ptr [bp-2h],0h
	jc	67A1h

l0800_678F:
	ja	6798h

l0800_6791:
	cmp	word ptr [bp-4h],0FEFEh
	jbe	67A1h

l0800_6798:
	mov	ax,3h
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_67A1:
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	call	75EAh
	add	sp,8h
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
	sub	sp,0Eh
	mov	al,[2E75h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,[2E76h]
	mov	dh,0h
	add	ax,dx
	mov	[bp-0Eh],ax
	xor	dx,dx
	cmp	dx,[2A09h]
	jc	67F0h

l0800_67E1:
	jnz	67E9h

l0800_67E3:
	cmp	ax,[2A07h]
	jc	67F0h

l0800_67E9:
	mov	ax,3h
	mov	sp,bp
	pop	bp
	ret

l0800_67F0:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	push	word ptr [bp-0Eh]
	push	ds
	mov	ax,2E77h
	push	ax
	call	4152h
	add	sp,0Ch
	mov	ax,1h
	push	ax
	mov	ax,0FFFFh
	mov	dx,0FFEEh
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F0Ah
	add	sp,4h
	cmp	ax,601Ah
	jz	6839h

l0800_6836:
	jmp	68F1h

l0800_6839:
	mov	ax,1h
	push	ax
	xor	ax,ax
	mov	dx,2h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	xor	ax,ax
	mov	dx,10h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	xor	ax,ax
	push	ax
	mov	dx,1Ah
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E27h
	add	sp,4h
	push	ax
	call	401Eh
	add	sp,6h
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	sub	dx,1Ch
	sbb	ax,0h
	push	ax
	push	dx
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	sub	dx,1Ch
	sbb	ax,0h
	push	ax
	push	dx
	call	75EAh
	add	sp,8h
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	sub	dx,1Ch
	sbb	ax,0h
	sub	dx,[2A03h]
	sbb	ax,[2A05h]
	add	dx,[4E88h]
	adc	ax,[4E8Ah]
	add	dx,0Eh
	adc	ax,0h
	mov	[bp-6h],ax
	mov	[bp-8h],dx
	jmp	6959h

l0800_68F1:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	call	409Ch
	add	sp,8h
	mov	ax,1h
	push	ax
	xor	ax,ax
	mov	dx,0Eh
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	call	75EAh
	add	sp,8h
	mov	ax,[2A09h]
	mov	dx,[2A07h]
	sub	dx,[2A03h]
	sbb	ax,[2A05h]
	add	dx,[4E88h]
	adc	ax,[4E8Ah]
	add	dx,0Eh
	adc	ax,0h
	mov	[bp-6h],ax
	mov	[bp-8h],dx

l0800_6959:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	and	ax,1h
	or	ax,0h
	jz	6988h

l0800_696F:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,90h
	push	ax
	call	4047h
	add	sp,6h
	add	word ptr [bp-8h],1h
	adc	word ptr [bp-6h],0h

l0800_6988:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	add	ax,0FFE0h
	adc	dx,0FFh
	mov	[2A05h],dx
	mov	[2A03h],ax
	xor	ax,ax
	push	ax
	mov	dx,2h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	3F58h
	add	sp,4h
	sub	[bp-8h],ax
	sbb	[bp-6h],dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [2A05h]
	push	word ptr [2A03h]
	call	409Ch
	add	sp,8h
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E27h
	add	sp,4h
	cmp	ax,601Ah
	jz	6A1Eh

l0800_6A1B:
	jmp	6AA4h

l0800_6A1E:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	pop	bx
	pop	cx
	add	cx,ax
	adc	bx,dx
	mov	[bp-2h],bx
	mov	[bp-4h],cx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-0Ah],dx
	mov	[bp-0Ch],ax
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	add	dx,[bp-0Ch]
	adc	ax,[bp-0Ah]
	mov	bx,[2A05h]
	mov	cx,[2A03h]
	add	cx,[bp-8h]
	adc	bx,[bp-6h]
	cmp	ax,bx
	jc	6AA4h

l0800_6A7A:
	ja	6A80h

l0800_6A7C:
	cmp	dx,cx
	jbe	6AA4h

l0800_6A80:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	add	dx,[bp-0Ch]
	adc	ax,[bp-0Ah]
	mov	bx,[2A05h]
	mov	cx,[2A03h]
	add	cx,[bp-8h]
	adc	bx,[bp-6h]
	sub	dx,cx
	sbb	ax,bx
	add	[bp-8h],dx
	adc	[bp-6h],ax

l0800_6AA4:
	xor	ax,ax
	push	ax
	mov	dx,0Ah
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	call	409Ch
	add	sp,8h
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
	sub	sp,18h
	push	si
	mov	al,[2E75h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,[2E76h]
	mov	dh,0h
	add	ax,dx
	xor	dx,dx
	cmp	dx,[2A09h]
	jc	6B04h

l0800_6AF4:
	jnz	6AFCh

l0800_6AF6:
	cmp	ax,[2A07h]
	jc	6B04h

l0800_6AFC:
	mov	ax,3h
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_6B04:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	or	dx,dx
	jnz	6B1Bh

l0800_6B16:
	cmp	ax,3F3h
	jz	6B23h

l0800_6B1B:
	mov	ax,3h
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_6B23:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	mov	dx,3F3h
	push	ax
	push	dx
	call	409Ch
	add	sp,8h

l0800_6B38:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-2h]
	push	ax
	call	409Ch
	add	sp,8h
	jmp	6B84h

l0800_6B66:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	push	dx
	push	ax
	call	409Ch
	add	sp,8h

l0800_6B84:
	mov	ax,[bp-8h]
	mov	dx,[bp-6h]
	sub	word ptr [bp-8h],1h
	sbb	word ptr [bp-6h],0h
	or	ax,dx
	jnz	6B66h

l0800_6B96:
	mov	ax,[bp-4h]
	or	ax,[bp-2h]
	jnz	6B38h

l0800_6B9E:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	add	ax,1h
	adc	dx,0h
	push	dx
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-0Ah],dx
	mov	[bp-0Ch],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-0Eh],dx
	mov	[bp-10h],ax
	mov	ax,[bp-0Eh]
	mov	dx,[bp-10h]
	sub	dx,[bp-0Ch]
	sbb	ax,[bp-0Ah]
	add	dx,1h
	adc	ax,0h
	mov	[bp-12h],ax
	mov	[bp-14h],dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-0Ah]
	push	word ptr [bp-0Ch]
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	ax,[bp-0Eh]
	mov	dx,[bp-10h]
	add	dx,1h
	adc	ax,0h
	push	ax
	push	dx
	call	409Ch
	add	sp,8h
	mov	al,[2E75h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,[2E76h]
	mov	dh,0h
	add	ax,dx
	sub	ax,0Ch
	shr	ax,1h
	shr	ax,1h
	mov	word ptr [bp-2h],0h
	mov	[bp-4h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	push	word ptr [bp-4h]
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	[4E8Eh],dx
	mov	[4E8Ch],ax
	mov	ax,[bp-12h]
	mov	dx,[bp-14h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	jmp	6CAFh

l0800_6C89:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	push	dx
	push	ax
	call	409Ch
	add	sp,8h
	sub	word ptr [bp-4h],1h
	sbb	word ptr [bp-2h],0h

l0800_6CAF:
	mov	ax,[bp-4h]
	or	ax,[bp-2h]
	jnz	6C89h

l0800_6CB7:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,[2E75h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,[2E76h]
	mov	dh,0h
	add	ax,dx
	xor	dx,dx
	push	dx
	push	ax
	push	ds
	mov	ax,2E77h
	push	ax
	call	4152h
	add	sp,0Ch
	xor	si,si
	jmp	6E93h

l0800_6CE4:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	mov	ax,[bp-6h]
	mov	dx,[bp-8h]
	and	ax,3FFFh
	mov	[bp-18h],dx
	mov	[bp-16h],ax
	mov	cx,7h
	mov	bx,6EBCh

l0800_6D0D:
	mov	ax,cs:[bx]
	cmp	ax,[bp-8h]
	jnz	6D1Eh

l0800_6D15:
	mov	ax,cs:[bx+0Eh]
	cmp	ax,[bp-16h]
	jz	6D26h

l0800_6D1E:
	add	bx,2h
	loop	6D0Dh

l0800_6D23:
	jmp	6E8Bh

l0800_6D26:
	jmp	word ptr cs:[bx+1Ch]
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
	mov	ax,9h
	pop	si
	mov	sp,bp
	pop	bp
	ret

l0800_6E93:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	cmp	dx,[2A09h]
	jnc	6EAAh

l0800_6EA7:
	jmp	6CE4h

l0800_6EAA:
	jnz	6EB5h

l0800_6EAC:
	cmp	ax,[2A07h]
	jnc	6EB5h

l0800_6EB2:
	jmp	6CE4h

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
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	ds
	mov	ax,2223h
	push	ax
	call	0B2EFh
	add	sp,0Ah
	pop	bp
	ret

;; fn0800_6EFF: 0800:6EFF
fn0800_6EFF proc
	push	bp
	mov	bp,sp
	push	ds
	mov	ax,2231h
	push	ax
	call	0B2EFh
	add	sp,4h
	cmp	word ptr [bp+4h],9h
	jbe	6F1Eh

l0800_6F13:
	push	ds
	mov	ax,223Eh
	push	ax
	call	0B2EFh
	add	sp,4h

l0800_6F1E:
	pop	bp
	ret

;; fn0800_6F20: 0800:6F20
fn0800_6F20 proc
	push	bp
	mov	bp,sp
	sub	sp,1Ah
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	cl,2h
	call	8C69h
	mov	[bp-0Eh],dx
	mov	[bp-10h],ax
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-12h],dx
	mov	[bp-14h],ax
	mov	ax,1h
	push	ax
	push	word ptr [bp-0Eh]
	push	word ptr [bp-10h]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	jmp	6FCCh

l0800_6F6D:
	mov	ax,1h
	push	ax
	xor	ax,ax
	mov	dx,4h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah

l0800_6F86:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	cl,2h
	call	8C69h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	or	ax,[bp-2h]
	jz	6FC4h

l0800_6FA4:
	mov	ax,1h
	push	ax
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	add	dx,4h
	adc	ax,0h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah

l0800_6FC4:
	mov	ax,[bp-4h]
	or	ax,[bp-2h]
	jnz	6F86h

l0800_6FCC:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F58h
	add	sp,4h
	or	dx,dx
	jnz	6FE3h

l0800_6FDE:
	cmp	ax,3ECh
	jz	6F6Dh

l0800_6FE3:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	sub	ax,[bp-14h]
	sbb	dx,[bp-12h]
	mov	[bp-16h],dx
	mov	[bp-18h],ax
	mov	[2A05h],dx
	mov	[2A03h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	cmp	word ptr [bp-16h],0h
	jnc	7035h

l0800_7032:
	jmp	70F9h

l0800_7035:
	ja	7040h

l0800_7037:
	cmp	word ptr [bp-18h],12h
	ja	7040h

l0800_703D:
	jmp	70F9h

l0800_7040:
	xor	ax,ax
	push	ax
	push	word ptr [4E8Eh]
	push	word ptr [4E8Ch]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	3E5Dh
	add	sp,4h
	mov	[bp-0Ah],dx
	mov	[bp-0Ch],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-0Eh]
	push	word ptr [bp-10h]
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-0Ah]
	push	word ptr [bp-0Ch]
	call	409Ch
	add	sp,8h
	xor	ax,ax
	push	ax
	push	word ptr [bp-12h]
	push	word ptr [bp-14h]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	mov	ax,[bp-16h]
	mov	dx,[bp-18h]
	sub	dx,8h
	sbb	ax,0h
	push	ax
	push	dx
	push	word ptr [bp-16h]
	push	word ptr [bp-18h]
	call	75EAh
	add	sp,8h
	add	word ptr [2A03h],8h
	adc	word ptr [2A05h],0h

l0800_70F9:
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	cmp	ax,[bp-16h]
	jnc	7108h

l0800_7105:
	jmp	724Dh

l0800_7108:
	jnz	7112h

l0800_710A:
	cmp	dx,[bp-18h]
	jnc	7112h

l0800_710F:
	jmp	724Dh

l0800_7112:
	xor	ax,ax
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	xor	ax,ax
	push	ax
	mov	ax,[bp-12h]
	mov	dx,[bp-14h]
	sub	dx,4h
	sbb	ax,0h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	mov	ax,[bp-0Eh]
	mov	dx,[bp-10h]
	add	dx,4h
	adc	ax,0h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3F58h
	add	sp,4h
	or	dx,dx
	jz	7181h

l0800_717E:
	jmp	723Fh

l0800_7181:
	cmp	ax,3ECh
	jz	7189h

l0800_7186:
	jmp	723Fh

l0800_7189:
	mov	ax,1h
	push	ax
	xor	ax,ax
	mov	dx,4h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	mov	dx,3ECh
	push	ax
	push	dx
	call	409Ch
	add	sp,8h

l0800_71B7:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-2h]
	push	ax
	call	409Ch
	add	sp,8h
	mov	ax,[bp-4h]
	or	ax,[bp-2h]
	jz	7234h

l0800_71E5:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	add	ax,1h
	adc	dx,0h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-6h]
	push	ax
	call	409Ch
	add	sp,8h
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
	mov	cl,2h
	call	8C69h
	push	dx
	push	ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch

l0800_7234:
	mov	ax,[bp-4h]
	or	ax,[bp-2h]
	jz	723Fh

l0800_723C:
	jmp	71B7h

l0800_723F:
	add	word ptr [4E8Ch],4h
	adc	word ptr [4E8Eh],0h
	mov	sp,bp
	pop	bp
	ret

l0800_724D:
	mov	ax,[bp-16h]
	mov	dx,[bp-18h]
	add	dx,[4E88h]
	adc	ax,[4E8Ah]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	mov	ax,[2A03h]
	and	ax,3h
	or	ax,0h
	jz	7274h

l0800_726C:
	add	word ptr [bp-4h],2h
	adc	word ptr [bp-2h],0h

l0800_7274:
	mov	ax,[bp-4h]
	and	ax,3h
	or	ax,0h
	jz	7298h

l0800_727F:
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
	mov	cl,2h
	call	8CAAh
	add	ax,1h
	adc	dx,0h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	jmp	72C6h

l0800_7298:
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
	mov	cl,2h
	call	8CAAh
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	jmp	72C6h

l0800_72AB:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,0h
	push	ax
	call	4047h
	add	sp,6h
	add	word ptr [2A03h],1h
	adc	word ptr [2A05h],0h

l0800_72C6:
	mov	ax,[2A03h]
	and	ax,3h
	or	ax,0h
	jnz	72ABh

l0800_72D1:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-6h],dx
	mov	[bp-8h],ax
	mov	ax,1h
	push	ax
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	neg	ax
	neg	dx
	sbb	ax,0h
	sub	dx,4h
	sbb	ax,0h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	dx,[2A05h]
	mov	ax,[2A03h]
	mov	cl,2h
	call	8CAAh
	push	dx
	push	ax
	call	409Ch
	add	sp,8h
	mov	ax,[bp-0Ah]
	mov	dx,[bp-0Ch]
	and	ax,3FFFh
	mov	[bp-1Ah],ax
	cmp	ax,[bp-2h]
	jc	734Dh

l0800_733A:
	ja	7341h

l0800_733C:
	cmp	dx,[bp-4h]
	jbe	734Dh

l0800_7341:
	mov	ax,[bp-0Ch]
	mov	dx,[bp-1Ah]
	mov	[bp-2h],dx
	mov	[bp-4h],ax

l0800_734D:
	mov	ax,[bp-0Ah]
	and	ax,0C000h
	or	word ptr [bp-4h],0h
	or	[bp-2h],ax
	xor	ax,ax
	push	ax
	push	word ptr [4E8Eh]
	push	word ptr [4E8Ch]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	409Ch
	add	sp,8h
	xor	ax,ax
	push	ax
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	add	word ptr [4E8Ch],4h
	adc	word ptr [4E8Eh],0h
	mov	sp,bp
	pop	bp
	ret

;; fn0800_73AC: 0800:73AC
;;   Called from:
;;     0800:0FFB (in fn0800_0DE8)
fn0800_73AC proc
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,[2E75h]
	mov	ah,0h
	mov	cl,8h
	shl	ax,cl
	mov	dl,[2E76h]
	mov	dh,0h
	add	ax,dx
	xor	dx,dx
	push	dx
	push	ax
	push	ds
	mov	ax,2E77h
	push	ax
	call	4152h
	add	sp,0Ch
	mov	ax,1h
	push	ax
	mov	ax,0FFFFh
	mov	dx,0FFFCh
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [2A11h]
	push	word ptr [2A0Fh]
	call	409Ch
	add	sp,8h
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	push	word ptr [2A09h]
	push	word ptr [2A07h]
	call	75EAh
	add	sp,8h
	xor	ax,ax
	ret

;; fn0800_741D: 0800:741D
;;   Called from:
;;     0800:1002 (in fn0800_0DE8)
fn0800_741D proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	push	si
	jmp	75C1h

l0800_7427:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E9Ah
	add	sp,4h
	or	ax,ax
	jz	7449h

l0800_7439:
	cmp	ax,1h
	jz	7469h

l0800_743E:
	cmp	ax,3h
	jnz	7446h

l0800_7443:
	jmp	7566h

l0800_7446:
	jmp	758Fh

l0800_7449:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3DCFh
	add	sp,4h
	push	ax
	call	4047h
	add	sp,6h
	jmp	75C1h

l0800_7469:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3E5Dh
	add	sp,4h
	push	dx
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3FADh
	add	sp,4h
	mov	word ptr [bp-2h],0h
	mov	[bp-4h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	call	401Eh
	add	sp,6h
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	75EAh
	add	sp,8h
	mov	ax,1h
	push	ax
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	add	dx,2h
	adc	ax,0h
	neg	ax
	neg	dx
	sbb	ax,0h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [2A03h]
	call	40BFh
	add	sp,6h
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	cmp	ax,[bp-2h]
	jnz	754Ah

l0800_7506:
	cmp	dx,[bp-4h]
	jnz	754Ah

l0800_750B:
	mov	ax,1h
	push	ax
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	neg	ax
	neg	dx
	sbb	ax,0h
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch
	jmp	75C1h

l0800_754A:
	mov	ax,1h
	push	ax
	push	word ptr [2A05h]
	push	word ptr [2A03h]
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	jmp	75C1h

l0800_7566:
	xor	si,si
	jmp	7588h

l0800_756A:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3DCFh
	add	sp,4h
	push	ax
	call	4047h
	add	sp,6h
	inc	si

l0800_7588:
	cmp	si,3h
	jnz	756Ah

l0800_758D:
	jmp	75C1h

l0800_758F:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	mov	bx,[2A09h]
	mov	cx,[2A07h]
	sub	cx,ax
	sbb	bx,dx
	push	bx
	push	cx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3B0Ah
	add	sp,0Ch

l0800_75C1:
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	cmp	dx,[2A09h]
	jnc	75D8h

l0800_75D5:
	jmp	7427h

l0800_75D8:
	jnz	75E3h

l0800_75DA:
	cmp	ax,[2A07h]
	jnc	75E3h

l0800_75E0:
	jmp	7427h

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
	sub	sp,16h
	push	si
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	mov	[4680h],ax
	mov	[467Eh],dx
	mov	[2A05h],ax
	mov	[2A03h],dx
	mov	[4678h],ax
	mov	[4676h],dx
	mov	ax,[bp+0Ah]
	mov	dx,[bp+8h]
	sub	dx,12h
	sbb	ax,0h
	mov	[4E86h],ax
	mov	[4E84h],dx
	cmp	word ptr [4680h],0h
	ja	7634h

l0800_7626:
	jz	762Bh

l0800_7628:
	jmp	79FDh

l0800_762B:
	cmp	word ptr [bp+4h],12h
	ja	7634h

l0800_7631:
	jmp	79FDh

l0800_7634:
	mov	word ptr [2E4Bh],0h
	mov	word ptr [2E49h],0h
	mov	word ptr [465Ch],0h
	mov	word ptr [4668h],0h
	mov	word ptr [4666h],0h
	mov	word ptr [2A05h],0h
	mov	word ptr [2A03h],0h
	mov	word ptr [2A01h],0h
	mov	word ptr [29FFh],0h
	mov	word ptr [467Ch],0h
	mov	word ptr [467Ah],0h
	mov	word ptr [4674h],0h
	mov	word ptr [4672h],0h
	mov	word ptr [2E43h],0h
	mov	word ptr [2E45h],0h
	mov	word ptr [4E82h],0h
	mov	word ptr [4E8Ah],0h
	mov	word ptr [4E88h],0h
	mov	word ptr [29F9h],0h
	mov	word ptr [29F7h],0h
	mov	ax,1h
	mov	dx,0Fh
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[bp-14h],dx
	mov	[bp-16h],ax
	inc	dx
	mov	[2E73h],dx
	mov	word ptr [2E71h],0h
	mov	ax,1h
	mov	dx,10h
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[bp-10h],dx
	mov	[bp-12h],ax
	inc	dx
	mov	[2E39h],dx
	mov	word ptr [2E37h],0h
	mov	ax,1h
	mov	dx,10h
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[bp-0Ch],dx
	mov	[bp-0Eh],ax
	inc	dx
	mov	[2E35h],dx
	mov	word ptr [2E33h],0h
	mov	ax,1h
	mov	dx,10h
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[bp-8h],dx
	mov	[bp-0Ah],ax
	inc	dx
	mov	[2E41h],dx
	mov	word ptr [2E3Fh],0h
	mov	ax,1h
	mov	dx,10h
	push	ax
	push	dx
	call	4311h
	add	sp,4h
	mov	[bp-4h],dx
	mov	[bp-6h],ax
	inc	dx
	mov	[2E3Dh],dx
	mov	word ptr [2E3Bh],0h
	call	87F8h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	mov	[4664h],dx
	mov	[4662h],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	mov	[4660h],dx
	mov	[465Eh],ax
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	ax,[2A21h]
	cwd
	add	ax,4300h
	adc	dx,524Eh
	push	dx
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [4680h]
	push	word ptr [467Eh]
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	push	ax
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	call	401Eh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	call	401Eh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	xor	ax,ax
	push	ax
	call	401Eh
	add	sp,6h
	push	ds
	mov	ax,2240h
	push	ax
	call	0B2EFh
	add	sp,4h
	mov	ax,[2E4Fh]
	mov	[bp-2h],ax
	mov	ax,1h
	push	ax
	cmp	word ptr [2A1Fh],0h
	jz	7808h

l0800_7806:
	jmp	780Ah

l0800_7808:
	xor	ax,ax

l0800_780A:
	push	ax
	call	8465h
	add	sp,4h
	mov	ax,1h
	push	ax
	cmp	word ptr [2E4Fh],0h
	jz	781Eh

l0800_781C:
	jmp	7820h

l0800_781E:
	xor	ax,ax

l0800_7820:
	push	ax
	call	8465h
	add	sp,4h
	cmp	word ptr [2A23h],1h
	jz	7843h

l0800_782E:
	cmp	word ptr [2E4Fh],0h
	jz	7843h

l0800_7835:
	mov	ax,10h
	push	ax
	push	word ptr [2E4Fh]
	call	8465h
	add	sp,4h

l0800_7843:
	mov	ax,[2A21h]
	cmp	ax,1h
	jz	7852h

l0800_784B:
	cmp	ax,2h
	jz	7857h

l0800_7850:
	jmp	785Ah

l0800_7852:
	call	7A02h
	jmp	785Ah

l0800_7857:
	call	7C78h

l0800_785A:
	xor	si,si
	jmp	7870h

l0800_785E:
	mov	bx,si
	inc	si
	mov	al,[bx+4682h]
	push	ax
	call	8624h
	add	sp,2h
	dec	word ptr [4E82h]

l0800_7870:
	cmp	word ptr [4E82h],0h
	jnz	785Eh

l0800_7877:
	mov	ax,[bp-2h]
	mov	[2E4Fh],ax
	mov	ax,[4680h]
	mov	dx,[467Eh]
	sub	dx,[2A03h]
	sbb	ax,[2A05h]
	cmp	ax,[4E8Ah]
	ja	78B3h

l0800_7892:
	jc	789Ah

l0800_7894:
	cmp	dx,[4E88h]
	jnc	78B3h

l0800_789A:
	mov	ax,[4680h]
	mov	dx,[467Eh]
	sub	dx,[2A03h]
	sbb	ax,[2A05h]
	sub	[4E88h],dx
	sbb	[4E8Ah],ax
	jmp	78BFh

l0800_78B3:
	mov	word ptr [4E8Ah],0h
	mov	word ptr [4E88h],0h

l0800_78BF:
	cmp	word ptr [2A21h],2h
	jnz	78D0h

l0800_78C6:
	add	word ptr [4E88h],2h
	adc	word ptr [4E8Ah],0h

l0800_78D0:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0AD2Fh
	add	sp,4h
	sub	ax,[465Eh]
	sbb	dx,[4660h]
	mov	[2A05h],dx
	mov	[2A03h],ax
	xor	ax,ax
	push	ax
	mov	ax,[4660h]
	mov	dx,[465Eh]
	add	dx,8h
	adc	ax,0h
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	sub	dx,12h
	sbb	ax,0h
	push	ax
	push	dx
	call	409Ch
	add	sp,8h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [2E4Bh]
	call	401Eh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	push	word ptr [2E49h]
	call	401Eh
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,[4E88h]
	push	ax
	call	4047h
	add	sp,6h
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,[29F7h]
	push	ax
	call	4047h
	add	sp,6h
	xor	ax,ax
	push	ax
	mov	ax,[4660h]
	mov	dx,[465Eh]
	add	dx,[2A03h]
	adc	ax,[2A05h]
	push	ax
	push	dx
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	call	0ACB3h
	add	sp,0Ah
	xor	ax,ax
	push	ax
	mov	ax,[4664h]
	mov	dx,[4662h]
	add	dx,[467Eh]
	adc	ax,[4680h]
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	push	word ptr [bp-14h]
	push	word ptr [bp-16h]
	call	4346h
	add	sp,4h
	push	word ptr [bp-10h]
	push	word ptr [bp-12h]
	call	4346h
	add	sp,4h
	push	word ptr [bp-0Ch]
	push	word ptr [bp-0Eh]
	call	4346h
	add	sp,4h
	push	word ptr [bp-8h]
	push	word ptr [bp-0Ah]
	call	4346h
	add	sp,4h
	push	word ptr [bp-4h]
	push	word ptr [bp-6h]
	call	4346h
	add	sp,4h
	push	ds
	mov	ax,223Ch
	push	ax
	call	0B2EFh
	add	sp,4h

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
	sub	sp,4h
	push	si
	mov	ax,[4664h]
	mov	dx,[4662h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	jmp	7C1Ah

l0800_7A19:
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,284Fh
	push	ax
	call	43D4h
	add	sp,6h
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,278Fh
	push	ax
	call	43D4h
	add	sp,6h
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,290Fh
	push	ax
	call	43D4h
	add	sp,6h
	call	7FDCh
	xor	ax,ax
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,290Fh
	push	ax
	call	441Ch
	add	sp,6h
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,284Fh
	push	ax
	call	441Ch
	add	sp,6h
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,278Fh
	push	ax
	call	441Ch
	add	sp,6h
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,290Fh
	push	ax
	call	83A1h
	add	sp,6h
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,284Fh
	push	ax
	call	83A1h
	add	sp,6h
	mov	ax,10h
	push	ax
	push	ds
	mov	ax,278Fh
	push	ax
	call	83A1h
	add	sp,6h
	mov	ax,10h
	push	ax
	push	word ptr [466Ah]
	call	8489h
	add	sp,4h
	jmp	7BC0h

l0800_7ACB:
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	3E27h
	add	sp,4h
	mov	[2E47h],ax
	add	[467Ah],ax
	adc	word ptr [467Ch],0h
	push	ds
	mov	ax,290Fh
	push	ax
	push	word ptr [2E47h]
	call	8407h
	add	sp,6h
	cmp	word ptr [2E47h],0h
	jz	7B56h

l0800_7AFB:
	cmp	word ptr [2E45h],0h
	jz	7B32h

l0800_7B02:
	jmp	7B17h

l0800_7B04:
	call	8359h
	xor	al,[2E4Fh]
	mov	bx,[4E82h]
	mov	[bx+4682h],al
	inc	word ptr [4E82h]

l0800_7B17:
	mov	ax,[2E47h]
	dec	word ptr [2E47h]
	or	ax,ax
	jnz	7B04h

l0800_7B22:
	jmp	7B3Dh

l0800_7B24:
	call	8359h
	xor	al,[2E4Fh]
	push	ax
	call	8624h
	add	sp,2h

l0800_7B32:
	mov	ax,[2E47h]
	dec	word ptr [2E47h]
	or	ax,ax
	jnz	7B24h

l0800_7B3D:
	test	word ptr [2E4Fh],1h
	jz	7B52h

l0800_7B45:
	mov	ax,[2E4Fh]
	shr	ax,1h
	or	ax,8000h
	mov	[2E4Fh],ax
	jmp	7B56h

l0800_7B52:
	shr	word ptr [2E4Fh],1h

l0800_7B56:
	mov	ax,[466Ah]
	or	ax,[466Ch]
	jz	7BC0h

l0800_7B5F:
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	3E27h
	add	sp,4h
	mov	[2E29h],ax
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	3E27h
	add	sp,4h
	mov	[2E2Bh],ax
	push	ds
	mov	ax,284Fh
	push	ax
	push	word ptr [2E2Bh]
	call	8407h
	add	sp,6h
	push	ds
	mov	ax,278Fh
	push	ax
	push	word ptr [2E29h]
	call	8407h
	add	sp,6h
	add	word ptr [2E29h],2h
	mov	ax,[2E29h]
	add	[467Ah],ax
	adc	word ptr [467Ch],0h
	jmp	7BB5h

l0800_7BB2:
	call	8359h

l0800_7BB5:
	mov	ax,[2E29h]
	dec	word ptr [2E29h]
	or	ax,ax
	jnz	7BB2h

l0800_7BC0:
	mov	ax,[466Ah]
	mov	dx,[466Ch]
	sub	word ptr [466Ah],1h
	sbb	word ptr [466Ch],0h
	or	ax,dx
	jz	7BD8h

l0800_7BD5:
	jmp	7ACBh

l0800_7BD8:
	cmp	word ptr [2E45h],0h
	jnz	7BFCh

l0800_7BDF:
	xor	si,si
	jmp	7BF5h

l0800_7BE3:
	mov	bx,si
	inc	si
	mov	al,[bx+4682h]
	push	ax
	call	8624h
	add	sp,2h
	dec	word ptr [4E82h]

l0800_7BF5:
	cmp	word ptr [4E82h],0h
	jnz	7BE3h

l0800_7BFC:
	add	word ptr [29F7h],1h
	adc	word ptr [29F9h],0h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax

l0800_7C1A:
	mov	ax,[467Ch]
	mov	dx,[467Ah]
	cmp	ax,[4680h]
	jnc	7C2Ah

l0800_7C27:
	jmp	7A19h

l0800_7C2A:
	jnz	7C35h

l0800_7C2C:
	cmp	dx,[467Eh]
	jnc	7C35h

l0800_7C32:
	jmp	7A19h

l0800_7C35:
	mov	cl,10h
	sub	cl,[2E45h]
	shr	word ptr [2E43h],cl
	cmp	word ptr [2E45h],0h
	jnz	7C4Dh

l0800_7C46:
	cmp	word ptr [4E82h],0h
	jz	7C57h

l0800_7C4D:
	mov	al,[2E43h]
	push	ax
	call	8624h
	add	sp,2h

l0800_7C57:
	cmp	word ptr [2E45h],8h
	ja	7C65h

l0800_7C5E:
	cmp	word ptr [4E82h],0h
	jz	7C73h

l0800_7C65:
	mov	ax,[2E43h]
	mov	cl,8h
	shr	ax,cl
	push	ax
	call	8624h
	add	sp,2h

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
	sub	sp,4h
	push	si
	mov	ax,[4664h]
	mov	dx,[4662h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	jmp	7E6Dh

l0800_7C8F:
	call	7FDCh
	xor	ax,ax
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	jmp	7DCDh

l0800_7CAC:
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	3E27h
	add	sp,4h
	mov	[2E47h],ax
	add	[467Ah],ax
	adc	word ptr [467Ch],0h
	push	ax
	call	7EAFh
	add	sp,2h
	mov	ax,[466Ah]
	or	ax,[466Ch]
	jnz	7CD9h

l0800_7CD6:
	jmp	7DCDh

l0800_7CD9:
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	3E27h
	add	sp,4h
	mov	[2E29h],ax
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	3E27h
	add	sp,4h
	mov	[2E2Bh],ax
	cmp	word ptr [2E29h],0h
	jnz	7D1Dh

l0800_7D02:
	mov	ax,3h
	push	ax
	mov	ax,6h
	push	ax
	call	854Bh
	add	sp,4h
	mov	al,[2E2Bh]
	push	ax
	call	8600h
	add	sp,2h
	jmp	7DACh

l0800_7D1D:
	cmp	word ptr [2E29h],7h
	jnc	7D68h

l0800_7D24:
	mov	bx,[2E29h]
	mov	al,[bx+21DBh]
	mov	ah,0h
	push	ax
	mov	al,[bx+21D4h]
	mov	ah,0h
	push	ax
	call	854Bh
	add	sp,4h
	mov	bx,[2E2Bh]
	mov	cl,8h
	shr	bx,cl
	mov	si,bx
	mov	al,[bx+21F2h]
	mov	ah,0h
	push	ax
	mov	al,[si+21E2h]
	mov	ah,0h
	push	ax
	call	854Bh
	add	sp,4h
	mov	al,[2E2Bh]
	and	al,0FFh
	push	ax
	call	8600h
	add	sp,2h
	jmp	7DACh

l0800_7D68:
	mov	ax,4h
	push	ax
	mov	ax,0Fh
	push	ax
	call	854Bh
	add	sp,4h
	mov	al,[2E29h]
	sub	al,6h
	push	ax
	call	8600h
	add	sp,2h
	mov	bx,[2E2Bh]
	mov	cl,8h
	shr	bx,cl
	mov	si,bx
	mov	al,[bx+21F2h]
	mov	ah,0h
	push	ax
	mov	al,[si+21E2h]
	mov	ah,0h
	push	ax
	call	854Bh
	add	sp,4h
	mov	al,[2E2Bh]
	and	al,0FFh
	push	ax
	call	8600h
	add	sp,2h

l0800_7DAC:
	add	word ptr [2E29h],2h
	mov	ax,[2E29h]
	add	[467Ah],ax
	adc	word ptr [467Ch],0h
	jmp	7DC2h

l0800_7DBF:
	call	8359h

l0800_7DC2:
	mov	ax,[2E29h]
	dec	word ptr [2E29h]
	or	ax,ax
	jnz	7DBFh

l0800_7DCD:
	mov	ax,[466Ah]
	mov	dx,[466Ch]
	sub	word ptr [466Ah],1h
	sbb	word ptr [466Ch],0h
	or	ax,dx
	jz	7DE5h

l0800_7DE2:
	jmp	7CACh

l0800_7DE5:
	mov	ax,4h
	push	ax
	mov	ax,0Fh
	push	ax
	call	854Bh
	add	sp,4h
	mov	al,0h
	push	ax
	call	8600h
	add	sp,2h
	mov	ax,[467Ch]
	mov	dx,[467Ah]
	cmp	ax,[4680h]
	ja	7E1Eh

l0800_7E09:
	jc	7E11h

l0800_7E0B:
	cmp	dx,[467Eh]
	jnc	7E1Eh

l0800_7E11:
	mov	ax,1h
	push	ax
	push	ax
	call	854Bh
	add	sp,4h
	jmp	7E2Bh

l0800_7E1E:
	mov	ax,1h
	push	ax
	xor	ax,ax
	push	ax
	call	854Bh
	add	sp,4h

l0800_7E2B:
	cmp	word ptr [2E45h],0h
	jnz	7E4Fh

l0800_7E32:
	xor	si,si
	jmp	7E48h

l0800_7E36:
	mov	bx,si
	inc	si
	mov	al,[bx+4682h]
	push	ax
	call	8624h
	add	sp,2h
	dec	word ptr [4E82h]

l0800_7E48:
	cmp	word ptr [4E82h],0h
	jnz	7E36h

l0800_7E4F:
	add	word ptr [29F7h],1h
	adc	word ptr [29F9h],0h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0AD2Fh
	add	sp,4h
	mov	[bp-2h],dx
	mov	[bp-4h],ax

l0800_7E6D:
	mov	ax,[467Ch]
	mov	dx,[467Ah]
	cmp	ax,[4680h]
	jnc	7E7Dh

l0800_7E7A:
	jmp	7C8Fh

l0800_7E7D:
	jnz	7E88h

l0800_7E7F:
	cmp	dx,[467Eh]
	jnc	7E88h

l0800_7E85:
	jmp	7C8Fh

l0800_7E88:
	mov	cl,8h
	sub	cl,[2E45h]
	shl	word ptr [2E43h],cl
	cmp	word ptr [2E45h],0h
	jnz	7EA0h

l0800_7E99:
	cmp	word ptr [4E82h],0h
	jz	7EAAh

l0800_7EA0:
	mov	al,[2E43h]
	push	ax
	call	8624h
	add	sp,2h

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
	mov	si,[bp+4h]
	jmp	7FD1h

l0800_7EBA:
	cmp	si,0Ch
	jnc	7F32h

l0800_7EBF:
	jmp	7EF6h

l0800_7EC1:
	mov	ax,1h
	push	ax
	xor	ax,ax
	push	ax
	call	854Bh
	add	sp,4h
	call	8359h
	xor	al,[2E4Fh]
	push	ax
	call	8600h
	add	sp,2h
	test	word ptr [2E4Fh],1h
	jz	7EF1h

l0800_7EE4:
	mov	ax,[2E4Fh]
	shr	ax,1h
	or	ax,8000h
	mov	[2E4Fh],ax
	jmp	7EF5h

l0800_7EF1:
	shr	word ptr [2E4Fh],1h

l0800_7EF5:
	dec	si

l0800_7EF6:
	or	si,si
	jnz	7EC1h

l0800_7EFA:
	jmp	7FD1h

l0800_7EFD:
	mov	ax,1h
	push	ax
	xor	ax,ax
	push	ax
	call	854Bh
	add	sp,4h
	call	8359h
	xor	al,[2E4Fh]
	push	ax
	call	8600h
	add	sp,2h
	test	word ptr [2E4Fh],1h
	jz	7F2Dh

l0800_7F20:
	mov	ax,[2E4Fh]
	shr	ax,1h
	or	ax,8000h
	mov	[2E4Fh],ax
	jmp	7F31h

l0800_7F2D:
	shr	word ptr [2E4Fh],1h

l0800_7F31:
	dec	si

l0800_7F32:
	test	si,3h
	jnz	7EFDh

l0800_7F38:
	mov	ax,5h
	push	ax
	mov	ax,17h
	push	ax
	call	854Bh
	add	sp,4h
	cmp	si,48h
	jc	7F8Fh

l0800_7F4B:
	mov	ax,4h
	push	ax
	mov	ax,0Fh
	push	ax
	call	854Bh
	add	sp,4h
	xor	di,di
	jmp	7F6Ch

l0800_7F5D:
	call	8359h
	xor	al,[2E4Fh]
	push	ax
	call	8600h
	add	sp,2h
	inc	di

l0800_7F6C:
	cmp	di,48h
	jnz	7F5Dh

l0800_7F71:
	test	word ptr [2E4Fh],1h
	jz	7F86h

l0800_7F79:
	mov	ax,[2E4Fh]
	shr	ax,1h
	or	ax,8000h
	mov	[2E4Fh],ax
	jmp	7F8Ah

l0800_7F86:
	shr	word ptr [2E4Fh],1h

l0800_7F8A:
	sub	si,48h
	jmp	7FD1h

l0800_7F8F:
	mov	ax,4h
	push	ax
	mov	ax,si
	sub	ax,0Ch
	shr	ax,1h
	shr	ax,1h
	push	ax
	call	854Bh
	add	sp,4h
	jmp	7FB4h

l0800_7FA5:
	call	8359h
	xor	al,[2E4Fh]
	push	ax
	call	8600h
	add	sp,2h
	dec	si

l0800_7FB4:
	or	si,si
	jnz	7FA5h

l0800_7FB8:
	test	word ptr [2E4Fh],1h
	jz	7FCDh

l0800_7FC0:
	mov	ax,[2E4Fh]
	shr	ax,1h
	or	ax,8000h
	mov	[2E4Fh],ax
	jmp	7FD1h

l0800_7FCD:
	shr	word ptr [2E4Fh],1h

l0800_7FD1:
	or	si,si
	jz	7FD8h

l0800_7FD5:
	jmp	7EBAh

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
	sub	sp,4h
	mov	word ptr [466Ch],0h
	mov	word ptr [466Ah],0h
	mov	word ptr [2E47h],0h
	mov	ax,[2A0Dh]
	mov	dx,[2A0Bh]
	mov	[4670h],ax
	mov	[466Eh],dx
	xor	ax,ax
	push	ax
	mov	ax,[4664h]
	mov	dx,[4662h]
	add	dx,[467Ah]
	adc	ax,[467Ch]
	add	dx,[4672h]
	adc	ax,[4674h]
	push	ax
	push	dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	0ACB3h
	add	sp,0Ah
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	0ACB3h
	add	sp,0Ah
	jmp	82A1h

l0800_8042:
	xor	ax,ax
	mov	dx,0FFFFh
	sub	dx,[2E31h]
	sbb	ax,0h
	sub	dx,[4672h]
	sbb	ax,[4674h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	mov	ax,[4678h]
	mov	dx,[4676h]
	cmp	ax,[bp-2h]
	ja	807Ch

l0800_8068:
	jc	806Fh

l0800_806A:
	cmp	dx,[bp-4h]
	jnc	807Ch

l0800_806F:
	mov	ax,[4678h]
	mov	dx,[4676h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx

l0800_807C:
	mov	ax,[2E73h]
	mov	dx,[2E71h]
	add	dx,[2E31h]
	mov	[2E6Fh],ax
	mov	[2E6Dh],dx
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	mov	ax,[2E6Dh]
	add	ax,[4672h]
	push	word ptr [2E73h]
	push	ax
	call	4110h
	add	sp,0Ch
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	sub	[4676h],dx
	sbb	[4678h],ax
	add	[4672h],dx
	adc	[4674h],ax
	mov	ax,[2E6Fh]
	mov	dx,[2E6Dh]
	add	dx,[4672h]
	mov	[2E67h],ax
	mov	[2E65h],dx
	mov	[2E6Bh],ax
	mov	[2E69h],dx
	mov	ax,[4670h]
	mov	dx,[466Eh]
	cmp	ax,[4674h]
	jbe	80EDh

l0800_80EA:
	jmp	81E9h

l0800_80ED:
	jc	80F8h

l0800_80EF:
	cmp	dx,[4672h]
	jc	80F8h

l0800_80F5:
	jmp	81E9h

l0800_80F8:
	mov	ax,[2E6Fh]
	mov	dx,[2E6Dh]
	add	dx,[466Eh]
	mov	[2E67h],ax
	mov	[2E65h],dx
	jmp	81E9h

l0800_810D:
	call	8832h
	cmp	word ptr [2E29h],2h
	jc	8193h

l0800_8117:
	mov	ax,[2E6Dh]
	add	ax,[2E29h]
	cmp	ax,[2E65h]
	jbe	813Fh

l0800_8124:
	mov	ax,[466Ah]
	or	ax,[466Ch]
	jz	8130h

l0800_812D:
	jmp	8209h

l0800_8130:
	mov	ax,[2E65h]
	xor	dx,dx
	sub	ax,[2E6Dh]
	sbb	dx,0h
	mov	[2E29h],ax

l0800_813F:
	push	ds
	mov	ax,290Fh
	push	ax
	push	word ptr [2E47h]
	call	831Dh
	add	sp,6h
	push	ds
	mov	ax,278Fh
	push	ax
	mov	ax,[2E29h]
	sub	ax,2h
	push	ax
	call	831Dh
	add	sp,6h
	push	ds
	mov	ax,284Fh
	push	ax
	mov	ax,[2E2Bh]
	dec	ax
	push	ax
	call	831Dh
	add	sp,6h
	push	word ptr [2E29h]
	call	89A8h
	add	sp,2h
	add	word ptr [466Ah],1h
	adc	word ptr [466Ch],0h
	mov	word ptr [2E47h],0h
	mov	ax,[2E29h]
	add	[465Ch],ax
	jmp	81A5h

l0800_8193:
	mov	ax,1h
	push	ax
	call	89A8h
	add	sp,2h
	inc	word ptr [2E47h]
	inc	word ptr [465Ch]

l0800_81A5:
	cmp	word ptr [465Ch],400h
	jc	81E9h

l0800_81AD:
	mov	ax,[465Ch]
	add	[4666h],ax
	adc	word ptr [4668h],0h
	push	word ptr [4680h]
	push	word ptr [467Eh]
	mov	cx,[4668h]
	mov	bx,[4666h]
	xor	dx,dx
	mov	ax,63h
	call	8F18h
	push	dx
	push	ax
	call	8BC2h
	push	dx
	push	ax
	push	ds
	mov	ax,2244h
	push	ax
	call	0B2EFh
	add	sp,8h
	mov	word ptr [465Ch],0h

l0800_81E9:
	mov	ax,[2E65h]
	dec	ax
	cmp	ax,[2E6Dh]
	jbe	8209h

l0800_81F3:
	cmp	word ptr [466Ch],0h
	jnc	81FDh

l0800_81FA:
	jmp	810Dh

l0800_81FD:
	jnz	8209h

l0800_81FF:
	cmp	word ptr [466Ah],0FEh
	jnc	8209h

l0800_8206:
	jmp	810Dh

l0800_8209:
	mov	ax,[2E69h]
	xor	dx,dx
	sub	ax,[2E6Dh]
	sbb	dx,0h
	mov	[4674h],dx
	mov	[4672h],ax
	mov	ax,[2E6Dh]
	xor	dx,dx
	sub	ax,[2E71h]
	sbb	dx,0h
	sub	ax,[2E31h]
	sbb	dx,0h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	mov	ax,[2E71h]
	add	ax,[bp-4h]
	mov	cx,ax
	mov	dx,[2E6Dh]
	xor	bx,bx
	sub	dx,ax
	sbb	bx,0h
	add	dx,[4672h]
	push	dx
	push	word ptr [2E73h]
	push	ax
	push	word ptr [2E73h]
	push	word ptr [2E71h]
	call	0B0F3h
	add	sp,0Ah
	mov	ax,[2E65h]
	cmp	ax,[2E69h]
	jc	82B9h

l0800_8269:
	mov	ax,[2E67h]
	mov	dx,[2E65h]
	cmp	ax,[2E6Bh]
	jnz	8285h

l0800_8276:
	cmp	dx,[2E69h]
	jnz	8285h

l0800_827C:
	mov	ax,[4676h]
	or	ax,[4678h]
	jz	82B9h

l0800_8285:
	cmp	word ptr [466Ch],0h
	jnz	8293h

l0800_828C:
	cmp	word ptr [466Ah],0FEh
	jz	82B9h

l0800_8293:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	sub	[466Eh],dx
	sbb	[4670h],ax

l0800_82A1:
	mov	ax,[4676h]
	or	ax,[4678h]
	jz	82ADh

l0800_82AA:
	jmp	8042h

l0800_82AD:
	mov	ax,[4672h]
	or	ax,[4674h]
	jz	82B9h

l0800_82B6:
	jmp	8042h

l0800_82B9:
	mov	ax,[2E67h]
	mov	dx,[2E65h]
	cmp	ax,[2E6Bh]
	jnz	82EDh

l0800_82C6:
	cmp	dx,[2E69h]
	jnz	82EDh

l0800_82CC:
	mov	ax,[4676h]
	or	ax,[4678h]
	jnz	82EDh

l0800_82D5:
	cmp	word ptr [466Ch],0h
	jnz	82E3h

l0800_82DC:
	cmp	word ptr [466Ah],0FEh
	jz	82EDh

l0800_82E3:
	mov	ax,[2E47h]
	add	ax,[4672h]
	mov	[2E47h],ax

l0800_82ED:
	push	ds
	mov	ax,290Fh
	push	ax
	push	word ptr [2E47h]
	call	831Dh
	add	sp,6h
	add	word ptr [466Ah],1h
	adc	word ptr [466Ch],0h
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	call	0ACB3h
	add	sp,0Ah
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
	mov	si,[bp+4h]
	cmp	si,1h
	jbe	8332h

l0800_8329:
	push	si
	call	0C08h
	add	sp,2h
	jmp	8334h

l0800_8332:
	mov	ax,si

l0800_8334:
	mov	dx,0Ch
	imul	dx
	les	bx,[bp+6h]
	add	bx,ax
	add	word ptr es:[bx],1h
	adc	word ptr es:[bx+2h],0h
	push	word ptr [29DDh]
	push	word ptr [29DBh]
	push	si
	call	401Eh
	add	sp,6h
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
	sub	sp,2h
	push	word ptr [29E5h]
	push	word ptr [29E3h]
	call	3DCFh
	add	sp,4h
	mov	[bp-1h],al
	mov	al,[2E4Bh]
	xor	al,[bp-1h]
	mov	ah,0h
	and	ax,0FFh
	shl	ax,1h
	mov	bx,ax
	mov	ax,[bx+2A29h]
	mov	dx,[2E4Bh]
	mov	cl,8h
	shr	dx,cl
	xor	ax,dx
	mov	[2E4Bh],ax
	add	word ptr [29FFh],1h
	adc	word ptr [2A01h],0h
	mov	al,[bp-1h]
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
	sub	sp,2h
	push	si
	push	di
	mov	di,[bp+8h]
	mov	si,di
	jmp	83C4h

l0800_83B0:
	mov	ax,si
	mov	dx,0Ch
	imul	dx
	les	bx,[bp+4h]
	add	bx,ax
	cmp	word ptr es:[bx+0Ah],0h
	jnz	83CBh

l0800_83C3:
	dec	di

l0800_83C4:
	mov	ax,si
	dec	si
	or	ax,ax
	jnz	83B0h

l0800_83CB:
	mov	ax,5h
	push	ax
	push	di
	call	8489h
	add	sp,4h
	xor	si,si
	mov	ax,[bp+4h]
	add	ax,0Ah
	mov	[bp-2h],ax
	cmp	si,di
	jnc	8401h

l0800_83E5:
	mov	ax,4h
	push	ax
	mov	es,[bp+6h]
	mov	bx,[bp-2h]
	push	word ptr es:[bx]
	call	8489h
	add	sp,4h
	add	word ptr [bp-2h],0Ch
	inc	si
	cmp	si,di
	jc	83E5h

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
	sub	sp,2h
	push	si
	push	di
	mov	si,[bp+4h]
	cmp	si,1h
	jbe	8420h

l0800_8417:
	push	si
	call	0C08h
	add	sp,2h
	jmp	8422h

l0800_8420:
	mov	ax,si

l0800_8422:
	mov	[bp-2h],ax
	mov	dx,0Ch
	imul	dx
	les	bx,[bp+6h]
	add	bx,ax
	mov	di,bx
	push	word ptr es:[bx+0Ah]
	push	word ptr es:[di+6h]
	call	8489h
	add	sp,4h
	cmp	word ptr [bp-2h],1h
	jbe	845Fh

l0800_8445:
	mov	ax,[bp-2h]
	dec	ax
	push	ax
	mov	cl,[bp-2h]
	dec	cl
	mov	ax,1h
	shl	ax,cl
	mov	dx,si
	sub	dx,ax
	push	dx
	call	8489h
	add	sp,4h

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
	mov	ax,[bp+4h]
	mov	dx,[bp+6h]
	cmp	word ptr [2A21h],2h
	jnz	847Fh

l0800_8475:
	push	dx
	push	ax
	call	854Bh
	add	sp,4h
	pop	bp
	ret

l0800_847F:
	push	dx
	push	ax
	call	8489h
	add	sp,4h
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
	mov	di,[bp+4h]
	jmp	853Ah

l0800_8494:
	shr	word ptr [2E43h],1h
	test	di,1h
	jz	84A4h

l0800_849E:
	or	word ptr [2E43h],8000h

l0800_84A4:
	shr	di,1h
	inc	word ptr [2E45h]
	mov	ax,[2E45h]
	cmp	ax,10h
	jz	84B5h

l0800_84B2:
	jmp	853Ah

l0800_84B5:
	mov	al,[2E43h]
	push	ax
	call	8624h
	add	sp,2h
	mov	ax,[2E43h]
	mov	cl,8h
	shr	ax,cl
	push	ax
	call	8624h
	add	sp,2h
	xor	si,si
	jmp	84E3h

l0800_84D1:
	mov	bx,si
	inc	si
	mov	al,[bx+4682h]
	push	ax
	call	8624h
	add	sp,2h
	dec	word ptr [4E82h]

l0800_84E3:
	cmp	word ptr [4E82h],0h
	jnz	84D1h

l0800_84EA:
	mov	ax,[2A01h]
	mov	dx,[29FFh]
	cmp	ax,[2A05h]
	jc	8532h

l0800_84F7:
	ja	84FFh

l0800_84F9:
	cmp	dx,[2A03h]
	jbe	8532h

l0800_84FF:
	mov	ax,[2A01h]
	mov	dx,[29FFh]
	sub	dx,[2A03h]
	sbb	ax,[2A05h]
	cmp	ax,[4E8Ah]
	jc	8532h

l0800_8514:
	ja	851Ch

l0800_8516:
	cmp	dx,[4E88h]
	jbe	8532h

l0800_851C:
	mov	ax,[2A01h]
	mov	dx,[29FFh]
	sub	dx,[2A03h]
	sbb	ax,[2A05h]
	mov	[4E8Ah],ax
	mov	[4E88h],dx

l0800_8532:
	xor	ax,ax
	mov	[2E45h],ax
	mov	[2E43h],ax

l0800_853A:
	mov	ax,[bp+6h]
	dec	word ptr [bp+6h]
	or	ax,ax
	jz	8547h

l0800_8544:
	jmp	8494h

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
	mov	cl,[bp+6h]
	dec	cl
	mov	di,1h
	shl	di,cl
	jmp	85EFh

l0800_855D:
	shl	word ptr [2E43h],1h
	test	[bp+4h],di
	jz	856Ah

l0800_8566:
	inc	word ptr [2E43h]

l0800_856A:
	shr	di,1h
	inc	word ptr [2E45h]
	mov	ax,[2E45h]
	cmp	ax,8h
	jnz	85EFh

l0800_8578:
	mov	al,[2E43h]
	push	ax
	call	8624h
	add	sp,2h
	xor	si,si
	jmp	8598h

l0800_8586:
	mov	bx,si
	inc	si
	mov	al,[bx+4682h]
	push	ax
	call	8624h
	add	sp,2h
	dec	word ptr [4E82h]

l0800_8598:
	cmp	word ptr [4E82h],0h
	jnz	8586h

l0800_859F:
	mov	ax,[2A01h]
	mov	dx,[29FFh]
	cmp	ax,[2A05h]
	jc	85E7h

l0800_85AC:
	ja	85B4h

l0800_85AE:
	cmp	dx,[2A03h]
	jbe	85E7h

l0800_85B4:
	mov	ax,[2A01h]
	mov	dx,[29FFh]
	sub	dx,[2A03h]
	sbb	ax,[2A05h]
	cmp	ax,[4E8Ah]
	jc	85E7h

l0800_85C9:
	ja	85D1h

l0800_85CB:
	cmp	dx,[4E88h]
	jbe	85E7h

l0800_85D1:
	mov	ax,[2A01h]
	mov	dx,[29FFh]
	sub	dx,[2A03h]
	sbb	ax,[2A05h]
	mov	[4E8Ah],ax
	mov	[4E88h],dx

l0800_85E7:
	xor	ax,ax
	mov	[2E45h],ax
	mov	[2E43h],ax

l0800_85EF:
	mov	ax,[bp+6h]
	dec	word ptr [bp+6h]
	or	ax,ax
	jz	85FCh

l0800_85F9:
	jmp	855Dh

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
	mov	dl,[bp+4h]
	cmp	word ptr [2E45h],0h
	jz	861Bh

l0800_860D:
	mov	bx,[4E82h]
	mov	[bx+4682h],dl
	inc	word ptr [4E82h]
	pop	bp
	ret

l0800_861B:
	push	dx
	call	8624h
	add	sp,2h
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
	mov	ax,[2A05h]
	mov	dx,[2A03h]
	cmp	ax,[4E86h]
	ja	8678h

l0800_8634:
	jc	863Ch

l0800_8636:
	cmp	dx,[4E84h]
	jnc	8678h

l0800_863C:
	push	word ptr [29E1h]
	push	word ptr [29DFh]
	mov	al,[bp+4h]
	push	ax
	call	4047h
	add	sp,6h
	mov	al,[2E49h]
	xor	al,[bp+4h]
	mov	ah,0h
	and	ax,0FFh
	shl	ax,1h
	mov	bx,ax
	mov	ax,[bx+2A29h]
	mov	dx,[2E49h]
	mov	cl,8h
	shr	dx,cl
	xor	ax,dx
	mov	[2E49h],ax
	add	word ptr [2A03h],1h
	adc	word ptr [2A05h],0h

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
	mov	si,[bp+6h]
	mov	ds,[bp+8h]
	mov	di,[bp+0Ah]
	mov	es,[bp+0Ch]
	add	si,4h
	call	87EFh
	push	ax
	push	bx
	add	si,0Ah
	stc
	lodsb
	adc	al,al
	add	al,al
	jmp	8745h

l0800_86A1:
	lodsb
	adc	al,al
	jmp	86ADh
0800:86A6                   90                                  .         

l0800_86A7:
	mov	cl,4h

l0800_86A9:
	add	al,al
	jz	86A1h

l0800_86AD:
	adc	bh,bh
	loop	86A9h

l0800_86B1:
	mov	cl,3h
	add	cl,bh
	add	cl,cl
	rep movsw
	jmp	8745h

l0800_86BC:
	lodsb
	adc	al,al
	jmp	86EAh
0800:86C1    90                                            .              

l0800_86C2:
	lodsb
	adc	al,al
	jmp	86F0h
0800:86C7                      90                                .        

l0800_86C8:
	lodsb
	adc	al,al
	jmp	86F9h
0800:86CD                                        90                    .  

l0800_86CE:
	lodsb
	adc	al,al
	jmp	8705h
0800:86D3          90                                        .            

l0800_86D4:
	lodsb
	adc	al,al
	jmp	870Eh
0800:86D9                            90                            .      

l0800_86DA:
	lodsb
	adc	al,al
	jmp	8714h
0800:86DF                                              90                .

l0800_86E0:
	lodsb
	adc	al,al
	jmp	8726h
0800:86E5                90                                    .          

l0800_86E6:
	add	al,al
	jz	86BCh

l0800_86EA:
	adc	cl,cl
	add	al,al
	jz	86C2h

l0800_86F0:
	jnc	8701h

l0800_86F2:
	nop
	nop
	nop
	add	al,al
	jz	86C8h

l0800_86F9:
	dec	cx
	adc	cl,cl
	cmp	cl,9h
	jz	86A7h

l0800_8701:
	add	al,al
	jz	86CEh

l0800_8705:
	jnc	8728h

l0800_8707:
	nop
	nop
	nop
	add	al,al
	jz	86D4h

l0800_870E:
	adc	bh,bh
	add	al,al
	jz	86DAh

l0800_8714:
	jc	8784h

l0800_8716:
	nop
	nop
	nop
	or	bh,bh
	jnz	8728h

l0800_871D:
	nop
	nop
	nop
	inc	bh

l0800_8722:
	add	al,al
	jz	86E0h

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
	rep movsb	byte ptr es:[di],byte ptr es:[si]
	sti
	mov	si,bp
	jmp	8745h
0800:873B                                  90                        .    

l0800_873C:
	lodsb
	adc	al,al
	jc	8753h

l0800_8741:
	nop
	nop
	nop

l0800_8744:
	movsb

l0800_8745:
	add	al,al
	jc	8751h

l0800_8749:
	nop
	nop
	nop
	movsb
	add	al,al
	jnc	8744h

l0800_8751:
	jz	873Ch

l0800_8753:
	mov	cx,2h
	sub	bh,bh
	add	al,al
	jz	879Bh

l0800_875C:
	nop
	nop
	nop

l0800_875F:
	jnc	86E6h

l0800_8761:
	add	al,al
	jz	87A0h

l0800_8765:
	nop
	nop
	nop

l0800_8768:
	jnc	8728h

l0800_876A:
	inc	cx
	add	al,al
	jz	87A5h

l0800_876F:
	nop
	nop
	nop

l0800_8772:
	jnc	8701h

l0800_8774:
	mov	cl,[si]
	inc	si
	or	cl,cl
	jz	87B4h

l0800_877B:
	nop
	nop
	nop
	add	cx,8h
	jmp	8701h

l0800_8784:
	add	al,al
	jz	87AAh

l0800_8788:
	nop
	nop
	nop

l0800_878B:
	adc	bh,bh
	or	bh,4h
	add	al,al
	jz	87AFh

l0800_8794:
	nop
	nop
	nop

l0800_8797:
	jc	8728h

l0800_8799:
	jmp	8722h

l0800_879B:
	lodsb
	adc	al,al
	jmp	875Fh

l0800_87A0:
	lodsb
	adc	al,al
	jmp	8768h

l0800_87A5:
	lodsb
	adc	al,al
	jmp	8772h

l0800_87AA:
	lodsb
	adc	al,al
	jmp	878Bh

l0800_87AF:
	lodsb
	adc	al,al
	jmp	8797h

l0800_87B4:
	push	ax
	mov	bx,di
	and	di,0Fh
	add	di,8000h
	mov	cl,4h
	shr	bx,cl
	mov	ax,es
	add	ax,bx
	sub	ax,800h
	mov	es,ax
	mov	bx,si
	and	si,0Fh
	shr	bx,cl
	mov	ax,ds
	add	ax,bx
	mov	ds,ax
	pop	ax
	add	al,al
	jnz	87E3h

l0800_87DD:
	nop
	nop
	nop
	lodsb
	adc	al,al

l0800_87E3:
	jnc	87E8h

l0800_87E5:
	jmp	8745h

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
	call	87F4h
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
	les	di,[2E37h]
	mov	ax,[2E31h]
	mov	cx,8000h

l0800_8804:
	rep stosw

l0800_8806:
	les	di,[2E33h]
	mov	ax,[2E31h]
	mov	cx,8000h

l0800_8810:
	rep stosw

l0800_8812:
	les	di,[2E3Bh]
	mov	cx,[2E31h]
	xor	ax,ax

l0800_881C:
	rep stosw

l0800_881E:
	les	di,[2E3Fh]
	xor	ax,ax
	mov	cx,[2E31h]

l0800_8828:
	stosw
	inc	ax
	loop	8828h

l0800_882C:
	mov	[2E2Dh],cx
	pop	di
	ret

;; fn0800_8832: 0800:8832
;;   Called from:
;;     0800:810D (in fn0800_7FDC)
fn0800_8832 proc
	push	si
	push	di
	call	889Ah
	cmp	word ptr [2E29h],2h
	jc	8897h

l0800_883E:
	nop
	nop
	nop
	mov	ax,[2E65h]
	sub	ax,[2E6Dh]
	cmp	ax,3h
	jc	8897h

l0800_884D:
	nop
	nop
	nop
	mov	si,[2E29h]
	mov	di,[2E2Bh]
	mov	ax,[2E2Dh]
	push	ax
	inc	ax
	cmp	ax,[2E31h]
	jnz	8865h

l0800_8863:
	xor	ax,ax

l0800_8865:
	mov	[2E2Dh],ax
	inc	word ptr [2E6Dh]
	call	889Ah
	dec	word ptr [2E6Dh]
	pop	word ptr [2E2Dh]
	cmp	[2E29h],si
	jbe	888Fh

l0800_887D:
	nop
	nop
	nop
	mov	word ptr [2E29h],1h
	mov	word ptr [2E2Bh],0h
	jmp	8897h
0800:888E                                           90                  . 

l0800_888F:
	mov	[2E29h],si
	mov	[2E2Bh],di

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
	mov	word ptr [2E2Bh],0h
	mov	word ptr [2E29h],1h
	les	di,[2E6Dh]
	mov	ax,es:[di]
	mov	[4E90h],ax
	inc	di
	mov	dx,[2E69h]
	sub	dx,di
	mov	cx,dx

l0800_88BC:
	rep scasb

l0800_88BE:
	jnz	88C1h

l0800_88C0:
	dec	cx

l0800_88C1:
	sub	dx,cx
	les	di,[2E6Dh]
	mov	ax,[2E69h]
	sub	ax,di
	mov	[4E94h],ax
	mov	di,[4E90h]
	shl	di,1h
	mov	es,[2E39h]
	mov	ax,es:[di]

l0800_88DC:
	cmp	ax,[2E31h]
	jnz	88E5h

l0800_88E2:
	jmp	8984h

l0800_88E5:
	mov	di,ax
	shl	di,1h
	mov	es,[2E41h]
	mov	bx,es:[di]
	mov	[4E92h],bx
	mov	bx,[2E2Dh]
	cmp	bx,ax
	ja	8903h

l0800_88FC:
	nop
	nop
	nop
	add	bx,[2E31h]

l0800_8903:
	sub	bx,ax
	les	si,[2E6Dh]
	sub	si,bx
	mov	ax,es:[si]
	cmp	[4E90h],ax
	jnz	897Eh

l0800_8914:
	nop
	nop
	nop
	mov	es,[2E3Dh]
	mov	cx,es:[di]
	cmp	cx,bx
	jbe	892Dh

l0800_8922:
	nop
	nop
	nop
	mov	bx,1h
	mov	cx,dx
	jmp	8963h
0800:892C                                     90                      .   

l0800_892D:
	cmp	cx,dx
	jbe	893Ah

l0800_8931:
	nop
	nop
	nop
	sub	cx,dx
	sub	bx,cx
	mov	cx,dx

l0800_893A:
	cmp	cx,dx
	jnz	8963h

l0800_893E:
	nop
	nop
	nop
	les	di,[2E6Dh]
	add	di,cx
	mov	si,di
	sub	si,bx
	mov	ax,[4E94h]
	sub	ax,cx
	mov	cx,ax
	push	ds
	mov	ds,[2E6Fh]
	rep cmpsb
	jz	895Ch

l0800_895B:
	inc	cx

l0800_895C:
	pop	ds
	sub	ax,cx
	mov	cx,dx
	add	cx,ax

l0800_8963:
	cmp	cx,[2E2Fh]
	jbe	896Dh

l0800_8969:
	mov	cx,[2E2Fh]

l0800_896D:
	cmp	cx,[2E29h]
	jc	897Eh

l0800_8973:
	nop
	nop
	nop
	mov	[2E29h],cx
	mov	[2E2Bh],bx

l0800_897E:
	mov	ax,[4E92h]
	jmp	88DCh

l0800_8984:
	cmp	word ptr [2E29h],2h
	jnz	89A5h

l0800_898B:
	nop
	nop
	nop
	cmp	word ptr [2E2Bh],100h
	jbe	89A5h

l0800_8996:
	nop
	nop
	nop
	mov	word ptr [2E29h],1h
	mov	word ptr [2E2Bh],0h

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
	mov	dx,[2E31h]
	cld

l0800_89B1:
	mov	di,[2E2Dh]
	shl	di,1h
	mov	es,[2E41h]
	mov	ax,es:[di]
	mov	es:[di],dx
	cmp	[2E2Dh],ax
	jz	89EAh

l0800_89C7:
	nop
	nop
	nop
	les	di,[2E6Dh]
	sub	di,dx
	mov	di,es:[di]
	shl	di,1h
	mov	es,[2E39h]
	mov	es:[di],ax
	cmp	ax,dx
	jnz	89EAh

l0800_89E0:
	nop
	nop
	nop
	mov	es,[2E35h]
	mov	es:[di],dx

l0800_89EA:
	les	di,[2E6Dh]
	mov	di,es:[di]
	shl	di,1h
	mov	ax,[2E2Dh]
	mov	es,[2E39h]
	cmp	es:[di],dx
	jnz	8A08h

l0800_89FF:
	nop
	nop
	nop
	mov	es:[di],ax
	jmp	8A18h
0800:8A07                      90                                .        

l0800_8A08:
	mov	es,[2E35h]
	mov	bx,es:[di]
	shl	bx,1h
	mov	es,[2E41h]
	mov	es:[bx],ax

l0800_8A18:
	mov	es,[2E35h]
	mov	es:[di],ax
	les	di,[2E6Dh]
	mov	al,es:[di]
	inc	di
	mov	bx,[2E69h]
	sub	bx,di
	mov	cx,bx

l0800_8A2F:
	rep scasb

l0800_8A31:
	jnz	8A34h

l0800_8A33:
	dec	cx

l0800_8A34:
	sub	bx,cx
	mov	di,[2E2Dh]
	shl	di,1h
	mov	es,[2E3Dh]
	mov	es:[di],bx
	jmp	8A86h
0800:8A45                90                                    .          

l0800_8A46:
	mov	di,[2E2Dh]
	shl	di,1h
	mov	es,[2E3Dh]
	mov	es:[di],bx
	mov	es,[2E41h]
	mov	ax,[2E2Dh]
	xchg	es:[di],ax
	cmp	[2E2Dh],ax
	jz	8A86h

l0800_8A63:
	nop
	nop
	nop
	les	di,[2E6Dh]
	sub	di,dx
	mov	di,es:[di]
	shl	di,1h
	mov	es,[2E39h]
	mov	es:[di],ax
	cmp	ax,dx
	jnz	8A86h

l0800_8A7C:
	nop
	nop
	nop
	mov	es,[2E35h]
	mov	es:[di],dx

l0800_8A86:
	mov	ax,[2E2Dh]
	inc	ax
	cmp	ax,dx
	jnz	8A90h

l0800_8A8E:
	xor	ax,ax

l0800_8A90:
	mov	[2E2Dh],ax
	inc	word ptr [2E6Dh]
	dec	word ptr [bp+4h]
	jz	8AA8h

l0800_8A9C:
	nop
	nop
	nop
	dec	bx
	cmp	bx,1h
	ja	8A46h

l0800_8AA5:
	jmp	89B1h

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
	mov	ah,43h
	xor	al,al
	lds	dx,[bp+4h]
	int	21h
	pop	ds
	jc	8AE9h

l0800_8ADF:
	les	bx,[bp+8h]
	mov	es:[bx],cx
	xor	ax,ax
	jmp	8AEDh

l0800_8AE9:
	push	ax
	call	8D64h

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
	mov	si,[bp+8h]
	or	si,si
	jnz	8B36h

l0800_8B18:
	jmp	8B28h

l0800_8B1A:
	dec	word ptr [224Eh]
	mov	bx,[224Eh]
	shl	bx,1h
	call	word ptr [bx+4E96h]

l0800_8B28:
	cmp	word ptr [224Eh],0h
	jnz	8B1Ah

l0800_8B2F:
	call	0150h
	call	word ptr [2352h]

l0800_8B36:
	call	01B9h
	call	0163h
	cmp	word ptr [bp+6h],0h
	jnz	8B55h

l0800_8B42:
	or	si,si
	jnz	8B4Eh

l0800_8B46:
	call	word ptr [2354h]
	call	word ptr [2356h]

l0800_8B4E:
	push	word ptr [bp+4h]
	call	0164h
	pop	cx

l0800_8B55:
	pop	si
	pop	bp
	ret	6h

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
	push	word ptr [bp+4h]
	call	8B0Dh
	pop	bp
	ret

;; fn0800_8B69: 0800:8B69
fn0800_8B69 proc
	push	bp
	mov	bp,sp
	mov	ax,1h
	push	ax
	xor	ax,ax
	push	ax
	push	word ptr [bp+4h]
	call	8B0Dh
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
	mov	ah,2Ah
	int	21h
	les	bx,[bp+4h]
	mov	es:[bx],cx
	mov	es:[bx+2h],dx
	pop	bp
	ret

;; fn0800_8BA8: 0800:8BA8
;;   Called from:
;;     0800:9779 (in fn0800_9764)
fn0800_8BA8 proc
	push	bp
	mov	bp,sp
	mov	ah,2Ch
	int	21h
	les	bx,[bp+4h]
	mov	es:[bx],cx
	mov	es:[bx+2h],dx
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

;; fn0800_8BBE: 0800:8BBE
;;   Called from:
;;     0800:8BBB (in fn0800_8BBB)
fn0800_8BBE proc
	xor	cx,cx
	jmp	8BD8h

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

;; fn0800_8BC5: 0800:8BC5
;;   Called from:
;;     0800:8BC2 (in fn0800_8BC2)
fn0800_8BC5 proc
	mov	cx,1h
	jmp	8BD8h

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

;; fn0800_8BCD: 0800:8BCD
;;   Called from:
;;     0800:8BCA (in fn0800_8BCA)
fn0800_8BCD proc
	mov	cx,2h
	jmp	8BD8h
0800:8BD2       59 0E 51 B9 03 00                           Y.Q...        

;; fn0800_8BD8: 0800:8BD8
;;   Called from:
;;     0800:8BC0 (in fn0800_8BBE)
;;     0800:8BC8 (in fn0800_8BC5)
;;     0800:8BD0 (in fn0800_8BCD)
fn0800_8BD8 proc
	push	bp
	push	si
	push	di
	mov	bp,sp
	mov	di,cx
	mov	ax,[bp+0Ah]
	mov	dx,[bp+0Ch]
	mov	bx,[bp+0Eh]
	mov	cx,[bp+10h]
	or	cx,cx
	jnz	8BF7h

l0800_8BEF:
	or	dx,dx
	jz	8C5Ch

l0800_8BF3:
	or	bx,bx
	jz	8C5Ch

l0800_8BF7:
	test	di,1h
	jnz	8C19h

l0800_8BFD:
	or	dx,dx
	jns	8C0Bh

l0800_8C01:
	neg	dx
	neg	ax
	sbb	dx,0h
	or	di,0Ch

l0800_8C0B:
	or	cx,cx
	jns	8C19h

l0800_8C0F:
	neg	cx
	neg	bx
	sbb	cx,0h
	xor	di,4h

l0800_8C19:
	mov	bp,cx
	mov	cx,20h
	push	di
	xor	di,di
	xor	si,si

l0800_8C23:
	shl	ax,1h
	rcl	dx,1h
	rcl	si,1h
	rcl	di,1h
	cmp	di,bp
	jc	8C3Ah

l0800_8C2F:
	ja	8C35h

l0800_8C31:
	cmp	si,bx
	jc	8C3Ah

l0800_8C35:
	sub	si,bx
	sbb	di,bp
	inc	ax

l0800_8C3A:
	loop	8C23h

l0800_8C3C:
	pop	bx
	test	bx,2h
	jz	8C49h

l0800_8C43:
	mov	ax,si
	mov	dx,di
	shr	bx,1h

l0800_8C49:
	test	bx,4h
	jz	8C56h

l0800_8C4F:
	neg	dx
	neg	ax
	sbb	dx,0h

l0800_8C56:
	pop	di
	pop	si
	pop	bp
	retf	8h

l0800_8C5C:
	div	bx
	test	di,2h
	jz	8C65h

l0800_8C64:
	xchg	dx,ax

l0800_8C65:
	xor	dx,dx
	jmp	8C56h

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

;; fn0800_8C6C: 0800:8C6C
;;   Called from:
;;     0800:8C69 (in fn0800_8C69)
fn0800_8C6C proc
	cmp	cl,10h
	jnc	8C81h

l0800_8C71:
	mov	bx,ax
	shl	ax,cl
	shl	dx,cl
	neg	cl
	add	cl,10h
	shr	bx,cl
	or	dx,bx
	retf

l0800_8C81:
	sub	cl,10h
	xchg	dx,ax
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

;; fn0800_8C8D: 0800:8C8D
;;   Called from:
;;     0800:8C8A (in fn0800_8C8A)
fn0800_8C8D proc
	cmp	cl,10h
	jnc	8CA2h

l0800_8C92:
	mov	bx,dx
	shr	ax,cl
	sar	dx,cl
	neg	cl
	add	cl,10h
	shl	bx,cl
	or	ax,bx
	retf

l0800_8CA2:
	sub	cl,10h
	xchg	dx,ax
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

;; fn0800_8CAD: 0800:8CAD
;;   Called from:
;;     0800:8CAA (in fn0800_8CAA)
fn0800_8CAD proc
	cmp	cl,10h
	jnc	8CC2h

l0800_8CB2:
	mov	bx,dx
	shr	ax,cl
	shr	dx,cl
	neg	cl
	add	cl,10h
	shl	bx,cl
	or	ax,bx
	retf

l0800_8CC2:
	sub	cl,10h
	xchg	dx,ax
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

;; fn0800_8CCE: 0800:8CCE
;;   Called from:
;;     0800:8CCB (in fn0800_8CCB)
fn0800_8CCE proc
	or	cx,cx
	jge	8CDEh

l0800_8CD2:
	not	bx
	not	cx
	add	bx,1h
	adc	cx,0h
	jmp	8D0Dh

l0800_8CDE:
	add	ax,bx
	jnc	8CE6h

l0800_8CE2:
	add	dx,1000h

l0800_8CE6:
	mov	ch,cl
	mov	cl,4h
	shl	ch,cl
	add	dh,ch
	mov	ch,al
	shr	ax,cl
	add	dx,ax
	mov	al,ch
	and	ax,0Fh
	retf
0800:8CFA                               07 0E 06 0B C9 7D           .....}
0800:8D00 0C F7 D3 F7 D1 83 C3 01 83 D1 00 EB D1          .............   

l0800_8D0D:
	sub	ax,bx
	jnc	8D15h

l0800_8D11:
	sub	dx,1000h

l0800_8D15:
	mov	bh,cl
	mov	cl,4h
	shl	bh,cl
	xor	bl,bl
	sub	dx,bx
	mov	ch,al
	shr	ax,cl
	add	dx,ax
	mov	al,ch
	and	ax,0Fh
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
	mov	si,[bp+4h]
	or	si,si
	jl	8D4Bh

l0800_8D36:
	cmp	si,58h
	jle	8D3Eh

l0800_8D3B:
	mov	si,57h

l0800_8D3E:
	mov	[2516h],si
	mov	al,[si+2518h]
	cbw
	mov	si,ax
	jmp	8D58h

l0800_8D4B:
	neg	si
	cmp	si,23h
	jg	8D3Bh

l0800_8D52:
	mov	word ptr [2516h],0FFFFh

l0800_8D58:
	mov	[007Fh],si
	mov	ax,0FFFFh
	pop	si
	pop	bp
	ret	2h

;; fn0800_8D64: 0800:8D64
;;   Called from:
;;     0800:8AEA (in fn0800_8ACF)
fn0800_8D64 proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+4h]
	push	si
	call	8D2Bh
	mov	ax,si
	pop	si
	pop	bp
	ret	2h

;; fn0800_8D76: 0800:8D76
;;   Called from:
;;     0800:A9D6 (in fn0800_A96D)
fn0800_8D76 proc
	push	bp
	mov	bp,sp
	mov	ax,4400h
	mov	bx,[bp+4h]
	int	21h
	xchg	dx,ax
	and	ax,80h
	pop	bp
	ret

;; fn0800_8D87: 0800:8D87
;;   Called from:
;;     0800:8E22 (in fn0800_8E09)
;;     0800:9A43 (in fn0800_9828)
fn0800_8D87 proc
	push	bp
	mov	bp,sp
	sub	sp,22h
	push	si
	push	di
	push	es
	les	di,[bp+0Ah]
	mov	bx,[bp+8h]
	cmp	bx,24h
	ja	8DF7h

l0800_8D9B:
	cmp	bl,2h
	jc	8DF7h

l0800_8DA0:
	mov	ax,[bp+0Eh]
	mov	cx,[bp+10h]
	or	cx,cx
	jge	8DBCh

l0800_8DAA:
	cmp	byte ptr [bp+6h],0h
	jz	8DBCh

l0800_8DB0:
	mov	byte ptr es:[di],2Dh
	inc	di
	neg	cx
	neg	ax
	sbb	cx,0h

l0800_8DBC:
	lea	si,[bp-22h]
	jcxz	8DD1h

l0800_8DC1:
	xchg	cx,ax
	sub	dx,dx
	div	bx
	xchg	cx,ax
	div	bx
	mov	ss:[si],dl
	inc	si
	jcxz	8DD9h

l0800_8DCF:
	jmp	8DC1h

l0800_8DD1:
	sub	dx,dx
	div	bx
	mov	ss:[si],dl
	inc	si

l0800_8DD9:
	or	ax,ax
	jnz	8DD1h

l0800_8DDD:
	lea	cx,[bp-22h]
	neg	cx
	add	cx,si
	cld

l0800_8DE5:
	dec	si
	mov	al,ss:[si]
	sub	al,0Ah
	jnc	8DF1h

l0800_8DED:
	add	al,3Ah
	jmp	8DF4h

l0800_8DF1:
	add	al,[bp+4h]

l0800_8DF4:
	stosb
	loop	8DE5h

l0800_8DF7:
	mov	al,0h
	stosb
	pop	es
	mov	dx,[bp+0Ch]
	mov	ax,[bp+0Ah]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	0Eh

;; fn0800_8E09: 0800:8E09
;;   Called from:
;;     0800:8EA5 (in fn0800_8E6A)
fn0800_8E09 proc
	push	bp
	mov	bp,sp
	xor	ax,ax
	push	ax
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	mov	ax,0Ah
	push	ax
	mov	al,0h
	push	ax
	mov	al,61h
	push	ax
	call	8D87h
	pop	bp
	ret	6h

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
	mov	bx,[bp+4h]
	shl	bx,1h
	and	word ptr [bx+24EAh],0FDFFh
	mov	ah,42h
	mov	al,[bp+0Ah]
	mov	bx,[bp+4h]
	mov	cx,[bp+8h]
	mov	dx,[bp+6h]
	int	21h
	jc	8E4Bh

l0800_8E49:
	jmp	8E50h

l0800_8E4B:
	push	ax
	call	8D2Bh
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
	mov	ah,39h
	lds	dx,[bp+4h]
	int	21h
	pop	ds
	jc	8E64h

l0800_8E60:
	xor	ax,ax
	jmp	8E68h

l0800_8E64:
	push	ax
	call	8D2Bh

l0800_8E68:
	pop	bp
	ret

;; fn0800_8E6A: 0800:8E6A
;;   Called from:
;;     0800:A69F (in fn0800_A614)
fn0800_8E6A proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+0Ah]
	or	ax,[bp+0Ch]
	jnz	8E7Dh

l0800_8E75:
	mov	[bp+0Ch],ds
	mov	word ptr [bp+0Ah],4ED6h

l0800_8E7D:
	push	word ptr [bp+4h]
	mov	ax,[bp+6h]
	or	ax,[bp+8h]
	jnz	8E8Fh

l0800_8E88:
	mov	dx,ds
	mov	ax,2572h
	jmp	8E95h

l0800_8E8F:
	mov	dx,[bp+8h]
	mov	ax,[bp+6h]

l0800_8E95:
	push	dx
	push	ax
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	call	9CE6h
	add	sp,8h
	push	dx
	push	ax
	call	8E09h
	push	ds
	mov	ax,2576h
	push	ax
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	call	0BF5Fh
	add	sp,8h
	mov	dx,[bp+0Ch]
	mov	ax,[bp+0Ah]
	pop	bp
	ret	0Ah
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
	xchg	si,ax
	xchg	dx,ax
	test	ax,ax
	jz	8F21h

l0800_8F1F:
	mul	bx

l0800_8F21:
	jcxz	8F28h

l0800_8F23:
	xchg	cx,ax
	mul	si
	add	ax,cx

l0800_8F28:
	xchg	si,ax
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
	mov	cl,4h
	shr	ax,cl
	add	dx,ax
	mov	al,ch
	mov	ah,bl
	shr	bx,cl
	pop	cx
	add	cx,bx
	mov	bl,ah
	and	ax,0Fh
	and	bx,0Fh
	cmp	dx,cx
	jnz	8F4Fh

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
	mov	bx,[bp+4h]
	shl	bx,1h
	test	word ptr [bx+24EAh],2h
	jz	8F66h

l0800_8F60:
	mov	ax,5h
	push	ax
	jmp	8F7Ah

l0800_8F66:
	push	ds
	mov	ah,3Fh
	mov	bx,[bp+4h]
	mov	cx,[bp+0Ah]
	lds	dx,[bp+6h]
	int	21h
	pop	ds
	jc	8F79h

l0800_8F77:
	jmp	8F7Dh

l0800_8F79:
	push	ax

l0800_8F7A:
	call	8D2Bh

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
	mov	ah,41h
	lds	dx,[bp+4h]
	int	21h
	pop	ds
	jc	8F91h

l0800_8F8D:
	xor	ax,ax
	jmp	8F95h

l0800_8F91:
	push	ax
	call	8D2Bh

l0800_8F95:
	pop	bp
	ret

;; fn0800_8F97: 0800:8F97
;;   Called from:
;;     0800:BF33 (in fn0800_BF18)
fn0800_8F97 proc
	push	bp
	mov	bp,sp
	sub	sp,2Ah
	push	si
	push	di
	mov	word ptr [bp-4h],0h
	mov	word ptr [bp-6h],0h
	jmp	8FC6h

;; fn0800_8FAB: 0800:8FAB
;;   Called from:
;;     0800:9110 (in fn0800_8F97)
;;     0800:91D3 (in fn0800_8F97)
;;     0800:9275 (in fn0800_8F97)
;;     0800:9327 (in fn0800_8F97)
fn0800_8FAB proc
	les	di,[bp+10h]
	test	byte ptr [bp-1h],20h
	jz	8FBCh

l0800_8FB4:
	les	di,es:[di]
	add	word ptr [bp+10h],4h
	ret

l0800_8FBC:
	mov	di,es:[di]
	push	ds
	pop	es
	add	word ptr [bp+10h],2h
	ret

l0800_8FC6:
	push	es
	cld

l0800_8FC8:
	mov	si,[bp+0Ch]

l0800_8FCB:
	mov	es,[bp+0Eh]
	lodsb	al,es:[si]
	or	al,al
	jz	9042h

l0800_8FD4:
	cmp	al,25h
	jz	9045h

l0800_8FD8:
	cbw
	xchg	di,ax
	inc	word ptr [bp-6h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	or	ax,ax
	jl	9016h

l0800_8FEC:
	cbw
	or	di,di
	js	902Bh

l0800_8FF1:
	cmp	byte ptr [di+257Ch],1h
	jnz	902Bh

l0800_8FF8:
	xchg	bx,ax
	or	bl,bl
	js	9019h

l0800_8FFD:
	cmp	byte ptr [bx+257Ch],1h
	jnz	9019h

l0800_9004:
	inc	word ptr [bp-6h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	or	ax,ax
	jg	8FF8h

l0800_9016:
	jmp	93A6h

l0800_9019:
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	bx
	call	word ptr [bp+6h]
	add	sp,6h
	dec	word ptr [bp-6h]
	jmp	8FCBh

l0800_902B:
	cmp	ax,di
	jz	8FCBh

l0800_902F:
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	ax
	call	word ptr [bp+6h]
	add	sp,6h
	dec	word ptr [bp-6h]
	jmp	93BEh

l0800_9042:
	jmp	93BEh

l0800_9045:
	mov	word ptr [bp-0Ah],0FFFFh
	mov	es,[bp+0Eh]
	mov	byte ptr [bp-1h],20h

l0800_9051:
	lodsb	al,es:[si]
	cbw
	mov	[bp+0Ch],si
	xchg	di,ax
	or	di,di
	jl	9075h

l0800_905C:
	mov	bl,[di+257Ch]
	xor	bh,bh
	cmp	bx,15h
	jbe	906Ah

l0800_9067:
	jmp	93A6h

l0800_906A:
	shl	bx,1h
	jmp	word ptr cs:[bx+9459h]

l0800_9071:
	xchg	di,ax
	jmp	8FD8h

l0800_9075:
	jmp	93BEh

l0800_9078:
	or	byte ptr [bp-1h],1h
	jmp	9051h

l0800_907E:
	sub	di,30h
	xchg	[bp-0Ah],di
	or	di,di
	jl	9051h

l0800_9088:
	mov	ax,0Ah
	mul	di
	add	[bp-0Ah],ax
	jmp	9051h

l0800_9092:
	or	byte ptr [bp-1h],8h
	jmp	9051h

l0800_9098:
	or	byte ptr [bp-1h],4h
	jmp	9051h

l0800_909E:
	or	byte ptr [bp-1h],2h
	jmp	9051h

l0800_90A4:
	and	byte ptr [bp-1h],0DFh
	jmp	9051h

l0800_90AA:
	or	byte ptr [bp-1h],20h
	jmp	9051h

l0800_90B0:
	mov	ax,[bp-6h]
	sub	dx,dx
	test	byte ptr [bp-1h],1h
	jz	9110h

l0800_90BB:
	jmp	9051h

l0800_90BD:
	mov	si,8h
	jmp	90CEh

l0800_90C2:
	mov	si,0Ah
	jmp	90CEh

l0800_90C7:
	mov	si,10h
	jmp	90CEh

l0800_90CC:
	xor	si,si

l0800_90CE:
	test	di,20h
	jnz	90DDh

l0800_90D4:
	cmp	di,58h
	jz	90DDh

l0800_90D9:
	or	byte ptr [bp-1h],4h

l0800_90DD:
	push	ss
	lea	ax,[bp-8h]
	push	ax
	push	ss
	lea	ax,[bp-6h]
	push	ax
	mov	ax,[bp-0Ah]
	and	ax,7FFFh
	push	ax
	push	si
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	94B0h
	add	sp,14h
	cmp	word ptr [bp-8h],0h
	jle	911Fh

l0800_9107:
	test	byte ptr [bp-1h],1h
	jnz	911Ch

l0800_910D:
	inc	word ptr [bp-4h]

l0800_9110:
	call	8FABh
	stosw
	test	byte ptr [bp-1h],4h
	jz	911Ch

l0800_911A:
	xchg	dx,ax
	stosw

l0800_911C:
	jmp	8FC8h

l0800_911F:
	jl	9124h

l0800_9121:
	jmp	93BEh

l0800_9124:
	jmp	93A6h

l0800_9127:
	call	912Ah

;; fn0800_912A: 0800:912A
;;   Called from:
;;     0800:9127 (in fn0800_8F97)
;;     0800:9127 (in fn0800_8F97)
fn0800_912A proc
	jmp	93C5h
0800:912D                                        FF 76 0A              .v.
0800:9130 FF 76 08 50 FF 56 06 83 C4 06 FF 4E FA 81 66 F6 .v.P.V.....N..f.
0800:9140 FF 7F E8 00 00                                  .....           

;; fn0800_9145: 0800:9145
fn0800_9145 proc
	jmp	93EFh
0800:9148                         52 3C 3A 74 19 0B C0 7E         R<:t...~
0800:9150 10 FF 76 0A FF 76 08 50 FF 56 06 83 C4 06 FF 4E ..v..v.P.V.....N
0800:9160 FA 5A 8C DB EB 1F E8 00 00                      .Z.......       

;; fn0800_9169: 0800:9169
fn0800_9169 proc
	jmp	93EFh
0800:916C                                     5B 0B C0 7E             [..~
0800:9170 14 52 53 FF 76 0A FF 76 08 50 FF 56 06 83 C4 06 .RS.v..v.P.V....
0800:9180 FF 4E FA 5B 5A F6 46 FF 01 75 10 E8 1D FE FF 46 .N.[Z.F..u.....F
0800:9190 FC 92 AB F6 46 FF 20 74 02 93 AB E9 2A FE       ....F. t....*.  

l0800_919E:
	jmp	93A6h

l0800_91A1:
	push	ss
	lea	ax,[bp-8h]
	push	ax
	push	ss
	lea	ax,[bp-6h]
	push	ax
	mov	ax,7FFFh
	and	ax,[bp-0Ah]
	push	ax
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0A2D0h
	add	sp,12h
	cmp	word ptr [bp-8h],0h
	jle	9203h

l0800_91CA:
	mov	al,[bp-1h]
	cbw
	test	ax,1h
	jnz	91FDh

l0800_91D3:
	call	8FABh
	inc	word ptr [bp-4h]
	test	byte ptr [bp-1h],4h
	jz	91E4h

l0800_91DF:
	mov	ax,4h
	jmp	91F1h

l0800_91E4:
	test	byte ptr [bp-1h],8h
	jz	91EFh

l0800_91EA:
	mov	ax,8h
	jmp	91F1h

l0800_91EF:
	xor	ax,ax

l0800_91F1:
	push	ax
	push	es
	push	di
	call	0A2D4h
	add	sp,6h
	jmp	8FC8h

l0800_91FD:
	call	0A2D8h
	jmp	8FC8h

l0800_9203:
	call	0A2D8h
	jl	919Eh

l0800_9208:
	jmp	93BEh

l0800_920B:
	call	920Eh

;; fn0800_920E: 0800:920E
;;   Called from:
;;     0800:920B (in fn0800_8F97)
;;     0800:920B (in fn0800_8F97)
fn0800_920E proc
	jmp	93C5h
0800:9211    F6 46 FF 01 75 06 E8 91 FD FF 46 FC 81 66 F6  .F..u.....F..f.
0800:9220 FF 7F 74 2D F6 46 FF 01 75 01 AA FF 46 FA 06 FF ..t-.F..u...F...
0800:9230 76 0A FF 76 08 FF 56 04 59 59 07 0B C0 7E 12 0A v..v..V.YY...~..
0800:9240 C0 78 09 93 80 BF 7C 25 01 93 7E 05 FF 4E F6 7F .x....|%..~..N..
0800:9250 D3 06 FF 76 0A FF 76 08 50 FF 56 06 83 C4 06 07 ...v..v.P.V.....
0800:9260 FF 4E FA F6 46 FF 01 75 03 B0 00 AA E9 59 FD    .N..F..u.....Y. 

l0800_926F:
	test	byte ptr [bp-1h],1h
	jnz	9278h

l0800_9275:
	call	8FABh

l0800_9278:
	mov	si,[bp-0Ah]
	or	si,si
	jge	9282h

l0800_927F:
	mov	si,1h

l0800_9282:
	jz	92A2h

l0800_9284:
	inc	word ptr [bp-6h]
	push	es
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	pop	es
	or	ax,ax
	jl	92AEh

l0800_9298:
	test	byte ptr [bp-1h],1h
	jnz	929Fh

l0800_929E:
	stosb

l0800_929F:
	dec	si
	jg	9284h

l0800_92A2:
	test	byte ptr [bp-1h],1h
	jnz	92ABh

l0800_92A8:
	inc	word ptr [bp-4h]

l0800_92AB:
	jmp	8FC8h

l0800_92AE:
	jmp	93A6h

l0800_92B1:
	push	es
	sub	ax,ax
	cld
	push	ss
	pop	es
	lea	di,[bp-2Ah]
	mov	cx,10h

l0800_92BD:
	rep stosw

l0800_92BF:
	pop	es
	lodsb	al,es:[si]
	and	byte ptr [bp-1h],0EFh
	cmp	al,5Eh
	jnz	92D0h

l0800_92CA:
	or	byte ptr [bp-1h],10h
	lodsb	al,es:[si]

l0800_92D0:
	mov	ah,0h

l0800_92D2:
	mov	dl,al
	mov	di,ax
	mov	cl,3h
	shr	di,cl
	mov	cx,107h
	and	cl,dl
	shl	ch,cl
	or	[bp+di-2Ah],ch

l0800_92E4:
	lodsb	al,es:[si]
	cmp	al,0h
	jz	9313h

l0800_92EA:
	cmp	al,5Dh
	jz	9316h

l0800_92EE:
	cmp	al,2Dh
	jnz	92D2h

l0800_92F2:
	cmp	dl,es:[si]
	ja	92D2h

l0800_92F7:
	cmp	byte ptr es:[si],5Dh
	jz	92D2h

l0800_92FD:
	lodsb	al,es:[si]
	sub	al,dl
	jz	92E4h

l0800_9303:
	add	dl,al

l0800_9305:
	rol	ch,1h
	adc	di,0h
	or	[bp+di-2Ah],ch
	dec	al
	jnz	9305h

l0800_9311:
	jmp	92E4h

l0800_9313:
	jmp	93BEh

l0800_9316:
	mov	[bp+0Ch],si
	and	word ptr [bp-0Ah],7FFFh
	mov	si,[bp-0Ah]
	test	byte ptr [bp-1h],1h
	jnz	932Ah

l0800_9327:
	call	8FABh

l0800_932A:
	dec	si
	jl	9385h

l0800_932D:
	inc	word ptr [bp-6h]
	push	es
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	pop	es
	or	ax,ax
	jl	9394h

l0800_9341:
	xchg	si,ax
	mov	bx,si
	mov	cl,3h
	shr	si,cl
	mov	cx,107h
	and	cl,bl
	shl	ch,cl
	test	[bp+si-2Ah],ch
	xchg	si,ax
	xchg	bx,ax
	jz	935Eh

l0800_9356:
	test	byte ptr [bp-1h],10h
	jz	9364h

l0800_935C:
	jmp	936Dh

l0800_935E:
	test	byte ptr [bp-1h],10h
	jz	936Dh

l0800_9364:
	test	byte ptr [bp-1h],1h
	jnz	932Ah

l0800_936A:
	stosb
	jmp	932Ah

l0800_936D:
	push	es
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	ax
	call	word ptr [bp+6h]
	add	sp,6h
	pop	es
	dec	word ptr [bp-6h]
	inc	si
	cmp	si,[bp-0Ah]
	jge	938Eh

l0800_9385:
	test	byte ptr [bp-1h],1h
	jnz	9391h

l0800_938B:
	inc	word ptr [bp-4h]

l0800_938E:
	mov	al,0h
	stosb

l0800_9391:
	jmp	8FC8h

l0800_9394:
	inc	si
	cmp	si,[bp-0Ah]
	jge	93A6h

l0800_939A:
	test	byte ptr [bp-1h],1h
	jnz	93A6h

l0800_93A0:
	mov	al,0h
	stosb
	inc	word ptr [bp-4h]

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
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	mov	ax,0FFFFh
	push	ax
	call	word ptr [bp+6h]
	add	sp,6h
	cmp	word ptr [bp-4h],1h
	sbb	word ptr [bp-4h],0h

;; fn0800_93BE: 0800:93BE
;;   Called from:
;;     0800:9121 (in fn0800_8F97)
;;     0800:9208 (in fn0800_8F97)
;;     0800:9313 (in fn0800_8F97)
;;     0800:93BA (in fn0800_93A6)
fn0800_93BE proc
	pop	es
	mov	ax,[bp-4h]
	jmp	9453h

;; fn0800_93C5: 0800:93C5
;;   Called from:
;;     0800:912A (in fn0800_912A)
;;     0800:920E (in fn0800_920E)
fn0800_93C5 proc
	inc	word ptr [bp-6h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	or	ax,ax
	jle	93EAh

l0800_93D7:
	or	al,al
	js	93E4h

l0800_93DB:
	xchg	bx,ax
	cmp	byte ptr [bx+257Ch],1h
	xchg	bx,ax
	jz	93C5h

l0800_93E4:
	pop	cx
	add	cx,3h
	jmp	cx

l0800_93EA:
	jz	93E4h

l0800_93EC:
	pop	cx
	jmp	93A6h

;; fn0800_93EF: 0800:93EF
;;   Called from:
;;     0800:9145 (in fn0800_9145)
;;     0800:9169 (in fn0800_9169)
fn0800_93EF proc
	sub	dx,dx
	mov	cx,4h

l0800_93F4:
	dec	word ptr [bp-0Ah]
	jl	9442h

l0800_93F9:
	push	dx
	push	cx
	inc	word ptr [bp-6h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	pop	cx
	pop	dx
	or	ax,ax
	jle	9444h

l0800_940F:
	dec	cl
	jl	9444h

l0800_9413:
	mov	ch,al
	sub	ch,30h
	jc	9444h

l0800_941A:
	cmp	ch,0Ah
	jc	9436h

l0800_941F:
	sub	ch,11h
	jc	9444h

l0800_9424:
	cmp	ch,6h
	jc	9433h

l0800_9429:
	sub	ch,20h
	jc	9444h

l0800_942E:
	cmp	ch,6h
	jnc	9444h

l0800_9433:
	add	ch,0Ah

l0800_9436:
	shl	dx,1h
	shl	dx,1h
	shl	dx,1h
	shl	dx,1h
	add	dl,ch
	jmp	93F4h

l0800_9442:
	sub	ax,ax

l0800_9444:
	cmp	cl,4h
	jz	944Fh

l0800_9449:
	pop	cx
	add	cx,3h
	jmp	cx

l0800_944F:
	pop	cx
	jmp	93A6h

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
	sub	bl,30h
	jc	94ADh

l0800_948B:
	cmp	bl,9h
	jbe	94A2h

l0800_9490:
	cmp	bl,2Ah
	ja	949Ah

l0800_9495:
	sub	bl,7h
	jmp	949Dh

l0800_949A:
	sub	bl,27h

l0800_949D:
	cmp	bl,9h
	jbe	94ADh

l0800_94A2:
	cmp	bl,cl
	jnc	94ADh

l0800_94A6:
	inc	sp
	inc	sp
	clc
	mov	bh,0h
	jmp	94AFh

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
	sub	sp,6h
	push	si
	push	di
	mov	byte ptr [bp-1h],0h
	mov	word ptr [bp-4h],0h
	mov	word ptr [bp-6h],1h

l0800_94C6:
	inc	word ptr [bp-4h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	or	ax,ax
	jl	9549h

l0800_94D8:
	cbw
	xchg	bx,ax
	test	bl,80h
	jnz	94E7h

l0800_94DF:
	mov	di,2251h
	test	byte ptr [bx+di],1h
	jnz	94C6h

l0800_94E7:
	xchg	bx,ax
	dec	word ptr [bp+0Eh]
	jl	9550h

l0800_94ED:
	cmp	al,2Bh
	jz	94F8h

l0800_94F1:
	cmp	al,2Dh
	jnz	950Fh

l0800_94F5:
	inc	byte ptr [bp-1h]

l0800_94F8:
	dec	word ptr [bp+0Eh]
	jl	9550h

l0800_94FD:
	inc	word ptr [bp-4h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	or	ax,ax
	jl	9549h

l0800_950F:
	sub	si,si
	mov	di,si
	mov	cx,[bp+0Ch]
	jcxz	956Eh

l0800_9518:
	cmp	cx,24h
	ja	9550h

l0800_951D:
	cmp	cl,2h
	jc	9550h

l0800_9522:
	cmp	al,30h
	jnz	959Eh

l0800_9526:
	cmp	cl,10h
	jnz	959Ch

l0800_952B:
	dec	word ptr [bp+0Eh]
	jl	956Bh

l0800_9530:
	inc	word ptr [bp-4h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	cmp	al,78h
	jz	959Ch

l0800_9542:
	cmp	al,58h
	jz	959Ch

l0800_9546:
	jmp	95C8h

l0800_9549:
	mov	word ptr [bp-6h],0FFFFh
	jmp	9555h

l0800_9550:
	mov	word ptr [bp-6h],0h

l0800_9555:
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	ax
	call	word ptr [bp+6h]
	add	sp,6h
	dec	word ptr [bp-4h]
	sub	ax,ax
	cwd
	jmp	9622h

l0800_956B:
	jmp	9612h

l0800_956E:
	cmp	al,30h
	mov	word ptr [bp+0Ch],0Ah
	jnz	959Eh

l0800_9577:
	dec	word ptr [bp+0Eh]
	jl	956Bh

l0800_957C:
	inc	word ptr [bp-4h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	mov	word ptr [bp+0Ch],8h
	cmp	al,78h
	jz	9597h

l0800_9593:
	cmp	al,58h
	jnz	95C8h

l0800_9597:
	mov	word ptr [bp+0Ch],10h

l0800_959C:
	jmp	95B5h

l0800_959E:
	mov	cx,[bp+0Ch]
	xchg	bx,ax
	call	9485h
	xchg	bx,ax
	jc	9550h

l0800_95A8:
	xchg	si,ax
	jmp	95B5h

l0800_95AB:
	xchg	si,ax
	mul	word ptr [bp+0Ch]
	add	si,ax
	adc	di,dx
	jnz	95E5h

l0800_95B5:
	dec	word ptr [bp+0Eh]
	jl	9612h

l0800_95BA:
	inc	word ptr [bp-4h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx

l0800_95C8:
	mov	cx,[bp+0Ch]
	xchg	bx,ax
	call	9485h
	xchg	bx,ax
	jnc	95ABh

l0800_95D2:
	jmp	9602h

l0800_95D4:
	xchg	si,ax
	mul	cx
	xchg	di,ax
	xchg	dx,cx
	mul	dx
	add	si,di
	adc	ax,cx
	xchg	di,ax
	adc	dl,dh
	jnz	9636h

l0800_95E5:
	dec	word ptr [bp+0Eh]
	jl	9612h

l0800_95EA:
	inc	word ptr [bp-4h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	mov	cx,[bp+0Ch]
	xchg	bx,ax
	call	9485h
	xchg	bx,ax
	jnc	95D4h

l0800_9602:
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	ax
	call	word ptr [bp+6h]
	add	sp,6h
	dec	word ptr [bp-4h]

l0800_9612:
	mov	dx,di
	xchg	si,ax
	cmp	byte ptr [bp-1h],0h
	jz	9622h

l0800_961B:
	neg	dx
	neg	ax
	sbb	dx,0h

l0800_9622:
	les	di,[bp+10h]
	mov	bx,[bp-4h]
	add	es:[di],bx
	les	di,[bp+14h]
	mov	bx,[bp-6h]
	mov	es:[di],bx
	jmp	964Ch

l0800_9636:
	mov	ax,0FFFFh
	mov	dx,7FFFh
	add	al,[bp-1h]
	adc	ah,0h
	adc	dx,0h
	mov	word ptr [bp-6h],2h
	jmp	9622h

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
	mov	ah,2Bh
	les	si,[bp+4h]
	mov	cx,es:[si]
	mov	dx,es:[si+2h]
	int	21h
	pop	si
	pop	bp
	ret

;; fn0800_9667: 0800:9667
fn0800_9667 proc
	push	bp
	mov	bp,sp
	push	si
	mov	ah,2Dh
	les	si,[bp+4h]
	mov	cx,es:[si]
	mov	dx,es:[si+2h]
	int	21h
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
	sub	sp,0Ch
	push	ss
	lea	ax,[bp-4h]
	push	ax
	call	8B95h
	pop	cx
	pop	cx
	push	ss
	lea	ax,[bp-8h]
	push	ax
	call	8BA8h
	pop	cx
	pop	cx
	push	ss
	lea	ax,[bp-8h]
	push	ax
	push	ss
	lea	ax,[bp-4h]
	push	ax
	call	0C04Fh
	add	sp,8h
	mov	[bp-0Ah],dx
	mov	[bp-0Ch],ax
	mov	ax,[bp+4h]
	or	ax,[bp+6h]
	jz	97ACh

l0800_979C:
	les	bx,[bp+4h]
	mov	ax,[bp-0Ah]
	mov	dx,[bp-0Ch]
	mov	es:[bx+2h],ax
	mov	es:[bx],dx

l0800_97AC:
	mov	dx,[bp-0Ah]
	mov	ax,[bp-0Ch]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_97B6: 0800:97B6
;;   Called from:
;;     0800:AD3E (in fn0800_AD2F)
fn0800_97B6 proc
	push	bp
	mov	bp,sp
	mov	ax,1h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+4h]
	call	8E29h
	add	sp,8h
	pop	bp
	ret

;; fn0800_97CC: 0800:97CC
;;   Called from:
;;     0800:0C36 (in fn0800_0C29)
;;     0800:0C79 (in fn0800_0C6C)
fn0800_97CC proc
	push	bp
	mov	bp,sp
	mov	dx,[bp+4h]
	cmp	dx,0FFh
	jnz	97DCh

l0800_97D7:
	mov	ax,0FFFFh
	jmp	97F6h

l0800_97DC:
	mov	al,dl
	mov	ah,0h
	mov	bx,ax
	test	byte ptr [bx+2251h],8h
	jz	97F2h

l0800_97E9:
	mov	al,dl
	mov	ah,0h
	add	ax,0FFE0h
	jmp	97F6h

l0800_97F2:
	mov	al,dl
	mov	ah,0h

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
	mov	ah,41h
	lds	dx,[bp+4h]
	int	21h
	pop	ds
	jc	980Ah

l0800_9806:
	xor	ax,ax
	jmp	980Eh

l0800_980A:
	push	ax
	call	8D2Bh

l0800_980E:
	pop	bp
	ret

;; fn0800_9810: 0800:9810
;;   Called from:
;;     0800:9A7A (in fn0800_9828)
;;     0800:9A83 (in fn0800_9828)
fn0800_9810 proc
	mov	al,dh
	call	9817h
	mov	al,dl

;; fn0800_9817: 0800:9817
;;   Called from:
;;     0800:9812 (in fn0800_9810)
;;     0800:9815 (in fn0800_9810)
fn0800_9817 proc
	aam	10h
	xchg	al,ah
	call	9820h
	xchg	al,ah

;; fn0800_9820: 0800:9820
;;   Called from:
;;     0800:981B (in fn0800_9817)
;;     0800:981E (in fn0800_9817)
fn0800_9820 proc
	add	al,90h
	daa
	adc	al,40h
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
	sub	sp,96h
	push	si
	push	di
	mov	word ptr [bp-12h],0h
	mov	word ptr [bp-14h],50h
	mov	word ptr [bp-16h],0h
	jmp	988Ch

;; fn0800_9842: 0800:9842
;;   Called from:
;;     0800:9AF4 (in fn0800_9828)
;;     0800:9B54 (in fn0800_9828)
;;     0800:9B86 (in fn0800_9828)
fn0800_9842 proc
	push	di
	mov	cx,0FFFFh
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
	dec	byte ptr [bp-14h]
	jnz	988Bh

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
	lea	ax,[bp+0FF6Ah]
	sub	di,ax
	push	ss
	lea	ax,[bp+0FF6Ah]
	push	ax
	push	di
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	call	word ptr [bp+0Eh]
	or	ax,ax
	jnz	987Bh

l0800_9876:
	mov	word ptr [bp-16h],1h

l0800_987B:
	mov	word ptr [bp-14h],50h
	add	[bp-12h],di
	lea	di,[bp+0FF6Ah]
	pop	es
	pop	dx
	pop	cx
	pop	bx

l0800_988B:
	ret

l0800_988C:
	push	es
	cld
	lea	di,[bp+0FF6Ah]
	mov	[bp-4h],di

l0800_9895:
	mov	di,[bp-4h]

l0800_9898:
	les	si,[bp+6h]

l0800_989B:
	lodsb	al,es:[si]
	or	al,al
	jz	98B3h

l0800_98A1:
	cmp	al,25h
	jz	98B6h

l0800_98A5:
	mov	ss:[di],al
	inc	di
	dec	byte ptr [bp-14h]
	jg	989Bh

l0800_98AE:
	call	9858h
	jmp	989Bh

l0800_98B3:
	jmp	9C96h

l0800_98B6:
	mov	[bp-10h],si
	lodsb	al,es:[si]
	cmp	al,25h
	jz	98A5h

l0800_98BF:
	mov	[bp-4h],di
	xor	cx,cx
	mov	[bp-0Eh],cx
	mov	word ptr [bp-2h],20h
	mov	[bp-0Bh],cl
	mov	word ptr [bp-8h],0FFFFh
	mov	word ptr [bp-0Ah],0FFFFh
	jmp	98DDh

l0800_98DB:
	lodsb	al,es:[si]

l0800_98DD:
	xor	ah,ah
	mov	dx,ax
	mov	bx,ax
	sub	bl,20h
	cmp	bl,60h
	jnc	98FEh

l0800_98EB:
	mov	bl,[bx+2605h]
	cmp	bx,17h
	jbe	98F7h

l0800_98F4:
	jmp	9C82h

l0800_98F7:
	shl	bx,1h
	jmp	word ptr cs:[bx+9CB6h]

l0800_98FE:
	jmp	9C82h

l0800_9901:
	cmp	ch,0h
	ja	98FEh

l0800_9906:
	or	word ptr [bp-2h],1h
	jmp	98DBh

l0800_990C:
	cmp	ch,0h
	ja	98FEh

l0800_9911:
	or	word ptr [bp-2h],2h
	jmp	98DBh

l0800_9917:
	cmp	ch,0h
	ja	98FEh

l0800_991C:
	cmp	byte ptr [bp-0Bh],2Bh
	jz	9925h

l0800_9922:
	mov	[bp-0Bh],dl

l0800_9925:
	jmp	98DBh

l0800_9927:
	and	word ptr [bp-2h],0DFh
	jmp	9931h

l0800_992D:
	or	word ptr [bp-2h],20h

l0800_9931:
	mov	ch,5h
	jmp	98DBh

l0800_9935:
	cmp	ch,0h
	ja	9987h

l0800_993A:
	test	word ptr [bp-2h],2h
	jnz	996Ah

l0800_9941:
	or	word ptr [bp-2h],8h
	mov	ch,1h
	jmp	98DBh

l0800_9949:
	jmp	9C82h

l0800_994C:
	mov	di,[bp+4h]
	mov	ax,ss:[di]
	add	word ptr [bp+4h],2h
	cmp	ch,2h
	jnc	996Dh

l0800_995B:
	or	ax,ax
	jns	9965h

l0800_995F:
	neg	ax
	or	word ptr [bp-2h],2h

l0800_9965:
	mov	[bp-8h],ax
	mov	ch,3h

l0800_996A:
	jmp	98DBh

l0800_996D:
	cmp	ch,4h
	jnz	9949h

l0800_9972:
	mov	[bp-0Ah],ax
	inc	ch
	jmp	98DBh

l0800_997A:
	cmp	ch,4h
	jnc	9949h

l0800_997F:
	mov	ch,4h
	inc	word ptr [bp-0Ah]
	jmp	98DBh

l0800_9987:
	xchg	dx,ax
	sub	al,30h
	cbw
	cmp	ch,2h
	ja	99A9h

l0800_9990:
	mov	ch,2h
	xchg	[bp-8h],ax
	or	ax,ax
	jl	996Ah

l0800_9999:
	shl	ax,1h
	mov	dx,ax
	shl	ax,1h
	shl	ax,1h
	add	ax,dx
	add	[bp-8h],ax
	jmp	98DBh

l0800_99A9:
	cmp	ch,4h
	jnz	9949h

l0800_99AE:
	xchg	[bp-0Ah],ax
	or	ax,ax
	jz	996Ah

l0800_99B5:
	shl	ax,1h
	mov	dx,ax
	shl	ax,1h
	shl	ax,1h
	add	ax,dx
	add	[bp-0Ah],ax
	jmp	98DBh

l0800_99C5:
	or	word ptr [bp-2h],10h
	jmp	9931h

l0800_99CC:
	or	word ptr [bp-2h],100h

l0800_99D1:
	and	word ptr [bp-2h],0EFh
	jmp	9931h

l0800_99D8:
	mov	bh,8h
	jmp	99E6h

l0800_99DC:
	mov	bh,0Ah
	jmp	99EAh

l0800_99E0:
	mov	bh,10h
	mov	bl,0E9h
	add	bl,dl

l0800_99E6:
	mov	byte ptr [bp-0Bh],0h

l0800_99EA:
	mov	[bp-5h],dl
	xor	dx,dx
	mov	[bp-6h],dl
	mov	di,[bp+4h]
	mov	ax,ss:[di]
	jmp	9A0Ah

l0800_99FA:
	mov	bh,0Ah
	mov	byte ptr [bp-6h],1h
	mov	[bp-5h],dl
	mov	di,[bp+4h]
	mov	ax,ss:[di]
	cwd

l0800_9A0A:
	inc	di
	inc	di
	mov	[bp+6h],si
	test	word ptr [bp-2h],10h
	jz	9A1Bh

l0800_9A16:
	mov	dx,ss:[di]
	inc	di
	inc	di

l0800_9A1B:
	mov	[bp+4h],di
	lea	di,[bp-45h]
	or	ax,ax
	jnz	9A32h

l0800_9A25:
	or	dx,dx
	jnz	9A32h

l0800_9A29:
	cmp	word ptr [bp-0Ah],0h
	jnz	9A36h

l0800_9A2F:
	jmp	9895h

l0800_9A32:
	or	word ptr [bp-2h],4h

l0800_9A36:
	push	dx
	push	ax
	push	ss
	push	di
	mov	al,bh
	cbw
	push	ax
	mov	al,[bp-6h]
	push	ax
	push	bx
	call	8D87h
	push	ss
	pop	es
	mov	dx,[bp-0Ah]
	or	dx,dx
	jge	9A52h

l0800_9A4F:
	jmp	9B46h

l0800_9A52:
	jmp	9B54h

l0800_9A55:
	mov	[bp-5h],dl
	mov	[bp+6h],si
	lea	di,[bp-46h]
	mov	bx,[bp+4h]
	push	word ptr ss:[bx]
	inc	bx
	inc	bx
	mov	[bp+4h],bx
	test	word ptr [bp-2h],20h
	jz	9A80h

l0800_9A70:
	mov	dx,ss:[bx]
	inc	bx
	inc	bx
	mov	[bp+4h],bx
	push	ss
	pop	es
	call	9810h
	mov	al,3Ah
	stosb

l0800_9A80:
	push	ss
	pop	es
	pop	dx
	call	9810h
	mov	byte ptr ss:[di],0h
	mov	byte ptr [bp-6h],0h
	and	word ptr [bp-2h],0FBh
	lea	cx,[bp-46h]
	sub	di,cx
	xchg	di,cx
	mov	dx,[bp-0Ah]
	cmp	dx,cx
	jg	9AA2h

l0800_9AA0:
	mov	dx,cx

l0800_9AA2:
	jmp	9B46h

l0800_9AA5:
	mov	[bp+6h],si
	mov	[bp-5h],dl
	mov	di,[bp+4h]
	mov	ax,ss:[di]
	add	word ptr [bp+4h],2h
	push	ss
	pop	es
	lea	di,[bp-45h]
	xor	ah,ah
	mov	ss:[di],ax
	mov	cx,1h
	jmp	9B89h

l0800_9AC5:
	mov	[bp+6h],si
	mov	[bp-5h],dl
	mov	di,[bp+4h]
	test	word ptr [bp-2h],20h
	jnz	9AE2h

l0800_9AD5:
	mov	di,ss:[di]
	add	word ptr [bp+4h],2h
	push	ds
	pop	es
	or	di,di
	jmp	9AEDh

l0800_9AE2:
	les	di,ss:[di]
	add	word ptr [bp+4h],4h
	mov	ax,es
	or	ax,di

l0800_9AED:
	jnz	9AF4h

l0800_9AEF:
	push	ds
	pop	es
	mov	di,25FEh

l0800_9AF4:
	call	9842h
	cmp	cx,[bp-0Ah]
	jbe	9AFFh

l0800_9AFC:
	mov	cx,[bp-0Ah]

l0800_9AFF:
	jmp	9B89h

l0800_9B02:
	mov	[bp+6h],si
	mov	[bp-5h],dl
	mov	di,[bp+4h]
	mov	cx,[bp-0Ah]
	or	cx,cx
	jge	9B15h

l0800_9B12:
	mov	cx,6h

l0800_9B15:
	push	ss
	push	di
	push	cx
	push	ss
	lea	bx,[bp-45h]
	push	bx
	push	dx
	mov	ax,1h
	and	ax,[bp-2h]
	push	ax
	mov	ax,[bp-2h]
	test	ax,100h
	jz	9B36h

l0800_9B2D:
	mov	ax,8h
	add	word ptr [bp+4h],0Ah
	jmp	9B3Dh

l0800_9B36:
	add	word ptr [bp+4h],8h
	mov	ax,6h

l0800_9B3D:
	push	ax
	call	0A2CCh
	push	ss
	pop	es
	lea	di,[bp-45h]

l0800_9B46:
	test	word ptr [bp-2h],8h
	jz	9B65h

l0800_9B4D:
	mov	dx,[bp-8h]
	or	dx,dx
	jle	9B65h

l0800_9B54:
	call	9842h
	cmp	byte ptr es:[di],2Dh
	jnz	9B5Eh

l0800_9B5D:
	dec	cx

l0800_9B5E:
	sub	dx,cx
	jle	9B65h

l0800_9B62:
	mov	[bp-0Eh],dx

l0800_9B65:
	cmp	byte ptr es:[di],2Dh
	jz	9B76h

l0800_9B6B:
	mov	al,[bp-0Bh]
	or	al,al
	jz	9B86h

l0800_9B72:
	dec	di
	mov	es:[di],al

l0800_9B76:
	cmp	word ptr [bp-0Eh],0h
	jle	9B86h

l0800_9B7C:
	mov	cx,[bp-0Ah]
	or	cx,cx
	jge	9B86h

l0800_9B83:
	dec	word ptr [bp-0Eh]

l0800_9B86:
	call	9842h

l0800_9B89:
	mov	si,di
	mov	di,[bp-4h]
	mov	bx,[bp-8h]
	mov	ax,5h
	and	ax,[bp-2h]
	cmp	ax,5h
	jnz	9BAFh

l0800_9B9C:
	mov	ah,[bp-5h]
	cmp	ah,6Fh
	jnz	9BB1h

l0800_9BA4:
	cmp	word ptr [bp-0Eh],0h
	jg	9BAFh

l0800_9BAA:
	mov	word ptr [bp-0Eh],1h

l0800_9BAF:
	jmp	9BCCh

l0800_9BB1:
	cmp	ah,78h
	jz	9BBBh

l0800_9BB6:
	cmp	ah,58h
	jnz	9BCCh

l0800_9BBB:
	or	word ptr [bp-2h],40h
	dec	bx
	dec	bx
	sub	word ptr [bp-0Eh],2h
	jge	9BCCh

l0800_9BC7:
	mov	word ptr [bp-0Eh],0h

l0800_9BCC:
	add	cx,[bp-0Eh]
	test	word ptr [bp-2h],2h
	jnz	9BE2h

l0800_9BD6:
	jmp	9BDEh

l0800_9BD8:
	mov	al,20h
	call	984Fh
	dec	bx

l0800_9BDE:
	cmp	bx,cx
	jg	9BD8h

l0800_9BE2:
	test	word ptr [bp-2h],40h
	jz	9BF4h

l0800_9BE9:
	mov	al,30h
	call	984Fh
	mov	al,[bp-5h]
	call	984Fh

l0800_9BF4:
	mov	dx,[bp-0Eh]
	or	dx,dx
	jle	9C22h

l0800_9BFB:
	sub	cx,dx
	sub	bx,dx
	mov	al,es:[si]
	cmp	al,2Dh
	jz	9C0Eh

l0800_9C06:
	cmp	al,20h
	jz	9C0Eh

l0800_9C0A:
	cmp	al,2Bh
	jnz	9C15h

l0800_9C0E:
	lodsb	al,es:[si]
	call	984Fh
	dec	cx
	dec	bx

l0800_9C15:
	xchg	dx,cx
	jcxz	9C20h

l0800_9C19:
	mov	al,30h
	call	984Fh
	loop	9C19h

l0800_9C20:
	xchg	dx,cx

l0800_9C22:
	jcxz	9C36h

l0800_9C24:
	sub	bx,cx

l0800_9C26:
	lodsb	al,es:[si]
	mov	ss:[di],al
	inc	di
	dec	byte ptr [bp-14h]
	jg	9C34h

l0800_9C31:
	call	9858h

l0800_9C34:
	loop	9C26h

l0800_9C36:
	or	bx,bx
	jle	9C43h

l0800_9C3A:
	mov	cx,bx

l0800_9C3C:
	mov	al,20h
	call	984Fh
	loop	9C3Ch

l0800_9C43:
	jmp	9898h

l0800_9C46:
	mov	[bp+6h],si
	mov	di,[bp+4h]
	test	word ptr [bp-2h],20h
	jnz	9C5Eh

l0800_9C53:
	mov	di,ss:[di]
	add	word ptr [bp+4h],2h
	push	ds
	pop	es
	jmp	9C65h

l0800_9C5E:
	les	di,ss:[di]
	add	word ptr [bp+4h],4h

l0800_9C65:
	mov	ax,50h
	sub	al,[bp-14h]
	add	ax,[bp-12h]
	mov	es:[di],ax
	test	word ptr [bp-2h],10h
	jz	9C7Fh

l0800_9C78:
	inc	di
	inc	di
	mov	word ptr es:[di],0h

l0800_9C7F:
	jmp	9895h

l0800_9C82:
	mov	si,[bp-10h]
	mov	es,[bp+8h]
	mov	di,[bp-4h]
	mov	al,25h

l0800_9C8D:
	call	984Fh
	lodsb	al,es:[si]
	or	al,al
	jnz	9C8Dh

l0800_9C96:
	cmp	byte ptr [bp-14h],50h
	jge	9C9Fh

l0800_9C9C:
	call	9858h

l0800_9C9F:
	pop	es
	cmp	word ptr [bp-16h],0h
	jz	9CABh

l0800_9CA6:
	mov	ax,0FFFFh
	jmp	9CAEh

l0800_9CAB:
	mov	ax,[bp-12h]

l0800_9CAE:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	0Ch
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
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	0BFC7h
	pop	cx
	pop	cx
	mov	si,ax
	inc	ax
	push	ax
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0B03Bh
	add	sp,0Ah
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
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
	cmp	dx,cs:[9D35h]
	jz	9D7Fh

l0800_9D48:
	mov	ds,dx
	mov	ds,[0002h]
	cmp	word ptr [0002h],0h
	jz	9D5Ch

l0800_9D55:
	mov	cs:[9D37h],ds
	jmp	9D94h

l0800_9D5C:
	mov	ax,ds
	cmp	ax,cs:[9D35h]
	jz	9D7Ah

l0800_9D65:
	mov	ax,[0008h]
	mov	cs:[9D37h],ax
	push	ds
	xor	ax,ax
	push	ax
	call	9E15h
	mov	ds,cs:[9D3Bh]
	jmp	9D9Dh

l0800_9D7A:
	mov	dx,cs:[9D35h]

l0800_9D7F:
	mov	word ptr cs:[9D35h],0h
	mov	word ptr cs:[9D37h],0h
	mov	word ptr cs:[9D39h],0h

l0800_9D94:
	mov	ds,cs:[9D3Bh]
	push	dx
	xor	ax,ax
	push	ax

l0800_9D9D:
	call	0A1D6h
	add	sp,4h
	ret

;; fn0800_9DA4: 0800:9DA4
;;   Called from:
;;     0800:9E92 (in fn0800_9E75)
fn0800_9DA4 proc
	mov	ds,dx
	push	ds
	mov	es,[0002h]
	mov	word ptr [0002h],0h
	mov	[0008h],es
	cmp	dx,cs:[9D35h]
	jz	9DEAh

l0800_9DBC:
	cmp	word ptr es:[0002h],0h
	jnz	9DEAh

l0800_9DC4:
	mov	ax,[0000h]
	pop	bx
	push	es
	add	es:[0000h],ax
	mov	cx,es
	add	dx,ax
	mov	es,dx
	cmp	word ptr es:[0002h],0h
	jnz	9DE3h

l0800_9DDC:
	mov	es:[0008h],cx
	jmp	9DEDh

l0800_9DE3:
	mov	es:[0002h],cx
	jmp	9DEDh

l0800_9DEA:
	call	9E3Eh

l0800_9DED:
	pop	es
	mov	ax,es
	add	ax,es:[0000h]
	mov	ds,ax
	cmp	word ptr [0002h],0h
	jz	9DFFh

l0800_9DFE:
	ret

l0800_9DFF:
	mov	ax,[0000h]
	add	es:[0000h],ax
	mov	ax,es
	mov	bx,ds
	add	bx,[0000h]
	mov	es,bx
	mov	es:[0002h],ax

;; fn0800_9E15: 0800:9E15
;;   Called from:
;;     0800:9D70 (in fn0800_9D41)
;;     0800:9E11 (in fn0800_9DA4)
;;     0800:9FEF (in fn0800_9F9F)
fn0800_9E15 proc
	mov	bx,ds
	cmp	bx,[0006h]
	jz	9E36h

l0800_9E1D:
	mov	es,[0006h]
	mov	ds,[0004h]
	mov	[0006h],es
	mov	es:[0004h],ds
	mov	cs:[9D39h],ds
	mov	ds,bx
	ret

l0800_9E36:
	mov	word ptr cs:[9D39h],0h
	ret

;; fn0800_9E3E: 0800:9E3E
;;   Called from:
;;     0800:9DEA (in fn0800_9DA4)
fn0800_9E3E proc
	mov	ax,cs:[9D39h]
	or	ax,ax
	jz	9E67h

l0800_9E46:
	mov	bx,ss
	pushf
	cli
	mov	ss,ax
	mov	es,ss:[0006h]
	mov	ss:[0006h],ds
	mov	[0004h],ss
	mov	ss,bx
	popf
	mov	es:[0004h],ds
	mov	[0006h],es
	ret

l0800_9E67:
	mov	cs:[9D39h],ds
	mov	[0004h],ds
	mov	[0006h],ds
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
	mov	cs:[9D3Bh],ds
	mov	dx,[bp+6h]
	or	dx,dx
	jz	9E95h

l0800_9E86:
	cmp	dx,cs:[9D37h]
	jnz	9E92h

l0800_9E8D:
	call	9D41h
	jmp	9E95h

l0800_9E92:
	call	9DA4h

l0800_9E95:
	mov	ds,cs:[9D3Bh]
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_9E9E: 0800:9E9E
;;   Called from:
;;     0800:9FDE (in fn0800_9F9F)
fn0800_9E9E proc
	push	ax
	mov	ds,cs:[9D3Bh]
	xor	ax,ax
	push	ax
	push	ax
	call	0A215h
	add	sp,4h
	and	ax,0Fh
	jz	9EC7h

l0800_9EB3:
	mov	dx,10h
	sub	dx,ax
	xor	ax,ax
	mov	ds,cs:[9D3Bh]
	push	ax
	push	dx
	call	0A215h
	add	sp,4h

l0800_9EC7:
	pop	ax
	push	ax
	xor	bx,bx
	mov	bl,ah
	mov	cl,4h
	shr	bx,cl
	shl	ax,cl
	mov	ds,cs:[9D3Bh]
	push	bx
	push	ax
	call	0A215h
	add	sp,4h
	pop	bx
	cmp	ax,0FFFFh
	jz	9EFEh

l0800_9EE6:
	mov	cs:[9D35h],dx
	mov	cs:[9D37h],dx
	mov	ds,dx
	mov	[0000h],bx
	mov	[0002h],dx
	mov	ax,4h
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
	mov	cl,4h
	shr	bx,cl
	shl	ax,cl
	mov	ds,cs:[9D3Bh]
	push	bx
	push	ax
	call	0A215h
	add	sp,4h
	pop	bx
	cmp	ax,0FFFFh
	jz	9F58h

l0800_9F20:
	and	ax,0Fh
	jnz	9F3Dh

l0800_9F25:
	mov	cx,cs:[9D37h]
	mov	cs:[9D37h],dx
	mov	ds,dx
	mov	[0000h],bx
	mov	[0002h],cx
	mov	ax,4h
	ret

l0800_9F3D:
	push	bx
	push	dx
	neg	ax
	add	ax,10h
	xor	bx,bx
	push	bx
	push	ax
	call	0A215h
	add	sp,4h
	pop	dx
	pop	bx
	cmp	ax,0FFFFh
	jz	9F58h

l0800_9F55:
	inc	dx
	jmp	9F25h

l0800_9F58:
	xor	ax,ax
	cwd
	ret

;; fn0800_9F5C: 0800:9F5C
;;   Called from:
;;     0800:9FE3 (in fn0800_9F9F)
fn0800_9F5C proc
	mov	bx,dx
	sub	[0000h],ax
	add	dx,[0000h]
	mov	ds,dx
	mov	[0000h],ax
	mov	[0002h],bx
	mov	bx,dx
	add	bx,[0000h]
	mov	ds,bx
	mov	[0002h],dx
	mov	ax,4h
	ret

;; fn0800_9F7F: 0800:9F7F
;;   Called from:
;;     0800:BB4D (in fn0800_BA89)
fn0800_9F7F proc
	push	bp
	mov	bp,sp
	xor	dx,dx
	mov	ax,[bp+4h]
	jmp	9F92h

;; fn0800_9F89: 0800:9F89
;;   Called from:
;;     0800:431D (in fn0800_4311)
;;     0800:A013 (in fn0800_A006)
fn0800_9F89 proc
	push	bp
	mov	bp,sp
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]

;; fn0800_9F92: 0800:9F92
;;   Called from:
;;     0800:9F87 (in fn0800_9F7F)
;;     0800:9F8F (in fn0800_9F89)
fn0800_9F92 proc
	mov	cx,ax
	or	cx,dx
	push	si
	push	di
	mov	cs:[9D3Bh],ds
	jz	9FFDh

;; fn0800_9F9F: 0800:9F9F
;;   Called from:
;;     0800:9F9D (in fn0800_9F92)
;;     0800:9F9D (in fn0800_9F92)
;;     0800:9F9D (in fn0800_9F92)
fn0800_9F9F proc
	add	ax,13h
	adc	dx,0h
	jc	9FE8h

l0800_9FA7:
	test	dx,0FFF0h
	jnz	9FE8h

l0800_9FAD:
	mov	cl,4h
	shr	ax,cl
	shl	dx,cl
	or	ah,dl
	mov	dx,cs:[9D35h]
	or	dx,dx
	jz	9FDEh

l0800_9FBE:
	mov	dx,cs:[9D39h]
	or	dx,dx
	jz	9FD9h

l0800_9FC7:
	mov	bx,dx

l0800_9FC9:
	mov	ds,dx
	cmp	[0000h],ax
	jnc	9FEDh

l0800_9FD1:
	mov	dx,[0006h]
	cmp	dx,bx
	jnz	9FC9h

l0800_9FD9:
	call	9F02h
	jmp	9FFDh

l0800_9FDE:
	call	9E9Eh
	jmp	9FFDh

l0800_9FE3:
	call	9F5Ch
	jmp	9FFDh

l0800_9FE8:
	xor	ax,ax
	cwd
	jmp	9FFDh

l0800_9FED:
	ja	9FE3h

l0800_9FEF:
	call	9E15h
	mov	bx,[0008h]
	mov	[0002h],bx
	mov	ax,4h

l0800_9FFD:
	mov	ds,cs:[9D3Bh]
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_A006: 0800:A006
fn0800_A006 proc
	push	bx
	mov	si,cs:[9D3Dh]
	push	si
	mov	si,cs:[9D3Fh]
	push	si
	call	9F89h
	add	sp,4h
	or	dx,dx
	jnz	0A01Fh

l0800_A01D:
	pop	bx
	ret

l0800_A01F:
	pop	ds
	mov	es,dx
	push	es
	push	ds
	push	bx
	mov	dx,[0000h]
	cld
	dec	dx
	mov	di,4h
	mov	si,di
	mov	cx,6h
	rep movsw
	or	dx,dx
	jz	0A070h

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
	cmp	cx,1000h
	jbe	0A052h

l0800_A04F:
	mov	cx,1000h

l0800_A052:
	shl	cx,1h
	shl	cx,1h
	shl	cx,1h
	rep movsw
	sub	dx,1000h
	jbe	0A070h

l0800_A060:
	mov	ax,es
	add	ax,1000h
	mov	es,ax
	mov	ax,ds
	add	ax,1000h
	mov	ds,ax
	jmp	0A043h

l0800_A070:
	mov	ds,cs:[9D3Bh]
	call	9E75h
	add	sp,4h
	pop	dx
	mov	ax,4h
	ret

;; fn0800_A080: 0800:A080
fn0800_A080 proc
	cmp	bx,cs:[9D37h]
	jz	0A0CBh

l0800_A087:
	mov	di,bx
	add	di,ax
	mov	es,di
	mov	si,cx
	sub	si,ax
	mov	es:[0000h],si
	mov	es:[0002h],bx
	push	es
	push	ax
	mov	es,bx
	mov	es:[0000h],ax
	mov	dx,bx
	add	dx,cx
	mov	es,dx
	cmp	word ptr es:[0002h],0h
	jz	0A0B8h

l0800_A0B1:
	mov	es:[0002h],di
	jmp	0A0BDh

l0800_A0B8:
	mov	es:[0008h],di

l0800_A0BD:
	mov	si,bx
	call	9E75h
	add	sp,4h
	mov	dx,si
	mov	ax,4h
	ret

l0800_A0CB:
	push	bx
	mov	es,bx
	mov	es:[0000h],ax
	add	bx,ax
	push	bx
	xor	ax,ax
	push	ax
	call	0A1D6h
	add	sp,4h
	pop	dx
	mov	ax,4h
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
	mov	si,[bp+6h]
	inc	si
	sub	si,[007Bh]
	add	si,3Fh
	mov	cl,6h
	shr	si,cl
	cmp	si,[2698h]
	jnz	0A18Dh

l0800_A17B:
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	mov	[008Dh],ax
	mov	[008Bh],dx
	mov	ax,1h
	jmp	0A1D1h

l0800_A18D:
	mov	cl,6h
	shl	si,cl
	mov	dx,[0091h]
	mov	ax,si
	add	ax,[007Bh]
	cmp	ax,dx
	jbe	0A1A5h

l0800_A19F:
	mov	si,dx
	sub	si,[007Bh]

l0800_A1A5:
	push	si
	push	word ptr [007Bh]
	call	0A401h
	pop	cx
	pop	cx
	mov	dx,ax
	cmp	dx,0FFh
	jnz	0A1C1h

l0800_A1B6:
	mov	ax,si
	mov	cl,6h
	shr	ax,cl
	mov	[2698h],ax
	jmp	0A17Bh

l0800_A1C1:
	mov	ax,[007Bh]
	add	ax,dx
	mov	[0091h],ax
	mov	word ptr [008Fh],0h
	xor	ax,ax

l0800_A1D1:
	pop	si
	pop	bp
	ret	4h

;; fn0800_A1D6: 0800:A1D6
;;   Called from:
;;     0800:9D9D (in fn0800_9D41)
;;     0800:A0D8 (in fn0800_A080)
fn0800_A1D6 proc
	push	bp
	mov	bp,sp
	mov	cx,[0089h]
	mov	bx,[0087h]
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
	call	8F2Fh
	jc	0A20Ch

l0800_A1EC:
	mov	cx,[0091h]
	mov	bx,[008Fh]
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
	call	8F2Fh
	ja	0A20Ch

l0800_A1FF:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0A162h
	or	ax,ax
	jnz	0A211h

l0800_A20C:
	mov	ax,0FFFFh
	jmp	0A213h

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
	sub	sp,8h
	mov	ax,[008Dh]
	xor	dx,dx
	mov	cl,4h
	call	8C69h
	add	ax,[008Bh]
	adc	dx,0h
	add	ax,[bp+4h]
	adc	dx,[bp+6h]
	cmp	dx,0Fh
	jl	0A246h

l0800_A237:
	jg	0A23Eh

l0800_A239:
	cmp	ax,0FFFFh
	jbe	0A246h

l0800_A23E:
	mov	dx,0FFFFh
	mov	ax,0FFFFh
	jmp	0A29Fh

l0800_A246:
	mov	dx,[008Dh]
	mov	ax,[008Bh]
	mov	cx,[bp+6h]
	mov	bx,[bp+4h]
	call	8CCBh
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	mov	cx,[0089h]
	mov	bx,[0087h]
	mov	dx,[bp-2h]
	call	8F2Fh
	jc	0A23Eh

l0800_A26C:
	mov	cx,[0091h]
	mov	bx,[008Fh]
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
	call	8F2Fh
	ja	0A23Eh

l0800_A27F:
	mov	ax,[008Dh]
	mov	dx,[008Bh]
	mov	[bp-6h],ax
	mov	[bp-8h],dx
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0A162h
	or	ax,ax
	jz	0A23Eh

l0800_A299:
	mov	dx,[bp-6h]
	mov	ax,[bp-8h]

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
	lds	dx,[bp+8h]
	mov	ah,44h
	mov	al,[bp+6h]
	mov	bx,[bp+4h]
	mov	cx,[bp+0Ch]
	int	21h
	pop	ds
	jc	0A2C6h

l0800_A2BA:
	cmp	word ptr [bp+6h],0h
	jnz	0A2C4h

l0800_A2C0:
	mov	ax,dx
	jmp	0A2CAh

l0800_A2C4:
	jmp	0A2CAh

l0800_A2C6:
	push	ax
	call	8D2Bh

l0800_A2CA:
	pop	bp
	ret

;; fn0800_A2CC: 0800:A2CC
;;   Called from:
;;     0800:9B3E (in fn0800_9828)
fn0800_A2CC proc
	jmp	word ptr [26F4h]

;; fn0800_A2D0: 0800:A2D0
;;   Called from:
;;     0800:91BE (in fn0800_8F97)
fn0800_A2D0 proc
	jmp	word ptr [26F6h]

;; fn0800_A2D4: 0800:A2D4
;;   Called from:
;;     0800:91F4 (in fn0800_8F97)
fn0800_A2D4 proc
	jmp	word ptr [26F8h]

;; fn0800_A2D8: 0800:A2D8
;;   Called from:
;;     0800:91FD (in fn0800_8F97)
;;     0800:9203 (in fn0800_8F97)
fn0800_A2D8 proc
	jmp	word ptr [26FAh]
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
	jz	0A378h

l0800_A371:
	inc	dx
	stosb
	or	al,al
	jnz	0A378h

l0800_A377:
	inc	bx

l0800_A378:
	xchg	al,ah
	xor	al,al
	stc
	jcxz	0A394h

l0800_A37F:
	lodsb
	dec	cx
	sub	al,22h
	jz	0A394h

l0800_A385:
	add	al,22h
	cmp	al,5Ch
	jnz	0A392h

l0800_A38B:
	cmp	byte ptr [si],22h
	jnz	0A392h

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
	mov	ah,4Ah
	mov	bx,[bp+6h]
	mov	es,[bp+4h]
	int	21h
	jc	0A415h

l0800_A410:
	mov	ax,0FFFFh
	jmp	0A41Bh

l0800_A415:
	push	bx
	push	ax
	call	8D2Bh
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
	les	si,[bp+4h]
	cld
	sub	ax,ax
	cwd
	mov	cx,0Ah
	mov	bh,0h
	mov	di,2251h

l0800_A487:
	mov	bl,es:[si]
	inc	si
	test	byte ptr [bx+di],1h
	jnz	0A487h

l0800_A490:
	mov	bp,0h
	cmp	bl,2Bh
	jz	0A49Eh

l0800_A498:
	cmp	bl,2Dh
	jnz	0A4A2h

l0800_A49D:
	inc	bp

l0800_A49E:
	mov	bl,es:[si]
	inc	si

l0800_A4A2:
	cmp	bl,39h
	ja	0A4D6h

l0800_A4A7:
	sub	bl,30h
	jc	0A4D6h

l0800_A4AC:
	mul	cx
	add	ax,bx
	adc	dl,dh
	jz	0A49Eh

l0800_A4B4:
	jmp	0A4C8h

l0800_A4B6:
	mov	di,dx
	mov	cx,0Ah
	mul	cx
	xchg	di,ax
	xchg	cx,dx
	mul	dx
	xchg	dx,ax
	xchg	di,ax
	add	ax,bx
	adc	dx,cx

l0800_A4C8:
	mov	bl,es:[si]
	inc	si
	cmp	bl,39h
	ja	0A4D6h

l0800_A4D1:
	sub	bl,30h
	jnc	0A4B6h

l0800_A4D6:
	dec	bp
	jl	0A4E0h

l0800_A4D9:
	neg	dx
	neg	ax
	sbb	dx,0h

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
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0A53Ch
	add	sp,6h
	mov	dx,ax
	cmp	dx,0FFh
	jnz	0A513h

l0800_A50F:
	mov	ax,dx
	jmp	0A53Ah

l0800_A513:
	and	dx,0FEh
	test	word ptr [bp+8h],80h
	jnz	0A520h

l0800_A51D:
	or	dx,1h

l0800_A520:
	push	dx
	mov	ax,1h
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0A53Ch
	add	sp,8h
	mov	dx,ax
	cmp	dx,0FFh
	jz	0A50Fh

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
	mov	cx,[bp+0Ah]
	mov	ah,43h
	mov	al,[bp+8h]
	lds	dx,[bp+4h]
	int	21h
	pop	ds
	jc	0A553h

l0800_A550:
	xchg	cx,ax
	jmp	0A557h

l0800_A553:
	push	ax
	call	8D2Bh

l0800_A557:
	pop	bp
	ret

;; fn0800_A559: 0800:A559
;;   Called from:
;;     0800:A66F (in fn0800_A614)
fn0800_A559 proc
	push	bp
	mov	bp,sp
	mov	dx,[bp+4h]
	cmp	dx,[24E8h]
	jc	0A56Eh

l0800_A565:
	mov	ax,6h
	push	ax
	call	8D2Bh
	jmp	0A57Dh

l0800_A56E:
	mov	bx,dx
	shl	bx,1h
	mov	word ptr [bx+24EAh],0h
	push	dx
	call	0A57Fh
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
	mov	ah,3Eh
	mov	bx,[bp+4h]
	int	21h
	jc	0A597h

l0800_A58B:
	shl	bx,1h
	mov	word ptr [bx+24EAh],0h
	xor	ax,ax
	jmp	0A59Bh

l0800_A597:
	push	ax
	call	8D2Bh

l0800_A59B:
	pop	bp
	ret

;; fn0800_A59D: 0800:A59D
;;   Called from:
;;     0800:AF76 (in fn0800_AED6)
fn0800_A59D proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	mov	ax,[bp+4h]
	cmp	ax,[24E8h]
	jc	0A5B2h

l0800_A5AC:
	mov	ax,6h
	push	ax
	jmp	0A60Dh

l0800_A5B2:
	mov	bx,[bp+4h]
	shl	bx,1h
	test	word ptr [bx+24EAh],200h
	jz	0A5C4h

l0800_A5BF:
	mov	ax,1h
	jmp	0A610h

l0800_A5C4:
	mov	ax,4400h
	mov	bx,[bp+4h]
	int	21h
	jc	0A60Ch

l0800_A5CE:
	test	dl,80h
	jnz	0A608h

l0800_A5D3:
	mov	ax,4201h
	xor	cx,cx
	mov	dx,cx
	int	21h
	jc	0A60Ch

l0800_A5DE:
	push	dx
	push	ax
	mov	ax,4202h
	xor	cx,cx
	mov	dx,cx
	int	21h
	mov	[bp-4h],ax
	mov	[bp-2h],dx
	pop	dx
	pop	cx
	jc	0A60Ch

l0800_A5F3:
	mov	ax,4200h
	int	21h
	jc	0A60Ch

l0800_A5FA:
	cmp	dx,[bp-2h]
	jc	0A608h

l0800_A5FF:
	ja	0A606h

l0800_A601:
	cmp	ax,[bp-4h]
	jc	0A608h

l0800_A606:
	jmp	0A5BFh

l0800_A608:
	xor	ax,ax
	jmp	0A610h

l0800_A60C:
	push	ax

l0800_A60D:
	call	8D2Bh

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
	mov	si,0FFFFh
	les	bx,[bp+4h]
	mov	ax,es:[bx+12h]
	cmp	ax,[bp+4h]
	jz	0A62Ah

l0800_A627:
	jmp	0A6B2h

l0800_A62A:
	les	bx,[bp+4h]
	cmp	word ptr es:[bx+6h],0h
	jz	0A65Fh

l0800_A634:
	cmp	word ptr es:[bx],0h
	jge	0A647h

l0800_A63A:
	push	word ptr [bp+6h]
	push	bx
	call	0A6B7h
	pop	cx
	pop	cx
	or	ax,ax
	jnz	0A6B2h

l0800_A647:
	les	bx,[bp+4h]
	test	word ptr es:[bx+2h],4h
	jz	0A65Fh

l0800_A652:
	push	word ptr es:[bx+0Ah]
	push	word ptr es:[bx+8h]
	call	9E75h
	pop	cx
	pop	cx

l0800_A65F:
	les	bx,[bp+4h]
	cmp	byte ptr es:[bx+4h],0h
	jl	0A675h

l0800_A669:
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	0A559h
	pop	cx
	mov	si,ax

l0800_A675:
	les	bx,[bp+4h]
	mov	word ptr es:[bx+2h],0h
	mov	word ptr es:[bx+6h],0h
	mov	word ptr es:[bx],0h
	mov	byte ptr es:[bx+4h],0FFh
	cmp	word ptr es:[bx+10h],0h
	jz	0A6B2h

l0800_A695:
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	ax
	push	word ptr es:[bx+10h]
	call	8E6Ah
	push	dx
	push	ax
	call	97F8h
	pop	cx
	pop	cx
	les	bx,[bp+4h]
	mov	word ptr es:[bx+10h],0h

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
	mov	ax,[bp+4h]
	or	ax,[bp+6h]
	jnz	0A6C9h

l0800_A6C3:
	call	0A877h
	jmp	0A778h

l0800_A6C9:
	les	bx,[bp+4h]
	mov	ax,es:[bx+12h]
	cmp	ax,[bp+4h]
	jz	0A6DBh

l0800_A6D5:
	mov	ax,0FFFFh
	jmp	0A77Ah

l0800_A6DB:
	les	bx,[bp+4h]
	cmp	word ptr es:[bx],0h
	jl	0A732h

l0800_A6E4:
	test	word ptr es:[bx+2h],8h
	jnz	0A701h

l0800_A6EC:
	mov	ax,es:[bx+0Eh]
	mov	dx,[bp+4h]
	add	dx,5h
	cmp	ax,[bp+6h]
	jnz	0A778h

l0800_A6FB:
	cmp	es:[bx+0Ch],dx
	jnz	0A778h

l0800_A701:
	les	bx,[bp+4h]
	mov	word ptr es:[bx],0h
	mov	ax,es:[bx+0Eh]
	mov	dx,[bp+4h]
	add	dx,5h
	cmp	ax,[bp+6h]
	jnz	0A778h

l0800_A718:
	cmp	es:[bx+0Ch],dx
	jnz	0A778h

l0800_A71E:
	mov	ax,es:[bx+0Ah]
	mov	dx,es:[bx+8h]
	mov	es:[bx+0Eh],ax
	mov	es:[bx+0Ch],dx
	jmp	0A778h
0800:A730 EB 46                                           .F              

l0800_A732:
	les	bx,[bp+4h]
	mov	ax,es:[bx+6h]
	add	ax,es:[bx]
	inc	ax
	mov	si,ax
	sub	es:[bx],si
	push	ax
	mov	ax,es:[bx+0Ah]
	mov	dx,es:[bx+8h]
	mov	es:[bx+0Eh],ax
	mov	es:[bx+0Ch],dx
	push	ax
	push	dx
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	0C632h
	add	sp,8h
	cmp	ax,si
	jz	0A778h

l0800_A765:
	les	bx,[bp+4h]
	test	word ptr es:[bx+2h],200h
	jnz	0A778h

l0800_A770:
	or	word ptr es:[bx+2h],10h
	jmp	0A6D5h

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
	sub	sp,4h
	push	si
	push	di
	mov	di,[bp+8h]
	xor	cx,cx
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	jmp	0A7A1h

l0800_A798:
	les	bx,[bp-4h]
	mov	es:[bx],cl
	inc	word ptr [bp-4h]

l0800_A7A1:
	cmp	cx,0Ah
	jz	0A7D8h

l0800_A7A6:
	dec	di
	jle	0A7D8h

l0800_A7A9:
	les	bx,[bp+0Ah]
	dec	word ptr es:[bx]
	jl	0A7C6h

l0800_A7B1:
	mov	ax,es:[bx+0Eh]
	mov	si,es:[bx+0Ch]
	inc	word ptr es:[bx+0Ch]
	mov	es,ax
	mov	al,es:[si]
	mov	ah,0h
	jmp	0A7D1h

l0800_A7C6:
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	call	0AEC2h
	pop	cx
	pop	cx

l0800_A7D1:
	mov	cx,ax
	cmp	ax,0FFFFh
	jnz	0A798h

l0800_A7D8:
	cmp	cx,0FFh
	jnz	0A7F3h

l0800_A7DD:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	cmp	ax,[bp+6h]
	jnz	0A7F3h

l0800_A7E8:
	cmp	dx,[bp+4h]
	jnz	0A7F3h

l0800_A7ED:
	xor	dx,dx
	xor	ax,ax
	jmp	0A811h

l0800_A7F3:
	les	bx,[bp-4h]
	mov	byte ptr es:[bx],0h
	les	bx,[bp+0Ah]
	test	word ptr es:[bx+2h],10h
	jz	0A80Bh

l0800_A805:
	xor	dx,dx
	xor	ax,ax
	jmp	0A811h

l0800_A80B:
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]

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
	mov	ah,2Fh
	int	21h
	push	es
	push	bx
	mov	ah,1Ah
	lds	dx,[bp+8h]
	int	21h
	mov	ah,4Eh
	mov	cx,[bp+0Ch]
	lds	dx,[bp+4h]
	int	21h
	pushf
	pop	cx
	xchg	bx,ax
	mov	ah,1Ah
	pop	dx
	pop	ds
	int	21h
	push	cx
	popf
	pop	ds
	jc	0A844h

l0800_A840:
	xor	ax,ax
	jmp	0A848h

l0800_A844:
	push	bx
	call	8D2Bh

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
	mov	ah,2Fh
	int	21h
	push	es
	push	bx
	mov	ah,1Ah
	lds	dx,[bp+4h]
	int	21h
	mov	ah,4Fh
	int	21h
	pushf
	pop	cx
	xchg	bx,ax
	mov	ah,1Ah
	pop	dx
	pop	ds
	int	21h
	push	cx
	popf
	pop	ds
	jc	0A871h

l0800_A86D:
	xor	ax,ax
	jmp	0A875h

l0800_A871:
	push	bx
	call	8D2Bh

l0800_A875:
	pop	bp
	ret

;; fn0800_A877: 0800:A877
;;   Called from:
;;     0800:A6C3 (in fn0800_A6B7)
fn0800_A877 proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	push	si
	push	di
	xor	di,di
	mov	si,[24E8h]
	mov	[bp-2h],ds
	mov	word ptr [bp-4h],2358h
	jmp	0A8A8h

l0800_A88F:
	les	bx,[bp-4h]
	test	word ptr es:[bx+2h],3h
	jz	0A8A4h

l0800_A89A:
	push	word ptr [bp-2h]
	push	bx
	call	0A6B7h
	pop	cx
	pop	cx
	inc	di

l0800_A8A4:
	add	word ptr [bp-4h],14h

l0800_A8A8:
	mov	ax,si
	dec	si
	or	ax,ax
	jnz	0A88Fh

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
	les	bx,[bp+0Ch]
	inc	word ptr [bp+0Ch]
	mov	cl,es:[bx]
	mov	al,cl
	cmp	al,72h
	jnz	0A8D5h

l0800_A8CD:
	mov	dx,1h
	mov	si,1h
	jmp	0A8F3h

l0800_A8D5:
	cmp	cl,77h
	jnz	0A8DFh

l0800_A8DA:
	mov	dx,302h
	jmp	0A8E7h

l0800_A8DF:
	cmp	cl,61h
	jnz	0A8EFh

l0800_A8E4:
	mov	dx,902h

l0800_A8E7:
	mov	di,80h
	mov	si,2h
	jmp	0A8F3h

l0800_A8EF:
	xor	ax,ax
	jmp	0A967h

l0800_A8F3:
	les	bx,[bp+0Ch]
	mov	cl,es:[bx]
	inc	word ptr [bp+0Ch]
	cmp	cl,2Bh
	jz	0A914h

l0800_A901:
	les	bx,[bp+0Ch]
	cmp	byte ptr es:[bx],2Bh
	jnz	0A92Bh

l0800_A90A:
	cmp	cl,74h
	jz	0A914h

l0800_A90F:
	cmp	cl,62h
	jnz	0A92Bh

l0800_A914:
	cmp	cl,2Bh
	jnz	0A91Fh

l0800_A919:
	les	bx,[bp+0Ch]
	mov	cl,es:[bx]

l0800_A91F:
	and	dx,0FCh
	or	dx,4h
	mov	di,180h
	mov	si,3h

l0800_A92B:
	cmp	cl,74h
	jnz	0A936h

l0800_A930:
	or	dx,4000h
	jmp	0A953h

l0800_A936:
	cmp	cl,62h
	jnz	0A941h

l0800_A93B:
	or	dx,8000h
	jmp	0A950h

l0800_A941:
	mov	ax,[2512h]
	and	ax,0C000h
	or	dx,ax
	mov	ax,dx
	test	ax,8000h
	jz	0A953h

l0800_A950:
	or	si,40h

l0800_A953:
	mov	word ptr [2354h],0C7B5h
	les	bx,[bp+8h]
	mov	es:[bx],dx
	les	bx,[bp+4h]
	mov	es:[bx],di
	mov	ax,si

l0800_A967:
	pop	di
	pop	si
	pop	bp
	ret	0Ch

;; fn0800_A96D: 0800:A96D
;;   Called from:
;;     0800:AAAC (in fn0800_AA7E)
fn0800_A96D proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	ss
	lea	ax,[bp-2h]
	push	ax
	push	ss
	lea	ax,[bp-4h]
	push	ax
	call	0A8B7h
	les	bx,[bp+0Eh]
	mov	es:[bx+2h],ax
	or	ax,ax
	jz	0A9B9h

l0800_A991:
	cmp	byte ptr es:[bx+4h],0h
	jge	0A9CDh

l0800_A998:
	push	word ptr [bp-4h]
	mov	ax,[bp-2h]
	or	ax,[bp+4h]
	push	ax
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	call	0B140h
	add	sp,8h
	les	bx,[bp+0Eh]
	mov	es:[bx+4h],al
	or	al,al
	jge	0A9CDh

l0800_A9B9:
	les	bx,[bp+0Eh]
	mov	byte ptr es:[bx+4h],0FFh
	mov	word ptr es:[bx+2h],0h

l0800_A9C7:
	xor	dx,dx
	xor	ax,ax
	jmp	0AA2Eh

l0800_A9CD:
	les	bx,[bp+0Eh]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	8D76h
	pop	cx
	or	ax,ax
	jz	0A9E7h

l0800_A9DE:
	les	bx,[bp+0Eh]
	or	word ptr es:[bx+2h],200h

l0800_A9E7:
	mov	ax,200h
	push	ax
	les	bx,[bp+0Eh]
	test	word ptr es:[bx+2h],200h
	jz	0A9FBh

l0800_A9F6:
	mov	ax,1h
	jmp	0A9FDh

l0800_A9FB:
	xor	ax,ax

l0800_A9FD:
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+10h]
	push	word ptr [bp+0Eh]
	call	0BA89h
	add	sp,0Ch
	or	ax,ax
	jz	0AA1Fh

l0800_AA12:
	push	word ptr [bp+10h]
	push	word ptr [bp+0Eh]
	call	0A614h
	pop	cx
	pop	cx
	jmp	0A9C7h

l0800_AA1F:
	les	bx,[bp+0Eh]
	mov	word ptr es:[bx+10h],0h
	mov	dx,[bp+10h]
	mov	ax,[bp+0Eh]

l0800_AA2E:
	mov	sp,bp
	pop	bp
	ret	0Eh

;; fn0800_AA34: 0800:AA34
;;   Called from:
;;     0800:AA84 (in fn0800_AA7E)
fn0800_AA34 proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	mov	[bp-2h],ds
	mov	word ptr [bp-4h],2358h

l0800_AA42:
	les	bx,[bp-4h]
	cmp	byte ptr es:[bx+4h],0h
	jl	0AA64h

l0800_AA4C:
	mov	ax,[bp-4h]
	add	word ptr [bp-4h],14h
	push	ax
	mov	ax,[24E8h]
	mov	dx,14h
	imul	dx
	add	ax,2358h
	pop	dx
	cmp	dx,ax
	jc	0AA42h

l0800_AA64:
	les	bx,[bp-4h]
	cmp	byte ptr es:[bx+4h],0h
	jl	0AA74h

l0800_AA6E:
	xor	dx,dx
	xor	ax,ax
	jmp	0AA7Ah

l0800_AA74:
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]

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
	sub	sp,4h
	call	0AA34h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	or	ax,dx
	jnz	0AA97h

l0800_AA91:
	xor	dx,dx
	xor	ax,ax
	jmp	0AAAFh

l0800_AA97:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	xor	ax,ax
	push	ax
	call	0A96Dh

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
	jmp	0AB91h

l0800_AABB:
	inc	word ptr [bp+8h]
	les	bx,[bp+4h]
	mov	ax,es:[bx+6h]
	cmp	ax,[bp+8h]
	jbe	0AACFh

l0800_AACA:
	mov	ax,[bp+8h]
	jmp	0AAD6h

l0800_AACF:
	les	bx,[bp+4h]
	mov	ax,es:[bx+6h]

l0800_AAD6:
	mov	di,ax
	les	bx,[bp+4h]
	test	word ptr es:[bx+2h],40h
	jz	0AB50h

l0800_AAE3:
	cmp	word ptr es:[bx+6h],0h
	jz	0AB50h

l0800_AAEA:
	mov	ax,es:[bx+6h]
	cmp	ax,[bp+8h]
	jnc	0AB50h

l0800_AAF3:
	cmp	word ptr es:[bx],0h
	jnz	0AB50h

l0800_AAF9:
	dec	word ptr [bp+8h]
	xor	di,di
	jmp	0AB0Eh

l0800_AB00:
	les	bx,[bp+4h]
	add	di,es:[bx+6h]
	mov	ax,es:[bx+6h]
	sub	[bp+8h],ax

l0800_AB0E:
	les	bx,[bp+4h]
	mov	ax,es:[bx+6h]
	cmp	ax,[bp+8h]
	jbe	0AB00h

l0800_AB1A:
	push	di
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	8F50h
	add	sp,8h
	mov	dx,ax
	add	[bp+0Ah],dx
	cmp	dx,di
	jz	0AB91h

l0800_AB36:
	mov	ax,di
	sub	ax,dx
	add	[bp+8h],ax

l0800_AB3D:
	les	bx,[bp+4h]
	or	word ptr es:[bx+2h],20h
	jmp	0AB9Ah

l0800_AB47:
	les	bx,[bp+0Ah]
	mov	es:[bx],dl
	inc	word ptr [bp+0Ah]

l0800_AB50:
	dec	word ptr [bp+8h]
	mov	ax,[bp+8h]
	or	ax,ax
	jz	0AB8Ch

l0800_AB5A:
	dec	di
	jz	0AB8Ch

l0800_AB5D:
	les	bx,[bp+4h]
	dec	word ptr es:[bx]
	jl	0AB7Ah

l0800_AB65:
	mov	ax,es:[bx+0Eh]
	mov	si,es:[bx+0Ch]
	inc	word ptr es:[bx+0Ch]
	mov	es,ax
	mov	al,es:[si]
	mov	ah,0h
	jmp	0AB85h

l0800_AB7A:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0AEC2h
	pop	cx
	pop	cx

l0800_AB85:
	mov	dx,ax
	cmp	ax,0FFFFh
	jnz	0AB47h

l0800_AB8C:
	cmp	dx,0FFh
	jz	0AB3Dh

l0800_AB91:
	cmp	word ptr [bp+8h],0h
	jz	0AB9Ah

l0800_AB97:
	jmp	0AABBh

l0800_AB9A:
	mov	ax,[bp+8h]
	pop	di
	pop	si
	pop	bp
	ret	0Ah

;; fn0800_ABA3: 0800:ABA3
;;   Called from:
;;     0800:4126 (in fn0800_4110)
fn0800_ABA3 proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	push	si
	push	di
	mov	di,[bp+8h]
	or	di,di
	jnz	0ABB6h

l0800_ABB2:
	xor	ax,ax
	jmp	0AC2Bh

l0800_ABB6:
	mov	bx,di
	xor	cx,cx
	mov	ax,[bp+0Ah]
	xor	dx,dx
	call	8F18h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	cmp	dx,1h
	ja	0ABF2h

l0800_ABCD:
	jc	0ABD3h

l0800_ABCF:
	or	ax,ax
	jnc	0ABF2h

l0800_ABD3:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	word ptr [bp-4h]
	push	word ptr [bp+0Eh]
	push	word ptr [bp+0Ch]
	call	0AAB3h
	mov	dx,[bp-4h]
	sub	dx,ax
	push	dx
	xor	dx,dx
	pop	ax
	div	di
	jmp	0AC2Bh

l0800_ABF2:
	mov	si,[bp+0Ah]
	inc	si
	jmp	0AC0Bh

l0800_ABF8:
	mov	bx,di
	xor	cx,cx
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
	call	8CCBh
	mov	[bp+6h],dx
	mov	[bp+4h],ax

l0800_AC0B:
	dec	si
	mov	ax,si
	or	ax,ax
	jz	0AC26h

l0800_AC12:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	di
	push	word ptr [bp+0Eh]
	push	word ptr [bp+0Ch]
	call	0AAB3h
	or	ax,ax
	jz	0ABF8h

l0800_AC26:
	mov	ax,[bp+0Ah]
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
	sub	sp,4h
	push	si
	les	bx,[bp+4h]
	cmp	word ptr es:[bx],0h
	jge	0AC4Dh

l0800_AC41:
	mov	cx,es:[bx+6h]
	add	cx,es:[bx]
	inc	cx
	mov	si,cx
	jmp	0AC5Ch

l0800_AC4D:
	les	bx,[bp+4h]
	mov	ax,es:[bx]
	cwd
	xor	ax,dx
	sub	ax,dx
	mov	cx,ax
	mov	si,ax

l0800_AC5C:
	les	bx,[bp+4h]
	test	word ptr es:[bx+2h],40h
	jnz	0ACAAh

l0800_AC67:
	les	bx,[bp+4h]
	mov	ax,es:[bx+0Eh]
	mov	dx,es:[bx+0Ch]
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	cmp	word ptr es:[bx],0h
	jge	0ACA3h

l0800_AC7E:
	jmp	0AC8Dh

l0800_AC80:
	dec	word ptr [bp-4h]
	les	bx,[bp-4h]
	cmp	byte ptr es:[bx],0Ah
	jnz	0AC8Dh

l0800_AC8C:
	inc	si

l0800_AC8D:
	mov	ax,cx
	dec	cx
	or	ax,ax
	jnz	0AC80h

l0800_AC94:
	jmp	0ACAAh

l0800_AC96:
	les	bx,[bp-4h]
	inc	word ptr [bp-4h]
	cmp	byte ptr es:[bx],0Ah
	jnz	0ACA3h

l0800_ACA2:
	inc	si

l0800_ACA3:
	mov	ax,cx
	dec	cx
	or	ax,ax
	jnz	0AC96h

l0800_ACAA:
	mov	ax,si
	pop	si
	mov	sp,bp
	pop	bp
	ret	4h

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
	mov	si,[bp+0Ch]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0A6B7h
	pop	cx
	pop	cx
	or	ax,ax
	jz	0ACCEh

l0800_ACC9:
	mov	ax,0FFFFh
	jmp	0AD2Ch

l0800_ACCE:
	cmp	si,1h
	jnz	0ACEAh

l0800_ACD3:
	les	bx,[bp+4h]
	cmp	word ptr es:[bx],0h
	jle	0ACEAh

l0800_ACDC:
	push	word ptr [bp+6h]
	push	bx
	call	0AC31h
	cwd
	sub	[bp+8h],ax
	sbb	[bp+0Ah],dx

l0800_ACEA:
	les	bx,[bp+4h]
	and	word ptr es:[bx+2h],0FE5Fh
	mov	word ptr es:[bx],0h
	mov	ax,es:[bx+0Ah]
	mov	dx,es:[bx+8h]
	mov	es:[bx+0Eh],ax
	mov	es:[bx+0Ch],dx
	push	si
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	8E29h
	add	sp,8h
	cmp	dx,0FFh
	jnz	0AD2Ah

l0800_AD20:
	cmp	ax,0FFFFh
	jnz	0AD2Ah

l0800_AD25:
	mov	ax,0FFFFh
	jmp	0AD2Ch

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
	sub	sp,4h
	les	bx,[bp+4h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	97B6h
	pop	cx
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	cmp	dx,0FFh
	jnz	0AD52h

l0800_AD4D:
	cmp	ax,0FFFFh
	jz	0AD7Bh

l0800_AD52:
	les	bx,[bp+4h]
	cmp	word ptr es:[bx],0h
	jge	0AD6Bh

l0800_AD5B:
	push	word ptr [bp+6h]
	push	bx
	call	0AC31h
	cwd
	add	[bp-4h],ax
	adc	[bp-2h],dx
	jmp	0AD7Bh

l0800_AD6B:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0AC31h
	cwd
	sub	[bp-4h],ax
	sbb	[bp-2h],dx

l0800_AD7B:
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
	mov	sp,bp
	pop	bp
	ret

;; fn0800_AD85: 0800:AD85
;;   Called from:
;;     0800:4168 (in fn0800_4152)
fn0800_AD85 proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	push	si
	push	di
	mov	di,[bp+8h]
	or	di,di
	jz	0AE07h

l0800_AD94:
	mov	bx,di
	xor	cx,cx
	mov	ax,[bp+0Ah]
	xor	dx,dx
	call	8F18h
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	cmp	dx,1h
	ja	0ADC9h

l0800_ADAB:
	jc	0ADB1h

l0800_ADAD:
	or	ax,ax
	jnc	0ADC9h

l0800_ADB1:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	word ptr [bp-4h]
	push	word ptr [bp+0Eh]
	push	word ptr [bp+0Ch]
	call	0B4BEh
	xor	dx,dx
	div	di
	jmp	0AE0Ah

l0800_ADC9:
	xor	si,si
	cmp	si,[bp+0Ah]
	jnc	0AE07h

l0800_ADD0:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	di
	push	word ptr [bp+0Eh]
	push	word ptr [bp+0Ch]
	call	0B4BEh
	xor	dx,dx
	or	dx,dx
	jnz	0ADEAh

l0800_ADE6:
	cmp	ax,di
	jz	0ADEEh

l0800_ADEA:
	mov	ax,si
	jmp	0AE0Ah

l0800_ADEE:
	mov	bx,di
	xor	cx,cx
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
	call	8CCBh
	mov	[bp+6h],dx
	mov	[bp+4h],ax
	inc	si
	cmp	si,[bp+0Ah]
	jc	0ADD0h

l0800_AE07:
	mov	ax,[bp+0Ah]

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
	sub	sp,4h
	push	si
	mov	si,14h
	mov	[bp-2h],ds
	mov	word ptr [bp-4h],2358h
	jmp	0AE40h

l0800_AE24:
	les	bx,[bp-4h]
	mov	ax,es:[bx+2h]
	and	ax,300h
	cmp	ax,300h
	jnz	0AE3Ch

l0800_AE33:
	push	word ptr [bp-2h]
	push	bx
	call	0A6B7h
	pop	cx
	pop	cx

l0800_AE3C:
	add	word ptr [bp-4h],14h

l0800_AE40:
	mov	ax,si
	dec	si
	or	ax,ax
	jnz	0AE24h

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
	les	bx,[bp+4h]
	test	word ptr es:[bx+2h],200h
	jz	0AE5Dh

l0800_AE5A:
	call	0AE10h

l0800_AE5D:
	les	bx,[bp+4h]
	push	word ptr es:[bx+6h]
	mov	ax,es:[bx+0Ah]
	mov	dx,es:[bx+8h]
	mov	es:[bx+0Eh],ax
	mov	es:[bx+0Ch],dx
	push	ax
	push	dx
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	0B97Fh
	add	sp,8h
	les	bx,[bp+4h]
	mov	es:[bx],ax
	or	ax,ax
	jle	0AE95h

l0800_AE8C:
	and	word ptr es:[bx+2h],0DFh
	xor	ax,ax
	jmp	0AEBEh

l0800_AE95:
	les	bx,[bp+4h]
	cmp	word ptr es:[bx],0h
	jnz	0AEAEh

l0800_AE9E:
	mov	ax,es:[bx+2h]
	and	ax,0FE7Fh
	or	ax,20h
	mov	es:[bx+2h],ax
	jmp	0AEBBh

l0800_AEAE:
	les	bx,[bp+4h]
	mov	word ptr es:[bx],0h
	or	word ptr es:[bx+2h],10h

l0800_AEBB:
	mov	ax,0FFFFh

l0800_AEBE:
	pop	bp
	ret	4h

;; fn0800_AEC2: 0800:AEC2
;;   Called from:
;;     0800:3DF9 (in fn0800_3DCF)
;;     0800:3EC4 (in fn0800_3E9A)
;;     0800:A7CC (in fn0800_A77D)
;;     0800:AB80 (in fn0800_AAB3)
fn0800_AEC2 proc
	push	bp
	mov	bp,sp
	les	bx,[bp+4h]
	inc	word ptr es:[bx]
	push	word ptr [bp+6h]
	push	bx
	call	0AED6h
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
	mov	ax,[bp+4h]
	or	ax,[bp+6h]
	jnz	0AEE8h

l0800_AEE2:
	mov	ax,0FFFFh
	jmp	0AFBDh

l0800_AEE8:
	les	bx,[bp+4h]
	cmp	word ptr es:[bx],0h
	jle	0AF0Bh

l0800_AEF1:
	les	bx,[bp+4h]
	dec	word ptr es:[bx]
	mov	ax,es:[bx+0Eh]
	mov	si,es:[bx+0Ch]
	inc	word ptr es:[bx+0Ch]
	mov	es,ax
	mov	al,es:[si]
	jmp	0AFBBh

l0800_AF0B:
	les	bx,[bp+4h]
	cmp	word ptr es:[bx],0h
	jl	0AF7Fh

l0800_AF14:
	test	word ptr es:[bx+2h],110h
	jnz	0AF7Fh

l0800_AF1C:
	test	word ptr es:[bx+2h],1h
	jz	0AF7Fh

l0800_AF24:
	les	bx,[bp+4h]
	or	word ptr es:[bx+2h],80h
	cmp	word ptr es:[bx+6h],0h
	jz	0AF43h

l0800_AF34:
	push	word ptr [bp+6h]
	push	bx
	call	0AE4Ch
	or	ax,ax
	jz	0AEF1h

l0800_AF3F:
	jmp	0AEE2h
0800:AF41    EB AE                                         ..             

l0800_AF43:
	les	bx,[bp+4h]
	test	word ptr es:[bx+2h],200h
	jz	0AF51h

l0800_AF4E:
	call	0AE10h

l0800_AF51:
	mov	ax,1h
	push	ax
	push	ds
	mov	ax,4EE4h
	push	ax
	les	bx,[bp+4h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	0B97Fh
	add	sp,8h
	or	ax,ax
	jnz	0AF9Eh

l0800_AF6D:
	les	bx,[bp+4h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	0A59Dh
	pop	cx
	cmp	ax,1h
	jz	0AF8Ah

l0800_AF7F:
	les	bx,[bp+4h]
	or	word ptr es:[bx+2h],10h
	jmp	0AEE2h

l0800_AF8A:
	les	bx,[bp+4h]
	mov	ax,es:[bx+2h]
	and	ax,0FE7Fh
	or	ax,20h
	mov	es:[bx+2h],ax
	jmp	0AEE2h

l0800_AF9E:
	cmp	byte ptr [4EE4h],0Dh
	jnz	0AFB0h

l0800_AFA5:
	les	bx,[bp+4h]
	test	word ptr es:[bx+2h],40h
	jz	0AF43h

l0800_AFB0:
	les	bx,[bp+4h]
	and	word ptr es:[bx+2h],0DFh
	mov	al,[4EE4h]

l0800_AFBB:
	mov	ah,0h

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
	sub	sp,4h
	push	si
	push	di
	les	di,[bp+4h]
	mov	ax,es
	or	ax,di
	jz	0AFFCh

l0800_AFDC:
	mov	al,0h
	mov	ah,es:[di]
	mov	cx,0FFFFh
	cld

l0800_AFE5:
	repne scasb

l0800_AFE7:
	not	cx
	dec	cx
	jz	0AFFCh

l0800_AFEC:
	les	di,[26AAh]
	mov	[bp-2h],es
	mov	bx,es
	or	bx,di
	mov	[bp-4h],di
	jnz	0B009h

l0800_AFFC:
	xor	dx,dx
	xor	ax,ax
	jmp	0B035h

l0800_B002:
	add	word ptr [bp-4h],4h
	les	di,[bp-4h]

l0800_B009:
	les	di,es:[di]
	mov	bx,es
	or	bx,di
	jz	0AFFCh

l0800_B012:
	mov	al,es:[di]
	or	al,al
	jz	0AFFCh

l0800_B019:
	cmp	ah,al
	jnz	0B002h

l0800_B01D:
	mov	bx,cx
	cmp	byte ptr es:[bx+di],3Dh
	jnz	0B002h

l0800_B025:
	push	ds
	lds	si,[bp+4h]
	rep cmpsb
	pop	ds
	xchg	bx,cx
	jnz	0B002h

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
	les	di,[bp+4h]
	lds	si,[bp+8h]
	mov	cx,[bp+0Ch]
	shr	cx,1h
	cld
	rep movsw
	jnc	0B053h

l0800_B052:
	movsb

l0800_B053:
	mov	ds,dx
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
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
	les	di,[bp+4h]
	mov	cx,[bp+8h]
	mov	al,[bp+0Ah]
	mov	ah,al
	cld
	test	di,1h
	jz	0B079h

l0800_B075:
	jcxz	0B080h

l0800_B077:
	stosb
	dec	cx

l0800_B079:
	shr	cx,1h

l0800_B07B:
	rep stosw

l0800_B07D:
	jnc	0B080h

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
	mov	al,[bp+8h]
	push	ax
	push	word ptr [bp+0Ah]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0B05Fh
	add	sp,8h
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
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
	mov	cx,[bp+0Ah]
	mov	bx,[bp+8h]
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
	call	8F2Fh
	jnc	0B0BEh

l0800_B0B8:
	std
	mov	ax,1h
	jmp	0B0C1h

l0800_B0BE:
	cld
	xor	ax,ax

l0800_B0C1:
	lds	si,[bp+4h]
	les	di,[bp+8h]
	mov	cx,[bp+0Ch]
	or	ax,ax
	jz	0B0D4h

l0800_B0CE:
	add	si,cx
	dec	si
	add	di,cx
	dec	di

l0800_B0D4:
	test	di,1h
	jz	0B0DEh

l0800_B0DA:
	jcxz	0B0EDh

l0800_B0DC:
	movsb
	dec	cx

l0800_B0DE:
	sub	si,ax
	sub	di,ax
	shr	cx,1h
	rep movsw
	jnc	0B0EDh

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
	push	word ptr [bp+0Ch]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	call	0B0A1h
	add	sp,0Ah
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
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
	mov	cx,[bp+4h]
	mov	ah,3Ch
	lds	dx,[bp+6h]
	int	21h
	pop	ds
	jc	0B126h

l0800_B124:
	jmp	0B12Ah

l0800_B126:
	push	ax
	call	8D2Bh

l0800_B12A:
	pop	bp
	ret	6h

;; fn0800_B12E: 0800:B12E
;;   Called from:
;;     0800:B23E (in fn0800_B140)
fn0800_B12E proc
	push	bp
	mov	bp,sp
	mov	bx,[bp+4h]
	sub	cx,cx
	sub	dx,dx
	mov	ah,40h
	int	21h
	pop	bp
	ret	2h

;; fn0800_B140: 0800:B140
;;   Called from:
;;     0800:A9A8 (in fn0800_A96D)
fn0800_B140 proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	push	si
	push	di
	mov	si,[bp+8h]
	mov	di,[bp+0Ah]
	test	si,0C000h
	jnz	0B15Ch

l0800_B154:
	mov	ax,[2512h]
	and	ax,0C000h
	or	si,ax

l0800_B15C:
	xor	ax,ax
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0A53Ch
	add	sp,6h
	mov	[bp-2h],ax
	test	si,100h
	jz	0B1F2h

l0800_B174:
	and	di,[2514h]
	mov	ax,di
	test	ax,180h
	jnz	0B186h

l0800_B17F:
	mov	ax,1h
	push	ax
	call	8D2Bh

l0800_B186:
	cmp	word ptr [bp-2h],0FFh
	jnz	0B1AFh

l0800_B18C:
	cmp	word ptr [2516h],2h
	jz	0B19Dh

l0800_B193:
	push	word ptr [2516h]

l0800_B197:
	call	8D2Bh
	jmp	0B29Ah

l0800_B19D:
	test	di,80h
	jz	0B1A7h

l0800_B1A3:
	xor	ax,ax
	jmp	0B1AAh

l0800_B1A7:
	mov	ax,1h

l0800_B1AA:
	mov	[bp-2h],ax
	jmp	0B1BBh

l0800_B1AF:
	test	si,400h
	jz	0B1F2h

l0800_B1B5:
	mov	ax,50h
	push	ax
	jmp	0B197h

l0800_B1BB:
	test	si,0F0h
	jz	0B1DDh

l0800_B1C1:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	xor	ax,ax
	push	ax
	call	0B113h
	mov	di,ax
	or	ax,ax
	jge	0B1D6h

l0800_B1D3:
	jmp	0B298h

l0800_B1D6:
	push	di
	call	0A57Fh
	pop	cx
	jmp	0B1F2h

l0800_B1DD:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	word ptr [bp-2h]
	call	0B113h
	mov	di,ax
	or	ax,ax
	jge	0B265h

l0800_B1EF:
	jmp	0B298h

l0800_B1F2:
	push	si
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0B2A0h
	add	sp,6h
	mov	di,ax
	or	ax,ax
	jl	0B265h

l0800_B205:
	xor	ax,ax
	push	ax
	push	di
	call	0A2A3h
	pop	cx
	pop	cx
	mov	[bp-4h],ax
	test	ax,80h
	jz	0B237h

l0800_B216:
	or	si,2000h
	test	si,8000h
	jz	0B241h

l0800_B220:
	and	ax,0FFh
	or	ax,20h
	xor	dx,dx
	push	dx
	push	ax
	mov	ax,1h
	push	ax
	push	di
	call	0A2A3h
	add	sp,8h
	jmp	0B241h

l0800_B237:
	test	si,200h
	jz	0B241h

l0800_B23D:
	push	di
	call	0B12Eh

l0800_B241:
	test	word ptr [bp-2h],1h
	jz	0B265h

l0800_B248:
	test	si,100h
	jz	0B265h

l0800_B24E:
	test	si,0F0h
	jz	0B265h

l0800_B254:
	mov	ax,1h
	push	ax
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0A53Ch
	add	sp,8h

l0800_B265:
	or	di,di
	jl	0B298h

l0800_B269:
	test	si,300h
	jz	0B274h

l0800_B26F:
	mov	ax,1000h
	jmp	0B276h

l0800_B274:
	xor	ax,ax

l0800_B276:
	mov	dx,si
	and	dx,0F8FFh
	or	dx,ax
	push	dx
	test	word ptr [bp-2h],1h
	jz	0B28Ah

l0800_B286:
	xor	ax,ax
	jmp	0B28Dh

l0800_B28A:
	mov	ax,100h

l0800_B28D:
	pop	dx
	or	dx,ax
	mov	bx,di
	shl	bx,1h
	mov	[bx+24EAh],dx

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
	sub	sp,2h
	mov	al,1h
	mov	cx,[bp+8h]
	test	cx,2h
	jnz	0B2BBh

l0800_B2B1:
	mov	al,2h
	test	cx,4h
	jnz	0B2BBh

l0800_B2B9:
	mov	al,0h

l0800_B2BB:
	push	ds
	lds	dx,[bp+4h]
	mov	cl,0F0h
	and	cl,[bp+8h]
	or	al,cl
	mov	ah,3Dh
	int	21h
	pop	ds
	jc	0B2E7h

l0800_B2CD:
	mov	[bp-2h],ax
	mov	ax,[bp+8h]
	and	ax,0B8FFh
	or	ax,8000h
	mov	bx,[bp-2h]
	shl	bx,1h
	mov	[bx+24EAh],ax
	mov	ax,[bp-2h]
	jmp	0B2EBh

l0800_B2E7:
	push	ax
	call	8D2Bh

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
	mov	ax,0B4BEh
	push	ax
	push	ds
	mov	ax,236Ch
	push	ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	lea	ax,[bp+8h]
	push	ax
	call	9828h
	pop	bp
	ret

;; fn0800_B30A: 0800:B30A
;;   Called from:
;;     0800:4076 (in fn0800_4047)
;;     0800:B669 (in fn0800_B4BE)
fn0800_B30A proc
	push	bp
	mov	bp,sp
	les	bx,[bp+6h]
	dec	word ptr es:[bx]
	push	word ptr [bp+8h]
	push	bx
	mov	al,[bp+4h]
	cbw
	push	ax
	call	0B324h
	add	sp,6h
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
	mov	al,[bp+4h]
	mov	[4EE6h],al
	les	bx,[bp+6h]
	cmp	word ptr es:[bx],0FFh
	jge	0B389h

l0800_B337:
	inc	word ptr es:[bx]
	mov	ax,es:[bx+0Eh]
	mov	si,es:[bx+0Ch]
	inc	word ptr es:[bx+0Ch]
	mov	dl,[4EE6h]
	mov	es,ax
	mov	es:[si],dl
	mov	es,[bp+8h]
	test	word ptr es:[bx+2h],8h
	jnz	0B35Dh

l0800_B35A:
	jmp	0B4A3h

l0800_B35D:
	cmp	byte ptr [4EE6h],0Ah
	jz	0B36Eh

l0800_B364:
	cmp	byte ptr [4EE6h],0Dh
	jz	0B36Eh

l0800_B36B:
	jmp	0B4A3h

l0800_B36E:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	call	0A6B7h
	pop	cx
	pop	cx
	or	ax,ax
	jnz	0B380h

l0800_B37D:
	jmp	0B4A3h

l0800_B380:
	mov	ax,0FFFFh
	jmp	0B4A8h
0800:B386                   E9 1A 01                            ...       

l0800_B389:
	les	bx,[bp+6h]
	test	word ptr es:[bx+2h],90h
	jnz	0B39Ch

l0800_B394:
	test	word ptr es:[bx+2h],2h
	jnz	0B3A6h

l0800_B39C:
	les	bx,[bp+6h]
	or	word ptr es:[bx+2h],10h
	jmp	0B380h

l0800_B3A6:
	les	bx,[bp+6h]
	or	word ptr es:[bx+2h],100h
	cmp	word ptr es:[bx+6h],0h
	jz	0B421h

l0800_B3B6:
	cmp	word ptr es:[bx],0h
	jz	0B3C9h

l0800_B3BC:
	push	word ptr [bp+8h]
	push	bx
	call	0A6B7h
	pop	cx
	pop	cx
	or	ax,ax
	jnz	0B380h

l0800_B3C9:
	les	bx,[bp+6h]
	mov	ax,es:[bx+6h]
	neg	ax
	mov	es:[bx],ax
	mov	ax,es:[bx+0Eh]
	mov	si,es:[bx+0Ch]
	inc	word ptr es:[bx+0Ch]
	mov	dl,[4EE6h]
	mov	es,ax
	mov	es:[si],dl
	mov	es,[bp+8h]
	test	word ptr es:[bx+2h],8h
	jnz	0B3F8h

l0800_B3F5:
	jmp	0B4A3h

l0800_B3F8:
	cmp	byte ptr [4EE6h],0Ah
	jz	0B409h

l0800_B3FF:
	cmp	byte ptr [4EE6h],0Dh
	jz	0B409h

l0800_B406:
	jmp	0B4A3h

l0800_B409:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	call	0A6B7h
	pop	cx
	pop	cx
	or	ax,ax
	jnz	0B41Bh

l0800_B418:
	jmp	0B4A3h

l0800_B41B:
	jmp	0B380h
0800:B41E                                           E9 82               ..
0800:B420 00                                              .               

l0800_B421:
	les	bx,[bp+6h]
	mov	al,es:[bx+4h]
	cbw
	shl	ax,1h
	mov	bx,ax
	test	word ptr [bx+24EAh],800h
	jz	0B44Ch

l0800_B435:
	mov	ax,2h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	mov	bx,[bp+6h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	8E29h
	add	sp,8h

l0800_B44C:
	cmp	byte ptr [4EE6h],0Ah
	jnz	0B478h

l0800_B453:
	les	bx,[bp+6h]
	test	word ptr es:[bx+2h],40h
	jnz	0B478h

l0800_B45E:
	mov	ax,1h
	push	ax
	push	ds
	mov	ax,26AEh
	push	ax
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	0C779h
	add	sp,8h
	cmp	ax,1h
	jnz	0B495h

l0800_B478:
	mov	ax,1h
	push	ax
	push	ds
	mov	ax,4EE6h
	push	ax
	les	bx,[bp+6h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	0C779h
	add	sp,8h
	cmp	ax,1h
	jz	0B4A3h

l0800_B495:
	les	bx,[bp+6h]
	test	word ptr es:[bx+2h],200h
	jnz	0B4A3h

l0800_B4A0:
	jmp	0B39Ch

l0800_B4A3:
	mov	al,[4EE6h]
	mov	ah,0h

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
	sub	sp,2h
	push	si
	push	di
	mov	di,[bp+8h]
	mov	[bp-2h],di
	les	bx,[bp+4h]
	test	word ptr es:[bx+2h],8h
	jz	0B504h

l0800_B4D7:
	jmp	0B4FAh

l0800_B4D9:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	les	bx,[bp+0Ah]
	inc	word ptr [bp+0Ah]
	mov	al,es:[bx]
	cbw
	push	ax
	call	0B324h
	add	sp,6h
	cmp	ax,0FFFFh
	jnz	0B4FAh

l0800_B4F5:
	xor	ax,ax
	jmp	0B6A0h

l0800_B4FA:
	mov	ax,di
	dec	di
	or	ax,ax
	jnz	0B4D9h

l0800_B501:
	jmp	0B69Dh

l0800_B504:
	les	bx,[bp+4h]
	test	word ptr es:[bx+2h],40h
	jnz	0B512h

l0800_B50F:
	jmp	0B625h

l0800_B512:
	cmp	word ptr es:[bx+6h],0h
	jnz	0B51Ch

l0800_B519:
	jmp	0B5DBh

l0800_B51C:
	cmp	es:[bx+6h],di
	jnc	0B583h

l0800_B522:
	cmp	word ptr es:[bx],0h
	jz	0B535h

l0800_B528:
	push	word ptr [bp+6h]
	push	bx
	call	0A6B7h
	pop	cx
	pop	cx
	or	ax,ax
	jnz	0B4F5h

l0800_B535:
	les	bx,[bp+4h]
	mov	al,es:[bx+4h]
	cbw
	shl	ax,1h
	mov	bx,ax
	test	word ptr [bx+24EAh],800h
	jz	0B560h

l0800_B549:
	mov	ax,2h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	mov	bx,[bp+4h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	8E29h
	add	sp,8h

l0800_B560:
	push	di
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	les	bx,[bp+4h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	0C779h
	add	sp,8h
	cmp	ax,di
	jc	0B57Dh

l0800_B57A:
	jmp	0B69Dh

l0800_B57D:
	jmp	0B4F5h
0800:B580 E9 1A 01                                        ...             

l0800_B583:
	les	bx,[bp+4h]
	mov	ax,es:[bx]
	add	ax,di
	jl	0B5B1h

l0800_B58D:
	cmp	word ptr es:[bx],0h
	jnz	0B59Fh

l0800_B593:
	mov	ax,0FFFFh
	sub	ax,es:[bx+6h]
	mov	es:[bx],ax
	jmp	0B5B1h

l0800_B59F:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0A6B7h
	pop	cx
	pop	cx
	or	ax,ax
	jz	0B5B1h

l0800_B5AE:
	jmp	0B4F5h

l0800_B5B1:
	push	di
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	les	bx,[bp+4h]
	push	word ptr es:[bx+0Eh]
	push	word ptr es:[bx+0Ch]
	call	0B03Bh
	add	sp,0Ah
	les	bx,[bp+4h]
	mov	ax,es:[bx]
	add	ax,di
	mov	es:[bx],ax
	add	es:[bx+0Ch],di
	jmp	0B69Dh

l0800_B5DB:
	les	bx,[bp+4h]
	mov	al,es:[bx+4h]
	cbw
	shl	ax,1h
	mov	bx,ax
	test	word ptr [bx+24EAh],800h
	jz	0B606h

l0800_B5EF:
	mov	ax,2h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	mov	bx,[bp+4h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	8E29h
	add	sp,8h

l0800_B606:
	push	di
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	les	bx,[bp+4h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	0C779h
	add	sp,8h
	cmp	ax,di
	jnc	0B69Dh

l0800_B620:
	jmp	0B4F5h
0800:B623          EB 78                                     .x           

l0800_B625:
	les	bx,[bp+4h]
	cmp	word ptr es:[bx+6h],0h
	jz	0B680h

l0800_B62F:
	jmp	0B677h

l0800_B631:
	les	bx,[bp+4h]
	inc	word ptr es:[bx]
	jge	0B659h

l0800_B639:
	mov	ax,es:[bx+0Eh]
	mov	si,es:[bx+0Ch]
	inc	word ptr es:[bx+0Ch]
	les	bx,[bp+0Ah]
	inc	word ptr [bp+0Ah]
	mov	dl,es:[bx]
	mov	es,ax
	mov	es:[si],dl
	mov	al,dl
	mov	ah,0h
	jmp	0B66Fh

l0800_B659:
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	les	bx,[bp+0Ah]
	inc	word ptr [bp+0Ah]
	mov	al,es:[bx]
	push	ax
	call	0B30Ah
	add	sp,6h

l0800_B66F:
	cmp	ax,0FFFFh
	jnz	0B677h

l0800_B674:
	jmp	0B4F5h

l0800_B677:
	mov	ax,di
	dec	di
	or	ax,ax
	jnz	0B631h

l0800_B67E:
	jmp	0B69Dh

l0800_B680:
	push	di
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	les	bx,[bp+4h]
	mov	al,es:[bx+4h]
	cbw
	push	ax
	call	0C632h
	add	sp,8h
	cmp	ax,di
	jnc	0B69Dh

l0800_B69A:
	jmp	0B4F5h

l0800_B69D:
	mov	ax,[bp-2h]

l0800_B6A0:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	0Ah

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
	mov	cx,[4EE8h]
	les	di,[bp+4h]
	lds	si,[bp+8h]
	shr	cx,1h
	jnc	0B6C6h

l0800_B6BD:
	mov	al,es:[di]
	movsb
	mov	[si-1h],al
	jz	0B6CFh

l0800_B6C6:
	mov	ax,es:[di]
	movsw
	mov	[si-2h],ax
	loop	0B6C6h

l0800_B6CF:
	pop	ds
	pop	di
	pop	si
	pop	bp
	ret	8h

;; fn0800_B6D6: 0800:B6D6
;;   Called from:
;;     0800:B935 (in fn0800_B6D6)
;;     0800:B944 (in fn0800_B6D6)
;;     0800:B97A (in fn0800_B95E)
fn0800_B6D6 proc
	push	bp
	mov	bp,sp
	sub	sp,14h
	push	si
	push	di
	mov	si,[bp+4h]

l0800_B6E1:
	cmp	si,2h
	ja	0B726h

l0800_B6E6:
	cmp	si,2h
	jz	0B6EEh

l0800_B6EB:
	jmp	0B956h

l0800_B6EE:
	mov	ax,[bp+8h]
	mov	dx,[bp+6h]
	add	dx,[4EE8h]
	mov	[bp-6h],ax
	mov	[bp-8h],dx
	push	ax
	push	dx
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	call	word ptr [4EEAh]
	add	sp,8h
	or	ax,ax
	jg	0B714h

l0800_B711:
	jmp	0B956h

l0800_B714:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]

l0800_B720:
	call	0B6A8h
	jmp	0B956h

l0800_B726:
	mov	ax,si
	dec	ax
	imul	word ptr [4EE8h]
	mov	dx,[bp+8h]
	mov	bx,[bp+6h]
	add	bx,ax
	mov	[bp-6h],dx
	mov	[bp-8h],bx
	mov	ax,si
	shr	ax,1h
	imul	word ptr [4EE8h]
	mov	dx,[bp+8h]
	mov	bx,[bp+6h]
	add	bx,ax
	mov	[bp-2h],dx
	mov	[bp-4h],bx
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	push	word ptr [bp-2h]
	push	bx
	call	word ptr [4EEAh]
	add	sp,8h
	or	ax,ax
	jle	0B775h

l0800_B766:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	call	0B6A8h

l0800_B775:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	word ptr [4EEAh]
	add	sp,8h
	or	ax,ax
	jle	0B79Ah

l0800_B78C:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	jmp	0B7BDh

l0800_B79A:
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	call	word ptr [4EEAh]
	add	sp,8h
	or	ax,ax
	jle	0B7C0h

l0800_B7B1:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]

l0800_B7BD:
	call	0B6A8h

l0800_B7C0:
	cmp	si,3h
	jnz	0B7D4h

l0800_B7C5:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	jmp	0B720h

l0800_B7D4:
	mov	ax,[bp+8h]
	mov	dx,[bp+6h]
	add	dx,[4EE8h]
	mov	[bp-0Ah],ax
	mov	[bp-0Ch],dx
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	jmp	0B813h

l0800_B7EC:
	or	di,di
	jnz	0B805h

l0800_B7F0:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp-0Ah]
	push	word ptr [bp-0Ch]
	call	0B6A8h
	mov	ax,[4EE8h]
	add	[bp-0Ch],ax

l0800_B805:
	mov	ax,[bp-4h]
	cmp	ax,[bp-8h]
	jnc	0B883h

l0800_B80D:
	mov	ax,[4EE8h]
	add	[bp-4h],ax

l0800_B813:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	word ptr [4EEAh]
	add	sp,8h
	mov	di,ax
	or	ax,ax
	jle	0B7ECh

l0800_B82C:
	mov	ax,[bp-4h]
	cmp	ax,[bp-8h]
	jnc	0B87Bh

l0800_B834:
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	call	word ptr [4EEAh]
	add	sp,8h
	mov	di,ax
	or	ax,ax
	jge	0B855h

l0800_B84D:
	mov	ax,[4EE8h]
	sub	[bp-8h],ax
	jmp	0B873h

l0800_B855:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [bp-6h]
	push	word ptr [bp-8h]
	call	0B6A8h
	or	di,di
	jz	0B87Bh

l0800_B868:
	mov	ax,[4EE8h]
	add	[bp-4h],ax
	sub	[bp-8h],ax
	jmp	0B87Bh

l0800_B873:
	mov	ax,[bp-4h]
	cmp	ax,[bp-8h]
	jc	0B834h

l0800_B87B:
	mov	ax,[bp-4h]
	cmp	ax,[bp-8h]
	jc	0B813h

l0800_B883:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	word ptr [4EEAh]
	add	sp,8h
	or	ax,ax
	jg	0B8AAh

l0800_B89A:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	add	dx,[4EE8h]
	mov	[bp-2h],ax
	mov	[bp-4h],dx

l0800_B8AA:
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	sub	dx,[4EE8h]
	mov	[bp-12h],ax
	mov	[bp-14h],dx
	mov	ax,[bp+8h]
	mov	dx,[bp+6h]
	mov	[bp-0Eh],ax
	mov	[bp-10h],dx
	jmp	0B8E0h

l0800_B8C8:
	push	word ptr [bp-0Eh]
	push	word ptr [bp-10h]
	push	word ptr [bp-12h]
	push	word ptr [bp-14h]
	call	0B6A8h
	mov	ax,[4EE8h]
	add	[bp-10h],ax
	sub	[bp-14h],ax

l0800_B8E0:
	mov	ax,[bp-10h]
	cmp	ax,[bp-0Ch]
	jnc	0B8F0h

l0800_B8E8:
	mov	ax,[bp-14h]
	cmp	ax,[bp-0Ch]
	jnc	0B8C8h

l0800_B8F0:
	xor	ax,ax
	push	ax
	push	word ptr [4EE8h]
	mov	ax,[bp-4h]
	xor	dx,dx
	sub	ax,[bp-0Ch]
	sbb	dx,0h
	push	dx
	push	ax
	call	8BBBh
	mov	di,ax
	xor	ax,ax
	push	ax
	push	word ptr [4EE8h]
	mov	ax,si
	imul	word ptr [4EE8h]
	mov	dx,[bp+6h]
	add	dx,ax
	xor	ax,ax
	sub	dx,[bp-4h]
	sbb	ax,0h
	push	ax
	push	dx
	call	8BBBh
	mov	si,ax
	cmp	si,di
	jnc	0B93Dh

l0800_B92E:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	ax
	call	0B6D6h
	mov	si,di
	jmp	0B6E1h

l0800_B93D:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	di
	call	0B6D6h
	mov	ax,[bp-2h]
	mov	dx,[bp-4h]
	mov	[bp+8h],ax
	mov	[bp+6h],dx
	jmp	0B6E1h

l0800_B956:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	6h

;; fn0800_B95E: 0800:B95E
;;   Called from:
;;     0800:60EC (in fn0800_5E64)
fn0800_B95E proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+0Ah]
	mov	[4EE8h],ax
	or	ax,ax
	jz	0B97Dh

l0800_B96B:
	mov	ax,[bp+0Ch]
	mov	[4EEAh],ax
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	word ptr [bp+8h]
	call	0B6D6h

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
	sub	sp,4h
	push	si
	push	di
	mov	ax,[bp+4h]
	cmp	ax,[24E8h]
	jc	0B99Ah

l0800_B990:
	mov	ax,6h
	push	ax
	call	8D2Bh
	jmp	0BA44h

l0800_B99A:
	mov	ax,[bp+0Ah]
	inc	ax
	cmp	ax,2h
	jc	0B9B0h

l0800_B9A3:
	mov	bx,[bp+4h]
	shl	bx,1h
	test	word ptr [bx+24EAh],200h
	jz	0B9B5h

l0800_B9B0:
	xor	ax,ax
	jmp	0BA44h

l0800_B9B5:
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8F50h
	add	sp,8h
	mov	[bp-2h],ax
	inc	ax
	cmp	ax,2h
	jc	0B9DDh

l0800_B9D0:
	mov	bx,[bp+4h]
	shl	bx,1h
	test	word ptr [bx+24EAh],4000h
	jnz	0B9E2h

l0800_B9DD:
	mov	ax,[bp-2h]
	jmp	0BA44h

l0800_B9E2:
	mov	cx,[bp-2h]
	les	si,[bp+6h]
	mov	di,si
	mov	bx,si
	cld

l0800_B9ED:
	lodsb	al,es:[si]
	cmp	al,1Ah
	jz	0BA21h

l0800_B9F3:
	cmp	al,0Dh
	jz	0B9FCh

l0800_B9F7:
	stosb
	loop	0B9EDh

l0800_B9FA:
	jmp	0BA19h

l0800_B9FC:
	loop	0B9EDh

l0800_B9FE:
	push	es
	push	bx
	mov	ax,1h
	push	ax
	lea	ax,[bp-3h]
	push	ss
	push	ax
	push	word ptr [bp+4h]
	call	8F50h
	add	sp,8h
	pop	bx
	pop	es
	cld
	mov	al,[bp-3h]
	stosb

l0800_BA19:
	cmp	di,bx
	jnz	0BA1Fh

l0800_BA1D:
	jmp	0B9B5h

l0800_BA1F:
	jmp	0BA41h

l0800_BA21:
	push	bx
	mov	ax,1h
	push	ax
	neg	cx
	sbb	ax,ax
	push	ax
	push	cx
	push	word ptr [bp+4h]
	call	8E29h
	add	sp,8h
	mov	bx,[bp+4h]
	shl	bx,1h
	or	word ptr [bx+24EAh],200h
	pop	bx

l0800_BA41:
	sub	di,bx
	xchg	di,ax

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
	mov	ah,56h
	lds	dx,[bp+4h]
	les	di,[bp+8h]
	int	21h
	pop	ds
	jc	0BA60h

l0800_BA5C:
	xor	ax,ax
	jmp	0BA64h

l0800_BA60:
	push	ax
	call	8D2Bh

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
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0ACB3h
	add	sp,0Ah
	or	ax,ax
	jnz	0BA87h

l0800_BA7F:
	les	bx,[bp+4h]
	and	word ptr es:[bx+2h],0EFh

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
	mov	di,[bp+0Ch]
	mov	si,[bp+0Eh]
	les	bx,[bp+4h]
	mov	ax,es:[bx+12h]
	cmp	ax,[bp+4h]
	jnz	0BAABh

l0800_BAA0:
	cmp	di,2h
	jg	0BAABh

l0800_BAA5:
	cmp	si,7FFFh
	jbe	0BAB1h

l0800_BAAB:
	mov	ax,0FFFFh
	jmp	0BB94h

l0800_BAB1:
	cmp	word ptr [26B2h],0h
	jnz	0BAC7h

l0800_BAB8:
	cmp	word ptr [bp+4h],236Ch
	jnz	0BAC7h

l0800_BABF:
	mov	word ptr [26B2h],1h
	jmp	0BADBh

l0800_BAC7:
	cmp	word ptr [26B0h],0h
	jnz	0BADBh

l0800_BACE:
	cmp	word ptr [bp+4h],2358h
	jnz	0BADBh

l0800_BAD5:
	mov	word ptr [26B0h],1h

l0800_BADB:
	les	bx,[bp+4h]
	cmp	word ptr es:[bx],0h
	jz	0BAF6h

l0800_BAE4:
	mov	ax,1h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+6h]
	push	bx
	call	0ACB3h
	add	sp,0Ah

l0800_BAF6:
	les	bx,[bp+4h]
	test	word ptr es:[bx+2h],4h
	jz	0BB0Eh

l0800_BB01:
	push	word ptr es:[bx+0Ah]
	push	word ptr es:[bx+8h]
	call	9E75h
	pop	cx
	pop	cx

l0800_BB0E:
	les	bx,[bp+4h]
	and	word ptr es:[bx+2h],0F3h
	mov	word ptr es:[bx+6h],0h
	mov	ax,[bp+6h]
	mov	dx,[bp+4h]
	add	dx,5h
	mov	es:[bx+0Ah],ax
	mov	es:[bx+8h],dx
	mov	es:[bx+0Eh],ax
	mov	es:[bx+0Ch],dx
	cmp	di,2h
	jz	0BB92h

l0800_BB3A:
	or	si,si
	jbe	0BB92h

l0800_BB3E:
	mov	word ptr [2352h],0C7F0h
	mov	ax,[bp+8h]
	or	ax,[bp+0Ah]
	jnz	0BB6Bh

l0800_BB4C:
	push	si
	call	9F7Fh
	pop	cx
	mov	[bp+0Ah],dx
	mov	[bp+8h],ax
	or	ax,dx
	jnz	0BB5Eh

l0800_BB5B:
	jmp	0BAABh

l0800_BB5E:
	les	bx,[bp+4h]
	or	word ptr es:[bx+2h],4h
	jmp	0BB6Bh
0800:BB68                         E9 40 FF                        .@.     

l0800_BB6B:
	les	bx,[bp+4h]
	mov	ax,[bp+0Ah]
	mov	dx,[bp+8h]
	mov	es:[bx+0Eh],ax
	mov	es:[bx+0Ch],dx
	mov	es:[bx+0Ah],ax
	mov	es:[bx+8h],dx
	mov	es:[bx+6h],si
	cmp	di,1h
	jnz	0BB92h

l0800_BB8D:
	or	word ptr es:[bx+2h],8h

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
	mov	si,[bp+4h]
	mov	ax,[bp+0Ah]
	or	ax,[bp+0Ch]
	jz	0BBE4h

l0800_BBA7:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	call	0BFC7h
	pop	cx
	pop	cx
	cmp	ax,si
	jc	0BBD2h

l0800_BBB6:
	push	si
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	call	0C01Eh
	add	sp,0Ah
	les	bx,[bp+0Ah]
	mov	byte ptr es:[bx+si],0h
	jmp	0BBE4h

l0800_BBD2:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	call	0BF9Eh
	add	sp,8h

l0800_BBE4:
	pop	si
	pop	bp
	ret	0Ah

;; fn0800_BBE9: 0800:BBE9
fn0800_BBE9 proc
	push	bp
	mov	bp,sp
	sub	sp,2h
	les	bx,[bp+4h]
	cmp	byte ptr es:[bx-1h],2Eh
	jnz	0BBFCh

l0800_BBF9:
	dec	word ptr [bp+4h]

l0800_BBFC:
	dec	word ptr [bp+4h]
	les	bx,[bp+4h]
	mov	al,es:[bx]
	cbw
	mov	[bp-2h],ax
	mov	cx,4h
	mov	bx,0BC39h

l0800_BC0F:
	mov	ax,cs:[bx]
	cmp	ax,[bp-2h]
	jz	0BC1Eh

l0800_BC17:
	add	bx,2h
	loop	0BC0Fh

l0800_BC1C:
	jmp	0BC31h

l0800_BC1E:
	jmp	word ptr cs:[bx+8h]
0800:BC22       C4 5E 04 26 80 7F FE 00 75 05 B8 01 00 EB   .^.&....u.....
0800:BC30 02                                              .               

l0800_BC31:
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret	4h
0800:BC39                            00 00 2F 00 3A 00 5C          ../.:.\
0800:BC40 00 2C BC 2C BC 22 BC 2C BC                      .,.,.".,.       

;; fn0800_BC49: 0800:BC49
;;   Called from:
;;     0800:BE5C (in fn0800_BE3B)
fn0800_BC49 proc
	push	bp
	mov	bp,sp
	sub	sp,58h
	push	si
	push	di
	xor	di,di
	mov	ax,[bp+8h]
	or	ax,[bp+0Ah]
	jz	0BC62h

l0800_BC5B:
	les	bx,[bp+8h]
	mov	byte ptr es:[bx],0h

l0800_BC62:
	mov	ax,[bp+0Ch]
	or	ax,[bp+0Eh]
	jz	0BC71h

l0800_BC6A:
	les	bx,[bp+0Ch]
	mov	byte ptr es:[bx],0h

l0800_BC71:
	mov	ax,[bp+10h]
	or	ax,[bp+12h]
	jz	0BC80h

l0800_BC79:
	les	bx,[bp+10h]
	mov	byte ptr es:[bx],0h

l0800_BC80:
	mov	ax,[bp+14h]
	or	ax,[bp+16h]
	jz	0BC8Fh

l0800_BC88:
	les	bx,[bp+14h]
	mov	byte ptr es:[bx],0h

l0800_BC8F:
	lea	ax,[bp-58h]
	mov	[bp-2h],ss
	mov	[bp-4h],ax
	jmp	0BC9Dh

l0800_BC9A:
	inc	word ptr [bp+4h]

l0800_BC9D:
	les	bx,[bp+4h]
	cmp	byte ptr es:[bx],20h
	jz	0BC9Ah

l0800_BCA6:
	push	word ptr [bp+6h]
	push	bx
	call	0BFC7h
	pop	cx
	pop	cx
	mov	si,ax
	cmp	ax,50h
	jle	0BCB9h

l0800_BCB6:
	mov	si,50h

l0800_BCB9:
	les	bx,[bp-4h]
	mov	byte ptr es:[bx],0h
	inc	word ptr [bp-4h]
	push	si
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0C01Eh
	add	sp,0Ah
	add	[bp-4h],si
	les	bx,[bp-4h]
	mov	byte ptr es:[bx],0h
	xor	si,si

l0800_BCE2:
	dec	word ptr [bp-4h]
	les	bx,[bp-4h]
	mov	al,es:[bx]
	cbw
	mov	[bp-6h],ax
	mov	cx,7h
	mov	bx,0BE1Fh

l0800_BCF5:
	mov	ax,cs:[bx]
	cmp	ax,[bp-6h]
	jz	0BD04h

l0800_BCFD:
	add	bx,2h
	loop	0BCF5h

l0800_BD02:
	jmp	0BCE2h

l0800_BD04:
	jmp	word ptr cs:[bx+0Eh]
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
	push	word ptr [bp+16h]
	push	word ptr [bp+14h]
	push	word ptr [bp+12h]
	push	word ptr [bp+10h]
	push	word ptr [bp+0Eh]
	push	word ptr [bp+0Ch]
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0BC49h
	add	sp,14h
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
	les	bx,[bp+4h]
	mov	byte ptr es:[bx],0h
	mov	ax,0BE64h
	push	ax
	push	ss
	lea	ax,[bp+4h]
	push	ax
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	lea	ax,[bp+0Ch]
	push	ax
	call	9828h
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
	lea	ax,[bp+0Ch]
	push	ax
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	ss
	lea	ax,[bp+4h]
	push	ax
	mov	ax,0BF0Dh
	push	ax
	mov	ax,0BEE5h
	push	ax
	call	8F97h
	add	sp,10h
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
	les	di,[bp+4h]
	mov	dx,di
	xor	al,al
	mov	cx,0FFFFh

l0800_BF70:
	repne scasb

l0800_BF72:
	push	es
	lea	si,[di-1h]
	les	di,[bp+8h]
	mov	cx,0FFFFh

l0800_BF7C:
	repne scasb

l0800_BF7E:
	not	cx
	sub	di,cx
	push	es
	pop	ds
	pop	es
	xchg	di,si
	test	si,1h
	jz	0BF8Fh

l0800_BF8D:
	movsb
	dec	cx

l0800_BF8F:
	shr	cx,1h
	rep movsw
	jnc	0BF96h

l0800_BF95:
	movsb

l0800_BF96:
	xchg	dx,ax
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
	les	di,[bp+8h]
	mov	si,di
	xor	al,al
	mov	cx,0FFFFh

l0800_BFAE:
	repne scasb

l0800_BFB0:
	not	cx
	push	ds
	mov	ax,es
	mov	ds,ax
	les	di,[bp+4h]
	rep movsb
	pop	ds
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
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
	les	di,[bp+4h]
	xor	ax,ax
	cmp	ax,[bp+6h]
	jnz	0BFD9h

l0800_BFD5:
	cmp	ax,di
	jz	0BFE3h

l0800_BFD9:
	cld
	mov	cx,0FFFFh

l0800_BFDD:
	repne scasb

l0800_BFDF:
	xchg	cx,ax
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
	les	di,[bp+8h]
	mov	si,di
	mov	ax,[bp+0Ch]
	mov	cx,ax
	jcxz	0C018h

l0800_BFFA:
	mov	bx,ax
	xor	al,al

l0800_BFFE:
	repne scasb

l0800_C000:
	sub	bx,cx
	mov	cx,bx
	mov	di,si
	lds	si,[bp+4h]
	rep cmpsb
	mov	al,[si-1h]
	mov	bl,es:[di-1h]
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
	les	di,[bp+8h]
	mov	si,di
	xor	al,al
	mov	bx,[bp+0Ch]
	mov	cx,bx

l0800_C030:
	repne scasb

l0800_C032:
	sub	bx,cx
	push	ds
	mov	di,es
	mov	ds,di
	les	di,[bp+4h]
	xchg	bx,cx
	rep movsb
	mov	cx,bx

l0800_C042:
	rep stosb

l0800_C044:
	pop	ds
	mov	dx,[bp+6h]
	mov	ax,[bp+4h]
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
	sub	sp,4h
	push	si
	call	0C379h
	mov	ax,[26E4h]
	mov	dx,[26E2h]
	add	dx,0A600h
	adc	ax,12CEh
	mov	[bp-2h],ax
	mov	[bp-4h],dx
	les	bx,[bp+4h]
	mov	si,es:[bx]
	add	si,0F844h
	mov	ax,si
	sar	ax,1h
	sar	ax,1h
	cwd
	push	ax
	push	dx
	mov	dx,786h
	mov	ax,1F80h
	pop	cx
	pop	bx
	call	8F18h
	add	[bp-4h],ax
	adc	[bp-2h],dx
	mov	ax,si
	and	ax,3h
	cwd
	push	ax
	push	dx
	mov	dx,1E1h
	mov	ax,3380h
	pop	cx
	pop	bx
	call	8F18h
	add	[bp-4h],ax
	adc	[bp-2h],dx
	test	si,3h
	jz	0C0B9h

l0800_C0B0:
	add	word ptr [bp-4h],5180h
	adc	word ptr [bp-2h],1h

l0800_C0B9:
	xor	cx,cx
	les	bx,[bp+4h]
	mov	al,es:[bx+3h]
	cbw
	dec	ax
	mov	si,ax
	jmp	0C0D0h

l0800_C0C8:
	dec	si
	mov	al,[si+26B4h]
	cbw
	add	cx,ax

l0800_C0D0:
	or	si,si
	jg	0C0C8h

l0800_C0D4:
	les	bx,[bp+4h]
	mov	al,es:[bx+2h]
	cbw
	dec	ax
	add	cx,ax
	cmp	byte ptr es:[bx+3h],2h
	jle	0C0EEh

l0800_C0E6:
	test	word ptr es:[bx],3h
	jnz	0C0EEh

l0800_C0ED:
	inc	cx

l0800_C0EE:
	les	bx,[bp+8h]
	mov	al,es:[bx+1h]
	mov	ah,0h
	push	ax
	mov	ax,cx
	mov	dx,18h
	imul	dx
	pop	dx
	add	ax,dx
	mov	si,ax
	cmp	word ptr [26E6h],0h
	jz	0C128h

l0800_C10B:
	mov	al,es:[bx+1h]
	mov	ah,0h
	push	ax
	push	cx
	xor	ax,ax
	push	ax
	les	bx,[bp+4h]
	mov	ax,es:[bx]
	add	ax,0F84Eh
	push	ax
	call	0C553h
	or	ax,ax
	jz	0C128h

l0800_C127:
	dec	si

l0800_C128:
	mov	ax,si
	cwd
	push	ax
	push	dx
	xor	dx,dx
	mov	ax,0E10h
	pop	cx
	pop	bx
	call	8F18h
	add	[bp-4h],ax
	adc	[bp-2h],dx
	les	bx,[bp+8h]
	mov	al,es:[bx]
	mov	ah,0h
	cwd
	push	ax
	push	dx
	xor	dx,dx
	mov	ax,3Ch
	pop	cx
	pop	bx
	call	8F18h
	les	bx,[bp+8h]
	mov	bl,es:[bx+3h]
	mov	bh,0h
	push	ax
	mov	ax,bx
	push	dx
	cwd
	pop	bx
	pop	cx
	add	cx,ax
	adc	bx,dx
	add	[bp-4h],cx
	adc	[bp-2h],bx
	mov	dx,[bp-2h]
	mov	ax,[bp-4h]
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_C177: 0800:C177
fn0800_C177 proc
	push	bp
	mov	bp,sp
	call	0C379h
	mov	ax,[26E4h]
	mov	dx,[26E2h]
	add	dx,0A600h
	adc	ax,12CEh
	sub	[bp+4h],dx
	sbb	[bp+6h],ax
	les	bx,[bp+0Ch]
	mov	byte ptr es:[bx+2h],0h
	xor	ax,ax
	mov	dx,3Ch
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BCAh
	les	bx,[bp+0Ch]
	mov	es:[bx+3h],al
	xor	ax,ax
	mov	dx,3Ch
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BBBh
	mov	[bp+6h],dx
	mov	[bp+4h],ax
	xor	ax,ax
	mov	dx,3Ch
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BCAh
	les	bx,[bp+0Ch]
	mov	es:[bx],al
	xor	ax,ax
	mov	dx,3Ch
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BBBh
	mov	[bp+6h],dx
	mov	[bp+4h],ax
	xor	ax,ax
	mov	dx,88F8h
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BBBh
	shl	ax,1h
	shl	ax,1h
	add	ax,7BCh
	les	bx,[bp+8h]
	mov	es:[bx],ax
	xor	ax,ax
	mov	dx,88F8h
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BCAh
	mov	[bp+6h],dx
	mov	[bp+4h],ax
	cmp	word ptr [bp+6h],0h
	jl	0C26Fh

l0800_C22B:
	jnz	0C234h

l0800_C22D:
	cmp	word ptr [bp+4h],2250h
	jc	0C26Fh

l0800_C234:
	sub	word ptr [bp+4h],2250h
	sbb	word ptr [bp+6h],0h
	les	bx,[bp+8h]
	inc	word ptr es:[bx]
	xor	ax,ax
	mov	dx,2238h
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BBBh
	les	bx,[bp+8h]
	add	es:[bx],ax
	xor	ax,ax
	mov	dx,2238h
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BCAh
	mov	[bp+6h],dx
	mov	[bp+4h],ax

l0800_C26F:
	cmp	word ptr [26E6h],0h
	jz	0C2B4h

l0800_C276:
	xor	ax,ax
	mov	dx,18h
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BCAh
	push	ax
	xor	ax,ax
	mov	dx,18h
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BBBh
	push	ax
	xor	ax,ax
	push	ax
	les	bx,[bp+8h]
	mov	ax,es:[bx]
	add	ax,0F84Eh
	push	ax
	call	0C553h
	or	ax,ax
	jz	0C2B4h

l0800_C2AC:
	add	word ptr [bp+4h],1h
	adc	word ptr [bp+6h],0h

l0800_C2B4:
	xor	ax,ax
	mov	dx,18h
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BCAh
	les	bx,[bp+0Ch]
	mov	es:[bx+1h],al
	xor	ax,ax
	mov	dx,18h
	push	ax
	push	dx
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	8BBBh
	mov	[bp+6h],dx
	mov	[bp+4h],ax
	add	word ptr [bp+4h],1h
	adc	word ptr [bp+6h],0h
	les	bx,[bp+8h]
	test	word ptr es:[bx],3h
	jnz	0C326h

l0800_C2F3:
	cmp	word ptr [bp+6h],0h
	jl	0C30Bh

l0800_C2F9:
	jg	0C301h

l0800_C2FB:
	cmp	word ptr [bp+4h],3Ch
	jbe	0C30Bh

l0800_C301:
	sub	word ptr [bp+4h],1h
	sbb	word ptr [bp+6h],0h
	jmp	0C326h

l0800_C30B:
	cmp	word ptr [bp+6h],0h
	jnz	0C326h

l0800_C311:
	cmp	word ptr [bp+4h],3Ch
	jnz	0C326h

l0800_C317:
	les	bx,[bp+8h]
	mov	byte ptr es:[bx+3h],2h
	mov	byte ptr es:[bx+2h],1Dh
	jmp	0C377h

l0800_C326:
	les	bx,[bp+8h]
	mov	byte ptr es:[bx+3h],0h
	jmp	0C34Dh

l0800_C330:
	les	bx,[bp+8h]
	mov	al,es:[bx+3h]
	cbw
	mov	bx,ax
	mov	al,[bx+26B4h]
	cbw
	cwd
	sub	[bp+4h],ax
	sbb	[bp+6h],dx
	mov	bx,[bp+8h]
	inc	byte ptr es:[bx+3h]

l0800_C34D:
	les	bx,[bp+8h]
	mov	al,es:[bx+3h]
	cbw
	mov	bx,ax
	mov	al,[bx+26B4h]
	cbw
	cwd
	cmp	dx,[bp+6h]
	jl	0C330h

l0800_C362:
	jnz	0C369h

l0800_C364:
	cmp	ax,[bp+4h]
	jc	0C330h

l0800_C369:
	les	bx,[bp+8h]
	inc	byte ptr es:[bx+3h]
	mov	al,[bp+4h]
	mov	es:[bx+2h],al

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
	sub	sp,4h
	push	si
	push	ds
	mov	ax,26E8h
	push	ax
	call	0AFCBh
	pop	cx
	pop	cx
	mov	[bp-2h],dx
	mov	[bp-4h],ax
	or	ax,dx
	jnz	0C397h

l0800_C394:
	jmp	0C42Ch

l0800_C397:
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	call	0BFC7h
	pop	cx
	pop	cx
	cmp	ax,4h
	jnc	0C3AAh

l0800_C3A7:
	jmp	0C42Ch

l0800_C3AA:
	les	bx,[bp-4h]
	mov	al,es:[bx]
	cbw
	mov	bx,ax
	mov	al,[bx+2251h]
	cbw
	test	ax,0Ch
	jz	0C42Ch

l0800_C3BD:
	mov	bx,[bp-4h]
	mov	al,es:[bx+1h]
	cbw
	mov	bx,ax
	mov	al,[bx+2251h]
	cbw
	test	ax,0Ch
	jz	0C42Ch

l0800_C3D1:
	mov	bx,[bp-4h]
	mov	al,es:[bx+2h]
	cbw
	mov	bx,ax
	mov	al,[bx+2251h]
	cbw
	test	ax,0Ch
	jz	0C42Ch

l0800_C3E5:
	mov	bx,[bp-4h]
	cmp	byte ptr es:[bx+3h],2Dh
	jz	0C404h

l0800_C3EF:
	cmp	byte ptr es:[bx+3h],2Bh
	jz	0C404h

l0800_C3F6:
	mov	al,es:[bx+3h]
	cbw
	mov	bx,ax
	test	byte ptr [bx+2251h],2h
	jz	0C42Ch

l0800_C404:
	les	bx,[bp-4h]
	mov	al,es:[bx+3h]
	cbw
	mov	bx,ax
	mov	al,[bx+2251h]
	cbw
	test	ax,2h
	jnz	0C467h

l0800_C418:
	mov	bx,[bp-4h]
	mov	al,es:[bx+4h]
	cbw
	mov	bx,ax
	mov	al,[bx+2251h]
	cbw
	test	ax,2h
	jnz	0C467h

l0800_C42C:
	mov	word ptr [26E6h],1h
	mov	word ptr [26E4h],0h
	mov	word ptr [26E2h],4650h
	push	ds
	mov	ax,26EBh
	push	ax
	push	word ptr [26DCh]
	push	word ptr [26DAh]
	call	0BF9Eh
	add	sp,8h
	push	ds
	mov	ax,26EFh
	push	ax
	push	word ptr [26E0h]
	push	word ptr [26DEh]
	call	0BF9Eh
	add	sp,8h
	jmp	0C54Eh

l0800_C467:
	mov	ax,4h
	push	ax
	xor	ax,ax
	push	ax
	push	word ptr [26E0h]
	push	word ptr [26DEh]
	call	0B083h
	add	sp,8h
	mov	ax,3h
	push	ax
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	push	word ptr [26DCh]
	push	word ptr [26DAh]
	call	0C01Eh
	add	sp,0Ah
	les	bx,[26DAh]
	mov	byte ptr es:[bx+3h],0h
	mov	ax,[bp-4h]
	add	ax,3h
	push	word ptr [bp-2h]
	push	ax
	call	0A471h
	pop	cx
	pop	cx
	push	ax
	push	dx
	xor	dx,dx
	mov	ax,0E10h
	pop	cx
	pop	bx
	call	8F18h
	mov	[26E4h],dx
	mov	[26E2h],ax
	mov	word ptr [26E6h],0h
	mov	si,3h
	jmp	0C542h

l0800_C4CA:
	les	bx,[bp-4h]
	mov	al,es:[bx+si]
	cbw
	mov	bx,ax
	test	byte ptr [bx+2251h],0Ch
	jz	0C541h

l0800_C4DA:
	mov	ax,[bp-4h]
	add	ax,si
	push	word ptr [bp-2h]
	push	ax
	call	0BFC7h
	pop	cx
	pop	cx
	cmp	ax,3h
	jc	0C54Eh

l0800_C4ED:
	les	bx,[bp-4h]
	mov	al,es:[bx+si+1h]
	cbw
	mov	bx,ax
	mov	al,[bx+2251h]
	cbw
	test	ax,0Ch
	jz	0C54Eh

l0800_C501:
	mov	bx,[bp-4h]
	mov	al,es:[bx+si+2h]
	cbw
	mov	bx,ax
	mov	al,[bx+2251h]
	cbw
	test	ax,0Ch
	jz	0C54Eh

l0800_C515:
	mov	ax,3h
	push	ax
	mov	ax,[bp-4h]
	add	ax,si
	push	word ptr [bp-2h]
	push	ax
	push	word ptr [26E0h]
	push	word ptr [26DEh]
	call	0C01Eh
	add	sp,0Ah
	les	bx,[26DEh]
	mov	byte ptr es:[bx+3h],0h
	mov	word ptr [26E6h],1h
	jmp	0C54Eh

l0800_C541:
	inc	si

l0800_C542:
	les	bx,[bp-4h]
	cmp	byte ptr es:[bx+si],0h
	jz	0C54Eh

l0800_C54B:
	jmp	0C4CAh

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
	cmp	word ptr [bp+6h],0h
	jnz	0C589h

l0800_C55D:
	mov	si,[bp+8h]
	cmp	word ptr [bp+8h],3Bh
	jc	0C572h

l0800_C566:
	mov	ax,[bp+4h]
	add	ax,46h
	test	ax,3h
	jnz	0C572h

l0800_C571:
	dec	si

l0800_C572:
	mov	word ptr [bp+6h],0h
	jmp	0C57Ch

l0800_C579:
	inc	word ptr [bp+6h]

l0800_C57C:
	mov	bx,[bp+6h]
	shl	bx,1h
	cmp	[bx+26C0h],si
	jbe	0C579h

l0800_C587:
	jmp	0C5AAh

l0800_C589:
	cmp	word ptr [bp+6h],3h
	jc	0C59Ah

l0800_C58F:
	mov	ax,[bp+4h]
	add	ax,46h
	test	ax,3h
	jz	0C59Dh

l0800_C59A:
	dec	word ptr [bp+8h]

l0800_C59D:
	mov	bx,[bp+6h]
	dec	bx
	shl	bx,1h
	mov	ax,[bx+26C0h]
	add	[bp+8h],ax

l0800_C5AA:
	cmp	word ptr [bp+6h],4h
	jc	0C62Bh

l0800_C5B0:
	jz	0C5BAh

l0800_C5B2:
	cmp	word ptr [bp+6h],0Ah
	ja	0C62Bh

l0800_C5B8:
	jnz	0C626h

l0800_C5BA:
	mov	bx,[bp+6h]
	shl	bx,1h
	cmp	word ptr [bp+4h],10h
	jle	0C5D4h

l0800_C5C5:
	cmp	word ptr [bp+6h],4h
	jnz	0C5D4h

l0800_C5CB:
	mov	cx,[bx+26BEh]
	add	cx,7h
	jmp	0C5D8h

l0800_C5D4:
	mov	cx,[bx+26C0h]

l0800_C5D8:
	mov	bx,[bp+4h]
	add	bx,7B2h
	test	bl,3h
	jz	0C5E5h

l0800_C5E4:
	dec	cx

l0800_C5E5:
	mov	bx,[bp+4h]
	inc	bx
	sar	bx,1h
	sar	bx,1h
	add	bx,cx
	mov	ax,16Dh
	mul	word ptr [bp+4h]
	add	ax,bx
	add	ax,4h
	xor	dx,dx
	mov	bx,7h
	div	bx
	sub	cx,dx
	mov	ax,[bp+8h]
	cmp	word ptr [bp+6h],4h
	jnz	0C61Ah

l0800_C60C:
	cmp	ax,cx
	ja	0C626h

l0800_C610:
	jnz	0C62Bh

l0800_C612:
	cmp	byte ptr [bp+0Ah],2h
	jc	0C62Bh

l0800_C618:
	jmp	0C626h

l0800_C61A:
	cmp	ax,cx
	jc	0C626h

l0800_C61E:
	jnz	0C62Bh

l0800_C620:
	cmp	byte ptr [bp+0Ah],1h
	ja	0C62Bh

l0800_C626:
	mov	ax,1h
	jmp	0C62Dh

l0800_C62B:
	xor	ax,ax

l0800_C62D:
	pop	si
	pop	bp
	ret	8h

;; fn0800_C632: 0800:C632
;;   Called from:
;;     0800:A75B (in fn0800_A6B7)
;;     0800:B690 (in fn0800_B4BE)
fn0800_C632 proc
	push	bp
	mov	bp,sp
	sub	sp,8Eh
	push	si
	push	di
	mov	di,[bp+4h]
	cmp	di,[24E8h]
	jc	0C64Eh

l0800_C644:
	mov	ax,6h
	push	ax
	call	8D2Bh
	jmp	0C773h

l0800_C64E:
	mov	ax,[bp+0Ah]
	inc	ax
	cmp	ax,2h
	jnc	0C65Ch

l0800_C657:
	xor	ax,ax
	jmp	0C773h

l0800_C65C:
	mov	bx,di
	shl	bx,1h
	test	word ptr [bx+24EAh],800h
	jz	0C677h

l0800_C668:
	mov	ax,2h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	di
	call	8E29h
	add	sp,8h

l0800_C677:
	mov	bx,di
	shl	bx,1h
	test	word ptr [bx+24EAh],4000h
	jnz	0C696h

l0800_C683:
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	di
	call	0C779h
	add	sp,8h
	jmp	0C773h

l0800_C696:
	mov	bx,di
	shl	bx,1h
	and	word ptr [bx+24EAh],0FDFFh
	mov	ax,[bp+8h]
	mov	dx,[bp+6h]
	mov	[bp-0Ah],ax
	mov	[bp-0Ch],dx
	mov	ax,[bp+0Ah]
	mov	[bp-6h],ax
	jmp	0C727h

l0800_C6B4:
	dec	word ptr [bp-6h]
	les	bx,[bp-0Ch]
	inc	word ptr [bp-0Ch]
	mov	al,es:[bx]
	mov	[bp-7h],al
	cmp	al,0Ah
	jnz	0C6D1h

l0800_C6C7:
	les	bx,[bp-4h]
	mov	byte ptr es:[bx],0Dh
	inc	word ptr [bp-4h]

l0800_C6D1:
	les	bx,[bp-4h]
	mov	al,[bp-7h]
	mov	es:[bx],al
	inc	word ptr [bp-4h]
	lea	ax,[bp+0FF72h]
	mov	dx,[bp-4h]
	xor	bx,bx
	sub	dx,ax
	sbb	bx,0h
	or	bx,bx
	jl	0C731h

l0800_C6EF:
	jnz	0C6F7h

l0800_C6F1:
	cmp	dx,80h
	jc	0C731h

l0800_C6F7:
	lea	ax,[bp+0FF72h]
	mov	si,[bp-4h]
	xor	dx,dx
	sub	si,ax
	sbb	dx,0h
	push	si
	push	ss
	push	ax
	push	di
	call	0C779h
	add	sp,8h
	mov	dx,ax
	cmp	ax,si
	jz	0C727h

l0800_C715:
	cmp	dx,0FFh
	jnz	0C71Fh

l0800_C71A:
	mov	ax,0FFFFh
	jmp	0C76Eh

l0800_C71F:
	mov	ax,[bp+0Ah]
	sub	ax,[bp-6h]
	jmp	0C76Ah

l0800_C727:
	lea	ax,[bp+0FF72h]
	mov	[bp-2h],ss
	mov	[bp-4h],ax

l0800_C731:
	cmp	word ptr [bp-6h],0h
	jz	0C73Ah

l0800_C737:
	jmp	0C6B4h

l0800_C73A:
	lea	ax,[bp+0FF72h]
	mov	si,[bp-4h]
	xor	dx,dx
	sub	si,ax
	sbb	dx,0h
	mov	ax,si
	or	ax,ax
	jbe	0C770h

l0800_C74E:
	push	si
	push	ss
	lea	ax,[bp+0FF72h]
	push	ax
	push	di
	call	0C779h
	add	sp,8h
	mov	dx,ax
	cmp	ax,si
	jz	0C770h

l0800_C762:
	cmp	dx,0FFh
	jz	0C71Ah

l0800_C767:
	mov	ax,[bp+0Ah]

l0800_C76A:
	add	ax,dx
	sub	ax,si

l0800_C76E:
	jmp	0C773h

l0800_C770:
	mov	ax,[bp+0Ah]

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
	mov	bx,[bp+4h]
	shl	bx,1h
	test	word ptr [bx+24EAh],1h
	jz	0C78Fh

l0800_C789:
	mov	ax,5h
	push	ax
	jmp	0C7B0h

l0800_C78F:
	push	ds
	mov	ah,40h
	mov	bx,[bp+4h]
	mov	cx,[bp+0Ah]
	lds	dx,[bp+6h]
	int	21h
	pop	ds
	jc	0C7AFh

l0800_C7A0:
	push	ax
	mov	bx,[bp+4h]
	shl	bx,1h
	or	word ptr [bx+24EAh],1000h
	pop	ax
	jmp	0C7B3h

l0800_C7AF:
	push	ax

l0800_C7B0:
	call	8D2Bh

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
