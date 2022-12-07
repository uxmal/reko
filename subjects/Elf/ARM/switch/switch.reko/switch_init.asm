;;; Segment .init (000082F0)

;; _init: 000082F0
;;   Called from:
;;     00008664 (in __libc_csu_init)
_init proc
	push	lr
	bl	call_gmon_start
	bl	frame_dummy
	bl	__do_global_ctors_aux
	pop	pc
