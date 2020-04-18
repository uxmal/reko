;;; Segment .fini (00016EE4)

;; _fini: 00016EE4
_fini proc
	save	%sp,0xFFFFFFA0_32,%sp
	call	00011514
	sethi	0_32,%g0
	jmpl	%i7,8_i32,%g0
	restore	%g0,%g0,%g0
