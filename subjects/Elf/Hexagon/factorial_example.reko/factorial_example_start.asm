;;; Segment .start (00000000)
00000000 4C C0 00 58 3E C0 00 58 42 C0 00 58 00 00 00 00 L..X>..XB..X....
00000010 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000080 00 C0 3C 72 B8 FB 3C 71 00 C0 9C 52 00 C0 3C 72 ..<r..<q...R..<r
00000090 34 FD 3C 71 00 C0 9C 52                         4.<q...R       

;; hexagon_start_init: 00000098
hexagon_start_init proc
6E9DC000     	{ r0 = rev }
7600DFE0     	{ r0 = and(r0,000000FF) }
7500C040     	{ p0 = cmp.eq(r0,00000002) }
5C20C008     	{ if (!p0) jump:nt	000000B4 }
7220C040     	{ r0.h = 0100 }
7120C000     	{ r0.l = 0000 }
6700C03C     	{ reseved60 = r0 }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C00C004     	{ if (p0) jump:nt	000000C8 }
6700C03C     	{ reseved60 = r0 }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C00C004     	{ if (p0) jump:nt	000000DC }
6700C03D     	{ reseved61 = r0 }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C00C004     	{ if (p0) jump:nt	000000F0 }
6700C03E     	{ reseved62 = r0 }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C00C004     	{ if (p0) jump:nt	00000104 }
6700C03F     	{ reseved63 = r0 }
A200C000     	{ dckill }
57C0C002     	{ isync }
56C0D000     	{ ickill }
57C0C002     	{ isync }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5120C000     	{ if (!p0) callr	r0 }
7800C000     	{ r0 = 00000000 }
6700C006     	{ ssr = r0 }
57C0C002     	{ isync }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121C030     	{ r1.l = 00C0 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121C034     	{ r1.l = 00D0 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C030     	{ r0.l = 00C0 }
9180C000     	{ r0 = memw(r0) }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7220C000     	{ r0.h = 0000 }
7120C034     	{ r0.l = 00D0 }
9180C000     	{ r0 = memw(r0) }
7500C001     	{ p1 = cmp.eq(r0,00000000) }
6B21C002     	{ p2 = or(p0,p1) }
6E92C000     	{ r0 = syscfg }
7B14C003     	{ r3 = mux(p2,00000000,00000028) }
F120C300     	{ r0 = or(r0,r3) }
6700C012     	{ syscfg = r0 }
57C0C002     	{ isync }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C003     	{ p3 = cmp.eq(r0,00000000) }
74E0DFE0     	{ if (!p3) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
71E1E438     	{ r1.l = 90E3 }
4481C003     	{  }
4581D800     	{ if (!p3) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
71E0E438     	{ r0.l = 90E3 }
9180C000     	{ r0 = memw(r0) }
7500C003     	{ p3 = cmp.eq(r0,00000000) }
6B23C200     	{ p0 = or(p2,p3) }
7A00E000     	{ r0 = mux(p0,00000000,00000001) }
7221C000     	{ r1.h = 0000 }
7121C03C     	{ r1.l = 00F0 }
A181C000     	{ memw(r1) = r0 }
9181C000     	{ r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121C038     	{ r1.l = 00E0 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C038     	{ r0.l = 00E0 }
9180C000     	{ r0 = memw(r0) }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
6BC0C000     	{ p0 = not(p0) }
8940C000     	{ r0 = p0 }
7F00C000     	{ nop }
6700C02A     	{ isdben = r0 }
6C20C000     	{ brkpt }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFC4     	{ r1.l = 3F10 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFC8     	{ r1.l = 3F20 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFCC     	{ r1.l = 3F30 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFD0     	{ r1.l = 3F40 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFE0     	{ r1.l = 3F80 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFE4     	{ r1.l = 3F90 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFE8     	{ r1.l = 3FA0 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7221C000     	{ r1.h = 0000 }
7121CFDC     	{ r1.l = 3F70 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7221C000     	{ r1.h = 0000 }
7121CFD8     	{ r1.l = 3F60 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFEC     	{ r1.l = 3FB0 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFF0     	{ r1.l = 3FC0 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFF4     	{ r1.l = 3FD0 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFF8     	{ r1.l = 3FE0 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFC0     	{ r1.l = 3F00 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7480DFE0     	{ if (!p0) r0 = add(r0,FFFFFFFF) }
7221C000     	{ r1.h = 0000 }
7121CFD4     	{ r1.l = 3F50 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7221C000     	{ r1.h = 0000 }
7121CFFC     	{ r1.l = 3FF0 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120CFD8     	{ r0.l = 3F60 }
9180C000     	{ r0 = memw(r0) }
6700C010     	{ evb = r0 }
7220C000     	{ r0.h = 0000 }
7120DACC     	{ r0.l = 6B30 }
9180C000     	{ r0 = memw(r0) }
6700C000     	{ sgp0 = r0 }
7220C000     	{ r0.h = 0000 }
7120CFEC     	{ r0.l = 3FB0 }
9180C000     	{ r0 = memw(r0) }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
6BC0C000     	{ p0 = not(p0) }
8940C001     	{ r1 = p0 }
6E92C000     	{ r0 = syscfg }
8F01C1C0     	{ r0 = insert(00000001,00000001) }
6700C012     	{ syscfg = r0 }
7220C000     	{ r0.h = 0000 }
7120CFF0     	{ r0.l = 3FC0 }
9180C000     	{ r0 = memw(r0) }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
6BC0C000     	{ p0 = not(p0) }
8940C001     	{ r1 = p0 }
6E92C000     	{ r0 = syscfg }
8F21C1E0     	{ r0 = insert(00000001,00000009) }
6700C012     	{ syscfg = r0 }
7220C000     	{ r0.h = 0000 }
7120CFF4     	{ r0.l = 3FD0 }
9180C000     	{ r0 = memw(r0) }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
6BC0C000     	{ p0 = not(p0) }
8940C001     	{ r1 = p0 }
6E92C000     	{ r0 = syscfg }
8F21C1A0     	{ r0 = insert(00000001,00000009) }
6700C012     	{ syscfg = r0 }
7220C000     	{ r0.h = 0000 }
7120CFF8     	{ r0.l = 3FE0 }
9180C000     	{ r0 = memw(r0) }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
6BC0C000     	{ p0 = not(p0) }
8940C001     	{ r1 = p0 }
6E86C000     	{ r0 = ssr }
8F61C1E0     	{ r0 = insert(00000001,00000019) }
6700C006     	{ ssr = r0 }
6E9DC000     	{ r0 = rev }
7600DFE0     	{ r0 = and(r0,000000FF) }
7580C040     	{ p0 = cmp.gtu(r0,00000002) }
5C00C00A     	{ if (p0) jump:nt	000004F8 }
6E92C000     	{ r0 = syscfg }
8D40C281     	{ r1 = extractu(r0,00000002,00000012) }
7501C000     	{ p0 = cmp.eq(r1,00000000) }
5C00C03E     	{ if (p0) jump:nt	00000570 }
6E87C000     	{ r0 = ccr }
7800C003     	{ r3 = 00000000 }
8F43C400     	{ r0 = insert(00000004,00000014) }
6700C007     	{ ccr = r0 }
6E92C000     	{ r0 = syscfg }
8F43C300     	{ r0 = insert(00000003,00000013) }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
57C0C002     	{ isync }
6E9DC001     	{ r1 = rev }
7601DFE1     	{ r1 = and(r1,000000FF) }
7501C040     	{ p0 = cmp.eq(r1,00000002) }
5C00C004     	{ if (p0) jump:nt	00000538 }
A840C000     	{ syncht }
6700C012     	{ syscfg = r0 }
57C0C002     	{ isync }
A820C000     	{ l2kill }
6E9DC002     	{ r2 = rev }
8D22C482     	{ r2 = extractu(r2,00000004,0000000C) }
0000406D 9D02D102 7223C000 	{ r3.h = 0000; r2 = memb(r2+00001B44) }
7123CFC0     	{ r3.l = 3F00 }
9183C003     	{ r3 = memw(r3) }
D5A3C203     	{ r3 = min(r3,r2) }
8F43C300     	{ r0 = insert(00000003,00000013) }
6700C012     	{ syscfg = r0 }
57C0C002     	{ isync }
7221C000     	{ r1.h = 0000 }
7121CFC4     	{ r1.l = 3F10 }
9181C001     	{ r1 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120CFCC     	{ r0.l = 3F30 }
9180C000     	{ r0 = memw(r0) }
6E92C002     	{ r2 = syscfg }
8F01C122     	{ r2 = insert(00000001,00000001) }
8F00C142     	{ r2 = insert(00000001,00000001) }
6E9DC000     	{ r0 = rev }
7600DFE0     	{ r0 = and(r0,000000FF) }
7500C040     	{ p0 = cmp.eq(r0,00000002) }
5C00C014     	{ if (p0) jump:nt	000005C8 }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C00C004     	{ if (p0) jump:nt	000005B8 }
8CC2D802     	{ r2 = setbit(r2,00000010) }
7221C000     	{ r1.h = 0000 }
7121CFD4     	{ r1.l = 3F50 }
9181C001     	{ r1 = memw(r1) }
8F41C1E2     	{ r2 = insert(00000001,00000011) }
6702C012     	{ syscfg = r2 }
57C0C002     	{ isync }
6E87C002     	{ r2 = ccr }
7220C000     	{ r0.h = 0000 }
7120CFC8     	{ r0.l = 3F20 }
9180C000     	{ r0 = memw(r0) }
7221C000     	{ r1.h = 0000 }
7121CFD0     	{ r1.l = 3F40 }
9181C001     	{ r1 = memw(r1) }
8E41C1C0     	{ r0 |= asl(r1,00000001) }
8F40C202     	{ r2 = insert(00000002,00000012) }
8CC2D402     	{ r2 = setbit(r2,00000008) }
6702C007     	{ ccr = r2 }
7600C000     	{ r0 = and(r0,00000000) }
8CC0D000     	{ r0 = setbit(r0,00000000) }
6220C008     	{ USR = r0 }
57C0C002     	{ isync }
7220C000     	{ r0.h = 0000 }
7120CFE0     	{ r0.l = 3F80 }
9180C000     	{ r0 = memw(r0) }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C00C046     	{ if (p0) jump:nt	000006A8 }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7540C020     	{ p0 = cmp.gt(r0,00000001) }
5C00C018     	{ if (p0) jump:nt	0000065C }
7220C000     	{ r0.h = 0000 }
7120CFE8     	{ r0.l = 3FA0 }
9180C000     	{ r0 = memw(r0) }
7500C003     	{ p3 = cmp.eq(r0,00000000) }
7B9FE3E0     	{ r0 = mux(p3,0000001F,0000003F) }
7C00C002     	{ r3:r2 = combine(00000000,00000000) }
6C02C000     	{ tlbw(r3:r2,r0) }
57C0C002     	{ isync }
BFE0FFE0     	{ r0 = add(r0,FFFFFFFF) }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5CFFE0F8     	{ if (!p0) jump:nt	00000648 }
7222C000     	{ r2.h = 0000 }
7122C000     	{ r2.l = 0000 }
8C02CC22     	{ r2 = lsr(r2,0000000C) }
72E1C3F0     	{ r1.h = 0FC3 }
7121C000     	{ r1.l = 0000 }
72E0F700     	{ r0.h = DC03 }
7120C000     	{ r0.l = 0000 }
F121C201     	{ r1 = or(r1,r2) }
8E42C1C0     	{ r0 |= asl(r2,00000001) }
8CC0C400     	{ r0 = setbit(r0,00000008) }
7620FE00     	{ r0 = and(r0,FFFFFFF0) }
7800C002     	{ r2 = 00000000 }
6C00C300     	{ tlbw(r1:r0,r3) }
6E92C000     	{ r0 = syscfg }
8CC0C000     	{ r0 = setbit(r0,00000000) }
8943C001     	{ r1 = p3 }
8F01C1E0     	{ r0 = insert(00000001,00000001) }
6700C012     	{ syscfg = r0 }
57C0C002     	{ isync }
7800C000     	{ r0 = 00000000 }
6700C00A     	{ imask = r0 }
78DFFFE1     	{ r1 = FFFFFFFF }
6701C018     	{ iel = r1 }
6701C01A     	{ iahl = r1 }
6401C020     	{ cswi(r1) }
6E86C000     	{ r0 = ssr }
8CC0D200     	{ r0 = setbit(r0,00000004) }
6700C006     	{ ssr = r0 }
6E92C000     	{ r0 = syscfg }
8CC0C400     	{ r0 = setbit(r0,00000008) }
6700C012     	{ syscfg = r0 }
57C0C002     	{ isync }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	000006F4 }
6700C007     	{ ccr = r0 }
57C0C002     	{ isync }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	0000070C }
6220C008     	{ USR = r0 }
57C0C002     	{ isync }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	00000724 }
6700C012     	{ syscfg = r0 }
57C0C002     	{ isync }
7220C000     	{ r0.h = 0000 }
71E0E438     	{ r0.l = 90E3 }
9180C000     	{ r0 = memw(r0) }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C00C016     	{ if (p0) jump:nt	00000760 }
7220C000     	{ r0.h = 0000 }
71E0E018     	{ r0.l = 8063 }
7800C401     	{ r1 = 00000020 }
723CC000     	{ r28.h = 0000 }
717CD610     	{ r28.l = 5841 }
50BCC000     	{ callr	r28 }
7800C2C0     	{ r0 = 00000016 }
7221C000     	{ r1.h = 0000 }
71E1E028     	{ r1.l = 80A3 }
5400C000     	{ trap0(00000000) }
723CC000     	{ r28.h = 0000 }
713CFBB8     	{ r28.l = EEE0 }
529CC000     	{ jumpr	r28 }
7F00C000     	{ nop }

;; __coredump: 00000770
__coredump proc
7220C000     	{ r0.h = 0000 }
7120CFFC     	{ r0.l = 3FF0 }
9180C000     	{ r0 = memw(r0) }
ABC0C008     	{ memd(r0++#8) = r1:r0 }
ABC0C208     	{ memd(r0++#8) = r3:r2 }
ABC0C408     	{ memd(r0++#8) = r5:r4 }
A1C0C600     	{ memd(r0) = r7:r6 }
A000C000     	{ dccleana(r0) }
B000C100     	{ r0 = add(r0,00000008) }
ABC0C808     	{ memd(r0++#8) = r9:r8 }
ABC0CA08     	{ memd(r0++#8) = r11:r10 }
ABC0CC08     	{ memd(r0++#8) = r13:r12 }
A1C0CE00     	{ memd(r0) = r15:r14 }
A000C000     	{ dccleana(r0) }
B000C100     	{ r0 = add(r0,00000008) }
ABC0D008     	{ memd(r0++#8) = r17:r16 }
ABC0D208     	{ memd(r0++#8) = r19:r18 }
ABC0D408     	{ memd(r0++#8) = r21:r20 }
A1C0D600     	{ memd(r0) = r23:r22 }
A000C000     	{ dccleana(r0) }
B000C100     	{ r0 = add(r0,00000008) }
ABC0D808     	{ memd(r0++#8) = r25:r24 }
ABC0DA08     	{ memd(r0++#8) = r27:r26 }
ABC0DC08     	{ memd(r0++#8) = r29:r28 }
A1C0DE00     	{ memd(r0) = r31:r30 }
A000C000     	{ dccleana(r0) }
B000C100     	{ r0 = add(r0,00000008) }
6A00C001     	{ r1 = SA0 }
AB80C108     	{ memw(r0++#4) = r1 }
6A01C001     	{ r1 = LC0 }
AB80C108     	{ memw(r0++#4) = r1 }
6A02C001     	{ r1 = SA1 }
AB80C108     	{ memw(r0++#4) = r1 }
6A03C001     	{ r1 = LC1 }
AB80C108     	{ memw(r0++#4) = r1 }
6A04C001     	{ r1 = P3:0 }
AB80C108     	{ memw(r0++#4) = r1 }
6A06C001     	{ r1 = M0 }
AB80C108     	{ memw(r0++#4) = r1 }
6A07C001     	{ r1 = M1 }
AB80C108     	{ memw(r0++#4) = r1 }
6A08C001     	{ r1 = USR }
A180C100     	{ memw(r0) = r1 }
A000C000     	{ dccleana(r0) }
B000C080     	{ r0 = add(r0,00000004) }
6A09C001     	{ r1 = PC }
AB80C108     	{ memw(r0++#4) = r1 }
6A0AC001     	{ r1 = UGP }
AB80C108     	{ memw(r0++#4) = r1 }
6E80C001     	{ r1 = sgp0 }
AB80C108     	{ memw(r0++#4) = r1 }
6E86C001     	{ r1 = ssr }
AB80C108     	{ memw(r0++#4) = r1 }
6E8AC001     	{ r1 = imask }
AB80C108     	{ memw(r0++#4) = r1 }
6E89C001     	{ r1 = badva }
AB80C108     	{ memw(r0++#4) = r1 }
6E83C001     	{ r1 = elr }
AB80C108     	{ memw(r0++#4) = r1 }
6E82C001     	{ r1 = stid }
A180C100     	{ memw(r0) = r1 }
A000C000     	{ dccleana(r0) }
B000C080     	{ r0 = add(r0,00000004) }
6E90C001     	{ r1 = evb }
AB80C108     	{ memw(r0++#4) = r1 }
6E94C001     	{ r1 = ipend }
AB80C108     	{ memw(r0++#4) = r1 }
6E92C001     	{ r1 = syscfg }
AB80C108     	{ memw(r0++#4) = r1 }
6E91C001     	{ r1 = modectl }
AB80C108     	{ memw(r0++#4) = r1 }
6E9DC001     	{ r1 = rev }
AB80C108     	{ memw(r0++#4) = r1 }
7800C001     	{ r1 = 00000000 }
AB80C108     	{ memw(r0++#4) = r1 }
7800C001     	{ r1 = 00000000 }
AB80C108     	{ memw(r0++#4) = r1 }
7800C001     	{ r1 = 00000000 }
A180C100     	{ memw(r0) = r1 }
A000C000     	{ dccleana(r0) }
B000C080     	{ r0 = add(r0,00000004) }
6E9CC001     	{ r1 = diag }
AB80C108     	{ memw(r0++#4) = r1 }
6E96C001     	{ r1 = iad }
AB80C108     	{ memw(r0++#4) = r1 }
6E98C001     	{ r1 = iel }
AB80C108     	{ memw(r0++#4) = r1 }
6E9AC001     	{ r1 = iahl }
AB80C108     	{ memw(r0++#4) = r1 }
6E9FC001     	{ r1 = pcyclehi }
AB80C108     	{ memw(r0++#4) = r1 }
6E9EC001     	{ r1 = pcyclelo }
AB80C108     	{ memw(r0++#4) = r1 }
6EA0C001     	{ r1 = isdbst }
AB80C108     	{ memw(r0++#4) = r1 }
6EA1C001     	{ r1 = isdbcfg0 }
A180C100     	{ memw(r0) = r1 }
A000C000     	{ dccleana(r0) }
B000C080     	{ r0 = add(r0,00000004) }
6EA2C001     	{ r1 = isdbcfg1 }
AB80C108     	{ memw(r0++#4) = r1 }
6EA4C001     	{ r1 = brkptpc0 }
AB80C108     	{ memw(r0++#4) = r1 }
6EA5C001     	{ r1 = brkptcfg0 }
AB80C108     	{ memw(r0++#4) = r1 }
6EA6C001     	{ r1 = brkptpc1 }
AB80C108     	{ memw(r0++#4) = r1 }
6EA7C001     	{ r1 = brkptcfg1 }
AB80C108     	{ memw(r0++#4) = r1 }
6EAAC001     	{ r1 = isdben }
AB80C108     	{ memw(r0++#4) = r1 }
6EABC001     	{ r1 = isdgpr }
A180C100     	{ memw(r0) = r1 }
A000C000     	{ dccleana(r0) }
9780F761     	{ r1 = memw(r0-276) }
6E86C000     	{ r0 = ssr }
8CC0D020     	{ r0 = clrbit(r0,00000000) }
8CC0D120     	{ r0 = clrbit(r0,00000002) }
6700C006     	{ ssr = r0 }
57C0C002     	{ isync }
7800D9A0     	{ r0 = 000000CD }
5400C000     	{ trap0(00000000) }
78DFFFE2     	{ r2 = FFFFFFFF }
78DFFFE0     	{ r0 = FFFFFFFF }
6460C000     	{ stop(r0) }
7F004000 7F004000 7F00C000 	{ nop; nop; nop }

;; event_handle_reset: 00000970
event_handle_reset proc
6E88C001     	{ r1 = htid }
723C4000 723D4000 7220C000 	{ r0.h = 0000; r29.h = 0000; r28.h = 0000 }
713C5AE4 8C014241 713D5AFC 7120DB14 	{ r0.l = 6C50; r29.l = 6BF0; r1 = asl(r1,00000002); r28.l = 6B90 }
F31C411C F31D411D F300C100 	{ r0 = add(r0,r1); r29 = add(r29,r1); r28 = add(r28,r1) }
7800C002     	{ r2 = 00000000 }
6702C006     	{ ssr = r2 }
57C0C002     	{ isync }
6702C00A     	{ imask = r2 }
7222C000     	{ r2.h = 0000 }
7122DACC     	{ r2.l = 6B30 }
F302C102     	{ r2 = add(r2,r1) }
9182C002     	{ r2 = memw(r2) }
6702C000     	{ sgp0 = r2 }
7222C001     	{ r2.h = 0004 }
7122C000     	{ r2.l = 0000 }
6222C00B     	{ gp = r2 }
7222C004     	{ r2.h = 0010 }
7122C000     	{ r2.l = 0000 }
6702C006     	{ ssr = r2 }
7222C000     	{ r2.h = 0000 }
7122CFF8     	{ r2.l = 3FE0 }
9182C002     	{ r2 = memw(r2) }
7502C010     	{ p0 = !cmp.eq(r2,00000000) }
8940C001     	{ r1 = p0 }
6E86C002     	{ r2 = ssr }
8F61C1E2     	{ r2 = insert(00000001,00000019) }
6702C006     	{ ssr = r2 }
7222C001     	{ r2.h = 0004 }
6222C008     	{ USR = r2 }
7222C013     	{ r2.h = 004C }
6702C007     	{ ccr = r2 }
57C0C002     	{ isync }
72A2FABE     	{ r2.h = EAFA }
71A2FEEF     	{ r2.l = FBBE }
30213023     	{ r3 = r2; r1 = r2 }
F5034204 F5034206 F5034208 F503C20A 	{ r11:r10 = combine(r3,r2); r9:r8 = combine(r3,r2); r7:r6 = combine(r3,r2); r5:r4 = combine(r3,r2) }
F503420C F503420E F5034210 F503C212 	{ r19:r18 = combine(r3,r2); r17:r16 = combine(r3,r2); r15:r14 = combine(r3,r2); r13:r12 = combine(r3,r2) }
F5034214 F5034216 F5034218 F503C21A 	{ r27:r26 = combine(r3,r2); r25:r24 = combine(r3,r2); r23:r22 = combine(r3,r2); r21:r20 = combine(r3,r2) }
723F4000 919C401C 919DC01D 	{ r29 = memw(r29); r28 = memw(r28); r31.h = 0000 }
717F5240 58004008 7800401E 9180C000 	{ r0 = memw(r0); r30 = 00000000; jump	00000A68; r31.l = 4901 }

;; thread_start: 00000A64
thread_start proc
529CC000     	{ jumpr	r28 }

;; event_handle_nmi: 00000A68
event_handle_nmi proc
7800C020     	{ r0 = 00000001 }
6700C002     	{ stid = r0 }
59FFFE80     	{ jump	00000770 }

;; event_handle_error: 00000A74
event_handle_error proc
7800C040     	{ r0 = 00000002 }
6700C002     	{ stid = r0 }
59FFFE7A     	{ jump	00000770 }

;; event_handle_rsvd: 00000A80
event_handle_rsvd proc
72E0DEAD     	{ r0.h = 7AB7 }
71A0FEEF     	{ r0.l = FBBE }
6700C002     	{ stid = r0 }
59FFFE72     	{ jump	00000770 }
7F004000 7F004000 7F004000 7F00C000 	{ nop; nop; nop; nop }
7F004000 7F004000 7F004000 7F00C000 	{ nop; nop; nop; nop }
7F004000 7F004000 7F004000 7F00C000 	{ nop; nop; nop; nop }

;; event_handle_tlbmissx: 00000AC0
event_handle_tlbmissx proc
651DC000     	{ crswap(r29,sgp0) }
BFFDF81D     	{ r29 = add(r29,FFFFFFC0) }
A1DDC000     	{ memd(r29) = r1:r0 }
A1DDC201     	{ memd(r29+8) = r3:r2 }
A1DDC402     	{ memd(r29+16) = r5:r4 }
A1DDC603     	{ memd(r29+24) = r7:r6 }
A1DDC804     	{ memd(r29+32) = r9:r8 }
6A04C009     	{ r9 = P3:0 }
6E86C008     	{ r8 = ssr }
6E83C007     	{ r7 = elr }
8508C001     	{ p1 = tstbit(r8,00000000) }
8C074C27 5C204108 7220C000 	{ r0.h = 0000; if (!p1) jump:nt	00000B00; r7 = lsr(r7,0000000C) }
B007C027     	{ r7 = add(r7,00000001) }
71205B84 7800C0C1 	{ r1 = 00000006; r0.l = 6E10 }
9200C006     	{ r6 = memw_locked(r0) }
B0064026 7546CFC0 	{ p0 = cmp.gt(r6,0000007E); r6 = add(r6,00000001) }
F4014606 7223C000 	{ r3.h = 0000; r6 = mux(p0,r1,r6) }
A0A0C600     	{ memw_locked(r0,p0) = r6 }
5CFF60F4 7123CFDC 	{ r3.l = 3F70; if (!p0) jump:nt	00000B04 }
8C074827 9183C003 	{ r3 = memw(r3); r7 = lsr(r7,00000008) }
C407C323     	{ r3 = addasl(r3,r7,00000001) }
8C074847 9143C003 	{ r3 = memh(r3); r7 = asl(r7,00000008) }
8D03CC85     	{ r5 = extractu(r3,0000000C,00000004) }
8D034404 29002801 	{ r1 = 00000000; r0 = 00000010; r4 = extractu(r3,00000004,00000004) }
8C045844 72E14000 72E0F000 	{ r0.h = C003; r1.h = 0003; r4 = asl(r4,00000018) }
F1214701 8E45C9C0 	{ r0 |= asl(r5,00000009); r1 = or(r1,r7) }
F1204400 7224C000 	{ r4.h = 0000; r0 = or(r0,r4) }
71245B80 7800C025 	{ r5 = 00000001; r4.l = 6E00 }
9204C002     	{ r2 = memw_locked(r4) }
7502C000     	{ p0 = cmp.eq(r2,00000000) }
5CFFE0FC     	{ if (!p0) jump:nt	00000B68 }
A0A4C500     	{ memw_locked(r4,p0) = r5 }
5CFFE0F8     	{ if (!p0) jump:nt	00000B68 }
6C81C005     	{ r5 = tlbp(r1) }
8505DF00     	{ p0 = tstbit(r5,0000001F) }
5C20C006     	{ if (!p0) jump:nt	00000B90 }
6C00C600     	{ tlbw(r1:r0,r6) }
57C0C002     	{ isync }
7800C001     	{ r1 = 00000000 }
9204C002     	{ r2 = memw_locked(r4) }
A0A4C100     	{ memw_locked(r4,p0) = r1 }
5CFFE0FC     	{ if (!p0) jump:nt	00000B94 }
6229C004     	{ P3:0 = r9 }
91DD4088 91DDC066 	{ r7:r6 = memd(r29+24); r9:r8 = memd(r29+32) }
3E121E09     	{ r3:r2 = memd(r29+8); r5:r4 = memd(r29+16) }
B01D481D 91DDC000 	{ r1:r0 = memd(r29); r29 = add(r29,00000040) }
651DC000     	{ crswap(r29,sgp0) }
57E0C000     	{ rte }

;; event_handle_tlbmissrw: 00000BC0
event_handle_tlbmissrw proc
651DC000     	{ crswap(r29,sgp0) }
BFFDF81D     	{ r29 = add(r29,FFFFFFC0) }
A1DDC000     	{ memd(r29) = r1:r0 }
A1DDC201     	{ memd(r29+8) = r3:r2 }
A1DDC402     	{ memd(r29+16) = r5:r4 }
A1DDC603     	{ memd(r29+24) = r7:r6 }
A1DDC804     	{ memd(r29+32) = r9:r8 }
6E86C008     	{ r8 = ssr }
6E89C007     	{ r7 = badva }
6A044009 71205B84 7800C0C1 	{ r1 = 00000006; r0.l = 6E10; r9 = P3:0 }
8C075427 7220C000 	{ r0.h = 0000; r7 = lsr(r7,00000014) }
9200C006     	{ r6 = memw_locked(r0) }
B0064026 75464FC0 7223C000 	{ r3.h = 0000; p0 = cmp.gt(r6,0000007E); r6 = add(r6,00000001) }
F4014606 7123CFDC 	{ r3.l = 3F70; r6 = mux(p0,r1,r6) }
A0A0C600     	{ memw_locked(r0,p0) = r6 }
5CFFE0F2     	{ if (!p0) jump:nt	00000BF8 }
9183C003     	{ r3 = memw(r3) }
C407C323     	{ r3 = addasl(r3,r7,00000001) }
8C074847 9143C003 	{ r3 = memh(r3); r7 = asl(r7,00000008) }
8D03CC85     	{ r5 = extractu(r3,0000000C,00000004) }
8D034404 29002801 	{ r1 = 00000000; r0 = 00000010; r4 = extractu(r3,00000004,00000004) }
8C045844 72E14000 72E0F000 	{ r0.h = C003; r1.h = 0003; r4 = asl(r4,00000018) }
F1214701 8E45C9C0 	{ r0 |= asl(r5,00000009); r1 = or(r1,r7) }
F1204400 7224C000 	{ r4.h = 0000; r0 = or(r0,r4) }
71245B80 7800C025 	{ r5 = 00000001; r4.l = 6E00 }
9204C002     	{ r2 = memw_locked(r4) }
A0A4C500     	{ memw_locked(r4,p0) = r5 }
7502C001     	{ p1 = cmp.eq(r2,00000000) }
5CFFE0FA     	{ if (!p0) jump:nt	00000C58 }
5CFFE1F8     	{ if (!p1) jump:nt	00000C58 }
6C81C005     	{ r5 = tlbp(r1) }
8505DF00     	{ p0 = tstbit(r5,0000001F) }
5C20C006     	{ if (!p0) jump:nt	00000C80 }
6C00C600     	{ tlbw(r1:r0,r6) }
57C0C002     	{ isync }
7800C001     	{ r1 = 00000000 }
9204C002     	{ r2 = memw_locked(r4) }
A0A4C100     	{ memw_locked(r4,p0) = r1 }
5CFFE0FC     	{ if (!p0) jump:nt	00000C84 }
6229C004     	{ P3:0 = r9 }
91DD4088 91DDC066 	{ r7:r6 = memd(r29+24); r9:r8 = memd(r29+32) }
3E121E09     	{ r3:r2 = memd(r29+8); r5:r4 = memd(r29+16) }
B01D481D 91DDC000 	{ r1:r0 = memd(r29); r29 = add(r29,00000040) }
651DC000     	{ crswap(r29,sgp0) }
57E0C000     	{ rte }

;; event_handle_trap0: 00000CB0
event_handle_trap0 proc
651DC000     	{ crswap(r29,sgp0) }
6A044005 BFFD7B1D 75004800 A7DDE4FB 	{ memd(r29-40) = r5:r4; p0 = cmp.eq(r0,00000040); r29 = add(r29,FFFFFFD8); r5 = P3:0 }
75004881 75004A42 72645555 A1DDC201 	{ memd(r29+8) = r3:r2; r4.h = 5555; p2 = cmp.eq(r0,00000052); p1 = cmp.eq(r0,00000044) }
7223C000     	{ r3.h = 0000 }
7123C03C     	{ r3.l = 00F0 }
9183C003     	{ r3 = memw(r3) }
75034023 5C00CB8E 	{ if (p3.new) jump:nt	00000E00; p3 = cmp.eq(r3,00000001) }
5C00407A 6B224100 71645555 A1DDC602 	{ memd(r29+16) = r7:r6; r4.l = 5555; p0 = or(p1,p2); if (p0) jump:nt	00000DDC }
5C004072 72234000 780059A6 A1DDC803 	{ memd(r29+24) = r9:r8; r6 = 000000CD; r3.h = 0000; if (p0) jump:nt	00000DDC }
71234040 72244000 75004302 F200C603 	{ p3 = cmp.eq(r0,r6); p2 = cmp.eq(r0,00000018); r4.h = 0000; r3.l = 0100 }
70654009 6B234203 71244020 9183C007 	{ r7 = memw(r3); r4.l = 0080; p3 = or(p2,p3); r9 = r5 }
6E884008 7F00C000 	{ nop; r8 = htid }
72264002 5C004370 75074000 7F00C000 	{ nop; p0 = cmp.eq(r7,00000000); if (p3) jump:nt	00000E14; r6.h = 0008 }
75074000 5C204000 48150037 	{ r7 = memw(r3); r5 = 00000001; if (!p0) jump:nt	00000D44; p0 = cmp.eq(r7,00000000) }
9203C007     	{ r7 = memw_locked(r3) }
A0A3C501     	{ memw_locked(r3,p1) = r5 }
5CFF61FC 7507C000 	{ p0 = cmp.eq(r7,00000000); if (!p1) jump:nt	00000D4C }
5CFFE0F2     	{ if (!p0) jump:nt	00000D40 }
F4024842 A1C4C000 	{ memd(r4) = r1:r0; r2 = mux(p2,r2,r8) }
A184C202     	{ memw(r4+8) = r2 }
A184C503     	{ memw(r4+12) = r5 }
A044C000     	{ dccleaninva(r4) }
9104C1E6     	{ r6 = memb(r4+15) }
A044C000     	{ dccleaninva(r4) }
7226C000     	{ r6.h = 0000 }
7126C03C     	{ r6.l = 00F0 }
9186C006     	{ r6 = memw(r6) }
1006410C 28062805 	{ r5 = 00000000; r6 = 00000000; if (p0.new) jump:nt	00000DA0; p0 = cmp.eq(r6,00000002) }
9184C066     	{ r6 = memw(r4+12) }
A044C000     	{ dccleaninva(r4) }
78004005 91C4C000 	{ r1:r0 = memd(r4); r5 = 00000000 }
9203C007     	{ r7 = memw_locked(r3) }
A0A3C501     	{ memw_locked(r3,p1) = r5 }
5CFF61FC 79602884 	{ memw(r29+32) = r4; p0 = cmp.eq(r6,00000000); if (!p1) jump:nt	00000DA0 }
5C204010 3E021E13 	{ r7:r6 = memd(r29+16); r5:r4 = memd(r29); if (!p0) jump:nt	00000DD0 }
62294004 B01D451D 91DD4022 91DDC068 	{ r9:r8 = memd(r29+24); r3:r2 = memd(r29+8); r29 = add(r29,00000028); P3:0 = r9 }
651DC000     	{ crswap(r29,sgp0) }
57E0C000     	{ rte }
78005820 163741B8 919DC104 	{ r4 = memw(r29+32); jump	00000D44; r7 = 00000001; r0 = 000000C1 }
6E9FC001     	{ r1 = pcyclehi }
6E9EC000     	{ r0 = pcyclelo }
5C00420C E5404402 91DDC046 	{ r7:r6 = memd(r29+16); r3:r2 = mpyu(r0,r4); if (p2) jump:nt	00000DFC }
E5414400 8002E022 	{ r3:r2 = lsr(r3:r2,00000020); r1:r0 = mpyu(r1,r4) }
D300C2E0     	{ r1:r0 = add(r3:r2,r1:r0) }
62254004 B01D451D 3E091E02 	{ r5:r4 = memd(r29); r3:r2 = memd(r29+8); r29 = add(r29,00000028); P3:0 = r5 }
651DC000     	{ crswap(r29,sgp0) }
57E0C000     	{ rte }
5C204208 7126C026 	{ r6.l = 0098; if (!p2) jump:nt	00000E20 }
F2014602 59FFFF94 	{ jump	00000D44; p2 = cmp.eq(r1,r6) }
7221C000     	{ r1.h = 0000 }
7121CFFC     	{ r1.l = 3FF0 }
9181C001     	{ r1 = memw(r1) }
59FFFF8A     	{ jump	00000D40 }

;; event_handle_trap1: 00000E30
event_handle_trap1 proc
7800C120     	{ r0 = 00000009 }
6700C002     	{ stid = r0 }
59FFFC9C     	{ jump	00000770 }

;; event_handle_int: 00000E3C
event_handle_int proc
651DC000     	{ crswap(r29,sgp0) }
A09DC014     	{ allocframe(+000000A0) }
6A004000 A1DDC000 	{ memd(r29) = r1:r0; r0 = SA0 }
6A014001 A1DDC201 	{ memd(r29+8) = r3:r2; r1 = LC0 }
6A024002 A1DDC402 	{ memd(r29+16) = r5:r4; r2 = SA1 }
6A034003 A1DDC603 	{ memd(r29+24) = r7:r6; r3 = LC1 }
6A044004 A1DDC804 	{ memd(r29+32) = r9:r8; r4 = P3:0 }
6A064005 A1DDCA05 	{ memd(r29+40) = r11:r10; r5 = M0 }
6A074006 A1DDCC06 	{ memd(r29+48) = r13:r12; r6 = M1 }
6A084007 A1DDCE07 	{ memd(r29+56) = r15:r14; r7 = USR }
6A0A4008 A1DDD008 	{ memd(r29+64) = r17:r16; r8 = UGP }
6E83C009     	{ r9 = elr }
A1DDD209     	{ memd(r29+72) = r19:r18 }
A1DDD40A     	{ memd(r29+80) = r21:r20 }
A1DDD60B     	{ memd(r29+88) = r23:r22 }
A1DDD80C     	{ memd(r29+96) = r25:r24 }
A1DDDA0D     	{ memd(r29+104) = r27:r26 }
A1DDC00E     	{ memd(r29+112) = r1:r0 }
6E86C000     	{ r0 = ssr }
72214000 8F404807 70022A79 	{ memd(r29+120) = r3:r2; r2 = r0; r7 = insert(00000008,00000010); r1.h = 0000 }
760043E0 71215000 A1DDC410 	{ memd(r29+128) = r5:r4; r1.l = 4000; r0 = and(r0,0000001F) }
C4004141 A1DDC611 	{ memd(r29+136) = r7:r6; r1 = addasl(r1,r0,00000002) }
78004003 707C401F 91814001 A1DDC812 	{ memd(r29+144) = r9:r8; r1 = memw(r1); r31 = r28; r3 = 00000000 }
8F434302 75014000 A1DDDE13 	{ memd(r29+152) = r31:r30; p0 = cmp.eq(r1,00000000); r2 = insert(00000003,00000013) }
5C00C01E     	{ if (p0) jump:nt	00000F24 }
6702C006     	{ ssr = r2 }
651DC000     	{ crswap(r29,sgp0) }
50A1C000     	{ callr	r1 }
651DC000     	{ crswap(r29,sgp0) }
6E86C000     	{ r0 = ssr }
723A4000 48613E8B 	{ r7:r6 = memd(r29+136); r1 = 00000006; r26.h = 0000 }
70274007 8F414300 713AC001 	{ r26.l = 0004; r0 = insert(00000003,00000013); r7 = asrh(r7) }
7607C3E7     	{ r7 = and(r7,0000001F) }
C65AC7DA     	{ r26 &= lsl(r26,r7) }
6700C006     	{ ssr = r0 }
641AC060     	{ ciad(r26) }
91DD427E 91DDC248 	{ r9:r8 = memd(r29+144); r31:r30 = memd(r29+152) }
6709C003     	{ elr = r9 }
6228400A 3E8B1E82 	{ r5:r4 = memd(r29+128); r7:r6 = memd(r29+136); UGP = r8 }
62274008 707F401C 3E791E70 	{ r1:r0 = memd(r29+112); r3:r2 = memd(r29+120); r28 = r31; USR = r7 }
62264007 91DD41BA 91DDC198 	{ r25:r24 = memd(r29+96); r27:r26 = memd(r29+104); M1 = r6 }
62254006 3E5F1E56 	{ r21:r20 = memd(r29+80); r23:r22 = memd(r29+88); M0 = r5 }
62244004 3E4D1E44 	{ r17:r16 = memd(r29+64); r19:r18 = memd(r29+72); P3:0 = r4 }
62234003 91DD40EE 91DDC0CC 	{ r13:r12 = memd(r29+48); r15:r14 = memd(r29+56); LC1 = r3 }
62224002 91DD40AA 91DDC088 	{ r9:r8 = memd(r29+32); r11:r10 = memd(r29+40); SA1 = r2 }
62214001 3E1B1E12 	{ r5:r4 = memd(r29+16); r7:r6 = memd(r29+24); LC0 = r1 }
62204000 3E091E00 	{ r1:r0 = memd(r29); r3:r2 = memd(r29+8); SA0 = r0 }
901EC01E     	{ deallocframe }
651DC000     	{ crswap(r29,sgp0) }
57E0C000     	{ rte }
7F004000 7F004000 7F00C000 	{ nop; nop; nop }

;; .NoHandler: 00000FA0
.NoHandler proc
529FC000     	{ jumpr	r31 }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000003     	{ r3 = memw(r0); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00005000 00001B88 00000001 	{ r1 = memw(r0); r0 = memw(r0); r16 = memb(r16+3); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00000001     	{ r1 = memw(r0); r0 = memw(r0) }
00001080     	{ r0 = memb(r16); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000FA0     	{ r0 = memw(r18+28); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00001320     	{ r0 = memb(r2+3); r0 = memw(r0) }
000014A8     	{ r16 = memb(r18+4); r0 = memw(r0) }
00001630     	{ r0 = memb(r3+6); r0 = memw(r0) }
000017B8     	{ r16 = memb(r19+7); r0 = memw(r0) }
00001940     	{ r0 = memb(r4+1); r0 = memw(r0) }
00001AC8     	{ r16 = memb(r20+2); r0 = memw(r0) }
00000770     	{ r0 = memw(r7+28); r0 = memw(r0) }
00000770     	{ r0 = memw(r7+28); r0 = memw(r0) }
00000770     	{ r0 = memw(r7+28); r0 = memw(r0) }
00000770     	{ r0 = memw(r7+28); r0 = memw(r0) }
00000770     	{ r0 = memw(r7+28); r0 = memw(r0) }
00000770     	{ r0 = memw(r7+28); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
03030200     	{ r0 = memw(r0+8); r3 = memw(r0+12) }
04040404     	{ r4 = memw(r0+16); r4 = memw(r0+16) }
04040404     	{ r4 = memw(r0+16); r4 = memw(r0+16) }
04040404     	{ r4 = memw(r0+16); r4 = memw(r0+16) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000000     	{ r0 = memw(r0); r0 = memw(r0) }
00000005     	{ r5 = memw(r0); r0 = memw(r0) }
00170007     	{ r7 = memw(r0); r7 = memw(r1) }
00370027     	{ r7 = memw(r2); r7 = memw(r3) }
00570047     	{ r7 = memw(r4); r7 = memw(r5) }
00770067     	{ r7 = memw(r6); r7 = memw(r7) }
00970087     	{ r7 = memw(r16); r7 = memw(r17) }
00B700A7     	{ r7 = memw(r18); r7 = memw(r19) }
00D700C7     	{ r7 = memw(r20); r7 = memw(r21) }
00F700E7     	{ r7 = memw(r22); r7 = memw(r23) }
01170107     	{ r7 = memw(r0+4); r7 = memw(r1+4) }
01370127     	{ r7 = memw(r2+4); r7 = memw(r3+4) }
01570147     	{ r7 = memw(r4+4); r7 = memw(r5+4) }
01770167     	{ r7 = memw(r6+4); r7 = memw(r7+4) }
01970187     	{ r7 = memw(r16+4); r7 = memw(r17+4) }
01B701A7     	{ r7 = memw(r18+4); r7 = memw(r19+4) }
01D701C7     	{ r7 = memw(r20+4); r7 = memw(r21+4) }
01F701E7     	{ r7 = memw(r22+4); r7 = memw(r23+4) }
02170207     	{ r7 = memw(r0+8); r7 = memw(r1+8) }
02370227     	{ r7 = memw(r2+8); r7 = memw(r3+8) }
02570247     	{ r7 = memw(r4+8); r7 = memw(r5+8) }
02770267     	{ r7 = memw(r6+8); r7 = memw(r7+8) }
02970287     	{ r7 = memw(r16+8); r7 = memw(r17+8) }
02B702A7     	{ r7 = memw(r18+8); r7 = memw(r19+8) }
02D702C7     	{ r7 = memw(r20+8); r7 = memw(r21+8) }
02F702E7     	{ r7 = memw(r22+8); r7 = memw(r23+8) }
03170307     	{ r7 = memw(r0+12); r7 = memw(r1+12) }
03370327     	{ r7 = memw(r2+12); r7 = memw(r3+12) }
03570347     	{ r7 = memw(r4+12); r7 = memw(r5+12) }
03770367     	{ r7 = memw(r6+12); r7 = memw(r7+12) }
03970387     	{ r7 = memw(r16+12); r7 = memw(r17+12) }
03B703A7     	{ r7 = memw(r18+12); r7 = memw(r19+12) }
03D703C7     	{ r7 = memw(r20+12); r7 = memw(r21+12) }
03F703E7     	{ r7 = memw(r22+12); r7 = memw(r23+12) }
04170407     	{ r7 = memw(r0+16); r7 = memw(r1+16) }
04370427     	{ r7 = memw(r2+16); r7 = memw(r3+16) }
04570447     	{ r7 = memw(r4+16); r7 = memw(r5+16) }
04770467     	{ r7 = memw(r6+16); r7 = memw(r7+16) }
04970487     	{ r7 = memw(r16+16); r7 = memw(r17+16) }
04B704A7     	{ r7 = memw(r18+16); r7 = memw(r19+16) }
04D704C7     	{ r7 = memw(r20+16); r7 = memw(r21+16) }
04F704E7     	{ r7 = memw(r22+16); r7 = memw(r23+16) }
05170507     	{ r7 = memw(r0+20); r7 = memw(r1+20) }
05370527     	{ r7 = memw(r2+20); r7 = memw(r3+20) }
05570547     	{ r7 = memw(r4+20); r7 = memw(r5+20) }
05770567     	{ r7 = memw(r6+20); r7 = memw(r7+20) }
05970587     	{ r7 = memw(r16+20); r7 = memw(r17+20) }
05B705A7     	{ r7 = memw(r18+20); r7 = memw(r19+20) }
05D705C7     	{ r7 = memw(r20+20); r7 = memw(r21+20) }
05F705E7     	{ r7 = memw(r22+20); r7 = memw(r23+20) }
06170607     	{ r7 = memw(r0+24); r7 = memw(r1+24) }
06370627     	{ r7 = memw(r2+24); r7 = memw(r3+24) }
06570647     	{ r7 = memw(r4+24); r7 = memw(r5+24) }
06770667     	{ r7 = memw(r6+24); r7 = memw(r7+24) }
06970687     	{ r7 = memw(r16+24); r7 = memw(r17+24) }
06B706A7     	{ r7 = memw(r18+24); r7 = memw(r19+24) }
06D706C7     	{ r7 = memw(r20+24); r7 = memw(r21+24) }
06F706E7     	{ r7 = memw(r22+24); r7 = memw(r23+24) }
07170707     	{ r7 = memw(r0+28); r7 = memw(r1+28) }
07370727     	{ r7 = memw(r2+28); r7 = memw(r3+28) }
07570747     	{ r7 = memw(r4+28); r7 = memw(r5+28) }
07770767     	{ r7 = memw(r6+28); r7 = memw(r7+28) }
07970787     	{ r7 = memw(r16+28); r7 = memw(r17+28) }
07B707A7     	{ r7 = memw(r18+28); r7 = memw(r19+28) }
07D707C7     	{ r7 = memw(r20+28); r7 = memw(r21+28) }
07F707E7     	{ r7 = memw(r22+28); r7 = memw(r23+28) }
08170807     	{ r7 = memw(r0); r7 = memw(r1) }
08370827     	{ r7 = memw(r2); r7 = memw(r3) }
08570847     	{ r7 = memw(r4); r7 = memw(r5) }
08770867     	{ r7 = memw(r6); r7 = memw(r7) }
08970887     	{ r7 = memw(r16); r7 = memw(r17) }
08B708A7     	{ r7 = memw(r18); r7 = memw(r19) }
08D708C7     	{ r7 = memw(r20); r7 = memw(r21) }
08F708E7     	{ r7 = memw(r22); r7 = memw(r23) }
09170907     	{ r7 = memw(r0+4); r7 = memw(r1+4) }
09370927     	{ r7 = memw(r2+4); r7 = memw(r3+4) }
09570947     	{ r7 = memw(r4+4); r7 = memw(r5+4) }
09770967     	{ r7 = memw(r6+4); r7 = memw(r7+4) }
09970987     	{ r7 = memw(r16+4); r7 = memw(r17+4) }
09B709A7     	{ r7 = memw(r18+4); r7 = memw(r19+4) }
09D709C7     	{ r7 = memw(r20+4); r7 = memw(r21+4) }
09F709E7     	{ r7 = memw(r22+4); r7 = memw(r23+4) }
0A170A07     	{ r7 = memw(r0+8); r7 = memw(r1+8) }
0A370A27     	{ r7 = memw(r2+8); r7 = memw(r3+8) }
0A570A47     	{ r7 = memw(r4+8); r7 = memw(r5+8) }
0A770A67     	{ r7 = memw(r6+8); r7 = memw(r7+8) }
0A970A87     	{ r7 = memw(r16+8); r7 = memw(r17+8) }
0AB70AA7     	{ r7 = memw(r18+8); r7 = memw(r19+8) }
0AD70AC7     	{ r7 = memw(r20+8); r7 = memw(r21+8) }
0AF70AE7     	{ r7 = memw(r22+8); r7 = memw(r23+8) }
0B170B07     	{ r7 = memw(r0+12); r7 = memw(r1+12) }
0B370B27     	{ r7 = memw(r2+12); r7 = memw(r3+12) }
0B570B47     	{ r7 = memw(r4+12); r7 = memw(r5+12) }
0B770B67     	{ r7 = memw(r6+12); r7 = memw(r7+12) }
0B970B87     	{ r7 = memw(r16+12); r7 = memw(r17+12) }
0BB70BA7     	{ r7 = memw(r18+12); r7 = memw(r19+12) }
0BD70BC7     	{ r7 = memw(r20+12); r7 = memw(r21+12) }
0BF70BE7     	{ r7 = memw(r22+12); r7 = memw(r23+12) }
0C170C07     	{ r7 = memw(r0+16); r7 = memw(r1+16) }
0C370C27     	{ r7 = memw(r2+16); r7 = memw(r3+16) }
0C570C47     	{ r7 = memw(r4+16); r7 = memw(r5+16) }
0C770C67     	{ r7 = memw(r6+16); r7 = memw(r7+16) }
0C970C87     	{ r7 = memw(r16+16); r7 = memw(r17+16) }
0CB70CA7     	{ r7 = memw(r18+16); r7 = memw(r19+16) }
0CD70CC7     	{ r7 = memw(r20+16); r7 = memw(r21+16) }
0CF70CE7     	{ r7 = memw(r22+16); r7 = memw(r23+16) }
0D170D07     	{ r7 = memw(r0+20); r7 = memw(r1+20) }
0D370D27     	{ r7 = memw(r2+20); r7 = memw(r3+20) }
0D570D47     	{ r7 = memw(r4+20); r7 = memw(r5+20) }
0D770D67     	{ r7 = memw(r6+20); r7 = memw(r7+20) }
0D970D87     	{ r7 = memw(r16+20); r7 = memw(r17+20) }
0DB70DA7     	{ r7 = memw(r18+20); r7 = memw(r19+20) }
0DD70DC7     	{ r7 = memw(r20+20); r7 = memw(r21+20) }
0DF70DE7     	{ r7 = memw(r22+20); r7 = memw(r23+20) }
0E170E07     	{ r7 = memw(r0+24); r7 = memw(r1+24) }
0E370E27     	{ r7 = memw(r2+24); r7 = memw(r3+24) }
0E570E47     	{ r7 = memw(r4+24); r7 = memw(r5+24) }
0E770E67     	{ r7 = memw(r6+24); r7 = memw(r7+24) }
0E970E87     	{ r7 = memw(r16+24); r7 = memw(r17+24) }
0EB70EA7     	{ r7 = memw(r18+24); r7 = memw(r19+24) }
0ED70EC7     	{ r7 = memw(r20+24); r7 = memw(r21+24) }
0EF70EE7     	{ r7 = memw(r22+24); r7 = memw(r23+24) }
0F170F07     	{ r7 = memw(r0+28); r7 = memw(r1+28) }
0F370F27     	{ r7 = memw(r2+28); r7 = memw(r3+28) }
0F570F47     	{ r7 = memw(r4+28); r7 = memw(r5+28) }
0F770F67     	{ r7 = memw(r6+28); r7 = memw(r7+28) }
0F970F87     	{ r7 = memw(r16+28); r7 = memw(r17+28) }
0FB70FA7     	{ r7 = memw(r18+28); r7 = memw(r19+28) }
0FD70FC7     	{ r7 = memw(r20+28); r7 = memw(r21+28) }
0FF70FE7     	{ r7 = memw(r22+28); r7 = memw(r23+28) }
10171007     	{ r7 = memb(r0); r7 = memb(r1) }
10371027     	{ r7 = memb(r2); r7 = memb(r3) }
10571047     	{ r7 = memb(r4); r7 = memb(r5) }
10771067     	{ r7 = memb(r6); r7 = memb(r7) }
10971087     	{ r7 = memb(r16); r7 = memb(r17) }
10B710A7     	{ r7 = memb(r18); r7 = memb(r19) }
10D710C7     	{ r7 = memb(r20); r7 = memb(r21) }
10F710E7     	{ r7 = memb(r22); r7 = memb(r23) }
11171107     	{ r7 = memb(r0+1); r7 = memb(r1+1) }
11371127     	{ r7 = memb(r2+1); r7 = memb(r3+1) }
11571147     	{ r7 = memb(r4+1); r7 = memb(r5+1) }
11771167     	{ r7 = memb(r6+1); r7 = memb(r7+1) }
11971187     	{ r7 = memb(r16+1); r7 = memb(r17+1) }
11B711A7     	{ r7 = memb(r18+1); r7 = memb(r19+1) }
11D711C7     	{ r7 = memb(r20+1); r7 = memb(r21+1) }
11F711E7     	{ r7 = memb(r22+1); r7 = memb(r23+1) }
12171207     	{ r7 = memb(r0+2); r7 = memb(r1+2) }
12371227     	{ r7 = memb(r2+2); r7 = memb(r3+2) }
12571247     	{ r7 = memb(r4+2); r7 = memb(r5+2) }
12771267     	{ r7 = memb(r6+2); r7 = memb(r7+2) }
12971287     	{ r7 = memb(r16+2); r7 = memb(r17+2) }
12B712A7     	{ r7 = memb(r18+2); r7 = memb(r19+2) }
12D712C7     	{ r7 = memb(r20+2); r7 = memb(r21+2) }
12F712E7     	{ r7 = memb(r22+2); r7 = memb(r23+2) }
13171307     	{ r7 = memb(r0+3); r7 = memb(r1+3) }
13371327     	{ r7 = memb(r2+3); r7 = memb(r3+3) }
13571347     	{ r7 = memb(r4+3); r7 = memb(r5+3) }
13771367     	{ r7 = memb(r6+3); r7 = memb(r7+3) }
13971387     	{ r7 = memb(r16+3); r7 = memb(r17+3) }
13B713A7     	{ r7 = memb(r18+3); r7 = memb(r19+3) }
13D713C7     	{ r7 = memb(r20+3); r7 = memb(r21+3) }
13F713E7     	{ r7 = memb(r22+3); r7 = memb(r23+3) }
14171407     	{ r7 = memb(r0+4); r7 = memb(r1+4) }
14371427     	{ r7 = memb(r2+4); r7 = memb(r3+4) }
14571447     	{ r7 = memb(r4+4); r7 = memb(r5+4) }
14771467     	{ r7 = memb(r6+4); r7 = memb(r7+4) }
14971487     	{ r7 = memb(r16+4); r7 = memb(r17+4) }
14B714A7     	{ r7 = memb(r18+4); r7 = memb(r19+4) }
14D714C7     	{ r7 = memb(r20+4); r7 = memb(r21+4) }
14F714E7     	{ r7 = memb(r22+4); r7 = memb(r23+4) }
15171507     	{ r7 = memb(r0+5); r7 = memb(r1+5) }
15371527     	{ r7 = memb(r2+5); r7 = memb(r3+5) }
15571547     	{ r7 = memb(r4+5); r7 = memb(r5+5) }
15771567     	{ r7 = memb(r6+5); r7 = memb(r7+5) }
15971587     	{ r7 = memb(r16+5); r7 = memb(r17+5) }
15B715A7     	{ r7 = memb(r18+5); r7 = memb(r19+5) }
15D715C7     	{ r7 = memb(r20+5); r7 = memb(r21+5) }
15F715E7     	{ r7 = memb(r22+5); r7 = memb(r23+5) }
16171607     	{ r7 = memb(r0+6); r7 = memb(r1+6) }
16371627     	{ r7 = memb(r2+6); r7 = memb(r3+6) }
16571647     	{ r7 = memb(r4+6); r7 = memb(r5+6) }
16771667     	{ r7 = memb(r6+6); r7 = memb(r7+6) }
16971687     	{ r7 = memb(r16+6); r7 = memb(r17+6) }
16B716A7     	{ r7 = memb(r18+6); r7 = memb(r19+6) }
16D716C7     	{ r7 = memb(r20+6); r7 = memb(r21+6) }
16F716E7     	{ r7 = memb(r22+6); r7 = memb(r23+6) }
17171707     	{ r7 = memb(r0+7); r7 = memb(r1+7) }
17371727     	{ r7 = memb(r2+7); r7 = memb(r3+7) }
17571747     	{ r7 = memb(r4+7); r7 = memb(r5+7) }
17771767     	{ r7 = memb(r6+7); r7 = memb(r7+7) }
17971787     	{ r7 = memb(r16+7); r7 = memb(r17+7) }
17B717A7     	{ r7 = memb(r18+7); r7 = memb(r19+7) }
17D717C7     	{ r7 = memb(r20+7); r7 = memb(r21+7) }
17F717E7     	{ r7 = memb(r22+7); r7 = memb(r23+7) }
18171807     	{ r7 = memb(r0); r7 = memb(r1) }
18371827     	{ r7 = memb(r2); r7 = memb(r3) }
18571847     	{ r7 = memb(r4); r7 = memb(r5) }
18771867     	{ r7 = memb(r6); r7 = memb(r7) }
18971887     	{ r7 = memb(r16); r7 = memb(r17) }
18B718A7     	{ r7 = memb(r18); r7 = memb(r19) }
18D718C7     	{ r7 = memb(r20); r7 = memb(r21) }
18F718E7     	{ r7 = memb(r22); r7 = memb(r23) }
19171907     	{ r7 = memb(r0+1); r7 = memb(r1+1) }
19371927     	{ r7 = memb(r2+1); r7 = memb(r3+1) }
19571947     	{ r7 = memb(r4+1); r7 = memb(r5+1) }
19771967     	{ r7 = memb(r6+1); r7 = memb(r7+1) }
19971987     	{ r7 = memb(r16+1); r7 = memb(r17+1) }
19B719A7     	{ r7 = memb(r18+1); r7 = memb(r19+1) }
19D719C7     	{ r7 = memb(r20+1); r7 = memb(r21+1) }
19F719E7     	{ r7 = memb(r22+1); r7 = memb(r23+1) }
1A171A07     	{ r7 = memb(r0+2); r7 = memb(r1+2) }
1A371A27     	{ r7 = memb(r2+2); r7 = memb(r3+2) }
1A571A47     	{ r7 = memb(r4+2); r7 = memb(r5+2) }
1A771A67     	{ r7 = memb(r6+2); r7 = memb(r7+2) }
1A971A87     	{ r7 = memb(r16+2); r7 = memb(r17+2) }
1AB71AA7     	{ r7 = memb(r18+2); r7 = memb(r19+2) }
1AD71AC7     	{ r7 = memb(r20+2); r7 = memb(r21+2) }
1AF71AE7     	{ r7 = memb(r22+2); r7 = memb(r23+2) }
1B171B07     	{ r7 = memb(r0+3); r7 = memb(r1+3) }
1B371B27     	{ r7 = memb(r2+3); r7 = memb(r3+3) }
1B571B47     	{ r7 = memb(r4+3); r7 = memb(r5+3) }
1B771B67     	{ r7 = memb(r6+3); r7 = memb(r7+3) }
1B971B87     	{ r7 = memb(r16+3); r7 = memb(r17+3) }
1BB71BA7     	{ r7 = memb(r18+3); r7 = memb(r19+3) }
1BD71BC7     	{ r7 = memb(r20+3); r7 = memb(r21+3) }
1BF71BE7     	{ r7 = memb(r22+3); r7 = memb(r23+3) }
1C171C07     	{ r7 = memb(r0+4); r7 = memb(r1+4) }
1C371C27     	{ r7 = memb(r2+4); r7 = memb(r3+4) }
1C571C47     	{ r7 = memb(r4+4); r7 = memb(r5+4) }
1C771C67     	{ r7 = memb(r6+4); r7 = memb(r7+4) }
1C971C87     	{ r7 = memb(r16+4); r7 = memb(r17+4) }
1CB71CA7     	{ r7 = memb(r18+4); r7 = memb(r19+4) }
1CD71CC7     	{ r7 = memb(r20+4); r7 = memb(r21+4) }
1CF71CE7     	{ r7 = memb(r22+4); r7 = memb(r23+4) }
1D171D07     	{ r7 = memb(r0+5); r7 = memb(r1+5) }
1D371D27     	{ r7 = memb(r2+5); r7 = memb(r3+5) }
1D571D47     	{ r7 = memb(r4+5); r7 = memb(r5+5) }
1D771D67     	{ r7 = memb(r6+5); r7 = memb(r7+5) }
1D971D87     	{ r7 = memb(r16+5); r7 = memb(r17+5) }
1DB71DA7     	{ r7 = memb(r18+5); r7 = memb(r19+5) }
1DD71DC7     	{ r7 = memb(r20+5); r7 = memb(r21+5) }
1DF71DE7     	{ r7 = memb(r22+5); r7 = memb(r23+5) }
1E171E07     	{ r7 = memb(r0+6); r7 = memb(r1+6) }
1E371E27     	{ r7 = memb(r2+6); r7 = memb(r3+6) }
1E571E47     	{ r7 = memb(r4+6); r7 = memb(r5+6) }
1E771E67     	{ r7 = memb(r6+6); r7 = memb(r7+6) }
1E971E87     	{ r7 = memb(r16+6); r7 = memb(r17+6) }
1EB71EA7     	{ r7 = memb(r18+6); r7 = memb(r19+6) }
1ED71EC7     	{ r7 = memb(r20+6); r7 = memb(r21+6) }
1EF71EE7     	{ r7 = memb(r22+6); r7 = memb(r23+6) }
1F171F07     	{ r7 = memb(r0+7); r7 = memb(r1+7) }
1F371F27     	{ r7 = memb(r2+7); r7 = memb(r3+7) }
1F571F47     	{ r7 = memb(r4+7); r7 = memb(r5+7) }
1F771F67     	{ r7 = memb(r6+7); r7 = memb(r7+7) }
1F971F87     	{ r7 = memb(r16+7); r7 = memb(r17+7) }
1FB71FA7     	{ r7 = memb(r18+7); r7 = memb(r19+7) }
1FD71FC7     	{ r7 = memb(r20+7); r7 = memb(r21+7) }
1FF71FE7     	{ r7 = memb(r22+7); r7 = memb(r23+7) }
20172007     	{ r7 = add(r7,00000000); r7 = add(r7,00000001) }
20372027     	{ r7 = add(r7,00000002); r7 = add(r7,00000003) }
20572047     	{ r7 = add(r7,00000004); r7 = add(r7,00000005) }
20772067     	{ r7 = add(r7,00000006); r7 = add(r7,00000007) }
20972087     	{ r7 = add(r7,00000008); r7 = add(r7,00000009) }
20B720A7     	{ r7 = add(r7,0000000A); r7 = add(r7,0000000B) }
20D720C7     	{ r7 = add(r7,0000000C); r7 = add(r7,0000000D) }
20F720E7     	{ r7 = add(r7,0000000E); r7 = add(r7,0000000F) }
21172107     	{ r7 = add(r7,00000010); r7 = add(r7,00000011) }
21372127     	{ r7 = add(r7,00000012); r7 = add(r7,00000013) }
21572147     	{ r7 = add(r7,00000014); r7 = add(r7,00000015) }
21772167     	{ r7 = add(r7,00000016); r7 = add(r7,00000017) }
21972187     	{ r7 = add(r7,00000018); r7 = add(r7,00000019) }
21B721A7     	{ r7 = add(r7,0000001A); r7 = add(r7,0000001B) }
21D721C7     	{ r7 = add(r7,0000001C); r7 = add(r7,0000001D) }
21F721E7     	{ r7 = add(r7,0000001E); r7 = add(r7,0000001F) }
22172207     	{ r7 = add(r7,00000020); r7 = add(r7,00000021) }
22372227     	{ r7 = add(r7,00000022); r7 = add(r7,00000023) }
22572247     	{ r7 = add(r7,00000024); r7 = add(r7,00000025) }
22772267     	{ r7 = add(r7,00000026); r7 = add(r7,00000027) }
22972287     	{ r7 = add(r7,00000028); r7 = add(r7,00000029) }
22B722A7     	{ r7 = add(r7,0000002A); r7 = add(r7,0000002B) }
22D722C7     	{ r7 = add(r7,0000002C); r7 = add(r7,0000002D) }
22F722E7     	{ r7 = add(r7,0000002E); r7 = add(r7,0000002F) }
23172307     	{ r7 = add(r7,00000030); r7 = add(r7,00000031) }
23372327     	{ r7 = add(r7,00000032); r7 = add(r7,00000033) }
23572347     	{ r7 = add(r7,00000034); r7 = add(r7,00000035) }
23772367     	{ r7 = add(r7,00000036); r7 = add(r7,00000037) }
23972387     	{ r7 = add(r7,00000038); r7 = add(r7,00000039) }
23B723A7     	{ r7 = add(r7,0000003A); r7 = add(r7,0000003B) }
23D723C7     	{ r7 = add(r7,0000003C); r7 = add(r7,0000003D) }
23F723E7     	{ r7 = add(r7,0000003E); r7 = add(r7,0000003F) }
24172407     	{ r7 = add(r7,FFFFFFC0); r7 = add(r7,FFFFFFC1) }
24372427     	{ r7 = add(r7,FFFFFFC2); r7 = add(r7,FFFFFFC3) }
24572447     	{ r7 = add(r7,FFFFFFC4); r7 = add(r7,FFFFFFC5) }
24772467     	{ r7 = add(r7,FFFFFFC6); r7 = add(r7,FFFFFFC7) }
24972487     	{ r7 = add(r7,FFFFFFC8); r7 = add(r7,FFFFFFC9) }
24B724A7     	{ r7 = add(r7,FFFFFFCA); r7 = add(r7,FFFFFFCB) }
24D724C7     	{ r7 = add(r7,FFFFFFCC); r7 = add(r7,FFFFFFCD) }
24F724E7     	{ r7 = add(r7,FFFFFFCE); r7 = add(r7,FFFFFFCF) }
25172507     	{ r7 = add(r7,FFFFFFD0); r7 = add(r7,FFFFFFD1) }
25372527     	{ r7 = add(r7,FFFFFFD2); r7 = add(r7,FFFFFFD3) }
25572547     	{ r7 = add(r7,FFFFFFD4); r7 = add(r7,FFFFFFD5) }
25772567     	{ r7 = add(r7,FFFFFFD6); r7 = add(r7,FFFFFFD7) }
25972587     	{ r7 = add(r7,FFFFFFD8); r7 = add(r7,FFFFFFD9) }
25B725A7     	{ r7 = add(r7,FFFFFFDA); r7 = add(r7,FFFFFFDB) }
25D725C7     	{ r7 = add(r7,FFFFFFDC); r7 = add(r7,FFFFFFDD) }
25F725E7     	{ r7 = add(r7,FFFFFFDE); r7 = add(r7,FFFFFFDF) }
26172607     	{ r7 = add(r7,FFFFFFE0); r7 = add(r7,FFFFFFE1) }
26372627     	{ r7 = add(r7,FFFFFFE2); r7 = add(r7,FFFFFFE3) }
26572647     	{ r7 = add(r7,FFFFFFE4); r7 = add(r7,FFFFFFE5) }
26772667     	{ r7 = add(r7,FFFFFFE6); r7 = add(r7,FFFFFFE7) }
26972687     	{ r7 = add(r7,FFFFFFE8); r7 = add(r7,FFFFFFE9) }
26B726A7     	{ r7 = add(r7,FFFFFFEA); r7 = add(r7,FFFFFFEB) }
26D726C7     	{ r7 = add(r7,FFFFFFEC); r7 = add(r7,FFFFFFED) }
26F726E7     	{ r7 = add(r7,FFFFFFEE); r7 = add(r7,FFFFFFEF) }
27172707     	{ r7 = add(r7,FFFFFFF0); r7 = add(r7,FFFFFFF1) }
27372727     	{ r7 = add(r7,FFFFFFF2); r7 = add(r7,FFFFFFF3) }
27572747     	{ r7 = add(r7,FFFFFFF4); r7 = add(r7,FFFFFFF5) }
27772767     	{ r7 = add(r7,FFFFFFF6); r7 = add(r7,FFFFFFF7) }
27972787     	{ r7 = add(r7,FFFFFFF8); r7 = add(r7,FFFFFFF9) }
27B727A7     	{ r7 = add(r7,FFFFFFFA); r7 = add(r7,FFFFFFFB) }
27D727C7     	{ r7 = add(r7,FFFFFFFC); r7 = add(r7,FFFFFFFD) }
27F727E7     	{ r7 = add(r7,FFFFFFFE); r7 = add(r7,FFFFFFFF) }
28172807     	{ r7 = 00000000; r7 = 00000001 }
28372827     	{ r7 = 00000002; r7 = 00000003 }
28572847     	{ r7 = 00000004; r7 = 00000005 }
28772867     	{ r7 = 00000006; r7 = 00000007 }
28972887     	{ r7 = 00000008; r7 = 00000009 }
28B728A7     	{ r7 = 0000000A; r7 = 0000000B }
28D728C7     	{ r7 = 0000000C; r7 = 0000000D }
28F728E7     	{ r7 = 0000000E; r7 = 0000000F }
29172907     	{ r7 = 00000010; r7 = 00000011 }
29372927     	{ r7 = 00000012; r7 = 00000013 }
29572947     	{ r7 = 00000014; r7 = 00000015 }
29772967     	{ r7 = 00000016; r7 = 00000017 }
29972987     	{ r7 = 00000018; r7 = 00000019 }
29B729A7     	{ r7 = 0000001A; r7 = 0000001B }
29D729C7     	{ r7 = 0000001C; r7 = 0000001D }
29F729E7     	{ r7 = 0000001E; r7 = 0000001F }
2A172A07     	{ r7 = 00000020; r7 = 00000021 }
2A372A27     	{ r7 = 00000022; r7 = 00000023 }
2A572A47     	{ r7 = 00000024; r7 = 00000025 }
2A772A67     	{ r7 = 00000026; r7 = 00000027 }
2A972A87     	{ r7 = 00000028; r7 = 00000029 }
2AB72AA7     	{ r7 = 0000002A; r7 = 0000002B }
2AD72AC7     	{ r7 = 0000002C; r7 = 0000002D }
2AF72AE7     	{ r7 = 0000002E; r7 = 0000002F }
2B172B07     	{ r7 = 00000030; r7 = 00000031 }
2B372B27     	{ r7 = 00000032; r7 = 00000033 }
2B572B47     	{ r7 = 00000034; r7 = 00000035 }
2B772B67     	{ r7 = 00000036; r7 = 00000037 }
2B972B87     	{ r7 = 00000038; r7 = 00000039 }
2BB72BA7     	{ r7 = 0000003A; r7 = 0000003B }
2BD72BC7     	{ r7 = 0000003C; r7 = 0000003D }
2BF72BE7     	{ r7 = 0000003E; r7 = 0000003F }
2C172C07     	{ r7 = add(r29,00000000); r7 = add(r29,00000004) }
2C372C27     	{ r7 = add(r29,00000008); r7 = add(r29,0000000C) }
2C572C47     	{ r7 = add(r29,00000010); r7 = add(r29,00000014) }
2C772C67     	{ r7 = add(r29,00000018); r7 = add(r29,0000001C) }
2C972C87     	{ r7 = add(r29,00000020); r7 = add(r29,00000024) }
2CB72CA7     	{ r7 = add(r29,00000028); r7 = add(r29,0000002C) }
2CD72CC7     	{ r7 = add(r29,00000030); r7 = add(r29,00000034) }
2CF72CE7     	{ r7 = add(r29,00000038); r7 = add(r29,0000003C) }
2D172D07     	{ r7 = add(r29,00000040); r7 = add(r29,00000044) }
2D372D27     	{ r7 = add(r29,00000048); r7 = add(r29,0000004C) }
2D572D47     	{ r7 = add(r29,00000050); r7 = add(r29,00000054) }
2D772D67     	{ r7 = add(r29,00000058); r7 = add(r29,0000005C) }
2D972D87     	{ r7 = add(r29,00000060); r7 = add(r29,00000064) }
2DB72DA7     	{ r7 = add(r29,00000068); r7 = add(r29,0000006C) }
2DD72DC7     	{ r7 = add(r29,00000070); r7 = add(r29,00000074) }
2DF72DE7     	{ r7 = add(r29,00000078); r7 = add(r29,0000007C) }
2E172E07     	{ r7 = add(r29,00000080); r7 = add(r29,00000084) }
2E372E27     	{ r7 = add(r29,00000088); r7 = add(r29,0000008C) }
2E572E47     	{ r7 = add(r29,00000090); r7 = add(r29,00000094) }
2E772E67     	{ r7 = add(r29,00000098); r7 = add(r29,0000009C) }
2E972E87     	{ r7 = add(r29,000000A0); r7 = add(r29,000000A4) }
2EB72EA7     	{ r7 = add(r29,000000A8); r7 = add(r29,000000AC) }
2ED72EC7     	{ r7 = add(r29,000000B0); r7 = add(r29,000000B4) }
2EF72EE7     	{ r7 = add(r29,000000B8); r7 = add(r29,000000BC) }
2F172F07     	{ r7 = add(r29,000000C0); r7 = add(r29,000000C4) }
2F372F27     	{ r7 = add(r29,000000C8); r7 = add(r29,000000CC) }
2F572F47     	{ r7 = add(r29,000000D0); r7 = add(r29,000000D4) }
2F772F67     	{ r7 = add(r29,000000D8); r7 = add(r29,000000DC) }
2F972F87     	{ r7 = add(r29,000000E0); r7 = add(r29,000000E4) }
2FB72FA7     	{ r7 = add(r29,000000E8); r7 = add(r29,000000EC) }
2FD72FC7     	{ r7 = add(r29,000000F0); r7 = add(r29,000000F4) }
2FF72FE7     	{ r7 = add(r29,000000F8); r7 = add(r29,000000FC) }
30173007     	{ r7 = r0; r7 = r1 }
30373027     	{ r7 = r2; r7 = r3 }
30573047     	{ r7 = r4; r7 = r5 }
30773067     	{ r7 = r6; r7 = r7 }
30973087     	{ r7 = r16; r7 = r17 }
30B730A7     	{ r7 = r18; r7 = r19 }
30D730C7     	{ r7 = r20; r7 = r21 }
30F730E7     	{ r7 = r22; r7 = r23 }
31173107     	{ r7 = r0; r7 = r1 }
31373127     	{ r7 = r2; r7 = r3 }
31573147     	{ r7 = r4; r7 = r5 }
31773167     	{ r7 = r6; r7 = r7 }
31973187     	{ r7 = r16; r7 = r17 }
31B731A7     	{ r7 = r18; r7 = r19 }
31D731C7     	{ r7 = r20; r7 = r21 }
31F731E7     	{ r7 = r22; r7 = r23 }
32173207     	{ r7 = r0; r7 = r1 }
32373227     	{ r7 = r2; r7 = r3 }
32573247     	{ r7 = r4; r7 = r5 }
32773267     	{ r7 = r6; r7 = r7 }
32973287     	{ r7 = r16; r7 = r17 }
32B732A7     	{ r7 = r18; r7 = r19 }
32D732C7     	{ r7 = r20; r7 = r21 }
32F732E7     	{ r7 = r22; r7 = r23 }
33173307     	{ r7 = r0; r7 = r1 }
33373327     	{ r7 = r2; r7 = r3 }
33573347     	{ r7 = r4; r7 = r5 }
33773367     	{ r7 = r6; r7 = r7 }
33973387     	{ r7 = r16; r7 = r17 }
33B733A7     	{ r7 = r18; r7 = r19 }
33D733C7     	{ r7 = r20; r7 = r21 }
33F733E7     	{ r7 = r22; r7 = r23 }
34173407     	{ r7 = sxth(r0); r7 = sxth(r1) }
34373427     	{ r7 = sxth(r2); r7 = sxth(r3) }
34573447     	{ r7 = sxth(r4); r7 = sxth(r5) }
34773467     	{ r7 = sxth(r6); r7 = sxth(r7) }
34973487     	{ r7 = sxth(r16); r7 = sxth(r17) }
34B734A7     	{ r7 = sxth(r18); r7 = sxth(r19) }
34D734C7     	{ r7 = sxth(r20); r7 = sxth(r21) }
34F734E7     	{ r7 = sxth(r22); r7 = sxth(r23) }
35173507     	{ r7 = sxtb(r0); r7 = sxtb(r1) }
35373527     	{ r7 = sxtb(r2); r7 = sxtb(r3) }
35573547     	{ r7 = sxtb(r4); r7 = sxtb(r5) }
35773567     	{ r7 = sxtb(r6); r7 = sxtb(r7) }
35973587     	{ r7 = sxtb(r16); r7 = sxtb(r17) }
35B735A7     	{ r7 = sxtb(r18); r7 = sxtb(r19) }
35D735C7     	{ r7 = sxtb(r20); r7 = sxtb(r21) }
35F735E7     	{ r7 = sxtb(r22); r7 = sxtb(r23) }
36173607     	{ r7 = zxth(r0); r7 = zxth(r1) }
36373627     	{ r7 = zxth(r2); r7 = zxth(r3) }
36573647     	{ r7 = zxth(r4); r7 = zxth(r5) }
36773667     	{ r7 = zxth(r6); r7 = zxth(r7) }
36973687     	{ r7 = zxth(r16); r7 = zxth(r17) }
36B736A7     	{ r7 = zxth(r18); r7 = zxth(r19) }
36D736C7     	{ r7 = zxth(r20); r7 = zxth(r21) }
36F736E7     	{ r7 = zxth(r22); r7 = zxth(r23) }
37173707     	{ r7 = and(r0,000000FF); r7 = and(r1,000000FF) }
37373727     	{ r7 = and(r2,000000FF); r7 = and(r3,000000FF) }
37573747     	{ r7 = and(r4,000000FF); r7 = and(r5,000000FF) }
37773767     	{ r7 = and(r6,000000FF); r7 = and(r7,000000FF) }
37973787     	{ r7 = and(r16,000000FF); r7 = and(r17,000000FF) }
37B737A7     	{ r7 = and(r18,000000FF); r7 = and(r19,000000FF) }
37D737C7     	{ r7 = and(r20,000000FF); r7 = and(r21,000000FF) }
37F737E7     	{ r7 = and(r22,000000FF); r7 = and(r23,000000FF) }
38173807     	{ r7 = add(r7,r0); r7 = add(r7,r1) }
38373827     	{ r7 = add(r7,r2); r7 = add(r7,r3) }
38573847     	{ r7 = add(r7,r4); r7 = add(r7,r5) }
38773867     	{ r7 = add(r7,r6); r7 = add(r7,r7) }
38973887     	{ r7 = add(r7,r16); r7 = add(r7,r17) }
38B738A7     	{ r7 = add(r7,r18); r7 = add(r7,r19) }
38D738C7     	{ r7 = add(r7,r20); r7 = add(r7,r21) }
38F738E7     	{ r7 = add(r7,r22); r7 = add(r7,r23) }
39173907     	{ p0 = cmp.eq(r0,00000003); p0 = cmp.eq(r1,00000003) }
39373927     	{ p0 = cmp.eq(r2,00000003); p0 = cmp.eq(r3,00000003) }
39573947     	{ p0 = cmp.eq(r4,00000003); p0 = cmp.eq(r5,00000003) }
39773967     	{ p0 = cmp.eq(r6,00000003); p0 = cmp.eq(r7,00000003) }
39973987     	{ p0 = cmp.eq(r16,00000003); p0 = cmp.eq(r17,00000003) }
39B739A7     	{ p0 = cmp.eq(r18,00000003); p0 = cmp.eq(r19,00000003) }
39D739C7     	{ p0 = cmp.eq(r20,00000003); p0 = cmp.eq(r21,00000003) }
39F739E7     	{ p0 = cmp.eq(r22,00000003); p0 = cmp.eq(r23,00000003) }
3A173A07     	{ r7 = -00000001; r7 = -00000001 }
3A373A27     	{ r7 = -00000001; r7 = -00000001 }
3A573A47     	{ r7 = -00000001; r7 = -00000001 }
3A773A67     	{ r7 = -00000001; r7 = -00000001 }
3A973A87     	{ r7 = -00000001; r7 = -00000001 }
3AB73AA7     	{ r7 = -00000001; r7 = -00000001 }
3AD73AC7     	{ r7 = -00000001; r7 = -00000001 }
3AF73AE7     	{ r7 = -00000001; r7 = -00000001 }
3B173B07     	{ r7 = -00000001; r7 = -00000001 }
3B373B27     	{ r7 = -00000001; r7 = -00000001 }
3B573B47     	{ r7 = -00000001; r7 = -00000001 }
3B773B67     	{ r7 = -00000001; r7 = -00000001 }
3B973B87     	{ r7 = -00000001; r7 = -00000001 }
3BB73BA7     	{ r7 = -00000001; r7 = -00000001 }
3BD73BC7     	{ r7 = -00000001; r7 = -00000001 }
3BF73BE7     	{ r7 = -00000001; r7 = -00000001 }
3C173C07     	{ r23:r22 = combine(00000000,00000000); r23:r22 = combine(00000002,00000000) }
3C373C27     	{ r23:r22 = combine(00000000,00000001); r23:r22 = combine(00000002,00000001) }
3C573C47     	{ r23:r22 = combine(00000000,00000002); r23:r22 = combine(00000002,00000002) }
3C773C67     	{ r23:r22 = combine(00000000,00000003); r23:r22 = combine(00000002,00000003) }
3C973C87     	{ r23:r22 = combine(00000000,00000000); r23:r22 = combine(00000002,00000000) }
3CB73CA7     	{ r23:r22 = combine(00000000,00000001); r23:r22 = combine(00000002,00000001) }
3CD73CC7     	{ r23:r22 = combine(00000000,00000002); r23:r22 = combine(00000002,00000002) }
3CF73CE7     	{ r23:r22 = combine(00000000,00000003); r23:r22 = combine(00000002,00000003) }
3D173D07     	{ r23:r22 = combine(00000000,00000000); r23:r22 = combine(00000000,00000000) }
3D373D27     	{ r23:r22 = combine(00000000,00000001); r23:r22 = combine(00000000,00000001) }
3D573D47     	{ r23:r22 = combine(00000000,00000002); r23:r22 = combine(00000000,00000002) }
3D773D67     	{ r23:r22 = combine(00000000,00000003); r23:r22 = combine(00000000,00000003) }
3D973D87     	{ r23:r22 = combine(00000000,00000000); r23:r22 = combine(00000000,00000000) }
3DB73DA7     	{ r23:r22 = combine(00000000,00000001); r23:r22 = combine(00000000,00000001) }
3DD73DC7     	{ r23:r22 = combine(00000000,00000002); r23:r22 = combine(00000000,00000002) }
3DF73DE7     	{ r23:r22 = combine(00000000,00000003); r23:r22 = combine(00000000,00000003) }
3E173E07     	{ r23:r22 = combine(00000000,00000000); r23:r22 = combine(00000002,00000000) }
3E373E27     	{ r23:r22 = combine(00000000,00000001); r23:r22 = combine(00000002,00000001) }
3E573E47     	{ r23:r22 = combine(00000000,00000002); r23:r22 = combine(00000002,00000002) }
3E773E67     	{ r23:r22 = combine(00000000,00000003); r23:r22 = combine(00000002,00000003) }
3E973E87     	{ r23:r22 = combine(00000000,00000000); r23:r22 = combine(00000002,00000000) }
3EB73EA7     	{ r23:r22 = combine(00000000,00000001); r23:r22 = combine(00000002,00000001) }
3ED73EC7     	{ r23:r22 = combine(00000000,00000002); r23:r22 = combine(00000002,00000002) }
3EF73EE7     	{ r23:r22 = combine(00000000,00000003); r23:r22 = combine(00000002,00000003) }
3F173F07     	{ r23:r22 = combine(00000000,00000000); r23:r22 = combine(00000000,00000000) }
3F373F27     	{ r23:r22 = combine(00000000,00000001); r23:r22 = combine(00000000,00000001) }
3F573F47     	{ r23:r22 = combine(00000000,00000002); r23:r22 = combine(00000000,00000002) }
3F773F67     	{ r23:r22 = combine(00000000,00000003); r23:r22 = combine(00000000,00000003) }
3F973F87     	{ r23:r22 = combine(00000000,00000000); r23:r22 = combine(00000000,00000000) }
3FB73FA7     	{ r23:r22 = combine(00000000,00000001); r23:r22 = combine(00000000,00000001) }
3FD73FC7     	{ r23:r22 = combine(00000000,00000002); r23:r22 = combine(00000000,00000002) }
3FF73FE7     	{ r23:r22 = combine(00000000,00000003); r23:r22 = combine(00000000,00000003) }
40174007 40374027 	{ if (p3) memb(r23) = r0 }
40574047 40774067 40974087 40B740A7 	{ if (p3) memw(r23+64) = r0; if (p3) memuh(r23+24) = r0.h; if (p3) memuh(r23+16) = r0 }
40D740C7 40F740E7 	{ if (p3) memd(r23+192) = r1:r0 }
41174107 41374127 41574147 41774167 41974187 41B741A7 	{ if (p0) r7 = memw(r23+48); if (p0) r7 = memuh(r23+22); if (p0) r7 = memh(r23+20); if (p0) r7 = memb(r23+9); if (p0) r7 = memb(r23+8) }
41D741C7     	{  }
41F741E7     	{  }
42174207 42374227 	{ if (p3.new) memb(r23) = r2 }
42574247 42774267 42974287 42B742A7 42D742C7 	{ if (p3.new) memw(r23+64) = r2; if (p3.new) memuh(r23+24) = r2.h; if (p3.new) memuh(r23+16) = r2 }
42F742E7     	{  }
43174307 43374327 43574347 43774367 43974387 43B743A7 	{ if (p0.new) r7 = memw(r23+112); if (p0.new) r7 = memuh(r23+54); if (p0.new) r7 = memh(r23+52); if (p0.new) r7 = memb(r23+25); if (p0.new) r7 = memb(r23+24) }
43D743C7     	{  }
43F743E7     	{  }
44174407 44374427 44574447 44774467 44974487 44B744A7 	{ if (!p3) memw(r23+48) = r4.h; if (!p3) memuh(r23+16) = r4.h; if (!p3) memuh(r23+8) = r4; if (!p3) memb(r23) = r4 }
44D744C7     	{  }
44F744E7     	{  }
45174507 45374527 45574547 45774567 45974587 45B745A7 	{ if (!p0) r7 = memw(r23-80); if (!p0) r7 = memuh(r23-42); if (!p0) r7 = memh(r23-44); if (!p0) r7 = memb(r23-23); if (!p0) r7 = memb(r23-24) }
45D745C7     	{  }
45F745E7     	{  }
46174607 46374627 	{ if (p3.new) memb(r23) = r6 }
46574647 46774667 46974687 46B746A7 46D746C7 	{ if (p3.new) memw(r23+64) = r6; if (p3.new) memuh(r23+24) = r6.h; if (p3.new) memuh(r23+16) = r6 }
46F746E7     	{  }
47174707 47374727 47574747 47774767 47974787 47B747A7 	{ if (!p0.new) r7 = memw(r23-16); if (!p0.new) r7 = memuh(r23-10); if (!p0.new) r7 = memh(r23-12); if (!p0.new) r7 = memb(r23-7); if (!p0.new) r7 = memb(r23-8) }
47D747C7     	{  }
47F747E7     	{  }
48174807 48374827 	{ memb(gp+47360) = r8 }
48574847 48774867 48974887 48B748A7 	{ memw(gp+47376) = r8; memuh(gp+47372) = r8.h; memuh(gp+47368) = r8 }
48D748C7 48F748E7 	{ memd(gp+47384) = r9:r8 }
49174907 49374927 49574947 49774967 49974987 49B749A7 	{ r7 = memw(gp+47408); r7 = memuh(gp+23702); r7 = memh(gp+23700); r7 = memb(gp+11849); r7 = memb(gp+11848) }
49D749C7     	{  }
49F749E7     	{  }
4A174A07 4A374A27 	{ memb(gp+112960) = r10 }
4A574A47 4A774A67 4A974A87 4AB74AA7 4AD74AC7 	{ memw(gp+112976) = r10; memuh(gp+112972) = r10.h; memuh(gp+112968) = r10 }
4AF74AE7     	{  }
4B174B07 4B374B27 4B574B47 4B774B67 4B974B87 4BB74BA7 	{ r7 = memw(gp+113008); r7 = memuh(gp+56502); r7 = memh(gp+56500); r7 = memb(gp+28249); r7 = memb(gp+28248) }
4BD74BC7     	{  }
4BF74BE7     	{  }
4C174C07 4C374C27 	{ memb(gp+178560) = r12 }
4C574C47 4C774C67 4C974C87 4CB74CA7 4CD74CC7 	{ memw(gp+178576) = r12; memuh(gp+178572) = r12.h; memuh(gp+178568) = r12 }
4CF74CE7     	{  }
4D174D07 4D374D27 4D574D47 4D774D67 4D974D87 4DB74DA7 	{ r7 = memw(gp+178608); r7 = memuh(gp+89302); r7 = memh(gp+89300); r7 = memb(gp+44649); r7 = memb(gp+44648) }
4DD74DC7     	{  }
4DF74DE7     	{  }
4E174E07 4E374E27 	{ memb(gp+244160) = r14 }
4E574E47 4E774E67 4E974E87 4EB74EA7 4ED74EC7 	{ memw(gp+244176) = r14; memuh(gp+244172) = r14.h; memuh(gp+244168) = r14 }
4EF74EE7     	{  }
4F174F07 4F374F27 4F574F47 4F774F67 4F974F87 4FB74FA7 	{ r7 = memw(gp+244208); r7 = memuh(gp+122102); r7 = memh(gp+122100); r7 = memb(gp+61049); r7 = memb(gp+61048) }
4FD74FC7     	{  }
4FF74FE7     	{  }
50175007     	{  }
50375027     	{  }
50575047     	{  }
50775067     	{  }
50975087     	{  }
50B750A7 50D750C7 	{ callr	r23 }
50F750E7     	{  }
51175107 51375127 51575147 	{ if (!p1) callr	r23; if (p1) callr	r23 }
51775167     	{  }
51975187     	{  }
51B751A7     	{  }
51D751C7     	{  }
51F751E7     	{  }
52175207     	{  }
52375227     	{  }
52575247     	{  }
52775267     	{  }
52975287 52B752A7 52D752C7 	{ hintjr(r23); jumpr	r23 }
52F752E7     	{  }
53175307     	{  }
53375327     	{  }
53575347 53775367 53975387 	{ if (!p3) jumpr:t	r23; if (p3) jumpr:t	r23 }
53B753A7     	{  }
53D753C7     	{  }
53F753E7     	{  }
54175407 54375427 54575447 54775467 54975487 54B754A7 54D754C7 	{ trap1(000000A1); trap1(000000A1); pause(000000A1); pause(000000A1); trap0(000000A1); trap0(000000A1) }
54F754E7     	{  }
55175507     	{  }
55375527     	{  }
55575547     	{  }
55775567     	{  }
55975587     	{  }
55B755A7 55D755C7 55F755E7 56175607 	{ r7 = icdtagr(r23); icdtagw(r23,r7); r7 = icdatar(r23) }
56375627     	{  }
56575647     	{  }
56775667     	{  }
56975687     	{  }
56B756A7     	{  }
56D756C7 56F756E7 	{ ickill }
57175707     	{  }
57375727     	{  }
57575747     	{  }
57775767     	{  }
57975787     	{  }
57B757A7     	{  }
57D757C7 57F757E7 58175807 58375827 58575847 58775867 58975887 58B758A7 58D758C7 58F758E7 59175907 59375927 59575947 59775967 59975987 59B759A7 59D759C7 59F759E7 5A175A07 5A375A27 5A575A47 5A775A67 5A975A87 5AB75AA7 5AD75AC7 5AF75AE7 5B175B07 5B375B27 5B575B47 5B775B67 5B975B87 5BB75BA7 5BD75BC7 5BF75BE7 5C175C07 5C375C27 5C575C47 5C775C67 5C975C87 5CB75CA7 5CD75CC7 5CF75CE7 5D175D07 5D375D27 5D575D47 5D775D67 5D975D87 5DB75DA7 5DD75DC7 5DF75DE7 5E175E07 	{ if (p1) call	00000510; if (p1) call	000004CC; if (p1) call	FFFF8488; if (p1) call	FFFF8444; if (p1) call	00010400; if (p1) call	000103BC; if (p1) call	00008378; if (p1) call	00008334; if (!p0.new) jump:t	000004F0; if (p0.new) jump:t	000004AC; if (!p0.new) jump:t	FFFF8468; if (p0.new) jump:t	FFFF8424; if (!p0.new) jump:t	000103E0; if (p0.new) jump:t	0001039C; if (!p0.new) jump:t	00008358; if (p0.new) jump:t	00008314; call	FFFBDED0; call	FFEBDE8C; call	FFDBDE48; call	FFCBDE04; call	FFBBDDC0; call	FFABDD7C; call	FF9BDD38; call	FF8BDCF4; call	007BDCB0; call	006BDC6C; call	005BDC28; call	004BDBE4; call	003BDBA0; call	002BDB5C; call	001BDB18; call	000BDAD4; jump	FFFBDA90; jump	FFEBDA4C; jump	FFDBDA08; jump	FFCBD9C4; jump	FFBBD980; jump	FFABD93C; jump	FF9BD8F8; jump	FF8BD8B4; jump	007BD870; jump	006BD82C; jump	005BD7E8; jump	004BD7A4; jump	003BD760; jump	002BD71C; jump	001BD6D8; jump	000BD694; rteunlock; isync }
5E375E27     	{  }
5E575E47     	{  }
5E775E67     	{  }
5E975E87     	{  }
5EB75EA7     	{  }
5ED75EC7     	{  }
5EF75EE7     	{  }
5F175F07     	{  }
5F375F27     	{  }
5F575F47     	{  }
5F775F67     	{  }
5F975F87     	{  }
5FB75FA7     	{  }
5FD75FC7     	{  }
5FF75FE7     	{  }
60176007 60376027 60576047 	{ loop1(0000278C,r23); loop0(00002788,r23) }
60776067     	{  }
60976087 60B760A7 60D760C7 60F760E7 61176107 61376127 61576147 61776167 61976187 61B761A7 61D761C7 61F761E7 62176207 62376227 62576247 62776267 	{ trace(r23); M1 = r23; g7 = r23; if (!LE(r23,00000000)) jump:nt	FFFFEB90; if (LE(r23,00000000)) jump:nt	FFFFEB4C; if (!EQ(r23,00000000)) jump:nt	FFFFEB08; if (EQ(r23,00000000)) jump:nt	FFFFEAC4; if (!GE(r23,00000000)) jump:nt	FFFFEA80; if (GE(r23,00000000)) jump:nt	FFFFEA3C; if (!NE(r23,00000000)) jump:nt	FFFFE9F8; if (NE(r23,00000000)) jump:nt	FFFFE9B4; p3 = sp3loop0(000027A4,r23); p3 = sp2loop0(000027A0,r23); p3 = sp1loop0(0000279C,r23); p3 = sp1loop0(00002798,r23) }
62976287     	{  }
62B762A7     	{  }
62D762C7     	{  }
62F762E7     	{  }
63176307     	{  }
63376327     	{  }
63576347     	{  }
63776367     	{  }
63976387     	{  }
63B763A7     	{  }
63D763C7     	{  }
63F763E7     	{  }
64176407 64376427 	{ swi(r23) }
64576447     	{  }
64776467     	{  }
64976487     	{  }
64B764A7     	{  }
64D764C7     	{  }
64F764E7     	{  }
65176507 65376527 65576547 	{ crswap(r23,sgp1); crswap(r23,sgp0) }
65776567     	{  }
65976587     	{  }
65B765A7     	{  }
65D765C7     	{  }
65F765E7     	{  }
66176607 66376627 	{ r7 = getimask(r23) }
66576647     	{  }
66776667 66976687 	{ r7 = iassignr(r23) }
66B766A7     	{  }
66D766C7     	{  }
66F766E7     	{  }
67176707 67376727 67576747 	{ brkptcfg1 = r23; ccr = r23 }
67776767     	{  }
67976787     	{  }
67B767A7     	{  }
67D767C7     	{  }
67F767E7     	{  }
68176807     	{  }
68376827     	{  }
68576847     	{  }
68776867     	{  }
68976887     	{  }
68B768A7     	{  }
68D768C7     	{  }
68F768E7     	{  }
69176907 69376927 69576947 	{ loop1(0000293C,00000177); loop0(00002938,00000173) }
69776967     	{  }
69976987     	{  }
69B769A7 69D769C7 69F769E7 6A176A07 6A376A27 6A576A47 	{  }
6A776A67     	{  }
6A976A87     	{  }
6AB76AA7     	{  }
6AD76AC7     	{  }
6AF76AE7     	{  }
6B176B07 6B376B27 6B576B47 6B776B67 6B976B87 	{  }
6BB76BA7     	{  }
6BD76BC7 6BF76BE7 6C176C07 	{ p3 = or(p3,or(p3,!p3)); p3 = or(p3,and(p3,!p3)) }
6C376C27 6C576C47 	{  }
6C776C67     	{  }
6C976C87 6CB76CA7 6CD76CC7 	{ tlbinvasid(r23); r7 = tlbp(r23) }
6CF76CE7     	{  }
6D176D07     	{  }
6D376D27     	{  }
6D576D47     	{  }
6D776D67     	{  }
6D976D87     	{  }
6DB76DA7     	{  }
6DD76DC7     	{  }
6DF76DE7     	{  }
6E176E07     	{  }
6E376E27     	{  }
6E576E47     	{  }
6E776E67     	{  }
Unknown system register 23.


;; hexagon_pre_main: 00003BB8
hexagon_pre_main proc
7800C01E     	{ r30 = 00000000 }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7221C000     	{ r1.h = 0000 }
71E1E018     	{ r1.l = 8063 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7224C000     	{ r4.h = 0000 }
71E4E018     	{ r4.l = 8063 }
9184C004     	{ r4 = memw(r4) }
7225C000     	{ r5.h = 0000 }
7125C000     	{ r5.l = 0000 }
7505C000     	{ p0 = cmp.eq(r5,00000000) }
7220C400     	{ r0.h = 1000 }
7120C000     	{ r0.l = 0000 }
7400C005     	{ if (p0) r5 = add(r0,00000000) }
F304C505     	{ r5 = add(r4,r5) }
B005C1E5     	{ r5 = add(r5,0000000F) }
7625FE05     	{ r5 = and(r5,FFFFFFF0) }
7221C000     	{ r1.h = 0000 }
71E1E01C     	{ r1.l = 8073 }
A181C500     	{ memw(r1) = r5 }
9181C005     	{ r5 = memw(r1) }
7227C000     	{ r7.h = 0000 }
7127C000     	{ r7.l = 0000 }
7507C000     	{ p0 = cmp.eq(r7,00000000) }
7220C010     	{ r0.h = 0040 }
7120C000     	{ r0.l = 0000 }
7400C007     	{ if (p0) r7 = add(r0,00000000) }
F305C706     	{ r6 = add(r5,r7) }
7626FE06     	{ r6 = and(r6,FFFFFFF0) }
71E0E020     	{ r0.l = 8083 }
7220C000     	{ r0.h = 0000 }
9180C000     	{ r0 = memw(r0) }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5C20C018     	{ if (!p0) jump:nt	00003C78 }
7221C000     	{ r1.h = 0000 }
71E1E020     	{ r1.l = 8083 }
A181C600     	{ memw(r1) = r6 }
9181C006     	{ r6 = memw(r1) }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
7221C000     	{ r1.h = 0000 }
71E1E020     	{ r1.l = 8083 }
4481C000     	{  }
4581C000     	{ if (!p0) r0 = memw(r1) }
7226C000     	{ r6.h = 0000 }
71E6E020     	{ r6.l = 8083 }
9186C006     	{ r6 = memw(r6) }
F327C607     	{ r7 = sub(r6,r7) }
B007C1E7     	{ r7 = add(r7,0000000F) }
7627FE07     	{ r7 = and(r7,FFFFFFF0) }
7221C000     	{ r1.h = 0000 }
71E1E024     	{ r1.l = 8093 }
A181C700     	{ memw(r1) = r7 }
9181C007     	{ r7 = memw(r1) }
7221C000     	{ r1.h = 0000 }
7121C000     	{ r1.l = 0000 }
1101E10A     	{ if (p0.new) jump:t	00003CBC; p0 = cmp.gtu(r1,-00000001) }
7220C000     	{ r0.h = 0000 }
71E0E020     	{ r0.l = 8083 }
9180C000     	{ r0 = memw(r0) }
7620FE1D     	{ r29 = and(r0,FFFFFFF0) }
7220C001     	{ r0.h = 0004 }
7120C000     	{ r0.l = 0000 }
6220C00B     	{ gp = r0 }
723CC000     	{ r28.h = 0000 }
717CD2C0     	{ r28.l = 4B01 }
7220C000     	{ r0.h = 0000 }
71E0E02C     	{ r0.l = 80B3 }
7802C001     	{ r1 = 00000400 }
50BCC000     	{ callr	r28 }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
1080C126     	{ if (p0.new) jump:nt	00003D34; p0 = cmp.gt(r0,00000002) }
7220C000     	{ r0.h = 0000 }
71E0E700     	{ r0.l = 9C03 }
7222C000     	{ r2.h = 0000 }
71E2F240     	{ r2.l = C903 }
723CC000     	{ r28.h = 0000 }
717CD390     	{ r28.l = 4E41 }
78004001 50BC4000 F320C202 	{ r2 = sub(r2,r0); callr	r28; r1 = 00000000 }
7220C001     	{ r0.h = 0004 }
7120C100     	{ r0.l = 0400 }
7222C001     	{ r2.h = 0004 }
7122C100     	{ r2.l = 0400 }
723CC000     	{ r28.h = 0000 }
717CD390     	{ r28.l = 4E41 }
78004001 50BC4000 F320C202 	{ r2 = sub(r2,r0); callr	r28; r1 = 00000000 }

;; hexagon_start_main: 00003D34
hexagon_start_main proc
7222C000     	{ r2.h = 0000 }
7122C000     	{ r2.l = 0000 }
7223C000     	{ r3.h = 0000 }
71E3E430     	{ r3.l = 90C3 }
7542C020     	{ p0 = cmp.gt(r2,00000001) }
7E004000 7E004001 45C3C000 	{ if (!p0) r1:r0 = memd(r3); if (p0) r1 = 00000000; if (p0) r0 = 00000000 }
7222C000     	{ r2.h = 0000 }
7122FDC0     	{ r2.l = F700 }
7223C000     	{ r3.h = 0000 }
71A3F360     	{ r3.l = CD82 }
72A4FABE     	{ r4.h = EAFA }
71A4FEEF     	{ r4.l = FBBE }
7064C005     	{ r5 = r4 }
F5054406 F5054408 F505440A F505C40C 	{ r13:r12 = combine(r5,r4); r11:r10 = combine(r5,r4); r9:r8 = combine(r5,r4); r7:r6 = combine(r5,r4) }
F505440E F5054410 F5054412 F505C414 	{ r21:r20 = combine(r5,r4); r19:r18 = combine(r5,r4); r17:r16 = combine(r5,r4); r15:r14 = combine(r5,r4) }
F5054416 F5054418 F505C41A 	{ r27:r26 = combine(r5,r4); r25:r24 = combine(r5,r4); r23:r22 = combine(r5,r4) }
723CC000     	{ r28.h = 0000 }
717CD4C0     	{ r28.l = 5301 }
50BCC000     	{ callr	r28 }
7220C000     	{ r0.h = 0000 }
7120C000     	{ r0.l = 0000 }
7500C000     	{ p0 = cmp.eq(r0,00000000) }
5120C000     	{ if (!p0) callr	r0 }
7800C7E0     	{ r0 = 0000003F }
6460C000     	{ stop(r0) }
