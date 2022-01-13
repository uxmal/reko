;;; Segment code (00001000)

;; fn00001000: 00001000
fn00001000 proc
	bra	$0000100A
00001002       56 42 43 43 20 30 2E 39                     VBCC 0.9      

l0000100A:
	move.l	d0,d2
	movea.l	a0,a2
	lea	$0000948A,a4
	movea.l	$00000004,a6
	cmpi.w	#$0024,$0014(a6)
	bcc	$00001036

l00001020:
	lea	$00001494,a0
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
	lea	$0000948A,a4
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
	lea	$000014D0,a3
	move.l	#$000014D0,d0
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
fn0000131C proc
	movem.l	a2-a3,-(a7)
	tst.l	$000014C4
	bne	$0000134E

l00001328:
	movea.l	$000014D8,a3
	moveq	#$01,d0
	move.l	d0,$000014C4
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
	lea	$000014CC,a3
	move.l	#$000014C8,d0
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
	jsr.l	$00001468
	move.l	d0,-(a7)
	bsr	$0000131C
	lea	$000C(a7),a7
	movem.l	(a7)+,a2-a3
	rts
0000138E                                           00 00               ..

;; fn00001390: 00001390
;;   Called from:
;;     0000147E (in fn00001468)
fn00001390 proc
	movem.l	d2-d6/a2-a4,-(a7)
	movea.l	$0028(a7),a4
	movea.l	$0024(a7),a3
	movea.l	$002C(a7),a2
	moveq	#$00,d4
	bra	$0000145A

l000013A6:
	moveq	#$00,d3
	bra	$00001450

l000013AC:
	moveq	#$00,d2
	bra	$00001446

l000013B2:
	move.l	d4,d0
	lsl.l	#$04,d0
	lea	(a3,d0),a0
	move.l	d2,d0
	lsl.l	#$02,d0
	adda.l	d0,a0
	moveq	#$14,d0
	move.l	d0,d1
	move.l	d2,d5
	swap.l	d1
	swap.l	d5
	mulu.w	d2,d1
	mulu.w	d0,d5
	mulu.w	d2,d0
	add.w	d5,d1
	swap.l	d1
	clr.w	d1
	add.l	d1,d0
	lea	(a4,d0),a1
	move.l	d3,d0
	lsl.l	#$02,d0
	adda.l	d0,a1
	move.l	(a0),d0
	move.l	(a1),d1
	move.l	d0,d5
	move.l	d1,d6
	swap.l	d5
	swap.l	d6
	mulu.w	d1,d5
	mulu.w	d0,d6
	mulu.w	d1,d0
	add.w	d6,d5
	swap.l	d5
	clr.w	d5
	add.l	d5,d0
	moveq	#$14,d1
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
	lea	(a2,d1),a0
	move.l	d3,d1
	lsl.l	#$02,d1
	add.l	(a0,d1),d0
	moveq	#$14,d1
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
	lea	(a2,d1),a0
	move.l	d3,d1
	lsl.l	#$02,d1
	move.l	d0,(a0,d1)
	addq.l	#$01,d2

l00001446:
	moveq	#$04,d0
	cmp.l	d2,d0
	bgt	$000013B2

l0000144E:
	addq.l	#$01,d3

l00001450:
	moveq	#$04,d0
	cmp.l	d3,d0
	bgt	$000013AC

l00001458:
	addq.l	#$01,d4

l0000145A:
	moveq	#$05,d0
	cmp.l	d4,d0
	bgt	$000013A6

l00001462:
	movem.l	(a7)+,d2-d6/a2-a4
	rts

;; fn00001468: 00001468
;;   Called from:
;;     0000137A (in fn00001354)
fn00001468 proc
	lea	-$00F0(a7),a7
	lea	$00A0(a7),a0
	move.l	a0,-(a7)
	lea	$0054(a7),a0
	move.l	a0,-(a7)
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	bsr	$00001390
	lea	$000C(a7),a7
	lea	$00F0(a7),a7
	rts
0000148C                                     00 24 00 63             .$.c
00001490 41 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A...............
000014A0 00 00 00 00 00 00 00 00 00 00 00 00             ............    
