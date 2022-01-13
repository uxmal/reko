;;; Segment code (00001000)

;; fn00001000: 00001000
fn00001000 proc
	bra	$0000100A
00001002       56 42 43 43 20 30 2E 39                     VBCC 0.9      

l0000100A:
	move.l	d0,d2
	movea.l	a0,a2
	lea	$0000BD66,a4
	movea.l	$00000004,a6
	cmpi.w	#$0024,$0014(a6)
	bcc	$00001036

l00001020:
	lea	$00003D70,a0
	move.l	#$00000030,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0

l00001030:
	clr.l	(a0)+
	subq.l	#$01,d0
	bne	$00001030

l00001036:
	move.l	a7,-$7FF6(a4)
	move.l	a6,-$7FF2(a4)
	suba.l	a1,a1
	jsr.l	-$0126(a6)
	movea.l	d0,a3
	moveq	#$00,d0
	lea	000012BC,a1                                            ; $0274(pc)
	jsr.l	-$0228(a6)
	tst.l	d0
	beq	$00001226

l00001056:
	move.l	d0,-$7FEE(a4)
	move.l	$00AC(a3),d0
	bne	$00001066

l00001060:
	moveq	#$02,d4
	moveq	#$08,d2
	bra	$00001076

l00001066:
	movea.l	a2,a0
	moveq	#$03,d4

l0000106A:
	cmpi.b	#$20,(a0)
	bne	$00001072

l00001070:
	addq.w	#$01,d4

l00001072:
	tst.b	(a0)+
	bne	$0000106A

l00001076:
	move.l	d4,d0
	lsl.l	#$02,d0
	move.l	d0,d5
	add.l	d2,d0
	add.l	#$00000011,d0
	move.l	d0,d3
	move.l	#$00010001,d1
	jsr.l	-$00C6(a6)
	tst.l	d0
	beq	$0000123E

l00001096:
	move.l	d0,-(a7)
	movea.l	d0,a5
	move.l	d3,(a5)
	subq.l	#$01,d4
	move.l	d4,$000C(a5)
	lea	$0010(a5),a0
	adda.l	d5,a0
	move.l	a0,$0008(a5)
	clr.l	-(a7)
	move.l	$00AC(a3),d0
	beq	$00001190

l000010B6:
	lsl.l	#$02,d0
	movea.l	d0,a0
	move.l	$0010(a0),d0
	lsl.l	#$02,d0
	movea.l	$0008(a5),a1
	lea	$0010(a5),a6
	movea.l	d0,a0
	moveq	#$00,d0
	move.b	(a0)+,d0
	clr.b	(a0,d0)
	move.l	a0,(a6)+
	moveq	#$01,d3
	lea	(a2,d2),a0

l000010DA:
	cmpi.b	#$20,-(a0)
	dbhi	d2,$000010DA

l000010E2:
	clr.b	$0001(a0)

l000010E6:
	move.b	(a2)+,d1
	beq	$00001148

l000010EA:
	cmp.b	#$20,d1
	beq	$000010E6

l000010F0:
	cmp.b	#$09,d1
	beq	$000010E6

l000010F6:
	cmp.l	$000C(a5),d3
	beq	$00001148

l000010FC:
	move.l	a1,(a6)+
	addq.w	#$01,d3
	cmp.b	#$22,d1
	beq	$0000111A

l00001106:
	move.b	d1,(a1)+

l00001108:
	move.b	(a2)+,d1
	beq	$00001148

l0000110C:
	cmp.b	#$20,d1
	beq	$00001116

l00001112:
	move.b	d1,(a1)+
	bra	$00001108

l00001116:
	clr.b	(a1)+
	bra	$000010E6

l0000111A:
	move.b	(a2)+,d1
	beq	$00001148

l0000111E:
	cmp.b	#$22,d1
	beq	$00001116

l00001124:
	cmp.b	#$2A,d1
	bne	$00001144

l0000112A:
	move.b	(a2)+,d1
	move.b	d1,d2
	and.b	#$DF,d2
	cmp.b	#$4E,d2
	bne	$0000113C

l00001138:
	moveq	#$0A,d1
	bra	$00001144

l0000113C:
	cmp.b	#$45,d2
	bne	$00001144

l00001142:
	moveq	#$1B,d1

l00001144:
	move.b	d1,(a1)+
	bra	$0000111A

l00001148:
	clr.b	(a1)
	clr.l	(a6)
	pea	$0010(a5)
	move.l	d3,-(a7)
	movea.l	-$7FEE(a4),a6
	jsr.l	-$0036(a6)
	move.l	d0,-$7FE2(a4)
	jsr.l	-$003C(a6)
	move.l	d0,-$7FDE(a4)
	movea.l	-$7FF2(a4),a6
	cmpi.w	#$0024,$0014(a6)
	bcs	$0000117A

l00001172:
	move.l	$00E0(a3),-$7FDA(a4)
	bne	$0000117E

l0000117A:
	move.l	d0,-$7FDA(a4)

l0000117E:
	cmpi.b	#$0D,$0008(a3)
	bne	$000011F8

l00001186:
	movea.l	$00B0(a3),a0
	move.l	(a0)+,d0
	suba.l	d0,a0
	bra	$00001202

l00001190:
	bsr	$00001214
	move.l	d0,(a7)
	move.l	d0,-$7FEA(a4)
	move.l	d0,-$7FE6(a4)
	move.l	d0,-(a7)
	clr.l	-(a7)
	movea.l	-$7FEE(a4),a6
	movea.l	d0,a2
	move.l	$0024(a2),d0
	beq	$000011B6

l000011AE:
	movea.l	d0,a0
	move.l	(a0),d1
	jsr.l	-$007E(a6)

l000011B6:
	lea	000012C8,a0                                            ; $0112(pc)
	move.l	a0,d1
	move.l	#$000003ED,d2
	jsr.l	-$001E(a6)
	move.l	d0,$0004(a5)
	bne	$000011D2

l000011CC:
	moveq	#$14,d2
	bra	$0000127C

l000011D2:
	move.l	d0,-$7FE2(a4)
	move.l	d0,-$7FDE(a4)
	move.l	d0,-$7FDA(a4)
	move.l	d0,$009C(a3)
	move.l	d0,$00A0(a3)
	lsl.l	#$02,d0
	movea.l	d0,a0
	move.l	$0008(a0),d0
	beq	$000011F4

l000011F0:
	move.l	d0,$00A4(a3)

l000011F4:
	movea.l	-$7FF2(a4),a6

l000011F8:
	move.l	$003E(a3),d0
	movea.l	$003A(a3),a0
	sub.l	a0,d0

l00001202:
	move.l	a0,-$7FCE(a4)
	move.l	a7,-$7FCA(a4)
	jsr.l	$00001354
	moveq	#$00,d2
	bra	$0000127C

;; fn00001214: 00001214
;;   Called from:
;;     00001190 (in fn00001000)
;;     00001262 (in fn00001000)
fn00001214 proc
	lea	$005C(a3),a0
	jsr.l	-$0180(a6)
	lea	$005C(a3),a0
	jsr.l	-$0174(a6)
	rts

l00001226:
	movem.l	d7/a5-a6,-(a7)
	move.l	#$00038007,d7
	movea.l	$00000004,a6
	jsr.l	-$006C(a6)
	movem.l	(a7)+,d7/a5-a6
	bra	$0000125C

l0000123E:
	movea.l	-$7FEE(a4),a1
	jsr.l	-$019E(a6)
	movem.l	d7/a5-a6,-(a7)
	move.l	#$00010000,d7
	movea.l	$00000004,a6
	jsr.l	-$006C(a6)
	movem.l	(a7)+,d7/a5-a6

l0000125C:
	tst.l	$00AC(a3)
	bne	$00001268

l00001262:
	bsr	$00001214
	movea.l	d0,a2
	bsr	$0000126C

l00001268:
	moveq	#$14,d0
	rts

;; fn0000126C: 0000126C
;;   Called from:
;;     00001266 (in fn00001000)
;;     000012AE (in fn0000127C)
fn0000126C proc
	jsr.l	-$0084(a6)
	movea.l	a2,a1
	jsr.l	-$017A(a6)
	rts

;; fn00001278: 00001278
;;   Called from:
;;     0000130C (in fn000012D0)
fn00001278 proc
	move.l	$0004(a7),d2

;; fn0000127C: 0000127C
;;   Called from:
;;     000011CE (in fn00001000)
;;     00001212 (in fn00001000)
;;     00001278 (in fn00001278)
fn0000127C proc
	lea	$0000BD66,a4
	movea.l	-$7FF2(a4),a6
	movea.l	-$7FF6(a4),a7
	subq.l	#$08,a7
	movea.l	(a7)+,a2
	movea.l	(a7)+,a5
	move.l	$0004(a5),d1
	beq	$0000129E

l00001296:
	movea.l	-$7FEE(a4),a6
	jsr.l	-$0024(a6)

l0000129E:
	movea.l	-$7FF2(a4),a6
	movea.l	-$7FEE(a4),a1
	jsr.l	-$019E(a6)
	move.l	a2,d0
	beq	$000012B0

l000012AE:
	bsr	$0000126C

l000012B0:
	movea.l	a5,a1
	move.l	(a5),d0
	jsr.l	-$00D2(a6)
	move.l	d2,d0
	rts
000012BC                                     64 6F 73 2E             dos.
000012C0 6C 69 62 72 61 72 79 00 4E 49 4C 3A 00 00 00 00 library.NIL:....

;; fn000012D0: 000012D0
;;   Called from:
;;     0000134A (in fn0000131C)
fn000012D0 proc
	movem.l	d2/a2-a3,-(a7)
	lea	$00003EB8,a3
	move.l	#$00003EB8,d0
	beq	$00001308

l000012E2:
	moveq	#$01,d2
	tst.l	$0004(a3)
	beq	$000012F6

l000012EA:
	addq.l	#$01,d2
	move.l	d2,d0
	lsl.l	#$02,d0
	tst.l	(a3,d0)
	bne	$000012EA

l000012F6:
	subq.l	#$01,d2
	beq	$00001308

l000012FA:
	move.l	d2,d0
	lsl.l	#$02,d0
	movea.l	(a3,d0),a2
	jsr.l	(a2)
	subq.l	#$01,d2
	bne	$000012FA

l00001308:
	move.l	$0010(a7),-(a7)
	jsr.l	$00001278
	addq.w	#$04,a7
	movem.l	(a7)+,d2/a2-a3
	rts
0000131A                               00 00                       ..    

;; fn0000131C: 0000131C
;;   Called from:
;;     00001382 (in fn00001354)
;;     000023AA (in fn00002388)
fn0000131C proc
	movem.l	a2-a3,-(a7)
	tst.l	$00003DA0
	bne	$0000134E

l00001328:
	movea.l	$00003EC8,a3
	moveq	#$01,d0
	move.l	d0,$00003DA0
	move.l	a3,d0
	beq	$00001346

l0000133A:
	movea.l	$0004(a3),a2
	jsr.l	(a2)
	movea.l	(a3),a3
	move.l	a3,d0
	bne	$0000133A

l00001346:
	move.l	$000C(a7),-(a7)
	bsr	$000012D0
	addq.w	#$04,a7

l0000134E:
	movem.l	(a7)+,a2-a3
	rts

;; fn00001354: 00001354
;;   Called from:
;;     0000120A (in fn00001000)
fn00001354 proc
	movem.l	a2-a3,-(a7)
	lea	$00003EB0,a3
	move.l	#$00003EAC,d0
	beq	$00001372

l00001366:
	tst.l	(a3)
	beq	$00001372

l0000136A:
	movea.l	(a3)+,a2
	jsr.l	(a2)
	tst.l	(a3)
	bne	$0000136A

l00001372:
	move.l	$0010(a7),-(a7)
	move.l	$0010(a7),-(a7)
	jsr.l	$00001390
	move.l	d0,-(a7)
	bsr	$0000131C
	lea	$000C(a7),a7
	movem.l	(a7)+,a2-a3
	rts
0000138E                                           00 00               ..

;; fn00001390: 00001390
;;   Called from:
;;     0000137A (in fn00001354)
fn00001390 proc
	subq.w	#$08,a7
	lea	$0004(a7),a0
	move.l	a0,-(a7)
	pea	000013E4                                               ; $004C(pc)
	jsr.l	$00002B40
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	pea	000013E8                                               ; $0040(pc)
	jsr.l	$00002B40
	lea	$0014(a7),a0
	move.l	a0,-(a7)
	lea	$0014(a7),a0
	move.l	a0,-(a7)
	pea	000013EC                                               ; $002E(pc)
	jsr.l	$00002B40
	move.l	$0020(a7),-(a7)
	move.l	$0020(a7),-(a7)
	pea	000013F4                                               ; $0024(pc)
	jsr.l	$000013FC
	lea	$0028(a7),a7
	addq.w	#$08,a7
	rts
