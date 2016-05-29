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

fn0800_0150()
	mov	es,cs:[025A]
	push	si
	push	di
	mov	si,2714
	mov	di,2714
	call	01E6
	pop	di
	pop	si
	ret	

fn0800_0163()
	ret	

fn0800_0164()
	mov	bp,sp
	mov	ah,4C
	mov	al,[bp+02]
	int	21
0800:016D                                        B9 0E 00              ...
0800:0170 BA 2F 00 E9 D5 00 1E B8 00 35 CD 21 89 1E 5B 00 ./.......5.!..[.
0800:0180 8C 06 5D 00 B8 04 35 CD 21 89 1E 5F 00 8C 06 61 ..]...5.!.._...a
0800:0190 00 B8 05 35 CD 21 89 1E 63 00 8C 06 65 00 B8 06 ...5.!..c...e...
0800:01A0 35 CD 21 89 1E 67 00 8C 06 69 00 B8 00 25 8C CA 5.!..g...i...%..
0800:01B0 8E DA BA 6D 01 CD 21 1F C3                      ...m..!..      

fn0800_01B9()
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

fn0800_01E6()
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
0800:023D                                        B4 40 BB              .@.
0800:0240 02 00 CD 21 C3 B9 1E 00 BA 3D 00 2E 8E 1E 5A 02 ...!.....=....Z.
0800:0250 E8 EA FF B8 03 00 50 E8 0F 89 00 00 03 40       ......P......@ 

fn0800_025E()
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
Code vector at 0800:03F0 (18 bytes)
	0800:0321
	0800:0330
	0800:0330
	0800:0326
	0800:0326
	0800:032B
	0800:0335
	0800:0335
	0800:0335
0800:03F0 21 03 30 03 30 03 26 03 26 03 2B 03 35 03 35 03 !.0.0.&.&.+.5.5.
0800:0400 35 03                                           5.             

fn0800_0402()
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
	repne	
	scasb	

l0800_04E6:
	not	cx
	mov	ax,002E
	dec	di
	std	

l0800_04ED:
	repne	
	scasb	

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
	repne	
	scasb	

l0800_0509:
	not	cx
	mov	ax,005C
	sub	di,cx

l0800_0510:
	repne	
	scasb	

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
	repne	
	scasb	

l0800_0532:
	dec	di
	mov	cx,0005

l0800_0536:
	rep	
	movsb	

l0800_0538:
	mov	word ptr [2A19],0001

l0800_053E:
	pop	di
	pop	si
	ret	

fn0800_0541()
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
0800:066A                               C7 06 1F 2A 01 00           ...*..
0800:0670 C7 06 4F 2E 00 00 E9 F0 01 C7 06 1D 2A 01 00 E9 ..O.........*...
0800:0680 E7 01 C7 06 1B 2A 01 00 E9 DE 01 C7 06 17 2A 01 .....*........*.
0800:0690 00 E9 D5 01 C7 06 15 2A 01 00 E9 CC 01 83 3E 19 .......*......>.
0800:06A0 2A 00 74 03 E9 C2 01 C4 7E FC 33 C0 B9 FF FF F2 *.t.....~.3.....
0800:06B0 AE F7 D1 49 83 F9 01 76 10 FF 76 FE FF 76 FC B8 ...I...v..v..v..
0800:06C0 03 00 50 E8 5E 06 83 C4 06 C4 5E FC 26 8A 07 50 ..P.^.....^.&..P
0800:06D0 1E B8 44 08 50 E8 51 05 83 C4 06 A3 23 2A 3D 06 ..D.P.Q.....#*=.
0800:06E0 00 7C 10 FF 76 FE FF 76 FC B8 03 00 50 E8 34 06 .|..v..v....P.4.
0800:06F0 83 C4 06 8B 1E 23 2A D1 E3 D1 E3 FF B7 47 05 FF .....#*......G..
0800:0700 B7 45 05 1E B8 4B 08 50 1E B8 71 42 50 E8 92 B7 .E...K.P..qBP...
0800:0710 83 C4 0C E9 53 01 1E B8 4F 2E 50 1E B8 4F 08 50 ....S...O.P..O.P
0800:0720 FF 76 FE FF 76 FC E8 EF B7 83 C4 0C 83 3E 4F 2E .v..v........>O.
0800:0730 00 75 10 FF 76 FE FF 76 FC B8 04 00 50 E8 E4 05 .u..v..v....P...
0800:0740 83 C4 06 C7 06 1F 2A 00 00 E9 1D 01 1E B8 0F 2A ......*........*
0800:0750 50 1E B8 53 08 50 FF 76 FE FF 76 FC E8 B9 B7 83 P..S.P.v..v.....
0800:0760 C4 0C E9 04 01 1E B8 21 2A 50 1E B8 56 08 50 FF .......!*P..V.P.
0800:0770 76 FE FF 76 FC E8 A0 B7 83 C4 0C 83 3E 21 2A 02 v..v........>!*.
0800:0780 7F 14 83 3E 21 2A 00 74 03 E9 DD 00 83 3E 25 2A ...>!*.t.....>%*
0800:0790 00 74 03 E9 D3 00 FF 76 FE FF 76 FC B8 05 00 50 .t.....v..v....P
0800:07A0 E8 81 05 83 C4 06 E9 C0 00 1E 07 BF E3 42 06 8E .............B..
0800:07B0 46 FE 57 8B 7E FC 33 C0 B9 FF FF F2 AE F7 D1 2B F.W.~.3........+
0800:07C0 F9 D1 E9 8B F7 5F 8C C0 07 1E 8E D8 F3 A5 13 C9 ....._..........
0800:07D0 F3 A4 1F 1E B8 E3 42 50 E8 91 04 83 C4 04 1E 07 ......BP........
0800:07E0 BF E3 42 33 C0 B9 FF FF F2 AE F7 D1 49 49 8B D9 ..B3........II..
0800:07F0 80 BF E3 42 5C 74 72 BF E3 42 BE 59 08 B9 FF FF ...B\tr..B.Y....
0800:0800 F2 AE 4F B9 02 00 F3 A4 EB 5F 1E 07 BF 7E 42 06 ..O......_...~B.
0800:0810 8E 46 FE 57 8B 7E FC 33 C0 B9 FF FF F2 AE F7 D1 .F.W.~.3........
0800:0820 2B F9 D1 E9 8B F7 5F 8C C0 07 1E 8E D8 F3 A5 13 +....._.........
0800:0830 C9 F3 A4 1F 1E B8 7E 42 50 E8 30 04 83 C4 04 1E ......~BP.0.....
0800:0840 07 BF 7E 42 33 C0 B9 FF FF F2 AE F7 D1 49 49 8B ..~B3........II.
0800:0850 D9 80 BF 7E 42 5C 74 11 BF 7E 42 BE 59 08 B9 FF ...~B\t..~B.Y...
0800:0860 FF F2 AE 4F B9 02 00 F3 A4                      ...O.....      

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
	repne	
	scasb	

l0800_08BD:
	dec	di
	mov	cx,0002

l0800_08C1:
	rep	
	movsb	

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
	repne	
	scasb	

l0800_08E8:
	dec	di
	mov	cx,0002

l0800_08EC:
	rep	
	movsb	

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
	repne	
	scasb	

l0800_0927:
	dec	di
	mov	cx,0002

l0800_092B:
	rep	
	movsb	

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
	repne	
	scasb	

l0800_0943:
	dec	di
	mov	cx,0002

l0800_0947:
	rep	
	movsb	

l0800_0949:
	push	ds
	pop	es
	mov	di,4271
	mov	si,0861
	mov	cx,FFFF
	xor	ax,ax

l0800_0956:
	repne	
	scasb	

l0800_0958:
	dec	di
	mov	cx,0005

l0800_095C:
	rep	
	movsb	

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
0800:098D                                        9D 06 16              ...
0800:0990 07 65 07 0A 08 A9 07 4C 07 6A 06 82 06 79 06 8B .e.....L.j...y..
0800:09A0 06 94 06                                        ...            

fn0800_09A3()
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
	repne	
	scasb	

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
	repne	
	scasb	

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
	repne	
	scasb	

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
	repne	
	scasb	

l0800_0A60:
	not	cx
	sub	di,cx

l0800_0A64:
	rep	
	cmpsb	

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
	repne	
	scasb	

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
0800:0ABC                                     55 8B EC 83             U...
0800:0AC0 EC 0E 56 8B 76 04 FF 76 08 FF 76 06 16 8D 46 F2 ..V.v..v..v...F.
0800:0AD0 50 E8 CF 2A 83 C4 08 FF 36 05 2A FF 36 03 2A FF P..*....6.*.6.*.
0800:0AE0 36 09 2A FF 36 07 2A E8 8F 00 83 C4 08 8B C8 BB 6.*.6.*.........
0800:0AF0 64 00 33 D2 F7 F3 52 8B C1 33 D2 F7 F3 50 FF 36 d.3...R..3...P.6
0800:0B00 05 2A FF 36 03 2A FF 36 09 2A FF 36 07 2A 16 8D .*.6.*.6.*.6.*..
0800:0B10 46 F2 50 B8 14 00 50 50 1E B8 66 08 50 E8 CF A7 F.P...PP..f.P...
0800:0B20 83 C4 18 83 3E 25 2A 02 74 04 0B F6 74 19 8B DE ....>%*.t...t...
0800:0B30 D1 E3 D1 E3 FF B7 9B 05 FF B7 99 05 1E B8 89 08 ................
0800:0B40 50 E8 AB A7 83 C4 08 1E B8 27 08 50 E8 A0 A7 83 P........'.P....
0800:0B50 C4 04 A1 09 2A 8B 16 07 2A 01 16 F3 29 11 06 F5 ....*...*...)...
0800:0B60 29 A1 05 2A 8B 16 03 2A 01 16 EF 29 11 06 F1 29 )..*...*...)...)
0800:0B70 FF 06 13 2A 5E 8B E5 5D C3                      ...*^..].      

fn0800_0B79()
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
0800:0C08                         55 8B EC 56 8B 76 04 B9         U..V.v..
0800:0C10 01 00 EB 01 41 BB 02 00 8B C6 33 D2 F7 F3 8B F0 ....A.....3.....
0800:0C20 0B C0 75 F0 8B C1 5E 5D C3                      ..u...^].      

fn0800_0C29()
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

