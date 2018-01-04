;;; Segment code (80000000)

;; fn80000000: 80000000
fn80000000 proc
	jsr.l	+$03CC(pc)                                           ; 800003CC
	rts	
80000006                   4E 71 4E 71 4E 71 4E 71 4E 71       NqNqNqNqNq
80000010 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000020 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000030 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000040 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000050 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000060 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000070 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000080 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000090 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
800000A0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
800000B0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
800000C0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
800000D0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
800000E0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
800000F0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000100 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000110 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000120 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
80000130 4E 71                                           Nq             

;; fn80000132: 80000132
fn80000132 proc
	link	a6,#$FFF0
	move.l	d2,-(a7)
	fmove.x	$800004FC,fp0
	fmove.x	fp0,-$000C(a6)
	clr.l	-$0010(a6)

l8000014A:
	fmove.l	-$0010(a6),fp0
	fcmp.x	+$0014(a6),fp0
	fbnlt	$80000172

l8000015A:
	fmove.x	-$000C(a6),fp0
	fmul.x	+$0008(a6),fp0
	fmove.x	fp0,-$000C(a6)
	addq.l	#$01,-$0010(a6)
	bra	$8000014A

l80000172:
	move.l	-$000C(a6),d0
	move.l	-$0008(a6),d1
	move.l	-$0004(a6),d2
	move.l	d2,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.x	(a7)+,fp0
	move.l	(a7)+,d2
	unlk	a6
	rts	

;; fn8000018E: 8000018E
fn8000018E proc
	link	a6,#$FFF0
	move.l	d2,-(a7)
	fmove.x	$80000508,fp0
	fmove.x	fp0,-$000C(a6)
	moveq	#$01,d0
	move.l	d0,-$0010(a6)

l800001A8:
	fmove.l	-$0010(a6),fp0
	fcmp.x	+$0008(a6),fp0
	fbnle	$800001D4

l800001B8:
	fmove.l	-$0010(a6),fp0
	fmove.x	-$000C(a6),fp1
	fmul.x	fp0,fp1
	fmove.x	fp1,-$000C(a6)
	addq.l	#$01,-$0010(a6)
	bra	$800001A8

l800001D4:
	nop	
	move.l	-$000C(a6),d0
	move.l	-$0008(a6),d1
	move.l	-$0004(a6),d2
	move.l	d2,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.x	(a7)+,fp0
	move.l	(a7)+,d2
	unlk	a6
	rts	

;; fn800001F2: 800001F2
fn800001F2 proc
	link	a6,#$FFE4
	fmovem.x	fp2,-(a7)
	move.l	d2,-(a7)
	fmove.x	+$0008(a6),fp0
	fmove.x	fp0,-$000C(a6)
	fmove.x	$80000514,fp1
	fmove.x	fp1,-$0018(a6)
	moveq	#$03,d0
	move.l	d0,-$001C(a6)

l8000021C:
	moveq	#$64,d0
	cmp.l	-$001C(a6),d0
	blt	$8000028A

