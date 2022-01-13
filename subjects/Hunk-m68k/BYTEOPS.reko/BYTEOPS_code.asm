;;; Segment code (00001000)

;; fn00001000: 00001000
fn00001000 proc
	bra	$0000100A
00001002       56 42 43 43 20 30 2E 39                     VBCC 0.9      

l0000100A:
	move.l	d0,d2
	movea.l	a0,a2
	lea	$0000AB7E,a4
	movea.l	$00000004,a6
	cmpi.w	#$0024,$0014(a6)
	bcc	$00001036

l00001020:
	lea	$00002B88,a0
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
	lea	$0000AB7E,a4
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
	lea	$00002CD0,a3
	move.l	#$00002CD0,d0
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
;;     000023EA (in fn000023C8)
fn0000131C proc
	movem.l	a2-a3,-(a7)
	tst.l	$00002BB8
	bne	$0000134E

l00001328:
	movea.l	$00002CE0,a3
	moveq	#$01,d0
	move.l	d0,$00002BB8
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
	lea	$00002CC8,a3
	move.l	#$00002CC4,d0
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
	movem.l	d2-d5,-(a7)
	st	d2
	move.b	#$8F,d3
	add.b	d2,d3
	sub.b	d3,d2
	moveq	#$00,d0
	move.b	d2,d0
	moveq	#$00,d1
	move.b	d3,d1
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
	move.b	d0,d2
	moveq	#$00,d0
	move.b	d3,d0
	moveq	#$00,d1
	move.b	d2,d1
	move.l	d1,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	jsr.l	$00002AC8
	addq.w	#$08,a7
	movem.l	(a7)+,d1
	move.b	d0,d3
	moveq	#$00,d0
	move.b	d3,d0
	moveq	#$00,d1
	move.b	d2,d1
	move.l	d1,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	jsr.l	$00002A88
	addq.w	#$08,a7
	movem.l	(a7)+,d1
	move.b	d0,d3
	moveq	#$00,d0
	move.b	d2,d0
	lsl.l	#$05,d0
	move.b	d0,d2
	moveq	#$00,d0
	move.b	d3,d0
	moveq	#$00,d1
	move.b	d2,d1
	asr.l	d1,d0
	move.b	d0,d3
	moveq	#$00,d0
	move.b	d3,d0
	move.l	d0,-(a7)
	moveq	#$00,d0
	move.b	d2,d0
	move.l	d0,-(a7)
	pea	0000142C                                               ; $0016(pc)
	jsr.l	$0000143C
	lea	$000C(a7),a7
	movem.l	(a7)+,d2-d5
	rts
0000142A                               00 00 61 20 3D 20           ..a = 
00001430 25 64 2C 20 62 20 3D 20 25 64 0A 00             %d, b = %d..    

;; fn0000143C: 0000143C
;;   Called from:
;;     0000141A (in fn00001390)
fn0000143C proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00002CE8,-(a7)
	jsr.l	$00001458
	lea	$000C(a7),a7
	rts

;; fn00001458: 00001458
;;   Called from:
;;     0000144C (in fn0000143C)
fn00001458 proc
	lea	-$0044(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$007C(a7),d3
	movea.l	$0074(a7),a5
	movea.l	$0078(a7),a4
	moveq	#$00,d6
	tst.b	(a4)
	beq	$00001D9E

l00001474:
	cmpi.b	#$25,(a4)
	bne	$00001D78

l0000147C:
	clr.l	$0040(a7)
	moveq	#-$01,d5
	clr.l	$0048(a7)
	moveq	#$69,d4
	lea	$004C(a7),a3
	moveq	#$00,d7
	clr.l	$0066(a7)
	lea	$0001(a4),a2
	move.l	$0048(a7),d2

l0000149A:
	moveq	#$00,d1

l0000149C:
	lea	00001DB0,a0                                            ; $0914(pc)
	move.l	d0,-(a7)
	move.b	(a0,d1),d0
	cmp.b	(a2),d0
	movem.l	(a7)+,d0
	bne	$000014C0

l000014AE:
	move.l	d1,d0
	move.l	d1,-(a7)
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	move.l	(a7)+,d1
	or.l	d0,d2
	addq.l	#$01,a2
	bra	$000014CA

l000014C0:
	addq.l	#$01,d1
	cmp.l	#$00000005,d1
	bcs	$0000149C

l000014CA:
	cmp.l	#$00000005,d1
	bcs	$0000149A

l000014D2:
	move.l	d2,$0048(a7)
	cmpi.b	#$2A,(a2)
	bne	$00001510

l000014DC:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	bge	$00001508

l000014F4:
	ori.l	#$00000004,$0048(a7)
	move.l	$002C(a7),d0
	neg.l	d0
	move.l	d0,$0040(a7)
	bra	$0000157C

l00001508:
	move.l	$002C(a7),$0040(a7)
	bra	$0000157C

l00001510:
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$0000275D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$0000157C

l0000152C:
	move.l	$0040(a7),d2

l00001530:
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
	lea	$0000275D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00001530

l00001578:
	move.l	d2,$0040(a7)

l0000157C:
	cmpi.b	#$2E,(a2)
	bne	$00001610

l00001584:
	addq.l	#$01,a2
	cmpi.b	#$2A,(a2)
	bne	$000015AA

l0000158C:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	blt	$00001610

l000015A4:
	move.l	$002C(a7),d5
	bra	$00001610

l000015AA:
	moveq	#$00,d5
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$0000275D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00001610

l000015C8:
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
	lea	$0000275D,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$000015C8

l00001610:
	cmpi.b	#$68,(a2)
	beq	$00001634

l00001616:
	cmpi.b	#$6C,(a2)
	beq	$00001634

l0000161C:
	cmpi.b	#$4C,(a2)
	beq	$00001634

l00001622:
	cmpi.b	#$6A,(a2)
	beq	$00001634

l00001628:
	cmpi.b	#$7A,(a2)
	beq	$00001634

l0000162E:
	cmpi.b	#$74,(a2)
	bne	$0000163A

l00001634:
	move.b	(a2)+,d4
	ext.w	d4
	ext.l	d4

l0000163A:
	cmp.l	#$00000068,d4
	bne	$0000164C

l00001642:
	cmpi.b	#$68,(a2)
	bne	$0000164C

l00001648:
	moveq	#$02,d4
	addq.l	#$01,a2

l0000164C:
	cmp.l	#$0000006C,d4
	bne	$0000165E

l00001654:
	cmpi.b	#$6C,(a2)
	bne	$0000165E

l0000165A:
	moveq	#$01,d4
	addq.l	#$01,a2

l0000165E:
	cmp.l	#$0000006A,d4
	bne	$00001668

l00001666:
	moveq	#$01,d4

l00001668:
	cmp.l	#$0000007A,d4
	bne	$00001672

l00001670:
	moveq	#$6C,d4

l00001672:
	cmp.l	#$00000074,d4
	bne	$0000167C

l0000167A:
	moveq	#$69,d4

l0000167C:
	move.b	(a2)+,d1
	move.b	d1,d0
	cmp.b	#$25,d1
	beq	$00001B04

l00001688:
	cmp.b	#$58,d0
	beq	$000016D6

l0000168E:
	cmp.b	#$63,d0
	beq	$00001A84

l00001696:
	cmp.b	#$64,d0
	beq	$000016D6

l0000169C:
	cmp.b	#$69,d0
	beq	$000016D6

l000016A2:
	move.b	d0,$002C(a7)
	cmp.b	#$6E,d0
	beq	$00001B16

l000016AE:
	move.b	$002C(a7),d0
	sub.b	#$6F,d0
	cmp.b	#$01,d0
	bls	$000016D6

l000016BC:
	move.b	$002C(a7),d0
	cmp.b	#$73,d0
	beq	$00001AC0

l000016C8:
	cmp.b	#$75,d0
	beq	$000016D6

l000016CE:
	cmp.b	#$78,d0
	bne	$00001BAE

l000016D6:
	cmp.b	#$70,d1
	bne	$000016E8

l000016DC:
	moveq	#$6C,d4
	moveq	#$78,d1
	ori.l	#$00000001,$0048(a7)

l000016E8:
	cmp.b	#$64,d1
	beq	$000016F6

l000016EE:
	cmp.b	#$69,d1
	bne	$00001840

l000016F6:
	cmp.l	#$00000001,d4
	bne	$0000171C

l000016FE:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$0034(a7)
	move.l	-$0008(a0),$0030(a7)
	bra	$000017B4

l0000171C:
	cmp.l	#$0000006C,d4
	bne	$00001748

l00001724:
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
	bra	$000017B4

l00001748:
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
	bne	$0000178E

l00001772:
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

l0000178E:
	cmp.l	#$00000002,d4
	bne	$000017B4

l00001796:
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

l000017B4:
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
	bge	$000017FE

l000017DE:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2D,(a0)
	movem.l	$0030(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0038(a7)
	bra	$00001974

l000017FE:
	move.b	$002C(a7),d1
	moveq	#$10,d0
	and.l	$0048(a7),d0
	beq	$00001818

l0000180A:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2B,(a0)
	bra	$0000182C

l00001818:
	moveq	#$08,d0
	and.l	$0048(a7),d0
	beq	$0000182C

l00001820:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$20,(a0)

l0000182C:
	move.l	$0034(a7),$003C(a7)
	move.l	$0030(a7),$0038(a7)
	move.b	d1,$002C(a7)
	bra	$00001974

l00001840:
	cmp.l	#$00000001,d4
	bne	$00001864

l00001848:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	move.l	-$0008(a0),$0038(a7)
	bra	$0000189E

l00001864:
	cmp.l	#$0000006C,d4
	bne	$00001886

l0000186C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)
	bra	$0000189E

l00001886:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)

