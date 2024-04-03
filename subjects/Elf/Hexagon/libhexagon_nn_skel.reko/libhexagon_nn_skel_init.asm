;;; Segment .init (00009440)

;; _init: 00009440
_init proc
	{ allocframe(+00000008) }
	{ memw(r29) = r27 }
	{ memw(r29+4) = r24 }
	{ r24 = PC }
	{ r10.l = 8CF3 }
	{ r10.h = FFF7 }
	{ r24 = sub(r24,r10) }
	{ r0.h = FFFF }
	{ r0.l = F1C3 }
	{ r0 = add(r24,r0) }
	{ r1 = memw(r0) }
	{ nop; if (p0.new) jump:nt	00009480; p0 = cmp.eq(r1,00000000) }

l00009474:
	{ r1 = 00000000; r3 = 00000000; r2 = 00000000 }
	{ call	fn000094F0 }

l00009480:
	{ r27.h = 0000 }
	{ r27.l = 4280 }
	{ r27 = add(r24,r27) }

l0000948C:
	{ r27 = add(r27,FFFFFFFC) }
	{ r0 = memw(r27) }
	{ nop; if (p0.new) jump:nt	000094A4; p0 = cmp.eq(r0,00000000) }

l0000949C:
	{ callr	r0 }
	{ jump	0000948C }

l000094A4:
	{ r24 = memw(r29+4) }
	{ deallocframe; r27 = memw(r29) }
	{ jumpr	r31 }
