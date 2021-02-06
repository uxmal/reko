;;; Segment .fini (00410340)

;; _fini: 00410340
;;   Called from:
;;     00404A40 (in __libc_exit_fini)
_fini proc
	addiu	sp,sp,FFFFFFE0
	sw	r28,0018(sp)
	sw	ra,001C(sp)
	lw	r7,0018(sp)
	lw	r7,001C(sp)
	addiu	sp,sp,00000020
	jrc	ra