000013E2       00 00 25 64 00 00 25 64 00 00 25 64 20 25   ..%d..%d..%d %
000013F0 64 00 00 00 25 6C 64 20 25 6C 64 00             d...%ld %ld.    

;; fn000013FC: 000013FC
;;   Called from:
;;     000013D4 (in fn00001390)
fn000013FC proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00003ED0,-(a7)
	jsr.l	$00001418
	lea	$000C(a7),a7
	rts

;; fn00001418: 00001418
;;   Called from:
;;     0000140C (in fn000013FC)
fn00001418 proc
	lea	-$0044(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$007C(a7),d3
	movea.l	$0074(a7),a5
	movea.l	$0078(a7),a4
	moveq	#$00,d6
	tst.b	(a4)
	beq	$00001D5E

l00001434:
	cmpi.b	#$25,(a4)
	bne	$00001D38

l0000143C:
	clr.l	$0040(a7)
	moveq	#-$01,d5
	clr.l	$0048(a7)
	moveq	#$69,d4
	lea	$004C(a7),a3
	moveq	#$00,d7
	clr.l	$0066(a7)
	lea	$0001(a4),a2
	move.l	$0048(a7),d2

l0000145A:
	moveq	#$00,d1

l0000145C:
	lea	00001D70,a0                                            ; $0914(pc)
	move.l	d0,-(a7)
	move.b	(a0,d1),d0
	cmp.b	(a2),d0
	movem.l	(a7)+,d0
	bne	$00001480

l0000146E:
	move.l	d1,d0
	move.l	d1,-(a7)
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	move.l	(a7)+,d1
	or.l	d0,d2
	addq.l	#$01,a2
	bra	$0000148A

l00001480:
	addq.l	#$01,d1
	cmp.l	#$00000005,d1
	bcs	$0000145C

l0000148A:
	cmp.l	#$00000005,d1
	bcs	$0000145A

l00001492:
	move.l	d2,$0048(a7)
	cmpi.b	#$2A,(a2)
	bne	$000014D0

l0000149C:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	bge	$000014C8

l000014B4:
	ori.l	#$00000004,$0048(a7)
	move.l	$002C(a7),d0
	neg.l	d0
	move.l	d0,$0040(a7)
	bra	$0000153C

l000014C8:
	move.l	$002C(a7),$0040(a7)
	bra	$0000153C

l000014D0:
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$0000153C

l000014EC:
	move.l	$0040(a7),d2

l000014F0:
	moveq	#$0A,d0
	move.l	d3,-(a7)
	move.l	d0,d1
	move.l	d2,d3
	swap.l	d1
	swap.l	d3
	mulu.w	d2,d1
	mulu.w	d0,d3
	mulu.w	d2,d0
	add.w	d3,d1
	swap.l	d1
	clr.w	d1
	add.l	d1,d0
	move.l	(a7)+,d3
	move.b	(a2)+,d1
	ext.w	d1
	ext.l	d1
	sub.l	#$00000030,d1
	move.l	d1,d2
	add.l	d0,d2
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$000014F0

l00001538:
	move.l	d2,$0040(a7)

l0000153C:
	cmpi.b	#$2E,(a2)
	bne	$000015D0

l00001544:
	addq.l	#$01,a2
	cmpi.b	#$2A,(a2)
	bne	$0000156A

l0000154C:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	blt	$000015D0

l00001564:
	move.l	$002C(a7),d5
	bra	$000015D0

l0000156A:
	moveq	#$00,d5
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$000015D0

l00001588:
	moveq	#$0A,d0
	move.l	d2,-(a7)
	move.l	d0,d1
	move.l	d5,d2
	swap.l	d1
	swap.l	d2
	mulu.w	d5,d1
	mulu.w	d0,d2
	mulu.w	d5,d0
	add.w	d2,d1
	swap.l	d1
	clr.w	d1
	add.l	d1,d0
	move.l	(a7)+,d2
	move.b	(a2)+,d1
	ext.w	d1
	ext.l	d1
	sub.l	#$00000030,d1
	move.l	d1,d5
	add.l	d0,d5
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00001588

l000015D0:
	cmpi.b	#$68,(a2)
	beq	$000015F4

l000015D6:
	cmpi.b	#$6C,(a2)
	beq	$000015F4

l000015DC:
	cmpi.b	#$4C,(a2)
	beq	$000015F4

l000015E2:
	cmpi.b	#$6A,(a2)
	beq	$000015F4

l000015E8:
	cmpi.b	#$7A,(a2)
	beq	$000015F4

l000015EE:
	cmpi.b	#$74,(a2)
	bne	$000015FA

l000015F4:
	move.b	(a2)+,d4
	ext.w	d4
	ext.l	d4

l000015FA:
	cmp.l	#$00000068,d4
	bne	$0000160C

l00001602:
	cmpi.b	#$68,(a2)
	bne	$0000160C

l00001608:
	moveq	#$02,d4
	addq.l	#$01,a2

l0000160C:
	cmp.l	#$0000006C,d4
	bne	$0000161E

l00001614:
	cmpi.b	#$6C,(a2)
	bne	$0000161E

l0000161A:
	moveq	#$01,d4
	addq.l	#$01,a2

l0000161E:
	cmp.l	#$0000006A,d4
	bne	$00001628

l00001626:
	moveq	#$01,d4

l00001628:
	cmp.l	#$0000007A,d4
	bne	$00001632

l00001630:
	moveq	#$6C,d4

l00001632:
	cmp.l	#$00000074,d4
	bne	$0000163C

l0000163A:
	moveq	#$69,d4

l0000163C:
	move.b	(a2)+,d1
	move.b	d1,d0
	cmp.b	#$25,d1
	beq	$00001AC4

l00001648:
	cmp.b	#$58,d0
	beq	$00001696

l0000164E:
	cmp.b	#$63,d0
	beq	$00001A44

l00001656:
	cmp.b	#$64,d0
	beq	$00001696

l0000165C:
	cmp.b	#$69,d0
	beq	$00001696

l00001662:
	move.b	d0,$002C(a7)
	cmp.b	#$6E,d0
	beq	$00001AD6

l0000166E:
	move.b	$002C(a7),d0
	sub.b	#$6F,d0
	cmp.b	#$01,d0
	bls	$00001696

l0000167C:
	move.b	$002C(a7),d0
	cmp.b	#$73,d0
	beq	$00001A80

l00001688:
	cmp.b	#$75,d0
	beq	$00001696

l0000168E:
	cmp.b	#$78,d0
	bne	$00001B6E

l00001696:
	cmp.b	#$70,d1
	bne	$000016A8

l0000169C:
	moveq	#$6C,d4
	moveq	#$78,d1
	ori.l	#$00000001,$0048(a7)

l000016A8:
	cmp.b	#$64,d1
	beq	$000016B6

l000016AE:
	cmp.b	#$69,d1
	bne	$00001800

l000016B6:
	cmp.l	#$00000001,d4
	bne	$000016DC

l000016BE:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$0034(a7)
	move.l	-$0008(a0),$0030(a7)
	bra	$00001774

l000016DC:
	cmp.l	#$0000006C,d4
	bne	$00001708

l000016E4:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),d0
	move.l	d0,$0034(a7)
	move.l	d1,-(a7)
	moveq	#$1F,d1
	asr.l	d1,d0
	move.l	d0,$0034(a7)
	move.l	(a7)+,d1
	bra	$00001774

l00001708:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),d0
	move.l	d0,$0034(a7)
	move.l	d1,-(a7)
	moveq	#$1F,d1
	asr.l	d1,d0
	move.l	d0,$0034(a7)
	move.l	(a7)+,d1
	cmp.l	#$00000068,d4
	bne	$0000174E

l00001732:
	move.w	$0036(a7),d0
	move.l	d1,-(a7)
	move.w	d0,d1
	ext.l	d1
	move.l	d1,$0038(a7)
	move.l	d0,-(a7)
	moveq	#$1F,d0
	asr.l	d0,d1
	move.l	d1,$0038(a7)
	move.l	(a7)+,d1
	move.l	(a7)+,d0

l0000174E:
	cmp.l	#$00000002,d4
	bne	$00001774

l00001756:
	move.b	$0037(a7),d0
	move.l	d1,-(a7)
	move.b	d0,d1
	ext.w	d1
	ext.l	d1
	move.l	d1,$0038(a7)
	move.l	d0,-(a7)
	moveq	#$1F,d0
	asr.l	d0,d1
	move.l	d1,$0038(a7)
	move.l	(a7)+,d1
	move.l	(a7)+,d0

l00001774:
	move.b	d1,$002C(a7)
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	$003C(a7),d0
	move.l	$0038(a7),d1
	move.l	d2,-(a7)
	moveq	#$00,d2
	sub.l	#$00000000,d0
	subx.l	d2,d1
	movem.l	(a7)+,d2
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	bge	$000017BE

l0000179E:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2D,(a0)
	movem.l	$0030(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0038(a7)
	bra	$00001934

l000017BE:
	move.b	$002C(a7),d1
	moveq	#$10,d0
	and.l	$0048(a7),d0
	beq	$000017D8

l000017CA:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2B,(a0)
	bra	$000017EC

l000017D8:
	moveq	#$08,d0
	and.l	$0048(a7),d0
	beq	$000017EC

l000017E0:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$20,(a0)

l000017EC:
	move.l	$0034(a7),$003C(a7)
	move.l	$0030(a7),$0038(a7)
	move.b	d1,$002C(a7)
	bra	$00001934

l00001800:
	cmp.l	#$00000001,d4
	bne	$00001824

l00001808:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	move.l	-$0008(a0),$0038(a7)
	bra	$0000185E

l00001824:
	cmp.l	#$0000006C,d4
	bne	$00001846

l0000182C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)
	bra	$0000185E

l00001846:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)

l0000185E:
	cmp.l	#$00000068,d4
	bne	$0000187A

l00001866:
	move.w	$003E(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.w	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l0000187A:
	cmp.l	#$00000002,d4
	bne	$00001896

l00001882:
	move.b	$003F(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l00001896:
	moveq	#$01,d0
	and.l	$0048(a7),d0
	move.b	d1,$002C(a7)
	tst.l	d0
	beq	$00001934

l000018A6:
	cmp.b	#$6F,d1
	bne	$000018E2

l000018AC:
	tst.l	d5
	bne	$000018D6

l000018B0:
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	$0044(a7),d0
	move.l	$0040(a7),d1
	move.l	d2,-(a7)
	moveq	#$00,d2
	sub.l	#$00000000,d0
	subx.l	d2,d1
	movem.l	(a7)+,d2
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	beq	$000018E2

l000018D6:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$30,(a0)

l000018E2:
	cmp.b	#$78,d1
	beq	$000018F2

l000018E8:
	move.b	d1,$002C(a7)
	cmp.b	#$58,d1
	bne	$00001934

l000018F2:
	move.b	d1,$002C(a7)
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	$0044(a7),d0
	move.l	$0040(a7),d1
	move.l	d2,-(a7)
	moveq	#$00,d2
	sub.l	#$00000000,d0
	subx.l	d2,d1
	movem.l	(a7)+,d2
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	beq	$00001934

l0000191C:
	lea	$006A(a7),a0
	lea	(a0,d7),a1
	addq.l	#$01,d7
	move.b	#$30,(a1)
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	d1,(a0)
	move.b	d1,$002C(a7)

l00001934:
	move.b	$002C(a7),d1
	lea	$0062(a7),a3
	cmp.b	#$78,d1
	beq	$00001948

l00001942:
	cmp.b	#$58,d1
	bne	$00001952

l00001948:
	move.l	#$00000010,$002C(a7)
	bra	$00001970

l00001952:
	cmp.b	#$6F,d1
	bne	$00001962

l00001958:
	move.l	#$00000008,$0030(a7)
	bra	$0000196A

l00001962:
	move.l	#$0000000A,$0030(a7)

l0000196A:
	move.l	$0030(a7),$002C(a7)

l00001970:
	move.l	$002C(a7),$006C(a7)
	cmp.b	#$58,d1
	beq	$00001982

l0000197C:
	lea	00001D78,a6                                            ; $03FC(pc)
	bra	$00001986

l00001982:
	lea	00001D88,a6                                            ; $0406(pc)

l00001986:
	move.l	a6,$002C(a7)
	move.l	d3,$007C(a7)
	move.l	d5,$0044(a7)
	move.l	d6,$0030(a7)
	move.l	d7,$0062(a7)
	movem.l	$0038(a7),d6-d7
	move.l	$0066(a7),d3
	movea.l	$002C(a7),a1

l000019A8:
	move.l	$006C(a7),d1
	move.l	d1,d0
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	a1,-(a7)
	movem.l	d0-d1,-(a7)
	movem.l	d6-d7,-(a7)
	jsr.l	$00002700
	lea	$0010(a7),a7
	movea.l	(a7)+,a1
	move.l	d0,d4
	move.l	d1,d5
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	move.l	d5,d2
	move.b	(a1,d2),-(a3)
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	a1,-(a7)
	movem.l	d0-d1,-(a7)
	movem.l	d6-d7,-(a7)
	jsr.l	$000023B8
	lea	$0010(a7),a7
	movea.l	(a7)+,a1
	move.l	d0,d6
	move.l	d1,d7
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	addq.l	#$01,d3
	move.l	d7,d0
	move.l	d6,d1
	moveq	#$00,d2
	sub.l	#$00000000,d0
	subx.l	d2,d1
	bne	$000019A8

l00001A16:
	move.l	d3,$0066(a7)
	move.l	$0062(a7),d7
	move.l	$0030(a7),d6
	move.l	$0044(a7),d5
	move.l	$007C(a7),d3
	cmp.l	#$FFFFFFFF,d5
	bne	$00001A38

l00001A32:
	moveq	#$00,d5
	bra	$00001B84

l00001A38:
	andi.l	#$FFFFFFFD,$0048(a7)
	bra	$00001B84

l00001A44:
	cmp.l	#$0000006C,d4
	bne	$00001A60

l00001A4C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)
	bra	$00001A72

l00001A60:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)

