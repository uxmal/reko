;;; Segment code (00001000)

;; fn00001000: 00001000
fn00001000 proc
	bra	$0000100A
00001002       56 42 43 43 20 30 2E 39                     VBCC 0.9      

l0000100A:
	move.l	d0,d2
	movea.l	a0,a2
	lea	$0000BE66,a4
	movea.l	$00000004,a6
	cmpi.w	#$0024,$0014(a6)
	bcc	$00001036

l00001020:
	lea	$00003E70,a0
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
	lea	$0000BE66,a4
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
	lea	$00003FB8,a3
	move.l	#$00003FB8,d0
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
;;     00002422 (in fn00002400)
fn0000131C proc
	movem.l	a2-a3,-(a7)
	tst.l	$00003EA0
	bne	$0000134E

l00001328:
	movea.l	$00003FC8,a3
	moveq	#$01,d0
	move.l	d0,$00003EA0
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
	lea	$00003FB0,a3
	move.l	#$00003FAC,d0
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
	jsr.l	$000013D8
	move.l	d0,-(a7)
	bsr	$0000131C
	lea	$000C(a7),a7
	movem.l	(a7)+,a2-a3
	rts
0000138E                                           00 00               ..

;; fn00001390: 00001390
;;   Called from:
;;     00001394 (in fn00001394)
;;     00001396 (in fn00001394)
;;     00001398 (in fn00001394)
;;     0000139A (in fn00001394)
;;     0000139C (in fn00001394)
;;     0000139E (in fn00001394)
;;     000013A0 (in fn00001394)
;;     000013A2 (in fn00001394)
;;     000013A4 (in fn00001394)
;;     000013A6 (in fn00001394)
fn00001390 proc
	rts
00001392       00 00                                       ..            

;; fn00001394: 00001394
;;   Called from:
;;     000013AC (in fn000013AC)
;;     000013AE (in fn000013AC)
;;     000013B0 (in fn000013AC)
;;     000013B2 (in fn000013AC)
;;     000013B4 (in fn000013AC)
;;     000013B6 (in fn000013AC)
;;     000013B8 (in fn000013AC)
;;     000013BA (in fn000013AC)
;;     000013BC (in fn000013AC)
;;     000013BE (in fn000013AC)
fn00001394 proc
	bsr	$00001390
	bsr	$00001390
	bsr	$00001390
	bsr	$00001390
	bsr	$00001390
	bsr	$00001390
	bsr	$00001390
	bsr	$00001390
	bsr	$00001390
	bsr	$00001390
	rts
000013AA                               00 00                       ..    

;; fn000013AC: 000013AC
;;   Called from:
;;     000013C4 (in fn000013C4)
;;     000013C6 (in fn000013C4)
;;     000013C8 (in fn000013C4)
;;     000013CA (in fn000013C4)
;;     000013CC (in fn000013C4)
;;     000013CE (in fn000013C4)
;;     000013D0 (in fn000013C4)
;;     000013D2 (in fn000013C4)
;;     000013D4 (in fn000013C4)
fn000013AC proc
	bsr	$00001394
	bsr	$00001394
	bsr	$00001394
	bsr	$00001394
	bsr	$00001394
	bsr	$00001394
	bsr	$00001394
	bsr	$00001394
	bsr	$00001394
	bsr	$00001394
	rts
000013C2       00 00                                       ..            

;; fn000013C4: 000013C4
;;   Called from:
;;     0000140E (in fn000013D8)
fn000013C4 proc
	bsr	$000013AC
	bsr	$000013AC
	bsr	$000013AC
	bsr	$000013AC
	bsr	$000013AC
	bsr	$000013AC
	bsr	$000013AC
	bsr	$000013AC
	bsr	$000013AC
	rts

;; fn000013D8: 000013D8
;;   Called from:
;;     0000137A (in fn00001354)
fn000013D8 proc
	subq.w	#$08,a7
	movem.l	d2,-(a7)
	pea	0000142C                                               ; $004E(pc)
	jsr.l	$00003DE0
	lea	$000C(a7),a0
	move.l	a0,-(a7)
	pea	00001448                                               ; $005A(pc)
	jsr.l	$00002BB8
	move.l	$0014(a7),-(a7)
	pea	0000144C                                               ; $0050(pc)
	jsr.l	$00001474
	moveq	#$01,d2
	lea	$0014(a7),a7
	bra	$00001412

l0000140E:
	bsr	$000013C4
	addq.l	#$01,d2

l00001412:
	cmp.l	$0008(a7),d2
	ble	$0000140E

l00001418:
	pea	00001468                                               ; $0050(pc)
	jsr.l	$00003DE0
	addq.w	#$04,a7
	movem.l	(a7)+,d2
	addq.w	#$08,a7
	rts
0000142C                                     65 6E 74 65             ente
00001430 72 20 6E 75 6D 62 65 72 20 6F 66 20 69 74 65 72 r number of iter
00001440 61 74 69 6F 6E 73 20 00 25 6C 64 00 65 78 65 63 ations .%ld.exec
00001450 75 74 69 6E 67 20 25 6C 64 20 69 74 65 72 61 74 uting %ld iterat
00001460 69 6F 6E 73 0A 00 00 00 66 69 6E 69 73 68 65 64 ions....finished
00001470 0A 00 00 00                                     ....            

;; fn00001474: 00001474
;;   Called from:
;;     00001400 (in fn000013D8)
fn00001474 proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00003FD0,-(a7)
	jsr.l	$00001490
	lea	$000C(a7),a7
	rts

;; fn00001490: 00001490
;;   Called from:
;;     00001484 (in fn00001474)
fn00001490 proc
	lea	-$0044(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$007C(a7),d3
	movea.l	$0074(a7),a5
	movea.l	$0078(a7),a4
	moveq	#$00,d6
	tst.b	(a4)
	beq	$00001DD6

l000014AC:
	cmpi.b	#$25,(a4)
	bne	$00001DB0

l000014B4:
	clr.l	$0040(a7)
	moveq	#-$01,d5
	clr.l	$0048(a7)
	moveq	#$69,d4
	lea	$004C(a7),a3
	moveq	#$00,d7
	clr.l	$0066(a7)
	lea	$0001(a4),a2
	move.l	$0048(a7),d2

l000014D2:
	moveq	#$00,d1

l000014D4:
	lea	00001DE8,a0                                            ; $0914(pc)
	move.l	d0,-(a7)
	move.b	(a0,d1),d0
	cmp.b	(a2),d0
	movem.l	(a7)+,d0
	bne	$000014F8

l000014E6:
	move.l	d1,d0
	move.l	d1,-(a7)
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	move.l	(a7)+,d1
	or.l	d0,d2
	addq.l	#$01,a2
	bra	$00001502

l000014F8:
	addq.l	#$01,d1
	cmp.l	#$00000005,d1
	bcs	$000014D4

l00001502:
	cmp.l	#$00000005,d1
	bcs	$000014D2

l0000150A:
	move.l	d2,$0048(a7)
	cmpi.b	#$2A,(a2)
	bne	$00001548

l00001514:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	bge	$00001540

l0000152C:
	ori.l	#$00000004,$0048(a7)
	move.l	$002C(a7),d0
	neg.l	d0
	move.l	d0,$0040(a7)
	bra	$000015B4

l00001540:
	move.l	$002C(a7),$0040(a7)
	bra	$000015B4

l00001548:
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$000015B4

l00001564:
	move.l	$0040(a7),d2

l00001568:
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
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00001568

l000015B0:
	move.l	d2,$0040(a7)

l000015B4:
	cmpi.b	#$2E,(a2)
	bne	$00001648

l000015BC:
	addq.l	#$01,a2
	cmpi.b	#$2A,(a2)
	bne	$000015E2

l000015C4:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	blt	$00001648

l000015DC:
	move.l	$002C(a7),d5
	bra	$00001648

l000015E2:
	moveq	#$00,d5
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00001648

l00001600:
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
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00001600

l00001648:
	cmpi.b	#$68,(a2)
	beq	$0000166C

l0000164E:
	cmpi.b	#$6C,(a2)
	beq	$0000166C

l00001654:
	cmpi.b	#$4C,(a2)
	beq	$0000166C

l0000165A:
	cmpi.b	#$6A,(a2)
	beq	$0000166C

l00001660:
	cmpi.b	#$7A,(a2)
	beq	$0000166C

l00001666:
	cmpi.b	#$74,(a2)
	bne	$00001672

l0000166C:
	move.b	(a2)+,d4
	ext.w	d4
	ext.l	d4

l00001672:
	cmp.l	#$00000068,d4
	bne	$00001684

l0000167A:
	cmpi.b	#$68,(a2)
	bne	$00001684

l00001680:
	moveq	#$02,d4
	addq.l	#$01,a2

l00001684:
	cmp.l	#$0000006C,d4
	bne	$00001696

l0000168C:
	cmpi.b	#$6C,(a2)
	bne	$00001696

l00001692:
	moveq	#$01,d4
	addq.l	#$01,a2

l00001696:
	cmp.l	#$0000006A,d4
	bne	$000016A0

l0000169E:
	moveq	#$01,d4

l000016A0:
	cmp.l	#$0000007A,d4
	bne	$000016AA

l000016A8:
	moveq	#$6C,d4

l000016AA:
	cmp.l	#$00000074,d4
	bne	$000016B4

l000016B2:
	moveq	#$69,d4

l000016B4:
	move.b	(a2)+,d1
	move.b	d1,d0
	cmp.b	#$25,d1
	beq	$00001B3C

l000016C0:
	cmp.b	#$58,d0
	beq	$0000170E

l000016C6:
	cmp.b	#$63,d0
	beq	$00001ABC

l000016CE:
	cmp.b	#$64,d0
	beq	$0000170E

l000016D4:
	cmp.b	#$69,d0
	beq	$0000170E

l000016DA:
	move.b	d0,$002C(a7)
	cmp.b	#$6E,d0
	beq	$00001B4E

l000016E6:
	move.b	$002C(a7),d0
	sub.b	#$6F,d0
	cmp.b	#$01,d0
	bls	$0000170E

l000016F4:
	move.b	$002C(a7),d0
	cmp.b	#$73,d0
	beq	$00001AF8

l00001700:
	cmp.b	#$75,d0
	beq	$0000170E

l00001706:
	cmp.b	#$78,d0
	bne	$00001BE6

l0000170E:
	cmp.b	#$70,d1
	bne	$00001720

l00001714:
	moveq	#$6C,d4
	moveq	#$78,d1
	ori.l	#$00000001,$0048(a7)

l00001720:
	cmp.b	#$64,d1
	beq	$0000172E

l00001726:
	cmp.b	#$69,d1
	bne	$00001878

l0000172E:
	cmp.l	#$00000001,d4
	bne	$00001754

l00001736:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$0034(a7)
	move.l	-$0008(a0),$0030(a7)
	bra	$000017EC

l00001754:
	cmp.l	#$0000006C,d4
	bne	$00001780

l0000175C:
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
	bra	$000017EC

l00001780:
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
	bne	$000017C6

l000017AA:
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

l000017C6:
	cmp.l	#$00000002,d4
	bne	$000017EC

l000017CE:
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

l000017EC:
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
	bge	$00001836

l00001816:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2D,(a0)
	movem.l	$0030(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0038(a7)
	bra	$000019AC

l00001836:
	move.b	$002C(a7),d1
	moveq	#$10,d0
	and.l	$0048(a7),d0
	beq	$00001850

l00001842:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2B,(a0)
	bra	$00001864

l00001850:
	moveq	#$08,d0
	and.l	$0048(a7),d0
	beq	$00001864

l00001858:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$20,(a0)

l00001864:
	move.l	$0034(a7),$003C(a7)
	move.l	$0030(a7),$0038(a7)
	move.b	d1,$002C(a7)
	bra	$000019AC

l00001878:
	cmp.l	#$00000001,d4
	bne	$0000189C

l00001880:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	move.l	-$0008(a0),$0038(a7)
	bra	$000018D6

l0000189C:
	cmp.l	#$0000006C,d4
	bne	$000018BE

l000018A4:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)
	bra	$000018D6

l000018BE:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)

