;;; Segment code (00001000)

fn00001000()
	movem.l	d1-d6/a0-a6,-(a7)
	movea.l	a0,a2
	move.l	d0,d2
	lea	$00009608,a4
	movea.l	$00000004,a6
	lea	$0000B620,a3
	moveq	#$+00,d1
	move.l	#$000009E6,d0
	bra	$00001024

l00001022:
	move.l	d1,(a3)+

l00001024:
	dbra	d0,$00001022

l00001028:
	move.l	a7,$2058(a4)
	move.l	a6,$2050(a4)
	clr.l	$2054(a4)
	moveq	#$+00,d0
	move.l	#$00003000,d1
	jsr.l	$-0132(a6)
	lea	$01AA(pc),a1
	moveq	#$+00,d0
	jsr.l	$-0228(a6)
	move.l	d0,$47A4(a4)
	bne	$00001056

l00001050:
	moveq	#$+64,d0
	bra	$000011E4

l00001056:
	movea.l	$0114(a6),a3
	move.l	$0098(a3),$204C(a4)
	tst.l	$00AC(a3)
	beq	$000010F8

l00001068:
	move.l	a7,d0
	sub.l	$0038(a7),d0
	addi.l	#$00000080,d0
	move.l	d0,$201C(a4)
	movea.l	$00AC(a3),a0
	adda.l	a0,a0
	adda.l	a0,a0
	movea.l	$0010(a0),a1
	adda.l	a1,a1
	adda.l	a1,a1
	move.l	d2,d0
	moveq	#$+00,d1
	move.b	(a1)+,d1
	move.l	a1,$2060(a4)
	add.l	d1,d0
	addq.l	#$07,d0
	andi.w	#$FFFC,d0
	move.l	d0,$2068(a4)
	movem.l	d1/a1,-(a7)
	move.l	#$00010001,d1
	jsr.l	$-00C6(a6)
	movem.l	(a7)+,d1/a1
	tst.l	d0
	bne	$000010C0

l000010B4:
	move.l	#$000003E8,d0
	move.l	d0,-(a7)
	bra	$000011DA

l000010C0:
	movea.l	d0,a0
	move.l	d0,$2064(a4)
	move.l	d2,d0
	subq.l	#$01,d0
	add.l	d1,d2

l000010CC:
	move.b	(a2,d0),(02,a2,d2)
	subq.l	#$01,d2
	dbra	d0,$000010CC

l000010D8:
	move.b	#$20,(02,a4,d2)
	subq.l	#$01,d2
	move.b	#$22,(02,a4,d2)

l000010E6:
	move.b	(a1,d2),(01,a1,d2)
	dbra	d2,$000010E6

l000010F0:
	move.b	#$22,(a0)
	move.l	a0,-(a7)
	bra	$00001170

l000010F8:
	move.l	$003A(a3),$201C(a4)
	moveq	#$+7F,d0
	addq.l	#$01,d0
	add.l	d0,$201C(a4)
	lea	$005C(a3),a0
	jsr.l	$-0180(a6)
	lea	$005C(a3),a0
	jsr.l	$-0174(a6)
	move.l	d0,$2054(a4)
	move.l	d0,-(a7)
	movea.l	d0,a2
	move.l	$0024(a2),d0
	beq	$0000113C

l00001124:
	movea.l	$47A4(a4),a6
	movea.l	d0,a0
	move.l	$0000(a0),d1
	jsr.l	$-0060(a6)
	move.l	d0,$204C(a4)
	move.l	d0,d1
	jsr.l	$-007E(a6)

l0000113C:
	move.l	$0020(a2),d1
	beq	$0000115C

l00001142:
	move.l	#$000003ED,d2
	jsr.l	$-001E(a6)
	move.l	d0,$205C(a4)
	beq	$0000115C

l00001152:
	lsl.l	#$02,d0
	movea.l	d0,a0
	move.l	$0008(a0),$00A4(a3)

l0000115C:
	movea.l	$2054(a4),a0
	move.l	a0,-(a7)
	pea	$2018(a4)
	movea.l	$0024(a0),a0
	move.l	$0004(a0),$2060(a4)

l00001170:
	jsr.l	$4F3C(pc)
	jsr.l	$5E44(pc)
	moveq	#$+00,d0
	bra	$00001180

fn0000117C()
	move.l	$0004(a7),d0

fn00001180()
	movea.l	$2058(a4),a7
	move.l	d0,-(a7)
	move.l	$2044(a4),d0
	beq	$00001190

fn0000118C()
	movea.l	d0,a0
	jsr.l	(a0)

fn00001190()
	jsr.l	$717C(pc)
	jsr.l	$4F1A(pc)
	tst.l	$2054(a4)
	beq	$000011C8

l0000119E:
	movea.l	$47A4(a4),a6
	move.l	$205C(a4),d1
	beq	$000011AC

l000011A8:
	jsr.l	$-0024(a6)

l000011AC:
	move.l	$204C(a4),d1
	beq	$000011B6

l000011B2:
	jsr.l	$-005A(a6)

l000011B6:
	movea.l	$00000004,a6
	jsr.l	$-0084(a6)
	movea.l	$2054(a4),a1
	jsr.l	$-017A(a6)
	bra	$000011DA

l000011C8:
	move.l	$2068(a4),d0
	beq	$000011DA

l000011CE:
	movea.l	$2064(a4),a1
	movea.l	$00000004,a6
	jsr.l	$-00D2(a6)

fn000011DA()
	movea.l	$47A4(a4),a1
	jsr.l	$-019E(a6)
	move.l	(a7)+,d0

l000011E4:
	movem.l	(a7)+,d1-d6/a0-a6
	rts	
000011EA                               64 6F 73 2E 6C 69           dos.li
000011F0 62 72 61 72 79 00 00 00                         brary...       

fn000011F8()
	link	a5,#$FF5C
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00001204:
	movem.l	d2/d4-d7/a2-a3,-(a7)
	move.l	$0008(a5),d7
	pea	$0005(a4)
	pea	$0630(a4)
	jsr.l	$4F5C(pc)
	addq.w	#$08,a7
	moveq	#$+03,d0
	cmp.l	d0,d7
	bge	$00001224

l00001220:
	jsr.l	$1156(pc)

l00001224:
	movea.l	$000C(a5),a3
	movea.l	$0004(a3),a0

l0000122C:
	tst.b	(a0)+
	bne	$0000122C

l00001230:
	subq.l	#$01,a0
	suba.l	$0004(a3),a0
	move.l	a0,d0
	moveq	#$+01,d1
	cmp.l	d1,d0
	ble	$00001246

l0000123E:
	move.l	d1,-(a7)
	jsr.l	$10E2(pc)
	addq.w	#$04,a7

l00001246:
	movea.l	$0004(a3),a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	pea	$0634(a4)
	bsr	$00001AFA
	addq.w	#$08,a7
	move.l	d0,$22D4(a4)
	moveq	#$+02,d1
	cmp.l	d1,d0
	ble	$00001270

l00001266:
	pea	$00000001
	jsr.l	$10B8(pc)
	addq.w	#$04,a7

l00001270:
	movea.l	$0008(a3),a0

l00001274:
	tst.b	(a0)+
	bne	$00001274

l00001278:
	subq.l	#$01,a0
	suba.l	$0008(a3),a0
	move.l	a0,d0
	moveq	#$+01,d1
	cmp.l	d1,d0
	ble	$00001290

l00001286:
	pea	$00000002
	jsr.l	$1098(pc)
	addq.w	#$04,a7

l00001290:
	movea.l	$0008(a3),a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	pea	$0638(a4)
	bsr	$00001AFA
	addq.w	#$08,a7
	move.l	d0,$22D8(a4)
	moveq	#$+05,d1
	cmp.l	d1,d0
	ble	$000012BA

l000012B0:
	pea	$00000002
	jsr.l	$106E(pc)
	addq.w	#$04,a7

l000012BA:
	move.l	$22D8(a4),d0
	asl.l	#$02,d0
	lea	$0476(a4),a0
	movea.l	(a0,d0),a0
	lea	$38A0(a4),a1

l000012CC:
	move.b	(a0)+,(a1)+
	bne	$000012CC

l000012D0:
	pea	$0640(a4)
	pea	$38A0(a4)
	jsr.l	$689C(pc)
	addq.w	#$08,a7
	move.l	d7,d4
	subq.l	#$03,d4
	moveq	#$+03,d7
	tst.l	d4
	bne	$000012EC

l000012E8:
	jsr.l	$108E(pc)

l000012EC:
	moveq	#$+00,d0
	move.l	d0,$22D0(a4)
	move.l	d0,$22E8(a4)
	moveq	#$+00,d1
	move.w	d1,$26EC(a4)
	move.l	d0,$22E4(a4)
	move.w	d1,$26EE(a4)
	move.l	d0,$22E0(a4)
	move.l	#$00003000,$22C8(a4)
	move.w	#$FFFF,$2710(a4)
	moveq	#$+01,d0
	move.l	d0,$22DC(a4)
	clr.b	$381C(a4)
	lea	$000C(a3),a2
	bra	$00001494

l00001328:
	move.b	$0001(a3),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	pea	$0642(a4)
	bsr	$00001AFA
	addq.w	#$08,a7
	move.l	d0,d6
	moveq	#$+09,d0
	cmp.l	d0,d6
	blt	$0000134E

l00001344:
	pea	$00000003
	jsr.l	$0FDA(pc)
	addq.w	#$04,a7

l0000134E:
	moveq	#$+06,d0
	cmp.l	d0,d6
	bge	$00001386

l00001354:
	movea.l	(a2),a0

l00001356:
	tst.b	(a0)+
	bne	$00001356

l0000135A:
	subq.l	#$01,a0
	suba.l	(a2),a0
	cmpa.w	#$0002,a0
	bne	$0000137E

l00001364:
	addq.l	#$01,d7
	subq.l	#$01,d4
	addq.l	#$04,a2
	bne	$00001376

l0000136C:
	pea	$00000003
	jsr.l	$0FB2(pc)
	addq.w	#$04,a7

l00001376:
	movea.l	(a2),a0
	move.l	a0,$-0022(a5)
	bra	$00001386

l0000137E:
	movea.l	(a2),a0
	addq.l	#$02,a0
	move.l	a0,$-0022(a5)

l00001386:
	movea.l	$-0022(a5),a3
	move.l	d6,d0
	cmpi.l	#$00000009,d0
	bcc	$0000148E

l00001396:
	add.w	d0,d0
	move.w	(06,pc,d0),d0
	jmp.l	(04,pc,d0)
000013A0 00 B8 00 2E 00 80 00 94 00 58 00 E2 00 1A 00 10 .........X......
000013B0 00 24 70 01 29 40 22 E0 60 00 00 D4 70 01 29 40 .$p.)@".`...p.)@
000013C0 22 E4 60 00 00 CA 70 01 29 40 22 E8 60 00 00 C0 ".`...p.)@".`...
000013D0 48 6C 26 EC 48 6C 06 4C 2F 0B 4E BA 6A 08 4F EF Hl&.Hl.L/.N.j.O.
000013E0 00 0C 30 2C 26 EC 66 0A 48 78 00 03 4E BA 0F 34 ..0,&.f.Hx..N..4
000013F0 58 4F 42 AC 22 E4 60 00 00 96 48 6C 22 C8 48 6C XOB.".`...Hl".Hl
00001400 06 50 2F 0B 4E BA 69 DE 4F EF 00 0C 0C AC 00 00 .P/.N.i.O.......
00001410 04 00 22 C8 64 78 48 78 00 03 4E BA 0F 06 58 4F ..".dxHx..N...XO
00001420 60 6C 48 6C 22 D0 48 6C 06 54 2F 0B 4E BA 69 B6 `lHl".Hl.T/.N.i.
00001430 4F EF 00 0C 60 58 48 6C 27 10 48 6C 06 58 2F 0B O...`XHl'.Hl.X/.
00001440 4E BA 69 A2 4F EF 00 0C 30 2C 27 10 0C 40 04 00 N.i.O...0,'..@..
00001450 64 3C 39 7C 04 00 27 10 60 34 48 6C 22 DC 48 6C d<9|..'.`4Hl".Hl
00001460 06 5C 2F 0B 4E BA 69 7E 4F EF 00 0C 20 2C 22 DC .\/.N.i~O... ,".
00001470 67 06 72 02 B0 81 6F 16 48 78 00 03 4E BA 0E A4 g.r...o.Hx..N...
00001480 58 4F 60 0A 20 4B 43 EC 38 1C 12 D8 66 FC       XO`. KC.8...f. 

l0000148E:
	addq.l	#$01,d7
	subq.l	#$01,d4
	addq.l	#$04,a2

l00001494:
	movea.l	(a2),a3
	move.b	(a3),d0
	moveq	#$+2D,d1
	cmp.b	d1,d0
	beq	$00001328

l000014A0:
	moveq	#$+2F,d1
	cmp.b	d1,d0
	beq	$00001328

l000014A8:
	move.l	$22DC(a4),d0
	subq.l	#$01,d0
	beq	$000014B6

l000014B0:
	subq.l	#$01,d0
	beq	$000014E0

l000014B4:
	bra	$00001504

l000014B6:
	pea	$0660(a4)
	pea	$38A0(a4)
	jsr.l	$66B6(pc)
	addq.w	#$08,a7
	moveq	#$+00,d0
	move.w	$2710(a4),d0
	cmpi.l	#$00008000,d0
	ble	$000014D8

l000014D2:
	move.w	#$8000,$2710(a4)

l000014D8:
	move.w	#$1000,$2712(a4)
	bra	$00001504

l000014E0:
	pea	$0662(a4)
	pea	$38A0(a4)
	jsr.l	$668C(pc)
	addq.w	#$08,a7
	move.w	$2710(a4),d0
	cmpi.w	#$1000,d0
	bls	$000014FE

l000014F8:
	move.w	#$1000,$2710(a4)

l000014FE:
	move.w	#$00FF,$2712(a4)

l00001504:
	moveq	#$+05,d0
	cmp.l	$22D8(a4),d0
	bne	$00001516

l0000150C:
	clr.l	$22E8(a4)
	moveq	#$+00,d0
	move.w	d0,$26EC(a4)

l00001516:
	tst.w	$26EC(a4)
	beq	$0000152A

l0000151C:
	pea	$0664(a4)
	pea	$38A0(a4)
	jsr.l	$6650(pc)
	addq.w	#$08,a7

l0000152A:
	tst.l	$22E8(a4)
	beq	$0000153E

l00001530:
	pea	$0666(a4)
	pea	$38A0(a4)
	jsr.l	$663C(pc)
	addq.w	#$08,a7

l0000153E:
	pea	$0668(a4)
	pea	$38A0(a4)
	jsr.l	$662E(pc)
	pea	$066E(a4)
	pea	$381C(a4)
	jsr.l	$6622(pc)
	lea	$0010(a7),a7
	tst.l	d4
	bne	$00001562

l0000155E:
	jsr.l	$0E18(pc)

l00001562:
	movea.l	(a2),a0
	moveq	#$+2E,d0
	cmp.b	(a0),d0
	bne	$000015A6

l0000156A:
	tst.b	(a0)+
	bne	$0000156A

l0000156E:
	subq.l	#$01,a0
	suba.l	(a2),a0
	move.l	a0,d0
	moveq	#$+04,d1
	cmp.l	d1,d0
	ble	$00001582

l0000157A:
	move.l	d1,-(a7)
	jsr.l	$0DA6(pc)
	addq.w	#$04,a7

l00001582:
	movea.l	(a2),a0
	lea	$0000(a4),a1

l00001588:
	move.b	(a0)+,(a1)+
	bne	$00001588

l0000158C:
	pea	$0000(a4)
	jsr.l	$0E04(pc)
	addq.w	#$04,a7
	addq.l	#$01,d7
	subq.l	#$01,d4
	bne	$000015A0

l0000159C:
	jsr.l	$0DDA(pc)

l000015A0:
	moveq	#$+00,d0
	move.l	d0,$22E0(a4)

l000015A6:
	tst.l	$22D4(a4)
	bne	$000015CC

l000015AC:
	move.l	$22D8(a4),d0
	asl.l	#$02,d0
	lea	$0476(a4),a0
	movea.l	(a0,d0),a0
	tst.b	(a0)
	beq	$000015CC

l000015BE:
	pea	$38AD(a4)
	pea	$38A0(a4)
	bsr	$000019C8
	addq.w	#$08,a7

l000015CC:
	jsr.l	$0798(pc)
	clr.w	$-001E(a5)
	moveq	#$+00,d6
	moveq	#$+00,d5
	clr.l	-(a7)
	jsr.l	$61FA(pc)
	addq.w	#$04,a7
	move.l	$22D4(a4),d1
	asl.l	#$02,d1
	move.l	$22D8(a4),d2
	asl.l	#$02,d2
	lea	$0448(a4),a0
	move.l	(a0,d2),-(a7)
	lea	$0406(a4),a0
	move.l	(a0,d1),-(a7)
	pea	$067C(a4)
	move.l	d0,$002C(a7)
	jsr.l	$4B6C(pc)
	lea	$000C(a7),a7
	move.w	$26EC(a4),d0
	beq	$00001624

l00001612:
	moveq	#$+00,d1
	move.w	d0,d1
	move.l	d1,-(a7)
	pea	$068A(a4)
	jsr.l	$6B0E(pc)
	addq.w	#$08,a7
	bra	$0000163A

l00001624:
	tst.l	$22D4(a4)
	bne	$0000163A

l0000162A:
	tst.l	$22E4(a4)
	beq	$0000163A

l00001630:
	pea	$06A0(a4)
	jsr.l	$4ADC(pc)
	addq.w	#$04,a7

l0000163A:
	pea	$06AE(a4)
	jsr.l	$4AD2(pc)
	addq.w	#$04,a7
	move.l	d7,d0
	asl.l	#$02,d0
	movea.l	$000C(a5),a0
	adda.l	d0,a0
	movea.l	a0,a2
	move.l	$-001C(a5),d7
	bra	$0000190E

l00001658:
	lea	$2714(a4),a3
	pea	$-0087(a5)
	pea	$00001000
	pea	$2714(a4)
	move.l	(a2),-(a7)
	bsr	$00001B44
	lea	$0010(a7),a7
	tst.l	d0
	bne	$00001904

l00001678:
	move.l	(a2),-(a7)
	pea	$06B2(a4)
	jsr.l	$4AF2(pc)
	pea	$00000001
	jsr.l	$73D2(pc)
	lea	$000C(a7),a7
	bra	$00001904

l00001692:
	lea	$-0087(a5),a0
	lea	$3714(a4),a1

l0000169A:
	move.b	(a0)+,(a1)+
	bne	$0000169A

l0000169E:
	move.l	a3,-(a7)
	pea	$3714(a4)
	jsr.l	$64D0(pc)
	pea	$06C2(a4)
	pea	$3714(a4)
	jsr.l	$0832(pc)
	move.l	d0,$22AC(a4)
	pea	$0000002E
	move.l	a3,-(a7)
	jsr.l	$650E(pc)
	lea	$0018(a7),a7
	tst.l	d0
	beq	$000016F0

l000016CA:
	lea	$0000(a4),a0
	movea.l	a0,a1

l000016D0:
	tst.b	(a1)+
	bne	$000016D0

l000016D4:
	subq.l	#$01,a1
	suba.l	a0,a1
	move.l	a1,-(a7)
	move.l	a0,-(a7)
	move.l	d0,-(a7)
	jsr.l	$63AA(pc)
	lea	$000C(a7),a7
	tst.l	d0
	bne	$000016F0

l000016EA:
	moveq	#$+01,d0
	move.l	d0,$22E0(a4)

l000016F0:
	moveq	#$+02,d0
	cmp.l	$22D4(a4),d0
	beq	$000016FE

l000016F8:
	tst.l	$22E0(a4)
	beq	$0000171A

l000016FE:
	lea	$-0087(a5),a0
	lea	$3798(a4),a1

l00001706:
	move.b	(a0)+,(a1)+
	bne	$00001706

l0000170A:
	pea	$06C6(a4)
	pea	$3798(a4)
	jsr.l	$6462(pc)
	addq.w	#$08,a7
	bra	$00001752

l0000171A:
	lea	$3714(a4),a0
	lea	$3798(a4),a1

l00001722:
	move.b	(a0)+,(a1)+
	bne	$00001722

l00001726:
	pea	$0000002E
	pea	$3798(a4)
	jsr.l	$649E(pc)
	addq.w	#$08,a7
	tst.l	d0
	beq	$00001744

l00001738:
	lea	$0000(a4),a0
	movea.l	d0,a1

l0000173E:
	move.b	(a0)+,(a1)+
	bne	$0000173E

l00001742:
	bra	$00001752

l00001744:
	pea	$0000(a4)
	pea	$3798(a4)
	jsr.l	$6428(pc)
	addq.w	#$08,a7

l00001752:
	pea	$06D4(a4)
	pea	$3798(a4)
	jsr.l	$0788(pc)
	addq.w	#$08,a7
	move.l	d0,$22B0(a4)
	move.l	a3,-(a7)
	moveq	#$+14,d0
	move.l	d0,-(a7)
	move.l	d0,-(a7)
	pea	$06D8(a4)
	jsr.l	$69BA(pc)
	lea	$0010(a7),a7
	move.l	$22D4(a4),d0
	tst.l	d0
	beq	$0000178A

l00001780:
	subq.l	#$01,d0
	beq	$000017B8

l00001784:
	subq.l	#$01,d0
	beq	$000017B8

l00001788:
	bra	$000017C0

l0000178A:
	pea	$06E0(a4)
	pea	$381C(a4)
	jsr.l	$0750(pc)
	move.l	d0,$22B4(a4)
	jsr.l	$0ECE(pc)
	moveq	#$+00,d7
	move.w	d0,d7
	move.l	$22B4(a4),(a7)
	jsr.l	$7252(pc)
	pea	$381C(a4)
	jsr.l	$67D2(pc)
	lea	$000C(a7),a7
	bra	$000017C0

l000017B8:
	jsr.l	$302C(pc)
	moveq	#$+00,d7
	move.w	d0,d7

l000017C0:
	move.l	$22AC(a4),-(a7)
	jsr.l	$7234(pc)
	move.l	$22B0(a4),(a7)
	jsr.l	$722C(pc)
	addq.w	#$04,a7
	moveq	#$+02,d0
	cmp.l	$22D4(a4),d0
	bne	$000017E4

l000017DA:
	pea	$3798(a4)
	jsr.l	$67A2(pc)
	addq.w	#$04,a7

l000017E4:
	moveq	#$+01,d0
	cmp.l	d0,d7
	bgt	$0000184C

l000017EA:
	move.l	$22D4(a4),d1
	beq	$000017F4

l000017F0:
	subq.l	#$01,d1
	bne	$00001866

l000017F4:
	tst.l	$22E0(a4)
	beq	$00001866

l000017FA:
	pea	$0000000D
	pea	$3714(a4)
	jsr.l	$7BB2(pc)
	addq.w	#$08,a7
	addq.l	#$01,d0
	bne	$00001816

l0000180C:
	pea	$00000009
	jsr.l	$0B12(pc)
	addq.w	#$04,a7

l00001816:
	pea	$3714(a4)
	jsr.l	$6766(pc)
	addq.w	#$04,a7
	addq.l	#$01,d0
	bne	$0000182E

l00001824:
	pea	$00000009
	jsr.l	$0AFA(pc)
	addq.w	#$04,a7

l0000182E:
	pea	$3714(a4)
	pea	$3798(a4)
	jsr.l	$6706(pc)
	addq.w	#$08,a7
	addq.l	#$01,d0
	bne	$00001866

l00001840:
	pea	$00000009
	jsr.l	$0ADE(pc)
	addq.w	#$04,a7
	bra	$00001866

l0000184C:
	pea	$3714(a4)
	jsr.l	$0A96(pc)
	move.l	d0,$22BC(a4)
	move.l	d0,$22B8(a4)
	pea	$3798(a4)
	jsr.l	$6720(pc)
	addq.w	#$08,a7

l00001866:
	move.l	$22B8(a4),d0
	move.l	$22BC(a4),d1
	cmp.l	d0,d1
	bls	$00001876

l00001872:
	move.l	d0,$22BC(a4)

l00001876:
	add.l	d0,d5
	move.l	$22BC(a4),d1
	add.l	d1,d6
	addq.w	#$01,$-001E(a5)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	bsr	$00001AC0
	addq.w	#$08,a7
	moveq	#$+00,d1
	move.w	d0,d1
	move.l	d1,d0
	move.l	d1,$001C(a7)
	moveq	#$+64,d1
	jsr.l	$78DC(pc)
	move.l	$001C(a7),d1
	move.l	d0,$0024(a7)
	move.l	d1,d0
	moveq	#$+64,d1
	jsr.l	$78CC(pc)
	move.l	d1,-(a7)
	move.l	$0028(a7),-(a7)
	move.l	$22BC(a4),-(a7)
	move.l	$22B8(a4),-(a7)
	pea	$06E4(a4)
	jsr.l	$686C(pc)
	lea	$0014(a7),a7
	moveq	#$+02,d0
	cmp.l	$22D4(a4),d0
	beq	$000018D2

l000018CE:
	tst.l	d7
	beq	$000018EA

l000018D2:
	move.l	d7,d0
	asl.l	#$02,d0
	lea	$0600(a4),a0
	move.l	(a0,d0),-(a7)
	pea	$0700(a4)
	jsr.l	$488E(pc)
	addq.w	#$08,a7
	bra	$000018F4

l000018EA:
	pea	$0708(a4)
	jsr.l	$4822(pc)
	addq.w	#$04,a7

l000018F4:
	movea.l	a3,a0

l000018F6:
	tst.b	(a0)+
	bne	$000018F6

l000018FA:
	subq.l	#$01,a0
	suba.l	a3,a0
	move.l	a0,d0
	addq.l	#$01,d0
	adda.l	d0,a3

l00001904:
	tst.b	(a3)
	bne	$00001692

l0000190A:
	subq.l	#$01,d4
	addq.l	#$04,a2

l0000190E:
	tst.l	d4
	bne	$00001658

l00001914:
	clr.l	-(a7)
	jsr.l	$5EBE(pc)
	sub.l	$0024(a7),d0
	move.l	d6,(a7)
	move.l	d5,-(a7)
	move.l	d0,$0024(a7)
	bsr	$00001AC0
	moveq	#$+00,d1
	move.w	$-001E(a5),d1
	moveq	#$+00,d2
	move.w	d0,d2
	move.l	d2,d0
	move.l	d1,$002A(a7)
	moveq	#$+64,d1
	jsr.l	$7838(pc)
	move.l	d0,$0032(a7)
	move.l	d2,d0
	moveq	#$+64,d1
	jsr.l	$782C(pc)
	move.l	$0024(a7),d0
	move.l	d1,$002E(a7)
	move.l	#$00015180,d1
	jsr.l	$781A(pc)
	move.l	d1,d0
	move.l	#$00000E10,d1
	jsr.l	$780E(pc)
	move.l	d0,$0036(a7)
	move.l	$0024(a7),d0
	move.l	#$00000E10,d1
	jsr.l	$77FC(pc)
	move.l	d1,d0
	moveq	#$+3C,d1
	jsr.l	$77F4(pc)
	move.l	d0,$003A(a7)
	move.l	$0024(a7),d0
	moveq	#$+3C,d1
	jsr.l	$77E6(pc)
	move.l	d1,(a7)
	move.l	$003A(a7),-(a7)
	move.l	$003A(a7),-(a7)
	move.l	$0036(a7),-(a7)
	move.l	$003E(a7),-(a7)
	move.l	d6,-(a7)
	move.l	d5,-(a7)
	pea	$0748(a4)
	pea	$00000009
	move.l	$004A(a7),-(a7)
	pea	$070A(a4)
	jsr.l	$6772(pc)
	moveq	#$+00,d0
	movem.l	$-00C0(a5),d2/d4-d7/a2-a3
	unlk	a5
	rts	

fn000019C8()
	link	a5,#$FFF8
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l000019D4:
	movem.l	d2-d3/a2-a3,-(a7)
	move.b	$07FB(a4),d0
	moveq	#$+00,d1
	move.b	d0,d1
	move.b	$07FA(a4),d0
	ext.w	d0
	moveq	#$+00,d2
	move.w	d0,d2
	asl.l	#$08,d2
	add.l	d1,d2
	move.b	$07F9(a4),d0
	moveq	#$+00,d1
	move.b	d0,d1
	move.b	$07F8(a4),d0
	ext.w	d0
	moveq	#$+00,d3
	move.w	d0,d3
	asl.l	#$08,d3
	add.l	d1,d3
	swap.l	d3
	clr.w	d3
	add.l	d2,d3
	moveq	#$+20,d0
	add.l	d0,d3
	move.l	d3,-(a7)
	jsr.l	$0488(pc)
	movea.l	d0,a2
	movea.l	a2,a3
	move.l	a2,(a7)
	pea	$07F4(a4)
	jsr.l	$00005C1C
	addq.w	#$08,a7
	bra	$00001A6A

l00001A28:
	movea.l	a3,a0

l00001A2A:
	tst.b	(a0)+
	bne	$00001A2A

l00001A2E:
	subq.l	#$01,a0
	suba.l	a3,a0
	move.l	a0,d0
	move.b	(01,a3,d0),d0
	movea.l	a3,a0

l00001A3A:
	tst.b	(a0)+
	bne	$00001A3A

l00001A3E:
	subq.l	#$01,a0
	suba.l	a3,a0
	move.l	a0,d1
	move.b	(02,a3,d1),d1
	movea.l	a3,a0

l00001A4A:
	tst.b	(a0)+
	bne	$00001A4A

l00001A4E:
	subq.l	#$01,a0
	suba.l	a3,a0
	moveq	#$+00,d2
	move.b	d1,d2
	move.b	d0,d1
	ext.w	d1
	moveq	#$+00,d0
	move.w	d1,d0
	asl.l	#$08,d0
	add.l	d2,d0
	move.l	a0,d1
	add.l	d1,d0
	addq.l	#$03,d0
	adda.l	d0,a3

l00001A6A:
	movea.l	$0008(a5),a0
	movea.l	a3,a1

l00001A70:
	move.b	(a0)+,d0
	cmp.b	(a1)+,d0
	bne	$00001A28

l00001A76:
	tst.b	d0
	bne	$00001A70

l00001A7A:
	bne	$00001A28

l00001A7C:
	movea.l	a3,a0

l00001A7E:
	tst.b	(a0)+
	bne	$00001A7E

l00001A82:
	subq.l	#$01,a0
	suba.l	a3,a0
	move.l	a0,d0
	addq.l	#$01,d0
	movea.l	a3,a0
	adda.l	d0,a0
	move.b	$0001(a0),d0
	moveq	#$+00,d1
	move.b	d0,d1
	move.b	(a0),d0
	ext.w	d0
	moveq	#$+00,d2
	move.w	d0,d2
	asl.l	#$08,d2
	add.l	d1,d2
	addq.l	#$02,d2
	move.l	d2,-(a7)
	move.l	$000C(a5),-(a7)
	move.l	a0,-(a7)
	jsr.l	$6838(pc)
	move.l	a2,(a7)
	jsr.l	$0416(pc)
	movem.l	$-0018(a5),d2-d3/a2-a3
	unlk	a5
	rts	

fn00001AC0()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00001AC8:
	movem.l	d6-d7,-(a7)
	move.l	$000C(a7),d7
	move.l	$0010(a7),d6
	tst.l	d7
	beq	$00001ADC

l00001AD8:
	tst.l	d6
	bne	$00001AE0

l00001ADC:
	moveq	#$+00,d0
	bra	$00001AF4

l00001AE0:
	move.l	d7,d0
	sub.l	d6,d0
	move.l	#$00002710,d1
	jsr.l	$766A(pc)
	move.l	d7,d1
	jsr.l	$76B6(pc)

l00001AF4:
	movem.l	(a7)+,d6-d7
	rts	

fn00001AFA()
	link	a5,#$FFFC
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00001B06:
	movem.l	d7/a2-a3,-(a7)
	movea.l	$0018(a7),a3
	move.b	$001F(a7),d7
	move.l	d7,d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$5CA2(pc)
	addq.w	#$04,a7
	movea.l	a3,a2
	move.b	d0,$000C(a7)
	bra	$00001B2A

l00001B28:
	addq.l	#$01,a2

l00001B2A:
	move.b	(a2),d0
	beq	$00001B36

l00001B2E:
	move.b	$000C(a7),d1
	cmp.b	d1,d0
	bne	$00001B28

l00001B36:
	move.l	a2,d0
	move.l	a3,d1
	sub.l	d1,d0
	movem.l	(a7)+,d7/a2-a3
	unlk	a5
	rts	

fn00001B44()
	link	a5,#$FFD8
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00001B50:
	movem.l	d5-d7/a2-a3,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a2
	move.l	$0010(a5),d7
	lea	$00009D54,a0
	lea	$-0027(a5),a1
	move.b	(a0)+,(a1)+
	move.b	(a0)+,(a1)+
	pea	$-0027(a5)
	move.l	$0014(a5),-(a7)
	move.l	a3,-(a7)
	bsr	$00001BC2
	lea	$000C(a7),a7
	tst.l	d0
	beq	$00001BBA

l00001B84:
	moveq	#$+00,d6

l00001B86:
	lea	$-0027(a5),a0
	movea.l	a0,a1

l00001B8C:
	tst.b	(a1)+
	bne	$00001B8C

l00001B90:
	subq.l	#$01,a1
	suba.l	a0,a1
	move.l	a1,d0
	move.l	d0,d5
	addq.l	#$01,d5
	sub.l	d5,d7
	blt	$00001BB6

l00001B9E:
	addq.l	#$01,d6
	movea.l	a2,a1

l00001BA2:
	move.b	(a0)+,(a1)+
	bne	$00001BA2

l00001BA6:
	adda.l	d5,a2
	pea	$-0027(a5)
	bsr	$00001C2A
	addq.w	#$04,a7
	tst.l	d0
	bne	$00001B86

l00001BB6:
	clr.b	(a2)
	move.l	d6,d0

l00001BBA:
	movem.l	(a7)+,d5-d7/a2-a3
	unlk	a5
	rts	

fn00001BC2()
	link	a5,#$0000
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00001BCE:
	movem.l	a2-a3,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a2
	move.l	a3,-(a7)
	move.l	a2,-(a7)
	jsr.l	$6152(pc)
	addq.w	#$08,a7
	tst.l	d0
	beq	$00001BFC

l00001BE8:
	moveq	#$+3A,d1
	cmp.b	(-01,a2,d0),d1
	beq	$00001BFC

l00001BF0:
	pea	$074A(a4)
	move.l	a2,-(a7)
	jsr.l	$5F7E(pc)
	addq.w	#$08,a7

l00001BFC:
	clr.l	-(a7)
	move.l	a3,-(a7)
	pea	$3C20(a4)
	jsr.l	$7004(pc)
	lea	$000C(a7),a7
	tst.l	d0
	beq	$00001C14

l00001C10:
	moveq	#$+00,d0
	bra	$00001C22

l00001C14:
	lea	$3C28(a4),a0
	movea.l	$0010(a5),a1

l00001C1C:
	move.b	(a0)+,(a1)+
	bne	$00001C1C

l00001C20:
	moveq	#$+01,d0

l00001C22:
	movem.l	(a7)+,a2-a3
	unlk	a5
	rts	

fn00001C2A()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00001C32:
	move.l	a3,-(a7)
	movea.l	$0008(a7),a3
	pea	$3C20(a4)
	jsr.l	$71AC(pc)
	addq.w	#$04,a7
	tst.l	d0
	beq	$00001C4A

l00001C46:
	moveq	#$+00,d0
	bra	$00001C56

l00001C4A:
	lea	$3C28(a4),a0
	movea.l	a3,a1

l00001C50:
	move.b	(a0)+,(a1)+
	bne	$00001C50

l00001C54:
	moveq	#$+01,d0

l00001C56:
	movea.l	(a7)+,a3
	rts	
00001C5A                               00 00 4E F9 00 00           ..N...
00001C60 91 74 4E F9 00 00 93 B4 4E F9 00 00 91 A6 70 61 .tN.....N.....pa
00001C70 BF EC 20 1C 65 00 5A 76 48 E7 03 00 3E 2F 00 0E .. .e.ZvH...>/..
00001C80 7C 01 60 02 52 46 E2 4F 66 FA 20 06 4C DF 00 C0 |.`.RF.Of. .L...
00001C90 4E 75 4E 55 FF F8 BF EC 20 1C 65 00 5A 50 48 E7 NuNU.... .e.ZPH.
00001CA0 01 30 26 6D 00 08 2E 2D 00 10 4A 87 67 5E 0C 87 .0&m...-..J.g^..
00001CB0 00 00 FF FF 63 08 20 3C 00 00 FF FF 60 02 20 07 ....c. <....`. .
00001CC0 2F 00 61 00 01 D4 58 4F 24 40 60 36 0C 87 00 00 /.a...XO$@`6....
00001CD0 FF FF 63 08 20 3C 00 00 FF FF 60 02 20 07 2F 0B ..c. <....`. ./.
00001CE0 2F 00 2F 0A 2F 40 00 18 61 00 05 36 2E AD 00 0C /././@..a..6....
00001CF0 2F 2F 00 18 2F 0A 61 00 05 66 4F EF 00 14 9E AF //../.a..fO.....
00001D00 00 0C 4A 87 66 C6 2F 0A 61 00 01 BE 4C ED 0C 80 ..J.f./.a...L...
00001D10 FF EC 4E 5D 4E 75 BF EC 20 1C 65 00 59 D0 48 E7 ..N]Nu.. .e.Y.H.
00001D20 01 30 26 6F 00 10 24 6F 00 14 48 6C 07 50 2F 0B .0&o..$o..Hl.P/.
00001D30 61 00 01 B0 26 40 2E 8B 61 00 05 62 50 4F 2E 00 a...&@..a..bPO..
00001D40 60 14 2F 0B 61 00 01 D2 72 00 12 00 2E 8A 2F 01 `./.a...r...../.
00001D50 61 00 03 F8 50 4F 20 07 53 87 4A 80 66 E4 4C DF a...PO .S.J.f.L.
00001D60 0C 80 4E 75                                     ..Nu           

fn00001D64()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00001D6C:
	movem.l	d4-d7,-(a7)
	moveq	#$+00,d6
	bra	$00001DAA

l00001D74:
	move.l	d6,d4
	moveq	#$+08,d5
	bra	$00001D96

l00001D7A:
	moveq	#$+00,d0
	move.w	d4,d0
	asr.l	#$01,d0
	move.l	d0,d7
	btst	#$0000,d4
	beq	$00001D90

l00001D88:
	move.l	d7,d0
	eori.w	#$A001,d0
	bra	$00001D92

l00001D90:
	move.l	d7,d0

l00001D92:
	move.l	d0,d4
	subq.w	#$01,d5

l00001D96:
	tst.w	d5
	bne	$00001D7A

l00001D9A:
	moveq	#$+00,d0
	move.w	d6,d0
	add.l	d0,d0
	lea	$22EC(a4),a0
	move.w	d4,(a4,d0)
	addq.w	#$01,d6

l00001DAA:
	cmpi.w	#$0100,d6
	bcs	$00001D74

l00001DB0:
	movem.l	(a7)+,d4-d7
	rts	
00001DB6                   BF EC 20 1C 65 00 59 30 48 E7       .. .e.Y0H.
00001DC0 33 10 26 6F 00 18 3E 2F 00 1E 3C 2F 00 22 60 24 3.&o..>/..</."`$
00001DD0 20 06 E0 48 72 00 12 1B 24 06 76 00 16 02 B3 83  ..Hr...$.v.....
00001DE0 72 00 46 01 C6 81 D6 83 41 EC 22 EC 32 30 38 00 r.F.....A.".208.
00001DF0 B1 41 2C 01 20 07 2E 00 53 47 4A 40 66 D2 20 06 .A,. ...SGJ@f. .
00001E00 4C DF 08 CC 4E 75 4E 55 FF FC BF EC 20 1C 65 00 L...NuNU.... .e.
00001E10 58 DC 48 E7 07 30 26 6F 00 20 2E 2F 00 24 7A 00 X.H..0&o. ./.$z.
00001E20 2F 3C 00 00 FF FF 61 00 00 70 24 40 2E 8B 4E BA /<....a..p$@..N.
00001E30 68 74 58 4F 2C 00 60 40 0C 87 00 00 FF FF 63 08 htXO,.`@......c.
00001E40 20 3C 00 00 FF FF 60 02 20 07 2F 0B 2F 00 2F 0A  <....`. ./././.
00001E50 2F 40 00 20 61 00 03 CA 20 2F 00 20 72 00 32 00 /@. a... /. r.2.
00001E60 70 00 30 05 2E 80 2F 01 2F 0A 61 00 FF 4A 4F EF p.0..././.a..JO.
00001E70 00 14 2A 00 9E AF 00 14 4A 87 66 BC 42 A7 2F 06 ..*.....J.f.B./.
00001E80 2F 0B 4E BA 68 98 2E 8A 61 00 00 3E 20 05 4C ED /.N.h...a..> .L.
00001E90 0C E0 FF E8 4E 5D 4E 75                         ....N]Nu       

fn00001E98()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00001EA0:
	movem.l	d7/a3,-(a7)
	move.l	$000C(a7),d7
	move.l	d7,-(a7)
	jsr.l	$6492(pc)
	addq.w	#$04,a7
	movea.l	d0,a3
	tst.l	d0
	bne	$00001EC0

l00001EB6:
	pea	$00000005
	bsr	$00002322
	addq.w	#$04,a7

l00001EC0:
	move.l	a3,d0
	movem.l	(a7)+,d7/a3
	rts	

fn00001EC8()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00001ED0:
	move.l	a3,-(a7)
	movea.l	$0008(a7),a3
	move.l	a3,-(a7)
	jsr.l	$68C8(pc)
	addq.w	#$04,a7
	movea.l	(a7)+,a3
	rts	

fn00001EE2()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00001EEA:
	movem.l	a2-a3,-(a7)
	movea.l	$000C(a7),a3
	movea.l	$0010(a7),a2
	move.l	a2,-(a7)
	move.l	a3,-(a7)
	jsr.l	$6936(pc)
	addq.w	#$08,a7
	movea.l	d0,a3
	tst.l	d0
	bne	$00001F10

l00001F06:
	pea	$00000006
	bsr	$00002322
	addq.w	#$04,a7

l00001F10:
	move.l	a3,d0
	movem.l	(a7)+,a2-a3
	rts	
00001F18                         BF EC 20 1C 65 00 57 CE         .. .e.W.
00001F20 48 E7 01 10 26 6F 00 0C 53 AB 00 08 6D 0E 20 6B H...&o..S...m. k
00001F30 00 04 52 AB 00 04 70 00 10 10 60 08 2F 0B 4E BA ..R...p...`./.N.
00001F40 55 E4 58 4F 2E 00 52 80 66 0A 48 78 00 07 61 00 U.XO..R.f.Hx..a.
00001F50 03 D2 58 4F 20 07 4C DF 08 80 4E 75 4E 55 FF FC ..XO .L...NuNU..
00001F60 BF EC 20 1C 65 00 57 86 2F 0B 26 6F 00 10 2F 0B .. .e.W./.&o../.
00001F70 61 A6 2E 8B 1F 40 00 08 61 9E 58 4F 72 00 12 00 a....@..a.XOr...
00001F80 70 00 10 2F 00 04 E1 40 D0 41 26 5F 4E 5D 4E 75 p../...@.A&_N]Nu
00001F90 4E 55 FF FC BF EC 20 1C 65 00 57 52 48 E7 20 10 NU.... .e.WRH. .
00001FA0 26 6F 00 14 2F 0B 61 B4 2E 8B 3F 40 00 0C 61 AC &o../.a...?@..a.
00001FB0 58 4F 72 00 32 00 30 2F 00 08 74 00 34 00 48 42 XOr.2.0/..t.4.HB
00001FC0 42 42 D4 81 20 02 4C DF 08 04 4E 5D 4E 75 BF EC BB.. .L...N]Nu..
00001FD0 20 1C 65 00 57 18 48 E7 01 10 26 6F 00 0C 53 AB  .e.W.H...&o..S.
00001FE0 00 08 6D 0E 20 6B 00 04 52 AB 00 04 70 00 10 10 ..m. k..R...p...
00001FF0 60 08 2F 0B 4E BA 55 2E 58 4F 2E 00 52 80 66 0A `./.N.U.XO..R.f.
00002000 48 78 00 07 61 00 03 1C 58 4F 48 78 00 01 48 78 Hx..a...XOHx..Hx
00002010 FF FF 2F 0B 4E BA 67 06 4F EF 00 0C 20 07 4C DF ../.N.g.O... .L.
00002020 08 80 4E 75 4E 55 FF FC BF EC 20 1C 65 00 56 BE ..NuNU.... .e.V.
00002030 2F 0B 26 6F 00 10 2F 0B 61 00 FE DE 2E 8B 1F 40 /.&o../.a......@
00002040 00 08 61 00 FE D4 48 78 00 01 48 78 FF FE 2F 0B ..a...Hx..Hx../.
00002050 1F 40 00 15 4E BA 66 C6 4F EF 00 10 70 00 10 2F .@..N.f.O...p../
00002060 00 05 72 00 12 2F 00 04 E1 41 D2 40 20 01 26 5F ..r../...A.@ .&_
00002070 4E 5D 4E 75 4E 55 FF FC BF EC 20 1C 65 00 56 6E N]NuNU.... .e.Vn
00002080 48 E7 20 10 26 6F 00 14 2F 0B 61 00 FE D0 2E 8B H. .&o../.a.....
00002090 3F 40 00 0C 61 00 FE C6 48 78 00 01 48 78 FF FC ?@..a...Hx..Hx..
000020A0 2F 0B 3F 40 00 1A 4E BA 66 74 4F EF 00 10 30 2F /.?@..N.ftO...0/
000020B0 00 0A 72 00 32 00 30 2F 00 08 74 00 34 00 48 42 ..r.2.0/..t.4.HB
000020C0 42 42 D4 81 20 02 4C DF 08 04 4E 5D 4E 75 4E 55 BB.. .L...N]NuNU
000020D0 FF FC BF EC 20 1C 65 00 56 14 48 E7 20 10 26 6F .... .e.V.H. .&o
000020E0 00 14 2F 0B 61 00 FE 32 2E 8B 1F 40 00 0C 61 00 ../.a..2...@..a.
000020F0 FE 28 58 4F 72 00 12 2F 00 08 74 00 14 00 E1 42 .(XOr../..t....B
00002100 D4 41 20 02 4C DF 08 04 4E 5D 4E 75 BF EC 20 1C .A .L...N]Nu.. .
00002110 65 00 55 DA 48 E7 01 10 3E 2F 00 0E 26 6F 00 10 e.U.H...>/..&o..
00002120 70 00 30 07 E0 80 72 00 12 00 2F 0B 2F 01 61 00 p.0...r..././.a.
00002130 00 1A 20 07 72 00 12 00 2E 8B 2F 01 61 00 00 0C .. .r...../.a...
00002140 4F EF 00 0C 4C DF 08 80 4E 75 BF EC 20 1C 65 00 O...L...Nu.. .e.
00002150 55 9C 48 E7 01 10 1E 2F 00 0F 26 6F 00 10 53 AB U.H..../..&o..S.
00002160 00 0C 6D 10 20 6B 00 04 52 AB 00 04 10 87 70 00 ..m. k..R.....p.
00002170 10 07 60 0E 70 00 10 07 2F 0B 2F 00 4E BA 51 0E ..`.p..././.N.Q.
00002180 50 4F 52 80 66 0A 48 78 00 08 61 00 01 96 58 4F POR.f.Hx..a...XO
00002190 4C DF 08 80 4E 75 BF EC 20 1C 65 00 55 50 48 E7 L...Nu.. .e.UPH.
000021A0 01 10 2E 2F 00 0C 26 6F 00 10 20 07 42 40 48 40 .../..&o.. .B@H@
000021B0 72 00 32 00 2F 0B 2F 01 61 00 FF 52 20 07 72 00 r.2././.a..R .r.
000021C0 32 00 2E 8B 2F 01 61 00 FF 44 4F EF 00 0C 4C DF 2.../.a..DO...L.
000021D0 08 80 4E 75 BF EC 20 1C 65 00 55 12 48 E7 01 10 ..Nu.. .e.U.H...
000021E0 3E 2F 00 0E 26 6F 00 10 70 00 30 07 72 00 46 01 >/..&o..p.0.r.F.
000021F0 C0 81 72 00 12 00 2F 0B 2F 01 61 00 FF 4E 70 00 ..r..././.a..Np.
00002200 30 07 E0 80 72 00 46 01 C0 81 72 00 12 00 2E 8B 0...r.F...r.....
00002210 2F 01 61 00 FF 36 4F EF 00 0C 4C DF 08 80 4E 75 /.a..6O...L...Nu
00002220 BF EC 20 1C 65 00 54 C6 48 E7 01 30 26 6F 00 10 .. .e.T.H..0&o..
00002230 2E 2F 00 14 24 6F 00 18 2F 0A 2F 07 48 78 00 01 ./..$o.././.Hx..
00002240 2F 0B 4E BA 65 84 4F EF 00 10 B0 87 67 0A 48 78 /.N.e.O.....g.Hx
00002250 00 07 61 00 00 CE 58 4F 4C DF 0C 80 4E 75 BF EC ..a...XOL...Nu..
00002260 20 1C 65 00 54 88 48 E7 01 30 26 6F 00 10 2E 2F  .e.T.H..0&o.../
00002270 00 14 24 6F 00 18 2F 0A 2F 07 48 78 00 01 2F 0B ..$o.././.Hx../.
00002280 4E BA 63 B6 4F EF 00 10 B0 87 67 0A 48 78 00 08 N.c.O.....g.Hx..
00002290 61 00 00 90 58 4F 4C DF 0C 80 4E 75             a...XOL...Nu   

fn0000229C()
	link	a5,#$FFF8
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l000022A8:
	move.l	a3,-(a7)
	movea.l	$0014(a7),a3
	move.l	a3,-(a7)
	jsr.l	$63F4(pc)
	pea	$00000002
	clr.l	-(a7)
	move.l	a3,-(a7)
	move.l	d0,$0014(a7)
	jsr.l	$645C(pc)
	move.l	a3,(a7)
	jsr.l	$63DE(pc)
	clr.l	(a7)
	move.l	$0014(a7),-(a7)
	move.l	a3,-(a7)
	move.l	d0,$0020(a7)
	jsr.l	$6446(pc)
	move.l	$0020(a7),d0
	movea.l	$-000C(a5),a3
	unlk	a5
	rts	

fn000022E6()
	link	a5,#$FFF8
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l000022F2:
	move.l	a3,-(a7)
	movea.l	$0014(a7),a3
	pea	$0754(a4)
	move.l	a3,-(a7)
	bsr	$00001EE2
	move.l	d0,(a7)
	move.l	d0,$000C(a7)
	bsr	$0000229C
	move.l	$000C(a7),(a7)
	move.l	d0,$0010(a7)
	jsr.l	$66E6(pc)
	move.l	$0010(a7),d0
	movea.l	$-000C(a5),a3
	unlk	a5
	rts	

fn00002322()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l0000232A:
	move.l	d7,-(a7)
	move.w	$000A(a7),d7
	move.l	$22B4(a4),-(a7)
	jsr.l	$66C4(pc)
	move.l	$22B0(a4),(a7)
	jsr.l	$66BC(pc)
	pea	$381C(a4)
	jsr.l	$4C44(pc)
	pea	$3798(a4)
	jsr.l	$4C3C(pc)
	moveq	#$+00,d0
	move.w	d7,d0
	asl.l	#$02,d0
	lea	$051E(a4),a0
	move.l	(a0,d0),(a7)
	pea	$0758(a4)
	jsr.l	$3E0E(pc)
	pea	$00000001
	jsr.l	$66EE(pc)
	lea	$0014(a7),a7
	move.l	(a7)+,d7
	rts	

fn00002376()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l0000237E:
	pea	$00D5(a4)
	pea	$075E(a4)
	jsr.l	$3DEA(pc)
	clr.l	(a7)
	jsr.l	$66CC(pc)
	addq.w	#$08,a7
	rts	

fn00002394()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l0000239C:
	move.l	a3,-(a7)
	movea.l	$0008(a7),a3
	bra	$000023B4

l000023A4:
	move.b	(a3),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$5410(pc)
	addq.w	#$04,a7
	move.b	d0,(a3)+

l000023B4:
	tst.b	(a3)
	bne	$000023A4

l000023B8:
	movea.l	(a7)+,a3
	rts	
000023BC                                     BF EC 20 1C             .. .
000023C0 65 00 53 2A 48 E7 01 10 26 6F 00 0C 2E 2F 00 10 e.S*H...&o.../..
000023D0 20 07 72 0C 4E BA 6D 7E D7 C0 60 12 70 00 26 80  .r.N.m~..`.p.&.
000023E0 37 7C FF FF 00 04 27 40 00 06 42 6B 00 0A 20 07 7|....'@..Bk.. .
000023F0 53 87 D6 FC FF F4 4A 80 66 E2 4C DF 08 80 4E 75 S.....J.f.L...Nu
00002400 4E 55 FF EC BF EC 20 1C 65 00 52 E2 48 E7 2F 30 NU.... .e.R.H./0
00002410 26 6D 00 08 2E 2D 00 0C 78 00 7A 00 60 14 20 05 &m...-..x.z.`. .
00002420 C0 FC 00 0C 4A B3 08 00 67 06 52 44 3B 45 FF F2 ....J...g.RD;E..
00002430 52 45 70 00 30 05 B0 87 6D E4 4A 44 67 00 00 EA REp.0...m.JDg...
00002440 70 01 B8 40 66 00 00 CA 32 2D FF F2 C2 FC 00 0C p..@f...2-......
00002450 52 73 18 0A 60 00 00 D2 20 2C 3D 24 72 0C 4E BA Rs..`... ,=$r.N.
00002460 6C F4 2F 40 00 1C 20 2C 3D 28 72 0C 4E BA 6C E6 l./@.. ,=(r.N.l.
00002470 22 33 08 00 24 2F 00 1C D3 B3 28 00 20 2C 3D 28 "3..$/....(. ,=(
00002480 72 0C 4E BA 6C D0 42 B3 08 00 20 2C 3D 24 72 0C r.N.l.B... ,=$r.
00002490 4E BA 6C C2 52 73 08 0A 60 14 70 00 30 2A 00 04 N.l.Rs..`.p.0*..
000024A0 29 40 3D 24 72 0C 4E BA 6C AC 52 73 08 0A 20 2C )@=$r.N.l.Rs.. ,
000024B0 3D 24 72 0C 4E BA 6C 9E 2C 00 24 4B D5 C6 70 00 =$r.N.l.,.$K..p.
000024C0 30 2A 00 04 0C 80 00 00 FF FF 66 CE 20 2C 3D 28 0*........f. ,=(
000024D0 37 80 68 04 72 0C 4E BA 6C 7C 52 73 08 0A 60 14 7.h.r.N.l|Rs..`.
000024E0 70 00 30 2A 00 04 29 40 3D 28 72 0C 4E BA 6C 66 p.0*..)@=(r.N.lf
000024F0 52 73 08 0A 20 2C 3D 28 72 0C 4E BA 6C 58 24 4B Rs.. ,=(r.N.lX$K
00002500 D5 C0 70 00 30 2A 00 04 0C 80 00 00 FF FF 66 D0 ..p.0*........f.
00002510 2F 07 2F 0B 61 00 00 D2 50 4F 4A 80 66 00 FF 3A /./.a...POJ.f..:
00002520 2F 07 2F 0B 61 00 00 0C 4C ED 0C F4 FF D0 4E 5D /./.a...L.....N]
00002530 4E 75 4E 55 FF F0 BF EC 20 1C 65 00 51 B0 48 E7 NuNU.... .e.Q.H.
00002540 0F 10 26 6D 00 08 2E 2D 00 0C 7C 00 2A 3C 80 00 ..&m...-..|.*<..
00002550 00 00 78 01 60 4C 42 6D FF F4 60 38 30 2D FF F4 ..x.`LBm..`80-..
00002560 C0 FC 00 0C 2F 40 00 14 32 33 08 0A B2 44 66 20 ..../@..23...Df 
00002570 20 06 22 05 4E BA 6C 30 72 00 32 04 2F 01 2F 00  .".N.l0r.2././.
00002580 61 00 00 2E 50 4F 22 2F 00 14 27 80 18 06 DC 85 a...PO"/..'.....
00002590 52 6D FF F4 70 00 30 2D FF F4 B0 87 6D BE 52 44 Rm..p.0-....m.RD
000025A0 E2 8D 70 10 B8 40 63 AE 4C DF 08 F0 4E 5D 4E 75 ..p..@c.L...N]Nu
000025B0 BF EC 20 1C 65 00 51 36 48 E7 07 00 2E 2F 00 10 .. .e.Q6H..../..
000025C0 3C 2F 00 16 7A 00 60 0E DA 85 08 07 00 00 67 04 </..z.`.......g.
000025D0 08 C5 00 00 E2 8F 20 06 2C 00 53 46 4A 40 66 E8 ...... .,.SFJ@f.
000025E0 20 05 4C DF 00 E0 4E 75 4E 55 FF F0 BF EC 20 1C  .L...NuNU.... .
000025F0 65 00 50 FA 48 E7 0F 10 26 6D 00 08 2E 2D 00 0C e.P.H...&m...-..
00002600 7C FF 7A FF 42 6D FF F2 60 3C 30 2D FF F2 22 00 |.z.Bm..`<0-..".
00002610 C2 FC 00 0C 28 33 18 00 4A 84 67 26 B8 85 64 14 ....(3..J.g&..d.
00002620 2C 05 29 6C 3D 24 3D 28 2A 04 72 00 32 00 29 41 ,.)l=$=(*.r.2.)A
00002630 3D 24 60 0E B8 86 64 0A 2C 04 72 00 32 00 29 41 =$`...d.,.r.2.)A
00002640 3D 28 52 6D FF F2 70 00 30 2D FF F2 B0 87 6D BA =(Rm..p.0-....m.
00002650 70 FF BA 80 67 04 BC 80 66 04 70 00 60 02 70 01 p...g...f.p.`.p.
00002660 4C DF 08 F0 4E 5D 4E 75                         L...N]Nu       

fn00002668()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l00002670:
	move.l	d7,-(a7)
	move.l	$22AC(a4),-(a7)
	jsr.l	$-03DA(pc)
	addq.w	#$04,a7
	move.l	d0,$22B8(a4)
	moveq	#$+12,d1
	cmp.l	d1,d0
	bhi	$0000268C

l00002686:
	moveq	#$+02,d0
	bra	$0000275C

l0000268C:
	moveq	#$+00,d0
	move.l	d0,-(a7)
	move.l	d0,-(a7)
	move.l	$22AC(a4),-(a7)
	jsr.l	$6086(pc)
	moveq	#$+00,d0
	move.l	d0,(a7)
	move.l	d0,-(a7)
	move.l	$22B0(a4),-(a7)
	jsr.l	$6078(pc)
	lea	$0014(a7),a7
	move.l	$22D8(a4),d0
	cmpi.l	#$00000006,d0
	bcc	$00002738

l000026BA:
	add.w	d0,d0
	move.w	(06,pc,d0),d0
	jmp.l	(04,pc,d0)
000026C4             00 0A 00 34 00 54 00 5C 00 64 00 6C     ...4.T.\.d.l
000026D0 2F 2C 22 AC 4E BA F9 9E 58 4F E0 88 0C 80 00 52 /,".N...XO.....R
000026E0 4E 43 66 04 70 03 60 74 20 2C 22 B8 2F 00 2F 00 NCf.p.`t ,"././.
000026F0 61 00 11 2A 50 4F 7E 00 60 3E 2F 2C 22 AC 4E BA a..*PO~.`>/,".N.
00002700 F9 24 58 4F 0C 40 4D 5A 66 08 61 00 00 54 2E 00 .$XO.@MZf.a..T..
00002710 60 26 61 00 06 A2 2E 00 60 1E 61 00 07 72 2E 00 `&a.....`.a..r..
00002720 60 16 61 00 09 56 2E 00 60 0E 61 00 0F 3C 2E 00 `.a..V..`.a..<..
00002730 60 06 61 00 0F 92 2E 00                         `.a.....       

l00002738:
	moveq	#$+01,d0
	cmp.w	d0,d7
	bls	$00002742

l0000273E:
	move.l	d7,d0
	bra	$0000275C

l00002742:
	move.l	$22B0(a4),-(a7)
	jsr.l	$-04AA(pc)
	addq.w	#$04,a7
	move.l	d0,$22BC(a4)
	cmp.l	$22B8(a4),d0
	bcs	$0000275A

l00002756:
	moveq	#$+02,d0
	bra	$0000275C

l0000275A:
	move.l	d7,d0

l0000275C:
	move.l	(a7)+,d7
	rts	
00002760 4E 55 FF CC BF EC 20 1C 65 00 4F 82 48 E7 3F 10 NU.... .e.O.H.?.
00002770 48 78 00 03 48 6C 07 92 48 6C 38 A0 4E BA 53 0A Hx..Hl..Hl8.N.S.
00002780 4F EF 00 0C 4A 80 67 20 48 78 00 03 48 6C 07 96 O...J.g Hx..Hl..
00002790 48 6C 38 A0 4E BA 52 CE 48 6C 38 AD 48 6C 38 A0 Hl8.N.R.Hl8.Hl8.
000027A0 4E BA F2 26 4F EF 00 14 70 00 10 2C 38 AE 72 00 N..&O...p..,8.r.
000027B0 12 2C 38 AD 74 00 34 01 E1 82 D4 80 20 2C 22 B8 .,8.t.4..... ,".
000027C0 B0 82 62 06 70 02 60 00 05 BA 0C AC 00 00 7F F0 ..b.p.`.........
000027D0 22 C8 63 08 29 7C 00 00 7F F0 22 C8 42 A7 48 78 ".c.)|....".B.Hx
000027E0 00 02 2F 2C 22 AC 4E BA 5F 34 2E AC 22 AC 4E BA ../,".N._4..".N.
000027F0 F8 DE 2E AC 22 AC 3F 40 00 28 4E BA F8 D2 4F EF ....".?@.(N...O.
00002800 00 0C 3B 40 FF E6 30 2F 00 1C 67 04 53 6D FF E6 ..;@..0/..g.Sm..
00002810 72 00 32 00 70 00 30 2D FF E6 E1 80 D0 80 D0 81 r.2.p.0-........
00002820 2E 00 20 2C 22 B8 BE 80 63 02 2E 00 2F 2C 22 AC .. ,"...c.../,".
00002830 4E BA F8 9C 2E AC 22 AC 3F 40 00 34 4E BA F8 90 N.....".?@.4N...
00002840 2E AC 22 AC 3F 40 00 20 4E BA F8 84 2E AC 22 AC ..".?@. N.....".
00002850 3F 40 00 32 4E BA F8 78 2E AC 22 AC 3B 40 FF E4 ?@.2N..x..".;@..
00002860 4E BA F8 6C 2E AC 22 AC 3F 40 00 30 4E BA F8 60 N..l..".?@.0N..`
00002870 2E AC 22 AC 3F 40 00 2E 4E BA F8 54 2E AC 22 AC ..".?@..N..T..".
00002880 4E BA F8 4C 2E AC 22 AC 3F 40 00 2C 4E BA F8 40 N..L..".?@.,N..@
00002890 2E AC 22 AC 3F 40 00 2A 4E BA F8 34 2E AC 22 AC ..".?@.*N..4..".
000028A0 3F 40 00 24 4E BA F8 28 2E AC 22 B0 48 78 00 20 ?@.$N..(..".Hx. 
000028B0 48 6C 38 AF 4E BA F9 A8 30 2F 00 28 72 00 32 00 Hl8.N...0/.(r.2.
000028C0 E9 81 28 07 98 81 42 97 2F 01 2F 2C 22 AC 2F 41 ..(...B././,"./A
000028D0 00 36 4E BA 5E 48 2E 84 2F 04 61 00 0F 40 4F EF .6N.^H../.a..@O.
000028E0 00 18 60 10 2F 2C 22 B0 42 A7 4E BA F8 5E 50 4F ..`./,".B.N..^PO
000028F0 52 AC 22 BC 70 0F C0 AC 22 BC 66 E8 2F 2C 22 B0 R.".p...".f./,".
00002900 4E BA 5D A2 72 00 12 2C 38 AE 74 00 14 2C 38 AD N.].r..,8.t..,8.
00002910 76 00 36 02 E1 83 D6 81 72 20 96 81 2E AC 22 B0 v.6.....r ....".
00002920 2F 03 48 6C 38 CF 2F 40 00 28 4E BA F9 32 4F EF /.Hl8./@.(N..2O.
00002930 00 0C 4A 6F 00 30 67 00 01 74 2F 3C 00 00 FF FF ..Jo.0g..t/<....
00002940 4E BA F5 56 26 40 70 00 30 2F 00 24 42 97 2F 00 N..V&@p.0/.$B./.
00002950 2F 2C 22 AC 4E BA 5D C6 4F EF 00 0C 42 6D FF EA /,".N.].O...Bm..
00002960 60 36 2F 2C 22 AC 4E BA F7 66 2E AC 22 AC 3F 40 `6/,".N..f..".?@
00002970 00 24 4E BA F7 5A 58 4F 72 00 32 2D FF EA E5 81 .$N..ZXOr.2-....
00002980 34 2F 00 20 76 00 36 02 74 00 34 00 E9 82 D4 83 4/. v.6.t.4.....
00002990 27 82 18 00 52 6D FF EA 30 2F 00 30 32 2D FF EA '...Rm..0/.02-..
000029A0 B2 40 66 BE 70 00 30 2F 00 30 48 7A 03 DE 48 78 .@f.p.0/.0Hz..Hx
000029B0 00 04 2F 00 2F 0B 4E BA 56 40 4F EF 00 10 42 6D .././.N.V@O...Bm
000029C0 FF EA 60 00 00 D2 70 00 30 2D FF EA E5 80 2C 33 ..`...p.0-....,3
000029D0 08 00 2E 06 02 87 00 0F FF F0 2F 2C 22 B0 42 A7 ........../,".B.
000029E0 4E BA F7 68 20 07 E8 88 72 00 32 00 2E AC 22 B0 N..h ...r.2...".
000029F0 2F 01 4E BA F7 E0 4F EF 00 0C 42 6D FF E8 70 00 /.N...O...Bm..p.
00002A00 30 2D FF EA E5 80 22 33 08 00 92 87 2A 01 DE 85 0-...."3....*...
00002A10 70 00 46 00 BA 80 62 3A 20 07 90 86 0C 80 00 00 p.F...b: .......
00002A20 FF F0 64 2E 0C 6D 00 FF FF E8 67 26 20 05 72 00 ..d..m....g& .r.
00002A30 12 00 2F 2C 22 B0 2F 01 4E BA F7 10 50 4F 52 6D ../,"./.N...PORm
00002A40 FF E8 52 6D FF EA 30 2F 00 30 32 2D FF EA B2 40 ..Rm..0/.02-...@
00002A50 66 AC 70 00 30 2D FF E8 56 80 44 80 48 78 00 01 f.p.0-..V.D.Hx..
00002A60 2F 00 2F 2C 22 B0 4E BA 5C B4 30 2D FF E8 72 00 /./,".N.\.0-..r.
00002A70 12 00 2E AC 22 B0 2F 01 4E BA F6 D0 70 00 30 2D ...."./.N...p.0-
00002A80 FF E8 54 80 48 78 00 01 2F 00 2F 2C 22 B0 4E BA ..T.Hx.././,".N.
00002A90 5C 8C 4F EF 00 1C 30 2F 00 30 32 2D FF EA B2 40 \.O...0/.02-...@
00002AA0 66 00 FF 24 2F 0B 4E BA F4 20 58 4F 2F 2C 22 B0 f..$/.N.. XO/,".
00002AB0 42 A7 4E BA F6 96 2E AC 22 B0 4E BA 5B E8 50 4F B.N.....".N.[.PO
00002AC0 90 AF 00 1C 2A 00 08 05 00 00 67 0E 2F 2C 22 B0 ....*.....g./,".
00002AD0 42 A7 4E BA F6 76 50 4F 52 85 20 05 44 80 48 78 B.N..vPOR. .D.Hx
00002AE0 00 01 2F 00 2F 2C 22 B0 4E BA 5C 32 70 00 30 2F .././,".N.\2p.0/
00002AF0 00 34 2E AC 22 B0 2F 00 4E BA F6 DA 70 00 30 2F .4.."./.N...p.0/
00002B00 00 36 2E AC 22 B0 2F 00 4E BA F6 CA 70 00 30 2F .6.."./.N...p.0/
00002B10 00 3E 2E AC 22 B0 2F 00 4E BA F6 BA 70 00 30 2F .>.."./.N...p.0/
00002B20 00 44 2E AC 22 B0 2F 00 4E BA F6 AA 20 05 72 00 .D.."./.N... .r.
00002B30 32 00 2E AC 22 B0 2F 01 4E BA F6 9A 20 2C 22 BC 2..."./.N... ,".
00002B40 E8 88 72 00 32 00 2E AC 22 B0 2F 01 4E BA F6 86 ..r.2..."./.N...
00002B50 4F EF 00 24 20 04 90 AC 22 BC D0 AC 3D 30 2E 00 O..$ ..."...=0..
00002B60 72 40 D2 81 DE 81 4A AC 22 E8 67 08 06 87 00 00 r@....J.".g.....
00002B70 02 00 60 0E 70 01 B0 AC 22 DC 66 06 06 87 00 00 ..`.p...".f.....
00002B80 01 80 20 04 D0 AF 00 22 42 A7 2F 00 2F 2C 22 AC .. ...."B././,".
00002B90 4E BA 5B 8A 48 78 00 02 42 A7 2F 2C 22 B0 4E BA N.[.Hx..B./,".N.
00002BA0 5B 7C 2E AC 22 B0 4E BA 5A FC 2C 00 20 2C 22 B8 [|..".N.Z.,. ,".
00002BB0 90 84 90 AF 00 3A 2E 80 2F 2C 22 B0 2F 2C 22 AC .....:../,"./,".
00002BC0 4E BA F0 D0 4F EF 00 20 20 06 02 80 00 00 01 FF N...O..  .......
00002BD0 22 06 E0 89 E2 89 3F 40 00 20 3B 41 FF E6 4A 40 ".....?@. ;A..J@
00002BE0 67 04 52 6D FF E6 42 A7 48 78 00 02 2F 2C 22 B0 g.Rm..B.Hx../,".
00002BF0 4E BA 5B 2A 30 2F 00 2C 72 00 32 00 2E AC 22 B0 N.[*0/.,r.2...".
00002C00 2F 01 4E BA F5 D0 70 00 30 2D FF E6 2E AC 22 B0 /.N...p.0-....".
00002C10 2F 00 4E BA F5 C0 42 97 48 78 00 18 2F 2C 22 B0 /.N...B.Hx../,".
00002C20 4E BA 5A FA 70 00 10 2C 38 AE 72 00 12 2C 38 AD N.Z.p..,8.r..,8.
00002C30 74 00 34 01 E1 82 D4 80 70 20 94 80 70 00 30 02 t.4.....p ..p.0.
00002C40 2E AC 22 B0 2F 00 4E BA F5 8C 42 97 48 78 00 1C .."./.N...B.Hx..
00002C50 2F 2C 22 B0 4E BA 5A C6 70 00 30 2F 00 56 2E AC /,".N.Z.p.0/.V..
00002C60 22 B0 2F 00 4E BA F5 6E 70 00 30 2D FF E4 2E AC "./.N..np.0-....
00002C70 22 B0 2F 00 4E BA F5 5E 42 97 48 78 00 0A 2F 2C "./.N..^B.Hx../,
00002C80 22 B0 4E BA 5A 98 4F EF 00 38 70 00 30 2F 00 2E ".N.Z.O..8p.0/..
00002C90 E9 80 22 04 D2 80 DC 87 20 06 74 20 90 82 B2 80 .."..... .t ....
00002CA0 63 06 92 86 D2 82 DE 81 70 10 90 87 72 0F C0 81 c.......p...r...
00002CB0 22 07 D2 80 E8 89 70 00 30 01 2F 2C 22 B0 2F 00 ".....p.0./,"./.
00002CC0 2F 40 00 26 3F 41 00 24 4E BA F5 0A 50 4F 70 00 /@.&?A.$N...POp.
00002CD0 30 2D FF E4 B0 AF 00 1E 6C 08 32 2F 00 1C 3B 41 0-......l.2/..;A
00002CE0 FF E4 70 00 30 2D FF E4 2F 2C 22 B0 2F 00 4E BA ..p.0-../,"./.N.
00002CF0 F4 E4 20 04 D0 AC 3D 30 72 20 D0 81 E8 88 72 00 .. ...=0r ....r.
00002D00 32 00 2E AC 22 B0 2F 01 4E BA F4 CA 4F EF 00 0C 2..."./.N...O...
00002D10 20 05 72 40 D2 81 D0 81 3B 40 FF E2 4A AC 22 E8  .r@....;@..J.".
00002D20 67 08 06 6D 02 00 FF E2 60 0E 70 01 B0 AC 22 DC g..m....`.p...".
00002D30 66 06 06 6D 01 80 FF E2 70 00 30 2D FF E2 2F 2C f..m....p.0-../,
00002D40 22 B0 2F 00 4E BA F4 8E 48 78 00 01 48 78 00 04 "./.N...Hx..Hx..
00002D50 2F 2C 22 B0 4E BA 59 C6 20 2C 22 BC E8 88 72 00 /,".N.Y. ,"...r.
00002D60 32 00 2E AC 22 B0 2F 01 4E BA F4 6A 4F EF 00 18 2..."./.N..jO...
00002D70 20 2C 22 B8 90 84 90 AF 00 22 67 04 70 01 60 02  ,"......"g.p.`.
00002D80 70 00 4C DF 08 FC 4E 5D 4E 75 BF EC 20 1C 65 00 p.L...N]Nu.. .e.
00002D90 49 5C 48 E7 00 30 26 6F 00 0C 24 6F 00 10 B7 CA I\H..0&o..$o....
00002DA0 63 04 70 01 60 0A B7 CA 64 04 70 FF 60 02 70 00 c.p.`...d.p.`.p.
00002DB0 4C DF 0C 00 4E 75 BF EC 20 1C 65 00 49 30 48 E7 L...Nu.. .e.I0H.
00002DC0 21 00 0C AC 00 00 FE FE 22 B8 63 06 70 02 60 00 !.......".c.p.`.
00002DD0 00 B8 48 78 00 03 48 6C 07 9A 48 6C 38 A0 4E BA ..Hx..Hl..Hl8.N.
00002DE0 4C A8 4F EF 00 0C 4A 80 67 20 48 78 00 03 48 6C L.O...J.g Hx..Hl
00002DF0 07 9E 48 6C 38 A0 4E BA 4C 6C 48 6C 38 AD 48 6C ..Hl8.N.LlHl8.Hl
00002E00 38 A0 4E BA EB C4 4F EF 00 14 70 00 10 2C 38 AE 8.N...O...p..,8.
00002E10 72 00 12 2C 38 AD 74 00 34 01 E1 82 D4 80 2F 2C r..,8.t.4...../,
00002E20 22 B0 2F 02 48 6C 38 AF 4E BA F4 34 4F EF 00 0C "./.Hl8.N..4O...
00002E30 70 00 10 2C 38 AE 72 00 12 2C 38 AD 74 00 34 01 p..,8.r..,8.t.4.
00002E40 E1 82 D4 80 20 2C 22 B8 D4 80 2E 02 72 40 DE 81 .... ,".....r@..
00002E50 4A AC 22 E8 67 08 06 87 00 00 02 00 60 0E 72 01 J.".g.......`.r.
00002E60 B2 AC 22 DC 66 06 06 87 00 00 01 80 0C 87 00 00 ..".f...........
00002E70 FE FE 63 04 70 02 60 10 20 2C 22 B8 2F 00 2F 00 ..c.p.`. ,"././.
00002E80 61 00 09 9A 50 4F 70 00 4C DF 00 84 4E 75 4E 55 a...POp.L...NuNU
00002E90 FF F4 BF EC 20 1C 65 00 48 54 48 E7 21 00 70 00 .... .e.HTH.!.p.
00002EA0 10 2C 38 AE 72 00 12 2C 38 AD 74 00 34 01 E1 82 .,8.r..,8.t.4...
00002EB0 D4 80 2E 02 20 2C 22 B8 B0 87 62 06 70 02 60 00 .... ,"...b.p.`.
00002EC0 01 B0 2F 2C 22 B0 2F 07 48 6C 38 AF 4E BA F3 90 ../,"./.Hl8.N...
00002ED0 48 78 00 01 48 78 FF EE 2F 2C 22 B0 4E BA 58 3E Hx..Hx../,".N.X>
00002EE0 2E AC 22 AC 4E BA F1 3E 4F EF 00 18 0C 40 60 1A ..".N..>O....@`.
00002EF0 66 6C 48 78 00 01 48 78 00 02 2F 2C 22 AC 4E BA flHx..Hx../,".N.
00002F00 58 1C 48 78 00 10 2F 2C 22 B0 2F 2C 22 AC 4E BA X.Hx../,"./,".N.
00002F10 ED 82 42 97 48 78 00 1A 2F 2C 22 AC 4E BA 57 FE ..B.Hx../,".N.W.
00002F20 2E AC 22 AC 4E BA F0 36 72 00 32 00 2E AC 22 B0 ..".N..6r.2...".
00002F30 2F 01 4E BA F1 D8 70 E4 D0 AC 22 B8 2E 80 2F 00 /.N...p...".../.
00002F40 61 00 08 DA 4F EF 00 28 20 2C 22 B8 22 00 92 AC a...O..( ,"."...
00002F50 22 BC D2 AC 3D 30 2E 01 74 0E 9E 82 60 3E 2F 2C "...=0..t...`>/,
00002F60 22 B0 2F 2C 22 B8 4E BA F2 2E 48 78 00 01 48 78 "./,".N...Hx..Hx
00002F70 00 0E 2F 2C 22 B0 4E BA 57 A4 20 2C 22 B8 2E 80 ../,".N.W. ,"...
00002F80 2F 00 61 00 08 98 4F EF 00 18 20 2C 22 B8 90 AC /.a...O... ,"...
00002F90 22 BC D0 AC 3D 30 2E 00 72 0E DE 81 2F 2C 22 B0 "...=0..r.../,".
00002FA0 4E BA 57 02 58 4F 08 00 00 00 67 10 2F 2C 22 B0 N.W.XO....g./,".
00002FB0 48 78 00 90 4E BA F1 94 50 4F 52 87 2F 2C 22 B0 Hx..N...POR./,".
00002FC0 42 A7 4E BA F1 D2 2E AC 22 B0 4E BA 56 D8 72 20 B.N.....".N.V.r 
00002FD0 90 81 29 40 22 BC 42 97 48 78 00 02 2F 2C 22 B0 ..)@".B.Hx../,".
00002FE0 4E BA 57 3A 2E AC 22 B0 4E BA F0 8A 9E 80 2E AC N.W:..".N.......
00002FF0 22 B0 2F 2C 22 BC 4E BA F1 9E 70 00 2E 80 2F 00 "./,".N...p.../.
00003000 2F 2C 22 AC 4E BA 57 16 2E AC 22 AC 4E BA EF 4E /,".N.W...".N..N
00003010 4F EF 00 1C 0C 40 60 1A 66 3C 2F 2C 22 AC 4E BA O....@`.f</,".N.
00003020 EF 70 2E AC 22 AC 2F 40 00 14 4E BA EF 64 22 2F .p.."./@..N..d"/
00003030 00 14 D2 80 2E AC 22 AC 2F 41 00 14 4E BA EF 52 ......"./A..N..R
00003040 58 4F 22 2F 00 10 D2 80 20 2C 22 BC D0 87 B2 80 XO"/.... ,".....
00003050 63 04 92 80 DE 81 42 A7 48 78 00 0A 2F 2C 22 B0 c.....B.Hx../,".
00003060 4E BA 56 BA 2E AC 22 B0 2F 07 4E BA F1 2A 70 00 N.V..."./.N..*p.
00003070 4C ED 00 84 FF EC 4E 5D 4E 75 4E 55 FF F4 BF EC L.....N]NuNU....
00003080 20 1C 65 00 46 68 48 E7 27 00 70 00 10 2C 38 AE  .e.FhH.'.p..,8.
00003090 72 00 12 2C 38 AD 74 00 34 01 E1 82 D4 80 20 2C r..,8.t.4..... ,
000030A0 22 B8 B0 82 62 06 70 02 60 00 02 BE 2F 2C 22 AC "...b.p.`.../,".
000030B0 4E BA EE DE 58 4F 0C 80 00 00 03 F3 67 06 70 02 N...XO......g.p.
000030C0 60 00 02 A6 2F 2C 22 B0 48 78 03 F3 4E BA F0 C8 `.../,".Hx..N...
000030D0 50 4F 2F 2C 22 AC 4E BA EE B8 2E 00 2C 07 2E AC PO/,".N.....,...
000030E0 22 B0 2F 07 4E BA F0 B0 50 4F 60 14 2F 2C 22 AC "./.N...PO`./,".
000030F0 4E BA EE 9E 2E AC 22 B0 2F 00 4E BA F0 9A 50 4F N....."./.N...PO
00003100 20 07 53 87 4A 80 66 E4 4A 86 66 C6 2F 2C 22 AC  .S.J.f.J.f./,".
00003110 4E BA EE 7E 52 80 2E AC 22 B0 2F 00 4E BA F0 78 N..~R..."./.N..x
00003120 2E AC 22 AC 4E BA EE 6A 2E AC 22 AC 2F 40 00 18 ..".N..j.."./@..
00003130 4E BA EE 5E 22 2F 00 18 24 00 94 81 2E AC 22 B0 N..^"/..$.....".
00003140 2F 01 2F 40 00 20 2F 42 00 24 4E BA F0 4A 20 2F /./@. /B.$N..J /
00003150 00 20 52 80 2E AC 22 B0 2F 00 4E BA F0 3A 70 00 . R..."./.N..:p.
00003160 10 2C 38 AE 72 00 12 2C 38 AD 74 00 34 01 E1 82 .,8.r..,8.t.4...
00003170 D4 80 70 0C 94 80 E4 82 2E AC 22 B0 2F 02 4E BA ..p......."./.N.
00003180 F0 16 2E AC 22 B0 4E BA 55 1C 4F EF 00 14 29 40 ....".N.U.O...)@
00003190 3D 2C 20 2F 00 18 2E 00 52 87 60 16 2F 2C 22 AC =, /....R.`./,".
000031A0 4E BA ED EE 2E AC 22 B0 2F 00 4E BA EF EA 50 4F N....."./.N...PO
000031B0 53 87 4A 87 66 E6 70 00 10 2C 38 AE 72 00 12 2C S.J.f.p..,8.r..,
000031C0 38 AD 74 00 34 01 E1 82 D4 80 2F 2C 22 B0 2F 02 8.t.4...../,"./.
000031D0 48 6C 38 AF 4E BA F0 88 4F EF 00 0C 7A 00 60 00 Hl8.N...O...z.`.
000031E0 01 74 2F 2C 22 AC 4E BA ED A8 58 4F 2E 00 20 07 .t/,".N...XO.. .
000031F0 02 80 3F FF FF FF 04 80 00 00 03 E9 6D 00 01 52 ..?.........m..R
00003200 0C 80 00 00 00 0D 6C 00 01 48 D0 40 30 3B 00 06 ......l..H.@0;..
00003210 4E FB 00 04 00 18 00 3E 00 64 01 3A 01 3A 01 3A N......>.d.:.:.:
00003220 01 3A 00 A6 00 F0 01 2A 01 3A 01 3A 01 26 70 00 .:.....*.:.:.&p.
00003230 30 05 48 6C 07 A2 2F 00 61 00 01 36 2E 87 61 00 0.Hl../.a..6..a.
00003240 01 8A 70 00 30 05 2E 80 61 00 01 54 50 4F 52 45 ..p.0...a..TPORE
00003250 60 00 01 02 70 00 30 05 48 6C 07 A8 2F 00 61 00 `...p.0.Hl../.a.
00003260 01 10 2E 87 61 00 01 64 70 00 30 05 2E 80 61 00 ....a..dp.0...a.
00003270 01 2E 50 4F 52 45 60 00 00 DC 70 00 30 05 48 6C ..PORE`...p.0.Hl
00003280 07 AE 2F 00 61 00 00 EA 2E AC 22 B0 48 78 03 EB ../.a.....".Hx..
00003290 4E BA EF 04 2E AC 22 AC 4E BA EC F6 2E AC 22 B0 N.....".N.....".
000032A0 2F 00 4E BA EE F2 58 AC 3D 2C 70 00 30 05 2E 80 /.N...X.=,p.0...
000032B0 61 00 00 EC 4F EF 00 10 60 00 00 9A 70 00 30 05 a...O...`...p.0.
000032C0 48 6C 07 B4 2F 00 61 00 00 A8 50 4F 2F 2C 22 AC Hl../.a...PO/,".
000032D0 4E BA EC BE 58 4F E5 80 2E 00 4A 87 67 16 20 07 N...XO....J.g. .
000032E0 58 80 48 78 00 01 2F 00 2F 2C 22 AC 4E BA 54 2E X.Hx.././,".N.T.
000032F0 4F EF 00 0C 4A 87 66 D4 70 00 30 05 2F 00 61 00 O...J.f.p.0./.a.
00003300 00 9E 58 4F 60 4E 70 00 30 05 48 6C 07 BA 2F 00 ..XO`Np.0.Hl../.
00003310 61 00 00 5E 2E AC 22 AC 4E BA EC 76 E5 80 48 78 a..^..".N..v..Hx
00003320 00 01 2F 00 2F 2C 22 AC 4E BA 53 F2 70 00 30 05 .././,".N.S.p.0.
00003330 2E 80 61 00 00 6A 4F EF 00 14 60 18 70 07 60 28 ..a..jO...`.p.`(
00003340 2F 2C 22 B0 48 78 03 F2 4E BA EE 4C 50 4F 60 04 /,".Hx..N..LPO`.
00003350 70 08 60 14 2F 2C 22 AC 4E BA 53 4A 58 4F B0 AC p.`./,".N.SJXO..
00003360 22 B8 65 00 FE 7E 70 00 4C DF 00 E4 4E 5D 4E 75 ".e..~p.L...N]Nu
00003370 BF EC 20 1C 65 00 43 76 48 E7 01 10 3E 2F 00 0E .. .e.CvH...>/..
00003380 26 6F 00 10 70 00 30 07 2F 0B 2F 00 48 6C 07 C0 &o..p.0././.Hl..
00003390 4E BA 4D 98 4F EF 00 0C 4C DF 08 80 4E 75 BF EC N.M.O...L...Nu..
000033A0 20 1C 65 00 43 48 2F 07 3E 2F 00 0A 48 6C 07 CE  .e.CH/.>/..Hl..
000033B0 4E BA 2D 5E 58 4F 70 09 BE 40 63 0A 48 6C 07 DE N.-^XOp..@c.Hl..
000033C0 4E BA 2D 4E 58 4F 2E 1F 4E 75 4E 55 FF EC BF EC N.-NXO..NuNU....
000033D0 20 1C 65 00 43 18 48 E7 0F 00 2E 2D 00 08 2F 2C  .e.C.H....-../,
000033E0 22 AC 4E BA EB AC E5 80 28 00 2E AC 22 AC 4E BA ".N.....(...".N.
000033F0 52 B4 2A 00 48 78 00 01 2F 04 2F 2C 22 AC 4E BA R.*.Hx.././,".N.
00003400 53 1C 2E AC 22 AC 4E BA EC 6C 4F EF 00 10 0C 80 S...".N..lO.....
00003410 00 00 03 EC 66 40 48 78 00 01 48 78 00 04 2F 2C ....f@Hx..Hx../,
00003420 22 AC 4E BA 52 F8 4F EF 00 0C 2F 2C 22 AC 4E BA ".N.R.O.../,".N.
00003430 EB 60 58 4F E5 80 2C 00 4A 86 67 16 20 06 58 80 .`XO..,.J.g. .X.
00003440 48 78 00 01 2F 00 2F 2C 22 AC 4E BA 52 D0 4F EF Hx.././,".N.R.O.
00003450 00 0C 4A 86 66 D4 2F 2C 22 AC 4E BA 52 48 90 85 ..J.f./,".N.RH..
00003460 2C 00 29 46 22 BC 2E AC 22 B0 2F 07 4E BA ED 28 ,.)F"..."./.N..(
00003470 2E AC 22 B0 4E BA 52 2E 50 4F 2E 00 70 12 BC 80 ..".N.R.PO..p...
00003480 63 66 42 A7 2F 2C 3D 2C 2F 2C 22 B0 4E BA 52 8E cfB./,=,/,".N.R.
00003490 2E AC 22 B0 4E BA EA FA 42 97 2F 07 2F 2C 22 B0 ..".N...B././,".
000034A0 2B 40 FF F0 4E BA 52 76 2E AC 22 B0 42 A7 4E BA +@..N.Rv..".B.N.
000034B0 EC E6 2E AC 22 B0 2F 04 4E BA EC DC 2E AC 22 B0 ...."./.N.....".
000034C0 2F 2D FF F0 4E BA EC D0 42 97 2F 05 2F 2C 22 AC /-..N...B././,".
000034D0 4E BA 52 4A 20 06 51 80 2E 80 2F 06 61 00 03 3E N.RJ .Q.../.a..>
000034E0 4F EF 00 2C 50 AC 22 BC 20 2C 22 BC B0 86 65 00 O..,P.". ,"...e.
000034F0 00 B4 42 A7 2F 07 2F 2C 22 B0 4E BA 52 20 20 05 ..B././,".N.R  .
00003500 59 80 42 97 2F 00 2F 2C 22 AC 4E BA 52 10 20 04 Y.B././,".N.R. .
00003510 58 80 2E 80 2F 2C 22 B0 2F 2C 22 AC 4E BA E7 74 X.../,"./,".N..t
00003520 2E AC 22 AC 4E BA EB 4E 4F EF 00 1C 0C 80 00 00 ..".N..NO.......
00003530 03 EC 66 68 48 78 00 01 48 78 00 04 2F 2C 22 AC ..fhHx..Hx../,".
00003540 4E BA 51 DA 2E AC 22 B0 48 78 03 EC 4E BA EC 48 N.Q...".Hx..N..H
00003550 4F EF 00 10 2F 2C 22 AC 4E BA EA 36 2C 00 2E AC O.../,".N..6,...
00003560 22 B0 2F 06 4E BA EC 30 50 4F 4A 86 67 2A 2F 2C "./.N..0POJ.g*/,
00003570 22 AC 4E BA EA 1C 52 80 2E AC 22 B0 2F 00 4E BA ".N...R..."./.N.
00003580 EC 16 20 06 E5 80 2E 80 2F 2C 22 B0 2F 2C 22 AC .. ...../,"./,".
00003590 4E BA E7 00 4F EF 00 10 4A 86 66 B8 58 AC 3D 2C N...O...J.f.X.=,
000035A0 60 00 00 BC DC AC 3D 30 70 03 C0 AC 22 BC 67 02 `.....=0p...".g.
000035B0 54 86 20 06 E4 8E 72 03 C0 81 67 06 2A 06 52 85 T. ...r...g.*.R.
000035C0 60 14 2A 06 60 10 2F 2C 22 B0 42 A7 4E BA EB 7C `.*.`./,".B.N..|
000035D0 50 4F 52 AC 22 BC 70 03 C0 AC 22 BC 66 E8 2F 2C POR.".p...".f./,
000035E0 22 B0 4E BA 50 C0 28 00 20 2C 22 BC 44 80 59 80 ".N.P.(. ,".D.Y.
000035F0 48 78 00 01 2F 00 2F 2C 22 B0 4E BA 51 20 20 2C Hx.././,".N.Q  ,
00003600 22 BC E4 88 2E AC 22 B0 2F 00 4E BA EB 8A 4F EF "....."./.N...O.
00003610 00 14 2C 2D FF F0 2E 06 02 87 3F FF FF FF BE 85 ..,-......?.....
00003620 63 02 2A 07 20 06 02 80 C0 00 00 00 22 05 82 80 c.*. ......."...
00003630 42 A7 2F 2C 3D 2C 2F 2C 22 B0 2F 41 00 1C 4E BA B./,=,/,"./A..N.
00003640 50 DC 2E AC 22 B0 2F 2F 00 1C 4E BA EB 4A 42 97 P...".//..N..JB.
00003650 2F 04 2F 2C 22 B0 4E BA 50 C4 58 AC 3D 2C 4C ED /./,".N.P.X.=,L.
00003660 00 F0 FF DC 4E 5D 4E 75 BF EC 20 1C 65 00 40 7E ....N]Nu.. .e.@~
00003670 2F 02 70 00 10 2C 38 AE 72 00 12 2C 38 AD 74 00 /.p..,8.r..,8.t.
00003680 34 01 E1 82 D4 80 2F 2C 22 B0 2F 02 48 6C 38 AF 4...../,"./.Hl8.
00003690 4E BA EB CC 48 78 00 01 48 78 FF FC 2F 2C 22 B0 N...Hx..Hx../,".
000036A0 4E BA 50 7A 2E AC 22 B0 2F 2C 22 D0 4E BA EA E8 N.Pz.."./,".N...
000036B0 20 2C 22 B8 2E 80 2F 00 61 00 01 62 4F EF 00 20  ,".../.a..bO.. 
000036C0 70 00 24 1F 4E 75 4E 55 FF FC BF EC 20 1C 65 00 p.$.NuNU.... .e.
000036D0 40 1C 2F 07 60 00 01 2C 2F 2C 22 AC 4E BA E8 F0 @./.`..,/,".N...
000036E0 58 4F 4A 80 67 0E 53 80 67 26 55 80 67 00 00 CE XOJ.g.S.g&U.g...
000036F0 60 00 00 F0 2F 2C 22 AC 4E BA E8 1E 72 00 12 00 `.../,".N...r...
00003700 2E AC 22 B0 2F 01 4E BA EA 42 50 4F 60 00 00 F4 .."./.N..BPO`...
00003710 2F 2C 22 AC 4E BA E8 7A 2E AC 22 B0 2F 00 4E BA /,".N..z.."./.N.
00003720 EA 76 2E AC 22 AC 4E BA E9 A6 72 00 32 00 2E AC .v..".N...r.2...
00003730 22 B0 42 A7 2F 41 00 10 4E BA E9 D2 20 2F 00 10 ".B./A..N... /..
00003740 2E 80 2F 00 61 00 00 D6 20 2C 22 BC 54 80 44 80 ../.a... ,".T.D.
00003750 48 78 00 01 2F 00 2F 2C 22 B0 4E BA 4F C0 20 2C Hx.././,".N.O. ,
00003760 22 BC 72 00 32 00 2E AC 22 B0 2F 01 4E BA EA 66 ".r.2..."./.N..f
00003770 4F EF 00 20 20 2F 00 04 22 2C 22 BC B2 80 66 28 O..  /..","...f(
00003780 22 00 44 81 48 78 00 01 2F 01 2F 2C 22 AC 4E BA ".D.Hx.././,".N.
00003790 4F 8C 2E AF 00 10 2F 2C 22 B0 2F 2C 22 AC 4E BA O...../,"./,".N.
000037A0 E4 F2 4F EF 00 14 60 5A 48 78 00 01 2F 01 2F 2C ..O...`ZHx.././,
000037B0 22 B0 4E BA 4F 68 4F EF 00 0C 60 46 7E 00 60 1A ".N.OhO...`F~.`.
000037C0 2F 2C 22 AC 4E BA E7 52 72 00 12 00 2E AC 22 B0 /,".N..Rr.....".
000037D0 2F 01 4E BA E9 76 50 4F 52 47 70 03 BE 40 66 E0 /.N..vPORGp..@f.
000037E0 60 20 2F 2C 22 AC 4E BA 4E BC 22 2C 22 B8 92 80 ` /,".N.N.","...
000037F0 2E 81 2F 2C 22 B0 2F 2C 22 AC 4E BA E4 96 4F EF ../,"./,".N...O.
00003800 00 0C 2F 2C 22 AC 4E BA 4E 9C 58 4F B0 AC 22 B8 ../,".N.N.XO..".
00003810 65 00 FE C6 70 00 2E 1F 4E 5D 4E 75 4E 55 FF F8 e...p...N]NuNU..
00003820 BF EC 20 1C 65 00 3E C6 48 E7 27 00 2E 2F 00 20 .. .e.>.H.'../. 
00003830 2C 2F 00 24 29 47 45 3A 29 47 22 BC 29 47 45 42 ,/.$)GE:)G".)GEB
00003840 20 06 72 12 90 81 29 40 3D 34 BE 81 63 00 02 8C  .r...)@=4..c...
00003850 70 00 39 40 26 F0 39 40 26 F2 39 40 45 5E 72 00 p.9@&.9@&.9@E^r.
00003860 29 41 45 52 29 41 22 BC 29 41 22 C0 29 41 45 3E )AER)A".)A".)AE>
00003870 29 41 45 46 39 40 26 F8 39 40 26 F6 39 40 3D 38 )AEF9@&.9@&.9@=8
00003880 29 41 3D 30 29 41 22 CC 2F 3C 00 00 FF FF 4E BA )A=0)A"./<....N.
00003890 E6 08 29 40 3C 00 2E BC 00 01 00 00 4E BA E5 FA ..)@<.......N...
000038A0 29 40 27 02 2E BC 00 01 00 00 4E BA E5 EC 29 40 )@'.......N...)@
000038B0 27 06 2E BC 00 01 00 00 4E BA E5 DE 29 40 26 FA '.......N...)@&.
000038C0 2E BC 00 01 00 00 4E BA E5 D0 29 40 26 FE 4E BA ......N...)@&.N.
000038D0 24 90 2E AC 22 AC 4E BA 4D CC 29 40 45 56 2E AC $...".N.M.)@EV..
000038E0 22 B0 4E BA 4D C0 29 40 45 5A 20 3C 52 4E 43 00 ".N.M.)@EZ <RNC.
000038F0 D0 AC 22 DC 2E AC 22 B0 2F 00 4E BA E8 9A 2E AC .."..."./.N.....
00003900 22 B0 2F 2C 45 3A 4E BA E8 8E 2E AC 22 B0 42 A7 "./,E:N.....".B.
00003910 4E BA E8 84 2E AC 22 B0 42 A7 4E BA E7 F0 2E AC N.....".B.N.....
00003920 22 B0 42 A7 4E BA E7 E6 2E AC 22 B0 42 A7 4E BA ".B.N.....".B.N.
00003930 E7 DC 48 6C 07 E0 4E BA 27 D8 30 2C 26 EC 3F 40 ..Hl..N.'.0,&.?@
00003940 00 34 4A AC 22 E4 56 C1 44 01 48 81 48 C1 74 00 .4J.".V.D.H.H.t.
00003950 34 01 48 78 00 01 2F 02 61 00 0C 16 30 2C 26 EC 4.Hx../.a...0,&.
00003960 56 C1 44 01 48 81 48 C1 70 00 30 01 48 78 00 01 V.D.H.H.p.0.Hx..
00003970 2F 00 61 00 0B FC 4F EF 00 30 4A AC 22 D8 67 18 /.a...O..0J.".g.
00003980 4A 6C 26 EC 67 12 70 00 30 2C 26 EC 48 78 00 10 Jl&.g.p.0,&.Hx..
00003990 2F 00 61 00 0B DC 50 4F 20 2C 22 DC 53 80 67 06 /.a...PO ,".S.g.
000039A0 53 80 67 08 60 0A 61 00 01 3C 60 04 61 00 03 C8 S.g.`.a..<`.a...
000039B0 7A 00 60 26 20 05 2A 00 52 45 72 00 32 00 70 00 z.`& .*.REr.2.p.
000039C0 41 EC 3D 3A 10 30 18 00 2F 00 61 00 0D BC 58 4F A.=:.0../.a...XO
000039D0 30 2C 3D 38 53 40 39 40 3D 38 4A 6C 3D 38 66 D4 0,=8S@9@=8Jl=8f.
000039E0 39 6F 00 14 26 EC 20 2C 45 3A 90 AC 22 BC 22 2C 9o..&. ,E:..".",
000039F0 3D 30 B2 80 63 06 91 AC 3D 30 60 06 70 00 29 40 =0..c...=0`.p.)@
00003A00 3D 30 70 02 B0 AC 22 DC 66 04 54 AC 3D 30 2F 2C =0p...".f.T.=0/,
00003A10 22 B0 4E BA 4C 90 22 2C 45 5A 90 81 29 40 22 BC ".N.L.",EZ..)@".
00003A20 50 81 42 97 2F 01 2F 2C 22 B0 4E BA 4C F0 70 EE P.B././,".N.L.p.
00003A30 D0 AC 22 BC 2E AC 22 B0 2F 00 4E BA E7 5A 70 00 .."..."./.N..Zp.
00003A40 30 2C 26 F0 2E AC 22 B0 2F 00 4E BA E6 C0 70 00 0,&..."./.N...p.
00003A50 30 2C 26 F2 2E AC 22 B0 2F 00 4E BA E6 B0 20 2C 0,&..."./.N... ,
00003A60 3D 30 72 00 12 00 2E AC 22 B0 2F 01 4E BA E6 DC =0r....."./.N...
00003A70 20 2C 22 CC 72 00 12 00 2E AC 22 B0 2F 01 4E BA  ,".r....."./.N.
00003A80 E6 CA 20 2C 45 5A D0 AC 22 BC 42 97 2F 00 2F 2C .. ,EZ..".B././,
00003A90 22 B0 4E BA 4C 88 20 2C 45 56 D0 AC 45 3A 42 97 ".N.L. ,EV..E:B.
00003AA0 2F 00 2F 2C 22 AC 4E BA 4C 74 2E AC 3C 00 4E BA /./,".N.Lt..<.N.
00003AB0 E4 18 2E AC 27 02 4E BA E4 10 2E AC 27 06 4E BA ....'.N.....'.N.
00003AC0 E4 08 2E AC 26 FA 4E BA E4 00 2E AC 26 FE 4E BA ....&.N.....&.N.
00003AD0 E3 F8 48 6C 07 E4 4E BA 26 38 4C ED 00 E4 FF E8 ..Hl..N.&8L.....
00003AE0 4E 5D 4E 75 4E 55 FF FC BF EC 20 1C 65 00 3B FE N]NuNU.... .e.;.
00003AF0 48 E7 23 00 2E 2C 45 56 60 00 02 16 48 78 00 10 H.#..,EV`...Hx..
00003B00 48 6C 21 2C 4E BA E8 B6 48 78 00 10 48 6C 21 EC Hl!,N...Hx..Hl!.
00003B10 4E BA E8 AA 48 78 00 10 48 6C 20 6C 4E BA E8 9E N...Hx..Hl lN...
00003B20 61 00 06 4A 42 97 2F 07 2F 2C 22 AC 4E BA 4B EE a..JB././,".N.K.
00003B30 48 78 00 10 48 6C 20 6C 4E BA E8 C6 48 78 00 10 Hx..Hl lN...Hx..
00003B40 48 6C 21 2C 4E BA E8 BA 48 78 00 10 48 6C 21 EC Hl!,N...Hx..Hl!.
00003B50 4E BA E8 AE 48 78 00 10 48 6C 20 6C 61 00 09 28 N...Hx..Hl la..(
00003B60 48 78 00 10 48 6C 21 2C 61 00 09 1C 4F EF 00 48 Hx..Hl!,a...O..H
00003B70 48 78 00 10 48 6C 21 EC 61 00 09 0C 20 2C 45 4E Hx..Hl!.a... ,EN
00003B80 72 00 32 00 48 78 00 10 2F 01 61 00 0A 2C 4F EF r.2.Hx../.a..,O.
00003B90 00 10 60 00 01 28 2F 2C 22 B4 4E BA E3 C0 39 40 ..`..(/,".N...9@
00003BA0 26 F4 72 00 32 00 D3 AC 45 3E 72 00 32 00 48 6C &.r.2...E>r.2.Hl
00003BB0 20 6C 2F 01 61 00 09 38 4F EF 00 0C 4A 6C 26 F4  l/.a..8O...Jl&.
00003BC0 67 00 00 90 4A 6C 26 F6 67 54 60 2A 30 2C 3D 38 g...Jl&.gT`*0,=8
00003BD0 22 00 52 41 39 41 3D 38 3F 40 00 0C 61 00 08 62 ".RA9A=8?@..a..b
00003BE0 32 2F 00 0C 74 00 34 01 32 2C 26 EC B3 00 41 EC 2/..t.4.2,&...A.
00003BF0 3D 3A 11 80 28 00 30 2C 26 F4 22 00 53 41 39 41 =:..(.0,&.".SA9A
00003C00 26 F4 4A 40 66 C6 60 26 61 00 08 36 32 2C 26 EC &.J@f.`&a..62,&.
00003C10 B3 00 72 00 12 00 2F 01 61 00 0B 6E 58 4F 30 2C ..r.../.a..nXO0,
00003C20 26 F4 22 00 53 41 39 41 26 F4 4A 40 66 DA 70 00 &.".SA9A&.J@f.p.
00003C30 30 2C 26 EC 22 00 E2 81 2E 01 08 00 00 00 67 0C 0,&.".........g.
00003C40 20 07 00 40 80 00 39 40 26 EC 60 06 20 07 39 40  ..@..9@&.`. .9@
00003C50 26 EC 4A AC 45 4E 67 64 2F 2C 22 B4 4E BA E2 FE &.J.ENgd/,".N...
00003C60 39 40 27 0E 2E AC 22 B4 4E BA E2 F2 39 40 27 0C 9@'...".N...9@'.
00003C70 72 00 32 00 48 6C 21 2C 2F 01 61 00 08 72 70 00 r.2.Hl!,/.a..rp.
00003C80 30 2C 27 0E 48 6C 21 EC 2F 00 61 00 08 62 4F EF 0,'.Hl!./.a..bO.
00003C90 00 14 30 2C 27 0E 22 00 54 41 39 41 27 0E 70 00 ..0,'.".TA9A'.p.
00003CA0 30 01 D1 AC 45 3E 60 04 61 00 07 96 30 2C 27 0E 0...E>`.a...0,'.
00003CB0 22 00 53 41 39 41 27 0E 4A 40 66 EC 20 2C 45 4E ".SA9A'.J@f. ,EN
00003CC0 53 AC 45 4E 4A 80 66 00 FE CE 30 2C 26 F6 66 30 S.ENJ.f...0,&.f0
00003CD0 7C 00 60 26 20 06 2C 00 52 46 72 00 32 00 70 00 |.`& .,.RFr.2.p.
00003CE0 41 EC 3D 3A 10 30 18 00 2F 00 61 00 0A 9C 58 4F A.=:.0../.a...XO
00003CF0 30 2C 3D 38 53 40 39 40 3D 38 4A 6C 3D 38 66 D4 0,=8S@9@=8Jl=8f.
00003D00 52 AC 22 CC 2F 2C 22 AC 4E BA 49 9A 58 4F 2E 00 R."./,".N.I.XO..
00003D10 20 2C 45 3E B0 AC 45 3A 65 00 FD E2 70 00 30 2C  ,E>..E:e...p.0,
00003D20 26 F6 72 10 92 80 74 00 34 2C 26 F8 E2 A2 39 42 &.r...t.4,&...9B
00003D30 26 F8 4A 6C 26 F6 66 06 4A 6C 3D 38 67 0C 70 00 &.Jl&.f.Jl=8g.p.
00003D40 10 02 2F 00 61 00 0A 42 58 4F 30 2C 26 F6 72 08 ../.a..BXO0,&.r.
00003D50 B0 41 62 06 4A 6C 3D 38 67 14 70 00 30 2C 26 F8 .Ab.Jl=8g.p.0,&.
00003D60 E0 80 72 00 12 00 2F 01 61 00 0A 1E 58 4F 4C DF ..r.../.a...XOL.
00003D70 00 C4 4E 5D 4E 75 BF EC 20 1C 65 00 39 70 48 E7 ..N]Nu.. .e.9pH.
00003D80 33 00 2E 2C 45 56 60 00 02 16 61 00 03 E0 42 A7 3..,EV`...a...B.
00003D90 2F 07 2F 2C 22 AC 4E BA 49 84 4F EF 00 0C 60 00 /./,".N.I.O...`.
00003DA0 01 72 2F 2C 22 B4 4E BA E1 B4 39 40 26 F4 72 00 .r/,".N...9@&.r.
00003DB0 32 00 D3 AC 45 3E 72 00 32 00 2E 81 61 00 02 1E 2...E>r.2...a...
00003DC0 58 4F 4A AC 45 4E 67 00 01 4A 2F 2C 22 B4 4E BA XOJ.ENg..J/,".N.
00003DD0 E1 8C 39 40 27 0E 2E AC 22 B4 4E BA E1 80 58 4F ..9@'...".N...XO
00003DE0 39 40 27 0C 30 2C 27 0E 66 20 48 78 00 03 48 78 9@'.0,'.f Hx..Hx
00003DF0 00 06 61 00 08 94 30 2C 27 0C 72 00 12 00 2E 81 ..a...0,'.r.....
00003E00 61 00 09 48 50 4F 60 00 00 E0 72 07 B0 41 64 74 a..HPO`...r..Adt
00003E10 72 00 32 00 74 00 41 EC 07 64 14 30 18 00 72 00 r.2.t.A..d.0..r.
00003E20 32 02 74 00 34 00 70 00 41 EC 07 6B 10 30 28 00 2.t.4.p.A..k.0(.
00003E30 74 00 34 00 2F 02 2F 01 61 00 08 4E 70 00 30 2C t.4././.a..Np.0,
00003E40 27 0C E0 80 72 00 41 EC 07 72 12 30 08 00 74 00 '...r.A..r.0..t.
00003E50 34 01 72 00 41 EC 07 82 12 30 08 00 70 00 30 01 4.r.A....0..p.0.
00003E60 2E 80 2F 02 61 00 08 22 70 00 30 2C 27 0C 72 00 ../.a.."p.0,'.r.
00003E70 46 01 C0 81 72 00 12 00 2E 81 61 00 08 CE 4F EF F...r.....a...O.
00003E80 00 0C 60 64 48 78 00 04 48 78 00 0F 61 00 07 FA ..`dHx..Hx..a...
00003E90 70 00 30 2C 27 0E 5D 80 72 00 12 00 2E 81 61 00 p.0,'.].r.....a.
00003EA0 08 AA 70 00 30 2C 27 0C E0 80 72 00 41 EC 07 72 ..p.0,'...r.A..r
00003EB0 12 30 08 00 74 00 34 01 72 00 41 EC 07 82 12 30 .0..t.4.r.A....0
00003EC0 08 00 70 00 30 01 2E 80 2F 02 61 00 07 BC 70 00 ..p.0.../.a...p.
00003ED0 30 2C 27 0C 72 00 46 01 C0 81 72 00 12 00 2E 81 0,'.r.F...r.....
00003EE0 61 00 08 68 4F EF 00 0C 30 2C 27 0E 22 00 54 41 a..hO...0,'.".TA
00003EF0 39 41 27 0E 70 00 30 01 D1 AC 45 3E 60 04 61 00 9A'.p.0...E>`.a.
00003F00 05 40 30 2C 27 0E 22 00 53 41 39 41 27 0E 4A 40 .@0,'.".SA9A'.J@
00003F10 66 EC 20 2C 45 4E 53 AC 45 4E 4A 80 66 00 FE 84 f. ,ENS.ENJ.f...
00003F20 48 78 00 04 48 78 00 0F 61 00 07 5E 42 97 61 00 Hx..Hx..a..^B.a.
00003F30 08 1A 50 4F 20 2C 45 3E B0 AC 45 3A 64 0E 70 01 ..PO ,E>..E:d.p.
00003F40 2F 00 2F 00 61 00 07 42 50 4F 60 0C 48 78 00 01 /./.a..BPO`.Hx..
00003F50 42 A7 61 00 07 34 50 4F 30 2C 26 F6 66 30 7C 00 B.a..4PO0,&.f0|.
00003F60 60 26 20 06 2C 00 52 46 72 00 32 00 70 00 41 EC `& .,.RFr.2.p.A.
00003F70 3D 3A 10 30 18 00 2F 00 61 00 08 0E 58 4F 30 2C =:.0../.a...XO0,
00003F80 3D 38 53 40 39 40 3D 38 4A 6C 3D 38 66 D4 52 AC =8S@9@=8Jl=8f.R.
00003F90 22 CC 2F 2C 22 AC 4E BA 47 0C 58 4F 2E 00 20 2C "./,".N.G.XO.. ,
00003FA0 45 3E B0 AC 45 3A 65 00 FD E2 30 2C 26 F6 72 08 E>..E:e...0,&.r.
00003FB0 92 40 34 2C 26 F8 26 02 E3 63 39 43 26 F8 4A 6C .@4,&.&..c9C&.Jl
00003FC0 26 F6 66 06 4A 6C 3D 38 67 0C 70 00 10 03 2F 00 &.f.Jl=8g.p.../.
00003FD0 61 00 07 B6 58 4F 4C DF 00 CC 4E 75 BF EC 20 1C a...XOL...Nu.. .
00003FE0 65 00 37 0A 48 E7 07 00 3E 2F 00 12 60 00 01 72 e.7.H...>/..`..r
00003FF0 70 0C BE 40 64 00 00 98 60 46 48 78 00 01 42 A7 p..@d...`FHx..B.
00004000 61 00 06 86 61 00 04 3A 32 2C 26 EC B3 00 72 00 a...a..:2,&...r.
00004010 12 00 2E 81 61 00 07 34 50 4F 70 00 30 2C 26 EC ....a..4POp.0,&.
00004020 22 00 E2 81 2C 01 08 00 00 00 67 0C 20 06 00 40 "...,.....g. ..@
00004030 80 00 39 40 26 EC 60 06 20 06 39 40 26 EC 53 47 ..9@&.`. .9@&.SG
00004040 4A 47 66 B6 60 00 01 1A 48 78 00 01 42 A7 61 00 JGf.`...Hx..B.a.
00004050 06 38 61 00 03 EC 32 2C 26 EC B3 00 72 00 12 00 .8a...2,&...r...
00004060 2E 81 61 00 06 E6 50 4F 70 00 30 2C 26 EC 22 00 ..a...POp.0,&.".
00004070 E2 81 2C 01 08 00 00 00 67 0C 20 06 00 40 80 00 ..,.....g. ..@..
00004080 39 40 26 EC 60 06 20 06 39 40 26 EC 53 47 20 07 9@&.`. .9@&.SG .
00004090 02 40 00 03 66 B2 48 78 00 05 48 78 00 17 61 00 .@..f.Hx..Hx..a.
000040A0 05 E8 50 4F 70 48 BE 40 65 5A 48 78 00 04 48 78 ..POpH.@eZHx..Hx
000040B0 00 0F 61 00 05 D4 50 4F 7A 00 60 18 61 00 03 82 ..a...POz.`.a...
000040C0 32 2C 26 EC B3 00 72 00 12 00 2F 01 61 00 06 7C 2,&...r.../.a..|
000040D0 58 4F 52 45 70 48 BA 40 66 E2 70 00 30 2C 26 EC XOREpH.@f.p.0,&.
000040E0 22 00 E2 81 2C 01 08 00 00 00 67 0C 20 06 00 40 "...,.....g. ..@
000040F0 80 00 39 40 26 EC 60 06 20 06 39 40 26 EC 04 47 ..9@&.`. .9@&..G
00004100 00 48 60 5C 70 00 30 07 72 0C 90 81 E4 80 72 00 .H`\p.0.r.....r.
00004110 32 00 48 78 00 04 2F 01 61 00 05 6E 50 4F 60 18 2.Hx../.a..nPO`.
00004120 61 00 03 1E 32 2C 26 EC B3 00 72 00 12 00 2F 01 a...2,&...r.../.
00004130 61 00 06 18 58 4F 53 47 4A 47 66 E4 70 00 30 2C a...XOSGJGf.p.0,
00004140 26 EC 22 00 E2 81 2C 01 08 00 00 00 67 0C 20 06 &."...,.....g. .
00004150 00 40 80 00 39 40 26 EC 60 06 20 06 39 40 26 EC .@..9@&.`. .9@&.
00004160 4A 47 66 00 FE 8C 4C DF 00 E0 4E 75 BF EC 20 1C JGf...L...Nu.. .
00004170 65 00 35 7A 48 E7 31 20 70 00 29 40 45 4E 42 6C e.5zH.1 p.)@ENBl
00004180 26 F4 29 6C 22 C8 45 4A 22 2C 45 56 D2 AC 45 3E &.)l".EJ",EV..E>
00004190 D2 AC 45 46 2F 00 2F 01 2F 2C 22 AC 4E BA 45 7E ..EF/././,".N.E~
000041A0 70 00 2E 80 2F 00 2F 2C 22 B4 4E BA 45 70 4F EF p..././,".N.EpO.
000041B0 00 14 60 00 01 D6 70 00 30 2C 27 10 22 3C 00 00 ..`...p.0,'."<..
000041C0 FF FF 92 80 24 2C 45 46 92 82 2E 01 22 2C 45 42 ....$,EF....",EB
000041D0 B2 87 64 02 2E 01 20 6C 3C 00 72 00 32 00 D1 C1 ..d... l<.r.2...
000041E0 29 48 3C 04 D1 C2 2F 2C 22 AC 2F 07 2F 08 4E BA )H<.../,"././.N.
000041F0 E0 30 4F EF 00 0C 9F AC 45 42 DF AC 45 46 20 2C .0O.....EB..EF ,
00004200 45 46 20 6C 3C 04 22 48 D3 C0 29 49 3C 0C 29 49 EF l<."H..)I<.)I
00004210 3C 08 22 2C 45 4A B2 80 64 00 00 F6 20 6C 3C 04 <.",EJ..d... l<.
00004220 D1 C1 29 48 3C 0C 60 00 00 E8 4E BA 1B A2 30 2C ..)H<.`...N...0,
00004230 27 0E 72 02 B0 41 65 00 00 88 20 6C 3C 04 72 00 '.r..Ae... l<.r.
00004240 32 00 22 48 D3 C1 24 6C 3C 0C B3 CA 63 12 4A AC 2."H..$l<...c.J.
00004250 45 4E 66 00 00 D6 20 0A 90 AC 3C 04 39 40 27 0E ENf... ...<.9@'.
00004260 70 00 30 2C 26 F4 48 6C 20 6C 2F 00 61 00 01 80 p.0,&.Hl l/.a...
00004270 30 2C 27 0E 55 40 72 00 32 00 48 6C 21 EC 2F 01 0,'.U@r.2.Hl!./.
00004280 61 00 01 6C 30 2C 27 0C 53 40 72 00 32 00 48 6C a..l0,'.S@r.2.Hl
00004290 21 2C 2F 01 61 00 01 58 70 00 30 2C 27 0E 2E 80 !,/.a..Xp.0,'...
000042A0 4E BA 1C D4 4F EF 00 18 52 AC 45 4E 42 6C 26 F4 N...O...R.ENBl&.
000042B0 30 2C 27 0E 32 2C 45 5E D2 40 39 41 45 5E 60 20 0,'.2,E^.@9AE^` 
000042C0 48 78 00 01 4E BA 1C B0 58 4F 30 2C 26 F4 52 40 Hx..N...XO0,&.R@
000042D0 39 40 26 F4 30 2C 45 5E 22 00 52 41 39 41 45 5E 9@&.0,E^".RA9AE^
000042E0 0C 41 04 00 65 2A 70 00 30 01 D1 AC 45 52 20 2C .A..e*p.0...ER ,
000042F0 45 52 72 63 4E BA 4E 5E 22 2C 45 3A 4E BA 4E A8 ERrcN.N^",E:N.N.
00004300 2F 00 48 6C 07 E8 4E BA 3E 22 50 4F 42 6C 45 5E /.Hl..N.>"POBlE^
00004310 20 6C 3C 0C 53 88 22 6C 3C 04 B3 C8 64 0C 0C AC  l<.S."l<...d...
00004320 00 00 FF FE 45 4E 65 00 FF 02 20 2C 3C 08 90 AC ....ENe... ,<...
00004330 3C 04 29 40 45 46 22 2C 3C 04 24 01 94 AC 3C 00 <.)@EF",<.$...<.
00004340 76 00 36 2C 27 10 94 83 2E 02 20 6C 3C 00 D1 C7 v.6,'..... l<...
00004350 24 08 92 82 D2 80 2F 01 2F 2C 3C 00 2F 02 4E BA $....././,<./.N.
00004360 3F 84 4F EF 00 0C 20 6C 3C 08 22 6C 3C 0C B3 C8 ?.O... l<."l<...
00004370 65 28 B3 C8 66 06 4A AC 45 42 67 1E 0C AC 00 00 e(..f.J.EBg.....
00004380 FF FE 45 4E 67 14 9F AC 45 4A 4A AC 45 42 66 00 ..ENg...EJJ.EBf.
00004390 FE 26 4A AC 45 46 66 00 FE 1E 20 6C 3C 0C B1 EC .&J.EFf... l<...
000043A0 3C 08 66 1E 4A AC 45 42 66 18 0C AC 00 00 FF FE <.f.J.EBf.......
000043B0 45 4E 67 0E 70 00 30 2C 26 F4 D0 AC 45 46 39 40 ENg.p.0,&...EF9@
000043C0 26 F4 70 00 30 2C 26 F4 48 6C 20 6C 2F 00 61 00 &.p.0,&.Hl l/.a.
000043D0 00 1E 52 AC 45 4E 70 00 2E 80 2F 00 2F 2C 22 B4 ..R.ENp..././,".
000043E0 4E BA 43 3A 4F EF 00 10 4C DF 04 8C 4E 75 BF EC N.C:O...L...Nu..
000043F0 20 1C 65 00 32 F8 48 E7 01 10 3E 2F 00 0E 26 6F  .e.2.H...>/..&o
00004400 00 10 70 01 BE 40 63 12 70 00 30 07 2F 00 4E BA ..p..@c.p.0./.N.
00004410 D8 60 58 4F 72 00 32 00 60 06 70 00 30 07 22 00 .`XOr.2.`.p.0.".
00004420 70 0C 4E BA 4D 30 52 B3 08 00 70 00 30 07 2F 2C p.N.M0R...p.0./,
00004430 22 B4 2F 00 4E BA DC D6 50 4F 4C DF 08 80 4E 75 "./.N...POL...Nu
00004440 BF EC 20 1C 65 00 32 A6 48 E7 30 00 2F 2C 22 AC .. .e.2.H.0./,".
00004450 4E BA DA C6 58 4F 72 00 12 00 34 2C 26 F0 76 00 N...XOr...4,&.v.
00004460 16 02 B3 83 72 00 46 01 C6 81 D6 83 E0 4A 41 EC ....r.F......JA.
00004470 22 EC 32 30 38 00 B5 41 39 41 26 F0 52 AC 22 C0 ".208..A9A&.R.".
00004480 4C DF 00 0C 4E 75 BF EC 20 1C 65 00 32 60 48 E7 L...Nu.. .e.2`H.
00004490 03 10 26 6F 00 10 3E 2F 00 16 2C 07 60 0E 20 06 ..&o..>/..,.`. .
000044A0 C0 FC 00 0C 4A 73 08 0A 66 0C 53 47 20 06 2C 00 ....Js..f.SG .,.
000044B0 53 46 4A 40 66 E8 70 00 30 07 48 78 00 05 2F 00 SFJ@f.p.0.Hx../.
000044C0 61 00 00 F6 50 4F 7C 00 60 1A 20 06 C0 FC 00 0C a...PO|.`. .....
000044D0 72 00 32 33 08 0A 48 78 00 04 2F 01 61 00 00 DA r.23..Hx../.a...
000044E0 50 4F 52 46 BC 47 65 E2 4C DF 08 C0 4E 75 4E 55 PORF.Ge.L...NuNU
000044F0 FF FC BF EC 20 1C 65 00 31 F4 48 E7 21 10 3E 2F .... .e.1.H.!.>/
00004500 00 1A 26 6F 00 1C 70 01 BE 40 63 12 70 00 30 07 ..&o..p..@c.p.0.
00004510 2F 00 4E BA D7 5C 58 4F 72 00 32 00 60 06 70 00 /.N..\XOr.2.`.p.
00004520 30 07 22 00 3F 41 00 0C C2 FC 00 0C 20 33 18 06 0.".?A...... 3..
00004530 74 00 34 00 70 00 30 33 18 0A 2F 00 2F 02 61 00 t.4.p.03.././.a.
00004540 00 78 50 4F 30 2F 00 0C 72 01 B0 41 63 1A 53 40 .xPO0/..r..Ac.S@
00004550 E1 61 24 07 94 41 72 00 32 02 74 00 34 00 2F 02 .a$..Ar.2.t.4./.
00004560 2F 01 61 00 00 54 50 4F 4C DF 08 84 4E 5D 4E 75 /.a..TPOL...N]Nu
00004570 BF EC 20 1C 65 00 31 76 48 E7 03 00 3E 2F 00 0E .. .e.1vH...>/..
00004580 3C 2F 00 12 70 02 B0 AC 22 DC 66 14 70 00 30 07 </..p...".f.p.0.
00004590 72 00 32 06 2F 01 2F 00 61 00 00 EE 50 4F 60 12 r.2././.a...PO`.
000045A0 70 00 30 07 72 00 32 06 2F 01 2F 00 61 00 00 0A p.0.r.2././.a...
000045B0 50 4F 4C DF 00 C0 4E 75 BF EC 20 1C 65 00 31 2E POL...Nu.. .e.1.
000045C0 48 E7 07 00 3E 2F 00 12 3C 2F 00 16 60 00 00 A8 H...>/..</..`...
000045D0 30 2C 26 F8 22 00 E2 49 39 41 26 F8 08 07 00 00 0,&."..I9A&.....
000045E0 67 0C 70 00 30 01 00 40 80 00 39 40 26 F8 E2 4F g.p.0..@..9@&..O
000045F0 30 2C 26 F6 22 00 52 41 39 41 26 F6 70 10 B2 40 0,&.".RA9A&.p..@
00004600 66 74 30 2C 26 F8 72 00 12 00 2F 01 61 00 01 7A ft0,&.r.../.a..z
00004610 70 00 30 2C 26 F8 E0 80 72 00 12 00 2E 81 61 00 p.0,&...r.....a.
00004620 01 68 58 4F 7A 00 60 26 20 05 2A 00 52 45 72 00 .hXOz.`& .*.REr.
00004630 32 00 70 00 41 EC 3D 3A 10 30 18 00 2F 00 61 00 2.p.A.=:.0../.a.
00004640 01 48 58 4F 30 2C 3D 38 53 40 39 40 3D 38 4A 6C .HXO0,=8S@9@=8Jl
00004650 3D 38 66 D4 20 2C 22 BC 22 2C 22 C0 B2 80 63 0C =8f. ,".","...c.
00004660 92 80 B2 AC 3D 30 63 04 29 41 3D 30 70 00 39 40 ....=0c.)A=0p.9@
00004670 26 F6 39 40 26 F8 20 06 2C 00 53 46 4A 40 66 00 &.9@&. .,.SFJ@f.
00004680 FF 50 4C DF 00 E0 4E 75 BF EC 20 1C 65 00 30 5E .PL...Nu.. .e.0^
00004690 48 E7 0F 00 3E 2F 00 16 3C 2F 00 1A 20 06 53 40 H...>/..</.. .S@
000046A0 72 01 E1 61 2A 01 60 00 00 90 30 2C 26 F8 22 00 r..a*.`...0,&.".
000046B0 D2 41 39 41 26 F8 20 07 C0 45 67 08 20 01 52 40 .A9A&. ..Eg. .R@
000046C0 39 40 26 F8 E2 4D 30 2C 26 F6 22 00 52 41 39 41 9@&..M0,&.".RA9A
000046D0 26 F6 51 41 66 62 30 2C 26 F8 72 00 12 00 2F 01 &.QAfb0,&.r.../.
000046E0 61 00 00 A6 58 4F 78 00 60 26 20 04 28 00 52 44 a...XOx.`& .(.RD
000046F0 72 00 32 00 70 00 41 EC 3D 3A 10 30 18 00 2F 00 r.2.p.A.=:.0../.
00004700 61 00 00 86 58 4F 30 2C 3D 38 53 40 39 40 3D 38 a...XO0,=8S@9@=8
00004710 4A 6C 3D 38 66 D4 20 2C 22 BC 22 2C 22 C0 B2 80 Jl=8f. ,".","...
00004720 63 0C 92 80 B2 AC 3D 30 63 04 29 41 3D 30 70 00 c.....=0c.)A=0p.
00004730 39 40 26 F6 39 40 26 F8 20 06 2C 00 53 46 4A 40 9@&.9@&. .,.SFJ@
00004740 66 00 FF 68 4C DF 00 F0 4E 75 BF EC 20 1C 65 00 f..hL...Nu.. .e.
00004750 2F 9C 2F 07 1E 2F 00 0B 4A 6C 26 F6 67 1A 30 2C /./../..Jl&.g.0,
00004760 3D 38 22 00 52 41 39 41 3D 38 72 00 32 00 41 EC =8".RA9A=8r.2.A.
00004770 3D 3A 11 87 18 00 60 0C 70 00 10 07 2F 00 61 00 =:....`.p.../.a.
00004780 00 08 58 4F 2E 1F 4E 75 BF EC 20 1C 65 00 2F 5E ..XO..Nu.. .e./^
00004790 48 E7 21 00 1E 2F 00 0F 20 2C 22 BC B0 AC 3D 34 H.!../.. ,"...=4
000047A0 64 3A 70 00 10 07 2F 2C 22 B0 2F 00 4E BA D9 9C d:p.../,"./.N...
000047B0 50 4F 70 00 10 07 32 2C 26 F2 74 00 14 01 B1 82 POp...2,&.t.....
000047C0 70 00 46 00 C4 80 D4 82 E0 49 41 EC 22 EC 30 30 p.F......IA.".00
000047D0 28 00 B3 40 39 40 26 F2 52 AC 22 BC 4C DF 00 84 (..@9@&.R.".L...
000047E0 4E 75 00 00                                     Nu..           

fn000047E4()
	cmpa.l	$201C(a4),a7
	bcs	$000076EC

l000047EC:
	move.l	d7,-(a7)
	move.l	$22AC(a4),-(a7)
	jsr.l	$-2556(pc)
	addq.w	#$04,a7
	move.l	d0,$22B8(a4)
	move.l	d0,$22BC(a4)
	moveq	#$+12,d1
	cmp.l	d1,d0
	bcc	$0000480C

l00004806:
	moveq	#$+06,d0
	bra	$00004898

l0000480C:
	move.l	$22D8(a4),d0
	cmpi.l	#$00000006,d0
	bcc	$00004874

l00004818:
	add.w	d0,d0
	move.w	(06,pc,d0),d0
	jmp.l	(04,pc,d0)
00004822       00 0A 00 12 00 32 00 3A 00 42 00 4A 61 00   .....2.:.B.Ja.
00004830 09 58 2E 00 60 3E 2F 2C 22 AC 4E BA D7 E8 58 4F .X..`>/,".N...XO
00004840 0C 40 4D 5A 66 08 61 00 00 54 2E 00 60 26 61 00 .@MZf.a..T..`&a.
00004850 03 EC 2E 00 60 1E 61 00 04 04 2E 00 60 16 61 00 ....`.a.....`.a.
00004860 04 76 2E 00 60 0E 61 00 07 98 2E 00 60 06 61 00 .v..`.a.....`.a.
00004870 07 B0 2E 00                                     ....           

l00004874:
	tst.w	d7
	beq	$0000487C

l00004878:
	move.l	d7,d0
	bra	$00004898

l0000487C:
	move.l	$22AC(a4),-(a7)
	jsr.l	$-25E4(pc)
	move.l	d0,$22BC(a4)
	move.l	$22B0(a4),(a7)
	jsr.l	$-25F0(pc)
	addq.w	#$04,a7
	move.l	d0,$22B8(a4)
	moveq	#$+00,d0

l00004898:
	move.l	(a7)+,d7
	rts	
0000489C                                     4E 55 FF E4             NU..
000048A0 BF EC 20 1C 65 00 2E 46 48 E7 2F 00 0C AC 00 00 .. .e..FH./.....
000048B0 00 32 22 B8 64 06 70 06 60 00 03 78 42 A7 48 78 .2".d.p.`..xB.Hx
000048C0 00 20 2F 2C 22 AC 4E BA 3E 54 2E AC 22 AC 4E BA . /,".N.>T..".N.
000048D0 D6 C0 4F EF 00 0C E0 88 0C 80 00 52 4E 43 67 06 ..O........RNCg.
000048E0 70 06 60 00 03 4E 42 A7 48 78 00 02 2F 2C 22 AC p.`..NB.Hx../,".
000048F0 4E BA 3E 2A 2E AC 22 AC 4E BA D7 D4 2E AC 22 AC N.>*..".N.....".
00004900 3F 40 00 20 4E BA D7 C8 4F EF 00 0C 2A 00 30 2F ?@. N...O...*.0/
00004910 00 14 67 02 53 45 72 00 32 00 70 00 30 05 E1 80 ..g.SEr.2.p.0...
00004920 D0 80 D0 81 2E 00 48 78 00 01 48 78 00 10 2F 2C ......Hx..Hx../,
00004930 22 AC 4E BA 3D E8 2E AC 22 AC 4E BA D7 92 2E AC ".N.=...".N.....
00004940 22 AC 3F 40 00 20 4E BA D7 86 2E AC 22 AC 3F 40 ".?@. N.....".?@
00004950 00 22 4E BA D7 7A 2E AC 22 AC 3F 40 00 24 4E BA ."N..z..".?@.$N.
00004960 D7 6E 2E AC 22 AC 3F 40 00 26 4E BA D7 62 32 2F .n..".?@.&N..b2/
00004970 00 20 74 00 34 01 54 82 E9 82 42 97 2F 02 2F 2C . t.4.T...B././,
00004980 22 AC 3F 40 00 30 4E BA 3D 94 2E AC 22 AC 4E BA ".?@.0N.=...".N.
00004990 D7 3E 2E AC 22 AC 3F 40 00 28 4E BA D7 32 2E AC .>..".?@.(N..2..
000049A0 22 AC 3F 40 00 32 4E BA D7 26 2E AC 22 AC 3F 40 ".?@.2N..&..".?@
000049B0 00 34 4E BA D7 1A 2E AC 22 B0 48 78 4D 5A 3F 40 .4N.....".HxMZ?@
000049C0 00 3A 4E BA D7 48 2E AC 22 B0 42 A7 4E BA D7 C8 .:N..H..".B.N...
000049D0 2E AC 22 B0 42 A7 4E BA D7 BE 30 2F 00 3A 72 00 ..".B.N...0/.:r.
000049E0 32 00 2E AC 22 B0 2F 01 4E BA D7 EA 30 2F 00 40 2..."./.N...0/.@
000049F0 72 00 32 00 2E AC 22 B0 2F 01 4E BA D7 D8 30 2F r.2..."./.N...0/
00004A00 00 4A 72 00 32 00 2E AC 22 B0 2F 01 4E BA D7 C6 .Jr.2..."./.N...
00004A10 30 2F 00 4C 72 00 32 00 2E AC 22 B0 2F 01 4E BA 0/.Lr.2..."./.N.
00004A20 D7 B4 2E AC 22 B0 42 A7 4E BA D6 E2 30 2F 00 48 ....".B.N...0/.H
00004A30 72 00 32 00 2E AC 22 B0 2F 01 4E BA D7 98 30 2F r.2..."./.N...0/
00004A40 00 56 72 00 32 00 2E AC 22 B0 2F 01 4E BA D7 86 .Vr.2..."./.N...
00004A50 2E AC 22 B0 48 78 00 1E 4E BA D7 7A 30 2F 00 58 ..".Hx..N..z0/.X
00004A60 72 00 32 00 2E AC 22 B0 2F 01 4E BA D7 68 4F EF r.2..."./.N..hO.
00004A70 00 44 2F 2C 22 B0 42 A7 4E BA D6 92 30 2F 00 1E .D/,".B.N...0/..
00004A80 72 00 32 00 51 81 48 78 00 01 2F 01 2F 2C 22 AC r.2.Q.Hx.././,".
00004A90 4E BA 3C 8A 4F EF 00 14 78 00 2F 2C 22 AC 4E BA N.<.O...x./,".N.
00004AA0 D4 78 58 4F 72 00 12 00 48 AF 00 02 00 16 67 5E .xXOr...H.....g^
00004AB0 2F 2C 22 AC 4E BA D6 18 58 4F 42 6D FF F4 3A 2F /,".N...XOBm..:/
00004AC0 00 16 3F 40 00 14 60 36 2F 2C 22 AC 4E BA D4 4A ..?@..`6/,".N..J
00004AD0 58 4F 72 00 12 00 D3 6D FF F4 70 00 30 2D FF F4 XOr....m..p.0-..
00004AE0 2F 2C 22 B0 2F 00 4E BA D6 EC 70 00 30 2F 00 1C /,"./.N...p.0/..
00004AF0 2E AC 22 B0 2F 00 4E BA D6 DC 4F EF 00 0C 20 05 .."./.N...O... .
00004B00 2A 00 53 45 4A 40 66 C0 30 2F 00 16 D8 40 4A 6F *.SEJ@f.0/...@Jo
00004B10 00 16 66 86 2F 2C 22 B0 4E BA 3B 8A 58 4F 0C 80 ..f./,".N.;.XO..
00004B20 00 00 02 00 6C 16 2F 2C 22 B0 4E BA 3B 78 58 4F ....l./,".N.;xXO
00004B30 22 3C 00 00 02 00 92 80 2C 01 60 22 2F 2C 22 B0 "<......,.`"/,".
00004B40 4E BA 3B 62 58 4F 72 10 92 80 70 0F C2 80 2C 01 N.;bXOr...p...,.
00004B50 60 0C 2F 2C 22 B0 42 A7 4E BA D5 F0 50 4F 20 06 `./,".B.N...PO .
00004B60 2C 00 53 46 4A 40 66 EA 2F 2C 22 B0 4E BA 3B 36 ,.SFJ@f./,".N.;6
00004B70 E8 80 42 97 48 78 00 20 2F 2C 22 AC 3F 40 00 22 ..B.Hx. /,".?@."
00004B80 4E BA 3B 9A 61 00 06 02 4F EF 00 0C 4A 40 66 00 N.;.a...O...J@f.
00004B90 00 A2 2F 2C 22 B0 4E BA 3B 0C 42 97 2F 07 2F 2C ../,".N.;.B././,
00004BA0 22 AC 2F 40 00 24 4E BA 3B 74 2E AC 22 AC 4E BA "./@.$N.;t..".N.
00004BB0 D6 EC 90 87 2E 80 2F 2C 22 B0 2F 2C 22 AC 4E BA ....../,"./,".N.
00004BC0 D0 D2 4F EF 00 14 20 2F 00 18 22 00 02 81 00 00 ..O... /..".....
00004BD0 01 FF E0 88 E2 88 2A 00 3F 41 00 14 4A 41 67 02 ......*.?A..JAg.
00004BE0 52 45 42 A7 48 78 00 02 2F 2C 22 B0 4E BA 3B 2E REB.Hx../,".N.;.
00004BF0 30 2F 00 20 72 00 32 00 2E AC 22 B0 2F 01 4E BA 0/. r.2..."./.N.
00004C00 D5 D4 70 00 30 05 2E AC 22 B0 2F 00 4E BA D5 C6 ..p.0..."./.N...
00004C10 70 00 30 04 2E AC 22 B0 2F 00 4E BA D5 B8 30 2F p.0..."./.N...0/
00004C20 00 2E 72 00 32 00 2E AC 22 B0 2F 01 4E BA D5 A6 ..r.2..."./.N...
00004C30 70 00 4C ED 00 F4 FF D0 4E 5D 4E 75 BF EC 20 1C p.L.....N]Nu.. .
00004C40 65 00 2A AA 2F 2C 22 AC 61 00 0F 3C 58 4F 4A 40 e.*./,".a..<XOJ@
00004C50 66 04 70 06 60 04 61 00 05 30 4E 75 BF EC 20 1C f.p.`.a..0Nu.. .
00004C60 65 00 2A 8A 2F 2C 22 AC 61 00 0F 1C 58 4F 4A 40 e.*./,".a...XOJ@
00004C70 66 04 70 06 60 5E 48 78 00 01 48 78 FF EE 2F 2C f.p.`^Hx..Hx../,
00004C80 22 AC 4E BA 3A 98 2E AC 22 B0 48 78 60 1A 4E BA ".N.:...".Hx`.N.
00004C90 D4 7C 48 78 00 10 2F 2C 22 B0 2F 2C 22 AC 4E BA .|Hx../,"./,".N.
00004CA0 CF F2 2E AC 22 B0 42 A7 4E BA D4 EC 2E AC 22 B0 ....".B.N.....".
00004CB0 42 A7 4E BA D4 E2 2E AC 22 AC 4E BA D2 A0 72 00 B.N.....".N...r.
00004CC0 32 00 2E AC 22 B0 2F 01 4E BA D4 42 61 00 04 BA 2..."./.N..Ba...
00004CD0 4F EF 00 28 4E 75 4E 55 FF F4 BF EC 20 1C 65 00 O..(NuNU.... .e.
00004CE0 2A 0C 48 E7 23 00 2F 2C 22 AC 4E BA D2 A4 58 4F *.H.#./,".N...XO
00004CF0 0C 80 00 00 03 F3 67 06 70 06 60 00 01 8C 2F 2C ......g.p.`.../,
00004D00 22 B0 48 78 03 F3 4E BA D4 8E 50 4F 2F 2C 22 AC ".Hx..N...PO/,".
00004D10 4E BA D2 7E 2E 00 2C 07 2E AC 22 B0 2F 07 4E BA N..~..,..."./.N.
00004D20 D4 76 50 4F 60 14 2F 2C 22 AC 4E BA D2 64 2E AC .vPO`./,".N..d..
00004D30 22 B0 2F 00 4E BA D4 60 50 4F 20 07 53 87 4A 80 "./.N..`PO .S.J.
00004D40 66 E4 4A 86 66 C6 2F 2C 22 AC 4E BA D2 44 53 80 f.J.f./,".N..DS.
00004D50 2E AC 22 B0 2F 00 4E BA D4 3E 2E AC 22 AC 4E BA .."./.N..>..".N.
00004D60 D2 30 2E AC 22 AC 2F 40 00 14 4E BA D2 24 22 2F .0.."./@..N..$"/
00004D70 00 14 24 00 94 81 2E AC 22 B0 2F 01 2F 40 00 1C ..$....."././@..
00004D80 2F 42 00 20 4E BA D4 10 20 2F 00 1C 53 80 2E AC /B. N... /..S...
00004D90 22 B0 2F 00 4E BA D4 00 2E AC 22 B0 4E BA 39 06 "./.N.....".N.9.
00004DA0 29 40 3D 2C 48 78 00 01 48 78 00 04 2F 2C 22 AC )@=,Hx..Hx../,".
00004DB0 4E BA 39 6A 4F EF 00 1C 2E 2F 00 14 60 16 2F 2C N.9jO..../..`./,
00004DC0 22 AC 4E BA D1 CC 2E AC 22 B0 2F 00 4E BA D3 C8 ".N....."./.N...
00004DD0 50 4F 53 87 4A 87 66 E6 48 78 00 01 48 78 00 04 POS.J.f.Hx..Hx..
00004DE0 2F 2C 22 AC 4E BA 39 36 2E AC 22 AC 4E BA D1 A2 /,".N.96..".N...
00004DF0 52 80 E5 80 48 78 00 01 2F 00 2F 2C 22 AC 4E BA R...Hx.././,".N.
00004E00 39 1C 4F EF 00 18 60 60 2F 2C 22 AC 4E BA D1 82 9.O...``/,".N...
00004E10 2E AC 22 B0 2F 00 2F 40 00 14 4E BA D3 7A 50 4F .."././@..N..zPO
00004E20 20 2F 00 0C 02 80 3F FF FF FF 04 80 00 00 03 E9  /....?.........
00004E30 67 0E 53 80 67 0A 53 80 67 10 5F 80 67 2A 60 24 g.S.g.S.g._.g*`$
00004E40 61 00 00 4E 4A 40 67 20 60 3E 2F 2C 22 AC 4E BA a..NJ@g `>/,".N.
00004E50 D1 40 2E AC 22 B0 2F 00 4E BA D3 3C 50 4F 58 AC .@.."./.N..<POX.
00004E60 3D 2C 60 04 70 08 60 20 2F 2C 22 AC 4E BA 38 36 =,`.p.` /,".N.86
00004E70 2E AC 22 AC 2F 40 00 10 4E BA D4 22 58 4F 22 2F .."./@..N.."XO"/
00004E80 00 0C B2 80 65 82 70 00 4C DF 00 C4 4E 5D 4E 75 ....e.p.L...N]Nu
00004E90 4E 55 FF FC BF EC 20 1C 65 00 28 52 48 E7 0F 00 NU.... .e.(RH...
00004EA0 2F 2C 22 AC 4E BA D0 EA E5 80 2E 00 2E AC 22 AC /,".N.........".
00004EB0 4E BA 37 F2 58 4F 2C 00 70 12 BE 80 63 16 2F 2C N.7.XO,.p...c./,
00004EC0 22 AC 4E BA D0 CC 2A 00 2E AC 22 AC 4E BA D0 C2 ".N...*...".N...
00004ED0 58 4F 28 00 2F 2C 22 AC 4E BA D1 9A 58 4F E0 88 XO(./,".N...XO..
00004EE0 0C 80 00 52 4E 43 67 00 00 AE 20 07 E4 88 2F 2C ...RNCg... .../,
00004EF0 22 B0 2F 00 4E BA D2 A0 42 97 2F 06 2F 2C 22 AC "./.N...B././,".
00004F00 4E BA 38 1A 2E 87 2F 2C 22 B0 2F 2C 22 AC 4E BA N.8.../,"./,".N.
00004F10 CD 82 2E AC 22 AC 4E BA D1 5C 4F EF 00 18 0C 80 ....".N..\O.....
00004F20 00 00 03 EC 66 68 48 78 00 01 48 78 00 04 2F 2C ....fhHx..Hx../,
00004F30 22 AC 4E BA 37 E8 2E AC 22 B0 48 78 03 EC 4E BA ".N.7...".Hx..N.
00004F40 D2 56 4F EF 00 10 2F 2C 22 AC 4E BA D0 44 2E 00 .VO.../,".N..D..
00004F50 2E AC 22 B0 2F 07 4E BA D2 3E 50 4F 4A 87 67 2A .."./.N..>POJ.g*
00004F60 2F 2C 22 AC 4E BA D0 2A 53 80 2E AC 22 B0 2F 00 /,".N..*S..."./.
00004F70 4E BA D2 24 20 07 E5 80 2E 80 2F 2C 22 B0 2F 2C N..$ ...../,"./,
00004F80 22 AC 4E BA CD 0E 4F EF 00 10 4A 87 66 B8 58 AC ".N...O...J.f.X.
00004F90 3D 2C 70 00 60 60 20 05 E4 88 2F 2C 22 B0 2F 00 =,p.`` .../,"./.
00004FA0 4E BA D1 F4 2E AC 22 B0 4E BA 36 FA 42 97 2F 2C N.....".N.6.B./,
00004FB0 3D 2C 2F 2C 22 B0 2F 40 00 20 4E BA 37 60 2E AC =,/,"./@. N.7`..
00004FC0 22 B0 2F 04 4E BA D1 D0 58 AC 3D 2C 42 97 2F 2F "./.N...X.=,B.//
00004FD0 00 24 2F 2C 22 B0 4E BA 37 44 61 00 01 AC 22 06 .$/,".N.7Da...".
00004FE0 D2 87 42 97 2F 01 2F 2C 22 AC 3F 40 00 34 4E BA ..B././,".?@.4N.
00004FF0 37 2C 30 2F 00 34 4C ED 00 F0 FF EC 4E 5D 4E 75 7,0/.4L.....N]Nu
00005000 BF EC 20 1C 65 00 26 E6 2F 2C 22 AC 61 00 0B 78 .. .e.&./,".a..x
00005010 58 4F 4A 40 66 04 70 06 60 04 61 00 01 6C 4E 75 XOJ@f.p.`.a..lNu
00005020 4E 55 FF FC BF EC 20 1C 65 00 26 C2 48 E7 03 00 NU.... .e.&.H...
00005030 60 00 01 2C 2F 2C 22 AC 4E BA CF 94 58 4F 4A 80 `..,/,".N...XOJ.
00005040 67 0E 53 80 67 26 55 80 67 00 00 C2 60 00 00 E4 g.S.g&U.g...`...
00005050 2F 2C 22 AC 4E BA CE C2 72 00 12 00 2E AC 22 B0 /,".N...r.....".
00005060 2F 01 4E BA D0 E6 50 4F 60 00 00 F4 2F 2C 22 AC /.N...PO`.../,".
00005070 4E BA CF 1E 2E AC 22 B0 2F 00 4E BA D1 1A 2E AC N....."./.N.....
00005080 22 AC 4E BA D0 4A 7E 00 3E 00 20 07 72 00 32 00 ".N..J~.>. .r.2.
00005090 2E AC 22 B0 2F 01 4E BA D1 3C 2E AC 22 AC 4E BA .."./.N..<..".N.
000050A0 CF D4 4F EF 00 0C E0 88 0C 80 00 52 4E 43 66 48 ..O........RNCfH
000050B0 61 00 00 D6 4A 40 66 00 00 C8 20 2C 22 B8 54 80 a...J@f... ,".T.
000050C0 44 80 48 78 00 01 2F 00 2F 2C 22 B0 4E BA 36 4E D.Hx.././,".N.6N
000050D0 20 2C 22 B8 72 00 32 00 2E AC 22 B0 2F 01 4E BA  ,".r.2..."./.N.
000050E0 D0 F4 48 78 00 01 2F 2C 22 B8 2F 2C 22 B0 4E BA ..Hx../,"./,".N.
000050F0 36 2C 4F EF 00 1C 60 66 2F 07 2F 2C 22 B0 2F 2C 6,O...`f/./,"./,
00005100 22 AC 4E BA CB 8E 4F EF 00 0C 60 52 7C 00 60 1A ".N...O...`R|.`.
00005110 2F 2C 22 AC 4E BA CE 02 72 00 12 00 2E AC 22 B0 /,".N...r.....".
00005120 2F 01 4E BA D0 26 50 4F 52 46 70 03 BC 40 66 E0 /.N..&PORFp..@f.
00005130 60 2C 2F 2C 22 AC 4E BA D1 64 2E AC 22 AC 2F 40 `,/,".N..d.."./@
00005140 00 0C 4E BA 35 60 22 2F 00 0C 92 80 2E 81 2F 2C ..N.5`"/....../,
00005150 22 B0 2F 2C 22 AC 4E BA CB 3A 4F EF 00 0C 2F 2C "./,".N..:O.../,
00005160 22 AC 4E BA 35 40 2E AC 22 AC 2F 40 00 0C 4E BA ".N.5@.."./@..N.
00005170 D1 2C 58 4F 22 2F 00 08 B2 80 65 00 FE B8 70 00 .,XO"/....e...p.
00005180 4C DF 00 C0 4E 5D 4E 75 4E 55 FF FC BF EC 20 1C L...N]NuNU.... .
00005190 65 00 25 5A 48 E7 27 00 2F 2C 22 AC 4E BA 35 06 e.%ZH.'./,".N.5.
000051A0 2E 00 2E AC 22 AC 4E BA CD E8 58 4F 2C 00 20 06 ....".N...XO,. .
000051B0 E0 88 0C 80 00 52 4E 43 67 06 70 06 60 00 01 8C .....RNCg.p.`...
000051C0 20 06 72 03 C0 81 29 40 22 DC 2F 2C 22 AC 4E BA  .r...)@"./,".N.
000051D0 CD C0 29 40 22 B8 2E AC 22 AC 4E BA CD B4 29 40 ..)@"...".N...)@
000051E0 22 BC 2E AC 22 AC 4E BA CD 74 39 40 26 F0 2E AC "...".N..t9@&...
000051F0 22 AC 4E BA CD 68 39 40 26 F2 2E AC 22 AC 4E BA ".N..h9@&...".N.
00005200 CD 5C 2E AC 22 BC 2F 2C 22 AC 4E BA CB FA 50 4F .\.."./,".N...PO
00005210 32 2C 26 F2 B0 41 67 06 70 04 60 00 01 2E 2F 3C 2,&..Ag.p.`.../<
00005220 00 00 FF FF 4E BA CC 72 29 40 3C 00 2E BC 00 00 ....N..r)@<.....
00005230 FF FF 4E BA CC 64 29 40 3C 18 20 6C 3C 00 D1 FC ..N..d)@<. l<...
00005240 00 00 FF FD 29 48 3C 04 20 6C 3C 18 70 00 30 2C ....)H<. l<.p.0,
00005250 27 10 D1 C0 29 48 3C 1C 70 00 39 40 26 EE 2A 00 '...)H<.p.9@&.*.
00005260 39 40 26 F6 70 00 29 40 22 C4 29 40 22 C0 30 2C 9@&.p.)@".)@".0,
00005270 26 EC 48 78 00 01 3F 40 00 1A 61 00 07 36 50 4F &.Hx..?@..a..6PO
00005280 4A 40 67 0A 70 01 B0 AC 22 D4 66 02 7A 09 4A 45 J@g.p...".f.z.JE
00005290 66 4E 48 78 00 01 61 00 07 1A 58 4F 4A 40 67 3A fNHx..a...XOJ@g:
000052A0 4A AC 22 D8 67 2A 48 78 00 10 61 00 07 06 58 4F J.".g*Hx..a...XO
000052B0 72 02 B2 AC 22 D4 66 0A 34 2C 26 EC 66 04 39 40 r...".f.4,&.f.9@
000052C0 26 EC 34 2C 26 EC B4 40 67 06 4A 42 67 02 7A 0B &.4,&..@g.JBg.z.
000052D0 30 2C 26 EC 66 0A 7A 0A 60 06 70 00 39 40 26 EC 0,&.f.z.`.p.9@&.
000052E0 4A 45 66 1C 20 2C 22 DC 53 80 67 06 53 80 67 0A JEf. ,".S.g.S.g.
000052F0 60 0E 61 00 00 5E 2A 00 60 06 61 00 01 EA 2A 00 `.a..^*.`.a...*.
00005300 39 6F 00 12 26 EC 2F 2C 3C 00 4E BA CB BC 2E AC 9o..&./,<.N.....
00005310 3C 18 4E BA CB B4 20 07 D0 AC 22 BC 72 12 D0 81 <.N... ...".r...
00005320 42 97 2F 00 2F 2C 22 AC 4E BA 33 F2 4F EF 00 0C B././,".N.3.O...
00005330 4A 45 67 04 20 05 60 12 30 2C 26 F0 32 2C 26 EE JEg. .`.0,&.2,&.
00005340 B2 40 67 04 70 05 60 02 70 00 4C DF 00 E4 4E 5D .@g.p.`.p.L...N]
00005350 4E 75 BF EC 20 1C 65 00 23 94 48 E7 3B 00 60 00 Nu.. .e.#.H.;.`.
00005360 01 46 48 78 00 10 48 6C 20 6C 61 00 04 9A 48 78 .FHx..Hl la...Hx
00005370 00 10 48 6C 21 2C 61 00 04 8E 48 78 00 10 48 6C ..Hl!,a...Hx..Hl
00005380 21 EC 61 00 04 82 48 78 00 10 61 00 06 5A 4F EF !.a...Hx..a..ZO.
00005390 00 1C 7E 00 3E 00 60 00 01 04 48 6C 20 6C 61 00 ..~.>.`...Hl la.
000053A0 04 D2 58 4F 39 40 26 F4 72 00 32 00 D3 AC 22 C0 ..XO9@&.r.2...".
000053B0 4A 6C 26 F4 67 00 00 90 60 16 61 00 05 3C 32 2C Jl&.g...`.a..<2,
000053C0 26 EC B3 00 72 00 12 00 2F 01 61 00 07 0C 58 4F &...r.../.a...XO
000053D0 30 2C 26 F4 22 00 53 41 39 41 26 F4 4A 40 66 DA 0,&.".SA9A&.J@f.
000053E0 70 00 30 2C 26 EC 22 00 E2 81 2C 01 08 00 00 00 p.0,&."...,.....
000053F0 67 0C 20 06 00 40 80 00 39 40 26 EC 60 06 20 06 g. ..@..9@&.`. .
00005400 39 40 26 EC 70 00 30 2C 26 F6 72 01 24 01 E1 A2 9@&.p.0,&.r.$...
00005410 53 82 22 2C 22 C4 C2 82 74 00 34 00 70 00 20 6C S.","...t.4.p. l
00005420 3C 04 10 10 76 00 16 28 00 01 78 00 38 03 E1 84 <...v..(..x.8...
00005430 76 00 16 28 00 02 48 43 42 43 D6 84 D6 80 E5 A3 v..(..HCBC......
00005440 D6 81 29 43 22 C4 4A 87 67 5C 48 6C 21 2C 61 00 ..)C".J.g\Hl!,a.
00005450 04 22 58 4F 52 40 39 40 27 0C 48 6C 21 EC 61 00 ."XOR@9@'.Hl!.a.
00005460 04 12 58 4F 54 40 39 40 27 0E 72 00 32 00 D3 AC ..XOT@9@'.r.2...
00005470 22 C0 60 18 20 6C 3C 1C 70 00 30 2C 27 0C 91 C0 ".`. l<.p.0,'...
00005480 70 00 10 10 2F 00 61 00 06 50 58 4F 30 2C 27 0E p.../.a..PXO0,'.
00005490 22 00 53 41 39 41 27 0E 4A 40 66 D8 20 07 53 87 ".SA9A'.J@f. .S.
000054A0 4A 80 66 00 FE F6 20 2C 22 C0 B0 AC 22 B8 65 00 J.f... ,"...".e.
000054B0 FE B2 20 6C 3C 18 70 00 30 2C 27 10 22 48 D3 C0 .. l<.p.0,'."H..
000054C0 22 2C 3C 1C 92 AC 3C 18 74 00 34 00 92 82 2F 2C ",<...<.t.4.../,
000054D0 22 B0 2F 01 2F 09 4E BA CD 86 4F EF 00 0C 70 00 "././.N...O...p.
000054E0 4C DF 00 DC 4E 75 BF EC 20 1C 65 00 22 00 48 E7 L...Nu.. .e.".H.
000054F0 21 00 60 00 01 9C 61 00 04 00 32 2C 26 EC B3 00 !.`...a...2,&...
00005500 72 00 12 00 2F 01 61 00 05 D0 58 4F 70 00 30 2C r.../.a...XOp.0,
00005510 26 EC 22 00 E2 81 2E 01 08 00 00 00 67 0C 20 07 &.".........g. .
00005520 00 40 80 00 39 40 26 EC 60 06 20 07 39 40 26 EC .@..9@&.`. .9@&.
00005530 52 AC 22 C0 48 78 00 01 61 00 05 44 58 4F 4A 40 R.".Hx..a..DXOJ@
00005540 67 B4 48 78 00 01 61 00 05 36 58 4F 4A 40 67 00 g.Hx..a..6XOJ@g.
00005550 00 8C 48 78 00 01 61 00 05 26 58 4F 4A 40 66 16 ..Hx..a..&XOJ@f.
00005560 39 7C 00 02 27 0E 61 00 03 90 72 00 12 00 52 81 9|..'.a...r...R.
00005570 39 41 27 0C 60 2E 48 78 00 01 61 00 05 02 58 4F 9A'.`.Hx..a...XO
00005580 4A 40 66 08 39 7C 00 03 27 0E 60 14 61 00 03 6A J@f.9|..'.`.a..j
00005590 72 00 12 00 50 81 39 41 27 0E 51 41 67 00 00 E8 r...P.9A'.QAg...
000055A0 61 00 01 7C 70 00 30 2C 27 0E D1 AC 22 C0 60 18 a..|p.0,'...".`.
000055B0 20 6C 3C 1C 70 00 30 2C 27 0C 91 C0 70 00 10 10  l<.p.0,'...p...
000055C0 2F 00 61 00 05 14 58 4F 30 2C 27 0E 22 00 53 41 /.a...XO0,'.".SA
000055D0 39 41 27 0E 4A 40 66 D8 60 00 FF 5A 61 00 00 F2 9A'.J@f.`..Za...
000055E0 30 2C 27 0E 72 09 B0 41 66 60 61 00 01 FC 70 00 0,'.r..Af`a...p.
000055F0 30 2C 26 F4 D1 AC 22 C0 60 16 61 00 02 FC 32 2C 0,&...".`.a...2,
00005600 26 EC B3 00 72 00 12 00 2F 01 61 00 04 CC 58 4F &...r.../.a...XO
00005610 30 2C 26 F4 22 00 53 41 39 41 26 F4 4A 40 66 DA 0,&.".SA9A&.J@f.
00005620 70 00 30 2C 26 EC 22 00 E2 81 2E 01 08 00 00 00 p.0,&.".........
00005630 67 0E 20 07 00 40 80 00 39 40 26 EC 60 00 FE F6 g. ..@..9@&.`...
00005640 20 07 39 40 26 EC 60 00 FE EC 61 00 00 D2 70 00  .9@&.`...a...p.
00005650 30 2C 27 0E D1 AC 22 C0 60 18 20 6C 3C 1C 70 00 0,'...".`. l<.p.
00005660 30 2C 27 0C 91 C0 70 00 10 10 2F 00 61 00 04 6A 0,'...p.../.a..j
00005670 58 4F 30 2C 27 0E 22 00 53 41 39 41 27 0E 4A 40 XO0,'.".SA9A'.J@
00005680 66 D8 60 00 FE B0 48 78 00 01 61 00 03 F2 58 4F f.`...Hx..a...XO
00005690 20 2C 22 C0 B0 AC 22 B8 65 00 FE 9A 20 6C 3C 18  ,"...".e... l<.
000056A0 70 00 30 2C 27 10 22 48 D3 C0 22 2C 3C 1C 92 AC p.0,'."H..",<...
000056B0 3C 18 74 00 34 00 92 82 2F 2C 22 B0 2F 01 2F 09 <.t.4.../,"././.
000056C0 4E BA CB 9C 4F EF 00 0C 70 00 4C DF 00 84 4E 75 N...O...p.L...Nu
000056D0 4E 55 FF FC BF EC 20 1C 65 00 20 12 48 78 00 01 NU.... .e. .Hx..
000056E0 61 00 03 9C 58 4F 58 40 39 40 27 0E 48 78 00 01 a...XOX@9@'.Hx..
000056F0 61 00 03 8C 58 4F 4A 40 67 20 30 2C 27 0E 48 78 a...XOJ@g 0,'.Hx
00005700 00 01 3F 40 00 04 61 00 03 76 58 4F 32 2F 00 00 ..?@..a..vXO2/..
00005710 53 41 D2 41 D2 40 39 41 27 0E 4E 5D 4E 75 4E 55 SA.A.@9A'.N]NuNU
00005720 FF FC BF EC 20 1C 65 00 1F C4 2F 02 42 6C 27 0C .... .e.../.Bl'.
00005730 48 78 00 01 61 00 03 48 58 4F 4A 40 67 00 00 82 Hx..a..HXOJ@g...
00005740 48 78 00 01 61 00 03 38 39 40 27 0C 48 78 00 01 Hx..a..89@'.Hx..
00005750 61 00 03 2C 50 4F 4A 40 67 50 30 2C 27 0C 48 78 a..,POJ@gP0,'.Hx
00005760 00 01 3F 40 00 08 61 00 03 16 58 4F 32 2F 00 04 ..?@..a...XO2/..
00005770 D2 41 D2 40 00 41 00 04 39 41 27 0C 48 78 00 01 .A.@.A..9A'.Hx..
00005780 61 00 02 FC 58 4F 4A 40 66 36 30 2C 27 0C 48 78 a...XOJ@f60,'.Hx
00005790 00 01 3F 40 00 08 61 00 02 E6 58 4F 32 2F 00 04 ..?@..a...XO2/..
000057A0 D2 41 D2 40 39 41 27 0C 60 16 30 2C 27 0C 66 10 .A.@9A'.`.0,'.f.
000057B0 48 78 00 01 61 00 02 C8 58 4F 54 40 39 40 27 0C Hx..a...XOT@9@'.
000057C0 30 2C 27 0C 3F 40 00 04 61 00 01 2E 72 00 12 00 0,'.?@..a...r...
000057D0 30 2F 00 04 74 00 34 00 E1 82 D4 81 52 82 39 42 0/..t.4.....R.9B
000057E0 27 0C 24 1F 4E 5D 4E 75 BF EC 20 1C 65 00 1E FE '.$.N]Nu.. .e...
000057F0 48 78 00 04 61 00 02 88 58 4F E5 40 06 40 00 0C Hx..a...XO.@.@..
00005800 39 40 26 F4 4E 75 BF EC 20 1C 65 00 1E E0 48 E7 9@&.Nu.. .e...H.
00005810 03 10 26 6F 00 10 3E 2F 00 16 70 00 30 07 2F 00 ..&o..>/..p.0./.
00005820 2F 0B 4E BA CB 98 48 78 00 05 61 00 01 BA 4F EF /.N...Hx..a...O.
00005830 00 0C 2E 00 4A 47 67 34 70 10 BE 40 63 02 2E 00 ....JGg4p..@c...
00005840 7C 00 60 16 48 78 00 04 61 00 01 9C 58 4F 22 06 |.`.Hx..a...XO".
00005850 C2 FC 00 0C 37 80 18 0A 52 46 BC 47 65 E6 70 00 ....7...RF.Ge.p.
00005860 30 07 2F 00 2F 0B 4E BA CC CA 50 4F 4C DF 08 C0 0././.N...POL...
00005870 4E 75 4E 55 FF FC BF EC 20 1C 65 00 1E 70 48 E7 NuNU.... .e..pH.
00005880 23 10 26 6F 00 1C 7C 00 60 02 52 46 20 06 C0 FC #.&o..|.`.RF ...
00005890 00 0C 2E 00 30 33 78 0A 67 F0 72 00 32 00 70 01 ....03x.g.r.2.p.
000058A0 24 00 E3 A2 53 82 20 2C 22 C4 C0 82 B0 B3 78 06 $...S. ,".....x.
000058B0 66 D8 70 00 30 33 78 0A 2F 00 61 00 01 2A 58 4F f.p.03x./.a..*XO
000058C0 70 02 BC 40 64 04 20 06 60 26 70 00 30 06 53 80 p..@d. .`&p.0.S.
000058D0 72 00 32 00 2F 01 2F 40 00 14 61 00 01 0A 58 4F r.2././@..a...XO
000058E0 72 01 24 2F 00 10 E5 A1 74 00 34 00 84 81 20 02 r.$/....t.4... .
000058F0 4C DF 08 C4 4E 5D 4E 75 4E 55 FF FC BF EC 20 1C L...N]NuNU.... .
00005900 65 00 1D EA 48 E7 03 00 20 6C 3C 00 D1 FC 00 00 e...H... l<.....
00005910 FF FD 22 6C 3C 04 B3 C8 66 00 00 86 2F 2C 22 AC .."l<...f.../,".
00005920 4E BA C9 7A 2E AC 22 AC 2F 40 00 0C 4E BA 2D 76 N..z.."./@..N.-v
00005930 58 4F 22 2F 00 08 92 80 2E 01 0C 87 00 00 FF FD XO"/............
00005940 63 08 20 3C 00 00 FF FD 60 02 20 07 2C 00 20 6C c. <....`. .,. l
00005950 3C 00 29 48 3C 04 2F 2C 22 AC 2F 00 2F 08 2F 40 <.)H<./,"./././@
00005960 00 14 4E BA C8 BC 4F EF 00 0C 20 2F 00 08 9E 80 ..N...O... /....
00005970 70 02 BE 80 63 02 2E 00 20 6C 3C 00 D1 C6 2F 2C p...c... l<.../,
00005980 22 AC 2F 07 2F 08 4E BA C8 98 20 07 44 80 48 78 "././.N... .D.Hx
00005990 00 01 2F 00 2F 2C 22 AC 4E BA 2D 82 4F EF 00 18 .././,".N.-.O...
000059A0 20 6C 3C 04 52 AC 3C 04 10 10 4C DF 00 C0 4E 5D  l<.R.<...L...N]
000059B0 4E 75 BF EC 20 1C 65 00 1D 34 2F 07 3E 2F 00 0A Nu.. .e..4/.>/..
000059C0 70 02 B0 AC 22 DC 66 0E 70 00 30 07 2F 00 61 00 p...".f.p.0./.a.
000059D0 00 AE 58 4F 60 0C 70 00 30 07 2F 00 61 00 00 08 ..XO`.p.0./.a...
000059E0 58 4F 2E 1F 4E 75 4E 55 FF FC BF EC 20 1C 65 00 XO..NuNU.... .e.
000059F0 1C FC 48 E7 37 00 3E 2F 00 22 7C 00 7A 01 60 70 ..H.7.>/."|.z.`p
00005A00 30 2C 26 F6 66 48 61 00 FE F0 1F 40 00 14 61 00 0,&.fHa....@..a.
00005A10 FE E8 72 00 12 2F 00 14 74 00 34 01 72 00 12 00 ..r../..t.4.r...
00005A20 70 00 30 01 E1 80 72 00 20 6C 3C 04 12 10 48 41 p.0...r. l<...HA
00005A30 42 41 76 00 16 28 00 01 48 43 42 43 E1 83 D6 81 BAv..(..HCBC....
00005A40 D6 80 D6 82 29 43 22 C4 70 10 39 40 26 F6 08 2C ....)C".p.9@&..,
00005A50 00 00 22 C7 67 02 8C 45 22 2C 22 C4 E2 89 29 41 ..".g..E","...)A
00005A60 22 C4 DA 45 30 2C 26 F6 53 40 39 40 26 F6 53 47 "..E0,&.S@9@&.SG
00005A70 4A 47 66 8C 20 06 4C DF 00 EC 4E 5D 4E 75 BF EC JGf. .L...N]Nu..
00005A80 20 1C 65 00 1C 68 48 E7 03 00 3E 2F 00 0E 7C 00  .e..hH...>/..|.
00005A90 60 3A 30 2C 26 F6 66 12 61 00 FE 5E 72 00 12 00 `:0,&.f.a..^r...
00005AA0 29 41 22 C4 70 08 39 40 26 F6 DC 46 08 2C 00 07 )A".p.9@&..F.,..
00005AB0 22 C7 67 02 52 46 22 2C 22 C4 D2 81 29 41 22 C4 ".g.RF","...)A".
00005AC0 30 2C 26 F6 53 40 39 40 26 F6 53 47 4A 47 66 C2 0,&.S@9@&.SGJGf.
00005AD0 20 06 4C DF 00 C0 4E 75 BF EC 20 1C 65 00 1C 0E  .L...Nu.. .e...
00005AE0 48 E7 31 20 1E 2F 00 17 20 6C 3C 18 22 48 D3 FC H.1 ./.. l<."H..
00005AF0 00 00 FF FF 24 6C 3C 1C B5 C9 66 52 70 00 30 2C ....$l<...fRp.0,
00005B00 27 10 22 48 D3 C0 72 00 32 00 20 3C 00 00 FF FF '."H..r.2. <....
00005B10 90 81 2F 2C 22 B0 2F 00 2F 09 4E BA C7 42 20 6C ../,"././.N..B l
00005B20 3C 1C 70 00 30 2C 27 10 91 C0 72 00 32 00 2E 81 <.p.0,'...r.2...
00005B30 2F 2C 3C 18 2F 08 4E BA 27 AC 4F EF 00 14 20 6C /,<./.N.'.O... l
00005B40 3C 18 70 00 30 2C 27 10 D1 C0 29 48 3C 1C 20 6C <.p.0,'...)H<. l
00005B50 3C 1C 10 C7 29 48 3C 1C 30 2C 26 EE 22 00 E0 49 <...)H<.0,&."..I
00005B60 74 00 14 07 76 00 16 00 B5 83 70 00 46 00 C6 80 t...v.....p.F...
00005B70 D6 83 41 EC 22 EC 30 30 38 00 B3 40 39 40 26 EE ..A.".008..@9@&.
00005B80 4C DF 04 8C 4E 75 BF EC 20 1C 65 00 1B 60 48 E7 L...Nu.. .e..`H.
00005B90 01 10 26 6F 00 0C 2F 0B 4E BA C7 02 58 4F 0C 80 ..&o../.N...XO..
00005BA0 00 00 04 00 65 08 20 3C 00 00 04 00 60 0C 2F 0B ....e. <....`./.
00005BB0 4E BA C6 EA 58 4F 02 40 FF FC 2E 00 60 50 2F 0B N...XO.@....`P/.
00005BC0 4E BA C3 9A 58 4F 0C 40 52 4E 66 40 2F 0B 4E BA N...XO.@RNf@/.N.
00005BD0 C3 8C 58 4F 72 00 32 00 02 81 00 00 FF 00 0C 81 ..XOr.2.........
00005BE0 00 00 43 00 66 14 48 78 00 01 48 78 FF FC 2F 0B ..C.f.Hx..Hx../.
00005BF0 4E BA 2B 2A 4F EF 00 0C 60 18 48 78 00 01 48 78 N.+*O...`.Hx..Hx
00005C00 FF FE 2F 0B 4E BA 2B 16 4F EF 00 0C 55 47 4A 47 ../.N.+.O...UGJG
00005C10 66 AC 20 07 4C DF 08 80 4E 75 00 00             f. .L...Nu..   

fn00005C1C()
	movem.l	d0-d2/a0-a2,-(a7)
	movea.l	$001C(a7),a0
	movea.l	$0020(a7),a1
	lea	$0012(a0),a0
	moveq	#$-80,d2
	add.b	d2,d2
	move.b	(a0)+,d2
	addx.b	d2,d2
	add.b	d2,d2
	bra	$00005CF2

l00005C3A:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005C70

l00005C40:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005C8A

l00005C46:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005C90

l00005C4C:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005C98

l00005C52:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005CA4

l00005C58:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005CAA

l00005C5E:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005CB0

l00005C64:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005CBC

l00005C6A:
	moveq	#$+03,d0

l00005C6C:
	add.b	d2,d2
	beq	$00005C3A

l00005C70:
	addx.w	d1,d1
	dbra	d0,$00005C6C

l00005C76:
	addq.w	#$02,d1

l00005C78:
	move.b	(a0)+,(a1)+
	move.b	(a0)+,(a1)+
	move.b	(a0)+,(a1)+
	move.b	(a0)+,(a1)+
	dbra	d1,$00005C78

l00005C84:
	bra	$00005CF2

l00005C86:
	add.b	d2,d2
	beq	$00005C40

l00005C8A:
	addx.w	d0,d0
	add.b	d2,d2
	beq	$00005C46

l00005C90:
	bcc	$00005CA0

l00005C92:
	subq.w	#$01,d0
	add.b	d2,d2
	beq	$00005C4C

l00005C98:
	addx.w	d0,d0
	cmp.b	#$09,d0
	beq	$00005C6A

l00005CA0:
	add.b	d2,d2
	beq	$00005C52

l00005CA4:
	bcc	$00005CC0

l00005CA6:
	add.b	d2,d2
	beq	$00005C58

l00005CAA:
	addx.w	d1,d1
	add.b	d2,d2
	beq	$00005C5E

l00005CB0:
	bcs	$00005D20

l00005CB2:
	tst.w	d1
	bne	$00005CBE

l00005CB6:
	addq.w	#$01,d1

l00005CB8:
	add.b	d2,d2
	beq	$00005C64

l00005CBC:
	addx.w	d1,d1

l00005CBE:
	rol.w	#$08,d1

l00005CC0:
	move.b	(a0)+,d1
	movea.l	a1,a2
	suba.w	d1,a2
	subq.w	#$01,a2
	lsr.w	#$01,d0
	bcc	$00005CCE

l00005CCC:
	move.b	(a2)+,(a1)+

l00005CCE:
	subq.w	#$01,d0
	tst.w	d1
	bne	$00005CE0

l00005CD4:
	move.b	(a2),d1

l00005CD6:
	move.b	d1,(a1)+
	move.b	d1,(a1)+
	dbra	d0,$00005CD6

l00005CDE:
	bra	$00005CF2

l00005CE0:
	move.b	(a2)+,(a1)+
	move.b	(a2)+,(a1)+
	dbra	d0,$00005CE0

l00005CE8:
	bra	$00005CF2

l00005CEA:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bcs	$00005CFE

l00005CF0:
	move.b	(a0)+,(a1)+

l00005CF2:
	add.b	d2,d2
	bcs	$00005CFC

l00005CF6:
	move.b	(a0)+,(a1)+
	add.b	d2,d2
	bcc	$00005CF0

l00005CFC:
	beq	$00005CEA

l00005CFE:
	moveq	#$+02,d0
	moveq	#$+00,d1
	add.b	d2,d2
	beq	$00005D32

l00005D06:
	bcc	$00005C86

l00005D0A:
	add.b	d2,d2
	beq	$00005D38

l00005D0E:
	bcc	$00005CC0

l00005D10:
	addq.w	#$01,d0
	add.b	d2,d2
	beq	$00005D3E

l00005D16:
	bcc	$00005CA0

l00005D18:
	move.b	(a0)+,d0
	beq	$00005D50

l00005D1C:
	addq.w	#$08,d0
	bra	$00005CA0

l00005D20:
	add.b	d2,d2
	beq	$00005D44

l00005D24:
	addx.w	d1,d1
	ori.w	#$0004,d1
	add.b	d2,d2
	beq	$00005D4A

l00005D2E:
	bcs	$00005CBE

l00005D30:
	bra	$00005CB8

l00005D32:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005D06

l00005D38:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005D0E

l00005D3E:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005D16

l00005D44:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005D24

l00005D4A:
	move.b	(a0)+,d2
	addx.b	d2,d2
	bra	$00005D2E

l00005D50:
	add.b	d2,d2
	bne	$00005D58

l00005D54:
	move.b	(a0)+,d2
	addx.b	d2,d2

l00005D58:
	bcs	$00005CF2

l00005D5A:
	movem.l	(a7)+,d0-d2/a0-a2
	rts	
00005D60 48 E7 78 FC 30 39 00 00 BD 18 48 40 30 39 00 00 H.x.09....H@09..
00005D70 BD 18 22 00 24 00 26 00 20 40 22 40 24 40 26 40 ..".$.&. @"@$@&@
00005D80 28 79 00 00 BD 0A 2A 79 00 00 BD 0E 28 3C 00 01 (y....*y....(<..
00005D90 00 00 D9 C4 DB C4 38 3C 07 FF 48 E4 F0 F0 48 E5 ......8<..H...H.
00005DA0 F0 F0 51 CC FF F6 70 00 72 00 53 42 20 79 00 00 ..Q...p.r.SB y..
00005DB0 BD 06 22 79 00 00 BD 02 30 C0 32 C1 52 41 51 CA .."y....0.2.RAQ.
00005DC0 FF F8 42 79 00 00 BD 12 4C DF 3F 1E 4E 75 48 E7 ..By....L.?.NuH.
00005DD0 70 00 61 00 00 82 0C 79 00 02 00 00 BD 16 65 6E p.a....y......en
00005DE0 20 39 00 00 D2 14 90 B9 00 00 D2 0C B0 BC 00 00  9..............
00005DF0 00 03 65 5A 32 39 00 00 BD 16 34 39 00 00 BD 14 ..eZ29....49....
00005E00 36 39 00 00 BD 12 30 03 52 40 B0 79 00 00 BD 18 69....0.R@.y....
00005E10 66 02 70 00 33 C0 00 00 BD 12 06 B9 00 00 00 01 f.p.3...........
00005E20 00 00 D2 0C 61 30 04 B9 00 00 00 01 00 00 D2 0C ....a0..........
00005E30 33 C3 00 00 BD 12 B2 79 00 00 BD 16 64 04 72 01 3......y....d.r.
00005E40 74 00 33 C1 00 00 BD 16 33 C2 00 00 BD 14 70 00 t.3.....3.....p.
00005E50 4C DF 00 0E 4E 75 48 E7 7F FE 33 FC 00 01 00 00 L...NuH...3.....
00005E60 BD 16 42 79 00 00 BD 14 22 79 00 00 D2 10 24 79 ..By...."y....$y
00005E70 00 00 D2 0C 26 79 00 00 BD 0A 28 79 00 00 BD 0E ....&y....(y....
00005E80 2A 79 00 00 BD 06 2C 79 00 00 BD 02 20 4A 10 18 *y....,y.... J..
00005E90 22 09 92 88 3E 01 53 41 B0 18 56 C9 FF FC 9E 41 "...>.SA..V....A
00005EA0 2C 09 9C 8A 70 00 10 12 E1 48 10 2A 00 01 D0 40 ,...p....H.*...@
00005EB0 30 33 08 00 B0 79 00 00 BD 18 67 00 00 90 22 00 03...y....g...".
00005EC0 D2 41 3A 36 18 00 78 00 38 39 00 00 BD 12 B8 40 .A:6..x.89.....@
00005ED0 62 06 D8 79 00 00 BD 18 98 40 20 4A 91 C4 10 18 b..y.....@ J....
00005EE0 B0 12 66 62 10 10 B0 2A 00 01 66 5A 36 35 18 00 ..fb...*..fZ65..
00005EF0 B6 44 63 06 78 01 36 07 60 2A B6 47 63 06 96 47 .Dc.x.6.`*.Gc..G
00005F00 98 43 36 07 B6 47 66 1C 30 06 90 43 67 16 41 F2 .C6..Gf.0..Cg.A.
00005F10 30 00 22 48 93 C4 53 40 32 00 B3 08 56 C8 FF FC 0."H..S@2...V...
00005F20 92 40 D6 41 B6 79 00 00 BD 1A 63 06 36 39 00 00 .@.A.y....c.69..
00005F30 BD 1A B6 79 00 00 BD 16 65 0C 33 C3 00 00 BD 16 ...y....e.3.....
00005F40 33 C4 00 00 BD 14 30 05 60 00 FF 6A 0C 79 00 02 3.....0.`..j.y..
00005F50 00 00 BD 16 66 18 0C 79 01 00 00 00 BD 14 63 0E ....f..y......c.
00005F60 33 FC 00 01 00 00 BD 16 42 79 00 00 BD 14 70 00 3.......By....p.
00005F70 4C DF 7F FE 4E 75 48 E7 7F FE 3A 39 00 00 BD 12 L...NuH...:9....
00005F80 7C 00 3C 39 00 00 BD 18 3E 2F 00 3E 24 79 00 00 |.<9....>/.>$y..
00005F90 D2 0C 26 79 00 00 BD 0A 28 79 00 00 BD 0E 2A 79 ..&y....(y....*y
00005FA0 00 00 BD 06 2C 79 00 00 BD 02 70 00 30 05 D0 40 ....,y....p.0..@
00005FB0 32 36 08 00 3D 86 08 00 B2 45 67 18 20 4A 91 C6 26..=....Eg. J..
00005FC0 10 18 E1 48 10 10 D0 40 37 81 08 00 BC 41 66 04 ...H...@7....Af.
00005FD0 39 86 08 00 10 12 E1 48 10 2A 00 01 D0 40 BC 73 9......H.*...@.s
00005FE0 08 00 66 06 37 85 08 00 60 0C 72 00 32 34 08 00 ..f.7...`.r.24..
00005FF0 D2 41 3D 85 18 00 39 85 08 00 20 4A 10 18 22 39 .A=...9... J.."9
00006000 00 00 D2 10 92 88 34 01 53 41 B0 18 56 C9 FF FC ......4.SA..V...
00006010 94 41 30 05 D0 40 3B 82 08 00 60 2E 30 05 D0 40 .A0..@;...`.0..@
00006020 3B 82 08 00 BA 76 08 00 67 20 32 36 08 00 3D 85 ;....v..g 26..=.
00006030 08 00 20 4A 91 C6 10 18 E1 48 10 10 D0 40 37 81 .. J.....H...@7.
00006040 08 00 BC 41 66 04 39 86 08 00 52 45 BC 45 66 02 ...Af.9...RE.Ef.
00006050 7A 00 52 4A 53 47 67 0C 53 42 B4 7C 00 01 62 BC z.RJSGg.SB.|..b.
00006060 60 00 FF 4A 33 C5 00 00 BD 12 23 CA 00 00 D2 0C `..J3.....#.....
00006070 4C DF 7F FE 4E 75 00 00                         L...Nu..       

fn00006078()
	movem.l	d2-d3/a2-a3/a6,-(a7)
	movea.l	$0038(a7),a6
	movea.l	$0018(a7),a0
	movea.l	$001C(a7),a1
	movea.l	$0020(a7),a2
	movea.l	$0024(a7),a3
	move.l	$0028(a7),d0
	move.l	$002C(a7),d1
	move.l	$0030(a7),d2
	move.l	$0034(a7),d3
	jsr.l	$-015C(a6)
	movem.l	(a7)+,d2-d3/a2-a3/a6
	rts	
000060AA                               00 00                       ..   

fn000060AC()
	rts	

fn000060AE()
	rts	

fn000060B0()
	movem.l	d5-d7/a2-a3,-(a7)
	move.l	$0018(a7),d7
	movea.l	$001C(a7),a3
	move.l	$0020(a7),d6
	move.l	d7,-(a7)
	jsr.l	$3332(pc)
	addq.w	#$04,a7
	movea.l	d0,a2
	move.l	a2,d0
	bne	$000060D2

l000060CE:
	moveq	#$-01,d0
	bra	$00006108

l000060D2:
	btst	#$0003,$0003(a2)
	beq	$000060EA

l000060DA:
	pea	$00000002
	clr.l	-(a7)
	move.l	d7,-(a7)
	jsr.l	$22A6(pc)
	lea	$000C(a7),a7

l000060EA:
	move.l	d6,-(a7)
	move.l	a3,-(a7)
	move.l	$0004(a2),-(a7)
	jsr.l	$29BA(pc)
	lea	$000C(a7),a7
	move.l	d0,d5
	tst.l	$2030(a4)
	beq	$00006106

l00006102:
	moveq	#$-01,d0
	bra	$00006108

l00006106:
	move.l	d5,d0

l00006108:
	movem.l	(a7)+,d5-d7/a2-a3
	rts	
0000610E                                           00 00               ..

fn00006110()
	movem.l	d6-d7/a3,-(a7)
	movea.l	$0010(a7),a3
	movea.l	a3,a0

l0000611A:
	tst.b	(a0)+
	bne	$0000611A

l0000611E:
	subq.l	#$01,a0
	suba.l	a3,a0
	move.l	a0,d6

l00006124:
	moveq	#$+00,d7
	move.b	(a3)+,d7
	tst.l	d7
	beq	$0000615A

l0000612C:
	subq.l	#$01,$1E22(a4)
	blt	$00006144

l00006132:
	movea.l	$1E1A(a4),a0
	addq.l	#$01,$1E1A(a4)
	move.l	d7,d0
	move.b	d0,(a0)
	moveq	#$+00,d1
	move.b	d0,d1
	bra	$00006124

l00006144:
	move.l	d7,d0
	moveq	#$+00,d1
	move.b	d0,d1
	pea	$1E16(a4)
	move.l	d1,-(a7)
	jsr.l	$113C(pc)
	addq.w	#$08,a7
	move.l	d0,d1
	bra	$00006124

l0000615A:
	pea	$1E16(a4)
	pea	$0000FFFF
	jsr.l	$112A(pc)
	addq.w	#$08,a7
	move.l	d6,d0
	movem.l	(a7)+,d6-d7/a3
	rts	

fn00006170()
	link	a5,#$FFDC
	movem.l	d4-d7/a2-a3,-(a7)
	movea.l	$0008(a5),a3
	moveq	#$+00,d6
	lea	$000C(a5),a0
	move.l	a0,$-000E(a5)

l00006186:
	move.b	(a3)+,d7
	tst.b	d7
	beq	$0000627E

l0000618E:
	moveq	#$+25,d0
	cmp.b	d0,d7
	bne	$0000624C

l00006196:
	move.b	(a3)+,d7
	moveq	#$+00,d0
	move.b	d7,d0
	subi.w	#$0064,d0
	beq	$000061F2

l000061A2:
	subi.w	#$000C,d0
	beq	$000061BE

l000061A8:
	subq.w	#$03,d0
	beq	$000061B2

l000061AC:
	subq.w	#$05,d0
	beq	$000061BE

l000061B0:
	bra	$0000621A

l000061B2:
	movea.l	$-000E(a5),a0
	movea.l	(a0)+,a2
	move.l	a0,$-000E(a5)
	bra	$0000620C

l000061BE:
	movea.l	$-000E(a5),a0
	move.l	(a0)+,d4
	move.l	a0,$-000E(a5)
	lea	$-0014(a5),a2
	moveq	#$+07,d5

l000061CE:
	tst.l	d5
	bmi	$000061E8

l000061D2:
	move.l	d4,d0
	moveq	#$+0F,d1
	and.l	d1,d0
	lea	$00BE(pc),a0
	adda.l	d0,a0
	move.b	(a0),(a2)
	subq.l	#$01,a2
	asr.l	#$04,d4
	subq.l	#$01,d5
	bra	$000061CE

l000061E8:
	clr.b	$-0013(a5)
	lea	$-001B(a5),a2
	bra	$0000620C

l000061F2:
	movea.l	$-000E(a5),a0
	move.l	(a0)+,d4
	move.l	a0,$-000E(a5)
	move.l	d4,-(a7)
	pea	$-001B(a5)
	jsr.l	$1AF6(pc)
	addq.w	#$08,a7
	lea	$-001B(a5),a2

l0000620C:
	move.l	a2,-(a7)
	jsr.l	$-00FE(pc)
	addq.w	#$04,a7
	add.l	d0,d6
	bra	$00006186

l0000621A:
	addq.l	#$01,d6
	subq.l	#$01,$1E22(a4)
	blt	$00006236

l00006222:
	movea.l	$1E1A(a4),a0
	addq.l	#$01,$1E1A(a4)
	move.l	d7,d0
	move.b	d0,(a0)
	moveq	#$+00,d1
	move.b	d0,d1
	bra	$00006186

l00006236:
	moveq	#$+00,d0
	move.b	d7,d0
	pea	$1E16(a4)
	move.l	d0,-(a7)
	jsr.l	$104C(pc)
	addq.w	#$08,a7
	move.l	d0,d1
	bra	$00006186

l0000624C:
	addq.l	#$01,d6
	subq.l	#$01,$1E22(a4)
	blt	$00006268

l00006254:
	movea.l	$1E1A(a4),a0
	addq.l	#$01,$1E1A(a4)
	move.l	d7,d0
	move.b	d0,(a0)
	moveq	#$+00,d1
	move.b	d0,d1
	bra	$00006186

l00006268:
	moveq	#$+00,d0
	move.b	d7,d0
	pea	$1E16(a4)
	move.l	d0,-(a7)
	jsr.l	$101A(pc)
	addq.w	#$08,a7
	move.l	d0,d1
	bra	$00006186

l0000627E:
	pea	$1E16(a4)
	pea	$0000FFFF
	jsr.l	$1006(pc)
	move.l	d6,d0
	movem.l	$-003C(a5),d4-d7/a2-a3
	unlk	a5
	rts	
00006296                   30 31 32 33 34 35 36 37 38 39       0123456789
000062A0 41 42 43 44 45 46 00 00 4E 55 FF E4 48 E7 23 30 ABCDEF..NU..H.#0
000062B0 26 6D 00 08 24 6D 00 0C 7E 00 2B 6D 00 18 FF EC &m..$m..~.+m....
000062C0 20 6D 00 14 10 10 67 00 01 3A 42 AD FF E4 7C 00  m....g..:B...|.
000062D0 1C 18 2B 48 00 14 43 EC 1F 11 08 31 00 03 68 00 ..+H..C....1..h.
000062E0 66 00 00 F6 70 25 BC 80 66 00 00 C0 70 25 B0 10 f...p%..f...p%..
000062F0 66 20 52 AD 00 14 4E 93 2C 00 41 EC 1F 11 08 30 f R...N.,.A....0
00006300 00 03 68 00 66 F0 70 25 BC 80 67 B4 20 07 60 00 ..h.f.p%..g. .`.
00006310 00 F4 70 2A 20 6D 00 14 B0 10 67 0C 2B 6D FF EC ..p* m....g.+m..
00006320 FF E8 58 AD FF EC 60 08 42 AD FF E8 52 AD 00 14 ..X...`.B...R...
00006330 42 AD FF F4 48 6D FF E4 2F 2D FF E8 48 6D FF F4 B...Hm../-..Hm..
00006340 2F 2D 00 10 2F 0B 2F 2D 00 14 61 00 00 C2 4F EF /-.././-..a...O.
00006350 00 18 2B 40 FF F0 4A 80 56 C1 44 01 48 81 48 C1 ..+@..J.V.D.H.H.
00006360 4A 81 67 04 2B 40 00 14 20 2D FF F4 74 FF B0 82 J.g.+@.. -..t...
00006370 66 16 4A 81 67 04 DE AD FF E4 4A 87 6F 06 20 07 f.J.g.....J.o. .
00006380 60 00 00 82 20 02 60 7C 20 2D FF F4 67 0A 72 00 `... .`| -..g.r.
00006390 12 00 2F 01 4E 92 58 4F 4A AD FF F0 66 04 20 07 ../.N.XOJ...f. .
000063A0 60 62 DE AD FF E4 60 00 FF 18 4E 93 2B 40 FF F4 `b....`...N.+@..
000063B0 20 6D FF F4 43 EC 1F 11 20 08 08 31 00 03 08 00  m..C... ..1....
000063C0 66 E8 20 2D FF F4 BC 80 67 00 FE F6 72 00 12 00 f. -....g...r...
000063D0 2F 01 4E 92 20 07 60 2C 4E 93 2B 40 FF F4 20 6D /.N. .`,N.+@.. m
000063E0 FF F4 43 EC 1F 11 20 08 08 31 00 03 08 00 66 E8 ..C... ..1....f.
000063F0 20 2D FF F4 72 00 12 00 2F 01 4E 92 58 4F 60 00  -..r.../.N.XO`.
00006400 FE C0 20 07 4C ED 0C C4 FF D0 4E 5D 4E 75 4E 55 .. .L.....N]NuNU
00006410 FF C8 48 E7 27 32 26 6D 00 08 24 6D 00 0C 70 00 ..H.'2&m..$m..p.
00006420 2B 40 FF F0 2B 40 FF EC 2B 40 FF E8 4A AD 00 18 +@..+@..+@..J...
00006430 67 0A 72 01 20 6D 00 1C 20 81 60 06 20 6D 00 1C g.r. m.. .`. m..
00006440 20 80 70 00 10 13 41 EC 1F 11 08 30 00 02 08 00  .p...A....0....
00006450 67 28 20 2D FF E8 72 0A 4E BA 2C FA 72 00 12 1B g( -..r.N.,.r...
00006460 74 0F C2 82 D0 81 2B 40 FF E8 70 00 10 13 41 EC t.....+@..p...A.
00006470 1F 11 08 30 00 02 08 00 66 D8 10 13 72 6C B0 01 ...0....f...rl..
00006480 66 0A 52 8B 70 01 2B 40 FF F0 60 1E 72 68 B0 01 f.R.p.+@..`.rh..
00006490 66 0A 52 8B 70 01 2B 40 FF EC 60 0E 72 4C B0 01 f.R.p.+@..`.rL..
000064A0 66 08 52 8B 70 01 2B 40 FF F0 4E 92 2E 00 10 13 f.R.p.+@..N.....
000064B0 72 63 B0 01 67 1E 72 6E B0 01 67 18 72 5B B0 01 rc..g.rn..g.r[..
000064C0 67 12 41 EC 1F 11 08 30 00 03 78 00 67 06 4E 92 g.A....0..x.g.N.
000064D0 2E 00 60 EE 70 FF BE 80 66 0C 20 6D 00 14 20 87 ..`.p...f. m.. .
000064E0 70 00 60 00 06 42 70 00 10 13 04 40 00 58 67 00 p.`..Bp....@.Xg.
000064F0 02 5C 57 40 67 00 04 2C 51 40 67 00 03 9A 53 40 .\W@g..,Q@g...S@
00006500 67 00 00 EE 5B 40 67 00 00 86 5B 40 67 22 53 40 g...[@g...[@g"S@
00006510 67 00 01 9E 53 40 67 00 02 1E 57 40 67 00 03 CA g...S@g...W@g...
00006520 55 40 67 00 00 C8 57 40 67 00 02 22 60 00 05 E8 U@g...W@g.."`...
00006530 20 6D 00 1C 42 90 4A AD 00 18 67 00 05 DE 20 2D  m..B.J...g... -
00006540 FF EC 66 1C 4A AD FF F0 66 16 22 6D 00 18 20 51 ..f.J...f."m.. Q
00006550 2C 6D 00 10 22 16 24 01 53 82 20 82 60 00 05 BC ,m..".$.S. .`...
00006560 4A 80 67 16 22 6D 00 18 20 51 2C 6D 00 10 20 16 J.g."m.. Q,m.. .
00006570 22 00 53 81 30 81 60 00 05 A2 22 6D 00 18 20 51 ".S.0.`..."m.. Q
00006580 22 6D 00 10 20 11 53 80 20 80 60 00 05 8E 7C 00 "m.. .S. .`...|.
00006590 20 2D FF E8 67 06 72 01 B0 81 6F 20 70 2D BE 80  -..g.r...o p-..
000065A0 67 06 72 2B BE 81 66 14 BE 80 66 04 70 FF 60 02 g.r+..f...f.p.`.
000065B0 70 00 2C 00 4E 92 2E 00 53 AD FF E8 70 30 BE 80 p.,.N...S...p0..
000065C0 66 5C 4E 92 2E 00 41 EC 1F 11 08 30 00 01 78 00 f\N...A....0..x.
000065D0 67 08 20 07 72 20 90 81 60 02 20 07 72 58 B0 81 g. .r ..`. .rX..
000065E0 66 00 00 E6 4E 92 2E 00 60 00 01 62 7C 00 60 2E f...N...`..b|.`.
000065F0 7C 00 20 2D FF E8 67 06 72 01 B0 81 6F 20 70 2D |. -..g.r...o p-
00006600 BE 80 67 06 72 2B BE 81 66 14 BE 80 66 04 70 FF ..g.r+..f...f.p.
00006610 60 02 70 00 2C 00 4E 92 2E 00 53 AD FF E8 41 EC `.p.,.N...S...A.
00006620 1F 11 08 30 00 02 78 00 66 0C 20 6D 00 14 20 87 ...0..x.f. m.. .
00006630 70 00 60 00 04 F2 42 AD FF E4 20 2D FF E4 72 0A p.`...B... -..r.
00006640 4E BA 2B 12 22 07 74 0F C2 82 D0 81 2B 40 FF E4 N.+.".t.....+@..
00006650 4E 92 2E 00 53 AD FF E8 67 0C 41 EC 1F 11 08 30 N...S...g.A....0
00006660 00 02 78 00 66 D4 4A AD 00 18 67 00 04 AE 4A 86 ..x.f.J...g...J.
00006670 6A 04 44 AD FF E4 4A AD FF F0 67 10 22 6D 00 18 j.D...J...g."m..
00006680 20 51 20 2D FF E4 20 80 60 00 04 90 4A AD FF EC  Q -.. .`...J...
00006690 67 10 22 6D 00 18 20 51 20 2D FF E4 30 80 60 00 g."m.. Q -..0.`.
000066A0 04 7A 22 6D 00 18 20 51 20 AD FF E4 60 00 04 6C .z"m.. Q ...`..l
000066B0 70 30 BE 80 6D 06 70 37 BE 80 6F 0C 20 6D 00 14 p0..m.p7..o. m..
000066C0 20 87 70 00 60 00 04 60 42 AD FF E4 20 2D FF E4  .p.`..`B... -..
000066D0 E7 80 22 07 74 07 C2 82 D0 81 2B 40 FF E4 4E 92 ..".t.....+@..N.
000066E0 2E 00 53 AD FF E8 67 0C 70 30 BE 80 6D 06 70 37 ..S...g.p0..m.p7
000066F0 BE 80 6F D8 4A AD 00 18 67 00 04 20 4A AD FF F0 ..o.J...g.. J...
00006700 67 10 22 6D 00 18 20 51 20 2D FF E4 20 80 60 00 g."m.. Q -.. .`.
00006710 04 0A 4A AD FF EC 67 10 22 6D 00 18 20 51 20 2D ..J...g."m.. Q -
00006720 FF E4 30 80 60 00 03 F4 22 6D 00 18 20 51 20 AD ..0.`..."m.. Q .
00006730 FF E4 60 00 03 E6 4A AD FF E8 66 06 70 08 2B 40 ..`...J...f.p.+@
00006740 FF E8 42 AD FF EC 70 01 2B 40 FF F0 41 EC 1F 11 ..B...p.+@..A...
00006750 08 30 00 07 78 00 66 0C 20 6D 00 14 20 87 70 00 .0..x.f. m.. .p.
00006760 60 00 03 C4 2A 07 4E 92 2E 00 20 2D FF E8 67 06 `...*.N... -..g.
00006770 72 02 B0 81 6F 38 70 30 BA 80 66 32 70 78 BE 80 r...o8p0..f2px..
00006780 67 06 70 58 BE 80 66 26 4E 92 2E 00 41 EC 1F 11 g.pX..f&N...A...
00006790 08 30 00 07 78 00 66 0C 20 6D 00 14 20 87 70 00 .0..x.f. m.. .p.
000067A0 60 00 03 84 42 AD FF E4 53 AD FF E8 60 42 41 EC `...B...S...`BA.
000067B0 1F 11 08 30 00 02 58 00 67 0A 20 05 72 30 90 81 ...0..X.g. .r0..
000067C0 2B 40 FF E4 41 EC 1F 11 08 30 00 00 58 00 67 0A +@..A....0..X.g.
000067D0 20 05 72 37 90 81 2B 40 FF E4 41 EC 1F 11 08 30  .r7..+@..A....0
000067E0 00 01 58 00 67 0A 20 05 72 57 90 81 2B 40 FF E4 ..X.g. .rW..+@..
000067F0 53 AD FF E8 67 5E 41 EC 1F 11 08 30 00 07 78 00 S...g^A....0..x.
00006800 67 52 20 2D FF E4 E9 80 2B 40 FF E4 41 EC 1F 11 gR -....+@..A...
00006810 08 30 00 02 78 00 67 0A 20 07 72 30 90 81 81 AD .0..x.g. .r0....
00006820 FF E4 41 EC 1F 11 08 30 00 00 78 00 67 0A 20 07 ..A....0..x.g. .
00006830 72 37 90 81 81 AD FF E4 41 EC 1F 11 08 30 00 01 r7......A....0..
00006840 78 00 67 0A 20 07 72 57 90 81 81 AD FF E4 4E 92 x.g. .rW......N.
00006850 2E 00 60 9C 4A AD 00 18 67 00 02 C0 4A AD FF F0 ..`.J...g...J...
00006860 67 10 22 6D 00 18 20 51 20 2D FF E4 20 80 60 00 g."m.. Q -.. .`.
00006870 02 AA 4A AD FF EC 67 10 22 6D 00 18 20 51 20 2D ..J...g."m.. Q -
00006880 FF E4 30 80 60 00 02 94 22 6D 00 18 20 51 20 AD ..0.`..."m.. Q .
00006890 FF E4 60 00 02 86 4A AD 00 18 67 0C 22 6D 00 18 ..`...J...g."m..
000068A0 20 51 52 91 20 07 10 80 53 AD FF E8 6F 1E 4E 92  QR. ...S...o.N.
000068B0 2E 00 70 FF BE 80 67 14 4A AD 00 18 67 EA 22 6D ..p...g.J...g."m
000068C0 00 18 20 51 52 91 20 07 10 80 60 DC 70 FF BE 80 .. QR. ...`.p...
000068D0 66 0C 20 6D 00 14 20 87 70 00 60 00 02 4A 41 EB f. m.. .p.`..JA.
000068E0 00 01 20 08 60 00 02 40 4A AD 00 18 67 0C 22 6D .. .`..@J...g."m
000068F0 00 18 20 51 52 91 20 07 10 80 4E 92 2E 00 70 FF .. QR. ...N...p.
00006900 BE 80 67 12 53 AD FF E8 67 0C 41 EC 1F 11 08 30 ..g.S...g.A....0
00006910 00 03 78 00 67 D2 22 6D 00 18 20 51 42 10 60 00 ..x.g."m.. QB.`.
00006920 01 FA 52 8B 70 5E B0 13 66 0E 70 01 52 8B 2B 4B ..R.p^..f.p.R.+K
00006930 FF E0 2B 40 FF C8 60 08 42 AD FF C8 2B 4B FF E0 ..+@..`.B...+K..
00006940 70 5D 20 6D FF E0 B0 10 66 02 52 8B 10 13 72 5D p] m....f.R...r]
00006950 B0 01 67 14 4A 00 66 0C 20 6D 00 14 20 87 70 00 ..g.J.f. m.. .p.
00006960 60 00 01 C4 52 8B 60 E4 2B 4B FF DC 4A AD FF C8 `...R.`.+K..J...
00006970 67 00 00 E6 2B 6D FF E0 FF D8 42 AD FF D4 20 6D g...+m....B... m
00006980 FF D8 22 6D FF DC B3 C8 67 00 00 9E 10 10 72 2D .."m....g.....r-
00006990 B0 01 66 5E 4A AD FF D4 67 58 4D E8 00 01 BD C9 ..f^J...gXM.....
000069A0 67 50 52 AD FF D8 70 00 20 6D FF D8 10 10 48 ED gPR...p. m....H.
000069B0 00 01 FF D0 22 2D FF D4 B2 80 63 0C 2B 40 FF D4 ...."-....c.+@..
000069C0 2B 41 FF D0 2B 41 FF CC BE AD FF D4 65 52 BE AD +A..+A......eR..
000069D0 FF D0 62 4C 22 2D 00 18 67 08 2C 41 22 56 74 00 ..bL"-..g.,A"Vt.
000069E0 12 82 22 6D 00 14 22 87 43 EB 00 01 20 09 60 00 .."m..".C... .`.
000069F0 01 36 72 00 12 00 B2 87 66 1E 4A AD 00 18 67 08 .6r.....f.J...g.
00006A00 2C 6D 00 18 22 56 42 11 22 6D 00 14 22 87 43 EB ,m.."VB."m..".C.
00006A10 00 01 20 09 60 00 01 10 70 00 10 10 2B 40 FF D4 .. .`...p...+@..
00006A20 52 AD FF D8 60 00 FF 58 4A AD 00 18 67 0C 22 6D R...`..XJ...g."m
00006A30 00 18 20 51 52 91 20 07 10 80 4E 92 2E 00 70 FF .. QR. ...N...p.
00006A40 BE 80 67 08 53 AD FF E8 66 00 FF 2A 22 6D 00 18 ..g.S...f..*"m..
00006A50 20 51 42 10 60 00 00 C4 2B 6D FF E0 FF D8 42 AD  QB.`...+m....B.
00006A60 FF D4 20 6D FF D8 22 6D FF DC B3 C8 67 5E 10 10 .. m.."m....g^..
00006A70 72 2D B0 01 66 40 4A AD FF D4 67 3A 4D E8 00 01 r-..f@J...g:M...
00006A80 BD C9 67 32 52 AD FF D8 70 00 20 6D FF D8 10 10 ..g2R...p. m....
00006A90 48 ED 00 01 FF D0 22 2D FF D4 B2 80 63 0C 2B 40 H....."-....c.+@
00006AA0 FF D4 2B 41 FF D0 2B 41 FF CC BE AD FF D4 65 06 ..+A..+A......e.
00006AB0 BE AD FF D0 63 32 72 00 12 00 74 00 14 00 2B 41 ....c2r...t...+A
00006AC0 FF D4 B4 87 67 22 52 AD FF D8 60 96 4A AD 00 18 ....g"R...`.J...
00006AD0 67 08 22 6D 00 18 20 51 42 10 20 6D 00 14 20 87 g."m.. QB. m.. .
00006AE0 41 EB 00 01 20 08 60 3E 4A AD 00 18 67 0C 22 6D A... .`>J...g."m
00006AF0 00 18 20 51 52 91 20 07 10 80 4E 92 2E 00 70 FF .. QR. ...N...p.
00006B00 BE 80 67 08 53 AD FF E8 66 00 FF 4E 22 6D 00 18 ..g.S...f..N"m..
00006B10 20 51 42 10 60 04 70 00 60 0C 20 6D 00 14 20 87  QB.`.p.`. m.. .
00006B20 41 EB 00 01 20 08 4C DF 4C E4 4E 5D 4E 75 00 00 A... .L.L.N]Nu..

fn00006B30()
	link	a5,#$FFC4
	movem.l	d2/d5-d7/a2-a3,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a2
	moveq	#$+00,d7
	moveq	#$+00,d6
	moveq	#$+00,d5
	moveq	#$+00,d0
	move.b	#$20,$-0005(a5)
	moveq	#$+00,d1
	move.l	d1,$-000A(a5)
	moveq	#$-01,d2
	move.l	d2,$-000E(a5)
	lea	$-0030(a5),a0
	move.b	d0,$-000F(a5)
	move.b	d0,$-0004(a5)
	move.l	d1,$-001C(a5)
	move.l	d1,$-0018(a5)
	move.l	a0,$-0034(a5)

l00006B72:
	move.b	(a3),d0
	beq	$00006BA2

l00006B76:
	moveq	#$+00,d1
	move.b	d0,d1
	subi.w	#$0020,d1
	beq	$00006B94

l00006B80:
	subq.w	#$03,d1
	beq	$00006B98

l00006B84:
	subq.w	#$08,d1
	beq	$00006B90

l00006B88:
	subq.w	#$02,d1
	bne	$00006BA2

l00006B8C:
	moveq	#$+01,d7
	bra	$00006B9E

l00006B90:
	moveq	#$+01,d6
	bra	$00006B9E

l00006B94:
	moveq	#$+01,d5
	bra	$00006B9E

l00006B98:
	move.b	#$01,$-0004(a5)

l00006B9E:
	addq.l	#$01,a3
	bra	$00006B72

l00006BA2:
	move.b	(a3),d0
	moveq	#$+30,d1
	cmp.b	d1,d0
	bne	$00006BB0

l00006BAA:
	addq.l	#$01,a3
	move.b	d1,$-0005(a5)

l00006BB0:
	moveq	#$+2A,d0
	cmp.b	(a3),d0
	bne	$00006BC2

l00006BB6:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),$-000A(a5)
	addq.l	#$01,a3
	bra	$00006BD0

l00006BC2:
	pea	$-000A(a5)
	move.l	a3,-(a7)
	jsr.l	$11D0(pc)
	addq.w	#$08,a7
	adda.l	d0,a3

l00006BD0:
	move.b	(a3),d0
	moveq	#$+2E,d1
	cmp.b	d1,d0
	bne	$00006BFA

l00006BD8:
	addq.l	#$01,a3
	moveq	#$+2A,d0
	cmp.b	(a3),d0
	bne	$00006BEC

l00006BE0:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),$-000E(a5)
	addq.l	#$01,a3
	bra	$00006BFA

l00006BEC:
	pea	$-000E(a5)
	move.l	a3,-(a7)
	jsr.l	$11A6(pc)
	addq.w	#$08,a7
	adda.l	d0,a3

l00006BFA:
	move.b	(a3),d0
	moveq	#$+6C,d1
	cmp.b	d1,d0
	bne	$00006C0C

l00006C02:
	move.b	#$01,$-000F(a5)
	addq.l	#$01,a3
	bra	$00006C14

l00006C0C:
	moveq	#$+68,d1
	cmp.b	d1,d0
	bne	$00006C14

l00006C12:
	addq.l	#$01,a3

l00006C14:
	move.b	(a3)+,d0
	moveq	#$+00,d1
	move.b	d0,d1
	move.b	d0,$-0010(a5)
	subi.w	#$0050,d1
	beq	$00006D90

l00006C26:
	subq.w	#$08,d1
	beq	$00006DA4

l00006C2C:
	subi.w	#$000B,d1
	beq	$00006E38

l00006C34:
	subq.w	#$01,d1
	beq	$00006C5C

l00006C38:
	subi.w	#$000B,d1
	beq	$00006D4C

l00006C40:
	subq.w	#$01,d1
	beq	$00006D90

l00006C46:
	subq.w	#$03,d1
	beq	$00006E00

l00006C4C:
	subq.w	#$02,d1
	beq	$00006D30

l00006C52:
	subq.w	#$03,d1
	beq	$00006DA4

l00006C58:
	bra	$00006E4E

l00006C5C:
	tst.b	$-000F(a5)
	beq	$00006C6A

l00006C62:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),d0
	bra	$00006C70

l00006C6A:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),d0

l00006C70:
	move.l	d0,$-0014(a5)
	bge	$00006C80

l00006C76:
	moveq	#$+01,d1
	neg.l	$-0014(a5)
	move.l	d1,$-0018(a5)

l00006C80:
	move.l	$-0018(a5),d0
	beq	$00006C8A

l00006C86:
	moveq	#$+2D,d1
	bra	$00006C94

l00006C8A:
	tst.b	d6
	beq	$00006C92

l00006C8E:
	moveq	#$+2B,d1
	bra	$00006C94

l00006C92:
	moveq	#$+20,d1

l00006C94:
	move.b	d1,$-0030(a5)
	moveq	#$+00,d1
	move.b	d6,d1
	or.l	d1,d0
	moveq	#$+00,d1
	move.b	d5,d1
	or.l	d1,d0
	beq	$00006CAE

l00006CA6:
	addq.l	#$01,$-0034(a5)
	addq.l	#$01,$-001C(a5)

l00006CAE:
	move.l	$-0014(a5),-(a7)
	move.l	$-0034(a5),-(a7)
	jsr.l	$0F3E(pc)
	addq.w	#$08,a7
	move.l	d0,$-0038(a5)

l00006CC0:
	move.l	$-000E(a5),d0
	tst.l	d0
	bpl	$00006CCE

l00006CC8:
	moveq	#$+01,d1
	move.l	d1,$-000E(a5)

l00006CCE:
	move.l	$-0038(a5),d0
	move.l	$-000E(a5),d1
	sub.l	d0,d1
	movem.l	a6,$-003C(a5)
	ble	$00006D14

l00006CE0:
	movea.l	$-0034(a5),a0
	adda.l	d1,a0
	move.l	d0,-(a7)
	move.l	a0,-(a7)
	move.l	$-0034(a5),-(a7)
	jsr.l	$15F6(pc)
	lea	$000C(a7),a7
	moveq	#$+00,d0
	move.b	$-0005(a5),d0
	move.l	$-003C(a5),d1
	movea.l	$-0034(a5),a0
	bra	$00006D08

l00006D06:
	move.b	d0,(a0)+

l00006D08:
	subq.l	#$01,d1
	bcc	$00006D06

l00006D0C:
	move.l	$-000E(a5),d0
	move.l	d0,$-0038(a5)

l00006D14:
	add.l	d0,$-001C(a5)
	lea	$-0030(a5),a0
	move.l	a0,$-0034(a5)
	tst.b	d7
	beq	$00006E54

l00006D26:
	move.b	#$20,$-0005(a5)
	bra	$00006E54

l00006D30:
	tst.b	$-000F(a5)
	beq	$00006D3E

l00006D36:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),d0
	bra	$00006D44

l00006D3E:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),d0

l00006D44:
	move.l	d0,$-0014(a5)
	bra	$00006CAE

l00006D4C:
	tst.b	$-000F(a5)
	beq	$00006D5A

l00006D52:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),d0
	bra	$00006D60

l00006D5A:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),d0

l00006D60:
	move.l	d0,$-0014(a5)
	tst.b	$-0004(a5)
	beq	$00006D7C

l00006D6A:
	movea.l	$-0034(a5),a0
	move.b	#$30,(a0)+
	moveq	#$+01,d1
	move.l	d1,$-001C(a5)
	move.l	a0,$-0034(a5)

l00006D7C:
	move.l	d0,-(a7)
	move.l	$-0034(a5),-(a7)
	jsr.l	$0F0A(pc)
	addq.w	#$08,a7
	move.l	d0,$-0038(a5)
	bra	$00006CC0

l00006D90:
	move.l	$-000E(a5),d0
	tst.l	d0
	bpl	$00006D9E

l00006D98:
	moveq	#$+08,d0
	move.l	d0,$-000E(a5)

l00006D9E:
	move.b	#$01,$-000F(a5)

l00006DA4:
	tst.b	$-000F(a5)
	beq	$00006DB2

l00006DAA:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),d0
	bra	$00006DB8

l00006DB2:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),d0

l00006DB8:
	move.l	d0,$-0014(a5)
	tst.b	$-0004(a5)
	beq	$00006DD8

l00006DC2:
	movea.l	$-0034(a5),a0
	move.b	#$30,(a0)+
	move.b	#$78,(a0)+
	moveq	#$+02,d1
	move.l	d1,$-001C(a5)
	move.l	a0,$-0034(a5)

l00006DD8:
	move.l	d0,-(a7)
	move.l	$-0034(a5),-(a7)
	jsr.l	$0EEE(pc)
	addq.w	#$08,a7
	move.l	d0,$-0038(a5)
	btst	#$0005,$-0010(a5)
	bne	$00006CC0

l00006DF2:
	pea	$-0030(a5)
	jsr.l	$0ADE(pc)
	addq.w	#$04,a7
	bra	$00006CC0

l00006E00:
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	movea.l	(a0),a1
	move.l	a1,$-0034(a5)
	bne	$00006E14

l00006E0C:
	lea	$0120(pc),a0
	move.l	a0,$-0034(a5)

l00006E14:
	movea.l	$-0034(a5),a0

l00006E18:
	tst.b	(a0)+
	bne	$00006E18

l00006E1C:
	subq.l	#$01,a0
	suba.l	$-0034(a5),a0
	move.l	a0,$-001C(a5)
	move.l	$-000E(a5),d0
	tst.l	d0
	bmi	$00006E54

l00006E2E:
	cmpa.l	d0,a0
	ble	$00006E54

l00006E32:
	move.l	d0,$-001C(a5)
	bra	$00006E54

l00006E38:
	moveq	#$+01,d0
	move.l	d0,$-001C(a5)
	movea.l	(a2),a0
	addq.l	#$04,(a2)
	move.l	(a0),d0
	move.b	d0,$-0030(a5)
	clr.b	$-002F(a5)
	bra	$00006E54

l00006E4E:
	moveq	#$+00,d0
	bra	$00006F24

l00006E54:
	move.l	$-001C(a5),d0
	move.l	$-000A(a5),d1
	cmp.l	d0,d1
	bge	$00006E68

l00006E60:
	moveq	#$+00,d2
	move.l	d2,$-000A(a5)
	bra	$00006E6C

l00006E68:
	sub.l	d0,$-000A(a5)

l00006E6C:
	tst.b	d7
	beq	$00006EA8

l00006E70:
	subq.l	#$01,$-001C(a5)
	blt	$00006E8E

l00006E76:
	moveq	#$+00,d0
	movea.l	$-0034(a5),a0
	move.b	(a0)+,d0
	move.l	d0,-(a7)
	move.l	a0,$-0034(a5)
	movea.l	$0010(a5),a0
	jsr.l	(a0)
	addq.w	#$04,a7
	bra	$00006E70

l00006E8E:
	subq.l	#$01,$-000A(a5)
	blt	$00006F22

l00006E96:
	moveq	#$+00,d0
	move.b	$-0005(a5),d0
	move.l	d0,-(a7)
	movea.l	$0010(a5),a0
	jsr.l	(a0)
	addq.w	#$04,a7
	bra	$00006E8E

l00006EA8:
	tst.l	$-0018(a5)
	bne	$00006EB6

l00006EAE:
	tst.b	d5
	bne	$00006EB6

l00006EB2:
	tst.b	d6
	beq	$00006EEC

l00006EB6:
	movea.l	$-0034(a5),a0
	move.b	(a0),d0
	moveq	#$+20,d1
	cmp.b	d1,d0
	beq	$00006ECE

l00006EC2:
	moveq	#$+2B,d1
	cmp.b	d1,d0
	beq	$00006ECE

l00006EC8:
	moveq	#$+2D,d1
	cmp.b	d1,d0
	bne	$00006EEC

l00006ECE:
	move.l	$-001C(a5),d0
	tst.l	d0
	bmi	$00006EEC

l00006ED6:
	moveq	#$+00,d0
	move.b	(a0)+,d0
	move.l	d0,-(a7)
	move.l	a0,$-0034(a5)
	movea.l	$0010(a5),a0
	jsr.l	(a0)
	addq.w	#$04,a7
	subq.l	#$01,$-001C(a5)

l00006EEC:
	subq.l	#$01,$-000A(a5)
	blt	$00006F04

l00006EF2:
	moveq	#$+00,d0
	move.b	$-0005(a5),d0
	move.l	d0,-(a7)
	movea.l	$0010(a5),a0
	jsr.l	(a0)
	addq.w	#$04,a7
	bra	$00006EEC

l00006F04:
	subq.l	#$01,$-001C(a5)
	blt	$00006F22

l00006F0A:
	moveq	#$+00,d0
	movea.l	$-0034(a5),a0
	move.b	(a0)+,d0
	move.l	d0,-(a7)
	move.l	a0,$-0034(a5)
	movea.l	$0010(a5),a0
	jsr.l	(a0)
	addq.w	#$04,a7
	bra	$00006F04

l00006F22:
	move.l	a3,d0

l00006F24:
	movem.l	(a7)+,d2/d5-d7/a2-a3
	unlk	a5
	rts	
00006F2C                                     00 00                   .. 

fn00006F2E()
	link	a5,#$FFF4
	movem.l	d7/a2-a3,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a2
	move.l	$0010(a5),$-000A(a5)

l00006F44:
	move.b	(a2)+,d7
	tst.b	d7
	beq	$00006F7E

l00006F4A:
	moveq	#$+25,d0
	cmp.b	d0,d7
	bne	$00006F72

l00006F50:
	cmp.b	(a2),d0
	bne	$00006F58

l00006F54:
	addq.l	#$01,a2
	bra	$00006F72

l00006F58:
	move.l	a3,-(a7)
	pea	$-000A(a5)
	move.l	a2,-(a7)
	bsr	$00006B30
	lea	$000C(a7),a7
	move.l	d0,$-0006(a5)
	beq	$00006F72

l00006F6E:
	movea.l	d0,a2
	bra	$00006F44

l00006F72:
	moveq	#$+00,d0
	move.b	d7,d0
	move.l	d0,-(a7)
	jsr.l	(a3)
	addq.w	#$04,a7
	bra	$00006F44

l00006F7E:
	movem.l	(a7)+,d7/a2-a3
	unlk	a5
	rts	
00006F86                   00 00                               ..       

fn00006F88()
	movem.l	a3/a6,-(a7)
	movea.l	$000C(a7),a3
	move.l	a3,d1
	movea.l	$47A4(a4),a6
	jsr.l	$-0048(a6)
	tst.l	d0
	bne	$00006FB0

l00006F9E:
	jsr.l	$-0084(a6)
	move.l	d0,$2030(a4)
	moveq	#$+02,d0
	move.l	d0,$47A0(a4)
	moveq	#$-01,d0
	bra	$00006FB2

l00006FB0:
	moveq	#$+00,d0

l00006FB2:
	movem.l	(a7)+,a3/a6
	rts	

fn00006FB8()
	link	a5,#$FFEC
	movem.l	d2/d7/a2-a3/a6,-(a7)
	movea.l	$0008(a5),a3

l00006FC4:
	cmpi.l	#$00000020,$46DE(a4)
	bge	$000070B4

l00006FD0:
	move.b	(a3),d0
	moveq	#$+20,d1
	cmp.b	d1,d0
	beq	$00006FE4

l00006FD8:
	moveq	#$+09,d1
	cmp.b	d1,d0
	beq	$00006FE4

l00006FDE:
	moveq	#$+0A,d1
	cmp.b	d1,d0
	bne	$00006FE8

l00006FE4:
	addq.l	#$01,a3
	bra	$00006FD0

l00006FE8:
	move.b	(a3),d0
	beq	$000070B4

l00006FEE:
	move.l	$46DE(a4),d1
	asl.l	#$02,d1
	addq.l	#$01,$46DE(a4)
	lea	$46E6(a4),a0
	adda.l	d1,a0
	movea.l	a0,a2
	moveq	#$+22,d1
	cmp.b	d1,d0
	bne	$0000708C

l00007008:
	addq.l	#$01,a3
	movea.l	a3,a0
	move.l	a0,(a2)
	move.l	a0,$-0014(a5)

l00007012:
	move.b	(a3),d0
	moveq	#$+22,d1
	cmp.b	d1,d0
	beq	$0000707C

l0000701A:
	tst.b	d0
	beq	$0000707C

l0000701E:
	moveq	#$+2A,d1
	cmp.b	d1,d0
	bne	$00007070

l00007024:
	addq.l	#$01,a3
	moveq	#$+00,d0
	move.b	(a3),d0
	subi.w	#$0030,d0
	beq	$0000703E

l00007030:
	subi.w	#$0015,d0
	beq	$00007046

l00007036:
	subi.w	#$0009,d0
	beq	$00007054

l0000703C:
	bra	$00007062

l0000703E:
	movea.l	$-0014(a5),a0
	clr.b	(a0)
	bra	$000070B4

l00007046:
	movea.l	$-0014(a5),a0
	move.b	#$17,(a0)+
	move.l	a0,$-0014(a5)
	bra	$0000706C

l00007054:
	movea.l	$-0014(a5),a0
	move.b	#$0A,(a0)+
	move.l	a0,$-0014(a5)
	bra	$0000706C

l00007062:
	movea.l	$-0014(a5),a0
	move.b	(a3),(a0)+
	move.l	a0,$-0014(a5)

l0000706C:
	addq.l	#$01,a3
	bra	$00007012

l00007070:
	movea.l	$-0014(a5),a0
	move.b	(a3)+,(a0)+
	move.l	a0,$-0014(a5)
	bra	$00007012

l0000707C:
	addq.l	#$01,a3
	movea.l	$-0014(a5),a0
	clr.b	(a0)+
	move.l	a0,$-0014(a5)
	bra	$00006FC4

l0000708C:
	move.l	a3,(a2)

l0000708E:
	move.b	(a3),d0
	beq	$000070A8

l00007092:
	moveq	#$+20,d1
	cmp.b	d1,d0
	beq	$000070A8

l00007098:
	moveq	#$+09,d1
	cmp.b	d1,d0
	beq	$000070A8

l0000709E:
	moveq	#$+0A,d1
	cmp.b	d1,d0
	beq	$000070A8

l000070A4:
	addq.l	#$01,a3
	bra	$0000708E

l000070A8:
	tst.b	(a3)
	bne	$000070AE

l000070AC:
	bra	$000070B4

l000070AE:
	clr.b	(a3)+
	bra	$00006FC4

l000070B4:
	move.l	$46DE(a4),d0
	bne	$000070C0

l000070BA:
	movea.l	$2054(a4),a0
	bra	$000070C4

l000070C0:
	lea	$46E6(a4),a0

l000070C4:
	move.l	a0,$46E2(a4)
	tst.l	d0
	bne	$00007148

l000070CC:
	lea	$0138(pc),a0
	lea	$46A4(a4),a1
	move.l	(a0)+,(a1)+
	move.l	(a0)+,(a1)+
	move.l	(a0)+,(a1)+
	move.l	(a0)+,(a1)+
	move.w	(a0),(a1)
	movea.l	$2054(a4),a1
	movea.l	$0024(a1),a0
	pea	$00000028
	move.l	$0004(a0),-(a7)
	pea	$46A4(a4)
	jsr.l	$09E6(pc)
	lea	$000C(a7),a7
	lea	$46A4(a4),a0
	move.l	a0,d1
	move.l	#$000003EE,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-001E(a6)
	move.l	d0,$4564(a4)
	move.l	d0,$456C(a4)
	moveq	#$+10,d1
	move.l	d1,$4568(a4)
	move.l	d0,$4574(a4)
	move.l	d1,$4570(a4)
	asl.l	#$02,d0
	move.l	d0,$-0010(a5)
	suba.l	a1,a1
	movea.l	$2050(a4),a6
	jsr.l	$-0126(a6)
	movea.l	$-0010(a5),a0
	movea.l	d0,a1
	move.l	$0008(a0),$00A4(a1)
	moveq	#$+00,d7
	move.l	d0,$-000C(a5)
	bra	$00007184

l00007148:
	movea.l	$47A4(a4),a6
	jsr.l	$-0036(a6)
	move.l	d0,$4564(a4)
	jsr.l	$-003C(a6)
	move.l	d0,$456C(a4)
	lea	$00BA(pc),a0
	move.l	a0,d1
	move.l	#$000003ED,d2
	jsr.l	$-001E(a6)
	move.l	d0,$4574(a4)
	tst.l	d0
	bne	$00007182

l00007174:
	lea	$00A4(pc),a0
	move.l	a0,d1
	jsr.l	$-001E(a6)
	move.l	d0,$4574(a4)

l00007182:
	moveq	#$+10,d7

l00007184:
	move.l	d7,d0
	ori.w	#$8001,d0
	or.l	d0,$4560(a4)
	move.l	d7,d0
	ori.w	#$8002,d0
	or.l	d0,$4568(a4)
	ori.l	#$00008003,$4570(a4)
	tst.l	$1E5C(a4)
	beq	$000071AA

l000071A6:
	moveq	#$+00,d0
	bra	$000071B0

l000071AA:
	move.l	#$00008000,d0

l000071B0:
	move.l	d0,d7
	clr.l	$1E10(a4)
	move.l	d7,d0
	ori.w	#$0001,d0
	move.l	d0,$1E0C(a4)
	moveq	#$+01,d0
	move.l	d0,$1E32(a4)
	move.l	d7,d0
	ori.w	#$0002,d0
	move.l	d0,$1E2E(a4)
	moveq	#$+02,d0
	move.l	d0,$1E54(a4)
	move.l	d7,d0
	ori.w	#$0080,d0
	move.l	d0,$1E50(a4)
	lea	$2058(pc),a0
	move.l	a0,$2048(a4)
	move.l	$46E2(a4),-(a7)
	move.l	$46DE(a4),-(a7)
	jsr.l	$-5FF8(pc)
	clr.l	(a7)
	jsr.l	$1862(pc)
	movem.l	$-0028(a5),d2/d7/a2-a3/a6
	unlk	a5
	rts	
00007204             63 6F 6E 3A 31 30 2F 31 30 2F 33 32     con:10/10/32
00007210 30 2F 38 30 2F 00 2A 00 4E 49 4C 3A 00 00 00 00 0/80/.*.NIL:....
00007220 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00007230 00 00 70 61                                     ..pa           

fn00007234()
	move.l	a3,-(a7)
	movea.l	$0008(a7),a3
	tst.l	$0014(a3)
	beq	$0000724C

l00007240:
	btst	#$0003,$001B(a3)
	bne	$0000724C

l00007248:
	moveq	#$+00,d0
	bra	$00007280

l0000724C:
	move.l	$2014(a4),-(a7)
	jsr.l	$11EC(pc)
	addq.w	#$04,a7
	move.l	d0,$0004(a3)
	move.l	d0,$0010(a3)
	bne	$0000726A

l00007260:
	moveq	#$+0C,d0
	move.l	d0,$47A0(a4)
	moveq	#$-01,d0
	bra	$00007280

l0000726A:
	move.l	$2014(a4),$0014(a3)
	moveq	#$-0D,d0
	and.l	d0,$0018(a3)
	moveq	#$+00,d0
	move.l	d0,$000C(a3)
	move.l	d0,$0008(a3)

l00007280:
	movea.l	(a7)+,a3
	rts	
00007284             00 00 00 00 00 00 70 61                 ......pa   

fn0000728C()
	link	a5,#$FFEC
	movem.l	d2/d4-d7/a3,-(a7)
	move.l	$0008(a5),d7
	movea.l	$000C(a5),a3
	move.l	d7,d4
	moveq	#$+31,d0
	and.l	$0018(a3),d0
	beq	$000072AC

l000072A6:
	moveq	#$-01,d0
	bra	$0000750E

l000072AC:
	btst	#$0007,$001A(a3)
	sne	d0
	neg.b	d0
	ext.w	d0
	ext.l	d0
	move.l	d0,d6
	tst.l	$0014(a3)
	bne	$00007342

l000072C4:
	btst	#$0002,$001B(a3)
	bne	$00007342

l000072CC:
	moveq	#$+00,d0
	move.l	d0,$000C(a3)
	moveq	#$-01,d1
	cmp.l	d1,d7
	beq	$0000750E

l000072DA:
	move.l	a3,-(a7)
	jsr.l	$-00A8(pc)
	addq.w	#$04,a7
	tst.l	d0
	beq	$000072F2

l000072E6:
	bset	#$0005,$001B(a3)
	moveq	#$-01,d0
	bra	$0000750E

l000072F2:
	bset	#$0001,$001B(a3)
	tst.b	d6
	beq	$0000730A

l000072FC:
	move.l	$0014(a3),d0
	move.l	d0,d1
	neg.l	d1
	move.l	d1,$000C(a3)
	bra	$00007312

l0000730A:
	move.l	$0014(a3),d0
	move.l	d0,$000C(a3)

l00007312:
	subq.l	#$01,$000C(a3)
	blt	$0000732A

l00007318:
	movea.l	$0004(a3),a0
	addq.l	#$01,$0004(a3)
	move.l	d7,d0
	move.b	d0,(a0)
	moveq	#$+00,d1
	move.b	d0,d1
	bra	$0000733C

l0000732A:
	move.l	d7,d0
	moveq	#$+00,d1
	move.b	d0,d1
	move.l	a3,-(a7)
	move.l	d1,-(a7)
	bsr	$0000728C
	addq.w	#$08,a7
	move.l	d0,d1

l0000733C:
	move.l	d1,d0
	bra	$0000750E

l00007342:
	btst	#$0002,$001B(a3)
	beq	$000073A2

l0000734A:
	moveq	#$-01,d0
	cmp.l	d0,d7
	bne	$00007356

l00007350:
	moveq	#$+00,d0
	bra	$0000750E

l00007356:
	move.l	d7,d0
	move.b	d0,$-0001(a5)
	tst.b	d6
	beq	$00007382

l00007360:
	moveq	#$+0A,d0
	cmp.l	d0,d7
	bne	$00007382

l00007366:
	moveq	#$+02,d0
	move.l	d0,-(a7)
	pea	$01AC(pc)
	move.l	$001C(a3),-(a7)
	move.l	d0,$-0010(a5)
	jsr.l	$-12C6(pc)
	lea	$000C(a7),a7
	move.l	d0,d5
	bra	$0000739C

l00007382:
	moveq	#$+01,d0
	move.l	d0,-(a7)
	pea	$-0001(a5)
	move.l	$001C(a3),-(a7)
	move.l	d0,$-0010(a5)
	jsr.l	$-12E2(pc)
	lea	$000C(a7),a7
	move.l	d0,d5

l0000739C:
	moveq	#$-01,d7
	bra	$0000747C

l000073A2:
	bset	#$0001,$001B(a3)
	tst.b	d6
	beq	$000073FA

l000073AC:
	moveq	#$-01,d0
	cmp.l	d0,d7
	beq	$000073FA

l000073B2:
	addq.l	#$02,$000C(a3)
	moveq	#$+0A,d1
	cmp.l	d1,d7
	bne	$000073DE

l000073BC:
	movea.l	$0004(a3),a0
	addq.l	#$01,$0004(a3)
	move.b	#$0D,(a0)
	move.l	$000C(a3),d1
	tst.l	d1
	bmi	$000073DA

l000073D0:
	move.l	a3,-(a7)
	move.l	d0,-(a7)
	bsr	$0000728C
	addq.w	#$08,a7

l000073DA:
	addq.l	#$01,$000C(a3)

l000073DE:
	movea.l	$0004(a3),a0
	addq.l	#$01,$0004(a3)
	move.l	d7,d0
	move.b	d0,(a0)
	move.l	$000C(a3),d0
	tst.l	d0
	bpl	$000073F8

l000073F2:
	move.l	d7,d0
	bra	$0000750E

l000073F8:
	moveq	#$-01,d7

l000073FA:
	move.l	$0004(a3),d0
	sub.l	$0010(a3),d0
	move.l	d0,$-0010(a5)
	beq	$0000747A

l00007408:
	btst	#$0006,$001A(a3)
	beq	$00007462

l00007410:
	pea	$00000002
	clr.l	-(a7)
	move.l	$001C(a3),-(a7)
	jsr.l	$0F6E(pc)
	lea	$000C(a7),a7
	move.l	d0,$-0014(a5)
	tst.b	d6
	beq	$00007462

l0000742A:
	subq.l	#$01,$-0014(a5)
	blt	$00007462

l00007430:
	clr.l	-(a7)
	move.l	$-0014(a5),-(a7)
	move.l	$001C(a3),-(a7)
	jsr.l	$0F4E(pc)
	pea	$00000001
	pea	$-0003(a5)
	move.l	$001C(a3),-(a7)
	jsr.l	$0B66(pc)
	lea	$0018(a7),a7
	tst.l	$2030(a4)
	bne	$00007462

l00007458:
	move.b	$-0003(a5),d0
	moveq	#$+1A,d1
	cmp.b	d1,d0
	beq	$0000742A

l00007462:
	move.l	$-0010(a5),-(a7)
	move.l	$0010(a3),-(a7)
	move.l	$001C(a3),-(a7)
	jsr.l	$-13BE(pc)
	lea	$000C(a7),a7
	move.l	d0,d5
	bra	$0000747C

l0000747A:
	moveq	#$+00,d5

l0000747C:
	moveq	#$-01,d0
	cmp.l	d0,d5
	bne	$0000748A

l00007482:
	bset	#$0005,$001B(a3)
	bra	$00007496

l0000748A:
	cmp.l	$-0010(a5),d5
	beq	$00007496

l00007490:
	bset	#$0004,$001B(a3)

l00007496:
	tst.b	d6
	beq	$000074A8

l0000749A:
	move.l	$0014(a3),d1
	move.l	d1,d2
	neg.l	d2
	move.l	d2,$000C(a3)
	bra	$000074C0

l000074A8:
	btst	#$0002,$001B(a3)
	beq	$000074B8

l000074B0:
	moveq	#$+00,d1
	move.l	d1,$000C(a3)
	bra	$000074C0

l000074B8:
	move.l	$0014(a3),d1
	move.l	d1,$000C(a3)

l000074C0:
	movea.l	$0010(a3),a0
	move.l	a0,$0004(a3)
	cmp.l	d0,d7
	beq	$000074F6

l000074CC:
	subq.l	#$01,$000C(a3)
	blt	$000074E4

l000074D2:
	movea.l	$0004(a3),a0
	addq.l	#$01,$0004(a3)
	move.l	d7,d0
	move.b	d0,(a0)
	moveq	#$+00,d1
	move.b	d0,d1
	bra	$000074F6

l000074E4:
	move.l	d7,d0
	moveq	#$+00,d1
	move.b	d0,d1
	move.l	a3,-(a7)
	move.l	d1,-(a7)
	bsr	$0000728C
	addq.w	#$08,a7
	move.l	d0,d1

l000074F6:
	moveq	#$+30,d0
	and.l	$0018(a3),d0
	beq	$00007502

l000074FE:
	moveq	#$-01,d0
	bra	$0000750E

l00007502:
	moveq	#$-01,d0
	cmp.l	d0,d4
	bne	$0000750C

l00007508:
	moveq	#$+00,d0
	bra	$0000750E

l0000750C:
	move.l	d4,d0

l0000750E:
	movem.l	(a7)+,d2/d4-d7/a3
	unlk	a5
	rts	
00007516                   0D 0A 00 00 00 00 00 00 00 00       ..........
00007520 00 00 70 61 48 E7 07 10 26 6F 00 14 08 2B 00 07 ..paH...&o...+..
00007530 00 1A 56 C0 44 00 48 80 48 C0 2E 00 70 30 C0 AB ..V.D.H.H...p0..
00007540 00 18 67 0A 42 AB 00 08 70 FF 60 00 01 46 08 2B ..g.B...p.`..F.+
00007550 00 07 00 1B 67 14 08 2B 00 06 00 1B 67 0C 2F 0B ....g..+....g./.
00007560 48 78 FF FF 4E BA FD 26 50 4F 4A AB 00 14 66 34 Hx..N..&POJ...f4
00007570 42 AB 00 08 08 2B 00 02 00 1B 67 10 70 01 27 40 B....+....g.p.'@
00007580 00 14 41 EB 00 20 27 48 00 10 60 76 2F 0B 4E BA ..A.. 'H..`v/.N.
00007590 FC A4 58 4F 4A 80 67 6A 08 EB 00 05 00 1B 70 FF ..XOJ.gj......p.
000075A0 60 00 00 F0 4A 07 67 5A 54 AB 00 08 20 2B 00 08 `...J.gZT... +..
000075B0 6E 50 20 6B 00 04 52 AB 00 04 7C 00 1C 10 20 06 nP k..R...|... .
000075C0 72 0D 90 81 67 08 72 0D 90 81 67 24 60 2E 53 AB r...g.r...g$`.S.
000075D0 00 08 6D 10 20 6B 00 04 52 AB 00 04 70 00 10 10 ..m. k..R...p...
000075E0 60 00 00 B0 2F 0B 61 00 FF 3C 58 4F 60 00 00 A4 `.../.a..<XO`...
000075F0 08 EB 00 04 00 1B 70 FF 60 00 00 98 20 06 60 00 ......p.`... .`.
00007600 00 92 08 2B 00 01 00 1B 66 4E 08 EB 00 00 00 1B ...+....fN......
00007610 2F 2B 00 14 2F 2B 00 10 2F 2B 00 1C 4E BA 09 92 /+../+../+..N...
00007620 4F EF 00 0C 2A 00 4A 85 6A 06 08 EB 00 05 00 1B O...*.J.j.......
00007630 4A 85 66 06 08 EB 00 04 00 1B 4A 85 6F 1A 4A 07 J.f.......J.o.J.
00007640 67 0A 20 05 44 80 27 40 00 08 60 04 27 45 00 08 g. .D.'@..`.'E..
00007650 20 6B 00 10 27 48 00 04 70 32 C0 AB 00 18 67 16  k..'H..p2....g.
00007660 4A 07 67 08 70 FF 27 40 00 08 60 06 70 00 27 40 J.g.p.'@..`.p.'@
00007670 00 08 70 FF 60 1C 53 AB 00 08 6D 0E 20 6B 00 04 ..p.`.S...m. k..
00007680 52 AB 00 04 70 00 10 10 60 08 2F 0B 61 00 FE 96 R...p...`./.a...
00007690 58 4F 4C DF 08 E0 4E 75                         XOL...Nu       

fn00007698()
	movem.l	d5-d7,-(a7)
	move.l	$0010(a7),d7
	move.l	$1DD8(a4),d0
	subq.l	#$01,d0
	move.l	d0,d6

l000076A8:
	tst.w	d6
	bmi	$000076DC

l000076AC:
	move.l	d6,d0
	ext.l	d0
	asl.l	#$03,d0
	lea	$4560(a4),a0
	move.l	(a0,d0),d5
	tst.b	d5
	beq	$000076D8

l000076BE:
	btst	#$0004,d5
	bne	$000076D8

l000076C4:
	move.l	d6,d0
	ext.l	d0
	asl.l	#$03,d0
	lea	$4560(a4),a0
	move.l	(04,a0,d0),-(a7)
	jsr.l	$19B2(pc)
	addq.w	#$04,a7

l000076D8:
	subq.w	#$01,d6
	bra	$000076A8

l000076DC:
	move.l	d7,-(a7)
	jsr.l	$-6562(pc)
	addq.w	#$04,a7
	movem.l	(a7)+,d5-d7
	rts	
000076EA                               00 00                       ..   

fn000076EC()
	movea.l	$2058(a4),a7
	jsr.l	$19B8(pc)
	pea	$00000014
	jsr.l	$135E(pc)
	ori.b	#$00,d0
	ori.b	#$00,d0
	moveq	#$+61,d0

fn00007708()
	link	a5,#$FFF8
	move.l	a3,-(a7)
	movea.l	$46A0(a4),a3
	move.l	a3,d0
	bne	$0000771A

fn00007716()
	lea	$007E(pc),a3

fn0000771A()
	move.b	(a3),$4778(a4)
	move.b	$0001(a3),$4779(a4)
	move.b	$0002(a3),$477A(a4)
	clr.b	$477B(a4)
	lea	$4778(a4),a0
	move.l	a0,$4770(a4)
	lea	$0003(a3),a0
	pea	$-0008(a5)
	move.l	a0,-(a7)
	jsr.l	$0658(pc)
	addq.w	#$08,a7
	addq.l	#$03,d0
	adda.l	d0,a3
	move.l	$-0008(a5),d0
	move.l	#$00000E10,d1
	jsr.l	$1A00(pc)
	move.l	d0,$476C(a4)
	move.b	(a3),d0
	beq	$0000777E

l00007760:
	move.b	d0,$477C(a4)
	move.b	$0001(a3),$477D(a4)
	move.b	$0002(a3),$477E(a4)
	moveq	#$+00,d0
	move.b	d0,$477F(a4)
	moveq	#$+01,d1
	move.l	d1,$4768(a4)
	bra	$00007786

l0000777E:
	clr.b	$477C(a4)
	clr.l	$4768(a4)

l00007786:
	lea	$477C(a4),a0
	move.l	a0,$4774(a4)
	movea.l	(a7)+,a3
	unlk	a5
	rts	
00007794             43 53 54 36 00 00 00 00 00 00 00 00     CST6........
000077A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
000077B0 00 00 00 00 00 00 00 00 00 00 70 61             ..........pa   

fn000077BC()
	move.l	$0004(a7),d0
	cmpi.b	#$61,d0
	blt	$000077D0

l000077C6:
	cmpi.b	#$7A,d0
	bgt	$000077D0

l000077CC:
	subi.b	#$20,d0

l000077D0:
	rts	
000077D2       00 00                                       ..           

fn000077D4()
	link	a5,#$FFF0
	movem.l	d5-d7/a3,-(a7)
	movea.l	$0008(a5),a3
	pea	$-0008(a5)
	jsr.l	$0D34(pc)
	addq.w	#$04,a7
	moveq	#$+00,d5
	move.b	$-0007(a5),d0
	move.l	d0,d6
	addi.b	#$0A,d6
	moveq	#$+00,d7

l000077F8:
	cmp.b	d6,d7
	bcc	$0000781E

l000077FC:
	moveq	#$+00,d0
	move.b	d7,d0
	addq.l	#$02,d0
	moveq	#$+04,d1
	jsr.l	$1970(pc)
	tst.l	d1
	bne	$00007814

l0000780C:
	addi.l	#$0000016E,d5
	bra	$0000781A

l00007814:
	addi.l	#$0000016D,d5

l0000781A:
	addq.b	#$01,d7
	bra	$000077F8

l0000781E:
	moveq	#$+01,d7

l00007820:
	move.b	$-0006(a5),d0
	cmp.b	d0,d7
	bcc	$0000783C

l00007828:
	moveq	#$+00,d0
	move.b	d7,d0
	moveq	#$+00,d1
	lea	$1E5F(a4),a0
	move.b	(a0,d0),d1
	add.l	d1,d5
	addq.b	#$01,d7
	bra	$00007820

l0000783C:
	moveq	#$+00,d0
	move.b	d6,d0
	addq.l	#$02,d0
	moveq	#$+04,d1
	jsr.l	$1930(pc)
	tst.l	d1
	bne	$00007858

l0000784C:
	move.b	$-0006(a5),d0
	moveq	#$+02,d1
	cmp.b	d1,d0
	bls	$00007858

l00007856:
	addq.l	#$01,d5

l00007858:
	moveq	#$+00,d0
	move.b	$-0005(a5),d0
	subq.l	#$01,d0
	add.l	d0,d5
	move.l	d5,d0
	moveq	#$+18,d1
	jsr.l	$18EE(pc)
	move.l	d0,d5
	moveq	#$+00,d0
	move.b	$-0004(a5),d0
	add.l	d0,d5
	move.l	d5,d0
	moveq	#$+3C,d1
	jsr.l	$18DC(pc)
	move.l	d0,d5
	moveq	#$+00,d0
	move.b	$-0003(a5),d0
	add.l	d0,d5
	move.l	d5,d0
	moveq	#$+3C,d1
	jsr.l	$18CA(pc)
	move.l	d0,d5
	moveq	#$+00,d0
	move.b	$-0002(a5),d0
	add.l	d0,d5
	jsr.l	$-0190(pc)
	add.l	$476C(a4),d5
	move.l	a3,d0
	beq	$000078A6

l000078A4:
	move.l	d5,(a3)

l000078A6:
	move.l	d5,d0
	movem.l	(a7)+,d5-d7/a3
	unlk	a5
	rts	
000078B0 00 00 00 00 00 00 70 61 20 6F 00 04 22 6F 00 08 ......pa o.."o..
000078C0 20 2F 00 0C 6F 0A 12 10 10 D1 12 C1 53 80 66 F6  /..o.......S.f.
000078D0 4E 75 00 00                                     Nu..           

fn000078D4()
	movem.l	d2/a2-a3,-(a7)
	movea.l	$0010(a7),a3
	movea.l	a3,a2

l000078DE:
	move.b	(a2),d0
	beq	$00007906

l000078E2:
	moveq	#$+00,d1
	move.b	d0,d1
	lea	$1F11(a4),a0
	btst	#$0001,(a0,d1)
	beq	$000078FC

l000078F2:
	moveq	#$+00,d1
	move.b	d0,d1
	moveq	#$+20,d2
	sub.l	d2,d1
	bra	$00007900

l000078FC:
	moveq	#$+00,d1
	move.b	d0,d1

l00007900:
	move.b	d1,(a2)
	addq.l	#$01,a2
	bra	$000078DE

l00007906:
	move.l	a3,d0
	movem.l	(a7)+,d2/a2-a3
	rts	
0000790E                                           00 00               ..

fn00007910()
	link	a5,#$FFB8
	movem.l	d6-d7/a2-a3,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a2
	pea	$0000003F
	move.l	a3,-(a7)
	pea	$-0048(a5)
	jsr.l	$013A(pc)
	lea	$000C(a7),a7
	clr.b	$-0009(a5)
	lea	$-0048(a5),a0
	movea.l	a0,a1

l0000793C:
	tst.b	(a1)+
	bne	$0000793C

l00007940:
	subq.l	#$01,a1
	suba.l	a0,a1
	move.l	a1,d7
	move.l	a2,d0
	beq	$0000794E

l0000794A:
	moveq	#$+00,d0
	move.b	d0,(a2)

l0000794E:
	tst.l	$0010(a5)
	beq	$0000795C

l00007954:
	moveq	#$+00,d0
	movea.l	$0010(a5),a0
	move.b	d0,(a0)

l0000795C:
	tst.l	$0014(a5)
	beq	$0000796A

l00007962:
	moveq	#$+00,d0
	movea.l	$0014(a5),a0
	move.b	d0,(a0)

l0000796A:
	tst.l	$0018(a5)
	beq	$00007976

l00007970:
	movea.l	$0018(a5),a0
	clr.b	(a0)

l00007976:
	move.l	d7,d6

l00007978:
	move.l	d6,d0
	subq.l	#$01,d6
	tst.l	d0
	ble	$000079B2

l00007980:
	moveq	#$+00,d0
	move.b	(-48,a5,d6),d0
	subi.w	#$002E,d0
	beq	$00007998

l0000798C:
	subq.w	#$01,d0
	beq	$000079B0

l00007990:
	subi.w	#$000B,d0
	beq	$000079B0

l00007996:
	bra	$00007978

l00007998:
	move.l	$0018(a5),d0
	beq	$000079AA

l0000799E:
	lea	$-0047(a5),a0
	adda.l	d6,a0
	movea.l	d0,a1

l000079A6:
	move.b	(a0)+,(a1)+
	bne	$000079A6

l000079AA:
	clr.b	(-48,a5,d6)
	bra	$000079B2

l000079B0:
	move.l	d7,d6

l000079B2:
	move.l	d6,d0
	subq.l	#$01,d6
	tst.l	d0
	ble	$000079FC

l000079BA:
	moveq	#$+00,d0
	move.b	(-48,a5,d6),d0
	subi.w	#$002F,d0
	beq	$000079E6

l000079C6:
	subi.w	#$000B,d0
	bne	$000079B2

l000079CC:
	move.l	$0014(a5),d0
	beq	$000079DE

l000079D2:
	lea	$-0047(a5),a0
	adda.l	d6,a0
	movea.l	d0,a1

l000079DA:
	move.b	(a0)+,(a1)+
	bne	$000079DA

l000079DE:
	addq.l	#$01,d6
	clr.b	(-48,a5,d6)
	bra	$000079FC

l000079E6:
	move.l	$0014(a5),d0
	beq	$000079F8

l000079EC:
	lea	$-0047(a5),a0
	adda.l	d6,a0
	movea.l	d0,a1

l000079F4:
	move.b	(a0)+,(a1)+
	bne	$000079F4

l000079F8:
	clr.b	(-48,a5,d6)

l000079FC:
	tst.l	d6
	bpl	$00007A12

l00007A00:
	move.l	$0014(a5),d0
	beq	$00007A5C

l00007A06:
	lea	$-0048(a5),a0
	movea.l	d0,a1

l00007A0C:
	move.b	(a0)+,(a1)+
	bne	$00007A0C

l00007A10:
	bra	$00007A5C

l00007A12:
	move.l	d6,d0
	subq.l	#$01,d6
	tst.l	d0
	ble	$00007A4C

l00007A1A:
	moveq	#$+00,d0
	move.b	(-48,a5,d6),d0
	subi.w	#$003A,d0
	bne	$00007A12

l00007A26:
	move.l	$0010(a5),d0
	beq	$00007A38

l00007A2C:
	lea	$-0047(a5),a0
	adda.l	d6,a0
	movea.l	d0,a1

l00007A34:
	move.b	(a0)+,(a1)+
	bne	$00007A34

l00007A38:
	clr.b	(-47,a5,d6)
	move.l	a2,d0
	beq	$00007A5C

l00007A40:
	lea	$-0048(a5),a0
	movea.l	a2,a1

l00007A46:
	move.b	(a0)+,(a1)+
	bne	$00007A46

l00007A4A:
	bra	$00007A5C

l00007A4C:
	move.l	$0010(a5),d0
	beq	$00007A5C

l00007A52:
	lea	$-0048(a5),a0
	movea.l	d0,a1

l00007A58:
	move.b	(a0)+,(a1)+
	bne	$00007A58

l00007A5C:
	movem.l	(a7)+,d6-d7/a2-a3
	unlk	a5
	rts	

fn00007A64()
	movea.l	$0008(a7),a1
	movea.l	$0004(a7),a0
	move.l	$000C(a7),d0
	move.l	a0,d1
	bra	$00007A78

l00007A74:
	move.b	(a1)+,(a0)+
	beq	$00007A80

l00007A78:
	subq.l	#$01,d0
	bcc	$00007A74

l00007A7C:
	bra	$00007A84

l00007A7E:
	clr.b	(a0)+

l00007A80:
	subq.l	#$01,d0
	bcc	$00007A7E

l00007A84:
	move.l	d1,d0
	rts	

fn00007A88()
	movem.l	d6-d7/a2-a3,-(a7)
	movea.l	$0014(a7),a3
	movea.l	$0018(a7),a2
	move.l	$001C(a7),d7

l00007A98:
	tst.l	d7
	beq	$00007ABC

l00007A9C:
	tst.b	(a3)
	beq	$00007ABC

l00007AA0:
	tst.b	(a2)
	beq	$00007ABC

l00007AA4:
	moveq	#$+00,d0
	move.b	(a3)+,d0
	moveq	#$+00,d1
	move.b	(a2)+,d1
	sub.l	d1,d0
	move.l	d0,d6
	tst.l	d6
	beq	$00007AB8

l00007AB4:
	move.l	d6,d0
	bra	$00007AD2

l00007AB8:
	subq.l	#$01,d7
	bra	$00007A98

l00007ABC:
	tst.l	d7
	beq	$00007AD0

l00007AC0:
	tst.b	(a3)
	beq	$00007AC8

l00007AC4:
	moveq	#$+01,d0
	bra	$00007AD2

l00007AC8:
	tst.b	(a2)
	beq	$00007AD0

l00007ACC:
	moveq	#$-01,d0
	bra	$00007AD2

l00007AD0:
	moveq	#$+00,d0

l00007AD2:
	movem.l	(a7)+,d6-d7/a2-a3
	rts	

fn00007AD8()
	link	a5,#$FFF8
	movem.l	d6-d7/a2-a3,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a2
	move.l	$0010(a5),d7
	movea.l	a2,a0

l00007AEE:
	tst.b	(a0)+
	bne	$00007AEE

l00007AF2:
	subq.l	#$01,a0
	suba.l	a2,a0
	move.l	a0,d6
	movea.l	a3,a0

l00007AFA:
	tst.b	(a0)+
	bne	$00007AFA

l00007AFE:
	subq.l	#$01,a0
	suba.l	a3,a0
	move.l	a0,d0
	movea.l	a3,a1
	adda.l	d0,a1
	move.l	a1,$-0008(a5)
	cmp.l	d7,d6
	bls	$00007B12

l00007B10:
	move.l	d7,d6

l00007B12:
	move.l	d6,d0
	movea.l	a2,a0
	bra	$00007B1A

l00007B18:
	move.b	(a0)+,(a1)+

l00007B1A:
	subq.l	#$01,d0
	bcc	$00007B18

l00007B1E:
	movea.l	$-0008(a5),a0
	clr.b	(a0,d6)
	move.l	a3,d0
	movem.l	(a7)+,d6-d7/a2-a3
	unlk	a5
	rts	

fn00007B30()
	movem.l	d2/a2-a3,-(a7)
	movea.l	$0010(a7),a3
	movea.l	a3,a2

l00007B3A:
	move.b	(a2),d0
	beq	$00007B62

l00007B3E:
	moveq	#$+00,d1
	move.b	d0,d1
	lea	$1F11(a4),a0
	btst	#$0000,(a0,d1)
	beq	$00007B58

l00007B4E:
	moveq	#$+00,d1
	move.b	d0,d1
	moveq	#$+20,d2
	add.l	d2,d1
	bra	$00007B5C

l00007B58:
	moveq	#$+00,d1
	move.b	d0,d1

l00007B5C:
	move.b	d1,(a2)
	addq.l	#$01,a2
	bra	$00007B3A

l00007B62:
	move.l	a3,d0
	movem.l	(a7)+,d2/a2-a3
	rts	
00007B6A                               00 00 00 00 00 00           ......
00007B70 00 00 70 61                                     ..pa           

fn00007B74()
	movea.l	$0008(a7),a1
	movea.l	$0004(a7),a0
	move.l	a0,d0

l00007B7E:
	tst.b	(a0)+
	bne	$00007B7E

l00007B82:
	subq.l	#$01,a0

l00007B84:
	move.b	(a1)+,(a0)+
	bne	$00007B84

l00007B88:
	rts	
00007B8A                               00 00 48 E7 01 10           ..H...
00007B90 26 6F 00 0C 2E 2F 00 10 70 00 10 13 B0 87 66 04 &o.../..p.....f.
00007BA0 20 0B 60 08 10 1B 4A 00 66 EE 70 00 4C DF 08 80  .`...J.f.p.L...
00007BB0 4E 75 48 E7 01 10 26 6F 00 0C 2E 2F 00 10 2F 07 NuH...&o.../../.
00007BC0 2F 0B 61 C8 50 4F 4C DF 08 80 4E 75             /.a.POL...Nu   

fn00007BCC()
	movem.l	d7/a2-a3,-(a7)
	movea.l	$0010(a7),a3
	move.l	$0014(a7),d7
	suba.l	a2,a2

l00007BDA:
	move.b	(a3),d0
	beq	$00007BEC

l00007BDE:
	moveq	#$+00,d1
	move.b	d0,d1
	cmp.l	d7,d1
	bne	$00007BE8

l00007BE6:
	movea.l	a3,a2

l00007BE8:
	addq.l	#$01,a3
	bra	$00007BDA

l00007BEC:
	move.l	a2,d0
	movem.l	(a7)+,d7/a2-a3
	rts	

fn00007BF4()
	move.l	$0008(a7),d0
	movea.l	$0004(a7),a0
	link	a5,#$FFF4
	movea.l	a7,a1

l00007C02:
	moveq	#$+0A,d1
	jsr.l	$15A2(pc)
	addi.w	#$0030,d1
	move.b	d1,(a1)+
	tst.l	d0
	bne	$00007C02

l00007C12:
	move.l	a1,d0

l00007C14:
	move.b	-(a1),(a0)+
	cmpa.l	a1,a7
	bne	$00007C14

l00007C1A:
	clr.b	(a0)
	sub.l	a7,d0
	unlk	a5
	rts	
00007C22       00 00                                       ..           

fn00007C24()
	link	a5,#$FFF8
	movem.l	d7/a2-a3,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a2
	move.l	a3,$-0004(a5)

l00007C38:
	move.b	(a2)+,d7
	tst.b	d7
	beq	$00007C76

l00007C3E:
	moveq	#$+00,d0
	move.b	d7,d0
	subi.w	#$002A,d0
	beq	$00007C58

l00007C48:
	subi.w	#$0015,d0
	bne	$00007C6C

l00007C4E:
	move.b	(a3)+,d0
	tst.b	d0
	bne	$00007C38

l00007C54:
	moveq	#$+00,d0
	bra	$00007C84

l00007C58:
	tst.b	(a3)
	beq	$00007C38

l00007C5C:
	move.l	a2,-(a7)
	move.l	a3,-(a7)
	bsr	$00007C24
	addq.w	#$08,a7
	tst.l	d0
	bne	$00007C38

l00007C68:
	addq.l	#$01,a3
	bra	$00007C58

l00007C6C:
	move.b	(a3)+,d0
	cmp.b	d0,d7
	beq	$00007C38

l00007C72:
	moveq	#$+00,d0
	bra	$00007C84

l00007C76:
	tst.b	(a3)
	beq	$00007C7E

l00007C7A:
	moveq	#$+00,d0
	bra	$00007C84

l00007C7E:
	move.l	a3,d0
	sub.l	$-0004(a5),d0

l00007C84:
	movem.l	(a7)+,d7/a2-a3
	unlk	a5
	rts	

fn00007C8C()
	move.l	$0008(a7),d0
	movea.l	$0004(a7),a0
	link	a5,#$FFF4
	movea.l	a7,a1

l00007C9A:
	move.l	d0,d1
	andi.w	#$0007,d1
	addi.w	#$0030,d1
	move.b	d1,(a1)+
	lsr.l	#$03,d0
	bne	$00007C9A

l00007CAA:
	move.l	a1,d0

l00007CAC:
	move.b	-(a1),(a0)+
	cmpa.l	a1,a7
	bne	$00007CAC

l00007CB2:
	clr.b	(a0)
	sub.l	a7,d0
	unlk	a5
	rts	
00007CBA                               00 00 30 31 32 33           ..0123
00007CC0 34 35 36 37 38 39 61 62 63 64 65 66             456789abcdef   

fn00007CCC()
	move.l	$0008(a7),d0
	movea.l	$0004(a7),a0
	lea	$0004(a7),a1

l00007CD8:
	move.w	d0,d1
	andi.w	#$000F,d1
	move.b	(-24,pc,d1),(a1)+
	lsr.l	#$04,d0
	bne	$00007CD8

l00007CE6:
	move.l	a1,d0
	move.l	a7,d1
	addq.l	#$04,d1

l00007CEC:
	move.b	-(a1),(a0)+
	cmp.l	a1,d1
	bne	$00007CEC

l00007CF2:
	clr.b	(a0)
	sub.l	d1,d0
	rts	

fn00007CF8()
	move.l	$0008(a7),d0
	movea.l	$0004(a7),a0
	link	a5,#$FFF4
	movea.l	a7,a1
	bge	$00007D0E

l00007D08:
	move.b	#$2D,(a0)+
	neg.l	d0

l00007D0E:
	moveq	#$+0A,d1
	jsr.l	$1496(pc)
	addi.w	#$0030,d1
	move.b	d1,(a1)+
	tst.l	d0
	bne	$00007D0E

l00007D1E:
	move.b	-(a1),(a0)+
	cmpa.l	a1,a7
	bne	$00007D1E

l00007D24:
	clr.b	(a0)
	move.l	a0,d0
	unlk	a5
	sub.l	$0004(a7),d0
	rts	

fn00007D30()
	link	a5,#$FFF8
	movem.l	d7/a2-a3,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a0

l00007D40:
	tst.b	(a0)+
	bne	$00007D40

l00007D44:
	subq.l	#$01,a0
	suba.l	$000C(a5),a0
	move.l	a0,d7
	movea.l	$000C(a5),a0
	adda.l	d7,a0
	movea.l	a0,a2

l00007D54:
	tst.l	d7
	ble	$00007D8A

l00007D58:
	subq.l	#$01,a2
	moveq	#$+00,d0
	move.b	(a2),d0
	subi.w	#$002F,d0
	beq	$00007D6C

l00007D64:
	subi.w	#$000B,d0
	beq	$00007D6E

l00007D6A:
	bra	$00007D86

l00007D6C:
	subq.l	#$01,d7

l00007D6E:
	move.l	d7,d0
	movea.l	$000C(a5),a0
	movea.l	a3,a1
	bra	$00007D7A

l00007D78:
	move.b	(a0)+,(a1)+

l00007D7A:
	subq.l	#$01,d0
	bcc	$00007D78

l00007D7E:
	clr.b	(a3,d7)
	move.l	d7,d0
	bra	$00007D8E

l00007D86:
	subq.l	#$01,d7
	bra	$00007D54

l00007D8A:
	clr.b	(a3)
	moveq	#$+00,d0

l00007D8E:
	movem.l	(a7)+,d7/a2-a3
	unlk	a5
	rts	
00007D96                   00 00                               ..       

fn00007D98()
	movea.l	$0004(a7),a0
	movea.l	a0,a1
	moveq	#$+00,d1
	moveq	#$+00,d0
	move.l	d2,-(a7)
	cmpi.b	#$2B,(a0)
	beq	$00007DB0

l00007DAA:
	cmpi.b	#$2D,(a0)
	bne	$00007DB2

l00007DB0:
	addq.w	#$01,a0

l00007DB2:
	move.b	(a0)+,d0
	subi.b	#$30,d0
	blt	$00007DCC

l00007DBA:
	cmpi.b	#$09,d0
	bgt	$00007DCC

l00007DC0:
	move.l	d1,d2
	asl.l	#$02,d1
	add.l	d2,d1
	add.l	d1,d1
	add.l	d0,d1
	bra	$00007DB2

l00007DCC:
	cmpi.b	#$2D,(a1)
	bne	$00007DD4

l00007DD2:
	neg.l	d1

l00007DD4:
	move.l	(a7)+,d2
	move.l	a0,d0
	subq.l	#$01,d0
	movea.l	$0008(a7),a0
	move.l	d1,(a0)
	sub.l	a1,d0
	rts	
00007DE4             4E 55 00 00 48 E7 00 30 26 6D 00 08     NU..H..0&m..
00007DF0 24 6D 00 0C 42 AC 47 80 29 4B 47 84 48 6D 00 10 $m..B.G.)KG.Hm..
00007E00 2F 0A 48 6C 47 80 48 7A 00 34 48 7A 00 10 4E BA /.HlG.Hz.4Hz..N.
00007E10 E4 98 4C ED 0C 00 FF F8 4E 5D 4E 75 2F 07 52 AC ..L.....N]Nu/.R.
00007E20 47 80 7E 00 20 6C 47 84 1E 18 29 48 47 84 4A 87 G.~. lG...)HG.J.
00007E30 67 04 20 07 60 02 70 FF 2E 1F 4E 75 4E 55 00 00 g. .`.p...NuNU..
00007E40 53 AC 47 80 53 AC 47 84 4E 5D 4E 75             S.G.S.G.N]Nu   

fn00007E4C()
	link	a5,#$FFE8
	movem.l	d7/a2-a3/a6,-(a7)
	move.l	$000C(a5),d7
	tst.l	d7
	bgt	$00007E62

l00007E5C:
	moveq	#$-01,d0
	bra	$00007F32

l00007E62:
	moveq	#$+08,d0
	cmp.l	d0,d7
	bcc	$00007E6A

l00007E68:
	move.l	d0,d7

l00007E6A:
	move.l	d7,d0
	addq.l	#$03,d0
	move.l	d0,d7
	andi.w	#$FFFC,d7
	movea.l	$0008(a5),a2
	move.l	$0008(a5),d0
	add.l	d7,d0
	add.l	d7,$1DEC(a4)
	lea	$1DE8(a4),a0
	movea.l	(a0),a3
	move.l	d0,$-0010(a5)
	move.l	a0,$-000C(a5)

l00007E90:
	move.l	a3,d0
	beq	$00007F24

l00007E96:
	movea.l	a3,a0
	move.l	$0004(a3),d0
	adda.l	d0,a0
	movem.l	d7,$-0014(a5)
	movea.l	$-0010(a5),a1
	cmpa.l	a1,a3
	bls	$00007EBC

l00007EAC:
	move.l	a3,(a2)
	move.l	d7,$0004(a2)
	movea.l	$-000C(a5),a6
	move.l	a2,(a6)
	moveq	#$+00,d0
	bra	$00007F32

l00007EBC:
	cmpa.l	a1,a3
	bne	$00007EDA

l00007EC0:
	movea.l	(a3),a6
	move.l	a6,(a2)
	move.l	$0004(a3),d0
	move.l	d0,d1
	add.l	d7,d1
	move.l	d1,$0004(a2)
	movea.l	$-000C(a5),a6
	move.l	a2,(a6)
	moveq	#$+00,d0
	bra	$00007F32

l00007EDA:
	cmpa.l	a0,a2
	bcc	$00007EE6

l00007EDE:
	sub.l	d7,$1DEC(a4)
	moveq	#$-01,d0
	bra	$00007F32

l00007EE6:
	cmpa.l	a0,a2
	bne	$00007F14

l00007EEA:
	move.l	(a3),d0
	beq	$00007EFA

l00007EEE:
	cmpa.l	d0,a1
	bls	$00007EFA

l00007EF2:
	sub.l	d7,$1DEC(a4)
	moveq	#$-01,d0
	bra	$00007F32

l00007EFA:
	add.l	d7,$0004(a3)
	move.l	(a3),d0
	beq	$00007F10

l00007F02:
	cmpa.l	d0,a1
	bne	$00007F10

l00007F06:
	move.l	$0004(a1),d0
	add.l	d0,$0004(a3)
	move.l	(a1),(a3)

l00007F10:
	moveq	#$+00,d0
	bra	$00007F32

l00007F14:
	move.l	a3,$-000C(a5)
	move.l	$-0014(a5),$-0018(a5)
	movea.l	(a3),a3
	bra	$00007E90

l00007F24:
	movea.l	$-000C(a5),a0
	move.l	a2,(a0)
	clr.l	(a2)
	move.l	d7,$0004(a2)
	moveq	#$+00,d0

l00007F32:
	movem.l	(a7)+,d7/a2-a3/a6
	unlk	a5
	rts	
00007F3A                               00 00                       ..   

fn00007F3C()
	movem.l	d2/a2-a3/a6,-(a7)
	movea.l	$0014(a7),a3
	movea.l	$0018(a7),a2
	move.l	a3,d1
	move.l	a2,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-004E(a6)
	tst.l	d0
	bne	$00007F6A

l00007F58:
	jsr.l	$-0084(a6)
	move.l	d0,$2030(a4)
	moveq	#$+05,d0
	move.l	d0,$47A0(a4)
	moveq	#$-01,d0
	bra	$00007F6C

l00007F6A:
	moveq	#$+00,d0

l00007F6C:
	movem.l	(a7)+,d2/a2-a3/a6
	rts	
00007F72       00 00 00 00 00 00 00 00 00 00 00 00 00 00   ..............

fn00007F80()
	movem.l	a3/a6,-(a7)
	movea.l	$000C(a7),a3
	move.l	a3,d1
	movea.l	$47A4(a4),a6
	jsr.l	$-0048(a6)
	tst.l	d0
	bne	$00007FA8

l00007F96:
	jsr.l	$-0084(a6)
	move.l	d0,$2030(a4)
	moveq	#$+02,d0
	move.l	d0,$47A0(a4)
	moveq	#$-01,d0
	bra	$00007FAA

l00007FA8:
	moveq	#$+00,d0

l00007FAA:
	movem.l	(a7)+,a3/a6
	rts	

fn00007FB0()
	movem.l	d5-d7/a2-a3,-(a7)
	move.l	$0018(a7),d7
	movea.l	$001C(a7),a3
	move.l	$0020(a7),d6
	move.l	d7,-(a7)
	jsr.l	$1432(pc)
	addq.w	#$04,a7
	movea.l	d0,a2
	move.l	a2,d0
	bne	$00007FD2

l00007FCE:
	moveq	#$-01,d0
	bra	$00007FF0

l00007FD2:
	move.l	d6,-(a7)
	move.l	a3,-(a7)
	move.l	$0004(a2),-(a7)
	jsr.l	$0B9A(pc)
	lea	$000C(a7),a7
	move.l	d0,d5
	tst.l	$2030(a4)
	beq	$00007FEE

l00007FEA:
	moveq	#$-01,d0
	bra	$00007FF0

l00007FEE:
	move.l	d5,d0

l00007FF0:
	movem.l	(a7)+,d5-d7/a2-a3
	rts	
00007FF6                   00 00 4E 55 FF F0 48 E7 0F 30       ..NU..H..0
00008000 2E 2D 00 0C 2C 2D 00 10 70 01 BE 80 6F 00 00 D6 .-..,-..p...o...
00008010 20 6D 00 08 22 48 D3 C6 26 49 70 02 BE 80 66 26  m.."H..&Ip...f&
00008020 2F 0B 2F 08 22 6D 00 14 4E 91 50 4F 4A 80 6F 00 /./."m..N.POJ.o.
00008030 00 B4 2F 06 2F 0B 2F 2D 00 08 4E BA F8 7C 4F EF ../././-..N..|O.
00008040 00 0C 60 00 00 A0 20 07 4A 80 6A 02 52 80 E2 80 ..`... .J.j.R...
00008050 22 06 4E BA 11 00 20 6D 00 08 D1 C0 2F 06 2F 08 ".N... m...././.
00008060 2F 2D 00 08 4E BA F8 52 4F EF 00 0C 24 6D 00 08 /-..N..RO...$m..
00008070 78 00 7A 01 BA 87 6C 2E 2F 2D 00 08 2F 0B 20 6D x.z...l./-../. m
00008080 00 14 4E 90 50 4F 4A 80 6A 16 52 84 D5 C6 B7 CA ..N.POJ.j.R.....
00008090 67 0E 2F 06 2F 0A 2F 0B 4E BA F8 1E 4F EF 00 0C g./././.N...O...
000080A0 D7 C6 52 85 60 CE B5 ED 00 08 67 10 2F 06 2F 0A ..R.`.....g././.
000080B0 2F 2D 00 08 4E BA F8 02 4F EF 00 0C 2F 2D 00 14 /-..N...O.../-..
000080C0 2F 06 2F 04 2F 2D 00 08 61 00 FF 2E 20 4A D1 C6 /././-..a... J..
000080D0 20 07 90 84 53 80 2E AD 00 14 2F 06 2F 00 2F 08  ...S...../././.
000080E0 61 00 FF 16 4C ED 0C F0 FF D8 4E 5D 4E 75 00 00 a...L.....N]Nu..
000080F0 2F 07 2E 2F 00 08 52 AC 47 88 53 AC 1E 22 6D 12 /../..R.G.S.."m.
00008100 20 6C 1E 1A 52 AC 1E 1A 20 07 10 80 72 00 12 00  l..R... ...r...
00008110 60 14 20 07 72 00 12 00 48 6C 1E 16 2F 01 4E BA `. .r...Hl../.N.
00008120 F1 6C 50 4F 22 00 2E 1F 4E 75                   .lPO"...Nu     

fn0000812A()
	link	a5,#$0000
	move.l	a3,-(a7)
	movea.l	$0008(a5),a3
	clr.l	$4788(a4)
	pea	$000C(a5)
	move.l	a3,-(a7)
	pea	$-004E(pc)
	jsr.l	$-1214(pc)
	pea	$1E16(a4)
	pea	$0000FFFF
	jsr.l	$-0EC2(pc)
	move.l	$4788(a4),d0
	movea.l	$-0004(a5),a3
	unlk	a5
	rts	
0000815E                                           00 00               ..

fn00008160()
	link	a5,#$FFE4
	movem.l	d4-d7/a2-a3,-(a7)
	movea.l	$0008(a5),a3
	move.l	$000C(a5),d7
	clr.b	$-0001(a5)
	clr.l	$2030(a4)
	move.l	$47A0(a4),$-000E(a5)
	moveq	#$+03,d5

l00008180:
	cmp.l	$1DD8(a4),d5
	bge	$00008198

l00008186:
	move.l	d5,d0
	asl.l	#$03,d0
	lea	$4560(a4),a0
	tst.l	(a0,d0)
	beq	$00008198

l00008194:
	addq.l	#$01,d5
	bra	$00008180

l00008198:
	move.l	$1DD8(a4),d0
	cmp.l	d5,d0
	bne	$000081AC

l000081A0:
	moveq	#$+18,d0
	move.l	d0,$47A0(a4)
	moveq	#$-01,d0
	bra	$000082D0

l000081AC:
	move.l	d5,d0
	asl.l	#$03,d0
	lea	$4560(a4),a0
	adda.l	d0,a0
	movea.l	a0,a2
	move.l	$0010(a5),d0
	beq	$000081C4

l000081BE:
	btst	#$0002,d0
	beq	$000081CE

l000081C4:
	move.l	#$000003EC,$-0012(a5)
	bra	$000081D6

l000081CE:
	move.l	#$000003EE,$-0012(a5)

l000081D6:
	move.l	#$00008000,d0
	and.l	$1DF0(a4),d0
	eor.l	d0,d7
	btst	#$0003,d7
	beq	$000081F4

l000081E8:
	move.l	d7,d0
	andi.w	#$FFFC,d0
	move.l	d0,d7
	ori.w	#$0002,d7

l000081F4:
	move.l	d7,d0
	moveq	#$+03,d1
	and.l	d1,d0
	tst.l	d0
	beq	$00008206

l000081FE:
	subq.l	#$01,d0
	beq	$00008206

l00008202:
	subq.l	#$01,d0
	bne	$0000820C

l00008206:
	move.l	d7,d6
	addq.l	#$01,d6
	bra	$00008218

l0000820C:
	moveq	#$+16,d0
	move.l	d0,$47A0(a4)
	moveq	#$-01,d0
	bra	$000082D0

l00008218:
	move.l	d7,d0
	andi.l	#$00000300,d0
	beq	$000082AA

l00008224:
	btst	#$000A,d7
	beq	$00008240

l0000822A:
	move.b	#$01,$-0001(a5)
	move.l	$-0012(a5),-(a7)
	move.l	a3,-(a7)
	jsr.l	$0D82(pc)
	addq.w	#$08,a7
	move.l	d0,d4
	bra	$0000827C

l00008240:
	btst	#$0009,d7
	bne	$0000825C

l00008246:
	pea	$000003ED
	move.l	a3,-(a7)
	jsr.l	$0974(pc)
	addq.w	#$08,a7
	move.l	d0,d4
	tst.l	d4
	bpl	$0000825C

l00008258:
	bset	#$0009,d7

l0000825C:
	btst	#$0009,d7
	beq	$0000827C

l00008262:
	move.b	#$01,$-0001(a5)
	move.l	$-000E(a5),$47A0(a4)
	move.l	$-0012(a5),-(a7)
	move.l	a3,-(a7)
	jsr.l	$0DA8(pc)
	addq.w	#$08,a7
	move.l	d0,d4

l0000827C:
	tst.b	$-0001(a5)
	beq	$000082B8

l00008282:
	move.l	d7,d0
	moveq	#$+78,d1
	add.l	d1,d1
	and.l	d1,d0
	tst.l	d0
	beq	$000082B8

l0000828E:
	tst.l	d4
	bmi	$000082B8

l00008292:
	move.l	d4,-(a7)
	jsr.l	$0DF0(pc)
	pea	$000003ED
	move.l	a3,-(a7)
	jsr.l	$0922(pc)
	lea	$000C(a7),a7
	move.l	d0,d4
	bra	$000082B8

l000082AA:
	pea	$000003ED
	move.l	a3,-(a7)
	jsr.l	$0910(pc)
	addq.w	#$08,a7
	move.l	d0,d4

l000082B8:
	tst.l	$2030(a4)
	bne	$000082C4

l000082BE:
	moveq	#$-01,d0
	cmp.l	d0,d4
	bne	$000082C8

l000082C4:
	moveq	#$-01,d0
	bra	$000082D0

l000082C8:
	move.l	d6,(a2)
	move.l	d4,$0004(a2)
	move.l	d5,d0

l000082D0:
	movem.l	(a7)+,d4-d7/a2-a3
	unlk	a5
	rts	
000082D8                         00 00 00 00 00 00 00 00         ........
000082E0 00 00 00 00                                     ....           

fn000082E4()
	movea.l	$0004(a7),a0
	movea.l	$0008(a7),a1
	move.l	$000C(a7),d0
	ble	$00008308

l000082F2:
	cmpa.l	a0,a1
	bcs	$00008302

l000082F6:
	adda.l	d0,a0
	adda.l	d0,a1

l000082FA:
	move.b	-(a0),-(a1)
	subq.l	#$01,d0
	bne	$000082FA

l00008300:
	rts	

l00008302:
	move.b	(a0)+,(a1)+
	subq.l	#$01,d0
	bne	$00008302

l00008308:
	rts	
0000830A                               00 00                       ..   

fn0000830C()
	movem.l	a2-a3/a6,-(a7)
	movea.l	$4790(a4),a3

l00008314:
	move.l	a3,d0
	beq	$0000832C

l00008318:
	movea.l	(a3),a2
	movea.l	a3,a1
	move.l	$0008(a1),d0
	movea.l	$00000004,a6
	jsr.l	$-00D2(a6)
	movea.l	a2,a3
	bra	$00008314

l0000832C:
	suba.l	a0,a0
	move.l	a0,$4794(a4)
	move.l	a0,$4790(a4)
	movem.l	(a7)+,a2-a3/a6
	rts	

fn0000833C()
	movem.l	d7/a2-a3,-(a7)
	move.l	$0010(a7),d7
	move.l	$479C(a4),d0
	beq	$0000835C

l0000834A:
	movea.l	d0,a2
	move.l	(a2),-(a7)
	move.l	d0,-(a7)
	jsr.l	$-0504(pc)
	addq.w	#$08,a7
	suba.l	a0,a0
	move.l	a0,$479C(a4)

l0000835C:
	move.l	d7,d0
	bne	$00008364

l00008360:
	moveq	#$+00,d0
	bra	$00008382

l00008364:
	addq.l	#$04,d7
	move.l	d7,-(a7)
	jsr.l	$00D4(pc)
	addq.w	#$04,a7
	movea.l	d0,a3
	tst.l	d0
	bne	$00008378

l00008374:
	moveq	#$+00,d0
	bra	$00008382

l00008378:
	movea.l	a3,a2
	move.l	d7,(a2)
	lea	$0004(a3),a0
	move.l	a0,d0

l00008382:
	movem.l	(a7)+,d7/a2-a3
	rts	

fn00008388()
	movem.l	d4-d7/a3,-(a7)
	move.l	$0018(a7),d7
	move.l	$001C(a7),d6
	move.l	$0020(a7),d5
	move.l	d7,-(a7)
	jsr.l	$105A(pc)
	addq.w	#$04,a7
	movea.l	d0,a3
	move.l	a3,d0
	bne	$000083AA

l000083A6:
	moveq	#$-01,d0
	bra	$000083C8

l000083AA:
	move.l	d5,-(a7)
	move.l	d6,-(a7)
	move.l	$0004(a3),-(a7)
	jsr.l	$0746(pc)
	lea	$000C(a7),a7
	move.l	d0,d4
	tst.l	$2030(a4)
	beq	$000083C6

l000083C2:
	moveq	#$-01,d0
	bra	$000083C8

l000083C6:
	move.l	d4,d0

l000083C8:
	movem.l	(a7)+,d4-d7/a3
	rts	
000083CE                                           00 00               ..

fn000083D0()
	movem.l	d7/a2-a3/a6,-(a7)
	move.l	$0014(a7),d7
	moveq	#$+0C,d0
	add.l	d0,d7
	move.l	d7,d0
	moveq	#$+00,d1
	movea.l	$00000004,a6
	jsr.l	$-00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	bne	$000083F2

l000083EE:
	moveq	#$+00,d0
	bra	$0000842A

l000083F2:
	move.l	d7,$0008(a3)
	lea	$4790(a4),a2
	movea.l	$0004(a2),a0
	move.l	a0,$0004(a3)
	suba.l	a0,a0
	move.l	a0,(a3)
	tst.l	(a2)
	bne	$0000840C

l0000840A:
	move.l	a3,(a2)

l0000840C:
	move.l	$0004(a2),d0
	beq	$00008416

l00008412:
	movea.l	d0,a1
	move.l	a3,(a1)

l00008416:
	move.l	a3,$0004(a2)
	tst.l	$1DDC(a4)
	bne	$00008424

l00008420:
	move.l	a3,$1DDC(a4)

l00008424:
	lea	$000C(a3),a0
	move.l	a0,d0

l0000842A:
	movem.l	(a7)+,d7/a2-a3/a6
	rts	
00008430 00 00 00 00 00 00 00 00 00 00 00 00             ............   

fn0000843C()
	movem.l	d6-d7/a2-a3,-(a7)
	move.l	$0014(a7),d7
	tst.l	d7
	bgt	$0000844E

l00008448:
	moveq	#$+00,d0
	bra	$000084F0

l0000844E:
	moveq	#$+08,d0
	cmp.l	d0,d7
	bcc	$00008456

l00008454:
	move.l	d0,d7

l00008456:
	move.l	d7,d0
	addq.l	#$03,d0
	move.l	d0,d7
	andi.w	#$FFFC,d7
	lea	$1DE8(a4),a2
	movea.l	(a2),a3

l00008466:
	move.l	a3,d0
	beq	$000084AA

l0000846A:
	move.l	$0004(a3),d0
	cmp.l	d7,d0
	blt	$000084A4

l00008472:
	cmp.l	d7,d0
	bne	$00008482

l00008476:
	movea.l	(a3),a0
	move.l	a0,(a2)
	sub.l	d7,$1DEC(a4)
	move.l	a3,d0
	bra	$000084F0

l00008482:
	move.l	$0004(a3),d0
	sub.l	d7,d0
	moveq	#$+08,d1
	cmp.l	d1,d0
	bcs	$000084A4

l0000848E:
	movea.l	a3,a0
	adda.l	d7,a0
	move.l	a0,(a2)
	movea.l	a0,a2
	move.l	(a3),(a2)
	move.l	d0,$0004(a2)
	sub.l	d7,$1DEC(a4)
	move.l	a3,d0
	bra	$000084F0

l000084A4:
	movea.l	a3,a2
	movea.l	(a3),a3
	bra	$00008466

l000084AA:
	move.l	d7,d0
	move.l	$1E6C(a4),d1
	add.l	d1,d0
	subq.l	#$01,d0
	jsr.l	$0CC0(pc)
	move.l	$1E6C(a4),d1
	jsr.l	$0C98(pc)
	move.l	d0,d6
	addq.l	#$08,d6
	move.l	d6,d0
	addq.l	#$03,d0
	move.l	d0,d6
	andi.w	#$FFFC,d6
	move.l	d6,-(a7)
	jsr.l	$-0100(pc)
	addq.w	#$04,a7
	movea.l	d0,a3
	move.l	a3,d0
	beq	$000084EE

l000084DC:
	move.l	d6,-(a7)
	move.l	a3,-(a7)
	jsr.l	$-0694(pc)
	move.l	d7,(a7)
	bsr	$0000843C
	addq.w	#$08,a7
	bra	$000084F0

l000084EE:
	moveq	#$+00,d0

l000084F0:
	movem.l	(a7)+,d6-d7/a2-a3
	rts	
000084F6                   00 00 00 00 00 00 00 00 00 00       ..........
00008500 00 00 00 00                                     ....           

fn00008504()
	move.l	d7,-(a7)
	move.l	$0008(a7),d7
	move.l	d7,-(a7)
	jsr.l	$-00D0(pc)
	addq.w	#$04,a7
	move.l	(a7)+,d7
	rts	
00008516                   00 00                               ..       

fn00008518()
	link	a5,#$FFE0
	movem.l	d2/d4-d7/a3/a6,-(a7)
	movea.l	$0008(a5),a3
	lea	$-000C(a5),a0
	move.l	a0,d1
	movea.l	$47A4(a4),a6
	jsr.l	$-00C0(a6)
	move.l	$-000C(a5),d0
	move.l	#$000007BA,d7
	move.l	d0,d6
	move.l	d0,$-0010(a5)

l00008542:
	cmpi.l	#$0000016D,d6
	ble	$00008568

l0000854A:
	move.l	d7,d0
	moveq	#$+04,d1
	jsr.l	$0C26(pc)
	tst.l	d1
	bne	$0000855E

l00008556:
	subi.l	#$0000016E,d6
	bra	$00008564

l0000855E:
	subi.l	#$0000016D,d6

l00008564:
	addq.l	#$01,d7
	bra	$00008542

l00008568:
	cmpi.l	#$0000016D,d6
	bne	$00008580

l00008570:
	move.l	d7,d0
	moveq	#$+04,d1
	jsr.l	$0C00(pc)
	tst.l	d1
	beq	$00008580

l0000857C:
	addq.l	#$01,d7
	moveq	#$+00,d6

l00008580:
	move.l	d6,d0
	move.l	d7,d1
	subi.l	#$000007BC,d1
	move.b	d1,$0001(a3)
	moveq	#$+00,d2
	move.b	d1,d2
	move.l	d0,$-0010(a5)
	move.l	d2,d0
	moveq	#$+04,d1
	jsr.l	$0BDA(pc)
	tst.l	d1
	bne	$000085A6

l000085A2:
	moveq	#$+1D,d0
	bra	$000085A8

l000085A6:
	moveq	#$+1C,d0

l000085A8:
	move.b	d0,$1E71(a4)
	moveq	#$+00,d4
	move.l	$-0010(a5),d5

l000085B2:
	moveq	#$+0C,d0
	cmp.l	d0,d4
	bge	$000085D6

l000085B8:
	moveq	#$+00,d0
	lea	$1E70(a4),a0
	move.b	(a0,d4),d0
	cmp.l	d5,d0
	bgt	$000085D6

l000085C6:
	moveq	#$+00,d0
	lea	$1E70(a4),a0
	move.b	(a0,d4),d0
	sub.l	d0,d5
	addq.l	#$01,d4
	bra	$000085B2

l000085D6:
	move.l	d5,d0
	move.l	d4,d1
	addq.l	#$01,d1
	move.b	d1,$0002(a3)
	move.l	d0,$-0010(a5)
	addq.l	#$01,d0
	move.b	d0,$0003(a3)
	move.l	$-000C(a5),d0
	moveq	#$+07,d1
	jsr.l	$0B84(pc)
	move.b	d1,(a3)
	move.l	$-0008(a5),d0
	moveq	#$+3C,d1
	jsr.l	$0B78(pc)
	move.b	d0,$0004(a3)
	move.l	$-0008(a5),d0
	moveq	#$+3C,d1
	jsr.l	$0B6A(pc)
	move.b	d1,$0005(a3)
	move.l	$-0004(a5),d0
	moveq	#$+32,d1
	jsr.l	$0B5C(pc)
	move.b	d0,$0006(a3)
	move.l	$-0004(a5),d0
	moveq	#$+32,d1
	jsr.l	$0B4E(pc)
	add.l	d1,d1
	move.b	d1,$0007(a3)
	movem.l	(a7)+,d2/d4-d7/a3/a6
	unlk	a5
	rts	
00008638                         4E 55 FF F4 48 E7 0F 30         NU..H..0
00008640 26 6D 00 08 2E 2D 00 10 24 6D 00 14 7C 00 BC 87 &m...-..$m..|...
00008650 6C 46 2A 2D 00 0C 4A 85 6F 3A 78 00 18 1B 53 AA lF*-..J.o:x...S.
00008660 00 0C 6D 12 20 6A 00 04 52 AA 00 04 20 04 10 80 ..m. j..R... ...
00008670 72 00 12 00 60 12 20 04 72 00 12 00 2F 0A 2F 01 r...`. .r..././.
00008680 4E BA EC 0A 50 4F 22 00 52 81 66 04 20 06 60 0A N...PO".R.f. .`.
00008690 53 85 60 C2 52 86 60 B6 20 06 4C DF 0C F0 4E 5D S.`.R.`. .L...N]
000086A0 4E 75 00 00                                     Nu..           

fn000086A4()
	movem.l	d7/a3,-(a7)
	movea.l	$000C(a7),a3
	btst	#$0001,$001B(a3)
	beq	$000086C6

l000086B4:
	move.l	$0014(a3),d0
	beq	$00008716

l000086BA:
	move.l	a3,-(a7)
	pea	$0000FFFF
	jsr.l	$-1434(pc)
	addq.w	#$08,a7

l000086C6:
	pea	$00000001
	clr.l	-(a7)
	move.l	$001C(a3),-(a7)
	jsr.l	$-0348(pc)
	lea	$000C(a7),a7
	move.l	d0,d7
	moveq	#$-01,d0
	cmp.l	d0,d7
	beq	$000086E6

l000086E0:
	tst.l	$0014(a3)
	bne	$000086EA

l000086E6:
	move.l	d7,d0
	bra	$00008716

l000086EA:
	btst	#$0001,$001B(a3)
	beq	$000086FE

l000086F2:
	move.l	$0004(a3),d0
	sub.l	$0010(a3),d0
	add.l	d7,d0
	bra	$00008716

l000086FE:
	btst	#$0007,$001A(a3)
	beq	$00008710

l00008706:
	move.l	d7,d0
	move.l	$0008(a3),d1
	add.l	d1,d0
	bra	$00008716

l00008710:
	move.l	d7,d0
	sub.l	$0008(a3),d0

l00008716:
	movem.l	(a7)+,d7/a3
	rts	

fn0000871C()
	movem.l	d6-d7/a3,-(a7)
	movea.l	$0010(a7),a3
	move.l	$0014(a7),d7
	move.l	$0018(a7),d6
	btst	#$0001,$001B(a3)
	beq	$00008742

l00008734:
	move.l	a3,-(a7)
	pea	$0000FFFF
	jsr.l	$-14AE(pc)
	addq.w	#$08,a7
	bra	$0000875C

l00008742:
	moveq	#$+01,d0
	cmp.l	d0,d6
	bne	$0000875C

l00008748:
	btst	#$0007,$001A(a3)
	beq	$00008758

l00008750:
	move.l	$0008(a3),d0
	add.l	d0,d7
	bra	$0000875C

l00008758:
	sub.l	$0008(a3),d7

l0000875C:
	move.l	$0010(a3),$0004(a3)
	moveq	#$+00,d0
	move.l	d0,$000C(a3)
	move.l	d0,$0008(a3)
	btst	#$0007,$001B(a3)
	beq	$0000877A

l00008774:
	moveq	#$-04,d0
	and.l	d0,$0018(a3)

l0000877A:
	move.l	d6,-(a7)
	move.l	d7,-(a7)
	move.l	$001C(a3),-(a7)
	jsr.l	$-03FA(pc)
	lea	$000C(a7),a7
	addq.l	#$01,d0
	bne	$00008792

l0000878E:
	moveq	#$-01,d0
	bra	$0000879A

l00008792:
	moveq	#$-31,d0
	and.l	d0,$0018(a3)
	moveq	#$+00,d0

l0000879A:
	movem.l	(a7)+,d6-d7/a3
	rts	

fn000087A0()
	move.l	a3,-(a7)
	movea.l	$0008(a7),a3
	move.l	a3,d0
	beq	$000087BA

l000087AA:
	clr.l	-(a7)
	jsr.l	$-0470(pc)
	addq.w	#$04,a7
	movea.l	a3,a0
	subq.l	#$04,a0
	move.l	a0,$479C(a4)

l000087BA:
	movea.l	(a7)+,a3
	rts	
000087BE                                           00 00               ..
000087C0 00 00 00 00 00 00 70 61 4E 55 FF F4 48 E7 0F 30 ......paNU..H..0
000087D0 26 6D 00 08 2E 2D 00 10 24 6D 00 14 20 2D 00 0C &m...-..$m.. -..
000087E0 67 44 7C 00 BC 87 6C 3C 2A 2D 00 0C 4A 85 6F 30 gD|...l<*-..J.o0
000087F0 53 AA 00 08 6D 0E 20 6A 00 04 52 AA 00 04 70 00 S...m. j..R...p.
00008800 10 10 60 08 2F 0A 4E BA ED 1C 58 4F 28 00 70 FF ..`./.N...XO(.p.
00008810 B8 80 66 04 20 06 60 0E 20 04 16 C0 53 85 60 CC ..f. .`. ...S.`.
00008820 52 86 60 C0 20 06 4C DF 0C F0 4E 5D 4E 75 00 00 R.`. .L...N]Nu..

fn00008830()
	link	a5,#$FFF8
	movem.l	a2-a3,-(a7)
	lea	$1DF4(a4),a3

l0000883C:
	move.l	a3,d0
	beq	$0000884C

l00008840:
	tst.l	$0018(a3)
	beq	$0000884C

l00008846:
	movea.l	a3,a2
	movea.l	(a3),a3
	bra	$0000883C

l0000884C:
	move.l	a3,d0
	bne	$00008872

l00008850:
	pea	$00000022
	jsr.l	$-0418(pc)
	addq.w	#$04,a7
	movea.l	d0,a3
	tst.l	d0
	bne	$00008864

l00008860:
	moveq	#$+00,d0
	bra	$00008880

l00008864:
	move.l	a3,(a2)
	moveq	#$+21,d0
	moveq	#$+00,d1
	movea.l	a3,a0

l0000886C:
	move.b	d1,(a0)+
	dbra	d0,$0000886C

l00008872:
	move.l	a3,-(a7)
	move.l	$000C(a5),-(a7)
	move.l	$0008(a5),-(a7)
	jsr.l	$0010(pc)

l00008880:
	movem.l	$-0010(a5),a2-a3
	unlk	a5
	rts	
0000888A                               00 00                       ..   

fn0000888C()
	link	a5,#$FFF0
	movem.l	d4-d7/a2-a3,-(a7)
	movea.l	$000C(a5),a3
	movea.l	$0010(a5),a2
	tst.l	$0018(a2)
	beq	$000088AA

l000088A2:
	move.l	a2,-(a7)
	jsr.l	$0154(pc)
	addq.w	#$04,a7

l000088AA:
	move.l	$1E5C(a4),d5
	moveq	#$+01,d7
	moveq	#$+00,d0
	move.b	(a3,d7),d0
	subi.w	#$0061,d0
	beq	$000088C2

l000088BC:
	subq.w	#$01,d0
	beq	$000088C6

l000088C0:
	bra	$000088CE

l000088C2:
	moveq	#$+00,d5
	bra	$000088CC

l000088C6:
	move.l	#$00008000,d5

l000088CC:
	addq.l	#$01,d7

l000088CE:
	moveq	#$+2B,d1
	cmp.b	(a3,d7),d1
	seq	d0
	neg.b	d0
	ext.w	d0
	ext.l	d0
	move.l	d0,d4
	moveq	#$+00,d0
	move.b	(a3),d0
	subi.w	#$0061,d0
	beq	$000088F6

l000088E8:
	subi.w	#$0011,d0
	beq	$00008930

l000088EE:
	subq.w	#$05,d0
	beq	$0000896E

l000088F2:
	bra	$000089B2

l000088F6:
	pea	$0000000C
	move.l	#$00008102,-(a7)
	move.l	$0008(a5),-(a7)
	jsr.l	$-07A4(pc)
	lea	$000C(a7),a7
	move.l	d0,d6
	moveq	#$-01,d0
	cmp.l	d0,d6
	bne	$0000891A

l00008914:
	moveq	#$+00,d0
	bra	$000089E8

l0000891A:
	tst.l	d4
	beq	$00008924

l0000891E:
	moveq	#$+40,d0
	add.l	d0,d0
	bra	$00008926

l00008924:
	moveq	#$+02,d0

l00008926:
	move.l	d0,d7
	ori.w	#$4000,d7
	bra	$000089B6

l00008930:
	tst.l	d4
	beq	$00008938

l00008934:
	moveq	#$+02,d0
	bra	$0000893A

l00008938:
	moveq	#$+00,d0

l0000893A:
	ori.w	#$8000,d0
	pea	$0000000C
	move.l	d0,-(a7)
	move.l	$0008(a5),-(a7)
	jsr.l	$-07E8(pc)
	lea	$000C(a7),a7
	move.l	d0,d6
	moveq	#$-01,d0
	cmp.l	d0,d6
	bne	$0000895E

l00008958:
	moveq	#$+00,d0
	bra	$000089E8

l0000895E:
	tst.l	d4
	beq	$00008968

l00008962:
	moveq	#$+40,d0
	add.l	d0,d0
	bra	$0000896A

l00008968:
	moveq	#$+01,d0

l0000896A:
	move.l	d0,d7
	bra	$000089B6

l0000896E:
	tst.l	d4
	beq	$00008976

l00008972:
	moveq	#$+02,d0
	bra	$00008978

l00008976:
	moveq	#$+01,d0

l00008978:
	ori.w	#$8000,d0
	ori.w	#$0100,d0
	ori.w	#$0200,d0
	pea	$0000000C
	move.l	d0,-(a7)
	move.l	$0008(a5),-(a7)
	jsr.l	$-082E(pc)
	lea	$000C(a7),a7
	move.l	d0,d6
	moveq	#$-01,d0
	cmp.l	d0,d6
	bne	$000089A2

l0000899E:
	moveq	#$+00,d0
	bra	$000089E8

l000089A2:
	tst.l	d4
	beq	$000089AC

l000089A6:
	moveq	#$+40,d0
	add.l	d0,d0
	bra	$000089AE

l000089AC:
	moveq	#$+02,d0

l000089AE:
	move.l	d0,d7
	bra	$000089B6

l000089B2:
	moveq	#$+00,d0
	bra	$000089E8

l000089B6:
	suba.l	a0,a0
	move.l	a0,$0010(a2)
	moveq	#$+00,d0
	move.l	d0,$0014(a2)
	move.l	d6,$001C(a2)
	move.l	$0010(a2),$0004(a2)
	move.l	d0,$000C(a2)
	move.l	d0,$0008(a2)
	tst.l	d5
	bne	$000089DE

l000089D8:
	move.l	#$00008000,d0

l000089DE:
	move.l	d7,d1
	or.l	d0,d1
	move.l	d1,$0018(a2)
	move.l	a2,d0

l000089E8:
	movem.l	(a7)+,d4-d7/a2-a3
	unlk	a5
	rts	
000089F0 00 00 00 00 00 00 70 61                         ......pa       

fn000089F8()
	movem.l	d6-d7/a3,-(a7)
	movea.l	$0010(a7),a3
	btst	#$0001,$001B(a3)
	beq	$00008A18

l00008A08:
	move.l	a3,-(a7)
	pea	$0000FFFF
	jsr.l	$-1782(pc)
	addq.w	#$08,a7
	move.l	d0,d7
	bra	$00008A1A

l00008A18:
	moveq	#$+00,d7

l00008A1A:
	moveq	#$+0C,d0
	and.l	$0018(a3),d0
	bne	$00008A36

l00008A22:
	tst.l	$0014(a3)
	beq	$00008A36

l00008A28:
	move.l	$0014(a3),-(a7)
	move.l	$0010(a3),-(a7)
	jsr.l	$-0BE4(pc)
	addq.w	#$08,a7

l00008A36:
	clr.l	$0018(a3)
	move.l	$001C(a3),-(a7)
	jsr.l	$092E(pc)
	addq.w	#$04,a7
	move.l	d0,d6
	moveq	#$-01,d0
	cmp.l	d0,d7
	beq	$00008A52

l00008A4C:
	tst.l	d6
	bne	$00008A52

l00008A50:
	moveq	#$+00,d0

l00008A52:
	movem.l	(a7)+,d6-d7/a3
	rts	

fn00008A58()
	movem.l	d6-d7/a3,-(a7)
	move.l	$0010(a7),d7
	lea	$1DF4(a4),a3

l00008A64:
	move.l	a3,d0
	beq	$00008A9C

l00008A68:
	btst	#$0002,$001B(a3)
	bne	$00008A98

l00008A70:
	btst	#$0001,$001B(a3)
	beq	$00008A98

l00008A78:
	move.l	$0004(a3),d0
	sub.l	$0010(a3),d0
	move.l	d0,d6
	tst.l	d6
	beq	$00008A98

l00008A86:
	move.l	d6,-(a7)
	move.l	$0010(a3),-(a7)
	move.l	$001C(a3),-(a7)
	jsr.l	$-29E0(pc)
	lea	$000C(a7),a7

l00008A98:
	movea.l	(a3),a3
	bra	$00008A64

l00008A9C:
	move.l	d7,-(a7)
	jsr.l	$-1406(pc)
	addq.w	#$04,a7
	movem.l	(a7)+,d6-d7/a3
	rts	
00008AAA                               00 00                       ..   

fn00008AAC()
	movem.l	d2-d3/d5-d7/a3/a6,-(a7)
	move.l	$0020(a7),d7
	movea.l	$0024(a7),a3
	move.l	$0028(a7),d6
	tst.l	$2048(a4)
	beq	$00008AC6

l00008AC2:
	jsr.l	$097A(pc)

l00008AC6:
	clr.l	$2030(a4)
	move.l	d7,d1
	move.l	a3,d2
	move.l	d6,d3
	movea.l	$47A4(a4),a6
	jsr.l	$-0030(a6)
	move.l	d0,d5
	moveq	#$-01,d0
	cmp.l	d0,d5
	bne	$00008AEE

l00008AE0:
	jsr.l	$-0084(a6)
	move.l	d0,$2030(a4)
	moveq	#$+05,d0
	move.l	d0,$47A0(a4)

l00008AEE:
	move.l	d5,d0
	movem.l	(a7)+,d2-d3/d5-d7/a3/a6
	rts	
00008AF6                   00 00                               ..       

fn00008AF8()
	movem.l	d2-d7/a6,-(a7)
	move.l	$0020(a7),d7
	move.l	$0024(a7),d6
	move.l	$0028(a7),d5
	tst.l	$2048(a4)
	beq	$00008B12

l00008B0E:
	jsr.l	$092E(pc)

l00008B12:
	clr.l	$2030(a4)
	move.l	d5,d0
	subq.l	#$01,d0
	move.l	d7,d1
	move.l	d6,d2
	move.l	d0,d3
	movea.l	$47A4(a4),a6
	jsr.l	$-0042(a6)
	move.l	d0,d4
	moveq	#$-01,d0
	cmp.l	d0,d4
	bne	$00008B3E

l00008B30:
	jsr.l	$-0084(a6)
	move.l	d0,$2030(a4)
	moveq	#$+16,d0
	move.l	d0,$47A0(a4)

l00008B3E:
	move.l	d5,d0
	tst.l	d0
	beq	$00008B4E

l00008B44:
	subq.l	#$01,d0
	beq	$00008B52

l00008B48:
	subq.l	#$01,d0
	beq	$00008B58

l00008B4C:
	bra	$00008B66

l00008B4E:
	move.l	d6,d0
	bra	$00008B66

l00008B52:
	move.l	d4,d0
	add.l	d6,d0
	bra	$00008B66

l00008B58:
	move.l	d7,d1
	moveq	#$+00,d2
	move.l	d2,d3
	movea.l	$47A4(a4),a6
	jsr.l	$-0042(a6)

l00008B66:
	movem.l	(a7)+,d2-d7/a6
	rts	
00008B6C                                     00 00 00 00             ....
00008B70 00 00 70 61                                     ..pa           

fn00008B74()
	movem.l	d2-d3/d5-d7/a3/a6,-(a7)
	move.l	$0020(a7),d7
	movea.l	$0024(a7),a3
	move.l	$0028(a7),d6
	tst.l	$2048(a4)
	beq	$00008B8E

l00008B8A:
	jsr.l	$08B2(pc)

l00008B8E:
	clr.l	$2030(a4)
	move.l	d7,d1
	move.l	a3,d2
	move.l	d6,d3
	movea.l	$47A4(a4),a6
	jsr.l	$-002A(a6)
	move.l	d0,d5
	moveq	#$-01,d0
	cmp.l	d0,d5
	bne	$00008BB6

l00008BA8:
	jsr.l	$-0084(a6)
	move.l	d0,$2030(a4)
	moveq	#$+05,d0
	move.l	d0,$47A0(a4)

l00008BB6:
	move.l	d5,d0
	movem.l	(a7)+,d2-d3/d5-d7/a3/a6
	rts	
00008BBE                                           00 00               ..

fn00008BC0()
	movem.l	d2/d6-d7/a3/a6,-(a7)
	movea.l	$0018(a7),a3
	move.l	$001C(a7),d7
	tst.l	$2048(a4)
	beq	$00008BD6

l00008BD2:
	jsr.l	$086A(pc)

l00008BD6:
	clr.l	$2030(a4)
	move.l	a3,d1
	move.l	d7,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-001E(a6)
	move.l	d0,d6
	tst.l	d6
	bne	$00008BFE

l00008BEC:
	jsr.l	$-0084(a6)
	move.l	d0,$2030(a4)
	moveq	#$+02,d0
	move.l	d0,$47A0(a4)
	moveq	#$-01,d0
	bra	$00008C00

l00008BFE:
	move.l	d6,d0

l00008C00:
	movem.l	(a7)+,d2/d6-d7/a3/a6
	rts	
00008C06                   00 00                               ..       

fn00008C08()
	link	a5,#$FED8
	movem.l	d2/d6-d7/a2-a3/a6,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a2
	move.l	$0010(a5),d7
	clr.l	$2030(a4)
	move.l	d7,$47A8(a4)
	tst.l	$1E7C(a4)
	bne	$00008C38

l00008C2A:
	pea	$0000006C
	jsr.l	$-08F2(pc)
	addq.w	#$04,a7
	move.l	d0,$1E7C(a4)

l00008C38:
	tst.l	$1E80(a4)
	bne	$00008C4C

l00008C3E:
	pea	$0000006C
	jsr.l	$-0906(pc)
	addq.w	#$04,a7
	move.l	d0,$1E80(a4)

l00008C4C:
	pea	$-0128(a5)
	move.l	$1E80(a4),-(a7)
	pea	$-0108(a5)
	move.l	$1E7C(a4),-(a7)
	move.l	a2,-(a7)
	jsr.l	$-134E(pc)
	pea	$-0108(a5)
	move.l	$1E7C(a4),-(a7)
	jsr.l	$-10F6(pc)
	lea	$001C(a7),a7
	move.b	$-0128(a5),d0
	tst.b	d0
	beq	$00008C96

l00008C7A:
	pea	$016C(pc)
	move.l	$1E80(a4),-(a7)
	jsr.l	$-110E(pc)
	pea	$-0128(a5)
	move.l	$1E80(a4),-(a7)
	jsr.l	$-111A(pc)
	lea	$0010(a7),a7

l00008C96:
	movea.l	$1E80(a4),a0
	movea.l	a2,a1

l00008C9C:
	move.b	(a0)+,d0
	cmp.b	(a1)+,d0
	bne	$00008CAE

l00008CA2:
	tst.b	d0
	bne	$00008C9C

l00008CA6:
	bne	$00008CAE

l00008CA8:
	tst.l	$204C(a4)
	bne	$00008CD8

l00008CAE:
	move.l	$1E7C(a4),d1
	moveq	#$-02,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-0054(a6)
	move.l	d0,$47AC(a4)
	tst.l	d0
	bne	$00008CEC

l00008CC4:
	moveq	#$+02,d0
	move.l	d0,$47A0(a4)
	move.l	#$000000CD,$2030(a4)
	moveq	#$-01,d0
	bra	$00008DDC

l00008CD8:
	move.l	$1E7C(a4),-(a7)
	jsr.l	$-053C(pc)
	addq.w	#$04,a7
	clr.l	$1E7C(a4)
	move.l	$204C(a4),$47AC(a4)

l00008CEC:
	move.l	$1E80(a4),-(a7)
	jsr.l	$-11C0(pc)
	pea	$00000104
	move.l	d0,$1E80(a4)
	jsr.l	$-08C0(pc)
	addq.w	#$08,a7
	moveq	#$+00,d6
	move.l	d0,$-0008(a5)

l00008D08:
	movea.l	a2,a0

l00008D0A:
	tst.b	(a0)+
	bne	$00008D0A

l00008D0E:
	subq.l	#$01,a0
	suba.l	a2,a0
	cmp.l	a0,d6
	bge	$00008D2A

l00008D16:
	move.b	(a2,d6),d0
	moveq	#$+3F,d1
	cmp.b	d1,d0
	beq	$00008D2A

l00008D20:
	moveq	#$+23,d1
	cmp.b	d1,d0
	beq	$00008D2A

l00008D26:
	addq.l	#$01,d6
	bra	$00008D08

l00008D2A:
	movea.l	a2,a0

l00008D2C:
	tst.b	(a0)+
	bne	$00008D2C

l00008D30:
	subq.l	#$01,a0
	suba.l	a2,a0
	cmp.l	a0,d6
	bne	$00008D86

l00008D38:
	tst.l	$478C(a4)
	bne	$00008D86

l00008D3E:
	move.l	$47A8(a4),-(a7)
	move.l	a2,-(a7)
	move.l	$-0008(a5),-(a7)
	bsr	$00008F10
	move.l	d0,d6
	movea.l	$-0008(a5),a0
	movea.l	a3,a1
	moveq	#$+40,d0

l00008D56:
	move.l	(a0)+,(a1)+
	dbra	d0,$00008D56

l00008D5C:
	pea	$00000104
	move.l	$-0008(a5),-(a7)
	jsr.l	$-0F18(pc)
	lea	$0014(a7),a7
	move.l	$47AC(a4),d0
	move.l	$204C(a4),d1
	cmp.l	d0,d1
	beq	$00008D82

l00008D78:
	move.l	d0,d1
	movea.l	$47A4(a4),a6
	jsr.l	$-005A(a6)

l00008D82:
	move.l	d6,d0
	bra	$00008DDC

l00008D86:
	move.l	$47AC(a4),d1
	move.l	$-0008(a5),d2
	movea.l	$47A4(a4),a6
	jsr.l	$-0066(a6)
	tst.l	d0
	bne	$00008DB6

l00008D9A:
	moveq	#$+14,d0
	move.l	d0,$47A0(a4)
	move.l	#$000000CC,$2030(a4)
	pea	$00000104
	move.l	d2,-(a7)
	jsr.l	$-0F62(pc)
	moveq	#$-01,d0
	bra	$00008DDC

l00008DB6:
	move.l	$-0008(a5),-(a7)
	bsr	$00008E70
	move.l	d0,d6
	movea.l	$-0008(a5),a0
	movea.l	a3,a1
	moveq	#$+40,d0

l00008DC8:
	move.l	(a0)+,(a1)+
	dbra	d0,$00008DC8

l00008DCE:
	pea	$00000104
	move.l	$-0008(a5),-(a7)
	jsr.l	$-0F8A(pc)
	move.l	d6,d0

l00008DDC:
	movem.l	$-0140(a5),d2/d6-d7/a2-a3/a6
	unlk	a5
	rts	
00008DE6                   2E 00                               ..       

fn00008DE8()
	movem.l	d2/d7/a2-a3/a6,-(a7)
	movea.l	$0018(a7),a3
	clr.l	$2030(a4)
	tst.l	$1E80(a4)
	bne	$00008DFE

l00008DFA:
	moveq	#$-01,d0
	bra	$00008E6A

l00008DFE:
	move.l	$1E7C(a4),d0
	beq	$00008E2A

l00008E04:
	move.l	d0,d1
	moveq	#$-02,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-0054(a6)
	move.l	d0,$47AC(a4)
	tst.l	d0
	bne	$00008E30

l00008E18:
	moveq	#$+02,d0
	move.l	d0,$47A0(a4)
	move.l	#$000000CD,$2030(a4)
	moveq	#$-01,d0
	bra	$00008E6A

l00008E2A:
	move.l	$204C(a4),$47AC(a4)

l00008E30:
	pea	$00000104
	jsr.l	$-09F8(pc)
	movea.l	d0,a2
	movea.l	a3,a0
	movea.l	a2,a1
	moveq	#$+40,d0

l00008E40:
	move.l	(a0)+,(a1)+
	dbra	d0,$00008E40

l00008E46:
	move.l	a2,(a7)
	bsr	$00008E70
	move.l	d0,d7
	movea.l	a2,a0
	movea.l	a3,a1
	moveq	#$+40,d0

l00008E54:
	move.l	(a0)+,(a1)+
	dbra	d0,$00008E54

l00008E5A:
	pea	$00000104
	move.l	a2,-(a7)
	jsr.l	$-1014(pc)
	lea	$000C(a7),a7
	move.l	d7,d0

l00008E6A:
	movem.l	(a7)+,d2/d7/a2-a3/a6
	rts	

fn00008E70()
	movem.l	d2/d6-d7/a3/a6,-(a7)
	movea.l	$0018(a7),a3
	moveq	#$+00,d7

l00008E7A:
	moveq	#$+74,d0
	add.l	d0,d0
	cmp.l	d0,d7
	beq	$00008EE6

l00008E82:
	move.l	$47AC(a4),d1
	move.l	a3,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-006C(a6)
	tst.l	d0
	bne	$00008E9C

l00008E94:
	jsr.l	$-0084(a6)
	move.l	d0,d7
	bra	$00008E7A

l00008E9C:
	tst.l	$47A8(a4)
	bne	$00008EAA

l00008EA2:
	move.l	$0004(a3),d0
	tst.l	d0
	bgt	$00008E7A

l00008EAA:
	tst.l	$478C(a4)
	beq	$00008ECE

l00008EB0:
	lea	$0008(a3),a0
	move.l	a0,-(a7)
	jsr.l	$-1386(pc)
	move.l	$1E80(a4),(a7)
	move.l	d0,-(a7)
	jsr.l	$-129C(pc)
	addq.w	#$08,a7
	move.l	d0,d6
	tst.l	d6
	bne	$00008EE6

l00008ECC:
	bra	$00008E7A

l00008ECE:
	lea	$0008(a3),a0
	move.l	$1E80(a4),-(a7)
	move.l	a0,-(a7)
	jsr.l	$05B0(pc)
	addq.w	#$08,a7
	move.l	d0,d6
	tst.l	d6
	bne	$00008EE6

l00008EE4:
	bra	$00008E7A

l00008EE6:
	move.l	$47AC(a4),d0
	move.l	$204C(a4),d1
	cmp.l	d0,d1
	beq	$00008EFC

l00008EF2:
	move.l	d0,d1
	movea.l	$47A4(a4),a6
	jsr.l	$-005A(a6)

l00008EFC:
	moveq	#$+74,d0
	add.l	d0,d0
	cmp.l	d0,d7
	bne	$00008F08

l00008F04:
	moveq	#$+01,d0
	bra	$00008F0A

l00008F08:
	moveq	#$+00,d0

l00008F0A:
	movem.l	(a7)+,d2/d6-d7/a3/a6
	rts	

fn00008F10()
	link	a5,#$FFF4
	movem.l	d2/d6-d7/a2-a3/a6,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a2
	clr.l	$2030(a4)
	move.l	a2,d1
	moveq	#$-02,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-0054(a6)
	move.l	d0,d6
	tst.l	d6
	bne	$00008F48

l00008F36:
	moveq	#$+02,d0
	move.l	d0,$47A0(a4)
	move.l	#$000000CD,$2030(a4)
	moveq	#$-01,d0
	bra	$00008F9A

l00008F48:
	pea	$00000104
	jsr.l	$-0A48(pc)
	addq.w	#$04,a7
	move.l	d0,$-000C(a5)
	move.l	d6,d1
	move.l	d0,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-0066(a6)
	tst.l	d0
	bne	$00008F78

l00008F66:
	moveq	#$+02,d0
	move.l	d0,$47A0(a4)
	move.l	#$000000CD,$2030(a4)
	moveq	#$-01,d7
	bra	$00008F80

l00008F78:
	move.l	d6,d1
	jsr.l	$-005A(a6)
	moveq	#$+00,d7

l00008F80:
	movea.l	d2,a0
	movea.l	a3,a1
	moveq	#$+40,d0

l00008F86:
	move.l	(a0)+,(a1)+
	dbra	d0,$00008F86

l00008F8C:
	pea	$00000104
	move.l	$-000C(a5),-(a7)
	jsr.l	$-1148(pc)
	move.l	d7,d0

l00008F9A:
	movem.l	$-0024(a5),d2/d6-d7/a2-a3/a6
	unlk	a5
	rts	
00008FA4             00 00 00 00 00 00 00 00 00 00 00 00     ............
00008FB0 00 00 00 00 00 00 70 61                         ......pa       

fn00008FB8()
	link	a5,#$FFFC
	movem.l	d2/d7/a3/a6,-(a7)
	movea.l	$0008(a5),a3
	tst.l	$2048(a4)
	beq	$00008FCE

l00008FCA:
	jsr.l	$0472(pc)

l00008FCE:
	clr.l	$2030(a4)
	move.l	a3,d1
	moveq	#$-02,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-0054(a6)
	move.l	d0,d7
	tst.l	d7
	beq	$00008FEE

l00008FE4:
	move.l	d7,d1
	jsr.l	$-005A(a6)
	moveq	#$-01,d0
	bra	$00009014

l00008FEE:
	move.l	a3,d1
	move.l	#$000003EE,d2
	jsr.l	$-001E(a6)
	move.l	d0,d7
	tst.l	d7
	bne	$00009012

l00009000:
	jsr.l	$-0084(a6)
	move.l	d0,$2030(a4)
	moveq	#$+02,d0
	move.l	d0,$47A0(a4)
	moveq	#$-01,d0
	bra	$00009014

l00009012:
	move.l	d7,d0

l00009014:
	movem.l	(a7)+,d2/d7/a3/a6
	unlk	a5
	rts	

fn0000901C()
	link	a5,#$FFFC
	movem.l	d2/d7/a3/a6,-(a7)
	movea.l	$0008(a5),a3
	tst.l	$2048(a4)
	beq	$00009032

l0000902E:
	jsr.l	$040E(pc)

l00009032:
	clr.l	$2030(a4)
	move.l	a3,d1
	moveq	#$-02,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-0054(a6)
	move.l	d0,d7
	tst.l	d7
	beq	$00009054

l00009048:
	move.l	d7,d1
	jsr.l	$-005A(a6)
	move.l	a3,d1
	jsr.l	$-0048(a6)

l00009054:
	move.l	a3,d1
	move.l	#$000003EE,d2
	jsr.l	$-001E(a6)
	move.l	d0,d7
	tst.l	d7
	bne	$00009078

l00009066:
	jsr.l	$-0084(a6)
	move.l	d0,$2030(a4)
	moveq	#$+02,d0
	move.l	d0,$47A0(a4)
	moveq	#$-01,d0
	bra	$0000907A

l00009078:
	move.l	d7,d0

l0000907A:
	movem.l	(a7)+,d2/d7/a3/a6
	unlk	a5
	rts	
00009082       00 00                                       ..           

fn00009084()
	movem.l	d7/a6,-(a7)
	move.l	$000C(a7),d7
	tst.l	$2048(a4)
	beq	$00009096

l00009092:
	jsr.l	$03AA(pc)

l00009096:
	move.l	d7,d1
	movea.l	$47A4(a4),a6
	jsr.l	$-0024(a6)
	moveq	#$+00,d0
	movem.l	(a7)+,d7/a6
	rts	

fn000090A8()
	link	a5,#$FFA8
	movem.l	d7/a3/a6,-(a7)
	lea	$0092(pc),a1
	moveq	#$+00,d0
	movea.l	$00000004,a6
	jsr.l	$-0228(a6)
	movea.l	d0,a3
	move.l	a3,d0
	bne	$000090CE

l000090C4:
	pea	$00000014
	jsr.l	$-1A30(pc)
	addq.w	#$04,a7

l000090CE:
	moveq	#$+00,d7
	movea.l	$2060(a4),a0
	move.b	$-0001(a0),d7
	move.l	d7,d0
	lea	$-0050(a5),a1
	bra	$000090E2

l000090E0:
	move.b	(a0)+,(a1)+

l000090E2:
	subq.l	#$01,d0
	bcc	$000090E0

l000090E6:
	clr.b	(-50,a5,d7)
	lea	$-0050(a5),a0
	move.l	a0,$1E90(a4)
	move.l	a3,-(a7)
	pea	$00000028
	pea	$000000FA
	moveq	#$+00,d0
	move.l	d0,-(a7)
	move.l	d0,-(a7)
	pea	$1EAC(a4)
	moveq	#$+00,d1
	move.l	d1,-(a7)
	pea	$1E98(a4)
	move.l	d1,-(a7)
	jsr.l	$-3098(pc)
	pea	$00000014
	jsr.l	$-1A80(pc)
	movem.l	$-0064(a5),d7/a3/a6
	unlk	a5
	rts	
00009126                   2A 2A 20 53 74 61 63 6B 20 4F       ** Stack O
00009130 76 65 72 66 6C 6F 77 20 2A 2A 00 00 45 58 49 54 verflow **..EXIT
00009140 00 00 69 6E 74 75 69 74 69 6F 6E 2E 6C 69 62 72 ..intuition.libr
00009150 61 72 79 00                                     ary.           

fn00009154()
	movem.l	d2-d3,-(a7)
	move.l	d0,d2
	move.l	d1,d3
	swap.l	d2
	swap.l	d3
	mulu.w	d1,d2
	mulu.w	d0,d3
	mulu.w	d1,d0
	add.w	d3,d2
	swap.l	d2
	clr.w	d2
	add.l	d2,d0
	movem.l	(a7)+,d2-d3
	rts	

fn00009174()
	tst.l	d0
	bpl	$00009196

l0000917A:
	neg.l	d0
	tst.l	d1
	bpl	$0000918C

l00009182:
	neg.l	d1
	bsr	$000091A6
	neg.l	d1
	rts	

l0000918C:
	bsr	$000091A6
	neg.l	d0
	neg.l	d1
	rts	

l00009196:
	tst.l	d1
	bpl	$000091A6

l0000919C:
	neg.l	d1
	bsr	$000091A6
	neg.l	d0
	rts	

fn000091A6()
	move.l	d2,-(a7)
	swap.l	d1
	move.w	d1,d2
	bne	$000091D0

l000091B0:
	swap.l	d0
	swap.l	d1
	swap.l	d2
	move.w	d0,d2
	beq	$000091C0

l000091BC:
	divu.w	d1,d2
	move.w	d2,d0

l000091C0:
	swap.l	d0
	move.w	d0,d2
	divu.w	d1,d2
	move.w	d2,d0
	swap.l	d2
	move.w	d2,d1
	move.l	(a7)+,d2
	rts	

l000091D0:
	move.l	d3,-(a7)
	moveq	#$+10,d3
	cmpi.w	#$0080,d1
	bcc	$000091E0

l000091DC:
	rol.l	#$08,d1
	subq.w	#$08,d3

l000091E0:
	cmpi.w	#$0800,d1
	bcc	$000091EC

l000091E8:
	rol.l	#$04,d1
	subq.w	#$04,d3

l000091EC:
	cmpi.w	#$2000,d1
	bcc	$000091F8

l000091F4:
	rol.l	#$02,d1
	subq.w	#$02,d3

l000091F8:
	tst.w	d1
	bmi	$00009202

l000091FE:
	rol.l	#$01,d1
	subq.w	#$01,d3

l00009202:
	move.w	d0,d2
	lsr.l	d3,d0
	swap.l	d2
	clr.w	d2
	lsr.l	d3,d2
	swap.l	d3
	divu.w	d1,d0
	move.w	d0,d3
	move.w	d2,d0
	move.w	d3,d2
	swap.l	d1
	mulu.w	d1,d2
	sub.l	d2,d0
	bcc	$00009226

l00009220:
	subq.w	#$01,d3
	add.l	d1,d0

l00009224:
	bcc	$00009224

l00009226:
	moveq	#$+00,d1
	move.w	d3,d1
	swap.l	d3
	rol.l	d3,d0
	swap.l	d0
	exg	d0,d1
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	rts	
00009238                         4E 55 FF 98 48 E7 33 32         NU..H.32
00009240 7E 00 20 6C 20 60 1E 28 FF FF 70 4F BE 80 6F 02 ~. l `.(..pO..o.
00009250 2E 00 20 07 43 ED FF AF 60 02 12 D8 53 80 64 FA .. .C...`...S.d.
00009260 42 35 78 AF 93 C9 2C 78 00 04 4E AE FE DA 26 40 B5x...,x..N...&@
00009270 20 2B 00 AC 67 48 E5 80 24 40 2C 2A 00 38 4A 86  +..gH..$@,*.8J.
00009280 66 04 2C 2B 00 A0 4A 86 67 34 22 06 41 FA 00 B6 f.,+..J.g4".A...
00009290 24 08 76 0B 2C 6C 47 A4 4E AE FF D0 20 47 52 87 $.v.,lG.N... GR.
000092A0 20 08 1B BC 00 0A 08 AF 22 06 41 ED FF AF 24 08  .......".A...$.
000092B0 26 07 2C 6C 47 A4 4E AE FF D0 70 FF 60 52 43 FA &.,lG.N...p.`RC.
000092C0 00 90 70 00 2C 78 00 04 4E AE FD D8 2B 40 FF 9A ..p.,x..N...+@..
000092D0 66 04 70 FF 60 3A 41 ED FF AF 29 48 1E E0 2F 2D f.p.`:A...)H../-
000092E0 FF 9A 48 78 00 3C 48 78 00 FA 70 00 2F 00 2F 00 ..Hx.<Hx..p././.
000092F0 48 6C 1E FC 48 6C 1E E8 48 6C 1E D4 42 A7 4E BA Hl..Hl..Hl..B.N.
00009300 CD 78 4F EF 00 24 53 80 67 04 70 FF 60 02 70 00 .xO..$S.g.p.`.p.
00009310 4C DF 4C CC 4E 5D 4E 75 2A 2A 20 55 73 65 72 20 L.L.N]Nu** User 
00009320 41 62 6F 72 74 20 52 65 71 75 65 73 74 65 64 20 Abort Requested 
00009330 2A 2A 00 00 43 4F 4E 54 49 4E 55 45 00 00 41 42 **..CONTINUE..AB
00009340 4F 52 54 00 2A 2A 2A 20 42 72 65 61 6B 3A 20 00 ORT.*** Break: .
00009350 69 6E 74 75 69 74 69 6F 6E 2E 6C 69 62 72 61 72 intuition.librar
00009360 79 00 00 00 00 00 00 00 00 00 70 61             y.........pa   

fn0000936C()
	movem.l	d7/a3,-(a7)
	move.l	$000C(a7),d7
	move.l	d7,-(a7)
	jsr.l	$007E(pc)
	addq.w	#$04,a7
	movea.l	d0,a3
	move.l	a3,d0
	bne	$00009386

l00009382:
	moveq	#$-01,d0
	bra	$000093AE

l00009386:
	btst	#$0004,$0003(a3)
	beq	$00009394

l0000938E:
	moveq	#$+00,d0
	move.l	d0,(a3)
	bra	$000093AE

l00009394:
	move.l	$0004(a3),-(a7)
	jsr.l	$-0314(pc)
	addq.w	#$04,a7
	moveq	#$+00,d0
	move.l	d0,(a3)
	tst.l	$2030(a4)
	beq	$000093AC

l000093A8:
	moveq	#$-01,d0
	bra	$000093AE

l000093AC:
	moveq	#$+00,d0

l000093AE:
	movem.l	(a7)+,d7/a3
	rts	

fn000093B4()
	movem.l	d2/d6-d7/a3/a6,-(a7)
	movea.l	$0018(a7),a3
	move.l	$001C(a7),d7
	move.l	d7,d6
	not.l	d6
	moveq	#$-10,d0
	eor.l	d0,d6
	move.l	a3,d1
	move.l	d6,d2
	movea.l	$47A4(a4),a6
	jsr.l	$-00BA(a6)
	tst.l	d0
	bne	$000093EA

l000093D8:
	moveq	#$+02,d0
	move.l	d0,$47A0(a4)
	move.l	#$000000CD,$2030(a4)
	moveq	#$-01,d0
	bra	$000093EC

l000093EA:
	moveq	#$+00,d0

l000093EC:
	movem.l	(a7)+,d2/d6-d7/a3/a6
	rts	
000093F2       00 00                                       ..           

fn000093F4()
	move.l	d7,-(a7)
	move.l	$0008(a7),d7
	moveq	#$+00,d0
	move.l	d0,$2030(a4)
	tst.l	d7
	bmi	$00009426

l00009404:
	cmp.l	$1DD8(a4),d7
	bge	$00009426

l0000940A:
	move.l	d7,d0
	asl.l	#$03,d0
	lea	$4560(a4),a0
	tst.l	(a0,d0)
	beq	$00009426

l00009418:
	move.l	d7,d0
	asl.l	#$03,d0
	lea	$4560(a4),a0
	adda.l	d0,a0
	move.l	a0,d0
	bra	$0000942E

l00009426:
	moveq	#$+09,d0
	move.l	d0,$47A0(a4)
	moveq	#$+00,d0

l0000942E:
	move.l	(a7)+,d7
	rts	
00009432       00 00 00 00 00 00 00 00 70 61               ........pa   

fn0000943C()
	movem.l	d7/a6,-(a7)
	moveq	#$+00,d0
	move.l	#$00003000,d1
	movea.l	$00000004,a6
	jsr.l	$-0132(a6)
	move.l	d0,d7
	andi.l	#$00003000,d7
	tst.l	d7
	beq	$0000947C

l0000945C:
	tst.l	$2048(a4)
	beq	$0000947C

l00009462:
	movea.l	$2048(a4),a0
	jsr.l	(a0)
	tst.l	d0
	bne	$0000946E

l0000946C:
	bra	$0000947C

l0000946E:
	clr.l	$2048(a4)
	pea	$00000014
	jsr.l	$-1DDE(pc)
	addq.w	#$04,a7

l0000947C:
	movem.l	(a7)+,d7/a6
	rts	
00009482       61 B8 4E 75 00 00                           a.Nu..       

fn00009488()
	link	a5,#$FFD0
	movem.l	d5-d7/a2-a3,-(a7)
	movea.l	$0008(a5),a3
	movea.l	$000C(a5),a2
	moveq	#$+00,d6
	move.l	a3,$-0004(a5)

l0000949E:
	move.b	(a2)+,d7
	tst.b	d7
	beq	$000095AE

l000094A6:
	moveq	#$+00,d0
	move.b	d7,d0
	subi.w	#$0023,d0
	beq	$000094CA

l000094B0:
	subq.w	#$04,d0
	beq	$0000955C

l000094B6:
	subi.w	#$0018,d0
	bne	$00009586

l000094BE:
	move.b	(a3)+,d0
	tst.b	d0
	bne	$0000949E

l000094C4:
	moveq	#$+00,d0
	bra	$000095BC

l000094CA:
	move.b	(a2)+,d7
	moveq	#$+00,d0
	move.b	d7,d0
	subi.w	#$0028,d0
	beq	$000094F0

l000094D6:
	subi.w	#$0017,d0
	bne	$0000952C

l000094DC:
	tst.b	(a3)
	beq	$0000949E

l000094E0:
	move.l	a2,-(a7)
	move.l	a3,-(a7)
	bsr	$00009488
	addq.w	#$08,a7
	tst.l	d0
	bne	$0000949E

l000094EC:
	addq.l	#$01,a3
	bra	$000094DC

l000094F0:
	move.b	(a2)+,d7
	moveq	#$+29,d0
	cmp.b	d0,d7
	beq	$00009504

l000094F8:
	movea.l	d6,a0
	addq.l	#$01,d6
	move.l	a0,d0
	move.b	d7,(-25,a7,d0)
	bra	$000094F0

l00009504:
	clr.b	(-25,a5,d6)

l00009508:
	pea	$-0025(a5)
	move.l	a3,-(a7)
	bsr	$000095C4
	addq.w	#$08,a7
	tst.l	d0
	beq	$0000949E

l00009518:
	lea	$-0025(a5),a0
	movea.l	a0,a1

l0000951E:
	tst.b	(a1)+
	bne	$0000951E

l00009522:
	subq.l	#$01,a1
	suba.l	a0,a1
	move.l	a1,d0
	adda.l	d0,a3
	bra	$00009508

l0000952C:
	move.l	d7,d5

l0000952E:
	move.b	(a3),d0
	beq	$0000949E

l00009534:
	moveq	#$+00,d1
	move.b	d0,d1
	move.l	d1,-(a7)
	jsr.l	$-1D7E(pc)
	moveq	#$+00,d1
	move.b	d5,d1
	move.l	d1,(a7)
	move.l	d0,$0018(a7)
	jsr.l	$-1D8C(pc)
	addq.w	#$04,a7
	move.l	$0014(a7),d1
	cmp.l	d0,d1
	bne	$0000949E

l00009558:
	addq.l	#$01,a3
	bra	$0000952E

l0000955C:
	move.b	(a2)+,d7
	moveq	#$+00,d0
	move.b	d7,d0
	move.l	d0,-(a7)
	jsr.l	$-1DA8(pc)
	moveq	#$+00,d1
	move.b	(a3)+,d1
	move.l	d1,(a7)
	move.l	d0,$0018(a7)
	jsr.l	$-1DB6(pc)
	addq.w	#$04,a7
	move.l	$0014(a7),d1
	cmp.l	d0,d1
	beq	$0000949E

l00009582:
	moveq	#$+00,d0
	bra	$000095BC

l00009586:
	moveq	#$+00,d0
	move.b	d7,d0
	move.l	d0,-(a7)
	jsr.l	$-1DD0(pc)
	moveq	#$+00,d1
	move.b	(a3)+,d1
	move.l	d1,(a7)
	move.l	d0,$0018(a7)
	jsr.l	$-1DDE(pc)
	addq.w	#$04,a7
	move.l	$0014(a7),d1
	cmp.l	d0,d1
	beq	$0000949E

l000095AA:
	moveq	#$+00,d0
	bra	$000095BC

l000095AE:
	tst.b	(a3)
	beq	$000095B6

l000095B2:
	moveq	#$+00,d0
	bra	$000095BC

l000095B6:
	move.l	a3,d0
	sub.l	$-0004(a5),d0

l000095BC:
	movem.l	(a7)+,d5-d7/a2-a3
	unlk	a5
	rts	

fn000095C4()
	link	a5,#$FFFC
	movem.l	a2-a3,-(a7)
	movea.l	$0014(a7),a3
	movea.l	$0018(a7),a2

l000095D4:
	tst.b	(a2)
	beq	$000095FE

l000095D8:
	moveq	#$+00,d0
	move.b	(a3)+,d0
	move.l	d0,-(a7)
	jsr.l	$-1E22(pc)
	moveq	#$+00,d1
	move.b	(a2)+,d1
	move.l	d1,(a7)
	move.l	d0,$000C(a7)
	jsr.l	$-1E30(pc)
	addq.w	#$04,a7
	move.l	$0008(a7),d1
	cmp.l	d0,d1
	beq	$000095D4

l000095FA:
	moveq	#$+00,d0
	bra	$00009600

l000095FE:
	moveq	#$+01,d0

l00009600:
	movem.l	(a7)+,a2-a3
	unlk	a5
	rts	
00009608                         2E 52 4E 43 00 50 52 4F         .RNC.PRO
00009610 2D 50 41 43 4B 20 32 2E 30 38 20 53 6F 66 74 77 -PACK 2.08 Softw
00009620 61 72 65 20 44 65 76 65 6C 6F 70 65 72 73 20 46 are Developers F
00009630 69 6C 65 20 43 6F 6D 70 72 65 73 73 69 6F 6E 20 ile Compression 
00009640 55 74 69 6C 69 74 79 20 33 20 41 70 72 20 39 32 Utility 3 Apr 92
00009650 0A 43 6F 70 79 72 69 67 68 74 20 28 63 29 20 31 .Copyright (c) 1
00009660 39 39 31 2C 39 32 20 52 6F 62 20 4E 6F 72 74 68 991,92 Rob North
00009670 65 6E 20 43 6F 6D 70 75 74 69 6E 67 2C 20 55 4B en Computing, UK
00009680 2E 20 41 6C 6C 20 52 69 67 68 74 73 20 52 65 73 . All Rights Res
00009690 65 72 76 65 64 0A 50 4F 20 42 6F 78 20 31 37 20 erved.PO Box 17 
000096A0 42 6F 72 64 6F 6E 20 48 61 6E 74 73 20 47 55 33 Bordon Hants GU3
000096B0 35 20 38 44 59 20 55 2E 4B 2E 20 54 65 6C 3A 30 5 8DY U.K. Tel:0
000096C0 34 32 38 2D 37 31 33 36 33 35 20 46 61 78 3A 30 428-713635 Fax:0
000096D0 34 32 38 2D 37 31 33 39 39 39 0A 0A 00 55 73 61 428-713999...Usa
000096E0 67 65 3A 20 50 50 20 3C 63 6F 6D 6D 61 6E 64 3E ge: PP <command>
000096F0 20 3C 66 69 6C 65 20 74 79 70 65 3E 20 5B 2D 3C  <file type> [-<
00009700 6F 70 74 69 6F 6E 73 3E 5D 20 5B 2E 3C 65 78 74 options>] [.<ext
00009710 3E 5D 20 3C 66 69 6C 65 28 73 29 2E 2E 2E 3E 0A >] <file(s)...>.
00009720 0A 3C 43 6F 6D 6D 61 6E 64 73 3E 20 20 70 3A 20 .<Commands>  p: 
00009730 50 61 63 6B 20 20 20 20 75 3A 20 55 6E 70 61 63 Pack    u: Unpac
00009740 6B 20 20 20 20 76 3A 20 56 65 72 69 66 79 0A 0A k    v: Verify..
00009750 3C 46 69 6C 65 20 54 79 70 65 73 3E 20 28 66 69 <File Types> (fi
00009760 6C 65 20 74 79 70 65 73 20 3C 70 73 61 6D 3E 20 le types <psam> 
00009770 61 72 65 20 6D 61 64 65 20 74 6F 20 75 6E 70 61 are made to unpa
00009780 63 6B 20 77 68 65 6E 20 72 75 6E 29 0A 0A 20 20 ck when run)..  
00009790 64 3A 20 44 61 74 61 20 20 20 70 3A 20 49 42 4D d: Data   p: IBM
000097A0 20 50 43 20 20 73 3A 20 41 74 61 72 69 20 53 54  PC  s: Atari ST
000097B0 20 20 61 3A 20 43 42 4D 20 41 6D 69 67 61 20 20   a: CBM Amiga  
000097C0 6D 3A 20 4D 43 36 38 30 30 30 20 20 6C 3A 20 41 m: MC68000  l: A
000097D0 74 61 72 69 20 4C 79 6E 78 0A 0A 3C 4F 70 74 69 tari Lynx..<Opti
000097E0 6F 6E 73 3E 20 28 70 3D 70 61 63 6B 20 63 6F 6D ons> (p=pack com
000097F0 6D 61 6E 64 2C 20 75 3D 75 6E 70 61 63 6B 20 63 mand, u=unpack c
00009800 6F 6D 6D 61 6E 64 2C 20 76 3D 76 65 72 69 66 79 ommand, v=verify
00009810 20 63 6F 6D 6D 61 6E 64 29 0A 0A 20 2D 6B 3A 20  command).. -k: 
00009820 28 70 75 76 29 20 6C 6F 63 6B 2F 75 6E 6C 6F 63 (puv) lock/unloc
00009830 6B 20 77 69 74 68 20 4B 65 79 20 20 20 20 20 20 k with Key      
00009840 20 20 20 2D 6F 3A 20 28 70 29 20 4F 76 65 72 77    -o: (p) Overw
00009850 72 69 74 65 20 69 6E 70 75 74 20 3C 66 69 6C 65 rite input <file
00009860 3E 0A 20 20 20 20 20 2D 6B 20 30 78 31 32 33 34 >.     -k 0x1234
00009870 20 28 68 65 78 29 20 20 20 20 20 20 20 20 20 20  (hex)          
00009880 20 20 20 20 20 20 20 20 20 20 2D 70 3A 20 28 70           -p: (p
00009890 29 20 63 68 61 6E 67 65 20 50 61 63 6B 20 62 6C ) change Pack bl
000098A0 6F 63 6B 20 73 69 7A 65 0A 20 2D 6C 3A 20 28 70 ock size. -l: (p
000098B0 29 20 4C 6F 63 6B 20 3C 66 69 6C 65 3E 20 28 73 ) Lock <file> (s
000098C0 74 6F 70 73 20 75 6E 70 61 63 6B 69 6E 67 29 20 tops unpacking) 
000098D0 20 20 20 20 20 2D 70 20 31 30 32 34 30 20 28 31      -p 10240 (1
000098E0 30 32 34 30 20 62 79 74 65 73 29 0A 20 2D 6D 3A 0240 bytes). -m:
000098F0 20 28 70 29 20 70 61 63 6B 69 6E 67 20 4D 65 74  (p) packing Met
00009900 68 6F 64 20 28 64 65 66 61 75 6C 74 3D 31 29 20 hod (default=1) 
00009910 20 20 20 20 2D 76 3A 20 28 70 29 20 56 61 6C 69     -v: (p) Vali
00009920 64 61 74 65 20 64 61 74 61 20 77 68 65 6E 20 75 date data when u
00009930 6E 70 61 63 6B 69 6E 67 0A 20 20 20 20 20 2D 6D npacking.     -m
00009940 20 31 20 65 6D 70 68 61 73 69 73 20 6F 6E 20 70  1 emphasis on p
00009950 61 63 6B 65 64 20 73 69 7A 65 20 20 20 20 20 20 acked size      
00009960 20 2D 78 3A 20 28 70 29 20 4D 43 36 38 30 30 30  -x: (p) MC68000
00009970 20 65 58 65 63 20 61 64 64 72 65 73 73 20 6F 66  eXec address of
00009980 66 73 65 74 0A 20 20 20 20 20 2D 6D 20 32 20 65 fset.     -m 2 e
00009990 6D 70 68 61 73 69 73 20 6F 6E 20 75 6E 70 61 63 mphasis on unpac
000099A0 6B 69 6E 67 20 73 70 65 65 64 20 20 20 20 20 20 king speed      
000099B0 2D 78 20 30 78 31 30 30 20 6A 75 6D 70 73 20 74 -x 0x100 jumps t
000099C0 6F 20 73 74 61 72 74 20 2B 20 30 78 31 30 30 0A o start + 0x100.
000099D0 0A 5B 2E 3C 65 78 74 3E 5D 20 6F 75 74 70 75 74 .[.<ext>] output
000099E0 20 66 69 6C 65 20 65 78 74 65 6E 73 69 6F 6E 0A  file extension.
000099F0 00 00 50 61 63 6B 69 6E 67 00 55 6E 70 61 63 6B ..Packing.Unpack
00009A00 69 6E 67 00 56 65 72 69 66 79 69 6E 67 00 00 00 ing.Verifying...
00009A10 99 F2 00 00 99 FA 00 00 9A 04 44 61 74 61 00 00 ..........Data..
00009A20 49 42 4D 20 50 43 00 00 41 74 61 72 69 20 53 54 IBM PC..Atari ST
00009A30 00 00 43 42 4D 20 41 6D 69 67 61 00 4D 43 36 38 ..CBM Amiga.MC68
00009A40 30 30 30 00 41 74 61 72 69 20 4C 79 6E 78 00 00 000.Atari Lynx..
00009A50 00 00 9A 1A 00 00 9A 20 00 00 9A 28 00 00 9A 32 ....... ...(...2
00009A60 00 00 9A 3C 00 00 9A 44 00 00 45 58 45 00 53 54 ...<...D..EXE.ST
00009A70 00 00 41 4D 00 00 4D 43 36 38 00 00 00 00 00 00 ..AM..MC68......
00009A80 9A 68 00 00 9A 6A 00 00 9A 6E 00 00 9A 72 00 00 .h...j...n...r..
00009A90 9A 76 00 00 9A 7C 4F 4B 00 00 42 41 44 20 43 4F .v...|OK..BAD CO
00009AA0 4D 4D 41 4E 44 00 42 41 44 20 46 49 4C 45 20 54 MMAND.BAD FILE T
00009AB0 59 50 45 00 42 41 44 20 4F 50 54 49 4F 4E 00 00 YPE.BAD OPTION..
00009AC0 42 41 44 20 45 58 54 45 4E 53 49 4F 4E 00 49 4E BAD EXTENSION.IN
00009AD0 53 55 46 46 49 43 49 45 4E 54 20 4D 45 4D 4F 52 SUFFICIENT MEMOR
00009AE0 59 00 43 41 4E 27 54 20 4F 50 45 4E 20 46 49 4C Y.CAN'T OPEN FIL
00009AF0 45 00 43 41 4E 27 54 20 52 45 41 44 20 46 49 4C E.CAN'T READ FIL
00009B00 45 00 43 41 4E 27 54 20 57 52 49 54 45 20 46 49 E.CAN'T WRITE FI
00009B10 4C 45 00 00 43 41 4E 27 54 20 52 45 4E 41 4D 45 LE..CAN'T RENAME
00009B20 20 46 49 4C 45 00 00 00 9A 96 00 00 9A 9A 00 00  FILE...........
00009B30 9A A6 00 00 9A B4 00 00 9A C0 00 00 9A CE 00 00 ................
00009B40 9A E2 00 00 9A F2 00 00 9B 02 00 00 9B 14 4F 4B ..............OK
00009B50 00 00 57 41 52 4E 49 4E 47 3A 20 4F 56 45 52 4C ..WARNING: OVERL
00009B60 41 59 00 00 43 41 4E 27 54 20 50 41 43 4B 00 00 AY..CAN'T PACK..
00009B70 41 4C 52 45 41 44 59 20 50 41 43 4B 45 44 00 00 ALREADY PACKED..
00009B80 50 41 43 4B 45 44 20 44 41 54 41 20 43 52 43 20 PACKED DATA CRC 
00009B90 45 52 52 4F 52 00 55 4E 50 41 43 4B 45 44 20 44 ERROR.UNPACKED D
00009BA0 41 54 41 20 43 52 43 20 45 52 52 4F 52 00 4E 4F ATA CRC ERROR.NO
00009BB0 54 20 41 20 50 41 43 4B 45 44 20 46 49 4C 45 00 T A PACKED FILE.
00009BC0 43 41 4E 27 54 20 50 41 43 4B 20 4F 56 45 52 4C CAN'T PACK OVERL
00009BD0 41 59 20 48 55 4E 4B 00 55 4E 45 58 50 45 43 54 AY HUNK.UNEXPECT
00009BE0 45 44 20 48 55 4E 4B 00 4C 4F 43 4B 45 44 00 00 ED HUNK.LOCKED..
00009BF0 4B 45 59 20 52 45 51 55 49 52 45 44 00 00 57 52 KEY REQUIRED..WR
00009C00 4F 4E 47 20 4B 45 59 00 00 00 9B 4E 00 00 9B 52 ONG KEY....N...R
00009C10 00 00 9B 64 00 00 9B 70 00 00 9B 80 00 00 9B 96 ...d...p........
00009C20 00 00 9B AE 00 00 9B C0 00 00 9B D8 00 00 9B E8 ................
00009C30 00 00 9B F0 00 00 9B FE 25 73 00 00 50 55 56 00 ........%s..PUV.
00009C40 44 50 53 41 4D 4C 00 00 5F 00 4D 4B 58 44 50 57 DPSAML.._.MKXDPW
00009C50 4C 4F 56 00 25 68 69 00 25 49 00 00 25 49 00 00 LOV.%hi.%I..%I..
00009C60 25 69 68 00 25 69 00 00 31 00 32 00 4B 00 56 00 %ih.%i..1.2.K.V.
00009C70 2E 42 49 4E 00 00 50 52 4F 50 41 43 4B 31 2E 24 .BIN..PROPACK1.$
00009C80 24 24 00 00 25 73 20 25 73 20 66 69 6C 65 28 73 $$..%s %s file(s
00009C90 29 00 20 75 73 69 6E 67 20 74 68 65 20 4B 65 79 ). using the Key
00009CA0 3A 20 30 78 25 78 00 00 20 61 6E 64 20 6C 6F 63 : 0x%x.. and loc
00009CB0 6B 69 6E 67 00 00 0A 0A 00 00 43 61 6E 27 74 20 king......Can't 
00009CC0 66 69 6E 64 20 25 73 0A 00 00 72 62 00 00 50 52 find %s...rb..PR
00009CD0 4F 50 41 43 4B 32 2E 24 24 24 00 00 77 2B 62 00 OPACK2.$$$..w+b.
00009CE0 25 2D 2A 2E 2A 73 20 00 77 2B 62 00 5B 25 37 6C %-*.*s .w+b.[%7l
00009CF0 75 5D 20 5B 25 37 6C 75 5D 20 5B 25 32 75 2E 25 u] [%7lu] [%2u.%
00009D00 30 32 75 25 25 5D 00 00 20 5B 25 73 5D 0A 00 00 02u%%].. [%s]...
00009D10 0A 00 0A 25 33 75 20 66 69 6C 65 28 73 29 25 2D ...%3u file(s)%-
00009D20 2A 73 20 5B 25 37 6C 75 5D 20 5B 25 37 6C 75 5D *s [%7lu] [%7lu]
00009D30 20 5B 25 32 75 2E 25 30 32 75 25 25 5D 20 5B 25  [%2u.%02u%%] [%
00009D40 6C 75 68 20 25 6C 75 6D 20 25 6C 75 73 5D 0A 00 luh %lum %lus]..
00009D50 00 00 2F 00 00 00 00 00 72 62 00 00 72 62 00 00 ../.....rb..rb..
00009D60 0A 25 73 0A 00 00 25 73 00 00 00 00 00 0E 08 0A .%s...%s........
00009D70 12 13 16 00 04 04 04 05 05 05 00 06 08 09 15 17 ................
00009D80 1D 1F 28 29 2C 2D 38 39 3C 3D 01 03 04 04 05 05 ..(),-89<=......
00009D90 05 05 06 06 06 06 06 06 06 06 45 58 45 00 45 58 ..........EXE.EX
00009DA0 45 00 43 4F 4D 00 43 4F 4D 00 43 4F 44 45 00 00 E.COM.COM.CODE..
00009DB0 44 41 54 41 00 00 42 53 53 20 00 00 53 59 4D 42 DATA..BSS ..SYMB
00009DC0 00 00 44 42 55 47 00 00 48 55 4E 4B 20 25 75 20 ..DBUG..HUNK %u 
00009DD0 28 25 73 29 20 00 08 08 08 08 08 08 08 08 08 08 (%s) ...........
00009DE0 08 08 08 08 00 00 08 00 20 20 20 00 08 08 08 00 ........   .....
00009DF0 08 08 08 25 32 75 25 25 00 00 00 00 52 4E 43 02 ...%2u%%....RNC.
00009E00 00 00 62 3E 00 00 15 D2 7E D4 C9 C8 02 03 17 43 ..b>....~......C
00009E10 3C 4F 4D 5F 31 2E 42 49 4E 00 01 F6 83 EC 10 8B <OM_1.BIN.......
00009E20 EC BE FA 02 FC E8 41 00 05 0F 8B 00 C8 E8 39 00 ......A.......9.
00009E30 8B D0 03 C6 35 05 06 06 C3 F8 E8 2D 00 AD 88 66 ....5......-...f
00009E40 0A 32 E4 89 76 0C 0D F7 03 C1 11 3B 2E FE 76 78 .2..v......;..vx
00009E50 0E FD 57 4E 4F 8B CA F3 A4 47 89 7E 00 5E FC 8B ..WNO....G.~.^..
00009E60 FE 56 BE 56 01 B9 A0 01 10 C3 E8 60 02 26 D8 AD .V.V.......`.&..
00009E70 86 E0 60 C3 19 02 81 C7 80 64 00 06 04 87 06 06 ..`......d......
00009E80 8B 01 40 04 89 46 0C BF 80 62 C6 46 0B 00 B0 02 ..@..F...b.F....
00009E90 33 E8 AA 10 56 38 05 FA 05 04 38 E8 F4 05 06 06 3...V8....8.....
00009EA0 E8 EE 00 E8 93 2B 08 46 08 EB 1C 08 13 57 00 51 .....+.F.....W.Q
00009EB0 00 14 50 00 83 C1 02 58 40 31 8B D6 7F 2B F0 88 ..P....X@1...+..
00009EC0 62 8B F2 06 35 3B 00 E3 25 0B 1B 8A 4E 0B 53 2E b...5;..%...N.S.
00009ED0 6F D3 7A C0 BA 01 00 D3 E2 4A 21 56 0C 23 D0 8B o.z......J!V.#..
00009EE0 44 02 D3 E3 D3 E0 0B C2 09 5E 0C 4A E1 0E FF 4E D........^.J...N
00009EF0 08 75 B2 FE 4E 0A 75 93 BE 80 78 83 C4 10 33 C0 .u..N.u...x...3.
00009F00 FF 30 E6 87 4A 4E 0C AD C0 37 23 D9 AD 3B C3 06 .0..JN...7#..;..
00009F10 75 F6 8B 4C 3C 11 B9 72 8A C5 E8 1A 00 32 ED 80 u..L<..r.....2..
00009F20 F9 02 72 10 FE C9 8A C1 E8 0C 00 BB 54 E1 E3 0B ..r.........T...
00009F30 C3 8B C8 C3 B0 10 51 8A C8 8B 98 4C 8B 52 8A 6E ......Q....L.R.n
00009F40 48 0B 6C 23 00 D3 52 2A E9 73 21 02 E9 26 86 CD H.l#..R*.s!..&..
00009F50 10 B8 6C D0 D3 CA D3 E8 D3 EB 0B DA 83 C6 02 96 ..l.............
00009F60 16 2A 1B CD B5 10 22 DC 08 1C 93 66 89 46 88 46 .*...."....f.F.F
00009F70 07 58 59 C3 57 52 60 7C B0 05 E8 A0 60 FF 64 E3 .XY.WR`|....`.d.
00009F80 67 8B FC 35 51 B0 F5 C3 94 FF 36 88 05 47 E2 F5 g..5Q.....6..G..
00009F90 59 56 8B F4 83 48 36 8B 7C 0B 21 01 33 DB 87 BA YV...H6.|.!.3...
00009FA0 00 80 51 56 36 3A 04 75 32 50 53 1C 8C B8 55 19 ..QV6:.u2PS...U.
00009FB0 E0 48 AB A7 B1 9B 62 C8 56 6B 10 D7 86 D1 DB D1 .H....b.Vk......
00009FC0 D0 E2 FA AB 8B C6 2B C4 2D BC 2E 36 8A 7D 24 26 ......+.-..6.}$&
00009FD0 89 45 3C 59 5B 58 03 DA 46 E2 C6 5E 59 D1 EA FE .E<Y[X..F..^Y...
00009FE0 C0 3C 11 75 BA 5E 85 04 5A 5F C3 41 01 4B 56 50 .<.u.^..Z_.A.KVP
00009FF0 03 02 67 6A 86 16 40 03 6E 03 FC 27 E8 47 0F 03 ..gj..@.n..'.G..
0000A000 3F 03 87 03 33 66 93 12 B4 7C 14 1D 03 09 5C 01 ?...3f...|....\.
0000A010 B9 0E 02 E0 19 09 CA 8B 07 56 12 E8 A4 01 C0 07 .........V......
0000A020 11 C2 00 E8 71 BD 6B 10 18 E1 0C 01 88 03 22 06 ....q.k.......".
0000A030 01 31 02 20 E8 A5 F0 02 17 69 94 3E 17 62 0C 00 .1. .....i.>.b..
0000A040 17 4D 00 E3 2D AC 32 C0 41 AA E2 F9 D1 4E 7C 10 .M..-.2.A....N|.
0000A050 1F 22 1F AA 1D 1F 8B 01 1F 8B CF 2B CE 87 45 14 .".........+..E.
0000A060 E8 0B 43 29 16 E3 F7 29 E0 01 29 56 51 1A 8B 7E ..C)...)..)VQ..~
0000A070 02 56 E5 8B C3 B9 08 00 D1 E8 73 03 35 01 A0 E2 .V........s.5...
0000A080 F7 AB FE C3 75 ED 59 C0 18 AC 32 D8 8A 01 C7 32 ....u.Y...2....2
0000A090 FF D1 E3 8B 19 97 09 E2 2D F1 5E 3B DA 74 1A E8 ........-.^;.t..
0000A0A0 00 00 5A 83 C2 0D B4 09 CD 21 B8 FF 4C 04 70 42 ..Z......!..L.pB
0000A0B0 61 64 20 43 52 43 0D 0A 24 C3 90 94 4A 77 19 76 ad CRC..$...Jw.v
0000A0C0 04 D3 12 D0 76 08 7D 03 38 5F 7A AE 28 50 7A B8 ....v.}.8_z.(Pz.
0000A0D0 00 E8 B3 B4 2A 68 02 15 68 FC 56 80 F6 17 7C 9B ....*h..h.V...|.
0000A0E0 02 25 68 5F 0F 68 58 0C 87 68 43 C0 35 68 83 C4 .%h_.hX..hC.5h..
0000A0F0 12 F8 F7 5E FA 06 CE 88 AC 87 5C 85 14 E8 10 7D ...^......\....}
0000A100 60 13 59 87 10 B3 12 F5 20 87 7D 00 1D 47 87 10 `.Y..... .}..G..
0000A110 E8 96 D5 08 87 B4 5D 99 54 04 2A 18 FE 03 18 F8 ......].T.*.....
0000A120 00 E8 9D E1 02 18 61 28 18 7C 5A 0C 3F 18 45 2D ......a(.|Z.?.E-
0000A130 B2 99 AA 79 12 AA 79 14 F8 F7 1A FA 4F 8F 79 45 ...y..y.....O.yE
0000A140 58 45 01 A8 79 BA 4D 5A D0 29 00 49 02 06 47 00 XE..y.MZ.).I..G.
0000A150 0C BC 03 0B 04 00 8C 00 D3 8E C3 8C CA 8E DA 8B ................
0000A160 6B 0E 86 A6 8B F1 83 EE 02 8B FE D1 E9 FD F3 A5 k...............
0000A170 53 B8 2E 00 50 8B 2E 0A 00 CB B8 00 10 3B C5 76 S...P........;.v
0000A180 17 01 C5 2B E8 2B D0 2B D8 B1 2A 30 B1 03 86 FB ...+.+.+..*0....
0000A190 8B C8 D1 FF 06 48 8B F0 8B F8 2C B9 74 0B ED 75 .....H....,.t..u
0000A1A0 D9 FC 8E C2 8E DB 83 EC 1C 8B EC 81 EC 80 01 8B ................
0000A1B0 C4 3A 4F 05 0A 89 46 0A 2E 05 0C 11 BE 11 00 AC .:O...F.........
0000A1C0 88 46 14 33 FF 89 7E 15 00 90 16 06 8C 56 04 E8 .F.3..~......V..
0000A1D0 8D 06 37 01 E8 32 01 0E 19 1A 8C 5E 8A 46 83 64 ..7..2.....^.F.d
0000A1E0 56 08 E8 7B A0 99 0A E8 70 75 05 0C E8 67 6F 1D V..{....pu...go.
0000A1F0 14 04 1D 12 EB 22 11 13 D2 19 99 14 CB F0 04 99 ....."..........
0000A200 8E 74 5E E4 07 04 00 46 9F 3B 4A B0 AA B8 1A A8 .t^....F.;J.....
0000A210 B8 1A 8A 4E 7C 15 06 3E A7 16 05 19 A7 16 58 18 ...N|..>......X.
0000A220 75 FF 4E 12 75 A4 8B DF 83 E7 0F 81 C7 00 80 B1 u.N.u...........
0000A230 04 D3 EB 8C C0 03 C3 2D 00 08 8E C0 8B DE 83 E6 .......-........
0000A240 0F E3 0F D8 2E 0F 8E 99 D8 FE 4E 14 74 03 E9 59 ..........N.t..Y
0000A250 FF 1F 1E 07 0E 1F 8C C2 32 ED BE 9A 02 AC 8A C8 ........2.......
0000A260 E3 13 AD 03 F6 C0 AE D5 18 32 E4 AC 03 F8 26 01 .........2....&.
0000A270 15 E2 F8 EB E8 20 8B 36 60 04 90 3E 06 00 03 0C ..... .6`..>....
0000A280 FA 01 16 02 AD EA 76 10 1B 30 DA 33 26 DB FA 8B ......v..0.3&...
0000A290 01 E6 8E D7 FB 2E FF 2F F3 01 B8 12 0A 16 CF BF ......./........
0000A2A0 04 05 9E 0D 7C CF 1E 98 10 18 8B C5 8A 6E 7C 15 ....|........n|.
0000A2B0 38 99 10 18 89 46 88 BA 46 56 10 06 94 11 9F 4A 8....F..FV.....J
0000A2C0 11 69 52 11 93 F9 11 11 3E 16 07 44 4A 13 07 C3 .iR.....>..DJ...
0000A2D0 89 C6 5C 2F 03 3D F8 79 C6 DF 79 0A 00 C6 04 00 ..\/.=.y..y.....
0000A2E0 E8 28 01 AA 8E 42 10 E8 1F B8 C3 8B D3 E8 60 18 .(...B........`.
0000A2F0 0F 5E 1A BE 12 C7 18 26 02 C7 12 E5 C2 02 DF 81 .^.....&........
0000A300 A1 DF 7B 50 DF 75 1F 01 E8 1A 02 09 DF D8 43 DF ..{P.u........C.
0000A310 D1 E1 12 DF B6 3B FC 7F 1D 3E D7 AC 21 00 D7 61 .....;...>..!..a
0000A320 FF 1F 33 F6 8B 3B 4E 0E A0 2F 8B 46 1A E8 5E 7C ..3..;N../.F..^|
0000A330 01 01 1F E5 1D 03 F7 1F E5 4C 01 E5 50 1E 06 51 .........L..P..Q
0000A340 87 53 8B 7E 08 DF 0D FE 81 17 59 E3 01 42 FD 01 .S.~......Y..B..
0000A350 01 47 26 40 02 83 FE 00 75 07 8C 30 D8 80 40 8E .G&@....u..0..@.
0000A360 D8 E2 B8 FF E4 4A 75 E1 07 1F 58 3B C3 74 18 0E .....Ju...X;.t..
0000A370 1F BA 12 03 0C 6A 11 65 49 D7 0F AC CB 74 48 1A .....j.eI....tH.
0000A380 FA 27 0F 29 F9 03 29 54 73 AA 29 6D 4E 29 67 01 .'.)..)Ts.)mN)g.
0000A390 E7 34 E5 01 29 CA 29 29 7C C3 12 BE 29 A8 51 7E .4..).))|...).Q~
0000A3A0 29 01 5F 1B 8C F7 DF 01 52 EC 01 FD BA 3E 03 4B )._.....R....>.K
0000A3B0 74 1F B9 1E 13 D5 02 36 BA 02 2D 97 5D 02 26 D4 t......6..-.].&.
0000A3C0 02 1C 75 02 34 FD 11 02 0F 45 01 E8 40 06 A1 E8 ..u.4....E..@...
0000A3D0 89 50 DE 83 A8 DE 67 7D 1D 22 C2 02 DE E0 50 DE .P....g}."....P.
0000A3E0 D9 F8 12 DE 7D BE 59 1F E8 02 D7 10 1C F5 04 10 ....}.Y.........
0000A3F0 2B FD F7 10 FE 9B BF 10 20 0D A1 10 4D 43 36 38 +....... ...MC68
0000A400 6E 8B 11 32 48 E7 9E FF FF 41 FA 02 30 4F EF FE n..2H....A..0O..
0000A410 80 24 4F 20 18 47 E8 00 0A 4B FA FF EA 4D F5 7C .$O .G...K...M.|
0000A420 C6 0D 49 F3 05 00 70 00 10 2B FF FE 41 F6 E1 0F ..I...p..+..A...
0000A430 08 08 D3 5D 70 67 02 52 48 22 48 B1 CC 63 2C 20 ...]pg.RH"H..c, 
0000A440 0C A0 0F 04 52 4C DE 11 04 1B B9 60 49 EC FF E0 ....RL.....`I...
0000A450 4C D4 00 FF 48 E0 FF 00 B9 CB 62 F0 97 CC D7 C8 L...H.....b.....
0000A460 63 00 12 2F 09 03 30 3C 00 38 22 D8 0B 01 51 C8 c../..0<.8"...Q.
0000A470 FF 8E FA 4E 75 7E 00 1C 2B 00 01 E1 5E 1C 13 72 ...Nu~..+...^..r
0000A480 02 61 3C 1C A6 20 4A 05 07 D0 41 EA 00 80 30 07 .a<.. J...A...0.
0000A490 31 EA 01 00 E3 07 C0 80 03 88 53 40 38 00 21 60 1.........S@8.!`
0000A4A0 1C 19 81 8D 61 42 44 80 43 94 9D FF 1F 1B 36 1A ....aBD.C.....6.
0000A4B0 D9 01 8E 4D FC 30 3B 2A 25 6B 1A 1A 41 DB 0D 10 ...M.0;*%k..A...
0000A4C0 0B 57 9E 58 10 13 EF A8 72 01 EF 69 53 41 CC 81 .W.X....r..iSA..
0000A4D0 8C 80 51 CC FF C0 BB CE 65 9A 60 4F B8 E0 F4 30 ..Q.....e.`O...0
0000A4E0 18 C0 46 90 58 66 F8 12 28 00 3C 9E 01 6C 7D 34 ..F.Xf..(.<..l}4
0000A4F0 E2 AE 10 C6 0B 3D B0 9F 0C 02 6D 16 53 AA 00 19 .....=....m.S...
0000A500 14 00 30 0F 3E A8 25 1D 16 C6 1D 05 C0 AF 09 70 ..0.>.%........p
0000A510 FF 72 10 9B 11 04 11 00 0F DE 01 EE AE 48 46 58 .r...........HFX
0000A520 1C 4B 1C 23 C3 60 23 09 92 07 7E 10 D8 1D 17 72 .K.#.`#...~....r
0000A530 03 02 E1 88 10 18 51 C9 00 E3 70 1F 72 05 61 70 ......Q...p.r.ap
0000A540 CA 99 7C 34 5C 00 F1 36 00 4F EF FF F0 22 4F 70 ..|4\..6.O..."Op
0000A550 0F 72 04 61 B6 12 C0 51 CA FF F6 70 01 E2 98 A3 .r.a...Q...p....
0000A560 74 00 76 48 E7 07 00 38 03 43 EF 00 0C B2 19 66 t.vH...8.C.....f
0000A570 3A 7A 01 E3 6D 53 45 30 C5 2A 02 48 45 3E 01 53 :z..mSE0.*.HE>.S
0000A580 47 E3 55 E2 56 51 CF C0 4F 7A 10 9A 01 EA 06 6E G.U.VQ..Oz.....n
0000A590 30 C6 11 41 B7 03 1A 03 9A 04 11 45 30 B3 7C 1F 0..A.......E0.|.
0000A5A0 05 EB 6E 0C 53 46 31 46 AF D4 A0 E5 E2 88 52 30 ..n.SF1F......R0
0000A5B0 01 B2 C9 11 66 AE 0C 4C DF 00 E0 73 00 60 10 87 ....f..L...s.`..
0000A5C0 20 3A 00 4C 31 9B C0 0B 01 80 D8 89 3A 70 11 0A  :.L1.......:p..
0000A5D0 3F 20 B9 27 0B 83 08 08 90 8E DB 08 07 26 4E D7 ? .'.........&N.
0000A5E0 18 1D 2E 81 33 FF FF C4 2F 42 1E 00 1F 2F 4D 00 ....3.../B.../M.
0000A5F0 52 4C EF 0E 7F FF 00 16 34 45 52 15 5E 5D 51 3E RL......4ER.^]Q>
0000A600 3F 13 61 3E 87 3F 3C C3 61 3F 3A E1 11 3F B2 C6 ?.a>.?<.a?:..?..
0000A610 03 AA E6 A8 45 4E D6 13 45 CE 87 45 C6 1F 03 8E ....EN..E..E....
0000A620 05 0F 45 48 03 87 45 3C C3 03 45 30 B0 AB 20 1A ..EH..E<..E0.. .
0000A630 DB 37 BB 2D D7 07 11 F8 E2 5D C3 0F 4B BA A1 4B .7.-.....]..K..K
0000A640 94 F1 F7 4B F1 40 4B 2F 93 E4 02 90 87 4C 8E 43 ...K.@K/.....L.C
0000A650 4C 00 E0 0E 4C 22 4B 00 61 00 01 DE 72 FF B4 6B L...L"K.a...r..k
0000A660 CC 50 66 31 3A F8 47 5C 7C 42 86 1F 5C 01 2A EB .Pf1:.G\|B..\.*.
0000A670 02 5C 4A 76 00 E1 22 03 74 07 E2 49 64 04 0A 41 .\Jv..".t..Id..A
0000A680 A0 01 00 7B 30 C1 52 03 30 66 EA 7B 12 19 B3 B8 ...{0.R.0f.{....
0000A690 66 02 32 02 02 42 00 FF D4 42 34 32 20 32 49 0F f.2..B...B42 2I.
0000A6A0 0A 53 80 66 E8 81 92 58 9B C0 70 22 4D 61 C0 72 .S.f...X..p"Ma.r
0000A6B0 FE B4 7A 00 54 66 1E C7 4D 02 00 C6 14 9E 1D F8 ..z.Tf..M.......
0000A6C0 20 9E 9C 27 9D 84 0F 9D 82 17 87 9D D2 C3 01 9D  ..'............
0000A6D0 2E E1 47 9D 40 FF 7A 3A F8 F7 91 F8 79 91 2D 53 ..G.@.z:....y.-S
0000A6E0 54 48 B9 BC 60 1A E3 0F 7C F8 0C FF D3 42 87 AA TH..`...|....B..
0000A6F0 9E FD 61 95 46 F4 F7 FF D5 72 D2 89 BA F8 89 24 ..a.F....r.....$
0000A700 6F 00 0C 44 22 0D 30 0F A4 00 66 2E 20 4D 43 FA o..D".0...f. MC.
0000A710 00 8C 38 D1 D9 01 E9 00 00 04 2C 48 20 18 67 1A ..8.......,H .g.
0000A720 ED 5B D8 F3 D3 91 C1 C1 67 0E D3 C0 D8 11 01 66 .[......g......f
0000A730 30 F2 43 1D FD 60 EE EC C3 60 37 25 58 A1 1C 03 0.C..`...`7%X...
0000A740 14 03 30 1C 25 85 08 D2 AA E6 0F 07 71 10 07 14 ..0.%.......q...
0000A750 C7 07 18 1F 23 3C 03 08 B6 4F 8F B6 04 F8 E3 3F ....#<...O.....?
0000A760 00 3F C3 51 4C 4E 41 E3 12 B8 E2 06 9F 22 C6 76 .?.QLNA......".v
0000A770 3E 41 C8 1B 1F C7 AA 61 0F C7 47 F7 B3 5D E3 F7 >A.....a..G..]..
0000A780 D3 E2 39 D3 54 15 2B 03 1A 0F D4 8C 15 87 D4 FC ..9.T.+.........
0000A790 D8 21 E5 94 F8 47 E4 7D 50 F7 9F E5 B4 43 53 C6 .!...G.}P....CS.
0000A7A0 E8 03 53 11 C2 66 78 47 35 CD 6E 26 A8 AD F9 2C ..S..fxG5.n&...,
0000A7B0 26 9C BE 25 0E 1B 5F 25 F0 21 6B 6D 88 E5 47 25 &..%.._%.!km..G%
0000A7C0 4E F6 F7 FC 6D F7 FC 19 6A 9F 19 41 4D 01 E3 6B N...m...j..AM..k
0000A7D0 C8 1C 0F 03 E9 14 07 AF 48 7A 02 B8 4B 09 FE 0D ........Hz..K...
0000A7E0 07 FF F2 20 4D 81 19 00 72 01 2C 78 80 BF 4E AE ... M...r.,x..N.
0000A7F0 FF 3A 72 FC 18 4A 80 67 11 90 24 07 40 2A 57 4A .:r..J.g..$.@*WJ
0000A800 95 01 09 62 2A 55 DB CD 80 01 2E 8D 58 4D 41 ED ...b*U......XMA.
0000A810 C6 AF 0C 98 66 64 43 D1 E2 A7 3B E4 07 37 CA 20 ....fdC...;..7. 
0000A820 01 2B FF FA 72 FF B0 42 D2 39 3A 33 48 40 AB 2F .+..r..B.9:3H@./
0000A830 3E 0F F2 03 7E 41 0D 7E 31 0C 43 2F 28 4D D9 D4 >...~A.~1.C/(M..
0000A840 E5 07 1F C2 52 1F E0 9C A7 1F D8 2E 1F D0 3E 03 ....R.........>.
0000A850 A4 05 5F 1F 5E 03 2F 1F 52 03 94 1F 46 BB 3F 39 .._.^./.R...F.?9
0000A860 1F 6F 02 9D 05 91 25 A9 53 81 FD 04 3F 2B 20 1F .o....%.S...?+ .
0000A870 67 F7 5B F8 66 F9 5F CD 38 01 6C DD 22 0F 60 00 g.[.f._.8.l.".`.
0000A880 FE EC 56 9A 3B 94 2F D6 48 2F 36 00 38 F9 04 2F ..V.;./.H/6.8../
0000A890 74 C2 88 2F CB FF F6 9B 27 49 33 36 04 F2 25 2F t../....'I36..%/
0000A8A0 04 3E 9A 03 0B 5F 2F CB 44 27 2F 0C 94 40 61 EC .>..._/.D'/..@a.
0000A8B0 66 2C 03 42 9C 22 14 67 26 18 05 20 14 03 41 FA f,.B.".g&.. ..A.
0000A8C0 03 FD 8E 20 50 D1 C8 47 01 C2 B5 24 08 58 82 00 ... P..G...$.X..
0000A8D0 15 D5 B5 08 00 53 04 81 66 F4 60 D6 4D 65 C6 FD .....S..f.`.Me..
0000A8E0 6C E5 88 58 80 2F 40 00 40 22 2F 00 04 2F 01 50 l..X./@.@"/../.P
0000A8F0 93 22 4E 4A 01 93 2E 22 1F 58 4F 10 FD 7F 44 FF ."NJ...".XO...D.
0000A900 11 E1 03 F2 13 D3 B5 E8 7C 38 D3 74 9C 3E D3 6A ........|8.t.>.j
0000A910 04 0F D3 01 80 09 87 D3 42 C3 01 D3 14 E3 1E D3 ........B.......
0000A920 E2 43 BD 2E C7 B8 9D 29 03 D8 C2 C3 D0 70 C3 C8 .C.....).....p..
0000A930 E3 03 9C E1 05 C3 56 F0 03 C3 4A F8 03 C3 74 3E ......V...J...t>
0000A940 3E 19 26 07 9F 03 12 87 C9 B4 42 C9 8E 23 C9 86 >.&.......B..#..
0000A950 5F EC 60 A2 7C 04 DE 7C BB 0F 3E 85 DC 19 1F 85 _.`.|..|..>.....
0000A960 BA 0A 07 85 01 80 FE 00 10 3E 85 01 27 87 74 5C .........>..'.t\
0000A970 D4 E1 88 B2 D0 88 C4 FB 17 BE 5C 9C 01 1F 88 6E ..........\....n
0000A980 23 77 5C D6 DD 03 5C 46 F7 47 5C 45 CE B3 8A 84 #w\...\F.G\E....
0000A990 9E E6 E1 9E DE 38 9E 45 D6 BF BE 05 0F 9E 64 03 .....8.E......d.
0000A9A0 87 9E 58 C3 03 9E 4C E3 2E 9E EE 0D 68 E0 FB F7 ..X...L.....h...
0000A9B0 FD 68 2C DF 68 82 19 0F E2 60 2A BC 68 A1 DE 70 .h,.h....`*.h..p
0000A9C0 38 DE 74 99 3E DE 5E 17 EF 67 36 01 87 DE 08 DD 8.t.>.^..g6.....
0000A9D0 69 67 B4 50 C2 D2 9C 27 C2 CA 0E C2 C2 3E 03 96 ig.P...'.....>..
0000A9E0 05 1F C2 50 03 0F C2 44 03 87 C2 38 D1 28 25 3B ...P...D...8.(%;
0000A9F0 BE 5B 10 F5 EF 5B E8 19 87 78 C6 DC 28 5B 43 4F .[...[...x..([CO
0000AA00 15 4D 5F 32 00 7B 01 46 BE 4A 02 33 FC E8 31 05 .M_2.{.F.J.3..1.
0000AA10 00 0A 8B C8 E8 34 00 8B D0 0C 03 C6 05 06 06 F8 .....4..........
0000AA20 B8 62 83 C6 04 AD 32 E4 8B EE 8B F7 03 C1 0D 3B .b....2........;
0000AA30 FE E7 76 0D FD 57 4E 4F 8B CA F3 A4 47 8B EF 5E ..v..WNO....G..^
0000AA40 FC 8B FE 56 BE 4C 01 B9 FA 00 86 0F C3 E8 02 2F ...V.L........./
0000AA50 2E D8 AD 18 86 E0 C3 8B F5 F9 AC 12 C0 02 C0 BF ................
0000AA60 4C E9 8B 70 00 0A EB 31 82 04 32 08 04 33 20 04 L..p...1..2..3 .
0000AA70 3B 04 06 35 B1 04 22 C7 74 F5 12 FF E2 F8 B1 03 ;..5..".t.......
0000AA80 02 CF 02 C9 F3 A5 EB 5E 87 18 72 6E 07 16 CA 12 .......^..rn....
0000AA90 C9 07 05 C9 73 0D 06 05 C8 FE C9 0D 07 80 F9 09 ....s...........
0000AAA0 74 CE 06 0C C0 73 21 05 39 75 03 27 A6 38 08 00 t....s!.9u.'.8..
0000AAB0 72 57 0A FF 75 0B FE C7 F0 01 17 8A 1C 46 80 B0 rW..u........F..
0000AAC0 4E 2B F3 06 FA F3 26 A4 FB 8E D0 6B 26 31 0C A4 N+....&....k&1..
0000AAD0 21 72 05 C0 04 73 F6 74 EF 1C B1 02 2A 3F 0E 74 !r...s.t....*?.t
0000AAE0 8F 73 92 03 05 2D 73 CC FE C1 82 07 2A 73 9D E1 .s...-s.....*s..
0000AAF0 8A 0C 46 0A C9 74 26 80 C1 08 EB 91 E1 01 50 80 ..F..t&.......P.
0000AB00 CF 04 60 6B A3 EB 98 82 BC CE 0B 04 D1 0C 14 A7 ..`k............
0000AB10 BE E9 33 0A C0 FF E6 90 D2 51 E8 AD 01 A8 BE 76 ..3......Q.....v
0000AB20 AC 52 4F 3E 3F 52 36 02 03 52 E8 2A 00 53 50 F0 .RO>?R6..R.*.SP.
0000AB30 18 54 4E 01 B9 3F 5A 01 06 60 54 1F 5A E8 FE 00 .TN..?Z..`T.Z...
0000AB40 FC 02 5A 8A FC 0C 5A 3A FC 10 5A 5D FC 08 5A 0C ..Z...Z:..Z]..Z.
0000AB50 8C 5A 49 96 59 71 CF 0B C1 F9 3F 59 B9 BF B1 03 .ZI.Yq....?Y....
0000AB60 07 5A 2C 73 CB 41 E7 05 59 83 E7 17 59 CF E0 09 .Z,s.A..Y...Y...
0000AB70 59 8B CF 2B CE C4 F9 04 00 62 61 56 81 E9 EC 00 Y..+.....baV....
0000AB80 02 8B FC 51 33 DB 8B C3 B9 08 00 D1 E8 73 03 35 ...Q3........s.5
0000AB90 01 A0 E2 F7 AB FE C3 75 ED 59 80 18 AC 32 D8 8A .......u.Y...2..
0000ABA0 C7 32 06 FF D1 E3 8B 19 09 0C E2 F1 81 C4 2D 5E .2............-^
0000ABB0 2E 3B DA 38 74 1A E8 00 00 5A 83 C2 0D B4 09 CD .;.8t....Z......
0000ABC0 21 B8 FF 4C 04 42 61 00 64 20 43 52 43 0D 0A 24 !..L.Ba.d CRC..$
0000ABD0 4B C3 4A B4 C0 EB 01 6A BE 6E F9 34 07 3E 1E 01 K.J....j.n.4.>..
0000ABE0 0C 44 07 B9 10 00 C1 CF 13 D2 E2 F5 19 BA 9B 59 .D.............Y
0000ABF0 BA 3C 04 04 3D 10 04 3E 04 7E 45 0D BA 07 A5 26 .<..=..>.~E....&
0000AC00 30 55 FF 18 03 FE 39 D1 CA 23 EB 63 38 14 7A 21 0U....9..#.c8.z!
0000AC10 BF AC C5 BE AC C5 BD 9E C5 71 C4 0B B6 F9 0B C5 .........q......
0000AC20 63 F9 19 C5 0C 02 26 18 A4 1B 6A 64 B3 CB 0B C1 c.....&...jd....
0000AC30 01 0A 73 EA 74 E3 6C D1 83 2B 73 86 25 D1 BF 9F ..s.t.l..+s.%...
0000AC40 D1 91 03 9F D1 85 0C 87 D1 97 EB 8C E7 0A D1 A1 ................
0000AC50 CC 05 2B E9 DA 0F 01 CC BE D0 36 97 2C 7E C9 0B ..+.......6.,~..
0000AC60 2C 23 01 FB F7 7F F7 10 7F C8 57 51 45 58 45 B5 ,#........WQEXE.
0000AC70 C2 05 02 69 4D 5A 3B 6C 14 F9 03 02 00 0C EF 03 ...iMZ;l........
0000AC80 0B 04 00 00 8C D3 8E C3 8C CA 8E 18 DA 8B 0E 88 ................
0000AC90 8B F1 31 83 EE 97 FE D1 72 E9 FD F3 A5 53 B8 2E ..1.....r....S..
0000ACA0 00 50 8B 2E 0A 00 CB B8 00 10 3B C5 76 C0 17 C5 .P........;.v...
0000ACB0 2B E8 2B D0 36 2B D8 2A 30 2E B1 03 18 D3 E0 8B +.+.6+.*0.......
0000ACC0 C8 D1 E0 48 48 8B F0 8B F8 2C 0B ED 00 75 D9 FC ...HH....,...u..
0000ACD0 8E C2 8E DB BE C0 E0 E8 E0 01 50 53 3A E8 DB 00 ..........PS:...
0000ACE0 53 8B D3 E8 D4 01 53 31 BE 12 12 63 01 C1 05 33 S.....S1...c...3
0000ACF0 FF 06 28 1B FB D5 80 45 50 8B DF 83 E7 0F 81 1A ..(....EP.......
0000AD00 C7 00 80 CA E3 D3 EB 8C C0 03 C3 2D 00 08 8E C0 ...........-....
0000AD10 8B DE 83 E6 0F C6 0F D8 0F 16 8E D8 58 0B 39 82 ............X.9.
0000AD20 1F 58 9E 5A 59 33 F6 E8 43 00 8C DA 0E 1F 32 ED .X.ZY3..C.....2.
0000AD30 BE 49 02 AC 8A C8 E3 13 AD 03 C2 32 2E 33 FF 78 .I.........2.3.x
0000AD40 32 E4 AC 03 F8 26 01 15 E2 F8 EB E8 8B 36 04 00 2....&.......6..
0000AD50 8B 3E 06 00 03 FA 01 16 A7 83 EA 76 10 6E 30 DA .>.........v.n0.
0000AD60 33 24 DB FA 8B B8 7C E6 8E D7 FB 2E FF 2F 50 1E 3$....|....../P.
0000AD70 06 16 07 17 0F 59 E3 01 42 01 84 5C 26 40 5D 83 .....Y..B..\&@].
0000AD80 FE 00 31 75 07 95 80 C4 80 4F D8 E2 E4 4A 75 E1 ..1u.....O...Ju.
0000AD90 88 6C 60 07 96 3B C3 74 18 C3 91 BA 35 02 E3 0C .l`..;.t....5...
0000ADA0 6C E6 01 F3 A2 75 55 4D 7C D3 73 7C 74 F7 7C 5B l....uUM|.s|t.|[
0000ADB0 0F 2A 5B 5A 0F 51 B3 01 32 8A 51 D3 DE FB A5 FB .*[Z.Q..2.Q.....
0000ADC0 F7 7E DF F7 06 57 FB EC 54 F0 24 04 73 09 03 E9 .~...W..T.$.s...
0000ADD0 79 FF 43 07 DB E2 39 07 7D 5D 7F 91 76 4E 5D 08 y.C...9.}]..vN].
0000ADE0 02 E9 5D 03 02 5D 5D 4B FC A7 5D 8B 01 C7 F7 21 ..]..]]K..]....!
0000ADF0 C7 38 21 F5 08 85 71 FD 87 85 7F 5D 16 49 85 53 .8!...q....].I.S
0000AE00 54 F4 84 02 82 3A 60 1A 1F 74 76 0C 01 89 FF FF T....:`..tv.....
0000AE10 48 E7 86 03 41 FA 02 3F 00 18 47 E8 00 0A 4B FA H...A..?..G...K.
0000AE20 FF 1B F0 4D F5 9C 18 0D 49 F3 05 70 00 03 10 2B ...M....I..p...+
0000AE30 FF FE 41 F6 87 0F 08 08 2E 2D 14 67 02 52 48 22 ..A......-.g.RH"
0000AE40 48 B1 CC 63 2C 20 0C 0F 1B 04 52 4C 11 C0 04 1B H..c, ....RL....
0000AE50 49 EC FF E0 4C 6B D4 56 86 48 E0 FF 00 B9 CB 62 I...Lk.V.H.....b
0000AE60 F0 97 CC D7 C8 5D 00 00 12 2F 09 30 3C 00 3F 31 .....].../.0<.?1
0000AE70 22 D8 01 51 C8 70 FF FA 4E 75 7E 80 DE 07 1E 1B "..Q.p..Nu~.....
0000AE80 DF 07 CC 05 60 3A BA 81 09 60 30 47 05 44 94 03 ....`:...`0G.D..
0000AE90 05 46 05 78 4C 09 05 52 7A 6B 03 37 84 67 CA DD .F.xL..Rzk.7.g..
0000AEA0 46 51 CD FF F8 54 46 1A DB 80 01 51 CE FF F6 60 FQ...TF....Q...`
0000AEB0 70 6C 19 B6 DB 40 45 05 64 0E 41 53 07 B4 99 0D pl...@E.d.AS....
0000AEC0 BA 7B 09 B8 2F 0B AE 21 64 1A 05 A0 39 05 65 02 .{../..!d...9.e.
0000AED0 6E 4A 46 66 08 52 0D 0B A8 11 71 E1 5E 1C 1B 20 nJFf.R....q.^.. 
0000AEE0 4D 90 C6 53 48 E2 4D 64 02 1A D8 DC 3B 1D 06 0C M..SH.Md....;...
0000AEF0 1C 10 1A C6 01 E0 67 FA 60 12 DA 13 01 88 09 08 ......g.`.......
0000AF00 1B 85 65 0E 71 10 39 65 06 05 00 64 F4 67 EC 7A ..e.q.9e...d.g.z
0000AF10 02 7C 00 E1 49 2C 64 9C FC 7E 07 08 2A 64 B0 52 .|..I,d..~..*d.R
0000AF20 00 7D 28 64 88 1A 1B 67 34 1C 50 45 60 F1 33 67 .}(d...g4.PE`.3g
0000AF30 20 67 00 38 01 04 09 1C 0A 65 8E 60 86 28 CD CE  g.8.....e.`.(..
0000AF40 05 D0 A2 05 D2 8A 05 DA 31 05 DE 25 66 04 40 69 ........1..%f.@i
0000AF50 98 60 0C 72 01 03 E1 88 10 18 51 C9 19 3B 20 70 .`.r......Q..; p
0000AF60 3A 00 B6 9B C0 24 6F 00 44 22 0D 30 C3 0B A4 66 :....$o.D".0...f
0000AF70 2E 01 B7 43 FA 00 8C D1 D9 C6 01 E9 5D 00 2C 48 ...C........].,H
0000AF80 20 18 67 1A 43 F5 8C BF D3 60 91 35 67 0E D3 C0  .g.C....`.5g...
0000AF90 01 B0 3C 00 01 66 F2 43 87 1D FD 60 EE 60 8F 60 ..<..f.C...`.`.`
0000AFA0 25 58 00 71 0C 03 14 C0 03 1C 25 41 00 1C 08 D2 %X.q......%A....
0000AFB0 AA 0F CE 07 10 38 07 14 07 18 E0 23 3C 70 0A 3B .....8.....#<p.;
0000AFC0 3F 20 39 AD FC 0B 72 4F 20 08 90 8E 4E D7 4F EF ? 9...rO ...N.O.
0000AFD0 01 80 2E 81 4C DF FF FF 3F 00 3F C0 51 4C 4E 41 ....L...?.?.QLNA
0000AFE0 42 1E 80 21 2F 4D 00 01 52 4C EF 7F FF 00 16 B6 B..!/M..RL......
0000AFF0 21 09 A1 F8 0A 65 88 8C 97 CF 32 02 B8 1B 87 8D !....e....2.....
0000B000 9A C3 5B 8D 46 E0 0C 8D 70 0F B6 75 38 D9 44 1B ..[.F...p..u8.D.
0000B010 F4 E8 9D 7C D2 03 3C 9D 56 03 05 58 A3 05 5E CF ...|..<.V..X..^.
0000B020 09 05 64 06 80 9D 10 1B B9 00 3D 1A C0 0A 05 D0 ..d.......=.....
0000B030 AD E6 0E E2 5C 60 72 2A 2B A4 13 AF A4 84 AF A2 ....\`r*+.......
0000B040 E1 AF B8 C5 0B 9C 42 AF 9C A0 AF 9C 65 7C 7E 01 ......B.....e|~.
0000B050 3E AF 96 1B 1F AF 18 01 0A AF 0E 84 45 1A 9B 77 >...........E..w
0000B060 73 A1 B5 0C E0 02 0B 64 E8 67 E0 9C 3A BB 34 20 s......d.g..:.4 
0000B070 BB 8F 32 2A 64 A4 1C BB 30 0F 74 7C 3A BD 3A 24 ..2*d...0.t|:.:$
0000B080 BD 19 7C 24 01 1D BF 20 65 1C E7 15 51 74 F3 C6 ..|$... e...Qt..
0000B090 45 05 C8 14 05 CA 05 4A D6 3E C9 01 1F C3 8A BF E......J.>......
0000B0A0 12 C3 F9 EE 03 12 50 C4 86 F8 15 C4 7C F4 0C 0C ......P.....|...
0000B0B0 C4 22 4B 61 C5 D8 0C 72 FF B4 6B 72 66 1F 00 02 ."Ka...r..krf...
0000B0C0 92 47 0F D4 4F F7 8F D4 68 87 D4 4A C5 04 D4 DF .G..O...h..J....
0000B0D0 4F EF FE 00 20 4F 76 00 22 03 74 07 E2 49 64 04 O... Ov.".t..Id.
0000B0E0 0A 41 A0 01 51 CA FF F6 30 C1 52 03 66 EA 74 00 .A..Q...0.R.f.t.
0000B0F0 12 19 B3 02 32 02 02 42 00 60 D4 42 34 37 20 00 ....2..B.`.B47 .
0000B100 E0 60 49 0F 53 80 66 E8 C5 37 02 00 48 12 C2 9B .`I.S.f..7..H...
0000B110 C0 B8 7C 22 4D 61 B8 72 FE B4 7A 00 BE 66 74 6E ..|"Ma.r..z..ftn
0000B120 EF 1E 8B 32 99 1E DA 0C DC F9 1B 1D 7C BE 11 BE ...2........|...
0000B130 1D A2 01 5F 1D 5C 47 2F 1D 49 F7 A3 80 E8 32 80 ..._.\G/.I....2.
0000B140 F8 F7 E7 F8 04 BF C4 A0 36 38 ED 59 25 01 F8 07 ........68.Y%...
0000B150 CC 01 F6 D0 5B AF 32 F8 F7 BC FB 42 47 3D 48 9B ....[.2....BG=H.
0000B160 C0 67 55 3A C2 03 14 3F 21 14 DB 9F 07 26 04 46 .gU:...?!....&.F
0000B170 1F 2B F8 12 12 94 4E 04 C4 D1 2E EA D2 2C F8 5B .+....N......,.[
0000B180 05 7F 38 F7 FE E0 78 CF B5 42 89 3B FD 2B 88 38 ..8...x..B.;.+.8
0000B190 3C 7D 86 1B 8F 2D 38 47 87 4C 42 C7 F7 4C D8 B6 <}...-8G.LB..L..
0000B1A0 2D 54 FF 03 8F 45 50 66 1A 14 8B 96 77 EB E2 1A -T...EPf....w...
0000B1B0 96 70 95 52 9C 3E 95 50 1B 97 A5 02 C3 47 95 3B .p.R.>.P.....G.;
0000B1C0 FB F7 E8 F4 80 FC A5 47 1F 5F 41 4D 02 56 F4 70 .......G._AM.V.p
0000B1D0 0E 38 03 E9 13 99 0E 48 7A 02 22 86 6A FE 07 2E .8.....Hz.".j...
0000B1E0 FF F2 18 2A 57 4A 95 67 00 01 FA 2A 55 DB CD 01 ...*WJ.g...*U...
0000B1F0 2E 8D 00 58 4D 41 ED 00 08 0C 98 C1 3F 43 02 66 ...XMA......?C.f
0000B200 E2 28 88 F8 07 84 18 E8 20 2B EA 72 FF 18 B0 42 .(...... +.r...B
0000B210 66 0B D2 48 0C 40 3F 00 2F 0F F2 F8 03 8E F8 0D f..H.@?./.......
0000B220 7E F8 0C 7C 08 28 4D D9 D4 89 6C 6F 4A 0D BA 04 ~..|.(M...loJ...
0000B230 AA B2 6E 06 A1 54 F4 28 54 C0 0B DA 46 DA 45 53 ..n..T.(T...F.ES
0000B240 06 45 1A DB B9 2D FC 93 05 8E 17 F2 AE D4 2F 59 .E...-......../Y
0000B250 8C 41 11 66 20 09 64 12 53 7A 45 02 15 A1 6E B0 .A.f .d.SzE...n.
0000B260 E5 01 1B 26 91 09 13 76 D1 D2 51 EE 0C 52 46 EF ...&...v..Q..RF.
0000B270 02 19 05 84 7E 08 47 7E C2 01 6E 1A 83 45 0C 60 ....~.G~..n..E.`
0000B280 18 C9 01 3D 0E 2A 91 AB E2 F4 56 E2 6C 6D FE 62 ...=.*....V.lm.b
0000B290 F0 01 0B AC 52 45 F3 03 17 70 A1 88 24 55 EE 7A ....RE...p..$U.z
0000B2A0 66 02 77 51 8E 36 57 19 71 7A 1D 6C E1 01 0F 8E f.wQ.6W.qz.l....
0000B2B0 20 1F 50 2A 58 E8 2A 07 5F 66 48 0C 94 60 DD EC  .P*X.*._fH..`..
0000B2C0 66 00 FE 03 30 42 9C 22 14 67 31 07 28 07 20 14 f...0B.".g1.(. .
0000B2D0 80 03 41 FA FE 0E 20 50 37 D1 C8 01 60 51 F8 24 ..A... P7...`Q.$
0000B2E0 08 58 42 82 15 E7 D5 B5 08 00 53 81 66 F4 60 D4 .XB.......S.f.`.
0000B2F0 20 3A FD EE E5 88 58 80 2F 40 00 40 22 2F 88 6F  :....X./@.@"/.o
0000B300 58 4F 83 54 7F FF E3 38 CA 88 8D F2 89 7C 5A B2 XO.T...8.....|Z.
0000B310 F0 9C 03 7A 79 48 7A 01 E0 E1 07 7A B8 F1 1E 7A ...zyHz....z...z
0000B320 F1 40 64 D0 24 A0 F8 14 54 EA C6 AB D8 EF 7E 49 .@d.$...T.....~I
0000B330 1D 1F 4A BA 15 0F 4A 78 23 84 4A 14 E1 4A 12 E5 ..J...Jx#.J..J..
0000B340 01 3D 08 54 BF FA 67 7C E6 07 3C 44 68 01 0B B2 .=.T..g|..<Dh...
0000B350 F8 05 44 74 76 2A 44 22 1F 44 6C 0F 08 44 80 FF ..Dtv*D".Dl..D..
0000B360 39 91 01 0D 96 1F 42 01 09 38 72 C3 38 6A E1 01 9.....B..8r.8j..
0000B370 38 50 F0 16 38 FE 7C 30 0E 5B 38 4E FA A9 76 50 8P..8.|0.[8N..vP
0000B380 E7 FB 91 55 76 00 F9 07 FB D8 F5 23 76 7D C6 03 ...Uv......#v}..
0000B390 5F 76 B0 44 5F 76 F7 8F 11 09 8F 11 0B AA 54 52 _v.D_v........TR
0000B3A0 70 1B 4A F8 01 1B 7C 30 17 3F 1B 10 00 F5 4E 54 p.J...|0.?....NT
0000B3B0 B4 4F 5B C4 04 14 E1 5B 82 D0 5B 02 F8 07 5B 7D .O[....[..[...[}
0000B3C0 DA 66 7F 57 F7 BF BC 2B DF BC 01 84 5D 50 E1 5D .f.W...+....]P.]
0000B3D0 48 F0 01 5D 2E F8 17 5D 7D 0E 10 62 79 3C 1F 00 H..]...]}..by<..
0000B3E0 00 00 00 28 00 00 00 00 00 00 00 00 00 00 00 00 ...(............
0000B3F0 00 00 00 00 00 00 00 00 00 00 80 00 00 00 B4 1E ................
0000B400 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000B410 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000B420 B4 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 .@..............
0000B430 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000B440 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000B450 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000B460 00 00 00 00 00 00 80 00 1F 1C 1F 1E 1F 1E 1F 1F ................
0000B470 1E 1F 1E 1F 00 00 04 00 1F 1C 1F 1E 1F 1E 1F 1F ................
0000B480 1E 1F 1E 1F 00 00 00 00 00 00 00 00 FF FF 00 00 ................
0000B490 00 0E 00 0E 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000B4A0 FF FF 00 00 00 04 00 04 00 00 00 00 00 00 91 26 ...............&
0000B4B0 00 00 B4 8C FF FF 00 00 00 04 00 04 00 00 00 00 ................
0000B4C0 00 00 91 3C 00 00 00 00 FF FF 00 00 00 0E 00 0E ...<............
0000B4D0 00 00 00 00 00 00 93 18 00 00 00 00 FF FF 00 00 ................
0000B4E0 00 04 00 04 00 00 00 00 00 00 00 00 00 00 B4 C8 ................
0000B4F0 FF FF 00 00 00 04 00 04 00 00 00 00 00 00 93 34 ...............4
0000B500 00 00 00 00 FF FF 00 00 00 04 00 04 00 00 00 00 ................
0000B510 00 00 93 3E 00 00 00 00 00 20 20 20 20 20 20 20 ...>.....       
0000B520 20 20 28 28 28 28 28 20 20 20 20 20 20 20 20 20   (((((         
0000B530 20 20 20 20 20 20 20 20 20 48 10 10 10 10 10 10          H......
0000B540 10 10 10 10 10 10 10 10 10 84 84 84 84 84 84 84 ................
0000B550 84 84 84 10 10 10 10 10 10 10 81 81 81 81 81 81 ................
0000B560 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 ................
0000B570 01 01 01 01 10 10 10 10 10 10 82 82 82 82 82 82 ................
0000B580 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 ................
0000B590 02 02 02 02 10 10 10 10 20 20 20 20 20 20 20 20 ........        
0000B5A0 20 20 28 28 28 28 28 20 20 20 20 20 20 20 20 20   (((((         
0000B5B0 20 20 20 20 20 20 20 20 20 48 10 10 10 10 10 10          H......
0000B5C0 10 10 10 10 10 10 10 10 10 84 84 84 84 84 84 84 ................
0000B5D0 84 84 84 10 10 10 10 10 10 10 81 81 81 81 81 81 ................
0000B5E0 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 ................
0000B5F0 01 01 01 01 10 10 10 10 10 10 82 82 82 82 82 82 ................
0000B600 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 ................
0000B610 02 02 02 02 10 10 10 10 20 00 00 00 00 00 02 00 ........ .......
