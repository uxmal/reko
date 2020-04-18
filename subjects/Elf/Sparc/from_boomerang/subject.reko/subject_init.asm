;;; Segment .init (00010C80)

;; _init: 00010C80
;;   Called from:
;;     00010A1C (in _start)
_init proc
	save	%sp,0xFFFFFFA0_32,%sp
	call	00010C90
	sethi	0_32,%g0
	unimp

;; fn00010C90: 00010C90
;;   Called from:
;;     00010C84 (in _init)
fn00010C90 proc
	ld	[%o7+8_i32],%o0
	add	%o7,%o0,%o0
	ld	[%o0-8_i32],%l0
	subcc	%l0,%g0,%g0
	be	00010CB0
	sethi	0_32,%g0

l00010CA8:
	jmpl	%l0,%g0,%o7
	sethi	0_32,%g0

l00010CB0:
	jmpl	%i7,8_i32,%g0
	restore	%g0,%g0,%g0
