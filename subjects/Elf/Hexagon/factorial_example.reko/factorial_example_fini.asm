;;; Segment .fini (0000B360)

;; _fini: 0000B360
_fini proc
	{ allocframe(+00000008) }
	{ memw(r29) = r27 }
	{ r27.h = 0000 }
	{ r27.l = 8033 }

l0000B370:
	{ r27 = add(r27,00000004) }
	{ r0 = memw(r27) }
	{ if (p0.new) jump:nt 0000B384; p0 = cmp.eq(r0,00000000) }

l0000B37C:
	{ callr r0 }
	{ jump 0000B370 }

l0000B384:
	{ deallocframe; r27 = memw(r29) }
	{ jumpr r31 }