l0000189E:
	cmp.l	#$00000068,d4
	bne	$000018BA

l000018A6:
	move.w	$003E(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.w	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l000018BA:
	cmp.l	#$00000002,d4
	bne	$000018D6

l000018C2:
	move.b	$003F(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l000018D6:
	moveq	#$01,d0
	and.l	$0048(a7),d0
	move.b	d1,$002C(a7)
	tst.l	d0
	beq	$00001974

l000018E6:
	cmp.b	#$6F,d1
	bne	$00001922

l000018EC:
	tst.l	d5
	bne	$00001916

l000018F0:
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
	beq	$00001922

l00001916:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$30,(a0)

l00001922:
	cmp.b	#$78,d1
	beq	$00001932

l00001928:
	move.b	d1,$002C(a7)
	cmp.b	#$58,d1
	bne	$00001974

l00001932:
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
	beq	$00001974

l0000195C:
	lea	$006A(a7),a0
	lea	(a0,d7),a1
	addq.l	#$01,d7
	move.b	#$30,(a1)
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	d1,(a0)
	move.b	d1,$002C(a7)

l00001974:
	move.b	$002C(a7),d1
	lea	$0062(a7),a3
	cmp.b	#$78,d1
	beq	$00001988

l00001982:
	cmp.b	#$58,d1
	bne	$00001992

l00001988:
	move.l	#$00000010,$002C(a7)
	bra	$000019B0

l00001992:
	cmp.b	#$6F,d1
	bne	$000019A2

l00001998:
	move.l	#$00000008,$0030(a7)
	bra	$000019AA

l000019A2:
	move.l	#$0000000A,$0030(a7)

l000019AA:
	move.l	$0030(a7),$002C(a7)

l000019B0:
	move.l	$002C(a7),$006C(a7)
	cmp.b	#$58,d1
	beq	$000019C2

l000019BC:
	lea	00001DB8,a6                                            ; $03FC(pc)
	bra	$000019C6

l000019C2:
	lea	00001DC8,a6                                            ; $0406(pc)

l000019C6:
	move.l	a6,$002C(a7)
	move.l	d3,$007C(a7)
	move.l	d5,$0044(a7)
	move.l	d6,$0030(a7)
	move.l	d7,$0062(a7)
	movem.l	$0038(a7),d6-d7
	move.l	$0066(a7),d3
	movea.l	$002C(a7),a1

l000019E8:
	move.l	$006C(a7),d1
	move.l	d1,d0
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	a1,-(a7)
	movem.l	d0-d1,-(a7)
	movem.l	d6-d7,-(a7)
	jsr.l	$00002648
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
	jsr.l	$000023F8
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
	bne	$000019E8

l00001A56:
	move.l	d3,$0066(a7)
	move.l	$0062(a7),d7
	move.l	$0030(a7),d6
	move.l	$0044(a7),d5
	move.l	$007C(a7),d3
	cmp.l	#$FFFFFFFF,d5
	bne	$00001A78

l00001A72:
	moveq	#$00,d5
	bra	$00001BC4

l00001A78:
	andi.l	#$FFFFFFFD,$0048(a7)
	bra	$00001BC4

l00001A84:
	cmp.l	#$0000006C,d4
	bne	$00001AA0

l00001A8C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)
	bra	$00001AB2

l00001AA0:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)

