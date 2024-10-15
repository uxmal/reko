;;; Segment .fini (00409CB0)

;; _fini: 00409CB0
_fini proc
	lui	r28,%hi(FFFFEBB0)
	addiu	r28,r28,%lo(FFFFEBB0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	sw	ra,001C(sp)
	sw	r28,0018(sp)
	lw	r25,-7FE4(r28)
	nop
	addiu	r25,r25,+00000620
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(sp)
	nop
	lw	ra,001C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000020
