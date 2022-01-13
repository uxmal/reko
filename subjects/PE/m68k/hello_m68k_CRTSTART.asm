;;; Segment CRTSTART (00002130)
00002130 00 20 00 04 4E 56 FF D8 20 38 03 16 20 40 67 20 . ..NV.. 8.. @g 
00002140 08 00 00 00 66 1A 0C 90 4D 50 47 4D 66 12 20 28 ....f...MPGMf. (
00002150 00 04 20 40 67 0A 0C 50 53 48 66 04 2B 48 FA A0 .. @g..PSHf.+H..
00002160 74 00 2B 42 FA 98 4A AD FA A0 66 40 74 01 2B 42 t.+B..J...f@t.+B
00002170 F9 4C 43 EE FF E0 70 01 30 7C 09 10 22 D8 22 D8 .LC...p.0|..".".
00002180 22 D8 22 D8 51 C8 FF F6 48 6E FF E0 4E BA 00 D6 ".".Q...Hn..N...
00002190 58 4F 2D 40 FF D8 74 00 2D 42 FF DC 41 EE FF D8 XO-@..t.-B..A...
000021A0 2B 48 F9 50 2B 42 FA A4 60 22 4E 71 4E AD 00 6A +H.P+B..`"NqN..j
000021B0 2B 40 FA A4 58 AD FA A4 20 6D FA A0 2B 68 00 02 +@..X... m..+h..
000021C0 F9 4C 2B 68 00 06 F9 50 4E BA 00 26 4E BA 00 F6 .L+h...PN..&N...
000021D0 2F 2D FA 98 2F 2D F9 50 2F 2D F9 4C 4E AD 00 2A /-../-.P/-.LN..*
000021E0 4F EF 00 0C 2F 00 4E BA 00 AC 4E 5E 4E 75 00 00 O.../.N...N^Nu..

;; fn000021F0: 000021F0
fn000021F0 proc
	link	a6,#$0000
	movem.l	d3-d4/a2,-(a7)
	movea.l	-$0560(a5),a0
	move.l	$001C(a0),d0
	movea.l	d0,a1
	beq	$0000225A

l00002204:
	moveq	#$03,d4
	lea	-$0668(a5),a2
	move.l	#$46535953,d3
	move.l	#$45434F4E,d2
	moveq	#$00,d1

l00002218:
	movea.l	$0004(a1),a0
	move.l	(a0),d0
	cmp.l	d2,d0
	beq	$00002228

l00002222:
	cmp.l	d3,d0
	beq	$00002238

l00002226:
	bra	$00002250

l00002228:
	lea	-$06A8(a5),a0
	ori.b	#$41,(a0,d1)
	move.l	a1,(a2,d1*4)
	bra	$00002250

l00002238:
	lea	-$06A8(a5),a0
	ori.b	#$01,(a0,d1)
	movea.l	$0008(a1),a0
	movea.l	(a0),a0
	movea.w	$0002(a0),a0
	move.l	a0,(a2,d1*4)

l00002250:
	lea	$0014(a1),a1
	addq.l	#$01,d1
	cmp.l	d4,d1
	blt	$00002218

l0000225A:
	movem.l	(a7)+,d3-d4/a2
	unlk	a6
	rts
00002262       00 00                                       ..            

;; fn00002264: 00002264
fn00002264 proc
	link	a6,#$0000
	move.l	a2,-(a7)
	movea.l	$0008(a6),a0
	move.l	a0,d0
	beq	$0000228C

l00002272:
	move.b	(a0),d1
	beq	$0000228C

l00002276:
	movea.l	d0,a2
	moveq	#$00,d0
	move.b	d1,d0
	tst.l	d0
	lea	$0001(a0),a1
	beq	$0000228A

l00002284:
	move.b	(a1)+,(a2)+
	subq.l	#$01,d0
	bne	$00002284

l0000228A:
	clr.b	(a2)

l0000228C:
	move.l	a0,d0
	movea.l	(a7)+,a2
	unlk	a6
	rts

;; fn00002294: 00002294
fn00002294 proc
	link	a6,#$0000
	moveq	#$00,d2
	move.l	d2,-(a7)
	move.l	d2,-(a7)
	move.l	$0008(a6),-(a7)
	jsr.l	00002354                                             ; $00B2(pc)
	unlk	a6
	rts
000022AA                               00 00 4E 56 00 00           ..NV..
000022B0 74 00 2F 02 72 01 2F 01 2F 2E 00 08 4E BA 00 96 t./.r././...N...
000022C0 4E 5E 4E 75                                     N^Nu            

;; fn000022C4: 000022C4
fn000022C4 proc
	link	a6,#$FFFC
	move.l	a2,-(a7)
	pea	-$02E4(a5)
	pea	-$02E8(a5)
	jsr.l	000023B4                                             ; $00E2(pc)
	addq.w	#$08,a7
	pea	-$02EC(a5)
	pea	-$02F0(a5)
	jsr.l	000023B4                                             ; $00D4(pc)
	addq.w	#$04,a7
	move.w	#$A89F,-(a7)
	move.b	#$01,-(a7)
	jsr.l	$00C2(a5)
	movea.l	(a7)+,a2
	subq.w	#$04,a7
	move.w	#$A1AD,-(a7)
	clr.b	-(a7)
	jsr.l	$00C2(a5)
	cmpa.l	(a7)+,a2
	beq	$0000231C

l00002304:
	lea	-$0004(a6),a1
	move.l	#$73797376,d0
	illegal	#$A1AD
	move.l	a0,(a1)
	tst.w	d0
	bne	$0000231C

l00002316:
	move.l	-$0004(a6),-$06B8(a5)

l0000231C:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	unlk	a6
	rts
00002324             4E 56 00 00 74 01 2F 02 72 00 2F 01     NV..t./.r./.
00002330 2F 01 4E BA 00 20 4E 5E 4E 75 00 00 4E 56 00 00 /.N.. N^Nu..NV..
00002340 74 01 2F 02 2F 02 72 00 2F 01 4E BA 00 08 4E 5E t././.r./.N...N^
00002350 4E 75 00 00                                     Nu..            

;; fn00002354: 00002354
;;   Called from:
;;     000022A2 (in fn00002294)
fn00002354 proc
	link	a6,#$0000
	move.b	$0013(a6),-$0564(a5)
	tst.l	$000C(a6)
	bne	$00002386

l00002364:
	tst.l	-$02C4(a5)
	beq	$00002378

l0000236A:
	move.l	-$02C8(a5),-(a7)
	move.l	-$02C4(a5),-(a7)
	jsr.l	000023B4                                             ; $0042(pc)
	addq.w	#$08,a7

l00002378:
	pea	-$02D8(a5)
	pea	-$02E0(a5)
	jsr.l	000023B4                                             ; $0034(pc)
	addq.w	#$08,a7

l00002386:
	pea	-$02D0(a5)
	pea	-$02D4(a5)
	jsr.l	000023B4                                             ; $0026(pc)
	addq.w	#$08,a7
	tst.l	$0010(a6)
	bne	$000023AE

l0000239A:
	tst.l	-$0560(a5)
	beq	$000023AA

l000023A0:
	movea.l	-$0560(a5),a0
	move.l	$0008(a6),$000E(a0)

l000023AA:
	jsr.l	$0062(a5)

l000023AE:
	unlk	a6
	rts
000023B2       00 00                                       ..            

;; fn000023B4: 000023B4
;;   Called from:
;;     000022D2 (in fn000022C4)
;;     000022E0 (in fn000022C4)
;;     00002372 (in fn00002354)
;;     00002380 (in fn00002354)
;;     0000238E (in fn00002354)
fn000023B4 proc
	link	a6,#$0000
	movem.l	d3-d5,-(a7)
	moveq	#-$01,d5
	move.l	$000C(a6),d4
	move.l	$0008(a6),d3
	cmp.l	d3,d4
	bls	$000023DE

l000023CA:
	movea.l	d3,a0
	move.l	(a0),d0
	beq	$000023D8

l000023D0:
	cmp.l	d5,d0
	beq	$000023D8

l000023D4:
	movea.l	d0,a0
	jsr.l	(a0)

l000023D8:
	addq.l	#$04,d3
	cmp.l	d3,d4
	bhi	$000023CA

l000023DE:
	movem.l	(a7)+,d3-d5
	unlk	a6
	rts
000023E6                   00 00 4E 56 00 00 74 02 2F 02       ..NV..t./.
000023F0 4E BA 00 06 4E 5E 4E 75                         N...N^Nu        

;; fn000023F8: 000023F8
fn000023F8 proc
	link	a6,#$0000
	jsr.l	00002418                                             ; $001C(pc)
	move.l	$0008(a6),-(a7)
	jsr.l	0000243C                                             ; $0038(pc)
	addq.w	#$04,a7
	pea	$000000FF
	movea.l	-$0508(a5),a0
	jsr.l	(a0)
	unlk	a6
	rts

;; fn00002418: 00002418
;;   Called from:
;;     000023FC (in fn000023F8)
fn00002418 proc
	link	a6,#$0000
	pea	$000000FC
	jsr.l	0000243C                                             ; $001C(pc)
	addq.w	#$04,a7
	movea.l	-$03F4(a5),a0
	move.l	a0,d0
	beq	$00002430

l0000242E:
	jsr.l	(a0)

l00002430:
	pea	$000000FF
	jsr.l	0000243C                                             ; $0008(pc)
	unlk	a6
	rts

;; fn0000243C: 0000243C
;;   Called from:
;;     00002404 (in fn000023F8)
;;     00002420 (in fn00002418)
;;     00002434 (in fn00002418)
fn0000243C proc
	link	a6,#$0000
	move.l	a2,-(a7)
	lea	-$03F4(a5),a1
	moveq	#$00,d1
	lea	-$044C(a5),a0
	move.l	a0,d2
	move.l	$0008(a6),d0

l00002452:
	movea.l	d2,a0
	cmp.l	(a0),d0
	beq	$00002460

l00002458:
	addq.l	#$08,d2
	addq.l	#$01,d1
	cmp.l	a1,d2
	bcs	$00002452

l00002460:
	lea	-$044C(a5),a0
	cmp.l	(a0,d1*8),d0
	bne	$0000248E

l0000246A:
	lea	-$0448(a5),a0
	movea.l	(a0,d1*8),a0
	movea.l	a0,a1
	lea	$0001(a1),a2

l00002478:
	tst.b	(a1)+
	bne	$00002478

l0000247C:
	suba.l	a2,a1
	move.l	a1,-(a7)
	move.l	a0,-(a7)
	moveq	#$02,d2
	move.l	d2,-(a7)
	jsr.l	$0092(a5)
	lea	$000C(a7),a7

l0000248E:
	movea.l	(a7)+,a2
	unlk	a6
	rts
