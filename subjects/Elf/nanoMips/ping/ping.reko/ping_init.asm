;;; Segment .init (004000D4)

;; _init: 004000D4
;;   Called from:
;;     00404970 (in __libc_start_init)
_init proc
	addiu	sp,sp,FFFFFFE0
	sw	r28,0018(sp)
	sw	ra,001C(sp)
	lw	r7,0018(sp)
	lw	r7,001C(sp)
	addiu	sp,sp,00000020
	jrc	ra
