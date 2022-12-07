;;; Segment code (00001000)

;; fn00001000: 00001000
fn00001000 proc
	bra	$0000100A
00001002       56 42 43 43 20 30 2E 39                     VBCC 0.9      

l0000100A:
	move.l	d0,d2
	movea.l	a0,a2
	lea	$0000BE16,a4
	movea.l	$00000004,a6
	cmpi.w	#$0024,$0014(a6)
	bcc	$00001036

l00001020:
	lea	$00003E20,a0
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
	lea	$0000BE16,a4
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
	lea	$00003F68,a3
	move.l	#$00003F68,d0
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
;;     00002552 (in fn00002530)
fn0000131C proc
	movem.l	a2-a3,-(a7)
	tst.l	$00003E50
	bne	$0000134E

l00001328:
	movea.l	$00003F78,a3
	moveq	#$01,d0
	move.l	d0,$00003E50
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
	lea	$00003F60,a3
	move.l	#$00003F5C,d0
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
	movem.l	d2-d3,-(a7)
	lea	$0018(a7),a0
	move.l	a0,-(a7)
	pea	0000147C                                               ; $00DE(pc)
	jsr.l	fn00002BF0
	move.l	$0020(a7),-(a7)
	pea	00001480                                               ; $00D4(pc)
	jsr.l	fn000015A4
	lea	$0018(a7),a0
	move.l	a0,-(a7)
	pea	0000149C                                               ; $00E0(pc)
	jsr.l	fn00002BF0
	lea	$0024(a7),a0
	move.l	a0,-(a7)
	pea	000014A0                                               ; $00D4(pc)
	jsr.l	fn00002BF0
	moveq	#$01,d3
	lea	$0020(a7),a7
	bra	$0000145A

l000013DE:
	moveq	#$01,d2
	bra	$00001452

l000013E2:
	move.l	$0008(a7),d0
	add.l	$000C(a7),d0
	move.l	d0,d1
	add.l	d2,d1
	move.l	d1,$0008(a7)
	move.l	$0008(a7),d0
	asr.l	#$01,d0
	move.l	d0,$000C(a7)
	pea	$0000000A
	move.l	$0010(a7),-(a7)
	jsr.l	fn000014AC
	addq.w	#$08,a7
	move.l	d0,$0008(a7)
	cmp.l	$000C(a7),d2
	beq	$0000141A

l00001416:
	moveq	#$00,d0
	bra	$0000141C

l0000141A:
	moveq	#$01,d0

l0000141C:
	move.l	d0,$0008(a7)
	move.l	$0008(a7),d0
	or.l	d2,d0
	move.l	d0,$000C(a7)
	bne	$00001430

l0000142C:
	moveq	#$01,d0
	bra	$00001432

l00001430:
	moveq	#$00,d0

l00001432:
	move.l	d0,$0008(a7)
	move.l	$0008(a7),d0
	add.l	d2,d0
	move.l	d0,$000C(a7)
	cmp.l	$000C(a7),d2
	blt	$0000144A

l00001446:
	moveq	#$00,d0
	bra	$0000144C

l0000144A:
	moveq	#$01,d0

l0000144C:
	move.l	d0,$0008(a7)
	addq.l	#$01,d2

l00001452:
	moveq	#$28,d0
	cmp.l	d2,d0
	bge	$000013E2

l00001458:
	addq.l	#$01,d3

l0000145A:
	cmp.l	$0018(a7),d3
	ble	$000013DE

l00001462:
	move.l	$0008(a7),-(a7)
	pea	000014A4                                               ; $003E(pc)
	jsr.l	fn000015A4
	addq.w	#$08,a7
	movem.l	(a7)+,d2-d3
	lea	$0014(a7),a7
	rts
0000147C                                     25 6C 64 00             %ld.
00001480 65 78 65 63 75 74 69 6E 67 20 25 6C 64 20 69 74 executing %ld it
00001490 65 72 61 74 69 6F 6E 73 0A 00 00 00 25 6C 64 00 erations....%ld.
000014A0 25 6C 64 00 61 3D 25 64 0A 00 00 00             %ld.a=%d....    

;; fn000014AC: 000014AC
;;   Called from:
;;     00001404 (in fn00001390)
fn000014AC proc
	movem.l	$0004(a7),d0-d1
	tst.l	d1
	bmi	$000014C0

l000014B6:
	tst.l	d0
	bmi	$000014CC

l000014BA:
	bsr	fn0000151E
	move.l	d1,d0
	rts

l000014C0:
	neg.l	d1
	tst.l	d0
	bmi	$000014D6

l000014C6:
	bsr	fn0000151E
	move.l	d1,d0
	rts

l000014CC:
	neg.l	d0
	bsr	fn0000151E
	neg.l	d1
	move.l	d1,d0
	rts

l000014D6:
	neg.l	d0
	bsr	fn0000151E
	neg.l	d1
	move.l	d1,d0
	rts
000014E0 4C EF 00 03 00 04 61 36 20 01 4E 75             L.....a6 .Nu    

;; fn000014EC: 000014EC
;;   Called from:
;;     000030AA (in fn00002C3C)
;;     0000312E (in fn00002C3C)
fn000014EC proc
	movem.l	$0004(a7),d0-d1
	tst.l	d0
	bpl	$0000150C

l000014F6:
	neg.l	d0
	tst.l	d1
	bpl	$00001504

l000014FC:
	neg.l	d1
	bsr	fn0000151E
	neg.l	d1
	rts

l00001504:
	bsr	fn0000151E
	neg.l	d0
	neg.l	d1
	rts

l0000150C:
	tst.l	d1
	bpl	fn0000151E

l00001510:
	neg.l	d1
	bsr	fn0000151E
	neg.l	d0
	rts
00001518                         4C EF 00 03 00 04               L.....  

;; fn0000151E: 0000151E
;;   Called from:
;;     000014BA (in fn000014AC)
;;     000014C6 (in fn000014AC)
;;     000014CE (in fn000014AC)
;;     000014D8 (in fn000014AC)
;;     000014FE (in fn000014EC)
;;     00001504 (in fn000014EC)
;;     0000150E (in fn000014EC)
;;     00001512 (in fn000014EC)
;;     00002682 (in fn00002664)
fn0000151E proc
	move.l	d2,-(a7)
	swap.l	d1
	move.w	d1,d2
	bne	$00001544

l00001526:
	swap.l	d0
	swap.l	d1
	swap.l	d2
	move.w	d0,d2
	beq	$00001534

l00001530:
	divu.w	d1,d2
	move.w	d2,d0

l00001534:
	swap.l	d0
	move.w	d0,d2
	divu.w	d1,d2
	move.w	d2,d0
	swap.l	d2
	move.w	d2,d1
	move.l	(a7)+,d2
	rts

l00001544:
	move.l	d3,-(a7)
	moveq	#$10,d3
	cmp.w	#$0080,d1
	bcc	$00001552

l0000154E:
	rol.l	#$08,d1
	subq.w	#$08,d3

l00001552:
	cmp.w	#$0800,d1
	bcc	$0000155C

l00001558:
	rol.l	#$04,d1
	subq.w	#$04,d3

l0000155C:
	cmp.w	#$2000,d1
	bcc	$00001566

l00001562:
	rol.l	#$02,d1
	subq.w	#$02,d3

l00001566:
	tst.w	d1
	bmi	$0000156E

l0000156A:
	rol.l	#$01,d1
	subq.w	#$01,d3

l0000156E:
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
	bcc	$00001590

l0000158A:
	subq.w	#$01,d3
	add.l	d1,d0

l0000158E:
	bcc	$0000158E

l00001590:
	moveq	#$00,d1
	move.w	d3,d1
	swap.l	d3
	rol.l	d3,d0
	swap.l	d0
	exg	d0,d1
	move.l	(a7)+,d3
	move.l	(a7)+,d2
	rts
000015A2       00 00                                       ..            

;; fn000015A4: 000015A4
;;   Called from:
;;     000013B0 (in fn00001390)
;;     0000146A (in fn00001390)
fn000015A4 proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00003F80,-(a7)
	jsr.l	fn000015C0
	lea	$000C(a7),a7
	rts

