;;; Segment Image base (0800:0000)

fn0800_0000()
	pushf	
	jmp	0004

l0800_0004:
	jmp	0007

l0800_0007:
	jmp	000A

l0800_000A:
	pop	bx
	mov	ax,0FFF
	and	ax,bx
	push	ax
	jmp	0014

l0800_0014:
	jmp	0017

l0800_0017:
	jmp	001A

l0800_001A:
	popf	
	jmp	001E

l0800_001E:
	jmp	0021

l0800_0021:
	jmp	0024

l0800_0024:
	pushf	
	jmp	0028

l0800_0028:
	jmp	002B

l0800_002B:
	jmp	002E

l0800_002E:
	pop	ax
	and	eax,0000F000
	cmp	ax,F000
	jz	0043

l0800_003B:
	or	bx,F000
	push	bx
	jmp	0043

l0800_0043:
	jmp	0046

l0800_0046:
	jmp	0049

l0800_0049:
	popf	
	jmp	004D

l0800_004D:
	jmp	0050

l0800_0050:
	jmp	0053

l0800_0053:
	pushf	
	jmp	0057

l0800_0057:
	jmp	005A

l0800_005A:
	jmp	005D

l0800_005D:
	pop	ax
	and	ax,F000
	jnz	00AE

l0800_0064:
	call	word ptr [00000094]
	push	sp
	push	7369
	and	[bx+si+72],dh
	outsw	
	jc	00D7

l0800_0076:
	insw	
	and	[bp+si+65],dh
	jno	00F1

l0800_007C:
	imul	si,[bp+si+65],2073
	popa	
	and	[bp+di],dh
	cmp	[6F20],dh
	jc	00AA

l0800_008A:
	bound	sp,[di+74]
	jz	00F4

l0800_008F:
	jc	00BF

l0800_0091:
	or	ax,000A
	pop	si
	mov	ax,cs
	mov	ds,ax
	cld	

l0800_009A:
	lodsb	
	or	al,al
	jz	00A9

l0800_00A0:
	mov	ah,02
	mov	dl,al
	int	21
	jmp	009A

l0800_00A9:
	mov	ax,4C02

l0800_00AA:
	add	cl,[si-33]

l0800_00AC:
	int	21

l0800_00AD:
	and	[si+8CDA],cx

l0800_00AE:
	mov	dx,ds
	mov	ax,cs

l0800_00B1:
	enter	8EFD,D8
	add	ax,00FF
	and	ax,FF00
	mov	es,ax
	mov	ecx,00009138

l0800_00BF:
	cmp	[bx+di+0000],dl

l0800_00C3:
	sub	ecx,000000E0
	mov	esi,00009137
	mov	di,si

l0800_00D2:
	rep	
	movsb	

l0800_00D4:
	push	000000E0

l0800_00D7:
	add	[bx+si],al
	add	[bx-77],ah

l0800_00DA:
	mov	[esp+02],ax

l0800_00DC:
	inc	sp
	and	al,02
	retf	
0800:00E0 8E C0 8E D0 66 BC F0 A8 00 00 66 BF 38 91 00 00 ....f.....f.8...
0800:00F0 66                                              f              

l0800_00F1:
	mov	cx,D6A0

l0800_00F4:
	add	[bx+si],al
	sub	cx,di
	xor	al,al
	cld	

l0800_00FB:
	rep	
	stosb	

l0800_00FD:
	mov	ds,dx
	mov	ds,[0000002C]
	xor	si,si

l0800_0108:
	lodsb	
	or	al,al
	jnz	0108

l0800_010E:
	lodsb	
	or	al,al
	jnz	0108

l0800_0114:
	sub	sp,si
	and	esp,0000FFFC
	xor	edi,edi
	mov	di,sp
	xor	esi,esi
	push	esi
	jmp	0134

l0800_012A:
	push	edi
	stosb	

l0800_012D:
	lodsb	
	stosb	
	or	al,al
	jnz	012D

l0800_0134:
	lodsb	
	or	al,al
	jnz	012A

l0800_013A:
	mov	cs:[0000A8F0],esp
	lodsw	
	mov	bx,si

l0800_0146:
	push	00
	lodsb	
	or	al,al
	jz	0154

l0800_014E:
	lodsb	
	or	al,al
	jnz	0146

l0800_0154:
	mov	si,bx
	mov	di,sp

l0800_0158:
	lodsb	
	stosb	
	or	al,al
	jnz	0158

l0800_015F:
	mov	ebp,esp
	mov	ds,dx
	xor	ecx,ecx
	xor	ebx,ebx
	mov	cl,[00000080]
	inc	cx
	mov	si,0080
	add	si,cx
	mov	di,sp
	dec	di
	sub	sp,cx
	and	sp,FFFC
	push	ebx
	std	

l0800_0183:
	xor	al,al
	stosb	
	inc	ebx

l0800_0188:
	cmp	si,0080
	jz	01AB

l0800_018F:
	lodsb	
	cmp	al,20
	jbe	0188

l0800_0195:
	stosb	
	cmp	si,0080
	jz	01A3

l0800_019D:
	lodsb	
	cmp	al,20
	ja	0195

l0800_01A3:
	mov	cx,di
	inc	cx
	push	ecx
	jmp	0183

l0800_01AB:
	push	ebp
	mov	eax,esp
	push	dword ptr cs:[0000A8F0]
	push	eax
	push	ebx
	mov	ebx,0000D6AF
	shr	bx,04
	mov	ax,cs
	add	bx,ax
	sub	bx,dx
	mov	es,dx
	mov	ah,4A
	int	21
	mov	ax,cs
	mov	ds,ax
	mov	es,ax
	cld	
	call	2D98
	add	[di-77],dl
	in	ax,E8
	cbw	
	add	[bx+si],ax
	add	al,ch
	jg	0250

l0800_01EB:
	add	[bx+si],al
	leave	
	ret	
0800:01EF                                              00                .
0800:01F0 55 89 E5 FF 75 10 FF 75 0C 6A 01 FF 75 08 68 08 U...u..u.j..u.h.
0800:0200 91 00 00 E8 38 79 00 00 C9 C3 8D 36 55 89 E5 FF ....8y.....6U...
0800:0210 75 10 FF 75 0C 6A 02 FF 75 08 68 08 91 00 00 E8 u..u.j..u.h.....
0800:0220 1C 79 00 00 C9 C3 00 00 55 89 E5 FF 75 08 E8 2D .y......U...u..-
0800:0230 00 00 00 C9 C3 8D 76 00 55 89 E5 53 31 DB 8D 36 ......v.U..S1..6

l0800_0240:
	lea	ax,[si]
	illegal	
	loopne	024E
	add	ax,A8F4
	add	[bx+si],al
	push	ax
	call	0226
	illegal	

l0800_0250:
	inc	word ptr [bp+di+04C4]
	inc	bx
	test	bx,bx
	jle	0240

l0800_0259:
	mov	bx,[di-04]
	leave	
	ret	
0800:025E                                           00 00               ..
0800:0260 55 89 E5 53 8B 5D 08 53 E8 C3 3B 00 00 53 E8 5D U..S.].S..;..S.]
0800:0270 25 00 00 53 E8 DB 3D 00 00 53 E8 D1 6A 00 00 8B %..S..=..S..j...
0800:0280 5D FC C9 C3 55 89 E5 83 EC 08 8B 55 08 0F B7 4D ]...U......U...M
0800:0290 F8 89 4D F8 81 4D F8 00 00 67 00 A1 9C B2 00 00 ..M..M...g......
0800:02A0 05 00 F8 FF FF 89 D1 29 C1 89 4D FC 0F 01 55 FA .......)..M...U.
0800:02B0 EA B7 02 00 00 08 00 B9 10 00 00 00 66 8E D9 66 ............f..f
0800:02C0 8E C1 31 C9 66 8E E1 66 8E E9 B9 10 00 00 00 66 ..1.f..f.......f
0800:02D0 8E D1 66 C7 45 FA FF 07 89 D1 2B 0D 9C B2 00 00 ..f.E.....+.....
0800:02E0 89 4D FC 0F 01 5D FA B9 38 00 00 00 66 0F 00 D1 .M...]..8...f...
0800:02F0 80 A2 45 08 00 00 FD B9 40 00 00 00 66 0F 00 D9 ..E.....@...f...
0800:0300 C9 C3 00 00 55 89 E5 53 83 7D 08 00 0F 95 C0 0F ....U..S.}......
0800:0310 B6 D8 53 E8 C8 FE FF FF 83 C4 04 83 7D 08 00 74 ..S.........}..t
0800:0320 1B 8D 45 0C 50 FF 75 08 E8 33 75 00 00 6A 0A E8 ..E.P.u..3u..j..
0800:0330 28 54 00 00 53 E8 32 74 00 00 8D 36 53 E8 2A 74 (T..S.2t...6S.*t
0800:0340 00 00 8D 36 55 89 E5 68 90 86 00 00 E8 B3 FF FF ...6U..h........
0800:0350 FF C9 C3 00 55 89 E5 C7 05 EC B3 00 00 00 0F 00 ....U...........
0800:0360 00 68 D0 B3 00 00 6A 10 A1 90 90 00 00 FF D0 8A .h....j.........
0800:0370 05 EC B3 00 00 88 05 40 91 00 00 C9 C3 8D 76 00 .......@......v.
0800:0380 55 89 E5 80 3D 40 91 00 00 00 74 41 C7 05 EC B3 U...=@....tA....
0800:0390 00 00 00 0F 00 00 68 D0 B3 00 00 6A 10 A1 90 90 ......h....j....
0800:03A0 00 00 FF D0 0F B6 05 EC B3 00 00 0F B6 15 40 91 ..............@.
0800:03B0 00 00 83 C4 08 39 D0 74 14 89 15 EC B3 00 00 68 .....9.t.......h
0800:03C0 D0 B3 00 00 6A 10 A1 90 90 00 00 FF D0 C9 C3 90 ....j...........
0800:03D0 55 89 E5 53 8B 5D 0C 8B 55 08 80 CE 35 89 15 EC U..S.]..U...5...
0800:03E0 B3 00 00 68 D0 B3 00 00 6A 21 A1 90 90 00 00 FF ...h....j!......
0800:03F0 D0 66 A1 F2 B3 00 00 66 89 43 02 66 A1 E0 B3 00 .f.....f.C.f....
0800:0400 00 66 89 03 8B 5D FC C9 C3 8D 76 00 55 89 E5 8B .f...]....v.U...
0800:0410 45 0C 8B 4D 08 80 CD 25 89 0D EC B3 00 00 66 8B E..M...%......f.
0800:0420 50 02 66 89 15 F4 B3 00 00 0F B7 00 A3 E4 B3 00 P.f.............
0800:0430 00 68 D0 B3 00 00 6A 21 A1 90 90 00 00 FF D0 C9 .h....j!........
0800:0440 C3 00 00 00 55 89 E5 83 EC 34 57 53 8B 55 08 8B ....U....4WS.U..
0800:0450 1D 04 B3 00 00 2B 1D 9C B2 00 00 66 C7 45 EC 00 .....+.....f.E..
0800:0460 00 66 C7 45 FC 00 00 66 C7 45 FA 00 00 30 C0 89 .f.E...f.E...0..
0800:0470 D7 FC B9 FF FF FF FF F2 AE 89 C8 F7 D0 3D 00 10 .............=..
0800:0480 00 00 76 0C B8 03 C0 00 00 E9 91 00 00 00 8D 36 ..v............6
0800:0490 52 FF 35 04 B3 00 00 E8 C0 74 00 00 8B 15 04 B3 R.5......t......
0800:04A0 00 00 83 C4 08 80 3A 00 74 4C 80 7A 01 3A 75 46 ......:.tL.z.:uF
0800:04B0 8A 02 04 BF 3C 19 77 10 C7 45 E8 00 0E 00 00 0F ....<.w..E......
0800:04C0 BE 02 83 C0 BF EB 1C 90 8B 15 04 B3 00 00 8A 02 ................
0800:04D0 04 9F 3C 19 77 20 C7 45 E8 00 0E 00 00 0F BE 02 ..<.w .E........
0800:04E0 83 C0 9F 89 45 E0 8D 45 CC 50 6A 21 A1 90 90 00 ....E..E.Pj!....
0800:04F0 00 FF D0 83 C4 08 C7 45 E8 00 3B 00 00 89 D8 C1 .......E..;.....
0800:0500 E8 04 66 89 45 F0 83 E3 0F 89 5D E0 8D 5D CC 53 ..f.E.....]..].S
0800:0510 6A 21 A1 90 90 00 00 FF D0 53 E8 09 00 00 00 8D j!.......S......
0800:0520 65 C4 5B 5F C9 C3 00 00 55 89 E5 8B 45 08 F6 40 e.[_....U...E..@
0800:0530 20 01 75 04 31 C0 C9 C3 0F B7 40 1C 05 00 40 41  .u.1.....@...@A
0800:0540 03 C9 C3 00 55 89 E5 83 EC 34 53 8B 45 08 66 C7 ....U....4S.E.f.
0800:0550 45 EC 00 00 66 C7 45 FC 00 00 66 C7 45 FA 00 00 E...f.E...f.E...
0800:0560 C7 45 E8 00 3E 00 00 89 45 DC 8D 5D CC 53 6A 21 .E..>...E..].Sj!
0800:0570 A1 90 90 00 00 FF D0 53 E8 AB FF FF FF 8B 5D C8 .......S......].
0800:0580 C9 C3 00 00 55 89 E5 83 EC 3C 57 56 53 8B 7D 08 ....U....<WVS.}.
0800:0590 8B 75 0C 66 C7 45 EC 00 00 66 C7 45 FC 00 00 66 .u.f.E...f.E...f
0800:05A0 C7 45 FA 00 00 6A 30 56 E8 4B 71 00 00 66 C7 46 .E...j0V.Kq..f.F
0800:05B0 0A 01 00 66 C7 46 08 FF 01 C7 45 E8 00 44 00 00 ...f.F....E..D..
0800:05C0 89 7D DC 8D 5D CC 53 6A 21 A1 90 90 00 00 FF D0 .}..].Sj!.......
0800:05D0 53 E8 52 FF FF FF 83 C4 14 85 C0 75 7A 80 7D E0 S.R........uz.}.
0800:05E0 00 7D 09 66 81 4E 08 00 20 EB 07 90 66 81 4E 08 .}.f.N.. ...f.N.
0800:05F0 00 80 8D 45 C8 50 6A 01 6A 00 57 E8 0C 06 00 00 ...E.Pj.j.W.....
0800:0600 83 C4 10 85 C0 75 39 8D 46 24 50 6A 02 6A 00 57 .....u9.F$Pj.j.W
0800:0610 E8 F7 05 00 00 83 C4 10 85 C0 75 24 8D 45 C4 50 ..........u$.E.P
0800:0620 6A 00 FF 75 C8 57 E8 E1 05 00 00 85 C0 75 28 8B j..u.W.......u(.
0800:0630 45 C4 39 45 C8 74 09 B8 0F C0 00 00 EB 19 8D 36 E.9E.t.........6
0800:0640 8B 46 24 05 FF 01 00 00 C1 E8 09 89 46 28 C7 46 .F$.........F(.F
0800:0650 2C 00 02 00 00 31 C0 8D 65 B8 5B 5E 5F C9 C3 00 ,....1..e.[^_...
0800:0660 55 89 E5 83 EC 4C 57 56 53 8B 75 08 66 C7 45 EC U....LWVS.u.f.E.
0800:0670 00 00 66 C7 45 FC 00 00 66 C7 45 FA 00 00 85 F6 ..f.E...f.E.....
0800:0680 0F 84 22 01 00 00 C7 45 E8 00 2A 00 00 8D 5D CC .."....E..*...].
0800:0690 53 6A 21 A1 90 90 00 00 FF D0 0F B7 4D E4 89 4D Sj!.........M..M
0800:06A0 C8 0F B6 4D E1 89 4D C4 0F B6 7D E0 C7 45 E8 00 ...M..M...}..E..
0800:06B0 2C 00 00 53 6A 21 A1 90 90 00 00 FF D0 53 E8 65 ,..Sj!.......S.e
0800:06C0 FE FF FF 85 C0 0F 85 EE 00 00 00 0F B6 5D E5 0F .............]..
0800:06D0 B6 4D E4 89 4D C0 0F B6 4D E1 89 4D BC 0F B6 4D .M..M...M..M...M
0800:06E0 E0 89 4D B8 8B 45 C8 05 4E F8 FF FF 89 C2 C1 E2 ..M..E..N.......
0800:06F0 09 01 C2 8D 14 90 8D 14 D0 89 D1 C1 E1 04 29 D1 ..............).
0800:0700 C1 E1 07 89 0E 85 C0 7D 08 8B 45 C8 05 51 F8 FF .......}..E..Q..
0800:0710 FF 89 C2 C1 FA 02 8D 04 92 8D 04 82 8D 04 C2 C1 ................
0800:0720 E0 02 29 D0 C1 E0 07 01 06 8B 4D C4 8B 14 8D 78 ..).......M....x
0800:0730 8D 00 00 8D 04 92 8D 04 82 8D 04 C2 C1 E0 02 29 ...............)
0800:0740 D0 C1 E0 07 01 06 8B 45 C8 05 4E F8 FF FF A8 03 .......E..N.....
0800:0750 75 0B 83 F9 02 7E 06 81 06 80 51 01 00 8D 04 BF u....~....Q.....
0800:0760 8D 04 87 8D 04 C7 C1 E0 02 29 F8 C1 E0 07 01 06 .........)......
0800:0770 8D 04 5B 8D 04 43 C1 E0 05 01 D8 C1 E0 04 01 06 ..[..C..........
0800:0780 8B 4D C0 8D 04 49 8D 04 80 C1 E0 02 01 06 8B 4D .M...I.........M
0800:0790 BC 01 0E 8B 4D B8 8D 04 89 8D 04 80 8D 04 80 8D ....M...........
0800:07A0 04 80 C1 E0 04 89 46 04 83 7D 0C 00 75 06 31 C0 ......F..}..u.1.
0800:07B0 EB 07 8D 36 B8 0E C0 00 00 8D 65 A8 5B 5E 5F C9 ...6......e.[^_.
0800:07C0 C3 00 00 00 55 89 E5 83 EC 34 57 53 8B 55 08 8B ....U....4WS.U..
0800:07D0 1D 04 B3 00 00 2B 1D 9C B2 00 00 66 C7 45 EC 00 .....+.....f.E..
0800:07E0 00 66 C7 45 FC 00 00 66 C7 45 FA 00 00 30 C0 89 .f.E...f.E...0..
0800:07F0 D7 FC B9 FF FF FF FF F2 AE 89 C8 F7 D0 3D 00 10 .............=..
0800:0800 00 00 77 38 52 FF 35 04 B3 00 00 E8 4C 71 00 00 ..w8R.5.....Lq..
0800:0810 C7 45 E8 00 39 00 00 89 D8 C1 E8 04 66 89 45 F0 .E..9.......f.E.
0800:0820 83 E3 0F 89 5D E0 8D 5D CC 53 6A 21 A1 90 90 00 ....]..].Sj!....
0800:0830 00 FF D0 53 E8 EF FC FF FF EB 06 90 B8 03 C0 00 ...S............
0800:0840 00 8D 65 C4 5B 5F C9 C3 55 89 E5 83 EC 38 57 56 ..e.[_..U....8WV
0800:0850 53 BB 0E C0 00 00 8B 35 04 B3 00 00 2B 35 9C B2 S......5....+5..
0800:0860 00 00 66 C7 45 EC 00 00 66 C7 45 FC 00 00 66 C7 ..f.E...f.E...f.
0800:0870 45 FA 00 00 30 C0 8B 7D 08 FC B9 FF FF FF FF F2 E...0..}........
0800:0880 AE 89 C8 F7 D0 3D 00 10 00 00 76 0C B8 03 C0 00 .....=....v.....
0800:0890 00 E9 60 01 00 00 8D 36 FF 75 08 FF 35 04 B3 00 ..`....6.u..5...
0800:08A0 00 E8 B6 70 00 00 89 F0 C1 E8 04 66 89 45 F0 83 ...p.......f.E..
0800:08B0 E6 0F 89 75 E0 8B 45 0C 25 B0 00 00 00 83 C4 08 ...u..E.%.......
0800:08C0 83 F8 30 74 3A 8B 45 0C 83 E0 03 0D 40 3D 00 00 ..0t:.E.....@=..
0800:08D0 89 45 E8 C7 45 E4 00 00 00 00 8D 5D CC 53 6A 21 .E..E......].Sj!
0800:08E0 A1 90 90 00 00 FF D0 53 E8 3B FC FF FF 89 C3 83 .......S.;......
0800:08F0 C4 0C 85 DB 75 09 0F B7 4D E8 8B 55 14 89 0A 8B ....u...M..U....
0800:0900 45 0C 25 B0 00 00 00 83 F8 30 74 79 7F 16 83 F8 E.%......0ty....
0800:0910 10 74 41 7F 07 85 C0 74 2F EB 6A 90 83 F8 20 74 .tA....t/.j... t
0800:0920 53 EB 62 90 3D 90 00 00 00 74 31 7F 0B 3D 80 00 S.b.=....t1..=..
0800:0930 00 00 74 14 EB 4F 8D 36 3D A0 00 00 00 74 35 3D ..t..O.6=....t5=
0800:0940 B0 00 00 00 74 16 EB 3D 85 DB 74 74 89 D8 E9 A3 ....t..=..tt....
0800:0950 00 00 00 90 85 DB 74 68 EB 2B 8D 36 85 DB 75 25 ......th.+.6..u%
0800:0960 0F B7 45 E8 50 E8 DA FB FF FF B8 0A C0 00 00 E9 ..E.P...........
0800:0970 82 00 00 00 85 DB 75 D4 0F B7 45 E8 50 E8 C2 FB ......u...E.P...
0800:0980 FF FF 83 C4 04 C7 45 E8 00 3C 00 00 8B 45 10 C1 ......E..<...E..
0800:0990 E8 08 34 01 83 E0 01 89 45 E4 8D 5D CC 53 6A 21 ..4.....E..].Sj!
0800:09A0 A1 90 90 00 00 FF D0 53 E8 7B FB FF FF 89 C3 85 .......S.{......
0800:09B0 DB 75 99 0F B7 4D E8 8B 55 14 89 0A EB 36 8D 36 .u...M..U....6.6
0800:09C0 8B 55 0C F6 C2 40 74 2C 8D 45 C8 50 6A 02 6A 00 .U...@t,.E.Pj.j.
0800:09D0 8B 4D 14 FF 31 E8 32 02 00 00 89 C3 83 C4 10 85 .M..1.2.........
0800:09E0 DB 74 11 8B 55 14 FF 32 E8 57 FB FF FF E9 5A FF .t..U..2.W....Z.
0800:09F0 FF FF 8D 36 31 C0 8D 65 BC 5B 5E 5F C9 C3 00 00 ...61..e.[^_....
0800:0A00 55 89 E5 83 EC 40 57 56 53 8B 75 10 C7 45 C8 00 U....@WVS.u..E..
0800:0A10 00 00 00 8B 15 04 B3 00 00 2B 15 9C B2 00 00 89 .........+......
0800:0A20 55 C4 66 C7 45 EC 00 00 66 C7 45 FC 00 00 66 C7 U.f.E...f.E...f.
0800:0A30 45 FA 00 00 85 F6 74 7B 8D 4D CC 89 4D C0 8D 36 E.....t{.M..M..6
0800:0A40 89 F7 81 FE 00 10 00 00 7E 05 BF 00 10 00 00 C7 ........~.......
0800:0A50 45 E8 00 3F 00 00 8B 55 08 89 55 DC 89 7D E4 8B E..?...U..U..}..
0800:0A60 45 C4 C1 E8 04 66 89 45 F0 8B 4D C4 83 E1 0F 89 E....f.E..M.....
0800:0A70 4D E0 FF 75 C0 6A 21 A1 90 90 00 00 FF D0 FF 75 M..u.j!........u
0800:0A80 C0 E8 A2 FA FF FF 83 C4 0C 85 C0 75 30 0F B7 5D ...........u0..]
0800:0A90 E8 53 FF 35 04 B3 00 00 FF 75 0C E8 1C 6C 00 00 .S.5.....u...l..
0800:0AA0 83 C4 0C 01 5D 0C 29 DE 01 5D C8 39 FB 7C 04 85 ....].)..].9.|..
0800:0AB0 F6 75 8D 8B 4D C8 8B 55 14 89 0A 31 C0 8D 65 B4 .u..M..U...1..e.
0800:0AC0 5B 5E 5F C9 C3 00 00 00 55 89 E5 83 EC 3C 57 56 [^_.....U....<WV
0800:0AD0 53 8B 5D 08 8B 75 0C A1 04 B3 00 00 2B 05 9C B2 S.]..u......+...
0800:0AE0 00 00 89 45 C8 66 C7 45 EC 00 00 66 C7 45 FC 00 ...E.f.E...f.E..
0800:0AF0 00 66 C7 45 FA 00 00 30 D2 89 DF 88 D0 FC B9 FF .f.E...0........
0800:0B00 FF FF FF F2 AE F7 D1 89 4D C4 81 F9 00 08 00 00 ........M.......
0800:0B10 77 17 89 F7 FC B9 FF FF FF FF F2 AE F7 D1 89 4D w..............M
0800:0B20 C4 81 F9 00 08 00 00 76 17 C7 05 1C D6 00 00 03 .......v........
0800:0B30 C0 00 00 B8 FF FF FF FF E9 C7 00 00 00 8D 76 00 ..............v.
0800:0B40 53 FF 35 04 B3 00 00 E8 10 6E 00 00 56 A1 04 B3 S.5......n..V...
0800:0B50 00 00 05 00 08 00 00 50 E8 FF 6D 00 00 C7 45 E8 .......P..m...E.
0800:0B60 00 56 00 00 8B 7D C8 C1 EF 04 66 89 7D F0 8B 45 .V...}....f.}..E
0800:0B70 C8 83 E0 0F 89 45 E0 66 89 7D EE 8D B0 00 08 00 .....E.f.}......
0800:0B80 00 89 75 CC 8D 5D CC 53 6A 21 A1 90 90 00 00 FF ..u..].Sj!......
0800:0B90 D0 53 E8 91 F9 FF FF 89 C2 83 C4 1C 85 D2 74 62 .S............tb
0800:0BA0 C7 45 E8 00 41 00 00 66 89 7D F0 89 75 E0 53 6A .E..A..f.}..u.Sj
0800:0BB0 21 A1 90 90 00 00 FF D0 53 E8 6A F9 FF FF 89 C2 !.......S.j.....
0800:0BC0 83 C4 0C 85 D2 75 3D C7 45 E8 00 56 00 00 8B 55 .....u=.E..V...U
0800:0BD0 C8 C1 EA 04 66 89 55 F0 8B 45 C8 83 E0 0F 89 45 ....f.U..E.....E
0800:0BE0 E0 66 89 55 EE 05 00 08 00 00 89 45 CC 8D 5D CC .f.U.......E..].
0800:0BF0 53 6A 21 A1 90 90 00 00 FF D0 53 E8 28 F9 FF FF Sj!.......S.(...
0800:0C00 89 C2 89 D0 8D 65 B8 5B 5E 5F C9 C3 55 89 E5 83 .....e.[^_..U...
0800:0C10 EC 34 56 53 8B 55 08 8B 45 0C 8B 75 14 66 C7 45 .4VS.U..E..u.f.E
0800:0C20 EC 00 00 66 C7 45 FC 00 00 66 C7 45 FA 00 00 8B ...f.E...f.E....
0800:0C30 4D 10 80 CD 42 89 4D E8 89 55 DC 89 C1 C1 E9 10 M...B.M..U......
0800:0C40 89 4D E4 25 FF FF 00 00 89 45 E0 8D 5D CC 53 6A .M.%.....E..].Sj
0800:0C50 21 A1 90 90 00 00 FF D0 53 E8 CA F8 FF FF 85 C0 !.......S.......
0800:0C60 75 10 8B 55 E0 C1 E2 10 0F B7 45 E8 09 C2 89 16 u..U......E.....
0800:0C70 31 C0 8D 65 C4 5B 5E C9 C3 00 00 00 55 89 E5 83 1..e.[^.....U...
0800:0C80 EC 34 53 8B 5D 08 66 C7 45 EC 00 00 66 C7 45 FC .4S.].f.E...f.E.
0800:0C90 00 00 66 C7 45 FA 00 00 6A 2C FF 75 0C E8 56 6A ..f.E...j,.u..Vj
0800:0CA0 00 00 C7 45 E8 00 44 00 00 89 5D DC 8D 5D CC 53 ...E..D...]..].S
0800:0CB0 6A 21 A1 90 90 00 00 FF D0 53 E8 69 F8 FF FF 85 j!.......S.i....
0800:0CC0 C0 75 12 80 7D E0 00 7D 07 31 C0 EB 08 8D 76 00 .u..}..}.1....v.
0800:0CD0 B8 1E C0 00 00 8B 5D C8 C9 C3 00 00 55 89 E5 83 ......].....U...
0800:0CE0 EC 34 57 53 8B 55 08 8B 1D 04 B3 00 00 2B 1D 9C .4WS.U.......+..
0800:0CF0 B2 00 00 66 C7 45 EC 00 00 66 C7 45 FC 00 00 66 ...f.E...f.E...f
0800:0D00 C7 45 FA 00 00 30 C0 89 D7 FC B9 FF FF FF FF F2 .E...0..........
0800:0D10 AE 89 C8 F7 D0 3D 00 10 00 00 77 38 52 FF 35 04 .....=....w8R.5.
0800:0D20 B3 00 00 E8 34 6C 00 00 C7 45 E8 00 41 00 00 89 ....4l...E..A...
0800:0D30 D8 C1 E8 04 66 89 45 F0 83 E3 0F 89 5D E0 8D 5D ....f.E.....]..]
0800:0D40 CC 53 6A 21 A1 90 90 00 00 FF D0 53 E8 D7 F7 FF .Sj!.......S....
0800:0D50 FF EB 06 90 B8 03 C0 00 00 8D 65 C4 5B 5F C9 C3 ..........e.[_..
0800:0D60 55 89 E5 83 EC 40 57 56 53 8B 7D 0C 8B 75 10 C7 U....@WVS.}..u..
0800:0D70 45 C8 00 00 00 00 8B 15 04 B3 00 00 2B 15 9C B2 E...........+...
0800:0D80 00 00 89 55 C4 66 C7 45 EC 00 00 66 C7 45 FC 00 ...U.f.E...f.E..
0800:0D90 00 66 C7 45 FA 00 00 85 F6 74 79 8D 4D CC 89 4D .f.E.....ty.M..M
0800:0DA0 C0 8D 76 00 89 F3 81 FE 00 10 00 00 7E 05 BB 00 ..v.........~...
0800:0DB0 10 00 00 53 57 FF 35 04 B3 00 00 E8 FC 68 00 00 ...SW.5......h..
0800:0DC0 83 C4 0C C7 45 E8 00 40 00 00 8B 55 08 89 55 DC ....E..@...U..U.
0800:0DD0 89 5D E4 8B 45 C4 C1 E8 04 66 89 45 F0 8B 4D C4 .]..E....f.E..M.
0800:0DE0 83 E1 0F 89 4D E0 FF 75 C0 6A 21 A1 90 90 00 00 ....M..u.j!.....
0800:0DF0 FF D0 FF 75 C0 E8 2E F7 FF FF 83 C4 0C 85 C0 75 ...u...........u
0800:0E00 1D 0F B7 45 E8 01 C7 29 C6 01 45 C8 39 D8 7C 04 ...E...)..E.9.|.
0800:0E10 85 F6 75 90 8B 4D C8 8B 55 14 89 0A 31 C0 8D 65 ..u..M..U...1..e
0800:0E20 B4 5B 5E 5F C9 C3 00 00 55 89 E5 83 EC 18 57 56 .[^_....U.....WV
0800:0E30 53 83 7D 08 00 75 08 A1 54 91 00 00 89 45 08 8B S.}..u..T....E..
0800:0E40 7D 08 03 3D 1C B3 00 00 2B 3D 9C B2 00 00 B8 04 }..=....+=......
0800:0E50 05 00 00 89 FB 8B 4D 0C 31 D2 CD 31 72 03 66 31 ......M.1..1r.f1
0800:0E60 C0 89 C7 89 5D FC 89 75 F8 66 85 FF 74 48 8B 55 ....]..u.f..tH.U
0800:0E70 0C C1 EA 10 B8 01 05 00 00 89 D3 8B 4D 0C CD 31 ............M..1
0800:0E80 72 0F C1 E3 10 66 89 CB C1 E6 10 66 89 FE 66 31 r....f.....f..f1
0800:0E90 C0 89 C2 89 5D FC 89 75 F8 66 89 55 F4 66 85 D2 ....]..u.f.U.f..
0800:0EA0 0F 85 88 00 00 00 A1 1C B3 00 00 2B 05 9C B2 00 ...........+....
0800:0EB0 00 39 45 FC 72 B8 8B 7D FC 03 3D 9C B2 00 00 A1 .9E.r..}..=.....
0800:0EC0 1C B3 00 00 29 C7 89 7D EC 83 7D 10 00 74 28 8B ....)..}..}..t(.
0800:0ED0 4D FC 89 CB C1 EB 10 8B 75 0C C1 EE 10 B8 00 06 M.......u.......
0800:0EE0 00 00 8B 7D 0C CD 31 72 03 66 31 C0 89 C2 66 89 ...}..1r.f1...f.
0800:0EF0 55 F4 66 85 D2 75 37 8B 45 0C 03 45 EC 89 45 F0 U.f..u7.E..E..E.
0800:0F00 39 05 4C 91 00 00 73 38 66 8B 1D F4 B1 00 00 89 9.L...s8f.......
0800:0F10 C2 4A 89 D1 C1 E9 10 B8 08 00 00 00 CD 31 72 03 .J...........1r.
0800:0F20 66 31 C0 89 C2 66 89 55 F4 66 85 D2 74 06 0F B7 f1...f.U.f..t...
0800:0F30 C2 EB 1F 90 8B 55 0C 03 55 EC 89 15 4C 91 00 00 .....U..U...L...
0800:0F40 8B 5D EC 8B 4D 14 89 19 8B 75 F8 8B 7D 18 89 37 .]..M....u..}..7
0800:0F50 31 C0 8D 65 DC 5B 5E 5F C9 C3 8D 36 55 89 E5 83 1..e.[^_...6U...
0800:0F60 EC 30 57 56 53 C7 45 E4 00 00 00 00 C7 05 4C 91 .0WVS.E.......L.
0800:0F70 00 00 00 00 00 00 C7 45 D4 00 00 00 00 BF 20 B3 .......E...... .
0800:0F80 00 00 8D 36 8B 45 D4 8D 04 80 8D 14 85 00 00 00 ...6.E..........
0800:0F90 00 83 7C 17 08 00 74 35 8B 8A 20 B3 00 00 03 4C ..|...t5.. ....L
0800:0FA0 17 08 89 4D D0 8B 5D E4 39 D9 76 03 89 4D E4 8B ...M..].9.v..M..
0800:0FB0 75 D0 39 35 4C 91 00 00 73 13 8B 45 D4 8D 14 80 u.95L...s..E....
0800:0FC0 F6 44 97 10 04 74 06 89 35 4C 91 00 00 FF 45 D4 .D...t..5L....E.
0800:0FD0 83 7D D4 01 7E AE 81 45 E4 FF 0F 00 00 81 65 E4 .}..~..E......e.
0800:0FE0 00 F0 FF FF 8B 55 E4 C1 EA 10 B8 01 05 00 00 89 .....U..........
0800:0FF0 D3 8B 4D E4 CD 31 72 0F C1 E3 10 66 89 CB C1 E6 ..M..1r....f....
0800:1000 10 66 89 FE 66 31 C0 66 89 45 DC 89 5D FC 89 75 .f..f1.f.E..]..u
0800:1010 F8 66 8B 55 DC 66 85 C0 74 11 0F B7 C2 50 68 A4 .f.U.f..t....Ph.
0800:1020 86 00 00 E8 DC F2 FF FF 83 C4 08 A1 9C B2 00 00 ................
0800:1030 03 45 FC A3 1C B3 00 00 8B 4D E4 89 0D 54 91 00 .E.......M...T..
0800:1040 00 89 0D 50 91 00 00 31 C0 B9 01 00 00 00 CD 31 ...P...1.......1
0800:1050 72 09 66 A3 88 B2 00 00 66 31 C0 89 C7 89 FA 66 r.f.....f1.....f
0800:1060 85 FF 74 11 0F B7 C2 50 68 A4 86 00 00 E8 92 F2 ..t....Ph.......
0800:1070 FF FF 83 C4 08 66 8B 1D 44 91 00 00 8D 7D F0 B8 .....f..D....}..
0800:1080 0B 00 00 00 CD 31 72 03 66 31 C0 89 C1 89 CA 66 .....1r.f1.....f
0800:1090 85 C9 74 11 0F B7 C2 50 68 A4 86 00 00 E8 62 F2 ..t....Ph.....b.
0800:10A0 FF FF 83 C4 08 8B 45 FC 66 89 45 F2 89 C2 C1 EA ......E.f.E.....
0800:10B0 10 88 55 F4 C1 E8 18 88 45 F7 8B 15 4C 91 00 00 ..U.....E...L...
0800:10C0 4A 81 FA FF FF 0F 00 76 17 C1 EA 0C 8A 45 F6 C0 J......v.....E..
0800:10D0 E8 04 25 FF 00 00 00 0C 08 C0 E0 04 EB 07 8D 36 ..%............6
0800:10E0 8A 45 F6 24 70 80 65 F6 0F 08 45 F6 66 89 55 F0 .E.$p.e...E.f.U.
0800:10F0 C1 EA 10 89 55 D8 8A 4D D8 80 E1 0F 80 65 F6 F0 ....U..M.....e..
0800:1100 08 4D F6 66 8B 15 88 B2 00 00 8D 7D F0 B8 0C 00 .M.f.......}....
0800:1110 00 00 89 D3 CD 31 72 03 66 31 C0 89 C6 89 F2 66 .....1r.f1.....f
0800:1120 85 F6 74 11 0F B7 C2 50 68 A4 86 00 00 E8 D2 F1 ..t....Ph.......
0800:1130 FF FF 83 C4 08 66 8B 1D 88 B2 00 00 B8 0A 00 00 .....f..........
0800:1140 00 CD 31 72 09 66 A3 F4 B1 00 00 66 31 C0 89 C1 ..1r.f.....f1...
0800:1150 89 CA 66 85 C9 74 11 0F B7 C2 50 68 A4 86 00 00 ..f..t....Ph....
0800:1160 E8 9F F1 FF FF 83 C4 08 8D 45 E8 50 8D 45 EC 50 .........E.P.E.P
0800:1170 6A 00 68 00 00 10 00 8B 45 E4 03 05 48 91 00 00 j.h.....E...H...
0800:1180 50 E8 A2 FC FF FF 83 C4 14 89 C2 66 85 D2 74 11 P..........f..t.
0800:1190 0F B7 C2 50 68 A4 86 00 00 E8 66 F1 FF FF 83 C4 ...Ph.....f.....
0800:11A0 08 8B 4D EC 81 C1 00 00 10 00 89 4D E0 03 0D 48 ..M........M...H
0800:11B0 91 00 00 89 0D 54 91 00 00 89 0D 50 91 00 00 C7 .....T.....P....
0800:11C0 05 AD 8D 00 00 30 20 00 00 66 8B 1D 44 91 00 00 .....0 ..f..D...
0800:11D0 66 89 1D B1 8D 00 00 BE AC 8D 00 00 2B 35 9C B2 f...........+5..
0800:11E0 00 00 89 75 D8 66 8B 45 D8 66 89 45 F2 8B 55 D8 ...u.f.E.f.E..U.
0800:11F0 C1 EA 10 88 55 F4 C1 EE 18 89 75 D8 8A 4D D8 88 ....U.....u..M..
0800:1200 4D F7 8A 55 F6 80 E2 70 80 65 F6 0F 08 55 F6 66 M..U...p.e...U.f
0800:1210 C7 45 F0 06 00 BB 06 00 00 00 C1 EB 10 89 5D D8 .E............].
0800:1220 8A 45 D8 24 0F 80 65 F6 F0 08 45 F6 BA 07 00 00 .E.$..e...E.....
0800:1230 00 8D 7D F0 B8 0C 00 00 00 89 D3 CD 31 72 03 66 ..}.........1r.f
0800:1240 31 C0 89 C7 89 FA 66 85 FF 74 11 0F B7 C2 50 68 1.....f..t....Ph
0800:1250 A4 86 00 00 E8 AB F0 FF FF 83 C4 08 C7 45 D4 00 .............E..
0800:1260 00 00 00 BF 20 B3 00 00 8B 75 D4 8D 34 B6 8D 14 .... ....u..4...
0800:1270 B5 00 00 00 00 83 7C 17 0C 00 74 1D FF 74 17 0C ......|...t..t..
0800:1280 8B 82 20 B3 00 00 03 05 1C B3 00 00 50 FF 74 17 .. .........P.t.
0800:1290 04 E8 72 43 00 00 83 C4 0C 8B 45 D4 8D 14 80 C1 ..rC......E.....
0800:12A0 E2 02 8B 4C 17 08 39 4C 17 0C 73 22 8B 44 17 08 ...L..9L..s".D..
0800:12B0 2B 44 17 0C 50 8B 82 20 B3 00 00 03 44 17 0C 03 +D..P.. ....D...
0800:12C0 05 1C B3 00 00 50 E8 2D 64 00 00 83 C4 08 FF 45 .....P.-d......E
0800:12D0 D4 83 7D D4 01 7E 91 66 8B 1D F4 B1 00 00 66 89 ..}..~.f......f.
0800:12E0 1D 04 B2 00 00 FF 75 E0 E8 1F 44 00 00 89 C2 89 ......u...D.....
0800:12F0 15 00 B2 00 00 66 8B 35 88 B2 00 00 A1 0C B3 00 .....f.5........
0800:1300 00 66 8B 0D F4 B1 00 00 0F B2 25 00 B2 00 00 56 .f........%....V
0800:1310 50 66 8E D9 66 8E C1 CB EB FE 8D 36 55 89 E5 57 Pf..f......6U..W
0800:1320 53 8B 7D 0C 66 0F B6 5D 08 B8 00 03 00 00 31 C9 S.}.f..]......1.
0800:1330 CD 31 72 03 66 31 C0 89 C3 89 DA 66 85 DB 74 0E .1r.f1.....f..t.
0800:1340 0F B7 C2 50 68 A4 86 00 00 E8 B6 EF FF FF 8D 65 ...Ph..........e
0800:1350 F8 5B 5F C9 C3 8D 76 00 55 89 E5 0F B6 45 08 80 .[_...v.U....E..
0800:1360 CC 4C CD 21 C9 C3 8D 36 55 89 E5 83 EC 08 57 56 .L.!...6U.....WV
0800:1370 53 8B 7D 08 85 FF 7D 0C B8 0E C0 00 00 E9 A8 00 S.}...}.........
0800:1380 00 00 8D 36 89 F8 03 05 50 91 00 00 39 05 54 91 ...6....P...9.T.
0800:1390 00 00 72 10 8B 1D 50 91 00 00 A3 50 91 00 00 EB ..r...P....P....
0800:13A0 71 8D 76 00 8D B7 FF 0F 00 00 81 E6 00 F0 FF FF q.v.............
0800:13B0 8D 45 F8 50 8D 45 FC 50 FF 35 58 91 00 00 56 FF .E.P.E.P.5X...V.
0800:13C0 35 54 91 00 00 E8 5E FA FF FF 83 C4 14 66 85 C0 5T....^......f..
0800:13D0 74 0A 25 FF FF 00 00 EB 51 8D 76 00 8B 45 FC 39 t.%.....Q.v..E.9
0800:13E0 05 54 91 00 00 75 15 8B 1D 50 91 00 00 01 35 54 .T...u...P....5T
0800:13F0 91 00 00 01 3D 50 91 00 00 EB 17 90 8B 5D FC 01 ....=P.......]..
0800:1400 DE 89 35 54 91 00 00 8B 55 FC 01 FA 89 15 50 91 ..5T....U.....P.
0800:1410 00 00 57 6A 00 89 D8 03 05 1C B3 00 00 50 E8 A9 ..Wj.........P..
0800:1420 62 00 00 8B 55 0C 89 1A 31 C0 8D 65 EC 5B 5E 5F b...U...1..e.[^_
0800:1430 C9 C3 8D 36 55 89 E5 83 EC 24 57 56 53 A1 48 91 ...6U....$WVS.H.
0800:1440 00 00 F7 D8 23 45 08 89 45 EC 8B 55 08 29 C2 89 ....#E..E..U.)..
0800:1450 D7 03 7D 0C 03 3D 48 91 00 00 89 7D E0 FF 4D E0 ..}..=H....}..M.
0800:1460 8B 45 E0 31 D2 F7 35 48 91 00 00 89 45 E0 89 C7 .E.1..5H....E...
0800:1470 0F AF 3D 48 91 00 00 83 3D 4C 91 00 00 FF 0F 84 ..=H....=L......
0800:1480 CA 00 00 00 8D 4D F8 51 8D 5D FC 53 6A 00 57 8B .....M.Q.].Sj.W.
0800:1490 75 10 FF 36 E8 8F F9 FF FF 66 89 45 E4 66 85 C0 u..6.....f.E.f..
0800:14A0 74 0A 25 FF FF 00 00 E9 C4 00 00 00 8B 7D F8 B8 t.%..........}..
0800:14B0 09 05 00 00 89 FE 31 DB 8B 4D E0 8B 55 EC CD 31 ......1..M..U..1
0800:14C0 72 03 66 31 C0 66 89 45 E4 66 85 C0 0F 84 8E 00 r.f1.f.E.f......
0800:14D0 00 00 C1 EE 10 B8 02 05 00 00 CD 31 72 03 66 31 ...........1r.f1
0800:14E0 C0 66 8B 1D F4 B1 00 00 B8 08 00 00 00 B9 FF FF .f..............
0800:14F0 00 00 BA FF FF FF FF CD 31 72 03 66 31 C0 89 C3 ........1r.f1...
0800:1500 66 85 DB 75 38 66 8B 1D F4 B1 00 00 8D 7D F0 B8 f..u8f.......}..
0800:1510 0B 00 00 00 CD 31 72 03 66 31 C0 89 C1 66 85 C9 .....1r.f1...f..
0800:1520 75 1B 8A 45 F6 C0 E8 04 A8 08 74 11 66 81 7D F0 u..E......t.f.}.
0800:1530 FF FF 75 09 8A 45 F6 24 0F 3C 0F 74 07 0F B7 45 ..u..E.$.<.t...E
0800:1540 E4 EB 2D 90 C7 05 4C 91 00 00 FF FF FF FF 8B 45 ..-...L........E
0800:1550 EC 03 05 9C B2 00 00 2B 05 1C B3 00 00 89 45 FC .......+......E.
0800:1560 8B 55 08 2B 55 EC 03 55 FC 8B 4D 10 89 11 31 C0 .U.+U..U..M...1.
0800:1570 8D 65 D0 5B 5E 5F C9 C3 55 89 E5 83 EC 04 57 56 .e.[^_..U.....WV
0800:1580 53 8B 7D 0C 8B 45 08 03 05 1C B3 00 00 89 C1 2B S.}..E.........+
0800:1590 0D 9C B2 00 00 89 CB C1 EB 10 89 FE C1 EE 10 B8 ................
0800:15A0 00 06 00 00 CD 31 72 03 66 31 C0 89 C1 89 CA 66 .....1r.f1.....f
0800:15B0 85 C9 74 0E 0F B7 C2 50 68 A4 86 00 00 E8 42 ED ..t....Ph.....B.
0800:15C0 FF FF 8D 65 F0 5B 5E 5F C9 C3 8D 36 55 89 E5 83 ...e.[^_...6U...
0800:15D0 EC 04 57 56 53 8B 7D 0C 8B 45 08 03 05 1C B3 00 ..WVS.}..E......
0800:15E0 00 89 C1 2B 0D 9C B2 00 00 89 CB C1 EB 10 89 FE ...+............
0800:15F0 C1 EE 10 B8 01 06 00 00 CD 31 72 03 66 31 C0 89 .........1r.f1..
0800:1600 C1 89 CA 66 85 C9 74 0E 0F B7 C2 50 68 A4 86 00 ...f..t....Ph...
0800:1610 00 E8 EE EC FF FF 8D 65 F0 5B 5E 5F C9 C3 8D 36 .......e.[^_...6
0800:1620 55 89 E5 56 53 8B 45 08 A8 02 74 0A C7 05 58 91 U..VS.E...t...X.
0800:1630 00 00 01 00 00 00 A8 01 74 33 31 DB 39 1D 18 B3 ........t31.9...
0800:1640 00 00 7E 29 BE 20 B3 00 00 8D 76 00 8D 04 9B C1 ..~). ....v.....
0800:1650 E0 02 FF 74 06 08 FF B0 20 B3 00 00 E8 17 FF FF ...t.... .......
0800:1660 FF 83 C4 08 43 39 1D 18 B3 00 00 7F DF 31 C0 8D ....C9.......1..
0800:1670 65 F8 5B 5E C9 C3 8D 36 55 89 E5 68 B4 86 00 00 e.[^...6U..h....
0800:1680 E8 7F EC FF FF C9 C3 90 55 89 E5 B8 1B C0 00 00 ........U.......
0800:1690 C9 C3 8D 36 55 89 E5 83 EC 18 57 56 53 8B 5D 0C ...6U.....WVS.].
0800:16A0 8B 45 08 8B 04 85 B4 8D 00 00 89 45 F8 66 8B 15 .E.........E.f..
0800:16B0 44 91 00 00 66 89 55 FC 83 7D 08 07 7F 0E 8B 75 D...f.U..}.....u
0800:16C0 08 03 35 AC B2 00 00 EB 0F 8D 76 00 8B 75 08 03 ..5.......v..u..
0800:16D0 35 B0 B2 00 00 83 C6 F8 83 45 10 EC 8B 4D 08 C1 5........E...M..
0800:16E0 E1 03 89 4D F0 89 99 08 B2 00 00 8B 7D 10 89 B9 ...M........}...
0800:16F0 0C B2 00 00 03 3D 1C B3 00 00 A1 0C B3 00 00 83 .....=..........
0800:1700 C0 E0 89 47 08 8B 15 1C B3 00 00 83 C2 E1 03 15 ...G............
0800:1710 0C B3 00 00 C7 02 24 20 00 00 66 8B 0D 44 91 00 ......$ ..f..D..
0800:1720 00 66 89 4A 04 89 F7 89 FB 8B 45 F0 05 5C 91 00 .f.J......E..\..
0800:1730 00 89 45 F4 B8 04 02 00 00 CD 31 72 03 66 31 C0 ..E.......1r.f1.
0800:1740 89 C3 8B 45 F4 66 89 48 04 8B 7D F0 89 97 5C 91 ...E.f.H..}...\.
0800:1750 00 00 0F B7 DB 85 DB 75 27 89 F0 88 C3 B8 05 02 .......u'.......
0800:1760 00 00 66 8B 4D FC 8B 55 F8 CD 31 72 03 66 31 C0 ..f.M..U..1r.f1.
0800:1770 89 C3 0F B7 DB 85 DB 75 07 31 C0 EB 05 8D 76 00 .......u.1....v.
0800:1780 89 D8 8D 65 DC 5B 5E 5F C9 C3 8D 36 55 89 E5 83 ...e.[^_...6U...
0800:1790 EC 04 57 56 53 8B 7D 08 83 FF 07 7F 0B 89 FB 03 ..WVS.}.........
0800:17A0 1D AC B2 00 00 EB 0C 90 89 FB 03 1D B0 B2 00 00 ................
0800:17B0 83 C3 F8 C1 E7 03 66 83 BF 60 91 00 00 00 74 1B ......f..`....t.
0800:17C0 81 C7 5C 91 00 00 89 7D FC B8 05 02 00 00 66 8B ..\....}......f.
0800:17D0 4F 04 8B 17 CD 31 72 03 66 31 C0 8D 65 F0 5B 5E O....1r.f1..e.[^
0800:17E0 5F C9 C3 90                                     _...           

fn0800_17E4()
	push	ebp
	mov	ebp,esp
	sub	esp,000000A4
	push	edi
	push	esi
	push	ebx
	mov	eax,00001687
	push	es
	int	2F
	mov	[000091DE],es
	mov	[000091DC],di
	pop	es
	mov	[ebp+FFFFFF6C],ax
	mov	[ebp+FFFFFF60],bx
	test	ax,ax
	jnz	1D7B

l0800_1825:
	test	bl,01
	jnz	183B

l0800_182B:
	push	000086F4
	call	2970
	add	esp,04

l0800_183B:
	cmp	dword ptr [0000B28C],00
	jz	185A

l0800_1847:
	movzx	eax,si
	shl	eax,04
	cmp	[0000B290],eax
	jnc	186A

l0800_185A:
	push	0000871F
	call	2970
	add	esp,04

l0800_186A:
	mov	eax,[0000B28C]
	shr	eax,04
	mov	[ebp+FFFFFF5C],ax
	movzx	esi,si
	mov	edx,esi
	shl	edx,04
	add	[0000B28C],edx
	sub	[0000B290],edx
	mov	eax,00000001
	mov	es,[ebp+FFFFFF5C]
	call	dword ptr [000091DC]
	jc	18B0
	xor	ax,ax
	push	ds
	pop	es
	mov	esi,eax
	test	si,si
	jz	18CA
	push	0000874D
	call	2970
	add	esp,04
	mov	eax,0000000D
	mov	ebx,00000007
	int	31
	jc	18DD
	xor	ax,ax
	mov	edx,eax
	test	dx,dx
	jz	18F5
	push	0000874D
	call	2970
	add	esp,04
	xor	eax,eax
	mov	ecx,00000001
	int	31
	jc	190B
	mov	[00009144],ax
	xor	ax,ax
	mov	edx,eax
	test	dx,dx
	jz	1923
	push	0000874D
	call	2970
	add	esp,04
	mov	bx,cs
	lea	edi,[ebp-08]
	mov	eax,0000000B
	int	31
	jc	1937
	xor	ax,ax
	mov	edx,eax
	test	dx,dx
	jz	194F
	push	0000874D
	call	2970
	add	esp,04
	mov	dl,[ebp-02]
	shr	dl,04
	movzx	ecx,dl
	or	cl,04
	mov	[ebp+FFFFFF68],ecx
	mov	bl,[ebp+FFFFFF68]
	shl	bl,04
	and	byte ptr [ebp-02],0F
	or	[ebp-02],bl
	mov	si,[00009144]
	lea	edi,[ebp-08]
	mov	eax,0000000C
	mov	ebx,esi
	int	31
	jc	1994
	xor	ax,ax
	mov	esi,eax
	test	si,si
	jz	19AC
	push	0000874D
	call	2970
	add	esp,04
	mov	bx,[00009144]
	mov	eax,00000008
	mov	ecx,0000FFFF
	mov	edx,FFFFFFFF
	int	31
	jc	19CC
	xor	ax,ax
	mov	ebx,eax
	test	bx,bx
	jz	19E4
	push	0000874D
	call	2970
	add	esp,04
	mov	bx,[00009144]
	mov	eax,0000000A
	int	31
	jc	19FE
	mov	[00009146],ax
	xor	ax,ax
	mov	edx,eax
	test	dx,dx
	jz	1A16
	push	0000874D
	call	2970
	add	esp,04
	pushf	
	pop	edx
	mov	[ebp+FFFFFF68],edx
	mov	cx,[ebp+FFFFFF68]
	mov	[0000B3F0],cx
	mov	dword ptr [00009090],0000131C
	mov	dword ptr [0000908C],00001358
	mov	dword ptr [0000B348],000086C8
	movzx	ebx,word ptr [00009144]
	mov	ax,[00009146]
	push	ebx
	push	00001A6D
	retf	
	mov	ds,ax
	mov	es,ax
	mov	ss,ax
	mov	edx,[di]
	inc	si
	xchg	ax,cx
	add	[bx+si],al
	mov	[di],edx
	cld	
	mov	cl,00
	add	bh,al
	add	ax,B1F8
	add	[bx+si],al
	lock	
	test	al,00
	add	[bx+si+0604],bh
	add	[bx+si],al
	int	31
	jc	1AA0
	shl	bx,10
	mov	ebx,ecx
	xor	eax,eax
	mov	di,ax
	mov	[di],bx
	dec	ax
	xchg	ax,cx
	add	[bx+si],al
	mov	si,di
	test	edi,edi
	jz	1AC0
	movzx	ax,si
	push	ax
	push	86A4
	add	[bx+si],al
	call	0302
	illegal	
	add	sp,08
	mov	word ptr [di-10],1D9C
	add	[bx+si],al
	mov	eax,[9144]
	add	[bx+si],al
	mov	[di+FF7C],eax
	illegal	
	mov	[di-0C],eax
	mov	ax,0203
	add	[bx+si],al
	xor	bl,bl
	mov	ecx,[di-0C]
	mov	dx,[di-10]
	int	31
	jc	1AED
	xor	eax,eax
	mov	word ptr [di-18],1DA0
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-14],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,01
	mov	ecx,[di-14]
	mov	dx,[di-18]
	int	31
	jc	1B14
	xor	eax,eax
	mov	word ptr [di-20],1DA4
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-1C],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,02
	mov	ecx,[di-1C]
	mov	dx,[di-20]
	int	31
	jc	1B3B
	xor	eax,eax
	mov	word ptr [di-28],1DA8
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-24],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,03
	mov	ecx,[di-24]
	mov	dx,[di-28]
	int	31
	jc	1B62
	xor	eax,eax
	mov	word ptr [di-30],1DAC
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-2C],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,04
	mov	ecx,[di-2C]
	mov	dx,[di-30]
	int	31
	jc	1B89
	xor	eax,eax
	mov	word ptr [di-38],1DB0
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-34],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,05
	mov	ecx,[di-34]
	mov	dx,[di-38]
	int	31
	jc	1BB0
	xor	eax,eax
	mov	word ptr [di-40],1DB4
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-3C],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,06
	mov	ecx,[di-3C]
	mov	dx,[di-40]
	int	31
	jc	1BD7
	xor	eax,eax
	mov	word ptr [di-48],1DB8
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-44],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,07
	mov	ecx,[di-44]
	mov	dx,[di-48]
	int	31
	jc	1BFE
	xor	eax,eax
	mov	word ptr [di-50],1DBC
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-4C],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,08
	mov	ecx,[di-4C]
	mov	dx,[di-50]
	int	31
	jc	1C25
	xor	eax,eax
	mov	word ptr [di-58],1DC0
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-54],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,09
	mov	ecx,[di-54]
	mov	dx,[di-58]
	int	31
	jc	1C4C
	xor	eax,eax
	mov	word ptr [di-60],1DC4
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-5C],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,0A
	mov	ecx,[di-5C]
	mov	dx,[di-60]
	int	31
	jc	1C73
	xor	eax,eax
	mov	word ptr [di-68],1DC8
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-64],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,0B
	mov	ecx,[di-64]
	mov	dx,[di-68]
	int	31
	jc	1C9A
	xor	eax,eax
	mov	word ptr [di-70],1DCC
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-6C],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,0C
	mov	ecx,[di-6C]
	mov	dx,[di-70]
	int	31
	jc	1CC1
	xor	eax,eax
	mov	word ptr [di-78],1DD0
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-74],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,0D
	mov	ecx,[di-74]
	mov	dx,[di-78]
	int	31
	jc	1CE8
	xor	eax,eax
	mov	word ptr [di-80],1DD4
	add	[bx+si],al
	mov	eax,[di+FF7C]
	illegal	
	mov	[di-7C],eax
	mov	ax,0203
	add	[bx+si],al
	mov	bl,0E
	mov	ecx,[di-7C]
	mov	dx,[di-80]
	int	31
	jc	1D0F
	xor	eax,eax
	mov	word ptr [di+FF78],FFFF
	fcomp	dword ptr [di]
	add	[bx+si],al
	mov	ax,0203
	add	[bx+si],al
	mov	bl,0F
	mov	ecx,[di+FF7C]
	illegal	
	mov	dx,[di+FF78]
	illegal	
	int	31
	jc	1D34
	xor	eax,eax
	mov	ax,0400
	add	[bx+si],al
	int	31
	mov	[di+FF70],eax
	illegal	
	mov	si,bx
	mov	[di+FF74],edx
	illegal	
	mov	[di+FF72],esi
	illegal	
	mov	[di+FF76],cl
	illegal	
	movzx	ax,byte ptr [di+FF75]
	illegal	
	mov	[B2AC],ax
	add	[bx+si],al
	movzx	dx,byte ptr [di+FF74]
	illegal	
	mov	[di],dx
	mov	al,B2
	add	[bx+si],al
	call	5352
	add	[bx+si],al
	call	5552
	add	[bx+si],al