fn0800_0C6C()
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
0800:0C93          55 8B EC 83 EC 66 56 57 FF 76 06 FF 76    U....fVW.v..v
0800:0CA0 04 16 8D 46 9A 50 E8 60 28 83 C4 08 BE 14 27 16 ...F.P.`(.....'.
0800:0CB0 07 8D 7E 9A 33 C0 B9 FF FF F2 AE F7 D1 2B F9 F3 ..~.3........+..
0800:0CC0 A6 74 05 1B C0 1D FF FF 0B C0 74 41 16 8D 46 9A .t........tA..F.
0800:0CD0 50 1E B8 8F 08 50 E8 16 A6 83 C4 08 16 07 8D 7E P....P.........~
0800:0CE0 9A 06 1E 07 57 BF 14 27 8B C7 5F 8C C2 07 52 50 ....W..'.._...RP
0800:0CF0 33 C0 B9 FF FF F2 AE F7 D1 2B F9 D1 E9 8B F7 5F 3........+....._
0800:0D00 8C C0 07 1E 8E D8 F3 A5 13 C9 F3 A4 1F FF 76 06 ..............v.
0800:0D10 FF 76 04 1E B8 9E 08 50 E8 D4 A5 83 C4 08 5F 5E .v.....P......_^
0800:0D20 8B E5 5D C3                                     ..].           

fn0800_0D24()
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

fn0800_0DA9()
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

fn0800_0DCE()
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
0800:0DE8                         56 8B 1E 23 2A D1 E3 D1         V..#*...
0800:0DF0 E3 FF B7 2F 05 FF B7 2D 05 8B 1E 25 2A D1 E3 D1 .../...-...%*...
0800:0E00 E3 FF B7 0B 05 FF B7 09 05 1E B8 AC 08 50 E8 DE .............P..
0800:0E10 A4 83 C4 0C 83 3E 4F 2E 00 74 0F FF 36 4F 2E 1E .....>O..t..6O..
0800:0E20 B8 BA 08 50 E8 C8 A4 83 C4 06 83 3E 1F 2A 00 74 ...P.......>.*.t
0800:0E30 0B 1E B8 CF 08 50 E8 B6 A4 83 C4 04 1E B8 DC 08 .....P..........
0800:0E40 50 E8 AB A4 83 C4 04 1E B8 7E 42 50 E8 29 28 83 P........~BP.)(.
0800:0E50 C4 04 1E B8 DF 08 50 1E B8 77 44 50 E8 5F 29 83 ......P..wDP._).
0800:0E60 C4 08 1E B8 EC 08 50 1E B8 77 44 50 E8 C5 33 83 ......P..wDP..3.
0800:0E70 C4 08 89 16 DD 29 A3 DB 29 E9 78 02 E8 E5 28 E8 .....)..).x...(.
0800:0E80 0A 2A 0B C0 75 03 E9 6B 02 1E B8 41 45 50 E8 02 .*..u..k...AEP..
0800:0E90 FE 83 C4 04 FF 36 E5 29 FF 36 E3 29 E8 F5 32 83 .....6.).6.)..2.
0800:0EA0 C4 04 89 16 09 2A A3 07 2A 89 16 05 2A A3 03 2A .....*..*...*..*
0800:0EB0 33 F6 83 3E 09 2A 00 77 16 75 07 83 3E 07 2A 12 3..>.*.w.u..>.*.
0800:0EC0 77 0D 83 3E 15 2A 00 75 06 BE 03 00 E9 8D 01 E8 w..>.*.u........
0800:0ED0 C0 2A 0B C0 74 06 BE 0E 00 E9 80 01 8B 1E 23 2A .*..t.........#*
0800:0EE0 83 FB 05 76 03 E9 1F 01 D1 E3 2E FF A7 21 11 83 ...v.........!..
0800:0EF0 3E 09 2A 00 72 29 75 07 83 3E 07 2A 12 72 20 FF >.*.r)u..>.*.r .
0800:0F00 36 E5 29 FF 36 E3 29 E8 4E 30 83 C4 04 B1 08 E8 6.).6.).N0......
0800:0F10 98 7D 83 FA 52 75 08 3D 43 4E 75 03 BE 04 00 0B .}..Ru.=CNu.....
0800:0F20 F6 74 03 E9 E1 00 FF 36 09 2A FF 36 07 2A FF 36 .t.....6.*.6.*.6
0800:0F30 09 2A FF 36 07 2A E8 B1 66 83 C4 08 A1 05 2A 8B .*.6.*..f.....*.
0800:0F40 16 03 2A 3B 06 09 2A 73 03 E9 BB 00 75 09 3B 16 ..*;..*s....u.;.
0800:0F50 07 2A 73 03 E9 B0 00 83 3E 15 2A 00 75 03 E9 A6 .*s.....>.*.u...
0800:0F60 00 FF 36 E5 29 FF 36 E3 29 E8 FB AA 83 C4 04 FF ..6.).6.).......
0800:0F70 36 E1 29 FF 36 DF 29 E8 ED AA 83 C4 04 FF 36 E1 6.).6.).......6.
0800:0F80 29 FF 36 DF 29 B8 4E 52 BA 00 43 50 52 E8 0C 31 ).6.).NR..CPR..1
0800:0F90 83 C4 08 FF 36 E1 29 FF 36 DF 29 FF 36 09 2A FF ....6.).6.).6.*.
0800:0FA0 36 07 2A E8 F6 30 83 C4 08 FF 36 09 2A FF 36 07 6.*..0....6.*.6.
0800:0FB0 2A FF 36 E1 29 FF 36 DF 29 FF 36 E5 29 FF 36 E3 *.6.).6.).6.).6.
0800:0FC0 29 E8 46 2B 83 C4 0C BE 02 00 EB 3B FF 36 E5 29 ).F+.......;.6.)
0800:0FD0 FF 36 E3 29 E8 33 2F 83 C4 04 3D 5A 4D 75 07 E8 .6.).3/...=ZMu..
0800:0FE0 82 4E 8B F0 EB 21 E8 B3 56 8B F0 EB 1A E8 CF 57 .N...!..V......W
0800:0FF0 8B F0 EB 13 E8 DD 5A 8B F0 EB 0C E8 AE 63 8B F0 ......Z......c..
0800:1000 EB 05 E8 18 64 8B F0 FF 36 E1 29 FF 36 DF 29 E8 ....d...6.).6.).
0800:1010 82 31 83 C4 04 89 16 05 2A A3 03 2A A1 05 2A 8B .1......*..*..*.
0800:1020 16 03 2A 3B 06 09 2A 72 20 75 06 3B 16 07 2A 72 ..*;..*r u.;..*r
0800:1030 18 83 3E 15 2A 00 75 11 A1 09 2A 8B 16 07 2A A3 ..>.*.u...*...*.
0800:1040 05 2A 89 16 03 2A BE 03 00 83 FE 04 75 0E A1 09 .*...*......u...
0800:1050 2A 8B 16 07 2A A3 05 2A 89 16 03 2A 80 3E 7E 42 *...*..*...*.>~B
0800:1060 00 74 54 83 FE 03 74 05 83 FE 04 75 4A A1 09 2A .tT...t....uJ..*
0800:1070 8B 16 07 2A A3 05 2A 89 16 03 2A FF 36 E5 29 FF ...*..*...*.6.).
0800:1080 36 E3 29 E8 E1 A9 83 C4 04 FF 36 E1 29 FF 36 DF 6.).......6.).6.
0800:1090 29 E8 D3 A9 83 C4 04 FF 36 09 2A FF 36 07 2A FF ).......6.*.6.*.
0800:10A0 36 E1 29 FF 36 DF 29 FF 36 E5 29 FF 36 E3 29 E8 6.).6.).6.).6.).
0800:10B0 58 2A 83 C4 0C 33 F6 FF 36 E5 29 FF 36 E3 29 E8 X*...3..6.).6.).
0800:10C0 52 95 83 C4 04 FF 36 E1 29 FF 36 DF 29 E8 44 95 R.....6.).6.).D.
0800:10D0 83 C4 04 83 FE 02 7E 0D 1E B8 DC 44 50 E8 9F 7E ......~....DP..~
0800:10E0 83 C4 04 EB 03 E8 F7 26 1E B8 41 45 50 56 E8 CB .......&..AEPV..
0800:10F0 F9 83 C4 06 1E B8 41 45 50 E8 E6 1C 83 C4 04 0B ......AEP.......
0800:1100 C0 74 03 E9 76 FD FF 36 DD 29 FF 36 DB 29 E8 03 .t..v..6.).6.)..
0800:1110 95 83 C4 04 1E B8 77 44 50 E8 63 7E 83 C4 04 5E ......wDP.c~...^
0800:1120 C3 F4 0F EF 0E 02 10 FB 0F CC 0F ED 0F 56 8B 1E .............V..
0800:1130 23 2A D1 E3 D1 E3 FF B7 2F 05 FF B7 2D 05 8B 1E #*....../...-...
0800:1140 25 2A D1 E3 D1 E3 FF B7 0B 05 FF B7 09 05 1E B8 %*..............
0800:1150 F0 08 50 E8 99 A1 83 C4 0C 83 3E 4F 2E 00 74 0F ..P.......>O..t.
0800:1160 FF 36 4F 2E 1E B8 FE 08 50 E8 83 A1 83 C4 06 1E .6O.....P.......
0800:1170 B8 13 09 50 E8 78 A1 83 C4 04 83 3E 25 2A 01 74 ...P.x.....>%*.t
0800:1180 03 E9 3E 01 1E B8 7E 42 50 E8 EC 24 83 C4 04 E9 ..>...~BP..$....
0800:1190 30 01 E8 CF 25 E8 F4 26 0B C0 75 03 E9 23 01 1E 0...%..&..u..#..
0800:11A0 B8 41 45 50 E8 EC FA 83 C4 04 FF 36 E5 29 FF 36 .AEP.......6.).6
0800:11B0 E3 29 E8 DF 2F 83 C4 04 89 16 05 2A A3 03 2A 0B .)../......*..*.
0800:11C0 D2 77 0C 72 05 3D 12 00 73 05 BE 07 00 EB 74 8B .w.r.=..s.....t.
0800:11D0 1E 23 2A 83 FB 05 77 6B D1 E3 2E FF A7 D6 12 FF .#*...wk........
0800:11E0 36 E5 29 FF 36 E3 29 E8 6E 2D 83 C4 04 B1 08 E8 6.).6.).n-......
0800:11F0 B8 7A 83 FA 52 75 0C 3D 43 4E 75 07 E8 75 41 8B .z..Ru.=CNu..uA.
0800:1200 F0 EB 40 BE 07 00 EB 3B FF 36 E5 29 FF 36 E3 29 ..@....;.6.).6.)
0800:1210 E8 F7 2C 83 C4 04 3D 5A 4D 75 07 E8 E0 34 8B F0 ..,...=ZMu...4..
0800:1220 EB 21 E8 72 39 8B F0 EB 1A E8 85 39 8B F0 EB 13 .!.r9......9....
0800:1230 E8 22 3A 8B F0 EB 0C E8 55 3F 8B F0 EB 05 E8 68 .":.....U?.....h
0800:1240 3F 8B F0 FF 36 E1 29 FF 36 DF 29 E8 46 2F 83 C4 ?...6.).6.).F/..
0800:1250 04 89 16 09 2A A3 07 2A FF 36 E5 29 FF 36 E3 29 ....*..*.6.).6.)
0800:1260 E8 31 2F 83 C4 04 89 16 05 2A A3 03 2A 0B F6 74 .1/......*..*..t
0800:1270 0E A1 05 2A 8B 16 03 2A A3 09 2A 89 16 07 2A FF ...*...*..*...*.
0800:1280 36 E5 29 FF 36 E3 29 E8 8A 93 83 C4 04 FF 36 E1 6.).6.).......6.
0800:1290 29 FF 36 DF 29 E8 7C 93 83 C4 04 83 3E 25 2A 02 ).6.).|.....>%*.
0800:12A0 74 04 0B F6 74 0D 1E B8 DC 44 50 E8 D1 7C 83 C4 t...t....DP..|..
0800:12B0 04 EB 03 E8 29 25 1E B8 41 45 50 56 E8 FD F7 83 ....)%..AEPV....
0800:12C0 C4 06 1E B8 41 45 50 E8 18 1B 83 C4 04 0B C0 74 ....AEP........t
0800:12D0 03 E9 BE FE 5E C3 30 12 DF 11 3E 12 37 12 08 12 ....^.0...>.7...
0800:12E0 29 12 55 8B EC 81 EC 84 00 56 57 1E B8 48 43 50 ).U......VW..HCP
0800:12F0 8B 1E 25 2A D1 E3 D1 E3 FF B7 0B 05 FF B7 09 05 ..%*............
0800:1300 1E B8 16 09 50 E8 E7 9F 83 C4 0C 83 3E 4F 2E 00 ....P.......>O..
0800:1310 74 0F FF 36 4F 2E 1E B8 25 09 50 E8 D1 9F 83 C4 t..6O...%.P.....
0800:1320 06 1E B8 3A 09 50 E8 C6 9F 83 C4 04 1E B8 3D 09 ...:.P........=.
0800:1330 50 1E B8 48 43 50 E8 45 97 83 C4 08 89 16 D1 29 P..HCP.E.......)
0800:1340 A3 CF 29 A1 25 2A 3D 03 00 74 08 3D 04 00 74 21 ..).%*=..t.=..t!
0800:1350 E9 25 01 A1 CF 29 0B 06 D1 29 74 03 E9 19 01 1E .%...)...)t.....
0800:1360 B8 48 43 50 B8 07 00 50 E8 B9 F9 83 C4 06 E9 07 .HCP...P........
0800:1370 01 A1 CF 29 0B 06 D1 29 74 4C FF 36 D1 29 FF 36 ...)...)tL.6.).6
0800:1380 CF 29 E8 A2 2A 83 C4 04 3D 5A 4D 75 2D B8 02 00 .)..*...=ZMu-...
0800:1390 50 33 C0 50 50 FF 36 D1 29 FF 36 CF 29 E8 13 99 P3.PP.6.).6.)...
0800:13A0 83 C4 0A FF 36 D1 29 FF 36 CF 29 E8 81 99 83 C4 ....6.).6.).....
0800:13B0 04 89 16 ED 29 A3 EB 29 EB 0C C7 06 D1 29 00 00 ....)..).....)..
0800:13C0 C7 06 CF 29 00 00 A1 CF 29 0B 06 D1 29 75 23 1E ...)....)...)u#.
0800:13D0 B8 41 09 50 1E B8 48 43 50 E8 58 2E 83 C4 08 89 .A.P..HCP.X.....
0800:13E0 16 D1 29 A3 CF 29 C7 06 ED 29 00 00 C7 06 EB 29 ..)..)...).....)
0800:13F0 00 00 C6 46 92 52 C6 46 93 4E C6 46 94 43 C6 46 ...F.R.F.N.F.C.F
0800:1400 95 41 C6 46 96 00 C6 46 97 0C C6 46 9A 00 C6 46 .A.F...F...F...F
0800:1410 9B 0C C6 46 9C 00 C6 46 9D 00 33 C0 50 B8 04 00 ...F...F..3.P...
0800:1420 50 16 8D 46 9A 50 E8 A6 18 83 C4 08 B1 08 D3 E8 P..F.P..........
0800:1430 88 46 98 33 C0 50 B8 04 00 50 16 8D 46 9A 50 E8 .F.3.P...P..F.P.
0800:1440 8D 18 83 C4 08 88 46 99 FF 36 D1 29 FF 36 CF 29 ......F..6.).6.)
0800:1450 33 C0 BA 0C 00 50 52 16 8D 46 92 50 E8 F3 2C 83 3....PR..F.P..,.
0800:1460 C4 0C FF 36 D1 29 FF 36 CF 29 FF 36 ED 29 FF 36 ...6.).6.).6.).6
0800:1470 EB 29 E8 27 2C 83 C4 08 33 C0 50 E8 78 08 83 C4 .).',...3.P.x...
0800:1480 02 33 F6 E9 A1 00 1E B8 41 45 50 16 8D 46 92 50 .3......AEP..F.P
0800:1490 E8 76 20 83 C4 08 1E B8 41 45 50 16 8D 86 7C FF .v .....AEP...|.
0800:14A0 50 E8 FF 20 83 C4 08 16 8D 46 92 50 E8 8E 13 83 P.. .....F.P....
0800:14B0 C4 04 89 56 FE 89 46 FC 0B C2 75 11 16 8D 46 92 ...V..F...u...F.
0800:14C0 50 E8 98 0A 83 C4 04 89 56 FE 89 46 FC FF 76 FE P.......V..F..v.
0800:14D0 FF 76 FC 16 8D 86 7C FF 50 E8 55 14 83 C4 08 89 .v....|.P.U.....
0800:14E0 56 FA 89 46 F8 0B C2 75 14 FF 76 FE FF 76 FC 16 V..F...u..v..v..
0800:14F0 8D 86 7C FF 50 E8 8D 0B 83 C4 08 EB 0C FF 76 FA ..|.P.........v.
0800:1500 FF 76 F8 E8 E6 0E 83 C4 04 BE 01 00 C4 1E 51 2E .v............Q.
0800:1510 26 8A 47 04 B4 00 B1 08 D3 E0 26 8A 57 05 B6 00 &.G.......&.W...
0800:1520 03 C2 3D 8C FF 73 12 1E B8 41 45 50 E8 B3 18 83 ..=..s...AEP....
0800:1530 C4 04 0B C0 74 03 E9 4D FF 0B F6 75 14 FF 36 D1 ....t..M...u..6.
0800:1540 29 FF 36 CF 29 E8 CC 90 83 C4 04 5F 5E 8B E5 5D ).6.)......_^..]
0800:1550 C3 E8 0A 09 B8 01 00 50 E8 9B 07 83 C4 02 B8 01 .......P........
0800:1560 00 50 33 C0 50 E8 57 18 83 C4 04 A1 D1 29 8B 16 .P3.P.W......)..
0800:1570 CF 29 A3 E1 29 89 16 DF 29 1E B8 45 09 50 1E B8 .)..)...)..E.P..
0800:1580 77 44 50 E8 38 22 83 C4 08 1E B8 41 09 50 1E B8 wDP.8".....A.P..
0800:1590 77 44 50 E8 9E 2C 83 C4 08 89 16 DD 29 A3 DB 29 wDP..,......)..)
0800:15A0 E9 F4 02 1E B8 3D 09 50 1E B8 41 45 50 E8 84 2C .....=.P..AEP..,
0800:15B0 83 C4 08 89 16 E5 29 A3 E3 29 1E B8 41 45 50 16 ......)..)..AEP.
0800:15C0 8D 46 92 50 E8 42 1F 83 C4 08 1E B8 41 45 50 16 .F.P.B......AEP.
0800:15D0 8D 86 7C FF 50 E8 CB 1F 83 C4 08 1E B8 41 45 50 ..|.P........AEP
0800:15E0 E8 B0 F6 83 C4 04 16 8D 46 92 50 E8 4F 12 83 C4 ........F.P.O...
0800:15F0 04 52 50 16 8D 86 7C FF 50 E8 35 13 83 C4 08 89 .RP...|.P.5.....
0800:1600 56 FA 89 46 F8 FF 36 D1 29 FF 36 CF 29 E8 84 2B V..F..6.).6.)..+
0800:1610 83 C4 04 2D 04 00 83 DA 00 89 16 E9 29 A3 E7 29 ...-........)..)
0800:1620 A1 E9 29 99 B1 08 E8 61 76 C4 7E F8 50 33 C0 B9 ..)....av.~.P3..
0800:1630 FF FF F2 AE F7 D1 33 C0 2B F9 F2 AE 74 07 BF 01 ......3.+...t...
0800:1640 00 33 C0 8E C0 4F 8C C0 8E C0 58 26 88 45 01 A1 .3...O....X&.E..
0800:1650 E9 29 99 C4 7E F8 50 33 C0 B9 FF FF F2 AE F7 D1 .)..~.P3........
0800:1660 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E C0 4F 3.+...t....3...O
0800:1670 8C C0 8E C0 58 26 88 45 02 8B 16 E9 29 A1 E7 29 ....X&.E....)..)
0800:1680 B1 08 E8 05 76 C4 7E F8 50 33 C0 B9 FF FF F2 AE ....v.~.P3......
0800:1690 F7 D1 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E ..3.+...t....3..
0800:16A0 C0 4F 8C C0 8E C0 58 26 88 45 03 C4 7E F8 33 C0 .O....X&.E..~.3.
0800:16B0 B9 FF FF F2 AE F7 D1 33 C0 2B F9 F2 AE 74 07 BF .......3.+...t..
0800:16C0 01 00 33 C0 8E C0 4F 8C C0 8A 16 E7 29 8E C0 26 ..3...O.....)..&
0800:16D0 88 55 04 BF 01 00 33 C0 C7 06 05 2A 00 00 A3 03 .U....3....*....
0800:16E0 2A 8B F0 FF 36 E5 29 FF 36 E3 29 E8 A6 2A 83 C4 *...6.).6.)..*..
0800:16F0 04 89 16 09 2A A3 07 2A 33 C0 50 FF 36 E9 29 FF ....*..*3.P.6.).
0800:1700 36 E7 29 FF 36 E1 29 FF 36 DF 29 E8 A5 95 83 C4 6.).6.).6.).....
0800:1710 0A 83 3E 09 2A 00 73 03 E9 87 00 77 07 83 3E 07 ..>.*.s....w..>.
0800:1720 2A 12 76 7E 83 3E 21 2A 00 74 77 FF 36 E5 29 FF *.v~.>!*.tw.6.).
0800:1730 36 E3 29 E8 22 28 83 C4 04 B1 08 E8 6C 75 83 FA 6.)."(......lu..
0800:1740 52 75 05 3D 43 4E 74 5A FF 36 09 2A FF 36 07 2A Ru.=CNtZ.6.*.6.*
0800:1750 FF 36 09 2A FF 36 07 2A E8 8F 5E 83 C4 08 A1 05 .6.*.6.*..^.....
0800:1760 2A 8B 16 03 2A 3B 06 09 2A 72 0D 75 06 3B 16 07 *...*;..*r.u.;..
0800:1770 2A 72 05 B8 01 00 EB 02 33 C0 8B F8 0B C0 74 22 *r......3.....t"
0800:1780 B8 01 00 50 A1 05 2A 8B 16 03 2A F7 D8 F7 DA 1D ...P..*...*.....
0800:1790 00 00 50 52 FF 36 E1 29 FF 36 DF 29 E8 14 95 83 ..PR.6.).6.)....
0800:17A0 C4 0A 0B FF 75 03 E9 96 00 A1 09 2A 8B 16 07 2A ....u......*...*
0800:17B0 A3 05 2A 89 16 03 2A FF 36 E5 29 FF 36 E3 29 E8 ..*...*.6.).6.).
0800:17C0 A5 A2 83 C4 04 83 3E 09 2A 00 72 26 75 07 83 3E ......>.*.r&u..>
0800:17D0 07 2A 12 72 1D FF 36 E5 29 FF 36 E3 29 E8 78 27 .*.r..6.).6.).x'
0800:17E0 83 C4 04 B1 08 E8 C2 74 83 FA 52 75 05 3D 43 4E .......t..Ru.=CN
0800:17F0 74 2C FF 36 D1 29 FF 36 CF 29 B8 4E 52 BA 00 43 t,.6.).6.).NR..C
0800:1800 50 52 E8 97 28 83 C4 08 FF 36 D1 29 FF 36 CF 29 PR..(....6.).6.)
0800:1810 FF 36 09 2A FF 36 07 2A E8 81 28 83 C4 08 FF 36 .6.*.6.*..(....6
0800:1820 09 2A FF 36 07 2A FF 36 D1 29 FF 36 CF 29 FF 36 .*.6.*.6.).6.).6
0800:1830 E5 29 FF 36 E3 29 E8 D1 22 83 C4 0C BE 02 00 FF .).6.)..".......
0800:1840 36 D1 29 FF 36 CF 29 FF 36 ED 29 FF 36 EB 29 E8 6.).6.).6.).6.).
0800:1850 4A 28 83 C4 08 FF 36 E5 29 FF 36 E3 29 E8 B4 8D J(....6.).6.)...
0800:1860 83 C4 04 1E B8 41 45 50 56 E8 50 F2 83 C4 06 C4 .....AEPV.P.....
0800:1870 1E 51 2E 26 8A 47 04 B4 00 B1 08 D3 E0 26 8A 57 .Q.&.G.......&.W
0800:1880 05 B6 00 03 C2 3D 8C FF 72 0D 1E B8 52 09 50 E8 .....=..r...R.P.
0800:1890 5D 9A 83 C4 04 EB 12 1E B8 41 45 50 E8 43 15 83 ]........AEP.C..
0800:18A0 C4 04 0B C0 74 03 E9 FA FC FF 36 DD 29 FF 36 DB ....t.....6.).6.
0800:18B0 29 E8 60 8D 83 C4 04 1E B8 77 44 50 E8 C0 76 83 ).`......wDP..v.
0800:18C0 C4 04 E8 99 05 FF 36 D1 29 FF 36 CF 29 E8 44 8D ......6.).6.).D.
0800:18D0 83 C4 04 5F 5E 8B E5 5D C3 55 8B EC 83 EC 6E 57 ..._^..].U....nW
0800:18E0 1E B8 48 43 50 8B 1E 25 2A D1 E3 D1 E3 FF B7 0B ..HCP..%*.......
0800:18F0 05 FF B7 09 05 1E B8 6C 09 50 E8 F2 99 83 C4 0C .......l.P......
0800:1900 1E B8 7D 09 50 1E B8 48 43 50 E8 71 91 83 C4 08 ..}.P..HCP.q....
0800:1910 89 16 D1 29 A3 CF 29 0B C2 75 0F 1E B8 48 43 50 ...)..)..u...HCP
0800:1920 B8 07 00 50 E8 FD F3 83 C4 06 B8 01 00 50 E8 C5 ...P.........P..
0800:1930 03 83 C4 02 33 C0 50 50 E8 84 14 83 C4 04 EB 7C ....3.PP.......|
0800:1940 16 8D 46 92 50 E8 4B F3 83 C4 04 FF 76 FA FF 76 ..F.P.K.....v..v
0800:1950 F8 E8 98 0A 83 C4 04 FF 76 FA FF 76 F8 E8 9E 09 ........v..v....
0800:1960 83 C4 04 C4 7E FC 83 C7 02 33 C0 B9 FF FF F2 AE ....~....3......
0800:1970 F7 D1 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E ..3.+...t....3..
0800:1980 C0 4F 8C C0 8E C0 26 80 7D 01 00 75 20 A1 53 2E .O....&.}..u .S.
0800:1990 8B 16 51 2E 83 C2 08 3B 46 FE 75 05 3B 56 FC 74 ..Q....;F.u.;V.t
0800:19A0 0C FF 76 FE FF 76 FC E8 57 08 83 C4 04 16 8D 46 ..v..v..W......F
0800:19B0 92 50 B8 0D 00 50 E8 03 F1 83 C4 06 16 8D 46 92 .P...P........F.
0800:19C0 50 16 8D 46 F8 50 16 8D 46 FC 50 E8 F7 0F 83 C4 P..F.P..F.P.....
0800:19D0 0C 0B C0 74 03 E9 68 FF E8 83 04 FF 36 D1 29 FF ...t..h.....6.).
0800:19E0 36 CF 29 E8 2E 8C 83 C4 04 5F 8B E5 5D C3 55 8B 6.)......_..].U.
0800:19F0 EC 83 EC 72 56 57 1E B8 48 43 50 8B 1E 25 2A D1 ...rVW..HCP..%*.
0800:1A00 E3 D1 E3 FF B7 0B 05 FF B7 09 05 1E B8 86 09 50 ...............P
0800:1A10 E8 DC 98 83 C4 0C 1E B8 97 09 50 1E B8 48 43 50 ..........P..HCP
0800:1A20 E8 5B 90 83 C4 08 89 16 D1 29 A3 CF 29 0B C2 75 .[.......)..)..u
0800:1A30 0F 1E B8 48 43 50 B8 07 00 50 E8 E7 F2 83 C4 06 ...HCP...P......
0800:1A40 A1 27 2A 3B 06 9A 26 75 34 A1 84 09 8B 16 82 09 .'*;..&u4.......
0800:1A50 89 46 F6 89 56 F4 8B 46 F4 BA 04 00 C4 1E 9C 26 .F..V..F.......&
0800:1A60 8D 46 F4 26 8C 57 06 26 89 47 04 C7 06 27 2A 01 .F.&.W.&.G...'*.
0800:1A70 00 C7 06 9A 26 02 00 C7 06 1B 2A 01 00 33 C0 50 ....&.....*..3.P
0800:1A80 B8 01 00 50 E8 38 13 83 C4 04 B8 01 00 50 E8 65 ...P.8.......P.e
0800:1A90 02 83 C4 02 83 3E 25 2A 07 74 03 E9 16 02 1E B8 .....>%*.t......
0800:1AA0 9B 09 50 1E B8 DC 44 50 E8 13 1D 83 C4 08 E9 03 ..P...DP........
0800:1AB0 02 16 8D 46 8E 50 E8 DA F1 83 C4 04 FF 76 FA FF ...F.P.......v..
0800:1AC0 76 F8 E8 39 0A 83 C4 04 33 C0 50 C4 7E F8 B9 FF v..9....3.P.~...
0800:1AD0 FF F2 AE F7 D1 33 C0 2B F9 F2 AE 74 07 BF 01 00 .....3.+...t....
0800:1AE0 33 C0 8E C0 4F 8C C0 8E C0 26 8A 45 01 98 B1 08 3...O....&.E....
0800:1AF0 D3 E0 C4 7E F8 50 33 C0 B9 FF FF F2 AE F7 D1 33 ...~.P3........3
0800:1B00 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E C0 4F 8C .+...t....3...O.
0800:1B10 C0 8E C0 26 8A 45 02 B4 00 5A 03 D0 33 C0 C4 7E ...&.E...Z..3..~
0800:1B20 F8 52 50 B9 FF FF F2 AE F7 D1 33 C0 2B F9 F2 AE .RP.......3.+...
0800:1B30 74 07 BF 01 00 33 C0 8E C0 4F 8C C0 8E C0 26 8A t....3...O....&.
0800:1B40 45 03 98 B1 08 D3 E0 C4 7E F8 50 33 C0 B9 FF FF E.......~.P3....
0800:1B50 F2 AE F7 D1 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 ....3.+...t....3
0800:1B60 C0 8E C0 4F 8C C0 8E C0 26 8A 45 04 B4 00 5A 03 ...O....&.E...Z.
0800:1B70 D0 58 03 C2 5A 83 D2 00 52 50 FF 36 D1 29 FF 36 .X..Z...RP.6.).6
0800:1B80 CF 29 E8 2E 91 83 C4 0A 83 3E 25 2A 07 75 03 E9 .).......>%*.u..
0800:1B90 CF 00 1E 07 BF DC 44 06 57 BF 7E 42 33 C0 B9 FF ......D.W.~B3...
0800:1BA0 FF F2 AE F7 D1 2B F9 D1 E9 8B F7 5F 8C C0 07 1E .....+....._....
0800:1BB0 8E D8 F3 A5 13 C9 F3 A4 1F 83 3E 25 2A 08 75 69 ..........>%*.ui
0800:1BC0 80 3E 7E 42 00 74 35 80 7E 8E 5C 75 2F 16 8D 46 .>~B.t5.~.\u/..F
0800:1BD0 8F 50 1E 07 BF DC 44 33 C0 B9 FF FF F2 AE F7 D1 .P....D3........
0800:1BE0 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E C0 4F 3.+...t....3...O
0800:1BF0 8C C0 50 57 E8 12 19 83 C4 08 EB 2D 16 8D 46 8E ..PW.......-..F.
0800:1C00 50 1E 07 BF DC 44 33 C0 B9 FF FF F2 AE F7 D1 33 P....D3........3
0800:1C10 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E C0 4F 8C .+...t....3...O.
0800:1C20 C0 50 57 E8 E3 18 83 C4 08 1E B8 DC 44 50 E8 47 .PW.........DP.G
0800:1C30 1A 83 C4 04 16 8D 46 8E 50 1E 07 BF DC 44 33 C0 ......F.P....D3.
0800:1C40 B9 FF FF F2 AE F7 D1 33 C0 2B F9 F2 AE 74 07 BF .......3.+...t..
0800:1C50 01 00 33 C0 8E C0 4F 8C C0 50 57 E8 45 19 83 C4 ..3...O..PW.E...
0800:1C60 08 1E B8 A8 09 50 1E B8 DC 44 50 E8 C6 25 83 C4 .....P...DP..%..
0800:1C70 08 89 16 E1 29 A3 DF 29 A1 D1 29 8B 16 CF 29 A3 ....)..)..)...).
0800:1C80 E5 29 89 16 E3 29 E8 EB 36 8B F0 FF 36 E1 29 FF .)...)..6...6.).
0800:1C90 36 DF 29 E8 7E 89 83 C4 04 0B F6 74 0B 1E B8 DC 6.).~......t....
0800:1CA0 44 50 E8 DA 72 83 C4 04 16 8D 46 8E 50 56 E8 0B DP..r.....F.PV..
0800:1CB0 EE 83 C4 06 16 8D 46 8E 50 16 8D 46 F8 50 16 8D ......F.P..F.P..
0800:1CC0 46 FC 50 E8 FF 0C 83 C4 0C 0B C0 74 03 E9 E1 FD F.P........t....
0800:1CD0 FF 36 D1 29 FF 36 CF 29 E8 39 89 83 C4 04 83 3E .6.).6.).9.....>
0800:1CE0 25 2A 07 75 0B 1E B8 DC 44 50 E8 92 72 83 C4 04 %*.u....DP..r...
0800:1CF0 5F 5E 8B E5 5D C3 55 8B EC 83 EC 02 56 33 C0 33 _^..].U.....V3.3
0800:1D00 D2 A3 A8 45 89 16 A6 45 A3 ED 29 89 16 EB 29 B8 ...E...E..)...).
0800:1D10 02 00 50 B8 FF FF BA FC FF 50 52 FF 36 D1 29 FF ..P......PR.6.).
0800:1D20 36 CF 29 E8 8D 8F 83 C4 0A FF 36 D1 29 FF 36 CF 6.).......6.).6.
0800:1D30 29 E8 29 21 83 C4 04 89 16 ED 29 A3 EB 29 FF 36 ).)!......)..).6
0800:1D40 D1 29 FF 36 CF 29 E8 4B 24 83 C4 04 3B 16 ED 29 .).6.).K$...;..)
0800:1D50 77 17 72 06 3B 06 EB 29 73 0F 1E B8 48 43 50 B8 w.r.;..)s...HCP.
0800:1D60 0B 00 50 E8 BE EF 83 C4 06 33 C0 50 FF 36 ED 29 ..P......3.P.6.)
0800:1D70 FF 36 EB 29 FF 36 D1 29 FF 36 CF 29 E8 34 8F 83 .6.).6.).6.).4..
0800:1D80 C4 0A FF 36 D1 29 FF 36 CF 29 E8 D0 20 83 C4 04 ...6.).6.).. ...
0800:1D90 81 FA 4E 52 75 05 3D 41 43 74 0F 1E B8 48 43 50 ..NRu.=ACt...HCP
0800:1DA0 B8 0B 00 50 E8 7D EF 83 C4 06 FF 36 D1 29 FF 36 ...P.}.....6.).6
0800:1DB0 CF 29 E8 72 20 83 C4 04 8B F0 FF 36 D1 29 FF 36 .).r ......6.).6
0800:1DC0 CF 29 E8 62 20 83 C4 04 89 46 FE 83 7E 04 00 74 .).b ....F..~..t
0800:1DD0 13 33 C0 50 56 E8 39 25 83 C4 04 89 16 53 2E A3 .3.PV.9%.....S..
0800:1DE0 51 2E EB 14 33 C0 BA F0 FF 50 52 E8 23 25 83 C4 Q...3....PR.#%..
0800:1DF0 04 89 16 53 2E A3 51 2E 33 C0 50 FF 36 ED 29 FF ...S..Q.3.P.6.).
0800:1E00 36 EB 29 FF 36 D1 29 FF 36 CF 29 E8 A5 8E 83 C4 6.).6.).6.).....
0800:1E10 0A FF 36 D1 29 FF 36 CF 29 33 C0 50 56 FF 36 53 ..6.).6.)3.PV.6S
0800:1E20 2E FF 36 51 2E E8 E8 22 83 C4 0C 33 C0 50 8B C6 ..6Q..."...3.P..
0800:1E30 2D 08 00 50 A1 51 2E 05 08 00 FF 36 53 2E 50 E8 -..P.Q.....6S.P.
0800:1E40 8D 0E 83 C4 08 3B 46 FE 74 0F 1E B8 48 43 50 B8 .....;F.t...HCP.
0800:1E50 0D 00 50 E8 CE EE 83 C4 06 5E 8B E5 5D C3 56 C4 ..P......^..].V.
0800:1E60 1E 51 2E 26 8A 47 04 B4 00 B1 08 D3 E0 26 8A 57 .Q.&.G.......&.W
0800:1E70 05 B6 00 03 C2 8B F0 33 C0 50 8B C6 2D 08 00 50 .......3.P..-..P
0800:1E80 A1 51 2E 05 08 00 FF 36 53 2E 50 E8 41 0E 83 C4 .Q.....6S.P.A...
0800:1E90 08 8B D0 B1 08 D3 E8 C4 1E 51 2E 26 88 47 06 26 .........Q.&.G.&
0800:1EA0 88 57 07 33 C0 50 A1 ED 29 8B 16 EB 29 83 C2 04 .W.3.P..)...)...
0800:1EB0 15 00 00 50 52 FF 36 D1 29 FF 36 CF 29 E8 F3 8D ...PR.6.).6.)...
0800:1EC0 83 C4 0A FF 36 D1 29 FF 36 CF 29 E8 59 1F 83 C4 ....6.).6.).Y...
0800:1ED0 04 8B D0 3B F2 76 20 8B C6 2B C2 33 D2 52 50 FF ...;.v ..+.3.RP.
0800:1EE0 36 ED 29 FF 36 EB 29 FF 36 D1 29 FF 36 CF 29 E8 6.).6.).6.).6.).
0800:1EF0 D1 1C 83 C4 0C EB 22 3B F2 73 1E 8B C2 2B C6 33 ......";.s...+.3
0800:1F00 D2 52 50 FF 36 ED 29 FF 36 EB 29 FF 36 D1 29 FF .RP.6.).6.).6.).
0800:1F10 36 CF 29 E8 83 1D 83 C4 0C 33 C0 50 FF 36 ED 29 6.)......3.P.6.)
0800:1F20 FF 36 EB 29 FF 36 D1 29 FF 36 CF 29 E8 84 8D 83 .6.).6.).6.)....
0800:1F30 C4 0A FF 36 D1 29 FF 36 CF 29 33 C0 50 56 FF 36 ...6.).6.)3.PV.6
0800:1F40 53 2E FF 36 51 2E E8 09 22 83 C4 0C FF 36 53 2E S..6Q..."....6S.
0800:1F50 FF 36 51 2E E8 EF 23 83 C4 04 5E C3 55 8B EC 83 .6Q...#...^.U...
0800:1F60 EC 08 56 57 C4 7E 04 33 C0 B9 FF FF F2 AE F7 D1 ..VW.~.3........
0800:1F70 B8 3A 00 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E C0 .:.+...t....3...
0800:1F80 4F 8C C0 0B F8 74 26 C4 7E 04 33 C0 B9 FF FF F2 O....t&.~.3.....
0800:1F90 AE F7 D1 B8 3A 00 2B F9 F2 AE 74 07 BF 01 00 33 ....:.+...t....3
0800:1FA0 C0 8E C0 4F 8C C0 47 89 46 06 89 7E 04 C4 1E 51 ...O..G.F..~...Q
0800:1FB0 2E 26 8A 47 04 B4 00 B1 08 D3 E0 26 8A 57 05 B6 .&.G.......&.W..
0800:1FC0 00 03 C2 89 46 FA A1 53 2E 8B 16 51 2E 03 56 FA ....F..S...Q..V.
0800:1FD0 89 46 FE 89 56 FC C4 7E 04 33 C0 B9 FF FF F2 AE .F..V..~.3......
0800:1FE0 F7 D1 49 83 C1 04 89 4E F8 8B 46 FC 33 D2 2B 06 ..I....N..F.3.+.
0800:1FF0 51 2E 83 DA 00 03 46 F8 83 D2 00 B1 08 E8 8A 6C Q.....F........l
0800:2000 C4 5E FC 26 88 07 8B 46 FC 33 D2 2B 06 51 2E 83 .^.&...F.3.+.Q..
0800:2010 DA 00 02 46 F8 26 88 47 01 8B 7E FC 83 C7 02 06 ...F.&.G..~.....
0800:2020 8E 46 06 57 8B 7E 04 33 C0 B9 FF FF F2 AE F7 D1 .F.W.~.3........
0800:2030 2B F9 D1 E9 8B F7 5F 8C C0 07 1E 8E D8 F3 A5 13 +....._.........
0800:2040 C9 F3 A4 1F 8E 46 FE 03 5E F8 26 C6 47 FF 00 8B .....F..^.&.G...
0800:2050 46 FA 03 46 F8 B1 08 D3 E8 C4 1E 51 2E 26 88 47 F..F.......Q.&.G
0800:2060 04 8A 46 FA 02 46 F8 26 88 47 05 33 C0 50 50 50 ..F..F.&.G.3.PPP
0800:2070 FF 76 F8 E8 12 06 83 C4 08 8B 56 FE 8B 46 FC 5F .v........V..F._
0800:2080 5E 8B E5 5D C3 55 8B EC 83 EC 0C 56 57 C4 1E 51 ^..].U.....VW..Q
0800:2090 2E 26 8A 47 04 B4 00 B1 08 D3 E0 26 8A 57 05 B6 .&.G.......&.W..
0800:20A0 00 03 C2 89 46 F6 A1 53 2E 8B 16 51 2E 03 56 F6 ....F..S...Q..V.
0800:20B0 89 46 FA 89 56 F8 C4 7E 04 33 C0 B9 FF FF F2 AE .F..V..~.3......
0800:20C0 F7 D1 49 83 C1 05 89 4E F4 C4 5E 08 26 8A 07 98 ..I....N..^.&...
0800:20D0 B1 08 D3 E0 26 8A 57 01 B6 00 03 C2 8B 16 53 2E ....&.W.......S.
0800:20E0 8B 1E 51 2E 03 D8 4B 89 56 FE 89 5E FC 8B 46 F8 ..Q...K.V..^..F.
0800:20F0 33 D2 2B 46 FC 83 DA 00 50 FF 36 53 2E 53 8B 46 3.+F....P.6S.S.F
0800:2100 FC 03 46 F4 FF 36 53 2E 50 E8 E7 8F 83 C4 0A C4 ..F..6S.P.......
0800:2110 7E FC 06 8E 46 06 57 8B 7E 04 33 C0 B9 FF FF F2 ~...F.W.~.3.....
0800:2120 AE F7 D1 2B F9 D1 E9 8B F7 5F 8C C0 07 1E 8E D8 ...+....._......
0800:2130 F3 A5 13 C9 F3 A4 1F C4 7E FC 33 C0 B9 FF FF F2 ........~.3.....
0800:2140 AE F7 D1 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 ...3.+...t....3.
0800:2150 8E C0 4F 8C C0 8E C0 26 C6 45 01 00 C4 7E FC 33 ..O....&.E...~.3
0800:2160 C0 B9 FF FF F2 AE F7 D1 33 C0 2B F9 F2 AE 74 07 ........3.+...t.
0800:2170 BF 01 00 33 C0 8E C0 4F 8C C0 8E C0 26 C6 45 02 ...3...O....&.E.
0800:2180 00 C4 7E FC 33 C0 B9 FF FF F2 AE F7 D1 33 C0 2B ..~.3........3.+
0800:2190 F9 F2 AE 74 07 BF 01 00 33 C0 8E C0 4F 8C C0 8E ...t....3...O...
0800:21A0 C0 26 C6 45 03 00 C4 7E FC 33 C0 B9 FF FF F2 AE .&.E...~.3......
0800:21B0 F7 D1 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E ..3.+...t....3..
0800:21C0 C0 4F 8C C0 8E C0 26 C6 45 04 00 8B 46 F6 03 46 .O....&.E...F..F
0800:21D0 F4 B1 08 D3 E8 C4 1E 51 2E 26 88 47 04 8A 46 F6 .......Q.&.G..F.
0800:21E0 02 46 F4 26 88 47 05 33 C0 50 50 50 FF 76 F4 E8 .F.&.G.3.PPP.v..
0800:21F0 96 04 83 C4 08 8B 56 FE 8B 46 FC 5F 5E 8B E5 5D ......V..F._^..]
0800:2200 C3 55 8B EC 83 EC 0A 57 C4 1E 51 2E 26 8A 47 04 .U.....W..Q.&.G.
0800:2210 B4 00 B1 08 D3 E0 26 8A 57 05 B6 00 03 C2 89 46 ......&.W......F
0800:2220 FA A1 53 2E 8B 16 51 2E 03 56 FA 89 46 FE 89 56 ..S...Q..V..F..V
0800:2230 FC C4 5E 04 26 8A 07 98 D3 E0 26 8A 57 01 B6 00 ..^.&.....&.W...
0800:2240 03 C2 8B 16 51 2E 03 D0 33 C0 2B 56 04 1D 00 00 ....Q...3.+V....
0800:2250 89 56 F8 8B 46 04 03 46 F8 89 46 F6 8B 56 FC 33 .V..F..F..F..V.3
0800:2260 DB 2B D0 83 DB 00 52 FF 76 06 50 FF 76 06 FF 76 .+....R.v.P.v..v
0800:2270 04 E8 7F 8E 83 C4 0A 8B 46 FA 2B 46 F8 B1 08 D3 ........F.+F....
0800:2280 E8 C4 1E 51 2E 26 88 47 04 8A 46 FA 2A 46 F8 26 ...Q.&.G..F.*F.&
0800:2290 88 47 05 33 C0 50 50 8B 46 F8 33 D2 F7 DA F7 D8 .G.3.PP.F.3.....
0800:22A0 83 DA 00 52 50 E8 E0 03 83 C4 08 A1 B0 45 8B 16 ...RP........E..
0800:22B0 AE 45 3B 46 06 75 33 3B 56 04 75 2E C4 7E 04 83 .E;F.u3;V.u..~..
0800:22C0 C7 02 33 C0 B9 FF FF F2 AE F7 D1 33 C0 2B F9 F2 ..3........3.+..
0800:22D0 AE 74 07 BF 01 00 33 C0 8E C0 4F 8C C0 47 A3 AC .t....3...O..G..
0800:22E0 45 89 3E AA 45 5F 8B E5 5D C3 A1 AE 45 3B 46 04 E.>.E_..]...E;F.
0800:22F0 76 07 8B 46 F8 29 06 AA 45 5F 8B E5 5D C3 55 8B v..F.)..E_..].U.
0800:2300 EC 83 EC 0A 57 C4 1E 51 2E 26 8A 47 04 B4 00 B1 ....W..Q.&.G....
0800:2310 08 D3 E0 26 8A 57 05 B6 00 03 C2 89 46 FA A1 53 ...&.W......F..S
0800:2320 2E 8B 16 51 2E 03 56 FA 89 46 FE 89 56 FC C4 7E ...Q..V..F..V..~
0800:2330 04 33 C0 B9 FF FF F2 AE F7 D1 33 C0 2B F9 F2 AE .3........3.+...
0800:2340 74 07 BF 01 00 33 C0 8E C0 4F 8C C0 83 C7 05 33 t....3...O.....3
0800:2350 C0 2B 7E 04 1D 00 00 89 7E F8 8B 46 04 03 46 F8 .+~.....~..F..F.
0800:2360 89 46 F6 8B 56 FC 33 DB 2B D0 83 DB 00 52 FF 76 .F..V.3.+....R.v
0800:2370 06 50 FF 76 06 FF 76 04 E8 78 8D 83 C4 0A 8B 46 .P.v..v..x.....F
0800:2380 FA 2B 46 F8 B1 08 D3 E8 C4 1E 51 2E 26 88 47 04 .+F.......Q.&.G.
0800:2390 8A 46 FA 2A 46 F8 26 88 47 05 33 C0 50 50 8B 46 .F.*F.&.G.3.PP.F
0800:23A0 F8 33 D2 F7 DA F7 D8 83 DA 00 52 50 E8 D9 02 83 .3........RP....
0800:23B0 C4 08 8B 46 06 8B 56 04 03 56 F8 A3 AC 45 89 16 ...F..V..V...E..
0800:23C0 AA 45 0B D0 74 12 8B 46 06 8B 56 04 A3 AC 45 89 .E..t..F..V...E.
0800:23D0 16 AA 45 5F 8B E5 5D C3 A1 AA 45 3B 46 04 76 07 ..E_..]...E;F.v.
0800:23E0 8B 46 F8 29 06 AA 45 5F 8B E5 5D C3 55 8B EC 83 .F.)..E_..].U...
0800:23F0 EC 08 57 FF 76 06 FF 76 04 E8 02 01 83 C4 04 89 ..W.v..v........
0800:2400 56 FA 89 46 F8 C4 7E 04 33 C0 B9 FF FF F2 AE F7 V..F..~.3.......
0800:2410 D1 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E C0 .3.+...t....3...
0800:2420 4F 8C C0 8E C0 26 8A 45 01 98 B1 08 D3 E0 C4 7E O....&.E.......~
0800:2430 04 50 33 C0 B9 FF FF F2 AE F7 D1 33 C0 2B F9 F2 .P3........3.+..
0800:2440 AE 74 07 BF 01 00 33 C0 8E C0 4F 8C C0 8E C0 26 .t....3...O....&
0800:2450 8A 45 02 B4 00 5A 03 D0 33 C0 C4 7E 04 52 50 B9 .E...Z..3..~.RP.
0800:2460 FF FF F2 AE F7 D1 33 C0 2B F9 F2 AE 74 07 BF 01 ......3.+...t...
0800:2470 00 33 C0 8E C0 4F 8C C0 8E C0 26 8A 45 03 98 B1 .3...O....&.E...
0800:2480 08 D3 E0 C4 7E 04 50 33 C0 B9 FF FF F2 AE F7 D1 ....~.P3........
0800:2490 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E C0 4F 3.+...t....3...O
0800:24A0 8C C0 8E C0 26 8A 45 04 B4 00 5A 03 D0 58 03 C2 ....&.E...Z..X..
0800:24B0 5A 83 D2 00 89 56 FE 89 46 FC FF 76 FA FF 76 F8 Z....V..F..v..v.
0800:24C0 8B 46 FE 8B 56 FC 2B 16 A6 45 1B 06 A8 45 50 52 .F..V.+..E...EPR
0800:24D0 FF 36 D1 29 FF 36 CF 29 E8 BE 17 83 C4 0C FF 76 .6.).6.).......v
0800:24E0 FE FF 76 FC 8B 46 FA 8B 56 F8 F7 D8 F7 DA 1D 00 ..v..F..V.......
0800:24F0 00 50 52 E8 92 01 83 C4 08 5F 8B E5 5D C3 55 8B .PR......_..].U.
0800:2500 EC 83 EC 0C 57 C4 7E 04 33 C0 B9 FF FF F2 AE F7 ....W.~.3.......
0800:2510 D1 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E C0 .3.+...t....3...
0800:2520 4F 8C C0 8E C0 26 8A 45 01 98 B1 08 D3 E0 C4 7E O....&.E.......~
0800:2530 04 50 33 C0 B9 FF FF F2 AE F7 D1 33 C0 2B F9 F2 .P3........3.+..
0800:2540 AE 74 07 BF 01 00 33 C0 8E C0 4F 8C C0 8E C0 26 .t....3...O....&
0800:2550 8A 45 02 B4 00 5A 03 D0 33 C0 C4 7E 04 52 50 B9 .E...Z..3..~.RP.
0800:2560 FF FF F2 AE F7 D1 33 C0 2B F9 F2 AE 74 07 BF 01 ......3.+...t...
0800:2570 00 33 C0 8E C0 4F 8C C0 8E C0 26 8A 45 03 98 B1 .3...O....&.E...
0800:2580 08 D3 E0 C4 7E 04 50 33 C0 B9 FF FF F2 AE F7 D1 ....~.P3........
0800:2590 33 C0 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E C0 4F 3.+...t....3...O
0800:25A0 8C C0 8E C0 26 8A 45 04 B4 00 5A 03 D0 58 03 C2 ....&.E...Z..X..
0800:25B0 5A 83 D2 00 2B 06 A6 45 1B 16 A8 45 89 56 FE 89 Z...+..E...E.V..
0800:25C0 46 FC 33 C0 50 FF 76 FE FF 76 FC FF 36 D1 29 FF F.3.P.v..v..6.).
0800:25D0 36 CF 29 E8 DD 86 83 C4 0A FF 36 D1 29 FF 36 CF 6.).......6.).6.
0800:25E0 29 E8 79 18 83 C4 04 89 56 FA 89 46 F8 B1 08 E8 ).y.....V..F....
0800:25F0 98 66 83 FA 52 75 05 3D 43 4E 74 0F 1E B8 48 43 .f..Ru.=CNt...HC
0800:2600 50 B8 0C 00 50 E8 1C E7 83 C4 06 8B 46 F8 25 FF P...P.......F.%.
0800:2610 00 0D 00 00 74 3F FF 36 D1 29 FF 36 CF 29 E8 3C ....t?.6.).6.).<
0800:2620 18 83 C4 04 89 16 09 2A A3 07 2A FF 36 D1 29 FF .......*..*.6.).
0800:2630 36 CF 29 E8 27 18 83 C4 04 89 16 05 2A A3 03 2A 6.).'.......*..*
0800:2640 A1 05 2A 8B 16 03 2A 83 C2 12 15 00 00 89 46 F6 ..*...*.......F.
0800:2650 89 56 F4 EB 28 FF 36 D1 29 FF 36 CF 29 E8 FD 17 .V..(.6.).6.)...
0800:2660 83 C4 04 89 16 05 2A A3 03 2A 89 16 09 2A A3 07 ......*..*...*..
0800:2670 2A 05 08 00 83 D2 00 89 56 F6 89 46 F4 8B 56 F6 *.......V..F..V.
0800:2680 8B 46 F4 5F 8B E5 5D C3 55 8B EC 83 EC 10 C4 1E .F._..].U.......
0800:2690 51 2E 26 8A 47 04 B4 00 B1 08 D3 E0 26 8A 57 05 Q.&.G.......&.W.
0800:26A0 B6 00 03 C2 8B 16 53 2E 03 D8 89 56 FA 89 5E F8 ......S....V..^.
0800:26B0 A1 53 2E 8B 16 51 2E 83 C2 08 89 46 FE 89 56 FC .S...Q.....F..V.
0800:26C0 E9 F0 00 8B 46 FE 8B 56 FC 89 46 F6 89 56 F4 83 ....F..V..F..V..
0800:26D0 46 FC 02 C4 5E FC FF 46 FC 26 80 3F 00 75 F4 E9 F...^..F.&.?.u..
0800:26E0 9B 00 C4 5E FC FF 46 FC 26 80 3F 00 75 F4 C4 5E ...^..F.&.?.u..^
0800:26F0 FC 26 8A 07 98 B1 08 D3 E0 26 8A 57 01 B6 00 03 .&.......&.W....
0800:2700 C2 33 D2 50 26 8A 47 02 98 D3 E0 26 8A 5F 03 B7 .3.P&.G....&._..
0800:2710 00 03 C3 03 D0 58 15 00 00 89 46 F2 89 56 F0 8B .....X....F..V..
0800:2720 46 F2 3B 46 0A 7C 52 75 05 3B 56 08 72 4B 8B 46 F.;F.|Ru.;V.rK.F
0800:2730 F2 8B 56 F0 03 56 04 13 46 06 99 B1 08 E8 4A 65 ..V..V..F.....Je
0800:2740 C4 5E FC 26 88 07 8B 46 F2 8B 56 F0 03 56 04 13 .^.&...F..V..V..
0800:2750 46 06 99 26 88 47 01 8B 56 F2 8B 46 F0 03 46 04 F..&.G..V..F..F.
0800:2760 13 56 06 B1 08 E8 22 65 C4 5E FC 26 88 47 02 8A .V...."e.^.&.G..
0800:2770 46 F0 02 46 04 26 88 47 03 83 46 FC 04 C4 5E FC F..F.&.G..F...^.
0800:2780 26 80 3F 00 74 03 E9 59 FF FF 46 FC 8B 46 FC 33 &.?.t..Y..F..F.3
0800:2790 D2 2B 06 51 2E 83 DA 00 B1 08 E8 ED 64 C4 5E F4 .+.Q........d.^.
0800:27A0 26 88 07 8B 46 FC 33 D2 2B 06 51 2E 83 DA 00 26 &...F.3.+.Q....&
0800:27B0 88 47 01 8B 46 FE 8B 56 FC 3B 46 FA 74 03 E9 02 .G..F..V.;F.t...
0800:27C0 FF 3B 56 F8 74 03 E9 FA FE 8B 46 08 0B 46 0A 75 .;V.t.....F..F.u
0800:27D0 0E 8B 46 06 8B 56 04 01 16 A6 45 11 06 A8 45 8B ..F..V....E...E.
0800:27E0 E5 5D C3 55 8B EC 83 EC 6A FF 76 06 FF 76 04 16 .].U....j.v..v..
0800:27F0 8D 46 96 50 E8 12 0D 83 C4 08 16 8D 46 96 50 E8 .F.P........F.P.
0800:2800 3B 00 83 C4 04 89 56 FE 89 46 FC 0B C2 75 08 33 ;.....V..F...u.3
0800:2810 D2 33 C0 8B E5 5D C3 FF 76 06 FF 76 04 16 8D 46 .3...]..v..v...F
0800:2820 96 50 E8 7E 0D 83 C4 08 FF 76 FE FF 76 FC 16 8D .P.~.....v..v...
0800:2830 46 96 50 E8 FB 00 83 C4 08 8B E5 5D C3 55 8B EC F.P........].U..
0800:2840 83 EC 08 56 57 C4 7E 04 33 C0 B9 FF FF F2 AE F7 ...VW.~.3.......
0800:2850 D1 B8 3A 00 2B F9 F2 AE 74 07 BF 01 00 33 C0 8E ..:.+...t....3..
0800:2860 C0 4F 8C C0 0B F8 74 26 C4 7E 04 33 C0 B9 FF FF .O....t&.~.3....
0800:2870 F2 AE F7 D1 B8 3A 00 2B F9 F2 AE 74 07 BF 01 00 .....:.+...t....
0800:2880 33 C0 8E C0 4F 8C C0 47 89 46 06 89 7E 04 C4 1E 3...O..G.F..~...
0800:2890 51 2E 26 8A 47 04 B4 00 B1 08 D3 E0 26 8A 57 05 Q.&.G.......&.W.
0800:28A0 B6 00 03 C2 8B 16 53 2E 03 D8 89 56 FE 89 5E FC ......S....V..^.
0800:28B0 A1 53 2E 8B 16 51 2E 83 C2 08 89 46 FA 89 56 F8 .S...Q.....F..V.
0800:28C0 EB 55 8B 76 F8 83 C6 02 1E 8E 5E FA C4 7E 04 33 .U.v......^..~.3
0800:28D0 C0 B9 FF FF F2 AE F7 D1 2B F9 F3 A6 74 05 1B C0 ........+...t...
0800:28E0 1D FF FF 1F 0B C0 75 0C 8B 56 FA 8B 46 F8 5F 5E ......u..V..F._^
0800:28F0 8B E5 5D C3 C4 5E F8 26 8A 07 98 B1 08 D3 E0 26 ..]..^.&.......&
0800:2900 8A 57 01 B6 00 03 C2 8B 16 53 2E 8B 1E 51 2E 03 .W.......S...Q..
0800:2910 D8 89 56 FA 89 5E F8 8B 46 FA 8B 56 F8 3B 46 FE ..V..^..F..V.;F.
0800:2920 75 A0 3B 56 FC 75 9B 33 D2 33 C0 5F 5E 8B E5 5D u.;V.u.3.3._^..]
0800:2930 C3 55 8B EC 56 57 C4 7E 08 83 C7 02 33 C0 B9 FF .U..VW.~....3...
0800:2940 FF F2 AE F7 D1 33 C0 2B F9 F2 AE 74 07 BF 01 00 .....3.+...t....
0800:2950 33 C0 8E C0 4F 8C C0 47 89 46 0A 89 7E 08 EB 54 3...O..G.F..~..T
0800:2960 8B 76 08 1E 8E 5E 0A C4 7E 04 33 C0 B9 FF FF F2 .v...^..~.3.....
0800:2970 AE F7 D1 2B F9 F3 A6 74 05 1B C0 1D FF FF 1F 0B ...+...t........
0800:2980 C0 75 0A 8B 56 0A 8B 46 08 5F 5E 5D C3 C4 7E 08 .u..V..F._^]..~.
0800:2990 33 C0 B9 FF FF F2 AE F7 D1 33 C0 2B F9 F2 AE 74 3........3.+...t
0800:29A0 07 BF 01 00 33 C0 8E C0 4F 8C C0 83 C7 05 89 46 ....3...O......F
0800:29B0 0A 89 7E 08 C4 5E 08 26 80 3F 00 75 A3 33 D2 33 ..~..^.&.?.u.3.3
0800:29C0 C0 5F 5E 5D C3 55 8B EC 83 EC 78 56 57 A1 AE 45 ._^].U....xVW..E
0800:29D0 0B 06 B0 45 75 31                               ...Eu1         

fn006B_A326()
	mov	ax,[45AA]
	or	ax,[45AC]
	jnz	A357

l006B_A32F:
	push	ds
	mov	ax,4541
	push	ax
	call	A732
	add	sp,04
	or	ax,ax
	jnz	A346

l006B_A33E:
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	

l006B_A346:
	mov	ax,[2E53]
	mov	dx,[2E51]
	add	dx,08
	mov	[45B0],ax
	mov	[45AE],dx

l006B_A357:
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	AE59
	add	sp,08
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp-78]
	push	ax
	call	AEF3
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
	jmp	A5A3
	mov	ax,[45AA]
	or	ax,[45AC]
	jnz	A3A8
	jmp	A562
	jmp	A4A4
	push	ss
	lea	ax,[bp-78]
	push	ax
	push	word ptr [45AC]
	push	word ptr [45AA]
	call	BCA7
	add	sp,08
	or	ax,ax
	jnz	A3C5
	jmp	A47B
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
	repne	
	scasb	
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
	pop	ds
	les	di,[bp+0C]
	push	es
	mov	es,[45AC]
	push	di
	mov	di,[45AA]
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
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
	repne	
	scasb	
	dec	di
	pop	cx
	rep	
	movsb	
	mov	ds,dx
	les	di,[45AA]
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	xor	ax,ax
	sub	di,cx
	repne	
	scasb	
	jz	A465
	mov	di,0001
	xor	ax,ax
	mov	es,ax
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
	les	di,[45AA]
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	xor	ax,ax
	sub	di,cx
	repne	
	scasb	
	jz	A497
	mov	di,0001
	xor	ax,ax
	mov	es,ax
	dec	di
	mov	ax,es
	add	di,05
	mov	[45AC],ax
	mov	[45AA],di
	les	bx,[45AA]
	cmp	byte ptr es:[bx],00
	jz	A4B1
	jmp	A3AB
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
	jmp	A562
	cmp	word ptr [2A1B],00
	jz	A50B
	push	ss
	pop	es
	lea	di,[bp-6A]
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
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
	call	3936
	add	sp,0A
	or	ax,ax
	jz	A579
	cmp	word ptr [2A1B],00
	jnz	A53C
	mov	si,[45AE]
	add	si,02
	push	ds
	mov	ds,[45B0]
	push	ss
	pop	es
	lea	di,[bp-6A]
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	sub	di,cx
	rep	
	cmpsb	
	jz	A537
	sbb	ax,ax
	sbb	ax,FFFF
	pop	ds
	or	ax,ax
	jz	A579
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
	mov	ax,[45B0]
	mov	dx,[45AE]
	cmp	ax,[bp-02]
	jz	A571
	jmp	A4DA
	cmp	dx,[bp-04]
	jz	A579
	jmp	A4DA
	les	di,[45AE]
	add	di,02
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	xor	ax,ax
	sub	di,cx
	repne	
	scasb	
	jz	A598
	mov	di,0001
	xor	ax,ax
	mov	es,ax
	dec	di
	mov	ax,es
	inc	di
	mov	[45AC],ax
	mov	[45AA],di
	mov	ax,[45B0]
	mov	dx,[45AE]
	cmp	ax,[bp-02]
	jz	A5B2
	jmp	A39C
	cmp	dx,[bp-04]
	jz	A5BA
	jmp	A39C
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
	call	A315
	add	sp,0C
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	

fn0800_2C9A()
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
0800:2CCF                                              55                U
0800:2CD0 8B EC 56 8B 76 08 EB 25 C4 5E 04 8A 46 0A 26 32 ..V.v..%.^..F.&2
0800:2CE0 07 B4 00 25 FF 00 D1 E0 8B D8 8B 87 29 2A 8B 56 ...%........)*.V
0800:2CF0 0A B1 08 D3 EA 33 C2 89 46 0A FF 46 04 8B C6 4E .....3..F..F...N
0800:2D00 0B C0 75 D4 8B 46 0A 5E 5D C3 55 8B EC 83 EC 0C ..u..F.^].U.....
0800:2D10 56 33 C0 BA F0 FF 50 52 E8 F6 15 83 C4 04 89 56 V3....PR.......V
0800:2D20 FE 89 46 FC FF 76 06 FF 76 04 E8 02 80 83 C4 04 ..F..v..v.......
0800:2D30 89 56 F6 89 46 F4 33 F6 EB 55 83 7E 0A 00 72 0F .V..F.3..U.~..r.
0800:2D40 77 06 83 7E 08 F0 76 07 33 D2 B8 F0 FF EB 06 8B w..~..v.3.......
0800:2D50 56 0A 8B 46 08 89 56 FA 89 46 F8 FF 76 06 FF 76 V..F..V..F..v..v
0800:2D60 04 FF 76 FA 50 FF 76 FE FF 76 FC E8 A2 13 83 C4 ..v.P.v..v......
0800:2D70 0C 56 FF 76 F8 FF 76 FE FF 76 FC E8 51 FF 83 C4 .V.v..v..v..Q...
0800:2D80 08 8B F0 8B 46 FA 8B 56 F8 29 56 08 19 46 0A 8B ....F..V.)V..F..
0800:2D90 46 08 0B 46 0A 75 A3 33 C0 50 FF 76 F6 FF 76 F4 F..F.u.3.P.v..v.
0800:2DA0 FF 76 06 FF 76 04 E8 0A 7F 83 C4 0A FF 76 FE FF .v..v........v..
0800:2DB0 76 FC E8 91 15 83 C4 04 8B C6 5E 8B E5 5D C3    v.........^..].

fn0800_2DBF()
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

fn006B_A732()
	push	bp
	mov	bp,sp
	sub	sp,10
	push	si
	push	di
	cmp	word ptr [4654],00
	jnz	A761

l006B_A741:
	mov	ax,[4652]
	cmp	ax,[4656]
	jnz	A761

l006B_A74A:
	cmp	word ptr [464E],00
	jz	A761

l006B_A751:
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10]
	push	ax
	call	ADC9
	add	sp,06
	mov	si,ax

