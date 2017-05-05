;;; Segment code (00000000)

;; fn00000000: 00000000
fn00000000 proc
	jp	00000100
00000003          00 00 00 00 00 00 00 00 00 00 00 00 00    .............
00000010 1A 3F 02 00 00 00 00 00 00 00 00 00 00 00 00 00 .?..............
00000020 1A 4F 02 00 00 00 00 00 1F 00 00 00 00 00 00 00 .O..............
00000030 1A 53 02 00 00 00 00 00 1A 57 02 00 00 00 00 00 .S.......W......
00000040 1F 00 00 00 00 00 00 00 1A 60 02 00 00 00 00 00 .........`......
00000050 1A 64 02 00 00 00 00 00 1A 68 02 00 00 00 00 00 .d.......h......
00000060 1A 6C 02 00 00 00 00 00 1F 00 00 00 00 00 00 00 .l..............
00000070 1A 32 02 00 00 00 00 00 1A 39 02 00 00 00 00 00 .2.......9......
00000080 00 3E A0 FF EF C2 6E 04 37 C6 F7 37 C7 A5 37 C8 .>....n.7..7..7.
00000090 00 37 C9 03 37 CD 03 37 CE F0 37 CB A0 37 CF 00 .7..7..7..7..7..
000000A0 37 E4 35 37 D0 08 37 D1 08 37 E5 01 37 DA 40 37 7.57..7..7..7.@7
000000B0 E9 01 EF E9 6E 08 EF E9 6E 20 37 EA 00 37 D2 C0 ....n...n 7..7..
000000C0 37 DB 20 EF DB 6E E0 B8 E6 B9 E6 3A 20 FF 36 00 7. ..n.....: .6.
000000D0 1C A4 02 EA 26 92 7F C0 FF CE F5 1C A4 02 B0 BF ....&...........
000000E0 1C 00 00 1C A4 02 1C 23 08 1C 22 08 01 50 E3 00 .......#.."..P..
000000F0 00 4D E7 ED 28 E3 00 00 2E 2F ED E5 2E EF ED 20 .M..(..../..... 

l00000100:
	invalid
	nop
	ld	h,58
	calr	FFFFE458
	nop
	nop
	pop	iy
	invalid
	invalid
	invalid
	push	iz
	push	sp
	or	xhl,xiy
	nop
	nop
	push	iz
	invalid
	sra	20,xsp
	pop	xwa
	calr	FFFFEC48
	push	sp
	set	07,(xiz-0x47)
	invalid
	set	07,(xwa)
	ld	sp,0FC3
	invalid
	invalid
	set	04,(004F4A04)
	set	02,(004F4A02)
	call	00000000
	pop	xde
	invalid
	call	00000000
	invalid
	call	00000000
	invalid
	sbc	(xsp),wa
	ld	sp,4ED3
	ld	sp,10C2
	invalid
	call	00000000
	invalid
	call	00000000
	invalid
	call	00000000
	invalid
	call	00000000
	invalid
	set	07,(00A82E02)
	invalid
	decf
	push	sr
	and	(xwa-0x3D),xiz
	invalid
	pop	sr
	nop
	nop
	call	000002A4
	srl	02,w
	ld	sp,0FC3
	push	sp
	ex	xwa,xhl
	invalid
	invalid
	swi	06
	jr	Z,00000181
	invalid
	call	00000273
	and	(xde),wa
Index was outside the bounds of the array.

