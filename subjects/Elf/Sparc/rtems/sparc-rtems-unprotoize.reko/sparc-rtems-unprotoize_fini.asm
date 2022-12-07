;;; Segment .fini (00016EE4)

;; _fini: 00016EE4
_fini proc
	save	%sp,FFFFFFA0,%sp
	call	__do_global_dtors_aux
	sethi	00000000,%g0
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0
