;;; Segment .init (00016EC8)

;; _init: 00016EC8
;;   Called from:
;;     000114CC (in _start)
_init proc
	save	%sp,FFFFFFA0,%sp
	call	frame_dummy
	sethi	00000000,%g0
	call	__do_global_ctors_aux
	sethi	00000000,%g0
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0