;; fn000015C0: 000015C0
;;   Called from:
;;     000015B4 (in fn000015A4)
fn000015C0 proc
	lea	-$0044(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$007C(a7),d3
	movea.l	$0074(a7),a5
	movea.l	$0078(a7),a4
	moveq	#$00,d6
	tst.b	(a4)
	beq	$00001F06

l000015DC:
	cmpi.b	#$25,(a4)
	bne	$00001EE0

l000015E4:
	clr.l	$0040(a7)
	moveq	#-$01,d5
	clr.l	$0048(a7)
	moveq	#$69,d4
	lea	$004C(a7),a3
	moveq	#$00,d7
	clr.l	$0066(a7)
	lea	$0001(a4),a2
	move.l	$0048(a7),d2

l00001602:
	moveq	#$00,d1

l00001604:
	lea	00001F18,a0                                            ; $0914(pc)
	move.l	d0,-(a7)
	move.b	(a0,d1),d0
	cmp.b	(a2),d0
	movem.l	(a7)+,d0
	bne	$00001628

l00001616:
	move.l	d1,d0
	move.l	d1,-(a7)
	moveq	#$01,d1
	lsl.l	d0,d1
	move.l	d1,d0
	move.l	(a7)+,d1
	or.l	d0,d2
	addq.l	#$01,a2
	bra	$00001632

l00001628:
	addq.l	#$01,d1
	cmp.l	#$00000005,d1
	bcs	$00001604

l00001632:
	cmp.l	#$00000005,d1
	bcs	$00001602

l0000163A:
	move.l	d2,$0048(a7)
	cmpi.b	#$2A,(a2)
	bne	$00001678

l00001644:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	bge	$00001670

l0000165C:
	ori.l	#$00000004,$0048(a7)
	move.l	$002C(a7),d0
	neg.l	d0
	move.l	d0,$0040(a7)
	bra	$000016E4

l00001670:
	move.l	$002C(a7),$0040(a7)
	bra	$000016E4

l00001678:
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$000016E4

l00001694:
	move.l	$0040(a7),d2

l00001698:
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
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00001698

l000016E0:
	move.l	d2,$0040(a7)

l000016E4:
	cmpi.b	#$2E,(a2)
	bne	$00001778

l000016EC:
	addq.l	#$01,a2
	cmpi.b	#$2A,(a2)
	bne	$00001712

l000016F4:
	addq.l	#$01,a2
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$002C(a7)
	blt	$00001778

l0000170C:
	move.l	$002C(a7),d5
	bra	$00001778

l00001712:
	moveq	#$00,d5
	move.b	(a2),d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00001778

l00001730:
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
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00001730

l00001778:
	cmpi.b	#$68,(a2)
	beq	$0000179C

l0000177E:
	cmpi.b	#$6C,(a2)
	beq	$0000179C

l00001784:
	cmpi.b	#$4C,(a2)
	beq	$0000179C

l0000178A:
	cmpi.b	#$6A,(a2)
	beq	$0000179C

l00001790:
	cmpi.b	#$7A,(a2)
	beq	$0000179C

l00001796:
	cmpi.b	#$74,(a2)
	bne	$000017A2

l0000179C:
	move.b	(a2)+,d4
	ext.w	d4
	ext.l	d4

l000017A2:
	cmp.l	#$00000068,d4
	bne	$000017B4

l000017AA:
	cmpi.b	#$68,(a2)
	bne	$000017B4

l000017B0:
	moveq	#$02,d4
	addq.l	#$01,a2

l000017B4:
	cmp.l	#$0000006C,d4
	bne	$000017C6

l000017BC:
	cmpi.b	#$6C,(a2)
	bne	$000017C6

l000017C2:
	moveq	#$01,d4
	addq.l	#$01,a2

l000017C6:
	cmp.l	#$0000006A,d4
	bne	$000017D0

l000017CE:
	moveq	#$01,d4

l000017D0:
	cmp.l	#$0000007A,d4
	bne	$000017DA

l000017D8:
	moveq	#$6C,d4

l000017DA:
	cmp.l	#$00000074,d4
	bne	$000017E4

l000017E2:
	moveq	#$69,d4

l000017E4:
	move.b	(a2)+,d1
	move.b	d1,d0
	cmp.b	#$25,d1
	beq	$00001C6C

l000017F0:
	cmp.b	#$58,d0
	beq	$0000183E

l000017F6:
	cmp.b	#$63,d0
	beq	$00001BEC

l000017FE:
	cmp.b	#$64,d0
	beq	$0000183E

l00001804:
	cmp.b	#$69,d0
	beq	$0000183E

l0000180A:
	move.b	d0,$002C(a7)
	cmp.b	#$6E,d0
	beq	$00001C7E

l00001816:
	move.b	$002C(a7),d0
	sub.b	#$6F,d0
	cmp.b	#$01,d0
	bls	$0000183E

l00001824:
	move.b	$002C(a7),d0
	cmp.b	#$73,d0
	beq	$00001C28

l00001830:
	cmp.b	#$75,d0
	beq	$0000183E

l00001836:
	cmp.b	#$78,d0
	bne	$00001D16

l0000183E:
	cmp.b	#$70,d1
	bne	$00001850

l00001844:
	moveq	#$6C,d4
	moveq	#$78,d1
	ori.l	#$00000001,$0048(a7)

l00001850:
	cmp.b	#$64,d1
	beq	$0000185E

l00001856:
	cmp.b	#$69,d1
	bne	$000019A8

l0000185E:
	cmp.l	#$00000001,d4
	bne	$00001884

l00001866:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$0034(a7)
	move.l	-$0008(a0),$0030(a7)
	bra	$0000191C

l00001884:
	cmp.l	#$0000006C,d4
	bne	$000018B0

l0000188C:
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
	bra	$0000191C

l000018B0:
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
	bne	$000018F6

l000018DA:
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

l000018F6:
	cmp.l	#$00000002,d4
	bne	$0000191C

l000018FE:
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

l0000191C:
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
	bge	$00001966

l00001946:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2D,(a0)
	movem.l	$0030(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0038(a7)
	bra	$00001ADC

l00001966:
	move.b	$002C(a7),d1
	moveq	#$10,d0
	and.l	$0048(a7),d0
	beq	$00001980

l00001972:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$2B,(a0)
	bra	$00001994

l00001980:
	moveq	#$08,d0
	and.l	$0048(a7),d0
	beq	$00001994

l00001988:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$20,(a0)

l00001994:
	move.l	$0034(a7),$003C(a7)
	move.l	$0030(a7),$0038(a7)
	move.b	d1,$002C(a7)
	bra	$00001ADC

l000019A8:
	cmp.l	#$00000001,d4
	bne	$000019CC

l000019B0:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$08,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	move.l	-$0008(a0),$0038(a7)
	bra	$00001A06

l000019CC:
	cmp.l	#$0000006C,d4
	bne	$000019EE

l000019D4:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)
	bra	$00001A06

l000019EE:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.l	-$0004(a0),$003C(a7)
	clr.l	$0038(a7)

l00001A06:
	cmp.l	#$00000068,d4
	bne	$00001A22

l00001A0E:
	move.w	$003E(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.w	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l00001A22:
	cmp.l	#$00000002,d4
	bne	$00001A3E

l00001A2A:
	move.b	$003F(a7),d0
	move.l	d1,-(a7)
	moveq	#$00,d1
	move.b	d0,d1
	move.l	d1,$0040(a7)
	clr.l	$003C(a7)
	move.l	(a7)+,d1

l00001A3E:
	moveq	#$01,d0
	and.l	$0048(a7),d0
	move.b	d1,$002C(a7)
	tst.l	d0
	beq	$00001ADC

l00001A4E:
	cmp.b	#$6F,d1
	bne	$00001A8A

l00001A54:
	tst.l	d5
	bne	$00001A7E

l00001A58:
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
	beq	$00001A8A

l00001A7E:
	lea	$006A(a7),a0
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	#$30,(a0)

l00001A8A:
	cmp.b	#$78,d1
	beq	$00001A9A

l00001A90:
	move.b	d1,$002C(a7)
	cmp.b	#$58,d1
	bne	$00001ADC

l00001A9A:
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
	beq	$00001ADC

l00001AC4:
	lea	$006A(a7),a0
	lea	(a0,d7),a1
	addq.l	#$01,d7
	move.b	#$30,(a1)
	adda.l	d7,a0
	addq.l	#$01,d7
	move.b	d1,(a0)
	move.b	d1,$002C(a7)

l00001ADC:
	move.b	$002C(a7),d1
	lea	$0062(a7),a3
	cmp.b	#$78,d1
	beq	$00001AF0

l00001AEA:
	cmp.b	#$58,d1
	bne	$00001AFA

l00001AF0:
	move.l	#$00000010,$002C(a7)
	bra	$00001B18

l00001AFA:
	cmp.b	#$6F,d1
	bne	$00001B0A

l00001B00:
	move.l	#$00000008,$0030(a7)
	bra	$00001B12

l00001B0A:
	move.l	#$0000000A,$0030(a7)

l00001B12:
	move.l	$0030(a7),$002C(a7)

l00001B18:
	move.l	$002C(a7),$006C(a7)
	cmp.b	#$58,d1
	beq	$00001B2A

l00001B24:
	lea	00001F20,a6                                            ; $03FC(pc)
	bra	$00001B2E

l00001B2A:
	lea	00001F30,a6                                            ; $0406(pc)

l00001B2E:
	move.l	a6,$002C(a7)
	move.l	d3,$007C(a7)
	move.l	d5,$0044(a7)
	move.l	d6,$0030(a7)
	move.l	d7,$0062(a7)
	movem.l	$0038(a7),d6-d7
	move.l	$0066(a7),d3
	movea.l	$002C(a7),a1

l00001B50:
	move.l	$006C(a7),d1
	move.l	d1,d0
	moveq	#$1F,d2
	asr.l	d2,d0
	move.l	d0,-(a7)
	move.l	d1,-(a7)
	move.l	a1,-(a7)
	movem.l	d0-d1,-(a7)
	movem.l	d6-d7,-(a7)
	jsr.l	fn000027B0
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
	jsr.l	fn00002560
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
	bne	$00001B50

l00001BBE:
	move.l	d3,$0066(a7)
	move.l	$0062(a7),d7
	move.l	$0030(a7),d6
	move.l	$0044(a7),d5
	move.l	$007C(a7),d3
	cmp.l	#$FFFFFFFF,d5
	bne	$00001BE0

l00001BDA:
	moveq	#$00,d5
	bra	$00001D2C

l00001BE0:
	andi.l	#$FFFFFFFD,$0048(a7)
	bra	$00001D2C

l00001BEC:
	cmp.l	#$0000006C,d4
	bne	$00001C08

l00001BF4:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)
	bra	$00001C1A

l00001C08:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	move.b	-$0001(a0),(a3)

l00001C1A:
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001D2C

l00001C28:
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
	beq	$00001C4A

l00001C44:
	cmp.l	$0066(a7),d5
	bls	$00001C66

l00001C4A:
	tst.b	(a1)
	beq	$00001C66

l00001C4E:
	move.l	$0066(a7),d0

l00001C52:
	addq.l	#$01,d0
	addq.l	#$01,a1
	tst.l	d5
	bls	$00001C5E

l00001C5A:
	cmp.l	d0,d5
	bls	$00001C62

l00001C5E:
	tst.b	(a1)
	bne	$00001C52

l00001C62:
	move.l	d0,$0066(a7)

l00001C66:
	moveq	#$00,d5
	bra	$00001D2C

l00001C6C:
	lea	00001F14,a3                                            ; $02A8(pc)
	move.l	#$00000001,$0066(a7)
	moveq	#$00,d5
	bra	$00001D2C

l00001C7E:
	cmp.l	#$00000001,d4
	bne	$00001CA0

l00001C86:
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
	bra	$00001D0E

l00001CA0:
	cmp.l	#$0000006C,d4
	bne	$00001CBE

l00001CA8:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)
	bra	$00001D0E

l00001CBE:
	cmp.l	#$00000068,d4
	bne	$00001CDC

l00001CC6:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.w	d6,(a0)
	bra	$00001D0E

l00001CDC:
	cmp.l	#$00000002,d4
	bne	$00001CFA

l00001CE4:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.b	d6,(a0)
	bra	$00001D0E

l00001CFA:
	move.l	d3,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d3
	addq.l	#$04,d3
	movea.l	d3,a0
	movea.l	-$0004(a0),a0
	move.l	d6,(a0)

l00001D0E:
	moveq	#$00,d5
	clr.l	$0040(a7)
	bra	$00001D2C

l00001D16:
	tst.b	d1
	bne	$00001D1C

l00001D1A:
	subq.l	#$01,a2

l00001D1C:
	movea.l	a4,a3
	move.l	a2,d0
	sub.l	a4,d0
	move.l	d0,$0066(a7)
	moveq	#$00,d5
	clr.l	$0040(a7)

l00001D2C:
	cmp.l	$0066(a7),d5
	bhi	$00001D3A

l00001D32:
	move.l	$0066(a7),$002C(a7)
	bra	$00001D3E

l00001D3A:
	move.l	d5,$002C(a7)

l00001D3E:
	move.l	d0,-(a7)
	move.l	$0030(a7),d0
	add.l	d7,d0
	move.l	d0,$0034(a7)
	move.l	(a7)+,d0
	move.l	d0,-(a7)
	move.l	$0034(a7),d0
	cmp.l	$0044(a7),d0
	movem.l	(a7)+,d0
	bcs	$00001D62