l0800_1D7B:
	lea	esp,[ebp+FFFFFF50]
	pop	ebx
	pop	esi
	pop	edi
	leave	
	ret	
0800:1D8D                                        8D 76 00              .v.

fn0800_1D90()
	push	ebp
	mov	ebp,esp
	leave	
	ret	
0800:1D99                            00 00 00 6A 00 EB 3C          ...j..<
0800:1DA0 6A 01 EB 38 6A 02 EB 34 6A 03 EB 30 6A 04 EB 2C j..8j..4j..0j..,
0800:1DB0 6A 05 EB 28 6A 06 EB 24 6A 07 EB 20 6A 08 EB 1C j..(j..$j.. j...
0800:1DC0 6A 09 EB 18 6A 0A EB 14 6A 0B EB 10 6A 0C EB 0C j...j...j...j...
0800:1DD0 6A 0D EB 08 6A 0E EB 04 6A 0F EB 00 50 53 1E 2E j...j...j...PS..
0800:1DE0 A1 FC B1 00 00 66 8E D8 8B 5C 24 28 66 39 44 24 .....f...\$(f9D$
0800:1DF0 2C 74 06 8B 1D F8 B1 00 00 83 EB 1C 8B 44 24 0C ,t...........D$.
0800:1E00 89 03 8B 44 24 18 89 43 04 8B 44 24 1C 89 43 08 ...D$..C..D$..C.
0800:1E10 8B 44 24 20 89 43 0C 8B 44 24 24 89 43 10 8B 44 .D$ .C..D$$.C..D
0800:1E20 24 28 89 43 14 8B 44 24 2C 89 43 18 C7 44 24 1C $(.C..D$,.C..D$.
0800:1E30 55 1E 00 00 66 8C C8 89 44 24 20 81 64 24 24 FF U...f...D$ .d$$.
0800:1E40 F8 FF FF 89 5C 24 28 66 8C D8 89 44 24 2C 1F 5B ....\$(f...D$,.[
0800:1E50 58 83 C4 04 CB 60 1E 06 0F A0 0F A8 66 8C D0 66 X....`......f..f
0800:1E60 8E D8 66 8E C0 66 8C C8 66 39 44 24 3C 75 06 54 ..f..f..f9D$<u.T
0800:1E70 E8 97 4E 00 00 83 7C 24 30 20 73 05 E8 5F 44 00 ..N...|$0 s.._D.
0800:1E80 00 8B 15 FC D5 00 00 09 D2 74 05 E8 30 43 00 00 .........t..0C..
0800:1E90 0F A9 0F A1 07 1F 61 83 C4 10 F7 04 24 00 02 00 ......a.....$...
0800:1EA0 00 75 10 9D 2E 0F B2 25 E8 A8 00 00 2E FF 2D DC .u.....%......-.
0800:1EB0 A8 00 00 81 24 24 FF FD FF FF 9D 2E 0F B2 25 E8 ....$$........%.
0800:1EC0 A8 00 00 FB 2E FF 2D DC A8 00 00 90 50 B0 20 E6 ......-.....P. .
0800:1ED0 20 8D 05 08 B2 00 00 E9 14 01 00 00 50 B0 20 E6  ...........P. .
0800:1EE0 20 8D 05 10 B2 00 00 E9 04 01 00 00 50 B0 20 E6  ...........P. .
0800:1EF0 20 8D 05 18 B2 00 00 E9 F4 00 00 00 50 B0 20 E6  ...........P. .
0800:1F00 20 8D 05 20 B2 00 00 E9 E4 00 00 00 50 B0 20 E6  .. ........P. .
0800:1F10 20 8D 05 28 B2 00 00 E9 D4 00 00 00 50 B0 20 E6  ..(........P. .
0800:1F20 20 8D 05 30 B2 00 00 E9 C4 00 00 00 50 B0 20 E6  ..0........P. .
0800:1F30 20 8D 05 38 B2 00 00 E9 B4 00 00 00 50 B0 20 E6  ..8........P. .
0800:1F40 20 8D 05 40 B2 00 00 E9 A4 00 00 00 50 B0 20 E6  ..@........P. .
0800:1F50 A0 EB 00 EB 00 E6 20 8D 05 48 B2 00 00 E9 8E 00 ...... ..H......
0800:1F60 00 00 8D 36 50 B0 20 E6 A0 EB 00 EB 00 E6 20 8D ...6P. ....... .
0800:1F70 05 50 B2 00 00 EB 79 90 50 B0 20 E6 A0 EB 00 EB .P....y.P. .....
0800:1F80 00 E6 20 8D 05 58 B2 00 00 EB 65 90 50 B0 20 E6 .. ..X....e.P. .
0800:1F90 A0 EB 00 EB 00 E6 20 8D 05 60 B2 00 00 EB 51 90 ...... ..`....Q.
0800:1FA0 50 B0 20 E6 A0 EB 00 EB 00 E6 20 8D 05 68 B2 00 P. ....... ..h..
0800:1FB0 00 EB 3D 90 50 B0 20 E6 A0 EB 00 EB 00 E6 20 8D ..=.P. ....... .
0800:1FC0 05 70 B2 00 00 EB 29 90 50 B0 20 E6 A0 EB 00 EB .p....).P. .....
0800:1FD0 00 E6 20 8D 05 78 B2 00 00 EB 15 90 50 B0 20 E6 .. ..x......P. .
0800:1FE0 A0 EB 00 EB 00 E6 20 8D 05 80 B2 00 00 EB 01 90 ...... .........
0800:1FF0 51 52 1E 06 66 2E 8B 15 F4 B1 00 00 66 8E DA 66 QR..f.......f..f
0800:2000 8E C2 2E 8B 08 2E 8B 40 04 89 08 66 2E 8B 0D 88 .......@...f....
0800:2010 B2 00 00 89 48 04 89 60 0C 66 8C 50 10 89 C4 66 ....H..`.f.P...f
0800:2020 8E D2 FC CB 0F B2 24 24 07 1F 5A 59 58 FB CF 90 ......$$..ZYX...
0800:2030 FA FC 83 F8 FF 0F 84 96 00 00 00 89 E1 2E 0F B2 ................
0800:2040 25 F8 B1 00 00 66 8C D2 66 8E DA 66 8E C2 89 0D %....f..f..f....
0800:2050 00 B2 00 00 C7 05 1C D6 00 00 00 00 00 00 03 0D ................
0800:2060 1C B3 00 00 83 C1 0C 51 FF 14 85 94 90 00 00 8B .......Q........
0800:2070 15 1C D6 00 00 8B 0D FC D5 00 00 09 C9 75 12 0F .............u..
0800:2080 B2 25 00 B2 00 00 66 8C D1 66 8E D9 66 8E C1 FB .%....f..f..f...
0800:2090 CB BC D4 A8 00 00 0F B4 0D 00 B2 00 00 60 0F A0 .............`..
0800:20A0 0F A0 6A 00 6A 00 64 8B 01 89 44 24 38 64 8B 41 ..j.j.d...D$8d.A
0800:20B0 04 89 44 24 3C 9C 58 0D 00 02 00 00 89 44 24 40 ..D$<.X......D$@
0800:20C0 83 C1 08 89 4C 24 44 66 8C 64 24 48 E9 BA FD FF ....L$Df.d$H....
0800:20D0 FF EA 00 00 00 00 00 00 55 89 E5 83 EC 3C 57 56 ........U....<WV
0800:20E0 53 68 E8 91 00 00 6A 00 6A 00 A1 94 B2 00 00 FF Sh....j.j.......
0800:20F0 30 E8 52 E7 FF FF 83 C4 10 85 C0 0F 85 3E 02 00 0.R..........>..
0800:2100 00 8D 5D D4 53 6A 06 8D 45 F8 50 FF 35 E8 91 00 ..].Sj..E.P.5...
0800:2110 00 E8 EA E8 FF FF 83 C4 10 85 C0 0F 85 1E 02 00 ................
0800:2120 00 83 7D D4 06 0F 85 14 02 00 00 66 81 7D F8 4D ..}........f.}.M
0800:2130 5A 0F 85 08 02 00 00 0F B7 7D FC C1 E7 09 8D 45 Z........}.....E
0800:2140 D0 50 6A 00 57 FF 35 E8 91 00 00 E8 BC EA FF FF .Pj.W.5.........
0800:2150 83 C4 10 85 C0 0F 85 C5 01 00 00 39 7D D0 0F 85 ...........9}...
0800:2160 BC 01 00 00 53 6A 20 8D 5D D8 53 FF 35 E8 91 00 ....Sj .].S.5...
0800:2170 00 E8 8A E8 FF FF 83 C4 10 85 C0 0F 85 9F 01 00 ................
0800:2180 00 83 7D D4 20 0F 85 95 01 00 00 8D 45 C8 50 8D ..}. .......E.P.
0800:2190 45 CC 50 68 58 87 00 00 53 E8 5A 57 00 00 83 C4 E.PhX...S.ZW....
0800:21A0 10 83 F8 02 0F 85 76 01 00 00 83 7D C8 00 0F 8E ......v....}....
0800:21B0 6C 01 00 00 83 7D CC 00 0F 8E 62 01 00 00 81 C7 l....}....b.....
0800:21C0 00 02 00 00 FF 75 C8 E8 B4 55 00 00 89 45 C4 83 .....u...U...E..
0800:21D0 C4 04 85 C0 75 0D 68 63 87 00 00 E8 24 E1 FF FF ....u.hc....$...
0800:21E0 83 C4 04 8D 45 D0 50 6A 00 57 FF 35 E8 91 00 00 ....E.Pj.W.5....
0800:21F0 E8 17 EA FF FF 83 C4 10 85 C0 0F 85 20 01 00 00 ............ ...
0800:2200 39 7D D0 0F 85 17 01 00 00 8D 45 D4 50 FF 75 C8 9}........E.P.u.
0800:2210 FF 75 C4 FF 35 E8 91 00 00 E8 E2 E7 FF FF 83 C4 .u..5...........
0800:2220 10 85 C0 0F 85 F7 00 00 00 8B 45 D4 39 45 C8 0F ..........E.9E..
0800:2230 85 EB 00 00 00 8B 45 C8 05 FF 01 00 00 25 00 FE ......E......%..
0800:2240 FF FF 01 C7 8B 45 C4 A3 88 90 00 00 6A 0A 50 E8 .....E......j.P.
0800:2250 B8 56 00 00 89 45 C4 83 C4 08 85 C0 0F 84 BE 00 .V...E..........
0800:2260 00 00 C6 00 00 FF 45 C4 FF 4D CC 8B 45 CC A3 E4 ......E..M..E...
0800:2270 91 00 00 8D 04 40 C1 E0 02 50 E8 01 55 00 00 A3 .....@...P..U...
0800:2280 E0 91 00 00 83 C4 04 85 C0 75 0D 68 63 87 00 00 .........u.hc...
0800:2290 E8 6F E0 FF FF 83 C4 04 31 F6 39 35 E4 91 00 00 .o......1.95....
0800:22A0 0F 8E 99 00 00 00 8D 36 8B 15 E0 91 00 00 8D 04 .......6........
0800:22B0 76 8D 1C 85 00 00 00 00 89 7C 1A 04 6A 0A 8D 45 v........|..j..E
0800:22C0 C4 50 FF 75 C4 E8 BE 56 00 00 8B 15 E0 91 00 00 .P.u...V........
0800:22D0 89 44 1A 08 8B 45 C4 83 C4 0C 80 38 20 75 41 FF .D...E.....8 uA.
0800:22E0 45 C4 8B 45 C4 89 04 1A 6A 0A 50 E8 1C 56 00 00 E..E....j.P..V..
0800:22F0 89 45 C4 83 C4 08 85 C0 74 26 C6 00 00 FF 45 C4 .E......t&....E.
0800:2300 A1 E0 91 00 00 8B 44 18 08 05 FF 01 00 00 25 00 ......D.......%.
0800:2310 FE FF FF 01 C7 46 39 35 E4 91 00 00 7F 8A EB 1F .....F95........
0800:2320 C7 05 E0 91 00 00 00 00 00 00 C7 05 E4 91 00 00 ................
0800:2330 00 00 00 00 FF 35 E8 91 00 00 E8 05 E2 FF FF 8D .....5..........
0800:2340 65 B8 5B 5E 5F C9 C3 90 55 89 E5 57 56 53 8B 75 e.[^_...U..WVS.u
0800:2350 08 8B 7D 14 F6 45 0C 03 74 12 B8 22 C0 00 00 EB ..}..E..t.."....
0800:2360 3E 8D 76 00 B8 16 C0 00 00 EB 34 90 31 DB 8D 36 >.v.......4.1..6
0800:2370 39 1D E4 91 00 00 76 EC 8B 15 E0 91 00 00 8D 04 9.....v.........
0800:2380 5B FF 34 82 56 E8 A6 55 00 00 83 C4 08 85 C0 74 [.4.V..U.......t
0800:2390 03 43 EB DC 89 1F C7 47 04 00 00 00 00 31 C0 8D .C.....G.....1..
0800:23A0 65 F4 5B 5E 5F C9 C3 90 55 89 E5 31 C0 C9 C3 90 e.[^_...U..1....
0800:23B0 55 89 E5 57 56 53 8B 7D 08 8B 75 10 8B 07 8D 04 U..WVS.}..u.....
0800:23C0 40 C1 E0 02 89 C3 03 1D E0 91 00 00 8B 47 04 39 @............G.9
0800:23D0 43 08 77 10 8B 55 14 C7 02 00 00 00 00 31 C0 E9 C.w..U.......1..
0800:23E0 81 00 00 00 8B 43 08 2B 47 04 39 C6 76 02 89 C6 .....C.+G.9.v...
0800:23F0 8B 5B 04 03 5F 04 39 1D EC 91 00 00 74 22 68 EC .[.._.9.....t"h.
0800:2400 91 00 00 6A 00 53 FF 35 E8 91 00 00 E8 FB E7 FF ...j.S.5........
0800:2410 FF 83 C4 10 85 C0 75 1E 39 1D EC 91 00 00 75 40 ......u.9.....u@
0800:2420 FF 75 14 56 FF 75 0C FF 35 E8 91 00 00 E8 CE E5 .u.V.u..5.......
0800:2430 FF FF 85 C0 74 0E C7 05 EC 91 00 00 00 00 00 00 ....t...........
0800:2440 EB 23 8D 36 8B 4D 14 8B 09 01 0D EC 91 00 00 8B .#.6.M..........
0800:2450 55 14 39 32 75 0A 01 77 04 31 C0 EB 08 8D 76 00 U.92u..w.1....v.
0800:2460 B8 0F C0 00 00 8D 65 F4 5B 5E 5F C9 C3 8D 76 00 ......e.[^_...v.
0800:2470 55 89 E5 56 53 8B 4D 08 8B 5D 0C 8B 45 10 8B 75 U..VS.M..]..E..u
0800:2480 14 83 F8 01 74 12 7F 08 85 C0 74 23 EB 26 8D 36 ....t.....t#.&.6
0800:2490 83 F8 02 74 0B EB 1D 90 01 59 04 EB 1F 8D 76 00 ...t.....Y....v.
0800:24A0 8B 01 8B 15 E0 91 00 00 8D 04 40 03 5C 82 08 89 ..........@.\...
0800:24B0 59 04 EB 08 B8 0E C0 00 00 EB 08 90 8B 41 04 89 Y............A..
0800:24C0 06 31 C0 8D 65 F8 5B 5E C9 C3 8D 36 55 89 E5 56 .1..e.[^...6U..V
0800:24D0 53 8B 75 08 8B 5D 0C 6A 30 6A 00 53 E8 EB 51 00 S.u..].j0j.S..Q.
0800:24E0 00 66 C7 43 0A 01 00 66 C7 43 08 FF 81 8B 06 8B .f.C...f.C......
0800:24F0 15 E0 91 00 00 8D 04 40 8B 44 82 08 89 43 24 05 .......@.D...C$.
0800:2500 FF 01 00 00 C1 E8 09 89 43 28 C7 43 2C 00 02 00 ........C(.C,...
0800:2510 00 31 C0 8D 65 F8 5B 5E C9 C3 00 00 55 89 E5 53 .1..e.[^....U..S
0800:2520 8B 5D 08 53 E8 B7 DC FF FF 53 A1 8C 90 00 00 FF .].S.....S......
0800:2530 D0 8B 5D FC C9 C3 00 00 55 89 E5 8B 45 08 8B 0D ..].....U...E...
0800:2540 9C B2 00 00 F7 D9 66 C7 80 08 08 00 00 FF FF 66 ......f........f
0800:2550 89 88 0A 08 00 00 89 CA C1 EA 10 88 90 0C 08 00 ................
0800:2560 00 C6 80 0D 08 00 00 9A 80 A0 0E 08 00 00 F0 80 ................
0800:2570 88 0E 08 00 00 0F 80 A0 0E 08 00 00 0F 80 88 0E ................
0800:2580 08 00 00 C0 C1 E9 18 88 88 0F 08 00 00 C9 C3 90 ................
0800:2590 55 89 E5 8B 45 08 8B 0D 9C B2 00 00 F7 D9 66 C7 U...E.........f.
0800:25A0 80 10 08 00 00 FF FF 66 89 88 12 08 00 00 89 CA .......f........
0800:25B0 C1 EA 10 88 90 14 08 00 00 C6 80 15 08 00 00 92 ................
0800:25C0 80 A0 16 08 00 00 F0 80 88 16 08 00 00 0F 80 A0 ................
0800:25D0 16 08 00 00 0F 80 88 16 08 00 00 C0 C1 E9 18 88 ................
0800:25E0 88 17 08 00 00 C9 C3 90 55 89 E5 53 8B 45 08 05 ........U..S.E..
0800:25F0 18 08 00 00 8B 1D 9C B2 00 00 F7 DB B9 FF FF 00 ................
0800:2600 00 66 89 08 66 89 58 02 89 DA C1 EA 10 88 50 04 .f..f.X.......P.
0800:2610 C6 40 05 9A C1 E9 10 88 CA 80 E2 0F 80 60 06 F0 .@...........`..
0800:2620 08 50 06 30 D2 80 60 06 0F 08 50 06 C1 EB 18 88 .P.0..`...P.....
0800:2630 58 07 8B 5D FC C9 C3 90 55 89 E5 53 8B 45 08 05 X..]....U..S.E..
0800:2640 20 08 00 00 8B 1D 9C B2 00 00 F7 DB B9 FF FF 00  ...............
0800:2650 00 66 89 08 66 89 58 02 89 DA C1 EA 10 88 50 04 .f..f.X.......P.
0800:2660 C6 40 05 92 C1 E9 10 88 CA 80 E2 0F 80 60 06 F0 .@...........`..
0800:2670 08 50 06 30 D2 80 60 06 0F 08 50 06 C1 EB 18 88 .P.0..`...P.....
0800:2680 58 07 8B 5D FC C9 C3 90 55 89 E5 8B 45 08 66 C7 X..]....U...E.f.
0800:2690 80 28 08 00 00 FF FF 66 C7 80 2A 08 00 00 00 00 .(.....f..*.....
0800:26A0 C6 80 2C 08 00 00 00 C6 80 2D 08 00 00 9A 80 A0 ..,......-......
0800:26B0 2E 08 00 00 F0 80 88 2E 08 00 00 0F 80 A0 2E 08 ................
0800:26C0 00 00 0F 80 88 2E 08 00 00 C0 C6 80 2F 08 00 00 ............/...
0800:26D0 00 C9 C3 90 55 89 E5 8B 45 08 66 C7 80 30 08 00 ....U...E.f..0..
0800:26E0 00 FF FF 66 C7 80 32 08 00 00 00 00 C6 80 34 08 ...f..2.......4.
0800:26F0 00 00 00 C6 80 35 08 00 00 92 80 A0 36 08 00 00 .....5......6...
0800:2700 F0 80 88 36 08 00 00 0F 80 A0 36 08 00 00 0F 80 ...6......6.....
0800:2710 88 36 08 00 00 C0 C6 80 37 08 00 00 00 C9 C3 90 .6......7.......
0800:2720 55 89 E5 53 8B 4D 08 8B 15 9C B2 00 00 81 C2 98 U..S.M..........
0800:2730 F7 FF FF 8D 81 38 08 00 00 29 D1 BB 2F 00 00 00 .....8...)../...
0800:2740 66 89 18 66 89 48 02 89 CA C1 EA 10 88 50 04 C6 f..f.H.......P..
0800:2750 40 05 82 C1 EB 10 88 DA 80 E2 0F 80 60 06 F0 08 @...........`...
0800:2760 50 06 30 D2 80 60 06 0F 08 50 06 C1 E9 18 88 48 P.0..`...P.....H
0800:2770 07 8B 5D FC C9 C3 8D 36 55 89 E5 53 8B 4D 08 8B ..]....6U..S.M..
0800:2780 15 9C B2 00 00 81 C2 68 F7 FF FF 8D 81 40 08 00 .......h.....@..
0800:2790 00 29 D1 BB 67 00 00 00 66 89 18 66 89 48 02 89 .)..g...f..f.H..
0800:27A0 CA C1 EA 10 88 50 04 C6 40 05 89 C1 EB 10 88 DA .....P..@.......
0800:27B0 80 E2 0F 80 60 06 F0 08 50 06 30 D2 80 60 06 0F ....`...P.0..`..
0800:27C0 08 50 06 C1 E9 18 88 48 07 8B 5D FC C9 C3 8D 36 .P.....H..]....6
0800:27D0 55 89 E5 53 8B 5D 08 53 E8 5B FD FF FF 53 E8 AD U..S.].S.[...S..
0800:27E0 FD FF FF 53 E8 FF FD FF FF 53 E8 49 FE FF FF 53 ...S.....S.I...S
0800:27F0 E8 93 FE FF FF 53 E8 D9 FE FF FF 53 E8 1F FF FF .....S.....S....
0800:2800 FF 53 E8 71 FF FF FF 83 C4 20 53 E8 00 39 00 00 .S.q..... S..9..
0800:2810 8B 5D FC C9 C3 00 00 00 66 55 66 89 E5 66 68 E8 .]......fUf..fh.
0800:2820 03 00 00 66 E8 EB 05 00 00 66 83 C4 04 66 BA 64 ...f.....f...f.d
0800:2830 00 00 00 EC 66 25 FF 00 00 00 A8 01 66 74 1D 66 ....f%......ft.f
0800:2840 68 E8 03 00 00 66 E8 C9 05 00 00 66 83 C4 04 66 h....f.....f...f
0800:2850 BA 60 00 00 00 EC 66 EB C4 8D 76 00 A8 02 66 75 .`....f...v...fu
0800:2860 BC 66 C9 66 C3 8D 76 00 66 55 66 89 E5 66 53 66 .f.f..v.fUf..fSf
0800:2870 E8 0B 09 00 00 66 85 C0 66 75 65 66 BA 92 00 00 .....f..fuef....
0800:2880 00 EC 66 0F B6 D8 66 68 E8 03 00 00 66 E8 82 05 ..f...fh....f...
0800:2890 00 00 66 83 C4 04 88 D8 0C 02 66 BA 92 00 00 00 ..f.......f.....
0800:28A0 EE 66 E8 D9 08 00 00 66 85 C0 66 75 33 66 E8 65 .f.....f..fu3f.e
0800:28B0 FF FF FF B0 D1 66 BA 64 00 00 00 EE 66 E8 56 FF .....f.d....f.V.
0800:28C0 FF FF B0 DF 66 BA 60 00 00 00 EE 66 E8 47 FF FF ....f.`....f.G..
0800:28D0 FF 8D 76 00 66 E8 A6 08 00 00 66 85 C0 66 74 F4 ..v.f.....f..ft.
0800:28E0 66 67 8B 5D FC 66 C9 66 C3 8D 76 00 66 55 66 89 fg.].f.f..v.fUf.
0800:28F0 E5 66 53 66 E8 87 08 00 00 66 85 C0 66 74 65 66 .fSf.....f..ftef
0800:2900 BA 92 00 00 00 EC 66 0F B6 D8 66 68 E8 03 00 00 ......f...fh....
0800:2910 66 E8 FE 04 00 00 66 83 C4 04 88 D8 24 FD 66 BA f.....f.....$.f.
0800:2920 92 00 00 00 EE 66 E8 55 08 00 00 66 85 C0 66 74 .....f.U...f..ft
0800:2930 33 66 E8 E1 FE FF FF B0 D1 66 BA 64 00 00 00 EE 3f.......f.d....
0800:2940 66 E8 D2 FE FF FF B0 DD 66 BA 60 00 00 00 EE 66 f.......f.`....f
0800:2950 E8 C3 FE FF FF 8D 76 00 66 E8 22 08 00 00 66 85 ......v.f."...f.
0800:2960 C0 66 75 F4 66 67 8B 5D FC 66 C9 66 C3 00 00 00 .fu.fg.].f.f....

fn0800_2970()
	push	ebp
	mov	ebp,esp
	push	ebx
	mov	ebx,[ebp+08]
	test	ebx,ebx
	jz	298E

l0800_2982:
	push	ebx
	call	2F30
	add	esp,04

l0800_298E:
	test	ebx,ebx
	setnz	al
	and	eax,000000FF
	push	eax
	call	2A3C
	mov	ebx,[ebp-04]
	leave	
	ret	
0800:29AC                                     55 89 E5 83             U...
0800:29B0 3D 8C B2 00 00 00 74 1B FF 35 90 B2 00 00 FF 35 =.....t..5.....5
0800:29C0 8C B2 00 00 E8 EF 28 00 00 C7 05 8C B2 00 00 00 ......(.........
0800:29D0 00 00 00 C9 C3 8D 76 00                         ......v.       

fn0800_29D8()
	push	ebp
	mov	ebp,esp
	sub	esp,04
	push	ebx
	mov	word ptr [ebp-02],F000
	lea	si,[bp+00]

l0800_29EC:
	cmp	word ptr [ebp-02],00
	jz	2A33

l0800_29F4:
	mov	eax,00004800
	mov	bx,[ebp-02]
	int	21
	jnc	2A09

l0800_2A03:
	mov	eax,FFFFFFFF

l0800_2A09:
	mov	edx,eax
	mov	[ebp-02],bx
	test	edx,edx
	jl	29EC

l0800_2A16:
	shl	edx,04
	mov	[0000B28C],edx
	movzx	eax,word ptr [ebp-02]
	shl	eax,04
	mov	[0000B290],eax

l0800_2A33:
	mov	ebx,[ebp-08]
	leave	
	ret	

fn0800_2A3C()
	push	ebp
	mov	ebp,esp
	push	ebx
	mov	ebx,[ebp+08]
	call	2C1C
	call	3DA4
	call	3A34
	call	1D90
	movzx	ebx,bl
	mov	eax,ebx
	or	ah,4C
	int	21
0800:2A6C                                     66 67 8B 5D             fg.]
0800:2A70 FC 66 C9 66 C3 00 00 00 55 89 E5 83 3D F0 91 00 .f.f....U...=...
0800:2A80 00 00 74 1B FF 35 F4 91 00 00 FF 35 F0 91 00 00 ..t..5.....5....
0800:2A90 E8 23 28 00 00 C7 05 F0 91 00 00 00 00 00 00 C9 .#(.............
0800:2AA0 C3 8D 76 00                                     ..v.           

fn0800_2AA4()
	push	ebp
	mov	ebp,esp
	sub	esp,60
	push	edi
	push	esi
	push	ebx
	mov	eax,00008800
	int	15
	jnc	2AC0

l0800_2ABE:
	xor	ax,ax

l0800_2AC0:
	mov	ebx,eax
	movzx	ebx,bx
	mov	edx,ebx
	shl	edx,0A
	add	edx,00100000
	mov	[ebp-54],edx
	mov	ebx,00100000
	cmp	edx,00100000
	jbe	2C0C

l0800_2AEE:
	mov	eax,0000877B
	mov	esi,eax
	mov	edx,ebx
	push	es
	xor	ax,ax
	mov	es,ax
	mov	ax,es:[00000066]
	mov	es,ax
	mov	di,0012
	mov	cx,0007

l0800_2B0E:
	rep	
	cmpsb	

l0800_2B10:
	jnz	2B2A

l0800_2B13:
	xor	edx,edx
	mov	dl,es:[0000002E]
	shl	edx,10
	mov	dx,es:[0000002C]

l0800_2B2A:
	pop	es
	mov	ebx,edx
	cmp	[ebp-54],ebx
	jbe	2C0C

l0800_2B3A:
	lea	eax,[ebp-20]
	mov	edi,eax
	sub	edi,[0000B29C]
	mov	word ptr [ebp-60],0000

l0800_2B50:
	movzx	eax,word ptr [ebp-60]
	mov	byte ptr [eax+ebp-50],00
	inc	word ptr [ebp-60]
	cmp	word ptr [ebp-60],2F
	jbe	2B50

l0800_2B68:
	mov	word ptr [ebp-40],FFFF
	mov	dword ptr [ebp-3E],00100000
	mov	byte ptr [ebp-3B],93
	mov	word ptr [ebp-38],FFFF
	mov	[ebp-36],edi
	mov	byte ptr [ebp-33],93
	lea	esi,[ebp-50]
	mov	eax,00008700
	mov	ecx,00000010
	int	15
	mov	edx,eax
	shr	dx,08
	mov	[ebp-58],dl
	test	dl,dl
	jnz	2BD5

l0800_2BAE:
	cmp	byte ptr [ebp-1D],56
	jnz	2BD5

l0800_2BB6:
	cmp	dword ptr [ebp-1C],4449534B
	jnz	2BD5

l0800_2BC2:
	movzx	edi,word ptr [ebp-02]
	shl	edi,0A
	cmp	edi,ebx
	jbe	2BD5

l0800_2BD2:
	mov	ebx,edi

l0800_2BD5:
	cmp	[ebp-54],ebx
	jbe	2C0C

l0800_2BDD:
	mov	[000091F0],ebx
	mov	esi,[ebp-54]
	sub	esi,ebx
	mov	[000091F4],esi
	mov	eax,[ebp-54]
	cmp	[0000B2A0],eax
	jnc	2C0C

l0800_2C05:
	mov	[0000B2A0],eax

l0800_2C0C:
	lea	esp,[ebp-6C]
	pop	ebx
	pop	esi
	pop	edi
	leave	
	ret	
0800:2C1B                                  90                        .   

fn0800_2C1C()
	push	ebp
	mov	ebp,esp
	leave	
	ret	
0800:2C25                00 00 00                              ...       

fn0800_2C28()
	push	ebp
	mov	ebp,esp
	mov	ecx,[0000B2A8]
	mov	word ptr [0000B0FC],FFFF
	mov	[0000B0FE],cx
	mov	edx,ecx
	shr	edx,10
	mov	[0000B100],dl
	mov	byte ptr [0000B101],9A
	and	byte ptr [0000B102],F0
	or	byte ptr [0000B102],0F
	and	byte ptr [0000B102],0F
	or	byte ptr [0000B102],C0
	mov	eax,ecx
	shr	eax,18
	mov	[0000B103],al
	mov	word ptr [0000B104],FFFF
	mov	[0000B106],cx
	mov	[0000B108],dl
	mov	byte ptr [0000B109],92
	and	byte ptr [0000B10A],F0
	or	byte ptr [0000B10A],0F
	and	byte ptr [0000B10A],0F
	or	byte ptr [0000B10A],C0
	mov	[0000B10B],al
	mov	edx,0000FFFF
	mov	[0000B10C],dx
	mov	[0000B10E],cx
	mov	eax,ecx
	shr	eax,10
	mov	[0000B110],al
	mov	byte ptr [0000B111],9A
	shr	edx,10
	mov	al,dl
	and	al,0F
	and	byte ptr [0000B112],F0
	or	[0000B112],al
	xor	al,al
	and	byte ptr [0000B112],0F
	or	[0000B112],al
	shr	ecx,18
	mov	[0000B113],cl
	mov	ecx,[0000B2A8]
	mov	edx,0000FFFF
	mov	[0000B114],dx
	mov	[0000B116],cx
	mov	eax,ecx
	shr	eax,10
	mov	[0000B118],al
	mov	byte ptr [0000B119],92
	shr	edx,10
	mov	al,dl
	and	al,0F
	and	byte ptr [0000B11A],F0
	or	[0000B11A],al
	xor	al,al
	and	byte ptr [0000B11A],0F
	or	[0000B11A],al
	shr	ecx,18
	mov	[0000B11B],cl
	leave	
	ret	
0800:2D95                00 00 00                              ...       

fn0800_2D98()
	push	ebp
	mov	ebp,esp
	sub	esp,04
	mov	edx,[ebp+0C]
	mov	eax,[ebp+08]
	mov	[0000B298],eax
	mov	[0000B294],edx
	call	2ED0
	mov	eax,00003000
	int	21
	mov	edx,eax
	ror	dx,08
	mov	[ebp-04],dx
	cmp	dx,02FF
	ja	2DE9

l0800_2DD9:
	push	00008783
	call	2970
	add	esp,04

l0800_2DE9:
	call	29D8
	call	17E4
	call	3C80
	call	2AA4
	call	3558
	call	30E8
	leave	
	ret	
	add	[bx+si],al
	add	[bp+55],ah
	mov	ebp,esp
	jmp	2E1C
	jmp	2E1F
	jmp	2E22
	leave	
	ret	
	add	[bx+si],al

fn0800_2E28()
	push	ebp
	mov	ebp,esp
	mov	edx,00000021
	in	al,dx
	mov	cl,al
	jmp	2E39

l0800_2E39:
	jmp	2E3C

l0800_2E3C:
	mov	al,11
	mov	edx,00000020
	out	dx,al
	jmp	2E48

l0800_2E48:
	jmp	2E4B

l0800_2E4B:
	mov	al,[ebp+08]
	mov	edx,00000021
	out	dx,al
	jmp	2E59

l0800_2E59:
	jmp	2E5C

l0800_2E5C:
	mov	al,04
	out	dx,al
	jmp	2E62

l0800_2E62:
	jmp	2E65

l0800_2E65:
	mov	al,01
	out	dx,al
	jmp	2E6B

l0800_2E6B:
	jmp	2E6E

l0800_2E6E:
	mov	al,cl
	out	dx,al
	jmp	2E74

l0800_2E74:
	jmp	2E77

l0800_2E77:
	leave	
	ret	
0800:2E7B                                  90 66 55 66 89            .fUf.
0800:2E80 E5 66 BA A1 00 00 00 EC 88 C1 66 EB 00 66 EB 00 .f........f..f..
0800:2E90 B0 11 66 BA A0 00 00 00 EE 66 EB 00 66 EB 00 67 ..f......f..f..g
0800:2EA0 8A 45 08 66 BA A1 00 00 00 EE 66 EB 00 66 EB 00 .E.f......f..f..
0800:2EB0 B0 02 EE 66 EB 00 66 EB 00 B0 01 EE 66 EB 00 66 ...f..f.....f..f
0800:2EC0 EB 00 88 C8 EE 66 EB 00 66 EB 00 66 C9 66 C3 00 .....f..f..f.f..

fn0800_2ED0()
	push	ebp
	mov	ebp,esp
	mov	ax,cs
	mov	[0000B2A4],ax
	and	eax,0000FFFF
	shl	eax,04
	mov	[0000B2A8],eax
	mov	edx,eax
	neg	edx
	mov	[0000B29C],edx
	leave	
	ret	

fn0800_2F00()
	push	ebp
	mov	ebp,esp
	mov	ecx,[ebp+08]
	cmp	ecx,0A
	jnz	2F1F

l0800_2F11:
	mov	eax,00000200
	mov	edx,0000000D
	int	21

l0800_2F1F:
	mov	eax,00000200
	mov	edx,ecx
	int	21
	leave	
	ret	
0800:2F2E                                           00 00               ..

fn0800_2F30()
	push	ebp
	mov	ebp,esp
	push	ebx
	mov	ebx,[ebp+08]
	cmp	byte ptr [ebx],00
	jz	2F5E

l0800_2F43:
	nop	

l0800_2F44:
	movsx	eax,byte ptr [ebx]
	push	eax
	inc	ebx
	call	2F00
	add	esp,04
	cmp	byte ptr [ebx],00
	jnz	2F44

l0800_2F5E:
	push	0A
	call	2F00
	mov	ebx,[ebp-04]
	leave	
	ret	
0800:2F70 66 55 66 89 E5 66 83 EC 08 FA 66 9C 66 58 66 67 fUf..f....f.fXfg
0800:2F80 A3 FC 91 00 00 66 67 A1 F4 8D 00 00 66 FF D0 66 .....fg.....f..f
0800:2F90 67 0F B7 55 F8 66 67 89 55 F8 66 67 81 4D F8 00 g..U.fg.U.fg.M..
0800:2FA0 00 67 00 66 67 8B 15 A8 B2 00 00 66 81 C2 F4 B0 .g.fg......f....
0800:2FB0 00 00 66 67 89 55 FC 67 0F 01 55 FA 66 0F 20 C0 ..fg.U.g..U.f. .
0800:2FC0 0C 01 66 0F 22 C0 66 EA CE 2F 00 00 18 00 66 BA ..f.".f../....f.
0800:2FD0 10 00 00 00 8E DA 8E C2 66 31 D2 8E E2 8E EA 66 ........f1.....f
0800:2FE0 BA 10 00 00 00 8E D2 66 EA EF 2F 00 00 08 00 83 .......f../.....
0800:2FF0 3D F8 91 00 00 00 74 1B E8 9B 21 00 00 68 F4 A8 =.....t...!..h..
0800:3000 00 00 E8 7D D2 FF FF 83 C4 04 6A 28 6A 20 E8 55 ...}......j(j .U
0800:3010 23 00 00 9C 58 80 E4 B9 80 CC 30 50 EB 00 EB 00 #...X.....0P....
0800:3020 EB 00 9D EB 00 EB 00 EB 00 EA 30 30 00 00 18 00 ..........00....
0800:3030 66 C9 66 C3 66 55 66 89 E5 66 83 EC 08 FA 0F 06 f.f.fUf..f......
0800:3040 66 EA 48 30 00 00 08 00 E8 67 21 00 00 6A 70 6A f.H0.....g!..jpj
0800:3050 08 E8 12 23 00 00 83 C4 08 EA 60 30 00 00 18 00 ...#......`0....
0800:3060 66 BA 20 00 00 00 8E DA 8E C2 8E E2 8E EA 8E D2 f. .............
0800:3070 66 67 0F B7 05 A4 B2 00 00 67 A3 92 30 00 00 66 fg.......g..0..f
0800:3080 0F 20 C0 24 FE 66 0F 22 C0 66 EB 00 66 EA 94 30 . .$.f.".f..f..0
0800:3090 00 00 00 00 67 A1 A4 B2 00 00 8E D8 8E C0 8E E0 ....g...........
0800:30A0 8E E8 8E D0 66 67 81 4D F8 00 00 FF FF 66 67 C7 ....fg.M.....fg.
0800:30B0 45 FC 00 00 00 00 67 0F 01 5D FA 66 67 A1 F8 8D E.....g..].fg...
0800:30C0 00 00 66 FF D0 66 67 8B 15 FC 91 00 00 66 52 66 ..f..fg......fRf
0800:30D0 EB 00 66 EB 00 66 EB 00 66 9D 66 EB 00 66 EB 00 ..f..f..f.f..f..
0800:30E0 66 EB 00 66 C9 66 C3 90 66 55 66 89 E5 66 0F 01 f..f.f..fUf..f..
0800:30F0 E0 A8 01 66 74 10 66 68 A0 87 00 00 66 E8 6E F8 ...ft.fh....f.n.
0800:3100 FF FF 66 83 C4 04 66 E8 1C FB FF FF 66 E8 5E FE ..f...f.....f.^.
0800:3110 FF FF 66 EA 1A 31 00 00 08 00 E8 35 22 00 00 E8 ..f..1.....5"...
0800:3120 A8 20 00 00 68 F4 A8 00 00 E8 FA D0 FF FF 83 C4 . ..h...........
0800:3130 04 C7 05 AC B2 00 00 20 00 00 00 C7 05 B0 B2 00 ....... ........
0800:3140 00 28 00 00 00 E8 86 0E 00 00 C7 05 F8 91 00 00 .(..............
0800:3150 01 00 00 00 EA 5B 31 00 00 18 00 66 E8 D3 FE FF .....[1....f....
0800:3160 FF 66 E8 09 FE FF FF 66 EA 6F 31 00 00 08 00 E8 .f.....f.o1.....
0800:3170 B8 26 00 00 EA 7B 31 00 00 18 00 66 C9 66 C3 00 .&...{1....f.f..
0800:3180 31 C0 8E E0 F7 D0 8E E8 67 64 A1 00 00 00 00 67 1.......gd.....g
0800:3190 65 3B 05 10 00 00 00 66 75 23 89 C2 F7 D0 67 64 e;.....fu#....gd
0800:31A0 A3 00 00 00 00 67 65 3B 05 10 00 00 00 67 64 89 .....ge;.....gd.
0800:31B0 15 00 00 00 00 66 75 05 66 31 C0 66 C3 66 B8 01 .....fu.f1.f.f..
0800:31C0 00 00 00 66 C3 00 00 00 66 9C 66 53 66 56 66 57 ...f....f.fSfVfW
0800:31D0 66 55 FA 66 67 8B 44 24 18 67 88 05 12 32 00 00 fU.fg.D$.g...2..
0800:31E0 66 67 8B 6C 24 1C 66 67 8B 45 1C 66 67 8B 5D 10 fg.l$.fg.E.fg.].
0800:31F0 66 67 8B 4D 18 66 67 8B 55 14 66 67 8B 75 04 66 fg.M.fg.U.fg.u.f
0800:3200 67 8B 7D 00 67 8E 5D 24 67 8E 45 22 66 67 8B 6D g.}.g.]$g.E"fg.m
0800:3210 08 CD 00 66 55 66 67 8B 6C 24 20 66 67 8F 45 08 ...fUfg.l$ fg.E.
0800:3220 66 67 89 45 1C 66 67 89 5D 10 66 67 89 4D 18 66 fg.E.fg.].fg.M.f
0800:3230 67 89 55 14 66 67 89 75 04 66 67 89 7D 00 67 8C g.U.fg.u.fg.}.g.
0800:3240 5D 24 67 8C 45 22 66 9C 66 58 67 89 45 20 8C D0 ]$g.E"f.fXg.E ..
0800:3250 8E D8 8E C0 66 5D 66 5F 66 5E 66 5B 66 9D 66 C3 ....f]f_f^f[f.f.
0800:3260 66 55 66 89 E5 66 53 FA 66 67 C7 05 14 B4 00 00 fUf..fS.fg......
0800:3270 B8 92 00 00 67 80 25 41 B1 00 00 FD 66 BB 48 00 ....g.%A....f.H.
0800:3280 00 00 0F 00 DB 66 9C 66 58 66 89 C1 66 81 C9 00 .....f.fXf..f...
0800:3290 00 02 00 67 8B 1D A4 B2 00 00 66 89 E0 66 6A 00 ...g......f..fj.
0800:32A0 66 6A 00 66 53 66 53 66 53 66 50 66 51 66 53 66 fj.fSfSfSfPfQfSf
0800:32B0 68 B7 32 00 00 66 CF 66 67 8B 5D FC 66 C9 66 C3 h.2..f.fg.].f.f.
0800:32C0 B4 89 CD 15 66 55 66 89 E5 67 80 25 39 B1 00 00 ....fUf..g.%9...
0800:32D0 FD 66 B8 40 00 00 00 66 0F 00 D8 66 C9 66 C3 00 .f.@...f...f.f..

