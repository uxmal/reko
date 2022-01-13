;;; Segment code (00001000)

;; fn00001000: 00001000
fn00001000 proc
	bra	$0000100A
00001002       56 42 43 43 20 30 2E 39                     VBCC 0.9      

l0000100A:
	move.l	d0,d2
	movea.l	a0,a2
	lea	$0000BD86,a4
	movea.l	$00000004,a6
	cmpi.w	#$0024,$0014(a6)
	bcc	$00001036

l00001020:
	lea	$00003D90,a0
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
	lea	$0000BD86,a4
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
	lea	$00003ED8,a3
	move.l	#$00003ED8,d0
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
;;     00002BDE (in fn00002BBC)
fn0000131C proc
	movem.l	a2-a3,-(a7)
	tst.l	$00003DC0
	bne	$0000134E

l00001328:
	movea.l	$00003EE8,a3
	moveq	#$01,d0
	move.l	d0,$00003DC0
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
	lea	$00003ED0,a3
	move.l	#$00003ECC,d0
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
	pea	000013DC                                               ; $004A(pc)
	jsr.l	$00002F18
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	pea	000013F0                                               ; $0048(pc)
	jsr.l	$0000141C
	lea	$0010(a7),a7
	move.l	(a7),d0
	cmp.l	$0004(a7),d0
	beq	$000013D8

l000013BE:
	move.l	$0004(a7),-(a7)
	move.l	$0004(a7),-(a7)
	bsr	$00001408
	move.l	d0,-(a7)
	pea	000013F8                                               ; $002E(pc)
	jsr.l	$00002F18
	lea	$0010(a7),a7

l000013D8:
	addq.w	#$08,a7
	rts
000013DC                                     45 6E 74 65             Ente
000013E0 72 20 32 20 6E 75 6D 62 65 72 73 3A 20 00 00 00 r 2 numbers: ...
000013F0 25 64 20 25 64 00 00 00 4D 61 78 69 6D 75 6D 3A %d %d...Maximum:
00001400 20 25 64 0A 00 00 00 00                          %d.....        

;; fn00001408: 00001408
;;   Called from:
;;     000013C6 (in fn00001390)
fn00001408 proc
	move.l	$0008(a7),d1
	move.l	$0004(a7),d0
	cmp.l	d0,d1
	bge	$00001416

l00001414:
	bra	$00001418

l00001416:
	move.l	d1,d0

l00001418:
	rts
0000141A                               00 00                       ..    

;; fn0000141C: 0000141C
;;   Called from:
;;     000013AC (in fn00001390)
fn0000141C proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00003EF4,-(a7)
	jsr.l	$00001468
	lea	$000C(a7),a7
	rts

;; fn00001438: 00001438
;;   Called from:
;;     00001772 (in fn00001468)
;;     0000198E (in fn00001468)
;;     00001A74 (in fn00001468)
;;     00001AF6 (in fn00001468)
;;     00001D36 (in fn00001468)
;;     00001D54 (in fn00001468)
;;     00001EAE (in fn00001468)
;;     00001EC8 (in fn00001468)
;;     00002194 (in fn00001468)
;;     000021AE (in fn00001468)
;;     000023E8 (in fn00001468)
;;     00002454 (in fn00001468)
fn00001438 proc
	movem.l	a2,-(a7)
	movea.l	$000C(a7),a2
	move.l	a2,d0
	beq	$00001462

l00001444:
	move.l	$0004(a2),d0
	cmp.l	$0008(a2),d0
	bcc	$00001456

l0000144E:
	movea.l	$0004(a2),a0
	move.b	$000B(a7),(a0)

l00001456:
	lea	$0014(a2),a0
	addq.l	#$01,(a0)
	lea	$0004(a2),a0
	subq.l	#$01,(a0)

l00001462:
	movea.l	(a7)+,a2
	rts
00001466                   00 00                               ..        

;; fn00001468: 00001468
;;   Called from:
;;     0000142C (in fn0000141C)
fn00001468 proc
	lea	-$004C(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$0084(a7),d2
	movea.l	$0080(a7),a4
	movea.l	$007C(a7),a2
	clr.l	$003C(a7)
	moveq	#$00,d4
	moveq	#$00,d5
	tst.b	(a4)
	beq	$0000246A

l0000148A:
	moveq	#$00,d3
	cmpi.b	#$25,(a4)
	bne	$00002368

l00001494:
	moveq	#-$01,d6
	move.b	#$69,$0048(a7)
	clr.b	$0049(a7)
	lea	$0001(a4),a3
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00001518

l000014BC:
	moveq	#$00,d6
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00001518

l000014D6:
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
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$000014D6

l00001518:
	cmpi.b	#$68,(a3)
	beq	$00001542

l0000151E:
	cmpi.b	#$6C,(a3)
	beq	$00001542

l00001524:
	cmpi.b	#$4C,(a3)
	beq	$00001542

l0000152A:
	cmpi.b	#$7A,(a3)
	beq	$00001542

l00001530:
	cmpi.b	#$6A,(a3)
	beq	$00001542

l00001536:
	cmpi.b	#$74,(a3)
	beq	$00001542

l0000153C:
	cmpi.b	#$2A,(a3)
	bne	$000015AA

l00001542:
	move.b	$0049(a7),d7
	move.b	$0048(a7),d1

l0000154A:
	cmpi.b	#$2A,(a3)
	bne	$00001554

l00001550:
	moveq	#$01,d7
	bra	$00001576

l00001554:
	cmp.b	#$68,d1
	bne	$00001564

l0000155A:
	cmpi.b	#$68,(a3)
	bne	$00001564

l00001560:
	moveq	#$02,d1
	bra	$00001576

l00001564:
	cmp.b	#$6C,d1
	bne	$00001574

l0000156A:
	cmpi.b	#$6C,(a3)
	bne	$00001574

l00001570:
	moveq	#$01,d1
	bra	$00001576

l00001574:
	move.b	(a3),d1

l00001576:
	addq.l	#$01,a3
	cmpi.b	#$68,(a3)
	beq	$0000154A

l0000157E:
	cmpi.b	#$6C,(a3)
	beq	$0000154A

l00001584:
	cmpi.b	#$4C,(a3)
	beq	$0000154A

l0000158A:
	cmpi.b	#$7A,(a3)
	beq	$0000154A

l00001590:
	cmpi.b	#$6A,(a3)
	beq	$0000154A

l00001596:
	cmpi.b	#$74,(a3)
	beq	$0000154A

l0000159C:
	cmpi.b	#$2A,(a3)
	beq	$0000154A

l000015A2:
	move.b	d1,$0048(a7)
	move.b	d7,$0049(a7)

l000015AA:
	cmpi.b	#$6A,$0048(a7)
	bne	$000015B8

l000015B2:
	move.b	#$01,$0048(a7)

l000015B8:
	cmpi.b	#$74,$0048(a7)
	bne	$000015C6

l000015C0:
	move.b	#$69,$0048(a7)

l000015C6:
	cmpi.b	#$7A,$0048(a7)
	bne	$000015D4

l000015CE:
	move.b	#$6C,$0048(a7)

l000015D4:
	move.b	(a3)+,d7
	beq	$0000164A

l000015D8:
	cmp.b	#$25,d7
	beq	$0000164A

l000015DE:
	cmp.b	#$63,d7
	beq	$0000164A

l000015E4:
	cmp.b	#$6E,d7
	beq	$0000164A

l000015EA:
	cmp.b	#$5B,d7
	beq	$0000164A

l000015F0:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000161A

l00001602:
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
	bra	$00001626

l0000161A:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,d1
	addq.w	#$04,a7

l00001626:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$000015F0

l00001648:
	moveq	#$01,d3

l0000164A:
	move.b	d7,d1
	sub.b	#$25,d1
	beq	$00001A96

l00001654:
	sub.b	#$36,d1
	beq	$00001792

l0000165C:
	subq.b	#$08,d1
	beq	$00001672

l00001660:
	sub.b	#$0B,d1
	beq	$00001B04

l00001668:
	subq.b	#$05,d1
	beq	$000019B0

l0000166E:
	bra	$00001BA6

l00001672:
	cmp.l	#$FFFFFFFF,d6
	bne	$0000167C

l0000167A:
	moveq	#$01,d6

l0000167C:
	tst.b	$0049(a7)
	bne	$00001696

l00001682:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a1
	bra	$00001698

l00001696:
	suba.l	a1,a1

l00001698:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	move.l	a1,$002C(a7)
	tst.l	(a0)
	blt	$000016D0

l000016B0:
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
	bra	$000016EE

l000016D0:
	movea.l	$002C(a7),a1
	move.l	a2,-(a7)
	move.l	a1,$0030(a7)
	jsr.l	$00002604
	move.l	d0,$0038(a7)
	movea.l	$0030(a7),a1
	move.l	a1,$0030(a7)
	addq.w	#$04,a7

l000016EE:
	movea.l	$002C(a7),a1
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$0034(a7)
	beq	$00001766

l00001704:
	move.l	a1,$002C(a7)
	cmp.l	d3,d6
	bcs	$00001766

l0000170C:
	move.b	$0049(a7),d7
	movea.l	$002C(a7),a4

l00001714:
	tst.b	d7
	bne	$0000171A

l00001718:
	move.b	d5,(a4)+

l0000171A:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001744

l0000172C:
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
	bra	$00001750

l00001744:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,d1
	addq.w	#$04,a7

l00001750:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00001762

l0000175E:
	cmp.l	d3,d6
	bcc	$00001714

l00001762:
	move.b	d7,$0049(a7)

l00001766:
	cmp.l	#$FFFFFFFF,d5
	beq	$00001778

l0000176E:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l00001778:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00002362

l00001784:
	tst.l	d3
	beq	$00002362

l0000178A:
	addq.l	#$01,$003C(a7)
	bra	$00002362

l00001792:
	clr.b	$002C(a7)
	cmpi.b	#$5E,(a3)
	bne	$000017A4

l0000179C:
	move.b	#$01,$002C(a7)
	addq.l	#$01,a3

l000017A4:
	clr.l	$0034(a7)
	move.b	$002C(a7),d7
	move.l	$0034(a7),d1

l000017B0:
	tst.b	d7
	beq	$000017BC

l000017B4:
	move.l	#$000000FF,d5
	bra	$000017BE

l000017BC:
	moveq	#$00,d5

l000017BE:
	lea	$004E(a7),a0
	move.b	d5,(a0,d1)
	addq.l	#$01,d1
	cmp.l	#$00000020,d1
	bcs	$000017B0

l000017D0:
	move.l	d2,$0084(a7)
	move.b	d7,$002C(a7)
	move.b	$002C(a7),d2

l000017DC:
	tst.b	(a3)
	beq	$00001852

l000017E0:
	move.b	(a3)+,d1
	cmpi.b	#$2D,(a3)
	bne	$000017F4

l000017E8:
	cmp.b	$0001(a3),d1
	bcc	$000017F4

l000017EE:
	addq.l	#$01,a3
	move.b	(a3)+,d7
	bra	$000017F6

l000017F4:
	move.b	d1,d7

l000017F6:
	moveq	#$00,d5
	move.b	d1,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcs	$0000184C

l00001802:
	tst.b	d2
	beq	$00001826

l00001806:
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
	bra	$00001842

l00001826:
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

l00001842:
	addq.l	#$01,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcc	$00001802

l0000184C:
	cmpi.b	#$5D,(a3)
	bne	$000017DC

l00001852:
	move.l	$0084(a7),d2
	addq.l	#$01,a3
	tst.b	$0049(a7)
	bne	$00001872

l0000185E:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a6
	bra	$00001874

l00001872:
	suba.l	a6,a6

l00001874:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000018A6

l00001886:
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
	bra	$000018B4

l000018A6:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l000018B4:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$002C(a7)
	beq	$00001982

l000018C8:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	$0000254C
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
	beq	$00001982

l000018F8:
	cmp.l	d3,d6
	bcs	$00001982

l000018FE:
	move.b	$0049(a7),d7

l00001902:
	tst.b	d7
	bne	$00001908

l00001906:
	move.b	d5,(a6)+

l00001908:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001932

l0000191A:
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
	bra	$0000193E

l00001932:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,d1
	addq.w	#$04,a7

l0000193E:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$0000197E

l0000194C:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	$0000254C
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
	beq	$0000197E

l0000197A:
	cmp.l	d3,d6
	bcc	$00001902

l0000197E:
	move.b	d7,$0049(a7)

l00001982:
	cmp.l	#$FFFFFFFF,d5
	beq	$00001994

l0000198A:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l00001994:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00002362

l000019A0:
	tst.l	d3
	beq	$00002362

l000019A6:
	clr.b	(a6)+
	addq.l	#$01,$003C(a7)
	bra	$00002362

l000019B0:
	tst.b	$0049(a7)
	bne	$000019CA

l000019B6:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a5
	bra	$000019CC

l000019CA:
	suba.l	a5,a5

l000019CC:
	cmp.l	#$FFFFFFFF,d5
	beq	$00001A68

l000019D6:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00001A68

l000019F2:
	cmp.l	d3,d6
	bcs	$00001A68

l000019F6:
	move.b	$0049(a7),d7

l000019FA:
	tst.b	d7
	bne	$00001A00

l000019FE:
	move.b	d5,(a5)+

l00001A00:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001A2A

l00001A12:
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
	bra	$00001A36

l00001A2A:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,d1
	addq.w	#$04,a7

l00001A36:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00001A64

l00001A44:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00001A64

l00001A60:
	cmp.l	d3,d6
	bcc	$000019FA

l00001A64:
	move.b	d7,$0049(a7)

l00001A68:
	cmp.l	#$FFFFFFFF,d5
	beq	$00001A7A

l00001A70:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l00001A7A:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00002362

l00001A86:
	tst.l	d3
	beq	$00002362

l00001A8C:
	clr.b	(a5)+
	addq.l	#$01,$003C(a7)
	bra	$00002362

l00001A96:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001AC8

l00001AA8:
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
	bra	$00001AD6

l00001AC8:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00001AD6:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$00000025,$002C(a7)
	beq	$00002362

l00001AEA:
	cmp.l	#$FFFFFFFF,d5
	beq	$00001AFC

l00001AF2:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l00001AFC:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00002362

l00001B04:
	tst.b	$0049(a7)
	bne	$00001B9C

l00001B0C:
	cmpi.b	#$01,$0048(a7)
	bne	$00001B2E

l00001B14:
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
	bra	$00001B9C

l00001B2E:
	cmpi.b	#$6C,$0048(a7)
	bne	$00001B4C

l00001B36:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)
	bra	$00001B9C