l000018D6:
	cmp.l	#$00000068,d4
	bne	$000018F2

l000018DE:
	move.w	$003E(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.w	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l000018F2:
	cmp.l	#$00000002,d4
	bne	$0000190E

l000018FA:
	move.b	$003F(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l0000190E:
	moveq	#$01,d0
	and.l	$0048(a7),d0
	move.b	d1,$002C(a7)
	tst.l	d0
	beq	$000019AC

l0000191E:
	cmp.b	#$6F,d1
	bne	$0000195A

l00001924:
	tst.l	d5
	bne	$0000194E

l00001928:
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
	beq	$0000195A

l0000194E:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$30,(a0)

l0000195A:
	cmp.b	#$78,d1
	beq	$0000196A

l00001960:
	move.b	d1,$002C(a7)
	cmp.b	#$58,d1
	bne	$000019AC

l0000196A:
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
	beq	$000019AC

l00001994:
	lea	$006A(a7),a0
	lea	(a0,d7),a1
	addq.l	#$01,d7
	move.b	#$30,(a1)
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	d1,(a0)
	move.b	d1,$002C(a7)

l000019AC:
	move.b	$002C(a7),d1
	lea	$0062(a7),a3
	cmp.b	#$78,d1
	beq	$000019C0

l000019BA:
	cmp.b	#$58,d1
	bne	$000019CA

l000019C0:
	move.l	#$00000010,$002C(a7)
	bra	$000019E8

l000019CA:
	cmp.b	#$6F,d1
	bne	$000019DA

l000019D0:
	move.l	#$00000008,$0030(a7)
	bra	$000019E2

l000019DA:
	move.l	#$0000000A,$0030(a7)

l000019E2:
	move.l	$0030(a7),$002C(a7)

l000019E8:
	move.l	$002C(a7),$006C(a7)
	cmp.b	#$58,d1
	beq	$000019FA

l000019F4:
	lea	00001DF0,a6                                            ; $03FC(pc)
	bra	$000019FE

l000019FA:
	lea	00001E00,a6                                            ; $0406(pc)

l000019FE:
	move.l	a6,$002C(a7)
	move.l	d3,$007C(a7)
	move.l	d5,$0044(a7)
	move.l	d6,$0030(a7)
	move.l	d7,$0062(a7)
	movem.l	$0038(a7),d6-d7
	move.l	$0066(a7),d3
	movea.l	$002C(a7),a1

l00001A20:
	move.l	$006C(a7),d1
	move.l	d1,d0
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	a1,-(a7)
	movem.l	d0-d1,-(a7)
	movem.l	d6-d7,-(a7)
	jsr.l	$00002778
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
	jsr.l	$00002430
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
	bne	$00001A20

l00001A8E:
	move.l	d3,$0066(a7)
	move.l	$0062(a7),d7
	move.l	$0030(a7),d6
	move.l	$0044(a7),d5
	move.l	$007C(a7),d3
	cmp.l	#$FFFFFFFF,d5
	bne	$00001AB0

l00001AAA:
	moveq	#$00,d5
	bra	$00001BFC

l00001AB0:
	andi.l	#$FFFFFFFD,$0048(a7)
	bra	$00001BFC

l00001ABC:
	cmp.l	#$0000006C,d4
	bne	$00001AD8

l00001AC4:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)
	bra	$00001AEA

l00001AD8:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)

l00001AEA:
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001BFC

l00001AF8:
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
	beq	$00001B1A

l00001B14:
	cmp.l	$0066(a7),d5
	bls	$00001B36

l00001B1A:
	tst.b	(a1)
	beq	$00001B36

l00001B1E:
	move.l	$0066(a7),d0

l00001B22:
	addq.l	#$01,d0
	addq.l	#$01,a1
	tst.l	d5
	bls	$00001B2E

l00001B2A:
	cmp.l	d0,d5
	bls	$00001B32

l00001B2E:
	tst.b	(a1)
	bne	$00001B22

l00001B32:
	move.l	d0,$0066(a7)

l00001B36:
	moveq	#$00,d5
	bra	$00001BFC

l00001B3C:
	lea	00001DE4,a3                                            ; $02A8(pc)
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001BFC

l00001B4E:
	cmp.l	#$00000001,d4
	bne	$00001B70

l00001B56:
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
	bra	$00001BDE

l00001B70:
	cmp.l	#$0000006C,d4
	bne	$00001B8E

l00001B78:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)
	bra	$00001BDE

l00001B8E:
	cmp.l	#$00000068,d4
	bne	$00001BAC

l00001B96:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.w	d6,(a0)
	bra	$00001BDE

l00001BAC:
	cmp.l	#$00000002,d4
	bne	$00001BCA

l00001BB4:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.b	d6,(a0)
	bra	$00001BDE

l00001BCA:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)

l00001BDE:
	moveq	#$00,d5
	clr.l	$0040(a7)
	bra	$00001BFC

l00001BE6:
	tst.b	d1
	bne	$00001BEC

l00001BEA:
	subq.l	#$01,a2

l00001BEC:
	movea.l	a4,a3
	move.l	a2,d0
	sub.l	a4,d0
	move.l	d0,$0066(a7)
	moveq	#$00,d5
	clr.l	$0040(a7)

l00001BFC:
	cmp.l	$0066(a7),d5
	bhi	$00001C0A

l00001C02:
	move.l	$0066(a7),$002C(a7)
	bra	$00001C0E

l00001C0A:
	move.l	d5,$002C(a7)

l00001C0E:
	move.l	d0,-(a7)
	move.l	$0030(a7),d0
	add.l	d7,d0
	move.l	d0,$0034(a7)
	move.l	(a7)+,d0
	move.l	d0,-(a7)
	move.l	$0034(a7),d0
	cmp.l	$0044(a7),d0
	movem.l	(a7)+,d0
	bcs	$00001C32

l00001C2C:
	clr.l	$002C(a7)
	bra	$00001C42

l00001C32:
	move.l	d0,-(a7)
	move.l	$0044(a7),d0
	sub.l	$0034(a7),d0
	move.l	d0,$0030(a7)
	move.l	(a7)+,d0

l00001C42:
	move.l	$002C(a7),$0030(a7)
	moveq	#$02,d0
	and.l	$0048(a7),d0
	beq	$00001C84

l00001C50:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001C84

l00001C56:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001E10
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001C7C

l00001C76:
	move.l	d6,d0
	bra	$00001DD8

l00001C7C:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001C56

l00001C84:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	bne	$00001CD6

l00001C8C:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001CD6

l00001C94:
	move.l	$0048(a7),d4
	movea.l	$0030(a7),a4

l00001C9C:
	move.l	a5,-(a7)
	moveq	#$02,d0
	and.l	d4,d0
	beq	$00001CAA

l00001CA4:
	movea.w	#$0030,a0
	bra	$00001CAE

l00001CAA:
	movea.w	#$0020,a0

l00001CAE:
	move.l	a0,-(a7)
	jsr.l	$00001E10
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001CC6

l00001CC0:
	move.l	d6,d0
	bra	$00001DD8

l00001CC6:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001C9C

l00001CCE:
	move.l	a4,$0030(a7)
	move.l	d4,$0048(a7)

l00001CD6:
	moveq	#$02,d0
	and.l	$0048(a7),d0
	bne	$00001D12

l00001CDE:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001D12

l00001CE4:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001E10
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D0A

l00001D04:
	move.l	d6,d0
	bra	$00001DD8

l00001D0A:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001CE4

l00001D12:
	move.l	$0066(a7),d2
	cmp.l	$0066(a7),d5
	bls	$00001D40

l00001D1C:
	move.l	a5,-(a7)
	pea	$00000030
	jsr.l	$00001E10
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D38

l00001D32:
	move.l	d6,d0
	bra	$00001DD8

l00001D38:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d5
	bhi	$00001D1C

l00001D40:
	moveq	#$00,d2
	tst.l	$0066(a7)
	beq	$00001D76

l00001D48:
	movea.l	$0066(a7),a4

l00001D4C:
	move.l	a5,-(a7)
	lea	(a3,d2),a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001E10
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D6E