fn0800_32E0()
	push	ebp
	mov	ebp,esp
	push	esi
	cli	
	and	byte ptr [0000B139],FD
	mov	ecx,[0000B2A8]
	add	ecx,0000B2B4
	mov	eax,0000DE0C
	mov	esi,ecx
	mov	edx,esp
	int	67
	mov	esp,edx
	mov	dx,0010
	mov	ss,dx
	mov	ds,dx
	mov	es,dx
	xor	dx,dx
	mov	fs,dx
	mov	gs,dx
	cld	
	mov	esi,[ebp-04]
	leave	
	ret	
0800:3329                            8D 76 00 66 55 66 89          .v.fUf.
0800:3330 E5 66 53 FA 0F 06 66 67 0F B7 0D A4 B2 00 00 66 .fS...fg.......f
0800:3340 B8 0C DE 00 00 66 BB 30 00 00 00 66 89 E2 66 51 .....f.0...f..fQ
0800:3350 66 51 66 51 66 51 66 51 66 52 66 6A 00 66 51 66 fQfQfQfQfRfj.fQf
0800:3360 68 70 33 00 00 8E DB 66 67 36 FF 1D 04 8E 00 00 hp3....fg6......
0800:3370 FC 66 67 8B 5D FC 66 C9 66 C3 8D 36 55 89 E5 56 .fg.].f.f..6U..V
0800:3380 53 8B 75 08 8B 5D 0C EA 8E 33 00 00 18 00 66 E8 S.u..]...3....f.
0800:3390 98 FF FF FF 66 53 66 56 66 E8 2A FE FF FF 66 83 ....fSfVf.*...f.
0800:33A0 C4 08 66 E8 38 FF FF FF 66 EA B0 33 00 00 08 00 ..f.8...f..3....
0800:33B0 8D 65 F8 5B 5E C9 C3 90 55 89 E5 53 8B 5D 08 EA .e.[^...U..S.]..
0800:33C0 C6 33 00 00 18 00 66 E8 60 FF FF FF 66 53 66 E8 .3....f.`...fSf.
0800:33D0 68 F6 FF FF 66 EB FD 90                         h...f...       

fn0800_33D8()
	push	ebp
	mov	ebp,esp
	sub	esp,08
	push	esi
	push	ebx
	mov	ecx,000000FF
	mov	eax,000035FF
	push	es
	int	21
	mov	si,es
	pop	es
	mov	[ebp-02],si
	mov	[ebp-04],bx
	mov	edx,000000FE
	lea	si,[bp+00]

l0800_3408:
	mov	eax,edx
	or	ah,35
	push	es
	int	21
	mov	ax,es
	pop	es
	mov	[ebp-06],ax
	mov	[ebp-08],bx
	cmp	[ebp-04],bx
	jnz	342A

l0800_3423:
	cmp	[ebp-02],ax
	jz	343C

l0800_342A:
	mov	ecx,edx
	mov	eax,[ebp-08]
	mov	[ebp-04],eax
	jmp	3451
0800:343A                               8D 36                       .6   

l0800_343C:
	mov	eax,ecx
	sub	eax,edx
	inc	eax
	cmp	eax,07
	jle	3451

l0800_344B:
	test	dl,07
	jz	345A

l0800_3451:
	dec	edx
	cmp	edx,50
	jnz	3408

l0800_345A:
	mov	eax,edx
	lea	esp,[ebp-10]
	pop	ebx
	pop	esi
	leave	
	ret	
0800:346A                               8D 36                       .6   

fn0800_346C()
	push	ebp
	mov	ebp,esp
	push	esi
	push	ebx
	mov	edx,[ebp+08]
	mov	eax,00003D00
	int	21
	jnc	348B

l0800_3485:
	mov	eax,FFFFFFFF

l0800_348B:
	mov	ebx,eax
	test	ebx,ebx
	jl	34E0

l0800_3494:
	xor	edx,edx
	mov	eax,00004400
	int	21
	jnc	34A8

l0800_34A2:
	mov	edx,FFFFFFFF

l0800_34A8:
	mov	esi,edx
	mov	eax,00004407
	int	21
	movzx	eax,al
	jnc	34C0

l0800_34BA:
	mov	eax,FFFFFFFF

l0800_34C0:
	mov	ecx,eax
	mov	eax,00003E00
	int	21
	test	esi,esi
	jl	34E0

l0800_34D1:
	test	dl,dl
	jge	34E0

l0800_34D6:
	cmp	ecx,000000FF
	jz	34E8

l0800_34E0:
	xor	eax,eax
	jmp	34EE
0800:34E6                   8D 36                               .6       

l0800_34E8:
	mov	eax,00000001

l0800_34EE:
	lea	esp,[ebp-08]
	pop	ebx
	pop	esi
	leave	
	ret	
0800:34FB                                  90                        .   

fn0800_34FC()
	push	ebp
	mov	ebp,esp
	push	ebx
	push	000087DB
	call	346C
	test	eax,eax
	jnz	351C

l0800_3515:
	xor	eax,eax
	jmp	354C
0800:351B                                  90                        .   

l0800_351C:
	mov	eax,00004300
	mov	ebx,00000001
	int	67
	mov	ecx,eax
	mov	[000092C0],dx
	test	ch,FF
	jnz	3546

l0800_353A:
	mov	dword ptr [000092BC],00000001

l0800_3546:
	mov	eax,00000001

l0800_354C:
	mov	ebx,[ebp-04]
	leave	
	ret	
0800:3555                8D 76 00                              .v.       

fn0800_3558()
	push	ebp
	mov	ebp,esp
	sub	esp,28
	push	edi
	push	esi
	push	ebx
	call	34FC
	test	eax,eax
	jnz	35BE

l0800_3573:
	mov	dword ptr [ebp-28],00000000
	push	000087E4
	call	346C
	add	esp,04
	test	eax,eax
	jnz	35A8

l0800_3592:
	push	000087ED
	call	346C
	add	esp,04
	test	eax,eax
	jz	35B1

l0800_35A8:
	mov	dword ptr [ebp-28],00000001

l0800_35B1:
	cmp	dword ptr [ebp-28],00
	jz	3A25

l0800_35BE:
	mov	eax,0000DE00
	int	67
	mov	[ebp-1C],ax
	test	ah,FF
	jnz	3A25

l0800_35D4:
	mov	dword ptr [00008DFC],0000332C
	mov	dword ptr [00008E00],000032E0
	mov	dword ptr [00009090],0000337C
	mov	dword ptr [0000908C],000033B8
	mov	edx,[0000B28C]
	add	edx,00000FFF
	and	edx,FFFFF000
	add	edx,00002000
	mov	[ebp-28],edx
	cmp	dword ptr [0000B28C],00
	jz	3648

l0800_3632:
	mov	eax,edx
	sub	eax,[0000B28C]
	cmp	[0000B290],eax
	jnc	3658

l0800_3648:
	push	000087F6
	call	2970
	add	esp,04

l0800_3658:
	mov	ecx,[ebp-28]
	sub	ecx,[0000B28C]
	sub	[0000B290],ecx
	mov	ebx,[ebp-28]
	mov	[0000B28C],ebx
	add	ebx,FFFFE000
	mov	[000092C4],ebx
	mov	esi,ebx
	add	esi,00001000
	mov	[000092C8],esi
	mov	edi,esi
	or	edi,07
	mov	eax,[000092C4]
	shr	eax,04
	mov	fs,ax
	mov	fs:[00000000],edi
	mov	dword ptr [ebp-04],00000001
	mov	edx,[ebp-04]
	cmp	edx,000007FF
	ja	3703

l0800_36D0:
	mov	ecx,[ebp-04]
	mov	dword ptr fs:[00000000+ecx*4],00000000
	mov	ebx,[ebp-04]
	inc	ebx
	mov	[ebp-04],ebx
	mov	esi,[ebp-04]
	mov	edi,[ebp-04]
	cmp	edi,000007FF
	jbe	36D0

l0800_3703:
	mov	eax,[000092C8]
	shr	eax,04
	mov	[ebp-0C],eax
	mov	eax,0000DE01
	mov	bx,[ebp-0C]
	xor	edi,edi
	mov	esi,0000B144
	push	es
	mov	es,bx
	int	67
	pop	es
	mov	[00008E04],ebx
	mov	[ebp-24],di
	mov	eax,0000DE0A
	int	67
	mov	[0000B2AC],ebx
	mov	[0000B2B0],ecx
	movzx	esi,word ptr [0000B2AC]
	mov	[0000B2AC],esi
	movzx	edi,word ptr [0000B2B0]
	mov	[0000B2B0],edi
	cmp	dword ptr [0000B2AC],08
	jnz	3862

l0800_3782:
	mov	dword ptr [000092DC],00000001
	cli	
	call	33D8
	mov	[0000B2AC],eax
	mov	dword ptr [ebp-20],00000000
	lea	si,[bp+00]

l0800_37A8:
	mov	eax,[ebp-20]
	add	eax,[0000B2AC]
	mov	[ebp-14],eax
	mov	edx,[ebp-20]
	shl	edx,02
	mov	[ebp-10],edx
	add	edx,000092E0
	or	ah,35
	mov	[ebp-18],eax
	push	es
	int	21
	mov	si,es
	pop	es
	mov	[edx+02],si
	mov	edi,[ebp-10]
	mov	[edi+000092E0],bx
	mov	eax,[ebp-20]
	add	eax,08
	or	ah,35
	mov	[ebp-18],eax
	push	es
	int	21
	mov	si,es
	pop	es
	mov	[ebp-06],si
	mov	[ebp-08],bx
	mov	edi,[ebp-14]
	or	edi,00002500
	mov	eax,edi
	mov	cx,[ebp-06]
	mov	edx,ebx
	push	ds
	mov	ds,cx
	int	21
	pop	ds
	inc	dword ptr [ebp-20]
	cmp	dword ptr [ebp-20],07
	jle	37A8

l0800_383A:
	push	dword ptr [0000B2AC]
	call	2E28
	mov	bx,[0000B2AC]
	mov	cx,[0000B2B0]
	add	esp,04
	mov	eax,0000DE0B
	int	67

l0800_3862:
	mov	edx,[000092C4]
	mov	[0000B2B4],edx
	mov	ecx,[0000B2A8]
	add	ecx,000092CE
	mov	[0000B2B8],ecx
	mov	ebx,[0000B2A8]
	add	ebx,000092D6
	mov	[0000B2BC],ebx
	mov	word ptr [0000B2C2],0040
	mov	esi,0000330D
	mov	[ebp-0C],si
	movzx	edi,si
	mov	[0000B2C4],edi
	mov	word ptr [0000B2C8],0018
	mov	word ptr [000092CE],0067
	mov	eax,[0000B2A8]
	add	eax,0000B0F4
	mov	[000092D0],eax
	mov	word ptr [000092D6],07FF
	mov	edx,[0000B2A8]
	add	edx,0000A8F4
	mov	[000092D8],edx
	call	2C28
	mov	ecx,[0000B2A8]
	add	ecx,0000B18C
	mov	[ebp-14],ecx
	mov	word ptr [0000B134],0067
	mov	bx,[ebp-14]
	mov	[0000B136],bx
	shr	ecx,10
	mov	[ebp-0C],ecx
	mov	al,[ebp-0C]
	mov	[0000B138],al
	mov	byte ptr [0000B139],89
	mov	edx,00000067
	shr	edx,10
	mov	[ebp-10],edx
	mov	cl,[ebp-10]
	and	cl,0F
	and	byte ptr [0000B13A],F0
	or	[0000B13A],cl
	mov	byte ptr [ebp-0C],00
	and	byte ptr [0000B13A],0F
	mov	bl,[ebp-0C]
	or	[0000B13A],bl
	mov	esi,[ebp-14]
	shr	esi,18
	mov	[ebp-14],esi
	mov	al,[ebp-14]
	mov	[0000B13B],al
	mov	word ptr [0000B1F2],0068
	call	32E0
	jmp	far 0000:39BC
0800:39BA                               08 00 E8 EB EF FF           ......
0800:39C0 FF 68 F4 A8 00 00 E8 5D C8 FF FF 83 C4 04 66 8B .h.....]......f.
0800:39D0 55 DC 66 C1 EA 02 0F B7 CA C1 E1 0C 51 FF 35 C4 U.f.........Q.5.
0800:39E0 92 00 00 E8 44 18 00 00 83 C4 08 E8 64 19 00 00 ....D.......d...
0800:39F0 E8 DB 05 00 00 66 C7 05 C0 B2 00 00 38 00 EA 05 .....f......8...
0800:3A00 3A 00 00 18 00 66 E8 21 F9 FF FF 66 E8 CF F8 FF :....f.!...f....
0800:3A10 FF 66 EA 19 3A 00 00 08 00 E8 FA 1D 00 00 EA 25 .f..:..........%
0800:3A20 3A 00 00 18 00                                  :....          

l0800_3A25:
	lea	esp,[ebp-34]
	pop	ebx
	pop	esi
	pop	edi
	leave	
	ret	

fn0800_3A34()
	push	ebp
	mov	ebp,esp
	sub	esp,04
	push	esi
	push	ebx
	cmp	dword ptr [000092DC],00
	jz	3AB0

l0800_3A4D:
	mov	dword ptr [000092DC],00000000
	cli	
	push	08
	call	2E28
	mov	cx,[0000B2B0]
	mov	eax,0000DE0B
	mov	ebx,00000008
	int	67
	xor	esi,esi
	nop	
	mov	eax,esi
	add	eax,[0000B2AC]
	mov	ecx,eax
	or	ch,25
	mov	eax,ecx
	mov	bx,[000092E2+esi*4]
	mov	dx,[000092E0+esi*4]
	push	ds
	mov	ds,bx
	int	21
0800:3AA5                1F 66 46 66 83 FE 07 66 7E CD FB      .fFf...f~..

l0800_3AB0:
	cmp	dword ptr [000092BC],00
	jz	3AD7

l0800_3ABC:
	mov	dword ptr [000092BC],00000000
	mov	eax,00004500
	mov	dx,[000092C0]
	int	67

l0800_3AD7:
	lea	esp,[ebp-0C]
	pop	ebx
	pop	esi
	leave	
	ret	
0800:3AE4             66 55 66 89 E5 67 8A 45 08 24 0F 3C     fUf..g.E.$.<
0800:3AF0 09 66 77 10 66 25 FF 00 00 00 66 83 C0 30 66 EB .fw.f%....f..0f.
0800:3B00 0D 8D 76 00 66 25 FF 00 00 00 66 83 C0 37 66 50 ..v.f%....f..7fP
0800:3B10 66 E8 EA F3 FF FF 66 C9 66 C3 8D 36 66 55 66 89 f.....f.f..6fUf.
0800:3B20 E5 66 53 67 8A 5D 08 88 D8 C0 E8 04 66 25 FF 00 .fSg.]......f%..
0800:3B30 00 00 66 50 66 E8 AA FF FF FF 66 0F B6 C3 66 50 ..fPf.....f...fP
0800:3B40 66 E8 9E FF FF FF 66 67 8B 5D FC 66 C9 66 C3 90 f.....fg.].f.f..
0800:3B50 66 55 66 89 E5 66 53 67 8B 5D 08 66 67 0F B6 45 fUf..fSg.].fg..E
0800:3B60 09 66 50 66 E8 B3 FF FF FF 66 0F B6 C3 66 50 66 .fPf.....f...fPf
0800:3B70 E8 A7 FF FF FF 66 67 8B 5D FC 66 C9 66 C3 8D 36 .....fg.].f.f..6
0800:3B80 66 55 66 89 E5 66 53 66 67 8B 5D 08 66 89 D8 66 fUf..fSfg.].f..f
0800:3B90 C1 E8 10 66 50 66 E8 B5 FF FF FF 66 0F B7 DB 66 ...fPf.....f...f
0800:3BA0 53 66 E8 A9 FF FF FF 66 67 8B 5D FC 66 C9 66 C3 Sf.....fg.].f.f.
0800:3BB0 66 55 66 89 E5 66 56 66 53 66 67 8B 5D 08 66 67 fUf..fVfSfg.].fg
0800:3BC0 8B 75 0C 66 89 DA 66 89 F1 66 89 CA 66 31 C9 66 .u.f..f..f..f1.f
0800:3BD0 52 66 E8 A9 FF FF FF 66 53 66 E8 A1 FF FF FF 66 Rf.....fSf.....f
0800:3BE0 67 8D 65 F8 66 5B 66 5E 66 C9 66 C3 55 89 E5 83 g.e.f[f^f.f.U...
0800:3BF0 3D 00 93 00 00 00 74 1B FF 35 04 93 00 00 FF 35 =.....t..5.....5
0800:3C00 00 93 00 00 E8 AF 16 00 00 C7 05 00 93 00 00 00 ................
0800:3C10 00 00 00 C9 C3 8D 76 00 66 55 66 89 E5 66 53 66 ......v.fUf..fSf
0800:3C20 B8 00 05 00 00 67 FF 1D CC B2 00 00 66 89 C2 85 .....g......f...
0800:3C30 D2 66 75 0C 66 68 1A 88 00 00 66 E8 30 ED FF FF .fu.fh....f.0...
0800:3C40 66 67 8B 5D FC 66 C9 66 C3 8D 76 00 66 55 66 89 fg.].f.f..v.fUf.
0800:3C50 E5 66 53 66 B8 00 06 00 00 67 FF 1D CC B2 00 00 .fSf.....g......
0800:3C60 66 89 C2 85 D2 66 75 0C 66 68 3B 88 00 00 66 E8 f....fu.fh;...f.
0800:3C70 FC EC FF FF 66 67 8B 5D FC 66 C9 66 C3 8D 76 00 ....fg.].f.f..v.

fn0800_3C80()
	push	ebp
	mov	ebp,esp
	sub	esp,04
	push	ebx
	mov	eax,00004300
	int	2F
	mov	ecx,eax
	cmp	cl,80
	jnz	3D99

l0800_3CA0:
	mov	eax,00004310
	push	ds
	push	es
	int	2F
	mov	ax,es
	pop	es
	pop	ds
	mov	[0000B2CE],ax
	mov	[0000B2CC],bx
	mov	eax,00000800
	call	dword ptr [0000B2CC]
	mov	ecx,eax
	movzx	edx,cx
	mov	ebx,edx
	shl	ebx,0A
	jz	3D99
	mov	[00009304],ebx
	mov	eax,00000900
	mov	edx,ecx
	call	dword ptr [0000B2CC]
	mov	ecx,eax
	mov	[00009308],dx
	test	cx,cx
	jnz	3D18
	push	0000885D
	call	2970
	add	esp,04
	mov	byte ptr [0000930A],01
	mov	eax,00000C00
	mov	dx,[00009308]
	call	dword ptr [0000B2CC]
	shl	edx,10
	mov	dx,bx
	mov	ecx,eax
	mov	[00009300],edx
	test	cx,cx
	jnz	3D56
	push	00008887
	call	2970
	mov	byte ptr [0000930B],01
	mov	ecx,[00009300]
	add	ecx,[00009304]
	cmp	[0000B2A0],ecx
	jnc	3D81
	mov	[0000B2A0],ecx
	mov	dword ptr [00008DF4],00003C18
	mov	dword ptr [00008DF8],00003C4C

l0800_3D99:
	mov	ebx,[ebp-08]
	leave	
	ret	
0800:3DA2       8D 36                                       .6           

fn0800_3DA4()
	push	ebp
	mov	ebp,esp
	push	ebx
	cmp	byte ptr [0000930B],00
	jz	3DEA

l0800_3DB6:
	mov	eax,00000D00
	mov	dx,[00009308]
	call	dword ptr [0000B2CC]
	mov	ecx,eax
	mov	byte ptr [0000930B],00
	test	cx,cx
	jnz	3DEA
	push	000088B2
	call	2970
	add	esp,04

l0800_3DEA:
	cmp	byte ptr [0000930A],00
	jz	3E25

l0800_3DF5:
	mov	eax,00000A00
	mov	dx,[00009308]
	call	dword ptr [0000B2CC]
	mov	ecx,eax
	mov	byte ptr [0000930A],00
	test	cx,cx
	jnz	3E25
	push	000088DA
	call	2970

l0800_3E25:
	mov	ebx,[ebp-04]
	leave	
	ret	
0800:3E2E                                           00 00               ..
0800:3E30 55 89 E5 56 53 8B 75 08 BB 0C 8E 00 00 83 3D 0C U..VS.u.......=.
0800:3E40 8E 00 00 00 74 30 8D 36 0F B7 43 04 8D 04 C6 8B ....t0.6..C.....
0800:3E50 13 8A 4B 06 66 89 10 66 C7 40 02 08 00 C6 40 04 ..K.f..f.@....@.
0800:3E60 00 80 C9 80 88 48 05 C1 EA 10 66 89 50 06 83 C3 .....H....f.P...
0800:3E70 08 83 3B 00 75 D2 8D 65 F8 5B 5E C9 C3 00 00 00 ..;.u..e.[^.....
0800:3E80 6A 00 6A 00 E9 33 05 00 00 8D 76 00 6A 00 6A 01 j.j..3....v.j.j.
0800:3E90 E9 27 05 00 00 8D 76 00 6A 00 6A 03 E9 1B 05 00 .'....v.j.j.....
0800:3EA0 00 8D 76 00 6A 00 6A 04 E9 0F 05 00 00 8D 76 00 ..v.j.j.......v.
0800:3EB0 6A 00 6A 05 E9 03 05 00 00 8D 76 00 6A 00 6A 06 j.j.......v.j.j.
0800:3EC0 E9 F7 04 00 00 8D 76 00 6A 00 6A 07 E9 EB 04 00 ......v.j.j.....
0800:3ED0 00 8D 76 00 6A 00 6A 08 E9 DF 04 00 00 8D 76 00 ..v.j.j.......v.
0800:3EE0 6A 00 6A 09 E9 D3 04 00 00 8D 76 00 6A 0A E9 C9 j.j.......v.j...
0800:3EF0 04 00 00 90 6A 0B E9 C1 04 00 00 90 6A 0C E9 B9 ....j.......j...
0800:3F00 04 00 00 90 6A 00 6A 0F E9 AF 04 00 00 8D 76 00 ....j.j.......v.
0800:3F10 6A 00 6A 10 E9 A3 04 00 00 8D 76 00 6A 00 6A 11 j.j.......v.j.j.
0800:3F20 E9 97 04 00 00 8D 76 00 6A 00 6A 12 E9 8B 04 00 ......v.j.j.....
0800:3F30 00 8D 76 00 6A 00 6A 13 E9 7F 04 00 00 8D 76 00 ..v.j.j.......v.
0800:3F40 6A 00 6A 14 E9 73 04 00 00 8D 76 00 6A 00 6A 15 j.j..s....v.j.j.
0800:3F50 E9 67 04 00 00 8D 76 00 6A 00 6A 16 E9 5B 04 00 .g....v.j.j..[..
0800:3F60 00 8D 76 00 6A 00 6A 17 E9 4F 04 00 00 8D 76 00 ..v.j.j..O....v.
0800:3F70 6A 00 6A 18 E9 43 04 00 00 8D 76 00 6A 00 6A 19 j.j..C....v.j.j.
0800:3F80 E9 37 04 00 00 8D 76 00 6A 00 6A 1A E9 2B 04 00 .7....v.j.j..+..
0800:3F90 00 8D 76 00 6A 00 6A 1B E9 1F 04 00 00 8D 76 00 ..v.j.j.......v.
0800:3FA0 6A 00 6A 1C E9 13 04 00 00 8D 76 00 6A 00 6A 1D j.j.......v.j.j.
0800:3FB0 E9 07 04 00 00 8D 76 00 6A 00 6A 1E E9 FB 03 00 ......v.j.j.....
0800:3FC0 00 8D 76 00 6A 00 6A 1F E9 EF 03 00 00 00 00 00 ..v.j.j.........
0800:3FD0 55 89 E5 53 31 DB 8D 36 89 D8 03 05 AC B2 00 00 U..S1..6........
0800:3FE0 C1 E0 03 8D 88 F4 A8 00 00 8B 14 9D 08 8F 00 00 ................
0800:3FF0 66 89 90 F4 A8 00 00 66 C7 41 02 08 00 C6 41 04 f......f.A....A.
0800:4000 00 C6 41 05 8E C1 EA 10 66 89 51 06 43 83 FB 07 ..A.....f.Q.C...
0800:4010 7E C6 31 DB 89 D8 03 05 B0 B2 00 00 C1 E0 03 8D ~.1.............
0800:4020 88 F4 A8 00 00 8B 14 9D 28 8F 00 00 66 89 90 F4 ........(...f...
0800:4030 A8 00 00 66 C7 41 02 08 00 C6 41 04 00 C6 41 05 ...f.A....A...A.
0800:4040 8E C1 EA 10 66 89 51 06 43 83 FB 07 7E C6 8B 5D ....f.Q.C...~..]
0800:4050 FC C9 C3 00 55 89 E5 8B 45 08 BA 20 4F 00 00 66 ....U...E.. O..f
0800:4060 89 90 68 08 00 00 66 C7 80 6A 08 00 00 08 00 C6 ..h...f..j......
0800:4070 80 6C 08 00 00 00 C6 80 6D 08 00 00 EC C1 EA 10 .l......m.......
0800:4080 66 89 90 6E 08 00 00 66 C7 80 78 08 00 00 FF FF f..n...f..x.....
0800:4090 66 C7 80 7A 08 00 00 00 00 C6 80 7C 08 00 00 00 f..z.......|....
0800:40A0 C6 80 7D 08 00 00 FA 80 A0 7E 08 00 00 F0 80 88 ..}......~......
0800:40B0 7E 08 00 00 07 80 A0 7E 08 00 00 0F 80 88 7E 08 ~......~......~.
0800:40C0 00 00 C0 C6 80 7F 08 00 00 40 66 C7 80 80 08 00 .........@f.....
0800:40D0 00 FF FF 66 C7 80 82 08 00 00 00 00 C6 80 84 08 ...f............
0800:40E0 00 00 00 C6 80 85 08 00 00 F2 80 A0 86 08 00 00 ................
0800:40F0 F0 80 88 86 08 00 00 07 80 A0 86 08 00 00 0F 80 ................
0800:4100 88 86 08 00 00 C0 C6 80 87 08 00 00 40 66 C7 80 ............@f..
0800:4110 88 08 00 00 FF FF 66 C7 80 8A 08 00 00 00 00 C6 ......f.........
0800:4120 80 8C 08 00 00 00 C6 80 8D 08 00 00 9A 80 A0 8E ................
0800:4130 08 00 00 F0 80 88 8E 08 00 00 07 80 A0 8E 08 00 ................
0800:4140 00 0F 80 88 8E 08 00 00 C0 C6 80 8F 08 00 00 40 ...............@
0800:4150 66 C7 80 90 08 00 00 FF FF 66 C7 80 92 08 00 00 f........f......
0800:4160 00 00 C6 80 94 08 00 00 00 C6 80 95 08 00 00 92 ................
0800:4170 80 A0 96 08 00 00 F0 80 88 96 08 00 00 07 80 A0 ................
0800:4180 96 08 00 00 0F 80 88 96 08 00 00 C0 C6 80 97 08 ................
0800:4190 00 00 40 C9 C3 00 00 00 6A 0E 60 0F 20 D0 89 44 ..@.....j.`. ..D
0800:41A0 24 0C 1E 06 0F A0 0F A8 66 8C D2 66 8E DA 66 8E $.......f..f..f.
0800:41B0 C2 6A 00 50 E8 87 1C 00 00 83 C4 08 09 C0 0F 84 .j.P............
0800:41C0 2F 02 00 00 A1 D0 B2 00 00 09 C0 74 09 89 44 24 /..........t..D$
0800:41D0 38 E9 1D 02 00 00 F6 44 24 3C 03 0F 85 04 02 00 8......D$<......
0800:41E0 00 66 8C D0 66 8E D8 66 8E C0 89 E0 50 E8 1A 2B .f..f..f....P..+
0800:41F0 00 00 8D 36 6A 0D 60 1E 06 0F A0 0F A8 F7 44 24 ...6j.`.......D$
0800:4200 40 00 00 02 00 0F 85 B7 00 00 00 F6 44 24 3C 03 @...........D$<.
0800:4210 74 B2 8B 54 24 38 8A 02 3C CD 75 A8 83 44 24 38 t..T$8..<.u..D$8
0800:4220 02 8A 42 01 36 88 05 6E 42 00 00 66 8C D0 66 8E ..B.6..nB..f..f.
0800:4230 D8 66 8E C0 EA 3B 42 00 00 18 00 66 67 FF 15 FC .f...;B....fg...
0800:4240 8D 00 00 66 67 8B 44 24 2C 66 67 8B 5C 24 20 66 ...fg.D$,fg.\$ f
0800:4250 67 8B 4C 24 28 66 67 8B 54 24 24 66 67 8B 74 24 g.L$(fg.T$$fg.t$
0800:4260 14 66 67 8B 7C 24 10 66 67 8B 6C 24 18 CD 00 66 .fg.|$.fg.l$...f
0800:4270 67 89 44 24 2C 66 67 89 5C 24 20 66 67 89 4C 24 g.D$,fg.\$ fg.L$
0800:4280 28 66 67 89 54 24 24 66 67 89 74 24 14 66 67 89 (fg.T$$fg.t$.fg.
0800:4290 7C 24 10 66 67 89 6C 24 18 66 9C 66 58 66 67 8B |$.fg.l$.f.fXfg.
0800:42A0 54 24 40 66 81 E2 00 02 00 00 66 09 D0 67 89 44 T$@f......f..g.D
0800:42B0 24 40 66 67 FF 15 00 8E 00 00 66 EA F3 43 00 00 $@fg......f..C..
0800:42C0 08 00 B8 30 00 00 00 66 8E D8 0F B7 5C 24 3C C1 ...0...f....\$<.
0800:42D0 E3 04 03 5C 24 38 8A 03 3C CD 74 11 66 8C D0 66 ...\$8..<.t.f..f
0800:42E0 8E D8 66 8E C0 89 E0 50 E8 1F 2A 00 00 66 83 44 ..f....P..*..f.D
0800:42F0 24 38 02 0F B6 73 01 83 FE 15 75 0E 80 7C 24 2D $8...s....u..|$-
0800:4300 87 74 6E 80 7C 24 2D 89 74 4B C1 E6 02 66 83 6C .tn.|$-.tK...f.l
0800:4310 24 44 06 0F B7 5C 24 48 C1 E3 04 03 5C 24 44 8B $D...\$H....\$D.
0800:4320 44 24 38 66 89 03 8B 44 24 3C 66 89 43 02 8B 44 D$8f...D$<f.C..D
0800:4330 24 40 66 89 43 04 8B 06 66 89 44 24 38 C1 E8 10 $@f.C...f.D$8...
0800:4340 66 89 44 24 3C 81 64 24 40 FF FC FF FF 83 C4 10 f.D$<.d$@.......
0800:4350 61 83 C4 08 CF 8B 5C 24 20 8B 74 24 14 66 8C D0 a.....\$ .t$.f..
0800:4360 66 8E D8 66 8E C0 8B 64 24 44 EA C4 32 00 00 18 f..f...d$D..2...
0800:4370 00 0F B7 5C 24 4C 0F B7 44 24 14 C1 E3 04 01 C3 ...\$L..D$......
0800:4380 8B 73 12 81 E6 FF FF FF 00 8B 7B 1A 81 E7 FF FF .s........{.....
0800:4390 FF 00 0F B7 4C 24 28 01 C9 66 8C D8 66 8E C0 FC ....L$(..f..f...
0800:43A0 51 57 56 E8 C4 32 00 00 83 C4 0C 83 64 24 40 FE QWV..2......d$@.
0800:43B0 83 C4 10 61 30 E4 83 C4 08 CF 8D 36 60 1E 06 0F ...a0......6`...
0800:43C0 A0 0F A8 F7 44 24 40 00 00 02 00 75 18 F6 44 24 ....D$@....u..D$
0800:43D0 3C 03 75 11 66 8C D0 66 8E D8 66 8E C0 89 E0 50 <.u.f..f..f....P
0800:43E0 E8 27 29 00 00 66 8C D0 66 8E D8 66 8E C0 E8 ED .')..f..f..f....
0800:43F0 1E 00 00 F7 44 24 40 00 00 02 00 75 15 F6 44 24 ....D$@....u..D$
0800:4400 3C 03 74 0E 8B 15 FC D5 00 00 09 D2 0F 85 7F 0B <.t.............
0800:4410 00 00 0F A9 0F A1 07 1F 61 83 C4 08 CF 6A 08 6A ........a....j.j
0800:4420 20 EB 5D 6A 09 6A 21 EB 57 6A 0A 6A 22 EB 51 6A  .]j.j!.Wj.j".Qj
0800:4430 0B 6A 23 EB 4B 6A 0C 6A 24 EB 45 6A 0D 6A 25 EB .j#.Kj.j$.Ej.j%.
0800:4440 3F 6A 0E 6A 26 EB 39 6A 0F 6A 27 EB 33 6A 70 6A ?j.j&.9j.j'.3jpj
0800:4450 28 EB 2D 6A 71 6A 29 EB 27 6A 72 6A 2A EB 21 6A (.-jqj).'jrj*.!j
0800:4460 73 6A 2B EB 1B 6A 74 6A 2C EB 15 6A 75 6A 2D EB sj+..jtj,..juj-.
0800:4470 0F 6A 76 6A 2E EB 09 6A 77 6A 2F EB 03 8D 76 00 .jvj...jwj/...v.
0800:4480 60 1E 06 0F A0 0F A8 F7 44 24 40 00 00 02 00 75 `.......D$@....u
0800:4490 34 66 8C D0 66 8E D8 66 8E C0 8B 44 24 34 88 05 4f..f..f...D$4..
0800:44A0 B4 44 00 00 EA AB 44 00 00 18 00 66 67 FF 15 FC .D....D....fg...
0800:44B0 8D 00 00 CD 00 66 67 FF 15 00 8E 00 00 66 EA F3 .....fg......f..
0800:44C0 43 00 00 08 00 B8 30 00 00 00 66 8E D8 8B 74 24 C.....0...f...t$
0800:44D0 34 E9 34 FE FF FF 8D 36 6A 00 6A 20 60 8D 1D 7C 4.4....6j.j `..|
0800:44E0 D4 00 00 E4 21 EB 00 EB 00 0C 01 E6 21 EB 00 EB ....!.......!...
0800:44F0 00 B0 20 E6 20 E9 FA 05 00 00 8D 36 66 50 66 53 .. . ......6fPfS
0800:4500 66 51 66 67 8D 1D 7C D4 00 00 E4 21 66 EB 00 66 fQfg..|....!f..f
0800:4510 EB 00 0C 01 E6 21 66 EB 00 66 EB 00 B0 20 E6 20 .....!f..f... . 
0800:4520 66 E9 FE 05 00 00 8D 36 E4 21 EB 00 EB 00 24 FE f......6.!....$.
0800:4530 E6 21 C3 90 6A 00 6A 21 60 8D 1D 94 D4 00 00 E4 .!..j.j!`.......
0800:4540 21 EB 00 EB 00 0C 02 E6 21 EB 00 EB 00 B0 20 E6 !.......!..... .
0800:4550 20 E9 9E 05 00 00 8D 36 66 50 66 53 66 51 66 67  ......6fPfSfQfg
0800:4560 8D 1D 94 D4 00 00 E4 21 66 EB 00 66 EB 00 0C 02 .......!f..f....
0800:4570 E6 21 66 EB 00 66 EB 00 B0 20 E6 20 66 E9 A2 05 .!f..f... . f...
0800:4580 00 00 8D 36 E4 21 EB 00 EB 00 24 FD E6 21 C3 90 ...6.!....$..!..
0800:4590 6A 00 6A 22 60 8D 1D AC D4 00 00 E4 21 EB 00 EB j.j"`.......!...
0800:45A0 00 0C 04 E6 21 EB 00 EB 00 B0 20 E6 20 E9 42 05 ....!..... . .B.
0800:45B0 00 00 8D 36 66 50 66 53 66 51 66 67 8D 1D AC D4 ...6fPfSfQfg....
0800:45C0 00 00 E4 21 66 EB 00 66 EB 00 0C 04 E6 21 66 EB ...!f..f.....!f.
0800:45D0 00 66 EB 00 B0 20 E6 20 66 E9 46 05 00 00 8D 36 .f... . f.F....6
0800:45E0 E4 21 EB 00 EB 00 24 FB E6 21 C3 90 6A 00 6A 23 .!....$..!..j.j#
0800:45F0 60 8D 1D C4 D4 00 00 E4 21 EB 00 EB 00 0C 08 E6 `.......!.......
0800:4600 21 EB 00 EB 00 B0 20 E6 20 E9 E6 04 00 00 8D 36 !..... . ......6
0800:4610 66 50 66 53 66 51 66 67 8D 1D C4 D4 00 00 E4 21 fPfSfQfg.......!
0800:4620 66 EB 00 66 EB 00 0C 08 E6 21 66 EB 00 66 EB 00 f..f.....!f..f..
0800:4630 B0 20 E6 20 66 E9 EA 04 00 00 8D 36 E4 21 EB 00 . . f......6.!..
0800:4640 EB 00 24 F7 E6 21 C3 90 6A 00 6A 24 60 8D 1D DC ..$..!..j.j$`...
0800:4650 D4 00 00 E4 21 EB 00 EB 00 0C 10 E6 21 EB 00 EB ....!.......!...
0800:4660 00 B0 20 E6 20 E9 8A 04 00 00 8D 36 66 50 66 53 .. . ......6fPfS
0800:4670 66 51 66 67 8D 1D DC D4 00 00 E4 21 66 EB 00 66 fQfg.......!f..f
0800:4680 EB 00 0C 10 E6 21 66 EB 00 66 EB 00 B0 20 E6 20 .....!f..f... . 
0800:4690 66 E9 8E 04 00 00 8D 36 E4 21 EB 00 EB 00 24 EF f......6.!....$.
0800:46A0 E6 21 C3 90 6A 00 6A 25 60 8D 1D F4 D4 00 00 E4 .!..j.j%`.......
0800:46B0 21 EB 00 EB 00 0C 20 E6 21 EB 00 EB 00 B0 20 E6 !..... .!..... .
0800:46C0 20 E9 2E 04 00 00 8D 36 66 50 66 53 66 51 66 67  ......6fPfSfQfg
0800:46D0 8D 1D F4 D4 00 00 E4 21 66 EB 00 66 EB 00 0C 20 .......!f..f... 
0800:46E0 E6 21 66 EB 00 66 EB 00 B0 20 E6 20 66 E9 32 04 .!f..f... . f.2.
0800:46F0 00 00 8D 36 E4 21 EB 00 EB 00 24 DF E6 21 C3 90 ...6.!....$..!..
0800:4700 6A 00 6A 26 60 8D 1D 0C D5 00 00 E4 21 EB 00 EB j.j&`.......!...
0800:4710 00 0C 40 E6 21 EB 00 EB 00 B0 20 E6 20 E9 D2 03 ..@.!..... . ...
0800:4720 00 00 8D 36 66 50 66 53 66 51 66 67 8D 1D 0C D5 ...6fPfSfQfg....
0800:4730 00 00 E4 21 66 EB 00 66 EB 00 0C 40 E6 21 66 EB ...!f..f...@.!f.
0800:4740 00 66 EB 00 B0 20 E6 20 66 E9 D6 03 00 00 8D 36 .f... . f......6
0800:4750 E4 21 EB 00 EB 00 24 BF E6 21 C3 90 6A 00 6A 27 .!....$..!..j.j'
0800:4760 60 8D 1D 24 D5 00 00 E4 21 EB 00 EB 00 0C 80 E6 `..$....!.......
0800:4770 21 EB 00 EB 00 B0 20 E6 20 E9 76 03 00 00 8D 36 !..... . .v....6
0800:4780 66 50 66 53 66 51 66 67 8D 1D 24 D5 00 00 E4 21 fPfSfQfg..$....!
0800:4790 66 EB 00 66 EB 00 0C 80 E6 21 66 EB 00 66 EB 00 f..f.....!f..f..
0800:47A0 B0 20 E6 20 66 E9 7A 03 00 00 8D 36 E4 21 EB 00 . . f.z....6.!..
0800:47B0 EB 00 24 7F E6 21 C3 90 6A 00 6A 28 60 8D 1D 3C ..$..!..j.j(`..<
0800:47C0 D5 00 00 E4 A1 EB 00 EB 00 0C 01 E6 A1 EB 00 EB ................
0800:47D0 00 B0 20 E6 A0 EB 00 EB 00 E6 20 E9 14 03 00 00 .. ....... .....
0800:47E0 66 50 66 53 66 51 66 67 8D 1D 3C D5 00 00 E4 A1 fPfSfQfg..<.....
0800:47F0 66 EB 00 66 EB 00 0C 01 E6 A1 66 EB 00 66 EB 00 f..f......f..f..
0800:4800 B0 20 E6 A0 66 EB 00 66 EB 00 E6 20 66 E9 12 03 . ..f..f... f...
0800:4810 00 00 8D 36 E4 A1 EB 00 EB 00 24 FE E6 A1 C3 90 ...6......$.....
0800:4820 6A 00 6A 29 60 8D 1D 54 D5 00 00 E4 A1 EB 00 EB j.j)`..T........
0800:4830 00 0C 02 E6 A1 EB 00 EB 00 B0 20 E6 A0 EB 00 EB .......... .....
0800:4840 00 E6 20 E9 AC 02 00 00 66 50 66 53 66 51 66 67 .. .....fPfSfQfg
0800:4850 8D 1D 54 D5 00 00 E4 A1 66 EB 00 66 EB 00 0C 02 ..T.....f..f....
0800:4860 E6 A1 66 EB 00 66 EB 00 B0 20 E6 A0 66 EB 00 66 ..f..f... ..f..f
0800:4870 EB 00 E6 20 66 E9 AA 02 00 00 8D 36 E4 A1 EB 00 ... f......6....
0800:4880 EB 00 24 FD E6 A1 C3 90 6A 00 6A 2A 60 8D 1D 6C ..$.....j.j*`..l
0800:4890 D5 00 00 E4 A1 EB 00 EB 00 0C 04 E6 A1 EB 00 EB ................
0800:48A0 00 B0 20 E6 A0 EB 00 EB 00 E6 20 E9 44 02 00 00 .. ....... .D...
0800:48B0 66 50 66 53 66 51 66 67 8D 1D 6C D5 00 00 E4 A1 fPfSfQfg..l.....
0800:48C0 66 EB 00 66 EB 00 0C 04 E6 A1 66 EB 00 66 EB 00 f..f......f..f..
0800:48D0 B0 20 E6 A0 66 EB 00 66 EB 00 E6 20 66 E9 42 02 . ..f..f... f.B.
0800:48E0 00 00 8D 36 E4 A1 EB 00 EB 00 24 FB E6 A1 C3 90 ...6......$.....
0800:48F0 6A 00 6A 2B 60 8D 1D 84 D5 00 00 E4 A1 EB 00 EB j.j+`...........
0800:4900 00 0C 08 E6 A1 EB 00 EB 00 B0 20 E6 A0 EB 00 EB .......... .....
0800:4910 00 E6 20 E9 DC 01 00 00 66 50 66 53 66 51 66 67 .. .....fPfSfQfg
0800:4920 8D 1D 84 D5 00 00 E4 A1 66 EB 00 66 EB 00 0C 08 ........f..f....
0800:4930 E6 A1 66 EB 00 66 EB 00 B0 20 E6 A0 66 EB 00 66 ..f..f... ..f..f
0800:4940 EB 00 E6 20 66 E9 DA 01 00 00 8D 36 E4 A1 EB 00 ... f......6....
0800:4950 EB 00 24 F7 E6 A1 C3 90 6A 00 6A 2C 60 8D 1D 9C ..$.....j.j,`...
0800:4960 D5 00 00 E4 A1 EB 00 EB 00 0C 10 E6 A1 EB 00 EB ................
0800:4970 00 B0 20 E6 A0 EB 00 EB 00 E6 20 E9 74 01 00 00 .. ....... .t...
0800:4980 66 50 66 53 66 51 66 67 8D 1D 9C D5 00 00 E4 A1 fPfSfQfg........
0800:4990 66 EB 00 66 EB 00 0C 10 E6 A1 66 EB 00 66 EB 00 f..f......f..f..
0800:49A0 B0 20 E6 A0 66 EB 00 66 EB 00 E6 20 66 E9 72 01 . ..f..f... f.r.
0800:49B0 00 00 8D 36 E4 A1 EB 00 EB 00 24 EF E6 A1 C3 90 ...6......$.....
0800:49C0 6A 00 6A 2D 60 8D 1D B4 D5 00 00 E4 A1 EB 00 EB j.j-`...........
0800:49D0 00 0C 20 E6 A1 EB 00 EB 00 B0 20 E6 A0 EB 00 EB .. ....... .....
0800:49E0 00 E6 20 E9 0C 01 00 00 66 50 66 53 66 51 66 67 .. .....fPfSfQfg
0800:49F0 8D 1D B4 D5 00 00 E4 A1 66 EB 00 66 EB 00 0C 20 ........f..f... 
0800:4A00 E6 A1 66 EB 00 66 EB 00 B0 20 E6 A0 66 EB 00 66 ..f..f... ..f..f
0800:4A10 EB 00 E6 20 66 E9 0A 01 00 00 8D 36 E4 A1 EB 00 ... f......6....
0800:4A20 EB 00 24 DF E6 A1 C3 90 6A 00 6A 2E 60 8D 1D CC ..$.....j.j.`...
0800:4A30 D5 00 00 E4 A1 EB 00 EB 00 0C 40 E6 A1 EB 00 EB ..........@.....
0800:4A40 00 B0 20 E6 A0 EB 00 EB 00 E6 20 E9 A4 00 00 00 .. ....... .....
0800:4A50 66 50 66 53 66 51 66 67 8D 1D CC D5 00 00 E4 A1 fPfSfQfg........
0800:4A60 66 EB 00 66 EB 00 0C 40 E6 A1 66 EB 00 66 EB 00 f..f...@..f..f..
0800:4A70 B0 20 E6 A0 66 EB 00 66 EB 00 E6 20 66 E9 A2 00 . ..f..f... f...
0800:4A80 00 00 8D 36 E4 A1 EB 00 EB 00 24 BF E6 A1 C3 90 ...6......$.....
0800:4A90 6A 00 6A 2F 60 8D 1D E4 D5 00 00 E4 A1 EB 00 EB j.j/`...........
0800:4AA0 00 0C 80 E6 A1 EB 00 EB 00 B0 20 E6 A0 EB 00 EB .......... .....
0800:4AB0 00 E6 20 EB 3F 8D 76 00 66 50 66 53 66 51 66 67 .. .?.v.fPfSfQfg
0800:4AC0 8D 1D E4 D5 00 00 E4 A1 66 EB 00 66 EB 00 0C 80 ........f..f....
0800:4AD0 E6 A1 66 EB 00 66 EB 00 B0 20 E6 A0 66 EB 00 66 ..f..f... ..f..f
0800:4AE0 EB 00 E6 20 66 EB 3D 90 E4 A1 EB 00 EB 00 24 7F ... f.=.......$.
0800:4AF0 E6 A1 C3 90 1E 06 0F A0 0F A8 66 8C D0 66 8E D8 ..........f..f..
0800:4B00 66 8E C0 8D 05 00 D6 00 00 8B 48 00 89 4B 00 89 f.........H..K..
0800:4B10 43 04 89 58 00 89 59 04 89 25 FC D5 00 00 E9 D0 C..X..Y..%......
0800:4B20 F8 FF FF 90 66 67 8D 05 00 D6 00 00 66 67 2E 8B ....fg......fg..
0800:4B30 48 00 67 2E 89 4B 00 67 2E 89 43 04 67 2E 89 58 H.g..K.g..C.g..X
0800:4B40 00 67 2E 89 59 04 67 2E 89 25 FC D5 00 00 66 59 .g..Y.g..%....fY
0800:4B50 66 5B 66 58 CF 8D 76 00 50 B0 20 E6 20 8D 05 4C f[fX..v.P. . ..L
0800:4B60 B3 00 00 E9 E0 02 00 00 66 50 66 51 B0 20 E6 20 ........fPfQ. . 
0800:4B70 66 67 8D 0D 4C B3 00 00 66 E9 0E 03 00 00 8D 36 fg..L...f......6
0800:4B80 50 B0 20 E6 20 8D 05 54 B3 00 00 E9 B8 02 00 00 P. . ..T........
0800:4B90 66 50 66 51 B0 20 E6 20 66 67 8D 0D 54 B3 00 00 fPfQ. . fg..T...
0800:4BA0 66 E9 E6 02 00 00 8D 36 50 B0 20 E6 20 8D 05 5C f......6P. . ..\
0800:4BB0 B3 00 00 E9 90 02 00 00 66 50 66 51 B0 20 E6 20 ........fPfQ. . 
0800:4BC0 66 67 8D 0D 5C B3 00 00 66 E9 BE 02 00 00 8D 36 fg..\...f......6
0800:4BD0 50 B0 20 E6 20 8D 05 64 B3 00 00 E9 68 02 00 00 P. . ..d....h...
0800:4BE0 66 50 66 51 B0 20 E6 20 66 67 8D 0D 64 B3 00 00 fPfQ. . fg..d...
0800:4BF0 66 E9 96 02 00 00 8D 36 50 B0 20 E6 20 8D 05 6C f......6P. . ..l
0800:4C00 B3 00 00 E9 40 02 00 00 66 50 66 51 B0 20 E6 20 ....@...fPfQ. . 
0800:4C10 66 67 8D 0D 6C B3 00 00 66 E9 6E 02 00 00 8D 36 fg..l...f.n....6
0800:4C20 50 B0 20 E6 20 8D 05 74 B3 00 00 E9 18 02 00 00 P. . ..t........
0800:4C30 66 50 66 51 B0 20 E6 20 66 67 8D 0D 74 B3 00 00 fPfQ. . fg..t...
0800:4C40 66 E9 46 02 00 00 8D 36 50 B0 20 E6 20 8D 05 7C f.F....6P. . ..|
0800:4C50 B3 00 00 E9 F0 01 00 00 66 50 66 51 B0 20 E6 20 ........fPfQ. . 
0800:4C60 66 67 8D 0D 7C B3 00 00 66 E9 1E 02 00 00 8D 36 fg..|...f......6
0800:4C70 50 B0 20 E6 20 8D 05 84 B3 00 00 E9 C8 01 00 00 P. . ...........
0800:4C80 66 50 66 51 B0 20 E6 20 66 67 8D 0D 84 B3 00 00 fPfQ. . fg......
0800:4C90 66 E9 F6 01 00 00 8D 36 50 B0 20 E6 A0 EB 00 EB f......6P. .....
0800:4CA0 00 E6 20 8D 05 8C B3 00 00 E9 9A 01 00 00 8D 36 .. ............6
0800:4CB0 66 50 66 51 B0 20 E6 A0 66 EB 00 66 EB 00 E6 20 fPfQ. ..f..f... 
0800:4CC0 66 67 8D 0D 8C B3 00 00 66 E9 BE 01 00 00 8D 36 fg......f......6
0800:4CD0 50 B0 20 E6 A0 EB 00 EB 00 E6 20 8D 05 94 B3 00 P. ....... .....
0800:4CE0 00 E9 62 01 00 00 8D 36 66 50 66 51 B0 20 E6 A0 ..b....6fPfQ. ..
0800:4CF0 66 EB 00 66 EB 00 E6 20 66 67 8D 0D 94 B3 00 00 f..f... fg......
0800:4D00 66 E9 86 01 00 00 8D 36 50 B0 20 E6 A0 EB 00 EB f......6P. .....
0800:4D10 00 E6 20 8D 05 9C B3 00 00 E9 2A 01 00 00 8D 36 .. .......*....6
0800:4D20 66 50 66 51 B0 20 E6 A0 66 EB 00 66 EB 00 E6 20 fPfQ. ..f..f... 
0800:4D30 66 67 8D 0D 9C B3 00 00 66 E9 4E 01 00 00 8D 36 fg......f.N....6
0800:4D40 50 B0 20 E6 A0 EB 00 EB 00 E6 20 8D 05 A4 B3 00 P. ....... .....
0800:4D50 00 E9 F2 00 00 00 8D 36 66 50 66 51 B0 20 E6 A0 .......6fPfQ. ..
0800:4D60 66 EB 00 66 EB 00 E6 20 66 67 8D 0D A4 B3 00 00 f..f... fg......
0800:4D70 66 E9 16 01 00 00 8D 36 50 B0 20 E6 A0 EB 00 EB f......6P. .....
0800:4D80 00 E6 20 8D 05 AC B3 00 00 E9 BA 00 00 00 8D 36 .. ............6
0800:4D90 66 50 66 51 B0 20 E6 A0 66 EB 00 66 EB 00 E6 20 fPfQ. ..f..f... 
0800:4DA0 66 67 8D 0D AC B3 00 00 66 E9 DE 00 00 00 8D 36 fg......f......6
0800:4DB0 50 B0 20 E6 A0 EB 00 EB 00 E6 20 8D 05 B4 B3 00 P. ....... .....
0800:4DC0 00 E9 82 00 00 00 8D 36 66 50 66 51 B0 20 E6 A0 .......6fPfQ. ..
0800:4DD0 66 EB 00 66 EB 00 E6 20 66 67 8D 0D B4 B3 00 00 f..f... fg......
0800:4DE0 66 E9 A6 00 00 00 8D 36 50 B0 20 E6 A0 EB 00 EB f......6P. .....
0800:4DF0 00 E6 20 8D 05 BC B3 00 00 EB 4D 90 66 50 66 51 .. .......M.fPfQ
0800:4E00 B0 20 E6 A0 66 EB 00 66 EB 00 E6 20 66 67 8D 0D . ..f..f... fg..
0800:4E10 BC B3 00 00 66 EB 75 90 50 B0 20 E6 A0 EB 00 EB ....f.u.P. .....
0800:4E20 00 E6 20 8D 05 C4 B3 00 00 EB 1D 90 66 50 66 51 .. .........fPfQ
0800:4E30 B0 20 E6 A0 66 EB 00 66 EB 00 E6 20 66 67 8D 0D . ..f..f... fg..
0800:4E40 C4 B3 00 00 66 EB 45 90 51 52 1E 06 BA 2C 00 00 ....f.E.QR...,..
0800:4E50 00 66 8E DA 66 8E C2 36 8B 08 36 8B 50 04 89 0A .f..f..6..6.P...
0800:4E60 C7 42 04 24 00 00 00 89 62 0C 89 D4 BA 2C 00 00 .B.$....b....,..
0800:4E70 00 66 8E D2 FC CB 8D 36 8B 24 24 B8 10 00 00 00 .f.....6.$$.....
0800:4E80 66 8E D0 07 1F 5A 59 58 CF 8D 76 00 66 52 66 56 f....ZYX..v.fRfV
0800:4E90 66 89 E0 66 0F A8 66 0F A0 66 1E 66 06 66 16 66 f..f..f..f.f.f.f
0800:4EA0 50 66 9C 66 0E 66 68 CA 4E 00 00 66 31 D2 66 31 Pf.f.fh.N..f1.f1
0800:4EB0 C0 8C D2 89 E0 66 C1 E2 04 66 01 C2 B8 0C DE 66 .....f...f.....f
0800:4EC0 67 2E 8B 35 CC B3 00 00 CD 67 66 5E 66 5A 66 59 g..5.....gf^fZfY
0800:4ED0 66 58 CF 90 B8 2C 00 00 00 66 8E D8 66 8E C0 66 fX...,...f..f..f
0800:4EE0 8E D0 2E 8B 01 2E 8B 61 04 89 04 24 C7 44 24 04 .......a...$.D$.
0800:4EF0 24 00 00 00 83 44 24 08 08 89 54 24 0C FC CB 90 $....D$...T$....
0800:4F00 83 6C 24 FC 08 8B 24 24 B8 30 00 00 00 66 8E D0 .l$...$$.0...f..
0800:4F10 66 8E D8 B8 0C DE 00 00 2E FF 1D 04 8E 00 00 90 f...............
0800:4F20 FA 83 F8 FF 74 75 66 8C D1 66 8E D9 66 8E C1 C7 ....tuf..f..f...
0800:4F30 05 1C D6 00 00 00 00 00 00 8B 4C 24 08 03 0D 1C ..........L$....
0800:4F40 B3 00 00 83 C1 04 51 FF 14 85 94 90 00 00 83 C4 ......Q.........
0800:4F50 04 8B 15 1C D6 00 00 8B 0D FC D5 00 00 09 D1 75 ...............u
0800:4F60 0D B9 1F 00 00 00 66 8E D9 66 8E C1 FB CB 59 5A ......f..f....YZ
0800:4F70 9C 81 0C 24 00 02 00 00 52 51 6A 00 6A FF 50 6A ...$....RQj.j.Pj
0800:4F80 00 6A 00 53 6A 00 55 56 57 6A 1F 6A 1F 0F A0 0F .j.Sj.UVWj.j....
0800:4F90 A8 68 F3 43 00 00 E9 25 12 00 00 59 5A 58 83 C0 .h.C...%...YZX..
0800:4FA0 24 50 FF 70 EC 52 FF 70 E8 6A 00 6A FF FF 70 F8 $P.p.R.p.j.j..p.
0800:4FB0 FF 70 F4 FF 70 F0 53 6A 00 55 56 57 6A 1F 6A 1F .p..p.Sj.UVWj.j.
0800:4FC0 0F A0 0F A8 8B 50 FC 8B 48 E4 66 8C D0 66 8E D8 .....P..H.f..f..
0800:4FD0 66 8E C0 89 0D 18 D6 00 00 52 FF 52 14 83 C4 04 f........R.R....
0800:4FE0 68 F3 43 00 00 E9 D6 11 00 00 8D 36 56 57 8B 74 h.C........6VW.t
0800:4FF0 24 0C 8B 7C 24 10 8B 54 24 14 C7 05 D0 B2 00 00 $..|$..T$.......
0800:5000 2E 50 00 00 B8 1F 00 00 00 8E D8 89 D1 C1 E9 02 .P..............
0800:5010 F3 A5 89 D1 83 E1 03 F3 A4 31 C0 66 8C D7 66 8E .........1.f..f.
0800:5020 DF C7 05 D0 B2 00 00 00 00 00 00 5F 5E C3 B8 01 ..........._^...
0800:5030 00 00 00 EB E6 8D 76 00 56 57 8B 74 24 0C 8B 7C ......v.VW.t$..|
0800:5040 24 10 8B 54 24 14 C7 05 D0 B2 00 00 7A 50 00 00 $..T$.......zP..
0800:5050 B8 1F 00 00 00 8E C0 89 D1 C1 E9 02 F3 A5 89 D1 ................
0800:5060 83 E1 03 F3 A4 31 C0 66 8C D7 66 8E C7 C7 05 D0 .....1.f..f.....
0800:5070 B2 00 00 00 00 00 00 5F 5E C3 B8 01 00 00 00 EB ......._^.......
0800:5080 E6 00 00 00 55 89 E5 6A 00 6A 0C 6A 00 68 00 10 ....U..j.j.j.h..
0800:5090 00 00 68 08 91 00 00 E8 A4 2A 00 00 85 C0 74 08 ..h......*....t.
0800:50A0 2B 05 9C B2 00 00 C9 C3 31 C0 C9 C3 55 89 E5 53 +.......1...U..S
0800:50B0 E8 CF FF FF FF 89 C3 85 DB 74 15 68 00 10 00 00 .........t.h....
0800:50C0 03 05 9C B2 00 00 50 E8 2C 26 00 00 89 D8 EB 02 ......P.,&......
0800:50D0 31 C0 8B 5D FC C9 C3 90 55 89 E5 57 56 53 8B 7D 1..]....U..WVS.}
0800:50E0 08 8B 75 0C 8B 1D 0C 93 00 00 03 1D 9C B2 00 00 ..u.............
0800:50F0 89 F8 C1 E8 16 8D 1C 83 F6 03 01 75 1C E8 AA FF ...........u....
0800:5100 FF FF 85 C0 75 0A B8 19 C0 00 00 EB 3E 8D 76 00 ....u.......>.v.
0800:5110 25 00 F0 FF FF 0C 07 89 03 8B 13 81 E2 00 F0 FF %...............
0800:5120 FF 03 15 9C B2 00 00 89 F8 C1 E8 0A 25 FC 0F 00 ............%...
0800:5130 00 01 C2 F7 C6 01 00 00 00 74 0C F6 02 01 74 07 .........t....t.
0800:5140 8B 02 83 E0 60 09 C6 89 32 31 C0 8D 65 F4 5B 5E ....`...21..e.[^
0800:5150 5F C9 C3 90 55 89 E5 8B 4D 08 8B 15 0C 93 00 00 _...U...M.......
0800:5160 03 15 9C B2 00 00 89 C8 C1 E8 16 8D 14 82 F6 02 ................
0800:5170 01 74 21 8B 12 81 E2 00 F0 FF FF 03 15 9C B2 00 .t!.............
0800:5180 00 89 C8 C1 E8 0A 25 FC 0F 00 00 01 C2 8B 02 C9 ......%.........
0800:5190 C3 8D 76 00 31 C0 C9 C3 55 89 E5 8B 15 0C 93 00 ..v.1...U.......
0800:51A0 00 0F 22 DA 0F 20 C0 0D 00 00 00 80 0F 22 C0 EB ..".. ......."..
0800:51B0 00 C9 C3 90 55 89 E5 0F 20 C0 25 FF FF FF 7F 0F ....U... .%.....
0800:51C0 22 C0 EB 00 31 D2 0F 22 DA C9 C3 90 55 89 E5 53 "...1.."....U..S
0800:51D0 E8 D7 FE FF FF A3 0C 93 00 00 85 C0 75 0D 68 00 ............u.h.
0800:51E0 89 00 00 E8 1C B1 FF FF 83 C4 04 31 DB 39 1D A0 ...........1.9..
0800:51F0 B2 00 00 76 30 8D 76 00 89 D8 0C 07 50 53 E8 D5 ...v0.v.....PS..
0800:5200 FE FF FF 83 C4 08 85 C0 74 0D 68 00 89 00 00 E8 ........t.h.....
0800:5210 F0 B0 FF FF 83 C4 04 81 C3 00 10 00 00 39 1D A0 .............9..
0800:5220 B2 00 00 77 D3 8B 5D FC C9 C3 8D 36 55 89 E5 53 ...w..]....6U..S
0800:5230 8B 45 08 A3 0C 93 00 00 8B 5D 0C 39 1D A0 B2 00 .E.......].9....
0800:5240 00 76 2E 90 89 D8 0C 07 50 53 E8 89 FE FF FF 83 .v......PS......
0800:5250 C4 08 85 C0 74 0D 68 00 89 00 00 E8 A4 B0 FF FF ....t.h.........
0800:5260 83 C4 04 81 C3 00 10 00 00 39 1D A0 B2 00 00 77 .........9.....w
0800:5270 D3 8B 5D FC C9 C3 00 00 55 89 E5 53 83 7D 08 00 ..].....U..S.}..
0800:5280 0F 95 C0 0F B6 D8 53 E8 54 AF FF FF 83 C4 04 83 ......S.T.......
0800:5290 7D 08 00 74 1B 8D 45 0C 50 FF 75 08 E8 BF 25 00 }..t..E.P.u...%.
0800:52A0 00 6A 0A E8 B4 04 00 00 53 E8 BE 24 00 00 8D 36 .j......S..$...6
0800:52B0 53 E8 B6 24 00 00 8D 36 55 89 E5 56 53 8B 75 08 S..$...6U..VS.u.
0800:52C0 8B 45 0C 8D 1C 30 81 FB 00 00 10 00 77 0A 6A FE .E...0......w.j.
0800:52D0 6A 03 50 89 F0 EB 57 90 81 FE FF FF FF 00 77 2C j.P...W.......w,
0800:52E0 89 D8 81 FB 00 00 00 01 76 05 B8 00 00 00 01 6A ........v......j
0800:52F0 FF 6A 02 29 F0 50 89 F0 03 05 9C B2 00 00 50 68 .j.).P........Ph
0800:5300 08 91 00 00 E8 37 27 00 00 83 C4 14 81 FB 00 00 .....7'.........
0800:5310 00 01 76 2B 89 F2 81 FA FF FF FF 00 77 05 BA 00 ..v+........w...
0800:5320 00 00 01 6A 00 6A 00 89 D8 29 D0 50 89 D0 03 05 ...j.j...).P....
0800:5330 9C B2 00 00 50 68 08 91 00 00 E8 01 27 00 00 39 ....Ph......'..9
0800:5340 1D A0 B2 00 00 73 06 89 1D A0 B2 00 00 8D 65 F8 .....s........e.
0800:5350 5B 5E C9 C3 55 89 E5 E8 50 D6 FF FF E8 8B E8 FF [^..U...P.......
0800:5360 FF E8 12 D7 FF FF C9 C3 55 89 E5 83 EC 04 83 3D ........U......=
0800:5370 E8 B2 00 00 00 75 34 BA 21 00 00 00 EC 25 FF 00 .....u4.!....%..
0800:5380 00 00 89 45 FC EB 00 EB 00 BA A1 00 00 00 EC EB ...E............
0800:5390 00 EB 00 66 C1 E0 08 66 0B 45 FC 66 A3 02 B3 00 ...f...f.E.f....
0800:53A0 00 C7 05 E8 B2 00 00 01 00 00 00 E8 AC 00 00 00 ................
0800:53B0 C7 05 FC B2 00 00 08 00 00 00 66 A1 E4 B2 00 00 ..........f.....
0800:53C0 66 A3 00 B3 00 00 B0 11 BA 20 00 00 00 EE EB 00 f........ ......
0800:53D0 EB 00 8A 45 08 BA 21 00 00 00 EE EB 00 EB 00 B0 ...E..!.........
0800:53E0 04 EE EB 00 EB 00 B0 01 EE EB 00 EB 00 B0 11 BA ................
0800:53F0 A0 00 00 00 EE EB 00 EB 00 8A 45 0C BA A1 00 00 ..........E.....
0800:5400 00 EE EB 00 EB 00 B0 02 EE EB 00 EB 00 B0 01 EE ................
0800:5410 EB 00 EB 00 8A 05 02 B3 00 00 BA 21 00 00 00 EE ...........!....
0800:5420 EB 00 EB 00 66 8B 0D 02 B3 00 00 66 C1 E9 08 88 ....f......f....
0800:5430 C8 BA A1 00 00 00 EE EB 00 EB 00 B0 20 BA 20 00 ............ . .
0800:5440 00 00 EE EB 00 EB 00 BA A0 00 00 00 EE EB 00 EB ................
0800:5450 00 BA 60 00 00 00 EC C9 C3 8D 76 00 55 89 E5 56 ..`.......v.U..V
0800:5460 53 31 F6 90 31 C9 BB 01 00 00 00 31 D2 8D 76 00 S1..1......1..v.
0800:5470 0F B7 C1 66 0F B6 80 EC B2 00 00 66 39 F0 77 03 ...f.......f9.w.
0800:5480 66 09 DA 66 41 66 01 DB 66 83 F9 0F 76 E2 89 D0 f..fAf..f...v...
0800:5490 B0 00 66 3D 00 FF 74 03 80 E2 FB 0F B7 C6 66 89 ..f=..t.......f.
0800:54A0 14 45 D4 B2 00 00 66 46 66 83 FE 08 76 B6 8D 65 .E....fFf...v..e
0800:54B0 F8 5B 5E C9 C3 00 00 00 55 89 E5 57 56 53 8B 5D .[^.....U..WVS.]
0800:54C0 10 8B 75 14 8B 7D 18 6A 00 FF 75 0C FF 35 10 93 ..u..}.j..u..5..
0800:54D0 00 00 E8 D1 1C 00 00 83 C4 0C 56 53 FF 35 10 93 ..........VS.5..
0800:54E0 00 00 E8 7D 1B 00 00 89 07 31 C0 8D 65 F4 5B 5E ...}.....1..e.[^
0800:54F0 5F C9 C3 90 55 89 E5 57 56 53 8B 5D 10 8B 75 14 _...U..WVS.]..u.
0800:5500 8B 7D 18 83 3D 18 B3 00 00 01 7F 38 A1 18 B3 00 .}..=......8....
0800:5510 00 8D 04 80 C1 E0 02 BA 20 B3 00 00 8B 4D 0C 89 ........ ....M..
0800:5520 4C 02 04 89 5C 02 0C 89 B0 20 B3 00 00 89 7C 02 L...\.... ....|.
0800:5530 08 8B 4D 1C 89 4C 02 10 FF 05 18 B3 00 00 31 C0 ..M..L........1.
0800:5540 EB 07 8D 36 B8 4E 61 BC 00 8D 65 F4 5B 5E 5F C9 ...6.Na...e.[^_.
0800:5550 C3 8D 76 00 55 89 E5 6A 00 6A 00 68 00 10 00 00 ..v.U..j.j.h....
0800:5560 E8 8B AC FF FF A3 04 B3 00 00 83 C4 0C 85 C0 75 ...............u
0800:5570 0D 68 18 89 00 00 E8 89 AD FF FF 83 C4 04 E8 D1 .h..............
0800:5580 AD FF FF E8 50 CB FF FF E8 F7 18 00 00 E8 D6 0B ....P...........
0800:5590 00 00 6A 00 FF 35 88 90 00 00 E8 5D 19 00 00 A3 ..j..5.....]....
0800:55A0 10 93 00 00 83 C4 08 85 C0 7D 19 FF 35 1C D6 00 .........}..5...
0800:55B0 00 FF 35 88 90 00 00 68 44 89 00 00 E8 43 AD FF ..5....hD....C..
0800:55C0 FF 83 C4 0C 68 08 B3 00 00 6A 00 68 F4 54 00 00 ....h....j.h.T..
0800:55D0 68 B8 54 00 00 E8 8E 1D 00 00 A3 1C D6 00 00 83 h.T.............
0800:55E0 C4 10 85 C0 74 14 50 FF 35 88 90 00 00 68 6E 89 ....t.P.5....hn.
0800:55F0 00 00 E8 0D AD FF FF 83 C4 0C A1 48 B3 00 00 8B ...........H....
0800:5600 00 FF D0 C9 C3 8D 76 00 55 89 E5 56 53 8B 5D 0C ......v.U..VS.].
0800:5610 8B 75 10 6A 00 FF 75 08 FF 35 10 93 00 00 E8 85 .u.j..u..5......
0800:5620 1B 00 00 83 C4 0C 56 53 FF 35 10 93 00 00 E8 31 ......VS.5.....1
0800:5630 1A 00 00 83 C4 0C 39 F0 74 10 FF 35 88 90 00 00 ......9.t..5....
0800:5640 68 B9 89 00 00 E8 BA AC FF FF 8D 65 F8 5B 5E C9 h..........e.[^.
0800:5650 C3 8D 76 00 55 89 E5 83 EC 10 57 56 53 8B 75 08 ..v.U.....WVS.u.
0800:5660 C7 45 FC 00 00 00 00 8B 45 0C 83 38 00 74 10 90 .E......E..8.t..
0800:5670 FF 45 FC 8B 55 FC 8B 45 0C 83 3C 90 00 75 F1 8B .E..U..E..<..u..
0800:5680 55 FC 8D 14 95 04 00 00 00 29 D6 89 75 F8 89 F0 U........)..u...
0800:5690 03 05 1C B3 00 00 89 45 F4 31 DB 39 5D FC 7E 3D .......E.1.9].~=
0800:56A0 8B 55 0C 8B 3C 9A 30 C0 FC B9 FF FF FF FF F2 AE .U..<.0.........
0800:56B0 F7 D1 89 4D F0 29 CE 83 E6 FC 8B 55 F4 89 34 9A ...M.).....U..4.
0800:56C0 8B 45 0C FF 34 98 89 F2 03 15 1C B3 00 00 52 E8 .E..4.........R.
0800:56D0 88 22 00 00 83 C4 08 43 39 5D FC 7F C3 8B 45 FC .".....C9]....E.
0800:56E0 8B 55 F4 C7 04 82 00 00 00 00 8B 55 F8 8B 45 10 .U.........U..E.
0800:56F0 89 10 83 7D 14 00 74 08 8B 45 FC 8B 55 14 89 02 ...}..t..E..U...
0800:5700 89 F0 8D 65 E4 5B 5E 5F C9 C3 8D 36 55 89 E5 83 ...e.[^_...6U...
0800:5710 EC 0C 8D 45 F8 50 8D 45 FC 50 FF 35 94 B2 00 00 ...E.P.E.P.5....
0800:5720 FF 75 08 E8 2C FF FF FF 89 C2 6A 00 8D 45 F4 50 .u..,.....j..E.P
0800:5730 FF 35 F0 A8 00 00 52 E8 18 FF FF FF 8D 48 F4 89 .5....R......H..
0800:5740 CA 03 15 1C B3 00 00 8B 45 F8 89 02 8B 45 FC 89 ........E....E..
0800:5750 42 04 8B 45 F4 89 42 08 89 C8 C9 C3 55 89 E5 83 B..E..B.....U...
0800:5760 EC 34 53 8B 5D 08 83 FB 0A 75 0A 6A 0D E8 EA FF .4S.]....u.j....
0800:5770 FF FF 83 C4 04 66 C7 45 EC 00 00 66 C7 45 FC 00 .....f.E...f.E..
0800:5780 00 66 C7 45 FA 00 00 C7 45 E8 00 02 00 00 89 5D .f.E....E......]
0800:5790 E0 8D 45 CC 50 6A 21 A1 90 90 00 00 FF D0 31 C0 ..E.Pj!.......1.
0800:57A0 F6 45 EC 01 74 05 B8 FF FF FF FF 8B 5D C8 C9 C3 .E..t.......]...
0800:57B0 55 89 E5 53 8B 5D 08 80 3B 00 74 12 0F BE 03 50 U..S.]..;.t....P
0800:57C0 E8 97 FF FF FF 43 83 C4 04 80 3B 00 75 EE 6A 0A .....C....;.u.j.
0800:57D0 E8 87 FF FF FF 31 C0 8B 5D FC C9 C3 55 89 E5 56 .....1..]...U..V
0800:57E0 53 8B 75 08 8B 5D 0C EA EE 57 00 00 18 00 66 E8 S.u..]...W....f.
0800:57F0 6C DA FF FF 66 53 66 56 66 E8 CA D9 FF FF 66 83 l...fSfVf.....f.
0800:5800 C4 08 66 E8 B8 DA FF FF 66 EA 10 58 00 00 08 00 ..f.....f..X....
0800:5810 8D 65 F8 5B 5E C9 C3 90 55 89 E5 C7 05 48 B3 00 .e.[^...U....H..
0800:5820 00 10 8A 00 00 E8 2A FD FF FF C9 C3 55 89 E5 81 ......*.....U...
0800:5830 3D 90 90 00 00 C4 5F 00 00 75 1E C7 05 90 90 00 =....._..u......
0800:5840 00 DC 57 00 00 C7 05 FC 8D 00 00 60 32 00 00 C7 ..W........`2...
0800:5850 05 00 8E 00 00 C0 32 00 00 0F 20 C0 0C 08 0F 22 ......2... ...."
0800:5860 C0 E8 B2 FF FF FF C9 C3 55 89 E5 53 8B 5D 08 EA ........U..S.]..
0800:5870 76 58 00 00 18 00 66 E8 B8 D7 FF FF 66 53 66 E8 vX....f.....fSf.
0800:5880 B8 D1 FF FF 66 EB FD 90 55 89 E5 53 C7 05 14 93 ....f...U..S....
0800:5890 00 00 00 00 00 00 31 D2 B9 20 B3 00 00 8D 76 00 ......1.. ....v.
0800:58A0 8D 04 92 C1 E0 02 83 7C 01 08 00 74 19 8B 98 20 .......|...t... 
0800:58B0 B3 00 00 03 5C 01 08 89 D8 39 05 14 93 00 00 73 ....\....9.....s
0800:58C0 05 A3 14 93 00 00 42 83 FA 01 7E D4 A1 14 93 00 ......B...~.....
0800:58D0 00 05 FF 0F 00 00 25 00 F0 FF FF A3 14 93 00 00 ......%.........
0800:58E0 81 05 14 93 00 00 00 10 00 00 A1 14 93 00 00 05 ................
0800:58F0 00 00 10 00 A3 18 93 00 00 8B 1D 9C B2 00 00 81 ................
0800:5900 C3 00 00 00 40 89 1D 1C B3 00 00 50 E8 FB FD FF ....@......P....
0800:5910 FF 89 C1 83 C4 04 FB BA 0C B3 00 00 9C 58 BB 1F .............X..
0800:5920 00 00 00 53 51 50 6A 17 FF 32 66 8E DB 66 8E C3 ...SQPj..2f..f..
0800:5930 CF 68 DC 89 00 00 E8 C9 A9 FF FF 8B 5D FC C9 C3 .h..........]...
0800:5940 55 89 E5 56 53 8B 45 08 8B 75 0C 8B 1D 18 93 00 U..VS.E..u......
0800:5950 00 85 C0 7D 07 B8 0E C0 00 00 EB 28 01 05 18 93 ...}.......(....
0800:5960 00 00 83 3D 1C 93 00 00 00 74 15 50 53 E8 A2 00 ...=.....t.PS...
0800:5970 00 00 85 C0 74 0A 89 1D 18 93 00 00 EB 06 8D 36 ....t..........6
0800:5980 89 1E 31 C0 8D 65 F8 5B 5E C9 C3 90 55 89 E5 56 ..1..e.[^...U..V
0800:5990 53 8B 45 08 8B 4D 10 89 C6 81 E6 00 F0 FF FF 89 S.E..M..........
0800:59A0 C3 29 F3 03 5D 0C 81 C3 FF 0F 00 00 81 E3 00 F0 .)..]...........
0800:59B0 FF FF 8B 15 18 93 00 00 81 C2 FF 0F 00 00 81 E2 ................
0800:59C0 00 F0 FF FF 89 15 18 93 00 00 29 F0 01 D0 89 01 ..........).....
0800:59D0 85 DB 74 34 89 F0 0C 07 50 A1 18 93 00 00 05 00 ..t4....P.......
0800:59E0 00 00 40 50 E8 EF F6 FF FF 83 C4 08 85 C0 75 1A ..@P..........u.
0800:59F0 81 C6 00 10 00 00 81 05 18 93 00 00 00 10 00 00 ................
0800:5A00 81 C3 00 F0 FF FF 75 CC 31 C0 8D 65 F8 5B 5E C9 ......u.1..e.[^.
0800:5A10 C3 8D 76 00 55 89 E5 57 56 53 8B 75 08 8B 7D 0C ..v.U..WVS.u..}.
0800:5A20 89 F3 03 1D 1C B3 00 00 2B 1D 9C B2 00 00 EB 3A ........+......:
0800:5A30 53 E8 1E F7 FF FF 83 C4 04 A8 01 74 0F 80 CC 02 S..........t....
0800:5A40 50 53 E8 91 F6 FF FF EB 0E 8D 76 00 68 00 02 00 PS........v.h...
0800:5A50 00 53 E8 E9 03 00 00 83 C4 08 85 C0 75 21 81 C3 .S..........u!..
0800:5A60 00 10 00 00 81 E3 00 F0 FF FF 8D 04 37 03 05 1C ............7...
0800:5A70 B3 00 00 2B 05 9C B2 00 00 39 C3 72 B3 31 C0 8D ...+.....9.r.1..
0800:5A80 65 F4 5B 5E 5F C9 C3 90 55 89 E5 57 56 53 8B 7D e.[^_...U..WVS.}
0800:5A90 08 89 FB 03 1D 1C B3 00 00 2B 1D 9C B2 00 00 EB .........+......
0800:5AA0 40 8D 76 00 53 E8 AA F6 FF FF 89 C6 83 C4 04 F7 @.v.S...........
0800:5AB0 C6 01 00 00 00 74 1E 53 68 E9 89 00 00 E8 E2 1D .....t.Sh.......
0800:5AC0 00 00 89 F0 80 E4 FD 50 53 E8 0A F6 FF FF 83 C4 .......PS.......
0800:5AD0 10 85 C0 75 23 81 C3 00 10 00 00 81 E3 00 F0 FF ...u#...........
0800:5AE0 FF 8B 45 0C 01 F8 03 05 1C B3 00 00 2B 05 9C B2 ..E.........+...
0800:5AF0 00 00 39 C3 72 AE 31 C0 8D 65 F4 5B 5E 5F C9 C3 ..9.r.1..e.[^_..
0800:5B00 55 89 E5 56 53 8B 45 08 A8 02 74 0A C7 05 1C 93 U..VS.E...t.....
0800:5B10 00 00 01 00 00 00 A8 01 74 52 31 DB 39 1D 18 B3 ........tR1.9...
0800:5B20 00 00 7E 2D BE 20 B3 00 00 8D 76 00 8D 04 9B C1 ..~-. ....v.....
0800:5B30 E0 02 FF 74 06 08 FF B0 20 B3 00 00 E8 D3 FE FF ...t.... .......
0800:5B40 FF 83 C4 08 85 C0 75 26 43 39 1D 18 B3 00 00 7F ......u&C9......
0800:5B50 DB A1 18 93 00 00 2B 05 14 93 00 00 50 FF 35 14 ......+.....P.5.
0800:5B60 93 00 00 E8 AC FE FF FF 85 C0 75 02 31 C0 8D 65 ..........u.1..e
0800:5B70 F8 5B 5E C9 C3 8D 76 00 55 89 E5 68 FC 89 00 00 .[^...v.U..h....
0800:5B80 E8 7F A7 FF FF C9 C3 90 55 89 E5 83 EC 04 57 56 ........U.....WV
0800:5B90 53 8B 5D 08 8B 7D 0C 83 FB 07 7F 0C 89 DE 03 35 S.]..}.........5
0800:5BA0 AC B2 00 00 EB 0D 8D 36 89 DE 03 35 B0 B2 00 00 .......6...5....
0800:5BB0 83 C6 F8 83 FB 07 7F 14 89 D8 03 05 AC B2 00 00 ................
0800:5BC0 8D 14 C5 F4 A8 00 00 EB 12 8D 76 00 89 D8 03 05 ..........v.....
0800:5BD0 B0 B2 00 00 8D 14 C5 B4 A8 00 00 8B 04 9D 48 8F ..............H.
0800:5BE0 00 00 66 89 02 66 C7 42 02 08 00 C6 42 04 00 C6 ..f..f.B....B...
0800:5BF0 42 05 8E C1 E8 10 66 89 42 06 81 3D 90 90 00 00 B.....f.B..=....
0800:5C00 DC 57 00 00 74 37 66 A1 A4 B2 00 00 C1 E0 10 0F .W..t7f.........
0800:5C10 B7 4D FC 89 4D FC 09 45 FC 66 8B 04 9D C8 8F 00 .M..M..E.f......
0800:5C20 00 66 89 45 FC 8D 04 9D 20 93 00 00 50 56 E8 9D .f.E.... ...PV..
0800:5C30 A7 FF FF 8D 45 FC 50 56 E8 CF A7 FF FF 8B 04 9D ....E.PV........
0800:5C40 48 90 00 00 89 47 14 31 C0 8D 65 F0 5B 5E 5F C9 H....G.1..e.[^_.
0800:5C50 C3 8D 76 00 55 89 E5 53 8B 4D 08 83 F9 07 7F 0C ..v.U..S.M......
0800:5C60 89 CB 03 1D AC B2 00 00 EB 0D 8D 36 89 CB 03 1D ...........6....
0800:5C70 B0 B2 00 00 83 C3 F8 83 F9 07 7F 14 89 C8 03 05 ................
0800:5C80 AC B2 00 00 8D 14 C5 F4 A8 00 00 EB 12 8D 76 00 ..............v.
0800:5C90 89 C8 03 05 B0 B2 00 00 8D 14 C5 B4 A8 00 00 8B ................
0800:5CA0 04 8D 08 8F 00 00 66 89 02 66 C7 42 02 08 00 C6 ......f..f.B....
0800:5CB0 42 04 00 C6 42 05 8E C1 E8 10 66 89 42 06 81 3D B...B.....f.B..=
0800:5CC0 90 90 00 00 DC 57 00 00 74 0E 8D 04 8D 20 93 00 .....W..t.... ..
0800:5CD0 00 50 53 E8 34 A7 FF FF 8B 5D FC C9 C3 8D 76 00 .PS.4....]....v.
0800:5CE0 55 89 E5 83 EC 08 57 56 53 8B 5D 08 8B 75 0C 8B U.....WVS.]..u..
0800:5CF0 55 10 83 FB 07 7F 0D 89 D9 03 0D AC B2 00 00 EB U...............
0800:5D00 0E 8D 76 00 89 D9 03 0D B0 B2 00 00 83 C1 F8 89 ..v.............
0800:5D10 4D F8 8D 04 DD 00 00 00 00 89 B0 4C B3 00 00 8D M..........L....
0800:5D20 4A F0 89 88 50 B3 00 00 A1 1C B3 00 00 8B 0D 0C J...P...........
0800:5D30 B3 00 00 83 C1 E0 89 4C 02 F8 A1 1C B3 00 00 83 .......L........
0800:5D40 C0 E1 03 05 0C B3 00 00 C7 00 78 4E 00 00 66 C7 ..........xN..f.
0800:5D50 40 04 08 00 83 FB 07 7F 13 89 D8 03 05 AC B2 00 @...............
0800:5D60 00 8D 14 C5 F4 A8 00 00 EB 11 8D 36 89 D8 03 05 ...........6....
0800:5D70 B0 B2 00 00 8D 14 C5 B4 A8 00 00 8B 04 9D 88 8F ................
0800:5D80 00 00 66 89 02 66 C7 42 02 08 00 C6 42 04 00 C6 ..f..f.B....B...
0800:5D90 42 05 8E C1 E8 10 66 89 42 06 81 3D 90 90 00 00 B.....f.B..=....
0800:5DA0 DC 57 00 00 0F 84 8B 00 00 00 A1 1C B3 00 00 83 .W..............
0800:5DB0 C0 E9 03 05 0C B3 00 00 C7 00 00 4F 00 00 66 C7 ...........O..f.
0800:5DC0 40 04 08 00 BF 60 93 00 00 BE B4 B2 00 00 FC B9 @....`..........
0800:5DD0 06 00 00 00 F3 A5 C7 05 70 93 00 00 D4 4E 00 00 ........p....N..
0800:5DE0 66 C7 05 74 93 00 00 08 00 B9 60 93 00 00 2B 0D f..t......`...+.
0800:5DF0 9C B2 00 00 89 0D CC B3 00 00 66 A1 A4 B2 00 00 ..........f.....
0800:5E00 C1 E0 10 0F B7 4D FC 89 4D FC 09 45 FC 66 8B 04 .....M..M..E.f..
0800:5E10 9D 08 90 00 00 66 89 45 FC 8D 04 9D 20 93 00 00 .....f.E.... ...
0800:5E20 50 FF 75 F8 E8 A7 A5 FF FF 8D 45 FC 50 FF 75 F8 P.u.......E.P.u.
0800:5E30 E8 D7 A5 FF FF 31 C0 8D 65 EC 5B 5E 5F C9 C3 90 .....1..e.[^_...
0800:5E40 55 89 E5 83 EC 10 57 56 53 8B 45 08 8D 88 00 00 U.....WVS.E.....
0800:5E50 00 C0 89 CB 81 E3 00 F0 FF FF 89 5D F8 8B 5D 0C ...........]..].
0800:5E60 80 CB 05 89 5D FC 3D FF FF FF 3F 0F 86 43 01 00 ....].=...?..C..
0800:5E70 00 39 0D 14 93 00 00 77 3F A1 18 93 00 00 05 FF .9.....w?.......
0800:5E80 0F 00 00 25 00 F0 FF FF 39 C1 73 2C E8 F3 F1 FF ...%....9.s,....
0800:5E90 FF 89 C7 85 FF 74 60 68 00 10 00 00 89 F8 03 05 .....t`h........
0800:5EA0 9C B2 00 00 50 E8 4E 18 00 00 8B 45 FC 09 F8 0C ....P.N....E....
0800:5EB0 02 E9 E4 00 00 00 8D 36 31 D2 8D 36 39 15 18 B3 .......61..69...
0800:5EC0 00 00 0F 8E EC 00 00 00 8D 04 92 C1 E0 02 8D B0 ................
0800:5ED0 20 B3 00 00 39 88 20 B3 00 00 77 0D 8B 80 20 B3  ...9. ...w... .
0800:5EE0 00 00 03 46 08 39 C1 72 03 42 EB D0 E8 93 F1 FF ...F.9.r.B......
0800:5EF0 FF 89 C7 85 FF 75 0D B8 19 C0 00 00 E9 B8 00 00 .....u..........
0800:5F00 00 8D 76 00 8B 06 03 46 0C 39 45 F8 73 06 83 7E ..v....F.9E.s..~
0800:5F10 0C 00 75 18 68 00 10 00 00 89 F8 03 05 9C B2 00 ..u.h...........
0800:5F20 00 50 E8 D1 17 00 00 83 C4 08 EB 5F 8B 5D F8 2B .P........._.].+
0800:5F30 1E 89 5D F4 8B 5E 04 01 5D F4 C7 45 F0 00 10 00 ..]..^..]..E....
0800:5F40 00 8B 45 F8 05 00 10 00 00 8B 16 03 56 0C 39 D0 ..E.........V.9.
0800:5F50 76 22 2B 55 F8 89 55 F0 B8 00 10 00 00 29 D0 50 v"+U..U......).P
0800:5F60 8B 45 F0 01 F8 03 05 9C B2 00 00 50 E8 87 17 00 .E.........P....
0800:5F70 00 83 C4 08 FF 75 F0 89 F8 03 05 9C B2 00 00 50 .....u.........P
0800:5F80 FF 75 F4 E8 80 F6 FF FF 83 C4 0C F6 46 10 02 74 .u..........F..t
0800:5F90 04 80 4D FC 02 8B 45 FC 09 F8 50 8B 45 F8 05 00 ..M...E...P.E...
0800:5FA0 00 00 40 50 E8 2F F1 FF FF 85 C0 75 0C 31 C0 EB ..@P./.....u.1..
0800:5FB0 08 8D 76 00 B8 0B C0 00 00 8D 65 E4 5B 5E 5F C9 ..v.......e.[^_.
0800:5FC0 C3 00 00 00 55 89 E5 56 53 8B 75 08 8B 5D 0C EA ....U..VS.u..]..
0800:5FD0 D6 5F 00 00 18 00 66 E8 58 D0 FF FF 66 53 66 56 ._....f.X...fSfV
0800:5FE0 66 E8 E2 D1 FF FF 66 83 C4 08 66 E8 80 CF FF FF f.....f...f.....
0800:5FF0 66 EA F8 5F 00 00 08 00 8D 65 F8 5B 5E C9 C3 00 f.._.....e.[^...
0800:6000 55 89 E5 53 8B 55 08 8B 4D 0C 8B 5D 10 81 FA FF U..S.U..M..]....
0800:6010 FF 00 00 77 0B 66 A1 A4 B2 00 00 66 89 01 EB 12 ...w.f.....f....
0800:6020 2B 15 9C B2 00 00 89 D0 C1 E8 04 66 89 01 66 83 +..........f..f.
0800:6030 E2 0F 66 89 13 8B 5D FC C9 C3 8D 36 55 89 E5 68 ..f...]....6U..h
0800:6040 E4 B3 00 00 68 F4 B3 00 00 FF 75 08 E8 AF FF FF ....h.....u.....
0800:6050 FF C9 C3 90 55 89 E5 68 D0 B3 00 00 68 F2 B3 00 ....U..h....h...
0800:6060 00 FF 75 08 E8 97 FF FF FF C9 C3 90 55 89 E5 68 ..u.........U..h
0800:6070 3C 8A 00 00 E8 2B 18 00 00 FF 35 E4 B3 00 00 FF <....+....5.....
0800:6080 35 E8 B3 00 00 FF 35 E0 B3 00 00 FF 35 EC B3 00 5.....5.....5...
0800:6090 00 68 59 8A 00 00 E8 09 18 00 00 FF 35 D8 B3 00 .hY.........5...
0800:60A0 00 FF 35 D0 B3 00 00 FF 35 D4 B3 00 00 68 81 8A ..5.....5....h..
0800:60B0 00 00 E8 ED 17 00 00 83 C4 28 0F B7 05 F0 B3 00 .........(......
0800:60C0 00 50 0F B7 05 FE B3 00 00 50 0F B7 05 00 B4 00 .P.......P......
0800:60D0 00 50 68 9F 8A 00 00 E8 C8 17 00 00 C9 C3 00 00 .Ph.............
0800:60E0 55 89 E5 83 3D 18 B4 00 00 00 75 21 C7 05 18 B4 U...=.....u!....
0800:60F0 00 00 10 00 00 00 89 E0 A3 14 B4 00 00 66 C7 05 .............f..
0800:6100 76 B4 00 00 68 00 C6 05 78 D4 00 00 FF C9 C3 90 v...h...x.......
0800:6110 55 89 E5 53 8B 5D 08 E8 C4 FF FF FF 81 C3 48 08 U..S.]........H.
0800:6120 00 00 B9 10 B4 00 00 2B 0D 9C B2 00 00 BA 67 20 .......+......g 
0800:6130 00 00 66 89 13 66 89 4B 02 89 C8 C1 E8 10 88 43 ..f..f.K.......C
0800:6140 04 C6 43 05 89 C1 EA 10 88 D0 24 0F 80 63 06 F0 ..C.......$..c..
0800:6150 08 43 06 30 C0 80 63 06 0F 08 43 06 C1 E9 18 88 .C.0..c...C.....
0800:6160 4B 07 8B 5D FC C9 C3 00 55 89 E5 C7 05 04 D6 00 K..]....U.......
0800:6170 00 00 D6 00 00 C7 05 00 D6 00 00 00 D6 00 00 C9 ................
0800:6180 C3 8D 76 00 55 89 E5 8B 45 08 8B 0D 00 D6 00 00 ..v.U...E.......
0800:6190 81 F9 00 D6 00 00 74 26 39 C1 75 18 8B 11 8B 41 ......t&9.u....A
0800:61A0 04 89 42 04 8B 51 04 8B 01 89 02 51 8B 41 14 FF ..B..Q.....Q.A..
0800:61B0 D0 C9 C3 90 8B 09 81 F9 00 D6 00 00 75 DA C9 C3 ............u...
0800:61C0 55 89 E5 83 EC 10 57 56 53 8D 75 08 C7 05 FC D5 U.....WVS.u.....
0800:61D0 00 00 00 00 00 00 8B 1D 00 D6 00 00 81 FB 00 D6 ................
0800:61E0 00 00 0F 84 F0 00 00 00 8B 4B 08 89 4D F0 8B 3B .........K..M..;
0800:61F0 89 7D FC 8B 4B 0C 85 0D 18 D6 00 00 0F 85 C7 00 .}..K...........
0800:6200 00 00 8B 7D F0 8D 3C 7F 8D 0C BD 00 00 00 00 89 ...}..<.........
0800:6210 4D F4 81 C1 78 93 00 00 89 4D F8 8B 43 04 8B 7D M...x....M..C..}
0800:6220 FC 89 47 04 8B 53 04 8B 03 89 02 8B 4D F4 83 B9 ..G..S......M...
0800:6230 78 93 00 00 00 75 19 FF 75 F0 68 DD 8A 00 00 E8 x....u..u.h.....
0800:6240 60 16 00 00 56 E8 C2 0A 00 00 83 C4 0C EB 7A 90 `...V.........z.
0800:6250 8B 7D F8 83 3F 01 75 0C 53 8B 43 14 FF D0 83 C4 .}..?.u.S.C.....
0800:6260 04 EB 66 90 83 46 44 D8 8B 56 44 03 15 1C B3 00 ..f..FD..VD.....
0800:6270 00 8B 0D 0C B3 00 00 83 C1 F0 89 0A 8B 7D F0 89 .............}..
0800:6280 7A 04 8B 43 10 89 42 08 A1 18 D6 00 00 89 42 0C z..C..B.......B.
0800:6290 8B 46 38 89 42 10 8B 46 40 89 42 14 8B 46 24 89 .F8.B..F@.B..F$.
0800:62A0 42 18 8B 46 28 89 42 1C 8B 46 2C 89 42 20 89 5A B..F(.B..F,.B .Z
0800:62B0 24 8B 4D F8 8B 01 89 46 38 81 66 40 FF FA FF FF $.M....F8.f@....
0800:62C0 8B 79 04 09 3D 18 D6 00 00 8B 5D FC 81 FB 00 D6 .y..=.....].....
0800:62D0 00 00 0F 85 10 FF FF FF 8D 65 E4 5B 5E 5F C9 C3 .........e.[^_..
0800:62E0 55 89 E5 83 7D 38 10 77 77 8B 45 38 FF 24 85 F4 U...}8.ww.E8.$..
0800:62F0 62 00 00 90 50 63 00 00 38 63 00 00 60 63 00 00 b...Pc..8c..`c..
0800:6300 38 63 00 00 40 63 00 00 40 63 00 00 48 63 00 00 8c..@c..@c..Hc..
0800:6310 50 63 00 00 60 63 00 00 58 63 00 00 58 63 00 00 Pc..`c..Xc..Xc..
0800:6320 58 63 00 00 58 63 00 00 58 63 00 00 58 63 00 00 Xc..Xc..Xc..Xc..
0800:6330 60 63 00 00 60 63 00 00 B8 05 00 00 00 EB 26 90 `c..`c........&.
0800:6340 B8 10 00 00 00 EB 1E 90 B8 04 00 00 00 EB 16 90 ................
0800:6350 B8 08 00 00 00 EB 0E 90 B8 0B 00 00 00 EB 06 90 ................
0800:6360 B8 07 00 00 00 50 6A 01 E8 13 00 00 00 C9 C3 90 .....Pj.........
0800:6370 55 89 E5 8B 45 08 C7 40 08 00 00 00 00 C9 C3 90 U...E..@........
0800:6380 55 89 E5 56 53 8B 5D 0C 83 7D 08 01 76 0A B8 24 U..VS.]..}..v..$
0800:6390 C0 00 00 EB 7B 8D 76 00 8D 43 FF 83 F8 1F 76 08 ....{.v..C....v.
0800:63A0 B8 0E C0 00 00 EB 69 90 8D 4B FF 8D 04 49 C1 E0 ......i..K...I..
0800:63B0 03 BA F8 94 00 00 83 7C 02 08 00 75 51 89 5C 02 .......|...uQ.\.
0800:63C0 08 BE 01 00 00 00 D3 E6 89 74 02 0C C7 44 02 14 .........t...D..
0800:63D0 70 63 00 00 8D 04 5B 8D 14 C5 00 00 00 00 8D 8A pc....[.........
0800:63E0 E0 94 00 00 A1 00 D6 00 00 89 82 E0 94 00 00 C7 ................
0800:63F0 41 04 00 D6 00 00 8B 82 E0 94 00 00 89 48 04 8B A............H..
0800:6400 41 04 89 08 C7 05 FC D5 00 00 01 00 00 00 31 C0 A.............1.
0800:6410 8D 65 F8 5B 5E C9 C3 90 55 89 E5 57 56 53 8B 75 .e.[^...U..WVS.u
0800:6420 08 8B 7D 0C 8B 5D 10 8D 46 FF 83 F8 1E 76 09 B8 ..}..]..F....v..
0800:6430 0E C0 00 00 EB 64 8D 36 85 DB 74 24 8D 04 76 8D .....d.6..t$..v.
0800:6440 14 85 00 00 00 00 8B 82 78 93 00 00 89 03 8B 82 ........x.......
0800:6450 7C 93 00 00 89 43 04 8B 82 80 93 00 00 89 43 08 |....C........C.
0800:6460 85 FF 74 34 8D 04 76 8D 14 85 00 00 00 00 8B 07 ..t4..v.........
0800:6470 89 82 78 93 00 00 8B 47 04 89 82 7C 93 00 00 8B ..x....G...|....
0800:6480 47 08 89 82 80 93 00 00 8D 4E FF B8 01 00 00 00 G........N......
0800:6490 D3 E0 09 82 7C 93 00 00 31 C0 8D 65 F4 5B 5E 5F ....|...1..e.[^_
0800:64A0 C9 C3 8D 36 55 89 E5 53 8B 55 08 8B 5D 0C 8B 4D ...6U..S.U..]..M
0800:64B0 10 85 C9 74 07 A1 18 D6 00 00 89 01 85 DB 74 5B ...t..........t[
0800:64C0 83 FA 02 74 1F 7F 09 83 FA 01 74 0C EB 2A 8D 36 ...t......t..*.6
0800:64D0 83 FA 03 74 1F EB 21 90 8B 15 18 D6 00 00 0B 13 ...t..!.........
0800:64E0 EB 1E 8D 36 8B 03 F7 D0 89 C2 23 15 18 D6 00 00 ...6......#.....
0800:64F0 EB 0E 8D 36 8B 13 EB 08 B8 0E C0 00 00 EB 1E 90 ...6............
0800:6500 A1 18 D6 00 00 F7 D0 85 D0 74 0A C7 05 FC D5 00 .........t......
0800:6510 00 01 00 00 00 89 15 18 D6 00 00 31 C0 8B 5D FC ...........1..].
0800:6520 C9 C3 8D 36 55 89 E5 8B 4D 08 A1 00 D6 00 00 3D ...6U...M......=
0800:6530 00 D6 00 00 74 0E 8D 36 0B 50 0C 8B 00 3D 00 D6 ....t..6.P...=..
0800:6540 00 00 75 F4 89 11 31 C0 C9 C3 8D 36 55 89 E5 83 ..u...1....6U...
0800:6550 EC 0C 57 56 53 8B 75 08 83 FE 0F 0F 87 B7 00 00 ..WVS.u.........
0800:6560 00 8D 04 76 83 3C C5 84 D4 00 00 00 74 0A B8 07 ...v.<......t...
0800:6570 C0 00 00 E9 11 01 00 00 8B 45 10 83 38 01 75 58 .........E..8.uX
0800:6580 8B 50 04 4A 83 FA 1E 0F 87 8B 00 00 00 8D 1C 76 .P.J...........v
0800:6590 C1 E3 03 BF 7C D4 00 00 8B 4D 10 8B 49 04 89 4C ....|....M..I..L
0800:65A0 1F 08 8B 45 10 8B 40 04 48 89 C1 B8 01 00 00 00 ...E..@.H.......
0800:65B0 D3 E0 89 44 1F 0C 8B 55 10 8B 52 08 89 54 1F 10 ...D...U..R..T..
0800:65C0 8B 0D 48 B3 00 00 8D 83 7C D4 00 00 50 56 8B 51 ..H.....|...PV.Q
0800:65D0 1C FF D2 EB 2F 8D 76 00 8B 4D 10 83 39 02 75 38 ..../.v..M..9.u8
0800:65E0 8D 1C 76 C1 E3 03 BF 7C D4 00 00 C7 44 1F 08 FF ..v....|....D...
0800:65F0 FF FF FF A1 48 B3 00 00 FF 71 08 FF 71 04 56 8B ....H....q..q.V.
0800:6600 50 24 FF D2 89 45 F4 85 C0 74 15 C7 44 1F 08 00 P$...E...t..D...
0800:6610 00 00 00 8B 45 F4 EB 71 B8 0E C0 00 00 EB 6A 90 ....E..q......j.
0800:6620 BF 01 00 00 00 89 F1 D3 E7 89 F8 F7 D0 21 05 F8 .............!..
0800:6630 97 00 00 83 FE 07 7F 1C BA 21 00 00 00 EC 88 C3 .........!......
0800:6640 0F B6 CB 0F A3 F1 73 30 89 F8 F6 D0 20 D8 EE EB ......s0.... ...
0800:6650 36 8D 76 00 BA A1 00 00 00 EC 88 C3 0F B6 CB 8D 6.v.............
0800:6660 7E F8 0F A3 F9 73 11 B8 FE FF FF FF 89 F9 D3 C0 ~....s..........
0800:6670 20 D8 EE EB 12 8D 76 00 B8 01 00 00 00 89 F1 D3  .....v.........
0800:6680 E0 09 05 F8 97 00 00 31 C0 8D 65 E8 5B 5E 5F C9 .......1..e.[^_.
0800:6690 C3 8D 76 00 55 89 E5 83 EC 0C 56 53 8B 5D 08 83 ..v.U.....VS.]..
0800:66A0 FB 0F 77 0D 8D 04 5B 83 3C C5 84 D4 00 00 00 75 ..w...[.<......u
0800:66B0 0B B8 0E C0 00 00 E9 A8 00 00 00 90 A1 F8 97 00 ................
0800:66C0 00 89 D9 D3 F8 A8 01 75 51 83 FB 07 7F 26 BA 21 .......uQ....&.!
0800:66D0 00 00 00 EC 88 45 F4 B8 01 00 00 00 D3 E0 89 45 .....E.........E
0800:66E0 FC 8A 55 FC 0A 55 F4 88 D0 BA 21 00 00 00 EE EB ..U..U....!.....
0800:66F0 29 8D 76 00 BA A1 00 00 00 EC 88 45 F8 8D 73 F8 ).v........E..s.
0800:6700 B8 01 00 00 00 89 F1 D3 E0 89 45 FC 8A 55 FC 0A ..........E..U..
0800:6710 55 F8 88 D0 BA A1 00 00 00 EE 8D 04 5B 8D 34 C5 U...........[.4.
0800:6720 00 00 00 00 83 BE 84 D4 00 00 00 7E 1B A1 48 B3 ...........~..H.
0800:6730 00 00 53 8B 40 20 FF D0 8D 86 7C D4 00 00 50 E8 ..S.@ ....|...P.
0800:6740 40 FA FF FF EB 0D 8D 36 A1 48 B3 00 00 53 8B 40 @......6.H...S.@
0800:6750 28 FF D0 8D 04 5B C7 04 C5 84 D4 00 00 00 00 00 (....[..........
0800:6760 00 31 C0 8D 65 EC 5B 5E C9 C3 8D 36 55 89 E5 53 .1..e.[^...6U..S
0800:6770 31 DB 8D 36 53 E8 1A FF FF FF 83 C4 04 43 83 FB 1..6S........C..
0800:6780 0F 7E F1 8B 5D FC C9 C3 55 89 E5 8B 45 08 FF 30 .~..]...U...E..0
0800:6790 E8 D7 0F 00 00 8D 76 00 55 89 E5 8B 45 08 FF 70 ......v.U...E..p
0800:67A0 08 FF 70 04 8B 00 03 05 1C B3 00 00 50 E8 4A 07 ..p.........P.J.
0800:67B0 00 00 C9 C3 55 89 E5 8B 45 08 FF 30 E8 0F 08 00 ....U...E..0....
0800:67C0 00 C9 C3 90 55 89 E5 8B 45 08 FF 70 08 8B 50 04 ....U...E..p..P.
0800:67D0 03 15 1C B3 00 00 52 FF 30 E8 86 08 00 00 C9 C3 ......R.0.......
0800:67E0 55 89 E5 8B 45 08 FF 70 08 8B 50 04 03 15 1C B3 U...E..p..P.....
0800:67F0 00 00 52 FF 30 E8 1E 09 00 00 C9 C3 55 89 E5 8B ..R.0.......U...
0800:6800 45 08 FF 70 08 FF 70 04 FF 30 E8 99 09 00 00 C9 E..p..p..0......
0800:6810 C3 8D 76 00 55 89 E5 8B 45 08 8B 50 04 03 15 1C ..v.U...E..P....
0800:6820 B3 00 00 52 8B 00 03 05 1C B3 00 00 50 E8 96 A2 ...R........P...
0800:6830 FF FF A3 1C D6 00 00 31 D2 85 C0 74 05 BA FF FF .......1...t....
0800:6840 FF FF 89 D0 C9 C3 8D 36 55 89 E5 83 EC 04 8B 55 .......6U......U
0800:6850 08 8B 0D 48 B3 00 00 8D 45 FC 50 FF 32 8B 41 04 ...H....E.P.2.A.
0800:6860 FF D0 A3 1C D6 00 00 BA FF FF FF FF 85 C0 75 03 ..............u.
0800:6870 8B 55 FC 89 D0 C9 C3 90 55 89 E5 8B 45 08 31 D2 .U......U...E.1.
0800:6880 83 78 04 00 74 09 8B 50 04 03 15 1C B3 00 00 52 .x..t..P.......R
0800:6890 31 D2 83 38 00 74 08 8B 10 03 15 1C B3 00 00 52 1..8.t.........R
0800:68A0 E8 BB 9D FF FF A3 1C D6 00 00 31 D2 85 C0 74 05 ..........1...t.
0800:68B0 BA FF FF FF FF 89 D0 C9 C3 8D 76 00 55 89 E5 8B ..........v.U...
0800:68C0 45 08 8B 00 03 05 1C B3 00 00 50 E8 0C A4 FF FF E.........P.....
0800:68D0 A3 1C D6 00 00 31 D2 85 C0 74 05 BA FF FF FF FF .....1...t......
0800:68E0 89 D0 C9 C3 55 89 E5 8B 55 08 8B 42 04 03 05 1C ....U...U..B....
0800:68F0 B3 00 00 50 FF 32 E8 61 09 00 00 C9 C3 8D 76 00 ...P.2.a......v.
0800:6900 55 89 E5 8B 45 08 8B 00 03 05 1C B3 00 00 50 E8 U...E.........P.
0800:6910 B0 9E FF FF A3 1C D6 00 00 31 D2 85 C0 74 05 BA .........1...t..
0800:6920 FF FF FF FF 89 D0 C9 C3 55 89 E5 8B 45 08 8B 00 ........U...E...
0800:6930 03 05 1C B3 00 00 50 E8 08 9B FF FF A3 1C D6 00 ......P.........
0800:6940 00 31 D2 85 C0 74 05 BA FF FF FF FF 89 D0 C9 C3 .1...t..........
0800:6950 55 89 E5 8B 45 08 8B 15 48 B3 00 00 FF 70 04 FF U...E...H....p..
0800:6960 30 8B 42 0C FF D0 A3 1C D6 00 00 31 D2 85 C0 74 0.B........1...t
0800:6970 05 BA FF FF FF FF 89 D0 C9 C3 8D 36 55 89 E5 8B ...........6U...
0800:6980 45 08 8B 15 48 B3 00 00 FF 70 04 FF 30 8B 42 10 E...H....p..0.B.
0800:6990 FF D0 A3 1C D6 00 00 31 D2 85 C0 74 05 BA FF FF .......1...t....
0800:69A0 FF FF 89 D0 C9 C3 8D 36 55 89 E5 8B 45 08 FF 70 .......6U...E..p
0800:69B0 14 FF 70 10 FF 70 0C FF 70 08 FF 70 04 FF 30 E8 ..p..p..p..p..0.
0800:69C0 2C 09 00 00 C9 C3 8D 36 55 89 E5 31 C0 C9 C3 90 ,......6U..1....
0800:69D0 55 89 E5 8B 45 08 8B 50 08 03 15 1C B3 00 00 52 U...E..P.......R
0800:69E0 FF 70 04 FF 30 E8 62 FB FF FF A3 1C D6 00 00 31 .p..0.b........1
0800:69F0 D2 85 C0 74 05 BA FF FF FF FF 89 D0 C9 C3 8D 36 ...t...........6
0800:6A00 55 89 E5 8B 45 08 FF 30 E8 87 FC FF FF A3 1C D6 U...E..0........
0800:6A10 00 00 31 D2 85 C0 74 05 BA FF FF FF FF 89 D0 C9 ..1...t.........
0800:6A20 C3 8D 76 00 55 89 E5 8B 45 08 FF 70 04 FF 30 E8 ..v.U...E..p..0.
0800:6A30 4C F9 FF FF A3 1C D6 00 00 31 D2 85 C0 74 05 BA L........1...t..
0800:6A40 FF FF FF FF 89 D0 C9 C3 55 89 E5 8B 45 08 31 D2 ........U...E.1.
0800:6A50 83 78 08 00 74 09 8B 50 08 03 15 1C B3 00 00 52 .x..t..P.......R
0800:6A60 31 D2 83 78 04 00 74 09 8B 50 04 03 15 1C B3 00 1..x..t..P......
0800:6A70 00 52 FF 30 E8 9F F9 FF FF A3 1C D6 00 00 31 D2 .R.0..........1.
0800:6A80 85 C0 74 05 BA FF FF FF FF 89 D0 C9 C3 8D 76 00 ..t...........v.
0800:6A90 55 89 E5 8B 45 08 31 D2 83 78 08 00 74 09 8B 50 U...E.1..x..t..P
0800:6AA0 08 03 15 1C B3 00 00 52 31 D2 83 78 04 00 74 09 .......R1..x..t.
0800:6AB0 8B 50 04 03 15 1C B3 00 00 52 FF 30 E8 E3 F9 FF .P.......R.0....
0800:6AC0 FF A3 1C D6 00 00 31 D2 85 C0 74 05 BA FF FF FF ......1...t.....
0800:6AD0 FF 89 D0 C9 C3 8D 76 00 55 89 E5 8B 45 08 8B 00 ......v.U...E...
0800:6AE0 03 05 1C B3 00 00 50 E8 38 FA FF FF A3 1C D6 00 ......P.8.......
0800:6AF0 00 31 D2 85 C0 74 05 BA FF FF FF FF 89 D0 C9 C3 .1...t..........
0800:6B00 55 89 E5 83 EC 04 56 53 8B 45 08 8B 50 08 03 15 U.....VS.E..P...
0800:6B10 9C B2 00 00 8B 58 04 BE FF FF FF FF 89 D9 D3 E6 .....X..........
0800:6B20 21 F2 52 53 FF 30 E8 C5 96 FF FF 85 C0 74 09 2B !.RS.0.......t.+
0800:6B30 05 9C B2 00 00 EB 03 90 31 C0 8D 65 F4 5B 5E C9 ........1..e.[^.
0800:6B40 C3 8D 76 00 55 89 E5 83 EC 04 56 53 8B 45 08 8B ..v.U.....VS.E..
0800:6B50 50 08 03 15 9C B2 00 00 8B 58 04 BE FF FF FF FF P........X......
0800:6B60 89 D9 D3 E6 21 F2 52 53 FF 30 E8 9D 96 FF FF 85 ....!.RS.0......
0800:6B70 C0 74 09 2B 05 9C B2 00 00 EB 03 90 31 C0 8D 65 .t.+........1..e
0800:6B80 F4 5B 5E C9 C3 8D 76 00 55 89 E5 8B 45 08 8B 15 .[^...v.U...E...
0800:6B90 48 B3 00 00 FF 30 8B 42 14 FF D0 A3 1C D6 00 00 H....0.B........
0800:6BA0 31 D2 85 C0 74 05 BA FF FF FF FF 89 D0 C9 C3 90 1...t...........
0800:6BB0 55 89 E5 A1 48 B3 00 00 8B 40 18 FF D0 A3 1C D6 U...H....@......
0800:6BC0 00 00 31 D2 85 C0 74 05 BA FF FF FF FF 89 D0 C9 ..1...t.........
0800:6BD0 C3 00 00 00 55 89 E5 56 53 8B 75 08 31 DB F6 46 ....U..VS.u.1..F
0800:6BE0 3C 03 75 06 F6 46 42 02 74 05 BB 01 00 00 00 56 <.u..FB.t......V
0800:6BF0 68 F3 8A 00 00 E8 AA 0C 00 00 FF 76 24 FF 76 28 h..........v$.v(
0800:6C00 FF 76 20 FF 76 2C 68 10 8B 00 00 E8 94 0C 00 00 .v .v,h.........
0800:6C10 83 C4 1C 85 DB 74 05 8B 46 44 EB 03 8D 46 44 50 .....t..FD...FDP
0800:6C20 FF 76 18 FF 76 10 FF 76 14 68 35 8B 00 00 E8 71 .v..v..v.h5....q
0800:6C30 0C 00 00 FF 76 40 FF 76 38 68 5A 8B 00 00 E8 61 ....v@.v8hZ....a
0800:6C40 0C 00 00 83 C4 20 0F B7 06 50 0F B7 46 04 50 0F ..... ...P..F.P.
0800:6C50 B7 46 08 50 0F B7 46 0C 50 85 DB 74 07 0F B7 46 .F.P..F.P..t...F
0800:6C60 48 EB 09 90 66 8C D0 25 FF FF 00 00 50 0F B7 46 H...f..%....P..F
0800:6C70 3C 50 68 70 8B 00 00 E8 28 0C 00 00 0F B7 46 58 <Php....(.....FX
0800:6C80 50 50 0F B7 46 4C 50 0F B7 46 50 50 68 A1 8B 00 PP..FLP..FPPh...
0800:6C90 00 E8 0E 0C 00 00 83 C4 30 B8 D7 8B 00 00 85 DB ........0.......
0800:6CA0 74 05 B8 D2 8B 00 00 50 FF 76 34 FF 76 30 68 DE t......P.v4.v0h.
0800:6CB0 8B 00 00 E8 EC 0B 00 00 83 C4 10 83 7E 30 0E 75 ............~0.u
0800:6CC0 10 FF 76 1C 68 03 8C 00 00 E8 D6 0B 00 00 83 C4 ..v.h...........
0800:6CD0 08 85 DB 75 2F 31 DB 90 89 D8 83 E0 07 BA 20 00 ...u/1........ .
0800:6CE0 00 00 83 F8 07 75 05 BA 0A 00 00 00 52 FF 74 9E .....u......R.t.
0800:6CF0 44 68 23 8C 00 00 E8 A9 0B 00 00 83 C4 0C 43 83 Dh#...........C.
0800:6D00 FB 1F 7E D4 8D 65 F8 5B 5E C9 C3 00 55 89 E5 53 ..~..e.[^...U..S
0800:6D10 8B 5D 08 6A 01 E8 C6 94 FF FF 53 E8 B4 FE FF FF .].j......S.....
0800:6D20 68 2A 8C 00 00 E8 DA 95 FF FF 8B 5D FC C9 C3 00 h*.........]....
0800:6D30 66 8C D0 66 8E D8 66 8E C0 89 E0 50 E8 CB FF FF f..f..f....P....
0800:6D40 FF 00 00 00 0F A9 0F A1 07 1F 61 83 C4 08 CF 00 ..........a.....
0800:6D50 55 89 E5 8B 55 08 83 BA A0 08 00 00 00 75 1B C7 U...U........u..
0800:6D60 82 A0 08 00 00 10 00 00 00 89 E0 89 82 9C 08 00 ................
0800:6D70 00 66 C7 82 FE 08 00 00 68 00 C9 C3 55 89 E5 53 .f......h...U..S
0800:6D80 8B 5D 08 53 68 42 8C 00 00 E8 16 0B 00 00 0F B7 .].ShB..........
0800:6D90 03 50 68 58 8C 00 00 E8 08 0B 00 00 0F B7 43 08 .PhX..........C.
0800:6DA0 50 FF 73 04 68 68 8C 00 00 E8 F6 0A 00 00 0F B7 P.s.hh..........
0800:6DB0 43 10 50 FF 73 0C 68 7C 8C 00 00 E8 E4 0A 00 00 C.P.s.h|........
0800:6DC0 83 C4 28 0F B7 43 18 50 FF 73 14 68 90 8C 00 00 ..(..C.P.s.h....
0800:6DD0 E8 CF 0A 00 00 FF 73 1C 68 A4 8C 00 00 E8 C2 0A ......s.h.......
0800:6DE0 00 00 FF 73 24 FF 73 20 68 AE 8C 00 00 E8 B2 0A ...s$.s h.......
0800:6DF0 00 00 83 C4 20 FF 73 30 FF 73 2C FF 73 34 FF 73 .... .s0.s,.s4.s
0800:6E00 28 68 C4 8C 00 00 E8 99 0A 00 00 FF 73 38 FF 73 (h..........s8.s
0800:6E10 3C FF 73 44 FF 73 40 68 E9 8C 00 00 E8 83 0A 00 <.sD.s@h........
0800:6E20 00 83 C4 28 0F B7 43 5C 50 0F B7 43 58 50 0F B7 ...(..C\P..CXP..
0800:6E30 43 48 50 0F B7 43 54 50 0F B7 43 50 50 0F B7 43 CHP..CTP..CPP..C
0800:6E40 4C 50 68 0E 8D 00 00 E8 58 0A 00 00 0F B7 43 60 LPh.....X.....C`
0800:6E50 50 68 3F 8D 00 00 E8 49 0A 00 00 83 C4 24 0F B7 Ph?....I.....$..
0800:6E60 43 64 50 68 49 8D 00 00 E8 37 0A 00 00 0F B7 43 CdPhI....7.....C
0800:6E70 66 50 68 5A 8D 00 00 E8 28 0A 00 00 8B 5D FC C9 fPhZ....(....]..
0800:6E80 C3 00 00 00 55 89 E5 C7 05 FC 97 00 00 01 00 00 ....U...........
0800:6E90 00 C7 05 00 98 00 00 00 00 00 00 C7 05 08 98 00 ................
0800:6EA0 00 01 00 00 00 C7 05 0C 98 00 00 01 00 00 00 C7 ................
0800:6EB0 05 14 98 00 00 01 00 00 00 C7 05 18 98 00 00 01 ................
0800:6EC0 00 00 00 C9 C3 8D 76 00 55 89 E5 8B 4D 08 31 D2 ......v.U...M.1.
0800:6ED0 39 15 00 91 00 00 76 1D 8D 04 52 83 3C 85 FC 97 9.....v...R.<...
0800:6EE0 00 00 00 75 07 89 11 31 C0 C9 C3 90 42 39 15 00 ...u...1....B9..
0800:6EF0 91 00 00 77 E3 B8 11 C0 00 00 C9 C3 55 89 E5 83 ...w........U...
0800:6F00 EC 08 57 56 53 8B 5D 08 66 8B 4D 10 66 89 4D F8 ..WVS.].f.M.f.M.
0800:6F10 8D 45 FC 50 E8 AF FF FF FF A3 1C D6 00 00 83 C4 .E.P............
0800:6F20 04 85 C0 0F 85 83 00 00 00 89 DE BF 6C 8D 00 00 ............l...
0800:6F30 B9 09 00 00 00 FC A8 00 F3 A6 75 14 8B 45 FC 8D ..........u..E..
0800:6F40 14 40 C7 04 95 FC 97 00 00 03 00 00 00 EB 76 90 .@............v.
0800:6F50 8B 45 FC 8D 04 40 8D 04 85 00 98 00 00 50 0F B7 .E...@.......P..
0800:6F60 45 F8 50 FF 75 0C 53 E8 DC 98 FF FF A3 1C D6 00 E.P.u.S.........
0800:6F70 00 83 C4 10 85 C0 75 14 8B 45 FC 8D 14 40 C7 04 ......u..E...@..
0800:6F80 95 FC 97 00 00 01 00 00 00 EB 3A 90 8B 45 FC 8D ..........:..E..
0800:6F90 04 40 8D 04 85 00 98 00 00 50 0F B7 45 F8 50 FF .@.......P..E.P.
0800:6FA0 75 0C 53 E8 A0 B3 FF FF 85 C0 74 08 B8 FF FF FF u.S.......t.....
0800:6FB0 FF EB 12 90 8B 45 FC 8D 14 40 C7 04 95 FC 97 00 .....E...@......
0800:6FC0 00 02 00 00 00 8D 65 EC 5B 5E 5F C9 C3 8D 76 00 ......e.[^_...v.
0800:6FD0 55 89 E5 53 8B 5D 08 39 1D 00 91 00 00 76 0D 8D U..S.].9.....v..
0800:6FE0 04 5B 83 3C 85 FC 97 00 00 00 75 14 C7 05 1C D6 .[.<......u.....
0800:6FF0 00 00 06 C0 00 00 B8 FF FF FF FF EB 5F 8D 76 00 ............_.v.
0800:7000 8D 04 5B 8B 04 85 FC 97 00 00 83 F8 01 74 09 83 ..[..........t..
0800:7010 F8 02 74 18 EB 36 8D 36 8D 04 5B FF 34 85 00 98 ..t..6.6..[.4...
0800:7020 00 00 E8 1D 95 FF FF EB 13 8D 76 00 8D 04 5B 8D ..........v...[.
0800:7030 04 85 00 98 00 00 50 E8 6C B3 FF FF A3 1C D6 00 ......P.l.......
0800:7040 00 85 C0 74 07 B8 FF FF FF FF EB 10 8D 04 5B C7 ...t..........[.
0800:7050 04 85 FC 97 00 00 00 00 00 00 31 C0 8B 5D FC C9 ..........1..]..
0800:7060 C3 8D 76 00 55 89 E5 83 EC 08 53 8B 55 08 8B 5D ..v.U.....S.U..]
0800:7070 0C 8B 4D 10 39 15 00 91 00 00 76 0D 8D 04 52 83 ..M.9.....v...R.
0800:7080 3C 85 FC 97 00 00 00 75 0F C7 05 1C D6 00 00 06 <......u........
0800:7090 C0 00 00 EB 79 8D 76 00 8D 04 52 8B 04 85 FC 97 ....y.v...R.....
0800:70A0 00 00 83 F8 01 74 09 83 F8 02 74 2C EB 56 8D 36 .....t....t,.V.6
0800:70B0 8D 45 FC 50 51 53 8D 04 52 FF 34 85 00 98 00 00 .E.PQS..R.4.....
0800:70C0 E8 3B 99 FF FF A3 1C D6 00 00 BA FF FF FF FF 85 .;..............
0800:70D0 C0 75 2C 8B 55 FC EB 27 8D 45 F8 50 51 53 8D 04 .u,.U..'.E.PQS..
0800:70E0 52 8D 04 85 00 98 00 00 50 E8 C2 B2 FF FF A3 1C R.......P.......
0800:70F0 D6 00 00 BA FF FF FF FF 85 C0 75 03 8B 55 F8 89 ..........u..U..
0800:7100 D0 EB 10 90 C7 05 1C D6 00 00 0E C0 00 00 B8 FF ................
0800:7110 FF FF FF 8B 5D F4 C9 C3 55 89 E5 83 EC 04 8B 55 ....]...U......U
0800:7120 08 8B 4D 10 39 15 00 91 00 00 76 0D 8D 04 52 83 ..M.9.....v...R.
0800:7130 3C 85 FC 97 00 00 00 75 13 C7 05 1C D6 00 00 06 <......u........
0800:7140 C0 00 00 B8 FF FF FF FF C9 C3 8D 36 8D 04 52 8B ...........6..R.
0800:7150 04 85 FC 97 00 00 83 F8 01 74 09 83 F8 04 74 30 .........t....t0
0800:7160 EB 32 8D 36 8D 45 FC 50 51 FF 75 0C 8D 04 52 FF .2.6.E.PQ.u...R.
0800:7170 34 85 00 98 00 00 E8 E5 9B FF FF A3 1C D6 00 00 4...............
0800:7180 BA FF FF FF FF 85 C0 75 03 8B 55 FC 89 D0 C9 C3 .......u..U.....
0800:7190 89 C8 C9 C3 C7 05 1C D6 00 00 0E C0 00 00 B8 FF ................
0800:71A0 FF FF FF C9 C3 8D 76 00 55 89 E5 83 EC 08 53 8B ......v.U.....S.
0800:71B0 55 08 8B 5D 0C 8B 4D 10 39 15 00 91 00 00 76 0D U..]..M.9.....v.
0800:71C0 8D 04 52 83 3C 85 FC 97 00 00 00 75 0F C7 05 1C ..R.<......u....
0800:71D0 D6 00 00 06 C0 00 00 EB 79 8D 76 00 8D 04 52 8B ........y.v...R.
0800:71E0 04 85 FC 97 00 00 83 F8 01 74 09 83 F8 02 74 2C .........t....t,
0800:71F0 EB 56 8D 36 8D 45 FC 50 51 53 8D 04 52 FF 34 85 .V.6.E.PQS..R.4.
0800:7200 00 98 00 00 E8 03 9A FF FF A3 1C D6 00 00 BA FF ................
0800:7210 FF FF FF 85 C0 75 2C 8B 55 FC EB 27 8D 45 F8 50 .....u,.U..'.E.P
0800:7220 51 53 8D 04 52 8D 04 85 00 98 00 00 50 E8 3E B2 QS..R.......P.>.
0800:7230 FF FF A3 1C D6 00 00 BA FF FF FF FF 85 C0 75 03 ..............u.
0800:7240 8B 55 F8 89 D0 EB 10 90 C7 05 1C D6 00 00 0E C0 .U..............
0800:7250 00 00 B8 FF FF FF FF 8B 5D F4 C9 C3 55 89 E5 8B ........]...U...
0800:7260 55 08 8B 4D 0C 39 15 00 91 00 00 76 0D 8D 04 52 U..M.9.....v...R
0800:7270 83 3C 85 FC 97 00 00 00 75 12 C7 05 1C D6 00 00 .<......u.......
0800:7280 06 C0 00 00 B8 FF FF FF FF C9 C3 90 8D 04 52 8B ..............R.
0800:7290 04 85 FC 97 00 00 83 F8 01 74 09 83 F8 02 74 28 .........t....t(
0800:72A0 EB 3A 8D 36 51 8D 04 52 FF 34 85 00 98 00 00 E8 .:.6Q..R.4......
0800:72B0 D0 92 FF FF A3 1C D6 00 00 31 D2 85 C0 74 05 BA .........1...t..
0800:72C0 FF FF FF FF 89 D0 C9 C3 51 8D 04 52 8D 04 85 00 ........Q..R....
0800:72D0 98 00 00 50 E8 F3 B1 FF FF EB D9 90 C7 05 1C D6 ...P............
0800:72E0 00 00 0E C0 00 00 B8 FF FF FF FF C9 C3 8D 76 00 ..............v.
0800:72F0 55 89 E5 8B 55 18 39 15 00 91 00 00 76 0D 8D 04 U...U.9.....v...
0800:7300 52 83 3C 85 FC 97 00 00 00 75 11 C7 05 1C D6 00 R.<......u......
0800:7310 00 06 C0 00 00 B8 FF FF FF FF C9 C3 8D 04 52 83 ..............R.
0800:7320 3C 85 FC 97 00 00 03 75 2B A1 48 B3 00 00 8D 4D <......u+.H....M
0800:7330 08 51 FF 75 0C FF 75 1C 8B 40 08 FF D0 A3 1C D6 .Q.u..u..@......
0800:7340 00 00 BA FF FF FF FF 85 C0 75 03 8B 55 08 89 D0 .........u..U...
0800:7350 C9 C3 8D 36 C7 05 1C D6 00 00 0E C0 00 00 B8 FF ...6............
0800:7360 FF FF FF C9 C3 00 00 00 55 89 E5 57 56 53 8B 7D ........U..WVS.}
0800:7370 0C 8B 75 10 8B 5D 14 53 56 57 FF 75 08 E8 2A 00 ..u..].SVW.u..*.
0800:7380 00 00 83 C4 10 3D 70 17 00 00 75 17 53 56 57 FF .....=p...u.SVW.
0800:7390 75 08 E8 CD 01 00 00 3D 70 17 00 00 75 05 B8 70 u......=p...u..p
0800:73A0 17 00 00 8D 65 F4 5B 5E 5F C9 C3 00 55 89 E5 81 ....e.[^_...U...
0800:73B0 EC 28 01 00 00 57 56 53 8B 75 08 8D 45 DC 50 6A .(...WVS.u..E.Pj
0800:73C0 20 8D 45 E0 50 6A 00 FF 75 10 FF D6 83 C4 14 85  .E.Pj..u.......
0800:73D0 C0 0F 85 7F 01 00 00 83 7D DC 20 0F 85 F0 00 00 ........}. .....
0800:73E0 00 0F B7 45 E0 3D 07 01 00 00 74 28 7F 12 3D CC ...E.=....t(..=.
0800:73F0 00 00 00 0F 84 BB 00 00 00 E9 CA 00 00 00 8D 36 ...............6
0800:7400 3D 08 01 00 00 74 29 3D 0B 01 00 00 74 3A E9 B5 =....t)=....t:..
0800:7410 00 00 00 90 C7 85 D8 FE FF FF 00 00 00 00 31 F6 ..............1.
0800:7420 BB 20 00 00 00 8B 7D E4 03 7D E8 E9 BA 00 00 00 . ....}..}......
0800:7430 C7 85 D8 FE FF FF 00 00 00 00 8B 75 E4 BB 20 00 ...........u.. .
0800:7440 00 00 E9 A0 00 00 00 90 31 DB 8D 36 84 DB 75 3E ........1..6..u>
0800:7450 8D 45 DC 50 68 00 01 00 00 8D 85 DC FE FF FF 50 .E.Ph..........P
0800:7460 53 FF 75 10 FF D6 83 C4 14 85 C0 0F 85 E5 00 00 S.u.............
0800:7470 00 81 7D DC FF 00 00 00 77 0B 8B 45 DC C6 84 28 ..}.....w..E...(
0800:7480 DC FE FF FF FF 85 DB 75 05 BB 20 00 00 00 0F B6 .......u.. .....
0800:7490 C3 80 BC 28 DC FE FF FF 00 75 05 43 EB AE 8D 36 ...(.....u.C...6
0800:74A0 80 E3 E0 8B 55 F4 89 95 D8 FE FF FF 8B 75 E4 EB ....U........u..
0800:74B0 36 8D 76 00 C7 85 D8 FE FF FF 00 10 00 00 31 DB 6.v...........1.
0800:74C0 8B 75 E4 EB 22 8D 76 00 81 7D E0 00 86 01 0B 74 .u..".v..}.....t
0800:74D0 07 B8 70 17 00 00 EB 7E C7 85 D8 FE FF FF 00 10 ..p....~........
0800:74E0 00 00 8B 75 E4 31 DB 8B 7D E8 8B 85 D8 FE FF FF ...u.1..}.......
0800:74F0 01 F0 25 FF 0F 00 00 74 0A 39 F0 76 02 89 F0 29 ..%....t.9.v...)
0800:7500 C6 01 C7 85 F6 74 1D 68 05 03 00 00 56 FF B5 D8 .....t.h....V...
0800:7510 FE FF FF 56 53 FF 75 10 8B 55 0C FF D2 83 C4 18 ...VS.u..U......
0800:7520 85 C0 75 32 89 F8 03 45 EC 74 20 68 07 03 00 00 ..u2...E.t h....
0800:7530 50 8B 85 D8 FE FF FF 01 F0 50 57 8D 04 1E 50 FF P........PW...P.
0800:7540 75 10 8B 55 0C FF D2 85 C0 75 0B 8B 45 F4 8B 55 u..U.....u..E..U
0800:7550 14 89 42 04 31 C0 8D A5 CC FE FF FF 5B 5E 5F C9 ..B.1.......[^_.
0800:7560 C3 00 00 00 55 89 E5 83 EC 38 57 56 53 8B 7D 10 ....U....8WVS.}.
0800:7570 8B 5D 14 8D 45 C8 50 6A 34 8D 45 CC 50 6A 00 57 .]..E.Pj4.E.Pj.W
0800:7580 8B 4D 08 FF D1 83 C4 14 85 C0 0F 85 D1 00 00 00 .M..............
0800:7590 83 7D C8 33 76 09 81 7D CC 7F 45 4C 46 74 0D B8 .}.3v..}..ELFt..
0800:75A0 70 17 00 00 E9 B8 00 00 00 8D 76 00 66 81 7D D0 p.........v.f.}.
0800:75B0 01 01 75 07 66 83 7D DE 03 74 0D B8 71 17 00 00 ..u.f.}..t..q...
0800:75C0 E9 9C 00 00 00 8D 76 00 8B 45 E4 89 43 04 0F B7 ......v..E..C...
0800:75D0 5D F8 0F B7 45 F6 0F AF D8 8D 43 03 24 FC 29 C4 ]...E.....C.$.).
0800:75E0 89 E6 8D 45 C8 50 53 56 FF 75 E8 57 8B 4D 08 FF ...E.PSV.u.W.M..
0800:75F0 D1 83 C4 14 85 C0 75 69 39 5D C8 73 07 B8 72 17 ......ui9].s..r.
0800:7600 00 00 EB 5D 31 DB 66 83 7D F8 00 74 52 8D 76 00 ...]1.f.}..tR.v.
0800:7610 0F B7 45 F6 0F AF C3 01 F0 83 38 01 75 38 BA 00 ..E.......8.u8..
0800:7620 03 00 00 F6 40 18 04 74 05 BA 01 03 00 00 F6 40 ....@..t.......@
0800:7630 18 02 74 03 80 CA 02 F6 40 18 01 74 03 80 CA 04 ..t.....@..t....
0800:7640 52 FF 70 14 FF 70 08 FF 70 10 FF 70 04 57 8B 4D R.p..p..p..p.W.M
0800:7650 0C FF D1 83 C4 18 43 0F B7 45 F8 39 C3 7C B1 31 ......C..E.9.|.1
0800:7660 C0 8D 65 BC 5B 5E 5F C9 C3 00 00 00 55 89 E5 57 ..e.[^_.....U..W
0800:7670 56 8B 75 08 8B 7D 0C 8B 55 10 FC 8D 04 16 39 C7 V.u..}..U.....9.
0800:7680 73 20 8D 04 17 39 C6 73 19 39 F7 72 15 74 0C 01 s ...9.s.9.r.t..
0800:7690 D6 4E 01 D7 4F FD 89 D1 F3 FC A4 8B 45 08 5E 5F .N..O.......E.^_
0800:76A0 C9 C3 FC 89 D1 C1 F9 02 78 09 F3 A5 89 D1 83 E1 ........x.......
0800:76B0 03 F3 A4 8B 45 08 5E 5F C9 C3 8D 36 55 89 E5 57 ....E.^_...6U..W
0800:76C0 56 8B 7D 08 8B 75 0C EB AE 00 00 00 55 89 E5 57 V.}..u......U..W
0800:76D0 8B 7D 08 8A 45 0C 88 C4 89 C2 C1 E0 10 66 89 D0 .}..E........f..
0800:76E0 8B 55 10 FC 89 D1 C1 E9 02 F3 AB 89 D1 83 E1 03 .U..............
0800:76F0 F3 AA 8B 45 08 5F C9 C3 55 89 E5 57 8B 7D 08 8B ...E._..U..W.}..
0800:7700 55 0C 31 C0 EB DD 00 00 55 89 E5 8B 55 08 83 3D U.1.....U...U..=
0800:7710 04 91 00 00 1F 7F 19 A1 04 91 00 00 89 14 85 20 ............... 
0800:7720 D6 00 00 FF 05 04 91 00 00 31 C0 C9 C3 8D 76 00 .........1....v.
0800:7730 B8 FF FF FF FF C9 C3 90 55 89 E5 FF 75 08 E8 C5 ........U...u...
0800:7740 FF FF FF C9 C3 8D 76 00 55 89 E5 53 8B 1D 04 91 ......v.U..S....
0800:7750 00 00 4B 78 0F 8D 76 00 8B 04 9D 20 D6 00 00 FF ..Kx..v.... ....
0800:7760 D0 4B 79 F4 8B 5D FC C9 C3 8D 76 00 55 89 E5 53 .Ky..]....v.U..S
0800:7770 8B 5D 08 E8 D0 FF FF FF 53 E8 9E AD FF FF 8D 36 .]......S......6
0800:7780 55 89 E5 53 8B 5D 08 83 C3 04 8D 36 6A 00 53 68 U..S.].....6j.Sh
0800:7790 08 91 00 00 E8 23 03 00 00 83 C4 0C 85 C0 74 08 .....#........t.
0800:77A0 89 18 83 C0 04 EB 10 90 53 E8 0E 00 00 00 83 C4 ........S.......
0800:77B0 04 85 C0 75 D7 31 C0 8B 5D FC C9 C3 55 89 E5 31 ...u.1..]...U..1
0800:77C0 C0 C9 C3 00 55 89 E5 56 53 8B 75 08 31 DB 39 9E ....U..VS.u.1.9.
0800:77D0 80 00 00 00 76 18 8D 36 0F BE 04 33 50 E8 7A DF ....v..6...3P.z.
0800:77E0 FF FF 83 C4 04 43 39 9E 80 00 00 00 77 EA C7 86 .....C9.....w...
0800:77F0 80 00 00 00 00 00 00 00 8D 65 F8 5B 5E C9 C3 90 .........e.[^...
0800:7800 55 89 E5 56 53 8B 75 0C 8B 5D 08 83 FE 0A 75 1C U..VS.u..]....u.
0800:7810 8B 83 80 00 00 00 C6 04 18 00 53 E8 90 DF FF FF ..........S.....
0800:7820 C7 83 80 00 00 00 00 00 00 00 EB 2D 85 F6 74 09 ...........-..t.
0800:7830 83 BB 80 00 00 00 7F 76 0F 53 E8 85 FF FF FF 56 .......v.S.....V
0800:7840 E8 17 DF FF FF EB 12 90 8B 83 80 00 00 00 89 F2 ................
0800:7850 88 14 18 FF 83 80 00 00 00 8D 65 F8 5B 5E C9 C3 ..........e.[^..
0800:7860 55 89 E5 81 EC 84 00 00 00 53 C7 45 FC 00 00 00 U........S.E....
0800:7870 00 8D 9D 7C FF FF FF 53 68 00 78 00 00 6A 00 FF ...|...Sh.x..j..
0800:7880 75 0C FF 75 08 E8 5E 04 00 00 83 C4 14 83 7D FC u..u..^.......}.
0800:7890 00 74 06 53 E8 2B FF FF FF 8B 9D 78 FF FF FF C9 .t.S.+.....x....
0800:78A0 C3 8D 76 00 55 89 E5 8D 45 0C 50 FF 75 08 E8 AD ..v.U...E.P.u...
0800:78B0 FF FF FF C9 C3 00 00 00 55 89 E5 8B 45 08 8B 10 ........U...E...
0800:78C0 0F B6 12 FF 00 89 D0 C9 C3 8D 76 00 55 89 E5 8B ..........v.U...
0800:78D0 45 0C FF 08 C9 C3 8D 36 55 89 E5 8D 55 08 52 68 E......6U...U.Rh
0800:78E0 CC 78 00 00 68 B8 78 00 00 FF 75 10 FF 75 0C E8 .x..h.x...u..u..
0800:78F0 38 0B 00 00 C9 C3 8D 36 55 89 E5 8D 45 10 50 FF 8......6U...E.P.
0800:7900 75 0C FF 75 08 E8 CE FF FF FF C9 C3 55 89 E5 8B u..u........U...
0800:7910 55 08 8B 4D 0C 8D 76 00 0F BE 02 39 C8 75 05 89 U..M..v....9.u..
0800:7920 D0 C9 C3 90 80 3A 00 74 03 42 EB EC 31 C0 C9 C3 .....:.t.B..1...
0800:7930 55 89 E5 53 8B 5D 08 8B 4D 0C EB 08 85 C0 74 10 U..S.]..M.....t.
0800:7940 39 C2 75 0C 0F B6 13 43 0F B6 01 41 85 D2 75 EC 9.u....C...A..u.
0800:7950 29 C2 89 D0 8B 5D FC C9 C3 00 00 00 55 89 E5 53 )....]......U..S
0800:7960 8B 4D 0C 8B 5D 08 8A 01 88 03 41 8D 53 01 84 C0 .M..].....A.S...
0800:7970 74 0C 8D 36 8A 01 88 02 41 42 84 C0 75 F6 89 D8 t..6....AB..u...
0800:7980 8B 5D FC C9 C3 00 00 00 55 89 E5 83 EC 04 57 56 .]......U.....WV
0800:7990 53 8B 75 08 8B 7D 10 31 DB 8D 76 00 8A 06 31 D2 S.u..}.1..v...1.
0800:79A0 3C 20 74 14 3C 0C 74 10 3C 0A 74 0C 3C 0D 74 08 < t.<.t.<.t.<.t.
0800:79B0 3C 09 74 04 3C 0B 75 05 BA 01 00 00 00 85 D2 74 <.t.<.u........t
0800:79C0 03 46 EB D8 8A 16 88 D0 04 D0 3C 09 77 1A 0F BE .F........<.w...
0800:79D0 CA 89 4D FC 89 C8 83 C0 D0 39 F8 7D 0B 89 D8 0F ..M......9.}....
0800:79E0 AF C7 8D 5C 01 D0 EB 3D 88 D0 04 9F 3C 19 77 1C ...\...=....<.w.
0800:79F0 0F BE CA 89 4D FC 89 C8 83 C0 A9 39 F8 7D 0D 89 ....M......9.}..
0800:7A00 D8 0F AF C7 8D 5C 01 A9 EB 1B 8D 36 88 D0 04 BF .....\.....6....
0800:7A10 3C 19 77 14 0F BE D2 8D 42 C9 39 F8 7D 0A 0F AF <.w.....B.9.}...
0800:7A20 DF 8D 5C 1A C9 46 EB 9C 83 7D 0C 00 74 05 8B 4D ..\..F...}..t..M
0800:7A30 0C 89 31 89 D8 8D 65 F0 5B 5E 5F C9 C3 00 00 00 ..1...e.[^_.....
0800:7A40 55 89 E5 57 56 53 8B 45 0C 8B 75 18 89 C2 03 55 U..WVS.E..u....U
0800:7A50 10 83 C0 07 24 F8 80 E2 F8 39 C2 72 56 89 D3 29 ....$....9.rV..)
0800:7A60 C3 83 FB 1F 76 4D 89 C1 8D 79 18 89 79 04 C7 41 ....vM...y..y..A
0800:7A70 18 00 00 00 00 8B 41 04 29 C2 89 50 04 8B 7D 14 ......A.)..P..}.
0800:7A80 89 79 08 89 71 0C 89 59 10 8B 41 04 8B 40 04 89 .y..q..Y..A..@..
0800:7A90 41 14 8B 5D 08 EB 0B 90 8B 42 10 39 41 10 73 0F A..].....B.9A.s.
0800:7AA0 89 D3 8B 13 85 D2 74 07 39 72 0C 7F F3 74 E9 89 ......t.9r...t..
0800:7AB0 11 89 0B 8D 65 F4 5B 5E 5F C9 C3 00 55 89 E5 83 ....e.[^_...U...
0800:7AC0 EC 04 57 56 53 8B 45 08 8B 55 10 8B 75 0C 83 C6 ..WVS.E..U..u...
0800:7AD0 07 83 E6 F8 8B 18 85 DB 74 59 8D 36 8B 43 08 F7 ........tY.6.C..
0800:7AE0 D0 85 D0 75 48 8D 7B 04 89 7D FC 8B 4B 04 85 C9 ...uH.{..}..K...
0800:7AF0 74 3B 8D 36 39 71 04 72 2B 76 19 8D 14 0E 8B 01 t;.69q.r+v......
0800:7B00 89 02 8B 79 04 29 F7 89 7A 04 8B 7D FC 89 17 EB ...y.)..z..}....
0800:7B10 0A 8D 76 00 8B 01 8B 7D FC 89 07 29 73 14 89 C8 ..v....}...)s...
0800:7B20 EB 13 8D 36 89 4D FC 8B 09 85 C9 75 C7 8B 1B 85 ...6.M.....u....
0800:7B30 DB 75 A9 31 C0 8D 65 F0 5B 5E 5F C9 C3 00 00 00 .u.1..e.[^_.....
0800:7B40 55 89 E5 6A FF 6A 00 FF 75 18 FF 75 14 FF 75 10 U..j.j..u..u..u.
0800:7B50 FF 75 0C FF 75 08 E8 05 00 00 00 C9 C3 00 00 00 .u..u...........
0800:7B60 55 89 E5 83 EC 10 57 56 53 8B 45 08 8B 7D 0C 8B U.....WVS.E..}..
0800:7B70 75 1C 03 75 20 89 75 FC 8B 00 89 45 F8 85 C0 0F u..u .u....E....
0800:7B80 84 10 01 00 00 8D 76 00 8B 75 F8 8B 46 08 F7 D0 ......v..u..F...
0800:7B90 85 45 10 0F 85 EC 00 00 00 8B 75 FC 39 75 F8 0F .E........u.9u..
0800:7BA0 83 E0 00 00 00 8B 45 F8 03 40 10 39 45 1C 0F 83 ......E..@.9E...
0800:7BB0 D1 00 00 00 8B 75 F8 83 C6 04 89 75 F4 8B 75 F8 .....u.....u..u.
0800:7BC0 8B 5E 04 85 DB 0F 84 BA 00 00 00 90 39 7B 04 0F .^..........9{..
0800:7BD0 82 A3 00 00 00 89 5D F0 39 5D 1C 76 06 8B 75 1C ......].9].v..u.
0800:7BE0 89 75 F0 31 C9 39 4D 14 7E 1C 8D 36 BA 01 00 00 .u.1.9M.~..6....
0800:7BF0 00 D3 E2 8B 45 18 33 45 F0 85 C2 74 03 01 55 F0 ....E.3E...t..U.
0800:7C00 41 39 4D 14 7F E6 8B 45 F0 29 D8 01 F8 39 43 04 A9M....E.)...9C.
0800:7C10 72 66 8B 45 F0 01 F8 39 45 FC 72 69 8B 4D F0 80 rf.E...9E.ri.M..
0800:7C20 E1 F8 39 D9 76 16 89 C8 29 D8 8B 13 89 11 8B 53 ..9.v...)......S
0800:7C30 04 29 C2 89 51 04 89 43 04 89 5D F4 8B 45 F0 83 .)..Q..C..]..E..
0800:7C40 E0 07 8D 7C 38 07 83 E7 F8 39 79 04 76 16 8D 14 ...|8....9y.v...
0800:7C50 0F 8B 01 89 02 8B 49 04 29 F9 89 4A 04 8B 75 F4 ......I.)..J..u.
0800:7C60 89 16 EB 07 8B 01 8B 75 F4 89 06 8B 75 F8 29 7E .......u....u.)~
0800:7C70 14 8B 45 F0 EB 21 8D 36 89 5D F4 8B 1B 85 DB 0F ..E..!.6.]......
0800:7C80 85 47 FF FF FF 8B 75 F8 8B 36 89 75 F8 85 F6 0F .G....u..6.u....
0800:7C90 85 F3 FE FF FF 31 C0 8D 65 E4 5B 5E 5F C9 C3 00 .....1..e.[^_...
0800:7CA0 55 89 E5 83 EC 20 57 56 53 8B 45 08 8B 75 0C 8D U.... WVS.E..u..
0800:7CB0 5D FF 8D 36 31 D2 F7 F6 89 C1 8A 82 0C 91 00 00 ]..61...........
0800:7CC0 88 03 4B 89 C8 85 C0 75 EB EB 10 90 0F BE 03 50 ..K....u.......P
0800:7CD0 FF 75 14 8B 7D 10 FF D7 83 C4 08 43 39 EB 75 EC .u..}......C9.u.
0800:7CE0 8D 65 D4 5B 5E 5F C9 C3 55 89 E5 83 EC 4C 57 56 .e.[^_..U....LWV
0800:7CF0 53 E9 1F 07 00 00 8D 36 8B 55 08 80 3A 25 74 18 S......6.U..:%t.
0800:7D00 0F BE 02 50 42 89 55 08 FF 75 18 8B 55 14 FF D2 ...PB.U..u..U...
0800:7D10 83 C4 08 E9 FD 06 00 00 FF 45 08 31 F6 C7 45 DC .........E.1..E.
0800:7D20 FF FF FF FF C7 45 D8 00 00 00 00 C6 45 D4 20 31 .....E......E. 1
0800:7D30 C9 C7 45 CC 00 00 00 00 31 FF 8D 36 8B 45 08 80 ..E.....1..6.E..
0800:7D40 38 23 75 08 BF 01 00 00 00 EB 26 90 8B 55 08 80 8#u.......&..U..
0800:7D50 3A 2D 75 10 C7 45 D8 01 00 00 00 42 89 55 08 EB :-u..E.....B.U..
0800:7D60 DB 8D 76 00 8B 45 08 80 38 2B 75 0C B9 2B 00 00 ..v..E..8+u..+..
0800:7D70 00 40 89 45 08 EB C5 90 8B 55 08 80 3A 20 75 10 .@.E.....U..: u.
0800:7D80 85 C9 75 05 B9 20 00 00 00 FF 45 08 EB AE 8D 36 ..u.. ....E....6
0800:7D90 8B 45 08 80 38 30 75 08 C6 45 D4 30 40 89 45 08 .E..80u..E.0@.E.
0800:7DA0 8B 55 08 8A 12 80 C2 D0 80 FA 09 77 27 8D 76 00 .U.........w'.v.
0800:7DB0 8D 04 F6 8D 74 06 D0 89 75 BC 8B 55 08 0F BE 12 ....t...u..U....
0800:7DC0 01 D6 FF 45 08 8B 45 08 8A 00 04 D0 3C 09 76 E0 ...E..E.....<.v.
0800:7DD0 EB 22 8D 36 8B 55 08 80 3A 2A 75 18 83 45 0C 04 .".6.U..:*u..E..
0800:7DE0 8B 45 0C 8B 70 FC 42 89 55 08 85 F6 7D 06 80 75 .E..p.B.U...}..u
0800:7DF0 D8 01 F7 DE 8B 55 08 80 3A 2E 75 55 42 89 55 08 .....U..:.uUB.U.
0800:7E00 8A 02 04 D0 3C 09 77 30 C7 45 DC 00 00 00 00 90 ....<.w0.E......
0800:7E10 8B 55 DC 8D 14 D2 8B 45 DC 8D 44 10 D0 8B 55 08 .U.....E..D...U.
0800:7E20 0F BE 12 01 C2 89 55 DC FF 45 08 8B 45 08 8A 00 ......U..E..E...
0800:7E30 04 D0 3C 09 76 DA EB 19 8B 55 08 80 3A 2A 75 11 ..<.v....U..:*u.
0800:7E40 83 45 0C 04 8B 45 0C 8B 40 FC 89 45 DC 42 89 55 .E...E..@..E.B.U
0800:7E50 08 8B 55 08 80 3A 6C 75 04 42 89 55 08 8B 45 08 ..U..:lu.B.U..E.
0800:7E60 0F BE 18 83 FB 7A 0F 87 94 05 00 00 FF 24 9D 74 .....z.......$.t
0800:7E70 7E 00 00 90 F8 83 00 00 00 84 00 00 00 84 00 00 ~...............
0800:7E80 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7E90 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7EA0 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7EB0 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7EC0 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7ED0 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7EE0 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7EF0 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7F00 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7F10 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7F20 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7F30 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7F40 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7F50 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7F60 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7F70 00 84 00 00 00 84 00 00 00 84 00 00 60 80 00 00 ............`...
0800:7F80 00 84 00 00 64 82 00 00 00 84 00 00 00 84 00 00 ....d...........
0800:7F90 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7FA0 00 84 00 00 00 84 00 00 00 84 00 00 9C 82 00 00 ................
0800:7FB0 58 82 00 00 00 84 00 00 00 84 00 00 94 82 00 00 X...............
0800:7FC0 00 84 00 00 00 84 00 00 70 82 00 00 00 84 00 00 ........p.......
0800:7FD0 00 84 00 00 7C 82 00 00 00 84 00 00 88 82 00 00 ....|...........
0800:7FE0 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:7FF0 00 84 00 00 00 84 00 00 00 84 00 00 60 80 00 00 ............`...
0800:8000 9C 81 00 00 64 82 00 00 00 84 00 00 00 84 00 00 ....d...........
0800:8010 00 84 00 00 00 84 00 00 00 84 00 00 00 84 00 00 ................
0800:8020 00 84 00 00 00 84 00 00 00 84 00 00 9C 82 00 00 ................
0800:8030 58 82 00 00 00 84 00 00 00 84 00 00 94 82 00 00 X...............
0800:8040 AC 81 00 00 00 84 00 00 70 82 00 00 00 84 00 00 ........p.......
0800:8050 00 84 00 00 7C 82 00 00 00 84 00 00 88 82 00 00 ....|...........
0800:8060 83 45 0C 04 8B 55 0C 8B 52 FC 89 55 D0 83 45 0C .E...U..R..U..E.
0800:8070 04 8B 45 0C 8B 58 FC 0F BE 13 89 55 C8 43 FF 75 ..E..X.....U.C.u
0800:8080 18 FF 75 14 52 FF 75 D0 E8 13 FC FF FF 83 C4 10 ..u.R.u.........
0800:8090 83 7D D0 00 0F 84 78 03 00 00 C7 45 C4 00 00 00 .}....x....E....
0800:80A0 00 E9 D8 00 00 00 8D 36 80 3B 20 7F 73 83 7D C4 .......6.; .s.}.
0800:80B0 00 74 0D 6A 2C FF 75 18 8B 45 14 FF D0 EB 12 90 .t.j,.u..E......
0800:80C0 6A 3C FF 75 18 8B 55 14 FF D2 C7 45 C4 01 00 00 j<.u..U....E....
0800:80D0 00 83 C4 08 0F BE 3B EB 15 8D 76 00 0F BE C9 89 ......;...v.....
0800:80E0 4D BC 51 FF 75 18 8B 45 14 FF D0 83 C4 08 43 8A M.Q.u..E......C.
0800:80F0 0B 80 F9 20 7F E6 FF 75 18 FF 75 14 FF 75 C8 8D ... ...u..u..u..
0800:8100 4F FF 8B 55 D0 D3 EA 89 F1 29 F9 B8 02 00 00 00 O..U.....)......
0800:8110 D3 E0 48 21 C2 52 E8 85 FB FF FF 83 C4 10 EB 5E ..H!.R.........^
0800:8120 4E 89 75 BC 8B 55 D0 0F A3 F2 73 4D 83 7D C4 00 N.u..U....sM.}..
0800:8130 74 0E 6A 2C FF 75 18 8B 45 14 FF D0 EB 13 8D 36 t.j,.u..E......6
0800:8140 6A 3C FF 75 18 8B 55 14 FF D2 C7 45 C4 01 00 00 j<.u..U....E....
0800:8150 00 83 C4 08 8A 0B 80 F9 20 7E 23 90 0F BE C9 89 ........ ~#.....
0800:8160 4D BC 51 FF 75 18 8B 45 14 FF D0 83 C4 08 43 8A M.Q.u..E......C.
0800:8170 0B 80 F9 20 7F E6 EB 06 43 80 3B 20 7F FA 0F BE ... ....C.; ....
0800:8180 33 43 85 F6 0F 85 1E FF FF FF 83 7D C4 00 0F 84 3C.........}....
0800:8190 7E 02 00 00 6A 3E E9 6C 02 00 00 90 83 45 0C 04 ~...j>.l.....E..
0800:81A0 8B 45 0C 0F BE 40 FC E9 5A 02 00 00 83 7D DC FF .E...@..Z....}..
0800:81B0 75 07 C7 45 DC FF FF FF 7F 83 45 0C 04 8B 45 0C u..E......E...E.
0800:81C0 8B 78 FC 85 FF 75 05 BF 75 8D 00 00 85 F6 7E 36 .x...u..u.....~6
0800:81D0 83 7D D8 00 75 30 31 DB 89 F9 80 3F 00 74 0D 90 .}..u01....?.t..
0800:81E0 39 5D DC 7E 07 43 47 80 3F 00 75 F4 89 CF 39 F3 9].~.CG.?.u...9.
0800:81F0 7D 14 8D 36 6A 20 FF 75 18 8B 55 14 FF D2 43 83 }..6j .u..U...C.
0800:8200 C4 08 39 F3 7C EE 31 DB 80 3F 00 74 1E 8D 76 00 ..9.|.1..?.t..v.
0800:8210 43 39 5D DC 7C 15 0F BE 07 50 47 FF 75 18 8B 55 C9].|....PG.u..U
0800:8220 14 FF D2 83 C4 08 80 3F 00 75 E5 39 F3 0F 8D DF .......?.u.9....
0800:8230 01 00 00 83 7D D8 00 0F 84 D5 01 00 00 8D 76 00 ....}.........v.
0800:8240 6A 20 FF 75 18 8B 45 14 FF D0 43 83 C4 08 39 F3 j .u..E...C...9.
0800:8250 7C EE E9 BB 01 00 00 90 C7 45 C8 08 00 00 00 EB |........E......
0800:8260 6B 8D 76 00 C7 45 C8 0A 00 00 00 EB 37 8D 76 00 k.v..E......7.v.
0800:8270 C7 45 C8 0A 00 00 00 EB 53 8D 76 00 C7 45 C8 10 .E......S.v..E..
0800:8280 00 00 00 EB 47 8D 76 00 C7 45 C8 10 00 00 00 EB ....G.v..E......
0800:8290 13 8D 76 00 8B 55 10 89 55 C8 EB 08 8B 45 10 89 ..v..U..U....E..
0800:82A0 45 C8 EB 28 83 45 0C 04 8B 55 0C 8B 5A FC 85 DB E..(.E...U..Z...
0800:82B0 7C 0A 89 5D D0 89 4D CC EB 1F 8D 36 F7 DB 89 5D |..]..M....6...]
0800:82C0 D0 C7 45 CC 2D 00 00 00 EB 0F 8D 36 83 45 0C 04 ..E.-......6.E..
0800:82D0 8B 45 0C 8B 40 FC 89 45 D0 8D 55 FF 89 55 C0 31 .E..@..E..U..U.1
0800:82E0 DB 83 7D D0 00 74 1D 85 FF 74 19 83 7D C8 08 75 ..}..t...t..}..u
0800:82F0 07 BB 76 8D 00 00 EB 0C 83 7D C8 10 75 06 BB 78 ..v......}..u..x
0800:8300 8D 00 00 90 8B 45 D0 31 D2 F7 75 C8 89 D7 89 C1 .....E.1..u.....
0800:8310 8A 87 24 91 00 00 8B 55 C0 88 02 4A 89 55 C0 89 ..$....U...J.U..
0800:8320 4D D0 85 C9 75 DE 89 D0 F7 D0 01 E8 29 C6 83 7D M...u.......)..}
0800:8330 CC 00 74 01 4E 85 DB 74 13 30 C0 89 DF FC B9 FF ..t.N..t.0......
0800:8340 FF FF FF F2 AE 89 C8 F7 D0 48 29 C6 80 7D D4 20 .........H)..}. 
0800:8350 75 1A 83 7D D8 00 75 14 4E 78 11 90 6A 20 FF 75 u..}..u.Nx..j .u
0800:8360 18 8B 45 14 FF D0 83 C4 08 4E 79 F0 83 7D CC 00 ..E......Ny..}..
0800:8370 74 0E FF 75 CC FF 75 18 8B 55 14 FF D2 83 C4 08 t..u..u..U......
0800:8380 85 DB 74 1D 80 3B 00 74 18 8D 76 00 0F BE 03 50 ..t..;.t..v....P
0800:8390 43 FF 75 18 8B 55 14 FF D2 83 C4 08 80 3B 00 75 C.u..U.......;.u
0800:83A0 EB 80 7D D4 30 75 2B 4E 78 28 8D 36 6A 30 FF 75 ..}.0u+Nx(.6j0.u
0800:83B0 18 8B 45 14 FF D0 83 C4 08 4E 79 F0 EB 14 8D 36 ..E......Ny....6
0800:83C0 8B 55 C0 0F BE 12 52 FF 75 18 8B 45 14 FF D0 83 .U....R.u..E....
0800:83D0 C4 08 FF 45 C0 39 6D C0 75 E6 83 7D D8 00 74 32 ...E.9m.u..}..t2
0800:83E0 4E 78 2F 90 6A 20 FF 75 18 8B 55 14 FF D2 83 C4 Nx/.j .u..U.....
0800:83F0 08 4E 79 F0 EB 1C 8D 36 FF 4D 08 EB 15 8D 76 00 .Ny....6.M....v.
0800:8400 8B 45 08 0F BE 00 50 FF 75 18 8B 55 14 FF D2 83 .E....P.u..U....
0800:8410 C4 08 FF 45 08 8B 45 08 80 38 00 0F 85 D7 F8 FF ...E..E..8......
0800:8420 FF 8D 65 A8 5B 5E 5F C9 C3 00 00 00 55 89 E5 83 ..e.[^_.....U...
0800:8430 EC 0C 57 56 53 C7 45 F8 00 00 00 00 E9 30 02 00 ..WVS.E......0..
0800:8440 00 8D 76 00 83 FB 25 0F 84 83 00 00 00 88 D8 31 ..v...%........1
0800:8450 D2 3C 20 74 14 3C 0C 74 10 3C 0A 74 0C 3C 0D 74 .< t.<.t.<.t.<.t
0800:8460 08 3C 09 74 04 3C 0B 75 05 BA 01 00 00 00 85 D2 .<.t.<.u........
0800:8470 74 46 8D 36 FF 75 18 8B 4D 10 FF D1 89 C3 88 D8 tF.6.u..M.......
0800:8480 83 C4 04 31 D2 3C 20 74 14 3C 0C 74 10 3C 0A 74 ...1.< t.<.t.<.t
0800:8490 0C 3C 0D 74 08 3C 09 74 04 3C 0B 75 05 BA 01 00 .<.t.<.t.<.u....
0800:84A0 00 00 85 D2 75 CE FF 75 18 53 8B 4D 14 FF D1 83 ....u..u.S.M....
0800:84B0 C4 08 E9 BA 01 00 00 90 FF 75 18 8B 4D 10 FF D1 .........u..M...
0800:84C0 83 C4 04 39 C3 0F 84 A6 01 00 00 E9 B3 01 00 00 ...9............
0800:84D0 C7 45 FC 00 00 00 00 8B 4D 08 0F B6 19 41 89 4D .E......M....A.M
0800:84E0 08 83 FB 73 0F 84 FE 00 00 00 7F 14 83 FB 2A 0F ...s..........*.
0800:84F0 84 67 01 00 00 83 FB 64 74 12 E9 69 01 00 00 90 .g.....dt..i....
0800:8500 83 FB 78 74 4B E9 5E 01 00 00 8D 36 31 F6 FF 75 ..xtK.^....61..u
0800:8510 18 8B 4D 10 FF D1 89 C3 83 C4 04 83 FB 2D 0F 94 ..M..........-..
0800:8520 C0 0F B6 F8 85 FF 74 1B EB 0C 8D 36 8D 04 F6 8D ......t....6....
0800:8530 44 06 D0 8D 34 03 FF 75 18 8B 4D 10 FF D1 89 C3 D...4..u..M.....
0800:8540 83 C4 04 8D 43 D0 83 F8 09 76 E1 EB 6B 8D 76 00 ....C....v..k.v.
0800:8550 31 F6 FF 75 18 8B 4D 10 FF D1 89 C3 83 C4 04 83 1..u..M.........
0800:8560 FB 2D 0F 94 C0 0F B6 F8 85 FF 74 10 FF 75 18 8B .-........t..u..
0800:8570 4D 10 FF D1 89 C3 83 C4 04 8D 76 00 8D 43 D0 83 M.........v..C..
0800:8580 F8 09 77 0C 89 F0 C1 E0 04 8D 74 03 D0 EB DD 90 ..w.......t.....
0800:8590 8D 43 9F 83 F8 05 77 0C 89 F0 C1 E0 04 8D 74 03 .C....w.......t.
0800:85A0 A9 EB C9 90 8D 43 BF 83 F8 05 77 0C 89 F0 C1 E0 .....C....w.....
0800:85B0 04 8D 74 03 C9 EB B5 90 FF 75 18 53 8B 4D 14 FF ..t......u.S.M..
0800:85C0 D1 83 C4 08 85 FF 74 02 F7 DE 83 7D FC 00 0F 85 ......t....}....
0800:85D0 9D 00 00 00 83 45 0C 04 8B 4D 0C 8B 41 FC 89 30 .....E...M..A..0
0800:85E0 E9 83 00 00 00 8D 76 00 83 7D FC 00 75 0D 83 45 ......v..}..u..E
0800:85F0 0C 04 8B 4D 0C 8B 49 FC 89 4D F4 FF 75 18 8B 4D ...M..I..M..u..M
0800:8600 10 FF D1 89 C3 83 C4 04 88 D8 31 D2 3C 20 74 14 ..........1.< t.
0800:8610 3C 0C 74 10 3C 0A 74 0C 3C 0D 74 08 3C 09 74 04 <.t.<.t.<.t.<.t.
0800:8620 3C 0B 75 05 BA 01 00 00 00 85 D2 75 13 83 7D FC <.u........u..}.
0800:8630 00 75 C8 88 D8 8B 4D F4 88 01 41 EB BB 8D 76 00 .u....M...A...v.
0800:8640 FF 75 18 53 8B 4D 14 FF D1 83 C4 08 83 7D FC 00 .u.S.M.......}..
0800:8650 75 1F 8B 4D F4 C6 01 00 EB 0E 8D 36 C7 45 FC 01 u..M.......6.E..
0800:8660 00 00 00 E9 6F FE FF FF 83 7D FC 00 75 03 FF 45 ....o....}..u..E
0800:8670 F8 8B 4D 08 0F B6 19 41 89 4D 08 85 DB 0F 85 C1 ..M....A.M......
0800:8680 FD FF FF 8B 45 F8 8D 65 E8 5B 5E 5F C9 C3 00 00 ....E..e.[^_....
0800:8690 4E 6F 74 20 65 6E 6F 75 67 68 20 6D 65 6D 6F 72 Not enough memor
0800:86A0 79 2E 00 00 44 50 4D 49 20 65 72 72 6F 72 20 25 y...DPMI error %
0800:86B0 30 34 78 00 64 70 6D 69 5F 70 72 6F 67 5F 6D 75 04x.dpmi_prog_mu
0800:86C0 6E 6C 6F 63 6B 00 8D 36 5C 0F 00 00 68 13 00 00 nlock..6\...h...
0800:86D0 34 14 00 00 78 15 00 00 CC 15 00 00 20 16 00 00 4...x....... ...
0800:86E0 78 16 00 00 88 16 00 00 8C 17 00 00 94 16 00 00 x...............
0800:86F0 8C 17 00 00 44 50 4D 49 20 68 6F 73 74 20 64 6F ....DPMI host do
0800:8700 65 73 6E 27 74 20 73 75 70 70 6F 72 74 20 33 32 esn't support 32
0800:8710 2D 62 69 74 20 70 72 6F 67 72 61 6D 73 21 00 4E -bit programs!.N
0800:8720 6F 74 20 65 6E 6F 75 67 68 20 44 4F 53 20 6D 65 ot enough DOS me
0800:8730 6D 6F 72 79 20 66 6F 72 20 44 50 4D 49 20 68 6F mory for DPMI ho
0800:8740 73 74 20 64 61 74 61 20 61 72 65 61 00 44 50 4D st data area.DPM
0800:8750 49 20 65 72 72 6F 72 00 20 25 64 20 25 2A 64 20 I error. %d %*d 
0800:8760 25 64 00 65 66 73 5F 69 6E 69 74 3A 20 6F 75 74 %d.efs_init: out
0800:8770 20 6F 66 20 6D 65 6D 6F 72 79 00 56 44 49 53 4B  of memory.VDISK
0800:8780 20 56 00 44 4F 53 20 33 2E 30 30 20 6F 72 20 68  V.DOS 3.00 or h
0800:8790 69 67 68 65 72 20 72 65 71 75 69 72 65 64 2E 00 igher required..
0800:87A0 54 68 65 20 70 72 6F 63 65 73 73 6F 72 20 69 73 The processor is
0800:87B0 20 69 6E 20 61 6E 20 75 6E 6B 6E 6F 77 6E 20 70  in an unknown p
0800:87C0 72 6F 74 65 63 74 65 64 20 6D 6F 64 65 20 65 6E rotected mode en
0800:87D0 76 69 72 6F 6E 6D 65 6E 74 2E 00 45 4D 4D 58 58 vironment..EMMXX
0800:87E0 58 58 30 00 45 4D 4D 51 58 58 58 30 00 24 4D 4D XX0.EMMQXXX0.$MM
0800:87F0 58 58 58 58 30 00 6E 6F 74 20 65 6E 6F 75 67 68 XXXX0.not enough
0800:8800 20 6C 6F 77 20 44 4F 53 20 6D 65 6D 6F 72 79 20  low DOS memory 
0800:8810 61 76 61 69 6C 61 62 6C 65 00 58 4D 53 20 65 72 available.XMS er
0800:8820 72 6F 72 3A 20 63 61 6E 27 74 20 65 6E 61 62 6C ror: can't enabl
0800:8830 65 20 41 32 30 20 6C 69 6E 65 00 58 4D 53 20 65 e A20 line.XMS e
0800:8840 72 72 6F 72 3A 20 63 61 6E 27 74 20 64 69 73 61 rror: can't disa
0800:8850 62 6C 65 20 41 32 30 20 6C 69 6E 65 00 58 4D 53 ble A20 line.XMS
0800:8860 20 65 72 72 6F 72 3A 20 63 61 6E 27 74 20 61 6C  error: can't al
0800:8870 6C 6F 63 61 74 65 20 65 78 74 65 6E 64 65 64 20 locate extended 
0800:8880 6D 65 6D 6F 72 79 00 58 4D 53 20 65 72 72 6F 72 memory.XMS error
0800:8890 3A 20 63 61 6E 27 74 20 6C 6F 63 6B 20 64 6F 77 : can't lock dow
0800:88A0 6E 20 65 78 74 65 6E 64 65 64 20 6D 65 6D 6F 72 n extended memor
0800:88B0 79 00 58 4D 53 20 65 72 72 6F 72 3A 20 63 61 6E y.XMS error: can
0800:88C0 27 74 20 75 6E 6C 6F 63 6B 20 65 78 74 65 6E 64 't unlock extend
0800:88D0 65 64 20 6D 65 6D 6F 72 79 00 58 4D 53 20 65 72 ed memory.XMS er
0800:88E0 72 6F 72 3A 20 63 61 6E 27 74 20 66 72 65 65 20 ror: can't free 
0800:88F0 65 78 74 65 6E 64 65 64 20 6D 65 6D 6F 72 79 00 extended memory.
0800:8900 6E 6F 74 20 65 6E 6F 75 67 68 20 6D 65 6D 6F 72 not enough memor
0800:8910 79 00 61 2E 6F 75 74 00 6E 6F 74 20 65 6E 6F 75 y.a.out.not enou
0800:8920 67 68 20 6C 6F 77 20 6D 65 6D 6F 72 79 20 66 6F gh low memory fo
0800:8930 72 20 44 4F 53 20 62 6F 75 6E 63 65 20 62 75 66 r DOS bounce buf
0800:8940 66 65 72 00 63 61 6E 27 74 20 6F 70 65 6E 20 70 fer.can't open p
0800:8950 72 6F 67 72 61 6D 20 66 69 6C 65 20 60 25 73 27 rogram file `%s'
0800:8960 20 28 65 72 72 6F 72 20 30 78 25 78 29 00 63 61  (error 0x%x).ca
0800:8970 6E 27 74 20 65 78 65 63 75 74 65 20 70 72 6F 67 n't execute prog
0800:8980 72 61 6D 20 60 25 73 27 20 28 65 72 72 6F 72 20 ram `%s' (error 
0800:8990 30 78 25 78 29 00 65 72 72 6F 72 20 73 65 65 6B 0x%x).error seek
0800:89A0 69 6E 67 20 69 6E 20 70 72 6F 67 72 61 6D 20 66 ing in program f
0800:89B0 69 6C 65 20 60 25 73 27 00 65 72 72 6F 72 20 72 ile `%s'.error r
0800:89C0 65 61 64 69 6E 67 20 70 72 6F 67 72 61 6D 20 66 eading program f
0800:89D0 69 6C 65 20 60 25 73 27 00 00 00 00 72 61 77 5F ile `%s'....raw_
0800:89E0 70 72 6F 67 5F 72 75 6E 00 75 6E 6C 6F 63 6B 69 prog_run.unlocki
0800:89F0 6E 67 20 6C 61 20 25 30 38 78 0A 00 72 61 77 5F ng la %08x..raw_
0800:8A00 70 72 6F 67 5F 6D 75 6E 6C 6F 63 6B 61 6C 6C 00 prog_munlockall.
0800:8A10 88 58 00 00 40 59 00 00 8C 59 00 00 14 5A 00 00 .X..@Y...Y...Z..
0800:8A20 88 5A 00 00 00 5B 00 00 78 5B 00 00 88 5B 00 00 .Z...[..x[...[..
0800:8A30 54 5C 00 00 E0 5C 00 00 54 5C 00 00 63 6F 6E 74 T\...\..T\..cont
0800:8A40 65 6E 74 73 20 6F 66 20 72 65 61 6C 5F 63 61 6C ents of real_cal
0800:8A50 6C 5F 64 61 74 61 3A 0A 00 65 61 78 20 25 30 38 l_data:..eax %08
0800:8A60 78 20 20 65 62 78 20 25 30 38 78 20 20 65 63 78 x  ebx %08x  ecx
0800:8A70 20 25 30 38 78 20 20 65 64 78 20 25 30 38 78 0A  %08x  edx %08x.
0800:8A80 00 65 73 69 20 25 30 38 78 20 20 65 64 69 20 25 .esi %08x  edi %
0800:8A90 30 38 78 20 20 65 62 70 20 25 30 38 78 0A 00 73 08x  ebp %08x..s
0800:8AA0 70 20 25 30 34 78 20 20 73 73 20 25 30 34 78 20 p %04x  ss %04x 
0800:8AB0 20 66 6C 61 67 73 20 25 30 34 78 0A 00 47 44 42  flags %04x..GDB
0800:8AC0 20 74 6F 6C 64 20 75 73 20 74 6F 20 74 61 6B 65  told us to take
0800:8AD0 20 73 69 67 6E 61 6C 20 25 64 21 0A 00 55 6E 65  signal %d!..Une
0800:8AE0 78 70 65 63 74 65 64 20 73 69 67 6E 61 6C 20 25 xpected signal %
0800:8AF0 64 0A 00 44 75 6D 70 20 6F 66 20 74 72 61 70 5F d..Dump of trap_
0800:8B00 73 74 61 74 65 20 61 74 20 25 30 38 78 3A 0A 00 state at %08x:..
0800:8B10 45 41 58 20 25 30 38 78 20 45 42 58 20 25 30 38 EAX %08x EBX %08
0800:8B20 78 20 45 43 58 20 25 30 38 78 20 45 44 58 20 25 x ECX %08x EDX %
0800:8B30 30 38 78 0A 00 45 53 49 20 25 30 38 78 20 45 44 08x..ESI %08x ED
0800:8B40 49 20 25 30 38 78 20 45 42 50 20 25 30 38 78 20 I %08x EBP %08x 
0800:8B50 45 53 50 20 25 30 38 78 0A 00 45 49 50 20 25 30 ESP %08x..EIP %0
0800:8B60 38 78 20 45 46 4C 41 47 53 20 25 30 38 78 0A 00 8x EFLAGS %08x..
0800:8B70 43 53 20 25 30 34 78 20 53 53 20 25 30 34 78 20 CS %04x SS %04x 
0800:8B80 44 53 20 25 30 34 78 20 45 53 20 25 30 34 78 20 DS %04x ES %04x 
0800:8B90 46 53 20 25 30 34 78 20 47 53 20 25 30 34 78 0A FS %04x GS %04x.
0800:8BA0 00 76 38 36 3A 20 20 20 20 20 20 20 20 20 20 20 .v86:           
0800:8BB0 20 44 53 20 25 30 34 78 20 45 53 20 25 30 34 78  DS %04x ES %04x
0800:8BC0 20 46 53 20 25 30 34 78 20 47 53 20 25 30 34 78  FS %04x GS %04x
0800:8BD0 0A 00 75 73 65 72 00 6B 65 72 6E 65 6C 00 74 72 ..user.kernel.tr
0800:8BE0 61 70 6E 6F 20 25 64 2C 20 65 72 72 6F 72 20 25 apno %d, error %
0800:8BF0 30 38 78 2C 20 66 72 6F 6D 20 25 73 20 6D 6F 64 08x, from %s mod
0800:8C00 65 0A 00 70 61 67 65 20 66 61 75 6C 74 20 6C 69 e..page fault li
0800:8C10 6E 65 61 72 20 61 64 64 72 65 73 73 20 25 30 38 near address %08
0800:8C20 78 0A 00 25 30 38 78 25 63 00 74 65 72 6D 69 6E x..%08x%c.termin
0800:8C30 61 74 65 64 20 64 75 65 20 74 6F 20 74 72 61 70 ated due to trap
0800:8C40 0A 00 44 75 6D 70 20 6F 66 20 54 53 53 20 61 74 ..Dump of TSS at
0800:8C50 20 25 30 38 78 3A 0A 00 62 61 63 6B 5F 6C 69 6E  %08x:..back_lin
0800:8C60 6B 20 25 30 34 78 0A 00 45 53 50 30 20 25 30 38 k %04x..ESP0 %08
0800:8C70 78 20 53 53 30 20 25 30 34 78 0A 00 45 53 50 31 x SS0 %04x..ESP1
0800:8C80 20 25 30 38 78 20 53 53 31 20 25 30 34 78 0A 00  %08x SS1 %04x..
0800:8C90 45 53 50 32 20 25 30 38 78 20 53 53 32 20 25 30 ESP2 %08x SS2 %0
0800:8CA0 34 78 0A 00 43 52 33 20 25 30 38 78 0A 00 45 49 4x..CR3 %08x..EI
0800:8CB0 50 20 25 30 38 78 20 45 46 4C 41 47 53 20 25 30 P %08x EFLAGS %0
0800:8CC0 38 78 0A 00 45 41 58 20 25 30 38 78 20 45 42 58 8x..EAX %08x EBX
0800:8CD0 20 25 30 38 78 20 45 43 58 20 25 30 38 78 20 45  %08x ECX %08x E
0800:8CE0 44 58 20 25 30 38 78 0A 00 45 53 49 20 25 30 38 DX %08x..ESI %08
0800:8CF0 78 20 45 44 49 20 25 30 38 78 20 45 42 50 20 25 x EDI %08x EBP %
0800:8D00 30 38 78 20 45 53 50 20 25 30 38 78 0A 00 43 53 08x ESP %08x..CS
0800:8D10 20 25 30 34 78 20 53 53 20 25 30 34 78 20 44 53  %04x SS %04x DS
0800:8D20 20 25 30 34 78 20 45 53 20 25 30 34 78 20 46 53  %04x ES %04x FS
0800:8D30 20 25 30 34 78 20 47 53 20 25 30 34 78 0A 00 4C  %04x GS %04x..L
0800:8D40 44 54 20 25 30 34 78 0A 00 74 72 61 63 65 5F 74 DT %04x..trace_t
0800:8D50 72 61 70 20 25 30 34 78 0A 00 49 4F 50 42 20 6F rap %04x..IOPB o
0800:8D60 66 66 73 65 74 20 25 30 34 78 0A 00 2F 64 65 76 ffset %04x../dev
0800:8D70 2F 6D 65 6D 00 00 30 00 30 78 00 00 1F 00 00 00 /mem..0.0x......
0800:8D80 1C 00 00 00 1F 00 00 00 1E 00 00 00 1F 00 00 00 ................
0800:8D90 1E 00 00 00 1F 00 00 00 1F 00 00 00 1E 00 00 00 ................
0800:8DA0 1F 00 00 00 1E 00 00 00 1F 00 00 00 EA 00 00 00 ................
0800:8DB0 00 00 00 00 CC 1E 00 00 DC 1E 00 00 EC 1E 00 00 ................
0800:8DC0 FC 1E 00 00 0C 1F 00 00 1C 1F 00 00 2C 1F 00 00 ............,...
0800:8DD0 3C 1F 00 00 4C 1F 00 00 64 1F 00 00 78 1F 00 00 <...L...d...x...
0800:8DE0 8C 1F 00 00 A0 1F 00 00 B4 1F 00 00 C8 1F 00 00 ................
0800:8DF0 DC 1F 00 00 68 28 00 00 EC 28 00 00 34 30 00 00 ....h(...(..40..
0800:8E00 70 2F 00 00 00 00 00 00 50 00 00 00 80 3E 00 00 p/......P....>..
0800:8E10 00 00 0E 00 8C 3E 00 00 01 00 0E 00 98 3E 00 00 .....>.......>..
0800:8E20 03 00 6E 00 A4 3E 00 00 04 00 6E 00 B0 3E 00 00 ..n..>....n..>..
0800:8E30 05 00 6E 00 BC 3E 00 00 06 00 0E 00 C8 3E 00 00 ..n..>.......>..
0800:8E40 07 00 0E 00 D4 3E 00 00 08 00 0E 00 E0 3E 00 00 .....>.......>..
0800:8E50 09 00 0E 00 EC 3E 00 00 0A 00 0E 00 F4 3E 00 00 .....>.......>..
0800:8E60 0B 00 0E 00 FC 3E 00 00 0C 00 0E 00 F4 41 00 00 .....>.......A..
0800:8E70 0D 00 0E 00 98 41 00 00 0E 00 0E 00 04 3F 00 00 .....A.......?..
0800:8E80 0F 00 0E 00 10 3F 00 00 10 00 0E 00 1C 3F 00 00 .....?.......?..
0800:8E90 11 00 0E 00 28 3F 00 00 12 00 0E 00 34 3F 00 00 ....(?......4?..
0800:8EA0 13 00 0E 00 40 3F 00 00 14 00 0E 00 4C 3F 00 00 ....@?......L?..
0800:8EB0 15 00 0E 00 58 3F 00 00 16 00 0E 00 64 3F 00 00 ....X?......d?..
0800:8EC0 17 00 0E 00 70 3F 00 00 18 00 0E 00 7C 3F 00 00 ....p?......|?..
0800:8ED0 19 00 0E 00 88 3F 00 00 1A 00 0E 00 94 3F 00 00 .....?.......?..
0800:8EE0 1B 00 0E 00 A0 3F 00 00 1C 00 0E 00 AC 3F 00 00 .....?.......?..
0800:8EF0 1D 00 0E 00 B8 3F 00 00 1E 00 0E 00 C4 3F 00 00 .....?.......?..
0800:8F00 1F 00 0E 00 00 00 00 00 1D 44 00 00 23 44 00 00 .........D..#D..
0800:8F10 29 44 00 00 2F 44 00 00 35 44 00 00 3B 44 00 00 )D../D..5D..;D..
0800:8F20 41 44 00 00 47 44 00 00 4D 44 00 00 53 44 00 00 AD..GD..MD..SD..
0800:8F30 59 44 00 00 5F 44 00 00 65 44 00 00 6B 44 00 00 YD.._D..eD..kD..
0800:8F40 71 44 00 00 77 44 00 00 D8 44 00 00 34 45 00 00 qD..wD...D..4E..
0800:8F50 90 45 00 00 EC 45 00 00 48 46 00 00 A4 46 00 00 .E...E..HF...F..
0800:8F60 00 47 00 00 5C 47 00 00 B8 47 00 00 20 48 00 00 .G..\G...G.. H..
0800:8F70 88 48 00 00 F0 48 00 00 58 49 00 00 C0 49 00 00 .H...H..XI...I..
0800:8F80 28 4A 00 00 90 4A 00 00 58 4B 00 00 80 4B 00 00 (J...J..XK...K..
0800:8F90 A8 4B 00 00 D0 4B 00 00 F8 4B 00 00 20 4C 00 00 .K...K...K.. L..
0800:8FA0 48 4C 00 00 70 4C 00 00 98 4C 00 00 D0 4C 00 00 HL..pL...L...L..
0800:8FB0 08 4D 00 00 40 4D 00 00 78 4D 00 00 B0 4D 00 00 .M..@M..xM...M..
0800:8FC0 E8 4D 00 00 18 4E 00 00 FC 44 00 00 58 45 00 00 .M...N...D..XE..
0800:8FD0 B4 45 00 00 10 46 00 00 6C 46 00 00 C8 46 00 00 .E...F..lF...F..
0800:8FE0 24 47 00 00 80 47 00 00 E0 47 00 00 48 48 00 00 $G...G...G..HH..
0800:8FF0 B0 48 00 00 18 49 00 00 80 49 00 00 E8 49 00 00 .H...I...I...I..
0800:9000 50 4A 00 00 B8 4A 00 00 68 4B 00 00 90 4B 00 00 PJ...J..hK...K..
0800:9010 B8 4B 00 00 E0 4B 00 00 08 4C 00 00 30 4C 00 00 .K...K...L..0L..
0800:9020 58 4C 00 00 80 4C 00 00 B0 4C 00 00 E8 4C 00 00 XL...L...L...L..
0800:9030 20 4D 00 00 58 4D 00 00 90 4D 00 00 C8 4D 00 00  M..XM...M...M..
0800:9040 FC 4D 00 00 2C 4E 00 00 28 45 00 00 84 45 00 00 .M..,N..(E...E..
0800:9050 E0 45 00 00 3C 46 00 00 98 46 00 00 F4 46 00 00 .E..<F...F...F..
0800:9060 50 47 00 00 AC 47 00 00 14 48 00 00 7C 48 00 00 PG...G...H..|H..
0800:9070 E4 48 00 00 4C 49 00 00 B4 49 00 00 1C 4A 00 00 .H..LI...I...J..
0800:9080 84 4A 00 00 E8 4A 00 00 12 89 00 00 68 58 00 00 .J...J......hX..
0800:9090 C4 5F 00 00 88 67 00 00 98 67 00 00 B4 67 00 00 ._...g...g...g..
0800:90A0 C4 67 00 00 E0 67 00 00 FC 67 00 00 14 68 00 00 .g...g...g...h..
0800:90B0 48 68 00 00 78 68 00 00 BC 68 00 00 E4 68 00 00 Hh..xh...h...h..
0800:90C0 50 69 00 00 7C 69 00 00 A8 69 00 00 C8 69 00 00 Pi..|i...i...i..
0800:90D0 D0 69 00 00 00 6A 00 00 24 6A 00 00 48 6A 00 00 .i...j..$j..Hj..
0800:90E0 90 6A 00 00 D8 6A 00 00 00 6B 00 00 44 6B 00 00 .j...j...k..Dk..
0800:90F0 88 6B 00 00 B0 6B 00 00 00 69 00 00 28 69 00 00 .k...k...i..(i..
0800:9100 14 00 00 00 00 00 00 00 00 00 00 00 30 31 32 33 ............0123
0800:9110 34 35 36 37 38 39 61 62 63 64 65 66 00 00 00 00 456789abcdef....
0800:9120 00 00 00 00 30 31 32 33 34 35 36 37 38 39 61 62 ....0123456789ab
0800:9130 63 64 65 66 00 00 00 00 00 00 00 00 00 00 00 00 cdef............
0800:9140 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:9150 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:9160 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:9170 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:9180 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:9190 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:91A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:91B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:91C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:91D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:91E0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0800:91F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