l00001B4C:
	cmpi.b	#$68,$0048(a7)
	bne	$00001B6A

l00001B54:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.w	d4,(a0)
	bra	$00001B9C

l00001B6A:
	cmpi.b	#$02,$0048(a7)
	bne	$00001B88

l00001B72:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.b	d4,(a0)
	bra	$00001B9C

l00001B88:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)

l00001B9C:
	moveq	#$01,d3
	addq.l	#$01,$003C(a7)
	bra	$00002362

l00001BA6:
	clr.l	$0030(a7)
	clr.l	$002C(a7)
	clr.l	$006E(a7)
	tst.b	d7
	bne	$00001BB8

l00001BB6:
	subq.l	#$01,a3

l00001BB8:
	cmp.b	#$70,d7
	bne	$00001BC6

l00001BBE:
	move.b	#$6C,$0048(a7)
	moveq	#$78,d7

l00001BC6:
	cmp.l	#$0000002D,d5
	bne	$00001BD4

l00001BCE:
	cmp.b	#$75,d7
	bne	$00001BDC

l00001BD4:
	cmp.l	#$0000002B,d5
	bne	$00001C2C

l00001BDC:
	cmp.l	d3,d6
	bcs	$00001C2C

l00001BE0:
	move.l	d5,$006E(a7)
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001C16

l00001BF6:
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
	bra	$00001C24

l00001C16:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00001C24:
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4

l00001C2C:
	cmp.b	#$69,d7
	bne	$00001D9E

l00001C34:
	cmp.l	#$00000030,d5
	bne	$00001D60

l00001C3E:
	cmp.l	d3,d6
	bcs	$00001D60

l00001C44:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001C76

l00001C56:
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
	bra	$00001C84

l00001C76:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00001C84:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$00002BEC,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$00001CAA

l00001CA6:
	or.b	#$20,d0

l00001CAA:
	cmp.l	#$00000078,d0
	bne	$00001D42

l00001CB4:
	cmp.l	d3,d6
	bcs	$00001D42

l00001CBA:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001CEC

l00001CCC:
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
	bra	$00001CFA

l00001CEC:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00001CFA:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$00001D26

l00001D20:
	cmp.l	d3,d6
	bcs	$00001D26

l00001D24:
	moveq	#$78,d7

l00001D26:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$00001D3C

l00001D30:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l00001D3C:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00001D44

l00001D42:
	moveq	#$6F,d7

l00001D44:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$00001D5A

l00001D4E:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l00001D5A:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00001D9E

l00001D60:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00001D9E

l00001D7C:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$00001D9E

l00001D98:
	cmp.l	d3,d6
	bcs	$00001D9E

l00001D9C:
	moveq	#$78,d7

l00001D9E:
	cmp.b	#$78,d7
	bne	$00001ED2

l00001DA6:
	cmp.l	#$00000030,d5
	bne	$00001ED2

l00001DB0:
	cmp.l	d3,d6
	bcs	$00001ED2

l00001DB6:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001DE8

l00001DC8:
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
	bra	$00001DF6

l00001DE8:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00001DF6:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$00002BEC,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$00001E1C

l00001E18:
	or.b	#$20,d0

l00001E1C:
	cmp.l	#$00000078,d0
	bne	$00001EB8

l00001E26:
	cmp.l	d3,d6
	bcs	$00001EB8

l00001E2C:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001E5E

l00001E3E:
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
	bra	$00001E6C

l00001E5E:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00001E6C:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$00001E9E

l00001E94:
	cmp.l	d3,d6
	bcs	$00001E9E

l00001E98:
	move.l	$004A(a7),d5
	bra	$00001ED2

l00001E9E:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$00001EB4

l00001EA8:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l00001EB4:
	subq.l	#$01,d3
	subq.l	#$01,d4

l00001EB8:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$00001ECE

l00001EC2:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l00001ECE:
	subq.l	#$01,d3
	subq.l	#$01,d4

l00001ED2:
	cmp.b	#$78,d7
	beq	$00001EDE

l00001ED8:
	cmp.b	#$58,d7
	bne	$00001EE8

l00001EDE:
	move.l	#$00000010,$0040(a7)
	bra	$00001F06

l00001EE8:
	cmp.b	#$6F,d7
	bne	$00001EF8

l00001EEE:
	move.l	#$00000008,$0034(a7)
	bra	$00001F00

l00001EF8:
	move.l	#$0000000A,$0034(a7)

l00001F00:
	move.l	$0034(a7),$0040(a7)

l00001F06:
	move.l	$0040(a7),$0072(a7)
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$0000216E