l006B_A761:
	cmp	word ptr [4654],00
	jnz	A76B
	jmp	A8A1
	mov	word ptr [4617],0000
	push	word ptr [461D]
	push	word ptr [461B]
	mov	ax,0065
	push	ax
	push	ds
	mov	ax,45B2
	push	ax
	call	20CD
	add	sp,0A
	or	ax,dx
	jnz	A78F
	jmp	A86B
	push	ds
	pop	es
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	mov	ax,000D
	sub	di,cx
	repne	
	scasb	
	jz	A7AD
	mov	di,0001
	xor	ax,ax
	mov	es,ax
	dec	di
	mov	ax,es
	or	di,ax
	jz	A7DB
	push	ds
	pop	es
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	mov	ax,000D
	sub	di,cx
	repne	
	scasb	
	jz	A7D2
	mov	di,0001
	xor	ax,ax
	mov	es,ax
	dec	di
	mov	ax,es
	mov	es,ax
	mov	byte ptr es:[di],00
	mov	al,[45B2]
	cbw	
	or	ax,ax
	jnz	A7F5
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A732
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	ds
	pop	es
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	dec	cx
	mov	[4619],cx
	push	ds
	mov	ax,45B2
	push	ax
	call	85BC
	add	sp,04
	cmp	word ptr [464E],00
	jnz	A84E
	les	di,[bp+04]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
	pop	ds
	inc	word ptr [4617]
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	ds
	mov	ax,45B2
	push	ax
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10]
	push	ax
	call	AD1D
	add	sp,0A
	mov	si,ax
	mov	word ptr [4654],0000
	jmp	A8A1
	push	word ptr [461D]
	push	word ptr [461B]
	call	1F64
	add	sp,04
	mov	word ptr [4654],0000
	mov	word ptr [461D],0000
	mov	word ptr [461B],0000
	inc	word ptr [4652]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A732
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	mov	ax,[4652]
	cmp	ax,[4656]
	jnz	A8AD
	jmp	A9B6
	cmp	ax,[269A]
	jnz	A8BB
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
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
	call	38EE
	add	sp,08
	push	ds
	pop	es
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	dec	cx
	mov	[4619],cx
	push	ds
	mov	ax,45B2
	push	ax
	call	85BC
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
	jnz	A957
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
	call	BB84
	add	sp,08
	mov	[461D],dx
	mov	[461B],ax
	mov	word ptr [4654],0001
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A732
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	ds
	mov	ax,45B2
	push	ax
	call	ACAC
	add	sp,04
	cmp	word ptr [464E],00
	jnz	A9A1
	les	di,[bp+04]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,45B2
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
	pop	ds
	inc	word ptr [4617]
	inc	word ptr [4652]
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	ds
	mov	ax,45B2
	push	ax
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10]
	push	ax
	call	AD1D
	add	sp,0A
	mov	si,ax
	or	si,si
	jnz	A9BD
	jmp	AA40
	cmp	word ptr [2A1B],00
	jz	A9EE
	jmp	A9EA
	push	ds
	mov	ax,45B2
	push	ax
	call	AB04
	add	sp,04
	or	ax,ax
	jz	A9EE
	push	ds
	mov	ax,45B2
	push	ax
	xor	ax,ax
	push	ax
	push	ss
	lea	ax,[bp-10]
	push	ax
	call	AD1D
	add	sp,0A
	mov	si,ax
	or	si,si
	jnz	A9C6
	or	si,si
	jz	AA40
	mov	bx,[4619]
	mov	byte ptr [bx+45B2],00
	cmp	word ptr [4617],00
	jnz	AA19
	cmp	word ptr [4650],00
	jz	AA19
	push	ds
	mov	ax,45B2
	push	ax
	push	ds
	mov	ax,0A17
	push	ax
	call	2C3F
	add	sp,08
	mov	ax,[461B]
	or	ax,[461D]
	jz	AA2A
	mov	word ptr [4654],0001
	jmp	AA2E
	inc	word ptr [4652]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A732
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	ds
	mov	ax,45B2
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AE59
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
	repne	
	scasb	
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
	repne	
	scasb	
	dec	di
	pop	cx
	rep	
	movsb	
	mov	ds,[bp-02]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	85BC
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
	repne	
	scasb	
	not	cx
	sub	di,cx
	rep	
	cmpsb	
	jz	AAB9
	sbb	ax,ax
	sbb	ax,FFFF
	pop	ds
	or	ax,ax
	jz	AAE5
	mov	si,[bp+04]
	mov	cx,[bp+06]
	push	ds
	pop	es
	mov	di,4477
	push	ds
	mov	ds,cx
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	sub	di,cx
	rep	
	cmpsb	
	jz	AAE0
	sbb	ax,ax
	sbb	ax,FFFF
	pop	ds
	or	ax,ax
	jnz	AAF7
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	A732
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	inc	word ptr [4617]
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,0082
	push	si
	push	di
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	ss
	lea	ax,[bp+FF7E]
	push	ax
	call	AE59
	add	sp,08
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	ss
	lea	ax,[bp-0E]
	push	ax
	call	AEF3
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
	call	AC1D
	add	sp,0A
	or	ax,ax
	jnz	AB8E
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
	call	37F2
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
	cmp	word ptr [09AC],00
	jnz	ABA3
	mov	word ptr [09AE],0000
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	ss
	pop	es
	lea	di,[bp+FF7E]
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
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
	call	AE59
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
	repne	
	scasb	
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
	repne	
	scasb	
	dec	di
	pop	cx
	rep	
	movsb	
	mov	ds,dx
	dec	word ptr [09AC]
	mov	bx,[09AC]
	shl	bx,01
	inc	word ptr [bx+09AE]
	jmp	AB0D
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
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
	repne	
	scasb	
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
	pop	ds
	push	ss
	pop	es
	lea	di,[bp-66]
	mov	si,0A32
	mov	cx,FFFF
	xor	ax,ax
	repne	
	scasb	
	dec	di
	mov	cx,0004
	rep	
	movsb	
	push	ss
	lea	ax,[bp-66]
	push	ax
	mov	ax,0010
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	AD1D
	add	sp,0A
	or	ax,ax
	jz	AC9D
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	mov	ax,0010
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ADC9
	add	sp,06
	or	ax,ax
	jnz	ACA3
	dec	word ptr [bp+0C]
	cmp	word ptr [bp+0C],00
	jnz	AC86
	mov	ax,[bp+0C]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
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
	call	AD1D
	add	sp,0A
	or	ax,ax
	jnz	AD17
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	ss
	lea	ax,[bp-1C]
	push	ax
	call	AEF3
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
	repne	
	scasb	
	not	cx
	sub	di,cx
	rep	
	cmpsb	
	jz	ACFF
	sbb	ax,ax
	sbb	ax,FFFF
	pop	ds
	or	ax,ax
	jnz	AD17
	les	di,[bp+04]
	mov	si,0A36
	mov	cx,FFFF
	xor	ax,ax
	repne	
	scasb	
	dec	di
	mov	cx,0005
	rep	
	movsb	
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
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
	call	2167
	add	sp,0A
	or	ax,ax
	jz	AD41
	mov	ax,0001
	pop	di
	pop	si
	pop	bp
	ret	
	push	ds
	pop	es
	mov	di,0A3B
	mov	si,4641
	mov	cx,0002
	xor	ax,ax
	rep	
	cmpsb	
	jz	AD57
	sbb	ax,ax
	sbb	ax,FFFF
	or	ax,ax
	jz	AD81
	push	ds
	pop	es
	mov	di,0A3D
	mov	si,4641
	mov	cx,0003
	xor	ax,ax
	rep	
	cmpsb	
	jz	AD71
	sbb	ax,ax
	sbb	ax,FFFF
	or	ax,ax
	jz	AD81
	mov	al,[4638]
	cbw	
	and	ax,[bp+08]
	cmp	ax,[bp+08]
	jz	AD94
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ADC9
	add	sp,06
	pop	di
	pop	si
	pop	bp
	ret	
	mov	ax,[bp+04]
	or	ax,[bp+06]
	jz	ADC3
	les	di,[bp+04]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,4641
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
	pop	ds
	xor	ax,ax
	pop	di
	pop	si
	pop	bp
	ret	

fn006B_ADC9()
	push	bp
	mov	bp,sp
	push	si
	push	di
	push	ds
	mov	ax,4623
	push	ax
	call	219A
	add	sp,04
	or	ax,ax
	jz	ADE4

l006B_ADDD:
	mov	ax,0001
	pop	di
	pop	si
	pop	bp
	ret	

l006B_ADE4:
	push	ds
	pop	es
	mov	di,0A3B
	mov	si,4641
	mov	cx,0002
	xor	ax,ax
	rep	
	cmpsb	
	jz	ADFA
	sbb	ax,ax
	sbb	ax,FFFF
	or	ax,ax
	jz	ADCE
	push	ds
	pop	es
	mov	di,0A3D
	mov	si,4641
	mov	cx,0003
	xor	ax,ax
	rep	
	cmpsb	
	jz	AE14
	sbb	ax,ax
	sbb	ax,FFFF
	or	ax,ax
	jz	ADCE
	mov	al,[4638]
	cbw	
	and	ax,[bp+08]
	cmp	ax,[bp+08]
	jnz	ADCE
	mov	ax,[bp+04]
	or	ax,[bp+06]
	jz	AE53
	les	di,[bp+04]
	push	es
	push	ds
	pop	es
	push	di
	mov	di,4641
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
	pop	ds
	xor	ax,ax
	pop	di
	pop	si
	pop	bp
	ret	
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
	call	378B
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
	repne	
	scasb	
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
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
	repne	
	scasb	
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
	repne	
	scasb	
	dec	di
	pop	cx
	rep	
	movsb	
	mov	ds,[bp-16]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
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
	call	378B
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
	repne	
	scasb	
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
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
	repne	
	scasb	
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
	repne	
	scasb	
	dec	di
	pop	cx
	rep	
	movsb	
	mov	ds,[bp-16]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
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
	call	2167
	add	sp,0A
	or	ax,ax
	jz	AFB2
	mov	ax,0001
	mov	sp,bp
	pop	bp
	ret	
	les	bx,[bp+04]
	mov	ax,[bp-16]
	mov	es:[bx],ax
	mov	ax,[bp-14]
	mov	es:[bx+02],ax
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret	
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
	repne	
	scasb	
	not	cx
	mov	ax,005C
	sub	di,cx
	repne	
	scasb	
	jz	AFF7
	mov	di,0001
	xor	ax,ax
	mov	es,ax
	dec	di
	mov	ax,es
	mov	[bp-06],ax
	mov	[bp-08],di
	mov	dx,di
	or	dx,ax
	jz	B082
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	cmp	ax,[bp+06]
	jnz	B016
	cmp	dx,[bp+04]
	jz	B020
	les	bx,[bp-08]
	cmp	byte ptr es:[bx-01],3A
	jnz	B082
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	inc	dx
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	B082
	les	bx,[bp-08]
	mov	byte ptr es:[bx],00
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	mov	ax,0010
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	call	AD1D
	add	sp,0A
	or	ax,ax
	jz	B06E
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	07A2
	add	sp,04
	or	ax,ax
	jz	B06E
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	mov	ax,000E
	push	ax
	call	8674
	add	sp,06
	les	bx,[bp-08]
	mov	byte ptr es:[bx],5C
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	inc	dx
	mov	[bp-02],ax
	mov	[bp-04],dx
	les	di,[bp-04]
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	mov	ax,005C
	sub	di,cx
	repne	
	scasb	
	jz	B09E
	mov	di,0001
	xor	ax,ax
	mov	es,ax
	dec	di
	mov	ax,es
	mov	[bp-06],ax
	mov	[bp-08],di
	mov	dx,di
	or	dx,ax
	jnz	B02F
	xor	ax,ax
	pop	di
	mov	sp,bp
	pop	bp
	ret	
	push	si
	push	di
	cmp	byte ptr [427E],00
	jz	B0E6
	push	ds
	pop	es
	mov	di,44DC
	push	es
	push	di
	mov	di,427E
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	sub	di,cx
	shr	cx,01
	mov	si,di
	pop	di
	mov	ax,es
	pop	es
	push	ds
	mov	ds,ax
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
	pop	ds
	jmp	B0F6
	push	ds
	mov	ax,4541
	push	ax
	push	ds
	mov	ax,44DC
	push	ax
	call	AE59
	add	sp,08
	push	ds
	pop	es
	mov	di,44DC
	mov	si,0A40
	mov	cx,FFFF
	xor	ax,ax
	repne	
	scasb	
	dec	di
	mov	cx,000D
	rep	
	movsb	
	pop	di
	pop	si
	ret	
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
	call	37F2
	add	sp,10
	pop	bp
	ret	
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
	call	AE59
	add	sp,08
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	pop	es
	lea	di,[bp-6A]
	xor	ax,ax
	mov	cx,FFFF
	repne	
	scasb	
	not	cx
	xor	ax,ax
	sub	di,cx
	repne	
	scasb	
	jz	B168
	mov	di,0001
	xor	ax,ax
	mov	es,ax
	dec	di
	mov	ax,es
	push	ax
	push	di
	call	AEF3
	add	sp,08
	push	ds
	mov	ax,0A4D
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	23CE
	add	sp,08
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,dx
	jz	B1B3
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	1F64
	add	sp,04
	mov	ax,0180
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	1E46
	add	sp,06
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	08CF
	add	sp,04
	push	ss
	lea	ax,[bp-6A]
	push	ax
	push	ds
	mov	ax,44DC
	push	ax
	call	339A
	add	sp,08
	cmp	ax,FFFF
	jnz	B1D7
	push	ds
	mov	ax,44DC
	push	ax
	mov	ax,000A
	push	ax
	call	8674
	add	sp,06
	pop	di
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,08
	push	ds
	mov	ax,0A13
	push	ax
	push	ds
	mov	ax,4541
	push	ax
	call	BB84
	add	sp,08
	mov	[29E5],dx
	mov	[29E3],ax
	push	ds
	mov	ax,0A50
	push	ax
	push	ds
	mov	ax,44DC
	push	ax
	call	BB84
	add	sp,08
	mov	[29E1],dx
	mov	[29DF],ax
	push	ds
	mov	ax,44DC
	push	ax
	push	ss
	lea	ax,[bp-04]
	push	ax
	call	AF8D
	add	sp,08
	cmp	byte ptr [0A12],00
	jnz	B23F
	mov	ax,[bp-02]
	mov	[4621],ax
	mov	ax,[bp-04]
	mov	[461F],ax
	mov	byte ptr [0A12],01
	mov	ax,0001
	mov	sp,bp
	pop	bp
	ret	
	push	ds
	mov	ax,4541
	push	ax
	push	ss
	lea	ax,[bp-08]
	push	ax
	call	AF8D
	add	sp,08
	push	ds
	mov	ax,461F
	push	ax
	push	ss
	lea	ax,[bp-08]
	push	ax
	call	B2AB
	add	sp,08
	or	ax,ax
	jl	B277
	push	ss
	lea	ax,[bp-04]
	push	ax
	push	ss
	lea	ax,[bp-08]
	push	ax
	call	B2AB
	add	sp,08
	or	ax,ax
	jle	B27E
	mov	ax,0001
	mov	sp,bp
	pop	bp
	ret	
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	1F64
	add	sp,04
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	1F64
	add	sp,04
	push	ds
	mov	ax,44DC
	push	ax
	call	08CF

fn0010_B850()
	sub	ax,8356
	les	ax,[si]
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret	
0010:B85B                                  55 8B EC 83 EC            U....
0010:B860 02 C4 5E 04 26 8B 47 02 89 46 FE C4 5E 08 26 3B ..^.&.G..F..^.&;
0010:B870 47 02 75 10 C4 5E 04 26 8B 07 C4 5E 08 26 2B 07 G.u..^.&...^.&+.
0010:B880 8B E5 5D C3 8B 46 FE C4 5E 08 26 2B 47 02 8B E5 ..]..F..^.&+G...
0010:B890 5D C3 55 8B EC 83 EC 72 57 80 3E 7E 42 00 74 0E ].U....rW.>~B.t.
0010:B8A0 83 3E 23 2A 01 75 07 83 3E 17 2A 00 75 07 33 C0 .>#*.u..>.*.u.3.
0010:B8B0 5F 8B E5 5D C3 1E B8 DC 44 50 16 8D 46 8E 50 E8 _..]....DP..F.P.
0010:B8C0 47 FB 83 C4 08 1E B8 41 45 50 16 07 8D 7E 8E 33 G......AEP...~.3
0010:B8D0 C0 B9 FF FF F2 AE F7 D1 33 C0 2B F9 F2 AE 74 07 ........3.+...t.
0010:B8E0 BF 01 00 33 C0 8E C0 4F 8C C0 50 57 E8 B4 FB 83 ...3...O..PW....
0010:B8F0 C4 08 1E B8 4D 0A 50 16 8D 46 8E 50 E8 7F 70 83 ....M.P..F.P..p.
0010:B900 C4 08 89 56 FE 89 46 FC 0B C2 75 07 33 C0 5F 8B ...V..F...u.3._.
0010:B910 E5 5D C3 FF 76 FE FF 76 FC E8 78 07 83 C4 04 0B .]..v..v..x.....
0010:B920 D2 77 1A 75 05 3D 12 00 77 13 FF 76 FE FF 76 FC .w.u.=..w..v..v.
0010:B930 E8 E1 6B 83 C4 04 33 C0 5F 8B E5 5D C3 FF 76 FE ..k...3._..]..v.
0010:B940 FF 76 FC E8 17 04 83 C4 04 B1 08 E8 5C 52 83 FA .v..........\R..
0010:B950 52 75 05 3D 43 4E 74 13 FF 76 FE FF 76 FC E8 B3 Ru.=CNt..v..v...
0010:B960 6B 83 C4 04 33 C0 5F 8B E5 5D C3 FF 76 FE FF 76 k...3._..]..v..v
0010:B970 FC E8 E9 03 83 C4 04 3B 16 09 2A 75 06 3B 06 07 .......;..*u.;..
0010:B980 2A 74 13 FF 76 FE FF 76 FC E8 88 6B 83 C4 04 33 *t..v..v...k...3
0010:B990 C0 5F 8B E5 5D C3 16 8D 46 8E 50 16 8D 46 F4 50 ._..]...F.P..F.P
0010:B9A0 E8 9A FB 83 C4 08 1E B8 41 45 50 16 8D 46 F8 50 ........AEP..F.P
0010:B9B0 E8 8A FB 83 C4 08 16 8D 46 F8 50 16 8D 46 F4 50 ........F.P..F.P
0010:B9C0 E8 98 FE 83 C4 08 0B C0 7D 13 FF 76 FE FF 76 FC ........}..v..v.
0010:B9D0 E8 41 6B 83 C4 04 33 C0 5F 8B E5 5D C3 FF 76 FE .Ak...3._..]..v.
0010:B9E0 FF 76 FC E8 77 03 83 C4 04 05 12 00 83 D2 00 89 .v..w...........
0010:B9F0 16 05 2A A3 03 2A FF 76 FE FF 76 FC E8 15 6B 83 ..*..*.v..v...k.
0010:BA00 C4 04 B8 01 00 5F 8B E5 5D C3                   ....._..].     

fn04C4_6ECA()
	push	bp
	mov	bp,sp
	sub	sp,08
	mov	ax,[bp+0C]
	or	ax,[bp+0E]
	jnz	6EDB

l04C4_6ED8:
	jmp	6F7F

l04C4_6EDB:
	cmp	word ptr [bp+0E],00
	jl	6EF1
	jg	6EEA
	cmp	word ptr [bp+0C],FDE8
	jbe	6EF1
	xor	dx,dx
	mov	ax,FDE8
	jmp	6EF7
	mov	dx,[bp+0E]
	mov	ax,[bp+0C]
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [bp-06]
	push	ax
	call	76D1
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	jmp	6F6B
	cmp	word ptr [bp+0E],00
	jl	6F25
	jg	6F1E
	cmp	word ptr [bp+0C],FDE8
	jbe	6F25
	xor	dx,dx
	mov	ax,FDE8
	jmp	6F2B
	mov	dx,[bp+0E]
	mov	ax,[bp+0C]
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [bp-06]
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	74D0
	add	sp,0C
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	7512
	add	sp,0C
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	sub	[bp+0C],dx
	sbb	[bp+0E],ax
	mov	ax,[bp+0C]
	or	ax,[bp+0E]
	jnz	6F0F
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	7706
	add	sp,04

l04C4_6F7F:
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,04
	push	ds
	mov	ax,0A54
	push	ax
	push	ds
	mov	ax,43AD
	push	ax
	call	6B7E
	add	sp,08
	push	ds
	mov	ax,0A61
	push	ax
	push	ds
	mov	ax,43AD
	push	ax
	call	75F4
	add	sp,08
	mov	[29D5],dx
	mov	[29D3],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	7554
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
	call	E073
	add	sp,0A
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29D5]
	push	word ptr [29D3]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	6ECA
	add	sp,0C
	push	word ptr [29D5]
	push	word ptr [29D3]
	call	EE27
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
	call	E073
	add	sp,0A
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	word ptr [29D5]
	push	word ptr [29D3]
	call	6ECA
	add	sp,0C
	push	word ptr [29D5]
	push	word ptr [29D3]
	call	D9D4
	add	sp,04
	push	ds
	mov	ax,43AD
	push	ax
	call	C33F
	add	sp,04
	mov	sp,bp
	pop	bp
	ret	
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
	call	68C9
	add	sp,08
	push	ss
	pop	es
	lea	di,[bp-6A]
	mov	si,0A54
	mov	cx,FFFF
	xor	ax,ax
	repne	
	scasb	
	dec	di
	mov	cx,000D
	rep	
	movsb	
	push	ds
	mov	ax,0A61
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	75F4
	add	sp,08
	mov	[bp-02],dx
	mov	[bp-04],ax
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	E073
	add	sp,0A
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	6ECA
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
	call	E073
	add	sp,0A
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	7554
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
	call	6ECA
	add	sp,0C
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	D9D4
	add	sp,04
	push	word ptr [29D1]
	push	word ptr [29CF]
	call	D9D4
	add	sp,04
	mov	ax,0180
	push	ax
	push	ds
	mov	ax,4348
	push	ax
	call	D8B6
	add	sp,06
	push	ds
	mov	ax,4348
	push	ax
	call	C33F
	add	sp,04
	push	ds
	mov	ax,4348
	push	ax
	push	ss
	lea	ax,[bp-6A]
	push	ax
	call	EE0A
	add	sp,08
	cmp	ax,FFFF
	jnz	7172
	push	ss
	lea	ax,[bp-6A]
	push	ax
	mov	ax,000A
	push	ax
	call	40E4
	add	sp,06
	push	ds
	mov	ax,0A65
	push	ax
	push	ds
	mov	ax,4348
	push	ax
	call	75F4
	add	sp,08
	mov	[29D1],dx
	mov	[29CF],ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	

fn04C4_718F()
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	les	bx,[bp+04]
	dec	word ptr es:[bx]
	jl	71B3

l04C4_719E:
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	mov	es,ax
	mov	al,es:[si]
	mov	ah,00
	jmp	71BF

l04C4_71B3:
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	E282
	add	sp,04

l04C4_71BF:
	mov	[bp-02],ax
	cmp	ax,FFFF
	jnz	71DF
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	7631
	add	sp,04
	push	dx
	push	ax
	mov	ax,0008
	push	ax
	call	40E4
	add	sp,06
	mov	al,[bp-02]
	pop	si
	mov	sp,bp
	pop	bp
	ret	

fn04C4_71E7()
	push	bp
	mov	bp,sp
	sub	sp,04
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	718F
	add	sp,04
	mov	ah,00
	mov	[bp-02],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	718F
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

fn04C4_721D()
	push	bp
	mov	bp,sp
	sub	sp,08
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	71E7
	add	sp,04
	mov	word ptr [bp-02],0000
	mov	[bp-04],ax
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	71E7
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
04C4:725A                               55 8B EC 83 EC 02           U.....
04C4:7260 56 C4 5E 04 26 FF 0F 7C 15 26 8B 47 0E 26 8B 77 V.^.&..|.&.G.&.w
04C4:7270 0C 26 FF 47 0C 8E C0 26 8A 04 B4 00 EB 0C FF 76 .&.G...&.......v
04C4:7280 06 FF 76 04 E8 FB 6F 83 C4 04 89 46 FE 3D FF FF ..v...o....F.=..
04C4:7290 75 18 FF 76 06 FF 76 04 E8 96 03 83 C4 04 52 50 u..v..v.......RP
04C4:72A0 B8 08 00 50 E8 3D CE 83 C4 06 B8 01 00 50 B8 FF ...P.=.......P..
04C4:72B0 FF BA FF FF 50 52 FF 76 06 FF 76 04 E8 B4 6D 83 ....PR.v..v...m.
04C4:72C0 C4 0A 8A 46 FE 5E 8B E5 5D C3 55 8B EC 83 EC 04 ...F.^..].U.....
04C4:72D0 FF 76 06 FF 76 04 E8 B6 FE 83 C4 04 B4 00 89 46 .v..v..........F
04C4:72E0 FE FF 76 06 FF 76 04 E8 A5 FE 83 C4 04 B4 00 89 ..v..v..........
04C4:72F0 46 FC B8 01 00 50 B8 FF FF BA FE FF 50 52 FF 76 F....P......PR.v
04C4:7300 06 FF 76 04 E8 6C 6D 83 C4 0A 8B 46 FE B1 08 D3 ..v..lm....F....
04C4:7310 E0 03 46 FC 8B E5 5D C3 55 8B EC 83 EC 08 FF 76 ..F...].U......v
04C4:7320 06 FF 76 04 E8 C0 FE 83 C4 04 C7 46 FE 00 00 89 ..v........F....
04C4:7330 46 FC FF 76 06 FF 76 04 E8 AC FE 83 C4 04 C7 46 F..v..v........F
04C4:7340 FA 00 00 89 46 F8 B8 01 00 50 B8 FF FF BA FC FF ....F....P......
04C4:7350 50 52 FF 76 06 FF 76 04 E8 18 6D 83 C4 0A 8B 56 PR.v..v...m....V
04C4:7360 FC 33 C0 03 46 F8 13 56 FA 8B E5 5D C3 55 8B EC .3..F..V...].U..
04C4:7370 83 EC 04 FF 76 06 FF 76 04 E8 13 FE 83 C4 04 B4 ....v..v........
04C4:7380 00 89 46 FE FF 76 06 FF 76 04 E8 02 FE 83 C4 04 ..F..v..v.......
04C4:7390 B4 00 89 46 FC B1 08 D3 E0 03 46 FE 8B E5 5D C3 ...F......F...].
04C4:73A0 55 8B EC 8B 46 04 0B 46 06 74 1B FF 76 0A FF 76 U...F..F.t..v..v
04C4:73B0 08 E8 DB FD 83 C4 04 C4 5E 04 FF 46 04 26 88 07 ........^..F.&..
04C4:73C0 0A C0 75 E7 EB 10 FF 76 0A FF 76 08 E8 C0 FD 83 ..u....v..v.....
04C4:73D0 C4 04 0A C0 75 F0 8B 56 06 8B 46 04 5D C3       ....u..V..F.]. 

fn04C4_73DE()
	push	bp
	mov	bp,sp
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	mov	ax,[bp+04]
	mov	cl,08
	shr	ax,cl
	push	ax
	call	7407
	add	sp,06
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	mov	al,[bp+04]
	push	ax
	call	7407
	add	sp,06
	pop	bp
	ret	

fn04C4_7407()
	push	bp
	mov	bp,sp
	push	si
	mov	cl,[bp+04]
	les	bx,[bp+06]
	inc	word ptr es:[bx]
	jge	742F

l04C4_7416:
	mov	ax,es:[bx+0E]
	mov	si,es:[bx+0C]
	inc	word ptr es:[bx+0C]
	mov	dl,cl
	mov	es,ax
	mov	es:[si],dl
	mov	al,dl
	mov	ah,00
	jmp	743C

l04C4_742F:
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	cx
	call	E6CA
	add	sp,06

l04C4_743C:
	cmp	ax,FFFF
	jnz	7459
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	call	7631
	add	sp,04
	push	dx
	push	ax
	mov	ax,0009
	push	ax
	call	40E4
	add	sp,06
	pop	si
	pop	bp
	ret	

fn04C4_745C()
	push	bp
	mov	bp,sp
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	call	73DE
	add	sp,06
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	word ptr [bp+04]
	call	73DE
	add	sp,06
	pop	bp
	ret	
04C4:747F                                              55                U
04C4:7480 8B EC FF 76 08 FF 76 06 8A 46 04 24 FF 50 E8 76 ...v..v..F.$.P.v
04C4:7490 FF 83 C4 06 FF 76 08 FF 76 06 8B 46 04 B1 08 D3 .....v..v..F....
04C4:74A0 E8 24 FF 50 E8 60 FF 83 C4 06 5D C3 55 8B EC FF .$.P.`....].U...
04C4:74B0 76 0A FF 76 08 C4 5E 04 26 8A 07 50 E8 48 FF 83 v..v..^.&..P.H..
04C4:74C0 C4 06 C4 5E 04 FF 46 04 26 80 3F 00 75 E1 5D C3 ...^..F.&.?.u.].
04C4:74D0 55 8B EC FF 76 0E FF 76 0C FF 76 08 B8 01 00 50 U...v..v..v....P
04C4:74E0 FF 76 06 FF 76 04 E8 7A 6A 83 C4 0C 33 D2 3B 56 .v..v..zj...3.;V
04C4:74F0 0A 75 05 3B 46 08 74 18 FF 76 0E FF 76 0C E8 30 .u.;F.t..v..v..0
04C4:7500 01 83 C4 04 52 50 B8 08 00 50 E8 D7 CB 83 C4 06 ....RP...P......
04C4:7510 5D C3 55 8B EC FF 76 0E FF 76 0C FF 76 08 B8 01 ].U...v..v..v...
04C4:7520 00 50 FF 76 06 FF 76 04 E8 1A 6C 83 C4 0C 33 D2 .P.v..v...l...3.
04C4:7530 3B 56 0A 75 05 3B 46 08 74 18 FF 76 0E FF 76 0C ;V.u.;F.t..v..v.
04C4:7540 E8 EE 00 83 C4 04 52 50 B8 09 00 50 E8 95 CB 83 ......RP...P....
04C4:7550 C4 06 5D C3 55 8B EC 83 EC 08 FF 76 06 FF 76 04 ..].U......v..v.
04C4:7560 E8 8C 6B 83 C4 04 89 56 FE 89 46 FC B8 02 00 50 ..k....V..F....P
04C4:7570 33 C0 50 50 FF 76 06 FF 76 04 E8 F6 6A 83 C4 0A 3.PP.v..v...j...
04C4:7580 FF 76 06 FF 76 04 E8 66 6B 83 C4 04 89 56 FA 89 .v..v..fk....V..
04C4:7590 46 F8 33 C0 50 FF 76 FE FF 76 FC FF 76 06 FF 76 F.3.P.v..v..v..v
04C4:75A0 04 E8 CF 6A 83 C4 0A 8B 56 FA 8B 46 F8 8B E5 5D ...j....V..F...]
04C4:75B0 C3 55 8B EC 83 EC 08 1E B8 69 0A 50 FF 76 06 FF .U.......i.P.v..
04C4:75C0 76 04 E8 2F 00 83 C4 08 89 56 FE 89 46 FC FF 76 v../.....V..F..v
04C4:75D0 FE 50 E8 7F FF 83 C4 04 89 56 FA 89 46 F8 FF 76 .P.......V..F..v
04C4:75E0 FE FF 76 FC E8 ED 63 83 C4 04 8B 56 FA 8B 46 F8 ..v...c....V..F.
04C4:75F0 8B E5 5D C3 55 8B EC 83 EC 04 FF 76 0A FF 76 08 ..].U......v..v.
04C4:7600 FF 76 06 FF 76 04 E8 35 68 83 C4 08 89 56 FE 89 .v..v..5h....V..
04C4:7610 46 FC 0B 46 FE 75 10 FF 76 06 FF 76 04 B8 07 00 F..F.u..v..v....
04C4:7620 50 E8 C0 CA 83 C4 06 8B 56 FE 8B 46 FC 8B E5 5D P.......V..F...]
04C4:7630 C3 55 8B EC 8B 46 06 8B 56 04 3B 06 E5 29 75 0D .U...F..V.;..)u.
04C4:7640 3B 16 E3 29 75 07 8C DA B8 41 45 5D C3 8B 46 06 ;..)u....AE]..F.
04C4:7650 8B 56 04 3B 06 E1 29 75 0D 3B 16 DF 29 75 07 8C .V.;..)u.;..)u..
04C4:7660 DA B8 DC 44 5D C3 8B 46 06 8B 56 04 3B 06 D1 29 ...D]..F..V.;..)
04C4:7670 75 0D 3B 16 CF 29 75 07 8C DA B8 48 43 5D C3 8B u.;..)u....HC]..
04C4:7680 46 06 8B 56 04 3B 06 DD 29 75 0D 3B 16 DB 29 75 F..V.;..)u.;..)u
04C4:7690 07 8C DA B8 77 44 5D C3 8B 46 06 8B 56 04 3B 06 ....wD]..F..V.;.
04C4:76A0 D9 29 75 0D 3B 16 D7 29 75 07 8C DA B8 12 44 5D .)u.;..)u.....D]
04C4:76B0 C3 8B 46 06 8B 56 04 3B 06 D5 29 75 0D 3B 16 D3 ..F..V.;..)u.;..
04C4:76C0 29 75 07 8C DA B8 AD 43 5D C3 8C DA B8 60 0A 5D )u.....C]....`.]
04C4:76D0 C3                                              .              

fn0800_4311()
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

fn0800_4346()
	push	bp
	mov	bp,sp
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	9E75
	add	sp,04
	pop	bp
	ret	
