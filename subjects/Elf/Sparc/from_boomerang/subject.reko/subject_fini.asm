;;; Segment .fini (00010CB8)

;; _fini: 00010CB8
_fini proc
	save	%sp,FFFFFFA0,%sp
	call	fn00010CC8
	sethi	00000000,%g0
	unimp

;; fn00010CC8: 00010CC8
;;   Called from:
;;     00010CBC (in _fini)
fn00010CC8 proc
	ld	[%o7+8],%o0
	add	%o7,%o0,%o0
	ld	[%o0-4],%l0
	subcc	%l0,%g0,%g0
	be	00010CE8
	sethi	00000000,%g0

l00010CE0:
	jmpl	%l0,%g0,%o7
	sethi	00000000,%g0

l00010CE8:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0
