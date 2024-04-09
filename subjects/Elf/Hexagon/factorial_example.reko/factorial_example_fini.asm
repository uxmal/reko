;;; Segment .fini (0000B360)

;; _fini: 0000B360
_fini proc
	{ allocframe(#0x8) }
	{ memw(r29) = r27 }
	{ r27.h = #0x0 }
	{ r27.l = #0x8033 }

l0000B370:
	{ r27 = add(r27,#0x4) }
	{ r0 = memw(r27) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000B384 }

l0000B37C:
	{ callr r0 }
	{ jump 0000B370 }

l0000B384:
	{ r27 = memw(r29); deallocframe }
	{ jumpr r31 }
