;;; Segment Image base (0000C02A)

fn0000C02A()
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
	bra	$0000C3DE

l0000C050:
	move.l	a4,d3
	addq.l	#$01,a4
	move.b	(a2,d3),d3
	and.l	d7,d3
	move.l	d3,d0
	andi.w	#$00F0,d0
	lsr.b	#$02,d0
	cmpi.w	#$002C,d0
	bgt	$0000C3DE

l0000C06A:
	movea.l	(04,pc,d0),a5
	jmp.l	(a5)
Code vector at 0000C070 (45 bytes)
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
0000C070 00 00 C0 A0 00 00 C0 B6 00 00 C0 D0 00 00 C0 FA ................
0000C080 00 00 C1 36 00 00 C1 6A 00 00 C1 A0 00 00 C2 18 ...6...j........
0000C090 00 00 C2 2A 00 00 C2 3E 00 00 C2 7A 00          ...*...>...z.  
0000C09D                                        00 C2 E0              ...

l0000C0A0:
	and.l	d2,d3
	addq.l	#$01,d3
	bra	$0000C0AE

l0000C0A6:
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04

l0000C0AE:
	dbra	d3,$0000C0A6

l0000C0B2:
	bra	$0000C3DE

l0000C0B6:
	and.b	d2,d3
	move.w	#$1111,d6
	mulu.w	d3,d6
	move.w	d6,d3
	swap.l	d6
	move.w	d3,d6
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3DE

l0000C0D0:
	and.b	d2,d3
	addq.w	#$02,d3
	lsl.l	#$02,d3
	movea.l	a6,a0
	suba.l	d3,a0
	move.b	$0003(a0),d6
	lsl.w	#$08,d6
	move.b	$0002(a0),d6
	swap.l	d6
	move.b	$0001(a0),d6
	lsl.w	#$08,d6
	move.b	(a0),d6
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3DE

l0000C0FA:
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
	lsl.w	#$08,d6
	move.b	(a0),d6
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3DE

l0000C136:
	andi.l	#$0000000F,d3
	addq.w	#$01,d3
	bra	$0000C162

l0000C142:
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

l0000C162:
	dbra	d3,$0000C142

l0000C166:
	bra	$0000C3DE

l0000C16A:
	and.l	d2,d3
	move.l	d3,d5
	moveq	#$+0F,d4
	move.l	a4,d3
	addq.l	#$01,a4
	move.b	(a2,d3),d3
	and.l	d7,d3
	moveq	#$+00,d1
	bra	$0000C190

l0000C17E:
	btst	d1,d3
	beq	$0000C18A

l0000C182:
	move.l	d4,d0
	not.l	d0
	and.l	d0,d6
	or.l	d5,d6

l0000C18A:
	lsr.b	#$01,d3
	lsl.l	#$04,d4
	lsl.l	#$04,d5

l0000C190:
	tst.b	d3
	bne	$0000C17E

l0000C194:
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3DE

l0000C1A0:
	and.l	d2,d3
	lea	$00FF0F0C,a3
	move.l	d3,(a3)
	move.l	a4,d3
	addq.l	#$01,a4
	move.b	(a2,d3),d3
	and.l	d7,d3
	move.b	d3,d1
	lsr.b	#$04,d1
	move.l	d1,$0004(a3)
	move.l	d3,d1
	and.b	d2,d1
	move.l	d1,$0008(a3)
	move.l	a4,d3
	addq.l	#$01,a4
	move.b	(a2,d3),d3
	and.l	d7,d3
	move.l	d2,d5
	moveq	#$+00,d4
	moveq	#$+00,d1
	bra	$0000C208

l0000C1D6:
	btst	d1,d3
	beq	$0000C1EA

l0000C1DA:
	move.l	d5,d0
	not.l	d0
	and.l	d0,d6
	move.l	d4,d0
	lsl.w	#$02,d0
	or.l	(a3,d0),d6
	addq.l	#$01,d4

l0000C1EA:
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

l0000C208:
	tst.b	d3
	bne	$0000C1D6

l0000C20C:
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3DE

l0000C218:
	lsl.l	#$04,d6
	and.l	d2,d3
	or.l	d3,d6
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3DE

l0000C22A:
	lsr.l	#$04,d6
	and.l	d2,d3
	ror.l	#$04,d3
	or.l	d3,d6
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3DE

l0000C23E:
	and.l	d2,d3
	move.l	d3,d5
	move.l	a4,d3
	addq.l	#$01,a4
	move.b	(a2,d3),d3
	and.l	d7,d3
	move.l	d3,d4
	lsr.b	#$04,d4
	and.b	d2,d3
	moveq	#$+00,d6
	bra	$0000C25C

