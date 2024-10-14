;;; Segment .text (80000080)

;; deregister_tm_clones: 80000080
;;   Called from:
;;     8000012C (in __do_global_dtors_aux)
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
	pea	__TMC_END__
	jsr.l	(a0)
	addq.l	#$04,a7

l800000AA:
	unlk	a6
	rts

;; register_tm_clones: 800000AE
;;   Called from:
;;     8000018A (in frame_dummy)
;;     800001A0 (in frame_dummy)
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
	pea	__TMC_END__
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
	tst.b	__TMC_END__
	bne	$8000014C

l800000F4:
	lea	__DTOR_LIST__,a2
	move.l	dtor_idx.3228,d0
	move.l	#$80002718,d2
	subi.l	#$80002714,d2
	asr.l	#$02,d2
	subq.l	#$01,d2
	cmp.l	d0,d2
	bls	$8000012C

l80000114:
	addq.l	#$01,d0
	move.l	d0,dtor_idx.3228
	movea.l	(a2,d0*4),a0
	jsr.l	(a0)
	move.l	dtor_idx.3228,d0
	cmp.l	d0,d2
	bhi	$80000114

l8000012C:
	jsr.l	deregister_tm_clones                                 ; -$00AC(pc)
	lea	$00000000,a0
	tst.l	a0
	beq	$80000144

l8000013A:
	pea	__EH_FRAME_BEGIN__
	jsr.l	(a0)
	addq.l	#$04,a7

l80000144:
	move.b	#$01,__TMC_END__

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
	pea	object.3241
	pea	__EH_FRAME_BEGIN__
	jsr.l	(a0)
	addq.l	#$08,a7

l8000017E:
	lea	__JCR_END__,a0
	tst.l	(a0)
	bne	$8000018E

l80000188:
	unlk	a6
	bra	register_tm_clones

l8000018E:
	lea	$00000000,a1
	tst.l	a1
	beq	$80000188

l80000198:
	move.l	a0,-(a7)
	jsr.l	(a1)
	addq.l	#$04,a7
	unlk	a6
	bra	register_tm_clones

;; call_frame_dummy: 800001A4
call_frame_dummy proc
	link	a6,#$0000
	unlk	a6
	rts

;; sine_taylor: 800001AC
;;   Called from:
;;     800004AA (in main)
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
;;   Called from:
;;     800003FC (in sine_taylor)
;;     80000454 (in sine_taylor)
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
;;   Called from:
;;     800003EC (in sine_taylor)
;;     80000444 (in sine_taylor)
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
	jsr.l	pow_int                                              ; -$007A(pc)
	lea	$000C(a7),a7
	fmove.x	fp0,fp2
	move.l	-$0004(a6),-(a7)
	jsr.l	factorial                                            ; -$00C0(pc)
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
	jsr.l	pow_int                                              ; -$00D2(pc)
	lea	$000C(a7),a7
	fmove.x	fp0,fp2
	move.l	-$0004(a6),-(a7)
	jsr.l	factorial                                            ; -$0118(pc)
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
	jsr.l	sine_taylor                                          ; -$02FE(pc)
	addq.l	#$08,a7
	move.l	a6,d0
	subq.l	#$04,d0
	move.l	d0,-(a7)
	move.l	#$BC6A7EFA,-(a7)
	move.l	#$3F689374,-(a7)
	move.l	#$51EB851F,-(a7)
	move.l	#$40091EB8,-(a7)
	jsr.l	_sin
	lea	$0014(a7),a7
	clr.l	d0
	unlk	a6
	rts

;; _sin: 800004DE
;;   Called from:
;;     800004CE (in main)
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
	movea.l	__CTOR_LIST__,a0
	lea	__CTOR_LIST__,a2
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
