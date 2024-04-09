;;; Segment .init (00009440)

;; _init: 00009440
_init proc
	{ allocframe(#0x8) }
	{ memw(r29) = r27 }
	{ memw(r29+4) = r24 }
	{ r24 = PC }
	{ r10.l = #0x8CF3 }
	{ r10.h = #0xFFF7 }
	{ r24 = sub(r24,r10) }
	{ r0.h = #0xFFFF }
	{ r0.l = #0xF1C3 }
	{ r0 = add(r24,r0) }
	{ r1 = memw(r0) }
	{ nop; if (p0.new) jump:nt 00009480; p0 = cmp.eq(r1,#0x0) }

l00009474:
	{ r1 = #0x0; r3 = #0x0; r2 = #0x0 }
	{ call fn000094F0 }

l00009480:
	{ r27.h = #0x0 }
	{ r27.l = #0x4280 }
	{ r27 = add(r24,r27) }

l0000948C:
	{ r27 = add(r27,#0xFFFFFFFC) }
	{ r0 = memw(r27) }
	{ nop; if (p0.new) jump:nt 000094A4; p0 = cmp.eq(r0,#0x0) }

l0000949C:
	{ callr r0 }
	{ jump 0000948C }

l000094A4:
	{ r24 = memw(r29+4) }
	{ deallocframe; r27 = memw(r29) }
	{ jumpr r31 }
