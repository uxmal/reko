;;; Segment .fini (0000B360)

;; _fini: 0000B360
_fini proc
A09DC001     	{ allocframe(+00000008) }
A19DDB00     	{ memw(r29) = r27 }
723BC000     	{ r27.h = 0000 }
71FBE00C     	{ r27.l = 8033 }
B01BC09B     	{ r27 = add(r27,00000004) }
919BC000     	{ r0 = memw(r27) }
1000C006     	{ if (p0.new) jump:nt	0000B384; p0 = cmp.eq(r0,00000000) }
50A0C000     	{ callr	r0 }
59FFFFF8     	{ jump	0000B370 }
919D401B 901EC01E 	{ deallocframe; r27 = memw(r29) }
529FC000     	{ jumpr	r31 }
