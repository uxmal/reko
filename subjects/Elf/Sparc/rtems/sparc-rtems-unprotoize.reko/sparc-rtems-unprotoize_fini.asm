;;; Segment .fini (00016EE4)

;; _fini: 00016EE4
_fini proc
	save	%sp,0xFFFFFFA0,%sp
	call	00011514
	sethi	0x00000000,%g0
	jmpl	%i7,8,%g0
	restore	%g0,%g0,%g0
