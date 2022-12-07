;;; Segment code (00001000)

;; fn00001000: 00001000
fn00001000 proc
	bra	$0000100A
00001002       56 42 43 43 20 30 2E 39                     VBCC 0.9      

l0000100A:
	move.l	d0,d2
	movea.l	a0,a2
	lea	$0000BE8A,a4
	movea.l	$00000004,a6
	cmpi.w	#$0024,$0014(a6)
	bcc	$00001036

l00001020:
	lea	$00003E94,a0
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
	lea	$0000BE8A,a4
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
	lea	$00003FDC,a3
	move.l	#$00003FDC,d0
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
;;     000013FC (in fn00001390)
;;     00002446 (in fn00002424)
fn0000131C proc
	movem.l	a2-a3,-(a7)
	tst.l	$00003EC4
	bne	$0000134E

l00001328:
	movea.l	$00003FEC,a3
	moveq	#$01,d0
	move.l	d0,$00003EC4
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
	lea	$00003FD4,a3
	move.l	#$00003FD0,d0
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
	lea	-$0010(a7),a7
	movem.l	d2-d3,-(a7)
	pea	00001438                                               ; $00A0(pc)
	jsr.l	fn00003E04
	lea	$0010(a7),a0
	move.l	a0,-(a7)
	pea	00001458                                               ; $00B0(pc)
	jsr.l	fn00002BDC
	moveq	#$01,d2
	lea	$000C(a7),a7
	bra	$000013F4

l000013BA:
	pea	00001410                                               ; $0056(pc)
	jsr.l	fn00003E04
	lea	$0014(a7),a0
	move.l	a0,-(a7)
	pea	00001420                                               ; $0056(pc)
	jsr.l	fn00002BDC
	move.l	$001C(a7),-(a7)
	bsr	fn0000145C
	move.l	d0,d3
	move.l	d3,-(a7)
	move.l	$0024(a7),-(a7)
	pea	00001424                                               ; $0040(pc)
	jsr.l	fn00001498
	lea	$001C(a7),a7
	addq.l	#$01,d2

l000013F4:
	cmp.l	$000C(a7),d2
	ble	$000013BA

l000013FA:
	clr.l	-(a7)
	jsr.l	fn0000131C
	addq.w	#$04,a7
	movem.l	(a7)+,d2-d3
	lea	$0010(a7),a7
	rts
0000140E                                           00 00               ..
00001410 49 6E 70 75 74 20 6E 75 6D 62 65 72 3A 20 00 00 Input number: ..
00001420 25 64 00 00 66 69 62 6F 6E 61 63 63 69 28 25 64 %d..fibonacci(%d
00001430 29 20 3D 20 25 75 0A 00 49 6E 70 75 74 20 6E 75 ) = %u..Input nu
00001440 6D 62 65 72 20 6F 66 20 69 74 65 72 61 74 69 6F mber of iteratio
00001450 6E 73 3A 20 00 00 00 00 25 64 00 00             ns: ....%d..    

;; fn0000145C: 0000145C
;;   Called from:
;;     000013D8 (in fn00001390)
;;     00001472 (in fn0000145C)
;;     0000147E (in fn0000145C)
fn0000145C proc
	subq.w	#$04,a7
	movem.l	d2,-(a7)
	move.l	$000C(a7),d2
	moveq	#$02,d0
	cmp.l	d2,d0
	bge	$0000148C

l0000146C:
	move.l	d2,d0
	subq.l	#$01,d0
	move.l	d0,-(a7)
	bsr	fn0000145C
	move.l	d2,d1
	subq.l	#$02,d1
	move.l	d1,-(a7)
	move.l	d0,$000C(a7)
	bsr	fn0000145C
	move.l	d0,d1
	move.l	$000C(a7),d0
	add.l	d1,d0
	addq.w	#$08,a7
	bra	$0000148E

l0000148C:
	moveq	#$01,d0

l0000148E:
	movem.l	(a7)+,d2
	addq.w	#$04,a7
	rts
00001496                   00 00                               ..        

;; fn00001498: 00001498
;;   Called from:
;;     000013E8 (in fn00001390)
fn00001498 proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00003FF4,-(a7)
	jsr.l	fn000014B4
	lea	$000C(a7),a7
	rts

;; fn000014B4: 000014B4
;;   Called from:
;;     000014A8 (in fn00001498)
fn000014B4 proc
	lea	-$0044(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$007C(a7),d3
	movea.l	$0074(a7),a5
	movea.l	$0078(a7),a4
	moveq	#$00,d6
	tst.b	(a4)
	beq	$00001DFA

l000014D0:
	cmpi.b	#$25,(a4)
	bne	$00001DD4

l000014D8:
	clr.l	$0040(a7)
	moveq	#-$01,d5
	clr.l	$0048(a7)
	moveq	#$69,d4
	lea	$004C(a7),a3
	moveq	#$00,d7
	clr.l	$0066(a7)
	lea	$0001(a4),a2
	move.l	$0048(a7),d2

l000014F6:
	moveq	#$00,d1

l000014F8:
	lea	00001E0C,a0                                            ; $0914(pc)
	move.l	d0,-(a7)
	move.b	(a0,d1),d0
	cmp.b	(a2),d0
	movem.l	(a7)+,d0
	bne	$0000151C

l0000150A:
	move.l	d1,d0
	move.l	d1,-(a7)
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	move.l	(a7)+,d1
	or.l	d0,d2
	addq.l	#$01,a2
	bra	$00001526

l0000151C:
	addq.l	#$01,d1
	cmp.l	#$00000005,d1
	bcs	$000014F8

l00001526:
	cmp.l	#$00000005,d1
	bcs	$000014F6

l0000152E:
	move.l	d2,$0048(a7)
	cmpi.b	#$2A,(a2)
	bne	$0000156C

l00001538:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	bge	$00001564

l00001550:
	ori.l	#$00000004,$0048(a7)
	move.l	$002C(a7),d0
	neg.l	d0
	move.l	d0,$0040(a7)
	bra	$000015D8

l00001564:
	move.l	$002C(a7),$0040(a7)
	bra	$000015D8

l0000156C:
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$000015D8

l00001588:
	move.l	$0040(a7),d2

l0000158C:
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
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$0000158C

l000015D4:
	move.l	d2,$0040(a7)

l000015D8:
	cmpi.b	#$2E,(a2)
	bne	$0000166C

l000015E0:
	addq.l	#$01,a2
	cmpi.b	#$2A,(a2)
	bne	$00001606

l000015E8:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	blt	$0000166C

l00001600:
	move.l	$002C(a7),d5
	bra	$0000166C

l00001606:
	moveq	#$00,d5
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$0000166C

l00001624:
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
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00001624

l0000166C:
	cmpi.b	#$68,(a2)
	beq	$00001690

l00001672:
	cmpi.b	#$6C,(a2)
	beq	$00001690

l00001678:
	cmpi.b	#$4C,(a2)
	beq	$00001690

l0000167E:
	cmpi.b	#$6A,(a2)
	beq	$00001690

l00001684:
	cmpi.b	#$7A,(a2)
	beq	$00001690

l0000168A:
	cmpi.b	#$74,(a2)
	bne	$00001696

l00001690:
	move.b	(a2)+,d4
	ext.w	d4
	ext.l	d4

l00001696:
	cmp.l	#$00000068,d4
	bne	$000016A8

l0000169E:
	cmpi.b	#$68,(a2)
	bne	$000016A8

l000016A4:
	moveq	#$02,d4
	addq.l	#$01,a2

l000016A8:
	cmp.l	#$0000006C,d4
	bne	$000016BA

l000016B0:
	cmpi.b	#$6C,(a2)
	bne	$000016BA

l000016B6:
	moveq	#$01,d4
	addq.l	#$01,a2

l000016BA:
	cmp.l	#$0000006A,d4
	bne	$000016C4

l000016C2:
	moveq	#$01,d4

l000016C4:
	cmp.l	#$0000007A,d4
	bne	$000016CE

l000016CC:
	moveq	#$6C,d4

l000016CE:
	cmp.l	#$00000074,d4
	bne	$000016D8

l000016D6:
	moveq	#$69,d4

l000016D8:
	move.b	(a2)+,d1
	move.b	d1,d0
	cmp.b	#$25,d1
	beq	$00001B60

l000016E4:
	cmp.b	#$58,d0
	beq	$00001732

l000016EA:
	cmp.b	#$63,d0
	beq	$00001AE0

l000016F2:
	cmp.b	#$64,d0
	beq	$00001732

l000016F8:
	cmp.b	#$69,d0
	beq	$00001732

l000016FE:
	move.b	d0,$002C(a7)
	cmp.b	#$6E,d0
	beq	$00001B72

l0000170A:
	move.b	$002C(a7),d0
	sub.b	#$6F,d0
	cmp.b	#$01,d0
	bls	$00001732

l00001718:
	move.b	$002C(a7),d0
	cmp.b	#$73,d0
	beq	$00001B1C

l00001724:
	cmp.b	#$75,d0
	beq	$00001732

l0000172A:
	cmp.b	#$78,d0
	bne	$00001C0A

l00001732:
	cmp.b	#$70,d1
	bne	$00001744

l00001738:
	moveq	#$6C,d4
	moveq	#$78,d1
	ori.l	#$00000001,$0048(a7)

l00001744:
	cmp.b	#$64,d1
	beq	$00001752

l0000174A:
	cmp.b	#$69,d1
	bne	$0000189C

l00001752:
	cmp.l	#$00000001,d4
	bne	$00001778

l0000175A:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$0034(a7)
	move.l	-$0008(a0),$0030(a7)
	bra	$00001810

l00001778:
	cmp.l	#$0000006C,d4
	bne	$000017A4

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
	bra	$00001810

l000017A4:
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
	bne	$000017EA

l000017CE:
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

l000017EA:
	cmp.l	#$00000002,d4
	bne	$00001810

l000017F2:
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

l00001810:
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
	bge	$0000185A

l0000183A:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2D,(a0)
	movem.l	$0030(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0038(a7)
	bra	$000019D0

l0000185A:
	move.b	$002C(a7),d1
	moveq	#$10,d0
	and.l	$0048(a7),d0
	beq	$00001874

l00001866:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2B,(a0)
	bra	$00001888

l00001874:
	moveq	#$08,d0
	and.l	$0048(a7),d0
	beq	$00001888

l0000187C:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$20,(a0)

l00001888:
	move.l	$0034(a7),$003C(a7)
	move.l	$0030(a7),$0038(a7)
	move.b	d1,$002C(a7)
	bra	$000019D0

l0000189C:
	cmp.l	#$00000001,d4
	bne	$000018C0

l000018A4:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	move.l	-$0008(a0),$0038(a7)
	bra	$000018FA

l000018C0:
	cmp.l	#$0000006C,d4
	bne	$000018E2

l000018C8:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)
	bra	$000018FA

l000018E2:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)

