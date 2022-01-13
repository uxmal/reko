;;; Segment CRTHEAP (000024AC)
000024AC                                     00 50 00 02             .P..

;; fn000024B0: 000024B0
fn000024B0 proc
	link	a6,#$0000
	move.l	-$053C(a5),-(a7)
	move.l	$0008(a6),-(a7)
	jsr.l	000024C4                                             ; $0008(pc)
	unlk	a6
	rts

;; fn000024C4: 000024C4
;;   Called from:
;;     000024BC (in fn000024B0)
fn000024C4 proc
	link	a6,#$0000
	move.l	d3,-(a7)
	move.l	d4,-(a7)
	move.l	$0008(a6),d3
	moveq	#-$20,d4
	cmp.l	d3,d4
	bcc	$000024E0

l000024D6:
	moveq	#$00,d0
	move.l	(a7)+,d4
	move.l	(a7)+,d3
	unlk	a6
	rts

l000024E0:
	addq.l	#$03,d3
	andi.w	#$FFFC,d3
	move.l	$000C(a6),d4

l000024EA:
	move.l	d3,-(a7)
	jsr.l	00002510                                             ; $0024(pc)
	addq.w	#$04,a7
	tst.l	d0
	bne	$00002506

l000024F6:
	tst.l	d4
	beq	$00002506

l000024FA:
	move.l	d3,-(a7)
	jsr.l	00002644                                             ; $0148(pc)
	addq.w	#$04,a7
	tst.l	d0
	bne	$000024EA

l00002506:
	move.l	(a7)+,d4
	move.l	(a7)+,d3
	unlk	a6
	rts
0000250E                                           00 00               ..

;; fn00002510: 00002510
;;   Called from:
;;     000024EC (in fn000024C4)
fn00002510 proc
	link	a6,#$0000
	move.l	d3,-(a7)
	move.l	a2,-(a7)
	move.l	$0008(a6),d3
	move.l	d3,-(a7)
	jsr.l	000027B0                                             ; $0292(pc)
	addq.w	#$04,a7
	tst.l	d0
	movea.l	d0,a2
	bne	$0000254A

l0000252A:
	move.l	d3,-(a7)
	jsr.l	000028A0                                             ; $0374(pc)
	addq.w	#$04,a7
	moveq	#-$01,d2
	cmp.l	d0,d2
	beq	$000025A8

l00002538:
	move.l	d3,-(a7)
	jsr.l	000027B0                                             ; $0276(pc)
	addq.w	#$04,a7
	tst.l	d0
	movea.l	d0,a2
	bne	$0000254A

l00002546:
	jsr.l	000027A0                                             ; $025A(pc)

l0000254A:
	movea.l	(a2),a0
	move.l	$0004(a0),d0
	andi.w	#$FFFC,d0
	move.l	$0004(a2),d1
	andi.w	#$FFFC,d1
	sub.l	d1,d0
	sub.l	d3,d0
	moveq	#$04,d2
	cmp.l	d0,d2
	beq	$00002586

l00002566:
	move.l	d3,-(a7)
	move.l	a2,-(a7)
	jsr.l	000025B4                                             ; $004A(pc)
	addq.w	#$08,a7
	tst.l	d0
	movea.l	d0,a0
	beq	$00002586

l00002576:
	move.l	$0004(a0),d0
	bclr.l	#$01,d0
	bset	#$0000,d0
	move.l	d0,$0004(a0)

l00002586:
	move.l	$0004(a2),d0
	andi.w	#$FFFC,d0
	move.l	d0,$0004(a2)
	move.l	(a2),-$0534(a5)
	move.l	$0004(a2),d0
	andi.w	#$FFFC,d0
	addq.l	#$04,d0
	movea.l	(a7)+,a2
	move.l	(a7)+,d3
	unlk	a6
	rts

l000025A8:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	move.l	(a7)+,d3
	unlk	a6
	rts
000025B2       00 00                                       ..            

;; fn000025B4: 000025B4
;;   Called from:
;;     0000256A (in fn00002510)
fn000025B4 proc
	link	a6,#$0000
	move.l	a2,-(a7)
	movea.l	$0008(a6),a2
	movea.l	(a2),a0
	move.l	$0004(a0),d0
	andi.w	#$FFFC,d0
	move.l	$0004(a2),d1
	andi.w	#$FFFC,d1
	sub.l	d1,d0
	subq.l	#$04,d0
	cmp.l	$000C(a6),d0
	bls	$00002608

