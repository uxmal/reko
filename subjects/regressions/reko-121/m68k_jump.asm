;;; Segment Image base (0000C000)

fn0000C000()
	movem.l	d0-d7/a0-a6,-(a7)
	movea.l	a0,a2
	movea.l	a1,a6
	move.l	(a2)+,d0
	move.l	d0,$00FF0F08
	moveq	#$+00,d6
	move.l	d6,$00FF0F04
	movea.l	d6,a4
	moveq	#$+0F,d2
	move.l	#$000000FF,d7
	bra	$0000C3B4

l0000C026:
	move.l	a4,d3
	addq.l	#$01,a4
	move.b	(a2,d3),d3
	and.l	d7,d3
	move.l	d3,d0
	andi.w	#$00F0,d0
	lsr.b	#$02,d0
	cmpi.w	#$002C,d0
	bgt	$0000C3B4

l0000C040:
	movea.l	(04,pc,d0),a5
	jmp.l	(a5)
Code vector at 0000C046 (45 bytes)
	0000C0A0
	0000C0B6
	0000C0D0
	0000C0FA
	0000C136
	0000C16A
	0000C1A0
	0000C218
	0000C22A
	0000C23E
	0000C27A
0000C046                   00 00 C0 A0 00 00 C0 B6 00 00       ..........
0000C050 C0 D0 00 00 C0 FA 00 00 C1 36 00 00 C1 6A 00 00 .........6...j..
0000C060 C1 A0 00 00 C2 18 00 00 C2 2A 00 00 C2 3E 00 00 .........*...>..
0000C070 C2 7A 00                                        .z.            
0000C073          00 C2 E0 C6 82 52 83 60 08 2C C6 58 B9    .....R.`.,.X.
0000C080 00 FF 0F 04 51 CB FF F6 60 00 03 2A C6 02 3C 3C ....Q...`..*..<<
0000C090 11 11 CC C3 36 06 48 46 3C 03 2C C6 58 B9 00 FF ....6.HF<.,.X...

l0000C0A0:
	btst	d7,d4
	bra	$0000C3B4