l00001A72:
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001B84

l00001A80:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a3
	movea.l	a3,a1
	clr.l	$0066(a7)
	tst.l	d5
	beq	$00001AA2

l00001A9C:
	cmp.l	$0066(a7),d5
	bls	$00001ABE

l00001AA2:
	tst.b	(a1)
	beq	$00001ABE

l00001AA6:
	move.l	$0066(a7),d0

l00001AAA:
	addq.l	#$01,d0
	addq.l	#$01,a1
	tst.l	d5
	bls	$00001AB6

l00001AB2:
	cmp.l	d0,d5
	bls	$00001ABA

l00001AB6:
	tst.b	(a1)
	bne	$00001AAA

l00001ABA:
	move.l	d0,$0066(a7)

l00001ABE:
	moveq	#$00,d5
	bra	$00001B84

l00001AC4:
	lea	00001D6C,a3                                            ; $02A8(pc)
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001B84

l00001AD6:
	cmp.l	#$00000001,d4
	bne	$00001AF8

l00001ADE:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,$0004(a0)
	clr.l	(a0)
	bra	$00001B66

l00001AF8:
	cmp.l	#$0000006C,d4
	bne	$00001B16

l00001B00:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)
	bra	$00001B66

l00001B16:
	cmp.l	#$00000068,d4
	bne	$00001B34

l00001B1E:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.w	d6,(a0)
	bra	$00001B66

l00001B34:
	cmp.l	#$00000002,d4
	bne	$00001B52

l00001B3C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.b	d6,(a0)
	bra	$00001B66

l00001B52:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)

l00001B66:
	moveq	#$00,d5
	clr.l	$0040(a7)
	bra	$00001B84

l00001B6E:
	tst.b	d1
	bne	$00001B74

l00001B72:
	subq.l	#$01,a2

l00001B74:
	movea.l	a4,a3
	move.l	a2,d0
	sub.l	a4,d0
	move.l	d0,$0066(a7)
	moveq	#$00,d5
	clr.l	$0040(a7)

l00001B84:
	cmp.l	$0066(a7),d5
	bhi	$00001B92

l00001B8A:
	move.l	$0066(a7),$002C(a7)
	bra	$00001B96

l00001B92:
	move.l	d5,$002C(a7)

l00001B96:
	move.l	d0,-(a7)
	move.l	$0030(a7),d0
	add.l	d7,d0
	move.l	d0,$0034(a7)
	move.l	(a7)+,d0
	move.l	d0,-(a7)
	move.l	$0034(a7),d0
	cmp.l	$0044(a7),d0
	movem.l	(a7)+,d0
	bcs	$00001BBA

l00001BB4:
	clr.l	$002C(a7)
	bra	$00001BCA

l00001BBA:
	move.l	d0,-(a7)
	move.l	$0044(a7),d0
	sub.l	$0034(a7),d0
	move.l	d0,$0030(a7)
	move.l	(a7)+,d0

l00001BCA:
	move.l	$002C(a7),$0030(a7)
	moveq	#$02,d0
	and.l	$0048(a7),d0
	beq	$00001C0C

l00001BD8:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001C0C

l00001BDE:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001D98
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001C04

l00001BFE:
	move.l	d6,d0
	bra	$00001D60

l00001C04:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001BDE

l00001C0C:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	bne	$00001C5E

l00001C14:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001C5E

l00001C1C:
	move.l	$0048(a7),d4
	movea.l	$0030(a7),a4

l00001C24:
	move.l	a5,-(a7)
	moveq	#$02,d0
	and.l	d4,d0
	beq	$00001C32

l00001C2C:
	movea.w	#$0030,a0
	bra	$00001C36

l00001C32:
	movea.w	#$0020,a0

l00001C36:
	move.l	a0,-(a7)
	jsr.l	$00001D98
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001C4E

l00001C48:
	move.l	d6,d0
	bra	$00001D60

l00001C4E:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001C24

l00001C56:
	move.l	a4,$0030(a7)
	move.l	d4,$0048(a7)

l00001C5E:
	moveq	#$02,d0
	and.l	$0048(a7),d0
	bne	$00001C9A

l00001C66:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001C9A

l00001C6C:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001D98
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001C92

l00001C8C:
	move.l	d6,d0
	bra	$00001D60

l00001C92:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001C6C

l00001C9A:
	move.l	$0066(a7),d2
	cmp.l	$0066(a7),d5
	bls	$00001CC8

l00001CA4:
	move.l	a5,-(a7)
	pea	$00000030
	jsr.l	$00001D98
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001CC0

l00001CBA:
	move.l	d6,d0
	bra	$00001D60

l00001CC0:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d5
	bhi	$00001CA4

l00001CC8:
	moveq	#$00,d2
	tst.l	$0066(a7)
	beq	$00001CFE

l00001CD0:
	movea.l	$0066(a7),a4

l00001CD4:
	move.l	a5,-(a7)
	lea	(a3,d2),a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001D98
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001CF6

l00001CF2:
	move.l	d6,d0
	bra	$00001D60

l00001CF6:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001CD4

l00001CFE:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	beq	$00001D34

l00001D06:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001D34

l00001D0E:
	movea.l	$0030(a7),a3

l00001D12:
	move.l	a5,-(a7)
	pea	$00000020
	jsr.l	$00001D98
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D2C

l00001D28:
	move.l	d6,d0
	bra	$00001D60

l00001D2C:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a3
	bhi	$00001D12

l00001D34:
	movea.l	a2,a4
	bra	$00001D58

l00001D38:
	move.l	a5,-(a7)
	move.b	(a4)+,d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001D98
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D56

l00001D52:
	move.l	d6,d0
	bra	$00001D60

l00001D56:
	addq.l	#$01,d6

l00001D58:
	tst.b	(a4)
	bne	$00001434

l00001D5E:
	move.l	d6,d0

l00001D60:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$0044(a7),a7
	rts
00001D6A                               00 00 25 00 00 00           ..%...
00001D70 23 30 2D 20 2B 00 00 00 30 31 32 33 34 35 36 37 #0- +...01234567
00001D80 38 39 61 62 63 64 65 66 30 31 32 33 34 35 36 37 89abcdef01234567
00001D90 38 39 41 42 43 44 45 46                         89ABCDEF        

;; fn00001D98: 00001D98
;;   Called from:
;;     00001BEE (in fn00001418)
;;     00001C38 (in fn00001418)
;;     00001C7C (in fn00001418)
;;     00001CAA (in fn00001418)
;;     00001CE2 (in fn00001418)
;;     00001D18 (in fn00001418)
;;     00001D42 (in fn00001418)
fn00001D98 proc
	movem.l	d2/a2-a3,-(a7)
	move.l	$0010(a7),d2
	movea.l	$0014(a7),a2
	lea	$0018(a2),a0
	moveq	#$02,d0
	or.l	d0,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001DDC

l00001DB4:
	moveq	#$0A,d0
	cmp.l	d2,d0
	bne	$00001DC6

l00001DBA:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	bne	$00001DDC

l00001DC6:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a3
	addq.l	#$01,a3
	move.l	a3,(a1)
	move.b	d2,(a0)
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00001DEA

l00001DDC:
	move.l	a2,-(a7)
	move.l	d2,-(a7)
	jsr.l	$00001DF4
	move.l	d0,d1
	addq.w	#$08,a7

l00001DEA:
	move.l	d1,d0
	movem.l	(a7)+,d2/a2-a3
	rts
00001DF2       00 00                                       ..            

;; fn00001DF4: 00001DF4
;;   Called from:
;;     00001DE0 (in fn00001D98)
fn00001DF4 proc
	movem.l	d2-d6/a2-a4/a6,-(a7)
	move.l	$0028(a7),d5
	movea.l	$002C(a7),a2
	jsr.l	$00002388
	move.l	a2,d0
	bne	$00001E10

l00001E0A:
	moveq	#-$01,d0
	bra	$00001F02

l00001E10:
	moveq	#$49,d0
	and.l	$0018(a2),d0
	moveq	#$40,d6
	cmp.l	d0,d6
	beq	$00001E22

l00001E1C:
	moveq	#-$01,d0
	bra	$00001F02

l00001E22:
	tst.l	$001C(a2)
	bne	$00001E40

l00001E28:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00001E38

l00001E30:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00001E40

l00001E38:
	move.l	#$00000400,$001C(a2)

l00001E40:
	tst.l	$0008(a2)
	bne	$00001E80

l00001E46:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00001E5A

l00001E56:
	moveq	#$02,d4
	bra	$00001E5C

l00001E5A:
	moveq	#$01,d4

l00001E5C:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	$00001FB4
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00001E76

l00001E70:
	moveq	#-$01,d0
	bra	$00001F02

l00001E76:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)
	bra	$00001EDE

l00001E80:
	tst.l	(a2)
	beq	$00001EDA

l00001E84:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00001EB0

l00001E90:
	moveq	#$0A,d0
	cmp.l	d5,d0
	bne	$00001EB0

l00001E96:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	d5,(a0)
	move.l	a2,-(a7)
	jsr.l	$00001F08
	addq.w	#$04,a7
	bra	$00001F02

l00001EB0:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003D78,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00001EDE

l00001ECE:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00001F02

l00001EDA:
	moveq	#$00,d0
	bra	$00001F02

l00001EDE:
	lea	$0018(a2),a0
	moveq	#$02,d0
	or.l	d0,(a0)
	lea	$0008(a2),a1
	movea.l	(a1),a0
	move.b	d5,(a0)
	movea.l	(a1),a1
	addq.l	#$01,a1
	move.l	a1,$0004(a2)
	move.l	$001C(a2),d0
	subq.l	#$01,d0
	move.l	d0,$0014(a2)
	moveq	#$00,d0

l00001F02:
	movem.l	(a7)+,d2-d6/a2-a4/a6
	rts

;; fn00001F08: 00001F08
;;   Called from:
;;     00001EA6 (in fn00001DF4)
;;     00002B0A (in fn00002AFC)
;;     00002B28 (in fn00002AFC)
;;     00003D52 (in fn00003D2C)
fn00001F08 proc
	movem.l	d2-d4/a2/a6,-(a7)
	movea.l	$0018(a7),a2
	jsr.l	$00002388
	move.l	a2,d0
	bne	$00001F1E

l00001F1A:
	moveq	#-$01,d0
	bra	$00001F96

l00001F1E:
	tst.l	$001C(a2)
	bne	$00001F3C

l00001F24:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00001F34

l00001F2C:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00001F3C

l00001F34:
	move.l	#$00000400,$001C(a2)

l00001F3C:
	tst.l	$0008(a2)
	bne	$00001F46

l00001F42:
	moveq	#$00,d0
	bra	$00001F96

l00001F46:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00001F8C

l00001F4E:
	tst.l	(a2)
	beq	$00001F7C

l00001F52:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003D78,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00001F80

l00001F70:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00001F96

l00001F7C:
	moveq	#$00,d0
	bra	$00001F96

l00001F80:
	move.l	$0008(a2),$0004(a2)
	move.l	$001C(a2),$0014(a2)

l00001F8C:
	lea	$0018(a2),a0
	moveq	#-$04,d0
	and.l	d0,(a0)
	moveq	#$00,d0

l00001F96:
	movem.l	(a7)+,d2-d4/a2/a6
	rts