l000025DA:
	jsr.l	0000273C                                             ; $0162(pc)
	tst.l	d0
	movea.l	d0,a0
	beq	$00002608

l000025E4:
	move.l	$0004(a2),d0
	andi.w	#$FFFC,d0
	add.l	$000C(a6),d0
	addq.l	#$04,d0
	movea.l	d0,a1
	move.l	a1,$0004(a0)
	move.l	a0,(a1)
	move.l	(a2),(a0)
	move.l	a0,(a2)
	move.l	a0,d0
	movea.l	(a7)+,a2
	unlk	a6
	rts
00002606                   4E 71                               Nq        

l00002608:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	unlk	a6
	rts

;; fn00002610: 00002610
fn00002610 proc
	link	a6,#$0000
	move.l	-$02CC(a5),d0
	move.l	$0008(a6),-$02CC(a5)
	unlk	a6
	rts
00002622       00 00 4E 56 00 00 20 2D FD 34 4E 5E 4E 75   ..NV.. -.4N^Nu
00002630 4E 56 00 00 74 00 2F 02 4E BA FF D6 70 00 4E 5E NV..t./.N...p.N^
00002640 4E 75 00 00                                     Nu..            

;; fn00002644: 00002644
;;   Called from:
;;     000024FC (in fn000024C4)
fn00002644 proc
	link	a6,#$0000
	movea.l	-$02CC(a5),a0
	move.l	a0,d0
	beq	$00002664

l00002650:
	move.l	$0008(a6),-(a7)
	jsr.l	(a0)
	addq.w	#$04,a7
	tst.l	d0
	beq	$00002664

l0000265C:
	moveq	#$01,d0
	unlk	a6
	rts
00002662       4E 71                                       Nq            

l00002664:
	moveq	#$00,d0
	unlk	a6
	rts