0000C0A6                   C6 02 54 43 E5 8B 20 4E 91 C3       ..TC.. N..
0000C0B0 1C 28 00 03 E1 4E                               .(...N         

l0000C0B6:
	move.b	$0002(a0),d6
	swap.l	d6
	move.b	$0001(a0),d6
	lsl.w	#$08,d6
	move.b	(a0),d6
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3B4

l0000C0D0:
	andi.b	#$0F,d3
	lsl.l	#$08,d3
	move.l	d3,d4
	move.l	a4,d3
	addq.l	#$01,a4
	move.b	(a2,d3),d3
	and.l	d7,d3
	or.l	d3,d4
	addq.l	#$02,d4
	lsl.l	#$02,d4
	movea.l	a6,a0
	suba.l	d4,a0
	move.b	$0003(a0),d6
	lsl.w	#$08,d6
	move.b	$0002(a0),d6
	swap.l	d6
	move.b	$0001(a0),d6

l0000C0FA:
	ori.b	#$4E,d1

l0000C0FC:
	lsl.w	#$08,d6
	move.b	(a0),d6
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3B4
0000C10C                                     02 83 00 00             ....
0000C110 00 0F 52 43 60 00 00 22                         ..RC`.."       

l0000C118:
	move.b	(a2,a4),d6
	lsl.w	#$08,d6
	move.b	(01,a2,a4),d6
	swap.l	d6
	move.b	(02,a2,a4),d6
	lsl.w	#$08,d6
	move.b	(03,a2,a4),d6
	addq.l	#$04,a4
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04

l0000C136:
	btst	d7,d4

l0000C138:
	dbra	d3,$0000C118

l0000C13C:
	bra	$0000C3B4
0000C140 C6 82 2A 03 78 0F 26 0C 52 8C 16 32 38 00 C6 87 ..*.x.&.R..28...
0000C150 72 00 60 12 03 03 67 08 20 04 46 80 CC 80 8C 85 r.`...g. .F.....
0000C160 E2 0B E9 8C E9 8D 4A 03 66 EA                   ......J.f.     

l0000C16A:
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3B4
0000C176                   C6 82 47 F9 00 FF 0F 0C 26 83       ..G.....&.
0000C180 26 0C 52 8C 16 32 38 00 C6 87 12 03 E8 09 27 41 &.R..28.......'A
0000C190 00 04 22 03 C2 02 27 41 00 08 26 0C 52 8C 16 32 .."...'A..&.R..2

l0000C1A0:
	move.w	d0,d4
	and.l	d7,d3
	move.l	d2,d5
	moveq	#$+00,d4
	moveq	#$+00,d1
	bra	$0000C1DE

l0000C1AC:
	btst	d1,d3
	beq	$0000C1C0

l0000C1B0:
	move.l	d5,d0
	not.l	d0
	and.l	d0,d6
	move.l	d4,d0
	lsl.w	#$02,d0
	or.l	(a3,d0),d6
	addq.l	#$01,d4

l0000C1C0:
	lsr.b	#$01,d3
	move.l	(a3),d0
	lsl.l	#$04,d0
	move.l	d0,(a3)
	move.l	$0004(a0),d0
	lsl.l	#$04,d0
	move.l	d0,$0004(a0)
	move.l	$0008(a0),d0
	lsl.l	#$04,d0
	move.l	d0,$0008(a0)
	lsl.l	#$04,d5

l0000C1DE:
	tst.b	d3
	bne	$0000C1AC

l0000C1E2:
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3B4
0000C1EE                                           E9 8E               ..
0000C1F0 C6 82 8C 83 2C C6 58 B9 00 FF 0F 04 60 00 01 B6 ....,.X.....`...
0000C200 E8 8E C6 82 E8 9B 8C 83 2C C6 58 B9 00 FF 0F 04 ........,.X.....
0000C210 60 00 01 A2 C6 82 2A 03                         `.....*.       

l0000C218:
	move.l	a4,d3
	addq.l	#$01,a4
	move.b	(a2,d3),d3
	and.l	d7,d3
	move.l	d3,d4
	lsr.b	#$04,d4
	and.b	d2,d3
	moveq	#$+00,d6

l0000C22A:
	bra	$0000C232

l0000C22C:
	or.l	d5,d6
	lsl.l	#$04,d5
	lsl.l	#$04,d4

l0000C232:
	move.l	d3,d0
	subq.l	#$01,d3
	tst.l	d0
	bne	$0000C22C

l0000C23A:
	bra	$0000C240

l0000C23C:
	or.l	d4,d6

l0000C23E:
	lsl.l	#$04,d4

l0000C240:
	tst.l	d4
	bne	$0000C23C

l0000C244:
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3B4
0000C250 C6 82 22 03 26 0C 52 8C 16 32 38 00 C6 87 2A 03 ..".&.R..28...*.
0000C260 E8 0D C6 82 28 03 26 0C 52 8C 16 32 38 00 C6 87 ....(.&.R..28...
0000C270 2E 03 E8 0B CE 82 7C 00 60 08                   ......|.`.     

l0000C27A:
	or.l	d1,d6
	lsl.l	#$04,d1
	lsl.l	#$04,d5
	lsl.l	#$04,d4
	move.l	d3,d0
	subq.l	#$01,d3
	tst.l	d0
	bne	$0000C27A

l0000C28A:
	bra	$0000C292

l0000C28C:
	or.l	d5,d6
	lsl.l	#$04,d5
	lsl.l	#$04,d4

l0000C292:
	move.l	d7,d0
	subq.l	#$01,d7
	tst.l	d0
	bne	$0000C28C

l0000C29A:
	bra	$0000C2A0

l0000C29C:
	or.l	d4,d6
	lsl.l	#$04,d4

l0000C2A0:
	tst.l	d4
	bne	$0000C29C

l0000C2A4:
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	move.l	#$000000FF,d7
	bra	$0000C3B4
0000C2B6                   20 0C 52 8C 7A 00 1A 32 08 00        .R.z..2..
0000C2C0 CA 87 78 00 18 32 C8 00 02 83 00 00 00 0F 20 03 ..x..2........ .
0000C2D0 0C 80 00 00 00 09 6E 00 00 D4 D0 80 D0 80 2A 7B ......n.......*{
0000C2E0 08 04 4E D5 00 00 C3 36 00 00 C3 44 00 00 C3 52 ..N....6...D...R
0000C2F0 00 00 C3 60 00 00 C3 6C 00 00 C3 80 00 00 C3 94 ...`...l........
0000C300 00 00 C3 A6 00 00 C3 B8 00 00 C3 C8 02 86 00 FF ................
0000C310 FF FF E0 9D 8C 85 60 00 00 94 02 86 FF 00 FF FF ......`.........
0000C320 48 45 8C 85 60 00 00 86 02 86 FF FF 00 FF E1 8D HE..`...........
0000C330 8C 85 60 00 00 78 02 86 FF FF FF 00 8C 85 60 00 ..`..x........`.
0000C340 00 6C 02 86 00 00 FF FF E0 9D 8C 85 48 44 8C 84 .l..........HD..
0000C350 52 8C 60 00 00 58 02 86 00 FF 00 FF E0 9D 8C 85 R.`..X..........
0000C360 E1 8C 8C 84 52 8C 60 00 00 44 02 86 00 FF FF 00 ....R.`..D......
0000C370 E0 9D 8C 85 8C 84 52 8C 60 00 00 32 02 86 FF 00 ......R.`..2....
0000C380 00 FF 48 45 8C 85 E1 8C 8C 84 52 8C 60 1E 02 86 ..HE......R.`...
0000C390 FF 00 FF 00 48 45 8C 85 8C 84 52 8C 60 0E 02 86 ....HE....R.`...
0000C3A0 FF FF 00 00 E1 8D 8C 85 8C 84 52 8C 2C C6 58 B9 ..........R.,.X.
0000C3B0 00 FF 0F 04                                     ....           

l0000C3B4:
	move.l	$00FF0F04,d0
	cmp.l	$00FF0F08,d0
	blt	$0000C026

l0000C3C4:
	movem.l	(a7)+,d0-d7/a0-a6
	rts	