00001F9C                                     4A B9 00 00             J...
00001FA0 3D A8 67 0E 2F 39 00 00 3D A8 4E B9 00 00 23 18 =.g./9..=.N...#.
00001FB0 58 4F 4E 75                                     XONu            

;; fn00001FB4: 00001FB4
;;   Called from:
;;     00001E62 (in fn00001DF4)
;;     00003CB2 (in fn00003C30)
fn00001FB4 proc
	movem.l	d2,-(a7)
	move.l	$0008(a7),d2
	bne	$00001FC2

l00001FBE:
	moveq	#$00,d0
	bra	$00002018

l00001FC2:
	tst.l	$00003DA8
	bne	$00001FE6

l00001FCA:
	movea.l	$00003DA4,a0
	move.l	a0,-(a7)
	move.l	a0,-(a7)
	clr.l	-(a7)
	jsr.l	$000022A8
	move.l	d0,$00003DA8
	lea	$000C(a7),a7

l00001FE6:
	tst.l	$00003DA8
	bne	$00001FF2

l00001FEE:
	moveq	#$00,d0
	bra	$00002018

l00001FF2:
	moveq	#$04,d0
	add.l	d2,d0
	move.l	d0,-(a7)
	move.l	$00003DA8,-(a7)
	jsr.l	$00002184
	movea.l	d0,a1
	addq.w	#$08,a7
	move.l	a1,d0
	bne	$00002010

l0000200C:
	moveq	#$00,d0
	bra	$00002018

l00002010:
	move.l	d2,(a1)
	lea	$0004(a1),a0
	move.l	a0,d0

l00002018:
	movem.l	(a7)+,d2
	rts
0000201E                                           00 00               ..

;; fn00002020: 00002020
fn00002020 proc
	move.l	$0004(a7),d0
	movea.l	d0,a0
	tst.l	d0
	beq	$0000204A

l0000202A:
	tst.l	$00003DA8
	beq	$0000204A

l00002032:
	moveq	#$04,d0
	add.l	-(a0),d0
	move.l	d0,-(a7)
	move.l	a0,-(a7)
	move.l	$00003DA8,-(a7)
	jsr.l	$000020E8
	lea	$000C(a7),a7

l0000204A:
	rts