l00001D5C:
	clr.l	$002C(a7)
	bra	$00001D72

l00001D62:
	move.l	d0,-(a7)
	move.l	$0044(a7),d0
	sub.l	$0034(a7),d0
	move.l	d0,$0030(a7)
	move.l	(a7)+,d0

l00001D72:
	move.l	$002C(a7),$0030(a7)
	moveq	#$02,d0
	and.l	$0048(a7),d0
	beq	$00001DB4

l00001D80:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001DB4

l00001D86:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00001F40
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001DAC

l00001DA6:
	move.l	d6,d0
	bra	$00001F08

l00001DAC:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001D86

l00001DB4:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	bne	$00001E06

l00001DBC:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001E06

l00001DC4:
	move.l	$0048(a7),d4
	movea.l	$0030(a7),a4

l00001DCC:
	move.l	a5,-(a7)
	moveq	#$02,d0
	and.l	d4,d0
	beq	$00001DDA

l00001DD4:
	movea.w	#$0030,a0
	bra	$00001DDE

l00001DDA:
	movea.w	#$0020,a0

l00001DDE:
	move.l	a0,-(a7)
	jsr.l	fn00001F40
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001DF6

l00001DF0:
	move.l	d6,d0
	bra	$00001F08

l00001DF6:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001DCC

l00001DFE:
	move.l	a4,$0030(a7)
	move.l	d4,$0048(a7)

l00001E06:
	moveq	#$02,d0
	and.l	$0048(a7),d0
	bne	$00001E42

l00001E0E:
	moveq	#$00,d2
	tst.l	d7
	beq	$00001E42

l00001E14:
	move.l	a5,-(a7)
	lea	$006E(a7),a0
	adda.l	d2,a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00001F40
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001E3A

l00001E34:
	move.l	d6,d0
	bra	$00001F08

l00001E3A:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d7
	bhi	$00001E14

l00001E42:
	move.l	$0066(a7),d2
	cmp.l	$0066(a7),d5
	bls	$00001E70

l00001E4C:
	move.l	a5,-(a7)
	pea	$00000030
	jsr.l	fn00001F40
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001E68

l00001E62:
	move.l	d6,d0
	bra	$00001F08

l00001E68:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmp.l	d2,d5
	bhi	$00001E4C

l00001E70:
	moveq	#$00,d2
	tst.l	$0066(a7)
	beq	$00001EA6

l00001E78:
	movea.l	$0066(a7),a4

l00001E7C:
	move.l	a5,-(a7)
	lea	(a3,d2),a0
	move.b	(a0),d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00001F40
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001E9E

l00001E9A:
	move.l	d6,d0
	bra	$00001F08

l00001E9E:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a4
	bhi	$00001E7C

l00001EA6:
	moveq	#$04,d0
	and.l	$0048(a7),d0
	beq	$00001EDC

l00001EAE:
	moveq	#$00,d2
	tst.l	$0030(a7)
	beq	$00001EDC

l00001EB6:
	movea.l	$0030(a7),a3

l00001EBA:
	move.l	a5,-(a7)
	pea	$00000020
	jsr.l	fn00001F40
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001ED4

l00001ED0:
	move.l	d6,d0
	bra	$00001F08

l00001ED4:
	addq.l	#$01,d6
	addq.l	#$01,d2
	cmpa.l	d2,a3
	bhi	$00001EBA

l00001EDC:
	movea.l	a2,a4
	bra	$00001F00

l00001EE0:
	move.l	a5,-(a7)
	move.b	(a4)+,d0
	ext.w	d0
	ext.l	d0
	move.l	d0,-(a7)
	jsr.l	fn00001F40
	addq.w	#$08,a7
	cmp.l	#$FFFFFFFF,d0
	bne	$00001EFE

l00001EFA:
	move.l	d6,d0
	bra	$00001F08

l00001EFE:
	addq.l	#$01,d6

l00001F00:
	tst.b	(a4)
	bne	$000015DC

l00001F06:
	move.l	d6,d0

l00001F08:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$0044(a7),a7
	rts
00001F12       00 00 25 00 00 00 23 30 2D 20 2B 00 00 00   ..%...#0- +...
00001F20 30 31 32 33 34 35 36 37 38 39 61 62 63 64 65 66 0123456789abcdef
00001F30 30 31 32 33 34 35 36 37 38 39 41 42 43 44 45 46 0123456789ABCDEF

;; fn00001F40: 00001F40
;;   Called from:
;;     00001D96 (in fn000015C0)
;;     00001DE0 (in fn000015C0)
;;     00001E24 (in fn000015C0)
;;     00001E52 (in fn000015C0)
;;     00001E8A (in fn000015C0)
;;     00001EC0 (in fn000015C0)
;;     00001EEA (in fn000015C0)
fn00001F40 proc
	movem.l	d2/a2-a3,-(a7)
	move.l	$0010(a7),d2
	movea.l	$0014(a7),a2
	lea	$0018(a2),a0
	moveq	#$02,d0
	or.l	d0,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00001F84

l00001F5C:
	moveq	#$0A,d0
	cmp.l	d2,d0
	bne	$00001F6E

l00001F62:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	bne	$00001F84

l00001F6E:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a3
	addq.l	#$01,a3
	move.l	a3,(a1)
	move.b	d2,(a0)
	move.b	(a0),d0
	moveq	#$00,d1
	move.b	d0,d1
	bra	$00001F92

l00001F84:
	move.l	a2,-(a7)
	move.l	d2,-(a7)
	jsr.l	fn00001F9C
	move.l	d0,d1
	addq.w	#$08,a7

l00001F92:
	move.l	d1,d0
	movem.l	(a7)+,d2/a2-a3
	rts
00001F9A                               00 00                       ..    

;; fn00001F9C: 00001F9C
;;   Called from:
;;     00001F88 (in fn00001F40)
fn00001F9C proc
	movem.l	d2-d6/a2-a4/a6,-(a7)
	move.l	$0028(a7),d5
	movea.l	$002C(a7),a2
	jsr.l	fn00002530
	move.l	a2,d0
	bne	$00001FB8

l00001FB2:
	moveq	#-$01,d0
	bra	$000020AA

l00001FB8:
	moveq	#$49,d0
	and.l	$0018(a2),d0
	moveq	#$40,d6
	cmp.l	d0,d6
	beq	$00001FCA

l00001FC4:
	moveq	#-$01,d0
	bra	$000020AA

l00001FCA:
	tst.l	$001C(a2)
	bne	$00001FE8

l00001FD0:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00001FE0

l00001FD8:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00001FE8

l00001FE0:
	move.l	#$00000400,$001C(a2)

l00001FE8:
	tst.l	$0008(a2)
	bne	$00002028

l00001FEE:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00002002

l00001FFE:
	moveq	#$02,d4
	bra	$00002004

l00002002:
	moveq	#$01,d4

l00002004:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	fn0000215C
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$0000201E

l00002018:
	moveq	#-$01,d0
	bra	$000020AA

l0000201E:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)
	bra	$00002086

l00002028:
	tst.l	(a2)
	beq	$00002082

l0000202C:
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00002058

l00002038:
	moveq	#$0A,d0
	cmp.l	d5,d0
	bne	$00002058

l0000203E:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	d5,(a0)
	move.l	a2,-(a7)
	jsr.l	fn000020B0
	addq.w	#$04,a7
	bra	$000020AA

l00002058:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003E28,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00002086

l00002076:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$000020AA

l00002082:
	moveq	#$00,d0
	bra	$000020AA

l00002086:
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

l000020AA:
	movem.l	(a7)+,d2-d6/a2-a4/a6
	rts

;; fn000020B0: 000020B0
;;   Called from:
;;     0000204E (in fn00001F9C)
;;     00002BBA (in fn00002BAC)
;;     00002BD8 (in fn00002BAC)
;;     00003E02 (in fn00003DDC)
fn000020B0 proc
	movem.l	d2-d4/a2/a6,-(a7)
	movea.l	$0018(a7),a2
	jsr.l	fn00002530
	move.l	a2,d0
	bne	$000020C6

l000020C2:
	moveq	#-$01,d0
	bra	$0000213E

l000020C6:
	tst.l	$001C(a2)
	bne	$000020E4

l000020CC:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$000020DC

l000020D4:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$000020E4

l000020DC:
	move.l	#$00000400,$001C(a2)

l000020E4:
	tst.l	$0008(a2)
	bne	$000020EE

l000020EA:
	moveq	#$00,d0
	bra	$0000213E

l000020EE:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002134

l000020F6:
	tst.l	(a2)
	beq	$00002124

l000020FA:
	lea	$0008(a2),a0
	move.l	$0004(a2),d4
	sub.l	(a0),d4
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003E28,a6
	move.l	d4,d3
	jsr.l	-$0030(a6)
	cmp.l	d0,d4
	beq	$00002128

l00002118:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)
	moveq	#-$01,d0
	bra	$0000213E

l00002124:
	moveq	#$00,d0
	bra	$0000213E

l00002128:
	move.l	$0008(a2),$0004(a2)
	move.l	$001C(a2),$0014(a2)

l00002134:
	lea	$0018(a2),a0
	moveq	#-$04,d0
	and.l	d0,(a0)
	moveq	#$00,d0

l0000213E:
	movem.l	(a7)+,d2-d4/a2/a6
	rts
00002144             4A B9 00 00 3E 58 67 0E 2F 39 00 00     J...>Xg./9..
00002150 3E 58 4E B9 00 00 24 C0 58 4F 4E 75             >XN...$.XONu    

;; fn0000215C: 0000215C
;;   Called from:
;;     0000200A (in fn00001F9C)
;;     00003D62 (in fn00003CE0)
fn0000215C proc
	movem.l	d2,-(a7)
	move.l	$0008(a7),d2
	bne	$0000216A

l00002166:
	moveq	#$00,d0
	bra	$000021C0

l0000216A:
	tst.l	$00003E58
	bne	$0000218E

l00002172:
	movea.l	$00003E54,a0
	move.l	a0,-(a7)
	move.l	a0,-(a7)
	clr.l	-(a7)
	jsr.l	fn00002450
	move.l	d0,$00003E58
	lea	$000C(a7),a7

l0000218E:
	tst.l	$00003E58
	bne	$0000219A

l00002196:
	moveq	#$00,d0
	bra	$000021C0

l0000219A:
	moveq	#$04,d0
	add.l	d2,d0
	move.l	d0,-(a7)
	move.l	$00003E58,-(a7)
	jsr.l	fn0000232C
	movea.l	d0,a1
	addq.w	#$08,a7
	move.l	a1,d0
	bne	$000021B8

l000021B4:
	moveq	#$00,d0
	bra	$000021C0

l000021B8:
	move.l	d2,(a1)
	lea	$0004(a1),a0
	move.l	a0,d0

