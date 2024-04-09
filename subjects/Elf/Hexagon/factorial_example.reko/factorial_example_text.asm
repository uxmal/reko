;;; Segment .text (00005000)

;; .EventVectors: 00005000
.EventVectors proc
	{ jump event_handle_reset }
00005004             32 DD FF 59 36 DD FF 59 3A DD FF 59     2..Y6..Y:..Y
00005010 58 DD FF 59 36 DD FF 59 D4 DD FF 59 32 DD FF 59 X..Y6..Y...Y2..Y
00005020 48 DE FF 59 06 DF FF 59 2C DD FF 59 2A DD FF 59 H..Y...Y,..Y*..Y
00005030 28 DD FF 59 26 DD FF 59 24 DD FF 59 22 DD FF 59 (..Y&..Y$..Y"..Y
00005040 FE DE FF 59 FC DE FF 59 FA DE FF 59 F8 DE FF 59 ...Y...Y...Y...Y
00005050 F6 DE FF 59 F4 DE FF 59 F2 DE FF 59 F0 DE FF 59 ...Y...Y...Y...Y
00005060 EE DE FF 59 EC DE FF 59 EA DE FF 59 E8 DE FF 59 ...Y...Y...Y...Y
00005070 E6 DE FF 59 E4 DE FF 59 E2 DE FF 59 E0 DE FF 59 ...Y...Y...Y...Y
00005080 DE DE FF 59 DC DE FF 59 DA DE FF 59 D8 DE FF 59 ...Y...Y...Y...Y
00005090 D6 DE FF 59 D4 DE FF 59 D2 DE FF 59 D0 DE FF 59 ...Y...Y...Y...Y
000050A0 CE DE FF 59 CC DE FF 59 CA DE FF 59 C8 DE FF 59 ...Y...Y...Y...Y
000050B0 C6 DE FF 59 C4 DE FF 59 C2 DE FF 59 C0 DE FF 59 ...Y...Y...Y...Y

;; strict_aliasing_workaround: 000050C0
strict_aliasing_workaround proc
	{ allocframe(#0x10) }
	{ memw(r30-12) = r0 }
	{ r0 = memw(r30-12) }
	{ memw(r30-4) = r0 }
	{ r0 = memw(r30-4) }
	{ deallocframe }
	{ jumpr r31 }

;; fact: 000050DC
;;   Called from:
;;     00005108 (in fact)
;;     000051AC (in main)
fact proc
	{ allocframe(#0x8) }
	{ memw(r30-4) = r0 }
	{ r0 = memw(r30-4) }
	{ p0 = cmp.gt(r0,#0x1) }
	{ r0 = mux(p0,#0x1,#0x0) }
	{ p0 = tstbit(r0,#0x0) }
	{ if (p0) jump:nt 00005100 }

l000050F8:
	{ r0 = #0x1 }
	{ jump 00005118 }

l00005100:
	{ r0 = memw(r30-4) }
	{ r0 = add(r0,#0xFFFFFFFF) }
	{ call fact }
	{ r1 = r0 }
	{ r0 = memw(r30-4) }
	{ r0 = mpyi(r1,r0) }

l00005118:
	{ deallocframe }
	{ jumpr r31 }

;; main: 00005120
;;   Called from:
;;     000055F4 (in __libc_start_main)
main proc
	{ allocframe(#0x18) }
	{ memw(r30-12) = r0 }
	{ memw(r30-16) = r1 }
	{ r0 = memw(r30-12) }
	{ p0 = cmp.eq(r0,#0x2) }
	{ r0 = mux(p0,#0x1,#0x0) }
	{ p0 = tstbit(r0,#0x0) }
	{ if (!p0) jump:nt 00005180 }

l00005140:
	{ r0 = memw(r30-16) }
	{ r0 = add(r0,#0x4) }
	{ r0 = memw(r0) }
	{ call atoi }
	{ memw(r30-4) = r0 }
	{ r0 = memw(r30-4) }
	{ memw(r29) = r0 }
	{ immext(#0xD000); r0 = #0xD000 }
	{ call printf }
	{ r0 = memw(r30-4) }
	{ p0 = cmp.gt(r0,#0xFFFFFFFF) }
	{ r0 = mux(p0,#0x1,#0x0) }
	{ p0 = tstbit(r0,#0x0) }
	{ if (!p0) jump:nt 00005194 }

l0000517C:
	{ jump 000051A8 }

l00005180:
	{ immext(#0xD000); r0 = #0xD013 }
	{ call printf }
	{ r0 = #0xFFFFFFFF }
	{ jump 000051D4 }

l00005194:
	{ immext(#0xD000); r0 = #0xD02F }
	{ call printf }
	{ r0 = #0xFFFFFFFF }
	{ jump 000051D4 }

l000051A8:
	{ r0 = memw(r30-4) }
	{ call fact }
	{ memw(r30-8) = r0 }
	{ r0 = memw(r30-4) }
	{ memw(r29) = r0 }
	{ r0 = memw(r30-8) }
	{ memw(r29+4) = r0 }
	{ immext(#0xD040); r0 = #0xD050 }
	{ call printf }
	{ r0 = #0x0 }

l000051D4:
	{ deallocframe }
	{ jumpr r31 }

;; atoi: 000051DC
;;   Called from:
;;     0000514C (in main)
atoi proc
	{ allocframe(#0x8) }
	{ memw(r30-4) = r0 }
	{ r0 = memw(r30-4) }
	{ r1 = #0x0 }
	{ r2 = #0xA }
	{ call _Stoul }
	{ deallocframe }
	{ jumpr r31 }
000051FC                                     00 C0 00 7F             ....

;; thread_create: 00005200
thread_create proc
	{ r4.h = #0x0; r7 = asl(r2,#0x2); r5.h = #0x0; r6.h = #0x0 }
	{ r4.l = #0x6B90; r5.l = #0x6BF0; r6.l = #0x6C50; r8 = #0x1 }
	{ r5 = add(r7,r5); r8 &= asl(r8,r2); r4 = add(r4,r7); r6 = add(r6,r7) }
	{ memw(r4) = r0 }
	{ memw(r5) = r1 }
	{ memw(r6) = r3 }
	{ start(r8) }
	{ jumpr r31 }

;; thread_stop: 00005240
;;   Called from:
;;     0000AEE4 (in _exit)
thread_stop proc
	{ r0 = htid; r1 = #0x1 }
	{ r1 &= lsl(r1,r0) }
	{ stop(r1) }
	{ r28.h = #0x0 }
	{ r28.l = #0x1DC0 }
	{ jumpr r28 }
0000525C                                     00 C0 00 7F             ....

;; thread_join: 00005260
thread_join proc
	{ r1 = htid; r3 = #0x1 }
	{ r1 &= asl(r3,r1) }
	{ r1 = sub(#0xFFFFFFFF,r1) }
	{ r0 = and(r0,r1) }
	{ r0 = combine(r0.l,r0.l) }
	{ r2 = modectl }
	{ r2 = and(r0,r2) }
	{ p0 = cmp.eq(r2,#0x0); if (p0.new) jumpr r31 }
00005284             04 C0 40 54 F8 FF FF 59 00 C0 00 7F     ..@T...Y....

;; thread_get_tnum: 00005290
thread_get_tnum proc
	{ r0 = htid; jumpr r31 }
00005298                         00 40 00 7F 00 C0 00 7F         .@......

;; thread_stack_size: 000052A0
thread_stack_size proc
	{ r2.h = #0x0 }
	{ r2.l = #0x6CB0; r3 = asl(r0,#0x2) }
	{ r4 = add(r2,r3) }
	{ memw(r4) = r1 }
	{ jumpr r31 }
000052B8                         00 C0 00 7F 00 C0 00 7F         ........

;; __sys_get_cmdline: 000052C0
__sys_get_cmdline proc
	{ r2 = #0x0; immext(#0xE400); r3 = memw(gp+58400); allocframe(#0x10) }
	{ p0 = cmp.eq(r3,#0x0); if (p0.new) jump:nt 00005300; memd(r29+8) = r17:r16 }

l000052D8:
	{ r16 = add(r29,#0x0); memw(r29) = r0 }
	{ r2 = setbit(r16,#0x4) }
	{ call hexagon_cache_cleaninv; memw(r2) = r1 }
	{ call hexagon_cache_cleaninv; r1 = #0x8; r0 = add(r29,#0x0) }
	{ r0 = #0x15 }
	{ r1 = r16 }
	{ trap0(#0x0) }
	{ r2 = r0 }

l00005300:
	{ r0 = r2; r17:r16 = memd(r29+8); dealloc_return }
00005308                         00 C0 00 7F 00 C0 00 7F         ........

;; printf: 00005310
;;   Called from:
;;     00005164 (in main)
;;     00005188 (in main)
;;     0000519C (in main)
;;     000051CC (in main)
printf proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = add(r29,#0x18); immext(#0xE480); r0 = #0xE498 }
	{ call _Lockfilelock }
	{ r2 = r16; immext(#0xE480); r1 = #0xE498; r3 = memd(r29+4) }
	{ call _Printf; immext(#0x5340); r0 = #0x5360 }
	{ call _Unlockfilelock; nop; immext(#0xE480); r0 = #0xE498; r16 = r0 }
	{ r0 = r16; nop; r17:r16 = memd(r29+8); dealloc_return }

;; prout: 00005360
prout proc
	{ r4 = r1; r17:r16 = combine(r0,r2); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ call fwrite; r1:r0 = combine(#0x1,r4); r3 = r17 }
	{ p0 = cmp.eq(r0,r16); if (p0.new) r0 = add(r17,#0x0); r0 = #-0x1; r17:r16 = memd(r29) }
	{ dealloc_return }
00005388                         00 C0 00 7F 00 C0 00 7F         ........

;; memset: 00005390
memset proc
	{ if (r2=#0x0) jump:nt 00005490; r7 = vsplatb(r1); r6 = r0 }

l0000539C:
	{ r5:r4 = combine(r7,r7); p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:nt 000053BC }

l000053A4:
	{ loop0(000053B0,r2); r3 = r0; nop }
	{ nop; memb(r3++#1) = r1 }
	{ jumpr r31 }

l000053BC:
	{ p0 = tstbit(r0,#0x0); if (!p0.new) jump:nt 000053D0; p1 = cmp.eq(r2,#0x1) }

l000053C4:
	{ r2 = add(r2,#0xFFFFFFFF); if (p1) jump:nt 00005490; r6 = r0; memb(r0) = r1 }

l000053D0:
	{ p0 = tstbit(r6,#0x1); if (!p0.new) jump:nt 000053E4 }

l000053D8:
	{ r2 = add(r2,#0xFFFFFFFE); p0 = cmp.eq(r2,#0x2); if (p0.new) jump:nt 00005490; memuh(r6++#2) = r7 }

l000053E4:
	{ p0 = tstbit(r6,#0x2); if (!p0.new) jump:nt 000053F8 }

l000053EC:
	{ r2 = add(r2,#0xFFFFFFFC); p0 = cmp.eq(r2,#0x4); if (p0.new) jump:nt 00005490; memw(r6++#4) = r7 }

l000053F8:
	{ p0 = cmp.gtu(r2,#0x7F); if (!p0.new) jump:nt 00005450 }

l00005400:
	{ r3 = and(r6,#0x1F) }
	{ if (r3=#0x0) jump:nt 00005430 }

l00005408:
	{ r2 = add(r2,#0xFFFFFFF8); memd(r6++#8) = r5:r4 }
	{ r3 = and(r6,#0x1F) }
	{ if (r3=#0x0) jump:nt 00005430 }

l00005418:
	{ r2 = add(r2,#0xFFFFFFF8); memd(r6++#8) = r5:r4 }
	{ r3 = and(r6,#0x1F) }
	{ if (r3=#0x0) jump:nt 00005430 }

l00005428:
	{ r2 = add(r2,#0xFFFFFFF8); memd(r6++#8) = r5:r4 }

l00005430:
	{ if (r1!=#0x0) jump:nt 00005494; r3 = lsr(r2,#0x5) }

l00005438:
	{ loop0(00005444,r3); r8 = r3; r3 = r6 }
	{ r6 = add(r6,#0x20); r2 = add(r2,#0xFFFFFFE0); dczeroa(r6) }

l00005450:
	{ p0 = cmp.gtu(r2,#0x7); if (!p0.new) jump:nt 00005468; r8 = lsr(r2,#0x3) }

l00005458:
	{ loop0(00005460,r8); nop }
	{ r2 = add(r2,#0xFFFFFFF8); memd(r6++#8) = r5:r4 }

l00005468:
	{ p0 = tstbit(r2,#0x2); if (!p0.new) jump:nt 00005478 }

l00005470:
	{ r2 = add(r2,#0xFFFFFFFC); memw(r6++#4) = r7 }

l00005478:
	{ p0 = tstbit(r2,#0x1); if (!p0.new) jump:nt 00005488 }

l00005480:
	{ r2 = add(r2,#0xFFFFFFFE); memuh(r6++#2) = r7 }

l00005488:
	{ p0 = cmp.eq(r2,#0x1) }
	{ if (p0) memb(r6) = r1 }

l00005490:
	{ jumpr r31 }

l00005494:
	{ loop0(00005498,r3) }
	{ dczeroa(r6) }
	{ r2 = add(r2,#0xFFFFFFE0); memd(r6++#16) = r5:r4; memd(r6+8) = r5:r4 }
	{ memd(r6++#16) = r5:r4; memd(r6+8) = r5:r4 }
	{ jump 00005450 }
000054B4             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; __libc_start_main: 000054C0
__libc_start_main proc
	{ p0 = cmp.gt(r1,#0xFFFFFFFF); r16 = r3; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ if (!p0) jump:nt 000055FC }

l000054D0:
	{ p0 = cmp.gt(r1,#0x0); if (!p0.new) jump:t 00005500 }

l000054D4:
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:t 00005500 }

l000054D8:
	{ r3 = memb(r0) }
	{ if (!p0.new) jump:nt 00005500; p0 = cmp.eq(r3,#0x20) }

l000054E8:
	{ r3 = add(r0,#0x1); r1 = add(r1,#0xFFFFFFFF) }
	{ p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:t 000054D8; r0 = r3 }

l000054FC:
	{ jump 00005504 }

l00005500:
	{ r3 = r0 }

l00005504:
	{ p0 = cmp.gt(r1,#0x0); if (!p0.new) jump:nt 000055C8; r0 = #0x0; r17 = #0x0 }

l0000550C:
	{ r6 = #0x0; r17 = #0x0; r5:r4 = combine(#0x0,#0x1) }
	{ p0 = cmp.eq(r4,#0x0); if (p0.new) jump:nt 000055C8 }

l00005518:
	{ if (p0.new) jump:nt 000055C8; p0 = cmp.gt(r17,#0x63) }

l00005520:
	{ r7 = memb(r4) }
	{ p0 = cmp.eq(r7,#0x0); if (p0.new) jump:t 0000554C }

l0000552C:
	{ p0 = cmp.eq(r7,#0x9); if (p0.new) jump:t 0000554C }

l00005530:
	{ jump 000055BC }
00005534             08 58 20 5C 40 C4 07 75 11 33 43 31     .X \@..u.3C1
00005540 20 C4 03 17 3C 58 20 5C 00 C4 07 75              ...<X \...u    

l0000554C:
	{ p0 = cmp.eq(r6,#0x0); if (!p0.new) jump:t 00005570 }

l00005550:
	{ r6 = #0x0; p1 = cmp.eq(r4,r3); p0 = cmp.eq(r7,#0x0); memb(r4) = #0x0 }
	{ if (!p1) jump:nt 000055A0; if (p0) r5 = #0x1 }

l00005564:
	{ if (!p0.new) jump:t 000055A0; p0 = !cmp.eq(r7,00000000) }

l0000556C:
	{ jump 000055BC }

l00005570:
	{ p0 = cmp.eq(r7,#0x0); if (p0.new) jump:nt 000055C8 }

l00005574:
	{ r0 = #0x0; jump 000055BC; r3 = add(r4,#0x1) }
0000557C                                     11 33 44 31             .3D1
00005580 06 C0 24 91 0C 58 00 5C 40 C4 06 75 F8 E0 76 10 ..$..X.\@..u..v.
00005590 3A 40 00 58 41 43 00 00 E0 C4 00 78 40 32 16 68 :@.XAC.....x@2.h

l000055A0:
	{ r7 = memb(r3) }
	{ r17 = add(r17,#0x1); immext(#0xE700); memw(r17<<#2+0000E700) = r3 }
	{ p0 = cmp.eq(r5,#0x0); if (!p0.new) jump:nt 000055C8 }

l000055B8:
	{ r5 = #0x0; r3 = r4 }

l000055BC:
	{ r4 = add(r4,#0x1); r1 = add(r1,#0xFFFFFFFF) }

l000055C8:
	{ p0 = cmp.eq(r2,#0x0); if (p0.new) jump:t 000055D8; immext(#0xE700); memw(r17<<#2+0000E700) = r0 }

l000055D4:
	{ callr r2 }

l000055D8:
	{ p0 = cmp.eq(r8,#0x0); if (p0.new) jump:t 000055E4; if (!p0.new) r0 = add(r16,#0x0) }

l000055E0:
	{ call atexit }

l000055E4:
	{ immext(#0xE700); r1:r0 = combine(#0xE700,r17); immext(#0xE440); r2 = #0xE440 }
	{ call main }
	{ call exit }

l000055FC:
	{ r0 = #0x1; r17:r16 = memd(r29); dealloc_return }
00005604             BE CE 00 5A B4 4C 00 5A 00 C0 00 78     ...Z.L.Z...x

;; hexagon_cache_cleaninv: 00005610
;;   Called from:
;;     000052E0 (in __sys_get_cmdline)
;;     000052E8 (in __sys_get_cmdline)
;;     0000AFD4 (in __sys_close)
;;     0000B050 (in __sys_remove)
;;     0000B058 (in __sys_remove)
;;     0000B2C4 (in __sys_write)
;;     0000B2CC (in __sys_write)
hexagon_cache_cleaninv proc
	{ r2 = and(r0,#0x1F) }
	{ r2 += add(r1,#0x1F) }
	{ r1 = lsr(r2,#0x5) }
	{ r1 = asl(r1,#0x5); r3 = #0x0; r4 = r0 }
	{ r3 -= lsr(r2,#0x5) }
	{ r2 = sub(#0x0,r3) }
	{ loop0(00005634,r2) }
	{ r2 = add(r4,#0x20) }
	{ dccleaninva(r4) }
	{ r4 = r2; nop }
	{ r0 = add(r0,r1) }
	{ r0 = add(r0,#0xFFFFFFE0) }
	{ r1 = memb(r0) }
	{ dccleaninva(r0) }
	{ jumpr r31 }

;; hexagon_cache_inva: 00005658
hexagon_cache_inva proc
	{ r2 = and(r0,#0x1F) }
	{ r2 += add(r1,#0x1F) }
	{ r1 = lsr(r2,#0x5) }
	{ r1 = #0x0 }
	{ r1 -= lsr(r2,#0x5) }
	{ r1 = sub(#0x0,r1) }
	{ loop0(00005678,r1) }
	{ r1 = add(r0,#0x20) }
	{ dcinva(r0) }
	{ r0 = r1; nop }
	{ jumpr r31 }
0000568C                                     00 C0 00 7F             ....

;; __registerx: 00005690
__registerx proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r19:r18 = combine(r4,r3); p0 = cmp.eq(r16,#0x0); memd(r29) = r19:r18 }
	{ if (p0) jump:nt 000056D0 }

l000056A4:
	{ r0 = memw(r16); if (cmp.eq(r0.new,#0x0)) jump:nt 000056D0 }

l000056B0:
	{ r1:r0 = combine(#0x18,#0x1) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 000056D8; if (p0.new) memw(r0+12) = r18 }

l000056BC:
	{ memw(r0+8) = r17; memw(r0+20) = #0x0 }
	{ memw(r0+4) = r16; memw(r0+16) = r19 }
	{ r1 = memw(gp+116) }
	{ memw(gp) = r0 }

l000056D0:
	{ r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

l000056D8:
	{ call abort; nop }

;; __register_frame_info_bases: 000056E0
;;   Called from:
;;     000056D8 (in __registerx)
__register_frame_info_bases proc
	{ r17:r16 = combine(r2,r0); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r18 = r3; p0 = cmp.eq(r16,#0x0); memd(r29) = r19:r18 }
	{ if (p0) jump:nt 00005720 }

l000056F4:
	{ r0 = memw(r16); if (cmp.eq(r0.new,#0x0)) jump:nt 00005720 }

l00005700:
	{ r1:r0 = combine(#0x18,#0x1) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00005728; if (p0.new) memw(r0+12) = r17 }

l0000570C:
	{ memw(r0+8) = #0x0; memw(r0+20) = #0x0 }
	{ memw(r0+4) = r16; memw(r0+16) = r18 }
	{ r1 = memw(gp+116) }
	{ memw(gp) = r0 }

l00005720:
	{ r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

l00005728:
	{ call abort; nop }

;; __deregister_frame_info_bases: 00005730
;;   Called from:
;;     00005728 (in __register_frame_info_bases)
__deregister_frame_info_bases proc
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000576C; allocframe(#0x0) }

l00005738:
	{ r1 = memw(r0) }
	{ immext(#0x10040); r1 = #0x10074 }
	{ r2 = r1 }
	{ r1 = memw(r2) }
	{ r3 = memw(r1+4) }
	{ call free; r0 = r1; r3 = memw(r1) }

l0000576C:
	{ r0 = #0x0; dealloc_return }

;; _Printf: 00005770
;;   Called from:
;;     00005338 (in printf)
_Printf proc
	{ call __save_r16_through_r25; allocframe(#0xC8) }
	{ r19:r18 = combine(r1,r0); r0 = add(r29,#0x54); r1 = r3; r5:r4 = combine(#0x0,#0x0) }
	{ immext(#0x7FFFFFC0); r17:r16 = combine(r2,#0x7FFFFFFF); r21 = add(r29,#0x60); memd(r29+88) = r5:r4 }
	{ call _Vacopy; r22 = add(r29,#0x50); r23 = #0x3E7; r24 = #0x3E8 }
	{ immext(#0xD080); r25 = #0xD088; memw(r21+8) = r18; memw(r21+12) = r19 }
	{ jump 000057B8; memw(r21+52) = #0xFFFFFF80 }

l000057B4:
	{ r17 = add(r17,#0x1) }

l000057B8:
	{ r3 = add(r29,#0x58); r2 = r16; r0 = add(r29,#0x50); r1 = r17 }
	{ call _Mbtowc; memw(r22) = #0x0 }
	{ r18 = r0; if (cmp.gt(r18.new,#0x0)) jump:t 000057DC }

l000057D8:
	{ r18 = !cmp.eq(r0,00000000) }

l000057DC:
	{ r0 = memw(r29+80) }
	{ p0 = cmp.eq(r0,#0x25); if (p0.new) r1 = #0xFFFFFFFF; if (!p0.new) r1 = #0x0 }
	{ r19 = add(r1,r18) }
	{ r1 = r17; r2 = r19; r0 = memw(r21+12); r3 = memw(r21+8) }
	{ r20 = #0xFFFFFFFF; callr r3 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00005ABC; memw(r21+12) = r0 }

l00005810:
	{ r0 = memw(r21+52) }
	{ r0 = add(r0,r19); memb(r21+13) = r0.new }
	{ if (!p0.new) jump:nt 00005AA4; r17 = add(r17,r18); p0 = cmp.eq(r0,#0x25) }

l0000582C:
	{ r19:r18 = combine(#0x0,#0x0); memw(r21+44) = #0x0 }

l00005830:
	{ call _Getpctype; r20 = memb(r18) }
	{ r0 = memb(r13+r20<<#1) }
	{ r0 = and(r0,#0x20); if (cmp.eq(r0.new,#0x0)) jump:nt 0000585C }

l00005848:
	{ r19 = memb(r18++#1) }
	{ r19 += add(r0,#0xFFFFFFD0) }
	{ if (p0.new) jump:t 00005830; p0 = cmp.gt(r24,r19); memw(r21+44) = r19 }

l0000585C:
	{ r0 = memb(r18) }
	{ if (!p0.new) jump:t 0000587C; p0 = cmp.eq(r0,#0x24) }

l00005868:
	{ r18 = r18; r0 = r19 }
	{ jump 00005880; p0 = cmp.gtu(r23,r0); if (!p0.new) r18 = add(r17,#0x0); if (!p0.new) memw(r21+44) = #0x0 }

l0000587C:
	{ r18 = r17; memw(r21+44) = #0x0 }

l00005880:
	{ r2 = #0x5; memw(r21+36) = #0xFFFFFF80; memuh(r21+60) = #0x0 }
	{ immext(#0xD080); r0 = #0xD088; memw(r21+40) = #0x0; memw(r21+20) = #0x0 }
	{ memw(r21+24) = #0x0; memw(r21+28) = #0x0 }
	{ memw(r21+32) = #0x0 }
	{ call memchr; r1 = memb(r18) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:t 000058E4 }

l000058AC:
	{ r0 = #0x0; jump 000058BC; r18 = add(r18,#0x1) }

l000058B4:
	{ r18 = add(r18,#0x1); r1 = memuh(r21+60) }

l000058BC:
	{ r0 = sub(r0,r25); r2 = #0x5 }
	{ immext(#0xD080); r0 = memw(r0<<#2+0000D098) }
	{ r0 = or(r1,r0); memb(r21+30) = r0.new }
	{ immext(#0xD080); r0 = #0xD088; r1 = memb(r18) }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 000058B4 }

l000058E4:
	{ r0 = memb(r18) }
	{ if (!p0.new) jump:t 00005928; p0 = cmp.eq(r0,#0x2A); if (!p0.new) memw(r21+56) = #0x0 }

l000058F4:
	{ r0 = memw(r29+84) }
	{ r1 = add(r0,#0x4) }
	{ r0 = memw(r0) }
	{ p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:t 00005920; if (!p0.new) r1 = memh(r21+60); memw(r21+56) = r0 }

l00005910:
	{ r1 = setbit(r1,#0x4); r0 = sub(#0x0,r0) }
	{ memuh(r21+60) = r1; memw(r21+56) = r0 }

l00005920:
	{ jump 00005964; r18 = add(r18,#0x1) }

l00005928:
	{ call _Getpctype; r17 = memb(r18) }
	{ jump 0000593C }

l00005934:
	{ call _Getpctype; r18 = r18; r17 = memb(r18+1) }

l00005938:
	{ r18 = r18; r17 = memb(r18+1) }

l0000593C:
	{ r0 = memb(r13+r17<<#1) }
	{ r0 = and(r0,#0x20); if (cmp.eq(r0.new,#0x0)) jump:t 00005964 }

l0000594C:
	{ if (cmp.eq(r0.new,r16)) jump:t 00005938 }

l00005954:
	{ r1 = memb(r18) }
	{ r1 += add(r0,#0xFFFFFFD0); jump 00005934 }

l00005964:
	{ r0 = memb(r18) }
	{ if (!p0.new) jump:t 0000599C; p0 = cmp.eq(r0,#0x2E); if (!p0.new) memw(r21+48) = #0xFFFFFFFF }

l00005974:
	{ r0 = memb(r18+1) }
	{ if (!p0.new) jump:t 000059A0; p0 = cmp.eq(r0,#0x2A); if (!p0.new) memw(r21+48) = #0x0 }

l00005984:
	{ r18 = add(r18,#0x2); r0 = memd(r29+84) }
	{ r1 = add(r0,#0x4) }
	{ jump 000059E4; r0 = memw(r0); memb(r21+12) = r0.new }

l0000599C:
	{ jump 000059E4 }

l000059A0:
	{ call _Getpctype; r17 = memb(r18+1) }
	{ jump 000059C0; r18 = add(r18,#0x1); r0 = memb(r13+r17<<#1) }

l000059B4:
	{ call _Getpctype; r18 = r18; r17 = memb(r18+1) }

l000059B8:
	{ r18 = r18; r17 = memb(r18+1) }

l000059BC:
	{ r0 = memb(r13+r17<<#1) }

l000059C0:
	{ r0 = and(r0,#0x20); if (cmp.eq(r0.new,#0x0)) jump:t 000059E4 }

l000059CC:
	{ if (cmp.eq(r0.new,r16)) jump:t 000059B8 }

l000059D4:
	{ r1 = memb(r18) }
	{ r1 += add(r0,#0xFFFFFFD0); jump 000059B4 }

l000059E4:
	{ call strchr; immext(#0xD080); r0 = #0xD0B0; r1 = memb(r18) }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 00005A00; if (!p0.new) r17 = add(r18,#0x0); if (p0.new) memb(r21-2) = #0x0 }

l000059FC:
	{ r9 = r10; jump 00005A50 }

l00005A00:
	{ r0 = memb(r17++#1); memb(r21+62) = r0.new }
	{ p0 = cmp.eq(r0,#0x6C); if (p0.new) r0 = memb(r17) }
	{ if (!p0.new) jump:t 00005A50; if (p0.new) r17 = add(r18,#0x2); p0 = cmp.eq(r0,#0x6C) }

l00005A20:
	{ jump 00005A50; nop; nop; memb(r21-2) = #0x71 }
00005A30 10 58 20 5C 00 4D 00 75 00 C0 31 43 0A 58 20 5C .X \.M.u..1C.X \
00005A40 51 60 12 74 00 CD 00 75 00 40 00 7F 62 DF 15 3C Q`.t...u.@..b..<

l00005A50:
	{ r0 = memw(r21+44); if (!cmp.gt(r0.new,#0x0)) jump:t 00005A80 }

l00005A5C:
	{ r0 = add(r29,#0x4) }
	{ call _Vacopy; r1 = memw(r29+84) }
	{ r1 = add(r29,#0x4); r3 = add(r29,#0x8); r0 = add(r29,#0x60); r2 = memb(r17) }
	{ jump 00005A8C; nop }

l00005A80:
	{ r1 = add(r29,#0x54); r0 = add(r29,#0x60); r3 = add(r29,#0x8); r2 = memb(r17) }

l00005A8C:
	{ call _Putfld }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 00005AB8 }

l00005A94:
	{ call _Puttxt; r1 = add(r29,#0x8); r0 = add(r29,#0x60) }
	{ p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:t 000057B4 }

l00005AA0:
	{ jump 00005AB8 }

l00005AA4:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 000057B8; if (p0.new) r0 = add(r29,#0x60) }

l00005AAC:
	{ r20 = memw(r0+52) }
	{ immext(#0x4880); r0 = r12; jump fn00005B40 }

l00005AB8:
	{ r20 = #0xFFFFFFFF }

l00005ABC:
	{ immext(#0x4880); r0 = r12; jump fn00005B1C }
00005AC4             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; _Putfld: 00005AD0
;;   Called from:
;;     00005A8C (in _Printf)
_Putfld proc
	{ p0 = cmp.gtu(r2,#0x40); r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ if (p0) jump:nt 00005B00 }

l00005AE0:
	{ if (!p0.new) jump:t 00005C48; p0 = cmp.eq(r2,#0x25); if (p0.new) r0 = memw(r16+20) }

l00005AEC:
	{ r1 = add(r0,#0x1); r0 = add(r3,r0) }
	{ jump 00005CFC; memb(r0) = #0x25 }

l00005B00:
	{ if (p0.new) jump:t 00005B60; p0 = cmp.gtu(r2,#0x52) }

l00005B08:
	{ r0 = add(r2,#0xFFFFFFBF); if (cmp.gtu(r0.new,#0x6)) jump:t 00005C48 }

l00005B14:
	{ r0 = and(r0,#0x71); if (cmp.eq(r0.new,#0x0)) jump:nt 00005C48 }

;; fn00005B1C: 00005B1C
;;   Called from:
;;     00005ABC (in _Printf)
;;     00005AE0 (in _Putfld)
;;     00005B04 (in _Putfld)
;;     00005B10 (in _Putfld)
;;     00005B78 (in _Putfld)
;;     00005B8C (in _Putfld)
fn00005B1C proc
	{ r0 = memw(r16+44); if (!cmp.gt(r0.new,#-0x1)) jump:nt 00005C78 }

l00005B20:
	{ if (!cmp.gt(r0.new,#-0x1)) jump:nt fn00005C7C }

;; fn00005B24: 00005B24
;;   Called from:
;;     00005B4C (in fn00005B40)
;;     00005B4C (in fn00005B40)
;;     00005B54 (in fn00005B54)
;;     00005CFC (in fn00005B1C)
;;     00005ED4 (in _Putfld)
;;     00005EE4 (in _Putfld)
fn00005B24 proc
	{ r0 = memw(r1) }

;; fn00005B28: 00005B28
;;   Called from:
;;     00005B1C (in fn00005B1C)
;;     00005B24 (in fn00005B24)
fn00005B28 proc
	{ r0 = add(r0,#0x7) }
	{ r0 = and(r0,#0xFFFFFFF8) }
	{ r4 = add(r0,#0x8); memb(r1) = r4.new }
	{ r5:r4 = memd(r0) }

;; fn00005B40: 00005B40
;;   Called from:
;;     00005AB0 (in _Printf)
;;     00005B3C (in fn00005B28)
fn00005B40 proc
	{ r0 = add(r6,#0xFFFFFFFF); p0 = cmp.gt(r6,#0x0); memd(r16) = r5:r4 }
	{ if (p0) jump:nt fn00005B24; memw(r16+44) = r0 }

;; fn00005B54: 00005B54
;;   Called from:
;;     00005B24 (in fn00005B24)
;;     00005B4C (in fn00005B40)
;;     00005B4C (in fn00005B40)
fn00005B54 proc
	{ r1:r0 = combine(r5,r4) }
	{ r1:r0 = lsr(r1:r0,#0x30); jump fn00005C7C }

l00005B60:
	{ if (p0.new) jump:t 00005B8C; if (p0.new) r0 = add(r2,#0xFFFFFFA8); p0 = cmp.gtu(r2,#0x57) }

l00005B6C:
	{ if (!p0.new) jump:t 00005C48; if (p0.new) r2 = add(r16,#0x2C); p0 = cmp.eq(r2,#0x53) }

l00005B78:
	{ r0 = #0x0; r3 = memw(r16+44); memb(r16-2) = #0x6C }
	{ p0 = cmp.gt(r3,#-0x1); if (p0.new) jump:t 00005EA8 }

l00005B88:
	{ jump 00005EC4 }

l00005B8C:
	{ if (p0.new) jump:t 00005C48; p0 = cmp.gtu(r0,#0x20) }

l00005B94:
	{ r4 = memw(gp+120) }
	{ r0 = memw(r14+r0<<#2) }
	{ jumpr r0 }
00005BA0 60 41 90 91 42 C0 C2 26 C0 C7 30 91 0A 58 20 5C `A..B..&..0..X \
00005BB0 20 4E 00 75 00 C0 81 43 1A 40 00 58 E0 C0 00 B0  N.u...C.@.X....
00005BC0 10 58 20 5C 80 4D 00 75 00 40 81 43 04 C0 81 47 .X \.M.u.@.C...G
00005BD0 84 40 00 B0 00 D2 A1 A1 1A 40 00 58 00 C0 80 91 .@.......@.X....
00005BE0 10 58 20 5C E0 60 04 74 40 CD 00 75 00 FF 20 76 .X \.`.t@..u.. v
00005BF0 04 41 00 B0 00 D2 A1 A1 0C 40 00 58 04 C0 C0 91 .A.......@.X....
00005C00 80 40 04 B0 00 D2 A1 A1 00 C0 84 91 04 E0 20 73 .@............ s
00005C10 60 41 90 91 00 C4 D0 A1 C8 60 B0 10 E4 7F E0 BF `A.......`......
00005C20 0B D2 B0 A1 C0 C7 30 91 C0 58 00 5C E0 CC 80 75 ......0..X.\...u
00005C30 74 78 20 5C 40 4C 00 75 00 C0 D0 43 C4 C2 C0 49 tx \@L.u...C...I
00005C40 00 44 E0 D3 6A C1 00 58                         .D..j..X        

l00005C48:
	{ p0 = cmp.eq(r2,#0x0); r0 = memw(r16+20) }
	{ r1 = add(r0,#0x1); r0 = add(r3,r0) }
	{ if (p0) jump:nt 00005CFC; memb(r0) = #0x25 }

l00005C60:
	{ r0 = memw(r16+20) }
	{ r1 = add(r0,#0x1); r0 = add(r3,r0) }
	{ jump 00005CFC; memb(r0) = r2 }

l00005C78:
	{ r0 = memh(r16+6) }

;; fn00005C7C: 00005C7C
;;   Called from:
;;     00005B60 (in fn00005B54)
;;     00005C78 (in fn00005B1C)
fn00005C7C proc
	{ r0 = sxth(r0); if (!cmp.gt(r0.new,#-0x1)) jump:nt 00005CB0 }

l00005C88:
	{ p0 = tstbit(r1,#0x0); r0 = and(r1,#0x2); if (cmp.eq(r0.new,#0x0)) jump:nt 00005CCC }

l00005C98:
	{ r1 = memw(r16+20) }
	{ r4 = add(r1,#0x1); r1 = add(r3,r1); memb(r16+5) = r4.new }
	{ memb(r1) = #0x2B }

l00005CB0:
	{ r0 = add(r16,#0x14); r1 = memw(r16+20) }
	{ r4 = add(r1,#0x1); r1 = add(r3,r1); memb(r16+5) = r4.new }
	{ memb(r1) = #0x2D }

l00005CCC:
	{ if (!p0) jump:nt 00005CE8; r0 = add(r16,#0x14); if (p0) r1 = memw(r16+20) }

l00005CD8:
	{ r4 = add(r1,#0x1); r1 = add(r3,r1); memb(r16+5) = r4.new }

l00005CE8:
	{ r1:r0 = combine(r2,r16); r4 = memw(r0) }
	{ call _Ldtob; r2 = add(r3,r4); memb(r16+4) = r2.new }

l00005CFC:
	{ r1 = #0x0 }

l00005D00:
	{ r0 = r1; r17:r16 = memd(r29+8); dealloc_return }
00005D08                         60 41 90 91 42 C0 C2 26         `A..B..&
00005D10 C0 C7 30 91 0A 58 20 5C 20 4E 00 75 00 C0 81 43 ..0..X \ N.u...C
00005D20 1A 40 00 58 E0 C0 00 B0 10 58 20 5C 80 4D 00 75 .@.X.....X \.M.u
00005D30 00 40 81 43 04 C0 81 47 84 40 00 B0 00 D2 A1 A1 .@.C...G.@......
00005D40 1A 40 00 58 00 C0 80 91 10 58 20 5C E0 60 04 74 .@.X.....X \.`.t
00005D50 40 CD 00 75 00 FF 20 76 04 41 00 B0 00 D2 A1 A1 @..u.. v.A......
00005D60 0C 40 00 58 04 C0 C0 91 80 40 04 B0 00 D2 A1 A1 .@.X.....@......
00005D70 00 C0 84 91 04 C0 40 84 60 41 90 91 00 C4 D0 A1 ......@.`A......
00005D80 C8 60 B0 10 E4 7F E0 BF 0B D2 B0 A1 C0 C7 30 91 .`............0.
00005D90 A6 58 00 5C E0 CC 80 75 56 78 20 5C 50 41 00 58 .X.\...uVx \PA.X
00005DA0 40 4C 00 75 00 C0 10 43 08 48 00 5C 40 CF 00 75 @L.u...C.H.\@..u
00005DB0 A8 58 20 5C 80 CE 00 75 84 01 80 00 AC C0 01 16 .X \...u........
00005DC0 C2 47 30 91 60 C1 90 91 DC 58 20 5C 02 60 00 7E .G0.`....X \.`.~
00005DD0 80 CD 02 75 10 C1 C0 11 00 C0 81 91 82 40 00 B0 ...u.........@..
00005DE0 00 D2 A1 A1 02 00 83 0B F8 60 B3 10 E0 7F E3 BF .........`......
00005DF0 0B D2 B0 A1 01 40 1D B0 00 40 70 70 02 28 03 6C .....@...@pp.(.l
00005E00 03 42 C3 8C 7F E6 50 3C 60 40 00 58 00 C0 43 3C .B....P<`@.X..C<
00005E10 C0 C7 30 91 D4 58 00 5C E0 CC 80 75 7C 78 20 5C ..0..X.\...u|x \
00005E20 40 CC 00 75 00 C0 81 91 82 40 00 B0 00 D2 A1 A1 @..u.....@......
00005E30 00 00 82 0B F8 60 B2 10 E3 7F E2 BF 0B D3 B0 A1 .....`..........
00005E40 5E 7F FF 59 A1 41 90 91 00 C3 A0 A1 04 40 00 7C ^..Y.A.......@.|
00005E50 60 41 90 91 16 C0 C2 26 00 C0 81 91 82 40 00 B0 `A.....&.....@..
00005E60 00 D2 A1 A1 00 00 82 0B F8 60 B2 10 E4 7F E2 BF .........`......
00005E70 0B D2 B0 A1 04 E0 20 73 00 6F 30 73 A2 40 90 91 ...... s.o0s.@..
00005E80 00 C4 D0 A1 AC 41 00 58 02 C2 03 F3 C2 47 30 91 .....A.X.....G0.
00005E90 60 C1 90 91 BC 58 20 5C 82 65 10 74 80 CD 02 75 `....X \.e.t...u
00005EA0 12 41 C0 11 00 C0 00 78                         .A.....x        

l00005EA8:
	{ r0 = memw(r1) }
	{ r3 = add(r0,#0x4) }
	{ r3 = memw(r2); r0 = memw(r0) }
	{ p0 = cmp.gt(r3,#0x0); if (p0.new) jump:t 00005EA8; r4 = add(r3,#0xFFFFFFFF); memb(r2) = r4.new }

l00005EC4:
	{ r1:r0 = combine(r0,r16) }

l00005EC8:
	{ call _Putstr }
	{ r1 = #0xFFFFFFFF; p0 = cmp.gt(r0,#0xFFFFFFFF) }
	{ jump 00005D00; if (p0) r1 = #0x0 }
00005EDC                                     AE 48 00 5C             .H.\
00005EE0 00 4D 00 75 00 C0 50 43 08 58 00 5C 80 CE 00 75 .M.u..PC.X.\...u
00005EF0 AA 58 20 5C 40 CF 00 75 A0 40 00 58 00 C0 90 91 .X \@..u.@.X....
00005F00 0C 58 20 5C 00 4D 00 75 00 C0 D0 43 E4 C2 C0 49 .X \.M.u...C...I
00005F10 00 C4 E0 D3 00 C0 D0 A1 04 40 00 7C 80 C7 30 91 .........@.|..0.
00005F20 00 41 00 76 24 C0 02 24 00 C0 D0 91 00 44 80 D2 .A.v$..$.....D..
00005F30 1E C8 00 5C 00 C5 C2 8C 18 48 20 5C 00 4F 00 75 ...\.....H \.O.u
00005F40 A1 C0 90 43 00 41 03 F3 21 40 01 B0 05 D3 B0 A1 ...C.A..!@......
00005F50 30 C0 00 3C A0 C0 90 91 21 40 00 B0 00 40 03 F3 0..<....!@...@..
00005F60 05 D5 B0 A1 00 C2 00 A1 A0 C0 90 91 00 40 03 F3 .............@..
00005F70 04 D2 B0 A1 CE 50 00 5A 00 D0 02 F5 C0 FE FF 59 .....P.Z.......Y
00005F80 12 41 C0 11 00 C0 00 78 00 C0 81 91 82 40 00 B0 .A.....x.....@..
00005F90 00 D2 A1 A1 00 00 82 0B F8 60 B2 10 E4 7F E2 BF .........`......
00005FA0 0B D2 B0 A1 A1 C0 90 91 22 40 01 B0 01 41 03 F3 ........"@...A..
00005FB0 05 D4 B0 A1 A4 7E FF 59 00 C0 01 A1 64 58 00 5C .....~.Y....dX.\
00005FC0 20 CF 80 75 80 58 00 5C 60 CE 80 75 90 58 00 5C  ..u.X.\`..u.X.\
00005FD0 00 CE 80 75 B4 48 00 5C 00 CD 00 75 C4 58 00 5C ...u.H.\...u.X.\
00005FE0 40 CD 00 75 98 58 20 5C 80 CD 00 75 00 C0 81 91 @..u.X \...u....
00005FF0 82 40 00 B0 00 D2 A1 A1 00 00 82 0B F8 60 B2 10 .@...........`..
00006000 E3 7F E2 BF 0B D3 B0 A1 94 C0 00 58 C2 41 C0 11 ...........X.A..
00006010 80 C0 90 47 00 C0 81 91 82 40 00 B0 00 D2 A1 A1 ...G.....@......
00006020 00 00 82 0B 00 40 42 75 80 04 23 73 F4 60 DF 5C .....@Bu..#s.`.\
00006030 0B C3 90 A1 AE C0 00 58 00 C0 40 84 06 40 00 58 .......X..@..@.X
00006040 00 C0 D0 A1 00 C0 D0 91 E4 FF 7F 7C 40 44 80 D2 ...........|@D..
00006050 30 48 20 5C 80 62 90 74 C1 C3 70 43 00 40 01 85 0H \.b.t..pC.@..
00006060 40 40 01 76 AC C0 02 24 80 42 10 B0 A1 C0 90 91 @@.v...$.B......
00006070 24 40 01 B0 01 41 03 F3 05 D4 B0 A1 AA 40 00 58 $@...A.......@.X
00006080 2B C0 01 3C 48 58 20 5C 40 CF 00 75 00 C0 81 91 +..<HX \@..u....
00006090 82 40 00 B0 00 D2 A1 A1 00 00 82 0B F8 60 B2 10 .@...........`..
000060A0 E3 7F E2 BF 0B D3 B0 A1 44 C0 00 58 A1 C0 90 91 ........D..X....
000060B0 24 40 01 B0 01 41 03 F3 05 D4 B0 A1 8A 40 00 58 $@...A.......@.X
000060C0 2D C0 01 3C 28 58 20 5C 80 CE 00 75 00 C0 81 91 -..<(X \...u....
000060D0 82 40 00 B0 00 D2 A1 A1 00 00 82 0B F8 60 B2 10 .@...........`..
000060E0 E3 7F E2 BF 0B D3 B0 A1 24 C0 00 58 14 58 20 5C ........$..X.X \
000060F0 20 CE 00 75 00 C0 81 91 82 40 00 B0 00 D2 A1 A1  ..u.....@......
00006100 00 00 82 0B F8 60 B2 10 E3 7F E2 BF 0B D3 B0 A1 .....`..........
00006110 38 C0 00 58 00 C0 81 91 82 40 00 B0 00 D2 A1 A1 8..X.....@......
00006120 00 00 82 0B F8 60 B2 10 E3 7F E2 BF 0B D3 B0 A1 .....`..........
00006130 E6 7D FF 59 A1 41 90 91 00 D3 A0 A1 00 C0 81 91 .}.Y.A..........
00006140 82 40 00 B0 00 D2 A1 A1 00 00 82 0B F8 60 B2 10 .@...........`..
00006150 E3 7F E2 BF 0B D3 B0 A1 D2 7D FF 59 A1 41 90 91 .........}.Y.A..
00006160 00 CB A0 A1 00 C0 81 91 82 40 00 B0 00 D2 A1 A1 .........@......
00006170 00 00 82 0B F8 60 B2 10 E3 7F E2 BF 0B D3 B0 A1 .....`..........
00006180 A1 C1 90 91 02 C0 41 84 BA 7D FF 59 00 C2 C0 A1 ......A..}.Y....
00006190 01 40 00 78 82 41 90 91 2C C0 C2 26 DA C8 00 5A .@.x.A..,..&...Z
000061A0 2A 40 00 10 81 C0 90 47 AA 7D FF 59 00 40 21 F3 *@.....G.}.Y.@!.
000061B0 07 D2 B0 A1 0E 40 20 5C 80 42 10 B0 A1 C0 90 41 .....@ \.B.....A
000061C0 24 40 01 B0 01 41 03 F3 05 D4 B0 A1 20 C0 01 3C $@...A...... ..<
000061D0 00 50 02 F5 04 C0 80 91 02 C4 03 F3 9A 4F 00 5A .P...........O.Z
000061E0 04 C2 90 A1 8C FD FF 59 7C CA 00 5A 88 7D FF 59 .......Y|..Z.}.Y
000061F0 07 C0 90 A1 84 7D FF 59 80 41 90 91 07 D2 B0 A1 .....}.Y.A......

;; _Putstr: 00006200
;;   Called from:
;;     00005EC8 (in _Putfld)
;;     00005EC8 (in _Putfld)
_Putstr proc
	{ call __save_r16_through_r23; allocframe(#0x70) }
	{ r17 = r0; r18 = r1; r23:r22 = combine(#0x0,#0x0) }
	{ call _Getmbcurmax; r20 = memw(r17+16); r19 = memw(r17+24) }
	{ r16 = add(r29,#0x10); r22 = #0x40; r19 = add(r19,r0); memd(r29) = r23:r22 }
	{ r1 = memb(r17+60) }
	{ r1 = and(r1,#0x4) }
	{ r1 = #0x41 }
	{ call malloc; r21 = #0xFFFFFFFF; r0 = r19; r22 = r19 }
	{ r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 00006310 }

l00006250:
	{ immext(#0x7FFFFFC0); r0 = #0x7FFFFFFF }
	{ r23 = mux(p0,r20,r0) }
	{ r19 = #0x0; r21 = #-0x1 }
	{ call _Wctomb; r2 = add(r29,#0x0); r0 = add(r29,#0x8); r1 = memw(r18) }
	{ r20 = r0; if (!cmp.gt(r20.new,#-0x1)) jump:nt 00006300 }

l0000627C:
	{ if (!cmp.eq(r0.new,#0x0)) jump:t 0000628C }

l00006284:
	{ if (!cmp.gt(r20.new,#-0x1)) jump:nt 00006304 }

l0000628C:
	{ if (!p0.new) r0 = memw(r17+20) }
	{ r1 = add(r0,r20) }
	{ call _Puttxt; r1:r0 = combine(r16,r17); memw(r17+56) = #0x0 }
	{ r19 = r0 }
	{ r0 = #0x0; memw(r17+20) = #0x0 }
	{ call memcpy; r2 = r20; r1 = add(r29,#0x8); r0 = add(r0,r16) }
	{ r0 = memw(r17+20) }
	{ r0 = add(r0,r20); memb(r17+5) = r0.new }
	{ r0 = memw(r18); if (cmp.eq(r0.new,#0x0)) jump:nt 000062E0 }

l000062D8:
	{  }
	{ p0 = cmp.eq(r11,#0x0); if (!p0.new) jump:t 00006300; r21 = r19 }

l000062E0:
	{ r21 = r19 }

l000062E4:
	{ call _Puttxt; r1:r0 = combine(r16,r17) }
	{ r21 = r0 }
	{ p0 = cmp.eq(r21,#0x0); if (p0.new) r21 = #0x0; if (p0.new) memw(r17+56) = #0x0; if (p0.new) memw(r17+20) = #0x0 }

l00006300:
	{ r0 = add(r29,#0x10); if (cmp.eq(r0.new,r16)) jump:nt 00006310 }

l00006304:
	{ if (cmp.eq(r0.new,r16)) jump:nt 00006314 }

l0000630C:
	{ r0 = r16 }

l00006310:
	{ immext(#0x4000); r0 = r13; jump fn000063D0 }

l00006314:
	{ r0 = r13; jump fn000063D4 }
00006318                         F4 CD 3B 17 00 C0 00 7F         ..;.....

;; _Puttxt: 00006320
;;   Called from:
;;     00005A94 (in _Printf)
;;     00006298 (in _Putstr)
;;     000062E4 (in _Putstr)
_Puttxt proc
	{ call __save_r16_through_r23; allocframe(#0x20) }
	{ r17:r16 = combine(r1,r0) }
	{ r3 = memw(r16+24); r2 = memw(r16+20) }
	{ r3 = sub(r3,r2); r0 = memw(r16+24); r4 = memw(r16+28) }
	{ r5 = sub(r3,r0); r3 = memw(r16); r0 = memw(r16+4) }
	{ r4 = sub(r5,r4); r5 = memuh(r16+60) }
	{ r4 = sub(r4,r3); r3 = memw(r16+40) }
	{ r0 = sub(r4,r0) }
	{ r20 = sub(r0,r3); r5 = and(r5,#0x4) }
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:t 000063A4 }

l00006364:
	{ r21 = #0x20; r22 = r20 }
	{ r19 = minu(r22,r21) }

l0000636C:
	{  }
	{ immext(#0xD140); r1 = #0xD140; r0 = memw(r16+12); r3 = memw(r16+8) }
	{ callr r3; r2 = r19; r18 = #-0x1 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000654C; memw(r16+12) = r0 }

l0000638C:
	{ r0 = memw(r16+52) }
	{ r0 = add(r0,r19); memb(r16+13) = r0.new }
	{ if (cmp.gt(r22.new,#0x0)) jump:t 0000636C }

l000063A4:
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 000063CC }

l000063A8:
	{ r1 = r17; r18 = #0xFFFFFFFF; r0 = memw(r16+12); r3 = memw(r16+8) }
	{ callr r3 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000654C; memw(r16+12) = r0 }

l000063C0:
	{ r0 = memw(r16+20); r1 = memw(r16+20) }
	{ r0 = add(r0,r1); memb(r16+13) = r0.new }

l000063CC:
	{ r21 = #0x20; r19 = memw(r16+24) }

;; fn000063D0: 000063D0
;;   Called from:
;;     00006310 (in _Putstr)
;;     000063CC (in _Puttxt)
fn000063D0 proc
	{ r19 = memw(r16+24) }

;; fn000063D4: 000063D4
;;   Called from:
;;     00006314 (in _Putstr)
fn000063D4 proc
	{  }

;; fn000063D8: 000063D8
;;   Called from:
;;     00006380 (in _Puttxt)
;;     000063B8 (in _Puttxt)
;;     000063CC (in _Puttxt)
;;     000063D4 (in fn000063D4)
;;     000063D4 (in fn000063D4)
;;     000063D4 (in fn000063D4)
fn000063D8 proc
	{ r17 = minu(r19,r21) }
	{ immext(#0xD140); r1 = #0xD168; r0 = memw(r16+12); r3 = memw(r16+8) }
	{ callr r3; r2 = r17; r18 = #-0x1 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000654C; memw(r16+12) = r0 }

l000063FC:
	{ r0 = memw(r16+52) }
	{ r0 = add(r0,r17); memb(r16+13) = r0.new }
	{  }
	{ r2 = memw(r16+28); if (!cmp.gt(r2.new,#0x0)) jump:nt 0000643C }

l0000641C:
	{ r0 = memw(r16+12); r1 = memw(r16+16) }
	{ r3 = memw(r16+8) }
	{ callr r3 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000654C; memw(r16+12) = r0 }

l00006430:
	{ r0 = memw(r16+20); r1 = memw(r16+28) }
	{ r0 = add(r0,r1); memb(r16+13) = r0.new }

l0000643C:
	{ r21 = #0x20; r19 = memw(r16+32) }

l00006440:
	{ r19 = memw(r16+32) }

l00006448:
	{ r17 = minu(r19,r21) }
	{ immext(#0xD140); r1 = #0xD168; r0 = memw(r16+12); r3 = memw(r16+8) }
	{ callr r3; r2 = r17; r18 = #-0x1 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000654C; memw(r16+12) = r0 }

l0000646C:
	{ r0 = memw(r16+52) }
	{ r0 = add(r0,r17); memb(r16+13) = r0.new }
	{  }
	{ r2 = memw(r16+36); if (!cmp.gt(r2.new,#0x0)) jump:nt 000064B0 }

l0000648C:
	{ r1 = memw(r16+16); r3 = memw(r16+28) }
	{ r1 = add(r1,r3); r0 = memw(r16+12); r3 = memw(r16+8) }
	{ callr r3 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000654C; memw(r16+12) = r0 }

l000064A4:
	{ r0 = memw(r16+20); r1 = memw(r16+4) }
	{ r0 = add(r0,r1); memb(r16+13) = r0.new }

l000064B0:
	{ r21 = #0x20; nop; r19 = memw(r16+40) }

l000064B4:
	{ nop; r19 = memw(r16+40) }

l000064C0:
	{ r17 = minu(r19,r21) }
	{ immext(#0xD140); r1 = #0xD168; r0 = memw(r16+12); r3 = memw(r16+8) }
	{ callr r3; r2 = r17; r18 = #-0x1 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000654C; memw(r16+12) = r0 }

l000064E4:
	{ r0 = memw(r16+52) }
	{ r0 = add(r0,r17); memb(r16+13) = r0.new }
	{  }
	{ p0 = cmp.gt(r20,#0x0); r0 = memb(r16+60) }
	{ r0 = and(r0,#0x4); if (cmp.eq(r0.new,#0x0)) jump:t 00006548 }

l0000650C:
	{ if (p0) r19 = #0x20 }
	{ r17 = minu(r20,r19) }

l00006514:
	{  }
	{ immext(#0xD140); r1 = #0xD140; r0 = memw(r16+12); r3 = memw(r16+8) }
	{ callr r3; r2 = r17; r18 = #-0x1 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000654C; memw(r16+12) = r0 }

l00006534:
	{ r0 = memw(r16+52) }
	{ r0 = add(r0,r17); memb(r16+13) = r0.new }
	{ if (cmp.gt(r20.new,#0x0)) jump:t 00006514 }

l00006548:
	{ r18 = #0x0 }

l0000654C:
	{ immext(#0x3DC0); r0 = r10; jump fn0000661C }
00006554             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; _Tls_get__Mbcurmax: 00006560
;;   Called from:
;;     000066B0 (in _Getmbcurmax)
_Tls_get__Mbcurmax proc
	{ r1 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ immext(#0x10000); r16 = #0x10014; memd(r29) = r19:r18 }
	{ immext(#0x10000); r18 = #0x10018 }

l00006578:
	{ r0 = memw_locked(r16) }
	{ p0 = cmp.gt(r0,#0x0) }
	{ if (p0) jump:nt 0000658C }

l00006584:
	{ memw_locked(r16,p0) = r1 }
	{ if (!p0) jump:nt 00006578 }

l0000658C:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 000065B0; if (p0.new) r17 = #0x2 }

l00006594:
	{ immext(#0x10000); r0 = #0x10018; immext(#0x6FC0); r1 = #0x6FF0 }
	{ call sys_Tlsalloc }
	{ nop; memw(r16) = r17 }

l000065B0:
	{ r0 = memw(r16); if (!cmp.gt(r0.new,#0x1)) jump:t 000065B0 }

l000065BC:
	{ r0 = memw(r18) }
	{ r1:r0 = combine(#0x1,#0x1); r16 = r0; if (!cmp.eq(r16.new,#0x0)) jump:t 000065F8 }

l000065D0:
	{ r17:r16 = combine(r0,#0x0) }
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt 000065F8 }

l000065D8:
	{ call sys_Tlsset; r1 = r17; r0 = memw(r18) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 000065F4; if (p0.new) r16 = add(r17,#0x0); if (!p0.new) r0 = add(r17,#0x0) }

l000065EC:
	{ call free }
	{ jump 000065F8 }

l000065F4:
	{ memb(r17) = #0x6 }

l000065F8:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; _Tls_get__Mbstate: 00006604
;;   Called from:
;;     000066BC (in _Getpmbstate)
;;     00006DF8 (in _Wctomb)
;;     00008F40 (in _Mbtowc)
_Tls_get__Mbstate proc
	{ r1 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ immext(#0x10000); r16 = #0x1001C; immext(#0x10000); r17 = #0x10020 }

;; fn0000661C: 0000661C
;;   Called from:
;;     0000654C (in fn000063D8)
;;     0000660C (in _Tls_get__Mbstate)
fn0000661C proc
	{ memd(r29) = r19:r18 }

;; fn00006620: 00006620
;;   Called from:
;;     0000661C (in fn0000661C)
;;     0000661C (in fn0000661C)
fn00006620 proc
	{ r0 = memw_locked(r16) }
	{ p0 = cmp.gt(r0,#0x0) }
	{ if (p0) jump:nt 00006634 }

l0000662C:
	{ memw_locked(r16,p0) = r1 }
	{ if (!p0) jump:nt fn00006620 }

l00006634:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 00006654; if (p0.new) r18 = #0x2 }

l0000663C:
	{ immext(#0x10000); r0 = #0x10020; immext(#0x6FC0); r1 = #0x6FF0 }
	{ call sys_Tlsalloc }
	{ memw(r16) = r18 }

l00006654:
	{ r0 = memw(r16); if (!cmp.gt(r0.new,#0x1)) jump:t 00006654 }

l00006660:
	{ r0 = memw(r17) }
	{ r1:r0 = combine(#0x40,#0x1); r16 = r0; if (!cmp.eq(r16.new,#0x0)) jump:t 000066A4 }

l00006674:
	{ r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 000066A4 }

l00006680:
	{ r1 = r16; r0 = memw(r17) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00006694 }

l00006688:
	{ call free; r16 = #0x0; r0 = r16 }
	{ jump 000066A4 }

l00006694:
	{ call __hexagon_memcpy_likely_aligned_min32bytes_mult8bytes; immext(#0xD180); r1:r0 = combine(#0xD18C,r16); r2 = #0x40 }

l000066A4:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; _Getmbcurmax: 000066B0
;;   Called from:
;;     00006210 (in _Putstr)
_Getmbcurmax proc
	{ call _Tls_get__Mbcurmax; allocframe(#0x0) }
	{ r0 = memb(r0); dealloc_return }

;; _Getpmbstate: 000066BC
_Getpmbstate proc
	{ jump _Tls_get__Mbstate }

;; _Stoulx: 000066C0
;;   Called from:
;;     00006874 (in _Stoul)
;;     00006880 (in _Stoul)
;;     00006894 (in _Stoul)
_Stoulx proc
	{ call __save_r16_through_r27; p0 = cmp.eq(r3,#0x0); allocframe(#0x40) }
	{ r4 = p0; r19:r18 = combine(r2,r0); r16 = r1; memw(r29+8) = r3 }
	{ r22 = add(r18,#0xFFFFFFFF); if (!p0) memw(r3) = #0x0; memw(r29+4) = r4 }

l000066E0:
	{ call _Getpctype; r17 = memb(r22+1) }
	{ r22 = add(r22,#0x1); r0 = memuh(r13+r17<<#1) }
	{ r0 = and(r0,#0x144); if (!cmp.eq(r0.new,#0x0)) jump:t 000066E0 }

l000066FC:
	{ r0 = memb(r1++#1) }
	{ if (!p0.new) jump:t 00006714; if (!p0.new) r21 = #0x2B; p0 = cmp.eq(r0,#0x2B) }

l0000670C:
	{ r13 = r0; jump 00006720; r22 = r1 }

l00006714:
	{ p0 = cmp.eq(r0,#0x2D); if (p0.new) r22 = add(r1,#0x0) }
	{ if (p0) r21 = add(r0,#0x0) }

l00006720:
	{ p0 = cmp.gt(r11,#-0x1); if (p0.new) jump:t 00006730 }

l00006724:
	{ p0 = cmp.eq(r11,#0x1); if (p0.new) jump:t 00006730 }

l00006728:
	{ if (!p0.new) jump:t 00006740; p0 = cmp.gt(r19,#0x24) }

l00006730:
	{ p0 = cmp.eq(r8,#0x0); if (p0.new) jump:nt 00006890; r0 = #0x0 }

l00006738:
	{ jump __restore_r16_through_r27_and_deallocframe; memw(r16) = r18 }

l00006740:
	{ p0 = cmp.gt(r11,#0x0); if (!p0.new) jump:nt 00006768 }

l00006744:
	{ p0 = cmp.eq(r11,#0x10); if (!p0.new) jump:t 00006788 }

l00006748:
	{ r19 = #0x10; r0 = memb(r22) }
	{ if (!p0.new) jump:t 00006788; p0 = cmp.eq(r0,#0x30); if (p0.new) r0 = memb(r22+1) }

l00006758:
	{ r0 = setbit(r0,#0xA) }
	{ p0 = cmpb.eq(r0,#0x78); jump 00006788; if (p0.new) r22 = add(r22,#0x2) }

l00006768:
	{ r19 = #0xA; r0 = memb(r22) }
	{ if (!p0.new) jump:t 00006788; p0 = cmp.eq(r0,#0x30) }

l00006774:
	{ r19 = #0x8; r0 = memb(r22+1) }
	{ r0 = setbit(r0,#0xA) }
	{ p0 = cmpb.eq(r0,#0x78); if (p0.new) r22 = add(r22,#0x2); if (p0.new) r19 = #0x10 }

l00006788:
	{ jump 00006794; r25 = r22 }

l00006790:
	{ r25 = add(r25,#0x1) }

l00006794:
	{ r17 = memb(r25) }
	{ if (p0.new) jump:t 00006790; p0 = cmp.eq(r17,#0x30) }

l000067A0:
	{ call _Getptolower }
	{ immext(#0xD1C0); r1:r0 = combine(r0,#0xD1D0); r2 = r19 }
	{ call memchr; immext(#0xD1C0); r27:r26 = combine(#0xD1D0,#0x0); r1 = memh(r29+r17<<#1) }
	{ r24 = r25; r20 = r0; nop; if (cmp.eq(r20.new,#0x0)) jump:nt 0000680C }

l000067D0:
	{ call _Getptolower; r17 = memb(r24+1) }

l000067D4:
	{ r17 = memb(r24+1) }

l000067D8:
	{ immext(#0xD1C0); r1:r0 = combine(r0,#0xD1D0); r3 = sub(r20,r27); r2 = r19 }
	{ r24 = add(r24,#0x1); r17 = r26; r23 = and(r3,#0xFF); r1 = memh(r29+r17<<#1) }
	{ call memchr; r26 = add(r23,mpyi(r26,r19)) }
	{ r20 = r0; if (!cmp.eq(r20.new,#0x0)) jump:t 000067D0 }

l0000680C:
	{ r23 = r26; r24 = r25 }
	{ if (p0.new) jump:nt 00006880; if (!p0.new) r0 = sub(r24,r25); p0 = cmp.eq(r22,r24) }

l00006820:
	{ immext(#0xD1C0); r1 = memb(r19+0000D1F8) }
	{ r0 = sub(r0,r1); if (!cmp.gt(r0.new,#-0x1)) jump:nt 00006868 }

l00006834:
	{ if (!p0.new) r1 = add(r19,#0x0) }
	{ r0 = sub(r26,r23); if (cmp.gtu(r0.new,r26)) jump:t 00006848 }

l00006844:
	{ p0 = cmp.eq(r0,r9); if (p0.new) jump:t 00006868 }

l00006848:
	{ call _Geterrno }
	{ r26 = #0xFFFFFFFF; r21 = #0x2B; r0 = memw(r29+4); memw(r0) = #0x22 }
	{ p0 = r0; if (!p0.new) r0 = memw(r29+8) }
	{ if (!p0) memw(r0) = #0x1 }

l00006868:
	{ p0 = cmp.eq(r21,#0x2D); r0 = sub(#0x0,r26); p1 = cmp.eq(r16,#0x0) }
	{ jump 00006890; if (!p0) r0 = add(r26,#0x0) }

l00006880:
	{ p0 = cmp.eq(r8,#0x0); if (p0.new) jump:nt 00006890; r0 = #0x0 }

l00006888:
	{ jump __restore_r16_through_r27_and_deallocframe; memw(r16) = r18 }

l00006890:
	{ jump __restore_r16_through_r27_and_deallocframe }

;; _Stoul: 00006894
;;   Called from:
;;     000051F0 (in atoi)
_Stoul proc
	{ r0 = #0x0; jump _Stoulx }
00006898                         00 C0 00 7F 00 C0 00 7F         ........

;; _Clearlocks: 000068A0
_Clearlocks proc
	{ r0 = #0x0; nop; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ immext(#0xE8C0); r17:r16 = combine(#0xE8D4,#0x20); immext(#0x10000); memb(gp+65540) = r0 }
	{ nop }

l000068C0:
	{ call __sys_Mtxdst; r17 = add(r17,#0xFFFFFFFF); r16 = add(r16,#0x4); r0 = r16 }
	{ p0 = cmp.eq(r9,#0x0); if (!p0.new) jump:t 000068C0 }

l000068D0:
	{ r0 = #0x0; immext(#0xE900); r17:r16 = combine(#0xE904,#0x30) }
	{ immext(#0x10000); memb(gp+65540) = r0 }

l000068E4:
	{ call __sys_Mtxdst; r17 = add(r17,#0xFFFFFFFF); r16 = add(r16,#0x4); r0 = r16 }
	{ p0 = cmp.eq(r9,#0x0); if (!p0.new) jump:t 000068E4 }

l000068F4:
	{ nop; nop; r17:r16 = memd(r29); dealloc_return }

;; _Initlocks: 00006900
_Initlocks proc
	{ immext(#0xE8C0); r17:r16 = combine(#0xE8D4,#0x20); nop; memd(r29-16) = r17:r16; allocframe(#0x8) }

l00006910:
	{ call __sys_Mtxinit; r17 = add(r17,#0xFFFFFFFF); r16 = add(r16,#0x4); r0 = r16 }
	{ p0 = cmp.eq(r9,#0x0); if (!p0.new) jump:t 00006910 }

l00006920:
	{ r0 = #0x1; immext(#0xE900); r17:r16 = combine(#0xE904,#0x30) }
	{ immext(#0x10000); memb(gp+65540) = r0 }

l00006934:
	{ call __sys_Mtxinit; r17 = add(r17,#0xFFFFFFFF); r16 = add(r16,#0x4); r0 = r16 }
	{ p0 = cmp.eq(r9,#0x0); if (!p0.new) jump:t 00006934 }

l00006944:
	{ r0 = #0x1; r17:r16 = memd(r29); immext(#0x10000) }

;; _Lockfilelock: 00006958
;;   Called from:
;;     00005328 (in printf)
;;     00006944 (in _Initlocks)
;;     000070E0 (in fwrite)
;;     00007388 (in puts)
;;     00009214 (in fflush)
;;     000092E8 (in fputc)
;;     00009368 (in fputs)
_Lockfilelock proc
	{ call __save_r16_through_r21; allocframe(#0x18) }
	{ r16 = r0; immext(#0x10000); r20 = #0x1002C; r1 = #0x1 }
	{ immext(#0xE8C0); r18 = #0xE8E0; immext(#0x10000); r19 = #0x10024 }

l0000697C:
	{ r0 = memw_locked(r20) }
	{ p0 = cmp.gt(r0,#0x0) }
	{ if (p0) jump:nt 00006990 }

l00006988:
	{ memw_locked(r20,p0) = r1 }
	{ if (!p0) jump:nt 0000697C }

l00006990:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 000069E4 }

l00006994:
	{ nop; immext(#0xE8C0); r17 = #0xE8E0; r21 = #0x14 }

l000069A0:
	{ call __sys_Mtxinit; r21 = add(r21,#0xFFFFFFFF); r17 = add(r17,#0x4); r0 = r17 }
	{ p0 = cmp.eq(r13,#0x0); if (!p0.new) jump:t 000069A0 }

l000069B0:
	{ immext(#0xE900); r17 = #0xE930; r0 = #0x1; r21 = #0x4 }
	{ memb(r19) = r0 }

l000069C0:
	{ call __sys_Mtxinit; r21 = add(r21,#0xFFFFFFFF); r17 = add(r17,#0x4); r0 = r17 }
	{ p0 = cmp.eq(r13,#0x0); if (!p0.new) jump:t 000069C0 }

l000069D0:
	{ r0 = #0x1; r1 = #0x2; immext(#0x10000); memb(gp+65540) = r1.new }

l000069E4:
	{ r0 = memw(r20); if (!cmp.gt(r0.new,#0x1)) jump:t 000069E4 }

l000069F0:
	{ r0 = memb(r16+2); r1 = memb(r19) }
	{ p0 = r1 }
	{ r0 = addasl(r18,r0,#0x2); r1 = mux(p0,#0x14,#0x0) }
	{ call lockMutex }
	{ jump __restore_r16_through_r21_and_deallocframe; nop }

;; _Unlockfilelock: 00006A10
;;   Called from:
;;     00005344 (in printf)
;;     000071F0 (in fwrite)
;;     000073CC (in puts)
;;     000092B4 (in fflush)
;;     00009354 (in fputc)
;;     00009454 (in fputs)
_Unlockfilelock proc
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00006A40; allocframe(#0x0) }

l00006A18:
	{ immext(#0x10000); r1 = memb(gp+65572); r0 = memb(r0+2) }
	{ p0 = r1 }
	{ r1 = mux(p0,#0x14,#0x0) }
	{ immext(#0xE8C0); r1 = #0xE8E0 }
	{ r0 = addasl(r1,r0,#0x2); call __sys_Mtxunlock }

l00006A40:
	{ dealloc_return }

;; _Locksyslock: 00006A44
;;   Called from:
;;     00006F2C (in atexit)
;;     00007230 (in malloc)
;;     000078A0 (in signal)
;;     000090EC (in _Closreg)
;;     00009164 (in fclose)
;;     000091D0 (in fclose)
;;     0000926C (in fflush)
_Locksyslock proc
	{ call __save_r16_through_r21; allocframe(#0x18) }
	{ r16 = r0; immext(#0x10000); r20 = #0x1002C; r1 = #0x1 }
	{ immext(#0xE900); r18 = #0xE930; immext(#0x10000); r19 = #0x10028 }

l00006A68:
	{ r0 = memw_locked(r20) }
	{ p0 = cmp.gt(r0,#0x0) }
	{ if (p0) jump:nt 00006A7C }

l00006A74:
	{ memw_locked(r20,p0) = r1 }
	{ if (!p0) jump:nt 00006A68 }

l00006A7C:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 00006AD4 }

l00006A80:
	{ nop; nop; immext(#0xE8C0); r17 = #0xE8E0; r21 = #0x14 }

l00006A90:
	{ call __sys_Mtxinit; r21 = add(r21,#0xFFFFFFFF); r17 = add(r17,#0x4); r0 = r17 }
	{ p0 = cmp.eq(r13,#0x0); if (!p0.new) jump:t 00006A90 }

l00006AA0:
	{ immext(#0xE900); r17 = #0xE930; r0 = #0x1; r21 = #0x4 }
	{ immext(#0x10000); memb(gp+65540) = r0 }

l00006AB4:
	{ call __sys_Mtxinit; r21 = add(r21,#0xFFFFFFFF); r17 = add(r17,#0x4); r0 = r17 }
	{ p0 = cmp.eq(r13,#0x0); if (!p0.new) jump:t 00006AB4 }

l00006AC4:
	{ r0 = #0x1; r1 = #0x2; memb(r19) = r0.new }

l00006AD4:
	{ r0 = memw(r20); if (!cmp.gt(r0.new,#0x1)) jump:t 00006AD4 }

l00006AE0:
	{ p0 = r0 }
	{ r0 = mux(p0,#0x4,#0x0); if (!cmp.gt(r0.new,r16)) jump:t 00006AF4 }

l00006AF0:
	{ call fn0000AEF4 }

l00006AF4:
	{ jump __restore_r16_through_r21_and_deallocframe }

;; _Unlocksyslock: 00006AF8
;;   Called from:
;;     00006F60 (in atexit)
;;     00007094 (in free)
;;     00007340 (in malloc)
;;     000078B0 (in signal)
;;     000090FC (in _Closreg)
;;     000091C4 (in fclose)
;;     000091E0 (in fclose)
_Unlocksyslock proc
	{ allocframe(#0x0) }
	{ immext(#0x10000); r1 = memb(gp+65576) }
	{ p0 = r1 }
	{ r1 = mux(p0,#0x4,#0x0) }
	{ immext(#0xE900); r1 = #0xE930 }
	{ r0 = addasl(r1,r0,#0x2); call __sys_Mtxunlock }
	{ dealloc_return }
00006B24             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; _Tls_get__Tolotab: 00006B30
;;   Called from:
;;     00006BD8 (in _Getptolower)
_Tls_get__Tolotab proc
	{ r1 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ immext(#0x10000); r16 = #0x10030; immext(#0x10000); r17 = #0x10034 }
	{ memd(r29) = r19:r18 }

l00006B4C:
	{ r0 = memw_locked(r16) }
	{ p0 = cmp.gt(r0,#0x0) }
	{ if (p0) jump:nt 00006B60 }

l00006B58:
	{ memw_locked(r16,p0) = r1 }
	{ if (!p0) jump:nt 00006B4C }

l00006B60:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 00006B80; if (p0.new) r18 = #0x2 }

l00006B68:
	{ immext(#0x10000); r0 = #0x10034; immext(#0x6FC0); r1 = #0x6FF0 }
	{ call sys_Tlsalloc }
	{ memw(r16) = r18 }

l00006B80:
	{ r0 = memw(r16); if (!cmp.gt(r0.new,#0x1)) jump:t 00006B80 }

l00006B8C:
	{ r0 = memw(r17) }
	{ r1:r0 = combine(#0x4,#0x1); r16 = r0; if (!cmp.eq(r16.new,#0x0)) jump:t 00006BCC }

l00006BA0:
	{ r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 00006BCC }

l00006BAC:
	{ r1 = r16; r0 = memw(r17) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00006BC0 }

l00006BB4:
	{ call free; r16 = #0x0; r0 = r16 }
	{ jump 00006BCC }

l00006BC0:
	{ immext(#0xD200); r0 = #0xD222; memb(r16) = r0.new }

l00006BCC:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }

l00006BD0:
	{ r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; _Getptolower: 00006BD8
;;   Called from:
;;     000067A0 (in _Stoulx)
;;     000067D0 (in _Stoul)
_Getptolower proc
	{ call _Tls_get__Tolotab; allocframe(#0x0) }
	{ r0 = memw(r0); dealloc_return }
00006BE4             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; _Vacopy: 00006BF0
;;   Called from:
;;     00005790 (in _Printf)
;;     00005A64 (in _Printf)
_Vacopy proc
	{ r2 = #0x4; r3 = r1; allocframe(#0x8) }
	{ call memcpy; r1 = add(r29,#0x4); memw(r29+4) = r3 }
	{ dealloc_return }
00006C04             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; _Wctombx: 00006C10
;;   Called from:
;;     00006E28 (in _Wctomb)
_Wctombx proc
	{ call __save_r16_through_r23; allocframe(#0x20) }
	{ r17:r16 = combine(r1,r0); r18 = r2; r20 = r4 }
	{ p0 = cmp.eq(r16,#0x0); r5 = memw(r3) }
	{ if (p0) jump:nt 00006D24; nop }

l00006C34:
	{ r19 = #0x0; nop; r21 = #0x0; r0 = memuh(r18+6) }
	{ p0 = cmpb.gtu(r0,#0xF); if (p0.new) jump:nt 00006D10; if (!p0.new) r0 = zxtb(r0) }

l00006C4C:
	{ r22 = memw(r6+r0<<#2); if (cmp.eq(r22.new,#0x0)) jump:nt 00006D10 }

l00006C58:
	{ p0 = cmp.gt(r0,r11); if (p0.new) jump:nt 00006D10; if (p0.new) r21 = add(r21,#0x1) }

l00006C60:
	{ if (p0.new) jump:nt 00006D10; if (!p0.new) r0 = zxtb(r17); immext(#0xFC0); p0 = cmp.gt(r21,#0xFEF) }

l00006C70:
	{ r1 = memuh(r4+r0<<#1) }
	{ immext(#0x8000); r0 = and(r1,#0x8000); r3 = r17; r2 = and(r1,#0xFF) }
	{ r3 = or(r2,and(r3,#0xFFFFFF00)); immext(#0x1000); r4 = and(r1,#0x1000); p0 = cmp.eq(r0,#0x0) }
	{ p1 = cmp.eq(r4,#0x0); immext(#0x2000); r5 = and(r1,#0x2000); if (p0) r3 = add(r17,#0x0) }
	{ r0 = extractu(r1,#0x4,#0xC); r4 = asl(r3,#0x8); if (p1) r17 = add(r3,#0x0); p0 = cmp.eq(r5,#0x0) }
	{ r4 |= lsr(r3,#0x18) }
	{ if (p0) jump:nt 00006CE0; if (!p1) r17 = add(r4,#0x0) }

l00006CC0:
	{ r2 = add(r16,r19); r21 = #0x0; r19 = r19; p0 = cmp.eq(r2,#0x0) }
	{ if (p0) r3 = add(r17,#0x0); if (!p0) r3 = add(r1,#0x0) }
	{ p0 = cmpb.eq(r3,#0x0); if (p0.new) jump:nt 00006CEC; memb(r2) = r3 }

l00006CE0:
	{ immext(#0x4000); r1 = and(r1,#0x4000) }

l00006CEC:
	{ immext(#0x3640); r0 = r11; jump 00006D3C; memuh(r18+6) = r0 }
00006CF8                         24 C0 08 10 00 50 00 78         $....P.x
00006D00 2A F1 42 21 00 C0 70 70 6C 40 00 58 08 D1 00 AB *.B!..ppl@.X....

l00006D10:
	{ call _Geterrno; r19 = #0xFFFFFFFF }
	{ immext(#0x3600); r0 = r11; jump 00006DB8; memw(r0) = #0x58 }

l00006D24:
	{ memw(r18+4) = #0x0; memw(r18) = #0x0 }
	{ r0 = memw(r3) }
	{ r0 = memuh(r0) }
	{ immext(#0xF00); r19 = and(r0,#0xF00) }
	{ immext(#0x3600); r0 = r11; jump 00006D58 }

l00006D3C:
	{ r0 = r11; jump 00006D5C }
00006D40 13 40 00 78 A1 10 A0 F0 D7 40 00 00 70 C0 0B 17 .@.x.....@..p...
00006D50 20 4B 11 8C 0A E0 42 24                          K....B$        

l00006D58:
	{ r1:r0 = combine(r17,#0x1) }

l00006D5C:
	{ r1 = add(#0xC2,asl(r1,#0xC)); jump 00006DA8 }
00006D64             20 50 11 8C 0A E0 42 24 40 E0 11 73      P....B$@..s
00006D70 12 4C E1 DE 1C C0 00 58 20 55 11 8C 0A E0 42 24 .L.....X U....B$
00006D80 60 E0 11 73 12 72 E1 DE 12 C0 00 58 20 5A 11 8C `..s.r.....X Z..
00006D90 0A E0 42 24 80 E0 11 73 92 78 E1 DE 08 C0 00 58 ..B$...s.x.....X
00006DA0 A0 E0 11 73 D2 FE E1 DE                         ...s....        

l00006DA8:
	{ r3 = sub(#0x0,r0); r1 = #0xFFFFFFFA; r4 = #-0x1; memb(r16) = r1 }
	{ r3 = max(r3,r4); r1 += mpyi(r0,#0x6); r2 = add(r16,#0x1) }

l00006DB8:
	{ r1 += mpyi(r0,#0x6); r2 = add(r16,#0x1) }
	{ loop0(00006DC8,r0); r3 += add(r0,#0x2) }
	{ r0 &= asr(r17,r1); r1 = add(r1,#0xFFFFFFFA) }
	{ r0 = and(r0,#0x3F) }
	{ r0 = setbit(r0,#0xE); memb(r2++#1) = r0.new }
	{ r19 = sub(r0,r16) }
	{ immext(#0x3540); r0 = r11; jump fn00006E54; nop }

;; _Wctomb: 00006DF0
;;   Called from:
;;     00006264 (in _Putstr)
_Wctomb proc
	{ r17:r16 = combine(r2,r0); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ call _Tls_get__Mbstate; r18 = r1; memd(r29) = r19:r18 }
	{ p0 = cmp.eq(r16,#0x0); r4 = #0x0; r19 = r0 }
	{ if (p0) jump:nt 00006E1C }

l00006E14:
	{ call _Tls_get__Wcstate }
	{ r4 = r0 }

l00006E1C:
	{ r3:r2 = combine(r19,r17); r1:r0 = combine(r18,r16); r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ jump _Wctombx; deallocframe }

;; abort: 00006E30
;;   Called from:
;;     000056D8 (in __registerx)
;;     00005728 (in __register_frame_info_bases)
;;     000095C4 (in _Atexit)
abort proc
	{ call raise; r0 = #0x6; allocframe(#0x0) }
	{ call exit; r0 = #0x1 }

;; calloc: 00006E40
;;   Called from:
;;     00006E38 (in abort)
calloc proc
	{ r17 = mpyi(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ call malloc; r0 = r17 }
	{ r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 00006E60 }

;; fn00006E54: 00006E54
;;   Called from:
;;     00006DE4 (in _Wctombx)
fn00006E54 proc
	{ if (cmp.eq(r16.new,#0x0)) jump:nt fn00006E64 }

;; fn00006E5C: 00006E5C
;;   Called from:
;;     00006E54 (in fn00006E54)
;;     00006E54 (in fn00006E54)
;;     00006E54 (in fn00006E54)
;;     00006E54 (in fn00006E54)
fn00006E5C proc
	{ r2 = r17; r1:r0 = combine(#0x0,#0x0) }

l00006E60:
	{ r0 = r16; r17:r16 = memd(r29); dealloc_return }

;; fn00006E64: 00006E64
;;   Called from:
;;     00006E54 (in fn00006E54)
fn00006E64 proc
	{ r17:r16 = memd(r29); dealloc_return }
00006E68                         00 C0 00 7F 00 C0 00 7F         ........

;; _Tls_get__Errno: 00006E70
;;   Called from:
;;     00006F10 (in _Geterrno)
_Tls_get__Errno proc
	{ r1 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ immext(#0x10000); r16 = #0x10038; immext(#0x10000); r17 = #0x1003C }
	{ memd(r29) = r19:r18 }

l00006E8C:
	{ r0 = memw_locked(r16) }
	{ p0 = cmp.gt(r0,#0x0) }
	{ if (p0) jump:nt 00006EA0 }

l00006E98:
	{ memw_locked(r16,p0) = r1 }
	{ if (!p0) jump:nt 00006E8C }

l00006EA0:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 00006EC0; if (p0.new) r18 = #0x2 }

l00006EA8:
	{ immext(#0x10000); r0 = #0x1003C; immext(#0x6FC0); r1 = #0x6FF0 }
	{ call sys_Tlsalloc }
	{ memw(r16) = r18 }

l00006EC0:
	{ r0 = memw(r16); if (!cmp.gt(r0.new,#0x1)) jump:t 00006EC0 }

l00006ECC:
	{ r0 = memw(r17) }
	{ r1:r0 = combine(#0x4,#0x1); r16 = r0; if (!cmp.eq(r16.new,#0x0)) jump:t 00006F04 }

l00006EE0:
	{ r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 00006F04 }

l00006EEC:
	{ r1 = r16; r0 = memw(r17) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00006F04; if (p0.new) memw(r16) = #0x0 }

l00006EF8:
	{ call free; r16 = #0x0; r0 = r16 }
	{ jump 00006F04 }

l00006F04:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; _Geterrno: 00006F10
;;   Called from:
;;     00006848 (in _Stoul)
;;     00006D10 (in _Wctombx)
;;     00008EEC (in fn00008D4C)
;;     000091E8 (in fclose)
;;     00009698 (in _Feraise)
;;     0000AFF8 (in __sys_close)
;;     0000B004 (in __sys_close)
;;     0000B078 (in __sys_remove)
;;     0000B084 (in __sys_remove)
;;     0000B2EC (in __sys_write)
;;     0000B2F8 (in __sys_write)
_Geterrno proc
	{ jump _Tls_get__Errno }
00006F14             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; atexit: 00006F20
;;   Called from:
;;     000055E0 (in __libc_start_main)
atexit proc
	{ r16 = r0; r0 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ call _Locksyslock }
	{ r17 = #0xFFFFFFFF; r0 = memw(gp+12) }
	{ r1 = memw(gp+64) }
	{ call _Atrealloc }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00006F60 }

l00006F48:
	{ r0 = memw(gp+12) }
	{ r0 = add(r0,#0xFFFFFFFF); r17 = #0x0; r1 = memw(gp+8) }
	{ memw(gp) = r0; memw(r30+r0<<#2) = r16 }

l00006F60:
	{ call _Unlocksyslock; r0 = #0x1 }
	{ r0 = r17; r17:r16 = memd(r29); dealloc_return }

;; exit: 00006F70
;;   Called from:
;;     000055F8 (in __libc_start_main)
;;     00006E38 (in abort)
;;     00007870 (in raise)
exit proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r2 = memw(gp+4) }
	{ r1 = memw(gp+12) }

l00006F84:
	{ r2 = add(r1,#0x1); r0 = memw(gp+8) }
	{ r0 = memw(r6+r1<<#2); memw(gp+64) = r2 }
	{ callr r0 }
	{ r1 = memw(gp+12) }
	{ r0 = memw(gp+4); if (cmp.gtu(r0.new,r1)) jump:t 00006F84 }

l00006FA8:
	{ r0 = add(r0,#0xFFFFFFFF); r1 = memw(gp+8) }
	{ r1 = memw(r22+r0<<#2); memw(gp) = r0 }
	{ callr r1 }
	{ immext(#0xE940); r17 = #0xE940; r0 = memw(gp+64); if (!cmp.eq(r0.new,#0x0)) jump:t 00006FA8 }

l00006FD0:
	{ if (cmp.eq(r0.new,r17)) jump:t 00006FDC }

l00006FD8:
	{ r1:r0 = combine(#0x50,r16); memw(gp+544) = r17 }

l00006FDC:
	{ memw(gp+544) = r17 }

l00006FE0:
	{ call _Exit; memw(gp+32) = r1 }
	{ nop }
	{ nop }

;; free: 00006FF0
;;   Called from:
;;     0000575C (in __deregister_frame_info_bases)
;;     000065EC (in _Tls_get__Mbcurmax)
;;     00006688 (in fn00006620)
;;     00006BB4 (in _Tls_get__Tolotab)
;;     00006EF8 (in _Tls_get__Errno)
;;     00006FEC (in exit)
;;     00007214 (in dkw_malloc_init)
;;     00007974 (in _Tls_get__Ctype)
;;     00008FF4 (in _Tls_get__Wcstate)
;;     00009090 (in _Atrealloc)
;;     000091A8 (in fclose)
;;     00009758 (in _Fofree)
;;     00009CC4 (in _Tls_get__Locale)
free proc
	{ memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 0000709C }

l00007000:
	{ r1 = #0x8 }
	{ r0 = and(r0,#0x7); if (!cmp.eq(r0.new,#0x0)) jump:t 0000709C }

l00007014:
	{ r0 = #0x1 }
	{ r0 = add(r16,#0xFFFFFFF8); r1 = memw(gp+72) }
	{  }
	{ memw(r16-4) = r1; memw(gp) = r0 }
	{ r2 = memw(r0+4); if (cmp.eq(r2.new,#0x0)) jump:nt 00007094 }

l0000703C:
	{ r2 = #0x0; r3 = add(r0,r1) }
	{ memw(gp+64) = r2 }
	{ r2 = memw(r0+4) }
	{ r3 = memw(r2); r2 = memw(r2+4) }
	{ r1 = add(r1,r3); memw(r0+4) = r2 }
	{ jump 00007094; memw(r0) = r1 }
00007060 02 C0 61 70 21 C0 82 91 04 C0 01 10 FA F1 30 15 ..ap!.........0.
00007070 03 C0 82 91 04 43 02 F3 10 E0 02 21 12 50 04 14 .....C.....!.P..
00007080 1C C0 00 5C 04 40 70 70 C3 FF 90 97 04 5F 03 E2 ...\.@pp....._..
00007090 14 E1 42 21                                     ..B!            

l00007094:
	{ call _Unlocksyslock; r0 = #0x1 }

l0000709C:
	{ r17:r16 = memd(r29); dealloc_return }
000070A0 00 40 62 70 C1 FF 90 97 C4 7F FF 59 01 43 01 F3 .@bp.......Y.C..
000070B0 00 D3 A2 A1 BE 7F FF 59 FF 61 90 A7 01 C0 82 A1 .......Y.a......

;; fwrite: 000070C0
;;   Called from:
;;     0000536C (in prout)
fwrite proc
	{ call __save_r16_through_r25; allocframe(#0x38) }
	{ r17:r16 = combine(r3,r1); r0 = #0x0; r19 = r0 }
	{ r21 = mpyi(r2,r16) }
	{ p0 = cmp.eq(r8,#0x0); if (p0.new) jump:nt 00007208; r0 = r2 }

l000070E0:
	{ call _Lockfilelock; r0 = r17; r23 = r21 }

l000070E8:
	{ r18 = r23; r0 = memw(r17+16) }
	{ r1 = memw(r17+24) }
	{ call _Fwprep; r22 = #0x0; r0 = r17 }
	{ p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:nt 000071C8 }

l00007100:
	{ p0 = or(p0,!p0); r3 = r18; r2 = r18; r0 = memb(r17+1) }
	{ p0 = or(p0,!p0); p1 = or(p0,p0); r0 = and(r0,#0x4); if (cmp.eq(r0.new,#0x0)) jump:t 00007148 }

l00007120:
	{ r3 = p0; call memchr }
	{ p0 = cmp.eq(r0,#0x0); r3 = r18; r1 = memd(r29+8) }
	{ p1 = r1; if (p0) jump:nt 00007148 }

l0000713C:
	{ p0 = and(p0,p0); r3 = add(r0,sub(#0x41,r19)) }
	{ p1 = or(p0,p0) }

l00007148:
	{ r5 = p1; r1 = r19; r0 = memw(r17+16); r2 = memw(r17+24) }
	{ r4 = sub(r2,r0); memw(r29+8) = r5 }
	{ r20 = minu(r3,r4); p0 = cmp.gtu(r3,r4) }
	{ r3 = p0; r2 = r20; r23 = sub(r18,r20) }
	{ call memcpy }
	{ r0 = memw(r17+16) }
	{ r22 = add(r0,r20); r0 = memw(r29+4); memb(r17+4) = r22.new }
	{ if (p0.new) jump:t 000071B8; if (!p0.new) r0 = memw(r29+8) }

l00007194:
	{ p0 = r0; if (p0.new) jump:t 000071B4; if (!p0.new) r0 = add(r17,#0x0); if (!p0.new) r24 = memw(r17+8) }

l000071A4:
	{ call fflush }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 000071C8; if (!p0.new) r18 = add(r23,#0x0); r22 = sub(r22,r24) }

l000071B4:
	{ p0 = cmp.eq(r18,r20); r22 = #0x0; r18 = #0x0; r19 = add(r19,r20) }

l000071B8:
	{ r22 = #0x0; r18 = #0x0; r19 = add(r19,r20) }

l000071C0:
	{ if (!p0) jump:nt 000070E8 }

l000071C4:
	{ jump 000071C8 }

l000071C8:
	{ p0 = cmp.eq(r22,#0x0); r0 = memb(r17+1) }
	{ r0 = and(r0,#0x8); if (cmp.eq(r0.new,#0x0)) jump:t 000071F0 }

l000071D8:
	{ call fflush; r0 = r17; r19 = memw(r17+16); r20 = memw(r17+8) }
	{ r22 = sub(r19,r20); p0 = cmp.eq(r0,#0x0) }
	{ if (p0) r22 = #0x0 }

l000071F0:
	{ call _Unlockfilelock; r0 = r17; r17 = sub(r21,r18) }
	{ call __qdsp_udivsi3; r0 = sub(r17,r22); r1 = r16 }

l00007208:
	{ jump __restore_r16_through_r25_and_deallocframe }
0000720C                                     00 C0 00 7F             ....

;; dkw_malloc_init: 00007210
dkw_malloc_init proc
	{ r1 = add(r1,sub(#0x41,r0)) }
	{ r1 = lsr(r1,#0x3); jump free }

;; malloc: 00007220
;;   Called from:
;;     00006238 (in _Putstr)
;;     00006E48 (in calloc)
;;     00009030 (in _Atrealloc)
;;     000096FC (in _Fofind)
malloc proc
	{ r16 = r0; r0 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r1 = add(r16,#0xF) }
	{ call _Locksyslock; r17 = and(r1,#0xFFFFFFF8) }
	{  }
	{ if (p0.new) r0 = #0x8 }
	{ r17 = maxu(r0,r17); jump 00007250 }
00007248                         D4 7E FF 5B 00 C0 00 7F         .~.[....

l00007250:
	{ immext(#0x10040); r2 = #0x10048; r4 = memw(gp+68) }
	{ p0 = cmp.eq(r4,#0x0); if (!p0.new) jump:t 00007280; r5 = r4 }

l00007264:
	{ r0 = r2 }
	{ r1 = memw(r0) }
	{ r2 = r1 }
	{ r3 = memw(r2++#4) }
	{ jump 000072E0 }

l00007280:
	{ r0 = r5 }
	{ r1 = memw(r0) }
	{ r5 = r1 }
	{ r3 = memw(r5++#4) }
	{ jump 000072E0 }
0000729C                                     04 C0 84 91             ....
000072A0 00 C0 62 70 01 40 80 91 0C C4 03 20 02 C0 61 70 ..bp.@..... ..ap
000072B0 23 40 82 9B F8 F1 33 22 14 C0 00 58 80 C0 80 49 #@....3"...X...I
000072C0 90 C0 D1 D5 F6 43 00 5A 00 C0 70 70 BE 40 70 10 .....C.Z..pp.@p.
000072D0 00 E1 80 74 20 41 10 8C F6 E9 78 14 0E C0 00 58 ...t A....x....X

l000072E0:
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 000072F8 }

l000072E4:
	{ r2 = add(r3,#0xFFFFFFF8); if (!cmp.gtu(r17,r2.new)) jump:t 00007300 }

l000072F0:
	{ r2 = memw(r1+4); memb(r0) = r2.new }

l000072F8:
	{ r0 = #0x0; jump 00007340; r0 = #0x1 }

l000072FC:
	{ r0 = #0x1 }

l00007300:
	{ r2 = add(r1,r17); memb(r0) = r2.new }
	{  }
	{ r3 = memw(r1); r2 = memw(r0) }
	{ r0 = sub(r3,r17); memb(r2) = r0.new }
	{ p0 = cmp.eq(r2,#0x0); if (p0.new) jump:nt 00007334; r3 = #0x0 }

l00007328:
	{ r0 = memw(r2+4) }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) r3 = add(r2,#0x4) }

l00007334:
	{ r0 = #0x1; r16 = add(r1,#0x8); memw(gp+96) = r3 }

l00007340:
	{ call _Unlocksyslock }
	{ r0 = r16; r17:r16 = memd(r29); dealloc_return }
0000734C                                     00 C0 00 7F             ....

;; memchr: 00007350
;;   Called from:
;;     000058A0 (in _Printf)
;;     000067B0 (in _Stoulx)
;;     000067F8 (in _Stoul)
;;     00007120 (in fwrite)
memchr proc
	{ p0 = cmp.eq(r2,#0x0); if (p0.new) jump:nt 00007374; r0 = #0x0; r3 = r0 }

l00007358:
	{ r1 = and(r1,#0xFF); nop }
	{ r4 = memb(r3); if (cmp.eq(r4.new,r1)) jump:nt 00007378 }

l00007364:
	{ if (cmp.eq(r4.new,r1)) jump:nt 0000737C }

l0000736C:
	{ r2 = add(r2,#0xFFFFFFFF); if (!cmp.eq(r2.new,#0x0)) jump:t 00007364 }

l00007374:
	{ jumpr r31 }

l00007378:
	{ r0 = r3; jumpr r31 }

l0000737C:
	{ nop }

;; puts: 00007380
;;   Called from:
;;     0000737C (in memchr)
puts proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ call _Lockfilelock; immext(#0xE480); r0 = #0xE498 }
	{ call fputs; immext(#0xE480); r1:r0 = combine(#0xE498,r16) }
	{ p1 = or(p1,!p1); p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:nt 000073BC }

l000073A8:
	{ call fputc; immext(#0xE480); r1:r0 = combine(#0xE498,#0xA) }
	{ p0 = cmp.gt(r0,#0xFFFFFFFF) }
	{ p1 = not(p0) }

l000073BC:
	{ r1 = p1; immext(#0xE480); r0 = #0xE498 }
	{ call _Unlockfilelock }
	{ r1 = memd(r29); r17:r16 = memd(r29+8) }
	{ p0 = r1 }
	{ r0 = mux(p0,#0xFFFFFFFF,#0x0); dealloc_return }

;; memcpy: 000073E0
;;   Called from:
;;     000062B0 (in _Putstr)
;;     00006BF8 (in _Vacopy)
;;     00007174 (in fwrite)
;;     00007B8C (in _Ldtob)
;;     00008270 (in _Litob)
;;     000088FC (in _LXp_movx)
;;     00008A40 (in fn00008A3C)
;;     00008A78 (in fn00008A5C)
;;     00008B10 (in _LXp_invx)
;;     00008B64 (in fn00008B64)
;;     00008CB4 (in fn00008CAC)
;;     00008CB4 (in fn00008CB4)
;;     00009054 (in _Atrealloc)
;;     00009078 (in _Atrealloc)
;;     000093F0 (in fputs)
;;     00009880 (in _Genld)
;;     00009958 (in _Genld)
;;     000099A8 (in _Genld)
;;     00009A3C (in _Genld)
;;     0000A390 (in __hexagon_memcpy_likely_aligned_min32bytes_mult8bytes)
memcpy proc
	{ p2 = cmp.eq(r2,#0x0); r14 = or(r1,r0); p0 = cmp.gtu(r2,#0x17); p1 = cmp.eq(r1,r0) }
	{ p1 = or(p2,p1); r9 = lsr(r2,#0x3); p3 = cmp.gtu(r2,#0x5F); r14 = or(r14,r2) }
	{ p2 = bitsclr(r14,#0x7); if (p1) jumpr:nt r31; dcfetch(r1,#0x0) }
0000740C                                     02 42 63 6B             .Bck
00007410 4A 6A 00 5C 02 FF 42 74 36 60 20 5C FF 7F E8 71 Jj.\..Bt6` \...q
00007420 0F C0 40 76 FF 7F 68 72 A7 40 02 8C 05 44 01 B0 ..@v..hr.@...D..
00007430 03 C0 9D A0 B8 76 7F 71 4F 47 48 CC E9 40 01 76 .....v.qOGH..@.v
00007440 00 D0 DD A1 13 40 21 F3 00 40 3F 72 03 40 49 75 .....@!..@?r.@Iu
00007450 01 D2 DD A1 14 41 02 F3 07 49 02 F3 16 2A 2D 70 .....A...I...*-p
00007460 00 47 8F 85 EF 43 0F 76 05 44 05 B0 00 C0 05 94 .G...C.v.D......
00007470 04 42 2F F3 48 43 0F 8C F0 40 0F 76 01 FF 21 76 .B/.HC...@.v..!v
00007480 2F 43 0F 8C E3 43 04 76 11 4F 00 F3 F4 C0 14 76 /C...C.v.O.....v
00007490 03 40 54 75 75 C1 14 EF 15 C1 75 74 03 41 87 75 .@Tuu.....ut.A.u
000074A0 24 45 04 8C 05 44 05 B0 00 C0 05 94 01 40 0F 75 $E...D.......@.u
000074B0 2F 60 AF 74 05 44 05 B0 00 C0 05 94 01 40 04 75 /`.t.D.......@.u
000074C0 05 44 05 B0 0E 41 08 76 00 C0 05 94 05 44 05 B0 .D...A.v.....D..
000074D0 10 41 00 5C 22 40 04 75 00 C0 05 94 11 44 D1 74 .A.\"@.u.....D.t
000074E0 00 C0 D1 A0 F3 43 13 76 00 C0 D1 A0 26 40 00 5C .....C.v....&@.\
000074F0 02 40 49 85 4C 40 C1 9B 2A D8 C1 41 46 4A 0C C2 .@I.L@..*..AFJ..
00007500 00 43 08 85 09 D0 09 F3 46 4E 86 C3 00 44 08 85 .C......FN...D..
00007510 0E 42 08 76 08 E6 00 AB 46 4E 86 C3 00 45 08 85 .B.v....FN...E..
00007520 E2 40 89 75 08 E6 40 AB 4C 4A 0B FD 02 40 49 85 .@.u..@.LJ...@I.
00007530 2A 64 C1 9B 08 E6 80 AB 08 C0 AF 60 46 4A 0C C2 *d.........`FJ..
00007540 00 41 55 F2 0B E6 C0 AB 0C 8A 0B F5 2A E0 C1 9B .AU.........*...
00007550 03 40 84 75 E4 7F 64 74 03 E4 63 74 5E 40 04 12 .@.u..dt..ct^@..
00007560 0E 64 7F 7C 03 C3 93 75 30 C3 00 5C 18 40 04 60 .d.|...u0..\.@.`
00007570 20 40 84 75 08 C0 64 70 11 44 11 74 00 C0 05 94  @.u..dp.D.t....
00007580 03 48 04 F2 00 C0 D1 A0 A4 4E 04 D3 52 4A 0C C2 .H.......N..RJ..
00007590 2C 40 C1 9B 0F F2 C0 AB 46 4C 0A C2 2A 40 C1 9B ,@......FL..*@..
000075A0 08 C6 C0 AB 52 4A 0C C2 2C 40 C1 9B 08 D2 C0 AB ....RJ..,@......
000075B0 46 8C 0A C2 20 40 84 75 2A 40 C1 9B 08 C6 C0 AB F... @.u*@......
000075C0 2C 40 00 58 08 D2 C0 AB 18 40 04 60 20 40 84 75 ,@.X.....@.` @.u
000075D0 E8 FF E4 BF A4 4E 04 D3 11 44 11 74 00 C0 05 94 .....N...D.t....
000075E0 00 C0 D1 A0 46 4A 0C C2 2C 40 C1 9B 08 C6 C0 AB ....FJ..,@......
000075F0 46 4C 0A C2 2A 40 C1 9B 08 C6 C0 AB 46 4A 0C C2 FL..*@......FJ..
00007600 2C 40 C1 9B 08 C6 C0 AB 46 8C 0A C2 20 40 84 75 ,@......F... @.u
00007610 2A 40 C1 9B 08 C6 C0 AB 00 40 03 75 2F 43 03 8C *@.......@.u/C..
00007620 E4 C0 03 76 45 43 03 8C 00 40 5F 53 00 40 22 FB ...vEC...@_S.@".
00007630 03 C0 0F 75 00 42 03 85 12 43 00 5C 0E C4 05 76 ...u.B...C.\...v
00007640 08 C0 0F 60 46 4A 0C C2 03 41 55 F2 08 C6 C0 AB ...`FJ...AU.....
00007650 0C 8A 0B F5 2A E6 C1 9B 46 4E 86 C3 00 41 03 85 ....*...FN...A..
00007660 0E 42 05 76 08 E6 80 AB 46 4E 86 C3 00 40 03 85 .B.v....FN...@..
00007670 E2 7F 02 74 08 E6 40 AB 00 40 22 F3 00 40 9F 52 ...t..@..@"..@.R
00007680 00 C6 00 40 10 40 A2 60 E2 FF E2 BF 26 80 01 9B ...@.@.`....&...
00007690 0B E6 00 AB 00 40 22 F3 00 40 9F 52 00 C6 00 A1 .....@"..@.R....
000076A0 08 C0 A9 60 26 80 C1 9B 0B E6 C0 AB 00 40 22 F3 ...`&........@".
000076B0 00 40 9F 52 00 C6 C0 A1 54 C0 DD 91 04 1E 0D 3E .@.R....T......>
000076C0 1E C0 1E 90 00 C0 9F 52 00 C0 00 7F 00 C0 00 7F .......R........
000076D0 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F ................

;; strlen: 000076E0
;;   Called from:
;;     000093B0 (in fputs)
;;     0000B034 (in __sys_remove)
;;     0000B048 (in __sys_remove)
strlen proc
	{ p1 = bitsclr(r0,#0x7); if (!p1.new) jump:nt 00007730; r2 = #0x0; r4 = memb(r0) }

l000076EC:
	{ p0 = cmp.eq(r4,#0x0); if (p0.new) jump:nt 00007728 }

l000076F0:
	{ r7:r6 = combine(#0x0,#0x0); if (!p0) r2 = add(r2,#0x8); nop; if (!p0) r5:r4 = memd(r0+r2) }

l00007700:
	{ p0 = any8(vcmpb.eq(r5:r4,r7:r6)); if (!p0.new) jump:t 00007700; if (!p0.new) r2 = add(r2,#0x8); if (!p0.new) r5:r4 = memd(r0+r2) }

l00007710:
	{ p0 = vcmpb.eq(r5:r4,r7:r6) }
	{ r4 = p0 }
	{ r4 = ct0(r4); r2 = add(r2,#0xFFFFFFF8) }
	{ r0 = add(r2,r4); jumpr r31 }

l00007728:
	{ nop; r0 = #0x0; jumpr r31 }

l00007730:
	{ r1 = add(r0,r2); p1 = cmp.eq(r4,#0x0); if (p1.new) jump:nt 00007758 }

l00007738:
	{ p0 = bitsclr(r1,#0x7); if (!p0.new) jump:nt 00007730; if (!p0.new) r2 = add(r2,#0x1); if (!p0.new) r4 = memb(r1+1) }

l00007748:
	{ r7:r6 = combine(#0x0,#0x0); jump 00007700; r2 = add(r2,#0x8); r4 = memd(r12+r2) }

l00007758:
	{ r0 = r2; jumpr r31 }
0000775C                                     00 C0 00 7F             ....

;; raise: 00007760
;;   Called from:
;;     00006E30 (in abort)
raise proc
	{ r1 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x20) }
	{ call signal; r18 = #0xFFFFFFFF; r16 = r0; memd(r29+16) = r19:r18 }
	{ r17 = r0 }
	{ p0 = cmp.eq(r9,#0x0); if (!p0.new) jump:t 000077E4 }

l00007780:
	{ r0 = add(r16,#0xFFFFFFFE); r1 = memw(gp+124); if (!cmp.gtu(r0.new,#0xD)) jump:nt 00007808 }

l00007790:
	{ immext(#0x66666640); r1:r0 = combine(#0x66666642,#0x27) }
	{ r2 = add(r3,#0x8); memb(r3+9) = #0x80 }

l000077A0:
	{ r4 = mpy(r16,r0); r5 = add(r16,#0x9) }
	{ r3 = lsr(r4,#0x1F); p0 = cmp.gtu(r5,#0x12) }
	{ r3 += asr(r4,r1) }
	{ r16 -= mpyi(r3,#0xA) }
	{ if (p0) jump:nt 000077A0; r4 = add(r16,#0x30); r16 = r3; memb(r2++#-1) = r4.new }

l000077CC:
	{ r0 = #0x3; immext(#0xE4C0); r1 = #0xE4E8 }
	{ call fputs; r17 = add(r2,#0x1) }
	{ jump 00007850 }

l000077E4:
	{ p0 = cmp.eq(r9,#0x1); if (p0.new) jump:t 000077FC; r18 = #0x0 }

l000077EC:
	{ call signal; r18 = #0x0; r1:r0 = combine(#0x0,#0x0) }
	{ r0 = r16; callr r17 }

l000077FC:
	{ r0 = r18; r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ dealloc_return }

l00007808:
	{ immext(#0xD440); r17 = #0xD45C; r0 = memw(r30+r0<<#2) }
	{ jumpr r0 }
00007818                         1C 40 00 58 52 43 00 00         .@.XRC..
00007820 71 C1 00 78 16 40 00 58 51 43 00 00 71 C6 00 78 q..x.@.XQC..q..x
00007830 10 40 00 58 51 43 00 00 51 C4 00 78 0A 40 00 58 .@.XQC..Q..x.@.X
00007840 52 43 00 00 11 C3 00 78 52 43 00 00 F1 C5 00 78 RC.....xRC.....x

l00007850:
	{ call fputs; immext(#0xE4C0); r1:r0 = combine(#0xE4E8,r17) }
	{ immext(#0xD4C0); r0 = #0xD4CC; immext(#0xE4C0); r1 = #0xE4E8 }
	{ call fputs }
	{ call exit; r0 = #0x1 }
	{ nop }
	{ nop }

;; signal: 00007880
;;   Called from:
;;     00007768 (in raise)
;;     000077EC (in raise)
;;     0000787C (in raise)
signal proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r0 = add(r16,#0xFFFFFFFF); r18 = #-0x1; memd(r29) = r19:r18 }
	{ if (p0.new) jump:t 000078BC; p0 = cmp.gtu(r0,#0x2A) }

l00007898:
	{ p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:t 000078BC; if (!p0.new) r0 = #0x1 }

l000078A0:
	{ call _Locksyslock }
	{ r0 = #0x1; immext(#0xEA80); r18 = memw(r16<<#2+0000EA80) }
	{ call _Unlocksyslock; immext(#0xEA80); memw(r16<<#2+0000EA80) = r17 }

l000078BC:
	{ r0 = r18; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }
000078C8                         00 C0 00 7F 00 C0 00 7F         ........

;; strchr: 000078D0
;;   Called from:
;;     000059E4 (in _Printf)
strchr proc
	{ r2 = and(r1,#0xFF) }

l000078D4:
	{ r1 = memb(r0) }
	{ p0 = cmp.eq(r1,#0x0); if (!p0.new) jump:t 000078D4; r0 = add(r0,#0x1); r1 = #0x0 }

l000078E4:
	{ r0 = r1 }
	{ jumpr r31 }
000078EC                                     00 C0 00 7F             ....

;; _Tls_get__Ctype: 000078F0
;;   Called from:
;;     00007998 (in _Getpctype)
_Tls_get__Ctype proc
	{ r1 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ immext(#0x10040); r16 = #0x1004C; immext(#0x10040); r17 = #0x10050 }
	{ memd(r29) = r19:r18 }

l0000790C:
	{ r0 = memw_locked(r16) }
	{ p0 = cmp.gt(r0,#0x0) }
	{ if (p0) jump:nt 00007920 }

l00007918:
	{ memw_locked(r16,p0) = r1 }
	{ if (!p0) jump:nt 0000790C }

l00007920:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 00007940; if (p0.new) r18 = #0x2 }

l00007928:
	{ immext(#0x10040); r0 = #0x10050; immext(#0x6FC0); r1 = #0x6FF0 }
	{ call sys_Tlsalloc }
	{ memw(r16) = r18 }

l00007940:
	{ r0 = memw(r16); if (!cmp.gt(r0.new,#0x1)) jump:t 00007940 }

l0000794C:
	{ r0 = memw(r17) }
	{ r1:r0 = combine(#0x4,#0x1); r16 = r0; if (!cmp.eq(r16.new,#0x0)) jump:t 0000798C }

l00007960:
	{ r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 0000798C }

l0000796C:
	{ r1 = r16; r0 = memw(r17) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00007980 }

l00007974:
	{ call free; r16 = #0x0; r0 = r16 }
	{ jump 0000798C }

l00007980:
	{ immext(#0xD4C0); r0 = #0xD4E2; memb(r16) = r0.new }

l0000798C:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }

l00007990:
	{ r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; _Getpctype: 00007998
;;   Called from:
;;     00005830 (in _Printf)
;;     00005928 (in _Printf)
;;     00005934 (in _Printf)
;;     000059A0 (in _Printf)
;;     000059B4 (in _Printf)
;;     000066E0 (in _Stoulx)
_Getpctype proc
	{ call _Tls_get__Ctype; allocframe(#0x0) }
	{ r0 = memw(r0); dealloc_return }
000079A4             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; _Exit: 000079B0
;;   Called from:
;;     00006FE0 (in exit)
_Exit proc
	{ call _exit; allocframe(#0x0) }
	{ nop }
	{ nop }

;; _Fwprep: 000079C0
;;   Called from:
;;     000070F4 (in fwrite)
;;     000079BC (in _Exit)
;;     000092FC (in fputc)
;;     00009390 (in fputs)
_Fwprep proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r1 = memw(r16+16) }
	{ r0 = memw(r16+24); if (!cmp.gtu(r0.new,r1)) jump:t 000079D8 }

l000079D8:
	{ r0 = memuh(r16) }
	{ immext(#0x9000); r2 = and(r0,#0x9002); if (!cmp.eq(r2.new,#0x2)) jump:t 00007A50 }

l000079EC:
	{ r3 = and(r0,r2) }
	{ r2 = memw(r16+12); if (cmp.gtu(r2.new,r1)) jump:t 00007A10 }

l00007A00:
	{ r0 = r16 }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 00007A9C; r1 = #0xFFFFFFFF }

l00007A0C:
	{ r0 = memh(r16) }

l00007A10:
	{ immext(#0x800); r0 = and(r0,#0x800); if (!cmp.eq(r0.new,#0x0)) jump:t 00007A84 }

l00007A20:
	{ r0 = add(r16,#0x4C); if (!cmp.eq(r0.new,r17)) jump:t 00007A84 }

l00007A2C:
	{ r0 = #0x200 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00007A74; memw(r16+8) = r0 }

l00007A38:
	{ r2 = add(r0,#0x200); r1 = memh(r16); memw(r16+16) = r0 }
	{ r0 = setbit(r1,#0xC); memw(r16+52) = r0; memw(r16+48) = r0 }
	{ jump 00007A80; memw(r16+12) = r2; memuh(r16+8) = r0 }

l00007A50:
	{ r1 = r0 }
	{ immext(#0x4000); r1 = and(#0x4002,asl(r1,#0x2)) }
	{ r2 = togglebit(r1,#0x1C); r1 = #0xFFFFFFFF }
	{ r0 = or(r2,r0) }
	{ r0 = setbit(r0,#0x12); jump 00007A9C; memb(r16) = r0.new }

l00007A74:
	{ r0 = add(r17,#0x1); memw(r16+8) = r17; memw(r16+16) = r17 }
00007A78                         89 04 89 A2 03 C0 90 A1         ........

l00007A80:
	{ call _Closreg }

l00007A84:
	{ r1 = #0x0; r0 = memw(r16+8); r2 = memh(r16) }
	{ immext(#0x6000); r2 = or(r2,#0x6000); r3 = memw(r16+12); memw(r16+20) = r0 }
	{ memw(r16+24) = r3; memuh(r16+8) = r2 }

l00007A9C:
	{ r0 = r1; r17:r16 = memd(r29); dealloc_return }
00007AA4             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; _Getmem: 00007AB0
_Getmem proc
	{ p0 = cmp.gt(r0,#0x0); if (!p0.new) jump:nt 00007AC4; allocframe(#0x0) }

l00007AB8:
	{ call __sys_sbrk }
	{ p0 = cmp.eq(r0,#0xFFFFFFFF); r0 = #-0x1; dealloc_return }

l00007AC4:
	{ r0 = #0x0; dealloc_return }
00007AC8                         00 C0 00 7F 00 C0 00 7F         ........

;; _Ldtob: 00007AD0
;;   Called from:
;;     00005CF0 (in fn00005C7C)
_Ldtob proc
	{ call __save_r16_through_r27; allocframe(#0x100) }
	{ r26 = r1; r16 = r0 }
	{ r18 = setbit(r26,#0xA); r1:r0 = memd(r16) }
	{ p0 = cmp.eq(r18,#0x61); memd(r29+152) = r1:r0 }
	{ r0 = p0; if (p0) jump:nt 00007B18; memb(r29) = r0.new }

l00007B00:
	{ if (!cmp.gt(r0.new,#-0x1)) jump:nt 00007B18 }

l00007B08:
	{ jump 00007B18; p0 = cmp.eq(r18,#0x67); if (p0.new) memw(r16+48) = #0x1 }
00007B14             06 C6 50 3C                             ..P<        

l00007B18:
	{ call _LDunscale; r0 = add(r29,#0x96); r1 = r16 }
	{ r2 = setbit(r26,#0x4); r1 = zxth(r0) }
	{ p0 = cmp.eq(r2,#0x65); immext(#0xD6C0); r1 = #0xD6E2; r0 = memw(r16+16) }
	{ if (p0) jump:nt 00007B8C; if (!p0) r2 = add(r26,#0xFFFFFF9A) }

l00007B44:
	{ immext(#0xD6C0); r4 = #0xD6E6; r3 = #0x2 }
	{ jump 00007B80 }
00007B50 00 40 40 75 00 C0 9D 91 01 40 40 85 22 C0 20 5C .@@u.....@@.". \
00007B60 02 42 DA 8C 5B 43 00 00 80 04 A1 4A 10 58 00 5C .B..[C.....J.X.\
00007B70 42 73 9A 74 A0 CC 02 75 5B 43 00 00 23 28 E4 2A Bs.t...u[C..#(.*

l00007B80:
	{ r2 = and(r2,#0xFF) }
	{ p0 = cmp.gtu(r3,r2); if (!p0.new) r1 = add(r4,#0x0) }

l00007B8C:
	{ call memcpy; r2 = #0x3; memw(r16+28) = #0xFFFFFF83 }
	{ jump __restore_r16_through_r27_and_deallocframe }
00007B9C                                     1E 41 20 5C             .A \
00007BA0 80 C8 90 41 20 CC 1A 75 22 40 00 B0 04 D2 B0 A1 ...A ..u"@......
00007BB0 02 4F 00 7E 02 4B 80 7E 30 C0 00 3C 80 C0 90 91 .O.~.K.~0..<....
00007BC0 23 40 00 B0 04 D3 B0 A1 00 C2 00 A1 A0 C0 90 91 #@..............
00007BD0 40 40 00 B0 05 D2 B0 A1 EC 40 01 10 F4 C1 20 5C @@.......@.... \
00007BE0 82 41 90 91 14 C3 C0 49 E1 7F 62 75 22 40 02 B0 .A.....I..bu"@..
00007BF0 64 42 DD 91 00 C0 D0 91 40 54 E4 D2 13 40 22 74 dB......@T...@"t
00007C00 33 C4 A0 7E 16 C0 F3 70 06 40 00 5C 57 C0 16 B0 3..~...p.@.\W...
00007C10 41 DF C1 8C 02 54 1D B0 00 40 57 75 63 49 5D 91 A....T...@WucI].
00007C20 13 C0 DD A1 12 40 C2 8C 82 7F E3 BF 00 C0 02 3C .....@.........<
00007C30 5A 40 20 5C 19 40 72 70 4B C2 5D A1 06 40 00 58 Z@ \.@rpK.]..@.X
00007C40 18 E0 12 73 60 C2 DD 91 40 40 F4 D2 4E C8 00 5C ...s`...@@..N..\
00007C50 88 4F 00 5A 00 53 1D B0 C1 29 9F 27 00 40 57 75 .O.Z.S...).'.@Wu
00007C60 60 C2 DD 91 02 40 40 89 31 40 E0 88 01 D4 BD A1 `....@@.1@......
00007C70 08 C0 20 5C 42 40 91 84 18 D5 00 5A 13 C0 DD A1 .. \B@.....Z....
00007C80 1E 60 C9 10 E0 40 00 78 E1 C0 19 B0 C1 40 19 B0 .`...@.x.....@..
00007C90 C2 C0 00 78 00 C0 62 70 22 40 00 B0 0E C0 C2 24 ...x..bp"@.....$
00007CA0 11 44 11 8C E3 41 11 76 E2 7F E0 BF 78 C5 A1 AB .D...A.v....x...
00007CB0 F2 E0 B9 10 10 60 C0 10 21 C0 01 B0 18 40 00 60 .....`..!....@.`
00007CC0 02 40 40 76 E3 FF E1 BF 00 80 00 7F 78 D8 03 AB .@@v........x...
00007CD0 01 C2 01 F3 F9 40 01 B0 20 C0 9D 91 00 40 40 85 .....@.. ....@@.
00007CE0 B4 F8 DF 5C 01 D9 32 F3 00 41 56 F2 00 60 01 74 ...\..2..AV..`.t
00007CF0 00 E0 93 74 20 4C 1A 75 02 40 E0 70 52 C0 C2 26 ...t L.u.@.pR..&
00007D00 01 42 41 F2 5B 43 00 00 41 C6 00 78 5C 43 00 00 .BA.[C..A..x\C..
00007D10 03 28 35 28 0E C1 20 5C 03 D4 1D B0 03 C3 02 F3 .(5(.. \........
00007D20 23 C0 23 91 E1 40 83 75 E3 61 20 7E 03 E0 A0 7E #.#..@.u.a ~...~
00007D30 04 54 1D B0 01 40 85 74 06 34 00 31 24 C0 06 E2 .T...@.t.4.1$...
00007D40 E0 7F E0 BF E2 7F E2 BF E4 7F E4 BF E5 FF 24 97 ..............$.
00007D50 F8 E3 35 14 E0 41 03 75 23 60 05 74 00 C3 A4 42 ..5..A.u#`.t...B
00007D60 0C E1 82 11 12 54 1D B0 20 40 00 B0 62 C9 5D 91 .....T.. @..b.].
00007D70 82 40 02 B0 4B CA BD A1 02 40 E0 70 10 C0 C2 24 .@..K....@.p...$
00007D80 08 C0 02 60 03 42 12 F3 E2 FF E2 BF E4 FF 23 97 ...`.B........#.
00007D90 04 84 21 3A FF E2 A3 A7 81 41 90 91 A4 E0 93 26 ..!:.....A.....&
00007DA0 01 C0 E0 70 9E 41 00 58 E1 7F E1 BF 0C D3 B0 A1 ...p.A.X........
00007DB0 C1 52 1D B0 8A 2E 00 28 94 41 00 58 00 C0 21 3C .R.....(.A.X..!<
00007DC0 62 42 DD 91 00 C3 C0 49 40 40 E2 D2 0C 58 00 5C bB.....I@@...X.\
00007DD0 20 DA 9D 46 43 DF C3 8C 06 40 00 58 13 C2 DD A1  ..FC....@.X....
00007DE0 04 DA 9D A1 B6 42 00 5A E0 2D 31 28 D6 62 4F 01 .....B.Z.-1(.bO.
00007DF0 20 41 00 78 62 49 5D 91 01 C4 80 49 D6 41 00 00  A.xbI]....I.A..
00007E00 E3 42 02 E0 02 C0 61 70 20 C0 03 ED 24 5F 00 8C .B....ap ...$_..
00007E10 03 CD 00 8C 22 44 03 EF A3 DF 00 8E 00 43 01 D5 ...."D.......C..
00007E20 01 43 41 D5 4B C2 5D A1 1A 41 C1 11 13 E0 00 7E .CA.K.]..A.....~
00007E30 40 C0 C1 10 93 7F 20 76 FF 43 00 00 81 C7 00 76 @..... v.C.....v
00007E40 38 40 01 10 4B D3 5D A1 97 43 00 00 02 45 00 78 8@..K.]..C...E.x
00007E50 80 2D 31 28 50 C5 00 5A 2C C0 00 58 60 40 40 76 .-1(P..Z,..X`@@v
00007E60 13 C0 00 78 84 FF 20 76 24 40 04 10 00 40 44 76 ...x.. v$@...@Dv
00007E70 4B CA BD A1 00 4C 1D B0 61 40 00 78 13 40 64 70 K....L..a@.x.@dp
00007E80 22 C3 C0 49 66 C2 00 5A 14 40 00 58 00 C0 00 7F "..If..Z.@.X....
00007E90 32 45 00 5A 02 4C 1D B0 60 2C 31 28 04 46 1D B0 2E.Z.L..`,1(.F..
00007EA0 61 40 00 78 80 2D 33 28 A4 45 00 5A 02 C3 1D B0 a@.x.-3(.E.Z....
00007EB0 0C 63 CB 11 63 E0 00 7E 00 4F 1D B0 61 40 00 78 .c..c..~.O..a@.x
00007EC0 82 2D C4 2C 96 C5 00 5A 13 41 13 8C C0 4C 12 75 .-.,...Z.A...L.u
00007ED0 E4 E0 75 24 E1 40 00 78 58 08 80 CC 02 40 40 89 ..u$.@.xX....@@.
00007EE0 0A 40 20 5C 03 D4 BD A1 61 C9 5D 91 61 C1 01 B0 .@ \....a.].a...
00007EF0 63 42 00 78 10 38 82 2E 12 40 C2 8C 13 43 A0 D5 cB.x.8...@...C..
00007F00 30 C0 02 3C 70 40 CB 10 1B C0 72 70 1A 66 12 73 0..<p@....rp.f.s
00007F10 54 43 C0 49 78 C3 C0 49 60 42 DD 91 02 C3 C0 49 TC.Ix..I`B.....I
00007F20 40 40 E2 D2 62 48 00 5C 16 E0 80 7E E0 C1 DD 91 @@..bH.\...~....
00007F30 20 40 F8 D2 14 D8 00 5C 30 40 E0 88 E0 2D 31 28  @.....\0@...-1(
00007F40 42 C0 90 84 43 5F C3 8C 86 C2 00 5A 78 3E 8E 58 B...C_.....Zx>.X
00007F50 40 58 E0 D2 F4 F8 DF 5C 13 FF F3 BF 02 74 15 FD @X.....\.....t..
00007F60 00 C0 53 75 00 40 40 89 0A 40 20 5C 02 D4 BD A1 ..Su.@@..@ \....
00007F70 0C 44 00 5A E0 2D 31 28 22 60 CE 10 10 41 00 78 .D.Z.-1("`...A.x
00007F80 00 C1 1B B0 FB 40 1B B0 F1 C0 00 78 10 C0 71 70 .....@.....x..qp
00007F90 20 40 10 B0 12 C0 C2 24 94 4A 00 5A 40 61 36 73  @.....$.J.Z@a6s
00007FA0 F1 FF F0 BF 16 C0 01 F5 F2 60 BE 10 00 46 17 B0 .........`...F..
00007FB0 78 C2 BB AB 10 60 C8 10 20 C0 1B B0 18 40 10 60 x....`.. ....@.`
00007FC0 E2 7F E0 BF 01 C0 50 76 00 80 00 7F 78 DA 02 AB ......Pv....x...
00007FD0 00 C1 00 F3 1B 41 00 B0 41 C0 9D 91 00 40 41 85 .....A..A....@A.
00007FE0 9E F8 DF 5C 01 5B 32 F3 60 C9 5D 91 E0 40 00 B0 ...\.[2.`.]..@..
00007FF0 4B CA BD A1 02 C0 32 91 24 58 20 5C 00 46 02 75 K.....2.$X \.F.u
00008000 9A 40 9D 47 B0 C0 9D 47 02 54 1D B0 B0 40 9D 91 .@.G...G.T...@..
00008010 9A C0 9D 91 02 41 C2 8C 00 40 00 7F 00 C0 00 7F .....A...@......
00008020 E0 7F E0 BF E1 7F E1 BF 4B CC BD A1 23 C0 22 9B ........K...#.".
00008030 F8 78 DF 5C 00 C6 03 75 04 40 00 58 F2 FF E2 BF .x.\...u.@.X....
00008040 33 3C 82 0C 00 40 43 85 0A 48 20 5C 00 E4 E0 70 3<...@C..H \...p
00008050 0C 40 00 58 23 C0 00 B0 A0 4C 1A 75 A1 C8 1A 75 .@.X#....L.u...u
00008060 00 C0 21 6B 23 C0 00 7A 00 43 02 D5 00 7C FF 0F ..!k#..z.C...|..
00008070 32 38 13 28 00 41 40 F2 00 60 01 74 00 E0 82 74 28.(.A@..`.t...t
00008080 02 40 00 70 30 E3 82 21 03 46 00 78 02 40 E0 70 .@.p0..!.F.x.@.p
00008090 0E E1 C2 21 01 C2 32 3A 80 46 81 75 23 67 00 7E ...!..2:.F.u#g.~
000080A0 03 E6 80 7E 20 C0 00 B0 01 42 12 F3 22 33 00 33 ...~ ....B.."3.3
000080B0 E1 7F 21 97 FC E3 33 20 20 47 03 75 03 62 12 FB ..!...3  G.u.b..
000080C0 21 E0 01 74 0E 61 82 11 00 C1 03 40 F2 7F F2 BF !..t.a.....@....
000080D0 20 40 00 B0 61 C9 5D 91 21 40 01 B0 4B CB BD A1  @..a.].!@..K...
000080E0 03 40 E0 70 02 40 72 70 00 50 1A F5 64 C9 5D 91 .@.p.@rp.P..d.].
000080F0 48 CB 00 5A 2E D1 00 58 00 C0 00 7F 00 C0 00 7F H..Z...X........

;; _LDunscale: 00008100
;;   Called from:
;;     00007B18 (in _Ldtob)
;;     00008374 (in _LXp_setw)
;;     000083F4 (in _LXp_setw)
;;     00008468 (in fn00008468)
;;     00008530 (in fn00008468)
;;     00008634 (in fn00008468)
;;     000086D0 (in fn00008468)
;;     0000887C (in _LXp_mulh)
;;     00009528 (in sqrtl)
_LDunscale proc
	{ call _Dunscale; allocframe(#0x0) }
	{ dealloc_return }
0000810C                                     00 C0 00 7F             ....

;; _Litob: 00008110
_Litob proc
	{ call __save_r16_through_r27; immext(#0xD700); r2 = #0xD718; allocframe(#0x48) }
	{ p0 = cmp.eq(r1,#0x58); immext(#0xD700); r3 = #0xD730; r16 = r0 }
	{ if (!p1.new) jump:t 00008140; if (p0) r17 = add(r2,#0x0); if (!p0) r17 = add(r3,#0x0); p1 = cmp.eq(r1,#0x6F) }

l00008138:
	{ r8 = #0x8; jump 00008180; r21:r20 = memd(r16) }

l00008140:
	{ p0 = !cmp.eq(r1,00000078); p1 = !cmp.eq(r1,00000058); p2 = cmp.eq(r1,#0x64); r3:r2 = memd(r16) }
	{ p0 = fastcorner9(p1,p0); if (p2) jump:nt 0000816C; if (p0.new) r22 = #0xA; if (!p0.new) r22 = #0x10 }

l00008160:
	{ if (!p0.new) jump:t 00008180; if (!p0.new) r21:r20 = combine(r3,r2); p0 = cmp.eq(r1,#0x69) }

l0000816C:
	{ r21:r20 = combine(r3,r2); r0 = #0x3F }
	{ r21:r20 += asr(r21:r20,#0x3F) }
	{ XOREQ r21:r20,asr(r3:r2,r0); jump 00008180 }

l00008180:
	{ r1:r0 = combine(#0x0,#0x0) }
	{ p0 = cmp.eq(r21:r20,r1:r0); if (!p0.new) jump:t 00008194 }

l0000818C:
	{ r0 = memw(r16+48); if (cmp.eq(r0.new,#0x0)) jump:nt 000082D4 }

l00008194:
	{ r1:r0 = combine(r21,r20); r24 = add(r29,#0x0); r23 = #0x17; r19:r18 = combine(#0x0,#0x3) }

l00008198:
	{ r24 = add(r29,#0x0); r23 = #0x17; r19:r18 = combine(#0x0,#0x3) }

l000081A0:
	{ call __qdsp_umoddi3; r3:r2 = combine(r19,r18) }
	{ r0 = memb(r20+r0); memb(r24+23) = r0.new }

l000081B0:
	{ r25:r24 = combine(#0x0,#0x1); call __hexagon_udivdi3; r1:r0 = combine(r21,r20); r3:r2 = combine(r19,r18) }
	{ r21:r20 = combine(r1,r0) }
	{ p0 = cmp.gt(r25:r24,r21:r20); if (p0.new) jump:nt 00008230; memd(r16) = r21:r20 }

l000081D0:
	{ r26 = add(r23,#0xFFFFFFFF); r23 = add(r29,#0x0); nop; nop }

l000081E0:
	{ call __qdsp_divdi3; r1:r0 = combine(r21,r20); r3:r2 = combine(r19,r18); r27 = add(r23,r26) }
	{ p0 = cmp.gt(r25:r24,r1:r0); r5:r4 = mpyu(r0,r18); r2 = add(r26,#0xFFFFFFFF); memd(r16) = r1:r0 }
	{ r5 += mpyi(r0,r19) }
	{ r5 += mpyi(r18,r1) }
	{ r5:r4 = combine(r5,r4) }
	{ r5:r4 = sub(r21:r20,r5:r4); if (p0.new) r21:r20 = combine(r1,r0) }
	{ if (p0) jump:nt 0000822C; r3 = memb(r20+r4) }

l00008220:
	{ if (p0.new) jump:t 000081E0; p0 = cmp.gt(r26,#0x0); r26 = r2 }

l0000822C:
	{ r23 = add(r2,#0x1) }

l00008230:
	{ p0 = cmp.eq(r14,#0x8); if (!p0.new) jump:t 00008268; if (p0.new) r0 = memb(r16-4) }

l00008238:
	{ p0 = cmp.gtu(r23,#0x17); r0 = and(r0,#0x8); if (cmp.eq(r0.new,#0x0)) jump:nt 00008268 }

l00008248:
	{ if (!p0) r0 = add(r29,#0x0) }
	{ r0 = memb(r13+r23) }
	{ if (p0.new) jump:t 00008268; p0 = cmp.eq(r0,#0x30) }

l00008258:
	{ r0 = add(r29,#0x0) }
	{ r0 = add(r0,add(r23,#0x3F)); r23 = add(r23,#0xFFFFFFFF) }
	{ memb(r0) = #0x30 }

l00008268:
	{ r2 = sub(#0x18,r23); r1 = add(r29,#0x0); r0 = memw(r16+16) }
	{ call memcpy; r1 = add(r1,r23); memw(r16+28) = r2 }
	{ r1 = memw(r16+48) }
	{ p0 = cmp.gt(r1,#0xFFFFFFFF); r0 = memw(r16+28); if (!cmp.gtu(r1,r0.new)) jump:t 000082A4 }

l0000828C:
	{ r2 = memuh(r16+60) }
	{ immext(#0xFFC0); r1 = and(r2,#0xFFEF); memw(r16+24) = r0 }
	{ jump __restore_r16_through_r27_and_deallocframe; memuh(r16+60) = r1 }

l000082A4:
	{ if (p0) jump:nt 000082D0; if (!p0) r1 = memuh(r16+60) }

l000082AC:
	{ r1 = and(r1,#0x14) }
	{ r1 = memw(r16+24); r2 = memw(r16+20) }
	{ r0 = sub(r1,r0); r3 = memw(r16+24) }
	{ r0 = sub(r0,r2) }
	{ r0 = sub(r0,r3) }
	{ p0 = cmp.gt(r0,#0x0); if (p0.new) memw(r16+24) = r0 }

l000082D0:
	{ jump __restore_r16_through_r27_and_deallocframe }

l000082D4:
	{ r8 = #0x18; jump 000081B0; r19:r18 = combine(#0x0,r22) }
000082DC                                     00 C0 00 7F             ....

;; _LXp_getw: 000082E0
_LXp_getw proc
	{ p0 = cmp.eq(r1,#0x0); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ if (p0) jump:nt 00008348; r17:r16 = memd(gp+192) }

l000082F0:
	{ p0 = cmp.eq(r1,#0x1); if (!p0.new) jump:t 000082FC }

l000082F4:
	{ jump 00008348; r17:r16 = memd(r0) }

l000082FC:
	{ r17:r16 = memd(r0); r3:r2 = memd(gp+192) }
	{ p0 = dfcmp.eq(r17:r16,r3:r2); if (p0.new) jump:t 00008348 }

l0000830C:
	{ p0 = cmp.gt(r1,#0x1); if (!p0.new) jump:t 0000832C; r5:r4 = memd(r0+8) }

l00008314:
	{ p0 = dfcmp.eq(r5:r4,r3:r2); if (p0.new) jump:t 00008348 }

l0000831C:
	{ p0 = cmp.eq(r1,#0x2); if (!p0.new) jump:t 0000832C; if (!p0) r3:r2 = combine(r5,r4); if (!p0) r1:r0 = combine(r17,r16) }

l00008328:
	{ jump 00008340 }

l0000832C:
	{ call __hexagon_adddf3; r1:r0 = combine(r5,r4); r3:r2 = memd(r0+16) }
	{ r3:r2 = combine(r1,r0); r1:r0 = combine(r17,r16) }

l00008340:
	{ call __hexagon_adddf3 }
	{ r17:r16 = combine(r1,r0) }

l00008348:
	{ r1:r0 = combine(r17,r16); r17:r16 = memd(r29); dealloc_return }

;; _LXp_setw: 00008350
;;   Called from:
;;     00008B5C (in fn00008B34)
;;     00008BF0 (in fn00008BE8)
;;     00008C90 (in _LXp_sqrtx)
_LXp_setw proc
	{ call __save_r16_through_r23; allocframe(#0x30) }
	{ r19:r18 = combine(r3,r2); r17:r16 = combine(r1,r0) }
	{ p0 = cmp.gt(r9,#0x0); if (!p0.new) jump:nt 00008448; memd(r29+8) = r19:r18 }

l00008368:
	{ p0 = cmp.eq(r9,#0x1); if (p0.new) jump:t 000083A0; if (!p0.new) r0 = add(r29,#0x6); if (!p0.new) r1 = add(r29,#0x8) }

l00008374:
	{ call _LDunscale }
	{ p0 = cmp.gt(r0,#0x0); r1 = zxth(r0) }
	{ if (!p0) jump:nt 000083AC }

l00008388:
	{ r3:r2 = combine(#0x0,#0x0); r1:r0 = memd(r29+8) }
	{ immext(#0x1F80); r0 = r8; jump 0000845C; memd(r16) = r1:r0; memd(r16+8) = r3:r2 }
0000839C                                     32 C0 DD 91             2...

l000083A0:
	{ immext(#0x1F80); r0 = r8; jump 00008420; memd(r16) = r19:r18 }

l000083AC:
	{ call _LDint; r1 = #0x1A; r0 = add(r29,#0x8) }
	{ call _LDscale; r0 = add(r29,#0x8); r1 = memh(r29+6) }
	{ r1:r0 = combine(r19,r18); r3:r2 = memd(r29+8) }
	{ call __hexagon_fast2_subdf3; memd(r16) = r3:r2 }
	{ r19:r18 = combine(r1,r0); p0 = cmp.gt(r17,#0x2) }
	{ if (!p0) jump:nt 0000843C; memd(r16+8) = r19:r18 }

l000083E0:
	{ r23:r22 = memd(gp+192) }
	{ p1 = dfcmp.eq(r19:r18,r23:r22); if (p1.new) jump:t 0000843C; if (!p1.new) r0 = add(r29,#0x6); if (!p1.new) r20 = add(r16,#0x8) }

l000083F4:
	{ call _LDunscale; r1 = r20 }
	{ call _LDint; r1:r0 = combine(#0x1A,r20) }
	{ call _LDscale; r0 = r20; r1 = memh(r29+6) }
	{ call __hexagon_fast2_subdf3; r1:r0 = combine(r19,r18); r3:r2 = memd(r20) }
	{ p0 = cmp.gt(r9,#0x3); if (!p0.new) jump:t 00008448; memd(r16+16) = r1:r0 }

l00008420:
	{ memd(r16+16) = r1:r0 }

l00008424:
	{ p0 = dfcmp.eq(r1:r0,r23:r22); if (p0.new) jump:t 00008448 }

l0000842C:
	{ r1:r0 = combine(#0x0,#0x0) }
	{ immext(#0x1F00); r0 = r8; jump 00008470; memd(r16+24) = r1:r0 }

l0000843C:
	{ if (!p0) jump:nt 00008448 }

l00008440:
	{ r1:r0 = combine(#0x0,#0x0) }
	{ memd(r16+16) = r1:r0 }

l00008448:
	{ immext(#0x1EC0); r0 = r8; jump 00008528 }

;; _LXp_addh: 00008450
;;   Called from:
;;     00008898 (in _LXp_mulh)
;;     000088AC (in _LXp_mulh)
;;     00008940 (in _LXp_addx)
;;     00008984 (in _LXp_mulh)
;;     00008984 (in fn00008970)
;;     00008AA4 (in fn00008AA4)
;;     00008B80 (in fn00008B64)
;;     00008BB8 (in fn00008B64)
;;     00008CEC (in fn00008CCC)
;;     00008CEC (in fn00008CCC)
_LXp_addh proc
	{ call __save_r16_through_r27; allocframe(#0x60) }
	{ r19:r18 = combine(r3,r2); r17 = r1; memw(r29+8) = r0 }

l0000845C:
	{ r17 = r1; memw(r29+8) = r0 }

;; fn00008460: 00008460
;;   Called from:
;;     00008458 (in _LXp_addh)
;;     0000845C (in _LXp_setw)
fn00008460 proc
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt fn00008780; memd(r29+40) = r19:r18; memd(r29+32) = r19:r18 }

;; fn00008468: 00008468
;;   Called from:
;;     00008430 (in _LXp_setw)
;;     00008448 (in _LXp_setw)
;;     00008460 (in fn00008460)
;;     00008460 (in fn00008460)
fn00008468 proc
	{ call _LDunscale; r1 = add(r29,#0x20); r0 = add(r29,#0x1E) }

l00008470:
	{ r0 = add(r29,#0x1E) }
	{ p0 = cmp.gt(r0,#0x0); if (!p0.new) jump:nt 000084E0 }

l00008478:
	{ p0 = cmph.eq(r0,#0x2); if (p0.new) jump:t 00008754; if (!p0.new) r0 = memw(r29+8) }

l00008484:
	{ call _LDtest }
	{ p0 = cmp.gt(r0,#0x0); if (!p0.new) jump:nt 00008750 }

l0000848C:
	{ p0 = cmph.eq(r0,#0x2); if (p0.new) jump:t fn00008780 }

l00008494:
	{ r1 = add(r29,#0x28); r0 = memd(r29+8) }
	{ r1 = or(r1,#0x6); r0 = memh(r0+6) }
	{ r1 = memh(r1) }
	{ r0 = xor(r0,r1) }
	{ r0 = sxth(r0); if (cmp.gt(r0.new,#-0x1)) jump:t fn00008780 }

l000084B4:
	{ immext(#0xE600); r16 = #0xE630; r0 = #0x1 }
	{ p0 = cmp.gt(r17,#0x1); r1:r0 = memd(r16); r2 = memw(r29+8) }
	{ if (!p0) jump:nt fn00008780; memd(r2) = r1:r0 }

l000084D0:
	{ r1:r0 = combine(#0x0,#0x0); r2 = memd(r29+8) }
	{ jump __restore_r16_through_r27_and_deallocframe; r0 = memw(r29+8); memd(r2+8) = r1:r0 }

l000084E0:
	{ p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:t fn00008780 }

l000084E4:
	{ p0 = cmp.gt(r9,#0x0); if (!p0.new) jump:t fn00008780; if (p0.new) r16 = #0x0 }

l000084EC:
	{ r20 = add(r17,#0xFFFFFFFF); r19 = #0x800; r0 = memw(r29+8); r25:r24 = memd(gp+192) }
	{ r23:r22 = combine(#0x0,#0x0); r0 = addasl(r0,r20,#0x3); r1 = add(r0,#0x8); r21 = add(r0,#0xFFFFFFF8) }
	{ memw(r29+4) = r1; memw(r29) = r0 }

l00008510:
	{ r26 = add(r16,#0x1); r0 = memd(r29+8); r1 = memd(r29+4) }
	{ r18 = addasl(r0,r16,#0x3); r27 = addasl(r1,r16,#0x3) }
	{ jump 00008530; r3:r2 = memd(r18) }

l00008528:
	{ memd(r29+40) = r23:r22; memd(r18) = r3:r2 }

l00008530:
	{ call _LDunscale; r0 = add(r29,#0xE); r1 = add(r29,#0x10); memd(r29+16) = r3:r2 }
	{ p0 = cmp.gt(r0,#0x0); if (p0.new) jump:nt fn00008780 }

l00008540:
	{ r0 = zxth(r0); if (cmp.eq(r0.new,#0x0)) jump:nt 00008764 }

l0000854C:
	{ r4 = memh(r29+14) }
	{ r5 = sub(r4,r0) }
	{ if (p0.new) jump:nt 000085C0; if (!p0.new) r6 = add(r26,#0x0); if (!p0.new) r1 = add(r27,#0x0); p0 = cmp.gt(r5,#0xFFFFFFE6) }

l00008564:
	{ r3:r2 = memd(r29+40) }
	{ p0 = dfcmp.eq(r3:r2,r25:r24); if (p0.new) jump:nt 000085C0 }

l00008570:
	{ r0 = r6; if (!cmp.gtu(r17,r0.new)) jump:nt 0000858C }

l0000857C:
	{ r1 = add(r1,#0x8); r5:r4 = memd(r1) }
	{ p0 = dfcmp.eq(r5:r4,r25:r24); if (!p0.new) jump:t 00008570 }

l0000858C:
	{ p0 = cmp.gt(r12,r0); if (p0.new) jump:t 00008598; jump 000085A0; if (p0.new) r0 = add(r0,#0x1) }

l00008598:
	{ p0 = cmp.eq(r17,r0); if (p0.new) r0 = add(r0,#0xFFFFFFFF) }

l000085A0:
	{ p0 = cmp.gt(r0,r8); if (p0.new) jump:t 00008528 }

l000085A4:
	{ r0 = addasl(r21,r0,#0x3); r1 = sub(r0,r16) }
	{ loop0(000085B0,r1) }
	{ r5:r4 = memd(r0) }
	{ r0 = add(r0,#0xFFFFFFF8); memd(r0+8) = r5:r4 }
	{ jump 00008528 }

l000085C0:
	{ p0 = cmp.gt(r5,#0x19); if (!p0.new) jump:t 000085D8; r1:r0 = memd(r29+40) }

l000085C8:
	{ p0 = dfcmp.eq(r1:r0,r25:r24); if (p0.new) jump:t 000085D8 }

l000085D0:
	{ r11 = r4; jump 00008748; r16 = add(r16,#0x1) }

l000085D8:
	{ call __hexagon_adddf3; r3:r2 = memd(r18) }
	{ r3:r2 = combine(r1,r0) }
	{ p0 = dfcmp.eq(r3:r2,r25:r24); if (!p0.new) jump:t 00008634; memd(r18) = r3:r2 }

l000085F0:
	{ r0 = r16; r1 = memd(r29+4) }
	{ r1 = addasl(r1,r16,#0x3); nop; nop }

l00008600:
	{ r2 = add(r1,#0x8); r0 = add(r0,#0x1); if (!cmp.gtu(r17,r0.new)) jump:nt 00008620 }

l00008610:
	{ p0 = dfcmp.eq(r5:r4,r25:r24); if (!p0.new) jump:t 00008600; r1 = r2; memd(r1-8) = r5:r4 }

l00008620:
	{ r0 = memw(r29) }
	{ memd(r0) = r23:r22 }
	{ r3:r2 = memd(r18) }
	{ p0 = dfcmp.eq(r3:r2,r25:r24); if (p0.new) jump:nt fn00008780 }

l00008634:
	{ call _LDunscale; r0 = add(r29,#0x1E); r1 = add(r29,#0x28); memd(r29+40) = r3:r2 }
	{ r0 = add(r19,#0xFFFFFFE6) }
	{ r1 = memh(r29+30) }
	{ r1 = sub(r1.l,r0.l); call _LDint; r0 = add(r29,#0x28) }
	{ call _LDscale; r0 = add(r29,#0x28); r1 = memh(r29+30) }
	{ call __hexagon_fast2_subdf3; r1:r0 = memd(r18); r3:r2 = memd(r29+40) }
	{ p0 = dfcmp.eq(r1:r0,r25:r24); if (!p0.new) jump:t 000086AC; memd(r18) = r1:r0 }

l0000867C:
	{ r1 = r16; r0 = memd(r29+4) }
	{ r0 = addasl(r0,r16,#0x3) }

l00008684:
	{ r2 = add(r0,#0x8); r1 = add(r1,#0x1) }
	{ r5:r4 = memd(r0) }
	{ p0 = dfcmp.eq(r5:r4,r25:r24); if (!p0.new) jump:t 00008684; r0 = r2; memd(r0-8) = r5:r4 }

l000086A4:
	{ r0 = memw(r29) }
	{ memd(r0) = r23:r22 }

l000086AC:
	{ r26 = add(r16,#0xFFFFFFFF); r19 = #0x800; r16 = #0x0 }
	{ if (p0.new) jump:t 00008748; if (!p0.new) r1 = add(r29,#0x20); if (!p0.new) r0 = add(r29,#0xE); p0 = cmp.eq(r26,#0x0) }

l000086C8:
	{ r16 = r26; r3:r2 = memd(r18-16) }
	{ call _LDunscale; memd(r29+32) = r3:r2 }
	{ jump 00008748; r19 = memh(r29+14) }
000086E0 C0 41 1D B0 01 40 72 70 30 40 10 B0 50 D1 02 20 .A...@rp0@..P.. 
000086F0 02 C0 D2 91 06 7D FF 5B 05 C2 DD A1 2A 4A 00 5A .....}.[....*J.Z
00008700 40 E3 32 73 2E 4A 00 5A 00 40 72 70 E1 C0 5D 91 @.2s.J.Z.@rp..].
00008710 12 40 D2 91 A0 C0 DD 91 C6 4F 00 5A 02 D2 13 F5 .@.......O.Z....
00008720 00 58 E0 D2 05 C0 DD A1 00 40 C0 6B 02 60 01 FD .X.......@.k.`..
00008730 82 72 13 FD C0 C3 1D B0 E4 7C FF 5B 01 44 1D B0 .r.......|.[.D..
00008740 F3 40 5D 91 04 C2 DD A1                         .@].....        

l00008748:
	{ p0 = cmp.gt(r9,r8); if (p0.new) jump:t 00008510 }

l0000874C:
	{ jump fn00008780 }

l00008750:
	{ r19:r18 = memd(r29+40) }

l00008754:
	{ r0 = memw(r29+8) }
	{ jump __restore_r16_through_r27_and_deallocframe; r0 = memw(r29+8); memd(r0) = r19:r18 }

l00008764:
	{ r0 = r16; r3:r2 = memd(r29+40) }
	{ p0 = cmp.gt(r9,r0); if (p0.new) jump:t fn00008780; memd(r18) = r3:r2 }

l00008770:
	{ r1 = memw(r29+8) }
	{ jump __restore_r16_through_r27_and_deallocframe; r0 = memw(r29+8); memd(r30+r0<<#3) = r23:r22 }

;; fn00008780: 00008780
;;   Called from:
;;     00008460 (in fn00008460)
;;     00008460 (in fn00008460)
;;     0000848C (in fn00008468)
;;     000084A8 (in fn00008468)
;;     000084C8 (in fn00008468)
;;     000084E0 (in fn00008468)
;;     000084E4 (in fn00008468)
;;     0000853C (in fn00008468)
;;     0000863C (in fn00008468)
;;     0000874C (in fn00008468)
;;     00008768 (in fn00008468)
fn00008780 proc
	{ jump __restore_r16_through_r27_and_deallocframe; r0 = memw(r29+8) }

;; _LXp_mulh: 00008788
;;   Called from:
;;     00008A2C (in fn00008A2C)
;;     00008A48 (in fn00008A48)
;;     00008A48 (in fn00008A48)
;;     00008A7C (in fn00008A5C)
;;     00008B1C (in fn00008B1C)
;;     00008B1C (in fn00008B1C)
;;     00008CC0 (in fn00008CB4)
;;     00008CC0 (in _LXp_sqrtx)
_LXp_mulh proc
	{ call __save_r16_through_r27; allocframe(#0x60) }
	{ r17:r16 = combine(r1,r0); r19:r18 = combine(r3,r2) }
	{ p0 = cmp.gt(r9,#0x0); if (!p0.new) jump:nt 000088EC; if (!p0) r3:r2 = combine(r19,r18) }

l000087A0:
	{ call __hexagon_fast_muldf3; r1:r0 = memd(r16) }
	{ r3:r2 = combine(r1,r0); r0 = add(r29,#0x10) }
	{ call _LDtest; memd(r29+16) = r3:r2 }
	{ r23:r22 = combine(#0x0,#0x0); r1 = add(r29,#0x10); r20 = r0; if (!cmp.gt(r20.new,#-0x1)) jump:nt 000087FC }

l000087CC:
	{ if (!cmp.eq(r0.new,#0x2)) jump:t 000087DC }

l000087D4:
	{ r0 = #0x1 }
	{ p0 = cmp.gt(r20,#0x0); r1:r0 = memd(r29+16) }

l000087DC:
	{ r1:r0 = memd(r29+16) }

l000087E0:
	{ if (!p0) jump:nt 000088EC; memd(r16) = r1:r0 }

l000087E8:
	{ p0 = cmp.gt(r9,#0x1); if (!p0.new) jump:t 000088EC }

l000087EC:
	{ r1:r0 = combine(#0x0,#0x0) }
	{ immext(#0x1B40); r0 = r8; jump 00008870; memd(r16+8) = r1:r0 }

l000087FC:
	{ r0 = add(r1,#0x8); r25:r24 = combine(#0x1,#0x0); r27:r26 = memd(gp+192) }
	{ memw(r29) = r0; memd(r16) = r23:r22 }

l00008810:
	{ if (p0.new) jump:t 00008868; if (!p0.new) r1 = add(r29,#0x10); if (!p0.new) r0 = add(r25,r24); p0 = cmp.gt(r25,#0x3) }

l00008820:
	{ r21 = addasl(r1,r25,#0x3); r20 = addasl(r16,r0,#0x3) }

l00008828:
	{ r0 = add(r24,r25); if (!cmp.gtu(r17,r0.new)) jump:nt 00008860 }

l00008834:
	{ p0 = dfcmp.eq(r1:r0,r27:r26); if (p0.new) jump:nt 00008860; if (!p0.new) r25 = add(r25,#0x1); if (!p0.new) r3:r2 = combine(r19,r18) }

l00008844:
	{ call __hexagon_fast_muldf3 }
	{ r21 = add(r21,#0x8); p0 = cmp.gt(r25,#0x3); memd(r20++#8) = r23:r22; memd(r21) = r1:r0 }
	{ if (!p0) jump:nt 00008828 }

l0000885C:
	{ jump 00008868 }

l00008860:
	{ r25 = #0x8; memd(r21) = r23:r22 }

l00008868:
	{ r3:r2 = memd(r29+16) }
	{ p0 = dfcmp.eq(r3:r2,r27:r26); if (p0.new) jump:nt 000088EC; if (!p0.new) r1 = add(r29,#0x8); if (!p0.new) r0 = add(r29,#0x6) }

l00008870:
	{ if (p0.new) jump:nt 000088F0; if (!p0.new) r1 = add(r29,#0x8); if (!p0.new) r0 = add(r29,#0x6) }

l0000887C:
	{ call _LDunscale; memd(r29+8) = r3:r2 }
	{ call _LDint; r1 = #0x1A; r0 = add(r29,#0x8) }
	{ call _LDscale; r0 = add(r29,#0x8); r1 = memh(r29+6) }
	{ call _LXp_addh; r1:r0 = combine(r17,r16); r3:r2 = memd(r29+8) }
	{ call __hexagon_fast2_subdf3; r3:r2 = memd(r29+8); r1:r0 = memd(r29+16) }
	{ call _LXp_addh; r3:r2 = combine(r1,r0); r1:r0 = combine(r17,r16) }
	{ nop; r0 = #0x0; r1 = memd(r29) }

l000088C0:
	{ r2 = add(r1,#0x8); r0 = add(r0,#0x1); if (!cmp.gtu(r25,r0.new)) jump:nt 000088E0 }

l000088D0:
	{ p0 = dfcmp.eq(r5:r4,r27:r26); if (!p0.new) jump:t 000088C0; r1 = r2; memd(r1-8) = r5:r4 }

l000088E0:
	{ r25 = add(r25,#0xFFFFFFFF); r24 = add(r24,#0x1); if (cmp.gtu(r17,r24.new)) jump:t 00008810 }

l000088EC:
	{ immext(#0x1A40); r0 = r8; jump 0000897C }

l000088F0:
	{ r0 = r8; jump 00008980 }

;; _LXp_movx: 000088F4
_LXp_movx proc
	{ r3 = asl(r1,#0x3); r1 = r2; allocframe(#0x8) }
	{ call memcpy; r16 = r0; r2 = r3; memd(r29) = r17:r16 }
	{ r0 = r16; r17:r16 = memd(r29); dealloc_return }

;; _LXp_addx: 00008910
_LXp_addx proc
	{ call __save_r16_through_r23; allocframe(#0x20) }
	{ r17:r16 = combine(r0,r3); r19:r18 = combine(r2,r1) }
	{ p0 = cmp.gt(r8,#0x0); if (!p0.new) jump:nt 0000894C; if (p0.new) r20 = #0x0 }

l00008928:
	{ r23:r22 = memd(gp+192) }

l0000892C:
	{ r3:r2 = memd(r19) }
	{ p0 = dfcmp.eq(r3:r2,r23:r22); if (p0.new) jump:nt 0000894C; if (!p0.new) r19 = add(r19,#0x8); if (!p0.new) r1:r0 = combine(r18,r17) }

l00008940:
	{ call _LXp_addh }
	{ r20 = add(r20,#0x1); if (cmp.gtu(r16,r20.new)) jump:t 0000892C }

l0000894C:
	{ immext(#0x19C0); r0 = r9; jump fn00008A1C }

l00008950:
	{ r0 = r9; jump fn00008A20 }

;; _LXp_subx: 00008954
_LXp_subx proc
	{ call __save_r16_through_r23; allocframe(#0x20) }
	{ r17:r16 = combine(r0,r3); r19:r18 = combine(r2,r1) }
	{ p0 = cmp.gt(r8,#0x0); if (!p0.new) jump:nt fn00008994; if (p0.new) r20 = #0x0 }

l0000896C:
	{ r23:r22 = memd(gp+192) }

;; fn00008970: 00008970
;;   Called from:
;;     0000896C (in _LXp_subx)
;;     00008988 (in _LXp_mulh)
fn00008970 proc
	{ r3:r2 = memd(r19) }
	{ p0 = dfcmp.eq(r3:r2,r23:r22); if (p0.new) jump:nt fn00008994; if (!p0.new) r20 = add(r20,#0x1); if (!p0.new) r1:r0 = combine(r18,r17) }

l0000897C:
	{ if (!p0.new) r20 = add(r20,#0x1); if (!p0.new) r1:r0 = combine(r18,r17) }

l00008980:
	{ if (!p0.new) r1:r0 = combine(r18,r17) }

l00008984:
	{ r3 = togglebit(r3,#0x1E); call _LXp_addh }
	{ p0 = cmp.gt(r8,r12); if (p0.new) jump:t fn00008970; r19 = add(r19,#0x8) }

;; fn00008994: 00008994
;;   Called from:
;;     00008964 (in _LXp_subx)
;;     00008974 (in fn00008970)
;;     00008988 (in _LXp_mulh)
;;     0000899C (in fn00008970)
fn00008994 proc
	{ immext(#0x1980); r0 = r9; jump 00008A44; nop }

;; _LXp_ldexpx: 000089A0
_LXp_ldexpx proc
	{ call __save_r16_through_r23; allocframe(#0x20) }
	{ r17:r16 = combine(r1,r0); r19:r18 = combine(#0xFFFFFFFF,r2); r21:r20 = memd(gp+192) }
	{ r22 = r16; nop; nop }

l000089C0:
	{ r23 = add(r22,#0x8); r2 = r18; r19 = add(r19,#0x1) }
	{ call ldexpl; r1:r0 = memd(r22) }
	{ p0 = dfcmp.eq(r1:r0,r21:r20); if (!p0.new) jump:t 000089C0; r22 = r23; memd(r22) = r1:r0 }

l000089E8:
	{ immext(#0x1940); r0 = r8; jump fn00008A48 }

;; _LXp_mulx: 000089F0
;;   Called from:
;;     00008B70 (in fn00008B64)
;;     00008B8C (in fn00008B64)
;;     00008CCC (in fn00008CCC)
;;     00008CCC (in fn00008CCC)
;;     00008CDC (in fn00008CCC)
;;     00008CDC (in fn00008CCC)
;;     00008CF8 (in fn00008CCC)
;;     00008CF8 (in fn00008CCC)
;;     00008D10 (in _LXp_sqrtx)
_LXp_mulx proc
	{ call __save_r16_through_r27; allocframe(#0x30) }
	{ r17:r16 = combine(r1,r0); r19:r18 = combine(r3,r2); r20 = r4 }
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:t fn00008AB8 }

l00008A08:
	{ p0 = cmp.eq(r11,#0x0); if (p0.new) jump:t fn00008AB8 }

l00008A0C:
	{ r3:r2 = memd(r18); r25:r24 = memd(gp+192) }
	{ p0 = dfcmp.eq(r3:r2,r25:r24); if (p0.new) jump:t fn00008A2C }

;; fn00008A1C: 00008A1C
;;   Called from:
;;     0000894C (in _LXp_addx)
;;     00008A14 (in _LXp_mulx)
fn00008A1C proc
	{ r1:r0 = memd(r18+8) }

;; fn00008A20: 00008A20
;;   Called from:
;;     00008950 (in _LXp_addx)
;;     00008A1C (in fn00008A1C)
fn00008A20 proc
	{ p0 = dfcmp.eq(r1:r0,r25:r24); if (!p0.new) jump:nt fn00008A3C; if (!p0.new) r1:r0 = combine(r16,r20) }

;; fn00008A2C: 00008A2C
;;   Called from:
;;     00008A14 (in _LXp_mulx)
;;     00008A20 (in fn00008A20)
;;     00008A20 (in fn00008A20)
fn00008A2C proc
	{ call _LXp_mulh; r1:r0 = combine(r17,r16) }
	{ immext(#0x1900); r0 = r8; jump fn00008AA4 }

;; fn00008A3C: 00008A3C
;;   Called from:
;;     00008A20 (in fn00008A20)
;;     00008A20 (in fn00008A20)
fn00008A3C proc
	{ r21 = asl(r17,#0x3) }
	{ call memcpy; r2 = r21 }

l00008A44:
	{ r2 = r21 }

;; fn00008A48: 00008A48
;;   Called from:
;;     000089E8 (in _LXp_ldexpx)
;;     00008A40 (in fn00008A3C)
;;     00008A44 (in fn00008994)
fn00008A48 proc
	{ call _LXp_mulh; r1:r0 = combine(r17,r16); r3:r2 = memd(r18) }
	{ p0 = cmp.gt(r11,#0x1); if (!p0.new) jump:t fn00008AB8; if (p0.new) r23 = #0x1 }

;; fn00008A5C: 00008A5C
;;   Called from:
;;     00008A54 (in fn00008A48)
;;     00008A54 (in fn00008A48)
;;     00008A54 (in fn00008A48)
;;     00008AA4 (in fn00008AA4)
;;     00008AB8 (in fn00008AB8)
fn00008A5C proc
	{ r22 = addasl(r20,r17,#0x3) }
	{ r26 = addasl(r18,r23,#0x3) }
	{ r1:r0 = memd(r26) }
	{ p0 = dfcmp.eq(r1:r0,r25:r24); if (p0.new) jump:nt fn00008AB8; if (!p0.new) r2 = add(r21,#0x0); if (!p0.new) r1:r0 = combine(r20,r22) }

l00008A78:
	{ call memcpy }
	{ call _LXp_mulh; r1:r0 = combine(r17,r22); r27:r26 = combine(r22,#0x0); r3:r2 = memd(r26) }
	{ p0 = cmp.gt(r9,#0x0); if (!p0.new) jump:t 00008AB0 }

l00008A90:
	{ r3:r2 = memd(r27) }
	{ p0 = dfcmp.eq(r3:r2,r25:r24); if (p0.new) jump:nt 00008AB0; if (!p0.new) r27 = add(r27,#0x8); if (!p0.new) r1:r0 = combine(r17,r16) }

;; fn00008AA4: 00008AA4
;;   Called from:
;;     00008A34 (in fn00008A2C)
;;     00008A9C (in fn00008A5C)
;;     00008A9C (in fn00008A5C)
fn00008AA4 proc
	{ call _LXp_addh }
	{ r26 = add(r26,#0x1); if (cmp.gtu(r17,r26.new)) jump:t 00008A90 }

l00008AB0:
	{ r23 = add(r23,#0x1) }

l00008AB4:
	{  }

;; fn00008AB8: 00008AB8
;;   Called from:
;;     00008A04 (in _LXp_mulx)
;;     00008A08 (in _LXp_mulx)
;;     00008A54 (in fn00008A48)
;;     00008A70 (in fn00008A5C)
;;     00008AB0 (in fn00008AA4)
;;     00008AB4 (in fn00008AA4)
fn00008AB8 proc
	{ immext(#0x1880); r0 = r8; jump 00008B18 }

;; _LXp_invx: 00008AC0
_LXp_invx proc
	{ call __save_r16_through_r27; allocframe(#0x30) }
	{ r17:r16 = combine(r1,r0); r22 = r2 }
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt fn00008BFC; if (!p0.new) r0 = add(r16,#0x0) }

l00008AD8:
	{ call _LDtest }
	{ p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:nt 00008B00 }

l00008AE0:
	{ p0 = cmph.eq(r0,#0x0); if (!p0.new) jump:t fn00008BD0 }

l00008AE8:
	{ immext(#0xE600); r0 = #0xE620 }
	{ r1:r0 = memd(r0) }
	{ immext(#0x1840); r0 = r8; jump fn00008B64; memd(r16) = r1:r0 }

l00008B00:
	{ r19 = addasl(r22,r17,#0x3); r20 = asl(r17,#0x3); r25:r24 = memd(gp+224); r27:r26 = memd(r16) }
	{ call memcpy; r1:r0 = combine(r16,r19); r2 = r20 }

l00008B18:
	{ r2 = r20 }

;; fn00008B1C: 00008B1C
;;   Called from:
;;     00008B10 (in _LXp_invx)
;;     00008B18 (in fn00008AA4)
fn00008B1C proc
	{ call _LXp_mulh; r1:r0 = combine(r17,r19); r3:r2 = combine(r25,r24) }
	{ p0 = cmp.gt(r9,#0x1); if (!p0.new) jump:t fn00008BE8; if (!p0.new) r3:r2 = combine(r27,r26); if (!p0) r1:r0 = combine(r27,r26) }

;; fn00008B34: 00008B34
;;   Called from:
;;     00008B28 (in fn00008B1C)
;;     00008B28 (in fn00008B1C)
;;     00008B64 (in fn00008B64)
;;     00008BD0 (in fn00008BD0)
fn00008B34 proc
	{ call __hexagon_adddf3; r3:r2 = memd(r16+8); r25:r24 = memd(gp+216) }
	{ call __hexagon_divdf3; r3:r2 = combine(r1,r0); r1:r0 = combine(r25,r24) }
	{ r3:r2 = combine(r1,r0); r1:r0 = combine(r17,r16); r24 = #0x1; r27:r26 = memd(gp+192) }
	{ r21 = addasl(r22,r17,#0x4); call _LXp_setw }

;; fn00008B64: 00008B64
;;   Called from:
;;     00008AF4 (in _LXp_invx)
;;     00008B68 (in fn00008B34)
fn00008B64 proc
	{ call memcpy; r1:r0 = combine(r16,r22); r2 = r20 }
	{ call _LXp_mulx; r3:r2 = combine(r17,r19); r4 = r21; r1:r0 = combine(r17,r22) }
	{ call _LXp_addh; r1:r0 = combine(r17,r22); r3:r2 = memd(gp+216) }
	{ call _LXp_mulx; r3:r2 = combine(r17,r16); r1:r0 = combine(r17,r22); r4 = r21 }
	{ r25 = #0x0; r18 = r22 }
	{ r3:r2 = memd(r18) }
	{ p0 = dfcmp.eq(r3:r2,r27:r26); if (p0.new) jump:nt 00008BC4; if (!p0.new) r18 = add(r18,#0x8); if (!p0.new) r1:r0 = combine(r17,r16) }

l00008BB8:
	{ call _LXp_addh }
	{ r25 = add(r25,#0x1) }

l00008BC4:
	{ r24 = asl(r24,#0x1); if (cmp.gtu(r17,r24.new)) jump:t fn00008B64 }

;; fn00008BD0: 00008BD0
;;   Called from:
;;     00008AE0 (in _LXp_invx)
;;     00008BC4 (in fn00008B64)
fn00008BD0 proc
	{ r0 = zxth(r0); if (!cmp.eq(r0.new,#0x1)) jump:t fn00008BFC }

l00008BDC:
	{ immext(#0x1740); r0 = r8; jump fn00008CAC; memd(r16) = r1:r0 }

;; fn00008BE8: 00008BE8
;;   Called from:
;;     00008B28 (in fn00008B1C)
;;     00008B28 (in fn00008B1C)
;;     00008BD0 (in fn00008BD0)
;;     00008BFC (in fn00008BFC)
fn00008BE8 proc
	{ call __hexagon_divdf3; r1:r0 = memd(gp+216) }
	{ call _LXp_setw; r3:r2 = combine(r1,r0); r1:r0 = combine(r17,r16) }

;; fn00008BFC: 00008BFC
;;   Called from:
;;     00008AD0 (in _LXp_invx)
;;     00008BF0 (in fn00008BE8)
fn00008BFC proc
	{ immext(#0x1740); r0 = r8; jump fn00008C4C }

;; _LXp_sqrtx: 00008C04
_LXp_sqrtx proc
	{ call __save_r16_through_r27; allocframe(#0x30) }
	{ r17:r16 = combine(r1,r0); r18 = r2 }
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt fn00008D20; if (!p0.new) r0 = add(r16,#0x0) }

l00008C1C:
	{ call _LDtest }
	{ p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:t 00008C34; r5:r4 = memd(r16) }

l00008C28:
	{ r1:r0 = memd(gp+192) }
	{ p0 = dfcmp.ge(r5:r4,r1:r0); if (p0.new) jump:t 00008C60 }

l00008C34:
	{ r1:r0 = memd(gp+192) }
	{ p0 = dfcmp.ge(r5:r4,r1:r0); if (p0.new) jump:t fn00008D20; if (!p0.new) r0 = #0x1 }

l00008C44:
	{ call _Feraise }
	{ immext(#0xE600); r0 = #0xE630 }

;; fn00008C4C: 00008C4C
;;   Called from:
;;     00008BFC (in fn00008BFC)
;;     00008C54 (in _LXp_sqrtx)
fn00008C4C proc
	{ r0 = #0x30 }
	{ r1:r0 = memd(r0) }
	{ immext(#0x16C0); r0 = r8; jump fn00008D44; memd(r16) = r1:r0 }

l00008C60:
	{ r19 = addasl(r18,r17,#0x3); p0 = cmp.gt(r9,#0x1); if (!p0.new) jump:t 00008C78; if (!p0) r1:r0 = combine(r5,r4) }

l00008C6C:
	{ call __hexagon_adddf3; r3:r2 = memd(r16+8) }
	{ r5:r4 = combine(r1,r0) }

l00008C78:
	{ call sqrtl; r1:r0 = combine(r5,r4); r21:r20 = memd(gp+216) }
	{ call __hexagon_divdf3; r3:r2 = combine(r1,r0); r1:r0 = combine(r21,r20) }
	{ call _LXp_setw; r20 = addasl(r18,r17,#0x4); r3:r2 = combine(r1,r0); r1:r0 = combine(r17,r19) }
	{ p0 = cmp.gt(r9,#0x2); if (!p0.new) jump:t 00008D10; if (p0.new) r26 = #0x2 }

l00008CA8:
	{ r21 = asl(r17,#0x3); r23:r22 = memd(gp+232); r25:r24 = memd(gp+240) }

;; fn00008CAC: 00008CAC
;;   Called from:
;;     00008BDC (in fn00008BD0)
;;     00008CB0 (in _LXp_sqrtx)
fn00008CAC proc
	{ r23:r22 = memd(gp+232); r25:r24 = memd(gp+240) }

;; fn00008CB4: 00008CB4
;;   Called from:
;;     00008CAC (in fn00008CAC)
;;     00008D08 (in fn00008CCC)
;;     00008D08 (in fn00008CCC)
;;     00008D14 (in fn00008D14)
fn00008CB4 proc
	{ call memcpy; r1:r0 = combine(r19,r18); r2 = r21 }
	{ call _LXp_mulh; r1:r0 = combine(r17,r18); r3:r2 = combine(r23,r22) }

;; fn00008CCC: 00008CCC
;;   Called from:
;;     00008CC0 (in _LXp_sqrtx)
;;     00008CC0 (in fn00008CB4)
fn00008CCC proc
	{ call _LXp_mulx; r4 = r20; r1:r0 = combine(r17,r18); r3:r2 = combine(r17,r16) }
	{ call _LXp_mulx; r3:r2 = combine(r17,r19); r1:r0 = combine(r17,r18); r4 = r20 }
	{ call _LXp_addh; r1:r0 = combine(r17,r18); r3:r2 = combine(r25,r24) }
	{ call _LXp_mulx; r1:r0 = combine(r17,r19); r3:r2 = combine(r17,r18); r4 = r20 }
	{ r26 = asl(r26,#0x1); if (cmp.gtu(r17,r26.new)) jump:t fn00008CB4 }

l00008D10:
	{ call _LXp_mulx; r1:r0 = combine(r17,r16); r4 = r20; r3:r2 = combine(r17,r19) }

;; fn00008D14: 00008D14
;;   Called from:
;;     00008D08 (in fn00008CCC)
;;     00008D08 (in fn00008CCC)
fn00008D14 proc
	{ r1:r0 = combine(r17,r16); r4 = r20; r3:r2 = combine(r17,r19) }

;; fn00008D20: 00008D20
;;   Called from:
;;     00008C14 (in _LXp_sqrtx)
;;     00008C38 (in _LXp_sqrtx)
;;     00008D10 (in _LXp_sqrtx)
;;     00008D14 (in fn00008D14)
fn00008D20 proc
	{ immext(#0x1600); r0 = r8; jump fn00008DE0 }
00008D28                         00 C0 00 7F 00 C0 00 7F         ........

;; _Mbtowcx: 00008D30
;;   Called from:
;;     00008F5C (in _Mbtowc)
_Mbtowcx proc
	{ p0 = cmp.eq(r1,#0x0); allocframe(#0x0) }
	{ r5 = memw(r3); r6 = memh(r3+6) }
	{ r7 = memw(r4) }
	{ if (p0) jump:nt 00008EFC; if (!p0) r7 = add(r1,#0x0); if (!p0) r8 = #0x0 }

;; fn00008D44: 00008D44
;;   Called from:
;;     00008C54 (in fn00008C4C)
;;     00008C54 (in fn00008C4C)
fn00008D44 proc
	{ if (!p0) r7 = add(r1,#0x0); if (!p0) r8 = #0x0 }

;; fn00008D4C: 00008D4C
;;   Called from:
;;     00008D44 (in fn00008D44)
;;     00008D44 (in fn00008D44)
;;     00008D44 (in fn00008D44)
;;     00008DE0 (in fn00008DE0)
;;     00008DEC (in fn00008DE0)
fn00008D4C proc
	{ p0 = cmp.eq(r2,#0x0); if (p0.new) jump:nt 00008EDC }

l00008D50:
	{ p0 = cmpb.gtu(r6,#0xF); if (p0.new) jump:nt 00008EEC; if (!p0.new) r8 = add(r8,#0x1); if (!p0.new) r6 = zxtb(r6) }

l00008D60:
	{ immext(#0xFC0); p0 = cmp.gt(r8,#0xFEF); r6 = memw(r6+r6<<#2); if (cmp.eq(r6.new,#0x0)) jump:nt 00008EEC }

l00008D74:
	{ if (!p0) r13 = add(r5,#0x0); if (!p0) r12 = memb(r7) }
	{ r6 = memuh(r4+r12<<#1); if (cmp.eq(r6.new,#0x0)) jump:nt 00008EEC }

l00008D88:
	{ r15 = and(r6,#0x0); r14 = and(r6,#0xFF) }
	{ r13 = or(r14,and(r13,#0xFFFFFF00)); immext(#0x4000); r9 = and(r6,#0x4000); p0 = cmp.eq(r15,#0x0) }
	{ immext(#0x1000); r14 = and(r6,#0x1000); p1 = cmp.eq(r9,#0x0); if (!p0) r5 = add(r13,#0x0) }
	{ r9 = asl(r5,#0x8); p0 = cmp.eq(r14,#0x0) }
	{ r9 |= lsr(r5,#0x18); if (p1) jump:nt 00008DDC }

l00008DC0:
	{ p2 = !cmp.eq(r12,00000000); p1 = cmp.eq(r12,#0x0); if (!p1.new) r7 = add(r7,#0x1); if (p2.new) r13 = #0xFFFFFFFF }
	{ if (!p2) r13 = #0x0; if (!p1) r8 = #0x0 }
	{ r2 = add(r13,r2) }

l00008DDC:
	{ r6 = extractu(r6,#0x4,#0xC); immext(#0x2000); r12 = and(r6,#0x2000); if (!p0) r5 = add(r9,#0x0) }

;; fn00008DE0: 00008DE0
;;   Called from:
;;     00008D20 (in fn00008D20)
;;     00008DDC (in fn00008D4C)
fn00008DE0 proc
	{ immext(#0x2000); r12 = and(r6,#0x2000); if (!p0) r5 = add(r9,#0x0) }
	{ if (p0.new) jump:t fn00008D4C; if (!p0.new) r2 = sub(r7,r1); p0 = cmp.eq(r12,#0x0) }

l00008DF8:
	{ p1 = cmp.eq(r7,r1); p0 = cmp.eq(r0,#0x0); memw(r3) = r5 }
	{ p0 = cmp.eq(r5,#0x0); if (p1) r0 = #0xFFFFFFFD; if (!p1) r0 = add(r2,#0x0) }
	{ r0 = #-0x1; memuh(r3+8) = r6 }
	{ dealloc_return }
00008E18                         7E C0 01 10 60 40 02 10         ~...`@..
00008E20 24 E0 81 74 00 40 06 DD 16 58 20 5C E7 FF 24 97 $..t.@...X \..$.
00008E30 05 50 07 76 78 C0 03 24 05 DC 07 76 1E 58 20 5C .P.vx..$...v.X \
00008E40 26 60 00 7E 00 D8 05 75 44 40 00 58 E5 C3 07 76 &`.~...uD@.X...v
00008E50 08 D8 07 76 4C 48 20 5C E6 7F 06 74 00 D0 08 75 ...vLH \...t...u
00008E60 00 40 06 DD E7 C7 07 76 C7 C6 45 8E 32 40 20 5C .@.....v..E.2@ \
00008E70 05 C0 67 70 56 C0 00 58 05 DE 07 76 0A 58 20 5C ..gpV..X...v.X \
00008E80 46 60 00 7E 00 DC 05 75 24 40 00 58 E5 C1 07 76 F`.~...u$@.X...v
00008E90 05 DF 07 76 0A 58 20 5C 66 60 00 7E 00 DE 05 75 ...v.X \f`.~...u
00008EA0 18 40 00 58 E5 C0 07 76 85 DF 07 76 0A 58 20 5C .@.X...v...v.X \
00008EB0 A6 60 00 7E 80 DF 05 75 0C 40 00 58 65 C0 07 76 .`.~...u.@.Xe..v
00008EC0 16 48 20 5C 86 60 00 7E 00 DF 05 75 65 C0 07 76 .H \.`.~...ue..v
00008ED0 24 40 04 B0 E2 7F E2 BF AA E0 72 24             $@........r$    

l00008EDC:
	{ r2 = #0xFFFFFFFE; r0 = and(r6,#0xFF); memw(r3) = r5 }
	{ r0 = r2; memuh(r3+8) = r0 }
	{ dealloc_return }

l00008EEC:
	{ call _Geterrno }
	{ r2 = #0xFFFFFFFF; memw(r0) = #0x58 }
	{ r0 = r2; dealloc_return }

l00008EFC:
	{ memw(r3+4) = #0x0; memw(r3) = #0x0 }
	{ r0 = memw(r4) }
	{ r0 = memuh(r0) }
	{ immext(#0xF00); r2 = and(r0,#0xF00) }
	{ r0 = r2; dealloc_return }
00008F14             02 40 00 78 31 10 30 F0 40 3F 20 50     .@.x1.0.@? P
00008F20 00 40 00 75 02 40 00 78 00 C7 80 46 F8 40 37 10 .@.u.@.x...F.@7.
00008F30 82 64 21 FB 80 C1 23 3C 40 3F 20 50             .d!...#<@? P    

;; _Mbtowc: 00008F3C
;;   Called from:
;;     000057C4 (in _Printf)
_Mbtowc proc
	{ memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ call _Tls_get__Mbstate; r17:r16 = combine(r0,r3); r19:r18 = combine(r1,r2); memd(r29) = r19:r18 }
	{ r3:r2 = combine(r16,r18); r1:r0 = combine(r19,r17); r4 = r0; r17:r16 = memd(r29+8) }
	{ jump _Mbtowcx; r19:r18 = memd(r29); deallocframe }
00008F64             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; _Tls_get__Wcstate: 00008F70
;;   Called from:
;;     00006E14 (in _Wctomb)
;;     0000901C (in _Getpwcstate)
_Tls_get__Wcstate proc
	{ r1 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ immext(#0x10040); r16 = #0x10054; immext(#0x10040); r17 = #0x10058 }
	{ memd(r29) = r19:r18 }

l00008F8C:
	{ r0 = memw_locked(r16) }
	{ p0 = cmp.gt(r0,#0x0) }
	{ if (p0) jump:nt 00008FA0 }

l00008F98:
	{ memw_locked(r16,p0) = r1 }
	{ if (!p0) jump:nt 00008F8C }

l00008FA0:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 00008FC0; if (p0.new) r18 = #0x2 }

l00008FA8:
	{ immext(#0x10040); r0 = #0x10058; immext(#0x6FC0); r1 = #0x6FF0 }
	{ call sys_Tlsalloc }
	{ memw(r16) = r18 }

l00008FC0:
	{ r0 = memw(r16); if (!cmp.gt(r0.new,#0x1)) jump:t 00008FC0 }

l00008FCC:
	{ r0 = memw(r17) }
	{ r1:r0 = combine(#0x40,#0x1); r16 = r0; if (!cmp.eq(r16.new,#0x0)) jump:t 00009010 }

l00008FE0:
	{ r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 00009010 }

l00008FEC:
	{ r1 = r16; r0 = memw(r17) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00009000 }

l00008FF4:
	{ call free; r16 = #0x0; r0 = r16 }
	{ jump 00009010 }

l00009000:
	{ call __hexagon_memcpy_likely_aligned_min32bytes_mult8bytes; immext(#0xD740); r1:r0 = combine(#0xD748,r16); r2 = #0x40 }

l00009010:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; _Getpwcstate: 0000901C
_Getpwcstate proc
	{ jump _Tls_get__Wcstate }

;; _Atrealloc: 00009020
;;   Called from:
;;     00006F40 (in atexit)
;;     000095A4 (in _Atexit)
_Atrealloc proc
	{ memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r17 = memw(gp+4) }
	{ r0 = r17 }
	{ r0 += lsr(r0,#0x1) }
	{ r0 = asl(r0,#0x2); call malloc }
	{ r1 = #0x0; r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 000090B4 }

l00009048:
	{ r0 = r16; r2 = memw(gp+64); r1 = memw(gp+8) }
	{ r2 = asl(r2,#0x2); call memcpy }
	{ r0 = memw(gp+12); r3 = memw(gp+4) }
	{ r2 = add(r0,r17); r3 = sub(r3,r0); r1 = memw(gp+8) }
	{ r1 = addasl(r1,r0,#0x2); r0 = addasl(r16,r2,#0x2) }
	{ r2 = asl(r3,#0x2); call memcpy }
	{ r0 = memw(gp+8) }
	{ immext(#0xE940); r1 = #0xE940 }
	{ call free }
	{ r1 = #0x1; r0 = memw(gp+12); r2 = memw(gp+4) }
	{ r3 = add(r0,r17); r0 = add(r2,r17); memw(gp+512) = r16 }
	{ memw(gp+96) = r3; memw(gp) = r0 }

l000090B4:
	{ r0 = r1; r17:r16 = memd(r29); dealloc_return }
000090BC                                     00 C0 00 7F             ....

;; _Closreg: 000090C0
;;   Called from:
;;     00007A80 (in _Fwprep)
_Closreg proc
	{ r1 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ immext(#0x10040); r16 = #0x1005C }

l000090D0:
	{ r0 = memw_locked(r16) }
	{ p0 = cmp.gt(r0,#0x0) }
	{ if (p0) jump:nt 000090E4 }

l000090DC:
	{ memw_locked(r16,p0) = r1 }
	{ if (!p0) jump:nt 000090D0 }

l000090E4:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 00009108; if (p0.new) r0 = #0x2 }

l000090EC:
	{ call _Locksyslock }
	{ call _Atexit; immext(#0x9100); r0 = #0x9114 }
	{ call _Unlocksyslock; r0 = #0x2; r17 = #0x2 }
	{ memw(r16) = r17 }

l00009108:
	{ r0 = memw(r16); if (!cmp.gt(r0.new,#0x1)) jump:t 00009108 }

;; closeall: 00009114
;;   Called from:
;;     00009108 (in closeall)
;;     00009108 (in _Closreg)
closeall proc
	{ immext(#0xE500); r17:r16 = combine(#0xE514,#0x38); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r0 = memw(r16); if (cmp.eq(r0.new,#0x0)) jump:t 0000912C }

l0000912C:
	{ r16 = add(r16,#0x4); r17 = add(r17,#0xFFFFFFFF) }
	{ r17:r16 = memd(r29); dealloc_return }
0000913C                                     00 C0 00 7F             ....

;; fclose: 00009140
fclose proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r0 = memb(r16); memd(r29) = r19:r18 }
	{ r0 = and(r0,#0x3); if (cmp.eq(r0.new,#0x0)) jump:nt 000091D0 }

l00009158:
	{ if (!cmp.gt(r0.new,#-0x1)) jump:nt 000091D4 }

l00009160:
	{ r0 = r16 }
	{ call _Locksyslock; r0 = #0x2; r17 = r0 }
	{ r0 = memb(r16) }
	{ r0 = and(r0,#0x40); if (cmp.eq(r0.new,#0x0)) jump:nt 00009180 }

l0000917C:
	{ r0 = memw(r16+8) }

l00009180:
	{ call close; r0 = memw(r16+4); memw(r16+8) = #0x0 }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) r17 = #0xFFFFFFFF; r1 = memw(r16+64) }
	{ call remove; r0 = r1 }
	{ r18 = r0; r1 = memw(r16+64) }
	{ call free; r0 = r1 }
	{ p0 = cmp.eq(r18,#0x0); if (!p0.new) r17 = #0xFFFFFFFF; memw(r16+64) = #0x0 }
	{ call _Fofree; r0 = r16 }
	{ call _Unlocksyslock; r0 = #0x2 }
	{ jump 000091F4 }

l000091D0:
	{ call _Locksyslock; r0 = #0x2 }

l000091D4:
	{ r0 = #0x2 }

l000091D8:
	{ call _Fofree; r0 = r16 }
	{ call _Unlocksyslock; r0 = #0x2 }
	{ call _Geterrno; r17 = #0xFFFFFFFF }
	{ memw(r0) = #0x9 }

l000091F4:
	{ r0 = r17; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; fflush: 00009200
;;   Called from:
;;     000071A4 (in fwrite)
;;     000071D8 (in fwrite)
;;     0000933C (in fputc)
;;     00009424 (in fputs)
fflush proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ p0 = cmp.eq(r8,#0x0); if (p0.new) jump:nt 0000926C; if (!p0.new) r0 = add(r16,#0x0); memd(r29) = r19:r18 }

l00009214:
	{ call _Lockfilelock }
	{ r0 = memuh(r16) }
	{ immext(#0x2000); r1 = and(r0,#0x2000) }
	{ r17 = memw(r16+8) }
	{ r1 = memw(r16+16) }
	{ call write; r2 = sub(r1,r17); r1 = r17; r0 = memw(r16+4) }
	{ p0 = cmp.gt(r0,#0x0); if (!p0.new) jump:nt 000092C4; if (p0.new) r17 = add(r17,r0) }

l00009248:
	{ r1 = memw(r16+16) }
	{ r17 = memw(r16+8); r0 = memh(r16) }
	{ immext(#0xDFC0); r1 = and(r0,#0xDFFF); r0 = r16; memw(r16+16) = r17 }
	{ memw(r16+52) = r17; memw(r16+24) = r17 }
	{ jump 000092B0; memuh(r16) = r1 }

l0000926C:
	{ call _Locksyslock; r0 = #0x2; immext(#0xE500); r17:r16 = combine(#0xE500,#0x38) }
	{ r18 = #0x14 }

l00009280:
	{ r0 = memw(r16); if (cmp.eq(r0.new,#0x0)) jump:t 00009294 }

l0000928C:
	{ p0 = cmp.gt(r0,#0xFFFFFFFF); if (!p0.new) r17 = #0xFFFFFFFF }

l00009294:
	{ r0 = #0x2; r16 = add(r16,#0x4); r18 = add(r18,#0xFFFFFFFF); if (!cmp.eq(r18.new,#0x0)) jump:t 00009280 }

l000092A8:
	{ jump 000092B8 }
000092AC                                     00 C0 70 70             ..pp

l000092B0:
	{ r17 = #0x0 }

l000092B4:
	{ call _Unlockfilelock }

l000092B8:
	{ r0 = r17; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

l000092C4:
	{ r0 = r16; r17 = #0xFFFFFFFF; r1 = memw(r16+8); r2 = memh(r16) }
	{ r2 = setbit(r2,#0x12); memw(r16+16) = r1; memw(r16+52) = r1 }
	{ jump 000092B4; memw(r16+24) = r1; memuh(r16+8) = r2 }

;; fputc: 000092E0
;;   Called from:
;;     000073A8 (in puts)
fputc proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ call _Lockfilelock; r0 = r17 }
	{ r0 = memw(r17+16) }
	{ r1 = memw(r17+24) }
	{ call _Fwprep; r0 = r17 }
	{ p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:nt 00009348; if (p0.new) r0 = memw(r17+16) }

l0000930C:
	{ p0 = cmpb.eq(r16,#0xA); r1 = add(r0,#0x1) }
	{ memb(r0) = r16 }
	{ r0 = memuh(r17) }
	{ immext(#0x800); r1 = and(r0,#0x800) }
	{ immext(#0x400); r0 = and(r0,#0x400); if (cmp.eq(r0.new,#0x0)) jump:nt 00009350 }

l0000933C:
	{ call fflush; r0 = r17 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00009350 }

l00009348:
	{ r0 = r9; jump 00009354; r16 = #0xFFFFFFFF }

l00009350:
	{ r0 = r17; r16 = and(r16,#0xFF) }

l00009354:
	{ call _Unlockfilelock }
	{ r0 = r16; r17:r16 = memd(r29); dealloc_return }

;; fputs: 00009360
;;   Called from:
;;     00007394 (in puts)
;;     000077D8 (in raise)
;;     00007850 (in raise)
;;     0000786C (in raise)
fputs proc
	{ r17:r16 = combine(r0,r1); memd(r29-16) = r17:r16; allocframe(#0x20) }
	{ call _Lockfilelock; r0 = r16; memd(r29+16) = r19:r18 }
	{ jump 00009378 }

l00009374:
	{ r17 = add(r17,r18) }

l00009378:
	{ r0 = memb(r17); if (cmp.eq(r0.new,#0x0)) jump:nt 00009430 }

l00009384:
	{ r0 = r16; r1 = memw(r16+24) }
	{ call _Fwprep }
	{ p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:nt 00009448 }

l00009398:
	{ r0 = memb(r16+1) }
	{ r0 = and(r0,#0x4); if (cmp.eq(r0.new,#0x0)) jump:t 000093B0 }

l000093A8:
	{ r1:r0 = combine(#0xA,r17) }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 000093CC }

l000093B0:
	{ call strlen; r0 = r17 }
	{ p0 = or(p0,!p0); r3 = r0 }
	{ r1 = p0; jump 000093DC }

l000093CC:
	{ p0 = and(p0,p0); r3 = add(r0,sub(#0x41,r17)) }
	{ r0 = p0; memb(r29+2) = r0.new }

l000093DC:
	{ r1 = r17; r0 = memw(r16+16); r2 = memw(r16+24) }

l000093E0:
	{ r0 = memw(r16+16); r2 = memw(r16+24) }
	{ r4 = sub(r2,r0) }
	{ r18 = minu(r3,r4); p0 = cmp.gtu(r3,r4) }
	{ r3 = p0; call memcpy; r2 = r18 }
	{ r0 = memw(r16+16); r1 = memd(r29+4) }
	{ p0 = r1; if (p0.new) jump:t 00009374; r0 = add(r0,r18); memb(r16+4) = r0.new }

l00009418:
	{ p0 = r0; if (p0.new) jump:t 00009374; if (!p0.new) r0 = add(r16,#0x0) }

l00009424:
	{ call fflush }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:t 00009374 }

l0000942C:
	{ jump 00009448 }

l00009430:
	{ r0 = memb(r16+1) }
	{ r0 = and(r0,#0x8); if (cmp.eq(r0.new,#0x0)) jump:nt 00009450 }

l00009440:
	{ r0 = r16 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00009450 }

l00009448:
	{ r0 = r8; jump 00009454; r16 = #0xFFFFFFFF }

l00009450:
	{ r16 = #0x0; r0 = r16 }

l00009454:
	{ call _Unlockfilelock }
	{ r0 = r16; r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ dealloc_return }
00009464             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; ldexpl: 00009470
;;   Called from:
;;     000089D0 (in _LXp_ldexpx)
ldexpl proc
	{ r16 = r2; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ p0 = cmp.eq(r8,#0x0); if (p0.new) jump:nt 000094AC; if (!p0.new) r0 = add(r29,#0x0); memd(r29) = r1:r0 }

l00009484:
	{ call _LDtest }
	{ p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:t 000094AC }

l0000948C:
	{ call _LDscale; r0 = add(r29,#0x0); r1 = r16 }
	{ p0 = cmp.eq(r0,#0x1); if (!p0.new) jump:t 000094A0; jump 000094A8; if (p0.new) r0 = #0x4 }

l000094A0:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 000094AC; if (p0.new) r0 = #0x8 }

l000094A8:
	{ call _Feraise }

l000094AC:
	{ r1:r0 = memd(r29); r17:r16 = memd(r29+8) }
	{ dealloc_return }
000094B4             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; ldiv: 000094C0
ldiv proc
	{ call __hexagon_divsi3; r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r2 = mpyi(r0,r17) }
	{ r1 = sub(r16,r2); r17:r16 = memd(r29); dealloc_return }
000094D8                         00 C0 00 7F 00 C0 00 7F         ........

;; close: 000094E0
;;   Called from:
;;     00009180 (in fclose)
close proc
	{ jump __sys_close }
000094E4             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; write: 000094F0
;;   Called from:
;;     00009234 (in fflush)
write proc
	{ call __sys_write; r16 = r2; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r0 = sub(r16,r0); r17:r16 = memd(r29); dealloc_return }
00009504             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; remove: 00009510
;;   Called from:
;;     00009198 (in fclose)
remove proc
	{ jump __sys_remove }
00009514             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; sqrtl: 00009520
;;   Called from:
;;     00008C78 (in _LXp_sqrtx)
sqrtl proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ call _LDunscale; r0 = add(r29,#0x6); r1 = add(r29,#0x8); memd(r29+8) = r17:r16 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00009558 }

l00009538:
	{ p0 = cmp.eq(r0,#0x1); if (p0.new) jump:t 00009540 }

l0000953C:
	{ p0 = cmp.eq(r0,#0x2); if (p0.new) jump:t 00009558 }

l00009540:
	{ r0 = add(r29,#0x8) }
	{ r0 = or(r0,#0x6) }
	{ r0 = memh(r0); if (cmp.gt(r0.new,#-0x1)) jump:t 00009558 }

l00009554:
	{ r0 = #0x1 }

l00009558:
	{ call fn0000ADC0; r1:r0 = combine(r17,r16) }
	{ r17:r16 = memd(r29+16); dealloc_return }
00009564             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; strrchr: 00009570
strrchr proc
	{ r2 = #0x0; r1 = and(r1,#0xFF) }

l00009574:
	{ r3 = r0 }
	{ r4 = memb(r3++#1) }
	{ p1 = cmp.eq(r4,r1); if (p1.new) r2 = add(r0,#0x0); r0 = r3; p0 = cmp.eq(r4,#0x0) }
	{ if (!p0) jump:nt 00009574 }

l0000958C:
	{ r0 = r2; jumpr r31 }

;; _Atexit: 00009590
;;   Called from:
;;     000090F0 (in _Closreg)
_Atexit proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r2 = memw(gp+64) }
	{ r1 = memw(gp+12) }
	{ call _Atrealloc }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 000095C4 }

l000095AC:
	{ r1 = memw(gp+12) }
	{ r0 = add(r1,#0xFFFFFFFF); r1 = memw(gp+8) }
	{ memw(gp) = r0; memw(r30+r0<<#2) = r16 }
	{ r17:r16 = memd(r29); dealloc_return }

l000095C4:
	{ call abort }
	{ nop }
	{ nop }

;; _Dunscale: 000095D0
;;   Called from:
;;     00008100 (in _LDunscale)
;;     000095CC (in _Atexit)
;;     0000B318 (in sqrt)
_Dunscale proc
	{ r17:r16 = combine(r0,r1); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r0 = r16; r2 = memh(r16+6) }
	{ r1 = extractu(r2,#0xB,#0x3) }
	{ call _Dnorm }
	{ r0 = #0x0; r1 = r0 }
	{ jump 00009664; memuh(r17) = #0x0 }
000095FC                                     00 C0 C1 70             ...p
00009600 40 40 00 78 E3 7F 03 78 22 E0 43 20 00 C0 31 3C @@.x...x".C ..1<
00009610 C1 C0 30 91 E1 41 01 76 28 E0 43 24 41 40 70 91 ..0..A.v(.C$A@p.
00009620 24 E0 43 24 21 40 70 91 20 E0 43 24 00 C0 70 91 $.C$!@p. .C$..p.
00009630 1A 40 00 58 10 40 00 75 40 60 00 7E 20 E0 80 7E .@.X.@.u@`.~ ..~
00009640 62 C0 50 91 03 7C 1F 78 41 40 C1 BF FF 43 00 00 b.P..|.xA@...C..
00009650 E0 C7 00 78 00 42 00 00 E3 41 42 DA 03 CA B0 A1 ...x.B...AB.....
00009660 00 C1 51 A1                                     ..Q.            

l00009664:
	{ r0 = sxth(r0); r17:r16 = memd(r29); dealloc_return }
0000966C                                     00 C0 00 7F             ....

;; _Feraise: 00009670
;;   Called from:
;;     00008C44 (in _LXp_sqrtx)
;;     000094A8 (in ldexpl)
_Feraise proc
	{ r1 = setbit(r0,#0x8); r2 = and(r0,#0xC); allocframe(#0x8) }
	{ if (p0.new) r16 = add(r0,#0x0); if (!p0.new) r16 = add(r1,#0x0); p0 = cmp.eq(r2,#0x0); memd(r29) = r17:r16 }
	{ call feraiseexcept; r0 = r16; r17 = and(r16,#0x3) }
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt 000096A8 }

l00009698:
	{ call _Geterrno }
	{ r17:r16 = memd(r29); memw(r0) = #0x21 }
	{ dealloc_return }

l000096A8:
	{ r0 = and(r16,#0xC); if (cmp.eq(r0.new,#0x0)) jump:nt 000096B8 }

l000096B4:
	{ memw(r0) = #0x22 }

l000096B8:
	{ r17:r16 = memd(r29); dealloc_return }
000096BC                                     00 C0 00 7F             ....

;; _Fofind: 000096C0
_Fofind proc
	{ immext(#0xE500); r17:r16 = combine(#0xE538,#0x0); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ p0 = cmp.gtu(r8,#0x13); if (p0.new) jump:nt 000096F4 }

l000096D0:
	{ r0 = memw(r17); if (cmp.eq(r0.new,#0x0)) jump:nt 000096FC }

l000096DC:
	{ r16 = add(r16,#0x1); r1 = memuh(r0) }
	{ jump 0000971C; immext(#0xFF40); memuh(r0+65344) = #0x3F }

l000096F4:
	{ r0 = #0x0; r17:r16 = memd(r29); dealloc_return }

l000096FC:
	{ call malloc; r0 = #0x50 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000971C }

l00009708:
	{ memw(r17) = r0; memb(r0+2) = r16 }
	{ r17:r16 = memd(r29); immext(#0x80); memuh(r0+128) = #0x0 }
	{ dealloc_return }

l0000971C:
	{ r17:r16 = memd(r29); dealloc_return }

;; _Fofree: 00009720
;;   Called from:
;;     000091BC (in fclose)
;;     000091D8 (in fclose)
_Fofree proc
	{ r2 = add(r0,#0x4C); allocframe(#0x0) }
	{ r1 = memb(r0) }
	{ r1 = and(r1,#0x80) }
	{ nop; immext(#0xE500); r2 = #0xE534; r1 = #-0x1 }

l00009740:
	{ r1 = add(r1,#0x1) }
	{ r2 = add(r2,#0x4); r4 = memw(r2+4) }
	{ p0 = cmp.eq(r4,r0); if (!p0.new) jump:t 00009740; if (p0.new) r3 = add(r2,#0x0) }

l00009754:
	{ memw(r3) = #0x0 }
	{ call free }
	{ dealloc_return }
00009760 81 45 00 B0 FF 60 40 3C 00 C0 20 3C 02 06 02 AD .E...`@<.. <....
00009770 02 05 02 AC 02 02 02 A4 01 08 02 A7 1E C0 1E 96 ................

;; _Genld: 00009780
_Genld proc
	{ call __save_r16_through_r23; allocframe(#0x30) }
	{ call localeconv; r20 = r1; r19:r18 = combine(r4,r3); r17:r16 = combine(r2,r0) }
	{ r1 = setbit(r20,#0xA); immext(#0xD780); r3:r2 = combine(#0xD788,#0x1); r0 = memw(r0+44) }
	{ r18 = max(r2,r18); p0 = cmp.gt(r18,#0x0); p1 = cmp.eq(r1,#0x66); if (!p0.new) r17 = add(r3,#0x0) }
	{ if (p1) jump:nt 000097E0; r21 = memb(r0) }

l000097C0:
	{ p0 = cmp.eq(r20,#0x67); p2 = cmp.eq(r20,#0x47) }
	{ p3 = or(p0,p2); if (!p3.new) jump:t 000097FC }

l000097D0:
	{ if (!p3.new) jump:t 000097FC; p3 = cmp.gt(r19,#0xFFFFFFFB) }

l000097D8:
	{ r0 = memw(r16+48); if (!cmp.gt(r0.new,r19)) jump:t 000097FC }

l000097E0:
	{ if (p1) jump:nt 00009868; r20 = add(r19,#0x1); if (!p1) r0 = memb(r16-4) }

l000097E4:
	{ r20 = add(r19,#0x1); if (!p1) r0 = memb(r16-4) }

l000097EC:
	{ r0 = and(r0,#0x8); if (cmp.eq(r0.new,#0x0)) jump:nt 00009844 }

l000097F8:
	{ r0 = memw(r16+48) }

l000097FC:
	{ if (p2) jump:nt 00009810; if (!p2) r22 = #0x70 }

l00009804:
	{ if (p1.new) jump:t 000099D4; if (!p0) jump:nt 000099C8; p1 = cmp.eq(r20,#0x61) }

l00009810:
	{ r0 = memw(r16+48); if (!cmp.gt(r0.new,r18)) jump:t 0000982C }
	{ r1 = and(r1,#0x8) }
	{ p1 = cmp.eq(r1,#0x0); if (p1.new) r0 = add(r18,#0x0); if (p1.new) memw(r16+48) = r18 }
	{ if (p0) r22 = #0x65; if (!p0) r22 = #0x45; r0 = add(r0,#0xFFFFFFFF); r1 = #0x0 }
	{ r0 = max(r1,r0); jump 000099D4; memb(r16+12) = r0.new }

l00009844:
	{ r0 = memw(r16+48) }
	{ p0 = cmp.gt(r0,r18); if (p0.new) r0 = add(r18,#0x0); if (p0.new) memw(r16+48) = r18 }
	{ r1 = sxth(r20) }
	{ r0 = sub(r0,r1) }
	{ p0 = cmp.gt(r0,#0xFFFFFFFF); memw(r16+48) = r0; if (!p0.new) memw(r16+48) = #0x0 }

l00009868:
	{ r19 = sxth(r20) }
	{ p0 = cmp.gt(r19,r18); r0 = memw(r16+28); r1 = memw(r16+16) }
	{ if (!p0) jump:nt 00009958; r0 = add(r1,r0) }

l00009880:
	{ call memcpy; r1 = r17; r2 = r18 }
	{ r2 = sub(r19,r18); r1 = memw(r16+28); r0 = memw(r16+16) }
	{ p0 = cmp.gt(r0,#0x0); r1 = add(r1,r18); memw(r16+32) = r2 }
	{ if (p0) jump:nt 000098AC; if (!p0) r2 = memb(r16-4); memw(r16+28) = r1 }

l000098A4:
	{ r2 = and(r2,#0x8); if (cmp.eq(r2.new,#0x0)) jump:nt 000098C4 }

l000098AC:
	{ r0 = memw(r16+16) }

l000098B0:
	{ r0 = add(r0,r1) }
	{ memb(r0) = r21 }
	{ r1 = memw(r16+4); r0 = memw(r16+16) }
	{ r1 = add(r1,#0x1) }

l000098C4:
	{ jump 00009B18; memw(r16+40) = r0 }
000098CC                                     82 04 80 07             ....
000098D0 21 40 00 B0 00 40 02 F3 07 D5 B0 A1 30 C0 00 3C !@...@......0..<
000098E0 82 41 90 91 0A E0 82 24 80 C7 30 91 00 41 00 76 .A.....$..0..A.v
000098F0 10 C0 02 24 82 04 80 07 21 40 00 B0 00 40 02 F3 ...$....!@...@..
00009900 07 D5 B0 A1 00 D5 00 A1 82 C1 90 91 00 40 53 76 .............@Sv
00009910 03 40 42 76 84 04 81 07 00 42 40 F2 00 41 04 F3 .@Bv.....B@..A..
00009920 01 40 71 70 03 E0 94 74 03 C0 E3 70 04 42 03 F3 .@qp...t...p.B..
00009930 03 C0 43 76 02 44 B2 D5 83 08 84 AC 11 C0 E2 70 ..Cv.D.........p
00009940 50 6D FF 5B 89 09 92 70 80 C1 90 91 E6 40 00 58 Pm.[...p.....@.X
00009950 00 40 31 F3 0A D2 B0 A1                         .@1.....        

l00009958:
	{ call memcpy; r1 = r17; r2 = r19 }
	{ r2 = sub(r18,r20); r0 = memw(r16+28); r3 = memw(r16+16) }
	{ p0 = cmp.gt(r3,#0x0); if (p0.new) jump:t 00009980; r0 = add(r0,r19); memb(r16+7) = r0.new }

l00009978:
	{ r1 = and(r1,#0x8) }

l00009980:
	{ r3 = r0; r1 = memw(r16+16) }
	{ r0 = add(r0,r1); memw(r16+28) = r3 }
	{ memb(r0) = r21 }
	{ r3 = memw(r16+16); r0 = memw(r16+28) }
	{ r1 = add(r17,r19); r5 = sxth(r2); r4 = memw(r16+16) }
	{ p0 = cmp.gt(r5,r3); r0 = add(r4,r0); if (p0.new) r2 = add(r3,#0x0) }
	{ r17 = sxth(r2) }
	{ call memcpy; r2 = r17 }
	{ r0 = memw(r16+28); r1 = memw(r16+16) }
	{ r0 = add(r0,r17); r1 = sub(r1,r17); memb(r16+7) = r0.new }
	{ memw(r16+32) = r1 }

l000099C8:
	{ p0 = cmp.eq(r20,#0x41); r22 = r20 }
	{ if (p0) r22 = #0x50 }

l000099D4:
	{ r0 = memw(r16+28); r1 = memw(r16+16) }
	{ r3 = add(r0,#0x1); r0 = add(r1,r0); r2 = memb(r17++#1) }
	{ memb(r0) = r2 }
	{ r0 = memw(r16+48); if (cmp.gt(r0.new,#0x0)) jump:t 00009A00 }

l000099F8:
	{ r0 = and(r0,#0x8); if (cmp.eq(r0.new,#0x0)) jump:nt 00009A5C }

l00009A00:
	{ r0 = memw(r16+28); r2 = memw(r16+16) }

l00009A04:
	{ r1 = add(r0,#0x1); r0 = add(r2,r0) }
	{ memb(r0) = r21 }
	{ r2 = memw(r16+48); if (!cmp.gt(r2.new,#0x0)) jump:nt 00009A5C }

l00009A20:
	{ r1 = memw(r16+28); r5 = memw(r16+16) }
	{ r4 = add(r18.l,r0.l); r0 = add(r5,r1); r1 = r17 }
	{ p0 = cmp.gt(r4,r2); if (!p0.new) r2 = add(r18,#0xFFFFFFFF) }
	{ r17 = sxth(r2) }
	{ call memcpy; r2 = r17 }
	{ r0 = memw(r16+16); r1 = memw(r16+28) }
	{ r20 = add(r1,r17); r0 = sub(r0,r17); memb(r16+7) = r20.new }
	{ memw(r16+32) = r0 }

l00009A5C:
	{ r20 = memw(r16+28) }
	{ p0 = cmp.gt(r19,#0xFFFFFFFF); r21 = r20; r17 = memw(r16+16) }
	{ r21 += add(r17,#0x2); r0 = add(r17,r20) }
	{ if (!p0) jump:nt 00009A80; memb(r0++#1) = r22 }

l00009A78:
	{ jump 00009A88; memb(r0) = #0x2B }

l00009A80:
	{ r19 = sub(#0x0,r19); memb(r0) = #0x2D }

l00009A88:
	{ p0 = cmp.gt(r11,#0x0); if (!p0.new) jump:nt 00009AB4; if (p0.new) r23 = add(r29,#0x0); r18 = #0x0 }

l00009A94:
	{ call div; r1:r0 = combine(#0xA,r19); r18 = add(r18,#0x1) }
	{ r23 = r23; memb(r23) = r1 }
	{ r19 = sxth(r0) }
	{ p0 = cmp.gt(r10,#0x1); if (p0.new) jump:t 00009AD0; if (p0.new) r17 = add(r21,#0x0) }

l00009AB4:
	{ r0 = setbit(r22,#0xA) }
	{ if (!p0.new) jump:nt 00009AD4; if (!p0.new) r17 = add(r21,#0x0); p0 = cmp.eq(r0,#0x65) }

l00009AC4:
	{ r17 += add(r20,#0x3); jump 00009AD4; memb(r21) = #0x30 }

l00009AD0:
	{ jump 00009ADC }

l00009AD4:
	{ p0 = cmp.eq(r10,#0x0); if (p0.new) jump:nt 00009B04 }

l00009AD8:
	{ p0 = cmp.gt(r10,#0x0); if (!p0.new) jump:nt 00009B0C }

l00009ADC:
	{ loop0(00009AE8,r18); r1 = r17; r0 = add(r29,#0x0); r2 = r18 }
	{ r3 = add(r0,r2); r2 = add(r2,#0xFFFFFFFF) }
	{ r3 = memb(r3-1) }
	{ r3 = add(r3,#0x30) }
	{ jump 00009B0C; r17 = add(r17,r18) }

l00009B04:
	{ r0 = #0x30; memb(r17++#1) = r0.new }

l00009B0C:
	{ r0 = memw(r16+28); r1 = memw(r16+16) }

l00009B10:
	{ r17 -= add(r1,r0) }

l00009B18:
	{ r0 = memuh(r16+60) }
	{ r0 = and(r0,#0x14); if (!cmp.eq(r0.new,#0x10)) jump:t 00009B44 }

l00009B28:
	{ r3 = memw(r16); r0 = memw(r16+8) }
	{ r3 += add(r2,r1); r5 = memw(r16+4); r4 = memw(r16+24) }
	{ r0 += add(r3,r5) }
	{ p0 = cmp.gt(r4,r0); if (p0.new) r0 = sub(r4,r0); if (p0.new) memw(r16+24) = r0.new }

l00009B44:
	{ jump __restore_r16_through_r23_and_deallocframe }

l00009B48:
	{ nop }
	{ nop }

;; _LDint: 00009B50
;;   Called from:
;;     000083AC (in _LXp_setw)
;;     000083FC (in _LXp_setw)
;;     0000864C (in fn00008468)
;;     00008884 (in _LXp_mulh)
;;     00009B4C (in _Genld)
_LDint proc
	{ call _Dint; allocframe(#0x0) }
	{ dealloc_return }
00009B5C                                     00 C0 00 7F             ....

;; _LDscale: 00009B60
;;   Called from:
;;     000083B4 (in _LXp_setw)
;;     00008404 (in _LXp_setw)
;;     00008658 (in fn00008468)
;;     0000888C (in _LXp_mulh)
;;     0000948C (in ldexpl)
_LDscale proc
	{ call _Dscale; allocframe(#0x0) }
	{ dealloc_return }
00009B6C                                     00 C0 00 7F             ....

;; _LDtest: 00009B70
;;   Called from:
;;     00008484 (in fn00008468)
;;     000087B0 (in _LXp_mulh)
;;     00008AD8 (in _LXp_invx)
;;     00008C1C (in _LXp_sqrtx)
;;     00009484 (in ldexpl)
_LDtest proc
	{ call _Dtest; allocframe(#0x0) }
	{ dealloc_return }
00009B7C                                     00 C0 00 7F             ....

;; div: 00009B80
;;   Called from:
;;     00009A94 (in _Genld)
div proc
	{ call __hexagon_divsi3; r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r2 = mpyi(r0,r17) }
	{ r1 = sub(r16,r2); r17:r16 = memd(r29); dealloc_return }
00009B98                         00 C0 00 7F 00 C0 00 7F         ........

;; feraiseexcept: 00009BA0
;;   Called from:
;;     00009688 (in _Feraise)
feraiseexcept proc
	{ memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r0 = add(r29,#0x4); r16 = and(r0,#0x1F); if (cmp.eq(r16.new,#0x0)) jump:nt 00009BD8 }

l00009BB4:
	{ r0 = add(r29,#0x4); r1 = memd(r29+4) }
	{ r1 |= asl(r16,#0x1); call fesetenv }
	{ r0 = memw(r29+4) }
	{ r16 &= lsr(r0,#0x19) }
	{ r0 = r16 }

l00009BD8:
	{ r0 = #0x0; r17:r16 = memd(r29+8); dealloc_return }

;; _Force_raise: 00009BE0
_Force_raise proc
	{ r3:r2 = memd(gp+192); allocframe(#0x10) }
	{ r17:r16 = combine(#0x5,r0); memd(r29+8) = r17:r16; memd(r29) = r19:r18 }
	{ immext(#0xD780); r18 = #0xD798 }
	{ r0 = memw(r18-8) }
	{ r0 = and(r0,r16); if (cmp.eq(r0.new,#0x0)) jump:t 00009C14 }

l00009C08:
	{ r1:r0 = memd(r18); r3:r2 = memd(r18+8) }
	{ r3:r2 = combine(r1,r0) }

l00009C14:
	{ r1:r0 = combine(r3,r2); r18 = add(r18,#0x18); r17 = add(r17,#0xFFFFFFFF) }
	{ r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; fesetenv: 00009C2C
;;   Called from:
;;     00009BB8 (in feraiseexcept)
fesetenv proc
	{ r1 = memw(r0) }
	{ USR = r1 }
	{ r0 = #0x0; jumpr r31 }
00009C38                         00 C0 00 7F 00 C0 00 7F         ........

;; _Tls_get__Locale: 00009C40
;;   Called from:
;;     00009CEC (in localeconv)
_Tls_get__Locale proc
	{ r1 = #0x1; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ immext(#0x10040); r16 = #0x10060; immext(#0x10040); r17 = #0x10064 }
	{ memd(r29) = r19:r18 }

l00009C5C:
	{ r0 = memw_locked(r16) }
	{ p0 = cmp.gt(r0,#0x0) }
	{ if (p0) jump:nt 00009C70 }

l00009C68:
	{ memw_locked(r16,p0) = r1 }
	{ if (!p0) jump:nt 00009C5C }

l00009C70:
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:t 00009C90; if (p0.new) r18 = #0x2 }

l00009C78:
	{ immext(#0x10040); r0 = #0x10064; immext(#0x6FC0); r1 = #0x6FF0 }
	{ call sys_Tlsalloc }
	{ memw(r16) = r18 }

l00009C90:
	{ r0 = memw(r16); if (!cmp.gt(r0.new,#0x1)) jump:t 00009C90 }

l00009C9C:
	{ r0 = memw(r17) }
	{ r1:r0 = combine(#0x50,#0x1); r16 = r0; if (!cmp.eq(r16.new,#0x0)) jump:t 00009CE0 }

l00009CB0:
	{ r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 00009CE0 }

l00009CBC:
	{ r1 = r16; r0 = memw(r17) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 00009CD0 }

l00009CC4:
	{ call free; r16 = #0x0; r0 = r16 }
	{ jump 00009CE0 }

l00009CD0:
	{ call __hexagon_memcpy_likely_aligned_min32bytes_mult8bytes; immext(#0xD800); r1:r0 = combine(#0xD808,r16); r2 = #0x50 }

l00009CE0:
	{ r0 = r16; r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ dealloc_return }

;; localeconv: 00009CEC
;;   Called from:
;;     00009788 (in _Genld)
localeconv proc
	{ jump _Tls_get__Locale }

;; _Dint: 00009CF0
;;   Called from:
;;     00009B50 (in _LDint)
_Dint proc
	{ r3 = #0x7FF; r2 = memuh(r0+6) }
	{ r4 = extractu(r2,#0xB,#0x3); r3 = #0x2; if (!cmp.eq(r4.new,r3)) jump:t 00009D30 }

l00009D08:
	{  }
	{ r1 = memuh(r0+4) }
	{ r1 = memuh(r0+2) }
	{ r0 = memuh(r0) }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) r3 = #0x1; if (!p0.new) r3 = #0x2 }
	{ r0 = sxth(r3); jumpr r31 }

l00009D30:
	{ immext(#0x7FC0); r3 = and(r2,#0x7FFF) }
	{ r3 = memuh(r0+4) }
	{ r3 = memuh(r0+2) }
	{ r3 = #0x0; r5 = memuh(r0) }
	{ immext(#0x400); r5 = sub(#0x433,r4); r3 = #0x0 }
	{ r4 = sub(r5.l,r1.l); r1 = sub(r5,r1); if (!cmp.gt(r4.new,#0x0)) jump:nt 00009E18 }

l00009D74:
	{ if (!p0.new) jump:t 00009DA4; immext(#0xFFC0); if (p0.new) r3 = #0xFFFF; p0 = cmp.gt(r1,#0x34) }

l00009D84:
	{ immext(#0x8000); r1 = and(r2,#0x8000); memuh(r0) = #0x0; memuh(r0+4) = #0x0 }
	{ r0 = sxth(r3); jumpr r31; memuh(r0+2) = #0xFF80; memuh(r0+6) = r1 }

l00009DA4:
	{ r2 = asr(r4,#0x4); r1 = and(r4,#0xF) }
	{ r1 = extractu(r4,#0x10,#0x0); immext(#0xD840); r5 = memuh(r1<<#1+0000D868) }
	{ p0 = cmp.eq(r1,#0x1); immext(#0xD880); r3 = memw(r2<<#2+0000D888) }
	{ r1 = memuh(r12+r3<<#1) }
	{ r4 = r1; r1 = and(r1,r5) }
	{ XOREQ r4,and(r4,r5); if (p0) jump:nt 00009DFC }

l00009DE0:
	{ p0 = cmp.eq(r2,#0x3); if (!p0.new) jump:t 00009E08 }

l00009DE4:
	{ r2 = memh(r0+4); memuh(r0+4) = #0x0 }
	{ r1 = or(r2,r1) }
	{ r2 = memh(r0+2); memuh(r0+2) = #0xFF80 }
	{ r1 = or(r2,r1) }

l00009DFC:
	{ r2 = memh(r0); memuh(r0) = #0x0 }
	{ r1 = or(r2,r1) }

l00009E08:
	{ r0 = zxth(r1) }
	{ p0 = !cmp.eq(r0,00000000); if (p0.new) r3 = #0xFFFFFFFF; if (!p0.new) r3 = #0x0 }

l00009E18:
	{ r0 = sxth(r3); jumpr r31 }
00009E1C                                     00 C0 00 7F             ....

;; _Dnorm: 00009E20
;;   Called from:
;;     000095E4 (in _Dunscale)
_Dnorm proc
	{ r1 = memuh(r0+6) }
	{ r3 = and(r1,#0xF); immext(#0x8000); r1 = and(r1,#0x8000) }
	{ p0 = cmp.eq(r3,#0x0); if (!p0.new) jump:t 00009E58; memuh(r0+6) = r3 }

l00009E38:
	{ r3 = #0x0; r2 = memuh(r0+4); if (!cmp.eq(r2.new,#0x0)) jump:t 00009E58 }

l00009E48:
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 00009E5C }

l00009E50:
	{ r4 = memuh(r0); if (cmp.eq(r4.new,#0x0)) jump:nt 00009F60 }

l00009E58:
	{ r1 = #0x1; jump 00009E78; nop }

l00009E5C:
	{ nop }

l00009E60:
	{ r2 = add(r2,#0xFFFFFFF0); r3 = memh(r0+4); r5 = memh(r0+2) }
	{ r4 = memh(r0); memuh(r0) = #0x0 }
	{ memuh(r0+8) = r3; memuh(r0) = r5 }
	{ memuh(r0+2) = r4 }

l00009E78:
	{ p0 = cmph.eq(r3,#0x0); if (p0.new) jump:t 00009E60 }

l00009E80:
	{ p0 = cmph.gtu(r3,#0xF); if (p0.new) jump:t 00009EF4; if (p0.new) r4 = add(r3,#0x0) }

l00009E8C:
	{ r7 = #0x10; r5 = memh(r0); r8 = memh(r0+2) }
	{ r9 = memh(r0+4); r6 = memw(gp+132) }

l00009EA0:
	{ r13 = and(r5,r6); r4 = and(r9,r6); r12 = and(r8,r6); r2 = add(r2,#0xFFFFFFFF) }
	{ r4 = lsr(r4,#0xF); r12 = lsr(r12,#0xF) }
	{ r13 = lsr(r13,#0xF); r4 |= asl(r3,#0x1) }
	{ r12 |= asl(r9,#0x1); r13 |= asl(r8,#0x1); r14 = zxth(r4); r3 = r4 }
	{ r5 = asl(r5,#0x1); if (p0.new) jump:t 00009EA0; r9:r8 = combine(r12,r13); p0 = cmp.gtu(r7,r14) }

l00009EE0:
	{ memuh(r0+2) = r13; memuh(r0+6) = r4 }
	{ jump 00009EF4; memuh(r0+4) = r12; memuh(r0) = r5 }

l00009EF4:
	{ r3 = zxth(r4) }
	{ r5 = #0x20 }
	{ r3 = memh(r0); r13 = memh(r0+2) }
	{ r12 = memh(r0+4); r5 = memw(gp+136) }

l00009F10:
	{ r7 = asl(r4,#0xF); r9 = and(r12,r5); r8 = and(r13,r5); r2 = add(r2,#0x1) }
	{ r6 = asl(r12,#0xF); r3 = asl(r13,#0xF); r12 = and(r3,r5); r13 = zxth(r4) }
	{ r7 |= lsr(r9,#0x1); r6 |= lsr(r8,#0x1) }
	{ p0 = cmph.gtu(r4,#0x3F); r3 |= lsr(r12,#0x1) }
	{ r4 = lsr(r13,#0x1); if (p0) jump:nt 00009F10; r13:r12 = combine(r6,r7) }

l00009F4C:
	{ memuh(r0+8) = r7; memuh(r0) = r6 }
	{ memuh(r0+8) = r3; memuh(r0) = r4 }
	{ r3 = and(r4,#0xF) }
	{ r3 = or(r3,r1); r1 = sxth(r2) }

l00009F60:
	{ r1 = sxth(r2) }

l00009F68:
	{ r0 = r1; jumpr r31 }
00009F6C                                     00 C0 00 7F             ....

;; _Dscale: 00009F70
;;   Called from:
;;     00009B60 (in _LDscale)
_Dscale proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r2 = memh(r16+6) }
	{ r0 = extractu(r2,#0xB,#0x3); if (!cmp.eq(r0.new,#0x0)) jump:t 00009F98 }

l00009F88:
	{ r0 = r16 }
	{ p0 = cmp.gt(r0,#0x0); if (p0.new) jump:t 0000A1D8; r1 = #0x0 }

l00009F94:
	{ jump 00009FD4 }

l00009F98:
	{ r1 = zxth(r0) }
	{ r1 = #0x2; r3 = #0x7FF }
	{ r0 = and(r2,#0xF); if (!cmp.eq(r0.new,#0x0)) jump:t 0000A1D8 }

l00009FB4:
	{ if (!cmp.eq(r0.new,#0x0)) jump:t 0000A1DC }

l00009FBC:
	{ if (!cmp.eq(r0.new,#0x0)) jump:t 0000A1DC }

l00009FC4:
	{ jump 0000A1D8; p0 = !cmp.eq(r0,00000000); if (p0.new) r1 = #0x2; if (!p0.new) r1 = #0x1 }

l00009FD4:
	{ p0 = cmp.gt(r9,#0x0); if (!p0.new) jump:nt 00009FE8; r2 = sxth(r0) }

l00009FDC:
	{ immext(#0x7C0); r0 = sub(#0x7FF,r2); if (!cmp.gt(r0.new,r17)) jump:t 0000A04C }

l00009FE8:
	{ r3 = #0xFFFFFFCB; r1 = sub(#0x0,r2); r0 = memuh(r16+6) }

l00009FEC:
	{ r1 = sub(#0x0,r2); r0 = memuh(r16+6) }

l00009FF8:
	{ immext(#0x8000); r0 = and(r0,#0x800F); r2 = add(r2,r17) }
	{ r0 |= asl(r2,#0x4); immext(#0xFFC0); r1 = #0xFFFF; immext(#0xFFC0) }
	{ r1 = and(r0,#0xF); immext(#0x8000); r0 = and(r0,#0x8000) }
	{ r1 = setbit(r1,#0x8); p0 = cmp.gtu(r3,r2); if (p0.new) memuh(r16) = #0x0; if (p0.new) memuh(r16+4) = #0x0 }
	{ if (!p0) jump:nt 0000A06C; r1 = #-0x1; memuh(r16+8) = r1 }

l0000A040:
	{ jump 0000A1D8; memuh(r16+2) = #0xFF80; memuh(r16+6) = r0 }

l0000A04C:
	{ immext(#0xE680); r0 = #0xE690 }
	{ r1:r0 = memd(r0) }
	{ r2 = memh(r16+6); if (cmp.gt(r2.new,#-0x1)) jump:t 0000A064 }

l0000A064:
	{ r1 = #0x1; jump 0000A1D8; memd(r16) = r1:r0 }

l0000A06C:
	{ r4 = aslh(r2); r3 = #0x0; r2 = sxth(r2) }
	{ if (p0.new) jump:t 0000A0F0; if (p0.new) r4 = #0x0; immext(#0xFFF0FFC0); p0 = cmp.gt(r4,#0xFFF0FFFF) }

l0000A084:
	{ r4 = #0x0; r12 = memh(r16+4); r13 = memh(r16+2) }
	{ nop; nop; immext(#0xFFF10000); r6 = #0xFFF10000; r5 = memh(r16) }

l0000A0A0:
	{ immext(#0x100000); r14 = #0x100000; r4 = zxth(r4); r9 = r1 }
	{ r14 += asl(r2,#0x10); r7 = r13; r4 = !cmp.eq(r4,00000000); r13:r12 = combine(r12,r9) }
	{ r4 = or(r5,r4); r2 = asrh(r14); r1 = #0x0; r5 = r7 }
	{ if (p0.new) jump:t 0000A0A0; p0 = cmp.gt(r6,r14) }

l0000A0D4:
	{ r8 = r13; r1 = #0x0; memuh(r16+6) = #0xFF80; memuh(r16+4) = r9 }
	{ jump 0000A0F0; memuh(r16+2) = r8; memuh(r16) = r7 }

l0000A0F0:
	{ r5 = sub(#0x0,r2) }
	{ p0 = cmph.eq(r5,#0x0); if (p0.new) jump:nt 0000A144; if (!p0.new) r8 = zxth(r1) }

l0000A100:
	{ r2 = sub(r3.l,r2.l); r4 = zxth(r4); r3 = memuh(r16+2) }
	{ r1 &= lsr(r8,r2); r7 = sub(#0x10,r2); r6 = memuh(r16+4); r5 = memuh(r16) }
	{ r8 &= asl(r8,r7); r12 &= asl(r6,r7); r4 = !cmp.eq(r4,00000000); memuh(r16+6) = r1 }
	{ r9 &= asl(r3,r7); r4 |= asl(r5,r7) }
	{ r8 |= lsr(r6,r2); r12 |= lsr(r3,r2) }
	{ r9 |= lsr(r5,r2); memuh(r16+4) = r8; memuh(r16+2) = r12 }
	{ memuh(r16) = r9 }

l0000A144:
	{ r1 = or(r1,r0); immext(#0x8000); r3 = #0x8001; r2 = zxth(r4) }
	{  }
	{ if (!p0.new) r2 = memh(r16); memuh(r16+6) = r1 }
	{ immext(#0x8000); r3 = #0x8000 }
	{ r2 = memh(r16) }
	{ p0 = tstbit(r2,#0x0); if (!p0.new) jump:nt 0000A19C }

l0000A170:
	{ r2 = add(r2,#0x1) }
	{ r3 = zxth(r2); memuh(r16+8) = r2 }
	{ p0 = cmp.eq(r3,#0x0); if (!p0.new) jump:t 0000A19C; if (p0.new) r2 = memh(r16+2) }

l0000A180:
	{ r2 = add(r2,#0x1) }
	{ r3 = zxth(r2); memuh(r16+8) = r2 }
	{ p0 = cmp.eq(r3,#0x0); if (!p0.new) jump:t 0000A19C; if (p0.new) r2 = memh(r16+4) }

l0000A190:
	{ r2 = add(r2,#0x1) }
	{ r3 = zxth(r2); memuh(r16+8) = r2 }
	{ p0 = cmp.eq(r3,#0x0); if (p0.new) jump:nt 0000A1C8 }

l0000A19C:
	{ r1 = zxth(r1) }
	{ r0 = memuh(r16+4); if (!cmp.eq(r0.new,#0x0)) jump:t 0000A1D0 }

l0000A1B0:
	{ if (!cmp.eq(r0.new,#0x0)) jump:t 0000A1D4 }

l0000A1B8:
	{ jump 0000A1D8; p0 = cmp.eq(r0,#0x0); immext(#0xFFC0); if (!p0.new) r1 = #0xFFFF }

l0000A1C8:
	{ r0 = add(r1,#0x1); memb(r16+3) = r0.new }

l0000A1D0:
	{ immext(#0xFFC0); r1 = #0xFFFF }

l0000A1D4:
	{ r1 = #0x3F }

l0000A1D8:
	{ r0 = sxth(r1); r17:r16 = memd(r29); dealloc_return }

l0000A1DC:
	{ r17:r16 = memd(r29); dealloc_return }

;; _Dtest: 0000A1E0
;;   Called from:
;;     00009B70 (in _LDtest)
_Dtest proc
	{ r1 = #0x7FF0; r2 = memuh(r0+6) }
	{ r1 = #0x2; r3 = and(r2,r1) }
	{ r2 = and(r2,#0xF); if (!cmp.eq(r2.new,#0x0)) jump:t 0000A25C }

l0000A200:
	{ if (!cmp.eq(r2.new,#0x0)) jump:t fegetenv }

l0000A208:
	{ if (!cmp.eq(r2.new,#0x0)) jump:t fegetenv }

l0000A210:
	{ p0 = !cmp.eq(r0,00000000); if (p0.new) r1 = #0x2; if (!p0.new) r1 = #0x1 }
	{ r0 = r1; jumpr r31 }
0000A220 FF 41 00 00 E1 47 02 76 14 E0 43 24 41 40 60 91 .A...G.v..C$A@`.
0000A230 0E E0 43 24 21 40 60 91 0A E0 43 24 01 40 00 78 ..C$!@`...C$.@.x
0000A240 00 40 60 91 10 C0 02 24 FF 41 00 00 00 C6 02 76 .@`....$.A.....v
0000A250 00 40 00 75 C1 7F 0F 7E E1 FF 8F 7E             .@.u...~...~    

l0000A25C:
	{ r0 = r1; jumpr r31 }

;; fegetenv: 0000A260
;;   Called from:
;;     0000A200 (in _Dtest)
;;     0000A208 (in _Dtest)
fegetenv proc
	{ r1 = USR }
	{ memw(r0) = r1 }
	{ r0 = #0x0; jumpr r31 }
0000A26C                                     00 C0 00 7F             ....

;; __save_r16_through_r27: 0000A270
;;   Called from:
;;     000066C0 (in _Stoulx)
;;     00007AD0 (in _Ldtob)
;;     00008110 (in _Litob)
;;     00008450 (in _LXp_addh)
;;     00008788 (in _LXp_mulh)
;;     000089F0 (in _LXp_mulx)
;;     00008AC0 (in _LXp_invx)
;;     00008C04 (in _LXp_sqrtx)
__save_r16_through_r27 proc
	{ memd(r30-48) = r27:r26; memd(r30-40) = r25:r24 }

;; __save_r16_through_r23: 0000A278
;;   Called from:
;;     00006200 (in _Putstr)
;;     00006320 (in _Puttxt)
;;     00006C10 (in _Wctombx)
;;     00008350 (in _LXp_setw)
;;     00008910 (in _LXp_addx)
;;     00008954 (in _LXp_subx)
;;     000089A0 (in _LXp_ldexpx)
;;     00009780 (in _Genld)
;;     0000A270 (in __save_r16_through_r27)
__save_r16_through_r23 proc
	{ memd(r30-32) = r23:r22; memd(r30-24) = r21:r20 }

;; __save_r16_through_r19: 0000A280
;;   Called from:
;;     0000A278 (in __save_r16_through_r23)
__save_r16_through_r19 proc
	{ nop; jumpr r31; memd(r30-16) = r19:r18; memd(r30-8) = r17:r16 }

;; __save_r16_through_r27_stkchk: 0000A290
__save_r16_through_r27_stkchk proc
	{ memd(r30-48) = r27:r26; memd(r30-40) = r25:r24 }

;; __save_r16_through_r23_stkchk: 0000A298
;;   Called from:
;;     0000A290 (in __save_r16_through_r27_stkchk)
__save_r16_through_r23_stkchk proc
	{ memd(r30-32) = r23:r22; memd(r30-24) = r21:r20 }

;; __save_r16_through_r19_stkchk: 0000A2A0
;;   Called from:
;;     0000A298 (in __save_r16_through_r23_stkchk)
__save_r16_through_r19_stkchk proc
	{ r17 = UGP; memd(r30-16) = r19:r18; memd(r30-8) = r17:r16 }
	{ r16 = memw(r17+68) }
	{ p0 = cmp.gtu(r16,r29); if (!p0.new) jumpr:t r31 }
0000A2B8                         BC 65 FF 5B 00 C0 9D A0         .e.[....

;; __save_r16_through_r25: 0000A2C0
;;   Called from:
;;     00005770 (in _Printf)
;;     000070C0 (in fwrite)
__save_r16_through_r25 proc
	{ memd(r30-40) = r25:r24; memd(r30-32) = r23:r22 }

;; __save_r16_through_r21: 0000A2C8
;;   Called from:
;;     00006958 (in _Lockfilelock)
;;     00006A44 (in _Locksyslock)
;;     0000A2C0 (in __save_r16_through_r25)
__save_r16_through_r21 proc
	{ memd(r30-24) = r21:r20; memd(r30-16) = r19:r18 }

;; __save_r16_through_r17: 0000A2D0
;;   Called from:
;;     0000A2C8 (in __save_r16_through_r21)
__save_r16_through_r17 proc
	{ jumpr r31; memd(r30-8) = r17:r16 }

;; __save_r16_through_r25_stkchk: 0000A2D8
__save_r16_through_r25_stkchk proc
	{ memd(r30-40) = r25:r24; memd(r30-32) = r23:r22 }

;; __save_r16_through_r21_stkchk: 0000A2E0
;;   Called from:
;;     0000A2D8 (in __save_r16_through_r25_stkchk)
__save_r16_through_r21_stkchk proc
	{ memd(r30-24) = r21:r20; memd(r30-16) = r19:r18 }

;; __save_r16_through_r17_stkchk: 0000A2E8
;;   Called from:
;;     0000A2E0 (in __save_r16_through_r21_stkchk)
__save_r16_through_r17_stkchk proc
	{ r17 = UGP; memd(r30-8) = r17:r16 }
	{ r16 = memw(r17+68) }
	{ p0 = cmp.gtu(r16,r29); if (!p0.new) jumpr:t r31 }
0000A2FC                                     9A 65 FF 5B             .e.[
0000A300 00 C0 9D A0                                     ....            

;; __restore_r16_through_r23_and_deallocframe_before_tailcall: 0000A304
__restore_r16_through_r23_and_deallocframe_before_tailcall proc
	{ nop; r23:r22 = memd(r30-32); r21:r20 = memd(r30-24) }

;; __restore_r16_through_r19_and_deallocframe_before_tailcall: 0000A310
;;   Called from:
;;     0000A304 (in __restore_r16_through_r23_and_deallocframe_before_tailcall)
__restore_r16_through_r19_and_deallocframe_before_tailcall proc
	{ jump __restore_r16_through_r17_and_deallocframe_before_tailcall; r19:r18 = memd(r30-16) }

;; __restore_r16_through_r27_and_deallocframe_before_tailcall: 0000A318
__restore_r16_through_r27_and_deallocframe_before_tailcall proc
	{ nop; r27:r26 = memd(r30-48) }

;; __restore_r16_through_r25_and_deallocframe_before_tailcall: 0000A320
;;   Called from:
;;     0000A318 (in __restore_r16_through_r27_and_deallocframe_before_tailcall)
__restore_r16_through_r25_and_deallocframe_before_tailcall proc
	{ r25:r24 = memd(r30-40); r23:r22 = memd(r30-32) }

;; __restore_r16_through_r21_and_deallocframe_before_tailcall: 0000A328
;;   Called from:
;;     0000A320 (in __restore_r16_through_r25_and_deallocframe_before_tailcall)
__restore_r16_through_r21_and_deallocframe_before_tailcall proc
	{ r21:r20 = memd(r30-24); r19:r18 = memd(r30-16) }

;; __restore_r16_through_r17_and_deallocframe_before_tailcall: 0000A330
;;   Called from:
;;     0000A310 (in __restore_r16_through_r19_and_deallocframe_before_tailcall)
;;     0000A328 (in __restore_r16_through_r21_and_deallocframe_before_tailcall)
__restore_r16_through_r17_and_deallocframe_before_tailcall proc
	{ nop; jumpr r31; r17:r16 = memd(r30-8); deallocframe }

;; __restore_r16_through_r23_and_deallocframe: 0000A340
;;   Called from:
;;     00009B44 (in _Genld)
__restore_r16_through_r23_and_deallocframe proc
	{ r23:r22 = memd(r30-32); r21:r20 = memd(r30-24) }

;; __restore_r16_through_r19_and_deallocframe: 0000A348
;;   Called from:
;;     0000A340 (in __restore_r16_through_r23_and_deallocframe)
__restore_r16_through_r19_and_deallocframe proc
	{ jump __restore_r16_through_r17_and_deallocframe; r19:r18 = memd(r30-16) }

;; __restore_r16_through_r27_and_deallocframe: 0000A350
;;   Called from:
;;     00006738 (in _Stoulx)
;;     00006888 (in _Stoul)
;;     00006890 (in _Stoulx)
;;     00007B98 (in _Ldtob)
;;     0000829C (in _Litob)
;;     000082D0 (in _Litob)
;;     000084D4 (in fn00008468)
;;     00008758 (in fn00008468)
;;     00008774 (in fn00008468)
;;     00008780 (in fn00008780)
__restore_r16_through_r27_and_deallocframe proc
	{ r27:r26 = memd(r30-48) }

;; __restore_r16_through_r25_and_deallocframe: 0000A354
;;   Called from:
;;     00007208 (in fwrite)
;;     0000A350 (in __restore_r16_through_r27_and_deallocframe)
__restore_r16_through_r25_and_deallocframe proc
	{ nop; r25:r24 = memd(r30-40); r23:r22 = memd(r30-32) }

;; __restore_r16_through_r21_and_deallocframe: 0000A360
;;   Called from:
;;     00006A08 (in _Lockfilelock)
;;     00006AF4 (in _Locksyslock)
;;     0000A354 (in __restore_r16_through_r25_and_deallocframe)
__restore_r16_through_r21_and_deallocframe proc
	{ r21:r20 = memd(r30-24); r19:r18 = memd(r30-16) }

;; __restore_r16_through_r17_and_deallocframe: 0000A368
;;   Called from:
;;     0000A348 (in __restore_r16_through_r19_and_deallocframe)
;;     0000A360 (in __restore_r16_through_r21_and_deallocframe)
__restore_r16_through_r17_and_deallocframe proc
	{ r17:r16 = memd(r30-8); dealloc_return }

;; __deallocframe: 0000A370
__deallocframe proc
	{ dealloc_return }
0000A374             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; __hexagon_memcpy_likely_aligned_min32bytes_mult8bytes: 0000A380
;;   Called from:
;;     00006694 (in fn00006620)
;;     00009000 (in _Tls_get__Wcstate)
;;     00009CD0 (in _Tls_get__Locale)
__hexagon_memcpy_likely_aligned_min32bytes_mult8bytes proc
	{ p0 = bitsclr(r1,#0x7); p0 = bitsclr(r0,#0x7); r3 = #0xFFFFFFFD; if (p0.new) r5:r4 = memd(r1) }
	{ if (!p0) jump:nt memcpy; r3 += lsr(r2,#0x3); if (p0) r5:r4 = memd(r1+8); if (p0) memd(r0++#8) = r5:r4 }

l0000A3A0:
	{ loop0(0000A3B0,r3); r1 = add(r1,#0x18); r5:r4 = memd(r1+16); memd(r0++#8) = r5:r4 }
	{ r5:r4 = memd(r1++#8); memd(r0++#8) = r5:r4 }
	{ r0 -= add(r2,#0xFFFFFFF8); jumpr r31; memd(r0) = r5:r4 }
0000A3C4             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............
0000A3D0 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F ................

;; __qdsp_divdi3: 0000A3E0
;;   Called from:
;;     000081E0 (in _Litob)
__qdsp_divdi3 proc
	{ p2 = tstbit(r1,#0x1F); p3 = tstbit(r3,#0x1F) }
	{ r1:r0 = abs(r1:r0); r3:r2 = abs(r3:r2) }
	{ r6 = cl0(r1:r0); r7 = cl0(r3:r2); r5:r4 = combine(r3,r2); r3:r2 = combine(r1,r0) }
	{ p3 = xor(p2,p3); r10 = sub(r7,r6); r1:r0 = combine(#0x0,#0x0); r15:r14 = combine(#0x0,#0x1) }
	{ r13:r12 = lsl(r5:r4,r10); r15:r14 = lsl(r15:r14,r10); r11 = add(r10,#0x1) }
	{ loop0(0000A428,r11); p0 = cmp.gtu(r5:r4,r3:r2) }
	{ if (p0) jump:nt 0000A444 }

l0000A428:
	{ p0 = cmp.gtu(r13:r12,r3:r2) }
	{ r7:r6 = sub(r3:r2,r13:r12); r9:r8 = add(r15:r14,r1:r0) }
	{ r1:r0 = vmux(p0,r1:r0,r9:r8); r3:r2 = vmux(p0,r3:r2,r7:r6) }
	{ r15:r14 = lsr(r15:r14,#0x1); r13:r12 = lsr(r13:r12,#0x1) }

l0000A444:
	{ r3:r2 = neg(r1:r0) }
	{ r1:r0 = vmux(p3,r3:r2,r1:r0); jumpr r31 }
0000A450 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F ................

;; __hexagon_divsi3: 0000A460
;;   Called from:
;;     000094C0 (in ldiv)
;;     00009B80 (in div)
__hexagon_divsi3 proc
	{ r1 = abs(r0); r2 = abs(r1); p0 = cmp.gt(r0,#0xFFFFFFFF); p1 = cmp.gt(r1,#0xFFFFFFFF) }
	{ r3 = cl0(r1); r4 = cl0(r2); r5 = sub(r1,r2); p2 = cmp.gtu(r2,r1) }
	{ p1 = xor(p0,p1); if (p2) jumpr:nt r31; r0 = #0x0; p0 = cmp.gtu(r2,r5) }
0000A490 E0 7F 80 7A 04 44 23 F3 C4 3F 13 48 08 41 04 60 ...z.D#..?.H.A.`
0000A4A0 C2 44 02 C3 00 40 00 78 00 C0 00 7F 00 C0 00 7F .D...@.x........
0000A4B0 00 81 62 F2 22 41 42 80 81 61 22 FB 80 E3 00 FB ..b."AB..a".....
0000A4C0 00 41 62 F2 00 41 7F 53 80 E3 00 FB 00 40 40 76 .Ab..A.S.....@@v
0000A4D0 00 C0 9F 52 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F ...R............

;; __hexagon_udivdi3: 0000A4E0
__hexagon_udivdi3 proc
	{ r6 = cl0(r1:r0); r7 = cl0(r3:r2); r5:r4 = combine(r3,r2); r3:r2 = combine(r1,r0) }

;; fn0000A4E4: 0000A4E4
;;   Called from:
;;     000081B4 (in _Litob)
fn0000A4E4 proc
	{ r7 = cl0(r3:r2); r5:r4 = combine(r3,r2); r3:r2 = combine(r1,r0) }
	{ r10 = sub(r7,r6); r1:r0 = combine(#0x0,#0x0); r15:r14 = combine(#0x0,#0x1) }
	{ r13:r12 = lsl(r5:r4,r10); r15:r14 = lsl(r15:r14,r10); r11 = add(r10,#0x1) }
	{ loop0(0000A514,r11); p0 = cmp.gtu(r5:r4,r3:r2) }
	{ if (p0) jumpr:nt r31 }
0000A514             80 C2 8C D2 E6 42 2C D3 E8 CE 00 D3     .....B,.....
0000A520 00 48 00 D1 02 C6 02 D1 2E 81 0E 80 2C C1 0C 80 .H..........,...
0000A530 00 C0 9F 52 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F ...R............

;; __qdsp_udivsi3: 0000A540
;;   Called from:
;;     000071FC (in fwrite)
__qdsp_udivsi3 proc
	{ r2 = cl0(r0); r3 = cl0(r1); r5:r4 = combine(#0x1,#0x0); p0 = cmp.gtu(r1,r0) }
	{ r6 = sub(r3,r2); r1:r0 = combine(r0,r4); r4 = r1; if (p0) jumpr r31 }
0000A55C                                     08 41 06 60             .A.`
0000A560 C2 46 04 C3 00 40 00 7F 00 C0 00 7F 00 C0 00 7F .F...@..........
0000A570 00 81 62 F2 22 41 42 80 81 61 22 FB 80 E3 00 FB ..b."AB..a".....
0000A580 00 41 62 F2 00 40 9F 52 80 E3 00 FB 00 C0 00 7F .Ab..@.R........

;; __qdsp_umoddi3: 0000A590
;;   Called from:
;;     000081A0 (in _Litob)
__qdsp_umoddi3 proc
	{ r6 = cl0(r1:r0); r7 = cl0(r3:r2); r5:r4 = combine(r3,r2); r3:r2 = combine(r1,r0) }
	{ r10 = sub(r7,r6); r1:r0 = combine(#0x0,#0x0); r15:r14 = combine(#0x0,#0x1) }
	{ r13:r12 = lsl(r5:r4,r10); r15:r14 = lsl(r15:r14,r10); r11 = add(r10,#0x1) }
	{ loop0(0000A5C4,r11); p0 = cmp.gtu(r5:r4,r3:r2) }
	{ if (p0) jump:nt 0000A5E0 }

l0000A5C4:
	{ p0 = cmp.gtu(r13:r12,r3:r2) }
	{ r7:r6 = sub(r3:r2,r13:r12); r9:r8 = add(r15:r14,r1:r0) }
	{ r1:r0 = vmux(p0,r1:r0,r9:r8); r3:r2 = vmux(p0,r3:r2,r7:r6) }
	{ r15:r14 = lsr(r15:r14,#0x1); r13:r12 = lsr(r13:r12,#0x1) }

l0000A5E0:
	{ r1:r0 = combine(r3,r2); jumpr r31 }
0000A5E8                         00 C0 00 7F 00 C0 00 7F         ........
0000A5F0 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F ................

;; __hexagon_adddf3: 0000A600
;;   Called from:
;;     0000832C (in _LXp_getw)
;;     00008340 (in _LXp_getw)
;;     000085D8 (in fn00008468)
;;     00008B34 (in fn00008B34)
;;     00008C6C (in _LXp_sqrtx)
;;     0000A6A4 (in __hexagon_fast2_subdf3)
__hexagon_adddf3 proc
	{ r4 = extractu(r1,#0xB,#0x13); r5 = extractu(r3,#0xB,#0x13); immext(#0x20000000); r13:r12 = combine(#0x20000000,#0x0) }
	{ p3 = dfclass(r1:r0,#0x2); p3 = dfclass(r3:r2,#0x2); r9:r8 = combine(r13,r12); p2 = cmp.gtu(r5,r4) }
	{ if (!p3) jump:nt 0000A764; if (p2) r1:r0 = combine(r3,r2); if (p2) r3:r2 = combine(r1,r0); if (p2) r5:r4 = combine(r4,r5) }

l0000A630:
	{ r13:r12 = insert(r0,#0x34,#0xC); r9:r8 = insert(r2,#0x34,#0xC); r15 = sub(r4,r5); r7:r6 = combine(#0x3E,#0x1) }

l0000A640:
	{ r15 = min(r7,r15); r11:r10 = neg(r13:r12); p2 = cmp.gt(r1,#0xFFFFFFFF); r14 = #0x0 }
	{ r11:r10 = extractu(r9:r8,r15:r14); r9:r8 = asr(r9:r8,r15); if (p2.new) r13:r12 = combine(r11,r10); r15:r14 = combine(#0x0,#0x0) }
	{ p1 = cmp.eq(r11:r10,r15:r14); if (!p1.new) r8 = or(r8,r6); r5 = add(r4,#0xFFFFFBC4); p3 = cmp.gt(r3,#0xFFFFFFFF) }
	{ r13:r12 = add(r9:r8,r13:r12); r11:r10 = sub(r13:r12,r9:r8); immext(#0x7C0); r7:r6 = combine(#0x7F6,#0x3D) }
	{ p0 = !cmp.gtu(r4,r6) }
	{ if (p3.new) r13:r12 = combine(r11,r10) }
	{ r1:r0 = convert_d2df(r13:r12); if (p0.new) jump:nt 0000A6B0; p0 = cmp.eq(r13,#0x0); p0 = cmp.eq(r12,#0x0) }

l0000A69C:
	{ r1 += asl(r5,#0x14); jumpr r31 }

;; __hexagon_fast2_subdf3: 0000A6A4
;;   Called from:
;;     000083C8 (in _LXp_setw)
;;     00008410 (in _LXp_setw)
;;     00008664 (in fn00008468)
;;     000088A4 (in _LXp_mulh)
__hexagon_fast2_subdf3 proc
	{ r3 = togglebit(r3,#0x1E); jump __hexagon_adddf3; nop }

l0000A6B0:
	{ r28 = USR; r3 = #0x1; r1:r0 = combine(#0x0,#0x0) }
	{ r28 = extractu(r28,#0x2,#0x12); r3 = asl(r3,#0x1F) }
	{ p0 = cmp.eq(r28,#0x2); jumpr r31; if (p0.new) r1 = xor(r1,r3); nop }
0000A6D0 60 40 EC 80 F0 68 DF 5C 00 40 0D 75 00 C0 0C 75 `@...h.\.@.u...u
0000A6E0 9C 4B 41 8D C1 D4 05 8E 05 5C 05 F3 00 40 01 00 .KA......\...@..
0000A6F0 02 C0 00 7C 1F 40 00 00 C0 47 45 75 16 C8 00 5C ...|.@...GEu...\
0000A700 00 40 45 75 00 58 5F 53 3C C0 45 76 02 74 00 83 .@Eu.X_S<.Ev.t..
0000A710 00 CC 0D F5 42 DC 82 C3 00 7F 02 83 00 C0 9F 52 ....B..........R
0000A720 1C 40 08 6A 00 4C 0D F5 FF 7F FE 07 EC E7 7F 7C .@.j.L.........|
0000A730 C5 42 5C 8D 1C 45 9C 76 00 40 FF 07 08 C0 00 7C .B\..E.v.@.....|
0000A740 08 40 3C 62 25 5F 81 8E 1C C0 65 70 30 40 1C 75 .@<b%_....ep0@.u
0000A750 50 40 05 75 0C E8 09 FD 00 FF 0C 83 00 40 E0 D2 P@.u.........@..
0000A760 00 C0 9F 52                                     ...R            

l0000A764:
	{ r13:r12 = extractu(r1:r0,#0x3F,#0x7); r9:r8 = extractu(r3:r2,#0x3F,#0x7) }
	{ p3 = cmp.gtu(r13:r12,r9:r8); if (!p3.new) r1:r0 = combine(r3,r2); if (!p3.new) r3:r2 = combine(r1,r0) }
	{ p0 = dfclass(r1:r0,#0xF); if (!p0.new) jump:nt 0000A818; if (p3.new) r13:r12 = combine(r9,r8); if (p3.new) r9:r8 = combine(r13,r12) }

l0000A788:
	{ p1 = dfclass(r1:r0,#0x8); if (p1.new) jump:nt 0000A85C }

l0000A790:
	{ p2 = dfclass(r3:r2,#0x1); if (p2.new) jump:nt 0000A830; r13:r12 = combine(#0x0,#0x0) }

l0000A79C:
	{ p0 = dfclass(r1:r0,#0x4); if (p0.new) jump:nt 0000A7C8; immext(#0x20000000); r13:r12 = combine(#0x20000000,#0x0) }

l0000A7AC:
	{ r4 = extractu(r1,#0xB,#0x13); r9:r8 = asl(r9:r8,#0x9); r5 = #0x1 }
	{ r13:r12 = insert(r0,#0x34,#0xC); jump 0000A640; r15 = sub(r4,r5); r7:r6 = combine(#0x3E,#0x1) }

l0000A7C8:
	{ r13:r12 = extractu(r1:r0,#0x3F,#0x7); r9:r8 = extractu(r3:r2,#0x3F,#0x7) }
	{ r13:r12 = neg(r13:r12); r9:r8 = neg(r9:r8); p0 = cmp.gt(r1,#0xFFFFFFFF); p1 = cmp.gt(r3,#0xFFFFFFFF) }
	{ if (p0) r13:r12 = combine(r1,r0); if (p1) r9:r8 = combine(r3,r2) }
	{ r13:r12 = add(r9:r8,r13:r12) }
	{ r9:r8 = neg(r13:r12); p0 = cmp.gt(r13,#0xFFFFFFFF); r3:r2 = combine(#0x0,#0x0) }
	{ if (p0.new) r1:r0 = combine(r9,r8); if (p0) r1:r0 = combine(r13,r12); immext(#0x80000000); r3 = #0x80000000 }
	{ p0 = dfcmp.eq(r1:r0,r3:r2); if (p0.new) jump:nt 0000A838; if (!p0) r1 = or(r1,r3) }

l0000A814:
	{ jumpr r31 }

l0000A818:
	{ r28 = convert_df2sf(r1:r0); p0 = dfclass(r3:r2,#0xF); if (!p0) r3:r2 = combine(r1,r0) }
	{ r2 = convert_df2sf(r3:r2); jumpr r31; r1:r0 = combine(#0xFFFFFFFF,#0xFFFFFFFF) }

l0000A830:
	{ p0 = dfcmp.eq(r13:r12,r1:r0); if (!p0.new) jumpr:t r31 }

l0000A838:
	{ p0 = cmp.eq(r1:r0,r3:r2); if (p0.new) jumpr:t r31 }
0000A840 1C C0 08 6A DC 42 5C 8D 00 C0 00 7C 40 40 1C 75 ...j.B\....|@@.u
0000A850 00 40 9F 52 00 40 00 08 01 E0 00 7E             .@.R.@.....~    

l0000A85C:
	{ p0 = dfclass(r3:r2,#0x8); if (!p0.new) jumpr:t r31; p0 = !cmp.eq(r1,r3) }
0000A868                         00 40 F8 07 22 C0 00 78         .@.."..x
0000A870 00 40 82 84 00 C0 9F 52 00 C0 00 7F 00 C0 00 7F .@.....R........

;; __hexagon_divdf3: 0000A880
;;   Called from:
;;     00008B40 (in fn00008B34)
;;     00008BE8 (in fn00008BE8)
;;     00008C84 (in _LXp_sqrtx)
__hexagon_divdf3 proc
	{ p3 = dfclass(r1:r0,#0x2); p3 = dfclass(r3:r2,#0x2); immext(#0x40000000); r15:r14 = combine(#0x40000000,#0x0) }
	{ if (!p3) jump:nt 0000AA18; r15:r14 = insert(r0,#0x34,#0xC); r13:r12 = combine(r15,r14); r7:r6 = combine(#0x0,#0x0) }

l0000A8A0:
	{ r4 = extractu(r1,#0xB,#0x13); r5 = extractu(r3,#0xB,#0x13); r11:r10 = combine(#0x0,#0x1) }
	{ loop0(0000A8BC,#0x18); r13:r12 = insert(r2,#0x34,#0xC); r9:r8 = combine(r15,r14); r4 = sub(r4,r5) }
	{ r15:r14 = sub(r9:r8,r13:r12) }
	{ p0 = cmp.gt(r15,#0xFFFFFFFF); if (!p0) r9:r8 = combine(r15,r14) }
	{ r7:r6 = add(r7:r6,r7:r6,p0):carry; r9:r8 = asl(r9:r8,#0x1) }
	{ p1 = cmp.gtu(r11:r10,r9:r8); if (!p1.new) r6 = or(r6,r10); r9 = #0xFFFFFC02; r8 = #0x3FE }
	{ r15:r14 = neg(r7:r6); r28 = xor(r1,r3); p0 = cmp.gt(r4,r9); p0 = !cmp.gt(r4,r8) }
	{ if (!p0) jump:nt 0000A914; p1 = cmp.gt(r28,#0xFFFFFFFF); if (!p1) r15:r14 = combine(r7,r6) }

l0000A8FC:
	{ r1:r0 = convert_d2df(r15:r14); r4 = add(r4,#0xFFFFFFC9) }
	{ r1 += asl(r4,#0x14); jumpr r31 }
0000A90C                                     C0 5B 01 69             .[.i
0000A910 D8 FF FF 59                                     ...Y            

l0000A914:
	{ r1:r0 = convert_d2df(r15:r14); r4 = add(r4,#0xFFFFFFC9) }
	{ r1 += asl(r4,#0x14); r5 = extractu(r1,#0xB,#0x13) }
	{ r4 = add(r4,r5); r13:r12 = abs(r15:r14) }
	{ immext(#0x7C0); p0 = cmp.gt(r4,#0x7FE); if (p0.new) jump:nt 0000A9D4 }

l0000A938:
	{ p0 = cmp.gt(r4,#0x0); if (p0.new) jump:nt 0000A99C; r28 = #0x3F }

l0000A940:
	{ r7 = USR; r5 = add(clb(r12),#0xFFFFFFF4); r4 = sub(#0x3,r4); p3 = cmp.gt(r15,#0xFFFFFFFF) }
	{ r13:r12 = asl(r13:r12,r5); r5 = min(r28,r4); r4 = #0x0; r6 = #0x30 }
	{ r9:r8 = extractu(r13:r12,r5:r4); r13:r12 = asr(r13:r12,r5) }
	{ p0 = cmp.gtu(r11:r10,r9:r8); r13 = setbit(r13,#0xE); if (!p0.new) r12 = or(r12,r10) }
	{ r15:r14 = neg(r13:r12); p1 = bitsclr(r12,#0x7); if (!p1.new) r7 = or(r7,r6) }
	{ USR = r7; if (p3) r15:r14 = combine(r13,r12); r3:r2 = combine(#0x0,#0x0); r28 = #0xFFFFFBCA }
	{ r1:r0 = convert_d2df(r15:r14); p0 = dfcmp.uo(r3:r2,r3:r2) }
	{ r1 += asl(r28,#0x14); jumpr r31 }

l0000A99C:
	{ immext(#0x7FEFFFC0); r28 = #0x7FEFFFFF; r15:r14 = abs(r15:r14) }
	{ p0 = bitsclr(r1,r28); if (!p0.new) jumpr:t r31; p0 = cmp.eq(r0,#0x0); r28 = #0x7FFF }
0000A9B8                         07 40 08 6A 00 5C 4F C7         .@.j.\O.
0000A9C0 06 C6 00 78 07 C6 27 F9 08 C0 27 62 00 40 E0 D2 ...x..'...'b.@..
0000A9D0 00 C0 9F 52                                     ...R            

l0000A9D4:
	{ r28 = USR; immext(#0x7FEFFFC0); r15:r14 = combine(#0x7FEFFFFF,#0xFFFFFFFF); r1:r0 = combine(r15,r14) }
	{ immext(#0x7FF00000); r13:r12 = combine(#0x7FF00000,#0x0); r7 = extractu(r28,#0x2,#0x12); r28 = or(r28,#0x28) }
	{ USR = r28; XOREQ r7,lsr(r1,#0x1F); r6 = r7 }
	{ p0 = !cmp.eq(r6,00000001); p0 = dfcmp.eq(r15:r14,r15:r14); p0 = !cmp.eq(r7,00000002); if (!p0) r15:r14 = combine(r13,r12) }
	{ r1:r0 = insert(r14,#0x3F,#0x7); jumpr r31 }

l0000AA18:
	{ p0 = dfclass(r1:r0,#0xF); p0 = dfclass(r3:r2,#0xF) }
	{ p1 = dfclass(r1:r0,#0x8); p1 = dfclass(r3:r2,#0x8) }
	{ p2 = dfclass(r1:r0,#0x1); p2 = dfclass(r3:r2,#0x1) }
	{ if (!p0) jump:nt 0000AAF0; if (p1) jump:nt 0000AB10 }

l0000AA38:
	{ if (p2) jump:nt 0000AB10 }
	{ p0 = dfclass(r1:r0,#0xE); p0 = dfclass(r3:r2,#0x7) }
	{ p1 = dfclass(r1:r0,#0x7); p1 = dfclass(r3:r2,#0xE) }
	{ if (!p0) jump:nt 0000AAB0; if (!p1) jump:nt 0000AAC0 }
	{ p0 = dfclass(r1:r0,#0x2); p1 = dfclass(r3:r2,#0x2); r15:r14 = combine(#0x0,#0x0); r13:r12 = combine(#0x0,#0x0) }
	{ r15:r14 = insert(r0,#0x34,#0xC); r13:r12 = insert(r2,#0x34,#0xC); immext(#0x40000000); r28 = #0x40000000 }
	{ r4 = extractu(r1,#0xB,#0x13); r5 = extractu(r3,#0xB,#0x13); if (p0) r15 = or(r15,r28); if (p1) r13 = or(r13,r28) }
	{ r7 = add(clb(r14),#0xFFFFFFFE); r6 = add(clb(r12),#0xFFFFFFFE); r11:r10 = combine(#0x0,#0x1) }
	{ r15:r14 = asl(r15:r14,r7); r13:r12 = asl(r13:r12,r6); if (!p0) r4 = sub(r10,r7); if (!p1) r5 = sub(r10,r6) }
	{ jump 0000A90C; r7:r6 = combine(#0x0,#0x0); r9:r8 = combine(r15,r14); r4 = sub(r4,r5) }
	{ r1:r0 = xor(r1:r0,r3:r2); r3:r2 = combine(#0x0,#0x0) }
	{ r1:r0 = insert(r2,#0x3F,#0x7); jumpr r31 }
	{ p2 = dfclass(r3:r2,#0x1); p2 = dfclass(r1:r0,#0x7) }
	{ r28 = USR; if (!p2) jump:nt 0000AADC; r1 = xor(r1,r3) }
	{ r28 = or(r28,#0x4) }
	{ USR = r28 }
	{ immext(#0x7FF00000); r3:r2 = combine(#0x7FF00000,#0x0); p0 = dfcmp.uo(r3:r2,r3:r2) }
	{ r1:r0 = insert(r2,#0x3F,#0x7); jumpr r31 }

l0000AAF0:
	{ p0 = dfclass(r1:r0,#0x10); p1 = dfclass(r3:r2,#0x10); if (!p0.new) r1:r0 = combine(r3,r2); if (!p1.new) r3:r2 = combine(r1,r0) }
	{ r15 = convert_df2sf(r1:r0); r14 = convert_df2sf(r3:r2) }
	{ r1:r0 = combine(#0xFFFFFFFF,#0xFFFFFFFF); jumpr r31 }

l0000AB10:
	{ immext(#0x7F800000); r28 = #0x7F800001 }
	{ r1:r0 = convert_sf2df(r28); jumpr r31 }

;; __hexagon_fast_muldf3: 0000AB20
;;   Called from:
;;     000087A0 (in _LXp_mulh)
;;     00008844 (in _LXp_mulh)
__hexagon_fast_muldf3 proc
	{ p0 = dfclass(r1:r0,#0x2); p0 = dfclass(r3:r2,#0x2); immext(#0x40000000); r13:r12 = combine(#0x40000000,#0x0) }
	{ r13:r12 = insert(r0,#0x34,#0xC); r5:r4 = asl(r3:r2,#0xA); r28 = #0xFFFFFC00; r9:r8 = combine(#0x0,#0x1) }
	{ r7:r6 = mpyu(r4,r13); r5:r4 = insert(r8,#0x2,#0x3A) }
	{ r15:r14 = mpyu(r12,r4) }
	{ r7:r6 += lsr(r15:r14,#0x20); r11:r10 = mpyu(r13,r5); immext(#0x7C0); r5:r4 = combine(#0x7FC,#0x0) }
	{ r11:r10 += lsr(r7:r6,#0x20); if (!p0) jump:nt 0000ACC4; p1 = cmp.eq(r14,#0x0); p1 = cmp.eq(r6,#0x0) }

l0000AB70:
	{ r6 = extractu(r1,#0xB,#0x13); r7 = extractu(r3,#0xB,#0x13); if (!p1) r10 = or(r10,r8) }
	{ r15:r14 = neg(r11:r10); r6 += add(r28,r7); r28 = xor(r1,r3) }
	{ if (!p2.new) r11:r10 = combine(r15,r14); p0 = cmp.gt(r6,r4); if (p0.new) jump:nt 0000ABE0; p2 = cmp.gt(r28,#0xFFFFFFFF); p0 = !cmp.gt(r6,r5) }

l0000AB98:
	{ r1:r0 = convert_d2df(r11:r10); r6 = add(r6,#0xFFFFFBC6) }
	{ r1 += asl(r6,#0x14); jumpr r31; nop; nop }

l0000ABB0:
	{ p0 = bitsclr(r1,r4); if (!p0.new) jumpr:t r31; p0 = cmp.eq(r0,#0x0); r5 = #0x7FFF }
0000ABC0 04 40 08 6A 00 45 4D C7 05 C6 00 78 04 C5 24 F9 .@.j.EM....x..$.
0000ABD0 08 C0 24 62 00 40 E0 D2 00 40 9F 52 00 C0 00 7F ..$b.@...@.R....

l0000ABE0:
	{ r1:r0 = convert_d2df(r11:r10); r13:r12 = abs(r11:r10); r7 = add(r6,#0xFFFFFBC6) }
	{ r1 += asl(r7,#0x14); r7 = extractu(r1,#0xB,#0x13); immext(#0x7FEFFFC0); r4 = #0x7FEFFFFF }
	{ immext(#0xFFFFFBC0); r7 += add(r6,#0xFFFFFBC6); r5 = #0x0 }
	{ immext(#0x7C0); p0 = cmp.gt(r7,#0x7FE); if (p0.new) jump:nt 0000AC80 }

l0000AC14:
	{ p0 = cmp.gt(r7,#0x0); if (p0.new) jump:nt 0000ABB0; r5 = sub(r6,r5); r28 = #0x3F }

l0000AC20:
	{ r4 = #0x0; r5 = sub(#0x5,r5) }
	{ p3 = cmp.gt(r11,#0xFFFFFFFF); r5 = min(r28,r5); r11:r10 = combine(r13,r12) }
	{ r28 = USR; r15:r14 = extractu(r11:r10,r5:r4) }
	{ r11:r10 = asr(r11:r10,r5); r1 = insert(#0xB,#0x13); r4 = #0x30 }
	{ p0 = cmp.gtu(r9:r8,r15:r14); r11 = setbit(r11,#0xE); if (!p0.new) r10 = or(r10,r8) }
	{ r15:r14 = neg(r11:r10); p1 = bitsclr(r10,#0x7); if (!p1.new) r28 = or(r4,r28) }
	{ USR = r28; if (p3.new) r11:r10 = combine(r15,r14) }
	{ r1:r0 = convert_d2df(r11:r10); p0 = dfcmp.eq(r1:r0,r1:r0) }
	{ r1 = insert(#0xA,#0x12); jumpr r31; nop; nop }

l0000AC80:
	{ r28 = USR; immext(#0x7FEFFFC0); r13:r12 = combine(#0x7FEFFFFF,#0xFFFFFFFF); r1:r0 = combine(r11,r10) }
	{ r14 = extractu(r28,#0x2,#0x12); r28 = or(r28,#0x28); immext(#0x7FF00000); r5:r4 = combine(#0x7FF00000,#0x0) }
	{ USR = r28; XOREQ r14,lsr(r1,#0x1F); r28 = r14 }
	{ p0 = !cmp.eq(r28,00000001); p0 = dfcmp.eq(r1:r0,r1:r0); p0 = !cmp.eq(r14,00000002); if (!p0) r13:r12 = combine(r5,r4) }
	{ r1:r0 = insert(r12,#0x3F,#0x7); jumpr r31 }

l0000ACC4:
	{ r13:r12 = extractu(r1:r0,#0x3F,#0x7); r5:r4 = extractu(r3:r2,#0x3F,#0x7) }
	{ p3 = cmp.gtu(r13:r12,r5:r4); if (!p3.new) r1:r0 = combine(r3,r2); if (!p3.new) r3:r2 = combine(r1,r0) }
	{ p0 = dfclass(r1:r0,#0xF); if (!p0.new) jump:nt 0000AD74; if (p3.new) r13:r12 = combine(r5,r4); if (p3.new) r5:r4 = combine(r13,r12) }

l0000ACE8:
	{ p1 = dfclass(r1:r0,#0x8); p1 = dfclass(r3:r2,#0xE) }
	{ p0 = dfclass(r1:r0,#0x8); p0 = dfclass(r3:r2,#0x1) }
	{ if (p1) jump:nt 0000AD98; p2 = dfclass(r3:r2,#0x1) }

l0000AD00:
	{ if (p0) jump:nt 0000AD5C; if (p2) jump:nt 0000AD90; immext(#0x7C000000); r28 = #0x7C000000 }

l0000AD10:
	{ p0 = bitsclr(r1,r28); if (p0.new) jump:nt 0000AD30 }
	{ r28 = cl0(r5:r4) }
	{ r28 = add(r28,#0xFFFFFFF5) }
	{ r5:r4 = asl(r5:r4,r28) }
	{ r3:r2 = insert(r4,#0x3F,#0x7); r1 -= asl(r28,#0x14) }
	{ jump __hexagon_fast_muldf3 }
	{ r28 = USR; r1:r0 = xor(r1:r0,r3:r2) }
	{ r1:r0 = insert(r8,#0x3F,#0x7); r5 = extractu(r28,#0x2,#0x12); r28 = or(r28,#0x30) }
	{ USR = r28; XOREQ r5,lsr(r1,#0x1F); p0 = cmp.gt(r5,#0x1); if (!p0.new) r0 = #0x0 }
	{ if (!p0.new) r0 = #0x0; p0 = cmp.eq(r5,#0x3); jumpr r31 }

l0000AD5C:
	{ r28 = USR }
	{ r1:r0 = combine(#0xFFFFFFFF,#0xFFFFFFFF); r28 = or(r28,#0x2) }
	{ USR = r28 }
	{ p0 = dfcmp.uo(r1:r0,r1:r0); jumpr r31 }

l0000AD74:
	{ p0 = dfclass(r3:r2,#0xF); r28 = convert_df2sf(r1:r0); if (!p0) r3:r2 = combine(r1,r0) }
	{ r2 = convert_df2sf(r3:r2); jumpr r31; r1:r0 = combine(#0xFFFFFFFF,#0xFFFFFFFF); nop }

l0000AD90:
	{ r1:r0 = combine(r3,r2); r3:r2 = combine(r1,r0) }

l0000AD98:
	{ r3 = extract(r3,#0x1,#0x19) }
	{ XOREQ r1,asl(r3,#0x1F); jumpr r31 }
0000ADA4             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............
0000ADB0 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F 00 C0 00 7F ................

;; fn0000ADC0: 0000ADC0
;;   Called from:
;;     00009558 (in sqrtl)
;;     0000B348 (in sqrt)
fn0000ADC0 proc
	{ p3 = dfclass(r1:r0,#0x2); if (!p3.new) jump:nt 0000AE4C; immext(#0x10000000); r15:r14 = combine(#0x10000000,#0x0) }

l0000ADD0:
	{ r15:r14 = insert(r0,#0x34,#0xC); r4 = extractu(r1,#0xB,#0x13); r7:r6 = combine(r15,r14) }

l0000ADDC:
	{ loop0(0000AE04,#0x18); p1 = cmp.gt(r1,#-0x1); if (!p1.new) jump:nt 0000AE94; r28 = #0x3FF }

l0000ADE8:
	{ p0 = tstbit(r4,#0x0); r13:r12 = asl(r15:r14,#0x1); r4 = vavgh(r28,r4) }
	{ r9:r8 = vmux(p0,r15:r14,r13:r12); r3:r2 = lsr(r7:r6,#0x1) }
	{ r9:r8 = sub(r9:r8,r7:r6); r28 = #0x1 }
	{ r3:r2 += asl(r7:r6,#0x1); r9:r8 = asl(r9:r8,#0x1); r11:r10 = combine(r3,r2) }
	{ r9:r8 = sub(r9:r8,r3:r2); r11:r10 = or(r7:r6,r11:r10); r3:r2 = combine(r11,r10); r13:r12 = combine(r9,r8) }
	{ p0 = cmp.gt(r9,#0xFFFFFFFF); r3:r2 = lsr(r3:r2,#0x1); if (!p0.new) r9:r8 = combine(r13,r12); if (!p0) r7:r6 = combine(r11,r10) }
	{ p0 = cmp.eq(r9,#0x0); p0 = cmp.eq(r8,#0x0); if (!p0.new) r6 = or(r6,r28) }
	{ r1:r0 = convert_d2df(r7:r6); r4 = add(r4,#0xFFFFFBC5) }
	{ r1 += asl(r4,#0x14); jumpr r31 }

l0000AE4C:
	{ p0 = dfclass(r1:r0,#0x10); if (p0.new) jump:nt 0000AE84 }

l0000AE54:
	{ p0 = dfclass(r1:r0,#0x1); if (p0.new) jump:nt 0000AE90 }

l0000AE5C:
	{ p0 = cmp.gt(r1,#-0x1); if (p0.new) jump:nt 0000AE94 }

l0000AE60:
	{ p0 = dfclass(r1:r0,#0x8); if (p0.new) jump:nt 0000AE90 }

l0000AE68:
	{ r4 = add(clb(r0),#0xFFFFFFEA); immext(#0x10000000); r7:r6 = combine(#0x10000000,#0x0) }
	{ r4 = sub(#0x1,r4); r5 = add(r4,#0x8) }
	{ r15:r14 = asl(r1:r0,r5); jump 0000ADDC }

l0000AE84:
	{ r4 = convert_df2sf(r1:r0); jumpr r31; r1:r0 = combine(#0xFFFFFFFF,#0xFFFFFFFF) }

l0000AE90:
	{ jumpr r31 }

l0000AE94:
	{ immext(#0x7F800000); r4 = #0x7F800001 }
	{ r1:r0 = convert_sf2df(r4); jumpr r31 }
0000AEA4             00 C0 00 7F 00 C0 00 7F 00 C0 00 7F     ............

;; _exit: 0000AEB0
;;   Called from:
;;     000079B0 (in _Exit)
_exit proc
	{ r3 = r0; immext(#0xE400); r1 = memw(gp+58400); allocframe(#0x8) }
	{ p0 = cmp.eq(r1,#0x0); if (p0.new) jump:nt 0000AEE4; if (!p0.new) r4 = add(r29,#0x4); memw(r29+4) = r3 }

l0000AECC:
	{ r0 = #0x18 }
	{ r1 = r4 }
	{ r2 = r3 }
	{ trap0(#0x0) }
	{ r3 = r0 }
	{ r0 = r3; dealloc_return }

l0000AEE4:
	{ call thread_stop }
	{ r3 = #0x0 }
	{ r0 = r3; dealloc_return }

;; lockMutex: 0000AEF0
;;   Called from:
;;     00006A04 (in _Lockfilelock)
;;     0000AF1C (in fn0000AF10)
;;     0000AF24 (in fn0000AF20)
;;     0000B0A8 (in __sys_sbrk)
;;     0000B15C (in sys_Tlsalloc)
;;     0000B204 (in sys_Tlsfree)
lockMutex proc
	{ r2 = memw_locked(r0) }

;; fn0000AEF4: 0000AEF4
;;   Called from:
;;     00006AF0 (in _Locksyslock)
fn0000AEF4 proc
	{ r4 = htid; r3 = asr(r2,#0x10); r1 = sxth(r2) }
	{ p1 = cmp.eq(r3,r4); r4 = asl(r4,#0x10); r2 = add(r2,#0x1) }
	{ if (p1) jump:nt fn0000AF20 }

;; fn0000AF10: 0000AF10
;;   Called from:
;;     0000AF0C (in fn0000AEF4)
;;     0000AF0C (in lockMutex)
fn0000AF10 proc
	{ p1 = cmp.eq(r1,#0x0); r2 = add(r4,#0x1) }
	{ if (p1) jump:nt fn0000AF20 }

l0000AF1C:
	{ jump lockMutex }

;; fn0000AF20: 0000AF20
;;   Called from:
;;     0000AF0C (in fn0000AEF4)
;;     0000AF0C (in lockMutex)
;;     0000AF18 (in fn0000AF10)
fn0000AF20 proc
	{ memw_locked(r0,p0) = r2 }
	{ if (!p0) jump:nt lockMutex }

l0000AF28:
	{ jumpr r31 }
0000AF2C                                     00 C0 00 7F             ....

;; __sys_Mtxunlock: 0000AF30
;;   Called from:
;;     00006A38 (in _Unlockfilelock)
;;     00006B18 (in _Unlocksyslock)
;;     0000B118 (in __sys_sbrk)
;;     0000B1B4 (in sys_Tlsalloc)
;;     0000B210 (in sys_Tlsfree)
__sys_Mtxunlock proc
	{ r1 = memw_locked(r0) }
	{ r4 = htid; r5 = extractu(r1,#0x3,#0x13); r3 = #0x1 }
	{ p1 = cmp.eq(r5,r4); r3 = vsubh(r1,r3) }
	{ if (!p1) jumpr:nt r31 }
0000AF4C                                     00 C3 A0 A0             ....
0000AF50 F0 E0 FF 5C 00 C0 9F 52 00 40 00 7F 00 C0 00 7F ...\...R.@......

;; __sys_Mtxinit: 0000AF60
;;   Called from:
;;     00006910 (in _Initlocks)
;;     00006934 (in _Initlocks)
;;     000069A0 (in _Lockfilelock)
;;     000069C0 (in _Lockfilelock)
;;     00006A90 (in _Locksyslock)
;;     00006AB4 (in _Locksyslock)
;;     0000B130 (in BeforeBegin)
__sys_Mtxinit proc
	{ r1 = #0x0 }
	{ jumpr r31; memw(r0) = r1 }
0000AF6C                                     00 C0 00 7F             ....

;; __sys_Mtxdst: 0000AF70
;;   Called from:
;;     000068C0 (in _Clearlocks)
;;     000068E4 (in _Clearlocks)
;;     0000B140 (in AtEnd)
__sys_Mtxdst proc
	{ jumpr r31 }
0000AF74             00 40 00 7F 00 40 00 7F 00 C0 00 7F     .@...@......

;; __trylockMutex: 0000AF80
__trylockMutex proc
	{ r4 = htid }

l0000AF84:
	{ r2 = memw_locked(r0) }
	{ r3 = asr(r2,#0x10) }
	{ p1 = cmp.eq(r3,r4) }
	{ if (p1) r2 = add(r2,#0x1) }
	{ if (p1) jump:nt 0000AFB4 }

l0000AF98:
	{ r3 = sxth(r2) }
	{ p1 = cmp.eq(r3,#0x0) }
	{ r5 = asl(r4,#0x10) }
	{ if (p1) r2 = add(r5,#0x0) }
	{ if (p1) r2 = add(r2,#0x1) }
	{ if (p1) jump:nt 0000AFB4 }

l0000AFB0:
	{ r0 = #0x0; jumpr r31 }

l0000AFB4:
	{ memw_locked(r0,p0) = r2 }
	{ if (!p0) jump:nt 0000AF84 }

l0000AFBC:
	{ r0 = #0x1; jumpr r31 }

;; __sys_close: 0000AFC0
;;   Called from:
;;     000094E0 (in close)
__sys_close proc
	{ immext(#0xE400); r1 = memw(gp+58400); allocframe(#0x10) }
	{ p0 = cmp.eq(r1,#0x0); if (p0.new) jump:nt 0000B004; memd(r29+8) = r17:r16; memw(r29+4) = r0 }

l0000AFD4:
	{ call hexagon_cache_cleaninv; r0 = add(r29,#0x4); r1 = #0x4; r16 = add(r29,#0x4) }
	{ r0 = #0x2 }
	{ r1 = r16 }
	{ trap0(#0x0) }
	{ r2 = r0 }
	{ r16 = r1 }
	{ p0 = cmp.eq(r2,#-0x1); if (p0.new) jump:t 0000B010 }

l0000AFF8:
	{ call _Geterrno }
	{ jump 0000B010; r2 = #-0x1; memw(r0) = r16 }

l0000B004:
	{ call _Geterrno }
	{ r2 = #0xFFFFFFFF; memw(r0) = #0x59 }

l0000B010:
	{ r0 = r2; r17:r16 = memd(r29+8); dealloc_return }
0000B018                         00 C0 00 7F 00 C0 00 7F         ........

;; __sys_remove: 0000B020
;;   Called from:
;;     00009510 (in remove)
__sys_remove proc
	{ immext(#0xE400); r1 = memw(gp+58400); allocframe(#0x10) }
	{ p0 = cmp.eq(r1,#0x0); if (p0.new) jump:nt 0000B084; r16 = r0; memd(r29+8) = r17:r16 }

l0000B034:
	{ call strlen; r17 = add(r29,#0x0); r0 = r16; memw(r29) = r16 }
	{ r2 = setbit(r17,#0x4); r1 = r0; r0 = r16 }
	{ call strlen; memw(r2) = r1 }
	{ call hexagon_cache_cleaninv; r0 = r16; r1 = r0 }
	{ call hexagon_cache_cleaninv; r1 = #0x8; r0 = add(r29,#0x0) }
	{ r0 = #0xE }
	{ r1 = r17 }
	{ trap0(#0x0) }
	{ r2 = r0 }
	{ r16 = r1 }
	{ p0 = cmp.eq(r2,#-0x1); if (p0.new) jump:t 0000B090 }

l0000B078:
	{ call _Geterrno }
	{ jump 0000B090; r2 = #-0x1; memw(r0) = r16 }

l0000B084:
	{ call _Geterrno }
	{ r2 = #0xFFFFFFFF; memw(r0) = #0x59 }

l0000B090:
	{ r0 = r2; r17:r16 = memd(r29+8); dealloc_return }
0000B098                         00 C0 00 7F 00 C0 00 7F         ........

;; __sys_sbrk: 0000B0A0
;;   Called from:
;;     00007AB8 (in _Getmem)
__sys_sbrk proc
	{ r17 = r0; memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ call lockMutex; immext(#0x10040); r0 = #0x1006C }
	{ immext(#0xE000); r1 = #0xE018 }
	{ immext(#0x10040); r16 = memw(r0=00010068); if (!cmp.eq(r16.new,#0x0)) jump:t 0000B0E0 }

l0000B0CC:
	{ r3 = #0x0; r2 = memw(r1) }
	{ p0 = !cmp.eq(r2,00000000); if (p0.new) r16 = add(r2,#0x0); if (!p0.new) r16 = add(r3,#0x0) }
	{ memw(r0) = r16 }

l0000B0E0:
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:nt 0000B118 }

l0000B0E4:
	{ r3 = r17; r2 = memw(r1) }
	{ r3 += add(r16,#0x7) }
	{ r1 = and(r3,#0xFFFFFFF8) }
	{ immext(#0xE000); r2 = memw(gp+57392); if (cmp.eq(r2.new,#0x0)) jump:t 0000B104 }

l0000B104:
	{ jump 0000B118; memw(r0) = r1; memw(r16) = r17 }
0000B10C                                     02 5F FF 5B             ._.[
0000B110 F0 FF DF 78 0C C0 40 3C                         ...x..@<        

l0000B118:
	{ call __sys_Mtxunlock; immext(#0x10040); r0 = #0x1006C }
	{ r0 = r16; r17:r16 = memd(r29); dealloc_return }
0000B12C                                     00 C0 00 7F             ....

;; BeforeBegin: 0000B130
BeforeBegin proc
	{ jump __sys_Mtxinit; immext(#0x10040); r0 = #0x10070; nop }

;; AtEnd: 0000B140
AtEnd proc
	{ jump __sys_Mtxdst; immext(#0x10040); r0 = #0x10070; nop }

;; sys_Tlsalloc: 0000B150
;;   Called from:
;;     000065A4 (in _Tls_get__Mbcurmax)
;;     0000664C (in fn00006620)
;;     00006B78 (in _Tls_get__Tolotab)
;;     00006EB8 (in _Tls_get__Errno)
;;     00007938 (in _Tls_get__Ctype)
;;     00008FB8 (in _Tls_get__Wcstate)
;;     00009C88 (in _Tls_get__Locale)
sys_Tlsalloc proc
	{ r17:r16 = combine(#0xC,r0); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ p0 = cmp.eq(r8,#0x0); if (p0.new) jump:nt 0000B1C4 }

l0000B15C:
	{ call lockMutex; immext(#0x10040); r0 = #0x10070 }
	{ immext(#0xFE104FC0); r0 = #0xFE104FFF; immext(#0xEC00); r1 = #0xEC18 }
	{ immext(#0xEB00); r2 = #0xEB2C; r17 = #0xB }

l0000B180:
	{ immext(#0x1EFB000); r3 = add(r0,#0x1EFB001) }
	{ if (p0.new) jump:nt 0000B1B4; p0 = cmp.gtu(r3,#0x3F) }

l0000B190:
	{ r0 = add(r0,#0x1); r2 = add(r2,#0x4); r1 = add(r1,#0x18); r3 = memw(r2+4) }
	{ p0 = cmp.eq(r3,#0x0); if (!p0.new) jump:t 0000B180 }

l0000B1A0:
	{ loop0(0000B1A8,#0x6); r3 = #0x0; memw(r2) = #0x1 }
	{ nop; memw(r1++#4) = r3 }
	{ r17 = #0x0; memw(r16) = r0 }

l0000B1B4:
	{ call __sys_Mtxunlock; immext(#0x10040); r0 = #0x10070; nop }

l0000B1C4:
	{ r0 = r17; nop; r17:r16 = memd(r29); dealloc_return }

;; sys_Tlsfree: 0000B1D0
sys_Tlsfree proc
	{ immext(#0x1EFB000); r17 = add(r0,#0x1EFB000); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ if (p0.new) jump:t 0000B21C; r16 = #0x16; p0 = cmp.gtu(r17,#0x3F) }

l0000B1E8:
	{ immext(#0xEB00); r0 = memw(r17<<#2+0000EB30); if (cmp.eq(r0.new,#0x0)) jump:nt 0000B21C }

l0000B1F8:
	{ r0 = #0x30; immext(#0xEB00); r16 = #0xEB30 }
	{ call lockMutex }
	{ r0 = addasl(r16,r17,#0x2); r16 = #0x0 }
	{ call __sys_Mtxunlock; immext(#0x10040); r0 = #0x10070; memw(r0) = #0x0 }

l0000B21C:
	{ r0 = r16; r17:r16 = memd(r29); dealloc_return }

;; sys_Tlsset: 0000B224
;;   Called from:
;;     000065D8 (in _Tls_get__Mbcurmax)
sys_Tlsset proc
	{ immext(#0x1EFB000); r2 = add(r0,#0x1EFB000); r0 = #0x16 }
	{ if (p0.new) jump:t 0000B258; p0 = cmp.gtu(r2,#0x3F) }

l0000B238:
	{ immext(#0xEB00); r3 = memw(r2<<#2+0000EB30) }
	{ r3 = htid }
	{ immext(#0xEC00); r4 = #0xEC30; r0 = #0x0 }
	{ r2 = add(r4,mpyi(#0x18,r2)) }
	{ memw(r14+r3<<#2) = r1 }

l0000B258:
	{ nop; jumpr r31 }

;; sys_Tlsget: 0000B260
sys_Tlsget proc
	{ immext(#0x1EFB000); r1 = add(r0,#0x1EFB000); r0 = #0x0 }
	{ if (p0.new) jump:t 0000B294; p0 = cmp.gtu(r1,#0x3F) }

l0000B274:
	{ immext(#0xEB00); r2 = memw(r1<<#2+0000EB30); if (cmp.eq(r2.new,#0x0)) jump:nt 0000B294 }

l0000B284:
	{ immext(#0xEC00); r2 = #0xEC30 }
	{ r1 = add(r2,mpyi(#0x18,r1)) }
	{ r0 = memw(r30+r0<<#2) }

l0000B294:
	{ jumpr r31 }
0000B298                         00 C0 00 7F 00 C0 00 7F         ........

;; __sys_write: 0000B2A0
;;   Called from:
;;     000094F0 (in write)
__sys_write proc
	{ r3 = r0; immext(#0xE400); r4 = memw(gp+58400); allocframe(#0x18) }
	{ p0 = cmp.eq(r4,#0x0); if (p0.new) jump:nt 0000B2F8; r0 = r1; memd(r29+16) = r17:r16 }

l0000B2B8:
	{ r1 = r2; r16 = add(r29,#0x0); memw(r29) = r3 }
	{ r4 = setbit(r16,#0x4) }
	{ call hexagon_cache_cleaninv; memw(r4) = r0; memw(r16+8) = r2 }
	{ call hexagon_cache_cleaninv; r1 = #0xC; r0 = add(r29,#0x0) }
	{ r0 = #0x5 }
	{ r1 = r16 }
	{ trap0(#0x0) }
	{ r2 = r0 }
	{ r16 = r1 }
	{ p0 = cmp.eq(r2,#-0x1); if (p0.new) jump:t 0000B304 }

l0000B2EC:
	{ call _Geterrno }
	{ jump 0000B304; r2 = #-0x1; memw(r0) = r16 }

l0000B2F8:
	{ call _Geterrno }
	{ r2 = #0xFFFFFFFF; memw(r0) = #0x59 }

l0000B304:
	{ r0 = r2; r17:r16 = memd(r29+16); dealloc_return }
0000B30C                                     00 C0 00 7F             ....

;; sqrt: 0000B310
sqrt proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ call _Dunscale; r0 = add(r29,#0x6); r1 = add(r29,#0x8); memd(r29+8) = r17:r16 }
	{ p0 = cmp.eq(r0,#0x0); if (p0.new) jump:nt 0000B348 }

l0000B328:
	{ p0 = cmp.eq(r0,#0x1); if (p0.new) jump:t 0000B330 }

l0000B32C:
	{ p0 = cmp.eq(r0,#0x2); if (p0.new) jump:t 0000B348 }

l0000B330:
	{ r0 = add(r29,#0x8) }
	{ r0 = or(r0,#0x6) }
	{ r0 = memh(r0); if (cmp.gt(r0.new,#-0x1)) jump:t 0000B348 }

l0000B344:
	{ r0 = #0x1 }

l0000B348:
	{ call fn0000ADC0; r1:r0 = combine(r17,r16) }
	{ r17:r16 = memd(r29+16); dealloc_return }
