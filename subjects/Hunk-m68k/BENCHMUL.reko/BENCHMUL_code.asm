;;; Segment code (00001000)

;; fn00001000: 00001000
fn00001000 proc
	bra	$0000100A
00001002       56 42 43 43 20 30 2E 39                     VBCC 0.9      

l0000100A:
	move.l	d0,d2
	movea.l	a0,a2
	lea	$0000C0EE,a4
	movea.l	$00000004,a6
	cmpi.w	#$0024,$0014(a6)
	bcc	$00001036

l00001020:
	lea	$000040F8,a0
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
	bsr	fn00001214
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
	bra	fn0000127C

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
	jsr.l	fn00001354
	moveq	#$00,d2
	bra	fn0000127C

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
	bsr	fn00001214
	movea.l	d0,a2
	bsr	fn0000126C

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
	lea	$0000C0EE,a4
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
	bsr	fn0000126C

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
	lea	$00004240,a3
	move.l	#$00004240,d0
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
	jsr.l	fn00001278
	addq.w	#$04,a7
	movem.l	(a7)+,d2/a2-a3
	rts
0000131A                               00 00                       ..    

;; fn0000131C: 0000131C
;;   Called from:
;;     00001382 (in fn00001354)
;;     000026AA (in fn00002688)
fn0000131C proc
	movem.l	a2-a3,-(a7)
	tst.l	$00004128
	bne	$0000134E

l00001328:
	movea.l	$00004250,a3
	moveq	#$01,d0
	move.l	d0,$00004128
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
	bsr	fn000012D0
	addq.w	#$04,a7

l0000134E:
	movem.l	(a7)+,a2-a3
	rts

;; fn00001354: 00001354
;;   Called from:
;;     0000120A (in fn00001000)
fn00001354 proc
	movem.l	a2-a3,-(a7)
	lea	$00004238,a3
	move.l	#$00004234,d0
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
	jsr.l	fn00001390
	move.l	d0,-(a7)
	bsr	fn0000131C
	lea	$000C(a7),a7
	movem.l	(a7)+,a2-a3
	rts
0000138E                                           00 00               ..

;; fn00001390: 00001390
;;   Called from:
;;     0000137A (in fn00001354)
fn00001390 proc
	lea	-$0014(a7),a7
	movem.l	d2-d6,-(a7)
	pea	000016B0                                               ; $0318(pc)
	jsr.l	fn00004068
	lea	$0028(a7),a0
	move.l	a0,-(a7)
	pea	000016CC                                               ; $0324(pc)
	jsr.l	fn00002E40
	move.l	$0030(a7),-(a7)
	pea	000016D0                                               ; $031A(pc)
	jsr.l	fn000016FC
	lea	$0028(a7),a0
	move.l	a0,-(a7)
	pea	000016EC                                               ; $0326(pc)
	jsr.l	fn00002E40
	lea	$0034(a7),a0
	move.l	a0,-(a7)
	pea	000016F0                                               ; $031A(pc)
	jsr.l	fn00002E40
	moveq	#$01,d3
	lea	$0024(a7),a7
	bra	$0000168E

l000013EA:
	moveq	#$01,d2
	bra	$00001684

l000013F0:
	moveq	#$03,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	$0014(a7),d1
	move.l	d0,d4
	move.l	d1,d5
	swap.l	d4
	swap.l	d5
	mulu.w	d1,d4
	mulu.w	d0,d5
	mulu.w	d1,d0
	add.w	d5,d4
	swap.l	d4
	clr.w	d4
	add.l	d4,d0
	move.l	d0,d1
	move.l	$0014(a7),d4
	move.l	d1,d5
	move.l	d4,d6
	swap.l	d5
	swap.l	d6
	mulu.w	d4,d5
	mulu.w	d1,d6
	mulu.w	d4,d1
	add.w	d6,d5
	swap.l	d5
	clr.w	d5
	add.l	d5,d1
	move.l	d1,$0014(a7)
	addq.l	#$01,d2

l00001684:
	moveq	#$28,d0
	cmp.l	d2,d0
	bge	$000013F0

l0000168C:
	addq.l	#$01,d3

l0000168E:
	cmp.l	$0024(a7),d3
	ble	$000013EA

l00001696:
	move.l	$0014(a7),-(a7)
	pea	000016F4                                               ; $005A(pc)
	jsr.l	fn000016FC
	addq.w	#$08,a7
	movem.l	(a7)+,d2-d6
	lea	$0014(a7),a7
	rts
000016B0 65 6E 74 65 72 20 6E 75 6D 62 65 72 20 6F 66 20 enter number of 
000016C0 69 74 65 72 61 74 69 6F 6E 73 0A 00 25 6C 64 00 iterations..%ld.
000016D0 65 78 65 63 75 74 69 6E 67 20 25 6C 64 20 69 74 executing %ld it
000016E0 65 72 61 74 69 6F 6E 73 0A 00 00 00 25 64 00 00 erations....%d..
000016F0 25 64 00 00 61 3D 25 64 0A 00 00 00             %d..a=%d....    

;; fn000016FC: 000016FC
;;   Called from:
;;     000013BA (in fn00001390)
;;     0000169E (in fn00001390)
fn000016FC proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00004258,-(a7)
	jsr.l	fn00001718
	lea	$000C(a7),a7
	rts

;; fn00001718: 00001718
;;   Called from:
;;     0000170C (in fn000016FC)
fn00001718 proc
	lea	-$0044(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$007C(a7),d3
	movea.l	$0074(a7),a5
	movea.l	$0078(a7),a4
	moveq	#$00,d6
	tst.b	(a4)
	beq	$0000205E

l00001734:
	cmpi.b	#$25,(a4)
	bne	$00002038

l0000173C:
	clr.l	$0040(a7)
	moveq	#-$01,d5
	clr.l	$0048(a7)
	moveq	#$69,d4
	lea	$004C(a7),a3
	moveq	#$00,d7
	clr.l	$0066(a7)
	lea	$0001(a4),a2
	move.l	$0048(a7),d2

l0000175A:
	moveq	#$00,d1

l0000175C:
	lea	00002070,a0                                            ; $0914(pc)
	move.l	d0,-(a7)
	move.b	(a0,d1),d0
	cmp.b	(a2),d0
	movem.l	(a7)+,d0
	bne	$00001780

l0000176E:
	move.l	d1,d0
	move.l	d1,-(a7)
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	move.l	(a7)+,d1
	or.l	d0,d2
	addq.l	#$01,a2
	bra	$0000178A

l00001780:
	addq.l	#$01,d1
	cmp.l	#$00000005,d1
	bcs	$0000175C

l0000178A:
	cmp.l	#$00000005,d1
	bcs	$0000175A

l00001792:
	move.l	d2,$0048(a7)
	cmpi.b	#$2A,(a2)
	bne	$000017D0

l0000179C:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	bge	$000017C8

l000017B4:
	ori.l	#$00000004,$0048(a7)
	move.l	$002C(a7),d0
	neg.l	d0
	move.l	d0,$0040(a7)
	bra	$0000183C

l000017C8:
	move.l	$002C(a7),$0040(a7)
	bra	$0000183C

l000017D0:
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$0000183C

l000017EC:
	move.l	$0040(a7),d2

l000017F0:
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
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$000017F0

l00001838:
	move.l	d2,$0040(a7)

l0000183C:
	cmpi.b	#$2E,(a2)
	bne	$000018D0

l00001844:
	addq.l	#$01,a2
	cmpi.b	#$2A,(a2)
	bne	$0000186A

l0000184C:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	blt	$000018D0

l00001864:
	move.l	$002C(a7),d5
	bra	$000018D0

l0000186A:
	moveq	#$00,d5
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$000018D0

l00001888:
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
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00001888

l000018D0:
	cmpi.b	#$68,(a2)
	beq	$000018F4

l000018D6:
	cmpi.b	#$6C,(a2)
	beq	$000018F4

l000018DC:
	cmpi.b	#$4C,(a2)
	beq	$000018F4

l000018E2:
	cmpi.b	#$6A,(a2)
	beq	$000018F4

l000018E8:
	cmpi.b	#$7A,(a2)
	beq	$000018F4

l000018EE:
	cmpi.b	#$74,(a2)
	bne	$000018FA

l000018F4:
	move.b	(a2)+,d4
	ext.w	d4
	ext.l	d4

l000018FA:
	cmp.l	#$00000068,d4
	bne	$0000190C

l00001902:
	cmpi.b	#$68,(a2)
	bne	$0000190C

l00001908:
	moveq	#$02,d4
	addq.l	#$01,a2

l0000190C:
	cmp.l	#$0000006C,d4
	bne	$0000191E

l00001914:
	cmpi.b	#$6C,(a2)
	bne	$0000191E

l0000191A:
	moveq	#$01,d4
	addq.l	#$01,a2

l0000191E:
	cmp.l	#$0000006A,d4
	bne	$00001928

l00001926:
	moveq	#$01,d4

l00001928:
	cmp.l	#$0000007A,d4
	bne	$00001932

l00001930:
	moveq	#$6C,d4

l00001932:
	cmp.l	#$00000074,d4
	bne	$0000193C

l0000193A:
	moveq	#$69,d4

l0000193C:
	move.b	(a2)+,d1
	move.b	d1,d0
	cmp.b	#$25,d1
	beq	$00001DC4

l00001948:
	cmp.b	#$58,d0
	beq	$00001996

l0000194E:
	cmp.b	#$63,d0
	beq	$00001D44

l00001956:
	cmp.b	#$64,d0
	beq	$00001996

l0000195C:
	cmp.b	#$69,d0
	beq	$00001996

l00001962:
	move.b	d0,$002C(a7)
	cmp.b	#$6E,d0
	beq	$00001DD6

l0000196E:
	move.b	$002C(a7),d0
	sub.b	#$6F,d0
	cmp.b	#$01,d0
	bls	$00001996

l0000197C:
	move.b	$002C(a7),d0
	cmp.b	#$73,d0
	beq	$00001D80

l00001988:
	cmp.b	#$75,d0
	beq	$00001996

l0000198E:
	cmp.b	#$78,d0
	bne	$00001E6E

l00001996:
	cmp.b	#$70,d1
	bne	$000019A8

l0000199C:
	moveq	#$6C,d4
	moveq	#$78,d1
	ori.l	#$00000001,$0048(a7)

l000019A8:
	cmp.b	#$64,d1
	beq	$000019B6

l000019AE:
	cmp.b	#$69,d1
	bne	$00001B00

l000019B6:
	cmp.l	#$00000001,d4
	bne	$000019DC

l000019BE:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$0034(a7)
	move.l	-$0008(a0),$0030(a7)
	bra	$00001A74

l000019DC:
	cmp.l	#$0000006C,d4
	bne	$00001A08

l000019E4:
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
	bra	$00001A74

l00001A08:
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
	bne	$00001A4E

l00001A32:
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

l00001A4E:
	cmp.l	#$00000002,d4
	bne	$00001A74

l00001A56:
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

l00001A74:
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
	bge	$00001ABE

l00001A9E:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2D,(a0)
	movem.l	$0030(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0038(a7)
	bra	$00001C34

l00001ABE:
	move.b	$002C(a7),d1
	moveq	#$10,d0
	and.l	$0048(a7),d0
	beq	$00001AD8

l00001ACA:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2B,(a0)
	bra	$00001AEC

l00001AD8:
	moveq	#$08,d0
	and.l	$0048(a7),d0
	beq	$00001AEC

l00001AE0:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$20,(a0)

l00001AEC:
	move.l	$0034(a7),$003C(a7)
	move.l	$0030(a7),$0038(a7)
	move.b	d1,$002C(a7)
	bra	$00001C34

l00001B00:
	cmp.l	#$00000001,d4
	bne	$00001B24

l00001B08:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	move.l	-$0008(a0),$0038(a7)
	bra	$00001B5E

l00001B24:
	cmp.l	#$0000006C,d4
	bne	$00001B46

l00001B2C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)
	bra	$00001B5E

l00001B46:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)

