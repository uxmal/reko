;;; Segment CRTSTDIO (000015A4)
000015A4             00 10 00 02 4E 56 00 00 2F 03 2F 04     ....NV.././.
000015B0 48 6D F5 A0 4E BA 00 32 58 4F 26 00 48 6E 00 0C Hm..N..2XO&.Hn..
000015C0 2F 2E 00 08 48 6D F5 A0 4E BA 01 06 4F EF 00 0C /...Hm..N...O...
000015D0 28 00 48 6D F5 A0 2F 03 4E BA 00 A6 50 4F 20 04 (.Hm../.N...PO .
000015E0 28 1F 26 1F 4E 5E 4E 75                         (.&.N^Nu        

;; fn000015E8: 000015E8
fn000015E8 proc
	link	a6,#$0000
	move.l	d3,-(a7)
	move.l	a2,-(a7)
	movea.l	$0008(a6),a2
	move.l	$0010(a2),-(a7)
	jsr.l	$0082(a5)
	addq.w	#$04,a7
	tst.l	d0
	beq	$0000162A

l00001602:
	lea	-$0A60(a5),a0
	cmpa.l	a0,a2
	bne	$00001610

l0000160A:
	moveq	#$00,d3
	bra	$0000161A
0000160E                                           4E 71               Nq

l00001610:
	lea	-$0A40(a5),a0
	cmpa.l	a0,a2
	bne	$00001628

l00001618:
	moveq	#$01,d3

l0000161A:
	addq.l	#$01,-$0558(a5)
	move.l	$000C(a2),d0
	andi.w	#$010C,d0
	beq	$00001634

l00001628:
	moveq	#$00,d0

l0000162A:
	movea.l	(a7)+,a2
	move.l	(a7)+,d3
	unlk	a6
	rts
00001632       4E 71                                       Nq            

l00001634:
	lea	-$0A88(a5),a0
	tst.l	(a0,d3*4)
	bne	$00001652

l0000163E:
	pea	$00000200
	jsr.l	$0072(a5)
	addq.w	#$04,a7
	lea	-$0A88(a5),a0
	move.l	d0,(a0,d3*4)
	beq	$0000162A

l00001652:
	lea	-$0A88(a5),a0
	move.l	(a0,d3*4),d0
	move.l	d0,$0008(a2)
	move.l	d0,(a2)
	move.l	#$00000200,$0018(a2)
	move.l	#$00000200,$0004(a2)
	ori.w	#$1102,$000E(a2)
	moveq	#$01,d0
	movea.l	(a7)+,a2
	move.l	(a7)+,d3
	unlk	a6
	rts

;; fn00001680: 00001680
fn00001680 proc
	link	a6,#$0000
	move.l	a2,-(a7)
	tst.l	$0008(a6)
	movea.l	$000C(a6),a2
	beq	$000016B8

l00001690:
	btst.w	#$0004,$000E(a2)
	beq	$000016C8

l00001698:
	move.l	a2,-(a7)
	jsr.l	00001D80                                             ; $06E6(pc)
	addq.w	#$04,a7
	andi.w	#$EEFF,$000E(a2)
	moveq	#$00,d2
	move.l	d2,$0018(a2)
	move.l	d2,(a2)
	move.l	d2,$0008(a2)
	movea.l	(a7)+,a2
	unlk	a6
	rts

l000016B8:
	btst.w	#$0004,$000E(a2)
	beq	$000016C8

l000016C0:
	move.l	a2,-(a7)
	jsr.l	00001D80                                             ; $06BE(pc)
	addq.w	#$04,a7

l000016C8:
	movea.l	(a7)+,a2
	unlk	a6
	rts
000016CE                                           00 00               ..

;; fn000016D0: 000016D0
fn000016D0 proc
	link	a6,#$FDE0
	movem.l	d3-d7/a2-a4,-(a7)
	lea	-$07F8(a5),a3
	lea	-$0818(a5),a2
	moveq	#$00,d7
	move.l	d7,-$0220(a6)
	move.l	d7,-$0008(a6)
	movea.l	$000C(a6),a0
	move.b	(a0),d6
	addq.l	#$01,$000C(a6)
	tst.b	d6
	beq	$00001C34

l000016FA:
	move.l	-$000C(a6),d3
	move.l	-$0010(a6),d4
	move.l	-$0014(a6),d5
	movea.l	-$0018(a6),a4
	move.l	-$001C(a6),d7

l0000170E:
	tst.l	-$0220(a6)
	blt	$00001C34

l00001716:
	cmpi.b	#$20,d6
	blt	$00001734

l0000171C:
	cmpi.b	#$78,d6
	bgt	$00001734

l00001722:
	move.b	d6,d0
	extb.l	d0
	move.b	(a2,d0),d0
	andi.b	#$0F,d0
	extb.l	d0
	bra	$00001736
00001732       4E 71                                       Nq            

l00001734:
	moveq	#$00,d0

l00001736:
	lsl.l	#$03,d0
	add.l	-$0008(a6),d0
	move.b	(a3,d0),d0
	asr.b	#$04,d0
	extb.l	d0
	move.l	d0,-$0008(a6)
	moveq	#$07,d2
	cmp.l	d0,d2
	bcs	$00001C24

l00001750:
	move.w	($08,pc,d0.w*2),d0
	jmp.l	($04,pc,d0.w)
00001758                         00 10 00 28 00 40 00 A4         ...(.@..
00001760 00 E4 00 EC 01 20 01 44 48 6E FD E0 2F 2E 00 08 ..... .DHn../...
00001770 49 C6 2F 06 4E BA 04 CA 4F EF 00 0C 60 00 04 A6 I./.N...O...`...
00001780 7C 00 2D 46 FF F4 2D 46 FF F0 2D 46 FF EC 7A 00 |.-F..-F..-F..z.
00001790 76 00 78 FF 60 00 04 8E 49 C6 74 20 9C 82 72 10 v.x.`...I.t ..r.
000017A0 B2 86 65 00 04 80 1C 3B 60 18 3C 3B 62 08 4E FB ..e....;`.<;b.N.
000017B0 60 04 4E 71 00 1E 00 28 00 30 00 38 00 40 04 70 `.Nq...(.0.8.@.p
000017C0 00 05 05 01 05 05 05 05 05 05 05 02 05 03 05 05 ................
000017D0 04 00 08 C3 00 01 60 00 04 4C 4E 71 08 C3 00 07 ......`..LNq....
000017E0 60 00 04 42 08 C3 00 00 60 00 04 3A 08 C3 00 02 `..B....`..:....
000017F0 60 00 04 32 08 C3 00 03 60 00 04 2A 0C 06 00 2A `..2....`..*...*
00001800 66 1E 48 6E 00 10 4E BA 05 04 58 4F 2D 40 FF EC f.Hn..N...XO-@..
00001810 6C 00 04 12 08 C3 00 02 44 AE FF EC 60 00 04 06 l.......D...`...
00001820 49 C6 20 2E FF EC 22 00 E5 89 D2 80 D2 81 DC 81 I. ...".........
00001830 74 30 9C 82 2D 46 FF EC 60 00 03 EA 78 00 60 00 t0..-F..`...x.`.
00001840 03 E4 4E 71 0C 06 00 2A 66 16 48 6E 00 10 4E BA ..Nq...*f.Hn..N.
00001850 04 BC 58 4F 28 00 6C 00 03 CC 78 FF 60 00 03 C6 ..XO(.l...x.`...
00001860 49 C6 20 04 E5 88 D0 84 D0 80 DC 80 78 30 9C 84 I. .........x0..
00001870 28 06 60 00 03 B0 4E 71 49 C6 74 4C B4 86 67 0C (.`...NqI.tL..g.
00001880 72 68 B2 86 67 0E 60 00 03 9C 4E 71 08 C3 00 0A rh..g.`...Nq....
00001890 60 00 03 92 08 C3 00 05 60 00 03 8A 10 06 49 C0 `.......`.....I.
000018A0 74 45 90 82 72 33 B2 80 65 00 02 B2 10 3B 00 26 tE..r3..e....;.&
000018B0 30 3B 02 06 4E FB 00 02 00 50 00 50 00 DA 00 70 0;..N....P.P...p
000018C0 00 88 00 5A 00 88 00 90 00 C0 00 D0 01 18 01 58 ...Z...........X
000018D0 01 5C 02 A4 00 0D 01 0D 0D 0D 0D 0D 0D 0D 0D 0D .\..............
000018E0 0D 0D 0D 0D 0D 0D 0D 02 0D 0D 0D 0D 0D 0D 0D 0D ................
000018F0 0D 0D 03 04 05 05 05 0D 06 0D 0D 0D 0D 07 08 09 ................
00001900 0D 0D 0A 0D 0B 0D 0D 0C 7E 01 2D 47 FF F4 06 06 ........~.-G....
00001910 00 20 08 C3 00 06 49 EE FD E4 4A 84 6C 00 01 96 . ....I...J.l...
00001920 78 06 60 00 01 9C 4E 71 48 6E 00 10 4E BA 03 DE x.`...NqHn..N...
00001930 58 4F 1D 40 FD E4 49 EE FD E4 7E 01 60 00 02 1E XO.@..I...~.`...
00001940 08 C3 00 06 7C 0A 60 6C 48 6E 00 10 4E BA 03 BE ....|.`lHn..N...
00001950 58 4F 20 40 08 03 00 05 67 0E 30 AE FD E2 7C 01 XO @....g.0...|.
00001960 2D 46 FF F0 60 00 01 F6 20 AE FD E0 7C 01 2D 46 -F..`... ...|.-F
00001970 FF F0 60 00 01 E8 4E 71 7C 08 08 03 00 07 67 34 ..`...Nq|.....g4
00001980 08 C3 00 09 60 2E 4E 71 78 08 08 83 00 05 08 C3 ....`.Nqx.......
00001990 00 04 7E 07 2D 47 FF E8 7C 10 08 03 00 07 67 14 ..~.-G..|.....g.
000019A0 1D 7C 00 30 FF FE 20 2E FF E8 06 00 00 51 1D 40 .|.0.. ......Q.@
000019B0 FF FF 7A 02 08 03 00 05 67 72 2E 03 74 40 CE 82 ..z.....gr..t@..
000019C0 48 6E 00 10 67 56 4E BA 03 44 58 4F 48 C0 60 6C Hn..gVN..DXOH.`l
000019D0 48 6E 00 10 4E BA 03 36 58 4F 4A 80 28 40 66 04 Hn..N..6XOJ.(@f.
000019E0 28 6D F8 04 22 3C 7F FF FF FF 7E FF BE 84 67 02 (m.."<....~...g.
000019F0 22 04 20 4C 20 01 53 81 4A 80 67 0E 4A 10 67 0A ". L .S.J.g.J.g.
00001A00 52 48 20 01 53 81 4A 80 66 F2 91 CC 60 00 01 4C RH .S.J.f...`..L
00001A10 7C 0A 60 A0 7E 27 60 00 FF 7C 4E 71 4E BA 02 EE |.`.~'`..|NqN...
00001A20 58 4F 72 00 32 00 20 01 60 12 4E 71 2E 03 74 40 XOr.2. .`.Nq..t@
00001A30 CE 82 48 6E 00 10 4E BA 02 D4 58 4F 4A 87 67 0A ..Hn..N...XOJ.g.
00001A40 4A 80 6C 06 44 80 08 C3 00 08 4A 84 6C 06 78 01 J.l.D.....J.l.x.
00001A50 60 06 4E 71 08 83 00 03 4A 80 66 02 7A 00 49 EE `.Nq....J.f.z.I.
00001A60 FF E3 22 04 53 84 4A 81 6E 04 4A 80 67 1E 24 00 ..".S.J.n.J.g.$.
00001A70 4C 46 20 01 7E 30 D2 87 4C 46 00 00 74 39 B4 81 LF .~0..LF..t9..
00001A80 6C 04 D2 AE FF E8 18 81 53 4C 60 D6 41 EE FF E3 l.......SL`.A...
00001A90 91 CC 2E 08 52 4C 08 03 00 09 67 00 00 C0 0C 14 ....RL....g.....
00001AA0 00 30 66 06 4A 87 66 00 00 B4 19 3C 00 30 52 87 .0f.J.f....<.0R.
00001AB0 60 00 00 AA 4A 84 66 08 0C 06 00 67 66 02 78 01 `...J.f....gf.x.
00001AC0 08 03 00 0A 67 26 2F 2E FF F4 2F 04 10 06 49 C0 ....g&/.../...I.
00001AD0 2F 00 48 6E FD E4 2F 2E 00 10 20 6D FA C0 4E 90 /.Hn../... m..N.
00001AE0 4F EF 00 14 7E 0C DF AE 00 10 60 22 2F 2E FF F4 O...~.....`"/...
00001AF0 2F 04 10 06 49 C0 2F 00 48 6E FD E4 2F 2E 00 10 /...I./.Hn../...
00001B00 20 6D FA AC 4E 90 4F EF 00 14 50 AE 00 10 2E 03  m..N.O...P.....
00001B10 02 87 00 00 00 80 67 10 4A 84 66 0C 48 6E FD E4 ......g.J.f.Hn..
00001B20 20 6D FA B8 4E 90 58 4F 0C 06 00 67 66 10 4A 87  m..N.XO...gf.J.
00001B30 66 0C 48 6E FD E4 20 6D FA B0 4E 90 58 4F 0C 2E f.Hn.. m..N.XO..
00001B40 00 2D FD E4 66 08 08 C3 00 08 49 EE FD E5 20 4C .-..f.....I... L
00001B50 43 E8 00 01 4A 18 66 FC 91 C9 2E 08 4A AE FF F0 C...J.f.....J...
00001B60 66 00 00 C2 08 03 00 06 67 2C 08 03 00 08 67 08 f.......g,....g.
00001B70 1D 7C 00 2D FF FE 60 1C 08 03 00 00 67 0A 1D 7C .|.-..`.....g..|
00001B80 00 2B FF FE 60 0E 4E 71 08 03 00 01 67 08 1D 7C .+..`.Nq....g..|
00001B90 00 20 FF FE 7A 01 2C 2E FF EC 9C 87 9C 85 20 03 . ..z.,....... .
00001BA0 02 40 00 0C 66 16 48 6E FD E0 2F 2E 00 08 2F 06 .@..f.Hn../.../.
00001BB0 74 20 2F 02 4E BA 00 CE 4F EF 00 10 48 6E FD E0 t /.N...O...Hn..
00001BC0 2F 2E 00 08 2F 05 48 6E FF FE 4E BA 00 F8 4F EF /.../.Hn..N...O.
00001BD0 00 10 08 03 00 03 67 1C 08 03 00 02 66 16 48 6E ......g.....f.Hn
00001BE0 FD E0 2F 2E 00 08 2F 06 74 30 2F 02 4E BA 00 96 ../.../.t0/.N...
00001BF0 4F EF 00 10 48 6E FD E0 2F 2E 00 08 2F 07 2F 0C O...Hn../..././.
00001C00 4E BA 00 C2 4F EF 00 10 08 03 00 02 67 16 48 6E N...O.......g.Hn
00001C10 FD E0 2F 2E 00 08 2F 06 7C 20 2F 06 4E BA 00 66 ../.../.| /.N..f
00001C20 4F EF 00 10                                     O...            

l00001C24:
	movea.l	$000C(a6),a0
	move.b	(a0),d6
	addq.l	#$01,$000C(a6)
	tst.b	d6
	bne	$0000170E

l00001C34:
	move.l	-$0220(a6),d0
	movem.l	(a7)+,d3-d7/a2-a4
	unlk	a6
	rts

;; fn00001C40: 00001C40
;;   Called from:
;;     00001CAA (in fn00001C84)
;;     00001CF2 (in fn00001CC4)
fn00001C40 proc
	link	a6,#$0000
	movea.l	$000C(a6),a1
	subq.l	#$01,$0004(a1)
	blt	$00001C60

l00001C4E:
	movea.l	(a1),a0
	move.b	$000B(a6),(a0)
	movea.l	(a1),a0
	moveq	#$00,d0
	move.b	(a0)+,d0
	ext.l	d0
	move.l	a0,(a1)
	bra	$00001C6C

l00001C60:
	move.l	a1,-(a7)
	move.l	$0008(a6),-(a7)
	jsr.l	00001E94                                             ; $022E(pc)
	addq.w	#$08,a7

l00001C6C:
	moveq	#-$01,d2
	cmp.l	d0,d2
	movea.l	$0010(a6),a0
	bne	$00001C7C

l00001C76:
	move.l	d0,(a0)
	unlk	a6
	rts

l00001C7C:
	addq.l	#$01,(a0)
	unlk	a6
	rts
00001C82       00 00                                       ..            

;; fn00001C84: 00001C84
fn00001C84 proc
	link	a6,#$0000
	movem.l	d3-d6,-(a7)
	move.l	$000C(a6),d5
	move.l	d5,d0
	subq.l	#$01,d5
	tst.l	d0
	ble	$00001CBA

l00001C98:
	move.l	$0008(a6),d3
	move.l	$0010(a6),d4
	move.l	$0014(a6),d6

l00001CA4:
	move.l	d6,-(a7)
	move.l	d4,-(a7)
	move.l	d3,-(a7)
	jsr.l	00001C40                                             ; -$006A(pc)
	lea	$000C(a7),a7
	move.l	d5,d0
	subq.l	#$01,d5
	tst.l	d0
	bgt	$00001CA4

l00001CBA:
	movem.l	(a7)+,d3-d6
	unlk	a6
	rts
00001CC2       00 00                                       ..            

;; fn00001CC4: 00001CC4
fn00001CC4 proc
	link	a6,#$0000
	movem.l	d3-d6,-(a7)
	move.l	$000C(a6),d5
	move.l	d5,d0
	subq.l	#$01,d5
	tst.l	d0
	ble	$00001D02

l00001CD8:
	move.l	$0008(a6),d3
	move.l	$0010(a6),d4
	move.l	$0014(a6),d6

l00001CE4:
	movea.l	d3,a0
	addq.l	#$01,d3
	move.l	d6,-(a7)
	move.l	d4,-(a7)
	move.b	(a0),d0
	extb.l	d0
	move.l	d0,-(a7)
	jsr.l	00001C40                                             ; -$00B2(pc)
	lea	$000C(a7),a7
	move.l	d5,d0
	subq.l	#$01,d5
	tst.l	d0
	bgt	$00001CE4

l00001D02:
	movem.l	(a7)+,d3-d6
	unlk	a6
	rts
00001D0A                               00 00                       ..    

;; fn00001D0C: 00001D0C
fn00001D0C proc
	link	a6,#$0000
	movea.l	$0008(a6),a0
	movea.l	(a0),a1
	addq.w	#$04,a1
	move.l	a1,(a0)
	move.l	-$0004(a1),d0
	unlk	a6
	rts
00001D22       00 00                                       ..            

;; fn00001D24: 00001D24
;;   Called from:
;;     00001E32 (in fn00001E04)
;;     00001E4C (in fn00001E04)
fn00001D24 proc
	link	a6,#$0000
	move.l	a2,-(a7)
	movea.l	$0008(a6),a2
	move.l	a2,d0
	bne	$00001D44

l00001D32:
	moveq	#$00,d2
	move.l	d2,-(a7)
	jsr.l	00001E04                                             ; $00CE(pc)
	addq.w	#$04,a7
	movea.l	(a7)+,a2
	unlk	a6
	rts
00001D42       4E 71                                       Nq            

l00001D44:
	move.l	a2,-(a7)
	jsr.l	00001D80                                             ; $003A(pc)
	addq.w	#$04,a7
	tst.l	d0
	beq	$00001D58

l00001D50:
	moveq	#-$01,d0
	movea.l	(a7)+,a2
	unlk	a6
	rts

l00001D58:
	btst.w	#$0006,$000E(a2)
	beq	$00001D78

l00001D60:
	move.l	$0010(a2),-(a7)
	jsr.l	$008A(a5)
	addq.w	#$04,a7
	tst.l	d0
	sne	d0
	extb.l	d0
	movea.l	(a7)+,a2
	unlk	a6
	rts
00001D76                   4E 71                               Nq        

l00001D78:
	moveq	#$00,d0
	movea.l	(a7)+,a2
	unlk	a6
	rts

;; fn00001D80: 00001D80
;;   Called from:
;;     0000169A (in fn00001680)
;;     000016C2 (in fn00001680)
;;     00001D46 (in fn00001D24)
;;     00002098 (in fn00002068)
fn00001D80 proc
	link	a6,#$0000
	movem.l	d3-d4/a2,-(a7)
	moveq	#$00,d3
	movea.l	$0008(a6),a2
	move.l	$000C(a2),d1
	move.l	d1,d0
	moveq	#$03,d4
	and.l	d4,d0
	moveq	#$02,d2
	cmp.l	d0,d2
	bne	$00001DE0

l00001D9E:
	andi.w	#$0108,d1
	beq	$00001DE0

l00001DA4:
	move.l	$0008(a2),d0
	move.l	(a2),d4
	sub.l	d0,d4
	ble	$00001DE0

l00001DAE:
	move.l	d4,-(a7)
	move.l	d0,-(a7)
	move.l	$0010(a2),-(a7)
	jsr.l	$0092(a5)
	lea	$000C(a7),a7
	cmp.l	d4,d0
	bne	$00001DD8

l00001DC2:
	move.l	$000C(a2),d0
	btst.w	#$0007,d0
	beq	$00001DE0

l00001DCC:
	bclr.l	#$01,d0
	move.l	d0,$000C(a2)
	bra	$00001DE0
00001DD6                   4E 71                               Nq        

l00001DD8:
	bset	#$0005,$000F(a2)
	moveq	#-$01,d3

l00001DE0:
	move.l	$0008(a2),(a2)
	moveq	#$00,d4
	move.l	d4,$0004(a2)
	move.l	d3,d0
	movem.l	(a7)+,d3-d4/a2
	unlk	a6
	rts

;; fn00001DF4: 00001DF4
fn00001DF4 proc
	link	a6,#$0000
	moveq	#$01,d2
	move.l	d2,-(a7)
	jsr.l	00001E04                                             ; $0008(pc)
	unlk	a6
	rts

;; fn00001E04: 00001E04
;;   Called from:
;;     00001D36 (in fn00001D24)
;;     00001DFC (in fn00001DF4)
fn00001E04 proc
	link	a6,#$0000
	movem.l	d3-d7/a2,-(a7)
	moveq	#-$01,d7
	moveq	#$01,d6
	lea	-$0A80(a5),a2
	moveq	#$00,d4
	moveq	#$00,d5
	cmpa.l	-$0800(a5),a2
	bhi	$00001E64

l00001E1E:
	move.l	$0008(a6),d3

l00001E22:
	cmp.l	d6,d3
	bne	$00001E40

l00001E26:
	move.l	$000C(a2),d0
	andi.w	#$0083,d0
	beq	$00001E40

l00001E30:
	move.l	a2,-(a7)
	jsr.l	00001D24                                             ; -$010E(pc)
	addq.w	#$04,a7
	cmp.l	d7,d0
	beq	$00001E58

l00001E3C:
	add.l	d6,d4
	bra	$00001E58

l00001E40:
	tst.l	d3
	bne	$00001E58

l00001E44:
	btst.l	d6,$000F(a2)
	beq	$00001E58

l00001E4A:
	move.l	a2,-(a7)
	jsr.l	00001D24                                             ; -$0128(pc)
	addq.w	#$04,a7
	cmp.l	d7,d0
	bne	$00001E58

l00001E56:
	move.l	d0,d5

l00001E58:
	lea	$0020(a2),a2
	cmpa.l	-$0800(a5),a2
	bls	$00001E22

l00001E62:
	bra	$00001E68

l00001E64:
	move.l	$0008(a6),d3

l00001E68:
	moveq	#$01,d7
	cmp.l	d3,d7
	beq	$00001E70

l00001E6E:
	move.l	d5,d4

l00001E70:
	move.l	d4,d0
	movem.l	(a7)+,d3-d7/a2
	unlk	a6
	rts
00001E7A                               00 00 4E 56 00 00           ..NV..
00001E80 4E BA FF 72 4A 2D FA 9C 67 04 4E BA 01 4C 4E 5E N..rJ-..g.N..LN^
00001E90 4E 75 00 00                                     Nu..            

;; fn00001E94: 00001E94
;;   Called from:
;;     00001C66 (in fn00001C40)
fn00001E94 proc
	link	a6,#$FFFC
	movem.l	d3-d5/a2-a3,-(a7)
	movea.l	$000C(a6),a3
	move.l	$0010(a3),d3
	lea	$000C(a3),a2
	move.l	(a2),d0
	move.l	d0,d1
	andi.w	#$0082,d1
	beq	$00001F5C

l00001EB4:
	btst.w	#$0006,d0
	bne	$00001F5C

l00001EBC:
	btst.w	#$0000,d0
	beq	$00001EDA

l00001EC2:
	moveq	#$00,d5
	move.l	d5,$0004(a3)
	move.l	(a2),d0
	btst.w	#$0004,d0
	beq	$00001F5C

l00001ED2:
	move.l	$0008(a3),(a3)
	bclr.b	d5,$0003(a2)

l00001EDA:
	move.l	(a2),d0
	bset	#$0001,d0
	move.l	d0,(a2)
	bclr.l	#$04,d0
	move.l	d0,(a2)
	moveq	#$00,d5
	move.l	d5,$0004(a3)
	moveq	#$00,d4
	move.l	(a2),d0
	andi.w	#$010C,d0
	bne	$00001F1C

l00001EF8:
	lea	-$0A60(a5),a0
	cmpa.l	a0,a3
	beq	$00001F08

l00001F00:
	lea	-$0A40(a5),a0
	cmpa.l	a0,a3
	bne	$00001F14

l00001F08:
	move.l	d3,-(a7)
	jsr.l	$0082(a5)
	addq.w	#$04,a7
	tst.l	d0
	bne	$00001F1C

l00001F14:
	move.l	a3,-(a7)
	jsr.l	00002014                                             ; $00FE(pc)
	addq.w	#$04,a7

l00001F1C:
	move.l	(a2),d0
	andi.w	#$0108,d0
	beq	$00001F94

l00001F24:
	move.l	$0008(a3),d0
	move.l	(a3),d5
	sub.l	d0,d5
	movea.l	d0,a0
	addq.w	#$01,a0
	move.l	a0,(a3)
	move.l	$0018(a3),d0
	subq.l	#$01,d0
	move.l	d0,$0004(a3)
	tst.l	d5
	ble	$00001F6C

l00001F40:
	move.l	d5,-(a7)
	move.l	$0008(a3),-(a7)
	move.l	d3,-(a7)
	jsr.l	$0092(a5)
	lea	$000C(a7),a7
	move.l	d0,d4
	movea.l	$0008(a3),a0
	move.b	$000B(a6),(a0)
	bra	$00001FAE

l00001F5C:
	bset	#$0005,d0
	move.l	d0,(a2)
	moveq	#-$01,d0
	movem.l	(a7)+,d3-d5/a2-a3
	unlk	a6
	rts

l00001F6C:
	lea	-$06A8(a5),a0
	btst.w	#$0005,(a0,d3)
	beq	$00001F8A

l00001F78:
	moveq	#$02,d2
	move.l	d2,-(a7)
	moveq	#$00,d1
	move.l	d1,-(a7)
	move.l	d3,-(a7)
	jsr.l	$009A(a5)
	lea	$000C(a7),a7

l00001F8A:
	movea.l	$0008(a3),a0
	move.b	$000B(a6),(a0)
	bra	$00001FAE

l00001F94:
	moveq	#$01,d5
	move.b	$000B(a6),-$0001(a6)
	move.l	d5,-(a7)
	pea	-$0001(a6)
	move.l	d3,-(a7)
	jsr.l	$0092(a5)
	lea	$000C(a7),a7
	move.l	d0,d4

l00001FAE:
	cmp.l	d5,d4
	beq	$00001FC4

l00001FB2:
	bset	#$0005,$0003(a2)
	moveq	#-$01,d0
	movem.l	(a7)+,d3-d5/a2-a3
	unlk	a6
	rts
00001FC2       4E 71                                       Nq            

l00001FC4:
	move.l	$0008(a6),d0
	andi.l	#$000000FF,d0
	movem.l	(a7)+,d3-d5/a2-a3
	unlk	a6
	rts
00001FD6                   00 00                               ..        

;; fn00001FD8: 00001FD8
fn00001FD8 proc
	link	a6,#$0000
	movem.l	d3-d6,-(a7)
	moveq	#$20,d6
	moveq	#-$01,d5
	moveq	#$00,d3
	lea	-$0A20(a5),a0
	move.l	a0,d4
	cmpa.l	-$0800(a5),a0
	bhi	$00002008

l00001FF2:
	move.l	d4,-(a7)
	jsr.l	00002068                                             ; $0074(pc)
	addq.w	#$04,a7
	cmp.l	d5,d0
	beq	$00002000

l00001FFE:
	addq.l	#$01,d3

l00002000:
	add.l	d6,d4
	cmp.l	-$0800(a5),d4
	bls	$00001FF2

l00002008:
	move.l	d3,d0
	movem.l	(a7)+,d3-d6
	unlk	a6
	rts
00002012       00 00                                       ..            

;; fn00002014: 00002014
;;   Called from:
;;     00001F16 (in fn00001E94)
fn00002014 proc
	link	a6,#$0000
	move.l	a2,-(a7)
	addq.l	#$01,-$0558(a5)
	movea.l	$0008(a6),a2
	pea	$00000200
	jsr.l	$0072(a5)
	addq.w	#$04,a7
	move.l	d0,$0008(a2)
	beq	$00002044

l00002032:
	bset	#$0003,$000F(a2)
	move.l	#$00000200,$0018(a2)
	bra	$00002058
00002042       4E 71                                       Nq            

l00002044:
	bset	#$0002,$000F(a2)
	lea	$0014(a2),a0
	move.l	a0,$0008(a2)
	moveq	#$01,d2
	move.l	d2,$0018(a2)

l00002058:
	move.l	$0008(a2),(a2)
	moveq	#$00,d2
	move.l	d2,$0004(a2)
	movea.l	(a7)+,a2
	unlk	a6
	rts

;; fn00002068: 00002068
;;   Called from:
;;     00001FF4 (in fn00001FD8)
fn00002068 proc
	link	a6,#$0000
	move.l	d3,-(a7)
	move.l	a2,-(a7)
	moveq	#-$01,d3
	movea.l	$0008(a6),a2
	move.l	$000C(a2),d0
	btst.w	#$0006,d0
	beq	$00002090

l00002080:
	moveq	#$00,d3
	move.l	d3,$000C(a2)
	moveq	#-$01,d0
	movea.l	(a7)+,a2
	move.l	(a7)+,d3
	unlk	a6
	rts

l00002090:
	andi.w	#$0083,d0
	beq	$000020E0

l00002096:
	move.l	a2,-(a7)
	jsr.l	00001D80                                             ; -$0318(pc)
	addq.w	#$04,a7
	move.l	d0,d3
	move.l	a2,-(a7)
	jsr.l	000020F0                                             ; $004E(pc)
	addq.w	#$04,a7
	move.l	$0010(a2),-(a7)
	jsr.l	$00A2(a5)
	addq.w	#$04,a7
	tst.l	d0
	bge	$000020BC

l000020B6:
	moveq	#-$01,d3
	bra	$000020E0
000020BA                               4E 71                       Nq    

l000020BC:
	move.l	$001C(a2),d0
	beq	$000020E0

l000020C2:
	move.l	d0,-(a7)
	jsr.l	$00B2(a5)
	addq.w	#$04,a7
	tst.l	d0
	beq	$000020D0

l000020CE:
	moveq	#-$01,d3

l000020D0:
	move.l	$001C(a2),-(a7)
	jsr.l	$007A(a5)
	addq.w	#$04,a7
	moveq	#$00,d2
	move.l	d2,$001C(a2)

l000020E0:
	moveq	#$00,d2
	move.l	d2,$000C(a2)
	move.l	d3,d0
	movea.l	(a7)+,a2
	move.l	(a7)+,d3
	unlk	a6
	rts

;; fn000020F0: 000020F0
;;   Called from:
;;     000020A2 (in fn00002068)
fn000020F0 proc
	link	a6,#$0000
	move.l	a2,-(a7)
	movea.l	$0008(a6),a2
	move.l	$000C(a2),d0
	move.l	d0,d1
	andi.w	#$0083,d1
	beq	$00002128

l00002106:
	btst.w	#$0003,d0
	beq	$00002128

l0000210C:
	move.l	$0008(a2),-(a7)
	jsr.l	$007A(a5)
	addq.w	#$04,a7
	bclr.b	#$03,$000F(a2)
	moveq	#$00,d2
	move.l	d2,(a2)
	move.l	d2,$0008(a2)
	move.l	d2,$0004(a2)

l00002128:
	movea.l	(a7)+,a2
	unlk	a6
	rts
0000212E                                           00 00               ..
