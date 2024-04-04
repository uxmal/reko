;;; Segment .init (00003DC0)

;; _init: 00003DC0
;;   Called from:
;;     00003DBC (in hexagon_start_main)
_init proc
	{ allocframe(+00000008) }
	{ memw(r29) = r27 }
	{ r0.h = 0000 }
	{ r0.l = 6283 }
	{ r1 = memw(r0) }
	{ if (p0.new) jump:nt 00003DEC; p0 = cmp.eq(r1,00000000) }

l00003DD8:
	{ r1 = 00000000; r3 = 00000000; r2 = 00000000 }
	{ r28.h = 0000 }
	{ r28.l = 5B81 }
	{ callr r28 }

l00003DEC:
	{ r27.h = 0000 }
	{ r27.l = 8023 }

l00003DF4:
	{ r27 = add(r27,FFFFFFFC) }
	{ r0 = memw(r27) }
	{ if (p0.new) jump:nt 00003E08; p0 = cmp.eq(r0,00000000) }

l00003E00:
	{ callr r0 }
	{ jump 00003DF4 }

l00003E08:
	{ deallocframe; r27 = memw(r29) }
	{ jumpr r31 }