l00001B5E:
	cmp.l	#$00000068,d4
	bne	$00001B7A

l00001B66:
	move.w	$003E(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.w	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l00001B7A:
	cmp.l	#$00000002,d4
	bne	$00001B96

l00001B82:
	move.b	$003F(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l00001B96:
	moveq	#$01,d0
	and.l	$0048(a7),d0
	move.b	d1,$002C(a7)
	tst.l	d0
	beq	$00001C34

l00001BA6:
	cmp.b	#$6F,d1
	bne	$00001BE2

l00001BAC:
	tst.l	d5
	bne	$00001BD6

l00001BB0:
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
	beq	$00001BE2

l00001BD6:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$30,(a0)

l00001BE2:
	cmp.b	#$78,d1
	beq	$00001BF2

l00001BE8:
	move.b	d1,$002C(a7)
	cmp.b	#$58,d1
	bne	$00001C34

l00001BF2:
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
	beq	$00001C34

l00001C1C:
	lea	$006A(a7),a0
	lea	(a0,d7),a1
	addq.l	#$01,d7
	move.b	#$30,(a1)
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	d1,(a0)
	move.b	d1,$002C(a7)

l00001C34:
	move.b	$002C(a7),d1
	lea	$0062(a7),a3
	cmp.b	#$78,d1
	beq	$00001C48

l00001C42:
	cmp.b	#$58,d1
	bne	$00001C52

l00001C48:
	move.l	#$00000010,$002C(a7)
	bra	$00001C70

l00001C52:
	cmp.b	#$6F,d1
	bne	$00001C62

l00001C58:
	move.l	#$00000008,$0030(a7)
	bra	$00001C6A

l00001C62:
	move.l	#$0000000A,$0030(a7)

l00001C6A:
	move.l	$0030(a7),$002C(a7)

l00001C70:
	move.l	$002C(a7),$006C(a7)
	cmp.b	#$58,d1
	beq	$00001C82

l00001C7C:
	lea	00002078,a6                                            ; $03FC(pc)
	bra	$00001C86

l00001C82:
	lea	00002088,a6                                            ; $0406(pc)

l00001C86:
	move.l	a6,$002C(a7)
	move.l	d3,$007C(a7)
	move.l	d5,$0044(a7)
	move.l	d6,$0030(a7)
	move.l	d7,$0062(a7)
	movem.l	$0038(a7),d6-d7
	move.l	$0066(a7),d3
	movea.l	$002C(a7),a1

l00001CA8:
	move.l	$006C(a7),d1
	move.l	d1,d0
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	a1,-(a7)
	movem.l	d0-d1,-(a7)
	movem.l	d6-d7,-(a7)
	jsr.l	fn00002A00
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
	jsr.l	fn000026B8
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
	bne	$00001CA8

l00001D16:
	move.l	d3,$0066(a7)
	move.l	$0062(a7),d7
	move.l	$0030(a7),d6
	move.l	$0044(a7),d5
	move.l	$007C(a7),d3
	cmp.l	#$FFFFFFFF,d5
	bne	$00001D38

l00001D32:
	moveq	#$00,d5
	bra	$00001E84

l00001D38:
	andi.l	#$FFFFFFFD,$0048(a7)
	bra	$00001E84

l00001D44:
	cmp.l	#$0000006C,d4
	bne	$00001D60

l00001D4C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)
	bra	$00001D72

l00001D60:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)

l00001D72:
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001E84

l00001D80:
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
	beq	$00001DA2

l00001D9C:
	cmp.l	$0066(a7),d5
	bls	$00001DBE

l00001DA2:
	tst.b	(a1)
	beq	$00001DBE

l00001DA6:
	move.l	$0066(a7),d0

l00001DAA:
	addq.l	#$01,d0
	addq.l	#$01,a1
	tst.l	d5
	bls	$00001DB6

l00001DB2:
	cmp.l	d0,d5
	bls	$00001DBA

l00001DB6:
	tst.b	(a1)
	bne	$00001DAA

l00001DBA:
	move.l	d0,$0066(a7)

l00001DBE:
	moveq	#$00,d5
	bra	$00001E84

l00001DC4:
	lea	0000206C,a3                                            ; $02A8(pc)
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001E84

l00001DD6:
	cmp.l	#$00000001,d4
	bne	$00001DF8

l00001DDE:
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
	bra	$00001E66

l00001DF8:
	cmp.l	#$0000006C,d4
	bne	$00001E16

l00001E00:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)
	bra	$00001E66

l00001E16:
	cmp.l	#$00000068,d4
	bne	$00001E34

l00001E1E:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.w	d6,(a0)
	bra	$00001E66

l00001E34:
	cmp.l	#$00000002,d4
	bne	$00001E52

l00001E3C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.b	d6,(a0)
	bra	$00001E66

l00001E52:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)

l00001E66:
	moveq	#$00,d5
	clr.l	$0040(a7)
	bra	$00001E84

l00001E6E:
	tst.b	d1
	bne	$00001E74

l00001E72:
	subq.l	#$01,a2

l00001E74:
	movea.l	a4,a3
	move.l	a2,d0
	sub.l	a4,d0
	move.l	d0,$0066(a7)
	moveq	#$00,d5
	clr.l	$0040(a7)

l00001E84:
	cmp.l	$0066(a7),d5
	bhi	$00001E92

l00001E8A:
	move.l	$0066(a7),$002C(a7)
	bra	$00001E96

l00001E92:
	move.l	d5,$002C(a7)

l00001E96:
	move.l	d0,-(a7)
	move.l	$0030(a7),d0
	add.l	d7,d0
	move.l	d0,$0034(a7)
	move.l	(a7)+,d0
	move.l	d0,-(a7)
	move.l	$0034(a7),d0
	cmp.l	$0044(a7),d0
	movem.l	(a7)+,d0
	bcs	$00001EBA

l00001EB4:
	clr.l	$002C(a7)
	bra	$00001ECA

l00001EBA:
	move.l	d0,-(a7)
	move.l	$0044(a7),d0
	sub.l	$0034(a7),d0
	move.l	d0,$0030(a7)
	move.l	(a7)+,d0

l00001ECA:
	move.l	$002C(a7),$0030(a7)
	moveq	#$02,d0
	and.l	$0048(a7),d0
	beq	$00001F0C

l00001ED8:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001F0C

l00001EDE:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00002098
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001F04

l00001EFE:
	move.l	d6,d0
	bra	$00002060

l00001F04:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001EDE

l00001F0C:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	bne	$00001F5E

l00001F14:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001F5E

l00001F1C:
	move.l	$0048(a7),d4
	movea.l	$0030(a7),a4

l00001F24:
	move.l	a5,-(a7)
	moveq	#$02,d0
	and.l	d4,d0
	beq	$00001F32

l00001F2C:
	movea.w	#$0030,a0
	bra	$00001F36

l00001F32:
	movea.w	#$0020,a0

l00001F36:
	move.l	a0,-(a7)
	jsr.l	fn00002098
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001F4E

l00001F48:
	move.l	d6,d0
	bra	$00002060

l00001F4E:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001F24

l00001F56:
	move.l	a4,$0030(a7)
	move.l	d4,$0048(a7)

l00001F5E:
	moveq	#$02,d0
	and.l	$0048(a7),d0
	bne	$00001F9A

l00001F66:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001F9A

l00001F6C:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00002098
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001F92

l00001F8C:
	move.l	d6,d0
	bra	$00002060

l00001F92:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001F6C

l00001F9A:
	move.l	$0066(a7),d2
	cmp.l	$0066(a7),d5
	bls	$00001FC8

l00001FA4:
	move.l	a5,-(a7)
	pea	$00000030
	jsr.l	fn00002098
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001FC0

l00001FBA:
	move.l	d6,d0
	bra	$00002060

l00001FC0:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d5
	bhi	$00001FA4

l00001FC8:
	moveq	#$00,d2
	tst.l	$0066(a7)
	beq	$00001FFE

l00001FD0:
	movea.l	$0066(a7),a4

l00001FD4:
	move.l	a5,-(a7)
	lea	(a3,d2),a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00002098
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001FF6

l00001FF2:
	move.l	d6,d0
	bra	$00002060