0000204C                                     48 E7 30 38             H.08
00002050 28 6F 00 1C 24 6F 00 18 22 0A 66 0A 2F 0C 61 00 (o..$o..".f./.a.
00002060 FF 54 58 4F 60 7A 26 6A FF FC 2F 0C 61 00 FF 46 .TXO`z&j../.a..F
00002070 26 00 58 4F 67 68 B9 CB 64 04 20 0C 60 02 20 0B &.XOgh..d. .`. .
00002080 20 43 22 4A 24 00 B4 BC 00 00 00 10 65 3C 20 08  C"J$.......e< .
00002090 22 09 C0 3C 00 01 C2 3C 00 01 B2 00 66 1A 20 08 "..<...<....f. .
000020A0 4A 01 67 04 10 D9 53 82 72 03 C2 82 94 81 20 D9 J.g...S.r..... .
000020B0 59 82 66 FA 34 01 60 14 B4 BC 00 01 00 00 65 0A Y.f.4.`.......e.
000020C0 20 08 10 D9 53 82 66 FA 60 0C 20 08 53 42 65 06  ...S.f.`. .SBe.
000020D0 10 D9 51 CA FF FC 2F 0A 61 00 FF 46 58 4F 20 03 ..Q.../.a..FXO .
000020E0 4C DF 1C 0C 4E 75 00 00                         L...Nu..        

;; fn000020E8: 000020E8
;;   Called from:
;;     00002040 (in fn00002020)
fn000020E8 proc
	movem.l	d2/a2-a6,-(a7)
	move.l	$0020(a7),d1
	movea.l	$0024(a7),a5
	movea.l	$001C(a7),a4
	movea.l	$00003D74,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002118

l00002106:
	movea.l	$00003D74,a6
	movea.l	a4,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$02CA(a6)
	bra	$0000217C

l00002118:
	move.l	a4,d2
	beq	$0000217C

l0000211C:
	tst.l	d1
	beq	$0000217C

l00002120:
	movea.l	d1,a3
	lea	-$000C(a3),a3
	cmpa.l	$0014(a4),a5
	bcc	$00002162

l0000212C:
	movea.l	a4,a2

l0000212E:
	movea.l	(a2),a2
	tst.l	(a2)
	beq	$0000217C

l00002134:
	tst.b	$0008(a2)
	beq	$0000212E

l0000213A:
	cmp.l	$0014(a2),d1
	bcs	$0000212E

l00002140:
	cmp.l	$0018(a2),d1
	bcc	$0000212E

l00002146:
	movea.l	$00003D74,a6
	movea.l	a2,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$00C0(a6)
	move.l	$001C(a2),d0
	cmp.l	$0010(a4),d0
	bne	$0000217C

l00002160:
	movea.l	a2,a3

l00002162:
	movea.l	$00003D74,a6
	movea.l	a3,a1
	jsr.l	-$00FC(a6)
	move.l	-(a3),d0
	movea.l	$00003D74,a6
	movea.l	a3,a1
	jsr.l	-$00D2(a6)

l0000217C:
	movem.l	(a7)+,d2/a2-a6
	rts
00002182       00 00                                       ..            

;; fn00002184: 00002184
;;   Called from:
;;     00001FFE (in fn00001FB4)
fn00002184 proc
	movem.l	d2-d4/a2-a6,-(a7)
	move.l	$0028(a7),d2
	movea.l	$0024(a7),a4
	movea.l	$00003D74,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$000021B0

l0000219E:
	movea.l	$00003D74,a6
	movea.l	a4,a0
	move.l	d2,d0
	jsr.l	-$02C4(a6)
	bra	$000022A2

l000021B0:
	suba.l	a3,a3
	move.l	a4,d4
	beq	$000022A0

l000021B8:
	tst.l	d2
	beq	$000022A0

l000021BE:
	cmp.l	$0014(a4),d2
	bcc	$00002272

l000021C6:
	movea.l	(a4),a5

l000021C8:
	tst.l	(a5)
	beq	$000021EA

l000021CC:
	tst.b	$0008(a5)
	beq	$000021E6

l000021D2:
	movea.l	$00003D74,a6
	movea.l	a5,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3
	move.l	a3,d0
	bne	$00002256

l000021E6:
	movea.l	(a5),a5
	bra	$000021C8

l000021EA:
	moveq	#$28,d3
	add.l	$0010(a4),d3
	move.l	$000C(a4),d1
	movea.l	$00003D74,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$000022A0

l00002208:
	move.l	d3,(a3)+
	move.b	#$0A,$0008(a3)
	lea	$0024(a3),a2
	lea	$0010(a3),a0
	move.l	a2,(a0)
	move.l	a2,$0014(a3)
	movea.l	(a0),a1
	clr.l	(a1)
	movea.l	(a0),a2
	addq.l	#$04,a2
	move.l	$0010(a4),(a2)
	lea	$001C(a3),a1
	move.l	(a2),(a1)
	movea.l	(a1),a1
	adda.l	(a0),a1
	move.l	a1,$0018(a3)
	movea.l	$00003D74,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F0(a6)
	movea.l	$00003D74,a6
	movea.l	a3,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3

l00002256:
	move.l	#$00010000,d0
	and.l	$000C(a4),d0
	beq	$000022A0

l00002262:
	movea.l	a3,a2
	addq.l	#$07,d2
	lsr.l	#$03,d2

l00002268:
	clr.l	(a2)+
	clr.l	(a2)+
	subq.l	#$01,d2
	bne	$00002268

l00002270:
	bra	$000022A0

l00002272:
	moveq	#$10,d3
	add.l	d2,d3
	move.l	$000C(a4),d1
	movea.l	$00003D74,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$000022A0

l0000228C:
	move.l	d3,(a3)+
	movea.l	$00003D74,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F6(a6)
	addq.l	#$08,a3
	clr.l	(a3)+

l000022A0:
	move.l	a3,d0

l000022A2:
	movem.l	(a7)+,d2-d4/a2-a6
	rts

;; fn000022A8: 000022A8
;;   Called from:
;;     00001FD6 (in fn00001FB4)
fn000022A8 proc
	movem.l	d2-d3/a2/a6,-(a7)
	move.l	$0018(a7),d3
	movea.l	$001C(a7),a2
	movea.l	$00003D74,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$000022D6

l000022C2:
	movea.l	$00003D74,a6
	move.l	$0014(a7),d0
	move.l	d3,d1
	move.l	a2,d2
	jsr.l	-$02B8(a6)
	bra	$00002312

l000022D6:
	suba.l	a1,a1
	cmp.l	a2,d3
	bcs	$00002310

l000022DC:
	addq.l	#$07,d3
	movea.l	$00003D74,a6
	moveq	#$18,d0
	moveq	#$00,d1
	jsr.l	-$00C6(a6)
	movea.l	d0,a1
	move.l	a1,d0
	beq	$00002310

l000022F2:
	lea	$0004(a1),a0
	move.l	a0,(a1)
	clr.l	(a0)
	move.l	a1,$0008(a1)
	move.l	$0014(a7),$000C(a1)
	moveq	#-$08,d0
	and.l	d3,d0
	move.l	d0,$0010(a1)
	move.l	a2,$0014(a1)

l00002310:
	move.l	a1,d0

l00002312:
	movem.l	(a7)+,d2-d3/a2/a6
	rts

;; fn00002318: 00002318
fn00002318 proc
	movem.l	d2/a2/a6,-(a7)
	move.l	$0010(a7),d2
	movea.l	$00003D74,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$0000233C

l0000232E:
	movea.l	$00003D74,a6
	movea.l	d2,a0
	jsr.l	-$02BE(a6)
	bra	$00002380

l0000233C:
	tst.l	d2
	beq	$00002380

l00002340:
	movea.l	$00003D74,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d1
	beq	$00002372

l00002352:
	move.l	-(a2),d0
	movea.l	$00003D74,a6
	movea.l	a2,a1
	jsr.l	-$00D2(a6)
	movea.l	$00003D74,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d0
	bne	$00002352

l00002372:
	movea.l	$00003D74,a6
	movea.l	d2,a1
	moveq	#$18,d0
	jsr.l	-$00D2(a6)

l00002380:
	movem.l	(a7)+,d2/a2/a6
	rts
00002386                   00 00                               ..        

;; fn00002388: 00002388
;;   Called from:
;;     00001E00 (in fn00001DF4)
;;     00001F10 (in fn00001F08)
;;     00003C38 (in fn00003C30)
fn00002388 proc
	movem.l	a6,-(a7)
	movea.l	$00003D74,a6
	moveq	#$00,d0
	move.l	#$00001000,d1
	jsr.l	-$0132(a6)
	and.l	#$00001000,d0
	beq	$000023B2

l000023A6:
	pea	$00000014
	jsr.l	$0000131C
	addq.w	#$04,a7

l000023B2:
	movea.l	(a7)+,a6
	rts
000023B6                   00 00                               ..        

;; fn000023B8: 000023B8
;;   Called from:
;;     000019EC (in fn00001418)
fn000023B8 proc
	movem.l	d2-d6,-(a7)
	move.l	$001C(a7),d1
	move.l	$0018(a7),d0
	movea.l	d1,a0
	move.l	$0024(a7),d3
	move.l	$0020(a7),d2
	bne	$0000240E

l000023D0:
	cmp.l	d3,d0
	bcc	$000023E2

l000023D4:
	move.l	d3,d2
	jsr.l	$000024BC
	move.l	d0,d1
	bra	$000024B4

l000023E2:
	tst.l	d3
	bne	$000023EE

l000023E6:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l000023EE:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	$000024BC
	movea.l	d0,a1
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	$000024BC
	move.l	d0,d1
	move.l	a1,d0
	bra	$000024B6

l0000240E:
	cmp.l	d2,d0
	bcc	$00002418

l00002412:
	moveq	#$00,d0
	bra	$000024B4

l00002418:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002436

l00002422:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002436

l0000242A:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002436

l00002432:
	moveq	#$00,d4
	move.b	d2,d6

l00002436:
	lea	$00003DAC,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$00002456

l0000244A:
	cmp.l	d0,d2
	bcs	$00002452

l0000244E:
	cmp.l	a0,d3
	bhi	$00002412

l00002452:
	moveq	#$01,d0
	bra	$000024B4

l00002456:
	lsl.l	d5,d2
	move.l	d3,d6
	lsr.l	d4,d6
	or.l	d6,d2
	lsl.l	d5,d3
	move.l	d0,d1
	lsr.l	d4,d0
	lsl.l	d5,d1
	move.l	a0,d6
	lsr.l	d4,d6
	or.l	d6,d1
	move.l	a0,d6
	lsl.l	d5,d6
	movea.l	d6,a0
	jsr.l	$000024BC
	move.l	d0,d2
	exg	d0,d1
	move.l	d2,d4
	move.l	d2,d5
	move.l	d3,d6
	mulu.w	d3,d2
	swap.l	d5
	mulu.w	d5,d3
	swap.l	d6
	mulu.w	d6,d4
	mulu.w	d6,d5
	moveq	#$00,d6
	swap.l	d2
	add.w	d3,d2
	addx.w	d6,d5
	add.w	d4,d2
	addx.w	d6,d5
	swap.l	d2
	clr.w	d3
	clr.w	d4
	swap.l	d3
	swap.l	d4
	add.l	d4,d3
	add.l	d5,d3
	cmp.l	d3,d0
	bcs	$000024B2

l000024AC:
	bne	$000024B4

l000024AE:
	cmpa.l	d2,a0
	bcc	$000024B4

l000024B2:
	subq.l	#$01,d1

l000024B4:
	moveq	#$00,d0

l000024B6:
	movem.l	(a7)+,d2-d6
	rts

;; fn000024BC: 000024BC
;;   Called from:
;;     000023D6 (in fn000023B8)
;;     000023F4 (in fn000023B8)
;;     00002400 (in fn000023B8)
;;     00002472 (in fn000023B8)
;;     0000271E (in fn00002700)
;;     0000273C (in fn00002700)
;;     00002746 (in fn00002700)
;;     000027B4 (in fn00002700)
fn000024BC proc
	movem.l	d5-d7,-(a7)
	move.l	d2,d7
	beq	$000024D6

l000024C4:
	move.l	d1,d6
	move.l	d0,d5
	bne	$000024E4

l000024CA:
	tst.l	d1
	beq	$00002602

l000024D0:
	cmp.l	d1,d2
	bhi	$00002602

l000024D6:
	move.l	d1,d0
	move.l	d2,d1
	jsr.l	$0000267A
	bra	$00002602

l000024E4:
	swap.l	d2
	tst.w	d2
	bne	$0000250C

l000024EA:
	moveq	#$00,d2
	swap.l	d0
	swap.l	d1
	move.w	d1,d0
	divu.w	d7,d0
	move.w	d0,d2
	swap.l	d1
	move.w	d1,d0
	divu.w	d7,d0
	swap.l	d2
	move.w	d0,d2
	clr.w	d0
	swap.l	d0
	move.l	d0,d1
	move.l	d2,d0
	bra	$00002602

l0000250C:
	movem.l	d2-d4/a0-a1,-(a7)
	subq.l	#$08,a7
	clr.b	$0002(a7)
	moveq	#$00,d1
	moveq	#$00,d0
	tst.l	d7
	bmi	$00002528

l0000251E:
	addq.w	#$01,d0
	add.l	d6,d6
	addx.l	d5,d5
	add.l	d7,d7
	bpl	$0000251E

l00002528:
	move.w	d0,(a7)

l0000252A:
	move.l	d7,d3
	move.l	d5,d2
	swap.l	d2
	swap.l	d3
	cmp.w	d3,d2
	bne	$0000253C

l00002536:
	move.w	#$FFFF,d1
	bra	$00002546

l0000253C:
	move.l	d5,d1
	divu.w	d3,d1
	swap.l	d1
	clr.w	d1
	swap.l	d1

l00002546:
	movea.l	d6,a1
	clr.w	d6
	swap.l	d6

l0000254C:
	move.l	d7,d3
	move.l	d1,d2
	mulu.w	d7,d2
	swap.l	d3
	mulu.w	d1,d3
	move.l	d5,d4
	sub.l	d3,d4
	swap.l	d4
	move.w	d4,d0
	move.w	d6,d4
	tst.w	d0
	bne	$0000256C

l00002564:
	cmp.l	d4,d2
	bls	$0000256C

l00002568:
	subq.l	#$01,d1
	bra	$0000254C

l0000256C:
	movea.l	d5,a0
	move.l	d1,d6
	swap.l	d6
	move.l	d7,d5
	move.l	d6,d2
	move.l	d6,d3
	move.l	d5,d4
	mulu.w	d5,d6
	swap.l	d3
	mulu.w	d3,d5
	swap.l	d4
	mulu.w	d4,d2
	mulu.w	d4,d3
	moveq	#$00,d4
	swap.l	d6
	add.w	d5,d6
	addx.w	d4,d3
	add.w	d2,d6
	addx.w	d4,d3
	swap.l	d6
	clr.w	d5
	clr.w	d2
	swap.l	d5
	swap.l	d2
	add.l	d2,d5
	add.l	d3,d5
	move.l	d5,d2
	move.l	d6,d3
	move.l	a0,d5
	move.l	a1,d6
	sub.l	d3,d6
	subx.l	d2,d5
	bcc	$000025C4

l000025AE:
	subq.l	#$01,d1
	moveq	#$00,d2
	move.l	d7,d3
	swap.l	d3
	clr.w	d3
	add.l	d3,d6
	addx.l	d2,d5
	move.l	d7,d3
	clr.w	d3
	swap.l	d3
	add.l	d3,d5

l000025C4:
	tst.b	$0002(a7)
	bne	$000025E0

l000025CA:
	move.w	d1,$0004(a7)
	moveq	#$00,d1
	swap.l	d5
	swap.l	d6
	move.w	d6,d5
	clr.w	d6
	st	$0002(a7)
	bra	$0000252A

l000025E0:
	move.l	$0004(a7),d0
	move.w	d1,d0
	move.w	d5,d6
	swap.l	d6
	swap.l	d5
	move.w	(a7),d7
	beq	$000025FA

l000025F0:
	subq.w	#$01,d7

l000025F2:
	lsr.l	#$01,d5
	roxr.l	#$01,d6
	dbra	d7,$000025F2

l000025FA:
	move.l	d6,d1
	addq.l	#$08,a7
	movem.l	(a7)+,d2-d4/a0-a1

l00002602:
	movem.l	(a7)+,d5-d7
	rts
00002608                         4C EF 00 03 00 04 4A 81         L.....J.
00002610 6B 0A 4A 80 6B 12 61 62 20 01 4E 75 44 81 4A 80 k.J.k.ab .NuD.J.
00002620 6B 10 61 56 20 01 4E 75 44 80 61 4E 44 81 20 01 k.aV .NuD.aND. .
00002630 4E 75 44 80 61 44 44 81 20 01 4E 75 4C EF 00 03 NuD.aDD. .NuL...
00002640 00 04 61 36 20 01 4E 75                         ..a6 .Nu        

;; fn00002648: 00002648
;;   Called from:
;;     00002FFA (in fn00002B8C)
;;     0000307E (in fn00002B8C)
fn00002648 proc
	movem.l	$0004(a7),d0-d1
	tst.l	d0
	bpl	$00002668

l00002652:
	neg.l	d0
	tst.l	d1
	bpl	$00002660

l00002658:
	neg.l	d1
	bsr	$0000267A
	neg.l	d1
	rts

l00002660:
	bsr	$0000267A
	neg.l	d0
	neg.l	d1
	rts

l00002668:
	tst.l	d1
	bpl	$0000267A

l0000266C:
	neg.l	d1
	bsr	$0000267A
	neg.l	d0
	rts
00002674             4C EF 00 03 00 04                       L.....      

;; fn0000267A: 0000267A
;;   Called from:
;;     000024DA (in fn000024BC)
;;     0000265A (in fn00002648)
;;     00002660 (in fn00002648)
;;     0000266A (in fn00002648)
;;     0000266E (in fn00002648)
fn0000267A proc
	move.l	d2,-(a7)
	swap.l	d1
	move.w	d1,d2
	bne	$000026A0

l00002682:
	swap.l	d0
	swap.l	d1
	swap.l	d2
	move.w	d0,d2
	beq	$00002690

l0000268C:
	divu.w	d1,d2
	move.w	d2,d0

l00002690:
	swap.l	d0
	move.w	d0,d2
	divu.w	d1,d2
	move.w	d2,d0
	swap.l	d2
	move.w	d2,d1
	move.l	(a7)+,d2
	rts

l000026A0:
	move.l	d3,-(a7)
	moveq	#$10,d3
	cmp.w	#$0080,d1
	bcc	$000026AE

l000026AA:
	rol.l	#$08,d1
	subq.w	#$08,d3

l000026AE:
	cmp.w	#$0800,d1
	bcc	$000026B8

l000026B4:
	rol.l	#$04,d1
	subq.w	#$04,d3

l000026B8:
	cmp.w	#$2000,d1
	bcc	$000026C2

l000026BE:
	rol.l	#$02,d1
	subq.w	#$02,d3

l000026C2:
	tst.w	d1
	bmi	$000026CA

l000026C6:
	rol.l	#$01,d1
	subq.w	#$01,d3

l000026CA:
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
	bcc	$000026EC

l000026E6:
	subq.w	#$01,d3
	add.l	d1,d0

l000026EA:
	bcc	$000026EA

l000026EC:
	moveq	#$00,d1
	move.w	d3,d1
	swap.l	d3
	rol.l	d3,d0
	swap.l	d0
	exg	d0,d1
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	rts
000026FE                                           00 00               ..

;; fn00002700: 00002700
;;   Called from:
;;     000019C0 (in fn00001418)
fn00002700 proc
	movem.l	d2-d7,-(a7)
	move.l	$0020(a7),d1
	move.l	$001C(a7),d0
	movea.l	d1,a0
	move.l	$0028(a7),d3
	move.l	$0024(a7),d2
	bne	$00002752

l00002718:
	cmp.l	d3,d0
	bcc	$0000272A

l0000271C:
	move.l	d3,d2
	jsr.l	$000024BC
	moveq	#$00,d0
	bra	$0000280C

l0000272A:
	tst.l	d3
	bne	$00002736

l0000272E:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l00002736:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	$000024BC
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	$000024BC
	moveq	#$00,d0
	bra	$0000280C

l00002752:
	cmp.l	d2,d0
	bcs	$0000280C

l00002758:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002776

l00002762:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002776

l0000276A:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002776

l00002772:
	moveq	#$00,d4
	move.b	d2,d6

l00002776:
	lea	$00003DAC,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$00002798

l0000278A:
	cmp.l	d0,d2
	bcs	$00002792

l0000278E:
	cmp.l	d1,d3
	bhi	$0000280C

l00002792:
	sub.l	d3,d1
	subx.l	d2,d0
	bra	$0000280C

l00002798:
	lsl.l	d5,d2
	move.l	d3,d6
	lsr.l	d4,d6
	or.l	d6,d2
	lsl.l	d5,d3
	move.l	d0,d1
	lsr.l	d4,d0
	lsl.l	d5,d1
	move.l	a0,d6
	lsr.l	d4,d6
	or.l	d6,d1
	move.l	a0,d6
	lsl.l	d5,d6
	movea.l	d6,a0
	jsr.l	$000024BC
	exg	d2,d0
	movea.l	d3,a1
	move.l	d2,d4
	move.l	d2,d7
	move.l	d3,d6
	mulu.w	d3,d2
	swap.l	d7
	mulu.w	d7,d3
	swap.l	d6
	mulu.w	d6,d4
	mulu.w	d6,d7
	moveq	#$00,d6
	swap.l	d2
	add.w	d3,d2
	addx.w	d6,d7
	add.w	d4,d2
	addx.w	d6,d7
	swap.l	d2
	clr.w	d3
	clr.w	d4
	swap.l	d3
	swap.l	d4
	add.l	d4,d3
	add.l	d7,d3
	cmp.l	d2,d1
	bcs	$000027F4

l000027EE:
	bne	$000027F8

l000027F0:
	cmpa.l	d3,a0
	bcc	$000027F8

l000027F4:
	sub.l	a1,d3
	subx.l	d0,d2

l000027F8:
	move.l	a0,d6
	sub.l	d3,d6
	subx.l	d2,d1
	move.l	d1,d0
	moveq	#$20,d4
	sub.l	d5,d4
	lsl.l	d4,d1
	lsr.l	d5,d6
	or.l	d6,d1
	lsr.l	d5,d0

l0000280C:
	movem.l	(a7)+,d2-d7
	rts
00002812       00 00 00 20 20 20 20 20 20 20 20 20 28 28   ...         ((
00002820 28 28 28 20 20 20 20 20 20 20 20 20 20 20 20 20 (((             
00002830 20 20 20 20 20 88 10 10 10 10 10 10 10 10 10 10      ...........
00002840 10 10 10 10 10 04 04 04 04 04 04 04 04 04 04 10 ................
00002850 10 10 10 10 10 10 41 41 41 41 41 41 01 01 01 01 ......AAAAAA....
00002860 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 ................
00002870 10 10 10 10 10 10 42 42 42 42 42 42 02 02 02 02 ......BBBBBB....
00002880 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 ................
00002890 10 10 10 10 20 00 00 00 00 00 00 00 00 00 00 00 .... ...........
000028A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00002910 00 00 00 00 00 00 00 00 48 E7 00 32 48 78 00 20 ........H..2Hx. 
00002920 4E B9 00 00 1F B4 23 C0 00 00 3E CC 48 78 00 20 N.....#...>.Hx. 
00002930 4E B9 00 00 1F B4 23 C0 00 00 3E D0 48 78 00 20 N.....#...>.Hx. 
00002940 4E B9 00 00 1F B4 23 C0 00 00 3E D4 4F EF 00 0C N.....#...>.O...
00002950 4A B9 00 00 3E CC 67 10 4A B9 00 00 3E D0 67 08 J...>.g.J...>.g.
00002960 4A B9 00 00 3E D4 66 0C 48 78 00 14 4E B9 00 00 J...>.f.Hx..N...
00002970 13 1C 58 4F 20 79 00 00 3E CC 20 B9 00 00 3D 84 ..XO y..>. ...=.
00002980 20 79 00 00 3E CC 70 20 21 40 00 18 22 39 00 00  y..>.p !@.."9..
00002990 3D 84 2C 79 00 00 3D 78 4E AE FF 28 4A 80 67 10 =.,y..=xN..(J.g.
000029A0 20 79 00 00 3E CC 41 E8 00 18 00 90 00 00 02 04  y..>.A.........
000029B0 20 79 00 00 3E D0 20 B9 00 00 3D 88 20 79 00 00  y..>. ...=. y..
000029C0 3E D0 70 40 21 40 00 18 22 39 00 00 3D 88 2C 79 >.p@!@.."9..=.,y
000029D0 00 00 3D 78 4E AE FF 28 4A 80 67 10 20 79 00 00 ..=xN..(J.g. y..
000029E0 3E D0 41 E8 00 18 00 90 00 00 02 80 20 79 00 00 >.A......... y..
000029F0 3E D4 20 B9 00 00 3D 8C 20 79 00 00 3E D4 70 40 >. ...=. y..>.p@
00002A00 21 40 00 18 22 39 00 00 3D 8C 2C 79 00 00 3D 78 !@.."9..=.,y..=x
00002A10 4E AE FF 28 4A 80 67 10 20 79 00 00 3E D4 41 E8 N..(J.g. y..>.A.
00002A20 00 18 00 90 00 00 02 80 20 79 00 00 3E D4 42 A8 ........ y..>.B.
00002A30 00 04 20 79 00 00 3E D0 42 A8 00 04 20 79 00 00 .. y..>.B... y..
00002A40 3E CC 42 A8 00 04 20 79 00 00 3E D4 42 A8 00 08 >.B... y..>.B...
00002A50 20 79 00 00 3E D0 42 A8 00 08 20 79 00 00 3E CC  y..>.B... y..>.
00002A60 42 A8 00 08 24 79 00 00 3E D4 42 AA 00 14 22 79 B...$y..>.B..."y
00002A70 00 00 3E D0 42 A9 00 14 20 79 00 00 3E CC 42 A8 ..>.B... y..>.B.
00002A80 00 14 42 AA 00 1C 42 A9 00 1C 42 A8 00 1C 42 A8 ..B...B...B...B.
00002A90 00 10 20 79 00 00 3E CC 21 79 00 00 3E D0 00 0C .. y..>.!y..>...
00002AA0 20 79 00 00 3E D0 21 79 00 00 3E CC 00 10 20 79  y..>.!y..>... y
00002AB0 00 00 3E D0 21 79 00 00 3E D4 00 0C 20 79 00 00 ..>.!y..>... y..
00002AC0 3E D4 21 79 00 00 3E D0 00 10 20 79 00 00 3E D4 >.!y..>... y..>.
00002AD0 42 A8 00 0C 23 F9 00 00 3E CC 00 00 3E D8 23 F9 B...#...>...>.#.
00002AE0 00 00 3E D4 00 00 3E DC 4C DF 4C 00 4E 75 00 00 ..>...>.L.L.Nu..
00002AF0 42 A7 4E B9 00 00 2A FC 58 4F 4E 75             B.N...*.XONu    

;; fn00002AFC: 00002AFC
fn00002AFC proc
	movem.l	a2,-(a7)
	movea.l	$0008(a7),a2
	move.l	a2,d0
	beq	$00002B14

l00002B08:
	move.l	a2,-(a7)
	jsr.l	$00001F08
	addq.w	#$04,a7
	bra	$00002B3A

l00002B14:
	movea.l	$00003ED8,a2
	move.l	a2,d0
	beq	$00002B3A

l00002B1E:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002B30

l00002B26:
	move.l	a2,-(a7)
	jsr.l	$00001F08
	addq.w	#$04,a7

l00002B30:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00002B1E

l00002B3A:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	rts

;; fn00002B40: 00002B40
;;   Called from:
;;     0000139C (in fn00001390)
;;     000013AC (in fn00001390)
;;     000013C2 (in fn00001390)
fn00002B40 proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00003ECC,-(a7)
	jsr.l	$00002B8C
	lea	$000C(a7),a7
	rts

;; fn00002B5C: 00002B5C
;;   Called from:
;;     00002E96 (in fn00002B8C)
;;     000030B2 (in fn00002B8C)
;;     00003198 (in fn00002B8C)
;;     0000321A (in fn00002B8C)
;;     0000345A (in fn00002B8C)
;;     00003478 (in fn00002B8C)
;;     000035D2 (in fn00002B8C)
;;     000035EC (in fn00002B8C)
;;     000038B8 (in fn00002B8C)
;;     000038D2 (in fn00002B8C)
;;     00003B0C (in fn00002B8C)
;;     00003B78 (in fn00002B8C)
fn00002B5C proc
	movem.l	a2,-(a7)
	movea.l	$000C(a7),a2
	move.l	a2,d0
	beq	$00002B86

l00002B68:
	move.l	$0004(a2),d0
	cmp.l	$0008(a2),d0
	bcc	$00002B7A

l00002B72:
	movea.l	$0004(a2),a0
	move.b	$000B(a7),(a0)

l00002B7A:
	lea	$0014(a2),a0
	addq.l	#$01,(a0)
	lea	$0004(a2),a0
	subq.l	#$01,(a0)

l00002B86:
	movea.l	(a7)+,a2
	rts
00002B8A                               00 00                       ..    

;; fn00002B8C: 00002B8C
;;   Called from:
;;     00002B50 (in fn00002B40)
fn00002B8C proc
	lea	-$004C(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$0084(a7),d2
	movea.l	$0080(a7),a4
	movea.l	$007C(a7),a2
	clr.l	$003C(a7)
	moveq	#$00,d4
	moveq	#$00,d5
	tst.b	(a4)
	beq	$00003B8E

l00002BAE:
	moveq	#$00,d3
	cmpi.b	#$25,(a4)
	bne	$00003A8C

l00002BB8:
	moveq	#-$01,d6
	move.b	#$69,$0048(a7)
	clr.b	$0049(a7)
	lea	$0001(a4),a3
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00002C3C

l00002BE0:
	moveq	#$00,d6
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00002C3C

l00002BFA:
	moveq	#$0A,d0
	move.l	d2,-(a7)
	move.l	d0,d1
	move.l	d6,d2
	swap.l	d1
	swap.l	d2
	mulu.w	d6,d1
	mulu.w	d0,d2
	mulu.w	d6,d0
	add.w	d2,d1
	swap.l	d1
	clr.w	d1
	add.l	d1,d0
	move.l	(a7)+,d2
	moveq	#$00,d1
	move.b	(a3)+,d1
	sub.l	#$00000030,d1
	move.l	d1,d6
	add.l	d0,d6
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00002BFA

l00002C3C:
	cmpi.b	#$68,(a3)
	beq	$00002C66

l00002C42:
	cmpi.b	#$6C,(a3)
	beq	$00002C66

l00002C48:
	cmpi.b	#$4C,(a3)
	beq	$00002C66

l00002C4E:
	cmpi.b	#$7A,(a3)
	beq	$00002C66

l00002C54:
	cmpi.b	#$6A,(a3)
	beq	$00002C66

l00002C5A:
	cmpi.b	#$74,(a3)
	beq	$00002C66

l00002C60:
	cmpi.b	#$2A,(a3)
	bne	$00002CCE

l00002C66:
	move.b	$0049(a7),d7
	move.b	$0048(a7),d1

l00002C6E:
	cmpi.b	#$2A,(a3)
	bne	$00002C78

l00002C74:
	moveq	#$01,d7
	bra	$00002C9A

l00002C78:
	cmp.b	#$68,d1
	bne	$00002C88

l00002C7E:
	cmpi.b	#$68,(a3)
	bne	$00002C88

l00002C84:
	moveq	#$02,d1
	bra	$00002C9A

l00002C88:
	cmp.b	#$6C,d1
	bne	$00002C98

l00002C8E:
	cmpi.b	#$6C,(a3)
	bne	$00002C98

l00002C94:
	moveq	#$01,d1
	bra	$00002C9A

l00002C98:
	move.b	(a3),d1

l00002C9A:
	addq.l	#$01,a3
	cmpi.b	#$68,(a3)
	beq	$00002C6E

l00002CA2:
	cmpi.b	#$6C,(a3)
	beq	$00002C6E

l00002CA8:
	cmpi.b	#$4C,(a3)
	beq	$00002C6E

l00002CAE:
	cmpi.b	#$7A,(a3)
	beq	$00002C6E

l00002CB4:
	cmpi.b	#$6A,(a3)
	beq	$00002C6E

l00002CBA:
	cmpi.b	#$74,(a3)
	beq	$00002C6E

l00002CC0:
	cmpi.b	#$2A,(a3)
	beq	$00002C6E

l00002CC6:
	move.b	d1,$0048(a7)
	move.b	d7,$0049(a7)

l00002CCE:
	cmpi.b	#$6A,$0048(a7)
	bne	$00002CDC

l00002CD6:
	move.b	#$01,$0048(a7)

l00002CDC:
	cmpi.b	#$74,$0048(a7)
	bne	$00002CEA

l00002CE4:
	move.b	#$69,$0048(a7)

l00002CEA:
	cmpi.b	#$7A,$0048(a7)
	bne	$00002CF8

l00002CF2:
	move.b	#$6C,$0048(a7)

l00002CF8:
	move.b	(a3)+,d7
	beq	$00002D6E

l00002CFC:
	cmp.b	#$25,d7
	beq	$00002D6E

l00002D02:
	cmp.b	#$63,d7
	beq	$00002D6E

l00002D08:
	cmp.b	#$6E,d7
	beq	$00002D6E

l00002D0E:
	cmp.b	#$5B,d7
	beq	$00002D6E

l00002D14:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00002D3E

l00002D26:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00002D4A

l00002D3E:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,d1
	addq.w	#$04,a7

l00002D4A:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00002D14

l00002D6C:
	moveq	#$01,d3

l00002D6E:
	move.b	d7,d1
	sub.b	#$25,d1
	beq	$000031BA

l00002D78:
	sub.b	#$36,d1
	beq	$00002EB6

l00002D80:
	subq.b	#$08,d1
	beq	$00002D96

l00002D84:
	sub.b	#$0B,d1
	beq	$00003228

l00002D8C:
	subq.b	#$05,d1
	beq	$000030D4

l00002D92:
	bra	$000032CA

l00002D96:
	cmp.l	#$FFFFFFFF,d6
	bne	$00002DA0

l00002D9E:
	moveq	#$01,d6

l00002DA0:
	tst.b	$0049(a7)
	bne	$00002DBA

l00002DA6:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a1
	bra	$00002DBC

l00002DBA:
	suba.l	a1,a1

l00002DBC:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	move.l	a1,$002C(a7)
	tst.l	(a0)
	blt	$00002DF4

l00002DD4:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0038(a7)
	move.l	(a7)+,d1
	bra	$00002E12

l00002DF4:
	movea.l	$002C(a7),a1
	move.l	a2,-(a7)
	move.l	a1,$0030(a7)
	jsr.l	$00003C30
	move.l	d0,$0038(a7)
	movea.l	$0030(a7),a1
	move.l	a1,$0030(a7)
	addq.w	#$04,a7

l00002E12:
	movea.l	$002C(a7),a1
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$0034(a7)
	beq	$00002E8A

l00002E28:
	move.l	a1,$002C(a7)
	cmp.l	d3,d6
	bcs	$00002E8A

l00002E30:
	move.b	$0049(a7),d7
	movea.l	$002C(a7),a4

l00002E38:
	tst.b	d7
	bne	$00002E3E

l00002E3C:
	move.b	d5,(a4)+

l00002E3E:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00002E68

l00002E50:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00002E74

l00002E68:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,d1
	addq.w	#$04,a7

l00002E74:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00002E86

l00002E82:
	cmp.l	d3,d6
	bcc	$00002E38

l00002E86:
	move.b	d7,$0049(a7)

l00002E8A:
	cmp.l	#$FFFFFFFF,d5
	beq	$00002E9C

l00002E92:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l00002E9C:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003A86

l00002EA8:
	tst.l	d3
	beq	$00003A86

l00002EAE:
	addq.l	#$01,$003C(a7)
	bra	$00003A86

l00002EB6:
	clr.b	$002C(a7)
	cmpi.b	#$5E,(a3)
	bne	$00002EC8

l00002EC0:
	move.b	#$01,$002C(a7)
	addq.l	#$01,a3

l00002EC8:
	clr.l	$0034(a7)
	move.b	$002C(a7),d7
	move.l	$0034(a7),d1

l00002ED4:
	tst.b	d7
	beq	$00002EE0

l00002ED8:
	move.l	#$000000FF,d5
	bra	$00002EE2

l00002EE0:
	moveq	#$00,d5

l00002EE2:
	lea	$004E(a7),a0
	move.b	d5,(a0,d1)
	addq.l	#$01,d1
	cmp.l	#$00000020,d1
	bcs	$00002ED4

l00002EF4:
	move.l	d2,$0084(a7)
	move.b	d7,$002C(a7)
	move.b	$002C(a7),d2

l00002F00:
	tst.b	(a3)
	beq	$00002F76

l00002F04:
	move.b	(a3)+,d1
	cmpi.b	#$2D,(a3)
	bne	$00002F18

l00002F0C:
	cmp.b	$0001(a3),d1
	bcc	$00002F18

l00002F12:
	addq.l	#$01,a3
	move.b	(a3)+,d7
	bra	$00002F1A

l00002F18:
	move.b	d1,d7

l00002F1A:
	moveq	#$00,d5
	move.b	d1,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcs	$00002F70

l00002F26:
	tst.b	d2
	beq	$00002F4A

l00002F2A:
	lea	$004E(a7),a0
	move.l	d5,d0
	lsr.l	#$03,d0
	adda.l	d0,a0
	moveq	#$07,d0
	and.l	d5,d0
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	not.l	d0
	moveq	#$00,d1
	move.b	(a0),d1
	and.l	d1,d0
	move.b	d0,(a0)
	bra	$00002F66

l00002F4A:
	lea	$004E(a7),a0
	move.l	d5,d0
	lsr.l	#$03,d0
	adda.l	d0,a0
	moveq	#$07,d0
	and.l	d5,d0
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	moveq	#$00,d1
	move.b	(a0),d1
	or.l	d1,d0
	move.b	d0,(a0)

l00002F66:
	addq.l	#$01,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcc	$00002F26

l00002F70:
	cmpi.b	#$5D,(a3)
	bne	$00002F00

l00002F76:
	move.l	$0084(a7),d2
	addq.l	#$01,a3
	tst.b	$0049(a7)
	bne	$00002F96

l00002F82:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a6
	bra	$00002F98

l00002F96:
	suba.l	a6,a6

l00002F98:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00002FCA

l00002FAA:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0030(a7)
	move.l	(a7)+,d1
	bra	$00002FD8

l00002FCA:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00002FD8:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$002C(a7)
	beq	$000030A6

l00002FEC:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	$00002648
	addq.w	#$08,a7
	movea.l	(a7)+,a1
	movea.l	(a7)+,a0
	adda.l	d0,a0
	moveq	#$07,d0
	and.l	d5,d0
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	moveq	#$00,d1
	move.b	(a0),d1
	and.l	d1,d0
	beq	$000030A6

l0000301C:
	cmp.l	d3,d6
	bcs	$000030A6

l00003022:
	move.b	$0049(a7),d7

l00003026:
	tst.b	d7
	bne	$0000302C

l0000302A:
	move.b	d5,(a6)+

l0000302C:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003056

l0000303E:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00003062

l00003056:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,d1
	addq.w	#$04,a7

l00003062:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$000030A2

l00003070:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	$00002648
	addq.w	#$08,a7
	movea.l	(a7)+,a1
	movea.l	(a7)+,a0
	adda.l	d0,a0
	moveq	#$07,d0
	and.l	d5,d0
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	moveq	#$00,d1
	move.b	(a0),d1
	and.l	d1,d0
	beq	$000030A2

l0000309E:
	cmp.l	d3,d6
	bcc	$00003026

l000030A2:
	move.b	d7,$0049(a7)

l000030A6:
	cmp.l	#$FFFFFFFF,d5
	beq	$000030B8

l000030AE:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l000030B8:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003A86

l000030C4:
	tst.l	d3
	beq	$00003A86

l000030CA:
	clr.b	(a6)+
	addq.l	#$01,$003C(a7)
	bra	$00003A86

l000030D4:
	tst.b	$0049(a7)
	bne	$000030EE

l000030DA:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a5
	bra	$000030F0

l000030EE:
	suba.l	a5,a5

l000030F0:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000318C

l000030FA:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$0000318C

l00003116:
	cmp.l	d3,d6
	bcs	$0000318C

l0000311A:
	move.b	$0049(a7),d7

l0000311E:
	tst.b	d7
	bne	$00003124

l00003122:
	move.b	d5,(a5)+

l00003124:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000314E

l00003136:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$0000315A

l0000314E:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,d1
	addq.w	#$04,a7

l0000315A:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00003188

l00003168:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003188

l00003184:
	cmp.l	d3,d6
	bcc	$0000311E

l00003188:
	move.b	d7,$0049(a7)

l0000318C:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000319E

l00003194:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l0000319E:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003A86

l000031AA:
	tst.l	d3
	beq	$00003A86

l000031B0:
	clr.b	(a5)+
	addq.l	#$01,$003C(a7)
	bra	$00003A86

l000031BA:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000031EC

l000031CC:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0030(a7)
	move.l	(a7)+,d1
	bra	$000031FA

l000031EC:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l000031FA:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$00000025,$002C(a7)
	beq	$00003A86

l0000320E:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003220

l00003216:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l00003220:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00003A86

l00003228:
	tst.b	$0049(a7)
	bne	$000032C0

l00003230:
	cmpi.b	#$01,$0048(a7)
	bne	$00003252

l00003238:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,$0004(a0)
	clr.l	(a0)
	bra	$000032C0

l00003252:
	cmpi.b	#$6C,$0048(a7)
	bne	$00003270

l0000325A:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)
	bra	$000032C0

l00003270:
	cmpi.b	#$68,$0048(a7)
	bne	$0000328E

l00003278:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.w	d4,(a0)
	bra	$000032C0

l0000328E:
	cmpi.b	#$02,$0048(a7)
	bne	$000032AC

l00003296:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.b	d4,(a0)
	bra	$000032C0

l000032AC:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)

l000032C0:
	moveq	#$01,d3
	addq.l	#$01,$003C(a7)
	bra	$00003A86

l000032CA:
	clr.l	$0030(a7)
	clr.l	$002C(a7)
	clr.l	$006E(a7)
	tst.b	d7
	bne	$000032DC

l000032DA:
	subq.l	#$01,a3

l000032DC:
	cmp.b	#$70,d7
	bne	$000032EA

l000032E2:
	move.b	#$6C,$0048(a7)
	moveq	#$78,d7

l000032EA:
	cmp.l	#$0000002D,d5
	bne	$000032F8

l000032F2:
	cmp.b	#$75,d7
	bne	$00003300

l000032F8:
	cmp.l	#$0000002B,d5
	bne	$00003350

l00003300:
	cmp.l	d3,d6
	bcs	$00003350

l00003304:
	move.l	d5,$006E(a7)
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000333A

l0000331A:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0038(a7)
	move.l	(a7)+,d1
	bra	$00003348

l0000333A:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003348:
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4

l00003350:
	cmp.b	#$69,d7
	bne	$000034C2

l00003358:
	cmp.l	#$00000030,d5
	bne	$00003484

l00003362:
	cmp.l	d3,d6
	bcs	$00003484

l00003368:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000339A

l0000337A:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0038(a7)
	move.l	(a7)+,d1
	bra	$000033A8

l0000339A:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l000033A8:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$00002814,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$000033CE

l000033CA:
	or.b	#$20,d0

l000033CE:
	cmp.l	#$00000078,d0
	bne	$00003466

l000033D8:
	cmp.l	d3,d6
	bcs	$00003466

l000033DE:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003410

l000033F0:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0038(a7)
	move.l	(a7)+,d1
	bra	$0000341E

l00003410:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l0000341E:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$0000344A

l00003444:
	cmp.l	d3,d6
	bcs	$0000344A

l00003448:
	moveq	#$78,d7

l0000344A:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$00003460

l00003454:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l00003460:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00003468

l00003466:
	moveq	#$6F,d7

l00003468:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$0000347E

l00003472:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l0000347E:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$000034C2

l00003484:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$000034C2

l000034A0:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$000034C2

l000034BC:
	cmp.l	d3,d6
	bcs	$000034C2

l000034C0:
	moveq	#$78,d7

l000034C2:
	cmp.b	#$78,d7
	bne	$000035F6

l000034CA:
	cmp.l	#$00000030,d5
	bne	$000035F6

l000034D4:
	cmp.l	d3,d6
	bcs	$000035F6

l000034DA:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000350C

l000034EC:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0038(a7)
	move.l	(a7)+,d1
	bra	$0000351A

l0000350C:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l0000351A:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$00002814,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$00003540

l0000353C:
	or.b	#$20,d0

l00003540:
	cmp.l	#$00000078,d0
	bne	$000035DC

l0000354A:
	cmp.l	d3,d6
	bcs	$000035DC

l00003550:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003582

l00003562:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0038(a7)
	move.l	(a7)+,d1
	bra	$00003590

l00003582:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003590:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$000035C2

l000035B8:
	cmp.l	d3,d6
	bcs	$000035C2

l000035BC:
	move.l	$004A(a7),d5
	bra	$000035F6

l000035C2:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$000035D8

l000035CC:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l000035D8:
	subq.l	#$01,d3
	subq.l	#$01,d4

l000035DC:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$000035F2

l000035E6:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l000035F2:
	subq.l	#$01,d3
	subq.l	#$01,d4

l000035F6:
	cmp.b	#$78,d7
	beq	$00003602

l000035FC:
	cmp.b	#$58,d7
	bne	$0000360C

l00003602:
	move.l	#$00000010,$0040(a7)
	bra	$0000362A

l0000360C:
	cmp.b	#$6F,d7
	bne	$0000361C

l00003612:
	move.l	#$00000008,$0034(a7)
	bra	$00003624

l0000361C:
	move.l	#$0000000A,$0034(a7)

l00003624:
	move.l	$0034(a7),$0040(a7)

l0000362A:
	move.l	$0040(a7),$0072(a7)
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$00003892

l0000365C:
	cmpi.l	#$0000000A,$0072(a7)
	bne	$00003692

l00003666:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$00003892

l00003692:
	cmpi.l	#$00000008,$0072(a7)
	bne	$000036B2

l0000369C:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	cmp.l	#$00000037,d5
	bgt	$00003892

l000036B2:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.l	d6,$0040(a7)
	move.b	d7,$004A(a7)
	cmp.l	d3,d6
	bcs	$00003892

l000036C8:
	move.l	$0072(a7),d7
	movea.l	$0040(a7),a4

l000036D0:
	move.l	d7,d1
	move.l	d1,d0
	move.l	d2,-(a7)
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	(a7)+,d2
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	a1,-(a7)
	move.l	$003C(a7),-(a7)
	move.l	$003C(a7),-(a7)
	movem.l	d0-d1,-(a7)
	jsr.l	$00003BB0
	lea	$0010(a7),a7
	movea.l	(a7)+,a1
	move.l	d0,$0048(a7)
	move.l	d1,$004C(a7)
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00003730

l00003726:
	move.l	d5,d4
	sub.l	#$00000030,d4
	bra	$00003732

l00003730:
	moveq	#$00,d4

l00003732:
	move.l	d4,d1
	move.l	d1,d0
	move.l	d2,-(a7)
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	(a7)+,d2
	move.l	d2,-(a7)
	move.l	d1,d2
	add.l	$0048(a7),d2
	move.l	d2,$0034(a7)
	move.l	d0,d2
	move.l	d3,-(a7)
	move.l	$0048(a7),d3
	addx.l	d3,d2
	move.l	d2,$0034(a7)
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000001,d0
	beq	$00003782

l00003778:
	move.l	d5,d6
	sub.l	#$00000037,d6
	bra	$00003784

l00003782:
	moveq	#$00,d6

l00003784:
	move.l	d6,d1
	move.l	d1,d0
	move.l	d2,-(a7)
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	(a7)+,d2
	move.l	d2,-(a7)
	move.l	d1,d2
	add.l	$0034(a7),d2
	move.l	d2,$0048(a7)
	move.l	d0,d2
	move.l	d3,-(a7)
	move.l	$0034(a7),d3
	addx.l	d3,d2
	move.l	d2,$0048(a7)
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000002,d0
	beq	$000037D4

l000037CA:
	move.l	d5,d2
	sub.l	#$00000057,d2
	bra	$000037D6

l000037D4:
	moveq	#$00,d2

l000037D6:
	move.l	d2,d1
	move.l	d1,d0
	move.l	d2,-(a7)
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	(a7)+,d2
	move.l	d2,-(a7)
	move.l	d1,d2
	add.l	$0048(a7),d2
	move.l	d2,$0034(a7)
	move.l	d0,d2
	move.l	d3,-(a7)
	move.l	$0048(a7),d3
	addx.l	d3,d2
	move.l	d2,$0034(a7)
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	lea	$0018(a2),a0
	moveq	#$01,d0
	or.l	d0,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003828

l00003810:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00003834

l00003828:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,d1
	addq.w	#$04,a7

l00003834:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,$0034(a7)
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$00003892

l00003858:
	cmp.l	#$0000000A,d7
	bne	$0000387C

l00003860:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00003892

l0000387C:
	cmp.l	#$00000008,d7
	bne	$0000388C

l00003884:
	cmp.l	#$00000037,d5
	bgt	$00003892

l0000388C:
	cmpa.l	d3,a4
	bcc	$000036D0

l00003892:
	move.b	$004A(a7),d7
	move.l	$0034(a7),d4
	move.l	$0084(a7),d2
	tst.l	$006E(a7)
	beq	$000038C6

l000038A4:
	cmp.l	#$00000002,d3
	bne	$000038C6

l000038AC:
	cmp.l	#$FFFFFFFF,d5
	beq	$000038BE

l000038B4:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l000038BE:
	subq.l	#$01,d3
	subq.l	#$01,d4
	move.l	$006E(a7),d5

l000038C6:
	cmp.l	#$FFFFFFFF,d5
	beq	$000038D8

l000038CE:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l000038D8:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003A86

l000038E4:
	tst.l	d3
	beq	$00003A86

l000038EA:
	cmp.b	#$75,d7
	bne	$000039AC

l000038F2:
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0038(a7)
	move.l	(a7)+,d0
	tst.b	$0034(a7)
	beq	$0000391C

l00003906:
	subq.b	#$01,$0034(a7)
	beq	$00003974

l0000390C:
	subi.b	#$66,$0034(a7)
	beq	$00003958

l00003914:
	subq.b	#$04,$0034(a7)
	beq	$0000393C

l0000391A:
	bra	$00003990

l0000391C:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	$0030(a7),$0004(a0)
	move.l	$002C(a7),(a0)
	bra	$00003A82

l0000393C:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	$0030(a7),d0
	move.l	d0,(a0)
	bra	$00003A82

l00003958:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	$0030(a7),d0
	move.w	d0,(a0)
	bra	$00003A82

l00003974:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	$0030(a7),d0
	move.b	d0,(a0)
	bra	$00003A82

l00003990:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	$0030(a7),d0
	move.l	d0,(a0)
	bra	$00003A82

l000039AC:
	cmpi.l	#$0000002D,$006E(a7)
	bne	$000039C8

l000039B6:
	movem.l	$002C(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0034(a7)
	bra	$000039D4

l000039C8:
	move.l	$0030(a7),$0038(a7)
	move.l	$002C(a7),$0034(a7)
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0030(a7)
	move.l	(a7)+,d0
	tst.b	$002C(a7)
	beq	$000039FE

l000039E8:
	subq.b	#$01,$002C(a7)
	beq	$00003A50

l000039EE:
	subi.b	#$66,$002C(a7)
	beq	$00003A36

l000039F6:
	subq.b	#$04,$002C(a7)
	beq	$00003A1C

l000039FC:
	bra	$00003A6A

l000039FE:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	$0038(a7),$0004(a0)
	move.l	$0034(a7),(a0)
	bra	$00003A82

l00003A1C:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	$0038(a7),d0
	move.l	d0,(a0)
	bra	$00003A82

l00003A36:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	$0038(a7),d0
	move.w	d0,(a0)
	bra	$00003A82

l00003A50:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	$0038(a7),d0
	move.b	d0,(a0)
	bra	$00003A82

l00003A6A:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	$0038(a7),d0
	move.l	d0,(a0)

l00003A82:
	addq.l	#$01,$003C(a7)

l00003A86:
	movea.l	a3,a4
	bra	$00003B84

l00003A8C:
	move.b	(a4),d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	beq	$00003B18

l00003AA8:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003AD2

l00003ABA:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00003ADE

l00003AD2:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,d1
	addq.w	#$04,a7

l00003ADE:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$00002815,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003AA8

l00003B00:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003B12

l00003B08:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l00003B12:
	subq.l	#$01,d4
	moveq	#$01,d3
	bra	$00003B82

l00003B18:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003B4A

l00003B2A:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	move.l	a2,-(a7)
	movea.l	a0,a2
	addq.l	#$01,a2
	move.l	a2,(a1)
	movea.l	(a7)+,a2
	move.b	(a0),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0030(a7)
	move.l	(a7)+,d1
	bra	$00003B58

l00003B4A:
	move.l	a2,-(a7)
	jsr.l	$00003C30
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00003B58:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	(a4),d0
	ext.w	d0
	ext.l	d0
	cmp.l	$002C(a7),d0
	beq	$00003B82

l00003B6C:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003B7E

l00003B74:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002B5C
	addq.w	#$08,a7

l00003B7E:
	subq.l	#$01,d3
	subq.l	#$01,d4

l00003B82:
	addq.l	#$01,a4

l00003B84:
	tst.l	d3
	beq	$00003B8E

l00003B88:
	tst.b	(a4)
	bne	$00002BAE

l00003B8E:
	cmp.l	#$FFFFFFFF,d5
	bne	$00003BA0

l00003B96:
	tst.l	$003C(a7)
	bne	$00003BA0

l00003B9C:
	move.l	d5,d0
	bra	$00003BA4

l00003BA0:
	move.l	$003C(a7),d0

l00003BA4:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$004C(a7),a7
	rts
00003BAE                                           00 00               ..

;; fn00003BB0: 00003BB0
;;   Called from:
;;     000036EE (in fn00002B8C)
fn00003BB0 proc
	movem.l	d2-d6,-(a7)
	move.l	$001C(a7),d5
	move.l	$0024(a7),d6
	move.l	d5,d0
	move.l	d6,d1
	move.l	d0,d2
	swap.l	d0
	move.l	d1,d3
	swap.l	d1
	move.w	d2,d4
	mulu.w	d3,d4
	mulu.w	d1,d2
	mulu.w	d0,d3
	mulu.w	d0,d1
	move.l	d4,d0
	eor.w	d0,d0
	swap.l	d0
	add.l	d0,d2
	add.l	d3,d2
	bcc	$00003BE4

l00003BDE:
	add.l	#$00010000,d1

l00003BE4:
	swap.l	d2
	moveq	#$00,d0
	move.w	d2,d0
	move.w	d4,d2
	add.l	d1,d0
	move.l	d2,d1
	move.l	$0020(a7),d2
	move.l	d2,d3
	move.l	d5,d4
	swap.l	d3
	swap.l	d4
	mulu.w	d5,d3
	mulu.w	d2,d4
	mulu.w	d5,d2
	add.w	d4,d3
	swap.l	d3
	eor.w	d3,d3
	add.l	d3,d2
	add.l	d2,d0
	move.l	$0018(a7),d2
	move.l	d2,d3
	move.l	d6,d4
	swap.l	d3
	swap.l	d4
	mulu.w	d6,d3
	mulu.w	d2,d4
	mulu.w	d6,d2
	add.w	d4,d3
	swap.l	d3
	eor.w	d3,d3
	add.l	d3,d2
	add.l	d2,d0
	movem.l	(a7)+,d2-d6
	rts
00003C2E                                           00 00               ..

;; fn00003C30: 00003C30
;;   Called from:
;;     00002D40 (in fn00002B8C)
;;     00002DFE (in fn00002B8C)
;;     00002E6A (in fn00002B8C)
;;     00002FCC (in fn00002B8C)
;;     00003058 (in fn00002B8C)
;;     00003150 (in fn00002B8C)
;;     000031EE (in fn00002B8C)
;;     0000333C (in fn00002B8C)
;;     0000339C (in fn00002B8C)
;;     00003412 (in fn00002B8C)
;;     0000350E (in fn00002B8C)
;;     00003584 (in fn00002B8C)
;;     0000382A (in fn00002B8C)
;;     00003AD4 (in fn00002B8C)
;;     00003B4C (in fn00002B8C)
fn00003C30 proc
	movem.l	d2-d5/a2-a4/a6,-(a7)
	movea.l	$0024(a7),a2
	jsr.l	$00002388
	move.l	a2,d0
	bne	$00003C48

l00003C42:
	moveq	#-$01,d0
	bra	$00003D26

l00003C48:
	moveq	#$2A,d0
	and.l	$0018(a2),d0
	moveq	#$20,d5
	cmp.l	d0,d5
	beq	$00003C5A

l00003C54:
	moveq	#-$01,d0
	bra	$00003D26

l00003C5A:
	lea	$0018(a2),a0
	moveq	#$01,d0
	or.l	d0,(a0)
	move.l	#$00000200,d0
	and.l	(a0),d0
	beq	$00003C72

l00003C6C:
	jsr.l	$00003D2C

l00003C72:
	tst.l	$001C(a2)
	bne	$00003C90

l00003C78:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00003C88

l00003C80:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00003C90

l00003C88:
	move.l	#$00000400,$001C(a2)

l00003C90:
	tst.l	$0008(a2)
	bne	$00003CCC

l00003C96:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00003CAA

l00003CA6:
	moveq	#$02,d4
	bra	$00003CAC

l00003CAA:
	moveq	#$01,d4

l00003CAC:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	$00001FB4
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00003CC4

l00003CC0:
	moveq	#-$01,d0
	bra	$00003D26

l00003CC4:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)

l00003CCC:
	lea	$0004(a2),a0
	move.l	$0008(a2),(a0)
	move.l	$001C(a2),d3
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003D78,a6
	jsr.l	-$002A(a6)
	lea	$0014(a2),a0
	move.l	d0,(a0)
	subq.l	#$01,(a0)
	bge	$00003D12

l00003CF0:
	moveq	#-$01,d0
	cmp.l	$0014(a2),d0
	bne	$00003D02

l00003CF8:
	lea	$0018(a2),a0
	moveq	#$08,d0
	or.l	d0,(a0)
	bra	$00003D0A

l00003D02:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)

l00003D0A:
	clr.l	$0014(a2)
	moveq	#-$01,d0
	bra	$00003D26

l00003D12:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	(a0),d0
	and.l	#$000000FF,d0

l00003D26:
	movem.l	(a7)+,d2-d5/a2-a4/a6
	rts

;; fn00003D2C: 00003D2C
;;   Called from:
;;     00003C6C (in fn00003C30)
fn00003D2C proc
	movem.l	a2,-(a7)
	movea.l	$00003ED8,a2
	move.l	a2,d0
	beq	$00003D64

l00003D3A:
	move.l	#$00000202,d0
	and.l	$0018(a2),d0
	cmp.l	#$00000202,d0
	bne	$00003D5A

l00003D4C:
	tst.l	(a2)
	beq	$00003D5A

l00003D50:
	move.l	a2,-(a7)
	jsr.l	$00001F08
	addq.w	#$04,a7

l00003D5A:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00003D3A

l00003D64:
	movea.l	(a7)+,a2
	rts
00003D68                         00 24 00 63 41 00 00 00         .$.cA...
00003D70 00 00 00 00 00 00 40 00 00 00 00 00 00 01 02 02 ......@.........
00003D80 03 03 03 03 04 04 04 04 04 04 04 04 05 05 05 05 ................
00003D90 05 05 05 05 05 05 05 05 05 05 05 05 06 06 06 06 ................
00003DA0 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 ................
00003DB0 06 06 06 06 06 06 06 06 06 06 06 06 07 07 07 07 ................
00003DC0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003DD0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003DE0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003DF0 07 07 07 07 07 07 07 07 07 07 07 07 08 08 08 08 ................
00003E00 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E10 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E20 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E30 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E40 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E50 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E60 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E70 08 08 08 08 08 08 08 08 08 08 08 08 00 00 00 01 ................
00003E80 00 00 29 18 00 00 00 00 00 00 00 02 00 00 1F 9C ..).............
00003E90 00 00 2A F0 00 00 00 00 00 00 00 00 00 00 00 00 ..*.............
00003EA0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