l00001AB2:
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001BC4

l00001AC0:
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
	beq	$00001AE2

l00001ADC:
	cmp.l	$0066(a7),d5
	bls	$00001AFE

l00001AE2:
	tst.b	(a1)
	beq	$00001AFE

l00001AE6:
	move.l	$0066(a7),d0

l00001AEA:
	addq.l	#$01,d0
	addq.l	#$01,a1
	tst.l	d5
	bls	$00001AF6

l00001AF2:
	cmp.l	d0,d5
	bls	$00001AFA

l00001AF6:
	tst.b	(a1)
	bne	$00001AEA

l00001AFA:
	move.l	d0,$0066(a7)

l00001AFE:
	moveq	#$00,d5
	bra	$00001BC4

l00001B04:
	lea	00001DAC,a3                                            ; $02A8(pc)
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001BC4

l00001B16:
	cmp.l	#$00000001,d4
	bne	$00001B38

l00001B1E:
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
	bra	$00001BA6

l00001B38:
	cmp.l	#$0000006C,d4
	bne	$00001B56

l00001B40:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)
	bra	$00001BA6

l00001B56:
	cmp.l	#$00000068,d4
	bne	$00001B74

l00001B5E:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.w	d6,(a0)
	bra	$00001BA6

l00001B74:
	cmp.l	#$00000002,d4
	bne	$00001B92

l00001B7C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.b	d6,(a0)
	bra	$00001BA6

l00001B92:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)

l00001BA6:
	moveq	#$00,d5
	clr.l	$0040(a7)
	bra	$00001BC4

l00001BAE:
	tst.b	d1
	bne	$00001BB4

l00001BB2:
	subq.l	#$01,a2

l00001BB4:
	movea.l	a4,a3
	move.l	a2,d0
	sub.l	a4,d0
	move.l	d0,$0066(a7)
	moveq	#$00,d5
	clr.l	$0040(a7)

l00001BC4:
	cmp.l	$0066(a7),d5
	bhi	$00001BD2

l00001BCA:
	move.l	$0066(a7),$002C(a7)
	bra	$00001BD6

l00001BD2:
	move.l	d5,$002C(a7)

l00001BD6:
	move.l	d0,-(a7)
	move.l	$0030(a7),d0
	add.l	d7,d0
	move.l	d0,$0034(a7)
	move.l	(a7)+,d0
	move.l	d0,-(a7)
	move.l	$0034(a7),d0
	cmp.l	$0044(a7),d0
	movem.l	(a7)+,d0
	bcs	$00001BFA

l00001BF4:
	clr.l	$002C(a7)
	bra	$00001C0A

l00001BFA:
	move.l	d0,-(a7)
	move.l	$0044(a7),d0
	sub.l	$0034(a7),d0
	move.l	d0,$0030(a7)
	move.l	(a7)+,d0

l00001C0A:
	move.l	$002C(a7),$0030(a7)
	moveq	#$02,d0
	and.l	$0048(a7),d0
	beq	$00001C4C

l00001C18:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001C4C

l00001C1E:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001DD8
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001C44

l00001C3E:
	move.l	d6,d0
	bra	$00001DA0

l00001C44:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001C1E

l00001C4C:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	bne	$00001C9E

l00001C54:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001C9E

l00001C5C:
	move.l	$0048(a7),d4
	movea.l	$0030(a7),a4

l00001C64:
	move.l	a5,-(a7)
	moveq	#$02,d0
	and.l	d4,d0
	beq	$00001C72

l00001C6C:
	movea.w	#$0030,a0
	bra	$00001C76

l00001C72:
	movea.w	#$0020,a0

l00001C76:
	move.l	a0,-(a7)
	jsr.l	$00001DD8
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001C8E

l00001C88:
	move.l	d6,d0
	bra	$00001DA0

l00001C8E:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001C64

l00001C96:
	move.l	a4,$0030(a7)
	move.l	d4,$0048(a7)

l00001C9E:
	moveq	#$02,d0
	and.l	$0048(a7),d0
	bne	$00001CDA

l00001CA6:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001CDA

l00001CAC:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001DD8
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001CD2

l00001CCC:
	move.l	d6,d0
	bra	$00001DA0

l00001CD2:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001CAC

l00001CDA:
	move.l	$0066(a7),d2
	cmp.l	$0066(a7),d5
	bls	$00001D08

l00001CE4:
	move.l	a5,-(a7)
	pea	$00000030
	jsr.l	$00001DD8
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D00

l00001CFA:
	move.l	d6,d0
	bra	$00001DA0

l00001D00:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d5
	bhi	$00001CE4

l00001D08:
	moveq	#$00,d2
	tst.l	$0066(a7)
	beq	$00001D3E

l00001D10:
	movea.l	$0066(a7),a4

l00001D14:
	move.l	a5,-(a7)
	lea	(a3,d2),a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001DD8
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D36

l00001D32:
	move.l	d6,d0
	bra	$00001DA0

l00001D36:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001D14

l00001D3E:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	beq	$00001D74

l00001D46:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001D74

l00001D4E:
	movea.l	$0030(a7),a3

l00001D52:
	move.l	a5,-(a7)
	pea	$00000020
	jsr.l	$00001DD8
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D6C

l00001D68:
	move.l	d6,d0
	bra	$00001DA0

l00001D6C:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a3
	bhi	$00001D52

l00001D74:
	movea.l	a2,a4
	bra	$00001D98

l00001D78:
	move.l	a5,-(a7)
	move.b	(a4)+,d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$00001DD8
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D96

l00001D92:
	move.l	d6,d0
	bra	$00001DA0

l00001D96:
	addq.l	#$01,d6

l00001D98:
	tst.b	(a4)
	bne	$00001474

l00001D9E:
	move.l	d6,d0

l00001DA0:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$0044(a7),a7
	rts
00001DAA                               00 00 25 00 00 00           ..%...
00001DB0 23 30 2D 20 2B 00 00 00 30 31 32 33 34 35 36 37 #0- +...01234567
00001DC0 38 39 61 62 63 64 65 66 30 31 32 33 34 35 36 37 89abcdef01234567
00001DD0 38 39 41 42 43 44 45 46                         89ABCDEF        