l00001D6A:
	move.l	d6,d0
	bra	$00001DD8

l00001D6E:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001D4C

l00001D76:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	beq	$00001DAC

l00001D7E:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001DAC

l00001D86:
	movea.l	$0030(a7),a3

l00001D8A:
	move.l	a5,-(a7)
	pea	$00000020
	jsr.l	$00001E10
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001DA4

l00001DA0:
	move.l	d6,d0
	bra	$00001DD8

l00001DA4:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a3
	bhi	$00001D8A

l00001DAC:
	movea.l	a2,a4
	bra	$00001DD0

l00001DB0:
	move.l	a5,-(a7)
	move.b	(a4)+,d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001E10
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001DCE

l00001DCA:
	move.l	d6,d0
	bra	$00001DD8

l00001DCE:
	addq.l	#$01,d6

l00001DD0:
	tst.b	(a4)
	bne	$000014AC

l00001DD6:
	move.l	d6,d0

l00001DD8:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$0044(a7),a7
	rts
00001DE2       00 00 25 00 00 00 23 30 2D 20 2B 00 00 00   ..%...#0- +...
00001DF0 30 31 32 33 34 35 36 37 38 39 61 62 63 64 65 66 0123456789abcdef
00001E00 30 31 32 33 34 35 36 37 38 39 41 42 43 44 45 46 0123456789ABCDEF

;; fn00001E10: 00001E10
;;   Called from:
;;     00001C66 (in fn00001490)
;;     00001CB0 (in fn00001490)
;;     00001CF4 (in fn00001490)
;;     00001D22 (in fn00001490)
;;     00001D5A (in fn00001490)
;;     00001D90 (in fn00001490)
;;     00001DBA (in fn00001490)
fn00001E10 proc
	movem.l	d2/a2-a3,-(a7)
	move.l	$0010(a7),d2
	movea.l	$0014(a7),a2
	lea	$0018(a2),a0
	moveq	#$02,d0
	or.l	d0,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001E54

l00001E2C:
	moveq	#$0A,d0
	cmp.l	d2,d0
	bne	$00001E3E

l00001E32:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	bne	$00001E54

l00001E3E:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a3
	addq.l	#$01,a3
	move.l	a3,(a1)
	move.b	d2,(a0)
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00001E62

l00001E54:
	move.l	a2,-(a7)
	move.l	d2,-(a7)
	jsr.l	$00001E6C
	move.l	d0,d1
	addq.w	#$08,a7

l00001E62:
	move.l	d1,d0
	movem.l	(a7)+,d2/a2-a3
	rts
00001E6A                               00 00                       ..    

;; fn00001E6C: 00001E6C
;;   Called from:
;;     00001E58 (in fn00001E10)
;;     00003E44 (in fn00003DE0)
fn00001E6C proc
	movem.l	d2-d6/a2-a4/a6,-(a7)
	move.l	$0028(a7),d5
	movea.l	$002C(a7),a2
	jsr.l	$00002400
	move.l	a2,d0
	bne	$00001E88

l00001E82:
	moveq	#-$01,d0
	bra	$00001F7A

l00001E88:
	moveq	#$49,d0
	and.l	$0018(a2),d0
	moveq	#$40,d6
	cmp.l	d0,d6
	beq	$00001E9A

l00001E94:
	moveq	#-$01,d0
	bra	$00001F7A

l00001E9A:
	tst.l	$001C(a2)
	bne	$00001EB8

l00001EA0:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00001EB0

l00001EA8:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00001EB8

l00001EB0:
	move.l	#$00000400,$001C(a2)

l00001EB8:
	tst.l	$0008(a2)
	bne	$00001EF8

l00001EBE:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00001ED2

l00001ECE:
	moveq	#$02,d4
	bra	$00001ED4

l00001ED2:
	moveq	#$01,d4

l00001ED4:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	$0000202C
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00001EEE

l00001EE8:
	moveq	#-$01,d0
	bra	$00001F7A

l00001EEE:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)
	bra	$00001F56

l00001EF8:
	tst.l	(a2)
	beq	$00001F52

l00001EFC:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00001F28

l00001F08:
	moveq	#$0A,d0
	cmp.l	d5,d0
	bne	$00001F28

l00001F0E:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	d5,(a0)
	move.l	a2,-(a7)
	jsr.l	$00001F80
	addq.w	#$04,a7
	bra	$00001F7A

l00001F28:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003E78,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00001F56

l00001F46:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00001F7A

l00001F52:
	moveq	#$00,d0
	bra	$00001F7A

l00001F56:
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

l00001F7A:
	movem.l	(a7)+,d2-d6/a2-a4/a6
	rts

;; fn00001F80: 00001F80
;;   Called from:
;;     00001F1E (in fn00001E6C)
;;     00002B82 (in fn00002B74)
;;     00002BA0 (in fn00002B74)
;;     00003DCA (in fn00003DA4)
fn00001F80 proc
	movem.l	d2-d4/a2/a6,-(a7)
	movea.l	$0018(a7),a2
	jsr.l	$00002400
	move.l	a2,d0
	bne	$00001F96

l00001F92:
	moveq	#-$01,d0
	bra	$0000200E

l00001F96:
	tst.l	$001C(a2)
	bne	$00001FB4

l00001F9C:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00001FAC

l00001FA4:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00001FB4

l00001FAC:
	move.l	#$00000400,$001C(a2)

l00001FB4:
	tst.l	$0008(a2)
	bne	$00001FBE

l00001FBA:
	moveq	#$00,d0
	bra	$0000200E

l00001FBE:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002004

l00001FC6:
	tst.l	(a2)
	beq	$00001FF4

l00001FCA:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003E78,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00001FF8

l00001FE8:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$0000200E

l00001FF4:
	moveq	#$00,d0
	bra	$0000200E

l00001FF8:
	move.l	$0008(a2),$0004(a2)
	move.l	$001C(a2),$0014(a2)

l00002004:
	lea	$0018(a2),a0
	moveq	#-$04,d0
	and.l	d0,(a0)
	moveq	#$00,d0

l0000200E:
	movem.l	(a7)+,d2-d4/a2/a6
	rts
00002014             4A B9 00 00 3E A8 67 0E 2F 39 00 00     J...>.g./9..
00002020 3E A8 4E B9 00 00 23 90 58 4F 4E 75             >.N...#.XONu    

;; fn0000202C: 0000202C
;;   Called from:
;;     00001EDA (in fn00001E6C)
;;     00003D2A (in fn00003CA8)
fn0000202C proc
	movem.l	d2,-(a7)
	move.l	$0008(a7),d2
	bne	$0000203A

l00002036:
	moveq	#$00,d0
	bra	$00002090

l0000203A:
	tst.l	$00003EA8
	bne	$0000205E

l00002042:
	movea.l	$00003EA4,a0
	move.l	a0,-(a7)
	move.l	a0,-(a7)
	clr.l	-(a7)
	jsr.l	$00002320
	move.l	d0,$00003EA8
	lea	$000C(a7),a7

l0000205E:
	tst.l	$00003EA8
	bne	$0000206A

l00002066:
	moveq	#$00,d0
	bra	$00002090

l0000206A:
	moveq	#$04,d0
	add.l	d2,d0
	move.l	d0,-(a7)
	move.l	$00003EA8,-(a7)
	jsr.l	$000021FC
	movea.l	d0,a1
	addq.w	#$08,a7
	move.l	a1,d0
	bne	$00002088

l00002084:
	moveq	#$00,d0
	bra	$00002090

l00002088:
	move.l	d2,(a1)
	lea	$0004(a1),a0
	move.l	a0,d0

l00002090:
	movem.l	(a7)+,d2
	rts
00002096                   00 00                               ..        

;; fn00002098: 00002098
fn00002098 proc
	move.l	$0004(a7),d0
	movea.l	d0,a0
	tst.l	d0
	beq	$000020C2

l000020A2:
	tst.l	$00003EA8
	beq	$000020C2

l000020AA:
	moveq	#$04,d0
	add.l	-(a0),d0
	move.l	d0,-(a7)
	move.l	a0,-(a7)
	move.l	$00003EA8,-(a7)
	jsr.l	$00002160
	lea	$000C(a7),a7

l000020C2:
	rts
