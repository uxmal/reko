;;; Segment .init (000082F0)

;; _init: 000082F0
;;   Called from:
;;     00008664 (in __libc_csu_init)
_init proc
	push	lr
	bl	$0000836C
	bl	$00008404
	bl	$0000870C
	pop	pc