l000018FA:
	cmp.l	#$00000068,d4
	bne	$00001916

l00001902:
	move.w	$003E(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.w	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l00001916:
	cmp.l	#$00000002,d4
	bne	$00001932

l0000191E:
	move.b	$003F(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l00001932:
	moveq	#$01,d0
	and.l	$0048(a7),d0
	move.b	d1,$002C(a7)
	tst.l	d0
	beq	$000019D0

l00001942:
	cmp.b	#$6F,d1
	bne	$0000197E

l00001948:
	tst.l	d5
	bne	$00001972

l0000194C:
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
	beq	$0000197E

l00001972:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$30,(a0)

l0000197E:
	cmp.b	#$78,d1
	beq	$0000198E

l00001984:
	move.b	d1,$002C(a7)
	cmp.b	#$58,d1
	bne	$000019D0

l0000198E:
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
	beq	$000019D0

l000019B8:
	lea	$006A(a7),a0
	lea	(a0,d7),a1
	addq.l	#$01,d7
	move.b	#$30,(a1)
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	d1,(a0)
	move.b	d1,$002C(a7)

l000019D0:
	move.b	$002C(a7),d1
	lea	$0062(a7),a3
	cmp.b	#$78,d1
	beq	$000019E4

l000019DE:
	cmp.b	#$58,d1
	bne	$000019EE

l000019E4:
	move.l	#$00000010,$002C(a7)
	bra	$00001A0C

l000019EE:
	cmp.b	#$6F,d1
	bne	$000019FE

l000019F4:
	move.l	#$00000008,$0030(a7)
	bra	$00001A06

l000019FE:
	move.l	#$0000000A,$0030(a7)

l00001A06:
	move.l	$0030(a7),$002C(a7)

l00001A0C:
	move.l	$002C(a7),$006C(a7)
	cmp.b	#$58,d1
	beq	$00001A1E

l00001A18:
	lea	00001E14,a6                                            ; $03FC(pc)
	bra	$00001A22

l00001A1E:
	lea	00001E24,a6                                            ; $0406(pc)

l00001A22:
	move.l	a6,$002C(a7)
	move.l	d3,$007C(a7)
	move.l	d5,$0044(a7)
	move.l	d6,$0030(a7)
	move.l	d7,$0062(a7)
	movem.l	$0038(a7),d6-d7
	move.l	$0066(a7),d3
	movea.l	$002C(a7),a1

l00001A44:
	move.l	$006C(a7),d1
	move.l	d1,d0
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	a1,-(a7)
	movem.l	d0-d1,-(a7)
	movem.l	d6-d7,-(a7)
	jsr.l	fn0000279C
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
	jsr.l	fn00002454
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
	bne	$00001A44

l00001AB2:
	move.l	d3,$0066(a7)
	move.l	$0062(a7),d7
	move.l	$0030(a7),d6
	move.l	$0044(a7),d5
	move.l	$007C(a7),d3
	cmp.l	#$FFFFFFFF,d5
	bne	$00001AD4

l00001ACE:
	moveq	#$00,d5
	bra	$00001C20

l00001AD4:
	andi.l	#$FFFFFFFD,$0048(a7)
	bra	$00001C20

l00001AE0:
	cmp.l	#$0000006C,d4
	bne	$00001AFC

l00001AE8:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)
	bra	$00001B0E

l00001AFC:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)

l00001B0E:
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001C20

l00001B1C:
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
	beq	$00001B3E

l00001B38:
	cmp.l	$0066(a7),d5
	bls	$00001B5A

l00001B3E:
	tst.b	(a1)
	beq	$00001B5A

l00001B42:
	move.l	$0066(a7),d0

l00001B46:
	addq.l	#$01,d0
	addq.l	#$01,a1
	tst.l	d5
	bls	$00001B52

l00001B4E:
	cmp.l	d0,d5
	bls	$00001B56

l00001B52:
	tst.b	(a1)
	bne	$00001B46

l00001B56:
	move.l	d0,$0066(a7)

l00001B5A:
	moveq	#$00,d5
	bra	$00001C20

l00001B60:
	lea	00001E08,a3                                            ; $02A8(pc)
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001C20

l00001B72:
	cmp.l	#$00000001,d4
	bne	$00001B94

l00001B7A:
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
	bra	$00001C02

l00001B94:
	cmp.l	#$0000006C,d4
	bne	$00001BB2

l00001B9C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)
	bra	$00001C02

l00001BB2:
	cmp.l	#$00000068,d4
	bne	$00001BD0

l00001BBA:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.w	d6,(a0)
	bra	$00001C02

l00001BD0:
	cmp.l	#$00000002,d4
	bne	$00001BEE

l00001BD8:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.b	d6,(a0)
	bra	$00001C02

l00001BEE:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)

l00001C02:
	moveq	#$00,d5
	clr.l	$0040(a7)
	bra	$00001C20

l00001C0A:
	tst.b	d1
	bne	$00001C10

l00001C0E:
	subq.l	#$01,a2

l00001C10:
	movea.l	a4,a3
	move.l	a2,d0
	sub.l	a4,d0
	move.l	d0,$0066(a7)
	moveq	#$00,d5
	clr.l	$0040(a7)

l00001C20:
	cmp.l	$0066(a7),d5
	bhi	$00001C2E

l00001C26:
	move.l	$0066(a7),$002C(a7)
	bra	$00001C32

l00001C2E:
	move.l	d5,$002C(a7)

l00001C32:
	move.l	d0,-(a7)
	move.l	$0030(a7),d0
	add.l	d7,d0
	move.l	d0,$0034(a7)
	move.l	(a7)+,d0
	move.l	d0,-(a7)
	move.l	$0034(a7),d0
	cmp.l	$0044(a7),d0
	movem.l	(a7)+,d0
	bcs	$00001C56

l00001C50:
	clr.l	$002C(a7)
	bra	$00001C66

l00001C56:
	move.l	d0,-(a7)
	move.l	$0044(a7),d0
	sub.l	$0034(a7),d0
	move.l	d0,$0030(a7)
	move.l	(a7)+,d0

l00001C66:
	move.l	$002C(a7),$0030(a7)
	moveq	#$02,d0
	and.l	$0048(a7),d0
	beq	$00001CA8

l00001C74:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001CA8

l00001C7A:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00001E34
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001CA0

l00001C9A:
	move.l	d6,d0
	bra	$00001DFC

l00001CA0:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001C7A

l00001CA8:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	bne	$00001CFA

l00001CB0:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001CFA

l00001CB8:
	move.l	$0048(a7),d4
	movea.l	$0030(a7),a4

l00001CC0:
	move.l	a5,-(a7)
	moveq	#$02,d0
	and.l	d4,d0
	beq	$00001CCE

l00001CC8:
	movea.w	#$0030,a0
	bra	$00001CD2

l00001CCE:
	movea.w	#$0020,a0

l00001CD2:
	move.l	a0,-(a7)
	jsr.l	fn00001E34
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001CEA

l00001CE4:
	move.l	d6,d0
	bra	$00001DFC

l00001CEA:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001CC0

l00001CF2:
	move.l	a4,$0030(a7)
	move.l	d4,$0048(a7)

l00001CFA:
	moveq	#$02,d0
	and.l	$0048(a7),d0
	bne	$00001D36

l00001D02:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001D36

l00001D08:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00001E34
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D2E

l00001D28:
	move.l	d6,d0
	bra	$00001DFC

l00001D2E:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001D08

l00001D36:
	move.l	$0066(a7),d2
	cmp.l	$0066(a7),d5
	bls	$00001D64

l00001D40:
	move.l	a5,-(a7)
	pea	$00000030
	jsr.l	fn00001E34
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D5C

l00001D56:
	move.l	d6,d0
	bra	$00001DFC

l00001D5C:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d5
	bhi	$00001D40

l00001D64:
	moveq	#$00,d2
	tst.l	$0066(a7)
	beq	$00001D9A

l00001D6C:
	movea.l	$0066(a7),a4

l00001D70:
	move.l	a5,-(a7)
	lea	(a3,d2),a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00001E34
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001D92

l00001D8E:
	move.l	d6,d0
	bra	$00001DFC

l00001D92:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001D70

l00001D9A:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	beq	$00001DD0

l00001DA2:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001DD0

l00001DAA:
	movea.l	$0030(a7),a3

l00001DAE:
	move.l	a5,-(a7)
	pea	$00000020
	jsr.l	fn00001E34
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001DC8

l00001DC4:
	move.l	d6,d0
	bra	$00001DFC

l00001DC8:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a3
	bhi	$00001DAE

l00001DD0:
	movea.l	a2,a4
	bra	$00001DF4

l00001DD4:
	move.l	a5,-(a7)
	move.b	(a4)+,d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00001E34
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001DF2

l00001DEE:
	move.l	d6,d0
	bra	$00001DFC

l00001DF2:
	addq.l	#$01,d6

l00001DF4:
	tst.b	(a4)
	bne	$000014D0

l00001DFA:
	move.l	d6,d0

l00001DFC:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$0044(a7),a7
	rts