000020C4             48 E7 30 38 28 6F 00 1C 24 6F 00 18     H.08(o..$o..
000020D0 22 0A 66 0A 2F 0C 61 00 FF 54 58 4F 60 7A 26 6A ".f./.a..TXO`z&j
000020E0 FF FC 2F 0C 61 00 FF 46 26 00 58 4F 67 68 B9 CB ../.a..F&.XOgh..
000020F0 64 04 20 0C 60 02 20 0B 20 43 22 4A 24 00 B4 BC d. .`. . C"J$...
00002100 00 00 00 10 65 3C 20 08 22 09 C0 3C 00 01 C2 3C ....e< ."..<...<
00002110 00 01 B2 00 66 1A 20 08 4A 01 67 04 10 D9 53 82 ....f. .J.g...S.
00002120 72 03 C2 82 94 81 20 D9 59 82 66 FA 34 01 60 14 r..... .Y.f.4.`.
00002130 B4 BC 00 01 00 00 65 0A 20 08 10 D9 53 82 66 FA ......e. ...S.f.
00002140 60 0C 20 08 53 42 65 06 10 D9 51 CA FF FC 2F 0A `. .SBe...Q.../.
00002150 61 00 FF 46 58 4F 20 03 4C DF 1C 0C 4E 75 00 00 a..FXO .L...Nu..

;; fn00002160: 00002160
;;   Called from:
;;     000020B8 (in fn00002098)
fn00002160 proc
	movem.l	d2/a2-a6,-(a7)
	move.l	$0020(a7),d1
	movea.l	$0024(a7),a5
	movea.l	$001C(a7),a4
	movea.l	$00003E74,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002190

l0000217E:
	movea.l	$00003E74,a6
	movea.l	a4,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$02CA(a6)
	bra	$000021F4

l00002190:
	move.l	a4,d2
	beq	$000021F4

l00002194:
	tst.l	d1
	beq	$000021F4

l00002198:
	movea.l	d1,a3
	lea	-$000C(a3),a3
	cmpa.l	$0014(a4),a5
	bcc	$000021DA

l000021A4:
	movea.l	a4,a2

l000021A6:
	movea.l	(a2),a2
	tst.l	(a2)
	beq	$000021F4

l000021AC:
	tst.b	$0008(a2)
	beq	$000021A6

l000021B2:
	cmp.l	$0014(a2),d1
	bcs	$000021A6

l000021B8:
	cmp.l	$0018(a2),d1
	bcc	$000021A6

l000021BE:
	movea.l	$00003E74,a6
	movea.l	a2,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$00C0(a6)
	move.l	$001C(a2),d0
	cmp.l	$0010(a4),d0
	bne	$000021F4

l000021D8:
	movea.l	a2,a3

l000021DA:
	movea.l	$00003E74,a6
	movea.l	a3,a1
	jsr.l	-$00FC(a6)
	move.l	-(a3),d0
	movea.l	$00003E74,a6
	movea.l	a3,a1
	jsr.l	-$00D2(a6)

l000021F4:
	movem.l	(a7)+,d2/a2-a6
	rts
000021FA                               00 00                       ..    

;; fn000021FC: 000021FC
;;   Called from:
;;     00002076 (in fn0000202C)
fn000021FC proc
	movem.l	d2-d4/a2-a6,-(a7)
	move.l	$0028(a7),d2
	movea.l	$0024(a7),a4
	movea.l	$00003E74,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002228

l00002216:
	movea.l	$00003E74,a6
	movea.l	a4,a0
	move.l	d2,d0
	jsr.l	-$02C4(a6)
	bra	$0000231A

l00002228:
	suba.l	a3,a3
	move.l	a4,d4
	beq	$00002318

l00002230:
	tst.l	d2
	beq	$00002318

l00002236:
	cmp.l	$0014(a4),d2
	bcc	$000022EA

l0000223E:
	movea.l	(a4),a5

l00002240:
	tst.l	(a5)
	beq	$00002262

l00002244:
	tst.b	$0008(a5)
	beq	$0000225E

l0000224A:
	movea.l	$00003E74,a6
	movea.l	a5,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3
	move.l	a3,d0
	bne	$000022CE

l0000225E:
	movea.l	(a5),a5
	bra	$00002240

l00002262:
	moveq	#$28,d3
	add.l	$0010(a4),d3
	move.l	$000C(a4),d1
	movea.l	$00003E74,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$00002318

l00002280:
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
	movea.l	$00003E74,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F0(a6)
	movea.l	$00003E74,a6
	movea.l	a3,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3

l000022CE:
	move.l	#$00010000,d0
	and.l	$000C(a4),d0
	beq	$00002318

l000022DA:
	movea.l	a3,a2
	addq.l	#$07,d2
	lsr.l	#$03,d2

l000022E0:
	clr.l	(a2)+
	clr.l	(a2)+
	subq.l	#$01,d2
	bne	$000022E0

l000022E8:
	bra	$00002318

l000022EA:
	moveq	#$10,d3
	add.l	d2,d3
	move.l	$000C(a4),d1
	movea.l	$00003E74,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$00002318

l00002304:
	move.l	d3,(a3)+
	movea.l	$00003E74,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F6(a6)
	addq.l	#$08,a3
	clr.l	(a3)+

l00002318:
	move.l	a3,d0

l0000231A:
	movem.l	(a7)+,d2-d4/a2-a6
	rts

;; fn00002320: 00002320
;;   Called from:
;;     0000204E (in fn0000202C)
fn00002320 proc
	movem.l	d2-d3/a2/a6,-(a7)
	move.l	$0018(a7),d3
	movea.l	$001C(a7),a2
	movea.l	$00003E74,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$0000234E

l0000233A:
	movea.l	$00003E74,a6
	move.l	$0014(a7),d0
	move.l	d3,d1
	move.l	a2,d2
	jsr.l	-$02B8(a6)
	bra	$0000238A

l0000234E:
	suba.l	a1,a1
	cmp.l	a2,d3
	bcs	$00002388

l00002354:
	addq.l	#$07,d3
	movea.l	$00003E74,a6
	moveq	#$18,d0
	moveq	#$00,d1
	jsr.l	-$00C6(a6)
	movea.l	d0,a1
	move.l	a1,d0
	beq	$00002388

l0000236A:
	lea	$0004(a1),a0
	move.l	a0,(a1)
	clr.l	(a0)
	move.l	a1,$0008(a1)
	move.l	$0014(a7),$000C(a1)
	moveq	#-$08,d0
	and.l	d3,d0
	move.l	d0,$0010(a1)
	move.l	a2,$0014(a1)

l00002388:
	move.l	a1,d0

l0000238A:
	movem.l	(a7)+,d2-d3/a2/a6
	rts

;; fn00002390: 00002390
fn00002390 proc
	movem.l	d2/a2/a6,-(a7)
	move.l	$0010(a7),d2
	movea.l	$00003E74,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$000023B4

l000023A6:
	movea.l	$00003E74,a6
	movea.l	d2,a0
	jsr.l	-$02BE(a6)
	bra	$000023F8

l000023B4:
	tst.l	d2
	beq	$000023F8

l000023B8:
	movea.l	$00003E74,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d1
	beq	$000023EA

l000023CA:
	move.l	-(a2),d0
	movea.l	$00003E74,a6
	movea.l	a2,a1
	jsr.l	-$00D2(a6)
	movea.l	$00003E74,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d0
	bne	$000023CA

l000023EA:
	movea.l	$00003E74,a6
	movea.l	d2,a1
	moveq	#$18,d0
	jsr.l	-$00D2(a6)

l000023F8:
	movem.l	(a7)+,d2/a2/a6
	rts
000023FE                                           00 00               ..

;; fn00002400: 00002400
;;   Called from:
;;     00001E78 (in fn00001E6C)
;;     00001F88 (in fn00001F80)
;;     00003CB0 (in fn00003CA8)
fn00002400 proc
	movem.l	a6,-(a7)
	movea.l	$00003E74,a6
	moveq	#$00,d0
	move.l	#$00001000,d1
	jsr.l	-$0132(a6)
	and.l	#$00001000,d0
	beq	$0000242A

l0000241E:
	pea	$00000014
	jsr.l	$0000131C
	addq.w	#$04,a7

l0000242A:
	movea.l	(a7)+,a6
	rts
0000242E                                           00 00               ..

;; fn00002430: 00002430
;;   Called from:
;;     00001A64 (in fn00001490)
fn00002430 proc
	movem.l	d2-d6,-(a7)
	move.l	$001C(a7),d1
	move.l	$0018(a7),d0
	movea.l	d1,a0
	move.l	$0024(a7),d3
	move.l	$0020(a7),d2
	bne	$00002486

l00002448:
	cmp.l	d3,d0
	bcc	$0000245A

l0000244C:
	move.l	d3,d2
	jsr.l	$00002534
	move.l	d0,d1
	bra	$0000252C

l0000245A:
	tst.l	d3
	bne	$00002466

l0000245E:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l00002466:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	$00002534
	movea.l	d0,a1
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	$00002534
	move.l	d0,d1
	move.l	a1,d0
	bra	$0000252E

l00002486:
	cmp.l	d2,d0
	bcc	$00002490

l0000248A:
	moveq	#$00,d0
	bra	$0000252C

l00002490:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000024AE

l0000249A:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000024AE

l000024A2:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000024AE

l000024AA:
	moveq	#$00,d4
	move.b	d2,d6

l000024AE:
	lea	$00003EAC,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$000024CE

l000024C2:
	cmp.l	d0,d2
	bcs	$000024CA

l000024C6:
	cmp.l	a0,d3
	bhi	$0000248A

l000024CA:
	moveq	#$01,d0
	bra	$0000252C

l000024CE:
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
	jsr.l	$00002534
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
	bcs	$0000252A

l00002524:
	bne	$0000252C

l00002526:
	cmpa.l	d2,a0
	bcc	$0000252C

l0000252A:
	subq.l	#$01,d1

l0000252C:
	moveq	#$00,d0

l0000252E:
	movem.l	(a7)+,d2-d6
	rts

;; fn00002534: 00002534
;;   Called from:
;;     0000244E (in fn00002430)
;;     0000246C (in fn00002430)
;;     00002478 (in fn00002430)
;;     000024EA (in fn00002430)
;;     00002796 (in fn00002778)
;;     000027B4 (in fn00002778)
;;     000027BE (in fn00002778)
;;     0000282C (in fn00002778)
fn00002534 proc
	movem.l	d5-d7,-(a7)
	move.l	d2,d7
	beq	$0000254E

l0000253C:
	move.l	d1,d6
	move.l	d0,d5
	bne	$0000255C

l00002542:
	tst.l	d1
	beq	$0000267A

l00002548:
	cmp.l	d1,d2
	bhi	$0000267A

l0000254E:
	move.l	d1,d0
	move.l	d2,d1
	jsr.l	$000026F2
	bra	$0000267A

l0000255C:
	swap.l	d2
	tst.w	d2
	bne	$00002584

l00002562:
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
	bra	$0000267A

l00002584:
	movem.l	d2-d4/a0-a1,-(a7)
	subq.l	#$08,a7
	clr.b	$0002(a7)
	moveq	#$00,d1
	moveq	#$00,d0
	tst.l	d7
	bmi	$000025A0

l00002596:
	addq.w	#$01,d0
	add.l	d6,d6
	addx.l	d5,d5
	add.l	d7,d7
	bpl	$00002596

l000025A0:
	move.w	d0,(a7)

l000025A2:
	move.l	d7,d3
	move.l	d5,d2
	swap.l	d2
	swap.l	d3
	cmp.w	d3,d2
	bne	$000025B4

l000025AE:
	move.w	#$FFFF,d1
	bra	$000025BE

l000025B4:
	move.l	d5,d1
	divu.w	d3,d1
	swap.l	d1
	clr.w	d1
	swap.l	d1

l000025BE:
	movea.l	d6,a1
	clr.w	d6
	swap.l	d6

l000025C4:
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
	bne	$000025E4

l000025DC:
	cmp.l	d4,d2
	bls	$000025E4

l000025E0:
	subq.l	#$01,d1
	bra	$000025C4

l000025E4:
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
	bcc	$0000263C

l00002626:
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

l0000263C:
	tst.b	$0002(a7)
	bne	$00002658

l00002642:
	move.w	d1,$0004(a7)
	moveq	#$00,d1
	swap.l	d5
	swap.l	d6
	move.w	d6,d5
	clr.w	d6
	st	$0002(a7)
	bra	$000025A2

l00002658:
	move.l	$0004(a7),d0
	move.w	d1,d0
	move.w	d5,d6
	swap.l	d6
	swap.l	d5
	move.w	(a7),d7
	beq	$00002672

l00002668:
	subq.w	#$01,d7

l0000266A:
	lsr.l	#$01,d5
	roxr.l	#$01,d6
	dbra	d7,$0000266A

l00002672:
	move.l	d6,d1
	addq.l	#$08,a7
	movem.l	(a7)+,d2-d4/a0-a1

l0000267A:
	movem.l	(a7)+,d5-d7
	rts
00002680 4C EF 00 03 00 04 4A 81 6B 0A 4A 80 6B 12 61 62 L.....J.k.J.k.ab
00002690 20 01 4E 75 44 81 4A 80 6B 10 61 56 20 01 4E 75  .NuD.J.k.aV .Nu
000026A0 44 80 61 4E 44 81 20 01 4E 75 44 80 61 44 44 81 D.aND. .NuD.aDD.
000026B0 20 01 4E 75 4C EF 00 03 00 04 61 36 20 01 4E 75  .NuL.....a6 .Nu

;; fn000026C0: 000026C0
;;   Called from:
;;     00003072 (in fn00002C04)
;;     000030F6 (in fn00002C04)
fn000026C0 proc
	movem.l	$0004(a7),d0-d1
	tst.l	d0
	bpl	$000026E0

l000026CA:
	neg.l	d0
	tst.l	d1
	bpl	$000026D8

l000026D0:
	neg.l	d1
	bsr	$000026F2
	neg.l	d1
	rts

l000026D8:
	bsr	$000026F2
	neg.l	d0
	neg.l	d1
	rts

l000026E0:
	tst.l	d1
	bpl	$000026F2

l000026E4:
	neg.l	d1
	bsr	$000026F2
	neg.l	d0
	rts
000026EC                                     4C EF 00 03             L...
000026F0 00 04                                           ..              

;; fn000026F2: 000026F2
;;   Called from:
;;     00002552 (in fn00002534)
;;     000026D2 (in fn000026C0)
;;     000026D8 (in fn000026C0)
;;     000026E2 (in fn000026C0)
;;     000026E6 (in fn000026C0)
fn000026F2 proc
	move.l	d2,-(a7)
	swap.l	d1
	move.w	d1,d2
	bne	$00002718

l000026FA:
	swap.l	d0
	swap.l	d1
	swap.l	d2
	move.w	d0,d2
	beq	$00002708

l00002704:
	divu.w	d1,d2
	move.w	d2,d0

l00002708:
	swap.l	d0
	move.w	d0,d2
	divu.w	d1,d2
	move.w	d2,d0
	swap.l	d2
	move.w	d2,d1
	move.l	(a7)+,d2
	rts

l00002718:
	move.l	d3,-(a7)
	moveq	#$10,d3
	cmp.w	#$0080,d1
	bcc	$00002726

l00002722:
	rol.l	#$08,d1
	subq.w	#$08,d3

l00002726:
	cmp.w	#$0800,d1
	bcc	$00002730

l0000272C:
	rol.l	#$04,d1
	subq.w	#$04,d3

l00002730:
	cmp.w	#$2000,d1
	bcc	$0000273A

l00002736:
	rol.l	#$02,d1
	subq.w	#$02,d3

l0000273A:
	tst.w	d1
	bmi	$00002742

l0000273E:
	rol.l	#$01,d1
	subq.w	#$01,d3

l00002742:
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
	bcc	$00002764

l0000275E:
	subq.w	#$01,d3
	add.l	d1,d0

l00002762:
	bcc	$00002762

l00002764:
	moveq	#$00,d1
	move.w	d3,d1
	swap.l	d3
	rol.l	d3,d0
	swap.l	d0
	exg	d0,d1
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	rts
00002776                   00 00                               ..        

;; fn00002778: 00002778
;;   Called from:
;;     00001A38 (in fn00001490)
fn00002778 proc
	movem.l	d2-d7,-(a7)
	move.l	$0020(a7),d1
	move.l	$001C(a7),d0
	movea.l	d1,a0
	move.l	$0028(a7),d3
	move.l	$0024(a7),d2
	bne	$000027CA

l00002790:
	cmp.l	d3,d0
	bcc	$000027A2

l00002794:
	move.l	d3,d2
	jsr.l	$00002534
	moveq	#$00,d0
	bra	$00002884

l000027A2:
	tst.l	d3
	bne	$000027AE

l000027A6:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l000027AE:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	$00002534
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	$00002534
	moveq	#$00,d0
	bra	$00002884

l000027CA:
	cmp.l	d2,d0
	bcs	$00002884

l000027D0:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000027EE

l000027DA:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000027EE

l000027E2:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000027EE

l000027EA:
	moveq	#$00,d4
	move.b	d2,d6

l000027EE:
	lea	$00003EAC,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$00002810

l00002802:
	cmp.l	d0,d2
	bcs	$0000280A

l00002806:
	cmp.l	d1,d3
	bhi	$00002884

l0000280A:
	sub.l	d3,d1
	subx.l	d2,d0
	bra	$00002884

l00002810:
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
	jsr.l	$00002534
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
	bcs	$0000286C

l00002866:
	bne	$00002870

l00002868:
	cmpa.l	d3,a0
	bcc	$00002870

l0000286C:
	sub.l	a1,d3
	subx.l	d0,d2

l00002870:
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

l00002884:
	movem.l	(a7)+,d2-d7
	rts
0000288A                               00 00 00 20 20 20           ...   
00002890 20 20 20 20 20 20 28 28 28 28 28 20 20 20 20 20       (((((     
000028A0 20 20 20 20 20 20 20 20 20 20 20 20 20 88 10 10              ...
000028B0 10 10 10 10 10 10 10 10 10 10 10 10 10 04 04 04 ................
000028C0 04 04 04 04 04 04 04 10 10 10 10 10 10 10 41 41 ..............AA
000028D0 41 41 41 41 01 01 01 01 01 01 01 01 01 01 01 01 AAAA............
000028E0 01 01 01 01 01 01 01 01 10 10 10 10 10 10 42 42 ..............BB
000028F0 42 42 42 42 02 02 02 02 02 02 02 02 02 02 02 02 BBBB............
00002900 02 02 02 02 02 02 02 02 10 10 10 10 20 00 00 00 ............ ...
00002910 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00002990 48 E7 00 32 48 78 00 20 4E B9 00 00 20 2C 23 C0 H..2Hx. N... ,#.
000029A0 00 00 3F CC 48 78 00 20 4E B9 00 00 20 2C 23 C0 ..?.Hx. N... ,#.
000029B0 00 00 3F D0 48 78 00 20 4E B9 00 00 20 2C 23 C0 ..?.Hx. N... ,#.
000029C0 00 00 3F D4 4F EF 00 0C 4A B9 00 00 3F CC 67 10 ..?.O...J...?.g.
000029D0 4A B9 00 00 3F D0 67 08 4A B9 00 00 3F D4 66 0C J...?.g.J...?.f.
000029E0 48 78 00 14 4E B9 00 00 13 1C 58 4F 20 79 00 00 Hx..N.....XO y..
000029F0 3F CC 20 B9 00 00 3E 84 20 79 00 00 3F CC 70 20 ?. ...>. y..?.p 
00002A00 21 40 00 18 22 39 00 00 3E 84 2C 79 00 00 3E 78 !@.."9..>.,y..>x
00002A10 4E AE FF 28 4A 80 67 10 20 79 00 00 3F CC 41 E8 N..(J.g. y..?.A.
00002A20 00 18 00 90 00 00 02 04 20 79 00 00 3F D0 20 B9 ........ y..?. .
00002A30 00 00 3E 88 20 79 00 00 3F D0 70 40 21 40 00 18 ..>. y..?.p@!@..
00002A40 22 39 00 00 3E 88 2C 79 00 00 3E 78 4E AE FF 28 "9..>.,y..>xN..(
00002A50 4A 80 67 10 20 79 00 00 3F D0 41 E8 00 18 00 90 J.g. y..?.A.....
00002A60 00 00 02 80 20 79 00 00 3F D4 20 B9 00 00 3E 8C .... y..?. ...>.
00002A70 20 79 00 00 3F D4 70 40 21 40 00 18 22 39 00 00  y..?.p@!@.."9..
00002A80 3E 8C 2C 79 00 00 3E 78 4E AE FF 28 4A 80 67 10 >.,y..>xN..(J.g.
00002A90 20 79 00 00 3F D4 41 E8 00 18 00 90 00 00 02 80  y..?.A.........
00002AA0 20 79 00 00 3F D4 42 A8 00 04 20 79 00 00 3F D0  y..?.B... y..?.
00002AB0 42 A8 00 04 20 79 00 00 3F CC 42 A8 00 04 20 79 B... y..?.B... y
00002AC0 00 00 3F D4 42 A8 00 08 20 79 00 00 3F D0 42 A8 ..?.B... y..?.B.
00002AD0 00 08 20 79 00 00 3F CC 42 A8 00 08 24 79 00 00 .. y..?.B...$y..
00002AE0 3F D4 42 AA 00 14 22 79 00 00 3F D0 42 A9 00 14 ?.B..."y..?.B...
00002AF0 20 79 00 00 3F CC 42 A8 00 14 42 AA 00 1C 42 A9  y..?.B...B...B.
00002B00 00 1C 42 A8 00 1C 42 A8 00 10 20 79 00 00 3F CC ..B...B... y..?.
00002B10 21 79 00 00 3F D0 00 0C 20 79 00 00 3F D0 21 79 !y..?... y..?.!y
00002B20 00 00 3F CC 00 10 20 79 00 00 3F D0 21 79 00 00 ..?... y..?.!y..
00002B30 3F D4 00 0C 20 79 00 00 3F D4 21 79 00 00 3F D0 ?... y..?.!y..?.
00002B40 00 10 20 79 00 00 3F D4 42 A8 00 0C 23 F9 00 00 .. y..?.B...#...
00002B50 3F CC 00 00 3F D8 23 F9 00 00 3F D4 00 00 3F DC ?...?.#...?...?.
00002B60 4C DF 4C 00 4E 75 00 00 42 A7 4E B9 00 00 2B 74 L.L.Nu..B.N...+t
00002B70 58 4F 4E 75                                     XONu            

;; fn00002B74: 00002B74
fn00002B74 proc
	movem.l	a2,-(a7)
	movea.l	$0008(a7),a2
	move.l	a2,d0
	beq	$00002B8C

l00002B80:
	move.l	a2,-(a7)
	jsr.l	$00001F80
	addq.w	#$04,a7
	bra	$00002BB2

l00002B8C:
	movea.l	$00003FD8,a2
	move.l	a2,d0
	beq	$00002BB2

l00002B96:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002BA8

l00002B9E:
	move.l	a2,-(a7)
	jsr.l	$00001F80
	addq.w	#$04,a7

l00002BA8:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00002B96

l00002BB2:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	rts

;; fn00002BB8: 00002BB8
;;   Called from:
;;     000013F2 (in fn000013D8)
fn00002BB8 proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00003FCC,-(a7)
	jsr.l	$00002C04
	lea	$000C(a7),a7
	rts

;; fn00002BD4: 00002BD4
;;   Called from:
;;     00002F0E (in fn00002C04)
;;     0000312A (in fn00002C04)
;;     00003210 (in fn00002C04)
;;     00003292 (in fn00002C04)
;;     000034D2 (in fn00002C04)
;;     000034F0 (in fn00002C04)
;;     0000364A (in fn00002C04)
;;     00003664 (in fn00002C04)
;;     00003930 (in fn00002C04)
;;     0000394A (in fn00002C04)
;;     00003B84 (in fn00002C04)
;;     00003BF0 (in fn00002C04)
fn00002BD4 proc
	movem.l	a2,-(a7)
	movea.l	$000C(a7),a2
	move.l	a2,d0
	beq	$00002BFE

l00002BE0:
	move.l	$0004(a2),d0
	cmp.l	$0008(a2),d0
	bcc	$00002BF2

l00002BEA:
	movea.l	$0004(a2),a0
	move.b	$000B(a7),(a0)

l00002BF2:
	lea	$0014(a2),a0
	addq.l	#$01,(a0)
	lea	$0004(a2),a0
	subq.l	#$01,(a0)

l00002BFE:
	movea.l	(a7)+,a2
	rts
00002C02       00 00                                       ..            

;; fn00002C04: 00002C04
;;   Called from:
;;     00002BC8 (in fn00002BB8)
fn00002C04 proc
	lea	-$004C(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$0084(a7),d2
	movea.l	$0080(a7),a4
	movea.l	$007C(a7),a2
	clr.l	$003C(a7)
	moveq	#$00,d4
	moveq	#$00,d5
	tst.b	(a4)
	beq	$00003C06

l00002C26:
	moveq	#$00,d3
	cmpi.b	#$25,(a4)
	bne	$00003B04

l00002C30:
	moveq	#-$01,d6
	move.b	#$69,$0048(a7)
	clr.b	$0049(a7)
	lea	$0001(a4),a3
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00002CB4

l00002C58:
	moveq	#$00,d6
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00002CB4

l00002C72:
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
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00002C72

l00002CB4:
	cmpi.b	#$68,(a3)
	beq	$00002CDE

l00002CBA:
	cmpi.b	#$6C,(a3)
	beq	$00002CDE

l00002CC0:
	cmpi.b	#$4C,(a3)
	beq	$00002CDE

l00002CC6:
	cmpi.b	#$7A,(a3)
	beq	$00002CDE

l00002CCC:
	cmpi.b	#$6A,(a3)
	beq	$00002CDE

l00002CD2:
	cmpi.b	#$74,(a3)
	beq	$00002CDE

l00002CD8:
	cmpi.b	#$2A,(a3)
	bne	$00002D46

l00002CDE:
	move.b	$0049(a7),d7
	move.b	$0048(a7),d1

l00002CE6:
	cmpi.b	#$2A,(a3)
	bne	$00002CF0

l00002CEC:
	moveq	#$01,d7
	bra	$00002D12

l00002CF0:
	cmp.b	#$68,d1
	bne	$00002D00

l00002CF6:
	cmpi.b	#$68,(a3)
	bne	$00002D00

l00002CFC:
	moveq	#$02,d1
	bra	$00002D12

l00002D00:
	cmp.b	#$6C,d1
	bne	$00002D10

l00002D06:
	cmpi.b	#$6C,(a3)
	bne	$00002D10

l00002D0C:
	moveq	#$01,d1
	bra	$00002D12

l00002D10:
	move.b	(a3),d1

l00002D12:
	addq.l	#$01,a3
	cmpi.b	#$68,(a3)
	beq	$00002CE6

l00002D1A:
	cmpi.b	#$6C,(a3)
	beq	$00002CE6

l00002D20:
	cmpi.b	#$4C,(a3)
	beq	$00002CE6

l00002D26:
	cmpi.b	#$7A,(a3)
	beq	$00002CE6

l00002D2C:
	cmpi.b	#$6A,(a3)
	beq	$00002CE6

l00002D32:
	cmpi.b	#$74,(a3)
	beq	$00002CE6

l00002D38:
	cmpi.b	#$2A,(a3)
	beq	$00002CE6

l00002D3E:
	move.b	d1,$0048(a7)
	move.b	d7,$0049(a7)

l00002D46:
	cmpi.b	#$6A,$0048(a7)
	bne	$00002D54

l00002D4E:
	move.b	#$01,$0048(a7)

l00002D54:
	cmpi.b	#$74,$0048(a7)
	bne	$00002D62

l00002D5C:
	move.b	#$69,$0048(a7)

l00002D62:
	cmpi.b	#$7A,$0048(a7)
	bne	$00002D70

l00002D6A:
	move.b	#$6C,$0048(a7)

l00002D70:
	move.b	(a3)+,d7
	beq	$00002DE6

l00002D74:
	cmp.b	#$25,d7
	beq	$00002DE6

l00002D7A:
	cmp.b	#$63,d7
	beq	$00002DE6

l00002D80:
	cmp.b	#$6E,d7
	beq	$00002DE6

l00002D86:
	cmp.b	#$5B,d7
	beq	$00002DE6

l00002D8C:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00002DB6

l00002D9E:
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
	bra	$00002DC2

l00002DB6:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,d1
	addq.w	#$04,a7

l00002DC2:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00002D8C

l00002DE4:
	moveq	#$01,d3

l00002DE6:
	move.b	d7,d1
	sub.b	#$25,d1
	beq	$00003232

l00002DF0:
	sub.b	#$36,d1
	beq	$00002F2E

l00002DF8:
	subq.b	#$08,d1
	beq	$00002E0E

l00002DFC:
	sub.b	#$0B,d1
	beq	$000032A0

l00002E04:
	subq.b	#$05,d1
	beq	$0000314C

l00002E0A:
	bra	$00003342

l00002E0E:
	cmp.l	#$FFFFFFFF,d6
	bne	$00002E18

l00002E16:
	moveq	#$01,d6

l00002E18:
	tst.b	$0049(a7)
	bne	$00002E32

l00002E1E:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a1
	bra	$00002E34

l00002E32:
	suba.l	a1,a1

l00002E34:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	move.l	a1,$002C(a7)
	tst.l	(a0)
	blt	$00002E6C

l00002E4C:
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
	bra	$00002E8A

l00002E6C:
	movea.l	$002C(a7),a1
	move.l	a2,-(a7)
	move.l	a1,$0030(a7)
	jsr.l	$00003CA8
	move.l	d0,$0038(a7)
	movea.l	$0030(a7),a1
	move.l	a1,$0030(a7)
	addq.w	#$04,a7

l00002E8A:
	movea.l	$002C(a7),a1
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$0034(a7)
	beq	$00002F02

l00002EA0:
	move.l	a1,$002C(a7)
	cmp.l	d3,d6
	bcs	$00002F02

l00002EA8:
	move.b	$0049(a7),d7
	movea.l	$002C(a7),a4

l00002EB0:
	tst.b	d7
	bne	$00002EB6

l00002EB4:
	move.b	d5,(a4)+

l00002EB6:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00002EE0

l00002EC8:
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
	bra	$00002EEC

l00002EE0:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,d1
	addq.w	#$04,a7

l00002EEC:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00002EFE

l00002EFA:
	cmp.l	d3,d6
	bcc	$00002EB0

l00002EFE:
	move.b	d7,$0049(a7)

l00002F02:
	cmp.l	#$FFFFFFFF,d5
	beq	$00002F14

l00002F0A:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l00002F14:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003AFE

l00002F20:
	tst.l	d3
	beq	$00003AFE

l00002F26:
	addq.l	#$01,$003C(a7)
	bra	$00003AFE

l00002F2E:
	clr.b	$002C(a7)
	cmpi.b	#$5E,(a3)
	bne	$00002F40

l00002F38:
	move.b	#$01,$002C(a7)
	addq.l	#$01,a3

l00002F40:
	clr.l	$0034(a7)
	move.b	$002C(a7),d7
	move.l	$0034(a7),d1

l00002F4C:
	tst.b	d7
	beq	$00002F58

l00002F50:
	move.l	#$000000FF,d5
	bra	$00002F5A

l00002F58:
	moveq	#$00,d5

l00002F5A:
	lea	$004E(a7),a0
	move.b	d5,(a0,d1)
	addq.l	#$01,d1
	cmp.l	#$00000020,d1
	bcs	$00002F4C

l00002F6C:
	move.l	d2,$0084(a7)
	move.b	d7,$002C(a7)
	move.b	$002C(a7),d2

l00002F78:
	tst.b	(a3)
	beq	$00002FEE

l00002F7C:
	move.b	(a3)+,d1
	cmpi.b	#$2D,(a3)
	bne	$00002F90

l00002F84:
	cmp.b	$0001(a3),d1
	bcc	$00002F90

l00002F8A:
	addq.l	#$01,a3
	move.b	(a3)+,d7
	bra	$00002F92

l00002F90:
	move.b	d1,d7

l00002F92:
	moveq	#$00,d5
	move.b	d1,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcs	$00002FE8

l00002F9E:
	tst.b	d2
	beq	$00002FC2

l00002FA2:
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
	bra	$00002FDE

l00002FC2:
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

l00002FDE:
	addq.l	#$01,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcc	$00002F9E

l00002FE8:
	cmpi.b	#$5D,(a3)
	bne	$00002F78

l00002FEE:
	move.l	$0084(a7),d2
	addq.l	#$01,a3
	tst.b	$0049(a7)
	bne	$0000300E

l00002FFA:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a6
	bra	$00003010

l0000300E:
	suba.l	a6,a6

l00003010:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003042

l00003022:
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
	bra	$00003050

l00003042:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00003050:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$002C(a7)
	beq	$0000311E

l00003064:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	$000026C0
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
	beq	$0000311E

l00003094:
	cmp.l	d3,d6
	bcs	$0000311E

l0000309A:
	move.b	$0049(a7),d7

l0000309E:
	tst.b	d7
	bne	$000030A4

l000030A2:
	move.b	d5,(a6)+

l000030A4:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000030CE

l000030B6:
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
	bra	$000030DA

l000030CE:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,d1
	addq.w	#$04,a7

l000030DA:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$0000311A

l000030E8:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	$000026C0
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
	beq	$0000311A

l00003116:
	cmp.l	d3,d6
	bcc	$0000309E

l0000311A:
	move.b	d7,$0049(a7)

l0000311E:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003130

l00003126:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l00003130:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003AFE

l0000313C:
	tst.l	d3
	beq	$00003AFE

l00003142:
	clr.b	(a6)+
	addq.l	#$01,$003C(a7)
	bra	$00003AFE

l0000314C:
	tst.b	$0049(a7)
	bne	$00003166

l00003152:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a5
	bra	$00003168

l00003166:
	suba.l	a5,a5

l00003168:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003204

l00003172:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003204

l0000318E:
	cmp.l	d3,d6
	bcs	$00003204

l00003192:
	move.b	$0049(a7),d7

l00003196:
	tst.b	d7
	bne	$0000319C

l0000319A:
	move.b	d5,(a5)+

l0000319C:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000031C6

l000031AE:
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
	bra	$000031D2

l000031C6:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,d1
	addq.w	#$04,a7

l000031D2:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00003200

l000031E0:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003200

l000031FC:
	cmp.l	d3,d6
	bcc	$00003196

l00003200:
	move.b	d7,$0049(a7)

l00003204:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003216

l0000320C:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l00003216:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003AFE

l00003222:
	tst.l	d3
	beq	$00003AFE

l00003228:
	clr.b	(a5)+
	addq.l	#$01,$003C(a7)
	bra	$00003AFE

l00003232:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003264

l00003244:
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
	bra	$00003272

l00003264:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00003272:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$00000025,$002C(a7)
	beq	$00003AFE

l00003286:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003298

l0000328E:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l00003298:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00003AFE

l000032A0:
	tst.b	$0049(a7)
	bne	$00003338

l000032A8:
	cmpi.b	#$01,$0048(a7)
	bne	$000032CA

l000032B0:
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
	bra	$00003338

l000032CA:
	cmpi.b	#$6C,$0048(a7)
	bne	$000032E8

l000032D2:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)
	bra	$00003338

l000032E8:
	cmpi.b	#$68,$0048(a7)
	bne	$00003306

l000032F0:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.w	d4,(a0)
	bra	$00003338

l00003306:
	cmpi.b	#$02,$0048(a7)
	bne	$00003324

l0000330E:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.b	d4,(a0)
	bra	$00003338

l00003324:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)

l00003338:
	moveq	#$01,d3
	addq.l	#$01,$003C(a7)
	bra	$00003AFE

l00003342:
	clr.l	$0030(a7)
	clr.l	$002C(a7)
	clr.l	$006E(a7)
	tst.b	d7
	bne	$00003354

l00003352:
	subq.l	#$01,a3

l00003354:
	cmp.b	#$70,d7
	bne	$00003362

l0000335A:
	move.b	#$6C,$0048(a7)
	moveq	#$78,d7

l00003362:
	cmp.l	#$0000002D,d5
	bne	$00003370

l0000336A:
	cmp.b	#$75,d7
	bne	$00003378

l00003370:
	cmp.l	#$0000002B,d5
	bne	$000033C8

l00003378:
	cmp.l	d3,d6
	bcs	$000033C8

l0000337C:
	move.l	d5,$006E(a7)
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000033B2

l00003392:
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
	bra	$000033C0

l000033B2:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l000033C0:
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4

l000033C8:
	cmp.b	#$69,d7
	bne	$0000353A

l000033D0:
	cmp.l	#$00000030,d5
	bne	$000034FC

l000033DA:
	cmp.l	d3,d6
	bcs	$000034FC

l000033E0:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003412

l000033F2:
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
	bra	$00003420

l00003412:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003420:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$0000288C,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$00003446

l00003442:
	or.b	#$20,d0

l00003446:
	cmp.l	#$00000078,d0
	bne	$000034DE

l00003450:
	cmp.l	d3,d6
	bcs	$000034DE

l00003456:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003488

l00003468:
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
	bra	$00003496

l00003488:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003496:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
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
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$000034D8

l000034CC:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l000034D8:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$000034E0

l000034DE:
	moveq	#$6F,d7

l000034E0:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$000034F6

l000034EA:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l000034F6:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$0000353A

l000034FC:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$0000353A

l00003518:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$0000353A

l00003534:
	cmp.l	d3,d6
	bcs	$0000353A

l00003538:
	moveq	#$78,d7

l0000353A:
	cmp.b	#$78,d7
	bne	$0000366E

l00003542:
	cmp.l	#$00000030,d5
	bne	$0000366E

l0000354C:
	cmp.l	d3,d6
	bcs	$0000366E

l00003552:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003584

l00003564:
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
	bra	$00003592

l00003584:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003592:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$0000288C,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$000035B8

l000035B4:
	or.b	#$20,d0

l000035B8:
	cmp.l	#$00000078,d0
	bne	$00003654

l000035C2:
	cmp.l	d3,d6
	bcs	$00003654

l000035C8:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000035FA

l000035DA:
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
	bra	$00003608

l000035FA:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003608:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$0000363A

l00003630:
	cmp.l	d3,d6
	bcs	$0000363A

l00003634:
	move.l	$004A(a7),d5
	bra	$0000366E

l0000363A:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$00003650

l00003644:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l00003650:
	subq.l	#$01,d3
	subq.l	#$01,d4

l00003654:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$0000366A

l0000365E:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l0000366A:
	subq.l	#$01,d3
	subq.l	#$01,d4

l0000366E:
	cmp.b	#$78,d7
	beq	$0000367A

l00003674:
	cmp.b	#$58,d7
	bne	$00003684

l0000367A:
	move.l	#$00000010,$0040(a7)
	bra	$000036A2

l00003684:
	cmp.b	#$6F,d7
	bne	$00003694

l0000368A:
	move.l	#$00000008,$0034(a7)
	bra	$0000369C

l00003694:
	move.l	#$0000000A,$0034(a7)

l0000369C:
	move.l	$0034(a7),$0040(a7)

l000036A2:
	move.l	$0040(a7),$0072(a7)
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$0000390A

l000036D4:
	cmpi.l	#$0000000A,$0072(a7)
	bne	$0000370A

l000036DE:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$0000390A

l0000370A:
	cmpi.l	#$00000008,$0072(a7)
	bne	$0000372A

l00003714:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	cmp.l	#$00000037,d5
	bgt	$0000390A

l0000372A:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.l	d6,$0040(a7)
	move.b	d7,$004A(a7)
	cmp.l	d3,d6
	bcs	$0000390A

l00003740:
	move.l	$0072(a7),d7
	movea.l	$0040(a7),a4

l00003748:
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
	jsr.l	$00003C28
	lea	$0010(a7),a7
	movea.l	(a7)+,a1
	move.l	d0,$0048(a7)
	move.l	d1,$004C(a7)
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$000037A8

l0000379E:
	move.l	d5,d4
	sub.l	#$00000030,d4
	bra	$000037AA

l000037A8:
	moveq	#$00,d4

l000037AA:
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
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000001,d0
	beq	$000037FA

l000037F0:
	move.l	d5,d6
	sub.l	#$00000037,d6
	bra	$000037FC

l000037FA:
	moveq	#$00,d6

l000037FC:
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
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000002,d0
	beq	$0000384C

l00003842:
	move.l	d5,d2
	sub.l	#$00000057,d2
	bra	$0000384E

l0000384C:
	moveq	#$00,d2

l0000384E:
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
	blt	$000038A0

l00003888:
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
	bra	$000038AC

l000038A0:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,d1
	addq.w	#$04,a7

l000038AC:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,$0034(a7)
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$0000390A

l000038D0:
	cmp.l	#$0000000A,d7
	bne	$000038F4

l000038D8:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$0000390A

l000038F4:
	cmp.l	#$00000008,d7
	bne	$00003904

l000038FC:
	cmp.l	#$00000037,d5
	bgt	$0000390A

l00003904:
	cmpa.l	d3,a4
	bcc	$00003748

l0000390A:
	move.b	$004A(a7),d7
	move.l	$0034(a7),d4
	move.l	$0084(a7),d2
	tst.l	$006E(a7)
	beq	$0000393E

l0000391C:
	cmp.l	#$00000002,d3
	bne	$0000393E

l00003924:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003936

l0000392C:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l00003936:
	subq.l	#$01,d3
	subq.l	#$01,d4
	move.l	$006E(a7),d5

l0000393E:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003950

l00003946:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l00003950:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003AFE

l0000395C:
	tst.l	d3
	beq	$00003AFE

l00003962:
	cmp.b	#$75,d7
	bne	$00003A24

l0000396A:
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0038(a7)
	move.l	(a7)+,d0
	tst.b	$0034(a7)
	beq	$00003994

l0000397E:
	subq.b	#$01,$0034(a7)
	beq	$000039EC

l00003984:
	subi.b	#$66,$0034(a7)
	beq	$000039D0

l0000398C:
	subq.b	#$04,$0034(a7)
	beq	$000039B4

l00003992:
	bra	$00003A08

l00003994:
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
	bra	$00003AFA

l000039B4:
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
	bra	$00003AFA

l000039D0:
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
	bra	$00003AFA

l000039EC:
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
	bra	$00003AFA

l00003A08:
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
	bra	$00003AFA

l00003A24:
	cmpi.l	#$0000002D,$006E(a7)
	bne	$00003A40

l00003A2E:
	movem.l	$002C(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0034(a7)
	bra	$00003A4C

l00003A40:
	move.l	$0030(a7),$0038(a7)
	move.l	$002C(a7),$0034(a7)
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0030(a7)
	move.l	(a7)+,d0
	tst.b	$002C(a7)
	beq	$00003A76

l00003A60:
	subq.b	#$01,$002C(a7)
	beq	$00003AC8

l00003A66:
	subi.b	#$66,$002C(a7)
	beq	$00003AAE

l00003A6E:
	subq.b	#$04,$002C(a7)
	beq	$00003A94

l00003A74:
	bra	$00003AE2

l00003A76:
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
	bra	$00003AFA

l00003A94:
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
	bra	$00003AFA

l00003AAE:
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
	bra	$00003AFA

l00003AC8:
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
	bra	$00003AFA

l00003AE2:
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

l00003AFA:
	addq.l	#$01,$003C(a7)

l00003AFE:
	movea.l	a3,a4
	bra	$00003BFC

l00003B04:
	move.b	(a4),d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	beq	$00003B90

l00003B20:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003B4A

l00003B32:
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
	bra	$00003B56

l00003B4A:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,d1
	addq.w	#$04,a7

l00003B56:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$0000288D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003B20

l00003B78:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003B8A

l00003B80:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l00003B8A:
	subq.l	#$01,d4
	moveq	#$01,d3
	bra	$00003BFA

l00003B90:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003BC2

l00003BA2:
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
	bra	$00003BD0

l00003BC2:
	move.l	a2,-(a7)
	jsr.l	$00003CA8
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00003BD0:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	(a4),d0
	ext.w	d0
	ext.l	d0
	cmp.l	$002C(a7),d0
	beq	$00003BFA

l00003BE4:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003BF6

l00003BEC:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00002BD4
	addq.w	#$08,a7

l00003BF6:
	subq.l	#$01,d3
	subq.l	#$01,d4

l00003BFA:
	addq.l	#$01,a4

l00003BFC:
	tst.l	d3
	beq	$00003C06

l00003C00:
	tst.b	(a4)
	bne	$00002C26

l00003C06:
	cmp.l	#$FFFFFFFF,d5
	bne	$00003C18

l00003C0E:
	tst.l	$003C(a7)
	bne	$00003C18

l00003C14:
	move.l	d5,d0
	bra	$00003C1C

l00003C18:
	move.l	$003C(a7),d0

l00003C1C:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$004C(a7),a7
	rts
00003C26                   00 00                               ..        

;; fn00003C28: 00003C28
;;   Called from:
;;     00003766 (in fn00002C04)
fn00003C28 proc
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
	bcc	$00003C5C

l00003C56:
	add.l	#$00010000,d1

l00003C5C:
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
00003CA6                   00 00                               ..        

;; fn00003CA8: 00003CA8
;;   Called from:
;;     00002DB8 (in fn00002C04)
;;     00002E76 (in fn00002C04)
;;     00002EE2 (in fn00002C04)
;;     00003044 (in fn00002C04)
;;     000030D0 (in fn00002C04)
;;     000031C8 (in fn00002C04)
;;     00003266 (in fn00002C04)
;;     000033B4 (in fn00002C04)
;;     00003414 (in fn00002C04)
;;     0000348A (in fn00002C04)
;;     00003586 (in fn00002C04)
;;     000035FC (in fn00002C04)
;;     000038A2 (in fn00002C04)
;;     00003B4C (in fn00002C04)
;;     00003BC4 (in fn00002C04)
fn00003CA8 proc
	movem.l	d2-d5/a2-a4/a6,-(a7)
	movea.l	$0024(a7),a2
	jsr.l	$00002400
	move.l	a2,d0
	bne	$00003CC0

l00003CBA:
	moveq	#-$01,d0
	bra	$00003D9E

l00003CC0:
	moveq	#$2A,d0
	and.l	$0018(a2),d0
	moveq	#$20,d5
	cmp.l	d0,d5
	beq	$00003CD2

l00003CCC:
	moveq	#-$01,d0
	bra	$00003D9E

l00003CD2:
	lea	$0018(a2),a0
	moveq	#$01,d0
	or.l	d0,(a0)
	move.l	#$00000200,d0
	and.l	(a0),d0
	beq	$00003CEA

l00003CE4:
	jsr.l	$00003DA4

l00003CEA:
	tst.l	$001C(a2)
	bne	$00003D08

l00003CF0:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00003D00

l00003CF8:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00003D08

l00003D00:
	move.l	#$00000400,$001C(a2)

l00003D08:
	tst.l	$0008(a2)
	bne	$00003D44

l00003D0E:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00003D22

l00003D1E:
	moveq	#$02,d4
	bra	$00003D24

l00003D22:
	moveq	#$01,d4

l00003D24:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	$0000202C
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00003D3C

l00003D38:
	moveq	#-$01,d0
	bra	$00003D9E

l00003D3C:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)

l00003D44:
	lea	$0004(a2),a0
	move.l	$0008(a2),(a0)
	move.l	$001C(a2),d3
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003E78,a6
	jsr.l	-$002A(a6)
	lea	$0014(a2),a0
	move.l	d0,(a0)
	subq.l	#$01,(a0)
	bge	$00003D8A

l00003D68:
	moveq	#-$01,d0
	cmp.l	$0014(a2),d0
	bne	$00003D7A

l00003D70:
	lea	$0018(a2),a0
	moveq	#$08,d0
	or.l	d0,(a0)
	bra	$00003D82

l00003D7A:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)

l00003D82:
	clr.l	$0014(a2)
	moveq	#-$01,d0
	bra	$00003D9E

l00003D8A:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	(a0),d0
	and.l	#$000000FF,d0

l00003D9E:
	movem.l	(a7)+,d2-d5/a2-a4/a6
	rts

;; fn00003DA4: 00003DA4
;;   Called from:
;;     00003CE4 (in fn00003CA8)
fn00003DA4 proc
	movem.l	a2,-(a7)
	movea.l	$00003FD8,a2
	move.l	a2,d0
	beq	$00003DDC

l00003DB2:
	move.l	#$00000202,d0
	and.l	$0018(a2),d0
	cmp.l	#$00000202,d0
	bne	$00003DD2

l00003DC4:
	tst.l	(a2)
	beq	$00003DD2

l00003DC8:
	move.l	a2,-(a7)
	jsr.l	$00001F80
	addq.w	#$04,a7

l00003DD2:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00003DB2

l00003DDC:
	movea.l	(a7)+,a2
	rts

;; fn00003DE0: 00003DE0
;;   Called from:
;;     000013E2 (in fn000013D8)
;;     0000141C (in fn000013D8)
fn00003DE0 proc
	movem.l	d2/a2-a3,-(a7)
	movea.l	$0010(a7),a2
	moveq	#$00,d2
	tst.b	(a2)
	beq	$00003E60

l00003DEE:
	movea.l	$00003FD0,a0
	lea	$0018(a0),a1
	moveq	#$02,d0
	or.l	d0,(a1)
	lea	$0014(a0),a0
	subq.l	#$01,(a0)
	blt	$00003E36

l00003E04:
	cmpi.b	#$0A,(a2)
	bne	$00003E1C

l00003E0A:
	movea.l	$00003FD0,a0
	move.l	#$00000080,d0
	and.l	$0018(a0),d0
	bne	$00003E36

l00003E1C:
	movea.l	$00003FD0,a1
	addq.l	#$04,a1
	movea.l	(a1),a0
	movea.l	a0,a3
	addq.l	#$01,a3
	move.l	a3,(a1)
	move.b	(a2),(a0)
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00003E4E

l00003E36:
	move.l	$00003FD0,-(a7)
	move.b	(a2),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001E6C
	move.l	d0,d1
	addq.w	#$08,a7

l00003E4E:
	moveq	#-$01,d0
	cmp.l	d1,d0
	bne	$00003E58

l00003E54:
	moveq	#-$01,d0
	bra	$00003E62

l00003E58:
	addq.l	#$01,a2
	addq.l	#$01,d2
	tst.b	(a2)
	bne	$00003DEE

l00003E60:
	move.l	d2,d0

l00003E62:
	movem.l	(a7)+,d2/a2-a3
	rts
00003E68                         00 24 00 63 41 00 00 00         .$.cA...
00003E70 00 00 00 00 00 00 40 00 00 00 00 00 00 01 02 02 ......@.........
00003E80 03 03 03 03 04 04 04 04 04 04 04 04 05 05 05 05 ................
00003E90 05 05 05 05 05 05 05 05 05 05 05 05 06 06 06 06 ................
00003EA0 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 ................
00003EB0 06 06 06 06 06 06 06 06 06 06 06 06 07 07 07 07 ................
00003EC0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003ED0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003EE0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003EF0 07 07 07 07 07 07 07 07 07 07 07 07 08 08 08 08 ................
00003F00 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F10 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F20 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F30 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F40 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F50 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F60 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F70 08 08 08 08 08 08 08 08 08 08 08 08 00 00 00 01 ................
00003F80 00 00 29 90 00 00 00 00 00 00 00 02 00 00 20 14 ..)........... .
00003F90 00 00 2B 68 00 00 00 00 00 00 00 00 00 00 00 00 ..+h............
00003FA0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
