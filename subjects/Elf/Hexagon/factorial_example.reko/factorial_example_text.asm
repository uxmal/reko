;;; Segment .text (00005000)

;; .EventVectors: 00005000
.EventVectors proc
59FFDCB8     	{ jump	00000970 }
59FFDD32     	{ jump	00000A68 }
59FFDD36     	{ jump	00000A74 }
59FFDD3A     	{ jump	00000A80 }
59FFDD58     	{ jump	00000AC0 }
59FFDD36     	{ jump	00000A80 }
59FFDDD4     	{ jump	00000BC0 }
59FFDD32     	{ jump	00000A80 }
59FFDE48     	{ jump	00000CB0 }
59FFDF06     	{ jump	00000E30 }
59FFDD2C     	{ jump	00000A80 }
59FFDD2A     	{ jump	00000A80 }
59FFDD28     	{ jump	00000A80 }
59FFDD26     	{ jump	00000A80 }
59FFDD24     	{ jump	00000A80 }
59FFDD22     	{ jump	00000A80 }
59FFDEFE     	{ jump	00000E3C }
59FFDEFC     	{ jump	00000E3C }
59FFDEFA     	{ jump	00000E3C }
59FFDEF8     	{ jump	00000E3C }
59FFDEF6     	{ jump	00000E3C }
59FFDEF4     	{ jump	00000E3C }
59FFDEF2     	{ jump	00000E3C }
59FFDEF0     	{ jump	00000E3C }
59FFDEEE     	{ jump	00000E3C }
59FFDEEC     	{ jump	00000E3C }
59FFDEEA     	{ jump	00000E3C }
59FFDEE8     	{ jump	00000E3C }
59FFDEE6     	{ jump	00000E3C }
59FFDEE4     	{ jump	00000E3C }
59FFDEE2     	{ jump	00000E3C }
59FFDEE0     	{ jump	00000E3C }
59FFDEDE     	{ jump	00000E3C }
59FFDEDC     	{ jump	00000E3C }
59FFDEDA     	{ jump	00000E3C }
59FFDED8     	{ jump	00000E3C }
59FFDED6     	{ jump	00000E3C }
59FFDED4     	{ jump	00000E3C }
59FFDED2     	{ jump	00000E3C }
59FFDED0     	{ jump	00000E3C }
59FFDECE     	{ jump	00000E3C }
59FFDECC     	{ jump	00000E3C }
59FFDECA     	{ jump	00000E3C }
59FFDEC8     	{ jump	00000E3C }
59FFDEC6     	{ jump	00000E3C }
59FFDEC4     	{ jump	00000E3C }
59FFDEC2     	{ jump	00000E3C }
59FFDEC0     	{ jump	00000E3C }

;; strict_aliasing_workaround: 000050C0
strict_aliasing_workaround proc
A09DC002     	{ allocframe(+00000010) }
A79EE0FD     	{ memw(r30-12) = r0 }
979EFFA0     	{ r0 = memw(r30-12) }
A79EE0FF     	{ memw(r30-4) = r0 }
979EFFE0     	{ r0 = memw(r30-4) }
901EC01E     	{ deallocframe }
529FC000     	{ jumpr	r31 }

;; fact: 000050DC
fact proc
A09DC001     	{ allocframe(+00000008) }
A79EE0FF     	{ memw(r30-4) = r0 }
979EFFE0     	{ r0 = memw(r30-4) }
7540C020     	{ p0 = cmp.gt(r0,00000001) }
7A00C020     	{ r0 = mux(p0,00000001,00000000) }
8500C000     	{ p0 = tstbit(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	00005100 }
7800C020     	{ r0 = 00000001 }
5800C00E     	{ jump	00005118 }
979EFFE0     	{ r0 = memw(r30-4) }
BFE0FFE0     	{ r0 = add(r0,FFFFFFFF) }
5BFFFFEA     	{ call	000050DC }
7060C001     	{ r1 = r0 }
979EFFE0     	{ r0 = memw(r30-4) }
ED01C000     	{ r0 = mpyi(r1,r0) }
901EC01E     	{ deallocframe }
529FC000     	{ jumpr	r31 }

;; main: 00005120
main proc
A09DC003     	{ allocframe(+00000018) }
A79EE0FD     	{ memw(r30-12) = r0 }
A79EE1FC     	{ memw(r30-16) = r1 }
979EFFA0     	{ r0 = memw(r30-12) }
7500C040     	{ p0 = cmp.eq(r0,00000002) }
7A00C020     	{ r0 = mux(p0,00000001,00000000) }
8500C000     	{ p0 = tstbit(r0,00000000) }
5C20C022     	{ if (!p0) jump:nt	00005180 }
979EFF80     	{ r0 = memw(r30-16) }
B000C080     	{ r0 = add(r0,00000004) }
9180C000     	{ r0 = memw(r0) }
5A00C048     	{ call	000051DC }
A79EE0FF     	{ memw(r30-4) = r0 }
979EFFE0     	{ r0 = memw(r30-4) }
A19DC000     	{ memw(r29) = r0 }
00004340 7800C000 5A00C0D6 	{ call	00005310; r0 = 0000D000 }
979EFFE0     	{ r0 = memw(r30-4) }
7560FFE0     	{ p0 = cmp.gt(r0,FFFFFFFF) }
7A00C020     	{ r0 = mux(p0,00000001,00000000) }
8500C000     	{ p0 = tstbit(r0,00000000) }
5C20C00E     	{ if (!p0) jump:nt	00005194 }
5800C016     	{ jump	000051A8 }
00004340 7800C260 5A00C0C4 	{ call	00005310; r0 = 0000D013 }
78DFFFE0     	{ r0 = FFFFFFFF }
5800C022     	{ jump	000051D4 }
00004340 7800C5E0 5A00C0BA 	{ call	00005310; r0 = 0000D02F }
78DFFFE0     	{ r0 = FFFFFFFF }
5800C018     	{ jump	000051D4 }
979EFFE0     	{ r0 = memw(r30-4) }
5BFFFF98     	{ call	000050DC }
A79EE0FE     	{ memw(r30-8) = r0 }
979EFFE0     	{ r0 = memw(r30-4) }
A19DC000     	{ memw(r29) = r0 }
979EFFC0     	{ r0 = memw(r30-8) }
A19DC001     	{ memw(r29+4) = r0 }
00004341 7800C200 5A00C0A2 	{ call	00005310; r0 = 0000D050 }
7800C000     	{ r0 = 00000000 }
901EC01E     	{ deallocframe }
529FC000     	{ jumpr	r31 }

;; atoi: 000051DC
atoi proc
A09DC001     	{ allocframe(+00000008) }
A79EE0FF     	{ memw(r30-4) = r0 }
979EFFE0     	{ r0 = memw(r30-4) }
7800C001     	{ r1 = 00000000 }
7800C142     	{ r2 = 0000000A }
5A00CB52     	{ call	00006894 }
901EC01E     	{ deallocframe }
529FC000     	{ jumpr	r31 }
7F00C000     	{ nop }

;; thread_create: 00005200
thread_create proc
72244000 8C024247 72254000 7226C000 	{ r6.h = 0000; r5.h = 0000; r7 = asl(r2,00000002); r4.h = 0000 }
71245AE4 71255AFC 71265B14 7800C028 	{ r8 = 00000001; r6.l = 6C50; r5.l = 6BF0; r4.l = 6B90 }
F3074505 C6484288 38743876 	{ r6 = add(r6,r7); r4 = add(r4,r7); r8 &= asl(r8,r2); r5 = add(r7,r5) }
A184C000     	{ memw(r4) = r0 }
A185C100     	{ memw(r5) = r1 }
A186C300     	{ memw(r6) = r3 }
6468C020     	{ start(r8) }
529FC000     	{ jumpr	r31 }

;; thread_stop: 00005240
thread_stop proc
6E884000 7800C021 	{ r1 = 00000001; r0 = htid }
C641C0C1     	{ r1 &= lsl(r1,r0) }
6461C000     	{ stop(r1) }
723CC000     	{ r28.h = 0000 }
713CC770     	{ r28.l = 1DC0 }
529CC000     	{ jumpr	r28 }
7F00C000     	{ nop }

;; thread_join: 00005260
thread_join proc
6E884001 7800C023 	{ r3 = 00000001; r1 = htid }
C643C181     	{ r1 &= asl(r3,r1) }
7661FFE1     	{ r1 = sub(FFFFFFFF,r1) }
F100C100     	{ r0 = and(r0,r1) }
F3E0C000     	{ r0 = combine(r0.l,r0.l) }
6E91C002     	{ r2 = modectl }
F100C202     	{ r2 = and(r0,r2) }
59203FC6     	{ if (p0.new) jumpr	r31; p0 = cmp.eq(r2,00000000) }
5440C004     	{ pause(00000001) }
59FFFFF8     	{ jump	00005278 }
7F00C000     	{ nop }

;; thread_get_tnum: 00005290
thread_get_tnum proc
6E884000 529FC000 	{ jumpr	r31; r0 = htid }
7F004000 7F00C000 	{ nop; nop }

;; thread_stack_size: 000052A0
thread_stack_size proc
7222C000     	{ r2.h = 0000 }
71225B2C 8C00C243 	{ r3 = asl(r0,00000002); r2.l = 6CB0 }
F302C304     	{ r4 = add(r2,r3) }
A184C100     	{ memw(r4) = r1 }
529FC000     	{ jumpr	r31 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __sys_get_cmdline: 000052C0
__sys_get_cmdline proc
78004002 00004390 49804703 A09DC002 	{ allocframe(+00000010); r3 = memw(gp+224); r2 = 00000000 }
10034018 A1DDD001 	{ memd(r29+8) = r17:r16; if (p0.new) jump:nt	00005300; p0 = cmp.eq(r3,00000000) }
6C082800     	{ memw(r29) = r0; r16 = add(r29,00000000) }
8CD0C202     	{ r2 = setbit(r16,00000004) }
5A004198 A182C100 	{ memw(r2) = r1; call	00005610 }
5A004194 28812C00 	{ r0 = add(r29,00000000); r1 = 00000008; call	00005610 }
7800C2A0     	{ r0 = 00000015 }
7070C001     	{ r1 = r16 }
5400C000     	{ trap0(00000000) }
7060C002     	{ r2 = r0 }
70624000 3E0C1F40 	{ dealloc_return; r17:r16 = memd(r29+8); r0 = r2 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; printf: 00005310
printf proc
70604010 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r16 = r0 }
B01D4301 00004392 78004300 A1BDD501 	{ r0 = 0000E498; r1 = add(r29,00000018) }
5A00CB18     	{ call	00006958 }
70704002 00004392 49813C13 5A00421C 0000414D 7800C400 5A004B66 7F004000 00004392 29803008 70704000 7F004000 3E0C1F40 	{ dealloc_return; r17:r16 = memd(r29+8); nop; r0 = r16; r16 = r0; r0 = 00000018; nop; call	00006A10; r0 = 00005360; call	00005770; r3 = memd(r29+4); r1 = 00000018; r2 = r16 }

;; prout: 00005360
prout proc
70614004 F5004210 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r2); r4 = r1 }
5A004EAA 73246020 	{ call	000070C0 }
7071C003     	{ r3 = r17 }
F2005000 74116000 5A503E04 	{ r17:r16 = memd(r29); r0 = -00000001; if (p0.new) r0 = add(r17,00000000); p0 = cmp.eq(r0,r16) }
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; memset: 00005390
memset proc
61824080 8C4140E7 7060C006 	{ r6 = r0; r7 = vsplatb(r1); if (EQ(r2,00000000)) jump:nt	00005490 }
F5074704 1102C810 	{ if (p0.new) jump:t	000053C0; p0 = cmp.gtu(r2,-00000001); r5:r4 = combine(r7,r7) }
60024018 70604003 7F00C000 	{ nop; r3 = r0; loop0(000053B0,r2) }
7F008000 AB03C108 	{ memb(r3++#1) = r1; nop }
529FC000     	{ jumpr	r31 }
11C0430A 7502C021 	{ p1 = cmp.eq(r2,00000001); if (!p0.new) jump:t	000053D0; p0 = tstbit(r0,00000000) }
BFE27FE2 5C004166 71061001 	{ memb(r0) = r1; r6 = r0; if (p1) jump:nt	00005494; r2 = add(r2,FFFFFFFF) }
85064100 5C20C80A 	{ if (!p0.new) jump:nt	000053E8; p0 = tstbit(r6,00000001) }
BFE27FC2 1002425C AB46C708 	{ memuh(r6++#2) = r7; if (p0.new) jump:nt	00005494; p0 = cmp.eq(r2,00000004); r2 = add(r2,FFFFFFFE) }
85064200 5C20C80A 	{ if (!p0.new) jump:nt	000053FC; p0 = tstbit(r6,00000002) }
BFE27F82 10024452 AB86C708 	{ memw(r6++#4) = r7; if (p0.new) jump:nt	00005494; p0 = cmp.eq(r2,00000008); r2 = add(r2,FFFFFFFC) }
75824FE0 5C20C82C 	{ if (!p0.new) jump:nt	00005454; p0 = cmp.gtu(r2,0000007F) }
7606C3E3     	{ r3 = and(r6,0000001F) }
6183C016     	{ if (EQ(r3,00000000)) jump:nt	00005430 }
BFE27F02 ABC6C408 	{ memd(r6++#8) = r5:r4; r2 = add(r2,FFFFFFF8) }
7606C3E3     	{ r3 = and(r6,0000001F) }
6183C00E     	{ if (EQ(r3,00000000)) jump:nt	00005430 }
BFE27F02 ABC6C408 	{ memd(r6++#8) = r5:r4; r2 = add(r2,FFFFFFF8) }
7606C3E3     	{ r3 = and(r6,0000001F) }
6183C006     	{ if (EQ(r3,00000000)) jump:nt	00005430 }
BFE27F02 ABC6C408 	{ memd(r6++#8) = r5:r4; r2 = add(r2,FFFFFFF8) }
61014032 8C02C523 	{ r3 = lsr(r2,00000005); if (NE(r1,00000000)) jump:nt	00005494 }
60034018 70634008 7066C003 	{ r3 = r6; r8 = r3; loop0(00005444,r3) }
B0068406 BFE27C02 A0C6C000 	{ dczeroa(r6); r2 = add(r2,FFFFFFE0); r6 = add(r6,00000020) }
1142470C 8C02C328 	{ r8 = lsr(r2,00000003); if (!p0.new) jump:t	00005468; p0 = cmp.gtu(r2,-00000001) }
60084010 7F00C000 	{ nop; loop0(00005460,r8) }
BFE2BF02 ABC6C408 	{ memd(r6++#8) = r5:r4; r2 = add(r2,FFFFFFF8) }
85024200 5C20C808 	{ if (!p0.new) jump:nt	0000547C; p0 = tstbit(r2,00000002) }
BFE27F82 AB86C708 	{ memw(r6++#4) = r7; r2 = add(r2,FFFFFFFC) }
85024100 5C20C808 	{ if (!p0.new) jump:nt	0000548C; p0 = tstbit(r2,00000001) }
BFE27FC2 AB46C708 	{ memuh(r6++#2) = r7; r2 = add(r2,FFFFFFFE) }
7502C020     	{ p0 = cmp.eq(r2,00000001) }
4006C100     	{ if (p0) memb(r6) = r1 }
529FC000     	{ jumpr	r31 }
6003C008     	{ loop0(00005498,r3) }
A0C6C000     	{ dczeroa(r6) }
BFE27C02 ABC64410 A1C6C401 	{ memd(r6+8) = r5:r4; memd(r6++#16) = r5:r4; r2 = add(r2,FFFFFFE0) }
ABC68410 A1C6C401 	{ memd(r6+8) = r5:r4; memd(r6++#16) = r5:r4 }
59FFFFD0     	{ jump	00005450 }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __libc_start_main: 000054C0
__libc_start_main proc
75617FE0 70634010 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r3; p0 = cmp.gt(r1,FFFFFFFF) }
5C20C098     	{ if (!p0) jump:nt	000055FC }
10C1E018     	{ if (!p0.new) jump:t	00005500; p0 = cmp.gt(r1,00000000) }
1000E016     	{ if (p0.new) jump:t	00005500; p0 = cmp.eq(r0,00000000) }
91204003 2403E908 	{ r3 = memb(r0) }
5C204810 7503C400 	{ p0 = cmp.eq(r3,00000020); if (!p0.new) jump:nt	00005500 }
B0004023 BFE17FE1 24C3C00E 	{ r1 = add(r1,FFFFFFFF); r3 = add(r0,00000001) }
11F060F2 7063C000 	{ r0 = r3; if (!p0.new) jump:t	000054D8; p0 = cmp.eq(r0,-00000001) }
5800C004     	{ jump	00005504 }
7060C003     	{ r3 = r0 }
10C14062 28002809 	{ r17 = 00000000; r0 = 00000000; if (!p0.new) jump:nt	000055C8; p0 = cmp.gt(r1,00000000) }
78004006 28093D32 	{ r5:r4 = combine(00000000,00000001); r17 = 00000000; r6 = 00000000 }
1004C05A     	{ if (p0.new) jump:nt	000055C8; p0 = cmp.eq(r4,00000000) }
5C004858 7551CC60 	{ p0 = cmp.gt(r17,00000063); if (p0.new) jump:nt	000055C8 }
91244007 2503FF0A 	{ r7 = memb(r4) }
1007E012     	{ if (p0.new) jump:t	0000554C; p0 = cmp.eq(r7,00000000) }
1007E910     	{ if (p0.new) jump:t	0000554C; p0 = cmp.eq(r7,00000012) }
5800C046     	{ jump	000055BC }
5C205808 7507C440 	{ p0 = cmp.eq(r7,00000022); if (!p0.new) jump:t	00005544 }
31433311     	{ r1 = r1; r3 = r4 }
1703C420     	{ jump	00005580; r4 = r3 }
5C20583C 7507C400 	{ p0 = cmp.eq(r7,00000020); if (!p0.new) jump:t	000055BC }
1046E012     	{ if (!p0.new) jump:t	00005570; p0 = cmp.eq(r6,00000000) }
78004006 F2044301 79703240 	{ memb(r4) = 00000000; p0 = cmp.eq(r7,00000000); p1 = cmp.eq(r4,r3); r6 = 00000000 }
5C204122 7E00C025 	{ if (p0) r5 = 00000001; if (!p1) jump:nt	000055A0 }
5C20581E 7507C010 	{ p0 = !cmp.eq(r7,00000000); if (!p0.new) jump:t	000055A0 }
5800C028     	{ jump	000055BC }
1007C02C     	{ if (p0.new) jump:nt	000055C8; p0 = cmp.eq(r7,00000000) }
16064024 B004C023 	{ r3 = add(r4,00000001); jump	000055BC; r6 = 00000000 }
31443311     	{ r1 = r1; r4 = r4 }
9124C006     	{ r6 = memb(r4) }
5C00580C 7506C440 	{ p0 = cmp.eq(r6,00000022); if (p0.new) jump:t	0000559C }
1076E0F8     	{ if (!p0.new) jump:t	0000557C; p0 = cmp.eq(r22,00000001) }
5800403A 00004341 7800C4E0 68163240 	{ memb(r4) = 00000000; r6 = 00000001; r0 = 0000D067; jump	00005604 }
91234007 2403E00A 	{ r7 = memb(r3) }
B0114031 0000439C AD91E380 1045C00A 	{ if (!p0.new) jump:nt	000055C8; p0 = cmp.eq(r5,00000000); memw(r17<<#2+0000E700) = r3; r17 = add(r17,00000001) }
28053143     	{ r3 = r4; r5 = 00000000 }
B0044024 BFE17FE1 24B3E0AC 	{ r1 = add(r1,FFFFFFFF); r4 = add(r4,00000001) }
10026008 0000439C AD91E080 50A2C000 	{ callr	r2; memw(r17<<#2+0000E700) = r0; if (p0.new) jump:t	000055D8; p0 = cmp.eq(r2,00000000) }
10086006 7490E000 	{ if (!p0.new) r0 = add(r16,00000000); if (p0.new) jump:t	000055E4; p0 = cmp.eq(r8,00000000) }
5A00CCA0     	{ call	00006F20 }
0000439C 73316000 	{  }
00004391 7800C002 5BFFFD96 	{ call	00005120; r2 = 0000E440 }
5A00CCBC     	{ call	00006F70 }
78004020 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = 00000001 }
5A00CEBE     	{ call	00007380 }
5A004CB4 7800C000 	{ r0 = 00000000; call	00006F70 }

;; hexagon_cache_cleaninv: 00005610
hexagon_cache_cleaninv proc
7600C3E2     	{ r2 = and(r0,0000001F) }
E201C3E2     	{ r2 += add(r1,0000001F) }
8C024521 2403C018 	{ r1 = lsr(r2,00000005) }
8C014541 28033004 	{ r4 = r0; r3 = 00000000; r1 = asl(r1,00000005) }
8E02C523     	{ r3 -= lsr(r2,00000005) }
7643C002     	{ r2 = sub(00000000,r3) }
6002C008     	{ loop0(00005634,r2) }
B004C402     	{ r2 = add(r4,00000020) }
A044C000     	{ dccleaninva(r4) }
70628004 7F00C000 	{ nop; r4 = r2 }
F300C100     	{ r0 = add(r0,r1) }
BFE0FC00     	{ r0 = add(r0,FFFFFFE0) }
9100C001     	{ r1 = memb(r0) }
A040C000     	{ dccleaninva(r0) }
529FC000     	{ jumpr	r31 }

;; hexagon_cache_inva: 00005658
hexagon_cache_inva proc
7600C3E2     	{ r2 = and(r0,0000001F) }
E201C3E2     	{ r2 += add(r1,0000001F) }
8C024521 2403C014 	{ r1 = lsr(r2,00000005) }
7800C001     	{ r1 = 00000000 }
8E02C521     	{ r1 -= lsr(r2,00000005) }
7641C001     	{ r1 = sub(00000000,r1) }
6001C008     	{ loop0(00005678,r1) }
B000C401     	{ r1 = add(r0,00000020) }
A020C000     	{ dcinva(r0) }
70618000 7F00C000 	{ nop; r0 = r1 }
529FC000     	{ jumpr	r31 }
7F00C000     	{ nop }

;; __registerx: 00005690
__registerx proc
F5014010 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
F5044312 79802A05 	{ memd(r29) = r19:r18; p0 = cmp.eq(r16,00000000); r19:r18 = combine(r4,r3) }
5C00C018     	{ if (p0) jump:nt	000056D0 }
91904000 2402C016 5A004BCA 	{ if (cmp.eq(r0.new,00000000)) jump:nt	000056D4; r0 = memw(r16) }
7C00E300     	{ r1:r0 = combine(00000018,00000001) }
10004012 4680D218 	{ if (p0.new) memw(r0+12) = r18; if (p0.new) jump:nt	000056D8; p0 = cmp.eq(r0,00000000) }
A2093005     	{ memw(r0+20) = 00000000; memw(r0+8) = r17 }
A108040B     	{ memw(r0+16) = r19; memw(r0+4) = r16 }
498043A1 A1A0D300 	{ r1 = memw(gp+116) }
4880C01D     	{ memw(gp) = r0 }
3E0C1E05     	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
961EC01E     	{ dealloc_return }
5A004BAC 7F00C000 	{ nop; call	00006E30 }

;; __register_frame_info_bases: 000056E0
__register_frame_info_bases proc
F5024010 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r17:r16 = combine(r2,r0) }
70634012 79802A05 	{ memd(r29) = r19:r18; p0 = cmp.eq(r16,00000000); r18 = r3 }
5C00C018     	{ if (p0) jump:nt	00005720 }
91904000 2402C016 5A004BA2 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00005724; r0 = memw(r16) }
7C00E300     	{ r1:r0 = combine(00000018,00000001) }
10004012 4680D118 	{ if (p0.new) memw(r0+12) = r17; if (p0.new) jump:nt	00005728; p0 = cmp.eq(r0,00000000) }
F0021005     	{ memw(r0+20) = 00000000; memw(r0+8) = 00000000 }
A108040A     	{ memw(r0+16) = r18; memw(r0+4) = r16 }
498043A1 A1A0D300 	{ r1 = memw(gp+116) }
4880C01D     	{ memw(gp) = r0 }
3E0C1E05     	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
961EC01E     	{ dealloc_return }
5A004B84 7F00C000 	{ nop; call	00006E30 }

;; __deregister_frame_info_bases: 00005730
__deregister_frame_info_bases proc
1000401E A09DC000 	{ allocframe(+00000000); if (p0.new) jump:nt	0000576C; p0 = cmp.eq(r0,00000000) }
91804001 2403C01A 	{ r1 = memw(r0) }
00004401 7800C681 7061C002 	{ r2 = r1; r1 = 00010074 }
91824001 2403C010 	{ r1 = memw(r2) }
91814023 2073E0FA 	{ r3 = memw(r1+4) }
5A004C4A 70614000 91814003 A1A2D300 	{ r3 = memw(r1); r0 = r1; call	00006FF0 }
48003F40     	{ dealloc_return; r0 = 00000000 }

;; _Printf: 00005770
_Printf proc
5A0065A8 A09DC019 	{ allocframe(+000000C8); call	0000A2C0 }
F5014012 B01D4A80 30313C02 	{ r5:r4 = combine(00000000,00000000); r1 = r3; r0 = add(r29,00000054); r19:r18 = combine(r1,r0) }
07FF7FFF 730267F0 	{  }
6D8D2A5A     	{ memd(r29+88) = r5:r4; r21 = add(r29,00000060) }
5A004A30 B01D4A16 78017CF7 7801FD18 	{ r24 = 000003E8; r23 = 000003E7; r22 = add(r29,00000050); call	00006BF0 }
00004342 78004119 A2DA03DB 	{ memw(r21+12) = r19; memw(r21+8) = r18; r25 = 0000D088 }
58004006 3C55C680 	{ memw(r21+52) = FFFFFF80; jump	000057B8 }
B011C031     	{ r17 = add(r17,00000001) }
B01D4B03 70704002 2D403091 	{ r1 = r17; r0 = add(r29,00000050); r2 = r16; r3 = add(r29,00000058) }
5A005BBC 3C56C000 	{ memw(r22) = 00000000; call	00008F3C }
70604012 2482E008 9131C000 	{ if (cmp.gt(r18.new,00000000)) jump:t	000057E0; r18 = r0 }
7360E012     	{ r18 = !cmp.eq(r0,00000000) }
919DC280     	{ r0 = memw(r29+80) }
750044A0 7E0F7FE1 7E80E001 	{ if (!p0.new) r1 = 00000000; if (p0.new) r1 = FFFFFFFF; p0 = cmp.eq(r0,00000025) }
F3015213 24C3E01A 	{ r19 = add(r1,r18) }
70714001 70734002 03D002D3 	{ r3 = memw(r21+8); r0 = memw(r21+12); r2 = r19; r1 = r17 }
78DF7FF4 50A3C000 	{ callr	r3; r20 = FFFFFFFF }
1010405A A195C003 	{ memw(r21+12) = r0; if (p0.new) jump:nt	00005ABC; p0 = cmp.eq(r16,00000000) }
9195C1A0     	{ r0 = memw(r21+52) }
F3005300 A1B5D20D 919DC280 	{ memb(r21+13) = r0.new; r0 = add(r0,r19) }
5C206842 F3115211 7500C4A0 	{ p0 = cmp.eq(r0,00000025); r17 = add(r17,r18); if (!p0.new) jump:nt	00005AA4 }
7D9530DB     	{ memw(r21+44) = 00000000; r19:r18 = combine(00000000,00000000) }
5A0050B4 9132C014 	{ r20 = memb(r18); call	00007998 }
3A20D480     	{ r0 = memb(r13+r20<<#1) }
76004400 2402C010 E0134140 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00005860; r0 = and(r0,00000020) }
9B32C033     	{ r19 = memb(r18++#1) }
E200DA13     	{ r19 += add(r0,FFFFFFD0) }
5CDF78F0 F2585300 A195D30B 	{ memw(r21+44) = r19; p0 = cmp.gt(r24,r19); if (p0.new) jump:t	00005830 }
9132C000     	{ r0 = memb(r18) }
5C20580E 7500C480 	{ p0 = cmp.eq(r0,00000024); if (!p0.new) jump:t	0000587C }
31AA33B0     	{ r0 = r19; r18 = r18 }
5800400A F2774000 74916012 39D5C580 	{ if (!p0.new) memw(r21+44) = 00000000; if (!p0.new) r18 = add(r17,00000000); p0 = cmp.gtu(r23,r0); jump	00005880 }
709A30DB     	{ memw(r21+44) = 00000000; r18 = r17 }
780040A2 3C554480 3C35CF00 	{ memuh(r21+60) = 0000; memw(r21+36) = FFFFFF80; r2 = 00000005 }
00004342 78004100 F0DA10D5 	{ memw(r21+20) = 00000000; memw(r21+40) = 00000000; r0 = 0000D088 }
F0D610D7     	{ memw(r21+28) = 00000000; memw(r21+24) = 00000000 }
3C55C400     	{ memw(r21+32) = 00000000 }
5A004D58 9132C001 	{ r1 = memb(r18); call	00007350 }
1000E01E     	{ if (p0.new) jump:t	000058E4; p0 = cmp.eq(r0,00000000) }
16014008 B012C032 	{ r18 = add(r18,00000001); jump	000058BC; r1 = 00000000 }
B0124032 9175C3C1 	{ r1 = memuh(r21+60); r18 = add(r18,00000001) }
F3394000 7800C0A2 	{ r2 = 00000005; r0 = sub(r0,r25) }
00004342 9D80F600 F1214000 A1B5CA1E 5A004D3E 	{ memb(r21+30) = r0.new; r0 = or(r1,r0); r0 = memw(r0<<#2+0000D098) }
00004342 488010A1 1070E0EA 	{ if (!p0.new) jump:t	000058B4; p0 = cmp.eq(r16,00000001); r1 = memb(r18); r0 = 00000008 }
9132C000     	{ r0 = memb(r18) }
5C205820 75004540 39D5C700 	{ if (!p0.new) memw(r21+56) = 00000000; p0 = cmp.eq(r0,0000002A); if (!p0.new) jump:t	00005928 }
919DC2A0     	{ r0 = memw(r29+84) }
B0004081 A1BDD315 	{ r1 = add(r0,00000004) }
9180C000     	{ r0 = memw(r0) }
1180610E 475543C1 A195C00E 	{ memw(r21+56) = r0; if (!p0.new) r1 = memh(r21+60); if (p0.new) jump:t	00005920; p0 = tstbit(r0,00000000) }
8CC14201 7640C000 	{ r0 = sub(00000000,r0); r1 = setbit(r1,00000004) }
A155411E A195C00E 	{ memw(r21+56) = r0; memuh(r21+60) = r1 }
58004022 B012C032 	{ r18 = add(r18,00000001); jump	00005964 }
5A005038 9132C011 	{ r17 = memb(r18); call	00007998 }
5800C006     	{ jump	0000593C }
5A005032 51AA11A9 	{ r17 = memb(r18+1); r18 = r18; call	00007998 }
3A20D180     	{ r0 = memb(r13+r17<<#1) }
76004400 2402E012 919541C0 	{ if (cmp.eq(r0.new,00000000)) jump:t	00005968; r0 = and(r0,00000020) }
2032F0F6 E0004140 	{ if (cmp.eq(r0.new,r16)) jump:t	00005938 }
9132C001     	{ r1 = memb(r18) }
E2005A01 59FF7FEE A1B5D50E 	{ jump	00005938; r1 += add(r0,FFFFFFD0) }
9132C000     	{ r0 = memb(r18) }
5C20581A 750045C0 39D5E61F 	{ if (!p0.new) memw(r21+48) = FFFFFFFF; p0 = cmp.eq(r0,0000002E); if (!p0.new) jump:t	0000599C }
9132C020     	{ r0 = memb(r18+1) }
5C205814 75004540 39D5C600 	{ if (!p0.new) memw(r21+48) = 00000000; p0 = cmp.eq(r0,0000002A); if (!p0.new) jump:t	000059A0 }
402A3D50     	{ r0 = memd(r29+84); r18 = add(r18,00000002) }
B0004081 A1BDD315 	{ r1 = add(r0,00000004) }
5800402A 91804000 A1B5D20C 5800C024 	{ memb(r21+12) = r0.new; r0 = memw(r0); jump	000059E4 }
5A004FFC 9132C031 	{ r17 = memb(r18+1); call	00007998 }
5800400C B0124032 3A20D180 	{ r0 = memb(r13+r17<<#1); r18 = add(r18,00000001); jump	000059C0 }
5A004FF2 51AA11A9 	{ r17 = memb(r18+1); r18 = r18; call	00007998 }
3A20D180     	{ r0 = memb(r13+r17<<#1) }
76004400 2402E012 91954180 	{ if (cmp.eq(r0.new,00000000)) jump:t	000059E8; r0 = and(r0,00000020) }
2032F0F6 E0004140 	{ if (cmp.eq(r0.new,r16)) jump:t	000059B8 }
9132C001     	{ r1 = memb(r18) }
E2005A01 59FF7FEE A1B5D50C 	{ jump	000059B8; r1 += add(r0,FFFFFFD0) }
5A004F76 00004342 4B0010A1 10406008 74926011 3915DF00 	{ if (p0.new) memb(r21-2) = 00; if (!p0.new) r17 = add(r18,00000000); if (!p0.new) jump:t	00005A00; p0 = cmp.eq(r0,00000000); r1 = memb(r18); r0 = 00000030; call	000078D0 }
170AC92A     	{ jump	00005A50; r9 = r10 }
9B314020 A1B5C23E 5C205814 	{ memb(r21+62) = r0.new; r0 = memb(r17++#1) }
75004D80 4331C000 	{ if (p0.new) r0 = memb(r17); p0 = cmp.eq(r0,0000006C) }
5C20581E 74126051 7500CD80 	{ p0 = cmp.eq(r0,0000006C); if (p0.new) r17 = add(r18,00000002); if (!p0.new) jump:t	00005A50 }
58004018 7F004000 7F004000 3C15DF71 	{ memb(r21-2) = 71; nop; nop; jump	00005A50 }
5C205810 75004D00 4331C000 	{ if (p0.new) r0 = memb(r17); p0 = cmp.eq(r0,00000068); if (!p0.new) jump:t	00005A50 }
5C20580A 74126051 7500CD00 	{ p0 = cmp.eq(r0,00000068); if (p0.new) r17 = add(r18,00000002); if (!p0.new) jump:t	00005A50 }
7F004000 3C15DF62 	{ memb(r21-2) = 62; nop }
91954160 24C2E018 BFE07FE1 	{ if (!cmp.gt(r0.new,00000000)) jump:t	00005A84; r0 = memw(r21+44) }
B01D4080 A1B5D50B 	{ r0 = add(r29,00000004) }
5A0048C6 919DC2A1 	{ r1 = memw(r29+84); call	00006BF0 }
B01D4081 B01D4103 4D801092 	{ r2 = memb(r17); r0 = add(r29,00000060); r3 = add(r29,00000008); r1 = add(r29,00000004) }
5800400A 7F00C000 	{ nop; jump	00005A8C }
B01D4A81 B01D4C00 4C231092 	{ r2 = memb(r17); r3 = add(r29,00000008); r0 = add(r29,00000060); r1 = add(r29,00000054) }
5A00C022     	{ call	00005AD0 }
1040C014     	{ if (!p0.new) jump:nt	00005AB8; p0 = cmp.eq(r0,00000000) }
5A004446 2C212D80 	{ r0 = add(r29,00000060); r1 = add(r29,00000008); call	00006320 }
11A0E18C     	{  }
5800C00C     	{ jump	00005AB8 }
1060608A 741DEC00 	{ if (p0.new) r0 = add(r29,00000060); if (!p0.new) jump:t	000057B8; p0 = cmp.eq(r0,00000001) }
9180C1B4     	{ r20 = memw(r0+52) }
00004122 170CC048 78DFFFF4 	{ r20 = FFFFFFFF; jump	00005B40; r0 = r12 }
00004122 170CC030 7F00C000 	{ nop; jump	00005B1C; r0 = r12 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Putfld: 00005AD0
_Putfld proc
75824800 70604010 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r16 = r0; p0 = cmp.gtu(r2,00000040) }
5C00C012     	{ if (p0) jump:nt	00005B00 }
5C2058B4 750244A0 4390C0A0 	{ if (p0.new) r0 = memw(r16+20); p0 = cmp.eq(r2,00000025); if (!p0.new) jump:t	00005C48 }
B0004021 F3034000 A1B0D505 	{ r0 = add(r3,r0); r1 = add(r0,00000001) }
58004102 3C00C025 	{ memb(r0) = 25; jump	00005CFC }
5C005830 7582CA40 	{ p0 = cmp.gtu(r2,00000052); if (p0.new) jump:t	00005B60 }
BFE277E0 2502E6A0 C680C0E0 	{ if (cmp.gtu(r0.new,0000000D)) jump:t	00005C4C; r0 = add(r2,FFFFFFBF) }
76004E20 2402C09A 91904160 	{ if (cmp.eq(r0.new,00000001)) jump:nt	00005C4C; r0 = and(r0,00000071) }
26C2C0AE 9181C000 	{ if (!tstbit(r0.new,-00000001)) jump:nt	00005C7C }
B000C0E0     	{ r0 = add(r0,00000007) }
7620FF00     	{ r0 = and(r0,FFFFFFF8) }
B0004104 A1A1D200 91904166 	{ memb(r1) = r4.new; r4 = add(r0,00000008) }
91C0C004     	{ r5:r4 = memd(r0) }
BFE67FE0 75464000 A1D0C400 	{ memd(r16) = r5:r4; p0 = cmp.gt(r6,00000000); r0 = add(r6,FFFFFFFF) }
5CDF60EC A190C00B 	{ memw(r16+44) = r0; if (p0) jump:nt	00005B24 }
F505C400     	{ r1:r0 = combine(r5,r4) }
80007020 5800C092 	{ jump	00005C80; r1:r0 = lsr(r1:r0,00000030) }
5C005816 74027500 7582CAE0 	{ p0 = cmp.gtu(r2,00000057); if (p0.new) r0 = add(r2,FFFFFFA8); if (p0.new) jump:t	00005B8C }
5C20586E 74106582 7502CA60 	{ p0 = cmp.eq(r2,00000053); if (p0.new) r2 = add(r16,0000002C); if (!p0.new) jump:t	00005C48 }
78004000 91904163 3C10DF6C 	{ memb(r16-2) = 6C; r3 = memw(r16+44); r0 = 00000000 }
1193E192     	{  }
5800C19E     	{ jump	00005EC4 }
5C00585E 7580C400 	{ p0 = cmp.gtu(r0,00000020); if (p0.new) jump:t	00005C48 }
4980C3C4     	{ r4 = memw(gp+120) }
3A84E000     	{ r0 = memw(r14+r0<<#2) }
5280C000     	{ jumpr	r0 }
91904160 26C2C042 9130C7C0 	{ if (!tstbit(r0.new,-00000001)) jump:nt	00005C28; r0 = memw(r16+44) }
5C20580A 75004E20 4381C000 	{ if (p0.new) r0 = memw(r1); p0 = cmp.eq(r0,00000071); if (!p0.new) jump:t	00005BC0 }
5800401A B000C0E0 	{ r0 = add(r0,00000007); jump	00005BEC }
5C205810 75004D80 43814000 4781C004 	{ if (!p0.new) r4 = memw(r1); if (p0.new) r0 = memw(r1); p0 = cmp.eq(r0,0000006C); if (!p0.new) jump:t	00005BE0 }
B0004084 A1A1D200 5800401A 	{ memb(r1) = r4.new; r4 = add(r0,00000004) }
9180C000     	{ r0 = memw(r0) }
5C205810 740460E0 7500CD40 	{ p0 = cmp.eq(r0,0000006A); if (p0.new) r0 = add(r4,00000007); if (!p0.new) jump:t	00005C00 }
7620FF00     	{ r0 = and(r0,FFFFFFF8) }
B0004104 A1A1D200 5800400C 	{ memb(r1) = r4.new; r4 = add(r0,00000008) }
91C0C004     	{ r5:r4 = memd(r0) }
B0044080 A1A1D200 9184C000 	{ memb(r1) = r0.new; r0 = add(r4,00000004) }
7320E004     	{  }
91904160 A1D0C400 	{ memd(r16) = r5:r4; r0 = memw(r16+44) }
10B060C8 BFE07FE4 A1B0D20B 9130C7C0 	{ memb(r16+11) = r4.new; r4 = add(r0,FFFFFFFF); if (p0.new) jump:t	00005BA8; p0 = cmp.gt(r16,00000001) }
5C0058C0 7580CCE0 	{ p0 = cmp.gtu(r0,00000067); if (p0.new) jump:t	00005DA8 }
5C207874 75004C40 43D0C000 	{ if (p0.new) r1:r0 = memd(r16); p0 = cmp.eq(r0,00000062); if (!p0.new) jump:t	00005F18 }
49C0C2C4     	{ r5:r4 = memd(gp+176) }
D3E04400 5800C16A 	{ jump	00005F18; r1:r0 = and(r1:r0,r5:r4) }
59200580     	{ r0 = memw(r16+20); p0 = cmp.eq(r2,00000000) }
B0004021 F3034000 A1B0D505 	{ r0 = add(r3,r0); r1 = add(r0,00000001) }
5C004052 3C00C025 	{ memb(r0) = 25; if (p0) jump:nt	00005CFC }
9190C0A0     	{ r0 = memw(r16+20) }
B0004021 F3034000 A1B0D505 	{ r0 = add(r3,r0); r1 = add(r0,00000001) }
58004046 A100C200 	{ memb(r0) = r2; jump	00005CFC }
9150C060     	{ r0 = memh(r16+6) }
70E04000 26C2C01A 9170C3C1 	{ if (!tstbit(r0.new,-00000001)) jump:nt	00005CB4; r0 = sxth(r0) }
85014000 76014040 2402C022 B0104280 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00005CD4; r0 = and(r1,00000002); p0 = tstbit(r1,00000000) }
9190C0A1     	{ r1 = memw(r16+20) }
B0014024 F3034101 A1B0D405 58004020 	{ memb(r16+5) = r4.new; r1 = add(r3,r1); r4 = add(r1,00000001) }
3C01C02B     	{ memb(r1) = 2B }
B0104280 9190C0A1 	{ r1 = memw(r16+20); r0 = add(r16,00000014) }
B0014024 F3034101 A1B0D405 58004012 	{ memb(r16+5) = r4.new; r1 = add(r3,r1); r4 = add(r1,00000001) }
3C01C02D     	{ memb(r1) = 2D }
5C20400E B0104280 4190C0A1 	{ if (p0) r1 = memw(r16+20); r0 = add(r16,00000014); if (!p0) jump:nt	00005CE8 }
B0014024 F3034101 A1B0D405 3C01C020 	{ memb(r16+5) = r4.new; r1 = add(r3,r1); r4 = add(r1,00000001) }
F5025000 9180C004 	{ r4 = memw(r0); r1:r0 = combine(r2,r16) }
5A004EF0 F3034402 A1B0D204 7800C001 	{ memb(r16+4) = r2.new; r2 = add(r3,r4); call	00007AD0 }
70614000 3E0C1F40 	{ dealloc_return; r17:r16 = memd(r29+8); r0 = r1 }
91904160 26C2C042 9130C7C0 	{ if (!tstbit(r0.new,-00000001)) jump:nt	00005D90; r0 = memw(r16+44) }
5C20580A 75004E20 4381C000 	{ if (p0.new) r0 = memw(r1); p0 = cmp.eq(r0,00000071); if (!p0.new) jump:t	00005D28 }
5800401A B000C0E0 	{ r0 = add(r0,00000007); jump	00005D54 }
5C205810 75004D80 43814000 4781C004 	{ if (!p0.new) r4 = memw(r1); if (p0.new) r0 = memw(r1); p0 = cmp.eq(r0,0000006C); if (!p0.new) jump:t	00005D48 }
B0004084 A1A1D200 5800401A 	{ memb(r1) = r4.new; r4 = add(r0,00000004) }
9180C000     	{ r0 = memw(r0) }
5C205810 740460E0 7500CD40 	{ p0 = cmp.eq(r0,0000006A); if (p0.new) r0 = add(r4,00000007); if (!p0.new) jump:t	00005D68 }
7620FF00     	{ r0 = and(r0,FFFFFFF8) }
B0004104 A1A1D200 5800400C 	{ memb(r1) = r4.new; r4 = add(r0,00000008) }
91C0C004     	{ r5:r4 = memd(r0) }
B0044080 A1A1D200 9184C000 	{ memb(r1) = r0.new; r0 = add(r4,00000004) }
8440C004     	{ r5:r4 = sxtw(r0) }
91904160 A1D0C400 	{ memd(r16) = r5:r4; r0 = memw(r16+44) }
10B060C8 BFE07FE4 A1B0D20B 9130C7C0 	{ memb(r16+11) = r4.new; r4 = add(r0,FFFFFFFF); if (p0.new) jump:t	00005D10; p0 = cmp.gt(r16,00000001) }
5C0058A6 7580CCE0 	{ p0 = cmp.gtu(r0,00000067); if (p0.new) jump:t	00005EDC }
5C207856 58004150 75004C40 4310C000 	{ if (p0.new) r0 = memb(r16); p0 = cmp.eq(r0,00000062); jump	0000603C; if (!p0.new) jump:t	00006044 }
5C004808 7500CF40 	{ p0 = cmp.eq(r0,0000007A); if (p0.new) jump:nt	00005DB8 }
5C2058A8 7500CE80 	{ p0 = cmp.eq(r0,00000074); if (!p0.new) jump:t	00005F00 }
00800184     	{ r4 = memw(r16+4); r0 = memw(r16) }
1601C0AC     	{ jump	00005F14; r1 = 00000000 }
913047C2 9190C160 	{ r0 = memw(r16+44); r2 = memb(r16+62) }
5C2058DC 7E006002 7502CD80 	{ p0 = cmp.eq(r2,0000006C); if (p0.new) r2 = 00000000; if (!p0.new) jump:t	00005F80 }
11C0C110     	{ if (!p0.new) jump:t	00005DF4; p0 = tstbit(r0,00000000) }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B830002 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B360F8 BFE37FE0 A1B0D20B B01D4001 	{ memb(r16+11) = r0.new; r0 = add(r3,FFFFFFFF); if (p0.new) jump:t	00005DD8; p0 = cmp.gt(r19,00000001) }
70704000 6C032802 	{ memw(r29) = r2; r3 = add(r29,00000000); r0 = r16 }
8CC34203 3C50E67F 	{ memw(r16+48) = 0000007F; r3 = setbit(r3,00000004) }
58004060 3C43C000 	{ memw(r3) = 00000000; jump	00005EC8 }
9130C7C0     	{ r0 = memb(r16+62) }
5C0058D4 7580CCE0 	{ p0 = cmp.gtu(r0,00000067); if (p0.new) jump:t	00005FBC }
5C20787C 7500CC40 	{ p0 = cmp.eq(r0,00000062); if (!p0.new) jump:t	00006114 }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B260F8 BFE27FE3 A1B0D30B 	{ r3 = add(r2,FFFFFFFF); if (p0.new) jump:t	00005E24; p0 = cmp.gt(r18,00000001) }
59FF7F5E 919041A1 A1A0C300 	{ r1 = memw(r16+52); jump	00005CFC }
7C004004 91904160 26C2C016 9181C000 	{ if (!tstbit(r0.new,-00000001)) jump:nt	00005E80; r0 = memw(r16+44); r5:r4 = combine(00000000,00000000) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B260F8 BFE27FE4 A1B0D20B 7320E004 	{ memb(r16+11) = r4.new; r4 = add(r2,FFFFFFFF); if (p0.new) jump:t	00005E58; p0 = cmp.gt(r18,00000001) }
73306F00     	{  }
919040A2 A1D0C400 	{ memd(r16) = r5:r4; r2 = memw(r16+20) }
580041AC F303C202 	{ r2 = add(r3,r2); jump	000061DC }
913047C2 9190C160 	{ r0 = memw(r16+44); r2 = memb(r16+62) }
5C2058BC 74106582 7502CD80 	{ p0 = cmp.eq(r2,0000006C); if (p0.new) r2 = add(r16,0000002C); if (!p0.new) jump:t	0000600C }
11C04112 7800C000 	{ r0 = 00000000; if (!p0.new) jump:t	00005EC4; p0 = tstbit(r0,00000000) }
9181C000     	{ r0 = memw(r1) }
B0004083 A1A1D300 	{ r3 = add(r0,00000004) }
00230000     	{ r0 = memw(r0); r3 = memw(r2) }
10B360F8 BFE37FE4 A1A2D200 F500D000 	{ memb(r2) = r4.new; r4 = add(r3,FFFFFFFF); if (p0.new) jump:t	00005EA8; p0 = cmp.gt(r19,00000001) }
5A00C19C     	{ call	00006200 }
78DF7FE1 7560FFE0 	{ p0 = cmp.gt(r0,FFFFFFFF); r1 = FFFFFFFF }
59FF7F16 7E00C001 	{ if (p0) r1 = 00000000; jump	00005D00 }
5C0048AE 75004D00 4350C000 	{ if (p0.new) r0 = memh(r16); p0 = cmp.eq(r0,00000068); if (p0.new) jump:nt	00006038 }
5C005808 7500CE80 	{ p0 = cmp.eq(r0,00000074); if (p0.new) jump:t	00005EF8 }
5C2058AA 7500CF40 	{ p0 = cmp.eq(r0,0000007A); if (!p0.new) jump:t	00006044 }
580040A0 9190C000 	{ r0 = memw(r16); jump	00006038 }
5C20580C 75004D00 43D0C000 	{ if (p0.new) r1:r0 = memd(r16); p0 = cmp.eq(r0,00000068); if (!p0.new) jump:t	00005F18 }
49C0C2E4     	{ r5:r4 = memd(gp+184) }
D3E0C400     	{ r1:r0 = and(r1:r0,r5:r4) }
A1D0C000     	{ memd(r16) = r1:r0 }
7C004004 9130C780 	{ r0 = memb(r16+60); r5:r4 = combine(00000000,00000000) }
76004100 2402C024 91D0C000 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00005F6C; r0 = and(r0,00000008) }
D2804400 5C00C81E 	{ if (p0.new) jump:nt	00005F6C; p0 = cmp.eq(r1:r0,r5:r4) }
8CC2C500     	{ r0 = setbit(r2,0000000A) }
5C204818 75004F00 4390C0A1 	{ if (p0.new) r1 = memw(r16+20); p0 = cmp.eq(r0,00000078); if (!p0.new) jump:nt	00005F68 }
F3034100 B0014021 A1B0D305 	{ r1 = add(r1,00000001); r0 = add(r3,r1) }
3C00C030     	{ memb(r0) = 30 }
9190C0A0     	{ r0 = memw(r16+20) }
B0004021 F3034000 A1B0D505 	{ r0 = add(r3,r0); r1 = add(r0,00000001) }
A100C200     	{ memb(r0) = r2 }
9190C0A0     	{ r0 = memw(r16+20) }
F3034000 A1B0D204 5A0050CE 	{ memb(r16+4) = r0.new; r0 = add(r3,r0) }
F502D000     	{ r1:r0 = combine(r2,r16) }
59FFFEC0     	{ jump	00005CFC }
11C04112 7800C000 	{ r0 = 00000000; if (!p0.new) jump:t	00005FA4; p0 = tstbit(r0,00000000) }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B260F8 BFE27FE4 A1B0D20B 9190C0A1 	{ memb(r16+11) = r4.new; r4 = add(r2,FFFFFFFF); if (p0.new) jump:t	00005F88; p0 = cmp.gt(r18,00000001) }
B0014022 F3034101 A1B0D405 59FF7EA4 	{ memb(r16+5) = r2.new; r1 = add(r3,r1); r2 = add(r1,00000001) }
A101C000     	{ memb(r1) = r0 }
5C005864 7580CF20 	{ p0 = cmp.gtu(r0,00000079); if (p0.new) jump:t	00006084 }
5C005880 7580CE60 	{ p0 = cmp.gtu(r0,00000073); if (p0.new) jump:t	000060C4 }
5C005890 7580CE00 	{ p0 = cmp.gtu(r0,00000070); if (p0.new) jump:t	000060EC }
5C0048B4 7500CD00 	{ p0 = cmp.eq(r0,00000068); if (p0.new) jump:nt	0000613C }
5C0058C4 7500CD40 	{ p0 = cmp.eq(r0,0000006A); if (p0.new) jump:t	00006164 }
5C205898 7500CD80 	{ p0 = cmp.eq(r0,0000006C); if (!p0.new) jump:t	00006114 }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B260F8 BFE27FE3 A1B0D30B 	{ r3 = add(r2,FFFFFFFF); if (p0.new) jump:t	00005FEC; p0 = cmp.gt(r18,00000001) }
5800C094     	{ jump	00006130 }
11C041C2     	{  }
4790C080     	{ if (!p0.new) r0 = memw(r16+16) }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
75424000 73230480 	{ memw(r16+16) = r0; r3 = r2; p0 = cmp.gt(r2,00000000) }
5CDF60F4 A190C30B 	{ memw(r16+44) = r3; if (p0) jump:nt	00006014 }
5800C0AE     	{ jump	00006190 }
8440C000     	{ r1:r0 = sxtw(r0) }
58004006 A1D0C000 	{ memd(r16) = r1:r0; jump	00006048 }
91D0C000     	{ r1:r0 = memd(r16) }
7C7FFFE4     	{ r5:r4 = combine(FFFFFFFF,FFFFFFFF) }
D2804440 5C204830 74906280 4370C3C1 	{ if (p0.new) r1 = memuh(r16+60); if (!p0.new) r0 = add(r16,00000014); if (!p0.new) jump:nt	000060B0; p0 = cmp.gt(r1:r0,r5:r4) }
85014000 76014040 2402C0AC B0104280 	{ if (cmp.eq(r0.new,00000001)) jump:nt	000061BC; r0 = and(r1,00000002); p0 = tstbit(r1,00000000) }
9190C0A1     	{ r1 = memw(r16+20) }
B0014024 F3034101 A1B0D405 580040AA 	{ memb(r16+5) = r4.new; r1 = add(r3,r1); r4 = add(r1,00000001) }
3C01C02B     	{ memb(r1) = 2B }
5C205848 7500CF40 	{ p0 = cmp.eq(r0,0000007A); if (!p0.new) jump:t	00006114 }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B260F8 BFE27FE3 A1B0D30B 	{ r3 = add(r2,FFFFFFFF); if (p0.new) jump:t	0000608C; p0 = cmp.gt(r18,00000001) }
5800C044     	{ jump	00006130 }
9190C0A1     	{ r1 = memw(r16+20) }
B0014024 F3034101 A1B0D405 5800408A 	{ memb(r16+5) = r4.new; r1 = add(r3,r1); r4 = add(r1,00000001) }
3C01C02D     	{ memb(r1) = 2D }
5C205828 7500CE80 	{ p0 = cmp.eq(r0,00000074); if (!p0.new) jump:t	00006114 }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B260F8 BFE27FE3 A1B0D30B 	{ r3 = add(r2,FFFFFFFF); if (p0.new) jump:t	000060CC; p0 = cmp.gt(r18,00000001) }
5800C024     	{ jump	00006130 }
5C205814 7500CE20 	{ p0 = cmp.eq(r0,00000071); if (!p0.new) jump:t	00006114 }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B260F8 BFE27FE3 A1B0D30B 	{ r3 = add(r2,FFFFFFFF); if (p0.new) jump:t	000060F4; p0 = cmp.gt(r18,00000001) }
5800C038     	{ jump	00006180 }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B260F8 BFE27FE3 A1B0D30B 	{ r3 = add(r2,FFFFFFFF); if (p0.new) jump:t	00006114; p0 = cmp.gt(r18,00000001) }
59FF7DE6 919041A1 A1A0D300 	{ r1 = memw(r16+52); jump	00005CFC }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B260F8 BFE27FE3 A1B0D30B 	{ r3 = add(r2,FFFFFFFF); if (p0.new) jump:t	0000613C; p0 = cmp.gt(r18,00000001) }
59FF7DD2 919041A1 A1A0CB00 	{ r1 = memw(r16+52); jump	00005CFC }
9181C000     	{ r0 = memw(r1) }
B0004082 A1A1D200 0B820000 	{ memb(r1) = r2.new; r2 = add(r0,00000004) }
10B260F8 BFE27FE3 A1B0D30B 	{ r3 = add(r2,FFFFFFFF); if (p0.new) jump:t	00006164; p0 = cmp.gt(r18,00000001) }
9190C1A1     	{ r1 = memw(r16+52) }
8441C002     	{ r3:r2 = sxtw(r1) }
59FF7DBA A1C0C200 	{ memd(r0) = r3:r2; jump	00005CFC }
78004001 91904182 26C2C02C 5A00C8DA 	{ if (!tstbit(r2.new,-00000001)) jump:nt	000061F0; r2 = memw(r16+48); r1 = 00000000 }
1000402A 4790C081 	{ if (!p0.new) r1 = memw(r16+16); if (p0.new) jump:nt	000061F4; p0 = cmp.eq(r0,00000000) }
59FF7DAA F3214000 A1B0D207 5C20400E 	{ memb(r16+7) = r0.new; r0 = sub(r0,r1); jump	00005CFC }
B0104280 4190C0A1 	{ if (p0) r1 = memw(r16+20); r0 = add(r16,00000014) }
B0014024 F3034101 A1B0D405 3C01C020 	{ memb(r16+5) = r4.new; r1 = add(r3,r1); r4 = add(r1,00000001) }
F5025000 9180C004 	{ r4 = memw(r0); r1:r0 = combine(r2,r16) }
F303C402     	{ r2 = add(r3,r4) }
5A004F9A A190C204 	{ memw(r16+16) = r2; call	00008110 }
59FFFD8C     	{ jump	00005CFC }
5A00CA7C     	{ call	000076E0 }
59FF7D88 A190C007 	{ memw(r16+28) = r0; jump	00005CFC }
59FF7D84 91904180 A1B0D207 5A00603C 	{ memb(r16+7) = r0.new; r0 = memw(r16+48); jump	00005CFC }

;; _Putstr: 00006200
_Putstr proc
5A00603C A09DC00E 	{ allocframe(+00000070); call	0000A278 }
70604011 301A3C07 	{ r23:r22 = combine(00000000,00000000); r18 = r1; r17 = r0 }
5A004250 0C9C0E9B 	{ r19 = memw(r17+24); r20 = memw(r17+16); call	000066B0 }
B01D4210 78004816 780B2A07 	{ memd(r29) = r23:r22; r19 = add(r19,r0); r22 = 00000040; r16 = add(r29,00000010) }
9131C781     	{ r1 = memb(r17+60) }
76014081 2443E012 	{ r1 = and(r1,00000004) }
78004821 2103F30E 	{ r1 = 00000041 }
5A0047F4 78DF7FF5 30B030BE 	{ r22 = r19; r0 = r19; r21 = FFFFFFFF; call	00007220 }
70604010 2402C066 75747FE0 	{ if (cmp.eq(r16.new,00000000)) jump:nt	00006314; r16 = r0 }
07FF7FFF 7800C7E0 F4144017 24C3C046 	{ r23 = mux(p0,r20,r0); r0 = 7FFFFFFF }
280B3A0D     	{ r21 = -00000001; r19 = 00000000 }
5A0045C6 B01D4002 4C2000A1 	{ r1 = memw(r18); r0 = add(r29,00000008); r2 = add(r29,00000000); call	00006DF0 }
70604014 26C2C048 91924000 	{ if (!tstbit(r20.new,-00000001)) jump:nt	00006304; r20 = r0 }
2442E008 BFF47FF4 	{ if (!cmp.eq(r0.new,00000000)) jump:t	0000628C }
26C2C040 148C4F2A 	{ if (!tstbit(r20.new,-00000001)) jump:nt	00006304 }
4791C0A0     	{ if (!p0.new) r0 = memw(r17+20) }
F3005401 2143F610 	{ r1 = add(r0,r20) }
5A004044 F5105100 3C51C700 	{ memw(r17+56) = 00000000; r1:r0 = combine(r16,r17); call	00006320 }
70604013 26C3C03A 	{ r19 = r0 }
68003095     	{ memw(r17+20) = 00000000; r0 = 00000000 }
5A004898 70744002 2C213880 	{ r0 = add(r0,r16); r1 = add(r29,00000008); r2 = r20; call	000073E0 }
9191C0A0     	{ r0 = memw(r17+20) }
F3005400 A1B1D205 B0124092 	{ memb(r17+5) = r0.new; r0 = add(r0,r20) }
91924000 2402C00A F3345717 	{ if (cmp.eq(r0.new,00000000)) jump:nt	000062E4; r0 = memw(r18) }
24B3E0C8     	{  }
104B6012 7073C015 	{ r21 = r19; if (!p0.new) jump:t	00006300; p0 = cmp.eq(r11,00000000) }
5A00401E F510D100 	{ r1:r0 = combine(r16,r17); call	00006320 }
7060C015     	{ r21 = r0 }
75154000 7E006015 39514700 3951C280 	{ if (p0.new) memw(r17+20) = 00000000; if (p0.new) memw(r17+56) = 00000000; if (p0.new) r21 = 00000000; p0 = cmp.eq(r21,00000000) }
B01D4200 2002D008 5A004674 	{ if (cmp.eq(r0.new,r16)) jump:nt	00006314; r0 = add(r29,00000010) }
7070C000     	{ r0 = r16 }
00004100 170DC060 173BCDF4 	{ jump	00006300; r13 = r11; jump	000063D0; r0 = r13 }
7F00C000     	{ nop }

;; _Puttxt: 00006320
_Puttxt proc
5A005FAC A09DC004 	{ allocframe(+00000020); call	0000A278 }
F501C010     	{ r17:r16 = combine(r1,r0) }
0E830582     	{ r2 = memw(r16+20); r3 = memw(r16+24) }
F3224303 06800784 	{ r4 = memw(r16+28); r0 = memw(r16+24); r3 = sub(r3,r2) }
F3204305 08830980 	{ r0 = memw(r16+4); r3 = memw(r16); r5 = sub(r3,r0) }
F3244504 9170C3C5 	{ r5 = memuh(r16+60); r4 = sub(r5,r4) }
F3234404 9190C143 	{ r3 = memw(r16+40); r4 = sub(r4,r3) }
F320C400     	{ r0 = sub(r4,r0) }
F3234014 76054085 2443E028 	{ r5 = and(r5,00000004); r20 = sub(r0,r3) }
10CCE022     	{ if (!p0.new) jump:t	000063A4; p0 = cmp.gt(r12,00000000) }
2A0D30CE     	{ r22 = r20; r21 = 00000020 }
D5B65593 24C3E018 	{ r19 = minu(r22,r21) }
00004345 78004001 03800283 	{ r3 = memw(r16+8); r0 = memw(r16+12); r1 = 0000D140 }
50A34000 30B23A0A 	{ r18 = -00000001; r2 = r19; callr	r3 }
100040E4 A190C003 	{ memw(r16+12) = r0; if (p0.new) jump:nt	0000654C; p0 = cmp.eq(r0,00000001) }
9190C1A0     	{ r0 = memw(r16+52) }
F3005300 A1B0D20D F3335616 	{ memb(r16+13) = r0.new; r0 = add(r0,r19) }
24B2E0E8 9190C0A2 	{ if (cmp.gt(r22.new,00000001)) jump:t	0000636C }
10C2C014     	{ if (!p0.new) jump:nt	000063CC; p0 = cmp.gt(r2,00000000) }
70714001 78DF7FF2 03800283 	{ r3 = memw(r16+8); r0 = memw(r16+12); r18 = FFFFFFFF; r1 = r17 }
50A3C000     	{ callr	r3 }
100040CA A190C003 	{ memw(r16+12) = r0; if (p0.new) jump:nt	0000654C; p0 = cmp.eq(r0,00000001) }
0D800581     	{ r1 = memw(r16+20); r0 = memw(r16+20) }
F3004100 A1B0D20D 78004415 	{ memb(r16+13) = r0.new; r0 = add(r0,r1) }
919040D3 24C3C022 	{ r19 = memw(r16+24) }
D5B35591 24C3E018 	{ r17 = minu(r19,r21) }
00004345 78004501 03800283 	{ r3 = memw(r16+8); r0 = memw(r16+12); r1 = 0000D168 }
50A34000 30923A0A 	{ r18 = -00000001; r2 = r17; callr	r3 }
100040AC A190C003 	{ memw(r16+12) = r0; if (p0.new) jump:nt	0000654C; p0 = cmp.eq(r0,00000001) }
9190C1A0     	{ r0 = memw(r16+52) }
F3005100 A1B0D20D F3315313 	{ memb(r16+13) = r0.new; r0 = add(r0,r17) }
24B3E0E8     	{  }
919040E2 24C2C016 78DF7FF2 	{ if (!cmp.gt(r2.new,00000000)) jump:nt	00006440; r2 = memw(r16+28) }
03800481     	{ r1 = memw(r16+16); r0 = memw(r16+12) }
9190C043     	{ r3 = memw(r16+8) }
50A3C000     	{ callr	r3 }
10004092 A190C003 	{ memw(r16+12) = r0; if (p0.new) jump:nt	0000654C; p0 = cmp.eq(r0,00000001) }
0D800781     	{ r1 = memw(r16+28); r0 = memw(r16+20) }
F3004100 A1B0D20D 78004415 	{ memb(r16+13) = r0.new; r0 = add(r0,r1) }
91904113 24C3C022 	{ r19 = memw(r16+32) }
D5B35591 24C3E018 	{ r17 = minu(r19,r21) }
00004345 78004501 03800283 	{ r3 = memw(r16+8); r0 = memw(r16+12); r1 = 0000D168 }
50A34000 30923A0A 	{ r18 = -00000001; r2 = r17; callr	r3 }
10004074 A190C003 	{ memw(r16+12) = r0; if (p0.new) jump:nt	0000654C; p0 = cmp.eq(r0,00000000) }
9190C1A0     	{ r0 = memw(r16+52) }
F3005100 A1B0D20D F3315313 	{ memb(r16+13) = r0.new; r0 = add(r0,r17) }
24B3E0E8     	{  }
91904122 24C2C018 78DF7FF2 	{ if (!cmp.gt(r2.new,00000000)) jump:nt	000064B4; r2 = memw(r16+36) }
04810783     	{ r3 = memw(r16+28); r1 = memw(r16+16) }
F3014301 03800283 	{ r3 = memw(r16+8); r0 = memw(r16+12); r1 = add(r1,r3) }
50A3C000     	{ callr	r3 }
10004058 A190C003 	{ memw(r16+12) = r0; if (p0.new) jump:nt	0000654C; p0 = cmp.eq(r0,00000000) }
0D800981     	{ r1 = memw(r16+4); r0 = memw(r16+20) }
F3004100 A1B0D20D 78004415 	{ memb(r16+13) = r0.new; r0 = add(r0,r1) }
7F004000 91904153 24C3C024 	{ r19 = memw(r16+40); nop }
D5B35591 24C3E018 	{ r17 = minu(r19,r21) }
00004345 78004501 03800283 	{ r3 = memw(r16+8); r0 = memw(r16+12); r1 = 0000D168 }
50A34000 30923A0A 	{ r18 = -00000001; r2 = r17; callr	r3 }
10004038 A190C003 	{ memw(r16+12) = r0; if (p0.new) jump:nt	0000654C; p0 = cmp.eq(r0,00000000) }
9190C1A0     	{ r0 = memw(r16+52) }
F3005100 A1B0D20D F3315313 	{ memb(r16+13) = r0.new; r0 = add(r0,r17) }
24B3E0E8     	{  }
75544000 9130C780 	{ r0 = memb(r16+60); p0 = cmp.gt(r20,00000000) }
76004080 2402E024 5C204020 	{ if (cmp.eq(r0.new,00000000)) jump:t	0000654C; r0 = and(r0,00000004) }
7E00C413     	{ if (p0) r19 = 00000020 }
D5B45391 24C3E018 	{ r17 = minu(r20,r19) }
00004345 78004001 03800283 	{ r3 = memw(r16+8); r0 = memw(r16+12); r1 = 0000D140 }
50A34000 30923A0A 	{ r18 = -00000001; r2 = r17; callr	r3 }
10004010 A190C003 	{ memw(r16+12) = r0; if (p0.new) jump:nt	0000654C; p0 = cmp.eq(r0,00000000) }
9190C1A0     	{ r0 = memw(r16+52) }
F3005100 A1B0D20D F3315414 	{ memb(r16+13) = r0.new; r0 = add(r0,r17) }
24B2E0E8 7800C012 	{ if (cmp.gt(r20.new,00000001)) jump:t	00006514 }
000040F7 170AC068 7F00C000 	{ nop; jump	0000661C; r0 = r10 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Tls_get__Mbcurmax: 00006560
_Tls_get__Mbcurmax proc
78004021 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r1 = 00000001 }
00004400 69482A05 00004400 7800C312 9210C000 	{ r0 = memw_locked(r16); r18 = 00010018; memd(r29) = r19:r18; r16 = 00000014 }
7540C000     	{ p0 = cmp.gt(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	0000658C }
A0B0C100     	{ memw_locked(r16,p0) = r1 }
5CFFE0F8     	{ if (!p0) jump:nt	00006578 }
10406012 7E00E051 	{ if (p0.new) r17 = 00000002; if (!p0.new) jump:t	000065B0; p0 = cmp.eq(r0,00000000) }
00004400 78004300 000041BF 7800C601 5A00E5D6 	{ call	0000B150; r1 = 00006FF0; r0 = 00010018 }
7F004000 A190D100 	{ memw(r16) = r17; nop }
91904000 24C2E100 5A006654 	{ if (!cmp.gt(r0.new,00000002)) jump:t	000065B4; r0 = memw(r16) }
9192C000     	{ r0 = memw(r18) }
7C006020 70604010 2442E01C 5A00C43A 	{ if (!cmp.eq(r16.new,00000000)) jump:t	00006600; r16 = r0; r1:r0 = combine(00000001,00000001) }
7300E010     	{  }
1009C012     	{ if (p0.new) jump:nt	000065F8; p0 = cmp.eq(r9,00000000) }
5A006626 509100A0 	{ r0 = memw(r18); r1 = r17; call	0000B224 }
1000400A 74116010 7491E000 	{ if (!p0.new) r0 = add(r17,00000000); if (p0.new) r16 = add(r17,00000000); if (p0.new) jump:nt	000065F4; p0 = cmp.eq(r0,00000000) }
5A00C502     	{ call	00006FF0 }
5800C004     	{ jump	000065F8 }
3C11C006     	{ memb(r17) = 06 }
70704000 3E0C1E05 	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r16 }
961EC01E     	{ dealloc_return }

;; _Tls_get__Mbstate: 00006604
_Tls_get__Mbstate proc
78004021 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r1 = 00000001 }
00004400 78004390 00004400 7800C411 A1DDD200 	{ memd(r29) = r19:r18; r17 = 00010020; r16 = 0001001C }
9210C000     	{ r0 = memw_locked(r16) }
7540C000     	{ p0 = cmp.gt(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	00006634 }
A0B0C100     	{ memw_locked(r16,p0) = r1 }
5CFFE0F8     	{ if (!p0) jump:nt	00006620 }
10406010 7E00E052 	{ if (p0.new) r18 = 00000002; if (!p0.new) jump:t	00006654; p0 = cmp.eq(r0,00000000) }
00004400 78004400 000041BF 7800C601 5A00E582 	{ call	0000B150; r1 = 00006FF0; r0 = 00010020 }
A190D200     	{ memw(r16) = r18 }
91904000 24C2E100 5A006602 	{ if (!cmp.gt(r0.new,00000002)) jump:t	00006658; r0 = memw(r16) }
9191C000     	{ r0 = memw(r17) }
7C006800 70604010 2442E020 5A00C3E8 	{ if (!cmp.eq(r16.new,00000000)) jump:t	000066AC; r16 = r0; r1:r0 = combine(00000040,00000001) }
70604010 2402C018 5A0065D4 	{ if (cmp.eq(r16.new,00000000)) jump:nt	000066A8; r16 = r0 }
50810090     	{ r0 = memw(r17); r1 = r16 }
1000C008     	{ if (p0.new) jump:nt	00006694; p0 = cmp.eq(r0,00000000) }
5A0044B4 28083080 	{ r0 = r16; r16 = 00000000; call	00006FF0 }
5800C00A     	{ jump	000066A4 }
5A005E76 00004346 73306180 	{ call	0000A380 }
7800C802     	{ r2 = 00000040 }
70704000 3E0C1E05 	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r16 }
961EC01E     	{ dealloc_return }

;; _Getmbcurmax: 000066B0
_Getmbcurmax proc
5BFF7F58 A09DC000 	{ allocframe(+00000000); call	00006560 }
10003F40     	{ dealloc_return; r0 = memb(r0) }

;; _Getpmbstate: 000066BC
_Getpmbstate proc
59FFFFA4     	{ jump	00006604 }

;; _Stoulx: 000066C0
_Stoulx proc
5A005DD8 79303C80 	{ allocframe(00000040); p0 = cmp.eq(r3,00000000); call	0000A270 }
89404004 F5024012 70182823 	{ memw(r29+8) = r3; r16 = r1; r19:r18 = combine(r2,r0); r4 = p0 }
BFF27FF6 38C34000 A19DC401 	{ memw(r29+4) = r4; if (!p0) memw(r3) = 00000000; r22 = add(r18,FFFFFFFF) }
5A00495C 9136C031 	{ r17 = memb(r22+1); call	00007998 }
B0164036 3A60D180 	{ r0 = memuh(r13+r17<<#1); r22 = add(r22,00000001) }
76006880 2472E0F8 7076C001 	{ if (!cmp.eq(r0.new,00000001)) jump:t	000066E4; r0 = and(r0,00000144) }
9B21C020     	{ r0 = memb(r1++#1) }
5C20580A 7E806575 7500C560 	{ p0 = cmp.eq(r0,0000002B); if (!p0.new) r21 = 0000002B; if (!p0.new) jump:t	00006714 }
17004D0A 7061C016 	{ r22 = r1; jump	00006720; r13 = r0 }
750045A0 7401E016 	{ if (p0.new) r22 = add(r1,00000000); p0 = cmp.eq(r0,0000002D) }
7400C015     	{ if (p0) r21 = add(r0,00000000) }
11CBE108     	{ if (!p0.new) jump:t	00006730; p0 = tstbit(r11,00000000) }
100BE106     	{ if (p0.new) jump:t	00006730; p0 = cmp.eq(r11,00000002) }
5C20580C 7553C480 	{ p0 = cmp.gt(r19,00000024); if (!p0.new) jump:t	00006740 }
100840B0 7800C000 	{ r0 = 00000000; if (p0.new) jump:nt	00006890; p0 = cmp.eq(r8,00000001) }
58005E0C A190D200 	{ memw(r16) = r18; jump	0000A350 }
10CBC014     	{ if (!p0.new) jump:nt	00006768; p0 = cmp.gt(r11,00000000) }
104BF022     	{ if (!p0.new) jump:t	00006788; p0 = cmp.eq(r11,00000000) }
490B10E0     	{ r0 = memb(r22); r19 = 00000010 }
5C20581E 75004600 4316C020 	{ if (p0.new) r0 = memb(r22+1); p0 = cmp.eq(r0,00000030); if (!p0.new) jump:t	00006788 }
8CC0C500     	{ r0 = setbit(r0,0000000A) }
DD004F00 58004016 7416E056 	{ if (p0.new) r22 = add(r22,00000002); jump	0000678C; p0 = cmpb.eq(r0,78) }
48AB10E0     	{ r0 = memb(r22); r19 = 0000000A }
5C20580E 7500C600 	{ p0 = cmp.eq(r0,00000030); if (!p0.new) jump:t	00006788 }
488B31E0     	{ r0 = memb(r22+1); r19 = 00000008 }
8CC0C500     	{ r0 = setbit(r0,0000000A) }
DD004F00 74166056 7E00E213 	{ if (p0.new) r19 = 00000010; if (p0.new) r22 = add(r22,00000002); p0 = cmpb.eq(r0,78) }
58004006 7076C019 	{ r25 = r22; jump	00006794 }
B019C039     	{ r25 = add(r25,00000001) }
9139C011     	{ r17 = memb(r25) }
5CDF78FC 7511C600 	{ p0 = cmp.eq(r17,00000030); if (p0.new) jump:t	00006790 }
5A00C21C     	{ call	00006BD8 }
00004347 73006200 	{  }
7073C002     	{ r2 = r19 }
5A0045D0 00004347 7C00421A 3A41D181 	{ r1 = memh(r29+r17<<#1); r27:r26 = combine(0000D1D0,00000000); call	00007350 }
70794018 70604014 7F004000 2404C026 5A004204 	{ if (cmp.eq(r20.new,00000000)) jump:nt	00006818; nop; r20 = r0; r24 = r25 }
9138C031     	{ r17 = memb(r24+1) }
00004347 73006200 	{  }
F33B5403 7073C002 	{ r2 = r19; r3 = sub(r20,r27) }
B0184038 707A4011 76035FF7 3A41D181 	{ r1 = memh(r29+r17<<#1); r23 = and(r3,000000FF); r17 = r26; r24 = add(r24,00000001) }
5A0045AC E313DA17 	{ r26 = add(r23,mpyi(r26,r19)); call	00007350 }
70604014 2472E0E8 5800C006 	{ if (!cmp.eq(r20.new,00000001)) jump:t	000067D4; r20 = r0 }
707A4017 7079C018 	{ r24 = r25; r23 = r26 }
5C004836 FB397880 F216D800 	{ p0 = cmp.eq(r22,r24); if (!p0.new) r0 = sub(r24,r25); if (p0.new) jump:nt	00006880 }
00004347 9D33DE01 F3214000 26C2C020 1080600C 	{ if (!tstbit(r0.new,-00000001)) jump:nt	0000686C; r0 = sub(r0,r1); r1 = memb(r19+0000D1F8) }
7493E001     	{ if (!p0.new) r1 = add(r19,00000000) }
F3375A00 2102FA08 5A00DE80 	{ if (cmp.gtu(r0.new,r26)) jump:t	0000684C; r0 = sub(r26,r23) }
1400E912     	{ if (p0.new) jump:t	00006868; p0 = cmp.eq(r0,-00000001) }
5A00C364     	{ call	00006F10 }
78DF7FFA 78004575 919D4020 3C40C022 	{ memw(r0) = 00000022; r0 = memw(r29+4); r21 = 0000002B; r26 = FFFFFFFF }
85404000 479DC040 	{ if (!p0.new) r0 = memw(r29+8); p0 = r0 }
38C0C001     	{ if (!p0) memw(r0) = 00000001 }
751545A0 765A4000 7510C001 	{ p1 = cmp.eq(r16,00000000); r0 = sub(00000000,r26); p0 = cmp.eq(r21,0000002D) }
5800400E 749A4000 4490D801 	{ if (!p0) r0 = add(r26,00000000); jump	00006890 }
10084008 7800C000 	{ r0 = 00000000; if (p0.new) jump:nt	00006890; p0 = cmp.eq(r8,00000000) }
58005D64 A190D200 	{ memw(r16) = r18; jump	0000A350 }
5800DD60     	{ jump	0000A350 }

;; _Stoul: 00006894
_Stoul proc
1633C016     	{ jump	000066C0; r3 = 00000000 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Clearlocks: 000068A0
_Clearlocks proc
78004000 7F004000 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; nop; r0 = 00000000 }
000043A3 7C904290 00004400 4800C024 7F00C000 	{ nop; memb(gp+4) = r0; r17:r16 = combine(0000E8D4,00000020) }
5A006358 BFF17FF1 20483080 	{ r0 = r16; r16 = add(r16,00000004); r17 = add(r17,FFFFFFFF); call	0000AF70 }
1079E0FA     	{ if (!p0.new) jump:t	000068C0; p0 = cmp.eq(r25,00000001) }
78004000 000043A4 7C98C090 00004400 4800C028 5A006346 BFF17FF1 20483080 	{ r0 = r16; r16 = add(r16,00000004); r17 = add(r17,FFFFFFFF); call	0000AF70; memb(gp+4) = r0; r17:r16 = combine(0000E904,00000030); r0 = 00000000 }
1079E0FA     	{ if (!p0.new) jump:t	000068E4; p0 = cmp.eq(r25,00000001) }
7F004000 7F004000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); nop; nop }

;; _Initlocks: 00006900
_Initlocks proc
000043A3 7C904290 7F004000 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; nop; r17:r16 = combine(0000E8D4,00000020) }
5A006328 BFF17FF1 20483080 	{ r0 = r16; r16 = add(r16,00000004); r17 = add(r17,FFFFFFFF); call	0000AF60 }
1079E0FA     	{ if (!p0.new) jump:t	00006910; p0 = cmp.eq(r25,00000001) }
78004020 000043A4 7C98C090 00004400 4800C024 5A006316 BFF17FF1 20483080 	{ r0 = r16; r16 = add(r16,00000004); r17 = add(r17,FFFFFFFF); call	0000AF60; memb(gp+4) = r0; r17:r16 = combine(0000E904,00000030); r0 = 00000001 }
1079E0FA     	{ if (!p0.new) jump:t	00006934; p0 = cmp.eq(r25,00000001) }
78004020 91DD4010 00004400 48A0C428 961EC01E 	{ r17:r16 = memd(r29); r0 = 00000001 }

;; _Lockfilelock: 00006958
_Lockfilelock proc
5A005CB8 A09DC003 	{ allocframe(+00000018); call	0000A2C8 }
70604010 00004400 2ACC2811 000043A3 78004412 00004400 7800C493 9214C000 	{ r0 = memw_locked(r20); r19 = 00010024; r18 = 0000E8E0; r1 = 00000001; r20 = 0000002C; r16 = r0 }
7540C000     	{ p0 = cmp.gt(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	00006990 }
A0B4C100     	{ memw_locked(r20,p0) = r1 }
5CFFE0F8     	{ if (!p0) jump:nt	0000697C }
1040E02A     	{ if (!p0.new) jump:t	000069E4; p0 = cmp.eq(r0,00000000) }
7F004000 000043A3 2A09294D 5A0062E0 BFF57FF5 20493090 	{ r0 = r17; r17 = add(r17,00000004); r21 = add(r21,FFFFFFFF); call	0000AF60; r21 = 00000014; r17 = 00000020; nop }
107DE0FA     	{ if (!p0.new) jump:t	000069A0; p0 = cmp.eq(r29,00000001) }
000043A4 78004611 2810284D 	{ r21 = 00000004; r0 = 00000001; r17 = 0000E930 }
A113C000     	{ memb(r19) = r0 }
5A0062D0 BFF57FF5 20493090 	{ r0 = r17; r17 = add(r17,00000004); r21 = add(r21,FFFFFFFF); call	0000AF60 }
107DE0FA     	{ if (!p0.new) jump:t	000069C0; p0 = cmp.eq(r29,00000001) }
78004020 78004041 00004400 48A0C428 A194C100 91944000 24C2E100 1008C00E 	{ if (!cmp.gt(r0.new,00000002)) jump:t	000069E8; r0 = memw(r20); memb(gp+132) = r1.new; r1 = 00000002; r0 = 00000001 }
128030B1     	{ r1 = memb(r19); r0 = memb(r16+2) }
8541C000     	{ p0 = r1 }
C4005240 7A004281 2143E008 	{ r1 = mux(p0,00000014,00000000); r0 = addasl(r18,r0,00000002) }
5A00E276     	{ call	0000AEF0 }
58005CAC 7F00C000 	{ nop; jump	0000A360 }

;; _Unlockfilelock: 00006A10
_Unlockfilelock proc
10004018 A09DC000 	{ allocframe(+00000000); if (p0.new) jump:nt	00006A40; p0 = cmp.eq(r0,00000000) }
00004400 49004481 9120C040 	{ r0 = memb(r0+2); r1 = memb(gp+36) }
8541C000     	{ p0 = r1 }
7A004281 2143E00C 	{ r1 = mux(p0,00000014,00000000) }
000043A3 7800C401 C4004140 5A00E27C 	{ call	0000AF34; r0 = addasl(r1,r0,00000002); r1 = 0000E8E0 }
961EC01E     	{ dealloc_return }

;; _Locksyslock: 00006A44
_Locksyslock proc
5A005C42 A09DC003 	{ allocframe(+00000018); call	0000A2C8 }
70604010 00004400 2ACC2811 000043A4 78004612 00004400 7800C513 9214C000 	{ r0 = memw_locked(r20); r19 = 00010028; r18 = 0000E930; r1 = 00000001; r20 = 0000002C; r16 = r0 }
7540C000     	{ p0 = cmp.gt(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	00006A7C }
A0B4C100     	{ memw_locked(r20,p0) = r1 }
5CFFE0F8     	{ if (!p0) jump:nt	00006A68 }
1040E02C     	{ if (!p0.new) jump:t	00006AD4; p0 = cmp.eq(r0,00000000) }
7F004000 7F004000 000043A3 2A09294D 5A006268 BFF57FF5 20493090 	{ r0 = r17; r17 = add(r17,00000004); r21 = add(r21,FFFFFFFF); call	0000AF60; r21 = 00000014; r17 = 00000020; nop; nop }
107DE0FA     	{ if (!p0.new) jump:t	00006A90; p0 = cmp.eq(r29,00000001) }
000043A4 78004611 2810284D 	{ r21 = 00000004; r0 = 00000001; r17 = 0000E930 }
00004400 4800C024 5A006256 BFF57FF5 20493090 	{ r0 = r17; r17 = add(r17,00000004); r21 = add(r21,FFFFFFFF); call	0000AF60; memb(gp+4) = r0 }
107DE0FA     	{ if (!p0.new) jump:t	00006AB4; p0 = cmp.eq(r29,00000001) }
78004020 78004041 A1B3C400 A194C100 	{ memb(r19) = r0.new; r1 = 00000002; r0 = 00000001 }
91944000 24C2E100 9113C000 	{ if (!cmp.gt(r0.new,00000002)) jump:t	00006AD8; r0 = memw(r20) }
8540C000     	{ p0 = r0 }
7A004080 20C2F008 C4105240 	{ if (!cmp.gt(r16,r0.new)) jump:t	00006AF8; r0 = mux(p0,00000004,00000000) }
5A00E202     	{ call	0000AEF4 }
5800DC36     	{ jump	0000A360 }

;; _Unlocksyslock: 00006AF8
_Unlocksyslock proc
A09DC000     	{ allocframe(+00000000) }
00004400 4900C501 8541C000 	{ p0 = r1; r1 = memb(gp+40) }
7A004081 20C3E00C 	{ r1 = mux(p0,00000004,00000000) }
000043A4 7800C601 C4004140 5A00E20C 	{ call	0000AF34; r0 = addasl(r1,r0,00000002); r1 = 0000E930 }
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Tls_get__Tolotab: 00006B30
_Tls_get__Tolotab proc
78004021 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r1 = 00000001 }
00004400 78004610 00004400 7800C691 A1DDD200 	{ memd(r29) = r19:r18; r17 = 00010034; r16 = 00010030 }
9210C000     	{ r0 = memw_locked(r16) }
7540C000     	{ p0 = cmp.gt(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	00006B60 }
A0B0C100     	{ memw_locked(r16,p0) = r1 }
5CFFE0F8     	{ if (!p0) jump:nt	00006B4C }
10406010 7E00E052 	{ if (p0.new) r18 = 00000002; if (!p0.new) jump:t	00006B80; p0 = cmp.eq(r0,00000000) }
00004400 78004680 000041BF 7800C601 5A00E2EC 	{ call	0000B150; r1 = 00006FF0; r0 = 00010034 }
A190D200     	{ memw(r16) = r18 }
91904000 24C2E100 5A00636C 	{ if (!cmp.gt(r0.new,00000002)) jump:t	00006B84; r0 = memw(r16) }
9191C000     	{ r0 = memw(r17) }
7C006080 70604010 2442E01E 5A00C152 	{ if (!cmp.eq(r16.new,00000000)) jump:t	00006BD4; r16 = r0; r1:r0 = combine(00000004,00000001) }
70604010 2402C016 5A00633E 	{ if (cmp.eq(r16.new,00000000)) jump:nt	00006BD0; r16 = r0 }
50810090     	{ r0 = memw(r17); r1 = r16 }
1000C008     	{ if (p0.new) jump:nt	00006BC0; p0 = cmp.eq(r0,00000000) }
5A00421E 28083080 	{ r0 = r16; r16 = 00000000; call	00006FF0 }
5800C008     	{ jump	00006BCC }
00004348 78004440 A1B0D200 70704000 	{ memb(r16) = r0.new; r0 = 0000D222 }
3E0C1E05     	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
961EC01E     	{ dealloc_return }

;; _Getptolower: 00006BD8
_Getptolower proc
5BFF7FAC A09DC000 	{ allocframe(+00000000); call	00006B30 }
00003F40     	{ dealloc_return; r0 = memw(r0) }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Vacopy: 00006BF0
_Vacopy proc
78004082 70133C10 	{ allocframe(00000008); r3 = r1; r2 = 00000004 }
5A0043F4 6C112813 	{ memw(r29+4) = r3; r1 = add(r29,00000004); call	000073E0 }
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Wctombx: 00006C10
_Wctombx proc
5A005B34 A09DC004 	{ allocframe(+00000020); call	0000A278 }
F5014010 302A304C 	{ r20 = r4; r18 = r2; r17:r16 = combine(r1,r0) }
75104000 91834005 2403C06C 	{ r5 = memw(r3); p0 = cmp.eq(r16,00000000) }
5C00407C 7F00C000 	{ nop; if (p0) jump:nt	00006D24 }
78004013 7F004000 480D2BA0 	{ r0 = memuh(r18+6); r21 = 00000000; nop; r19 = 00000000 }
DD4041E0 5C004868 7080EC00 	{ if (!p0.new) r0 = zxtb(r0); if (p0.new) jump:nt	00006D14; p0 = cmpb.gtu(r0,0F) }
3A946016 2402C062 5BFFFD2E 	{ if (cmp.eq(r22.new,00000000)) jump:nt	00006D14; r22 = memw(r6+r0<<#2) }
14C04B5C 7415E035 	{ if (p0.new) r21 = add(r21,00000001); if (!p0.new) jump:nt	00006D10; p0 = cmp.gt(r0,-00000001) }
5C004858 70916C00 0000403F 7555C5E0 3A764081 2403C050 	{ r1 = memuh(r4+r0<<#1); p0 = cmp.gt(r21,00000FEF); if (!p0.new) r0 = zxtb(r17); if (p0.new) jump:nt	00006D10 }
00004200 76014000 30933712 	{ r2 = and(r1,000000FF); r3 = r17; r0 = and(r1,00008000) }
DA636002 00004040 76014004 7500C000 	{ p0 = cmp.eq(r0,00000000); r4 = and(r1,00001000); r3 = or(r2,and(r3,FFFFFF00)) }
75044001 00004080 76014005 7411C003 	{ if (p0) r3 = add(r17,00000000); r5 = and(r1,00002000); p1 = cmp.eq(r4,00000000) }
8D214400 8C034844 74234011 7505C000 	{ p0 = cmp.eq(r5,00000000); if (p1) r17 = add(r3,00000000); r4 = asl(r3,00000008); r0 = extractu(r1,00000004,0000000C) }
8E43D8A4     	{ r4 |= lsr(r3,00000018) }
5C004014 74A4C011 	{ if (!p1) r17 = add(r4,00000000); if (p0) jump:nt	00006CE0 }
F3105302 78004015 31BB3920 	{ p0 = cmp.eq(r2,00000000); r19 = r19; r21 = 00000000; r2 = add(r16,r19) }
74114003 7481C003 	{ if (!p0) r3 = add(r1,00000000); if (p0) r3 = add(r17,00000000) }
DD034000 5C00480C A102C300 	{ memb(r2) = r3; if (p0.new) jump:nt	00006CF0; p0 = cmpb.eq(r3,00) }
00004100 76014001 2433E0B0 	{ r1 = and(r1,00004000) }
000040D9 170B4028 A152C003 	{ memuh(r18+6) = r0; jump	00006D3C; r0 = r11 }
1008C024     	{ if (p0.new) jump:nt	00006D40; p0 = cmp.eq(r8,00000000) }
78005000 2142F12A 7070C000 	{ if (!cmp.gtu(r0.new,r17)) jump:t	00006D54; r0 = 00000080 }
5800406C AB00D108 	{ memb(r0++#1) = r17; jump	00006DE0 }
5A004100 78DFFFF3 	{ r19 = FFFFFFFF; call	00006F10 }
000040D8 170B4050 3C40C058 	{ memw(r0) = 00000058; jump	00006DB8; r0 = r11 }
F0A110A0     	{ memw(r18) = 00000000; memw(r18+4) = 00000000 }
9183C000     	{ r0 = memw(r3) }
9160C000     	{ r0 = memuh(r0) }
0000403C 7600C013 000040D8 170BC010 78004013 F0A010A1 	{ memw(r18+4) = 00000000; memw(r18) = 00000000; r19 = 00000000; jump	00006D58; r0 = r11; r19 = and(r0,00000F00) }
000040D7 170BC070 8C114B20 2442E00A 7311E020 	{ if (!cmp.eq(r0.new,00000000)) jump:t	00006D68; r0 = lsr(r17,0000000B); jump	00006E28; r0 = r11 }
DEC14612 5800C026 	{ jump	00006DAC; r1 = add(000000C2,asl(r1,0000000C)) }
8C115020 2442E00A 7311E040 	{ if (!cmp.eq(r0.new,00000000)) jump:t	00006D7C; r0 = lsr(r17,00000010) }
DEE14C12 5800C01C 	{ jump	00006DAC; r1 = add(000000E2,asl(r1,00000018)) }
8C115520 2442E00A 7311E060 	{ if (!cmp.eq(r0.new,00000000)) jump:t	00006D90; r0 = lsr(r17,00000015) }
DEE17212 5800C012 	{ jump	00006DAC; r1 = add(000000F2,asl(r1,00000004)) }
8C115A20 2442E00A 7311E080 	{ if (!cmp.eq(r0.new,00000000)) jump:t	00006DA4; r0 = lsr(r17,0000001A) }
DEE17892 5800C008 	{ jump	00006DAC; r1 = add(000000F2,asl(r1,00000011)) }
7311E0A0     	{  }
DEE1FED2     	{ r1 = add(000000FA,asl(r1,0000001D)) }
76404003 78DF7F41 7A041081 	{ memb(r16) = r1; r4 = -00000001; r1 = FFFFFFFA; r3 = sub(00000000,r0) }
D5C34403 E10040C1 B010C022 	{ r2 = add(r16,00000001); r1 += mpyi(r0,00000006); r3 = max(r3,r4) }
60004010 E200C043 	{ r3 += add(r0,00000002); loop0(00006DC8,r0) }
C6514100 BFE1FF41 	{ r1 = add(r1,FFFFFFFA); r0 &= asr(r17,r1) }
7600C7E0     	{ r0 = and(r0,0000003F) }
8CC08700 ABA2C208 	{  }
F310C300     	{ r0 = add(r16,r3) }
F330C013     	{ r19 = sub(r0,r16) }
000040D5 170B4038 7F00C000 	{ nop; jump	00006E54; r0 = r11 }

;; _Wctomb: 00006DF0
_Wctomb proc
F5024010 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r17:r16 = combine(r2,r0) }
5BFF7C06 701A2A05 	{ memd(r29) = r19:r18; r18 = r1; call	00006604 }
75104000 78004004 70604013 2403E00E 	{ r19 = r0; r4 = 00000000; p0 = cmp.eq(r16,00000000) }
5C00C006     	{ if (p0) jump:nt	00006E1C }
5A00D0AE     	{ call	00008F70 }
7060C004     	{ r4 = r0 }
F5135102 F5125000 3E0C1E05 	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r1:r0 = combine(r18,r16); r3:r2 = combine(r19,r17) }
59FF7EF4 901EC01E 	{ deallocframe; jump	00006C10 }

;; abort: 00006E30
abort proc
5A004498 68603C00 	{ allocframe(00000000); r0 = 00000006; call	00007760 }
5A00409C 7800C020 	{ r0 = 00000001; call	00006F70 }

;; calloc: 00006E40
calloc proc
ED014011 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17 = mpyi(r1,r0) }
5A0041EC 7071C000 	{ r0 = r17; call	00007220 }
70604010 2402C008 5BFF729C 	{ if (cmp.eq(r16.new,00000000)) jump:nt	00006E64; r16 = r0 }
30923D80     	{ r1:r0 = combine(00000000,00000000); r2 = r17 }
70704000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = r16 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Tls_get__Errno: 00006E70
_Tls_get__Errno proc
78004021 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r1 = 00000001 }
00004400 78004710 00004400 7800C791 A1DDD200 	{ memd(r29) = r19:r18; r17 = 0001003C; r16 = 00010038 }
9210C000     	{ r0 = memw_locked(r16) }
7540C000     	{ p0 = cmp.gt(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	00006EA0 }
A0B0C100     	{ memw_locked(r16,p0) = r1 }
5CFFE0F8     	{ if (!p0) jump:nt	00006E8C }
10406010 7E00E052 	{ if (p0.new) r18 = 00000002; if (!p0.new) jump:t	00006EC0; p0 = cmp.eq(r0,00000000) }
00004400 78004780 000041BF 7800C601 5A00E14C 	{ call	0000B150; r1 = 00006FF0; r0 = 0001003C }
A190D200     	{ memw(r16) = r18 }
91904000 24C2E100 5A0061CC 	{ if (!cmp.gt(r0.new,00000002)) jump:t	00006EC4; r0 = memw(r16) }
9191C000     	{ r0 = memw(r17) }
7C006080 70604010 2442E01A 5BFFFFB2 	{ if (!cmp.eq(r16.new,00000000)) jump:t	00006F0C; r16 = r0; r1:r0 = combine(00000004,00000001) }
70604010 2402C012 5A00619E 	{ if (cmp.eq(r16.new,00000000)) jump:nt	00006F08; r16 = r0 }
50810090     	{ r0 = memw(r17); r1 = r16 }
1000400A 3950C000 	{ if (p0.new) memw(r16) = 00000000; if (p0.new) jump:nt	00006F04; p0 = cmp.eq(r0,00000000) }
5A00407C 28083080 	{ r0 = r16; r16 = 00000000; call	00006FF0 }
5800C002     	{ jump	00006F04 }
70704000 3E0C1E05 	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r16 }
961EC01E     	{ dealloc_return }

;; _Geterrno: 00006F10
_Geterrno proc
59FFFFB0     	{ jump	00006E70 }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; atexit: 00006F20
atexit proc
70604010 78004020 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r0 = 00000001; r16 = r0 }
5BFFFD8C     	{ call	00006A44 }
78DF7FF1 4980C060 	{ r0 = memw(gp+12); r17 = FFFFFFFF }
49804201 2203E00A 	{ r1 = memw(gp+64) }
5A00D070     	{ call	00009020 }
1000C00E     	{ if (p0.new) jump:nt	00006F60; p0 = cmp.eq(r0,00000000) }
4980C060     	{ r0 = memw(gp+12) }
BFE07FE0 78004011 4980C041 	{ r1 = memw(gp+8); r17 = 00000000; r0 = add(r0,FFFFFFFF) }
48804003 3B81E010 	{ memw(r30+r0<<#2) = r16; memw(gp) = r0 }
5BFF7DCC 7800C020 	{ r0 = 00000001; call	00006AF8 }
70714000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = r17 }

;; exit: 00006F70
exit proc
70604010 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r0 }
4980C022     	{ r2 = memw(gp+4) }
49804061 2243E220 	{ r1 = memw(gp+12) }
B0014022 4980C040 	{ r0 = memw(gp+8); r2 = add(r1,00000001) }
3A806100 4880C203 	{ memw(gp+64) = r2; r0 = memw(r6+r1<<#2) }
50A0C000     	{ callr	r0 }
4980C061     	{ r1 = memw(gp+12) }
49804020 2132E1F4 5800C00C 	{ if (cmp.gtu(r0.new,r1)) jump:t	00006F88; r0 = memw(gp+4) }
BFE07FE0 4980C041 	{ r1 = memw(gp+8); r0 = add(r0,FFFFFFFF) }
3A816001 4880C010 	{ memw(gp) = r0; r1 = memw(r22+r0<<#2) }
50A1C000     	{ callr	r1 }
000043A5 78004011 49804200 2472E0F6 49804040 	{ if (!cmp.eq(r0.new,00000001)) jump:t	00006FB4; r0 = memw(gp+64); r17 = 0000E940 }
2002F106 5A00C00E 	{ if (cmp.eq(r0.new,r17)) jump:t	00006FDC }
73306A00     	{  }
4880D102     	{ memw(gp+544) = r17 }
5A0044E8 4880C101 	{ memw(gp+32) = r1; call	000079B0 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; free: 00006FF0
free proc
EBF41C10     	{ allocframe(00000008); memd(r29+496) = r17:r16 }
70604010 2402C054 9790FFC0 	{ if (cmp.eq(r16.new,00000000)) jump:nt	000070A0; r16 = r0 }
78004101 2103E04E 	{ r1 = 00000008 }
760040E0 2442E04A 5BFF7D1A 	{ if (!cmp.eq(r0.new,00000000)) jump:t	000070A0; r0 = and(r0,00000007) }
7800C020     	{ r0 = 00000001 }
BFF07F00 49804241 2403E008 	{ r1 = memw(gp+72); r0 = add(r16,FFFFFFF8) }
1541E01E     	{ if (!p0.new) jump:t	00007060; p0 = cmp.gtu(r1,r0) }
A79061FF 4880C012 	{ memw(gp) = r0; memw(r16-4) = r1 }
91804022 2402C032 9180C001 	{ if (cmp.eq(r2.new,00000000)) jump:nt	00007098; r2 = memw(r0+4) }
78004002 F3004103 2043E22C 	{ r3 = add(r0,r1); r2 = 00000000 }
4880C211     	{ memw(gp+64) = r2 }
9180C022     	{ r2 = memw(r0+4) }
00230122     	{ r2 = memw(r2+4); r3 = memw(r2) }
78310102     	{ memw(r0+4) = r2; r1 = add(r1,r3) }
5800401E A180C100 	{ memw(r0) = r1; jump	00007094 }
7061C002     	{ r2 = r1 }
9182C021     	{ r1 = memw(r2+4) }
1001C004     	{ if (p0.new) jump:nt	00007070; p0 = cmp.eq(r1,00000000) }
1530F1FA     	{ if (p1.new) jump:t	00007060; p1 = cmp.gtu(r0,r1) }
9182C003     	{ r3 = memw(r2) }
F3024304 2102E010 14045012 	{ if (cmp.gtu(r4.new,r0)) jump:t	00007098; r4 = add(r2,r3) }
5C00C01C     	{ if (p0) jump:nt	000070B8 }
70704004 9790FFC3 	{ r3 = memw(r16-8); r4 = r16 }
E2035F04 2142E114 5BFF7D32 	{ r4 += add(r3,FFFFFFF8) }
7800C020     	{ r0 = 00000001 }
3E041F40     	{ dealloc_return; r17:r16 = memd(r29) }
70624000 9790FFC1 	{ r1 = memw(r16-8); r0 = r2 }
59FF7FC4 F3014301 A1A2D300 	{ r1 = add(r1,r3); jump	00007030 }
59FF7FBE A79061FF A182C001 	{ memw(r2+4) = r0; memw(r16-4) = r1; jump	00007030 }

;; fwrite: 000070C0
fwrite proc
5A005900 A09DC007 	{ allocframe(+00000038); call	0000A2C0 }
F5034110 2800300B 	{ r19 = r0; r0 = 00000000; r17:r16 = combine(r3,r1) }
ED025015 2403C09C 	{ r21 = mpyi(r2,r16) }
10084098 7062C000 	{ r0 = r2; if (p0.new) jump:nt	00007208; p0 = cmp.eq(r8,00000001) }
5BFF7C3C 309030DF 	{ r23 = r21; r0 = r17; call	00006958 }
50FA0490     	{ r0 = memw(r17+16); r18 = r23 }
919140C1 2103E00A 	{ r1 = memw(r17+24) }
5A004466 280E3090 	{ r0 = r17; r22 = 00000000; call	000079C0 }
11C0C166     	{ if (!p0.new) jump:t	000071C8; p0 = tstbit(r0,00000000) }
6BE04000 70724003 50A21190 	{ r0 = memb(r17+1); r2 = r18; r3 = r18; p0 = or(p0,!p0) }
6BE04000 6B204001 76004080 2402E01E 7333E140 	{ if (cmp.eq(r0.new,00000000)) jump:t	00007154; r0 = and(r0,00000004); p1 = or(p0,p0); p0 = or(p0,!p0) }
89404003 5A004118 A1BDD502 	{ call	00007354; r3 = p0 }
75004000 50A33C21 	{ r1 = memd(r29+8); r3 = r18; p0 = cmp.eq(r0,00000000) }
85414001 5C00C00A 	{ if (p0) jump:nt	0000714C; p1 = r1 }
6B604000 DB80C333 	{ r3 = add(r0,sub(00000041,r19)); p0 = and(p0,p0) }
6B20C001     	{ p1 = or(p0,p0) }
89414005 70734001 04900692 	{ r2 = memw(r17+24); r0 = memw(r17+16); r1 = r19; r5 = p1 }
F3204204 A19DC502 	{ memw(r29+8) = r5; r4 = sub(r2,r0) }
D5A34494 F263C400 	{ p0 = cmp.gtu(r3,r4); r20 = minu(r3,r4) }
89404003 70744002 F3345217 A1BDD701 	{ r23 = sub(r18,r20); r2 = r20; r3 = p0 }
5A00C136     	{ call	000073E0 }
9191C080     	{ r0 = memw(r17+16) }
F3005416 919D4020 A1B1D404 85404000 	{ memb(r17+4) = r22.new; r0 = memw(r29+4); r22 = add(r0,r20) }
5C005816 479DC040 	{ if (!p0.new) r0 = memw(r29+8); if (p0.new) jump:t	000071B8 }
85404000 5C005810 74916000 4791C058 	{ if (!p0.new) r24 = memw(r17+8); if (!p0.new) r0 = add(r17,00000000); if (p0.new) jump:t	000071B8; p0 = r0 }
5A00D02E     	{ call	00009200 }
10404010 74976012 F338D616 	{ r22 = sub(r22,r24); if (!p0.new) r18 = add(r23,00000000); if (!p0.new) jump:nt	000071C8; p0 = cmp.eq(r0,00000000) }
F2125400 78004016 280A38CB 	{ r19 = add(r19,r20); r18 = 00000000; r22 = 00000000; p0 = cmp.eq(r18,r20) }
5CFFE094     	{ if (!p0) jump:nt	000070E8 }
5800C002     	{ jump	000071C8 }
59E01190     	{ r0 = memb(r17+1); p0 = cmp.eq(r22,00000000) }
76004100 2402E012 5C20C00E 	{ if (cmp.eq(r0.new,00000000)) jump:t	000071F4; r0 = and(r0,00000008) }
5A005014 70714000 049B029C 	{ r20 = memw(r17+8); r19 = memw(r17+16); r0 = r17; call	00009200 }
F3345316 7500C000 	{ p0 = cmp.eq(r0,00000000); r22 = sub(r19,r20) }
7E00C016     	{ if (p0) r22 = 00000000 }
5BFF7C10 70714000 F332D511 	{ r17 = sub(r21,r18); r0 = r17; call	00006A10 }
5A0059A2 F3365100 7070C001 	{ r1 = r16; r0 = sub(r17,r22); call	0000A540 }
5800D8A6     	{ jump	0000A354 }
7F00C000     	{ nop }

;; dkw_malloc_init: 00007210
dkw_malloc_init proc
DB81C120     	{ r1 = add(r1,sub(00000041,r0)) }
8C014321 59FF7EEE ABA0D510 	{  }

;; malloc: 00007220
malloc proc
70604010 78004020 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r0 = 00000001; r16 = r0 }
B010C1E1     	{ r1 = add(r16,0000000F) }
5BFF7C0A 7621FF11 	{ r17 = and(r1,FFFFFFF8); call	00006A44 }
15496860 7E00E100 	{ if (p0.new) r0 = 00000008; if (!p0.new) jump:t	000072F8; p0 = cmp.gtu(r9,r8) }
D5C05191 5800C008 	{ jump	00007254; r17 = maxu(r0,r17) }
5BFF7ED4 7F00C000 	{ nop; call	00006FF0 }
00004401 78004102 4980C224 	{ r4 = memw(gp+68); r2 = 00010048 }
10446012 7064C005 	{ r5 = r4; if (!p0.new) jump:t	00007280; p0 = cmp.eq(r4,00000000) }
7062C000     	{ r0 = r2 }
91804001 2403C02A 	{ r1 = memw(r0) }
7061C002     	{ r2 = r1 }
9B824023 2233F1F8 	{ r3 = memw(r2++#4) }
5800C032     	{ jump	000072E0 }
7065C000     	{ r0 = r5 }
91804001 2403C00C 	{ r1 = memw(r0) }
7061C005     	{ r5 = r1 }
9B854023 2233F1F8 	{ r3 = memw(r5++#4) }
5800C024     	{ jump	000072E0 }
9184C004     	{ r4 = memw(r4) }
7062C000     	{ r0 = r2 }
91804001 2003C40C 	{ r1 = memw(r0) }
7061C002     	{ r2 = r1 }
9B824023 2233F1F8 	{ r3 = memw(r2++#4) }
5800C014     	{ jump	000072E0 }
4980C080     	{ r0 = memw(gp+16) }
D5D1C090     	{ r16 = maxu(r17,r0) }
5A0043F6 7070C000 	{ r0 = r16; call	00007AB0 }
107040BE 7480E100 	{ if (!p0.new) r0 = add(r0,00000008); if (!p0.new) jump:nt	00007248; p0 = cmp.eq(r16,00000001) }
8C104120 1478E9F6 	{ if (!p0.new) jump:t	000072C4; p0 = cmp.eq(r8,-00000001); r0 = lsr(r16,00000001) }
5800C00E     	{ jump	000072F8 }
1000C00C     	{ if (p0.new) jump:nt	000072F8; p0 = cmp.eq(r0,00000000) }
BFE37F02 2242F10E 5800401A 	{ if (!cmp.gtu(r17,r2.new)) jump:t	00007304; r2 = add(r3,FFFFFFF8) }
91814022 A1A0D200 16084024 	{ memb(r0) = r2.new; r2 = memw(r1+4) }
7800C020     	{ r0 = 00000001 }
F3015102 A1A0D200 91814023 	{ memb(r0) = r2.new; r2 = add(r1,r17) }
A1A2D301     	{  }
00130002     	{ r2 = memw(r0); r3 = memw(r1) }
F3314300 A1A2D200 A181D100 	{ memb(r2) = r0.new; r0 = sub(r3,r17) }
1002400A 7800C003 	{ r3 = 00000000; if (p0.new) jump:nt	00007334; p0 = cmp.eq(r2,00000000) }
9182C020     	{ r0 = memw(r2+4) }
75004000 7482E083 	{ if (!p0.new) r3 = add(r2,00000004); p0 = cmp.eq(r0,00000000) }
78004020 B0014110 4880C311 	{ memw(gp+96) = r3; r16 = add(r1,00000008); r0 = 00000001 }
5BFFFBDC     	{ call	00006AF8 }
70704000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = r16 }
7F00C000     	{ nop }

;; memchr: 00007350
memchr proc
10024012 28003003 	{ r3 = r0; r0 = 00000000; if (p0.new) jump:nt	00007374; p0 = cmp.eq(r2,00000000) }
76015FE1 7F00C000 	{ nop; r1 = and(r1,000000FF) }
91234004 2002C10C B0034023 	{ if (cmp.eq(r4.new,r1)) jump:nt	0000737C; r4 = memb(r3) }
BFE27FE2 2472E0FC 529FC000 	{ if (!cmp.eq(r2.new,00000001)) jump:t	00007368; r2 = add(r2,FFFFFFFF) }
50303FC0     	{ jumpr	r31; r0 = r3 }
7F00C000     	{ nop }

;; puts: 00007380
puts proc
70604010 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r16 = r0 }
5BFF7AE8 00004392 7800C300 5A004FE6 00004392 7330E300 	{ call	00009360; r0 = 0000E498; call	00006958 }
6BE14101 11C0C10E 	{ if (!p0.new) jump:t	000073C0; p0 = tstbit(r0,00000000); p1 = or(p1,!p1) }
5A004F9C 00004392 7C05C300 7560FFE0 	{ p0 = cmp.gt(r0,FFFFFFFF); r1:r0 = combine(0000E498,0000000A); call	000092E0 }
6BC0C001     	{ p1 = not(p0) }
89414001 00004392 78004300 A1BDD500 	{ r0 = 0000E498; r1 = p1 }
5BFFFB22     	{ call	00006A10 }
3C011E0C     	{ r17:r16 = memd(r29+8); r1 = memd(r29) }
8541C000     	{ p0 = r1 }
7A005FE0 961EC01E 	{ dealloc_return; r0 = mux(p0,FFFFFFFF,00000000) }

;; memcpy: 000073E0
memcpy proc
75024002 F121400E 758242E0 F201C001 	{ p1 = cmp.eq(r1,r0); p0 = cmp.gtu(r2,00000017); r14 = or(r1,r0); p2 = cmp.eq(r2,00000000) }
6B214201 8C024329 75824BE3 F12EC20E 	{ r14 = or(r14,r2); p3 = cmp.gtu(r2,0000005F); r9 = lsr(r2,00000003); p1 = or(p2,p1) }
858E4702 535F4100 9401C000 	{ dcfetch	r1,00000000; if (p1) jumpr:nt	r31; p2 = bitsclr(r14,00000007) }
6B634202 5C006A4A 7442FF02 	{ if (p2.new) r2 = add(r2,FFFFFFF8); if (p2.new) jump:nt	000076A4; p2 = and(p3,p2) }
5C206036 71E87FFF 7640C00F 	{ r15 = sub(00000000,r0); r8.l = FFFF; if (!p0) jump:nt	00007684 }
72687FFF 8C0240A7 B0014405 A09DC003 	{ allocframe(+00000018); r5 = add(r1,00000020); r7 = cl0(r2); r8.h = FFFD }
717F76B8 CC48474F 760140E9 A1DDD000 	{ memd(r29) = r17:r16; r9 = and(r1,00000007); r15 &= lsr(r8,r7); r31.l = DAE1 }
F3214013 723F4000 75494003 A1DDD201 	{ memd(r29+8) = r19:r18; p3 = cmp.gt(r9,00000000); r31.h = 0000; r19 = sub(r0,r1) }
F3024114 F3024907 702D2A16 	{ memd(r29+16) = r21:r20; r21 = r2; r7 = add(r2,r9); r20 = add(r2,r1) }
858F4700 760F43EF B0054405 9405C000 	{ dcfetch	r5,00000000; r5 = add(r5,00000020); r15 = and(r15,0000001F); p0 = bitsclr(r15,00000007) }
F32F4204 8C0F4348 760F40F0 7621FF01 	{ r1 = and(r1,FFFFFFF8); r16 = and(r15,00000007); r8 = asl(r15,00000003); r4 = sub(r2,r15) }
8C0F432F 760443E3 F3004F11 7614C0F4 	{ r20 = and(r20,00000007); r17 = add(r0,r15); r3 = and(r4,0000001F); r15 = lsr(r15,00000003) }
75544003 EF14C175 	{ r21 += sub(r1,r20); p3 = cmp.gt(r20,00000000) }
7475C115     	{ if (p3) r21 = add(r21,00000008) }
75874103 8C044524 B0054405 9405C000 	{ dcfetch	r5,00000000; r5 = add(r5,00000020); r4 = lsr(r4,00000005); p3 = cmp.gtu(r7,00000008) }
750F4001 74AF602F B0054405 9405C000 	{ dcfetch	r5,00000000; r5 = add(r5,00000020); if (!p1.new) r15 = add(r15,00000001); p1 = cmp.eq(r15,00000000) }
75044001 B0054405 7608410E 9405C000 	{ dcfetch	r5,00000000; r14 = and(r8,00000008); r5 = add(r5,00000020); p1 = cmp.eq(r4,00000000) }
B0054405 5C004110 75044022 9405C000 	{ dcfetch	r5,00000000; p2 = cmp.eq(r4,00000001); if (p1) jump:nt	000074F0; r5 = add(r5,00000020) }
74D14411 A0D1C000 	{ dczeroa(r17); if (!p2) r17 = add(r17,00000020) }
761343F3 A0D1C000 	{ dczeroa(r17); r19 = and(r19,0000001F) }
5C004026 85494002 9BC1404C 41C1D82A 	{ if (p3) r11:r10 = memd(r1+8); r13:r12 = memd(r1++#16); p2 = r9; if (p0) jump:nt	00007538 }
C20C4A46 85084300 F309D009 	{ r9 = add(r9,r16); p0 = tstbit(r8,00000003); r7:r6 = valignb(r11:r10,r13:r12,p2) }
C3864E46 85084400 7608420E AB00E608 	{ if (p0) memb(r0++#1) = r6; r14 = and(r8,00000010); p0 = tstbit(r8,00000004); r7:r6 = lsr(r7:r6,r14) }
C3864E46 85084500 758940E2 AB40E608 	{ if (p0) memuh(r0++#2) = r6; p2 = cmp.gtu(r9,00000007); p0 = tstbit(r8,00000005); r7:r6 = lsr(r7:r6,r14) }
FD0B4A4C 85494002 9BC1642A AB80E608 	{ if (p0) memuh(r0++#4) = r6; if (p2) r11:r10 = memd(r1++#8); p2 = r9; if (p2) r13:r12 = combine(r11,r10) }
60AFC008     	{ p3 = sp1loop0(0000753C,r15) }
C20C4A46 F2554100 ABC0E60B 	{ if (p3) memd(r0++#8) = r7:r6; p0 = cmp.gt(r21,r1); r7:r6 = valignb(r11:r10,r13:r12,p2) }
F50B8A0C 9BC1E02A 	{ if (p0) r11:r10 = memd(r1++#8); r13:r12 = combine(r11,r10) }
75844003 74647FE4 7463E403 	{ if (p3.new) r3 = add(r3,00000020); if (p3.new) r4 = add(r4,FFFFFFFF); p3 = cmp.gtu(r4,00000000) }
1204405E 7C7F640E 7593C303 	{ p3 = cmp.gtu(r19,00000018); r15:r14 = combine(00000020,FFFFFFFF); if (p1.new) jump:t	00007618; p1 = cmp.eq(r4,00000000) }
5C00C330     	{ if (p3) jump:nt	000075C8 }
60044018 75844020 7064C008 	{ r8 = r4; p0 = cmp.gtu(r4,00000001); loop0(00007578,r4) }
74114411 9405C000 	{ dcfetch	r5,00000000; if (p0) r17 = add(r17,00000020) }
F2044803 A0D1C000 	{ dczeroa(r17); p3 = cmp.eq(r4,r8) }
D3044EA4 C20C4A52 9BC1402C ABC0F20F 	{ if (!p3) memd(r0++#8) = r19:r18; r13:r12 = memd(r1++#8); r19:r18 = valignb(r11:r10,r13:r12,p2); r5:r4 = vaddw(r15:r14,r5:r4) }
C20A4C46 9BC1402A ABC0C608 	{ memd(r0++#8) = r7:r6; r11:r10 = memd(r1++#8); r7:r6 = valignb(r13:r12,r11:r10,p2) }
C20C4A52 9BC1402C ABC0D208 	{ memd(r0++#8) = r19:r18; r13:r12 = memd(r1++#8); r19:r18 = valignb(r11:r10,r13:r12,p2) }
C20A8C46 75844020 9BC1402A ABC0C608 	{ memd(r0++#8) = r7:r6; r11:r10 = memd(r1++#8); p0 = cmp.gtu(r4,00000001); r7:r6 = valignb(r13:r12,r11:r10,p2) }
5800402C ABC0D208 	{ memd(r0++#8) = r19:r18; jump	00007618 }
60044018 75844020 BFE4FFE8 	{ r8 = add(r4,FFFFFFFF); p0 = cmp.gtu(r4,00000001); loop0(000075D4,r4) }
D3044EA4 74114411 9405C000 	{ dcfetch	r5,00000000; if (p0) r17 = add(r17,00000020); r5:r4 = vaddw(r15:r14,r5:r4) }
A0D1C000     	{ dczeroa(r17) }
C20C4A46 9BC1402C ABC0C608 	{ memd(r0++#8) = r7:r6; r13:r12 = memd(r1++#8); r7:r6 = valignb(r11:r10,r13:r12,p2) }
C20A4C46 9BC1402A ABC0C608 	{ memd(r0++#8) = r7:r6; r11:r10 = memd(r1++#8); r7:r6 = valignb(r13:r12,r11:r10,p2) }
C20C4A46 9BC1402C ABC0C608 	{ memd(r0++#8) = r7:r6; r13:r12 = memd(r1++#8); r7:r6 = valignb(r11:r10,r13:r12,p2) }
C20A8C46 75844020 9BC1402A ABC0C608 	{ memd(r0++#8) = r7:r6; r11:r10 = memd(r1++#8); p0 = cmp.gtu(r4,00000001); r7:r6 = valignb(r13:r12,r11:r10,p2) }
75034000 8C03432F 7603C0E4 	{ r4 = and(r3,00000007); r15 = lsr(r3,00000003); p0 = cmp.eq(r3,00000000) }
8C034345 535F4000 FB224000 750FC003 	{ p3 = cmp.eq(r15,00000000); if (p0) r0 = sub(r0,r2); if (p0) jumpr:nt	r31; r5 = asl(r3,00000003) }
85034200 5C004312 7605C40E 	{ r14 = and(r5,00000020); if (p3) jump:nt	0000765C; p0 = tstbit(r3,00000002) }
600FC008     	{ loop0(00007644,r15) }
C20C4A46 F2554103 ABC0C608 	{ memd(r0++#8) = r7:r6; p3 = cmp.gt(r21,r1); r7:r6 = valignb(r11:r10,r13:r12,p2) }
F50B8A0C 9BC1E62A 	{ if (p3) r11:r10 = memd(r1++#8); r13:r12 = combine(r11,r10) }
C3864E46 85034100 7605420E AB80E608 	{ if (p0) memuh(r0++#4) = r6; r14 = and(r5,00000010); p0 = tstbit(r3,00000001); r7:r6 = lsr(r7:r6,r14) }
C3864E46 85034000 74027FE2 AB40E608 	{ if (p0) memuh(r0++#2) = r6; if (p0.new) r2 = add(r2,FFFFFFFF); p0 = tstbit(r3,00000000); r7:r6 = lsr(r7:r6,r14) }
F3224000 529F4000 4000C600 	{ if (p0) memb(r0) = r6; jumpr	r31; r0 = sub(r0,r2) }
60A24010 BFE2FFE2 	{ r2 = add(r2,FFFFFFFF); p3 = sp1loop0(0000768C,r2) }
9B018026 AB00E60B 	{ if (p3) memb(r0++#1) = r6; r6 = memb(r1++#1) }
F3224000 529F4000 A100C600 	{ memb(r0) = r6; jumpr	r31; r0 = sub(r0,r2) }
60A9C008     	{ p3 = sp1loop0(000076A4,r9) }
9BC18026 ABC0E60B 	{ if (p3) memd(r0++#8) = r7:r6; r7:r6 = memd(r1++#8) }
F3224000 529F4000 A1C0C600 	{ memd(r0) = r7:r6; jumpr	r31; r0 = sub(r0,r2) }
91DDC054     	{ r21:r20 = memd(r29+16) }
3E0D1E04     	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
901EC01E     	{ deallocframe }
529FC000     	{ jumpr	r31 }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; strlen: 000076E0
strlen proc
85804701 5C204928 48021004 	{ r4 = memb(r0); r2 = 00000000; if (!p1.new) jump:nt	00007734; p1 = bitsclr(r0,00000007) }
1004C01E     	{ if (p0.new) jump:nt	00007728; p0 = cmp.eq(r4,00000000) }
7C004006 74824102 7F004000 31C0C204 	{ if (!p0) r5:r4 = memd(r0+r2); nop; if (!p0) r2 = add(r2,00000008); r7:r6 = combine(00000000,00000000) }
D2046600 5C205800 74826102 33C0C204 	{ if (!p0.new) r5:r4 = memd(r0+r2); if (!p0.new) r2 = add(r2,00000008); if (!p0.new) jump:t	00007704; p0 = any8(vcmpb.eq(r5:r4,r7:r6)) }
D204C6C0     	{ p0 = vcmpb.eq(r5:r4,r7:r6) }
8940C004     	{ r4 = p0 }
8C444084     	{  }
BFE2FF02     	{ r2 = add(r2,FFFFFFF8) }
F3024400 529FC000 	{ jumpr	r31; r0 = add(r2,r4) }
7F004000 48003FC0 	{ jumpr	r31; r0 = 00000000; nop }
F3004201 1204C014 	{ if (p1.new) jump:t	0000775C; p1 = cmp.eq(r4,00000000); r1 = add(r0,r2) }
85814700 5CFF68FC 74826022 4721C024 	{ if (!p0.new) r4 = memb(r1+1); if (!p0.new) r2 = add(r2,00000001); if (!p0.new) jump:nt	00007734; p0 = bitsclr(r1,00000007) }
7C004006 59FF7FDC B0024102 3AC0C204 	{ r4 = memd(r12+r2); r2 = add(r2,00000008); jump	00007704; r7:r6 = combine(00000000,00000000) }
50203FC0     	{ jumpr	r31; r0 = r2 }
7F00C000     	{ nop }

;; raise: 00007760
raise proc
78004021 EBF41C40 	{ allocframe(00000020); memd(r29+496) = r17:r16; r1 = 00000001 }
5A00408C 78DF7FF2 70082A15 	{ memd(r29+16) = r19:r18; r16 = r0; r18 = FFFFFFFF; call	00007880 }
70604011 2603C044 	{ r17 = r0 }
1049E034     	{ if (!p0.new) jump:t	000077E4; p0 = cmp.eq(r9,00000000) }
BFF07FC0 498043E1 2544CD44 B01D4003 	{ if (!cmp.gtu(r0.new,0000001A)) jump:nt	00007810; r1 = memw(gp+124); r0 = add(r16,FFFFFFFE) }
06665999 7C93E040 B0034102 3C03C480 	{ memb(r3+9) = 80; r2 = add(r3,00000008); r1:r0 = combine(66666642,00000027) }
ED104024 B010C125 	{ r5 = add(r16,00000009); r4 = mpy(r16,r0) }
8C045F23 7585C240 	{ p0 = cmp.gtu(r5,00000012); r3 = lsr(r4,0000001F) }
CCC4C103     	{ r3 += asr(r4,r1) }
E183C150     	{ r16 -= mpyi(r3,0000000A) }
5CDF60F4 B0104604 70634010 ABA2C478 	{  }
00004353 78004060 00004393 7800C501 5A004DC4 B002C031 	{ r17 = add(r2,00000001); call	00009360; r1 = 0000E4E8; r0 = 0000D4C3 }
5800C038     	{ jump	00007850 }
1009610C 7800C012 	{ r18 = 00000000; if (p0.new) jump:t	000077FC; p0 = cmp.eq(r9,00000002) }
5A00404A 280A3D80 	{ r1:r0 = combine(00000000,00000000); r18 = 00000000; call	00007880 }
70704000 50B1C000 	{ callr	r17; r0 = r16 }
70724000 3E1C1E15 	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = r18 }
961EC01E     	{ dealloc_return }
00004351 78004391 3A81E000 	{ r0 = memw(r30+r0<<#2); r17 = 0000D45C }
5280C000     	{ jumpr	r0 }
5800401C 00004352 7800C171 58004016 00004351 7800C671 58004010 00004351 7800C451 5800400A 00004352 7800C311 00004352 7800C5F1 5A004D88 00004393 7331E500 	{ call	00009360; r17 = 0000D4AF; r17 = 0000D498; jump	00007850; r17 = 0000D462; jump	00007850; r17 = 0000D473; jump	00007850; r17 = 0000D48B; jump	00007850 }
00004353 78004180 00004393 7800C501 5A00CD7A 	{ call	00009360; r1 = 0000E4E8; r0 = 0000D4CC }
5BFF7B80 7800C020 	{ r0 = 00000001; call	00006F70 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; signal: 00007880
signal proc
F5014010 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
BFF07FE0 7A0A2A05 	{ memd(r29) = r19:r18; r18 = -00000001; r0 = add(r16,FFFFFFFF) }
5C005816 7580C540 	{ p0 = cmp.gtu(r0,0000002A); if (p0.new) jump:t	000078BC }
11896012 7E80E020 	{ if (!p0.new) r0 = 00000001; if (p0.new) jump:t	000078BC; p0 = cmp.eq(r9,-00000001) }
5BFFF8D2     	{ call	00006A44 }
78004020 000043AA 9D90F012 5BFF7924 000043AA AD90F180 70724000 3E0C1E05 	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r18; memw(r16<<#2+0000EA80) = r17; call	00006AF8; r18 = memw(r16<<#2+0000EA80); r0 = 00000001 }
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; strchr: 000078D0
strchr proc
7601DFE2     	{ r2 = and(r1,000000FF) }
91204001 2003C20A 	{ r1 = memb(r0) }
107160FC 20102801 	{ r1 = 00000000; r0 = add(r0,00000001); if (!p0.new) jump:t	000078D4; p0 = cmp.eq(r17,00000001) }
7061C000     	{ r0 = r1 }
529FC000     	{ jumpr	r31 }
7F00C000     	{ nop }

;; _Tls_get__Ctype: 000078F0
_Tls_get__Ctype proc
78004021 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r1 = 00000001 }
00004401 78004190 00004401 7800C211 A1DDD200 	{ memd(r29) = r19:r18; r17 = 00010050; r16 = 0001004C }
9210C000     	{ r0 = memw_locked(r16) }
7540C000     	{ p0 = cmp.gt(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	00007920 }
A0B0C100     	{ memw_locked(r16,p0) = r1 }
5CFFE0F8     	{ if (!p0) jump:nt	0000790C }
10406010 7E00E052 	{ if (p0.new) r18 = 00000002; if (!p0.new) jump:t	00007940; p0 = cmp.eq(r0,00000000) }
00004401 78004200 000041BF 7800C601 5A00DC0C 	{ call	0000B150; r1 = 00006FF0; r0 = 00010050 }
A190D200     	{ memw(r16) = r18 }
91904000 24C2E100 5A005C8C 	{ if (!cmp.gt(r0.new,00000002)) jump:t	00007944; r0 = memw(r16) }
9191C000     	{ r0 = memw(r17) }
7C006080 70604010 2442E01E 5BFFFA72 	{ if (!cmp.eq(r16.new,00000000)) jump:t	00007994; r16 = r0; r1:r0 = combine(00000004,00000001) }
70604010 2402C016 5A005C5E 	{ if (cmp.eq(r16.new,00000000)) jump:nt	00007990; r16 = r0 }
50810090     	{ r0 = memw(r17); r1 = r16 }
1000C008     	{ if (p0.new) jump:nt	00007980; p0 = cmp.eq(r0,00000000) }
5BFF7B3E 28083080 	{ r0 = r16; r16 = 00000000; call	00006FF0 }
5800C008     	{ jump	0000798C }
00004353 78004440 A1B0D200 70704000 	{ memb(r16) = r0.new; r0 = 0000D4E2 }
3E0C1E05     	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
961EC01E     	{ dealloc_return }

;; _Getpctype: 00007998
_Getpctype proc
5BFF7FAC A09DC000 	{ allocframe(+00000000); call	000078F0 }
00003F40     	{ dealloc_return; r0 = memw(r0) }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Exit: 000079B0
_Exit proc
5A005A80 A09DC000 	{ allocframe(+00000000); call	0000AEB0 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Fwprep: 000079C0
_Fwprep proc
70604010 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r0 }
9190C081     	{ r1 = memw(r16+16) }
919040C0 2142E106 1601C064 	{ if (!cmp.gtu(r0.new,r1)) jump:t	000079DC; r0 = memw(r16+24) }
9170C000     	{ r0 = memuh(r16) }
00004240 76004042 2442E23A 7850C002 	{ if (!cmp.eq(r2.new,00000004)) jump:t	00007A58; r2 = and(r0,00009002) }
F1004203 2043E212 	{ r3 = and(r0,r2) }
91904062 2102E10E 5A004C02 	{ if (cmp.gtu(r2.new,r1)) jump:t	00007A14; r2 = memw(r16+12) }
7070C000     	{ r0 = r16 }
1040604C 78DFFFE1 	{ r1 = FFFFFFFF; if (!p0.new) jump:t	00007A9C; p0 = cmp.eq(r0,00000000) }
9150C000     	{ r0 = memh(r16) }
00004020 76004000 2442E03A 9190C051 	{ if (!cmp.eq(r0.new,00000000)) jump:t	00007A8C; r0 = and(r0,00000800) }
B0104980 2042F132 5BFF7BFC 	{ if (!cmp.eq(r0.new,r17)) jump:t	00007A88; r0 = add(r16,0000004C) }
7801C000     	{ r0 = 00000200 }
10004022 A190C002 	{ memw(r16+8) = r0; if (p0.new) jump:nt	00007A74; p0 = cmp.eq(r0,00000000) }
B0204002 80812480 	{ memw(r16+16) = r0; r1 = memh(r16); r2 = add(r0,00000200) }
8CC14600 AD800C80 	{ memw(r16+48) = r0; memw(r16+52) = r0; r0 = setbit(r1,0000000C) }
5800401C A3822080 	{ memuh(r16+8) = r0; memw(r16+12) = r2; jump	00007A80 }
7060C001     	{ r1 = r0 }
00004100 DE01C110 8CC14E42 78DFFFE1 	{ r1 = FFFFFFFF; r2 = togglebit(r1,0000001C); r1 = and(00000002,asl(r1,00000002)) }
F122C000     	{ r0 = or(r2,r0) }
8CC04900 5800401A A1B0CC00 B0114020 	{ memb(r16) = r0.new; jump	00007AA0; r0 = setbit(r0,00000012) }
A2890489     	{ memw(r16+16) = r17; memw(r16+8) = r17 }
A190C003     	{ memw(r16+12) = r0 }
5A00CB20     	{ call	000090C0 }
78004001 02802082 	{ r2 = memh(r16); r0 = memw(r16+8); r1 = 00000000 }
00004180 76824002 83830580 	{ memw(r16+20) = r0; r3 = memw(r16+12); r2 = or(r2,00006000) }
A6832082     	{ memuh(r16+8) = r2; memw(r16+24) = r3 }
70614000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = r1 }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Getmem: 00007AB0
_Getmem proc
10C0400A A09DC000 	{ allocframe(+00000000); if (!p0.new) jump:nt	00007AC4; p0 = cmp.gt(r0,00000000) }
5A00DAF4     	{ call	0000B0A0 }
75207FE0 5A403F40 	{ dealloc_return; r0 = -00000001; p0 = cmp.eq(r0,FFFFFFFF) }
48003F40     	{ dealloc_return; r0 = 00000000 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Ldtob: 00007AD0
_Ldtob proc
5A0053D0 A09DC020 	{ allocframe(+00000100); call	0000A270 }
7061401A 7060C010 	{ r16 = r0; r26 = r1 }
8CDA4512 91D0C000 	{ r1:r0 = memd(r16); r18 = setbit(r26,0000000A) }
75124C20 A1DDC013 	{ memd(r29+152) = r1:r0; p0 = cmp.eq(r18,00000061) }
89404000 5C004014 A1BDD400 91904180 	{ memb(r29) = r0.new; if (p0) jump:nt	00007B1C; r0 = p0 }
26C2C00C 1040E00A 	{ if (!tstbit(r0.new,-00000001)) jump:nt	00007B18 }
58004008 75124CE0 3950C601 	{ if (p0.new) memw(r16+48) = 00000001; p0 = cmp.eq(r18,00000067); jump	00007B18 }
3C50C606     	{ memw(r16+48) = 00000006 }
5A0042F4 B01D52C0 7070C001 	{ r1 = r16; r0 = add(r29,00000096); call	00008100 }
8CDA4202 70C04001 2443E216 	{ r1 = zxth(r0); r2 = setbit(r26,00000004) }
75024CA0 0000435B 4A210480 5C004028 749AD342 	{ if (!p0) r2 = add(r26,FFFFFF9A); if (p0) jump:nt	00007B8C; r0 = memw(r16+16); r1 = 00000022; p0 = cmp.eq(r2,00000065) }
0000435B 2A642823 5800C01A 	{ jump	00007B80; r3 = 00000002; r4 = 00000026 }
75404000 919DC000 	{ r0 = memw(r29); p0 = cmp.gt(r0,00000000) }
85404001 5C20C022 	{ if (!p0) jump:nt	00007BA0; p1 = r0 }
8CDA4202 0000435B 4AA10480 5C005810 749A7342 7502CCA0 	{ p0 = cmp.eq(r2,00000065); if (!p0.new) r2 = add(r26,FFFFFF9A); if (p0.new) jump:t	00007B8C; r0 = memw(r16+16); r1 = 0000002A; r2 = setbit(r26,00000004) }
0000435B 2AE42823 7602DFE2 	{ r2 = and(r2,000000FF); r3 = 00000002; r4 = 0000002E }
F2634200 7484E001 	{ if (!p0.new) r1 = add(r4,00000000); p0 = cmp.gtu(r3,r2) }
5BFF7C2A 78004062 3C50C383 	{ memw(r16+28) = FFFFFF83; r2 = 00000003; call	000073E0 }
5800D3DC     	{ jump	0000A350 }
5C20411E 4190C880 	{ if (p1) r0 = memw(r16+16); if (!p1) jump:nt	00007BD8 }
751ACC20     	{ p0 = cmp.eq(r26,00000061) }
B0004022 A1B0D204 7E004F02 	{ memb(r16+4) = r2.new; r2 = add(r0,00000001) }
7E804B02 3C00C030 	{ memb(r0) = 30; if (!p0) r2 = 00000058 }
9190C080     	{ r0 = memw(r16+16) }
B0004023 A1B0D304 	{ r3 = add(r0,00000001) }
A100C200     	{ memb(r0) = r2 }
9190C0A0     	{ r0 = memw(r16+20) }
B0004040 A1B0D205 100140EC 	{ memb(r16+5) = r0.new; r0 = add(r0,00000002) }
5C20C1F4     	{ if (!p1) jump:nt	00007DC4 }
91904182 49C0C314 	{ r21:r20 = memd(gp+192); r2 = memw(r16+48) }
75627FE1 B0024022 91DD4264 91D0C000 	{ r1:r0 = memd(r16); r5:r4 = memd(r29+152); r2 = add(r2,00000001); p1 = cmp.gt(r2,FFFFFFFF) }
D2E45440 74224013 7EA0C433 	{ if (!p1) r19 = 00000021; if (p1) r19 = add(r2,00000000); p0 = dfcmp.ge(r5:r4,r21:r20) }
70F3C016     	{ r22 = sxth(r19) }
5C004006 B016C057 	{ r23 = add(r22,00000002); if (p0) jump:nt	00007C14 }
8CC1DF41     	{ r1 = togglebit(r1,0000001E) }
B01D5402 75574000 915D4963 A1DDC013 	{ memd(r29+152) = r1:r0; r3 = memh(r29+150); p0 = cmp.gt(r23,00000000); r2 = add(r29,000000A0) }
8CC24012 BFE37F82 3C02C000 	{ memb(r2) = 00; r2 = add(r3,FFFFFFFC); r18 = setbit(r2,00000000) }
5C20405A 70724019 A15DC24B 	{ memuh(r29+150) = r2; r25 = r18; if (!p0) jump:nt	00007CE4 }
58004006 7312E018 	{ jump	00007C48 }
91DDC260     	{ r1:r0 = memd(r29+152) }
D2F44040 5C00C84E 	{ if (p0.new) jump:nt	00007CE8; p0 = dfcmp.ge(r21:r20,r1:r0) }
5A004F88 B01D5300 279F29C1 	{ r1 = 0000001C; r23 = add(r23,FFFFFFF9); r0 = add(r29,00000098); call	00009B60 }
75574000 91DDC260 	{ r1:r0 = memd(r29+152); p0 = cmp.gt(r23,00000000) }
89404002 88E04031 A1BDD401 5C20C008 	{ memb(r29+1) = r2.new; r17 = convert_df2w(r1:r0):chop; r2 = p0 }
84914042 5A00D518 	{ call	0000A6A8; r3:r2 = convert_w2df(r17) }
A1DDC013     	{ memd(r29+152) = r1:r0 }
10C9601E 780040E0 B019C0E1 	{ r1 = add(r25,00000007); r0 = 00000007; if (!p0.new) jump:t	00007CBC; p0 = cmp.gt(r9,00000000) }
B01940C1 7800C0C2 	{ r2 = 00000006; r1 = add(r25,00000006) }
7062C000     	{ r0 = r2 }
B0004022 24C2C00E 8C114411 	{ if (!cmp.gt(r2.new,00000000)) jump:nt	00007CB8; r2 = add(r0,00000001) }
761141E3 BFE07FE2 ABA1C578 	{  }
10B9E0F2     	{ if (p0.new) jump:t	00007C94; p0 = cmp.gt(r25,00000001) }
10C06010 B001C021 	{ r1 = add(r1,00000001); if (!p0.new) jump:t	00007CD4; p0 = cmp.gt(r0,00000000) }
60004018 76404002 BFE1FFE3 	{ r3 = add(r1,FFFFFFFF); r2 = sub(00000000,r0); loop0(00007CC8,r0) }
7F008000 AB03D878 	{ memb(r3++#-1) = r24; nop }
F301C201     	{ r1 = add(r1,r2) }
B00140F9 919DC020 	{ r0 = memw(r29+4); r25 = add(r1,00000007) }
85404000 5CDFF8B4 	{ if (p0.new) jump:t	00007C48; p0 = r0 }
F332D901     	{ r1 = sub(r25,r18) }
F2564100 74016000 7493E000 	{ if (!p0.new) r0 = add(r19,00000000); if (p0.new) r0 = add(r1,00000000); p0 = cmp.gt(r22,r1) }
751A4C20 70E04002 26C2C052 F2414201 	{ if (!tstbit(r2.new,-00000001)) jump:nt	00007DA0; r2 = sxth(r0); p0 = cmp.eq(r26,00000061) }
0000435B 7800C641 0000435C 28352803 5C20C10E 	{ if (!p1) jump:nt	00007D30; r3 = 00000000; r5 = 00000003; r1 = 0000D6F2 }
B01DD403     	{ r3 = add(r29,000000A0) }
F302C303     	{ r3 = add(r2,r3) }
9123C023     	{ r3 = memb(r3+1) }
758340E1 7E2061E3 7EA0E003 	{ if (!p1.new) r3 = 00000000; if (p1.new) r3 = 0000000F; p1 = cmp.gtu(r3,00000007) }
B01D5404 74854001 31003406 	{ r6 = sxth(r0); r0 = r0; if (!p0) r1 = add(r5,00000000); r4 = add(r29,000000A0) }
E206C024     	{ r4 += add(r6,00000001) }
BFE07FE0 BFE27FE2 BFE47FE4 9724FFE5 	{ r5 = memb(r4-1); r4 = add(r4,FFFFFFFF); r2 = add(r2,FFFFFFFF); r0 = add(r0,FFFFFFFF) }
1435E3F8     	{ if (p0.new) jump:t	00007D40; p0 = cmp.eq(r5,-00000001) }
750341E0 74056023 42A4C300 	{ if (p0.new) r3 = add(r5,00000001); p0 = cmp.eq(r3,0000000F) }
1182E10C     	{ if (p0.new) jump:t	00007D78; p0 = tstbit(r2,00000000) }
B01D5412 B0004020 915DC962 	{ r2 = memh(r29+150); r0 = add(r0,00000001); r18 = add(r29,000000A0) }
B0024082 A1BDCA4B 70E04002 	{ memb(r29+75) = r2.new; r2 = add(r2,00000004) }
24C2C010 6002C008 	{ if (!cmp.gt(r2.new,00000000)) jump:nt	00007D9C }
F3124203 BFE2FFE2 	{ r2 = add(r2,FFFFFFFF); r3 = add(r18,r2) }
9723FFE4     	{ r4 = memb(r3-1) }
3A218404 A7A3E2FF 91904181 	{ memb(r3-1) = r4.new; r4 = memb(r24+r4) }
2693E0A4     	{  }
70E0C001     	{ r1 = sxth(r0) }
5800419E BFE17FE1 A1B0D30C 	{ r1 = add(r1,FFFFFFFF); jump	000080E0 }
B01D52C1 28002E8A 	{ r18 = add(r29,000000A0); r0 = 00000000; r1 = add(r29,00000096) }
58004194 3C21C000 	{ memuh(r1) = 0000; jump	000080E0 }
91DD4262 49C0C300 	{ r1:r0 = memd(gp+192); r3:r2 = memd(r29+152) }
D2E24040 5C00580C 469DDA20 	{ if (p0.new) memw(r29+16) = r26; if (p0.new) jump:t	00007DE4; p0 = dfcmp.ge(r3:r2,r1:r0) }
8CC3DF43     	{ r3 = togglebit(r3,0000001E) }
58004006 A1DDC213 	{ memd(r29+152) = r3:r2; jump	00007DE4 }
A19DDA04     	{ memw(r29+16) = r26 }
5A0042B6 28312DE0 	{ r0 = add(r29,00000078); r1 = 00000003; call	00008350 }
014F62D6 78004120 915D4962 4980C401 	{ r1 = memw(gp+128); r2 = memh(r29+150); r0 = 14F8B589 }
000041D6 E00242E3 	{  }
7061C002     	{ r2 = r1 }
ED03C020     	{ r0 = mpy(r3,r0) }
8C005F24 8C00CD03 	{ r3 = asr(r0,0000000D); r4 = lsr(r0,0000001F) }
EF034422 8E00DFA3 	{ r3 += lsr(r0,0000001F); r2 += add(r3,r4) }
D5014300 D5414301 	{  }
A15DC24B     	{ memuh(r29+150) = r2 }
11C1411A 7E00E013 	{ if (p0.new) r19 = 00000000; if (!p0.new) jump:t	00007E5C; p0 = tstbit(r1,00000000) }
10C1C040     	{ if (!p0.new) jump:nt	00007EB0; p0 = cmp.gt(r1,00000000) }
76207F93 000043FF 7600C781 10014038 A15DD34B 	{ memuh(r29+150) = r19; if (p0.new) jump:nt	00007EB0; p0 = cmp.eq(r1,00000000); r1 = and(r0,0000FFFC); r19 = and(r0,FFFFFFFC) }
00004397 78004502 28312D80 	{ r0 = add(r29,00000060); r1 = 00000003; r2 = 0000E5E8 }
5A00C550     	{ call	000088F4 }
5800C02C     	{ jump	00007EB0 }
76404060 7800C013 	{ r19 = 00000000; r0 = sub(00000003,r0) }
7620FF84     	{ r4 = and(r0,FFFFFFFC) }
10044024 76444000 A1BDCA4B B01D4C00 	{ memb(r29+75) = r0.new; r0 = sub(00000000,r4); if (p0.new) jump:nt	00007EB0; p0 = cmp.eq(r4,00000000) }
78004061 70644013 49C0C322 	{ r3:r2 = memd(gp+200); r19 = r4; r1 = 00000003 }
5A00C266     	{ call	00008350 }
58004014 7F00C000 	{ nop; jump	00007EB0 }
5A004532 B01D4C02 28312C60 	{ r0 = add(r29,00000018); r1 = 00000003; r2 = add(r29,00000060); call	000088F4 }
B01D4604 78004061 28332D80 	{ r0 = add(r29,00000060); r3 = 00000003; r1 = 00000003; r4 = add(r29,00000030) }
5A0045A4 B01DC302 	{ r2 = add(r29,00000018); call	000089F0 }
11CB630C 7E00E063 	{ if (p0.new) r3 = 00000003; if (!p0.new) jump:t	00007EC8; p0 = tstbit(r11,00000000) }
B01D4F00 78004061 2CC42D82 	{ r2 = add(r29,00000060); r4 = add(r29,00000030); r1 = 00000003; r0 = add(r29,00000078) }
5A00C596     	{ call	000089F0 }
8C134113 75124CC0 2475E0E4 	{ p0 = cmp.eq(r18,00000066); r19 = asr(r19,00000001) }
780040E1 CC800858 	{ memw(r29+20) = r16; r0 = memw(r16+16); r1 = 00000007 }
89404002 5C20400A A1BDD403 915DC961 	{ memb(r29+3) = r2.new; if (!p0) jump:nt	00007EF4; r2 = p0 }
B001C161     	{ r1 = add(r1,0000000B) }
78004263 2E823810 	{ r0 = add(r0,r1); r2 = add(r29,000000A0); r3 = 00000013 }
8CC24012 D5A04313 3C02C030 	{ memb(r2) = 30; r19 = min(r0,r3); r18 = setbit(r2,00000000) }
10CB4070 7072C01B 	{ r27 = r18; if (!p0.new) jump:nt	00007FE4; p0 = cmp.gt(r11,00000000) }
7312661A     	{  }
49C04354 49C0C378 	{ r25:r24 = memd(gp+216); r21:r20 = memd(gp+208) }
91DD4260 49C0C302 	{ r3:r2 = memd(gp+192); r1:r0 = memd(r29+152) }
D2E24040 5C004862 7E80E016 	{ if (!p0.new) r22 = 00000000; if (p0.new) jump:nt	00007FE8; p0 = dfcmp.ge(r3:r2,r1:r0) }
91DDC1E0     	{ r1:r0 = memd(r29+120) }
D2F84020 5C00D814 	{ if (p0.new) jump:t	00007F5C; p0 = dfcmp.gt(r25:r24,r1:r0) }
88E04030 28312DE0 	{ r0 = add(r29,00000078); r1 = 00000003; r16 = convert_df2w(r1:r0):chop }
8490C042     	{ r3:r2 = convert_w2df(r16) }
8CC35F43 5A00C286 	{ call	00008454; r3 = togglebit(r3,0000001E) }
588E3E78     	{ r1:r0 = memd(r29+120); r22 = add(r22,r16) }
D2E05840 5CDFF8F4 	{ if (p0.new) jump:t	00007F3C; p0 = dfcmp.ge(r1:r0,r25:r24) }
BFF3FF13     	{ r19 = add(r19,FFFFFFF8) }
FD157402 7553C000 	{ p0 = cmp.gt(r19,00000000); if (!p0) r3:r2 = combine(r21,r20) }
89404000 5C20400A A1BDD402 5A00440C 	{ memb(r29+2) = r0.new; if (!p0) jump:nt	00007F7C; r0 = p0 }
28312DE0     	{ r0 = add(r29,00000078); r1 = 00000003 }
10CE6022 78004110 B01BC100 	{ r0 = add(r27,00000008); r16 = 00000008; if (!p0.new) jump:t	00007FBC; p0 = cmp.gt(r14,00000000) }
B01B40FB 7800C0F1 	{ r17 = 00000007; r27 = add(r27,00000007) }
7071C010     	{ r16 = r17 }
B0104020 24C2C012 5A004A94 	{ if (!cmp.gt(r0.new,00000000)) jump:nt	00007FB8; r0 = add(r16,00000001) }
73366140     	{  }
BFF0FFF1     	{ r17 = add(r16,FFFFFFFF) }
F501C016     	{ r23:r22 = combine(r1,r0) }
10BE60F2 B0174600 ABBBC278 	{  }
10C86010 B01BC020 	{ r0 = add(r27,00000001); if (!p0.new) jump:t	00007FD4; p0 = cmp.gt(r8,00000000) }
60104018 BFE07FE2 7650C001 	{ r1 = sub(00000000,r16); r2 = add(r0,FFFFFFFF); loop0(00007FC8,r16) }
7F008000 AB02DA78 	{ memb(r2++#-1) = r26; nop }
F300C100     	{ r0 = add(r0,r1) }
B000411B 919DC041 	{ r1 = memw(r29+8); r27 = add(r0,00000008) }
85414000 5CDFF89E 	{ if (p0.new) jump:t	00007F1C; p0 = r1 }
F3325B01 915DC960 	{ r0 = memh(r29+150); r1 = sub(r27,r18) }
B00040E0 A1BDCA4B 9132C002 	{ memb(r29+75) = r0.new; r0 = add(r0,00000007) }
5C205824 75024600 479D409A 479DC0B0 	{ if (!p0.new) r16 = memw(r29+20); if (!p0.new) r26 = memw(r29+16); p0 = cmp.eq(r2,00000030); if (!p0.new) jump:t	00008040 }
B01D5402 919D40B0 919DC09A 	{ r26 = memw(r29+16); r16 = memw(r29+20); r2 = add(r29,000000A0) }
8CC24102 7F004000 7F00C000 	{ nop; nop; r2 = setbit(r2,00000002) }
BFE07FE0 BFE17FE1 A1BDCC4B 9B22C023 	{ memb(r29+75) = r0.new; r1 = add(r1,FFFFFFFF); r0 = add(r0,FFFFFFFF) }
5CDF78F8 7503C600 	{ p0 = cmp.eq(r3,00000030); if (p0.new) jump:t	00008020 }
58004004 BFE2FFF2 	{ r18 = add(r2,FFFFFFFF); jump	00008040 }
0C823C33     	{ r3 = memd(r29+12); r2 = memw(r16+16) }
85434000 5C20480A 70E0E400 	{ if (p0.new) r0 = sxth(r0); if (!p0.new) jump:nt	0000805C; p0 = r3 }
5800400C B000C023 	{ r3 = add(r0,00000001); jump	00008068 }
751A4CA0 751AC8A1 	{ p1 = cmp.eq(r26,00000045); p0 = cmp.eq(r26,00000065) }
6B21C000     	{ p0 = or(p0,p1) }
7A00C023     	{ r3 = mux(p0,00000001,00000000) }
D5024300 0FFF7C00 28133832 F2404100 74016000 7482E000 	{ if (!p0.new) r0 = add(r2,00000000); if (p0.new) r0 = add(r1,00000000); p0 = cmp.gt(r0,r1); r2 = add(r2,r3); r3 = 00000001; r0 = add(r3.l,r2.l) }
70004002 2182E330 78004603 	{ if (cmp.gtu(r3,r2.new)) jump:t	000080E4; r2 = aslh(r0) }
70E04002 21C2E10E 3A32C201 	{ if (!cmp.gtu(r1,r2.new)) jump:t	000080AC; r2 = sxth(r0) }
75814680 7E006723 7E80E603 	{ if (!p0.new) r3 = 00000030; if (p0.new) r3 = 00000039; p0 = cmp.gtu(r1,00000034) }
B000C020     	{ r0 = add(r0,00000001) }
F3124201 33003322 	{ r2 = r2; r0 = r0; r1 = add(r18,r2) }
97217FE1 2033E3FC 	{ r1 = memb(r1-1) }
75034720 FB126203 7401E021 	{ if (p0.new) r1 = add(r1,00000001); if (p0.new) r3 = add(r18,r2); p0 = cmp.eq(r3,00000039) }
1182610E 4003C100 	{ if (p0) memb(r3) = r1; if (p0.new) jump:t	000080E0; p0 = tstbit(r2,00000000) }
BFF27FF2 B0004020 915DC961 	{ r1 = memh(r29+150); r0 = add(r0,00000001); r18 = add(r18,FFFFFFFF) }
B0014021 A1BDCB4B 	{ r1 = add(r1,00000001) }
70E04003 70724002 F51A5000 915DC964 	{ r4 = memh(r29+150); r1:r0 = combine(r26,r16); r2 = r18; r3 = sxth(r0) }
5A00CB48     	{ call	00009780 }
5800D12E     	{ jump	0000A350 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _LDunscale: 00008100
_LDunscale proc
5A004A68 A09DC000 	{ allocframe(+00000000); call	000095D0 }
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }

;; _Litob: 00008110
_Litob proc
5A0050B0 0000435C 69823C90 75014B00 0000435C 2B033008 5C20590C 74024011 74834011 7501CDE1 	{ p1 = cmp.eq(r1,0000006F); if (!p0) r17 = add(r3,00000000); if (p0) r17 = add(r2,00000000); if (!p1.new) jump:t	00008140; r16 = r0; r3 = 00000030; p0 = cmp.eq(r1,00000058); allocframe(00000048); r2 = 00000018; call	0000A270 }
160E4824 91D0C014 	{ r21:r20 = memd(r16); jump	00008180; r14 = 00000008 }
75014F10 75014B11 75014C82 91D0C002 	{ r3:r2 = memd(r16); p2 = cmp.eq(r1,00000064); p1 = !cmp.eq(r1,00000058); p0 = !cmp.eq(r1,00000078) }
6B014000 5C00420E 7E006156 7E80E216 	{ if (!p0.new) r22 = 00000010; if (p0.new) r22 = 0000000A; if (p2) jump:nt	00008170; p0 = fastcorner9(p1,p0) }
5C205810 FD036294 7501CD20 	{ p0 = cmp.eq(r1,00000069); if (!p0.new) r21:r20 = combine(r3,r2); if (!p0.new) jump:t	00008180 }
F5034214 7800C7E0 	{ r0 = 0000003F; r21:r20 = combine(r3,r2) }
8214FF94     	{  }
CB624014 5800C004 	{ jump	00008184; XOREQ	r21:r20,asr(r3:r2,r0) }
7C00C000     	{ r1:r0 = combine(00000000,00000000) }
D2944000 5C20D808 	{ if (!p0.new) jump:t	00008198; p0 = cmp.eq(r21:r20,r1:r0) }
91904180 2402C0A4 F5155400 	{ if (cmp.eq(r0.new,00000001)) jump:nt	000082D8; r0 = memw(r16+48) }
B01D4018 297F3DE5 	{ r19:r18 = combine(00000000,00000003); r23 = 00000017; r24 = add(r29,00000000) }
5A0051F8 F513D202 	{ r3:r2 = combine(r19,r18); call	0000A590 }
3A314000 A1B8C217 7C006018 	{ memb(r24+23) = r0.new; r0 = memb(r20+r0) }
5A005198 F5155400 F513D202 	{ r3:r2 = combine(r19,r18); r1:r0 = combine(r21,r20); call	0000A4E4 }
F501C014     	{ r21:r20 = combine(r1,r0) }
D2985440 5C004836 A1D0D400 	{ memd(r16) = r21:r20; if (p0.new) jump:nt	00008234; p0 = cmp.gt(r25:r24,r21:r20) }
BFF77FFA B01D4017 7F004000 7F00C000 	{ nop; nop; r23 = add(r29,00000000); r26 = add(r23,FFFFFFFF) }
5A005100 F5155400 F5135202 F317DA1B 	{ r27 = add(r23,r26); r3:r2 = combine(r19,r18); r1:r0 = combine(r21,r20); call	0000A3E0 }
D2984040 E5405204 BFFA7FE2 A1D0C000 	{ memd(r16) = r1:r0; r2 = add(r26,FFFFFFFF); r5:r4 = mpyu(r0,r18); p0 = cmp.gt(r25:r24,r1:r0) }
EF00D305     	{ r5 += mpyi(r0,r19) }
EF12C105     	{ r5 += mpyi(r18,r1) }
F505C404     	{ r5:r4 = combine(r5,r4) }
D32454E4 FD01C094 	{ if (p0.new) r21:r20 = combine(r1,r0); r5:r4 = sub(r21:r20,r5:r4) }
5C00400C 3A314403 A1BBC300 	{ r3 = memb(r20+r4); if (p0) jump:nt	0000822C }
5CDF78E0 755A4000 7062C01A 	{ r26 = r2; p0 = cmp.gt(r26,00000000); if (p0.new) jump:t	000081E0 }
B002C037     	{ r23 = add(r2,00000001) }
104E681C 4330C780 	{ if (p0.new) r0 = memb(r16-4); if (!p0.new) jump:t	00008268; p0 = cmp.eq(r14,00000010) }
759742E0 76004100 2402C018 5C00400A 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00008270; r0 = and(r0,00000008); p0 = cmp.gtu(r23,00000017) }
749DC000     	{ if (!p0) r0 = add(r29,00000000) }
3A20D700     	{ r0 = memb(r13+r23) }
5C00580C 7500C600 	{ p0 = cmp.eq(r0,00000030); if (p0.new) jump:t	00008268 }
B01DC000     	{ r0 = add(r29,00000000) }
DB6060F7 BFF7FFF7 	{ r23 = add(r23,FFFFFFFF); r0 = add(r0,add(r23,0000003F)) }
3C00C030     	{ memb(r0) = 30 }
76574302 4C010480 	{ r0 = memw(r16+16); r1 = add(r29,00000000); r2 = sub(00000018,r23) }
5BFF78B8 78F10782 	{ memw(r16+28) = r2; r1 = add(r1,r23); call	000073E0 }
9190C181     	{ r1 = memw(r16+48) }
75617FE0 919040E0 21C2E114 F3204100 	{ if (!cmp.gtu(r1,r0.new)) jump:t	000082AC; r0 = memw(r16+28); p0 = cmp.gt(r1,FFFFFFFF) }
9170C3C2     	{ r2 = memuh(r16+60) }
000043FF 760245E1 A190C006 	{ memw(r16+24) = r0; r1 = and(r2,0000FFEF) }
5800505A A150C11E 	{ memuh(r16+60) = r1; jump	0000A350 }
5C004016 4570C3C1 	{ if (!p0) r1 = memuh(r16+60); if (p0) jump:nt	000082D0 }
76014281 2443F012 	{ r1 = and(r1,00000014) }
0E810582     	{ r2 = memw(r16+20); r1 = memw(r16+24) }
F3204100 9190C0C3 	{ r3 = memw(r16+24); r0 = sub(r1,r0) }
F322C000     	{ r0 = sub(r0,r2) }
F323C000     	{ r0 = sub(r0,r3) }
75404000 4290C030 	{ if (p0.new) memw(r16+24) = r0; p0 = cmp.gt(r0,00000000) }
5800D040     	{ jump	0000A350 }
163F586E 7336E012 	{ jump	000081B0; r15 = 00000018 }
7F00C000     	{ nop }

;; _LXp_getw: 000082E0
_LXp_getw proc
75014000 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; p0 = cmp.eq(r1,00000000) }
5C004030 49C0C310 	{ r17:r16 = memd(gp+192); if (p0) jump:nt	00008348 }
1041E106     	{ if (!p0.new) jump:t	000082FC; p0 = cmp.eq(r1,00000002) }
5800402A 91C0C010 	{ r17:r16 = memd(r0); jump	00008348 }
91C04010 49C0C302 	{ r3:r2 = memd(gp+192); r17:r16 = memd(r0) }
D2F04200 5C00D822 	{ if (p0.new) jump:t	0000834C; p0 = dfcmp.eq(r17:r16,r3:r2) }
10C16110 91C0C024 	{ r5:r4 = memd(r0+8); if (!p0.new) jump:t	0000832C; p0 = cmp.gt(r1,00000002) }
D2E44200 5C00D81A 	{ if (p0.new) jump:t	0000834C; p0 = dfcmp.eq(r5:r4,r3:r2) }
10416208 FD056402 FD11F000 	{ if (!p0) r1:r0 = combine(r17,r16); if (!p0) r3:r2 = combine(r5,r4); if (!p0.new) jump:t	0000832C; p0 = cmp.eq(r1,00000004) }
5800C00C     	{ jump	00008340 }
5A00516A F5054400 91C0C042 	{ r3:r2 = memd(r0+16); r1:r0 = combine(r5,r4); call	0000A600 }
F5014002 F511D000 	{ r1:r0 = combine(r17,r16); r3:r2 = combine(r1,r0) }
5A00D160     	{ call	0000A600 }
F501C010     	{ r17:r16 = combine(r1,r0) }
F5115000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r1:r0 = combine(r17,r16) }

;; _LXp_setw: 00008350
_LXp_setw proc
5A004F94 A09DC006 	{ allocframe(+00000030); call	0000A278 }
F5034212 F501C010 	{ r17:r16 = combine(r1,r0); r19:r18 = combine(r3,r2) }
10C94074 A1DDD201 	{ memd(r29+8) = r19:r18; if (!p0.new) jump:nt	00008448; p0 = cmp.gt(r9,00000000) }
1009611C 749D60C0 749DE101 	{ if (!p0.new) r1 = add(r29,00000008); if (!p0.new) r0 = add(r29,00000006); if (p0.new) jump:t	000083A0; p0 = cmp.eq(r9,00000002) }
5BFFFEC6     	{ call	00008100 }
75404000 70C04001 2403C012 	{ r1 = zxth(r0); p0 = cmp.gt(r0,00000000) }
5C20C014     	{ if (!p0) jump:nt	000083AC }
5C013E08     	{ r1:r0 = memd(r29+8); r3:r2 = combine(00000000,00000000) }
0000407E 17084068 A1D04000 A1D0C201 	{ memd(r16+8) = r3:r2; memd(r16) = r1:r0; jump	0000845C; r0 = r8 }
91DDC032     	{ r19:r18 = memd(r29+8) }
0000407E 17084040 A1D0D200 	{ memd(r16) = r19:r18; jump	00008420; r0 = r8 }
5A004BD2 29A12C20 	{ r0 = add(r29,00000008); r1 = 0000001A; call	00009B50 }
5A004BD6 B01D4100 915DC061 	{ r1 = memh(r29+6); r0 = add(r29,00000008); call	00009B60 }
F5135200 91DDC022 	{ r3:r2 = memd(r29+8); r1:r0 = combine(r19,r18) }
5A00516E A1D0C200 	{ memd(r16) = r3:r2; call	0000A6A4 }
F5014012 7551C040 	{ p0 = cmp.gt(r17,00000002); r19:r18 = combine(r1,r0) }
5C204032 A1D0D201 	{ memd(r16+8) = r19:r18; if (!p0) jump:nt	0000843C }
49C0C316     	{ r23:r22 = memd(gp+192) }
D2F25601 5C00592C 74BD60C0 74B0E114 	{ if (!p1.new) r20 = add(r16,00000008); if (!p1.new) r0 = add(r29,00000006); if (p1.new) jump:t	00008440; p1 = dfcmp.eq(r19:r18,r23:r22) }
5BFF7E86 7074C001 	{ r1 = r20; call	00008100 }
5A004BAA 7334E340 	{ call	00009B50 }
5A004BAE 70744000 915DC061 	{ r1 = memh(r29+6); r0 = r20; call	00009B60 }
5A00514A F5135200 91D4C002 	{ r3:r2 = memd(r20); r1:r0 = combine(r19,r18); call	0000A6A4 }
10C96316 A1D0C002 	{ memd(r16+16) = r1:r0; if (!p0.new) jump:t	00008448; p0 = cmp.gt(r9,00000006) }
D2E05600 5C00D812 	{ if (p0.new) jump:t	0000844C; p0 = dfcmp.eq(r1:r0,r23:r22) }
7C00C000     	{ r1:r0 = combine(00000000,00000000) }
0000407C 17084020 A1D0C003 	{ memd(r16+24) = r1:r0; jump	00008470; r0 = r8 }
5C20C006     	{ if (!p0) jump:nt	00008448 }
7C00C000     	{ r1:r0 = combine(00000000,00000000) }
A1D0C002     	{ memd(r16+16) = r1:r0 }
0000407B 1708C070 5A004F10 A09DC00C 	{ allocframe(+00000060); call	0000A270; jump	00008528; r0 = r8 }

;; _LXp_addh: 00008450
_LXp_addh proc
5A004F10 A09DC00C 	{ allocframe(+00000060); call	0000A270 }
F5034212 70192820 	{ memw(r29+8) = r0; r17 = r1; r19:r18 = combine(r3,r2) }
10194090 EA2D0A25 	{ memd(r29+32) = r19:r18; memd(r29+40) = r19:r18; if (p0.new) jump:nt	00008780; p0 = cmp.eq(r25,00000001) }
5BFF7E4C B01D4401 B01DC3C0 	{ r0 = add(r29,0000001E); r1 = add(r29,00000020); call	00008100 }
10C0C036     	{ if (!p0.new) jump:nt	000084E0; p0 = cmp.gt(r0,00000000) }
DD004048 5C00786E 479DC040 	{ if (!p0.new) r0 = memw(r29+8); if (p0.new) jump:t	00008758; p0 = cmph.eq(r0,0002) }
5A00CB76     	{ call	00009B70 }
10D0C064     	{ if (!p0.new) jump:nt	00008750; p0 = cmp.gt(r16,00000000) }
DD004048 5C00F87A 	{ if (p0.new) jump:t	00008784; p0 = cmph.eq(r0,0002) }
4CA13C20     	{ r0 = memd(r29+8); r1 = add(r29,00000028) }
768140C1 9140C060 	{ r0 = memh(r0+6); r1 = or(r1,00000006) }
9141C001     	{ r1 = memh(r1) }
F160C100     	{ r0 = xor(r0,r1) }
70E04000 2692E06C 5A0048E0 	{ if (tstbit(r0.new,-00000001)) jump:t	00008784; r0 = sxth(r0) }
00004398 2B082810 75514020 91D04000 919DC042 	{ r2 = memw(r29+8); r1:r0 = memd(r16); p0 = cmp.gt(r17,00000001); r0 = 00000001; r16 = 00000030 }
5C20605C A1C2C000 	{ memd(r2) = r1:r0; if (!p0) jump:nt	00008780 }
5C003C22     	{ r2 = memd(r29+8); r1:r0 = combine(00000000,00000000) }
58004F3E 919D4040 A1C2C001 	{ memd(r2+8) = r1:r0; r0 = memw(r29+8); jump	0000A350 }
1190E150     	{ if (p0.new) jump:t	00008780; p0 = tstbit(r0,00000000) }
10D9604E 7E00E010 	{ if (p0.new) r16 = 00000000; if (!p0.new) jump:t	00008780; p0 = cmp.gt(r25,00000000) }
BFF17FF4 78044013 919D4040 49C0C318 	{ r25:r24 = memd(gp+192); r0 = memw(r29+8); r19 = 00000800; r20 = add(r17,FFFFFFFF) }
7C004016 C4144060 B0004101 BFE0FF15 	{ r21 = add(r0,FFFFFFF8); r1 = add(r0,00000008); r0 = addasl(r0,r20,00000003); r23:r22 = combine(00000000,00000000) }
E8110800     	{ memw(r29) = r0; memw(r29+4) = r1 }
B010403A 3C201C11 	{ r1 = memd(r29+4); r0 = memd(r29+8); r26 = add(r16,00000001) }
C4104072 C410C17B 	{ r27 = addasl(r1,r16,00000003); r18 = addasl(r0,r16,00000003) }
58004008 91D2C002 	{ r3:r2 = memd(r18); jump	00008530 }
A1DD5605 A1D2C200 	{ memd(r18) = r3:r2; memd(r29+40) = r23:r22 }
5BFF7DE8 B01D41C0 6C412A11 	{ memd(r29+16) = r3:r2; r1 = add(r29,00000010); r0 = add(r29,0000000E); call	00008100 }
1090C022     	{ if (p0.new) jump:nt	00008780; p0 = cmp.gt(r16,00000000) }
70C04000 2412C012 915D41E0 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00008768; r0 = zxth(r0) }
915DC0E4     	{ r4 = memh(r29+14) }
F320C405     	{ r5 = sub(r4,r0) }
5C004836 749A6006 749B6001 7565FCC0 	{ p0 = cmp.gt(r5,FFFFFFE6); if (!p0.new) r1 = add(r27,00000000); if (!p0.new) r6 = add(r26,00000000); if (p0.new) jump:nt	000085C0 }
91DDC0A2     	{ r3:r2 = memd(r29+40) }
D2E25800 5C00C82C 	{ if (p0.new) jump:nt	000085C4; p0 = dfcmp.eq(r3:r2,r25:r24) }
70664000 21C2D10E B0004026 	{ if (!cmp.gtu(r17,r0.new)) jump:nt	00008590; r0 = r6 }
B0014101 91C1C004 	{ r5:r4 = memd(r1); r1 = add(r1,00000008) }
D2E45800 5CFFF8F6 	{ if (!p0.new) jump:t	00008574; p0 = dfcmp.eq(r5:r4,r25:r24) }
14CC6006 5800400A 7400E020 	{ if (p0.new) r0 = add(r0,00000001); jump	000085A4; if (!p0.new) jump:t	00008598; p0 = cmp.gt(r12,-00000001) }
F2114000 7400FFE0 	{ if (p0.new) r0 = add(r0,FFFFFFFF); p0 = cmp.eq(r17,r0) }
14F0E8C4     	{ if (!p0.new) jump:t	00008528; p0 = cmp.gt(r0,-00000001) }
C4005560 F330C001 	{ r1 = sub(r0,r16); r0 = addasl(r21,r0,00000003) }
6001C008     	{ loop0(000085B0,r1) }
91C0C004     	{ r5:r4 = memd(r0) }
BFE0BF00 A1C0C401 	{ memd(r0+8) = r5:r4; r0 = add(r0,FFFFFFF8) }
59FFFFB6     	{ jump	00008528 }
10C5790C 91DDC0A0 	{ r1:r0 = memd(r29+40); if (!p0.new) jump:t	000085D8; p0 = cmp.gt(r5,00000012) }
D2E05800 5C00D808 	{ if (p0.new) jump:t	000085DC; p0 = dfcmp.eq(r1:r0,r25:r24) }
17044BBC B010C030 	{ r16 = add(r16,00000001); jump	00008748; r11 = r4 }
5A005014 91D2C002 	{ r3:r2 = memd(r18); call	0000A600 }
F501C002     	{ r3:r2 = combine(r1,r0) }
D2E25800 5C205828 A1D2C200 	{ memd(r18) = r3:r2; if (!p0.new) jump:t	00008638; p0 = dfcmp.eq(r3:r2,r25:r24) }
50803C11     	{ r1 = memd(r29+4); r0 = r16 }
C4104161 7F004000 7F00C000 	{ nop; nop; r1 = addasl(r1,r16,00000003) }
B0014102 B0004020 21C2D110 91C1C004 	{ if (!cmp.gtu(r17,r0.new)) jump:nt	00008628; r0 = add(r0,00000001); r2 = add(r1,00000008) }
D2E45800 5CFF78F8 70624001 A7C1E4FF 	{ memd(r1-8) = r5:r4; r1 = r2; if (!p0.new) jump:t	00008604; p0 = dfcmp.eq(r5:r4,r25:r24) }
919DC000     	{ r0 = memw(r29) }
A1C0D600     	{ memd(r0) = r23:r22 }
91D2C002     	{ r3:r2 = memd(r18) }
D2E25800 5C00C8AA 	{ if (p0.new) jump:nt	00008784; p0 = dfcmp.eq(r3:r2,r25:r24) }
5BFF7D66 B01D43C0 6CA12A29 	{ memd(r29+40) = r3:r2; r1 = add(r29,00000028); r0 = add(r29,0000001E); call	00008100 }
BFF3FCC0     	{ r0 = add(r19,FFFFFFE6) }
915D41E1 20C3E04E 	{ r1 = memh(r29+30) }
D5204101 5A004A82 B01DC500 	{ r0 = add(r29,00000028); call	00009B54; r1 = sub(r1.l,r0.l) }
5A004A84 B01D4500 915DC1E1 	{ r1 = memh(r29+30); r0 = add(r29,00000028); call	00009B60 }
5A005020 91D24000 91DDC0A2 	{ r3:r2 = memd(r29+40); r1:r0 = memd(r18); call	0000A6A4 }
D2E05800 5C20581E A1D2C000 	{ memd(r18) = r1:r0; if (!p0.new) jump:t	000086B0; p0 = dfcmp.eq(r1:r0,r25:r24) }
50813C10     	{ r0 = memd(r29+4); r1 = r16 }
C410C060     	{ r0 = addasl(r0,r16,00000003) }
B0004102 B0014021 21C3D110 	{ r1 = add(r1,00000001); r2 = add(r0,00000008) }
91C0C004     	{ r5:r4 = memd(r0) }
D2E45800 5CFF78F8 70624000 A7C0E4FF 	{ memd(r0-8) = r5:r4; r0 = r2; if (!p0.new) jump:t	00008688; p0 = dfcmp.eq(r5:r4,r25:r24) }
919DC000     	{ r0 = memw(r29) }
A1C0D600     	{ memd(r0) = r23:r22 }
BFF07FFA 78044013 7800C010 	{ r16 = 00000000; r19 = 00000800; r26 = add(r16,FFFFFFFF) }
5C005848 749D6401 749D61C0 751AC000 	{ p0 = cmp.eq(r26,00000000); if (!p0.new) r0 = add(r29,0000000E); if (!p0.new) r1 = add(r29,00000020); if (p0.new) jump:t	00008748 }
707A4010 97D2FFC2 	{ r3:r2 = memd(r18-16); r16 = r26 }
5BFF7D18 A1DDC204 	{ memd(r29+32) = r3:r2; call	00008100 }
58004038 915DC0F3 	{ r19 = memh(r29+14); jump	00008748 }
B01D41C0 70724001 B0104030 2002D150 91D2C002 	{ if (cmp.eq(r16.new,r17)) jump:nt	0000878C; r16 = add(r16,00000001); r1 = r18; r0 = add(r29,0000000E) }
5BFF7D06 A1DDC205 	{ memd(r29+40) = r3:r2; call	00008100 }
5A004A2A 7332E340 	{ call	00009B50 }
5A004A2E 70724000 915DC0E1 	{ r1 = memh(r29+14); r0 = r18; call	00009B60 }
91D24012 91DDC0A0 	{ r1:r0 = memd(r29+40); r19:r18 = memd(r18) }
5A004FC6 F513D202 	{ r3:r2 = combine(r19,r18); call	0000A6A4 }
D2E05800 A1DDC005 	{ memd(r29+40) = r1:r0; p0 = dfcmp.eq(r1:r0,r25:r24) }
6BC04000 FD016002 FD137282 B01DC3C0 	{ r0 = add(r29,0000001E); if (!p0.new) r3:r2 = combine(r19,r18); if (!p0) r3:r2 = combine(r1,r0); p0 = not(p0) }
5BFF7CE4 B01D4401 915D40F3 A1DDC204 	{ memd(r29+32) = r3:r2; r19 = memh(r29+14); r1 = add(r29,00000020); call	00008100 }
14A9E8E4     	{ if (p0.new) jump:t	00008510; p0 = cmp.gt(r9,-00000001) }
5800C01A     	{ jump	00008780 }
91DDC0B2     	{ r19:r18 = memd(r29+40) }
919DC040     	{ r0 = memw(r29+8) }
58004DFC 919D4040 A1C0D200 	{ memd(r0) = r19:r18; r0 = memw(r29+8); jump	0000A350 }
51803E29     	{ r3:r2 = memd(r29+40); r0 = r16 }
14C9600C A1D2C200 	{ memd(r18) = r3:r2; if (!p0.new) jump:t	00008780; p0 = cmp.gt(r9,-00000001) }
919DC041     	{ r1 = memw(r29+8) }
58004DEE 919D4040 3BC1E096 	{ memd(r30+r0<<#3) = r23:r22; r0 = memw(r29+8); jump	0000A350 }
58004DE8 919DC040 	{ r0 = memw(r29+8); jump	0000A350 }

;; _LXp_mulh: 00008788
_LXp_mulh proc
5A004D74 A09DC00C 	{ allocframe(+00000060); call	0000A270 }
F5014010 F503C212 	{ r19:r18 = combine(r3,r2); r17:r16 = combine(r1,r0) }
10C940AA FD13F202 	{ if (!p0) r3:r2 = combine(r19,r18); if (!p0.new) jump:nt	000088EC; p0 = cmp.gt(r9,00000001) }
5A0051C0 91D0C000 	{ r1:r0 = memd(r16); call	0000AB20 }
F5014002 B01DC200 	{ r0 = add(r29,00000010); r3:r2 = combine(r1,r0) }
5A0049E0 A1DDC202 	{ memd(r29+16) = r3:r2; call	00009B70 }
7C004016 B01D4201 70604014 26C2C022 70D44000 	{ if (!tstbit(r20.new,-00000001)) jump:nt	00008808; r20 = r0; r1 = add(r29,00000010); r23:r22 = combine(00000000,00000000) }
2442E208 5A004750 	{ if (!cmp.eq(r0.new,00000004)) jump:t	000087DC }
7800C020     	{ r0 = 00000001 }
75544000 91DDC040 	{ r1:r0 = memd(r29+16); p0 = cmp.gt(r20,00000000) }
5C204086 A1D0C000 	{ memd(r16) = r1:r0; if (!p0) jump:nt	000088EC }
10C9E182     	{ if (!p0.new) jump:t	000088EC; p0 = cmp.gt(r9,00000003) }
7C00C000     	{ r1:r0 = combine(00000000,00000000) }
0000406D 17084040 A1D0C001 	{ memd(r16+8) = r1:r0; jump	00008870; r0 = r8 }
B0014100 7C004038 49C0C31A 	{ r27:r26 = memd(gp+192); r25:r24 = combine(00000001,00000000); r0 = add(r1,00000008) }
A19D4000 A1D0D600 	{ memd(r16) = r23:r22; memw(r29) = r0 }
5C00582C 749D6201 FB197880 7559C060 	{ p0 = cmp.gt(r25,00000003); if (!p0.new) r0 = add(r25,r24); if (!p0.new) r1 = add(r29,00000010); if (p0.new) jump:t	00008868 }
C4194175 C400D074 	{ r20 = addasl(r16,r0,00000003); r21 = addasl(r1,r25,00000003) }
F3185900 21C2D11C 91D4C000 	{ if (!cmp.gtu(r17,r0.new)) jump:nt	00008864; r0 = add(r24,r25) }
D2E05A00 5C004816 74996039 FD13F282 	{ if (!p0.new) r3:r2 = combine(r19,r18); if (!p0.new) r25 = add(r25,00000001); if (p0.new) jump:nt	00008864; p0 = dfcmp.eq(r1:r0,r27:r26) }
5A00D16E     	{ call	0000AB20 }
B0154115 75594060 ABD45608 A1D5C000 	{ memd(r21) = r1:r0; memd(r20++#8) = r23:r22; p0 = cmp.gt(r25,00000003); r21 = add(r21,00000008) }
5CFFE0E8     	{ if (!p0) jump:nt	00008828 }
5800C006     	{ jump	00008868 }
78004119 A1D5D600 	{ memd(r21) = r23:r22; r25 = 00000008 }
91DDC042     	{ r3:r2 = memd(r29+16) }
D2E25A00 5C004840 749D6101 749DE0C0 	{ if (!p0.new) r0 = add(r29,00000006); if (!p0.new) r1 = add(r29,00000008); if (p0.new) jump:nt	000088F0; p0 = dfcmp.eq(r3:r2,r27:r26) }
5BFF7C42 A1DDC201 	{ memd(r29+8) = r3:r2; call	00008100 }
5A004966 29A12C20 	{ r0 = add(r29,00000008); r1 = 0000001A; call	00009B50 }
5A00496A B01D4100 915DC061 	{ r1 = memh(r29+6); r0 = add(r29,00000008); call	00009B60 }
5BFF7DDC F5115000 91DDC022 	{ r3:r2 = memd(r29+8); r1:r0 = combine(r17,r16); call	00008450 }
5A004F00 3E091E10 	{ r1:r0 = memd(r29+16); r3:r2 = memd(r29+8); call	0000A6A4 }
5BFF7DD2 F5014002 F511D000 	{ r1:r0 = combine(r17,r16); r3:r2 = combine(r1,r0); call	00008450 }
7F004000 48003C01 	{ r1 = memd(r29); r0 = 00000000; nop }
B0014102 B0004020 21C2D910 91C1C004 	{ if (!cmp.gtu(r25,r0.new)) jump:nt	000088E8; r0 = add(r0,00000001); r2 = add(r1,00000008) }
D2E45A00 5CFF78F8 70624001 A7C1E4FF 	{ memd(r1-8) = r5:r4; r1 = r2; if (!p0.new) jump:t	000088C4; p0 = dfcmp.eq(r5:r4,r27:r26) }
BFF97FF9 B0184038 21B2F198 00004069 	{ if (cmp.gtu(r17,r24.new)) jump:t	00008818; r24 = add(r24,00000001); r25 = add(r25,FFFFFFFF) }
1708C048     	{ jump	00008980; r0 = r8 }

;; _LXp_movx: 000088F4
_LXp_movx proc
8C014343 70213C10 	{ allocframe(00000008); r1 = r2; r3 = asl(r1,00000003) }
5BFF7572 70604010 70322A04 	{ memd(r29) = r17:r16; r2 = r3; r16 = r0; call	000073E0 }
70704000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = r16 }

;; _LXp_addx: 00008910
_LXp_addx proc
5A004CB4 A09DC004 	{ allocframe(+00000020); call	0000A278 }
F5004310 F502C112 	{ r19:r18 = combine(r2,r1); r17:r16 = combine(r0,r3) }
10C84016 7E00E014 	{ if (p0.new) r20 = 00000000; if (!p0.new) jump:nt	0000894C; p0 = cmp.gt(r8,00000000) }
49C0C316     	{ r23:r22 = memd(gp+192) }
91D3C002     	{ r3:r2 = memd(r19) }
D2E25600 5C00480E 74936113 FD12F180 	{ if (!p0.new) r1:r0 = combine(r18,r17); if (!p0.new) r19 = add(r19,00000008); if (p0.new) jump:nt	00008950; p0 = dfcmp.eq(r3:r2,r23:r22) }
5BFFFD88     	{ call	00008450 }
B0144034 21B2F0F4 00004067 	{ if (cmp.gtu(r16,r20.new)) jump:t	00008930; r20 = add(r20,00000001) }
1709C068     	{ jump	00008A20; r0 = r9 }

;; _LXp_subx: 00008954
_LXp_subx proc
5A004C92 A09DC004 	{ allocframe(+00000020); call	0000A278 }
F5004310 F502C112 	{ r19:r18 = combine(r2,r1); r17:r16 = combine(r0,r3) }
10C84018 7E00E014 	{ if (p0.new) r20 = 00000000; if (!p0.new) jump:nt	00008994; p0 = cmp.gt(r8,00000000) }
49C0C316     	{ r23:r22 = memd(gp+192) }
91D3C002     	{ r3:r2 = memd(r19) }
D2E25600 5C004810 74946034 FD12F180 	{ if (!p0.new) r1:r0 = combine(r18,r17); if (!p0.new) r20 = add(r20,00000001); if (p0.new) jump:nt	00008998; p0 = dfcmp.eq(r3:r2,r23:r22) }
8CC35F43 5BFFFD66 	{ call	00008454; r3 = togglebit(r3,0000001E) }
14B86CF2 B013C113 	{ r19 = add(r19,00000008); if (p0.new) jump:t	00008970; p0 = cmp.gt(r8,-00000001) }
00004066 17094058 7F00C000 	{ nop; jump	00008A44; r0 = r9 }

;; _LXp_ldexpx: 000089A0
_LXp_ldexpx proc
5A004C6C A09DC004 	{ allocframe(+00000020); call	0000A278 }
F5014010 73227FF2 	{ r17:r16 = combine(r1,r0) }
49C0C314     	{ r21:r20 = memd(gp+192) }
70704016 7F004000 7F00C000 	{ nop; nop; r22 = r16 }
B0164117 70724002 B0134033 21C3D114 	{ r19 = add(r19,00000001); r2 = r18; r23 = add(r22,00000008) }
5A004550 91D6C000 	{ r1:r0 = memd(r22); call	00009470 }
D2E05400 5CFF78F4 70774016 A1D6C000 	{ memd(r22) = r1:r0; r22 = r23; if (!p0.new) jump:t	000089C4; p0 = dfcmp.eq(r1:r0,r21:r20) }
00004065 1708C030 5A004C40 A09DC006 	{ allocframe(+00000030); call	0000A270; jump	00008A48; r0 = r8 }

;; _LXp_mulx: 000089F0
_LXp_mulx proc
5A004C40 A09DC006 	{ allocframe(+00000030); call	0000A270 }
F5014010 F5034212 7064C014 	{ r20 = r4; r19:r18 = combine(r3,r2); r17:r16 = combine(r1,r0) }
1009E05A     	{ if (p0.new) jump:t	00008AB8; p0 = cmp.eq(r9,00000000) }
100BE058     	{ if (p0.new) jump:t	00008AB8; p0 = cmp.eq(r11,00000000) }
91D24002 49C0C318 	{ r25:r24 = memd(gp+192); r3:r2 = memd(r18) }
D2E25800 5C00D80C 	{ if (p0.new) jump:t	00008A30; p0 = dfcmp.eq(r3:r2,r25:r24) }
91D2C020     	{ r1:r0 = memd(r18+8) }
D2E05800 5C20480E FD10F480 	{ if (!p0.new) r1:r0 = combine(r16,r20); if (!p0.new) jump:nt	00008A40; p0 = dfcmp.eq(r1:r0,r25:r24) }
5BFF7EAE F511D000 	{ r1:r0 = combine(r17,r16); call	00008788 }
00004064 1708C038 8C11C355 	{ r21 = asl(r17,00000003); jump	00008AA4; r0 = r8 }
5BFF74D0 7075C002 	{ r2 = r21; call	000073E0 }
5BFF7EA0 F5115000 91D2C002 	{ r3:r2 = memd(r18); r1:r0 = combine(r17,r16); call	00008788 }
10CB6132 7E00E037 	{ if (p0.new) r23 = 00000001; if (!p0.new) jump:t	00008AB8; p0 = cmp.gt(r11,00000002) }
C411D476     	{ r22 = addasl(r20,r17,00000003) }
C417D27A     	{ r26 = addasl(r18,r23,00000003) }
91DAC000     	{ r1:r0 = memd(r26) }
D2E05800 5C004828 74956002 FD14F680 	{ if (!p0.new) r1:r0 = combine(r20,r22); if (!p0.new) r2 = add(r21,00000000); if (p0.new) jump:nt	00008ABC; p0 = dfcmp.eq(r1:r0,r25:r24) }
5BFFF4B4     	{ call	000073E0 }
5BFF7E86 F5115600 7316601A 	{ r1:r0 = combine(r17,r22); call	00008788 }
91DAC002     	{ r3:r2 = memd(r26) }
10C9E012     	{ if (!p0.new) jump:t	00008AB0; p0 = cmp.gt(r9,00000000) }
91DBC002     	{ r3:r2 = memd(r27) }
D2E25800 5C00480E 749B611B FD11F080 	{ if (!p0.new) r1:r0 = combine(r17,r16); if (!p0.new) r27 = add(r27,00000008); if (p0.new) jump:nt	00008AB4; p0 = dfcmp.eq(r3:r2,r25:r24) }
5BFFFCD6     	{ call	00008450 }
B01A403A 21B2F1F4 B0174037 	{ if (cmp.gtu(r17,r26.new)) jump:t	00008A94; r26 = add(r26,00000001) }
21B3F3D8     	{  }
00004062 1708C030 5A004BD8 A09DC006 	{ allocframe(+00000030); call	0000A270; jump	00008B18; r0 = r8 }

;; _LXp_invx: 00008AC0
_LXp_invx proc
5A004BD8 A09DC006 	{ allocframe(+00000030); call	0000A270 }
F5014010 7062C016 	{ r22 = r2; r17:r16 = combine(r1,r0) }
10094096 7490E000 	{ if (!p0.new) r0 = add(r16,00000000); if (p0.new) jump:nt	00008BFC; p0 = cmp.eq(r9,00000001) }
5A00C84C     	{ call	00009B70 }
11C0C112     	{ if (!p0.new) jump:t	00008B00; p0 = tstbit(r0,00000000) }
DD004008 5C20D878 	{ if (!p0.new) jump:t	00008BD4; p0 = cmph.eq(r0,0000) }
00004398 7800C400 91C0C000 	{ r1:r0 = memd(r0); r0 = 0000E620 }
00004061 17084038 A1D0C000 	{ memd(r16) = r1:r0; jump	00008B64; r0 = r8 }
C4115673 8C114354 49C04398 91D0C01A 	{ r27:r26 = memd(r16); r25:r24 = memd(gp+224); r20 = asl(r17,00000003); r19 = addasl(r22,r17,00000003) }
5BFF7468 F5105300 7074C002 	{ r2 = r20; r1:r0 = combine(r16,r19); call	000073E0 }
5BFF7E36 F5115300 F519D802 	{ r3:r2 = combine(r25,r24); r1:r0 = combine(r17,r19); call	00008788 }
10C96160 FD1B7A82 FD1BFA00 	{ if (!p0) r1:r0 = combine(r27,r26); if (!p0.new) r3:r2 = combine(r27,r26); if (!p0.new) jump:t	00008BE8; p0 = cmp.gt(r9,00000002) }
5A004D66 91D04022 49C0C378 	{ r25:r24 = memd(gp+216); r3:r2 = memd(r16+8); call	0000A600 }
5A004EA0 F5014002 F519D800 	{ r1:r0 = combine(r25,r24); r3:r2 = combine(r1,r0); call	0000A880 }
F5014002 F5115000 78004038 49C0C31A 	{ r27:r26 = memd(gp+192); r24 = 00000001; r1:r0 = combine(r17,r16); r3:r2 = combine(r1,r0) }
C4115695 5BFFFBFA 	{ call	00008354; r21 = addasl(r22,r17,00000004) }
5BFF743E F5105600 7074C002 	{ r2 = r20; r1:r0 = combine(r16,r22); call	000073E0 }
5BFF7F40 F5115302 70754004 F511D600 	{ r1:r0 = combine(r17,r22); r4 = r21; r3:r2 = combine(r17,r19); call	000089F0 }
5BFF7C68 F5115600 49C0C362 	{ r3:r2 = memd(gp+216); r1:r0 = combine(r17,r22); call	00008450 }
5BFF7F32 F5115002 F5115600 7075C004 	{ r4 = r21; r1:r0 = combine(r17,r22); r3:r2 = combine(r17,r16); call	000089F0 }
78004019 7076C012 	{ r18 = r22; r25 = 00000000 }
91D2C002     	{ r3:r2 = memd(r18) }
D2E25A00 5C00480E 74926112 FD11F080 	{ if (!p0.new) r1:r0 = combine(r17,r16); if (!p0.new) r18 = add(r18,00000008); if (p0.new) jump:nt	00008BC8; p0 = dfcmp.eq(r3:r2,r27:r26) }
5BFFFC4C     	{ call	00008450 }
B0194039 21B3F1F4 	{ r25 = add(r25,00000001) }
8C184158 21B2F1D0 5800C018 	{ if (cmp.gtu(r17,r24.new)) jump:t	00008B68; r24 = asl(r24,00000001) }
70C04000 2442E116 7C00C000 	{ if (!cmp.eq(r0.new,00000002)) jump:t	00008C00; r0 = zxth(r0) }
0000405D 17084068 A1D0C000 	{ memd(r16) = r1:r0; jump	00008CAC; r0 = r8 }
5A004E4C 49C0C360 	{ r1:r0 = memd(gp+216); call	0000A880 }
5BFF7BB0 F5014002 F511D000 	{ r1:r0 = combine(r17,r16); r3:r2 = combine(r1,r0); call	00008350 }
0000405D 1708C028 5A004B36 A09DC006 	{ allocframe(+00000030); call	0000A270; jump	00008C4C; r0 = r8 }

;; _LXp_sqrtx: 00008C04
_LXp_sqrtx proc
5A004B36 A09DC006 	{ allocframe(+00000030); call	0000A270 }
F5014010 7062C012 	{ r18 = r2; r17:r16 = combine(r1,r0) }
10094086 7490E000 	{ if (!p0.new) r0 = add(r16,00000000); if (p0.new) jump:nt	00008D20; p0 = cmp.eq(r9,00000001) }
5A00C7AA     	{ call	00009B70 }
1180610A 91D0C004 	{ r5:r4 = memd(r16); if (p0.new) jump:t	00008C34; p0 = tstbit(r0,00000000) }
49C0C300     	{ r1:r0 = memd(gp+192) }
D2E44040 5C00D81A 	{ if (p0.new) jump:t	00008C64; p0 = dfcmp.ge(r5:r4,r1:r0) }
49C0C300     	{ r1:r0 = memd(gp+192) }
D2E44040 5C005874 7E80E020 	{ if (!p0.new) r0 = 00000001; if (p0.new) jump:t	00008D24; p0 = dfcmp.ge(r5:r4,r1:r0) }
5A00C516     	{ call	00009670 }
00004398 7800C600 91C0C000 	{ r1:r0 = memd(r0); r0 = 0000E630 }
0000405B 17084078 A1D0C000 	{ memd(r16) = r1:r0; jump	00008D44; r0 = r8 }
C4115273 10C9610C FD05E400 	{ if (!p0) r1:r0 = combine(r5,r4); if (!p0.new) jump:t	00008C7C; p0 = cmp.gt(r9,00000002); r19 = addasl(r18,r17,00000003) }
5A004CCA 91D0C022 	{ r3:r2 = memd(r16+8); call	0000A600 }
F501C004     	{ r5:r4 = combine(r1,r0) }
5A004454 F5054400 49C0C374 	{ r21:r20 = memd(gp+216); r1:r0 = combine(r5,r4); call	00009520 }
5A004DFE F5014002 F515D400 	{ r1:r0 = combine(r21,r20); r3:r2 = combine(r1,r0); call	0000A880 }
5BFF7B60 C4115294 F5014002 F511D300 	{ r1:r0 = combine(r17,r19); r3:r2 = combine(r1,r0); r20 = addasl(r18,r17,00000004); call	00008350 }
10C96238 7E00E05A 	{ if (p0.new) r26 = 00000002; if (!p0.new) jump:t	00008D10; p0 = cmp.gt(r9,00000004) }
8C114355 49C043B6 49C0C3D8 	{ r25:r24 = memd(gp+240); r23:r22 = memd(gp+232); r21 = asl(r17,00000003) }
5BFF7396 F5135200 7075C002 	{ r2 = r21; r1:r0 = combine(r19,r18); call	000073E0 }
5BFF7D64 F5115200 F517D602 	{ r3:r2 = combine(r23,r22); r1:r0 = combine(r17,r18); call	00008788 }
5BFF7E92 70744004 F5115200 F511D002 	{ r3:r2 = combine(r17,r16); r1:r0 = combine(r17,r18); r4 = r20; call	000089F0 }
5BFF7E8A F5115302 F5115200 7074C004 	{ r4 = r20; r1:r0 = combine(r17,r18); r3:r2 = combine(r17,r19); call	000089F0 }
5BFF7BB2 F5115200 F519D802 	{ r3:r2 = combine(r25,r24); r1:r0 = combine(r17,r18); call	00008450 }
5BFF7E7C F5115300 F5115202 7074C004 	{ r4 = r20; r3:r2 = combine(r17,r18); r1:r0 = combine(r17,r19); call	000089F0 }
8C1A415A 21B2F1D6 5BFF7E70 	{ if (cmp.gtu(r17,r26.new)) jump:t	00008CB8; r26 = asl(r26,00000001) }
F5115000 70744004 F511D302 	{ r3:r2 = combine(r17,r19); r4 = r20; r1:r0 = combine(r17,r16) }
00004058 1708C060 7F00C000 	{ nop; jump	00008DE0; r0 = r8 }
7F00C000     	{ nop }

;; _Mbtowcx: 00008D30
_Mbtowcx proc
79103C00     	{ allocframe(00000000); p0 = cmp.eq(r1,00000000) }
00352336     	{ r6 = memh(r3+6); r5 = memw(r3) }
91844007 2403C070 	{ r7 = memw(r4) }
5C0040DE 74814007 7E80C008 	{ if (!p0) r8 = 00000000; if (!p0) r7 = add(r1,00000000); if (p0) jump:nt	00008EFC }
1002C0C8     	{ if (p0.new) jump:nt	00008EDC; p0 = cmp.eq(r2,00000001) }
DD4641E0 5C0048CE 74886028 7086EC06 	{ if (!p0.new) r6 = zxtb(r6); if (!p0.new) r8 = add(r8,00000001); if (p0.new) jump:nt	00008EF0; p0 = cmpb.gtu(r6,0F) }
0000403F 754845E0 3A846606 2402C0C6 5C0040BE 	{ if (cmp.eq(r6.new,00000001)) jump:nt	00008EF8; r6 = memw(r6+r6<<#2); p0 = cmp.gt(r8,00000FEF) }
7485400D 4527C00C 	{ if (!p0) r12 = memb(r7); if (!p0) r13 = add(r5,00000000) }
3A664C86 2402C0B8 00004200 	{ if (cmp.eq(r6.new,00000001)) jump:nt	00008EF0; r6 = memuh(r4+r12<<#1) }
7606400F 7606DFEE 	{ r14 = and(r6,000000FF); r15 = and(r6,00000000) }
DA6D600E 00004100 76064009 750FC000 	{ p0 = cmp.eq(r15,00000000); r9 = and(r6,00004000); r13 = or(r14,and(r13,FFFFFF00)) }
00004040 7606400E 75094001 748DC005 	{ if (!p0) r5 = add(r13,00000000); p1 = cmp.eq(r9,00000000); r14 = and(r6,00001000) }
8C054849 750EC000 	{ p0 = cmp.eq(r14,00000000); r9 = asl(r5,00000008) }
8E4558A9 5C00C112 	{ if (p1) jump:nt	00008DE0; r9 |= lsr(r5,00000018) }
750C4012 750C4001 74A76027 7E4FFFED 	{ if (p2.new) r13 = FFFFFFFF; if (!p1.new) r7 = add(r7,00000001); p1 = cmp.eq(r12,00000000); p2 = !cmp.eq(r12,00000000) }
7EC0400D 7EA0C008 	{ if (!p1) r8 = 00000000; if (!p2) r13 = 00000000 }
F30DC202     	{ r2 = add(r13,r2) }
8D264406 00004080 7606400C 7489C005 	{ if (!p0) r5 = add(r9,00000000); r12 = and(r6,00002000); r6 = extractu(r6,00000004,0000000C) }
5CDF78B0 FB216782 750CC000 	{ p0 = cmp.eq(r12,00000000); if (!p0.new) r2 = sub(r7,r1); if (p0.new) jump:t	00008D4C }
F2074101 79000035 	{ memw(r3) = r5; p0 = cmp.eq(r0,00000000); p1 = cmp.eq(r7,r1) }
75054000 7E2F5FA0 74A24000 4480C500 	{ if (!p1) r0 = add(r2,00000000); if (p1) r0 = FFFFFFFD; p0 = cmp.eq(r5,00000000) }
7A602336     	{ memuh(r3+8) = r6; r0 = -00000001 }
961EC01E     	{ dealloc_return }
1001C07E     	{ if (p0.new) jump:nt	00008F14; p0 = cmp.eq(r1,00000000) }
10024060 7481E024 	{ if (!p0.new) r4 = add(r1,00000001); if (p0.new) jump:nt	00008EDC; p0 = cmp.eq(r2,00000000) }
DD064000 5C205816 9724FFE7 	{ r7 = memb(r4-1); if (!p0.new) jump:t	00008E54; p0 = cmpb.eq(r6,00) }
76075005 2403C078 	{ r5 = and(r7,00000080) }
7607DC05     	{ r5 = and(r7,000000E0) }
5C20581E 7E006026 7505D800 	{ p0 = cmp.eq(r5,000000C0); if (p0.new) r6 = 00000001; if (!p0.new) jump:t	00008E78 }
58004044 7607C3E5 	{ r5 = and(r7,0000001F); jump	00008ED0 }
7607D808     	{ r8 = and(r7,000000C0) }
5C20484C 74067FE6 7508D000 	{ p0 = cmp.eq(r8,00000080); if (p0.new) r6 = add(r6,FFFFFFFF); if (!p0.new) jump:nt	00008EEC }
DD064000 7607C7E7 	{ r7 = and(r7,0000003F); p0 = cmpb.eq(r6,00) }
8E45C6C7     	{ r7 |= asl(r5,00000006) }
5C204032 7067C005 	{ r5 = r7; if (!p0) jump:nt	00008ED0 }
5800C056     	{ jump	00008F20 }
7607DE05     	{ r5 = and(r7,000000F0) }
5C20580A 7E006046 7505DC00 	{ p0 = cmp.eq(r5,000000E0); if (p0.new) r6 = 00000002; if (!p0.new) jump:t	00008E90 }
58004024 7607C1E5 	{ r5 = and(r7,0000000F); jump	00008ED0 }
7607DF05     	{ r5 = and(r7,000000F8) }
5C20580A 7E006066 7505DE00 	{ p0 = cmp.eq(r5,000000F0); if (p0.new) r6 = 00000003; if (!p0.new) jump:t	00008EA8 }
58004018 7607C0E5 	{ r5 = and(r7,00000007); jump	00008ED0 }
7607DF85     	{ r5 = and(r7,000000FC) }
5C20580A 7E0060A6 7505DF80 	{ p0 = cmp.eq(r5,000000FC); if (p0.new) r6 = 00000005; if (!p0.new) jump:t	00008EC0 }
5800400C 7607C065 	{ r5 = and(r7,00000003); jump	00008ED0 }
5C204816 7E006086 7505DF00 	{ p0 = cmp.eq(r5,000000F8); if (p0.new) r6 = 00000004; if (!p0.new) jump:nt	00008EEC }
7607C065     	{ r5 = and(r7,00000003) }
B0044024 BFE27FE2 2472E0AA 78DF7FC2 	{ if (!cmp.eq(r2.new,00000001)) jump:t	00008E2C; r2 = add(r2,FFFFFFFF); r4 = add(r4,00000001) }
77600035     	{ memw(r3) = r5; r0 = and(r6,000000FF) }
70202330     	{ memuh(r3+8) = r0; r0 = r2 }
961EC01E     	{ dealloc_return }
5BFFF012     	{ call	00006F10 }
78DF7FE2 3C40C058 	{ memw(r0) = 00000058; r2 = FFFFFFFF }
50203F40     	{ dealloc_return; r0 = r2 }
F0311030     	{ memw(r3) = 00000000; memw(r3+4) = 00000000 }
9184C000     	{ r0 = memw(r4) }
9160C000     	{ r0 = memuh(r0) }
0000403C 7600C002 50203F40 	{ dealloc_return; r0 = r2; r2 = and(r0,00000F00) }
78004002 F0301031 	{ memw(r3+4) = 00000000; memw(r3) = 00000000; r2 = 00000000 }
50203F40     	{ dealloc_return; r0 = r2 }
75004000 78004002 4680C700 	{ if (p0.new) memw(r0) = r7; r2 = 00000000; p0 = cmp.eq(r0,00000000) }
103740F8 FB216482 3C23C180 	{ memuh(r3+6) = FF80; if (!p0.new) r2 = sub(r4,r1); if (p0.new) jump:nt	00008F1C; p0 = cmp.eq(r23,00000001) }
50203F40     	{ dealloc_return; r0 = r2 }

;; _Mbtowc: 00008F3C
_Mbtowc proc
EBF41C20     	{ allocframe(00000010); memd(r29+496) = r17:r16 }
5BFF6B62 F5004310 F5014212 A1DDD200 	{ memd(r29) = r19:r18; r19:r18 = combine(r1,r2); r17:r16 = combine(r0,r3); call	00006604 }
F5105202 F5135100 50043E0C 	{ r17:r16 = memd(r29+8); r4 = r0; r1:r0 = combine(r19,r17); r3:r2 = combine(r16,r18) }
59FF7EEA 3E051F00 	{ deallocframe; r19:r18 = memd(r29); jump	00008D30 }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Tls_get__Wcstate: 00008F70
_Tls_get__Wcstate proc
78004021 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r1 = 00000001 }
00004401 78004290 00004401 7800C311 A1DDD200 	{ memd(r29) = r19:r18; r17 = 00010058; r16 = 00010054 }
9210C000     	{ r0 = memw_locked(r16) }
7540C000     	{ p0 = cmp.gt(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	00008FA0 }
A0B0C100     	{ memw_locked(r16,p0) = r1 }
5CFFE0F8     	{ if (!p0) jump:nt	00008F8C }
10406010 7E00E052 	{ if (p0.new) r18 = 00000002; if (!p0.new) jump:t	00008FC0; p0 = cmp.eq(r0,00000000) }
00004401 78004300 000041BF 7800C601 5A00D0CC 	{ call	0000B150; r1 = 00006FF0; r0 = 00010058 }
A190D200     	{ memw(r16) = r18 }
91904000 24C2E100 5A00514C 	{ if (!cmp.gt(r0.new,00000002)) jump:t	00008FC4; r0 = memw(r16) }
9191C000     	{ r0 = memw(r17) }
7C006800 70604010 2442E020 5BFFEF32 	{ if (!cmp.eq(r16.new,00000000)) jump:t	00009018; r16 = r0; r1:r0 = combine(00000040,00000001) }
70604010 2402C018 5A00511E 	{ if (cmp.eq(r16.new,00000000)) jump:nt	00009014; r16 = r0 }
50810090     	{ r0 = memw(r17); r1 = r16 }
1000C008     	{ if (p0.new) jump:nt	00009000; p0 = cmp.eq(r0,00000000) }
5BFF6FFE 28083080 	{ r0 = r16; r16 = 00000000; call	00006FF0 }
5800C00A     	{ jump	00009010 }
5A0049C0 0000435D 73306100 	{ call	0000A380 }
7800C802     	{ r2 = 00000040 }
70704000 3E0C1E05 	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r16 }
961EC01E     	{ dealloc_return }

;; _Getpwcstate: 0000901C
_Getpwcstate proc
59FFFFAA     	{ jump	00008F70 }

;; _Atrealloc: 00009020
_Atrealloc proc
EBF41C10     	{ allocframe(00000008); memd(r29+496) = r17:r16 }
4980C031     	{ r17 = memw(gp+4) }
7071C000     	{ r0 = r17 }
8E00C1A0     	{ r0 += lsr(r0,00000001) }
8C004240 5BFFF0F8 	{ call	00007224; r0 = asl(r0,00000002) }
78004001 70604010 2402C03E 8C114131 	{ if (cmp.eq(r16.new,00000000)) jump:nt	000090BC; r16 = r0; r1 = 00000000 }
70704000 49804202 4980C041 	{ r1 = memw(gp+8); r2 = memw(gp+64); r0 = r16 }
8C024242 5BFFF1C6 	{ call	000073E4; r2 = asl(r2,00000002) }
49804060 4980C023 	{ r3 = memw(gp+4); r0 = memw(gp+12) }
F3005102 F3204303 4980C041 	{ r1 = memw(gp+8); r3 = sub(r3,r0); r2 = add(r0,r17) }
C4004141 C402D040 	{ r0 = addasl(r16,r2,00000002); r1 = addasl(r1,r0,00000002) }
8C034242 5BFFF1B4 	{ call	000073E4; r2 = asl(r3,00000002) }
4980C040     	{ r0 = memw(gp+8) }
000043A5 78004001 2003C008 	{ r1 = 0000E940 }
5BFFEFB0     	{ call	00006FF0 }
78004021 49804060 4980C022 	{ r2 = memw(gp+4); r0 = memw(gp+12); r1 = 00000001 }
F3005103 F3025100 4880D002 	{ memw(gp+512) = r16; r0 = add(r2,r17); r3 = add(r0,r17) }
48804303 4880C001 	{ memw(gp) = r0; memw(gp+96) = r3 }
70614000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = r1 }
7F00C000     	{ nop }

;; _Closreg: 000090C0
_Closreg proc
78004021 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r1 = 00000001 }
00004401 7800C390 9210C000 	{ r0 = memw_locked(r16); r16 = 0001005C }
7540C000     	{ p0 = cmp.gt(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	000090E4 }
A0B0C100     	{ memw_locked(r16,p0) = r1 }
5CFFE0F8     	{ if (!p0) jump:nt	000090D0 }
10406012 7E00E040 	{ if (p0.new) r0 = 00000002; if (!p0.new) jump:t	00009108; p0 = cmp.eq(r0,00000000) }
5BFFECAC     	{ call	00006A44 }
5A004250 00004244 7800C280 5BFF6CFE 28202829 	{ r17 = 00000002; r0 = 00000002; call	00006AF8; r0 = 00009114; call	00009590 }
A190D100     	{ memw(r16) = r17 }
91904000 24C2E100 3E041F40 	{ if (!cmp.gt(r0.new,00000002)) jump:t	0000910C; r0 = memw(r16) }

;; closeall: 00009114
closeall proc
00004394 7C9C4290 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17:r16 = combine(0000E514,00000038) }
91904000 2402E006 5A00C00C 	{ if (cmp.eq(r0.new,00000000)) jump:t	00009130; r0 = memw(r16) }
B0104090 BFF17FF1 2473E0FA 	{ r17 = add(r17,FFFFFFFF); r16 = add(r16,00000004) }
3E041F40     	{ dealloc_return; r17:r16 = memd(r29) }
7F00C000     	{ nop }

;; fclose: 00009140
fclose proc
70604010 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r16 = r0 }
D0800A05     	{ memd(r29) = r19:r18; r0 = memb(r16) }
76004060 2402C042 91904020 	{ if (cmp.eq(r0.new,00000000)) jump:nt	000091D4; r0 = and(r0,00000003) }
26C2C03E 5A004052 	{ if (!tstbit(r0.new,-00000001)) jump:nt	000091D4 }
7070C000     	{ r0 = r16 }
5BFF6C70 28203009 	{ r17 = r0; r0 = 00000002; call	00006A44 }
9130C000     	{ r0 = memb(r16) }
76004800 2402C008 5BFF6F3C 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00009184; r0 = and(r0,00000040) }
9190C040     	{ r0 = memw(r16+8) }
5A0041B0 C1801082 	{ memw(r16+8) = 00000000; r0 = memw(r16+4); call	000094E0 }
75004000 7E8F7FF1 91904201 2403C01A 	{ r1 = memw(r16+64); if (!p0.new) r17 = FFFFFFFF; p0 = cmp.eq(r0,00000000) }
5A0041BC 7061C000 	{ r0 = r1; call	00009510 }
70604012 9190C201 	{ r1 = memw(r16+64); r18 = r0 }
5BFF6F24 7061C000 	{ r0 = r1; call	00006FF0 }
75124000 7E8F7FF1 3C50C800 	{ memw(r16+64) = 00000000; if (!p0.new) r17 = FFFFFFFF; p0 = cmp.eq(r18,00000000) }
5A0042B2 7070C000 	{ r0 = r16; call	00009720 }
5BFF6C9A 7800C040 	{ r0 = 00000002; call	00006AF8 }
5800C014     	{ jump	000091F4 }
5BFF6C3A 7800C040 	{ r0 = 00000002; call	00006A44 }
5A0042A4 7070C000 	{ r0 = r16; call	00009720 }
5BFF6C8C 7800C040 	{ r0 = 00000002; call	00006AF8 }
5BFF6E94 78DFFFF1 	{ r17 = FFFFFFFF; call	00006F10 }
3C40C009     	{ memw(r0) = 00000009 }
70714000 3E0C1E05 	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r17 }
961EC01E     	{ dealloc_return }

;; fflush: 00009200
fflush proc
70604010 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r16 = r0 }
10084032 74906000 A1DDD200 	{ memd(r29) = r19:r18; if (!p0.new) r0 = add(r16,00000000); if (p0.new) jump:nt	0000926C; p0 = cmp.eq(r8,00000000) }
5BFFEBA2     	{ call	00006958 }
9170C000     	{ r0 = memuh(r16) }
00004080 76004001 2403C048 	{ r1 = and(r0,00002000) }
9190C051     	{ r17 = memw(r16+8) }
91904081 2143F114 	{ r1 = memw(r16+16) }
5A00415E F3314102 50910180 	{ r0 = memw(r16+4); r1 = r17; r2 = sub(r1,r17); call	000094F0 }
10C04042 FB11E011 	{ if (p0.new) r17 = add(r17,r0); if (!p0.new) jump:nt	000092C4; p0 = cmp.gt(r0,00000000) }
91904081 2133F1F6 	{ r1 = memw(r16+16) }
02892080     	{ r0 = memh(r16); r17 = memw(r16+8) }
0000437F 760047E1 70800489 	{ memw(r16+16) = r17; r0 = r16; r1 = and(r0,0000DFFF) }
AD890689     	{ memw(r16+24) = r17; memw(r16+52) = r17 }
58004026 A150C100 	{ memuh(r16) = r1; jump	000092B0 }
5BFF6BEC 78004040 00004394 7C9CC010 7800C292 	{ r18 = 00000014; r17:r16 = combine(0000E500,00000038); r0 = 00000002; call	00006A44 }
91904000 2402E00A 5BFFFFBC 	{ if (cmp.eq(r0.new,00000000)) jump:t	00009298; r0 = memw(r16) }
75607FE0 7E8FFFF1 	{ if (!p0.new) r17 = FFFFFFFF; p0 = cmp.gt(r0,FFFFFFFF) }
78004040 B0104090 BFF27FF2 2472E0F6 5BFFEC2A 	{ if (!cmp.eq(r18.new,00000001)) jump:t	0000928C; r18 = add(r18,FFFFFFFF); r16 = add(r16,00000004); r0 = 00000002 }
5800C008     	{ jump	000092B8 }
7070C000     	{ r0 = r16 }
7800C011     	{ r17 = 00000000 }
5BFFEBAE     	{ call	00006A10 }
70714000 3E0C1E05 	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r17 }
961EC01E     	{ dealloc_return }
70704000 78DF7FF1 02812082 	{ r2 = memh(r16); r1 = memw(r16+8); r17 = FFFFFFFF; r0 = r16 }
8CC24902 A4810D81 	{ memw(r16+52) = r1; memw(r16+16) = r1; r2 = setbit(r2,00000012) }
59FF7FEE A6812082 	{ memuh(r16+8) = r2; memw(r16+24) = r1; jump	000092B4 }

;; fputc: 000092E0
fputc proc
F5014010 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
5BFF6B38 7071C000 	{ r0 = r17; call	00006958 }
9191C080     	{ r0 = memw(r17+16) }
919140C1 2103E00C 	{ r1 = memw(r17+24) }
5BFF7362 7071C000 	{ r0 = r17; call	000079C0 }
11C04122 4391C080 	{ if (p0.new) r0 = memw(r17+16); if (!p0.new) jump:t	00009348; p0 = tstbit(r0,00000000) }
DD104140 B0004021 A1B1D304 	{ r1 = add(r0,00000001); p0 = cmpb.eq(r16,0A) }
A100D000     	{ memb(r0) = r16 }
9171C000     	{ r0 = memuh(r17) }
00004020 76004001 2443E00E 	{ r1 = and(r0,00000800) }
00004010 76004000 2402C012 5C20C00C 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00009358; r0 = and(r0,00000400) }
5BFF7F62 7071C000 	{ r0 = r17; call	00009200 }
1000C006     	{ if (p0.new) jump:nt	00009350; p0 = cmp.eq(r0,00000000) }
17094006 78DFFFF0 	{ r16 = FFFFFFFF; jump	00009354; r0 = r9 }
30903788     	{ r16 = and(r16,000000FF); r0 = r17 }
5BFFEB5E     	{ call	00006A10 }
70704000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = r16 }

;; fputs: 00009360
fputs proc
F5004110 EBF41C40 	{ allocframe(00000020); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r1) }
5BFF6AF8 70802A15 	{ memd(r29+16) = r19:r18; r0 = r16; call	00006958 }
5800C004     	{ jump	00009378 }
F311D211     	{ r17 = add(r17,r18) }
91314000 2402C05C 9190C080 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00009434; r0 = memb(r17) }
70704000 919040C1 2103E00A 	{ r1 = memw(r16+24); r0 = r16 }
5BFFF318     	{ call	000079C0 }
11C0C15A     	{ if (!p0.new) jump:t	00009448; p0 = tstbit(r0,00000000) }
9130C020     	{ r0 = memb(r16+1) }
76004080 2402E00A 5A0040E6 	{ if (cmp.eq(r0.new,00000000)) jump:t	000093B4; r0 = and(r0,00000004) }
7331E140     	{  }
1040E010     	{ if (!p0.new) jump:t	000093CC; p0 = cmp.eq(r0,00000000) }
5BFF7198 7071C000 	{ r0 = r17; call	000076E0 }
6BE04000 7060C003 	{ r3 = r0; p0 = or(p0,!p0) }
89404001 5800400E A1BDD502 	{ jump	000093E0; r1 = p0 }
6B604000 DB80C331 	{ r3 = add(r0,sub(00000041,r17)); p0 = and(p0,p0) }
89404000 A1BDD202 70714001 	{ memb(r29+2) = r0.new; r0 = p0 }
04800682     	{ r2 = memw(r16+24); r0 = memw(r16+16) }
F320C204     	{ r4 = sub(r2,r0) }
D5A34492 F263C400 	{ p0 = cmp.gtu(r3,r4); r18 = minu(r3,r4) }
89404003 5BFF6FF8 70724002 A1BDD701 	{ r2 = r18; call	000073E4; r3 = p0 }
04803C11     	{ r1 = memd(r29+4); r0 = memw(r16+16) }
85414000 5CDF78B8 F3005200 A1B0D204 919DC040 	{ memb(r16+4) = r0.new; r0 = add(r0,r18); if (p0.new) jump:t	00009378; p0 = r1 }
85404000 5CDF78AE 7490E000 	{ if (!p0.new) r0 = add(r16,00000000); if (p0.new) jump:t	00009378; p0 = r0 }
5BFFFEEE     	{ call	00009200 }
1030E0A6     	{ if (p0.new) jump:t	00009374; p0 = cmp.eq(r16,00000001) }
5800C00E     	{ jump	00009448 }
9130C020     	{ r0 = memb(r16+1) }
76004100 2402C00E 5BFF7EE2 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00009454; r0 = and(r0,00000008) }
7070C000     	{ r0 = r16 }
1000C006     	{ if (p0.new) jump:nt	00009450; p0 = cmp.eq(r0,00000000) }
17084006 78DFFFF0 	{ r16 = FFFFFFFF; jump	00009454; r0 = r8 }
28083080     	{ r0 = r16; r16 = 00000000 }
5BFFEADE     	{ call	00006A10 }
70704000 3E1C1E15 	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = r16 }
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; ldexpl: 00009470
ldexpl proc
70624010 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r16 = r2 }
1008401A 749D6000 A1DDC000 	{ memd(r29) = r1:r0; if (!p0.new) r0 = add(r29,00000000); if (p0.new) jump:nt	000094AC; p0 = cmp.eq(r8,00000000) }
5A00C376     	{ call	00009B70 }
1180E112     	{ if (p0.new) jump:t	000094AC; p0 = tstbit(r0,00000000) }
5A00436A 2C003081 	{ r1 = r16; r0 = add(r29,00000000); call	00009B60 }
10406106 5800400A 7E00E080 	{ if (p0.new) r0 = 00000004; jump	000094AC; if (!p0.new) jump:t	000094A0; p0 = cmp.eq(r0,00000002) }
10406006 7E00E100 	{ if (p0.new) r0 = 00000008; if (!p0.new) jump:t	000094AC; p0 = cmp.eq(r0,00000000) }
5A00C0E4     	{ call	00009670 }
3E001E0C     	{ r17:r16 = memd(r29+8); r1:r0 = memd(r29) }
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; ldiv: 000094C0
ldiv proc
5A0047D0 F5014010 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0); call	0000A460 }
ED00D102     	{ r2 = mpyi(r0,r17) }
F3225001 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r1 = sub(r16,r2) }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; close: 000094E0
close proc
5800CD70     	{ jump	0000AFC0 }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; write: 000094F0
write proc
5A004ED8 70624010 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r2; call	0000B2A0 }
F3205000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = sub(r16,r0) }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; remove: 00009510
remove proc
5800CD88     	{ jump	0000B020 }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; sqrtl: 00009520
sqrtl proc
F5014010 EBF41C30 	{ allocframe(00000018); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
5BFF75EC B01D40C0 6C212A0C 	{ memd(r29+8) = r17:r16; r1 = add(r29,00000008); r0 = add(r29,00000006); call	00008100 }
1000C012     	{ if (p0.new) jump:nt	00009558; p0 = cmp.eq(r0,00000000) }
1000E104     	{ if (p0.new) jump:t	00009540; p0 = cmp.eq(r0,00000002) }
1000E20E     	{ if (p0.new) jump:t	00009558; p0 = cmp.eq(r0,00000004) }
B01DC100     	{ r0 = add(r29,00000008) }
7680C0C0     	{ r0 = or(r0,00000006) }
91404000 2682E008 5A004090 	{ if (tstbit(r0.new,-00000001)) jump:t	0000955C; r0 = memh(r0) }
7800C020     	{ r0 = 00000001 }
5A004C34 F511D000 	{ r1:r0 = combine(r17,r16); call	0000ADC0 }
3E141F40     	{ dealloc_return; r17:r16 = memd(r29+16) }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; strrchr: 00009570
strrchr proc
28023711     	{ r1 = and(r1,000000FF); r2 = 00000000 }
7060C003     	{ r3 = r0 }
9B23C024     	{ r4 = memb(r3++#1) }
F2044101 74206002 30303940 	{ p0 = cmp.eq(r4,00000000); r0 = r3; if (p1.new) r2 = add(r0,00000000); p1 = cmp.eq(r4,r1) }
5CFFE0F6     	{ if (!p0) jump:nt	00009574 }
50203FC0     	{ jumpr	r31; r0 = r2 }

;; _Atexit: 00009590
_Atexit proc
70604010 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r16 = r0 }
4980C202     	{ r2 = memw(gp+64) }
49804061 2103E20A 	{ r1 = memw(gp+12) }
5BFFFD3E     	{ call	00009020 }
1000C00E     	{ if (p0.new) jump:nt	000095C4; p0 = cmp.eq(r0,00000000) }
4980C061     	{ r1 = memw(gp+12) }
BFE17FE0 4980C041 	{ r1 = memw(gp+8); r0 = add(r1,FFFFFFFF) }
48804003 3B81E010 	{ memw(r30+r0<<#2) = r16; memw(gp) = r0 }
3E041F40     	{ dealloc_return; r17:r16 = memd(r29) }
5BFFEC36     	{ call	00006E30 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Dunscale: 000095D0
_Dunscale proc
F5004110 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r1) }
50802382     	{ r2 = memh(r16+6); r0 = r16 }
8D024B81 2443E010 	{ r1 = extractu(r2,0000000B,00000003) }
5A00C41E     	{ call	00009E20 }
78004000 70604001 24C3C02C 	{ r1 = r0; r0 = 00000000 }
58004038 3C31C000 	{ memuh(r17) = 0000; jump	00009664 }
70C1C000     	{ r0 = zxth(r1) }
78004040 78037FE3 2043E022 	{ r3 = 000007FF; r0 = 00000002 }
3C31C000     	{ memuh(r17) = 0000 }
9130C0C1     	{ r1 = memb(r16+6) }
760141E1 2443E028 	{ r1 = and(r1,0000000F) }
91704041 2443E024 	{ r1 = memuh(r16+4) }
91704021 2443E020 	{ r1 = memuh(r16+2) }
9170C000     	{ r0 = memuh(r16) }
5800401A 75004010 7E006040 7E80E020 	{ if (!p0.new) r0 = 00000001; if (p0.new) r0 = 00000002; p0 = !cmp.eq(r0,00000000); jump	00009664 }
9150C062     	{ r2 = memh(r16+6) }
781F7C03 BFC14041 000043FF 7800C7E0 00004200 DA4241E3 A1B0CA03 A151C100 	{ memb(r16+3) = r2.new; r2 = or(r3,and(r2,0000800F)); r0 = 0000FFFF; r1 = add(r1,FFFFFC02); r3 = 00003FE0 }
70E04000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = sxth(r0) }
7F00C000     	{ nop }

;; _Feraise: 00009670
_Feraise proc
8CC04401 76004182 A09DC001 	{ allocframe(+00000008); r2 = and(r0,0000000C); r1 = setbit(r0,00000008) }
74006010 74816010 79202A04 	{ memd(r29) = r17:r16; p0 = cmp.eq(r2,00000000); if (!p0.new) r16 = add(r1,00000000); if (p0.new) r16 = add(r0,00000000) }
5A00428C 70704000 7610C071 	{ r17 = and(r16,00000003); r0 = r16; call	00009BA0 }
1009C00A     	{ if (p0.new) jump:nt	000096A8; p0 = cmp.eq(r9,00000000) }
5BFFEC3C     	{ call	00006F10 }
91DD4010 3C40C021 	{ memw(r0) = 00000021; r17:r16 = memd(r29) }
961EC01E     	{ dealloc_return }
76104180 2402C008 5BFFEC30 	{ if (cmp.eq(r0.new,00000000)) jump:nt	000096BC; r0 = and(r16,0000000C) }
3C40C022     	{ memw(r0) = 00000022 }
3E041F40     	{ dealloc_return; r17:r16 = memd(r29) }
7F00C000     	{ nop }

;; _Fofind: 000096C0
_Fofind proc
00004394 7C004710 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17:r16 = combine(0000E538,00000000) }
1108D314     	{ if (p0.new) jump:t	000096F4; p0 = cmp.gtu(r8,-00000001) }
91914000 2402C016 B0114091 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00009700; r0 = memw(r17) }
B0104030 91604001 2473E0FA 	{ r1 = memuh(r0); r16 = add(r16,00000001) }
5800401A 000043FD 3C20C03F 78004000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = 00000000; memuh(r0) = FF7F; jump	0000971C }
5BFF6D92 7800CA00 	{ r0 = 00000050; call	00007220 }
1000C00C     	{ if (p0.new) jump:nt	0000971C; p0 = cmp.eq(r0,00000000) }
A0901208     	{ memb(r0+2) = r16; memw(r17) = r0 }
91DD4010 00004002 3C20C000 961EC01E 	{ dealloc_return; memuh(r0) = 0080; r17:r16 = memd(r29) }
3E041F40     	{ dealloc_return; r17:r16 = memd(r29) }

;; _Fofree: 00009720
_Fofree proc
B0004982 A09DC000 	{ allocframe(+00000000); r2 = add(r0,0000004C) }
9120C001     	{ r1 = memb(r0) }
76015001 2403C01A 	{ r1 = and(r1,00000080) }
7F004000 00004394 2B423A01 B0014021 2503D30C 	{ r1 = add(r1,00000001); r1 = -00000001; r2 = 00000034; nop }
40420124     	{ r4 = memw(r2+4); r2 = add(r2,00000004) }
147460FA 7402E003 	{ if (p0.new) r3 = add(r2,00000000); if (!p0.new) jump:t	00009740; p0 = cmp.eq(r4,-00000001) }
3C43C000     	{ memw(r3) = 00000000 }
5BFFEC4C     	{ call	00006FF0 }
961EC01E     	{ dealloc_return }
B0004581 3C4060FF 3C20C000 	{ memuh(r0) = 0000; memw(r0+4) = FFFFFFFF; r1 = add(r0,0000002C) }
AD020602     	{ memw(r0+24) = r2; memw(r0+52) = r2 }
AC020502     	{ memw(r0+20) = r2; memw(r0+48) = r2 }
A4020202     	{ memw(r0+8) = r2; memw(r0+16) = r2 }
A7020801     	{ memw(r0+32) = r1; memw(r0+28) = r2 }
961EC01E     	{ dealloc_return }

;; _Genld: 00009780
_Genld proc
5A00457C A09DC006 	{ allocframe(+00000030); call	0000A278 }
5A0042B2 70614014 F5044312 F502C010 	{ r17:r16 = combine(r2,r0); r19:r18 = combine(r4,r3); r20 = r1; call	00009CEC }
8CD44501 0000435E 7C006102 9180C160 	{ r0 = memw(r0+44); r3:r2 = combine(0000D788,00000001); r1 = setbit(r20,0000000A) }
D5C25212 75524000 75014CC1 7483E011 	{ if (!p0.new) r17 = add(r3,00000000); p1 = cmp.eq(r1,00000066); p0 = cmp.gt(r18,00000000); r18 = max(r2,r18) }
5C004114 9100C015 	{ r21 = memb(r0); if (p1) jump:nt	000097E0 }
75144CE0 7514C8E2 	{ p2 = cmp.eq(r20,00000047); p0 = cmp.eq(r20,00000067) }
6B224003 5C20DB1A 	{ if (!p3.new) jump:t	00009800; p3 = or(p0,p2) }
5C205B16 7573FF63 	{ p3 = cmp.gt(r19,FFFFFFFB); if (!p3.new) jump:t	000097FC }
91904180 20C2F312 5C004144 	{ if (!cmp.gt(r19,r0.new)) jump:t	00009800; r0 = memw(r16+48) }
B0134034 4530CF80 	{ if (!p1) r0 = memb(r16-4); r20 = add(r19,00000001) }
76004100 2402C02C 58004030 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00009848; r0 = and(r0,00000008) }
9190C180     	{ r0 = memw(r16+48) }
5C00420A 7EC0CE16 	{ if (!p2) r22 = 00000070; if (p2) jump:nt	00009810 }
5C0059E8 5C2040E2 7514CC21 	{ p1 = cmp.eq(r20,00000061); if (!p0) jump:nt	000099CC; if (p1.new) jump:t	000099D4 }
91904180 20C2F20E 9130C781 	{ if (!cmp.gt(r18,r0.new)) jump:t	00009830; r0 = memw(r16+48) }
7601C101     	{ r1 = and(r1,00000008) }
75014001 74326000 4290D261 	{ if (p1.new) memw(r16+48) = r18; if (p1.new) r0 = add(r18,00000000); p1 = cmp.eq(r1,00000000) }
7E004CB6 7E8048B6 27F02801 	{ r1 = 00000000; r0 = add(r0,FFFFFFFF); if (!p0) r22 = 00000045; if (p0) r22 = 00000065 }
D5C14000 580040CE A1B0D40C 9190C180 	{ memb(r16+12) = r0.new; jump	000099D8; r0 = max(r1,r0) }
F2405200 74126000 4290D260 	{ if (p0.new) memw(r16+48) = r18; if (p0.new) r0 = add(r18,00000000); p0 = cmp.gt(r0,r18) }
70F4C001     	{ r1 = sxth(r20) }
F321C000     	{ r0 = sub(r0,r1) }
75607FE0 A190400C 39D0C600 	{ if (!p0.new) memw(r16+48) = 00000000; memw(r16+48) = r0; p0 = cmp.gt(r0,FFFFFFFF) }
70F44013 24C3C032 	{ r19 = sxth(r20) }
F2535200 07800481 	{ r1 = memw(r16+16); r0 = memw(r16+28); p0 = cmp.gt(r19,r18) }
5C204070 F301C000 	{ r0 = add(r1,r0); if (!p0) jump:nt	00009958 }
5BFF6DB0 309130A2 	{ r2 = r18; r1 = r17; call	000073E0 }
F3325302 07810C80 	{ r0 = memw(r16+16); r1 = memw(r16+28); r2 = sub(r19,r18) }
75404000 78A10882 	{ memw(r16+32) = r2; r1 = add(r1,r18); p0 = cmp.gt(r0,00000000) }
5C00400A 45304782 A190C107 	{ memw(r16+28) = r1; if (!p0) r2 = memb(r16-4); if (p0) jump:nt	000098AC }
76024102 2402C010 9190C080 	{ if (cmp.eq(r2.new,00000000)) jump:nt	000098C8; r2 = and(r2,00000008) }
F300C100     	{ r0 = add(r0,r1) }
A100D500     	{ memb(r0) = r21 }
09810C80     	{ r0 = memw(r16+16); r1 = memw(r16+4) }
B0014021 A1B0D309 	{ r1 = add(r1,00000001) }
5800412A A190C00A 	{ memw(r16+40) = r0; jump	00009B18 }
07800482     	{ r2 = memw(r16+16); r0 = memw(r16+28) }
B0004021 F3024000 A1B0D507 	{ r0 = add(r2,r0); r1 = add(r0,00000001) }
3C00C030     	{ memb(r0) = 30 }
91904182 2482E00A 9130C780 	{ if (cmp.gt(r2.new,00000000)) jump:t	000098F8; r2 = memw(r16+48) }
76004100 2402C010 07800482 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00009910; r0 = and(r0,00000008) }
B0004021 F3024000 A1B0D507 	{ r0 = add(r2,r0); r1 = add(r0,00000001) }
A100D500     	{ memb(r0) = r21 }
9190C182     	{ r2 = memw(r16+48) }
76534000 76424003 07810484 	{ r4 = memw(r16+16); r1 = memw(r16+28); r3 = sub(00000000,r2); r0 = sub(00000000,r19) }
F2404200 F3044100 70714001 7494E003 	{ if (!p0.new) r3 = add(r20,00000000); r1 = r17; r0 = add(r4,r1); p0 = cmp.gt(r0,r2) }
70E3C003     	{ r3 = sxth(r3) }
F3034204 7643C003 	{ r3 = sub(00000000,r3); r4 = add(r3,r2) }
D5B24402 AC840883 	{ memw(r16+32) = r3; memw(r16+48) = r4; r2 = min(r18,r4) }
70E2C011     	{ r17 = sxth(r2) }
5BFF6D50 70920989 	{ memw(r16+36) = r17; r2 = r17; call	000073E0 }
9190C180     	{ r0 = memw(r16+48) }
580040E6 F3314000 A1B0D20A 5BFF6D44 	{ memb(r16+10) = r0.new; r0 = sub(r0,r17); jump	00009B18 }
309130B2     	{ r2 = r19; r1 = r17 }
F3345202 07800C83 	{ r3 = memw(r16+16); r0 = memw(r16+28); r2 = sub(r18,r20) }
1083600C F3005300 A1B0D207 9130C781 	{ memb(r16+7) = r0.new; r0 = add(r0,r19); if (p0.new) jump:t	00009980; p0 = cmp.gt(r3,00000000) }
76014101 2403C00C 	{ r1 = and(r1,00000008) }
51030481     	{ r1 = memw(r16+16); r3 = r0 }
78100783     	{ memw(r16+28) = r3; r0 = add(r0,r1) }
A100D500     	{ memb(r0) = r21 }
0C830780     	{ r0 = memw(r16+28); r3 = memw(r16+16) }
F3115301 54250484 	{ r4 = memw(r16+16); r5 = sxth(r2); r1 = add(r17,r19) }
F2454300 F3044000 7403E002 	{ if (p0.new) r2 = add(r3,00000000); r0 = add(r4,r0); p0 = cmp.gt(r5,r3) }
70E2C011     	{ r17 = sxth(r2) }
5BFF6D1C 7071C002 	{ r2 = r17; call	000073E0 }
07800C81     	{ r1 = memw(r16+16); r0 = memw(r16+28) }
F3005100 F3314101 A1B0D407 580040AC 	{ memb(r16+7) = r0.new; r1 = sub(r1,r17); r0 = add(r0,r17) }
A190C108     	{ memw(r16+32) = r1 }
75144820 7074C016 	{ r22 = r20; p0 = cmp.eq(r20,00000041) }
7E00CA16     	{ if (p0) r22 = 00000050 }
07800481     	{ r1 = memw(r16+16); r0 = memw(r16+28) }
B0004023 F3014000 9B114022 A1B0D707 	{ r2 = memb(r17++#1); r0 = add(r1,r0); r3 = add(r0,00000001) }
A100C200     	{ memb(r0) = r2 }
91904180 2482E00A 9130C780 	{ if (cmp.gt(r0.new,00000000)) jump:t	00009A04; r0 = memw(r16+48) }
76004100 2402C032 07800482 	{ if (cmp.eq(r0.new,00000000)) jump:nt	00009A60; r0 = and(r0,00000008) }
B0004021 F3024000 A1B0D507 	{ r0 = add(r2,r0); r1 = add(r0,00000001) }
A100D500     	{ memb(r0) = r21 }
91904182 24C2C024 78DF7FE0 	{ if (!cmp.gt(r2.new,00000000)) jump:nt	00009A60; r2 = memw(r16+48) }
07810485     	{ r5 = memw(r16+16); r1 = memw(r16+28) }
D5005204 F3054100 7071C001 	{ r1 = r17; r0 = add(r5,r1); r4 = add(r18.l,r0.l) }
F2444200 7492FFE2 	{ if (!p0.new) r2 = add(r18,FFFFFFFF); p0 = cmp.gt(r4,r2) }
70E2C011     	{ r17 = sxth(r2) }
5BFF6CD2 7071C002 	{ r2 = r17; call	000073E0 }
0C800781     	{ r1 = memw(r16+28); r0 = memw(r16+16) }
F3015114 F3314000 A1B0D407 58004006 	{ memb(r16+7) = r20.new; r0 = sub(r0,r17); r20 = add(r1,r17) }
A190C008     	{ memw(r16+32) = r0 }
9190C0F4     	{ r20 = memw(r16+28) }
75737FE0 50CD0489 	{ r17 = memw(r16+16); r21 = r20; p0 = cmp.gt(r19,FFFFFFFF) }
E2114055 F311D400 	{ r0 = add(r17,r20); r21 += add(r17,00000002) }
5C204008 AB00D608 	{ memb(r0++#1) = r22; if (!p0) jump:nt	00009A80 }
58004008 3C00C02B 	{ memb(r0) = 2B; jump	00009A88 }
76534013 3C00C02D 	{ memb(r0) = 2D; r19 = sub(00000000,r19) }
10CB4016 741D6017 7800C012 	{ r18 = 00000000; if (p0.new) r23 = add(r29,00000000); if (!p0.new) jump:nt	00009AB4; p0 = cmp.gt(r11,00000000) }
5A004076 73336140 	{ call	00009B80 }
B012C032     	{ r18 = add(r18,00000001) }
71FF10F1     	{ memb(r23) = r1; r23 = r23 }
70E04013 24B3E0F8 	{ r19 = sxth(r0) }
108A6112 7415E011 	{ if (p0.new) r17 = add(r21,00000000); if (p0.new) jump:t	00009AD0; p0 = cmp.gt(r10,00000002) }
8CD6C500     	{ r0 = setbit(r22,0000000A) }
5C20480E 74956011 7500CCA0 	{ p0 = cmp.eq(r0,00000065); if (!p0.new) r17 = add(r21,00000000); if (!p0.new) jump:nt	00009AD4 }
E2144071 58004008 3C15C030 	{ memb(r21) = 30; jump	00009AD8; r17 += add(r20,00000003) }
5800C006     	{ jump	00009ADC }
100AC018     	{ if (p0.new) jump:nt	00009B04; p0 = cmp.eq(r10,00000000) }
10CAC01A     	{ if (!p0.new) jump:nt	00009B0C; p0 = cmp.gt(r10,00000000) }
60124018 70714001 2C0030A2 	{ r2 = r18; r0 = add(r29,00000000); r1 = r17; loop0(00009AE8,r18) }
F3004203 BFE2FFE2 	{ r2 = add(r2,FFFFFFFF); r3 = add(r0,r2) }
9703FFE3     	{ r3 = memb(r3-1) }
B0038603 ABA1C308 	{  }
58004008 F311D211 	{ r17 = add(r17,r18); jump	00009B0C }
78004600 ABB1C208 	{  }
07800481     	{ r1 = memw(r16+16); r0 = memw(r16+28) }
EF814031 A1B0D309 	{ r17 -= add(r1,r0) }
9170C3C0     	{ r0 = memuh(r16+60) }
76004280 2442F014 05810782 	{ if (!cmp.eq(r0.new,00000000)) jump:t	00009B48; r0 = and(r0,00000014) }
08830A80     	{ r0 = memw(r16+8); r3 = memw(r16) }
EF024123 09850E84 	{ r4 = memw(r16+24); r5 = memw(r16+4); r3 += add(r2,r1) }
EF03C520     	{ r0 += add(r3,r5) }
F2444000 FB206400 42B0D230 5800C3FE 	{ if (p0.new) memw(r16+24) = r0.new; if (p0.new) r0 = sub(r4,r0); p0 = cmp.gt(r4,r0) }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _LDint: 00009B50
_LDint proc
5A0040D0 A09DC000 	{ allocframe(+00000000); call	00009CF0 }
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }

;; _LDscale: 00009B60
_LDscale proc
5A004208 A09DC000 	{ allocframe(+00000000); call	00009F70 }
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }

;; _LDtest: 00009B70
_LDtest proc
5A004338 A09DC000 	{ allocframe(+00000000); call	0000A1E0 }
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }

;; div: 00009B80
div proc
5A004470 F5014010 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0); call	0000A460 }
ED00D102     	{ r2 = mpyi(r0,r17) }
F3225001 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r1 = sub(r16,r2) }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; feraiseexcept: 00009BA0
feraiseexcept proc
EBF41C20     	{ allocframe(00000010); memd(r29+496) = r17:r16 }
B01D4080 760043F0 2402C01A 5A00C358 	{ if (cmp.eq(r16.new,00000000)) jump:nt	00009BE0; r16 = and(r0,0000001F); r0 = add(r29,00000004) }
4C103C11     	{ r1 = memd(r29+4); r0 = add(r29,00000004) }
8E5041C1 5A00403A A1BDD501 	{ call	00009C30; r1 |= asl(r16,00000001) }
919DC020     	{ r0 = memw(r29+4) }
8E405930 2402C008 5A004008 	{ r16 &= lsr(r0,00000019) }
7070C000     	{ r0 = r16 }
78004000 3E0C1F40 	{ dealloc_return; r17:r16 = memd(r29+8); r0 = 00000000 }

;; _Force_raise: 00009BE0
_Force_raise proc
49C04302 A09DC002 	{ allocframe(+00000010); r3:r2 = memd(gp+192) }
732060B0     	{  }
EA0C0A05     	{ memd(r29) = r19:r18; memd(r29+8) = r17:r16 }
0000435E 7800C312 9792FFC0 	{ r0 = memw(r18-8); r18 = 0000D798 }
F1005000 2402E00C 5A00463E 	{ if (cmp.eq(r0.new,00000000)) jump:t	00009C18; r0 = and(r0,r16) }
91D24000 91D2C022 	{ r3:r2 = memd(r18+8); r1:r0 = memd(r18) }
F501C002     	{ r3:r2 = combine(r1,r0) }
F5034200 B0124312 BFF17FF1 2473E0F2 	{ r17 = add(r17,FFFFFFFF); r18 = add(r18,00000018); r1:r0 = combine(r3,r2) }
3E0C1E05     	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
961EC01E     	{ dealloc_return }

;; fesetenv: 00009C2C
fesetenv proc
9180C001     	{ r1 = memw(r0) }
6221C008     	{ USR = r1 }
48003FC0     	{ jumpr	r31; r0 = 00000000 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _Tls_get__Locale: 00009C40
_Tls_get__Locale proc
78004021 EBF41C20 	{ allocframe(00000010); memd(r29+496) = r17:r16; r1 = 00000001 }
00004401 78004410 00004401 7800C491 A1DDD200 	{ memd(r29) = r19:r18; r17 = 00010064; r16 = 00010060 }
9210C000     	{ r0 = memw_locked(r16) }
7540C000     	{ p0 = cmp.gt(r0,00000000) }
5C00C006     	{ if (p0) jump:nt	00009C70 }
A0B0C100     	{ memw_locked(r16,p0) = r1 }
5CFFE0F8     	{ if (!p0) jump:nt	00009C5C }
10406010 7E00E052 	{ if (p0.new) r18 = 00000002; if (!p0.new) jump:t	00009C90; p0 = cmp.eq(r0,00000000) }
00004401 78004480 000041BF 7800C601 5A00CA64 	{ call	0000B150; r1 = 00006FF0; r0 = 00010064 }
A190D200     	{ memw(r16) = r18 }
91904000 24C2E100 5A004AE4 	{ if (!cmp.gt(r0.new,00000002)) jump:t	00009C94; r0 = memw(r16) }
9191C000     	{ r0 = memw(r17) }
7C006A00 70604010 2442E020 5BFFE8CA 	{ if (!cmp.eq(r16.new,00000000)) jump:t	00009CE8; r16 = r0; r1:r0 = combine(00000050,00000001) }
70604010 2402C018 5A004AB6 	{ if (cmp.eq(r16.new,00000000)) jump:nt	00009CE4; r16 = r0 }
50810090     	{ r0 = memw(r17); r1 = r16 }
1000C008     	{ if (p0.new) jump:nt	00009CD0; p0 = cmp.eq(r0,00000000) }
5BFF6996 28083080 	{ r0 = r16; r16 = 00000000; call	00006FF0 }
5800C00A     	{ jump	00009CE0 }
5A004358 00004360 73306100 	{ call	0000A380 }
7800CA02     	{ r2 = 00000050 }
70704000 3E0C1E05 	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r0 = r16 }
961EC01E     	{ dealloc_return }

;; localeconv: 00009CEC
localeconv proc
59FFFFAA     	{ jump	00009C40 }

;; _Dint: 00009CF0
_Dint proc
78037FE3 9160C062 	{ r2 = memuh(r0+6); r3 = 000007FF }
8D024B84 78004043 2044E31C 760241E1 	{ if (!cmp.eq(r4.new,r3)) jump:t	00009D38; r3 = 00000002; r4 = extractu(r2,0000000B,00000003) }
2443E08A     	{  }
91604041 2443E086 	{ r1 = memuh(r0+4) }
91604021 2443E082 	{ r1 = memuh(r0+2) }
9160C000     	{ r0 = memuh(r0) }
75004000 7E006023 7E80E043 	{ if (!p0.new) r3 = 00000002; if (p0.new) r3 = 00000001; p0 = cmp.eq(r0,00000000) }
54303FC0     	{ jumpr	r31; r0 = sxth(r3) }
000041FF 760247E3 2443E014 	{ r3 = and(r2,00007FFF) }
91604043 2443E00E 	{ r3 = memuh(r0+4) }
91604023 2443E00A 	{ r3 = memuh(r0+2) }
78004003 91604005 2403C066 	{ r5 = memuh(r0); r3 = 00000000 }
00004010 76444665 7800C003 	{ r3 = 00000000; r5 = sub(00000433,r4) }
D5214504 F3214501 24C4C05A 70E1C001 	{ if (!cmp.gt(r4.new,00000000)) jump:nt	00009E20; r1 = sub(r5,r1); r4 = sub(r5.l,r1.l) }
5C205818 000043FF 7E0067E3 7541C680 	{ p0 = cmp.gt(r1,00000034); if (p0.new) r3 = 0000FFFF; if (!p0.new) jump:t	00009DA4 }
00004200 76024001 3C204000 3C20C100 	{ memuh(r0+4) = 0000; memuh(r0) = 0000; r1 = and(r2,00008000) }
70E34000 529F4000 3C204080 A140C103 	{ memuh(r0+6) = r1; memuh(r0+2) = FF80; jumpr	r31; r0 = sxth(r3) }
8C044402 7604C1E1 	{ r1 = and(r4,0000000F); r2 = asr(r4,00000004) }
8D045081 00004361 9D61DA85 75014020 00004362 9D82F203 3A60C381 	{ r1 = memuh(r12+r3<<#1); r3 = memw(r2<<#2+0000D888); p0 = cmp.eq(r1,00000001); r5 = memuh(r1<<#1+0000D868); r1 = extractu(r4,00000010,00000000) }
70614004 F101C501 	{ r1 = and(r1,r5); r4 = r1 }
EFC44544 5C004016 3BA0C38C 1002E20A 	{ if (p0) jump:nt	00009E00; XOREQ	r4,and(r4,r5) }
1042E314     	{ if (!p0.new) jump:t	00009E08; p0 = cmp.eq(r2,00000006) }
91404042 3C20C100 	{ memuh(r0+4) = 0000; r2 = memh(r0+4) }
F122C101     	{ r1 = or(r2,r1) }
91404022 3C20C080 	{ memuh(r0+2) = FF80; r2 = memh(r0+2) }
F122C101     	{ r1 = or(r2,r1) }
91404002 3C20C000 	{ memuh(r0) = 0000; r2 = memh(r0) }
F122C101     	{ r1 = or(r2,r1) }
70C1C000     	{ r0 = zxth(r1) }
75004010 7E0F7FE3 7E80E003 	{ if (!p0.new) r3 = 00000000; if (p0.new) r3 = FFFFFFFF; p0 = !cmp.eq(r0,00000000) }
54303FC0     	{ jumpr	r31; r0 = sxth(r3) }
7F00C000     	{ nop }

;; _Dnorm: 00009E20
_Dnorm proc
9160C061     	{ r1 = memuh(r0+6) }
760141E3 00004200 7601C001 10436014 A140C303 	{ memuh(r0+6) = r3; if (!p0.new) jump:t	00009E58; p0 = cmp.eq(r3,00000000); r1 = and(r1,00008000); r3 = and(r1,0000000F) }
78004003 91604042 2442E010 91604022 	{ if (!cmp.eq(r2.new,00000000)) jump:t	00009E60; r2 = memuh(r0+4); r3 = 00000000 }
2442E00A 7C006002 	{ if (!cmp.eq(r2.new,00000000)) jump:t	00009E5C }
91604004 2402C088 16024110 	{ if (cmp.eq(r4.new,00000001)) jump:nt	00009F64; r4 = memuh(r0) }
7F00C000     	{ nop }
BFE27E02 22030105 	{ r5 = memh(r0+2); r3 = memh(r0+4); r2 = add(r2,FFFFFFF0) }
91404004 3C20C000 	{ memuh(r0) = 0000; r4 = memh(r0) }
E3030205     	{ memuh(r0) = r5; memuh(r0+8) = r3 }
A140C401     	{ memuh(r0+2) = r4 }
DD034008 5CDFF8F4 	{ if (p0.new) jump:t	00009E64; p0 = cmph.eq(r3,0000) }
DD4341E8 5C00583A 7403E004 	{ if (p0.new) r4 = add(r3,00000000); if (p0.new) jump:t	00009EF8; p0 = cmph.gtu(r3,000F) }
78004207 91404005 9140C028 	{ r8 = memh(r0+2); r5 = memh(r0); r7 = 00000010 }
91404049 4980C426 	{ r6 = memw(gp+132); r9 = memh(r0+4) }
F105460D F1094604 F108460C BFE2FFE2 	{ r2 = add(r2,FFFFFFFF); r12 = and(r8,r6); r4 = and(r9,r6); r13 = and(r5,r6) }
8C044F24 8C0CCF2C 	{ r12 = lsr(r12,0000000F); r4 = lsr(r4,0000000F) }
8C0D4F2D 8E43C1C4 	{ r4 |= asl(r3,00000001); r13 = lsr(r13,0000000F) }
8E4941CC 8E4841CD 70C4400E 7064C003 	{ r3 = r4; r14 = zxth(r4); r13 |= asl(r8,00000001); r12 |= asl(r9,00000001) }
8C054145 5CDF78E8 F50C4D08 F267CE00 	{ p0 = cmp.gtu(r7,r14); r9:r8 = combine(r12,r13); if (p0.new) jump:t	00009EA4; r5 = asl(r5,00000001) }
A1404D01 A140C403 	{ memuh(r0+6) = r4; memuh(r0+2) = r13 }
58004006 A1404C02 A140C500 	{ memuh(r0) = r5; memuh(r0+4) = r12; jump	00009EF4 }
70C4C003     	{ r3 = zxth(r4) }
78004405 2103E32E 	{ r5 = 00000020 }
91404003 9140C02D 	{ r13 = memh(r0+2); r3 = memh(r0) }
9140404C 4980C445 	{ r5 = memw(gp+136); r12 = memh(r0+4) }
8C044F47 F10C4509 F10D4508 B002C022 	{ r2 = add(r2,00000001); r8 = and(r13,r5); r9 = and(r12,r5); r7 = asl(r4,0000000F) }
8C0C4F46 8C0D4F43 F103450C 70C4C00D 	{ r13 = zxth(r4); r12 = and(r3,r5); r3 = asl(r13,0000000F); r6 = asl(r12,0000000F) }
8E4941A7 8E48C1A6 	{ r6 |= lsr(r8,00000001); r7 |= lsr(r9,00000001) }
DD4447E8 8E4CC1A3 	{ r3 |= lsr(r12,00000001); p0 = cmph.gtu(r4,003F) }
8C0D4124 5CDF60E8 F506C70C 	{ r13:r12 = combine(r6,r7); if (p0) jump:nt	00009F14; r4 = lsr(r13,00000001) }
E2070106     	{ memuh(r0) = r6; memuh(r0+8) = r7 }
E0030304     	{ memuh(r0) = r4; memuh(r0+8) = r3 }
760441E3 A1A0CB03 	{ r3 = and(r4,0000000F) }
F1234103 70E24001 A1A0CD03 	{ r1 = sxth(r2); r3 = or(r3,r1) }
50103FC0     	{ jumpr	r31; r0 = r1 }
7F00C000     	{ nop }

;; _Dscale: 00009F70
_Dscale proc
F5014010 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
9150C062     	{ r2 = memh(r16+6) }
8D024B80 2442E00E 5BFF7F4E 	{ if (!cmp.eq(r0.new,00000000)) jump:t	00009F9C; r0 = extractu(r2,0000000B,00000003) }
7070C000     	{ r0 = r16 }
10906026 7800C001 	{ r1 = 00000000; if (p0.new) jump:t	0000A1D8; p0 = cmp.gt(r16,00000000) }
5800C020     	{ jump	00009FD4 }
70C0C001     	{ r1 = zxth(r0) }
78004041 78037FE3 2043E11C 	{ r3 = 000007FF; r1 = 00000002 }
760241E0 2452E018 91704040 	{ if (!cmp.eq(r0.new,00000000)) jump:t	0000A1DC; r0 = and(r2,0000000F) }
2452E014 91704020 	{ if (!cmp.eq(r0.new,00000000)) jump:t	0000A1DC }
2452E010 9170C000 	{ if (!cmp.eq(r0.new,00000000)) jump:t	0000A1DC }
5800410A 75004010 7E006041 7E80E021 	{ if (!p0.new) r1 = 00000001; if (p0.new) r1 = 00000002; p0 = !cmp.eq(r0,00000000); jump	0000A1D8 }
10C9400A 70E0C002 	{ r2 = sxth(r0); if (!p0.new) jump:nt	00009FE8; p0 = cmp.gt(r9,00000000) }
0000401F 764247E0 20C2F138 78DF7963 	{ if (!cmp.gt(r17,r0.new)) jump:t	0000A054; r0 = sub(000007FF,r2) }
76424001 91704060 21C5F118 	{ r0 = memuh(r16+6); r1 = sub(00000000,r2) }
00004200 760041E0 F302D102 	{ r2 = add(r2,r17); r0 = and(r0,0000800F) }
8E4244C0 000043FF 780047E1 A1B0CC03 5800C0E2 E2115FE2 	{ r1 = 0000FFFF; r0 |= asl(r2,00000004) }
760041E1 00004200 7600C000 8CC14401 F2634200 39304000 3930C100 	{ if (p0.new) memuh(r16+4) = 0000; if (p0.new) memuh(r16) = 0000; p0 = cmp.gtu(r3,r2); r1 = setbit(r1,00000008); r0 = and(r0,00008000); r1 = and(r0,0000000F) }
5C20401A 7A612381 	{ memuh(r16+8) = r1; r1 = -00000001; if (!p0) jump:nt	0000A06C }
580040CC 3C304080 A150C003 	{ memuh(r16+6) = r0; memuh(r16+2) = FF80; jump	0000A1D8 }
0000439A 7800C200 91C0C000 	{ r1:r0 = memd(r0); r0 = 0000E690 }
91504062 2682E006 8CC1DF41 	{ if (tstbit(r2.new,-00000001)) jump:t	0000A068; r2 = memh(r16+6) }
160141BA A1D0C000 	{ memd(r16) = r1:r0; jump	0000A1D8; r1 = 00000001 }
70024004 28033422 	{ r2 = sxth(r2); r3 = 00000000; r4 = aslh(r2) }
5C00583E 7E006004 0FFF43FF 7544C7E0 78004004 9150404C 9150C02D 	{ r13 = memh(r16+2); r12 = memh(r16+4); r4 = 00000000; p0 = cmp.gt(r4,FFF0FFFF); if (p0.new) r4 = 00000000; if (p0.new) jump:t	0000A0F0 }
7F004000 7F004000 0FFF4400 48062085 00014000 7800400E 70C44004 7061C009 	{ r9 = r1; r4 = zxth(r4); r14 = 00100000; r5 = memh(r16); r6 = 00000000; nop; nop }
8E0250CE 706D4007 73646004 F50CC90C 	{ r13:r12 = combine(r12,r9); r4 = !cmp.eq(r4,00000000); r7 = r13; r14 += asl(r2,00000010) }
F1254404 702E4002 28013075 	{ r5 = r7; r1 = 00000000; r2 = asrh(r14); r4 = or(r5,r4) }
5CDF78EA F246CE00 	{ p0 = cmp.gt(r6,r14); if (p0.new) jump:t	0000A0A0 }
706D4008 78004001 3C304180 A150C902 	{ memuh(r16+4) = r9; memuh(r16+6) = FF80; r1 = 00000000; r8 = r13 }
58004006 A1504801 A150C700 	{ memuh(r16) = r7; memuh(r16+2) = r8; jump	0000A0F0 }
7642C005     	{ r5 = sub(00000000,r2) }
DD054008 5C004828 70C1EC08 	{ if (!p0.new) r8 = zxth(r1); if (p0.new) jump:nt	0000A148; p0 = cmph.eq(r5,0000) }
D5224302 56442983 	{ r3 = memuh(r16+2); r4 = zxth(r4); r2 = sub(r3.l,r2.l) }
C6484241 76424207 2A860885 	{ r5 = memuh(r16); r6 = memuh(r16+4); r7 = sub(00000010,r2); r1 &= lsr(r8,r2) }
C6484788 C646478C 73646004 A150C103 	{ memuh(r16+6) = r1; r4 = !cmp.eq(r4,00000000); r12 &= asl(r6,r7); r8 &= asl(r8,r7) }
C6434789 CC05C784 	{ r4 |= asl(r5,r7); r9 &= asl(r3,r7) }
CC064248 CC03C24C 	{ r12 |= lsr(r3,r2); r8 |= lsr(r6,r2) }
CC054249 A1504802 A150CC01 	{ memuh(r16+2) = r12; memuh(r16+4) = r8; r9 |= lsr(r5,r2) }
A150C900     	{ memuh(r16) = r9 }
F1214001 00004200 28133642 15436210 47504002 A150C103 	{ memuh(r16+6) = r1; if (!p0.new) r2 = memh(r16); if (!p0.new) jump:t	0000A170; p0 = cmp.gtu(r3,r2); r2 = zxth(r4); r3 = 00000001; r1 = or(r1,r0) }
00004200 78004003 2043E220 	{ r3 = 00008000 }
9150C002     	{ r2 = memh(r16) }
11C2C318     	{ if (!p0.new) jump:t	0000A19C; p0 = tstbit(r2,00000000) }
B002C022     	{ r2 = add(r2,00000001) }
76232082     	{ memuh(r16+8) = r2; r3 = zxth(r2) }
10436012 4350C022 	{ if (p0.new) r2 = memh(r16+2); if (!p0.new) jump:t	0000A19C; p0 = cmp.eq(r3,00000000) }
B002C022     	{ r2 = add(r2,00000001) }
76232182     	{ memuh(r16+8) = r2; r3 = zxth(r2) }
1043600A 4350C042 	{ if (p0.new) r2 = memh(r16+4); if (!p0.new) jump:t	0000A19C; p0 = cmp.eq(r3,00000000) }
B002C022     	{ r2 = add(r2,00000001) }
76232282     	{ memuh(r16+8) = r2; r3 = zxth(r2) }
1003C018     	{ if (p0.new) jump:nt	0000A1C8; p0 = cmp.eq(r3,00000000) }
70C14001 2043E01A 	{ r1 = zxth(r1) }
91704040 2442E016 91704020 	{ if (!cmp.eq(r0.new,00000000)) jump:t	0000A1D4; r0 = memuh(r16+4) }
2442E012 48012880 	{ if (!cmp.eq(r0.new,00000000)) jump:t	0000A1D4 }
58004010 75004000 000043FF 7E80E7E1 B0014020 A1B0CA03 000043FF 	{ memb(r16+3) = r0.new; r0 = add(r1,00000001); if (!p0.new) r1 = 0000FFFF; p0 = cmp.eq(r0,00000000); jump	0000A1D8 }
7800C7E1     	{ r1 = 0000003F }
70E14000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = sxth(r1) }

;; _Dtest: 0000A1E0
_Dtest proc
785F7E01 9160C062 	{ r2 = memuh(r0+6); r1 = 00007FF0 }
78004041 F1024103 2043E11C 	{ r3 = and(r2,r1); r1 = 00000002 }
760241E2 2442E034 91604042 	{ if (!cmp.eq(r2.new,00000000)) jump:t	0000A260; r2 = and(r2,0000000F) }
2442E030 91604022 	{ if (!cmp.eq(r2.new,00000000)) jump:t	0000A260 }
2442E02C 9160C000 	{ if (!cmp.eq(r2.new,00000000)) jump:t	0000A260 }
75004010 7E006041 7E80E021 	{ if (!p0.new) r1 = 00000001; if (p0.new) r1 = 00000002; p0 = !cmp.eq(r0,00000000) }
50103FC0     	{ jumpr	r31; r0 = r1 }
000041FF 760247E1 2443E014 	{ r1 = and(r2,00007FFF) }
91604041 2443E00E 	{ r1 = memuh(r0+4) }
91604021 2443E00A 	{ r1 = memuh(r0+2) }
78004001 91604000 2402C010 000041FF 	{ if (cmp.eq(r0.new,00000000)) jump:nt	0000A264; r0 = memuh(r0); r1 = 00000000 }
7602C600     	{ r0 = and(r2,00000030) }
75004000 7E0F7FC1 7E8FFFE1 	{ if (!p0.new) r1 = FFFFFFFF; if (p0.new) r1 = FFFFFFFE; p0 = cmp.eq(r0,00000000) }
50103FC0     	{ jumpr	r31; r0 = r1 }

;; fegetenv: 0000A260
fegetenv proc
6A08C001     	{ r1 = USR }
A180C100     	{ memw(r0) = r1 }
48003FC0     	{ jumpr	r31; r0 = 00000000 }
7F00C000     	{ nop }

;; __save_r16_through_r27: 0000A270
__save_r16_through_r27 proc
A7DE7AFA A7DEF8FB 	{ memd(r30-40) = r25:r24; memd(r30-48) = r27:r26 }

;; __save_r16_through_r23: 0000A278
__save_r16_through_r23 proc
A7DE76FC A7DEF4FD 	{ memd(r30-24) = r21:r20; memd(r30-32) = r23:r22 }

;; __save_r16_through_r19: 0000A280
__save_r16_through_r19 proc
7F004000 529F4000 A7DE72FE A7DEF0FF 	{ memd(r30-8) = r17:r16; memd(r30-16) = r19:r18; jumpr	r31; nop }

;; __save_r16_through_r27_stkchk: 0000A290
__save_r16_through_r27_stkchk proc
A7DE7AFA A7DEF8FB 	{ memd(r30-40) = r25:r24; memd(r30-48) = r27:r26 }

;; __save_r16_through_r23_stkchk: 0000A298
__save_r16_through_r23_stkchk proc
A7DE76FC A7DEF4FD 	{ memd(r30-24) = r21:r20; memd(r30-32) = r23:r22 }

;; __save_r16_through_r19_stkchk: 0000A2A0
__save_r16_through_r19_stkchk proc
6A0A4011 A7DE72FE A7DEF0FF 	{ memd(r30-8) = r17:r16; memd(r30-16) = r19:r18; r17 = UGP }
9191C230     	{ r16 = memw(r17+68) }
F2705D00 537FD800 	{ if (!p0.new) jumpr:t	r31; p0 = cmp.gtu(r16,r29) }
5BFF65BC A09DC000 	{ allocframe(+00000000); call	00006E30 }

;; __save_r16_through_r25: 0000A2C0
__save_r16_through_r25 proc
A7DE78FB A7DEF6FC 	{ memd(r30-32) = r23:r22; memd(r30-40) = r25:r24 }

;; __save_r16_through_r21: 0000A2C8
__save_r16_through_r21 proc
A7DE74FD A7DEF2FE 	{ memd(r30-16) = r19:r18; memd(r30-24) = r21:r20 }

;; __save_r16_through_r17: 0000A2D0
__save_r16_through_r17 proc
529F4000 A7DEF0FF 	{ memd(r30-8) = r17:r16; jumpr	r31 }

;; __save_r16_through_r25_stkchk: 0000A2D8
__save_r16_through_r25_stkchk proc
A7DE78FB A7DEF6FC 	{ memd(r30-32) = r23:r22; memd(r30-40) = r25:r24 }

;; __save_r16_through_r21_stkchk: 0000A2E0
__save_r16_through_r21_stkchk proc
A7DE74FD A7DEF2FE 	{ memd(r30-16) = r19:r18; memd(r30-24) = r21:r20 }

;; __save_r16_through_r17_stkchk: 0000A2E8
__save_r16_through_r17_stkchk proc
6A0A4011 A7DEF0FF 	{ memd(r30-8) = r17:r16; r17 = UGP }
9191C230     	{ r16 = memw(r17+68) }
F2705D00 537FD800 	{ if (!p0.new) jumpr:t	r31; p0 = cmp.gtu(r16,r29) }
5BFF659A A09DC000 	{ allocframe(+00000000); call	00006E30 }

;; __restore_r16_through_r23_and_deallocframe_before_tailcall: 0000A304
__restore_r16_through_r23_and_deallocframe_before_tailcall proc
7F004000 97DE7F96 97DEFFB4 	{ r21:r20 = memd(r30-24); r23:r22 = memd(r30-32); nop }

;; __restore_r16_through_r19_and_deallocframe_before_tailcall: 0000A310
__restore_r16_through_r19_and_deallocframe_before_tailcall proc
58004010 97DEFFD2 	{ r19:r18 = memd(r30-16); jump	0000A330 }

;; __restore_r16_through_r27_and_deallocframe_before_tailcall: 0000A318
__restore_r16_through_r27_and_deallocframe_before_tailcall proc
7F004000 97DEFF5A 	{ r27:r26 = memd(r30-48); nop }

;; __restore_r16_through_r25_and_deallocframe_before_tailcall: 0000A320
__restore_r16_through_r25_and_deallocframe_before_tailcall proc
97DE7F78 97DEFF96 	{ r23:r22 = memd(r30-32); r25:r24 = memd(r30-40) }

;; __restore_r16_through_r21_and_deallocframe_before_tailcall: 0000A328
__restore_r16_through_r21_and_deallocframe_before_tailcall proc
97DE7FB4 97DEFFD2 	{ r19:r18 = memd(r30-16); r21:r20 = memd(r30-24) }

;; __restore_r16_through_r17_and_deallocframe_before_tailcall: 0000A330
__restore_r16_through_r17_and_deallocframe_before_tailcall proc
7F004000 529F4000 97DE7FF0 901EC01E 	{ deallocframe; r17:r16 = memd(r30-8); jumpr	r31; nop }

;; __restore_r16_through_r23_and_deallocframe: 0000A340
__restore_r16_through_r23_and_deallocframe proc
97DE7F96 97DEFFB4 	{ r21:r20 = memd(r30-24); r23:r22 = memd(r30-32) }

;; __restore_r16_through_r19_and_deallocframe: 0000A348
__restore_r16_through_r19_and_deallocframe proc
58004010 97DEFFD2 	{ r19:r18 = memd(r30-16); jump	0000A368 }

;; __restore_r16_through_r27_and_deallocframe: 0000A350
__restore_r16_through_r27_and_deallocframe proc
97DEFF5A     	{ r27:r26 = memd(r30-48) }

;; __restore_r16_through_r25_and_deallocframe: 0000A354
__restore_r16_through_r25_and_deallocframe proc
7F004000 97DE7F78 97DEFF96 	{ r23:r22 = memd(r30-32); r25:r24 = memd(r30-40); nop }

;; __restore_r16_through_r21_and_deallocframe: 0000A360
__restore_r16_through_r21_and_deallocframe proc
97DE7FB4 97DEFFD2 	{ r19:r18 = memd(r30-16); r21:r20 = memd(r30-24) }

;; __restore_r16_through_r17_and_deallocframe: 0000A368
__restore_r16_through_r17_and_deallocframe proc
97DE7FF0 961EC01E 	{ dealloc_return; r17:r16 = memd(r30-8) }

;; __deallocframe: 0000A370
__deallocframe proc
961EC01E     	{ dealloc_return }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __hexagon_memcpy_likely_aligned_min32bytes_mult8bytes: 0000A380
__hexagon_memcpy_likely_aligned_min32bytes_mult8bytes proc
85814700 85804700 78DF7FA3 43C1C004 	{ if (p0.new) r5:r4 = memd(r1); r3 = FFFFFFFD; p0 = bitsclr(r0,00000007); p0 = bitsclr(r1,00000007) }
5CF44028 8E0243A3 41C14024 ABC0E408 	{ if (p0) memd(r0++#8) = r5:r4; if (p0) r5:r4 = memd(r1+8); r3 += lsr(r2,00000003); if (!p0) jump:nt	000073E0 }
60034100 B0014301 91C14044 ABC0C408 	{ memd(r0++#8) = r5:r4; r5:r4 = memd(r1+16); r1 = add(r1,00000018); loop0(0000A3B0,r3) }
9BC18024 ABC0C408 	{ memd(r0++#8) = r5:r4; r5:r4 = memd(r1++#8) }
E2825F00 529F4000 A1C0C400 	{ memd(r0) = r5:r4; jumpr	r31; r0 -= add(r2,FFFFFFF8) }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __qdsp_divdi3: 0000A3E0
__qdsp_divdi3 proc
85015F02 8503DF03 	{ p3 = tstbit(r3,0000001F); p2 = tstbit(r1,0000001F) }
808040C0 8082C0C2 	{ r3:r2 = abs(r3:r2); r1:r0 = abs(r1:r0) }
88404046 88424047 F5034204 F501C002 	{ r3:r2 = combine(r1,r0); r5:r4 = combine(r3,r2); r7 = cl0(r3:r2); r6 = cl0(r1:r0) }
6B424303 F326470A 7C004000 7C00E00E 	{ r15:r14 = combine(00000000,00000001); r1:r0 = combine(00000000,00000000); r10 = sub(r7,r6); p3 = xor(p2,p3) }
C3844ACC C38E4ACE B00AC02B 	{ r11 = add(r10,00000001); r15:r14 = lsl(r15:r14,r10); r13:r12 = lsl(r5:r4,r10) }
600B4018 D284C280 	{ p0 = cmp.gtu(r5:r4,r3:r2); loop0(0000A428,r11) }
5C00C010     	{ if (p0) jump:nt	0000A444 }
D28CC280     	{ p0 = cmp.gtu(r13:r12,r3:r2) }
D32C42E6 D300CEE8 	{ r9:r8 = add(r15:r14,r1:r0); r7:r6 = sub(r3:r2,r13:r12) }
D1004800 D102C602 	{ r3:r2 = vmux(p0,r3:r2,r7:r6); r1:r0 = vmux(p0,r1:r0,r9:r8) }
800E812E 800CC12C 	{ r13:r12 = lsr(r13:r12,00000001); r15:r14 = lsr(r15:r14,00000001) }
8080C0A2     	{ r3:r2 = neg(r1:r0) }
D1024060 529FC000 	{ jumpr	r31; r1:r0 = vmux(p3,r3:r2,r1:r0) }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __hexagon_divsi3: 0000A460
__hexagon_divsi3 proc
8C804081 8C814082 75607FE0 7561FFE1 	{ p1 = cmp.gt(r1,FFFFFFFF); p0 = cmp.gt(r0,FFFFFFFF); r2 = abs(r1); r1 = abs(r0) }
8C0140A3 8C0240A4 F3224105 F262C102 	{ p2 = cmp.gtu(r2,r1); r5 = sub(r1,r2); r4 = cl0(r2); r3 = cl0(r1) }
6B404101 535F4200 78004000 F262C500 	{ p0 = cmp.gtu(r2,r5); r0 = 00000000; if (p2) jumpr:nt	r31; p1 = xor(p0,p1) }
7A807FE0 F3234404 48133FC4 	{ if (p0) jumpr	r31; r3 = 00000001; r4 = sub(r4,r3); r0 = mux(p1,FFFFFFFF,00000001) }
60044108 C30244C2 78004000 7F00C000 	{ nop; r0 = 00000000; r3:r2 = vlslw(r3:r2,r4); loop0(0000A4B0,r4) }
7F00C000     	{ nop }
F2628100 80424122 FB226181 FB00E380 	{ if (!p0.new) r0 = add(r0,r3); if (!p0.new) r1 = sub(r1,r2); r3:r2 = vlsrw(r3:r2,00000001); p0 = cmp.gtu(r2,r1) }
F2624100 537F4100 FB00E380 	{ if (!p0.new) r0 = add(r0,r3); if (!p1) jumpr:nt	r31; p0 = cmp.gtu(r2,r1) }
76404000 529FC000 	{ jumpr	r31; r0 = sub(00000000,r0) }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __hexagon_udivdi3: 0000A4E0
__hexagon_udivdi3 proc
88404046 88424047 F5034204 F501C002 	{ r3:r2 = combine(r1,r0); r5:r4 = combine(r3,r2); r7 = cl0(r3:r2); r6 = cl0(r1:r0) }
F326470A 7C004000 7C00E00E 	{ r15:r14 = combine(00000000,00000001); r1:r0 = combine(00000000,00000000); r10 = sub(r7,r6) }
C3844ACC C38E4ACE B00AC02B 	{ r11 = add(r10,00000001); r15:r14 = lsl(r15:r14,r10); r13:r12 = lsl(r5:r4,r10) }
600B4018 D284C280 	{ p0 = cmp.gtu(r5:r4,r3:r2); loop0(0000A514,r11) }
535FC000     	{ if (p0) jumpr:nt	r31 }
D28CC280     	{ p0 = cmp.gtu(r13:r12,r3:r2) }
D32C42E6 D300CEE8 	{ r9:r8 = add(r15:r14,r1:r0); r7:r6 = sub(r3:r2,r13:r12) }
D1004800 D102C602 	{ r3:r2 = vmux(p0,r3:r2,r7:r6); r1:r0 = vmux(p0,r1:r0,r9:r8) }
800E812E 800CC12C 	{ r13:r12 = lsr(r13:r12,00000001); r15:r14 = lsr(r15:r14,00000001) }
529FC000     	{ jumpr	r31 }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __qdsp_udivsi3: 0000A540
__qdsp_udivsi3 proc
8C0040A2 8C0140A3 7C004024 F261C000 	{ p0 = cmp.gtu(r1,r0); r5:r4 = combine(00000001,00000000); r3 = cl0(r1); r2 = cl0(r0) }
F3224306 F5004400 50143FC4 	{ if (p0) jumpr	r31; r4 = r1; r1:r0 = combine(r0,r4); r6 = sub(r3,r2) }
60064108 C30446C2 7F004000 7F00C000 	{ nop; nop; r3:r2 = vlslw(r5:r4,r6); loop0(0000A570,r6) }
7F00C000     	{ nop }
F2628100 80424122 FB226181 FB00E380 	{ if (!p0.new) r0 = add(r0,r3); if (!p0.new) r1 = sub(r1,r2); r3:r2 = vlsrw(r3:r2,00000001); p0 = cmp.gtu(r2,r1) }
F2624100 529F4000 FB00E380 	{ if (!p0.new) r0 = add(r0,r3); jumpr	r31; p0 = cmp.gtu(r2,r1) }
7F00C000     	{ nop }

;; __qdsp_umoddi3: 0000A590
__qdsp_umoddi3 proc
88404046 88424047 F5034204 F501C002 	{ r3:r2 = combine(r1,r0); r5:r4 = combine(r3,r2); r7 = cl0(r3:r2); r6 = cl0(r1:r0) }
F326470A 7C004000 7C00E00E 	{ r15:r14 = combine(00000000,00000001); r1:r0 = combine(00000000,00000000); r10 = sub(r7,r6) }
C3844ACC C38E4ACE B00AC02B 	{ r11 = add(r10,00000001); r15:r14 = lsl(r15:r14,r10); r13:r12 = lsl(r5:r4,r10) }
600B4018 D284C280 	{ p0 = cmp.gtu(r5:r4,r3:r2); loop0(0000A5C4,r11) }
5C00C010     	{ if (p0) jump:nt	0000A5E0 }
D28CC280     	{ p0 = cmp.gtu(r13:r12,r3:r2) }
D32C42E6 D300CEE8 	{ r9:r8 = add(r15:r14,r1:r0); r7:r6 = sub(r3:r2,r13:r12) }
D1004800 D102C602 	{ r3:r2 = vmux(p0,r3:r2,r7:r6); r1:r0 = vmux(p0,r1:r0,r9:r8) }
800E812E 800CC12C 	{ r13:r12 = lsr(r13:r12,00000001); r15:r14 = lsr(r15:r14,00000001) }
F5034200 529FC000 	{ jumpr	r31; r1:r0 = combine(r3,r2) }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __hexagon_adddf3: 0000A600
__hexagon_adddf3 proc
8D414B84 8D434B85 02004000 7C00C00C DC804053 DC824053 F50D4C08 F265C402 	{ p2 = cmp.gtu(r5,r4); r9:r8 = combine(r13,r12); p3 = dfclass(r3:r2,00000002); p3 = dfclass(r1:r0,00000002); r13:r12 = combine(20000000,00000000); r5 = extractu(r3,0000000B,00000013); r4 = extractu(r1,0000000B,00000013) }
5C2043A2 FD034240 FD014042 FD04C544 	{ if (p2) r5:r4 = combine(r4,r5); if (p2) r3:r2 = combine(r1,r0); if (p2) r1:r0 = combine(r3,r2); if (!p3) jump:nt	0000A764 }
8320742C 83227428 F325440F 7C00E7C6 	{ r7:r6 = combine(0000003E,00000001); r15 = sub(r4,r5); r9:r8 = insert(r2,00000034,0000000C); r13:r12 = insert(r0,00000034,0000000C) }
D5A74F0F 808C40AA 75617FE2 7800C00E 	{ r14 = 00000000; p2 = cmp.gt(r1,FFFFFFFF); r11:r10 = neg(r13:r12); r15 = min(r7,r15) }
C1084E0A C3884F08 FD0B4ACC 7C00C00E 	{ r15:r14 = combine(00000000,00000000); if (p2.new) r13:r12 = combine(r11,r10); r9:r8 = asr(r9:r8,r15); r11:r10 = extractu(r9:r8,r15:r14) }
D28A4E01 F92866A8 BFA47885 7563FFE3 	{ p3 = cmp.gt(r3,FFFFFFFF); r5 = add(r4,FFFFFBC4); if (!p1.new) r8 = or(r8,r6); p1 = cmp.eq(r11:r10,r15:r14) }
D30C48EC D3284CEA 0000401F 7C9EE6C6 F2644610 15444728 FD0BCAEC 	{ if (p3.new) r13:r12 = combine(r11,r10); if (!p0.new) jump:nt	0000A6D4; p0 = cmp.gtu(r4,r7); p0 = !cmp.gtu(r4,r6); r7:r6 = combine(000007F6,0000003D); r11:r10 = sub(r13:r12,r9:r8); r13:r12 = add(r9:r8,r13:r12) }
80EC4060 5C004812 750D4000 750CC000 	{ p0 = cmp.eq(r12,00000000); p0 = cmp.eq(r13,00000000); if (p0.new) jump:nt	0000A6B4; r1:r0 = convert_d2df(r13:r12) }
8E0554C1 529FC000 	{ jumpr	r31; r1 += asl(r5,00000014) }

;; __hexagon_fast2_subdf3: 0000A6A4
__hexagon_fast2_subdf3 proc
8CC35F43 59FF7FAE 7F00C000 	{ nop; jump	0000A604; r3 = togglebit(r3,0000001E) }
6A08401C 28133C00 	{ r1:r0 = combine(00000000,00000000); r3 = 00000001; r28 = USR }
8D5C42DC 8C03DF43 	{ r3 = asl(r3,0000001F); r28 = extractu(r28,00000002,00000012) }
751C4040 529F4000 F9616301 7F00C000 	{ nop; if (p0.new) r1 = xor(r1,r3); jumpr	r31; p0 = cmp.eq(r28,00000002) }
80EC4060 5CDF68F0 750D4000 750CC000 	{ p0 = cmp.eq(r12,00000000); p0 = cmp.eq(r13,00000000); if (p0.new) jump:nt	0000A6B4; r1:r0 = convert_d2df(r13:r12) }
8D414B9C 8E05D4C1 	{ r1 += asl(r5,00000014); r28 = extractu(r1,0000000B,00000013) }
F3055C05 00014000 7C00C002 0000401F 754547C0 5C00C816 	{ if (p0.new) jump:nt	0000A728; p0 = cmp.gt(r5,000007FE); r3:r2 = combine(00100000,00000000); r5 = add(r5,r28) }
75454000 535F5800 7645C03C 	{ r28 = sub(00000001,r5); if (p0.new) jumpr:t	r31; p0 = cmp.gt(r5,00000000) }
83007402 F50DCC00 	{ r1:r0 = combine(r13,r12); r3:r2 = insert(r0,00000034,00000004) }
C382DC42     	{ r3:r2 = lsr(r3:r2,r28) }
83027F00 529FC000 	{ jumpr	r31; r1:r0 = insert(r2,0000003F,00000007) }
6A08401C F50D4C00 07FE7FFF 7C7FE7EC 8D5C42C5 769C451C 07FF4000 7C00C008 623C4008 8E815F25 7065C01C 	{ r28 = r5; XOREQ	r5,lsr(r1,0000001F); USR = r28; r9:r8 = combine(7FF00000,00000000); r28 = or(r28,00000028); r5 = extractu(r28,00000002,00000012); r13:r12 = combine(7FEFFFFF,FFFFFFFF); r1:r0 = combine(r13,r12); r28 = USR }
751C4030 75054050 FD09E80C 	{ if (!p0) r13:r12 = combine(r9,r8); p0 = !cmp.eq(r5,00000002); p0 = !cmp.eq(r28,00000001) }
830CFF00     	{ r1:r0 = insert(r12,0000003F,00000007) }
D2E04000 529FC000 	{ jumpr	r31; p0 = dfcmp.eq(r1:r0,r1:r0) }
81007F0C 8102FF08 	{ r9:r8 = extractu(r3:r2,0000003F,00000007); r13:r12 = extractu(r1:r0,0000003F,00000007) }
D28C4883 FD0362E0 FD01E0E2 	{ if (!p3.new) r3:r2 = combine(r1,r0); if (!p3.new) r1:r0 = combine(r3,r2); p3 = cmp.gtu(r13:r12,r9:r8) }
DC8041F0 5C204850 FD0948EC FD0DCCE8 	{ if (p3.new) r9:r8 = combine(r13,r12); if (p3.new) r13:r12 = combine(r9,r8); if (!p0.new) jump:nt	0000A81C; p0 = dfclass(r1:r0,0000000F) }
DC804111 5C00C96A 	{ if (p1.new) jump:nt	0000A860; p1 = dfclass(r1:r0,00000008) }
DC824032 5C004A50 7C00C00C 	{ r13:r12 = combine(00000000,00000000); if (p2.new) jump:nt	0000A834; p2 = dfclass(r3:r2,00000001) }
DC804090 5C004816 02004000 7C00C00C 8D414B84 80084948 7800C025 	{ r5 = 00000001; r9:r8 = asl(r9:r8,00000009); r4 = extractu(r1,0000000B,00000013); r13:r12 = combine(20000000,00000000); if (p0.new) jump:nt	0000A7CC; p0 = dfclass(r1:r0,00000004) }
8320742C 59FF7F44 F325440F 7C00E7C6 	{ r7:r6 = combine(0000003E,00000001); r15 = sub(r4,r5); jump	0000A644; r13:r12 = insert(r0,00000034,0000000C) }
81007F0C 8102FF08 	{ r9:r8 = extractu(r3:r2,0000003F,00000007); r13:r12 = extractu(r1:r0,0000003F,00000007) }
808C40AC 808840A8 75617FE0 7563FFE1 	{ p1 = cmp.gt(r3,FFFFFFFF); p0 = cmp.gt(r1,FFFFFFFF); r9:r8 = neg(r9:r8); r13:r12 = neg(r13:r12) }
FD01400C FD03C228 	{ if (p1) r9:r8 = combine(r3,r2); if (p0) r13:r12 = combine(r1,r0) }
D30CC8EC     	{ r13:r12 = add(r9:r8,r13:r12) }
808C40A8 756D7FE0 7C00C002 	{ r3:r2 = combine(00000000,00000000); p0 = cmp.gt(r13,FFFFFFFF); r9:r8 = neg(r13:r12) }
FD094880 FD0D4C00 08004000 7800C003 D2E04200 5C004818 F921C381 	{ if (!p0) r1 = or(r1,r3); if (p0.new) jump:nt	0000A83C; p0 = dfcmp.eq(r1:r0,r3:r2); r3 = 80000000; if (p0) r1:r0 = combine(r13,r12); if (p0.new) r1:r0 = combine(r9,r8) }
529FC000     	{ jumpr	r31 }
8800403C DC8241F0 FD01E002 	{ if (!p0) r3:r2 = combine(r1,r0); p0 = dfclass(r3:r2,0000000F); r28 = convert_df2sf(r1:r0) }
88024022 529F4000 7C7FFFE0 	{ r1:r0 = combine(FFFFFFFF,FFFFFFFF); jumpr	r31; r2 = convert_df2sf(r3:r2) }
D2EC4000 537FD800 	{ if (!p0.new) jumpr:t	r31; p0 = dfcmp.eq(r13:r12,r1:r0) }
D2804200 535FD800 	{ if (p0.new) jumpr:t	r31; p0 = cmp.eq(r1:r0,r3:r2) }
6A08C01C     	{ r28 = USR }
8D5C42DC 7C00C000 	{ r1:r0 = combine(00000000,00000000); r28 = extractu(r28,00000002,00000012) }
751C4040 529F4000 08004000 7E00E001 DC824110 537F5800 F201C310 	{ p0 = !cmp.eq(r1,r3); if (!p0.new) jumpr:t	r31; p0 = dfclass(r3:r2,00000008); if (p0.new) r1 = 80000000; jumpr	r31; p0 = cmp.eq(r28,00000002) }
07F84000 7800C022 84824000 529FC000 	{ jumpr	r31; r1:r0 = convert_sf2df(r2); r2 = 7F800001 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __hexagon_divdf3: 0000A880
__hexagon_divdf3 proc
DC804053 DC824053 04004000 7C00C00E 5C2043C4 8320744E F50F4E0C 7C00C006 	{ r7:r6 = combine(00000000,00000000); r13:r12 = combine(r15,r14); r15:r14 = insert(r0,00000034,0000000C); if (!p3) jump:nt	0000AA18; r15:r14 = combine(40000000,00000000); p3 = dfclass(r3:r2,00000002); p3 = dfclass(r1:r0,00000002) }
8D414B84 8D434B85 7C00E00A 	{ r11:r10 = combine(00000000,00000001); r5 = extractu(r3,0000000B,00000013); r4 = extractu(r1,0000000B,00000013) }
690141C0 8322744C F50F4E08 F325C404 	{ r4 = sub(r4,r5); r9:r8 = combine(r15,r14); r13:r12 = insert(r2,00000034,0000000C); loop0(0000A8BC,00000018) }
D32CC8EE     	{ r15:r14 = sub(r9:r8,r13:r12) }
756F7FE0 FD0FEE08 	{ if (!p0) r9:r8 = combine(r15,r14); p0 = cmp.gt(r15,FFFFFFFF) }
C2C68606 8008C148 	{ r9:r8 = asl(r9:r8,00000001); r7:r6 = add(r7:r6,r7:r6,p0):carry }
D28A4881 F9266AA6 78DE4049 7801FFC8 	{ r8 = 000003FE; r9 = FFFFFC02; if (!p1.new) r6 = or(r6,r10); p1 = cmp.gtu(r11:r10,r9:r8) }
808640AE F161431C F2444900 F244C810 	{ p0 = !cmp.gt(r4,r8); p0 = cmp.gt(r4,r9); r28 = xor(r1,r3); r15:r14 = neg(r7:r6) }
5C204012 757C7FE1 FD07E62E 	{ if (!p1) r15:r14 = combine(r7,r6); p1 = cmp.gt(r28,FFFFFFFF); if (!p0) jump:nt	0000A914 }
80EE4060 BFE4F924 	{ r4 = add(r4,FFFFFFC9); r1:r0 = convert_d2df(r15:r14) }
8E0454C1 529FC000 	{ jumpr	r31; r1 += asl(r4,00000014) }
69015BC0 59FFFFD8 	{ jump	0000A8C0; loop0(0000A8BC,00000018) }
80EE4060 BFE4F924 	{ r4 = add(r4,FFFFFFC9); r1:r0 = convert_d2df(r15:r14) }
8E0454C1 8D41CB85 	{ r5 = extractu(r1,0000000B,00000013); r1 += asl(r4,00000014) }
F3044504 808EC0CC 	{ r13:r12 = abs(r15:r14); r4 = add(r4,r5) }
0000401F 754447C0 5C00C854 	{ if (p0.new) jump:nt	0000A9DC; p0 = cmp.gt(r4,000007FE) }
10844032 7800C7FC 	{ r28 = 0000003F; if (p0.new) jump:nt	0000A99C; p0 = cmp.gt(r4,00000000) }
6A084007 886C7A45 	{  }
76444064 756FFFE3 	{ p3 = cmp.gt(r15,FFFFFFFF); r4 = sub(00000003,r4) }
C38C458C D5BC4405 28042B06 	{ r6 = 00000030; r4 = 00000000; r5 = min(r28,r4); r13:r12 = asl(r13:r12,r5) }
C10C4408 C38CC50C 	{ r13:r12 = asr(r13:r12,r5); r9:r8 = extractu(r13:r12,r5:r4) }
D28A4880 8CCD570D F92CEA8C 	{ if (!p0.new) r12 = or(r12,r10); r13 = setbit(r13,0000000E); p0 = cmp.gtu(r11:r10,r9:r8) }
808C40AE 858C4701 F927E6A7 	{ if (!p1.new) r7 = or(r7,r6); p1 = bitsclr(r12,00000007); r15:r14 = neg(r13:r12) }
62274008 FD0D4C6E 7C004002 78DDF95C 	{ r28 = FFFFFBCA; r3:r2 = combine(00000000,00000000); if (p3) r15:r14 = combine(r13,r12); USR = r7 }
80EE4060 D2E2C260 	{ p0 = dfcmp.uo(r3:r2,r3:r2); r1:r0 = convert_d2df(r15:r14) }
8E1C54C1 529FC000 	{ jumpr	r31; r1 += asl(r28,00000014) }
07FE7FFF 780047FC 808EC0CE 	{ r15:r14 = abs(r15:r14); r28 = 7FEFFFFF }
C7815C00 537F5800 75004000 785FFFFC 	{ r28 = 00007FFF; p0 = cmp.eq(r0,00000000); if (!p0.new) jumpr:t	r31; p0 = bitsclr(r1,r28) }
6A084007 C74F5C00 7800C606 	{ r6 = 00000030; p0 = bitsset(r15,r28); r7 = USR }
F927C607     	{ if (p0) r7 = or(r7,r6) }
6227C008     	{ USR = r7 }
D2E04000 529FC000 	{ jumpr	r31; p0 = dfcmp.eq(r1:r0,r1:r0) }
6A08401C 07FE7FFF 7C7F67EE F50FCE00 	{ r1:r0 = combine(r15,r14); r15:r14 = combine(7FEFFFFF,FFFFFFFF); r28 = USR }
07FF4000 7C00400C 8D5C42C7 769CC51C 	{ r28 = or(r28,00000028); r7 = extractu(r28,00000002,00000012); r13:r12 = combine(7FF00000,00000000) }
623C4008 8E815F27 7067C006 	{ r6 = r7; XOREQ	r7,lsr(r1,0000001F); USR = r28 }
75064030 D2EE4E00 75074050 FD0DEC0E 	{ if (!p0) r15:r14 = combine(r13,r12); p0 = !cmp.eq(r7,00000002); p0 = dfcmp.eq(r15:r14,r15:r14); p0 = !cmp.eq(r6,00000001) }
830E7F00 529FC000 	{ jumpr	r31; r1:r0 = insert(r14,0000003F,00000007) }
DC8041F0 DC82C1F0 	{ p0 = dfclass(r3:r2,0000000F); p0 = dfclass(r1:r0,0000000F) }
DC804111 DC82C111 	{ p1 = dfclass(r3:r2,00000008); p1 = dfclass(r1:r0,00000008) }
DC804032 DC82C032 	{ p2 = dfclass(r3:r2,00000001); p2 = dfclass(r1:r0,00000001) }
5C204060 5C00C170 	{ if (p1) jump:nt	0000AB14; if (!p0) jump:nt	0000AAF0 }
5C00C26C     	{ if (p2) jump:nt	0000AB10 }
DC8041D0 DC82C0F0 	{ p0 = dfclass(r3:r2,00000007); p0 = dfclass(r1:r0,0000000E) }
DC8040F1 DC82C1D1 	{ p1 = dfclass(r3:r2,0000000E); p1 = dfclass(r1:r0,00000007) }
5C204032 5C20C13A 	{ if (!p1) jump:nt	0000AAC4; if (!p0) jump:nt	0000AAB0 }
DC804050 DC824051 7C00400E 7C00C00C 	{ r13:r12 = combine(00000000,00000000); r15:r14 = combine(00000000,00000000); p1 = dfclass(r3:r2,00000002); p0 = dfclass(r1:r0,00000002) }
8320744E 8322744C 04004000 7800C01C 8D414B84 8D434B85 F92F5C0F F92DDC2D 	{ if (p1) r13 = or(r13,r28); if (p0) r15 = or(r15,r28); r5 = extractu(r3,0000000B,00000013); r4 = extractu(r1,0000000B,00000013); r28 = 40000000; r13:r12 = insert(r2,00000034,0000000C); r15:r14 = insert(r0,00000034,0000000C) }
886E7F47     	{  }
886C7F46     	{  }
7C00E00A     	{ r11:r10 = combine(00000000,00000001) }
C38E478E C38C468C FB274A84 FB26CAA5 	{ if (!p1) r5 = sub(r10,r6); if (!p0) r4 = sub(r10,r7); r13:r12 = asl(r13:r12,r6); r15:r14 = asl(r15:r14,r7) }
59FF7F36 7C004006 F50F4E08 F325C404 	{ r4 = sub(r4,r5); r9:r8 = combine(r15,r14); r7:r6 = combine(00000000,00000000); jump	0000A90C }
D3E04280 7C00C002 	{ r3:r2 = combine(00000000,00000000); r1:r0 = xor(r1:r0,r3:r2) }
83027F00 529FC000 	{ jumpr	r31; r1:r0 = insert(r2,0000003F,00000007) }
DC824032 DC80C0F2 	{ p2 = dfclass(r1:r0,00000007); p2 = dfclass(r3:r2,00000001) }
6A08401C 5C20420A F161C301 	{ r1 = xor(r1,r3); if (!p2) jump:nt	0000AAE0; r28 = USR }
769CC09C     	{ r28 = or(r28,00000004) }
623CC008     	{ USR = r28 }
07FF4000 7C004002 D2E2C260 	{ p0 = dfcmp.uo(r3:r2,r3:r2); r3:r2 = combine(7FF00000,00000000) }
83027F00 529FC000 	{ jumpr	r31; r1:r0 = insert(r2,0000003F,00000007) }
DC804210 DC824211 FD036280 FD01E0A2 	{ if (!p1.new) r3:r2 = combine(r1,r0); if (!p0.new) r1:r0 = combine(r3,r2); p1 = dfclass(r3:r2,00000010); p0 = dfclass(r1:r0,00000010) }
8800402F 8802C02E 	{ r14 = convert_df2sf(r3:r2); r15 = convert_df2sf(r1:r0) }
7C7F7FE0 529FC000 	{ jumpr	r31; r1:r0 = combine(FFFFFFFF,FFFFFFFF) }
07F84000 7800C03C 849C4000 529FC000 	{ jumpr	r31; r1:r0 = convert_sf2df(r28); r28 = 7F800001 }

;; __hexagon_fast_muldf3: 0000AB20
__hexagon_fast_muldf3 proc
DC804050 DC824050 04004000 7C00C00C 8320744C 80024A44 78DE401C 7C00E008 	{ r9:r8 = combine(00000000,00000001); r28 = FFFFFC00; r5:r4 = asl(r3:r2,0000000A); r13:r12 = insert(r0,00000034,0000000C); r13:r12 = combine(40000000,00000000); p0 = dfclass(r3:r2,00000002); p0 = dfclass(r1:r0,00000002) }
E5444D06 83E8C2C4 	{ r5:r4 = insert(r8,00000002,0000003A); r7:r6 = mpyu(r4,r13) }
E54C440E E74CC506 	{  }
820E60A6     	{  }
E54D450A 0000401F 7C00C784 820660AA 	{  }
5C2040B2 750E4001 7506C001 	{ p1 = cmp.eq(r6,00000000); p1 = cmp.eq(r14,00000000); if (!p0) jump:nt	0000ACC8 }
8D414B86 8D434B87 F92AC8AA 	{ if (!p1) r10 = or(r10,r8); r7 = extractu(r3,0000000B,00000013); r6 = extractu(r1,0000000B,00000013) }
808A40AE EF1C4726 F161C31C 	{ r28 = xor(r1,r3); r6 += add(r28,r7); r15:r14 = neg(r11:r10) }
FD0F6ECA 14C6442C 757C7FE2 F246C510 	{ p0 = !cmp.gt(r6,r5); p2 = cmp.gt(r28,FFFFFFFF); if (!p0.new) jump:nt	0000ABE4; p0 = cmp.gt(r6,-00000001); if (!p2.new) r11:r10 = combine(r15,r14) }
80EA4060 BFA6F8C6 	{ r6 = add(r6,FFFFFBC6); r1:r0 = convert_d2df(r11:r10) }
8E0654C1 529F4000 7F004000 7F00C000 	{ nop; nop; jumpr	r31; r1 += asl(r6,00000014) }
C7814400 537F5800 75004000 785FFFE5 	{ r5 = 00007FFF; p0 = cmp.eq(r0,00000000); if (!p0.new) jumpr:t	r31; p0 = bitsclr(r1,r4) }
6A084004 C74D4500 7800C605 	{ r5 = 00000030; p0 = bitsset(r13,r5); r4 = USR }
F924C504     	{ if (p0) r4 = or(r4,r5) }
6224C008     	{ USR = r4 }
D2E04000 529F4000 7F00C000 	{ nop; jumpr	r31; p0 = dfcmp.eq(r1:r0,r1:r0) }
80EA4060 808A40CC BFA6F8C7 	{ r7 = add(r6,FFFFFBC6); r13:r12 = abs(r11:r10); r1:r0 = convert_d2df(r11:r10) }
8E0754C1 8D414B87 07FE7FFF 7800C7E4 0FFF7FEF E20640C7 7800C005 	{ r5 = 00000000; r7 += add(r6,FFFFFBC6); r4 = 7FEFFFFF; r7 = extractu(r1,0000000B,00000013); r1 += asl(r7,00000014) }
0000401F 754747C0 5C00C83C 	{ if (p0.new) jump:nt	0000AC88; p0 = cmp.gt(r7,000007FE) }
10B740CE F3254605 7800C7FC 	{ r28 = 0000003F; r5 = sub(r6,r5); if (p0.new) jump:nt	0000ABB0; p0 = cmp.gt(r23,00000001) }
78004004 7645C0A5 	{ r5 = sub(00000005,r5); r4 = 00000000 }
756B7FE3 D5BC4505 F50DCC0A 	{ r11:r10 = combine(r13,r12); r5 = min(r28,r5); p3 = cmp.gt(r11,FFFFFFFF) }
6A08401C C10AC40E 	{ r15:r14 = extractu(r11:r10,r5:r4); r28 = USR }
C38A450A 8F494B81 7800C604 	{ r4 = 00000030; r1 = insert(0000000B,00000013); r11:r10 = asr(r11:r10,r5) }
D2884E80 8CCB570B F92AE88A 	{ if (!p0.new) r10 = or(r10,r8); r11 = setbit(r11,0000000E); p0 = cmp.gtu(r9:r8,r15:r14) }
808A40AE 858A4701 F924FCBC 	{ if (!p1.new) r28 = or(r4,r28); p1 = bitsclr(r10,00000007); r15:r14 = neg(r11:r10) }
623C4008 FD0FCEEA 	{ if (p3.new) r11:r10 = combine(r15,r14); USR = r28 }
80EA4060 D2E0C000 	{ p0 = dfcmp.eq(r1:r0,r1:r0); r1:r0 = convert_d2df(r11:r10) }
8F494AA1 529F4000 7F004000 7F00C000 	{ nop; nop; jumpr	r31; r1 = insert(0000000A,00000012) }
6A08401C 07FE7FFF 7C7F67EC F50BCA00 	{ r1:r0 = combine(r11,r10); r13:r12 = combine(7FEFFFFF,FFFFFFFF); r28 = USR }
8D5C42CE 769C451C 07FF4000 7C00C004 623C4008 8E815F2E 706EC01C 	{ r28 = r14; XOREQ	r14,lsr(r1,0000001F); USR = r28; r5:r4 = combine(7FF00000,00000000); r28 = or(r28,00000028); r14 = extractu(r28,00000002,00000012) }
751C4030 D2E04000 750E4050 FD05E40C 	{ if (!p0) r13:r12 = combine(r5,r4); p0 = !cmp.eq(r14,00000002); p0 = dfcmp.eq(r1:r0,r1:r0); p0 = !cmp.eq(r28,00000001) }
830C7F00 529FC000 	{ jumpr	r31; r1:r0 = insert(r12,0000003F,00000007) }
81007F0C 8102FF04 	{ r5:r4 = extractu(r3:r2,0000003F,00000007); r13:r12 = extractu(r1:r0,0000003F,00000007) }
D28C4483 FD0362E0 FD01E0E2 	{ if (!p3.new) r3:r2 = combine(r1,r0); if (!p3.new) r1:r0 = combine(r3,r2); p3 = cmp.gtu(r13:r12,r5:r4) }
DC8041F0 5C20484E FD0544EC FD0DCCE4 	{ if (p3.new) r5:r4 = combine(r13,r12); if (p3.new) r13:r12 = combine(r5,r4); if (!p0.new) jump:nt	0000AD78; p0 = dfclass(r1:r0,0000000F) }
DC804111 DC82C1D1 	{ p1 = dfclass(r3:r2,0000000E); p1 = dfclass(r1:r0,00000008) }
DC804110 DC82C030 	{ p0 = dfclass(r3:r2,00000001); p0 = dfclass(r1:r0,00000008) }
5C004150 DC82C032 	{ p2 = dfclass(r3:r2,00000001); if (p1) jump:nt	0000AD98 }
5C00402E 5C004248 07C04000 7800C01C C7815C00 5C00C810 	{ if (p0.new) jump:nt	0000AD34; p0 = bitsclr(r1,r28); r28 = 7C000000; if (p2) jump:nt	0000AD94; if (p0) jump:nt	0000AD5C }
8844C05C     	{ r28 = cl0(r5:r4) }
BFFCFEBC     	{ r28 = add(r28,FFFFFFF5) }
C384DC84     	{ r5:r4 = asl(r5:r4,r28) }
83047F02 8E1CD441 	{ r1 -= asl(r28,00000014); r3:r2 = insert(r4,0000003F,00000007) }
59FFFEFA     	{ jump	0000AB20 }
6A08401C D3E0C280 	{ r1:r0 = xor(r1:r0,r3:r2); r28 = USR }
83087F00 8D5C42C5 769CC61C 	{ r28 = or(r28,00000030); r5 = extractu(r28,00000002,00000012); r1:r0 = insert(r8,0000003F,00000007) }
623C4008 8E815F25 75454020 7E80E000 	{ if (!p0.new) r0 = 00000000; p0 = cmp.gt(r5,00000001); XOREQ	r5,lsr(r1,0000001F); USR = r28 }
7E806000 59533FC0 	{ jumpr	r31; p0 = cmp.eq(r5,00000003); if (!p0.new) r0 = 00000000 }
6A08C01C     	{ r28 = USR }
7C7F7FE0 769CC05C 	{ r28 = or(r28,00000002); r1:r0 = combine(FFFFFFFF,FFFFFFFF) }
623CC008     	{ USR = r28 }
D2E04060 529FC000 	{ jumpr	r31; p0 = dfcmp.uo(r1:r0,r1:r0) }
DC8241F0 8800403C FD01E002 	{ if (!p0) r3:r2 = combine(r1,r0); r28 = convert_df2sf(r1:r0); p0 = dfclass(r3:r2,0000000F) }
88024022 529F4000 7C7F7FE0 7F00C000 	{ nop; r1:r0 = combine(FFFFFFFF,FFFFFFFF); jumpr	r31; r2 = convert_df2sf(r3:r2) }
F5034200 F501C002 	{ r3:r2 = combine(r1,r0); r1:r0 = combine(r3,r2) }
8DE3C1E3     	{ r3 = extract(r3,00000001,00000019) }
8E835F41 529FC000 	{ jumpr	r31; XOREQ	r1,asl(r3,0000001F) }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }
DC804053 5C204B46 01004000 7C00C00E 8320740E 8D414B84 F50FCE06 	{ r7:r6 = combine(r15,r14); r4 = extractu(r1,0000000B,00000013); r15:r14 = insert(r0,00000034,0000000C); r15:r14 = combine(10000000,00000000); if (!p3.new) jump:nt	0000AE50; p3 = dfclass(r1:r0,00000002) }
690142D0 13C1415C 	{  }
7801FFFC     	{ r28 = 000003FF }
85044000 800E414C F71CC404 	{ r4 = vavgh(r28,r4); r13:r12 = asl(r15:r14,00000001); p0 = tstbit(r4,00000000) }
D10E4C08 8006C122 	{ r3:r2 = lsr(r7:r6,00000001); r9:r8 = vmux(p0,r15:r14,r13:r12) }
D32648E8 7800C03C 	{ r28 = 00000001; r9:r8 = sub(r9:r8,r7:r6) }
820641C2     	{  }
80084148 F503C20A 	{ r11:r10 = combine(r3,r2); r9:r8 = asl(r9:r8,00000001) }
D32248E8 D3E64A4A F50B4A02 F509C80C 	{ r13:r12 = combine(r9,r8); r3:r2 = combine(r11,r10); r11:r10 = or(r7:r6,r11:r10); r9:r8 = sub(r9:r8,r3:r2) }
7569BFE0 80024122 FD0D6C88 FD0BEA06 	{ if (!p0) r7:r6 = combine(r11,r10); if (!p0.new) r9:r8 = combine(r13,r12); r3:r2 = lsr(r3:r2,00000001); p0 = cmp.gt(r9,FFFFFFFF) }
75094000 75084000 F926FC86 	{ if (!p0.new) r6 = or(r6,r28); p0 = cmp.eq(r8,00000000); p0 = cmp.eq(r9,00000000) }
80E64060 BFA4F8A4 	{ r4 = add(r4,FFFFFBC5); r1:r0 = convert_d2df(r7:r6) }
8E0454C1 529FC000 	{ jumpr	r31; r1 += asl(r4,00000014) }
DC804210 5C00C81C 	{ if (p0.new) jump:nt	0000AE88; p0 = dfclass(r1:r0,00000010) }
DC804030 5C00C81E 	{ if (p0.new) jump:nt	0000AE94; p0 = dfclass(r1:r0,00000001) }
11C1C11C     	{ if (!p0.new) jump:t	0000AE94; p0 = tstbit(r1,00000000) }
DC804110 5C00C818 	{ if (p0.new) jump:nt	0000AE94; p0 = dfclass(r1:r0,00000008) }
88607544     	{  }
01004000 7C00C006 76444024 B004C105 	{ r5 = add(r4,00000008); r4 = sub(00000001,r4); r7:r6 = combine(10000000,00000000) }
C380458E 59FFFFB0 	{ jump	0000ADE0; r15:r14 = asl(r1:r0,r5) }
88004024 529F4000 7C7FFFE0 	{ r1:r0 = combine(FFFFFFFF,FFFFFFFF); jumpr	r31; r4 = convert_df2sf(r1:r0) }
529FC000     	{ jumpr	r31 }
07F84000 7800C024 84844000 529FC000 	{ jumpr	r31; r1:r0 = convert_sf2df(r4); r4 = 7F800001 }
7F00C000     	{ nop }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; _exit: 0000AEB0
_exit proc
70604003 00004390 49804701 A09DC001 	{ allocframe(+00000008); r1 = memw(gp+224); r3 = r0 }
10014012 749D6084 A19DC301 	{ memw(r29+4) = r3; if (!p0.new) r4 = add(r29,00000004); if (p0.new) jump:nt	0000AEE4; p0 = cmp.eq(r1,00000000) }
7800C300     	{ r0 = 00000018 }
7064C001     	{ r1 = r4 }
7063C002     	{ r2 = r3 }
5400C000     	{ trap0(00000000) }
7060C003     	{ r3 = r0 }
50303F40     	{ dealloc_return; r0 = r3 }
5BFFD1AE     	{ call	00005240 }
7800C003     	{ r3 = 00000000 }
50303F40     	{ dealloc_return; r0 = r3 }

;; lockMutex: 0000AEF0
lockMutex proc
9200C002     	{ r2 = memw_locked(r0) }
6E884004 8C025003 70E2C001 	{ r1 = sxth(r2); r3 = asr(r2,00000010); r4 = htid }
F2034401 8C045044 B002C022 	{ r2 = add(r2,00000001); r4 = asl(r4,00000010); p1 = cmp.eq(r3,r4) }
5C00C10A     	{ if (p1) jump:nt	0000AF20 }
75014001 B004C022 	{ r2 = add(r4,00000001); p1 = cmp.eq(r1,00000000) }
5C00C104     	{ if (p1) jump:nt	0000AF20 }
59FFFFEA     	{ jump	0000AEF0 }
A0A0C200     	{ memw_locked(r0,p0) = r2 }
5CFFE0E6     	{ if (!p0) jump:nt	0000AEF0 }
529FC000     	{ jumpr	r31 }
7F00C000     	{ nop }

;; __sys_Mtxunlock: 0000AF30
__sys_Mtxunlock proc
9200C001     	{ r1 = memw_locked(r0) }
6E884004 8D414305 7800C023 	{ r3 = 00000001; r5 = extractu(r1,00000003,00000013); r4 = htid }
F2054401 F683C103 	{ r3 = vsubh(r1,r3); p1 = cmp.eq(r5,r4) }
537FC100     	{ if (!p1) jumpr:nt	r31 }
A0A0C300     	{ memw_locked(r0,p0) = r3 }
5CFFE0F0     	{ if (!p0) jump:nt	0000AF30 }
529FC000     	{ jumpr	r31 }
7F004000 7F00C000 	{ nop; nop }

;; __sys_Mtxinit: 0000AF60
__sys_Mtxinit proc
7800C001     	{ r1 = 00000000 }
529F4000 A180C100 	{ memw(r0) = r1; jumpr	r31 }
7F00C000     	{ nop }

;; __sys_Mtxdst: 0000AF70
__sys_Mtxdst proc
529FC000     	{ jumpr	r31 }
7F004000 7F004000 7F00C000 	{ nop; nop; nop }

;; __trylockMutex: 0000AF80
__trylockMutex proc
6E88C004     	{ r4 = htid }
9200C002     	{ r2 = memw_locked(r0) }
8C02D003     	{ r3 = asr(r2,00000010) }
F203C401     	{ p1 = cmp.eq(r3,r4) }
7422C022     	{ if (p1) r2 = add(r2,00000001) }
5C00C110     	{ if (p1) jump:nt	0000AFB4 }
70E2C003     	{ r3 = sxth(r2) }
7503C001     	{ p1 = cmp.eq(r3,00000000) }
8C04D045     	{ r5 = asl(r4,00000010) }
7425C002     	{ if (p1) r2 = add(r5,00000000) }
7422C022     	{ if (p1) r2 = add(r2,00000001) }
5C00C104     	{ if (p1) jump:nt	0000AFB4 }
48003FC0     	{ jumpr	r31; r0 = 00000000 }
A0A0C200     	{ memw_locked(r0,p0) = r2 }
5CFFE0E6     	{ if (!p0) jump:nt	0000AF84 }
48103FC0     	{ jumpr	r31; r0 = 00000001 }

;; __sys_close: 0000AFC0
__sys_close proc
00004390 49804701 A09DC002 	{ allocframe(+00000010); r1 = memw(gp+224) }
1001401C EA0C0810 	{ memw(r29+4) = r0; memd(r29+8) = r17:r16; if (p0.new) jump:nt	0000B004; p0 = cmp.eq(r1,00000000) }
5BFF531E B01D4080 28412C18 	{ r16 = add(r29,00000004); r1 = 00000004; r0 = add(r29,00000004); call	00005610 }
7800C040     	{ r0 = 00000002 }
7070C001     	{ r1 = r16 }
5400C000     	{ trap0(00000000) }
7060C002     	{ r2 = r0 }
7061C010     	{ r16 = r1 }
11C2E00E     	{ if (!p0.new) jump:t	0000B010; p0 = cmp.eq(r2,-00000001) }
5BFFDF8C     	{ call	00006F10 }
5800400A 7A020008 	{ memw(r0) = r16; r2 = -00000001; jump	0000B010 }
5BFFDF86     	{ call	00006F10 }
78DF7FE2 3C40C059 	{ memw(r0) = 00000059; r2 = FFFFFFFF }
70624000 3E0C1F40 	{ dealloc_return; r17:r16 = memd(r29+8); r0 = r2 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __sys_remove: 0000B020
__sys_remove proc
00004390 49804701 A09DC002 	{ allocframe(+00000010); r1 = memw(gp+224) }
1001402C 70082A0C 	{ memd(r29+8) = r17:r16; r16 = r0; if (p0.new) jump:nt	0000B084; p0 = cmp.eq(r1,00000000) }
5BFF6356 B01D4011 70802808 	{ memw(r29) = r16; r0 = r16; r17 = add(r29,00000000); call	000076E0 }
8CD14202 30013080 	{ r0 = r16; r1 = r0; r2 = setbit(r17,00000004) }
5BFF634C A182C100 	{ memw(r2) = r1; call	000076E0 }
5BFF52E0 30803101 	{ r1 = r0; r0 = r16; call	00005610 }
5BFF52DC 28812C00 	{ r0 = add(r29,00000000); r1 = 00000008; call	00005610 }
7800C1C0     	{ r0 = 0000000E }
7071C001     	{ r1 = r17 }
5400C000     	{ trap0(00000000) }
7060C002     	{ r2 = r0 }
7061C010     	{ r16 = r1 }
11C2E00E     	{ if (!p0.new) jump:t	0000B090; p0 = cmp.eq(r2,-00000001) }
5BFFDF4C     	{ call	00006F10 }
5800400A 7A020008 	{ memw(r0) = r16; r2 = -00000001; jump	0000B090 }
5BFFDF46     	{ call	00006F10 }
78DF7FE2 3C40C059 	{ memw(r0) = 00000059; r2 = FFFFFFFF }
70624000 3E0C1F40 	{ dealloc_return; r17:r16 = memd(r29+8); r0 = r2 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __sys_sbrk: 0000B0A0
__sys_sbrk proc
70604011 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17 = r0 }
5BFF7F24 00004401 7800C580 00004380 7800C301 00004401 9B805A10 	{  }
2442E012 00004404 	{  }
48030012     	{ r2 = memw(r1); r3 = 00000000 }
75024010 74026010 7483E010 	{ if (!p0.new) r16 = add(r3,00000000); if (p0.new) r16 = add(r2,00000000); p0 = !cmp.eq(r2,00000000) }
A180D000     	{ memw(r0) = r16 }
1009C01C     	{ if (p0.new) jump:nt	0000B118; p0 = cmp.eq(r9,00000000) }
50930012     	{ r2 = memw(r1); r3 = r17 }
E210C0E3     	{ r3 += add(r16,00000007) }
76237F01 2203E210 	{ r1 = and(r3,FFFFFFF8) }
00004380 49804382 2402E008 1542E106 	{ if (cmp.eq(r2.new,00000000)) jump:t	0000B10C; r2 = memw(gp+112) }
5800400A A0010089 	{ memw(r16) = r17; memw(r0) = r1; jump	0000B118 }
5BFF5F02 78DFFFF0 	{ r16 = FFFFFFFF; call	00006F10 }
3C40C00C     	{ memw(r0) = 0000000C }
5BFF7F0C 00004401 7800C580 70704000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = r16; r0 = 0001006C; call	0000AF30 }
7F00C000     	{ nop }

;; BeforeBegin: 0000B130
BeforeBegin proc
59FF7F18 00004401 78004600 7F00C000 	{ nop; r0 = 00010070; jump	0000AF60 }

;; AtEnd: 0000B140
AtEnd proc
59FF7F18 00004401 78004600 7F00C000 	{ nop; r0 = 00010070; jump	0000AF70 }

;; sys_Tlsalloc: 0000B150
sys_Tlsalloc proc
73206190     	{  }
EBF41C10     	{ allocframe(00000008); memd(r29+496) = r17:r16 }
1008C036     	{ if (p0.new) jump:nt	0000B1C4; p0 = cmp.eq(r8,00000000) }
5BFF7ECA 00004401 7800C600 0FE1413F 780047E0 000043B0 7800C301 000043AC 2AC228B9 001E7EC0 B000C023 5C004816 7583C7E0 	{ p0 = cmp.gtu(r3,0000003F); if (p0.new) jump:nt	0000B1B4; r3 = add(r0,01EFB001); r17 = 0000000B; r2 = 0000002C; r1 = 0000EC18; r0 = FE104FFF; r0 = 00010070; call	0000AEF0 }
B0004020 B0024082 41810123 	{ r3 = memw(r2+4); r1 = add(r1,00000018); r2 = add(r2,00000004); r0 = add(r0,00000001) }
1073E0F2     	{ if (!p0.new) jump:t	0000B180; p0 = cmp.eq(r19,00000001) }
69004032 68033120 	{ memw(r2) = 00000001; r3 = 00000000; loop0(0000B1A8,00000006) }
7F008000 AB81C308 	{ memw(r1++#4) = r3; nop }
68090080     	{ memw(r16) = r0; r17 = 00000000 }
5BFF7EBE 00004401 78004600 7F00C000 	{ nop; r0 = 00010070; call	0000AF30 }
70714000 7F004000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); nop; r0 = r17 }

;; sys_Tlsfree: 0000B1D0
sys_Tlsfree proc
001E7EC0 B0004011 EBF41C10 	{ allocframe(00000008); memd(r29+496) = r17:r16; r17 = add(r0,01EFB000) }
5C005820 780042D0 7591C7E0 	{ p0 = cmp.gtu(r17,0000003F); r16 = 00000016; if (p0.new) jump:t	0000B21C }
000043AC 9D917C00 2402C01A 00004401 	{ if (cmp.eq(r0.new,00000000)) jump:nt	0000B224; r0 = memw(r17<<#2+0000EB30) }
78004600 000043AC 7800C610 5BFFFE76 	{ call	0000AEF0; r16 = 0000EB30; r0 = 00000030 }
C4115040 7800C010 	{ r16 = 00000000; r0 = addasl(r16,r17,00000002) }
5BFF7E90 00004401 6B003000 70704000 3E041F40 	{ dealloc_return; r17:r16 = memd(r29); r0 = r16; memw(r0) = 00000000; r0 = 00000030; call	0000AF30 }

;; sys_Tlsset: 0000B224
sys_Tlsset proc
001E7EC0 B0004002 7800C2C0 	{ r0 = 00000016; r2 = add(r0,01EFB000) }
5C005814 7582C7E0 	{ p0 = cmp.gtu(r2,0000003F); if (p0.new) jump:t	0000B258 }
000043AC 9D827C03 2403C010 	{ r3 = memw(r2<<#2+0000EB30) }
6E88C003     	{ r3 = htid }
000043B0 2B042800 DF02C2C4 	{ r2 = add(r4,mpyi(00000018,r2)); r0 = 00000000; r4 = 00000030 }
3B82E301     	{ memw(r14+r3<<#2) = r1 }
7F004000 529FC000 	{ jumpr	r31; nop }

;; sys_Tlsget: 0000B260
sys_Tlsget proc
001E7EC0 B0004001 7800C000 	{ r0 = 00000000; r1 = add(r0,01EFB000) }
5C005814 7581C7E0 	{ p0 = cmp.gtu(r1,0000003F); if (p0.new) jump:t	0000B294 }
000043AC 9D817C02 2402C010 6E88C000 	{ if (cmp.eq(r2.new,00000000)) jump:nt	0000B29C; r2 = memw(r1<<#2+0000EB30) }
000043B0 7800C602 DF01C1C2 	{ r1 = add(r2,mpyi(00000018,r1)); r2 = 0000EC30 }
3A81E000     	{ r0 = memw(r30+r0<<#2) }
529FC000     	{ jumpr	r31 }
7F00C000     	{ nop }
7F00C000     	{ nop }

;; __sys_write: 0000B2A0
__sys_write proc
70604003 00004390 49804704 A09DC003 	{ allocframe(+00000018); r4 = memw(gp+224); r3 = r0 }
10044024 70102A14 	{ memd(r29+16) = r17:r16; r0 = r1; if (p0.new) jump:nt	0000B2F8; p0 = cmp.eq(r4,00000000) }
70624001 6C082803 	{ memw(r29) = r3; r16 = add(r29,00000000); r1 = r2 }
8CD0C204     	{ r4 = setbit(r16,00000004) }
5BFF51A6 A0400282 	{ memw(r16+8) = r2; memw(r4) = r0; call	00005610 }
5BFF51A2 28C12C00 	{ r0 = add(r29,00000000); r1 = 0000000C; call	00005610 }
7800C0A0     	{ r0 = 00000005 }
7070C001     	{ r1 = r16 }
5400C000     	{ trap0(00000000) }
7060C002     	{ r2 = r0 }
7061C010     	{ r16 = r1 }
11C2E00E     	{ if (!p0.new) jump:t	0000B304; p0 = cmp.eq(r2,-00000001) }
5BFFDE12     	{ call	00006F10 }
5800400A 7A020008 	{ memw(r0) = r16; r2 = -00000001; jump	0000B304 }
5BFFDE0C     	{ call	00006F10 }
78DF7FE2 3C40C059 	{ memw(r0) = 00000059; r2 = FFFFFFFF }
70624000 3E141F40 	{ dealloc_return; r17:r16 = memd(r29+16); r0 = r2 }
7F00C000     	{ nop }

;; sqrt: 0000B310
sqrt proc
F5014010 EBF41C30 	{ allocframe(00000018); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
5BFF715C B01D40C0 6C212A0C 	{ memd(r29+8) = r17:r16; r1 = add(r29,00000008); r0 = add(r29,00000006); call	000095D0 }
1000C012     	{ if (p0.new) jump:nt	0000B348; p0 = cmp.eq(r0,00000000) }
1000E104     	{ if (p0.new) jump:t	0000B330; p0 = cmp.eq(r0,00000002) }
1000E20E     	{ if (p0.new) jump:t	0000B348; p0 = cmp.eq(r0,00000004) }
B01DC100     	{ r0 = add(r29,00000008) }
7680C0C0     	{ r0 = or(r0,00000006) }
91404000 2682E008 5BFF7198 	{ if (tstbit(r0.new,-00000001)) jump:t	0000B34C; r0 = memh(r0) }
7800C020     	{ r0 = 00000001 }
5BFF7D3C F511D000 	{ r1:r0 = combine(r17,r16); call	0000ADC0 }
3E141F40     	{ dealloc_return; r17:r16 = memd(r29+16) }
