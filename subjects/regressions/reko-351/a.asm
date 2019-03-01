;;; Segment .init (80000074)
80000074             4E B9 80 00 01 60 4E B9 80 00 06 24     N....`N....$
;;; Segment .text (80000080)

;; deregister_tm_clones: 80000080
deregister_tm_clones proc
	link	a6,#$0000
	move.l	#$80002724,d0
	subi.l	#$80002721,d0
	moveq	#$06,d1
	cmp.l	d0,d1
	bcc	$800000AA

l80000096:
	lea	$00000000,a0
	tst.l	a0
	beq	$800000AA

l800000A0:
	pea	$80002724
	jsr.l	(a0)
	addq.l	#$04,a7

l800000AA:
	unlk	a6
	rts

;; register_tm_clones: 800000AE
register_tm_clones proc
	link	a6,#$0000
	move.l	#$80002724,d0
	subi.l	#$80002724,d0
	asr.l	#$02,d0
	move.l	d0,d0
	bpl	$800000C6

l800000C4:
	addq.l	#$01,d0

l800000C6:
	asr.l	#$01,d0
	beq	$800000E0

l800000CA:
	lea	$00000000,a0
	tst.l	a0
	beq	$800000E0

l800000D4:
	move.l	d0,-(a7)
	pea	$80002724
	jsr.l	(a0)
	addq.l	#$08,a7

l800000E0:
	unlk	a6
	rts

;; __do_global_dtors_aux: 800000E4
__do_global_dtors_aux proc
	link	a6,#$0000
	move.l	a2,-(a7)
	move.l	d2,-(a7)
	tst.b	$80002724
	bne	$8000014C

l800000F4:
	lea	$80002714,a2
	move.l	$80002726,d0
	move.l	#$80002718,d2
	subi.l	#$80002714,d2
	asr.l	#$02,d2
	subq.l	#$01,d2
	cmp.l	d0,d2
	bls	$8000012C

l80000114:
	addq.l	#$01,d0
	move.l	d0,$80002726
	movea.l	(a2,d0*4),a0
	jsr.l	(a0)
	move.l	$80002726,d0
	cmp.l	d0,d2
	bhi	$80000114

l8000012C:
	jsr.l	-$00AC(pc)                                           ; 80000080
	lea	$00000000,a0
	tst.l	a0
	beq	$80000144

l8000013A:
	pea	$8000065C
	jsr.l	(a0)
	addq.l	#$04,a7

l80000144:
	move.b	#$01,$80002724

l8000014C:
	move.l	-$0008(a6),d2
	movea.l	-$0004(a6),a2
	unlk	a6
	rts

;; call___do_global_dtors_aux: 80000158
call___do_global_dtors_aux proc
	link	a6,#$0000
	unlk	a6
	rts

;; frame_dummy: 80000160
frame_dummy proc
	link	a6,#$0000
	lea	$00000000,a0
	tst.l	a0
	beq	$8000017E

l8000016E:
	pea	$8000272A
	pea	$8000065C
	jsr.l	(a0)
	addq.l	#$08,a7

l8000017E:
	lea	$8000271C,a0
	tst.l	(a0)
	bne	$8000018E

l80000188:
	unlk	a6
	bra	$800000AE

l8000018E:
	lea	$00000000,a1
	tst.l	a1
	beq	$80000188

l80000198:
	move.l	a0,-(a7)
	jsr.l	(a1)
	addq.l	#$04,a7
	unlk	a6
	bra	$800000AE

;; call_frame_dummy: 800001A4
call_frame_dummy proc
	link	a6,#$0000
	unlk	a6
	rts

;; sine_taylor: 800001AC
sine_taylor proc
	link	a6,#$FFB0
	fmove.d	$0008(a6),fp0
	fmul.d	$0008(a6),fp0
	fmove.d	fp0,-$0008(a6)
	fmove.d	-$0008(a6),fp1
	fmul.d	-$0008(a6),fp1
	fmove.d	fp1,-$0010(a6)
	fmove.d	-$0008(a6),fp0
	fdiv.d	#6.0,fp0
	fmovecr	#$32,fp1
	fsub.x	fp0,fp1
	fmove.x	fp1,fp0
	fmove.x	fp0,fp1
	fmul.d	$0008(a6),fp1
	fmove.d	fp1,-$0018(a6)
	fmove.d	$0008(a6),fp0
	fmul.d	-$0010(a6),fp0
	fmove.d	fp0,-$0020(a6)
	fmove.d	-$0008(a6),fp0
	fdiv.d	#42.0,fp0
	fmovecr	#$32,fp1
	fsub.x	fp0,fp1
	fmove.x	fp1,fp0
	fmul.d	-$0020(a6),fp0
	fmove.x	fp0,fp1
	fdiv.d	#120.0,fp1
	fmove.d	fp1,-$0028(a6)
	fmove.d	-$0020(a6),fp0
	fmul.d	-$0010(a6),fp0
	fmove.d	fp0,-$0030(a6)
	fmove.d	-$0008(a6),fp0
	fdiv.d	#110.0,fp0
	fmovecr	#$32,fp1
	fsub.x	fp0,fp1
	fmove.x	fp1,fp0
	fmul.d	-$0030(a6),fp0
	fmove.x	fp0,fp1
	fdiv.d	#362880.0,fp1
	fmove.d	fp1,-$0038(a6)
	fmove.d	-$0030(a6),fp0
	fmul.d	-$0010(a6),fp0
	fmove.d	fp0,-$0040(a6)
	fmove.d	-$0008(a6),fp0
	fdiv.d	#210.0,fp0
	fmovecr	#$32,fp1
	fsub.x	fp0,fp1
	fmove.x	fp1,fp0
	fmul.d	-$0040(a6),fp0
	fmove.x	fp0,fp1
	fdiv.d	#6227020800.0,fp1
	fmove.d	fp1,-$0048(a6)
	move.l	-$0048(a6),-$0050(a6)
	move.l	-$0044(a6),-$004C(a6)
	fmove.d	-$0050(a6),fp0
	fadd.d	-$0038(a6),fp0
	fmove.d	fp0,-$0050(a6)
	fmove.d	-$0050(a6),fp1
	fadd.d	-$0028(a6),fp1
	fmove.d	fp1,-$0050(a6)
	fmove.d	-$0050(a6),fp0
	fadd.d	-$0018(a6),fp0
	fmove.d	fp0,-$0050(a6)
	move.l	-$0050(a6),d0
	move.l	-$004C(a6),d1
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.d	(a7)+,fp0
	unlk	a6
	rts

;; factorial: 8000033C
factorial proc
	link	a6,#$FFF8
	moveq	#$01,d0
	move.l	d0,-$0008(a6)
	moveq	#$02,d0
	move.l	d0,-$0004(a6)

l8000034C:
	move.l	-$0004(a6),d0
	cmp.l	$0008(a6),d0
	bgt	$8000036A

l80000356:
	move.l	-$0008(a6),d0
	muls.l	-$0004(a6),d0
	move.l	d0,-$0008(a6)
	addq.l	#$01,-$0004(a6)
	bra	$8000034C

l8000036A:
	move.l	-$0008(a6),d0
	unlk	a6
	rts

;; pow_int: 80000372
pow_int proc
	link	a6,#$FFF4
	move.l	#$3FF00000,-$000C(a6)
	clr.l	-$0008(a6)
	clr.l	-$0004(a6)

l80000386:
	move.l	-$0004(a6),d0
	cmp.l	$0010(a6),d0
	bge	$800003A8

l80000390:
	fmove.d	-$000C(a6),fp0
	fmul.d	$0008(a6),fp0
	fmove.d	fp0,-$000C(a6)
	addq.l	#$01,-$0004(a6)
	bra	$80000386

l800003A8:
	move.l	-$000C(a6),d0
	move.l	-$0008(a6),d1
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.d	(a7)+,fp0
	unlk	a6
	rts

;; sine_taylor: 800003BC
sine_taylor proc
	link	a6,#$FFF4
	fmovem.x	fp2,-(a7)
	move.l	$0008(a6),-$000C(a6)
	move.l	$000C(a6),-$0008(a6)
	moveq	#$03,d0
	move.l	d0,-$0004(a6)

l800003D6:
	move.l	-$0004(a6),d0
	cmp.l	$0010(a6),d0
	bgt	$80000428

l800003E0:
	move.l	-$0004(a6),-(a7)
	move.l	$000C(a6),-(a7)
	move.l	$0008(a6),-(a7)
	jsr.l	-$007A(pc)                                           ; 80000372
	lea	$000C(a7),a7
	fmove.x	fp0,fp2
	move.l	-$0004(a6),-(a7)
	jsr.l	-$00C0(pc)                                           ; 8000033C
	addq.l	#$04,a7
	fmove.l	d0,fp0
	fmove.x	fp2,fp1
	fdiv.x	fp0,fp1
	fmove.x	fp1,fp0
	fmove.d	-$000C(a6),fp1
	fadd.x	fp0,fp1
	fmove.d	fp1,-$000C(a6)
	addq.l	#$04,-$0004(a6)
	bra	$800003D6

l80000428:
	moveq	#$05,d0
	move.l	d0,-$0004(a6)

l8000042E:
	move.l	-$0004(a6),d0
	cmp.l	$0010(a6),d0
	bgt	$80000480

l80000438:
	move.l	-$0004(a6),-(a7)
	move.l	$000C(a6),-(a7)
	move.l	$0008(a6),-(a7)
	jsr.l	-$00D2(pc)                                           ; 80000372
	lea	$000C(a7),a7
	fmove.x	fp0,fp2
	move.l	-$0004(a6),-(a7)
	jsr.l	-$0118(pc)                                           ; 8000033C
	addq.l	#$04,a7
	fmove.l	d0,fp0
	fmove.x	fp2,fp1
	fdiv.x	fp0,fp1
	fmove.x	fp1,fp0
	fmove.d	-$000C(a6),fp1
	fsub.x	fp0,fp1
	fmove.d	fp1,-$000C(a6)
	addq.l	#$04,-$0004(a6)
	bra	$8000042E

l80000480:
	move.l	-$000C(a6),d0
	move.l	-$0008(a6),d1
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.d	(a7)+,fp0
	fmovem.x	-$0018(a6),fp2
	unlk	a6
	rts

;; main: 8000049A
main proc
	link	a6,#$FFFC
	move.l	#$51EB851F,-(a7)
	move.l	#$40091EB8,-(a7)
	jsr.l	-$02FE(pc)                                           ; 800001AC
	addq.l	#$08,a7
	move.l	a6,d0
	subq.l	#$04,d0
	move.l	d0,-(a7)
	move.l	#$BC6A7EFA,-(a7)
	move.l	#$3F689374,-(a7)
	move.l	#$51EB851F,-(a7)
	move.l	#$40091EB8,-(a7)
	jsr.l	$800004DE
	lea	$0014(a7),a7
	clr.l	d0
	unlk	a6
	rts

;; _sin: 800004DE
_sin proc
	link	a6,#$FFDC
	move.l	$0008(a6),-$0008(a6)
	move.l	$000C(a6),-$0004(a6)
	fmove.d	-$0008(a6),fp0
	fmul.d	-$0008(a6),fp0
	fmove.d	fp0,-$0024(a6)
	move.l	#$3FF00000,-$0010(a6)
	clr.l	-$000C(a6)
	clr.l	-$0018(a6)
	clr.l	-$0014(a6)
	moveq	#$01,d0
	move.l	d0,-$001C(a6)

l8000051A:
	fmove.d	-$0008(a6),fp0
	fdiv.d	-$0010(a6),fp0
	fcmp.d	$0010(a6),fp0
	fbnge	$8000060E

l80000530:
	fmove.d	-$0008(a6),fp0
	fdiv.d	-$0010(a6),fp0
	fmove.d	-$0018(a6),fp1
	fadd.x	fp0,fp1
	fmove.d	fp1,-$0018(a6)
	fmove.d	-$0008(a6),fp0
	fmul.d	-$0024(a6),fp0
	fmove.d	fp0,-$0008(a6)
	addq.l	#$01,-$001C(a6)
	move.l	-$001C(a6),d0
	fmove.l	d0,fp0
	fmove.d	-$0010(a6),fp1
	fmul.x	fp0,fp1
	fmove.d	fp1,-$0010(a6)
	addq.l	#$01,-$001C(a6)
	move.l	-$001C(a6),d0
	fmove.l	d0,fp0
	fmove.d	-$0010(a6),fp1
	fmul.x	fp0,fp1
	fmove.d	fp1,-$0010(a6)
	fmove.d	-$0008(a6),fp0
	fdiv.d	-$0010(a6),fp0
	fmove.d	-$0018(a6),fp1
	fsub.x	fp0,fp1
	fmove.d	fp1,-$0018(a6)
	fmove.d	-$0008(a6),fp0
	fmul.d	-$0024(a6),fp0
	fmove.d	fp0,-$0008(a6)
	addq.l	#$01,-$001C(a6)
	move.l	-$001C(a6),d0
	fmove.l	d0,fp0
	fmove.d	-$0010(a6),fp1
	fmul.x	fp0,fp1
	fmove.d	fp1,-$0010(a6)
	addq.l	#$01,-$001C(a6)
	move.l	-$001C(a6),d0
	fmove.l	d0,fp0
	fmove.d	-$0010(a6),fp1
	fmul.x	fp0,fp1
	fmove.d	fp1,-$0010(a6)
	movea.l	$0018(a6),a0
	move.l	(a0),d0
	addq.l	#$01,d0
	movea.l	$0018(a6),a0
	move.l	d0,(a0)
	bra	$8000051A

l8000060E:
	move.l	-$0018(a6),d0
	move.l	-$0014(a6),d1
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.d	(a7)+,fp0
	unlk	a6
	rts
80000622       4E 71                                       Nq           

;; __do_global_ctors_aux: 80000624
__do_global_ctors_aux proc
	link	a6,#$0000
	move.l	a2,-(a7)
	movea.l	$8000270C,a0
	lea	$8000270C,a2
	moveq	#-$01,d0
	cmp.l	a0,d0
	beq	$80000646

l8000063C:
	jsr.l	(a0)
	movea.l	-(a2),a0
	moveq	#-$01,d0
	cmp.l	a0,d0
	bne	$8000063C

l80000646:
	movea.l	-$0004(a6),a2
	unlk	a6
	rts

;; call___do_global_ctors_aux: 8000064E
call___do_global_ctors_aux proc
	link	a6,#$0000
	unlk	a6
	rts
;;; Segment .fini (80000656)
80000656                   4E B9 80 00 00 E4                   N.....   
;;; Segment .eh_frame (8000065C)
8000065C                                     00 00 00 14             ....
80000660 00 00 00 00 01 7A 52 00 02 7C 18 01 1B 0C 0F 04 .....zR..|......
80000670 98 01 00 00 00 00 00 14 00 00 00 1C FF FF FB 30 ...............0
80000680 00 00 01 90 00 42 8E 02 0C 0E 08 00 00 00 00 14 .....B..........
80000690 00 00 00 34 FF FF FC A8 00 00 00 36 00 42 8E 02 ...4.......6.B..
800006A0 0C 0E 08 00 00 00 00 14 00 00 00 4C FF FF FC C6 ...........L....
800006B0 00 00 00 4A 00 42 8E 02 0C 0E 08 00 00 00 00 14 ...J.B..........
800006C0 00 00 00 64 FF FF FE 1A 00 00 01 44 00 42 8E 02 ...d.......D.B..
800006D0 0C 0E 08 00 00 00 00 18 00 00 00 7C FF FF FC E0 ...........|....
800006E0 00 00 00 DE 00 42 8E 02 0C 0E 08 42 92 08 00 00 .....B.....B....
800006F0 00 00 00 14 00 00 00 98 FF FF FD A2 00 00 00 44 ...............D
80000700 00 42 8E 02 0C 0E 08 00 00 00 00 00             .B..........   
;;; Segment .ctors (8000270C)
8000270C                                     FF FF FF FF             ....
80002710 00 00 00 00                                     ....           
;;; Segment .dtors (80002714)
80002714             FF FF FF FF 00 00 00 00                 ........   
;;; Segment .jcr (8000271C)
8000271C                                     00 00 00 00             ....
;;; Segment .data (80002720)
80002720 00 00 00 00                                     ....           
;;; Segment .bss (80002724)
80002724             00 00                                   ..         
dtor_idx.3228		; 80002726
	dd	0x00000000
object.3241		; 8000272A
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00
80002742       00 00                                       ..           