0000266A                               00 00 4E 56 00 00           ..NV..
00002670 48 E7 18 30 26 2D FA EC 4A AD FA DC 66 50 20 3C H..0&-..J...fP <
00002680 00 00 02 00 A1 22 2B 48 FA DC 66 08 31 FC FD A3 ....."+H..f.1...
00002690 0A F0 A9 F4 20 6D FA DC A0 29 70 00 20 6D FA DC .... m...)p. m..
000026A0 20 50 72 07 20 C0 20 C0 20 C0 20 C0 20 C0 20 C0  Pr. . . . . . .
000026B0 20 C0 20 C0 20 C0 20 C0 20 C0 20 C0 20 C0 20 C0  . . . . . . . .
000026C0 20 C0 20 C0 51 C9 FF DE 78 20 2B 44 FA E0 2B 7C  . .Q...x +D..+|
000026D0 00 00 10 00 FA EC 20 3C 00 00 10 00 A1 1E 20 08 ...... <...... .
000026E0 24 48 66 08 31 FC FD A3 0A F0 A9 F4 20 2D FA EC $Hf.1....... -..
000026F0 A1 1E 20 08 26 48 66 08 31 FC FD A3 0A F0 A9 F4 .. .&Hf.1.......
00002700 4A 8A 67 04 20 4A A0 1F 4A 8B 67 04 20 4B A0 1F J.g. J..J.g. K..
00002710 78 04 2F 04 4E BA FD 9A 58 4F 28 00 66 08 31 FC x./.N...XO(.f.1.
00002720 FD A3 0A F0 A9 F4 2F 04 4E BA 03 EE 58 4F 2B 43 ....../.N...XO+C
00002730 FA EC 4C DF 0C 18 4E 5E 4E 75 00 00             ..L...N^Nu..    

;; fn0000273C: 0000273C
;;   Called from:
;;     000025DA (in fn000025B4)
;;     00002BD6 (in fn00002BB4)
;;     00002BE2 (in fn00002BB4)
;;     00002BEE (in fn00002BB4)
fn0000273C proc
	link	a6,#$0000
	tst.l	-$0530(a5)
	bne	$0000274E

l00002746:
	jsr.l	0000275C                                             ; $0016(pc)
	tst.l	d0
	beq	$00002758

l0000274E:
	move.l	-$0530(a5),d0
	movea.l	d0,a0
	move.l	(a0),-$0530(a5)

l00002758:
	unlk	a6
	rts

;; fn0000275C: 0000275C
;;   Called from:
;;     00002746 (in fn0000273C)
fn0000275C proc
	link	a6,#$0000
	move.l	#$00001000,d0
	illegal	#$A11E
	move.l	a0,d2
	bne	$00002774

l0000276C:
	moveq	#$00,d0
	unlk	a6
	rts
00002772       4E 71                                       Nq            

l00002774:
	move.l	d2,d0
	move.l	d0,-$0530(a5)
	addi.l	#$00000FF8,d2
	move.l	d0,d1
	addq.l	#$08,d1
	cmp.l	d0,d2
	bls	$00002794

l00002788:
	movea.l	d0,a0
	move.l	d1,(a0)
	move.l	d1,d0
	addq.l	#$08,d1
	cmp.l	d0,d2
	bhi	$00002788

l00002794:
	movea.l	d2,a0
	moveq	#$00,d2
	move.l	d2,(a0)
	moveq	#$01,d0
	unlk	a6
	rts

;; fn000027A0: 000027A0
;;   Called from:
;;     00002546 (in fn00002510)
;;     00002AD0 (in fn00002A54)
;;     00002B36 (in fn00002B18)
fn000027A0 proc
	link	a6,#$0000
	moveq	#$12,d2
	move.l	d2,-(a7)
	jsr.l	$005A(a5)
	unlk	a6
	rts

;; fn000027B0: 000027B0
;;   Called from:
;;     0000251E (in fn00002510)
;;     0000253A (in fn00002510)
fn000027B0 proc
	link	a6,#$0000
	movem.l	d3-d6/a2-a4,-(a7)
	lea	-$052C(a5),a3
	move.l	#$0000FFFC,d6
	moveq	#$01,d5
	moveq	#$03,d4
	suba.l	a1,a1
	movea.l	-$0534(a5),a2
	cmpa.l	a2,a3
	beq	$00002814

l000027D0:
	move.l	$0008(a6),d2

l000027D4:
	lea	$0004(a2),a4
	move.l	(a4),d0
	and.l	d4,d0
	cmp.l	d5,d0
	bne	$0000280C

l000027E0:
	movea.l	(a2),a0
	move.l	$0004(a0),d0
	move.l	d0,d1
	and.w	d6,d1
	move.l	(a4),d3
	and.w	d6,d3
	sub.l	d3,d1
	subq.l	#$04,d1
	cmp.l	d2,d1
	bcc	$00002880

l000027F8:
	and.l	d4,d0
	cmp.l	d5,d0
	bne	$0000280C

l000027FE:
	move.l	(a0),(a2)
	move.l	-$0530(a5),(a0)
	move.l	a0,-$0530(a5)
	bra	$000027E0
0000280A                               4E 71                       Nq    

l0000280C:
	movea.l	(a2),a2
	cmpa.l	a3,a2
	bne	$000027D4

l00002812:
	bra	$00002818

l00002814:
	move.l	$0008(a6),d2

l00002818:
	move.l	#$0000FFFC,d6
	moveq	#$01,d5
	moveq	#$03,d4
	movea.l	-$0538(a5),a2
	cmpa.l	-$0534(a5),a2
	beq	$00002882

l0000282C:
	lea	$0004(a2),a3
	move.l	(a3),d0
	and.l	d4,d0
	cmp.l	d5,d0
	bne	$0000288C

l00002838:
	movea.l	(a2),a0
	move.l	$0004(a0),d3
	move.l	d3,d0
	and.w	d6,d0
	move.l	(a3),d1
	and.w	d6,d1
	sub.l	d1,d0
	subq.l	#$04,d0
	cmp.l	d2,d0
	bcc	$00002880

l0000284E:
	and.l	d4,d3
	cmp.l	d5,d3
	bne	$0000288C

l00002854:
	move.l	(a0),(a2)
	move.l	-$0530(a5),(a0)
	move.l	a0,-$0530(a5)
	cmpa.l	-$0534(a5),a0
	bne	$00002838

l00002864:
	move.l	a2,-$0534(a5)
	movea.l	(a2),a0
	move.l	$0004(a0),d0
	andi.w	#$FFFC,d0
	move.l	(a3),d1
	andi.w	#$FFFC,d1
	sub.l	d1,d0
	subq.l	#$04,d0
	cmp.l	d2,d0
	bcs	$00002882

l00002880:
	movea.l	a2,a1

l00002882:
	move.l	a1,d0
	movem.l	(a7)+,d3-d6/a2-a4
	unlk	a6
	rts

l0000288C:
	movea.l	(a2),a2
	cmpa.l	-$0534(a5),a2
	bne	$0000282C

l00002894:
	move.l	a1,d0
	movem.l	(a7)+,d3-d6/a2-a4
	unlk	a6
	rts
0000289E                                           00 00               ..

;; fn000028A0: 000028A0
;;   Called from:
;;     0000252C (in fn00002510)
fn000028A0 proc
	link	a6,#$0000
	movem.l	d3-d7/a2,-(a7)
	movea.w	#$0010,a2
	moveq	#-$01,d6
	moveq	#-$01,d5
	move.l	$0008(a6),d3
	addq.l	#$04,d3
	addq.l	#$03,d3
	andi.w	#$FFFC,d3
	move.l	-$050C(a5),d4
	cmp.l	-$0520(a5),d4
	bge	$0000290E

l000028C6:
	move.l	d4,d7
	lsl.l	#$04,d7

l000028CA:
	movea.l	-$0524(a5),a0
	movea.l	(a0),a0
	tst.l	(a0,d7)
	beq	$000028E4

l000028D6:
	move.l	d3,-(a7)
	move.l	d4,-(a7)
	jsr.l	00002A54                                             ; $017A(pc)
	addq.w	#$08,a7
	cmp.l	d6,d0
	bne	$000028FC

l000028E4:
	movea.l	-$0524(a5),a0
	movea.l	(a0),a0
	tst.l	(a0,d7)
	beq	$0000290C

l000028F0:
	add.l	a2,d7
	addq.l	#$01,d4
	cmp.l	-$0520(a5),d4
	blt	$000028CA

l000028FA:
	bra	$0000290E

l000028FC:
	move.l	d4,-$050C(a5)
	moveq	#$00,d0
	movem.l	(a7)+,d3-d7/a2
	unlk	a6
	rts
0000290A                               4E 71                       Nq    

l0000290C:
	move.l	d4,d5

l0000290E:
	moveq	#-$01,d7
	cmp.l	d5,d7
	bne	$000029B4

l00002916:
	move.l	-$0520(a5),d4
	moveq	#$20,d7
	add.l	d7,d4
	lsl.l	#$04,d4
	tst.l	-$0524(a5)
	beq	$0000292E

l00002926:
	move.l	d4,d0
	movea.l	-$0524(a5),a0
	illegal	#$A024

l0000292E:
	tst.l	-$0524(a5)
	beq	$0000293C

l00002934:
	movea.l	-$0510(a5),a0
	tst.w	(a0)
	beq	$00002974

l0000293C:
	move.l	d4,d0
	illegal	#$A122
	move.l	a0,d0
	movea.l	a0,a2
	bne	$00002950

l00002946:
	moveq	#-$01,d0
	movem.l	(a7)+,d3-d7/a2
	unlk	a6
	rts

l00002950:
	movea.l	a2,a0
	illegal	#$A029
	tst.l	-$0524(a5)
	beq	$00002970

l0000295A:
	move.l	-$0520(a5),d0
	lsl.l	#$04,d0
	movea.l	(a2),a1
	movea.l	-$0524(a5),a0
	movea.l	(a0),a0
	illegal	#$A02E
	movea.l	-$0524(a5),a0
	illegal	#$A023

l00002970:
	move.l	a2,-$0524(a5)

l00002974:
	moveq	#$00,d1
	move.l	-$0520(a5),d0
	lsl.l	#$04,d0
	movea.l	-$0524(a5),a0
	movea.l	(a0),a0
	adda.l	d0,a0
	moveq	#$07,d0

l00002986:
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	move.l	d1,(a0)+
	dbra	d0,$00002986

l000029AA:
	move.l	-$0520(a5),d5
	moveq	#$20,d7
	add.l	d7,-$0520(a5)

l000029B4:
	move.l	d3,-(a7)
	move.l	d5,-(a7)
	jsr.l	000029C8                                             ; $0010(pc)
	addq.w	#$08,a7
	movem.l	(a7)+,d3-d7/a2
	unlk	a6
	rts
000029C6                   00 00                               ..        

;; fn000029C8: 000029C8
;;   Called from:
;;     000029B8 (in fn000028A0)
fn000029C8 proc
	link	a6,#$0000
	movem.l	d3-d5,-(a7)
	move.l	-$0514(a5),d4
	addi.l	#$00000FFF,d4
	andi.w	#$F000,d4
	move.l	$000C(a6),d3
	cmp.l	d3,d4
	bcc	$000029E8

l000029E6:
	move.l	d3,d4

l000029E8:
	move.l	d4,d0
	addq.l	#$04,d0
	illegal	#$A11E
	move.l	a0,d0
	beq	$00002A4A

l000029F2:
	movea.l	-$0524(a5),a1
	movea.l	(a1),a1
	andi.w	#$0003,d0
	beq	$00002A14

l000029FE:
	move.l	a0,d0
	addq.l	#$03,d0
	andi.w	#$FFFC,d0
	move.l	$0008(a6),d5
	move.l	d5,d1
	lsl.l	#$04,d1
	move.l	d0,(a1,d1)
	bra	$00002A20

l00002A14:
	move.l	$0008(a6),d5
	move.l	d5,d0
	lsl.l	#$04,d0
	move.l	a0,(a1,d0)

l00002A20:
	move.l	d5,d0
	lsl.l	#$04,d0
	adda.l	d0,a1
	move.l	a0,$000C(a1)
	move.l	d4,$0008(a1)
	moveq	#$00,d4
	move.l	d4,$0004(a1)
	move.l	d3,-(a7)
	move.l	d5,-(a7)
	jsr.l	00002A54                                             ; $001C(pc)
	addq.w	#$08,a7
	tst.l	d0
	beq	$00002A4C

l00002A42:
	move.l	d5,-(a7)
	jsr.l	00002AE0                                             ; $009C(pc)
	addq.w	#$04,a7

l00002A4A:
	moveq	#-$01,d0

l00002A4C:
	movem.l	(a7)+,d3-d5
	unlk	a6
	rts

;; fn00002A54: 00002A54
;;   Called from:
;;     000028DA (in fn000028A0)
;;     00002A38 (in fn000029C8)
fn00002A54 proc
	link	a6,#$0000
	movem.l	d3-d5/a2,-(a7)
	move.l	$0008(a6),d5
	lsl.l	#$04,d5
	movea.l	-$0524(a5),a0
	movea.l	(a0),a2
	adda.l	d5,a2
	move.l	$0008(a2),d4
	move.l	$0004(a2),d1
	move.l	d4,d0
	sub.l	d1,d0
	movea.l	(a2),a0
	movea.l	a0,a1
	adda.l	d1,a1
	move.l	$000C(a6),d3
	addq.l	#$03,d3
	andi.w	#$FFFC,d3
	cmp.l	d0,d3
	bls	$00002ABE

l00002A8A:
	sub.l	d0,d4
	add.l	d3,d4
	addq.l	#$04,d4
	andi.w	#$FFFC,d4
	move.l	d4,d0
	illegal	#$A020
	movea.l	-$0524(a5),a0
	movea.l	(a0),a2
	adda.l	d5,a2
	movea.l	-$0510(a5),a0
	tst.w	(a0)
	beq	$00002AB4

l00002AA8:
	moveq	#-$01,d0
	movem.l	(a7)+,d3-d5/a2
	unlk	a6
	rts
00002AB2       4E 71                                       Nq            

l00002AB4:
	move.l	d4,$0008(a2)
	movea.l	(a2),a1
	adda.l	$0004(a2),a1

l00002ABE:
	add.l	d3,$0004(a2)
	move.l	d3,-(a7)
	move.l	a1,-(a7)
	jsr.l	00002BB4                                             ; $00EE(pc)
	addq.w	#$08,a7
	tst.l	d0
	beq	$00002AD4

l00002AD0:
	jsr.l	000027A0                                             ; -$0330(pc)

l00002AD4:
	moveq	#$00,d0
	movem.l	(a7)+,d3-d5/a2
	unlk	a6
	rts
00002ADE                                           00 00               ..

;; fn00002AE0: 00002AE0
;;   Called from:
;;     00002A44 (in fn000029C8)
fn00002AE0 proc
	link	a6,#$0000
	move.l	d3,-(a7)
	movea.l	-$0524(a5),a0
	movea.l	(a0),a0
	move.l	$0008(a6),d3
	lsl.l	#$04,d3
	tst.l	(a0,d3)
	beq	$00002AFE

l00002AF8:
	movea.l	($0C,a0,d3),a0
	illegal	#$A01F

l00002AFE:
	movea.l	-$0524(a5),a0
	movea.l	(a0),a0
	moveq	#$00,d2
	move.l	d2,(a0,d3)
	move.l	d2,($04,a0,d3)
	move.l	d2,($08,a0,d3)
	move.l	(a7)+,d3
	unlk	a6
	rts

;; fn00002B18: 00002B18
fn00002B18 proc
	link	a6,#$0000
	move.l	a2,-(a7)
	move.l	$0008(a6),d0
	beq	$00002B78

l00002B24:
	subq.l	#$04,d0
	movea.l	d0,a0
	movea.l	(a0),a2
	move.l	$0004(a2),d1
	andi.w	#$FFFC,d1
	cmp.l	a0,d1
	beq	$00002B3A

l00002B36:
	jsr.l	000027A0                                             ; -$0396(pc)

l00002B3A:
	move.l	$0004(a2),d0
	bclr.l	#$01,d0
	bset	#$0000,d0
	move.l	d0,$0004(a2)
	moveq	#-$01,d2
	cmp.l	-$051C(a5),d2
	beq	$00002B78

l00002B52:
	movea.l	-$0534(a5),a0
	cmp.l	$0004(a0),d0
	bcc	$00002B78

l00002B5C:
	movea.l	(a2),a0
	move.l	$0004(a0),d1
	andi.w	#$FFFC,d1
	andi.w	#$FFFC,d0
	sub.l	d0,d1
	subq.l	#$04,d1
	cmp.l	-$051C(a5),d1
	bcs	$00002B78

l00002B74:
	move.l	a2,-$0534(a5)

l00002B78:
	movea.l	(a7)+,a2
	unlk	a6
	rts
00002B7E                                           00 00               ..
00002B80 4E 56 00 00 24 2E 00 0C 67 22 22 2E 00 08 20 01 NV..$...g""... .
00002B90 02 40 00 03 66 16 20 02 02 40 00 03 66 0E 2F 02 .@..f. ..@..f./.
00002BA0 2F 01 4E BA 00 10 4E 5E 4E 75 4E 71 70 FF 4E 5E /.N...N^NuNqp.N^
00002BB0 4E 75 00 00                                     Nu..            

;; fn00002BB4: 00002BB4
;;   Called from:
;;     00002AC6 (in fn00002A54)
fn00002BB4 proc
	link	a6,#$FFE8
	move.l	a2,-(a7)
	move.l	a3,-(a7)
	moveq	#$00,d2
	move.l	d2,-$0014(a6)
	move.l	d2,-$0010(a6)
	move.l	d2,-$000C(a6)
	move.l	d2,-$0008(a6)
	lea	-$0014(a6),a0
	move.l	a0,-$0018(a6)
	jsr.l	0000273C                                             ; -$049A(pc)
	move.l	d0,-$0014(a6)
	beq	$00002DE8

l00002BE2:
	jsr.l	0000273C                                             ; -$04A6(pc)
	move.l	d0,-$0010(a6)
	beq	$00002DE8

l00002BEE:
	jsr.l	0000273C                                             ; -$04B2(pc)
	move.l	d0,-$000C(a6)
	beq	$00002DE8

l00002BFA:
	pea	-$0004(a6)
	movea.l	$0008(a6),a2
	move.l	a2,-(a7)
	jsr.l	00002EA8                                             ; $02A4(pc)
	addq.w	#$08,a7
	tst.l	d0
	bne	$00002C28

l00002C0E:
	movea.l	-$0004(a6),a0
	move.l	$0004(a0),d1
	moveq	#$03,d2
	and.l	d2,d1
	moveq	#$02,d2
	cmp.l	d1,d2
	bne	$00002DE8

l00002C22:
	movea.l	a0,a3
	bra	$00002C32
00002C26                   4E 71                               Nq        

l00002C28:
	movea.l	-$0018(a6),a0
	movea.l	(a0),a3
	addq.l	#$04,-$0018(a6)

l00002C32:
	move.l	a2,$0004(a3)
	move.l	a2,d1
	bclr.l	#$01,d1
	bset	#$0000,d1
	move.l	d1,$0004(a3)
	move.l	a3,(a2)
	addq.l	#$03,d0
	moveq	#$03,d2
	cmp.l	d0,d2
	bcs	$00002C60

l00002C4E:
	move.w	($0A,pc,d0.w*2),d0
	jmp.l	($06,pc,d0.w)
00002C56                   4E 71 00 7C 00 A8 01 00 01 1C       Nq.|......

l00002C60:
	movea.l	-$0004(a6),a0
	move.l	$0004(a0),d0
	moveq	#$03,d2
	and.l	d2,d0
	moveq	#$02,d1
	cmp.l	d0,d1
	bne	$00002DE8

l00002C74:
	lea	-$052C(a5),a1
	cmpa.l	(a0),a1
	bne	$00002C92

l00002C7C:
	move.l	$0004(a3),d0
	andi.w	#$FFFC,d0
	add.l	$000C(a6),d0
	cmp.l	-$0528(a5),d0
	bls	$00002C92

l00002C8E:
	move.l	d0,-$0528(a5)

l00002C92:
	pea	-$0018(a6)
	movea.l	-$0004(a6),a0
	move.l	(a0),-(a7)
	move.l	$000C(a6),-(a7)
	move.l	a3,-(a7)
	jsr.l	00002E18                                             ; $0176(pc)
	lea	$0010(a7),a7
	pea	-$0018(a6)
	move.l	a3,-(a7)
	movea.l	-$0004(a6),a0
	movea.l	(a0),a0
	move.l	$0004(a0),d0
	andi.w	#$FFFC,d0
	movea.l	-$0004(a6),a0
	move.l	$0004(a0),d1
	andi.w	#$FFFC,d1
	sub.l	d1,d0
	move.l	d0,-(a7)
	move.l	a0,-(a7)
	bra	$00002DA4
00002CD4             20 4A D1 EE 00 0C 2B 48 FA D8 48 6E      J....+H..Hn
00002CE0 FF E8 48 6D FA D4 2F 2E 00 0C 2F 0B 4E BA 01 2A ..Hm../.../.N..*
00002CF0 4F EF 00 10 2B 4B FA CC 2B 4B FA C8 60 00 00 AE O...+K..+K..`...
00002D00 48 6E FF FC 20 6D FA D8 53 48 2F 08 4E BA 01 9A Hn.. m..SH/.N...
00002D10 50 4F 74 01 B4 80 67 04 4E BA FA 86 20 6E FF FC POt...g.N... n..
00002D20 20 50 22 28 00 04 02 41 FF FC 20 6E FF FC 20 28  P"(...A.. n.. (
00002D30 00 04 02 40 FF FC 92 80 20 4A D1 EE 00 0C 2B 48 ...@.... J....+H
00002D40 FA D8 41 ED FA D4 26 88 48 6E FF E8 2F 0B 2F 01 ..A...&.Hn.././.
00002D50 2F 2E FF FC 60 4E 4E 71 48 6E FF E8 2F 2D FA C8 /...`NNqHn../-..
00002D60 2F 2E 00 0C 2F 0B 4E BA 00 B0 4F EF 00 10 2B 4B /.../.N...O...+K
00002D70 FA C8 60 38 20 6E FF FC 20 10 41 ED FA D4 B0 88 ..`8 n.. .A.....
00002D80 66 16 20 6E FF FC 20 28 00 04 02 40 FF FC D0 AE f. n.. (...@....
00002D90 00 0C 2B 40 FA D8 60 14 48 6E FF E8 2F 00 2F 2E ..+@..`.Hn.././.
00002DA0 00 0C 2F 0B                                     ../.            

l00002DA4:
	jsr.l	00002E18                                             ; $0074(pc)
	lea	$0010(a7),a7
	movea.l	-$0534(a5),a0
	move.l	$0004(a0),d0
	andi.w	#$FFFC,d0
	cmp.l	a2,d0
	bls	$00002DDC

l00002DBC:
	movea.l	(a3),a0
	move.l	$0004(a0),d0
	andi.w	#$FFFC,d0
	move.l	$0004(a3),d1
	andi.w	#$FFFC,d1
	sub.l	d1,d0
	subq.l	#$04,d0
	cmp.l	-$051C(a5),d0
	bcs	$00002DDC

l00002DD8:
	move.l	a3,-$0534(a5)

l00002DDC:
	moveq	#$00,d0
	movea.l	(a7)+,a3
	movea.l	(a7)+,a2
	unlk	a6
	rts
00002DE6                   4E 71                               Nq        

l00002DE8:
	movea.l	-$0018(a6),a0
	tst.l	(a0)
	beq	$00002E0E

l00002DF0:
	movea.l	-$0018(a6),a0
	movea.l	(a0),a0
	move.l	-$0530(a5),(a0)
	movea.l	-$0018(a6),a0
	move.l	(a0),-$0530(a5)
	addq.l	#$04,-$0018(a6)
	movea.l	-$0018(a6),a0
	tst.l	(a0)
	bne	$00002DF0

l00002E0E:
	moveq	#-$01,d0
	movea.l	(a7)+,a3
	movea.l	(a7)+,a2
	unlk	a6
	rts

;; fn00002E18: 00002E18
;;   Called from:
;;     00002CA2 (in fn00002BB4)
;;     00002DA4 (in fn00002BB4)
fn00002E18 proc
	link	a6,#$0000
	move.l	a2,-(a7)
	move.l	a3,-(a7)
	movea.l	$0008(a6),a2
	move.l	$0004(a2),d2
	move.l	d2,d0
	moveq	#$03,d1
	and.l	d1,d0
	moveq	#$02,d1
	cmp.l	d0,d1
	beq	$00002E9A

l00002E34:
	movea.l	$0010(a6),a0
	addq.w	#$04,a0
	move.l	(a0),d1
	move.l	d1,d0
	andi.l	#$00000003,d0
	cmpi.l	#$00000002,d0
	bne	$00002E6C

l00002E4C:
	andi.w	#$FFFC,d2
	add.l	$000C(a6),d2
	move.l	d2,(a0)
	bclr.l	#$00,d2
	bset	#$0001,d2
	move.l	d2,(a0)
	move.l	$0010(a6),(a2)
	movea.l	(a7)+,a3
	movea.l	(a7)+,a2
	unlk	a6
	rts

l00002E6C:
	andi.w	#$FFFC,d2
	add.l	$000C(a6),d2
	andi.w	#$FFFC,d1
	cmp.l	d2,d1
	beq	$00002E9A

l00002E7C:
	movea.l	$0014(a6),a1
	movea.l	(a1),a0
	movea.l	(a0)+,a3
	move.l	a0,(a1)
	move.l	d2,$0004(a3)
	bclr.l	#$00,d2
	bset	#$0001,d2
	move.l	d2,$0004(a3)
	move.l	a3,(a2)
	movea.l	a3,a2

l00002E9A:
	move.l	$0010(a6),(a2)
	movea.l	(a7)+,a3
	movea.l	(a7)+,a2
	unlk	a6
	rts
00002EA6                   00 00                               ..        

;; fn00002EA8: 00002EA8
;;   Called from:
;;     00002C04 (in fn00002BB4)
fn00002EA8 proc
	link	a6,#$0000
	lea	-$052C(a5),a0
	cmpa.l	-$0538(a5),a0
	bne	$00002EBC

l00002EB6:
	moveq	#-$03,d0
	unlk	a6
	rts

l00002EBC:
	movea.l	-$0538(a5),a0
	move.l	$0004(a0),d0
	andi.w	#$FFFC,d0
	move.l	$0008(a6),d2
	cmp.l	d2,d0
	bls	$00002ED8

l00002ED0:
	moveq	#-$01,d0
	unlk	a6
	rts
00002ED6                   4E 71                               Nq        

l00002ED8:
	move.l	-$0528(a5),d0
	andi.w	#$FFFC,d0
	cmp.l	d2,d0
	bhi	$00002EEC

l00002EE4:
	moveq	#-$02,d0
	unlk	a6
	rts
00002EEA                               4E 71                       Nq    

l00002EEC:
	move.l	#$0000FFFC,d0
	movea.l	-$0538(a5),a1

l00002EF6:
	movea.l	(a1),a0
	move.l	$0004(a0),d1
	and.w	d0,d1
	cmp.l	d2,d1
	bhi	$00002F08

l00002F02:
	movea.l	a0,a1
	bra	$00002EF6
00002F06                   4E 71                               Nq        

l00002F08:
	movea.l	$000C(a6),a0
	move.l	a1,(a0)
	moveq	#$00,d0
	move.l	$0004(a1),d1
	andi.w	#$FFFC,d1
	cmp.l	d2,d1
	beq	$00002F1E

l00002F1C:
	moveq	#$01,d0

l00002F1E:
	unlk	a6
	rts
00002F22       00 00                                       ..            
