;;; Segment .init (00000588)

;; _init: 00000588
;;   Called from:
;;     000008FC (in __libc_csu_init)
_init proc
	lui	r28,+0002
	addiu	r28,r28,-00007B08
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	sw	ra,001C(sp)
	lw	r2,-7FA0(r28)
	beq	r2,r0,000005B8
	nop

l000005AC:
	lw	r25,-7FA0(r28)
	jalr	ra,r25
	nop

l000005B8:
	bal	000005C0
	nop
	lui	r28,+0002
	addiu	r28,r28,-00007B40
	addu	r28,r28,ra
	lw	r25,-7FCC(r28)
	addiu	r25,r25,+000007D4
	jalr	ra,r25
	nop
	bal	000005E4
	nop
	lui	r28,+0002
	addiu	r28,r28,-00007B64
	addu	r28,r28,ra
	lw	r25,-7FCC(r28)
	addiu	r25,r25,+00000970
	jalr	ra,r25
	nop
	lw	ra,001C(sp)
	jr	ra
	addiu	sp,sp,+00000020