l00001F38:
	cmpi.l	#$0000000A,$0072(a7)
	bne	$00001F6E

l00001F42:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$0000216E

l00001F6E:
	cmpi.l	#$00000008,$0072(a7)
	bne	$00001F8E

l00001F78:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	cmp.l	#$00000037,d5
	bgt	$0000216E

l00001F8E:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.l	d6,$0040(a7)
	move.b	d7,$004A(a7)
	cmp.l	d3,d6
	bcs	$0000216E

l00001FA4:
	move.l	$0072(a7),d7
	movea.l	$0040(a7),a4

l00001FAC:
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
	jsr.l	$0000248C
	lea	$0010(a7),a7
	movea.l	(a7)+,a1
	move.l	d0,$0048(a7)
	move.l	d1,$004C(a7)
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$0000200C

l00002002:
	move.l	d5,d4
	sub.l	#$00000030,d4
	bra	$0000200E

l0000200C:
	moveq	#$00,d4

l0000200E:
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
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000001,d0
	beq	$0000205E

l00002054:
	move.l	d5,d6
	sub.l	#$00000037,d6
	bra	$00002060

l0000205E:
	moveq	#$00,d6

l00002060:
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
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000002,d0
	beq	$000020B0

l000020A6:
	move.l	d5,d2
	sub.l	#$00000057,d2
	bra	$000020B2

l000020B0:
	moveq	#$00,d2

l000020B2:
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
	blt	$00002104

l000020EC:
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
	bra	$00002110

l00002104:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,d1
	addq.w	#$04,a7

l00002110:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,$0034(a7)
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$0000216E

l00002134:
	cmp.l	#$0000000A,d7
	bne	$00002158

l0000213C:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$0000216E

l00002158:
	cmp.l	#$00000008,d7
	bne	$00002168

l00002160:
	cmp.l	#$00000037,d5
	bgt	$0000216E

l00002168:
	cmpa.l	d3,a4
	bcc	$00001FAC

l0000216E:
	move.b	$004A(a7),d7
	move.l	$0034(a7),d4
	move.l	$0084(a7),d2
	tst.l	$006E(a7)
	beq	$000021A2

l00002180:
	cmp.l	#$00000002,d3
	bne	$000021A2

l00002188:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000219A

l00002190:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l0000219A:
	subq.l	#$01,d3
	subq.l	#$01,d4
	move.l	$006E(a7),d5

l000021A2:
	cmp.l	#$FFFFFFFF,d5
	beq	$000021B4

l000021AA:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l000021B4:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00002362

l000021C0:
	tst.l	d3
	beq	$00002362

l000021C6:
	cmp.b	#$75,d7
	bne	$00002288

l000021CE:
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0038(a7)
	move.l	(a7)+,d0
	tst.b	$0034(a7)
	beq	$000021F8

l000021E2:
	subq.b	#$01,$0034(a7)
	beq	$00002250

l000021E8:
	subi.b	#$66,$0034(a7)
	beq	$00002234

l000021F0:
	subq.b	#$04,$0034(a7)
	beq	$00002218

l000021F6:
	bra	$0000226C

l000021F8:
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
	bra	$0000235E

l00002218:
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
	bra	$0000235E

l00002234:
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
	bra	$0000235E

l00002250:
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
	bra	$0000235E

l0000226C:
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
	bra	$0000235E

l00002288:
	cmpi.l	#$0000002D,$006E(a7)
	bne	$000022A4

l00002292:
	movem.l	$002C(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0034(a7)
	bra	$000022B0

l000022A4:
	move.l	$0030(a7),$0038(a7)
	move.l	$002C(a7),$0034(a7)
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0030(a7)
	move.l	(a7)+,d0
	tst.b	$002C(a7)
	beq	$000022DA

l000022C4:
	subq.b	#$01,$002C(a7)
	beq	$0000232C

l000022CA:
	subi.b	#$66,$002C(a7)
	beq	$00002312

l000022D2:
	subq.b	#$04,$002C(a7)
	beq	$000022F8

l000022D8:
	bra	$00002346

l000022DA:
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
	bra	$0000235E

l000022F8:
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
	bra	$0000235E

l00002312:
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
	bra	$0000235E

l0000232C:
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
	bra	$0000235E

l00002346:
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

l0000235E:
	addq.l	#$01,$003C(a7)

l00002362:
	movea.l	a3,a4
	bra	$00002460

l00002368:
	move.b	(a4),d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	beq	$000023F4

l00002384:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000023AE

l00002396:
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
	bra	$000023BA

l000023AE:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,d1
	addq.w	#$04,a7

l000023BA:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00002384

l000023DC:
	cmp.l	#$FFFFFFFF,d5
	beq	$000023EE

l000023E4:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l000023EE:
	subq.l	#$01,d4
	moveq	#$01,d3
	bra	$0000245E

l000023F4:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00002426

l00002406:
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
	bra	$00002434

l00002426:
	move.l	a2,-(a7)
	jsr.l	$00002604
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00002434:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	(a4),d0
	ext.w	d0
	ext.l	d0
	cmp.l	$002C(a7),d0
	beq	$0000245E

l00002448:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000245A

l00002450:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	$00001438
	addq.w	#$08,a7

l0000245A:
	subq.l	#$01,d3
	subq.l	#$01,d4

l0000245E:
	addq.l	#$01,a4

l00002460:
	tst.l	d3
	beq	$0000246A

l00002464:
	tst.b	(a4)
	bne	$0000148A

l0000246A:
	cmp.l	#$FFFFFFFF,d5
	bne	$0000247C

l00002472:
	tst.l	$003C(a7)
	bne	$0000247C

l00002478:
	move.l	d5,d0
	bra	$00002480

l0000247C:
	move.l	$003C(a7),d0

l00002480:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$004C(a7),a7
	rts
0000248A                               00 00                       ..    

;; fn0000248C: 0000248C
;;   Called from:
;;     00001FCA (in fn00001468)
fn0000248C proc
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
	bcc	$000024C0

l000024BA:
	add.l	#$00010000,d1

l000024C0:
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
0000250A                               00 00 4C EF 00 03           ..L...
00002510 00 04 4A 81 6B 0A 4A 80 6B 12 61 62 20 01 4E 75 ..J.k.J.k.ab .Nu
00002520 44 81 4A 80 6B 10 61 56 20 01 4E 75 44 80 61 4E D.J.k.aV .NuD.aN
00002530 44 81 20 01 4E 75 44 80 61 44 44 81 20 01 4E 75 D. .NuD.aDD. .Nu
00002540 4C EF 00 03 00 04 61 36 20 01 4E 75             L.....a6 .Nu    

;; fn0000254C: 0000254C
;;   Called from:
;;     000018D6 (in fn00001468)
;;     0000195A (in fn00001468)
fn0000254C proc
	movem.l	$0004(a7),d0-d1
	tst.l	d0
	bpl	$0000256C

l00002556:
	neg.l	d0
	tst.l	d1
	bpl	$00002564

l0000255C:
	neg.l	d1
	bsr	$0000257E
	neg.l	d1
	rts

l00002564:
	bsr	$0000257E
	neg.l	d0
	neg.l	d1
	rts

l0000256C:
	tst.l	d1
	bpl	$0000257E

l00002570:
	neg.l	d1
	bsr	$0000257E
	neg.l	d0
	rts
00002578                         4C EF 00 03 00 04               L.....  

;; fn0000257E: 0000257E
;;   Called from:
;;     0000255E (in fn0000254C)
;;     00002564 (in fn0000254C)
;;     0000256E (in fn0000254C)
;;     00002572 (in fn0000254C)
;;     00003B46 (in fn00003B28)
fn0000257E proc
	move.l	d2,-(a7)
	swap.l	d1
	move.w	d1,d2
	bne	$000025A4

l00002586:
	swap.l	d0
	swap.l	d1
	swap.l	d2
	move.w	d0,d2
	beq	$00002594

l00002590:
	divu.w	d1,d2
	move.w	d2,d0

l00002594:
	swap.l	d0
	move.w	d0,d2
	divu.w	d1,d2
	move.w	d2,d0
	swap.l	d2
	move.w	d2,d1
	move.l	(a7)+,d2
	rts

l000025A4:
	move.l	d3,-(a7)
	moveq	#$10,d3
	cmp.w	#$0080,d1
	bcc	$000025B2

l000025AE:
	rol.l	#$08,d1
	subq.w	#$08,d3

l000025B2:
	cmp.w	#$0800,d1
	bcc	$000025BC

l000025B8:
	rol.l	#$04,d1
	subq.w	#$04,d3

l000025BC:
	cmp.w	#$2000,d1
	bcc	$000025C6

l000025C2:
	rol.l	#$02,d1
	subq.w	#$02,d3

l000025C6:
	tst.w	d1
	bmi	$000025CE

l000025CA:
	rol.l	#$01,d1
	subq.w	#$01,d3

l000025CE:
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
	bcc	$000025F0

l000025EA:
	subq.w	#$01,d3
	add.l	d1,d0

l000025EE:
	bcc	$000025EE

l000025F0:
	moveq	#$00,d1
	move.w	d3,d1
	swap.l	d3
	rol.l	d3,d0
	swap.l	d0
	exg	d0,d1
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	rts
00002602       00 00                                       ..            

;; fn00002604: 00002604
;;   Called from:
;;     0000161C (in fn00001468)
;;     000016DA (in fn00001468)
;;     00001746 (in fn00001468)
;;     000018A8 (in fn00001468)
;;     00001934 (in fn00001468)
;;     00001A2C (in fn00001468)
;;     00001ACA (in fn00001468)
;;     00001C18 (in fn00001468)
;;     00001C78 (in fn00001468)
;;     00001CEE (in fn00001468)
;;     00001DEA (in fn00001468)
;;     00001E60 (in fn00001468)
;;     00002106 (in fn00001468)
;;     000023B0 (in fn00001468)
;;     00002428 (in fn00001468)
fn00002604 proc
	movem.l	d2-d5/a2-a4/a6,-(a7)
	movea.l	$0024(a7),a2
	jsr.l	$00002BBC
	move.l	a2,d0
	bne	$0000261C

l00002616:
	moveq	#-$01,d0
	bra	$000026FA

l0000261C:
	moveq	#$2A,d0
	and.l	$0018(a2),d0
	moveq	#$20,d5
	cmp.l	d0,d5
	beq	$0000262E

l00002628:
	moveq	#-$01,d0
	bra	$000026FA

l0000262E:
	lea	$0018(a2),a0
	moveq	#$01,d0
	or.l	d0,(a0)
	move.l	#$00000200,d0
	and.l	(a0),d0
	beq	$00002646

l00002640:
	jsr.l	$00002AEC

l00002646:
	tst.l	$001C(a2)
	bne	$00002664

l0000264C:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$0000265C

l00002654:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00002664

l0000265C:
	move.l	#$00000400,$001C(a2)

l00002664:
	tst.l	$0008(a2)
	bne	$000026A0

l0000266A:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$0000267E

l0000267A:
	moveq	#$02,d4
	bra	$00002680

l0000267E:
	moveq	#$01,d4

l00002680:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	$00002718
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00002698

l00002694:
	moveq	#-$01,d0
	bra	$000026FA

l00002698:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)

