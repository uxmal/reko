;;; Segment .init (00003DC0)

;; _init: 00003DC0
;;   Called from:
;;     00003DBC (in hexagon_start_main)
_init proc
	{ allocframe(#0x8) }
	{ memw(r29) = r27 }
	{ r0.h = #0x0 }
	{ r0.l = #0x6283 }
	{ r1 = memw(r0) }
	{ p0 = cmp.eq(r1,#0x0); if (p0.new) jump:nt 00003DEC }

l00003DD8:
	{ r2 = #0x0; r3 = #0x0; r1 = #0x0 }
	{ r28.h = #0x0 }
	{ r28.l = #0x5B81 }
	{ callr r28 }

l00003DEC:
	{ r27.h = #0x0 }
	{ r27.l = #0x8023 }

l00003DF4:
	{ r27 = add(r27,#0xFFFFFFFC) }
	{ r0 = memw(r27) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00003E08 }

l00003E00:
	{ callr r0 }
	{ jump 00003DF4 }

l00003E08:
	{ r27 = memw(r29); deallocframe }
	{ jumpr r31 }
