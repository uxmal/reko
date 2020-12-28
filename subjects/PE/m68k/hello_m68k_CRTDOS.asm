;;; Segment CRTDOS (0000333C)
0000333C                                     00 88 00 02             ....

;; fn00003340: 00003340
fn00003340 proc
	link	a6,#$0000
	lea	-$044C(a5),a1
	move.w	$000A(a6),d1
	move.w	d1,d0
	ext.l	d0
	move.l	d0,-$06C0(a5)
	moveq	#$00,d2
	lea	-$0504(a5),a0
	move.l	a0,d0

l0000335C:
	movea.l	d0,a0
	cmp.w	(a0),d1
	beq	$00003374

l00003362:
	addq.l	#$04,d0
	addq.l	#$01,d2
	cmp.l	a1,d0
	bcs	$0000335C

l0000336A:
	moveq	#$16,d0
	move.l	d0,-$06C4(a5)
	unlk	a6
	rts

l00003374:
	lea	-$0502(a5),a0
	moveq	#$00,d0
	move.w	(a0,d2*4),d0
	move.l	d0,-$06C4(a5)
	unlk	a6
	rts
00003386                   00 00 4E 56 FE B0 2F 0A 24 6E       ..NV../.$n
00003390 00 08 4A 12 67 00 00 8A 20 4A 43 E8 00 01 4A 18 ..J.g... JC...J.
000033A0 66 FC 91 C9 B0 FC 00 FF 62 76 41 EE FF 00 10 DA f.......bvA.....
000033B0 66 FC 48 6E FF 00 4E AD 00 BA 58 4F 70 00 41 EE f.Hn..N...XOp.A.
000033C0 FE B0 72 04 20 C0 20 C0 20 C0 20 C0 51 C9 FF F6 ..r. . . . .Q...
000033D0 41 EE FF 00 2D 48 FE C2 41 EE FE B0 A0 0C 4A 40 A...-H..A.....J@
000033E0 66 22 70 00 41 EE FE B0 72 04 20 C0 20 C0 20 C0 f"p.A...r. . . .
000033F0 20 C0 51 C9 FF F6 41 EE FF 00 2D 48 FE C2 41 EE  .Q...A...-H..A.
00003400 FE B0 A0 09 4A 40 67 10 2F 00 4E BA FF 34 58 4F ....J@g./.N..4XO
00003410 70 FF 24 5F 4E 5E 4E 75 70 00 24 5F 4E 5E 4E 75 p.$_N^Nup.$_N^Nu
00003420 74 02 2B 42 F9 3C 70 FF 24 5F 4E 5E 4E 75 00 00 t.+B.<p.$_N^Nu..