l000026A0:
	lea	$0004(a2),a0
	move.l	$0008(a2),(a0)
	move.l	$001C(a2),d3
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003D98,a6
	jsr.l	-$002A(a6)
	lea	$0014(a2),a0
	move.l	d0,(a0)
	subq.l	#$01,(a0)
	bge	$000026E6

l000026C4:
	moveq	#-$01,d0
	cmp.l	$0014(a2),d0
	bne	$000026D6

l000026CC:
	lea	$0018(a2),a0
	moveq	#$08,d0
	or.l	d0,(a0)
	bra	$000026DE

l000026D6:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)

l000026DE:
	clr.l	$0014(a2)
	moveq	#-$01,d0
	bra	$000026FA

l000026E6:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	(a0),d0
	and.l	#$000000FF,d0

l000026FA:
	movem.l	(a7)+,d2-d5/a2-a4/a6
	rts
00002700 4A B9 00 00 3D C8 67 0E 2F 39 00 00 3D C8 4E B9 J...=.g./9..=.N.
00002710 00 00 2A 7C 58 4F 4E 75                         ..*|XONu        

;; fn00002718: 00002718
;;   Called from:
;;     00002686 (in fn00002604)
;;     0000397E (in fn00003910)
fn00002718 proc
	movem.l	d2,-(a7)
	move.l	$0008(a7),d2
	bne	$00002726

l00002722:
	moveq	#$00,d0
	bra	$0000277C

l00002726:
	tst.l	$00003DC8
	bne	$0000274A

l0000272E:
	movea.l	$00003DC4,a0
	move.l	a0,-(a7)
	move.l	a0,-(a7)
	clr.l	-(a7)
	jsr.l	$00002A0C
	move.l	d0,$00003DC8
	lea	$000C(a7),a7

l0000274A:
	tst.l	$00003DC8
	bne	$00002756

l00002752:
	moveq	#$00,d0
	bra	$0000277C

l00002756:
	moveq	#$04,d0
	add.l	d2,d0
	move.l	d0,-(a7)
	move.l	$00003DC8,-(a7)
	jsr.l	$000028E8
	movea.l	d0,a1
	addq.w	#$08,a7
	move.l	a1,d0
	bne	$00002774

l00002770:
	moveq	#$00,d0
	bra	$0000277C

l00002774:
	move.l	d2,(a1)
	lea	$0004(a1),a0
	move.l	a0,d0

l0000277C:
	movem.l	(a7)+,d2
	rts
00002782       00 00                                       ..            

;; fn00002784: 00002784
fn00002784 proc
	move.l	$0004(a7),d0
	movea.l	d0,a0
	tst.l	d0
	beq	$000027AE

l0000278E:
	tst.l	$00003DC8
	beq	$000027AE

l00002796:
	moveq	#$04,d0
	add.l	-(a0),d0
	move.l	d0,-(a7)
	move.l	a0,-(a7)
	move.l	$00003DC8,-(a7)
	jsr.l	$0000284C
	lea	$000C(a7),a7

l000027AE:
	rts
