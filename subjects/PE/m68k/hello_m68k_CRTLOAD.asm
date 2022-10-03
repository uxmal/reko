;;; Segment CRTLOAD (00001464)
00001464             00 00 00 01 9D CE 4A 38 02 8E 6A 02     ......J8..j.
00001470 A9 F4 4A 38 01 2F 66 04 4E BA 00 22 42 A7 2F 0D ..J8./f.N.."B./.
00001480 4E BA 00 66 4F EF 00 08 4E BA 00 0E 4E BA 00 0A N..fO...N...N...
00001490 4E BA 00 06 4E ED 00 42                         N...N..B        

;; fn00001498: 00001498
fn00001498 proc
	rts
0000149A                               00 00                       ..    

;; fn0000149C: 0000149C
fn0000149C proc
	suba.l	#$00000004,a7
	move.l	#$414C5254,-(a7)
	move.w	#$4270,-(a7)
	illegal	#$A9A0
	cmpi.l	#$00000000,(a7)
	bne	$000014BE

l000014B6:
	move.w	#$001A,$00000AF0
	bra	$000014E2

l000014BE:
	pea	-$06CE(a5)
	illegal	#$A86E
	illegal	#$A8FE
	illegal	#$A912
	illegal	#$A930
	illegal	#$A9CC
	move.l	#$00000000,-(a7)
	illegal	#$A97B
	illegal	#$A850
	move.w	#$4270,-(a7)
	move.l	#$00000000,-(a7)
	illegal	#$A986

l000014E2:
	addq.l	#$04,a7
	illegal	#$A9F4
	ori.b	#$E7,d0

;; fn000014E8: 000014E8
fn000014E8 proc
	movem.l	a2-a3,-(a7)
	subq.w	#$04,a7
	move.l	#$44415441,-(a7)
	clr.w	-(a7)
	illegal	#$A9A0
	tst.l	(a7)
	beq	$00001592

l000014FE:
	movea.l	(a7),a3
	movea.l	(a3),a3
	movea.l	a5,a2
	move.l	(a3),d0
	suba.l	d0,a2
	movea.l	a2,a0
	lsr.l	#$01,d0
	beq	$00001514

l0000150E:
	clr.w	(a0)+
	subq.l	#$01,d0
	bne	$0000150E

l00001514:
	move.l	$0004(a3),d0
	suba.l	d0,a2
	movea.l	a2,a1
	lea	$0010(a3),a0
	illegal	#$A02E
	move.l	$0008(a3),d0
	suba.l	d0,a2
	movea.l	a2,a0
	lsr.l	#$01,d0
	beq	$00001534

l0000152E:
	clr.w	(a0)+
	subq.l	#$01,d0
	bne	$0000152E

l00001534:
	move.l	$000C(a3),d0
	suba.l	d0,a2
	lea	$0010(a3),a0
	move.l	$0004(a3),d1
	adda.l	d1,a0
	movea.l	a2,a1
	movea.l	$000C(a3),a3
	adda.l	a0,a3
	illegal	#$A02E
	move.l	$0014(a7),d1
	move.l	$0010(a7),d2

l00001556:
	moveq	#$00,d0
	move.b	(a3)+,d0
	bgt	$00001576

l0000155C:
	blt	$00001572

l0000155E:
	move.b	(a3)+,d0
	beq	$0000159A

l00001562:
	lsl.w	#$08,d0
	move.b	(a3)+,d0
	swap.l	d0
	move.b	(a3)+,d0
	lsl.w	#$08,d0
	move.b	(a3)+,d0
	add.l	d0,d0
	bra	$00001578

l00001572:
	lsl.w	#$08,d0
	move.b	(a3)+,d0

l00001576:
	add.w	d0,d0

l00001578:
	adda.l	d0,a2
	tst.l	d1
	beq	$00001582

l0000157E:
	tst.l	(a2)
	bge	$00001586

l00001582:
	add.l	d2,(a2)
	bra	$0000158E

l00001586:
	movea.l	(a2),a0
	adda.l	d1,a0
	move.l	$0002(a0),(a2)

l0000158E:
	bra	$00001556
00001590 60 08                                           `.              

l00001592:
	move.w	#$FDA3,$00000AF0
	illegal	#$A9F4

l0000159A:
	illegal	#$A9A3
	movem.l	(a7)+,a2-a3
	rts
000015A2       00 00                                       ..            
