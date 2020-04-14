;;; Segment .init (00016EC8)

;; _init: 00016EC8
;;   Called from:
;;     000114CC (in _start)
_init proc
	save	%sp,0xFFFFFFA0,%sp
	call	000115D8
	sethi	0x00000000,%g0
	call	00016E6C
	sethi	0x00000000,%g0
	jmpl	%i7,8,%g0
	restore	%g0,%g0,%g0
