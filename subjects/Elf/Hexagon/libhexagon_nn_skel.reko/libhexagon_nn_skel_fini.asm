;;; Segment .fini (00024E20)

;; _fini: 00024E20
_fini proc
	{ allocframe(#0x8) }
	{ memw(r29) = r27 }
	{ r15 = PC }
	{ r10.l = 0x7462 }
	{ r10.h = 0xFFFF }
	{ r15 = sub(r15,r10) }
	{ r27.h = 0x0 }
	{ r27.l = 0x4290 }
	{ r27 = add(r15,r27) }

l00024E44:
	{ r27 = add(r27,0x4) }
	{ r0 = memw(r27) }
	{ nop; if (p0.new) jump:nt 00024E5C; p0 = cmp.eq(r0,0x0) }

l00024E54:
	{ callr r0 }
	{ jump 00024E44 }

l00024E5C:
	{ deallocframe; r27 = memw(r29) }
	{ jumpr r31 }