l00001FF6:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001FD4

l00001FFE:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	beq	$00002034

l00002006:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00002034

l0000200E:
	movea.l	$0030(a7),a3

l00002012:
	move.l	a5,-(a7)
	pea	$00000020
	jsr.l	fn00002098
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$0000202C

l00002028:
	move.l	d6,d0
	bra	$00002060

l0000202C:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a3
	bhi	$00002012

l00002034:
	movea.l	a2,a4
	bra	$00002058

l00002038:
	move.l	a5,-(a7)
	move.b	(a4)+,d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00002098
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00002056

l00002052:
	move.l	d6,d0
	bra	$00002060

l00002056:
	addq.l	#$01,d6

l00002058:
	tst.b	(a4)
	bne	$00001734

l0000205E:
	move.l	d6,d0

l00002060:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$0044(a7),a7
	rts
0000206A                               00 00 25 00 00 00           ..%...
00002070 23 30 2D 20 2B 00 00 00 30 31 32 33 34 35 36 37 #0- +...01234567
00002080 38 39 61 62 63 64 65 66 30 31 32 33 34 35 36 37 89abcdef01234567
00002090 38 39 41 42 43 44 45 46                         89ABCDEF        

;; fn00002098: 00002098
;;   Called from:
;;     00001EEE (in fn00001718)
;;     00001F38 (in fn00001718)
;;     00001F7C (in fn00001718)
;;     00001FAA (in fn00001718)
;;     00001FE2 (in fn00001718)
;;     00002018 (in fn00001718)
;;     00002042 (in fn00001718)
fn00002098 proc
	movem.l	d2/a2-a3,-(a7)
	move.l	$0010(a7),d2
	movea.l	$0014(a7),a2
	lea	$0018(a2),a0
	moveq	#$02,d0
	or.l	d0,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000020DC

l000020B4:
	moveq	#$0A,d0
	cmp.l	d2,d0
	bne	$000020C6

l000020BA:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	bne	$000020DC

l000020C6:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a3
	addq.l	#$01,a3
	move.l	a3,(a1)
	move.b	d2,(a0)
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$000020EA

l000020DC:
	move.l	a2,-(a7)
	move.l	d2,-(a7)
	jsr.l	fn000020F4
	move.l	d0,d1
	addq.w	#$08,a7

l000020EA:
	move.l	d1,d0
	movem.l	(a7)+,d2/a2-a3
	rts
000020F2       00 00                                       ..            

;; fn000020F4: 000020F4
;;   Called from:
;;     000020E0 (in fn00002098)
;;     000040CC (in fn00004068)
fn000020F4 proc
	movem.l	d2-d6/a2-a4/a6,-(a7)
	move.l	$0028(a7),d5
	movea.l	$002C(a7),a2
	jsr.l	fn00002688
	move.l	a2,d0
	bne	$00002110

l0000210A:
	moveq	#-$01,d0
	bra	$00002202

l00002110:
	moveq	#$49,d0
	and.l	$0018(a2),d0
	moveq	#$40,d6
	cmp.l	d0,d6
	beq	$00002122

l0000211C:
	moveq	#-$01,d0
	bra	$00002202

l00002122:
	tst.l	$001C(a2)
	bne	$00002140

l00002128:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00002138

l00002130:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00002140

l00002138:
	move.l	#$00000400,$001C(a2)

l00002140:
	tst.l	$0008(a2)
	bne	$00002180

l00002146:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$0000215A

l00002156:
	moveq	#$02,d4
	bra	$0000215C

l0000215A:
	moveq	#$01,d4

l0000215C:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	fn000022B4
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00002176

l00002170:
	moveq	#-$01,d0
	bra	$00002202

l00002176:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)
	bra	$000021DE

l00002180:
	tst.l	(a2)
	beq	$000021DA

l00002184:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$000021B0

l00002190:
	moveq	#$0A,d0
	cmp.l	d5,d0
	bne	$000021B0

l00002196:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	d5,(a0)
	move.l	a2,-(a7)
	jsr.l	fn00002208
	addq.w	#$04,a7
	bra	$00002202

l000021B0:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00004100,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$000021DE

l000021CE:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00002202

l000021DA:
	moveq	#$00,d0
	bra	$00002202

l000021DE:
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

l00002202:
	movem.l	(a7)+,d2-d6/a2-a4/a6
	rts

;; fn00002208: 00002208
;;   Called from:
;;     000021A6 (in fn000020F4)
;;     00002E0A (in fn00002DFC)
;;     00002E28 (in fn00002DFC)
;;     00004052 (in fn0000402C)
fn00002208 proc
	movem.l	d2-d4/a2/a6,-(a7)
	movea.l	$0018(a7),a2
	jsr.l	fn00002688
	move.l	a2,d0
	bne	$0000221E

l0000221A:
	moveq	#-$01,d0
	bra	$00002296

l0000221E:
	tst.l	$001C(a2)
	bne	$0000223C

l00002224:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00002234

l0000222C:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$0000223C

l00002234:
	move.l	#$00000400,$001C(a2)

l0000223C:
	tst.l	$0008(a2)
	bne	$00002246

l00002242:
	moveq	#$00,d0
	bra	$00002296

l00002246:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$0000228C

l0000224E:
	tst.l	(a2)
	beq	$0000227C

l00002252:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00004100,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00002280

l00002270:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00002296

l0000227C:
	moveq	#$00,d0
	bra	$00002296

l00002280:
	move.l	$0008(a2),$0004(a2)
	move.l	$001C(a2),$0014(a2)

l0000228C:
	lea	$0018(a2),a0
	moveq	#-$04,d0
	and.l	d0,(a0)
	moveq	#$00,d0

l00002296:
	movem.l	(a7)+,d2-d4/a2/a6
	rts
0000229C                                     4A B9 00 00             J...
000022A0 41 30 67 0E 2F 39 00 00 41 30 4E B9 00 00 26 18 A0g./9..A0N...&.
000022B0 58 4F 4E 75                                     XONu            

;; fn000022B4: 000022B4
;;   Called from:
;;     00002162 (in fn000020F4)
;;     00003FB2 (in fn00003F30)
fn000022B4 proc
	movem.l	d2,-(a7)
	move.l	$0008(a7),d2
	bne	$000022C2

l000022BE:
	moveq	#$00,d0
	bra	$00002318

l000022C2:
	tst.l	$00004130
	bne	$000022E6

l000022CA:
	movea.l	$0000412C,a0
	move.l	a0,-(a7)
	move.l	a0,-(a7)
	clr.l	-(a7)
	jsr.l	fn000025A8
	move.l	d0,$00004130
	lea	$000C(a7),a7

l000022E6:
	tst.l	$00004130
	bne	$000022F2

l000022EE:
	moveq	#$00,d0
	bra	$00002318

l000022F2:
	moveq	#$04,d0
	add.l	d2,d0
	move.l	d0,-(a7)
	move.l	$00004130,-(a7)
	jsr.l	fn00002484
	movea.l	d0,a1
	addq.w	#$08,a7
	move.l	a1,d0
	bne	$00002310

l0000230C:
	moveq	#$00,d0
	bra	$00002318

l00002310:
	move.l	d2,(a1)
	lea	$0004(a1),a0
	move.l	a0,d0

l00002318:
	movem.l	(a7)+,d2
	rts
0000231E                                           00 00               ..

;; fn00002320: 00002320
fn00002320 proc
	move.l	$0004(a7),d0
	movea.l	d0,a0
	tst.l	d0
	beq	$0000234A

l0000232A:
	tst.l	$00004130
	beq	$0000234A

l00002332:
	moveq	#$04,d0
	add.l	-(a0),d0
	move.l	d0,-(a7)
	move.l	a0,-(a7)
	move.l	$00004130,-(a7)
	jsr.l	fn000023E8
	lea	$000C(a7),a7

l0000234A:
	rts