;; fn00001DD8: 00001DD8
;;   Called from:
;;     00001C2E (in fn00001458)
;;     00001C78 (in fn00001458)
;;     00001CBC (in fn00001458)
;;     00001CEA (in fn00001458)
;;     00001D22 (in fn00001458)
;;     00001D58 (in fn00001458)
;;     00001D82 (in fn00001458)
fn00001DD8 proc
	movem.l	d2/a2-a3,-(a7)
	move.l	$0010(a7),d2
	movea.l	$0014(a7),a2
	lea	$0018(a2),a0
	moveq	#$02,d0
	or.l	d0,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001E1C

l00001DF4:
	moveq	#$0A,d0
	cmp.l	d2,d0
	bne	$00001E06

l00001DFA:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	bne	$00001E1C

l00001E06:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a3
	addq.l	#$01,a3
	move.l	a3,(a1)
	move.b	d2,(a0)
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00001E2A

l00001E1C:
	move.l	a2,-(a7)
	move.l	d2,-(a7)
	jsr.l	$00001E34
	move.l	d0,d1
	addq.w	#$08,a7

l00001E2A:
	move.l	d1,d0
	movem.l	(a7)+,d2/a2-a3
	rts
00001E32       00 00                                       ..            

;; fn00001E34: 00001E34
;;   Called from:
;;     00001E20 (in fn00001DD8)
fn00001E34 proc
	movem.l	d2-d6/a2-a4/a6,-(a7)
	move.l	$0028(a7),d5
	movea.l	$002C(a7),a2
	jsr.l	$000023C8
	move.l	a2,d0
	bne	$00001E50

l00001E4A:
	moveq	#-$01,d0
	bra	$00001F42

l00001E50:
	moveq	#$49,d0
	and.l	$0018(a2),d0
	moveq	#$40,d6
	cmp.l	d0,d6
	beq	$00001E62

l00001E5C:
	moveq	#-$01,d0
	bra	$00001F42

l00001E62:
	tst.l	$001C(a2)
	bne	$00001E80

l00001E68:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00001E78

l00001E70:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00001E80

l00001E78:
	move.l	#$00000400,$001C(a2)

l00001E80:
	tst.l	$0008(a2)
	bne	$00001EC0

l00001E86:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00001E9A

l00001E96:
	moveq	#$02,d4
	bra	$00001E9C

l00001E9A:
	moveq	#$01,d4

l00001E9C:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	$00001FF4
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00001EB6

l00001EB0:
	moveq	#-$01,d0
	bra	$00001F42

l00001EB6:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)
	bra	$00001F1E

l00001EC0:
	tst.l	(a2)
	beq	$00001F1A

l00001EC4:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00001EF0

l00001ED0:
	moveq	#$0A,d0
	cmp.l	d5,d0
	bne	$00001EF0

l00001ED6:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	d5,(a0)
	move.l	a2,-(a7)
	jsr.l	$00001F48
	addq.w	#$04,a7
	bra	$00001F42

l00001EF0:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00002B90,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00001F1E

l00001F0E:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00001F42

l00001F1A:
	moveq	#$00,d0
	bra	$00001F42

l00001F1E:
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

l00001F42:
	movem.l	(a7)+,d2-d6/a2-a4/a6
	rts

;; fn00001F48: 00001F48
;;   Called from:
;;     00001EE6 (in fn00001E34)
;;     00002A52 (in fn00002A44)
;;     00002A70 (in fn00002A44)
fn00001F48 proc
	movem.l	d2-d4/a2/a6,-(a7)
	movea.l	$0018(a7),a2
	jsr.l	$000023C8
	move.l	a2,d0
	bne	$00001F5E

l00001F5A:
	moveq	#-$01,d0
	bra	$00001FD6

l00001F5E:
	tst.l	$001C(a2)
	bne	$00001F7C

l00001F64:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00001F74

l00001F6C:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00001F7C

l00001F74:
	move.l	#$00000400,$001C(a2)

l00001F7C:
	tst.l	$0008(a2)
	bne	$00001F86

l00001F82:
	moveq	#$00,d0
	bra	$00001FD6

l00001F86:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00001FCC

l00001F8E:
	tst.l	(a2)
	beq	$00001FBC

l00001F92:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00002B90,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00001FC0

l00001FB0:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00001FD6

l00001FBC:
	moveq	#$00,d0
	bra	$00001FD6

l00001FC0:
	move.l	$0008(a2),$0004(a2)
	move.l	$001C(a2),$0014(a2)

l00001FCC:
	lea	$0018(a2),a0
	moveq	#-$04,d0
	and.l	d0,(a0)
	moveq	#$00,d0

l00001FD6:
	movem.l	(a7)+,d2-d4/a2/a6
	rts
00001FDC                                     4A B9 00 00             J...
00001FE0 2B C0 67 0E 2F 39 00 00 2B C0 4E B9 00 00 23 58 +.g./9..+.N...#X
00001FF0 58 4F 4E 75                                     XONu            

;; fn00001FF4: 00001FF4
;;   Called from:
;;     00001EA2 (in fn00001E34)
fn00001FF4 proc
	movem.l	d2,-(a7)
	move.l	$0008(a7),d2
	bne	$00002002

l00001FFE:
	moveq	#$00,d0
	bra	$00002058

l00002002:
	tst.l	$00002BC0
	bne	$00002026

l0000200A:
	movea.l	$00002BBC,a0
	move.l	a0,-(a7)
	move.l	a0,-(a7)
	clr.l	-(a7)
	jsr.l	$000022E8
	move.l	d0,$00002BC0
	lea	$000C(a7),a7

l00002026:
	tst.l	$00002BC0
	bne	$00002032

l0000202E:
	moveq	#$00,d0
	bra	$00002058

l00002032:
	moveq	#$04,d0
	add.l	d2,d0
	move.l	d0,-(a7)
	move.l	$00002BC0,-(a7)
	jsr.l	$000021C4
	movea.l	d0,a1
	addq.w	#$08,a7
	move.l	a1,d0
	bne	$00002050

l0000204C:
	moveq	#$00,d0
	bra	$00002058

l00002050:
	move.l	d2,(a1)
	lea	$0004(a1),a0
	move.l	a0,d0

l00002058:
	movem.l	(a7)+,d2
	rts
0000205E                                           00 00               ..

;; fn00002060: 00002060
fn00002060 proc
	move.l	$0004(a7),d0
	movea.l	d0,a0
	tst.l	d0
	beq	$0000208A

l0000206A:
	tst.l	$00002BC0
	beq	$0000208A