l80000224:
	fmove.l	-$001C(a6),fp0
	fmove.x	fp0,-(a7)
	fmove.x	+$0008(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	-$0106(pc)                                           ; 80000132
	lea	+$0018(a7),a7
	fmove.x	fp0,fp2
	fmove.l	-$001C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	-$00C0(pc)                                           ; 8000018E
	lea	+$000C(a7),a7
	fmove.x	fp2,fp1
	fdiv.x	fp0,fp1
	fmove.x	fp1,fp0
	fmul.x	-$0018(a6),fp0
	fmove.x	-$000C(a6),fp1
	fadd.x	fp0,fp1
	fmove.x	fp1,-$000C(a6)
	fneg.x	-$0018(a6),fp0
	fmove.x	fp0,-$0018(a6)
	addq.l	#$02,-$001C(a6)
	bra	$8000021C

l8000028A:
	move.l	-$000C(a6),d0
	move.l	-$0008(a6),d1
	move.l	-$0004(a6),d2
	move.l	d2,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.x	(a7)+,fp0
	move.l	-$002C(a6),d2
	fmovem.x	-$0028(a6),fp2
	unlk	a6
	rts	

;; fn800002AE: 800002AE
fn800002AE proc
	link	a6,#$FFE4
	fmovem.x	fp2,-(a7)
	move.l	d2,-(a7)
	fmove.x	$80000520,fp0
	fmove.x	fp0,-$000C(a6)
	fmove.x	$8000052C,fp1
	fmove.x	fp1,-$0018(a6)
	moveq	#$02,d0
	move.l	d0,-$001C(a6)

l800002DA:
	moveq	#$64,d0
	cmp.l	-$001C(a6),d0
	blt	$80000348

l800002E2:
	fmove.l	-$001C(a6),fp0
	fmove.x	fp0,-(a7)
	fmove.x	+$0008(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	-$01C4(pc)                                           ; 80000132
	lea	+$0018(a7),a7
	fmove.x	fp0,fp2
	fmove.l	-$001C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	-$017E(pc)                                           ; 8000018E
	lea	+$000C(a7),a7
	fmove.x	fp2,fp1
	fdiv.x	fp0,fp1
	fmove.x	fp1,fp0
	fmul.x	-$0018(a6),fp0
	fmove.x	-$000C(a6),fp1
	fadd.x	fp0,fp1
	fmove.x	fp1,-$000C(a6)
	fneg.x	-$0018(a6),fp0
	fmove.x	fp0,-$0018(a6)
	addq.l	#$02,-$001C(a6)
	bra	$800002DA

l80000348:
	move.l	-$000C(a6),d0
	move.l	-$0008(a6),d1
	move.l	-$0004(a6),d2
	move.l	d2,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.x	(a7)+,fp0
	move.l	-$002C(a6),d2
	fmovem.x	-$0028(a6),fp2
	unlk	a6
	rts	

;; fn8000036C: 8000036C
fn8000036C proc
	link	a6,#$0000
	fmovem.x	fp2,-(a7)
	move.l	d2,-(a7)
	fmove.x	+$0008(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	-$018E(pc)                                           ; 800001F2
	lea	+$000C(a7),a7
	fmove.x	fp0,fp2
	fmove.x	+$0008(a6),fp1
	fmove.x	fp1,-(a7)
	jsr.l	-$00E8(pc)                                           ; 800002AE
	lea	+$000C(a7),a7
	fmove.x	fp2,fp1
	fdiv.x	fp0,fp1
	fmove.x	fp1,fp0
	fmove.x	fp0,-(a7)
	move.l	(a7)+,d0
	move.l	(a7)+,d1
	move.l	(a7)+,d2
	move.l	d2,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.x	(a7)+,fp0
	move.l	-$0010(a6),d2
	fmovem.x	-$000C(a6),fp2
	unlk	a6
	rts	

;; fn800003CC: 800003CC
fn800003CC proc
	link	a6,#$FFF4
	fmove.x	$80000538,fp0
	fmove.x	fp0,-$000C(a6)
	fmove.x	-$000C(a6),fp0
	fmove.x	fp0,-(a7)
	fmove.x	-$000C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	-$02C0(pc)                                           ; 80000132
	lea	+$0018(a7),a7
	fmove.x	-$000C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	-$0276(pc)                                           ; 8000018E
	lea	+$000C(a7),a7
	fmove.x	-$000C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	-$0224(pc)                                           ; 800001F2
	lea	+$000C(a7),a7
	fmove.x	-$000C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	-$017A(pc)                                           ; 800002AE
	lea	+$000C(a7),a7
	fmove.x	-$000C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	-$00CE(pc)                                           ; 8000036C
	lea	+$000C(a7),a7
	clr.l	d0
	unlk	a6
	rts	