000027B0 48 E7 30 38 28 6F 00 1C 24 6F 00 18 22 0A 66 0A H.08(o..$o..".f.
000027C0 2F 0C 61 00 FF 54 58 4F 60 7A 26 6A FF FC 2F 0C /.a..TXO`z&j../.
000027D0 61 00 FF 46 26 00 58 4F 67 68 B9 CB 64 04 20 0C a..F&.XOgh..d. .
000027E0 60 02 20 0B 20 43 22 4A 24 00 B4 BC 00 00 00 10 `. . C"J$.......
000027F0 65 3C 20 08 22 09 C0 3C 00 01 C2 3C 00 01 B2 00 e< ."..<...<....
00002800 66 1A 20 08 4A 01 67 04 10 D9 53 82 72 03 C2 82 f. .J.g...S.r...
00002810 94 81 20 D9 59 82 66 FA 34 01 60 14 B4 BC 00 01 .. .Y.f.4.`.....
00002820 00 00 65 0A 20 08 10 D9 53 82 66 FA 60 0C 20 08 ..e. ...S.f.`. .
00002830 53 42 65 06 10 D9 51 CA FF FC 2F 0A 61 00 FF 46 SBe...Q.../.a..F
00002840 58 4F 20 03 4C DF 1C 0C 4E 75 00 00             XO .L...Nu..    

;; fn0000284C: 0000284C
;;   Called from:
;;     000027A4 (in fn00002784)
fn0000284C proc
	movem.l	d2/a2-a6,-(a7)
	move.l	$0020(a7),d1
	movea.l	$0024(a7),a5
	movea.l	$001C(a7),a4
	movea.l	$00003D94,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$0000287C

l0000286A:
	movea.l	$00003D94,a6
	movea.l	a4,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$02CA(a6)
	bra	$000028E0

l0000287C:
	move.l	a4,d2
	beq	$000028E0

l00002880:
	tst.l	d1
	beq	$000028E0

l00002884:
	movea.l	d1,a3
	lea	-$000C(a3),a3
	cmpa.l	$0014(a4),a5
	bcc	$000028C6

l00002890:
	movea.l	a4,a2

l00002892:
	movea.l	(a2),a2
	tst.l	(a2)
	beq	$000028E0

l00002898:
	tst.b	$0008(a2)
	beq	$00002892

l0000289E:
	cmp.l	$0014(a2),d1
	bcs	$00002892

l000028A4:
	cmp.l	$0018(a2),d1
	bcc	$00002892

l000028AA:
	movea.l	$00003D94,a6
	movea.l	a2,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$00C0(a6)
	move.l	$001C(a2),d0
	cmp.l	$0010(a4),d0
	bne	$000028E0

l000028C4:
	movea.l	a2,a3

l000028C6:
	movea.l	$00003D94,a6
	movea.l	a3,a1
	jsr.l	-$00FC(a6)
	move.l	-(a3),d0
	movea.l	$00003D94,a6
	movea.l	a3,a1
	jsr.l	-$00D2(a6)

l000028E0:
	movem.l	(a7)+,d2/a2-a6
	rts
000028E6                   00 00                               ..        

;; fn000028E8: 000028E8
;;   Called from:
;;     00002762 (in fn00002718)
fn000028E8 proc
	movem.l	d2-d4/a2-a6,-(a7)
	move.l	$0028(a7),d2
	movea.l	$0024(a7),a4
	movea.l	$00003D94,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002914

l00002902:
	movea.l	$00003D94,a6
	movea.l	a4,a0
	move.l	d2,d0
	jsr.l	-$02C4(a6)
	bra	$00002A06

l00002914:
	suba.l	a3,a3
	move.l	a4,d4
	beq	$00002A04

l0000291C:
	tst.l	d2
	beq	$00002A04

l00002922:
	cmp.l	$0014(a4),d2
	bcc	$000029D6

l0000292A:
	movea.l	(a4),a5

l0000292C:
	tst.l	(a5)
	beq	$0000294E

l00002930:
	tst.b	$0008(a5)
	beq	$0000294A

l00002936:
	movea.l	$00003D94,a6
	movea.l	a5,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3
	move.l	a3,d0
	bne	$000029BA

l0000294A:
	movea.l	(a5),a5
	bra	$0000292C

l0000294E:
	moveq	#$28,d3
	add.l	$0010(a4),d3
	move.l	$000C(a4),d1
	movea.l	$00003D94,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$00002A04

l0000296C:
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
	movea.l	$00003D94,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F0(a6)
	movea.l	$00003D94,a6
	movea.l	a3,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3

l000029BA:
	move.l	#$00010000,d0
	and.l	$000C(a4),d0
	beq	$00002A04

l000029C6:
	movea.l	a3,a2
	addq.l	#$07,d2
	lsr.l	#$03,d2

l000029CC:
	clr.l	(a2)+
	clr.l	(a2)+
	subq.l	#$01,d2
	bne	$000029CC

l000029D4:
	bra	$00002A04

l000029D6:
	moveq	#$10,d3
	add.l	d2,d3
	move.l	$000C(a4),d1
	movea.l	$00003D94,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$00002A04

l000029F0:
	move.l	d3,(a3)+
	movea.l	$00003D94,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F6(a6)
	addq.l	#$08,a3
	clr.l	(a3)+

l00002A04:
	move.l	a3,d0

l00002A06:
	movem.l	(a7)+,d2-d4/a2-a6
	rts

;; fn00002A0C: 00002A0C
;;   Called from:
;;     0000273A (in fn00002718)
fn00002A0C proc
	movem.l	d2-d3/a2/a6,-(a7)
	move.l	$0018(a7),d3
	movea.l	$001C(a7),a2
	movea.l	$00003D94,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002A3A

l00002A26:
	movea.l	$00003D94,a6
	move.l	$0014(a7),d0
	move.l	d3,d1
	move.l	a2,d2
	jsr.l	-$02B8(a6)
	bra	$00002A76

l00002A3A:
	suba.l	a1,a1
	cmp.l	a2,d3
	bcs	$00002A74

l00002A40:
	addq.l	#$07,d3
	movea.l	$00003D94,a6
	moveq	#$18,d0
	moveq	#$00,d1
	jsr.l	-$00C6(a6)
	movea.l	d0,a1
	move.l	a1,d0
	beq	$00002A74

l00002A56:
	lea	$0004(a1),a0
	move.l	a0,(a1)
	clr.l	(a0)
	move.l	a1,$0008(a1)
	move.l	$0014(a7),$000C(a1)
	moveq	#-$08,d0
	and.l	d3,d0
	move.l	d0,$0010(a1)
	move.l	a2,$0014(a1)

l00002A74:
	move.l	a1,d0

l00002A76:
	movem.l	(a7)+,d2-d3/a2/a6
	rts

;; fn00002A7C: 00002A7C
fn00002A7C proc
	movem.l	d2/a2/a6,-(a7)
	move.l	$0010(a7),d2
	movea.l	$00003D94,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002AA0

l00002A92:
	movea.l	$00003D94,a6
	movea.l	d2,a0
	jsr.l	-$02BE(a6)
	bra	$00002AE4

l00002AA0:
	tst.l	d2
	beq	$00002AE4

l00002AA4:
	movea.l	$00003D94,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d1
	beq	$00002AD6

l00002AB6:
	move.l	-(a2),d0
	movea.l	$00003D94,a6
	movea.l	a2,a1
	jsr.l	-$00D2(a6)
	movea.l	$00003D94,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d0
	bne	$00002AB6

l00002AD6:
	movea.l	$00003D94,a6
	movea.l	d2,a1
	moveq	#$18,d0
	jsr.l	-$00D2(a6)

l00002AE4:
	movem.l	(a7)+,d2/a2/a6
	rts
00002AEA                               00 00                       ..    

;; fn00002AEC: 00002AEC
;;   Called from:
;;     00002640 (in fn00002604)
fn00002AEC proc
	movem.l	a2,-(a7)
	movea.l	$00003EEC,a2
	move.l	a2,d0
	beq	$00002B24

l00002AFA:
	move.l	#$00000202,d0
	and.l	$0018(a2),d0
	cmp.l	#$00000202,d0
	bne	$00002B1A

l00002B0C:
	tst.l	(a2)
	beq	$00002B1A

l00002B10:
	move.l	a2,-(a7)
	jsr.l	$00002B28
	addq.w	#$04,a7

l00002B1A:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00002AFA

l00002B24:
	movea.l	(a7)+,a2
	rts

;; fn00002B28: 00002B28
;;   Called from:
;;     00002B12 (in fn00002AEC)
;;     00002EE2 (in fn00002ED4)
;;     00002F00 (in fn00002ED4)
;;     000039C2 (in fn00003910)
fn00002B28 proc
	movem.l	d2-d4/a2/a6,-(a7)
	movea.l	$0018(a7),a2
	jsr.l	$00002BBC
	move.l	a2,d0
	bne	$00002B3E

l00002B3A:
	moveq	#-$01,d0
	bra	$00002BB6

l00002B3E:
	tst.l	$001C(a2)
	bne	$00002B5C

l00002B44:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00002B54

l00002B4C:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00002B5C

l00002B54:
	move.l	#$00000400,$001C(a2)

l00002B5C:
	tst.l	$0008(a2)
	bne	$00002B66

l00002B62:
	moveq	#$00,d0
	bra	$00002BB6

l00002B66:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002BAC

l00002B6E:
	tst.l	(a2)
	beq	$00002B9C

l00002B72:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003D98,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00002BA0

l00002B90:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00002BB6

l00002B9C:
	moveq	#$00,d0
	bra	$00002BB6

l00002BA0:
	move.l	$0008(a2),$0004(a2)
	move.l	$001C(a2),$0014(a2)

l00002BAC:
	lea	$0018(a2),a0
	moveq	#-$04,d0
	and.l	d0,(a0)
	moveq	#$00,d0

l00002BB6:
	movem.l	(a7)+,d2-d4/a2/a6
	rts

;; fn00002BBC: 00002BBC
;;   Called from:
;;     0000260C (in fn00002604)
;;     00002B30 (in fn00002B28)
;;     0000391C (in fn00003910)
fn00002BBC proc
	movem.l	a6,-(a7)
	movea.l	$00003D94,a6
	moveq	#$00,d0
	move.l	#$00001000,d1
	jsr.l	-$0132(a6)
	and.l	#$00001000,d0
	beq	$00002BE6

l00002BDA:
	pea	$00000014
	jsr.l	$0000131C
	addq.w	#$04,a7

l00002BE6:
	movea.l	(a7)+,a6
	rts
00002BEA                               00 00 00 20 20 20           ...   
00002BF0 20 20 20 20 20 20 28 28 28 28 28 20 20 20 20 20       (((((     
00002C00 20 20 20 20 20 20 20 20 20 20 20 20 20 88 10 10              ...
00002C10 10 10 10 10 10 10 10 10 10 10 10 10 10 04 04 04 ................
00002C20 04 04 04 04 04 04 04 10 10 10 10 10 10 10 41 41 ..............AA
00002C30 41 41 41 41 01 01 01 01 01 01 01 01 01 01 01 01 AAAA............
00002C40 01 01 01 01 01 01 01 01 10 10 10 10 10 10 42 42 ..............BB
00002C50 42 42 42 42 02 02 02 02 02 02 02 02 02 02 02 02 BBBB............
00002C60 02 02 02 02 02 02 02 02 10 10 10 10 20 00 00 00 ............ ...
00002C70 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00002CF0 48 E7 00 32 48 78 00 20 4E B9 00 00 27 18 23 C0 H..2Hx. N...'.#.
00002D00 00 00 3E F4 48 78 00 20 4E B9 00 00 27 18 23 C0 ..>.Hx. N...'.#.
00002D10 00 00 3E F8 48 78 00 20 4E B9 00 00 27 18 23 C0 ..>.Hx. N...'.#.
00002D20 00 00 3E FC 4F EF 00 0C 4A B9 00 00 3E F4 67 10 ..>.O...J...>.g.
00002D30 4A B9 00 00 3E F8 67 08 4A B9 00 00 3E FC 66 0C J...>.g.J...>.f.
00002D40 48 78 00 14 4E B9 00 00 13 1C 58 4F 20 79 00 00 Hx..N.....XO y..
00002D50 3E F4 20 B9 00 00 3D A4 20 79 00 00 3E F4 70 20 >. ...=. y..>.p 
00002D60 21 40 00 18 22 39 00 00 3D A4 2C 79 00 00 3D 98 !@.."9..=.,y..=.
00002D70 4E AE FF 28 4A 80 67 10 20 79 00 00 3E F4 41 E8 N..(J.g. y..>.A.
00002D80 00 18 00 90 00 00 02 04 20 79 00 00 3E F8 20 B9 ........ y..>. .
00002D90 00 00 3D A8 20 79 00 00 3E F8 70 40 21 40 00 18 ..=. y..>.p@!@..
00002DA0 22 39 00 00 3D A8 2C 79 00 00 3D 98 4E AE FF 28 "9..=.,y..=.N..(
00002DB0 4A 80 67 10 20 79 00 00 3E F8 41 E8 00 18 00 90 J.g. y..>.A.....
00002DC0 00 00 02 80 20 79 00 00 3E FC 20 B9 00 00 3D AC .... y..>. ...=.
00002DD0 20 79 00 00 3E FC 70 40 21 40 00 18 22 39 00 00  y..>.p@!@.."9..
00002DE0 3D AC 2C 79 00 00 3D 98 4E AE FF 28 4A 80 67 10 =.,y..=.N..(J.g.
00002DF0 20 79 00 00 3E FC 41 E8 00 18 00 90 00 00 02 80  y..>.A.........
00002E00 20 79 00 00 3E FC 42 A8 00 04 20 79 00 00 3E F8  y..>.B... y..>.
00002E10 42 A8 00 04 20 79 00 00 3E F4 42 A8 00 04 20 79 B... y..>.B... y
00002E20 00 00 3E FC 42 A8 00 08 20 79 00 00 3E F8 42 A8 ..>.B... y..>.B.
00002E30 00 08 20 79 00 00 3E F4 42 A8 00 08 24 79 00 00 .. y..>.B...$y..
00002E40 3E FC 42 AA 00 14 22 79 00 00 3E F8 42 A9 00 14 >.B..."y..>.B...
00002E50 20 79 00 00 3E F4 42 A8 00 14 42 AA 00 1C 42 A9  y..>.B...B...B.
00002E60 00 1C 42 A8 00 1C 42 A8 00 10 20 79 00 00 3E F4 ..B...B... y..>.
00002E70 21 79 00 00 3E F8 00 0C 20 79 00 00 3E F8 21 79 !y..>... y..>.!y
00002E80 00 00 3E F4 00 10 20 79 00 00 3E F8 21 79 00 00 ..>... y..>.!y..
00002E90 3E FC 00 0C 20 79 00 00 3E FC 21 79 00 00 3E F8 >... y..>.!y..>.
00002EA0 00 10 20 79 00 00 3E FC 42 A8 00 0C 23 F9 00 00 .. y..>.B...#...
00002EB0 3E F4 00 00 3E EC 23 F9 00 00 3E FC 00 00 3E F0 >...>.#...>...>.
00002EC0 4C DF 4C 00 4E 75 00 00 42 A7 4E B9 00 00 2E D4 L.L.Nu..B.N.....
00002ED0 58 4F 4E 75                                     XONu            

;; fn00002ED4: 00002ED4
fn00002ED4 proc
	movem.l	a2,-(a7)
	movea.l	$0008(a7),a2
	move.l	a2,d0
	beq	$00002EEC

l00002EE0:
	move.l	a2,-(a7)
	jsr.l	$00002B28
	addq.w	#$04,a7
	bra	$00002F12

l00002EEC:
	movea.l	$00003EEC,a2
	move.l	a2,d0
	beq	$00002F12

l00002EF6:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002F08

l00002EFE:
	move.l	a2,-(a7)
	jsr.l	$00002B28
	addq.w	#$04,a7

l00002F08:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00002EF6

l00002F12:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	rts

;; fn00002F18: 00002F18
;;   Called from:
;;     00001396 (in fn00001390)
;;     000013CE (in fn00001390)
fn00002F18 proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00003EF8,-(a7)
	jsr.l	$00002F34
	lea	$000C(a7),a7
	rts

;; fn00002F34: 00002F34
;;   Called from:
;;     00002F28 (in fn00002F18)
fn00002F34 proc
	lea	-$0044(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$007C(a7),d3
	movea.l	$0074(a7),a5
	movea.l	$0078(a7),a4
	moveq	#$00,d6
	tst.b	(a4)
	beq	$0000387A

l00002F50:
	cmpi.b	#$25,(a4)
	bne	$00003854

l00002F58:
	clr.l	$0040(a7)
	moveq	#-$01,d5
	clr.l	$0048(a7)
	moveq	#$69,d4
	lea	$004C(a7),a3
	moveq	#$00,d7
	clr.l	$0066(a7)
	lea	$0001(a4),a2
	move.l	$0048(a7),d2

l00002F76:
	moveq	#$00,d1

l00002F78:
	lea	0000388C,a0                                            ; $0914(pc)
	move.l	d0,-(a7)
	move.b	(a0,d1),d0
	cmp.b	(a2),d0
	movem.l	(a7)+,d0
	bne	$00002F9C

l00002F8A:
	move.l	d1,d0
	move.l	d1,-(a7)
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	move.l	(a7)+,d1
	or.l	d0,d2
	addq.l	#$01,a2
	bra	$00002FA6

l00002F9C:
	addq.l	#$01,d1
	cmp.l	#$00000005,d1
	bcs	$00002F78

l00002FA6:
	cmp.l	#$00000005,d1
	bcs	$00002F76

l00002FAE:
	move.l	d2,$0048(a7)
	cmpi.b	#$2A,(a2)
	bne	$00002FEC

l00002FB8:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	bge	$00002FE4

l00002FD0:
	ori.l	#$00000004,$0048(a7)
	move.l	$002C(a7),d0
	neg.l	d0
	move.l	d0,$0040(a7)
	bra	$00003058

l00002FE4:
	move.l	$002C(a7),$0040(a7)
	bra	$00003058

l00002FEC:
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00003058

l00003008:
	move.l	$0040(a7),d2

l0000300C:
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
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$0000300C

l00003054:
	move.l	d2,$0040(a7)

l00003058:
	cmpi.b	#$2E,(a2)
	bne	$000030EC

l00003060:
	addq.l	#$01,a2
	cmpi.b	#$2A,(a2)
	bne	$00003086

l00003068:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	blt	$000030EC

l00003080:
	move.l	$002C(a7),d5
	bra	$000030EC

l00003086:
	moveq	#$00,d5
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$000030EC

l000030A4:
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
	lea	$00002BED,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$000030A4

l000030EC:
	cmpi.b	#$68,(a2)
	beq	$00003110

l000030F2:
	cmpi.b	#$6C,(a2)
	beq	$00003110

l000030F8:
	cmpi.b	#$4C,(a2)
	beq	$00003110

l000030FE:
	cmpi.b	#$6A,(a2)
	beq	$00003110

l00003104:
	cmpi.b	#$7A,(a2)
	beq	$00003110

l0000310A:
	cmpi.b	#$74,(a2)
	bne	$00003116

l00003110:
	move.b	(a2)+,d4
	ext.w	d4
	ext.l	d4

l00003116:
	cmp.l	#$00000068,d4
	bne	$00003128

l0000311E:
	cmpi.b	#$68,(a2)
	bne	$00003128

l00003124:
	moveq	#$02,d4
	addq.l	#$01,a2

l00003128:
	cmp.l	#$0000006C,d4
	bne	$0000313A

l00003130:
	cmpi.b	#$6C,(a2)
	bne	$0000313A

l00003136:
	moveq	#$01,d4
	addq.l	#$01,a2

l0000313A:
	cmp.l	#$0000006A,d4
	bne	$00003144

l00003142:
	moveq	#$01,d4

l00003144:
	cmp.l	#$0000007A,d4
	bne	$0000314E

l0000314C:
	moveq	#$6C,d4

l0000314E:
	cmp.l	#$00000074,d4
	bne	$00003158

l00003156:
	moveq	#$69,d4

l00003158:
	move.b	(a2)+,d1
	move.b	d1,d0
	cmp.b	#$25,d1
	beq	$000035E0

l00003164:
	cmp.b	#$58,d0
	beq	$000031B2

l0000316A:
	cmp.b	#$63,d0
	beq	$00003560

l00003172:
	cmp.b	#$64,d0
	beq	$000031B2

l00003178:
	cmp.b	#$69,d0
	beq	$000031B2

l0000317E:
	move.b	d0,$002C(a7)
	cmp.b	#$6E,d0
	beq	$000035F2

l0000318A:
	move.b	$002C(a7),d0
	sub.b	#$6F,d0
	cmp.b	#$01,d0
	bls	$000031B2

l00003198:
	move.b	$002C(a7),d0
	cmp.b	#$73,d0
	beq	$0000359C

l000031A4:
	cmp.b	#$75,d0
	beq	$000031B2

l000031AA:
	cmp.b	#$78,d0
	bne	$0000368A

l000031B2:
	cmp.b	#$70,d1
	bne	$000031C4

l000031B8:
	moveq	#$6C,d4
	moveq	#$78,d1
	ori.l	#$00000001,$0048(a7)

l000031C4:
	cmp.b	#$64,d1
	beq	$000031D2

l000031CA:
	cmp.b	#$69,d1
	bne	$0000331C

l000031D2:
	cmp.l	#$00000001,d4
	bne	$000031F8

l000031DA:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$0034(a7)
	move.l	-$0008(a0),$0030(a7)
	bra	$00003290

l000031F8:
	cmp.l	#$0000006C,d4
	bne	$00003224

l00003200:
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
	bra	$00003290

l00003224:
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
	bne	$0000326A

l0000324E:
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

l0000326A:
	cmp.l	#$00000002,d4
	bne	$00003290

l00003272:
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

l00003290:
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
	bge	$000032DA

l000032BA:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2D,(a0)
	movem.l	$0030(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0038(a7)
	bra	$00003450

l000032DA:
	move.b	$002C(a7),d1
	moveq	#$10,d0
	and.l	$0048(a7),d0
	beq	$000032F4

l000032E6:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2B,(a0)
	bra	$00003308

l000032F4:
	moveq	#$08,d0
	and.l	$0048(a7),d0
	beq	$00003308

l000032FC:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$20,(a0)

l00003308:
	move.l	$0034(a7),$003C(a7)
	move.l	$0030(a7),$0038(a7)
	move.b	d1,$002C(a7)
	bra	$00003450

l0000331C:
	cmp.l	#$00000001,d4
	bne	$00003340

l00003324:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	move.l	-$0008(a0),$0038(a7)
	bra	$0000337A

l00003340:
	cmp.l	#$0000006C,d4
	bne	$00003362

l00003348:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)
	bra	$0000337A

l00003362:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)

l0000337A:
	cmp.l	#$00000068,d4
	bne	$00003396

l00003382:
	move.w	$003E(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.w	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l00003396:
	cmp.l	#$00000002,d4
	bne	$000033B2

l0000339E:
	move.b	$003F(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l000033B2:
	moveq	#$01,d0
	and.l	$0048(a7),d0
	move.b	d1,$002C(a7)
	tst.l	d0
	beq	$00003450

l000033C2:
	cmp.b	#$6F,d1
	bne	$000033FE

l000033C8:
	tst.l	d5
	bne	$000033F2

l000033CC:
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
	beq	$000033FE

l000033F2:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$30,(a0)

l000033FE:
	cmp.b	#$78,d1
	beq	$0000340E

l00003404:
	move.b	d1,$002C(a7)
	cmp.b	#$58,d1
	bne	$00003450

l0000340E:
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
	beq	$00003450

l00003438:
	lea	$006A(a7),a0
	lea	(a0,d7),a1
	addq.l	#$01,d7
	move.b	#$30,(a1)
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	d1,(a0)
	move.b	d1,$002C(a7)

l00003450:
	move.b	$002C(a7),d1
	lea	$0062(a7),a3
	cmp.b	#$78,d1
	beq	$00003464

l0000345E:
	cmp.b	#$58,d1
	bne	$0000346E

l00003464:
	move.l	#$00000010,$002C(a7)
	bra	$0000348C

l0000346E:
	cmp.b	#$6F,d1
	bne	$0000347E

l00003474:
	move.l	#$00000008,$0030(a7)
	bra	$00003486

l0000347E:
	move.l	#$0000000A,$0030(a7)

l00003486:
	move.l	$0030(a7),$002C(a7)

l0000348C:
	move.l	$002C(a7),$006C(a7)
	cmp.b	#$58,d1
	beq	$0000349E

l00003498:
	lea	00003894,a6                                            ; $03FC(pc)
	bra	$000034A2

l0000349E:
	lea	000038A4,a6                                            ; $0406(pc)

l000034A2:
	move.l	a6,$002C(a7)
	move.l	d3,$007C(a7)
	move.l	d5,$0044(a7)
	move.l	d6,$0030(a7)
	move.l	d7,$0062(a7)
	movem.l	$0038(a7),d6-d7
	move.l	$0066(a7),d3
	movea.l	$002C(a7),a1

l000034C4:
	move.l	$006C(a7),d1
	move.l	d1,d0
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	a1,-(a7)
	movem.l	d0-d1,-(a7)
	movem.l	d6-d7,-(a7)
	jsr.l	$00003C74
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
	jsr.l	$00003A24
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
	bne	$000034C4

l00003532:
	move.l	d3,$0066(a7)
	move.l	$0062(a7),d7
	move.l	$0030(a7),d6
	move.l	$0044(a7),d5
	move.l	$007C(a7),d3
	cmp.l	#$FFFFFFFF,d5
	bne	$00003554

l0000354E:
	moveq	#$00,d5
	bra	$000036A0

l00003554:
	andi.l	#$FFFFFFFD,$0048(a7)
	bra	$000036A0

l00003560:
	cmp.l	#$0000006C,d4
	bne	$0000357C

l00003568:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)
	bra	$0000358E

l0000357C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)

l0000358E:
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$000036A0

l0000359C:
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
	beq	$000035BE

l000035B8:
	cmp.l	$0066(a7),d5
	bls	$000035DA

l000035BE:
	tst.b	(a1)
	beq	$000035DA

l000035C2:
	move.l	$0066(a7),d0

l000035C6:
	addq.l	#$01,d0
	addq.l	#$01,a1
	tst.l	d5
	bls	$000035D2

l000035CE:
	cmp.l	d0,d5
	bls	$000035D6

l000035D2:
	tst.b	(a1)
	bne	$000035C6

l000035D6:
	move.l	d0,$0066(a7)

l000035DA:
	moveq	#$00,d5
	bra	$000036A0

l000035E0:
	lea	00003888,a3                                            ; $02A8(pc)
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$000036A0

l000035F2:
	cmp.l	#$00000001,d4
	bne	$00003614

l000035FA:
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
	bra	$00003682

l00003614:
	cmp.l	#$0000006C,d4
	bne	$00003632

l0000361C:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)
	bra	$00003682

l00003632:
	cmp.l	#$00000068,d4
	bne	$00003650

l0000363A:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.w	d6,(a0)
	bra	$00003682

l00003650:
	cmp.l	#$00000002,d4
	bne	$0000366E

l00003658:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.b	d6,(a0)
	bra	$00003682

l0000366E:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)

l00003682:
	moveq	#$00,d5
	clr.l	$0040(a7)
	bra	$000036A0

l0000368A:
	tst.b	d1
	bne	$00003690

l0000368E:
	subq.l	#$01,a2

l00003690:
	movea.l	a4,a3
	move.l	a2,d0
	sub.l	a4,d0
	move.l	d0,$0066(a7)
	moveq	#$00,d5
	clr.l	$0040(a7)

l000036A0:
	cmp.l	$0066(a7),d5
	bhi	$000036AE

l000036A6:
	move.l	$0066(a7),$002C(a7)
	bra	$000036B2

l000036AE:
	move.l	d5,$002C(a7)

l000036B2:
	move.l	d0,-(a7)
	move.l	$0030(a7),d0
	add.l	d7,d0
	move.l	d0,$0034(a7)
	move.l	(a7)+,d0
	move.l	d0,-(a7)
	move.l	$0034(a7),d0
	cmp.l	$0044(a7),d0
	movem.l	(a7)+,d0
	bcs	$000036D6

l000036D0:
	clr.l	$002C(a7)
	bra	$000036E6

l000036D6:
	move.l	d0,-(a7)
	move.l	$0044(a7),d0
	sub.l	$0034(a7),d0
	move.l	d0,$0030(a7)
	move.l	(a7)+,d0

l000036E6:
	move.l	$002C(a7),$0030(a7)
	moveq	#$02,d0
	and.l	$0048(a7),d0
	beq	$00003728

l000036F4:
	moveq	#$00,d2
	tst.l	d7
	beq	$00003728

l000036FA:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$000038B4
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00003720

l0000371A:
	move.l	d6,d0
	bra	$0000387C

l00003720:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$000036FA

l00003728:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	bne	$0000377A

l00003730:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$0000377A

l00003738:
	move.l	$0048(a7),d4
	movea.l	$0030(a7),a4

l00003740:
	move.l	a5,-(a7)
	moveq	#$02,d0
	and.l	d4,d0
	beq	$0000374E

l00003748:
	movea.w	#$0030,a0
	bra	$00003752

l0000374E:
	movea.w	#$0020,a0

l00003752:
	move.l	a0,-(a7)
	jsr.l	$000038B4
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$0000376A

l00003764:
	move.l	d6,d0
	bra	$0000387C

l0000376A:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00003740

l00003772:
	move.l	a4,$0030(a7)
	move.l	d4,$0048(a7)

l0000377A:
	moveq	#$02,d0
	and.l	$0048(a7),d0
	bne	$000037B6

l00003782:
	moveq	#$00,d2
	tst.l	d7
	beq	$000037B6

l00003788:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$000038B4
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$000037AE

l000037A8:
	move.l	d6,d0
	bra	$0000387C

l000037AE:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00003788

l000037B6:
	move.l	$0066(a7),d2
	cmp.l	$0066(a7),d5
	bls	$000037E4

l000037C0:
	move.l	a5,-(a7)
	pea	$00000030
	jsr.l	$000038B4
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$000037DC

l000037D6:
	move.l	d6,d0
	bra	$0000387C

l000037DC:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d5
	bhi	$000037C0

l000037E4:
	moveq	#$00,d2
	tst.l	$0066(a7)
	beq	$0000381A

l000037EC:
	movea.l	$0066(a7),a4

l000037F0:
	move.l	a5,-(a7)
	lea	(a3,d2),a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$000038B4
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00003812

l0000380E:
	move.l	d6,d0
	bra	$0000387C

l00003812:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$000037F0

l0000381A:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	beq	$00003850

l00003822:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00003850

l0000382A:
	movea.l	$0030(a7),a3

l0000382E:
	move.l	a5,-(a7)
	pea	$00000020
	jsr.l	$000038B4
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00003848

l00003844:
	move.l	d6,d0
	bra	$0000387C

l00003848:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a3
	bhi	$0000382E

l00003850:
	movea.l	a2,a4
	bra	$00003874

l00003854:
	move.l	a5,-(a7)
	move.b	(a4)+,d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	$000038B4
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00003872

l0000386E:
	move.l	d6,d0
	bra	$0000387C

l00003872:
	addq.l	#$01,d6

l00003874:
	tst.b	(a4)
	bne	$00002F50

l0000387A:
	move.l	d6,d0

l0000387C:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$0044(a7),a7
	rts
00003886                   00 00 25 00 00 00 23 30 2D 20       ..%...#0- 
00003890 2B 00 00 00 30 31 32 33 34 35 36 37 38 39 61 62 +...0123456789ab
000038A0 63 64 65 66 30 31 32 33 34 35 36 37 38 39 41 42 cdef0123456789AB
000038B0 43 44 45 46                                     CDEF            

;; fn000038B4: 000038B4
;;   Called from:
;;     0000370A (in fn00002F34)
;;     00003754 (in fn00002F34)
;;     00003798 (in fn00002F34)
;;     000037C6 (in fn00002F34)
;;     000037FE (in fn00002F34)
;;     00003834 (in fn00002F34)
;;     0000385E (in fn00002F34)
fn000038B4 proc
	movem.l	d2/a2-a3,-(a7)
	move.l	$0010(a7),d2
	movea.l	$0014(a7),a2
	lea	$0018(a2),a0
	moveq	#$02,d0
	or.l	d0,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000038F8

l000038D0:
	moveq	#$0A,d0
	cmp.l	d2,d0
	bne	$000038E2

l000038D6:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	bne	$000038F8

l000038E2:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a3
	addq.l	#$01,a3
	move.l	a3,(a1)
	move.b	d2,(a0)
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00003906

l000038F8:
	move.l	a2,-(a7)
	move.l	d2,-(a7)
	jsr.l	$00003910
	move.l	d0,d1
	addq.w	#$08,a7

l00003906:
	move.l	d1,d0
	movem.l	(a7)+,d2/a2-a3
	rts
0000390E                                           00 00               ..

;; fn00003910: 00003910
;;   Called from:
;;     000038FC (in fn000038B4)
fn00003910 proc
	movem.l	d2-d6/a2-a4/a6,-(a7)
	move.l	$0028(a7),d5
	movea.l	$002C(a7),a2
	jsr.l	$00002BBC
	move.l	a2,d0
	bne	$0000392C

l00003926:
	moveq	#-$01,d0
	bra	$00003A1E

l0000392C:
	moveq	#$49,d0
	and.l	$0018(a2),d0
	moveq	#$40,d6
	cmp.l	d0,d6
	beq	$0000393E

l00003938:
	moveq	#-$01,d0
	bra	$00003A1E

l0000393E:
	tst.l	$001C(a2)
	bne	$0000395C

l00003944:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00003954

l0000394C:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$0000395C

l00003954:
	move.l	#$00000400,$001C(a2)

l0000395C:
	tst.l	$0008(a2)
	bne	$0000399C

l00003962:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00003976

l00003972:
	moveq	#$02,d4
	bra	$00003978

l00003976:
	moveq	#$01,d4

l00003978:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	$00002718
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00003992

l0000398C:
	moveq	#-$01,d0
	bra	$00003A1E

l00003992:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)
	bra	$000039FA

l0000399C:
	tst.l	(a2)
	beq	$000039F6

l000039A0:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$000039CC

l000039AC:
	moveq	#$0A,d0
	cmp.l	d5,d0
	bne	$000039CC

l000039B2:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	d5,(a0)
	move.l	a2,-(a7)
	jsr.l	$00002B28
	addq.w	#$04,a7
	bra	$00003A1E

l000039CC:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003D98,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$000039FA

l000039EA:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$00003A1E

l000039F6:
	moveq	#$00,d0
	bra	$00003A1E

l000039FA:
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

l00003A1E:
	movem.l	(a7)+,d2-d6/a2-a4/a6
	rts

;; fn00003A24: 00003A24
;;   Called from:
;;     00003508 (in fn00002F34)
fn00003A24 proc
	movem.l	d2-d6,-(a7)
	move.l	$001C(a7),d1
	move.l	$0018(a7),d0
	movea.l	d1,a0
	move.l	$0024(a7),d3
	move.l	$0020(a7),d2
	bne	$00003A7A

l00003A3C:
	cmp.l	d3,d0
	bcc	$00003A4E

l00003A40:
	move.l	d3,d2
	jsr.l	$00003B28
	move.l	d0,d1
	bra	$00003B20

l00003A4E:
	tst.l	d3
	bne	$00003A5A

l00003A52:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l00003A5A:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	$00003B28
	movea.l	d0,a1
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	$00003B28
	move.l	d0,d1
	move.l	a1,d0
	bra	$00003B22

l00003A7A:
	cmp.l	d2,d0
	bcc	$00003A84

l00003A7E:
	moveq	#$00,d0
	bra	$00003B20

l00003A84:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00003AA2

l00003A8E:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00003AA2

l00003A96:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00003AA2

l00003A9E:
	moveq	#$00,d4
	move.b	d2,d6

l00003AA2:
	lea	$00003DCC,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$00003AC2

l00003AB6:
	cmp.l	d0,d2
	bcs	$00003ABE

l00003ABA:
	cmp.l	a0,d3
	bhi	$00003A7E

l00003ABE:
	moveq	#$01,d0
	bra	$00003B20

l00003AC2:
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
	jsr.l	$00003B28
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
	bcs	$00003B1E

l00003B18:
	bne	$00003B20

l00003B1A:
	cmpa.l	d2,a0
	bcc	$00003B20

l00003B1E:
	subq.l	#$01,d1

l00003B20:
	moveq	#$00,d0

l00003B22:
	movem.l	(a7)+,d2-d6
	rts

;; fn00003B28: 00003B28
;;   Called from:
;;     00003A42 (in fn00003A24)
;;     00003A60 (in fn00003A24)
;;     00003A6C (in fn00003A24)
;;     00003ADE (in fn00003A24)
;;     00003C92 (in fn00003C74)
;;     00003CB0 (in fn00003C74)
;;     00003CBA (in fn00003C74)
;;     00003D28 (in fn00003C74)
fn00003B28 proc
	movem.l	d5-d7,-(a7)
	move.l	d2,d7
	beq	$00003B42

l00003B30:
	move.l	d1,d6
	move.l	d0,d5
	bne	$00003B50

l00003B36:
	tst.l	d1
	beq	$00003C6E

l00003B3C:
	cmp.l	d1,d2
	bhi	$00003C6E

l00003B42:
	move.l	d1,d0
	move.l	d2,d1
	jsr.l	$0000257E
	bra	$00003C6E

l00003B50:
	swap.l	d2
	tst.w	d2
	bne	$00003B78

l00003B56:
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
	bra	$00003C6E

l00003B78:
	movem.l	d2-d4/a0-a1,-(a7)
	subq.l	#$08,a7
	clr.b	$0002(a7)
	moveq	#$00,d1
	moveq	#$00,d0
	tst.l	d7
	bmi	$00003B94

l00003B8A:
	addq.w	#$01,d0
	add.l	d6,d6
	addx.l	d5,d5
	add.l	d7,d7
	bpl	$00003B8A

l00003B94:
	move.w	d0,(a7)

l00003B96:
	move.l	d7,d3
	move.l	d5,d2
	swap.l	d2
	swap.l	d3
	cmp.w	d3,d2
	bne	$00003BA8

l00003BA2:
	move.w	#$FFFF,d1
	bra	$00003BB2

l00003BA8:
	move.l	d5,d1
	divu.w	d3,d1
	swap.l	d1
	clr.w	d1
	swap.l	d1

l00003BB2:
	movea.l	d6,a1
	clr.w	d6
	swap.l	d6

l00003BB8:
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
	bne	$00003BD8

l00003BD0:
	cmp.l	d4,d2
	bls	$00003BD8

l00003BD4:
	subq.l	#$01,d1
	bra	$00003BB8

l00003BD8:
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
	bcc	$00003C30

l00003C1A:
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

l00003C30:
	tst.b	$0002(a7)
	bne	$00003C4C

l00003C36:
	move.w	d1,$0004(a7)
	moveq	#$00,d1
	swap.l	d5
	swap.l	d6
	move.w	d6,d5
	clr.w	d6
	st	$0002(a7)
	bra	$00003B96

l00003C4C:
	move.l	$0004(a7),d0
	move.w	d1,d0
	move.w	d5,d6
	swap.l	d6
	swap.l	d5
	move.w	(a7),d7
	beq	$00003C66

l00003C5C:
	subq.w	#$01,d7

l00003C5E:
	lsr.l	#$01,d5
	roxr.l	#$01,d6
	dbra	d7,$00003C5E

l00003C66:
	move.l	d6,d1
	addq.l	#$08,a7
	movem.l	(a7)+,d2-d4/a0-a1

l00003C6E:
	movem.l	(a7)+,d5-d7
	rts

;; fn00003C74: 00003C74
;;   Called from:
;;     000034DC (in fn00002F34)
fn00003C74 proc
	movem.l	d2-d7,-(a7)
	move.l	$0020(a7),d1
	move.l	$001C(a7),d0
	movea.l	d1,a0
	move.l	$0028(a7),d3
	move.l	$0024(a7),d2
	bne	$00003CC6

l00003C8C:
	cmp.l	d3,d0
	bcc	$00003C9E

l00003C90:
	move.l	d3,d2
	jsr.l	$00003B28
	moveq	#$00,d0
	bra	$00003D80

l00003C9E:
	tst.l	d3
	bne	$00003CAA

l00003CA2:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l00003CAA:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	$00003B28
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	$00003B28
	moveq	#$00,d0
	bra	$00003D80

l00003CC6:
	cmp.l	d2,d0
	bcs	$00003D80

l00003CCC:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00003CEA

l00003CD6:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00003CEA

l00003CDE:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00003CEA

l00003CE6:
	moveq	#$00,d4
	move.b	d2,d6

l00003CEA:
	lea	$00003DCC,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$00003D0C

l00003CFE:
	cmp.l	d0,d2
	bcs	$00003D06

l00003D02:
	cmp.l	d1,d3
	bhi	$00003D80

l00003D06:
	sub.l	d3,d1
	subx.l	d2,d0
	bra	$00003D80

l00003D0C:
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
	jsr.l	$00003B28
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
	bcs	$00003D68

l00003D62:
	bne	$00003D6C

l00003D64:
	cmpa.l	d3,a0
	bcc	$00003D6C

l00003D68:
	sub.l	a1,d3
	subx.l	d0,d2

l00003D6C:
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

l00003D80:
	movem.l	(a7)+,d2-d7
	rts
00003D86                   00 00 00 24 00 63 41 00 00 00       ...$.cA...
00003D90 00 00 00 00 00 00 40 00 00 00 00 00 00 01 02 02 ......@.........
00003DA0 03 03 03 03 04 04 04 04 04 04 04 04 05 05 05 05 ................
00003DB0 05 05 05 05 05 05 05 05 05 05 05 05 06 06 06 06 ................
00003DC0 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 ................
00003DD0 06 06 06 06 06 06 06 06 06 06 06 06 07 07 07 07 ................
00003DE0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003DF0 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003E00 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003E10 07 07 07 07 07 07 07 07 07 07 07 07 08 08 08 08 ................
00003E20 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E30 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E40 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E50 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E60 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E70 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E80 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003E90 08 08 08 08 08 08 08 08 08 08 08 08 00 00 00 01 ................
00003EA0 00 00 2C F0 00 00 00 00 00 00 00 02 00 00 27 00 ..,...........'.
00003EB0 00 00 2E C8 00 00 00 00 00 00 00 00 00 00 00 00 ................
00003EC0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
