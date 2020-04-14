;;; Segment .fini (0000874C)

;; _fini: 0000874C
;;   Called from:
;;     000086FC (in __libc_csu_fini)
_fini proc
	push	lr
	bl	$0000839C
	pop	pc
