;;; Segment code (00000000)

;; fn00000000: 00000000
fn00000000 proc
	jsr.l	$03CC(pc)
	rts	
00000006                   4E 71 4E 71 4E 71 4E 71 4E 71       NqNqNqNqNq
00000010 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000020 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000030 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000040 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000050 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000060 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000070 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000080 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000090 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
000000A0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
000000B0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
000000C0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
000000D0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
000000E0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
000000F0 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000100 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000110 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000120 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 4E 71 NqNqNqNqNqNqNqNq
00000130 4E 71                                           Nq             

;; fn00000132: 00000132
fn00000132 proc
	link	a6,#$FFF0
	move.l	d2,-(a7)
	fmove.x	$800004FC,fp0
	fmove.x	fp0,$-000C(a6)
	clr.l	$-0010(a6)

l0000014A:
	fmove.l	$-0010(a6),fp0
	fcmp.x	$0014(a6),fp0
	fbnlt	$00000172

l0000015A:
	fmove.x	$-000C(a6),fp0
	fmul.x	$0008(a6),fp0
	fmove.x	fp0,$-000C(a6)
	addq.l	#$01,$-0010(a6)
	bra	$0000014A

l00000172:
	move.l	$-000C(a6),d0
	move.l	$-0008(a6),d1
	move.l	$-0004(a6),d2
	move.l	d2,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.x	(a7)+,fp0
	move.l	(a7)+,d2
	unlk	a6
	rts	

;; fn0000018E: 0000018E
fn0000018E proc
	link	a6,#$FFF0
	move.l	d2,-(a7)
	fmove.x	$80000508,fp0
	fmove.x	fp0,$-000C(a6)
	moveq	#$+01,d0
	move.l	d0,$-0010(a6)

l000001A8:
	fmove.l	$-0010(a6),fp0
	fcmp.x	$0008(a6),fp0
	fbnle	$000001D4

l000001B8:
	fmove.l	$-0010(a6),fp0
	fmove.x	$-000C(a6),fp1
	fmul.x	fp0,fp1
	fmove.x	fp1,$-000C(a6)
	addq.l	#$01,$-0010(a6)
	bra	$000001A8

l000001D4:
	nop	
	move.l	$-000C(a6),d0
	move.l	$-0008(a6),d1
	move.l	$-0004(a6),d2
	move.l	d2,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.x	(a7)+,fp0
	move.l	(a7)+,d2
	unlk	a6
	rts	

;; fn000001F2: 000001F2
fn000001F2 proc
	link	a6,#$FFE4
	fmovem.x	fp2,-(a7)
	move.l	d2,-(a7)
	fmove.x	$0008(a6),fp0
	fmove.x	fp0,$-000C(a6)
	fmove.x	$80000514,fp1
	fmove.x	fp1,$-0018(a6)
	moveq	#$+03,d0
	move.l	d0,$-001C(a6)

l0000021C:
	moveq	#$+64,d0
	cmp.l	$-001C(a6),d0
	blt	$0000028A

l00000224:
	fmove.l	$-001C(a6),fp0
	fmove.x	fp0,-(a7)
	fmove.x	$0008(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	$-0106(pc)
	lea	$0018(a7),a7
	fmove.x	fp0,fp2
	fmove.l	$-001C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	$-00C0(pc)
	lea	$000C(a7),a7
	fmove.x	fp2,fp1
	fdiv.x	fp0,fp1
	fmove.x	fp1,fp0
	fmul.x	$-0018(a6),fp0
	fmove.x	$-000C(a6),fp1
	fadd.x	fp0,fp1
	fmove.x	fp1,$-000C(a6)
	fneg.x	$-0018(a6),fp0
	fmove.x	fp0,$-0018(a6)
	addq.l	#$02,$-001C(a6)
	bra	$0000021C

l0000028A:
	move.l	$-000C(a6),d0
	move.l	$-0008(a6),d1
	move.l	$-0004(a6),d2
	move.l	d2,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.x	(a7)+,fp0
	move.l	$-002C(a6),d2
	fmovem.x	$-0028(a6),fp2
	unlk	a6
	rts	

;; fn000002AE: 000002AE
fn000002AE proc
	link	a6,#$FFE4
	fmovem.x	fp2,-(a7)
	move.l	d2,-(a7)
	fmove.x	$80000520,fp0
	fmove.x	fp0,$-000C(a6)
	fmove.x	$8000052C,fp1
	fmove.x	fp1,$-0018(a6)
	moveq	#$+02,d0
	move.l	d0,$-001C(a6)

l000002DA:
	moveq	#$+64,d0
	cmp.l	$-001C(a6),d0
	blt	$00000348

l000002E2:
	fmove.l	$-001C(a6),fp0
	fmove.x	fp0,-(a7)
	fmove.x	$0008(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	$-01C4(pc)
	lea	$0018(a7),a7
	fmove.x	fp0,fp2
	fmove.l	$-001C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	$-017E(pc)
	lea	$000C(a7),a7
	fmove.x	fp2,fp1
	fdiv.x	fp0,fp1
	fmove.x	fp1,fp0
	fmul.x	$-0018(a6),fp0
	fmove.x	$-000C(a6),fp1
	fadd.x	fp0,fp1
	fmove.x	fp1,$-000C(a6)
	fneg.x	$-0018(a6),fp0
	fmove.x	fp0,$-0018(a6)
	addq.l	#$02,$-001C(a6)
	bra	$000002DA

l00000348:
	move.l	$-000C(a6),d0
	move.l	$-0008(a6),d1
	move.l	$-0004(a6),d2
	move.l	d2,-(a7)
	move.l	d1,-(a7)
	move.l	d0,-(a7)
	fmove.x	(a7)+,fp0
	move.l	$-002C(a6),d2
	fmovem.x	$-0028(a6),fp2
	unlk	a6
	rts	

;; fn0000036C: 0000036C
fn0000036C proc
	link	a6,#$0000
	fmovem.x	fp2,-(a7)
	move.l	d2,-(a7)
	fmove.x	$0008(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	$-018E(pc)
	lea	$000C(a7),a7
	fmove.x	fp0,fp2
	fmove.x	$0008(a6),fp1
	fmove.x	fp1,-(a7)
	jsr.l	$-00E8(pc)
	lea	$000C(a7),a7
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
	move.l	$-0010(a6),d2
	fmovem.x	$-000C(a6),fp2
	unlk	a6
	rts	

;; fn000003CC: 000003CC
fn000003CC proc
	link	a6,#$FFF4
	fmove.x	$80000538,fp0
	fmove.x	fp0,$-000C(a6)
	fmove.x	$-000C(a6),fp0
	fmove.x	fp0,-(a7)
	fmove.x	$-000C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	$-02C0(pc)
	lea	$0018(a7),a7
	fmove.x	$-000C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	$-0276(pc)
	lea	$000C(a7),a7
	fmove.x	$-000C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	$-0224(pc)
	lea	$000C(a7),a7
	fmove.x	$-000C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	$-017A(pc)
	lea	$000C(a7),a7
	fmove.x	$-000C(a6),fp0
	fmove.x	fp0,-(a7)
	jsr.l	$-00CE(pc)
	lea	$000C(a7),a7
	clr.l	d0
	unlk	a6
	rts	
