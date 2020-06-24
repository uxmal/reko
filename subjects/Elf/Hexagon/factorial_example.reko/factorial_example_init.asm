;;; Segment .init (00003DC0)

;; _init: 00003DC0
_init proc
A09DC001     	{ allocframe(+00000008) }
A19DDB00     	{ memw(r29) = r27 }
7220C000     	{ r0.h = 0000 }
71E0D8A0     	{ r0.l = 6283 }
9180C001     	{ r1 = memw(r0) }
1001C00C     	{ if (p0.new) jump:nt	00003DEC; p0 = cmp.eq(r1,00000000) }
78004002 28032801 	{ r1 = 00000000; r3 = 00000000; r2 = 00000000 }
723CC000     	{ r28.h = 0000 }
717CD6E0     	{ r28.l = 5B81 }
50BCC000     	{ callr	r28 }
723BC000     	{ r27.h = 0000 }
71FBE008     	{ r27.l = 8023 }
BFFBFF9B     	{ r27 = add(r27,FFFFFFFC) }
919BC000     	{ r0 = memw(r27) }
1000C006     	{ if (p0.new) jump:nt	00003E08; p0 = cmp.eq(r0,00000000) }
50A0C000     	{ callr	r0 }
59FFFFF8     	{ jump	00003DF4 }
919D401B 901EC01E 	{ deallocframe; r27 = memw(r29) }
529FC000     	{ jumpr	r31 }