l000021C0:
	movem.l	(a7)+,d2
	rts
000021C6                   00 00                               ..        

;; fn000021C8: 000021C8
fn000021C8 proc
	move.l	$0004(a7),d0
	movea.l	d0,a0
	tst.l	d0
	beq	$000021F2

l000021D2:
	tst.l	$00003E58
	beq	$000021F2

l000021DA:
	moveq	#$04,d0
	add.l	-(a0),d0
	move.l	d0,-(a7)
	move.l	a0,-(a7)
	move.l	$00003E58,-(a7)
	jsr.l	fn00002290
	lea	$000C(a7),a7

l000021F2:
	rts
000021F4             48 E7 30 38 28 6F 00 1C 24 6F 00 18     H.08(o..$o..
00002200 22 0A 66 0A 2F 0C 61 00 FF 54 58 4F 60 7A 26 6A ".f./.a..TXO`z&j
00002210 FF FC 2F 0C 61 00 FF 46 26 00 58 4F 67 68 B9 CB ../.a..F&.XOgh..
00002220 64 04 20 0C 60 02 20 0B 20 43 22 4A 24 00 B4 BC d. .`. . C"J$...
00002230 00 00 00 10 65 3C 20 08 22 09 C0 3C 00 01 C2 3C ....e< ."..<...<
00002240 00 01 B2 00 66 1A 20 08 4A 01 67 04 10 D9 53 82 ....f. .J.g...S.
00002250 72 03 C2 82 94 81 20 D9 59 82 66 FA 34 01 60 14 r..... .Y.f.4.`.
00002260 B4 BC 00 01 00 00 65 0A 20 08 10 D9 53 82 66 FA ......e. ...S.f.
00002270 60 0C 20 08 53 42 65 06 10 D9 51 CA FF FC 2F 0A `. .SBe...Q.../.
00002280 61 00 FF 46 58 4F 20 03 4C DF 1C 0C 4E 75 00 00 a..FXO .L...Nu..

;; fn00002290: 00002290
;;   Called from:
;;     000021E8 (in fn000021C8)
fn00002290 proc
	movem.l	d2/a2-a6,-(a7)
	move.l	$0020(a7),d1
	movea.l	$0024(a7),a5
	movea.l	$001C(a7),a4
	movea.l	$00003E24,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$000022C0

l000022AE:
	movea.l	$00003E24,a6
	movea.l	a4,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$02CA(a6)
	bra	$00002324

l000022C0:
	move.l	a4,d2
	beq	$00002324

l000022C4:
	tst.l	d1
	beq	$00002324

l000022C8:
	movea.l	d1,a3
	lea	-$000C(a3),a3
	cmpa.l	$0014(a4),a5
	bcc	$0000230A

l000022D4:
	movea.l	a4,a2

l000022D6:
	movea.l	(a2),a2
	tst.l	(a2)
	beq	$00002324

l000022DC:
	tst.b	$0008(a2)
	beq	$000022D6

l000022E2:
	cmp.l	$0014(a2),d1
	bcs	$000022D6

l000022E8:
	cmp.l	$0018(a2),d1
	bcc	$000022D6

l000022EE:
	movea.l	$00003E24,a6
	movea.l	a2,a0
	movea.l	d1,a1
	move.l	a5,d0
	jsr.l	-$00C0(a6)
	move.l	$001C(a2),d0
	cmp.l	$0010(a4),d0
	bne	$00002324

l00002308:
	movea.l	a2,a3

l0000230A:
	movea.l	$00003E24,a6
	movea.l	a3,a1
	jsr.l	-$00FC(a6)
	move.l	-(a3),d0
	movea.l	$00003E24,a6
	movea.l	a3,a1
	jsr.l	-$00D2(a6)

l00002324:
	movem.l	(a7)+,d2/a2-a6
	rts
0000232A                               00 00                       ..    

;; fn0000232C: 0000232C
;;   Called from:
;;     000021A6 (in fn0000215C)
fn0000232C proc
	movem.l	d2-d4/a2-a6,-(a7)
	move.l	$0028(a7),d2
	movea.l	$0024(a7),a4
	movea.l	$00003E24,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$00002358

l00002346:
	movea.l	$00003E24,a6
	movea.l	a4,a0
	move.l	d2,d0
	jsr.l	-$02C4(a6)
	bra	$0000244A

l00002358:
	suba.l	a3,a3
	move.l	a4,d4
	beq	$00002448

l00002360:
	tst.l	d2
	beq	$00002448

l00002366:
	cmp.l	$0014(a4),d2
	bcc	$0000241A

l0000236E:
	movea.l	(a4),a5

l00002370:
	tst.l	(a5)
	beq	$00002392

l00002374:
	tst.b	$0008(a5)
	beq	$0000238E

l0000237A:
	movea.l	$00003E24,a6
	movea.l	a5,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3
	move.l	a3,d0
	bne	$000023FE

l0000238E:
	movea.l	(a5),a5
	bra	$00002370

l00002392:
	moveq	#$28,d3
	add.l	$0010(a4),d3
	move.l	$000C(a4),d1
	movea.l	$00003E24,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$00002448

l000023B0:
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
	movea.l	$00003E24,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F0(a6)
	movea.l	$00003E24,a6
	movea.l	a3,a0
	move.l	d2,d0
	jsr.l	-$00BA(a6)
	movea.l	d0,a3

l000023FE:
	move.l	#$00010000,d0
	and.l	$000C(a4),d0
	beq	$00002448

l0000240A:
	movea.l	a3,a2
	addq.l	#$07,d2
	lsr.l	#$03,d2

l00002410:
	clr.l	(a2)+
	clr.l	(a2)+
	subq.l	#$01,d2
	bne	$00002410

l00002418:
	bra	$00002448

l0000241A:
	moveq	#$10,d3
	add.l	d2,d3
	move.l	$000C(a4),d1
	movea.l	$00003E24,a6
	move.l	d3,d0
	jsr.l	-$00C6(a6)
	movea.l	d0,a3
	move.l	a3,d0
	beq	$00002448

l00002434:
	move.l	d3,(a3)+
	movea.l	$00003E24,a6
	movea.l	a4,a0
	movea.l	a3,a1
	jsr.l	-$00F6(a6)
	addq.l	#$08,a3
	clr.l	(a3)+

l00002448:
	move.l	a3,d0

l0000244A:
	movem.l	(a7)+,d2-d4/a2-a6
	rts

;; fn00002450: 00002450
;;   Called from:
;;     0000217E (in fn0000215C)
fn00002450 proc
	movem.l	d2-d3/a2/a6,-(a7)
	move.l	$0018(a7),d3
	movea.l	$001C(a7),a2
	movea.l	$00003E24,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$0000247E

l0000246A:
	movea.l	$00003E24,a6
	move.l	$0014(a7),d0
	move.l	d3,d1
	move.l	a2,d2
	jsr.l	-$02B8(a6)
	bra	$000024BA

l0000247E:
	suba.l	a1,a1
	cmp.l	a2,d3
	bcs	$000024B8

l00002484:
	addq.l	#$07,d3
	movea.l	$00003E24,a6
	moveq	#$18,d0
	moveq	#$00,d1
	jsr.l	-$00C6(a6)
	movea.l	d0,a1
	move.l	a1,d0
	beq	$000024B8

l0000249A:
	lea	$0004(a1),a0
	move.l	a0,(a1)
	clr.l	(a0)
	move.l	a1,$0008(a1)
	move.l	$0014(a7),$000C(a1)
	moveq	#-$08,d0
	and.l	d3,d0
	move.l	d0,$0010(a1)
	move.l	a2,$0014(a1)

l000024B8:
	move.l	a1,d0

l000024BA:
	movem.l	(a7)+,d2-d3/a2/a6
	rts

;; fn000024C0: 000024C0
fn000024C0 proc
	movem.l	d2/a2/a6,-(a7)
	move.l	$0010(a7),d2
	movea.l	$00003E24,a0
	cmpi.w	#$0027,$0014(a0)
	bcs	$000024E4

l000024D6:
	movea.l	$00003E24,a6
	movea.l	d2,a0
	jsr.l	-$02BE(a6)
	bra	$00002528

l000024E4:
	tst.l	d2
	beq	$00002528

l000024E8:
	movea.l	$00003E24,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d1
	beq	$0000251A

l000024FA:
	move.l	-(a2),d0
	movea.l	$00003E24,a6
	movea.l	a2,a1
	jsr.l	-$00D2(a6)
	movea.l	$00003E24,a6
	movea.l	d2,a0
	jsr.l	-$0102(a6)
	movea.l	d0,a2
	move.l	a2,d0
	bne	$000024FA

l0000251A:
	movea.l	$00003E24,a6
	movea.l	d2,a1
	moveq	#$18,d0
	jsr.l	-$00D2(a6)

l00002528:
	movem.l	(a7)+,d2/a2/a6
	rts
0000252E                                           00 00               ..

;; fn00002530: 00002530
;;   Called from:
;;     00001FA8 (in fn00001F9C)
;;     000020B8 (in fn000020B0)
;;     00003CE8 (in fn00003CE0)
fn00002530 proc
	movem.l	a6,-(a7)
	movea.l	$00003E24,a6
	moveq	#$00,d0
	move.l	#$00001000,d1
	jsr.l	-$0132(a6)
	and.l	#$00001000,d0
	beq	$0000255A

l0000254E:
	pea	$00000014
	jsr.l	fn0000131C
	addq.w	#$04,a7

l0000255A:
	movea.l	(a7)+,a6
	rts
0000255E                                           00 00               ..

;; fn00002560: 00002560
;;   Called from:
;;     00001B94 (in fn000015C0)
fn00002560 proc
	movem.l	d2-d6,-(a7)
	move.l	$001C(a7),d1
	move.l	$0018(a7),d0
	movea.l	d1,a0
	move.l	$0024(a7),d3
	move.l	$0020(a7),d2
	bne	$000025B6

l00002578:
	cmp.l	d3,d0
	bcc	$0000258A

l0000257C:
	move.l	d3,d2
	jsr.l	fn00002664
	move.l	d0,d1
	bra	$0000265C

l0000258A:
	tst.l	d3
	bne	$00002596

l0000258E:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l00002596:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	fn00002664
	movea.l	d0,a1
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	fn00002664
	move.l	d0,d1
	move.l	a1,d0
	bra	$0000265E

l000025B6:
	cmp.l	d2,d0
	bcc	$000025C0

l000025BA:
	moveq	#$00,d0
	bra	$0000265C

l000025C0:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000025DE

l000025CA:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000025DE

l000025D2:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$000025DE

l000025DA:
	moveq	#$00,d4
	move.b	d2,d6

l000025DE:
	lea	$00003E5C,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$000025FE

l000025F2:
	cmp.l	d0,d2
	bcs	$000025FA

l000025F6:
	cmp.l	a0,d3
	bhi	$000025BA

l000025FA:
	moveq	#$01,d0
	bra	$0000265C

l000025FE:
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
	jsr.l	fn00002664
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
	bcs	$0000265A

l00002654:
	bne	$0000265C

l00002656:
	cmpa.l	d2,a0
	bcc	$0000265C

l0000265A:
	subq.l	#$01,d1

l0000265C:
	moveq	#$00,d0

l0000265E:
	movem.l	(a7)+,d2-d6
	rts

;; fn00002664: 00002664
;;   Called from:
;;     0000257E (in fn00002560)
;;     0000259C (in fn00002560)
;;     000025A8 (in fn00002560)
;;     0000261A (in fn00002560)
;;     000027CE (in fn000027B0)
;;     000027EC (in fn000027B0)
;;     000027F6 (in fn000027B0)
;;     00002864 (in fn000027B0)
fn00002664 proc
	movem.l	d5-d7,-(a7)
	move.l	d2,d7
	beq	$0000267E

l0000266C:
	move.l	d1,d6
	move.l	d0,d5
	bne	$0000268C

l00002672:
	tst.l	d1
	beq	$000027AA

l00002678:
	cmp.l	d1,d2
	bhi	$000027AA

l0000267E:
	move.l	d1,d0
	move.l	d2,d1
	jsr.l	fn0000151E
	bra	$000027AA

l0000268C:
	swap.l	d2
	tst.w	d2
	bne	$000026B4

l00002692:
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
	bra	$000027AA

l000026B4:
	movem.l	d2-d4/a0-a1,-(a7)
	subq.l	#$08,a7
	clr.b	$0002(a7)
	moveq	#$00,d1
	moveq	#$00,d0
	tst.l	d7
	bmi	$000026D0

l000026C6:
	addq.w	#$01,d0
	add.l	d6,d6
	addx.l	d5,d5
	add.l	d7,d7
	bpl	$000026C6

l000026D0:
	move.w	d0,(a7)

l000026D2:
	move.l	d7,d3
	move.l	d5,d2
	swap.l	d2
	swap.l	d3
	cmp.w	d3,d2
	bne	$000026E4

l000026DE:
	move.w	#$FFFF,d1
	bra	$000026EE

l000026E4:
	move.l	d5,d1
	divu.w	d3,d1
	swap.l	d1
	clr.w	d1
	swap.l	d1

l000026EE:
	movea.l	d6,a1
	clr.w	d6
	swap.l	d6

l000026F4:
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
	bne	$00002714

l0000270C:
	cmp.l	d4,d2
	bls	$00002714

l00002710:
	subq.l	#$01,d1
	bra	$000026F4

l00002714:
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
	bcc	$0000276C

l00002756:
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

l0000276C:
	tst.b	$0002(a7)
	bne	$00002788

l00002772:
	move.w	d1,$0004(a7)
	moveq	#$00,d1
	swap.l	d5
	swap.l	d6
	move.w	d6,d5
	clr.w	d6
	st	$0002(a7)
	bra	$000026D2

l00002788:
	move.l	$0004(a7),d0
	move.w	d1,d0
	move.w	d5,d6
	swap.l	d6
	swap.l	d5
	move.w	(a7),d7
	beq	$000027A2

l00002798:
	subq.w	#$01,d7

l0000279A:
	lsr.l	#$01,d5
	roxr.l	#$01,d6
	dbra	d7,$0000279A

l000027A2:
	move.l	d6,d1
	addq.l	#$08,a7
	movem.l	(a7)+,d2-d4/a0-a1

l000027AA:
	movem.l	(a7)+,d5-d7
	rts

;; fn000027B0: 000027B0
;;   Called from:
;;     00001B68 (in fn000015C0)
fn000027B0 proc
	movem.l	d2-d7,-(a7)
	move.l	$0020(a7),d1
	move.l	$001C(a7),d0
	movea.l	d1,a0
	move.l	$0028(a7),d3
	move.l	$0024(a7),d2
	bne	$00002802

l000027C8:
	cmp.l	d3,d0
	bcc	$000027DA

l000027CC:
	move.l	d3,d2
	jsr.l	fn00002664
	moveq	#$00,d0
	bra	$000028BC

l000027DA:
	tst.l	d3
	bne	$000027E6

l000027DE:
	moveq	#$01,d4
	divu.w	d3,d4
	ext.l	d4
	move.l	d4,d3

l000027E6:
	move.l	d0,d1
	moveq	#$00,d0
	move.l	d3,d2
	jsr.l	fn00002664
	move.l	d1,d0
	move.l	a0,d1
	jsr.l	fn00002664
	moveq	#$00,d0
	bra	$000028BC

l00002802:
	cmp.l	d2,d0
	bcs	$000028BC

l00002808:
	move.l	d2,d6
	moveq	#$18,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002826

l00002812:
	moveq	#$10,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002826

l0000281A:
	moveq	#$08,d4
	rol.l	#$08,d6
	tst.b	d6
	bne	$00002826

l00002822:
	moveq	#$00,d4
	move.b	d2,d6

l00002826:
	lea	$00003E5C,a1
	and.w	#$00FF,d6
	add.b	(a1,d6.w),d4
	moveq	#$20,d5
	sub.l	d4,d5
	bne	$00002848

l0000283A:
	cmp.l	d0,d2
	bcs	$00002842

l0000283E:
	cmp.l	d1,d3
	bhi	$000028BC

l00002842:
	sub.l	d3,d1
	subx.l	d2,d0
	bra	$000028BC

l00002848:
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
	jsr.l	fn00002664
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
	bcs	$000028A4

l0000289E:
	bne	$000028A8

l000028A0:
	cmpa.l	d3,a0
	bcc	$000028A8

l000028A4:
	sub.l	a1,d3
	subx.l	d0,d2

l000028A8:
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

l000028BC:
	movem.l	(a7)+,d2-d7
	rts
000028C2       00 00 00 20 20 20 20 20 20 20 20 20 28 28   ...         ((
000028D0 28 28 28 20 20 20 20 20 20 20 20 20 20 20 20 20 (((             
000028E0 20 20 20 20 20 88 10 10 10 10 10 10 10 10 10 10      ...........
000028F0 10 10 10 10 10 04 04 04 04 04 04 04 04 04 04 10 ................
00002900 10 10 10 10 10 10 41 41 41 41 41 41 01 01 01 01 ......AAAAAA....
00002910 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 01 ................
00002920 10 10 10 10 10 10 42 42 42 42 42 42 02 02 02 02 ......BBBBBB....
00002930 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 ................
00002940 10 10 10 10 20 00 00 00 00 00 00 00 00 00 00 00 .... ...........
00002950 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
000029C0 00 00 00 00 00 00 00 00 48 E7 00 32 48 78 00 20 ........H..2Hx. 
000029D0 4E B9 00 00 21 5C 23 C0 00 00 3F 7C 48 78 00 20 N...!\#...?|Hx. 
000029E0 4E B9 00 00 21 5C 23 C0 00 00 3F 80 48 78 00 20 N...!\#...?.Hx. 
000029F0 4E B9 00 00 21 5C 23 C0 00 00 3F 84 4F EF 00 0C N...!\#...?.O...
00002A00 4A B9 00 00 3F 7C 67 10 4A B9 00 00 3F 80 67 08 J...?|g.J...?.g.
00002A10 4A B9 00 00 3F 84 66 0C 48 78 00 14 4E B9 00 00 J...?.f.Hx..N...
00002A20 13 1C 58 4F 20 79 00 00 3F 7C 20 B9 00 00 3E 34 ..XO y..?| ...>4
00002A30 20 79 00 00 3F 7C 70 20 21 40 00 18 22 39 00 00  y..?|p !@.."9..
00002A40 3E 34 2C 79 00 00 3E 28 4E AE FF 28 4A 80 67 10 >4,y..>(N..(J.g.
00002A50 20 79 00 00 3F 7C 41 E8 00 18 00 90 00 00 02 04  y..?|A.........
00002A60 20 79 00 00 3F 80 20 B9 00 00 3E 38 20 79 00 00  y..?. ...>8 y..
00002A70 3F 80 70 40 21 40 00 18 22 39 00 00 3E 38 2C 79 ?.p@!@.."9..>8,y
00002A80 00 00 3E 28 4E AE FF 28 4A 80 67 10 20 79 00 00 ..>(N..(J.g. y..
00002A90 3F 80 41 E8 00 18 00 90 00 00 02 80 20 79 00 00 ?.A......... y..
00002AA0 3F 84 20 B9 00 00 3E 3C 20 79 00 00 3F 84 70 40 ?. ...>< y..?.p@
00002AB0 21 40 00 18 22 39 00 00 3E 3C 2C 79 00 00 3E 28 !@.."9..><,y..>(
00002AC0 4E AE FF 28 4A 80 67 10 20 79 00 00 3F 84 41 E8 N..(J.g. y..?.A.
00002AD0 00 18 00 90 00 00 02 80 20 79 00 00 3F 84 42 A8 ........ y..?.B.
00002AE0 00 04 20 79 00 00 3F 80 42 A8 00 04 20 79 00 00 .. y..?.B... y..
00002AF0 3F 7C 42 A8 00 04 20 79 00 00 3F 84 42 A8 00 08 ?|B... y..?.B...
00002B00 20 79 00 00 3F 80 42 A8 00 08 20 79 00 00 3F 7C  y..?.B... y..?|
00002B10 42 A8 00 08 24 79 00 00 3F 84 42 AA 00 14 22 79 B...$y..?.B..."y
00002B20 00 00 3F 80 42 A9 00 14 20 79 00 00 3F 7C 42 A8 ..?.B... y..?|B.
00002B30 00 14 42 AA 00 1C 42 A9 00 1C 42 A8 00 1C 42 A8 ..B...B...B...B.
00002B40 00 10 20 79 00 00 3F 7C 21 79 00 00 3F 80 00 0C .. y..?|!y..?...
00002B50 20 79 00 00 3F 80 21 79 00 00 3F 7C 00 10 20 79  y..?.!y..?|.. y
00002B60 00 00 3F 80 21 79 00 00 3F 84 00 0C 20 79 00 00 ..?.!y..?... y..
00002B70 3F 84 21 79 00 00 3F 80 00 10 20 79 00 00 3F 84 ?.!y..?... y..?.
00002B80 42 A8 00 0C 23 F9 00 00 3F 7C 00 00 3F 88 23 F9 B...#...?|..?.#.
00002B90 00 00 3F 84 00 00 3F 8C 4C DF 4C 00 4E 75 00 00 ..?...?.L.L.Nu..
00002BA0 42 A7 4E B9 00 00 2B AC 58 4F 4E 75             B.N...+.XONu    

;; fn00002BAC: 00002BAC
fn00002BAC proc
	movem.l	a2,-(a7)
	movea.l	$0008(a7),a2
	move.l	a2,d0
	beq	$00002BC4

l00002BB8:
	move.l	a2,-(a7)
	jsr.l	fn000020B0
	addq.w	#$04,a7
	bra	$00002BEA

l00002BC4:
	movea.l	$00003F88,a2
	move.l	a2,d0
	beq	$00002BEA

l00002BCE:
	moveq	#$02,d0
	and.l	$0018(a2),d0
	beq	$00002BE0

l00002BD6:
	move.l	a2,-(a7)
	jsr.l	fn000020B0
	addq.w	#$04,a7

l00002BE0:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00002BCE

l00002BEA:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	rts

;; fn00002BF0: 00002BF0
;;   Called from:
;;     000013A2 (in fn00001390)
;;     000013C0 (in fn00001390)
;;     000013D0 (in fn00001390)
fn00002BF0 proc
	lea	$0008(a7),a0
	move.l	a0,-(a7)
	move.l	$0008(a7),-(a7)
	move.l	$00003F7C,-(a7)
	jsr.l	fn00002C3C
	lea	$000C(a7),a7
	rts

;; fn00002C0C: 00002C0C
;;   Called from:
;;     00002F46 (in fn00002C3C)
;;     00003162 (in fn00002C3C)
;;     00003248 (in fn00002C3C)
;;     000032CA (in fn00002C3C)
;;     0000350A (in fn00002C3C)
;;     00003528 (in fn00002C3C)
;;     00003682 (in fn00002C3C)
;;     0000369C (in fn00002C3C)
;;     00003968 (in fn00002C3C)
;;     00003982 (in fn00002C3C)
;;     00003BBC (in fn00002C3C)
;;     00003C28 (in fn00002C3C)
fn00002C0C proc
	movem.l	a2,-(a7)
	movea.l	$000C(a7),a2
	move.l	a2,d0
	beq	$00002C36

l00002C18:
	move.l	$0004(a2),d0
	cmp.l	$0008(a2),d0
	bcc	$00002C2A

l00002C22:
	movea.l	$0004(a2),a0
	move.b	$000B(a7),(a0)

l00002C2A:
	lea	$0014(a2),a0
	addq.l	#$01,(a0)
	lea	$0004(a2),a0
	subq.l	#$01,(a0)

l00002C36:
	movea.l	(a7)+,a2
	rts
00002C3A                               00 00                       ..    

;; fn00002C3C: 00002C3C
;;   Called from:
;;     00002C00 (in fn00002BF0)
fn00002C3C proc
	lea	-$004C(a7),a7
	movem.l	d2-d7/a2-a6,-(a7)
	move.l	$0084(a7),d2
	movea.l	$0080(a7),a4
	movea.l	$007C(a7),a2
	clr.l	$003C(a7)
	moveq	#$00,d4
	moveq	#$00,d5
	tst.b	(a4)
	beq	$00003C3E

l00002C5E:
	moveq	#$00,d3
	cmpi.b	#$25,(a4)
	bne	$00003B3C

l00002C68:
	moveq	#-$01,d6
	move.b	#$69,$0048(a7)
	clr.b	$0049(a7)
	lea	$0001(a4),a3
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00002CEC

l00002C90:
	moveq	#$00,d6
	moveq	#$00,d0
	move.b	(a3),d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00002CEC

l00002CAA:
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
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00002CAA

l00002CEC:
	cmpi.b	#$68,(a3)
	beq	$00002D16

l00002CF2:
	cmpi.b	#$6C,(a3)
	beq	$00002D16

l00002CF8:
	cmpi.b	#$4C,(a3)
	beq	$00002D16

l00002CFE:
	cmpi.b	#$7A,(a3)
	beq	$00002D16

l00002D04:
	cmpi.b	#$6A,(a3)
	beq	$00002D16

l00002D0A:
	cmpi.b	#$74,(a3)
	beq	$00002D16

l00002D10:
	cmpi.b	#$2A,(a3)
	bne	$00002D7E

l00002D16:
	move.b	$0049(a7),d7
	move.b	$0048(a7),d1

l00002D1E:
	cmpi.b	#$2A,(a3)
	bne	$00002D28

l00002D24:
	moveq	#$01,d7
	bra	$00002D4A

l00002D28:
	cmp.b	#$68,d1
	bne	$00002D38

l00002D2E:
	cmpi.b	#$68,(a3)
	bne	$00002D38

l00002D34:
	moveq	#$02,d1
	bra	$00002D4A

l00002D38:
	cmp.b	#$6C,d1
	bne	$00002D48

l00002D3E:
	cmpi.b	#$6C,(a3)
	bne	$00002D48

l00002D44:
	moveq	#$01,d1
	bra	$00002D4A

l00002D48:
	move.b	(a3),d1

l00002D4A:
	addq.l	#$01,a3
	cmpi.b	#$68,(a3)
	beq	$00002D1E

l00002D52:
	cmpi.b	#$6C,(a3)
	beq	$00002D1E

l00002D58:
	cmpi.b	#$4C,(a3)
	beq	$00002D1E

l00002D5E:
	cmpi.b	#$7A,(a3)
	beq	$00002D1E

l00002D64:
	cmpi.b	#$6A,(a3)
	beq	$00002D1E

l00002D6A:
	cmpi.b	#$74,(a3)
	beq	$00002D1E

l00002D70:
	cmpi.b	#$2A,(a3)
	beq	$00002D1E

l00002D76:
	move.b	d1,$0048(a7)
	move.b	d7,$0049(a7)

l00002D7E:
	cmpi.b	#$6A,$0048(a7)
	bne	$00002D8C

l00002D86:
	move.b	#$01,$0048(a7)

l00002D8C:
	cmpi.b	#$74,$0048(a7)
	bne	$00002D9A

l00002D94:
	move.b	#$69,$0048(a7)

l00002D9A:
	cmpi.b	#$7A,$0048(a7)
	bne	$00002DA8

l00002DA2:
	move.b	#$6C,$0048(a7)

l00002DA8:
	move.b	(a3)+,d7
	beq	$00002E1E

l00002DAC:
	cmp.b	#$25,d7
	beq	$00002E1E

l00002DB2:
	cmp.b	#$63,d7
	beq	$00002E1E

l00002DB8:
	cmp.b	#$6E,d7
	beq	$00002E1E

l00002DBE:
	cmp.b	#$5B,d7
	beq	$00002E1E

l00002DC4:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00002DEE

l00002DD6:
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
	bra	$00002DFA

l00002DEE:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,d1
	addq.w	#$04,a7

l00002DFA:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00002DC4

l00002E1C:
	moveq	#$01,d3

l00002E1E:
	move.b	d7,d1
	sub.b	#$25,d1
	beq	$0000326A

l00002E28:
	sub.b	#$36,d1
	beq	$00002F66

l00002E30:
	subq.b	#$08,d1
	beq	$00002E46

l00002E34:
	sub.b	#$0B,d1
	beq	$000032D8

l00002E3C:
	subq.b	#$05,d1
	beq	$00003184

l00002E42:
	bra	$0000337A

l00002E46:
	cmp.l	#$FFFFFFFF,d6
	bne	$00002E50

l00002E4E:
	moveq	#$01,d6

l00002E50:
	tst.b	$0049(a7)
	bne	$00002E6A

l00002E56:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a1
	bra	$00002E6C

l00002E6A:
	suba.l	a1,a1

l00002E6C:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	move.l	a1,$002C(a7)
	tst.l	(a0)
	blt	$00002EA4

l00002E84:
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
	bra	$00002EC2

l00002EA4:
	movea.l	$002C(a7),a1
	move.l	a2,-(a7)
	move.l	a1,$0030(a7)
	jsr.l	fn00003CE0
	move.l	d0,$0038(a7)
	movea.l	$0030(a7),a1
	move.l	a1,$0030(a7)
	addq.w	#$04,a7

l00002EC2:
	movea.l	$002C(a7),a1
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$0034(a7)
	beq	$00002F3A

l00002ED8:
	move.l	a1,$002C(a7)
	cmp.l	d3,d6
	bcs	$00002F3A

l00002EE0:
	move.b	$0049(a7),d7
	movea.l	$002C(a7),a4

l00002EE8:
	tst.b	d7
	bne	$00002EEE

l00002EEC:
	move.b	d5,(a4)+

l00002EEE:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00002F18

l00002F00:
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
	bra	$00002F24

l00002F18:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,d1
	addq.w	#$04,a7

l00002F24:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00002F36

l00002F32:
	cmp.l	d3,d6
	bcc	$00002EE8

l00002F36:
	move.b	d7,$0049(a7)

l00002F3A:
	cmp.l	#$FFFFFFFF,d5
	beq	$00002F4C

l00002F42:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l00002F4C:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003B36

l00002F58:
	tst.l	d3
	beq	$00003B36

l00002F5E:
	addq.l	#$01,$003C(a7)
	bra	$00003B36

l00002F66:
	clr.b	$002C(a7)
	cmpi.b	#$5E,(a3)
	bne	$00002F78

l00002F70:
	move.b	#$01,$002C(a7)
	addq.l	#$01,a3

l00002F78:
	clr.l	$0034(a7)
	move.b	$002C(a7),d7
	move.l	$0034(a7),d1

l00002F84:
	tst.b	d7
	beq	$00002F90

l00002F88:
	move.l	#$000000FF,d5
	bra	$00002F92

l00002F90:
	moveq	#$00,d5

l00002F92:
	lea	$004E(a7),a0
	move.b	d5,(a0,d1)
	addq.l	#$01,d1
	cmp.l	#$00000020,d1
	bcs	$00002F84

l00002FA4:
	move.l	d2,$0084(a7)
	move.b	d7,$002C(a7)
	move.b	$002C(a7),d2

l00002FB0:
	tst.b	(a3)
	beq	$00003026

l00002FB4:
	move.b	(a3)+,d1
	cmpi.b	#$2D,(a3)
	bne	$00002FC8

l00002FBC:
	cmp.b	$0001(a3),d1
	bcc	$00002FC8

l00002FC2:
	addq.l	#$01,a3
	move.b	(a3)+,d7
	bra	$00002FCA

l00002FC8:
	move.b	d1,d7

l00002FCA:
	moveq	#$00,d5
	move.b	d1,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcs	$00003020

l00002FD6:
	tst.b	d2
	beq	$00002FFA

l00002FDA:
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
	bra	$00003016

l00002FFA:
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

l00003016:
	addq.l	#$01,d5
	moveq	#$00,d0
	move.b	d7,d0
	cmp.l	d5,d0
	bcc	$00002FD6

l00003020:
	cmpi.b	#$5D,(a3)
	bne	$00002FB0

l00003026:
	move.l	$0084(a7),d2
	addq.l	#$01,a3
	tst.b	$0049(a7)
	bne	$00003046

l00003032:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a6
	bra	$00003048

l00003046:
	suba.l	a6,a6

l00003048:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000307A

l0000305A:
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
	bra	$00003088

l0000307A:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00003088:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$FFFFFFFF,$002C(a7)
	beq	$00003156

l0000309C:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	fn000014EC
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
	beq	$00003156

l000030CC:
	cmp.l	d3,d6
	bcs	$00003156

l000030D2:
	move.b	$0049(a7),d7

l000030D6:
	tst.b	d7
	bne	$000030DC

l000030DA:
	move.b	d5,(a6)+

l000030DC:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003106

l000030EE:
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
	bra	$00003112

l00003106:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,d1
	addq.w	#$04,a7

l00003112:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00003152

l00003120:
	lea	$004E(a7),a0
	move.l	a0,-(a7)
	move.l	a1,-(a7)
	pea	$00000008
	move.l	d5,-(a7)
	jsr.l	fn000014EC
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
	beq	$00003152

l0000314E:
	cmp.l	d3,d6
	bcc	$000030D6

l00003152:
	move.b	d7,$0049(a7)

l00003156:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003168

l0000315E:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l00003168:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003B36

l00003174:
	tst.l	d3
	beq	$00003B36

l0000317A:
	clr.b	(a6)+
	addq.l	#$01,$003C(a7)
	bra	$00003B36

l00003184:
	tst.b	$0049(a7)
	bne	$0000319E

l0000318A:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a5
	bra	$000031A0

l0000319E:
	suba.l	a5,a5

l000031A0:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000323C

l000031AA:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$0000323C

l000031C6:
	cmp.l	d3,d6
	bcs	$0000323C

l000031CA:
	move.b	$0049(a7),d7

l000031CE:
	tst.b	d7
	bne	$000031D4

l000031D2:
	move.b	d5,(a5)+

l000031D4:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000031FE

l000031E6:
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
	bra	$0000320A

l000031FE:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,d1
	addq.w	#$04,a7

l0000320A:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmp.l	#$FFFFFFFF,d1
	beq	$00003238

l00003218:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003238

l00003234:
	cmp.l	d3,d6
	bcc	$000031CE

l00003238:
	move.b	d7,$0049(a7)

l0000323C:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000324E

l00003244:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l0000324E:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003B36

l0000325A:
	tst.l	d3
	beq	$00003B36

l00003260:
	clr.b	(a5)+
	addq.l	#$01,$003C(a7)
	bra	$00003B36

l0000326A:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000329C

l0000327C:
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
	bra	$000032AA

l0000329C:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l000032AA:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	cmpi.l	#$00000025,$002C(a7)
	beq	$00003B36

l000032BE:
	cmp.l	#$FFFFFFFF,d5
	beq	$000032D0

l000032C6:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l000032D0:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00003B36

l000032D8:
	tst.b	$0049(a7)
	bne	$00003370

l000032E0:
	cmpi.b	#$01,$0048(a7)
	bne	$00003302

l000032E8:
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
	bra	$00003370

l00003302:
	cmpi.b	#$6C,$0048(a7)
	bne	$00003320

l0000330A:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)
	bra	$00003370

l00003320:
	cmpi.b	#$68,$0048(a7)
	bne	$0000333E

l00003328:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.w	d4,(a0)
	bra	$00003370

l0000333E:
	cmpi.b	#$02,$0048(a7)
	bne	$0000335C

l00003346:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.b	d4,(a0)
	bra	$00003370

l0000335C:
	move.l	d2,d0
	addq.l	#$03,d0
	lsr.l	#$02,d0
	lsl.l	#$02,d0
	move.l	d0,d2
	addq.l	#$04,d2
	movea.l	d2,a0
	movea.l	-$0004(a0),a0
	move.l	d4,(a0)

l00003370:
	moveq	#$01,d3
	addq.l	#$01,$003C(a7)
	bra	$00003B36

l0000337A:
	clr.l	$0030(a7)
	clr.l	$002C(a7)
	clr.l	$006E(a7)
	tst.b	d7
	bne	$0000338C

l0000338A:
	subq.l	#$01,a3

l0000338C:
	cmp.b	#$70,d7
	bne	$0000339A

l00003392:
	move.b	#$6C,$0048(a7)
	moveq	#$78,d7

l0000339A:
	cmp.l	#$0000002D,d5
	bne	$000033A8

l000033A2:
	cmp.b	#$75,d7
	bne	$000033B0

l000033A8:
	cmp.l	#$0000002B,d5
	bne	$00003400

l000033B0:
	cmp.l	d3,d6
	bcs	$00003400

l000033B4:
	move.l	d5,$006E(a7)
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000033EA

l000033CA:
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
	bra	$000033F8

l000033EA:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l000033F8:
	move.l	$0034(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4

l00003400:
	cmp.b	#$69,d7
	bne	$00003572

l00003408:
	cmp.l	#$00000030,d5
	bne	$00003534

l00003412:
	cmp.l	d3,d6
	bcs	$00003534

l00003418:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$0000344A

l0000342A:
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
	bra	$00003458

l0000344A:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003458:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$000028C4,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$0000347E

l0000347A:
	or.b	#$20,d0

l0000347E:
	cmp.l	#$00000078,d0
	bne	$00003516

l00003488:
	cmp.l	d3,d6
	bcs	$00003516

l0000348E:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000034C0

l000034A0:
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
	bra	$000034CE

l000034C0:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l000034CE:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$000034FA

l000034F4:
	cmp.l	d3,d6
	bcs	$000034FA

l000034F8:
	moveq	#$78,d7

l000034FA:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$00003510

l00003504:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l00003510:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00003518

l00003516:
	moveq	#$6F,d7

l00003518:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$0000352E

l00003522:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l0000352E:
	subq.l	#$01,d3
	subq.l	#$01,d4
	bra	$00003572

l00003534:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	bne	$00003572

l00003550:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$00003572

l0000356C:
	cmp.l	d3,d6
	bcs	$00003572

l00003570:
	moveq	#$78,d7

l00003572:
	cmp.b	#$78,d7
	bne	$000036A6

l0000357A:
	cmp.l	#$00000030,d5
	bne	$000036A6

l00003584:
	cmp.l	d3,d6
	bcs	$000036A6

l0000358A:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$000035BC

l0000359C:
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
	bra	$000035CA

l000035BC:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l000035CA:
	move.l	$0034(a7),$0040(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$000028C4,a0
	btst.w	#$0000,($01,a0,d0.w)
	beq	$000035F0

l000035EC:
	or.b	#$20,d0

l000035F0:
	cmp.l	#$00000078,d0
	bne	$0000368C

l000035FA:
	cmp.l	d3,d6
	bcs	$0000368C

l00003600:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003632

l00003612:
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
	bra	$00003640

l00003632:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,$0038(a7)
	addq.w	#$04,a7

l00003640:
	move.l	$0034(a7),$004A(a7)
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	$0037(a7),d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$00003672

l00003668:
	cmp.l	d3,d6
	bcs	$00003672

l0000366C:
	move.l	$004A(a7),d5
	bra	$000036A6

l00003672:
	cmpi.l	#$FFFFFFFF,$004A(a7)
	beq	$00003688

l0000367C:
	move.l	a2,-(a7)
	move.l	$004E(a7),-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l00003688:
	subq.l	#$01,d3
	subq.l	#$01,d4

l0000368C:
	cmpi.l	#$FFFFFFFF,$0040(a7)
	beq	$000036A2

l00003696:
	move.l	a2,-(a7)
	move.l	$0044(a7),-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l000036A2:
	subq.l	#$01,d3
	subq.l	#$01,d4

l000036A6:
	cmp.b	#$78,d7
	beq	$000036B2

l000036AC:
	cmp.b	#$58,d7
	bne	$000036BC

l000036B2:
	move.l	#$00000010,$0040(a7)
	bra	$000036DA

l000036BC:
	cmp.b	#$6F,d7
	bne	$000036CC

l000036C2:
	move.l	#$00000008,$0034(a7)
	bra	$000036D4

l000036CC:
	move.l	#$0000000A,$0034(a7)

l000036D4:
	move.l	$0034(a7),$0040(a7)

l000036DA:
	move.l	$0040(a7),$0072(a7)
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$00003942

l0000370C:
	cmpi.l	#$0000000A,$0072(a7)
	bne	$00003742

l00003716:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	tst.l	d0
	beq	$00003942

l00003742:
	cmpi.l	#$00000008,$0072(a7)
	bne	$00003762

l0000374C:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.b	d7,$004A(a7)
	cmp.l	#$00000037,d5
	bgt	$00003942

l00003762:
	move.l	d2,$0084(a7)
	move.l	d4,$0034(a7)
	move.l	d6,$0040(a7)
	move.b	d7,$004A(a7)
	cmp.l	d3,d6
	bcs	$00003942

l00003778:
	move.l	$0072(a7),d7
	movea.l	$0040(a7),a4

l00003780:
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
	jsr.l	fn00003C60
	lea	$0010(a7),a7
	movea.l	(a7)+,a1
	move.l	d0,$0048(a7)
	move.l	d1,$004C(a7)
	movem.l	(a7)+,d1
	movem.l	(a7)+,d0
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$000037E0

l000037D6:
	move.l	d5,d4
	sub.l	#$00000030,d4
	bra	$000037E2

l000037E0:
	moveq	#$00,d4

l000037E2:
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
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000001,d0
	beq	$00003832

l00003828:
	move.l	d5,d6
	sub.l	#$00000037,d6
	bra	$00003834

l00003832:
	moveq	#$00,d6

l00003834:
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
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000002,d0
	beq	$00003884

l0000387A:
	move.l	d5,d2
	sub.l	#$00000057,d2
	bra	$00003886

l00003884:
	moveq	#$00,d2

l00003886:
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
	blt	$000038D8

l000038C0:
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
	bra	$000038E4

l000038D8:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,d1
	addq.w	#$04,a7

l000038E4:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,$0034(a7)
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000044,d0
	beq	$00003942

l00003908:
	cmp.l	#$0000000A,d7
	bne	$0000392C

l00003910:
	move.b	d5,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000004,d0
	beq	$00003942

l0000392C:
	cmp.l	#$00000008,d7
	bne	$0000393C

l00003934:
	cmp.l	#$00000037,d5
	bgt	$00003942

l0000393C:
	cmpa.l	d3,a4
	bcc	$00003780

l00003942:
	move.b	$004A(a7),d7
	move.l	$0034(a7),d4
	move.l	$0084(a7),d2
	tst.l	$006E(a7)
	beq	$00003976

l00003954:
	cmp.l	#$00000002,d3
	bne	$00003976

l0000395C:
	cmp.l	#$FFFFFFFF,d5
	beq	$0000396E

l00003964:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l0000396E:
	subq.l	#$01,d3
	subq.l	#$01,d4
	move.l	$006E(a7),d5

l00003976:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003988

l0000397E:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l00003988:
	subq.l	#$01,d3
	subq.l	#$01,d4
	tst.b	$0049(a7)
	bne	$00003B36

l00003994:
	tst.l	d3
	beq	$00003B36

l0000399A:
	cmp.b	#$75,d7
	bne	$00003A5C

l000039A2:
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0038(a7)
	move.l	(a7)+,d0
	tst.b	$0034(a7)
	beq	$000039CC

l000039B6:
	subq.b	#$01,$0034(a7)
	beq	$00003A24

l000039BC:
	subi.b	#$66,$0034(a7)
	beq	$00003A08

l000039C4:
	subq.b	#$04,$0034(a7)
	beq	$000039EC

l000039CA:
	bra	$00003A40

l000039CC:
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
	bra	$00003B32

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
	move.l	d0,(a0)
	bra	$00003B32

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
	move.w	d0,(a0)
	bra	$00003B32

l00003A24:
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
	bra	$00003B32

l00003A40:
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
	bra	$00003B32

l00003A5C:
	cmpi.l	#$0000002D,$006E(a7)
	bne	$00003A78

l00003A66:
	movem.l	$002C(a7),d0-d1
	neg.l	d1
	negx.l	d0
	movem.l	a6-a7,$0034(a7)
	bra	$00003A84

l00003A78:
	move.l	$0030(a7),$0038(a7)
	move.l	$002C(a7),$0034(a7)
	move.l	d0,-(a7)
	move.b	$004C(a7),d0
	subq.b	#$01,d0
	move.b	d0,$0030(a7)
	move.l	(a7)+,d0
	tst.b	$002C(a7)
	beq	$00003AAE

l00003A98:
	subq.b	#$01,$002C(a7)
	beq	$00003B00

l00003A9E:
	subi.b	#$66,$002C(a7)
	beq	$00003AE6

l00003AA6:
	subq.b	#$04,$002C(a7)
	beq	$00003ACC

l00003AAC:
	bra	$00003B1A

l00003AAE:
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
	bra	$00003B32

l00003ACC:
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
	bra	$00003B32

l00003AE6:
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
	bra	$00003B32

l00003B00:
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
	bra	$00003B32

l00003B1A:
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

l00003B32:
	addq.l	#$01,$003C(a7)

l00003B36:
	movea.l	a3,a4
	bra	$00003C34

l00003B3C:
	move.b	(a4),d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	beq	$00003BC8

l00003B58:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003B82

l00003B6A:
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
	bra	$00003B8E

l00003B82:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,d1
	addq.w	#$04,a7

l00003B8E:
	move.l	d1,d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	d1,d0
	and.l	#$000000FF,d0
	lea	$000028C5,a0
	adda.l	d0,a0
	moveq	#$00,d0
	move.b	(a0),d0
	and.l	#$00000008,d0
	bne	$00003B58

l00003BB0:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003BC2

l00003BB8:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l00003BC2:
	subq.l	#$01,d4
	moveq	#$01,d3
	bra	$00003C32

l00003BC8:
	lea	$0018(a2),a0
	ori.l	#$00000001,(a0)
	lea	$0014(a2),a0
	subq.l	#$01,(a0)
	blt	$00003BFA

l00003BDA:
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
	bra	$00003C08

l00003BFA:
	move.l	a2,-(a7)
	jsr.l	fn00003CE0
	move.l	d0,$0030(a7)
	addq.w	#$04,a7

l00003C08:
	move.l	$002C(a7),d5
	addq.l	#$01,d3
	addq.l	#$01,d4
	move.b	(a4),d0
	ext.w	d0
	ext.l	d0
	cmp.l	$002C(a7),d0
	beq	$00003C32

l00003C1C:
	cmp.l	#$FFFFFFFF,d5
	beq	$00003C2E

l00003C24:
	move.l	a2,-(a7)
	move.l	d5,-(a7)
	bsr	fn00002C0C
	addq.w	#$08,a7

l00003C2E:
	subq.l	#$01,d3
	subq.l	#$01,d4

l00003C32:
	addq.l	#$01,a4

l00003C34:
	tst.l	d3
	beq	$00003C3E

l00003C38:
	tst.b	(a4)
	bne	$00002C5E

l00003C3E:
	cmp.l	#$FFFFFFFF,d5
	bne	$00003C50

l00003C46:
	tst.l	$003C(a7)
	bne	$00003C50

l00003C4C:
	move.l	d5,d0
	bra	$00003C54

l00003C50:
	move.l	$003C(a7),d0

l00003C54:
	movem.l	(a7)+,d2-d7/a2-a6
	lea	$004C(a7),a7
	rts
00003C5E                                           00 00               ..

;; fn00003C60: 00003C60
;;   Called from:
;;     0000379E (in fn00002C3C)
fn00003C60 proc
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
	bcc	$00003C94

l00003C8E:
	add.l	#$00010000,d1

l00003C94:
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
00003CDE                                           00 00               ..

;; fn00003CE0: 00003CE0
;;   Called from:
;;     00002DF0 (in fn00002C3C)
;;     00002EAE (in fn00002C3C)
;;     00002F1A (in fn00002C3C)
;;     0000307C (in fn00002C3C)
;;     00003108 (in fn00002C3C)
;;     00003200 (in fn00002C3C)
;;     0000329E (in fn00002C3C)
;;     000033EC (in fn00002C3C)
;;     0000344C (in fn00002C3C)
;;     000034C2 (in fn00002C3C)
;;     000035BE (in fn00002C3C)
;;     00003634 (in fn00002C3C)
;;     000038DA (in fn00002C3C)
;;     00003B84 (in fn00002C3C)
;;     00003BFC (in fn00002C3C)
fn00003CE0 proc
	movem.l	d2-d5/a2-a4/a6,-(a7)
	movea.l	$0024(a7),a2
	jsr.l	fn00002530
	move.l	a2,d0
	bne	$00003CF8

l00003CF2:
	moveq	#-$01,d0
	bra	$00003DD6

l00003CF8:
	moveq	#$2A,d0
	and.l	$0018(a2),d0
	moveq	#$20,d5
	cmp.l	d0,d5
	beq	$00003D0A

l00003D04:
	moveq	#-$01,d0
	bra	$00003DD6

l00003D0A:
	lea	$0018(a2),a0
	moveq	#$01,d0
	or.l	d0,(a0)
	move.l	#$00000200,d0
	and.l	(a0),d0
	beq	$00003D22

l00003D1C:
	jsr.l	fn00003DDC

l00003D22:
	tst.l	$001C(a2)
	bne	$00003D40

l00003D28:
	moveq	#$04,d0
	and.l	$0018(a2),d0
	beq	$00003D38

l00003D30:
	moveq	#$01,d0
	move.l	d0,$001C(a2)
	bra	$00003D40

l00003D38:
	move.l	#$00000400,$001C(a2)

l00003D40:
	tst.l	$0008(a2)
	bne	$00003D7C

l00003D46:
	lea	$001C(a2),a1
	move.l	#$00000080,d0
	and.l	$0018(a2),d0
	beq	$00003D5A

l00003D56:
	moveq	#$02,d4
	bra	$00003D5C

l00003D5A:
	moveq	#$01,d4

l00003D5C:
	move.l	d4,d0
	add.l	(a1),d0
	move.l	d0,-(a7)
	jsr.l	fn0000215C
	movea.l	d0,a3
	addq.w	#$04,a7
	move.l	a3,d0
	bne	$00003D74

l00003D70:
	moveq	#-$01,d0
	bra	$00003DD6

l00003D74:
	lea	$0001(a3),a1
	move.l	a1,$0008(a2)

l00003D7C:
	lea	$0004(a2),a0
	move.l	$0008(a2),(a0)
	move.l	$001C(a2),d3
	move.l	(a0),d2
	move.l	(a2),d1
	movea.l	$00003E28,a6
	jsr.l	-$002A(a6)
	lea	$0014(a2),a0
	move.l	d0,(a0)
	subq.l	#$01,(a0)
	bge	$00003DC2

l00003DA0:
	moveq	#-$01,d0
	cmp.l	$0014(a2),d0
	bne	$00003DB2

l00003DA8:
	lea	$0018(a2),a0
	moveq	#$08,d0
	or.l	d0,(a0)
	bra	$00003DBA

l00003DB2:
	lea	$0018(a2),a0
	moveq	#$10,d0
	or.l	d0,(a0)

l00003DBA:
	clr.l	$0014(a2)
	moveq	#-$01,d0
	bra	$00003DD6

l00003DC2:
	lea	$0004(a2),a1
	movea.l	(a1),a0
	movea.l	a0,a4
	addq.l	#$01,a4
	move.l	a4,(a1)
	move.b	(a0),d0
	and.l	#$000000FF,d0

l00003DD6:
	movem.l	(a7)+,d2-d5/a2-a4/a6
	rts

;; fn00003DDC: 00003DDC
;;   Called from:
;;     00003D1C (in fn00003CE0)
fn00003DDC proc
	movem.l	a2,-(a7)
	movea.l	$00003F88,a2
	move.l	a2,d0
	beq	$00003E14

l00003DEA:
	move.l	#$00000202,d0
	and.l	$0018(a2),d0
	cmp.l	#$00000202,d0
	bne	$00003E0A

l00003DFC:
	tst.l	(a2)
	beq	$00003E0A

l00003E00:
	move.l	a2,-(a7)
	jsr.l	fn000020B0
	addq.w	#$04,a7

l00003E0A:
	lea	$000C(a2),a0
	movea.l	(a0),a2
	move.l	a2,d0
	bne	$00003DEA

l00003E14:
	movea.l	(a7)+,a2
	rts
00003E18                         00 24 00 63 41 00 00 00         .$.cA...
00003E20 00 00 00 00 00 00 40 00 00 00 00 00 00 01 02 02 ......@.........
00003E30 03 03 03 03 04 04 04 04 04 04 04 04 05 05 05 05 ................
00003E40 05 05 05 05 05 05 05 05 05 05 05 05 06 06 06 06 ................
00003E50 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 06 ................
00003E60 06 06 06 06 06 06 06 06 06 06 06 06 07 07 07 07 ................
00003E70 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003E80 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003E90 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 07 ................
00003EA0 07 07 07 07 07 07 07 07 07 07 07 07 08 08 08 08 ................
00003EB0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003EC0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003ED0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003EE0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003EF0 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F00 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F10 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 08 ................
00003F20 08 08 08 08 08 08 08 08 08 08 08 08 00 00 00 01 ................
00003F30 00 00 29 C8 00 00 00 00 00 00 00 02 00 00 21 44 ..)...........!D
00003F40 00 00 2B A0 00 00 00 00 00 00 00 00 00 00 00 00 ..+.............
00003F50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
