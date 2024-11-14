;;; Segment .text (00009840)

l00019850:
	{ r7:r6 = vaslw(r25:r24,#0x2) }
	{ r3 = and(#0x81,asl(r3,#0x4)); if (!cmp.gtu(r3.new,r2)) jump:t 00019874 }

l00019860:
	{ r3 = add(PC,#0x3B); r1 = #0xF7; memw(r29+4) = r2 }
	{ r2 = r23; memw(r29) = r7 }
	{ jump 00019904 }

l00019874:
	{ r2 = memw(r29+124) }
	{ r5 = r2; r4 = memw(r2+20); if (!cmp.gtu(r25,r4.new)) jump:t 0001989C }

l00019888:
	{ r3 = add(PC,#0x2D); r1 = #0xFA; memw(r29+4) = r25 }
	{ r2 = r23; memw(r29) = r4 }
	{ jump 00019904 }

l0001989C:
	{ immext(#0xED40); r3 = add(PC,#0xED6F); r1 = #0xFC; r2 = memw(r29+52) }
	{ r2 = memw(r2); if (!cmp.eq(r2.new,#0x1)) jump:t 00019900 }

l000198B8:
	{ r3 = add(PC,#0x28); r1 = #0xFD; r2 = memw(r29+52) }
	{ r2 = memw(r2+12); if (!cmp.eq(r2.new,#0x1)) jump:t 00019900 }

l000198D0:
	{ r3 = add(PC,#0x21); r4 = #0x4; r2 = memd(r29+48) }
	{ r1 = #0xFE }
	{ r2 = memw(r2+20); if (cmp.gtu(r4,r2.new)) jump:t 00019900 }

l000198E8:
	{ r3 = add(PC,#0x17); r1 = #0xFF; r2 = memw(r29+44) }
	{ p0 = cmp.gt(r24,#0x0); r2 = memw(r2+20); if (cmp.gtu(r2.new,#0x3)) jump:t 00019910 }

l00019900:
	{ r2 = r23 }

l00019904:
	{ call errlog_function }
	{ jump 00019CD4; r0 = #0xFFFFFFFF }

l00019910:
	{ r0 = p0; r2 = memw(r29+68); memw(r5+24) = r25 }
	{ r2 = #0x0; r23 = memd(r29+56); memw(r5) = r2 }
	{ r3 = memd(r29+84); memd(r29+112) = r7:r6 }
	{ if (p0) r20 = add(r24,#0x0); memw(r5+4) = r17; memw(r5+8) = r20 }
	{ if (!p0) jump:nt 00019974; memw(r5+12) = r24; memw(r29+20) = r0 }

l0001993C:
	{ r18 = max(r18,r2); r26 = memw(r29+40) }
	{ r19 = r3; r2 = memb(r23++#1) }
	{ r2 = sub(r2,r18) }
	{ r2 = convert_w2sf(r2) }
	{ r0 = sfmpy(r3,r2); call fn00009620; r20 = add(r20,#0xFFFFFFFF) }
	{ r2 = convert_uw2sf(r0):chop; r3 = r19; p0 = cmp.eq(r20,#0x0); memw(r26++#4) = r2.new }

l00019974:
	{ r14 = #0x0; r9 = r17; r2 = memd(r29+40); r1:r0 = memd(r29+112) }
	{ r3 = memd(r29+68); r18 = memd(r29+108) }
	{ r2 += add(r0,#0x7F); p0 = cmp.gt(r3,#0x0); r0 = memw(r29+20); r13 = memw(r29+60) }
	{ p1 = r0; r2 = and(r2,#0xFFFFFF80); if (p0) r12 = memw(r29+64); if (p0) r3 = memw(r29-128) }
	{ if (!p0) jump:nt 00019BD0; if (p0) r2 = sub(r22,r16); if (p0) r8 = add(r9,#0xFFFFFFFF); memw(r29+92) = r2 }

l000199B4:
	{ p3 = cmp.gt(r12,#0xFFFFFFFF); p2 = cmp.gt(r12,#0xFF); r4 = r18; r5 = memd(r29+104) }
	{ r7 = mpyi(r22,r21); r6 = #0xFF; r14 = #0x0; r1 = memw(r29+88) }
	{ r4 = add(r2,mpyi(r4,r5)); r3 = sub(r3,r13); r5 = #0x0; r2 = memd(r29+76) }
	{ r7 = mpyi(r7,r24); r4 += lsr(r4,#0x1F); p0 = cmp.gt(r1,#0xFF) }
	{ r8 = add(r3,mpyi(r8,r2)); r0 = p3; if (p0) r2 = add(r6,#0x0); memb(r29+31) = r0.new }
	{ p3 = cmp.gt(r1,#0xFFFFFFFF); if (!p2) r6 = zxtb(r12); r0 = memw(r29+124) }
	{ r8 += lsr(r8,#0x1F); p0 = r0; r3 = #0x0; memw(r29+52) = r5 }
	{ r5 = mpyi(r21,r24); if (!p3) r2 = add(r3,#0x0); if (!p0) r6 = add(r3,#0x0) }
	{ r1 = asr(r4,#0x1); memb(r29+22) = r1.new }
	{ memw(r29+56) = r3 }

l00019A30:
	{ if (!p0.new) jump:nt 00019BC0; r15 = #0x0; p0 = cmp.gt(r9,#0x0); if (p0.new) r3 = memw(r29+52) }

l00019A40:
	{ r12 = mpyi(r3,r13) }
	{ r3 = mpyi(r3,r9); memb(r29+16) = r3.new }

l00019A4C:
	{ p0 = cmp.gt(r10,#0x0); if (p0.new) jump:nt 00019A60; r9 = #0x0; if (p0.new) memw(r29+84) = r15 }
	{ jump 00019BB0; memw(r29+84) = r15 }
00019A60 01 1D 33 3D 04 43 0F ED 08 41 0F F3 C3 C1 9D 91 ..3=.C...A......
00019A70 01 52 08 ED 1C 44 23 F3 19 D4 BD A1 0A 41 00 5C .R...D#......A.\
00019A80 0A 40 20 7E E1 C9 9D 40 8C 40 00 58 1C C9 9D A1 .@ ~...@.@.X....
00019A90 A0 1D 93 3D 04 43 09 F3 81 1D 63 3D 08 40 09 ED ...=.C....c=.@..
00019AA0 04 58 04 ED E0 C2 9D 91 44 40 04 C4 0B C8 23 F3 .X......D@....#.
00019AB0 1A 40 61 70 14 40 00 78 09 40 00 78 03 C4 9D 91 .@ap.@.x.@.x....
00019AC0 00 40 43 75 1F CE 9D A1 5E C0 20 5C 17 40 6D 70 .@Cu....^. \.@mp
00019AD0 08 5C 14 F3 52 ED C2 21 4C 58 20 5C 17 40 6D 70 .\..R..!LX \.@mp
00019AE0 E0 FF 68 75 0A 40 8E 10 17 40 6D 70 08 CC 08 F3 ..hu.@...@mp....
00019AF0 40 40 00 58 17 C0 6D 70 00 41 36 60 12 50 08 ED @@.X..mp.A6`.P..
00019B00 0D 40 7A 70 11 C0 00 78 08 4B 11 F3 2E D0 C2 21 .@zp...x.K.....!
00019B10 2A 48 20 5C E0 7F 68 75 E3 7F 15 74 0E F2 08 FB *H \..hu...t....
00019B20 22 40 CD 10 08 78 0D FB 00 C0 2D 43 18 41 03 60 "@...x....-C.A.`
00019B30 1B 4E 15 E3 00 40 26 F3 20 C0 95 75 2F C0 2E 9B .N...@&. ..u/...
00019B40 10 40 20 5C 0F CF 22 F3 09 4F 00 EF 08 58 08 F3 .@ \.."..O...X..
00019B50 03 40 28 91 33 C0 2E 9B 00 83 26 F3 0F D3 22 F3 .@(.3.....&...".
00019B60 09 CF 00 EF 31 40 11 B0 0D 85 0D F3 00 C0 00 7F ....1@..........
00019B70 1A 47 1A F3 0D 40 77 70 03 C4 9D 91 34 40 14 B0 .G...@wp....4@..
00019B80 A8 E3 72 20 2A 40 0A B0 21 40 01 B0 EE 43 9D 91 ..r *@..!@...C..
00019B90 08 C9 84 AB 0E 4E C9 D5 00 D8 0A F2 8A E0 FF 5C .....N.........\
00019BA0 89 43 9D 91 72 C3 9D 91 29 40 09 B0 6A F2 72 20 .C..r...)@..j.r 

l00019BB0:
	{ r15 = memw(r29+84); r9 = memw(r29+80) }
	{ r15 = add(r15,#0x1); if (!cmp.eq(r15.new,r9)) jump:t 00019A4C }

l00019BC0:
	{ r4 = memd(r29+52); r3 = memd(r29+68) }

l00019BC4:
	{ r4 = add(r4,#0x1) }
	{ p0 = cmp.eq(r4,r3); if (!p0.new) jump:nt 00019A30; memw(r29+52) = r4 }

l00019BD0:
	{ r16 = r9; r2 = memw(r29+24) }
	{ r19 = add(r14,r2) }
	{ call fn00009760; immext(#0xFF0000); r1:r0 = combine(r19,#0xFF0000) }
	{ r12 = r16; r3 = memd(r29+20); r2 = memd(r29+28) }
	{ r4 = mpyi(r12,r18); p1 = r3; r13 = memw(r29+36); r14 = memw(r29+40) }
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 00019C74; if (p0.new) r5 = memw(r29+68) }

l00019C08:
	{ r4 = mpyi(r4,r5); immext(#0x100); r3:r2 = combine(#0x100,#0x0) }
	{ loop1(00019C18,r4) }
	{ if (p1) r5:r4 = combine(r13,r14) }
	{ if (!p1) jump:nt 00019C68; if (p1) r7 = memw(r29+92) }

l00019C24:
	{ loop0(00019C30,r24); r6 = mpyi(r3,r24) }
	{ r6 = addasl(r7,r6,#0x2) }
	{ r7 = #0x0; r8 = memw(r4++#4) }
	{ r9 = memw(r6++#4) }
	{ r8 = add(r9,r8) }
	{ immext(#0x8000); r8 = add(#0x8000,mpyi(r8,r0)) }
	{ r8 = asrh(r8); if (!cmp.gt(r8.new,#-0x1)) jump:nt 00019C5C }

l00019C54:
	{ p0 = cmp.gt(r2,r8); if (!p0.new) r7 = #0xFF }

l00019C5C:
	{ nop; memb(r5++#1) = r7 }
	{ r13 = add(r13,r24) }

l00019C68:
	{ r3 = add(r3,#0x1); nop; nop }

l00019C74:
	{ r3 = convert_w2sf(r19); r1 = #0x152; r4 = memd(r29+48); r6 = memd(r29+44) }
	{ r5 = memw(r29+32) }
	{ r2 = memw(r4+16); memw(r4+12) = #0x1 }
	{ r5 = sfmpy(r5,r3); r3 = #0x2; memw(r4+8) = #0x1; memw(r4+4) = #0x1 }
	{ memw(r4) = #0x1; memw(r2) = #0x0 }
	{ immext(#0xE980); r4 = add(PC,#0xE9B1); memw(r4+24) = #0x4; memw(r6) = #0x1 }
	{ r2 = memw(r6+16); memw(r6+8) = #0x1 }
	{ memw(r6+12) = #0x1; memw(r6+4) = #0x1 }
	{ memw(r2) = r5; memw(r6+24) = #0x4 }
	{ r2 = memw(r29+72); memw(r29+12) = r24 }
	{ r5 = memd(r29+68); memw(r29+8) = r18 }
	{ memw(r29+4) = r12; memw(r29) = r5 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00019CD4:
	{ r17:r16 = memd(r29+176); r19:r18 = memd(r29+168) }
	{ r21:r20 = memd(r29+160); r23:r22 = memd(r29+152) }
	{ r25:r24 = memd(r29+144); r27:r26 = memd(r29+136) }
	{ dealloc_return }

;; logmsg_function: 00019CE8
;;   Called from:
;;     00018E54 (in supernode_execute_hvx)
;;     00018E98 (in supernode_execute_hvx)
;;     00018ECC (in supernode_execute_hvx)
;;     00018EEC (in supernode_execute_hvx)
;;     00018F04 (in supernode_execute_hvx)
;;     00018F34 (in supernode_execute_hvx)
;;     00018F74 (in supernode_execute_hvx)
;;     00019068 (in supernode_execute_hvx)
;;     0001908C (in supernode_execute_hvx)
;;     000191B0 (in supernode_execute_hvx)
;;     0001920C (in supernode_execute_hvx)
;;     00019260 (in supernode_check_ref)
;;     00019470 (in supernode_check_ref)
;;     000197A0 (in supernode_execute_ref)
;;     000197C8 (in supernode_execute_ref)
;;     000197EC (in supernode_execute_ref)
;;     00019804 (in supernode_execute_ref)
;;     00019830 (in supernode_execute_ref)
;;     00019CCC (in supernode_execute_ref)
logmsg_function proc
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00019D08 }

l00019CF8:
	{ r0 = add(PC,#0xC); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l00019D08:
	{ dealloc_return }

;; errlog_function: 00019D0C
;;   Called from:
;;     00019040 (in supernode_execute_hvx)
;;     00019418 (in supernode_check_ref)
;;     00019904 (in supernode_execute_ref)
errlog_function proc
	{ immext(#0xE7C0); r0 = add(PC,#0xE7F4); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; supernode_execute_hvx_slice: 00019D30
supernode_execute_hvx_slice proc
	{ r16 = r1; memd(r29-16) = r17:r16; allocframe(#0x138) }
	{ r3 = memw(r16); memd(r29+272) = r25:r24 }
	{ r5 = memw(r16+4) }
	{ memd(r29+280) = r23:r22 }
	{ r2 = memw(r3+4); memw(r29+248) = r5 }
	{ r17 = memb(r3+32) }
	{ memd(r29+296) = r19:r18; memd(r29+288) = r21:r20 }
	{ p0 = cmp.eq(r17,#0x0); r24 = memw(r2); r1 = memw(r2+4) }
	{ r4 = memw(r2+24); r7 = memw(r2+8) }
	{ r5 = memw(r2+16); memw(r29+240) = r7 }
	{ r7 = memw(r2+20); r22 = memw(r2+12) }
	{ r2 = memw(r1+12); r23 = memw(r24+8) }
	{ r0 = r23; memd(r29+264) = r27:r26; memw(r29+252) = r0 }
	{ r2 = memw(r1+8); memw(r29+64) = r2 }
	{ r21 = memw(r24+4); r19 = memw(r1) }
	{ r1 = p0; r20 = memw(r1+4); r27 = memw(r4+8) }
	{ r18 = memw(r4+4); memw(r29+228) = r7 }
	{ r6 = memw(r24+12); r7 = memw(r24) }
	{ memw(r29+256) = r3; memw(r29+224) = r5 }
	{ memw(r29+112) = r6; memw(r29+72) = r7 }
	{ if (!p0) r2 = sub(r23,r20); memw(r29+244) = r2; memw(r29+220) = r1 }
	{ if (p0) jump:nt 00019DEC }

l00019DD4:
	{ p0 = cmp.eq(r9,#0x2); if (p0.new) jump:nt 00019DEC; if (p0.new) r0 = add(r2,r27) }

l00019DDC:
	{ p0 = cmp.eq(r9,#0x1); if (!p0.new) jump:nt 00019DF4; r0 = #0x0 }

l00019DE4:
	{ r0 = r27 }
	{ r0 += add(r23,#0xFFFFFFFF) }

l00019DEC:
	{ call fn00009760; r1 = r27 }

l00019DF4:
	{ p0 = cmp.eq(r17,#0x2); r25 = r0; r26 = r27; if (p0.new) r1 = add(r18,#0x0) }
	{ r0 = p0; if (p0) jump:nt 00019E3C; if (p0) r2 = sub(r21,r19); memb(r29+59) = r0.new }

l00019E18:
	{ r1:r0 = combine(r18,r18) }
	{ r0 = #0x0; r1 = memw(r29+220) }
	{ p0 = r1; if (!p0.new) jump:nt 00019E44; if (!p0) r1:r0 = combine(r18,r21) }

l00019E30:
	{ jump 00019E40 }
00019E34             E0 5F 15 E2 06 C0 00 58                 ._.....X    

l00019E3C:
	{ r0 = add(r2,r18) }

l00019E40:
	{ call fn00009760 }

l00019E44:
	{ r6 = add(r0,#0xFFFFFFFF); r7 = sub(r20,r23); r3 = memw(r29+240); r4 = memw(r29+256) }
	{ r22 = add(r25,#0xFFFFFFFF); r17 = r19; r2 = memw(r22+16); memw(r29+116) = r23 }
	{ r22 = add(r7,mpyi(r22,r26)); r23 = r6; r3 = memw(r3+16); r4 = memw(r4+8) }
	{ r6 = memw(r29+224); memw(r29+240) = r6 }
	{ r1 = memw(r29+228); r2 = memw(r2) }
	{ r22 += lsr(r22,#0x1F); r19 = memd(r29+64); r7 = memd(r29+72) }
	{ r7 = mpyi(r19,r7); r3 = sub(r17,r21); r9 = add(r19,#0x1F); r27 = memw(r3) }
	{ r23 = add(r3,mpyi(r23,r18)); r7 = mpyi(r7,r25); r4 = memw(r4); r5 = memw(r1+16) }
	{ r0 = sfsub(r2,r27); r24 = r0; r6 = memw(r6+16); r8 = memw(r24+16) }
	{ memw(r29+128) = r26 }
	{ r23 += lsr(r23,#0x1F); r8 = memw(r16+20); memw(r29+160) = r8 }
	{ r3 = memw(r4+16); memw(r29+56) = r16 }
	{ memw(r29+108) = r21; memw(r29+228) = r9 }
	{ memw(r29+196) = r8 }
	{ r26 = memw(r5); memw(r29+224) = r7 }
	{ r16 = memw(r6) }
	{ call fmaxf.1.0; memw(r29+60) = r3 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2); immext(#0x0); r21 = #0x0 }
	{ r2 = sfsub(r21,r27) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r18 = convert_uw2sf(r0):chop; r0 = sfsub(r26,r16); r2 = #0x0 }
	{ p1 = cmp.gt(r18,#0xFF) }
	{ p0 = cmp.gt(r18,#0xFFFFFFFF); if (p1) r18 = #0xFFFFFFFF }
	{ call fmaxf.1.0; if (!p0) r18 = add(r2,#0x0); r2 = #0x0; memb(r29+33) = r2.new }
	{ r2 = #0x0 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r2 = sfsub(r21,r16) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ r2 = convert_uw2sf(r0):chop }
	{ p0 = cmp.gt(r2,#-0x1); if (p0.new) jump:nt 00019F68; nop }

l00019F58:
	{ p0 = cmp.gt(r2,#0xFF); if (p0.new) r3 = #0xFF; if (!p0.new) r3 = zxtb(r2) }
	{ memw(r29+132) = r3 }

l00019F68:
	{ r2 = mpyi(r20,r17); p0 = cmp.eq(r17,#0x7); r6 = memw(r29+244); r9 = memw(r29+128) }
	{ r4 = add(#0x3,mpyi(r24,r25)); r8 = asr(r23,#0x1); r5 = memw(r29+112); memb(r29+41) = r8.new }
	{ r0 = p0; p2 = cmp.eq(r9,#0x2); p1 = cmp.eq(r5,#0x3) }
	{ r21 = mpyi(r2,r6); r7 = and(r3,#0xFFFFFFF0); r2 = memw(r29+224); memw(r29+220) = r0 }
	{ p3 = cmp.eq(r7,#0xA0); p0 = cmp.eq(r7,#0x20); r5 = add(r5,#0xF); r0 = memw(r29+220) }
	{ r1 = mpyi(r24,r25); p0 = fastcorner9(p2,p0); immext(#0x3FFFFFC0); r4 = and(r4,#0x3FFFFFFC) }
	{ r6 = asr(r22,#0x1); p3 = fastcorner9(p2,p3); r12 = memw(r29+228); memb(r29+26) = r6.new }
	{ r26 = and(r5,#0xFFFFFFF0); r5 = cmp.eq(r21,r7); r8 = r25 }
	{ r2 = mpyi(r2,r24); p0 = fastcorner9(p1,p0); r6 = and(r18,#0xFF); memw(r29+76) = r1 }
	{ p1 = r0; r3 = asl(r19,#0x2); r23 = r9; r13 = and(r12,#0xFFFFFFE0) }
	{ p1 = fastcorner9(p1,p2); r4 = asl(r4,#0x2); p2 = cmp.eq(r17,#0x3); r25 = r24 }
	{ p0 = fastcorner9(p2,p0); p2 = cmp.eq(r20,#0x7); memw(r29+244) = r13; memw(r29+96) = r5 }
	{ p2 = fastcorner9(p2,p1); p1 = cmp.eq(r20,#0x3) }
	{ p3 = or(p2,and(p0,p1)); r0 = p2; p0 = cmp.eq(r21,r7) }
	{ r0 = memw(r29+236); memw(r29+100) = r0 }
	{ p1 = r0; if (p1.new) jump:nt 0001A078; if (!p1.new) r22 = memw(r29+116); if (p1.new) r24 = memw(r29+108) }

l0001A050:
	{ p1 = !cmp.eq(r20,00000001); p2 = !cmp.eq(r17,00000001); r24 = memw(r29+108) }
	{ p1 = or(p2,p1); r0 = p3; memb(r29+30) = r0.new }
	{ if (!p1.new) jump:nt 0001A090 }

l0001A070:
	{ p0 = and(p0,p0); jump 0001A08C }

l0001A078:
	{ r0 = p3; r5 = #0x0; r22 = memd(r29+116) }
	{ if (p3) jump:nt 0001A0A8; memw(r29+200) = r5; memw(r29+120) = r0 }

l0001A08C:
	{ r5 = mux(p0,#0x1,#0x0); r0 = memw(r29+120); memb(r29+50) = r5.new }

l0001A090:
	{ r0 = memw(r29+120); memb(r29+50) = r5.new }

l0001A09C:
	{ if (!p0.new) jump:nt 0001A0D0; if (!p0.new) r6 = memw(r29-92); if (p0.new) memw(r29-20) = r6 }

l0001A0A8:
	{ r6 = asl(r17,#0x1); r16 = r7; r5 = r25; memw(r29+236) = r6 }
	{ r7 = mpyi(r8,r16); r5 += add(r6,#0x2); memw(r29+232) = r8 }
	{ r5 = mpyi(r7,r5); jump 0001A0E8 }
0001A0CC                                     A5 3D 78 50             .=xP

l0001A0D0:
	{ r5 = add(r5,r22); memw(r29+232) = r8 }
	{ r5 = mpyi(r5,r26); r6 = add(r6,r17) }
	{ r6 = addasl(r24,r6,#0x1) }
	{ r5 = mpyi(r5,r6) }

l0001A0E8:
	{ r6 = memw(r29+252) }
	{ r6 = memw(r6+4) }
	{ r6 += add(r3,#0x7F); memw(r29+188) = r6 }
	{ r3 = and(r6,#0xFFFFFF80); memb(r29+42) = r3.new }
	{ r18 = and(r3,#0xFFFFFF80); r5 = memw(r29+256) }
	{ r7 = add(r18,#0x17F) }
	{ r6 = and(r7,#0xFFFFFF80); r5 = memw(r5+40); memb(r29+46) = r6.new }
	{ r27 = memw(r5) }
	{ r4 = memw(r29+244) }
	{ p0 = cmp.eq(r19,r4); r19 = and(r6,#0xFFFFFF80) }
	{ r7 = add(r19,#0x17F) }
	{ r0 = p0; r3 = and(r7,#0xFFFFFF80); memb(r29+17) = r0.new }
	{ r3 = and(r3,#0xFFFFFF80); r2 = memw(r29+60) }
	{ if (p0) r3 = add(r2,#0x0) }
	{ call fn00009530; memw(r29+204) = r3 }
	{ r2 = mpyi(r24,r22); p0 = cmp.eq(r21,r16) }
	{ r3 = memd(r29+112); memd(r29+48) = r1:r0 }
	{ r2 = mpyi(r2,r3) }
	{ if (!p0) jump:nt 0001A17C; memw(r29+80) = r2 }

l0001A170:
	{ r4 = r23; r0 = memd(r29+120) }
	{ p0 = r0; if (!p0.new) jump:nt 0001A1A0 }

l0001A17C:
	{ r24 = r25; r26 = r16; r20 = #0x1; r22 = memw(r29+232) }
	{ r17 = #0x1; r2 = #0x0; r4 = #0x1; r7 = #0x0 }
	{ memw(r29+104) = r2; memw(r29+164) = r7 }

l0001A1A0:
	{ r3 = mpyi(r26,r20); r2 = memw(r29+104); memw(r29+224) = r20 }
	{ r2 = add(r22,r2); immext(#0x60000); r0 = #0x60000; memw(r29+116) = r22 }
	{ r2 = mpyi(r2,r26); memw(r29+228) = r4; memw(r29+192) = r26 }
	{ r20 = mpyi(r3,r17); r2 = mpyi(r4,r2); memw(r29+252) = r2; memw(r29+108) = r24 }
	{ r0 -= asl(r20,#0x6); r1 = asl(r2,#0x2) }
	{ call fn00009750 }
	{ r2 = memw(r29+240) }
	{ r2 = add(r2,r0) }
	{ call fn00009750; r1:r0 = combine(r0,r2) }
	{ r2 = r25; r3 = add(r0,#0x1); r21 = memw(r29+248) }
	{ r2 += lsr(r2,#0x1F); r4 = clrbit(r3,#0x0); p1 = cmp.eq(r21,#0x1) }
	{ r3 += lsr(r3,#0x1F); p0 = cmp.eq(r4,#0x2) }
	{ r25 -= asr(r2,#0x1); r2 = asr(r2,#0x1); memb(r29+55) = r2.new }
	{ r26 = p1; if (!p1) r25 = add(r2,#0x0) }
	{ if (p0) r24 = #0x2; memw(r29+256) = r26; memw(r29+176) = r25 }
	{ r1:r0 = combine(r24,r25); memw(r29+180) = r24 }
	{ r20 = and(#0x3D,lsr(r20,#0xB)); call fn00009750 }
	{ r2 = asr(r20,#0x1F); r3 = #0x40; memw(r29+212) = r0 }
	{ r20 += lsr(r2,#0x1A) }
	{ r2 = asr(r20,#0x6); r20 = r17 }
	{ r2 = combine(r3.l,r2.l) }
	{ r1:r0 = combine(r3,r2) }
	{ memd(r29+136) = r1:r0 }
	{ l2fetch(r27,r1:r0) }
	{ r0 = addasl(r18,r21,#0x7); r2 = #0x80; r1 = #0x0 }
	{ call fn000095F0; r17 = #0x0; memw(r29+208) = r0 }
	{ r7 = mpyi(r24,r25); r2 = memw(r29+72); immext(#0x4C0); immext(#0x4C0); if (!cmp.gt(r17.new,#0x0)) jump:nt 0001A314 }

l0001A29C:
	{ r2 = memw(r29+248); r0 = memw(r29+256) }
	{ r2 = asl(r2,#0x5); r6 = add(r24,#0xFFFFFFFF); r3 = memw(r29+220); memb(r29+43) = r6.new }
	{ r19 = addasl(r19,r2,#0x2); r3 = #-0x1; memw(r29+92) = r7 }
	{ memw(r29+220) = r3 }
	{ r2 = memw(r29+112); r3 = memw(r29+128) }
	{ r2 = mpyi(r3,r2); r1 = memw(r29+212); r0 = memw(r29+76) }
	{ r5 = mpyi(r2,r22); r1 += add(r20,#0x1); r6 = memw(r29+244); r4 = memw(r29+220) }
	{ r3 = add(#0x3F,mpyi(r1,r5)); memw(r29+88) = r18; memw(r29+84) = r17 }
	{ r2 = memw(r29+80); memw(r29+156) = r5 }
	{ r7 = mpyi(r2,r18); r2 = mpyi(r0,r6) }
	{ r4 = mpyi(r5,r4); r0 = asr(r3,#0x1F); r5 = memw(r29+160); memw(r29+152) = r7 }
	{ r1 = mpyi(r2,r18); r3 += lsr(r0,#0x1A); memb(r29+54) = r1.new }

l0001A314:
	{ r3 += lsr(r0,#0x1A); memb(r29+54) = r1.new }

l0001A320:
	{ r2 = asr(r3,#0x6); r3 = #0x40 }
	{ r2 = combine(r3.l,r2.l) }
	{ r1:r0 = combine(r3,r2) }
	{ l2fetch(r5,r1:r0) }
	{ p0 = cmp.gt(r6,#0x0); if (!p0.new) jump:nt 0001A6F4; if (p0.new) r3 = memw(r29+88) }

l0001A33C:
	{ r2 = add(r20,#0x1); r22 = #0x0; r4 = memw(r29+80); memb(r29+37) = r2.new }
	{ r4 = add(r2,mpyi(r4,r3)); memb(r29+60) = r4.new }

l0001A358:
	{ if (!p0.new) jump:nt 0001A6E8; r23 = #0x0; r6 = r25; p0 = cmp.gt(r24,#0x0) }
	{ r26 = memw(r29+92); r21 = memw(r29+220) }

l0001A370:
	{ r2 = mpyi(r6,r24); p1 = cmp.eq(r23,#0x0); r3 = memw(r29+172); r4 = memw(r29+220) }
	{ r7 = memw(r29+212); r5 = memw(r29+200) }
	{ p2 = cmp.eq(r3,r23); r3 = add(r25,r4); p0 = cmp.gt(r2,r26); if (p1) r2 = #0x0 }
	{ if (p2) r17 = sub(r3,r21); if (p2) r3 = add(r4,#0x0); if (!p1) r2 = add(r20,#0xFFFFFFFF); r4 = mux(p0,#0x1,#0x0) }
	{ r25 = r20; r4 = add(r4,r7); p0 = cmp.eq(r5,#0x0) }
	{ if (!p2) r17 = add(r4,#0x0); if (!p2) r3 = add(r4,r21) }
	{ if (!p0) jump:nt 0001A3D0; r5 = add(r17,r20); r6 = sub(r6,r17) }

l0001A3C4:
	{ p1 = cmp.eq(r14,#0x0); if (!p1.new) jump:nt 0001A418; if (!p1.new) r8 = memw(r29-128); if (!p1.new) r9 = memw(r29-4) }

l0001A3D0:
	{ r25 = r20; r4 = memw(r29+148); r5 = memw(r29+156) }
	{ r4 = add(r4,r17); r6 = memw(r29+152); memw(r29+256) = r6 }
	{ r4 = add(#0x3F,mpyi(r4,r5)); r3 = mpyi(r5,r3); r5 = memw(r29+160) }
	{ r5 += add(r3,r6); r7 = asr(r4,#0x1F) }
	{ r4 += lsr(r7,#0x1A) }
	{ r7 = asr(r4,#0x6); r4 = #0x40 }
	{ r3 = combine(r4.l,r7.l) }
	{ r1:r0 = combine(r4,r3) }
	{ l2fetch(r5,r1:r0) }
	{ jump 0001A45C }

l0001A418:
	{ r4 = mpyi(r9,r8); r6 = memw(r29+164); memw(r29+256) = r6 }
	{ r6 = add(r20,r6); r7 = memw(r29+248) }
	{ r6 = mpyi(r6,r7); r7 = #0x40 }
	{ r6 += mpyi(r3,r8); r3 = add(#0x3F,mpyi(r4,r5)); r4 = memw(r29+168) }
	{ r1 = asr(r3,#0x1F) }
	{ r6 = add(r4,mpyi(r6,r9)); r3 += lsr(r1,#0x1A) }
	{ r3 = asr(r3,#0x6) }
	{ r3 = combine(r7.l,r3.l) }
	{ r1:r0 = combine(r7,r3) }
	{ l2fetch(r6,r1:r0) }

l0001A45C:
	{ if (!p0) jump:nt 0001A574; r6 = r16; if (p0) r3 = memw(r29-92); r20 = memw(r29+240) }

l0001A46C:
	{ p0 = cmp.eq(r22,#0x0); if (p0.new) r16 = add(r6,#0x0); r4 = memw(r29+248); r7 = memw(r29+252) }
	{ r3 = add(r25,r3); r20 = r7; if (p0) r18 = add(r27,#0x0) }
	{ r5 = mpyi(r3,r4); r3 = memw(r29+168) }
	{ r20 = add(r3,mpyi(r20,r5)); if (!p0) jump:nt 0001A574; if (p0) r1 = add(r21,r5) }

l0001A49C:
	{ immext(#0x10C40); r7 = add(PC,#0x10C74); r0 = p2; memb(r29+31) = r0.new }
	{ r4 = memw(r7-112); immext(#0xFFFFFFC0); r3 = memw(r7-32) }
	{ immext(#0xFFFFFFC0); r6 = memw(r7-16); immext(#0xFFFFFFC0); r24 = memw(r7-64) }
	{ r0 = memw(r29+120) }
	{ p0 = r0; if (!p0.new) jump:nt 0001A510; if (!p0.new) r7 = sub(r17,r2); if (!p0.new) r6 = add(r2,r21) }

l0001A4E0:
	{ r8 = memw(r29+100); r7 = memw(r29+252) }
	{ r2 = memw(r29+236); r0 = memw(r29+240) }
	{ p0 = r8; if (!p0.new) r24 = add(r6,#0x0); if (!p0.new) r3 = add(r4,#0x0); r6 = memw(r29+168) }
	{ r5:r4 = combine(r17,r21) }
	{ r1 = add(r6,mpyi(r1,r7)); callr r24 }
	{ jump 0001A568 }

l0001A510:
	{ r3 = memw(r29+96); memb(r29+8) = r3.new }
	{ r0 = memw(r29+232) }
	{ r2 = memd(r29+104); memw(r29+28) = r4 }
	{ memw(r29+24) = r2; memw(r29+20) = r0 }
	{ r9 = memw(r29+228); r8 = memw(r29+224) }
	{ r12 = memw(r29+252); memw(r29+8) = r9 }
	{ r5 += mpyi(r6,r9); r8 = memw(r29+168); memw(r29+4) = r8 }
	{ r1 = memw(r29+108); memw(r29) = r25 }
	{ r4 = memw(r29+236); memw(r29+16) = r7 }
	{ r5 = add(r8,mpyi(r5,r12)); r0 = memw(r29+240); memw(r29+12) = r6 }
	{ r3 = memd(r29+112); r2 = memd(r29+116) }
	{ call fast_im2col_co }

l0001A568:
	{ r27 = r18; r6 = r16; r0 = memd(r29+124) }
	{ p2 = r0 }

l0001A574:
	{ r2 = memw(r29+248); if (!cmp.eq(r2.new,#0x0)) jump:t 0001A59C }

l0001A580:
	{ r3 = memw(r29+244); r1:r0 = memd(r29+136) }
	{ p0 = cmp.gt(r3,r2); if (!p0.new) r2 = #0x0 }
	{ if (!p2) r2 = add(r22,#0x0) }
	{ r2 = add(r27,mpyi(r2,r6)) }
	{ l2fetch(r2,r1:r0) }

l0001A59C:
	{ r24 = r22; r2 = r6; r16 = r6; r18 = memw(r29+236) }
	{ r24 = add(r27,mpyi(r24,r6)); r3 = sub(#0x0,r18) }
	{ call gvmsumb_asm; r1:r0 = combine(r19,r24) }
	{ r7 = mpyi(r18,r16); p0 = cmp.eq(r22,#0x0); r12 = memw(r29+228); r13 = memw(r29+252) }
	{ r0 = mpyi(r13,r12); if (p0) r1 = add(r27,#0x0); if (!p0) r7 = memw(r29-48) }
	{ r0 = add(r20,mpyi(r0,r21)); if (!p0) jump:nt 0001A664; if (!p0) r20 = add(r25,#0x0) }

l0001A5EC:
	{ r20 = r25; r2 = memw(r29+196); memb(r29+10) = r2.new }
	{ r2 = memw(r29+208) }
	{ memw(r29+36) = r5; memw(r29+24) = r2 }
	{ r4 = memw(r29+232); r14 = memw(r29+132) }
	{ r6 = mpyi(r21,r4); r8 = sub(#0x0,r14); memw(r29+20) = r19; memw(r29+12) = r17 }
	{ r3 = memw(r29+192); r5 = memw(r29+244) }
	{ r12 = combine(r12.l,r3.l); r3 = r13; r13 = memw(r29+224); memw(r29+8) = r20 }
	{ r9 = mpyi(r6,r5); r2 = memw(r29+204); memw(r29+4) = r13 }
	{ r7 = mpyi(r7,r14); r15 = memw(r29+184); memw(r29) = r12 }
	{ r6 = addasl(r15,r6,#0x2); r8 = memw(r29+216); memw(r29+28) = r8 }
	{ r2 += add(r9,r8); memw(r29+32) = r7; memw(r29+16) = r6 }
	{ call gvconvsum2dbbb_asm }
	{ jump 0001A6CC }

l0001A664:
	{ r2 = memw(r29+196); memb(r29+8) = r2.new }
	{ r2 = memw(r29+188) }
	{ r6 = mpyi(r21,r4); r7 = addasl(r2,r22,#0x2); r5 = memw(r29+244); memw(r29+12) = r17 }
	{ r8 = mpyi(r6,r5); r3 = memw(r29+192); memw(r29+8) = r20 }
	{ r12 = combine(r12.l,r3.l); r3 = r13; r1 = memw(r29+216); r2 = memw(r29+204) }
	{ r9 = add(r22,r1); r1 = r24; r13 = memw(r29+224); memb(r29+1) = r13.new }
	{ r14 = memw(r29+184); memw(r29+28) = r7 }
	{ r6 = addasl(r14,r6,#0x2); memw(r29) = r12 }
	{ call gvconv2dbbb_asm; memw(r29+16) = r6 }

l0001A6CC:
	{ r21 = add(r17,r21); r25 = memw(r29+176); r24 = memw(r29+180) }
	{ r26 = sub(r26,r25); r6 = memw(r29+256) }
	{ r23 = add(r23,#0x1); if (!cmp.eq(r23.new,r24)) jump:t 0001A370 }

l0001A6EC:
	{ r22 = add(r22,#0x20); if (cmp.gtu(r2,r22.new)) jump:t 0001A358 }

l0001A6F4:
	{ r2 = memw(r29+208); r0 = memw(r29+68) }

l0001A6F8:
	{ r0 = memw(r29+68) }
	{ r18 = memw(r29+88) }
	{ p0 = r0; if (p0.new) jump:nt 0001A74C; r17 = memw(r2); if (!p0.new) r2 = memw(r29-24) }

l0001A710:
	{ r3 = memw(r29+220); r7 = memw(r29+76) }
	{ r0 = memw(r29+204); r5 = memw(r29+64) }
	{ r1 = mpyi(r25,r2); r3 = mpyi(r3,r2); r2 = memw(r29+244) }
	{ r4 = r1 }
	{ r6 = mpyi(r3,r2); r3 += mpyi(r18,r7); r7 = memw(r29+216) }
	{ r0 += add(r7,r6); r6 = memw(r29+60) }
	{ r3 = add(r6,mpyi(r3,r5)); call unpad2d_bytes }

l0001A74C:
	{ r2 = memd(r29+84); r7 = memd(r29+72) }
	{ r2 = max(r2,r17); r22 = memw(r29+116) }
	{ r17 = r2; r18 = add(r18,#0x1); immext(#0xFFFFFB40); immext(#0xFFFFFB40) }
	{ memw(r16+8) = r17 }
	{ r3:r2 = combine(r1,r0); r0 = add(r16,#0x34); r1 = #0x1; r5:r4 = memd(r29+48) }
	{ r3:r2 = sub(r3:r2,r5:r4); r19:r18 = memd(r29+296); r21:r20 = memd(r29+288) }
	{ r23:r22 = memd(r29+280); r25:r24 = memd(r29+272) }
	{ r17:r16 = memd(r29+304); memd(r16+72) = r3:r2 }
	{ jump 00009730; r27:r26 = memd(r29+264); deallocframe }

;; fmaxf.1.0: 0001A7A8
;;   Called from:
;;     00018C9C (in supernode_execute_hvx)
;;     00019354 (in supernode_check_ref)
;;     000196D8 (in supernode_execute_ref)
;;     00019700 (in supernode_execute_ref)
;;     00019730 (in supernode_execute_ref)
;;     00019EDC (in supernode_execute_hvx_slice)
;;     00019F20 (in supernode_execute_hvx_slice)
fmaxf.1.0 proc
	{ immext(#0x38D1B700); r2 = #0x38D1B717 }
	{ jump fn00009600; r1:r0 = combine(r0,r2) }
0001A7B8                         00 00 00 00 00 00 00 00         ........

;; avgpool_execute: 0001A7C0
avgpool_execute proc
	{ r21 = r0; memd(r29-32) = r21:r20; allocframe(#0xC8) }
	{ r2 = memw(r21+4); memd(r29+184) = r19:r18 }
	{ r20 = memb(r21+32) }
	{ r3 = memw(r21+8); memd(r29+192) = r17:r16 }
	{ p0 = cmp.eq(r20,#0x0); r19 = memw(r2); r17 = memw(r2+8) }
	{ r18 = memw(r2+4); memd(r29+160) = r25:r24 }
	{ r7 = memw(r19+8) }
	{ r23:r22 = combine(r7,r1); memd(r29+152) = r27:r26; memd(r29+168) = r23:r22 }
	{ r0 = r7; r16 = memw(r3); r6 = memw(r17+4) }
	{ r1 = p0; r24 = memw(r19); r26 = memw(r17+8) }
	{ if (!p0) r2 = add(r26,r23); r25 = memw(r19+12); r27 = memw(r19+4) }
	{ r3 = memw(r18+4); memw(r29+72) = r6 }
	{ r6 = memw(r18+8) }
	{ memw(r29+64) = r3; memw(r29+108) = r6 }
	{ if (p0) jump:nt 0001A84C; memw(r29+144) = r1 }

l0001A828:
	{ p0 = cmp.eq(r12,#0x2); if (p0.new) jump:nt 0001A848; if (p0.new) r3 = memw(r29+108) }

l0001A830:
	{ p0 = cmp.eq(r12,#0x1); if (!p0.new) jump:nt 0001A858; r2 = #0x0; memb(r29+31) = r2.new }

l0001A840:
	{ r0 += add(r23,#0xFFFFFFFF); jump 0001A84C }

l0001A848:
	{ r0 = sub(r2,r3) }

l0001A84C:
	{ call fn00009760; r1 = r26 }
	{ memw(r29+124) = r0 }

l0001A858:
	{ p0 = cmp.eq(r12,#0x2); if (p0.new) jump:nt 0001A894; nop }

l0001A860:
	{ p0 = cmp.eq(r12,#0x1); if (p0.new) jump:nt 0001A888; if (!p0.new) r0 = memw(r29-112); if (p0.new) r1 = memw(r29+72) }

l0001A86C:
	{ r2 = #0x0; memb(r29+17) = r2.new }
	{ if (!p0.new) jump:nt 0001A8AC; if (p0.new) r0 = add(r27,#0x0); if (p0.new) r1 = memw(r29+72) }

l0001A884:
	{ jump 0001A8A0 }

l0001A888:
	{ r0 = r1 }
	{ r0 += add(r27,#0xFFFFFFFF); jump 0001A8A0 }

l0001A894:
	{ r1 = memd(r29+72); r3 = memd(r29+64) }
	{ r2 = add(r1,r27) }
	{ r0 = sub(r2,r3) }

l0001A8A0:
	{ call fn00009760 }
	{ memw(r29+68) = r0 }
	{ immext(#0xE040); r4 = add(PC,#0xE057); r1 = #0x50; r2 = r22 }

l0001A8AC:
	{ r4 = add(PC,#0x17); r1 = #0x50; r2 = r22 }

l0001A8B8:
	{ r7 = memw(r16+16); r3 = memw(r19+16) }
	{ memw(r29+104) = r7; memw(r29+100) = r3 }
	{ call logmsg_function; memw(r29) = r21 }
	{ r2 = memw(r18); if (!cmp.eq(r2.new,#0x1)) jump:t 0001A8E8 }

l0001A8D4:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001A8EC }

l0001A8DC:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001A8EC }

l0001A8E4:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001A908 }

l0001A8E8:
	{ immext(#0xE000); r3 = add(PC,#0xE034); r1 = #0x55 }

l0001A8EC:
	{ r3 = add(PC,#0x34); r1 = #0x55 }

l0001A8F4:
	{ call errlog_function; r2 = r22 }

l0001A8F8:
	{ r2 = r22 }
	{ jump 0001AC2C; r0 = #0xFFFFFFFF }
0001A904             02 59 18 ED                             .Y..        

l0001A908:
	{ r1 = #0x57; r3 = memd(r29+124); r4 = memd(r29+68) }
	{ r2 = mpyi(r2,r3); r7 = memw(r16+20) }
	{ r2 = mpyi(r2,r4) }
	{ r2 = asl(r2,#0x2); if (!cmp.gtu(r2.new,r7)) jump:t 0001A930 }

l0001A928:
	{ r3 = add(PC,#0x10); jump 0001A8F8 }

l0001A930:
	{ p0 = cmp.gt(r24,#0x0); r3 = memb(r21+32); if (!cmp.eq(r3.new,#0x0)) jump:t 0001A94C }

l0001A940:
	{ r3 = add(PC,#0x6); jump 0001A8F8; r1 = #0x58 }

l0001A94C:
	{ immext(#0x0); if (p0) r20 = #0x0; r2 = memd(r29+68); memw(r16+24) = r2 }
	{ immext(#0x3F800000); if (p0) r26 = #0x3F800000; r7 = memw(r29+124); memw(r29+128) = r26 }
	{ memw(r29+12) = r22 }
	{ memw(r29+16) = r21; memw(r16+4) = r2 }
	{ memw(r16) = r24; memw(r16+8) = r7 }
	{ if (!p0) jump:nt 0001AC0C; memw(r16+12) = r25 }

l0001A984:
	{ r18 = mpyi(r23,r25); r2 = memd(r29+124); r4 = memd(r29+68) }
	{ r8 = memw(r29+108); r7 = memw(r29+64) }
	{ r2 = add(r2,#0xFFFFFFFF); r4 = add(r4,#0xFFFFFFFF); r6 = memw(r29+128); r1 = memw(r29+72) }
	{ r3 = sub(r8,r23); r5 = sub(r7,r27); memw(r29+120) = r23; memw(r29+56) = r27 }
	{ r2 = add(r3,mpyi(r2,r6)); r4 = add(r5,mpyi(r4,r1)); r3 = sub(#0xFFFFFFFF,r27) }
	{ r4 += lsr(r4,#0x1F); r3 = sub(#0xFFFFFFFF,r23); r6 = #0x0; memw(r29+48) = r3 }
	{ r2 += lsr(r2,#0x1F); r5 = asl(r25,#0x2); memw(r29+96) = r3 }
	{ r6 = asr(r4,#0x1); r3 = #0x0; memw(r29+60) = r6; memw(r29+92) = r5 }
	{ r2 = asr(r2,#0x1); r4 = sub(#0x0,r6); memw(r29+32) = r3; memw(r29+44) = r6 }
	{ r5 = add(r2,sub(#0x7F,r8)); r3 = add(r6,sub(#0x7F,r7)); r2 = sub(#0x0,r2); memw(r29+88) = r2 }
	{ memw(r29+24) = r4; memw(r29+20) = r3 }
	{ memw(r29+40) = r5; memw(r29+36) = r2 }

l0001AA08:
	{ r2 = memw(r29+68); memw(r29+28) = r24 }
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 0001ABEC }

l0001AA14:
	{ r2 = memw(r29+24); memb(r29+21) = r2.new }
	{ r3 = memd(r29+32); r7 = memd(r29+20) }
	{ r2 = mpyi(r3,r2); r7 = #0x0; memw(r29+80) = r7 }
	{ memw(r29+52) = r2; memw(r29+76) = r7 }

l0001AA30:
	{ r2 = memw(r29+124); if (!cmp.gt(r2.new,#0x0)) jump:nt 0001ABC8 }

l0001AA3C:
	{ r2 = memd(r29+72); r5 = memd(r29+76) }
	{ r2 = mpyi(r5,r2); r1 = memd(r29+48); r7 = memd(r29+80) }
	{ r4 = memd(r29+52); r3 = memd(r29+84) }
	{ r5 = max(r7,r1); r4 = add(r5,r4); r1 = #0x0; r7 = memd(r29+40) }
	{ r3 = max(r3,r6); r5 = sub(#0xFFFFFFFF,r5); r1 = memw(r29+44); memw(r29+132) = r1 }
	{ r2 = sub(r2,r1); r5 = memw(r29+124); memw(r29+144) = r5 }
	{ r4 = mpyi(r4,r5); r7 = memw(r29+64); memw(r29+136) = r7 }
	{ r16 = max(r2,r6); r2 = add(r2,r7); r7 = memd(r29+56) }
	{ r27 = min(r7,r2); r1 = memd(r29+60); memw(r29+116) = r4 }
	{ r3 = add(r1,r3); r4 = memw(r29+120) }
	{ r3 = mpyi(r4,r3); r0 = memw(r29+36); memb(r29+35) = r0.new }

l0001AAA8:
	{ if (!p0.new) jump:nt 0001AB94; p0 = cmp.gt(r25,#0x0); if (p0.new) r3 = memw(r29-124); if (p0.new) r2 = memw(r29-128) }

l0001AAB8:
	{ r7 = #0x0; r21 = #0x0 }
	{ r1 = memw(r29+140); r4 = memw(r29+116) }
	{ r2 = mpyi(r3,r2); r3 = add(r3,r4); r5 = memw(r29+96); r6 = memw(r29+136) }
	{ r4 = max(r1,r7); r3 = mpyi(r3,r25); r1 = memd(r29+112); r0 = memd(r29+88) }
	{ r5 = max(r6,r5); r6 = add(r1,r4); r19 = memd(r29+92); r1 = memd(r29+100) }
	{ r2 = sub(r2,r0); r5 = sub(#0xFFFFFFFF,r5); r0 = memw(r29+108) }
	{ r24 = max(r2,r7); r19 = add(r1,mpyi(r19,r6)); r7 = memd(r29+104); r1 = memd(r29+120) }
	{ r23 = addasl(r7,r3,#0x2); r2 = add(r2,r0); r17 = sub(r5,r4) }
	{ r22 = min(r1,r2) }

l0001AB14:
	{ if (!p0.new) jump:nt 0001AB7C; r1:r0 = combine(r20,r20); p0 = cmp.gt(r27,r16); if (p0.new) r2 = memw(r29-112) }

l0001AB24:
	{ r1:r0 = combine(r20,r20) }
	{ r3 = sub(r2,r16); r2 = r19 }
	{ loop1(0001AB34,r3) }
	{ r3 = addasl(r2,r25,#0x2); if (!p0.new) jump:nt 0001AB70; r5 = add(r17,#0xFFFFFFFF); p0 = cmp.gt(r22,r24) }

l0001AB44:
	{ loop0(0001AB58,r5); r1 = sfadd(r1,r26); p0 = cmp.gtu(r17,#0x1); r4 = memw(r2) }
	{ if (!p0) jump:nt 0001AB6C }

l0001AB58:
	{ r0 = sfadd(r0,r4); r1 = sfadd(r1,r26); r4 = memw(r3) }
	{ r3 = addasl(r3,r25,#0x2); nop }

l0001AB6C:
	{ r0 = sfadd(r0,r4) }

l0001AB70:
	{ r2 = addasl(r2,r18,#0x2); nop; nop }

l0001AB7C:
	{ call fn00009610; r21 = add(r21,#0x1) }
	{ r23 = add(r23,#0x4); p0 = cmp.eq(r21,r25); r19 = add(r19,#0x4); memw(r23) = r0 }
	{ if (!p0) jump:nt 0001AB14 }

l0001AB94:
	{ r4 = memw(r29+132); r6 = memw(r29+124) }
	{ r4 = add(r4,#0x1); r2 = memw(r29+128); r3 = memw(r29+140) }
	{ r3 = add(r3,r2); p0 = cmp.eq(r4,r6); r7 = memw(r29+136); memw(r29+132) = r4 }
	{ r5 = sub(r7,r2); memw(r29+140) = r3 }
	{ if (!p0) jump:nt 0001AAA8; memw(r29+136) = r5 }

l0001ABC8:
	{ r4 = memd(r29+76); r6 = memd(r29+68) }
	{ r4 = add(r4,#0x1); r2 = memd(r29+72); r3 = memd(r29+84) }
	{ r3 = add(r3,r2); p0 = cmp.eq(r4,r6); r7 = memd(r29+80); memw(r29+76) = r4 }
	{ r5 = sub(r7,r2) }
	{ if (!p0) jump:nt 0001AA30; memw(r29+84) = r3; memw(r29+80) = r5 }

l0001ABEC:
	{ r4 = memw(r29+32); r24 = memw(r29+28) }
	{ r4 = add(r4,#0x1); r2 = memd(r29+56); r3 = memd(r29+60) }
	{ p0 = cmp.eq(r4,r24); r3 = add(r3,r2); memw(r29+32) = r4 }
	{ if (!p0) jump:nt 0001AA08; memw(r29+60) = r3 }

l0001AC0C:
	{ immext(#0xDD40); r4 = add(PC,#0xDD48); r2 = memw(r29+16); memb(r29) = r2.new }
	{ r2 = memw(r29+12) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001AC2C:
	{ r17:r16 = memd(r29+192); r19:r18 = memd(r29+184) }
	{ r21:r20 = memd(r29+176); r23:r22 = memd(r29+168) }
	{ r25:r24 = memd(r29+160); r27:r26 = memd(r29+152) }
	{ dealloc_return }

;; avgpool_check: 0001AC40
avgpool_check proc
	{ immext(#0xDC00); r4 = add(PC,#0xDC34); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x8C; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x8D; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 0001AC80 }

l0001AC6C:
	{ r3 = add(PC,#0x25) }

l0001AC70:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001AC80:
	{ r1 = #0x8E; r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x1)) jump:t 0001AC94 }

l0001AC90:
	{ r3 = memw(r17+4) }

l0001AC94:
	{ immext(#0xDC00); r3 = add(PC,#0xDC10); jump 0001AC70 }
0001ACA0 83 40 03 B0 22 40 02 B0 14 C2 02 25 04 40 83 91 .@.."@.....%.@..
0001ACB0 FA E0 72 24 70 43 00 00 83 42 49 6A 21 52 00 78 ..r$pC...BIj!R.x
0001ACC0 00 C2 9D A1 D6 FF FF 59 42 C0 91 91 02 40 82 91 .......YB....@..
0001ACD0 0E E0 42 24 6F 43 00 00 83 5D 49 6A C1 52 00 78 ..B$oC...]Ij.R.x
0001ACE0 00 C0 5D 3C C6 FF FF 59 6F 43 00 00 04 5F 49 6A ..]<...YoC..._Ij
0001ACF0 21 53 00 78 02 C0 70 70 08 40 00 5A 00 D1 9D A1 !S.x..pp.@.Z....
0001AD00 00 C0 00 78 40 1F 0C 3E                         ...x@..>        

;; logmsg_function: 0001AD08
;;   Called from:
;;     0001A8C0 (in avgpool_execute)
;;     0001AC24 (in avgpool_execute)
;;     0001AC54 (in avgpool_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001AD2C }

l0001AD18:
	{ r0 = add(PC,#0x1); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001AD2C:
	{ dealloc_return }

;; errlog_function: 0001AD30
;;   Called from:
;;     0001A8F4 (in avgpool_execute)
;;     0001AC70 (in avgpool_check)
;;     0001AD20 (in logmsg_function)
errlog_function proc
	{ immext(#0xDB00); r0 = add(PC,#0xDB25); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001AD54             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; biasadd_f_execute: 0001AD60
biasadd_f_execute proc
	{ immext(#0xDC80); r3 = add(PC,#0xDC81); memd(r29-16) = r17:r16; allocframe(#0x38) }
	{ r17:r16 = combine(r0,r1); r1 = #0x45 }
	{ r2 = memw(r17+4); memd(r29+40) = r19:r18 }
	{ memd(r29+32) = r21:r20; memd(r29+24) = r23:r22 }
	{ r5 = memw(r2+4); memd(r29+16) = r25:r24 }
	{ memd(r29+8) = r27:r26 }
	{ r4 = memw(r5+4); if (!cmp.eq(r4.new,#0x1)) jump:t 0001AE30 }

l0001AD94:
	{ r3 = add(PC,#0x11); r1 = #0x46 }
	{ r4 = memw(r5); if (!cmp.eq(r4.new,#0x1)) jump:t 0001AE30 }

l0001ADA8:
	{ r3 = add(PC,#0x3D); r1 = #0x47 }
	{ r4 = memw(r5+8); if (!cmp.eq(r4.new,#0x1)) jump:t 0001AE30 }

l0001ADBC:
	{ r18 = memw(r3+12); if (cmp.eq(r18.new,r4)) jump:t 0001ADDC }

l0001ADC8:
	{ r3 = add(PC,#0x28); r1 = #0x49; memw(r29+4) = r18 }
	{ r2 = r16; memw(r29) = r4 }
	{ jump 0001AE34 }

l0001ADDC:
	{ immext(#0xDC00); r4 = add(PC,#0xDC28); r23 = memw(r3); r24 = memw(r3+4) }
	{ r1 = #0x4C; r2 = r16; r22 = memw(r3+8); r6 = memw(r17+8) }
	{ r7 = mpyi(r23,r24); r20 = memw(r3+16); r19 = memw(r5+16) }
	{ r25 = memw(r6) }
	{ r3 = mpyi(r7,r22) }
	{ r21 = memw(r25+16); memw(r29) = r17 }
	{ r3 = mpyi(r3,r18) }
	{ r26 = asl(r3,#0x2); call logmsg_function }
	{ immext(#0xDC00); r3 = add(PC,#0xDC02); r1 = #0x4D }
	{ r2 = memw(r25+20); if (!cmp.gtu(r26,r2.new)) jump:t 0001AE40 }

l0001AE30:
	{ r2 = r16 }

l0001AE34:
	{ call errlog_function }
	{ jump 0001AEE4; r0 = #0xFFFFFFFF }

l0001AE40:
	{ r2 = mpyi(r24,r23); memw(r25+24) = r26; memw(r25) = r23 }
	{ r3 = mpyi(r2,r22); memw(r25+4) = r24; memw(r25+12) = r18 }
	{ memw(r25+8) = r22 }
	{ loop1(0001AE68,r3); p0 = cmp.eq(r3,#0x0); if (p0.new) jump:nt 0001AEC8; r2 = #0x0 }

l0001AE68:
	{ p0 = cmp.eq(r10,#0x0); if (p0.new) jump:nt 0001AEBC; r5 = add(r19,r2); r6 = add(r20,r2) }

l0001AE74:
	{ r7 = add(r18,#0xFFFFFFFF); r3 = add(r2,#0x4); p0 = cmp.gtu(r18,#0x1); r4 = add(r21,r2) }
	{ r5 = memw(r5); r6 = memw(r6) }
	{ loop0(0001AE90,r7); if (!p0) jump:nt 0001AEB4 }

l0001AE90:
	{ r5 = sfadd(r6,r5); r6 = add(r20,r3); r1 = add(r19,r3); memb(r4) = r5.new }
	{ r4 = add(r21,r3); r5 = memw(r1); r6 = memw(r6) }
	{ r3 = r7; nop }

l0001AEB4:
	{ r3 = sfadd(r6,r5); memb(r4) = r3.new }

l0001AEBC:
	{ r20 = addasl(r20,r18,#0x2); r21 = addasl(r21,r18,#0x2); nop }

l0001AEC0:
	{ r21 = addasl(r21,r18,#0x2); nop }

l0001AEC8:
	{ immext(#0xDB40); r4 = add(PC,#0xDB64); r1 = #0x58; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l0001AEE4:
	{ r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+16); r27:r26 = memd(r29+8) }
	{ dealloc_return }
0001AEF8                         00 40 00 7F 00 C0 00 7F         .@......

;; biasadd_check: 0001AF00
biasadd_check proc
	{ immext(#0xDA80); r4 = add(PC,#0xDA83); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x5E; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x5F; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 0001AF40 }

l0001AF2C:
	{ r3 = add(PC,#0x34) }
	{ call errlog_function; r2 = r16 }

l0001AF34:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001AF40:
	{ r1 = #0x60; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001AF58 }

l0001AF50:
	{ r3 = add(PC,#0x27); jump 0001AF34 }

l0001AF58:
	{ immext(#0xDA40); r4 = add(PC,#0xDA70); r1 = #0x61; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001AF78
;;   Called from:
;;     0001AE14 (in biasadd_f_execute)
;;     0001AED8 (in biasadd_f_execute)
;;     0001AF14 (in biasadd_check)
;;     0001AF68 (in biasadd_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001AF9C }

l0001AF88:
	{ r0 = add(PC,#0x20); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001AF9C:
	{ dealloc_return }

;; errlog_function: 0001AFA0
;;   Called from:
;;     0001AE34 (in biasadd_f_execute)
;;     0001AF30 (in biasadd_check)
;;     0001AF90 (in logmsg_function)
errlog_function proc
	{ immext(#0xD9C0); r0 = add(PC,#0xD9C4); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001AFC4             00 00 00 00 00 00 00 00 00 00 00 00     ............
0001AFD0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; concat_execute: 0001AFE0
concat_execute proc
	{ immext(#0xDAC0); r4 = add(PC,#0xDAE2); memd(r29-40) = r23:r22; allocframe(#0x40) }
	{ r23 = r0 }
	{ r21 = memw(r23+4); memd(r29+40) = r21:r20 }
	{ r3 = memw(r23+8) }
	{ r25 = r1; r1 = #0x45; memd(r29+48) = r19:r18; memd(r29+24) = r25:r24 }
	{ r2 = r25; r5 = memw(r21+4); r18 = memw(r3) }
	{ memd(r29+56) = r17:r16; memd(r29+16) = r27:r26 }
	{ r16 = memw(r23+16); r17 = memw(r21) }
	{ r19 = memw(r5+8); r20 = memw(r5+4) }
	{ r27 = memw(r5); r22 = memw(r18+16) }
	{ call logmsg_function; memw(r29) = r23 }
	{ r2 = memw(r17+16) }
	{ r2 = memw(r2); if (!cmp.eq(r2.new,#0x3)) jump:t 0001B10C }

l0001B040:
	{ r24 = add(r21,#0x4); r23 = r16; memw(r29+12) = r23 }
	{ r26 = #0x0; p0 = cmp.gt(r23,#0x0); r2 = #0x0; r5:r4 = combine(#0x0,#0x2) }
	{ r17 = mpyi(r3,r27); if (p0) jump:nt 0001B128; if (!p0) r26 = #0x0; if (p0) r3 = add(r24,#0x0) }

l0001B064:
	{ if (p0) r27 = #0x0; memw(r29+8) = r25; memw(r18) = r27 }
	{ memw(r18+4) = r20; memw(r18+8) = r19 }
	{ memw(r18+12) = r26; memw(r18+24) = r2 }
	{ if (!p0) jump:nt 0001B0D8 }

l0001B080:
	{ r18 = r22; r25 = r17; p0 = cmp.gt(r17,#0x0); r16 = memw(r24) }
	{ if (!p0) jump:nt 0001B0C0; r20 = memw(r16+16); r2 = memw(r16+12) }

l0001B098:
	{ r19 = asl(r2,#0x2) }
	{ nop }

l0001B0A0:
	{ call fn00009560; r2 = r19; r1:r0 = combine(r20,r18) }
	{ r18 = addasl(r18,r26,#0x2); r2 = memw(r16+12) }
	{ r20 = addasl(r20,r2,#0x2); r25 = add(r25,#0xFFFFFFFF); if (!cmp.eq(r25.new,#0x0)) jump:t 0001B0A0 }

l0001B0C0:
	{ r22 = addasl(r22,r2,#0x2); r3 = add(r21,#0x8); r21 = r24 }

l0001B0C4:
	{ r3 = add(r21,#0x8); r21 = r24 }

l0001B0CC:
	{ r24 = r3; r27 = add(r27,#0x1); if (!cmp.eq(r27.new,r23)) jump:t 0001B080 }

l0001B0D8:
	{ immext(#0xDA40); r4 = add(PC,#0xDA6A); r2 = memw(r29+12); memb(r29) = r2.new }

l0001B0DC:
	{ r4 = add(PC,#0x2A); r2 = memw(r29+12); memb(r29) = r2.new }

l0001B0EC:
	{ r2 = memw(r29+8) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001B0F8:
	{ r17:r16 = memd(r29+56); r19:r18 = memd(r29+48) }
	{ r21:r20 = memd(r29+40); r23:r22 = memd(r29+32) }
	{ r25:r24 = memd(r29+24); r27:r26 = memd(r29+16) }
	{ dealloc_return }

l0001B10C:
	{ immext(#0xD9C0); r3 = add(PC,#0xD9CF); r1 = #0x46 }

l0001B118:
	{ call errlog_function; r2 = r25 }

l0001B11C:
	{ r2 = r25 }
	{ jump 0001B0F8; r0 = #0xFFFFFFFF }

l0001B128:
	{ loop0(0001B12C,r23) }
	{ r6 = memw(r3) }
	{ r7 = memw(r6+8); if (cmp.eq(r7.new,r19)) jump:t 0001B14C }

l0001B13C:
	{ r3 = add(PC,#0x2E); r1 = #0x49; memw(r29) = r2 }
	{ jump 0001B118 }

l0001B14C:
	{ r7 = memw(r6+4); if (cmp.eq(r7.new,r20)) jump:t 0001B168 }

l0001B158:
	{ r3 = add(PC,#0x2B); r1 = #0x4C; memw(r29) = r2 }
	{ jump 0001B118 }

l0001B168:
	{ r7 = memw(r6); if (cmp.eq(r7.new,r27)) jump:t 0001B184 }

l0001B174:
	{ r3 = add(PC,#0x29); r1 = #0x4F; memw(r29) = r2 }
	{ jump 0001B118 }

l0001B184:
	{ r7 = add(r5,#0x8); r2 = add(r2,#0x1); r5 = r3; r6 = memw(r6+12) }
	{ r3 = r7; r26 = add(r6,r26) }
	{ r4 += mpyi(r17,r6); nop }
	{ r3 = memw(r18+20) }
	{ r2 = asl(r4,#0x2); if (!cmp.gtu(r2.new,r3)) jump:t 0001B064 }

l0001B1B0:
	{ r3 = add(PC,#0x8); jump 0001B11C; r1 = #0x55 }
0001B1BC                                     00 C0 00 7F             ....

;; concat_check: 0001B1C0
concat_check proc
	{ immext(#0xD880); r4 = add(PC,#0xD89A); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x6A; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ immext(#0xD880); r3 = add(PC,#0xD896); r1 = #0x6B; r2 = memw(r17+16) }
	{ r4 = #0x4; if (cmp.gtu(r4.new,r2)) jump:t 0001B24C }

l0001B1F8:
	{ r3 = add(PC,#0xF); r1 = #0x6C }
	{ r4 = memw(r17+20); if (!cmp.eq(r4.new,#0x1)) jump:t 0001B24C }

l0001B20C:
	{ r4 = memw(r17+4) }
	{ r4 = add(r4,#0x4); r5 = add(r5,#0x1); if (!cmp.gtu(r2,r5.new)) jump:nt 0001B234 }

l0001B214:
	{ r5 = add(r5,#0x1); if (!cmp.gtu(r2,r5.new)) jump:nt 0001B238 }

l0001B220:
	{ r3 = add(PC,#0x37); r6 = memw(r4); if (!cmp.eq(r6.new,#0x0)) jump:t 0001B214 }

l0001B230:
	{ r1 = #0x6E }

l0001B234:
	{ immext(#0xD840); r3 = add(PC,#0xD86A); r1 = #0x71; r2 = memw(r17+8) }

l0001B238:
	{ r3 = add(PC,#0x2A); r1 = #0x71; r2 = memw(r17+8) }
	{ r2 = memw(r2); if (!cmp.eq(r2.new,#0x0)) jump:t 0001B25C }

l0001B24C:
	{ call errlog_function; r2 = r16 }

l0001B250:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001B25C:
	{ immext(#0xD840); r4 = add(PC,#0xD84E); r1 = #0x73; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001B27C
;;   Called from:
;;     0001B028 (in concat_execute)
;;     0001B0F0 (in concat_execute)
;;     0001B1D4 (in concat_check)
;;     0001B26C (in concat_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001B2A0 }

l0001B28C:
	{ r0 = add(PC,#0x34); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001B2A0:
	{ dealloc_return }

;; errlog_function: 0001B2A4
;;   Called from:
;;     0001B118 (in concat_execute)
;;     0001B24C (in concat_check)
;;     0001B294 (in logmsg_function)
errlog_function proc
	{ immext(#0xD780); r0 = add(PC,#0xD798); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001B2C8                         00 00 00 00 00 00 00 00         ........

;; conv2d_f_execute_ref: 0001B2D0
conv2d_f_execute_ref proc
	{ allocframe(#0x78) }
	{ r2 = memw(r0+4); r3 = memw(r0+8) }
	{ memd(r29+104) = r19:r18; memd(r29+112) = r17:r16 }
	{ r18 = memb(r0+32); r16 = memw(r2) }
	{ p0 = cmp.eq(r18,#0x0); r19 = memw(r2+4); r4 = memw(r2+8) }
	{ r24 = memw(r16+8); memd(r29+80) = r25:r24 }
	{ memd(r29+72) = r27:r26; memd(r29+96) = r21:r20 }
	{ memd(r29+88) = r23:r22 }
	{ r0 = r24; r21 = memw(r3); memw(r29+44) = r0 }
	{ r2 = memw(r16) }
	{ r3 = memw(r4+8); memw(r29+28) = r4 }
	{ r26 = memw(r19+4); r27 = memw(r19) }
	{ r25 = memw(r19+12); r23 = memw(r16+12) }
	{ r17 = memw(r16+4); r4 = memw(r4+4) }
	{ r2 = p0; r7 = memw(r19+8); memw(r29+20) = r2 }
	{ if (!p0) r2 = sub(r24,r26); memw(r29+48) = r7; memw(r29+56) = r2 }
	{ if (p0) jump:nt 0001B35C }

l0001B338:
	{ p0 = cmp.eq(r10,#0x2); if (p0.new) jump:nt 0001B35C; if (p0.new) r0 = add(r2,r3) }

l0001B340:
	{ r0 = #0x0; p0 = cmp.eq(r18,#0x1); r20 = r1; memw(r29+60) = r3 }
	{ if (!p0) jump:nt 0001B370 }

l0001B350:
	{ r1 = r20; r3 = memd(r29+60) }
	{ r0 = r3 }
	{ r0 += add(r24,#0xFFFFFFFF) }

l0001B35C:
	{ r1 = r3; r22 = r4; r20 = r1; memw(r29+60) = r3 }
	{ call fn00009760 }
	{ r4 = r22 }

l0001B370:
	{ p0 = cmp.eq(r10,#0x2); if (p0.new) jump:nt 0001B3A4; r2 = sub(r17,r27) }

l0001B378:
	{ p0 = cmp.eq(r10,#0x1); if (p0.new) jump:nt 0001B39C; if (!p0) r1:r0 = combine(r4,r4); memw(r29+64) = r0 }

l0001B384:
	{ r18 = r4; r0 = #0x0; r1 = memd(r29+56) }
	{ p0 = r1; if (!p0.new) jump:nt 0001B3B4; if (!p0) r1:r0 = combine(r18,r17) }

l0001B398:
	{ jump 0001B3B0 }

l0001B39C:
	{ r0 += add(r17,#0xFFFFFFFF); jump 0001B3AC }

l0001B3A4:
	{ r0 = add(r2,r4); r1 = r4; memw(r29+64) = r0 }

l0001B3AC:
	{ r18 = r4 }

l0001B3B0:
	{ call fn00009760 }

l0001B3B4:
	{ immext(#0xD840); r4 = add(PC,#0xD854); r2 = memw(r19+16); r22 = memd(r29+44) }
	{ r1 = #0x5E; r19:r18 = combine(r18,r20); memw(r29+36) = r18 }
	{ r2 = r18; r7 = memw(r21+16); memw(r29+56) = r2 }
	{ r3 = memw(r22+28); r16 = memw(r16+16) }
	{ memw(r29+4) = r3; memw(r29+40) = r0 }
	{ memw(r29+32) = r21; memw(r29+52) = r7 }
	{ call logmsg_function; memw(r29) = r22 }
	{ immext(#0xD800); r4 = add(PC,#0xD83E); r1 = #0x5F; memw(r29+12) = r23 }
	{ r2 = r18; memw(r29+4) = r17 }
	{ r21 = memw(r29+20); memw(r29+8) = r24 }
	{ call logmsg_function; memw(r29) = r21 }
	{ immext(#0xD800); r4 = add(PC,#0xD833); r2 = r18; r20 = memd(r29+48) }
	{ r1 = #0x60; memw(r29+12) = r20 }
	{ memw(r29+8) = r26; memw(r29+4) = r27 }
	{ call logmsg_function; memw(r29) = r25 }
	{ immext(#0xD800); r4 = add(PC,#0xD827); r2 = r18; r3 = memd(r29+60) }
	{ r1 = #0x61; memw(r29+4) = r3 }
	{ call logmsg_function; memw(r29) = r19 }
	{ immext(#0xD800); r4 = add(PC,#0xD81F); r1 = #0x62; r2 = r18 }
	{ r3 = memb(r22+32); r22 = memw(r29+40) }
	{ call logmsg_function; memw(r29) = r3 }
	{ immext(#0xD800); r4 = add(PC,#0xD811); memw(r29+12) = r25; memw(r29) = r21 }
	{ r1 = #0x63; r19 = memw(r29+64) }
	{ r2 = r18; memw(r29+8) = r19; memw(r29+4) = r22 }
	{ call logmsg_function }
	{ r2 = mpyi(r21,r25); p0 = cmp.eq(r15,r12); if (p0.new) jump:nt 0001B4B0; r15 = r19 }

l0001B49C:
	{ immext(#0xD800); r3 = add(PC,#0xD800); r1 = #0x65; r2 = r18 }
	{ jump 0001B504 }

l0001B4B0:
	{ r9 = r22; r6 = r21; r3 = memd(r29+32) }
	{ r5 = r3; memw(r29+16) = r18 }
	{ r2 = mpyi(r2,r19); r4 = memw(r3+20) }
	{ r2 = mpyi(r2,r22) }
	{ r2 = asl(r2,#0x2); if (!cmp.gtu(r2.new,r4)) jump:t 0001B4E8 }

l0001B4D4:
	{ r3 = add(PC,#0x21); r1 = #0x67; memw(r29+4) = r2 }
	{ r2 = memd(r29+16); memw(r29) = r4 }
	{ jump 0001B504 }

l0001B4E8:
	{ r4 = memw(r29+28) }
	{ r3 = memw(r4); if (cmp.eq(r3.new,#0x1)) jump:t 0001B510 }

l0001B4F8:
	{ r3 = add(PC,#0x17); r1 = #0x69 }
	{ r2 = memw(r29+16) }

l0001B504:
	{ call errlog_function }
	{ jump 0001B728; r0 = #0xFFFFFFFF }

l0001B510:
	{ p0 = cmp.gt(r6,#0x0); r3 = memw(r4+12); if (cmp.eq(r3.new,#0x1)) jump:t 0001B52C }

l0001B520:
	{ r3 = add(PC,#0x0); jump 0001B504; r1 = #0x6A }

l0001B52C:
	{ if (p0) r2 = add(r15,#0xFFFFFFFF); if (p0) r3 = sub(r26,r24); memw(r5+24) = r2; memw(r5+4) = r9 }
	{ if (p0) r7 = add(r9,#0xFFFFFFFF); if (p0) r4 = sub(r27,r17); memw(r5) = r6; memw(r5+8) = r15 }
	{ if (!p0) jump:nt 0001B704; memw(r5+12) = r25 }

l0001B554:
	{ r5 = memd(r29+60); r6 = memd(r29+36) }
	{ r2 = add(r3,mpyi(r2,r5)); r7 = add(r4,mpyi(r7,r6)); r3 = #0x0; memb(r29+6) = r3.new }
	{ r3 = mpyi(r23,r25); immext(#0x0); r4 = #0x0 }
	{ r2 += lsr(r2,#0x1F); r7 += lsr(r7,#0x1F) }
	{ r5 = mpyi(r5,r25); r6 = asr(r7,#0x1) }
	{ r2 = asr(r2,#0x1); memb(r29+12) = r2.new }

l0001B594:
	{ if (!p0.new) jump:nt 0001B6F4; p0 = cmp.gt(r9,#0x0) }

l0001B59C:
	{ r7 = #0x0; r2 = memw(r29+24); memb(r29+11) = r7.new }
	{ r2 = mpyi(r2,r9); memb(r29+8) = r2.new }

l0001B5B4:
	{ if (!p0.new) jump:nt 0001B6DC; r13 = #0x0; p0 = cmp.gt(r15,#0x0) }
	{ r2 = memd(r29+36); r7 = memd(r29+44) }
	{ r6 = memd(r29+32); r0 = memd(r29+28) }
	{ r2 = mpyi(r7,r2); r7 = add(r7,r6) }
	{ r1 = mpyi(r7,r15); r14 = sub(r2,r0); memb(r29+16) = r1.new }

l0001B5DC:
	{ if (!p0.new) jump:nt 0001B6D4; p0 = cmp.gt(r25,#0x0); if (p0.new) r28 = memw(r29+56) }
	{ r0 = #0x0; r6 = memd(r29+60); r2 = memd(r29+64) }
	{ r2 = add(r13,r2) }
	{ r7 = mpyi(r13,r6); r2 = mpyi(r2,r25); r6 = memw(r29+48) }
	{ r1 = sub(r7,r6); r6 = memw(r29+52) }
	{ r10 = addasl(r6,r2,#0x2) }

l0001B60C:
	{ r11 = r4; r9 = r28; r7 = #0x0; p0 = cmp.gt(r27,#0x0) }
	{ if (!p0) jump:nt 0001B6BC }

l0001B620:
	{ r2 = add(r7,r14); if (!cmp.gtu(r17,r2.new)) jump:nt 0001B6B0 }

l0001B62C:
	{ if (p0.new) r21 = #0x0; if (p0.new) r12 = add(r2,r8) }
	{ if (!p0.new) jump:nt 0001B6B0; p0 = cmp.gt(r26,#0x0); if (p0.new) r2 = add(r9,#0x0) }

l0001B640:
	{ loop1(0001B648,r26); r18 = mpyi(r12,r24) }
	{ r12 = add(r21,r1); if (!cmp.gtu(r24,r12.new)) jump:nt 0001B6A4 }

l0001B654:
	{ p0 = cmp.gt(r12,#0xFFFFFFFF); if (p0.new) r6 = add(r23,#0xFFFFFFFF) }
	{ p0 = cmp.gt(r15,#0x0); if (!p0.new) jump:nt 0001B6A4; r12 = add(r12,r18) }

l0001B664:
	{ p0 = cmp.gtu(r23,#0x1); r22 = memw(r2) }
	{ loop0(0001B688,r6); r12 = mpyi(r12,r23) }
	{ r19 = addasl(r16,r12,#0x2); r12 = addasl(r2,r25,#0x2) }
	{ r20 = add(r19,#0x4); r19 = memw(r19) }
	{ if (!p0) jump:nt 0001B6A0 }

l0001B688:
	{ r11 += sfmpy(r19,r22); r12 = addasl(r12,r25,#0x2); r19 = memw(r20); r22 = memw(r12) }
	{ r20 = add(r20,#0x4); nop }

l0001B6A0:
	{ r11 += sfmpy(r19,r22) }

l0001B6A4:
	{ r2 = addasl(r2,r3,#0x2); r21 = add(r21,#0x1); nop }

l0001B6B0:
	{ r9 = addasl(r9,r5,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r27)) jump:t 0001B620 }

l0001B6BC:
	{ r0 = add(r0,#0x1); r28 = add(r28,#0x4); r10 = add(r10,#0x4); memw(r10) = r11 }

l0001B6C0:
	{ r28 = add(r28,#0x4); r10 = add(r10,#0x4); memw(r10) = r11 }

l0001B6CC:
	{ if (!p0.new) jump:nt 0001B60C; p0 = cmp.eq(r0,r25) }

l0001B6D4:
	{ r13 = add(r13,#0x1); if (!cmp.eq(r13.new,r15)) jump:t 0001B5DC }

l0001B6E0:
	{ r9 = memw(r29+40) }
	{ r2 = add(r2,#0x1) }
	{ if (!p0.new) jump:nt 0001B5B4; p0 = cmp.eq(r2,r9); memw(r29+44) = r2 }

l0001B6F4:
	{ r2 = memd(r29+24); r6 = memd(r29+20) }
	{ r2 = add(r2,#0x1) }
	{ p0 = cmp.eq(r2,r6); if (!p0.new) jump:nt 0001B594; memw(r29+24) = r2 }

l0001B704:
	{ immext(#0xD5C0); r4 = add(PC,#0xD5E9); r1 = #0x93; memw(r29+12) = r25 }
	{ r2 = memd(r29+16); memw(r29) = r6 }
	{ memw(r29+8) = r15; memw(r29+4) = r9 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001B728:
	{ r17:r16 = memd(r29+112); r19:r18 = memd(r29+104) }
	{ r21:r20 = memd(r29+96); r23:r22 = memd(r29+88) }
	{ r25:r24 = memd(r29+80); r27:r26 = memd(r29+72) }
	{ dealloc_return }

;; conv2d_check_ref: 0001B73C
conv2d_check_ref proc
	{ immext(#0xD400); r4 = add(PC,#0xD433); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x9B; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 0001B778 }

l0001B764:
	{ r3 = add(PC,#0x27); r1 = #0x9C; r2 = memw(r17+28) }
	{ jump 0001B78C; memw(r29) = r2 }

l0001B778:
	{ r1 = #0x9D; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001B79C }

l0001B788:
	{ r3 = add(PC,#0x1F) }

l0001B78C:
	{ call errlog_function; r2 = r16 }

l0001B790:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001B79C:
	{ r1 = #0x9E; r2 = memw(r17+4); if (!cmp.eq(r2.new,#0x0)) jump:t 0001B7B4 }

l0001B7AC:
	{ r3 = add(PC,#0x12); jump 0001B790 }

l0001B7B4:
	{ r4 = #0x0; r3 = memw(r17+8); if (!cmp.eq(r3.new,#0x0)) jump:nt 0001B7DC }

l0001B7C4:
	{ r3 = add(PC,#0x6); jump 0001B790; r1 = #0x9F }

l0001B7D0:
	{ r2 = add(r2,#0x4); r4 = add(r4,#0x1); if (cmp.gtu(r4.new,#0x2)) jump:nt 0001B7F8 }

l0001B7D4:
	{ r4 = add(r4,#0x1); if (cmp.gtu(r4.new,#0x2)) jump:nt 0001B7FC }

l0001B7DC:
	{ r5 = memw(r2); if (!cmp.eq(r5.new,#0x0)) jump:t 0001B7D0 }

l0001B7E0:
	{ if (!cmp.eq(r5.new,#0x0)) jump:t 0001B7D4 }

l0001B7E8:
	{ r3 = add(PC,#0x2F); r1 = #0xA2; memw(r29) = r4 }
	{ jump 0001B78C }

l0001B7F8:
	{ r1 = #0xA7; r2 = memw(r3); if (!cmp.eq(r2.new,#0x0)) jump:t 0001B814 }

l0001B7FC:
	{ r2 = memw(r3); if (!cmp.eq(r2.new,#0x0)) jump:t 0001B818 }

l0001B808:
	{ r3 = add(PC,#0x1D); memw(r29) = #0x0 }
	{ jump 0001B78C }

l0001B814:
	{ immext(#0xD3C0); r4 = add(PC,#0xD3DC); r1 = #0xAA; r2 = r16 }

l0001B818:
	{ r4 = add(PC,#0x1C); r1 = #0xAA; r2 = r16 }

l0001B824:
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001B834
;;   Called from:
;;     0001B3E0 (in conv2d_f_execute_ref)
;;     0001B404 (in conv2d_f_execute_ref)
;;     0001B428 (in conv2d_f_execute_ref)
;;     0001B444 (in conv2d_f_execute_ref)
;;     0001B464 (in conv2d_f_execute_ref)
;;     0001B48C (in conv2d_f_execute_ref)
;;     0001B720 (in conv2d_f_execute_ref)
;;     0001B750 (in conv2d_check_ref)
;;     0001B824 (in conv2d_check_ref)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001B858 }

l0001B844:
	{ r0 = add(PC,#0x11); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001B858:
	{ dealloc_return }

;; errlog_function: 0001B85C
;;   Called from:
;;     0001B504 (in conv2d_f_execute_ref)
;;     0001B78C (in conv2d_check_ref)
;;     0001B84C (in logmsg_function)
errlog_function proc
	{ immext(#0xD2C0); r0 = add(PC,#0xD2F5); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; matmul_execute_ref: 0001B880
matmul_execute_ref proc
	{ immext(#0xD500); r4 = add(PC,#0xD52A); allocframe(#0x58) }
	{ r3 = memw(r0+4); r5 = memw(r0+8) }
	{ r16 = r1; r1 = #0x4F; memd(r29+80) = r17:r16; memd(r29+56) = r23:r22 }
	{ r2 = r16; r6 = memw(r3+4); r23 = memw(r5) }
	{ r3 = memw(r3); memd(r29+72) = r19:r18 }
	{ memd(r29+64) = r21:r20; memd(r29+48) = r25:r24 }
	{ r5 = memw(r6+16); memd(r29+40) = r27:r26 }
	{ r18 = memw(r6+8); r25 = memw(r6) }
	{ r24 = memw(r3+4); memw(r29+36) = r5 }
	{ r17 = memw(r3+16); r19 = memw(r6+12) }
	{ r27 = memw(r6+4); r20 = memw(r3+12) }
	{ r21 = memw(r3+8); r26 = memw(r3) }
	{ call logmsg_function; r22 = memw(r23+16); memw(r29) = r0 }
	{ immext(#0xD4C0); r4 = add(PC,#0xD4DE); r1 = #0x52; memw(r29+28) = r19 }
	{ r2 = r16; memw(r29+8) = r21 }
	{ memw(r29+24) = r18; memw(r29+20) = r27 }
	{ memw(r29+16) = r25; memw(r29+12) = r20 }
	{ memw(r29+4) = r24; memw(r29) = r26 }
	{ call logmsg_function }
	{ immext(#0xD4C0); r3 = add(PC,#0xD4D8); r1 = #0x53; p0 = cmp.eq(r24,#0x1) }
	{ if (!p0) jump:nt 0001B988; if (p0) r1 = #0x54 }

l0001B92C:
	{ immext(#0xD4C0); r3 = add(PC,#0xD4C0); p0 = cmp.eq(r27,#0x1); if (p0.new) r1 = #0x55 }
	{ if (!p0) jump:nt 0001B988 }

l0001B940:
	{ immext(#0xD480); r3 = add(PC,#0xD4BE); p0 = cmp.eq(r26,#0x1); if (p0.new) r1 = #0x56 }
	{ if (!p0) jump:nt 0001B988 }

l0001B954:
	{ immext(#0xD480); r3 = add(PC,#0xD4AA); r2 = mpyi(r26,r21); p0 = cmp.eq(r25,#0x1) }
	{ if (!p0) jump:nt 0001B988; if (p0) r1 = #0x57; if (p0) r4 = memw(r23+20) }

l0001B970:
	{ immext(#0xD480); r3 = add(PC,#0xD4A5); r2 = mpyi(r2,r24) }
	{ r2 = mpyi(r2,r19) }
	{ r2 = asl(r2,#0x2); if (!cmp.gtu(r2.new,r4)) jump:t 0001B998 }

l0001B988:
	{ call errlog_function; r2 = r16 }

l0001B98C:
	{ r2 = r16 }

l0001B990:
	{ jump 0001BA58; r0 = #0xFFFFFFFF }

l0001B998:
	{ r15 = r17; r0 = r22; r28 = memw(r29+36); memw(r23+24) = r2 }
	{ memw(r23+4) = #0xFFFFFF81; memw(r23+12) = r19 }
	{ p0 = cmp.eq(r21,#0x0); if (!p0.new) r2 = #0x0; memw(r23) = #0x1; memw(r23+8) = r21 }
	{ if (p0) jump:nt 0001BA40; immext(#0x0); if (!p0) r3 = #0x0; if (!p0) r14 = add(r20,#0xFFFFFFFF) }

l0001B9D0:
	{ r6 = mpyi(r2,r20); p0 = cmp.eq(r11,#0x0); if (p0.new) jump:nt 0001BA38; r4 = r28 }

l0001B9DC:
	{ loop1(0001B9EC,r19); r5 = mpyi(r2,r19) }
	{ r5 = addasl(r0,r5,#0x2); r6 = addasl(r15,r6,#0x2) }
	{ r12 = addasl(r4,r19,#0x2); p0 = cmp.eq(r12,#0x0); if (p0.new) jump:nt 0001BA2C; r7 = r3 }

l0001B9F8:
	{ r7 = r3; r13 = add(r6,#0x4); r9 = memw(r4); r8 = memw(r6) }
	{ loop0(0001BA10,r14); p0 = cmp.gtu(r12,#0x1); if (!p0.new) jump:nt 0001BA28 }

l0001BA10:
	{ r7 += sfmpy(r8,r9); r12 = addasl(r12,r19,#0x2); r8 = memw(r13); r9 = memw(r12) }
	{ r13 = add(r13,#0x4); nop }

l0001BA28:
	{ r7 += sfmpy(r8,r9) }

l0001BA2C:
	{ r5 = add(r5,#0x4); nop; r4 = add(r4,#0x4); memw(r5) = r7 }

l0001BA38:
	{ r2 = add(r2,#0x1); if (!cmp.eq(r2.new,r21)) jump:t 0001B9D0 }

l0001BA40:
	{ immext(#0xD3C0); r4 = add(PC,#0xD3E6); r1 = #0x68; r2 = r16 }

l0001BA44:
	{ r4 = add(PC,#0x26); r1 = #0x68; r2 = r16 }

l0001BA50:
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001BA58:
	{ r17:r16 = memd(r29+80); r19:r18 = memd(r29+72) }
	{ r21:r20 = memd(r29+64); r23:r22 = memd(r29+56) }
	{ r25:r24 = memd(r29+48); r27:r26 = memd(r29+40) }
	{ dealloc_return }

;; matmul_check_ref: 0001BA6C
matmul_check_ref proc
	{ immext(#0xD2C0); r4 = add(PC,#0xD2C8); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x6F; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ immext(#0xD2C0); r3 = add(PC,#0xD2C4); r1 = #0x70 }
	{ r2 = memw(r17+16); if (!cmp.eq(r2.new,#0x2)) jump:t 0001BAD8 }

l0001BAA0:
	{ r3 = add(PC,#0x6); r1 = #0x71 }
	{ r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x1)) jump:t 0001BAD8 }

l0001BAB4:
	{ r3 = add(PC,#0x9); r1 = #0x72 }
	{ r2 = memw(r17+4); if (cmp.eq(r2.new,#0x0)) jump:nt 0001BAD8 }

l0001BAC8:
	{ r3 = add(PC,#0x1); r1 = #0x73 }
	{ r2 = memw(r17+8); if (!cmp.eq(r2.new,#0x0)) jump:t 0001BAE8 }

l0001BAD8:
	{ call errlog_function; r2 = r16 }

l0001BADC:
	{ r2 = r16 }

l0001BAE0:
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001BAE8:
	{ immext(#0xD280); r4 = add(PC,#0xD2AA); r1 = #0x74; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001BB08
;;   Called from:
;;     0001B8DC (in matmul_execute_ref)
;;     0001B910 (in matmul_execute_ref)
;;     0001BA50 (in matmul_execute_ref)
;;     0001BA80 (in matmul_check_ref)
;;     0001BAF8 (in matmul_check_ref)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001BB2C }

l0001BB18:
	{ r0 = add(PC,#0x2); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001BB2C:
	{ dealloc_return }

;; errlog_function: 0001BB30
;;   Called from:
;;     0001B988 (in matmul_execute_ref)
;;     0001BAD8 (in matmul_check_ref)
;;     0001BB20 (in logmsg_function)
errlog_function proc
	{ immext(#0xD1C0); r0 = add(PC,#0xD1E6); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001BB54             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; maxpool_execute: 0001BB60
maxpool_execute proc
	{ r23:r22 = combine(r1,r0); memd(r29-40) = r23:r22; allocframe(#0xA0) }
	{ r2 = memw(r22+4); memd(r29+144) = r19:r18 }
	{ r18 = memb(r22+32) }
	{ memd(r29+152) = r17:r16; memd(r29+136) = r21:r20 }
	{ p0 = cmp.eq(r18,#0x0); r20 = memw(r2+8); r3 = memw(r22+8) }
	{ r17 = memw(r2); memd(r29+112) = r27:r26 }
	{ r26 = memw(r2+4) }
	{ r1 = p0; r0 = memw(r17+8); memd(r29+120) = r25:r24 }
	{ r7 = memw(r20+4); r16 = memw(r3) }
	{ r27 = memw(r17); r19 = memw(r20+8) }
	{ r25 = memw(r17+12); r24 = memw(r17+4) }
	{ r2 = memw(r26+4); memw(r29+76) = r7 }
	{ r7 = memw(r26+8) }
	{ memw(r29+36) = r0; memw(r29+68) = r2 }
	{ memw(r29+32) = r7; memw(r29+104) = r1 }
	{ if (p0) jump:nt 0001BBE8; nop }

l0001BBC4:
	{ p0 = cmp.eq(r10,#0x2); if (p0.new) jump:nt 0001BBDC }

l0001BBC8:
	{ p0 = cmp.eq(r10,#0x1); if (!p0.new) jump:nt 0001BBF4; r21 = #0x0 }

l0001BBD0:
	{ r0 = r19; r2 = memd(r29+36) }
	{ r0 += add(r2,#0xFFFFFFFF); jump 0001BBE8 }

l0001BBDC:
	{ r2 = memd(r29+36); r3 = memd(r29+32) }
	{ r2 = add(r19,r2) }
	{ r0 = sub(r2,r3) }

l0001BBE8:
	{ call fn00009760; r1 = r19 }
	{ r21 = r0 }

l0001BBF4:
	{ p0 = cmp.eq(r10,#0x2); if (p0.new) jump:nt 0001BC30; nop }

l0001BBFC:
	{ p0 = cmp.eq(r10,#0x1); if (p0.new) jump:nt 0001BC24; if (p0.new) r1 = memw(r29+76) }

l0001BC04:
	{ r2 = #0x0; r0 = memw(r29+104); memb(r29+18) = r2.new }
	{ if (!p0.new) jump:nt 0001BC48; if (p0.new) r0 = add(r24,#0x0); if (p0.new) r1 = memw(r29+76) }

l0001BC20:
	{ jump 0001BC3C }

l0001BC24:
	{ r0 = r1 }
	{ r0 += add(r24,#0xFFFFFFFF); jump 0001BC3C }

l0001BC30:
	{ r1 = memd(r29+76); r3 = memd(r29+68) }
	{ r2 = add(r1,r24) }
	{ r0 = sub(r2,r3) }

l0001BC3C:
	{ call fn00009760 }
	{ memw(r29+72) = r0 }
	{ immext(#0xD280); r4 = add(PC,#0xD2A7); r1 = #0x54; r2 = r23 }

l0001BC48:
	{ r4 = add(PC,#0x27); r1 = #0x54; r2 = r23 }

l0001BC54:
	{ r7 = memw(r16+16); r3 = memw(r17+16) }
	{ memw(r29+104) = r7; memw(r29+100) = r3 }
	{ call logmsg_function; memw(r29) = r22 }
	{ r2 = memw(r26); if (!cmp.eq(r2.new,#0x1)) jump:t 0001BC84 }

l0001BC70:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001BC88 }

l0001BC78:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001BC88 }

l0001BC80:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001BCA4 }

l0001BC84:
	{ immext(#0xD280); r3 = add(PC,#0xD281); r1 = #0x59 }

l0001BC88:
	{ r3 = add(PC,#0x1); r1 = #0x59 }

l0001BC90:
	{ call errlog_function; r2 = r23 }

l0001BC94:
	{ r2 = r23 }
	{ jump 0001BF7C; r0 = #0xFFFFFFFF }
0001BCA0 02 59 1B ED                                     .Y..            

l0001BCA4:
	{ r1 = #0x5B; r3 = memw(r16+20); r4 = memd(r29+72) }
	{ r2 = mpyi(r2,r21) }
	{ r2 = mpyi(r2,r4) }
	{ r2 = asl(r2,#0x2); if (!cmp.gtu(r2.new,r3)) jump:t 0001BCC8 }

l0001BCC0:
	{ r3 = add(PC,#0x21); jump 0001BC94 }

l0001BCC8:
	{ p0 = cmp.gt(r27,#0x0); r3 = memb(r22+32); if (!cmp.eq(r3.new,#0x0)) jump:t 0001BCE4 }

l0001BCD8:
	{ r3 = add(PC,#0x17); jump 0001BC94; r1 = #0x5C }

l0001BCE4:
	{ immext(#0xFF800000); if (p0) r1 = #0xFF800000; r2 = memd(r29+72); memw(r16+24) = r2 }
	{ memw(r29+8) = r23; memw(r29+12) = r22 }
	{ memw(r16+4) = r2; memw(r16) = r27 }
	{ memw(r16+8) = r21; memw(r16+12) = r25 }
	{ if (!p0) jump:nt 0001BF5C; if (p0) r9 = memw(r29+68); if (p0) r12 = memw(r29+32) }

l0001BD10:
	{ r0 = #0x0; r13 = r12; r2 = r21; r4 = memd(r29+72) }
	{ r8 = add(r4,#0xFFFFFFFF); r4 = sub(r9,r24); r7 = memd(r29+76); r5 = memd(r29+36) }
	{ r6 = mpyi(r5,r25); r3 = sub(r12,r5); memw(r29+28) = r0; memw(r29+60) = r24 }
	{ r8 = add(r4,mpyi(r8,r7)); r7 = sub(#0xFFFFFFFF,r5) }
	{ r2 = add(r3,mpyi(r2,r19)); r8 += lsr(r8,#0x1F); r3 = sub(#0xFFFFFFFF,r24); memw(r29+96) = r7 }
	{ r2 += lsr(r2,#0x1F); r3 = asl(r25,#0x2); r7 = #0x0; memw(r29+52) = r3 }
	{ r3 = asr(r8,#0x1); r4 = #0x0; memw(r29+92) = r3 }
	{ r0 = sub(#0x0,r3); memw(r29+64) = r7; memw(r29+48) = r3 }
	{ r2 = add(r3,sub(#0x7F,r9)); r7 = asr(r2,#0x1); memw(r29+20) = r0 }
	{ r0 = add(r7,sub(#0x7F,r12)); r2 = sub(#0x0,r7); memw(r29+16) = r2 }
	{ memw(r29+44) = r0; memw(r29+40) = r2 }

l0001BD88:
	{ r2 = memw(r29+72); memw(r29+24) = r27 }
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 0001BF38 }

l0001BD94:
	{ r2 = memw(r29+20); memb(r29+22) = r2.new }
	{ r3 = memd(r29+28); r0 = memd(r29+16) }
	{ r2 = mpyi(r3,r2); r0 = #0x0; memw(r29+84) = r0 }
	{ memw(r29+56) = r2; memw(r29+80) = r0 }

l0001BDB0:
	{ p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 0001BF0C; if (p0.new) r9 = memw(r29+80); if (p0.new) r2 = memw(r29+76) }

l0001BDBC:
	{ r14 = memw(r29+52); r12 = memw(r29+84) }
	{ r2 = mpyi(r9,r2); r3 = memw(r29+88); r8 = memw(r29+56) }
	{ r12 = max(r12,r14); r8 = add(r9,r8); r14 = memw(r29+48); r15 = memw(r29+64) }
	{ r3 = max(r3,r4); r17 = sub(r2,r14); r14 = memw(r29+68); r0 = memw(r29+60) }
	{ r20 = mpyi(r8,r21); r3 = add(r15,r3); r8 = add(r17,r14); r26 = sub(#0xFFFFFFFF,r12) }
	{ r2 = max(r17,r4); r9 = #0x0; r16 = memw(r29+40); r28 = memw(r29+44) }
	{ r12 = mpyi(r5,r3); r15 = min(r0,r8) }

l0001BE18:
	{ r3 = mpyi(r9,r19); if (!p0.new) jump:nt 0001BEFC; r8 = add(r9,r20); p0 = cmp.gt(r25,#0x0) }

l0001BE28:
	{ r14 = max(r16,r4); r11 = mpyi(r8,r25); r0 = memd(r29+96); r17 = memd(r29+100) }
	{ r3 = sub(r3,r7); r23 = add(r12,r14) }
	{ r10 = max(r28,r0); r22 = max(r3,r4); r3 = add(r3,r13); r0 = #0x0 }
	{ r18 = sub(#0xFFFFFFFF,r10); r10 = memw(r29+92) }
	{ r18 = min(r5,r3); r10 = add(r17,mpyi(r10,r23)); r8 = sub(r18,r14); r3 = memw(r29+104) }
	{ r27 = addasl(r3,r11,#0x2) }

l0001BE68:
	{ if (!p0.new) jump:nt 0001BEE4; r14 = sub(r26,r2); r3 = r1; p0 = cmp.gt(r15,r2) }

l0001BE78:
	{ loop1(0001BE84,r14); r11 = r10; r3 = r1 }
	{ r23 = addasl(r11,r25,#0x2); p0 = cmp.gt(r10,r14); if (p0.new) jump:nt 0001BED8; r24 = add(r8,#0xFFFFFFFF) }

l0001BE90:
	{ p0 = cmp.gtu(r8,#0x1); r14 = memw(r11) }
	{ if (!p0) jump:nt 0001BED4; if (!p0) r23 = add(r14,#0x0); if (p0) r17 = add(r24,#0xFFFFFFFF) }

l0001BEA4:
	{ loop0(0001BEB8,r17); r24 = addasl(r23,r25,#0x2); p0 = cmp.gtu(r24,#0x1); r23 = memw(r23) }
	{ if (!p0) jump:nt 0001BED0 }

l0001BEB8:
	{ r3 = sfmax(r14,r3); r24 = addasl(r24,r25,#0x2); r17 = r23; r23 = memw(r24) }
	{ r14 = r17; nop }

l0001BED0:
	{ r3 = sfmax(r14,r3) }

l0001BED4:
	{ r3 = sfmax(r23,r3) }

l0001BED8:
	{ r11 = addasl(r11,r6,#0x2); nop; nop }

l0001BEE4:
	{ r0 = add(r0,#0x1); r10 = add(r10,#0x4); r27 = add(r27,#0x4); memw(r27) = r3 }
	{ if (!p0.new) jump:nt 0001BE68; p0 = cmp.eq(r0,r25) }

l0001BEFC:
	{ r16 = add(r16,r19); r28 = sub(r28,r19); r9 = add(r9,#0x1); if (!cmp.eq(r9.new,r21)) jump:t 0001BE18 }

l0001BF0C:
	{ r2 = memd(r29+76); r3 = memd(r29+88) }

l0001BF10:
	{ r3 = add(r3,r2); r0 = memw(r29+84); r8 = memw(r29+80) }
	{ r3 = sub(r0,r2); r8 = add(r8,#0x1); r2 = memd(r29+72); memw(r29+88) = r3 }
	{ p0 = cmp.eq(r8,r2); memw(r29+80) = r8; memw(r29+84) = r3 }
	{ if (!p0) jump:nt 0001BDB0 }

l0001BF38:
	{ r8 = memw(r29+28); r27 = memw(r29+24) }
	{ r8 = add(r8,#0x1); r2 = memd(r29+60); r3 = memd(r29+64) }
	{ r3 = add(r3,r2); p0 = cmp.eq(r8,r27); memw(r29+28) = r8 }
	{ if (!p0) jump:nt 0001BD88; memw(r29+64) = r3 }

l0001BF5C:
	{ immext(#0xCFC0); r4 = add(PC,#0xCFE1); r2 = memw(r29+12); memb(r29) = r2.new }
	{ r2 = memw(r29+8) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001BF7C:
	{ r17:r16 = memd(r29+152); r19:r18 = memd(r29+144) }
	{ r21:r20 = memd(r29+136); r23:r22 = memd(r29+128) }
	{ r25:r24 = memd(r29+120); r27:r26 = memd(r29+112) }
	{ dealloc_return }

;; maxpool_check: 0001BF90
maxpool_check proc
	{ immext(#0xCEC0); r4 = add(PC,#0xCED0); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x8B; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x8C; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 0001BFD0 }

l0001BFBC:
	{ r3 = add(PC,#0x1) }

l0001BFC0:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001BFD0:
	{ r1 = #0x8D; r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x1)) jump:t 0001BFE4 }

l0001BFE0:
	{ r3 = memw(r17+4) }

l0001BFE4:
	{ immext(#0xCE80); r3 = add(PC,#0xCEAC); jump 0001BFC0 }
0001BFF0 83 40 03 B0 22 40 02 B0 14 C2 02 25 04 40 83 91 .@.."@.....%.@..
0001C000 FA E0 72 24 3A 43 00 00 83 50 49 6A 01 52 00 78 ..r$:C...PIj.R.x
0001C010 00 C2 9D A1 D6 FF FF 59 42 C0 91 91 02 40 82 91 .......YB....@..
0001C020 0E E0 42 24 3A 43 00 00 83 4B 49 6A A1 52 00 78 ..B$:C...KIj.R.x
0001C030 00 C0 5D 3C C6 FF FF 59 3A 43 00 00 04 4D 49 6A ..]<...Y:C...MIj
0001C040 01 53 00 78 02 C0 70 70 08 40 00 5A 00 D1 9D A1 .S.x..pp.@.Z....
0001C050 00 C0 00 78 40 1F 0C 3E                         ...x@..>        

;; logmsg_function: 0001C058
;;   Called from:
;;     0001BC5C (in maxpool_execute)
;;     0001BF74 (in maxpool_execute)
;;     0001BFA4 (in maxpool_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001C07C }

l0001C068:
	{ r0 = add(PC,#0x1D); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001C07C:
	{ dealloc_return }

;; errlog_function: 0001C080
;;   Called from:
;;     0001BC90 (in maxpool_execute)
;;     0001BFC0 (in maxpool_check)
;;     0001C070 (in logmsg_function)
errlog_function proc
	{ immext(#0xCDC0); r0 = add(PC,#0xCDC1); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001C0A4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; relu_execute: 0001C0B0
relu_execute proc
	{ immext(#0xCF40); r4 = add(PC,#0xCF66); memd(r29-16) = r17:r16; allocframe(#0x28) }
	{ r17:r16 = combine(r0,r1) }
	{ r1 = #0x36; r2 = memw(r17+4); memd(r29+24) = r19:r18 }
	{ r3 = memw(r17+8) }
	{ memd(r29+8) = r23:r22; memd(r29+16) = r21:r20 }
	{ r18 = memw(r2); r22 = memw(r3) }
	{ r0 = memw(r18); r6 = memw(r18+12) }
	{ r7 = memw(r18+4); r5 = memw(r18+8) }
	{ r2 = mpyi(r7,r0); r19 = memw(r18+16); r20 = memw(r22+16) }
	{ memw(r29) = r17 }
	{ r3 = mpyi(r2,r5); r2 = r16 }
	{ r21 = mpyi(r3,r6) }
	{ r23 = asl(r21,#0x2); call logmsg_function }
	{ r2 = memw(r22+20); if (!cmp.gtu(r23,r2.new)) jump:t 0001C11C }

l0001C108:
	{ r3 = add(PC,#0xA); r1 = #0x37; r2 = r16 }
	{ call errlog_function }
	{ jump 0001C170; r0 = #0xFFFFFFFF }

l0001C11C:
	{ p0 = cmp.eq(r21,#0x0); r2 = memw(r18); r3 = memw(r18+4) }
	{ memw(r22+4) = r3; memw(r22) = r2 }
	{ r6 = memw(r18+8) }
	{ memw(r22+8) = r6 }
	{ r18 = #-0x1; r7 = memw(r18+12) }
	{ if (p0) jump:nt 0001C158; memw(r22+12) = r7; memw(r22+24) = r23 }

l0001C13C:
	{ call fn00009600; r21 = add(r21,#0xFFFFFFFF); r1 = r18; r0 = memw(r19) }
	{ r20 = add(r20,#0x4); p0 = cmp.eq(r21,#0x0); r19 = add(r19,#0x4); memw(r20) = r0 }
	{ if (!p0) jump:nt 0001C13C }

l0001C158:
	{ immext(#0xCEC0); r4 = add(PC,#0xCED5); r1 = #0x3F; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l0001C170:
	{ r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+8) }
	{ dealloc_return }

;; relu_check: 0001C17C
relu_check proc
	{ r0 = #0x0; jump relu_check__merged }

;; relu_check__merged: 0001C180
;;   Called from:
;;     0001C17C (in relu_check)
;;     0001C348 (in reluX_check)
relu_check__merged proc
	{ immext(#0xCDC0); r4 = add(PC,#0xCDE9); p0 = cmp.eq(r2,#0x1); allocframe(#0x20) }
	{ immext(#0xCE40); r6 = add(PC,#0xCE5E); r5 = #0x2; r7 = #0x1 }
	{ r16 = r1; if (!p0) r4 = add(r6,#0x0); memd(r29+24) = r17:r16; memd(r29+16) = r19:r18 }
	{ r19 = p0; r3:r2 = combine(#0x61,r16) }
	{ r1 = mux(p0,#0x6A,r3); r17 = r0; memw(r29+8) = r19 }
	{ call logmsg_function; r18 = mux(p0,r5,r7); memw(r29) = r17 }
	{ r2 = memw(r17+16); if (cmp.eq(r2.new,r18)) jump:t 0001C1E4 }

l0001C1CC:
	{ r3 = add(PC,#0x38); r2 = r16; r0 = memd(r29+8) }
	{ p0 = r0; jump 0001C20C; if (p0.new) r1 = #0x6B; if (!p0.new) r1 = #0x62 }

l0001C1E4:
	{ r0 = memw(r29+8) }
	{ p1 = r0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001C224 }

l0001C1F8:
	{ if (p1) r1 = #0x6C; if (!p1) r1 = #0x63 }
	{ immext(#0xCD80); r3 = add(PC,#0xCD8F) }
	{ r2 = r16 }

l0001C20C:
	{ call errlog_function }
	{ jump 0001C250; r0 = #0xFFFFFFFF }
0001C218                         35 43 00 00 83 5B 49 6A         5C...[Ij
0001C220 F8 FF FF 59                                     ...Y            

l0001C224:
	{ if (p1) jump:nt 0001C23C; if (p1) r1 = #0x6D; if (!p1) r1 = #0x64 }

l0001C230:
	{ immext(#0xCDC0); r4 = add(PC,#0xCDD0); jump 0001C244 }

l0001C23C:
	{ immext(#0xCD40); r4 = add(PC,#0xCD63) }

l0001C244:
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r0 = #0x0 }

l0001C250:
	{ r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ dealloc_return }

;; reluX_execute: 0001C258
reluX_execute proc
	{ immext(#0xCD40); r4 = add(PC,#0xCD5E); memd(r29-16) = r17:r16; allocframe(#0x30) }
	{ r17:r16 = combine(r0,r1) }
	{ r1 = #0x52; r3 = memw(r17+4); memd(r29+32) = r19:r18 }
	{ r2 = r16; r5 = memw(r17+8) }
	{ memd(r29+16) = r23:r22; memd(r29+24) = r21:r20 }
	{ r19 = memw(r3); r23 = memw(r5) }
	{ r3 = memw(r3+4); memd(r29+8) = r25:r24 }
	{ r0 = memw(r19); r6 = memw(r19+4) }
	{ r5 = mpyi(r6,r0); r7 = memw(r19+8); r8 = memw(r19+12) }
	{ r5 = mpyi(r5,r7); r3 = memw(r3+16); r20 = memw(r19+16) }
	{ r21 = memw(r23+16) }
	{ r18 = memw(r3); memw(r29) = r17 }
	{ r22 = mpyi(r5,r8) }
	{ r24 = asl(r22,#0x2); call logmsg_function }
	{ r2 = memw(r23+20); if (!cmp.gtu(r24,r2.new)) jump:t 0001C2D4 }

l0001C2BC:
	{ r3 = add(PC,#0x16); r1 = #0x53; r2 = r16 }
	{ call errlog_function }
	{ jump 0001C338; r0 = #0xFFFFFFFF }

l0001C2D4:
	{ p0 = cmp.eq(r22,#0x0); r2 = memw(r19); r3 = memw(r19+4) }
	{ memw(r23+4) = r3; memw(r23) = r2 }
	{ r6 = memw(r19+8) }
	{ memw(r23+8) = r6 }
	{ r19 = #-0x1; r7 = memw(r19+12) }
	{ if (p0) jump:nt 0001C31C; memw(r23+12) = r7; memw(r23+24) = r24 }

l0001C2F8:
	{ call fn00009600; r1 = r19; r0 = memw(r20) }
	{ call fn000097B0; r1:r0 = combine(r0,r18); r22 = add(r22,#0xFFFFFFFF) }
	{ r21 = add(r21,#0x4); p0 = cmp.eq(r22,#0x0); r20 = add(r20,#0x4); memw(r21) = r0 }
	{ if (!p0) jump:nt 0001C2F8 }

l0001C31C:
	{ immext(#0xCCC0); r4 = add(PC,#0xCCC0); r1 = #0x5B; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l0001C338:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }
	{ r21:r20 = memd(r29+24); r23:r22 = memd(r29+16) }
	{ r25:r24 = memd(r29+8); dealloc_return }

;; reluX_check: 0001C348
reluX_check proc
	{ r1 = #0x1; jump relu_check__merged }

;; logmsg_function: 0001C34C
;;   Called from:
;;     0001C0F4 (in relu_execute)
;;     0001C164 (in relu_execute)
;;     0001C1B4 (in relu_check__merged)
;;     0001C244 (in relu_check__merged)
;;     0001C2A8 (in reluX_execute)
;;     0001C32C (in reluX_execute)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001C370 }

l0001C35C:
	{ r0 = add(PC,#0x35); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001C370:
	{ dealloc_return }

;; errlog_function: 0001C374
;;   Called from:
;;     0001C110 (in relu_execute)
;;     0001C20C (in relu_check__merged)
;;     0001C2C8 (in reluX_execute)
;;     0001C364 (in logmsg_function)
errlog_function proc
	{ immext(#0xCBC0); r0 = add(PC,#0xCBD9); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001C398                         00 00 00 00 00 00 00 00         ........

;; softmax_execute: 0001C3A0
softmax_execute proc
	{ allocframe(#0x48) }
	{ r2 = memw(r0+8); r3 = memw(r0+4) }
	{ memd(r29+48) = r21:r20; memd(r29+64) = r17:r16 }
	{ r2 = r1; r5 = memw(r2); r20 = memw(r3) }
	{ memd(r29+56) = r19:r18; memd(r29+40) = r23:r22 }
	{ r7 = memw(r5+20); r4 = memw(r20+24) }
	{ p0 = cmp.gtu(r4,r7); if (p0.new) r1 = #0x40; memd(r29+32) = r25:r24; memd(r29+24) = r27:r26 }
	{ if (p0) jump:nt 0001C4F8 }

l0001C3D0:
	{ r4 = memw(r20); r2 = memw(r20+4) }
	{ r2 = mpyi(r2,r4); r3 = memw(r20+8); memw(r29+8) = r2 }
	{ r16 = memw(r20+12) }
	{ memw(r29+12) = r5; memw(r29) = r4 }
	{ memw(r29+4) = r3 }
	{ r25 = mpyi(r2,r3); r26 = memw(r20+16); if (!cmp.gt(r25.new,#0x0)) jump:nt 0001C4C8 }

l0001C3F8:
	{ r21 = #0x0; r2 = memd(r29+12) }
	{ r22 = memw(r2+16) }

l0001C400:
	{ p0 = cmp.gt(r16,#0x0); r19:r18 = combine(r23,r16); r27 = memw(r26) }
	{ r0 = r27; r17 = r27 }
	{ r1 = p0; if (!p0) jump:nt 0001C4B4; memb(r29+4) = r1.new }

l0001C420:
	{ call fn00009600; r1 = r17 }

l0001C424:
	{ r1 = r17 }

l0001C428:
	{ r2 = add(r19,#0x4); r17 = r0; r18 = add(r18,#0xFFFFFFFF); if (!cmp.eq(r18.new,#0x0)) jump:t 0001C50C }

l0001C43C:
	{ p0 = r0; if (!p0.new) jump:nt 0001C4B4; if (p0.new) r19 = #0x0; if (p0.new) r24 = add(r16,#0x0) }

l0001C44C:
	{ immext(#0x0); r18 = #0x0 }

l0001C454:
	{ r0 = sfsub(r27,r17); call fn00009780; r24 = add(r24,#0xFFFFFFFF) }
	{ r2 = add(r22,r19); p0 = cmp.eq(r24,#0x0) }
	{ r18 = sfadd(r18,r0); if (p0) jump:nt 0001C484; memw(r2) = r0 }

l0001C474:
	{ r2 = add(r23,r19); r19 = add(r19,#0x4) }
	{ jump 0001C454; r27 = memw(r2) }

l0001C484:
	{ call fn00009610; immext(#0x3F800000); r0 = #0x3F800000; r1 = r18 }
	{ r2 = memw(r29+16) }
	{ p0 = r2; if (!p0.new) jump:nt 0001C4B4; if (p0.new) r2 = add(r22,#0x0) }

l0001C4A0:
	{ loop0(0001C4A4,r16) }
	{ r3 = memw(r2) }
	{ r3 = sfmpy(r0,r3) }
	{ nop; r2 = add(r2,#0x4); memw(r2) = r3 }

l0001C4B4:
	{ r23 = addasl(r23,r16,#0x2); r26 = addasl(r26,r16,#0x2) }
	{ r22 = addasl(r22,r16,#0x2); r21 = add(r21,#0x1); if (!cmp.eq(r21.new,r25)) jump:t 0001C400 }

l0001C4C8:
	{ r0 = #0x0; r3 = memd(r29+12); r2 = memd(r29) }

l0001C4CC:
	{ r3 = memd(r29+12); r2 = memd(r29) }

l0001C4D0:
	{ r7 = memd(r29+4); r6 = memd(r29+8) }
	{ memw(r3+8) = r7; memw(r3) = r2 }
	{ memw(r3+12) = r16; memw(r3+4) = r6 }
	{ r2 = memw(r20+24) }
	{ memw(r3+24) = r2 }

l0001C4E4:
	{ r17:r16 = memd(r29+64); r19:r18 = memd(r29+56) }
	{ r21:r20 = memd(r29+48); r23:r22 = memd(r29+40) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+24) }
	{ dealloc_return }

l0001C4F8:
	{ immext(#0xCBC0); r3 = add(PC,#0xCBD6); call errlog_function }
	{ jump 0001C4E4; r0 = #0xFFFFFFFF }

l0001C50C:
	{ r0 = memw(r19) }
	{ r11 = r2; jump 0001C420 }

;; softmax_check: 0001C514
softmax_check proc
	{ immext(#0xCB40); r4 = add(PC,#0xCB45); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x5A; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ immext(#0xCB40); r3 = add(PC,#0xCB42); r1 = #0x5B }
	{ r2 = memw(r17+4); if (cmp.eq(r2.new,#0x0)) jump:nt 0001C5A8 }

l0001C548:
	{ r3 = add(PC,#0x3A); r1 = #0x5C }
	{ r4 = memw(r17+8); if (cmp.eq(r4.new,#0x0)) jump:nt 0001C5A8 }

l0001C55C:
	{ r3 = add(PC,#0x33); r1 = #0x5D }
	{ r2 = memw(r2); if (cmp.eq(r2.new,#0x0)) jump:nt 0001C5A8 }

l0001C570:
	{ r3 = add(PC,#0x2C); r1 = #0x5E }
	{ r2 = memw(r4); if (cmp.eq(r2.new,#0x0)) jump:nt 0001C5A8 }

l0001C584:
	{ r3 = add(PC,#0x26); r1 = #0x5F }
	{ r2 = memw(r17+16); if (!cmp.eq(r2.new,#0x1)) jump:t 0001C5A8 }

l0001C598:
	{ r3 = add(PC,#0x12); r1 = #0x60 }
	{ r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001C5B8 }

l0001C5A8:
	{ call errlog_function; r2 = r16 }

l0001C5AC:
	{ r2 = r16 }

l0001C5B0:
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001C5B8:
	{ immext(#0xCAC0); r4 = add(PC,#0xCAFD); r1 = #0x61; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001C5D8
;;   Called from:
;;     0001C528 (in softmax_check)
;;     0001C5C8 (in softmax_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001C5FC }

l0001C5E8:
	{ r0 = add(PC,#0x16); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001C5FC:
	{ dealloc_return }

;; errlog_function: 0001C600
;;   Called from:
;;     0001C4F8 (in softmax_execute)
;;     0001C5A8 (in softmax_check)
;;     0001C5F0 (in logmsg_function)
errlog_function proc
	{ immext(#0xCA00); r0 = add(PC,#0xCA3A); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001C624             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; lrn_8_execute_ref: 0001C630
lrn_8_execute_ref proc
	{ r2 = r1; allocframe(#0x88) }
	{ r4 = memw(r0+8); r5 = memw(r0+4) }
	{ memd(r29+120) = r19:r18; memd(r29+128) = r17:r16 }
	{ r3 = memw(r4); r18 = memw(r5) }
	{ memd(r29+112) = r21:r20; memd(r29+104) = r23:r22 }
	{ r6 = memw(r3+20); r7 = memw(r18+24) }
	{ p0 = cmp.gtu(r7,r6); memd(r29+96) = r25:r24; memd(r29+88) = r27:r26 }
	{ if (!p0) jump:nt 0001C678; r15 = memw(r5+4); r13 = memw(r5+8) }

l0001C660:
	{ immext(#0xCAC0); r3 = add(PC,#0xCAE2); r1 = #0xA1; memw(r29+4) = r7 }
	{ jump 0001C770; memw(r29) = r6 }

l0001C678:
	{ r6 = memw(r5+16); r7 = memw(r5+20) }
	{ r8 = memw(r5+24); r7 = memw(r7+16) }
	{ r6 = memw(r6+16); r14 = memw(r4+8) }
	{ r28 = memw(r4+4); r4 = memw(r8+16) }
	{ r8 = memw(r15+16); r12 = memw(r13+16) }
	{ r25 = memw(r5+12); r1 = memw(r4) }
	{ r21 = r25; r27 = memw(r8); r5 = memw(r18+4) }
	{ r9 = memw(r18); r0 = memw(r18) }
	{ r17 = memw(r18+4); r19 = memw(r18+12) }
	{ r16 = memw(r18+8); r4 = memw(r12) }
	{ r7 = memw(r7); r6 = memw(r6) }
	{ memw(r3) = r9; memw(r3+4) = r5 }
	{ r5 = memw(r18+8); r8 = memw(r3+16) }
	{ memw(r3+8) = r5 }
	{ r5 = memw(r18+12) }
	{ memw(r3+12) = r5 }
	{ r5 = memw(r18+24) }
	{ memw(r3+24) = r5 }
	{ r3 = memw(r25) }
	{ memw(r29+44) = r8 }
	{ p0 = cmp.eq(r3,#0x1); if (!p0.new) jump:nt 0001C768; if (!p0.new) r1 = #0xA6; if (p0.new) memw(r29+60) = r6 }

l0001C700:
	{ p0 = cmp.gt(r0,#0x0); memw(r29+44) = r8; memw(r29+64) = r7 }
	{ immext(#0x437F0000); if (p0) r1 = #0x437F0000; memw(r29+68) = r1; memw(r29+8) = r28 }
	{ memw(r29+12) = r15; memw(r29+16) = r14 }
	{ memw(r29+20) = r13; memw(r29+24) = r0 }
	{ if (!p0) jump:nt 0001C9A0 }

l0001C730:
	{ r0 = sfsub(r4,r27) }
	{ call fn00009610; memw(r29+40) = r0 }
	{ r20 = mpyi(r16,r19); r2 = #0x0; r7 = #0x0; r22 = r0 }
	{ memw(r29+36) = r2; memw(r29+28) = r7 }

l0001C74C:
	{ p0 = cmp.gt(r9,#0x0); if (!p0.new) jump:nt 0001C988 }

l0001C750:
	{ r3 = #0x0; r2 = memd(r29+28) }
	{ r2 = mpyi(r2,r17) }
	{ memw(r29+32) = r2 }

l0001C75C:
	{ p0 = cmp.gt(r8,#0x0); if (p0.new) jump:nt 0001C77C; jump 0001C984; if (!p0.new) r3 = add(r3,#0x1) }

l0001C768:
	{ immext(#0xC9C0); r3 = add(PC,#0xC9F4) }

l0001C770:
	{ call errlog_function }
	{ jump 0001C9C4; r0 = #0xFFFFFFFF }

l0001C77C:
	{ r4 = r3; r2 = memd(r29+32); r7 = memd(r29+36) }
	{ r2 = add(r4,r2); r25:r24 = combine(r4,#0x0); r3 = add(r4,#0x1); r6 = add(r7,r4) }
	{ r2 = mpyi(r2,r16); memw(r29+72) = r3; memw(r29+52) = r6 }
	{ memw(r29+48) = r2 }

l0001C7A0:
	{ p0 = cmp.gt(r11,#0x0); if (!p0.new) jump:t 0001C978; if (!p0.new) r24 = add(r24,#0x1); if (p0.new) r1 = memw(r29+40) }

l0001C7AC:
	{ call fn00009600; immext(#0x38D1B700); r0 = #0x38D1B717 }
	{ call fn00009610; immext(#0x437F0000); r0 = #0x437F0000; r1 = r0 }
	{ r26 = #0x0; r2 = memd(r29+48); r7 = memd(r29+44) }
	{ r23 = add(r24,r2); r2 = add(r24,#0x1); memw(r29+56) = r24 }
	{ r2 = mpyi(r23,r19); r23 = add(r7,mpyi(r23,r19)); memw(r29+76) = r2; memw(r29+84) = r0 }
	{ memw(r29+80) = r2 }

l0001C7E8:
	{ r11 = r21; r2 = r25; r5 = memw(r29+72); r24 = memw(r18+16) }
	{ r3 = memw(r11+4) }
	{ r3 = #0x0; r4 = r3 }
	{ r4 += lsr(r4,#0x1F) }
	{ r2 -= asr(r4,#0x1) }
	{ r5 += asr(r4,#0x1) }
	{ r6 = r16; r3 = memw(r11+8); r5 = memw(r11+12) }
	{ r3 = add(r3,#0xFFFFFFFF); r5 = add(r5,#0xFFFFFFFF); r7 = memw(r29+52); r9 = memw(r29+56) }
	{ r3 += lsr(r3,#0x1F); r5 += lsr(r5,#0x1F); r7 = sub(r7,r4); r12 = r19 }
	{ r6 = add(r9,mpyi(r6,r7)); r8 = asr(r3,#0x1); r4 = add(r4,r25); r14 = memw(r29+76) }
	{ r5 = asr(r5,#0x1) }
	{ r3 = sub(r6,r8); r6 = sub(r9,r8); r7 = add(r8,r9); r8 = add(r14,r8) }
	{ r12 = add(r26,mpyi(r12,r3)); immext(#0x0); r3 = #0x0; r9 = sub(r26,r5) }
	{ r5 = add(#0x0,lsr(r5,#0x2)); r13 = sub(r12,r5); r12 = r5 }
	{ r12 += add(r26,#0x1); r13 = add(r24,r13) }

l0001C888:
	{ p0 = cmp.gt(r9,r2); if (p0.new) jump:nt 0001C8F4 }

l0001C88C:
	{ p0 = cmp.gt(r2,#-0x1); if (p0.new) jump:nt 0001C8F4; if (!p0) r15:r14 = combine(r6,r13) }

l0001C894:
	{ if (!p0.new) jump:nt 0001C8F4; p0 = cmp.gt(r8,r6) }

l0001C89C:
	{ if (!p0.new) jump:nt 0001C8E4; p0 = cmp.gt(r16,r15) }

l0001C8A4:
	{ if (!p0.new) jump:nt 0001C8E4; p0 = cmp.gt(r15,#0xFFFFFFFF); if (p0.new) r0 = add(r9,#0x0); if (p0.new) r28 = add(r14,#0x0) }

l0001C8B4:
	{ if (!p0.new) jump:nt 0001C8E4; p0 = cmp.gt(r12,r9) }

l0001C8BC:
	{ loop0(0001C8C0,r5) }
	{ p0 = cmp.gt(r0,#-0x1); if (p0.new) jump:nt 0001C8DC; if (p0.new) r10 = add(r27,#0x0) }

l0001C8C8:
	{ p0 = cmp.gt(r11,r0); if (p0.new) jump:nt 0001C8DC; if (p0.new) r1 = memb(r28) }

l0001C8D0:
	{ r1 = convert_w2sf(r1) }
	{ r10 += sfmpy(r22,r1) }
	{ r3 += sfmpy(r10,r10) }

l0001C8DC:
	{ r0 = add(r0,#0x1); r28 = add(r28,#0x1) }

l0001C8E4:
	{ if (!p0.new) jump:nt 0001C89C; p0 = cmp.eq(r15,r7); r14 = add(r14,r19); r15 = add(r15,#0x1) }

l0001C8F4:
	{ p0 = cmp.eq(r2,r4); if (!p0.new) jump:nt 0001C888; r13 = add(r13,r20); r2 = add(r2,#0x1) }

l0001C900:
	{ r21 = r11; r0 = memd(r29+60); r2 = memd(r29+64) }
	{ r0 += sfmpy(r2,r3); call fn00009790 }
	{ r2 = memw(r29+68) }
	{ r2 = sfmpy(r2,r0) }
	{ r0 = togglebit(r2,#0x1E); call fn00009780 }
	{ r4 = togglebit(r27,#0x1E); r3 = r27; r2 = memw(r29+80) }
	{ r2 = add(r2,r26) }
	{ r2 = memb(r12+r2) }
	{ r2 = convert_w2sf(r2) }
	{ r3 += sfmpy(r22,r2); r2 = memw(r29+84) }
	{ r4 += sfmpy(r0,r3) }
	{ r0 = sfmpy(r2,r4); call fn00009620 }
	{ r2 = convert_uw2sf(r0):chop; r3 = #0x0; r26 = add(r26,#0x1) }
	{ p1 = cmp.eq(r26,r19) }
	{ p2 = cmp.gt(r2,#0xFF); p0 = cmp.gt(r2,#0xFFFFFFFF); if (p2.new) r2 = #0xFFFFFFFF }
	{ if (!p0) r2 = add(r3,#0x0) }
	{ if (!p1) jump:nt 0001C7E8; memb(r23++#1) = r2 }

l0001C974:
	{ r24 = memw(r29+76) }

l0001C978:
	{ if (!p0.new) jump:nt 0001C7A0; p0 = cmp.eq(r24,r16); if (p0.new) r3 = memw(r29+72) }

l0001C984:
	{ p0 = cmp.eq(r3,r9); if (!p0.new) jump:nt 0001C75C }

l0001C988:
	{ r3 = memd(r29+28); r7 = memd(r29+24) }
	{ r3 = r3; r2 = memd(r29+36) }
	{ p0 = cmp.eq(r3,r7); r2 = add(r2,r17); memw(r29+28) = r3 }
	{ if (!p0) jump:nt 0001C74C; memw(r29+36) = r2 }

l0001C9A0:
	{ r1 = memd(r29+12); r16 = memd(r29+20) }
	{ call lrn_8_execute_ref.extracted_region; r2 = add(r1,#0x10); r18 = add(r16,#0x10); r0 = memw(r29+8) }
	{ call lrn_8_execute_ref.extracted_region; r1 = r16; r2 = r18; r0 = memd(r29+16) }
	{ r0 = #0x0 }

l0001C9C4:
	{ r17:r16 = memd(r29+128); r19:r18 = memd(r29+120) }
	{ r21:r20 = memd(r29+112); r23:r22 = memd(r29+104) }
	{ r25:r24 = memd(r29+96); r27:r26 = memd(r29+88) }
	{ dealloc_return }

;; lrn_check: 0001C9D8
lrn_check proc
	{ immext(#0xC700); r4 = add(PC,#0xC71D); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0xCB; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0xCC; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x7)) jump:t 0001CA18 }

l0001CA04:
	{ r3 = add(PC,#0xC) }
	{ call errlog_function; r2 = r16 }

l0001CA0C:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001CA18:
	{ r1 = #0xCD; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 0001CA30 }

l0001CA28:
	{ r3 = add(PC,#0x3B); jump 0001CA0C }

l0001CA30:
	{ immext(#0xC700); r4 = add(PC,#0xC700); r1 = #0xCE; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001CA50
;;   Called from:
;;     0001C9EC (in lrn_check)
;;     0001CA40 (in lrn_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001CA74 }

l0001CA60:
	{ r0 = add(PC,#0x0); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001CA74:
	{ dealloc_return }

;; errlog_function: 0001CA78
;;   Called from:
;;     0001C770 (in lrn_8_execute_ref)
;;     0001CA08 (in lrn_check)
;;     0001CA68 (in logmsg_function)
errlog_function proc
	{ immext(#0xC640); r0 = add(PC,#0xC664); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; lrn_8_execute_ref.extracted_region: 0001CA9C
;;   Called from:
;;     0001C9A4 (in lrn_8_execute_ref)
;;     0001C9B4 (in lrn_8_execute_ref)
lrn_8_execute_ref.extracted_region proc
	{ allocframe(#0x0) }
	{ r3 = memw(r1); r4 = memw(r1+4) }
	{ memw(r0+4) = r4; memw(r0) = r3 }
	{ r6 = memw(r1+8) }
	{ memw(r0+8) = r6 }
	{ r7 = memw(r1+12) }
	{ memw(r0+12) = r7 }
	{ r3 = memw(r1+24) }
	{ r4 = memw(r0+20); if (cmp.gtu(r3,r4.new)) jump:t 0001CAD4 }

l0001CAC8:
	{ r3 = memw(r1+24); r1 = memw(r2) }
	{ call fn00009560; r2 = r3 }

l0001CAD4:
	{ dealloc_return }
0001CAD8                         00 00 00 00 00 00 00 00         ........

;; lrn_f_execute: 0001CAE0
lrn_f_execute proc
	{ allocframe(#0xE0) }
	{ r2 = memw(r0+8); r3 = memw(r0+4) }
	{ r16 = r1; memd(r29+192) = r23:r22; memd(r29+216) = r17:r16 }
	{ r23 = memw(r3); r2 = memw(r2) }
	{ memd(r29+208) = r19:r18; memd(r29+200) = r21:r20 }
	{ r4 = memw(r2+20); r5 = memw(r23+24) }
	{ p0 = cmp.gtu(r5,r4); if (p0.new) r2 = add(r16,#0x0); memd(r29+184) = r25:r24; memd(r29+176) = r27:r26 }
	{ if (!p0) jump:nt 0001CB2C; memw(r29+52) = r23; if (p0) memw(r29) = r4 }

l0001CB18:
	{ immext(#0xC680); r3 = add(PC,#0xC6BD); r1 = #0xA3; memw(r29+4) = r5 }
	{ jump 0001CC20 }

l0001CB2C:
	{ r4 = memw(r3+8); r5 = memw(r3+12) }
	{ r6 = memw(r3+16); r12 = memw(r3+4) }
	{ r5 = memw(r5+16); r4 = memw(r4+16) }
	{ r3 = memw(r6+16); r25 = memw(r23+4) }
	{ r5 = memw(r5); r6 = memw(r4) }
	{ r7 = memw(r23+4); r8 = memw(r23) }
	{ r24 = memw(r23+8); r22 = memw(r23+12) }
	{ r9 = memw(r23); r3 = memw(r3) }
	{ memw(r2) = r8; memw(r2+4) = r7 }
	{ r4 = memw(r23+8) }
	{ memw(r2+8) = r4 }
	{ r1 = memw(r23+12) }
	{ memw(r2+12) = r1 }
	{ r4 = memw(r23+24) }
	{ r7 = memw(r2+16); memw(r2+24) = r4 }
	{ r2 = memw(r12); memw(r29+112) = r12 }
	{ memw(r29+156) = r25; memw(r29+108) = r5 }
	{ if (p0.new) r0 = #0x0; if (p0.new) r2 = add(r9,#0x0); p0 = cmp.eq(r2,#0x1); memw(r29+104) = r6 }
	{ if (!p0) jump:nt 0001CC10; memw(r29+44) = r7; if (p0) memw(r29+32) = r2 }

l0001CBA8:
	{ p0 = cmp.gt(r9,#0x0) }
	{ if (!p0) jump:nt 0001CF94 }

l0001CBB0:
	{ r1:r0 = convert_sf2df(r6); r3 = togglebit(r3,#0x1E) }
	{ r4 = mpyi(r24,r22); memw(r29+100) = r3 }
	{ memw(r29+152) = r4 }
	{ r1:r0 = convert_sf2df(r5); r4 = #0x0; memd(r29+88) = r1:r0 }
	{ r4 = #0x0; memw(r29+40) = r4 }
	{ memw(r29+116) = r4 }
	{ r1:r0 = convert_sf2df(r3); memd(r29+80) = r1:r0 }
	{ memd(r29+72) = r1:r0 }

l0001CBE0:
	{ if (!p0.new) jump:nt 0001CF7C; r9 = #0x0; p0 = cmp.gt(r25,#0x0); if (p0.new) r3 = memw(r29+116) }

l0001CBF0:
	{ r3 = mpyi(r3,r25) }
	{ memw(r29+36) = r3 }

l0001CBF8:
	{ if (p0.new) jump:nt 0001CC2C; p0 = cmp.gt(r24,#0x0); if (p0.new) r3 = add(r9,#0x1); memw(r29+60) = r9 }

l0001CC08:
	{ jump 0001CF74; r9 = add(r9,#0x1) }

l0001CC10:
	{ immext(#0xC5C0); r3 = add(PC,#0xC5DF); r1 = #0xA8; r2 = r16 }

l0001CC20:
	{ call errlog_function }
	{ jump 0001CF94; r0 = #0xFFFFFFFF }

l0001CC2C:
	{ r19 = #0x0; r2 = memd(r29+36); r6 = memd(r29+40) }
	{ r2 = add(r9,r2); r5 = add(r6,r9); memw(r29+120) = r3 }
	{ r2 = mpyi(r2,r24); memw(r29+56) = r5 }
	{ memw(r29+48) = r2 }

l0001CC4C:
	{ p0 = cmp.gt(r14,#0x0); if (p0.new) jump:nt 0001CC5C; memw(r29+128) = r19 }

l0001CC54:
	{ jump 0001CF64; r19 = add(r19,#0x1) }

l0001CC5C:
	{ r3 = add(r19,#0x1); r2 = memw(r29+48); memb(r29+16) = r3.new }
	{ r3 = #0x0 }
	{ r2 = mpyi(r2,r22); memb(r29+31) = r2.new }

l0001CC7C:
	{ r1 = #0x69; r27 = r23; r2 = memw(r12+12); memw(r29+136) = r3 }
	{ immext(#0xC580); r4 = add(PC,#0xC58F); r6 = add(r2,#0xFFFFFFFF); r5 = memw(r12+4) }
	{ r6 += lsr(r6,#0x1F); r7 = memw(r12+8); r3 = memw(r23+16) }
	{ r8 = add(r7,#0xFFFFFFFF); memw(r29+8) = r2; memw(r29) = r5 }
	{ r8 += lsr(r8,#0x1F); r17 = r5; memw(r29+4) = r7 }
	{ r17 += lsr(r17,#0x1F); r18 = asr(r8,#0x1); memw(r29+132) = r20; memw(r29+140) = r3 }
	{ r20 = asr(r6,#0x1); r21 = asr(r17,#0x1); memw(r29+16) = r18 }
	{ r3:r2 = combine(#0x1,r16); r23 = r9; memw(r29+20) = r20; memw(r29+12) = r21 }
	{ call logmsg_function }
	{ r7:r6 = combine(r19,r20); r5 = r23; r4 = r22; r2 = memd(r29+120) }
	{ r26 = r5 }
	{ r26 -= asr(r17,#0x1) }
	{ r2 += asr(r17,#0x1) }
	{ immext(#0x0); r19 = #0x0; r27 = memw(r29+136) }
	{ r21 = add(r27,#0x1); r26 = r5; r2 = memd(r29+108); r17 = memd(r29+104) }
	{ jump 0001CE98; r20 = memw(r29+132) }
0001CD24             02 40 78 70 C3 41 9D 91 5B C4 9D 91     .@xp.A..[...
0001CD30 03 43 35 F3 17 DB 34 F3 13 42 03 E3 03 47 32 F3 .C5...4..B...G2.
0001CD40 25 D2 BD A1 00 40 00 00 13 40 00 78 03 45 15 F3 %....@...@.x.E..
0001CD50 02 C2 32 F3 1B 44 02 E3 15 47 12 F3 63 44 9D 91 ..2..D...G..cD..
0001CD60 28 C3 9D A1 22 C0 1B B0 26 40 1B E2 07 44 34 F3 (..."...&@...D4.
0001CD70 12 29 02 DD 43 43 07 C4 0A 41 14 DE 02 52 02 F3 .)..CC...A...R..
0001CD80 2A C6 9D A1 24 42 9D A1 2B D4 9D A1 6A 58 20 5C *...$B..+...jX \
0001CD90 00 5A 59 F2 14 60 03 74 48 E3 9D 46 0C 48 00 5C .ZY..`.tH..F.H.\
0001CDA0 E0 7F 7A 75 A2 44 9D 43 48 E3 9D 42 5A 40 00 58 ..zu.D.CH..BZ@.X
0001CDB0 29 C3 9D A1 19 C0 62 70 83 44 9D 91 54 C2 C2 20 ).....bp.D..T.. 
0001CDC0 46 58 20 5C 11 40 78 70 00 59 58 F2 1B E0 14 74 FX \.@xp.YX....t
0001CDD0 0C 48 00 5C 11 40 78 70 E0 7F 79 75 18 E0 17 74 .H.\.@xp..yu...t
0001CDE0 36 40 00 58 11 C0 78 70 42 45 9D 91 72 C5 9D 91 6@.X..xpBE..r...
0001CDF0 2E CF C2 14 24 48 20 5C E0 FF 78 75 20 48 20 5C ....$H \..xu H \
0001CE00 22 60 30 73 00 D8 56 F2 10 43 00 00 84 5C 49 6A "`0s..V..C...\Ij
0001CE10 05 C0 9B 91 00 40 85 84 00 5A 9D A1 02 D8 9D A1 .....@...Z......
0001CE20 93 45 05 EF A1 4E 00 78 02 40 DD A1 01 D9 9D A1 .E...N.x.@......
0001CE30 06 C0 93 84 F8 40 00 5A 03 C6 DD A1 9B 40 1B B0 .....@.Z.....@..
0001CE40 38 40 18 B0 F2 7F F2 BF DC E0 72 24 54 54 16 C4 8@........r$TT..
0001CE50 00 55 19 F2 39 40 19 B0 18 C0 71 70 B2 E0 FF 5C .U..9@....qp...\
0001CE60 02 45 9D 91 C7 C4 9D 91 23 45 9D 91 F9 C4 9D 91 .E......#E......
0001CE70 43 43 07 C4 00 42 1A F2 3A 40 1A B0 51 C3 9D 43 CC...B..:@..Q..C
0001CE80 86 60 FF 5C FA 41 9D 41 35 C2 9D 41 B2 1D DF 3C .`.\.A.A5..A...<
0001CE90 34 44 9D 91 5B C4 9D 91                         4D..[...        

l0001CE98:
	{ r17 += sfmpy(r2,r19); r7:r6 = convert_sf2df(r19); r5:r4 = memd(r29+80); r1:r0 = memd(r29+88) }
	{ r3:r2 = combine(#0x1,r16); memd(r29+16) = r7:r6 }
	{ immext(#0xC380); r4 = add(PC,#0xC3B5); r19:r18 = convert_sf2df(r17); memd(r29+8) = r5:r4 }
	{ r1 = #0x7C; memd(r29) = r1:r0; memd(r29+24) = r19:r18 }
	{ call logmsg_function }
	{ call fn00009790; r0 = r17 }
	{ r2 = memw(r29+100) }
	{ r0 = sfmpy(r0,r2); call fn00009780 }
	{ immext(#0xC380); r4 = add(PC,#0xC396); r17 = r0; r1 = #0x7F }
	{ r7:r6 = convert_sf2df(r17); r3:r2 = combine(#0x1,r16); memd(r29) = r19:r18 }
	{ r7:r6 = memd(r29+72); memd(r29+16) = r7:r6 }
	{ memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ immext(#0xC2C0); r4 = add(PC,#0xC2FB); r1 = #0xBD; r2 = memw(r29+124) }
	{ r3 = memw(r29+140); r5 = memw(r29+116) }
	{ r2 = add(r2,r27); r19 = memw(r29+128) }
	{ r2 = addasl(r3,r2,#0x2) }
	{ r3:r2 = combine(#0x1,r16); r0 = memw(r2); memw(r29+12) = r27 }
	{ memw(r29+8) = r19; memw(r29) = r5 }
	{ r17 = sfmpy(r17,r0); memw(r29+4) = r26 }
	{ r7:r6 = convert_sf2df(r17) }
	{ call logmsg_function; memd(r29+16) = r7:r6 }
	{ r20 = add(r20,#0x4); r9 = r26; r3 = r21; memw(r20) = r17 }
	{ p0 = cmp.eq(r13,r14); if (!p0.new) jump:nt 0001CC7C; r12 = memw(r29+112) }

l0001CF60:
	{ r19 = memd(r29+64); r7 = memd(r29+44) }

l0001CF64:
	{ if (!p0.new) jump:nt 0001CC4C; p0 = cmp.eq(r19,r24); if (p0.new) r9 = memw(r29+120); if (p0.new) r2 = memw(r29+32) }

l0001CF74:
	{ if (!p0.new) jump:nt 0001CBF8; p0 = cmp.eq(r9,r25) }

l0001CF7C:
	{ r4 = memd(r29+116); r3 = memd(r29+40) }
	{ r4 = add(r4,#0x1); r3 = add(r3,r25) }
	{ p0 = cmp.eq(r4,r2); if (!p0.new) jump:nt 0001CBE0; memw(r29+116) = r4; memw(r29+40) = r3 }

l0001CF90:
	{ r0 = #0x0 }

l0001CF94:
	{ r17:r16 = memd(r29+216); r19:r18 = memd(r29+208) }
	{ r21:r20 = memd(r29+200); r23:r22 = memd(r29+192) }
	{ r25:r24 = memd(r29+184); r27:r26 = memd(r29+176) }
	{ dealloc_return }

;; lrn_check: 0001CFA8
lrn_check proc
	{ immext(#0xC1C0); r4 = add(PC,#0xC1DF); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0xCB; r16 = r1; r17 = r0 }
	{ call logmsg_function; r3:r2 = combine(#0x2,r16); memw(r29) = r17 }
	{ r1 = #0xCC; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x5)) jump:t 0001CFEC }

l0001CFD8:
	{ r3 = add(PC,#0x8) }
	{ call errlog_function; r2 = r16 }

l0001CFE0:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001CFEC:
	{ r1 = #0xCE; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001D008 }

l0001CFFC:
	{ r3 = add(PC,#0x37); jump 0001CFE0; r1 = #0xCD }

l0001D008:
	{ immext(#0xC180); r4 = add(PC,#0xC1B8); r3:r2 = combine(#0x2,r16); memw(r29) = r17 }
	{ call logmsg_function }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001D024
;;   Called from:
;;     0001CCE0 (in lrn_f_execute)
;;     0001CEC4 (in lrn_f_execute)
;;     0001CF00 (in lrn_f_execute)
;;     0001CF44 (in lrn_f_execute)
;;     0001CFBC (in lrn_check)
;;     0001D018 (in lrn_check)
logmsg_function proc
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001D044 }

l0001D034:
	{ r0 = add(PC,#0x3C); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l0001D044:
	{ dealloc_return }

;; errlog_function: 0001D048
;;   Called from:
;;     0001CC20 (in lrn_f_execute)
;;     0001CFDC (in lrn_check)
errlog_function proc
	{ immext(#0xC100); r0 = add(PC,#0xC124); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001D06C                                     00 00 00 00             ....

;; nn_variable_read: 0001D070
nn_variable_read proc
	{ allocframe(#0x0) }
	{ r6 = memw(r1+28) }
	{ if (p0.new) jump:nt 0001D0A0; p0 = cmp.eq(r6,#0x3E); if (p0.new) r7 = memw(r29+16); if (p0.new) r6 = memw(r1+8) }

l0001D088:
	{ immext(#0xC200); r3 = add(PC,#0xC217); r1 = #0x7D }
	{ call errlog_function; r2 = r0 }

l0001D098:
	{ r2 = r0 }
	{ r0 = #-0x1; dealloc_return }

l0001D0A0:
	{ r2 = memw(r14+r2<<#2) }
	{ r6 = memw(r2+24); if (!cmp.gtu(r6.new,r7)) jump:t 0001D0BC }

l0001D0B0:
	{ r3 = add(PC,#0x2); jump 0001D098; r1 = #0x7E }

l0001D0BC:
	{ r6 = memw(r2) }
	{ memw(r3) = r6 }
	{ r6 = memw(r2+4); memb(r4) = r6.new }
	{ r3 = memd(r29+20); memw(r5) = r4 }
	{ r1 = memw(r2+12); memb(r7) = r1.new }
	{ memb(r3) = r4.new }
	{ call fn00009560; r2 = memw(r2+24) }
	{ r0 = #0x0; dealloc_return }

;; errlog_function: 0001D0F4
;;   Called from:
;;     0001D094 (in nn_variable_read)
;;     0001D13C (in nn_variable_write)
;;     0001D26C (in assign_execute)
;;     0001D2C8 (in assign_check)
;;     0001D390 (in variable_check)
;;     0001D418 (in variable_ctor)
;;     0001D490 (in variable_ctor)
errlog_function proc
	{ immext(#0xC180); r0 = add(PC,#0xC18D); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; nn_variable_write: 0001D118
nn_variable_write proc
	{ r6 = r2; allocframe(#0x0) }
	{ r7 = memw(r1+28) }
	{ if (p0.new) jump:nt 0001D148; p0 = cmp.eq(r7,#0x3E); if (p0.new) r2 = memw(r29+16); if (p0.new) r7 = memw(r1+8) }

l0001D130:
	{ immext(#0xC140); r3 = add(PC,#0xC16F); r1 = #0x94 }
	{ call errlog_function; r2 = r0 }

l0001D140:
	{ r2 = r0 }
	{ r0 = #-0x1; dealloc_return }

l0001D148:
	{ r6 = memw(r30+r6<<#2) }
	{ r7 = memw(r6+20); if (!cmp.gtu(r2,r7.new)) jump:t 0001D164 }

l0001D158:
	{ r3 = add(PC,#0x1A); jump 0001D140; r1 = #0x95 }

l0001D164:
	{ r0 = memw(r6+16); r1 = memd(r29+12) }
	{ memw(r6+24) = r2; memw(r6) = r3 }
	{ r3 = memw(r29+8) }
	{ memw(r6+8) = r5; memw(r6+4) = r4 }
	{ call fn00009560; memw(r6+12) = r3 }
	{ r0 = #0x0; dealloc_return }

;; assign_execute: 0001D180
assign_execute proc
	{ immext(#0xC1C0); r4 = add(PC,#0xC1DC); memd(r29-16) = r17:r16; allocframe(#0x20) }
	{ r17:r16 = combine(r0,r1); r1 = #0xA3 }
	{ r2 = memw(r17+16); memd(r29+16) = r19:r18 }
	{ r2 = r16; memw(r29+4) = r2 }
	{ memd(r29+8) = r21:r20; memw(r29) = r17 }
	{ call logmsg_function }
	{ r2 = memw(r17+16); if (cmp.eq(r2.new,#0x0)) jump:nt 0001D1F8 }

l0001D1B0:
	{ r2 = memw(r17+4) }
	{ r2 = add(r2,r19); r3 = memw(r13+r19) }
	{ r2 = memw(r2-4); r4 = memw(r3) }
	{ r5 = memw(r3+4) }
	{ memw(r2+4) = r5; memw(r2) = r4 }
	{ r7 = memw(r3+8); r1 = memw(r3+12) }
	{ memw(r2+8) = r7; memw(r2+12) = r1 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:nt 0001D280 }

l0001D1E4:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }
	{ r19 = add(r19,#0x8); r2 = memw(r17+16) }
	{ r18 = add(r18,#0x2); if (cmp.gtu(r2,r18.new)) jump:t 0001D1B0 }

l0001D1F8:
	{ r19:r18 = combine(#0x0,#0x4); r0 = #0x0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0001D278 }

l0001D1FC:
	{ r0 = #0x0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x0)) jump:nt 0001D27C }

l0001D20C:
	{ r2 = memw(r17+8); r3 = memw(r17+4) }
	{ r3 = memw(r21+r18); r2 = memw(r13+r19) }
	{ r4 = memw(r3) }
	{ r5 = memw(r3+4); memb(r2+1) = r5.new }
	{ r7 = memw(r3+8); r1 = memw(r3+12) }
	{ memw(r2+8) = r7; memw(r2+12) = r1 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:nt 0001D25C }

l0001D240:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }
	{ r19 = add(r19,#0x4); r18 = add(r18,#0x8); r2 = memw(r17+20) }
	{ r20 = add(r20,#0x1); if (cmp.gtu(r2,r20.new)) jump:t 0001D20C }

l0001D25C:
	{ immext(#0xC100); r3 = add(PC,#0xC139); r1 = #0xAC; memw(r29) = r20 }

l0001D26C:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF }

l0001D278:
	{ r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }

l0001D27C:
	{ r21:r20 = memd(r29+8); dealloc_return }

l0001D280:
	{ immext(#0xC0C0); r3 = add(PC,#0xC0FE); r1 = #0xA6; memw(r29) = r18 }
	{ jump 0001D26C }

;; assign_check: 0001D294
assign_check proc
	{ r2 = r1; allocframe(#0x0) }
	{ r3 = memw(r0+16) }
	{ p0 = tstbit(r3,#0x0); if (!p0.new) jump:nt 0001D2B0; if (p0.new) r1 = #0xB5 }

l0001D2A4:
	{ immext(#0xC080); r3 = add(PC,#0xC097); jump 0001D2C8 }

l0001D2B0:
	{ r1 = #0xB6; r0 = #0x0; r4 = memw(r0+20) }
	{ r3 = lsr(r3,#0x1); if (!cmp.gtu(r4,r3.new)) jump:t 0001D2D0 }

l0001D2C4:
	{ r3 = add(PC,#0xE) }

l0001D2C8:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF }

l0001D2D0:
	{ dealloc_return }

;; variable_execute: 0001D2D4
variable_execute proc
	{ immext(#0xC040); r4 = add(PC,#0xC04C); r2 = r1; allocframe(#0x8) }
	{ call logmsg_function; r1 = #0x2F; memw(r29) = r0 }
	{ r0 = #0x0; dealloc_return }
0001D2EC                                     00 C0 00 7F             ....

;; variable_check: 0001D2F0
variable_check proc
	{ immext(#0xBFC0); r4 = add(PC,#0xBFF8); memd(r29-16) = r17:r16; allocframe(#0x20) }
	{ r1 = #0x37; r16 = r1; r17 = r0 }
	{ r2 = r16; memd(r29+16) = r19:r18; memd(r29+8) = r21:r20 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r2 = memw(r17+20) }
	{ r3 = memw(r17+16); if (cmp.gtu(r3.new,r2)) jump:t 0001D3A4 }

l0001D324:
	{ if (p0) jump:nt 0001D398 }

l0001D328:
	{ r20 = #0x0; r19:r18 = combine(#0x0,#0x0) }
	{  }
	{ nop }
	{ r2 = memw(r17+8); r3 = memw(r17+4) }
	{ r3 = memw(r21+r19); r2 = memw(r13+r19) }
	{ r4 = memw(r3) }
	{ r5 = memw(r3+4); memb(r2+1) = r5.new }
	{ r7 = memw(r3+8); r1 = memw(r3+12) }
	{ memw(r2+8) = r7; memw(r2+12) = r1 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:nt 0001D384 }

l0001D368:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }
	{ r2 = memw(r17+16) }
	{ r20 = add(r20,#0x1); if (!cmp.gtu(r2,r20.new)) jump:nt 0001D398 }

l0001D380:
	{ r19 = add(r19,#0x4); r2 = memw(r17+20) }

l0001D384:
	{ immext(#0xBF80); r3 = add(PC,#0xBF8E); r1 = #0x3B }

l0001D390:
	{ call errlog_function; r2 = r16; r18 = #-0x1 }

l0001D398:
	{ r0 = r18; r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ r21:r20 = memd(r29+8); dealloc_return }

l0001D3A4:
	{ immext(#0xBF40); r3 = add(PC,#0xBF5E); r8 = #0x38; jump 0001D390 }

;; variable_ctor: 0001D3B0
variable_ctor proc
	{ r18 = r0; memd(r29-24) = r19:r18; allocframe(#0x20) }
	{ r17 = r5; memd(r29+24) = r17:r16 }
	{ r19 = memw(r29+44) }
	{ r6 = memd(r29+40); memw(r29+4) = r19 }
	{ call node_alloc_common; memw(r29) = r6 }
	{ r16 = r0; if (cmp.eq(r16.new,#0x0)) jump:nt 0001D480 }

l0001D3D8:
	{ r0 = p0; if (p0) jump:nt 0001D3F8; memb(r29+2) = r0.new }

l0001D3E8:
	{ r2 = add(r2,#0x7F) }
	{ r2 = lsr(r2,#0x7) }
	{ r2 = mpyi(r2,r17) }
	{ r1 = asl(r2,#0x7) }

l0001D3F8:
	{ call fn00009550; r0 = #0x80 }
	{ p0 = cmp.eq(r0,#0x0); if (!p0.new) jump:nt 0001D41C; memw(r16+40) = r0 }

l0001D408:
	{ immext(#0xBEC0); r3 = add(PC,#0xBED1); r1 = #0x5F; r2 = r18 }
	{ call errlog_function }

l0001D41C:
	{ r0 = memw(r29+8) }
	{ p0 = r0; if (p0.new) jump:nt 0001D474 }

l0001D428:
	{ r3 = #0x0; r5 = add(r17,#0xFFFFFFFF); r4 = memw(r16+8); r2 = memw(r16+8) }
	{ loop0(0001D450,r5); p0 = cmp.gtu(r17,#0x1) }
	{ r3 = add(r3,#0x4); r4 = memw(r12+r3) }
	{ memw(r4+16) = r2 }
	{ if (!p0) jump:nt 0001D474; r4 = memw(r19++#8) }

l0001D450:
	{ r4 = add(r4,#0x7F); r5 = memw(r16+8) }
	{ r4 = and(r4,#0xFFFFFF80) }
	{ r2 = add(r2,r4); r3 = add(r3,#0x4); r5 = memw(r28+r3) }
	{ memw(r5+16) = r2 }
	{ nop; r4 = memw(r19++#8) }

l0001D474:
	{ r0 = r16; r17:r16 = memd(r29+24); r19:r18 = memd(r29+16) }
	{ dealloc_return }

l0001D480:
	{ immext(#0xBE40); r3 = add(PC,#0xBE4E); r1 = #0x58; r2 = r18 }
	{ call errlog_function; r16 = #0x0 }
	{ jump 0001D474 }

;; variable_dtor: 0001D49C
variable_dtor proc
	{ immext(#0xBE00); r4 = add(PC,#0xBE1C); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x6B; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r0 = memw(r17+40); if (cmp.eq(r0.new,#0x0)) jump:nt 0001D4C4 }

l0001D4C4:
	{ jump node_free_common; r1:r0 = combine(r16,r17); r17:r16 = memd(r29+8); deallocframe }

;; logmsg_function: 0001D4D0
;;   Called from:
;;     0001D1A0 (in assign_execute)
;;     0001D2E0 (in variable_execute)
;;     0001D30C (in variable_check)
;;     0001D4B0 (in variable_dtor)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001D4F4 }

l0001D4E0:
	{ r0 = add(PC,#0x25); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001D4F4:
	{ dealloc_return }

l0001D4F8:
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; reshape_execute: 0001D500
;;   Called from:
;;     0001D4FC (in logmsg_function)
reshape_execute proc
	{ r17:r16 = combine(r0,r1); memd(r29-16) = r17:r16; allocframe(#0x80) }
	{ r2 = memw(r17+4); memd(r29+88) = r25:r24 }
	{ memd(r29+96) = r23:r22 }
	{ r3 = memw(r17+8); memd(r29+104) = r21:r20 }
	{ r24 = memw(r2); r23 = memw(r2+4) }
	{ r25 = memw(r3); memd(r29+112) = r19:r18 }
	{ r7 = memw(r24); r4 = memw(r24+4) }
	{ r2 = mpyi(r4,r7); r21 = memw(r24+8); r22 = memw(r24+12) }
	{ r6 = mpyi(r2,r21); memd(r29+80) = r27:r26; memw(r29+72) = r7 }
	{ memw(r29+68) = r4 }
	{ r18 = mpyi(r6,r22); r2 = memw(r23+12); if (!cmp.gt(r2.new,#0x3)) jump:t 0001D568 }

l0001D55C:
	{ r4 = addasl(r3,r2,#0x2) }
	{ jump 0001D578; r26 = memw(r4-16) }

l0001D568:
	{ p0 = cmp.gt(r2,#0x2); if (!p0.new) jump:nt 0001D580; r27:r26 = combine(#0x1,#0x1); if (p0.new) r3 = memw(r23+16) }

l0001D574:
	{ r26 = #0x1 }

l0001D578:
	{ r3 = addasl(r3,r2,#0x2) }
	{ r27 = memw(r3-12) }

l0001D580:
	{ p0 = cmp.gt(r2,#0x1); if (!p0.new) jump:nt 0001D594; if (p0.new) r3 = memw(r23+16) }

l0001D588:
	{ r4 = addasl(r3,r2,#0x2) }
	{ jump 0001D5A0; r19 = memw(r4-8) }

l0001D594:
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 0001D5A8; r19 = #0x1; r20 = #0x1 }

l0001D59C:
	{ r19 = #0x1; r3 = memw(r23+16) }

l0001D5A0:
	{ r2 = addasl(r3,r2,#0x2) }
	{ r20 = memw(r2-4) }

l0001D5A8:
	{ immext(#0xBE80); r4 = add(PC,#0xBEA7); r1 = #0x3F; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ immext(#0xBE80); r3 = add(PC,#0xBEB0); r1 = #0x41; r2 = memw(r25+20) }
	{ r4 = memw(r24+24); if (cmp.gtu(r4.new,r2)) jump:t 0001D770 }

l0001D5D8:
	{ r3 = add(PC,#0x26); r2 = lsr(r27,#0x1F); r1 = #0x43 }
	{ r2 += lsr(r26,#0x1F) }
	{ r2 += lsr(r19,#0x1F) }
	{ r2 += lsr(r20,#0x1F) }
	{ r2 = sub(#0x0,r26); r0 = r18 }
	{ r2 = mpyi(r27,r2) }
	{ r2 = mpyi(r2,r19); r18 = p0; p0 = cmp.gt(r27,#0xFFFFFFFF); memb(r29+14) = r18.new }
	{ r18 = p0; p0 = cmp.gt(r19,#0xFFFFFFFF); memb(r29+15) = r18.new }
	{ p0 = cmp.gt(r20,#0xFFFFFFFF); memb(r29+13) = r18.new }
	{ call fn00009760; memw(r29+64) = r18 }
	{ r18 = r0; r3 = memw(r29+56); r0 = memw(r25+16) }
	{ p2 = r3; if (!p2.new) r26 = add(r18,#0x0); r3 = memw(r29+52) }
	{ memw(r25) = r26 }
	{ p0 = r3; if (!p0.new) r19 = add(r18,#0x0); r3 = memw(r29+60) }
	{ memw(r25+8) = r19 }
	{ p1 = r3; if (!p1.new) r27 = add(r18,#0x0); r3 = memw(r29+64) }
	{ memw(r25+4) = r27 }
	{ p0 = r3; if (p0.new) r18 = add(r20,#0x0) }
	{ memw(r25+12) = r18 }
	{ r2 = memw(r24+24) }
	{ memw(r25+24) = r2 }
	{ call vmemcpy_asm; r1 = memw(r24+16); r2 = memw(r24+24) }
	{ r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x3)) jump:t 0001D71C }

l0001D6A4:
	{ r2 = memw(r17+8); r3 = memw(r17+4) }
	{ immext(#0xBDC0); r3 = add(PC,#0xBDEE); r4 = memw(r3+8); r2 = memw(r2+4) }
	{ r7 = memw(r4); r5 = memw(r4+4) }
	{ memw(r2+4) = r5; memw(r2) = r7 }
	{ r0 = memw(r4+8); r6 = memw(r4+12) }
	{ memw(r2+8) = r0; memw(r2+12) = r6 }
	{ r5 = memw(r4+24) }
	{ r7 = memw(r2+20); if (cmp.gtu(r5,r7.new)) jump:t 0001D770 }

l0001D6D4:
	{ call fn00009560; r2 = memw(r4+24); r1 = memw(r4+16) }
	{ r1 = #0x54; r2 = memw(r17+8); r3 = memw(r17+4) }
	{ immext(#0xBD80); r3 = add(PC,#0xBDB2); r4 = memw(r3+12) }
	{ r2 = memw(r2+8) }
	{ r7 = memw(r4); r5 = memw(r4+4) }
	{ memw(r2+4) = r5; memw(r2) = r7 }
	{ r0 = memw(r4+8); r6 = memw(r4+12) }
	{ memw(r2+8) = r0; memw(r2+12) = r6 }
	{ r5 = memw(r4+24) }
	{ r7 = memw(r2+20); if (cmp.gtu(r5,r7.new)) jump:t 0001D770 }

l0001D714:
	{ call fn00009560; r2 = memw(r4+24); r1 = memw(r4+16) }

l0001D71C:
	{ immext(#0xBD80); r4 = add(PC,#0xBD89); r2 = memw(r23); r3 = memw(r23+4) }
	{ r1 = #0x5D; r5 = memw(r23+8); r6 = memw(r23+12) }
	{ r7 = memd(r29+68); memw(r29+44) = r18 }
	{ memw(r29+40) = r19; memw(r29+28) = r6 }
	{ memw(r29+36) = r27; memw(r29+32) = r26 }
	{ memw(r29+24) = r5; memw(r29+20) = r3 }
	{ r3 = memd(r29+72); memw(r29+8) = r21 }
	{ r2 = r16; memw(r29+16) = r2; memw(r29+12) = r22 }
	{ memw(r29+4) = r7; memw(r29) = r3 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001D75C:
	{ r17:r16 = memd(r29+120); r19:r18 = memd(r29+112) }
	{ r21:r20 = memd(r29+104); r23:r22 = memd(r29+96) }
	{ r25:r24 = memd(r29+88); r27:r26 = memd(r29+80) }
	{ dealloc_return }

l0001D770:
	{ call errlog_function; r2 = r16 }
	{ jump 0001D75C; r0 = #0xFFFFFFFF }

;; reshape_check: 0001D780
reshape_check proc
	{ r0 = #0x0; jump reshape_check__merged }

;; reshape_check__merged: 0001D784
;;   Called from:
;;     0001D780 (in reshape_check)
;;     0001D840 (in qreshape_check)
reshape_check__merged proc
	{ immext(#0xBC80); r4 = add(PC,#0xBC99); p0 = cmp.eq(r2,#0x1); allocframe(#0x28) }
	{ r17:r16 = combine(r0,r1); if (p0) r19 = #0x6F; memd(r29+32) = r17:r16; memd(r29+24) = r19:r18 }
	{ immext(#0xBC80); r18 = add(PC,#0xBC9A); if (p0) r1 = #0x6C }
	{ if (p0) jump:nt 0001D7C4; memd(r29+16) = r21:r20; memd(r29+8) = r23:r22 }

l0001D7B0:
	{ r23:r22 = combine(#0x2,#0x1); r19 = #0x66; r21:r20 = combine(#0x64,#0x65); r1 = #0x63 }
	{ jump 0001D7DC }

l0001D7C4:
	{ immext(#0xBC00); r4 = add(PC,#0xBC06); r23:r22 = combine(#0x4,#0x3); r21:r20 = combine(#0x6D,#0x6E) }
	{ immext(#0xBC00); r18 = add(PC,#0xBC2F) }

l0001D7DC:
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = r21; r2 = memw(r17+16); if (cmp.eq(r2.new,r23)) jump:t 0001D808 }

l0001D7F4:
	{ r3 = add(PC,#0x34) }
	{ call errlog_function; r2 = r16 }

l0001D7FC:
	{ r2 = r16 }
	{ jump 0001D834; r0 = #0xFFFFFFFF }

l0001D808:
	{ r1 = r20; r2 = memw(r17+20); if (cmp.eq(r2.new,r22)) jump:t 0001D820 }

l0001D818:
	{ r3 = add(PC,#0x1F); jump 0001D7FC }

l0001D820:
	{ r2 = r16; r1 = r19; r4 = r18; memw(r29) = r17 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001D834:
	{ r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+8) }
	{ dealloc_return }

;; qreshape_check: 0001D840
qreshape_check proc
	{ r1 = #0x1; jump reshape_check__merged }

;; logmsg_function: 0001D844
;;   Called from:
;;     0001D5B4 (in reshape_execute)
;;     0001D754 (in reshape_execute)
;;     0001D7DC (in reshape_check__merged)
;;     0001D82C (in reshape_check__merged)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001D868 }

l0001D854:
	{ r0 = add(PC,#0x1D); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001D868:
	{ dealloc_return }

;; errlog_function: 0001D86C
;;   Called from:
;;     0001D770 (in reshape_execute)
;;     0001D7F8 (in reshape_check__merged)
;;     0001D85C (in logmsg_function)
errlog_function proc
	{ immext(#0xBB40); r0 = add(PC,#0xBB41); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; slice_execute_f: 0001D890
slice_execute_f proc
	{ r4 = #0x4; jump slice_impl }

;; slice_check_f: 0001D894
slice_check_f proc
	{ immext(#0xBC40); r4 = add(PC,#0xBC5F); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x85; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x86; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 0001D8C8 }

l0001D8C0:
	{ r3 = add(PC,#0xE); jump 0001D8E4 }

l0001D8C8:
	{ r1 = #0x87; r0 = #0x0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001D8EC }

l0001D8DC:
	{ r3 = add(PC,#0x3D) }
	{ call errlog_function; r2 = r16 }

l0001D8E4:
	{ r2 = r16 }

l0001D8E8:
	{ r0 = #0xFFFFFFFF }

l0001D8EC:
	{ r17:r16 = memd(r29+8); dealloc_return }

;; slice_execute_8: 0001D8F0
slice_execute_8 proc
	{ r1 = #0x1; jump slice_impl }

;; slice_check_8: 0001D8F4
slice_check_8 proc
	{ immext(#0xBBC0); r4 = add(PC,#0xBBFF); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x8D; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x8E; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 0001D928 }

l0001D920:
	{ r3 = add(PC,#0x2E); jump 0001D944 }

l0001D928:
	{ r1 = #0x8F; r0 = #0x0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001D94C }

l0001D93C:
	{ r3 = add(PC,#0x1D) }
	{ call errlog_function; r2 = r16 }

l0001D944:
	{ r2 = r16 }

l0001D948:
	{ r0 = #0xFFFFFFFF }

l0001D94C:
	{ r17:r16 = memd(r29+8); dealloc_return }

;; slice_execute_q8: 0001D950
slice_execute_q8 proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x8) }
	{ r2 = memw(r16+4); r4 = memw(r16+8) }
	{ r3 = memw(r2+12); r2 = memw(r4+4) }
	{ r7 = memw(r3); r5 = memw(r3+4) }
	{ memw(r2+4) = r5; memw(r2) = r7 }
	{ r4 = memw(r3+8); r0 = memw(r3+12) }
	{ memw(r2+12) = r0; memw(r2+8) = r4 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:t 0001D988 }

l0001D980:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }

l0001D988:
	{ r2 = memw(r16+8); r3 = memw(r16+4) }
	{ r3 = memw(r3+16); r2 = memw(r2+8) }
	{ r4 = memw(r3); r5 = memw(r3+4) }
	{ memw(r2+4) = r5; memw(r2) = r4 }
	{ r7 = memw(r3+8); r1 = memw(r3+12) }
	{ memw(r2+8) = r7; memw(r2+12) = r1 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:t 0001D9B8 }

l0001D9B0:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }

l0001D9B8:
	{ r1:r0 = combine(r17,r16); r2 = #0x1; r17:r16 = memd(r29); deallocframe }
	{ jump slice_impl }
0001D9C8                         00 40 00 7F 00 C0 00 7F         .@......

;; slice_check_q8: 0001D9D0
slice_check_q8 proc
	{ immext(#0xBB00); r4 = add(PC,#0xBB23); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x95; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x96; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x5)) jump:t 0001DA04 }

l0001D9FC:
	{ r3 = add(PC,#0x12); jump 0001DA20 }

l0001DA04:
	{ r1 = #0x97; r0 = #0x0; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 0001DA28 }

l0001DA18:
	{ r3 = add(PC,#0x1) }
	{ call errlog_function; r2 = r16 }

l0001DA20:
	{ r2 = r16 }

l0001DA24:
	{ r0 = #0xFFFFFFFF }

l0001DA28:
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001DA2C
;;   Called from:
;;     0001D8A8 (in slice_check_f)
;;     0001D908 (in slice_check_8)
;;     0001D9E4 (in slice_check_q8)
;;     0001DAB4 (in slice_impl)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001DA50 }

l0001DA3C:
	{ r0 = add(PC,#0x20); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001DA50:
	{ dealloc_return }

;; errlog_function: 0001DA54
;;   Called from:
;;     0001D8E0 (in slice_check_f)
;;     0001D940 (in slice_check_8)
;;     0001DA1C (in slice_check_q8)
;;     0001DA44 (in logmsg_function)
;;     0001DBEC (in slice_impl)
errlog_function proc
	{ immext(#0xBA80); r0 = add(PC,#0xBA84); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; slice_impl: 0001DA78
;;   Called from:
;;     0001D890 (in slice_execute_f)
;;     0001D8F0 (in slice_execute_8)
;;     0001D9C4 (in slice_execute_q8)
slice_impl proc
	{ immext(#0xBA80); r4 = add(PC,#0xBAA9); allocframe(#0x58) }
	{ r3 = memw(r0+4); r5 = memw(r0+8) }
	{ r17:r16 = combine(r1,r2); r1 = #0x40; memd(r29+56) = r23:r22; memd(r29+80) = r17:r16 }
	{ r2 = r17; r22 = memw(r3); r23 = memw(r5) }
	{ memd(r29+72) = r19:r18; memd(r29+64) = r21:r20 }
	{ memd(r29+48) = r25:r24 }
	{ r20 = memw(r3+8); memd(r29+40) = r27:r26 }
	{ r24 = memw(r3+4); r18 = memw(r22+16) }
	{ call logmsg_function; r19 = memw(r23+16); memw(r29) = r0 }
	{ r1 = #0x54; r3 = memw(r20); r6 = memw(r20+4) }
	{ p0 = cmp.eq(r3,#0xFFFFFFFF); p1 = cmp.eq(r6,#0xFFFFFFFF); r13 = memw(r22); r2 = memw(r24) }
	{ r8 = memw(r20+8); r14 = memw(r20+12) }
	{ if (p0) r20 = sub(r13,r2); if (!p0) r20 = add(r3,#0x0); r9 = memw(r22+4); r12 = memw(r24+4) }
	{ r3 = mpyi(r20,r16); if (p1) r21 = sub(r9,r12); p2 = cmp.eq(r8,#0xFFFFFFFF); r4 = memw(r22+8) }
	{ if (!p1) r21 = add(r6,#0x0); p0 = cmp.eq(r14,#0xFFFFFFFF); r5 = memw(r24+8); r6 = memw(r24+12) }
	{ r3 = mpyi(r3,r21); if (p2) r26 = sub(r4,r5); r7 = memw(r22+12) }
	{ if (!p2) r26 = add(r8,#0x0); if (p0) r8 = sub(r7,r6); if (!p0) r8 = add(r14,#0x0) }
	{ r3 = mpyi(r3,r26) }
	{ immext(#0xBA00); r3 = add(PC,#0xBA0F); r14 = mpyi(r3,r8) }
	{ r15 = memw(r23+20); if (cmp.gtu(r14,r15.new)) jump:t 0001DBEC }

l0001DB40:
	{ r3 = add(PC,#0x9); r1 = #0x55; p0 = cmp.gt(r20,#0x0) }
	{ if (!p0) jump:nt 0001DBEC; if (p0) r1 = #0x56 }

l0001DB54:
	{ immext(#0xB9C0); r3 = add(PC,#0xB9FC); p1 = cmp.gt(r21,#0x0); if (p1.new) r1 = #0x57 }
	{ r0 = p1; if (!p1) jump:nt 0001DBEC; memb(r29+6) = r0.new }

l0001DB74:
	{ r3 = add(PC,#0x2B); p1 = cmp.gt(r26,#0x0); if (p1.new) r1 = #0x58 }
	{ r0 = p1; if (!p1) jump:nt 0001DBEC; memb(r29+8) = r0.new }

l0001DB90:
	{ r3 = add(PC,#0x1A); p1 = cmp.gt(r8,#0x0) }
	{ if (!p1) jump:nt 0001DBEC; if (p1) r1 = #0x59 }

l0001DBA0:
	{ immext(#0xB9C0); r3 = add(PC,#0xB9D1) }
	{ r15 = add(r20,r2); if (!cmp.gtu(r13,r15.new)) jump:t 0001DBEC }

l0001DBB4:
	{ r3 = add(PC,#0x10); r1 = #0x5A }
	{ r13 = add(r21,r12); if (!cmp.gtu(r9,r13.new)) jump:t 0001DBEC }

l0001DBC8:
	{ r3 = add(PC,#0xB); r1 = #0x5B }
	{ r13 = add(r26,r5); if (!cmp.gtu(r4,r13.new)) jump:t 0001DBEC }

l0001DBDC:
	{ r3 = add(PC,#0x6); r1 = #0x5C }
	{ r13 = add(r8,r6); if (cmp.gtu(r7,r13.new)) jump:t 0001DBFC }

l0001DBEC:
	{ call errlog_function; r2 = r17 }

l0001DBF0:
	{ r2 = r17 }

l0001DBF4:
	{ jump 0001DCCC; r0 = #0xFFFFFFFF }

l0001DBFC:
	{ r0 = #0x0; memw(r23+24) = r14; memw(r23+4) = r21 }
	{ memw(r23) = r20; memw(r23+8) = r26 }
	{ if (!p0) jump:nt 0001DCCC; memw(r23+12) = r8 }

l0001DC18:
	{ r2 = add(r12,mpyi(r2,r9)); r3 = mpyi(r7,r4); r12 = r7 }
	{ r13 = mpyi(r8,r26); r23 = mpyi(r7,r16); r7 = #0x0 }
	{ r4 = add(r5,mpyi(r4,r2)); r9 = mpyi(r3,r9) }
	{ r25 = mpyi(r3,r16); r17 = mpyi(r8,r16); r3 = r16 }
	{ r12 = add(r6,mpyi(r12,r4)); r2 = mpyi(r13,r16); memb(r29+7) = r2.new }
	{ r3 = add(r18,mpyi(r3,r12)) }
	{ memw(r29+12) = r1 }

l0001DC5C:
	{ r24 = r3; r27 = #0x0; r0 = memd(r29+24); memw(r29+16) = r3 }
	{ p0 = r0; memw(r29+20) = r7 }
	{ if (!p0) jump:nt 0001DCB4 }

l0001DC74:
	{ r22 = r26; r16 = r24; r18 = r19; r0 = memd(r29+32) }
	{ p0 = r0; if (!p0.new) jump:nt 0001DCA8 }

l0001DC88:
	{ call fn00009560; r1:r0 = combine(r16,r18); r2 = r17; r16 = add(r16,r23) }
	{ r18 = add(r18,r17); r22 = add(r22,#0xFFFFFFFF); if (!cmp.eq(r22.new,#0x0)) jump:t 0001DC88 }

l0001DCA4:
	{ r19 = add(r19,r2) }

l0001DCA8:
	{ r24 = add(r24,r25); r27 = add(r27,#0x1); if (!cmp.eq(r27.new,r21)) jump:t 0001DC74 }

l0001DCB4:
	{ r7 = memd(r29+20); r2 = memd(r29+12) }

l0001DCB8:
	{ r3 = memw(r29+16) }
	{ r3 = add(r3,r2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r20)) jump:t 0001DC5C }

l0001DCCC:
	{ r17:r16 = memd(r29+80); r19:r18 = memd(r29+72) }
	{ r21:r20 = memd(r29+64); r23:r22 = memd(r29+56) }
	{ r25:r24 = memd(r29+48); r27:r26 = memd(r29+40) }
	{ dealloc_return }

;; split_execute_f: 0001DCE0
split_execute_f proc
	{ r4 = #0x4; jump split_impl; r3 = memw(r0+20) }

;; split_check_f: 0001DCE8
split_check_f proc
	{ immext(#0xB8C0); r4 = add(PC,#0xB8E0); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x75; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x76; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 0001DD1C }

l0001DD14:
	{ r3 = add(PC,#0xF); jump 0001DD38 }

l0001DD1C:
	{ r1 = #0x77; r0 = #0x0; r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x0)) jump:t 0001DD40 }

l0001DD30:
	{ r3 = add(PC,#0x3E) }
	{ call errlog_function; r2 = r16 }

l0001DD38:
	{ r2 = r16 }

l0001DD3C:
	{ r0 = #0xFFFFFFFF }

l0001DD40:
	{ r17:r16 = memd(r29+8); dealloc_return }

;; qsplit_execute_8: 0001DD44
qsplit_execute_8 proc
	{ r16 = r0; memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r18 = r1; r2 = memw(r16+4); memd(r29) = r19:r18 }
	{ r19 = memw(r16+20); r4 = memw(r16+8) }
	{ r17 = add(r19,#0xFFFFFFFE); r3 = memw(r2+8) }
	{ r2 = memw(r15+r17<<#2) }
	{ r7 = memw(r3); memb(r2) = r7.new }
	{ memw(r2+4) = r4; memw(r2+8) = r5 }
	{ r7 = memw(r3+12) }
	{ memw(r2+12) = r7 }
	{ r4 = memw(r3+24) }
	{ r5 = memw(r2+20); if (cmp.gtu(r4,r5.new)) jump:t 0001DD94 }

l0001DD8C:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }

l0001DD94:
	{ r2 = memw(r16+8); r3 = memw(r16+4) }
	{ r2 = addasl(r2,r19,#0x2); r3 = memw(r3+12) }
	{ r2 = memw(r2-4) }
	{ r4 = memw(r3); r5 = memw(r3+4) }
	{ memw(r2+4) = r5; memw(r2) = r4 }
	{ r7 = memw(r3+8); r1 = memw(r3+12) }
	{ memw(r2+8) = r7; memw(r2+12) = r1 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:t 0001DDCC }

l0001DDC4:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }

l0001DDCC:
	{ r1:r0 = combine(r18,r16); r3:r2 = combine(r17,#0x1); r17:r16 = memd(r29+8); r19:r18 = memd(r29) }
	{ jump split_impl; deallocframe }

;; qsplit_check: 0001DDE0
qsplit_check proc
	{ immext(#0xB7C0); r4 = add(PC,#0xB7E8); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x7D; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x7E; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x4)) jump:t 0001DE14 }

l0001DE0C:
	{ r3 = add(PC,#0x17); jump 0001DE30 }

l0001DE14:
	{ r1 = #0x7F; r0 = #0x0; r2 = memw(r17+20); if (cmp.gtu(r2.new,#0x2)) jump:t 0001DE38 }

l0001DE28:
	{ r3 = add(PC,#0x6) }
	{ call errlog_function; r2 = r16 }

l0001DE30:
	{ r2 = r16 }

l0001DE34:
	{ r0 = #0xFFFFFFFF }

l0001DE38:
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001DE3C
;;   Called from:
;;     0001DCFC (in split_check_f)
;;     0001DDF4 (in qsplit_check)
;;     0001DEE4 (in split_impl)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001DE60 }

l0001DE4C:
	{ r0 = add(PC,#0x25); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001DE60:
	{ dealloc_return }

;; errlog_function: 0001DE64
;;   Called from:
;;     0001DD34 (in split_check_f)
;;     0001DE2C (in qsplit_check)
;;     0001DE54 (in logmsg_function)
;;     0001DFF4 (in split_impl)
errlog_function proc
	{ immext(#0xB740); r0 = add(PC,#0xB749); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; split_impl: 0001DE88
;;   Called from:
;;     0001DCE0 (in split_execute_f)
;;     0001DDD8 (in qsplit_execute_8)
split_impl proc
	{ r22 = r0; memd(r29-40) = r23:r22; allocframe(#0x40) }
	{ r4 = memw(r22+4); memd(r29+40) = r21:r20 }
	{ r17:r16 = combine(r1,r3); memd(r29+56) = r17:r16; memd(r29+48) = r19:r18 }
	{ r18 = r2; r21 = memw(r4+4); memd(r29+24) = r25:r24 }
	{ r23 = memw(r22+8); memd(r29+16) = r27:r26 }
	{ r20 = memw(r21+12); r26 = memw(r21+8) }
	{ call fn00009750; r1:r0 = combine(r16,r20); r27 = memw(r21+4); r25 = memw(r21) }
	{ r19 = r0; r2 = memw(r21+24) }
	{ call fn00009760; r1:r0 = combine(r16,r2) }
	{ immext(#0xB700); r4 = add(PC,#0xB722); r1 = #0x46; r2 = r17 }
	{ call logmsg_function; r21 = r0; r24 = memw(r21+16); memw(r29) = r22 }
	{ p0 = cmp.eq(r8,#0x1); if (p0.new) jump:nt 0001DF24; r3:r2 = combine(r23,#0x0) }

l0001DEFC:
	{  }
	{ r4 = memw(r3) }
	{ r5 = memw(r4+20); if (!cmp.gtu(r21,r5.new)) jump:t 0001DF5C }

l0001DF14:
	{ r3 = add(PC,#0x3C); r1 = #0x4D; memw(r29) = r2 }
	{ jump 0001DFF4 }

l0001DF24:
	{ r0 = #0xFFFFFFFF; r2 = memw(r22+8); r3 = memw(r22+4) }
	{ r3 = memw(r3+4); r2 = memw(r2) }
	{ r4 = memw(r3); r5 = memw(r3+4) }
	{ memw(r2+4) = r5; memw(r2) = r4 }
	{ r7 = memw(r3+8); r1 = memw(r3+12) }
	{ memw(r2+8) = r7; memw(r2+12) = r1 }
	{ r4 = memw(r3+24) }
	{ r6 = memw(r2+20); if (cmp.gtu(r4,r6.new)) jump:t 0001E000 }

l0001DF50:
	{ call fn00009560; r2 = memw(r3+24); r1 = memw(r3+16) }
	{ r0 = #0x0; jump 0001E000 }

l0001DF5C:
	{ r2 = r2; memw(r4+24) = r21 }
	{ r4 = memw(r3++#4) }
	{ memw(r4+4) = r27; memw(r4+8) = r26 }
	{ memw(r4+12) = r19; memw(r4) = r25 }
	{ r2 = p1; call fn00009770; r1:r0 = combine(r16,r20); memb(r29+2) = r2.new }
	{ r1 = #0x57; if (p0.new) r20 = #0x0 }
	{ r17 = mpyi(r19,r18); r2 = mpyi(r27,r26); r0 = #0x0; r1 = memd(r29+8) }
	{ p0 = r1; if (!p0.new) jump:nt 0001E000 }

l0001DFA4:
	{ r2 = mpyi(r2,r25); r21 = mpyi(r17,r16) }
	{ r22 = mpyi(r2,r19) }

l0001DFB0:
	{ p0 = cmp.gt(r14,#0x0); if (!p0.new) jump:nt 0001DFE0; r25 = r22; r19 = r20 }

l0001DFB4:
	{ r25 = r22; r19 = r20 }

l0001DFBC:
	{ r2 = memw(r31+r20<<#2) }
	{ r19 = add(r24,mpyi(r19,r17)); r18 = memw(r2+16) }

l0001DFC8:
	{ call fn00009560; r1:r0 = combine(r19,r18); r2 = r17; r19 = add(r19,r21) }
	{ r18 = add(r18,r17); r25 = add(r25,#0xFFFFFFFF); if (!cmp.eq(r25.new,#0x0)) jump:t 0001DFC8 }

l0001DFE0:
	{ r20 = add(r20,#0x1); if (!cmp.eq(r20.new,r16)) jump:t 0001DFB0 }

l0001DFE4:
	{ if (!cmp.eq(r20.new,r16)) jump:t 0001DFB4 }

l0001DFEC:
	{ immext(#0xB600); r3 = add(PC,#0xB631) }

l0001DFF4:
	{ call errlog_function; r2 = r17 }
	{ r0 = #0xFFFFFFFF }

l0001E000:
	{ r17:r16 = memd(r29+56); r19:r18 = memd(r29+48) }
	{ r21:r20 = memd(r29+40); r23:r22 = memd(r29+32) }
	{ r25:r24 = memd(r29+24); r27:r26 = memd(r29+16) }
	{ dealloc_return }
0001E014             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; tanh_execute: 0001E020
tanh_execute proc
	{ immext(#0xB680); r4 = add(PC,#0xB6A0); memd(r29-16) = r17:r16; allocframe(#0x28) }
	{ r17:r16 = combine(r0,r1) }
	{ r1 = #0x37; r2 = memw(r17+4); memd(r29+16) = r21:r20 }
	{ r3 = memw(r17+8) }
	{ memd(r29+8) = r23:r22; memd(r29+24) = r19:r18 }
	{ r21 = memw(r2); r22 = memw(r3) }
	{ r0 = memw(r21); r6 = memw(r21+12) }
	{ r7 = memw(r21+4); r5 = memw(r21+8) }
	{ r2 = mpyi(r7,r0); r18 = memw(r21+16); r19 = memw(r22+16) }
	{ memw(r29) = r17 }
	{ r3 = mpyi(r2,r5); r2 = r16 }
	{ r20 = mpyi(r3,r6) }
	{ r23 = asl(r20,#0x2); call logmsg_function }
	{ r2 = memw(r22+20); if (!cmp.gtu(r23,r2.new)) jump:t 0001E08C }

l0001E078:
	{ r3 = add(PC,#0x23); r1 = #0x38; r2 = r16 }
	{ call errlog_function }
	{ jump 0001E0E0; r0 = #0xFFFFFFFF }

l0001E08C:
	{ p0 = cmp.eq(r20,#0x0); r2 = memw(r21); r3 = memw(r21+4) }
	{ memw(r22+4) = r3; memw(r22) = r2 }
	{ r6 = memw(r21+8) }
	{ memw(r22+8) = r6 }
	{ r7 = memw(r21+12) }
	{ if (p0) jump:nt 0001E0C4; memw(r22+12) = r7; memw(r22+24) = r23 }

l0001E0AC:
	{ call fn000097F0; r20 = r20; r0 = memw(r18) }
	{ r19 = add(r19,#0x4); p0 = cmp.eq(r20,#0x0); r18 = add(r18,#0x4); memw(r19) = r0 }
	{ if (!p0) jump:nt 0001E0AC }

l0001E0C4:
	{ immext(#0xB600); r4 = add(PC,#0xB621); r1 = #0x40; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l0001E0E0:
	{ r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+8) }
	{ dealloc_return }

;; tanh_check: 0001E0EC
tanh_check proc
	{ immext(#0xB580); r4 = add(PC,#0xB589); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x46; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x47; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x1)) jump:t 0001E12C }

l0001E118:
	{ r3 = add(PC,#0x37) }
	{ call errlog_function; r2 = r16 }

l0001E120:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001E12C:
	{ r1 = #0x48; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001E144 }

l0001E13C:
	{ r3 = add(PC,#0x22); jump 0001E120 }

l0001E144:
	{ immext(#0xB540); r4 = add(PC,#0xB566); r1 = #0x49; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001E164
;;   Called from:
;;     0001E064 (in tanh_execute)
;;     0001E0D4 (in tanh_execute)
;;     0001E100 (in tanh_check)
;;     0001E154 (in tanh_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001E188 }

l0001E174:
	{ r0 = add(PC,#0x29); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001E188:
	{ dealloc_return }

;; errlog_function: 0001E18C
;;   Called from:
;;     0001E080 (in tanh_execute)
;;     0001E11C (in tanh_check)
;;     0001E17C (in logmsg_function)
errlog_function proc
	{ immext(#0xB4C0); r0 = add(PC,#0xB4CD); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; sigmoid_execute: 0001E1B0
sigmoid_execute proc
	{ immext(#0xB580); r4 = add(PC,#0xB5B2); memd(r29-16) = r17:r16; allocframe(#0x28) }
	{ r17:r16 = combine(r0,r1) }
	{ r1 = #0x37; r2 = memw(r17+4); memd(r29+16) = r21:r20 }
	{ r3 = memw(r17+8) }
	{ memd(r29+8) = r23:r22; memd(r29+24) = r19:r18 }
	{ r21 = memw(r2); r22 = memw(r3) }
	{ r0 = memw(r21); r6 = memw(r21+12) }
	{ r7 = memw(r21+4); r5 = memw(r21+8) }
	{ r2 = mpyi(r7,r0); r18 = memw(r21+16); r19 = memw(r22+16) }
	{ memw(r29) = r17 }
	{ r3 = mpyi(r2,r5); r2 = r16 }
	{ r20 = mpyi(r3,r6) }
	{ r23 = asl(r20,#0x2); call logmsg_function }
	{ r2 = memw(r22+20); if (!cmp.gtu(r23,r2.new)) jump:t 0001E21C }

l0001E208:
	{ r3 = add(PC,#0x38); r1 = #0x38; r2 = r16 }
	{ call errlog_function }
	{ jump 0001E290; r0 = #0xFFFFFFFF }

l0001E21C:
	{ p0 = cmp.eq(r20,#0x0); r2 = memw(r21); r3 = memw(r21+4) }
	{ memw(r22+4) = r3; memw(r22) = r2 }
	{ r6 = memw(r21+8) }
	{ memw(r22+8) = r6 }
	{ immext(#0x3F000000); if (!p0) r21 = #0x3F000000; r7 = memw(r21+12) }
	{ if (p0) jump:nt 0001E274; memw(r22+12) = r7; memw(r22+24) = r23 }

l0001E244:
	{ immext(#0x3F800000); r22 = #0x3F800000 }

l0001E24C:
	{ r2 = memw(r18) }
	{ r0 = sfmpy(r2,r21); call fn000097F0 }
	{ r2 = sfadd(r0,r22); r18 = add(r18,#0x4); r20 = r20 }
	{ p0 = cmp.eq(r20,#0x0) }
	{ r2 = sfmpy(r2,r21); if (!p0) jump:nt 0001E24C; r19 = add(r19,#0x4); memb(r19) = r2.new }

l0001E274:
	{ immext(#0xB500); r4 = add(PC,#0xB516); r1 = #0x40; r2 = r16 }

l0001E278:
	{ r4 = add(PC,#0x16); r1 = #0x40; r2 = r16 }

l0001E284:
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l0001E290:
	{ r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+8) }
	{ dealloc_return }

;; sigmoid_check: 0001E29C
sigmoid_check proc
	{ immext(#0xB440); r4 = add(PC,#0xB475); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x46; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x47; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x1)) jump:t 0001E2DC }

l0001E2C8:
	{ r3 = add(PC,#0x26) }
	{ call errlog_function; r2 = r16 }

l0001E2D0:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001E2DC:
	{ r1 = #0x48; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001E2F4 }

l0001E2EC:
	{ r3 = add(PC,#0x11); jump 0001E2D0 }

l0001E2F4:
	{ immext(#0xB440); r4 = add(PC,#0xB455); r1 = #0x49; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001E314
;;   Called from:
;;     0001E1F4 (in sigmoid_execute)
;;     0001E284 (in sigmoid_execute)
;;     0001E2B0 (in sigmoid_check)
;;     0001E304 (in sigmoid_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001E338 }

l0001E324:
	{ r0 = add(PC,#0x12); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001E338:
	{ dealloc_return }

;; errlog_function: 0001E33C
;;   Called from:
;;     0001E210 (in sigmoid_execute)
;;     0001E2CC (in sigmoid_check)
;;     0001E32C (in logmsg_function)
errlog_function proc
	{ immext(#0xB380); r0 = add(PC,#0xB3B6); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; qtanh_execute_ref: 0001E360
qtanh_execute_ref proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x40) }
	{ r2 = memw(r16+4); memd(r29+16) = r27:r26 }
	{ r7 = memw(r16+8) }
	{ immext(#0x437F0000); r18 = #0x437F0000; memd(r29+32) = r23:r22; memd(r29+48) = r19:r18 }
	{ r1 = r18; r26 = memw(r2); r4 = memw(r2+4) }
	{ r2 = memw(r2+8); r27 = memw(r7) }
	{ r4 = memw(r4+16); r0 = memw(r26+4) }
	{ r2 = memw(r2+16); r6 = memw(r26) }
	{ r23 = memw(r4); r5 = memw(r26+12) }
	{ r3 = mpyi(r0,r6); r2 = memw(r2); r6 = memw(r26+8) }
	{ memd(r29+40) = r21:r20; memd(r29+24) = r25:r24 }
	{ r2 = mpyi(r3,r6); r0 = sfsub(r2,r23); r20 = memw(r7+8); r21 = memw(r7+4) }
	{ r22 = memw(r26+16); r24 = memw(r27+16) }
	{ r25 = mpyi(r2,r5); call fn00009610 }
	{ immext(#0xB500); r4 = add(PC,#0xB53E); r1 = #0x42; r2 = r17 }
	{ call logmsg_function; r19 = r0; memw(r29) = r16 }
	{ p0 = cmp.eq(r25,#0x0); r2 = memw(r27+20); if (!cmp.gtu(r25,r2.new)) jump:t 0001E41C }

l0001E404:
	{ r3 = add(PC,#0x3); r1 = #0x43; r2 = r17 }
	{ call errlog_function }
	{ jump 0001E4E8; r0 = #0xFFFFFFFF }

l0001E41C:
	{ r2 = memw(r26); r3 = memw(r26+4) }
	{ memw(r27+4) = r3; memw(r27) = r2 }
	{ r6 = memw(r26+8) }
	{ memw(r27+8) = r6 }
	{ immext(#0x3F000000); if (!p0) r17 = #0x3F000000; r7 = memw(r26+12); memw(r29+12) = r17 }
	{ if (p0) jump:nt fn0001E49C; memw(r27+12) = r7; memw(r27+24) = r25 }

l0001E450:
	{ immext(#0x42FF0000); r26 = #0x42FF0000; immext(#0x3F800000); r27 = #0x3F800000 }

l0001E460:
	{ r0 = r23; r2 = memb(r22++#1) }
	{ r2 = convert_w2sf(r2) }
	{ r0 += sfmpy(r19,r2); call fn000097F0 }
	{ r3 = sfadd(r0,r27); r2 = r17; r25 = add(r25,#0xFFFFFFFF) }
	{ r2 += sfmpy(r3,r26); p0 = cmp.eq(r25,#0x0) }
	{ r3 = convert_sf2uw(r2) }
	{ p1 = sfcmp.gt(r2,r18); if (p1.new) r3 = #0xFFFFFFFF }
	{ if (!p0) jump:nt 0001E460; memb(r24++#1) = r3 }

l0001E498:
	{ memb(r24++#1) = r3 }

;; fn0001E49C: 0001E49C
;;   Called from:
;;     0001E444 (in qtanh_execute_ref)
;;     0001E494 (in qtanh_execute_ref)
;;     0001E498 (in qtanh_execute_hvx)
;;     0001E4A4 (in qtanh_execute_hvx)
;;     0001E4B4 (in qtanh_execute_hvx)
;;     0001E85C (in qtanh_execute_hvx)
;;     0001E864 (in qtanh_execute_hvx)
;;     0001E88C (in qtanh_execute_hvx)
fn0001E49C proc
	{ immext(#0xB480); r4 = add(PC,#0xB4B5); r2 = memw(r21+16); memw(r21+12) = #0x1 }

l0001E4A4:
	{ r2 = memw(r21+16); memw(r21+12) = #0x1 }

l0001E4A8:
	{ r1 = #0x56; memw(r21+8) = #0x1; memw(r21) = #0x1 }
	{ memw(r21+4) = #0xFFFFFF81; immext(#0xBF800000); memw(r2-1082130432) = #0x0 }

l0001E4B4:
	{ immext(#0xBF800000); memw(r2-1082130432) = #0x0 }

l0001E4BC:
	{ r2 = memw(r29+12); memw(r21+24) = #0x4 }
	{ r3 = memw(r20+16); memw(r20+8) = #0x1 }
	{ memw(r20) = #0x1; memw(r20+4) = #0x1 }
	{ memw(r20+12) = #0xFFFFFF81; immext(#0x3F800000); memw(r3+1065353216) = #0x0 }

l0001E4D8:
	{ call logmsg_function; memw(r20+24) = #0x4; memw(r29) = r16 }
	{ r0 = #0x0 }

l0001E4E8:
	{ r17:r16 = memd(r29+56); r19:r18 = memd(r29+48) }
	{ r21:r20 = memd(r29+40); r23:r22 = memd(r29+32) }
	{ r25:r24 = memd(r29+24); r27:r26 = memd(r29+16) }
	{ dealloc_return }
0001E4FC                                     00 C0 00 7F             ....

;; qtanh_check: 0001E500
qtanh_check proc
	{ immext(#0xB440); r4 = add(PC,#0xB45E); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x94; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x95; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 0001E540 }

l0001E52C:
	{ r3 = add(PC,#0xC) }
	{ call errlog_function; r2 = r16 }

l0001E534:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001E540:
	{ r1 = #0x96; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 0001E558 }

l0001E550:
	{ r3 = add(PC,#0x37); jump 0001E534 }

l0001E558:
	{ immext(#0xB400); r4 = add(PC,#0xB43B); r1 = #0x97; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }
0001E578                         00 40 00 7F 00 C0 00 7F         .@......

;; qtanh_execute_hvx: 0001E580
qtanh_execute_hvx proc
	{ immext(#0xB380); r4 = add(PC,#0xB39A); memd(r29-16) = r17:r16; allocframe(#0x50) }
	{ r17:r16 = combine(r1,r0) }
	{ r1 = #0x72; r3 = memw(r16+4); memd(r29+64) = r19:r18 }
	{ r2 = r17; r5 = memw(r16+8) }
	{ memd(r29+56) = r21:r20; memd(r29+40) = r25:r24 }
	{ r6 = memw(r3+8); r7 = memw(r3+4) }
	{ r0 = memw(r5+4); r18 = memw(r5) }
	{ r5 = memw(r5+8); r21 = memw(r3) }
	{ r3 = memw(r7+16) }
	{ r5 = memw(r6+16); memw(r29+28) = r5 }
	{ memd(r29+48) = r23:r22 }
	{ r7 = memw(r18+16); memd(r29+32) = r27:r26 }
	{ r25 = memw(r17+4) }
	{ r23 = memw(r21+16); memw(r29+20) = r0 }
	{ r20 = memw(r21+8); r22 = memw(r21+4) }
	{ r26 = memw(r21+12); r27 = memw(r21) }
	{ r19 = memw(r3); memw(r29+24) = r7 }
	{ r24 = memw(r5) }
	{ call logmsg_function; memw(r29) = r16 }
	{ r1 = #0x73; r2 = memb(r16+32); if (!cmp.eq(r2.new,#0x0)) jump:t 0001E60C }

l0001E5F4:
	{ immext(#0xB300); r3 = add(PC,#0xB33D) }

l0001E5F8:
	{ r3 = add(PC,#0x3D) }

l0001E5FC:
	{ call errlog_function; r2 = r17 }

l0001E600:
	{ r2 = r17 }
	{ jump 0001E964; r0 = #0xFFFFFFFF }

l0001E60C:
	{ r2 = mpyi(r22,r27); r1 = #0x74; r3 = memw(r18+20) }
	{ r2 = mpyi(r2,r20) }
	{ r20 = mpyi(r2,r26); if (!cmp.gtu(r20.new,r3)) jump:t 0001E630 }

l0001E628:
	{ r3 = add(PC,#0x1F); jump 0001E600 }

l0001E630:
	{ r22 = add(r20,#0xFF); r2 = #0xFF; r3 = memw(r21); memw(r29+12) = r2 }
	{ r27 = and(r22,#0xFFFFFF00); r0 = r25; r4 = memw(r21+4); memb(r18+1) = r4.new }
	{ memw(r18) = r3 }
	{ r3 = add(r22,r0); r1 = #0x0; r2 = memw(r21+8) }
	{ r2 = r27; memw(r18+8) = r2; memw(r29+16) = r0 }
	{ r7 = memw(r21+12) }
	{ call fn000095F0; r21 = and(r3,#0xFFFFFF00); memw(r18+12) = r7; memw(r18+24) = r20 }
	{ call fn000095F0; r1:r0 = combine(#0x0,r21); r2 = r27 }
	{ call fn00009560; r2 = r20; r1:r0 = combine(r23,r21) }
	{ r1 = sfsub(r24,r19); immext(#0xC0800000); r2 = #0xC0800000 }
	{ p0 = sfcmp.ge(r19,r2); if (!p0.new) jump:nt 0001E76C }

l0001E6A0:
	{ immext(#0x40800000); r18 = #0x40800000 }
	{ p0 = sfcmp.ge(r18,r24); if (!p0.new) jump:nt 0001E76C }

l0001E6B0:
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r2,r1) }
	{ r2 = sfadd(r19,r18); r23 = r0 }
	{ call fn00009610; r1:r0 = combine(r23,r2) }
	{ r18 = convert_uw2sf(r0):chop; immext(#0x41700000); r2 = #0x41700000 }
	{ call fn000097C0; r0 = r2 }
	{ immext(#0x41FF0000); r2 = #0x41FF0000; immext(#0xBF800000); r3 = #0xBF800000 }
	{ r2 = sfmpy(r23,r2); r3 = sfadd(r0,r3); p0 = cmp.gt(r20,#0x0); r25 = memw(r29+20) }
	{ r2 = sfmpy(r2,r0); r3 = convert_uw2sf(r3):chop }
	{ r4 = convert_uw2sf(r2):chop; if (!p0) jump:nt 0001E828; if (p0) r2 = memw(r29+12) }

l0001E718:
	{ r5 = convert_uw2sf(r0):chop }
	{ r5 += lsr(r5,#0x1F); p0 = cmp.gt(r4,r3); if (!p0.new) r3 = add(r4,#0x0) }
	{ r6 = mpyi(r2,r26); r2 = r21; r3 = sxth(r3) }
	{ loop0(0001E738,r6); r4 = asr(r5,#0x1) }
	{ r5 = #0xFF; r7 = r4; r6 = memb(r2) }
	{ r6 = add(r6,r18) }
	{ r7 += mpyi(r6,r3) }
	{ r6 = asr(r7,#0xF) }
	{ if (p0.new) jump:nt 0001E760; p0 = cmp.gt(r6,#0xFF); if (!p0.new) r5 = add(r6,#0x0) }

l0001E758:
	{ p0 = cmp.gt(r6,#0xFFFFFFFF); if (!p0.new) r5 = #0x0 }

l0001E760:
	{ nop; memb(r2++#1) = r5 }
	{ jump 0001E828 }

l0001E76C:
	{ call fn00009600; immext(#0x38D1B700); r0 = #0x38D1B717 }
	{ call fn00009610; immext(#0x437F0000); r1 = #0x437F0000 }
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 0001E828; r3 = add(r21,r22); r25 = memw(r29+20) }

l0001E790:
	{ loop0(0001E79C,r20); r22 = and(r3,#0xFFFFFF00) }
	{ r3:r2 = combine(r22,r21) }
	{ r5 = r19; r4 = memb(r2++#1) }
	{ r4 = convert_w2sf(r4) }
	{ r5 += sfmpy(r0,r4); r3 = add(r3,#0x4) }
	{ r0 = #0x17; immext(#0x41000000); r1 = #0x41000000 }
	{ call fn00009600 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r18 = r21; r23 = r0; r2 = memd(r29+12) }
	{ r19 = mpyi(r2,r26); immext(#0x40800000); r24 = #0x40800000 }

l0001E7EC:
	{ r2 = memw(r22) }
	{ r2 = sfadd(r2,r24) }
	{ r0 = sfmpy(r23,r2); call fn00009620 }
	{ r3 = convert_uw2sf(r0):chop; r2 = #0xFF }
	{ if (p0.new) jump:nt 0001E818; p0 = cmp.gt(r3,#0xFF); if (!p0.new) r2 = add(r3,#0x0) }

l0001E810:
	{ p0 = cmp.gt(r3,#0xFFFFFFFF); if (!p0.new) r2 = #0x0 }

l0001E818:
	{ r19 = add(r19,#0xFFFFFFFF); r22 = add(r22,#0x4); memb(r18++#1) = r2 }
	{ p0 = cmp.eq(r11,#0x0); if (!p0.new) jump:nt 0001E7EC }

l0001E828:
	{ immext(#0xC8C0); r2 = add(PC,#0xC8E8); immext(#0x1010100); r3 = #0x1010101 }
	{ immext(#0x4040400); r4 = #0x4040404; immext(#0x80808080); r5 = #0x80808080 }
	{ immext(#0xF8F8F8C0); r6 = #0xF8F8F8F8; immext(#0x7070700); r7 = #0x7070707 }
	{ p0 = cmp.eq(r3,#-0x1); if (p0.new) jump:nt 0001E498 }

l0001E85C:
	{ p0 = cmp.eq(r5,#-0x1); if (p0.new) jump:nt fn0001E49C }

l0001E860:
	{ p0 = cmp.eq(r6,#-0x1); if (p0.new) jump:nt 0001E4A4 }

l0001E864:
	{ p0 = cmp.eq(r7,#-0x1); if (p0.new) jump:nt 0001E4A8; p0 = cmp.gt(r27,#0x0) }

l0001E86C:
	{ p0 = cmp.eq(r4,#-0x1); if (p0.new) jump:nt 0001E4B4 }

l0001E870:
	{ if (!p0) jump:nt 0001E900; immext(#0xFFFFFFC0); r4 = memw(r2-48) }

l0001E87C:
	{ r7 = #0x0; r2 = add(r27,#0x7F); r6 = #0x7; r4 = #0x0; r0 = r0 }
	{ r5 = lsr(r2,#0x7); p0 = cmp.eq(r7,#-0x1); if (p0.new) jump:nt 0001E4D8 }

l0001E894:
	{ loop0(0001E8A8,r5); r3 = #0x3; r2 = memw(r29+16); r4 = #0x0; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r0,r7); if (p0.new) jump:nt fn0001EA34 }

l0001E8A8:
	{ p1 = cmp.eq(r2,r0); if (p1.new) jump:nt 0001E5F4; p1 = cmp.eq(r3,r0); if (p1.new) jump:nt 0001E600; r5 = #0x11; r1 = add(r1,#0x1) }

l0001E8B4:
	{ p0 = tstbit(r3,#0x0); if (p0.new) jump:nt 0001E958 }
	{ p0 = cmp.eq(r4,r12); if (p0.new) jump:nt 0001EA4C }
	{ p1 = cmp.gtu(r7,#0x12); if (p1.new) jump:t 0001E718; p1 = cmp.gtu(r7,#0xB); if (!p1.new) jump:nt 0001ECA4 }
	{ p1 = cmp.gtu(r15,#0x12); if (p1.new) jump:t 0001E510 }
	{ p1 = cmp.eq(r14,r4); if (p1.new) jump:nt 0001EA64 }
	{ p0 = cmp.eq(r0,r7); if (p0.new) jump:nt 0001EA64; p1 = cmp.gtu(r6,#0xF); if (!p1.new) jump:nt 0001E8AC }
	{ p1 = cmp.eq(r0,r1); if (p1.new) jump:nt fn0001EA74 }
	{ p0 = cmp.gt(r0,r12); if (p0.new) jump:nt 0001EB3C }
	{ r8 = r2; jump 0001EB04; r9 = r3; jump 0001EB04 }
	{ r8 = #0x28; jump 0001E6F8; r9 = #0x29; jump 0001E730 }
	{ p1 = cmp.gtu(r15,#0x7); if (!p1.new) jump:nt 0001ECC0 }
	{ p0 = cmp.eq(r0,r10); if (p0.new) jump:nt 0001EA98 }
	{ p1 = cmp.gt(r7,#-0x1); if (p1.new) jump:nt 0001E6DC }
	{ r5 = r1; jump 0001E6A4; r2 = #0x12; r1 = add(r1,#0x1) }

l0001E900:
	{ call fn00009560; r2 = r20; r1 = memd(r29+16); r0 = memd(r29+24) }
	{ immext(#0xB040); r4 = add(PC,#0xB045); r2 = memw(r25+16); r5 = memw(r29+28) }
	{ r1 = #0x8E; memw(r25+12) = #0xFFFFFF81; memw(r25+8) = #0x1 }
	{ memw(r25) = #0x1; memw(r25+4) = #0xFFFFFF81 }
	{ r2 = r17; immext(#0xBF800000); memw(r2-1082130432) = #0x0; memw(r25+24) = #0x4 }
	{ r3 = memw(r5+16); memw(r5+8) = #0x1 }
	{ memw(r5) = #0x1; memw(r5+4) = #0x1 }
	{ memw(r5+12) = #0xFFFFFF81; immext(#0x3F800000); memw(r3+1065353216) = #0x0 }
	{ call logmsg_function; memw(r5+24) = #0x4; memw(r29) = r16 }
	{ r0 = #0x0 }

l0001E964:
	{ r17:r16 = memd(r29+72); r19:r18 = memd(r29+64) }
	{ r21:r20 = memd(r29+56); r23:r22 = memd(r29+48) }
	{ r25:r24 = memd(r29+40); r27:r26 = memd(r29+32) }
	{ dealloc_return }

;; logmsg_function: 0001E978
;;   Called from:
;;     0001E3EC (in qtanh_execute_ref)
;;     0001E4D8 (in fn0001E49C)
;;     0001E514 (in qtanh_check)
;;     0001E568 (in qtanh_check)
;;     0001E5E0 (in qtanh_execute_hvx)
;;     0001E954 (in qtanh_execute_hvx)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001E99C }

l0001E988:
	{ r0 = add(PC,#0x3C); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001E99C:
	{ dealloc_return }

;; errlog_function: 0001E9A0
;;   Called from:
;;     0001E410 (in qtanh_execute_ref)
;;     0001E530 (in qtanh_check)
;;     0001E5FC (in qtanh_execute_hvx)
;;     0001E990 (in logmsg_function)
errlog_function proc
	{ immext(#0xAF40); r0 = add(PC,#0xAF60); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001E9C4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; qsigmoid_execute_ref: 0001E9D0
qsigmoid_execute_ref proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x40) }
	{ r2 = memw(r16+4); memd(r29+16) = r27:r26 }
	{ r7 = memw(r16+8) }
	{ immext(#0x437F0000); r18 = #0x437F0000; memd(r29+32) = r23:r22; memd(r29+48) = r19:r18 }
	{ r1 = r18; r26 = memw(r2); r4 = memw(r2+4) }
	{ r2 = memw(r2+8); r27 = memw(r7) }
	{ r4 = memw(r4+16); r0 = memw(r26+4) }
	{ r2 = memw(r2+16); r6 = memw(r26) }
	{ r23 = memw(r4); r5 = memw(r26+12) }
	{ r3 = mpyi(r0,r6); r2 = memw(r2); r6 = memw(r26+8) }
	{ memd(r29+40) = r21:r20; memd(r29+24) = r25:r24 }
	{ r2 = mpyi(r3,r6); r0 = sfsub(r2,r23); r20 = memw(r7+8); r21 = memw(r7+4) }

;; fn0001EA34: 0001EA34
;;   Called from:
;;     0001E8A4 (in qtanh_execute_hvx)
;;     0001EA30 (in qsigmoid_execute_ref)
fn0001EA34 proc
	{ r0 = sfsub(r2,r23); r20 = memw(r7+8); r21 = memw(r7+4) }
	{ r22 = memw(r26+16); r24 = memw(r27+16) }
	{ r25 = mpyi(r2,r5); call fn00009610 }
	{ immext(#0xB0C0); r4 = add(PC,#0xB0D1); r1 = #0x42; r2 = r17 }
	{ call logmsg_function; r19 = r0; memw(r29) = r16 }
	{ p0 = cmp.eq(r25,#0x0); r2 = memw(r27+20); if (!cmp.gtu(r25,r2.new)) jump:t fn0001EA8C }

;; fn0001EA74: 0001EA74
;;   Called from:
;;     0001EA5C (in fn0001EA34)
;;     0001EA64 (in fn0001EA34)
fn0001EA74 proc
	{ r3 = add(PC,#0x19); r1 = #0x43; r2 = r17 }
	{ call errlog_function }
	{ jump 0001EB54; r0 = #0xFFFFFFFF }

;; fn0001EA8C: 0001EA8C
;;   Called from:
;;     0001EA5C (in fn0001EA34)
;;     0001EA64 (in fn0001EA34)
fn0001EA8C proc
	{ r2 = memw(r26); r3 = memw(r26+4) }
	{ memw(r27+4) = r3; memw(r27) = r2 }
	{ r6 = memw(r26+8) }
	{ memw(r27+8) = r6 }
	{ immext(#0x42FF0000); if (!p0) r17 = #0x42FF0000; r7 = memw(r26+12); memw(r29+12) = r17 }
	{ if (p0) jump:nt fn0001EB10; memw(r27+12) = r7; memw(r27+24) = r25 }

l0001EAC0:
	{ immext(#0x3F800000); r26 = #0x3F800000; immext(#0x3F000000); r27 = #0x3F000000 }

;; fn0001EAD0: 0001EAD0
;;   Called from:
;;     0001EAC0 (in fn0001EA8C)
;;     0001EEC8 (in qsigmoid_execute_hvx)
fn0001EAD0 proc
	{ r3 = r23; r2 = memb(r22++#1) }
	{ r2 = convert_w2sf(r2) }
	{ r3 += sfmpy(r19,r2) }
	{ r0 = sfmpy(r3,r27); call fn000097F0 }
	{ r3 = sfadd(r0,r26); r2 = r27; r25 = add(r25,#0xFFFFFFFF) }
	{ r2 += sfmpy(r3,r17); p0 = cmp.eq(r25,#0x0) }
	{ r3 = convert_sf2uw(r2) }
	{ p1 = sfcmp.gt(r2,r18); if (p1.new) r3 = #0xFFFFFFFF }

l0001EB08:
	{ if (!p0) jump:nt fn0001EAD0; memb(r24++#1) = r3 }

l0001EB0C:
	{ memb(r24++#1) = r3 }

;; fn0001EB10: 0001EB10
;;   Called from:
;;     0001EAB4 (in fn0001EA8C)
;;     0001EB08 (in fn0001EAD0)
;;     0001EB0C (in qsigmoid_execute_hvx)
;;     0001EB14 (in qsigmoid_execute_hvx)
;;     0001EB18 (in qsigmoid_execute_hvx)
;;     0001EB48 (in qsigmoid_execute_hvx)
;;     0001EEDC (in qsigmoid_execute_hvx)
fn0001EB10 proc
	{ immext(#0xB040); r4 = add(PC,#0xB047); r2 = memw(r21+16); memw(r21+12) = #0x1 }

l0001EB14:
	{ r4 = add(PC,#0x7); r2 = memw(r21+16); memw(r21+12) = #0x1 }

l0001EB18:
	{ r2 = memw(r21+16); memw(r21+12) = #0x1 }

l0001EB1C:
	{ r1 = #0x57; memw(r21+8) = #0x1; memw(r21) = #0x1 }

l0001EB24:
	{ memw(r21+4) = #0x1; memw(r2) = #0x0 }
	{ r2 = memw(r29+12); memw(r21+24) = #0x4 }
	{ r3 = memw(r20+16); memw(r20+8) = #0x1 }
	{ memw(r20) = #0x1; memw(r20+4) = #0x1 }
	{ memw(r20+12) = #0xFFFFFF81; immext(#0x3F800000); memw(r3+1065353216) = #0x0 }
	{ call logmsg_function; memw(r20+24) = #0x4; memw(r29) = r16 }

l0001EB48:
	{ memw(r20+24) = #0x4; memw(r29) = r16 }

l0001EB50:
	{ r0 = #0x0 }

l0001EB54:
	{ r17:r16 = memd(r29+56); r19:r18 = memd(r29+48) }
	{ r21:r20 = memd(r29+40); r23:r22 = memd(r29+32) }
	{ r25:r24 = memd(r29+24); r27:r26 = memd(r29+16) }
	{ dealloc_return }
0001EB68                         00 40 00 7F 00 C0 00 7F         .@......

;; qsigmoid_check: 0001EB70
qsigmoid_check proc
	{ immext(#0xAFC0); r4 = add(PC,#0xAFF7); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x95; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x96; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 0001EBB0 }

l0001EB9C:
	{ r3 = add(PC,#0x28) }
	{ call errlog_function; r2 = r16 }

l0001EBA4:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001EBB0:
	{ r1 = #0x97; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x3)) jump:t 0001EBC8 }

l0001EBC0:
	{ r3 = add(PC,#0x13); jump 0001EBA4 }

l0001EBC8:
	{ immext(#0xAFC0); r4 = add(PC,#0xAFD7); r1 = #0x98; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }
0001EBE8                         00 40 00 7F 00 C0 00 7F         .@......

;; qsigmoid_execute_hvx: 0001EBF0
qsigmoid_execute_hvx proc
	{ immext(#0xAF00); r4 = add(PC,#0xAF2D); memd(r29-16) = r17:r16; allocframe(#0x50) }
	{ r17:r16 = combine(r1,r0) }
	{ r1 = #0x73; r3 = memw(r16+4); memd(r29+64) = r19:r18 }
	{ r2 = r17; r5 = memw(r16+8) }
	{ memd(r29+56) = r21:r20; memd(r29+40) = r25:r24 }
	{ r6 = memw(r3+8); r7 = memw(r3+4) }
	{ r0 = memw(r5+4); r18 = memw(r5) }
	{ r5 = memw(r5+8); r21 = memw(r3) }
	{ r3 = memw(r7+16) }
	{ r5 = memw(r6+16); memw(r29+28) = r5 }
	{ memd(r29+48) = r23:r22 }
	{ r7 = memw(r18+16); memd(r29+32) = r27:r26 }
	{ r25 = memw(r17+4) }
	{ r23 = memw(r21+16); memw(r29+20) = r0 }
	{ r20 = memw(r21+8); r22 = memw(r21+4) }
	{ r26 = memw(r21+12); r27 = memw(r21) }
	{ r19 = memw(r3); memw(r29+24) = r7 }
	{ r24 = memw(r5) }
	{ call logmsg_function; memw(r29) = r16 }
	{ r1 = #0x74; r2 = memb(r16+32); if (!cmp.eq(r2.new,#0x0)) jump:t 0001EC7C }

l0001EC64:
	{ immext(#0xAEC0); r3 = add(PC,#0xAED3) }

l0001EC68:
	{ r3 = add(PC,#0x13) }

l0001EC6C:
	{ call errlog_function; r2 = r17 }

l0001EC70:
	{ r2 = r17 }
	{ jump 0001EFD4; r0 = #0xFFFFFFFF }

l0001EC7C:
	{ r2 = mpyi(r22,r27); r1 = #0x75; r3 = memw(r18+20) }
	{ r2 = mpyi(r2,r20) }
	{ r20 = mpyi(r2,r26); if (!cmp.gtu(r20.new,r3)) jump:t 0001ECA0 }

l0001EC98:
	{ r3 = add(PC,#0x35); jump 0001EC70 }

l0001ECA0:
	{ r22 = add(r20,#0xFF); r2 = #0xFF; r3 = memw(r21); memw(r29+12) = r2 }
	{ r27 = and(r22,#0xFFFFFF00); r0 = r25; r4 = memw(r21+4); memb(r18+1) = r4.new }
	{ memw(r18) = r3 }
	{ r3 = add(r22,r0); r1 = #0x0; r2 = memw(r21+8) }
	{ r2 = r27; memw(r18+8) = r2; memw(r29+16) = r0 }
	{ r7 = memw(r21+12) }
	{ call fn000095F0; r21 = and(r3,#0xFFFFFF00); memw(r18+12) = r7; memw(r18+24) = r20 }
	{ call fn000095F0; r1:r0 = combine(#0x0,r21); r2 = r27 }
	{ call fn00009560; r2 = r20; r1:r0 = combine(r23,r21) }
	{ r1 = sfsub(r24,r19); immext(#0xC0800000); r2 = #0xC0800000 }
	{ p0 = sfcmp.ge(r19,r2); if (!p0.new) jump:nt 0001EDDC }

l0001ED10:
	{ immext(#0x40800000); r18 = #0x40800000 }
	{ p0 = sfcmp.ge(r18,r24); if (!p0.new) jump:nt 0001EDDC }

l0001ED20:
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r2,r1) }
	{ r2 = sfadd(r19,r18); r23 = r0 }
	{ call fn00009610; r1:r0 = combine(r23,r2) }
	{ r18 = convert_uw2sf(r0):chop; immext(#0x41700000); r2 = #0x41700000 }
	{ call fn000097C0; r0 = r2 }
	{ immext(#0x41FF0000); r2 = #0x41FF0000; immext(#0xBF800000); r3 = #0xBF800000 }
	{ r2 = sfmpy(r23,r2); r3 = sfadd(r0,r3); p0 = cmp.gt(r20,#0x0); r25 = memw(r29+20) }
	{ r2 = sfmpy(r2,r0); r3 = convert_uw2sf(r3):chop }
	{ r4 = convert_uw2sf(r2):chop; if (!p0) jump:nt 0001EE98; if (p0) r2 = memw(r29+12) }

l0001ED88:
	{ r5 = convert_uw2sf(r0):chop }
	{ r5 += lsr(r5,#0x1F); p0 = cmp.gt(r4,r3); if (!p0.new) r3 = add(r4,#0x0) }
	{ r6 = mpyi(r2,r26); r2 = r21; r3 = sxth(r3) }
	{ loop0(0001EDA8,r6); r4 = asr(r5,#0x1) }
	{ r5 = #0xFF; r7 = r4; r6 = memb(r2) }
	{ r6 = add(r6,r18) }
	{ r7 += mpyi(r6,r3) }
	{ r6 = asr(r7,#0xF) }
	{ if (p0.new) jump:nt 0001EDD0; p0 = cmp.gt(r6,#0xFF); if (!p0.new) r5 = add(r6,#0x0) }

l0001EDC8:
	{ p0 = cmp.gt(r6,#0xFFFFFFFF); if (!p0.new) r5 = #0x0 }

l0001EDD0:
	{ nop; memb(r2++#1) = r5 }
	{ jump 0001EE98 }

l0001EDDC:
	{ call fn00009600; immext(#0x38D1B700); r0 = #0x38D1B717 }
	{ call fn00009610; immext(#0x437F0000); r1 = #0x437F0000 }
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 0001EE98; r3 = add(r21,r22); r25 = memw(r29+20) }

l0001EE00:
	{ loop0(0001EE0C,r20); r22 = and(r3,#0xFFFFFF00) }
	{ r3:r2 = combine(r22,r21) }
	{ r5 = r19; r4 = memb(r2++#1) }
	{ r4 = convert_w2sf(r4) }
	{ r5 += sfmpy(r0,r4); r3 = add(r3,#0x4) }
	{ r0 = #0x17; immext(#0x41000000); r1 = #0x41000000 }
	{ call fn00009600 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r18 = r21; r23 = r0; r2 = memd(r29+12) }
	{ r19 = mpyi(r2,r26); immext(#0x40800000); r24 = #0x40800000 }

l0001EE5C:
	{ r2 = memw(r22) }
	{ r2 = sfadd(r2,r24) }
	{ r0 = sfmpy(r23,r2); call fn00009620 }
	{ r3 = convert_uw2sf(r0):chop; r2 = #0xFF }
	{ if (p0.new) jump:nt 0001EE88; p0 = cmp.gt(r3,#0xFF); if (!p0.new) r2 = add(r3,#0x0) }

l0001EE80:
	{ p0 = cmp.gt(r3,#0xFFFFFFFF); if (!p0.new) r2 = #0x0 }

l0001EE88:
	{ r19 = add(r19,#0xFFFFFFFF); r22 = add(r22,#0x4); memb(r18++#1) = r2 }
	{ p0 = cmp.eq(r11,#0x0); if (!p0.new) jump:nt 0001EE5C }

l0001EE98:
	{ immext(#0xC240); r2 = add(PC,#0xC278); immext(#0x1010100); r3 = #0x1010101 }
	{ immext(#0x10101000); r4 = #0x10101010; immext(#0x2020200); r5 = #0x2020202 }
	{ immext(#0xF8F8F8C0); r6 = #0xF8F8F8F8; immext(#0x7070700); r7 = #0x7070707 }
	{ p0 = cmp.eq(r3,#-0x1); if (p0.new) jump:nt 0001EB08 }

l0001EECC:
	{ p0 = cmp.eq(r5,#-0x1); if (p0.new) jump:nt 0001EB0C }

l0001EED0:
	{ p0 = cmp.eq(r6,#-0x1); if (p0.new) jump:nt 0001EB14 }

l0001EED4:
	{ p0 = cmp.eq(r7,#-0x1); if (p0.new) jump:nt 0001EB18; p0 = cmp.gt(r27,#0x0) }

l0001EEDC:
	{ p0 = cmp.eq(r4,#-0x1); if (p0.new) jump:nt 0001EB24 }

l0001EEE0:
	{ if (!p0) jump:nt 0001EF74; immext(#0xFFFFFFC0); r4 = memw(r2-32) }

l0001EEEC:
	{ r7 = #0x0; r2 = add(r27,#0x7F); r6 = #0x7; r4 = #0x0; r0 = r0 }
	{ r5 = lsr(r2,#0x7); p0 = cmp.eq(r7,#-0x1); if (p0.new) jump:nt 0001EB48 }

l0001EF04:
	{ loop0(0001EF18,r5); r3 = #0x3; r2 = memw(r29+16); r4 = #0x0; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r0,r7); if (p0.new) jump:nt fn0001F0A4 }

l0001EF18:
	{ p1 = cmp.eq(r2,r0); if (p1.new) jump:nt 0001EC64; p1 = cmp.eq(r3,r0); if (p1.new) jump:nt 0001EC70; r5 = #0x11; r1 = add(r1,#0x1) }

l0001EF24:
	{ p0 = tstbit(r3,#0x0); if (p0.new) jump:nt 0001EFC8 }
	{ p0 = cmp.eq(r4,r12); if (p0.new) jump:nt 0001F0BC }
	{ p1 = cmp.gtu(r7,#0x12); if (p1.new) jump:t 0001ED88; p1 = cmp.gtu(r7,#0xB); if (!p1.new) jump:nt 0001F314 }
	{ p1 = cmp.gtu(r15,#0x12); if (p1.new) jump:t 0001EB80 }
	{ p1 = cmp.eq(r14,r4); if (p1.new) jump:nt 0001F0D4 }
	{ p0 = cmp.eq(r0,r7); if (p0.new) jump:nt 0001F0D4; p1 = cmp.gtu(r6,#0xF); if (!p1.new) jump:nt 0001EF1C }
	{ p1 = cmp.eq(r0,r1); if (p1.new) jump:nt 0001F0E4 }
	{ p0 = cmp.gt(r0,r12); if (p0.new) jump:nt 0001F1AC }
	{ r8 = r2; jump 0001F174; r9 = r3; jump 0001F174 }
	{ r8 = #0x28; jump 0001ED68; r9 = #0x29; jump 0001EDA0 }
	{ p1 = cmp.gtu(r15,#0x7); if (!p1.new) jump:nt 0001F330 }
	{ p0 = cmp.eq(r0,r10); if (p0.new) jump:nt 0001F108 }
	{ p1 = cmp.gt(r7,#-0x1); if (p1.new) jump:nt 0001ED4C }
	{ p1 = cmp.eq(r1,r5); if (p1.new) jump:nt 0001F114 }
	{ p1 = tstbit(r7,#0x0); if (p1.new) jump:nt 0001EF58; r2 = #0x12; r1 = add(r1,#0x1) }

l0001EF74:
	{ call fn00009560; r2 = r20; r1 = memd(r29+16); r0 = memd(r29+24) }
	{ immext(#0xABC0); r4 = add(PC,#0xABD7); r2 = memw(r25+16); r5 = memw(r29+28) }
	{ r1 = #0x8F; memw(r25+12) = #0xFFFFFF81; memw(r25+8) = #0x1 }
	{ memw(r25) = #0x1; memw(r25+4) = #0xFFFFFF81 }
	{ r2 = r17; memw(r2) = #0x0; memw(r25+24) = #0x4 }
	{ r3 = memw(r5+16); memw(r5+8) = #0x1 }
	{ memw(r5) = #0x1; memw(r5+4) = #0x1 }
	{ memw(r5+12) = #0xFFFFFF81; immext(#0x3F800000); memw(r3+1065353216) = #0x0 }
	{ call logmsg_function; memw(r5+24) = #0x4; memw(r29) = r16 }
	{ r0 = #0x0 }

l0001EFD4:
	{ r17:r16 = memd(r29+72); r19:r18 = memd(r29+64) }
	{ r21:r20 = memd(r29+56); r23:r22 = memd(r29+48) }
	{ r25:r24 = memd(r29+40); r27:r26 = memd(r29+32) }
	{ dealloc_return }

;; logmsg_function: 0001EFE8
;;   Called from:
;;     0001EA5C (in fn0001EA34)
;;     0001EA5C (in fn0001EA34)
;;     0001EB44 (in fn0001EB10)
;;     0001EB84 (in qsigmoid_check)
;;     0001EBD8 (in qsigmoid_check)
;;     0001EC50 (in qsigmoid_execute_hvx)
;;     0001EFC4 (in qsigmoid_execute_hvx)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001F00C }

l0001EFF8:
	{ r0 = add(PC,#0xC); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001F00C:
	{ dealloc_return }

;; errlog_function: 0001F010
;;   Called from:
;;     0001EA80 (in fn0001EA74)
;;     0001EBA0 (in qsigmoid_check)
;;     0001EC6C (in qsigmoid_execute_hvx)
;;     0001F000 (in logmsg_function)
errlog_function proc
	{ immext(#0xAAC0); r0 = add(PC,#0xAAF0); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001F034             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; add_f_execute: 0001F040
add_f_execute proc
	{ r5 = r0; allocframe(#0xA0) }
	{ r2 = memw(r5+4); r3 = memw(r5+8) }
	{ memd(r29+112) = r27:r26; memd(r29+144) = r19:r18 }
	{ r26 = memw(r2); r19 = memw(r2+4) }
	{ r22 = #0x0; memd(r29+136) = r21:r20; memd(r29+128) = r23:r22 }
	{ r6 = memw(r26+4); r0 = memw(r26) }
	{ p0 = cmp.eq(r6,#0x1); p1 = cmp.eq(r0,#0x1); r8 = memw(r19); r4 = memw(r19+4) }
	{ r10 = p1; r12 = memw(r26+8) }
	{ r18 = mux(p0,r4,r6); p2 = cmp.eq(r12,#0x1); r9 = memw(r19+8); memw(r29+92) = r4 }
	{ r4 = mux(p1,r8,r0); r7 = memw(r26+12) }
	{ r2 = mpyi(r4,r18); p1 = cmp.eq(r7,#0x1); r0 = memw(r19+12) }

;; fn0001F0A4: 0001F0A4
;;   Called from:
;;     0001EF14 (in qsigmoid_execute_hvx)
;;     0001F098 (in add_f_execute)
fn0001F0A4 proc
	{ r20 = mux(p2,r9,r12); memd(r29+120) = r25:r24; memw(r29+76) = r6 }
	{ r2 = mpyi(r2,r20); r21 = mux(p1,r0,r7) }
	{ r2 = mpyi(r2,r21); r6 = r1; r25 = memw(r3); memd(r29+152) = r17:r16 }
	{ r10 = p2; memw(r29+96) = r10; memw(r29+72) = r8 }
	{ memw(r29+68) = r9; memw(r29+84) = r10 }
	{ r16 = asl(r2,#0x2); memw(r29+104) = r4; memw(r29+80) = r0 }
	{ if (p0) jump:nt fn0001F0EC }

;; fn0001F0E8: 0001F0E8
;;   Called from:
;;     0001F0E4 (in fn0001F0A4)
;;     0001F0E4 (in fn0001F0A4)
fn0001F0E8 proc
	{ r22 = mpyi(r7,r12) }

;; fn0001F0EC: 0001F0EC
;;   Called from:
;;     0001F0E4 (in fn0001F0A4)
;;     0001F0E4 (in fn0001F0A4)
;;     0001F0E8 (in fn0001F0E8)
;;     0001F348 (in add_f_execute)
;;     0001F35C (in add_f_execute)
fn0001F0EC proc
	{ immext(#0xAB00); r0 = add(PC,#0xAB22); r1 = #0xBC; r2 = r6 }
	{ immext(#0xAB00); r4 = add(PC,#0xAB31); r3 = memw(r19+16); memw(r29+64) = r12 }
	{ r24 = r6; r27 = r5; r23 = memw(r25+16); memw(r29+88) = r7 }
	{ r17 = memw(r26+16) }
	{ call logmsg_function; memw(r29+100) = r3; memw(r29) = r5 }
	{ r1 = #0xBC; r2 = memw(r25+20); if (!cmp.gtu(r16,r2.new)) jump:t 0001F154 }

l0001F138:
	{ r0 = add(PC,#0x1A); r2 = r24 }
	{ immext(#0xAB00); r3 = add(PC,#0xAB0B) }

l0001F148:
	{ call errlog_function }
	{ jump 0001F380; r0 = #0xFFFFFFFF }

l0001F154:
	{ r2 = r24; r13 = memw(r19); r5 = memw(r26) }
	{ p0 = cmp.eq(r5,r13); r8 = memw(r26+12); r7 = memw(r26+8) }
	{ r6 = memw(r26+4); r12 = memw(r19+12) }
	{ r9 = memw(r19+8); r3 = memw(r19+4) }
	{ memw(r29+48) = r25; memw(r29+52) = r16 }
	{ memw(r29+56) = r17; memw(r29+60) = r27 }
	{ if (p0) jump:nt 0001F19C }

l0001F190:
	{ p0 = cmp.eq(r5,#0x1); if (p0.new) jump:nt 0001F19C }

l0001F194:
	{ if (!p0.new) jump:nt 0001F1D8; p0 = cmp.eq(r13,#0x1) }

l0001F19C:
	{ p0 = cmp.eq(r6,r3); if (p0.new) jump:nt 0001F1AC }

l0001F1A0:
	{ p0 = cmp.eq(r6,#0x1); if (p0.new) jump:nt 0001F1AC }

l0001F1A4:
	{ p0 = cmp.eq(r3,#0x1); if (!p0.new) jump:nt 0001F1D8; nop }

l0001F1AC:
	{ if (p0.new) jump:nt 0001F1C0; p0 = cmp.eq(r7,r9) }

l0001F1B4:
	{ p0 = cmp.eq(r7,#0x1); if (p0.new) jump:nt 0001F1C0 }

l0001F1B8:
	{ if (!p0.new) jump:nt 0001F1D8; p0 = cmp.eq(r9,#0x1) }

l0001F1C0:
	{ if (p0.new) jump:nt 0001F20C; p0 = cmp.eq(r8,r12) }

l0001F1C8:
	{ if (p0.new) jump:nt 0001F20C; p0 = cmp.eq(r8,#0x1) }

l0001F1D0:
	{ if (p0.new) jump:nt 0001F20C; p0 = cmp.eq(r12,#0x1) }

l0001F1D8:
	{ immext(#0xAA00); r0 = add(PC,#0xAA36); r1 = #0xBC; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r13 }
	{ immext(#0xAA40); r3 = add(PC,#0xAA69); memw(r29+20) = r3; memw(r29+8) = r7 }
	{ memw(r29+12) = r8; memw(r29+4) = r6 }
	{ jump 0001F148; memw(r29) = r5 }

l0001F20C:
	{ r19 = r2; r24 = memw(r29+104); memw(r29+44) = r21 }
	{ immext(#0xA9C0); r0 = add(PC,#0xA9F6); memw(r29+36) = r18; memw(r29+20) = r3 }
	{ immext(#0xAA40); r4 = add(PC,#0xAA65); r1 = #0xBC; memw(r29+40) = r20 }
	{ memw(r29+32) = r24; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r13 }
	{ memw(r29+12) = r8; memw(r29+8) = r7 }
	{ memw(r29+4) = r6; memw(r29) = r5 }
	{ call logmsg_function }
	{ p0 = cmp.gt(r24,#0x0); if (p0.new) r14 = #0x0; r3 = memw(r29+48); r26 = memw(r29+100) }
	{ r25 = memw(r29+56) }
	{ r2 = memw(r29+52); memb(r3+6) = r2.new }
	{ memw(r3+4) = r18 }
	{ memw(r3+8) = r20; memw(r3+12) = r21 }
	{ if (!p0) jump:nt 0001F358; if (p0) r13 = memw(r29+80); if (p0) r12 = memw(r29+88) }

l0001F288:
	{ r6 = !cmp.eq(r13,00000001); r5 = memd(r29+68); r7 = memd(r29+92) }
	{ r9 = mpyi(r5,r7); r0 = memd(r29+84); r2 = memd(r29+76) }
	{ p2 = r0; p0 = cmp.eq(r5,#0x1); r3 = memd(r29+64); r4 = memd(r29+72) }
	{ r8 = mpyi(r3,r2); r9 = mpyi(r9,r13); p1 = cmp.eq(r4,#0x1); r0 = memw(r29+96) }
	{ r4 = mpyi(r13,r5); r2 = mux(p2,#0x0,r12); p2 = cmp.eq(r7,#0x1); if (p1) r9 = add(r14,#0x0) }
	{ r8 = mpyi(r8,r12); p0 = r0; r3 = mux(p0,#0x0,r13); r5 = !cmp.eq(r12,00000001) }
	{ r7 = #0x0; if (p2) r4 = add(r14,#0x0); if (p0) r8 = add(r14,#0x0) }

l0001F2E0:
	{ p0 = cmp.gt(r10,#0x0); if (!p0.new) jump:nt 0001F348; r13:r12 = combine(r25,r26); r14 = #0x0 }

l0001F2E4:
	{ r13:r12 = combine(r25,r26); r14 = #0x0 }

l0001F2EC:
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 0001F338; r28 = r12; r15 = r13 }

l0001F2F8:
	{ loop1(0001F2FC,r20) }
	{ p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 0001F32C; r10 = r23; if (!p0) r1:r0 = combine(r15,r28) }

l0001F308:
	{ loop0(0001F30C,r21) }
	{ r0 = addasl(r0,r6,#0x2); r1 = addasl(r1,r5,#0x2); r11 = memw(r1); r16 = memw(r0) }
	{ r11 = sfadd(r11,r16) }
	{ r10 = add(r10,#0x4); memw(r10) = r11 }
	{ r23 = addasl(r23,r21,#0x2) }

l0001F32C:
	{ r28 = addasl(r28,r3,#0x2); r15 = addasl(r15,r2,#0x2); nop }

l0001F338:
	{ r13 = addasl(r13,r22,#0x2); r12 = addasl(r12,r4,#0x2); r14 = add(r14,#0x1); if (!cmp.eq(r14.new,r18)) jump:t 0001F2EC }

l0001F348:
	{ r25 = addasl(r25,r8,#0x2); r26 = addasl(r26,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r24)) jump:t 0001F2E0 }

l0001F34C:
	{ r26 = addasl(r26,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r24)) jump:t 0001F2E4 }

l0001F358:
	{ immext(#0xA880); r0 = add(PC,#0xA8B6); r2 = memw(r29+60); memb(r29) = r2.new }

l0001F35C:
	{ r0 = add(PC,#0x36); r2 = memw(r29+60); memb(r29) = r2.new }

l0001F36C:
	{ r4 = add(PC,#0x11); r1 = #0xBC; r2 = r19 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001F380:
	{ r17:r16 = memd(r29+152); r19:r18 = memd(r29+144) }
	{ r21:r20 = memd(r29+136); r23:r22 = memd(r29+128) }
	{ r25:r24 = memd(r29+120); r27:r26 = memd(r29+112) }
	{ dealloc_return }

;; add_f_check: 0001F394
add_f_check proc
	{ immext(#0xA800); r4 = add(PC,#0xA83F); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r17 = r0; r1 = #0x37; r16 = r1 }
	{ immext(#0xA800); r0 = add(PC,#0xA810); r2 = r16; memw(r29) = r17 }
	{ call logmsg_function }
	{ r1 = #0x38; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 0001F3E4 }

l0001F3C8:
	{ r0 = add(PC,#0x34) }
	{ immext(#0xA800); r3 = add(PC,#0xA813) }

l0001F3D4:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001F3E4:
	{ r1 = #0x39; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001F404 }

l0001F3F4:
	{ r0 = add(PC,#0x8) }
	{ immext(#0xA7C0); r3 = add(PC,#0xA7F6); jump 0001F3D4 }

l0001F404:
	{ immext(#0xA780); r0 = add(PC,#0xA7B4); r1 = #0x3A; r2 = r16 }
	{ immext(#0xA7C0); r4 = add(PC,#0xA7EE); call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001F428
;;   Called from:
;;     0001F120 (in fn0001F0EC)
;;     0001F250 (in fn0001F0EC)
;;     0001F378 (in fn0001F0EC)
;;     0001F3B4 (in add_f_check)
;;     0001F410 (in add_f_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001F448 }

l0001F438:
	{ r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001F448:
	{ dealloc_return }

;; errlog_function: 0001F44C
;;   Called from:
;;     0001F148 (in fn0001F0EC)
;;     0001F3D4 (in add_f_check)
errlog_function proc
	{ r3 = #0x0; r4 = r3; allocframe(#0x8) }
	{ call logv; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; mul_f_execute: 0001F470
;;   Called from:
;;     0001F46C (in errlog_function)
mul_f_execute proc
	{ immext(#0xA880); r4 = add(PC,#0xA8BA); memd(r29-40) = r23:r22; allocframe(#0xA8) }
	{ r23:r22 = combine(r1,#0x0); memd(r29+128) = r25:r24; memd(r29+120) = r27:r26 }
	{ r1 = #0x32; r2 = r23; r16 = r0; memd(r29+160) = r17:r16 }
	{ immext(#0xA840); r0 = add(PC,#0xA840); memw(r29) = r16 }
	{ memd(r29+152) = r19:r18; memd(r29+144) = r21:r20 }
	{ call logmsg_function }
	{ r5 = r16 }
	{ r2 = memw(r5+4); r3 = memw(r5+8) }
	{ r16 = memw(r2) }
	{ r19 = memw(r2+4); r3 = memw(r3) }
	{ r7 = memw(r16+4); r1 = memw(r16) }
	{ p0 = cmp.eq(r7,#0x1); p1 = cmp.eq(r1,#0x1); r8 = memw(r19); r4 = memw(r19+4) }
	{ r12 = memw(r16+8) }
	{ r18 = mux(p0,r4,r7); p2 = cmp.eq(r12,#0x1); r9 = memw(r19+8); memw(r29+96) = r4 }
	{ r0 = p1; r4 = mux(p1,r8,r1); r6 = memw(r16+12) }
	{ r2 = mpyi(r4,r18); p1 = cmp.eq(r6,#0x1); r1 = memw(r19+12) }
	{ r0 = p2; r20 = mux(p2,r9,r12); memw(r29+100) = r0; memw(r29+112) = r4 }
	{ r2 = mpyi(r2,r20); r21 = mux(p1,r1,r6); memw(r29+72) = r9; memw(r29+80) = r7 }
	{ r2 = mpyi(r2,r21); memw(r29+76) = r8; memw(r29+88) = r0 }
	{ r17 = asl(r2,#0x2); if (p0) jump:nt 0001F530; memw(r29+84) = r1 }

l0001F52C:
	{ r22 = mpyi(r6,r12) }

l0001F530:
	{ immext(#0xA800); r0 = add(PC,#0xA808); r1 = #0xBC; r2 = r23 }
	{ immext(#0xA800); r4 = add(PC,#0xA817); r27 = r23; memw(r29+92) = r6 }
	{ r6 = memw(r19+16) }
	{ r23 = memw(r3+16); memw(r29+68) = r12 }
	{ r25:r24 = combine(r3,r5); r26 = memw(r16+16) }
	{ call logmsg_function; memw(r29+108) = r6; memw(r29) = r5 }
	{ r1 = #0xBC; r2 = memw(r25+20); if (!cmp.gtu(r17,r2.new)) jump:t 0001F598 }

l0001F57C:
	{ r0 = add(PC,#0x0); r2 = r27 }
	{ immext(#0xA7C0); r3 = add(PC,#0xA7F1) }

l0001F58C:
	{ call errlog_function }
	{ jump 0001F7AC; r0 = #0xFFFFFFFF }

l0001F598:
	{ r2 = memw(r19); r5 = memw(r16) }
	{ p0 = cmp.eq(r5,r2); r8 = memw(r16+12); r7 = memw(r16+8) }
	{ r6 = memw(r16+4); r12 = memw(r19+12) }
	{ r9 = memw(r19+8); r3 = memw(r19+4) }
	{ memw(r29+52) = r25; memw(r29+56) = r17 }
	{ memw(r29+60) = r26; memw(r29+64) = r24 }
	{ if (p0) jump:nt 0001F5D8; memw(r29+104) = r27 }

l0001F5D0:
	{ p0 = cmp.eq(r5,#0x1); if (p0.new) jump:nt 0001F5D8 }

l0001F5D4:
	{ p0 = cmp.eq(r2,#0x1); if (!p0.new) jump:nt 0001F614 }

l0001F5D8:
	{ p0 = cmp.eq(r6,r3); if (p0.new) jump:nt 0001F5E8; nop }

l0001F5E0:
	{ p0 = cmp.eq(r6,#0x1); if (p0.new) jump:nt 0001F5E8 }

l0001F5E4:
	{ p0 = cmp.eq(r3,#0x1); if (!p0.new) jump:nt 0001F614 }

l0001F5E8:
	{ if (p0.new) jump:nt 0001F5FC; p0 = cmp.eq(r7,r9) }

l0001F5F0:
	{ p0 = cmp.eq(r7,#0x1); if (p0.new) jump:nt 0001F5FC }

l0001F5F4:
	{ if (!p0.new) jump:nt 0001F614; p0 = cmp.eq(r9,#0x1) }

l0001F5FC:
	{ if (p0.new) jump:nt 0001F648; p0 = cmp.eq(r8,r12) }

l0001F604:
	{ if (p0.new) jump:nt 0001F648; p0 = cmp.eq(r8,#0x1) }

l0001F60C:
	{ if (p0.new) jump:nt 0001F648; p0 = cmp.eq(r12,#0x1) }

l0001F614:
	{ immext(#0xA700); r0 = add(PC,#0xA724); r1 = #0xBC; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r2 }
	{ immext(#0xA740); r3 = add(PC,#0xA757); memw(r29+20) = r3; memw(r29+4) = r6 }
	{ r2 = memd(r29+104); memw(r29+8) = r7 }
	{ memw(r29+12) = r8; memw(r29) = r5 }
	{ jump 0001F58C }

l0001F648:
	{ immext(#0xA6C0); r0 = add(PC,#0xA6F0); r19 = memd(r29+112); memw(r29+44) = r21 }
	{ immext(#0xA740); r4 = add(PC,#0xA75F); r1 = #0xBC; memw(r29+32) = r19 }
	{ memw(r29+40) = r20; memw(r29+36) = r18 }
	{ memw(r29+28) = r12; memw(r29+24) = r9 }
	{ memw(r29+20) = r3; memw(r29+16) = r2 }
	{ r2 = memd(r29+104); memw(r29+4) = r6 }
	{ memw(r29+12) = r8; memw(r29+8) = r7 }
	{ call logmsg_function; memw(r29) = r5 }
	{ p0 = cmp.gt(r19,#0x0); if (p0.new) r14 = #0x0; r3 = memd(r29+52); r17 = memd(r29+108) }
	{ r24 = memw(r29+60) }
	{ r2 = memw(r29+56); memb(r3+6) = r2.new }
	{ memw(r3+8) = r20; memw(r3+12) = r21 }
	{ if (!p0) jump:nt 0001F784; if (p0) r13 = memw(r29+84); if (p0) r12 = memw(r29+92) }

l0001F6B4:
	{ r6 = !cmp.eq(r13,00000001); r5 = memd(r29+72); r7 = memd(r29+96) }
	{ r9 = mpyi(r5,r7); r0 = memd(r29+88); r2 = memd(r29+80) }
	{ p2 = r0; p0 = cmp.eq(r5,#0x1); r3 = memd(r29+68); r4 = memd(r29+76) }
	{ r8 = mpyi(r3,r2); r9 = mpyi(r9,r13); p1 = cmp.eq(r4,#0x1); r0 = memw(r29+100) }
	{ r4 = mpyi(r13,r5); r2 = mux(p2,#0x0,r12); p2 = cmp.eq(r7,#0x1); if (p1) r9 = add(r14,#0x0) }
	{ r8 = mpyi(r8,r12); p0 = r0; r3 = mux(p0,#0x0,r13); r5 = !cmp.eq(r12,00000001) }
	{ r7 = #0x0; if (p2) r4 = add(r14,#0x0); if (p0) r8 = add(r14,#0x0) }

l0001F70C:
	{ p0 = cmp.gt(r10,#0x0); if (!p0.new) jump:nt 0001F774; r13:r12 = combine(r24,r17); r14 = #0x0 }

l0001F710:
	{ r13:r12 = combine(r24,r17); r14 = #0x0 }

l0001F718:
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 0001F764; r28 = r12; r15 = r13 }

l0001F724:
	{ loop1(0001F728,r20) }
	{ p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 0001F758; r10 = r23; if (!p0) r1:r0 = combine(r15,r28) }

l0001F734:
	{ loop0(0001F738,r21) }
	{ r0 = addasl(r0,r6,#0x2); r1 = addasl(r1,r5,#0x2); r11 = memw(r1); r16 = memw(r0) }
	{ r11 = sfmpy(r11,r16) }
	{ r10 = add(r10,#0x4); memw(r10) = r11 }
	{ r23 = addasl(r23,r21,#0x2) }

l0001F758:
	{ r28 = addasl(r28,r3,#0x2); r15 = addasl(r15,r2,#0x2); nop }

l0001F764:
	{ r13 = addasl(r13,r22,#0x2); r12 = addasl(r12,r4,#0x2); r14 = add(r14,#0x1); if (!cmp.eq(r14.new,r18)) jump:t 0001F718 }

l0001F774:
	{ r24 = addasl(r24,r8,#0x2); r17 = addasl(r17,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r19)) jump:t 0001F70C }

l0001F778:
	{ r17 = addasl(r17,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r19)) jump:t 0001F710 }

l0001F784:
	{ immext(#0xA580); r0 = add(PC,#0xA5B4); r2 = memw(r29+64); memb(r29) = r2.new }

l0001F788:
	{ r0 = add(PC,#0x34); r2 = memw(r29+64); memb(r29) = r2.new }

l0001F798:
	{ r4 = add(PC,#0xF); r1 = #0xBC; r2 = memw(r29+104) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001F7AC:
	{ r17:r16 = memd(r29+160); r19:r18 = memd(r29+152) }
	{ r21:r20 = memd(r29+144); r23:r22 = memd(r29+136) }
	{ r25:r24 = memd(r29+128); r27:r26 = memd(r29+120) }
	{ dealloc_return }

;; mul_f_check: 0001F7C0
mul_f_check proc
	{ immext(#0xA500); r4 = add(PC,#0xA52F); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r17 = r0; r1 = #0x38; r16 = r1 }
	{ immext(#0xA500); r0 = add(PC,#0xA500); r2 = r16; memw(r29) = r17 }
	{ call logmsg_function }
	{ r1 = #0x39; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 0001F810 }

l0001F7F4:
	{ r0 = add(PC,#0x24) }
	{ immext(#0xA500); r3 = add(PC,#0xA503) }

l0001F800:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001F810:
	{ r1 = #0x3A; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001F830 }

l0001F820:
	{ r0 = add(PC,#0x38) }
	{ immext(#0xA4C0); r3 = add(PC,#0xA4E6); jump 0001F800 }

l0001F830:
	{ immext(#0xA480); r0 = add(PC,#0xA4A4); r1 = #0x3B; r2 = r16 }
	{ immext(#0xA4C0); r4 = add(PC,#0xA4DE); call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001F854
;;   Called from:
;;     0001F4A4 (in mul_f_execute)
;;     0001F564 (in mul_f_execute)
;;     0001F680 (in mul_f_execute)
;;     0001F7A4 (in mul_f_execute)
;;     0001F7E0 (in mul_f_check)
;;     0001F83C (in mul_f_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001F874 }

l0001F864:
	{ r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001F874:
	{ dealloc_return }

;; errlog_function: 0001F878
;;   Called from:
;;     0001F58C (in mul_f_execute)
;;     0001F800 (in mul_f_check)
errlog_function proc
	{ r3 = #0x0; r4 = r3; allocframe(#0x8) }
	{ call logv; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; neg_f_execute: 0001F8A0
;;   Called from:
;;     0001F89C (in errlog_function)
neg_f_execute proc
	{ allocframe(#0x0) }
	{ r7 = memw(r0+4); r5 = memw(r0+8) }
	{ r3 = memw(r7) }
	{ r2 = memw(r3+4); r7 = memw(r3+12) }
	{ r4 = memw(r3); r6 = memw(r3+8) }
	{ r2 = mpyi(r2,r4); r4 = memw(r5) }
	{ r2 = mpyi(r2,r6); r6 = memw(r4+20) }
	{ r7 = mpyi(r2,r7); r2 = r1 }
	{ r5 = asl(r7,#0x2); if (cmp.gtu(r5.new,r6)) jump:t 0001F910 }

l0001F8D8:
	{ loop0(0001F8E0,r7); r2 = memw(r4+16); r6 = memw(r3+16) }
	{ r6 = add(r6,#0x4); r7 = memw(r6) }
	{ r7 = togglebit(r7,#0x1E) }
	{ nop; r2 = add(r2,#0x4); memw(r2) = r7 }
	{ r0 = #0x0; r2 = memw(r3); r6 = memw(r3+4) }
	{ memw(r4+4) = r6; memw(r4) = r2 }
	{ r1 = memw(r3+8) }
	{ memw(r4+8) = r1 }
	{ r7 = memw(r3+12) }
	{ memw(r4+12) = r7; memw(r4+24) = r5 }
	{ dealloc_return }

l0001F910:
	{ immext(#0xA540); r3 = add(PC,#0xA544); call errlog_function; r1 = #0x36 }
	{ r0 = #-0x1; dealloc_return }

;; neg_f_check: 0001F924
neg_f_check proc
	{ immext(#0xA4C0); r4 = add(PC,#0xA4F5); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x41; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x42; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x1)) jump:t 0001F964 }

l0001F950:
	{ r3 = add(PC,#0x19) }
	{ call errlog_function; r2 = r16 }

l0001F958:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001F964:
	{ r1 = #0x43; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001F97C }

l0001F974:
	{ r3 = add(PC,#0x4); jump 0001F958 }

l0001F97C:
	{ immext(#0xA4C0); r4 = add(PC,#0xA4C8); r1 = #0x44; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001F99C
;;   Called from:
;;     0001F938 (in neg_f_check)
;;     0001F98C (in neg_f_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001F9C0 }

l0001F9AC:
	{ r0 = add(PC,#0x16); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001F9C0:
	{ dealloc_return }

;; errlog_function: 0001F9C4
;;   Called from:
;;     0001F910 (in neg_f_execute)
;;     0001F954 (in neg_f_check)
;;     0001F9B4 (in logmsg_function)
errlog_function proc
	{ immext(#0xA400); r0 = add(PC,#0xA43A); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001F9E8                         00 00 00 00 00 00 00 00         ........

;; sub_f_execute: 0001F9F0
sub_f_execute proc
	{ r5 = r0; allocframe(#0xA0) }
	{ r2 = memw(r5+4); r3 = memw(r5+8) }
	{ memd(r29+112) = r27:r26; memd(r29+144) = r19:r18 }
	{ r26 = memw(r2); r19 = memw(r2+4) }
	{ r22 = #0x0; memd(r29+136) = r21:r20; memd(r29+128) = r23:r22 }
	{ r6 = memw(r26+4); r0 = memw(r26) }
	{ p0 = cmp.eq(r6,#0x1); p1 = cmp.eq(r0,#0x1); r8 = memw(r19); r4 = memw(r19+4) }
	{ r10 = p1; r12 = memw(r26+8) }
	{ r18 = mux(p0,r4,r6); p2 = cmp.eq(r12,#0x1); r9 = memw(r19+8); memw(r29+92) = r4 }
	{ r4 = mux(p1,r8,r0); r7 = memw(r26+12) }
	{ r2 = mpyi(r4,r18); p1 = cmp.eq(r7,#0x1); r0 = memw(r19+12) }
	{ r20 = mux(p2,r9,r12); memd(r29+120) = r25:r24; memw(r29+76) = r6 }
	{ r2 = mpyi(r2,r20); r21 = mux(p1,r0,r7) }
	{ r2 = mpyi(r2,r21); r6 = r1; r25 = memw(r3); memd(r29+152) = r17:r16 }
	{ r10 = p2; memw(r29+96) = r10; memw(r29+72) = r8 }
	{ memw(r29+68) = r9; memw(r29+84) = r10 }
	{ r16 = asl(r2,#0x2); memw(r29+104) = r4; memw(r29+80) = r0 }
	{ if (p0) jump:nt 0001FA9C }

l0001FA98:
	{ r22 = mpyi(r7,r12) }

l0001FA9C:
	{ immext(#0xA400); r0 = add(PC,#0xA41C); r1 = #0xBC; r2 = r6 }
	{ immext(#0xA400); r4 = add(PC,#0xA42B); r3 = memw(r19+16); memw(r29+64) = r12 }
	{ r24 = r6; r27 = r5; r23 = memw(r25+16); memw(r29+88) = r7 }
	{ r17 = memw(r26+16) }
	{ call logmsg_function; memw(r29+100) = r3; memw(r29) = r5 }
	{ r1 = #0xBC; r2 = memw(r25+20); if (!cmp.gtu(r16,r2.new)) jump:t 0001FB04 }

l0001FAE8:
	{ r0 = add(PC,#0x14); r2 = r24 }
	{ immext(#0xA400); r3 = add(PC,#0xA405) }

l0001FAF8:
	{ call errlog_function }
	{ jump 0001FD30; r0 = #0xFFFFFFFF }

l0001FB04:
	{ r2 = r24; r13 = memw(r19); r5 = memw(r26) }
	{ p0 = cmp.eq(r5,r13); r8 = memw(r26+12); r7 = memw(r26+8) }
	{ r6 = memw(r26+4); r12 = memw(r19+12) }
	{ r9 = memw(r19+8); r3 = memw(r19+4) }
	{ memw(r29+48) = r25; memw(r29+52) = r16 }
	{ memw(r29+56) = r17; memw(r29+60) = r27 }
	{ if (p0) jump:nt 0001FB4C }

l0001FB40:
	{ p0 = cmp.eq(r5,#0x1); if (p0.new) jump:nt 0001FB4C }

l0001FB44:
	{ if (!p0.new) jump:nt 0001FB88; p0 = cmp.eq(r13,#0x1) }

l0001FB4C:
	{ p0 = cmp.eq(r6,r3); if (p0.new) jump:nt 0001FB5C }

l0001FB50:
	{ p0 = cmp.eq(r6,#0x1); if (p0.new) jump:nt 0001FB5C }

l0001FB54:
	{ p0 = cmp.eq(r3,#0x1); if (!p0.new) jump:nt 0001FB88; nop }

l0001FB5C:
	{ if (p0.new) jump:nt 0001FB70; p0 = cmp.eq(r7,r9) }

l0001FB64:
	{ p0 = cmp.eq(r7,#0x1); if (p0.new) jump:nt 0001FB70 }

l0001FB68:
	{ if (!p0.new) jump:nt 0001FB88; p0 = cmp.eq(r9,#0x1) }

l0001FB70:
	{ if (p0.new) jump:nt 0001FBBC; p0 = cmp.eq(r8,r12) }

l0001FB78:
	{ if (p0.new) jump:nt 0001FBBC; p0 = cmp.eq(r8,#0x1) }

l0001FB80:
	{ if (p0.new) jump:nt 0001FBBC; p0 = cmp.eq(r12,#0x1) }

l0001FB88:
	{ immext(#0xA300); r0 = add(PC,#0xA330); r1 = #0xBC; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r13 }
	{ immext(#0xA340); r3 = add(PC,#0xA363); memw(r29+20) = r3; memw(r29+8) = r7 }
	{ memw(r29+12) = r8; memw(r29+4) = r6 }
	{ jump 0001FAF8; memw(r29) = r5 }

l0001FBBC:
	{ r19 = r2; r24 = memw(r29+104); memw(r29+44) = r21 }
	{ immext(#0xA2C0); r0 = add(PC,#0xA2F0); memw(r29+36) = r18; memw(r29+20) = r3 }
	{ immext(#0xA340); r4 = add(PC,#0xA35F); r1 = #0xBC; memw(r29+40) = r20 }
	{ memw(r29+32) = r24; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r13 }
	{ memw(r29+12) = r8; memw(r29+8) = r7 }
	{ memw(r29+4) = r6; memw(r29) = r5 }
	{ call logmsg_function }
	{ p0 = cmp.gt(r24,#0x0); if (p0.new) r14 = #0x0; r3 = memw(r29+48); r26 = memw(r29+100) }
	{ r25 = memw(r29+56) }
	{ r2 = memw(r29+52); memb(r3+6) = r2.new }
	{ memw(r3+4) = r18 }
	{ memw(r3+8) = r20; memw(r3+12) = r21 }
	{ if (!p0) jump:nt 0001FD08; if (p0) r13 = memw(r29+80); if (p0) r12 = memw(r29+88) }

l0001FC38:
	{ r6 = !cmp.eq(r13,00000001); r5 = memd(r29+68); r7 = memd(r29+92) }
	{ r9 = mpyi(r5,r7); r0 = memd(r29+84); r2 = memd(r29+76) }
	{ p2 = r0; p0 = cmp.eq(r5,#0x1); r3 = memd(r29+64); r4 = memd(r29+72) }
	{ r8 = mpyi(r3,r2); r9 = mpyi(r9,r13); p1 = cmp.eq(r4,#0x1); r0 = memw(r29+96) }
	{ r4 = mpyi(r13,r5); r2 = mux(p2,#0x0,r12); p2 = cmp.eq(r7,#0x1); if (p1) r9 = add(r14,#0x0) }
	{ r8 = mpyi(r8,r12); p0 = r0; r3 = mux(p0,#0x0,r13); r5 = !cmp.eq(r12,00000001) }
	{ r7 = #0x0; if (p2) r4 = add(r14,#0x0); if (p0) r8 = add(r14,#0x0) }

l0001FC90:
	{ p0 = cmp.gt(r10,#0x0); if (!p0.new) jump:nt 0001FCF8; r13:r12 = combine(r25,r26); r14 = #0x0 }

l0001FC94:
	{ r13:r12 = combine(r25,r26); r14 = #0x0 }

l0001FC9C:
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 0001FCE8; r28 = r12; r15 = r13 }

l0001FCA8:
	{ loop1(0001FCAC,r20) }
	{ p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 0001FCDC; r10 = r23; if (!p0) r1:r0 = combine(r15,r28) }

l0001FCB8:
	{ loop0(0001FCBC,r21) }
	{ r0 = addasl(r0,r6,#0x2); r1 = addasl(r1,r5,#0x2); r11 = memw(r1); r16 = memw(r0) }
	{ r11 = sfsub(r11,r16) }
	{ r10 = add(r10,#0x4); memw(r10) = r11 }
	{ r23 = addasl(r23,r21,#0x2) }

l0001FCDC:
	{ r28 = addasl(r28,r3,#0x2); r15 = addasl(r15,r2,#0x2); nop }

l0001FCE8:
	{ r13 = addasl(r13,r22,#0x2); r12 = addasl(r12,r4,#0x2); r14 = add(r14,#0x1); if (!cmp.eq(r14.new,r18)) jump:t 0001FC9C }

l0001FCF8:
	{ r25 = addasl(r25,r8,#0x2); r26 = addasl(r26,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r24)) jump:t 0001FC90 }

l0001FCFC:
	{ r26 = addasl(r26,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r24)) jump:t 0001FC94 }

l0001FD08:
	{ immext(#0xA180); r0 = add(PC,#0xA1B0); r2 = memw(r29+60); memb(r29) = r2.new }

l0001FD0C:
	{ r0 = add(PC,#0x30); r2 = memw(r29+60); memb(r29) = r2.new }

l0001FD1C:
	{ r4 = add(PC,#0xB); r1 = #0xBC; r2 = r19 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001FD30:
	{ r17:r16 = memd(r29+152); r19:r18 = memd(r29+144) }
	{ r21:r20 = memd(r29+136); r23:r22 = memd(r29+128) }
	{ r25:r24 = memd(r29+120); r27:r26 = memd(r29+112) }
	{ dealloc_return }

;; sub_f_check: 0001FD44
sub_f_check proc
	{ immext(#0xA100); r4 = add(PC,#0xA139); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r17 = r0; r1 = #0x37; r16 = r1 }
	{ immext(#0xA100); r0 = add(PC,#0xA10A); r2 = r16; memw(r29) = r17 }
	{ call logmsg_function }
	{ r1 = #0x38; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 0001FD94 }

l0001FD78:
	{ r0 = add(PC,#0x2E) }
	{ immext(#0xA100); r3 = add(PC,#0xA10D) }

l0001FD84:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001FD94:
	{ r1 = #0x39; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001FDB4 }

l0001FDA4:
	{ r0 = add(PC,#0x2) }
	{ immext(#0xA0C0); r3 = add(PC,#0xA0F0); jump 0001FD84 }

l0001FDB4:
	{ immext(#0xA080); r0 = add(PC,#0xA0AE); r1 = #0x3A; r2 = r16 }
	{ immext(#0xA0C0); r4 = add(PC,#0xA0E8); call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001FDD8
;;   Called from:
;;     0001FAD0 (in sub_f_execute)
;;     0001FC00 (in sub_f_execute)
;;     0001FD28 (in sub_f_execute)
;;     0001FD64 (in sub_f_check)
;;     0001FDC0 (in sub_f_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001FDF8 }

l0001FDE8:
	{ r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001FDF8:
	{ dealloc_return }

;; errlog_function: 0001FDFC
;;   Called from:
;;     0001FAF8 (in sub_f_execute)
;;     0001FD84 (in sub_f_check)
errlog_function proc
	{ r3 = #0x0; r4 = r3; allocframe(#0x8) }
	{ call logv; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; rank_execute: 0001FE20
;;   Called from:
;;     0001FE1C (in errlog_function)
rank_execute proc
	{ immext(#0xA1C0); r4 = add(PC,#0xA1C4); memd(r29-16) = r17:r16; allocframe(#0x18) }
	{ r17:r16 = combine(r1,r0); r1 = #0x32 }
	{ r2 = memw(r16+4); memd(r29+8) = r19:r18 }
	{ r7 = memw(r16+8) }
	{ r2 = r17; r5 = memw(r2+4) }
	{ r19 = memw(r7); r3 = memw(r5+16) }
	{ call logmsg_function; r18 = memw(r3); memw(r29) = r16 }
	{ r2 = memw(r19+20); if (cmp.gtu(r2.new,#0x3)) jump:t 0001FE6C }

l0001FE58:
	{ r3 = add(PC,#0x27); r1 = #0x35; r2 = r17 }
	{ call errlog_function }
	{ jump 0001FE9C; r0 = #0xFFFFFFFF }

l0001FE6C:
	{ immext(#0xA180); r4 = add(PC,#0xA19D); r1 = #0x3B; r3 = memw(r19+16) }
	{ r2 = r17; memw(r19+12) = #0x1; memw(r19) = #0x1 }
	{ memw(r19+8) = #0x1; memw(r19+24) = #0x4 }
	{ memw(r19+4) = #0xFFFFFF81; memw(r3) = r18 }
	{ call logmsg_function; memw(r29) = r16 }
	{ r0 = #0x0 }

l0001FE9C:
	{ r17:r16 = memd(r29+16); r19:r18 = memd(r29+8) }
	{ dealloc_return }

;; rank_check: 0001FEA4
rank_check proc
	{ immext(#0xA0C0); r4 = add(PC,#0xA0F4); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x41; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x42; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 0001FEE4 }

l0001FED0:
	{ r3 = add(PC,#0x22) }
	{ call errlog_function; r2 = r16 }

l0001FED8:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0001FEE4:
	{ r1 = #0x43; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0001FEFC }

l0001FEF4:
	{ r3 = add(PC,#0xD); jump 0001FED8 }

l0001FEFC:
	{ immext(#0xA0C0); r4 = add(PC,#0xA0D1); r1 = #0x44; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 0001FF1C
;;   Called from:
;;     0001FE44 (in rank_execute)
;;     0001FE90 (in rank_execute)
;;     0001FEB8 (in rank_check)
;;     0001FF0C (in rank_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0001FF40 }

l0001FF2C:
	{ r0 = add(PC,#0x16); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0001FF40:
	{ dealloc_return }

;; errlog_function: 0001FF44
;;   Called from:
;;     0001FE60 (in rank_execute)
;;     0001FED4 (in rank_check)
;;     0001FF34 (in logmsg_function)
errlog_function proc
	{ immext(#0xA000); r0 = add(PC,#0xA03A); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0001FF68                         00 00 00 00 00 00 00 00         ........

;; range_execute: 0001FF70
range_execute proc
	{ immext(#0xA100); r4 = add(PC,#0xA10E); memd(r29-16) = r17:r16; allocframe(#0x28) }
	{ r17:r16 = combine(r0,r1) }
	{ r1 = #0x38; r2 = memw(r17+4); memd(r29+8) = r23:r22 }
	{ r3 = memw(r17+8) }
	{ memd(r29+24) = r19:r18; memd(r29+16) = r21:r20 }
	{ r5 = memw(r2+8); r0 = memw(r2+4) }
	{ r2 = memw(r2); r22 = memw(r3) }
	{ r7 = memw(r0+16); r5 = memw(r5+16) }
	{ r2 = r16; r6 = memw(r2+16); r18 = memw(r22+16) }
	{ r19 = memw(r7); r21 = memw(r5) }
	{ call logmsg_function; r20 = memw(r6); memw(r29) = r17 }
	{ p0 = cmp.gt(r11,r12); if (p0.new) jump:nt 0001FFD0; if (!p0.new) r2 = #0x0; if (!p0.new) r3 = add(r20,#0x0) }

l0001FFBC:
	{ r3:r2 = combine(r20,#0x0) }

l0001FFC0:
	{ r2 = add(r2,#0x1); r3 = add(r3,r21); if (cmp.gtu(r19,r3.new)) jump:t 0001FFC0 }

l0001FFD0:
	{ p1 = cmp.gt(r12,r11); if (p1.new) jump:nt 0001FFE0 }

l0001FFD4:
	{ r2 = add(r2,#0x1); r3 = add(r3,r21); if (cmp.gt(r3.new,r19)) jump:t 0001FFD4 }

l0001FFE0:
	{ r4 = memw(r22+20) }

l0001FFE4:
	{ r3 = asl(r2,#0x2); if (!cmp.gtu(r3.new,r4)) jump:t 00020004 }

l0001FFF0:
	{ r3 = add(PC,#0x2A); r1 = #0x3F; r2 = r16 }
	{ call errlog_function }
	{ jump 00020050; r0 = #0xFFFFFFFF }

l00020004:
	{ memw(r22) = #0x1; memw(r22+4) = #0x1 }
	{ memw(r22+8) = #0x1; memw(r22+12) = r2 }
	{ if (!p0) jump:nt 00020030; memw(r22+24) = r3 }

l00020018:
	{ memw(r18++#4) = r20 }
	{ r20 = add(r20,r21); if (cmp.gtu(r19,r20.new)) jump:t 00020018 }

l00020028:
	{ r20 = add(r20,r21); memw(r18++#4) = r20 }

l00020030:
	{ p0 = cmp.gt(r12,r11); if (p0.new) jump:nt 00020028 }

l00020034:
	{ immext(#0xA040); r4 = add(PC,#0xA070); r1 = #0x49; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l00020050:
	{ r17:r16 = memd(r29+32); r19:r18 = memd(r29+24) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+8) }
	{ dealloc_return }

;; range_check: 0002005C
range_check proc
	{ immext(#0x9FC0); r4 = add(PC,#0x9FD5); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x4F; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x50; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 0002009C }

l00020088:
	{ r3 = add(PC,#0x4) }
	{ call errlog_function; r2 = r16 }

l00020090:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l0002009C:
	{ r1 = #0x51; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 000200B4 }

l000200AC:
	{ r3 = add(PC,#0x2F); jump 00020090 }

l000200B4:
	{ immext(#0x9F80); r4 = add(PC,#0x9FB3); r1 = #0x52; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 000200D4
;;   Called from:
;;     0001FFA8 (in range_execute)
;;     00020044 (in range_execute)
;;     00020070 (in range_check)
;;     000200C4 (in range_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 000200F8 }

l000200E4:
	{ r0 = add(PC,#0x36); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l000200F8:
	{ dealloc_return }

;; errlog_function: 000200FC
;;   Called from:
;;     0001FFF8 (in range_execute)
;;     0002008C (in range_check)
;;     000200EC (in logmsg_function)
errlog_function proc
	{ immext(#0x9F00); r0 = add(PC,#0x9F1A); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; transpose_execute: 00020120
transpose_execute proc
	{ immext(#0xA000); r2 = add(PC,#0xA008); allocframe(#0x90) }
	{ r3 = add(r29,#0x30); r17:r16 = combine(r0,r1); r5 = add(r29,#0x40); memd(r29+136) = r17:r16 }
	{ r13 = setbit(r5,#0x4); r14 = setbit(r3,#0x4); r12 = memw(r17+4); memd(r29+128) = r19:r18 }
	{ r1 = #0x42; memd(r29+120) = r21:r20 }
	{ memd(r29+112) = r23:r22 }
	{ r18 = memw(r12); memd(r29+96) = r27:r26 }
	{ r4 = memw(r17+8) }
	{ r9:r8 = memd(r2); memd(r29+104) = r25:r24 }
	{ r21 = memw(r18+12); r20 = memw(r18+8) }
	{ r23 = memw(r18+4); r22 = memw(r18) }
	{ r26 = mpyi(r21,r20); memw(r29+48) = r22; memw(r29+56) = r20 }
	{ r3:r2 = combine(#0x2,r16); r7:r6 = memd(r2+8); memw(r14) = r23 }
	{ immext(#0x9F80); r4 = add(PC,#0x9FB4); r24 = memw(r4); memw(r13) = r26 }
	{ r27 = mpyi(r26,r23); memb(r29+16) = r27.new }
	{ memd(r29+80) = r9:r8 }
	{ r19 = memw(r24+16); memw(r29+72) = r21 }
	{ memd(r29+88) = r7:r6 }
	{ memw(r29+60) = r21; memw(r5+12) = #0x1 }
	{ call logmsg_function; memw(r29) = r17 }
	{ immext(#0x9F80); r4 = add(PC,#0x9F98); r3:r2 = combine(#0x3,r16); memw(r29+12) = r21 }
	{ r1 = #0x47; memw(r29) = r22 }
	{ memw(r29+8) = r20; memw(r29+4) = r23 }
	{ call logmsg_function }
	{ r2 = memw(r25); if (!cmp.eq(r2.new,#0x1)) jump:t 000201F4 }

l000201E8:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 000201F8 }

l000201F0:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 00020214 }

l000201F4:
	{ immext(#0x9F40); r3 = add(PC,#0x9F7E); r1 = #0x4B }

l000201F8:
	{ r3 = add(PC,#0x3E); r1 = #0x4B }

l00020200:
	{ call errlog_function; r2 = r16 }

l00020204:
	{ r2 = r16 }
	{ jump 00020418; r0 = #0xFFFFFFFF }
00020210 C2 C0 92 91                                     ....            

l00020214:
	{ r3 = memw(r24+20); if (!cmp.gtu(r2,r3.new)) jump:t 0002022C }

l00020220:
	{ r3 = add(PC,#0x1F); jump 00020204; r1 = #0x4C }

l0002022C:
	{ r3 = memw(r25+24) }
	{ r4 = #0x14; if (cmp.gtu(r4.new,r3)) jump:t 00020248 }

l0002023C:
	{ r3 = add(PC,#0x11); jump 00020204; r1 = #0x4E }

l00020248:
	{ r5 = lsr(r3,#0x2); r1 = #0x52; if (!cmp.eq(r5.new,#0x1)) jump:t 00020298 }

l00020258:
	{ r4 = add(PC,#0xB); r3:r2 = combine(#0x3,r16) }
	{ call logmsg_function }
	{ r2 = memw(r18); r3 = memw(r18+4) }
	{ memw(r24+4) = r3; memw(r24) = r2 }
	{ r2 = memw(r18+8); r0 = memw(r24+16) }
	{ memw(r24+8) = r2 }
	{ r7 = memw(r18+12) }
	{ memw(r24+12) = r7 }
	{ r2 = memw(r18+24) }
	{ memw(r24+24) = r2 }
	{ call fn00009560; r1 = memw(r18+16); r2 = memw(r18+24) }
	{ jump 00020414 }

l00020298:
	{ p0 = cmp.eq(r5,#0x0); if (!p0.new) jump:nt 000202B0; r13:r12 = combine(r21,#0x1); if (!p0.new) r4 = memw(r25+16) }

l000202A4:
	{ r8 = #0x3; r5 = #0x0; r7:r6 = combine(#0x1,#0x2) }
	{ jump 00020320 }

l000202B0:
	{ r2 = sub(#0x4,r5); p0 = cmp.gtu(r5,#0x1); r3 = add(r29,#0x50); r6 = r5 }
	{ loop0(000202CC,r6); r3 = addasl(r3,r2,#0x2) }
	{ if (!p0) jump:nt 000202D8; r5 = memw(r4++#4) }

l000202CC:
	{ r5 = add(r5,r2) }
	{ r5 = memw(r4++#4); memw(r3++#4) = r5 }

l000202D8:
	{ r2 = add(r5,r2); r9 = add(r29,#0x30) }
	{ r2 = add(r29,#0x50); memw(r3++#4) = r2 }
	{ r4 = setbit(r2,#0x4); r3 = add(r29,#0x40); r5 = memd(r29+80); r6 = memd(r29+88) }
	{ r8 = memw(r29+92); r2 = memw(r18+24) }
	{ r7 = memw(r4); r13 = memw(r30+r6<<#2) }
	{ r27 = memw(r22+r5<<#2); r22 = memw(r30+r5<<#2) }
	{ r20 = memw(r22+r6<<#2); r21 = memw(r30+r8<<#2) }
	{ r12 = memw(r22+r8<<#2); r23 = memw(r30+r7<<#2) }
	{ r26 = memw(r30+r7<<#2) }

l00020320:
	{ immext(#0x9E80); r4 = add(PC,#0x9EAD); memw(r24+24) = r2; memw(r24+12) = r21 }
	{ r3:r2 = combine(#0x3,r16); r1 = #0x89; memw(r24+8) = r20; memw(r24+4) = r23 }
	{ r25:r24 = combine(r13,r12); memw(r24) = r22; memw(r29+44) = r21 }
	{ memw(r29+40) = r20; memw(r29+36) = r23 }
	{ memw(r29+32) = r22; memw(r29+8) = r6 }
	{ memw(r29+28) = r12; memw(r29+24) = r13 }
	{ memw(r29+20) = r26; memw(r29+16) = r27 }
	{ memw(r29+12) = r8; memw(r29+4) = r7 }
	{ call logmsg_function; memw(r29) = r5 }
	{ r1:r0 = vaslw(r25:r24,#0x2); p0 = cmp.gt(r14,#0x0); if (!p0.new) jump:nt 000203FC; r3:r2 = combine(#0x0,#0x0) }

l00020380:
	{ r5:r4 = vaslw(r27:r26,#0x2) }

l00020384:
	{ p0 = cmp.gt(r15,#0x0); if (!p0.new) jump:nt 000203F0; r7:r6 = combine(#0x0,r2) }

l00020388:
	{ r7:r6 = combine(#0x0,r2) }

l0002038C:
	{ loop1(00020398,r20); p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 000203E4; r8 = r6 }

l00020398:
	{ p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 000203D8; r12 = r19; r9 = r8 }

l000203A4:
	{ loop0(000203C0,r21) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r13 = memw(r18+16) }
	{ r9 = add(r9,r0); r13 = memw(r28+r9) }
	{ nop; memw(r12++#4) = r13 }
	{ r19 = addasl(r19,r21,#0x2) }

l000203D8:
	{ r8 = add(r8,r1); nop; nop }

l000203E4:
	{ r6 = add(r6,r4); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r23)) jump:t 0002038C }

l000203F0:
	{ r2 = add(r2,r5); r3 = add(r3,#0x1); if (!cmp.eq(r3.new,r22)) jump:t 00020384 }

l000203F4:
	{ r3 = add(r3,#0x1); if (!cmp.eq(r3.new,r22)) jump:t 00020388 }

l000203FC:
	{ immext(#0x9E00); r4 = add(PC,#0x9E0F); r3:r2 = combine(#0x2,r16); r1 = #0x96 }

l00020400:
	{ r4 = add(PC,#0xF); r3:r2 = combine(#0x2,r16); r1 = #0x96 }
	{ call logmsg_function; memw(r29) = r17 }

l00020414:
	{ r0 = #0x0 }

l00020418:
	{ r17:r16 = memd(r29+136); r19:r18 = memd(r29+128) }
	{ r21:r20 = memd(r29+120); r23:r22 = memd(r29+112) }
	{ r25:r24 = memd(r29+104); r27:r26 = memd(r29+96) }
	{ dealloc_return }

;; transpose_check: 0002042C
transpose_check proc
	{ immext(#0x9C80); r4 = add(PC,#0x9CA5); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x9C; r16 = r1; r17 = r0 }
	{ call logmsg_function; r3:r2 = combine(#0x2,r16); memw(r29) = r17 }
	{ r1 = #0x9D; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x3)) jump:t 00020470 }

l0002045C:
	{ r3 = add(PC,#0x14) }
	{ call errlog_function; r2 = r16 }

l00020464:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00020470:
	{ r1 = #0x9F; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 0002048C }

l00020480:
	{ r3 = add(PC,#0x3F); jump 00020464; r1 = #0x9E }

l0002048C:
	{ immext(#0x9C40); r4 = add(PC,#0x9C7F); r3:r2 = combine(#0x2,r16); memw(r29) = r17 }
	{ call logmsg_function }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 000204A8
;;   Called from:
;;     000201B4 (in transpose_execute)
;;     000201D8 (in transpose_execute)
;;     00020260 (in transpose_execute)
;;     0002036C (in transpose_execute)
;;     0002040C (in transpose_execute)
;;     0002040C (in transpose_execute)
;;     00020440 (in transpose_check)
;;     0002049C (in transpose_check)
logmsg_function proc
	{ allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 000204C8 }

l000204B8:
	{ r0 = add(PC,#0x3E); r5 = add(r29,#0x10); r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l000204C8:
	{ dealloc_return }

;; errlog_function: 000204CC
;;   Called from:
;;     00020200 (in transpose_execute)
;;     00020460 (in transpose_check)
errlog_function proc
	{ immext(#0x9BC0); r0 = add(PC,#0x9BE6); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; addn_execute: 000204F0
addn_execute proc
	{ immext(#0x9D80); r4 = add(PC,#0x9D94); memd(r29-16) = r17:r16; allocframe(#0x30) }
	{ r17:r16 = combine(r0,r1) }
	{ r1 = #0x39; r18 = memw(r17+4); memd(r29+32) = r19:r18 }
	{ r2 = r16; r3 = memw(r17+8) }
	{ memd(r29+24) = r21:r20; memd(r29+16) = r23:r22 }
	{ r20 = memw(r18); r21 = memw(r3) }
	{ memd(r29+8) = r25:r24 }
	{ r23 = memw(r20+8); r22 = memw(r20+12) }
	{ r24 = memw(r20+4); r25 = memw(r20) }
	{ call logmsg_function; r19 = memw(r21+16); memw(r29) = r17 }
	{ r2 = memw(r17+16) }
	{ r3 = #0x2; if (cmp.gtu(r3.new,r2)) jump:t 000205A0 }

l0002053C:
	{ r4 = add(r18,#0x4) }

l00020540:
	{ r4 = add(r4,#0x4); r5 = add(r5,#0x1); if (!cmp.gtu(r2,r5.new)) jump:nt 000205A0 }

l00020550:
	{ r3 = add(PC,#0xF); r6 = memw(r4) }
	{ r7 = memw(r6); if (cmp.eq(r7.new,r25)) jump:t 00020564 }

l00020564:
	{ immext(#0x9D00); r3 = add(PC,#0x9D37); r7 = memw(r6+4); if (cmp.eq(r7.new,r24)) jump:t 00020578 }

l00020578:
	{ immext(#0x9D00); r3 = add(PC,#0x9D23); r7 = memw(r6+8); if (cmp.eq(r7.new,r23)) jump:t 0002058C }

l0002058C:
	{ immext(#0x9D00); r3 = add(PC,#0x9D0F); r6 = memw(r6+12); if (cmp.eq(r6.new,r22)) jump:t 00020540 }

l000205A0:
	{ immext(#0x9D00); r3 = add(PC,#0x9D0A); r2 = mpyi(r24,r25); r5 = memw(r21+20) }
	{ r2 = mpyi(r2,r23); r1 = #0x40 }
	{ r4 = mpyi(r2,r22) }
	{ r2 = asl(r4,#0x2); if (!cmp.gtu(r2.new,r5)) jump:t 000205D4 }

l000205C8:
	{ r2 = r16 }
	{ jump 00020654; r0 = #0xFFFFFFFF }

l000205D4:
	{ p0 = cmp.eq(r4,#0x0); r5 = memw(r20+4); r3 = memw(r20) }
	{ if (!p0) r3 = #0x0; memw(r21+4) = r5; memw(r21) = r3 }
	{ r6 = memw(r20+8) }
	{ memw(r21+8) = r6 }
	{ r7 = memw(r20+12) }
	{ if (p0) jump:nt 00020638; memw(r21+12) = r7; memw(r21+24) = r2 }

l000205F8:
	{ loop1(00020604,r4); immext(#0x0); r2 = #0x0 }
	{ r4 = r2; r6 = memw(r17+16) }
	{ p0 = cmp.eq(r6,#0x0); if (p0.new) jump:nt 0002062C; if (!p0.new) r5:r4 = combine(r18,r2) }

l00020610:
	{ loop0(00020614,r6) }
	{ r6 = memw(r5++#4) }
	{ r6 = memw(r6+16) }
	{ r6 = addasl(r6,r3,#0x2) }
	{ r6 = memw(r6) }
	{ r4 = sfadd(r4,r6); nop }

l0002062C:
	{ r19 = add(r19,#0x4); nop; r3 = r3; memw(r19) = r4 }

l00020638:
	{ immext(#0x9C80); r4 = add(PC,#0x9C80); r1 = #0x4C; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l00020654:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }
	{ r21:r20 = memd(r29+24); r23:r22 = memd(r29+16) }
	{ r25:r24 = memd(r29+8); dealloc_return }

;; addn_check: 00020664
addn_check proc
	{ immext(#0x9BC0); r4 = add(PC,#0x9BD5); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x52; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x53; r2 = memw(r17+16); if (cmp.gtu(r2.new,#0x1)) jump:t 000206A4 }

l00020690:
	{ r3 = add(PC,#0x3) }
	{ call errlog_function; r2 = r16 }

l00020698:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l000206A4:
	{ r1 = #0x54; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 000206BC }

l000206B4:
	{ r3 = add(PC,#0x2E); jump 00020698 }

l000206BC:
	{ immext(#0x9B80); r4 = add(PC,#0x9BB2); r1 = #0x55; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 000206DC
;;   Called from:
;;     00020524 (in addn_execute)
;;     00020648 (in addn_execute)
;;     00020678 (in addn_check)
;;     000206CC (in addn_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00020700 }

l000206EC:
	{ r0 = add(PC,#0x35); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00020700:
	{ dealloc_return }

;; errlog_function: 00020704
;;   Called from:
;;     00020694 (in addn_check)
;;     000206F4 (in logmsg_function)
errlog_function proc
	{ immext(#0x9B00); r0 = add(PC,#0x9B19); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
00020728                         00 00 00 00 00 00 00 00         ........
00020730 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; execute_qinstancenorm_ref: 00020740
execute_qinstancenorm_ref proc
	{ allocframe(#0x70) }
	{ r2 = memw(r0+4); r3 = memw(r0+8) }
	{ memd(r29+96) = r19:r18; memd(r29+72) = r25:r24 }
	{ r4 = memw(r2) }
	{ r17 = r1; memd(r29+64) = r27:r26; memd(r29+104) = r17:r16 }
	{ r25 = memw(r4+8); memd(r29+88) = r21:r20 }
	{ r19 = memw(r4); r24 = memw(r4+4) }
	{ r5 = mpyi(r25,r19); r27 = memw(r4+12) }
	{ r2 += mpyi(r27,#0x14); r16 = memw(r17+4); memd(r29+80) = r23:r22 }
	{ r1:r0 = combine(#0x0,r16); r18 = memw(r3); r21 = memw(r3+8) }
	{ r3 = mpyi(r5,r24); r22 = memw(r3+4); memw(r29+52) = r25 }
	{ r26 = memw(r4+16); r23 = memw(r18+16) }
	{ r20 = mpyi(r3,r27); call fn000095F0 }
	{ r1 = #0x71; r2 = memw(r18+20); if (!cmp.gtu(r20,r2.new)) jump:t 000207C8 }

l000207B4:
	{ r3 = add(PC,#0x16) }

l000207B8:
	{ call errlog_function; r2 = r17 }
	{ jump 00020AA8; r0 = #0xFFFFFFFF }

l000207C8:
	{ p0 = cmp.eq(r11,#0x1); if (p0.new) jump:nt 000207E0; if (!p0.new) r1 = #0x72; memw(r29+40) = r26 }

l000207D4:
	{ immext(#0x9B40); r3 = add(PC,#0x9B40); jump 000207B8 }

l000207E0:
	{ r2 = r17; memw(r18+12) = r27; memw(r18+8) = r24 }
	{ memw(r18+4) = r25 }
	{ memw(r18) = #0x1; memw(r29+12) = r27 }
	{ memw(r29+8) = r24; memw(r29+4) = r25 }
	{ call logmsg_function; memw(r29) = #0x1 }
	{ p0 = cmp.gt(r24,#0x0); r12 = r27; memw(r18+24) = r20 }
	{ r0 = p0; if (!p0) jump:nt 0002087C; memb(r29+12) = r0.new }

l00020824:
	{ r4 = #0x0; r2 = memd(r29+40) }

l00020828:
	{ loop1(00020834,r25); if (!p0.new) jump:nt 00020874; p0 = cmp.gt(r25,#0x0) }

l00020834:
	{ if (!p0.new) jump:nt 00020868; r5 = r3; p0 = cmp.gt(r12,#0x0); if (!p0) r7:r6 = combine(r2,r16) }

l00020844:
	{ loop0(00020848,r12) }
	{ r8 = memb(r7++#1); r9 = memw(r6) }
	{ r9 = add(r9,r8) }
	{ memw(r6++#4) = r9 }
	{ r13 = memw(r5) }
	{ r8 = add(r13,mpyi(r8,r8)); memw(r5++#4) = r8.new }

l00020868:
	{ nop; nop; nop }

l00020874:
	{ r4 = add(r4,#0x1); if (!cmp.eq(r4.new,r24)) jump:t 00020828 }

l0002087C:
	{ r2 = mpyi(r24,r25); p0 = cmp.gt(r12,#0x0); r24 = r12; memw(r29+36) = r24 }

l00020880:
	{ p0 = cmp.gt(r12,#0x0); r24 = r12; memw(r29+36) = r24 }
	{ r0 = p0; if (p0) r19 = add(r24,#0x0); memw(r29+32) = r23; memw(r29+24) = r21 }
	{ immext(#0x3727C580); if (p0) r26 = #0x3727C5AC; memw(r29+20) = r22; memw(r29+28) = r2 }
	{ if (!p0) jump:nt 00020920; immext(#0x3F800000); if (p0) r18 = #0x3F800000; memw(r29+56) = r0 }

l000208B4:
	{ r21 += mpyi(r24,#0xC); r22 = asl(r24,#0x3); r20 = r16; r2 = memd(r29+28) }
	{ r27 = addasl(r16,r24,#0x2); r17 = convert_w2sf(r2) }

l000208C8:
	{ r1 = r17; r2 = memw(r20) }
	{ r0 = convert_uw2sf(r2); call fn00009610 }
	{ r2 = add(r20,r22); r1 = r17; r23 = add(r20,r21) }
	{ r2 = togglebit(r0,#0x1E); memw(r2) = r0 }
	{ r3 = memw(r27++#4); r4 = memw(r20) }
	{ r0 = convert_uw2sf(r3); r3 = convert_uw2sf(r4) }
	{ r0 += sfmpy(r2,r3); call fn00009610 }
	{ r2 = sfadd(r0,r26) }
	{ call fn00009800; r0 = r2; memw(r23) = r0 }
	{ call fn00009610; r1:r0 = combine(r0,r18); r20 = add(r20,#0x4); r19 = r19 }
	{ p0 = cmp.eq(r11,#0x0); if (!p0.new) jump:nt 000208C8; memw(r23) = r0 }

l00020920:
	{ r4 = r24; r0 = memw(r29+48) }
	{ r27 = addasl(r16,r4,#0x4); p0 = r0 }
	{ if (p0) jump:nt 00020948; immext(#0x0); if (p0) r18 = #0x0 }

l0002093C:
	{ immext(#0x0); r18 = #0x0 }
	{ r9 = r10; jump 00020A04 }

l00020948:
	{ r2 += mpyi(r4,#0x3); r3 = asl(r4,#0x1) }
	{ r3 = addasl(r16,r3,#0x2); r5 = #0x0; r17 = r18; memb(r29+12) = r3.new }
	{ r3 = memw(r29+40) }
	{ memw(r29+44) = r2 }

l0002096C:
	{ p0 = cmp.gt(r25,#0x0); r21 = #0x0; memw(r29+40) = r5 }
	{ if (!p0) jump:nt 000209F8 }

l00020978:
	{ r0 = memw(r29+56) }
	{ p0 = r0; if (!p0.new) jump:nt 000209F0; if (p0.new) r26 = add(r3,#0x0); if (!p0) r25:r24 = combine(r3,r4) }

l0002098C:
	{ r23 = addasl(r27,r4,#0x2); r22 = r4; r20 = memd(r29+44); r19 = memd(r29+48) }

l00020998:
	{ r2 = memb(r25++#1); r3 = memw(r19) }
	{ r2 = convert_w2sf(r2); r4 = memw(r20) }
	{ r2 = sfsub(r2,r3) }
	{ r16 = sfmpy(r4,r2); r27 = add(r27,#0x4); memb(r27) = r16.new }
	{ r1:r0 = combine(r18,r16) }
	{ call fn00009600; r18 = r0; r1:r0 = combine(r17,r16) }
	{ r19 = add(r19,#0x4); r20 = add(r20,#0x4); r17 = r0 }
	{ r22 = add(r22,#0xFFFFFFFF); if (!cmp.eq(r22.new,#0x0)) jump:t 00020998 }

l000209E0:
	{ r4 = r24; r27 = r23; r25 = memw(r29+52) }
	{ r3 = add(r3,r4) }

l000209F0:
	{ r21 = add(r21,#0x1); if (!cmp.eq(r21.new,r25)) jump:t 00020978 }

l000209F8:
	{ r5 = memd(r29+40); r2 = memd(r29+36) }

l000209FC:
	{ r5 = add(r5,#0x1); if (!cmp.eq(r5.new,r2)) jump:t 0002096C }

l00020A04:
	{ r2 = memw(r29+28) }

l00020A08:
	{ r19 = mpyi(r2,r4); if (!cmp.gt(r19.new,#0x0)) jump:nt 00020A74 }

l00020A14:
	{ immext(#0x38D1B700); r0 = #0x38D1B717 }
	{ r27 -= asl(r19,#0x2); call fn00009600 }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2) }
	{ r20 = #0x0; r16 = r0 }

l00020A38:
	{ r27 = add(r27,#0x4); r2 = memw(r27) }
	{ r2 = sfsub(r2,r18) }
	{ r0 = sfmpy(r2,r16); call fn00009620 }
	{ r2 = convert_uw2sf(r0):chop; r19 = r19; r3 = memd(r29+32) }
	{ p1 = cmp.eq(r19,#0x0) }
	{ p2 = cmp.gt(r2,#0xFF); p0 = cmp.gt(r2,#0xFFFFFFFF); if (p2.new) r2 = #0xFFFFFFFF }
	{ if (!p0) r2 = add(r20,#0x0) }
	{ memb(r3++#1) = r2 }
	{ if (!p1) jump:nt 00020A38; memw(r29+32) = r3 }

l00020A74:
	{ r0 = #0x0; r4 = memd(r29+20); r3 = memd(r29+24) }
	{ memw(r4+12) = #0xFFFFFF81 }
	{ memw(r4+8) = #0x1; memw(r4+4) = #0x1 }
	{ memw(r4) = #0x1; memw(r3+8) = #0x1 }
	{ memw(r3) = #0x1; memw(r3+12) = #0x1 }
	{ memw(r3+4) = #0xFFFFFF81 }
	{ r2 = memw(r4+16) }
	{ memw(r2) = r18 }
	{ r7 = memw(r3+16) }
	{ memw(r7) = r17; memw(r4+24) = #0x4 }
	{ memw(r3+24) = #0x4 }

l00020AA8:
	{ r17:r16 = memd(r29+104); r19:r18 = memd(r29+96) }
	{ r21:r20 = memd(r29+88); r23:r22 = memd(r29+80) }
	{ r25:r24 = memd(r29+72); r27:r26 = memd(r29+64) }
	{ dealloc_return }

;; check_qinstancenorm: 00020ABC
check_qinstancenorm proc
	{ r2 = r1; allocframe(#0x0) }
	{ r1 = #0xFC; r3 = memw(r0+16); if (cmp.eq(r3.new,#0x3)) jump:t 00020AD8 }

l00020AD0:
	{ r3 = add(PC,#0x1B); jump 00020AF0 }

l00020AD8:
	{ r1 = #0xFD; r0 = #0x0; r3 = memw(r0+20) }
	{ p0 = cmp.eq(r3,#0x3); if (p0.new) dealloc_return }
00020AE4             60 42 00 00 03 C9 49 6A 6C C1 00 5A     `B....Ijl..Z

l00020AF0:
	{ r0 = #0xFFFFFFFF }
	{ dealloc_return }

;; execute_finstancenorm: 00020AF8
execute_finstancenorm proc
	{ r7 = r1; allocframe(#0x68) }
	{ r2 = memw(r0+4); r3 = memw(r0+8) }
	{ memd(r29+80) = r21:r20; memd(r29+72) = r23:r22 }
	{ r2 = memw(r2) }
	{ r6 = memw(r3); memd(r29+64) = r25:r24 }
	{ r5 = memw(r2); memd(r29+96) = r17:r16 }
	{ p0 = cmp.gt(r5,#0x0); r21 = memw(r2+8); r23 = memw(r2+4) }
	{ r4 = mpyi(r5,r21); r24 = memw(r2+12) }
	{ r4 = mpyi(r4,r23); memd(r29+88) = r19:r18; memd(r29+56) = r27:r26 }
	{ memw(r29+28) = r5; memw(r29+20) = r23 }
	{ memw(r29+24) = r7 }
	{ r0 = mpyi(r4,r24); r4 = memw(r6+20) }
	{ r3 = asl(r0,#0x2); if (!cmp.gtu(r3.new,r4)) jump:t 00020B74 }

l00020B4C:
	{ r3 = add(PC,#0x3E); r1 = #0xD6; r2 = r7 }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF }

l00020B60:
	{ r17:r16 = memd(r29+96); r19:r18 = memd(r29+88) }
	{ r21:r20 = memd(r29+80); r23:r22 = memd(r29+72) }
	{ r25:r24 = memd(r29+64); r27:r26 = memd(r29+56) }
	{ dealloc_return }

l00020B74:
	{ r0 = #0x0; r16 = memw(r7+4); r2 = memw(r2+16) }
	{ memw(r29+16) = r2; memw(r6+8) = r23 }
	{ memw(r6+24) = r3; memw(r6) = r5 }
	{ memw(r6+4) = r21; memw(r6+12) = r24 }
	{ if (!p0) jump:nt 00020B60 }

l00020B94:
	{ r2 = mpyi(r23,r21); r3 += mpyi(r24,#0x3) }
	{ r5 += mpyi(r24,#0xC); r4 = asl(r24,#0x1); r18 = #0x0; memw(r29) = r6 }
	{ r6 = mpyi(r2,r24); r2 = convert_w2sf(r2); memw(r29+44) = r5 }
	{ r17 = addasl(r16,r4,#0x2); r27 = asl(r24,#0x2); memw(r29+4) = r2; memw(r29+8) = r6 }
	{ r22 = addasl(r16,r3,#0x2); r5 = asl(r24,#0x3); r19 = memw(r29+4) }
	{ r1 += mpyi(r24,#0x14); memw(r29+48) = r5 }
	{ memw(r29+12) = r1 }

l00020BD8:
	{ p0 = cmp.gt(r23,#0x0); r1 = #0x0; r0 = memw(r7+4) }
	{ r3 = p0; r2 = memd(r29+12); memw(r29+36) = r18 }
	{ call fn000095F0; memw(r29+40) = r3 }
	{ r2 = memw(r29+8) }
	{ r3 = mpyi(r2,r18); r4 = memd(r29+16); r0 = memd(r29+40) }
	{ p0 = r0 }
	{ r18 = addasl(r4,r3,#0x2) }
	{ if (p0) jump:nt 00020D30; r3:r2 = combine(r18,#0x0) }

l00020C0C:
	{ p0 = cmp.gt(r24,#0x0); r25 = r24; r26 = r16 }
	{ immext(#0x3727C580); r23 = #0x3727C5AC }
	{ r0 = p0; if (!p0) jump:nt 00020C90; memb(r29+8) = r0.new }

l00020C2C:
	{ call fn00009610; r1 = r19; r0 = memw(r26) }

l00020C30:
	{ r1 = r19; r0 = memw(r26) }

l00020C38:
	{ r3 = add(r26,r27); r1 = r19; r20 = r0; r2 = memd(r29+48) }
	{ r2 = add(r26,r2) }
	{ memw(r2) = r20 }
	{ call fn00009610; r0 = memw(r3) }
	{ r2 = togglebit(r20,#0x1E); r7 = memw(r29+44) }
	{ r0 += sfmpy(r2,r20); r20 = add(r26,r7) }
	{ r0 = sfadd(r0,r23); memw(r20) = r0 }
	{ call fn00009800 }
	{ call fn00009610; immext(#0x3F800000); r0 = #0x3F800000; r1 = r0 }
	{ r25 = add(r25,#0xFFFFFFFF); r26 = add(r26,#0x4); memw(r20) = r0 }
	{ if (!p0.new) jump:nt 00020C2C; p0 = cmp.eq(r25,#0x0) }

l00020C90:
	{ r0 = memw(r29+32); r14 = memw(r29+28) }
	{ r23 = memw(r29+20) }
	{ p1 = r0; r0 = memw(r29+40) }
	{ p0 = r0; if (!p0.new) jump:nt 00020D20; if (p0.new) r2 = memw(r29) }

l00020CB0:
	{ r2 = #0x0; r3 = memw(r2+16) }

l00020CB4:
	{ p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 00020D18; r4 = #0x0 }

l00020CB8:
	{ r4 = #0x0 }

l00020CBC:
	{ if (!p1) jump:nt 00020D10; if (p1) r8 = add(r22,#0x0); if (p1) r7:r6 = combine(r18,r17) }

l00020CC8:
	{ loop0(00020CE0,r24); r5 = addasl(r3,r24,#0x2) }
	{ nop; nop; nop; nop }
	{ r6 = add(r6,#0x4); r7 = add(r7,#0x4); r9 = memw(r7); r12 = memw(r6) }
	{ r9 = sfsub(r9,r12); r8 = add(r8,#0x4); r13 = memw(r8) }
	{ r9 = sfmpy(r9,r13) }
	{ r3 = add(r3,#0x4); memw(r3) = r9 }
	{ r18 = addasl(r18,r24,#0x2); r3 = r5 }

l00020D10:
	{ r4 = add(r4,#0x1); if (!cmp.eq(r4.new,r21)) jump:t 00020CBC }

l00020D18:
	{ r2 = add(r2,#0x1); if (!cmp.eq(r2.new,r23)) jump:t 00020CB4 }

l00020D1C:
	{ if (!cmp.eq(r2.new,r23)) jump:t 00020CB8 }

l00020D20:
	{ r18 = memd(r29+36); r7 = memd(r29+24) }

l00020D24:
	{ r18 = add(r18,#0x1); if (!cmp.eq(r18.new,r14)) jump:t 00020BD8 }

l00020D30:
	{ loop1(00020D38,r21); p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 00020D7C }

l00020D38:
	{ if (!p0.new) jump:nt 00020D70; p0 = cmp.gt(r24,#0x0); if (!p0) r5:r4 = combine(r16,r3) }

l00020D44:
	{ loop0(00020D48,r24) }
	{ r8 = add(r5,r27); r4 = add(r4,#0x4); r6 = memw(r4); r7 = memw(r5) }
	{ r7 = sfadd(r6,r7) }
	{ r5 = add(r5,#0x4); memw(r5) = r7 }
	{ r1 = memw(r8) }
	{ r1 += sfmpy(r6,r6) }
	{ nop; memw(r8) = r1 }
	{ r3 = addasl(r3,r24,#0x2) }

l00020D70:
	{ nop; nop; nop }

l00020D7C:
	{ r2 = add(r2,#0x1); if (cmp.eq(r2.new,r23)) jump:nt 00020C0C }

;; check_finstancenorm: 00020D88
;;   Called from:
;;     00020D7C (in execute_finstancenorm)
check_finstancenorm proc
	{ r2 = r1; allocframe(#0x0) }
	{ r1 = #0x103; r3 = memw(r0+16); if (cmp.eq(r3.new,#0x1)) jump:t 00020DA4 }

l00020D9C:
	{ r3 = add(PC,#0xF); jump 00020DBC }

l00020DA4:
	{ r1 = #0x104; r0 = #0x0; r3 = memw(r0+20) }
	{ p0 = cmp.eq(r3,#0x1); if (p0.new) dealloc_return }
00020DB0 55 42 00 00 03 C3 49 6A 06 C0 00 5A             UB....Ij...Z    

l00020DBC:
	{ r0 = #0xFFFFFFFF }
	{ dealloc_return }

;; errlog_function: 00020DC4
;;   Called from:
;;     000207B8 (in execute_qinstancenorm_ref)
;;     00020B58 (in execute_finstancenorm)
errlog_function proc
	{ immext(#0x9500); r0 = add(PC,#0x9501); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }

;; logmsg_function: 00020DE8
;;   Called from:
;;     00020800 (in execute_qinstancenorm_ref)
logmsg_function proc
	{ r4 = #0x2; allocframe(#0x8) }
	{ r3 = memw(r2+16); if (cmp.gtu(r4,r3.new)) jump:t 00020E18 }

l00020DF8:
	{ r0 = add(PC,#0x11); r3 = #0x2; r5 = add(r29,#0x10) }
	{ immext(#0x9500); r4 = add(PC,#0x9534); r1 = #0x74; r6 = add(r29,#0x10) }
	{ call logv; memw(r29+4) = r6 }

l00020E18:
	{ dealloc_return }
00020E1C                                     00 00 00 00             ....

;; sub_int32_execute: 00020E20
sub_int32_execute proc
	{ r5 = r0; allocframe(#0xA0) }
	{ r2 = memw(r5+4); r3 = memw(r5+8) }
	{ memd(r29+112) = r27:r26; memd(r29+144) = r19:r18 }
	{ r26 = memw(r2); r19 = memw(r2+4) }
	{ r22 = #0x0; memd(r29+136) = r21:r20; memd(r29+128) = r23:r22 }
	{ r6 = memw(r26+4); r0 = memw(r26) }
	{ p0 = cmp.eq(r6,#0x1); p1 = cmp.eq(r0,#0x1); r8 = memw(r19); r4 = memw(r19+4) }
	{ r10 = p1; r12 = memw(r26+8) }
	{ r18 = mux(p0,r4,r6); p2 = cmp.eq(r12,#0x1); r9 = memw(r19+8); memw(r29+92) = r4 }
	{ r4 = mux(p1,r8,r0); r7 = memw(r26+12) }
	{ r2 = mpyi(r4,r18); p1 = cmp.eq(r7,#0x1); r0 = memw(r19+12) }
	{ r20 = mux(p2,r9,r12); memd(r29+120) = r25:r24; memw(r29+76) = r6 }
	{ r2 = mpyi(r2,r20); r21 = mux(p1,r0,r7) }
	{ r2 = mpyi(r2,r21); r6 = r1; r25 = memw(r3); memd(r29+152) = r17:r16 }
	{ r10 = p2; memw(r29+96) = r10; memw(r29+72) = r8 }
	{ memw(r29+68) = r9; memw(r29+84) = r10 }
	{ r16 = asl(r2,#0x2); memw(r29+104) = r4; memw(r29+80) = r0 }
	{ if (p0) jump:nt 00020ECC }

l00020EC8:
	{ r22 = mpyi(r7,r12) }

l00020ECC:
	{ immext(#0x94C0); r0 = add(PC,#0x94DD); r1 = #0xBD; r2 = r6 }
	{ immext(#0x94C0); r4 = add(PC,#0x94EC); r3 = memw(r19+16); memw(r29+64) = r12 }
	{ r24 = r6; r27 = r5; r23 = memw(r25+16); memw(r29+88) = r7 }
	{ r17 = memw(r26+16) }
	{ call logmsg_function; memw(r29+100) = r3; memw(r29) = r5 }
	{ r1 = #0xBD; r2 = memw(r25+20); if (!cmp.gtu(r16,r2.new)) jump:t 00020F34 }

l00020F18:
	{ r0 = add(PC,#0x15); r2 = r24 }
	{ immext(#0x94C0); r3 = add(PC,#0x94C6) }

l00020F28:
	{ call errlog_function }
	{ jump 00021160; r0 = #0xFFFFFFFF }

l00020F34:
	{ r2 = r24; r13 = memw(r19); r5 = memw(r26) }
	{ p0 = cmp.eq(r5,r13); r8 = memw(r26+12); r7 = memw(r26+8) }
	{ r6 = memw(r26+4); r12 = memw(r19+12) }
	{ r9 = memw(r19+8); r3 = memw(r19+4) }
	{ memw(r29+48) = r25; memw(r29+52) = r16 }
	{ memw(r29+56) = r17; memw(r29+60) = r27 }
	{ if (p0) jump:nt 00020F7C }

l00020F70:
	{ p0 = cmp.eq(r5,#0x1); if (p0.new) jump:nt 00020F7C }

l00020F74:
	{ if (!p0.new) jump:nt 00020FB8; p0 = cmp.eq(r13,#0x1) }

l00020F7C:
	{ p0 = cmp.eq(r6,r3); if (p0.new) jump:nt 00020F8C }

l00020F80:
	{ p0 = cmp.eq(r6,#0x1); if (p0.new) jump:nt 00020F8C }

l00020F84:
	{ p0 = cmp.eq(r3,#0x1); if (!p0.new) jump:nt 00020FB8; nop }

l00020F8C:
	{ if (p0.new) jump:nt 00020FA0; p0 = cmp.eq(r7,r9) }

l00020F94:
	{ p0 = cmp.eq(r7,#0x1); if (p0.new) jump:nt 00020FA0 }

l00020F98:
	{ if (!p0.new) jump:nt 00020FB8; p0 = cmp.eq(r9,#0x1) }

l00020FA0:
	{ if (p0.new) jump:nt 00020FEC; p0 = cmp.eq(r8,r12) }

l00020FA8:
	{ if (p0.new) jump:nt 00020FEC; p0 = cmp.eq(r8,#0x1) }

l00020FB0:
	{ if (p0.new) jump:nt 00020FEC; p0 = cmp.eq(r12,#0x1) }

l00020FB8:
	{ immext(#0x93C0); r0 = add(PC,#0x93F1); r1 = #0xBD; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r13 }
	{ immext(#0x9400); r3 = add(PC,#0x9424); memw(r29+20) = r3; memw(r29+8) = r7 }
	{ memw(r29+12) = r8; memw(r29+4) = r6 }
	{ jump 00020F28; memw(r29) = r5 }

l00020FEC:
	{ r19 = r2; r24 = memw(r29+104); memw(r29+44) = r21 }
	{ immext(#0x9380); r0 = add(PC,#0x93B1); memw(r29+36) = r18; memw(r29+20) = r3 }
	{ immext(#0x9400); r4 = add(PC,#0x9420); r1 = #0xBD; memw(r29+40) = r20 }
	{ memw(r29+32) = r24; memw(r29+28) = r12 }
	{ memw(r29+24) = r9; memw(r29+16) = r13 }
	{ memw(r29+12) = r8; memw(r29+8) = r7 }
	{ memw(r29+4) = r6; memw(r29) = r5 }
	{ call logmsg_function }
	{ p0 = cmp.gt(r24,#0x0); if (p0.new) r14 = #0x0; r3 = memw(r29+48); r26 = memw(r29+100) }
	{ r25 = memw(r29+56) }
	{ r2 = memw(r29+52); memb(r3+6) = r2.new }
	{ memw(r3+4) = r18 }
	{ memw(r3+8) = r20; memw(r3+12) = r21 }
	{ if (!p0) jump:nt 00021138; if (p0) r13 = memw(r29+80); if (p0) r12 = memw(r29+88) }

l00021068:
	{ r6 = !cmp.eq(r13,00000001); r5 = memd(r29+68); r7 = memd(r29+92) }
	{ r9 = mpyi(r5,r7); r0 = memd(r29+84); r2 = memd(r29+76) }
	{ p2 = r0; p0 = cmp.eq(r5,#0x1); r3 = memd(r29+64); r4 = memd(r29+72) }
	{ r8 = mpyi(r3,r2); r9 = mpyi(r9,r13); p1 = cmp.eq(r4,#0x1); r0 = memw(r29+96) }
	{ r4 = mpyi(r13,r5); r2 = mux(p2,#0x0,r12); p2 = cmp.eq(r7,#0x1); if (p1) r9 = add(r14,#0x0) }
	{ r8 = mpyi(r8,r12); p0 = r0; r3 = mux(p0,#0x0,r13); r5 = !cmp.eq(r12,00000001) }
	{ r7 = #0x0; if (p2) r4 = add(r14,#0x0); if (p0) r8 = add(r14,#0x0) }

l000210C0:
	{ p0 = cmp.gt(r10,#0x0); if (!p0.new) jump:nt 00021128; r13:r12 = combine(r25,r26); r14 = #0x0 }

l000210C4:
	{ r13:r12 = combine(r25,r26); r14 = #0x0 }

l000210CC:
	{ p0 = cmp.gt(r12,#0x0); if (!p0.new) jump:nt 00021118; r28 = r12; r15 = r13 }

l000210D8:
	{ loop1(000210DC,r20) }
	{ p0 = cmp.gt(r13,#0x0); if (!p0.new) jump:nt 0002110C; r10 = r23; if (!p0) r1:r0 = combine(r15,r28) }

l000210E8:
	{ loop0(000210EC,r21) }
	{ r0 = addasl(r0,r6,#0x2); r1 = addasl(r1,r5,#0x2); r11 = memw(r1); r16 = memw(r0) }
	{ r11 = sub(r11,r16) }
	{ nop; memw(r10++#4) = r11 }
	{ r23 = addasl(r23,r21,#0x2) }

l0002110C:
	{ r28 = addasl(r28,r3,#0x2); r15 = addasl(r15,r2,#0x2); nop }

l00021118:
	{ r13 = addasl(r13,r22,#0x2); r12 = addasl(r12,r4,#0x2); r14 = add(r14,#0x1); if (!cmp.eq(r14.new,r18)) jump:t 000210CC }

l00021128:
	{ r25 = addasl(r25,r8,#0x2); r26 = addasl(r26,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r24)) jump:t 000210C0 }

l0002112C:
	{ r26 = addasl(r26,r9,#0x2); r7 = add(r7,#0x1); if (!cmp.eq(r7.new,r24)) jump:t 000210C4 }

l00021138:
	{ immext(#0x9240); r0 = add(PC,#0x9271); r2 = memw(r29+60); memb(r29) = r2.new }

l0002113C:
	{ r0 = add(PC,#0x31); r2 = memw(r29+60); memb(r29) = r2.new }

l0002114C:
	{ r4 = add(PC,#0xC); r1 = #0xBD; r2 = r19 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00021160:
	{ r17:r16 = memd(r29+152); r19:r18 = memd(r29+144) }
	{ r21:r20 = memd(r29+136); r23:r22 = memd(r29+128) }
	{ r25:r24 = memd(r29+120); r27:r26 = memd(r29+112) }
	{ dealloc_return }

;; sub_int32_check: 00021174
sub_int32_check proc
	{ immext(#0x91C0); r4 = add(PC,#0x91FA); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r17 = r0; r1 = #0x37; r16 = r1 }
	{ immext(#0x91C0); r0 = add(PC,#0x91CD); r2 = r16; memw(r29) = r17 }
	{ call logmsg_function }
	{ r1 = #0x38; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 000211C4 }

l000211A8:
	{ r0 = add(PC,#0x31) }
	{ immext(#0x91C0); r3 = add(PC,#0x91CE) }

l000211B4:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l000211C4:
	{ r1 = #0x39; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 000211E4 }

l000211D4:
	{ r0 = add(PC,#0x5) }
	{ immext(#0x9180); r3 = add(PC,#0x91B1); jump 000211B4 }

l000211E4:
	{ immext(#0x9140); r0 = add(PC,#0x9171); r1 = #0x3A; r2 = r16 }
	{ immext(#0x9180); r4 = add(PC,#0x91A9); call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00021208
;;   Called from:
;;     00020F00 (in sub_int32_execute)
;;     00021030 (in sub_int32_execute)
;;     00021158 (in sub_int32_execute)
;;     00021194 (in sub_int32_check)
;;     000211F0 (in sub_int32_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00021228 }

l00021218:
	{ r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00021228:
	{ dealloc_return }

;; errlog_function: 0002122C
;;   Called from:
;;     00020F28 (in sub_int32_execute)
;;     000211B4 (in sub_int32_check)
errlog_function proc
	{ r3 = #0x0; r4 = r3; allocframe(#0x8) }
	{ call logv; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; prelu_execute: 00021250
;;   Called from:
;;     0002124C (in errlog_function)
prelu_execute proc
	{ r17:r16 = combine(r1,r0); memd(r29-16) = r17:r16; allocframe(#0x60) }
	{ immext(#0x38D1B700); r0 = #0x38D1B717; r2 = memw(r16+4); memd(r29+56) = r25:r24 }
	{ r3 = memw(r16+8) }
	{ memd(r29+80) = r19:r18; memd(r29+72) = r21:r20 }
	{ r24 = memw(r2); r5 = memw(r3+8) }
	{ r25 = memw(r3); r3 = memw(r3+4) }
	{ r18 = memw(r2+8); r21 = memw(r2+4) }
	{ r3 = memw(r24+4); memw(r29+28) = r3 }
	{ r3 = memw(r24+12); memw(r29+36) = r3 }
	{ r4 = memw(r18+16) }

;; fn00021298: 00021298
;;   Called from:
;;     00021294 (in prelu_execute)
;;     000212EC (in gvconv2dbbb_asm)
;;     00024FD0 (in gvconv2dbbb_asm)
fn00021298 proc
	{ r3 = memw(r21+16); memw(r29+40) = r3 }
	{ r2 = memw(r2+12) }
	{ r4 = memw(r4); memd(r29+48) = r27:r26 }
	{ r26 = memw(r3); r2 = memw(r2+16) }
	{ r7 = memw(r24+8); memd(r29+64) = r23:r22 }
	{ r1 = sfsub(r4,r26) }
	{ r7 = memw(r24); memw(r29+44) = r7 }
	{ memw(r29+24) = r5 }
	{ r22 = memw(r24+16); memw(r29+32) = r7 }
	{ call fn00009600; r23 = memw(r25+16); r19 = memw(r2) }
	{ immext(#0x437F0000); r2 = #0x437F0000 }
	{ call fn00009610; r1:r0 = combine(r0,r2); immext(#0x0); r20 = #0x0 }

l000212EC:
	{ immext(#0x0); r20 = #0x0 }

l000212F4:
	{ r2 = sfsub(r20,r26) }
	{ r0 = sfmpy(r2,r0); call fn00009620 }
	{ immext(#0x91C0); r4 = add(PC,#0x91EE); immext(#0x47800000); r3 = #0x47800000 }
	{ r3 = sfmpy(r19,r3); r27 = convert_uw2sf(r0):chop; r1 = #0x47; r2 = r17 }
	{ memw(r29) = r16 }
	{ r26 = convert_sf2uw(r3); call logmsg_function }
	{ immext(#0x91C0); r4 = add(PC,#0x91DA); r2 = memw(r18+16); r3 = memw(r21+16) }
	{ r1 = #0x4A; r2 = memw(r2); r3 = memw(r3) }
	{ r7:r6 = convert_sf2df(r2); r9:r8 = convert_sf2df(r3); r2 = r17 }
	{ memd(r29+8) = r7:r6; memd(r29) = r9:r8 }
	{ call logmsg_function }
	{ immext(#0x3F800000); r2 = #0x3F800000 }
	{ p0 = sfcmp.gt(r19,r2); if (!p0.new) jump:nt 00021384; if (p0.new) r1 = #0x4B }

l0002136C:
	{ immext(#0x9180); r3 = add(PC,#0x91B2) }

l00021374:
	{ call errlog_function; r2 = r17 }

l00021378:
	{ r2 = r17 }
	{ jump 00021558; r0 = #0xFFFFFFFF }

l00021384:
	{ p0 = sfcmp.gt(r20,r19); if (!p0.new) jump:nt 000213A4; r1 = #0x4E; p1 = cmp.gt(r27,#0xFFFFFFFF) }

l00021394:
	{ immext(#0x9180); r3 = add(PC,#0x91A0); jump 00021374; r1 = #0x4C }

l000213A4:
	{ r7:r6 = convert_sf2df(r19); p0 = cmp.gt(r27,#0xFF); r2 = memd(r29+36); r3 = memd(r29+32) }
	{ r3 = mpyi(r2,r3); if (p0) r27 = #0xFFFFFFFF; r0 = memd(r29+44); r5 = memd(r29+40) }
	{ immext(#0x9180); r4 = add(PC,#0x918E); r3 = mpyi(r3,r0); if (!p1) r27 = #0x0 }
	{ r20 = and(r27,#0xFF); memw(r29+8) = r26; memd(r29) = r7:r6 }
	{ immext(#0x8000); r2 = add(#0x8000,mpyi(r20,r26)); r27 = r20 }
	{ r27 -= lsr(r2,#0x10); r2 = r17 }
	{ immext(#0x10000); memw(r29+65548) = #0xFFFFFF80 }
	{ call logmsg_function }
	{ p0 = cmp.eq(r19,#0x0); r2 = memw(r25+20); if (!cmp.gtu(r19,r2.new)) jump:t 0002141C }

l00021410:
	{ r3 = add(PC,#0x29); jump 00021378; r1 = #0x50 }

l0002141C:
	{ r2 = memw(r24); r3 = memw(r24+4) }
	{ memw(r25+4) = r3; memw(r25) = r2 }
	{ r6 = memw(r24+8) }
	{ memw(r25+8) = r6 }
	{ if (!p0) r24 = #0x0; r7 = memw(r24+12) }
	{ if (p0) jump:nt 0002149C; memw(r25+12) = r7; memw(r25+24) = r19 }

l00021448:
	{ r3 = r27; r2 = memb(r22); memb(r23) = r2.new }

l0002144C:
	{ r2 = memb(r22); memb(r23) = r2.new }
	{ r2 = memb(r22); if (!cmp.gtu(r20,r2.new)) jump:t 00021490 }

l00021464:
	{ r4 = add(PC,#0x23); immext(#0x8000); r2 = add(#0x8000,mpyi(r2,r26)) }
	{ r3 += lsr(r2,#0x10); r2 = r17 }
	{ memw(r29+8) = r3; memw(r29+4) = r5 }
	{ call logmsg_function; memw(r29) = r24 }
	{ r23 = add(r23,#0x1); r22 = add(r22,#0x1); r24 = add(r24,#0x1); if (!cmp.eq(r24.new,r19)) jump:t 00021448 }

l00021490:
	{ r22 = add(r22,#0x1); r24 = add(r24,#0x1); if (!cmp.eq(r24.new,r19)) jump:t 0002144C }

l0002149C:
	{ r2 = memw(r21); r20 = memd(r29+28) }

l000214A0:
	{ r3 = memw(r21+4) }
	{ memw(r20+4) = r3; memw(r20) = r2 }
	{ r2 = memw(r21+8) }
	{ memw(r20+8) = r2 }
	{ r7 = memw(r21+12) }
	{ memw(r20+12) = r7 }
	{ r2 = memw(r21+24) }
	{ r6 = memw(r20+20); if (cmp.gtu(r2,r6.new)) jump:t 000214D0 }

l000214C8:
	{ call fn00009560; r2 = memw(r21+24); r1 = memw(r21+16) }

l000214D0:
	{ r2 = memw(r18); r4 = memd(r29+24) }
	{ r3 = memw(r18+4) }
	{ memw(r4+4) = r3; memw(r4) = r2 }
	{ r2 = memw(r18+8) }
	{ memw(r4+8) = r2 }
	{ r7 = memw(r18+12) }
	{ memw(r4+12) = r7 }
	{ r2 = memw(r18+24) }
	{ r6 = memw(r4+20); if (!cmp.gtu(r2,r6.new)) jump:t 00021500 }

l000214FC:
	{ r19 = add(r4,#0x10) }

l00021500:
	{ r19 = add(r4,#0x10); r0 = memw(r4+16); memw(r4+24) = r2 }
	{ call fn00009560; r2 = memw(r18+24); r1 = memw(r18+16) }
	{ immext(#0x9080); r4 = add(PC,#0x9094); r2 = memw(r19); r3 = memw(r20+16) }
	{ r1 = #0x61; r2 = memw(r2); r3 = memw(r3) }
	{ r7:r6 = convert_sf2df(r2); r9:r8 = convert_sf2df(r3); r2 = r17 }
	{ memd(r29+8) = r7:r6; memd(r29) = r9:r8 }
	{ call logmsg_function }
	{ immext(#0x9080); r4 = add(PC,#0x9081); r1 = #0x62; r2 = r17 }
	{ call logmsg_function; memw(r29) = r16 }
	{ r0 = #0x0 }

l00021558:
	{ r17:r16 = memd(r29+88); r19:r18 = memd(r29+80) }
	{ r21:r20 = memd(r29+72); r23:r22 = memd(r29+64) }
	{ r25:r24 = memd(r29+56); r27:r26 = memd(r29+48) }
	{ dealloc_return }
0002156C                                     00 C0 00 7F             ....

;; prelu_check: 00021570
prelu_check proc
	{ immext(#0x8F00); r4 = add(PC,#0x8F1A); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x69; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ immext(#0x8F00); r3 = add(PC,#0x8F15); r1 = #0x6A }
	{ r2 = memw(r17+16); if (!cmp.eq(r2.new,#0x4)) jump:t 000215F8 }

l000215A4:
	{ r3 = add(PC,#0x10); r1 = #0x6B }
	{ r2 = memw(r17+20); if (!cmp.eq(r2.new,#0x3)) jump:t 000215F8 }

l000215B8:
	{ r5 = #0x0; r4 = memw(r17+4) }

l000215BC:
	{ r2 = add(r2,#0x4); r5 = add(r5,#0x1); if (cmp.gt(r5.new,#0x2)) jump:nt 00021608 }

l000215CC:
	{ r3 = add(PC,#0x38); r6 = memw(r4++#4); if (!cmp.eq(r6.new,#0x0)) jump:t 000215E4 }

l000215DC:
	{ r1 = #0x6D }
	{ immext(#0x8EC0); r3 = add(PC,#0x8EEB); r6 = memw(r17+8) }

l000215E4:
	{ r3 = add(PC,#0x2B); r6 = memw(r17+8) }

l000215EC:
	{ r6 = memw(r4+r2); if (!cmp.eq(r6.new,#0x0)) jump:t 000215BC }

l000215F8:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00021608:
	{ immext(#0x8EC0); r4 = add(PC,#0x8ECF); r1 = #0x70; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00021628
;;   Called from:
;;     00021324 (in fn00021298)
;;     00021354 (in fn00021298)
;;     000213FC (in fn00021298)
;;     00021484 (in fn00021298)
;;     00021538 (in fn00021298)
;;     0002154C (in fn00021298)
;;     00021584 (in prelu_check)
;;     00021618 (in prelu_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 0002164C }

l00021638:
	{ r0 = add(PC,#0x3B); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l0002164C:
	{ dealloc_return }

;; errlog_function: 00021650
;;   Called from:
;;     00021374 (in fn00021298)
;;     000215F8 (in prelu_check)
;;     00021640 (in logmsg_function)
errlog_function proc
	{ immext(#0x8E00); r0 = add(PC,#0x8E1F); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
00021674             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; prelu_execute: 00021680
prelu_execute proc
	{ immext(#0x8F80); r4 = add(PC,#0x8FB5); memd(r29-16) = r17:r16; allocframe(#0x38) }
	{ r17:r16 = combine(r0,r1) }
	{ r1 = #0x38; r7 = memw(r17+4); memd(r29+40) = r19:r18 }
	{ r2 = r16; r5 = memw(r17+8) }
	{ memd(r29+32) = r21:r20; memd(r29+24) = r23:r22 }
	{ r6 = memw(r7+4); r18 = memw(r7) }
	{ r19 = memw(r5); memd(r29+16) = r25:r24 }
	{ r3 = memw(r6+16) }
	{ r23 = memw(r18+12); memd(r29+8) = r27:r26 }
	{ r20 = memw(r18+16); r24 = memw(r18+8) }
	{ r25 = memw(r18); r26 = memw(r18+4) }
	{ r21 = memw(r19+16); r22 = memw(r3) }
	{ call logmsg_function; memw(r29) = r17 }
	{ immext(#0x0); r2 = #0x0 }
	{ p0 = sfcmp.gt(r2,r22); if (!p0.new) jump:nt 00021704; if (p0.new) r1 = #0x39; if (!p0.new) r3 = memw(r19+20) }

l000216EC:
	{ immext(#0x8F40); r3 = add(PC,#0x8F61) }
	{ call errlog_function; r2 = r16 }

l000216F8:
	{ r2 = r16 }
	{ jump 00021794; r0 = #0xFFFFFFFF }

l00021704:
	{ r2 = mpyi(r26,r25) }
	{ r2 = mpyi(r2,r24) }
	{ r23 = mpyi(r2,r23) }
	{ r2 = asl(r23,#0x2); if (!cmp.gtu(r2.new,r3)) jump:t 00021724 }

l0002171C:
	{ r3 = add(PC,#0x4); r10 = #0x3A; jump 000216F8 }

l00021724:
	{ p0 = cmp.eq(r23,#0x0); r3 = memw(r18); r4 = memw(r18+4) }
	{ memw(r19+4) = r4; memw(r19) = r3 }
	{ r6 = memw(r18+8) }
	{ memw(r19+8) = r6 }
	{ r18 = #-0x1; r7 = memw(r18+12) }
	{ if (p0) jump:nt 00021778; memw(r19+12) = r7; memw(r19+24) = r2 }

l00021744:
	{ r19 = memw(r20) }
	{ call fn00009600; r1:r0 = combine(r18,r19) }
	{ r2 = sfmpy(r22,r19); r24 = r0 }
	{ call fn000097B0; r1:r0 = combine(r18,r2); r23 = add(r23,#0xFFFFFFFF) }
	{ r2 = sfadd(r24,r0); r20 = add(r20,#0x4); r21 = add(r21,#0x4); memb(r21) = r2.new }

l00021778:
	{ immext(#0x8EC0); r4 = add(PC,#0x8EF2); r1 = #0x42; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }

l00021794:
	{ r17:r16 = memd(r29+48); r19:r18 = memd(r29+40) }
	{ r21:r20 = memd(r29+32); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+16); r27:r26 = memd(r29+8) }
	{ dealloc_return }

;; prelu_check: 000217A8
prelu_check proc
	{ immext(#0x8E40); r4 = add(PC,#0x8E40); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x48; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r1 = #0x49; r2 = memw(r17+16); if (cmp.eq(r2.new,#0x2)) jump:t 000217E8 }

l000217D4:
	{ r3 = add(PC,#0x2F) }
	{ call errlog_function; r2 = r16 }

l000217DC:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l000217E8:
	{ r1 = #0x4A; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 00021800 }

l000217F8:
	{ r3 = add(PC,#0x1A); jump 000217DC }

l00021800:
	{ immext(#0x8E00); r4 = add(PC,#0x8E1E); r1 = #0x4B; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00021820
;;   Called from:
;;     000216CC (in prelu_execute)
;;     00021788 (in prelu_execute)
;;     000217BC (in prelu_check)
;;     00021810 (in prelu_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00021844 }

l00021830:
	{ r0 = add(PC,#0x1F); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00021844:
	{ dealloc_return }

;; errlog_function: 00021848
;;   Called from:
;;     000216F4 (in prelu_execute)
;;     000217D8 (in prelu_check)
;;     00021838 (in logmsg_function)
errlog_function proc
	{ immext(#0x8D80); r0 = add(PC,#0x8D83); r4 = r3; allocframe(#0x8) }
	{ r3 = #0x0; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ dealloc_return }
0002186C                                     00 00 00 00             ....

;; sum_f_execute: 00021870
sum_f_execute proc
	{ r28 = #0x1; allocframe(#0x30) }
	{ r2 = memw(r0+8); r8 = memw(r0+4) }
	{ r6 = memw(r0+16); memd(r29+40) = r17:r16 }
	{ r2 = r1; r7 = memw(r8); r14 = memw(r2) }
	{ p0 = cmp.eq(r6,#0x3); if (!p0.new) r17 = #0x1; memd(r29+32) = r19:r18; memd(r29+24) = r21:r20 }
	{ if (!p0) r1 = #0x1; if (!p0) r10 = #0x1; r15 = memw(r7); r3 = memw(r7+12) }
	{ if (p0) r18 = #0x0; if (p0) r13:r12 = combine(r3,r15); r4 = memw(r7+8); r5 = memw(r7+4) }
	{ r6 = memw(r7+16); r7 = memw(r14+16) }
	{ memd(r29+16) = r23:r22; memd(r29+8) = r25:r24 }
	{ if (p0) jump:nt 000218DC; memd(r29) = r27:r26 }

l000218D0:
	{ r13:r12 = combine(#0x1,#0x1); r9:r8 = combine(#0x1,#0x1) }
	{ jump fn00021A1C }

l000218DC:
	{ r9:r8 = combine(r4,r5); r10 = memw(r8+8); r1 = memw(r8+4) }
	{ r10 = memw(r10+16); r11 = memw(r1+12) }
	{ r1 = memw(r1+16); r10 = memw(r10) }
	{ if (p0.new) jump:nt 00021958; p0 = cmp.eq(r11,#0x0); if (!p0.new) r9:r8 = combine(r4,r5); if (!p0.new) r16 = add(r1,#0x0) }

l00021908:
	{ loop0(00021910,r11); r13:r12 = combine(r3,r15) }
	{ r17 = memw(r16++#4) }
	{ r17 = add(r10,sub(#0x7F,r17)); if (!cmp.gt(r17.new,#0x3)) jump:t 0002192C }

l00021920:
	{ r9:r8 = combine(#0x1,#0x1); r13 = #0x1; r12 = #0x1 }

l0002192C:
	{ p0 = cmp.eq(r9,#0x0); if (p0.new) jump:t 0002194C; if (p0.new) r13 = #0x1 }

l00021934:
	{ p0 = cmp.eq(r9,#0x1); if (p0.new) jump:t 0002194C; if (p0.new) r9 = #0x1 }

l0002193C:
	{ p0 = cmp.eq(r9,#0x2); if (p0.new) jump:t 0002194C; if (p0.new) r8 = #0x1 }

l00021944:
	{ p0 = cmp.eq(r17,#0x3); if (p0.new) r12 = #0x1 }

l0002194C:
	{ nop; nop }
	{ r18 = r11 }

l00021958:
	{ p0 = cmp.eq(r18,#0x0); r11 = r9; r0 = memb(r0+32); if (cmp.eq(r0.new,#0x2)) jump:t 0002197C }

l0002196C:
	{ r17 = r8; r28 = r12; r1 = r13 }
	{ jump fn00021A1C }

l0002197C:
	{ if (!p0) r11 = add(r9,#0x0); r17:r16 = combine(r8,r12); r0 = r13 }
	{ if (p0) jump:nt 000219D8; if (!p0) r0 = add(r13,#0x0); if (p0.new) r17:r16 = combine(r8,r12) }

l00021994:
	{ loop0(00021998,r18) }
	{ r18 = memw(r1++#4) }
	{ r18 = add(r10,sub(#0x7F,r18)); if (!cmp.gt(r18.new,#0x3)) jump:t 000219B4 }

l000219A8:
	{ r17 = #0x0; r0 = #0x0; r16 = #0x0 }
	{ jump 000219D0 }

l000219B4:
	{ p0 = cmp.eq(r10,#0x0); if (p0.new) jump:t 000219D0; if (p0.new) r0 = #0x0 }

l000219BC:
	{ p0 = cmp.eq(r10,#0x1); if (p0.new) jump:t 000219D0; if (p0.new) r11 = #0x0 }

l000219C4:
	{ p0 = cmp.eq(r10,#0x2); if (p0.new) jump:t 000219D0; if (p0.new) r17 = #0x0 }

l000219CC:
	{ p0 = cmp.eq(r18,#0x3); r16 = #-0x1 }

l000219D0:
	{ nop; nop }

l000219D8:
	{ p1 = cmp.eq(r16,#0x0); p2 = cmp.eq(r17,#0x0); p0 = cmp.eq(r11,#0x0); p3 = cmp.eq(r0,#0x0) }

l000219E0:
	{ p0 = cmp.eq(r11,#0x0); p3 = cmp.eq(r0,#0x0) }

;; fn000219E8: 000219E8
;;   Called from:
;;     000219D8 (in sum_f_execute)
;;     000219E0 (in gemaccb_asm)
fn000219E8 proc
	{ p1 = or(p1,p2); r1 = mux(p1,#0x1,r16) }
	{ p2 = or(p1,p0); if (!p2) r1 = add(r17,#0x0); r10 = mux(p1,#0x1,r16) }
	{ if (p3) jump:nt fn00021A1C; if (!p0) r10 = add(r1,#0x0); if (!p0) r1 = add(r11,#0x0); r17 = mux(p2,#0x1,r16) }

;; fn00021A0C: 00021A0C
;;   Called from:
;;     000219FC (in fn000219E8)
;;     000219FC (in fn000219E8)
;;     000219FC (in fn000219E8)
;;     00021A1C (in fn00021A1C)
fn00021A0C proc
	{ r28 = r17; r17 = r10; r10 = r1; r1 = r0 }

;; fn00021A1C: 00021A1C
;;   Called from:
;;     000218D8 (in sum_f_execute)
;;     00021978 (in sum_f_execute)
;;     000219FC (in fn000219E8)
;;     00021A0C (in fn00021A0C)
fn00021A1C proc
	{ r0 = mpyi(r13,r9); p0 = cmp.gt(r12,#0x0); r11 = memw(r14+20) }
	{ r0 = mpyi(r0,r8) }
	{ r0 = mpyi(r0,r12) }
	{ r0 = asl(r0,#0x2); if (!cmp.gtu(r0.new,r11)) jump:t 00021A58 }

l00021A3C:
	{ r0 = add(PC,#0x16); r1 = #0xC6 }
	{ immext(#0x8C80); r3 = add(PC,#0x8CA9); call errlog_function }
	{ jump 00021BB4; r0 = #0xFFFFFFFF }

l00021A58:
	{ r0 = #0x0; memw(r14+12) = r1; memw(r14+24) = r0 }
	{ if (p0) r0 = #0x0; memw(r14+8) = r10; memw(r14+4) = r17 }
	{ if (!p0) jump:nt 00021BB4; immext(#0x0); if (p0) r1 = #0x0; memw(r14) = r28 }

l00021A80:
	{ p0 = cmp.eq(r12,#0x1); p3 = cmp.eq(r9,#0x1); p1 = cmp.eq(r13,#0x1); p2 = cmp.eq(r8,#0x1) }

l00021A88:
	{ p1 = cmp.eq(r13,#0x1); p2 = cmp.eq(r8,#0x1) }

;; fn00021A90: 00021A90
;;   Called from:
;;     00021A80 (in fn00021A1C)
;;     00021A88 (in gemaddvvm_asm)
fn00021A90 proc
	{ r2 = mux(p0,r15,#0x1); r28 = mux(p3,r4,#0x1); r14 = mux(p1,r3,#0x1) }
	{ r15 = mux(p2,r5,#0x1) }

;; fn00021AA0: 00021AA0
;;   Called from:
;;     00021A9C (in fn00021A90)
;;     00021A9C (in fn00021A90)
;;     00021BA8 (in gemaddvvm_asm)
fn00021AA0 proc
	{ if (!p0.new) jump:nt 00021BA8; r10 = #0x0; p0 = cmp.gt(r8,#0x0) }

l00021AA4:
	{ r10 = #0x0; p0 = cmp.gt(r8,#0x0) }

l00021AAC:
	{ if (!p0.new) jump:nt 00021BA0; r11 = #0x0; p0 = cmp.gt(r9,#0x0) }

l00021AB0:
	{ r11 = #0x0; p0 = cmp.gt(r9,#0x0) }

l00021AB8:
	{ if (!p0.new) jump:nt 00021B98; r17:r16 = combine(#0x0,r7); p0 = cmp.gt(r13,#0x0) }

l00021AC4:
	{ p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 00021B84; r18 = r1 }

l00021ACC:
	{ r19:r18 = combine(#0x0,r1) }

l00021AD0:
	{ if (!p0.new) jump:nt 00021B7C; r22 = add(r19,r0); p0 = cmp.gt(r15,#0x0) }

l00021AD4:
	{ r22 = add(r19,r0); p0 = cmp.gt(r15,#0x0) }

l00021ADC:
	{ r21:r20 = combine(r10,#0x0) }
	{ r21 += mpyi(r22,r5) }

l00021AE4:
	{ if (!p0.new) jump:nt 00021B74; r24 = add(r21,r20); p0 = cmp.gt(r28,#0x0) }

l00021AF0:
	{ loop1(00021AFC,r28); r23:r22 = combine(r11,#0x0) }
	{ r23 += mpyi(r24,r4) }
	{ if (!p0.new) jump:nt 00021B68; r25 = add(r23,r22); r24 = r17; p0 = cmp.gt(r14,#0x0) }

l00021B0C:
	{ r26 = add(r14,#0xFFFFFFFF); p0 = cmp.gtu(r14,#0x1) }
	{ r24 += mpyi(r25,r3); if (p0) r27 = add(r26,#0xFFFFFFFF) }
	{ r25 = addasl(r6,r24,#0x2) }
	{ r24 = add(r25,#0x4) }
	{ if (p0) jump:nt 00021B34; r25 = memw(r25) }

l00021B2C:
	{ jump 00021B64; r24 = r25 }

l00021B34:
	{ loop0(00021B48,r27); p0 = cmp.gtu(r26,#0x1); r26 = add(r24,#0x4); r24 = memw(r24) }
	{ if (!p0) jump:nt 00021B60 }

l00021B48:
	{ r18 = sfadd(r18,r25); r27 = r24; r26 = add(r26,#0x4); r24 = memw(r26) }
	{ r25 = r27; nop }

l00021B60:
	{ r18 = sfadd(r18,r25) }

l00021B64:
	{ r18 = sfadd(r18,r24) }

l00021B68:
	{ r22 = add(r22,#0x1); nop; nop }

l00021B74:
	{ r20 = add(r20,#0x1); if (!cmp.eq(r20.new,r15)) jump:t 00021AE4 }

l00021B7C:
	{ r19 = add(r19,#0x1); if (!cmp.eq(r19.new,r2)) jump:t 00021AD0 }

l00021B80:
	{ if (!cmp.eq(r19.new,r2)) jump:t 00021AD4 }

l00021B84:
	{ r16 = add(r16,#0x4); r17 = r17; memw(r16) = r18 }

l00021B88:
	{ r17 = r17; memw(r16) = r18 }

l00021B8C:
	{ if (!p0.new) jump:nt 00021AC4; p0 = cmp.eq(r17,r13) }

l00021B94:
	{ r7 = addasl(r7,r13,#0x2) }

l00021B98:
	{ r11 = add(r11,#0x1); if (!cmp.eq(r11.new,r9)) jump:t 00021AB8 }

l00021BA0:
	{ r10 = add(r10,#0x1); if (!cmp.eq(r10.new,r8)) jump:t 00021AAC }

l00021BA4:
	{ if (!cmp.eq(r10.new,r8)) jump:t 00021AB0 }

l00021BA8:
	{ r0 = add(r0,#0x1); if (!cmp.eq(r0.new,r12)) jump:t fn00021AA0 }

l00021BAC:
	{ if (!cmp.eq(r0.new,r12)) jump:t 00021AA4 }

l00021BB4:
	{ r17:r16 = memd(r29+40); r19:r18 = memd(r29+32) }
	{ r21:r20 = memd(r29+24); r23:r22 = memd(r29+16) }
	{ r25:r24 = memd(r29+8); r27:r26 = memd(r29) }
	{ dealloc_return }
00021BC8                         00 40 00 7F 00 C0 00 7F         .@......

;; sum_f_check: 00021BD0
sum_f_check proc
	{ immext(#0x8AC0); r4 = add(PC,#0x8AC3); memd(r29-16) = r17:r16; allocframe(#0x10) }
	{ r1 = #0x37; r16 = r1; r17 = r0 }
	{ call logmsg_function; r2 = r16; memw(r29) = r17 }
	{ r2 = memw(r17+16) }
	{ r3 = #0x4; if (cmp.gtu(r3.new,r2)) jump:t 00021C1C }

l00021BFC:
	{ r0 = add(PC,#0x0); r1 = #0x38 }

l00021C04:
	{ immext(#0x8A80); r3 = add(PC,#0x8A9B) }

l00021C0C:
	{ call errlog_function; r2 = r16 }
	{ r0 = #0xFFFFFFFF; r17:r16 = memd(r29+8); dealloc_return }

l00021C18:
	{ r17:r16 = memd(r29+8); dealloc_return }

l00021C1C:
	{ p0 = cmp.eq(r2,#0x0); if (!p0.new) jump:nt 00021C30; r1 = #0x39 }

l00021C24:
	{ immext(#0x8A40); r0 = add(PC,#0x8A54); jump 00021C04 }

l00021C30:
	{ r1 = #0x3A; r2 = memw(r17+20); if (cmp.eq(r2.new,#0x1)) jump:t 00021C50 }

l00021C40:
	{ r0 = add(PC,#0x3C) }
	{ immext(#0x8A40); r3 = add(PC,#0x8A6A); jump 00021C0C }

l00021C50:
	{ immext(#0x8A40); r4 = add(PC,#0x8A6E); r1 = #0x3B; r2 = r16 }
	{ call logmsg_function; memw(r29) = r17 }
	{ r0 = #0x0 }
	{ r17:r16 = memd(r29+8); dealloc_return }

;; logmsg_function: 00021C6C
;;   Called from:
;;     00021BE4 (in sum_f_check)
;;     00021C5C (in sum_f_check)
logmsg_function proc
	{ r3 = #0x2; allocframe(#0x8) }
	{ r5 = memw(r2+16); if (cmp.gtu(r3,r5.new)) jump:t 00021C90 }

l00021C7C:
	{ r0 = add(PC,#0x0); r3 = #0x2; r5 = add(r29,#0x10) }
	{ call logv; r6 = add(r29,#0x10); memb(r29+1) = r6.new }

l00021C90:
	{ dealloc_return }

;; errlog_function: 00021C94
;;   Called from:
;;     00021A44 (in fn00021A1C)
;;     00021C0C (in sum_f_check)
;;     00021C84 (in logmsg_function)
errlog_function proc
	{ r3 = #0x0; r4 = r3; allocframe(#0x8) }
	{ call logv; r5 = add(r29,#0x10); r6 = add(r29,#0x10); memb(r29+1) = r6.new }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gemacca_asm: 00021CC0
;;   Called from:
;;     00021CBC (in errlog_function)
gemacca_asm proc
	{ r6 = lsr(r2,#0x10); r1 = lsr(r1,#0x1); r8 = zxth(r2); allocframe(#0x20) }
	{ loop1(00021D20,r1); r7 = lsr(r2,#0x14); memd(r29) = r17:r16; memd(r29+8) = r19:r18 }
	{ r13 = addasl(r0,r8,#0x1); r9 = asl(r8,#0x1); r10 = sub(#0x20,r8); memd(r29+16) = r21:r20 }
	{ M1 = r8; r6 = sub(r9,r6); r11 = sub(#0x10,r8); p2 = cmp.eq(r0,r0) }
	{ M0 = r11; r13 = addasl(r13,r8,#0x1); r12 = r13 }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ loop0(00021D40,r7); r17:r16 = combine(#0x0,#0x0); r19:r18 = combine(#0x0,#0x0) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r15:r14 = memd(r0+8); r21:r20 = memd(r0++m0) }
	{ r17:r16 += vraddub(r15:r14,r21:r20); p2 = not(p2); r21:r20 = memd(r0++m0); r15:r14 = memd(r0+8) }
	{ r19:r18 += vraddub(r15:r14,r21:r20); if (!p2) r12 = add(r12,r8); if (p2) r12 = add(r12,r10) }
	{ r16 = add(r17,r16); r17 = add(r19,r18); r0 = add(r0,r6) }
	{ r16 = mpyi(r16,r4); r17 = mpyi(r17,r4); r19:r18 = memd(r3) }
	{ r12 = r13; r16 = add(r16,r18); r17 = add(r17,r19) }
	{ r13 = addasl(r13,r8,#0x1); p2 = or(p2,!p2); memd(r3++#8) = r17:r16 }
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ r21:r20 = memd(r29+16) }
	{ dealloc_return }
00021D9C                                     00 00 00 00             ....

;; gemaccb_asm: 00021DA0
gemaccb_asm proc
	{ r2 = lsr(r2,#0x2); p0 = cmp.eq(r3,#-0x1); if (p0.new) jump:nt 000219E0; immext(#0x1010100); r4 = #0x1010101; p0 = cmp.eq(r3,#0x0) }

l00021DB0:
	{ loop0(00021DC0,r2); if (p0) jump:nt 00021DC8; r5 = #0x10; p0 = cmp.eq(r0,r0); if (!p0.new) jump:nt fn00021F70 }

l00021DC0:
	{ p0 = cmp.gtu(r4,#0x2); if (p0.new) jump:t 00021EC0; r0 = #0x10; r1 = add(r1,#0x1) }

l00021DC8:
	{ r0 = r1; jump 00021E0C }
00021DCC                                     04 C0 01 28             ...(
00021DD0 E3 C3 65 19 A3 E0 21 1C 04 44 43 1C 22 C0 21 28 ..e...!..DC.".!(
00021DE0 00 C0 9F 52 00 00 00 00 00 00 00 00 00 00 00 00 ...R............
00021DF0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; l2pref: 00021E00
l2pref proc
	{ r3 = zxth(r3); r2 = combine(r2.l,r1.l) }
	{ l2fetch(r0,r3:r2) }

l00021E0C:
	{ jumpr r31 }
00021E10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gemaddvvm_asm: 00021E20
;;   Called from:
;;     0000D584 (in gemm_asm)
gemaddvvm_asm proc
	{ r3 = lsr(r3,#0x1); r4 = asl(r4,#0x2); r1 = #0x0; r0 = r0 }
	{ M0 = r4; r3 = add(r3,#0xFFFFFFFF); r5 = memuh(r0); r1 = memb(r0+1); dcfetch(r0,#0x20) }
	{ r8 = r2; r11:r10 = memd(r0++#8); r5 = #0x0; r0 = r0 }

l00021E48:
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 00021A88; p0 = cmp.eq(r1,r6); if (!p0.new) jump:nt 00021E48; r9 = memw(r29); r2 = #0x30; r0 = r0 }

l00021E58:
	{ p0 = cmp.eq(r11,#-0x1); if (p0.new) jump:nt 00021A9C; p2 = !cmp.eq(r9,00000000); p0 = cmp.eq(r1,r0); if (!p0.new) jump:nt 00021E5C; r8 = #0x32; r0 = r0 }
	{ loop0(00021EA0,r3); r6 = #0x4; p1 = cmp.eq(r0,#0x2); if (p1.new) jump:nt 00021EF8; p1 = cmp.eq(r0,#0x2); if (p1.new) jump:nt 00021EFC }
	{ p0 = cmp.eq(r4,r3); if (!p0.new) jump:nt 00021E80; r10 = r2; jump 00021F8C; r2 = memuh(r0); r0 = memb(r4); dcfetch(r0,#0x60) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r8 = r2; jump 00021AB0; p0 = cmp.eq(r4,r6); if (!p0.new) jump:nt 00021EA8; r11:r10 = memd(r0++#8); r8 = #0x32; r0 = r0 }
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 00021AF0; r10 = r5; jump 00021FC4; p0 = cmp.eq(r1,r6); if (!p0.new) jump:nt 00021EB0; r2 = #0x30; r0 = r0 }
	{ p0 = cmp.eq(r11,#-0x1); if (p0.new) jump:nt 00021B04; r8 = r5; jump 00021AD0; p0 = cmp.eq(r1,r0); if (!p0.new) jump:nt 00021EC4; r8 = #0x32; r0 = r0 }
	{ p0 = cmp.eq(r4,r3); if (!p0.new) jump:nt 00021ED8; r10 = r2; jump 00021FE4; r2 = memuh(r0); r0 = memb(r4); dcfetch(r0,#0x60) }
	{ r8 = r2; jump 00021AF0; p0 = cmp.eq(r4,r6); if (!p0.new) jump:nt 00021EE8; r8 = #0x32; r0 = r0 }
	{ loop0(00021EF4,#0x5); r10 = r5; jump 00022000 }
	{ p1 = cmp.gtu(r6,#0x8); if (!p1.new) jump:t 00021FC4 }
	{ p1 = cmp.gtu(r6,#0xA); if (!p1.new) jump:t 000221CC; r9 = r8; jump 00021B08 }
	{ r6 = add(r6,r6); r11 = r10; jump 00022014 }
	{ r5 = #0x2; r1 = add(r1,#0x1) }
	{ r5 = #0x2; r0 = r0 }
	{ jumpr r31 }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gemmacbbw_asm: 00021F20
gemmacbbw_asm proc
	{ r4 = asl(r4,#0x2); allocframe(#0x38) }
	{ r28 = lsr(r5,#0x10); r5 = zxth(r5); memd(r29) = r17:r16; memw(r29+48) = r28 }
	{ M1 = r4; r3 = lsr(r3,#0x3); memd(r29+16) = r21:r20; memd(r29+24) = r23:r22 }
	{ M0 = r5; memd(r29+8) = r19:r18 }
	{ loop1(00021FE0,r3); memd(r29+32) = r25:r24 }
	{ r8 = asl(r5,#0x3); r3 = r2 }
	{ memd(r29+40) = r27:r26 }
	{ r6 = lsr(r28,#0x4); r8 = sub(r8,r5); r13 = r0; r2 = #0x30; r0 = add(r0,r0) }

;; fn00021F70: 00021F70
;;   Called from:
;;     00021DB0 (in gemaccb_asm)
;;     00021F64 (in gemmacbbw_asm)
fn00021F70 proc
	{ r2 = #0x30; r0 = add(r0,r0) }
	{ r9 = r1; r4 = sub(#0x10,r8); r2 = #0x30; r0 = add(r0,r0) }
	{ r8 += sub(r5,r28); r7 = add(r13,#0x30); r5 = add(r5,r5); r2 = #0x30; r0 = add(r0,r0) }
	{ r14 = lsr(r5,#0x1); r10 = add(r13,r5); r9 = memuh(r0); r2 = memb(r0+2); dcfetch(r7,#0x0) }
	{ r13 = addasl(r13,r5,#0x2); r7 = add(r7,r14); r9 = #0x0; r3:r2 = combine(r7,#0x0) }
	{ r15 = sub(#0x20,r14); r23:r22 = memd(r0+8); r2 = #0x30; r0 = add(r0,r0) }
	{ r17:r16 = memd(r0++m0); r2 = #0x30; r0 = add(r0,r0) }
	{ r11 = add(r10,r5); r6 = add(r6,#0xFFFFFFFF); r21:r20 = memd(r0+8); r2 = #0x30; r0 = add(r0,r0) }
	{ loop0(00021FE0,r6); r12 = add(r11,r5); r19:r18 = memd(r0++m0); r2 = #0x30; r0 = add(r0,r0) }
	{ p0 = cmp.gtu(r6,#0xA); if (p0.new) jump:t gemmpybbw_asm; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t gemmpybbw_asm; r9 = memuh(r0); r2 = memb(r2+2); dcfetch(r10,#0x0) }

;; fn00021FF0: 00021FF0
;;   Called from:
;;     00021FDC (in fn00021F70)
;;     00021FE0 (in fn00021F70)
fn00021FF0 proc
	{ p0 = cmp.gtu(r7,#0xB); if (p0.new) jump:t 000222F0; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 000222F0; r10 = add(r10,r14); r9 = #0x0; r7:r6 = combine(r7,#0x0) }
	{ p0 = cmp.gtu(r0,#0x8); if (p0.new) jump:t 00022300; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00022300; r23:r22 = memd(r0+8); r21:r20 = memd(r0++m0) }
	{ p0 = cmp.gtu(r1,#0x9); if (p0.new) jump:t 00022310; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00022310; r17:r16 = memd(r0+8); r19:r18 = memd(r0++m0) }
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00022324; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00022324; r11 = add(r11,r14); dcfetch(r11,#0x0) }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00022334; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00022334; r12 = add(r12,r14); dcfetch(r12,#0x0) }
	{ p0 = cmp.gtu(r6,#0xA); if (p0.new) jump:t 00022344; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00022344; r19:r18 = memd(r0+8); r21:r20 = memd(r0++m0) }
	{ p0 = cmp.gtu(r7,#0xB); if (p0.new) jump:t 00022354; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022354; r17:r16 = memd(r0+8); r25:r24 = memd(r0++m0) }
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00022368; p0 = cmp.gtu(r8,#0x8); if (p0.new) jump:t 00022368; r14 = r15; r15 = r14 }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00022378; p0 = cmp.gtu(r9,#0x9); if (p0.new) jump:t 00022378; r25:r24 = memd(r0+8); r23:r22 = memd(r0++m0) }
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 00022388; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00022388; r21:r20 = memd(r0+8); r27:r26 = memd(r0) }
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 00022398; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022398; r0 = add(r0,r4) }
	{ p0 = cmp.gtu(r6,#0x8); if (p0.new) jump:t 000223A8; p0 = cmp.gtu(r10,#0x8); if (p0.new) jump:t 000223A8; r9 = memuh(r0); r2 = memb(r0+2); dcfetch(r7,#0x0) }
	{ p0 = cmp.gtu(r7,#0x9); if (p0.new) jump:t 000223B8; p0 = cmp.gtu(r11,#0x9); if (p0.new) jump:t 000223B8; r7 = add(r7,r14); r9 = #0x0; r3:r2 = combine(r7,#0x0) }
	{ p0 = cmp.gtu(r8,#0xA); if (p0.new) jump:t 000223C8; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 000223C8; r23:r22 = memd(r0+8); r17:r16 = memd(r0++m0) }
	{ p0 = cmp.gtu(r9,#0xB); if (p0.new) jump:t 000223D8; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 000223D8; r21:r20 = memd(r0+8); r19:r18 = memd(r0++m0) }
	{ p0 = cmp.gtu(r6,#0xA); if (p0.new) jump:t 000223DC; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 000223DC; r7 = r13; r9 = #0x10; r2 = and(r2,#0x1) }
	{ p0 = cmp.gtu(r7,#0xB); if (p0.new) jump:t 000223EC; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 000223EC; r10 = add(r13,r5); r9 = #0x0; r7:r6 = combine(r7,#0x0) }
	{ p0 = cmp.gtu(r0,#0x8); if (p0.new) jump:t 000223FC; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 000223FC; r23:r22 = memd(r0+8); r21:r20 = memd(r0++m0) }
	{ p0 = cmp.gtu(r1,#0x9); if (p0.new) jump:t 0002240C; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 0002240C; r17:r16 = memd(r0+8); r19:r18 = memd(r0++m0) }
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00022420; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00022420; r11 = add(r10,r5); r3 = #0x32; r0 = add(r0,r0) }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00022430; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00022430; r12 = add(r11,r5) }
	{ p0 = cmp.gtu(r6,#0xA); if (p0.new) jump:t 0002243C; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 0002243C; r19:r18 = memd(r0+8); r21:r20 = memd(r0++m0) }
	{ p0 = cmp.gtu(r7,#0xB); if (p0.new) jump:t 0002244C; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 0002244C; r17:r16 = memd(r0+8); r25:r24 = memd(r0++m0) }
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00022460; p0 = cmp.gtu(r8,#0x8); if (p0.new) jump:t 00022460; r3 = #0x32; r0 = add(r0,r0) }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 0002246C; p0 = cmp.gtu(r9,#0x9); if (p0.new) jump:t 0002246C; r25:r24 = memd(r0+8); r23:r22 = memd(r0++m0) }
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 0002247C; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 0002247C; r21:r20 = memd(r0+8); r3 = #0x32; r0 = add(r0,r0) }
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 0002248C; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 0002248C; r27:r26 = memd(r0); r3 = #0x32; r0 = add(r0,r0) }
	{ p0 = cmp.gtu(r6,#0x8); if (p0.new) jump:t 000224A0; p0 = cmp.gtu(r10,#0x8); if (p0.new) jump:t 000224A0; r0 = add(r0,r4); r3 = #0x32; r0 = add(r0,r0) }
	{ p0 = cmp.gtu(r7,#0x9); if (p0.new) jump:t 000224B0; p0 = cmp.gtu(r11,#0x9); if (p0.new) jump:t 000224B0; r3 = #0x32; r0 = add(r0,r0) }
	{ p0 = cmp.gtu(r8,#0xA); if (p0.new) jump:t fn000224BC; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t fn000224BC; r0 = add(r0,r8) }
	{ r14 = lsr(r5,#0x1); p0 = cmp.gtu(r9,#0xB); if (p0.new) jump:t 000224C8; r3 = #0x32; r0 = add(r0,r0) }
	{ p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 000224D4; r15 = sub(#0x20,r14); r3 = #0x32; r0 = add(r0,r0) }
	{ r7 = add(r7,r14) }
	{ dcfetch(r7,#0x0) }
	{ r10 = add(r10,r14) }
	{ dcfetch(r10,#0x0) }
	{ r11 = add(r11,r14) }
	{ dcfetch(r11,#0x0) }
	{ r12 = add(r12,r14) }
	{ dcfetch(r12,#0x0) }
	{ r7 = add(r7,r15) }
	{ dcfetch(r7,#0x0) }
	{ r10 = add(r10,r15) }
	{ dcfetch(r10,#0x0) }
	{ loop0(00022240,#0x2); r11 = add(r11,r15) }
	{ dcfetch(r11,#0x0) }
	{ r13 = addasl(r13,r5,#0x2); r12 = add(r12,r15) }
	{ dcfetch(r12,#0x0) }
	{ nop }
	{ r7 = add(r7,r14); dcfetch(r7,#0x0) }
	{ r10 = add(r10,r14); dcfetch(r10,#0x0) }
	{ r11 = add(r11,r14); dcfetch(r11,#0x0) }
	{ r12 = add(r12,r14); dcfetch(r12,#0x0) }
	{ r7 = add(r7,r15); dcfetch(r7,#0x0) }
	{ r10 = add(r10,r15); dcfetch(r10,#0x0) }
	{ r11 = add(r11,r15); dcfetch(r11,#0x0) }
	{ r12 = add(r12,r15); dcfetch(r12,#0x0) }
	{ r23:r22 = memd(r0+8) }
	{ r9 = r1; r17:r16 = memd(r0++m0); dcfetch(r7,#0x0) }
	{ r7 = add(r7,r14); r21:r20 = memd(r0+8); r9 = #0x10; r2 = and(r2,#0x1) }
	{ immext(#0xFFFFFD40); loop0(000222AC,r6); r19:r18 = memd(r0++m0); r9 = #0x0; r3:r2 = combine(r7,#0x0) }
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+40) }
	{ r28 = memw(r29+48); dealloc_return }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gemmpybbw_asm: 000222E0
;;   Called from:
;;     0000D570 (in gemm_asm)
;;     00021FE0 (in fn00021F70)
;;     00021FE0 (in fn00021F70)
;;     00021FE0 (in fn00021F70)
gemmpybbw_asm proc
	{ allocframe(#0x40) }
	{ r28 = lsr(r5,#0x10); r5 = zxth(r5); p0 = cmp.eq(r12,r12); if (!p0.new) jump:nt fn000224BC; memw(r29+48) = r28 }

l000222F4:
	{ M0 = r5; r4 = asl(r4,#0x2) }
	{ r9 = lsr(r28,#0x4); r8 = asl(r5,#0x3); memd(r29) = r17:r16 }
	{ r3 = lsr(r3,#0x3); r8 = sub(r8,r5); r13 = r0; memd(r29+8) = r19:r18 }
	{ M1 = r4; r4 = sub(#0x10,r8); r9 = add(r9,#0xFFFFFFFF) }
	{ r8 += sub(r5,r28); r7 = add(r13,#0x50); r5 = add(r5,r5); memd(r29+16) = r21:r20 }
	{ loop1(000223A0,r3); r10 = add(r13,r5); memd(r29+24) = r23:r22; memd(r29+32) = r25:r24 }
	{ loop0(000223A0,r9); r6 = r1; r11 = add(r10,r5); memd(r29+40) = r27:r26 }
	{ r14 = lsr(r5,#0x1); r12 = add(r11,r5); r6 = memuh(r0); r2 = memb(r0+2); dcfetch(r7,#0x0) }
	{ r13 = addasl(r13,r5,#0x2); r7 = add(r7,r14); r15 = sub(#0x20,r14); r6 = #0x0; r3:r2 = combine(r7,#0x0) }
	{ r12 = r12; jump fn00022530; r12 = r12; jump 00022534; r23:r22 = memd(r0+8); r17:r16 = memd(r0++m0) }

l00022380:
	{ r12 = r12; jump fn00022548; r12 = r12; jump 0002254C; r21:r20 = memd(r0+8); r19:r18 = memd(r0++m0) }
00022390 00 40 00 7F 00 40 00 7F 00 40 00 7F 00 C0 00 7F .@...@...@......
000223A0 80 6A 16 19 81 6A 14 19 2A 42 06 29 00 C0 0A 94 .j...j..*B.)....
000223B0 80 6B 17 19 81 6B 15 19 0A 4E 0A F3 2B E7 06 28 .k...k...N..+..(
000223C0 80 68 10 19 81 68 12 19 36 40 C0 91 14 C0 C0 9D .h...h..6@......

l000223D0:
	{ p0 = cmp.gtu(r1,#0x9); if (p0.new) jump:t fn000226D0; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t fn000226D0; r17:r16 = memd(r0+8); r19:r18 = memd(r0++m0) }

l000223E0:
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 000226E4; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 000226E4; r11 = add(r11,r14); dcfetch(r11,#0x0) }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 000226F4; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 000226F4; r12 = add(r12,r14); dcfetch(r12,#0x0) }

l00022400:
	{ p0 = cmp.gtu(r6,#0xA); if (p0.new) jump:t 00022704; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00022704; r19:r18 = memd(r0+8); r21:r20 = memd(r0++m0) }

;; fn00022408: 00022408
;;   Called from:
;;     00022780 (in quantize_asm)
fn00022408 proc
	{ r19:r18 = memd(r0+8); r21:r20 = memd(r0++m0) }

;; fn00022410: 00022410
;;   Called from:
;;     00022408 (in fn00022408)
;;     00022408 (in fn00022408)
;;     00022408 (in fn00022408)
fn00022410 proc
	{ p0 = cmp.gtu(r7,#0xB); if (p0.new) jump:t 00022714; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022714; r17:r16 = memd(r0+8); r25:r24 = memd(r0++m0) }
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00022728; p0 = cmp.gtu(r8,#0x8); if (p0.new) jump:t 00022728; r14 = r15; r15 = r14 }

l00022424:
	{ p0 = cmp.gtu(r8,#0x8); if (p0.new) jump:t 0002272C; r14 = r15; r15 = r14 }

l00022430:
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00022738; p0 = cmp.gtu(r9,#0x9); if (p0.new) jump:t 00022738; r25:r24 = memd(r0+8); r23:r22 = memd(r0++m0) }

l00022440:
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 00022748; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00022748; r21:r20 = memd(r0+8); r27:r26 = memd(r0) }
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 00022758; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022758; r0 = add(r0,r4) }
	{ p0 = cmp.gtu(r6,#0x8); if (p0.new) jump:t 00022768; p0 = cmp.gtu(r10,#0x8); if (p0.new) jump:t 00022768; r6 = memuh(r0); r2 = memb(r0+2); dcfetch(r7,#0x0) }

;; fn00022464: 00022464
;;   Called from:
;;     00022850 (in fn00022548)
fn00022464 proc
	{ r6 = memuh(r0); r2 = memb(r0+2); dcfetch(r7,#0x0) }
	{ p0 = cmp.gtu(r7,#0x9); if (p0.new) jump:t 00022778; p0 = cmp.gtu(r11,#0x9); if (p0.new) jump:t 00022778; r7 = add(r7,r14); r6 = #0x0; r3:r2 = combine(r7,#0x0) }

l0002247C:
	{ p0 = cmp.gtu(r8,#0xA); if (p0.new) jump:t 00022788; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 00022788; r23:r22 = memd(r0+8); r17:r16 = memd(r0++m0) }
	{ p0 = cmp.gtu(r9,#0xB); if (p0.new) jump:t 00022798; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 00022798; r21:r20 = memd(r0+8); r19:r18 = memd(r0++m0) }
	{ p0 = cmp.gtu(r6,#0xA); if (p0.new) jump:t 0002279C; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 0002279C; r7 = r13; r6 = #0x10; r2 = and(r2,#0x1) }
	{ p0 = cmp.gtu(r7,#0xB); if (p0.new) jump:t 000227AC; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 000227AC; r10 = add(r13,r5); r6 = #0x0; r7:r6 = combine(r7,#0x0) }

;; fn000224BC: 000224BC
;;   Called from:
;;     000222E4 (in gemmpybbw_asm)
fn000224BC proc
	{ p0 = cmp.gtu(r0,#0x8); if (p0.new) jump:t fn000227BC; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t fn000227BC; r23:r22 = memd(r0+8); r21:r20 = memd(r0++m0) }

l000224CC:
	{ p0 = cmp.gtu(r1,#0x9); if (p0.new) jump:t 000227CC; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 000227CC; r17:r16 = memd(r0+8); r19:r18 = memd(r0++m0) }

;; fn000224D8: 000224D8
;;   Called from:
;;     000227CC (in fn00022610)
;;     000227CC (in fn000227BC)
;;     000227D8 (in fn00022610)
;;     00022888 (in fn00022888)
;;     00022888 (in fn00022878)
fn000224D8 proc
	{ r19:r18 = memd(r0++m0) }
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 000227E0; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 000227E0; r11 = add(r10,r5) }

l000224E8:
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 000227EC; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 000227EC; r12 = add(r11,r5) }
	{ p0 = cmp.gtu(r6,#0xA); if (p0.new) jump:t 000227F8; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 000227F8; r19:r18 = memd(r0+8); r21:r20 = memd(r0++m0) }

l000224F8:
	{ p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 000227FC; r19:r18 = memd(r0+8); r21:r20 = memd(r0++m0) }

l00022500:
	{ r21:r20 = memd(r0++m0) }

l00022504:
	{ p0 = cmp.gtu(r7,#0xB); if (p0.new) jump:t 00022808; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022808; r17:r16 = memd(r0+8); r25:r24 = memd(r0++m0) }

l00022514:
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 0002281C; p0 = cmp.gtu(r8,#0x8); if (p0.new) jump:t 0002281C; p0 = cmp.eq(r0,r0); if (!p0.new) jump:nt 000226D4; r2 = #0x32; r0 = add(r0,r0) }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 0002282C; p0 = cmp.gtu(r9,#0x9); if (p0.new) jump:t 0002282C; r25:r24 = memd(r0+8); r23:r22 = memd(r0++m0) }

l0002252C:
	{ r25:r24 = memd(r0+8); r23:r22 = memd(r0++m0) }

;; fn00022530: 00022530
;;   Called from:
;;     00022370 (in gemmpybbw_asm)
;;     0002252C (in fn00022610)
fn00022530 proc
	{ r23:r22 = memd(r0++m0) }
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 0002283C; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 0002283C; r21:r20 = memd(r0+8); r27:r26 = memd(r0) }

;; fn00022544: 00022544
;;   Called from:
;;     00022530 (in fn00022530)
;;     00022530 (in fn000227BC)
;;     00022534 (in fn00022530)
fn00022544 proc
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 0002284C; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 0002284C; r0 = add(r0,r4); r2 = #0x32; r0 = add(r0,r0) }

;; fn00022548: 00022548
;;   Called from:
;;     00022380 (in gemsumb_asm)
fn00022548 proc
	{ p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022850; r0 = add(r0,r4); r2 = #0x32; r0 = add(r0,r0) }

l0002254C:
	{ r0 = add(r0,r4); r2 = #0x32; r0 = add(r0,r0) }

l00022554:
	{ p0 = cmp.gtu(r6,#0x8); if (p0.new) jump:t fn00022860; p0 = cmp.gtu(r10,#0x8); if (p0.new) jump:t fn00022860; r0 = add(r0,r8); r2 = #0x32; r0 = add(r0,r0) }

;; fn00022564: 00022564
;;   Called from:
;;     00022554 (in fn00022548)
;;     00022554 (in fn00022610)
fn00022564 proc
	{ p0 = cmp.gtu(r7,#0x9); if (p0.new) jump:t 00022870; p0 = cmp.gtu(r11,#0x9); if (p0.new) jump:t 00022870; p0 = cmp.eq(r1,r1); if (!p0.new) jump:nt 00022724; r2 = #0x32; r0 = add(r0,r0) }
	{ p0 = cmp.gtu(r8,#0xA); if (p0.new) jump:t 00022880; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 00022880; p0 = cmp.eq(r2,r2); if (!p0.new) jump:nt 00022738; r2 = #0x32; r0 = add(r0,r0) }

l00022578:
	{ p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 00022884; p0 = cmp.eq(r2,r2); if (!p0.new) jump:nt 0002273C; r2 = #0x32; r0 = add(r0,r0) }

l00022584:
	{ p0 = cmp.gtu(r9,#0xB); if (p0.new) jump:t 00022890; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 00022890; p0 = cmp.eq(r3,r3); if (!p0.new) jump:nt 00022748; r2 = #0x32; r0 = add(r0,r0) }

;; fn0002258C: 0002258C
;;   Called from:
;;     000227C4 (in fn000227BC)
;;     00022938 (in fn00022C88)
fn0002258C proc
	{ p0 = cmp.eq(r3,r3); if (!p0.new) jump:nt fn00022750; r2 = #0x32; r0 = add(r0,r0) }

l00022590:
	{ r2 = #0x32; r0 = add(r0,r0) }

l00022594:
	{ r13 = addasl(r13,r5,#0x2); r14 = lsr(r5,#0x1); r2 = #0x32; r0 = add(r0,r0) }

l0002259C:
	{ r2 = #0x32; r0 = add(r0,r0) }
	{ loop0(000225C0,#0x3); r15 = sub(#0x20,r14); r2 = #0x32; r0 = add(r0,r0) }
	{ nop }
	{ nop; nop; nop; nop }

l000225B8:
	{ nop; nop }

l000225C0:
	{ r7 = add(r7,r14); dcfetch(r7,#0x0) }
	{ r10 = add(r10,r14); dcfetch(r10,#0x0) }

l000225CC:
	{ dcfetch(r10,#0x0) }

l000225D0:
	{ r11 = add(r11,r14); dcfetch(r11,#0x0) }

l000225D8:
	{ r12 = add(r12,r14); dcfetch(r12,#0x0) }
	{ r7 = add(r7,r15); dcfetch(r7,#0x0) }

;; fn000225E8: 000225E8
;;   Called from:
;;     00022424 (in fn00022898)
;;     00022610 (in fn00022610)
;;     000227C8 (in fn000227C8)
;;     000227FC (in fn00022898)
;;     00022810 (in fn00022898)
;;     00022960 (in vmemcpy_asm)
;;     000229C8 (in fn00022898)
fn000225E8 proc
	{ r10 = add(r10,r15); dcfetch(r10,#0x0) }

;; fn000225EC: 000225EC
;;   Called from:
;;     000225E8 (in fn000225E8)
;;     00022964 (in fn00022964)
fn000225EC proc
	{ dcfetch(r10,#0x0) }
	{ r11 = add(r11,r15); dcfetch(r11,#0x0) }

;; fn000225F8: 000225F8
;;   Called from:
;;     000225F0 (in fn000225EC)
;;     000225F0 (in fn000225EC)
;;     00022970 (in fn00022970)
fn000225F8 proc
	{ r12 = add(r12,r15); dcfetch(r12,#0x0) }
	{ r6 = r1; p0 = cmp.eq(r4,r4); if (!p0.new) jump:nt fn000227C8; r23:r22 = memd(r0+8); dcfetch(r7,#0x0) }

;; fn00022610: 00022610
;;   Called from:
;;     00022600 (in fn000225F8)
fn00022610 proc
	{ r7 = add(r7,r14); p0 = cmp.eq(r5,r5); if (!p0.new) jump:nt 000227D8; r17:r16 = memd(r0++m0); r6 = #0x10; r2 = and(r2,#0x1) }

l00022620:
	{ p0 = cmp.eq(r6,r6); if (!p0.new) jump:nt 000227EC; r21:r20 = memd(r0+8); r6 = #0x0; r3:r2 = combine(r7,#0x0) }

l0002262C:
	{ immext(#0xFFFFFD40); loop0(000226FC,r9); p0 = cmp.eq(r7,r7); if (!p0.new) jump:nt 000227F8; r19:r18 = memd(r0++m0) }

l0002263C:
	{ nop; nop; nop }
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+24) }

l00022650:
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+40) }
	{ r28 = memw(r29+48) }
	{ dealloc_return }

;; gemsuma_asm: 00022660
;;   Called from:
;;     0000D548 (in gemm_asm)
gemsuma_asm proc
	{ r6 = lsr(r2,#0x10); r1 = lsr(r1,#0x1); r8 = zxth(r2); allocframe(#0x20) }
	{ loop1(000226C0,r1); r7 = lsr(r2,#0x14); memd(r29) = r17:r16; memd(r29+8) = r19:r18 }
	{ r13 = addasl(r0,r8,#0x1); r9 = asl(r8,#0x1); r10 = sub(#0x20,r8); memd(r29+16) = r21:r20 }
	{ M1 = r8; r6 = sub(r9,r6); r11 = sub(#0x10,r8) }
	{ M0 = r11; r13 = addasl(r13,r8,#0x1); r12 = r13; p2 = cmp.eq(r0,r0) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ loop0(fn000226E0,r7); r17:r16 = combine(#0x0,#0x0); r19:r18 = combine(#0x0,#0x0) }

l000226C4:
	{ r17:r16 = combine(#0x0,#0x0); r19:r18 = combine(#0x0,#0x0) }

;; fn000226C8: 000226C8
;;   Called from:
;;     000226C0 (in gemsuma_asm)
;;     000226C4 (in fn00022980)
;;     00022980 (in fn00022980)
fn000226C8 proc
	{ nop; nop }

;; fn000226D0: 000226D0
;;   Called from:
;;     000223D0 (in quantize_asm)
;;     000223D0 (in quantize_asm)
fn000226D0 proc
	{ nop; nop; nop; nop }

l000226D8:
	{ nop; nop }

;; fn000226E0: 000226E0
;;   Called from:
;;     000226D0 (in fn000226D0)
;;     000226D0 (in fn000226D0)
;;     000226D8 (in fn00022A30)
fn000226E0 proc
	{ r15:r14 = memd(r0+8); r21:r20 = memd(r0++m0) }

l000226E4:
	{ r21:r20 = memd(r0++m0) }

;; fn000226E8: 000226E8
;;   Called from:
;;     000226E0 (in fn000226E0)
;;     000226E4 (in fn00022A30)
;;     00022A60 (in fn00022A30)
fn000226E8 proc
	{ r17:r16 += vraddub(r15:r14,r21:r20); p2 = not(p2); r21:r20 = memd(r0++m0); r15:r14 = memd(r0+8) }
	{ r19:r18 += vraddub(r15:r14,r21:r20); if (!p2) r12 = add(r12,r8); if (p2) r12 = add(r12,r10) }

l00022704:
	{ r16 = add(r17,r16); r17 = add(r19,r18); r0 = add(r0,r6) }
	{ r16 = mpyi(r16,r4); r17 = mpyi(r17,r4) }
	{ r12 = r13; r16 = add(r16,r5); r17 = add(r17,r5) }
	{ r13 = addasl(r13,r8,#0x1); p2 = cmp.eq(r0,r0); memd(r3++#8) = r17:r16 }

l0002272C:
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ r21:r20 = memd(r29+16) }
	{ dealloc_return }

l00022738:
	{ r0 = memw(r0); r0 = memw(r0) }

l0002273C:
	{ r0 = memw(r0); r0 = memw(r0) }

;; gemsumb_asm: 00022740
;;   Called from:
;;     0000D558 (in gemm_asm)
;;     0002273C (in fn00022750)
gemsumb_asm proc
	{ r2 = lsr(r2,#0x2); p0 = cmp.eq(r3,#-0x1); if (p0.new) jump:nt 00022380; immext(#0x1010100); r4 = #0x1010101; p0 = cmp.eq(r3,#0x0) }

;; fn00022750: 00022750
;;   Called from:
;;     0002258C (in fn0002258C)
;;     00022594 (in fn0002258C)
;;     00022740 (in gemsumb_asm)
fn00022750 proc
	{ loop0(00022760,r2); if (p0) jump:nt 00022768; r5 = #0x10; p0 = cmp.eq(r0,r0); if (!p0.new) jump:nt 00022910 }

l00022760:
	{ p0 = cmp.gtu(r4,#0x2); if (p0.new) jump:t fn00022860; r0 = #0x10; r1 = add(r1,#0x1) }

l00022768:
	{ r0 = r1; jump 000227AC }
0002276C                                     E3 C3 65 19             ..e.
00022770 A3 60 21 1C 22 C0 21 28                         .`!.".!(        

l00022778:
	{ jumpr r31 }
0002277C                                     00 00 00 00             ....

;; quantize_asm: 00022780
;;   Called from:
;;     000182C4 (in autorequantize_execute)
;;     00018644 (in requantize_execute)
quantize_asm proc
	{ p0 = cmp.eq(r1,#-0x1); if (p0.new) jump:nt 000223D0; r5 = #0x7F; p0 = cmp.eq(r4,#-0x1); if (p0.new) jump:nt fn00022408 }

l0002278C:
	{ p0 = bitsclr(r4,r5); p0 = cmp.eq(r8,r0); if (!p0.new) jump:nt 0002294C; r0 = #0x10; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r2,#-0x1); if (p0.new) jump:nt 000223E8; r4 = add(r4,#0x7F) }
	{ r4 = lsr(r4,#0x7); r0 = r9; jump 000224E8; p0 = cmp.eq(r8,r1); if (!p0.new) jump:nt vmemcpy_asm; r0 = #0x10; r1 = add(r1,#0x1) }

l000227AC:
	{ r0 = #0x10; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r9,r0); if (p0.new) jump:t 00022578; if (!p0) r4 = add(r4,#0xFFFFFFFF) }

l000227B4:
	{ if (!p0) r4 = add(r4,#0xFFFFFFFF) }

l000227B8:
	{ r1 = r9; jump 00022500; p0 = cmp.eq(r8,r2); if (!p0.new) jump:nt 0002297C; r0 = #0x10; r1 = add(r1,#0x1) }

;; fn000227BC: 000227BC
;;   Called from:
;;     000224BC (in fn000224BC)
;;     000224BC (in fn000224BC)
fn000227BC proc
	{ p0 = cmp.eq(r8,r2); if (!p0.new) jump:nt fn00022980; r0 = #0x10; r1 = add(r1,#0x1) }

l000227C0:
	{ r0 = #0x10; r1 = add(r1,#0x1) }

l000227C4:
	{ loop0(000227E0,r4); p0 = cmp.eq(r9,r1); if (p0.new) jump:t fn0002258C }

;; fn000227C8: 000227C8
;;   Called from:
;;     00022600 (in fn000225F8)
fn000227C8 proc
	{ p0 = cmp.eq(r9,r1); if (p0.new) jump:t 00022590 }

l000227CC:
	{ nop }
	{ nop; nop; nop; nop }

l000227D8:
	{ nop; nop }

l000227E0:
	{ r2 = r9; jump 0002252C; p0 = cmp.eq(r8,r3); if (!p0.new) jump:nt 000229A4; r0 = #0x10; r1 = add(r1,#0x1) }

l000227EC:
	{ p0 = cmp.eq(r9,r2); if (p0.new) jump:t 000225B8; r5 = r4; jump 00022400 }

l000227F4:
	{ r3 = r9; jump 00022540; p0 = cmp.eq(r8,r0); if (!p0.new) jump:nt 000229B4; r0 = #0x10; r1 = add(r1,#0x1) }

l000227F8:
	{ p0 = cmp.eq(r8,r0); if (!p0.new) jump:nt 000229B8; r0 = #0x10; r1 = add(r1,#0x1) }

l000227FC:
	{ r0 = #0x10; r1 = add(r1,#0x1) }

l00022800:
	{ p0 = cmp.eq(r9,r3); if (p0.new) jump:t 000225CC }

l00022804:
	{ r0 = r9; jump 0002254C; p0 = cmp.eq(r8,r1); if (!p0.new) jump:nt 000229C4; r0 = #0x10; r1 = add(r1,#0x1) }

l00022808:
	{ p0 = cmp.eq(r8,r1); if (!p0.new) jump:nt 000229C8; r0 = #0x10; r1 = add(r1,#0x1) }

l00022810:
	{ p0 = cmp.eq(r9,r0); if (p0.new) jump:t 000225D8; r7 = r6; jump 00022424 }

l00022818:
	{ r1 = r9; jump 00022560; p0 = cmp.eq(r8,r2); if (!p0.new) jump:nt 000229DC; r0 = #0x10; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r9,r1); if (p0.new) jump:t fn000225EC; r10 = r11; jump 0002297C; r3 = #0x12; r1 = add(r1,#0x1) }
	{ r2 = r9; jump 0002257C; p0 = cmp.eq(r8,r3); if (!p0.new) jump:nt 000229F4; r0 = #0x10; r1 = add(r1,#0x1) }

l0002283C:
	{ if (p0) jumpr:nt r31 }
00022840 E6 62 29 1C 0B C5 E4 1F A7 C3 E9 1F E7 E3 29 1C .b)...........).

l00022850:
	{ r7 = r6; jump fn00022464 }
00022854             AC CA CB 1F 0C C0 83 28 00 C0 9F 52     .......(...R

;; fn00022860: 00022860
;;   Called from:
;;     00022554 (in fn00022548)
;;     00022554 (in fn00022610)
fn00022860 proc
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

l00022868:
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; fn00022878: 00022878
;;   Called from:
;;     00022874 (in fn00022860)
;;     00022BF4 (in memconvert_hvx)
fn00022878 proc
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r8 = mpyi(r3,r4); r11 = asl(r4,#0x4) }

l00022884:
	{ r11 = asl(r4,#0x4) }

;; fn00022888: 00022888
;;   Called from:
;;     00022880 (in fn00022878)
;;     00022884 (in fn00022878)
fn00022888 proc
	{ p0 = cmp.eq(r5,#-0x1); if (p0.new) jump:nt fn000224D8; r8 = add(r8,#0xFF); immext(#0x8000); r9 = #0x8000 }

;; fn00022898: 00022898
;;   Called from:
;;     000224DC (in fn000224D8)
;;     00022500 (in fn00022ED8)
;;     00022888 (in fn00022888)
;;     00022888 (in fn00022878)
fn00022898 proc
	{ r8 = lsr(r8,#0x7); r3 = lsr(r3,#0x2); r7 = r2 }
	{ p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 000224F8; p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 000224F8; r10 = combine(r11.l,r3.l) }

l000228B0:
	{ p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 00022508; p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 00022508 }
	{ r6 = addasl(r2,r4,#0x2); l2fetch(r1,r11:r10) }
	{ p3 = sp1loop0(000228C4,r8) }
	{ p0 = cmp.eq(r8,r0); if (p0.new) jump:t 00022618; r7 = #0x10; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r8,r1); if (p0.new) jump:t 00022620; p0 = cmp.eq(r6,r7); if (p0.new) r7 = add(r2,#0x0); r1 = #0x14; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r8,r2); if (p0.new) jump:t 00022634; p0 = cmp.eq(r5,r0); if (!p0.new) jump:nt 000228DC; r7 = #0x10; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r8,r3); if (p0.new) jump:t 00022640; p0 = cmp.eq(r6,r7); if (p0.new) r7 = add(r2,#0x0); r1 = #0x14; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 0002254C; r11 = r10; jump 00022584; p0 = cmp.eq(r5,r1); if (!p0.new) jump:nt 000228F8; r7 = #0x10; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r6,r7); if (p0.new) r7 = add(r2,#0x0); r13 = r12; jump 00022594; r1 = #0x14; r1 = add(r1,#0x1) }

l00022910:
	{ r13 = r12; jump 0002259C; r1 = #0x14; r1 = add(r1,#0x1) }
00022918                         2C 40 A9 19 2D 40 A9 19         ,@..-@..

l00022920:
	{ p0 = cmp.eq(r5,r2); if (!p0.new) jump:nt 00022924; r7 = #0x10; r1 = add(r1,#0x1) }

l00022924:
	{ r7 = #0x10; r1 = add(r1,#0x1) }

l00022928:
	{ p0 = cmp.eq(r6,r7); if (p0.new) r7 = add(r2,#0x0); p0 = cmp.eq(r5,r3); if (!p0.new) jump:nt 0002292C; r1 = #0x14; r1 = add(r1,#0x1) }

l0002292C:
	{ if (p0.new) r7 = add(r2,#0x0); p0 = cmp.eq(r5,r3); if (!p0.new) jump:nt 00022930; r1 = #0x14; r1 = add(r1,#0x1) }

l00022930:
	{ p0 = cmp.eq(r5,r3); if (!p0.new) jump:nt 00022934; r1 = #0x14; r1 = add(r1,#0x1) }

l00022934:
	{ r1 = #0x14; r1 = add(r1,#0x1) }

l00022938:
	{ p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt fn0002258C; r7 = r6; jump 00022A88; r0 = #0x1A; r9 = add(r9,#0x1) }

l00022944:
	{ jumpr r31 }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; vmemcpy_asm: 00022960
;;   Called from:
;;     0000CFB4 (in fn0000CFB0)
;;     0000D02C (in unpad2d)
;;     0000D068 (in unpad2d_bytes)
;;     0000D854 (in im2col_co)
;;     0000DB68 (in im2col_cn)
;;     0000DEE4 (in im2col_slice_v0_co)
;;     0000E1B8 (in im2col_slice_co)
;;     0000E384 (in fast_im2col_co)
;;     0000E3C4 (in fast_im2col_co)
;;     00014BF4 (in input_execute)
;;     0001D68C (in reshape_execute)
vmemcpy_asm proc
	{ immext(#0x1010100); r8 = #0x1010101; r7 = add(r2,r0); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt fn000225E8 }

;; fn00022964: 00022964
;;   Called from:
;;     0000D030 (in unpad2d)
fn00022964 proc
	{ r8 = #0x1; r7 = add(r2,r0); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt fn000225EC }

;; fn00022970: 00022970
;;   Called from:
;;     00022960 (in vmemcpy_asm)
;;     00022964 (in fn00022964)
fn00022970 proc
	{ r5 = and(r0,#0x7F); r9 = add(r8,r8); r10 = add(r0,r2); p0 = cmp.eq(r7,#-0x1); if (p0.new) jump:nt fn000225F8 }

;; fn00022980: 00022980
;;   Called from:
;;     000227BC (in fn000227BC)
;;     00022970 (in fn00022970)
fn00022980 proc
	{ p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 000226C4; p0 = cmp.gt(r9,#-0x1); if (p0.new) jump:nt fn000226C8; r4 = and(r1,#0x7F); r7 = and(r7,#0x7F) }

l00022990:
	{ r6 = sub(r4,r5); r5 = add(r5,r2); r3 = sub(r2,r7); r0 = #0x0; jump 000229B0 }
	{ p0 = cmp.gtu(r8,#0x0); if (!p0.new) jump:t 00022664; p2 = cmp.gt(r5,#0x7F); if (!p2.new) r9 = add(r8,#0x0); r3 = add(r3,#0x7F) }
	{ r3 = lsr(r3,#0x7); p0 = tstbit(r9,#0x0); if (p0.new) jump:nt 00022640; p1 = cmp.gt(r6,#0xFFFFFFFF); r1 = #0x0; r0 = r0 }

l000229B8:
	{ p1 = cmp.gt(r6,#0xFFFFFFFF); r1 = #0x0; r0 = r0 }
	{ loop0(000229E0,r3); p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 00022650; immext(#0x80); if (p1) r1 = add(r1,#0x80) }

l000229C8:
	{ immext(#0x80); if (p1) r1 = add(r1,#0x80) }

l000229D0:
	{ nop; nop; nop; nop }
	{ p1 = cmp.gtu(r6,#0x1); if (p1.new) jump:nt 000229E4; r1 = #0x21; jump 00022BA0; r1 = #0x10; r1 = add(r1,#0x1) }

l000229E4:
	{ r1 = #0x21; jump 00022BA4; r1 = #0x10; r1 = add(r1,#0x1) }

l000229EC:
	{ r0 = #0x0; jump 00022A14; r0 = #0x18; r1 = add(r1,#0x1) }
	{ p1 = cmp.gtu(r6,#0x1); if (p1.new) jump:nt 000229F8; r1 = #0x0; r0 = r0 }
	{ r0 = r10; r0 = #0x8; r8 = r8 }

l00022A04:
	{ jumpr r31 }
00022A08                         00 00 00 00 00 00 00 00         ........
00022A10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; memconvert_hvx: 00022A20
;;   Called from:
;;     00013754 (in concat_execute_slice_asm)
memconvert_hvx proc
	{ r3 = combine(r3.l,r3.l); r4 = combine(r4.l,r4.l); p0 = cmp.eq(r11,r11); if (!p0.new) jump:nt 00022BF4; r6 = memw(r29) }

;; fn00022A30: 00022A30
;;   Called from:
;;     00022A20 (in memconvert_hvx)
;;     00022DA8 (in gvconv2dbbb_asm)
fn00022A30 proc
	{ immext(#0x1010100); r9 = #0x1010101; r3 = #0x3; jump 00022A5C; r11 = #0xB; jump 00022B44 }
00022A40 00 44 26 60 25 40 A3 19                         .D&`%@..        

l00022A48:
	{ r12 = #0x80; r15 = r0 }
	{ r6 = r1; r8 = add(r2,r0); r14 = and(r0,#0x7F); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 000226D8 }

l00022A5C:
	{ p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 000226E4 }

l00022A60:
	{ r11 = add(r9,r9); r13 = and(r6,#0x7F); r8 = and(r8,#0x7F); p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt fn000226E8 }

l00022A70:
	{ p0 = cmp.eq(r11,#-0x1); if (p0.new) jump:nt 000227B4; p0 = cmp.gt(r11,#-0x1); if (p0.new) jump:nt 000227B8; r11 = r12; jump 00022BFC; r6 = #0x0; r0 = r0 }

l00022A80:
	{ r7 = sub(r13,r14); r14 = add(r14,r2); r0 = #0x0; jump 00022AA0; r6 = r5; jump 0002284C }

l00022A88:
	{ r0 = #0x0; jump 00022AA8; r6 = r5; jump 00022854 }
00022A90 63 60 69 19 E1 7F 67 75 0A 42 28 F3             c`i...gu.B(.    

l00022A9C:
	{ r7 = r5; jump 00022868 }
00022AA0 48 46 44 19 26 4C 06 FB                         HFD.&L..        

l00022AA8:
	{ r10 = add(r10,#0x7F); p2 = cmp.gt(r14,#0x7F) }
	{ p0 = cmp.gtu(r4,#0x7); if (!p0.new) jump:nt 00022B40; if (!p2) r11 = add(r9,#0x0); r11 = r12; jump fn00022C3C; r6 = #0x10; r1 = add(r1,#0x1) }

l00022AC0:
	{ r10 = lsr(r10,#0x7); p0 = tstbit(r11,#0x0); if (p0.new) jump:nt fn00022750; r6 = r5; jump 0002288C }
	{ p0 = cmp.eq(r11,#-0x1); if (p0.new) jump:nt 0002275C; r9 = r8; jump 0002274C; r7 = r5; jump fn00022898 }
	{ loop0(00022B00,r10); p0 = cmp.gtu(r4,#0x6); if (!p0.new) jump:nt 00022B68; r15 = #0x8; r8 = r8 }
	{ p0 = cmp.gtu(r4,#0x7); if (!p0.new) jump:nt 00022B74; r15 = r0; r0 = add(r0,r5); r1 = add(r1,r2) }
	{ nop; nop; nop; nop }
	{ r11 = r12; jump 00022C8C; r6 = #0x10; r1 = add(r1,#0x1) }
	{ r9 = r8; jump 00022788; r6 = r5; jump 000228D4 }
	{ r1 = #0x1; jump 00022B1C; r7 = r5; jump 000228DC }
	{ p0 = cmp.gtu(r4,#0x6); if (!p0.new) jump:nt 00022BA8; p1 = cmp.gtu(r7,#0x1); if (p1.new) jump:nt 00022B1C; r1 = #0x21; jump 00022CD8 }
	{ p0 = cmp.gtu(r4,#0x7); if (!p0.new) jump:nt 00022BB4; r10 = r11; jump 00022B24; r15 = #0x18; r1 = add(r1,#0x1) }

l00022B30:
	{ r6 = r1; r8 = add(r2,r0); r14 = and(r0,#0x7F); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 000227B8 }

l00022B40:
	{ r11 = add(r9,r9); r13 = and(r6,#0x7F); r9 = r8; jump 000227C0; p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt fn000227C8 }
00022B50 A3 40 AB 19 A4 41 AB 19 E8 CF 08 76 02 41 07 1B .@...A.....v.A..
00022B60 C6 8B 4C 1F 4C C0 06 28 02 D8 8F 28 00 C0 9F 52 ..L.L..(...(...R
00022B70 00 00 00 00 00 00 00 00 00 00 00 00             ............    

l00022B7C:
	{ r0 = memw(r0); r0 = memw(r0) }

;; avgpool_aligned_hvx: 00022B80
;;   Called from:
;;     00012004 (in avgpool_execute_slice_asm)
;;     00022B7C (in fn00022610)
avgpool_aligned_hvx proc
	{ M0 = r2; r7 = sub(r5,r3); r6 = memw(r29) }
	{ r7 = mpyi(r7,r2); immext(#0x1010100); r9 = #0x1010101; r6 = combine(r6.l,r6.l) }
	{ loop1(00022BC0,r4); r10 = r1; r1 = add(r1,#0x80); p0 = cmp.eq(r6,r6); if (!p0.new) jump:nt fn00022D68 }

l00022BA0:
	{ r10 = r1; r1 = add(r1,#0x80); p0 = cmp.eq(r6,r6); if (!p0.new) jump:nt fn00022D6C }

l00022BA4:
	{ r1 = add(r1,#0x80); p0 = cmp.eq(r6,r6); if (!p0.new) jump:nt 00022D70 }

;; fn00022BAC: 00022BAC
;;   Called from:
;;     00022B9C (in avgpool_aligned_hvx)
;;     00022BA4 (in fn00022610)
fn00022BAC proc
	{ loop0(00022BC0,r3); r6 = r6; jump fn00022D6C }
00022BB4             00 40 00 7F 00 40 00 7F 00 C0 00 7F     .@...@......

l00022BC0:
	{  }
	{ r10 = #0x30; r0 = r0 }
	{ loop0(00022BC0,r3); r10 = add(r10,r7); nop }
	{ loop1(00022BC0,r4); p0 = cmp.gtu(r6,#0x0); if (!p0.new) jump:nt 00022C5C; r10 = r1; r2 = add(r2,#0xFFFFFF80) }

l00022BE4:
	{ p0 = cmp.gtu(r6,#0x1); if (!p0.new) jump:nt fn00022C6C; r1 = add(r1,#0x80); p0 = !cmp.eq(r2,00000000); r6 = r6; jump 00022DA4 }

l00022BF4:
	{ if (p0) jump:nt 00022BC0; r5 = r4; jump fn00022878; r0 = #0x12; r1 = add(r1,#0x1) }

l00022C00:
	{ jumpr r31 }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; avgpool_nonaligned_hvx: 00022C20
;;   Called from:
;;     00012018 (in avgpool_execute_slice_asm)
avgpool_nonaligned_hvx proc
	{ M0 = r2; r7 = sub(r5,r3); r6 = memw(r29) }
	{ r7 = mpyi(r7,r2); immext(#0x1010100); r9 = #0x1010101; r6 = combine(r6.l,r6.l) }

;; fn00022C3C: 00022C3C
;;   Called from:
;;     00022AB0 (in fn00022C88)
;;     00022C2C (in avgpool_nonaligned_hvx)
fn00022C3C proc
	{ loop1(fn00022C60,r4); r10 = r1; r1 = add(r1,#0x80); p0 = cmp.eq(r6,r6); if (!p0.new) jump:nt 00022E08 }

l00022C4C:
	{ loop0(fn00022C60,r3); r6 = r6; jump fn00022E0C }
00022C54             00 40 00 7F 00 40 00 7F                 .@...@..    

l00022C5C:
	{ nop }

;; fn00022C60: 00022C60
;;   Called from:
;;     00022C5C (in memconvert_hvx)
;;     00022CD0 (in fn0002303C)
fn00022C60 proc
	{ r10 = #0x30; r0 = r0 }
	{ nop }
	{  }

;; fn00022C6C: 00022C6C
;;   Called from:
;;     00022BE4 (in memconvert_hvx)
;;     00022C60 (in fn00022C60)
fn00022C6C proc
	{ nop }
	{ loop0(fn00022C60,r3); r10 = add(r10,r7); nop }
	{ loop1(fn00022C60,r4); p0 = cmp.gtu(r6,#0x0); if (!p0.new) jump:nt fn00022D04; r2 = add(r2,#0xFFFFFF80) }

;; fn00022C88: 00022C88
;;   Called from:
;;     00022C7C (in fn00022C6C)
;;     00022C7C (in fn00022C6C)
;;     00022FF8 (in fn00022FF8)
fn00022C88 proc
	{ p0 = cmp.gtu(r6,#0x1); if (!p0.new) jump:nt 00022D10; p0 = cmp.gt(r2,#0x0); r11 = add(r2,#0x80); r12 = and(r0,#0x7F) }

l00022C98:
	{ if (p0) r11 = #0x80; p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 00022920 }

l00022CA0:
	{ r13 = sub(#0x0,r0); r12 = add(r12,r11); r5 = r4; jump 00022924 }
00022CAC                                     45 C0 AC 19             E...
00022CB0 0A 49 00 5C E1 4F 4C 75 27 C3 6D 19 15 41 43 1E .I.\.OLu'.m..AC.
00022CC0 10 C0 43 1E                                     ..C.            

l00022CC4:
	{ r10 = r1; p0 = cmp.eq(r0,r0); if (!p0.new) jump:nt fn00022E84; r0 = #0x8; r9 = add(r9,#0x1) }

l00022CD0:
	{ if (p0) jump:nt fn00022C60; r1 = add(r1,#0x80); p0 = cmp.eq(r1,r1); if (!p0.new) jump:nt fn00022E90; r0 = #0x18; r1 = add(r1,#0x1) }

l00022CE0:
	{ jumpr r31 }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; maxpool_aligned_hvx: 00022D00
;;   Called from:
;;     00015FD0 (in maxpool_execute_slice_asm)
maxpool_aligned_hvx proc
	{ M0 = r2; r7 = sub(r5,r3); r10 = r1 }

;; fn00022D04: 00022D04
;;   Called from:
;;     00022C78 (in fn00022C6C)
;;     00022C7C (in fn00022C6C)
fn00022D04 proc
	{ r7 = sub(r5,r3); r10 = r1 }

;; fn00022D08: 00022D08
;;   Called from:
;;     00022D04 (in fn00022D04)
;;     0002303C (in fn0002303C)
fn00022D08 proc
	{ r10 = r1 }

;; fn00022D0C: 00022D0C
;;   Called from:
;;     00022D00 (in maxpool_aligned_hvx)
;;     00022D08 (in fn00022D08)
fn00022D0C proc
	{ loop1(fn00022D20,r4); r7 = mpyi(r7,r2); r1 = add(r1,#0x80) }

l00022D10:
	{ r7 = mpyi(r7,r2); r1 = add(r1,#0x80) }

;; fn00022D18: 00022D18
;;   Called from:
;;     00022D0C (in fn00022D08)
;;     00022D0C (in fn00022D0C)
;;     00022D0C (in fn00022D0C)
;;     00022D10 (in fn00022C88)
fn00022D18 proc
	{ loop0(fn00022D20,r3); p0 = cmp.eq(r0,r0); if (!p0.new) jump:nt fn00022ED8 }

;; fn00022D20: 00022D20
;;   Called from:
;;     00022D18 (in fn00022D18)
;;     00022D18 (in fn00022D18)
;;     00023054 (in fn00023054)
fn00022D20 proc
	{ r2 = r0; jump 00022E60; r10 = #0x30; r0 = r0 }
00022D28                         10 5F 03 60 0A 87 0A F3         ._.`....
00022D30 00 C0 00 7F 18 5E 24 60 0A 40 61 70 01 50 01 B0 .....^$`.@ap.P..
00022D40 02 F0 E2 BF EE 78 DF 5C 10 40 02 75 E0 40 40 1C .....x.\.@.u.@@.
00022D50 00 C1 20 29                                     .. )            

l00022D54:
	{ jumpr r31 }
00022D58                         00 00 00 00 00 00 00 00         ........

;; maxpool_nonaligned_hvx: 00022D60
;;   Called from:
;;     00015FD8 (in maxpool_execute_slice_asm)
maxpool_nonaligned_hvx proc
	{ M0 = r2; r7 = sub(r5,r3); r10 = r1 }

;; fn00022D68: 00022D68
;;   Called from:
;;     00022B9C (in avgpool_aligned_hvx)
fn00022D68 proc
	{ r10 = r1 }

;; fn00022D6C: 00022D6C
;;   Called from:
;;     00022BA4 (in fn00022610)
;;     00022BAC (in fn00022BAC)
;;     00022D60 (in fn00022D68)
;;     00022D68 (in fn00022D68)
fn00022D6C proc
	{ loop0(00022D80,r3); p0 = cmp.eq(r0,r0); if (!p0.new) jump:nt fn00022F2C }

l00022D70:
	{ p0 = cmp.eq(r0,r0); if (!p0.new) jump:nt 00022F30 }

l00022D74:
	{ loop1(00022D80,r4); r7 = mpyi(r7,r2); r1 = add(r1,#0x80) }

l00022D80:
	{ r10 = #0x30; r0 = r0 }
	{ nop; r2 = r0; jump 00022EC4 }

l00022D8C:
	{ loop0(00022D80,r3); r10 = add(r10,r7); nop }
	{ loop1(00022D80,r4); r2 = add(r2,#0xFFFFFF80); r12 = and(r0,#0x7F); r11 = r2 }
	{ p0 = cmp.gt(r2,#0x0); if (p0.new) r11 = #0x80; r13 = sub(#0x0,r0); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt fn00022A30 }

l00022DB8:
	{ r12 = add(r12,r11); p0 = cmp.gtu(r13,#0x0); if (!p0.new) jump:nt 00022A04 }

l00022DC0:
	{ if (p1.new) jump:nt 00022DD4; p1 = cmp.gt(r12,#0x7F); p0 = cmp.eq(r12,#-0x1); if (p0.new) jump:nt 00022A48 }

l00022DCC:
	{ r1 = #0x1; jump 00022DF4; r0 = #0x0; jump 00022DEC }

l00022DD4:
	{ r10 = r1; p0 = cmp.eq(r0,r0); if (!p0.new) jump:nt 00022F94; r0 = #0x8; r9 = add(r9,#0x1) }

l00022DD8:
	{ p0 = cmp.eq(r0,r0); if (!p0.new) jump:nt fn00022F98; r0 = #0x8; r9 = add(r9,#0x1) }

;; fn00022DE0: 00022DE0
;;   Called from:
;;     00022DD8 (in fn00022FA0)
;;     00022FA0 (in fn00022FA0)
fn00022DE0 proc
	{ if (p0) jump:nt 00022D80; r1 = add(r1,#0x80); r0 = #0x18; r1 = add(r1,#0x1) }

l00022DE8:
	{ r0 = #0x18; r1 = add(r1,#0x1) }

l00022DEC:
	{ jumpr r31 }
00022DF0 00 00 00 00                                     ....            

l00022DF4:
	{ r0 = memw(r0); r0 = memw(r0) }

;; fn00022DF8: 00022DF8
;;   Called from:
;;     00022DF4 (in fn00022FB4)
;;     00022FB4 (in fn00022FB4)
fn00022DF8 proc
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gemvmpybbw_asm: 00022E00
;;   Called from:
;;     000154C8 (in matmul_asm)
;;     00022DFC (in fn00022DF8)
;;     00022DFC (in fn00022DF8)
;;     00022FBC (in fn00022FBC)
gemvmpybbw_asm proc
	{ r6 = memd(r29); allocframe(#0x10) }
	{ r5 = asl(r5,#0x2); r17 = #0x7F; memd(r29) = r17:r16 }

l00022E08:
	{ r17 = #0x7F; memd(r29) = r17:r16 }

;; fn00022E0C: 00022E0C
;;   Called from:
;;     0002317C (in fn00023178)
;;     00023180 (in fn00023180)
;;     000231CC (in fn000231CC)
fn00022E0C proc
	{ memd(r29) = r17:r16 }

;; fn00022E10: 00022E10
;;   Called from:
;;     00022578 (in fn00022750)
;;     00022578 (in fn00022750)
;;     0002259C (in fn00022750)
;;     000227B0 (in fn00022750)
;;     00022AB0 (in fn00022C88)
;;     00022C4C (in fn00022C3C)
;;     00022D18 (in fn00022D18)
;;     00022D20 (in fn00022D20)
;;     00022DE8 (in fn00022FAC)
;;     00022E08 (in fn00022C3C)
;;     00022E0C (in fn00022E0C)
;;     00022E38 (in fn00022E38)
;;     00022FF8 (in fn00022FF8)
;;     00023180 (in fn00023180)
;;     00023180 (in fn00023180)
fn00022E10 proc
	{ p0 = bitsclr(r5,r17); if (!p0.new) jump:nt 00022E20; p0 = cmp.eq(r5,#-0x1); if (p0.new) jump:nt 00022A9C }

l00022E1C:
	{ r0 = r0; jump 00022E20 }

l00022E20:
	{ r7 = lsr(r6,#0x4); r8 = add(r2,#0x80) }
	{ r7 = add(r7,#0xFFFFFFFF); dcfetch(r0,#0x80) }
	{ loop0(00022E80,r7); immext(#0x1010100); r16 = #0x1010101; r11:r10 = memd(r0++#8) }

;; fn00022E38: 00022E38
;;   Called from:
;;     00022FDC (in fn00022FBC)
;;     00022FF0 (in fn00022FEC)
;;     00022FF0 (in fn00022FEC)
fn00022E38 proc
	{ r16 = #0x1; r11:r10 = memd(r0++#8) }

l00022E40:
	{ p0 = cmp.gtu(r10,#0x0); if (p0.new) jump:nt 00022F08; p0 = cmp.gtu(r0,#0x0); if (p0.new) jump:nt 00023108; r14 = #0x0; r2 = #0x10; r2 = and(r2,#0x1) }

;; fn00022E50: 00022E50
;;   Called from:
;;     00022E40 (in fn00022E10)
;;     00022F4C (in fn00022610)
;;     000231C0 (in fn000231AC)
fn00022E50 proc
	{ p0 = cmp.gtu(r11,#0x1); if (p0.new) jump:t 00022F58; p0 = cmp.gtu(r0,#0x1); if (p0.new) jump:t 00023158; r8 = memuh(r0); r2 = memb(r4+2); r13:r12 = memd(r0++#8) }

l00022E60:
	{ p0 = cmp.gtu(r12,#0x2); if (p0.new) jump:t 00022F68; p0 = cmp.gtu(r0,#0x2); if (p0.new) jump:t 00023168; r15 = #0x0; r2 = #0x10; r2 = and(r2,#0x1) }
	{ nop; nop; nop; nop }
	{ p0 = cmp.gtu(r13,#0x3); if (p0.new) jump:t 00022F88; p0 = cmp.gtu(r0,#0x3); if (p0.new) jump:t 00023188; r8 = memuh(r0); r2 = memb(r4+2); dcfetch(r0,#0x80) }

;; fn00022E84: 00022E84
;;   Called from:
;;     00022CC4 (in fn0002303C)
fn00022E84 proc
	{ p0 = cmp.gtu(r0,#0x3); if (p0.new) jump:t fn0002318C; r8 = memuh(r0); r2 = memb(r4+2); dcfetch(r0,#0x80) }

;; fn00022E88: 00022E88
;;   Called from:
;;     00023280 (in biasadd_relu_requant_nonaligned_hvx)
fn00022E88 proc
	{ r8 = memuh(r0); r2 = memb(r4+2); dcfetch(r0,#0x80) }

;; fn00022E90: 00022E90
;;   Called from:
;;     00022CD0 (in fn0002303C)
;;     00022E84 (in fn00022E88)
;;     00022E88 (in fn00022E88)
fn00022E90 proc
	{ r15:r14 += vraddub(r13:r12,r11:r10); r11:r10 = memd(r0++#8) }

;; fn00022E94: 00022E94
;;   Called from:
;;     00022E90 (in fn00022E90)
;;     00023200 (in biasadd_relu_requant_nonaligned_hvx)
fn00022E94 proc
	{ r11:r10 = memd(r0++#8) }
	{ p0 = cmp.gtu(r10,#0x0); if (p0.new) jump:t fn00022FA0; p0 = cmp.gtu(r0,#0x0); if (p0.new) jump:t 000231A0; r2 = #0x10; r2 = and(r2,#0x1) }

;; fn00022E9C: 00022E9C
;;   Called from:
;;     00023054 (in fn00023054)
fn00022E9C proc
	{ p0 = cmp.gtu(r0,#0x0); if (p0.new) jump:t fn000231A4; r2 = #0x10; r2 = and(r2,#0x1) }

;; fn00022EA4: 00022EA4
;;   Called from:
;;     00022E98 (in fn00022E94)
;;     00022E98 (in fn00022E94)
;;     00022E9C (in fn00022E9C)
fn00022EA4 proc
	{ p0 = cmp.gtu(r11,#0x1); if (p0.new) jump:t fn00022FAC; p0 = cmp.gtu(r0,#0x1); if (p0.new) jump:t fn000231AC; r8 = memuh(r0); r2 = memb(r4+2); r13:r12 = memd(r0++#8) }

l00022EB4:
	{ p0 = cmp.gtu(r12,#0x2); if (p0.new) jump:t fn00022FBC; p0 = cmp.gtu(r0,#0x2); if (p0.new) jump:t 000231BC; r2 = #0x10; r2 = and(r2,#0x1) }
	{ p0 = cmp.gtu(r13,#0x3); if (p0.new) jump:t 00022FC8; p0 = cmp.gtu(r0,#0x3); if (p0.new) jump:t 000231C8; r8 = #0x10; r2 = and(r2,#0x1) }

l00022EC4:
	{ p0 = cmp.gtu(r0,#0x3); if (p0.new) jump:t fn000231CC; r8 = #0x10; r2 = and(r2,#0x1) }

l00022ECC:
	{ r15:r14 += vraddub(r13:r12,r11:r10) }
	{ r14 = add(r14,r15) }
	{ r14 = mpyi(r14,r3); r15 = mpyi(r1,r3) }

;; fn00022ED8: 00022ED8
;;   Called from:
;;     00022A9C (in fn00022E10)
;;     00022D18 (in fn00022D18)
;;     00022ED4 (in fn00022610)
fn00022ED8 proc
	{ r15 = mpyi(r1,r3) }
	{ r14 += mpyi(r15,r6); r1 = combine(r1.l,r1.l) }
	{ p0 = cmp.eq(r14,#-0x1); if (p0.new) jump:nt 00022B30; p0 = cmp.eq(r1,#-0x1); if (p0.new) jump:nt 00022B30 }

l00022EEC:
	{ r5 = r7; jump 00022F34; p0 = cmp.eq(r6,r4); if (!p0.new) jump:nt 00022EF4 }
	{ p0 = cmp.eq(r5,r4); if (!p0.new) jump:nt 00022EFC; r17:r16 = memd(r29) }
	{ r4 = #0x8; r8 = r8 }
	{ dealloc_return }
	{ r0 = memw(r0); r0 = memw(r0) }

l00022F08:
	{ r0 = memw(r0); r0 = memw(r0) }

;; fn00022F0C: 00022F0C
;;   Called from:
;;     00022F08 (in fn00022E10)
;;     000230C0 (in biasadd_relu_requant_nonaligned_hvx)
fn00022F0C proc
	{ r0 = memw(r0); r0 = memw(r0) }

;; fn00022F10: 00022F10
;;   Called from:
;;     00022F0C (in fn00022F0C)
;;     00022F0C (in fn00022F0C)
;;     000232D0 (in biasadd_relu_requant_nonaligned_hvx)
fn00022F10 proc
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; fn00022F1C: 00022F1C
;;   Called from:
;;     00022F18 (in fn00022F10)
;;     00022F18 (in fn00022F10)
;;     00023248 (in biasadd_relu_requant_nonaligned_hvx)
fn00022F1C proc
	{ r0 = memw(r0); r0 = memw(r0) }

;; im2col33322_hvx: 00022F20
;;   Called from:
;;     00022F1C (in fn00022F1C)
;;     00022F1C (in fn00022F1C)
im2col33322_hvx proc
	{ allocframe(#0x40) }
	{ r2 = vsplatb(r2); r3 = #0x0; r0 = r0 }

;; fn00022F2C: 00022F2C
;;   Called from:
;;     00022D70 (in fn00022610)
;;     00022F24 (in im2col33322_hvx)
fn00022F2C proc
	{ p0 = cmp.eq(r2,#-0x1); if (p0.new) jump:nt 00022B7C; r7 = #0x20; memd(r29) = r17:r16; memd(r29+8) = r19:r18 }

l00022F30:
	{ r7 = #0x20; memd(r29) = r17:r16; memd(r29+8) = r19:r18 }

l00022F38:
	{ memd(r29+16) = r21:r20; memd(r29+24) = r23:r22 }
	{ M1 = r7; r7 = #0x78; memd(r29+32) = r25:r24; memd(r29+40) = r27:r26 }
	{ M0 = r7; r23 = #0x381; immext(#0x1010100); r7 = #0x1010101 }

l00022F58:
	{ r7 = #0x1 }

l00022F5C:
	{ r9 = asl(r7,#0x2); r10 = asl(r7,#0x3); r8 = add(r7,r7); r24 = #0x12A0 }
	{ r12 = asl(r10,#0x2); r13 = asl(r10,#0x3); r11 = add(r10,r10) }
	{ r27 = add(r13,r13); r3 = r4; r5 = add(r5,r4) }

;; fn00022F80: 00022F80
;;   Called from:
;;     00022F78 (in fn00022E50)
;;     000230E4 (in fn000230DC)
fn00022F80 proc
	{ r19 = r1; r25 = and(r1,#0x60); r1 = add(r1,r24); r15 = add(r3,r3) }
	{ r15 = mpyi(r15,r23); r25 = sub(#0x0,r25) }

l00022F94:
	{ r25 = sub(#0x0,r25) }

;; fn00022F98: 00022F98
;;   Called from:
;;     000226F8 (in fn000226E8)
;;     000227B4 (in fn00022A30)
;;     00022A70 (in fn00022A30)
;;     00022DD8 (in fn00022FA0)
;;     00022F90 (in fn00022F80)
;;     00022F90 (in fn00022F80)
;;     00022F94 (in fn00022FA0)
;;     0002314C (in gvconv2dbbb_asm)
;;     000231C4 (in gvconv2dbbb_asm)
;;     000231C4 (in gvconv2dbbb_asm)
fn00022F98 proc
	{ r15 = add(r0,r15); r22 = #0xFFFFFFE0; r21 = add(r25,#0xFFFFFFF7); p0 = cmp.gtu(r9,#0x9); if (!p0.new) jump:nt 00022DD8 }

;; fn00022FA0: 00022FA0
;;   Called from:
;;     00022E98 (in fn00022E94)
;;     00022E98 (in fn00022E94)
fn00022FA0 proc
	{ r21 = add(r25,#0xFFFFFFF7); p0 = cmp.gtu(r9,#0x9); if (!p0.new) jump:nt fn00022DE0 }

;; fn00022FA8: 00022FA8
;;   Called from:
;;     00022FA0 (in fn00022FA0)
;;     00022FA0 (in fn00022FA0)
;;     00022FA0 (in fn00022FA0)
fn00022FA8 proc
	{ r16 = add(r15,r23); p0 = cmp.gtu(r6,#0x0); if (!p0.new) jump:nt 00022DE8 }

;; fn00022FAC: 00022FAC
;;   Called from:
;;     00022EA4 (in fn00022EA4)
fn00022FAC proc
	{ p0 = cmp.gtu(r6,#0x0); if (!p0.new) jump:nt 00022DEC }

;; fn00022FB0: 00022FB0
;;   Called from:
;;     00022FAC (in fn00022FAC)
;;     00022FAC (in fn00022FAC)
;;     00022FAC (in fn00022FAC)
fn00022FB0 proc
	{ r17 = add(r16,r23); r20 = add(r25,#0xFFFFFFEE); p0 = cmp.gtu(r6,#0x1); if (!p0.new) jump:nt 00022DF4 }

;; fn00022FB4: 00022FB4
;;   Called from:
;;     00023358 (in fn00023350)
fn00022FB4 proc
	{ r20 = add(r25,#0xFFFFFFEE); p0 = cmp.gtu(r6,#0x1); if (!p0.new) jump:nt fn00022DF8 }

;; fn00022FBC: 00022FBC
;;   Called from:
;;     00022FB4 (in fn00022FB4)
;;     00022FB4 (in fn00022FB4)
;;     00022FB4 (in fn00022FB4)
fn00022FBC proc
	{ loop1(00022FE0,#0x8); r26 = #0x0; p0 = cmp.gtu(r6,#0x2); if (!p0.new) jump:nt gemvmpybbw_asm }

l00022FC8:
	{ nop; nop }

;; fn00022FCC: 00022FCC
;;   Called from:
;;     00022FC8 (in fn00022FBC)
;;     00023338 (in fn00023334)
fn00022FCC proc
	{ nop }
	{ nop; nop; nop; nop }
	{ r15 = #0x30; r0 = r0 }
	{ r0 = #0x31; r0 = r0 }
	{ r18 = #0xFFFFFFE6; r1 = #0x31; r0 = r0 }

;; fn00022FEC: 00022FEC
;;   Called from:
;;     00022FD4 (in fn00022FCC)
;;     000233AC (in fn000233AC)
fn00022FEC proc
	{ r1 = #0x31; r0 = r0 }
	{ loop0(00023020,#0x5); p0 = cmp.gtu(r9,#0x4); if (!p0.new) jump:nt fn00022E38 }

;; fn00022FF8: 00022FF8
;;   Called from:
;;     00022FDC (in fn00022FBC)
;;     00022FF0 (in fn00022FEC)
;;     00022FF0 (in fn00022FEC)
fn00022FF8 proc
	{ p0 = cmp.gt(r10,#-0x1); if (p0.new) jump:nt fn00022C88; p0 = cmp.gtu(r5,#0x5); if (!p0.new) jump:nt 00022E40 }

l00023000:
	{ p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt 00022C94; p0 = cmp.gtu(r4,#0x6); if (!p0.new) jump:nt 00022E4C }
	{ p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 00022C9C; p0 = cmp.gtu(r2,#0x4); if (!p0.new) jump:nt fn00022E50; r5 = #0x25; jump 00022CD4 }
	{ nop; nop; nop }
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 00022CB4; r26 = add(r26,#0x20); p0 = cmp.gtu(r2,#0x5); if (!p0.new) jump:nt 00022E68; r6 = #0x26; jump 00022CEC }

;; fn00023030: 00023030
;;   Called from:
;;     000233A0 (in fn0002338C)
;;     000233A0 (in fn0002338C)
fn00023030 proc
	{ p0 = cmp.eq(r7,#-0x1); if (p0.new) jump:nt 00022CC4; p0 = cmp.gt(r8,#-0x1); if (p0.new) jump:nt 00022CC4; p0 = cmp.gtu(r2,#0x6); if (!p0.new) jump:nt 00022E7C; r8 = #0x28; jump 00022CFC }

;; fn0002303C: 0002303C
;;   Called from:
;;     000231F8 (in biasadd_relu_requant_nonaligned_hvx)
fn0002303C proc
	{ r8 = #0x28; jump fn00022D08 }

;; fn00023040: 00023040
;;   Called from:
;;     0002303C (in fn0002303C)
;;     0002303C (in fn0002303C)
;;     00023200 (in biasadd_relu_requant_nonaligned_hvx)
fn00023040 proc
	{ p0 = cmp.gt(r9,#-0x1); if (p0.new) jump:nt 00022CD4; p0 = cmp.gtu(r2,#0x4); if (!p0.new) jump:nt fn00022E88; r5 = #0x25; jump fn00022D0C; r3 = #0x39; r0 = add(r0,r0) }
	{ if (p2.new) jump:nt 000230E0; p2 = cmp.eq(r26,r24); p0 = cmp.gtu(r2,#0x5); if (!p0.new) jump:nt 00022E98; r6 = #0x26; jump 00022D1C }

;; fn00023054: 00023054
;;   Called from:
;;     00023440 (in gvmaccb_asm)
fn00023054 proc
	{ p2 = cmp.eq(r26,r24); p0 = cmp.gtu(r2,#0x5); if (!p0.new) jump:nt fn00022E9C; r6 = #0x26; jump fn00022D20 }

l00023060:
	{ p0 = cmp.gt(r7,#-0x1); if (p0.new) jump:nt 00022CF4 }
	{ p0 = cmp.gtu(r2,#0x6); if (!p0.new) jump:nt 00022EB4; r8 = #0x28; jump 00022CB4 }
	{  }
	{ p0 = cmp.gtu(r2,#0x4); if (!p0.new) jump:nt 00022EBC; r5 = #0x25; jump 00022D40; r3 = #0x39; r0 = add(r0,r0) }
	{  }
	{ p0 = cmp.gtu(r2,#0x5); if (!p0.new) jump:nt 00022ECC; r6 = #0x26; jump 00022D50 }

;; fn00023088: 00023088
;;   Called from:
;;     00023248 (in biasadd_relu_requant_nonaligned_hvx)
fn00023088 proc
	{ r6 = #0x26; jump 00022D54 }
0002308C                                     4A 42 A7 19             JB..
00023090 4B 43 A8 19 26 46 72 19 67 E8 E7 1E 4B 43 A9 19 KC..&Fr.g...KC..
000230A0 24 44 72 19 67 65 E4 1E 07 F0 93 2B 4B 43 AA 19 $Dr.ge.....+KC..
000230B0 1A 4C 1A B0 25 45 72 19 67 E6 E7 1E 4A 43 A7 19 .L..%Er.g...JC..

l000230C0:
	{ p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt 00022D54; p0 = cmp.gtu(r2,#0x6); if (!p0.new) jump:nt fn00022F0C; r8 = #0x28; jump 00022D8C }

l000230CC:
	{ p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt maxpool_nonaligned_hvx; p0 = cmp.gtu(r2,#0x4); if (!p0.new) jump:nt 00022F14; r5 = #0x25; jump 00022D98; r3 = #0x39; r0 = add(r0,r0) }

;; fn000230DC: 000230DC
;;   Called from:
;;     000234D4 (in fn000234D4)
fn000230DC proc
	{ nop }
	{ r3 = add(r3,#0x1) }
	{ p0 = cmp.eq(r3,r5); if (!p0.new) jump:t fn00022F80 }

l000230E8:
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+40) }
	{ dealloc_return }
000230FC                                     00 00 00 00             ....

;; im2col7732_asm: 00023100
;;   Called from:
;;     00025228 (in gvconv2dbbb_asm)
im2col7732_asm proc
	{ r2 = vsplatb(r2); allocframe(#0x40) }
	{ r7 = #0xA0; memd(r29) = r17:r16; memd(r29+8) = r19:r18 }
	{ memd(r29+16) = r21:r20; memd(r29+24) = r23:r22 }
	{ M1 = r7; r7 = #0x20; memd(r29+32) = r25:r24; memd(r29+40) = r27:r26 }
	{ M0 = r7; r15 = #0xFFFFFFFE; r3 = #0x0; r0 = r0 }
	{ r15 += add(r4,r4); p0 = cmp.eq(r2,#-0x1); if (p0.new) jump:nt 00022D8C; r3 = #0x0; r1 = add(r1,#0x1) }

l00023138:
	{ r3 = #0x0; r1 = add(r1,#0x1) }

l0002313C:
	{ immext(#0x1010100); r7 = #0x1010101; r16 = #0x2A0; r3 = #0x0; r2 = and(r2,#0x1) }
	{ r9 = asl(r7,#0x2); r10 = asl(r7,#0x3); r8 = add(r7,r7); r3 = #0x0; r3 = add(r3,#-0x1) }

l00023158:
	{ r3 = #0x0; r3 = add(r3,#-0x1) }
	{ r12 = asl(r10,#0x2); r13 = asl(r10,#0x3); r11 = add(r10,r10); r3 = #0x0; r4 = sxth(r4) }
	{ r14 = asl(r11,#0x3); p0 = and(p0,p0); r23 = add(r4,r5); r3 = #0x0; r5 = sxtb(r5) }

;; fn00023178: 00023178
;;   Called from:
;;     00023168 (in fn00022E50)
;;     00023338 (in fn00023334)
fn00023178 proc
	{ r3 = #0x0; r5 = sxtb(r5) }
	{ r15 = mpyi(r15,r16); p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt fn00022E0C; p2 = cmp.eq(r4,#0x0); r3 = #0x0; r6 = zxth(r6) }

;; fn00023180: 00023180
;;   Called from:
;;     000234EC (in gvmaccb_asm)
fn00023180 proc
	{ p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt fn00022E10; p2 = cmp.eq(r4,#0x0); r3 = #0x0; r6 = zxth(r6) }

l00023188:
	{ r3 = #0x0; r6 = zxth(r6) }

;; fn0002318C: 0002318C
;;   Called from:
;;     00022E84 (in fn00022E88)
;;     0002317C (in fn00023178)
;;     00023180 (in fn00023180)
;;     00023180 (in fn00023180)
;;     00023188 (in gvmaccb_asm)
fn0002318C proc
	{ p1 = and(p1,p1); r22 = r4; r0 = add(r0,r15); r3 = #0x0; r7 = add(r7,#0xFF) }
	{ r19 = #0xFFFFFFEB; r15 = r0; r18 = add(r1,#0x0); r6 = mux(p2,r14,r8) }

;; fn000231A4: 000231A4
;;   Called from:
;;     00022E9C (in fn00022E9C)
;;     0002319C (in fn0002318C)
fn000231A4 proc
	{ r18 = add(r1,#0x0); r6 = mux(p2,r14,r8) }

;; fn000231AC: 000231AC
;;   Called from:
;;     00022EA4 (in fn00022EA4)
;;     000231A4 (in fn000231A4)
fn000231AC proc
	{ loop1(000231C0,#0x7); r21 = #0x0; r15 = add(r15,#0xFFFFFFFA); r16 = add(r15,#0x29A) }
	{ nop }
	{ p0 = cmp.eq(r6,#-0x1); if (p0.new) jump:nt fn00022E50; r15 = add(r15,#0x60); r15 = #0x0; r0 = r0 }

l000231C4:
	{ r15 = add(r15,#0x60); r15 = #0x0; r0 = r0 }

;; fn000231CC: 000231CC
;;   Called from:
;;     00022EC4 (in fn00022610)
;;     000231C0 (in fn000231AC)
fn000231CC proc
	{ p3 = cmp.eq(r21,#0x5); r15 = #0x2F; jump fn00022E0C; r0 = #0x1; r0 = r0 }
000231D8                         10 4C 10 B0 11 40 00 78         .L...@.x
000231E0 66 4A 09 F4 22 EF E2 1E 46 46 0E F4 35 40 15 B0 fJ.."...FF..5@..
000231F0 22 C2 73 19 48 40 A7 19                         ".s.H@..        

l000231F8:
	{ r17 = #0xFFFFFFE6; p0 = cmp.gtu(r1,#0x2); if (!p0.new) jump:nt fn0002303C }

l00023200:
	{ loop0(00023220,#0x4); p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt fn00022E94; p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt fn00023040; r1 = #0x21; jump 00022E08 }

l00023210:
	{ nop; nop; nop; nop }
	{ p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 00022EB4; p0 = cmp.gtu(r1,#0x2); if (!p0.new) jump:nt 00023064; r2 = #0x39; r8 = add(r8,r8) }
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 00022EC0; p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt 0002306C; r1 = #0x21; jump 00022EFC }
	{ p0 = cmp.eq(r11,#-0x1); if (p0.new) jump:nt 00022ECC; p0 = cmp.gtu(r1,#0x2); if (!p0.new) jump:nt 0002307C; r2 = #0x39; r8 = add(r8,r8) }
	{ p0 = cmp.eq(r12,#-0x1); if (p0.new) jump:nt 00022ED4; p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt 00023084; r1 = #0x21; jump 00022F18 }

l00023248:
	{ p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt fn00023088; r1 = #0x21; jump fn00022F1C }

l00023250:
	{ p0 = cmp.eq(r13,#-0x1); if (p0.new) jump:nt 00022EE4; p0 = cmp.gtu(r1,#0x2); if (!p0.new) jump:nt 00023094; r2 = #0x39; r0 = add(r0,r0) }
	{ p0 = cmp.eq(r7,#-0x1); if (p0.new) jump:nt 00022EF0; p0 = cmp.eq(r7,#-0x1); if (p0.new) jump:nt 00022EEC; p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt 0002309C; r1 = #0x21; jump 00022EF8 }
	{ p0 = cmp.eq(r14,#-0x1); if (p0.new) jump:nt 00022EFC; r17 = #0xFFFFFFE6; p0 = cmp.gtu(r1,#0x2); if (!p0.new) jump:nt 000230B0; r2 = #0x9; r1 = add(r1,#0x1) }
	{ p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt fn00022F10; p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt 000230BC; r1 = #0x21; jump fn00022E84; r2 = #0x39; r8 = add(r8,r8) }

l00023280:
	{ p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt 000230C0; r1 = #0x21; jump fn00022E88; r2 = #0x39; r8 = add(r8,r8) }

l0002328C:
	{ r15 = add(r0,#0x540); r18 = add(r1,#0x0); r6 = r8; r21 = #0x0 }
	{ loop1(000232C0,#0x7); r19 = #0xFFFFFFEB; r15 = add(r15,#0xFFFFFFFA); r16 = add(r15,#0x29A) }
	{ nop }
	{ nop; nop; nop; nop }
	{ p0 = cmp.eq(r6,#-0x1); if (p0.new) jump:nt 00022F50; r15 = add(r15,#0x60); r15 = #0x0; r0 = r0 }
	{ r16 = add(r16,#0x60); r15 = #0x2F; jump fn00022F0C; r0 = #0x1; r0 = r0 }

l000232D0:
	{ r15 = #0x2F; jump fn00022F10; r0 = #0x1; r0 = r0 }
000232D8                         A3 40 15 75 22 EF E2 1E         .@.u"...
000232E0 66 4A 09 F4 D1 7A DF 78 22 C2 73 19 28 41 00 69 fJ...z.x".s.(A.i
000232F0 48 51 A7 19 35 40 15 B0 21 C1 71 19 00 C0 00 7F HQ..5@..!.q.....
00023300 D1 7C DF 78 22 C2 71 19 4B 51 A8 19 21 41 71 19 .|.x".q.KQ..!Aq.
00023310 05 E1 E2 1E 4B 51 A9 19 22 42 71 19 05 F8 92 2B ....KQ.."Bq....+
00023320 4B 51 AA 19 21 41 71 19 68 E1 E2 1E 4B 51 AB 19 KQ..!Aq.h...KQ..
00023330 22 42 71 19                                     "Bq.            

;; fn00023334: 00023334
;;   Called from:
;;     000234F4 (in gvmaccb_asm)
;;     000236F0 (in vmemset_asm)
fn00023334 proc
	{ r2 = #0x39; r8 = add(r8,r8) }
	{ p0 = cmp.gt(r13,#-0x1); if (p0.new) jump:nt fn00022FCC; p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt fn00023178; r1 = #0x21; jump 0002300C }

l00023344:
	{ p0 = cmp.gt(r12,#-0x1); if (p0.new) jump:nt 00022FD4; p0 = cmp.gt(r14,#-0x1); if (p0.new) jump:nt 00022FD4; p0 = cmp.gtu(r1,#0x2); if (!p0.new) jump:nt 00023188; r2 = #0x9; r1 = add(r1,#0x1) }

;; fn00023350: 00023350
;;   Called from:
;;     00023600 (in biasadd_relu_requant_nonaligned_hvx)
fn00023350 proc
	{ r2 = #0x9; r1 = add(r1,#0x1) }
	{  }
	{ r18 = add(r18,#0x120); r1 = #0x21; jump fn00022FB4; r2 = #0x9; r0 = r0 }
00023364             48 91 A7 19 21 81 71 19 0E D0 92 2B     H...!.q....+
00023370 73 7D DF 78 0F 50 A0 B0                         s}.x.P..        

;; fn00023378: 00023378
;;   Called from:
;;     000236F0 (in vmemset_asm)
fn00023378 proc
	{ r18 = add(r1,#0x0); r21 = #0x0 }
	{ loop1(000233A0,#0x7); r6 = mux(p1,r14,r8); r15 = add(r15,#0xFFFFFFFA); r16 = add(r15,#0x29A) }

;; fn0002338C: 0002338C
;;   Called from:
;;     00023380 (in fn00023378)
;;     00023638 (in biasadd_relu_requant_nonaligned_hvx)
fn0002338C proc
	{ r16 = add(r15,#0x29A) }
	{ nop; nop; nop; nop }
	{ p0 = cmp.eq(r6,#-0x1); if (p0.new) jump:nt fn00023030; r15 = add(r15,#0x60); r15 = #0x0; r0 = r0 }

;; fn000233AC: 000233AC
;;   Called from:
;;     0002338C (in fn0002338C)
;;     000233A0 (in fn0002338C)
fn000233AC proc
	{ r16 = add(r16,#0x60); r15 = #0x2F; jump fn00022FEC; r0 = #0x1; r0 = r0 }
000233B8                         A3 40 15 75 35 40 15 B0         .@.u5@..
000233C0 22 EF E2 1E 91 75 DF 78 22 C2 73 19 48 53 A7 19 "....u.x".s.HS..
000233D0 66 4A 09 F4 21 C1 71 19 30 42 00 69 26 46 0E F4 fJ..!.q.0B.i&F..
000233E0 D1 7C DF 78 22 C2 71 19 00 40 00 7F 00 C0 00 7F .|.x".q..@......
000233F0 00 40 00 7F 00 40 00 7F 00 40 00 7F 00 C0 00 7F .@...@...@......
00023400 4B 53 A8 19 21 41 71 19 05 E1 E2 1E 4B 53 A9 19 KS..!Aq.....KS..
00023410 22 42 71 19 05 F8 92 2B 4B 53 AB 19 4A 53 AD 19 "Bq....+KS..JS..
00023420 21 41 71 19 68 E1 E2 1E 4B 53 AA 19 48 53 AC 19 !Aq.h...KS..HS..
00023430 22 42 71 19 08 D9 92 28 12 64 12 B0 21 41 71 19 "Bq....(.d..!Aq.

l00023440:
	{ r1 = #0x21; jump fn00023054; r2 = #0x9; r8 = r8 }
00023448                         49 53 AE 19 22 42 71 19         IS.."Bq.
00023450 0B F0 92 2B 4A 54 A7 19 48 53 A7 19 21 41 71 19 ...+JT..HS..!Aq.
00023460 2E E1 E2 1E 00 80 00 7F 22 82 71 19 0E D0 92 2B ........".q....+
00023470 0F 78 E0 B0 12 40 01 B0 15 C0 00 78 2B 42 20 69 .x...@.....x+B i
00023480 06 48 0E F4 4F FF EF BF 00 40 00 7F 00 C0 00 7F .H..O....@......
00023490 00 40 00 7F 00 40 00 7F 00 40 00 7F 00 C0 00 7F .@...@...@......
000234A0 49 50 A6 19 0F 4C 0F B0 E1 C0 0F 28 A3 40 15 75 IP...L.....(.@.u
000234B0 51 40 00 78 21 6F E1 1E E2 EF 03 1E 28 42 00 69 Q@.x!o......(B.i
000234C0 48 55 A7 19 66 4A 09 F4 21 C1 71 19 D1 FC DF 78 HU..fJ..!.q....x
000234D0 4B 55 A9 19                                     KU..            

;; fn000234D4: 000234D4
;;   Called from:
;;     0002388C (in fn0002388C)
;;     00023890 (in fn0002388C)
fn000234D4 proc
	{ r21 = add(r21,#0x1); r6 = mux(p0,r14,r6); r1 = #0x21; jump fn000230DC }
000234E0 4B 55 A8 19 21 41 71 19                         KU..!Aq.        

l000234E8:
	{ r2 = #0x9; r9 = add(r9,#0x1) }
	{ p0 = cmp.gt(r10,#-0x1); if (p0.new) jump:nt fn00023180; r2 = #0x39; r8 = add(r8,r8) }

l000234F4:
	{ p0 = cmp.gt(r11,#-0x1); if (p0.new) jump:nt 00023188; r18 = add(r18,#0x80); p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt fn00023334; r1 = #0x21; jump 000231C4 }

l00023504:
	{ p0 = cmp.gt(r12,#-0x1); if (p0.new) jump:nt 00023194; r2 = #0x39; r8 = add(r8,r8) }
	{ p0 = cmp.gt(r13,#-0x1); if (p0.new) jump:nt 000231A0; p0 = cmp.gt(r14,#-0x1); if (p0.new) jump:nt 0002319C; p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt 0002334C; r1 = #0x21; jump 00023120 }
	{ r2 = #0x39; r0 = add(r0,r0) }
	{  }
	{ p0 = cmp.gt(r7,#-0x1); if (p0.new) jump:nt 000231B4; p0 = cmp.gtu(r1,#0x1); if (!p0.new) jump:nt 00023364; r1 = #0x21; jump fn00023180 }

l00023530:
	{ p0 = cmp.gt(r9,#-0x1); if (p0.new) jump:nt 000231C4; r1 = #0x21; jump 00023138; r2 = #0x39; r0 = r0 }

l0002353C:
	{ r22 = add(r22,#0x1) }
	{ p2 = and(p2,p2); p0 = cmp.eq(r22,r23); r1 = add(r1,#0x4600) }
	{ if (!p0) jump:t 0002319C; p1 = cmp.gt(r22,#0x6E); r0 = add(r0,#0x540); p0 = cmp.gt(r22,#0x6D) }
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+40) }
	{ dealloc_return }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; biasadd_relu_requant_nonaligned_hvx: 00023580
biasadd_relu_requant_nonaligned_hvx proc
	{ r8 = add(r4,#0x7F); immext(#0x1010100); r14 = #0x1010101; allocframe(#0x20) }
	{ r15 = add(r14,r14); r7 = r4; memd(r29) = r17:r16; memd(r29+8) = r19:r18 }
	{ r17 = asl(r4,#0x2); p0 = cmp.eq(r5,#-0x1); if (p0.new) jump:nt 000231F8; r6 = and(r4,#0x7F); r16 = #0x1 }

l000235AC:
	{ r8 = lsr(r8,#0x7); r16 = combine(r17.l,r16.l); p2 = cmp.eq(r6,#0x0); r6 = add(r6,#0xFFFFFF80) }
	{ loop1(000235E0,r3); r18 = addasl(r1,r4,#0x5); if (p2) r6 = #0x0 }
	{ immext(#0x8000); r10 = #0x8000; l2fetch(r18,r17:r16) }
	{ nop; nop; nop }
	{ loop0(00023600,r8); r9 = r2; r1 = #0x10; r1 = add(r1,#0x1) }
	{ r18 = addasl(r18,r4,#0x2); p0 = cmp.eq(r0,r4); if (!p0.new) jump:nt 0002360C; r9 = #0x10; r1 = add(r1,#0x1) }

l000235F8:
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 00023248; r1 = #0x10; r1 = add(r1,#0x1) }

l00023600:
	{ p1 = cmp.eq(r15,r0); if (p1.new) jump:t fn00023350; r12 = and(r0,#0x7F); p0 = cmp.eq(r1,r5); if (!p0.new) jump:nt 00023620; r9 = #0x10; r1 = add(r1,#0x1) }

l0002360C:
	{ r9 = #0x10; r1 = add(r1,#0x1) }

l00023610:
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 00023260; r11 = r7; r1 = #0x10; r1 = add(r1,#0x1) }
	{ p1 = cmp.eq(r15,r1); if (p1.new) jump:t 0002336C; p0 = cmp.gt(r7,#0x7F); p0 = cmp.eq(r2,r6); if (!p0.new) jump:nt 00023640; r9 = #0x10; r1 = add(r1,#0x1) }

l00023620:
	{ p0 = cmp.gt(r7,#0x7F); p0 = cmp.eq(r2,r6); if (!p0.new) jump:nt 00023644; r9 = #0x10; r1 = add(r1,#0x1) }

l0002362C:
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 00023280; if (p0) r11 = #0x80; r1 = #0x10; r1 = add(r1,#0x1) }

l00023638:
	{ p1 = cmp.eq(r15,r2); if (p1.new) jump:t fn0002338C; r9 = r8; jump 000232D0; p0 = cmp.eq(r3,r7); if (!p0.new) jump:nt 0002365C; r9 = #0x10; r1 = add(r1,#0x1) }

l00023644:
	{ r9 = #0x10; r1 = add(r1,#0x1) }

l00023648:
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 0002329C; r12 = add(r12,r11); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 000232D0 }
	{ p1 = cmp.eq(r15,r3); if (p1.new) jump:t 000233A8; r13 = sub(#0x0,r0); r5 = r14; p0 = cmp.eq(r12,#-0x1); if (p0.new) jump:nt 000232DC }
	{ p1 = !cmp.gt(r12,0000007F); if (p1.new) r5 = add(r15,#0x0); r1 = #0x10; r1 = add(r1,#0x1) }
	{ r7 = add(r7,#0xFFFFFF80); r11 = r10; jump 00023308; r0 = #0x0; jump 00023694 }
	{ p0 = cmp.gt(r14,#-0x1); if (p0.new) jump:nt 000233E4; p0 = cmp.eq(r14,#-0x1); if (p0.new) jump:nt 000233E4; p0 = cmp.eq(r0,r4); if (!p0.new) jump:nt 0002369C; r9 = #0x10; r1 = add(r1,#0x1) }
	{ p0 = cmp.gtu(r15,#0x2); if (!p0.new) jump:t 00023374; r12 = r13; jump 000237E8 }
	{ p0 = cmp.eq(r5,#-0x1); if (p0.new) jump:nt 00023324; p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 000232E4; r1 = #0x10; r1 = add(r1,#0x1) }
	{ p0 = cmp.gt(r5,#-0x1); if (p0.new) jump:nt 00023330; p0 = cmp.gtu(r13,#0xE); if (!p0.new) jump:nt 0002330C }
	{ r0 = #0x8; r9 = add(r9,#0x1) }
	{ nop; r0 = #0x18; r1 = add(r1,#0x1) }
	{ r1 = addasl(r1,r6,#0x2); r7 = r4; r0 = add(r0,r6) }
	{ r1 = add(r1,#0xFFFFFF00); nop; l2fetch(r18,r17:r16) }
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ dealloc_return }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; vmemset_asm: 000236E0
;;   Called from:
;;     0000CFCC (in fn0000CFB0)
;;     0000CFE8 (in pad2d)
;;     0000D784 (in im2col_co)
;;     0000D7EC (in im2col_co)
;;     0000D894 (in im2col_co)
;;     0000D8D8 (in im2col_co)
;;     0000D91C (in im2col_co)
;;     0000D974 (in im2col_co)
;;     0000DED0 (in im2col_slice_v0_co)
;;     0000DF04 (in im2col_slice_v0_co)
;;     0000DF24 (in im2col_slice_v0_co)
;;     0000DFEC (in im2col_slice_v0_co)
;;     0000E0B4 (in im2col_slice_co)
;;     0000E35C (in fast_im2col_co)
;;     0000E390 (in fast_im2col_co)
;;     0000E3B8 (in fast_im2col_co)
vmemset_asm proc
	{ r1 = vsplatb(r1); immext(#0x1010100); r8 = #0x1010101; r7 = add(r2,r0) }
	{ p0 = cmp.eq(r1,#-0x1); if (p0.new) jump:nt fn00023334; r9 = add(r8,r8); r7 = and(r7,#0x7F); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt fn00023378 }

l00023700:
	{ r2 -= add(r7,#0xFFFFFF81); p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 00023444; r5 = and(r0,#0x7F); p0 = cmp.eq(r7,#-0x1); if (p0.new) jump:nt 00023388 }
	{ r2 = lsr(r2,#0x7); p0 = cmp.gt(r9,#-0x1); if (p0.new) jump:nt 00023458; r5 = add(r5,r2); r0 = #0x0; jump 00023730 }
	{ loop0(00023740,r2); p0 = cmp.gtu(r8,#0x0); if (!p0.new) jump:t 000233E4; p2 = cmp.gt(r5,#0x7F); if (!p2.new) r9 = add(r8,#0x0) }
	{ p0 = tstbit(r9,#0x0); if (p0.new) jump:nt 000233C0; p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 000233C0 }
	{ nop; nop }
	{ r0 = #0x0; jump 00023768; r0 = #0x18; r1 = add(r1,#0x1) }
	{ r0 = #0x8; r8 = r8 }
	{ jumpr r31 }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

l00023758:
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gvmaccimw_asm: 00023760
;;   Called from:
;;     0002375C (in gvconv2dbbb_asm)
gvmaccimw_asm proc
	{ r6 = memd(r29); r7 = memd(r29+4) }
	{ r9 = lsr(r5,#0x4); allocframe(#0x10) }
	{ r9 = add(r9,#0xFFFFFFFF); memd(r29) = r17:r16; memd(r29+8) = r19:r18 }
	{ nop; nop; nop }

l00023780:
	{ loop1(000237A0,r2); r6 = add(r6,#0xFFFFFFFF) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ loop0(000237C0,r9); r10 = r0; r0 = add(r0,r4) }
	{ r17:r16 = combine(#0x0,#0x0); r15:r14 = memd(r10+8); r13:r12 = memd(r10++#16) }
	{ nop; nop }
	{ r17:r16 += vraddub(r15:r14,r13:r12); r15:r14 = memd(r10+8); r13:r12 = memd(r10++#16) }
	{ r17:r16 += vraddub(r15:r14,r13:r12) }
	{ r16 = add(r16,r17); r11 = memw(r1) }
	{ r11 += mpyi(r16,r7) }
	{ nop; nop; memw(r1++#4) = r11 }
	{ p1 = cmp.eq(r6,#0x0); if (!p1.new) jump:t 00023780; r0 = add(r0,r3) }

l000237F0:
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ dealloc_return }
000237F8                         00 00 00 00 00 00 00 00         ........

;; gvmaccb_asm: 00023800
gvmaccb_asm proc
	{ r2 = lsr(r2,#0x2); p0 = cmp.eq(r3,#-0x1); if (p0.new) jump:nt 00023440; immext(#0x1010100); r4 = #0x1010101 }

l00023810:
	{ p0 = cmp.eq(r3,#0x0); if (p0.new) jump:t 00023848; r2 = add(r2,#0xFFFFFFFF) }

l00023818:
	{ loop0(00023840,r2); p0 = cmp.gtu(r4,#0x2); if (p0.new) jump:nt 000238D8; r5 = #0x10; r0 = #0x10; r1 = add(r1,#0x1) }

l00023828:
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ p0 = cmp.gtu(r4,#0x2); if (p0.new) jump:t gvmmacbbw_asm; r0 = #0x10; r1 = add(r1,#0x1) }

l00023848:
	{ r0 = r1; jump fn0002388C }
0002384C                                     04 C0 01 28             ...(
00023850 E3 C3 65 19 A3 E0 21 1C 04 44 43 1C 22 C0 21 28 ..e...!..DC.".!(
00023860 00 C0 9F 52 00 00 00 00 00 00 00 00 00 00 00 00 ...R............
00023870 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gvmaddvvm_asm: 00023880
gvmaddvvm_asm proc
	{ r4 = asl(r4,#0x2); immext(#0x80000000); r11 = #0x80000000; r1 = #0x0; r0 = r0 }

;; fn0002388C: 0002388C
;;   Called from:
;;     00023848 (in gvmaccb_asm)
;;     00023880 (in gvmaddvvm_asm)
fn0002388C proc
	{ r1 = #0x0; r0 = r0 }
	{ p0 = cmp.eq(r11,#-0x1); if (p0.new) jump:nt fn000234D4; r8 = r2; r3 = r3; r6 = memd(r29) }

;; fn0002389C: 0002389C
;;   Called from:
;;     0002388C (in fn0002388C)
;;     00023890 (in fn0002388C)
fn0002389C proc
	{ p2 = !cmp.eq(r6,00000000); r7 = #0x4; r5 = memuh(r0); r0 = memb(r0); dcfetch(r0,#0x20) }
	{ M0 = r4; r3 = #0x23; jump fn00023A70; p1 = cmp.eq(r0,#0x3); if (p1.new) jump:nt 00023934; r10 = memw(r0++#4) }
000238BC                                     08 42 03 60             .B.`
000238C0 20 40 AA 19 01 44 41 1C 41 C0 02 2B 00 C0 00 7F  @...DA.A..+....
000238D0 00 40 00 7F 00 40 00 7F                         .@...@..        

l000238D8:
	{ nop; nop }
	{ r5 = r2; jump 000234E8; p0 = cmp.eq(r1,r0); if (!p0.new) jump:nt 000238E4; r10 = memw(r0++#4); r8 = #0x36; r0 = r0 }

l000238E4:
	{ p0 = cmp.eq(r1,r0); if (!p0.new) jump:nt 000238E8; r10 = memw(r0++#4); r8 = #0x36; r0 = r0 }

l000238E8:
	{ r10 = memw(r0++#4); r8 = #0x36; r0 = r0 }

l000238F0:
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 00023530; p0 = cmp.eq(r1,r4); if (!p0.new) jump:nt 000238F0; r2 = memuh(r0); r0 = memb(r4); dcfetch(r0,#0x40) }

l00023900:
	{ loop0(00023914,#0x5); r5 = r2; jump 00023508; p0 = cmp.eq(r1,r0); if (!p0.new) jump:nt 00023904; r8 = #0x6; r0 = r0 }
	{ r5 = r2; jump 0002351C }
	{ p1 = cmp.gtu(r7,#0x6); if (p1.new) jump:t 000237E0 }
	{ r7 = add(r7,r7); r7 = r6; jump 00023524 }
	{ r5 = #0x2; r0 = r0 }
	{ jumpr r31 }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gvmmacbbw_asm: 00023940
;;   Called from:
;;     00023840 (in gvmaccb_asm)
gvmmacbbw_asm proc
	{ r5 = asl(r5,#0x2); r6 = memd(r29); r7 = memd(r29+4) }
	{ r8 = memw(r29+8); allocframe(#0x40) }
	{ M0 = r5; memd(r29) = r17:r16; memd(r29+8) = r19:r18 }
	{ r5 -= mpyi(r5,#0x3); r9 = lsr(r7,#0x4); memd(r29+16) = r21:r20; memd(r29+24) = r23:r22 }
	{ r24 = add(r3,#0x3); p0 = cmp.eq(r12,r12); if (!p0.new) jump:nt 00023B3C; memd(r29+32) = r25:r24; memd(r29+40) = r27:r26 }

l00023974:
	{ M1 = r5; r24 = lsr(r24,#0x2); r9 = add(r9,#0xFFFFFFFF) }

;; fn00023980: 00023980
;;   Called from:
;;     00023974 (in gvmmacbbw_asm)
;;     00023B04 (in fn00023A00)
fn00023980 proc
	{ loop1(000239A0,r24); r23 = r3; r8 = add(r8,#0xFFFFFFFF); r10 = r1 }
	{ nop; nop; nop; nop }
	{ r10 = memuh(r0); r2 = memb(r0+2); dcfetch(r0,#0x40) }
	{ r21 = r0; r20 = add(r0,r6); r10 = memuh(r0); deallocframe); dcfetch(r20,#0x40) }
	{ r15:r14 = memd(r21+8); r2 = #0x30; r0 = r0 }
	{ r11 = addasl(r20,r6,#0x1); r22 = add(r20,r6); r13:r12 = memd(r21++#16); r2 = #0x30; r0 = r0 }
	{ r17:r16 = memd(r20+8); r2 = #0x30; r0 = r0 }
	{ loop0(fn00023A00,r9); r0 = addasl(r0,r6,#0x2); r19:r18 = memd(r20++#16); r2 = #0x30; r0 = add(r0,r0) }
	{ nop; nop }
	{ nop; nop; nop; nop }

;; fn00023A00: 00023A00
;;   Called from:
;;     000239F0 (in fn00023980)
;;     00023DC0 (in gvmsumb_asm)
fn00023A00 proc
	{ p0 = cmp.gtu(r12,#0x8); if (p0.new) jump:t 00023B00; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00023D00; r10 = memuh(r0); r2 = memb(r0+2); dcfetch(r22,#0x40) }

l00023A10:
	{ p0 = cmp.gtu(r13,#0x9); if (p0.new) jump:t 00023B10; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00023D10; r10 = memuh(r0); deallocframe); dcfetch(r11,#0x40) }
	{ p0 = cmp.gtu(r14,#0xA); if (p0.new) jump:t 00023B20; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t gvmsumimw_asm; r19:r18 = memd(r22+8); r13:r12 = memd(r22++#16) }
	{ p0 = cmp.gtu(r15,#0xB); if (p0.new) jump:t 00023B30; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00023D30; r17:r16 = memd(r11+8); r15:r14 = memd(r11++#16) }
	{ p0 = cmp.gtu(r12,#0x8); if (p0.new) jump:t 00023B44; p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 00023B44; r10 = memuh(r0); r2 = memb(r0+2); dcfetch(r21,#0x40) }
	{ p0 = cmp.gtu(r13,#0x9); if (p0.new) jump:t 00023B54; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 00023B54; r10 = memuh(r0); deallocframe); dcfetch(r20,#0x40) }
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 00023D64; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00023D64; r15:r14 = memd(r21+8); r13:r12 = memd(r21++#16) }

;; fn00023A70: 00023A70
;;   Called from:
;;     000238AC (in fn0002389C)
fn00023A70 proc
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t fn00023D74; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t fn00023D74; r17:r16 = memd(r20+8); r19:r18 = memd(r20++#16) }

l00023A80:
	{ p0 = cmp.gtu(r12,#0x8); if (p0.new) jump:t fn00023B80; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00023D80; r10 = memuh(r0); r2 = memb(r0+2); dcfetch(r22,#0x40) }
	{ p0 = cmp.gtu(r13,#0x9); if (p0.new) jump:t 00023B90; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t fn00023D90; r10 = memuh(r0); deallocframe); dcfetch(r11,#0x40) }
	{ p0 = cmp.gtu(r14,#0xA); if (p0.new) jump:t 00023BA0; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00023DA0; r19:r18 = memd(r22+8); r13:r12 = memd(r22++#16) }
	{ p0 = cmp.gtu(r15,#0xB); if (p0.new) jump:t 00023BB0; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00023DB0; r17:r16 = memd(r11+8); r15:r14 = memd(r11++#16) }
	{ p0 = cmp.gtu(r12,#0x8); if (p0.new) jump:t 00023BC4; p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 00023BC4; p0 = cmp.gt(r23,#0x1); r2 = #0x32; r0 = r0 }
	{ p0 = cmp.gtu(r13,#0x9); if (p0.new) jump:t 00023BD4; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 00023BD4; p0 = cmp.gt(r23,#0x2); r2 = #0x3A; r0 = r0 }
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 00023DE4; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00023DE4 }
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 00023DEC; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00023DEC; p0 = cmp.gt(r23,#0x3); r2 = #0x3A; r0 = r0 }
	{ r23 = add(r23,#0xFFFFFFFC); r10 = r1; r2 = #0x3A; r0 = r0 }

l00023B00:
	{ r2 = #0x3A; r0 = r0 }
	{ if (!p1.new) jump:t fn00023980; r0 = add(r0,r4); p1 = cmp.eq(r8,#0x0) }

l00023B10:
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+40) }
	{ dealloc_return }
00023B24             00 00 00 00 00 00 00 00 00 00 00 00     ............
00023B30 00 00 00 00 00 00 00 00 00 00 00 00             ............    

l00023B3C:
	{ r0 = memw(r0); r0 = memw(r0) }

;; gvmmpybbw_asm: 00023B40
;;   Called from:
;;     00023B3C (in gvmmacbbw_asm)
gvmmpybbw_asm proc
	{ r5 = asl(r5,#0x2); r6 = memd(r29); r7 = memd(r29+4) }
	{ p0 = cmp.eq(r12,r12); if (!p0.new) jump:nt gvmsumimw_asm; r8 = memw(r29+8); allocframe(#0x40) }

l00023B54:
	{ M0 = r5; memd(r29+32) = r25:r24; memd(r29) = r17:r16 }
	{ r9 = lsr(r7,#0x4); r24 = add(r3,#0x3); memd(r29+16) = r21:r20; memd(r29+24) = r23:r22 }
	{ r24 = lsr(r24,#0x2); r9 = add(r9,#0xFFFFFFFF); memd(r29+8) = r19:r18; memd(r29+40) = r27:r26 }
	{ nop }

;; fn00023B80: 00023B80
;;   Called from:
;;     00023B7C (in gvmmpybbw_asm)
;;     00023CE4 (in gvconv2dbbb_asm)
;;     00025354 (in gvconv2dbbb_asm)
fn00023B80 proc
	{ loop1(00023BA0,r24); r23 = r3; r8 = add(r8,#0xFFFFFFFF); r10 = r1 }
	{ nop; nop; nop; nop }

l00023BA0:
	{ r21 = r0; r12 = r12; jump 00023D60; r10 = memuh(r0); r2 = memb(r0+2); dcfetch(r0,#0x40) }
00023BB0 14 46 00 F3 E2 4C 4C 1F 09 67 0A 28 08 C0 14 94 .F...LL..g.(....
00023BC0 2B 54 06 C4 16 46 14 F3 2E 40 D5 91 4C C0 D5 9B +T...F...@..L...
00023BD0 00 41 09 60 40 40 06 C4 30 40 D4 91 52 C0 D4 9B .A.`@@..0@..R...
00023BE0 80 68 0C 19 81 68 12 19 0A 42 0A 29 08 C0 16 94 .h...h...B.)....
00023BF0 80 69 0D 19 81 69 13 19 0B 67 0A 28 08 C0 0B 94 .i...i...g.(....
00023C00 80 6A 0E 19 81 6A 10 19 32 40 D6 91 4C C0 D6 9B .j...j..2@..L...
00023C10 80 6B 0F 19 81 6B 11 19 30 40 CB 91 4E C0 CB 9B .k...k..0@..N...
00023C20 82 68 0C 19 83 68 0E 19 08 42 0A 29 08 C0 15 94 .h...h...B.)....
00023C30 82 69 0D 19 83 69 0F 19 09 67 0A 28 08 C0 14 94 .i...i...g.(....
00023C40 82 6A 12 19 83 6A 10 19 2E 40 D5 91 4C C0 D5 9B .j...j...@..L...
00023C50 82 AB 13 19 83 6B 11 19 30 40 D4 91 52 C0 D4 9B .....k..0@..R...
00023C60 80 68 0C 19 81 68 12 19 0A 42 0A 29 08 C0 16 94 .h...h...B.)....
00023C70 80 69 0D 19 81 69 13 19 0B 67 0A 28 08 C0 0B 94 .i...i...g.(....
00023C80 80 6A 0E 19 81 6A 10 19 32 40 D6 91 4C C0 D6 9B .j...j..2@..L...
00023C90 80 6B 0F 19 81 6B 11 19 30 40 CB 91 4E C0 CB 9B .k...k..0@..N...
00023CA0 82 68 0C 19 83 68 0E 19 20 40 57 75 00 C0 22 2B .h...h.. @Wu.."+
00023CB0 82 69 0D 19 83 69 0F 19 40 40 57 75 01 C0 A2 2B .i...i..@@Wu...+
00023CC0 82 6A 12 19 83 EA 10 19                         .j......        

;; fn00023CC8: 00023CC8
;;   Called from:
;;     00023E68 (in fn00023E4C)
;;     00023E6C (in fn00023E4C)
fn00023CC8 proc
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t fn00023FCC; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t fn00023FCC; p0 = cmp.gt(r23,#0x3); r2 = #0x3A; r0 = r0 }

l00023CD8:
	{ r23 = add(r23,#0xFFFFFFFC); r10 = r1; r2 = #0x3A; r0 = r0 }

l00023CE4:
	{ if (!p1.new) jump:t fn00023B80; r0 = add(r0,r4); p1 = cmp.eq(r8,#0x0) }

l00023CF0:
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+40) }

l00023D00:
	{ dealloc_return }
00023D04             00 00 00 00 00 00 00 00 00 00 00 00     ............
00023D10 00 00 00 00 00 00 00 00                         ........        

l00023D18:
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gvmsumimw_asm: 00023D20
;;   Called from:
;;     00023B48 (in gvmmpybbw_asm)
;;     00023D1C (in fn00023D90)
gvmsumimw_asm proc
	{ r6 = memd(r29); r7 = memd(r29+4) }
	{ r9 = lsr(r5,#0x4); r8 = memw(r29+8); allocframe(#0x10) }
	{ r9 = add(r9,#0xFFFFFFFF); memd(r29) = r17:r16; memd(r29+8) = r19:r18 }
	{ nop; nop }

;; fn00023D40: 00023D40
;;   Called from:
;;     00023D38 (in gvmsumimw_asm)
;;     00023DA8 (in fn00023D90)
;;     00024178 (in fn000241C4)
;;     00024180 (in fn000241C4)
fn00023D40 proc
	{ loop1(00023D60,r2); r6 = add(r6,#0xFFFFFFFF) }
	{ nop; nop }
	{ nop; nop; nop; nop }

l00023D60:
	{ loop0(00023D80,r9); r10 = r0; r0 = add(r0,r4) }

l00023D6C:
	{ r17:r16 = combine(#0x0,#0x0); r15:r14 = memd(r10+8); r13:r12 = memd(r10++#16) }

l00023D70:
	{ r15:r14 = memd(r10+8); r13:r12 = memd(r10++#16) }

;; fn00023D74: 00023D74
;;   Called from:
;;     00023A70 (in fn00023A70)
;;     00023A70 (in fn00023A70)
;;     00023D70 (in fn00023D40)
fn00023D74 proc
	{ r13:r12 = memd(r10++#16) }
	{ nop; nop }
	{ r17:r16 += vraddub(r15:r14,r13:r12); r15:r14 = memd(r10+8); r13:r12 = memd(r10++#16) }

l00023D88:
	{ r13:r12 = memd(r10++#16) }

;; fn00023D8C: 00023D8C
;;   Called from:
;;     00023D80 (in fn00023D74)
;;     00023D88 (in fn00023D74)
fn00023D8C proc
	{ r17:r16 += vraddub(r15:r14,r13:r12) }

;; fn00023D90: 00023D90
;;   Called from:
;;     00023D8C (in fn00023D8C)
;;     00023D8C (in fn00023D8C)
fn00023D90 proc
	{ r16 = add(r16,r17); r11 = r8 }

l00023D98:
	{ r11 += mpyi(r16,r7) }
	{ nop; nop; memw(r1++#4) = r11 }
	{ p1 = cmp.eq(r6,#0x0); if (!p1.new) jump:t fn00023D40; r0 = add(r0,r3) }

l00023DB0:
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ dealloc_return }
00023DB8                         00 00 00 00 00 00 00 00         ........

;; gvmsumb_asm: 00023DC0
;;   Called from:
;;     0001A5B4 (in supernode_execute_hvx_slice)
gvmsumb_asm proc
	{ r2 = lsr(r2,#0x2); p0 = cmp.eq(r3,#-0x1); if (p0.new) jump:nt fn00023A00; p0 = cmp.eq(r3,#0x0) }

l00023DCC:
	{ if (p0) jump:nt 00023E08; immext(#0x1010100); r4 = #0x1010101; r2 = r2 }

l00023DD8:
	{ loop0(00023E00,r2); p0 = cmp.gtu(r4,#0x2); if (p0.new) jump:nt fn00023E98; r5 = #0x10; r0 = #0x10; r1 = add(r1,#0x1) }

l00023DE8:
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ p0 = cmp.gtu(r4,#0x2); if (p0.new) jump:t 00023F00; r0 = #0x10; r1 = add(r1,#0x1) }

l00023E08:
	{ r0 = r1; jump fn00023E4C }
00023E0C                                     E3 C3 65 19             ..e.
00023E10 A3 60 21 1C 22 C0 21 28 00 C0 9F 52 00 00 00 00 .`!.".!(...R....

;; gvconvsum2dbbw_asm: 00023E20
gvconvsum2dbbw_asm proc
	{ r8 = memw(r29); dcfetch(r0,#0x0) }
	{ r5 = asl(r5,#0x2); r6 = memw(r29+8); dcfetch(r0,#0x20) }
	{ r11 = memw(r29+20); allocframe(#0x48) }
	{ r25 = lsr(r8,#0x10); memd(r29+32) = r25:r24; memd(r29) = r17:r16 }
	{ r8 = mpy(r8.h,r8.l); memd(r29+16) = r21:r20; memd(r29+24) = r23:r22 }

;; fn00023E4C: 00023E4C
;;   Called from:
;;     00023E08 (in gvmsumb_asm)
;;     00023E48 (in gvconvsum2dbbw_asm)
fn00023E4C proc
	{ memd(r29+16) = r21:r20; memd(r29+24) = r23:r22 }
	{ M0 = r8; memd(r29+8) = r19:r18; memd(r29+40) = r27:r26 }
	{ r12 = addasl(r8,r8,#0x1); immext(#0x80000000); r16 = #0x80000001; r11 = #0x0; r0 = r0 }
	{ r23 = mpyi(r6,r3); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt fn00023CC8; r12 = sub(#0x10,r12); memw(r29+56) = r4 }

;; fn00023E7C: 00023E7C
;;   Called from:
;;     00023E68 (in fn00023E4C)
;;     00023E6C (in fn00023E4C)
fn00023E7C proc
	{ M1 = r12; r6 = memw(r29+84) }
	{ r13 = asl(r8,#0x1); r12 = add(r12,#0x10); memw(r29+48) = r0; memw(r29+52) = r1 }
	{ r6 = lsr(r6,#0x4); r23 = sub(r23,r13); r13 = sub(r6,r3); memw(r29+60) = r5 }

;; fn00023E98: 00023E98
;;   Called from:
;;     00023DD8 (in gvmsumb_asm)
;;     00023E90 (in fn00023E7C)
fn00023E98 proc
	{ r13 = sub(r6,r3); memw(r29+60) = r5 }
	{ r6 = add(r6,#0xFFFFFFFF); p3 = cmp.gt(r8,#0x60); r16 = memw(r29+104); memw(r29+64) = r28 }
	{ r3 = mpyi(r3,r25); if (p3) r12 = sub(r12,r8); r0 = memuh(r1+2); r0 = memb(r0); memw(r29+84) = r6 }
	{ r11 = memw(r29+48); memw(r29+92) -= #0x1 }
	{ r28 = add(r11,#0x40); r22 = memw(r29+56); memw(r29+48) += r3 }
	{ nop; nop; nop }

;; fn00023EE0: 00023EE0
;;   Called from:
;;     00023ED4 (in fn00023E98)
;;     00023ED4 (in fn00023E98)
;;     00024178 (in fn00024178)
fn00023EE0 proc
	{ r7 = #0x0; r9 = memw(r29+52); r6 = memw(r29+88) }
	{ r15 = r15; jump 000240AC; r9 = memuh(r0); r2 = memb(r0+2); dcfetch(r28,#0x0) }
00023EF8                         10 44 26 60 1C 48 1C F3         .D&`.H..

l00023F00:
	{ r15 = r15; jump fn000240C4; r9 = #0x0; r3:r2 = combine(r7,#0x0) }
00023F08                         00 40 00 7C 04 40 00 7C         .@.|.@.|
00023F10 2E 40 CB 91 14 C0 CB 9D 18 40 00 7C 1A 40 00 7C .@.......@.|.@.|
00023F20 30 40 CB 91 12 C0 CB 9D 00 40 00 7F 00 C0 00 7F 0@.......@......
00023F30 00 40 00 7F 00 40 00 7F 00 40 00 7F 00 C0 00 7F .@...@...@......
00023F40 20 54 4E EA 27 40 07 B0 A6 42 9D 91 00 C0 1C 94  TN.'@...B......
00023F50 00 47 06 60 80 68 14 19 1C 48 1C F3 43 C0 07 75 .G.`.h...H..C..u
00023F60 24 52 50 EA 81 68 12 19 07 40 60 7E 0A C2 09 29 $RP..h...@`~...)
00023F70 80 69 15 19 81 69 13 19 7C 4C 1C FB 0B E7 09 28 .i...i..|L.....(
00023F80 80 6A 0E 19 81 6A 10 19 32 40 CB 91 14 C0 CB 9D .j...j..2@......
00023F90 80 6B 0F 19 81 6B 11 19 30 40 CB 91 0E E0 CB 9D .k...k..0@......

l00023FA0:
	{ r25:r24 += vraddub(r19:r18,r21:r20); p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 000242A4 }

l00023FA8:
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r27:r26 += vraddub(r17:r16,r15:r14); p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t fn000240C4; r9 = memuh(r0); r2 = memb(r0+2); dcfetch(r28,#0x0) }

;; fn00023FCC: 00023FCC
;;   Called from:
;;     00023CC8 (in fn00023CC8)
;;     00023CC8 (in fn00023CC8)
;;     00023CF8 (in gvconv2dbbb_asm)
;;     00023FD0 (in fn00023FD0)
fn00023FCC proc
	{ dcfetch(r28,#0x0) }

;; fn00023FD0: 00023FD0
;;   Called from:
;;     00023FCC (in fn00023FCC)
;;     00023FCC (in fn00023FCC)
fn00023FD0 proc
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 000242D4; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 000240D4; r28 = add(r28,r8); r9 = #0x0; r3:r2 = combine(r7,#0x0) }

l00023FE0:
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 000242E4; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 000242E4; r15:r14 = memd(r11+8); r21:r20 = memd(r11++m0) }
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 000242F4; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000242F4; r17:r16 = memd(r11+8); r19:r18 = memd(r11++m0) }
	{ r1:r0 += vraddub(r15:r14,r21:r20); p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00024300; r28 = add(r28,r8); dcfetch(r28,#0x0) }
	{ r5:r4 += vraddub(r17:r16,r19:r18); p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00024310; r7 = add(r7,#0x1); r9 = #0x10; r2 = and(r2,#0x1) }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00024320; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00024320; p3 = cmp.eq(r7,#0x2); r9 = #0x0; r7:r6 = combine(r7,#0x0) }
	{ p0 = cmp.gtu(r14,#0xA); if (p0.new) jump:t 00024130; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024330; r19:r18 = memd(r11+8); r21:r20 = memd(r11++m0) }

l0002403C:
	{ r21:r20 = memd(r11++m0) }
	{ p0 = cmp.gtu(r15,#0xB); if (p0.new) jump:t 00024140; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024340; r17:r16 = memd(r11+8); r15:r14 = memd(r11++m0) }

l00024050:
	{ r25:r24 += vraddub(r19:r18,r21:r20); p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00024354; if (p3) r7 = #0x0; if (p3) r28 = add(r28,r12) }

l00024060:
	{ r27:r26 += vraddub(r17:r16,r15:r14); p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 00024164; r11 = sub(r11,r13); r9 = #0x10; r2 = and(r2,#0x1) }

l00024070:
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00024374; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 00024174; r9 = memuh(r0); deallocframe); dcfetch(r11,#0x40) }

l00024080:
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 00024384; r28 = add(r11,#0x40); r7 = #0x0 }

l00024088:
	{ r7 = #0x0 }

l0002408C:
	{ p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024390; r28 = add(r28,r8); r15:r14 = memd(r11+8); r21:r20 = memd(r11++m0) }

l00024090:
	{ r28 = add(r28,r8); r15:r14 = memd(r11+8); r21:r20 = memd(r11++m0) }

l0002409C:
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 000243A0; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000243A0; r17:r16 = memd(r11+8); r19:r18 = memd(r11++m0) }

l000240A4:
	{ r17:r16 = memd(r11+8); r19:r18 = memd(r11++m0) }

l000240AC:
	{ r11 = sub(r11,r23); r14 = memw(r29+108); r6 = memw(r29+112) }
	{ r0 = r6; r1 = add(r0,r1); r10 = memw(r29+96) }

l000240BC:
	{ r1 = add(r0,r1); r10 = memw(r29+96) }

;; fn000240C4: 000240C4
;;   Called from:
;;     00023F00 (in gvmsumb_asm)
;;     000240BC (in fn00023EE0)
fn000240C4 proc
	{ r0 += mpyi(r14,r1); r28 = add(r11,#0x40); r15 = memw(r29+60) }
	{ p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 00023D18; memw(r10++#4) = r0 }

l000240D4:
	{ memw(r10++#4) = r0 }

l000240D8:
	{ r2 = add(r2,r15); p0 = cmp.gt(r22,#0x1); p0 = cmp.eq(r4,r0); if (!p0.new) jump:nt 000240D8; r2 = #0x6; r0 = r0 }

l000240E8:
	{ r4 = r6; r5 = add(r4,r5); r12 = r0; jump 00023D00; dcfetch(r11,#0x0) }
000240F8                         04 45 0E EF 04 C0 0B 94         .E......
00024100 25 40 A4 19 08 E4 8A AB 02 4F 02 FB 41 40 56 75 %@.......O..A@Vu
00024110 01 41 45 1C 52 C0 E2 28 18 40 66 70 19 59 18 F3 .AE.R..(.@fp.Y..
00024120 01 CE 20 1A 18 D9 0E EF 26 40 B8 19 0C 4C 21 1F .. .....&@...L!.
00024130 09 F8 8A AB 22 4F 02 FB 60 40 56 75 02 42 46 1C ...."O..`@Vu.BF.

l00024140:
	{ r2 = #0xE; r8 = r8 }
	{ r26 = r6; r27 = add(r26,r27); p1 = cmp.eq(r0,#0xE); if (p1.new) jump:nt 00023D88 }

l00024150:
	{ r26 += mpyi(r14,r27) }
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 00023FA0; r12 = r2; jump 00023D6C; if (p0) memuh(r10++#4) = r26 }

;; fn00024160: 00024160
;;   Called from:
;;     00024154 (in gvconv2dbbb_asm)
;;     00024500 (in gvconvsum2dbbb_asm)
fn00024160 proc
	{ if (p0) r2 = add(r2,r15); p0 = cmp.eq(r7,r3); if (!p0.new) jump:nt 00024164; r2 = #0xE; r0 = r0 }

l00024164:
	{ p0 = cmp.eq(r7,r3); if (!p0.new) jump:nt 00024168; r2 = #0xE; r0 = r0 }

l00024168:
	{ r2 = #0xE; r0 = r0 }

l0002416C:
	{ r22 = add(r22,#0xFFFFFFFC); p1 = cmp.eq(r0,#0xE); if (p1.new) jump:nt 00023D70; memw(r29+96) = r10 }

l00024174:
	{ memw(r29+96) = r10 }

;; fn00024178: 00024178
;;   Called from:
;;     00023FA8 (in fn00023FCC)
;;     0002409C (in gvconv2dbbb_asm)
;;     000240A4 (in fn000241C4)
;;     0002416C (in gvconv2dbbb_asm)
;;     00024178 (in fn000241C4)
;;     00024178 (in fn000241C4)
;;     00024468 (in gvconv2dbbb_asm)
;;     00024478 (in gvconv2dbbb_asm)
;;     000244AC (in fn000241C4)
;;     000244F0 (in gvconvsum2dbbb_asm)
fn00024178 proc
	{ if (p2.new) jump:t fn00023EE0; p2 = cmp.gt(r22,#0x0); r12 = r3; jump fn00023D90 }

l00024180:
	{ r12 = r3; jump 00023D98 }

l00024184:
	{ r9 = memw(r29+92) }
	{ if (!p1.new) jump:t 00023EC0; p1 = cmp.eq(r9,#0x0) }
	{ loop0(00024198,#0x5); r6 = #0x4; r16 = memd(r29+104) }
	{ p1 = cmp.gtu(r6,#0xC); if (!p1.new) jump:t 00023E70 }
	{ r6 = add(r6,r6); r13 = r12; jump 00023DB4 }
	{ r0 = #0x3; r0 = r0 }
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+40) }
	{ r28 = memw(r29+64); dealloc_return }

;; gvconv2dbbw_asm: 000241C0
gvconv2dbbw_asm proc
	{ r6 = memd(r29); r7 = memd(r29+4) }

;; fn000241C4: 000241C4
;;   Called from:
;;     00023BA0 (in fn00023B80)
;;     00024060 (in gvconv2dbbb_asm)
;;     00024088 (in gvconv2dbbb_asm)
;;     00024090 (in gvconv2dbbb_asm)
;;     0002409C (in gvconv2dbbb_asm)
;;     00024144 (in gvconv2dbbb_asm)
;;     00024154 (in gvconv2dbbb_asm)
;;     0002416C (in gvconv2dbbb_asm)
;;     000241C0 (in gvconv2dbbw_asm)
;;     000242D4 (in fn00023FD0)
;;     000242E0 (in fn00023FD0)
;;     00024564 (in gvconv2dbbb_asm)
;;     000245C0 (in gvconv2dbbb_asm)
;;     00024658 (in gvconv2dbbb_asm)
;;     00024824 (in gvconv2dbbb_asm)
;;     000249F0 (in gvconv2dbbb_asm)
;;     00024A00 (in gvconv2dbbb_asm)
fn000241C4 proc
	{ r5 = asl(r5,#0x2); r8 = memw(r29+8); r9 = memw(r29+12) }
	{ r10 = memw(r29+16); r11 = memw(r29+20) }
	{ r12 = memw(r29+24); allocframe(#0x40) }
	{ r25 = lsr(r6,#0x10); memd(r29+32) = r25:r24; memd(r29) = r17:r16 }
	{ r6 = mpy(r6.h,r6.l); memd(r29+16) = r21:r20; memd(r29+24) = r23:r22 }
	{ M0 = r6; memd(r29+8) = r19:r18; memd(r29+40) = r27:r26 }
	{ memw(r29+48) = r0; memw(r29+52) = r1 }
	{ r1 = addasl(r6,r6,#0x1); immext(#0x80000000); r16 = #0x80000001; r11 = #0x0; r0 = r0 }
	{ r23 = mpyi(r8,r3); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 00024070; r1 = sub(#0x10,r1); memw(r29+56) = r4 }

l00024224:
	{ M1 = r1; r13 = asl(r6,#0x1); r1 = add(r1,#0x10); p3 = cmp.gt(r6,#0x60) }
	{ r7 = lsr(r7,#0x4); r23 = sub(r23,r13); r13 = sub(r7,r3); memw(r29+60) = r5 }
	{ r3 = mpyi(r3,r25); r7 = add(r7,#0xFFFFFFFF); if (p3) r1 = sub(r1,r6); r12 = #0x0; r0 = r0 }
	{ nop; nop; nop }
	{ r9 = add(r9,#0xFFFFFFFF); r11 = memw(r29+48) }

;; fn00024264: 00024264
;;   Called from:
;;     00024260 (in fn000241C4)
;;     00024500 (in gvconvsum2dbbb_asm)
fn00024264 proc
	{ r11 = memw(r29+48) }
	{ r26 = add(r11,#0x40); r22 = memw(r29+56); memw(r29+48) += r3 }
	{ nop; nop; nop }

;; fn00024280: 00024280
;;   Called from:
;;     00024264 (in fn00024264)
;;     00024274 (in fn00024264)
;;     0002448C (in fn000241C4)
fn00024280 proc
	{ r27 = #0x0; r24 = memw(r29+52) }
	{ loop1(000242C0,r8); r8 = memuh(r1+2); r2 = memb(r0+2); dcfetch(r26,#0x0) }
	{ loop0(00024320,r7); r26 = add(r26,r6); r8 = #0x1; r3:r2 = combine(r7,#0x0) }
	{ r15 = r15; jump 00024460; r15:r14 = memd(r11+8); r21:r20 = memd(r11++m0) }

l000242A4:
	{ r15:r14 = memd(r11+8); r21:r20 = memd(r11++m0) }
000242AC                                     E2 4F 4F 1F             .OO.
000242B0 30 40 CB 91 12 C0 CB 9D 00 40 00 7F 00 C0 00 7F 0@.......@......
000242C0 80 68 14 19 81 68 12 19 0A 42 18 29 00 C0 1A 94 .h...h...B.)....
000242D0 80 69 15 19                                     .i..            

l000242D4:
	{ p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 000245D4; r26 = add(r26,r6); r8 = #0x1; r7:r6 = combine(r7,#0x0) }

l000242E0:
	{ p0 = cmp.gtu(r14,#0xA); if (p0.new) jump:t 000243E0; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 000245E0; p3 = cmp.eq(r27,#0x1); r27 = add(r27,#0x1) }

l000242F0:
	{ if (p3) r27 = #0x0; if (p3) r26 = add(r26,r1); r19:r18 = memd(r11+8); r21:r20 = memd(r11++m0) }
	{ p0 = cmp.gtu(r15,#0xB); if (p0.new) jump:t 00024400; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024600; r17:r16 = memd(r11+8); r15:r14 = memd(r11++m0) }
	{ nop; nop; nop; nop }
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00024624; p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 00024424; r8 = memuh(r1+2); r2 = memb(r0+2); dcfetch(r26,#0x0) }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00024634; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 00024434; r26 = add(r26,r6); r8 = #0x1; r3:r2 = combine(r7,#0x0) }
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 00024644; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024644; r15:r14 = memd(r11+8); r21:r20 = memd(r11++m0) }
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 00024654; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024654; r17:r16 = memd(r11+8); r19:r18 = memd(r11++m0) }

l00024354:
	{ p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024658; r17:r16 = memd(r11+8); r19:r18 = memd(r11++m0) }

l00024360:
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00024660; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00024660; r8 = memuh(r1+2); r2 = memb(r0+2); dcfetch(r26,#0x0) }

l00024370:
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00024670; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00024670; r26 = add(r26,r6); r8 = #0x1; r7:r6 = combine(r7,#0x0) }

l00024374:
	{ p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00024674; r26 = add(r26,r6); r8 = #0x1; r7:r6 = combine(r7,#0x0) }

l00024380:
	{ p0 = cmp.gtu(r14,#0xA); if (p0.new) jump:t 00024480; p3 = cmp.eq(r27,#0x1); r27 = add(r27,#0x1); r19:r18 = memd(r11+8) }

l00024390:
	{ p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024690; if (p3) r27 = #0x0; if (p3) r26 = add(r26,r1); r21:r20 = memd(r11++m0) }

l000243A0:
	{ p0 = cmp.gtu(r15,#0xB); if (p0.new) jump:t 000244A0; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000246A0; r17:r16 = memd(r11+8); r15:r14 = memd(r11++m0) }

l000243B0:
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 000246B4; p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 000244B4; r11 = sub(r11,r13); r8 = #0x11; r2 = and(r2,#0x1) }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 000246C4; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 000244C4; r27 = #0x0; r26 = add(r11,#0x40) }
	{ loop0(00024320,r7); r26 = add(r26,r6); r8 = memuh(r1); deallocframe); dcfetch(r26,#0x0) }

l000243E0:
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 000246E4; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 000246E4; r15:r14 = memd(r11+8); r21:r20 = memd(r11++m0) }

l000243F0:
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 000246F4; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000246F4; r17:r16 = memd(r11+8); r19:r18 = memd(r11++m0) }

l00024400:
	{ r11 = sub(r11,r23); r0 = memw(r10++#4) }
	{ p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 00024050; r26 = add(r11,#0x40); r5 = memw(r29+60) }

l00024414:
	{ r2 = add(r2,r5); p0 = cmp.gt(r22,#0x1); p0 = cmp.eq(r4,r0); if (!p0.new) jump:nt 00024414; r2 = #0x6; r0 = r0 }

l00024424:
	{ r12 = r0; jump 0002403C; if (p0) r0 = memw(r10++#4) }
0002442C                                     25 40 A0 19             %@..
00024430 00 C0 0B 94 02 45 02 FB 41 40 56 75 01 41 45 1C .....E..A@Vu.AE.
00024440 52 C0 E2 28 01 4E 20 1A 20 E2 8A 9B 26 40 A0 19 R..(.N . ...&@..
00024450 0C 4C 21 1F 04 C0 0B 94 22 45 02 FB 60 40 56 75 .L!....."E..`@Vu

l00024460:
	{ p0 = cmp.eq(r6,r2); if (!p0.new) jump:nt 00024464; r2 = #0xE; r8 = r8 }

l00024464:
	{ r2 = #0xE; r8 = r8 }

l00024468:
	{ p1 = cmp.eq(r0,#0xE); if (p1.new) jump:nt 000240AC; if (p0) r0 = memw(r10++#4) }

l00024470:
	{ p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 000240BC; r22 = add(r22,#0xFFFFFFFC); r12 = r2; jump 00024088 }

l00024478:
	{ r12 = r2; jump 00024090 }

l0002447C:
	{ if (p0) r2 = add(r2,r5); p0 = cmp.eq(r7,r3); if (!p0.new) jump:nt 00024480; r2 = #0xE; r0 = r0 }

l00024480:
	{ p0 = cmp.eq(r7,r3); if (!p0.new) jump:nt 00024484; r2 = #0xE; r0 = r0 }

l00024484:
	{ r2 = #0xE; r0 = r0 }

l00024488:
	{ p1 = cmp.eq(r0,#0xE); if (p1.new) jump:nt 0002408C }

l0002448C:
	{ if (p2.new) jump:t fn00024280; p2 = cmp.gt(r22,#0x0); r12 = r3; jump 000240A4 }

l00024498:
	{ if (!p1.new) jump:t 00024260; p1 = cmp.eq(r9,#0x0) }

l000244A0:
	{ loop0(000244A8,#0x5); r6 = #0x4 }
	{ p1 = cmp.gtu(r6,#0xC); if (!p1.new) jump:t 00024180 }

l000244AC:
	{ r6 = add(r6,r6); r13 = r12; jump fn000240C4 }
000244B4             0C C0 2C 28 0D 1E 04 3E 1F 1E 16 3E     ..,(...>...>
000244C0 98 40 DD 91 BA C0 DD 91 1E C0 1E 96 00 00 00 00 .@..............
000244D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gvconvsum2dbbb_asm: 000244E0
;;   Called from:
;;     0001A65C (in supernode_execute_hvx_slice)
gvconvsum2dbbb_asm proc
	{ immext(#0x1010100); r8 = #0x1010101; r9 = #0x20; dcfetch(r0,#0x0) }
	{ r9 = #0x40; immext(#0x8000); r6 = #0x8000; p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt fn00024178 }

l00024500:
	{ p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt fn00024264; p0 = cmp.eq(r6,#-0x1); if (p0.new) jump:nt fn00024160; r9 = #0x60; p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 0002418C }

l00024510:
	{ r8 = add(r8,r8); p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 0002419C; r2 = #0x2; jump 00024538 }

l00024518:
	{ r2 = #0x2; jump 00024540 }
0002451C                                     73 61 68 19             sah.
00024520 08 48 08 F3 16 43 83 1E 0B C3 03 1E 73 62 68 19 .H...C......sbh.
00024530 08 48 08 F3 04 C0 00 94 73 63 68 19 08 40 9D 91 .H......sch..@..

l00024540:
	{ r6 = memw(r29+4) }
	{ r6 = mpy(r8.l,r6.l); p0 = cmp.eq(r6,#0x1); dcfetch(r0,#0x20) }
	{ memw(r29+4) = r6 }
	{ r6 = memw(r29+8); r14 = memw(r29+36) }
	{ r15 = memw(r29+40); r14 = #0x0; r0 = r0 }
	{ p0 = cmp.eq(r15,#-0x1); if (p0.new) jump:nt fn000241C4; r11 = memw(r29+20); allocframe(#0x48) }

l00024570:
	{ r25 = lsr(r8,#0x10); memd(r29+32) = r25:r24; memd(r29) = r17:r16 }
	{ r8 = mpy(r8.h,r8.l); memd(r29+16) = r21:r20; memd(r29+24) = r23:r22 }
	{ M0 = r8; r21 = #0x60; memd(r29+8) = r19:r18; memd(r29+40) = r27:r26 }
	{ r12 = addasl(r8,r8,#0x1); immext(#0x80000000); r16 = #0x80000001; r11 = #0x0; r0 = r0 }
	{ r23 = mpyi(r6,r3); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 00024400; r12 = sub(#0x10,r12); memw(r29+56) = r4 }

l000245B4:
	{ M1 = r12; if (p0) r21 = add(r21,#0xFFFFFFE0); r6 = memw(r29+84) }

l000245C0:
	{ r21 = asl(r8,#0x2); p3 = cmp.gt(r6,#0xC0); memw(r29+68) = r21 }
	{  }

l000245D0:
	{ r13 = asl(r8,#0x1); r12 = add(r12,#0x10); memw(r29+48) = r0; memw(r29+52) = r1 }

l000245D4:
	{ r12 = add(r12,#0x10); memw(r29+48) = r0; memw(r29+52) = r1 }

l000245DC:
	{ r6 = lsr(r6,#0x4); r23 = sub(r23,r13); r13 = sub(r6,r3); memw(r29+60) = r5 }

l000245E0:
	{ r23 = sub(r23,r13); r13 = sub(r6,r3); memw(r29+60) = r5 }
	{ r6 = add(r6,#0xFFFFFFFF); p3 = cmp.gt(r8,#0x60); r16 = memw(r29+104); memw(r29+64) = r28 }
	{ r3 = mpyi(r3,r25); r12 = sub(r12,r8); r0 = memuh(r1+2); r0 = memb(r0); memw(r29+84) = r6 }
	{ r21 = memw(r29+68); dcfetch(r0,#0x40) }
	{ nop; nop; nop }
	{ r11 = memw(r29+48); memw(r29+92) -= #0x1 }
	{ r28 = add(r11,r21); r22 = memw(r29+56); memw(r29+48) += r3 }
	{ nop; nop; nop }
	{ r7 = #0x0; r9 = memw(r29+52); r6 = memw(r29+88) }
	{ r15 = r15; jump 0002480C; r9 = memuh(r0); r2 = memb(r0+2); dcfetch(r28,#0x0) }

l00024658:
	{ loop1(000246A0,r6); r28 = add(r28,r8); r15 = r15; jump 0002481C; r9 = #0x0; r3:r2 = combine(r7,#0x0) }

l00024660:
	{ r15 = r15; jump 00024824; r9 = #0x0; r3:r2 = combine(r7,#0x0) }
00024668                         00 40 00 7C 04 40 00 7C         .@.|.@.|
00024670 2E 40 CB 91                                     .@..            

l00024674:
	{ r21:r20 = memd(r11++m0) }

l00024678:
	{ r25:r24 = combine(#0x0,#0x0); r27:r26 = combine(#0x0,#0x0); r17:r16 = memd(r11+8); r19:r18 = memd(r11++m0) }
	{ nop; nop }

l0002468C:
	{ nop }

l00024690:
	{ nop; nop; nop; nop }

l000246A0:
	{ r1:r0 += vraddub(r15:r14,r21:r20); r28 = add(r28,r8); r6 = memw(r29+84); dcfetch(r28,#0x0) }
	{ loop0(00024720,r6); p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 000249B0; p3 = cmp.eq(r7,#0x1) }

l000246BC:
	{ r5:r4 += vraddub(r17:r16,r19:r18); p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 000249BC; if (p3) r28 = add(r28,r12); r9 = #0x10; r2 = and(r2,#0x1) }

l000246CC:
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 000249CC; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 000249CC; r7 = sub(#0x1,r7); r9 = #0x0; r7:r6 = combine(r7,#0x0) }

l000246D0:
	{ p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 000249D0; r7 = sub(#0x1,r7); r9 = #0x0; r7:r6 = combine(r7,#0x0) }

l000246DC:
	{ p0 = cmp.gtu(r14,#0xA); if (p0.new) jump:t 000247DC; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 000249DC; r19:r18 = memd(r11+8); r21:r20 = memd(r11++m0) }

l000246E4:
	{ r19:r18 = memd(r11+8); r21:r20 = memd(r11++m0) }
	{ p0 = cmp.gtu(r15,#0xB); if (p0.new) jump:t 000247EC; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000249EC; r17:r16 = memd(r11+8); r15:r14 = memd(r11++m0) }

l000246F0:
	{ p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000249F0; r17:r16 = memd(r11+8); r15:r14 = memd(r11++m0) }

l000246FC:
	{ r25:r24 += vraddub(r19:r18,r21:r20); p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00024A00 }
	{ nop; nop; nop }
	{ nop; nop; nop; nop }
	{ r27:r26 += vraddub(r17:r16,r15:r14); p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 00024824; r9 = memuh(r0); r2 = memb(r0+2); dcfetch(r28,#0x0) }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00024A34; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 00024834; r28 = add(r28,r8); r9 = #0x0; r3:r2 = combine(r7,#0x0) }
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 00024A44; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024A44; r15:r14 = memd(r11+8); r21:r20 = memd(r11++m0) }
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 00024A54; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024A54; r17:r16 = memd(r11+8); r19:r18 = memd(r11++m0) }
	{ r1:r0 += vraddub(r15:r14,r21:r20); p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00024A60; r28 = add(r28,r8); dcfetch(r28,#0x0) }

l00024764:
	{ p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00024A64; r28 = add(r28,r8); dcfetch(r28,#0x0) }

l00024770:
	{ r5:r4 += vraddub(r17:r16,r19:r18); p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00024A70; p3 = cmp.eq(r7,#0x1); r9 = #0x10; r2 = and(r2,#0x1) }

l00024780:
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00024A80; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00024A80; r9 = memuh(r0); deallocframe); dcfetch(r28,#0x0) }

l00024790:
	{ p0 = cmp.gtu(r14,#0xA); if (p0.new) jump:t 00024890; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024A90; r19:r18 = memd(r11+8); r21:r20 = memd(r11++m0) }
	{ p0 = cmp.gtu(r15,#0xB); if (p0.new) jump:t 000248A0; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024AA0; r17:r16 = memd(r11+8); r15:r14 = memd(r11++m0) }
	{ r25:r24 += vraddub(r19:r18,r21:r20); p0 = cmp.gtu(r4,#0x8); if (p0.new) jump:t 00024AB4; r7 = sub(#0x1,r7); if (p3) r28 = add(r28,r12) }
	{ r27:r26 += vraddub(r17:r16,r15:r14); p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 000248C4; r11 = sub(r11,r13); r9 = #0x10; r2 = and(r2,#0x1) }
	{ p0 = cmp.gtu(r5,#0x9); if (p0.new) jump:t 00024AD4; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 000248D4; r21 = memw(r29+68); r9 = #0x0; r3:r2 = combine(r7,#0x0) }
	{ p0 = cmp.gtu(r2,#0xA); if (p0.new) jump:t 00024AE4; r28 = add(r11,r21); r7 = #0x0; dcfetch(r11,#0x40) }

l000247EC:
	{ dcfetch(r11,#0x40) }
	{ p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024AF4; r28 = add(r28,r8); r15:r14 = memd(r11+8); r21:r20 = memd(r11++m0) }

l00024800:
	{ p0 = cmp.gtu(r3,#0xB); if (p0.new) jump:t 00024B04; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024B04; r17:r16 = memd(r11+8); r19:r18 = memd(r11++m0) }

l0002480C:
	{ r19:r18 = memd(r11++m0) }

l00024810:
	{ r11 = sub(r11,r23); p0 = cmp.gt(r22,#0x1); r14 = memw(r29+108); r6 = memw(r29+112) }

l0002481C:
	{ r6 = memw(r29+112) }
	{ r0 = r6; r1 = add(r0,r1); r20 = #0x1; r10 = memw(r29+96) }

l00024824:
	{ r1 = add(r0,r1); r20 = #0x1; r10 = memw(r29+96) }

l00024830:
	{ r0 += mpyi(r14,r1); r16 = extractu(r2,#0x2,#0x2); r15 = memw(r29+60); r21 = memw(r29+68) }
	{ r16 &= asl(r20,r16); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 00024488; r1 = #0x31; jump 00024A10; memw(r10++#4) = r0 }

l00024850:
	{ r16 = vsplatb(r16); r4 = r6; r5 = add(r4,r5); p0 = cmp.eq(r4,r0); if (!p0.new) jump:nt 00024850 }

l00024854:
	{ r4 = r6; r5 = add(r4,r5); p0 = cmp.eq(r4,r0); if (!p0.new) jump:nt 00024854 }

l00024860:
	{ r4 += mpyi(r14,r5); p0 = tstbit(r0,#0x0); if (p0.new) jump:nt 000246F0; r12 = r0; jump 00024478; p0 = cmp.eq(r2,r0); if (!p0.new) jump:nt 00024A60 }

l00024870:
	{ p0 = cmp.eq(r4,#-0x1); if (p0.new) jump:nt 000244B8; r20 = mux(p0,#0x1,#0x10); p1 = cmp.gt(r22,#0x2); r1 = #0x31; jump 00024A40 }
	{ p0 = cmp.eq(r0,r0); if (p0.new) jump:t 000247D0; r28 = add(r11,r21); p0 = cmp.eq(r5,r1); if (!p0.new) jump:nt 00024880; if (p0) memuh(r10++#4) = r4 }
	{ r24 = r6; r25 = add(r24,r25); p0 = cmp.eq(r2,r1); if (!p0.new) jump:nt 00024A98; p1 = cmp.eq(r0,#0xE); if (p1.new) jump:nt 00024490 }
	{ r24 += mpyi(r14,r25); r8 = r8; jump 00024530; r12 = r1; jump 000244B8; r1 = #0x31; jump 00024A74 }
	{ p0 = cmp.eq(r0,r5); if (p0.new) jump:t 00024800; p2 = cmp.gt(r22,#0x3); r7 = add(r2,r15); if (p1) memuh(r10++#4) = r24 }
	{ r17 = extractu(r7,#0x2,#0x2); p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt 0002470C; r8 = r8; jump 00024A10; dcfetch(r11,#0x0) }
	{ r2 = add(r2,r15); r9 = r9; jump 00024560; p0 = cmp.eq(r6,r2); if (!p0.new) jump:nt 000248D4; r2 = #0xC; r0 = r0 }
	{ r17 &= asl(r20,r17); p0 = cmp.eq(r2,r2); if (!p0.new) jump:nt 00024AEC; p1 = cmp.eq(r0,#0xE); if (p1.new) jump:nt 00024524; dcfetch(r11,#0x20) }
	{ r17 = vsplatb(r17); r26 = r6; r27 = add(r26,r27); r9 = r9; jump 00024A40 }

l00024900:
	{ r26 += mpyi(r14,r27); p0 = cmp.eq(r0,r6); if (p0.new) jump:t 00024854; r12 = r2; jump 00024518; r1 = #0x31; jump 00024AD4 }

l00024910:
	{ p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 0002475C; p0 = tstbit(r1,#0x0); if (p0.new) jump:nt 000247A0; dcfetch(r11,#0x40) }

l0002491C:
	{ r20 = mux(p1,#0x1,#0x10); if (p0) r7 = add(r7,r15); p0 = cmp.eq(r7,r3); if (!p0.new) jump:nt 00024920; r2 = #0xC; r8 = r8 }

l00024920:
	{ if (p0) r7 = add(r7,r15); p0 = cmp.eq(r7,r3); if (!p0.new) jump:nt 00024924; r2 = #0xC; r8 = r8 }

l00024924:
	{ p0 = cmp.eq(r7,r3); if (!p0.new) jump:nt 00024928; r2 = #0xC; r8 = r8 }

l00024928:
	{ r2 = #0xC; r8 = r8 }

l0002492C:
	{ if (p0) r2 = add(r2,r15); r10 = r10; jump 000245C0; p0 = cmp.eq(r2,r3); if (!p0.new) jump:nt 00024B38; if (p2) memuh(r10++#4) = r26 }
0002493C                                     B2 42 07 8D             .B..
00024940 AB 67 30 1C 27 4F 07 FB 18 CA 9D A1 B3 42 07 8D .g0.'O.......B..
00024950 92 52 54 C6 34 40 08 7B AA CA CA 1F F2 40 52 8C .RT.4@.{.....@R.
00024960 93 53 54 C6 4B 4B EB 1F 43 CE 20 1A F3 40 53 8C .ST.KK..C. ..@S.
00024970 4A 53 B2 19 96 7F F6 BF 0C CC 23 1F 4B 53 B3 19 JS........#.KS..
00024980 22 4F 02 FB AB 4B CB 1F 0A D0 C2 28 5A 5B DF 5C "O...K.....(Z[.\
00024990 42 4F 02 FB 03 40 56 75 0B D8 C2 28 E9 42 9D 91 BO...@Vu...(.B..
000249A0 35 C2 9D 91 3E 59 FF 5C 01 C0 09 75 31 40 00 69 5...>Y.\...u1@.i

l000249B0:
	{ r6 = #0x4; r16 = memd(r29+104) }
	{ p1 = cmp.gtu(r6,#0xC); if (!p1.new) jump:t 0002468C }

l000249B8:
	{ r6 = add(r6,r6); r13 = r12; jump 000245D0 }

l000249BC:
	{ r13 = r12; jump 000245D4 }
000249C0 0C C0 30 28 0D 1E 04 3E 1F 1E 16 3E             ..0(...>...>    

l000249CC:
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+40) }

l000249D0:
	{ r27:r26 = memd(r29+40) }
	{ r28 = memw(r29+64); dealloc_return }
000249DC                                     00 00 00 00             ....

;; gvconv2dbbb_asm: 000249E0
;;   Called from:
;;     0001A6C4 (in supernode_execute_hvx_slice)
gvconv2dbbb_asm proc
	{ immext(#0x1010100); r8 = #0x1010101; r9 = #0x20; dcfetch(r0,#0x0) }

l000249EC:
	{ dcfetch(r0,#0x0) }

l000249F0:
	{ r9 = #0x40; immext(#0x8000); r6 = #0x8000; p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 00024678 }

l00024A00:
	{ p0 = cmp.eq(r8,#-0x1); if (p0.new) jump:nt 00024764; p0 = cmp.eq(r6,#-0x1); if (p0.new) jump:nt 00024660; r9 = #0x60; p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 0002468C }

l00024A10:
	{ r8 = add(r8,r8); p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 0002469C; r2 = #0x2; jump 00024A38; dcfetch(r0,#0x20) }
	{ p0 = cmp.gtu(r8,#0x1); if (!p0.new) jump:t 00024704; r8 = add(r8,r8); r3 = #0x3; jump 00024A4C; r3 = #0x3; jump 00024A34 }
	{ p0 = cmp.gtu(r8,#0x2); if (!p0.new) jump:t 00024714; r8 = add(r8,r8); dcfetch(r0,#0x40) }
	{ p0 = cmp.gtu(r8,#0x3); if (!p0.new) jump:t 00024720; r6 = memd(r29); r7 = memd(r29+4) }
	{ p0 = cmp.eq(r7,#0x1); r8 = memw(r29+8); r9 = memw(r29+12) }
	{ r7 = mpy(r7.l,r6.l); r10 = memw(r29+16); r11 = memw(r29+20) }
	{ r12 = memw(r29+24); r14 = memw(r29+28) }

l00024A64:
	{ p3 = cmp.gt(r7,#0xC0); r15 = memw(r29+32); r14 = #0x0; r0 = r0 }

l00024A70:
	{ p0 = cmp.eq(r15,#-0x1); if (p0.new) jump:nt 000246D0; r28 = #0x60; allocframe(#0x48) }

l00024A7C:
	{ if (p0) r28 = add(r28,#0xFFFFFFE0); memw(r29+68) = r28 }

l00024A80:
	{ memw(r29+68) = r28 }

l00024A84:
	{ r25 = lsr(r6,#0x10); memd(r29+32) = r25:r24; memd(r29) = r17:r16 }
	{ r6 = mpy(r6.h,r6.l); memd(r29+16) = r21:r20; memd(r29+24) = r23:r22 }
	{ M0 = r6; memd(r29+8) = r19:r18; memd(r29+40) = r27:r26 }
	{ r17 = asl(r6,#0x2); memw(r29+48) = r0; memw(r29+52) = r1 }
	{ if (!p3) r28 = add(r17,#0x0) }
	{ r1 = addasl(r6,r6,#0x1); immext(#0x80000000); r16 = #0x80000001; r11 = #0x0; r0 = r0 }
	{ r23 = mpyi(r8,r3); p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 0002491C; r1 = sub(#0x10,r1); memw(r29+56) = r4 }

l00024AD0:
	{ M1 = r1; r13 = asl(r6,#0x1); r1 = add(r1,#0x10) }
	{ r7 = lsr(r7,#0x4); r23 = sub(r23,r13); r13 = sub(r7,r3); memw(r29+60) = r5 }
	{ r3 = mpyi(r3,r25); r7 = add(r7,#0xFFFFFFFF); r1 = sub(r1,r6); r12 = #0x0; r0 = r0 }

l00024AF4:
	{ r1 = sub(r1,r6); r12 = #0x0; r0 = r0 }
	{ nop }
	{ r9 = add(r9,#0xFFFFFFFF); r11 = memw(r29+48) }

l00024B04:
	{ r11 = memw(r29+48) }
	{ r26 = add(r11,r28); r22 = memw(r29+56); memw(r29+48) += r3 }
	{ nop; nop; nop }
	{ p2 = !cmp.eq(r2,r2); r24 = memw(r29+52) }
	{ loop1(00024B60,r8); r8 = memuh(r1+2); r2 = memb(r0+2); dcfetch(r26,#0x0) }
	{ loop0(00024BC0,r7); r26 = add(r26,r6); r8 = #0x1; r3:r2 = combine(r7,#0x0) }
	{ r15 = r15; jump 00024D00; r15:r14 = memd(r11+8); r21:r20 = memd(r11++m0) }
00024B4C                                     E2 4F 4F 1F             .OO.
00024B50 30 40 CB 91 12 C0 CB 9D 00 40 00 7F 00 C0 00 7F 0@.......@......
00024B60 80 68 14 19 81 68 12 19 0A 42 18 29 00 C0 1A 94 .h...h...B.)....
00024B70 80 69 15 19 81 69 13 19 1A 46 1A F3 0B E7 18 28 .i...i...F.....(
00024B80 80 6A 0E 19 81 EA 10 19 02 40 C2 6B 5A 41 1A FB .j.......@.kZA..
00024B90 32 40 CB 91 14 C0 CB 9D 80 6B 0F 19 81 6B 11 19 2@.......k...k..
00024BA0 30 40 CB 91 0E E0 CB 9D 00 40 00 7F 00 C0 00 7F 0@.......@......
00024BB0 00 40 00 7F 00 40 00 7F 00 40 00 7F 00 C0 00 7F .@...@...@......
00024BC0 82 68 14 19 83 68 0E 19 08 42 18 29 00 C0 1A 94 .h...h...B.)....
00024BD0 82 69 15 19 83 69 0F 19 1A 46 1A F3 09 E7 18 28 .i...i...F.....(
00024BE0 82 6A 12 19 83 6A 10 19 2E 40 CB 91 14 C0 CB 9D .j...j...@......
00024BF0 82 6B 13 19 83 6B 11 19 30 40 CB 91 12 C0 CB 9D .k...k..0@......
00024C00 80 68 14 19 81 68 12 19 0A 42 18 29 00 C0 1A 94 .h...h...B.)....
00024C10 80 69 15 19 81 69 13 19 1A 46 1A F3 0B E7 18 28 .i...i...F.....(
00024C20 80 6A 0E 19 32 40 CB 91 00 C0 1A 94 02 40 C2 6B .j..2@.......@.k
00024C30 81 6A 10 19 5A 41 1A FB 14 C0 CB 9D 80 AB 0F 19 .j..ZA..........
00024C40 81 6B 11 19 30 40 CB 91 0E E0 CB 9D 82 68 14 19 .k..0@.......h..
00024C50 83 68 0E 19 0B 4B 2D F3 08 C2 18 29 82 69 15 19 .h...K-....).i..
00024C60 83 69 0F 19 12 42 02 F2 1A DC 0B F3 08 55 07 60 .i...B.......U.`
00024C70 1A 46 1A F3 09 67 18 28 00 C0 1A 94 82 6A 12 19 .F...g.(.....j..
00024C80 83 6A 10 19 2E 40 CB 91 14 C0 CB 9D 82 6B 13 19 .j...@.......k..
00024C90 83 AB 11 19 30 40 CB 91 12 C0 CB 9D B0 42 02 8D ....0@.......B..
00024CA0 0B 4B 37 F3 E8 71 03 1E 20 C0 8A 9B 24 40 A0 19 .K7..q.. ...$@..
00024CB0 1A 5C 0B F3 F5 3C 19 48 90 50 51 C6 20 40 56 75 .\...<.H.PQ. @Vu
00024CC0 00 40 44 1C E9 F1 03 1E F0 40 50 8C 0C 4C 20 1F .@D......@P..L .
00024CD0 00 40 52 1C 20 E0 8A 9B 48 53 B0 19 25 40 A0 19 .@R. ...HS..%@..
00024CE0 41 40 56 75 00 C0 0B 94 A8 60 30 1C 01 41 45 1C A@Vu.....`0..AE.
00024CF0 EA 71 03 1E 20 E2 8A 9B 26 40 A0 19 62 40 56 75 .q.. ...&@..b@Vu

l00024D00:
	{ p0 = cmp.eq(r2,r1); if (!p0.new) jump:nt 00024F08; p1 = cmp.eq(r0,#0xE); if (p1.new) jump:nt 00024900 }

l00024D08:
	{ r8 = r8; jump 00024998; r12 = r1; jump 00024920; p0 = cmp.eq(r6,r2); if (!p0.new) jump:nt 00024D0C; if (p2) r0 = memw(r10++#4) }
	{ p0 = cmp.eq(r0,r5); if (p0.new) jump:t 00024C68; p0 = cmp.eq(r2,r2); if (!p0.new) jump:nt 00024F24; p1 = cmp.eq(r0,#0xE); if (p1.new) jump:nt 0002495C; dcfetch(r11,#0x20) }
	{ p0 = cmp.eq(r0,#-0x1); if (p0.new) jump:nt 00024974; r21 = add(r2,r5); r8 = r8; jump 00024E78; r1 = #0x31; jump 00024EFC }
	{ r18 = extractu(r21,#0x2,#0x2); p0 = cmp.eq(r0,r6); if (p0.new) jump:t 00024C8C; r17 = mux(p0,#0x1,#0x10) }
	{ r18 &= asl(r17,r18); r9 = r9; jump 000249D4; r12 = r2; jump 0002495C; p0 = cmp.eq(r7,r3); if (!p0.new) jump:nt 00024D48 }
	{ r18 = vsplatb(r18); r10 = r10; jump 000249E8; p0 = cmp.eq(r2,r3); if (!p0.new) jump:nt 00024F60; r2 = #0xC; r0 = r0 }
	{ p0 = tstbit(r2,#0x0); if (p0.new) jump:nt 00024BF4; r2 = add(r2,r5); r9 = r9; jump 00024EB4; dcfetch(r11,#0x40) }
	{ p0 = cmp.eq(r0,r7); if (p0.new) jump:t 00024CC8; r22 = add(r22,#0xFFFFFFFC); if (p0) r21 = add(r21,r5); p1 = cmp.eq(r0,#0xE); if (p1.new) jump:nt 000249F8 }
	{ r19 = extractu(r21,#0x2,#0x2); r17 = mux(p1,#0x1,#0x10); if (p1) r21 = add(r21,r5); r12 = r3; jump 0002499C }
	{ r20 = extractu(r21,#0x2,#0x2); r19 &= asl(r17,r19); r17 = mux(p2,#0x1,#0x10); r11 = r11; jump 00024A28 }
	{ r19 = vsplatb(r19); r20 &= asl(r17,r20); r10 = r10; jump 00024EF8; r2 = #0xC; r8 = r8 }
	{ r20 = vsplatb(r20); p0 = tstbit(r3,#0x0); if (p0.new) jump:nt 00024C48; if (p0) r2 = add(r2,r5); r11 = r11; jump 00024F08 }
	{ p0 = tstbit(r4,#0x0); if (p0.new) jump:nt 00024C58; if (p1) r2 = add(r2,r5); r2 = #0xC; r0 = r0 }
	{ if (p3.new) jump:t 00024B20; if (p2) r2 = add(r2,r5); p3 = cmp.gt(r22,#0x0); r2 = #0xC; r8 = r8 }
	{ if (!p1.new) jump:t 00024B00; p1 = cmp.eq(r9,#0x0) }
	{ loop0(00024DF0,#0x5); r6 = #0x4 }
	{ p1 = cmp.gtu(r6,#0xC); if (!p1.new) jump:t 00024AC8 }
	{ r6 = add(r6,r6); r13 = r12; jump 00024A0C }
	{ r12 = #0x2; r0 = r0 }
	{ r17:r16 = memd(r29); r19:r18 = memd(r29+8) }
	{ r21:r20 = memd(r29+16); r23:r22 = memd(r29+24) }
	{ r25:r24 = memd(r29+32); r27:r26 = memd(r29+40) }
	{ r28 = memw(r29+68) }
	{ dealloc_return }
