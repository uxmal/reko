;;; Segment .init (004001BC)

;; _init: 004001BC
_init proc
	lui	r28,%hi(FFFF86A4)
	addiu	r28,r28,%lo(FFFF86A4)
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	sw	ra,001C(sp)
	sw	r28,0018(sp)
	lw	r25,-7FE4(r28)
	nop
	addiu	r25,r25,+0000072C
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(sp)
	nop
	lw	r25,-7FE0(r28)
	nop
	addiu	r25,r25,-000063F0
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(sp)
	nop
	lw	ra,001C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000020