00001E06                   00 00 25 00 00 00 23 30 2D 20       ..%...#0- 
00001E10 2B 00 00 00 30 31 32 33 34 35 36 37 38 39 61 62 +...0123456789ab
00001E20 63 64 65 66 30 31 32 33 34 35 36 37 38 39 41 42 cdef0123456789AB
00001E30 43 44 45 46                                     CDEF            

;; fn00001E34: 00001E34
;;   Called from:
;;     00001C8A (in fn000014B4)
;;     00001CD4 (in fn000014B4)
;;     00001D18 (in fn000014B4)
;;     00001D46 (in fn000014B4)
;;     00001D7E (in fn000014B4)
;;     00001DB4 (in fn000014B4)
;;     00001DDE (in fn000014B4)
fn00001E34 proc
	movem.l	d2/a2-a3,-(a7)
	move.l	$0010(a7),d2
	movea.l	$0014(a7),a2
	lea	$0018(a2),a0
	moveq	#$02,d0
	or.l	d0,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001E78

l00001E50:
	moveq	#$0A,d0
	cmp.l	d2,d0
	bne	$00001E62

l00001E56:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	bne	$00001E78

l00001E62:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a3
	addq.l	#$01,a3
	move.l	a3,(a1)
	move.b	d2,(a0)
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00001E86

l00001E78:
	move.l	a2,-(a7)
	move.l	d2,-(a7)
	jsr.l	fn00001E90
	move.l	d0,d1
	addq.w	#$08,a7

l00001E86:
	move.l	d1,d0
	movem.l	(a7)+,d2/a2-a3
	rts
00001E8E                                           00 00               ..

;; fn00001E90: 00001E90
;;   Called from:
;;     00001E7C (in fn00001E34)
;;     00003E68 (in fn00003E04)
fn00001E90 proc
	movem.l	d2-d6/a2-a4/a6,-(a7)
	move.l	$0028(a7),d5
	movea.l	$002C(a7),a2
	jsr.l	fn00002424
	move.l	a2,d0
	bne	$00001EAC

l00001EA6:
	moveq	#-$01,d0
	bra	$00001F9E

l00001EAC:
	moveq	#$49,d0
	and.l	$0018(a2),d0
	moveq	#$40,d6
	cmp.l	d0,d6
	beq	$00001EBE

l00001EB8:
	moveq	#-$01,d0
	bra	$00001F9E

l00001EBE:
	tst.l	$001C(a2)
	bne	$00001EDC

l00001EC4:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00001ED4

l00001ECC:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00001EDC

l00001ED4:
	move.l	#$00000400,$001C(a2)

l00001EDC:
	tst.l	$0008(a2)
	bne	$00001F1C

l00001EE2:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00001EF6

l00001EF2:
	moveq	#$02,d4
	bra	$00001EF8

l00001EF6:
	moveq	#$01,d4

l00001EF8:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	fn00002050
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00001F12

l00001F0C:
	moveq	#-$01,d0
	bra	$00001F9E

l00001F12:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)
	bra	$00001F7A

l00001F1C:
	tst.l	(a2)
	beq	$00001F76

l00001F20:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00001F4C

l00001F2C:
	moveq	#$0A,d0
	cmp.l	d5,d0
	bne	$00001F4C

l00001F32:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	d5,(a0)
	move.l	a2,-(a7)
	jsr.l	fn00001FA4
	addq.w	#$04,a7
	bra	$00001F9E

l00001F4C:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003E9C,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00001F7A

l00001F6A:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00001F9E

l00001F76:
	moveq	#$00,d0
	bra	$00001F9E

l00001F7A:
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

l00001F9E:
	movem.l	(a7)+,d2-d6/a2-a4/a6
	rts

;; fn00001FA4: 00001FA4
;;   Called from:
;;     00001F42 (in fn00001E90)
;;     00002BA6 (in fn00002B98)
;;     00002BC4 (in fn00002B98)
;;     00003DEE (in fn00003DC8)
fn00001FA4 proc
	movem.l	d2-d4/a2/a6,-(a7)
	movea.l	$0018(a7),a2
	jsr.l	fn00002424
	move.l	a2,d0
	bne	$00001FBA

l00001FB6:
	moveq	#-$01,d0
	bra	$00002032

l00001FBA:
	tst.l	$001C(a2)
	bne	$00001FD8

l00001FC0:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00001FD0

l00001FC8:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00001FD8

l00001FD0:
	move.l	#$00000400,$001C(a2)

l00001FD8:
	tst.l	$0008(a2)
	bne	$00001FE2

l00001FDE:
	moveq	#$00,d0
	bra	$00002032

l00001FE2:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002028

l00001FEA:
	tst.l	(a2)
	beq	$00002018

l00001FEE:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003E9C,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$0000201C

l0000200C:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00002032

l00002018:
	moveq	#$00,d0
	bra	$00002032

l0000201C:
	move.l	$0008(a2),$0004(a2)
	move.l	$001C(a2),$0014(a2)

l00002028:
	lea	$0018(a2),a0
	moveq	#-$04,d0
	and.l	d0,(a0)
	moveq	#$00,d0

l00002032:
	movem.l	(a7)+,d2-d4/a2/a6
	rts
00002038                         4A B9 00 00 3E CC 67 0E         J...>.g.
00002040 2F 39 00 00 3E CC 4E B9 00 00 23 B4 58 4F 4E 75 /9..>.N...#.XONu

;; fn00002050: 00002050
;;   Called from:
;;     00001EFE (in fn00001E90)
;;     00003D4E (in fn00003CCC)
fn00002050 proc
	movem.l	d2,-(a7)
	move.l	$0008(a7),d2
	bne	$0000205E

l0000205A:
	moveq	#$00,d0
	bra	$000020B4

l0000205E:
	tst.l	$00003ECC
	bne	$00002082

l00002066:
	movea.l	$00003EC8,a0
	move.l	a0,-(a7)
	move.l	a0,-(a7)
	clr.l	-(a7)
	jsr.l	fn00002344
	move.l	d0,$00003ECC
	lea	$000C(a7),a7

l00002082:
	tst.l	$00003ECC
	bne	$0000208E

l0000208A:
	moveq	#$00,d0
	bra	$000020B4

l0000208E:
	moveq	#$04,d0
	add.l	d2,d0
	move.l	d0,-(a7)
	move.l	$00003ECC,-(a7)
	jsr.l	fn00002220
	movea.l	d0,a1
	addq.w	#$08,a7
	move.l	a1,d0
	bne	$000020AC

l000020A8:
	moveq	#$00,d0
	bra	$000020B4

l000020AC:
	move.l	d2,(a1)
	lea	$0004(a1),a0
	move.l	a0,d0

l000020B4:
	movem.l	(a7)+,d2
	rts
000020BA                               00 00                       ..    

;; fn000020BC: 000020BC
fn000020BC proc
	move.l	$0004(a7),d0
	movea.l	d0,a0
	tst.l	d0
	beq	$000020E6

l000020C6:
	tst.l	$00003ECC
	beq	$000020E6

l000020CE:
	moveq	#$04,d0
	add.l	-(a0),d0
	move.l	d0,-(a7)
	move.l	a0,-(a7)
	move.l	$00003ECC,-(a7)
	jsr.l	fn00002184
	lea	$000C(a7),a7

l000020E6:
	rts