l00002072:
	moveq	#$04,d0
	add.l	-(a0),d0
	move.l	d0,-(a7)
	move.l	a0,-(a7)
	move.l	$00002BC0,-(a7)
	jsr.l	$00002128
	lea	$000C(a7),a7

l0000208A:
	rts
0000208C                                     48 E7 30 38             H.08
00002090 28 6F 00 1C 24 6F 00 18 22 0A 66 0A 2F 0C 61 00 (o..$o..".f./.a.
000020A0 FF 54 58 4F 60 7A 26 6A FF FC 2F 0C 61 00 FF 46 .TXO`z&j../.a..F
000020B0 26 00 58 4F 67 68 B9 CB 64 04 20 0C 60 02 20 0B &.XOgh..d. .`. .
000020C0 20 43 22 4A 24 00 B4 BC 00 00 00 10 65 3C 20 08  C"J$.......e< .
000020D0 22 09 C0 3C 00 01 C2 3C 00 01 B2 00 66 1A 20 08 "..<...<....f. .
000020E0 4A 01 67 04 10 D9 53 82 72 03 C2 82 94 81 20 D9 J.g...S.r..... .
000020F0 59 82 66 FA 34 01 60 14 B4 BC 00 01 00 00 65 0A Y.f.4.`.......e.
00002100 20 08 10 D9 53 82 66 FA 60 0C 20 08 53 42 65 06  ...S.f.`. .SBe.
00002110 10 D9 51 CA FF FC 2F 0A 61 00 FF 46 58 4F 20 03 ..Q.../.a..FXO .
00002120 4C DF 1C 0C 4E 75 00 00                         L...Nu..        

;; fn00002128: 00002128
;;   Called from:
;;     00002080 (in fn00002060)
fn00002128 proc
	movem.l	d2/a2-a6,-(a7)
	move.l	$0020(a7),d1
	movea.l	$0024(a7),a5
	movea.l	$001C(a7),a4
	movea.l	$00002B8C,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002158

l00002146:
	movea.l	$00002B8C,a6
	movea.l	a4,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$02CA(a6)
	bra	$000021BC

l00002158:
	move.l	a4,d2
	beq	$000021BC

l0000215C:
	tst.l	d1
	beq	$000021BC

l00002160:
	movea.l	d1,a3
	lea	-$000C(a3),a3
	cmpa.l	$0014(a4),a5
	bcc	$000021A2

l0000216C:
	movea.l	a4,a2

l0000216E:
	movea.l	(a2),a2
	tst.l	(a2)
	beq	$000021BC

l00002174:
	tst.b	$0008(a2)
	beq	$0000216E

l0000217A:
	cmp.l	$0014(a2),d1
	bcs	$0000216E

l00002180:
	cmp.l	$0018(a2),d1
	bcc	$0000216E

l00002186:
	movea.l	$00002B8C,a6
	movea.l	a2,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$00C0(a6)
	move.l	$001C(a2),d0
	cmp.l	$0010(a4),d0
	bne	$000021BC

l000021A0:
	movea.l	a2,a3

l000021A2:
	movea.l	$00002B8C,a6
	movea.l	a3,a1
	jsr.l	-$00FC(a6)
	move.l	-(a3),d0
	movea.l	$00002B8C,a6
	movea.l	a3,a1
	jsr.l	-$00D2(a6)

l000021BC:
	movem.l	(a7)+,d2/a2-a6
	rts
000021C2       00 00                                       ..            

;; fn000021C4: 000021C4
;;   Called from:
;;     0000203E (in fn00001FF4)
fn000021C4 proc
	movem.l	d2-d4/a2-a6,-(a7)
	move.l	$0028(a7),d2
	movea.l	$0024(a7),a4
	movea.l	$00002B8C,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$000021F0

l000021DE:
	movea.l	$00002B8C,a6
	movea.l	a4,a0
	move.l	d2,d0
	jsr.l	-$02C4(a6)
	bra	$000022E2

l000021F0:
	suba.l	a3,a3
	move.l	a4,d4
	beq	$000022E0

l000021F8:
	tst.l	d2
	beq	$000022E0

l000021FE:
	cmp.l	$0014(a4),d2
	bcc	$000022B2

l00002206:
	movea.l	(a4),a5

l00002208:
	tst.l	(a5)
	beq	$0000222A

l0000220C:
	tst.b	$0008(a5)
	beq	$00002226

l00002212:
	movea.l	$00002B8C,a6
	movea.l	a5,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3
	move.l	a3,d0
	bne	$00002296

l00002226:
	movea.l	(a5),a5
	bra	$00002208

l0000222A:
	moveq	#$28,d3
	add.l	$0010(a4),d3
	move.l	$000C(a4),d1
	movea.l	$00002B8C,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$000022E0

l00002248:
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
	movea.l	$00002B8C,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F0(a6)
	movea.l	$00002B8C,a6
	movea.l	a3,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3

l00002296:
	move.l	#$00010000,d0
	and.l	$000C(a4),d0
	beq	$000022E0

l000022A2:
	movea.l	a3,a2
	addq.l	#$07,d2
	lsr.l	#$03,d2

l000022A8:
	clr.l	(a2)+
	clr.l	(a2)+
	subq.l	#$01,d2
	bne	$000022A8

l000022B0:
	bra	$000022E0

l000022B2:
	moveq	#$10,d3
	add.l	d2,d3
	move.l	$000C(a4),d1
	movea.l	$00002B8C,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$000022E0

l000022CC:
	move.l	d3,(a3)+
	movea.l	$00002B8C,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F6(a6)
	addq.l	#$08,a3
	clr.l	(a3)+

l000022E0:
	move.l	a3,d0

l000022E2:
	movem.l	(a7)+,d2-d4/a2-a6
	rts

;; fn000022E8: 000022E8
;;   Called from:
;;     00002016 (in fn00001FF4)
fn000022E8 proc
	movem.l	d2-d3/a2/a6,-(a7)
	move.l	$0018(a7),d3
	movea.l	$001C(a7),a2
	movea.l	$00002B8C,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002316

l00002302:
	movea.l	$00002B8C,a6
	move.l	$0014(a7),d0
	move.l	d3,d1
	move.l	a2,d2
	jsr.l	-$02B8(a6)
	bra	$00002352

l00002316:
	suba.l	a1,a1
	cmp.l	a2,d3
	bcs	$00002350

l0000231C:
	addq.l	#$07,d3
	movea.l	$00002B8C,a6
	moveq	#$18,d0
	moveq	#$00,d1
	jsr.l	-$00C6(a6)
	movea.l	d0,a1
	move.l	a1,d0
	beq	$00002350

l00002332:
	lea	$0004(a1),a0
	move.l	a0,(a1)
	clr.l	(a0)
	move.l	a1,$0008(a1)
	move.l	$0014(a7),$000C(a1)
	moveq	#-$08,d0
	and.l	d3,d0
	move.l	d0,$0010(a1)
	move.l	a2,$0014(a1)

l00002350:
	move.l	a1,d0

l00002352:
	movem.l	(a7)+,d2-d3/a2/a6
	rts

;; fn00002358: 00002358
fn00002358 proc
	movem.l	d2/a2/a6,-(a7)
	move.l	$0010(a7),d2
	movea.l	$00002B8C,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$0000237C

l0000236E:
	movea.l	$00002B8C,a6
	movea.l	d2,a0
	jsr.l	-$02BE(a6)
	bra	$000023C0

l0000237C:
	tst.l	d2
	beq	$000023C0

l00002380:
	movea.l	$00002B8C,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d1
	beq	$000023B2

l00002392:
	move.l	-(a2),d0
	movea.l	$00002B8C,a6
	movea.l	a2,a1
	jsr.l	-$00D2(a6)
	movea.l	$00002B8C,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d0
	bne	$00002392

l000023B2:
	movea.l	$00002B8C,a6
	movea.l	d2,a1
	moveq	#$18,d0
	jsr.l	-$00D2(a6)

l000023C0:
	movem.l	(a7)+,d2/a2/a6
	rts
000023C6                   00 00                               ..        

;; fn000023C8: 000023C8
;;   Called from:
;;     00001E40 (in fn00001E34)
;;     00001F50 (in fn00001F48)
fn000023C8 proc
	movem.l	a6,-(a7)
	movea.l	$00002B8C,a6
	moveq	#$00,d0
	move.l	#$00001000,d1
	jsr.l	-$0132(a6)
	and.l	#$00001000,d0
	beq	$000023F2

l000023E6:
	pea	$00000014
	jsr.l	$0000131C
	addq.w	#$04,a7

l000023F2:
	movea.l	(a7)+,a6
	rts
000023F6                   00 00                               ..        

;; fn000023F8: 000023F8
;;   Called from:
;;     00001A2C (in fn00001458)
fn000023F8 proc
	movem.l	d2-d6,-(a7)
	move.l	$001C(a7),d1
	move.l	$0018(a7),d0
	movea.l	d1,a0
	move.l	$0024(a7),d3
	move.l	$0020(a7),d2
	bne	$0000244E

l00002410:
	cmp.l	d3,d0
	bcc	$00002422

l00002414:
	move.l	d3,d2
	jsr.l	$000024FC
	move.l	d0,d1
	bra	$000024F4

l00002422:
	tst.l	d3
	bne	$0000242E

l00002426:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l0000242E:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	$000024FC
	movea.l	d0,a1
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	$000024FC
	move.l	d0,d1
	move.l	a1,d0
	bra	$000024F6

l0000244E:
	cmp.l	d2,d0
	bcc	$00002458

l00002452:
	moveq	#$00,d0
	bra	$000024F4

l00002458:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002476

l00002462:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002476

l0000246A:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002476

l00002472:
	moveq	#$00,d4
	move.b	d2,d6

l00002476:
	lea	$00002BC4,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$00002496

l0000248A:
	cmp.l	d0,d2
	bcs	$00002492

l0000248E:
	cmp.l	a0,d3
	bhi	$00002452

l00002492:
	moveq	#$01,d0
	bra	$000024F4

l00002496:
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
	jsr.l	$000024FC
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
	bcs	$000024F2

l000024EC:
	bne	$000024F4

l000024EE:
	cmpa.l	d2,a0
	bcc	$000024F4

l000024F2:
	subq.l	#$01,d1

l000024F4:
	moveq	#$00,d0

l000024F6:
	movem.l	(a7)+,d2-d6
	rts

;; fn000024FC: 000024FC
;;   Called from:
;;     00002416 (in fn000023F8)
;;     00002434 (in fn000023F8)
;;     00002440 (in fn000023F8)
;;     000024B2 (in fn000023F8)
;;     00002666 (in fn00002648)
;;     00002684 (in fn00002648)
;;     0000268E (in fn00002648)
;;     000026FC (in fn00002648)
fn000024FC proc
	movem.l	d5-d7,-(a7)
	move.l	d2,d7
	beq	$00002516

l00002504:
	move.l	d1,d6
	move.l	d0,d5
	bne	$00002524

l0000250A:
	tst.l	d1
	beq	$00002642

l00002510:
	cmp.l	d1,d2
	bhi	$00002642

l00002516:
	move.l	d1,d0
	move.l	d2,d1
	jsr.l	$00002AFA
	bra	$00002642

l00002524:
	swap.l	d2
	tst.w	d2
	bne	$0000254C

l0000252A:
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
	bra	$00002642

l0000254C:
	movem.l	d2-d4/a0-a1,-(a7)
	subq.l	#$08,a7
	clr.b	$0002(a7)
	moveq	#$00,d1
	moveq	#$00,d0
	tst.l	d7
	bmi	$00002568

l0000255E:
	addq.w	#$01,d0
	add.l	d6,d6
	addx.l	d5,d5
	add.l	d7,d7
	bpl	$0000255E

l00002568:
	move.w	d0,(a7)

l0000256A:
	move.l	d7,d3
	move.l	d5,d2
	swap.l	d2
	swap.l	d3
	cmp.w	d3,d2
	bne	$0000257C

l00002576:
	move.w	#$FFFF,d1
	bra	$00002586

l0000257C:
	move.l	d5,d1
	divu.w	d3,d1
	swap.l	d1
	clr.w	d1
	swap.l	d1

l00002586:
	movea.l	d6,a1
	clr.w	d6
	swap.l	d6

l0000258C:
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
	bne	$000025AC

l000025A4:
	cmp.l	d4,d2
	bls	$000025AC

l000025A8:
	subq.l	#$01,d1
	bra	$0000258C

l000025AC:
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
	bcc	$00002604

l000025EE:
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

l00002604:
	tst.b	$0002(a7)
	bne	$00002620

l0000260A:
	move.w	d1,$0004(a7)
	moveq	#$00,d1
	swap.l	d5
	swap.l	d6
	move.w	d6,d5
	clr.w	d6
	st	$0002(a7)
	bra	$0000256A

l00002620:
	move.l	$0004(a7),d0
	move.w	d1,d0
	move.w	d5,d6
	swap.l	d6
	swap.l	d5
	move.w	(a7),d7
	beq	$0000263A

l00002630:
	subq.w	#$01,d7

l00002632:
	lsr.l	#$01,d5
	roxr.l	#$01,d6
	dbra	d7,$00002632

l0000263A:
	move.l	d6,d1
	addq.l	#$08,a7
	movem.l	(a7)+,d2-d4/a0-a1

l00002642:
	movem.l	(a7)+,d5-d7
	rts

;; fn00002648: 00002648
;;   Called from:
;;     00001A00 (in fn00001458)
fn00002648 proc
	movem.l	d2-d7,-(a7)
	move.l	$0020(a7),d1
	move.l	$001C(a7),d0
	movea.l	d1,a0
	move.l	$0028(a7),d3
	move.l	$0024(a7),d2
	bne	$0000269A

l00002660:
	cmp.l	d3,d0
	bcc	$00002672

l00002664:
	move.l	d3,d2
	jsr.l	$000024FC
	moveq	#$00,d0
	bra	$00002754

l00002672:
	tst.l	d3
	bne	$0000267E

l00002676:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l0000267E:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	$000024FC
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	$000024FC
	moveq	#$00,d0
	bra	$00002754

l0000269A:
	cmp.l	d2,d0
	bcs	$00002754

l000026A0:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000026BE

l000026AA:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000026BE

l000026B2:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000026BE

l000026BA:
	moveq	#$00,d4
	move.b	d2,d6

l000026BE:
	lea	$00002BC4,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$000026E0

l000026D2:
	cmp.l	d0,d2
	bcs	$000026DA

l000026D6:
	cmp.l	d1,d3
	bhi	$00002754

l000026DA:
	sub.l	d3,d1
	subx.l	d2,d0
	bra	$00002754

l000026E0:
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
	jsr.l	$000024FC
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
	bcs	$0000273C

l00002736:
	bne	$00002740

l00002738:
	cmpa.l	d3,a0
	bcc	$00002740

l0000273C:
	sub.l	a1,d3
	subx.l	d0,d2

l00002740:
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

l00002754:
	movem.l	(a7)+,d2-d7
	rts
0000275A                               00 00 00 20 20 20           ...   
00002760 20 20 20 20 20 20 28 28 28 28 28 20 20 20 20 20       (((((     
00002770 20 20 20 20 20 20 20 20 20 20 20 20 20 88 10 10              ...
00002780 10 10 10 10 10 10 10 10 10 10 10 10 10 04 04 04 ................
00002790 04 04 04 04 04 04 04 10 10 10 10 10 10 10 41 41 ..............AA
000027A0 41 41 41 41 01 01 01 01 01 01 01 01 01 01 01 01 AAAA............
000027B0 01 01 01 01 01 01 01 01 10 10 10 10 10 10 42 42 ..............BB
000027C0 42 42 42 42 02 02 02 02 02 02 02 02 02 02 02 02 BBBB............
000027D0 02 02 02 02 02 02 02 02 10 10 10 10 20 00 00 00 ............ ...
000027E0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00002860 48 E7 00 32 48 78 00 20 4E B9 00 00 1F F4 23 C0 H..2Hx. N.....#.
00002870 00 00 2C E4 48 78 00 20 4E B9 00 00 1F F4 23 C0 ..,.Hx. N.....#.
00002880 00 00 2C E8 48 78 00 20 4E B9 00 00 1F F4 23 C0 ..,.Hx. N.....#.
00002890 00 00 2C EC 4F EF 00 0C 4A B9 00 00 2C E4 67 10 ..,.O...J...,.g.
000028A0 4A B9 00 00 2C E8 67 08 4A B9 00 00 2C EC 66 0C J...,.g.J...,.f.
000028B0 48 78 00 14 4E B9 00 00 13 1C 58 4F 20 79 00 00 Hx..N.....XO y..
000028C0 2C E4 20 B9 00 00 2B 9C 20 79 00 00 2C E4 70 20 ,. ...+. y..,.p 
000028D0 21 40 00 18 22 39 00 00 2B 9C 2C 79 00 00 2B 90 !@.."9..+.,y..+.
000028E0 4E AE FF 28 4A 80 67 10 20 79 00 00 2C E4 41 E8 N..(J.g. y..,.A.
000028F0 00 18 00 90 00 00 02 04 20 79 00 00 2C E8 20 B9 ........ y..,. .
00002900 00 00 2B A0 20 79 00 00 2C E8 70 40 21 40 00 18 ..+. y..,.p@!@..
00002910 22 39 00 00 2B A0 2C 79 00 00 2B 90 4E AE FF 28 "9..+.,y..+.N..(
00002920 4A 80 67 10 20 79 00 00 2C E8 41 E8 00 18 00 90 J.g. y..,.A.....
00002930 00 00 02 80 20 79 00 00 2C EC 20 B9 00 00 2B A4 .... y..,. ...+.
00002940 20 79 00 00 2C EC 70 40 21 40 00 18 22 39 00 00  y..,.p@!@.."9..
00002950 2B A4 2C 79 00 00 2B 90 4E AE FF 28 4A 80 67 10 +.,y..+.N..(J.g.
00002960 20 79 00 00 2C EC 41 E8 00 18 00 90 00 00 02 80  y..,.A.........
00002970 20 79 00 00 2C EC 42 A8 00 04 20 79 00 00 2C E8  y..,.B... y..,.
00002980 42 A8 00 04 20 79 00 00 2C E4 42 A8 00 04 20 79 B... y..,.B... y
00002990 00 00 2C EC 42 A8 00 08 20 79 00 00 2C E8 42 A8 ..,.B... y..,.B.
000029A0 00 08 20 79 00 00 2C E4 42 A8 00 08 24 79 00 00 .. y..,.B...$y..
000029B0 2C EC 42 AA 00 14 22 79 00 00 2C E8 42 A9 00 14 ,.B..."y..,.B...
000029C0 20 79 00 00 2C E4 42 A8 00 14 42 AA 00 1C 42 A9  y..,.B...B...B.
000029D0 00 1C 42 A8 00 1C 42 A8 00 10 20 79 00 00 2C E4 ..B...B... y..,.
000029E0 21 79 00 00 2C E8 00 0C 20 79 00 00 2C E8 21 79 !y..,... y..,.!y
000029F0 00 00 2C E4 00 10 20 79 00 00 2C E8 21 79 00 00 ..,... y..,.!y..
00002A00 2C EC 00 0C 20 79 00 00 2C EC 21 79 00 00 2C E8 ,... y..,.!y..,.
00002A10 00 10 20 79 00 00 2C EC 42 A8 00 0C 23 F9 00 00 .. y..,.B...#...
00002A20 2C E4 00 00 2C F0 23 F9 00 00 2C EC 00 00 2C F4 ,...,.#...,...,.
00002A30 4C DF 4C 00 4E 75 00 00 42 A7 4E B9 00 00 2A 44 L.L.Nu..B.N...*D
00002A40 58 4F 4E 75                                     XONu            

;; fn00002A44: 00002A44
fn00002A44 proc
	movem.l	a2,-(a7)
	movea.l	$0008(a7),a2
	move.l	a2,d0
	beq	$00002A5C

l00002A50:
	move.l	a2,-(a7)
	jsr.l	$00001F48
	addq.w	#$04,a7
	bra	$00002A82

l00002A5C:
	movea.l	$00002CF0,a2
	move.l	a2,d0
	beq	$00002A82

l00002A66:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002A78

l00002A6E:
	move.l	a2,-(a7)
	jsr.l	$00001F48
	addq.w	#$04,a7

l00002A78:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00002A66

l00002A82:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	rts

;; fn00002A88: 00002A88
;;   Called from:
;;     000013E8 (in fn00001390)
fn00002A88 proc
	movem.l	$0004(a7),d0-d1
	tst.l	d1
	bmi	$00002A9C

l00002A92:
	tst.l	d0
	bmi	$00002AA8

l00002A96:
	bsr	$00002AFA
	move.l	d1,d0
	rts

l00002A9C:
	neg.l	d1
	tst.l	d0
	bmi	$00002AB2

l00002AA2:
	bsr	$00002AFA
	move.l	d1,d0
	rts

l00002AA8:
	neg.l	d0
	bsr	$00002AFA
	neg.l	d1
	move.l	d1,d0
	rts

l00002AB2:
	neg.l	d0
	bsr	$00002AFA
	neg.l	d1
	move.l	d1,d0
	rts
00002ABC                                     4C EF 00 03             L...
00002AC0 00 04 61 36 20 01 4E 75                         ..a6 .Nu        

;; fn00002AC8: 00002AC8
;;   Called from:
;;     000013CC (in fn00001390)
fn00002AC8 proc
	movem.l	$0004(a7),d0-d1
	tst.l	d0
	bpl	$00002AE8

l00002AD2:
	neg.l	d0
	tst.l	d1
	bpl	$00002AE0

l00002AD8:
	neg.l	d1
	bsr	$00002AFA
	neg.l	d1
	rts

l00002AE0:
	bsr	$00002AFA
	neg.l	d0
	neg.l	d1
	rts

l00002AE8:
	tst.l	d1
	bpl	$00002AFA

l00002AEC:
	neg.l	d1
	bsr	$00002AFA
	neg.l	d0
	rts
00002AF4             4C EF 00 03 00 04                       L.....      

;; fn00002AFA: 00002AFA
;;   Called from:
;;     0000251A (in fn000024FC)
;;     00002A96 (in fn00002A88)
;;     00002AA2 (in fn00002A88)
;;     00002AAA (in fn00002A88)
;;     00002AB4 (in fn00002A88)
;;     00002ADA (in fn00002AC8)
;;     00002AE0 (in fn00002AC8)
;;     00002AEA (in fn00002AC8)
;;     00002AEE (in fn00002AC8)
fn00002AFA proc
	move.l	d2,-(a7)
	swap.l	d1
	move.w	d1,d2
	bne	$00002B20

l00002B02:
	swap.l	d0
	swap.l	d1
	swap.l	d2
	move.w	d0,d2
	beq	$00002B10

l00002B0C:
	divu.w	d1,d2
	move.w	d2,d0

l00002B10:
	swap.l	d0
	move.w	d0,d2
	divu.w	d1,d2
	move.w	d2,d0
	swap.l	d2
	move.w	d2,d1
	move.l	(a7)+,d2
	rts

l00002B20:
	move.l	d3,-(a7)
	moveq	#$10,d3
	cmp.w	#$0080,d1
	bcc	$00002B2E

l00002B2A:
	rol.l	#$08,d1
	subq.w	#$08,d3

l00002B2E:
	cmp.w	#$0800,d1
	bcc	$00002B38

l00002B34:
	rol.l	#$04,d1
	subq.w	#$04,d3

l00002B38:
	cmp.w	#$2000,d1
	bcc	$00002B42

l00002B3E:
	rol.l	#$02,d1
	subq.w	#$02,d3

l00002B42:
	tst.w	d1
	bmi	$00002B4A

l00002B46:
	rol.l	#$01,d1
	subq.w	#$01,d3

l00002B4A:
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
	bcc	$00002B6C

l00002B66:
	subq.w	#$01,d3
	add.l	d1,d0

l00002B6A:
	bcc	$00002B6A

l00002B6C:
	moveq	#$00,d1
	move.w	d3,d1
	swap.l	d3
	rol.l	d3,d0
	swap.l	d0
	exg	d0,d1
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	rts
00002B7E                                           00 00               ..
00002B80 00 24 00 63 41 00 00 00 00 00 00 00 00 00 40 00 .$.cA.........@.
00002B90 00 00 00 00 00 01 02 02 03 03 03 03 04 04 04 04 ................
00002BA0 04 04 04 04 05 05 05 05 05 05 05 05 05 05 05 05 ................
00002BB0 05 05 05 05 06 06 06 06 06 06 06 06 06 06 06 06 ................
00002BC0 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 ................
00002BD0 06 06 06 06 07 07 07 07 07 07 07 07 07 07 07 07 ................
00002BE0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00002BF0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00002C00 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00002C10 07 07 07 07 08 08 08 08 08 08 08 08 08 08 08 08 ................
00002C20 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00002C30 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00002C40 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00002C50 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00002C60 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00002C70 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00002C80 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00002C90 08 08 08 08 00 00 00 01 00 00 28 60 00 00 00 00 ..........(`....
00002CA0 00 00 00 02 00 00 1F DC 00 00 2A 38 00 00 00 00 ..........*8....
00002CB0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00002CC0 00 00 00 00 00 00 00 00                         ........        
