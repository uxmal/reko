;;; Segment .init (00016EC8)

;; _init: 00016EC8
;;   Called from:
;;     000114CC (in _start)
_init proc
	save	%sp,0xFFFFFFA0_32,%sp
	call	000115D8
	sethi	0_32,%g0
	call	00016E6C
	sethi	0_32,%g0
	jmpl	%i7,8_i32,%g0
	restore	%g0,%g0,%g0