0800:4357                      55 8B EC EB 5D C4 5E 08 26        U...].^.&
0800:4360 8A 07 8A D0 98 3D 2A 00 74 0F 3D 2E 00 74 2F 3D .....=*.t.=..t/=
0800:4370 3F 00 74 16 EB 31 FF 46 04 C4 5E 04 26 80 3F 2E ?.t..1.F..^.&.?.
0800:4380 74 34 26 80 3F 00 75 EE EB 2C C4 5E 04 26 80 3F t4&.?.u..,.^.&.?
0800:4390 2E 74 23 26 80 3F 00 74 1D FF 46 04 EB 18 C4 5E .t#&.?.t..F....^
0800:43A0 04 26 80 3F 00 74 0F C4 5E 04 26 3A 17 74 04 33 .&.?.t..^.&:.t.3
0800:43B0 C0 5D C3 FF 46 04 FF 46 08 C4 5E 08 26 80 3F 00 .]..F..F..^.&.?.
0800:43C0 75 9A C4 5E 04 26 80 3F 00 75 05 B8 01 00 EB 02 u..^.&.?.u......
0800:43D0 33 C0 5D C3 55 8B EC 56 8B 4E 08 8B 76 04 8B C1 3.].U..V.N..v...
0800:43E0 BA 0C 00 F7 EA 03 F0 EB 26 8E 46 06 26 C7 44 02 ........&.F.&.D.
0800:43F0 00 00 26 C7 04 00 00 26 C7 44 04 FF FF 26 C7 44 ..&....&.D...&.D
0800:4400 08 00 00 26 C7 44 06 00 00 26 C7 44 0A 00 00 83 ...&.D...&.D....
0800:4410 EE 0C 8B C1 49 0B C0 75 D0 5E 5D C3 55 8B EC 83 ....I..u.^].U...
0800:4420 EC 02 56 57 33 FF 8B CF 8B 76 04 8B C1 BA 0C 00 ..VW3....v......
0800:4430 F7 EA 03 F0 3B 4E 08 73 19 8E 46 06 26 8B 04 26 ....;N.s..F.&..&
0800:4440 0B 44 02 74 04 47 89 4E FE 83 C6 0C 41 3B 4E 08 .D.t.G.N....A;N.
0800:4450 72 E7 0B FF 75 03 E9 F1 00 83 FF 01 74 03 E9 C4 r...u.......t...
0800:4460 00 8B 46 FE BA 0C 00 F7 EA C4 5E 04 03 D8 26 FF ..F.......^...&.
0800:4470 47 0A 5F 5E 8B E5 5D C3 A1 58 46 BA 0C 00 F7 EA G._^..]..XF.....
0800:4480 8B C8 C4 5E 04 03 D8 26 8B 47 02 26 8B 17 50 A1 ...^...&.G.&..P.
0800:4490 5A 46 BB 0C 00 52 F7 EB 8B D0 8B 5E 04 03 D8 58 ZF...R.....^...X
0800:44A0 26 01 07 58 26 11 47 02 8B 5E 04 03 D9 26 C7 47 &..X&.G..^...&.G
0800:44B0 02 00 00 26 C7 07 00 00 8B 5E 04 03 DA 26 FF 47 ...&.....^...&.G
0800:44C0 0A EB 1A 8E 46 06 26 8B 44 04 A3 5A 46 BA 0C 00 ....F.&.D..ZF...
0800:44D0 F7 EA 8B D0 8B 5E 04 03 D8 26 FF 47 0A C4 5E 04 .....^...&.G..^.
0800:44E0 03 DA 8B F3 26 83 7F 04 FF 75 D8 A1 58 46 26 89 ....&....u..XF&.
0800:44F0 44 04 8B 5E 04 03 D9 26 FF 47 0A EB 1A 8E 46 06 D..^...&.G....F.
0800:4500 26 8B 44 04 A3 58 46 BA 0C 00 F7 EA 8B C8 8B 5E &.D..XF........^
0800:4510 04 03 D8 26 FF 47 0A C4 5E 04 03 D9 8B F3 26 83 ...&.G..^.....&.
0800:4520 7F 04 FF 75 D8 FF 76 08 FF 76 06 FF 76 04 E8 0A ...u..v..v..v...
0800:4530 01 83 C4 06 0B C0 74 03 E9 3D FF FF 76 08 FF 76 ......t..=..v..v
0800:4540 06 FF 76 04 E8 09 00 83 C4 06 5F 5E 8B E5 5D C3 ..v......._^..].
0800:4550 55 8B EC 83 EC 0A 56 57 C7 46 FE 00 00 C7 46 FC U.....VW.F....F.
0800:4560 00 00 C7 46 FA 00 80 C7 46 F8 00 00 C7 46 F6 01 ...F....F....F..
0800:4570 00 EB 63 33 FF 8B 76 04 3B 7E 08 73 46 8E 46 06 ..c3..v.;~.sF.F.
0800:4580 26 8B 44 0A 3B 46 F6 75 31 FF 76 F6 FF 76 FA FF &.D.;F.u1.v..v..
0800:4590 76 F8 FF 76 FE FF 76 FC E8 27 46 52 50 E8 42 00 v..v..v..'FRP.B.
0800:45A0 83 C4 06 8E 46 06 26 89 54 08 26 89 44 06 8B 46 ....F.&.T.&.D..F
0800:45B0 FA 8B 56 F8 01 56 FC 11 46 FE 83 C6 0C 47 3B 7E ..V..V..F....G;~
0800:45C0 08 72 BA FF 46 F6 8B 46 FA 8B 56 F8 D1 E8 D1 DA .r..F..F..V.....
0800:45D0 89 46 FA 89 56 F8 83 7E F6 10 76 97 5F 5E 8B E5 .F..V..~..v._^..
0800:45E0 5D C3 55 8B EC 83 EC 04 8B 5E 08 C7 46 FE 00 00 ].U......^..F...
0800:45F0 C7 46 FC 00 00 EB 33 8B 46 FE 8B 56 FC D1 E2 D1 .F....3.F..V....
0800:4600 D0 89 46 FE 89 56 FC 8B 46 04 25 01 00 0D 00 00 ..F..V..F.%.....
0800:4610 74 08 83 4E FC 01 83 4E FE 00 8B 46 06 8B 56 04 t..N...N...F..V.
0800:4620 D1 E8 D1 DA 89 46 06 89 56 04 8B C3 4B 0B C0 75 .....F..V...K..u
0800:4630 C6 8B 56 FE 8B 46 FC 8B E5 5D C3 55 8B EC 83 EC ..V..F...].U....
0800:4640 0C 56 57 8B 7E 08 B8 FF FF BA FF FF 89 46 F6 89 .VW.~........F..
0800:4650 56 F4 89 46 FA 89 56 F8 33 C9 8B 76 04 3B CF 73 V..F..V.3..v.;.s
0800:4660 74 8E 46 06 26 8B 44 02 26 8B 14 89 46 FE 89 56 t.F.&.D.&...F..V
0800:4670 FC 0B D0 74 58 8B 46 FE 8B 56 FC 3B 46 FA 77 2B ...tX.F..V.;F.w+
0800:4680 72 05 3B 56 F8 73 24 8B 46 FA 8B 56 F8 89 46 F6 r.;V.s$.F..V..F.
0800:4690 89 56 F4 A1 5A 46 A3 58 46 8B 46 FE 8B 56 FC 89 .V..ZF.XF.F..V..
0800:46A0 46 FA 89 56 F8 89 0E 5A 46 EB 22 8B 46 FE 8B 56 F..V...ZF.".F..V
0800:46B0 FC 3B 46 F6 77 17 72 05 3B 56 F4 73 10 8B 46 FE .;F.w.r.;V.s..F.
0800:46C0 8B 56 FC 89 46 F6 89 56 F4 89 0E 58 46 83 C6 0C .V..F..V...XF...
0800:46D0 41 3B CF 72 8C 83 7E FA FF 75 06 83 7E F8 FF 74 A;.r..~..u..~..t
0800:46E0 0C 83 7E F6 FF 75 0E 83 7E F4 FF 75 08 33 C0 5F ..~..u..~..u.3._
0800:46F0 5E 8B E5 5D C3 B8 01 00 5F 5E 8B E5 5D C3 55 8B ^..]...._^..].U.
0800:4700 EC 83 EC 20 56 57 83 3E 05 2A 00 77 12 72 07 83 ... VW.>.*.w.r..
0800:4710 3E 03 2A 2A 73 09 B8 07 00 5F 5E 8B E5 5D C3 33 >.**s...._^..].3
0800:4720 C0 50 BA 28 00 50 52 FF 36 E5 29 FF 36 E3 29 E8 .P.(.PR.6.).6.).
0800:4730 81 65 83 C4 0A FF 36 E5 29 FF 36 E3 29 E8 6D F8 .e....6.).6.).m.
0800:4740 83 C4 04 8B F8 05 24 00 33 D2 3B 16 05 2A 72 11 ......$.3.;..*r.
0800:4750 77 06 3B 06 03 2A 76 09 B8 07 00 5F 5E 8B E5 5D w.;..*v...._^..]
0800:4760 C3 33 C0 50 8B C7 05 20 00 33 D2 52 50 FF 36 E5 .3.P... .3.RP.6.
0800:4770 29 FF 36 E3 29 E8 3B 65 83 C4 0A FF 36 E5 29 FF ).6.).;e....6.).
0800:4780 36 E3 29 E8 D2 F7 83 C4 04 B1 08 E8 1C 45 83 FA 6.)..........E..
0800:4790 52 75 05 3D 43 4E 74 09 B8 07 00 5F 5E 8B E5 5D Ru.=CNt...._^..]
0800:47A0 C3 FF 36 E5 29 FF 36 E3 29 E8 83 65 83 C4 04 89 ..6.).6.)..e....
0800:47B0 56 FA 89 46 F8 33 C0 50 BA 02 00 50 52 FF 36 E5 V..F.3.P...PR.6.
0800:47C0 29 FF 36 E3 29 E8 EB 64 83 C4 0A FF 36 E5 29 FF ).6.)..d....6.).
0800:47D0 36 E3 29 E8 D7 F7 83 C4 04 8B F8 FF 36 E5 29 FF 6.).........6.).
0800:47E0 36 E3 29 E8 C7 F7 83 C4 04 89 46 F6 0B FF 74 03 6.).......F...t.
0800:47F0 FF 4E F6 8B 46 F6 33 D2 B1 09 E8 6C 44 03 C7 83 .N..F.3....lD...
0800:4800 D2 00 89 56 FE 89 46 FC 33 C0 50 BA 16 00 50 52 ...V..F.3.P...PR
0800:4810 FF 36 E5 29 FF 36 E3 29 E8 98 64 83 C4 0A FF 36 .6.).6.)..d....6
0800:4820 E5 29 FF 36 E3 29 E8 84 F7 83 C4 04 FF 36 E5 29 .).6.).......6.)
0800:4830 FF 36 E3 29 E8 76 F7 83 C4 04 89 46 E4 FF 36 E5 .6.).v.....F..6.
0800:4840 29 FF 36 E3 29 E8 65 F7 83 C4 04 89 46 E2 FF 36 ).6.).e.....F..6
0800:4850 E5 29 FF 36 E3 29 E8 54 F7 83 C4 04 89 46 F0 FF .).6.).T.....F..
0800:4860 36 E5 29 FF 36 E3 29 E8 43 F7 83 C4 04 89 46 EE 6.).6.).C.....F.
0800:4870 FF 36 E5 29 FF 36 E3 29 E8 32 F7 83 C4 04 89 46 .6.).6.).2.....F
0800:4880 E8 FF 36 E5 29 FF 36 E3 29 E8 21 F7 83 C4 04 89 ..6.).6.).!.....
0800:4890 46 E6 FF 36 E5 29 FF 36 E3 29 E8 10 F7 83 C4 04 F..6.).6.)......
0800:48A0 89 46 EA FF 36 E5 29 FF 36 E3 29 E8 FF F6 83 C4 .F..6.).6.).....
0800:48B0 04 89 46 EC FF 36 E1 29 FF 36 DF 29 B8 5A 4D 50 ..F..6.).6.).ZMP
0800:48C0 E8 5B F7 83 C4 06 FF 36 E1 29 FF 36 DF 29 33 C0 .[.....6.).6.)3.
0800:48D0 50 50 E8 C7 F7 83 C4 08 FF 36 E1 29 FF 36 DF 29 PP.......6.).6.)
0800:48E0 33 C0 50 50 E8 B5 F7 83 C4 08 FF 36 E1 29 FF 36 3.PP.......6.).6
0800:48F0 DF 29 FF 76 F0 E8 C7 F7 83 C4 06 FF 36 E1 29 FF .).v........6.).
0800:4900 36 DF 29 FF 76 EE E8 B6 F7 83 C4 06 FF 36 E1 29 6.).v........6.)
0800:4910 FF 36 DF 29 FF 76 EC E8 A5 F7 83 C4 06 FF 36 E1 .6.).v........6.
0800:4920 29 FF 36 DF 29 FF 76 EA E8 94 F7 83 C4 06 FF 36 ).6.).v........6
0800:4930 E1 29 FF 36 DF 29 33 C0 50 E8 E2 F6 83 C4 06 FF .).6.)3.P.......
0800:4940 36 E1 29 FF 36 DF 29 FF 76 E8 E8 72 F7 83 C4 06 6.).6.).v..r....
0800:4950 FF 36 E1 29 FF 36 DF 29 FF 76 E6 E8 61 F7 83 C4 .6.).6.).v..a...
0800:4960 06 FF 36 E1 29 FF 36 DF 29 B8 1E 00 50 E8 4F F7 ..6.).6.)...P.O.
0800:4970 83 C4 06 FF 36 E1 29 FF 36 DF 29 FF 76 E2 E8 3E ....6.).6.).v..>
0800:4980 F7 83 C4 06 FF 36 E1 29 FF 36 DF 29 33 C0 50 E8 .....6.).6.)3.P.
0800:4990 8C F6 83 C4 06 33 C0 50 8B 46 E4 05 20 00 33 D2 .....3.P.F.. .3.
0800:49A0 52 50 FF 36 E5 29 FF 36 E3 29 E8 06 63 83 C4 0A RP.6.).6.)..c...
0800:49B0 C7 46 F4 00 00 FF 36 E5 29 FF 36 E3 29 E8 0F F4 .F....6.).6.)...
0800:49C0 83 C4 04 B4 00 8B F8 0B FF 74 57 FF 36 E5 29 FF .........tW.6.).
0800:49D0 36 E3 29 E8 D7 F5 83 C4 04 89 46 E0 33 F6 89 7E 6.).......F.3..~
0800:49E0 F6 EB 32 FF 36 E5 29 FF 36 E3 29 E8 E1 F3 83 C4 ..2.6.).6.).....
0800:49F0 04 B4 00 03 F0 FF 36 E1 29 FF 36 DF 29 56 E8 BE ......6.).6.)V..
0800:4A00 F6 83 C4 06 FF 36 E1 29 FF 36 DF 29 FF 76 E0 E8 .....6.).6.).v..
0800:4A10 AD F6 83 C4 06 8B 46 F6 FF 4E F6 0B C0 75 C4 01 ......F..N...u..
0800:4A20 7E F4 0B FF 75 8F FF 36 E1 29 FF 36 DF 29 E8 FE ~...u..6.).6.)..
0800:4A30 62 83 C4 04 0B D2 7F 1E 7C 05 3D 00 02 73 17 FF b.......|.=..s..
0800:4A40 36 E1 29 FF 36 DF 29 E8 E5 62 83 C4 04 BA 00 02 6.).6.)..b......
0800:4A50 2B D0 8B FA EB 2B FF 36 E1 29 FF 36 DF 29 E8 CE +....+.6.).6.)..
0800:4A60 62 83 C4 04 BA 10 00 2B D0 83 E2 0F 8B FA EB 11 b......+........
0800:4A70 FF 36 E1 29 FF 36 DF 29 B0 00 50 E8 C9 F5 83 C4 .6.).6.)..P.....
0800:4A80 06 8B C7 4F 0B C0 75 E8 FF 36 E1 29 FF 36 DF 29 ...O..u..6.).6.)
0800:4A90 E8 9C 62 83 C4 04 B1 04 E8 EF 41 89 46 F2 33 C0 ..b.......A.F.3.
0800:4AA0 50 FF 76 FA FF 76 F8 FF 36 E5 29 FF 36 E3 29 E8 P.v..v..6.).6.).
0800:4AB0 01 62 83 C4 0A E8 BC 08 8B D0 0B D2 74 06 5F 5E .b..........t._^
0800:4AC0 8B E5 5D C3 FF 36 E1 29 FF 36 DF 29 E8 60 62 83 ..]..6.).6.).`b.
0800:4AD0 C4 04 89 56 FA 89 46 F8 33 C0 50 FF 76 FE FF 76 ...V..F.3.P.v..v
0800:4AE0 FC FF 36 E5 29 FF 36 E3 29 E8 C7 61 83 C4 0A FF ..6.).6.)..a....
0800:4AF0 36 E5 29 FF 36 E3 29 E8 9A F6 83 C4 04 2B 46 FC 6.).6.)......+F.
0800:4B00 1B 56 FE 52 50 FF 36 E1 29 FF 36 DF 29 FF 36 E5 .V.RP.6.).6.).6.
0800:4B10 29 FF 36 E3 29 E8 F2 EF 83 C4 0C 8B 7E F8 81 E7 ).6.).......~...
0800:4B20 FF 01 8B 56 FA 8B 46 F8 B1 09 E8 7D 41 89 46 F6 ...V..F....}A.F.
0800:4B30 0B FF 74 03 FF 46 F6 33 C0 50 BA 02 00 50 52 FF ..t..F.3.P...PR.
0800:4B40 36 E1 29 FF 36 DF 29 E8 69 61 83 C4 0A FF 36 E1 6.).6.).ia....6.
0800:4B50 29 FF 36 DF 29 57 E8 66 F5 83 C4 06 FF 36 E1 29 ).6.)W.f.....6.)
0800:4B60 FF 36 DF 29 FF 76 F6 E8 55 F5 83 C4 06 FF 36 E1 .6.).v..U.....6.
0800:4B70 29 FF 36 DF 29 FF 76 F4 E8 44 F5 83 C4 06 FF 36 ).6.).v..D.....6
0800:4B80 E1 29 FF 36 DF 29 FF 76 F2 E8 33 F5 83 C4 06 33 .).6.).v..3....3
0800:4B90 C0 5F 5E 8B E5 5D C3 FF 36 E5 29 FF 36 E3 29 E8 ._^..]..6.).6.).
0800:4BA0 2C 12 83 C4 04 0B C0 75 04 B8 07 00 C3 E8 C4 07 ,......u........
0800:4BB0 C3 FF 36 E5 29 FF 36 E3 29 E8 12 12 83 C4 04 0B ..6.).6.).......
0800:4BC0 C0 75 04 B8 07 00 C3 B8 01 00 50 B8 FF FF BA EE .u........P.....
0800:4BD0 FF 50 52 FF 36 E5 29 FF 36 E3 29 E8 D5 60 83 C4 .PR.6.).6.)..`..
0800:4BE0 0A FF 36 E1 29 FF 36 DF 29 B8 1A 60 50 E8 2E F4 ..6.).6.)..`P...
0800:4BF0 83 C4 06 33 C0 BA 10 00 50 52 FF 36 E1 29 FF 36 ...3....PR.6.).6
0800:4C00 DF 29 FF 36 E5 29 FF 36 E3 29 E8 FD EE 83 C4 0C .).6.).6.)......
0800:4C10 FF 36 E1 29 FF 36 DF 29 33 C0 50 50 E8 7D F4 83 .6.).6.)3.PP.}..
0800:4C20 C4 08 FF 36 E1 29 FF 36 DF 29 33 C0 50 50 E8 6B ...6.).6.)3.PP.k
0800:4C30 F4 83 C4 08 FF 36 E1 29 FF 36 DF 29 FF 36 E5 29 .....6.).6.).6.)
0800:4C40 FF 36 E3 29 E8 E0 F1 83 C4 04 50 E8 D0 F3 83 C4 .6.)......P.....
0800:4C50 06 E8 20 07 C3 55 8B EC 83 EC 18 FF 36 E5 29 FF .. ..U......6.).
0800:4C60 36 E3 29 E8 F7 F1 83 C4 04 0B D2 75 05 3D F3 03 6.)........u.=..
0800:4C70 74 07 B8 07 00 8B E5 5D C3 FF 36 E1 29 FF 36 DF t......]..6.).6.
0800:4C80 29 33 C0 BA F3 03 50 52 E8 11 F4 83 C4 08 FF 36 )3....PR.......6
0800:4C90 E5 29 FF 36 E3 29 E8 C4 F1 83 C4 04 89 56 FA 89 .).6.).......V..
0800:4CA0 46 F8 89 56 FE 89 46 FC FF 36 E1 29 FF 36 DF 29 F..V..F..6.).6.)
0800:4CB0 FF 76 FE 50 E8 E5 F3 83 C4 08 EB 1E FF 36 E1 29 .v.P.........6.)
0800:4CC0 FF 36 DF 29 FF 36 E5 29 FF 36 E3 29 E8 8E F1 83 .6.).6.).6.)....
0800:4CD0 C4 04 52 50 E8 C5 F3 83 C4 08 8B 46 F8 8B 56 FA ..RP.......F..V.
0800:4CE0 83 6E F8 01 83 5E FA 00 0B C2 75 D0 8B 46 FC 0B .n...^....u..F..
0800:4CF0 46 FE 75 9A FF 36 E1 29 FF 36 DF 29 FF 36 E5 29 F.u..6.).6.).6.)
0800:4D00 FF 36 E3 29 E8 56 F1 83 C4 04 2D 01 00 83 DA 00 .6.).V....-.....
0800:4D10 52 50 E8 87 F3 83 C4 08 FF 36 E5 29 FF 36 E3 29 RP.......6.).6.)
0800:4D20 E8 3A F1 83 C4 04 89 56 F6 89 46 F4 FF 36 E5 29 .:.....V..F..6.)
0800:4D30 FF 36 E3 29 E8 26 F1 83 C4 04 89 56 F2 89 46 F0 .6.).&.....V..F.
0800:4D40 8B 46 F2 8B 56 F0 2B 56 F4 1B 46 F6 89 46 EE 89 .F..V.+V..F..F..
0800:4D50 56 EC FF 36 E1 29 FF 36 DF 29 FF 76 F6 FF 76 F4 V..6.).6.).v..v.
0800:4D60 E8 39 F3 83 C4 08 FF 36 E1 29 FF 36 DF 29 8B 46 .9.....6.).6.).F
0800:4D70 F2 8B 56 F0 83 EA 01 1D 00 00 50 52 E8 1D F3 83 ..V.......PR....
0800:4D80 C4 08 FF 36 E1 29 FF 36 DF 29 E8 A2 5F 83 C4 04 ...6.).6.).._...
0800:4D90 89 16 8E 4E A3 8C 4E B8 01 00 50 33 C0 BA 04 00 ...N..N...P3....
0800:4DA0 50 52 FF 36 E5 29 FF 36 E3 29 E8 06 5F 83 C4 0A PR.6.).6.).._...
0800:4DB0 8B 46 EE 8B 56 EC 89 46 FE 89 56 FC EB 26 FF 36 .F..V..F..V..&.6
0800:4DC0 E1 29 FF 36 DF 29 FF 36 E5 29 FF 36 E3 29 E8 8C .).6.).6.).6.)..
0800:4DD0 F0 83 C4 04 52 50 E8 C3 F2 83 C4 08 83 6E FC 01 ....RP.......n..
0800:4DE0 83 5E FE 00 8B 46 FC 0B 46 FE 75 D2 B8 01 00 50 .^...F..F.u....P
0800:4DF0 33 C0 BA 04 00 50 52 FF 36 E5 29 FF 36 E3 29 E8 3....PR.6.).6.).
0800:4E00 B1 5E 83 C4 0A FF 36 E5 29 FF 36 E3 29 E8 4D F0 .^....6.).6.).M.
0800:4E10 83 C4 04 05 01 00 83 D2 00 89 56 FE 89 46 FC B8 ..........V..F..
0800:4E20 01 00 50 8B 56 FE 8B 46 FC B1 02 E8 3B 3E 52 50 ..P.V..F....;>RP
0800:4E30 FF 36 E5 29 FF 36 E3 29 E8 78 5E 83 C4 0A E9 9D .6.).6.).x^.....
0800:4E40 00 FF 36 E5 29 FF 36 E3 29 E8 11 F0 83 C4 04 89 ..6.).6.).......
0800:4E50 56 FA 89 46 F8 FF 36 E1 29 FF 36 DF 29 FF 76 FA V..F..6.).6.).v.
0800:4E60 50 E8 38 F2 83 C4 08 8B 46 FA 8B 56 F8 25 FF 3F P.8.....F..V.%.?
0800:4E70 89 56 E8 89 46 EA B9 04 00 BB 14 4F 2E 8B 07 3B .V..F......O...;
0800:4E80 46 F8 75 09 2E 8B 47 08 3B 46 EA 74 07 83 C3 02 F.u...G.;F.t....
0800:4E90 E2 EA EB 43 2E FF 67 10 E8 91 00 8B D0 0B D2 74 ...C..g........t
0800:4EA0 3D 8B E5 5D C3 FF 36 E5 29 FF 36 E3 29 E8 AD EF =..]..6.).6.)...
0800:4EB0 83 C4 04 89 56 FE 89 46 FC FF 36 E1 29 FF 36 DF ....V..F..6.).6.
0800:4EC0 29 FF 76 FE 50 E8 D4 F1 83 C4 08 83 06 8C 4E 04 ).v.P.........N.
0800:4ED0 83 16 8E 4E 00 EB 07 B8 09 00 8B E5 5D C3 FF 36 ...N........]..6
0800:4EE0 E5 29 FF 36 E3 29 E8 46 5E 83 C4 04 50 52 FF 36 .).6.).F^...PR.6
0800:4EF0 E5 29 FF 36 E3 29 E8 9B F2 83 C4 04 5B 3B DA 5A .).6.)......[;.Z
0800:4F00 73 03 E9 3C FF 75 07 3B D0 73 03 E9 33 FF 33 C0 s..<.u.;.s..3.3.
0800:4F10 8B E5 5D C3 E9 03 EA 03 EB 03 F2 03 00 00 00 00 ..].............
0800:4F20 00 00 00 00 98 4E 98 4E A5 4E DE 4E 55 8B EC 83 .....N.N.N.NU...
0800:4F30 EC 1A FF 36 E5 29 FF 36 E3 29 E8 20 EF 83 C4 04 ...6.).6.). ....
0800:4F40 B1 02 E8 24 3D 89 56 F0 89 46 EE FF 36 E5 29 FF ...$=.V..F..6.).
0800:4F50 36 E3 29 E8 D9 5D 83 C4 04 89 56 E8 89 46 E6 83 6.)..]....V..F..
0800:4F60 7E F0 00 72 30 77 06 83 7E EE 12 76 28 FF 36 E5 ~..r0w..~..v(.6.
0800:4F70 29 FF 36 E3 29 E8 E5 EE 83 C4 04 89 56 EC 89 46 ).6.).......V..F
0800:4F80 EA FF 36 E5 29 FF 36 E3 29 E8 D1 EE 83 C4 04 89 ..6.).6.).......
0800:4F90 56 F4 89 46 F2 FF 36 E5 29 FF 36 E3 29 E8 B8 EF V..F..6.).6.)...
0800:4FA0 83 C4 04 B1 08 E8 02 3D 83 FA 52 75 08 3D 43 4E .......=..Ru.=CN
0800:4FB0 75 03 E9 31 01 FF 36 E1 29 FF 36 DF 29 8B 56 F0 u..1..6.).6.).V.
0800:4FC0 8B 46 EE B1 02 E8 E2 3C 52 50 E8 CF F0 83 C4 08 .F.....<RP......
0800:4FD0 33 C0 50 FF 76 E8 FF 76 E6 FF 36 E5 29 FF 36 E3 3.P.v..v..6.).6.
0800:4FE0 29 E8 CF 5C 83 C4 0A FF 76 F0 FF 76 EE FF 36 E1 )..\....v..v..6.
0800:4FF0 29 FF 36 DF 29 FF 36 E5 29 FF 36 E3 29 E8 0A EB ).6.).6.).6.)...
0800:5000 83 C4 0C E9 B6 00 B8 01 00 50 33 C0 BA 04 00 50 .........P3....P
0800:5010 52 FF 36 E5 29 FF 36 E3 29 E8 97 5C 83 C4 0A FF R.6.).6.)..\....
0800:5020 36 E1 29 FF 36 DF 29 33 C0 BA EC 03 50 52 E8 6B 6.).6.)3....PR.k
0800:5030 F0 83 C4 08 FF 36                               .....6         

fn04C4_83F6()
	in	ax,29
	push	word ptr [29E3]
	call	721D

fn04C4_83FE()
	out	dx,al

fn04C4_83FF()
	add	sp,04
	mov	[bp-04],dx
	mov	[bp-06],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-04]
	push	ax
	call	745C
	add	sp,08
	mov	ax,[bp-06]
	or	ax,[bp-04]
	jz	8471

fn04C4_8422()
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	sub	ax,0001
	sbb	dx,00
	mov	[bp-08],dx
	mov	[bp-0A],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-08]
	push	ax
	call	745C
	add	sp,08
	mov	dx,[bp-04]
	mov	ax,[bp-06]
	mov	cl,02
	call	C029
	push	dx
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	6ECA
	add	sp,0C

fn04C4_8471()
	mov	ax,[bp-06]
	or	ax,[bp-04]
	jz	847C
	jmp	83F4
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	7318
	add	sp,04
	or	dx,dx
	jnz	8496
	cmp	ax,03EC
	jnz	8496
	jmp	83C6
	add	word ptr [4E8C],04
	adc	word ptr [4E8E],00
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret	
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	dx,[bp-14]
	mov	ax,[bp-16]
	mov	cl,02
	call	C06A
	push	dx
	push	ax
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E0EF
	add	sp,04
	mov	[bp-04],dx
	mov	[bp-06],ax
	xor	ax,ax
	push	ax
	push	word ptr [4E8E]
	push	word ptr [4E8C]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0C]
	push	word ptr [bp-0E]
	call	745C
	add	sp,08
	add	word ptr [4E8C],04
	adc	word ptr [4E8E],00
	xor	ax,ax
	push	ax
	push	word ptr [bp-04]
	push	word ptr [bp-06]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	call	8734
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
	call	E073
	add	sp,0A
	mov	ax,[bp-02]
	mov	sp,bp
	pop	bp
	ret	
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	918E
	add	sp,04
	or	ax,ax
	jnz	8565
	mov	ax,0007
	ret	
	call	8734
	ret	
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	jmp	86FD
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	725A
	add	sp,04
	or	ax,ax
	jz	8595
	cmp	ax,0001
	jz	85B5
	cmp	ax,0003
	jnz	8592
	jmp	8698
	jmp	86C1
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	718F
	add	sp,04
	push	ax
	call	7407
	add	sp,06
	jmp	86FD
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	push	dx
	push	ax
	call	745C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	word ptr [bp-02],0000
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	ax
	call	747F
	add	sp,06
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	7318
	add	sp,04
	mov	cl,08
	call	C06A
	cmp	dx,52
	jnz	867A
	cmp	ax,4E43
	jnz	867A
	call	8734
	mov	si,ax
	or	si,si
	jz	8623
	pop	si
	mov	sp,bp
	pop	bp
	ret	
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
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A07]
	call	747F
	add	sp,06
	mov	ax,0001
	push	ax
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	jmp	86FD
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	6ECA
	add	sp,0C
	jmp	86FD
	xor	si,si
	jmp	86BA
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	718F
	add	sp,04
	push	ax
	call	7407
	add	sp,06
	inc	si
	cmp	si,03
	jnz	869C
	jmp	86FD
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	7554
	add	sp,04
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
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
	call	6ECA
	add	sp,0C
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
	add	sp,04
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	7554
	add	sp,04
	pop	bx
	cmp	bx,dx
	pop	dx
	jnc	8724
	jmp	8573
	jnz	872D
	cmp	dx,ax
	jnc	872D
	jmp	8573
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,0E
	push	si
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	and	ax,0003
	mov	[2A21],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	mov	[2A09],dx
	mov	[2A07],ax
	cmp	word ptr [2A21],00
	jnz	87AF
	push	word ptr [2A09]
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	6ECA
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
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	mov	[2A05],dx
	mov	[2A03],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	71E7
	add	sp,04
	mov	[2E4B],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	71E7
	add	sp,04
	mov	[2E49],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	71E7
	add	sp,04
	push	word ptr [2A05]
	push	word ptr [2A03]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	60CA
	add	sp,08
	cmp	ax,[2E49]
	jz	8818
	mov	ax,0005
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	mov	ax,0001
	mov	dx,000F
	push	ax
	push	dx
	call	76D1
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
	call	76D1
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
	call	8FDA
	add	sp,02
	or	ax,ax
	jz	88C4
	cmp	word ptr [2A25],02
	jz	88C4
	cmp	word ptr [2A25],07
	jz	88C4
	mov	si,000A
	or	si,si
	jnz	891D
	mov	ax,0001
	push	ax
	call	8FDA
	add	sp,02
	or	ax,ax
	jz	8917
	cmp	word ptr [2A23],01
	jz	890B
	mov	ax,0010
	push	ax
	call	8FDA
	add	sp,02
	mov	dx,ax
	cmp	word ptr [2A25],02
	jnz	88FB
	cmp	word ptr [2E4F],00
	jnz	88FB
	mov	[2E4F],dx
	cmp	[2E4F],dx
	jz	890B
	cmp	word ptr [2E4F],00
	jz	890B
	mov	si,000C
	cmp	word ptr [2E4F],00
	jnz	891D
	mov	si,000B
	jmp	891D
	mov	word ptr [2E4F],0000
	or	si,si
	jnz	893C
	mov	ax,[2A21]
	cmp	ax,0001
	jz	8930
	cmp	ax,0002
	jz	8937
	jmp	893C
	call	89A8
	mov	si,ax
	jmp	893C
	call	8B5B
	mov	si,ax
	mov	ax,[bp-0E]
	mov	[2E4F],ax
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	7706
	add	sp,04
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	7706
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
	call	E073
	add	sp,0A
	or	si,si
	jz	8990
	mov	ax,si
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	mov	ax,[2E4D]
	cmp	ax,[2E4B]
	jz	89A1
	mov	ax,0006
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,04
	jmp	8B0B
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,290F
	push	ax
	call	8DE4
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,284F
	push	ax
	call	8DE4
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,278F
	push	ax
	call	8DE4
	add	sp,06
	mov	ax,0010
	push	ax
	call	8FF9
	add	sp,02
	mov	word ptr [bp-02],0000
	mov	[bp-04],ax
	jmp	8AF6
	push	ds
	mov	ax,290F
	push	ax
	call	8E4D
	add	sp,04
	mov	[2E47],ax
	add	[29FF],ax
	adc	word ptr [2A01],00
	cmp	word ptr [2E47],00
	jnz	8A14
	jmp	8AA6
	jmp	8A24
	call	8ED5
	xor	al,[2E4F]
	push	ax
	call	90EF
	add	sp,02
	mov	ax,[2E47]
	dec	word ptr [2E47]
	or	ax,ax
	jnz	8A16
	test	word ptr [2E4F],0001
	jz	8A44
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	8A48
	shr	word ptr [2E4F],01
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
	call	C029
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
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jz	8B0B
	push	ds
	mov	ax,284F
	push	ax
	call	8E4D
	add	sp,04
	inc	ax
	mov	[2E2B],ax
	push	ds
	mov	ax,278F
	push	ax
	call	8E4D
	add	sp,04
	add	ax,0002
	mov	[2E29],ax
	add	[29FF],ax
	adc	word ptr [2A01],00
	jmp	8AEB
	les	bx,[2E55]
	sub	bx,[2E2B]
	mov	al,es:[bx]
	push	ax
	call	90EF
	add	sp,02
	mov	ax,[2E29]
	dec	word ptr [2E29]
	or	ax,ax
	jnz	8AD9
	mov	ax,[bp-04]
	mov	dx,[bp-02]
	sub	word ptr [bp-04],01
	sbb	word ptr [bp-02],00
	or	ax,dx
	jz	8B0B
	jmp	89F3
	mov	ax,[2A01]
	mov	dx,[29FF]
	cmp	ax,[2A09]
	jnc	8B1B
	jmp	89B1
	jnz	8B26
	cmp	dx,[2A07]
	jnc	8B26
	jmp	89B1
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
	call	7512
	add	sp,0C
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret	
	jmp	8CB2
	call	8ED5
	xor	al,[2E4F]
	push	ax
	call	90EF
	add	sp,02
	test	word ptr [2E4F],0001
	jz	8B81
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	8B85
	shr	word ptr [2E4F],01
	add	word ptr [29FF],01
	adc	word ptr [2A01],00
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	or	ax,ax
	jz	8B5E
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	or	ax,ax
	jz	8C26
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	or	ax,ax
	jnz	8BCA
	mov	word ptr [2E29],0002
	call	8ED5
	mov	ah,00
	inc	ax
	mov	[2E2B],ax
	jmp	8BF8
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	or	ax,ax
	jnz	8BE0
	mov	word ptr [2E29],0003
	jmp	8BF5
	call	8ED5
	mov	ah,00
	add	ax,0008
	mov	[2E29],ax
	cmp	word ptr [2E29],08
	jnz	8BF5
	jmp	8CA8
	call	8D35
	mov	ax,[2E29]
	add	[29FF],ax
	adc	word ptr [2A01],00
	jmp	8C18
	les	bx,[2E55]
	sub	bx,[2E2B]
	mov	al,es:[bx]
	push	ax
	call	90EF
	add	sp,02
	mov	ax,[2E29]
	dec	word ptr [2E29]
	or	ax,ax
	jnz	8C06
	jmp	8B8F
	call	8CFF
	cmp	word ptr [2E29],09
	jnz	8C77
	call	8DCF
	mov	ax,[2E47]
	add	[29FF],ax
	adc	word ptr [2A01],00
	jmp	8C4F
	call	8ED5
	xor	al,[2E4F]
	push	ax
	call	90EF
	add	sp,02
	mov	ax,[2E47]
	dec	word ptr [2E47]
	or	ax,ax
	jnz	8C41
	test	word ptr [2E4F],0001
	jz	8C70
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	8B8F
	shr	word ptr [2E4F],01
	jmp	8B8F
	call	8D35
	mov	ax,[2E29]
	add	[29FF],ax
	adc	word ptr [2A01],00
	jmp	8C9A
	les	bx,[2E55]
	sub	bx,[2E2B]
	mov	al,es:[bx]
	push	ax
	call	90EF
	add	sp,02
	mov	ax,[2E29]
	dec	word ptr [2E29]
	or	ax,ax
	jnz	8C88
	jmp	8B8F
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	mov	ax,[2A01]
	mov	dx,[29FF]
	cmp	ax,[2A09]
	jnc	8CC2
	jmp	8B8F
	jnz	8CCD
	cmp	dx,[2A07]
	jnc	8CCD
	jmp	8B8F
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
	call	7512
	add	sp,0C
	xor	ax,ax
	ret	
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	add	ax,0004
	mov	[2E29],ax
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	or	ax,ax
	jz	8D34
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	mov	dx,[2E29]
	dec	dx
	shl	dx,01
	add	dx,ax
	mov	[2E29],dx
	ret	
	mov	word ptr [2E2B],0000
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	or	ax,ax
	jz	8DBA
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	mov	[2E2B],ax
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	or	ax,ax
	jz	8DA3
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	mov	dx,[2E2B]
	shl	dx,01
	add	dx,ax
	or	dx,04
	mov	[2E2B],dx
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	or	ax,ax
	jnz	8DBA
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	mov	dx,[2E2B]
	shl	dx,01
	add	dx,ax
	mov	[2E2B],dx
	jmp	8DBA
	cmp	word ptr [2E2B],00
	jnz	8DBA
	mov	ax,0001
	push	ax
	call	9099
	add	sp,02
	add	ax,0002
	mov	[2E2B],ax
	call	8ED5
	mov	ah,00
	mov	dx,[2E2B]
	mov	cl,08
	shl	dx,cl
	add	dx,ax
	inc	dx
	mov	[2E2B],dx
	ret	
	mov	ax,0004
	push	ax
	call	9099
	add	sp,02
	shl	ax,01
	shl	ax,01
	add	ax,000C
	mov	[2E47],ax
	ret	
	push	bp
	mov	bp,sp
	push	si
	push	di
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	7794
	add	sp,06
	mov	ax,0005
	push	ax
	call	8FF9
	add	sp,02
	mov	[bp+08],ax
	or	ax,ax
	jz	8E49
	cmp	word ptr [bp+08],10
	jbe	8E14
	mov	word ptr [bp+08],0010
	xor	di,di
	mov	si,[bp+04]
	add	si,0A
	cmp	di,[bp+08]
	jnc	8E3A
	mov	ax,0004
	push	ax
	call	8FF9
	add	sp,02
	mov	es,[bp+06]
	mov	es:[si],ax
	add	si,0C
	inc	di
	cmp	di,[bp+08]
	jc	8E21
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	7910
	add	sp,06
	pop	di
	pop	si
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	push	di
	mov	word ptr [bp-02],0000
	mov	di,[bp+04]
	jmp	8E65
	add	di,0C
	inc	word ptr [bp-02]
	mov	es,[bp+06]
	cmp	word ptr es:[di+0A],00
	jz	8E5F
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
	jnz	8E5F
	cmp	cx,es:[di+06]
	jnz	8E5F
	mov	ax,[bp-02]
	mov	dx,000C
	imul	dx
	mov	bx,[bp+04]
	add	bx,ax
	push	word ptr es:[bx+0A]
	call	8FF9
	add	sp,02
	cmp	word ptr [bp-02],02
	jnc	8EB8
	mov	ax,[bp-02]
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	mov	ax,[bp-02]
	dec	ax
	push	ax
	call	8FF9
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
	push	bp
	mov	bp,sp
	sub	sp,08
	mov	ax,[2E73]
	mov	dx,[2E71]
	add	dx,FD
	cmp	ax,[2E6F]
	jz	8EEE
	jmp	8FCB
	cmp	dx,[2E6D]
	jz	8EF7
	jmp	8FCB
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	7554
	add	sp,04
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
	add	sp,04
	pop	bx
	pop	cx
	sub	cx,ax
	sbb	bx,dx
	mov	[bp-02],bx
	mov	[bp-04],cx
	cmp	word ptr [bp-02],00
	jc	8F36
	ja	8F2F
	cmp	word ptr [bp-04],FD
	jbe	8F36
	xor	dx,dx
	mov	ax,FFFD
	jmp	8F3C
	mov	dx,[bp-02]
	mov	ax,[bp-04]
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
	call	74D0
	add	sp,0C
	mov	ax,[bp-06]
	mov	dx,[bp-08]
	sub	[bp-04],dx
	mov	dx,[bp-04]
	sbb	[bp-02],ax
	mov	ax,[bp-02]
	or	ax,ax
	jc	8F8B
	ja	8F81
	cmp	dx,02
	jbe	8F8B
	mov	word ptr [bp-02],0000
	mov	word ptr [bp-04],0002
	push	word ptr [29E5]
	push	word ptr [29E3]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	mov	ax,[2E71]
	add	ax,[bp-08]
	push	word ptr [2E73]
	push	ax
	call	74D0
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
	call	E073
	add	sp,0A
	les	bx,[2E6D]
	inc	word ptr [2E6D]
	mov	al,es:[bx]
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	mov	ax,[bp+04]
	cmp	word ptr [2A21],02
	jnz	8FF0
	push	ax
	call	9099
	add	sp,02
	pop	bp
	ret	
	push	ax
	call	8FF9
	add	sp,02
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,06
	push	si
	push	di
	mov	si,[bp+04]
	xor	di,di
	mov	word ptr [bp-06],0001
	jmp	908A
	cmp	word ptr [2E45],00
	jnz	9062
	call	8ED5
	mov	ah,00
	mov	[bp-02],ax
	call	8ED5
	mov	ah,00
	mov	[bp-04],ax
	les	bx,[2E6D]
	mov	al,es:[bx+01]
	mov	ah,00
	xor	dx,dx
	mov	cl,18
	call	C029
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
	mov	ax,[29FB]
	and	ax,0001
	or	ax,0000
	jz	9070
	or	di,[bp-06]
	mov	ax,[29FD]
	mov	dx,[29FB]
	shr	ax,01
	rcr	dx,01
	mov	[29FD],ax
	mov	[29FB],dx
	shl	word ptr [bp-06],01
	dec	word ptr [2E45]
	dec	si
	or	si,si
	jz	9091
	jmp	900D
	mov	ax,di
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	di,[bp+04]
	xor	si,si
	jmp	90E5
	cmp	word ptr [2E45],00
	jnz	90C0
	call	8ED5
	mov	ah,00
	mov	word ptr [29FD],0000
	mov	[29FB],ax
	mov	word ptr [2E45],0008
	shl	si,01
	mov	ax,[29FB]
	and	ax,0080
	or	ax,0000
	jz	90CE
	inc	si
	mov	ax,[29FD]
	mov	dx,[29FB]
	shl	dx,01
	rcl	ax,01
	mov	[29FD],ax
	mov	[29FB],dx
	dec	word ptr [2E45]
	dec	di
	or	di,di
	jnz	90A5
	mov	ax,si
	pop	di
	pop	si
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	mov	ax,[2E5B]
	mov	dx,[2E59]
	dec	dx
	cmp	ax,[2E57]
	jnz	915E
	cmp	dx,[2E55]
	jnz	915E
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
	call	7512
	add	sp,0C
	push	word ptr [2E31]
	mov	ax,[2E55]
	sub	ax,[2E31]
	push	word ptr [2E57]
	push	ax
	push	word ptr [2E5B]
	push	word ptr [2E59]
	call	E4B3
	add	sp,0A
	mov	ax,[2E5B]
	mov	dx,[2E59]
	add	dx,[2E31]
	mov	[2E57],ax
	mov	[2E55],dx
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
	push	bp
	mov	bp,sp
	push	si
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	7554
	add	sp,04
	or	dx,dx
	jc	91AE
	jnz	91A9
	cmp	ax,0400
	jc	91AE
	mov	ax,0400
	jmp	91BD
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	7554
	add	sp,04
	and	ax,FFFC
	mov	si,ax
	jmp	921B
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	71E7
	add	sp,04
	cmp	ax,524E
	jnz	9218
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	71E7
	add	sp,04
	and	ax,FF00
	cmp	ax,4300
	jnz	9200
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFFC
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	E073
	add	sp,0A
	jmp	921F
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFFE
	push	ax
	push	dx
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	E073
	add	sp,0A
	sub	si,02
	or	si,si
	jnz	91C1
	mov	ax,si
	pop	si
	pop	bp
	ret	
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
	jnc	9240
	mov	cx,dx
	xor	ax,ax
	rep	
	cmpsb	
	jz	924B
	sbb	ax,ax
	sbb	ax,FFFF
	or	ax,ax
	jz	9283
	push	ds
	pop	es
	mov	di,4271
	mov	si,2202
	mov	dx,0003
	mov	cx,0004
	sub	dx,cx
	jnc	9265
	add	cx,dx
	xor	dx,dx
	shr	cx,01
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
	mov	cx,dx
	xor	ax,ax
	rep	
	stosb	
	push	ds
	mov	ax,2E75
	push	ax
	push	ds
	mov	ax,4271
	push	ax
	call	3D63
	add	sp,08
	mov	al,[2E75]
	mov	ah,00
	mov	cl,08
	shl	ax,cl
	mov	dl,[2E76]
	mov	dh,00
	add	ax,dx
	xor	dx,dx
	cmp	dx,[2A09]
	jc	92AD
	jnz	92A4
	cmp	ax,[2A07]
	jc	92AD
	mov	ax,0003
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	cmp	word ptr [2A0D],00
	jc	92CA
	ja	92BE
	cmp	word ptr [2A0B],7FF0
	jbe	92CA
	mov	word ptr [2A0D],0000
	mov	word ptr [2A0B],7FF0
	xor	ax,ax
	push	ax
	mov	dx,0002
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-22],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	di,ax
	cmp	word ptr [bp-22],00
	jz	9308
	dec	di
	mov	ax,di
	xor	dx,dx
	mov	cl,09
	call	C029
	add	ax,[bp-22]
	adc	dx,00
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	cmp	ax,[2A09]
	jc	933E
	ja	9331
	cmp	dx,[2A07]
	jbe	933E
	mov	ax,[2A09]
	mov	dx,[2A07]
	mov	[bp-02],ax
	mov	[bp-04],dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-24],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-26],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-28],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-2A],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-2C],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-2E],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-30],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-32],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-34],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
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
	call	7512
	add	sp,0C
	cmp	word ptr [bp-24],00
	jnz	9424
	jmp	95E0
	xor	ax,ax
	mov	dx,FFFF
	push	ax
	push	dx
	call	76D1
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp-34]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	xor	si,si
	mov	ax,[bp-0C]
	mov	[bp-36],ax
	jmp	9496
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	[bp-22],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	di,ax
	xor	dx,dx
	mov	cl,04
	call	C029
	add	ax,[bp-22]
	adc	dx,00
	mov	es,[bp-0A]
	mov	bx,[bp-36]
	mov	es:[bx+02],dx
	mov	es:[bx],ax
	add	word ptr [bp-36],04
	inc	si
	cmp	si,[bp-24]
	jnz	9456
	mov	ax,667B
	push	ax
	mov	ax,0004
	push	ax
	push	word ptr [bp-24]
	push	word ptr [bp-0A]
	push	word ptr [bp-0C]
	call	ED1E
	add	sp,0A
	xor	si,si
	mov	di,[bp-0C]
	xor	ax,ax
	adc	ax,0000
	neg	ax
	mov	[bp-38],ax
	jmp	95CC
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
	call	7407
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	dx,[bp-0E]
	mov	ax,[bp-10]
	mov	cl,04
	call	C06A
	push	ax
	call	747F
	add	sp,06
	mov	word ptr [bp-22],0000
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
	ja	9579
	jnz	953B
	cmp	word ptr [bp-18],00FF
	ja	9579
	mov	ax,[bp-0E]
	mov	dx,[bp-10]
	sub	dx,[bp-14]
	sbb	ax,[bp-12]
	or	ax,ax
	ja	9579
	jc	9552
	cmp	dx,F0
	jnc	9579
	cmp	word ptr [bp-22],00FF
	jz	9579
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[bp-18]
	push	ax
	call	7407
	add	sp,06
	inc	word ptr [bp-22]
	add	di,04
	inc	si
	mov	ax,si
	cmp	ax,[bp-24]
	jnz	9510
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
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[bp-22]
	push	ax
	call	7407
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
	call	E073
	add	sp,0A
	cmp	si,[bp-24]
	jz	95D4
	jmp	94C4
	push	word ptr [bp-0A]
	push	word ptr [bp-0C]
	call	7706
	add	sp,04
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,00
	push	ax
	call	7407
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E0EF
	add	sp,04
	add	ax,FFE0
	adc	dx,FF
	mov	[bp-1A],dx
	mov	[bp-1C],ax
	jmp	9626
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,00
	push	ax
	call	7407
	add	sp,06
	add	word ptr [bp-1C],01
	adc	word ptr [bp-1A],00
	mov	ax,[bp-1C]
	and	ax,000F
	or	ax,0000
	jnz	960D
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
	call	E073
	add	sp,0A
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	A9AA
	add	sp,08
	xor	ax,ax
	push	ax
	mov	dx,0020
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-30]
	call	747F
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-32]
	call	747F
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-2E]
	call	747F
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-2C]
	call	747F
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-1C]
	call	747F
	add	sp,06
	mov	dx,[2A05]
	mov	ax,[2A03]
	mov	cl,04
	call	C06A
	mov	[bp-22],ax
	mov	ax,[2A03]
	and	ax,000F
	or	ax,0000
	jz	96F3
	inc	word ptr [bp-22]
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-22]
	call	747F
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	dx,[bp-1A]
	mov	ax,[bp-1C]
	mov	cl,04
	call	C06A
	mov	dx,[bp-22]
	add	dx,ax
	push	dx
	call	747F
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
	jz	9758
	add	word ptr [bp-08],0200
	adc	word ptr [bp-06],00
	jmp	9768
	cmp	word ptr [2A21],01
	jnz	9768
	add	word ptr [bp-08],0180
	adc	word ptr [bp-06],00
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
	call	E073
	add	sp,0A
	mov	ax,0002
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E0EF
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
	call	6ECA
	add	sp,0C
	mov	ax,[bp-20]
	and	ax,01FF
	mov	[bp-22],ax
	mov	dx,[bp-1E]
	mov	ax,[bp-20]
	mov	cl,09
	call	C06A
	mov	di,ax
	cmp	word ptr [bp-22],00
	jz	9806
	inc	di
	xor	ax,ax
	push	ax
	mov	dx,0002
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-22]
	call	747F
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	di
	call	747F
	add	sp,06
	cmp	word ptr [2A1D],00
	jz	9879
	xor	ax,ax
	push	ax
	mov	dx,002E
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-22]
	call	747F
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	di
	call	747F
	add	sp,06
	xor	ax,ax
	push	ax
	mov	dx,0018
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
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
	call	747F
	add	sp,06
	xor	ax,ax
	push	ax
	mov	dx,001C
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-28]
	call	747F
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-2A]
	call	747F
	add	sp,06
	xor	ax,ax
	push	ax
	mov	dx,000A
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
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
	jc	995C
	ja	992E
	cmp	bx,cx
	jbe	995C
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
	call	C06A
	mov	[bp-28],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	ax
	call	747F
	add	sp,06
	mov	ax,[bp-2A]
	cmp	ax,[bp-28]
	jnc	999C
	mov	ax,[bp-28]
	mov	[bp-2A],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-2A]
	call	747F
	add	sp,06
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	add	ax,[4E88]
	adc	dx,[4E8A]
	add	ax,0020
	adc	dx,00
	mov	cl,04
	call	C06A
	mov	[bp-2C],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	ax
	call	747F
	add	sp,06
	mov	ax,[bp-1C]
	add	ax,0080
	mov	[bp-2E],ax
	cmp	word ptr [2A1D],00
	jz	99EF
	add	word ptr [bp-2E],0200
	jmp	99FB
	cmp	word ptr [2A21],01
	jnz	99FB
	add	word ptr [bp-2E],0180
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-2E]
	call	747F
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
	jz	9A33
	mov	ax,0001
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	mov	ax,[bp+04]
	cmp	ax,[bp+08]
	jbe	9A4B
	mov	ax,0001
	pop	bp
	ret	
	mov	ax,[bp+04]
	cmp	ax,[bp+08]
	jnc	9A58
	mov	ax,FFFF
	pop	bp
	ret	
	xor	ax,ax
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	push	di
	cmp	word ptr [2A09],00
	jc	9A7E
	ja	9A75
	cmp	word ptr [2A07],FEFE
	jbe	9A7E
	mov	ax,0003
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	ds
	pop	es
	mov	di,2206
	mov	si,4271
	mov	cx,0004
	mov	dx,0003
	cmp	dx,cx
	jnc	9A92
	mov	cx,dx
	xor	ax,ax
	rep	
	cmpsb	
	jz	9A9D
	sbb	ax,ax
	sbb	ax,FFFF
	or	ax,ax
	jz	9AD5
	push	ds
	pop	es
	mov	di,4271
	mov	si,2206
	mov	dx,0003
	mov	cx,0004
	sub	dx,cx
	jnc	9AB7
	add	cx,dx
	xor	dx,dx
	shr	cx,01
	rep	
	movsw	
	adc	cx,cx
	rep	
	movsb	
	mov	cx,dx
	xor	ax,ax
	rep	
	stosb	
	push	ds
	mov	ax,2E75
	push	ax
	push	ds
	mov	ax,4271
	push	ax
	call	3D63
	add	sp,08
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
	call	7512
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
	jz	9B39
	add	word ptr [bp-04],0200
	adc	word ptr [bp-02],00
	jmp	9B49
	cmp	word ptr [2A21],01
	jnz	9B49
	add	word ptr [bp-04],0180
	adc	word ptr [bp-02],00
	cmp	word ptr [bp-02],00
	jc	9B61
	ja	9B58
	cmp	word ptr [bp-04],FEFE
	jbe	9B61
	mov	ax,0003
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	A9AA
	add	sp,08
	xor	ax,ax
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
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
	jc	9BB0
	jnz	9BA9
	cmp	ax,[2A07]
	jc	9BB0
	mov	ax,0003
	mov	sp,bp
	pop	bp
	ret	
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	word ptr [bp-0E]
	push	ds
	mov	ax,2E77
	push	ax
	call	7512
	add	sp,0C
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFEE
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	72CA
	add	sp,04
	cmp	ax,601A
	jz	9BF9
	jmp	9CB1
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,0002
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	xor	ax,ax
	mov	dx,0010
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	6ECA
	add	sp,0C
	xor	ax,ax
	push	ax
	mov	dx,001A
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	71E7
	add	sp,04
	push	ax
	call	73DE
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
	call	A9AA
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
	jmp	9D19
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	745C
	add	sp,08
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,000E
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	A9AA
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
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E0EF
	add	sp,04
	and	ax,0001
	or	ax,0000
	jz	9D48
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,90
	push	ax
	call	7407
	add	sp,06
	add	word ptr [bp-08],01
	adc	word ptr [bp-06],00
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	ax
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E0EF
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
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	7318
	add	sp,04
	sub	[bp-08],ax
	sbb	[bp-06],dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A05]
	push	word ptr [2A03]
	call	745C
	add	sp,08
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	71E7
	add	sp,04
	cmp	ax,601A
	jz	9DDE
	jmp	9E64
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	pop	bx
	pop	cx
	add	cx,ax
	adc	bx,dx
	mov	[bp-02],bx
	mov	[bp-04],cx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
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
	jc	9E64
	ja	9E40
	cmp	dx,cx
	jbe	9E64
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
	xor	ax,ax
	push	ax
	mov	dx,000A
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	745C
	add	sp,08
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret	
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
	jc	9EC4
	jnz	9EBC
	cmp	ax,[2A07]
	jc	9EC4
	mov	ax,0003
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	or	dx,dx
	jnz	9EDB
	cmp	ax,03F3
	jz	9EE3
	mov	ax,0003
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	mov	dx,03F3
	push	ax
	push	dx
	call	745C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	mov	[bp-06],dx
	mov	[bp-08],ax
	mov	[bp-02],dx
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-02]
	push	ax
	call	745C
	add	sp,08
	jmp	9F44
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	push	dx
	push	ax
	call	745C
	add	sp,08
	mov	ax,[bp-08]
	mov	dx,[bp-06]
	sub	word ptr [bp-08],01
	sbb	word ptr [bp-06],00
	or	ax,dx
	jnz	9F26
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jnz	9EF8
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	add	ax,0001
	adc	dx,00
	push	dx
	push	ax
	call	745C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
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
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,[bp-0E]
	mov	dx,[bp-10]
	add	dx,01
	adc	ax,0000
	push	ax
	push	dx
	call	745C
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
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E0EF
	add	sp,04
	mov	[4E8E],dx
	mov	[4E8C],ax
	mov	ax,[bp-12]
	mov	dx,[bp-14]
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	A06F
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	push	dx
	push	ax
	call	745C
	add	sp,08
	sub	word ptr [bp-04],01
	sbb	word ptr [bp-02],00
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jnz	A049
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
	call	7512
	add	sp,0C
	xor	si,si
	jmp	A253
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
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
	mov	ax,cs:[bx]
	cmp	ax,[bp-08]
	jnz	A0DE
	mov	ax,cs:[bx+0E]
	cmp	ax,[bp-16]
	jz	A0E6
	add	bx,02
	loop	A0CD
	jmp	A24B
	jmp	word ptr cs:[bx+1C]
	push	ds
	mov	ax,220A
	push	ax
	push	si
	call	A2A6
	add	sp,06
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	A2E0
	add	sp,04
	push	si
	call	A2BF
	add	sp,02
	inc	si
	jmp	A253
	push	ds
	mov	ax,220F
	push	ax
	push	si
	call	A2A6
	add	sp,06
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	call	A2E0
	add	sp,04
	push	si
	call	A2BF
	add	sp,02
	inc	si
	jmp	A253
	push	ds
	mov	ax,2214
	push	ax
	push	si
	call	A2A6
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	mov	dx,03EB
	push	ax
	push	dx
	call	745C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-02]
	push	ax
	call	745C
	add	sp,08
	add	word ptr [4E8C],04
	adc	word ptr [4E8E],00
	push	si
	call	A2BF
	add	sp,02
	jmp	A253
	push	ds
	mov	ax,2219
	push	ax
	push	si
	call	A2A6
	add	sp,06
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	mov	cl,02
	call	C029
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,[bp-02]
	jz	A1D5
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
	call	E073
	add	sp,0A
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jnz	A197
	push	si
	call	A2BF
	add	sp,02
	jmp	A253
	push	ds
	mov	ax,221E
	push	ax
	push	si
	call	A2A6
	add	sp,06
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	mov	cl,02
	call	C029
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	ax,0001
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	push	si
	call	A2BF
	add	sp,02
	jmp	A253
	mov	ax,0008
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	mov	dx,03F2
	push	ax
	push	dx
	call	745C
	add	sp,08
	jmp	A253
	mov	ax,0009
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
	add	sp,04
	cmp	dx,[2A09]
	jnc	A26A
	jmp	A0A4
	jnz	A275
	cmp	ax,[2A07]
	jnc	A275
	jmp	A0A4
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	jmp	8C82
	add	bp,bx
	add	si,ax
	add	si,cx
	add	si,dx
	add	si,bp
	add	ax,[bx+si]
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bx+si],al
	add	[bp+si],ch
	insw	
	dec	bp
	insw	
	jo	A30B
	retf	
	insw	
	outsb	
	jz	A312
	insb	
	outsb	
	push	bp
	mov	bp,sp
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	push	ds
	mov	ax,2223
	push	ax
	call	E6AF
	add	sp,0A
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	push	ds
	mov	ax,2231
	push	ax
	call	E6AF
	add	sp,04
	cmp	word ptr [bp+04],09
	jbe	A2DE
	push	ds
	mov	ax,223E
	push	ax
	call	E6AF
	add	sp,04
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,1A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	mov	cl,02
	call	C029
	mov	[bp-0E],dx
	mov	[bp-10],ax
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
	add	sp,04
	mov	[bp-12],dx
	mov	[bp-14],ax
	mov	ax,0001
	push	ax
	push	word ptr [bp-0E]
	push	word ptr [bp-10]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	jmp	A38C
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,0004
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	mov	cl,02
	call	C029
	mov	[bp-02],dx
	mov	[bp-04],ax
	or	ax,[bp-02]
	jz	A384
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
	call	E073
	add	sp,0A
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jnz	A346
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	7318
	add	sp,04
	or	dx,dx
	jnz	A3A3
	cmp	ax,03EC
	jz	A32D
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
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
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E0EF
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	cmp	word ptr [bp-16],00
	jnc	A3F5
	jmp	A4B9
	ja	A400
	cmp	word ptr [bp-18],12
	ja	A400
	jmp	A4B9
	xor	ax,ax
	push	ax
	push	word ptr [4E8E]
	push	word ptr [4E8C]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	721D
	add	sp,04
	mov	[bp-0A],dx
	mov	[bp-0C],ax
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	ax
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0E]
	push	word ptr [bp-10]
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-0A]
	push	word ptr [bp-0C]
	call	745C
	add	sp,08
	xor	ax,ax
	push	ax
	push	word ptr [bp-12]
	push	word ptr [bp-14]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	mov	ax,[bp-16]
	mov	dx,[bp-18]
	sub	dx,08
	sbb	ax,0000
	push	ax
	push	dx
	push	word ptr [bp-16]
	push	word ptr [bp-18]
	call	A9AA
	add	sp,08
	add	word ptr [2A03],08
	adc	word ptr [2A05],00
	mov	ax,[2A05]
	mov	dx,[2A03]
	cmp	ax,[bp-16]
	jnc	A4C8
	jmp	A60D
	jnz	A4D2
	cmp	dx,[bp-18]
	jnc	A4D2
	jmp	A60D
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
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
	call	E073
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
	call	6ECA
	add	sp,0C
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	7318
	add	sp,04
	or	dx,dx
	jz	A541
	jmp	A5FF
	cmp	ax,03EC
	jz	A549
	jmp	A5FF
	mov	ax,0001
	push	ax
	xor	ax,ax
	mov	dx,0004
	push	ax
	push	dx
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	mov	dx,03EC
	push	ax
	push	dx
	call	745C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-02]
	push	ax
	call	745C
	add	sp,08
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jz	A5F4
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	add	ax,0001
	adc	dx,00
	mov	[bp-06],dx
	mov	[bp-08],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-06]
	push	ax
	call	745C
	add	sp,08
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	cl,02
	call	C029
	push	dx
	push	ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	6ECA
	add	sp,0C
	mov	ax,[bp-04]
	or	ax,[bp-02]
	jz	A5FF
	jmp	A577
	add	word ptr [4E8C],04
	adc	word ptr [4E8E],00
	mov	sp,bp
	pop	bp
	ret	
	mov	ax,[bp-16]
	mov	dx,[bp-18]
	add	dx,[4E88]
	adc	ax,[4E8A]
	mov	[bp-02],ax
	mov	[bp-04],dx
	mov	ax,[2A03]
	and	ax,0003
	or	ax,0000
	jz	A634
	add	word ptr [bp-04],02
	adc	word ptr [bp-02],00
	mov	ax,[bp-04]
	and	ax,0003
	or	ax,0000
	jz	A658
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	cl,02
	call	C06A
	add	ax,0001
	adc	dx,00
	mov	[bp-02],dx
	mov	[bp-04],ax
	jmp	A686
	mov	dx,[bp-02]
	mov	ax,[bp-04]
	mov	cl,02
	call	C06A
	mov	[bp-02],dx
	mov	[bp-04],ax
	jmp	A686
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,00
	push	ax
	call	7407
	add	sp,06
	add	word ptr [2A03],01
	adc	word ptr [2A05],00
	mov	ax,[2A03]
	and	ax,0003
	or	ax,0000
	jnz	A66B
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E0EF
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
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	dx,[2A05]
	mov	ax,[2A03]
	mov	cl,02
	call	C06A
	push	dx
	push	ax
	call	745C
	add	sp,08
	mov	ax,[bp-0A]
	mov	dx,[bp-0C]
	and	ax,3FFF
	mov	[bp-1A],ax
	cmp	ax,[bp-02]
	jc	A70D
	ja	A701
	cmp	dx,[bp-04]
	jbe	A70D
	mov	ax,[bp-0C]
	mov	dx,[bp-1A]
	mov	[bp-02],dx
	mov	[bp-04],ax
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
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	745C
	add	sp,08
	xor	ax,ax
	push	ax
	push	word ptr [bp-06]
	push	word ptr [bp-08]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	add	word ptr [4E8C],04
	adc	word ptr [4E8E],00
	mov	sp,bp
	pop	bp
	ret	
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
	call	7512
	add	sp,0C
	mov	ax,0001
	push	ax
	mov	ax,FFFF
	mov	dx,FFFC
	push	ax
	push	dx
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A11]
	push	word ptr [2A0F]
	call	745C
	add	sp,08
	push	word ptr [2A09]
	push	word ptr [2A07]
	push	word ptr [2A09]
	push	word ptr [2A07]
	call	A9AA
	add	sp,08
	xor	ax,ax
	ret	
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	jmp	A981
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	725A
	add	sp,04
	or	ax,ax
	jz	A809
	cmp	ax,0001
	jz	A829
	cmp	ax,0003
	jnz	A806
	jmp	A926
	jmp	A94F
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	718F
	add	sp,04
	push	ax
	call	7407
	add	sp,06
	jmp	A981
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	721D
	add	sp,04
	push	dx
	push	ax
	call	745C
	add	sp,08
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	736D
	add	sp,04
	mov	word ptr [bp-02],0000
	mov	[bp-04],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	call	73DE
	add	sp,06
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	call	A9AA
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
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2A03]
	call	747F
	add	sp,06
	mov	ax,[2A05]
	mov	dx,[2A03]
	cmp	ax,[bp-02]
	jnz	A90A
	cmp	dx,[bp-04]
	jnz	A90A
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
	call	E073
	add	sp,0A
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	6ECA
	add	sp,0C
	jmp	A981
	mov	ax,0001
	push	ax
	push	word ptr [2A05]
	push	word ptr [2A03]
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E073
	add	sp,0A
	jmp	A981
	xor	si,si
	jmp	A948
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	718F
	add	sp,04
	push	ax
	call	7407
	add	sp,06
	inc	si
	cmp	si,03
	jnz	A92A
	jmp	A981
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
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
	call	6ECA
	add	sp,0C
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
	add	sp,04
	cmp	dx,[2A09]
	jnc	A998
	jmp	A7E7
	jnz	A9A3
	cmp	ax,[2A07]
	jnc	A9A3
	jmp	A7E7
	xor	ax,ax
	pop	si
	mov	sp,bp
	pop	bp
	ret	
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
	ja	A9F4
	jz	A9EB
	jmp	ADBD
	cmp	word ptr [bp+04],12
	ja	A9F4
	jmp	ADBD
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
	call	76D1
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
	call	76D1
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
	call	76D1
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
	call	76D1
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
	call	76D1
	add	sp,04
	mov	[bp-04],dx
	mov	[bp-06],ax
	inc	dx
	mov	[2E3D],dx
	mov	word ptr [2E3B],0000
	call	BBB8
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
	add	sp,04
	mov	[4664],dx
	mov	[4662],ax
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E0EF
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
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [4680]
	push	word ptr [467E]
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	push	ax
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	call	73DE
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	call	73DE
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	xor	ax,ax
	push	ax
	call	73DE
	add	sp,06
	push	ds
	mov	ax,2240
	push	ax
	call	E6AF
	add	sp,04
	mov	ax,[2E4F]
	mov	[bp-02],ax
	mov	ax,0001
	push	ax
	cmp	word ptr [2A1F],00
	jz	ABC8
	jmp	ABCA
	xor	ax,ax
	push	ax
	call	B825
	add	sp,04
	mov	ax,0001
	push	ax
	cmp	word ptr [2E4F],00
	jz	ABDE
	jmp	ABE0
	xor	ax,ax
	push	ax
	call	B825
	add	sp,04
	cmp	word ptr [2A23],01
	jz	AC03
	cmp	word ptr [2E4F],00
	jz	AC03
	mov	ax,0010
	push	ax
	push	word ptr [2E4F]
	call	B825
	add	sp,04
	mov	ax,[2A21]
	cmp	ax,0001
	jz	AC12
	cmp	ax,0002
	jz	AC17
	jmp	AC1A
	call	ADC2
	jmp	AC1A
	call	B038
	xor	si,si
	jmp	AC30
	mov	bx,si
	inc	si
	mov	al,[bx+4682]
	push	ax
	call	B9E4
	add	sp,02
	dec	word ptr [4E82]
	cmp	word ptr [4E82],00
	jnz	AC1E
	mov	ax,[bp-02]
	mov	[2E4F],ax
	mov	ax,[4680]
	mov	dx,[467E]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	cmp	ax,[4E8A]
	ja	AC73
	jc	AC5A
	cmp	dx,[4E88]
	jnc	AC73
	mov	ax,[4680]
	mov	dx,[467E]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	sub	[4E88],dx
	sbb	[4E8A],ax
	jmp	AC7F
	mov	word ptr [4E8A],0000
	mov	word ptr [4E88],0000
	cmp	word ptr [2A21],02
	jnz	AC90
	add	word ptr [4E88],02
	adc	word ptr [4E8A],00
	push	word ptr [29E1]
	push	word ptr [29DF]
	call	E0EF
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
	call	E073
	add	sp,0A
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	ax,[2A05]
	mov	dx,[2A03]
	sub	dx,12
	sbb	ax,0000
	push	ax
	push	dx
	call	745C
	add	sp,08
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2E4B]
	call	73DE
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	push	word ptr [2E49]
	call	73DE
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[4E88]
	push	ax
	call	7407
	add	sp,06
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[29F7]
	push	ax
	call	7407
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
	call	E073
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
	call	E073
	add	sp,0A
	push	word ptr [bp-14]
	push	word ptr [bp-16]
	call	7706
	add	sp,04
	push	word ptr [bp-10]
	push	word ptr [bp-12]
	call	7706
	add	sp,04
	push	word ptr [bp-0C]
	push	word ptr [bp-0E]
	call	7706
	add	sp,04
	push	word ptr [bp-08]
	push	word ptr [bp-0A]
	call	7706
	add	sp,04
	push	word ptr [bp-04]
	push	word ptr [bp-06]
	call	7706
	add	sp,04
	push	ds
	mov	ax,223C
	push	ax
	call	E6AF
	add	sp,04
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	mov	ax,[4664]
	mov	dx,[4662]
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	AFDA
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,284F
	push	ax
	call	7794
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,278F
	push	ax
	call	7794
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,290F
	push	ax
	call	7794
	add	sp,06
	call	B39C
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,290F
	push	ax
	call	77DC
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,284F
	push	ax
	call	77DC
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,278F
	push	ax
	call	77DC
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,290F
	push	ax
	call	B761
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,284F
	push	ax
	call	B761
	add	sp,06
	mov	ax,0010
	push	ax
	push	ds
	mov	ax,278F
	push	ax
	call	B761
	add	sp,06
	mov	ax,0010
	push	ax
	push	word ptr [466A]
	call	B849
	add	sp,04
	jmp	AF80
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	71E7
	add	sp,04
	mov	[2E47],ax
	add	[467A],ax
	adc	word ptr [467C],00
	push	ds
	mov	ax,290F
	push	ax
	push	word ptr [2E47]
	call	B7C7
	add	sp,06
	cmp	word ptr [2E47],00
	jz	AF16
	cmp	word ptr [2E45],00
	jz	AEF2
	jmp	AED7
	call	B719
	xor	al,[2E4F]
	mov	bx,[4E82]
	mov	[bx+4682],al
	inc	word ptr [4E82]
	mov	ax,[2E47]
	dec	word ptr [2E47]
	or	ax,ax
	jnz	AEC4
	jmp	AEFD
	call	B719
	xor	al,[2E4F]
	push	ax
	call	B9E4
	add	sp,02
	mov	ax,[2E47]
	dec	word ptr [2E47]
	or	ax,ax
	jnz	AEE4
	test	word ptr [2E4F],0001
	jz	AF12
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	AF16
	shr	word ptr [2E4F],01
	mov	ax,[466A]
	or	ax,[466C]
	jz	AF80
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	71E7
	add	sp,04
	mov	[2E29],ax
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	71E7
	add	sp,04
	mov	[2E2B],ax
	push	ds
	mov	ax,284F
	push	ax
	push	word ptr [2E2B]
	call	B7C7
	add	sp,06
	push	ds
	mov	ax,278F
	push	ax
	push	word ptr [2E29]
	call	B7C7
	add	sp,06
	add	word ptr [2E29],02
	mov	ax,[2E29]
	add	[467A],ax
	adc	word ptr [467C],00
	jmp	AF75
	call	B719
	mov	ax,[2E29]
	dec	word ptr [2E29]
	or	ax,ax
	jnz	AF72
	mov	ax,[466A]
	mov	dx,[466C]
	sub	word ptr [466A],01
	sbb	word ptr [466C],00
	or	ax,dx
	jz	AF98
	jmp	AE8B
	cmp	word ptr [2E45],00
	jnz	AFBC
	xor	si,si
	jmp	AFB5
	mov	bx,si
	inc	si
	mov	al,[bx+4682]
	push	ax
	call	B9E4
	add	sp,02
	dec	word ptr [4E82]
	cmp	word ptr [4E82],00
	jnz	AFA3
	add	word ptr [29F7],01
	adc	word ptr [29F9],00
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	ax,[467C]
	mov	dx,[467A]
	cmp	ax,[4680]
	jnc	AFEA
	jmp	ADD9
	jnz	AFF5
	cmp	dx,[467E]
	jnc	AFF5
	jmp	ADD9
	mov	cl,10
	sub	cl,[2E45]
	shr	word ptr [2E43],cl
	cmp	word ptr [2E45],00
	jnz	B00D
	cmp	word ptr [4E82],00
	jz	B017
	mov	al,[2E43]
	push	ax
	call	B9E4
	add	sp,02
	cmp	word ptr [2E45],08
	ja	B025
	cmp	word ptr [4E82],00
	jz	B033
	mov	ax,[2E43]
	mov	cl,08
	shr	ax,cl
	push	ax
	call	B9E4
	add	sp,02
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,04
	push	si
	mov	ax,[4664]
	mov	dx,[4662]
	mov	[bp-02],ax
	mov	[bp-04],dx
	jmp	B22D
	call	B39C
	xor	ax,ax
	push	ax
	push	word ptr [bp-02]
	push	word ptr [bp-04]
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E073
	add	sp,0A
	jmp	B18D
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	71E7
	add	sp,04
	mov	[2E47],ax
	add	[467A],ax
	adc	word ptr [467C],00
	push	ax
	call	B26F
	add	sp,02
	mov	ax,[466A]
	or	ax,[466C]
	jnz	B099
	jmp	B18D
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	71E7
	add	sp,04
	mov	[2E29],ax
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	71E7
	add	sp,04
	mov	[2E2B],ax
	cmp	word ptr [2E29],00
	jnz	B0DD
	mov	ax,0003
	push	ax
	mov	ax,0006
	push	ax
	call	B90B
	add	sp,04
	mov	al,[2E2B]
	push	ax
	call	B9C0
	add	sp,02
	jmp	B16C
	cmp	word ptr [2E29],07
	jnc	B128
	mov	bx,[2E29]
	mov	al,[bx+21DB]
	mov	ah,00
	push	ax
	mov	al,[bx+21D4]
	mov	ah,00
	push	ax
	call	B90B
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
	call	B90B
	add	sp,04
	mov	al,[2E2B]
	and	al,FF
	push	ax
	call	B9C0
	add	sp,02
	jmp	B16C
	mov	ax,0004
	push	ax
	mov	ax,000F
	push	ax
	call	B90B
	add	sp,04
	mov	al,[2E29]
	sub	al,06
	push	ax
	call	B9C0
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
	call	B90B
	add	sp,04
	mov	al,[2E2B]
	and	al,FF
	push	ax
	call	B9C0
	add	sp,02
	add	word ptr [2E29],02
	mov	ax,[2E29]
	add	[467A],ax
	adc	word ptr [467C],00
	jmp	B182
	call	B719
	mov	ax,[2E29]
	dec	word ptr [2E29]
	or	ax,ax
	jnz	B17F
	mov	ax,[466A]
	mov	dx,[466C]
	sub	word ptr [466A],01
	sbb	word ptr [466C],00
	or	ax,dx
	jz	B1A5
	jmp	B06C
	mov	ax,0004
	push	ax
	mov	ax,000F
	push	ax
	call	B90B
	add	sp,04
	mov	al,00
	push	ax
	call	B9C0
	add	sp,02
	mov	ax,[467C]
	mov	dx,[467A]
	cmp	ax,[4680]
	ja	B1DE
	jc	B1D1
	cmp	dx,[467E]
	jnc	B1DE
	mov	ax,0001
	push	ax
	push	ax
	call	B90B
	add	sp,04
	jmp	B1EB
	mov	ax,0001
	push	ax
	xor	ax,ax
	push	ax
	call	B90B
	add	sp,04
	cmp	word ptr [2E45],00
	jnz	B20F
	xor	si,si
	jmp	B208
	mov	bx,si
	inc	si
	mov	al,[bx+4682]
	push	ax
	call	B9E4
	add	sp,02
	dec	word ptr [4E82]
	cmp	word ptr [4E82],00
	jnz	B1F6
	add	word ptr [29F7],01
	adc	word ptr [29F9],00
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	E0EF
	add	sp,04
	mov	[bp-02],dx
	mov	[bp-04],ax
	mov	ax,[467C]
	mov	dx,[467A]
	cmp	ax,[4680]
	jnc	B23D
	jmp	B04F
	jnz	B248
	cmp	dx,[467E]
	jnc	B248
	jmp	B04F
	mov	cl,08
	sub	cl,[2E45]
	shl	word ptr [2E43],cl
	cmp	word ptr [2E45],00
	jnz	B260
	cmp	word ptr [4E82],00
	jz	B26A
	mov	al,[2E43]
	push	ax
	call	B9E4
	add	sp,02
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	si,[bp+04]
	jmp	B391
	cmp	si,0C
	jnc	B2F2
	jmp	B2B6
	mov	ax,0001
	push	ax
	xor	ax,ax
	push	ax
	call	B90B
	add	sp,04
	call	B719
	xor	al,[2E4F]
	push	ax
	call	B9C0
	add	sp,02
	test	word ptr [2E4F],0001
	jz	B2B1
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	B2B5
	shr	word ptr [2E4F],01
	dec	si
	or	si,si
	jnz	B281
	jmp	B391
	mov	ax,0001
	push	ax
	xor	ax,ax
	push	ax
	call	B90B
	add	sp,04
	call	B719
	xor	al,[2E4F]
	push	ax
	call	B9C0
	add	sp,02
	test	word ptr [2E4F],0001
	jz	B2ED
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	B2F1
	shr	word ptr [2E4F],01
	dec	si
	test	si,0003
	jnz	B2BD
	mov	ax,0005
	push	ax
	mov	ax,0017
	push	ax
	call	B90B
	add	sp,04
	cmp	si,48
	jc	B34F
	mov	ax,0004
	push	ax
	mov	ax,000F
	push	ax
	call	B90B
	add	sp,04
	xor	di,di
	jmp	B32C
	call	B719
	xor	al,[2E4F]
	push	ax
	call	B9C0
	add	sp,02
	inc	di
	cmp	di,48
	jnz	B31D
	test	word ptr [2E4F],0001
	jz	B346
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	B34A
	shr	word ptr [2E4F],01
	sub	si,48
	jmp	B391
	mov	ax,0004
	push	ax
	mov	ax,si
	sub	ax,000C
	shr	ax,01
	shr	ax,01
	push	ax
	call	B90B
	add	sp,04
	jmp	B374
	call	B719
	xor	al,[2E4F]
	push	ax
	call	B9C0
	add	sp,02
	dec	si
	or	si,si
	jnz	B365
	test	word ptr [2E4F],0001
	jz	B38D
	mov	ax,[2E4F]
	shr	ax,01
	or	ax,8000
	mov	[2E4F],ax
	jmp	B391
	shr	word ptr [2E4F],01
	or	si,si
	jz	B398
	jmp	B27A
	pop	di
	pop	si
	pop	bp
	ret	
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
	call	E073
	add	sp,0A
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	E073
	add	sp,0A
	jmp	B661
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
	ja	B43C
	jc	B42F
	cmp	dx,[bp-04]
	jnc	B43C
	mov	ax,[4678]
	mov	dx,[4676]
	mov	[bp-02],ax
	mov	[bp-04],dx
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
	call	74D0
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
	jbe	B4AD
	jmp	B5A9
	jc	B4B8
	cmp	dx,[4672]
	jc	B4B8
	jmp	B5A9
	mov	ax,[2E6F]
	mov	dx,[2E6D]
	add	dx,[466E]
	mov	[2E67],ax
	mov	[2E65],dx
	jmp	B5A9
	call	BBF2
	cmp	word ptr [2E29],02
	jc	B553
	mov	ax,[2E6D]
	add	ax,[2E29]
	cmp	ax,[2E65]
	jbe	B4FF
	mov	ax,[466A]
	or	ax,[466C]
	jz	B4F0
	jmp	B5C9
	mov	ax,[2E65]
	xor	dx,dx
	sub	ax,[2E6D]
	sbb	dx,00
	mov	[2E29],ax
	push	ds
	mov	ax,290F
	push	ax
	push	word ptr [2E47]
	call	B6DD
	add	sp,06
	push	ds
	mov	ax,278F
	push	ax
	mov	ax,[2E29]
	sub	ax,0002
	push	ax
	call	B6DD
	add	sp,06
	push	ds
	mov	ax,284F
	push	ax
	mov	ax,[2E2B]
	dec	ax
	push	ax
	call	B6DD
	add	sp,06
	push	word ptr [2E29]
	call	BD68
	add	sp,02
	add	word ptr [466A],01
	adc	word ptr [466C],00
	mov	word ptr [2E47],0000
	mov	ax,[2E29]
	add	[465C],ax
	jmp	B565
	mov	ax,0001
	push	ax
	call	BD68
	add	sp,02
	inc	word ptr [2E47]
	inc	word ptr [465C]
	cmp	word ptr [465C],0400
	jc	B5A9
	mov	ax,[465C]
	add	[4666],ax
	adc	word ptr [4668],00
	push	word ptr [4680]
	push	word ptr [467E]
	mov	cx,[4668]
	mov	bx,[4666]
	xor	dx,dx
	mov	ax,0063
	call	C2D8
	push	dx
	push	ax
	call	BF82
	push	dx
	push	ax
	push	ds
	mov	ax,2244
	push	ax
	call	E6AF
	add	sp,08
	mov	word ptr [465C],0000
	mov	ax,[2E65]
	dec	ax
	cmp	ax,[2E6D]
	jbe	B5C9
	cmp	word ptr [466C],00
	jnc	B5BD
	jmp	B4CD
	jnz	B5C9
	cmp	word ptr [466A],FE
	jnc	B5C9
	jmp	B4CD
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
	call	E4B3
	add	sp,0A
	mov	ax,[2E65]
	cmp	ax,[2E69]
	jc	B679
	mov	ax,[2E67]
	mov	dx,[2E65]
	cmp	ax,[2E6B]
	jnz	B645
	cmp	dx,[2E69]
	jnz	B645
	mov	ax,[4676]
	or	ax,[4678]
	jz	B679
	cmp	word ptr [466C],00
	jnz	B653
	cmp	word ptr [466A],FE
	jz	B679
	mov	ax,[bp-02]
	mov	dx,[bp-04]
	sub	[466E],dx
	sbb	[4670],ax
	mov	ax,[4676]
	or	ax,[4678]
	jz	B66D
	jmp	B402
	mov	ax,[4672]
	or	ax,[4674]
	jz	B679
	jmp	B402
	mov	ax,[2E67]
	mov	dx,[2E65]
	cmp	ax,[2E6B]
	jnz	B6AD
	cmp	dx,[2E69]
	jnz	B6AD
	mov	ax,[4676]
	or	ax,[4678]
	jnz	B6AD
	cmp	word ptr [466C],00
	jnz	B6A3
	cmp	word ptr [466A],FE
	jz	B6AD
	mov	ax,[2E47]
	add	ax,[4672]
	mov	[2E47],ax
	push	ds
	mov	ax,290F
	push	ax
	push	word ptr [2E47]
	call	B6DD
	add	sp,06
	add	word ptr [466A],01
	adc	word ptr [466C],00
	xor	ax,ax
	push	ax
	push	ax
	push	ax
	push	word ptr [29DD]
	push	word ptr [29DB]
	call	E073
	add	sp,0A
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+04]
	cmp	si,01
	jbe	B6F2
	push	si
	call	3FC8
	add	sp,02
	jmp	B6F4
	mov	ax,si
	mov	dx,000C
	imul	dx
	les	bx,[bp+06]
	add	bx,ax
	add	word ptr es:[bx],01
	adc	word ptr es:[bx+02],00
	push	word ptr [29DD]
	push	word ptr [29DB]
	push	si
	call	73DE
	add	sp,06
	pop	si
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,02
	push	word ptr [29E5]
	push	word ptr [29E3]
	call	718F
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
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	push	di
	mov	di,[bp+08]
	mov	si,di
	jmp	B784
	mov	ax,si
	mov	dx,000C
	imul	dx
	les	bx,[bp+04]
	add	bx,ax
	cmp	word ptr es:[bx+0A],00
	jnz	B78B
	dec	di
	mov	ax,si
	dec	si
	or	ax,ax
	jnz	B770
	mov	ax,0005
	push	ax
	push	di
	call	B849
	add	sp,04
	xor	si,si
	mov	ax,[bp+04]
	add	ax,000A
	mov	[bp-02],ax
	cmp	si,di
	jnc	B7C1
	mov	ax,0004
	push	ax
	mov	es,[bp+06]
	mov	bx,[bp-02]
	push	word ptr es:[bx]
	call	B849
	add	sp,04
	add	word ptr [bp-02],0C
	inc	si
	cmp	si,di
	jc	B7A5
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	sub	sp,02
	push	si
	push	di
	mov	si,[bp+04]
	cmp	si,01
	jbe	B7E0
	push	si
	call	3FC8
	add	sp,02
	jmp	B7E2
	mov	ax,si
	mov	[bp-02],ax
	mov	dx,000C
	imul	dx
	les	bx,[bp+06]
	add	bx,ax
	mov	di,bx
	push	word ptr es:[bx+0A]
	push	word ptr es:[di+06]
	call	B849
	add	sp,04
	cmp	word ptr [bp-02],01
	jbe	B81F
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
	call	B849
	add	sp,04
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	mov	ax,[bp+04]
	mov	dx,[bp+06]
	cmp	word ptr [2A21],02
	jnz	B83F
	push	dx
	push	ax
	call	B90B
	add	sp,04
	pop	bp
	ret	
	push	dx
	push	ax
	call	B849
	add	sp,04
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	di,[bp+04]
	jmp	B8FA
	shr	word ptr [2E43],01
	test	di,0001
	jz	B864
	or	word ptr [2E43],8000
	shr	di,01
	inc	word ptr [2E45]
	mov	ax,[2E45]
	cmp	ax,0010
	jz	B875
	jmp	B8FA
	mov	al,[2E43]
	push	ax
	call	B9E4
	add	sp,02
	mov	ax,[2E43]
	mov	cl,08
	shr	ax,cl
	push	ax
	call	B9E4
	add	sp,02
	xor	si,si
	jmp	B8A3
	mov	bx,si
	inc	si
	mov	al,[bx+4682]
	push	ax
	call	B9E4
	add	sp,02
	dec	word ptr [4E82]
	cmp	word ptr [4E82],00
	jnz	B891
	mov	ax,[2A01]
	mov	dx,[29FF]
	cmp	ax,[2A05]
	jc	B8F2
	ja	B8BF
	cmp	dx,[2A03]
	jbe	B8F2
	mov	ax,[2A01]
	mov	dx,[29FF]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	cmp	ax,[4E8A]
	jc	B8F2
	ja	B8DC
	cmp	dx,[4E88]
	jbe	B8F2
	mov	ax,[2A01]
	mov	dx,[29FF]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	mov	[4E8A],ax
	mov	[4E88],dx
	xor	ax,ax
	mov	[2E45],ax
	mov	[2E43],ax
	mov	ax,[bp+06]
	dec	word ptr [bp+06]
	or	ax,ax
	jz	B907
	jmp	B854
	pop	di
	pop	si
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	cl,[bp+06]
	dec	cl
	mov	di,0001
	shl	di,cl
	jmp	B9AF
	shl	word ptr [2E43],01
	test	[bp+04],di
	jz	B92A
	inc	word ptr [2E43]
	shr	di,01
	inc	word ptr [2E45]
	mov	ax,[2E45]
	cmp	ax,0008
	jnz	B9AF
	mov	al,[2E43]
	push	ax
	call	B9E4
	add	sp,02
	xor	si,si
	jmp	B958
	mov	bx,si
	inc	si
	mov	al,[bx+4682]
	push	ax
	call	B9E4
	add	sp,02
	dec	word ptr [4E82]
	cmp	word ptr [4E82],00
	jnz	B946
	mov	ax,[2A01]
	mov	dx,[29FF]
	cmp	ax,[2A05]
	jc	B9A7
	ja	B974
	cmp	dx,[2A03]
	jbe	B9A7
	mov	ax,[2A01]
	mov	dx,[29FF]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	cmp	ax,[4E8A]
	jc	B9A7
	ja	B991
	cmp	dx,[4E88]
	jbe	B9A7
	mov	ax,[2A01]
	mov	dx,[29FF]
	sub	dx,[2A03]
	sbb	ax,[2A05]
	mov	[4E8A],ax
	mov	[4E88],dx
	xor	ax,ax
	mov	[2E45],ax
	mov	[2E43],ax
	mov	ax,[bp+06]
	dec	word ptr [bp+06]
	or	ax,ax
	jz	B9BC
	jmp	B91D
	pop	di
	pop	si
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	mov	dl,[bp+04]
	cmp	word ptr [2E45],00
	jz	B9DB
	mov	bx,[4E82]
	mov	[bx+4682],dl
	inc	word ptr [4E82]
	pop	bp
	ret	
	push	dx
	call	B9E4
	add	sp,02
	pop	bp
	ret	
	push	bp
	mov	bp,sp
	mov	ax,[2A05]
	mov	dx,[2A03]
	cmp	ax,[4E86]
	ja	BA38
	jc	B9FC
	cmp	dx,[4E84]
	jnc	BA38
	push	word ptr [29E1]
	push	word ptr [29DF]
	mov	al,[bp+04]
	push	ax
	call	7407
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
	pop	bp
	ret	

fn0800_867A()
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
	rep	
	movsw	

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
	rep	
	movsb	

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

fn0800_87EF()
	call	87F4
	mov	bx,ax

fn0800_87F4()
	lodsw	
	xchg	al,ah
	ret	
0800:87F8                         57 FC C4 3E 37 2E A1 31         W..>7..1
0800:8800 2E B9 00 80 F3 AB C4 3E 33 2E A1 31 2E B9 00 80 .......>3..1....
0800:8810 F3 AB C4 3E 3B 2E 8B 0E 31 2E 33 C0 F3 AB C4 3E ...>;...1.3....>
0800:8820 3F 2E 33 C0 8B 0E 31 2E AB 40 E2 FC 89 0E 2D 2E ?.3...1..@....-.
0800:8830 5F C3 56 57 E8 63 00 83 3E 29 2E 02 72 59 90 90 _.VW.c..>)..rY..
0800:8840 90 A1 65 2E 2B 06 6D 2E 3D 03 00 72 4A 90 90 90 ..e.+.m.=..rJ...
0800:8850 8B 36 29 2E 8B 3E 2B 2E A1 2D 2E 50 40 3B 06 31 .6)..>+..-.P@;.1
0800:8860 2E 75 02 33 C0 A3 2D 2E FF 06 6D 2E E8 2B 00 FF .u.3..-...m..+..
0800:8870 0E 6D 2E 8F 06 2D 2E 39 36 29 2E 76 12 90 90 90 .m...-.96).v....
0800:8880 C7 06 29 2E 01 00 C7 06 2B 2E 00 00 EB 09 90 89 ..).....+.......
0800:8890 36 29 2E 89 3E 2B 2E 5F 5E C3 56 57 FC C7 06 2B 6)..>+._^.VW...+
0800:88A0 2E 00 00 C7 06 29 2E 01 00 C4 3E 6D 2E 26 8B 05 .....)....>m.&..
0800:88B0 A3 90 4E 47 8B 16 69 2E 2B D7 8B CA F3 AE 75 01 ..NG..i.+.....u.
0800:88C0 49 2B D1 C4 3E 6D 2E A1 69 2E 2B C7 A3 94 4E 8B I+..>m..i.+...N.
0800:88D0 3E 90 4E D1 E7 8E 06 39 2E 26 8B 05 3B 06 31 2E >.N....9.&..;.1.
0800:88E0 75 03 E9 9F 00 8B F8 D1 E7 8E 06 41 2E 26 8B 1D u..........A.&..
0800:88F0 89 1E 92 4E 8B 1E 2D 2E 3B D8 77 07 90 90 90 03 ...N..-.;.w.....
0800:8900 1E 31 2E 2B D8 C4 36 6D 2E 2B F3 26 8B 04 39 06 .1.+..6m.+.&..9.
0800:8910 90 4E 75 6A 90 90 90 8E 06 3D 2E 26 8B 0D 3B CB .Nuj.....=.&..;.
0800:8920 76 0B 90 90 90 BB 01 00 8B CA EB 37 90 3B CA 76 v..........7.;.v
0800:8930 09 90 90 90 2B CA 2B D9 8B CA 3B CA 75 25 90 90 ....+.+...;.u%..
0800:8940 90 C4 3E 6D 2E 03 F9 8B F7 2B F3 A1 94 4E 2B C1 ..>m.....+...N+.
0800:8950 8B C8 1E 8E 1E 6F 2E F3 A6 74 01 41 1F 2B C1 8B .....o...t.A.+..
0800:8960 CA 03 C8 3B 0E 2F 2E 76 04 8B 0E 2F 2E 3B 0E 29 ...;./.v.../.;.)
0800:8970 2E 72 0B 90 90 90 89 0E 29 2E 89 1E 2B 2E A1 92 .r......)...+...
0800:8980 4E E9 58 FF 83 3E 29 2E 02 75 1A 90 90 90 81 3E N.X..>)..u.....>
0800:8990 2B 2E 00 01 76 0F 90 90 90 C7 06 29 2E 01 00 C7 +...v......)....
0800:89A0 06 2B 2E 00 00 5F 5E C3 55 8B EC 57 8B 16 31 2E .+..._^.U..W..1.
0800:89B0 FC 8B 3E 2D 2E D1 E7 8E 06 41 2E 26 8B 05 26 89 ..>-.....A.&..&.
0800:89C0 15 39 06 2D 2E 74 23 90 90 90 C4 3E 6D 2E 2B FA .9.-.t#....>m.+.
0800:89D0 26 8B 3D D1 E7 8E 06 39 2E 26 89 05 3B C2 75 0A &.=....9.&..;.u.
0800:89E0 90 90 90 8E 06 35 2E 26 89 15 C4 3E 6D 2E 26 8B .....5.&...>m.&.
0800:89F0 3D D1 E7 A1 2D 2E 8E 06 39 2E 26 39 15 75 09 90 =...-...9.&9.u..
0800:8A00 90 90 26 89 05 EB 11 90 8E 06 35 2E 26 8B 1D D1 ..&.......5.&...
0800:8A10 E3 8E 06 41 2E 26 89 07 8E 06 35 2E 26 89 05 C4 ...A.&....5.&...
0800:8A20 3E 6D 2E 26 8A 05 47 8B 1E 69 2E 2B DF 8B CB F3 >m.&..G..i.+....
0800:8A30 AE 75 01 49 2B D9 8B 3E 2D 2E D1 E7 8E 06 3D 2E .u.I+..>-.....=.
0800:8A40 26 89 1D EB 41 90 8B 3E 2D 2E D1 E7 8E 06 3D 2E &...A..>-.....=.
0800:8A50 26 89 1D 8E 06 41 2E A1 2D 2E 26 87 05 39 06 2D &....A..-.&..9.-
0800:8A60 2E 74 23 90 90 90 C4 3E 6D 2E 2B FA 26 8B 3D D1 .t#....>m.+.&.=.
0800:8A70 E7 8E 06 39 2E 26 89 05 3B C2 75 0A 90 90 90 8E ...9.&..;.u.....
0800:8A80 06 35 2E 26 89 15 A1 2D 2E 40 3B C2 75 02 33 C0 .5.&...-.@;.u.3.
0800:8A90 A3 2D 2E FF 06 6D 2E FF 4E 04 74 0C 90 90 90 4B .-...m..N.t....K
0800:8AA0 83 FB 01 77 A1 E9 09 FF 5F 5D C3 55 8B EC 83 3E ...w...._].U...>
0800:8AB0 4E 22 20 75 05 B8 01 00 EB 13 8B 1E 4E 22 D1 E3 N" u........N"..
0800:8AC0 8B 46 04 89 87 96 4E FF 06 4E 22 33 C0 5D C3 55 .F....N..N"3.].U
0800:8AD0 8B EC 1E B4 43 32 C0 C5 56 04 CD 21 1F 72 0A C4 ....C2..V..!.r..
0800:8AE0 5E 08 26 89 0F 33 C0 EB 04 50 E8 77 02 5D C3 55 ^.&..3...P.w.].U
0800:8AF0 8B EC 1E B4 43 B0 01 C5 56 04 8B 4E 08 CD 21 1F ....C...V..N..!.
0800:8B00 72 04 33 C0 EB 04 50 E8 5A 02 5D C3 C3          r.3...P.Z.]..  

fn0800_8B0D()
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

fn0800_8B5A()
	push	bp
	mov	bp,sp
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+04]
	call	8B0D
	pop	bp
	ret	
0800:8B69                            55 8B EC B8 01 00 50          U.....P
0800:8B70 33 C0 50 FF 76 04 E8 94 FF 5D C3 33 C0 50 B8 01 3.P.v....].3.P..
0800:8B80 00 50 33 C0 50 E8 85 FF C3 B8 01 00 50 50 33 C0 .P3.P.......PP3.
0800:8B90 50 E8 79 FF C3                                  P.y..          

fn0800_8B95()
	push	bp
	mov	bp,sp
	mov	ah,2A
	int	21
	les	bx,[bp+04]
	mov	es:[bx],cx
	mov	es:[bx+02],dx
	pop	bp
	ret	

fn0800_8BA8()
	push	bp
	mov	bp,sp
	mov	ah,2C
	int	21
	les	bx,[bp+04]
	mov	es:[bx],cx
	mov	es:[bx+02],dx
	pop	bp
	ret	

fn0800_8BBB()
	pop	cx
	push	cs
	push	cx
	xor	cx,cx
	jmp	8BD8

fn0800_8BC2()
	pop	cx
	push	cs
	push	cx
	mov	cx,0001
	jmp	8BD8

fn0800_8BCA()
	pop	cx
	push	cs
	push	cx
	mov	cx,0002
	jmp	8BD8
0800:8BD2       59 0E 51 B9 03 00                           Y.Q...       

fn0800_8BD8()
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

fn0800_8C69()
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
0800:8C8A                               5B 0E 53 80 F9 10           [.S...
0800:8C90 73 10 8B DA D3 E8 D3 FA F6 D9 80 C1 10 D3 E3 0B s...............
0800:8CA0 C3 CB 80 E9 10 92 99 D3 F8 CB 5B 0E 53 80 F9 10 ..........[.S...
0800:8CB0 73 10 8B DA D3 E8 D3 EA F6 D9 80 C1 10 D3 E3 0B s...............
0800:8CC0 C3 CB 80 E9 10 92 33 D2 D3 E8 CB                ......3....    

fn0800_8CCB()
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

fn0800_8D2B()
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
0800:8D64             55 8B EC 56 8B 76 04 56 E8 BC FF 8B     U..V.v.V....
0800:8D70 C6 5E 5D C2 02 00 55 8B EC B8 00 44 8B 5E 04 CD .^]...U....D.^..
0800:8D80 21 92 25 80 00 5D C3                            !.%..].        

fn0800_8D87()
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

fn0800_8E09()
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

fn0800_8E29()
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
0800:8E52       55 8B EC 1E B4 39 C5 56 04 CD 21 1F 72 04   U....9.V..!.r.
0800:8E60 33 C0 EB 04 50 E8 C3 FE 5D C3                   3...P...].     

fn0800_8E6A()
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

fn0800_8F18()
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

fn0800_8F2F()
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
0800:8F50 55 8B EC 8B 5E 04 D1 E3 F7 87 EA 24 02 00 74 06 U...^......$..t.
0800:8F60 B8 05 00 50 EB 14 1E B4 3F 8B 5E 04 8B 4E 0A C5 ...P....?.^..N..
0800:8F70 56 06 CD 21 1F 72 02 EB 04 50 E8 AE FD 5D C3    V..!.r...P...].

fn0800_8F7F()
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
0800:8F97                      55 8B EC 83 EC 2A 56 57 C7        U....*VW.
0800:8FA0 46 FC 00 00 C7 46 FA 00 00 EB 1B                F....F.....    

fn027F_E7BB()
	les	di,[bp+10]
	test	byte ptr [bp-01],20
	jz	E7CC

l027F_E7C4:
	les	di,es:[di]
	add	word ptr [bp+10],04
	ret	

l027F_E7CC:
	mov	di,es:[di]
	push	ds
	pop	es
	add	word ptr [bp+10],02
	ret	
	push	es
	cld	
	mov	si,[bp+0C]
	mov	es,[bp+0E]
	lodsb	
	or	al,al
	jz	E852
	cmp	al,25
	jz	E855
	cbw	
	xchg	ax,di
	inc	word ptr [bp-06]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	or	ax,ax
	jl	E826
	cbw	
	or	di,di
	js	E83B
	cmp	byte ptr [di+257C],01
	jnz	E83B
	xchg	ax,bx
	or	bl,bl
	js	E829
	cmp	byte ptr [bx+257C],01
	jnz	E829
	inc	word ptr [bp-06]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	or	ax,ax
	jg	E808
	jmp	EBB6
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	bx
	call	word ptr [bp+06]
	add	sp,06
	dec	word ptr [bp-06]
	jmp	E7DB
	cmp	ax,di
	jz	E7DB
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06

fn08C4_83FB()
	push	es
	dec	word ptr [bp-06]
	jmp	877E
08C4:8402       E9 79 03 C7 46 F6 FF FF 8E 46 0E C6 46 FF   .y..F....F..F.
08C4:8410 20 26 AC 98 89 76 0C 97 0B FF 7C 19 8A 9D 7C 25  &...v....|...|%
08C4:8420 32 FF 83 FB 15 76 03 E9 3C 03 D1 E3 2E FF A7 59 2....v..<......Y
08C4:8430 94 97 E9 63 FF E9 46 03 80 4E FF 01 EB D3 83 EF ...c..F..N......
08C4:8440 30 87 7E F6 0B FF 7C C9 B8 0A 00 F7 E7 01 46 F6 0.~...|.......F.
08C4:8450 EB BF 80 4E FF 08 EB B9 80 4E FF 04 EB B3 80 4E ...N.....N.....N
08C4:8460 FF 02 EB AD 80 66 FF DF EB A7 80 4E FF 20 EB A1 .....f.....N. ..
08C4:8470 8B 46 FA 2B D2 F6 46 FF 01 74 55 EB 94 BE 08 00 .F.+..F..tU.....
08C4:8480 EB 0C BE 0A 00 EB 07 BE 10 00 EB 02 33 F6 F7 C7 ............3...
08C4:8490 20 00 75 09 83 FF 58 74 04 80 4E FF 04 16 8D 46  .u...Xt..N....F
08C4:84A0 F8 50 16 8D 46 FA 50 8B 46 F6 25 FF 7F 50 56 FF .P..F.P.F.%..PV.
08C4:84B0 76 0A                                           v.             

fn027F_E902()
	push	word ptr [bp+08]
	push	word ptr [bp+06]
	push	word ptr [bp+04]
	call	ECC0
	add	sp,14
	cmp	word ptr [bp-08],00
	jle	E92F

l027F_E917:
	test	byte ptr [bp-01],01
	jnz	E92C

l027F_E91D:
	inc	word ptr [bp-04]
	call	E7BB
	stosw	
	test	byte ptr [bp-01],04
	jz	E92C

l027F_E92A:
	xchg	ax,dx
	stosw	

l027F_E92C:
	jmp	E7D8

l027F_E92F:
	jl	E934
	jmp	EBCE
	jmp	EBB6
	call	E93A
	jmp	EBD5
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	dec	word ptr [bp-06]
	and	word ptr [bp-0A],7FFF
	call	E955
	jmp	EBFF
	push	dx
	cmp	al,3A
	jz	E976
	or	ax,ax
	jle	E971
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	dec	word ptr [bp-06]
	pop	dx
	mov	bx,ds
	jmp	E995
	call	E979
	jmp	EBFF
	pop	bx
	or	ax,ax
	jle	E995
	push	dx
	push	bx
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	dec	word ptr [bp-06]
	pop	bx
	pop	dx
	test	byte ptr [bp-01],01
	jnz	E9AB
	call	E7BB
	inc	word ptr [bp-04]
	xchg	ax,dx
	stosw	
	test	byte ptr [bp-01],20
	jz	E9AB
	xchg	ax,bx
	stosw	
	jmp	E7D8
	jmp	EBB6
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
	call	FAE0
	add	sp,12
	cmp	word ptr [bp-08],00
	jle	EA13
	mov	al,[bp-01]
	cbw	
	test	ax,0001
	jnz	EA0D
	call	E7BB
	inc	word ptr [bp-04]
	test	byte ptr [bp-01],04
	jz	E9F4
	mov	ax,0004
	jmp	EA01
	test	byte ptr [bp-01],08
	jz	E9FF
	mov	ax,0008
	jmp	EA01
	xor	ax,ax
	push	ax
	push	es
	push	di
	call	FAE4
	add	sp,06
	jmp	E7D8
	call	FAE8
	jmp	E7D8
	call	FAE8
	jl	E9AE
	jmp	EBCE
	call	EA1E
	jmp	EBD5
	test	byte ptr [bp-01],01
	jnz	EA2D
	call	E7BB
	inc	word ptr [bp-04]
	and	word ptr [bp-0A],7FFF
	jz	EA61
	test	byte ptr [bp-01],01
	jnz	EA3B
	stosb	
	inc	word ptr [bp-06]
	push	es
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	pop	es
	or	ax,ax
	jle	EA61
	or	al,al
	js	EA5C
	xchg	ax,bx
	cmp	byte ptr [bx+257C],01
	xchg	ax,bx
	jle	EA61
	dec	word ptr [bp-0A]
	jg	EA34
	push	es
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	pop	es
	dec	word ptr [bp-06]
	test	byte ptr [bp-01],01
	jnz	EA7C
	mov	al,00
	stosb	
	jmp	E7D8
	test	byte ptr [bp-01],01
	jnz	EA88
	call	E7BB
	mov	si,[bp-0A]
	or	si,si
	jge	EA92
	mov	si,0001
	jz	EAB2
	inc	word ptr [bp-06]
	push	es
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	pop	es
	or	ax,ax
	jl	EABE
	test	byte ptr [bp-01],01
	jnz	EAAF
	stosb	
	dec	si
	jg	EA94
	test	byte ptr [bp-01],01
	jnz	EABB
	inc	word ptr [bp-04]
	jmp	E7D8
	jmp	EBB6
	push	es
	sub	ax,ax
	cld	
	push	ss
	pop	es
	lea	di,[bp-2A]
	mov	cx,0010
	rep	
	stosw	
	pop	es
	lodsb	
	and	byte ptr [bp-01],EF
	cmp	al,5E
	jnz	EAE0
	or	byte ptr [bp-01],10
	lodsb	
	mov	ah,00
	mov	dl,al
	mov	di,ax
	mov	cl,03
	shr	di,cl
	mov	cx,0107
	and	cl,dl
	shl	ch,cl
	or	[bp+di-2A],ch
	lodsb	
	cmp	al,00
	jz	EB23
	cmp	al,5D
	jz	EB26
	cmp	al,2D
	jnz	EAE2
	cmp	dl,es:[si]
	ja	EAE2
	cmp	byte ptr es:[si],5D
	jz	EAE2
	lodsb	
	sub	al,dl
	jz	EAF4
	add	dl,al
	rol	ch,01
	adc	di,00
	or	[bp+di-2A],ch
	dec	al
	jnz	EB15
	jmp	EAF4
	jmp	EBCE
	mov	[bp+0C],si
	and	word ptr [bp-0A],7FFF
	mov	si,[bp-0A]
	test	byte ptr [bp-01],01
	jnz	EB3A
	call	E7BB
	dec	si
	jl	EB95
	inc	word ptr [bp-06]
	push	es
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	pop	es
	or	ax,ax
	jl	EBA4
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
	jz	EB6E
	test	byte ptr [bp-01],10
	jz	EB74
	jmp	EB7D
	test	byte ptr [bp-01],10
	jz	EB7D
	test	byte ptr [bp-01],01
	jnz	EB3A
	stosb	
	jmp	EB3A
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
	jge	EB9E
	test	byte ptr [bp-01],01
	jnz	EBA1
	inc	word ptr [bp-04]
	mov	al,00
	stosb	
	jmp	E7D8
	inc	si
	cmp	si,[bp-0A]
	jge	EBB6
	test	byte ptr [bp-01],01
	jnz	EBB6
	mov	al,00
	stosb	
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	mov	ax,FFFF
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	cmp	word ptr [bp-04],01
	sbb	word ptr [bp-04],00

l08C4_877E:
	pop	es
	mov	ax,[bp-04]
	jmp	8813
	inc	word ptr [bp-06]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	or	ax,ax
	jle	87AA
	or	al,al
	js	87A4
	xchg	ax,bx
	cmp	byte ptr [bx+257C],01
	xchg	ax,bx
	jz	8785
	pop	cx
	add	cx,03
	jmp	cx
	jz	87A4
	pop	cx
	jmp	8766
	sub	dx,dx
	mov	cx,0004
	dec	word ptr [bp-0A]
	jl	8802
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
	jle	8804
	dec	cl
	jl	8804
	mov	ch,al
	sub	ch,30
	jc	8804
	cmp	ch,0A
	jc	87F6
	sub	ch,11
	jc	8804
	cmp	ch,06
	jc	87F3
	sub	ch,20
	jc	8804
	cmp	ch,06
	jnc	8804
	add	ch,0A
	shl	dx,01
	shl	dx,01
	shl	dx,01
	shl	dx,01
	add	dl,ch
	jmp	87B4
	sub	ax,ax
	cmp	cl,04
	jz	880F
	pop	cx
	add	cx,03
	jmp	cx
	pop	cx
	jmp	8766
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
	mov	si,BE93
	xchg	ax,bx
	mov	si,7193
	nop	
	js	87B3
	jle	87B5
	outsw	
	xchg	ax,dx
	ret	C290
	nop	
	int	03
	nop	
	mov	ax,[9291]
	nop	
	sahf	
	nop	
	cbw	
	nop	
	mov	bp,0B90
	xchg	ax,dx
	mov	cl,92
	mov	al,90
	mov	word ptr [bx+si+9127],90A4
	stosb	
	nop	
	push	bx
	sub	bl,30
	jc	886D
	cmp	bl,09
	jbe	8862
	cmp	bl,2A
	ja	885A
	sub	bl,07
	jmp	885D
	sub	bl,27
	cmp	bl,09
	jbe	886D
	cmp	bl,cl
	jnc	886D
	inc	sp
	inc	sp
	clc	
	mov	bh,00
	jmp	886F
	pop	bx
	stc	
	ret	

fn027F_ECC0()
	push	bp
	mov	bp,sp
	sub	sp,06
	push	si
	push	di
	mov	byte ptr [bp-01],00
	mov	word ptr [bp-04],0000
	mov	word ptr [bp-06],0001

l027F_ECD6:
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	or	ax,ax
	jl	ED59

l027F_ECE8:
	cbw	
	xchg	ax,bx
	test	bl,80
	jnz	ECF7

l027F_ECEF:
	mov	di,2251
	test	byte ptr [bx+di],01
	jnz	ECD6

l027F_ECF7:
	xchg	ax,bx
	dec	word ptr [bp+0E]
	jl	ED60

l027F_ECFD:
	cmp	al,2B
	jz	ED08

l027F_ED01:
	cmp	al,2D
	jnz	ED1F

l027F_ED05:
	inc	byte ptr [bp-01]

l027F_ED08:
	dec	word ptr [bp+0E]
	jl	ED60
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	or	ax,ax
	jl	ED59

l027F_ED1F:
	sub	si,si
	mov	di,si
	mov	cx,[bp+0C]
	jcxz	ED7E
	cmp	cx,24
	ja	ED60
	cmp	cl,02
	jc	ED60
	cmp	al,30
	jnz	EDAE
	cmp	cl,10
	jnz	EDAC
	dec	word ptr [bp+0E]
	jl	ED7B
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	cmp	al,78
	jz	EDAC
	cmp	al,58
	jz	EDAC
	jmp	EDD8

l027F_ED59:
	mov	word ptr [bp-06],FFFF
	jmp	ED65

l027F_ED60:
	mov	word ptr [bp-06],0000
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	dec	word ptr [bp-04]
	sub	ax,ax
	cwd	
	jmp	EE32
	jmp	EE22
	cmp	al,30
	mov	word ptr [bp+0C],000A
	jnz	EDAE
	dec	word ptr [bp+0E]
	jl	ED7B
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	mov	word ptr [bp+0C],0008
	cmp	al,78
	jz	EDA7
	cmp	al,58
	jnz	EDD8
	mov	word ptr [bp+0C],0010
	jmp	EDC5
	mov	cx,[bp+0C]
	xchg	ax,bx
	call	EC95
	xchg	ax,bx
	jc	ED60
	xchg	ax,si
	jmp	EDC5
	xchg	ax,si
	mul	word ptr [bp+0C]
	add	si,ax
	adc	di,dx
	jnz	EDF5
	dec	word ptr [bp+0E]
	jl	EE22
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	mov	cx,[bp+0C]
	xchg	ax,bx
	call	EC95
	xchg	ax,bx
	jnc	EDBB
	jmp	EE12
	xchg	ax,si
	mul	cx
	xchg	ax,di
	xchg	dx,cx
	mul	dx
	add	si,di
	adc	ax,cx
	xchg	ax,di
	adc	dl,dh
	jnz	EE46
	dec	word ptr [bp+0E]
	jl	EE22
	inc	word ptr [bp-04]
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	call	word ptr [bp+04]
	pop	cx
	pop	cx
	mov	cx,[bp+0C]
	xchg	ax,bx
	call	EC95
	xchg	ax,bx
	jnc	EDE4
	push	word ptr [bp+0A]
	push	word ptr [bp+08]
	push	ax
	call	word ptr [bp+06]
	add	sp,06
	dec	word ptr [bp-04]
	mov	dx,di
	xchg	ax,si
	cmp	byte ptr [bp-01],00
	jz	EE32
	neg	dx
	neg	ax
	sbb	dx,00
	les	di,[bp+10]
	mov	bx,[bp-04]
	add	es:[di],bx
	les	di,[bp+14]
	mov	bx,[bp-06]
	mov	es:[di],bx
	jmp	EE5C
	mov	ax,FFFF
	mov	dx,7FFF
	add	al,[bp-01]
	adc	ah,00
	adc	dx,00
	mov	word ptr [bp-06],0002
	jmp	EE32
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	
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
	mov	cx,0005
	cmp	cx,[24E8]
	jnc	EECD
	mov	bx,cx
	shl	bx,01
	mov	word ptr [bx+24EA],0000
	mov	ax,cx
	mov	dx,0014
	imul	dx
	mov	bx,ax
	mov	byte ptr [bx+235C],FF
	mov	ax,cx
	mov	dx,0014
	imul	dx
	add	ax,2358
	push	ax
	mov	ax,cx
	mov	dx,0014
	imul	dx
	mov	bx,ax
	pop	ax
	mov	[bx+236A],ax
	inc	cx
	cmp	cx,[24E8]
	jc	EE95
	mov	al,[235C]
	cbw	
	push	ax
	call	E586
	pop	cx
	or	ax,ax
	jnz	EEE0
	and	word ptr [235A],FDFF
	mov	ax,0200
	push	ax
	test	word ptr [235A],0200
	jz	EEF1
	mov	ax,0001
	jmp	EEF3
	xor	ax,ax
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	ds
	mov	ax,2358
	push	ax
	call	1299
	add	sp,0C
	mov	al,[2370]
	cbw	
	push	ax
	call	E586
	pop	cx
	or	ax,ax
	jnz	EF16
	and	word ptr [236E],FDFF
	mov	ax,0200
	push	ax
	test	word ptr [236E],0200
	jz	EF27
	mov	ax,0002
	jmp	EF29
	xor	ax,ax
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	ds
	mov	ax,236C
	push	ax
	call	1299
	add	sp,0C
	ret	
	push	bp
	mov	bp,sp
	sub	sp,08
	push	ss
	lea	ax,[bp-08]
	push	ax
	push	ss
	lea	ax,[bp-04]
	push	ax
	les	bx,[bp+04]
	push	word ptr es:[bx+02]
	push	word ptr es:[bx]
	call	1987
	add	sp,0C
	push	ss
	lea	ax,[bp-04]
	push	ax
	call	EE62
	pop	cx
	pop	cx
	push	ss
	lea	ax,[bp-08]
	push	ax
	call	EE77
	pop	cx
	pop	cx
	xor	ax,ax
	mov	sp,bp
	pop	bp
	ret	

fn0800_9764()
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
0800:97B6                   55 8B EC B8 01 00 50 33 C0 50       U.....P3.P
0800:97C0 50 FF 76 04 E8 62 F6 83 C4 08 5D C3             P.v..b....].   

fn0800_97CC()
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

fn0800_97F8()
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
0800:9810 8A C6 E8 02 00 8A C2 D4 10 86 E0 E8 02 00 86 E0 ................
0800:9820 04 90 27 14 40 27 AA C3                         ..'.@'..       

fn0800_9828()
	push	bp
	mov	bp,sp
	sub	sp,0096
	push	si
	push	di
	mov	word ptr [bp-12],0000
	mov	word ptr [bp-14],0050
	mov	word ptr [bp-16],0000
	jmp	988C
0800:9842       57 B9 FF FF 32 C0 F2 AE F7 D1 49 5F C3      W...2.....I_.

fn0800_984F()
	mov	ss:[di],al
	inc	di
	dec	byte ptr [bp-14]
	jnz	988B

fn0800_9858()
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
	mov	di,[bp-04]
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
0800:9901    80 FD 00 77 F8 83 4E FE 01 EB CF 80 FD 00 77  ...w..N.......w
0800:9910 ED 83 4E FE 02 EB C4                            ..N....        

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
0800:9927                      83 66 FE DF EB 04 83 4E FE        .f.....N.
0800:9930 20 B5 05 EB A6 80 FD 00 77 4D F7 46 FE 02 00 75  .......wM.F...u
0800:9940 29 83 4E FE 08 B5 01 EB 92 E9 36 03 8B 7E 04 36 ).N.......6..~.6
0800:9950 8B 05 83 46 04 02 80 FD 02 73 12 0B C0 79 06 F7 ...F.....s...y..
0800:9960 D8 83 4E FE 02 89 46 F8 B5 03 E9 6E FF 80 FD 04 ..N...F....n....
0800:9970 75 D7 89 46 F6 FE C5 E9 61 FF 80 FD 04 73 CA B5 u..F....a....s..
0800:9980 04 FF 46 F6 E9 54 FF 92 2C 30 98 80 FD 02 77 19 ..F..T..,0....w.
0800:9990 B5 02 87 46 F8 0B C0 7C D1 D1 E0 8B D0 D1 E0 D1 ...F...|........
0800:99A0 E0 03 C2 01 46 F8 E9 32 FF 80 FD 04 75 9B 87 46 ....F..2....u..F
0800:99B0 F6 0B C0 74 B5 D1 E0 8B D0 D1 E0 D1 E0 03 C2 01 ...t............
0800:99C0 46 F6 E9 16 FF 83 4E FE 10 E9 65 FF 81 4E FE 00 F.....N...e..N..
0800:99D0 01 83 66 FE EF E9 59 FF B7 08 EB 0A B7 0A EB 0A ..f...Y.........
0800:99E0 B7 10 B3 E9 02 DA C6 46 F5 00 88 56 FB 33 D2 88 .......F...V.3..
0800:99F0 56 FA 8B 7E 04 36 8B 05 EB 10 B7 0A C6 46 FA 01 V..~.6.......F..
0800:9A00 88 56 FB 8B 7E 04 36 8B 05 99 47 47 89 76 06 F7 .V..~.6...GG.v..
0800:9A10 46 FE 10 00 74 05 36 8B 15 47 47 89 7E 04 8D 7E F...t.6..GG.~..~
0800:9A20 BB 0B C0 75 0D 0B D2 75 09 83 7E F6 00 75 07 E9 ...u...u..~..u..
0800:9A30 63 FE 83 4E FE 04 52 50 16 57 8A C7 98 50 8A 46 c..N..RP.W...P.F
0800:9A40 FA 50 53 E8 41 F3 16 07 8B 56 F6 0B D2 7D 03 E9 .PS.A....V...}..
0800:9A50 F4 00 E9 FF 00 88 56 FB 89 76 06 8D 7E BA 8B 5E ......V..v..~..^
0800:9A60 04 36 FF 37 43 43 89 5E 04 F7 46 FE 20 00 74 10 .6.7CC.^..F. .t.
0800:9A70 36 8B 17 43 43 89 5E 04 16 07 E8 93 FD B0 3A AA 6..CC.^.......:.
0800:9A80 16 07 5A E8 8A FD 36 C6 05 00 C6 46 FA 00 83 66 ..Z...6....F...f
0800:9A90 FE FB 8D 4E BA 2B F9 87 CF 8B 56 F6 3B D1 7F 02 ...N.+....V.;...
0800:9AA0 8B D1 E9 A1 00 89 76 06 88 56 FB 8B 7E 04 36 8B ......v..V..~.6.
0800:9AB0 05 83 46 04 02 16 07 8D 7E BB 32 E4 36 89 05 B9 ..F.....~.2.6...
0800:9AC0 01 00 E9 C4 00 89 76 06 88 56 FB 8B 7E 04 F7 46 ......v..V..~..F
0800:9AD0 FE 20 00 75 0D 36 8B 3D 83 46 04 02 1E 07 0B FF . .u.6.=.F......
0800:9AE0 EB 0B 36 C4 3D 83 46 04 04 8C C0 0B C7 75 05 1E ..6.=.F......u..
0800:9AF0 07 BF FE 25 E8 4B FD 3B 4E F6 76 03 8B 4E F6 E9 ...%.K.;N.v..N..
0800:9B00 87 00 89 76 06 88 56 FB 8B 7E 04 8B 4E F6 0B C9 ...v..V..~..N...
0800:9B10 7D 03 B9 06 00 16 57 51 16 8D 5E BB 53 52 B8 01 }.....WQ..^.SR..
0800:9B20 00 23 46 FE 50 8B 46 FE A9 00 01 74 09 B8 08 00 .#F.P.F....t....
0800:9B30 83 46 04 0A EB 07 83 46 04 08 B8 06 00 50 E8 8B .F.....F.....P..
0800:9B40 07 16 07 8D 7E BB F7 46 FE 08 00 74 18 8B 56 F8 ....~..F...t..V.
0800:9B50 0B D2 7E 11 E8 EB FC 26 80 3D 2D 75 01 49 2B D1 ..~....&.=-u.I+.
0800:9B60 7E 03 89 56 F2 26 80 3D 2D 74 0B 8A 46 F5 0A C0 ~..V.&.=-t..F...
0800:9B70 74 14 4F 26 88 05 83 7E F2 00 7E 0A 8B 4E F6 0B t.O&...~..~..N..
0800:9B80 C9 7D 03 FF 4E F2 E8 B9 FC 8B F7 8B 7E FC 8B 5E .}..N.......~..^
0800:9B90 F8 B8 05 00 23 46 FE 3D 05 00 75 13 8A 66 FB 80 ....#F.=..u..f..
0800:9BA0 FC 6F 75 0D 83 7E F2 00 7F 05 C7 46 F2 01 00 EB .ou..~.....F....
0800:9BB0 1B 80 FC 78 74 05 80 FC 58 75 11 83 4E FE 40 4B ...xt...Xu..N.@K
0800:9BC0 4B 83 6E F2 02 7D 05 C7 46 F2 00 00 03 4E F2 F7 K.n..}..F....N..
0800:9BD0 46 FE 02 00 75 0C EB 06 B0 20 E8 72 FC 4B 3B D9 F...u.... .r.K;.
0800:9BE0 7F F6 F7 46 FE 40 00 74 0B B0 30 E8 61 FC 8A 46 ...F.@.t..0.a..F
0800:9BF0 FB E8 5B FC 8B 56 F2 0B D2 7E 27 2B CA 2B DA 26 ..[..V...~'+.+.&
0800:9C00 8A 04 3C 2D 74 08 3C 20 74 04 3C 2B 75 07 26 AC ..<-t.< t.<+u.&.
0800:9C10 E8 3C FC 49 4B 87 CA E3 07 B0 30 E8 31 FC E2 F9 .<.IK.....0.1...
0800:9C20 87 CA E3 12 2B D9 26 AC 36 88 05 47 FE 4E EC 7F ....+.&.6..G.N..
0800:9C30 03 E8 24 FC E2 F0 0B DB 7E 09 8B CB B0 20 E8 0E ..$.....~.... ..
0800:9C40 FC E2 F9 E9 52 FC 89 76 06 8B 7E 04 F7 46 FE 20 ....R..v..~..F. 
0800:9C50 00 75 0B 36 8B 3D 83 46 04 02 1E 07 EB 07 36 C4 .u.6.=.F......6.
0800:9C60 3D 83 46 04 04 B8 50 00 2A 46 EC 03 46 EE 26 89 =.F...P.*F..F.&.
0800:9C70 05 F7 46 FE 10 00 74 07 47 47 26 C7 05 00 00 E9 ..F...t.GG&.....
0800:9C80 13 FC                                           ..             

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
Code vector at 0800:9CB6 (48 bytes)
	0800:9917
0800:9CB6                   17 99 01 99 4C 99 0C 99 7A 99       ....L...z.
0800:9CC0 87 99 C5 99 CC 99 D1 99 35 99 FA 99 D8 99 DC 99 ........5.......
0800:9CD0 E0 99 55 9A 02 9B A5 9A C5 9A 46 9C 82 9C 82 9C ..U.......F.....
0800:9CE0 82 9C 27 99 2D 99                               ..'.-.         

fn0800_9CE6()
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

fn0800_9D41()
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

fn0800_9DA4()
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

fn0800_9E15()
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

fn0800_9E3E()
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

fn0800_9E75()
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

fn0800_9E9E()
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

fn0800_9F02()
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

fn0800_9F5C()
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
0800:9F7F                                              55                U
0800:9F80 8B EC 33 D2 8B 46 04 EB 09                      ..3..F...      

fn0800_9F89()
	push	bp
	mov	bp,sp
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	mov	cx,ax
	or	cx,dx
	push	si
	push	di
	mov	cs:[9D3B],ds
	jz	9FFD

l0800_9F9F:
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
0800:A006                   53 2E 8B 36 3D 9D 56 2E 8B 36       S..6=.V..6
0800:A010 3F 9D 56 E8 73 FF 83 C4 04 0B D2 75 02 5B C3 1F ?.V.s......u.[..
0800:A020 8E C2 06 1E 53 8B 16 00 00 FC 4A BF 04 00 8B F7 ....S.....J.....
0800:A030 B9 06 00 F3 A5 0B D2 74 37 8C C0 40 8E C0 8C D8 .......t7..@....
0800:A040 40 8E D8 33 FF 8B F7 8B CA 81 F9 00 10 76 03 B9 @..3.........v..
0800:A050 00 10 D1 E1 D1 E1 D1 E1 F3 A5 81 EA 00 10 76 10 ..............v.
0800:A060 8C C0 05 00 10 8E C0 8C D8 05 00 10 8E D8 EB D3 ................
0800:A070 2E 8E 1E 3B 9D E8 FD FD 83 C4 04 5A B8 04 00 C3 ...;.......Z....
0800:A080 2E 3B 1E 37 9D 74 44 8B FB 03 F8 8E C7 8B F1 2B .;.7.tD........+
0800:A090 F0 26 89 36 00 00 26 89 1E 02 00 06 50 8E C3 26 .&.6..&.....P..&
0800:A0A0 A3 00 00 8B D3 03 D1 8E C2 26 83 3E 02 00 00 74 .........&.>...t
0800:A0B0 07 26 89 3E 02 00 EB 05 26 89 3E 08 00 8B F3 E8 .&.>....&.>.....
0800:A0C0 B3 FD 83 C4 04 8B D6 B8 04 00 C3 53 8E C3 26 A3 ...........S..&.
0800:A0D0 00 00 03 D8 53 33 C0 50 E8 FB 00 83 C4 04 5A B8 ....S3.P......Z.
0800:A0E0 04 00 C3 55 8B EC 33 D2 EB 06 55 8B EC 8B 56 0A ...U..3...U...V.
0800:A0F0 8B 46 08 8B 5E 06 56 57 2E 8C 1E 3B 9D 2E 89 16 .F..^.VW...;....
0800:A100 3D 9D 2E A3 3F 9D 0B DB 74 3A 8B C8 0B CA 74 3E =...?...t:....t>
0800:A110 05 13 00 83 D2 00 72 3E F7 C2 F0 FF 75 38 B1 04 ......r>....u8..
0800:A120 D3 E8 D3 E2 0A E2 8E C3 26 8B 0E 00 00 3B C8 72 ........&....;.r
0800:A130 0E 77 07 8B D3 B8 04 00 EB 1F E8 43 FF EB 1A E8 .w.........C....
0800:A140 C4 FE EB 15 52 50 E8 40 FE 83 C4 04 EB 0B 53 50 ....RP.@......SP
0800:A150 E8 22 FD 83 C4 04 33 C0 99 2E 8E 1E 3B 9D 5F 5E ."....3.....;._^
0800:A160 5D C3                                           ].             

fn0800_A162()
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

fn0800_A1D6()
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

fn0800_A215()
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
0800:A2A3          55 8B EC 1E C5 56 08 B4 44 8A 46 06 8B    U....V..D.F..
0800:A2B0 5E 04 8B 4E 0C CD 21 1F 72 0C 83 7E 06 00 75 04 ^..N..!.r..~..u.
0800:A2C0 8B C2 EB 06 EB 04 50 E8 61 EA 5D C3 FF 26 F4 26 ......P.a.]..&.&
0800:A2D0 FF 26 F6 26 FF 26 F8 26 FF 26 FA 26 00 00 8F 06 .&.&.&.&.&.&....
0800:A2E0 A0 26 8F 06 A2 26 8F 06 A4 26 2E 8C 1E DC A2 89 .&...&...&......
0800:A2F0 36 A6 26 89 3E A8 26 FC 8E 06 7B 00 BE 80 00 32 6.&.>.&...{....2
0800:A300 E4 26 AC 40 8C C5 87 D6 93 8B 36 75 00 46 46 B9 .&.@......6u.FF.
0800:A310 01 00 80 3E 7D 00 03 72 11 8E 06 77 00 8B FE B1 ...>}..r...w....
0800:A320 7F 32 C0 F2 AE E3 6E 80 F1 7F 50 8B C1 03 C3 40 .2....n...P....@
0800:A330 25 FE FF 8B FC 2B F8 72 5C 8B E7 06 1F 16 07 51 %....+.r\......Q
0800:A340 49 F3 A4 32 C0 AA 8E DD 87 F2 87 D9 8B C3 8B D0 I..2............
0800:A350 43 E8 19 00 77 07 72 40 E8 12 00 77 F9 3C 20 74 C...w.r@...w.< t
0800:A360 08 3C 0D 74 04 3C 09 75 E8 32 C0 EB E4 0B C0 74 .<.t.<.u.2.....t
0800:A370 07 42 AA 0A C0 75 01 43 86 E0 32 C0 F9 E3 15 AC .B...u.C..2.....
0800:A380 49 2C 22 74 0F 04 22 3C 5C 75 07 80 3C 22 75 02 I,"t.."<\u..<"u.
0800:A390 AC 49 0B F6 C3 E9 AD 5E 59 03 CA 2E 8E 1E DC A2 .I.....^Y.......
0800:A3A0 89 1E 9A 26 43 03 DB 03 DB 8B F4 8B EC 2B EB 72 ...&C........+.r
0800:A3B0 E4 8B E5 89 2E 9C 26 8C 16 9E 26 E3 11 89 76 00 ......&...&...v.
0800:A3C0 8C 56 02 83 C5 04 36 AC 0A C0 E0 FA 74 ED 33 C0 .V....6.....t.3.
0800:A3D0 89 46 00 89 46 02 2E 8E 1E DC A2 8B 36 A6 26 8B .F..F.......6.&.
0800:A3E0 3E A8 26 FF 36 A4 26 FF 36 A2 26 A1 9A 26 A3 6B >.&.6.&.6.&..&.k
0800:A3F0 00 A1 9E 26 A3 6F 00 A1 9C 26 A3 6D 00 FF 26 A0 ...&.o...&.m..&.
0800:A400 26                                              &              

fn0800_A401()
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

fn0800_A471()
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
0800:A4F0 7F FF 59 59 5D C3 55 8B EC 33 C0 50 FF 76 06 FF ..YY].U..3.P.v..
0800:A500 76 04 E8 37 00 83 C4 06 8B D0 83 FA FF 75 04 8B v..7.........u..
0800:A510 C2 EB 27 83 E2 FE F7 46 08 80 00 75 03 83 CA 01 ..'....F...u....
0800:A520 52 B8 01 00 50 FF 76 06 FF 76 04 E8 0E 00 83 C4 R...P.v..v......
0800:A530 08 8B D0 83 FA FF 74 D7 33 C0 5D C3 55 8B EC 1E ......t.3.].U...
0800:A540 8B 4E 0A B4 43 8A 46 08 C5 56 04 CD 21 1F 72 03 .N..C.F..V..!.r.
0800:A550 91 EB 04 50 E8 D4 E7 5D C3                      ...P...].      

fn0800_A559()
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

fn0800_A57F()
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
0800:A59D                                        55 8B EC              U..
0800:A5A0 83 EC 04 8B 46 04 3B 06 E8 24 72 06 B8 06 00 50 ....F.;..$r....P
0800:A5B0 EB 5B 8B 5E 04 D1 E3 F7 87 EA 24 00 02 74 05 B8 .[.^......$..t..
0800:A5C0 01 00 EB 4C B8 00 44 8B 5E 04 CD 21 72 3E F6 C2 ...L..D.^..!r>..
0800:A5D0 80 75 35 B8 01 42 33 C9 8B D1 CD 21 72 2E 52 50 .u5..B3....!r.RP
0800:A5E0 B8 02 42 33 C9 8B D1 CD 21 89 46 FC 89 56 FE 5A ..B3....!.F..V.Z
0800:A5F0 59 72 19 B8 00 42 CD 21 72 12 3B 56 FE 72 09 77 Yr...B.!r.;V.r.w
0800:A600 05 3B 46 FC 72 02 EB B7 33 C0 EB 04 50 E8 1B E7 .;F.r...3...P...
0800:A610 8B E5 5D C3                                     ..].           

fn0800_A614()
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

fn0800_A6B7()
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
0800:A77D                                        55 8B EC              U..
0800:A780 83 EC 04 56 57 8B 7E 08 33 C9 8B 46 06 8B 56 04 ...VW.~.3..F..V.
0800:A790 89 46 FE 89 56 FC EB 09 C4 5E FC 26 88 0F FF 46 .F..V....^.&...F
0800:A7A0 FC 83 F9 0A 74 32 4F 7E 2F C4 5E 0A 26 FF 0F 7C ....t2O~/.^.&..|
0800:A7B0 15 26 8B 47 0E 26 8B 77 0C 26 FF 47 0C 8E C0 26 .&.G.&.w.&.G...&
0800:A7C0 8A 04 B4 00 EB 0B FF 76 0C FF 76 0A E8 F3 06 59 .......v..v....Y
0800:A7D0 59 8B C8 3D FF FF 75 C0 83 F9 FF 75 16 8B 46 FE Y..=..u....u..F.
0800:A7E0 8B 56 FC 3B 46 06 75 0B 3B 56 04 75 06 33 D2 33 .V.;F.u.;V.u.3.3
0800:A7F0 C0 EB 1E C4 5E FC 26 C6 07 00 C4 5E 0A 26 F7 47 ....^.&....^.&.G
0800:A800 02 10 00 74 06 33 D2 33 C0 EB 06 8B 56 06 8B 46 ...t.3.3....V..F
0800:A810 04 5F 5E 8B E5 5D C3 55 8B EC 1E B4 2F CD 21 06 ._^..].U..../.!.
0800:A820 53 B4 1A C5 56 08 CD 21 B4 4E 8B 4E 0C C5 56 04 S...V..!.N.N..V.
0800:A830 CD 21 9C 59 93 B4 1A 5A 1F CD 21 51 9D 1F 72 04 .!.Y...Z..!Q..r.
0800:A840 33 C0 EB 04 53 E8 E3 E4 5D C3 55 8B EC 1E B4 2F 3...S...].U..../
0800:A850 CD 21 06 53 B4 1A C5 56 04 CD 21 B4 4F CD 21 9C .!.S...V..!.O.!.
0800:A860 59 93 B4 1A 5A 1F CD 21 51 9D 1F 72 04 33 C0 EB Y...Z..!Q..r.3..
0800:A870 04 53 E8 B6 E4 5D C3                            .S...].        

fn0800_A877()
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
0800:A8B7                      55 8B EC 56 57 33 FF C4 5E        U..VW3..^
0800:A8C0 0C FF 46 0C 26 8A 0F 8A C1 3C 72 75 08 BA 01 00 ..F.&....<ru....
0800:A8D0 BE 01 00 EB 1E 80 F9 77 75 05 BA 02 03 EB 08 80 .......wu.......
0800:A8E0 F9 61 75 0B BA 02 09 BF 80 00 BE 02 00 EB 04 33 .au............3
0800:A8F0 C0 EB 74 C4 5E 0C 26 8A 0F FF 46 0C 80 F9 2B 74 ..t.^.&...F...+t
0800:A900 13 C4 5E 0C 26 80 3F 2B 75 21 80 F9 74 74 05 80 ..^.&.?+u!..tt..
0800:A910 F9 62 75 17 80 F9 2B 75 06 C4 5E 0C 26 8A 0F 83 .bu...+u..^.&...
0800:A920 E2 FC 83 CA 04 BF 80 01 BE 03 00 80 F9 74 75 06 .............tu.
0800:A930 81 CA 00 40 EB 1D 80 F9 62 75 06 81 CA 00 80 EB ...@....bu......
0800:A940 0F A1 12 25 25 00 C0 0B D0 8B C2 A9 00 80 74 03 ...%%.........t.
0800:A950 83 CE 40 C7 06 54 23 B5 C7 C4 5E 08 26 89 17 C4 ..@..T#...^.&...
0800:A960 5E 04 26 89 3F 8B C6 5F 5E 5D C2 0C 00 55 8B EC ^.&.?.._^]...U..
0800:A970 83 EC 04 FF 76 08 FF 76 06 16 8D 46 FE 50 16 8D ....v..v...F.P..
0800:A980 46 FC 50 E8 31 FF C4 5E 0E 26 89 47 02 0B C0 74 F.P.1..^.&.G...t
0800:A990 28 26 80 7F 04 00 7D 35 FF 76 FC 8B 46 FE 0B 46 (&....}5.v..F..F
0800:A9A0 04 50 FF 76 0C FF 76 0A E8 95 07 83 C4 08 C4 5E .P.v..v........^
0800:A9B0 0E 26 88 47 04 0A C0 7D 14 C4 5E 0E 26 C6 47 04 .&.G...}..^.&.G.
0800:A9C0 FF 26 C7 47 02 00 00 33 D2 33 C0 EB 61 C4 5E 0E .&.G...3.3..a.^.
0800:A9D0 26 8A 47 04 98 50 E8 9D E3 59 0B C0 74 09 C4 5E &.G..P...Y..t..^
0800:A9E0 0E 26 81 4F 02 00 02 B8 00 02 50 C4 5E 0E 26 F7 .&.O......P.^.&.
0800:A9F0 47 02 00 02 74 05 B8 01 00 EB 02 33 C0 50 33 C0 G...t......3.P3.
0800:AA00 50 50 FF 76 10 FF 76 0E E8 7E 10 83 C4 0C 0B C0 PP.v..v..~......
0800:AA10 74 0D FF 76 10 FF 76 0E E8 F9 FB 59 59 EB A8 C4 t..v..v....YY...
0800:AA20 5E 0E 26 C7 47 10 00 00 8B 56 10 8B 46 0E 8B E5 ^.&.G....V..F...
0800:AA30 5D C2 0E 00 55 8B EC 83 EC 04 8C 5E FE C7 46 FC ]...U......^..F.
0800:AA40 58 23 C4 5E FC 26 80 7F 04 00 7C 18 8B 46 FC 83 X#.^.&....|..F..
0800:AA50 46 FC 14 50 A1 E8 24 BA 14 00 F7 EA 05 58 23 5A F..P..$......X#Z
0800:AA60 3B D0 72 DE C4 5E FC 26 80 7F 04 00 7C 06 33 D2 ;.r..^.&....|.3.
0800:AA70 33 C0 EB 06 8B 56 FE 8B 46 FC 8B E5 5D C3 55 8B 3....V..F...].U.
0800:AA80 EC 83 EC 04 E8 AD FF 89 56 FE 89 46 FC 0B C2 75 ........V..F...u
0800:AA90 06 33 D2 33 C0 EB 18 FF 76 FE FF 76 FC FF 76 06 .3.3....v..v..v.
0800:AAA0 FF 76 04 FF 76 0A FF 76 08 33 C0 50 E8 BE FE 8B .v..v..v.3.P....
0800:AAB0 E5 5D C3 55 8B EC 56 57 E9 D6 00 FF 46 08 C4 5E .].U..VW....F..^
0800:AAC0 04 26 8B 47 06 3B 46 08 76 05 8B 46 08 EB 07 C4 .&.G.;F.v..F....
0800:AAD0 5E 04 26 8B 47 06 8B F8 C4 5E 04 26 F7 47 02 40 ^.&.G....^.&.G.@
0800:AAE0 00 74 6D 26 83 7F 06 00 74 66 26 8B 47 06 3B 46 .tm&....tf&.G.;F
0800:AAF0 08 73 5D 26 83 3F 00 75 57 FF 4E 08 33 FF EB 0E .s]&.?.uW.N.3...
0800:AB00 C4 5E 04 26 03 7F 06 26 8B 47 06 29 46 08 C4 5E .^.&...&.G.)F..^
0800:AB10 04 26 8B 47 06 3B 46 08 76 E6 57 FF 76 0C FF 76 .&.G.;F.v.W.v..v
0800:AB20 0A 26 8A 47 04 98 50 E8 26 E4 83 C4 08 8B D0 01 .&.G..P.&.......
0800:AB30 56 0A 3B D7 74 5B 8B C7 2B C2 01 46 08 C4 5E 04 V.;.t[..+..F..^.
0800:AB40 26 83 4F 02 20 EB 53 C4 5E 0A 26 88 17 FF 46 0A &.O. .S.^.&...F.
0800:AB50 FF 4E 08 8B 46 08 0B C0 74 32 4F 74 2F C4 5E 04 .N..F...t2Ot/.^.
0800:AB60 26 FF 0F 7C 15 26 8B 47 0E 26 8B 77 0C 26 FF 47 &..|.&.G.&.w.&.G
0800:AB70 0C 8E C0 26 8A 04 B4 00 EB 0B FF 76 06 FF 76 04 ...&.......v..v.
0800:AB80 E8 3F 03 59 59 8B D0 3D FF FF 75 BB 83 FA FF 74 .?.YY..=..u....t
0800:AB90 AC 83 7E 08 00 74 03 E9 21 FF 8B 46 08 5F 5E 5D ..~..t..!..F._^]
0800:ABA0 C2 0A 00 55 8B EC 83 EC 04 56 57 8B 7E 08 0B FF ...U.....VW.~...
0800:ABB0 75 04 33 C0 EB 75 8B DF 33 C9 8B 46 0A 33 D2 E8 u.3..u..3..F.3..
0800:ABC0 56 E3 89 56 FE 89 46 FC 83 FA 01 77 25 72 04 0B V..V..F....w%r..
0800:ABD0 C0 73 1F FF 76 06 FF 76 04 FF 76 FC FF 76 0E FF .s..v..v..v..v..
0800:ABE0 76 0C E8 CE FE 8B 56 FC 2B D0 52 33 D2 58 F7 F7 v.....V.+.R3.X..
0800:ABF0 EB 39 8B 76 0A 46 EB 13 8B DF 33 C9 8B 56 06 8B .9.v.F....3..V..
0800:AC00 46 04 E8 C6 E0 89 56 06 89 46 04 4E 8B C6 0B C0 F.....V..F.N....
0800:AC10 74 14 FF 76 06 FF 76 04 57 FF 76 0E FF 76 0C E8 t..v..v.W.v..v..
0800:AC20 91 FE 0B C0 74 D2 8B 46 0A 2B C6 5F 5E 8B E5 5D ....t..F.+._^..]
0800:AC30 C3 55 8B EC 83 EC 04 56 C4 5E 04 26 83 3F 00 7D .U.....V.^.&.?.}
0800:AC40 0C 26 8B 4F 06 26 03 0F 41 8B F1 EB 0F C4 5E 04 .&.O.&..A.....^.
0800:AC50 26 8B 07 99 33 C2 2B C2 8B C8 8B F0 C4 5E 04 26 &...3.+......^.&
0800:AC60 F7 47 02 40 00 75 43 C4 5E 04 26 8B 47 0E 26 8B .G.@.uC.^.&.G.&.
0800:AC70 57 0C 89 46 FE 89 56 FC 26 83 3F 00 7D 25 EB 0D W..F..V.&.?.}%..
0800:AC80 FF 4E FC C4 5E FC 26 80 3F 0A 75 01 46 8B C1 49 .N..^.&.?.u.F..I
0800:AC90 0B C0 75 EC EB 14 C4 5E FC FF 46 FC 26 80 3F 0A ..u....^..F.&.?.
0800:ACA0 75 01 46 8B C1 49 0B C0 75 EC 8B C6 5E 8B E5 5D u.F..I..u...^..]
0800:ACB0 C2 04 00 55 8B EC 56 8B 76 0C FF 76 06 FF 76 04 ...U..V.v..v..v.
0800:ACC0 E8 F4 F9 59 59 0B C0 74 05 B8 FF FF EB 5E 83 FE ...YY..t.....^..
0800:ACD0 01 75 17 C4 5E 04 26 83 3F 00 7E 0E FF 76 06 53 .u..^.&.?.~..v.S
0800:ACE0 E8 4E FF 99 29 46 08 19 56 0A C4 5E 04 26 81 67 .N..)F..V..^.&.g
0800:ACF0 02 5F FE 26 C7 07 00 00 26 8B 47 0A 26 8B 57 08 ._.&....&.G.&.W.
0800:AD00 26 89 47 0E 26 89 57 0C 56 FF 76 0A FF 76 08 26 &.G.&.W.V.v..v.&
0800:AD10 8A 47 04 98 50 E8 11 E1 83 C4 08 83 FA FF 75 0A .G..P.........u.
0800:AD20 3D FF FF 75 05 B8 FF FF EB 02 33 C0 5E 5D C3 55 =..u......3.^].U
0800:AD30 8B EC 83 EC 04 C4 5E 04 26 8A 47 04 98 50 E8 75 ......^.&.G..P.u
0800:AD40 EA 59 89 56 FE 89 46 FC 83 FA FF 75 05 3D FF FF .Y.V..F....u.=..
0800:AD50 74 29 C4 5E 04 26 83 3F 00 7D 10 FF 76 06 53 E8 t).^.&.?.}..v.S.
0800:AD60 CF FE 99 01 46 FC 11 56 FE EB 10 FF 76 06 FF 76 ....F..V....v..v
0800:AD70 04 E8 BD FE 99 29 46 FC 19 56 FE 8B 56 FE 8B 46 .....)F..V..V..F
0800:AD80 FC 8B E5 5D C3 55 8B EC 83 EC 04 56 57 8B 7E 08 ...].U.....VW.~.
0800:AD90 0B FF 74 73 8B DF 33 C9 8B 46 0A 33 D2 E8 78 E1 ..ts..3..F.3..x.
0800:ADA0 89 56 FE 89 46 FC 83 FA 01 77 1E 72 04 0B C0 73 .V..F....w.r...s
0800:ADB0 18 FF 76 06 FF 76 04 FF 76 FC FF 76 0E FF 76 0C ..v..v..v..v..v.
0800:ADC0 E8 FB 06 33 D2 F7 F7 EB 41 33 F6 3B 76 0A 73 37 ...3....A3.;v.s7
0800:ADD0 FF 76 06 FF 76 04 57 FF 76 0E FF 76 0C E8 DE 06 .v..v.W.v..v....
0800:ADE0 33 D2 0B D2 75 04 3B C7 74 04 8B C6 EB 1C 8B DF 3...u.;.t.......
0800:ADF0 33 C9 8B 56 06 8B 46 04 E8 D0 DE 89 56 06 89 46 3..V..F.....V..F
0800:AE00 04 46 3B 76 0A 72 C9 8B 46 0A 5F 5E 8B E5 5D C3 .F;v.r..F._^..].
0800:AE10 55 8B EC 83 EC 04 56 BE 14 00 8C 5E FE C7 46 FC U.....V....^..F.
0800:AE20 58 23 EB 1C C4 5E FC 26 8B 47 02 25 00 03 3D 00 X#...^.&.G.%..=.
0800:AE30 03 75 09 FF 76 FE 53 E8 7D F8 59 59 83 46 FC 14 .u..v.S.}.YY.F..
0800:AE40 8B C6 4E 0B C0 75 DD 5E 8B E5 5D C3 55 8B EC C4 ..N..u.^..].U...
0800:AE50 5E 04 26 F7 47 02 00 02 74 03 E8 B3 FF C4 5E 04 ^.&.G...t.....^.
0800:AE60 26 FF 77 06 26 8B 47 0A 26 8B 57 08 26 89 47 0E &.w.&.G.&.W.&.G.
0800:AE70 26 89 57 0C 50 52 26 8A 47 04 98 50 E8 00 0B 83 &.W.PR&.G..P....
0800:AE80 C4 08 C4 5E 04 26 89 07 0B C0 7E 09 26 83 67 02 ...^.&....~.&.g.
0800:AE90 DF 33 C0 EB 29 C4 5E 04 26 83 3F 00 75 10 26 8B .3..).^.&.?.u.&.
0800:AEA0 47 02 25 7F FE 0D 20 00 26 89 47 02 EB 0D C4 5E G.%... .&.G....^
0800:AEB0 04 26 C7 07 00 00 26 83 4F 02 10 B8 FF FF 5D C2 .&....&.O.....].
0800:AEC0 04 00 55 8B EC C4 5E 04 26 FF 07 FF 76 06 53 E8 ..U...^.&...v.S.
0800:AED0 04 00 59 59 5D C3 55 8B EC 56 8B 46 04 0B 46 06 ..YY].U..V.F..F.
0800:AEE0 75 06 B8 FF FF E9 D5 00 C4 5E 04 26 83 3F 00 7E u........^.&.?.~
0800:AEF0 1A C4 5E 04 26 FF 0F 26 8B 47 0E 26 8B 77 0C 26 ..^.&..&.G.&.w.&
0800:AF00 FF 47 0C 8E C0 26 8A 04 E9 B0 00 C4 5E 04 26 83 .G...&......^.&.
0800:AF10 3F 00 7C 6B 26 F7 47 02 10 01 75 63 26 F7 47 02 ?.|k&.G...uc&.G.
0800:AF20 01 00 74 5B C4 5E 04 26 81 4F 02 80 00 26 83 7F ..t[.^.&.O...&..
0800:AF30 06 00 74 0F FF 76 06 53 E8 11 FF 0B C0 74 B2 EB ..t..v.S.....t..
0800:AF40 A1 EB AE C4 5E 04 26 F7 47 02 00 02 74 03 E8 BF ....^.&.G...t...
0800:AF50 FE B8 01 00 50 1E B8 E4 4E 50 C4 5E 04 26 8A 47 ....P...NP.^.&.G
0800:AF60 04 98 50 E8 19 0A 83 C4 08 0B C0 75 31 C4 5E 04 ..P........u1.^.
0800:AF70 26 8A 47 04 98 50 E8 24 F6 59 3D 01 00 74 0B C4 &.G..P.$.Y=..t..
0800:AF80 5E 04 26 83 4F 02 10 E9 58 FF C4 5E 04 26 8B 47 ^.&.O...X..^.&.G
0800:AF90 02 25 7F FE 0D 20 00 26 89 47 02 E9 44 FF 80 3E .%... .&.G..D..>
0800:AFA0 E4 4E 0D 75 0B C4 5E 04 26 F7 47 02 40 00 74 93 .N.u..^.&.G.@.t.
0800:AFB0 C4 5E 04 26 83 67 02 DF A0 E4 4E B4 00 5E 5D C3 .^.&.g....N..^].
0800:AFC0 1E B8 58 23 50 E8 0E FF 59 59 C3                ..X#P...YY.    

fn0800_AFCB()
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
	repne	
	scasb	

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
	rep	
	cmpsb	

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

fn0800_B03B()
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
	rep	
	movsw	

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

fn0800_B05F()
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
	rep	
	stosw	

l0800_B07D:
	jnc	B080

l0800_B07F:
	stosb	

l0800_B080:
	pop	di
	pop	bp
	ret	

fn0800_B083()
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

fn0800_B0A1()
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
	rep	
	movsw	

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

fn0800_B0F3()
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
0800:B113          55 8B EC 1E 8B 4E 04 B4 3C C5 56 06 CD    U....N..<.V..
0800:B120 21 1F 72 02 EB 04 50 E8 01 DC 5D C2 06 00 55 8B !.r...P...]...U.
0800:B130 EC 8B 5E 04 2B C9 2B D2 B4 40 CD 21 5D C2 02 00 ..^.+.+..@.!]...
0800:B140 55 8B EC 83 EC 04 56 57 8B 76 08 8B 7E 0A F7 C6 U.....VW.v..~...
0800:B150 00 C0 75 08 A1 12 25 25 00 C0 0B F0 33 C0 50 FF ..u...%%....3.P.
0800:B160 76 06 FF 76 04 E8 D4 F3 83 C4 06 89 46 FE F7 C6 v..v........F...
0800:B170 00 01 74 7E 23 3E 14 25 8B C7 A9 80 01 75 07 B8 ..t~#>.%.....u..
0800:B180 01 00 50 E8 A5 DB 83 7E FE FF 75 23 83 3E 16 25 ..P....~..u#.>.%
0800:B190 02 74 0A FF 36 16 25 E8 91 DB E9 FD 00 F7 C7 80 .t..6.%.........
0800:B1A0 00 74 04 33 C0 EB 03 B8 01 00 89 46 FE EB 0C F7 .t.3.......F....
0800:B1B0 C6 00 04 74 3D B8 50 00 50 EB DC F7 C6 F0 00 74 ...t=.P.P......t
0800:B1C0 1C FF 76 06 FF 76 04 33 C0 50 E8 46 FF 8B F8 0B ..v..v.3.P.F....
0800:B1D0 C0 7D 03 E9 C2 00 57 E8 A5 F3 59 EB 15 FF 76 06 .}....W...Y...v.
0800:B1E0 FF 76 04 FF 76 FE E8 2A FF 8B F8 0B C0 7D 76 E9 .v..v..*.....}v.
0800:B1F0 A6 00 56 FF 76 06 FF 76 04 E8 A4 00 83 C4 06 8B ..V.v..v........
0800:B200 F8 0B C0 7C 60 33 C0 50 57 E8 97 F0 59 59 89 46 ...|`3.PW...YY.F
0800:B210 FC A9 80 00 74 21 81 CE 00 20 F7 C6 00 80 74 21 ....t!... ....t!
0800:B220 25 FF 00 0D 20 00 33 D2 52 50 B8 01 00 50 57 E8 %... .3.RP...PW.
0800:B230 71 F0 83 C4 08 EB 0A F7 C6 00 02 74 04 57 E8 ED q..........t.W..
0800:B240 FE F7 46 FE 01 00 74 1D F7 C6 00 01 74 17 F7 C6 ..F...t.....t...
0800:B250 F0 00 74 11 B8 01 00 50 50 FF 76 06 FF 76 04 E8 ..t....PP.v..v..
0800:B260 DA F2 83 C4 08 0B FF 7C 2F F7 C6 00 03 74 05 B8 .......|/....t..
0800:B270 00 10 EB 02 33 C0 8B D6 81 E2 FF F8 0B D0 52 F7 ....3.........R.
0800:B280 46 FE 01 00 74 04 33 C0 EB 03 B8 00 01 5A 0B D0 F...t.3......Z..
0800:B290 8B DF D1 E3 89 97 EA 24 8B C7 5F 5E 8B E5 5D C3 .......$.._^..].
0800:B2A0 55 8B EC 83 EC 02 B0 01 8B 4E 08 F7 C1 02 00 75 U........N.....u
0800:B2B0 0A B0 02 F7 C1 04 00 75 02 B0 00 1E C5 56 04 B1 .......u.....V..
0800:B2C0 F0 22 4E 08 0A C1 B4 3D CD 21 1F 72 1A 89 46 FE ."N....=.!.r..F.
0800:B2D0 8B 46 08 25 FF B8 0D 00 80 8B 5E FE D1 E3 89 87 .F.%......^.....
0800:B2E0 EA 24 8B 46 FE EB 04 50 E8 40 DA 8B E5 5D C3    .$.F...P.@...].

