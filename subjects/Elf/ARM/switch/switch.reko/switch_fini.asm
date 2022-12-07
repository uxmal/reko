;;; Segment .fini (0000874C)

;; _fini: 0000874C
;;   Called from:
;;     000086FC (in __libc_csu_fini)
_fini proc
	push	lr
	bl	__do_global_dtors_aux
	pop	pc