l0000C256:
	or.l	d5,d6
	lsl.l	#$04,d5
	lsl.l	#$04,d4

l0000C25C:
	move.l	d3,d0
	subq.l	#$01,d3
	tst.l	d0
	bne	$0000C256

l0000C264:
	bra	$0000C26A

l0000C266:
	or.l	d4,d6
	lsl.l	#$04,d4

l0000C26A:
	tst.l	d4
	bne	$0000C266

l0000C26E:
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	bra	$0000C3DE

l0000C27A:
	and.l	d2,d3
	move.l	d3,d1
	move.l	a4,d3
	addq.l	#$01,a4
	move.b	(a2,d3),d3
	and.l	d7,d3
	move.l	d3,d5
	lsr.b	#$04,d5
	and.l	d2,d3
	move.l	d3,d4
	move.l	a4,d3
	addq.l	#$01,a4
	move.b	(a2,d3),d3
	and.l	d7,d3
	move.l	d3,d7
	lsr.b	#$04,d3
	and.l	d2,d7
	moveq	#$+00,d6
	bra	$0000C2AC

l0000C2A4:
	or.l	d1,d6
	lsl.l	#$04,d1
	lsl.l	#$04,d5
	lsl.l	#$04,d4

l0000C2AC:
	move.l	d3,d0
	subq.l	#$01,d3
	tst.l	d0
	bne	$0000C2A4

l0000C2B4:
	bra	$0000C2BC

l0000C2B6:
	or.l	d5,d6
	lsl.l	#$04,d5
	lsl.l	#$04,d4

l0000C2BC:
	move.l	d7,d0
	subq.l	#$01,d7
	tst.l	d0
	bne	$0000C2B6

l0000C2C4:
	bra	$0000C2CA

l0000C2C6:
	or.l	d4,d6
	lsl.l	#$04,d4

l0000C2CA:
	tst.l	d4
	bne	$0000C2C6

l0000C2CE:
	move.l	d6,(a6)+
	addq.l	#$04,$00FF0F04
	move.l	#$000000FF,d7
	bra	$0000C3DE
0000C2E0 20 0C 52 8C 7A 00 1A 32 08 00 CA 87 78 00 18 32  .R.z..2....x..2
0000C2F0 C8 00 02 83 00 00 00 0F 20 03 0C 80 00 00 00 09 ........ .......
0000C300 6E 00 00 D4 D0 80 D0 80 2A 7B 08 04 4E D5 00 00 n.......*{..N...
0000C310 C3 36 00 00 C3 44 00 00 C3 52 00 00 C3 60 00 00 .6...D...R...`..
0000C320 C3 6C 00 00 C3 80 00 00 C3 94 00 00 C3 A6 00 00 .l..............
0000C330 C3 B8 00 00 C3 C8 02 86 00 FF FF FF E0 9D 8C 85 ................
0000C340 60 00 00 94 02 86 FF 00 FF FF 48 45 8C 85 60 00 `.........HE..`.
0000C350 00 86 02 86 FF FF 00 FF E1 8D 8C 85 60 00 00 78 ............`..x
0000C360 02 86 FF FF FF 00 8C 85 60 00 00 6C 02 86 00 00 ........`..l....
0000C370 FF FF E0 9D 8C 85 48 44 8C 84 52 8C 60 00 00 58 ......HD..R.`..X
0000C380 02 86 00 FF 00 FF E0 9D 8C 85 E1 8C 8C 84 52 8C ..............R.
0000C390 60 00 00 44 02 86 00 FF FF 00 E0 9D 8C 85 8C 84 `..D............
0000C3A0 52 8C 60 00 00 32 02 86 FF 00 00 FF 48 45 8C 85 R.`..2......HE..
0000C3B0 E1 8C 8C 84 52 8C 60 1E 02 86 FF 00 FF 00 48 45 ....R.`.......HE
0000C3C0 8C 85 8C 84 52 8C 60 0E 02 86 FF FF 00 00 E1 8D ....R.`.........
0000C3D0 8C 85 8C 84 52 8C 2C C6 58 B9 00 FF 0F 04       ....R.,.X..... 

l0000C3DE:
	move.l	$00FF0F04,d0
	cmp.l	$00FF0F08,d0
	blt	$0000C050

l0000C3EE:
	movem.l	(a7)+,d0-d7/a0-a6
	rts	