0000234C                                     48 E7 30 38             H.08
00002350 28 6F 00 1C 24 6F 00 18 22 0A 66 0A 2F 0C 61 00 (o..$o..".f./.a.
00002360 FF 54 58 4F 60 7A 26 6A FF FC 2F 0C 61 00 FF 46 .TXO`z&j../.a..F
00002370 26 00 58 4F 67 68 B9 CB 64 04 20 0C 60 02 20 0B &.XOgh..d. .`. .
00002380 20 43 22 4A 24 00 B4 BC 00 00 00 10 65 3C 20 08  C"J$.......e< .
00002390 22 09 C0 3C 00 01 C2 3C 00 01 B2 00 66 1A 20 08 "..<...<....f. .
000023A0 4A 01 67 04 10 D9 53 82 72 03 C2 82 94 81 20 D9 J.g...S.r..... .
000023B0 59 82 66 FA 34 01 60 14 B4 BC 00 01 00 00 65 0A Y.f.4.`.......e.
000023C0 20 08 10 D9 53 82 66 FA 60 0C 20 08 53 42 65 06  ...S.f.`. .SBe.
000023D0 10 D9 51 CA FF FC 2F 0A 61 00 FF 46 58 4F 20 03 ..Q.../.a..FXO .
000023E0 4C DF 1C 0C 4E 75 00 00                         L...Nu..        

;; fn000023E8: 000023E8
;;   Called from:
;;     00002340 (in fn00002320)
fn000023E8 proc
	movem.l	d2/a2-a6,-(a7)
	move.l	$0020(a7),d1
	movea.l	$0024(a7),a5
	movea.l	$001C(a7),a4
	movea.l	$000040FC,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002418

l00002406:
	movea.l	$000040FC,a6
	movea.l	a4,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$02CA(a6)
	bra	$0000247C

l00002418:
	move.l	a4,d2
	beq	$0000247C

l0000241C:
	tst.l	d1
	beq	$0000247C

l00002420:
	movea.l	d1,a3
	lea	-$000C(a3),a3
	cmpa.l	$0014(a4),a5
	bcc	$00002462

l0000242C:
	movea.l	a4,a2

l0000242E:
	movea.l	(a2),a2
	tst.l	(a2)
	beq	$0000247C

l00002434:
	tst.b	$0008(a2)
	beq	$0000242E

l0000243A:
	cmp.l	$0014(a2),d1
	bcs	$0000242E

l00002440:
	cmp.l	$0018(a2),d1
	bcc	$0000242E

l00002446:
	movea.l	$000040FC,a6
	movea.l	a2,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$00C0(a6)
	move.l	$001C(a2),d0
	cmp.l	$0010(a4),d0
	bne	$0000247C

l00002460:
	movea.l	a2,a3

l00002462:
	movea.l	$000040FC,a6
	movea.l	a3,a1
	jsr.l	-$00FC(a6)
	move.l	-(a3),d0
	movea.l	$000040FC,a6
	movea.l	a3,a1
	jsr.l	-$00D2(a6)

l0000247C:
	movem.l	(a7)+,d2/a2-a6
	rts
00002482       00 00                                       ..            

;; fn00002484: 00002484
;;   Called from:
;;     000022FE (in fn000022B4)
fn00002484 proc
	movem.l	d2-d4/a2-a6,-(a7)
	move.l	$0028(a7),d2
	movea.l	$0024(a7),a4
	movea.l	$000040FC,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$000024B0

l0000249E:
	movea.l	$000040FC,a6
	movea.l	a4,a0
	move.l	d2,d0
	jsr.l	-$02C4(a6)
	bra	$000025A2

l000024B0:
	suba.l	a3,a3
	move.l	a4,d4
	beq	$000025A0

l000024B8:
	tst.l	d2
	beq	$000025A0

l000024BE:
	cmp.l	$0014(a4),d2
	bcc	$00002572

l000024C6:
	movea.l	(a4),a5

l000024C8:
	tst.l	(a5)
	beq	$000024EA

l000024CC:
	tst.b	$0008(a5)
	beq	$000024E6

l000024D2:
	movea.l	$000040FC,a6
	movea.l	a5,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3
	move.l	a3,d0
	bne	$00002556

l000024E6:
	movea.l	(a5),a5
	bra	$000024C8

l000024EA:
	moveq	#$28,d3
	add.l	$0010(a4),d3
	move.l	$000C(a4),d1
	movea.l	$000040FC,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$000025A0

l00002508:
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
	movea.l	$000040FC,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F0(a6)
	movea.l	$000040FC,a6
	movea.l	a3,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3

l00002556:
	move.l	#$00010000,d0
	and.l	$000C(a4),d0
	beq	$000025A0

l00002562:
	movea.l	a3,a2
	addq.l	#$07,d2
	lsr.l	#$03,d2

l00002568:
	clr.l	(a2)+
	clr.l	(a2)+
	subq.l	#$01,d2
	bne	$00002568

l00002570:
	bra	$000025A0

l00002572:
	moveq	#$10,d3
	add.l	d2,d3
	move.l	$000C(a4),d1
	movea.l	$000040FC,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$000025A0

l0000258C:
	move.l	d3,(a3)+
	movea.l	$000040FC,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F6(a6)
	addq.l	#$08,a3
	clr.l	(a3)+

l000025A0:
	move.l	a3,d0

l000025A2:
	movem.l	(a7)+,d2-d4/a2-a6
	rts

;; fn000025A8: 000025A8
;;   Called from:
;;     000022D6 (in fn000022B4)
fn000025A8 proc
	movem.l	d2-d3/a2/a6,-(a7)
	move.l	$0018(a7),d3
	movea.l	$001C(a7),a2
	movea.l	$000040FC,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$000025D6

l000025C2:
	movea.l	$000040FC,a6
	move.l	$0014(a7),d0
	move.l	d3,d1
	move.l	a2,d2
	jsr.l	-$02B8(a6)
	bra	$00002612

l000025D6:
	suba.l	a1,a1
	cmp.l	a2,d3
	bcs	$00002610

l000025DC:
	addq.l	#$07,d3
	movea.l	$000040FC,a6
	moveq	#$18,d0
	moveq	#$00,d1
	jsr.l	-$00C6(a6)
	movea.l	d0,a1
	move.l	a1,d0
	beq	$00002610

l000025F2:
	lea	$0004(a1),a0
	move.l	a0,(a1)
	clr.l	(a0)
	move.l	a1,$0008(a1)
	move.l	$0014(a7),$000C(a1)
	moveq	#-$08,d0
	and.l	d3,d0
	move.l	d0,$0010(a1)
	move.l	a2,$0014(a1)

l00002610:
	move.l	a1,d0

l00002612:
	movem.l	(a7)+,d2-d3/a2/a6
	rts

;; fn00002618: 00002618
fn00002618 proc
	movem.l	d2/a2/a6,-(a7)
	move.l	$0010(a7),d2
	movea.l	$000040FC,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$0000263C

l0000262E:
	movea.l	$000040FC,a6
	movea.l	d2,a0
	jsr.l	-$02BE(a6)
	bra	$00002680

l0000263C:
	tst.l	d2
	beq	$00002680

l00002640:
	movea.l	$000040FC,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d1
	beq	$00002672

l00002652:
	move.l	-(a2),d0
	movea.l	$000040FC,a6
	movea.l	a2,a1
	jsr.l	-$00D2(a6)
	movea.l	$000040FC,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d0
	bne	$00002652

l00002672:
	movea.l	$000040FC,a6
	movea.l	d2,a1
	moveq	#$18,d0
	jsr.l	-$00D2(a6)

l00002680:
	movem.l	(a7)+,d2/a2/a6
	rts
00002686                   00 00                               ..        

;; fn00002688: 00002688
;;   Called from:
;;     00002100 (in fn000020F4)
;;     00002210 (in fn00002208)
;;     00003F38 (in fn00003F30)
fn00002688 proc
	movem.l	a6,-(a7)
	movea.l	$000040FC,a6
	moveq	#$00,d0
	move.l	#$00001000,d1
	jsr.l	-$0132(a6)
	and.l	#$00001000,d0
	beq	$000026B2

l000026A6:
	pea	$00000014
	jsr.l	fn0000131C
	addq.w	#$04,a7

l000026B2:
	movea.l	(a7)+,a6
	rts
000026B6                   00 00                               ..        

;; fn000026B8: 000026B8
;;   Called from:
;;     00001CEC (in fn00001718)
fn000026B8 proc
	movem.l	d2-d6,-(a7)
	move.l	$001C(a7),d1
	move.l	$0018(a7),d0
	movea.l	d1,a0
	move.l	$0024(a7),d3
	move.l	$0020(a7),d2
	bne	$0000270E

l000026D0:
	cmp.l	d3,d0
	bcc	$000026E2

l000026D4:
	move.l	d3,d2
	jsr.l	fn000027BC
	move.l	d0,d1
	bra	$000027B4

l000026E2:
	tst.l	d3
	bne	$000026EE

l000026E6:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l000026EE:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	fn000027BC
	movea.l	d0,a1
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	fn000027BC
	move.l	d0,d1
	move.l	a1,d0
	bra	$000027B6

l0000270E:
	cmp.l	d2,d0
	bcc	$00002718

l00002712:
	moveq	#$00,d0
	bra	$000027B4

l00002718:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002736

l00002722:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002736

l0000272A:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002736

l00002732:
	moveq	#$00,d4
	move.b	d2,d6

l00002736:
	lea	$00004134,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$00002756

l0000274A:
	cmp.l	d0,d2
	bcs	$00002752

l0000274E:
	cmp.l	a0,d3
	bhi	$00002712

l00002752:
	moveq	#$01,d0
	bra	$000027B4

l00002756:
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
	jsr.l	fn000027BC
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
	bcs	$000027B2

l000027AC:
	bne	$000027B4

l000027AE:
	cmpa.l	d2,a0
	bcc	$000027B4

l000027B2:
	subq.l	#$01,d1

l000027B4:
	moveq	#$00,d0

l000027B6:
	movem.l	(a7)+,d2-d6
	rts

;; fn000027BC: 000027BC
;;   Called from:
;;     000026D6 (in fn000026B8)
;;     000026F4 (in fn000026B8)
;;     00002700 (in fn000026B8)
;;     00002772 (in fn000026B8)
;;     00002A1E (in fn00002A00)
;;     00002A3C (in fn00002A00)
;;     00002A46 (in fn00002A00)
;;     00002AB4 (in fn00002A00)
fn000027BC proc
	movem.l	d5-d7,-(a7)
	move.l	d2,d7
	beq	$000027D6

l000027C4:
	move.l	d1,d6
	move.l	d0,d5
	bne	$000027E4

l000027CA:
	tst.l	d1
	beq	$00002902

l000027D0:
	cmp.l	d1,d2
	bhi	$00002902

l000027D6:
	move.l	d1,d0
	move.l	d2,d1
	jsr.l	fn0000297A
	bra	$00002902

l000027E4:
	swap.l	d2
	tst.w	d2
	bne	$0000280C

l000027EA:
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
	bra	$00002902

l0000280C:
	movem.l	d2-d4/a0-a1,-(a7)
	subq.l	#$08,a7
	clr.b	$0002(a7)
	moveq	#$00,d1
	moveq	#$00,d0
	tst.l	d7
	bmi	$00002828

l0000281E:
	addq.w	#$01,d0
	add.l	d6,d6
	addx.l	d5,d5
	add.l	d7,d7
	bpl	$0000281E

l00002828:
	move.w	d0,(a7)

l0000282A:
	move.l	d7,d3
	move.l	d5,d2
	swap.l	d2
	swap.l	d3
	cmp.w	d3,d2
	bne	$0000283C

l00002836:
	move.w	#$FFFF,d1
	bra	$00002846

l0000283C:
	move.l	d5,d1
	divu.w	d3,d1
	swap.l	d1
	clr.w	d1
	swap.l	d1

l00002846:
	movea.l	d6,a1
	clr.w	d6
	swap.l	d6

l0000284C:
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
	bne	$0000286C

l00002864:
	cmp.l	d4,d2
	bls	$0000286C

l00002868:
	subq.l	#$01,d1
	bra	$0000284C

l0000286C:
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
	bcc	$000028C4

l000028AE:
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

l000028C4:
	tst.b	$0002(a7)
	bne	$000028E0

l000028CA:
	move.w	d1,$0004(a7)
	moveq	#$00,d1
	swap.l	d5
	swap.l	d6
	move.w	d6,d5
	clr.w	d6
	st	$0002(a7)
	bra	$0000282A

l000028E0:
	move.l	$0004(a7),d0
	move.w	d1,d0
	move.w	d5,d6
	swap.l	d6
	swap.l	d5
	move.w	(a7),d7
	beq	$000028FA

l000028F0:
	subq.w	#$01,d7

l000028F2:
	lsr.l	#$01,d5
	roxr.l	#$01,d6
	dbra	d7,$000028F2

l000028FA:
	move.l	d6,d1
	addq.l	#$08,a7
	movem.l	(a7)+,d2-d4/a0-a1

l00002902:
	movem.l	(a7)+,d5-d7
	rts
00002908                         4C EF 00 03 00 04 4A 81         L.....J.
00002910 6B 0A 4A 80 6B 12 61 62 20 01 4E 75 44 81 4A 80 k.J.k.ab .NuD.J.
00002920 6B 10 61 56 20 01 4E 75 44 80 61 4E 44 81 20 01 k.aV .NuD.aND. .
00002930 4E 75 44 80 61 44 44 81 20 01 4E 75 4C EF 00 03 NuD.aDD. .NuL...
00002940 00 04 61 36 20 01 4E 75                         ..a6 .Nu        

;; fn00002948: 00002948
;;   Called from:
;;     000032FA (in fn00002E8C)
;;     0000337E (in fn00002E8C)
fn00002948 proc
	movem.l	$0004(a7),d0-d1
	tst.l	d0
	bpl	$00002968

l00002952:
	neg.l	d0
	tst.l	d1
	bpl	$00002960

l00002958:
	neg.l	d1
	bsr	fn0000297A
	neg.l	d1
	rts

l00002960:
	bsr	fn0000297A
	neg.l	d0
	neg.l	d1
	rts

l00002968:
	tst.l	d1
	bpl	fn0000297A

l0000296C:
	neg.l	d1
	bsr	fn0000297A
	neg.l	d0
	rts
00002974             4C EF 00 03 00 04                       L.....      

;; fn0000297A: 0000297A
;;   Called from:
;;     000027DA (in fn000027BC)
;;     0000295A (in fn00002948)
;;     00002960 (in fn00002948)
;;     0000296A (in fn00002948)
;;     0000296E (in fn00002948)
fn0000297A proc
	move.l	d2,-(a7)
	swap.l	d1
	move.w	d1,d2
	bne	$000029A0

l00002982:
	swap.l	d0
	swap.l	d1
	swap.l	d2
	move.w	d0,d2
	beq	$00002990

l0000298C:
	divu.w	d1,d2
	move.w	d2,d0

l00002990:
	swap.l	d0
	move.w	d0,d2
	divu.w	d1,d2
	move.w	d2,d0
	swap.l	d2
	move.w	d2,d1
	move.l	(a7)+,d2
	rts

l000029A0:
	move.l	d3,-(a7)
	moveq	#$10,d3
	cmp.w	#$0080,d1
	bcc	$000029AE

l000029AA:
	rol.l	#$08,d1
	subq.w	#$08,d3

l000029AE:
	cmp.w	#$0800,d1
	bcc	$000029B8

l000029B4:
	rol.l	#$04,d1
	subq.w	#$04,d3

l000029B8:
	cmp.w	#$2000,d1
	bcc	$000029C2

l000029BE:
	rol.l	#$02,d1
	subq.w	#$02,d3

l000029C2:
	tst.w	d1
	bmi	$000029CA

l000029C6:
	rol.l	#$01,d1
	subq.w	#$01,d3

l000029CA:
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
	bcc	$000029EC

l000029E6:
	subq.w	#$01,d3
	add.l	d1,d0

l000029EA:
	bcc	$000029EA

l000029EC:
	moveq	#$00,d1
	move.w	d3,d1
	swap.l	d3
	rol.l	d3,d0
	swap.l	d0
	exg	d0,d1
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	rts
000029FE                                           00 00               ..

;; fn00002A00: 00002A00
;;   Called from:
;;     00001CC0 (in fn00001718)
fn00002A00 proc
	movem.l	d2-d7,-(a7)
	move.l	$0020(a7),d1
	move.l	$001C(a7),d0
	movea.l	d1,a0
	move.l	$0028(a7),d3
	move.l	$0024(a7),d2
	bne	$00002A52

l00002A18:
	cmp.l	d3,d0
	bcc	$00002A2A

l00002A1C:
	move.l	d3,d2
	jsr.l	fn000027BC
	moveq	#$00,d0
	bra	$00002B0C

l00002A2A:
	tst.l	d3
	bne	$00002A36

l00002A2E:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l00002A36:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	fn000027BC
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	fn000027BC
	moveq	#$00,d0
	bra	$00002B0C

l00002A52:
	cmp.l	d2,d0
	bcs	$00002B0C

l00002A58:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002A76

l00002A62:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002A76

l00002A6A:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002A76

l00002A72:
	moveq	#$00,d4
	move.b	d2,d6

l00002A76:
	lea	$00004134,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$00002A98

l00002A8A:
	cmp.l	d0,d2
	bcs	$00002A92

l00002A8E:
	cmp.l	d1,d3
	bhi	$00002B0C

l00002A92:
	sub.l	d3,d1
	subx.l	d2,d0
	bra	$00002B0C

l00002A98:
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
	jsr.l	fn000027BC
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
	bcs	$00002AF4

l00002AEE:
	bne	$00002AF8

l00002AF0:
	cmpa.l	d3,a0
	bcc	$00002AF8

l00002AF4:
	sub.l	a1,d3
	subx.l	d0,d2

l00002AF8:
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

l00002B0C:
	movem.l	(a7)+,d2-d7
	rts
00002B12       00 00 00 20 20 20 20 20 20 20 20 20 28 28   ...         ((
00002B20 28 28 28 20 20 20 20 20 20 20 20 20 20 20 20 20 (((             
00002B30 20 20 20 20 20 88 10 10 10 10 10 10 10 10 10 10      ...........
00002B40 10 10 10 10 10 04 04 04 04 04 04 04 04 04 04 10 ................
00002B50 10 10 10 10 10 10 41 41 41 41 41 41 01 01 01 01 ......AAAAAA....
00002B60 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 ................
00002B70 10 10 10 10 10 10 42 42 42 42 42 42 02 02 02 02 ......BBBBBB....
00002B80 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 ................
00002B90 10 10 10 10 20 00 00 00 00 00 00 00 00 00 00 00 .... ...........
00002BA0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00002C10 00 00 00 00 00 00 00 00 48 E7 00 32 48 78 00 20 ........H..2Hx. 
00002C20 4E B9 00 00 22 B4 23 C0 00 00 42 54 48 78 00 20 N...".#...BTHx. 
00002C30 4E B9 00 00 22 B4 23 C0 00 00 42 58 48 78 00 20 N...".#...BXHx. 
00002C40 4E B9 00 00 22 B4 23 C0 00 00 42 5C 4F EF 00 0C N...".#...B\O...
00002C50 4A B9 00 00 42 54 67 10 4A B9 00 00 42 58 67 08 J...BTg.J...BXg.
00002C60 4A B9 00 00 42 5C 66 0C 48 78 00 14 4E B9 00 00 J...B\f.Hx..N...
00002C70 13 1C 58 4F 20 79 00 00 42 54 20 B9 00 00 41 0C ..XO y..BT ...A.
00002C80 20 79 00 00 42 54 70 20 21 40 00 18 22 39 00 00  y..BTp !@.."9..
00002C90 41 0C 2C 79 00 00 41 00 4E AE FF 28 4A 80 67 10 A.,y..A.N..(J.g.
00002CA0 20 79 00 00 42 54 41 E8 00 18 00 90 00 00 02 04  y..BTA.........
00002CB0 20 79 00 00 42 58 20 B9 00 00 41 10 20 79 00 00  y..BX ...A. y..
00002CC0 42 58 70 40 21 40 00 18 22 39 00 00 41 10 2C 79 BXp@!@.."9..A.,y
00002CD0 00 00 41 00 4E AE FF 28 4A 80 67 10 20 79 00 00 ..A.N..(J.g. y..
00002CE0 42 58 41 E8 00 18 00 90 00 00 02 80 20 79 00 00 BXA......... y..
00002CF0 42 5C 20 B9 00 00 41 14 20 79 00 00 42 5C 70 40 B\ ...A. y..B\p@
00002D00 21 40 00 18 22 39 00 00 41 14 2C 79 00 00 41 00 !@.."9..A.,y..A.
00002D10 4E AE FF 28 4A 80 67 10 20 79 00 00 42 5C 41 E8 N..(J.g. y..B\A.
00002D20 00 18 00 90 00 00 02 80 20 79 00 00 42 5C 42 A8 ........ y..B\B.
00002D30 00 04 20 79 00 00 42 58 42 A8 00 04 20 79 00 00 .. y..BXB... y..
00002D40 42 54 42 A8 00 04 20 79 00 00 42 5C 42 A8 00 08 BTB... y..B\B...
00002D50 20 79 00 00 42 58 42 A8 00 08 20 79 00 00 42 54  y..BXB... y..BT
00002D60 42 A8 00 08 24 79 00 00 42 5C 42 AA 00 14 22 79 B...$y..B\B..."y
00002D70 00 00 42 58 42 A9 00 14 20 79 00 00 42 54 42 A8 ..BXB... y..BTB.
00002D80 00 14 42 AA 00 1C 42 A9 00 1C 42 A8 00 1C 42 A8 ..B...B...B...B.
00002D90 00 10 20 79 00 00 42 54 21 79 00 00 42 58 00 0C .. y..BT!y..BX..
00002DA0 20 79 00 00 42 58 21 79 00 00 42 54 00 10 20 79  y..BX!y..BT.. y
00002DB0 00 00 42 58 21 79 00 00 42 5C 00 0C 20 79 00 00 ..BX!y..B\.. y..
00002DC0 42 5C 21 79 00 00 42 58 00 10 20 79 00 00 42 5C B\!y..BX.. y..B\
00002DD0 42 A8 00 0C 23 F9 00 00 42 54 00 00 42 60 23 F9 B...#...BT..B`#.
00002DE0 00 00 42 5C 00 00 42 64 4C DF 4C 00 4E 75 00 00 ..B\..BdL.L.Nu..
00002DF0 42 A7 4E B9 00 00 2D FC 58 4F 4E 75             B.N...-.XONu    

;; fn00002DFC: 00002DFC
fn00002DFC proc
	movem.l	a2,-(a7)
	movea.l	$0008(a7),a2
	move.l	a2,d0
	beq	$00002E14

l00002E08:
	move.l	a2,-(a7)
	jsr.l	fn00002208
	addq.w	#$04,a7
	bra	$00002E3A

l00002E14:
	movea.l	$00004260,a2
	move.l	a2,d0
	beq	$00002E3A

l00002E1E:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002E30

l00002E26:
	move.l	a2,-(a7)
	jsr.l	fn00002208
	addq.w	#$04,a7

l00002E30:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00002E1E

l00002E3A:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	rts

;; fn00002E40: 00002E40
;;   Called from:
;;     000013AC (in fn00001390)
;;     000013CA (in fn00001390)
;;     000013DA (in fn00001390)
fn00002E40 proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00004254,-(a7)
	jsr.l	fn00002E8C
	lea	$000C(a7),a7
	rts

;; fn00002E5C: 00002E5C
;;   Called from:
;;     00003196 (in fn00002E8C)
;;     000033B2 (in fn00002E8C)
;;     00003498 (in fn00002E8C)
;;     0000351A (in fn00002E8C)
;;     0000375A (in fn00002E8C)
;;     00003778 (in fn00002E8C)
;;     000038D2 (in fn00002E8C)
;;     000038EC (in fn00002E8C)
;;     00003BB8 (in fn00002E8C)
;;     00003BD2 (in fn00002E8C)
;;     00003E0C (in fn00002E8C)
;;     00003E78 (in fn00002E8C)
fn00002E5C proc
	movem.l	a2,-(a7)
	movea.l	$000C(a7),a2
	move.l	a2,d0
	beq	$00002E86

l00002E68:
	move.l	$0004(a2),d0
	cmp.l	$0008(a2),d0
	bcc	$00002E7A

l00002E72:
	movea.l	$0004(a2),a0
	move.b	$000B(a7),(a0)

l00002E7A:
	lea	$0014(a2),a0
	addq.l	#$01,(a0)
	lea	$0004(a2),a0
	subq.l	#$01,(a0)

l00002E86:
	movea.l	(a7)+,a2
	rts
00002E8A                               00 00                       ..    

;; fn00002E8C: 00002E8C
;;   Called from:
;;     00002E50 (in fn00002E40)
fn00002E8C proc
	lea	-$004C(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$0084(a7),d2
	movea.l	$0080(a7),a4
	movea.l	$007C(a7),a2
	clr.l	$003C(a7)
	moveq	#$00,d4
	moveq	#$00,d5
	tst.b	(a4)
	beq	$00003E8E

l00002EAE:
	moveq	#$00,d3
	cmpi.b	#$25,(a4)
	bne	$00003D8C

l00002EB8:
	moveq	#-$01,d6
	move.b	#$69,$0048(a7)
	clr.b	$0049(a7)
	lea	$0001(a4),a3
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00002F3C

l00002EE0:
	moveq	#$00,d6
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00002F3C

l00002EFA:
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
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00002EFA

l00002F3C:
	cmpi.b	#$68,(a3)
	beq	$00002F66

l00002F42:
	cmpi.b	#$6C,(a3)
	beq	$00002F66

l00002F48:
	cmpi.b	#$4C,(a3)
	beq	$00002F66

l00002F4E:
	cmpi.b	#$7A,(a3)
	beq	$00002F66

l00002F54:
	cmpi.b	#$6A,(a3)
	beq	$00002F66

l00002F5A:
	cmpi.b	#$74,(a3)
	beq	$00002F66

l00002F60:
	cmpi.b	#$2A,(a3)
	bne	$00002FCE

l00002F66:
	move.b	$0049(a7),d7
	move.b	$0048(a7),d1

l00002F6E:
	cmpi.b	#$2A,(a3)
	bne	$00002F78

l00002F74:
	moveq	#$01,d7
	bra	$00002F9A

l00002F78:
	cmp.b	#$68,d1
	bne	$00002F88

l00002F7E:
	cmpi.b	#$68,(a3)
	bne	$00002F88

l00002F84:
	moveq	#$02,d1
	bra	$00002F9A

l00002F88:
	cmp.b	#$6C,d1
	bne	$00002F98

l00002F8E:
	cmpi.b	#$6C,(a3)
	bne	$00002F98

l00002F94:
	moveq	#$01,d1
	bra	$00002F9A

l00002F98:
	move.b	(a3),d1

l00002F9A:
	addq.l	#$01,a3
	cmpi.b	#$68,(a3)
	beq	$00002F6E

l00002FA2:
	cmpi.b	#$6C,(a3)
	beq	$00002F6E

l00002FA8:
	cmpi.b	#$4C,(a3)
	beq	$00002F6E

l00002FAE:
	cmpi.b	#$7A,(a3)
	beq	$00002F6E

l00002FB4:
	cmpi.b	#$6A,(a3)
	beq	$00002F6E

l00002FBA:
	cmpi.b	#$74,(a3)
	beq	$00002F6E

l00002FC0:
	cmpi.b	#$2A,(a3)
	beq	$00002F6E

l00002FC6:
	move.b	d1,$0048(a7)
	move.b	d7,$0049(a7)

l00002FCE:
	cmpi.b	#$6A,$0048(a7)
	bne	$00002FDC

l00002FD6:
	move.b	#$01,$0048(a7)

l00002FDC:
	cmpi.b	#$74,$0048(a7)
	bne	$00002FEA

l00002FE4:
	move.b	#$69,$0048(a7)

l00002FEA:
	cmpi.b	#$7A,$0048(a7)
	bne	$00002FF8

l00002FF2:
	move.b	#$6C,$0048(a7)

l00002FF8:
	move.b	(a3)+,d7
	beq	$0000306E

l00002FFC:
	cmp.b	#$25,d7
	beq	$0000306E

l00003002:
	cmp.b	#$63,d7
	beq	$0000306E

l00003008:
	cmp.b	#$6E,d7
	beq	$0000306E

l0000300E:
	cmp.b	#$5B,d7
	beq	$0000306E

l00003014:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000303E

l00003026:
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
	bra	$0000304A

l0000303E:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,d1
	addq.w	#$04,a7

l0000304A:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003014

l0000306C:
	moveq	#$01,d3

l0000306E:
	move.b	d7,d1
	sub.b	#$25,d1
	beq	$000034BA

l00003078:
	sub.b	#$36,d1
	beq	$000031B6

l00003080:
	subq.b	#$08,d1
	beq	$00003096

l00003084:
	sub.b	#$0B,d1
	beq	$00003528

l0000308C:
	subq.b	#$05,d1
	beq	$000033D4

l00003092:
	bra	$000035CA

l00003096:
	cmp.l	#$FFFFFFFF,d6
	bne	$000030A0

l0000309E:
	moveq	#$01,d6

l000030A0:
	tst.b	$0049(a7)
	bne	$000030BA

l000030A6:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a1
	bra	$000030BC

l000030BA:
	suba.l	a1,a1

l000030BC:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	move.l	a1,$002C(a7)
	tst.l	(a0)
	blt	$000030F4

l000030D4:
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
	bra	$00003112

l000030F4:
	movea.l	$002C(a7),a1
	move.l	a2,-(a7)
	move.l	a1,$0030(a7)
	jsr.l	fn00003F30
	move.l	d0,$0038(a7)
	movea.l	$0030(a7),a1
	move.l	a1,$0030(a7)
	addq.w	#$04,a7

l00003112:
	movea.l	$002C(a7),a1
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$0034(a7)
	beq	$0000318A

l00003128:
	move.l	a1,$002C(a7)
	cmp.l	d3,d6
	bcs	$0000318A

l00003130:
	move.b	$0049(a7),d7
	movea.l	$002C(a7),a4

l00003138:
	tst.b	d7
	bne	$0000313E

l0000313C:
	move.b	d5,(a4)+

l0000313E:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003168

l00003150:
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
	bra	$00003174

l00003168:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,d1
	addq.w	#$04,a7

l00003174:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00003186

l00003182:
	cmp.l	d3,d6
	bcc	$00003138

l00003186:
	move.b	d7,$0049(a7)

l0000318A:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000319C

l00003192:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l0000319C:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003D86

l000031A8:
	tst.l	d3
	beq	$00003D86

l000031AE:
	addq.l	#$01,$003C(a7)
	bra	$00003D86

l000031B6:
	clr.b	$002C(a7)
	cmpi.b	#$5E,(a3)
	bne	$000031C8

l000031C0:
	move.b	#$01,$002C(a7)
	addq.l	#$01,a3

l000031C8:
	clr.l	$0034(a7)
	move.b	$002C(a7),d7
	move.l	$0034(a7),d1

l000031D4:
	tst.b	d7
	beq	$000031E0

l000031D8:
	move.l	#$000000FF,d5
	bra	$000031E2

l000031E0:
	moveq	#$00,d5

l000031E2:
	lea	$004E(a7),a0
	move.b	d5,(a0,d1)
	addq.l	#$01,d1
	cmp.l	#$00000020,d1
	bcs	$000031D4

l000031F4:
	move.l	d2,$0084(a7)
	move.b	d7,$002C(a7)
	move.b	$002C(a7),d2

l00003200:
	tst.b	(a3)
	beq	$00003276

l00003204:
	move.b	(a3)+,d1
	cmpi.b	#$2D,(a3)
	bne	$00003218

l0000320C:
	cmp.b	$0001(a3),d1
	bcc	$00003218

l00003212:
	addq.l	#$01,a3
	move.b	(a3)+,d7
	bra	$0000321A

l00003218:
	move.b	d1,d7

l0000321A:
	moveq	#$00,d5
	move.b	d1,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcs	$00003270

l00003226:
	tst.b	d2
	beq	$0000324A

l0000322A:
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
	bra	$00003266

l0000324A:
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

l00003266:
	addq.l	#$01,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcc	$00003226

l00003270:
	cmpi.b	#$5D,(a3)
	bne	$00003200

l00003276:
	move.l	$0084(a7),d2
	addq.l	#$01,a3
	tst.b	$0049(a7)
	bne	$00003296

l00003282:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a6
	bra	$00003298

l00003296:
	suba.l	a6,a6

l00003298:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000032CA

l000032AA:
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
	bra	$000032D8

l000032CA:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l000032D8:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$002C(a7)
	beq	$000033A6

l000032EC:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	fn00002948
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
	beq	$000033A6

l0000331C:
	cmp.l	d3,d6
	bcs	$000033A6

l00003322:
	move.b	$0049(a7),d7

l00003326:
	tst.b	d7
	bne	$0000332C

l0000332A:
	move.b	d5,(a6)+

l0000332C:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003356

l0000333E:
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
	bra	$00003362

l00003356:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,d1
	addq.w	#$04,a7

l00003362:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$000033A2

l00003370:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	fn00002948
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
	beq	$000033A2

l0000339E:
	cmp.l	d3,d6
	bcc	$00003326

l000033A2:
	move.b	d7,$0049(a7)

l000033A6:
	cmp.l	#$FFFFFFFF,d5
	beq	$000033B8

l000033AE:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l000033B8:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003D86

l000033C4:
	tst.l	d3
	beq	$00003D86

l000033CA:
	clr.b	(a6)+
	addq.l	#$01,$003C(a7)
	bra	$00003D86

l000033D4:
	tst.b	$0049(a7)
	bne	$000033EE

l000033DA:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a5
	bra	$000033F0

l000033EE:
	suba.l	a5,a5

l000033F0:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000348C

l000033FA:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$0000348C

l00003416:
	cmp.l	d3,d6
	bcs	$0000348C

l0000341A:
	move.b	$0049(a7),d7

l0000341E:
	tst.b	d7
	bne	$00003424

l00003422:
	move.b	d5,(a5)+

l00003424:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000344E

l00003436:
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
	bra	$0000345A

l0000344E:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,d1
	addq.w	#$04,a7

l0000345A:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00003488

l00003468:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003488

l00003484:
	cmp.l	d3,d6
	bcc	$0000341E

l00003488:
	move.b	d7,$0049(a7)

l0000348C:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000349E

l00003494:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l0000349E:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003D86

l000034AA:
	tst.l	d3
	beq	$00003D86

l000034B0:
	clr.b	(a5)+
	addq.l	#$01,$003C(a7)
	bra	$00003D86

l000034BA:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000034EC

l000034CC:
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
	bra	$000034FA

l000034EC:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l000034FA:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$00000025,$002C(a7)
	beq	$00003D86

l0000350E:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003520

l00003516:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l00003520:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00003D86

l00003528:
	tst.b	$0049(a7)
	bne	$000035C0

l00003530:
	cmpi.b	#$01,$0048(a7)
	bne	$00003552

l00003538:
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
	bra	$000035C0

l00003552:
	cmpi.b	#$6C,$0048(a7)
	bne	$00003570

l0000355A:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)
	bra	$000035C0

l00003570:
	cmpi.b	#$68,$0048(a7)
	bne	$0000358E

l00003578:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.w	d4,(a0)
	bra	$000035C0

l0000358E:
	cmpi.b	#$02,$0048(a7)
	bne	$000035AC

l00003596:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.b	d4,(a0)
	bra	$000035C0

l000035AC:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)

l000035C0:
	moveq	#$01,d3
	addq.l	#$01,$003C(a7)
	bra	$00003D86

l000035CA:
	clr.l	$0030(a7)
	clr.l	$002C(a7)
	clr.l	$006E(a7)
	tst.b	d7
	bne	$000035DC

l000035DA:
	subq.l	#$01,a3

l000035DC:
	cmp.b	#$70,d7
	bne	$000035EA

l000035E2:
	move.b	#$6C,$0048(a7)
	moveq	#$78,d7

l000035EA:
	cmp.l	#$0000002D,d5
	bne	$000035F8

l000035F2:
	cmp.b	#$75,d7
	bne	$00003600

l000035F8:
	cmp.l	#$0000002B,d5
	bne	$00003650

l00003600:
	cmp.l	d3,d6
	bcs	$00003650

l00003604:
	move.l	d5,$006E(a7)
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000363A

l0000361A:
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
	bra	$00003648

l0000363A:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003648:
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4

l00003650:
	cmp.b	#$69,d7
	bne	$000037C2

l00003658:
	cmp.l	#$00000030,d5
	bne	$00003784

l00003662:
	cmp.l	d3,d6
	bcs	$00003784

l00003668:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000369A

l0000367A:
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
	bra	$000036A8

l0000369A:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l000036A8:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$00002B14,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$000036CE

l000036CA:
	or.b	#$20,d0

l000036CE:
	cmp.l	#$00000078,d0
	bne	$00003766

l000036D8:
	cmp.l	d3,d6
	bcs	$00003766

l000036DE:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003710

l000036F0:
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
	bra	$0000371E

l00003710:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l0000371E:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$0000374A

l00003744:
	cmp.l	d3,d6
	bcs	$0000374A

l00003748:
	moveq	#$78,d7

l0000374A:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$00003760

l00003754:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l00003760:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00003768

l00003766:
	moveq	#$6F,d7

l00003768:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$0000377E

l00003772:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l0000377E:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$000037C2

l00003784:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$000037C2

l000037A0:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$000037C2

l000037BC:
	cmp.l	d3,d6
	bcs	$000037C2

l000037C0:
	moveq	#$78,d7

l000037C2:
	cmp.b	#$78,d7
	bne	$000038F6

l000037CA:
	cmp.l	#$00000030,d5
	bne	$000038F6

l000037D4:
	cmp.l	d3,d6
	bcs	$000038F6

l000037DA:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000380C

l000037EC:
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
	bra	$0000381A

l0000380C:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l0000381A:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$00002B14,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$00003840

l0000383C:
	or.b	#$20,d0

l00003840:
	cmp.l	#$00000078,d0
	bne	$000038DC

l0000384A:
	cmp.l	d3,d6
	bcs	$000038DC

l00003850:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003882

l00003862:
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
	bra	$00003890

l00003882:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003890:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$000038C2

l000038B8:
	cmp.l	d3,d6
	bcs	$000038C2

l000038BC:
	move.l	$004A(a7),d5
	bra	$000038F6

l000038C2:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$000038D8

l000038CC:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l000038D8:
	subq.l	#$01,d3
	subq.l	#$01,d4

l000038DC:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$000038F2

l000038E6:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l000038F2:
	subq.l	#$01,d3
	subq.l	#$01,d4

l000038F6:
	cmp.b	#$78,d7
	beq	$00003902

l000038FC:
	cmp.b	#$58,d7
	bne	$0000390C

l00003902:
	move.l	#$00000010,$0040(a7)
	bra	$0000392A

l0000390C:
	cmp.b	#$6F,d7
	bne	$0000391C

l00003912:
	move.l	#$00000008,$0034(a7)
	bra	$00003924

l0000391C:
	move.l	#$0000000A,$0034(a7)

l00003924:
	move.l	$0034(a7),$0040(a7)

l0000392A:
	move.l	$0040(a7),$0072(a7)
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$00003B92

l0000395C:
	cmpi.l	#$0000000A,$0072(a7)
	bne	$00003992

l00003966:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$00003B92

l00003992:
	cmpi.l	#$00000008,$0072(a7)
	bne	$000039B2

l0000399C:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	cmp.l	#$00000037,d5
	bgt	$00003B92

l000039B2:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.l	d6,$0040(a7)
	move.b	d7,$004A(a7)
	cmp.l	d3,d6
	bcs	$00003B92

l000039C8:
	move.l	$0072(a7),d7
	movea.l	$0040(a7),a4

l000039D0:
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
	jsr.l	fn00003EB0
	lea	$0010(a7),a7
	movea.l	(a7)+,a1
	move.l	d0,$0048(a7)
	move.l	d1,$004C(a7)
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00003A30

l00003A26:
	move.l	d5,d4
	sub.l	#$00000030,d4
	bra	$00003A32

l00003A30:
	moveq	#$00,d4

l00003A32:
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
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000001,d0
	beq	$00003A82

l00003A78:
	move.l	d5,d6
	sub.l	#$00000037,d6
	bra	$00003A84

l00003A82:
	moveq	#$00,d6

l00003A84:
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
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000002,d0
	beq	$00003AD4

l00003ACA:
	move.l	d5,d2
	sub.l	#$00000057,d2
	bra	$00003AD6

l00003AD4:
	moveq	#$00,d2

l00003AD6:
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
	blt	$00003B28

l00003B10:
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
	bra	$00003B34

l00003B28:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,d1
	addq.w	#$04,a7

l00003B34:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,$0034(a7)
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$00003B92

l00003B58:
	cmp.l	#$0000000A,d7
	bne	$00003B7C

l00003B60:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00003B92

l00003B7C:
	cmp.l	#$00000008,d7
	bne	$00003B8C

l00003B84:
	cmp.l	#$00000037,d5
	bgt	$00003B92

l00003B8C:
	cmpa.l	d3,a4
	bcc	$000039D0

l00003B92:
	move.b	$004A(a7),d7
	move.l	$0034(a7),d4
	move.l	$0084(a7),d2
	tst.l	$006E(a7)
	beq	$00003BC6

l00003BA4:
	cmp.l	#$00000002,d3
	bne	$00003BC6

l00003BAC:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003BBE

l00003BB4:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l00003BBE:
	subq.l	#$01,d3
	subq.l	#$01,d4
	move.l	$006E(a7),d5

l00003BC6:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003BD8

l00003BCE:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l00003BD8:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003D86

l00003BE4:
	tst.l	d3
	beq	$00003D86

l00003BEA:
	cmp.b	#$75,d7
	bne	$00003CAC

l00003BF2:
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0038(a7)
	move.l	(a7)+,d0
	tst.b	$0034(a7)
	beq	$00003C1C

l00003C06:
	subq.b	#$01,$0034(a7)
	beq	$00003C74

l00003C0C:
	subi.b	#$66,$0034(a7)
	beq	$00003C58

l00003C14:
	subq.b	#$04,$0034(a7)
	beq	$00003C3C

l00003C1A:
	bra	$00003C90

l00003C1C:
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
	bra	$00003D82

l00003C3C:
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
	bra	$00003D82

l00003C58:
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
	bra	$00003D82

l00003C74:
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
	bra	$00003D82

l00003C90:
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
	bra	$00003D82

l00003CAC:
	cmpi.l	#$0000002D,$006E(a7)
	bne	$00003CC8

l00003CB6:
	movem.l	$002C(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0034(a7)
	bra	$00003CD4

l00003CC8:
	move.l	$0030(a7),$0038(a7)
	move.l	$002C(a7),$0034(a7)
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0030(a7)
	move.l	(a7)+,d0
	tst.b	$002C(a7)
	beq	$00003CFE

l00003CE8:
	subq.b	#$01,$002C(a7)
	beq	$00003D50

l00003CEE:
	subi.b	#$66,$002C(a7)
	beq	$00003D36

l00003CF6:
	subq.b	#$04,$002C(a7)
	beq	$00003D1C

l00003CFC:
	bra	$00003D6A

l00003CFE:
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
	bra	$00003D82

l00003D1C:
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
	bra	$00003D82

l00003D36:
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
	bra	$00003D82

l00003D50:
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
	bra	$00003D82

l00003D6A:
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

l00003D82:
	addq.l	#$01,$003C(a7)

l00003D86:
	movea.l	a3,a4
	bra	$00003E84

l00003D8C:
	move.b	(a4),d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	beq	$00003E18

l00003DA8:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003DD2

l00003DBA:
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
	bra	$00003DDE

l00003DD2:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,d1
	addq.w	#$04,a7

l00003DDE:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$00002B15,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003DA8

l00003E00:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003E12

l00003E08:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l00003E12:
	subq.l	#$01,d4
	moveq	#$01,d3
	bra	$00003E82

l00003E18:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003E4A

l00003E2A:
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
	bra	$00003E58

l00003E4A:
	move.l	a2,-(a7)
	jsr.l	fn00003F30
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00003E58:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	(a4),d0
	ext.w	d0
	ext.l	d0
	cmp.l	$002C(a7),d0
	beq	$00003E82

l00003E6C:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003E7E

l00003E74:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002E5C
	addq.w	#$08,a7

l00003E7E:
	subq.l	#$01,d3
	subq.l	#$01,d4

l00003E82:
	addq.l	#$01,a4

l00003E84:
	tst.l	d3
	beq	$00003E8E

l00003E88:
	tst.b	(a4)
	bne	$00002EAE

l00003E8E:
	cmp.l	#$FFFFFFFF,d5
	bne	$00003EA0

l00003E96:
	tst.l	$003C(a7)
	bne	$00003EA0

l00003E9C:
	move.l	d5,d0
	bra	$00003EA4

l00003EA0:
	move.l	$003C(a7),d0

l00003EA4:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$004C(a7),a7
	rts
00003EAE                                           00 00               ..

;; fn00003EB0: 00003EB0
;;   Called from:
;;     000039EE (in fn00002E8C)
fn00003EB0 proc
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
	bcc	$00003EE4

l00003EDE:
	add.l	#$00010000,d1

l00003EE4:
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
00003F2E                                           00 00               ..

;; fn00003F30: 00003F30
;;   Called from:
;;     00003040 (in fn00002E8C)
;;     000030FE (in fn00002E8C)
;;     0000316A (in fn00002E8C)
;;     000032CC (in fn00002E8C)
;;     00003358 (in fn00002E8C)
;;     00003450 (in fn00002E8C)
;;     000034EE (in fn00002E8C)
;;     0000363C (in fn00002E8C)
;;     0000369C (in fn00002E8C)
;;     00003712 (in fn00002E8C)
;;     0000380E (in fn00002E8C)
;;     00003884 (in fn00002E8C)
;;     00003B2A (in fn00002E8C)
;;     00003DD4 (in fn00002E8C)
;;     00003E4C (in fn00002E8C)
fn00003F30 proc
	movem.l	d2-d5/a2-a4/a6,-(a7)
	movea.l	$0024(a7),a2
	jsr.l	fn00002688
	move.l	a2,d0
	bne	$00003F48

l00003F42:
	moveq	#-$01,d0
	bra	$00004026

l00003F48:
	moveq	#$2A,d0
	and.l	$0018(a2),d0
	moveq	#$20,d5
	cmp.l	d0,d5
	beq	$00003F5A

l00003F54:
	moveq	#-$01,d0
	bra	$00004026

l00003F5A:
	lea	$0018(a2),a0
	moveq	#$01,d0
	or.l	d0,(a0)
	move.l	#$00000200,d0
	and.l	(a0),d0
	beq	$00003F72

l00003F6C:
	jsr.l	fn0000402C

l00003F72:
	tst.l	$001C(a2)
	bne	$00003F90

l00003F78:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00003F88

l00003F80:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00003F90

l00003F88:
	move.l	#$00000400,$001C(a2)

l00003F90:
	tst.l	$0008(a2)
	bne	$00003FCC

l00003F96:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00003FAA

l00003FA6:
	moveq	#$02,d4
	bra	$00003FAC

l00003FAA:
	moveq	#$01,d4

l00003FAC:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	fn000022B4
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00003FC4

l00003FC0:
	moveq	#-$01,d0
	bra	$00004026

l00003FC4:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)

l00003FCC:
	lea	$0004(a2),a0
	move.l	$0008(a2),(a0)
	move.l	$001C(a2),d3
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00004100,a6
	jsr.l	-$002A(a6)
	lea	$0014(a2),a0
	move.l	d0,(a0)
	subq.l	#$01,(a0)
	bge	$00004012

l00003FF0:
	moveq	#-$01,d0
	cmp.l	$0014(a2),d0
	bne	$00004002

l00003FF8:
	lea	$0018(a2),a0
	moveq	#$08,d0
	or.l	d0,(a0)
	bra	$0000400A

l00004002:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)

l0000400A:
	clr.l	$0014(a2)
	moveq	#-$01,d0
	bra	$00004026

l00004012:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	(a0),d0
	and.l	#$000000FF,d0

l00004026:
	movem.l	(a7)+,d2-d5/a2-a4/a6
	rts

;; fn0000402C: 0000402C
;;   Called from:
;;     00003F6C (in fn00003F30)
fn0000402C proc
	movem.l	a2,-(a7)
	movea.l	$00004260,a2
	move.l	a2,d0
	beq	$00004064

l0000403A:
	move.l	#$00000202,d0
	and.l	$0018(a2),d0
	cmp.l	#$00000202,d0
	bne	$0000405A

l0000404C:
	tst.l	(a2)
	beq	$0000405A

l00004050:
	move.l	a2,-(a7)
	jsr.l	fn00002208
	addq.w	#$04,a7

l0000405A:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$0000403A

l00004064:
	movea.l	(a7)+,a2
	rts

;; fn00004068: 00004068
;;   Called from:
;;     0000139C (in fn00001390)
fn00004068 proc
	movem.l	d2/a2-a3,-(a7)
	movea.l	$0010(a7),a2
	moveq	#$00,d2
	tst.b	(a2)
	beq	$000040E8

l00004076:
	movea.l	$00004258,a0
	lea	$0018(a0),a1
	moveq	#$02,d0
	or.l	d0,(a1)
	lea	$0014(a0),a0
	subq.l	#$01,(a0)
	blt	$000040BE

l0000408C:
	cmpi.b	#$0A,(a2)
	bne	$000040A4

l00004092:
	movea.l	$00004258,a0
	move.l	#$00000080,d0
	and.l	$0018(a0),d0
	bne	$000040BE

l000040A4:
	movea.l	$00004258,a1
	addq.l	#$04,a1
	movea.l	(a1),a0
	movea.l	a0,a3
	addq.l	#$01,a3
	move.l	a3,(a1)
	move.b	(a2),(a0)
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$000040D6

l000040BE:
	move.l	$00004258,-(a7)
	move.b	(a2),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn000020F4
	move.l	d0,d1
	addq.w	#$08,a7

l000040D6:
	moveq	#-$01,d0
	cmp.l	d1,d0
	bne	$000040E0

l000040DC:
	moveq	#-$01,d0
	bra	$000040EA

l000040E0:
	addq.l	#$01,a2
	addq.l	#$01,d2
	tst.b	(a2)
	bne	$00004076

l000040E8:
	move.l	d2,d0

l000040EA:
	movem.l	(a7)+,d2/a2-a3
	rts
000040F0 00 24 00 63 41 00 00 00 00 00 00 00 00 00 40 00 .$.cA.........@.
00004100 00 00 00 00 00 01 02 02 03 03 03 03 04 04 04 04 ................
00004110 04 04 04 04 05 05 05 05 05 05 05 05 05 05 05 05 ................
00004120 05 05 05 05 06 06 06 06 06 06 06 06 06 06 06 06 ................
00004130 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 ................
00004140 06 06 06 06 07 07 07 07 07 07 07 07 07 07 07 07 ................
00004150 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00004160 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00004170 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00004180 07 07 07 07 08 08 08 08 08 08 08 08 08 08 08 08 ................
00004190 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
000041A0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
000041B0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
000041C0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
000041D0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
000041E0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
000041F0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00004200 08 08 08 08 00 00 00 01 00 00 2C 18 00 00 00 00 ..........,.....
00004210 00 00 00 02 00 00 22 9C 00 00 2D F0 00 00 00 00 ......"...-.....
00004220 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00004230 00 00 00 00 00 00 00 00                         ........        