fn0800_B2EF()
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
0800:B30A                               55 8B EC C4 5E 06           U...^.
0800:B310 26 FF 0F FF 76 08 53 8A 46 04 98 50 E8 05 00 83 &...v.S.F..P....
0800:B320 C4 06 5D C3 55 8B EC 56 8A 46 04 A2 E6 4E C4 5E ..].U..V.F...N.^
0800:B330 06 26 83 3F FF 7D 52 26 FF 07 26 8B 47 0E 26 8B .&.?.}R&..&.G.&.
0800:B340 77 0C 26 FF 47 0C 8A 16 E6 4E 8E C0 26 88 14 8E w.&.G....N..&...
0800:B350 46 08 26 F7 47 02 08 00 75 03 E9 46 01 80 3E E6 F.&.G...u..F..>.
0800:B360 4E 0A 74 0A 80 3E E6 4E 0D 74 03 E9 35 01 FF 76 N.t..>.N.t..5..v
0800:B370 08 FF 76 06 E8 40 F3 59 59 0B C0 75 03 E9 23 01 ..v..@.YY..u..#.
0800:B380 B8 FF FF E9 22 01 E9 1A 01 C4 5E 06 26 F7 47 02 ....".....^.&.G.
0800:B390 90 00 75 08 26 F7 47 02 02 00 75 0A C4 5E 06 26 ..u.&.G...u..^.&
0800:B3A0 83 4F 02 10 EB DA C4 5E 06 26 81 4F 02 00 01 26 .O.....^.&.O...&
0800:B3B0 83 7F 06 00 74 6B 26 83 3F 00 74 0D FF 76 08 53 ....tk&.?.t..v.S
0800:B3C0 E8 F4 F2 59 59 0B C0 75 B7 C4 5E 06 26 8B 47 06 ...YY..u..^.&.G.
0800:B3D0 F7 D8 26 89 07 26 8B 47 0E 26 8B 77 0C 26 FF 47 ..&..&.G.&.w.&.G
0800:B3E0 0C 8A 16 E6 4E 8E C0 26 88 14 8E 46 08 26 F7 47 ....N..&...F.&.G
0800:B3F0 02 08 00 75 03 E9 AB 00 80 3E E6 4E 0A 74 0A 80 ...u.....>.N.t..
0800:B400 3E E6 4E 0D 74 03 E9 9A 00 FF 76 08 FF 76 06 E8 >.N.t.....v..v..
0800:B410 A5 F2 59 59 0B C0 75 03 E9 88 00 E9 62 FF E9 82 ..YY..u.....b...
0800:B420 00 C4 5E 06 26 8A 47 04 98 D1 E0 8B D8 F7 87 EA ..^.&.G.........
0800:B430 24 00 08 74 17 B8 02 00 50 33 C0 50 50 8B 5E 06 $..t....P3.PP.^.
0800:B440 26 8A 47 04 98 50 E8 E0 D9 83 C4 08 80 3E E6 4E &.G..P.......>.N
0800:B450 0A 75 25 C4 5E 06 26 F7 47 02 40 00 75 1A B8 01 .u%.^.&.G.@.u...
0800:B460 00 50 1E B8 AE 26 50 26 8A 47 04 98 50 E8 09 13 .P...&P&.G..P...
0800:B470 83 C4 08 3D 01 00 75 1D B8 01 00 50 1E B8 E6 4E ...=..u....P...N
0800:B480 50 C4 5E 06 26 8A 47 04 98 50 E8 EC 12 83 C4 08 P.^.&.G..P......
0800:B490 3D 01 00 74 0E C4 5E 06 26 F7 47 02 00 02 75 03 =..t..^.&.G...u.
0800:B4A0 E9 F9 FE A0 E6 4E B4 00 5E 5D C3 55 8B EC 1E B8 .....N..^].U....
0800:B4B0 6C 23 50 FF 76 04 E8 6B FE 83 C4 06 5D C3 55 8B l#P.v..k....].U.
0800:B4C0 EC 83 EC 02 56 57 8B 7E 08 89 7E FE C4 5E 04 26 ....VW.~..~..^.&
0800:B4D0 F7 47 02 08 00 74 2D EB 21 FF 76 06 FF 76 04 C4 .G...t-.!.v..v..
0800:B4E0 5E 0A FF 46 0A 26 8A 07 98 50 E8 37 FE 83 C4 06 ^..F.&...P.7....
0800:B4F0 3D FF FF 75 05 33 C0 E9 A6 01 8B C7 4F 0B C0 75 =..u.3......O..u
0800:B500 D8 E9 99 01 C4 5E 04 26 F7 47 02 40 00 75 03 E9 .....^.&.G.@.u..
0800:B510 13 01 26 83 7F 06 00 75 03 E9 BF 00 26 39 7F 06 ..&....u....&9..
0800:B520 73 61 26 83 3F 00 74 0D FF 76 06 53 E8 88 F1 59 sa&.?.t..v.S...Y
0800:B530 59 0B C0 75 C0 C4 5E 04 26 8A 47 04 98 D1 E0 8B Y..u..^.&.G.....
0800:B540 D8 F7 87 EA 24 00 08 74 17 B8 02 00 50 33 C0 50 ....$..t....P3.P
0800:B550 50 8B 5E 04 26 8A 47 04 98 50 E8 CC D8 83 C4 08 P.^.&.G..P......
0800:B560 57 FF 76 0C FF 76 0A C4 5E 04 26 8A 47 04 98 50 W.v..v..^.&.G..P
0800:B570 E8 06 12 83 C4 08 3B C7 72 03 E9 20 01 E9 75 FF ......;.r.. ..u.
0800:B580 E9 1A 01 C4 5E 04 26 8B 07 03 C7 7C 24 26 83 3F ....^.&....|$&.?
0800:B590 00 75 0C B8 FF FF 26 2B 47 06 26 89 07 EB 12 FF .u....&+G.&.....
0800:B5A0 76 06 FF 76 04 E8 0F F1 59 59 0B C0 74 03 E9 44 v..v....YY..t..D
0800:B5B0 FF 57 FF 76 0C FF 76 0A C4 5E 04 26 FF 77 0E 26 .W.v..v..^.&.w.&
0800:B5C0 FF 77 0C E8 75 FA 83 C4 0A C4 5E 04 26 8B 07 03 .w..u.....^.&...
0800:B5D0 C7 26 89 07 26 01 7F 0C E9 C2 00 C4 5E 04 26 8A .&..&.......^.&.
0800:B5E0 47 04 98 D1 E0 8B D8 F7 87 EA 24 00 08 74 17 B8 G.........$..t..
0800:B5F0 02 00 50 33 C0 50 50 8B 5E 04 26 8A 47 04 98 50 ..P3.PP.^.&.G..P
0800:B600 E8 26 D8 83 C4 08 57 FF 76 0C FF 76 0A C4 5E 04 .&....W.v..v..^.
0800:B610 26 8A 47 04 98 50 E8 60 11 83 C4 08 3B C7 73 7D &.G..P.`....;.s}
0800:B620 E9 D2 FE EB 78 C4 5E 04 26 83 7F 06 00 74 51 EB ....x.^.&....tQ.
0800:B630 46 C4 5E 04 26 FF 07 7D 20 26 8B 47 0E 26 8B 77 F.^.&..} &.G.&.w
0800:B640 0C 26 FF 47 0C C4 5E 0A FF 46 0A 26 8A 17 8E C0 .&.G..^..F.&....
0800:B650 26 88 14 8A C2 B4 00 EB 16 FF 76 06 FF 76 04 C4 &.........v..v..
0800:B660 5E 0A FF 46 0A 26 8A 07 50 E8 9E FC 83 C4 06 3D ^..F.&..P......=
0800:B670 FF FF 75 03 E9 7E FE 8B C7 4F 0B C0 75 B3 EB 1D ..u..~...O..u...
0800:B680 57 FF 76 0C FF 76 0A C4 5E 04 26 8A 47 04 98 50 W.v..v..^.&.G..P
0800:B690 E8 9F 0F 83 C4 08 3B C7 73 03 E9 58 FE 8B 46 FE ......;.s..X..F.
0800:B6A0 5F 5E 8B E5 5D C2 0A 00 55 8B EC 56 57 1E FC 8B _^..]...U..VW...
0800:B6B0 0E E8 4E C4 7E 04 C5 76 08 D1 E9 73 09 26 8A 05 ..N.~..v...s.&..
0800:B6C0 A4 88 44 FF 74 09 26 8B 05 A5 89 44 FE E2 F7 1F ..D.t.&....D....
0800:B6D0 5F 5E 5D C2 08 00 55 8B EC 83 EC 14 56 57 8B 76 _^]...U.....VW.v
0800:B6E0 04 83 FE 02 77 40 83 FE 02 74 03 E9 68 02 8B 46 ....w@...t..h..F
0800:B6F0 08 8B 56 06 03 16 E8 4E 89 46 FA 89 56 F8 50 52 ..V....N.F..V.PR
0800:B700 FF 76 08 FF 76 06 FF 16 EA 4E 83 C4 08 0B C0 7F .v..v....N......
0800:B710 03 E9 42 02 FF 76 08 FF 76 06 FF 76 FA FF 76 F8 ..B..v..v..v..v.
0800:B720 E8 85 FF E9 30 02 8B C6 48 F7 2E E8 4E 8B 56 08 ....0...H...N.V.
0800:B730 8B 5E 06 03 D8 89 56 FA 89 5E F8 8B C6 D1 E8 F7 .^....V..^......
0800:B740 2E E8 4E 8B 56 08 8B 5E 06 03 D8 89 56 FE 89 5E ..N.V..^....V..^
0800:B750 FC FF 76 FA FF 76 F8 FF 76 FE 53 FF 16 EA 4E 83 ..v..v..v.S...N.
0800:B760 C4 08 0B C0 7E 0F FF 76 FE FF 76 FC FF 76 FA FF ....~..v..v..v..
0800:B770 76 F8 E8 33 FF FF 76 08 FF 76 06 FF 76 FE FF 76 v..3..v..v..v..v
0800:B780 FC FF 16 EA 4E 83 C4 08 0B C0 7E 0E FF 76 FE FF ....N.....~..v..
0800:B790 76 FC FF 76 08 FF 76 06 EB 23 FF 76 FA FF 76 F8 v..v..v..#.v..v.
0800:B7A0 FF 76 08 FF 76 06 FF 16 EA 4E 83 C4 08 0B C0 7E .v..v....N.....~
0800:B7B0 0F FF 76 08 FF 76 06 FF 76 FA FF 76 F8 E8 E8 FE ..v..v..v..v....
0800:B7C0 83 FE 03 75 0F FF 76 08 FF 76 06 FF 76 FE FF 76 ...u..v..v..v..v
0800:B7D0 FC E9 4C FF 8B 46 08 8B 56 06 03 16 E8 4E 89 46 ..L..F..V....N.F
0800:B7E0 F6 89 56 F4 89 46 FE 89 56 FC EB 27 0B FF 75 15 ..V..F..V..'..u.
0800:B7F0 FF 76 FE FF 76 FC FF 76 F6 FF 76 F4 E8 A9 FE A1 .v..v..v..v.....
0800:B800 E8 4E 01 46 F4 8B 46 FC 3B 46 F8 73 76 A1 E8 4E .N.F..F.;F.sv..N
0800:B810 01 46 FC FF 76 08 FF 76 06 FF 76 FE FF 76 FC FF .F..v..v..v..v..
0800:B820 16 EA 4E 83 C4 08 8B F8 0B C0 7E C0 8B 46 FC 3B ..N.......~..F.;
0800:B830 46 F8 73 47 FF 76 FA FF 76 F8 FF 76 08 FF 76 06 F.sG.v..v..v..v.
0800:B840 FF 16 EA 4E 83 C4 08 8B F8 0B C0 7D 08 A1 E8 4E ...N.......}...N
0800:B850 29 46 F8 EB 1E FF 76 FE FF 76 FC FF 76 FA FF 76 )F....v..v..v..v
0800:B860 F8 E8 44 FE 0B FF 74 13 A1 E8 4E 01 46 FC 29 46 ..D...t...N.F.)F
0800:B870 F8 EB 08 8B 46 FC 3B 46 F8 72 B9 8B 46 FC 3B 46 ....F.;F.r..F.;F
0800:B880 F8 72 90 FF 76 08 FF 76 06 FF 76 FE FF 76 FC FF .r..v..v..v..v..
0800:B890 16 EA 4E 83 C4 08 0B C0 7F 10 8B 46 FE 8B 56 FC ..N........F..V.
0800:B8A0 03 16 E8 4E 89 46 FE 89 56 FC 8B 46 FE 8B 56 FC ...N.F..V..F..V.
0800:B8B0 2B 16 E8 4E 89 46 EE 89 56 EC 8B 46 08 8B 56 06 +..N.F..V..F..V.
0800:B8C0 89 46 F2 89 56 F0 EB 18 FF 76 F2 FF 76 F0 FF 76 .F..V....v..v..v
0800:B8D0 EE FF 76 EC E8 D1 FD A1 E8 4E 01 46 F0 29 46 EC ..v......N.F.)F.
0800:B8E0 8B 46 F0 3B 46 F4 73 08 8B 46 EC 3B 46 F4 73 D8 .F.;F.s..F.;F.s.
0800:B8F0 33 C0 50 FF 36 E8 4E 8B 46 FC 33 D2 2B 46 F4 83 3.P.6.N.F.3.+F..
0800:B900 DA 00 52 50 E8 B4 D2 8B F8 33 C0 50 FF 36 E8 4E ..RP.....3.P.6.N
0800:B910 8B C6 F7 2E E8 4E 8B 56 06 03 D0 33 C0 2B 56 FC .....N.V...3.+V.
0800:B920 1D 00 00 50 52 E8 93 D2 8B F0 3B F7 73 0F FF 76 ...PR.....;.s..v
0800:B930 FE FF 76 FC 50 E8 9E FD 8B F7 E9 A4 FD FF 76 08 ..v.P.........v.
0800:B940 FF 76 06 57 E8 8F FD 8B 46 FE 8B 56 FC 89 46 08 .v.W....F..V..F.
0800:B950 89 56 06 E9 8B FD 5F 5E 8B E5 5D C2 06 00 55 8B .V...._^..]...U.
0800:B960 EC 8B 46 0A A3 E8 4E 0B C0 74 12 8B 46 0C A3 EA ..F...N..t..F...
0800:B970 4E FF 76 06 FF 76 04 FF 76 08 E8 59 FD 5D C3 55 N.v..v..v..Y.].U
0800:B980 8B EC 83 EC 04 56 57 8B 46 04 3B 06 E8 24 72 0A .....VW.F.;..$r.
0800:B990 B8 06 00 50 E8 94 D3 E9 AA 00 8B 46 0A 40 3D 02 ...P.......F.@=.
0800:B9A0 00 72 0D 8B 5E 04 D1 E3 F7 87 EA 24 00 02 74 05 .r..^......$..t.
0800:B9B0 33 C0 E9 8F 00 FF 76 0A FF 76 08 FF 76 06 FF 76 3.....v..v..v..v
0800:B9C0 04 E8 8C D5 83 C4 08 89 46 FE 40 3D 02 00 72 0D ........F.@=..r.
0800:B9D0 8B 5E 04 D1 E3 F7 87 EA 24 00 40 75 05 8B 46 FE .^......$.@u..F.
0800:B9E0 EB 62 8B 4E FE C4 76 06 8B FE 8B DE FC 26 AC 3C .b.N..v......&.<
0800:B9F0 1A 74 2E 3C 0D 74 05 AA E2 F3 EB 1D E2 EF 06 53 .t.<.t.........S
0800:BA00 B8 01 00 50 8D 46 FD 16 50 FF 76 04 E8 41 D5 83 ...P.F..P.v..A..
0800:BA10 C4 08 5B 07 FC 8A 46 FD AA 3B FB 75 02 EB 96 EB ..[...F..;.u....
0800:BA20 20 53 B8 01 00 50 F7 D9 1B C0 50 51 FF 76 04 E8  S...P....PQ.v..
0800:BA30 F7 D3 83 C4 08 8B 5E 04 D1 E3 81 8F EA 24 00 02 ......^......$..
0800:BA40 5B 2B FB 97 5F 5E 8B E5 5D C3 55 8B EC 57 1E B4 [+.._^..].U..W..
0800:BA50 56 C5 56 04 C4 7E 08 CD 21 1F 72 04 33 C0 EB 04 V.V..~..!.r.3...
0800:BA60 50 E8 C7 D2 5F 5D C3 55 8B EC 33 C0 50 50 50 FF P..._].U..3.PPP.
0800:BA70 76 06 FF 76 04 E8 3B F2 83 C4 0A 0B C0 75 08 C4 v..v..;......u..
0800:BA80 5E 04 26 83 67 02 EF 5D C3 55 8B EC 56 57 8B 7E ^.&.g..].U..VW.~
0800:BA90 0C 8B 76 0E C4 5E 04 26 8B 47 12 3B 46 04 75 0B ..v..^.&.G.;F.u.
0800:BAA0 83 FF 02 7F 06 81 FE FF 7F 76 06 B8 FF FF E9 E3 .........v......
0800:BAB0 00 83 3E B2 26 00 75 0F 81 7E 04 6C 23 75 08 C7 ..>.&.u..~.l#u..
0800:BAC0 06 B2 26 01 00 EB 14 83 3E B0 26 00 75 0D 81 7E ..&.....>.&.u..~
0800:BAD0 04 58 23 75 06 C7 06 B0 26 01 00 C4 5E 04 26 83 .X#u....&...^.&.
0800:BAE0 3F 00 74 12 B8 01 00 50 33 C0 50 50 FF 76 06 53 ?.t....P3.PP.v.S
0800:BAF0 E8 C0 F1 83 C4 0A C4 5E 04 26 F7 47 02 04 00 74 .......^.&.G...t
0800:BB00 0D 26 FF 77 0A 26 FF 77 08 E8 69 E3 59 59 C4 5E .&.w.&.w..i.YY.^
0800:BB10 04 26 83 67 02 F3 26 C7 47 06 00 00 8B 46 06 8B .&.g..&.G....F..
0800:BB20 56 04 83 C2 05 26 89 47 0A 26 89 57 08 26 89 47 V....&.G.&.W.&.G
0800:BB30 0E 26 89 57 0C 83 FF 02 74 58 0B F6 76 54 C7 06 .&.W....tX..vT..
0800:BB40 52 23 F0 C7 8B 46 08 0B 46 0A 75 1F 56 E8 2F E4 R#...F..F.u.V./.
0800:BB50 59 89 56 0A 89 46 08 0B C2 75 03 E9 4D FF C4 5E Y.V..F...u..M..^
0800:BB60 04 26 83 4F 02 04 EB 03 E9 40 FF C4 5E 04 8B 46 .&.O.....@..^..F
0800:BB70 0A 8B 56 08 26 89 47 0E 26 89 57 0C 26 89 47 0A ..V.&.G.&.W.&.G.
0800:BB80 26 89 57 08 26 89 77 06 83 FF 01 75 05 26 83 4F &.W.&.w....u.&.O
0800:BB90 02 08 33 C0 5F 5E 5D C3 55 8B EC 56 8B 76 04 8B ..3._^].U..V.v..
0800:BBA0 46 0A 0B 46 0C 74 3D FF 76 08 FF 76 06 E8 17 04 F..F.t=.v..v....
0800:BBB0 59 59 3B C6 72 1C 56 FF 76 08 FF 76 06 FF 76 0C YY;.r.V.v..v..v.
0800:BBC0 FF 76 0A E8 58 04 83 C4 0A C4 5E 0A 26 C6 00 00 .v..X.....^.&...
0800:BBD0 EB 12 FF 76 08 FF 76 06 FF 76 0C FF 76 0A E8 BD ...v..v..v..v...
0800:BBE0 03 83 C4 08 5E 5D C2 0A 00 55 8B EC 83 EC 02 C4 ....^]...U......
0800:BBF0 5E 04 26 80 7F FF 2E 75 03 FF 4E 04 FF 4E 04 C4 ^.&....u..N..N..
0800:BC00 5E 04 26 8A 07 98 89 46 FE B9 04 00 BB 39 BC 2E ^.&....F.....9..
0800:BC10 8B 07 3B 46 FE 74 07 83 C3 02 E2 F3 EB 13 2E FF ..;F.t..........
0800:BC20 67 08 C4 5E 04 26 80 7F FE 00 75 05 B8 01 00 EB g..^.&....u.....
0800:BC30 02 33 C0 8B E5 5D C2 04 00 00 00 2F 00 3A 00 5C .3...]...../.:.\
0800:BC40 00 2C BC 2C BC 22 BC 2C BC 55 8B EC 83 EC 58 56 .,.,.".,.U....XV
0800:BC50 57 33 FF 8B 46 08 0B 46 0A 74 07 C4 5E 08 26 C6 W3..F..F.t..^.&.
0800:BC60 07 00 8B 46 0C 0B 46 0E 74 07 C4 5E 0C 26 C6 07 ...F..F.t..^.&..
0800:BC70 00 8B 46 10 0B 46 12 74 07 C4 5E 10 26 C6 07 00 ..F..F.t..^.&...
0800:BC80 8B 46 14 0B 46 16 74 07 C4 5E 14 26 C6 07 00 8D .F..F.t..^.&....
0800:BC90 46 A8 8C 56 FE 89 46 FC EB 03 FF 46 04 C4 5E 04 F..V..F....F..^.
0800:BCA0 26 80 3F 20 74 F4 FF 76 06 53 E8 1A 03 59 59 8B &.? t..v.S...YY.
0800:BCB0 F0 3D 50 00 7E 03 BE 50 00 C4 5E FC 26 C6 07 00 .=P.~..P..^.&...
0800:BCC0 FF 46 FC 56 FF 76 06 FF 76 04 FF 76 FE FF 76 FC .F.V.v..v..v..v.
0800:BCD0 E8 4B 03 83 C4 0A 01 76 FC C4 5E FC 26 C6 07 00 .K.....v..^.&...
0800:BCE0 33 F6 FF 4E FC C4 5E FC 26 8A 07 98 89 46 FA B9 3..N..^.&....F..
0800:BCF0 07 00 BB 1F BE 2E 8B 07 3B 46 FA 74 07 83 C3 02 ........;F.t....
0800:BD00 E2 F3 EB DE 2E FF 67 0E 0B F6 75 13 C4 5E FC 26 ......g...u..^.&
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
0800:BE30 BD 08 BD 89 BD 48 BD E6 BD 89 BD 55 8B EC FF 76 .....H.....U...v
0800:BE40 16 FF 76 14 FF 76 12 FF 76 10 FF 76 0E FF 76 0C ..v..v..v..v..v.
0800:BE50 FF 76 0A FF 76 08 FF 76 06 FF 76 04 E8 EA FD 83 .v..v..v..v.....
0800:BE60 C4 14 5D C3 55 8B EC 56 8B 76 08 56 FF 76 0C FF ..].U..V.v.V.v..
0800:BE70 76 0A C4 5E 04 26 FF 77 02 26 FF 37 E8 BC F1 83 v..^.&.w.&.7....
0800:BE80 C4 0A C4 5E 04 26 01 37 53 06 26 8E 47 02 8C C0 ...^.&.7S.&.G...
0800:BE90 07 5B 26 8B 1F 8E C0 26 C6 07 00 8B C6 5E 5D C2 .[&....&.....^].
0800:BEA0 0A 00 55 8B EC C4 5E 04 26 C6 07 00 B8 64 BE 50 ..U...^.&....d.P
0800:BEB0 16 8D 46 04 50 FF 76 0A FF 76 08 8D 46 0C 50 E8 ..F.P.v..v..F.P.
0800:BEC0 66 D9 5D C3 55 8B EC C4 5E 04 26 C6 07 00 B8 64 f.].U...^.&....d
0800:BED0 BE 50 16 8D 46 04 50 FF 76 0A FF 76 08 FF 76 0C .P..F.P.v..v..v.
0800:BEE0 E8 45 D9 5D C3 55 8B EC 56 C4 5E 04 26 8B 47 02 .E.].U..V.^.&.G.
0800:BEF0 26 8B 37 26 FF 07 8E C0 26 8A 14 8A C2 0A C0 75 &.7&....&......u
0800:BF00 05 B8 FF FF EB 04 8A C2 B4 00 5E 5D C3 55 8B EC ..........^].U..
0800:BF10 C4 5E 06 26 FF 0F 5D C3 55 8B EC 16 8D 46 0C 50 .^.&..].U....F.P
0800:BF20 FF 76 0A FF 76 08 16 8D 46 04 50 B8 0D BF 50 B8 .v..v...F.P...P.
0800:BF30 E5 BE 50 E8 61 D0 83 C4 10 5D C3 55 8B EC FF 76 ..P.a....].U...v
0800:BF40 0E FF 76 0C FF 76 0A FF 76 08 16 8D 46 04 50 B8 ..v..v..v...F.P.
0800:BF50 0D BF 50 B8 E5 BE 50 E8 3D D0 83 C4 10 5D C3    ..P...P.=....].

fn0800_BF5F()
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
	repne	
	scasb	

l0800_BF72:
	push	es
	lea	si,[di-01]
	les	di,[bp+08]
	mov	cx,FFFF

l0800_BF7C:
	repne	
	scasb	

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
	rep	
	movsw	

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

fn0800_BF9E()
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
	repne	
	scasb	

l0800_BFB0:
	not	cx
	push	ds
	mov	ax,es
	mov	ds,ax
	les	di,[bp+04]

l0800_BFBA:
	rep	
	movsb	

l0800_BFBC:
	pop	ds
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	pop	di
	pop	si
	pop	bp
	ret	

fn0800_BFC7()
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
	repne	
	scasb	

l0800_BFDF:
	xchg	ax,cx
	not	ax
	dec	ax

l0800_BFE3:
	pop	di
	pop	bp
	ret	
0800:BFE6                   55 8B EC 56 57 8C DA FC C4 7E       U..VW....~
0800:BFF0 08 8B F7 8B 46 0C 8B C8 E3 1E 8B D8 32 C0 F2 AE ....F.......2...
0800:C000 2B D9 8B CB 8B FE C5 76 04 F3 A6 8A 44 FF 26 8A +......v....D.&.
0800:C010 5D FF 32 E4 8A FC 2B C3 8E DA 5F 5E 5D C3       ].2...+..._^]. 

fn0800_C01E()
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
	repne	
	scasb	

l0800_C032:
	sub	bx,cx
	push	ds
	mov	di,es
	mov	ds,di
	les	di,[bp+04]
	xchg	bx,cx

l0800_C03E:
	rep	
	movsb	

l0800_C040:
	mov	cx,bx

l0800_C042:
	rep	
	stosb	

l0800_C044:
	pop	ds
	mov	dx,[bp+06]
	mov	ax,[bp+04]
	pop	di
	pop	si
	pop	bp
	ret	

fn0800_C04F()
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
0800:C177                      55 8B EC E8 FC 01 A1 E4 26        U.......&
0800:C180 8B 16 E2 26 81 C2 00 A6 15 CE 12 29 56 04 19 46 ...&.......)V..F
0800:C190 06 C4 5E 0C 26 C6 47 02 00 33 C0 BA 3C 00 50 52 ..^.&.G..3..<.PR
0800:C1A0 FF 76 06 FF 76 04 E8 21 CA C4 5E 0C 26 88 47 03 .v..v..!..^.&.G.
0800:C1B0 33 C0 BA 3C 00 50 52 FF 76 06 FF 76 04 E8 FB C9 3..<.PR.v..v....
0800:C1C0 89 56 06 89 46 04 33 C0 BA 3C 00 50 52 FF 76 06 .V..F.3..<.PR.v.
0800:C1D0 FF 76 04 E8 F4 C9 C4 5E 0C 26 88 07 33 C0 BA 3C .v.....^.&..3..<
0800:C1E0 00 50 52 FF 76 06 FF 76 04 E8 CF C9 89 56 06 89 .PR.v..v.....V..
0800:C1F0 46 04 33 C0 BA F8 88 50 52 FF 76 06 FF 76 04 E8 F.3....PR.v..v..
0800:C200 B9 C9 D1 E0 D1 E0 05 BC 07 C4 5E 08 26 89 07 33 ..........^.&..3
0800:C210 C0 BA F8 88 50 52 FF 76 06 FF 76 04 E8 AB C9 89 ....PR.v..v.....
0800:C220 56 06 89 46 04 83 7E 06 00 7C 44 75 07 81 7E 04 V..F..~..|Du..~.
0800:C230 50 22 72 3B 81 6E 04 50 22 83 5E 06 00 C4 5E 08 P"r;.n.P".^...^.
0800:C240 26 FF 07 33 C0 BA 38 22 50 52 FF 76 06 FF 76 04 &..3..8"PR.v..v.
0800:C250 E8 68 C9 C4 5E 08 26 01 07 33 C0 BA 38 22 50 52 .h..^.&..3..8"PR
0800:C260 FF 76 06 FF 76 04 E8 61 C9 89 56 06 89 46 04 83 .v..v..a..V..F..
0800:C270 3E E6 26 00 74 3E 33 C0 BA 18 00 50 52 FF 76 06 >.&.t>3....PR.v.
0800:C280 FF 76 04 E8 44 C9 50 33 C0 BA 18 00 50 52 FF 76 .v..D.P3....PR.v
0800:C290 06 FF 76 04 E8 24 C9 50 33 C0 50 C4 5E 08 26 8B ..v..$.P3.P.^.&.
0800:C2A0 07 05 4E F8 50 E8 AB 02 0B C0 74 08 83 46 04 01 ..N.P.....t..F..
0800:C2B0 83 56 06 00 33 C0 BA 18 00 50 52 FF 76 06 FF 76 .V..3....PR.v..v
0800:C2C0 04 E8 06 C9 C4 5E 0C 26 88 47 01 33 C0 BA 18 00 .....^.&.G.3....
0800:C2D0 50 52 FF 76 06 FF 76 04 E8 E0 C8 89 56 06 89 46 PR.v..v.....V..F
0800:C2E0 04 83 46 04 01 83 56 06 00 C4 5E 08 26 F7 07 03 ..F...V...^.&...
0800:C2F0 00 75 33 83 7E 06 00 7C 12 7F 06 83 7E 04 3C 76 .u3.~..|....~.<v
0800:C300 0A 83 6E 04 01 83 5E 06 00 EB 1B 83 7E 06 00 75 ..n...^.....~..u
0800:C310 15 83 7E 04 3C 75 0F C4 5E 08 26 C6 47 03 02 26 ..~.<u..^.&.G..&
0800:C320 C6 47 02 1D EB 51 C4 5E 08 26 C6 47 03 00 EB 1D .G...Q.^.&.G....
0800:C330 C4 5E 08 26 8A 47 03 98 8B D8 8A 87 B4 26 98 99 .^.&.G.......&..
0800:C340 29 46 04 19 56 06 8B 5E 08 26 FE 47 03 C4 5E 08 )F..V..^.&.G..^.
0800:C350 26 8A 47 03 98 8B D8 8A 87 B4 26 98 99 3B 56 06 &.G.......&..;V.
0800:C360 7C CE 75 05 3B 46 04 72 C7 C4 5E 08 26 FE 47 03 |.u.;F.r..^.&.G.
0800:C370 8A 46 04 26 88 47 02 5D C3                      .F.&.G.].      

fn0800_C379()
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

fn0800_C553()
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

fn0800_C632()
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

fn0800_C779()
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
1483:0070 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
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
1483:09C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:09D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:09E0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:09F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:0A00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
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
1483:0AD0 FC 8B FE 56 BE 56 01 B9 A0 01 10 C3 E8 60 02 26 ...V.V.......`.&
1483:0AE0 D8 AD 86 E0 60 C3 19 02 81 C7 80 64 00 06 04 87 ....`......d....
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
1483:0C10 87 BA 00 80 51 56 36 3A 04 75 32 50 53 1C 8C B8 ....QV6:.u2PS...
1483:0C20 55 19 E0 48 AB A7 B1 9B 62 C8 56 6B 10 D7 86 D1 U..H....b.Vk....
1483:0C30 DB D1 D0 E2 FA AB 8B C6 2B C4 2D BC 2E 36 8A 7D ........+.-..6.}
1483:0C40 24 26 89 45 3C 59 5B 58 03 DA 46 E2 C6 5E 59 D1 $&.E<Y[X..F..^Y.
1483:0C50 EA FE C0 3C 11 75 BA 5E 85 04 5A 5F C3 41 01 4B ...<.u.^..Z_.A.K
1483:0C60 56 50 03 02 67 6A 86 16 40 03 6E 03 FC 27 E8 47 VP..gj..@.n..'.G
1483:0C70 0F 03 3F 03 87 03 33 66 93 12 B4 7C 14 1D 03 09 ..?...3f...|....
1483:0C80 5C 01 B9 0E 02 E0 19 09 CA 8B 07 56 12 E8 A4 01 \..........V....
1483:0C90 C0 07 11 C2 00 E8 71 BD 6B 10 18 E1 0C 01 88 03 ......q.k.......
1483:0CA0 22 06 01 31 02 20 E8 A5 F0 02 17 69 94 3E 17 62 "..1. .....i.>.b
1483:0CB0 0C 00 17 4D 00 E3 2D AC 32 C0 41 AA E2 F9 D1 4E ...M..-.2.A....N
1483:0CC0 7C 10 1F 22 1F AA 1D 1F 8B 01 1F 8B CF 2B CE 87 |..".........+..
1483:0CD0 45 14 E8 0B 43 29 16 E3 F7 29 E0 01 29 56 51 1A E...C)...)..)VQ.
1483:0CE0 8B 7E 02 56 E5 8B C3 B9 08 00 D1 E8 73 03 35 01 .~.V........s.5.
1483:0CF0 A0 E2 F7 AB FE C3 75 ED 59 C0 18 AC 32 D8 8A 01 ......u.Y...2...
1483:0D00 C7 32 FF D1 E3 8B 19 97 09 E2 2D F1 5E 3B DA 74 .2........-.^;.t
1483:0D10 1A E8 00 00 5A 83 C2 0D B4 09 CD 21 B8 FF 4C 04 ....Z......!..L.
1483:0D20 70 42 61 64 20 43 52 43 0D 0A 24 C3 90 94 4A 77 pBad CRC..$...Jw
1483:0D30 19 76 04 D3 12 D0 76 08 7D 03 38 5F 7A AE 28 50 .v....v.}.8_z.(P
1483:0D40 7A B8 00 E8 B3 B4 2A 68 02 15 68 FC 56 80 F6 17 z.....*h..h.V...
1483:0D50 7C 9B 02 25 68 5F 0F 68 58 0C 87 68 43 C0 35 68 |..%h_.hX..hC.5h
1483:0D60 83 C4 12 F8 F7 5E FA 06 CE 88 AC 87 5C 85 14 E8 .....^......\...
1483:0D70 10 7D 60 13 59 87 10 B3 12 F5 20 87 7D 00 1D 47 .}`.Y..... .}..G
1483:0D80 87 10 E8 96 D5 08 87 B4 5D 99 54 04 2A 18 FE 03 ........].T.*...
1483:0D90 18 F8 00 E8 9D E1 02 18 61 28 18 7C 5A 0C 3F 18 ........a(.|Z.?.
1483:0DA0 45 2D B2 99 AA 79 12 AA 79 14 F8 F7 1A FA 4F 8F E-...y..y.....O.
1483:0DB0 79 45 58 45 01 A8 79 C0 4D 5A D0 29 00 49 02 06 yEXE..y.MZ.).I..
1483:0DC0 47 00 0E BC 03 0B 06 00 0E 00 8C D3 8E C3 8C CA G...............
1483:0DD0 8E DA 30 8B 0E 89 8B F1 83 B9 60 EE 02 8B FE D1 ..0.......`.....
1483:0DE0 E9 FD F3 A5 53 B8 35 00 50 8B 2E 0A 00 03 16 CE ....S.5.P.......
1483:0DF0 CB B8 00 10 18 3B C5 76 1B C5 2B 06 E8 2B D8 2B .....;.v..+..+.+
1483:0E00 D0 2E C5 34 B1 03 C3 D3 E0 8B C8 D1 E0 48 48 8B ...4.........HH.
1483:0E10 F0 8B F8 0B 30 0B ED 75 8F D9 FC 07 8E DB 83 EC ....0..u........
1483:0E20 1C 8B EC 81 EC 80 01 8B C4 43 55 05 A0 8F 46 0A .........CU...F.
1483:0E30 A2 05 0C E1 BE 11 00 AC 88 46 14 33 FF 89 7E 15 .........F.3..~.
1483:0E40 10 96 16 0E 06 8C 56 04 80 93 37 01 E8 32 60 01 ......V...7..2`.
1483:0E50 0E 1A 8C 5E 00 30 8C 46 63 56 08 E8 74 7B 0E 9F ...^.0.FcV..t{..
1483:0E60 0A E8 75 0C 05 0C E8 6F 1D 14 E0 1D 12 EB 22 82 ..u....o......".
1483:0E70 13 D2 23 9F 3E 14 CB 04 0E 9F 8E 5E 9C 0D 04 00 ..#.>......^....
1483:0E80 88 A5 C9 3B B0 55 BE 1A 55 BE 0F 1A 8A 4E 15 06 ...;.U..U....N..
1483:0E90 87 AD 16 C3 05 AD 16 2E 58 18 BC FF 4E 12 75 A4 ........X...N.u.
1483:0EA0 8B DF 83 E7 0F 81 C7 00 80 B1 04 D3 EB 8C C0 03 ................
1483:0EB0 C3 2D 00 08 8E C0 8B DE 83 E6 0F 0F 62 D8 0F 8E .-..........b...
1483:0EC0 D8 E9 FE 4E 14 74 03 E9 59 FF 1F 1E 07 0E 1F 8C ...N.t..Y.......
1483:0ED0 C2 32 ED BE A0 02 AC 8A C8 E3 13 AD 03 C2 B5 2A .2.............*
1483:0EE0 D5 C3 32 E4 AC 03 F8 26 01 15 E2 F8 EB E8 0C 20 ..2....&....... 
1483:0EF0 8B 36 04 90 3E 01 06 00 03 FA 01 16 02 86 AD EA .6..>...........
1483:0F00 10 8E 24 30 DA 33 26 DB FA 8B 01 E6 8E D7 FB 2E ..$0.3&.........
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
1483:1100 B6 36 BA B6 2D 97 5D B6 26 D4 B6 1C 75 B6 34 FD .6..-.].&...u.4.
1483:1110 11 B6 0F 45 01 E8 40 06 B1 1C 89 52 5E 83 A9 5E ...E..@....R^..^
1483:1120 67 7D 1D 22 CA 02 5E E0 52 5E D9 F9 12 5E 7D BE g}."..^.R^...^}.
1483:1130 59 9F 1C 02 D7 C4 1C F5 04 C4 D9 FD F7 C4 FE 99 Y...............
1483:1140 BF C4 BF 01 AE C4 1E EB C4 D8 FA 0A C4 C9 FE 1C ................
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
1483:1770 BE 4C 01 B9 FA 00 0F 18 C3 E8 02 2F D8 AD B8 61 .L........./...a
1483:1780 86 E0 C3 8B F5 F9 AC 12 C0 02 C0 BF 4C E9 8B 00 ............L...
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
1483:1B10 8B C3 E6 8E D7 FB 2E FF 2F 50 1E 06 16 07 E0 17 ......../P......
1483:1B20 E5 E3 01 7C 42 01 22 E8 26 01 E9 83 FE 00 75 07 ...|B.".&.....u.
1483:1B30 8E 95 80 C4 80 1E E2 E4 4A 75 44 E1 30 F8 07 96 ........JuD.0...
1483:1B40 3B C3 74 0D 48 BA E2 02 94 10 F8 2E 74 C5 00 24 ;.t.H.......t..$
1483:1B50 0E 07 BF FB 02 AC AA E8 37 00 FA 4F B8 0D 0A AB ........7..O....
1483:1B60 B0 24 61 AA 1D BA EC 02 F3 01 19 D2 15 24 5C 56 .$a..........$\V
1483:1B70 BE 69 72 75 73 24 20 43 68 65 63 6B 20 46 61 69 .irus$ Check Fai
1483:1B80 6C 65 64 3A 20 02 B5 AF 32 31 BB 09 D9 E5 0C 30 led: ...21.....0
1483:1B90 0E F2 2C 28 35 F9 2E 28 F8 F7 91 F8 10 91 54 5A ..,(5..(......TZ
1483:1BA0 1F 87 B9 01 32 15 87 A5 E4 41 67 02 01 FB 84 E5 ....2....Ag.....
1483:1BB0 FF 06 1F FF EC 1C F8 24 0A 04 73 03 E9 79 FF A1 .......$..s..y..
1483:1BC0 0D E1 F1 39 0D 3F BF 25 4D F7 E0 25 BA 0E 03 E9 ...9.?.%M..%....
1483:1BD0 F9 97 D1 25 38 F4 25 33 BE 25 2C 97 CF 25 8B 01 ...%8.%3.%,..%..
1483:1BE0 F7 8F AD 38 8F AD 08 FB 4D 2D FE 85 4D 0A 7F 03 ...8....M-..M...
1483:1BF0 06 CF 4D 23 03 09 F9 4D 14 03 FF 24 4D 27 53 54 ..M#...M...$M'ST
1483:1C00 F0 4C 02 82 60 1A E9 26 7C 76 0C 86 3B FF FF 48 .L..`..&|v..;..H
1483:1C10 E7 03 B8 EC 41 FA 02 64 20 18 47 E8 00 0A 4B FA ....A..d .G...K.
1483:1C20 FF F0 4D F5 CC 0D 49 60 F3 05 70 00 10 2B 0E FF ..M...I`..p..+..
1483:1C30 FE 41 F6 1C 0F 08 08 2D B8 50 67 02 52 48 22 48 .A.....-.Pg.RH"H
1483:1C40 B1 CC 63 2C 20 0C 0F 04 52 6F 4C 11 04 01 1B 49 ..c, ...RoL....I
1483:1C50 EC FF E0 4C D4 AE 56 18 48 E0 FF 00 B9 CB 62 F0 ...L..V.H.....b.
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
1483:22F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2300 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2310 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2320 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2330 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2340 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2350 00 00 0C 8B 0C 8B 0C 8B 00 00 09 02 00 00 00 00 ................
1483:2360 00 00 00 00 00 00 00 00 00 00 58 23 00 00 0A 02 ..........X#....
1483:2370 01 00 00 00 00 00 00 00 00 00 00 00 00 00 6C 23 ..............l#
1483:2380 00 00 02 02 02 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2390 00 00 80 23 00 00 43 02 03 00 00 00 00 00 00 00 ...#..C.........
1483:23A0 00 00 00 00 00 00 94 23 00 00 42 02 04 00 00 00 .......#..B.....
1483:23B0 00 00 00 00 00 00 00 00 00 00 A8 23 00 00 00 00 ...........#....
1483:23C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:23D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:23E0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:23F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2400 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2410 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2420 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2430 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2440 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2450 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2460 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2470 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2480 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2490 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:24A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:24B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:24C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:24D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
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
1483:2730 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2740 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2750 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2760 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2770 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2780 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2790 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:27A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:27B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:27C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:27D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:27E0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:27F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2800 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2810 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2820 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2830 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2840 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2850 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2860 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2870 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2880 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:2890 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:28A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:28B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
1483:28C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