000020E8                         48 E7 30 38 28 6F 00 1C         H.08(o..
000020F0 24 6F 00 18 22 0A 66 0A 2F 0C 61 00 FF 54 58 4F $o..".f./.a..TXO
00002100 60 7A 26 6A FF FC 2F 0C 61 00 FF 46 26 00 58 4F `z&j../.a..F&.XO
00002110 67 68 B9 CB 64 04 20 0C 60 02 20 0B 20 43 22 4A gh..d. .`. . C"J
00002120 24 00 B4 BC 00 00 00 10 65 3C 20 08 22 09 C0 3C $.......e< ."..<
00002130 00 01 C2 3C 00 01 B2 00 66 1A 20 08 4A 01 67 04 ...<....f. .J.g.
00002140 10 D9 53 82 72 03 C2 82 94 81 20 D9 59 82 66 FA ..S.r..... .Y.f.
00002150 34 01 60 14 B4 BC 00 01 00 00 65 0A 20 08 10 D9 4.`.......e. ...
00002160 53 82 66 FA 60 0C 20 08 53 42 65 06 10 D9 51 CA S.f.`. .SBe...Q.
00002170 FF FC 2F 0A 61 00 FF 46 58 4F 20 03 4C DF 1C 0C ../.a..FXO .L...
00002180 4E 75 00 00                                     Nu..            

;; fn00002184: 00002184
;;   Called from:
;;     000020DC (in fn000020BC)
fn00002184 proc
	movem.l	d2/a2-a6,-(a7)
	move.l	$0020(a7),d1
	movea.l	$0024(a7),a5
	movea.l	$001C(a7),a4
	movea.l	$00003E98,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$000021B4

l000021A2:
	movea.l	$00003E98,a6
	movea.l	a4,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$02CA(a6)
	bra	$00002218

l000021B4:
	move.l	a4,d2
	beq	$00002218

l000021B8:
	tst.l	d1
	beq	$00002218

l000021BC:
	movea.l	d1,a3
	lea	-$000C(a3),a3
	cmpa.l	$0014(a4),a5
	bcc	$000021FE

l000021C8:
	movea.l	a4,a2

l000021CA:
	movea.l	(a2),a2
	tst.l	(a2)
	beq	$00002218

l000021D0:
	tst.b	$0008(a2)
	beq	$000021CA

l000021D6:
	cmp.l	$0014(a2),d1
	bcs	$000021CA

l000021DC:
	cmp.l	$0018(a2),d1
	bcc	$000021CA

l000021E2:
	movea.l	$00003E98,a6
	movea.l	a2,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$00C0(a6)
	move.l	$001C(a2),d0
	cmp.l	$0010(a4),d0
	bne	$00002218

l000021FC:
	movea.l	a2,a3

l000021FE:
	movea.l	$00003E98,a6
	movea.l	a3,a1
	jsr.l	-$00FC(a6)
	move.l	-(a3),d0
	movea.l	$00003E98,a6
	movea.l	a3,a1
	jsr.l	-$00D2(a6)

l00002218:
	movem.l	(a7)+,d2/a2-a6
	rts
0000221E                                           00 00               ..

;; fn00002220: 00002220
;;   Called from:
;;     0000209A (in fn00002050)
fn00002220 proc
	movem.l	d2-d4/a2-a6,-(a7)
	move.l	$0028(a7),d2
	movea.l	$0024(a7),a4
	movea.l	$00003E98,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$0000224C

l0000223A:
	movea.l	$00003E98,a6
	movea.l	a4,a0
	move.l	d2,d0
	jsr.l	-$02C4(a6)
	bra	$0000233E

l0000224C:
	suba.l	a3,a3
	move.l	a4,d4
	beq	$0000233C

l00002254:
	tst.l	d2
	beq	$0000233C

l0000225A:
	cmp.l	$0014(a4),d2
	bcc	$0000230E

l00002262:
	movea.l	(a4),a5

l00002264:
	tst.l	(a5)
	beq	$00002286

l00002268:
	tst.b	$0008(a5)
	beq	$00002282

l0000226E:
	movea.l	$00003E98,a6
	movea.l	a5,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3
	move.l	a3,d0
	bne	$000022F2

l00002282:
	movea.l	(a5),a5
	bra	$00002264

l00002286:
	moveq	#$28,d3
	add.l	$0010(a4),d3
	move.l	$000C(a4),d1
	movea.l	$00003E98,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$0000233C

l000022A4:
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
	movea.l	$00003E98,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F0(a6)
	movea.l	$00003E98,a6
	movea.l	a3,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3

l000022F2:
	move.l	#$00010000,d0
	and.l	$000C(a4),d0
	beq	$0000233C

l000022FE:
	movea.l	a3,a2
	addq.l	#$07,d2
	lsr.l	#$03,d2

l00002304:
	clr.l	(a2)+
	clr.l	(a2)+
	subq.l	#$01,d2
	bne	$00002304

l0000230C:
	bra	$0000233C

l0000230E:
	moveq	#$10,d3
	add.l	d2,d3
	move.l	$000C(a4),d1
	movea.l	$00003E98,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$0000233C

l00002328:
	move.l	d3,(a3)+
	movea.l	$00003E98,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F6(a6)
	addq.l	#$08,a3
	clr.l	(a3)+

l0000233C:
	move.l	a3,d0

l0000233E:
	movem.l	(a7)+,d2-d4/a2-a6
	rts

;; fn00002344: 00002344
;;   Called from:
;;     00002072 (in fn00002050)
fn00002344 proc
	movem.l	d2-d3/a2/a6,-(a7)
	move.l	$0018(a7),d3
	movea.l	$001C(a7),a2
	movea.l	$00003E98,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002372

l0000235E:
	movea.l	$00003E98,a6
	move.l	$0014(a7),d0
	move.l	d3,d1
	move.l	a2,d2
	jsr.l	-$02B8(a6)
	bra	$000023AE

l00002372:
	suba.l	a1,a1
	cmp.l	a2,d3
	bcs	$000023AC

l00002378:
	addq.l	#$07,d3
	movea.l	$00003E98,a6
	moveq	#$18,d0
	moveq	#$00,d1
	jsr.l	-$00C6(a6)
	movea.l	d0,a1
	move.l	a1,d0
	beq	$000023AC

l0000238E:
	lea	$0004(a1),a0
	move.l	a0,(a1)
	clr.l	(a0)
	move.l	a1,$0008(a1)
	move.l	$0014(a7),$000C(a1)
	moveq	#-$08,d0
	and.l	d3,d0
	move.l	d0,$0010(a1)
	move.l	a2,$0014(a1)

l000023AC:
	move.l	a1,d0

l000023AE:
	movem.l	(a7)+,d2-d3/a2/a6
	rts

;; fn000023B4: 000023B4
fn000023B4 proc
	movem.l	d2/a2/a6,-(a7)
	move.l	$0010(a7),d2
	movea.l	$00003E98,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$000023D8

l000023CA:
	movea.l	$00003E98,a6
	movea.l	d2,a0
	jsr.l	-$02BE(a6)
	bra	$0000241C

l000023D8:
	tst.l	d2
	beq	$0000241C

l000023DC:
	movea.l	$00003E98,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d1
	beq	$0000240E

l000023EE:
	move.l	-(a2),d0
	movea.l	$00003E98,a6
	movea.l	a2,a1
	jsr.l	-$00D2(a6)
	movea.l	$00003E98,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d0
	bne	$000023EE

l0000240E:
	movea.l	$00003E98,a6
	movea.l	d2,a1
	moveq	#$18,d0
	jsr.l	-$00D2(a6)

l0000241C:
	movem.l	(a7)+,d2/a2/a6
	rts
00002422       00 00                                       ..            

;; fn00002424: 00002424
;;   Called from:
;;     00001E9C (in fn00001E90)
;;     00001FAC (in fn00001FA4)
;;     00003CD4 (in fn00003CCC)
fn00002424 proc
	movem.l	a6,-(a7)
	movea.l	$00003E98,a6
	moveq	#$00,d0
	move.l	#$00001000,d1
	jsr.l	-$0132(a6)
	and.l	#$00001000,d0
	beq	$0000244E

l00002442:
	pea	$00000014
	jsr.l	fn0000131C
	addq.w	#$04,a7

l0000244E:
	movea.l	(a7)+,a6
	rts
00002452       00 00                                       ..            

;; fn00002454: 00002454
;;   Called from:
;;     00001A88 (in fn000014B4)
fn00002454 proc
	movem.l	d2-d6,-(a7)
	move.l	$001C(a7),d1
	move.l	$0018(a7),d0
	movea.l	d1,a0
	move.l	$0024(a7),d3
	move.l	$0020(a7),d2
	bne	$000024AA

l0000246C:
	cmp.l	d3,d0
	bcc	$0000247E

l00002470:
	move.l	d3,d2
	jsr.l	fn00002558
	move.l	d0,d1
	bra	$00002550

l0000247E:
	tst.l	d3
	bne	$0000248A

l00002482:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l0000248A:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	fn00002558
	movea.l	d0,a1
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	fn00002558
	move.l	d0,d1
	move.l	a1,d0
	bra	$00002552

l000024AA:
	cmp.l	d2,d0
	bcc	$000024B4

l000024AE:
	moveq	#$00,d0
	bra	$00002550

l000024B4:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000024D2

l000024BE:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000024D2

l000024C6:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000024D2

l000024CE:
	moveq	#$00,d4
	move.b	d2,d6

l000024D2:
	lea	$00003ED0,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$000024F2

l000024E6:
	cmp.l	d0,d2
	bcs	$000024EE

l000024EA:
	cmp.l	a0,d3
	bhi	$000024AE

l000024EE:
	moveq	#$01,d0
	bra	$00002550

l000024F2:
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
	jsr.l	fn00002558
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
	bcs	$0000254E

l00002548:
	bne	$00002550

l0000254A:
	cmpa.l	d2,a0
	bcc	$00002550

l0000254E:
	subq.l	#$01,d1

l00002550:
	moveq	#$00,d0

l00002552:
	movem.l	(a7)+,d2-d6
	rts

;; fn00002558: 00002558
;;   Called from:
;;     00002472 (in fn00002454)
;;     00002490 (in fn00002454)
;;     0000249C (in fn00002454)
;;     0000250E (in fn00002454)
;;     000027BA (in fn0000279C)
;;     000027D8 (in fn0000279C)
;;     000027E2 (in fn0000279C)
;;     00002850 (in fn0000279C)
fn00002558 proc
	movem.l	d5-d7,-(a7)
	move.l	d2,d7
	beq	$00002572

l00002560:
	move.l	d1,d6
	move.l	d0,d5
	bne	$00002580

l00002566:
	tst.l	d1
	beq	$0000269E

l0000256C:
	cmp.l	d1,d2
	bhi	$0000269E

l00002572:
	move.l	d1,d0
	move.l	d2,d1
	jsr.l	fn00002716
	bra	$0000269E

l00002580:
	swap.l	d2
	tst.w	d2
	bne	$000025A8

l00002586:
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
	bra	$0000269E

l000025A8:
	movem.l	d2-d4/a0-a1,-(a7)
	subq.l	#$08,a7
	clr.b	$0002(a7)
	moveq	#$00,d1
	moveq	#$00,d0
	tst.l	d7
	bmi	$000025C4

l000025BA:
	addq.w	#$01,d0
	add.l	d6,d6
	addx.l	d5,d5
	add.l	d7,d7
	bpl	$000025BA

l000025C4:
	move.w	d0,(a7)

l000025C6:
	move.l	d7,d3
	move.l	d5,d2
	swap.l	d2
	swap.l	d3
	cmp.w	d3,d2
	bne	$000025D8

l000025D2:
	move.w	#$FFFF,d1
	bra	$000025E2

l000025D8:
	move.l	d5,d1
	divu.w	d3,d1
	swap.l	d1
	clr.w	d1
	swap.l	d1

l000025E2:
	movea.l	d6,a1
	clr.w	d6
	swap.l	d6

l000025E8:
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
	bne	$00002608

l00002600:
	cmp.l	d4,d2
	bls	$00002608

l00002604:
	subq.l	#$01,d1
	bra	$000025E8

l00002608:
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
	bcc	$00002660

l0000264A:
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

l00002660:
	tst.b	$0002(a7)
	bne	$0000267C

l00002666:
	move.w	d1,$0004(a7)
	moveq	#$00,d1
	swap.l	d5
	swap.l	d6
	move.w	d6,d5
	clr.w	d6
	st	$0002(a7)
	bra	$000025C6

l0000267C:
	move.l	$0004(a7),d0
	move.w	d1,d0
	move.w	d5,d6
	swap.l	d6
	swap.l	d5
	move.w	(a7),d7
	beq	$00002696

l0000268C:
	subq.w	#$01,d7

l0000268E:
	lsr.l	#$01,d5
	roxr.l	#$01,d6
	dbra	d7,$0000268E

l00002696:
	move.l	d6,d1
	addq.l	#$08,a7
	movem.l	(a7)+,d2-d4/a0-a1

l0000269E:
	movem.l	(a7)+,d5-d7
	rts
000026A4             4C EF 00 03 00 04 4A 81 6B 0A 4A 80     L.....J.k.J.
000026B0 6B 12 61 62 20 01 4E 75 44 81 4A 80 6B 10 61 56 k.ab .NuD.J.k.aV
000026C0 20 01 4E 75 44 80 61 4E 44 81 20 01 4E 75 44 80  .NuD.aND. .NuD.
000026D0 61 44 44 81 20 01 4E 75 4C EF 00 03 00 04 61 36 aDD. .NuL.....a6
000026E0 20 01 4E 75                                      .Nu            

;; fn000026E4: 000026E4
;;   Called from:
;;     00003096 (in fn00002C28)
;;     0000311A (in fn00002C28)
fn000026E4 proc
	movem.l	$0004(a7),d0-d1
	tst.l	d0
	bpl	$00002704

l000026EE:
	neg.l	d0
	tst.l	d1
	bpl	$000026FC

l000026F4:
	neg.l	d1
	bsr	fn00002716
	neg.l	d1
	rts

l000026FC:
	bsr	fn00002716
	neg.l	d0
	neg.l	d1
	rts

l00002704:
	tst.l	d1
	bpl	fn00002716

l00002708:
	neg.l	d1
	bsr	fn00002716
	neg.l	d0
	rts
00002710 4C EF 00 03 00 04                               L.....          

;; fn00002716: 00002716
;;   Called from:
;;     00002576 (in fn00002558)
;;     000026F6 (in fn000026E4)
;;     000026FC (in fn000026E4)
;;     00002706 (in fn000026E4)
;;     0000270A (in fn000026E4)
fn00002716 proc
	move.l	d2,-(a7)
	swap.l	d1
	move.w	d1,d2
	bne	$0000273C

l0000271E:
	swap.l	d0
	swap.l	d1
	swap.l	d2
	move.w	d0,d2
	beq	$0000272C

l00002728:
	divu.w	d1,d2
	move.w	d2,d0

l0000272C:
	swap.l	d0
	move.w	d0,d2
	divu.w	d1,d2
	move.w	d2,d0
	swap.l	d2
	move.w	d2,d1
	move.l	(a7)+,d2
	rts

l0000273C:
	move.l	d3,-(a7)
	moveq	#$10,d3
	cmp.w	#$0080,d1
	bcc	$0000274A

l00002746:
	rol.l	#$08,d1
	subq.w	#$08,d3

l0000274A:
	cmp.w	#$0800,d1
	bcc	$00002754

l00002750:
	rol.l	#$04,d1
	subq.w	#$04,d3

l00002754:
	cmp.w	#$2000,d1
	bcc	$0000275E

l0000275A:
	rol.l	#$02,d1
	subq.w	#$02,d3

l0000275E:
	tst.w	d1
	bmi	$00002766

l00002762:
	rol.l	#$01,d1
	subq.w	#$01,d3

l00002766:
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
	bcc	$00002788

l00002782:
	subq.w	#$01,d3
	add.l	d1,d0

l00002786:
	bcc	$00002786

l00002788:
	moveq	#$00,d1
	move.w	d3,d1
	swap.l	d3
	rol.l	d3,d0
	swap.l	d0
	exg	d0,d1
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	rts
0000279A                               00 00                       ..    

;; fn0000279C: 0000279C
;;   Called from:
;;     00001A5C (in fn000014B4)
fn0000279C proc
	movem.l	d2-d7,-(a7)
	move.l	$0020(a7),d1
	move.l	$001C(a7),d0
	movea.l	d1,a0
	move.l	$0028(a7),d3
	move.l	$0024(a7),d2
	bne	$000027EE

l000027B4:
	cmp.l	d3,d0
	bcc	$000027C6

l000027B8:
	move.l	d3,d2
	jsr.l	fn00002558
	moveq	#$00,d0
	bra	$000028A8

l000027C6:
	tst.l	d3
	bne	$000027D2

l000027CA:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l000027D2:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	fn00002558
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	fn00002558
	moveq	#$00,d0
	bra	$000028A8

l000027EE:
	cmp.l	d2,d0
	bcs	$000028A8

l000027F4:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002812

l000027FE:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002812

l00002806:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002812

l0000280E:
	moveq	#$00,d4
	move.b	d2,d6

l00002812:
	lea	$00003ED0,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$00002834

l00002826:
	cmp.l	d0,d2
	bcs	$0000282E

l0000282A:
	cmp.l	d1,d3
	bhi	$000028A8

l0000282E:
	sub.l	d3,d1
	subx.l	d2,d0
	bra	$000028A8

l00002834:
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
	jsr.l	fn00002558
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
	bcs	$00002890

l0000288A:
	bne	$00002894

l0000288C:
	cmpa.l	d3,a0
	bcc	$00002894

l00002890:
	sub.l	a1,d3
	subx.l	d0,d2

l00002894:
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

l000028A8:
	movem.l	(a7)+,d2-d7
	rts
000028AE                                           00 00               ..
000028B0 00 20 20 20 20 20 20 20 20 20 28 28 28 28 28 20 .         ((((( 
000028C0 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20                 
000028D0 20 88 10 10 10 10 10 10 10 10 10 10 10 10 10 10  ...............
000028E0 10 04 04 04 04 04 04 04 04 04 04 10 10 10 10 10 ................
000028F0 10 10 41 41 41 41 41 41 01 01 01 01 01 01 01 01 ..AAAAAA........
00002900 01 01 01 01 01 01 01 01 01 01 01 01 10 10 10 10 ................
00002910 10 10 42 42 42 42 42 42 02 02 02 02 02 02 02 02 ..BBBBBB........
00002920 02 02 02 02 02 02 02 02 02 02 02 02 10 10 10 10 ................
00002930 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ...............
00002940 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
000029B0 00 00 00 00 48 E7 00 32 48 78 00 20 4E B9 00 00 ....H..2Hx. N...
000029C0 20 50 23 C0 00 00 3F F0 48 78 00 20 4E B9 00 00  P#...?.Hx. N...
000029D0 20 50 23 C0 00 00 3F F4 48 78 00 20 4E B9 00 00  P#...?.Hx. N...
000029E0 20 50 23 C0 00 00 3F F8 4F EF 00 0C 4A B9 00 00  P#...?.O...J...
000029F0 3F F0 67 10 4A B9 00 00 3F F4 67 08 4A B9 00 00 ?.g.J...?.g.J...
00002A00 3F F8 66 0C 48 78 00 14 4E B9 00 00 13 1C 58 4F ?.f.Hx..N.....XO
00002A10 20 79 00 00 3F F0 20 B9 00 00 3E A8 20 79 00 00  y..?. ...>. y..
00002A20 3F F0 70 20 21 40 00 18 22 39 00 00 3E A8 2C 79 ?.p !@.."9..>.,y
00002A30 00 00 3E 9C 4E AE FF 28 4A 80 67 10 20 79 00 00 ..>.N..(J.g. y..
00002A40 3F F0 41 E8 00 18 00 90 00 00 02 04 20 79 00 00 ?.A......... y..
00002A50 3F F4 20 B9 00 00 3E AC 20 79 00 00 3F F4 70 40 ?. ...>. y..?.p@
00002A60 21 40 00 18 22 39 00 00 3E AC 2C 79 00 00 3E 9C !@.."9..>.,y..>.
00002A70 4E AE FF 28 4A 80 67 10 20 79 00 00 3F F4 41 E8 N..(J.g. y..?.A.
00002A80 00 18 00 90 00 00 02 80 20 79 00 00 3F F8 20 B9 ........ y..?. .
00002A90 00 00 3E B0 20 79 00 00 3F F8 70 40 21 40 00 18 ..>. y..?.p@!@..
00002AA0 22 39 00 00 3E B0 2C 79 00 00 3E 9C 4E AE FF 28 "9..>.,y..>.N..(
00002AB0 4A 80 67 10 20 79 00 00 3F F8 41 E8 00 18 00 90 J.g. y..?.A.....
00002AC0 00 00 02 80 20 79 00 00 3F F8 42 A8 00 04 20 79 .... y..?.B... y
00002AD0 00 00 3F F4 42 A8 00 04 20 79 00 00 3F F0 42 A8 ..?.B... y..?.B.
00002AE0 00 04 20 79 00 00 3F F8 42 A8 00 08 20 79 00 00 .. y..?.B... y..
00002AF0 3F F4 42 A8 00 08 20 79 00 00 3F F0 42 A8 00 08 ?.B... y..?.B...
00002B00 24 79 00 00 3F F8 42 AA 00 14 22 79 00 00 3F F4 $y..?.B..."y..?.
00002B10 42 A9 00 14 20 79 00 00 3F F0 42 A8 00 14 42 AA B... y..?.B...B.
00002B20 00 1C 42 A9 00 1C 42 A8 00 1C 42 A8 00 10 20 79 ..B...B...B... y
00002B30 00 00 3F F0 21 79 00 00 3F F4 00 0C 20 79 00 00 ..?.!y..?... y..
00002B40 3F F4 21 79 00 00 3F F0 00 10 20 79 00 00 3F F4 ?.!y..?... y..?.
00002B50 21 79 00 00 3F F8 00 0C 20 79 00 00 3F F8 21 79 !y..?... y..?.!y
00002B60 00 00 3F F4 00 10 20 79 00 00 3F F8 42 A8 00 0C ..?... y..?.B...
00002B70 23 F9 00 00 3F F0 00 00 3F FC 23 F9 00 00 3F F8 #...?...?.#...?.
00002B80 00 00 40 00 4C DF 4C 00 4E 75 00 00 42 A7 4E B9 ..@.L.L.Nu..B.N.
00002B90 00 00 2B 98 58 4F 4E 75                         ..+.XONu        

;; fn00002B98: 00002B98
fn00002B98 proc
	movem.l	a2,-(a7)
	movea.l	$0008(a7),a2
	move.l	a2,d0
	beq	$00002BB0

l00002BA4:
	move.l	a2,-(a7)
	jsr.l	fn00001FA4
	addq.w	#$04,a7
	bra	$00002BD6

l00002BB0:
	movea.l	$00003FFC,a2
	move.l	a2,d0
	beq	$00002BD6

l00002BBA:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002BCC

l00002BC2:
	move.l	a2,-(a7)
	jsr.l	fn00001FA4
	addq.w	#$04,a7

l00002BCC:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00002BBA

l00002BD6:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	rts

;; fn00002BDC: 00002BDC
;;   Called from:
;;     000013AC (in fn00001390)
;;     000013CE (in fn00001390)
fn00002BDC proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00003FF0,-(a7)
	jsr.l	fn00002C28
	lea	$000C(a7),a7
	rts

;; fn00002BF8: 00002BF8
;;   Called from:
;;     00002F32 (in fn00002C28)
;;     0000314E (in fn00002C28)
;;     00003234 (in fn00002C28)
;;     000032B6 (in fn00002C28)
;;     000034F6 (in fn00002C28)
;;     00003514 (in fn00002C28)
;;     0000366E (in fn00002C28)
;;     00003688 (in fn00002C28)
;;     00003954 (in fn00002C28)
;;     0000396E (in fn00002C28)
;;     00003BA8 (in fn00002C28)
;;     00003C14 (in fn00002C28)
fn00002BF8 proc
	movem.l	a2,-(a7)
	movea.l	$000C(a7),a2
	move.l	a2,d0
	beq	$00002C22

l00002C04:
	move.l	$0004(a2),d0
	cmp.l	$0008(a2),d0
	bcc	$00002C16

l00002C0E:
	movea.l	$0004(a2),a0
	move.b	$000B(a7),(a0)

l00002C16:
	lea	$0014(a2),a0
	addq.l	#$01,(a0)
	lea	$0004(a2),a0
	subq.l	#$01,(a0)

l00002C22:
	movea.l	(a7)+,a2
	rts
00002C26                   00 00                               ..        

;; fn00002C28: 00002C28
;;   Called from:
;;     00002BEC (in fn00002BDC)
fn00002C28 proc
	lea	-$004C(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$0084(a7),d2
	movea.l	$0080(a7),a4
	movea.l	$007C(a7),a2
	clr.l	$003C(a7)
	moveq	#$00,d4
	moveq	#$00,d5
	tst.b	(a4)
	beq	$00003C2A

l00002C4A:
	moveq	#$00,d3
	cmpi.b	#$25,(a4)
	bne	$00003B28

l00002C54:
	moveq	#-$01,d6
	move.b	#$69,$0048(a7)
	clr.b	$0049(a7)
	lea	$0001(a4),a3
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00002CD8

l00002C7C:
	moveq	#$00,d6
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00002CD8

l00002C96:
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
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00002C96

l00002CD8:
	cmpi.b	#$68,(a3)
	beq	$00002D02

l00002CDE:
	cmpi.b	#$6C,(a3)
	beq	$00002D02

l00002CE4:
	cmpi.b	#$4C,(a3)
	beq	$00002D02

l00002CEA:
	cmpi.b	#$7A,(a3)
	beq	$00002D02

l00002CF0:
	cmpi.b	#$6A,(a3)
	beq	$00002D02

l00002CF6:
	cmpi.b	#$74,(a3)
	beq	$00002D02

l00002CFC:
	cmpi.b	#$2A,(a3)
	bne	$00002D6A

l00002D02:
	move.b	$0049(a7),d7
	move.b	$0048(a7),d1

l00002D0A:
	cmpi.b	#$2A,(a3)
	bne	$00002D14

l00002D10:
	moveq	#$01,d7
	bra	$00002D36

l00002D14:
	cmp.b	#$68,d1
	bne	$00002D24

l00002D1A:
	cmpi.b	#$68,(a3)
	bne	$00002D24

l00002D20:
	moveq	#$02,d1
	bra	$00002D36

l00002D24:
	cmp.b	#$6C,d1
	bne	$00002D34

l00002D2A:
	cmpi.b	#$6C,(a3)
	bne	$00002D34

l00002D30:
	moveq	#$01,d1
	bra	$00002D36

l00002D34:
	move.b	(a3),d1

l00002D36:
	addq.l	#$01,a3
	cmpi.b	#$68,(a3)
	beq	$00002D0A

l00002D3E:
	cmpi.b	#$6C,(a3)
	beq	$00002D0A

l00002D44:
	cmpi.b	#$4C,(a3)
	beq	$00002D0A

l00002D4A:
	cmpi.b	#$7A,(a3)
	beq	$00002D0A

l00002D50:
	cmpi.b	#$6A,(a3)
	beq	$00002D0A

l00002D56:
	cmpi.b	#$74,(a3)
	beq	$00002D0A

l00002D5C:
	cmpi.b	#$2A,(a3)
	beq	$00002D0A

l00002D62:
	move.b	d1,$0048(a7)
	move.b	d7,$0049(a7)

l00002D6A:
	cmpi.b	#$6A,$0048(a7)
	bne	$00002D78

l00002D72:
	move.b	#$01,$0048(a7)

l00002D78:
	cmpi.b	#$74,$0048(a7)
	bne	$00002D86

l00002D80:
	move.b	#$69,$0048(a7)

l00002D86:
	cmpi.b	#$7A,$0048(a7)
	bne	$00002D94

l00002D8E:
	move.b	#$6C,$0048(a7)

l00002D94:
	move.b	(a3)+,d7
	beq	$00002E0A

l00002D98:
	cmp.b	#$25,d7
	beq	$00002E0A

l00002D9E:
	cmp.b	#$63,d7
	beq	$00002E0A

l00002DA4:
	cmp.b	#$6E,d7
	beq	$00002E0A

l00002DAA:
	cmp.b	#$5B,d7
	beq	$00002E0A

l00002DB0:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00002DDA

l00002DC2:
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
	bra	$00002DE6

l00002DDA:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,d1
	addq.w	#$04,a7

l00002DE6:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00002DB0

l00002E08:
	moveq	#$01,d3

l00002E0A:
	move.b	d7,d1
	sub.b	#$25,d1
	beq	$00003256

l00002E14:
	sub.b	#$36,d1
	beq	$00002F52

l00002E1C:
	subq.b	#$08,d1
	beq	$00002E32

l00002E20:
	sub.b	#$0B,d1
	beq	$000032C4

l00002E28:
	subq.b	#$05,d1
	beq	$00003170

l00002E2E:
	bra	$00003366

l00002E32:
	cmp.l	#$FFFFFFFF,d6
	bne	$00002E3C

l00002E3A:
	moveq	#$01,d6

l00002E3C:
	tst.b	$0049(a7)
	bne	$00002E56

l00002E42:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a1
	bra	$00002E58

l00002E56:
	suba.l	a1,a1

l00002E58:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	move.l	a1,$002C(a7)
	tst.l	(a0)
	blt	$00002E90

l00002E70:
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
	bra	$00002EAE

l00002E90:
	movea.l	$002C(a7),a1
	move.l	a2,-(a7)
	move.l	a1,$0030(a7)
	jsr.l	fn00003CCC
	move.l	d0,$0038(a7)
	movea.l	$0030(a7),a1
	move.l	a1,$0030(a7)
	addq.w	#$04,a7

l00002EAE:
	movea.l	$002C(a7),a1
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$0034(a7)
	beq	$00002F26

l00002EC4:
	move.l	a1,$002C(a7)
	cmp.l	d3,d6
	bcs	$00002F26

l00002ECC:
	move.b	$0049(a7),d7
	movea.l	$002C(a7),a4

l00002ED4:
	tst.b	d7
	bne	$00002EDA

l00002ED8:
	move.b	d5,(a4)+

l00002EDA:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00002F04

l00002EEC:
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
	bra	$00002F10

l00002F04:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,d1
	addq.w	#$04,a7

l00002F10:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00002F22

l00002F1E:
	cmp.l	d3,d6
	bcc	$00002ED4

l00002F22:
	move.b	d7,$0049(a7)

l00002F26:
	cmp.l	#$FFFFFFFF,d5
	beq	$00002F38

l00002F2E:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l00002F38:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003B22

l00002F44:
	tst.l	d3
	beq	$00003B22

l00002F4A:
	addq.l	#$01,$003C(a7)
	bra	$00003B22

l00002F52:
	clr.b	$002C(a7)
	cmpi.b	#$5E,(a3)
	bne	$00002F64

l00002F5C:
	move.b	#$01,$002C(a7)
	addq.l	#$01,a3

l00002F64:
	clr.l	$0034(a7)
	move.b	$002C(a7),d7
	move.l	$0034(a7),d1

l00002F70:
	tst.b	d7
	beq	$00002F7C

l00002F74:
	move.l	#$000000FF,d5
	bra	$00002F7E

l00002F7C:
	moveq	#$00,d5

l00002F7E:
	lea	$004E(a7),a0
	move.b	d5,(a0,d1)
	addq.l	#$01,d1
	cmp.l	#$00000020,d1
	bcs	$00002F70

l00002F90:
	move.l	d2,$0084(a7)
	move.b	d7,$002C(a7)
	move.b	$002C(a7),d2

l00002F9C:
	tst.b	(a3)
	beq	$00003012

l00002FA0:
	move.b	(a3)+,d1
	cmpi.b	#$2D,(a3)
	bne	$00002FB4

l00002FA8:
	cmp.b	$0001(a3),d1
	bcc	$00002FB4

l00002FAE:
	addq.l	#$01,a3
	move.b	(a3)+,d7
	bra	$00002FB6

l00002FB4:
	move.b	d1,d7

l00002FB6:
	moveq	#$00,d5
	move.b	d1,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcs	$0000300C

l00002FC2:
	tst.b	d2
	beq	$00002FE6

l00002FC6:
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
	bra	$00003002

l00002FE6:
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

l00003002:
	addq.l	#$01,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcc	$00002FC2

l0000300C:
	cmpi.b	#$5D,(a3)
	bne	$00002F9C

l00003012:
	move.l	$0084(a7),d2
	addq.l	#$01,a3
	tst.b	$0049(a7)
	bne	$00003032

l0000301E:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a6
	bra	$00003034

l00003032:
	suba.l	a6,a6

l00003034:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003066

l00003046:
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
	bra	$00003074

l00003066:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00003074:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$002C(a7)
	beq	$00003142

l00003088:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	fn000026E4
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
	beq	$00003142

l000030B8:
	cmp.l	d3,d6
	bcs	$00003142

l000030BE:
	move.b	$0049(a7),d7

l000030C2:
	tst.b	d7
	bne	$000030C8

l000030C6:
	move.b	d5,(a6)+

l000030C8:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000030F2

l000030DA:
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
	bra	$000030FE

l000030F2:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,d1
	addq.w	#$04,a7

l000030FE:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$0000313E

l0000310C:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	fn000026E4
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
	beq	$0000313E

l0000313A:
	cmp.l	d3,d6
	bcc	$000030C2

l0000313E:
	move.b	d7,$0049(a7)

l00003142:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003154

l0000314A:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l00003154:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003B22

l00003160:
	tst.l	d3
	beq	$00003B22

l00003166:
	clr.b	(a6)+
	addq.l	#$01,$003C(a7)
	bra	$00003B22

l00003170:
	tst.b	$0049(a7)
	bne	$0000318A

l00003176:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a5
	bra	$0000318C

l0000318A:
	suba.l	a5,a5

l0000318C:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003228

l00003196:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003228

l000031B2:
	cmp.l	d3,d6
	bcs	$00003228

l000031B6:
	move.b	$0049(a7),d7

l000031BA:
	tst.b	d7
	bne	$000031C0

l000031BE:
	move.b	d5,(a5)+

l000031C0:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000031EA

l000031D2:
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
	bra	$000031F6

l000031EA:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,d1
	addq.w	#$04,a7

l000031F6:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00003224

l00003204:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003224

l00003220:
	cmp.l	d3,d6
	bcc	$000031BA

l00003224:
	move.b	d7,$0049(a7)

l00003228:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000323A

l00003230:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l0000323A:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003B22

l00003246:
	tst.l	d3
	beq	$00003B22

l0000324C:
	clr.b	(a5)+
	addq.l	#$01,$003C(a7)
	bra	$00003B22

l00003256:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003288

l00003268:
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
	bra	$00003296

l00003288:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00003296:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$00000025,$002C(a7)
	beq	$00003B22

l000032AA:
	cmp.l	#$FFFFFFFF,d5
	beq	$000032BC

l000032B2:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l000032BC:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00003B22

l000032C4:
	tst.b	$0049(a7)
	bne	$0000335C

l000032CC:
	cmpi.b	#$01,$0048(a7)
	bne	$000032EE

l000032D4:
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
	bra	$0000335C

l000032EE:
	cmpi.b	#$6C,$0048(a7)
	bne	$0000330C

l000032F6:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)
	bra	$0000335C

l0000330C:
	cmpi.b	#$68,$0048(a7)
	bne	$0000332A

l00003314:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.w	d4,(a0)
	bra	$0000335C

l0000332A:
	cmpi.b	#$02,$0048(a7)
	bne	$00003348

l00003332:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.b	d4,(a0)
	bra	$0000335C

l00003348:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)

l0000335C:
	moveq	#$01,d3
	addq.l	#$01,$003C(a7)
	bra	$00003B22

l00003366:
	clr.l	$0030(a7)
	clr.l	$002C(a7)
	clr.l	$006E(a7)
	tst.b	d7
	bne	$00003378

l00003376:
	subq.l	#$01,a3

l00003378:
	cmp.b	#$70,d7
	bne	$00003386

l0000337E:
	move.b	#$6C,$0048(a7)
	moveq	#$78,d7

l00003386:
	cmp.l	#$0000002D,d5
	bne	$00003394

l0000338E:
	cmp.b	#$75,d7
	bne	$0000339C

l00003394:
	cmp.l	#$0000002B,d5
	bne	$000033EC

l0000339C:
	cmp.l	d3,d6
	bcs	$000033EC

l000033A0:
	move.l	d5,$006E(a7)
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000033D6

l000033B6:
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
	bra	$000033E4

l000033D6:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l000033E4:
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4

l000033EC:
	cmp.b	#$69,d7
	bne	$0000355E

l000033F4:
	cmp.l	#$00000030,d5
	bne	$00003520

l000033FE:
	cmp.l	d3,d6
	bcs	$00003520

l00003404:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003436

l00003416:
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
	bra	$00003444

l00003436:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003444:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$000028B0,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$0000346A

l00003466:
	or.b	#$20,d0

l0000346A:
	cmp.l	#$00000078,d0
	bne	$00003502

l00003474:
	cmp.l	d3,d6
	bcs	$00003502

l0000347A:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000034AC

l0000348C:
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
	bra	$000034BA

l000034AC:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l000034BA:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$000034E6

l000034E0:
	cmp.l	d3,d6
	bcs	$000034E6

l000034E4:
	moveq	#$78,d7

l000034E6:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$000034FC

l000034F0:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l000034FC:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00003504

l00003502:
	moveq	#$6F,d7

l00003504:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$0000351A

l0000350E:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l0000351A:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$0000355E

l00003520:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$0000355E

l0000353C:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$0000355E

l00003558:
	cmp.l	d3,d6
	bcs	$0000355E

l0000355C:
	moveq	#$78,d7

l0000355E:
	cmp.b	#$78,d7
	bne	$00003692

l00003566:
	cmp.l	#$00000030,d5
	bne	$00003692

l00003570:
	cmp.l	d3,d6
	bcs	$00003692

l00003576:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000035A8

l00003588:
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
	bra	$000035B6

l000035A8:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l000035B6:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$000028B0,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$000035DC

l000035D8:
	or.b	#$20,d0

l000035DC:
	cmp.l	#$00000078,d0
	bne	$00003678

l000035E6:
	cmp.l	d3,d6
	bcs	$00003678

l000035EC:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000361E

l000035FE:
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
	bra	$0000362C

l0000361E:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l0000362C:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$0000365E

l00003654:
	cmp.l	d3,d6
	bcs	$0000365E

l00003658:
	move.l	$004A(a7),d5
	bra	$00003692

l0000365E:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$00003674

l00003668:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l00003674:
	subq.l	#$01,d3
	subq.l	#$01,d4

l00003678:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$0000368E

l00003682:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l0000368E:
	subq.l	#$01,d3
	subq.l	#$01,d4

l00003692:
	cmp.b	#$78,d7
	beq	$0000369E

l00003698:
	cmp.b	#$58,d7
	bne	$000036A8

l0000369E:
	move.l	#$00000010,$0040(a7)
	bra	$000036C6

l000036A8:
	cmp.b	#$6F,d7
	bne	$000036B8

l000036AE:
	move.l	#$00000008,$0034(a7)
	bra	$000036C0

l000036B8:
	move.l	#$0000000A,$0034(a7)

l000036C0:
	move.l	$0034(a7),$0040(a7)

l000036C6:
	move.l	$0040(a7),$0072(a7)
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$0000392E

l000036F8:
	cmpi.l	#$0000000A,$0072(a7)
	bne	$0000372E

l00003702:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$0000392E

l0000372E:
	cmpi.l	#$00000008,$0072(a7)
	bne	$0000374E

l00003738:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	cmp.l	#$00000037,d5
	bgt	$0000392E

l0000374E:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.l	d6,$0040(a7)
	move.b	d7,$004A(a7)
	cmp.l	d3,d6
	bcs	$0000392E

l00003764:
	move.l	$0072(a7),d7
	movea.l	$0040(a7),a4

l0000376C:
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
	jsr.l	fn00003C4C
	lea	$0010(a7),a7
	movea.l	(a7)+,a1
	move.l	d0,$0048(a7)
	move.l	d1,$004C(a7)
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$000037CC

l000037C2:
	move.l	d5,d4
	sub.l	#$00000030,d4
	bra	$000037CE

l000037CC:
	moveq	#$00,d4

l000037CE:
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
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000001,d0
	beq	$0000381E

l00003814:
	move.l	d5,d6
	sub.l	#$00000037,d6
	bra	$00003820

l0000381E:
	moveq	#$00,d6

l00003820:
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
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000002,d0
	beq	$00003870

l00003866:
	move.l	d5,d2
	sub.l	#$00000057,d2
	bra	$00003872

l00003870:
	moveq	#$00,d2

l00003872:
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
	blt	$000038C4

l000038AC:
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
	bra	$000038D0

l000038C4:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,d1
	addq.w	#$04,a7

l000038D0:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,$0034(a7)
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$0000392E

l000038F4:
	cmp.l	#$0000000A,d7
	bne	$00003918

l000038FC:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$0000392E

l00003918:
	cmp.l	#$00000008,d7
	bne	$00003928

l00003920:
	cmp.l	#$00000037,d5
	bgt	$0000392E

l00003928:
	cmpa.l	d3,a4
	bcc	$0000376C

l0000392E:
	move.b	$004A(a7),d7
	move.l	$0034(a7),d4
	move.l	$0084(a7),d2
	tst.l	$006E(a7)
	beq	$00003962

l00003940:
	cmp.l	#$00000002,d3
	bne	$00003962

l00003948:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000395A

l00003950:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l0000395A:
	subq.l	#$01,d3
	subq.l	#$01,d4
	move.l	$006E(a7),d5

l00003962:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003974

l0000396A:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l00003974:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003B22

l00003980:
	tst.l	d3
	beq	$00003B22

l00003986:
	cmp.b	#$75,d7
	bne	$00003A48

l0000398E:
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0038(a7)
	move.l	(a7)+,d0
	tst.b	$0034(a7)
	beq	$000039B8

l000039A2:
	subq.b	#$01,$0034(a7)
	beq	$00003A10

l000039A8:
	subi.b	#$66,$0034(a7)
	beq	$000039F4

l000039B0:
	subq.b	#$04,$0034(a7)
	beq	$000039D8

l000039B6:
	bra	$00003A2C

l000039B8:
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
	bra	$00003B1E

l000039D8:
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
	bra	$00003B1E

l000039F4:
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
	bra	$00003B1E

l00003A10:
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
	bra	$00003B1E

l00003A2C:
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
	bra	$00003B1E

l00003A48:
	cmpi.l	#$0000002D,$006E(a7)
	bne	$00003A64

l00003A52:
	movem.l	$002C(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0034(a7)
	bra	$00003A70

l00003A64:
	move.l	$0030(a7),$0038(a7)
	move.l	$002C(a7),$0034(a7)
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0030(a7)
	move.l	(a7)+,d0
	tst.b	$002C(a7)
	beq	$00003A9A

l00003A84:
	subq.b	#$01,$002C(a7)
	beq	$00003AEC

l00003A8A:
	subi.b	#$66,$002C(a7)
	beq	$00003AD2

l00003A92:
	subq.b	#$04,$002C(a7)
	beq	$00003AB8

l00003A98:
	bra	$00003B06

l00003A9A:
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
	bra	$00003B1E

l00003AB8:
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
	bra	$00003B1E

l00003AD2:
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
	bra	$00003B1E

l00003AEC:
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
	bra	$00003B1E

l00003B06:
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

l00003B1E:
	addq.l	#$01,$003C(a7)

l00003B22:
	movea.l	a3,a4
	bra	$00003C20

l00003B28:
	move.b	(a4),d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	beq	$00003BB4

l00003B44:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003B6E

l00003B56:
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
	bra	$00003B7A

l00003B6E:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,d1
	addq.w	#$04,a7

l00003B7A:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$000028B1,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003B44

l00003B9C:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003BAE

l00003BA4:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l00003BAE:
	subq.l	#$01,d4
	moveq	#$01,d3
	bra	$00003C1E

l00003BB4:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003BE6

l00003BC6:
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
	bra	$00003BF4

l00003BE6:
	move.l	a2,-(a7)
	jsr.l	fn00003CCC
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00003BF4:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	(a4),d0
	ext.w	d0
	ext.l	d0
	cmp.l	$002C(a7),d0
	beq	$00003C1E

l00003C08:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003C1A

l00003C10:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002BF8
	addq.w	#$08,a7

l00003C1A:
	subq.l	#$01,d3
	subq.l	#$01,d4

l00003C1E:
	addq.l	#$01,a4

l00003C20:
	tst.l	d3
	beq	$00003C2A

l00003C24:
	tst.b	(a4)
	bne	$00002C4A

l00003C2A:
	cmp.l	#$FFFFFFFF,d5
	bne	$00003C3C

l00003C32:
	tst.l	$003C(a7)
	bne	$00003C3C

l00003C38:
	move.l	d5,d0
	bra	$00003C40

l00003C3C:
	move.l	$003C(a7),d0

l00003C40:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$004C(a7),a7
	rts
00003C4A                               00 00                       ..    

;; fn00003C4C: 00003C4C
;;   Called from:
;;     0000378A (in fn00002C28)
fn00003C4C proc
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
	bcc	$00003C80

l00003C7A:
	add.l	#$00010000,d1

l00003C80:
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
00003CCA                               00 00                       ..    

;; fn00003CCC: 00003CCC
;;   Called from:
;;     00002DDC (in fn00002C28)
;;     00002E9A (in fn00002C28)
;;     00002F06 (in fn00002C28)
;;     00003068 (in fn00002C28)
;;     000030F4 (in fn00002C28)
;;     000031EC (in fn00002C28)
;;     0000328A (in fn00002C28)
;;     000033D8 (in fn00002C28)
;;     00003438 (in fn00002C28)
;;     000034AE (in fn00002C28)
;;     000035AA (in fn00002C28)
;;     00003620 (in fn00002C28)
;;     000038C6 (in fn00002C28)
;;     00003B70 (in fn00002C28)
;;     00003BE8 (in fn00002C28)
fn00003CCC proc
	movem.l	d2-d5/a2-a4/a6,-(a7)
	movea.l	$0024(a7),a2
	jsr.l	fn00002424
	move.l	a2,d0
	bne	$00003CE4

l00003CDE:
	moveq	#-$01,d0
	bra	$00003DC2

l00003CE4:
	moveq	#$2A,d0
	and.l	$0018(a2),d0
	moveq	#$20,d5
	cmp.l	d0,d5
	beq	$00003CF6

l00003CF0:
	moveq	#-$01,d0
	bra	$00003DC2

l00003CF6:
	lea	$0018(a2),a0
	moveq	#$01,d0
	or.l	d0,(a0)
	move.l	#$00000200,d0
	and.l	(a0),d0
	beq	$00003D0E

l00003D08:
	jsr.l	fn00003DC8

l00003D0E:
	tst.l	$001C(a2)
	bne	$00003D2C

l00003D14:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00003D24

l00003D1C:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00003D2C

l00003D24:
	move.l	#$00000400,$001C(a2)

l00003D2C:
	tst.l	$0008(a2)
	bne	$00003D68

l00003D32:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00003D46

l00003D42:
	moveq	#$02,d4
	bra	$00003D48

l00003D46:
	moveq	#$01,d4

l00003D48:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	fn00002050
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00003D60

l00003D5C:
	moveq	#-$01,d0
	bra	$00003DC2

l00003D60:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)

l00003D68:
	lea	$0004(a2),a0
	move.l	$0008(a2),(a0)
	move.l	$001C(a2),d3
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003E9C,a6
	jsr.l	-$002A(a6)
	lea	$0014(a2),a0
	move.l	d0,(a0)
	subq.l	#$01,(a0)
	bge	$00003DAE

l00003D8C:
	moveq	#-$01,d0
	cmp.l	$0014(a2),d0
	bne	$00003D9E

l00003D94:
	lea	$0018(a2),a0
	moveq	#$08,d0
	or.l	d0,(a0)
	bra	$00003DA6

l00003D9E:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)

l00003DA6:
	clr.l	$0014(a2)
	moveq	#-$01,d0
	bra	$00003DC2

l00003DAE:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	(a0),d0
	and.l	#$000000FF,d0

l00003DC2:
	movem.l	(a7)+,d2-d5/a2-a4/a6
	rts

;; fn00003DC8: 00003DC8
;;   Called from:
;;     00003D08 (in fn00003CCC)
fn00003DC8 proc
	movem.l	a2,-(a7)
	movea.l	$00003FFC,a2
	move.l	a2,d0
	beq	$00003E00

l00003DD6:
	move.l	#$00000202,d0
	and.l	$0018(a2),d0
	cmp.l	#$00000202,d0
	bne	$00003DF6

l00003DE8:
	tst.l	(a2)
	beq	$00003DF6

l00003DEC:
	move.l	a2,-(a7)
	jsr.l	fn00001FA4
	addq.w	#$04,a7

l00003DF6:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00003DD6

l00003E00:
	movea.l	(a7)+,a2
	rts

;; fn00003E04: 00003E04
;;   Called from:
;;     0000139C (in fn00001390)
;;     000013BE (in fn00001390)
fn00003E04 proc
	movem.l	d2/a2-a3,-(a7)
	movea.l	$0010(a7),a2
	moveq	#$00,d2
	tst.b	(a2)
	beq	$00003E84

l00003E12:
	movea.l	$00003FF4,a0
	lea	$0018(a0),a1
	moveq	#$02,d0
	or.l	d0,(a1)
	lea	$0014(a0),a0
	subq.l	#$01,(a0)
	blt	$00003E5A

l00003E28:
	cmpi.b	#$0A,(a2)
	bne	$00003E40

l00003E2E:
	movea.l	$00003FF4,a0
	move.l	#$00000080,d0
	and.l	$0018(a0),d0
	bne	$00003E5A

l00003E40:
	movea.l	$00003FF4,a1
	addq.l	#$04,a1
	movea.l	(a1),a0
	movea.l	a0,a3
	addq.l	#$01,a3
	move.l	a3,(a1)
	move.b	(a2),(a0)
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00003E72

l00003E5A:
	move.l	$00003FF4,-(a7)
	move.b	(a2),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00001E90
	move.l	d0,d1
	addq.w	#$08,a7

l00003E72:
	moveq	#-$01,d0
	cmp.l	d1,d0
	bne	$00003E7C

l00003E78:
	moveq	#-$01,d0
	bra	$00003E86

l00003E7C:
	addq.l	#$01,a2
	addq.l	#$01,d2
	tst.b	(a2)
	bne	$00003E12

l00003E84:
	move.l	d2,d0

l00003E86:
	movem.l	(a7)+,d2/a2-a3
	rts
00003E8C                                     00 24 00 63             .$.c
00003E90 41 00 00 00 00 00 00 00 00 00 40 00 00 00 00 00 A.........@.....
00003EA0 00 01 02 02 03 03 03 03 04 04 04 04 04 04 04 04 ................
00003EB0 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 ................
00003EC0 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 ................
00003ED0 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 ................
00003EE0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003EF0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003F00 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003F10 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003F20 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F30 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F40 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F50 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F60 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F70 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F80 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F90 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003FA0 00 00 00 01 00 00 29 B4 00 00 00 00 00 00 00 02 ......).........
00003FB0 00 00 20 38 00 00 2B 8C 00 00 00 00 00 00 00 00 .. 8..+.........
00003FC0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00003FD0 00 00 00 00                                     ....            
