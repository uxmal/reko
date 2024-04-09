;;; Segment .text (00009840)

l00019850:
	{ r7:r6 = vaslw(r25:r24,#0x2) }
	{ if (!cmp.gtu(r3.new,r2)) jump:t 00019874; r3 = and(#0x81,asl(r3,#0x4)) }

l00019860:
	{ memw(r29+4) = r2; r1 = #0xF7; r3 = add(PC,#0x3B) }
	{ memw(r29) = r7; r2 = r23 }
	{ jump 00019904 }

l00019874:
	{ r2 = memw(r29+124) }
	{ if (!cmp.gtu(r25,r4.new)) jump:t 0001989C; r4 = memw(r2+20); r5 = r2 }

l00019888:
	{ memw(r29+4) = r25; r1 = #0xFA; r3 = add(PC,#0x2D) }
	{ memw(r29) = r4; r2 = r23 }
	{ jump 00019904 }

l0001989C:
	{ r2 = memw(r29+52); r1 = #0xFC; r3 = add(PC,#0xED6F); immext(#0xED40) }
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 00019900; r2 = memw(r2) }

l000198B8:
	{ r2 = memw(r29+52); r1 = #0xFD; r3 = add(PC,#0x28) }
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 00019900; r2 = memw(r2+12) }

l000198D0:
	{ r2 = memd(r29+48); r4 = #0x4; r3 = add(PC,#0x21) }
	{ r1 = #0xFE }
	{ if (cmp.gtu(r4,r2.new)) jump:t 00019900; r2 = memw(r2+20) }

l000198E8:
	{ r2 = memw(r29+44); r1 = #0xFF; r3 = add(PC,#0x17) }
	{ if (cmp.gtu(r2.new,#0x3)) jump:t 00019910; r2 = memw(r2+20); p0 = cmp.gt(r24,#0x0) }

l00019900:
	{ r2 = r23 }

l00019904:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 00019CD4 }

l00019910:
	{ memw(r5+24) = r25; r2 = memw(r29+68); r0 = p0 }
	{ memw(r5) = r2; r23 = memd(r29+56); r2 = #0x0 }
	{ memd(r29+112) = r7:r6; r3 = memd(r29+84) }
	{ memw(r5+8) = r20; memw(r5+4) = r17; if (p0) r20 = add(r24,#0x0) }
	{ memw(r29+20) = r0; memw(r5+12) = r24; if (!p0) jump:nt 00019974 }

l0001993C:
	{ r26 = memw(r29+40); r18 = max(r18,r2) }
	{ r2 = memb(r23++#1); r19 = r3 }
	{ r2 = sub(r2,r18) }
	{ r2 = convert_w2sf(r2) }
	{ r20 = add(r20,#0xFFFFFFFF); call fn00009620; r0 = sfmpy(r3,r2) }
	{ memw(r26++#4) = r2.new; p0 = cmp.eq(r20,#0x0); r3 = r19; r2 = convert_uw2sf(r0):chop }

l00019974:
	{ r1:r0 = memd(r29+112); r2 = memd(r29+40); r9 = r17; r14 = #0x0 }
	{ r18 = memd(r29+108); r3 = memd(r29+68) }
	{ r13 = memw(r29+60); r0 = memw(r29+20); p0 = cmp.gt(r3,#0x0); r2 += add(r0,#0x7F) }
	{ if (p0) r3 = memw(r29-128); if (p0) r12 = memw(r29+64); r2 = and(r2,#0xFFFFFF80); p1 = r0 }
	{ memw(r29+92) = r2; if (p0) r8 = add(r9,#0xFFFFFFFF); if (p0) r2 = sub(r22,r16); if (!p0) jump:nt 00019BD0 }

l000199B4:
	{ r5 = memd(r29+104); r4 = r18; p2 = cmp.gt(r12,#0xFF); p3 = cmp.gt(r12,#0xFFFFFFFF) }
	{ r1 = memw(r29+88); r14 = #0x0; r6 = #0xFF; r7 = mpyi(r22,r21) }
	{ r2 = memd(r29+76); r5 = #0x0; r3 = sub(r3,r13); r4 = add(r2,mpyi(r4,r5)) }
	{ p0 = cmp.gt(r1,#0xFF); r4 += lsr(r4,#0x1F); r7 = mpyi(r7,r24) }
	{ memb(r29+31) = r0.new; if (p0) r2 = add(r6,#0x0); r0 = p3; r8 = add(r3,mpyi(r8,r2)) }
	{ r0 = memw(r29+124); if (!p2) r6 = zxtb(r12); p3 = cmp.gt(r1,#0xFFFFFFFF) }
	{ memw(r29+52) = r5; r3 = #0x0; p0 = r0; r8 += lsr(r8,#0x1F) }
	{ if (!p0) r6 = add(r3,#0x0); if (!p3) r2 = add(r3,#0x0); r5 = mpyi(r21,r24) }
	{ memb(r29+22) = r1.new; r1 = asr(r4,#0x1) }
	{ memw(r29+56) = r3 }

l00019A30:
	{ if (p0.new) r3 = memw(r29+52); p0 = cmp.gt(r9,#0x0); r15 = #0x0; if (!p0.new) jump:nt 00019BC0 }

l00019A40:
	{ r12 = mpyi(r3,r13) }
	{ memb(r29+16) = r3.new; r3 = mpyi(r3,r9) }

l00019A4C:
	{ if (p0.new) memw(r29+84) = r15; r9 = #0x0; if (p0.new) jump:nt 00019A60; p0 = cmp.gt(r10,#0x0) }
	{ memw(r29+84) = r15; jump 00019BB0 }
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
	{ r9 = memw(r29+80); r15 = memw(r29+84) }
	{ if (!cmp.eq(r15.new,r9)) jump:t 00019A4C; r15 = add(r15,#0x1) }

l00019BC0:
	{ r3 = memd(r29+68); r4 = memd(r29+52) }

l00019BC4:
	{ r4 = add(r4,#0x1) }
	{ memw(r29+52) = r4; if (!p0.new) jump:nt 00019A30; p0 = cmp.eq(r4,r3) }

l00019BD0:
	{ r2 = memw(r29+24); r16 = r9 }
	{ r19 = add(r14,r2) }
	{ r1:r0 = combine(r19,#0xFF0000); immext(#0xFF0000); call fn00009760 }
	{ r2 = memd(r29+28); r3 = memd(r29+20); r12 = r16 }
	{ r14 = memw(r29+40); r13 = memw(r29+36); p1 = r3; r4 = mpyi(r12,r18) }
	{ if (p0.new) r5 = memw(r29+68); if (!p0.new) jump:nt 00019C74; p0 = cmp.gt(r2,#0x0) }

l00019C08:
	{ r3:r2 = combine(#0x100,#0x0); immext(#0x100); r4 = mpyi(r4,r5) }
	{ loop1(00019C18,r4) }
	{ if (p1) r5:r4 = combine(r13,r14) }
	{ if (p1) r7 = memw(r29+92); if (!p1) jump:nt 00019C68 }

l00019C24:
	{ r6 = mpyi(r3,r24); loop0(00019C30,r24) }
	{ r6 = addasl(r7,r6,#0x2) }
	{ r8 = memw(r4++#4); r7 = #0x0 }
	{ r9 = memw(r6++#4) }
	{ r8 = add(r9,r8) }
	{ r8 = add(#0x8000,mpyi(r8,r0)); immext(#0x8000) }
	{ if (!cmp.gt(r8.new,#-0x1)) jump:nt 00019C5C; r8 = asrh(r8) }

l00019C54:
	{ if (!p0.new) r7 = #0xFF; p0 = cmp.gt(r2,r8) }

l00019C5C:
	{ memb(r5++#1) = r7; nop }
	{ r13 = add(r13,r24) }

l00019C68:
	{ nop; nop; r3 = add(r3,#0x1) }

l00019C74:
	{ r6 = memd(r29+44); r4 = memd(r29+48); r1 = #0x152; r3 = convert_w2sf(r19) }
	{ r5 = memw(r29+32) }
	{ memw(r4+12) = #0x1; r2 = memw(r4+16) }
	{ memw(r4+4) = #0x1; memw(r4+8) = #0x1; r3 = #0x2; r5 = sfmpy(r5,r3) }
	{ memw(r2) = #0x0; memw(r4) = #0x1 }
	{ memw(r6) = #0x1; memw(r4+24) = #0x4; r4 = add(PC,#0xE9B1); immext(#0xE980) }
	{ memw(r6+8) = #0x1; r2 = memw(r6+16) }
	{ memw(r6+4) = #0x1; memw(r6+12) = #0x1 }
	{ memw(r6+24) = #0x4; memw(r2) = r5 }
	{ memw(r29+12) = r24; r2 = memw(r29+72) }
	{ memw(r29+8) = r18; r5 = memd(r29+68) }
	{ memw(r29) = r5; memw(r29+4) = r12 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00019CD4:
	{ r19:r18 = memd(r29+168); r17:r16 = memd(r29+176) }
	{ r23:r22 = memd(r29+152); r21:r20 = memd(r29+160) }
	{ r27:r26 = memd(r29+136); r25:r24 = memd(r29+144) }
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
	{ if (cmp.gtu(r3,r5.new)) jump:t 00019D08; r5 = memw(r2+16) }

l00019CF8:
	{ r6 = add(r29,#0x10); r5 = add(r29,#0x10); r0 = add(PC,#0xC) }
	{ memw(r29+4) = r6; call logv }

l00019D08:
	{ dealloc_return }

;; errlog_function: 00019D0C
;;   Called from:
;;     00019040 (in supernode_execute_hvx)
;;     00019418 (in supernode_check_ref)
;;     00019904 (in supernode_execute_ref)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xE7F4); immext(#0xE7C0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; supernode_execute_hvx_slice: 00019D30
supernode_execute_hvx_slice proc
	{ allocframe(#0x138); memd(r29-16) = r17:r16; r16 = r1 }
	{ memd(r29+272) = r25:r24; r3 = memw(r16) }
	{ r5 = memw(r16+4) }
	{ memd(r29+280) = r23:r22 }
	{ memw(r29+248) = r5; r2 = memw(r3+4) }
	{ r17 = memb(r3+32) }
	{ memd(r29+288) = r21:r20; memd(r29+296) = r19:r18 }
	{ r1 = memw(r2+4); r24 = memw(r2); p0 = cmp.eq(r17,#0x0) }
	{ r7 = memw(r2+8); r4 = memw(r2+24) }
	{ memw(r29+240) = r7; r5 = memw(r2+16) }
	{ r22 = memw(r2+12); r7 = memw(r2+20) }
	{ r23 = memw(r24+8); r2 = memw(r1+12) }
	{ memw(r29+252) = r0; memd(r29+264) = r27:r26; r0 = r23 }
	{ memw(r29+64) = r2; r2 = memw(r1+8) }
	{ r19 = memw(r1); r21 = memw(r24+4) }
	{ r27 = memw(r4+8); r20 = memw(r1+4); r1 = p0 }
	{ memw(r29+228) = r7; r18 = memw(r4+4) }
	{ r7 = memw(r24); r6 = memw(r24+12) }
	{ memw(r29+224) = r5; memw(r29+256) = r3 }
	{ memw(r29+72) = r7; memw(r29+112) = r6 }
	{ memw(r29+220) = r1; memw(r29+244) = r2; if (!p0) r2 = sub(r23,r20) }
	{ if (p0) jump:nt 00019DEC }

l00019DD4:
	{ if (p0.new) r0 = add(r2,r27); if (p0.new) jump:nt 00019DEC; p0 = cmp.eq(r9,#0x2) }

l00019DDC:
	{ r0 = #0x0; if (!p0.new) jump:nt 00019DF4; p0 = cmp.eq(r9,#0x1) }

l00019DE4:
	{ r0 = r27 }
	{ r0 += add(r23,#0xFFFFFFFF) }

l00019DEC:
	{ r1 = r27; call fn00009760 }

l00019DF4:
	{ if (p0.new) r1 = add(r18,#0x0); r26 = r27; r25 = r0; p0 = cmp.eq(r17,#0x2) }
	{ memb(r29+59) = r0.new; if (p0) r2 = sub(r21,r19); if (p0) jump:nt 00019E3C; r0 = p0 }

l00019E18:
	{ r1:r0 = combine(r18,r18) }
	{ r1 = memw(r29+220); r0 = #0x0 }
	{ if (!p0) r1:r0 = combine(r18,r21); if (!p0.new) jump:nt 00019E44; p0 = r1 }

l00019E30:
	{ jump 00019E40 }
00019E34             E0 5F 15 E2 06 C0 00 58                 ._.....X    

l00019E3C:
	{ r0 = add(r2,r18) }

l00019E40:
	{ call fn00009760 }

l00019E44:
	{ r4 = memw(r29+256); r3 = memw(r29+240); r7 = sub(r20,r23); r6 = add(r0,#0xFFFFFFFF) }
	{ memw(r29+116) = r23; r2 = memw(r22+16); r17 = r19; r22 = add(r25,#0xFFFFFFFF) }
	{ r4 = memw(r4+8); r3 = memw(r3+16); r23 = r6; r22 = add(r7,mpyi(r22,r26)) }
	{ memw(r29+240) = r6; r6 = memw(r29+224) }
	{ r2 = memw(r2); r1 = memw(r29+228) }
	{ r7 = memd(r29+72); r19 = memd(r29+64); r22 += lsr(r22,#0x1F) }
	{ r27 = memw(r3); r9 = add(r19,#0x1F); r3 = sub(r17,r21); r7 = mpyi(r19,r7) }
	{ r5 = memw(r1+16); r4 = memw(r4); r7 = mpyi(r7,r25); r23 = add(r3,mpyi(r23,r18)) }
	{ r8 = memw(r24+16); r6 = memw(r6+16); r24 = r0; r0 = sfsub(r2,r27) }
	{ memw(r29+128) = r26 }
	{ memw(r29+160) = r8; r8 = memw(r16+20); r23 += lsr(r23,#0x1F) }
	{ memw(r29+56) = r16; r3 = memw(r4+16) }
	{ memw(r29+228) = r9; memw(r29+108) = r21 }
	{ memw(r29+196) = r8 }
	{ memw(r29+224) = r7; r26 = memw(r5) }
	{ r16 = memw(r6) }
	{ memw(r29+60) = r3; call fmaxf.1.0 }
	{ r2 = #0x437F0000; immext(#0x437F0000) }
	{ r21 = #0x0; immext(#0x0); r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = sfsub(r21,r27) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r2 = #0x0; r0 = sfsub(r26,r16); r18 = convert_uw2sf(r0):chop }
	{ p1 = cmp.gt(r18,#0xFF) }
	{ if (p1) r18 = #0xFFFFFFFF; p0 = cmp.gt(r18,#0xFFFFFFFF) }
	{ memb(r29+33) = r2.new; r2 = #0x0; if (!p0) r18 = add(r2,#0x0); call fmaxf.1.0 }
	{ r2 = #0x0 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = sfsub(r21,r16) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r2 = convert_uw2sf(r0):chop }
	{ nop; if (p0.new) jump:nt 00019F68; p0 = cmp.gt(r2,#-0x1) }

l00019F58:
	{ if (!p0.new) r3 = zxtb(r2); if (p0.new) r3 = #0xFF; p0 = cmp.gt(r2,#0xFF) }
	{ memw(r29+132) = r3 }

l00019F68:
	{ r9 = memw(r29+128); r6 = memw(r29+244); p0 = cmp.eq(r17,#0x7); r2 = mpyi(r20,r17) }
	{ memb(r29+41) = r8.new; r5 = memw(r29+112); r8 = asr(r23,#0x1); r4 = add(#0x3,mpyi(r24,r25)) }
	{ p1 = cmp.eq(r5,#0x3); p2 = cmp.eq(r9,#0x2); r0 = p0 }
	{ memw(r29+220) = r0; r2 = memw(r29+224); r7 = and(r3,#0xFFFFFFF0); r21 = mpyi(r2,r6) }
	{ r0 = memw(r29+220); r5 = add(r5,#0xF); p0 = cmp.eq(r7,#0x20); p3 = cmp.eq(r7,#0xA0) }
	{ r4 = and(r4,#0x3FFFFFFC); immext(#0x3FFFFFC0); p0 = fastcorner9(p2,p0); r1 = mpyi(r24,r25) }
	{ memb(r29+26) = r6.new; r12 = memw(r29+228); p3 = fastcorner9(p2,p3); r6 = asr(r22,#0x1) }
	{ r8 = r25; r5 = cmp.eq(r21,r7); r26 = and(r5,#0xFFFFFFF0) }
	{ memw(r29+76) = r1; r6 = and(r18,#0xFF); p0 = fastcorner9(p1,p0); r2 = mpyi(r2,r24) }
	{ r13 = and(r12,#0xFFFFFFE0); r23 = r9; r3 = asl(r19,#0x2); p1 = r0 }
	{ r25 = r24; p2 = cmp.eq(r17,#0x3); r4 = asl(r4,#0x2); p1 = fastcorner9(p1,p2) }
	{ memw(r29+96) = r5; memw(r29+244) = r13; p2 = cmp.eq(r20,#0x7); p0 = fastcorner9(p2,p0) }
	{ p1 = cmp.eq(r20,#0x3); p2 = fastcorner9(p2,p1) }
	{ p0 = cmp.eq(r21,r7); r0 = p2; p3 = or(p2,and(p0,p1)) }
	{ memw(r29+100) = r0; r0 = memw(r29+236) }
	{ if (p1.new) r24 = memw(r29+108); if (!p1.new) r22 = memw(r29+116); if (p1.new) jump:nt 0001A078; p1 = r0 }

l0001A050:
	{ r24 = memw(r29+108); p2 = !cmp.eq(r17,00000001); p1 = !cmp.eq(r20,00000001) }
	{ memb(r29+30) = r0.new; r0 = p3; p1 = or(p2,p1) }
	{ if (!p1.new) jump:nt 0001A090 }

l0001A070:
	{ jump 0001A08C; p0 = and(p0,p0) }

l0001A078:
	{ r22 = memd(r29+116); r5 = #0x0; r0 = p3 }
	{ memw(r29+120) = r0; memw(r29+200) = r5; if (p3) jump:nt 0001A0A8 }

l0001A08C:
	{ memb(r29+50) = r5.new; r0 = memw(r29+120); r5 = mux(p0,#0x1,#0x0) }

l0001A090:
	{ memb(r29+50) = r5.new; r0 = memw(r29+120) }

l0001A09C:
	{ if (p0.new) memw(r29-20) = r6; if (!p0.new) r6 = memw(r29-92); if (!p0.new) jump:nt 0001A0D0 }

l0001A0A8:
	{ memw(r29+236) = r6; r5 = r25; r16 = r7; r6 = asl(r17,#0x1) }
	{ memw(r29+232) = r8; r5 += add(r6,#0x2); r7 = mpyi(r8,r16) }
	{ jump 0001A0E8; r5 = mpyi(r7,r5) }
0001A0CC                                     A5 3D 78 50             .=xP

l0001A0D0:
	{ memw(r29+232) = r8; r5 = add(r5,r22) }
	{ r6 = add(r6,r17); r5 = mpyi(r5,r26) }
	{ r6 = addasl(r24,r6,#0x1) }
	{ r5 = mpyi(r5,r6) }

l0001A0E8:
	{ r6 = memw(r29+252) }
	{ r6 = memw(r6+4) }
	{ memw(r29+188) = r6; r6 += add(r3,#0x7F) }
	{ memb(r29+42) = r3.new; r3 = and(r6,#0xFFFFFF80) }
	{ r5 = memw(r29+256); r18 = and(r3,#0xFFFFFF80) }
	{ r7 = add(r18,#0x17F) }
	{ memb(r29+46) = r6.new; r5 = memw(r5+40); r6 = and(r7,#0xFFFFFF80) }
	{ r27 = memw(r5) }
	{ r4 = memw(r29+244) }
	{ r19 = and(r6,#0xFFFFFF80); p0 = cmp.eq(r19,r4) }
	{ r7 = add(r19,#0x17F) }
	{ memb(r29+17) = r0.new; r3 = and(r7,#0xFFFFFF80); r0 = p0 }
	{ r2 = memw(r29+60); r3 = and(r3,#0xFFFFFF80) }
	{ if (p0) r3 = add(r2,#0x0) }
	{ memw(r29+204) = r3; call fn00009530 }
	{ p0 = cmp.eq(r21,r16); r2 = mpyi(r24,r22) }
	{ memd(r29+48) = r1:r0; r3 = memd(r29+112) }
	{ r2 = mpyi(r2,r3) }
	{ memw(r29+80) = r2; if (!p0) jump:nt 0001A17C }

l0001A170:
	{ r0 = memd(r29+120); r4 = r23 }
	{ if (!p0.new) jump:nt 0001A1A0; p0 = r0 }

l0001A17C:
	{ r22 = memw(r29+232); r20 = #0x1; r26 = r16; r24 = r25 }
	{ r7 = #0x0; r4 = #0x1; r2 = #0x0; r17 = #0x1 }
	{ memw(r29+164) = r7; memw(r29+104) = r2 }

l0001A1A0:
	{ memw(r29+224) = r20; r2 = memw(r29+104); r3 = mpyi(r26,r20) }
	{ memw(r29+116) = r22; r0 = #0x60000; immext(#0x60000); r2 = add(r22,r2) }
	{ memw(r29+192) = r26; memw(r29+228) = r4; r2 = mpyi(r2,r26) }
	{ memw(r29+108) = r24; memw(r29+252) = r2; r2 = mpyi(r4,r2); r20 = mpyi(r3,r17) }
	{ r1 = asl(r2,#0x2); r0 -= asl(r20,#0x6) }
	{ call fn00009750 }
	{ r2 = memw(r29+240) }
	{ r2 = add(r2,r0) }
	{ r1:r0 = combine(r0,r2); call fn00009750 }
	{ r21 = memw(r29+248); r3 = add(r0,#0x1); r2 = r25 }
	{ p1 = cmp.eq(r21,#0x1); r4 = clrbit(r3,#0x0); r2 += lsr(r2,#0x1F) }
	{ p0 = cmp.eq(r4,#0x2); r3 += lsr(r3,#0x1F) }
	{ memb(r29+55) = r2.new; r2 = asr(r2,#0x1); r25 -= asr(r2,#0x1) }
	{ if (!p1) r25 = add(r2,#0x0); r26 = p1 }
	{ memw(r29+176) = r25; memw(r29+256) = r26; if (p0) r24 = #0x2 }
	{ memw(r29+180) = r24; r1:r0 = combine(r24,r25) }
	{ call fn00009750; r20 = and(#0x3D,lsr(r20,#0xB)) }
	{ memw(r29+212) = r0; r3 = #0x40; r2 = asr(r20,#0x1F) }
	{ r20 += lsr(r2,#0x1A) }
	{ r20 = r17; r2 = asr(r20,#0x6) }
	{ r2 = combine(r3.l,r2.l) }
	{ r1:r0 = combine(r3,r2) }
	{ memd(r29+136) = r1:r0 }
	{ l2fetch(r27,r1:r0) }
	{ r1 = #0x0; r2 = #0x80; r0 = addasl(r18,r21,#0x7) }
	{ memw(r29+208) = r0; r17 = #0x0; call fn000095F0 }
	{ if (!cmp.gt(r17.new,#0x0)) jump:nt 0001A314; immext(#0x4C0); immext(#0x4C0); r2 = memw(r29+72); r7 = mpyi(r24,r25) }

l0001A29C:
	{ r0 = memw(r29+256); r2 = memw(r29+248) }
	{ memb(r29+43) = r6.new; r3 = memw(r29+220); r6 = add(r24,#0xFFFFFFFF); r2 = asl(r2,#0x5) }
	{ memw(r29+92) = r7; r3 = #-0x1; r19 = addasl(r19,r2,#0x2) }
	{ memw(r29+220) = r3 }
	{ r3 = memw(r29+128); r2 = memw(r29+112) }
	{ r0 = memw(r29+76); r1 = memw(r29+212); r2 = mpyi(r3,r2) }
	{ r4 = memw(r29+220); r6 = memw(r29+244); r1 += add(r20,#0x1); r5 = mpyi(r2,r22) }
	{ memw(r29+84) = r17; memw(r29+88) = r18; r3 = add(#0x3F,mpyi(r1,r5)) }
	{ memw(r29+156) = r5; r2 = memw(r29+80) }
	{ r2 = mpyi(r0,r6); r7 = mpyi(r2,r18) }
	{ memw(r29+152) = r7; r5 = memw(r29+160); r0 = asr(r3,#0x1F); r4 = mpyi(r5,r4) }
	{ memb(r29+54) = r1.new; r3 += lsr(r0,#0x1A); r1 = mpyi(r2,r18) }

l0001A314:
	{ memb(r29+54) = r1.new; r3 += lsr(r0,#0x1A) }

l0001A320:
	{ r3 = #0x40; r2 = asr(r3,#0x6) }
	{ r2 = combine(r3.l,r2.l) }
	{ r1:r0 = combine(r3,r2) }
	{ l2fetch(r5,r1:r0) }
	{ if (p0.new) r3 = memw(r29+88); if (!p0.new) jump:nt 0001A6F4; p0 = cmp.gt(r6,#0x0) }

l0001A33C:
	{ memb(r29+37) = r2.new; r4 = memw(r29+80); r22 = #0x0; r2 = add(r20,#0x1) }
	{ memb(r29+60) = r4.new; r4 = add(r2,mpyi(r4,r3)) }

l0001A358:
	{ p0 = cmp.gt(r24,#0x0); r6 = r25; r23 = #0x0; if (!p0.new) jump:nt 0001A6E8 }
	{ r21 = memw(r29+220); r26 = memw(r29+92) }

l0001A370:
	{ r4 = memw(r29+220); r3 = memw(r29+172); p1 = cmp.eq(r23,#0x0); r2 = mpyi(r6,r24) }
	{ r5 = memw(r29+200); r7 = memw(r29+212) }
	{ if (p1) r2 = #0x0; p0 = cmp.gt(r2,r26); r3 = add(r25,r4); p2 = cmp.eq(r3,r23) }
	{ r4 = mux(p0,#0x1,#0x0); if (!p1) r2 = add(r20,#0xFFFFFFFF); if (p2) r3 = add(r4,#0x0); if (p2) r17 = sub(r3,r21) }
	{ p0 = cmp.eq(r5,#0x0); r4 = add(r4,r7); r25 = r20 }
	{ if (!p2) r3 = add(r4,r21); if (!p2) r17 = add(r4,#0x0) }
	{ r6 = sub(r6,r17); r5 = add(r17,r20); if (!p0) jump:nt 0001A3D0 }

l0001A3C4:
	{ if (!p1.new) r9 = memw(r29-4); if (!p1.new) r8 = memw(r29-128); if (!p1.new) jump:nt 0001A418; p1 = cmp.eq(r14,#0x0) }

l0001A3D0:
	{ r5 = memw(r29+156); r4 = memw(r29+148); r25 = r20 }
	{ memw(r29+256) = r6; r6 = memw(r29+152); r4 = add(r4,r17) }
	{ r5 = memw(r29+160); r3 = mpyi(r5,r3); r4 = add(#0x3F,mpyi(r4,r5)) }
	{ r7 = asr(r4,#0x1F); r5 += add(r3,r6) }
	{ r4 += lsr(r7,#0x1A) }
	{ r4 = #0x40; r7 = asr(r4,#0x6) }
	{ r3 = combine(r4.l,r7.l) }
	{ r1:r0 = combine(r4,r3) }
	{ l2fetch(r5,r1:r0) }
	{ jump 0001A45C }

l0001A418:
	{ memw(r29+256) = r6; r6 = memw(r29+164); r4 = mpyi(r9,r8) }
	{ r7 = memw(r29+248); r6 = add(r20,r6) }
	{ r7 = #0x40; r6 = mpyi(r6,r7) }
	{ r4 = memw(r29+168); r3 = add(#0x3F,mpyi(r4,r5)); r6 += mpyi(r3,r8) }
	{ r1 = asr(r3,#0x1F) }
	{ r3 += lsr(r1,#0x1A); r6 = add(r4,mpyi(r6,r9)) }
	{ r3 = asr(r3,#0x6) }
	{ r3 = combine(r7.l,r3.l) }
	{ r1:r0 = combine(r7,r3) }
	{ l2fetch(r6,r1:r0) }

l0001A45C:
	{ r20 = memw(r29+240); if (p0) r3 = memw(r29-92); r6 = r16; if (!p0) jump:nt 0001A574 }

l0001A46C:
	{ r7 = memw(r29+252); r4 = memw(r29+248); if (p0.new) r16 = add(r6,#0x0); p0 = cmp.eq(r22,#0x0) }
	{ if (p0) r18 = add(r27,#0x0); r20 = r7; r3 = add(r25,r3) }
	{ r3 = memw(r29+168); r5 = mpyi(r3,r4) }
	{ if (p0) r1 = add(r21,r5); if (!p0) jump:nt 0001A574; r20 = add(r3,mpyi(r20,r5)) }

l0001A49C:
	{ memb(r29+31) = r0.new; r0 = p2; r7 = add(PC,#0x10C74); immext(#0x10C40) }
	{ r3 = memw(r7-32); immext(#0xFFFFFFC0); r4 = memw(r7-112) }
	{ r24 = memw(r7-64); immext(#0xFFFFFFC0); r6 = memw(r7-16); immext(#0xFFFFFFC0) }
	{ r0 = memw(r29+120) }
	{ if (!p0.new) r6 = add(r2,r21); if (!p0.new) r7 = sub(r17,r2); if (!p0.new) jump:nt 0001A510; p0 = r0 }

l0001A4E0:
	{ r7 = memw(r29+252); r8 = memw(r29+100) }
	{ r0 = memw(r29+240); r2 = memw(r29+236) }
	{ r6 = memw(r29+168); if (!p0.new) r3 = add(r4,#0x0); if (!p0.new) r24 = add(r6,#0x0); p0 = r8 }
	{ r5:r4 = combine(r17,r21) }
	{ callr r24; r1 = add(r6,mpyi(r1,r7)) }
	{ jump 0001A568 }

l0001A510:
	{ memb(r29+8) = r3.new; r3 = memw(r29+96) }
	{ r0 = memw(r29+232) }
	{ memw(r29+28) = r4; r2 = memd(r29+104) }
	{ memw(r29+20) = r0; memw(r29+24) = r2 }
	{ r8 = memw(r29+224); r9 = memw(r29+228) }
	{ memw(r29+8) = r9; r12 = memw(r29+252) }
	{ memw(r29+4) = r8; r8 = memw(r29+168); r5 += mpyi(r6,r9) }
	{ memw(r29) = r25; r1 = memw(r29+108) }
	{ memw(r29+16) = r7; r4 = memw(r29+236) }
	{ memw(r29+12) = r6; r0 = memw(r29+240); r5 = add(r8,mpyi(r5,r12)) }
	{ r2 = memd(r29+116); r3 = memd(r29+112) }
	{ call fast_im2col_co }

l0001A568:
	{ r0 = memd(r29+124); r6 = r16; r27 = r18 }
	{ p2 = r0 }

l0001A574:
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 0001A59C; r2 = memw(r29+248) }

l0001A580:
	{ r1:r0 = memd(r29+136); r3 = memw(r29+244) }
	{ if (!p0.new) r2 = #0x0; p0 = cmp.gt(r3,r2) }
	{ if (!p2) r2 = add(r22,#0x0) }
	{ r2 = add(r27,mpyi(r2,r6)) }
	{ l2fetch(r2,r1:r0) }

l0001A59C:
	{ r18 = memw(r29+236); r16 = r6; r2 = r6; r24 = r22 }
	{ r3 = sub(#0x0,r18); r24 = add(r27,mpyi(r24,r6)) }
	{ r1:r0 = combine(r19,r24); call gvmsumb_asm }
	{ r13 = memw(r29+252); r12 = memw(r29+228); p0 = cmp.eq(r22,#0x0); r7 = mpyi(r18,r16) }
	{ if (!p0) r7 = memw(r29-48); if (p0) r1 = add(r27,#0x0); r0 = mpyi(r13,r12) }
	{ if (!p0) r20 = add(r25,#0x0); if (!p0) jump:nt 0001A664; r0 = add(r20,mpyi(r0,r21)) }

l0001A5EC:
	{ memb(r29+10) = r2.new; r2 = memw(r29+196); r20 = r25 }
	{ r2 = memw(r29+208) }
	{ memw(r29+24) = r2; memw(r29+36) = r5 }
	{ r14 = memw(r29+132); r4 = memw(r29+232) }
	{ memw(r29+12) = r17; memw(r29+20) = r19; r8 = sub(#0x0,r14); r6 = mpyi(r21,r4) }
	{ r5 = memw(r29+244); r3 = memw(r29+192) }
	{ memw(r29+8) = r20; r13 = memw(r29+224); r3 = r13; r12 = combine(r12.l,r3.l) }
	{ memw(r29+4) = r13; r2 = memw(r29+204); r9 = mpyi(r6,r5) }
	{ memw(r29) = r12; r15 = memw(r29+184); r7 = mpyi(r7,r14) }
	{ memw(r29+28) = r8; r8 = memw(r29+216); r6 = addasl(r15,r6,#0x2) }
	{ memw(r29+16) = r6; memw(r29+32) = r7; r2 += add(r9,r8) }
	{ call gvconvsum2dbbb_asm }
	{ jump 0001A6CC }

l0001A664:
	{ memb(r29+8) = r2.new; r2 = memw(r29+196) }
	{ r2 = memw(r29+188) }
	{ memw(r29+12) = r17; r5 = memw(r29+244); r7 = addasl(r2,r22,#0x2); r6 = mpyi(r21,r4) }
	{ memw(r29+8) = r20; r3 = memw(r29+192); r8 = mpyi(r6,r5) }
	{ r2 = memw(r29+204); r1 = memw(r29+216); r3 = r13; r12 = combine(r12.l,r3.l) }
	{ memb(r29+1) = r13.new; r13 = memw(r29+224); r1 = r24; r9 = add(r22,r1) }
	{ memw(r29+28) = r7; r14 = memw(r29+184) }
	{ memw(r29) = r12; r6 = addasl(r14,r6,#0x2) }
	{ memw(r29+16) = r6; call gvconv2dbbb_asm }

l0001A6CC:
	{ r24 = memw(r29+180); r25 = memw(r29+176); r21 = add(r17,r21) }
	{ r6 = memw(r29+256); r26 = sub(r26,r25) }
	{ if (!cmp.eq(r23.new,r24)) jump:t 0001A370; r23 = add(r23,#0x1) }

l0001A6EC:
	{ if (cmp.gtu(r2,r22.new)) jump:t 0001A358; r22 = add(r22,#0x20) }

l0001A6F4:
	{ r0 = memw(r29+68); r2 = memw(r29+208) }

l0001A6F8:
	{ r0 = memw(r29+68) }
	{ r18 = memw(r29+88) }
	{ if (!p0.new) r2 = memw(r29-24); r17 = memw(r2); if (p0.new) jump:nt 0001A74C; p0 = r0 }

l0001A710:
	{ r7 = memw(r29+76); r3 = memw(r29+220) }
	{ r5 = memw(r29+64); r0 = memw(r29+204) }
	{ r2 = memw(r29+244); r3 = mpyi(r3,r2); r1 = mpyi(r25,r2) }
	{ r4 = r1 }
	{ r7 = memw(r29+216); r3 += mpyi(r18,r7); r6 = mpyi(r3,r2) }
	{ r6 = memw(r29+60); r0 += add(r7,r6) }
	{ call unpad2d_bytes; r3 = add(r6,mpyi(r3,r5)) }

l0001A74C:
	{ r7 = memd(r29+72); r2 = memd(r29+84) }
	{ r22 = memw(r29+116); r2 = max(r2,r17) }
	{ immext(#0xFFFFFB40); immext(#0xFFFFFB40); r18 = add(r18,#0x1); r17 = r2 }
	{ memw(r16+8) = r17 }
	{ r5:r4 = memd(r29+48); r1 = #0x1; r0 = add(r16,#0x34); r3:r2 = combine(r1,r0) }
	{ r21:r20 = memd(r29+288); r19:r18 = memd(r29+296); r3:r2 = sub(r3:r2,r5:r4) }
	{ r25:r24 = memd(r29+272); r23:r22 = memd(r29+280) }
	{ memd(r16+72) = r3:r2; r17:r16 = memd(r29+304) }
	{ deallocframe; r27:r26 = memd(r29+264); jump 00009730 }

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
	{ r2 = #0x38D1B717; immext(#0x38D1B700) }
	{ r1:r0 = combine(r0,r2); jump fn00009600 }
0001A7B8                         00 00 00 00 00 00 00 00         ........

;; avgpool_execute: 0001A7C0
avgpool_execute proc
	{ allocframe(#0xC8); memd(r29-32) = r21:r20; r21 = r0 }
	{ memd(r29+184) = r19:r18; r2 = memw(r21+4) }
	{ r20 = memb(r21+32) }
	{ memd(r29+192) = r17:r16; r3 = memw(r21+8) }
	{ r17 = memw(r2+8); r19 = memw(r2); p0 = cmp.eq(r20,#0x0) }
	{ memd(r29+160) = r25:r24; r18 = memw(r2+4) }
	{ r7 = memw(r19+8) }
	{ memd(r29+168) = r23:r22; memd(r29+152) = r27:r26; r23:r22 = combine(r7,r1) }
	{ r6 = memw(r17+4); r16 = memw(r3); r0 = r7 }
	{ r26 = memw(r17+8); r24 = memw(r19); r1 = p0 }
	{ r27 = memw(r19+4); r25 = memw(r19+12); if (!p0) r2 = add(r26,r23) }
	{ memw(r29+72) = r6; r3 = memw(r18+4) }
	{ r6 = memw(r18+8) }
	{ memw(r29+108) = r6; memw(r29+64) = r3 }
	{ memw(r29+144) = r1; if (p0) jump:nt 0001A84C }

l0001A828:
	{ if (p0.new) r3 = memw(r29+108); if (p0.new) jump:nt 0001A848; p0 = cmp.eq(r12,#0x2) }

l0001A830:
	{ memb(r29+31) = r2.new; r2 = #0x0; if (!p0.new) jump:nt 0001A858; p0 = cmp.eq(r12,#0x1) }

l0001A840:
	{ jump 0001A84C; r0 += add(r23,#0xFFFFFFFF) }

l0001A848:
	{ r0 = sub(r2,r3) }

l0001A84C:
	{ r1 = r26; call fn00009760 }
	{ memw(r29+124) = r0 }

l0001A858:
	{ nop; if (p0.new) jump:nt 0001A894; p0 = cmp.eq(r12,#0x2) }

l0001A860:
	{ if (p0.new) r1 = memw(r29+72); if (!p0.new) r0 = memw(r29-112); if (p0.new) jump:nt 0001A888; p0 = cmp.eq(r12,#0x1) }

l0001A86C:
	{ memb(r29+17) = r2.new; r2 = #0x0 }
	{ if (p0.new) r1 = memw(r29+72); if (p0.new) r0 = add(r27,#0x0); if (!p0.new) jump:nt 0001A8AC }

l0001A884:
	{ jump 0001A8A0 }

l0001A888:
	{ r0 = r1 }
	{ jump 0001A8A0; r0 += add(r27,#0xFFFFFFFF) }

l0001A894:
	{ r3 = memd(r29+64); r1 = memd(r29+72) }
	{ r2 = add(r1,r27) }
	{ r0 = sub(r2,r3) }

l0001A8A0:
	{ call fn00009760 }
	{ memw(r29+68) = r0 }
	{ r2 = r22; r1 = #0x50; r4 = add(PC,#0xE057); immext(#0xE040) }

l0001A8AC:
	{ r2 = r22; r1 = #0x50; r4 = add(PC,#0x17) }

l0001A8B8:
	{ r3 = memw(r19+16); r7 = memw(r16+16) }
	{ memw(r29+100) = r3; memw(r29+104) = r7 }
	{ memw(r29) = r21; call logmsg_function }
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001A8E8; r2 = memw(r18) }

l0001A8D4:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001A8EC }

l0001A8DC:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001A8EC }

l0001A8E4:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001A908 }

l0001A8E8:
	{ r1 = #0x55; r3 = add(PC,#0xE034); immext(#0xE000) }

l0001A8EC:
	{ r1 = #0x55; r3 = add(PC,#0x34) }

l0001A8F4:
	{ r2 = r22; call errlog_function }

l0001A8F8:
	{ r2 = r22 }
	{ r0 = #0xFFFFFFFF; jump 0001AC2C }
0001A904             02 59 18 ED                             .Y..        

l0001A908:
	{ r4 = memd(r29+68); r3 = memd(r29+124); r1 = #0x57 }
	{ r7 = memw(r16+20); r2 = mpyi(r2,r3) }
	{ r2 = mpyi(r2,r4) }
	{ if (!cmp.gtu(r2.new,r7)) jump:t 0001A930; r2 = asl(r2,#0x2) }

l0001A928:
	{ jump 0001A8F8; r3 = add(PC,#0x10) }

l0001A930:
	{ if (!cmp.eq(r3.new,#0x0)) jump:t 0001A94C; r3 = memb(r21+32); p0 = cmp.gt(r24,#0x0) }

l0001A940:
	{ r1 = #0x58; jump 0001A8F8; r3 = add(PC,#0x6) }

l0001A94C:
	{ memw(r16+24) = r2; r2 = memd(r29+68); if (p0) r20 = #0x0; immext(#0x0) }
	{ memw(r29+128) = r26; r7 = memw(r29+124); if (p0) r26 = #0x3F800000; immext(#0x3F800000) }
	{ memw(r29+12) = r22 }
	{ memw(r16+4) = r2; memw(r29+16) = r21 }
	{ memw(r16+8) = r7; memw(r16) = r24 }
	{ memw(r16+12) = r25; if (!p0) jump:nt 0001AC0C }

l0001A984:
	{ r4 = memd(r29+68); r2 = memd(r29+124); r18 = mpyi(r23,r25) }
	{ r7 = memw(r29+64); r8 = memw(r29+108) }
	{ r1 = memw(r29+72); r6 = memw(r29+128); r4 = add(r4,#0xFFFFFFFF); r2 = add(r2,#0xFFFFFFFF) }
	{ memw(r29+56) = r27; memw(r29+120) = r23; r5 = sub(r7,r27); r3 = sub(r8,r23) }
	{ r3 = sub(#0xFFFFFFFF,r27); r4 = add(r5,mpyi(r4,r1)); r2 = add(r3,mpyi(r2,r6)) }
	{ memw(r29+48) = r3; r6 = #0x0; r3 = sub(#0xFFFFFFFF,r23); r4 += lsr(r4,#0x1F) }
	{ memw(r29+96) = r3; r5 = asl(r25,#0x2); r2 += lsr(r2,#0x1F) }
	{ memw(r29+92) = r5; memw(r29+60) = r6; r3 = #0x0; r6 = asr(r4,#0x1) }
	{ memw(r29+44) = r6; memw(r29+32) = r3; r4 = sub(#0x0,r6); r2 = asr(r2,#0x1) }
	{ memw(r29+88) = r2; r2 = sub(#0x0,r2); r3 = add(r6,sub(#0x7F,r7)); r5 = add(r2,sub(#0x7F,r8)) }
	{ memw(r29+20) = r3; memw(r29+24) = r4 }
	{ memw(r29+36) = r2; memw(r29+40) = r5 }

l0001AA08:
	{ memw(r29+28) = r24; r2 = memw(r29+68) }
	{ if (!p0.new) jump:nt 0001ABEC; p0 = cmp.gt(r2,#0x0) }

l0001AA14:
	{ memb(r29+21) = r2.new; r2 = memw(r29+24) }
	{ r7 = memd(r29+20); r3 = memd(r29+32) }
	{ memw(r29+80) = r7; r7 = #0x0; r2 = mpyi(r3,r2) }
	{ memw(r29+76) = r7; memw(r29+52) = r2 }

l0001AA30:
	{ if (!cmp.gt(r2.new,#0x0)) jump:nt 0001ABC8; r2 = memw(r29+124) }

l0001AA3C:
	{ r5 = memd(r29+76); r2 = memd(r29+72) }
	{ r7 = memd(r29+80); r1 = memd(r29+48); r2 = mpyi(r5,r2) }
	{ r3 = memd(r29+84); r4 = memd(r29+52) }
	{ r7 = memd(r29+40); r1 = #0x0; r4 = add(r5,r4); r5 = max(r7,r1) }
	{ memw(r29+132) = r1; r1 = memw(r29+44); r5 = sub(#0xFFFFFFFF,r5); r3 = max(r3,r6) }
	{ memw(r29+144) = r5; r5 = memw(r29+124); r2 = sub(r2,r1) }
	{ memw(r29+136) = r7; r7 = memw(r29+64); r4 = mpyi(r4,r5) }
	{ r7 = memd(r29+56); r2 = add(r2,r7); r16 = max(r2,r6) }
	{ memw(r29+116) = r4; r1 = memd(r29+60); r27 = min(r7,r2) }
	{ r4 = memw(r29+120); r3 = add(r1,r3) }
	{ memb(r29+35) = r0.new; r0 = memw(r29+36); r3 = mpyi(r4,r3) }

l0001AAA8:
	{ if (p0.new) r2 = memw(r29-128); if (p0.new) r3 = memw(r29-124); p0 = cmp.gt(r25,#0x0); if (!p0.new) jump:nt 0001AB94 }

l0001AAB8:
	{ r21 = #0x0; r7 = #0x0 }
	{ r4 = memw(r29+116); r1 = memw(r29+140) }
	{ r6 = memw(r29+136); r5 = memw(r29+96); r3 = add(r3,r4); r2 = mpyi(r3,r2) }
	{ r0 = memd(r29+88); r1 = memd(r29+112); r3 = mpyi(r3,r25); r4 = max(r1,r7) }
	{ r1 = memd(r29+100); r19 = memd(r29+92); r6 = add(r1,r4); r5 = max(r6,r5) }
	{ r0 = memw(r29+108); r5 = sub(#0xFFFFFFFF,r5); r2 = sub(r2,r0) }
	{ r1 = memd(r29+120); r7 = memd(r29+104); r19 = add(r1,mpyi(r19,r6)); r24 = max(r2,r7) }
	{ r17 = sub(r5,r4); r2 = add(r2,r0); r23 = addasl(r7,r3,#0x2) }
	{ r22 = min(r1,r2) }

l0001AB14:
	{ if (p0.new) r2 = memw(r29-112); p0 = cmp.gt(r27,r16); r1:r0 = combine(r20,r20); if (!p0.new) jump:nt 0001AB7C }

l0001AB24:
	{ r1:r0 = combine(r20,r20) }
	{ r2 = r19; r3 = sub(r2,r16) }
	{ loop1(0001AB34,r3) }
	{ p0 = cmp.gt(r22,r24); r5 = add(r17,#0xFFFFFFFF); if (!p0.new) jump:nt 0001AB70; r3 = addasl(r2,r25,#0x2) }

l0001AB44:
	{ r4 = memw(r2); p0 = cmp.gtu(r17,#0x1); r1 = sfadd(r1,r26); loop0(0001AB58,r5) }
	{ if (!p0) jump:nt 0001AB6C }

l0001AB58:
	{ r4 = memw(r3); r1 = sfadd(r1,r26); r0 = sfadd(r0,r4) }
	{ nop; r3 = addasl(r3,r25,#0x2) }

l0001AB6C:
	{ r0 = sfadd(r0,r4) }

l0001AB70:
	{ nop; nop; r2 = addasl(r2,r18,#0x2) }

l0001AB7C:
	{ r21 = add(r21,#0x1); call fn00009610 }
	{ memw(r23) = r0; r19 = add(r19,#0x4); p0 = cmp.eq(r21,r25); r23 = add(r23,#0x4) }
	{ if (!p0) jump:nt 0001AB14 }

l0001AB94:
	{ r6 = memw(r29+124); r4 = memw(r29+132) }
	{ r3 = memw(r29+140); r2 = memw(r29+128); r4 = add(r4,#0x1) }
	{ memw(r29+132) = r4; r7 = memw(r29+136); p0 = cmp.eq(r4,r6); r3 = add(r3,r2) }
	{ memw(r29+140) = r3; r5 = sub(r7,r2) }
	{ memw(r29+136) = r5; if (!p0) jump:nt 0001AAA8 }

l0001ABC8:
	{ r6 = memd(r29+68); r4 = memd(r29+76) }
	{ r3 = memd(r29+84); r2 = memd(r29+72); r4 = add(r4,#0x1) }
	{ memw(r29+76) = r4; r7 = memd(r29+80); p0 = cmp.eq(r4,r6); r3 = add(r3,r2) }
	{ r5 = sub(r7,r2) }
	{ memw(r29+80) = r5; memw(r29+84) = r3; if (!p0) jump:nt 0001AA30 }

l0001ABEC:
	{ r24 = memw(r29+28); r4 = memw(r29+32) }
	{ r3 = memd(r29+60); r2 = memd(r29+56); r4 = add(r4,#0x1) }
	{ memw(r29+32) = r4; r3 = add(r3,r2); p0 = cmp.eq(r4,r24) }
	{ memw(r29+60) = r3; if (!p0) jump:nt 0001AA08 }

l0001AC0C:
	{ memb(r29) = r2.new; r2 = memw(r29+16); r4 = add(PC,#0xDD48); immext(#0xDD40) }
	{ r2 = memw(r29+12) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001AC2C:
	{ r19:r18 = memd(r29+184); r17:r16 = memd(r29+192) }
	{ r23:r22 = memd(r29+168); r21:r20 = memd(r29+176) }
	{ r27:r26 = memd(r29+152); r25:r24 = memd(r29+160) }
	{ dealloc_return }

;; avgpool_check: 0001AC40
avgpool_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xDC34); immext(#0xDC00) }
	{ r17 = r0; r16 = r1; r1 = #0x8C }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001AC80; r2 = memw(r17+16); r1 = #0x8D }

l0001AC6C:
	{ r3 = add(PC,#0x25) }

l0001AC70:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001AC80:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001AC94; r2 = memw(r17+20); r1 = #0x8E }

l0001AC90:
	{ r3 = memw(r17+4) }

l0001AC94:
	{ jump 0001AC70; r3 = add(PC,#0xDC10); immext(#0xDC00) }
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
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001AD2C; r5 = memw(r2+16) }

l0001AD18:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x1) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001AD2C:
	{ dealloc_return }

;; errlog_function: 0001AD30
;;   Called from:
;;     0001A8F4 (in avgpool_execute)
;;     0001AC70 (in avgpool_check)
;;     0001AD20 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xDB25); immext(#0xDB00) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001AD54             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; biasadd_f_execute: 0001AD60
biasadd_f_execute proc
	{ allocframe(#0x38); memd(r29-16) = r17:r16; r3 = add(PC,#0xDC81); immext(#0xDC80) }
	{ r1 = #0x45; r17:r16 = combine(r0,r1) }
	{ memd(r29+40) = r19:r18; r2 = memw(r17+4) }
	{ memd(r29+24) = r23:r22; memd(r29+32) = r21:r20 }
	{ memd(r29+16) = r25:r24; r5 = memw(r2+4) }
	{ memd(r29+8) = r27:r26 }
	{ if (!cmp.eq(r4.new,#0x1)) jump:t 0001AE30; r4 = memw(r5+4) }

l0001AD94:
	{ r1 = #0x46; r3 = add(PC,#0x11) }
	{ if (!cmp.eq(r4.new,#0x1)) jump:t 0001AE30; r4 = memw(r5) }

l0001ADA8:
	{ r1 = #0x47; r3 = add(PC,#0x3D) }
	{ if (!cmp.eq(r4.new,#0x1)) jump:t 0001AE30; r4 = memw(r5+8) }

l0001ADBC:
	{ if (cmp.eq(r18.new,r4)) jump:t 0001ADDC; r18 = memw(r3+12) }

l0001ADC8:
	{ memw(r29+4) = r18; r1 = #0x49; r3 = add(PC,#0x28) }
	{ memw(r29) = r4; r2 = r16 }
	{ jump 0001AE34 }

l0001ADDC:
	{ r24 = memw(r3+4); r23 = memw(r3); r4 = add(PC,#0xDC28); immext(#0xDC00) }
	{ r6 = memw(r17+8); r22 = memw(r3+8); r2 = r16; r1 = #0x4C }
	{ r19 = memw(r5+16); r20 = memw(r3+16); r7 = mpyi(r23,r24) }
	{ r25 = memw(r6) }
	{ r3 = mpyi(r7,r22) }
	{ memw(r29) = r17; r21 = memw(r25+16) }
	{ r3 = mpyi(r3,r18) }
	{ call logmsg_function; r26 = asl(r3,#0x2) }
	{ r1 = #0x4D; r3 = add(PC,#0xDC02); immext(#0xDC00) }
	{ if (!cmp.gtu(r26,r2.new)) jump:t 0001AE40; r2 = memw(r25+20) }

l0001AE30:
	{ r2 = r16 }

l0001AE34:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001AEE4 }

l0001AE40:
	{ memw(r25) = r23; memw(r25+24) = r26; r2 = mpyi(r24,r23) }
	{ memw(r25+12) = r18; memw(r25+4) = r24; r3 = mpyi(r2,r22) }
	{ memw(r25+8) = r22 }
	{ r2 = #0x0; if (p0.new) jump:nt 0001AEC8; p0 = cmp.eq(r3,#0x0); loop1(0001AE68,r3) }

l0001AE68:
	{ r6 = add(r20,r2); r5 = add(r19,r2); if (p0.new) jump:nt 0001AEBC; p0 = cmp.eq(r10,#0x0) }

l0001AE74:
	{ r4 = add(r21,r2); p0 = cmp.gtu(r18,#0x1); r3 = add(r2,#0x4); r7 = add(r18,#0xFFFFFFFF) }
	{ r6 = memw(r6); r5 = memw(r5) }
	{ if (!p0) jump:nt 0001AEB4; loop0(0001AE90,r7) }

l0001AE90:
	{ memb(r4) = r5.new; r1 = add(r19,r3); r6 = add(r20,r3); r5 = sfadd(r6,r5) }
	{ r6 = memw(r6); r5 = memw(r1); r4 = add(r21,r3) }
	{ nop; r3 = r7 }

l0001AEB4:
	{ memb(r4) = r3.new; r3 = sfadd(r6,r5) }

l0001AEBC:
	{ nop; r21 = addasl(r21,r18,#0x2); r20 = addasl(r20,r18,#0x2) }

l0001AEC0:
	{ nop; r21 = addasl(r21,r18,#0x2) }

l0001AEC8:
	{ r2 = r16; r1 = #0x58; r4 = add(PC,#0xDB64); immext(#0xDB40) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }

l0001AEE4:
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ r27:r26 = memd(r29+8); r25:r24 = memd(r29+16) }
	{ dealloc_return }
0001AEF8                         00 40 00 7F 00 C0 00 7F         .@......

;; biasadd_check: 0001AF00
biasadd_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xDA83); immext(#0xDA80) }
	{ r17 = r0; r16 = r1; r1 = #0x5E }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x2)) jump:t 0001AF40; r2 = memw(r17+16); r1 = #0x5F }

l0001AF2C:
	{ r3 = add(PC,#0x34) }
	{ r2 = r16; call errlog_function }

l0001AF34:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001AF40:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001AF58; r2 = memw(r17+20); r1 = #0x60 }

l0001AF50:
	{ jump 0001AF34; r3 = add(PC,#0x27) }

l0001AF58:
	{ r2 = r16; r1 = #0x61; r4 = add(PC,#0xDA70); immext(#0xDA40) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001AF78
;;   Called from:
;;     0001AE14 (in biasadd_f_execute)
;;     0001AED8 (in biasadd_f_execute)
;;     0001AF14 (in biasadd_check)
;;     0001AF68 (in biasadd_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001AF9C; r5 = memw(r2+16) }

l0001AF88:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x20) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001AF9C:
	{ dealloc_return }

;; errlog_function: 0001AFA0
;;   Called from:
;;     0001AE34 (in biasadd_f_execute)
;;     0001AF30 (in biasadd_check)
;;     0001AF90 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xD9C4); immext(#0xD9C0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001AFC4             00 00 00 00 00 00 00 00 00 00 00 00     ............
0001AFD0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; concat_execute: 0001AFE0
concat_execute proc
	{ allocframe(#0x40); memd(r29-40) = r23:r22; r4 = add(PC,#0xDAE2); immext(#0xDAC0) }
	{ r23 = r0 }
	{ memd(r29+40) = r21:r20; r21 = memw(r23+4) }
	{ r3 = memw(r23+8) }
	{ memd(r29+24) = r25:r24; memd(r29+48) = r19:r18; r1 = #0x45; r25 = r1 }
	{ r18 = memw(r3); r5 = memw(r21+4); r2 = r25 }
	{ memd(r29+16) = r27:r26; memd(r29+56) = r17:r16 }
	{ r17 = memw(r21); r16 = memw(r23+16) }
	{ r20 = memw(r5+4); r19 = memw(r5+8) }
	{ r22 = memw(r18+16); r27 = memw(r5) }
	{ memw(r29) = r23; call logmsg_function }
	{ r2 = memw(r17+16) }
	{ if (!cmp.eq(r2.new,#0x3)) jump:t 0001B10C; r2 = memw(r2) }

l0001B040:
	{ memw(r29+12) = r23; r23 = r16; r24 = add(r21,#0x4) }
	{ r5:r4 = combine(#0x0,#0x2); r2 = #0x0; p0 = cmp.gt(r23,#0x0); r26 = #0x0 }
	{ if (p0) r3 = add(r24,#0x0); if (!p0) r26 = #0x0; if (p0) jump:nt 0001B128; r17 = mpyi(r3,r27) }

l0001B064:
	{ memw(r18) = r27; memw(r29+8) = r25; if (p0) r27 = #0x0 }
	{ memw(r18+8) = r19; memw(r18+4) = r20 }
	{ memw(r18+24) = r2; memw(r18+12) = r26 }
	{ if (!p0) jump:nt 0001B0D8 }

l0001B080:
	{ r16 = memw(r24); p0 = cmp.gt(r17,#0x0); r25 = r17; r18 = r22 }
	{ r2 = memw(r16+12); r20 = memw(r16+16); if (!p0) jump:nt 0001B0C0 }

l0001B098:
	{ r19 = asl(r2,#0x2) }
	{ nop }

l0001B0A0:
	{ r1:r0 = combine(r20,r18); r2 = r19; call fn00009560 }
	{ r2 = memw(r16+12); r18 = addasl(r18,r26,#0x2) }
	{ if (!cmp.eq(r25.new,#0x0)) jump:t 0001B0A0; r25 = add(r25,#0xFFFFFFFF); r20 = addasl(r20,r2,#0x2) }

l0001B0C0:
	{ r21 = r24; r3 = add(r21,#0x8); r22 = addasl(r22,r2,#0x2) }

l0001B0C4:
	{ r21 = r24; r3 = add(r21,#0x8) }

l0001B0CC:
	{ if (!cmp.eq(r27.new,r23)) jump:t 0001B080; r27 = add(r27,#0x1); r24 = r3 }

l0001B0D8:
	{ memb(r29) = r2.new; r2 = memw(r29+12); r4 = add(PC,#0xDA6A); immext(#0xDA40) }

l0001B0DC:
	{ memb(r29) = r2.new; r2 = memw(r29+12); r4 = add(PC,#0x2A) }

l0001B0EC:
	{ r2 = memw(r29+8) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001B0F8:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ r27:r26 = memd(r29+16); r25:r24 = memd(r29+24) }
	{ dealloc_return }

l0001B10C:
	{ r1 = #0x46; r3 = add(PC,#0xD9CF); immext(#0xD9C0) }

l0001B118:
	{ r2 = r25; call errlog_function }

l0001B11C:
	{ r2 = r25 }
	{ r0 = #0xFFFFFFFF; jump 0001B0F8 }

l0001B128:
	{ loop0(0001B12C,r23) }
	{ r6 = memw(r3) }
	{ if (cmp.eq(r7.new,r19)) jump:t 0001B14C; r7 = memw(r6+8) }

l0001B13C:
	{ memw(r29) = r2; r1 = #0x49; r3 = add(PC,#0x2E) }
	{ jump 0001B118 }

l0001B14C:
	{ if (cmp.eq(r7.new,r20)) jump:t 0001B168; r7 = memw(r6+4) }

l0001B158:
	{ memw(r29) = r2; r1 = #0x4C; r3 = add(PC,#0x2B) }
	{ jump 0001B118 }

l0001B168:
	{ if (cmp.eq(r7.new,r27)) jump:t 0001B184; r7 = memw(r6) }

l0001B174:
	{ memw(r29) = r2; r1 = #0x4F; r3 = add(PC,#0x29) }
	{ jump 0001B118 }

l0001B184:
	{ r6 = memw(r6+12); r5 = r3; r2 = add(r2,#0x1); r7 = add(r5,#0x8) }
	{ r26 = add(r6,r26); r3 = r7 }
	{ nop; r4 += mpyi(r17,r6) }
	{ r3 = memw(r18+20) }
	{ if (!cmp.gtu(r2.new,r3)) jump:t 0001B064; r2 = asl(r4,#0x2) }

l0001B1B0:
	{ r1 = #0x55; jump 0001B11C; r3 = add(PC,#0x8) }
0001B1BC                                     00 C0 00 7F             ....

;; concat_check: 0001B1C0
concat_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xD89A); immext(#0xD880) }
	{ r17 = r0; r16 = r1; r1 = #0x6A }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r2 = memw(r17+16); r1 = #0x6B; r3 = add(PC,#0xD896); immext(#0xD880) }
	{ if (cmp.gtu(r4.new,r2)) jump:t 0001B24C; r4 = #0x4 }

l0001B1F8:
	{ r1 = #0x6C; r3 = add(PC,#0xF) }
	{ if (!cmp.eq(r4.new,#0x1)) jump:t 0001B24C; r4 = memw(r17+20) }

l0001B20C:
	{ r4 = memw(r17+4) }
	{ if (!cmp.gtu(r2,r5.new)) jump:nt 0001B234; r5 = add(r5,#0x1); r4 = add(r4,#0x4) }

l0001B214:
	{ if (!cmp.gtu(r2,r5.new)) jump:nt 0001B238; r5 = add(r5,#0x1) }

l0001B220:
	{ if (!cmp.eq(r6.new,#0x0)) jump:t 0001B214; r6 = memw(r4); r3 = add(PC,#0x37) }

l0001B230:
	{ r1 = #0x6E }

l0001B234:
	{ r2 = memw(r17+8); r1 = #0x71; r3 = add(PC,#0xD86A); immext(#0xD840) }

l0001B238:
	{ r2 = memw(r17+8); r1 = #0x71; r3 = add(PC,#0x2A) }
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 0001B25C; r2 = memw(r2) }

l0001B24C:
	{ r2 = r16; call errlog_function }

l0001B250:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001B25C:
	{ r2 = r16; r1 = #0x73; r4 = add(PC,#0xD84E); immext(#0xD840) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001B27C
;;   Called from:
;;     0001B028 (in concat_execute)
;;     0001B0F0 (in concat_execute)
;;     0001B1D4 (in concat_check)
;;     0001B26C (in concat_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001B2A0; r5 = memw(r2+16) }

l0001B28C:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x34) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001B2A0:
	{ dealloc_return }

;; errlog_function: 0001B2A4
;;   Called from:
;;     0001B118 (in concat_execute)
;;     0001B24C (in concat_check)
;;     0001B294 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xD798); immext(#0xD780) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001B2C8                         00 00 00 00 00 00 00 00         ........

;; conv2d_f_execute_ref: 0001B2D0
conv2d_f_execute_ref proc
	{ allocframe(#0x78) }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+112) = r17:r16; memd(r29+104) = r19:r18 }
	{ r16 = memw(r2); r18 = memb(r0+32) }
	{ r4 = memw(r2+8); r19 = memw(r2+4); p0 = cmp.eq(r18,#0x0) }
	{ memd(r29+80) = r25:r24; r24 = memw(r16+8) }
	{ memd(r29+96) = r21:r20; memd(r29+72) = r27:r26 }
	{ memd(r29+88) = r23:r22 }
	{ memw(r29+44) = r0; r21 = memw(r3); r0 = r24 }
	{ r2 = memw(r16) }
	{ memw(r29+28) = r4; r3 = memw(r4+8) }
	{ r27 = memw(r19); r26 = memw(r19+4) }
	{ r23 = memw(r16+12); r25 = memw(r19+12) }
	{ r4 = memw(r4+4); r17 = memw(r16+4) }
	{ memw(r29+20) = r2; r7 = memw(r19+8); r2 = p0 }
	{ memw(r29+56) = r2; memw(r29+48) = r7; if (!p0) r2 = sub(r24,r26) }
	{ if (p0) jump:nt 0001B35C }

l0001B338:
	{ if (p0.new) r0 = add(r2,r3); if (p0.new) jump:nt 0001B35C; p0 = cmp.eq(r10,#0x2) }

l0001B340:
	{ memw(r29+60) = r3; r20 = r1; p0 = cmp.eq(r18,#0x1); r0 = #0x0 }
	{ if (!p0) jump:nt 0001B370 }

l0001B350:
	{ r3 = memd(r29+60); r1 = r20 }
	{ r0 = r3 }
	{ r0 += add(r24,#0xFFFFFFFF) }

l0001B35C:
	{ memw(r29+60) = r3; r20 = r1; r22 = r4; r1 = r3 }
	{ call fn00009760 }
	{ r4 = r22 }

l0001B370:
	{ r2 = sub(r17,r27); if (p0.new) jump:nt 0001B3A4; p0 = cmp.eq(r10,#0x2) }

l0001B378:
	{ memw(r29+64) = r0; if (!p0) r1:r0 = combine(r4,r4); if (p0.new) jump:nt 0001B39C; p0 = cmp.eq(r10,#0x1) }

l0001B384:
	{ r1 = memd(r29+56); r0 = #0x0; r18 = r4 }
	{ if (!p0) r1:r0 = combine(r18,r17); if (!p0.new) jump:nt 0001B3B4; p0 = r1 }

l0001B398:
	{ jump 0001B3B0 }

l0001B39C:
	{ jump 0001B3AC; r0 += add(r17,#0xFFFFFFFF) }

l0001B3A4:
	{ memw(r29+64) = r0; r1 = r4; r0 = add(r2,r4) }

l0001B3AC:
	{ r18 = r4 }

l0001B3B0:
	{ call fn00009760 }

l0001B3B4:
	{ r22 = memd(r29+44); r2 = memw(r19+16); r4 = add(PC,#0xD854); immext(#0xD840) }
	{ memw(r29+36) = r18; r19:r18 = combine(r18,r20); r1 = #0x5E }
	{ memw(r29+56) = r2; r7 = memw(r21+16); r2 = r18 }
	{ r16 = memw(r16+16); r3 = memw(r22+28) }
	{ memw(r29+40) = r0; memw(r29+4) = r3 }
	{ memw(r29+52) = r7; memw(r29+32) = r21 }
	{ memw(r29) = r22; call logmsg_function }
	{ memw(r29+12) = r23; r1 = #0x5F; r4 = add(PC,#0xD83E); immext(#0xD800) }
	{ memw(r29+4) = r17; r2 = r18 }
	{ memw(r29+8) = r24; r21 = memw(r29+20) }
	{ memw(r29) = r21; call logmsg_function }
	{ r20 = memd(r29+48); r2 = r18; r4 = add(PC,#0xD833); immext(#0xD800) }
	{ memw(r29+12) = r20; r1 = #0x60 }
	{ memw(r29+4) = r27; memw(r29+8) = r26 }
	{ memw(r29) = r25; call logmsg_function }
	{ r3 = memd(r29+60); r2 = r18; r4 = add(PC,#0xD827); immext(#0xD800) }
	{ memw(r29+4) = r3; r1 = #0x61 }
	{ memw(r29) = r19; call logmsg_function }
	{ r2 = r18; r1 = #0x62; r4 = add(PC,#0xD81F); immext(#0xD800) }
	{ r22 = memw(r29+40); r3 = memb(r22+32) }
	{ memw(r29) = r3; call logmsg_function }
	{ memw(r29) = r21; memw(r29+12) = r25; r4 = add(PC,#0xD811); immext(#0xD800) }
	{ r19 = memw(r29+64); r1 = #0x63 }
	{ memw(r29+4) = r22; memw(r29+8) = r19; r2 = r18 }
	{ call logmsg_function }
	{ r15 = r19; if (p0.new) jump:nt 0001B4B0; p0 = cmp.eq(r15,r12); r2 = mpyi(r21,r25) }

l0001B49C:
	{ r2 = r18; r1 = #0x65; r3 = add(PC,#0xD800); immext(#0xD800) }
	{ jump 0001B504 }

l0001B4B0:
	{ r3 = memd(r29+32); r6 = r21; r9 = r22 }
	{ memw(r29+16) = r18; r5 = r3 }
	{ r4 = memw(r3+20); r2 = mpyi(r2,r19) }
	{ r2 = mpyi(r2,r22) }
	{ if (!cmp.gtu(r2.new,r4)) jump:t 0001B4E8; r2 = asl(r2,#0x2) }

l0001B4D4:
	{ memw(r29+4) = r2; r1 = #0x67; r3 = add(PC,#0x21) }
	{ memw(r29) = r4; r2 = memd(r29+16) }
	{ jump 0001B504 }

l0001B4E8:
	{ r4 = memw(r29+28) }
	{ if (cmp.eq(r3.new,#0x1)) jump:t 0001B510; r3 = memw(r4) }

l0001B4F8:
	{ r1 = #0x69; r3 = add(PC,#0x17) }
	{ r2 = memw(r29+16) }

l0001B504:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001B728 }

l0001B510:
	{ if (cmp.eq(r3.new,#0x1)) jump:t 0001B52C; r3 = memw(r4+12); p0 = cmp.gt(r6,#0x0) }

l0001B520:
	{ r1 = #0x6A; jump 0001B504; r3 = add(PC,#0x0) }

l0001B52C:
	{ memw(r5+4) = r9; memw(r5+24) = r2; if (p0) r3 = sub(r26,r24); if (p0) r2 = add(r15,#0xFFFFFFFF) }
	{ memw(r5+8) = r15; memw(r5) = r6; if (p0) r4 = sub(r27,r17); if (p0) r7 = add(r9,#0xFFFFFFFF) }
	{ memw(r5+12) = r25; if (!p0) jump:nt 0001B704 }

l0001B554:
	{ r6 = memd(r29+36); r5 = memd(r29+60) }
	{ memb(r29+6) = r3.new; r3 = #0x0; r7 = add(r4,mpyi(r7,r6)); r2 = add(r3,mpyi(r2,r5)) }
	{ r4 = #0x0; immext(#0x0); r3 = mpyi(r23,r25) }
	{ r7 += lsr(r7,#0x1F); r2 += lsr(r2,#0x1F) }
	{ r6 = asr(r7,#0x1); r5 = mpyi(r5,r25) }
	{ memb(r29+12) = r2.new; r2 = asr(r2,#0x1) }

l0001B594:
	{ p0 = cmp.gt(r9,#0x0); if (!p0.new) jump:nt 0001B6F4 }

l0001B59C:
	{ memb(r29+11) = r7.new; r2 = memw(r29+24); r7 = #0x0 }
	{ memb(r29+8) = r2.new; r2 = mpyi(r2,r9) }

l0001B5B4:
	{ p0 = cmp.gt(r15,#0x0); r13 = #0x0; if (!p0.new) jump:nt 0001B6DC }
	{ r7 = memd(r29+44); r2 = memd(r29+36) }
	{ r0 = memd(r29+28); r6 = memd(r29+32) }
	{ r7 = add(r7,r6); r2 = mpyi(r7,r2) }
	{ memb(r29+16) = r1.new; r14 = sub(r2,r0); r1 = mpyi(r7,r15) }

l0001B5DC:
	{ if (p0.new) r28 = memw(r29+56); p0 = cmp.gt(r25,#0x0); if (!p0.new) jump:nt 0001B6D4 }
	{ r2 = memd(r29+64); r6 = memd(r29+60); r0 = #0x0 }
	{ r2 = add(r13,r2) }
	{ r6 = memw(r29+48); r2 = mpyi(r2,r25); r7 = mpyi(r13,r6) }
	{ r6 = memw(r29+52); r1 = sub(r7,r6) }
	{ r10 = addasl(r6,r2,#0x2) }

l0001B60C:
	{ p0 = cmp.gt(r27,#0x0); r7 = #0x0; r9 = r28; r11 = r4 }
	{ if (!p0) jump:nt 0001B6BC }

l0001B620:
	{ if (!cmp.gtu(r17,r2.new)) jump:nt 0001B6B0; r2 = add(r7,r14) }

l0001B62C:
	{ if (p0.new) r12 = add(r2,r8); if (p0.new) r21 = #0x0 }
	{ if (p0.new) r2 = add(r9,#0x0); p0 = cmp.gt(r26,#0x0); if (!p0.new) jump:nt 0001B6B0 }

l0001B640:
	{ r18 = mpyi(r12,r24); loop1(0001B648,r26) }
	{ if (!cmp.gtu(r24,r12.new)) jump:nt 0001B6A4; r12 = add(r21,r1) }

l0001B654:
	{ if (p0.new) r6 = add(r23,#0xFFFFFFFF); p0 = cmp.gt(r12,#0xFFFFFFFF) }
	{ r12 = add(r12,r18); if (!p0.new) jump:nt 0001B6A4; p0 = cmp.gt(r15,#0x0) }

l0001B664:
	{ r22 = memw(r2); p0 = cmp.gtu(r23,#0x1) }
	{ r12 = mpyi(r12,r23); loop0(0001B688,r6) }
	{ r12 = addasl(r2,r25,#0x2); r19 = addasl(r16,r12,#0x2) }
	{ r19 = memw(r19); r20 = add(r19,#0x4) }
	{ if (!p0) jump:nt 0001B6A0 }

l0001B688:
	{ r22 = memw(r12); r19 = memw(r20); r12 = addasl(r12,r25,#0x2); r11 += sfmpy(r19,r22) }
	{ nop; r20 = add(r20,#0x4) }

l0001B6A0:
	{ r11 += sfmpy(r19,r22) }

l0001B6A4:
	{ nop; r21 = add(r21,#0x1); r2 = addasl(r2,r3,#0x2) }

l0001B6B0:
	{ if (!cmp.eq(r7.new,r27)) jump:t 0001B620; r7 = add(r7,#0x1); r9 = addasl(r9,r5,#0x2) }

l0001B6BC:
	{ memw(r10) = r11; r10 = add(r10,#0x4); r28 = add(r28,#0x4); r0 = add(r0,#0x1) }

l0001B6C0:
	{ memw(r10) = r11; r10 = add(r10,#0x4); r28 = add(r28,#0x4) }

l0001B6CC:
	{ p0 = cmp.eq(r0,r25); if (!p0.new) jump:nt 0001B60C }

l0001B6D4:
	{ if (!cmp.eq(r13.new,r15)) jump:t 0001B5DC; r13 = add(r13,#0x1) }

l0001B6E0:
	{ r9 = memw(r29+40) }
	{ r2 = add(r2,#0x1) }
	{ memw(r29+44) = r2; p0 = cmp.eq(r2,r9); if (!p0.new) jump:nt 0001B5B4 }

l0001B6F4:
	{ r6 = memd(r29+20); r2 = memd(r29+24) }
	{ r2 = add(r2,#0x1) }
	{ memw(r29+24) = r2; if (!p0.new) jump:nt 0001B594; p0 = cmp.eq(r2,r6) }

l0001B704:
	{ memw(r29+12) = r25; r1 = #0x93; r4 = add(PC,#0xD5E9); immext(#0xD5C0) }
	{ memw(r29) = r6; r2 = memd(r29+16) }
	{ memw(r29+4) = r9; memw(r29+8) = r15 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001B728:
	{ r19:r18 = memd(r29+104); r17:r16 = memd(r29+112) }
	{ r23:r22 = memd(r29+88); r21:r20 = memd(r29+96) }
	{ r27:r26 = memd(r29+72); r25:r24 = memd(r29+80) }
	{ dealloc_return }

;; conv2d_check_ref: 0001B73C
conv2d_check_ref proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xD433); immext(#0xD400) }
	{ r17 = r0; r16 = r1; r1 = #0x9B }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001B778; r2 = memw(r17+16) }

l0001B764:
	{ r2 = memw(r17+28); r1 = #0x9C; r3 = add(PC,#0x27) }
	{ memw(r29) = r2; jump 0001B78C }

l0001B778:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001B79C; r2 = memw(r17+20); r1 = #0x9D }

l0001B788:
	{ r3 = add(PC,#0x1F) }

l0001B78C:
	{ r2 = r16; call errlog_function }

l0001B790:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001B79C:
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 0001B7B4; r2 = memw(r17+4); r1 = #0x9E }

l0001B7AC:
	{ jump 0001B790; r3 = add(PC,#0x12) }

l0001B7B4:
	{ if (!cmp.eq(r3.new,#0x0)) jump:nt 0001B7DC; r3 = memw(r17+8); r4 = #0x0 }

l0001B7C4:
	{ r1 = #0x9F; jump 0001B790; r3 = add(PC,#0x6) }

l0001B7D0:
	{ if (cmp.gtu(r4.new,#0x2)) jump:nt 0001B7F8; r4 = add(r4,#0x1); r2 = add(r2,#0x4) }

l0001B7D4:
	{ if (cmp.gtu(r4.new,#0x2)) jump:nt 0001B7FC; r4 = add(r4,#0x1) }

l0001B7DC:
	{ if (!cmp.eq(r5.new,#0x0)) jump:t 0001B7D0; r5 = memw(r2) }

l0001B7E0:
	{ if (!cmp.eq(r5.new,#0x0)) jump:t 0001B7D4 }

l0001B7E8:
	{ memw(r29) = r4; r1 = #0xA2; r3 = add(PC,#0x2F) }
	{ jump 0001B78C }

l0001B7F8:
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 0001B814; r2 = memw(r3); r1 = #0xA7 }

l0001B7FC:
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 0001B818; r2 = memw(r3) }

l0001B808:
	{ memw(r29) = #0x0; r3 = add(PC,#0x1D) }
	{ jump 0001B78C }

l0001B814:
	{ r2 = r16; r1 = #0xAA; r4 = add(PC,#0xD3DC); immext(#0xD3C0) }

l0001B818:
	{ r2 = r16; r1 = #0xAA; r4 = add(PC,#0x1C) }

l0001B824:
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

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
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001B858; r5 = memw(r2+16) }

l0001B844:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x11) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001B858:
	{ dealloc_return }

;; errlog_function: 0001B85C
;;   Called from:
;;     0001B504 (in conv2d_f_execute_ref)
;;     0001B78C (in conv2d_check_ref)
;;     0001B84C (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xD2F5); immext(#0xD2C0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; matmul_execute_ref: 0001B880
matmul_execute_ref proc
	{ allocframe(#0x58); r4 = add(PC,#0xD52A); immext(#0xD500) }
	{ r5 = memw(r0+8); r3 = memw(r0+4) }
	{ memd(r29+56) = r23:r22; memd(r29+80) = r17:r16; r1 = #0x4F; r16 = r1 }
	{ r23 = memw(r5); r6 = memw(r3+4); r2 = r16 }
	{ memd(r29+72) = r19:r18; r3 = memw(r3) }
	{ memd(r29+48) = r25:r24; memd(r29+64) = r21:r20 }
	{ memd(r29+40) = r27:r26; r5 = memw(r6+16) }
	{ r25 = memw(r6); r18 = memw(r6+8) }
	{ memw(r29+36) = r5; r24 = memw(r3+4) }
	{ r19 = memw(r6+12); r17 = memw(r3+16) }
	{ r20 = memw(r3+12); r27 = memw(r6+4) }
	{ r26 = memw(r3); r21 = memw(r3+8) }
	{ memw(r29) = r0; r22 = memw(r23+16); call logmsg_function }
	{ memw(r29+28) = r19; r1 = #0x52; r4 = add(PC,#0xD4DE); immext(#0xD4C0) }
	{ memw(r29+8) = r21; r2 = r16 }
	{ memw(r29+20) = r27; memw(r29+24) = r18 }
	{ memw(r29+12) = r20; memw(r29+16) = r25 }
	{ memw(r29) = r26; memw(r29+4) = r24 }
	{ call logmsg_function }
	{ p0 = cmp.eq(r24,#0x1); r1 = #0x53; r3 = add(PC,#0xD4D8); immext(#0xD4C0) }
	{ if (p0) r1 = #0x54; if (!p0) jump:nt 0001B988 }

l0001B92C:
	{ if (p0.new) r1 = #0x55; p0 = cmp.eq(r27,#0x1); r3 = add(PC,#0xD4C0); immext(#0xD4C0) }
	{ if (!p0) jump:nt 0001B988 }

l0001B940:
	{ if (p0.new) r1 = #0x56; p0 = cmp.eq(r26,#0x1); r3 = add(PC,#0xD4BE); immext(#0xD480) }
	{ if (!p0) jump:nt 0001B988 }

l0001B954:
	{ p0 = cmp.eq(r25,#0x1); r2 = mpyi(r26,r21); r3 = add(PC,#0xD4AA); immext(#0xD480) }
	{ if (p0) r4 = memw(r23+20); if (p0) r1 = #0x57; if (!p0) jump:nt 0001B988 }

l0001B970:
	{ r2 = mpyi(r2,r24); r3 = add(PC,#0xD4A5); immext(#0xD480) }
	{ r2 = mpyi(r2,r19) }
	{ if (!cmp.gtu(r2.new,r4)) jump:t 0001B998; r2 = asl(r2,#0x2) }

l0001B988:
	{ r2 = r16; call errlog_function }

l0001B98C:
	{ r2 = r16 }

l0001B990:
	{ r0 = #0xFFFFFFFF; jump 0001BA58 }

l0001B998:
	{ memw(r23+24) = r2; r28 = memw(r29+36); r0 = r22; r15 = r17 }
	{ memw(r23+12) = r19; memw(r23+4) = #0xFFFFFF81 }
	{ memw(r23+8) = r21; memw(r23) = #0x1; if (!p0.new) r2 = #0x0; p0 = cmp.eq(r21,#0x0) }
	{ if (!p0) r14 = add(r20,#0xFFFFFFFF); if (!p0) r3 = #0x0; immext(#0x0); if (p0) jump:nt 0001BA40 }

l0001B9D0:
	{ r4 = r28; if (p0.new) jump:nt 0001BA38; p0 = cmp.eq(r11,#0x0); r6 = mpyi(r2,r20) }

l0001B9DC:
	{ r5 = mpyi(r2,r19); loop1(0001B9EC,r19) }
	{ r6 = addasl(r15,r6,#0x2); r5 = addasl(r0,r5,#0x2) }
	{ r7 = r3; if (p0.new) jump:nt 0001BA2C; p0 = cmp.eq(r12,#0x0); r12 = addasl(r4,r19,#0x2) }

l0001B9F8:
	{ r8 = memw(r6); r9 = memw(r4); r13 = add(r6,#0x4); r7 = r3 }
	{ if (!p0.new) jump:nt 0001BA28; p0 = cmp.gtu(r12,#0x1); loop0(0001BA10,r14) }

l0001BA10:
	{ r9 = memw(r12); r8 = memw(r13); r12 = addasl(r12,r19,#0x2); r7 += sfmpy(r8,r9) }
	{ nop; r13 = add(r13,#0x4) }

l0001BA28:
	{ r7 += sfmpy(r8,r9) }

l0001BA2C:
	{ memw(r5) = r7; r4 = add(r4,#0x4); nop; r5 = add(r5,#0x4) }

l0001BA38:
	{ if (!cmp.eq(r2.new,r21)) jump:t 0001B9D0; r2 = add(r2,#0x1) }

l0001BA40:
	{ r2 = r16; r1 = #0x68; r4 = add(PC,#0xD3E6); immext(#0xD3C0) }

l0001BA44:
	{ r2 = r16; r1 = #0x68; r4 = add(PC,#0x26) }

l0001BA50:
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001BA58:
	{ r19:r18 = memd(r29+72); r17:r16 = memd(r29+80) }
	{ r23:r22 = memd(r29+56); r21:r20 = memd(r29+64) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+48) }
	{ dealloc_return }

;; matmul_check_ref: 0001BA6C
matmul_check_ref proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xD2C8); immext(#0xD2C0) }
	{ r17 = r0; r16 = r1; r1 = #0x6F }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1 = #0x70; r3 = add(PC,#0xD2C4); immext(#0xD2C0) }
	{ if (!cmp.eq(r2.new,#0x2)) jump:t 0001BAD8; r2 = memw(r17+16) }

l0001BAA0:
	{ r1 = #0x71; r3 = add(PC,#0x6) }
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001BAD8; r2 = memw(r17+20) }

l0001BAB4:
	{ r1 = #0x72; r3 = add(PC,#0x9) }
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0001BAD8; r2 = memw(r17+4) }

l0001BAC8:
	{ r1 = #0x73; r3 = add(PC,#0x1) }
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 0001BAE8; r2 = memw(r17+8) }

l0001BAD8:
	{ r2 = r16; call errlog_function }

l0001BADC:
	{ r2 = r16 }

l0001BAE0:
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001BAE8:
	{ r2 = r16; r1 = #0x74; r4 = add(PC,#0xD2AA); immext(#0xD280) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001BB08
;;   Called from:
;;     0001B8DC (in matmul_execute_ref)
;;     0001B910 (in matmul_execute_ref)
;;     0001BA50 (in matmul_execute_ref)
;;     0001BA80 (in matmul_check_ref)
;;     0001BAF8 (in matmul_check_ref)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001BB2C; r5 = memw(r2+16) }

l0001BB18:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x2) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001BB2C:
	{ dealloc_return }

;; errlog_function: 0001BB30
;;   Called from:
;;     0001B988 (in matmul_execute_ref)
;;     0001BAD8 (in matmul_check_ref)
;;     0001BB20 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xD1E6); immext(#0xD1C0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001BB54             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; maxpool_execute: 0001BB60
maxpool_execute proc
	{ allocframe(#0xA0); memd(r29-40) = r23:r22; r23:r22 = combine(r1,r0) }
	{ memd(r29+144) = r19:r18; r2 = memw(r22+4) }
	{ r18 = memb(r22+32) }
	{ memd(r29+136) = r21:r20; memd(r29+152) = r17:r16 }
	{ r3 = memw(r22+8); r20 = memw(r2+8); p0 = cmp.eq(r18,#0x0) }
	{ memd(r29+112) = r27:r26; r17 = memw(r2) }
	{ r26 = memw(r2+4) }
	{ memd(r29+120) = r25:r24; r0 = memw(r17+8); r1 = p0 }
	{ r16 = memw(r3); r7 = memw(r20+4) }
	{ r19 = memw(r20+8); r27 = memw(r17) }
	{ r24 = memw(r17+4); r25 = memw(r17+12) }
	{ memw(r29+76) = r7; r2 = memw(r26+4) }
	{ r7 = memw(r26+8) }
	{ memw(r29+68) = r2; memw(r29+36) = r0 }
	{ memw(r29+104) = r1; memw(r29+32) = r7 }
	{ nop; if (p0) jump:nt 0001BBE8 }

l0001BBC4:
	{ if (p0.new) jump:nt 0001BBDC; p0 = cmp.eq(r10,#0x2) }

l0001BBC8:
	{ r21 = #0x0; if (!p0.new) jump:nt 0001BBF4; p0 = cmp.eq(r10,#0x1) }

l0001BBD0:
	{ r2 = memd(r29+36); r0 = r19 }
	{ jump 0001BBE8; r0 += add(r2,#0xFFFFFFFF) }

l0001BBDC:
	{ r3 = memd(r29+32); r2 = memd(r29+36) }
	{ r2 = add(r19,r2) }
	{ r0 = sub(r2,r3) }

l0001BBE8:
	{ r1 = r19; call fn00009760 }
	{ r21 = r0 }

l0001BBF4:
	{ nop; if (p0.new) jump:nt 0001BC30; p0 = cmp.eq(r10,#0x2) }

l0001BBFC:
	{ if (p0.new) r1 = memw(r29+76); if (p0.new) jump:nt 0001BC24; p0 = cmp.eq(r10,#0x1) }

l0001BC04:
	{ memb(r29+18) = r2.new; r0 = memw(r29+104); r2 = #0x0 }
	{ if (p0.new) r1 = memw(r29+76); if (p0.new) r0 = add(r24,#0x0); if (!p0.new) jump:nt 0001BC48 }

l0001BC20:
	{ jump 0001BC3C }

l0001BC24:
	{ r0 = r1 }
	{ jump 0001BC3C; r0 += add(r24,#0xFFFFFFFF) }

l0001BC30:
	{ r3 = memd(r29+68); r1 = memd(r29+76) }
	{ r2 = add(r1,r24) }
	{ r0 = sub(r2,r3) }

l0001BC3C:
	{ call fn00009760 }
	{ memw(r29+72) = r0 }
	{ r2 = r23; r1 = #0x54; r4 = add(PC,#0xD2A7); immext(#0xD280) }

l0001BC48:
	{ r2 = r23; r1 = #0x54; r4 = add(PC,#0x27) }

l0001BC54:
	{ r3 = memw(r17+16); r7 = memw(r16+16) }
	{ memw(r29+100) = r3; memw(r29+104) = r7 }
	{ memw(r29) = r22; call logmsg_function }
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001BC84; r2 = memw(r26) }

l0001BC70:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001BC88 }

l0001BC78:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001BC88 }

l0001BC80:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001BCA4 }

l0001BC84:
	{ r1 = #0x59; r3 = add(PC,#0xD281); immext(#0xD280) }

l0001BC88:
	{ r1 = #0x59; r3 = add(PC,#0x1) }

l0001BC90:
	{ r2 = r23; call errlog_function }

l0001BC94:
	{ r2 = r23 }
	{ r0 = #0xFFFFFFFF; jump 0001BF7C }
0001BCA0 02 59 1B ED                                     .Y..            

l0001BCA4:
	{ r4 = memd(r29+72); r3 = memw(r16+20); r1 = #0x5B }
	{ r2 = mpyi(r2,r21) }
	{ r2 = mpyi(r2,r4) }
	{ if (!cmp.gtu(r2.new,r3)) jump:t 0001BCC8; r2 = asl(r2,#0x2) }

l0001BCC0:
	{ jump 0001BC94; r3 = add(PC,#0x21) }

l0001BCC8:
	{ if (!cmp.eq(r3.new,#0x0)) jump:t 0001BCE4; r3 = memb(r22+32); p0 = cmp.gt(r27,#0x0) }

l0001BCD8:
	{ r1 = #0x5C; jump 0001BC94; r3 = add(PC,#0x17) }

l0001BCE4:
	{ memw(r16+24) = r2; r2 = memd(r29+72); if (p0) r1 = #0xFF800000; immext(#0xFF800000) }
	{ memw(r29+12) = r22; memw(r29+8) = r23 }
	{ memw(r16) = r27; memw(r16+4) = r2 }
	{ memw(r16+12) = r25; memw(r16+8) = r21 }
	{ if (p0) r12 = memw(r29+32); if (p0) r9 = memw(r29+68); if (!p0) jump:nt 0001BF5C }

l0001BD10:
	{ r4 = memd(r29+72); r2 = r21; r13 = r12; r0 = #0x0 }
	{ r5 = memd(r29+36); r7 = memd(r29+76); r4 = sub(r9,r24); r8 = add(r4,#0xFFFFFFFF) }
	{ memw(r29+60) = r24; memw(r29+28) = r0; r3 = sub(r12,r5); r6 = mpyi(r5,r25) }
	{ r7 = sub(#0xFFFFFFFF,r5); r8 = add(r4,mpyi(r8,r7)) }
	{ memw(r29+96) = r7; r3 = sub(#0xFFFFFFFF,r24); r8 += lsr(r8,#0x1F); r2 = add(r3,mpyi(r2,r19)) }
	{ memw(r29+52) = r3; r7 = #0x0; r3 = asl(r25,#0x2); r2 += lsr(r2,#0x1F) }
	{ memw(r29+92) = r3; r4 = #0x0; r3 = asr(r8,#0x1) }
	{ memw(r29+48) = r3; memw(r29+64) = r7; r0 = sub(#0x0,r3) }
	{ memw(r29+20) = r0; r7 = asr(r2,#0x1); r2 = add(r3,sub(#0x7F,r9)) }
	{ memw(r29+16) = r2; r2 = sub(#0x0,r7); r0 = add(r7,sub(#0x7F,r12)) }
	{ memw(r29+40) = r2; memw(r29+44) = r0 }

l0001BD88:
	{ memw(r29+24) = r27; r2 = memw(r29+72) }
	{ if (!p0.new) jump:nt 0001BF38; p0 = cmp.gt(r2,#0x0) }

l0001BD94:
	{ memb(r29+22) = r2.new; r2 = memw(r29+20) }
	{ r0 = memd(r29+16); r3 = memd(r29+28) }
	{ memw(r29+84) = r0; r0 = #0x0; r2 = mpyi(r3,r2) }
	{ memw(r29+80) = r0; memw(r29+56) = r2 }

l0001BDB0:
	{ if (p0.new) r2 = memw(r29+76); if (p0.new) r9 = memw(r29+80); if (!p0.new) jump:nt 0001BF0C; p0 = cmp.gt(r13,#0x0) }

l0001BDBC:
	{ r12 = memw(r29+84); r14 = memw(r29+52) }
	{ r8 = memw(r29+56); r3 = memw(r29+88); r2 = mpyi(r9,r2) }
	{ r15 = memw(r29+64); r14 = memw(r29+48); r8 = add(r9,r8); r12 = max(r12,r14) }
	{ r0 = memw(r29+60); r14 = memw(r29+68); r17 = sub(r2,r14); r3 = max(r3,r4) }
	{ r26 = sub(#0xFFFFFFFF,r12); r8 = add(r17,r14); r3 = add(r15,r3); r20 = mpyi(r8,r21) }
	{ r28 = memw(r29+44); r16 = memw(r29+40); r9 = #0x0; r2 = max(r17,r4) }
	{ r15 = min(r0,r8); r12 = mpyi(r5,r3) }

l0001BE18:
	{ p0 = cmp.gt(r25,#0x0); r8 = add(r9,r20); if (!p0.new) jump:nt 0001BEFC; r3 = mpyi(r9,r19) }

l0001BE28:
	{ r17 = memd(r29+100); r0 = memd(r29+96); r11 = mpyi(r8,r25); r14 = max(r16,r4) }
	{ r23 = add(r12,r14); r3 = sub(r3,r7) }
	{ r0 = #0x0; r3 = add(r3,r13); r22 = max(r3,r4); r10 = max(r28,r0) }
	{ r10 = memw(r29+92); r18 = sub(#0xFFFFFFFF,r10) }
	{ r3 = memw(r29+104); r8 = sub(r18,r14); r10 = add(r17,mpyi(r10,r23)); r18 = min(r5,r3) }
	{ r27 = addasl(r3,r11,#0x2) }

l0001BE68:
	{ p0 = cmp.gt(r15,r2); r3 = r1; r14 = sub(r26,r2); if (!p0.new) jump:nt 0001BEE4 }

l0001BE78:
	{ r3 = r1; r11 = r10; loop1(0001BE84,r14) }
	{ r24 = add(r8,#0xFFFFFFFF); if (p0.new) jump:nt 0001BED8; p0 = cmp.gt(r10,r14); r23 = addasl(r11,r25,#0x2) }

l0001BE90:
	{ r14 = memw(r11); p0 = cmp.gtu(r8,#0x1) }
	{ if (p0) r17 = add(r24,#0xFFFFFFFF); if (!p0) r23 = add(r14,#0x0); if (!p0) jump:nt 0001BED4 }

l0001BEA4:
	{ r23 = memw(r23); p0 = cmp.gtu(r24,#0x1); r24 = addasl(r23,r25,#0x2); loop0(0001BEB8,r17) }
	{ if (!p0) jump:nt 0001BED0 }

l0001BEB8:
	{ r23 = memw(r24); r17 = r23; r24 = addasl(r24,r25,#0x2); r3 = sfmax(r14,r3) }
	{ nop; r14 = r17 }

l0001BED0:
	{ r3 = sfmax(r14,r3) }

l0001BED4:
	{ r3 = sfmax(r23,r3) }

l0001BED8:
	{ nop; nop; r11 = addasl(r11,r6,#0x2) }

l0001BEE4:
	{ memw(r27) = r3; r27 = add(r27,#0x4); r10 = add(r10,#0x4); r0 = add(r0,#0x1) }
	{ p0 = cmp.eq(r0,r25); if (!p0.new) jump:nt 0001BE68 }

l0001BEFC:
	{ if (!cmp.eq(r9.new,r21)) jump:t 0001BE18; r9 = add(r9,#0x1); r28 = sub(r28,r19); r16 = add(r16,r19) }

l0001BF0C:
	{ r3 = memd(r29+88); r2 = memd(r29+76) }

l0001BF10:
	{ r8 = memw(r29+80); r0 = memw(r29+84); r3 = add(r3,r2) }
	{ memw(r29+88) = r3; r2 = memd(r29+72); r8 = add(r8,#0x1); r3 = sub(r0,r2) }
	{ memw(r29+84) = r3; memw(r29+80) = r8; p0 = cmp.eq(r8,r2) }
	{ if (!p0) jump:nt 0001BDB0 }

l0001BF38:
	{ r27 = memw(r29+24); r8 = memw(r29+28) }
	{ r3 = memd(r29+64); r2 = memd(r29+60); r8 = add(r8,#0x1) }
	{ memw(r29+28) = r8; p0 = cmp.eq(r8,r27); r3 = add(r3,r2) }
	{ memw(r29+64) = r3; if (!p0) jump:nt 0001BD88 }

l0001BF5C:
	{ memb(r29) = r2.new; r2 = memw(r29+12); r4 = add(PC,#0xCFE1); immext(#0xCFC0) }
	{ r2 = memw(r29+8) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001BF7C:
	{ r19:r18 = memd(r29+144); r17:r16 = memd(r29+152) }
	{ r23:r22 = memd(r29+128); r21:r20 = memd(r29+136) }
	{ r27:r26 = memd(r29+112); r25:r24 = memd(r29+120) }
	{ dealloc_return }

;; maxpool_check: 0001BF90
maxpool_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xCED0); immext(#0xCEC0) }
	{ r17 = r0; r16 = r1; r1 = #0x8B }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001BFD0; r2 = memw(r17+16); r1 = #0x8C }

l0001BFBC:
	{ r3 = add(PC,#0x1) }

l0001BFC0:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001BFD0:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001BFE4; r2 = memw(r17+20); r1 = #0x8D }

l0001BFE0:
	{ r3 = memw(r17+4) }

l0001BFE4:
	{ jump 0001BFC0; r3 = add(PC,#0xCEAC); immext(#0xCE80) }
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
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001C07C; r5 = memw(r2+16) }

l0001C068:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x1D) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001C07C:
	{ dealloc_return }

;; errlog_function: 0001C080
;;   Called from:
;;     0001BC90 (in maxpool_execute)
;;     0001BFC0 (in maxpool_check)
;;     0001C070 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xCDC1); immext(#0xCDC0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001C0A4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; relu_execute: 0001C0B0
relu_execute proc
	{ allocframe(#0x28); memd(r29-16) = r17:r16; r4 = add(PC,#0xCF66); immext(#0xCF40) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+24) = r19:r18; r2 = memw(r17+4); r1 = #0x36 }
	{ r3 = memw(r17+8) }
	{ memd(r29+16) = r21:r20; memd(r29+8) = r23:r22 }
	{ r22 = memw(r3); r18 = memw(r2) }
	{ r6 = memw(r18+12); r0 = memw(r18) }
	{ r5 = memw(r18+8); r7 = memw(r18+4) }
	{ r20 = memw(r22+16); r19 = memw(r18+16); r2 = mpyi(r7,r0) }
	{ memw(r29) = r17 }
	{ r2 = r16; r3 = mpyi(r2,r5) }
	{ r21 = mpyi(r3,r6) }
	{ call logmsg_function; r23 = asl(r21,#0x2) }
	{ if (!cmp.gtu(r23,r2.new)) jump:t 0001C11C; r2 = memw(r22+20) }

l0001C108:
	{ r2 = r16; r1 = #0x37; r3 = add(PC,#0xA) }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001C170 }

l0001C11C:
	{ r3 = memw(r18+4); r2 = memw(r18); p0 = cmp.eq(r21,#0x0) }
	{ memw(r22) = r2; memw(r22+4) = r3 }
	{ r6 = memw(r18+8) }
	{ memw(r22+8) = r6 }
	{ r7 = memw(r18+12); r18 = #-0x1 }
	{ memw(r22+24) = r23; memw(r22+12) = r7; if (p0) jump:nt 0001C158 }

l0001C13C:
	{ r0 = memw(r19); r1 = r18; r21 = add(r21,#0xFFFFFFFF); call fn00009600 }
	{ memw(r20) = r0; r19 = add(r19,#0x4); p0 = cmp.eq(r21,#0x0); r20 = add(r20,#0x4) }
	{ if (!p0) jump:nt 0001C13C }

l0001C158:
	{ r2 = r16; r1 = #0x3F; r4 = add(PC,#0xCED5); immext(#0xCEC0) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }

l0001C170:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; relu_check: 0001C17C
relu_check proc
	{ jump relu_check__merged; r0 = #0x0 }

;; relu_check__merged: 0001C180
;;   Called from:
;;     0001C17C (in relu_check)
;;     0001C348 (in reluX_check)
relu_check__merged proc
	{ allocframe(#0x20); p0 = cmp.eq(r2,#0x1); r4 = add(PC,#0xCDE9); immext(#0xCDC0) }
	{ r7 = #0x1; r5 = #0x2; r6 = add(PC,#0xCE5E); immext(#0xCE40) }
	{ memd(r29+16) = r19:r18; memd(r29+24) = r17:r16; if (!p0) r4 = add(r6,#0x0); r16 = r1 }
	{ r3:r2 = combine(#0x61,r16); r19 = p0 }
	{ memw(r29+8) = r19; r17 = r0; r1 = mux(p0,#0x6A,r3) }
	{ memw(r29) = r17; r18 = mux(p0,r5,r7); call logmsg_function }
	{ if (cmp.eq(r2.new,r18)) jump:t 0001C1E4; r2 = memw(r17+16) }

l0001C1CC:
	{ r0 = memd(r29+8); r2 = r16; r3 = add(PC,#0x38) }
	{ if (!p0.new) r1 = #0x62; if (p0.new) r1 = #0x6B; jump 0001C20C; p0 = r0 }

l0001C1E4:
	{ r0 = memw(r29+8) }
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001C224; r2 = memw(r17+20); p1 = r0 }

l0001C1F8:
	{ if (!p1) r1 = #0x63; if (p1) r1 = #0x6C }
	{ r3 = add(PC,#0xCD8F); immext(#0xCD80) }
	{ r2 = r16 }

l0001C20C:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001C250 }
0001C218                         35 43 00 00 83 5B 49 6A         5C...[Ij
0001C220 F8 FF FF 59                                     ...Y            

l0001C224:
	{ if (!p1) r1 = #0x64; if (p1) r1 = #0x6D; if (p1) jump:nt 0001C23C }

l0001C230:
	{ jump 0001C244; r4 = add(PC,#0xCDD0); immext(#0xCDC0) }

l0001C23C:
	{ r4 = add(PC,#0xCD63); immext(#0xCD40) }

l0001C244:
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r0 = #0x0 }

l0001C250:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24) }
	{ dealloc_return }

;; reluX_execute: 0001C258
reluX_execute proc
	{ allocframe(#0x30); memd(r29-16) = r17:r16; r4 = add(PC,#0xCD5E); immext(#0xCD40) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+32) = r19:r18; r3 = memw(r17+4); r1 = #0x52 }
	{ r5 = memw(r17+8); r2 = r16 }
	{ memd(r29+24) = r21:r20; memd(r29+16) = r23:r22 }
	{ r23 = memw(r5); r19 = memw(r3) }
	{ memd(r29+8) = r25:r24; r3 = memw(r3+4) }
	{ r6 = memw(r19+4); r0 = memw(r19) }
	{ r8 = memw(r19+12); r7 = memw(r19+8); r5 = mpyi(r6,r0) }
	{ r20 = memw(r19+16); r3 = memw(r3+16); r5 = mpyi(r5,r7) }
	{ r21 = memw(r23+16) }
	{ memw(r29) = r17; r18 = memw(r3) }
	{ r22 = mpyi(r5,r8) }
	{ call logmsg_function; r24 = asl(r22,#0x2) }
	{ if (!cmp.gtu(r24,r2.new)) jump:t 0001C2D4; r2 = memw(r23+20) }

l0001C2BC:
	{ r2 = r16; r1 = #0x53; r3 = add(PC,#0x16) }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001C338 }

l0001C2D4:
	{ r3 = memw(r19+4); r2 = memw(r19); p0 = cmp.eq(r22,#0x0) }
	{ memw(r23) = r2; memw(r23+4) = r3 }
	{ r6 = memw(r19+8) }
	{ memw(r23+8) = r6 }
	{ r7 = memw(r19+12); r19 = #-0x1 }
	{ memw(r23+24) = r24; memw(r23+12) = r7; if (p0) jump:nt 0001C31C }

l0001C2F8:
	{ r0 = memw(r20); r1 = r19; call fn00009600 }
	{ r22 = add(r22,#0xFFFFFFFF); r1:r0 = combine(r0,r18); call fn000097B0 }
	{ memw(r21) = r0; r20 = add(r20,#0x4); p0 = cmp.eq(r22,#0x0); r21 = add(r21,#0x4) }
	{ if (!p0) jump:nt 0001C2F8 }

l0001C31C:
	{ r2 = r16; r1 = #0x5B; r4 = add(PC,#0xCCC0); immext(#0xCCC0) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }

l0001C338:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ dealloc_return; r25:r24 = memd(r29+8) }

;; reluX_check: 0001C348
reluX_check proc
	{ jump relu_check__merged; r1 = #0x1 }

;; logmsg_function: 0001C34C
;;   Called from:
;;     0001C0F4 (in relu_execute)
;;     0001C164 (in relu_execute)
;;     0001C1B4 (in relu_check__merged)
;;     0001C244 (in relu_check__merged)
;;     0001C2A8 (in reluX_execute)
;;     0001C32C (in reluX_execute)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001C370; r5 = memw(r2+16) }

l0001C35C:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x35) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001C370:
	{ dealloc_return }

;; errlog_function: 0001C374
;;   Called from:
;;     0001C110 (in relu_execute)
;;     0001C20C (in relu_check__merged)
;;     0001C2C8 (in reluX_execute)
;;     0001C364 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xCBD9); immext(#0xCBC0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001C398                         00 00 00 00 00 00 00 00         ........

;; softmax_execute: 0001C3A0
softmax_execute proc
	{ allocframe(#0x48) }
	{ r3 = memw(r0+4); r2 = memw(r0+8) }
	{ memd(r29+64) = r17:r16; memd(r29+48) = r21:r20 }
	{ r20 = memw(r3); r5 = memw(r2); r2 = r1 }
	{ memd(r29+40) = r23:r22; memd(r29+56) = r19:r18 }
	{ r4 = memw(r20+24); r7 = memw(r5+20) }
	{ memd(r29+24) = r27:r26; memd(r29+32) = r25:r24; if (p0.new) r1 = #0x40; p0 = cmp.gtu(r4,r7) }
	{ if (p0) jump:nt 0001C4F8 }

l0001C3D0:
	{ r2 = memw(r20+4); r4 = memw(r20) }
	{ memw(r29+8) = r2; r3 = memw(r20+8); r2 = mpyi(r2,r4) }
	{ r16 = memw(r20+12) }
	{ memw(r29) = r4; memw(r29+12) = r5 }
	{ memw(r29+4) = r3 }
	{ if (!cmp.gt(r25.new,#0x0)) jump:nt 0001C4C8; r26 = memw(r20+16); r25 = mpyi(r2,r3) }

l0001C3F8:
	{ r2 = memd(r29+12); r21 = #0x0 }
	{ r22 = memw(r2+16) }

l0001C400:
	{ r27 = memw(r26); r19:r18 = combine(r23,r16); p0 = cmp.gt(r16,#0x0) }
	{ r17 = r27; r0 = r27 }
	{ memb(r29+4) = r1.new; if (!p0) jump:nt 0001C4B4; r1 = p0 }

l0001C420:
	{ r1 = r17; call fn00009600 }

l0001C424:
	{ r1 = r17 }

l0001C428:
	{ if (!cmp.eq(r18.new,#0x0)) jump:t 0001C50C; r18 = add(r18,#0xFFFFFFFF); r17 = r0; r2 = add(r19,#0x4) }

l0001C43C:
	{ if (p0.new) r24 = add(r16,#0x0); if (p0.new) r19 = #0x0; if (!p0.new) jump:nt 0001C4B4; p0 = r0 }

l0001C44C:
	{ r18 = #0x0; immext(#0x0) }

l0001C454:
	{ r24 = add(r24,#0xFFFFFFFF); call fn00009780; r0 = sfsub(r27,r17) }
	{ p0 = cmp.eq(r24,#0x0); r2 = add(r22,r19) }
	{ memw(r2) = r0; if (p0) jump:nt 0001C484; r18 = sfadd(r18,r0) }

l0001C474:
	{ r19 = add(r19,#0x4); r2 = add(r23,r19) }
	{ r27 = memw(r2); jump 0001C454 }

l0001C484:
	{ r1 = r18; r0 = #0x3F800000; immext(#0x3F800000); call fn00009610 }
	{ r2 = memw(r29+16) }
	{ if (p0.new) r2 = add(r22,#0x0); if (!p0.new) jump:nt 0001C4B4; p0 = r2 }

l0001C4A0:
	{ loop0(0001C4A4,r16) }
	{ r3 = memw(r2) }
	{ r3 = sfmpy(r0,r3) }
	{ memw(r2) = r3; r2 = add(r2,#0x4); nop }

l0001C4B4:
	{ r26 = addasl(r26,r16,#0x2); r23 = addasl(r23,r16,#0x2) }
	{ if (!cmp.eq(r21.new,r25)) jump:t 0001C400; r21 = add(r21,#0x1); r22 = addasl(r22,r16,#0x2) }

l0001C4C8:
	{ r2 = memd(r29); r3 = memd(r29+12); r0 = #0x0 }

l0001C4CC:
	{ r2 = memd(r29); r3 = memd(r29+12) }

l0001C4D0:
	{ r6 = memd(r29+8); r7 = memd(r29+4) }
	{ memw(r3) = r2; memw(r3+8) = r7 }
	{ memw(r3+4) = r6; memw(r3+12) = r16 }
	{ r2 = memw(r20+24) }
	{ memw(r3+24) = r2 }

l0001C4E4:
	{ r19:r18 = memd(r29+56); r17:r16 = memd(r29+64) }
	{ r23:r22 = memd(r29+40); r21:r20 = memd(r29+48) }
	{ r27:r26 = memd(r29+24); r25:r24 = memd(r29+32) }
	{ dealloc_return }

l0001C4F8:
	{ call errlog_function; r3 = add(PC,#0xCBD6); immext(#0xCBC0) }
	{ r0 = #0xFFFFFFFF; jump 0001C4E4 }

l0001C50C:
	{ r0 = memw(r19) }
	{ jump 0001C420; r11 = r2 }

;; softmax_check: 0001C514
softmax_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xCB45); immext(#0xCB40) }
	{ r17 = r0; r16 = r1; r1 = #0x5A }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1 = #0x5B; r3 = add(PC,#0xCB42); immext(#0xCB40) }
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0001C5A8; r2 = memw(r17+4) }

l0001C548:
	{ r1 = #0x5C; r3 = add(PC,#0x3A) }
	{ if (cmp.eq(r4.new,#0x0)) jump:nt 0001C5A8; r4 = memw(r17+8) }

l0001C55C:
	{ r1 = #0x5D; r3 = add(PC,#0x33) }
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0001C5A8; r2 = memw(r2) }

l0001C570:
	{ r1 = #0x5E; r3 = add(PC,#0x2C) }
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0001C5A8; r2 = memw(r4) }

l0001C584:
	{ r1 = #0x5F; r3 = add(PC,#0x26) }
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 0001C5A8; r2 = memw(r17+16) }

l0001C598:
	{ r1 = #0x60; r3 = add(PC,#0x12) }
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001C5B8; r2 = memw(r17+20) }

l0001C5A8:
	{ r2 = r16; call errlog_function }

l0001C5AC:
	{ r2 = r16 }

l0001C5B0:
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001C5B8:
	{ r2 = r16; r1 = #0x61; r4 = add(PC,#0xCAFD); immext(#0xCAC0) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001C5D8
;;   Called from:
;;     0001C528 (in softmax_check)
;;     0001C5C8 (in softmax_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001C5FC; r5 = memw(r2+16) }

l0001C5E8:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x16) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001C5FC:
	{ dealloc_return }

;; errlog_function: 0001C600
;;   Called from:
;;     0001C4F8 (in softmax_execute)
;;     0001C5A8 (in softmax_check)
;;     0001C5F0 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xCA3A); immext(#0xCA00) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001C624             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; lrn_8_execute_ref: 0001C630
lrn_8_execute_ref proc
	{ allocframe(#0x88); r2 = r1 }
	{ r5 = memw(r0+4); r4 = memw(r0+8) }
	{ memd(r29+128) = r17:r16; memd(r29+120) = r19:r18 }
	{ r18 = memw(r5); r3 = memw(r4) }
	{ memd(r29+104) = r23:r22; memd(r29+112) = r21:r20 }
	{ r7 = memw(r18+24); r6 = memw(r3+20) }
	{ memd(r29+88) = r27:r26; memd(r29+96) = r25:r24; p0 = cmp.gtu(r7,r6) }
	{ r13 = memw(r5+8); r15 = memw(r5+4); if (!p0) jump:nt 0001C678 }

l0001C660:
	{ memw(r29+4) = r7; r1 = #0xA1; r3 = add(PC,#0xCAE2); immext(#0xCAC0) }
	{ memw(r29) = r6; jump 0001C770 }

l0001C678:
	{ r7 = memw(r5+20); r6 = memw(r5+16) }
	{ r7 = memw(r7+16); r8 = memw(r5+24) }
	{ r14 = memw(r4+8); r6 = memw(r6+16) }
	{ r4 = memw(r8+16); r28 = memw(r4+4) }
	{ r12 = memw(r13+16); r8 = memw(r15+16) }
	{ r1 = memw(r4); r25 = memw(r5+12) }
	{ r5 = memw(r18+4); r27 = memw(r8); r21 = r25 }
	{ r0 = memw(r18); r9 = memw(r18) }
	{ r19 = memw(r18+12); r17 = memw(r18+4) }
	{ r4 = memw(r12); r16 = memw(r18+8) }
	{ r6 = memw(r6); r7 = memw(r7) }
	{ memw(r3+4) = r5; memw(r3) = r9 }
	{ r8 = memw(r3+16); r5 = memw(r18+8) }
	{ memw(r3+8) = r5 }
	{ r5 = memw(r18+12) }
	{ memw(r3+12) = r5 }
	{ r5 = memw(r18+24) }
	{ memw(r3+24) = r5 }
	{ r3 = memw(r25) }
	{ memw(r29+44) = r8 }
	{ if (p0.new) memw(r29+60) = r6; if (!p0.new) r1 = #0xA6; if (!p0.new) jump:nt 0001C768; p0 = cmp.eq(r3,#0x1) }

l0001C700:
	{ memw(r29+64) = r7; memw(r29+44) = r8; p0 = cmp.gt(r0,#0x0) }
	{ memw(r29+8) = r28; memw(r29+68) = r1; if (p0) r1 = #0x437F0000; immext(#0x437F0000) }
	{ memw(r29+16) = r14; memw(r29+12) = r15 }
	{ memw(r29+24) = r0; memw(r29+20) = r13 }
	{ if (!p0) jump:nt 0001C9A0 }

l0001C730:
	{ r0 = sfsub(r4,r27) }
	{ memw(r29+40) = r0; call fn00009610 }
	{ r22 = r0; r7 = #0x0; r2 = #0x0; r20 = mpyi(r16,r19) }
	{ memw(r29+28) = r7; memw(r29+36) = r2 }

l0001C74C:
	{ if (!p0.new) jump:nt 0001C988; p0 = cmp.gt(r9,#0x0) }

l0001C750:
	{ r2 = memd(r29+28); r3 = #0x0 }
	{ r2 = mpyi(r2,r17) }
	{ memw(r29+32) = r2 }

l0001C75C:
	{ if (!p0.new) r3 = add(r3,#0x1); jump 0001C984; if (p0.new) jump:nt 0001C77C; p0 = cmp.gt(r8,#0x0) }

l0001C768:
	{ r3 = add(PC,#0xC9F4); immext(#0xC9C0) }

l0001C770:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001C9C4 }
0001C77C                                     04 40 63 70             .@cp
0001C780 97 1C 82 3C 02 42 04 F3 18 60 04 73 23 40 04 B0 ...<.B...`.s#@..
0001C790 06 C4 07 F3 02 50 02 ED D6 08 23 E9 0C C2 9D A1 .....P....#.....
0001C7A0 EC 60 CB 10 38 60 98 74 41 C1 9D 43 2A 67 FD 5B .`..8`.tA..C*g.[
0001C7B0 DC 46 8D 03 E0 C2 00 78 2C 67 FD 5B 00 7C 37 04 .F.....x,g.[.|7.
0001C7C0 01 30 00 28 1A 40 00 78 B7 1C C2 3C 17 42 18 F3 .0.(.@.x...<.B..
0001C7D0 22 40 18 B0 0E D8 9D A1 02 53 17 ED 07 57 13 E3 "@.......S...W..
0001C7E0 50 09 32 E9 14 C2 9D A1 0B 40 75 70 02 40 79 70 P.2......@up.@yp
0001C7F0 45 42 9D 91 98 C0 92 91 23 C0 8B 91 34 33 03 28 EB......#...43.(
0001C800 A4 DF 04 8E 02 C1 04 8E 85 41 04 8E 7C E2 C2 20 .........A..|.. 
0001C810 04 41 04 8C 06 40 70 70 43 40 8B 91 65 C0 8B 91 .A...@ppC@..e...
0001C820 E3 7F E3 BF E5 7F E5 BF A7 41 9D 91 C9 C1 9D 91 .........A......
0001C830 A3 5F 03 8E A5 5F 05 8E 07 47 24 F3 0C C0 73 70 ._..._...G$...sp
0001C840 09 46 07 E3 08 41 03 8C 04 59 04 F3 6E C2 9D 91 .F...A...Y..n...
0001C850 05 C1 05 8C 03 46 28 F3 06 49 28 F3 07 49 08 F3 .....F(..I(..I..
0001C860 08 C8 0E F3 1A 4C 03 E3 00 40 00 00 03 40 00 78 .....L...@...@.x
0001C870 09 DA 25 F3 0A 41 05 DE 0D 4C 25 F3 0C C0 65 70 ..%..A...L%...ep
0001C880 2C 40 1A E2 0D CD 18 F3 36 C2 C9 14 34 41 C2 11 ,@......6...4A..
0001C890 0E ED 06 FD 30 48 20 5C 00 C6 48 F2 24 48 20 5C ....0H \..H.$H \
0001C8A0 00 CF 50 F2 20 48 20 5C E0 7F 6F 75 00 60 09 74 ..P. H \..ou.`.t
0001C8B0 1C E0 0E 74 18 48 20 5C 00 C9 4C F2 08 C0 05 60 ...t.H \..L....`
0001C8C0 0E 41 C0 11 0A E0 1B 74 0A 40 CB 14 01 C0 3C 43 .A.....t.@....<C
0001C8D0 01 C0 41 8B 8A C1 16 EF 83 CA 0A EF 20 80 00 B0 ..A......... ...
0001C8E0 3C C0 1C B0 DC 68 FF 5C 00 47 0F F2 0E 53 0E F3 <....h.\.G...S..
0001C8F0 2F C0 0F B0 CA 44 72 14 0D 54 0D F3 22 C0 02 B0 /....Dr..T.."...
0001C900 15 40 6B 70 02 1D F0 3C 80 43 02 EF 44 E7 FD 5B .@kp...<.C..D..[
0001C910 22 C2 9D 91 02 C0 42 EB 40 5F C2 8C 34 E7 FD 5B ".....B.@_..4..[
0001C920 44 5F DB 8C 03 40 7B 70 82 C2 9D 91 02 DA 02 F3 D_...@{p........
0001C930 02 C2 38 3A 02 C0 42 8B 83 42 16 EF A2 C2 9D 91 ..8:..B..B......
0001C940 84 C3 00 EF 00 44 42 EB 6E E6 FD 5B 22 40 80 8B .....DB.n..["@..
0001C950 03 40 00 78 3A C0 1A B0 01 D3 1A F2 E2 5F 42 75 .@.x:........_Bu
0001C960 E0 7F 62 75 E2 FF 4F 7E 02 C0 83 74 3E 61 FF 5C ..bu..O~...t>a.\
0001C970 08 C2 17 AB 78 C2 9D 91 14 68 FF 5C 00 50 18 F2 ....x....h.\.P..
0001C980 43 C2 9D 43                                     C..C            

l0001C984:
	{ if (!p0.new) jump:nt 0001C75C; p0 = cmp.eq(r3,r9) }

l0001C988:
	{ r7 = memd(r29+24); r3 = memd(r29+28) }
	{ r2 = memd(r29+36); r3 = r3 }
	{ memw(r29+28) = r3; r2 = add(r2,r17); p0 = cmp.eq(r3,r7) }
	{ memw(r29+36) = r2; if (!p0) jump:nt 0001C74C }

l0001C9A0:
	{ r16 = memd(r29+20); r1 = memd(r29+12) }
	{ r0 = memw(r29+8); r18 = add(r16,#0x10); r2 = add(r1,#0x10); call lrn_8_execute_ref.extracted_region }
	{ r0 = memd(r29+16); r2 = r18; r1 = r16; call lrn_8_execute_ref.extracted_region }
	{ r0 = #0x0 }

l0001C9C4:
	{ r19:r18 = memd(r29+120); r17:r16 = memd(r29+128) }
	{ r23:r22 = memd(r29+104); r21:r20 = memd(r29+112) }
	{ r27:r26 = memd(r29+88); r25:r24 = memd(r29+96) }
	{ dealloc_return }

;; lrn_check: 0001C9D8
lrn_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xC71D); immext(#0xC700) }
	{ r17 = r0; r16 = r1; r1 = #0xCB }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x7)) jump:t 0001CA18; r2 = memw(r17+16); r1 = #0xCC }

l0001CA04:
	{ r3 = add(PC,#0xC) }
	{ r2 = r16; call errlog_function }

l0001CA0C:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001CA18:
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001CA30; r2 = memw(r17+20); r1 = #0xCD }

l0001CA28:
	{ jump 0001CA0C; r3 = add(PC,#0x3B) }

l0001CA30:
	{ r2 = r16; r1 = #0xCE; r4 = add(PC,#0xC700); immext(#0xC700) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001CA50
;;   Called from:
;;     0001C9EC (in lrn_check)
;;     0001CA40 (in lrn_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001CA74; r5 = memw(r2+16) }

l0001CA60:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001CA74:
	{ dealloc_return }

;; errlog_function: 0001CA78
;;   Called from:
;;     0001C770 (in lrn_8_execute_ref)
;;     0001CA08 (in lrn_check)
;;     0001CA68 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xC664); immext(#0xC640) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; lrn_8_execute_ref.extracted_region: 0001CA9C
;;   Called from:
;;     0001C9A4 (in lrn_8_execute_ref)
;;     0001C9B4 (in lrn_8_execute_ref)
lrn_8_execute_ref.extracted_region proc
	{ allocframe(#0x0) }
	{ r4 = memw(r1+4); r3 = memw(r1) }
	{ memw(r0) = r3; memw(r0+4) = r4 }
	{ r6 = memw(r1+8) }
	{ memw(r0+8) = r6 }
	{ r7 = memw(r1+12) }
	{ memw(r0+12) = r7 }
	{ r3 = memw(r1+24) }
	{ if (cmp.gtu(r3,r4.new)) jump:t 0001CAD4; r4 = memw(r0+20) }

l0001CAC8:
	{ r1 = memw(r2); r3 = memw(r1+24) }
	{ r2 = r3; call fn00009560 }

l0001CAD4:
	{ dealloc_return }
0001CAD8                         00 00 00 00 00 00 00 00         ........

;; lrn_f_execute: 0001CAE0
lrn_f_execute proc
	{ allocframe(#0xE0) }
	{ r3 = memw(r0+4); r2 = memw(r0+8) }
	{ memd(r29+216) = r17:r16; memd(r29+192) = r23:r22; r16 = r1 }
	{ r2 = memw(r2); r23 = memw(r3) }
	{ memd(r29+200) = r21:r20; memd(r29+208) = r19:r18 }
	{ r5 = memw(r23+24); r4 = memw(r2+20) }
	{ memd(r29+176) = r27:r26; memd(r29+184) = r25:r24; if (p0.new) r2 = add(r16,#0x0); p0 = cmp.gtu(r5,r4) }
	{ if (p0) memw(r29) = r4; memw(r29+52) = r23; if (!p0) jump:nt 0001CB2C }

l0001CB18:
	{ memw(r29+4) = r5; r1 = #0xA3; r3 = add(PC,#0xC6BD); immext(#0xC680) }
	{ jump 0001CC20 }

l0001CB2C:
	{ r5 = memw(r3+12); r4 = memw(r3+8) }
	{ r12 = memw(r3+4); r6 = memw(r3+16) }
	{ r4 = memw(r4+16); r5 = memw(r5+16) }
	{ r25 = memw(r23+4); r3 = memw(r6+16) }
	{ r6 = memw(r4); r5 = memw(r5) }
	{ r8 = memw(r23); r7 = memw(r23+4) }
	{ r22 = memw(r23+12); r24 = memw(r23+8) }
	{ r3 = memw(r3); r9 = memw(r23) }
	{ memw(r2+4) = r7; memw(r2) = r8 }
	{ r4 = memw(r23+8) }
	{ memw(r2+8) = r4 }
	{ r1 = memw(r23+12) }
	{ memw(r2+12) = r1 }
	{ r4 = memw(r23+24) }
	{ memw(r2+24) = r4; r7 = memw(r2+16) }
	{ memw(r29+112) = r12; r2 = memw(r12) }
	{ memw(r29+108) = r5; memw(r29+156) = r25 }
	{ memw(r29+104) = r6; p0 = cmp.eq(r2,#0x1); if (p0.new) r2 = add(r9,#0x0); if (p0.new) r0 = #0x0 }
	{ if (p0) memw(r29+32) = r2; memw(r29+44) = r7; if (!p0) jump:nt 0001CC10 }

l0001CBA8:
	{ p0 = cmp.gt(r9,#0x0) }
	{ if (!p0) jump:nt 0001CF94 }

l0001CBB0:
	{ r3 = togglebit(r3,#0x1E); r1:r0 = convert_sf2df(r6) }
	{ memw(r29+100) = r3; r4 = mpyi(r24,r22) }
	{ memw(r29+152) = r4 }
	{ memd(r29+88) = r1:r0; r4 = #0x0; r1:r0 = convert_sf2df(r5) }
	{ memw(r29+40) = r4; r4 = #0x0 }
	{ memw(r29+116) = r4 }
	{ memd(r29+80) = r1:r0; r1:r0 = convert_sf2df(r3) }
	{ memd(r29+72) = r1:r0 }

l0001CBE0:
	{ if (p0.new) r3 = memw(r29+116); p0 = cmp.gt(r25,#0x0); r9 = #0x0; if (!p0.new) jump:nt 0001CF7C }

l0001CBF0:
	{ r3 = mpyi(r3,r25) }
	{ memw(r29+36) = r3 }

l0001CBF8:
	{ memw(r29+60) = r9; if (p0.new) r3 = add(r9,#0x1); p0 = cmp.gt(r24,#0x0); if (p0.new) jump:nt 0001CC2C }

l0001CC08:
	{ r9 = add(r9,#0x1); jump 0001CF74 }

l0001CC10:
	{ r2 = r16; r1 = #0xA8; r3 = add(PC,#0xC5DF); immext(#0xC5C0) }

l0001CC20:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001CF94 }

l0001CC2C:
	{ r6 = memd(r29+40); r2 = memd(r29+36); r19 = #0x0 }
	{ memw(r29+120) = r3; r5 = add(r6,r9); r2 = add(r9,r2) }
	{ memw(r29+56) = r5; r2 = mpyi(r2,r24) }
	{ memw(r29+48) = r2 }

l0001CC4C:
	{ memw(r29+128) = r19; if (p0.new) jump:nt 0001CC5C; p0 = cmp.gt(r14,#0x0) }

l0001CC54:
	{ r19 = add(r19,#0x1); jump 0001CF64 }

l0001CC5C:
	{ memb(r29+16) = r3.new; r2 = memw(r29+48); r3 = add(r19,#0x1) }
	{ r3 = #0x0 }
	{ memb(r29+31) = r2.new; r2 = mpyi(r2,r22) }

l0001CC7C:
	{ memw(r29+136) = r3; r2 = memw(r12+12); r27 = r23; r1 = #0x69 }
	{ r5 = memw(r12+4); r6 = add(r2,#0xFFFFFFFF); r4 = add(PC,#0xC58F); immext(#0xC580) }
	{ r3 = memw(r23+16); r7 = memw(r12+8); r6 += lsr(r6,#0x1F) }
	{ memw(r29) = r5; memw(r29+8) = r2; r8 = add(r7,#0xFFFFFFFF) }
	{ memw(r29+4) = r7; r17 = r5; r8 += lsr(r8,#0x1F) }
	{ memw(r29+140) = r3; memw(r29+132) = r20; r18 = asr(r8,#0x1); r17 += lsr(r17,#0x1F) }
	{ memw(r29+16) = r18; r21 = asr(r17,#0x1); r20 = asr(r6,#0x1) }
	{ memw(r29+12) = r21; memw(r29+20) = r20; r23 = r9; r3:r2 = combine(#0x1,r16) }
	{ call logmsg_function }
	{ r2 = memd(r29+120); r4 = r22; r5 = r23; r7:r6 = combine(r19,r20) }
	{ r26 = r5 }
	{ r26 -= asr(r17,#0x1) }
	{ r2 += asr(r17,#0x1) }
	{ r27 = memw(r29+136); r19 = #0x0; immext(#0x0) }
	{ r17 = memd(r29+104); r2 = memd(r29+108); r26 = r5; r21 = add(r27,#0x1) }
	{ r20 = memw(r29+132); jump 0001CE98 }
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
	{ r1:r0 = memd(r29+88); r5:r4 = memd(r29+80); r7:r6 = convert_sf2df(r19); r17 += sfmpy(r2,r19) }
	{ memd(r29+16) = r7:r6; r3:r2 = combine(#0x1,r16) }
	{ memd(r29+8) = r5:r4; r19:r18 = convert_sf2df(r17); r4 = add(PC,#0xC3B5); immext(#0xC380) }
	{ memd(r29+24) = r19:r18; memd(r29) = r1:r0; r1 = #0x7C }
	{ call logmsg_function }
	{ r0 = r17; call fn00009790 }
	{ r2 = memw(r29+100) }
	{ call fn00009780; r0 = sfmpy(r0,r2) }
	{ r1 = #0x7F; r17 = r0; r4 = add(PC,#0xC396); immext(#0xC380) }
	{ memd(r29) = r19:r18; r3:r2 = combine(#0x1,r16); r7:r6 = convert_sf2df(r17) }
	{ memd(r29+16) = r7:r6; r7:r6 = memd(r29+72) }
	{ memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r2 = memw(r29+124); r1 = #0xBD; r4 = add(PC,#0xC2FB); immext(#0xC2C0) }
	{ r5 = memw(r29+116); r3 = memw(r29+140) }
	{ r19 = memw(r29+128); r2 = add(r2,r27) }
	{ r2 = addasl(r3,r2,#0x2) }
	{ memw(r29+12) = r27; r0 = memw(r2); r3:r2 = combine(#0x1,r16) }
	{ memw(r29) = r5; memw(r29+8) = r19 }
	{ memw(r29+4) = r26; r17 = sfmpy(r17,r0) }
	{ r7:r6 = convert_sf2df(r17) }
	{ memd(r29+16) = r7:r6; call logmsg_function }
	{ memw(r20) = r17; r3 = r21; r9 = r26; r20 = add(r20,#0x4) }
	{ r12 = memw(r29+112); if (!p0.new) jump:nt 0001CC7C; p0 = cmp.eq(r13,r14) }

l0001CF60:
	{ r7 = memd(r29+44); r19 = memd(r29+64) }

l0001CF64:
	{ if (p0.new) r2 = memw(r29+32); if (p0.new) r9 = memw(r29+120); p0 = cmp.eq(r19,r24); if (!p0.new) jump:nt 0001CC4C }

l0001CF74:
	{ p0 = cmp.eq(r9,r25); if (!p0.new) jump:nt 0001CBF8 }

l0001CF7C:
	{ r3 = memd(r29+40); r4 = memd(r29+116) }
	{ r3 = add(r3,r25); r4 = add(r4,#0x1) }
	{ memw(r29+40) = r3; memw(r29+116) = r4; if (!p0.new) jump:nt 0001CBE0; p0 = cmp.eq(r4,r2) }

l0001CF90:
	{ r0 = #0x0 }

l0001CF94:
	{ r19:r18 = memd(r29+208); r17:r16 = memd(r29+216) }
	{ r23:r22 = memd(r29+192); r21:r20 = memd(r29+200) }
	{ r27:r26 = memd(r29+176); r25:r24 = memd(r29+184) }
	{ dealloc_return }

;; lrn_check: 0001CFA8
lrn_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xC1DF); immext(#0xC1C0) }
	{ r17 = r0; r16 = r1; r1 = #0xCB }
	{ memw(r29) = r17; r3:r2 = combine(#0x2,r16); call logmsg_function }
	{ if (cmp.eq(r2.new,#0x5)) jump:t 0001CFEC; r2 = memw(r17+16); r1 = #0xCC }

l0001CFD8:
	{ r3 = add(PC,#0x8) }
	{ r2 = r16; call errlog_function }

l0001CFE0:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001CFEC:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001D008; r2 = memw(r17+20); r1 = #0xCE }

l0001CFFC:
	{ r1 = #0xCD; jump 0001CFE0; r3 = add(PC,#0x37) }

l0001D008:
	{ memw(r29) = r17; r3:r2 = combine(#0x2,r16); r4 = add(PC,#0xC1B8); immext(#0xC180) }
	{ call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

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
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001D044; r5 = memw(r2+16) }

l0001D034:
	{ r6 = add(r29,#0x10); r5 = add(r29,#0x10); r0 = add(PC,#0x3C) }
	{ memw(r29+4) = r6; call logv }

l0001D044:
	{ dealloc_return }

;; errlog_function: 0001D048
;;   Called from:
;;     0001CC20 (in lrn_f_execute)
;;     0001CFDC (in lrn_check)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xC124); immext(#0xC100) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001D06C                                     00 00 00 00             ....

;; nn_variable_read: 0001D070
nn_variable_read proc
	{ allocframe(#0x0) }
	{ r6 = memw(r1+28) }
	{ if (p0.new) r6 = memw(r1+8); if (p0.new) r7 = memw(r29+16); p0 = cmp.eq(r6,#0x3E); if (p0.new) jump:nt 0001D0A0 }

l0001D088:
	{ r1 = #0x7D; r3 = add(PC,#0xC217); immext(#0xC200) }
	{ r2 = r0; call errlog_function }

l0001D098:
	{ r2 = r0 }
	{ dealloc_return; r0 = #-0x1 }

l0001D0A0:
	{ r2 = memw(r14+r2<<#2) }
	{ if (!cmp.gtu(r6.new,r7)) jump:t 0001D0BC; r6 = memw(r2+24) }

l0001D0B0:
	{ r1 = #0x7E; jump 0001D098; r3 = add(PC,#0x2) }

l0001D0BC:
	{ r6 = memw(r2) }
	{ memw(r3) = r6 }
	{ memb(r4) = r6.new; r6 = memw(r2+4) }
	{ memw(r5) = r4; r3 = memd(r29+20) }
	{ memb(r7) = r1.new; r1 = memw(r2+12) }
	{ memb(r3) = r4.new }
	{ r2 = memw(r2+24); call fn00009560 }
	{ dealloc_return; r0 = #0x0 }

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
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xC18D); immext(#0xC180) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; nn_variable_write: 0001D118
nn_variable_write proc
	{ allocframe(#0x0); r6 = r2 }
	{ r7 = memw(r1+28) }
	{ if (p0.new) r7 = memw(r1+8); if (p0.new) r2 = memw(r29+16); p0 = cmp.eq(r7,#0x3E); if (p0.new) jump:nt 0001D148 }

l0001D130:
	{ r1 = #0x94; r3 = add(PC,#0xC16F); immext(#0xC140) }
	{ r2 = r0; call errlog_function }

l0001D140:
	{ r2 = r0 }
	{ dealloc_return; r0 = #-0x1 }

l0001D148:
	{ r6 = memw(r30+r6<<#2) }
	{ if (!cmp.gtu(r2,r7.new)) jump:t 0001D164; r7 = memw(r6+20) }

l0001D158:
	{ r1 = #0x95; jump 0001D140; r3 = add(PC,#0x1A) }

l0001D164:
	{ r1 = memd(r29+12); r0 = memw(r6+16) }
	{ memw(r6) = r3; memw(r6+24) = r2 }
	{ r3 = memw(r29+8) }
	{ memw(r6+4) = r4; memw(r6+8) = r5 }
	{ memw(r6+12) = r3; call fn00009560 }
	{ dealloc_return; r0 = #0x0 }

;; assign_execute: 0001D180
assign_execute proc
	{ allocframe(#0x20); memd(r29-16) = r17:r16; r4 = add(PC,#0xC1DC); immext(#0xC1C0) }
	{ r1 = #0xA3; r17:r16 = combine(r0,r1) }
	{ memd(r29+16) = r19:r18; r2 = memw(r17+16) }
	{ memw(r29+4) = r2; r2 = r16 }
	{ memw(r29) = r17; memd(r29+8) = r21:r20 }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0001D1F8; r2 = memw(r17+16) }

l0001D1B0:
	{ r2 = memw(r17+4) }
	{ r3 = memw(r13+r19); r2 = add(r2,r19) }
	{ r4 = memw(r3); r2 = memw(r2-4) }
	{ r5 = memw(r3+4) }
	{ memw(r2) = r4; memw(r2+4) = r5 }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:nt 0001D280; r6 = memw(r2+20) }

l0001D1E4:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }
	{ r2 = memw(r17+16); r19 = add(r19,#0x8) }
	{ if (cmp.gtu(r2,r18.new)) jump:t 0001D1B0; r18 = add(r18,#0x2) }

l0001D1F8:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0001D278; r2 = memw(r17+20); r0 = #0x0; r19:r18 = combine(#0x0,#0x4) }

l0001D1FC:
	{ if (cmp.eq(r2.new,#0x0)) jump:nt 0001D27C; r2 = memw(r17+20); r0 = #0x0 }

l0001D20C:
	{ r3 = memw(r17+4); r2 = memw(r17+8) }
	{ r2 = memw(r13+r19); r3 = memw(r21+r18) }
	{ r4 = memw(r3) }
	{ memb(r2+1) = r5.new; r5 = memw(r3+4) }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:nt 0001D25C; r6 = memw(r2+20) }

l0001D240:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }
	{ r2 = memw(r17+20); r18 = add(r18,#0x8); r19 = add(r19,#0x4) }
	{ if (cmp.gtu(r2,r20.new)) jump:t 0001D20C; r20 = add(r20,#0x1) }

l0001D25C:
	{ memw(r29) = r20; r1 = #0xAC; r3 = add(PC,#0xC139); immext(#0xC100) }

l0001D26C:
	{ r2 = r16; call errlog_function }
	{ r0 = #0xFFFFFFFF }

l0001D278:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24) }

l0001D27C:
	{ dealloc_return; r21:r20 = memd(r29+8) }

l0001D280:
	{ memw(r29) = r18; r1 = #0xA6; r3 = add(PC,#0xC0FE); immext(#0xC0C0) }
	{ jump 0001D26C }

;; assign_check: 0001D294
assign_check proc
	{ allocframe(#0x0); r2 = r1 }
	{ r3 = memw(r0+16) }
	{ if (p0.new) r1 = #0xB5; if (!p0.new) jump:nt 0001D2B0; p0 = tstbit(r3,#0x0) }

l0001D2A4:
	{ jump 0001D2C8; r3 = add(PC,#0xC097); immext(#0xC080) }

l0001D2B0:
	{ r4 = memw(r0+20); r0 = #0x0; r1 = #0xB6 }
	{ if (!cmp.gtu(r4,r3.new)) jump:t 0001D2D0; r3 = lsr(r3,#0x1) }

l0001D2C4:
	{ r3 = add(PC,#0xE) }

l0001D2C8:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF }

l0001D2D0:
	{ dealloc_return }

;; variable_execute: 0001D2D4
variable_execute proc
	{ allocframe(#0x8); r2 = r1; r4 = add(PC,#0xC04C); immext(#0xC040) }
	{ memw(r29) = r0; r1 = #0x2F; call logmsg_function }
	{ dealloc_return; r0 = #0x0 }
0001D2EC                                     00 C0 00 7F             ....

;; variable_check: 0001D2F0
variable_check proc
	{ allocframe(#0x20); memd(r29-16) = r17:r16; r4 = add(PC,#0xBFF8); immext(#0xBFC0) }
	{ r17 = r0; r16 = r1; r1 = #0x37 }
	{ memd(r29+8) = r21:r20; memd(r29+16) = r19:r18; r2 = r16 }
	{ memw(r29) = r17; call logmsg_function }
	{ r2 = memw(r17+20) }
	{ if (cmp.gtu(r3.new,r2)) jump:t 0001D3A4; r3 = memw(r17+16) }

l0001D324:
	{ if (p0) jump:nt 0001D398 }

l0001D328:
	{ r19:r18 = combine(#0x0,#0x0); r20 = #0x0 }
	{  }
	{ nop }
	{ r3 = memw(r17+4); r2 = memw(r17+8) }
	{ r2 = memw(r13+r19); r3 = memw(r21+r19) }
	{ r4 = memw(r3) }
	{ memb(r2+1) = r5.new; r5 = memw(r3+4) }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:nt 0001D384; r6 = memw(r2+20) }

l0001D368:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }
	{ r2 = memw(r17+16) }
	{ if (!cmp.gtu(r2,r20.new)) jump:nt 0001D398; r20 = add(r20,#0x1) }

l0001D380:
	{ r2 = memw(r17+20); r19 = add(r19,#0x4) }

l0001D384:
	{ r1 = #0x3B; r3 = add(PC,#0xBF8E); immext(#0xBF80) }

l0001D390:
	{ r18 = #-0x1; r2 = r16; call errlog_function }

l0001D398:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = r18 }
	{ dealloc_return; r21:r20 = memd(r29+8) }

l0001D3A4:
	{ jump 0001D390; r8 = #0x38; r3 = add(PC,#0xBF5E); immext(#0xBF40) }

;; variable_ctor: 0001D3B0
variable_ctor proc
	{ allocframe(#0x20); memd(r29-24) = r19:r18; r18 = r0 }
	{ memd(r29+24) = r17:r16; r17 = r5 }
	{ r19 = memw(r29+44) }
	{ memw(r29+4) = r19; r6 = memd(r29+40) }
	{ memw(r29) = r6; call node_alloc_common }
	{ if (cmp.eq(r16.new,#0x0)) jump:nt 0001D480; r16 = r0 }

l0001D3D8:
	{ memb(r29+2) = r0.new; if (p0) jump:nt 0001D3F8; r0 = p0 }

l0001D3E8:
	{ r2 = add(r2,#0x7F) }
	{ r2 = lsr(r2,#0x7) }
	{ r2 = mpyi(r2,r17) }
	{ r1 = asl(r2,#0x7) }

l0001D3F8:
	{ r0 = #0x80; call fn00009550 }
	{ memw(r16+40) = r0; if (!p0.new) jump:nt 0001D41C; p0 = cmp.eq(r0,#0x0) }

l0001D408:
	{ r2 = r18; r1 = #0x5F; r3 = add(PC,#0xBED1); immext(#0xBEC0) }
	{ call errlog_function }

l0001D41C:
	{ r0 = memw(r29+8) }
	{ if (p0.new) jump:nt 0001D474; p0 = r0 }

l0001D428:
	{ r2 = memw(r16+8); r4 = memw(r16+8); r5 = add(r17,#0xFFFFFFFF); r3 = #0x0 }
	{ p0 = cmp.gtu(r17,#0x1); loop0(0001D450,r5) }
	{ r4 = memw(r12+r3); r3 = add(r3,#0x4) }
	{ memw(r4+16) = r2 }
	{ r4 = memw(r19++#8); if (!p0) jump:nt 0001D474 }

l0001D450:
	{ r5 = memw(r16+8); r4 = add(r4,#0x7F) }
	{ r4 = and(r4,#0xFFFFFF80) }
	{ r5 = memw(r28+r3); r3 = add(r3,#0x4); r2 = add(r2,r4) }
	{ memw(r5+16) = r2 }
	{ r4 = memw(r19++#8); nop }

l0001D474:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = r16 }
	{ dealloc_return }

l0001D480:
	{ r2 = r18; r1 = #0x58; r3 = add(PC,#0xBE4E); immext(#0xBE40) }
	{ r16 = #0x0; call errlog_function }
	{ jump 0001D474 }

;; variable_dtor: 0001D49C
variable_dtor proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xBE1C); immext(#0xBE00) }
	{ r17 = r0; r16 = r1; r1 = #0x6B }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r0.new,#0x0)) jump:nt 0001D4C4; r0 = memw(r17+40) }

l0001D4C4:
	{ deallocframe; r17:r16 = memd(r29+8); r1:r0 = combine(r16,r17); jump node_free_common }

;; logmsg_function: 0001D4D0
;;   Called from:
;;     0001D1A0 (in assign_execute)
;;     0001D2E0 (in variable_execute)
;;     0001D30C (in variable_check)
;;     0001D4B0 (in variable_dtor)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001D4F4; r5 = memw(r2+16) }

l0001D4E0:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x25) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001D4F4:
	{ dealloc_return }

l0001D4F8:
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; reshape_execute: 0001D500
;;   Called from:
;;     0001D4FC (in logmsg_function)
reshape_execute proc
	{ allocframe(#0x80); memd(r29-16) = r17:r16; r17:r16 = combine(r0,r1) }
	{ memd(r29+88) = r25:r24; r2 = memw(r17+4) }
	{ memd(r29+96) = r23:r22 }
	{ memd(r29+104) = r21:r20; r3 = memw(r17+8) }
	{ r23 = memw(r2+4); r24 = memw(r2) }
	{ memd(r29+112) = r19:r18; r25 = memw(r3) }
	{ r4 = memw(r24+4); r7 = memw(r24) }
	{ r22 = memw(r24+12); r21 = memw(r24+8); r2 = mpyi(r4,r7) }
	{ memw(r29+72) = r7; memd(r29+80) = r27:r26; r6 = mpyi(r2,r21) }
	{ memw(r29+68) = r4 }
	{ if (!cmp.gt(r2.new,#0x3)) jump:t 0001D568; r2 = memw(r23+12); r18 = mpyi(r6,r22) }

l0001D55C:
	{ r4 = addasl(r3,r2,#0x2) }
	{ r26 = memw(r4-16); jump 0001D578 }

l0001D568:
	{ if (p0.new) r3 = memw(r23+16); r27:r26 = combine(#0x1,#0x1); if (!p0.new) jump:nt 0001D580; p0 = cmp.gt(r2,#0x2) }

l0001D574:
	{ r26 = #0x1 }

l0001D578:
	{ r3 = addasl(r3,r2,#0x2) }
	{ r27 = memw(r3-12) }

l0001D580:
	{ if (p0.new) r3 = memw(r23+16); if (!p0.new) jump:nt 0001D594; p0 = cmp.gt(r2,#0x1) }

l0001D588:
	{ r4 = addasl(r3,r2,#0x2) }
	{ r19 = memw(r4-8); jump 0001D5A0 }

l0001D594:
	{ r20 = #0x1; r19 = #0x1; if (!p0.new) jump:nt 0001D5A8; p0 = cmp.gt(r2,#0x0) }

l0001D59C:
	{ r3 = memw(r23+16); r19 = #0x1 }

l0001D5A0:
	{ r2 = addasl(r3,r2,#0x2) }
	{ r20 = memw(r2-4) }

l0001D5A8:
	{ r2 = r16; r1 = #0x3F; r4 = add(PC,#0xBEA7); immext(#0xBE80) }
	{ memw(r29) = r17; call logmsg_function }
	{ r2 = memw(r25+20); r1 = #0x41; r3 = add(PC,#0xBEB0); immext(#0xBE80) }
	{ if (cmp.gtu(r4.new,r2)) jump:t 0001D770; r4 = memw(r24+24) }

l0001D5D8:
	{ r1 = #0x43; r2 = lsr(r27,#0x1F); r3 = add(PC,#0x26) }
	{ r2 += lsr(r26,#0x1F) }
	{ r2 += lsr(r19,#0x1F) }
	{ r2 += lsr(r20,#0x1F) }
	{ r0 = r18; r2 = sub(#0x0,r26) }
	{ r2 = mpyi(r27,r2) }
	{ memb(r29+14) = r18.new; p0 = cmp.gt(r27,#0xFFFFFFFF); r18 = p0; r2 = mpyi(r2,r19) }
	{ memb(r29+15) = r18.new; p0 = cmp.gt(r19,#0xFFFFFFFF); r18 = p0 }
	{ memb(r29+13) = r18.new; p0 = cmp.gt(r20,#0xFFFFFFFF) }
	{ memw(r29+64) = r18; call fn00009760 }
	{ r0 = memw(r25+16); r3 = memw(r29+56); r18 = r0 }
	{ r3 = memw(r29+52); if (!p2.new) r26 = add(r18,#0x0); p2 = r3 }
	{ memw(r25) = r26 }
	{ r3 = memw(r29+60); if (!p0.new) r19 = add(r18,#0x0); p0 = r3 }
	{ memw(r25+8) = r19 }
	{ r3 = memw(r29+64); if (!p1.new) r27 = add(r18,#0x0); p1 = r3 }
	{ memw(r25+4) = r27 }
	{ if (p0.new) r18 = add(r20,#0x0); p0 = r3 }
	{ memw(r25+12) = r18 }
	{ r2 = memw(r24+24) }
	{ memw(r25+24) = r2 }
	{ r2 = memw(r24+24); r1 = memw(r24+16); call vmemcpy_asm }
	{ if (!cmp.eq(r2.new,#0x3)) jump:t 0001D71C; r2 = memw(r17+20) }

l0001D6A4:
	{ r3 = memw(r17+4); r2 = memw(r17+8) }
	{ r2 = memw(r2+4); r4 = memw(r3+8); r3 = add(PC,#0xBDEE); immext(#0xBDC0) }
	{ r5 = memw(r4+4); r7 = memw(r4) }
	{ memw(r2) = r7; memw(r2+4) = r5 }
	{ r6 = memw(r4+12); r0 = memw(r4+8) }
	{ memw(r2+12) = r6; memw(r2+8) = r0 }
	{ r5 = memw(r4+24) }
	{ if (cmp.gtu(r5,r7.new)) jump:t 0001D770; r7 = memw(r2+20) }

l0001D6D4:
	{ r1 = memw(r4+16); r2 = memw(r4+24); call fn00009560 }
	{ r3 = memw(r17+4); r2 = memw(r17+8); r1 = #0x54 }
	{ r4 = memw(r3+12); r3 = add(PC,#0xBDB2); immext(#0xBD80) }
	{ r2 = memw(r2+8) }
	{ r5 = memw(r4+4); r7 = memw(r4) }
	{ memw(r2) = r7; memw(r2+4) = r5 }
	{ r6 = memw(r4+12); r0 = memw(r4+8) }
	{ memw(r2+12) = r6; memw(r2+8) = r0 }
	{ r5 = memw(r4+24) }
	{ if (cmp.gtu(r5,r7.new)) jump:t 0001D770; r7 = memw(r2+20) }

l0001D714:
	{ r1 = memw(r4+16); r2 = memw(r4+24); call fn00009560 }

l0001D71C:
	{ r3 = memw(r23+4); r2 = memw(r23); r4 = add(PC,#0xBD89); immext(#0xBD80) }
	{ r6 = memw(r23+12); r5 = memw(r23+8); r1 = #0x5D }
	{ memw(r29+44) = r18; r7 = memd(r29+68) }
	{ memw(r29+28) = r6; memw(r29+40) = r19 }
	{ memw(r29+32) = r26; memw(r29+36) = r27 }
	{ memw(r29+20) = r3; memw(r29+24) = r5 }
	{ memw(r29+8) = r21; r3 = memd(r29+72) }
	{ memw(r29+12) = r22; memw(r29+16) = r2; r2 = r16 }
	{ memw(r29) = r3; memw(r29+4) = r7 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001D75C:
	{ r19:r18 = memd(r29+112); r17:r16 = memd(r29+120) }
	{ r23:r22 = memd(r29+96); r21:r20 = memd(r29+104) }
	{ r27:r26 = memd(r29+80); r25:r24 = memd(r29+88) }
	{ dealloc_return }

l0001D770:
	{ r2 = r16; call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001D75C }

;; reshape_check: 0001D780
reshape_check proc
	{ jump reshape_check__merged; r0 = #0x0 }

;; reshape_check__merged: 0001D784
;;   Called from:
;;     0001D780 (in reshape_check)
;;     0001D840 (in qreshape_check)
reshape_check__merged proc
	{ allocframe(#0x28); p0 = cmp.eq(r2,#0x1); r4 = add(PC,#0xBC99); immext(#0xBC80) }
	{ memd(r29+24) = r19:r18; memd(r29+32) = r17:r16; if (p0) r19 = #0x6F; r17:r16 = combine(r0,r1) }
	{ if (p0) r1 = #0x6C; r18 = add(PC,#0xBC9A); immext(#0xBC80) }
	{ memd(r29+8) = r23:r22; memd(r29+16) = r21:r20; if (p0) jump:nt 0001D7C4 }

l0001D7B0:
	{ r1 = #0x63; r21:r20 = combine(#0x64,#0x65); r19 = #0x66; r23:r22 = combine(#0x2,#0x1) }
	{ jump 0001D7DC }

l0001D7C4:
	{ r21:r20 = combine(#0x6D,#0x6E); r23:r22 = combine(#0x4,#0x3); r4 = add(PC,#0xBC06); immext(#0xBC00) }
	{ r18 = add(PC,#0xBC2F); immext(#0xBC00) }

l0001D7DC:
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,r23)) jump:t 0001D808; r2 = memw(r17+16); r1 = r21 }

l0001D7F4:
	{ r3 = add(PC,#0x34) }
	{ r2 = r16; call errlog_function }

l0001D7FC:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; jump 0001D834 }

l0001D808:
	{ if (cmp.eq(r2.new,r22)) jump:t 0001D820; r2 = memw(r17+20); r1 = r20 }

l0001D818:
	{ jump 0001D7FC; r3 = add(PC,#0x1F) }

l0001D820:
	{ memw(r29) = r17; r4 = r18; r1 = r19; r2 = r16 }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001D834:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; qreshape_check: 0001D840
qreshape_check proc
	{ jump reshape_check__merged; r1 = #0x1 }

;; logmsg_function: 0001D844
;;   Called from:
;;     0001D5B4 (in reshape_execute)
;;     0001D754 (in reshape_execute)
;;     0001D7DC (in reshape_check__merged)
;;     0001D82C (in reshape_check__merged)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001D868; r5 = memw(r2+16) }

l0001D854:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x1D) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001D868:
	{ dealloc_return }

;; errlog_function: 0001D86C
;;   Called from:
;;     0001D770 (in reshape_execute)
;;     0001D7F8 (in reshape_check__merged)
;;     0001D85C (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xBB41); immext(#0xBB40) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; slice_execute_f: 0001D890
slice_execute_f proc
	{ jump slice_impl; r4 = #0x4 }

;; slice_check_f: 0001D894
slice_check_f proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xBC5F); immext(#0xBC40) }
	{ r17 = r0; r16 = r1; r1 = #0x85 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001D8C8; r2 = memw(r17+16); r1 = #0x86 }

l0001D8C0:
	{ jump 0001D8E4; r3 = add(PC,#0xE) }

l0001D8C8:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001D8EC; r2 = memw(r17+20); r0 = #0x0; r1 = #0x87 }

l0001D8DC:
	{ r3 = add(PC,#0x3D) }
	{ r2 = r16; call errlog_function }

l0001D8E4:
	{ r2 = r16 }

l0001D8E8:
	{ r0 = #0xFFFFFFFF }

l0001D8EC:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; slice_execute_8: 0001D8F0
slice_execute_8 proc
	{ jump slice_impl; r1 = #0x1 }

;; slice_check_8: 0001D8F4
slice_check_8 proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xBBFF); immext(#0xBBC0) }
	{ r17 = r0; r16 = r1; r1 = #0x8D }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001D928; r2 = memw(r17+16); r1 = #0x8E }

l0001D920:
	{ jump 0001D944; r3 = add(PC,#0x2E) }

l0001D928:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001D94C; r2 = memw(r17+20); r0 = #0x0; r1 = #0x8F }

l0001D93C:
	{ r3 = add(PC,#0x1D) }
	{ r2 = r16; call errlog_function }

l0001D944:
	{ r2 = r16 }

l0001D948:
	{ r0 = #0xFFFFFFFF }

l0001D94C:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; slice_execute_q8: 0001D950
slice_execute_q8 proc
	{ allocframe(#0x8); memd(r29-16) = r17:r16; r17:r16 = combine(r1,r0) }
	{ r4 = memw(r16+8); r2 = memw(r16+4) }
	{ r2 = memw(r4+4); r3 = memw(r2+12) }
	{ r5 = memw(r3+4); r7 = memw(r3) }
	{ memw(r2) = r7; memw(r2+4) = r5 }
	{ r0 = memw(r3+12); r4 = memw(r3+8) }
	{ memw(r2+8) = r4; memw(r2+12) = r0 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:t 0001D988; r6 = memw(r2+20) }

l0001D980:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }

l0001D988:
	{ r3 = memw(r16+4); r2 = memw(r16+8) }
	{ r2 = memw(r2+8); r3 = memw(r3+16) }
	{ r5 = memw(r3+4); r4 = memw(r3) }
	{ memw(r2) = r4; memw(r2+4) = r5 }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:t 0001D9B8; r6 = memw(r2+20) }

l0001D9B0:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }

l0001D9B8:
	{ deallocframe; r17:r16 = memd(r29); r2 = #0x1; r1:r0 = combine(r17,r16) }
	{ jump slice_impl }
0001D9C8                         00 40 00 7F 00 C0 00 7F         .@......

;; slice_check_q8: 0001D9D0
slice_check_q8 proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xBB23); immext(#0xBB00) }
	{ r17 = r0; r16 = r1; r1 = #0x95 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x5)) jump:t 0001DA04; r2 = memw(r17+16); r1 = #0x96 }

l0001D9FC:
	{ jump 0001DA20; r3 = add(PC,#0x12) }

l0001DA04:
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001DA28; r2 = memw(r17+20); r0 = #0x0; r1 = #0x97 }

l0001DA18:
	{ r3 = add(PC,#0x1) }
	{ r2 = r16; call errlog_function }

l0001DA20:
	{ r2 = r16 }

l0001DA24:
	{ r0 = #0xFFFFFFFF }

l0001DA28:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001DA2C
;;   Called from:
;;     0001D8A8 (in slice_check_f)
;;     0001D908 (in slice_check_8)
;;     0001D9E4 (in slice_check_q8)
;;     0001DAB4 (in slice_impl)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001DA50; r5 = memw(r2+16) }

l0001DA3C:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x20) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

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
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xBA84); immext(#0xBA80) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; slice_impl: 0001DA78
;;   Called from:
;;     0001D890 (in slice_execute_f)
;;     0001D8F0 (in slice_execute_8)
;;     0001D9C4 (in slice_execute_q8)
slice_impl proc
	{ allocframe(#0x58); r4 = add(PC,#0xBAA9); immext(#0xBA80) }
	{ r5 = memw(r0+8); r3 = memw(r0+4) }
	{ memd(r29+80) = r17:r16; memd(r29+56) = r23:r22; r1 = #0x40; r17:r16 = combine(r1,r2) }
	{ r23 = memw(r5); r22 = memw(r3); r2 = r17 }
	{ memd(r29+64) = r21:r20; memd(r29+72) = r19:r18 }
	{ memd(r29+48) = r25:r24 }
	{ memd(r29+40) = r27:r26; r20 = memw(r3+8) }
	{ r18 = memw(r22+16); r24 = memw(r3+4) }
	{ memw(r29) = r0; r19 = memw(r23+16); call logmsg_function }
	{ r6 = memw(r20+4); r3 = memw(r20); r1 = #0x54 }
	{ r2 = memw(r24); r13 = memw(r22); p1 = cmp.eq(r6,#0xFFFFFFFF); p0 = cmp.eq(r3,#0xFFFFFFFF) }
	{ r14 = memw(r20+12); r8 = memw(r20+8) }
	{ r12 = memw(r24+4); r9 = memw(r22+4); if (!p0) r20 = add(r3,#0x0); if (p0) r20 = sub(r13,r2) }
	{ r4 = memw(r22+8); p2 = cmp.eq(r8,#0xFFFFFFFF); if (p1) r21 = sub(r9,r12); r3 = mpyi(r20,r16) }
	{ r6 = memw(r24+12); r5 = memw(r24+8); p0 = cmp.eq(r14,#0xFFFFFFFF); if (!p1) r21 = add(r6,#0x0) }
	{ r7 = memw(r22+12); if (p2) r26 = sub(r4,r5); r3 = mpyi(r3,r21) }
	{ if (!p0) r8 = add(r14,#0x0); if (p0) r8 = sub(r7,r6); if (!p2) r26 = add(r8,#0x0) }
	{ r3 = mpyi(r3,r26) }
	{ r14 = mpyi(r3,r8); r3 = add(PC,#0xBA0F); immext(#0xBA00) }
	{ if (cmp.gtu(r14,r15.new)) jump:t 0001DBEC; r15 = memw(r23+20) }

l0001DB40:
	{ p0 = cmp.gt(r20,#0x0); r1 = #0x55; r3 = add(PC,#0x9) }
	{ if (p0) r1 = #0x56; if (!p0) jump:nt 0001DBEC }

l0001DB54:
	{ if (p1.new) r1 = #0x57; p1 = cmp.gt(r21,#0x0); r3 = add(PC,#0xB9FC); immext(#0xB9C0) }
	{ memb(r29+6) = r0.new; if (!p1) jump:nt 0001DBEC; r0 = p1 }

l0001DB74:
	{ if (p1.new) r1 = #0x58; p1 = cmp.gt(r26,#0x0); r3 = add(PC,#0x2B) }
	{ memb(r29+8) = r0.new; if (!p1) jump:nt 0001DBEC; r0 = p1 }

l0001DB90:
	{ p1 = cmp.gt(r8,#0x0); r3 = add(PC,#0x1A) }
	{ if (p1) r1 = #0x59; if (!p1) jump:nt 0001DBEC }

l0001DBA0:
	{ r3 = add(PC,#0xB9D1); immext(#0xB9C0) }
	{ if (!cmp.gtu(r13,r15.new)) jump:t 0001DBEC; r15 = add(r20,r2) }

l0001DBB4:
	{ r1 = #0x5A; r3 = add(PC,#0x10) }
	{ if (!cmp.gtu(r9,r13.new)) jump:t 0001DBEC; r13 = add(r21,r12) }

l0001DBC8:
	{ r1 = #0x5B; r3 = add(PC,#0xB) }
	{ if (!cmp.gtu(r4,r13.new)) jump:t 0001DBEC; r13 = add(r26,r5) }

l0001DBDC:
	{ r1 = #0x5C; r3 = add(PC,#0x6) }
	{ if (cmp.gtu(r7,r13.new)) jump:t 0001DBFC; r13 = add(r8,r6) }

l0001DBEC:
	{ r2 = r17; call errlog_function }

l0001DBF0:
	{ r2 = r17 }

l0001DBF4:
	{ r0 = #0xFFFFFFFF; jump 0001DCCC }

l0001DBFC:
	{ memw(r23+4) = r21; memw(r23+24) = r14; r0 = #0x0 }
	{ memw(r23+8) = r26; memw(r23) = r20 }
	{ memw(r23+12) = r8; if (!p0) jump:nt 0001DCCC }

l0001DC18:
	{ r12 = r7; r3 = mpyi(r7,r4); r2 = add(r12,mpyi(r2,r9)) }
	{ r7 = #0x0; r23 = mpyi(r7,r16); r13 = mpyi(r8,r26) }
	{ r9 = mpyi(r3,r9); r4 = add(r5,mpyi(r4,r2)) }
	{ r3 = r16; r17 = mpyi(r8,r16); r25 = mpyi(r3,r16) }
	{ memb(r29+7) = r2.new; r2 = mpyi(r13,r16); r12 = add(r6,mpyi(r12,r4)) }
	{ r3 = add(r18,mpyi(r3,r12)) }
	{ memw(r29+12) = r1 }

l0001DC5C:
	{ memw(r29+16) = r3; r0 = memd(r29+24); r27 = #0x0; r24 = r3 }
	{ memw(r29+20) = r7; p0 = r0 }
	{ if (!p0) jump:nt 0001DCB4 }

l0001DC74:
	{ r0 = memd(r29+32); r18 = r19; r16 = r24; r22 = r26 }
	{ if (!p0.new) jump:nt 0001DCA8; p0 = r0 }

l0001DC88:
	{ r16 = add(r16,r23); r2 = r17; r1:r0 = combine(r16,r18); call fn00009560 }
	{ if (!cmp.eq(r22.new,#0x0)) jump:t 0001DC88; r22 = add(r22,#0xFFFFFFFF); r18 = add(r18,r17) }

l0001DCA4:
	{ r19 = add(r19,r2) }

l0001DCA8:
	{ if (!cmp.eq(r27.new,r21)) jump:t 0001DC74; r27 = add(r27,#0x1); r24 = add(r24,r25) }

l0001DCB4:
	{ r2 = memd(r29+12); r7 = memd(r29+20) }

l0001DCB8:
	{ r3 = memw(r29+16) }
	{ if (!cmp.eq(r7.new,r20)) jump:t 0001DC5C; r7 = add(r7,#0x1); r3 = add(r3,r2) }

l0001DCCC:
	{ r19:r18 = memd(r29+72); r17:r16 = memd(r29+80) }
	{ r23:r22 = memd(r29+56); r21:r20 = memd(r29+64) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+48) }
	{ dealloc_return }

;; split_execute_f: 0001DCE0
split_execute_f proc
	{ r3 = memw(r0+20); jump split_impl; r4 = #0x4 }

;; split_check_f: 0001DCE8
split_check_f proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xB8E0); immext(#0xB8C0) }
	{ r17 = r0; r16 = r1; r1 = #0x75 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x2)) jump:t 0001DD1C; r2 = memw(r17+16); r1 = #0x76 }

l0001DD14:
	{ jump 0001DD38; r3 = add(PC,#0xF) }

l0001DD1C:
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 0001DD40; r2 = memw(r17+20); r0 = #0x0; r1 = #0x77 }

l0001DD30:
	{ r3 = add(PC,#0x3E) }
	{ r2 = r16; call errlog_function }

l0001DD38:
	{ r2 = r16 }

l0001DD3C:
	{ r0 = #0xFFFFFFFF }

l0001DD40:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; qsplit_execute_8: 0001DD44
qsplit_execute_8 proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r16 = r0 }
	{ memd(r29) = r19:r18; r2 = memw(r16+4); r18 = r1 }
	{ r4 = memw(r16+8); r19 = memw(r16+20) }
	{ r3 = memw(r2+8); r17 = add(r19,#0xFFFFFFFE) }
	{ r2 = memw(r15+r17<<#2) }
	{ memb(r2) = r7.new; r7 = memw(r3) }
	{ memw(r2+8) = r5; memw(r2+4) = r4 }
	{ r7 = memw(r3+12) }
	{ memw(r2+12) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r5.new)) jump:t 0001DD94; r5 = memw(r2+20) }

l0001DD8C:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }

l0001DD94:
	{ r3 = memw(r16+4); r2 = memw(r16+8) }
	{ r3 = memw(r3+12); r2 = addasl(r2,r19,#0x2) }
	{ r2 = memw(r2-4) }
	{ r5 = memw(r3+4); r4 = memw(r3) }
	{ memw(r2) = r4; memw(r2+4) = r5 }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:t 0001DDCC; r6 = memw(r2+20) }

l0001DDC4:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }

l0001DDCC:
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8); r3:r2 = combine(r17,#0x1); r1:r0 = combine(r18,r16) }
	{ deallocframe; jump split_impl }

;; qsplit_check: 0001DDE0
qsplit_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xB7E8); immext(#0xB7C0) }
	{ r17 = r0; r16 = r1; r1 = #0x7D }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x4)) jump:t 0001DE14; r2 = memw(r17+16); r1 = #0x7E }

l0001DE0C:
	{ jump 0001DE30; r3 = add(PC,#0x17) }

l0001DE14:
	{ if (cmp.gtu(r2.new,#0x2)) jump:t 0001DE38; r2 = memw(r17+20); r0 = #0x0; r1 = #0x7F }

l0001DE28:
	{ r3 = add(PC,#0x6) }
	{ r2 = r16; call errlog_function }

l0001DE30:
	{ r2 = r16 }

l0001DE34:
	{ r0 = #0xFFFFFFFF }

l0001DE38:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001DE3C
;;   Called from:
;;     0001DCFC (in split_check_f)
;;     0001DDF4 (in qsplit_check)
;;     0001DEE4 (in split_impl)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001DE60; r5 = memw(r2+16) }

l0001DE4C:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x25) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001DE60:
	{ dealloc_return }

;; errlog_function: 0001DE64
;;   Called from:
;;     0001DD34 (in split_check_f)
;;     0001DE2C (in qsplit_check)
;;     0001DE54 (in logmsg_function)
;;     0001DFF4 (in split_impl)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xB749); immext(#0xB740) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; split_impl: 0001DE88
;;   Called from:
;;     0001DCE0 (in split_execute_f)
;;     0001DDD8 (in qsplit_execute_8)
split_impl proc
	{ allocframe(#0x40); memd(r29-40) = r23:r22; r22 = r0 }
	{ memd(r29+40) = r21:r20; r4 = memw(r22+4) }
	{ memd(r29+48) = r19:r18; memd(r29+56) = r17:r16; r17:r16 = combine(r1,r3) }
	{ memd(r29+24) = r25:r24; r21 = memw(r4+4); r18 = r2 }
	{ memd(r29+16) = r27:r26; r23 = memw(r22+8) }
	{ r26 = memw(r21+8); r20 = memw(r21+12) }
	{ r25 = memw(r21); r27 = memw(r21+4); r1:r0 = combine(r16,r20); call fn00009750 }
	{ r2 = memw(r21+24); r19 = r0 }
	{ r1:r0 = combine(r16,r2); call fn00009760 }
	{ r2 = r17; r1 = #0x46; r4 = add(PC,#0xB722); immext(#0xB700) }
	{ memw(r29) = r22; r24 = memw(r21+16); r21 = r0; call logmsg_function }
	{ r3:r2 = combine(r23,#0x0); if (p0.new) jump:nt 0001DF24; p0 = cmp.eq(r8,#0x1) }

l0001DEFC:
	{  }
	{ r4 = memw(r3) }
	{ if (!cmp.gtu(r21,r5.new)) jump:t 0001DF5C; r5 = memw(r4+20) }

l0001DF14:
	{ memw(r29) = r2; r1 = #0x4D; r3 = add(PC,#0x3C) }
	{ jump 0001DFF4 }

l0001DF24:
	{ r3 = memw(r22+4); r2 = memw(r22+8); r0 = #0xFFFFFFFF }
	{ r2 = memw(r2); r3 = memw(r3+4) }
	{ r5 = memw(r3+4); r4 = memw(r3) }
	{ memw(r2) = r4; memw(r2+4) = r5 }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:t 0001E000; r6 = memw(r2+20) }

l0001DF50:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }
	{ jump 0001E000; r0 = #0x0 }

l0001DF5C:
	{ memw(r4+24) = r21; r2 = r2 }
	{ r4 = memw(r3++#4) }
	{ memw(r4+8) = r26; memw(r4+4) = r27 }
	{ memw(r4) = r25; memw(r4+12) = r19 }
	{ memb(r29+2) = r2.new; r1:r0 = combine(r16,r20); call fn00009770; r2 = p1 }
	{ if (p0.new) r20 = #0x0; r1 = #0x57 }
	{ r1 = memd(r29+8); r0 = #0x0; r2 = mpyi(r27,r26); r17 = mpyi(r19,r18) }
	{ if (!p0.new) jump:nt 0001E000; p0 = r1 }

l0001DFA4:
	{ r21 = mpyi(r17,r16); r2 = mpyi(r2,r25) }
	{ r22 = mpyi(r2,r19) }

l0001DFB0:
	{ r19 = r20; r25 = r22; if (!p0.new) jump:nt 0001DFE0; p0 = cmp.gt(r14,#0x0) }

l0001DFB4:
	{ r19 = r20; r25 = r22 }

l0001DFBC:
	{ r2 = memw(r31+r20<<#2) }
	{ r18 = memw(r2+16); r19 = add(r24,mpyi(r19,r17)) }

l0001DFC8:
	{ r19 = add(r19,r21); r2 = r17; r1:r0 = combine(r19,r18); call fn00009560 }
	{ if (!cmp.eq(r25.new,#0x0)) jump:t 0001DFC8; r25 = add(r25,#0xFFFFFFFF); r18 = add(r18,r17) }

l0001DFE0:
	{ if (!cmp.eq(r20.new,r16)) jump:t 0001DFB0; r20 = add(r20,#0x1) }

l0001DFE4:
	{ if (!cmp.eq(r20.new,r16)) jump:t 0001DFB4 }

l0001DFEC:
	{ r3 = add(PC,#0xB631); immext(#0xB600) }

l0001DFF4:
	{ r2 = r17; call errlog_function }
	{ r0 = #0xFFFFFFFF }

l0001E000:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ r27:r26 = memd(r29+16); r25:r24 = memd(r29+24) }
	{ dealloc_return }
0001E014             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; tanh_execute: 0001E020
tanh_execute proc
	{ allocframe(#0x28); memd(r29-16) = r17:r16; r4 = add(PC,#0xB6A0); immext(#0xB680) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+16) = r21:r20; r2 = memw(r17+4); r1 = #0x37 }
	{ r3 = memw(r17+8) }
	{ memd(r29+24) = r19:r18; memd(r29+8) = r23:r22 }
	{ r22 = memw(r3); r21 = memw(r2) }
	{ r6 = memw(r21+12); r0 = memw(r21) }
	{ r5 = memw(r21+8); r7 = memw(r21+4) }
	{ r19 = memw(r22+16); r18 = memw(r21+16); r2 = mpyi(r7,r0) }
	{ memw(r29) = r17 }
	{ r2 = r16; r3 = mpyi(r2,r5) }
	{ r20 = mpyi(r3,r6) }
	{ call logmsg_function; r23 = asl(r20,#0x2) }
	{ if (!cmp.gtu(r23,r2.new)) jump:t 0001E08C; r2 = memw(r22+20) }

l0001E078:
	{ r2 = r16; r1 = #0x38; r3 = add(PC,#0x23) }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001E0E0 }

l0001E08C:
	{ r3 = memw(r21+4); r2 = memw(r21); p0 = cmp.eq(r20,#0x0) }
	{ memw(r22) = r2; memw(r22+4) = r3 }
	{ r6 = memw(r21+8) }
	{ memw(r22+8) = r6 }
	{ r7 = memw(r21+12) }
	{ memw(r22+24) = r23; memw(r22+12) = r7; if (p0) jump:nt 0001E0C4 }

l0001E0AC:
	{ r0 = memw(r18); r20 = r20; call fn000097F0 }
	{ memw(r19) = r0; r18 = add(r18,#0x4); p0 = cmp.eq(r20,#0x0); r19 = add(r19,#0x4) }
	{ if (!p0) jump:nt 0001E0AC }

l0001E0C4:
	{ r2 = r16; r1 = #0x40; r4 = add(PC,#0xB621); immext(#0xB600) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }

l0001E0E0:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; tanh_check: 0001E0EC
tanh_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xB589); immext(#0xB580) }
	{ r17 = r0; r16 = r1; r1 = #0x46 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001E12C; r2 = memw(r17+16); r1 = #0x47 }

l0001E118:
	{ r3 = add(PC,#0x37) }
	{ r2 = r16; call errlog_function }

l0001E120:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001E12C:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001E144; r2 = memw(r17+20); r1 = #0x48 }

l0001E13C:
	{ jump 0001E120; r3 = add(PC,#0x22) }

l0001E144:
	{ r2 = r16; r1 = #0x49; r4 = add(PC,#0xB566); immext(#0xB540) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001E164
;;   Called from:
;;     0001E064 (in tanh_execute)
;;     0001E0D4 (in tanh_execute)
;;     0001E100 (in tanh_check)
;;     0001E154 (in tanh_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001E188; r5 = memw(r2+16) }

l0001E174:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x29) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001E188:
	{ dealloc_return }

;; errlog_function: 0001E18C
;;   Called from:
;;     0001E080 (in tanh_execute)
;;     0001E11C (in tanh_check)
;;     0001E17C (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xB4CD); immext(#0xB4C0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; sigmoid_execute: 0001E1B0
sigmoid_execute proc
	{ allocframe(#0x28); memd(r29-16) = r17:r16; r4 = add(PC,#0xB5B2); immext(#0xB580) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+16) = r21:r20; r2 = memw(r17+4); r1 = #0x37 }
	{ r3 = memw(r17+8) }
	{ memd(r29+24) = r19:r18; memd(r29+8) = r23:r22 }
	{ r22 = memw(r3); r21 = memw(r2) }
	{ r6 = memw(r21+12); r0 = memw(r21) }
	{ r5 = memw(r21+8); r7 = memw(r21+4) }
	{ r19 = memw(r22+16); r18 = memw(r21+16); r2 = mpyi(r7,r0) }
	{ memw(r29) = r17 }
	{ r2 = r16; r3 = mpyi(r2,r5) }
	{ r20 = mpyi(r3,r6) }
	{ call logmsg_function; r23 = asl(r20,#0x2) }
	{ if (!cmp.gtu(r23,r2.new)) jump:t 0001E21C; r2 = memw(r22+20) }

l0001E208:
	{ r2 = r16; r1 = #0x38; r3 = add(PC,#0x38) }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001E290 }

l0001E21C:
	{ r3 = memw(r21+4); r2 = memw(r21); p0 = cmp.eq(r20,#0x0) }
	{ memw(r22) = r2; memw(r22+4) = r3 }
	{ r6 = memw(r21+8) }
	{ memw(r22+8) = r6 }
	{ r7 = memw(r21+12); if (!p0) r21 = #0x3F000000; immext(#0x3F000000) }
	{ memw(r22+24) = r23; memw(r22+12) = r7; if (p0) jump:nt 0001E274 }

l0001E244:
	{ r22 = #0x3F800000; immext(#0x3F800000) }

l0001E24C:
	{ r2 = memw(r18) }
	{ call fn000097F0; r0 = sfmpy(r2,r21) }
	{ r20 = r20; r18 = add(r18,#0x4); r2 = sfadd(r0,r22) }
	{ p0 = cmp.eq(r20,#0x0) }
	{ memb(r19) = r2.new; r19 = add(r19,#0x4); if (!p0) jump:nt 0001E24C; r2 = sfmpy(r2,r21) }

l0001E274:
	{ r2 = r16; r1 = #0x40; r4 = add(PC,#0xB516); immext(#0xB500) }

l0001E278:
	{ r2 = r16; r1 = #0x40; r4 = add(PC,#0x16) }

l0001E284:
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }

l0001E290:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; sigmoid_check: 0001E29C
sigmoid_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xB475); immext(#0xB440) }
	{ r17 = r0; r16 = r1; r1 = #0x46 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001E2DC; r2 = memw(r17+16); r1 = #0x47 }

l0001E2C8:
	{ r3 = add(PC,#0x26) }
	{ r2 = r16; call errlog_function }

l0001E2D0:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001E2DC:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001E2F4; r2 = memw(r17+20); r1 = #0x48 }

l0001E2EC:
	{ jump 0001E2D0; r3 = add(PC,#0x11) }

l0001E2F4:
	{ r2 = r16; r1 = #0x49; r4 = add(PC,#0xB455); immext(#0xB440) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001E314
;;   Called from:
;;     0001E1F4 (in sigmoid_execute)
;;     0001E284 (in sigmoid_execute)
;;     0001E2B0 (in sigmoid_check)
;;     0001E304 (in sigmoid_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001E338; r5 = memw(r2+16) }

l0001E324:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x12) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001E338:
	{ dealloc_return }

;; errlog_function: 0001E33C
;;   Called from:
;;     0001E210 (in sigmoid_execute)
;;     0001E2CC (in sigmoid_check)
;;     0001E32C (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xB3B6); immext(#0xB380) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; qtanh_execute_ref: 0001E360
qtanh_execute_ref proc
	{ allocframe(#0x40); memd(r29-16) = r17:r16; r17:r16 = combine(r1,r0) }
	{ memd(r29+16) = r27:r26; r2 = memw(r16+4) }
	{ r7 = memw(r16+8) }
	{ memd(r29+48) = r19:r18; memd(r29+32) = r23:r22; r18 = #0x437F0000; immext(#0x437F0000) }
	{ r4 = memw(r2+4); r26 = memw(r2); r1 = r18 }
	{ r27 = memw(r7); r2 = memw(r2+8) }
	{ r0 = memw(r26+4); r4 = memw(r4+16) }
	{ r6 = memw(r26); r2 = memw(r2+16) }
	{ r5 = memw(r26+12); r23 = memw(r4) }
	{ r6 = memw(r26+8); r2 = memw(r2); r3 = mpyi(r0,r6) }
	{ memd(r29+24) = r25:r24; memd(r29+40) = r21:r20 }
	{ r21 = memw(r7+4); r20 = memw(r7+8); r0 = sfsub(r2,r23); r2 = mpyi(r3,r6) }
	{ r24 = memw(r27+16); r22 = memw(r26+16) }
	{ call fn00009610; r25 = mpyi(r2,r5) }
	{ r2 = r17; r1 = #0x42; r4 = add(PC,#0xB53E); immext(#0xB500) }
	{ memw(r29) = r16; r19 = r0; call logmsg_function }
	{ if (!cmp.gtu(r25,r2.new)) jump:t 0001E41C; r2 = memw(r27+20); p0 = cmp.eq(r25,#0x0) }

l0001E404:
	{ r2 = r17; r1 = #0x43; r3 = add(PC,#0x3) }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001E4E8 }

l0001E41C:
	{ r3 = memw(r26+4); r2 = memw(r26) }
	{ memw(r27) = r2; memw(r27+4) = r3 }
	{ r6 = memw(r26+8) }
	{ memw(r27+8) = r6 }
	{ memw(r29+12) = r17; r7 = memw(r26+12); if (!p0) r17 = #0x3F000000; immext(#0x3F000000) }
	{ memw(r27+24) = r25; memw(r27+12) = r7; if (p0) jump:nt fn0001E49C }

l0001E450:
	{ r27 = #0x3F800000; immext(#0x3F800000); r26 = #0x42FF0000; immext(#0x42FF0000) }

l0001E460:
	{ r2 = memb(r22++#1); r0 = r23 }
	{ r2 = convert_w2sf(r2) }
	{ call fn000097F0; r0 += sfmpy(r19,r2) }
	{ r25 = add(r25,#0xFFFFFFFF); r2 = r17; r3 = sfadd(r0,r27) }
	{ p0 = cmp.eq(r25,#0x0); r2 += sfmpy(r3,r26) }
	{ r3 = convert_sf2uw(r2) }
	{ if (p1.new) r3 = #0xFFFFFFFF; p1 = sfcmp.gt(r2,r18) }
	{ memb(r24++#1) = r3; if (!p0) jump:nt 0001E460 }

l0001E498:
	{ memb(r24++#1) = r3 }

;; fn0001E49C: 0001E49C
;;   Called from:
;;     0001E444 (in qtanh_execute_ref)
;;     0001E494 (in qtanh_execute_ref)
;;     0001E498 (in qtanh_execute_hvx)
;;     0001E4A4 (in qtanh_execute_hvx)
;;     0001E4B4 (in qtanh_execute_hvx)
;;     0001E860 (in qtanh_execute_hvx)
;;     0001E868 (in qtanh_execute_hvx)
;;     0001E88C (in qtanh_execute_hvx)
fn0001E49C proc
	{ memw(r21+12) = #0x1; r2 = memw(r21+16); r4 = add(PC,#0xB4B5); immext(#0xB480) }

l0001E4A4:
	{ memw(r21+12) = #0x1; r2 = memw(r21+16) }

l0001E4A8:
	{ memw(r21) = #0x1; memw(r21+8) = #0x1; r1 = #0x56 }
	{ memw(r2-1082130432) = #0x0; immext(#0xBF800000); memw(r21+4) = #0xFFFFFF81 }

l0001E4B4:
	{ memw(r2-1082130432) = #0x0; immext(#0xBF800000) }

l0001E4BC:
	{ memw(r21+24) = #0x4; r2 = memw(r29+12) }
	{ memw(r20+8) = #0x1; r3 = memw(r20+16) }
	{ memw(r20+4) = #0x1; memw(r20) = #0x1 }
	{ memw(r3+1065353216) = #0x0; immext(#0x3F800000); memw(r20+12) = #0xFFFFFF81 }

l0001E4D8:
	{ memw(r29) = r16; memw(r20+24) = #0x4; call logmsg_function }
	{ r0 = #0x0 }

l0001E4E8:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ r27:r26 = memd(r29+16); r25:r24 = memd(r29+24) }
	{ dealloc_return }
0001E4FC                                     00 C0 00 7F             ....

;; qtanh_check: 0001E500
qtanh_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xB45E); immext(#0xB440) }
	{ r17 = r0; r16 = r1; r1 = #0x94 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001E540; r2 = memw(r17+16); r1 = #0x95 }

l0001E52C:
	{ r3 = add(PC,#0xC) }
	{ r2 = r16; call errlog_function }

l0001E534:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001E540:
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001E558; r2 = memw(r17+20); r1 = #0x96 }

l0001E550:
	{ jump 0001E534; r3 = add(PC,#0x37) }

l0001E558:
	{ r2 = r16; r1 = #0x97; r4 = add(PC,#0xB43B); immext(#0xB400) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }
0001E578                         00 40 00 7F 00 C0 00 7F         .@......

;; qtanh_execute_hvx: 0001E580
qtanh_execute_hvx proc
	{ allocframe(#0x50); memd(r29-16) = r17:r16; r4 = add(PC,#0xB39A); immext(#0xB380) }
	{ r17:r16 = combine(r1,r0) }
	{ memd(r29+64) = r19:r18; r3 = memw(r16+4); r1 = #0x72 }
	{ r5 = memw(r16+8); r2 = r17 }
	{ memd(r29+40) = r25:r24; memd(r29+56) = r21:r20 }
	{ r7 = memw(r3+4); r6 = memw(r3+8) }
	{ r18 = memw(r5); r0 = memw(r5+4) }
	{ r21 = memw(r3); r5 = memw(r5+8) }
	{ r3 = memw(r7+16) }
	{ memw(r29+28) = r5; r5 = memw(r6+16) }
	{ memd(r29+48) = r23:r22 }
	{ memd(r29+32) = r27:r26; r7 = memw(r18+16) }
	{ r25 = memw(r17+4) }
	{ memw(r29+20) = r0; r23 = memw(r21+16) }
	{ r22 = memw(r21+4); r20 = memw(r21+8) }
	{ r27 = memw(r21); r26 = memw(r21+12) }
	{ memw(r29+24) = r7; r19 = memw(r3) }
	{ r24 = memw(r5) }
	{ memw(r29) = r16; call logmsg_function }
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 0001E60C; r2 = memb(r16+32); r1 = #0x73 }

l0001E5F4:
	{ r3 = add(PC,#0xB33D); immext(#0xB300) }

l0001E5F8:
	{ r3 = add(PC,#0x3D) }

l0001E5FC:
	{ r2 = r17; call errlog_function }

l0001E600:
	{ r2 = r17 }
	{ r0 = #0xFFFFFFFF; jump 0001E964 }

l0001E60C:
	{ r3 = memw(r18+20); r1 = #0x74; r2 = mpyi(r22,r27) }
	{ r2 = mpyi(r2,r20) }
	{ if (!cmp.gtu(r20.new,r3)) jump:t 0001E630; r20 = mpyi(r2,r26) }

l0001E628:
	{ jump 0001E600; r3 = add(PC,#0x1F) }

l0001E630:
	{ memw(r29+12) = r2; r3 = memw(r21); r2 = #0xFF; r22 = add(r20,#0xFF) }
	{ memb(r18+1) = r4.new; r4 = memw(r21+4); r0 = r25; r27 = and(r22,#0xFFFFFF00) }
	{ memw(r18) = r3 }
	{ r2 = memw(r21+8); r1 = #0x0; r3 = add(r22,r0) }
	{ memw(r29+16) = r0; memw(r18+8) = r2; r2 = r27 }
	{ r7 = memw(r21+12) }
	{ memw(r18+24) = r20; memw(r18+12) = r7; r21 = and(r3,#0xFFFFFF00); call fn000095F0 }
	{ r2 = r27; r1:r0 = combine(#0x0,r21); call fn000095F0 }
	{ r1:r0 = combine(r23,r21); r2 = r20; call fn00009560 }
	{ r2 = #0xC0800000; immext(#0xC0800000); r1 = sfsub(r24,r19) }
	{ if (!p0.new) jump:nt 0001E76C; p0 = sfcmp.ge(r19,r2) }

l0001E6A0:
	{ r18 = #0x40800000; immext(#0x40800000) }
	{ if (!p0.new) jump:nt 0001E76C; p0 = sfcmp.ge(r18,r24) }

l0001E6B0:
	{ r2 = #0x437F0000; immext(#0x437F0000) }
	{ r1:r0 = combine(r2,r1); call fn00009610 }
	{ r23 = r0; r2 = sfadd(r19,r18) }
	{ r1:r0 = combine(r23,r2); call fn00009610 }
	{ r2 = #0x41700000; immext(#0x41700000); r18 = convert_uw2sf(r0):chop }
	{ r0 = r2; call fn000097C0 }
	{ r3 = #0xBF800000; immext(#0xBF800000); r2 = #0x41FF0000; immext(#0x41FF0000) }
	{ r25 = memw(r29+20); p0 = cmp.gt(r20,#0x0); r3 = sfadd(r0,r3); r2 = sfmpy(r23,r2) }
	{ r3 = convert_uw2sf(r3):chop; r2 = sfmpy(r2,r0) }
	{ if (p0) r2 = memw(r29+12); if (!p0) jump:nt 0001E828; r4 = convert_uw2sf(r2):chop }

l0001E718:
	{ r5 = convert_uw2sf(r0):chop }
	{ if (!p0.new) r3 = add(r4,#0x0); p0 = cmp.gt(r4,r3); r5 += lsr(r5,#0x1F) }
	{ r3 = sxth(r3); r2 = r21; r6 = mpyi(r2,r26) }
	{ r4 = asr(r5,#0x1); loop0(0001E738,r6) }
	{ r6 = memb(r2); r7 = r4; r5 = #0xFF }
	{ r6 = add(r6,r18) }
	{ r7 += mpyi(r6,r3) }
	{ r6 = asr(r7,#0xF) }
	{ if (!p0.new) r5 = add(r6,#0x0); p0 = cmp.gt(r6,#0xFF); if (p0.new) jump:nt 0001E760 }

l0001E758:
	{ if (!p0.new) r5 = #0x0; p0 = cmp.gt(r6,#0xFFFFFFFF) }

l0001E760:
	{ memb(r2++#1) = r5; nop }
	{ jump 0001E828 }

l0001E76C:
	{ r0 = #0x38D1B717; immext(#0x38D1B700); call fn00009600 }
	{ r1 = #0x437F0000; immext(#0x437F0000); call fn00009610 }
	{ r25 = memw(r29+20); r3 = add(r21,r22); if (!p0.new) jump:nt 0001E828; p0 = cmp.gt(r12,#0x0) }

l0001E790:
	{ r22 = and(r3,#0xFFFFFF00); loop0(0001E79C,r20) }
	{ r3:r2 = combine(r22,r21) }
	{ r4 = memb(r2++#1); r5 = r19 }
	{ r4 = convert_w2sf(r4) }
	{ r3 = add(r3,#0x4); r5 += sfmpy(r0,r4) }
	{ r1 = #0x41000000; immext(#0x41000000); r0 = #0x17 }
	{ call fn00009600 }
	{ r2 = #0x437F0000; immext(#0x437F0000) }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = memd(r29+12); r23 = r0; r18 = r21 }
	{ r24 = #0x40800000; immext(#0x40800000); r19 = mpyi(r2,r26) }

l0001E7EC:
	{ r2 = memw(r22) }
	{ r2 = sfadd(r2,r24) }
	{ call fn00009620; r0 = sfmpy(r23,r2) }
	{ r2 = #0xFF; r3 = convert_uw2sf(r0):chop }
	{ if (!p0.new) r2 = add(r3,#0x0); p0 = cmp.gt(r3,#0xFF); if (p0.new) jump:nt 0001E818 }

l0001E810:
	{ if (!p0.new) r2 = #0x0; p0 = cmp.gt(r3,#0xFFFFFFFF) }

l0001E818:
	{ memb(r18++#1) = r2; r22 = add(r22,#0x4); r19 = add(r19,#0xFFFFFFFF) }
	{ if (!p0.new) jump:nt 0001E7EC; p0 = cmp.eq(r11,#0x0) }

l0001E828:
	{ r3 = #0x1010101; immext(#0x1010100); r2 = add(PC,#0xC8E8); immext(#0xC8C0) }
	{ r5 = #0x80808080; immext(#0x80808080); r4 = #0x4040404; immext(#0x4040400) }
	{ r7 = #0x7070707; immext(#0x7070700); r6 = #0xF8F8F8F8; immext(#0xF8F8F8C0) }
	{ if (p0.new) jump:nt 0001E498; p0 = cmp.eq(r3,#-0x1) }

l0001E85C:
	{ if (p0.new) jump:nt fn0001E49C; p0 = cmp.eq(r5,#-0x1) }

l0001E860:
	{ if (p0.new) jump:nt 0001E4A4; p0 = cmp.eq(r6,#-0x1) }

l0001E864:
	{ p0 = cmp.gt(r27,#0x0); if (p0.new) jump:nt 0001E4A8; p0 = cmp.eq(r7,#-0x1) }

l0001E86C:
	{ if (p0.new) jump:nt 0001E4B4; p0 = cmp.eq(r4,#-0x1) }

l0001E870:
	{ r4 = memw(r2-48); immext(#0xFFFFFFC0); if (!p0) jump:nt 0001E900 }

l0001E87C:
	{ r0 = r0; r4 = #0x0; r6 = #0x7; r2 = add(r27,#0x7F); r7 = #0x0 }
	{ if (p0.new) jump:nt 0001E4D8; p0 = cmp.eq(r7,#-0x1); r5 = lsr(r2,#0x7) }

l0001E894:
	{ r1 = add(r1,#0x1); r4 = #0x0; r2 = memw(r29+16); r3 = #0x3; loop0(0001E8A8,r5) }
	{ if (p0.new) jump:nt fn0001EA34; p0 = cmp.eq(r0,r7) }

l0001E8A8:
	{ r1 = add(r1,#0x1); r5 = #0x11; if (p1.new) jump:nt 0001E600; p1 = cmp.eq(r3,r0); if (p1.new) jump:nt 0001E5F4; p1 = cmp.eq(r2,r0) }

l0001E8B4:
	{ if (p0.new) jump:nt 0001E958; p0 = tstbit(r3,#0x0) }
	{ if (p0.new) jump:nt 0001EA4C; p0 = cmp.eq(r4,r12) }
	{ if (!p1.new) jump:nt 0001ECA4; p1 = cmp.gtu(r7,#0xB); if (p1.new) jump:t 0001E718; p1 = cmp.gtu(r7,#0x12) }
	{ if (p1.new) jump:t 0001E510; p1 = cmp.gtu(r15,#0x12) }
	{ if (p1.new) jump:nt 0001EA64; p1 = cmp.eq(r14,r4) }
	{ if (!p1.new) jump:nt 0001E8AC; p1 = cmp.gtu(r6,#0xF); if (p0.new) jump:nt 0001EA64; p0 = cmp.eq(r0,r7) }
	{ if (p1.new) jump:nt fn0001EA74; p1 = cmp.eq(r0,r1) }
	{ if (p0.new) jump:nt 0001EB3C; p0 = cmp.gt(r0,r12) }
	{ jump 0001EB04; r9 = r3; jump 0001EB04; r8 = r2 }
	{ jump 0001E730; r9 = #0x29; jump 0001E6F8; r8 = #0x28 }
	{ if (!p1.new) jump:nt 0001ECC0; p1 = cmp.gtu(r15,#0x7) }
	{ if (p0.new) jump:nt 0001EA98; p0 = cmp.eq(r0,r10) }
	{ if (p1.new) jump:nt 0001E6DC; p1 = cmp.gt(r7,#-0x1) }
	{ r1 = add(r1,#0x1); r2 = #0x12; jump 0001E6A4; r5 = r1 }

l0001E900:
	{ r0 = memd(r29+24); r1 = memd(r29+16); r2 = r20; call fn00009560 }
	{ r5 = memw(r29+28); r2 = memw(r25+16); r4 = add(PC,#0xB045); immext(#0xB040) }
	{ memw(r25+8) = #0x1; memw(r25+12) = #0xFFFFFF81; r1 = #0x8E }
	{ memw(r25+4) = #0xFFFFFF81; memw(r25) = #0x1 }
	{ memw(r25+24) = #0x4; memw(r2-1082130432) = #0x0; immext(#0xBF800000); r2 = r17 }
	{ memw(r5+8) = #0x1; r3 = memw(r5+16) }
	{ memw(r5+4) = #0x1; memw(r5) = #0x1 }
	{ memw(r3+1065353216) = #0x0; immext(#0x3F800000); memw(r5+12) = #0xFFFFFF81 }
	{ memw(r29) = r16; memw(r5+24) = #0x4; call logmsg_function }
	{ r0 = #0x0 }

l0001E964:
	{ r19:r18 = memd(r29+64); r17:r16 = memd(r29+72) }
	{ r23:r22 = memd(r29+48); r21:r20 = memd(r29+56) }
	{ r27:r26 = memd(r29+32); r25:r24 = memd(r29+40) }
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
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001E99C; r5 = memw(r2+16) }

l0001E988:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x3C) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001E99C:
	{ dealloc_return }

;; errlog_function: 0001E9A0
;;   Called from:
;;     0001E410 (in qtanh_execute_ref)
;;     0001E530 (in qtanh_check)
;;     0001E5FC (in qtanh_execute_hvx)
;;     0001E990 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xAF60); immext(#0xAF40) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001E9C4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; qsigmoid_execute_ref: 0001E9D0
qsigmoid_execute_ref proc
	{ allocframe(#0x40); memd(r29-16) = r17:r16; r17:r16 = combine(r1,r0) }
	{ memd(r29+16) = r27:r26; r2 = memw(r16+4) }
	{ r7 = memw(r16+8) }
	{ memd(r29+48) = r19:r18; memd(r29+32) = r23:r22; r18 = #0x437F0000; immext(#0x437F0000) }
	{ r4 = memw(r2+4); r26 = memw(r2); r1 = r18 }
	{ r27 = memw(r7); r2 = memw(r2+8) }
	{ r0 = memw(r26+4); r4 = memw(r4+16) }
	{ r6 = memw(r26); r2 = memw(r2+16) }
	{ r5 = memw(r26+12); r23 = memw(r4) }
	{ r6 = memw(r26+8); r2 = memw(r2); r3 = mpyi(r0,r6) }
	{ memd(r29+24) = r25:r24; memd(r29+40) = r21:r20 }
	{ r21 = memw(r7+4); r20 = memw(r7+8); r0 = sfsub(r2,r23); r2 = mpyi(r3,r6) }

;; fn0001EA34: 0001EA34
;;   Called from:
;;     0001E8A4 (in qtanh_execute_hvx)
;;     0001EA30 (in qsigmoid_execute_ref)
fn0001EA34 proc
	{ r21 = memw(r7+4); r20 = memw(r7+8); r0 = sfsub(r2,r23) }
	{ r24 = memw(r27+16); r22 = memw(r26+16) }
	{ call fn00009610; r25 = mpyi(r2,r5) }
	{ r2 = r17; r1 = #0x42; r4 = add(PC,#0xB0D1); immext(#0xB0C0) }
	{ memw(r29) = r16; r19 = r0; call logmsg_function }
	{ if (!cmp.gtu(r25,r2.new)) jump:t fn0001EA8C; r2 = memw(r27+20); p0 = cmp.eq(r25,#0x0) }

;; fn0001EA74: 0001EA74
;;   Called from:
;;     0001EA5C (in fn0001EA34)
;;     0001EA64 (in fn0001EA34)
fn0001EA74 proc
	{ r2 = r17; r1 = #0x43; r3 = add(PC,#0x19) }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001EB54 }

;; fn0001EA8C: 0001EA8C
;;   Called from:
;;     0001EA5C (in fn0001EA34)
;;     0001EA64 (in fn0001EA34)
fn0001EA8C proc
	{ r3 = memw(r26+4); r2 = memw(r26) }
	{ memw(r27) = r2; memw(r27+4) = r3 }
	{ r6 = memw(r26+8) }
	{ memw(r27+8) = r6 }
	{ memw(r29+12) = r17; r7 = memw(r26+12); if (!p0) r17 = #0x42FF0000; immext(#0x42FF0000) }
	{ memw(r27+24) = r25; memw(r27+12) = r7; if (p0) jump:nt fn0001EB10 }

l0001EAC0:
	{ r27 = #0x3F000000; immext(#0x3F000000); r26 = #0x3F800000; immext(#0x3F800000) }

;; fn0001EAD0: 0001EAD0
;;   Called from:
;;     0001EAC0 (in fn0001EA8C)
;;     0001EEC8 (in qsigmoid_execute_hvx)
fn0001EAD0 proc
	{ r2 = memb(r22++#1); r3 = r23 }
	{ r2 = convert_w2sf(r2) }
	{ r3 += sfmpy(r19,r2) }
	{ call fn000097F0; r0 = sfmpy(r3,r27) }
	{ r25 = add(r25,#0xFFFFFFFF); r2 = r27; r3 = sfadd(r0,r26) }
	{ p0 = cmp.eq(r25,#0x0); r2 += sfmpy(r3,r17) }
	{ r3 = convert_sf2uw(r2) }
	{ if (p1.new) r3 = #0xFFFFFFFF; p1 = sfcmp.gt(r2,r18) }

l0001EB08:
	{ memb(r24++#1) = r3; if (!p0) jump:nt fn0001EAD0 }

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
;;     0001EEE4 (in qsigmoid_execute_hvx)
fn0001EB10 proc
	{ memw(r21+12) = #0x1; r2 = memw(r21+16); r4 = add(PC,#0xB047); immext(#0xB040) }

l0001EB14:
	{ memw(r21+12) = #0x1; r2 = memw(r21+16); r4 = add(PC,#0x7) }

l0001EB18:
	{ memw(r21+12) = #0x1; r2 = memw(r21+16) }

l0001EB1C:
	{ memw(r21) = #0x1; memw(r21+8) = #0x1; r1 = #0x57 }

l0001EB24:
	{ memw(r2) = #0x0; memw(r21+4) = #0x1 }
	{ memw(r21+24) = #0x4; r2 = memw(r29+12) }
	{ memw(r20+8) = #0x1; r3 = memw(r20+16) }
	{ memw(r20+4) = #0x1; memw(r20) = #0x1 }
	{ memw(r3+1065353216) = #0x0; immext(#0x3F800000); memw(r20+12) = #0xFFFFFF81 }
	{ memw(r29) = r16; memw(r20+24) = #0x4; call logmsg_function }

l0001EB48:
	{ memw(r29) = r16; memw(r20+24) = #0x4 }

l0001EB50:
	{ r0 = #0x0 }

l0001EB54:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ r27:r26 = memd(r29+16); r25:r24 = memd(r29+24) }
	{ dealloc_return }
0001EB68                         00 40 00 7F 00 C0 00 7F         .@......

;; qsigmoid_check: 0001EB70
qsigmoid_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xAFF7); immext(#0xAFC0) }
	{ r17 = r0; r16 = r1; r1 = #0x95 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001EBB0; r2 = memw(r17+16); r1 = #0x96 }

l0001EB9C:
	{ r3 = add(PC,#0x28) }
	{ r2 = r16; call errlog_function }

l0001EBA4:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001EBB0:
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0001EBC8; r2 = memw(r17+20); r1 = #0x97 }

l0001EBC0:
	{ jump 0001EBA4; r3 = add(PC,#0x13) }

l0001EBC8:
	{ r2 = r16; r1 = #0x98; r4 = add(PC,#0xAFD7); immext(#0xAFC0) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }
0001EBE8                         00 40 00 7F 00 C0 00 7F         .@......

;; qsigmoid_execute_hvx: 0001EBF0
qsigmoid_execute_hvx proc
	{ allocframe(#0x50); memd(r29-16) = r17:r16; r4 = add(PC,#0xAF2D); immext(#0xAF00) }
	{ r17:r16 = combine(r1,r0) }
	{ memd(r29+64) = r19:r18; r3 = memw(r16+4); r1 = #0x73 }
	{ r5 = memw(r16+8); r2 = r17 }
	{ memd(r29+40) = r25:r24; memd(r29+56) = r21:r20 }
	{ r7 = memw(r3+4); r6 = memw(r3+8) }
	{ r18 = memw(r5); r0 = memw(r5+4) }
	{ r21 = memw(r3); r5 = memw(r5+8) }
	{ r3 = memw(r7+16) }
	{ memw(r29+28) = r5; r5 = memw(r6+16) }
	{ memd(r29+48) = r23:r22 }
	{ memd(r29+32) = r27:r26; r7 = memw(r18+16) }
	{ r25 = memw(r17+4) }
	{ memw(r29+20) = r0; r23 = memw(r21+16) }
	{ r22 = memw(r21+4); r20 = memw(r21+8) }
	{ r27 = memw(r21); r26 = memw(r21+12) }
	{ memw(r29+24) = r7; r19 = memw(r3) }
	{ r24 = memw(r5) }
	{ memw(r29) = r16; call logmsg_function }
	{ if (!cmp.eq(r2.new,#0x0)) jump:t 0001EC7C; r2 = memb(r16+32); r1 = #0x74 }

l0001EC64:
	{ r3 = add(PC,#0xAED3); immext(#0xAEC0) }

l0001EC68:
	{ r3 = add(PC,#0x13) }

l0001EC6C:
	{ r2 = r17; call errlog_function }

l0001EC70:
	{ r2 = r17 }
	{ r0 = #0xFFFFFFFF; jump 0001EFD4 }

l0001EC7C:
	{ r3 = memw(r18+20); r1 = #0x75; r2 = mpyi(r22,r27) }
	{ r2 = mpyi(r2,r20) }
	{ if (!cmp.gtu(r20.new,r3)) jump:t 0001ECA0; r20 = mpyi(r2,r26) }

l0001EC98:
	{ jump 0001EC70; r3 = add(PC,#0x35) }

l0001ECA0:
	{ memw(r29+12) = r2; r3 = memw(r21); r2 = #0xFF; r22 = add(r20,#0xFF) }
	{ memb(r18+1) = r4.new; r4 = memw(r21+4); r0 = r25; r27 = and(r22,#0xFFFFFF00) }
	{ memw(r18) = r3 }
	{ r2 = memw(r21+8); r1 = #0x0; r3 = add(r22,r0) }
	{ memw(r29+16) = r0; memw(r18+8) = r2; r2 = r27 }
	{ r7 = memw(r21+12) }
	{ memw(r18+24) = r20; memw(r18+12) = r7; r21 = and(r3,#0xFFFFFF00); call fn000095F0 }
	{ r2 = r27; r1:r0 = combine(#0x0,r21); call fn000095F0 }
	{ r1:r0 = combine(r23,r21); r2 = r20; call fn00009560 }
	{ r2 = #0xC0800000; immext(#0xC0800000); r1 = sfsub(r24,r19) }
	{ if (!p0.new) jump:nt 0001EDDC; p0 = sfcmp.ge(r19,r2) }

l0001ED10:
	{ r18 = #0x40800000; immext(#0x40800000) }
	{ if (!p0.new) jump:nt 0001EDDC; p0 = sfcmp.ge(r18,r24) }

l0001ED20:
	{ r2 = #0x437F0000; immext(#0x437F0000) }
	{ r1:r0 = combine(r2,r1); call fn00009610 }
	{ r23 = r0; r2 = sfadd(r19,r18) }
	{ r1:r0 = combine(r23,r2); call fn00009610 }
	{ r2 = #0x41700000; immext(#0x41700000); r18 = convert_uw2sf(r0):chop }
	{ r0 = r2; call fn000097C0 }
	{ r3 = #0xBF800000; immext(#0xBF800000); r2 = #0x41FF0000; immext(#0x41FF0000) }
	{ r25 = memw(r29+20); p0 = cmp.gt(r20,#0x0); r3 = sfadd(r0,r3); r2 = sfmpy(r23,r2) }
	{ r3 = convert_uw2sf(r3):chop; r2 = sfmpy(r2,r0) }
	{ if (p0) r2 = memw(r29+12); if (!p0) jump:nt 0001EE98; r4 = convert_uw2sf(r2):chop }

l0001ED88:
	{ r5 = convert_uw2sf(r0):chop }
	{ if (!p0.new) r3 = add(r4,#0x0); p0 = cmp.gt(r4,r3); r5 += lsr(r5,#0x1F) }
	{ r3 = sxth(r3); r2 = r21; r6 = mpyi(r2,r26) }
	{ r4 = asr(r5,#0x1); loop0(0001EDA8,r6) }
	{ r6 = memb(r2); r7 = r4; r5 = #0xFF }
	{ r6 = add(r6,r18) }
	{ r7 += mpyi(r6,r3) }
	{ r6 = asr(r7,#0xF) }
	{ if (!p0.new) r5 = add(r6,#0x0); p0 = cmp.gt(r6,#0xFF); if (p0.new) jump:nt 0001EDD0 }

l0001EDC8:
	{ if (!p0.new) r5 = #0x0; p0 = cmp.gt(r6,#0xFFFFFFFF) }

l0001EDD0:
	{ memb(r2++#1) = r5; nop }
	{ jump 0001EE98 }

l0001EDDC:
	{ r0 = #0x38D1B717; immext(#0x38D1B700); call fn00009600 }
	{ r1 = #0x437F0000; immext(#0x437F0000); call fn00009610 }
	{ r25 = memw(r29+20); r3 = add(r21,r22); if (!p0.new) jump:nt 0001EE98; p0 = cmp.gt(r12,#0x0) }

l0001EE00:
	{ r22 = and(r3,#0xFFFFFF00); loop0(0001EE0C,r20) }
	{ r3:r2 = combine(r22,r21) }
	{ r4 = memb(r2++#1); r5 = r19 }
	{ r4 = convert_w2sf(r4) }
	{ r3 = add(r3,#0x4); r5 += sfmpy(r0,r4) }
	{ r1 = #0x41000000; immext(#0x41000000); r0 = #0x17 }
	{ call fn00009600 }
	{ r2 = #0x437F0000; immext(#0x437F0000) }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = memd(r29+12); r23 = r0; r18 = r21 }
	{ r24 = #0x40800000; immext(#0x40800000); r19 = mpyi(r2,r26) }

l0001EE5C:
	{ r2 = memw(r22) }
	{ r2 = sfadd(r2,r24) }
	{ call fn00009620; r0 = sfmpy(r23,r2) }
	{ r2 = #0xFF; r3 = convert_uw2sf(r0):chop }
	{ if (!p0.new) r2 = add(r3,#0x0); p0 = cmp.gt(r3,#0xFF); if (p0.new) jump:nt 0001EE88 }

l0001EE80:
	{ if (!p0.new) r2 = #0x0; p0 = cmp.gt(r3,#0xFFFFFFFF) }

l0001EE88:
	{ memb(r18++#1) = r2; r22 = add(r22,#0x4); r19 = add(r19,#0xFFFFFFFF) }
	{ if (!p0.new) jump:nt 0001EE5C; p0 = cmp.eq(r11,#0x0) }

l0001EE98:
	{ r3 = #0x1010101; immext(#0x1010100); r2 = add(PC,#0xC278); immext(#0xC240) }
	{ r5 = #0x2020202; immext(#0x2020200); r4 = #0x10101010; immext(#0x10101000) }
	{ r7 = #0x7070707; immext(#0x7070700); r6 = #0xF8F8F8F8; immext(#0xF8F8F8C0) }
	{ if (p0.new) jump:nt 0001EB08; p0 = cmp.eq(r3,#-0x1) }

l0001EECC:
	{ if (p0.new) jump:nt 0001EB0C; p0 = cmp.eq(r5,#-0x1) }

l0001EED0:
	{ if (p0.new) jump:nt 0001EB14; p0 = cmp.eq(r6,#-0x1) }

l0001EED4:
	{ p0 = cmp.gt(r27,#0x0); if (p0.new) jump:nt 0001EB18; p0 = cmp.eq(r7,#-0x1) }

l0001EEDC:
	{ if (p0.new) jump:nt 0001EB24; p0 = cmp.eq(r4,#-0x1) }

l0001EEE0:
	{ r4 = memw(r2-32); immext(#0xFFFFFFC0); if (!p0) jump:nt 0001EF74 }

l0001EEEC:
	{ r0 = r0; r4 = #0x0; r6 = #0x7; r2 = add(r27,#0x7F); r7 = #0x0 }
	{ if (p0.new) jump:nt 0001EB48; p0 = cmp.eq(r7,#-0x1); r5 = lsr(r2,#0x7) }

l0001EF04:
	{ r1 = add(r1,#0x1); r4 = #0x0; r2 = memw(r29+16); r3 = #0x3; loop0(0001EF18,r5) }
	{ if (p0.new) jump:nt fn0001F0A4; p0 = cmp.eq(r0,r7) }

l0001EF18:
	{ r1 = add(r1,#0x1); r5 = #0x11; if (p1.new) jump:nt 0001EC70; p1 = cmp.eq(r3,r0); if (p1.new) jump:nt 0001EC64; p1 = cmp.eq(r2,r0) }

l0001EF24:
	{ if (p0.new) jump:nt 0001EFC8; p0 = tstbit(r3,#0x0) }
	{ if (p0.new) jump:nt 0001F0BC; p0 = cmp.eq(r4,r12) }
	{ if (!p1.new) jump:nt 0001F314; p1 = cmp.gtu(r7,#0xB); if (p1.new) jump:t 0001ED88; p1 = cmp.gtu(r7,#0x12) }
	{ if (p1.new) jump:t 0001EB80; p1 = cmp.gtu(r15,#0x12) }
	{ if (p1.new) jump:nt 0001F0D4; p1 = cmp.eq(r14,r4) }
	{ if (!p1.new) jump:nt 0001EF1C; p1 = cmp.gtu(r6,#0xF); if (p0.new) jump:nt 0001F0D4; p0 = cmp.eq(r0,r7) }
	{ if (p1.new) jump:nt 0001F0E4; p1 = cmp.eq(r0,r1) }
	{ if (p0.new) jump:nt 0001F1AC; p0 = cmp.gt(r0,r12) }
	{ jump 0001F174; r9 = r3; jump 0001F174; r8 = r2 }
	{ jump 0001EDA0; r9 = #0x29; jump 0001ED68; r8 = #0x28 }
	{ if (!p1.new) jump:nt 0001F330; p1 = cmp.gtu(r15,#0x7) }
	{ if (p0.new) jump:nt 0001F108; p0 = cmp.eq(r0,r10) }
	{ if (p1.new) jump:nt 0001ED4C; p1 = cmp.gt(r7,#-0x1) }
	{ if (p1.new) jump:nt 0001F114; p1 = cmp.eq(r1,r5) }
	{ r1 = add(r1,#0x1); r2 = #0x12; if (p1.new) jump:nt 0001EF58; p1 = tstbit(r7,#0x0) }

l0001EF74:
	{ r0 = memd(r29+24); r1 = memd(r29+16); r2 = r20; call fn00009560 }
	{ r5 = memw(r29+28); r2 = memw(r25+16); r4 = add(PC,#0xABD7); immext(#0xABC0) }
	{ memw(r25+8) = #0x1; memw(r25+12) = #0xFFFFFF81; r1 = #0x8F }
	{ memw(r25+4) = #0xFFFFFF81; memw(r25) = #0x1 }
	{ memw(r25+24) = #0x4; memw(r2) = #0x0; r2 = r17 }
	{ memw(r5+8) = #0x1; r3 = memw(r5+16) }
	{ memw(r5+4) = #0x1; memw(r5) = #0x1 }
	{ memw(r3+1065353216) = #0x0; immext(#0x3F800000); memw(r5+12) = #0xFFFFFF81 }
	{ memw(r29) = r16; memw(r5+24) = #0x4; call logmsg_function }
	{ r0 = #0x0 }

l0001EFD4:
	{ r19:r18 = memd(r29+64); r17:r16 = memd(r29+72) }
	{ r23:r22 = memd(r29+48); r21:r20 = memd(r29+56) }
	{ r27:r26 = memd(r29+32); r25:r24 = memd(r29+40) }
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
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001F00C; r5 = memw(r2+16) }

l0001EFF8:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0xC) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001F00C:
	{ dealloc_return }

;; errlog_function: 0001F010
;;   Called from:
;;     0001EA80 (in fn0001EA74)
;;     0001EBA0 (in qsigmoid_check)
;;     0001EC6C (in qsigmoid_execute_hvx)
;;     0001F000 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xAAF0); immext(#0xAAC0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001F034             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; add_f_execute: 0001F040
add_f_execute proc
	{ allocframe(#0xA0); r5 = r0 }
	{ r3 = memw(r5+8); r2 = memw(r5+4) }
	{ memd(r29+144) = r19:r18; memd(r29+112) = r27:r26 }
	{ r19 = memw(r2+4); r26 = memw(r2) }
	{ memd(r29+128) = r23:r22; memd(r29+136) = r21:r20; r22 = #0x0 }
	{ r0 = memw(r26); r6 = memw(r26+4) }
	{ r4 = memw(r19+4); r8 = memw(r19); p1 = cmp.eq(r0,#0x1); p0 = cmp.eq(r6,#0x1) }
	{ r12 = memw(r26+8); r10 = p1 }
	{ memw(r29+92) = r4; r9 = memw(r19+8); p2 = cmp.eq(r12,#0x1); r18 = mux(p0,r4,r6) }
	{ r7 = memw(r26+12); r4 = mux(p1,r8,r0) }
	{ r0 = memw(r19+12); p1 = cmp.eq(r7,#0x1); r2 = mpyi(r4,r18) }

;; fn0001F0A4: 0001F0A4
;;   Called from:
;;     0001EF14 (in qsigmoid_execute_hvx)
;;     0001F098 (in add_f_execute)
fn0001F0A4 proc
	{ memw(r29+76) = r6; memd(r29+120) = r25:r24; r20 = mux(p2,r9,r12) }
	{ r21 = mux(p1,r0,r7); r2 = mpyi(r2,r20) }
	{ memd(r29+152) = r17:r16; r25 = memw(r3); r6 = r1; r2 = mpyi(r2,r21) }
	{ memw(r29+72) = r8; memw(r29+96) = r10; r10 = p2 }
	{ memw(r29+84) = r10; memw(r29+68) = r9 }
	{ memw(r29+80) = r0; memw(r29+104) = r4; r16 = asl(r2,#0x2) }
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
;;     0001F36C (in add_f_execute)
fn0001F0EC proc
	{ r2 = r6; r1 = #0xBC; r0 = add(PC,#0xAB22); immext(#0xAB00) }
	{ memw(r29+64) = r12; r3 = memw(r19+16); r4 = add(PC,#0xAB31); immext(#0xAB00) }
	{ memw(r29+88) = r7; r23 = memw(r25+16); r27 = r5; r24 = r6 }
	{ r17 = memw(r26+16) }
	{ memw(r29) = r5; memw(r29+100) = r3; call logmsg_function }
	{ if (!cmp.gtu(r16,r2.new)) jump:t 0001F154; r2 = memw(r25+20); r1 = #0xBC }

l0001F138:
	{ r2 = r24; r0 = add(PC,#0x1A) }
	{ r3 = add(PC,#0xAB0B); immext(#0xAB00) }

l0001F148:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001F380 }

l0001F154:
	{ r5 = memw(r26); r13 = memw(r19); r2 = r24 }
	{ r7 = memw(r26+8); r8 = memw(r26+12); p0 = cmp.eq(r5,r13) }
	{ r12 = memw(r19+12); r6 = memw(r26+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+52) = r16; memw(r29+48) = r25 }
	{ memw(r29+60) = r27; memw(r29+56) = r17 }
	{ if (p0) jump:nt 0001F19C }

l0001F190:
	{ if (p0.new) jump:nt 0001F19C; p0 = cmp.eq(r5,#0x1) }

l0001F194:
	{ p0 = cmp.eq(r13,#0x1); if (!p0.new) jump:nt 0001F1D8 }

l0001F19C:
	{ if (p0.new) jump:nt 0001F1AC; p0 = cmp.eq(r6,r3) }

l0001F1A0:
	{ if (p0.new) jump:nt 0001F1AC; p0 = cmp.eq(r6,#0x1) }

l0001F1A4:
	{ nop; if (!p0.new) jump:nt 0001F1D8; p0 = cmp.eq(r3,#0x1) }

l0001F1AC:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 0001F1C0 }

l0001F1B4:
	{ if (p0.new) jump:nt 0001F1C0; p0 = cmp.eq(r7,#0x1) }

l0001F1B8:
	{ p0 = cmp.eq(r9,#0x1); if (!p0.new) jump:nt 0001F1D8 }

l0001F1C0:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 0001F20C }

l0001F1C8:
	{ p0 = cmp.eq(r8,#0x1); if (p0.new) jump:nt 0001F20C }

l0001F1D0:
	{ p0 = cmp.eq(r12,#0x1); if (p0.new) jump:nt 0001F20C }

l0001F1D8:
	{ memw(r29+28) = r12; r1 = #0xBC; r0 = add(PC,#0xAA36); immext(#0xAA00) }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+20) = r3; r3 = add(PC,#0xAA69); immext(#0xAA40) }
	{ memw(r29+4) = r6; memw(r29+12) = r8 }
	{ memw(r29) = r5; jump 0001F148 }

l0001F20C:
	{ memw(r29+44) = r21; r24 = memw(r29+104); r19 = r2 }
	{ memw(r29+20) = r3; memw(r29+36) = r18; r0 = add(PC,#0xA9F6); immext(#0xA9C0) }
	{ memw(r29+40) = r20; r1 = #0xBC; r4 = add(PC,#0xAA65); immext(#0xAA40) }
	{ memw(r29+28) = r12; memw(r29+32) = r24 }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; memw(r29+4) = r6 }
	{ call logmsg_function }
	{ r26 = memw(r29+100); r3 = memw(r29+48); if (p0.new) r14 = #0x0; p0 = cmp.gt(r24,#0x0) }
	{ r25 = memw(r29+56) }
	{ memb(r3+6) = r2.new; r2 = memw(r29+52) }
	{ memw(r3+4) = r18 }
	{ memw(r3+12) = r21; memw(r3+8) = r20 }
	{ if (p0) r12 = memw(r29+88); if (p0) r13 = memw(r29+80); if (!p0) jump:nt 0001F358 }

l0001F288:
	{ r7 = memd(r29+92); r5 = memd(r29+68); r6 = !cmp.eq(r13,00000001) }
	{ r2 = memd(r29+76); r0 = memd(r29+84); r9 = mpyi(r5,r7) }
	{ r4 = memd(r29+72); r3 = memd(r29+64); p0 = cmp.eq(r5,#0x1); p2 = r0 }
	{ r0 = memw(r29+96); p1 = cmp.eq(r4,#0x1); r9 = mpyi(r9,r13); r8 = mpyi(r3,r2) }
	{ if (p1) r9 = add(r14,#0x0); p2 = cmp.eq(r7,#0x1); r2 = mux(p2,#0x0,r12); r4 = mpyi(r13,r5) }
	{ r5 = !cmp.eq(r12,00000001); r3 = mux(p0,#0x0,r13); p0 = r0; r8 = mpyi(r8,r12) }
	{ if (p0) r8 = add(r14,#0x0); if (p2) r4 = add(r14,#0x0); r7 = #0x0 }

l0001F2E0:
	{ r14 = #0x0; r13:r12 = combine(r25,r26); if (!p0.new) jump:nt 0001F348; p0 = cmp.gt(r10,#0x0) }

l0001F2E4:
	{ r14 = #0x0; r13:r12 = combine(r25,r26) }

l0001F2EC:
	{ r15 = r13; r28 = r12; if (!p0.new) jump:nt 0001F338; p0 = cmp.gt(r12,#0x0) }

l0001F2F8:
	{ loop1(0001F2FC,r20) }
	{ if (!p0) r1:r0 = combine(r15,r28); r10 = r23; if (!p0.new) jump:nt 0001F32C; p0 = cmp.gt(r13,#0x0) }

l0001F308:
	{ loop0(0001F30C,r21) }
	{ r16 = memw(r0); r11 = memw(r1); r1 = addasl(r1,r5,#0x2); r0 = addasl(r0,r6,#0x2) }
	{ r11 = sfadd(r11,r16) }
	{ memw(r10) = r11; r10 = add(r10,#0x4) }
	{ r23 = addasl(r23,r21,#0x2) }

l0001F32C:
	{ nop; r15 = addasl(r15,r2,#0x2); r28 = addasl(r28,r3,#0x2) }

l0001F338:
	{ if (!cmp.eq(r14.new,r18)) jump:t 0001F2EC; r14 = add(r14,#0x1); r12 = addasl(r12,r4,#0x2); r13 = addasl(r13,r22,#0x2) }

l0001F348:
	{ if (!cmp.eq(r7.new,r24)) jump:t 0001F2E0; r7 = add(r7,#0x1); r26 = addasl(r26,r9,#0x2); r25 = addasl(r25,r8,#0x2) }

l0001F34C:
	{ if (!cmp.eq(r7.new,r24)) jump:t 0001F2E4; r7 = add(r7,#0x1); r26 = addasl(r26,r9,#0x2) }

l0001F358:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,#0xA8B6); immext(#0xA880) }

l0001F35C:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,#0x36) }

l0001F36C:
	{ r2 = r19; r1 = #0xBC; r4 = add(PC,#0x11) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001F380:
	{ r19:r18 = memd(r29+144); r17:r16 = memd(r29+152) }
	{ r23:r22 = memd(r29+128); r21:r20 = memd(r29+136) }
	{ r27:r26 = memd(r29+112); r25:r24 = memd(r29+120) }
	{ dealloc_return }

;; add_f_check: 0001F394
add_f_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xA83F); immext(#0xA800) }
	{ r16 = r1; r1 = #0x37; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,#0xA810); immext(#0xA800) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,#0x2)) jump:t 0001F3E4; r2 = memw(r17+16); r1 = #0x38 }

l0001F3C8:
	{ r0 = add(PC,#0x34) }
	{ r3 = add(PC,#0xA813); immext(#0xA800) }

l0001F3D4:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001F3E4:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001F404; r2 = memw(r17+20); r1 = #0x39 }

l0001F3F4:
	{ r0 = add(PC,#0x8) }
	{ jump 0001F3D4; r3 = add(PC,#0xA7F6); immext(#0xA7C0) }

l0001F404:
	{ r2 = r16; r1 = #0x3A; r0 = add(PC,#0xA7B4); immext(#0xA780) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,#0xA7EE); immext(#0xA7C0) }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001F428
;;   Called from:
;;     0001F120 (in fn0001F0EC)
;;     0001F250 (in fn0001F0EC)
;;     0001F378 (in fn0001F0EC)
;;     0001F3B4 (in add_f_check)
;;     0001F410 (in add_f_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001F448; r5 = memw(r2+16) }

l0001F438:
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10) }

l0001F448:
	{ dealloc_return }

;; errlog_function: 0001F44C
;;   Called from:
;;     0001F148 (in fn0001F0EC)
;;     0001F3D4 (in add_f_check)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r3 = #0x0 }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; mul_f_execute: 0001F470
;;   Called from:
;;     0001F46C (in errlog_function)
mul_f_execute proc
	{ allocframe(#0xA8); memd(r29-40) = r23:r22; r4 = add(PC,#0xA8BA); immext(#0xA880) }
	{ memd(r29+120) = r27:r26; memd(r29+128) = r25:r24; r23:r22 = combine(r1,#0x0) }
	{ memd(r29+160) = r17:r16; r16 = r0; r2 = r23; r1 = #0x32 }
	{ memw(r29) = r16; r0 = add(PC,#0xA840); immext(#0xA840) }
	{ memd(r29+144) = r21:r20; memd(r29+152) = r19:r18 }
	{ call logmsg_function }
	{ r5 = r16 }
	{ r3 = memw(r5+8); r2 = memw(r5+4) }
	{ r16 = memw(r2) }
	{ r3 = memw(r3); r19 = memw(r2+4) }
	{ r1 = memw(r16); r7 = memw(r16+4) }
	{ r4 = memw(r19+4); r8 = memw(r19); p1 = cmp.eq(r1,#0x1); p0 = cmp.eq(r7,#0x1) }
	{ r12 = memw(r16+8) }
	{ memw(r29+96) = r4; r9 = memw(r19+8); p2 = cmp.eq(r12,#0x1); r18 = mux(p0,r4,r7) }
	{ r6 = memw(r16+12); r4 = mux(p1,r8,r1); r0 = p1 }
	{ r1 = memw(r19+12); p1 = cmp.eq(r6,#0x1); r2 = mpyi(r4,r18) }
	{ memw(r29+112) = r4; memw(r29+100) = r0; r20 = mux(p2,r9,r12); r0 = p2 }
	{ memw(r29+80) = r7; memw(r29+72) = r9; r21 = mux(p1,r1,r6); r2 = mpyi(r2,r20) }
	{ memw(r29+88) = r0; memw(r29+76) = r8; r2 = mpyi(r2,r21) }
	{ memw(r29+84) = r1; if (p0) jump:nt 0001F530; r17 = asl(r2,#0x2) }

l0001F52C:
	{ r22 = mpyi(r6,r12) }

l0001F530:
	{ r2 = r23; r1 = #0xBC; r0 = add(PC,#0xA808); immext(#0xA800) }
	{ memw(r29+92) = r6; r27 = r23; r4 = add(PC,#0xA817); immext(#0xA800) }
	{ r6 = memw(r19+16) }
	{ memw(r29+68) = r12; r23 = memw(r3+16) }
	{ r26 = memw(r16+16); r25:r24 = combine(r3,r5) }
	{ memw(r29) = r5; memw(r29+108) = r6; call logmsg_function }
	{ if (!cmp.gtu(r17,r2.new)) jump:t 0001F598; r2 = memw(r25+20); r1 = #0xBC }

l0001F57C:
	{ r2 = r27; r0 = add(PC,#0x0) }
	{ r3 = add(PC,#0xA7F1); immext(#0xA7C0) }

l0001F58C:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001F7AC }

l0001F598:
	{ r5 = memw(r16); r2 = memw(r19) }
	{ r7 = memw(r16+8); r8 = memw(r16+12); p0 = cmp.eq(r5,r2) }
	{ r12 = memw(r19+12); r6 = memw(r16+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+56) = r17; memw(r29+52) = r25 }
	{ memw(r29+64) = r24; memw(r29+60) = r26 }
	{ memw(r29+104) = r27; if (p0) jump:nt 0001F5D8 }

l0001F5D0:
	{ if (p0.new) jump:nt 0001F5D8; p0 = cmp.eq(r5,#0x1) }

l0001F5D4:
	{ if (!p0.new) jump:nt 0001F614; p0 = cmp.eq(r2,#0x1) }

l0001F5D8:
	{ nop; if (p0.new) jump:nt 0001F5E8; p0 = cmp.eq(r6,r3) }

l0001F5E0:
	{ if (p0.new) jump:nt 0001F5E8; p0 = cmp.eq(r6,#0x1) }

l0001F5E4:
	{ if (!p0.new) jump:nt 0001F614; p0 = cmp.eq(r3,#0x1) }

l0001F5E8:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 0001F5FC }

l0001F5F0:
	{ if (p0.new) jump:nt 0001F5FC; p0 = cmp.eq(r7,#0x1) }

l0001F5F4:
	{ p0 = cmp.eq(r9,#0x1); if (!p0.new) jump:nt 0001F614 }

l0001F5FC:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 0001F648 }

l0001F604:
	{ p0 = cmp.eq(r8,#0x1); if (p0.new) jump:nt 0001F648 }

l0001F60C:
	{ p0 = cmp.eq(r12,#0x1); if (p0.new) jump:nt 0001F648 }

l0001F614:
	{ memw(r29+28) = r12; r1 = #0xBC; r0 = add(PC,#0xA724); immext(#0xA700) }
	{ memw(r29+16) = r2; memw(r29+24) = r9 }
	{ memw(r29+4) = r6; memw(r29+20) = r3; r3 = add(PC,#0xA757); immext(#0xA740) }
	{ memw(r29+8) = r7; r2 = memd(r29+104) }
	{ memw(r29) = r5; memw(r29+12) = r8 }
	{ jump 0001F58C }

l0001F648:
	{ memw(r29+44) = r21; r19 = memd(r29+112); r0 = add(PC,#0xA6F0); immext(#0xA6C0) }
	{ memw(r29+32) = r19; r1 = #0xBC; r4 = add(PC,#0xA75F); immext(#0xA740) }
	{ memw(r29+36) = r18; memw(r29+40) = r20 }
	{ memw(r29+24) = r9; memw(r29+28) = r12 }
	{ memw(r29+16) = r2; memw(r29+20) = r3 }
	{ memw(r29+4) = r6; r2 = memd(r29+104) }
	{ memw(r29+8) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; call logmsg_function }
	{ r17 = memd(r29+108); r3 = memd(r29+52); if (p0.new) r14 = #0x0; p0 = cmp.gt(r19,#0x0) }
	{ r24 = memw(r29+60) }
	{ memb(r3+6) = r2.new; r2 = memw(r29+56) }
	{ memw(r3+12) = r21; memw(r3+8) = r20 }
	{ if (p0) r12 = memw(r29+92); if (p0) r13 = memw(r29+84); if (!p0) jump:nt 0001F784 }

l0001F6B4:
	{ r7 = memd(r29+96); r5 = memd(r29+72); r6 = !cmp.eq(r13,00000001) }
	{ r2 = memd(r29+80); r0 = memd(r29+88); r9 = mpyi(r5,r7) }
	{ r4 = memd(r29+76); r3 = memd(r29+68); p0 = cmp.eq(r5,#0x1); p2 = r0 }
	{ r0 = memw(r29+100); p1 = cmp.eq(r4,#0x1); r9 = mpyi(r9,r13); r8 = mpyi(r3,r2) }
	{ if (p1) r9 = add(r14,#0x0); p2 = cmp.eq(r7,#0x1); r2 = mux(p2,#0x0,r12); r4 = mpyi(r13,r5) }
	{ r5 = !cmp.eq(r12,00000001); r3 = mux(p0,#0x0,r13); p0 = r0; r8 = mpyi(r8,r12) }
	{ if (p0) r8 = add(r14,#0x0); if (p2) r4 = add(r14,#0x0); r7 = #0x0 }

l0001F70C:
	{ r14 = #0x0; r13:r12 = combine(r24,r17); if (!p0.new) jump:nt 0001F774; p0 = cmp.gt(r10,#0x0) }

l0001F710:
	{ r14 = #0x0; r13:r12 = combine(r24,r17) }

l0001F718:
	{ r15 = r13; r28 = r12; if (!p0.new) jump:nt 0001F764; p0 = cmp.gt(r12,#0x0) }

l0001F724:
	{ loop1(0001F728,r20) }
	{ if (!p0) r1:r0 = combine(r15,r28); r10 = r23; if (!p0.new) jump:nt 0001F758; p0 = cmp.gt(r13,#0x0) }

l0001F734:
	{ loop0(0001F738,r21) }
	{ r16 = memw(r0); r11 = memw(r1); r1 = addasl(r1,r5,#0x2); r0 = addasl(r0,r6,#0x2) }
	{ r11 = sfmpy(r11,r16) }
	{ memw(r10) = r11; r10 = add(r10,#0x4) }
	{ r23 = addasl(r23,r21,#0x2) }

l0001F758:
	{ nop; r15 = addasl(r15,r2,#0x2); r28 = addasl(r28,r3,#0x2) }

l0001F764:
	{ if (!cmp.eq(r14.new,r18)) jump:t 0001F718; r14 = add(r14,#0x1); r12 = addasl(r12,r4,#0x2); r13 = addasl(r13,r22,#0x2) }

l0001F774:
	{ if (!cmp.eq(r7.new,r19)) jump:t 0001F70C; r7 = add(r7,#0x1); r17 = addasl(r17,r9,#0x2); r24 = addasl(r24,r8,#0x2) }

l0001F778:
	{ if (!cmp.eq(r7.new,r19)) jump:t 0001F710; r7 = add(r7,#0x1); r17 = addasl(r17,r9,#0x2) }

l0001F784:
	{ memb(r29) = r2.new; r2 = memw(r29+64); r0 = add(PC,#0xA5B4); immext(#0xA580) }

l0001F788:
	{ memb(r29) = r2.new; r2 = memw(r29+64); r0 = add(PC,#0x34) }

l0001F798:
	{ r2 = memw(r29+104); r1 = #0xBC; r4 = add(PC,#0xF) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001F7AC:
	{ r19:r18 = memd(r29+152); r17:r16 = memd(r29+160) }
	{ r23:r22 = memd(r29+136); r21:r20 = memd(r29+144) }
	{ r27:r26 = memd(r29+120); r25:r24 = memd(r29+128) }
	{ dealloc_return }

;; mul_f_check: 0001F7C0
mul_f_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xA52F); immext(#0xA500) }
	{ r16 = r1; r1 = #0x38; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,#0xA500); immext(#0xA500) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,#0x2)) jump:t 0001F810; r2 = memw(r17+16); r1 = #0x39 }

l0001F7F4:
	{ r0 = add(PC,#0x24) }
	{ r3 = add(PC,#0xA503); immext(#0xA500) }

l0001F800:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001F810:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001F830; r2 = memw(r17+20); r1 = #0x3A }

l0001F820:
	{ r0 = add(PC,#0x38) }
	{ jump 0001F800; r3 = add(PC,#0xA4E6); immext(#0xA4C0) }

l0001F830:
	{ r2 = r16; r1 = #0x3B; r0 = add(PC,#0xA4A4); immext(#0xA480) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,#0xA4DE); immext(#0xA4C0) }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001F854
;;   Called from:
;;     0001F4A4 (in mul_f_execute)
;;     0001F564 (in mul_f_execute)
;;     0001F680 (in mul_f_execute)
;;     0001F7A4 (in mul_f_execute)
;;     0001F7E0 (in mul_f_check)
;;     0001F83C (in mul_f_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001F874; r5 = memw(r2+16) }

l0001F864:
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10) }

l0001F874:
	{ dealloc_return }

;; errlog_function: 0001F878
;;   Called from:
;;     0001F58C (in mul_f_execute)
;;     0001F800 (in mul_f_check)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r3 = #0x0 }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; neg_f_execute: 0001F8A0
;;   Called from:
;;     0001F89C (in errlog_function)
neg_f_execute proc
	{ allocframe(#0x0) }
	{ r5 = memw(r0+8); r7 = memw(r0+4) }
	{ r3 = memw(r7) }
	{ r7 = memw(r3+12); r2 = memw(r3+4) }
	{ r6 = memw(r3+8); r4 = memw(r3) }
	{ r4 = memw(r5); r2 = mpyi(r2,r4) }
	{ r6 = memw(r4+20); r2 = mpyi(r2,r6) }
	{ r2 = r1; r7 = mpyi(r2,r7) }
	{ if (cmp.gtu(r5.new,r6)) jump:t 0001F910; r5 = asl(r7,#0x2) }

l0001F8D8:
	{ r6 = memw(r3+16); r2 = memw(r4+16); loop0(0001F8E0,r7) }
	{ r7 = memw(r6); r6 = add(r6,#0x4) }
	{ r7 = togglebit(r7,#0x1E) }
	{ memw(r2) = r7; r2 = add(r2,#0x4); nop }
	{ r6 = memw(r3+4); r2 = memw(r3); r0 = #0x0 }
	{ memw(r4) = r2; memw(r4+4) = r6 }
	{ r1 = memw(r3+8) }
	{ memw(r4+8) = r1 }
	{ r7 = memw(r3+12) }
	{ memw(r4+24) = r5; memw(r4+12) = r7 }
	{ dealloc_return }

l0001F910:
	{ r1 = #0x36; call errlog_function; r3 = add(PC,#0xA544); immext(#0xA540) }
	{ dealloc_return; r0 = #-0x1 }

;; neg_f_check: 0001F924
neg_f_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xA4F5); immext(#0xA4C0) }
	{ r17 = r0; r16 = r1; r1 = #0x41 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001F964; r2 = memw(r17+16); r1 = #0x42 }

l0001F950:
	{ r3 = add(PC,#0x19) }
	{ r2 = r16; call errlog_function }

l0001F958:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001F964:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001F97C; r2 = memw(r17+20); r1 = #0x43 }

l0001F974:
	{ jump 0001F958; r3 = add(PC,#0x4) }

l0001F97C:
	{ r2 = r16; r1 = #0x44; r4 = add(PC,#0xA4C8); immext(#0xA4C0) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001F99C
;;   Called from:
;;     0001F938 (in neg_f_check)
;;     0001F98C (in neg_f_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001F9C0; r5 = memw(r2+16) }

l0001F9AC:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x16) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001F9C0:
	{ dealloc_return }

;; errlog_function: 0001F9C4
;;   Called from:
;;     0001F910 (in neg_f_execute)
;;     0001F954 (in neg_f_check)
;;     0001F9B4 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xA43A); immext(#0xA400) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001F9E8                         00 00 00 00 00 00 00 00         ........

;; sub_f_execute: 0001F9F0
sub_f_execute proc
	{ allocframe(#0xA0); r5 = r0 }
	{ r3 = memw(r5+8); r2 = memw(r5+4) }
	{ memd(r29+144) = r19:r18; memd(r29+112) = r27:r26 }
	{ r19 = memw(r2+4); r26 = memw(r2) }
	{ memd(r29+128) = r23:r22; memd(r29+136) = r21:r20; r22 = #0x0 }
	{ r0 = memw(r26); r6 = memw(r26+4) }
	{ r4 = memw(r19+4); r8 = memw(r19); p1 = cmp.eq(r0,#0x1); p0 = cmp.eq(r6,#0x1) }
	{ r12 = memw(r26+8); r10 = p1 }
	{ memw(r29+92) = r4; r9 = memw(r19+8); p2 = cmp.eq(r12,#0x1); r18 = mux(p0,r4,r6) }
	{ r7 = memw(r26+12); r4 = mux(p1,r8,r0) }
	{ r0 = memw(r19+12); p1 = cmp.eq(r7,#0x1); r2 = mpyi(r4,r18) }
	{ memw(r29+76) = r6; memd(r29+120) = r25:r24; r20 = mux(p2,r9,r12) }
	{ r21 = mux(p1,r0,r7); r2 = mpyi(r2,r20) }
	{ memd(r29+152) = r17:r16; r25 = memw(r3); r6 = r1; r2 = mpyi(r2,r21) }
	{ memw(r29+72) = r8; memw(r29+96) = r10; r10 = p2 }
	{ memw(r29+84) = r10; memw(r29+68) = r9 }
	{ memw(r29+80) = r0; memw(r29+104) = r4; r16 = asl(r2,#0x2) }
	{ if (p0) jump:nt 0001FA9C }

l0001FA98:
	{ r22 = mpyi(r7,r12) }

l0001FA9C:
	{ r2 = r6; r1 = #0xBC; r0 = add(PC,#0xA41C); immext(#0xA400) }
	{ memw(r29+64) = r12; r3 = memw(r19+16); r4 = add(PC,#0xA42B); immext(#0xA400) }
	{ memw(r29+88) = r7; r23 = memw(r25+16); r27 = r5; r24 = r6 }
	{ r17 = memw(r26+16) }
	{ memw(r29) = r5; memw(r29+100) = r3; call logmsg_function }
	{ if (!cmp.gtu(r16,r2.new)) jump:t 0001FB04; r2 = memw(r25+20); r1 = #0xBC }

l0001FAE8:
	{ r2 = r24; r0 = add(PC,#0x14) }
	{ r3 = add(PC,#0xA405); immext(#0xA400) }

l0001FAF8:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001FD30 }

l0001FB04:
	{ r5 = memw(r26); r13 = memw(r19); r2 = r24 }
	{ r7 = memw(r26+8); r8 = memw(r26+12); p0 = cmp.eq(r5,r13) }
	{ r12 = memw(r19+12); r6 = memw(r26+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+52) = r16; memw(r29+48) = r25 }
	{ memw(r29+60) = r27; memw(r29+56) = r17 }
	{ if (p0) jump:nt 0001FB4C }

l0001FB40:
	{ if (p0.new) jump:nt 0001FB4C; p0 = cmp.eq(r5,#0x1) }

l0001FB44:
	{ p0 = cmp.eq(r13,#0x1); if (!p0.new) jump:nt 0001FB88 }

l0001FB4C:
	{ if (p0.new) jump:nt 0001FB5C; p0 = cmp.eq(r6,r3) }

l0001FB50:
	{ if (p0.new) jump:nt 0001FB5C; p0 = cmp.eq(r6,#0x1) }

l0001FB54:
	{ nop; if (!p0.new) jump:nt 0001FB88; p0 = cmp.eq(r3,#0x1) }

l0001FB5C:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 0001FB70 }

l0001FB64:
	{ if (p0.new) jump:nt 0001FB70; p0 = cmp.eq(r7,#0x1) }

l0001FB68:
	{ p0 = cmp.eq(r9,#0x1); if (!p0.new) jump:nt 0001FB88 }

l0001FB70:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 0001FBBC }

l0001FB78:
	{ p0 = cmp.eq(r8,#0x1); if (p0.new) jump:nt 0001FBBC }

l0001FB80:
	{ p0 = cmp.eq(r12,#0x1); if (p0.new) jump:nt 0001FBBC }

l0001FB88:
	{ memw(r29+28) = r12; r1 = #0xBC; r0 = add(PC,#0xA330); immext(#0xA300) }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+20) = r3; r3 = add(PC,#0xA363); immext(#0xA340) }
	{ memw(r29+4) = r6; memw(r29+12) = r8 }
	{ memw(r29) = r5; jump 0001FAF8 }

l0001FBBC:
	{ memw(r29+44) = r21; r24 = memw(r29+104); r19 = r2 }
	{ memw(r29+20) = r3; memw(r29+36) = r18; r0 = add(PC,#0xA2F0); immext(#0xA2C0) }
	{ memw(r29+40) = r20; r1 = #0xBC; r4 = add(PC,#0xA35F); immext(#0xA340) }
	{ memw(r29+28) = r12; memw(r29+32) = r24 }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; memw(r29+4) = r6 }
	{ call logmsg_function }
	{ r26 = memw(r29+100); r3 = memw(r29+48); if (p0.new) r14 = #0x0; p0 = cmp.gt(r24,#0x0) }
	{ r25 = memw(r29+56) }
	{ memb(r3+6) = r2.new; r2 = memw(r29+52) }
	{ memw(r3+4) = r18 }
	{ memw(r3+12) = r21; memw(r3+8) = r20 }
	{ if (p0) r12 = memw(r29+88); if (p0) r13 = memw(r29+80); if (!p0) jump:nt 0001FD08 }

l0001FC38:
	{ r7 = memd(r29+92); r5 = memd(r29+68); r6 = !cmp.eq(r13,00000001) }
	{ r2 = memd(r29+76); r0 = memd(r29+84); r9 = mpyi(r5,r7) }
	{ r4 = memd(r29+72); r3 = memd(r29+64); p0 = cmp.eq(r5,#0x1); p2 = r0 }
	{ r0 = memw(r29+96); p1 = cmp.eq(r4,#0x1); r9 = mpyi(r9,r13); r8 = mpyi(r3,r2) }
	{ if (p1) r9 = add(r14,#0x0); p2 = cmp.eq(r7,#0x1); r2 = mux(p2,#0x0,r12); r4 = mpyi(r13,r5) }
	{ r5 = !cmp.eq(r12,00000001); r3 = mux(p0,#0x0,r13); p0 = r0; r8 = mpyi(r8,r12) }
	{ if (p0) r8 = add(r14,#0x0); if (p2) r4 = add(r14,#0x0); r7 = #0x0 }

l0001FC90:
	{ r14 = #0x0; r13:r12 = combine(r25,r26); if (!p0.new) jump:nt 0001FCF8; p0 = cmp.gt(r10,#0x0) }

l0001FC94:
	{ r14 = #0x0; r13:r12 = combine(r25,r26) }

l0001FC9C:
	{ r15 = r13; r28 = r12; if (!p0.new) jump:nt 0001FCE8; p0 = cmp.gt(r12,#0x0) }

l0001FCA8:
	{ loop1(0001FCAC,r20) }
	{ if (!p0) r1:r0 = combine(r15,r28); r10 = r23; if (!p0.new) jump:nt 0001FCDC; p0 = cmp.gt(r13,#0x0) }

l0001FCB8:
	{ loop0(0001FCBC,r21) }
	{ r16 = memw(r0); r11 = memw(r1); r1 = addasl(r1,r5,#0x2); r0 = addasl(r0,r6,#0x2) }
	{ r11 = sfsub(r11,r16) }
	{ memw(r10) = r11; r10 = add(r10,#0x4) }
	{ r23 = addasl(r23,r21,#0x2) }

l0001FCDC:
	{ nop; r15 = addasl(r15,r2,#0x2); r28 = addasl(r28,r3,#0x2) }

l0001FCE8:
	{ if (!cmp.eq(r14.new,r18)) jump:t 0001FC9C; r14 = add(r14,#0x1); r12 = addasl(r12,r4,#0x2); r13 = addasl(r13,r22,#0x2) }

l0001FCF8:
	{ if (!cmp.eq(r7.new,r24)) jump:t 0001FC90; r7 = add(r7,#0x1); r26 = addasl(r26,r9,#0x2); r25 = addasl(r25,r8,#0x2) }

l0001FCFC:
	{ if (!cmp.eq(r7.new,r24)) jump:t 0001FC94; r7 = add(r7,#0x1); r26 = addasl(r26,r9,#0x2) }

l0001FD08:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,#0xA1B0); immext(#0xA180) }

l0001FD0C:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,#0x30) }

l0001FD1C:
	{ r2 = r19; r1 = #0xBC; r4 = add(PC,#0xB) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l0001FD30:
	{ r19:r18 = memd(r29+144); r17:r16 = memd(r29+152) }
	{ r23:r22 = memd(r29+128); r21:r20 = memd(r29+136) }
	{ r27:r26 = memd(r29+112); r25:r24 = memd(r29+120) }
	{ dealloc_return }

;; sub_f_check: 0001FD44
sub_f_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xA139); immext(#0xA100) }
	{ r16 = r1; r1 = #0x37; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,#0xA10A); immext(#0xA100) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,#0x2)) jump:t 0001FD94; r2 = memw(r17+16); r1 = #0x38 }

l0001FD78:
	{ r0 = add(PC,#0x2E) }
	{ r3 = add(PC,#0xA10D); immext(#0xA100) }

l0001FD84:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001FD94:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001FDB4; r2 = memw(r17+20); r1 = #0x39 }

l0001FDA4:
	{ r0 = add(PC,#0x2) }
	{ jump 0001FD84; r3 = add(PC,#0xA0F0); immext(#0xA0C0) }

l0001FDB4:
	{ r2 = r16; r1 = #0x3A; r0 = add(PC,#0xA0AE); immext(#0xA080) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,#0xA0E8); immext(#0xA0C0) }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001FDD8
;;   Called from:
;;     0001FAD0 (in sub_f_execute)
;;     0001FC00 (in sub_f_execute)
;;     0001FD28 (in sub_f_execute)
;;     0001FD64 (in sub_f_check)
;;     0001FDC0 (in sub_f_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001FDF8; r5 = memw(r2+16) }

l0001FDE8:
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10) }

l0001FDF8:
	{ dealloc_return }

;; errlog_function: 0001FDFC
;;   Called from:
;;     0001FAF8 (in sub_f_execute)
;;     0001FD84 (in sub_f_check)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r3 = #0x0 }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; rank_execute: 0001FE20
;;   Called from:
;;     0001FE1C (in errlog_function)
rank_execute proc
	{ allocframe(#0x18); memd(r29-16) = r17:r16; r4 = add(PC,#0xA1C4); immext(#0xA1C0) }
	{ r1 = #0x32; r17:r16 = combine(r1,r0) }
	{ memd(r29+8) = r19:r18; r2 = memw(r16+4) }
	{ r7 = memw(r16+8) }
	{ r5 = memw(r2+4); r2 = r17 }
	{ r3 = memw(r5+16); r19 = memw(r7) }
	{ memw(r29) = r16; r18 = memw(r3); call logmsg_function }
	{ if (cmp.gtu(r2.new,#0x3)) jump:t 0001FE6C; r2 = memw(r19+20) }

l0001FE58:
	{ r2 = r17; r1 = #0x35; r3 = add(PC,#0x27) }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 0001FE9C }

l0001FE6C:
	{ r3 = memw(r19+16); r1 = #0x3B; r4 = add(PC,#0xA19D); immext(#0xA180) }
	{ memw(r19) = #0x1; memw(r19+12) = #0x1; r2 = r17 }
	{ memw(r19+24) = #0x4; memw(r19+8) = #0x1 }
	{ memw(r3) = r18; memw(r19+4) = #0xFFFFFF81 }
	{ memw(r29) = r16; call logmsg_function }
	{ r0 = #0x0 }

l0001FE9C:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16) }
	{ dealloc_return }

;; rank_check: 0001FEA4
rank_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0xA0F4); immext(#0xA0C0) }
	{ r17 = r0; r16 = r1; r1 = #0x41 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x2)) jump:t 0001FEE4; r2 = memw(r17+16); r1 = #0x42 }

l0001FED0:
	{ r3 = add(PC,#0x22) }
	{ r2 = r16; call errlog_function }

l0001FED8:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0001FEE4:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0001FEFC; r2 = memw(r17+20); r1 = #0x43 }

l0001FEF4:
	{ jump 0001FED8; r3 = add(PC,#0xD) }

l0001FEFC:
	{ r2 = r16; r1 = #0x44; r4 = add(PC,#0xA0D1); immext(#0xA0C0) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001FF1C
;;   Called from:
;;     0001FE44 (in rank_execute)
;;     0001FE90 (in rank_execute)
;;     0001FEB8 (in rank_check)
;;     0001FF0C (in rank_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001FF40; r5 = memw(r2+16) }

l0001FF2C:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x16) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0001FF40:
	{ dealloc_return }

;; errlog_function: 0001FF44
;;   Called from:
;;     0001FE60 (in rank_execute)
;;     0001FED4 (in rank_check)
;;     0001FF34 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0xA03A); immext(#0xA000) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0001FF68                         00 00 00 00 00 00 00 00         ........

;; range_execute: 0001FF70
range_execute proc
	{ allocframe(#0x28); memd(r29-16) = r17:r16; r4 = add(PC,#0xA10E); immext(#0xA100) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+8) = r23:r22; r2 = memw(r17+4); r1 = #0x38 }
	{ r3 = memw(r17+8) }
	{ memd(r29+16) = r21:r20; memd(r29+24) = r19:r18 }
	{ r0 = memw(r2+4); r5 = memw(r2+8) }
	{ r22 = memw(r3); r2 = memw(r2) }
	{ r5 = memw(r5+16); r7 = memw(r0+16) }
	{ r18 = memw(r22+16); r6 = memw(r2+16); r2 = r16 }
	{ r21 = memw(r5); r19 = memw(r7) }
	{ memw(r29) = r17; r20 = memw(r6); call logmsg_function }
	{ if (!p0.new) r3 = add(r20,#0x0); if (!p0.new) r2 = #0x0; if (p0.new) jump:nt 0001FFD0; p0 = cmp.gt(r11,r12) }

l0001FFBC:
	{ r3:r2 = combine(r20,#0x0) }

l0001FFC0:
	{ if (cmp.gtu(r19,r3.new)) jump:t 0001FFC0; r3 = add(r3,r21); r2 = add(r2,#0x1) }

l0001FFD0:
	{ if (p1.new) jump:nt 0001FFE0; p1 = cmp.gt(r12,r11) }

l0001FFD4:
	{ if (cmp.gt(r3.new,r19)) jump:t 0001FFD4; r3 = add(r3,r21); r2 = add(r2,#0x1) }

l0001FFE0:
	{ r4 = memw(r22+20) }

l0001FFE4:
	{ if (!cmp.gtu(r3.new,r4)) jump:t 00020004; r3 = asl(r2,#0x2) }

l0001FFF0:
	{ r2 = r16; r1 = #0x3F; r3 = add(PC,#0x2A) }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 00020050 }

l00020004:
	{ memw(r22+4) = #0x1; memw(r22) = #0x1 }
	{ memw(r22+12) = r2; memw(r22+8) = #0x1 }
	{ memw(r22+24) = r3; if (!p0) jump:nt 00020030 }

l00020018:
	{ memw(r18++#4) = r20 }
	{ if (cmp.gtu(r19,r20.new)) jump:t 00020018; r20 = add(r20,r21) }

l00020028:
	{ memw(r18++#4) = r20; r20 = add(r20,r21) }

l00020030:
	{ if (p0.new) jump:nt 00020028; p0 = cmp.gt(r12,r11) }

l00020034:
	{ r2 = r16; r1 = #0x49; r4 = add(PC,#0xA070); immext(#0xA040) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }

l00020050:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; range_check: 0002005C
range_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0x9FD5); immext(#0x9FC0) }
	{ r17 = r0; r16 = r1; r1 = #0x4F }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x3)) jump:t 0002009C; r2 = memw(r17+16); r1 = #0x50 }

l00020088:
	{ r3 = add(PC,#0x4) }
	{ r2 = r16; call errlog_function }

l00020090:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l0002009C:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 000200B4; r2 = memw(r17+20); r1 = #0x51 }

l000200AC:
	{ jump 00020090; r3 = add(PC,#0x2F) }

l000200B4:
	{ r2 = r16; r1 = #0x52; r4 = add(PC,#0x9FB3); immext(#0x9F80) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 000200D4
;;   Called from:
;;     0001FFA8 (in range_execute)
;;     00020044 (in range_execute)
;;     00020070 (in range_check)
;;     000200C4 (in range_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000200F8; r5 = memw(r2+16) }

l000200E4:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x36) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l000200F8:
	{ dealloc_return }

;; errlog_function: 000200FC
;;   Called from:
;;     0001FFF8 (in range_execute)
;;     0002008C (in range_check)
;;     000200EC (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0x9F1A); immext(#0x9F00) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; transpose_execute: 00020120
transpose_execute proc
	{ allocframe(#0x90); r2 = add(PC,#0xA008); immext(#0xA000) }
	{ memd(r29+136) = r17:r16; r5 = add(r29,#0x40); r17:r16 = combine(r0,r1); r3 = add(r29,#0x30) }
	{ memd(r29+128) = r19:r18; r12 = memw(r17+4); r14 = setbit(r3,#0x4); r13 = setbit(r5,#0x4) }
	{ memd(r29+120) = r21:r20; r1 = #0x42 }
	{ memd(r29+112) = r23:r22 }
	{ memd(r29+96) = r27:r26; r18 = memw(r12) }
	{ r4 = memw(r17+8) }
	{ memd(r29+104) = r25:r24; r9:r8 = memd(r2) }
	{ r20 = memw(r18+8); r21 = memw(r18+12) }
	{ r22 = memw(r18); r23 = memw(r18+4) }
	{ memw(r29+56) = r20; memw(r29+48) = r22; r26 = mpyi(r21,r20) }
	{ memw(r14) = r23; r7:r6 = memd(r2+8); r3:r2 = combine(#0x2,r16) }
	{ memw(r13) = r26; r24 = memw(r4); r4 = add(PC,#0x9FB4); immext(#0x9F80) }
	{ memb(r29+16) = r27.new; r27 = mpyi(r26,r23) }
	{ memd(r29+80) = r9:r8 }
	{ memw(r29+72) = r21; r19 = memw(r24+16) }
	{ memd(r29+88) = r7:r6 }
	{ memw(r5+12) = #0x1; memw(r29+60) = r21 }
	{ memw(r29) = r17; call logmsg_function }
	{ memw(r29+12) = r21; r3:r2 = combine(#0x3,r16); r4 = add(PC,#0x9F98); immext(#0x9F80) }
	{ memw(r29) = r22; r1 = #0x47 }
	{ memw(r29+4) = r23; memw(r29+8) = r20 }
	{ call logmsg_function }
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 000201F4; r2 = memw(r25) }

l000201E8:
	{ if (!cmp.eq(r2.new,#0x1)) jump:t 000201F8 }

l000201F0:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 00020214 }

l000201F4:
	{ r1 = #0x4B; r3 = add(PC,#0x9F7E); immext(#0x9F40) }

l000201F8:
	{ r1 = #0x4B; r3 = add(PC,#0x3E) }

l00020200:
	{ r2 = r16; call errlog_function }

l00020204:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; jump 00020418 }
00020210 C2 C0 92 91                                     ....            

l00020214:
	{ if (!cmp.gtu(r2,r3.new)) jump:t 0002022C; r3 = memw(r24+20) }

l00020220:
	{ r1 = #0x4C; jump 00020204; r3 = add(PC,#0x1F) }

l0002022C:
	{ r3 = memw(r25+24) }
	{ if (cmp.gtu(r4.new,r3)) jump:t 00020248; r4 = #0x14 }

l0002023C:
	{ r1 = #0x4E; jump 00020204; r3 = add(PC,#0x11) }

l00020248:
	{ if (!cmp.eq(r5.new,#0x1)) jump:t 00020298; r1 = #0x52; r5 = lsr(r3,#0x2) }

l00020258:
	{ r3:r2 = combine(#0x3,r16); r4 = add(PC,#0xB) }
	{ call logmsg_function }
	{ r3 = memw(r18+4); r2 = memw(r18) }
	{ memw(r24) = r2; memw(r24+4) = r3 }
	{ r0 = memw(r24+16); r2 = memw(r18+8) }
	{ memw(r24+8) = r2 }
	{ r7 = memw(r18+12) }
	{ memw(r24+12) = r7 }
	{ r2 = memw(r18+24) }
	{ memw(r24+24) = r2 }
	{ r2 = memw(r18+24); r1 = memw(r18+16); call fn00009560 }
	{ jump 00020414 }

l00020298:
	{ if (!p0.new) r4 = memw(r25+16); r13:r12 = combine(r21,#0x1); if (!p0.new) jump:nt 000202B0; p0 = cmp.eq(r5,#0x0) }

l000202A4:
	{ r7:r6 = combine(#0x1,#0x2); r5 = #0x0; r8 = #0x3 }
	{ jump 00020320 }

l000202B0:
	{ r6 = r5; r3 = add(r29,#0x50); p0 = cmp.gtu(r5,#0x1); r2 = sub(#0x4,r5) }
	{ r3 = addasl(r3,r2,#0x2); loop0(000202CC,r6) }
	{ r5 = memw(r4++#4); if (!p0) jump:nt 000202D8 }

l000202CC:
	{ r5 = add(r5,r2) }
	{ memw(r3++#4) = r5; r5 = memw(r4++#4) }

l000202D8:
	{ r9 = add(r29,#0x30); r2 = add(r5,r2) }
	{ memw(r3++#4) = r2; r2 = add(r29,#0x50) }
	{ r6 = memd(r29+88); r5 = memd(r29+80); r3 = add(r29,#0x40); r4 = setbit(r2,#0x4) }
	{ r2 = memw(r18+24); r8 = memw(r29+92) }
	{ r13 = memw(r30+r6<<#2); r7 = memw(r4) }
	{ r22 = memw(r30+r5<<#2); r27 = memw(r22+r5<<#2) }
	{ r21 = memw(r30+r8<<#2); r20 = memw(r22+r6<<#2) }
	{ r23 = memw(r30+r7<<#2); r12 = memw(r22+r8<<#2) }
	{ r26 = memw(r30+r7<<#2) }

l00020320:
	{ memw(r24+12) = r21; memw(r24+24) = r2; r4 = add(PC,#0x9EAD); immext(#0x9E80) }
	{ memw(r24+4) = r23; memw(r24+8) = r20; r1 = #0x89; r3:r2 = combine(#0x3,r16) }
	{ memw(r29+44) = r21; memw(r24) = r22; r25:r24 = combine(r13,r12) }
	{ memw(r29+36) = r23; memw(r29+40) = r20 }
	{ memw(r29+8) = r6; memw(r29+32) = r22 }
	{ memw(r29+24) = r13; memw(r29+28) = r12 }
	{ memw(r29+16) = r27; memw(r29+20) = r26 }
	{ memw(r29+4) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; call logmsg_function }
	{ r3:r2 = combine(#0x0,#0x0); if (!p0.new) jump:nt 000203FC; p0 = cmp.gt(r14,#0x0); r1:r0 = vaslw(r25:r24,#0x2) }

l00020380:
	{ r5:r4 = vaslw(r27:r26,#0x2) }

l00020384:
	{ r7:r6 = combine(#0x0,r2); if (!p0.new) jump:nt 000203F0; p0 = cmp.gt(r15,#0x0) }

l00020388:
	{ r7:r6 = combine(#0x0,r2) }

l0002038C:
	{ r8 = r6; if (!p0.new) jump:nt 000203E4; p0 = cmp.gt(r12,#0x0); loop1(00020398,r20) }

l00020398:
	{ r9 = r8; r12 = r19; if (!p0.new) jump:nt 000203D8; p0 = cmp.gt(r13,#0x0) }

l000203A4:
	{ loop0(000203C0,r21) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r13 = memw(r18+16) }
	{ r13 = memw(r28+r9); r9 = add(r9,r0) }
	{ memw(r12++#4) = r13; nop }
	{ r19 = addasl(r19,r21,#0x2) }

l000203D8:
	{ nop; nop; r8 = add(r8,r1) }

l000203E4:
	{ if (!cmp.eq(r7.new,r23)) jump:t 0002038C; r7 = add(r7,#0x1); r6 = add(r6,r4) }

l000203F0:
	{ if (!cmp.eq(r3.new,r22)) jump:t 00020384; r3 = add(r3,#0x1); r2 = add(r2,r5) }

l000203F4:
	{ if (!cmp.eq(r3.new,r22)) jump:t 00020388; r3 = add(r3,#0x1) }

l000203FC:
	{ r1 = #0x96; r3:r2 = combine(#0x2,r16); r4 = add(PC,#0x9E0F); immext(#0x9E00) }

l00020400:
	{ r1 = #0x96; r3:r2 = combine(#0x2,r16); r4 = add(PC,#0xF) }
	{ memw(r29) = r17; call logmsg_function }

l00020414:
	{ r0 = #0x0 }

l00020418:
	{ r19:r18 = memd(r29+128); r17:r16 = memd(r29+136) }
	{ r23:r22 = memd(r29+112); r21:r20 = memd(r29+120) }
	{ r27:r26 = memd(r29+96); r25:r24 = memd(r29+104) }
	{ dealloc_return }

;; transpose_check: 0002042C
transpose_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0x9CA5); immext(#0x9C80) }
	{ r17 = r0; r16 = r1; r1 = #0x9C }
	{ memw(r29) = r17; r3:r2 = combine(#0x2,r16); call logmsg_function }
	{ if (cmp.eq(r2.new,#0x3)) jump:t 00020470; r2 = memw(r17+16); r1 = #0x9D }

l0002045C:
	{ r3 = add(PC,#0x14) }
	{ r2 = r16; call errlog_function }

l00020464:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l00020470:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 0002048C; r2 = memw(r17+20); r1 = #0x9F }

l00020480:
	{ r1 = #0x9E; jump 00020464; r3 = add(PC,#0x3F) }

l0002048C:
	{ memw(r29) = r17; r3:r2 = combine(#0x2,r16); r4 = add(PC,#0x9C7F); immext(#0x9C40) }
	{ call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

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
	{ if (cmp.gtu(r3,r5.new)) jump:t 000204C8; r5 = memw(r2+16) }

l000204B8:
	{ r6 = add(r29,#0x10); r5 = add(r29,#0x10); r0 = add(PC,#0x3E) }
	{ memw(r29+4) = r6; call logv }

l000204C8:
	{ dealloc_return }

;; errlog_function: 000204CC
;;   Called from:
;;     00020200 (in transpose_execute)
;;     00020460 (in transpose_check)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0x9BE6); immext(#0x9BC0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; addn_execute: 000204F0
addn_execute proc
	{ allocframe(#0x30); memd(r29-16) = r17:r16; r4 = add(PC,#0x9D94); immext(#0x9D80) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+32) = r19:r18; r18 = memw(r17+4); r1 = #0x39 }
	{ r3 = memw(r17+8); r2 = r16 }
	{ memd(r29+16) = r23:r22; memd(r29+24) = r21:r20 }
	{ r21 = memw(r3); r20 = memw(r18) }
	{ memd(r29+8) = r25:r24 }
	{ r22 = memw(r20+12); r23 = memw(r20+8) }
	{ r25 = memw(r20); r24 = memw(r20+4) }
	{ memw(r29) = r17; r19 = memw(r21+16); call logmsg_function }
	{ r2 = memw(r17+16) }
	{ if (cmp.gtu(r3.new,r2)) jump:t 000205A0; r3 = #0x2 }

l0002053C:
	{ r4 = add(r18,#0x4) }

l00020540:
	{ if (!cmp.gtu(r2,r5.new)) jump:nt 000205A0; r5 = add(r5,#0x1); r4 = add(r4,#0x4) }

l00020550:
	{ r6 = memw(r4); r3 = add(PC,#0xF) }
	{ if (cmp.eq(r7.new,r25)) jump:t 00020564; r7 = memw(r6) }

l00020564:
	{ if (cmp.eq(r7.new,r24)) jump:t 00020578; r7 = memw(r6+4); r3 = add(PC,#0x9D37); immext(#0x9D00) }

l00020578:
	{ if (cmp.eq(r7.new,r23)) jump:t 0002058C; r7 = memw(r6+8); r3 = add(PC,#0x9D23); immext(#0x9D00) }

l0002058C:
	{ if (cmp.eq(r6.new,r22)) jump:t 00020540; r6 = memw(r6+12); r3 = add(PC,#0x9D0F); immext(#0x9D00) }

l000205A0:
	{ r5 = memw(r21+20); r2 = mpyi(r24,r25); r3 = add(PC,#0x9D0A); immext(#0x9D00) }
	{ r1 = #0x40; r2 = mpyi(r2,r23) }
	{ r4 = mpyi(r2,r22) }
	{ if (!cmp.gtu(r2.new,r5)) jump:t 000205D4; r2 = asl(r4,#0x2) }

l000205C8:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; jump 00020654 }

l000205D4:
	{ r3 = memw(r20); r5 = memw(r20+4); p0 = cmp.eq(r4,#0x0) }
	{ memw(r21) = r3; memw(r21+4) = r5; if (!p0) r3 = #0x0 }
	{ r6 = memw(r20+8) }
	{ memw(r21+8) = r6 }
	{ r7 = memw(r20+12) }
	{ memw(r21+24) = r2; memw(r21+12) = r7; if (p0) jump:nt 00020638 }

l000205F8:
	{ r2 = #0x0; immext(#0x0); loop1(00020604,r4) }
	{ r6 = memw(r17+16); r4 = r2 }
	{ if (!p0.new) r5:r4 = combine(r18,r2); if (p0.new) jump:nt 0002062C; p0 = cmp.eq(r6,#0x0) }

l00020610:
	{ loop0(00020614,r6) }
	{ r6 = memw(r5++#4) }
	{ r6 = memw(r6+16) }
	{ r6 = addasl(r6,r3,#0x2) }
	{ r6 = memw(r6) }
	{ nop; r4 = sfadd(r4,r6) }

l0002062C:
	{ memw(r19) = r4; r3 = r3; nop; r19 = add(r19,#0x4) }

l00020638:
	{ r2 = r16; r1 = #0x4C; r4 = add(PC,#0x9C80); immext(#0x9C80) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }

l00020654:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ dealloc_return; r25:r24 = memd(r29+8) }

;; addn_check: 00020664
addn_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0x9BD5); immext(#0x9BC0) }
	{ r17 = r0; r16 = r1; r1 = #0x52 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.gtu(r2.new,#0x1)) jump:t 000206A4; r2 = memw(r17+16); r1 = #0x53 }

l00020690:
	{ r3 = add(PC,#0x3) }
	{ r2 = r16; call errlog_function }

l00020698:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l000206A4:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 000206BC; r2 = memw(r17+20); r1 = #0x54 }

l000206B4:
	{ jump 00020698; r3 = add(PC,#0x2E) }

l000206BC:
	{ r2 = r16; r1 = #0x55; r4 = add(PC,#0x9BB2); immext(#0x9B80) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 000206DC
;;   Called from:
;;     00020524 (in addn_execute)
;;     00020648 (in addn_execute)
;;     00020678 (in addn_check)
;;     000206CC (in addn_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00020700; r5 = memw(r2+16) }

l000206EC:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x35) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l00020700:
	{ dealloc_return }

;; errlog_function: 00020704
;;   Called from:
;;     00020694 (in addn_check)
;;     000206F4 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0x9B19); immext(#0x9B00) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
00020728                         00 00 00 00 00 00 00 00         ........
00020730 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; execute_qinstancenorm_ref: 00020740
execute_qinstancenorm_ref proc
	{ allocframe(#0x70) }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+72) = r25:r24; memd(r29+96) = r19:r18 }
	{ r4 = memw(r2) }
	{ memd(r29+104) = r17:r16; memd(r29+64) = r27:r26; r17 = r1 }
	{ memd(r29+88) = r21:r20; r25 = memw(r4+8) }
	{ r24 = memw(r4+4); r19 = memw(r4) }
	{ r27 = memw(r4+12); r5 = mpyi(r25,r19) }
	{ memd(r29+80) = r23:r22; r16 = memw(r17+4); r2 += mpyi(r27,#0x14) }
	{ r21 = memw(r3+8); r18 = memw(r3); r1:r0 = combine(#0x0,r16) }
	{ memw(r29+52) = r25; r22 = memw(r3+4); r3 = mpyi(r5,r24) }
	{ r23 = memw(r18+16); r26 = memw(r4+16) }
	{ call fn000095F0; r20 = mpyi(r3,r27) }
	{ if (!cmp.gtu(r20,r2.new)) jump:t 000207C8; r2 = memw(r18+20); r1 = #0x71 }

l000207B4:
	{ r3 = add(PC,#0x16) }

l000207B8:
	{ r2 = r17; call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 00020AA8 }

l000207C8:
	{ memw(r29+40) = r26; if (!p0.new) r1 = #0x72; if (p0.new) jump:nt 000207E0; p0 = cmp.eq(r11,#0x1) }

l000207D4:
	{ jump 000207B8; r3 = add(PC,#0x9B40); immext(#0x9B40) }

l000207E0:
	{ memw(r18+8) = r24; memw(r18+12) = r27; r2 = r17 }
	{ memw(r18+4) = r25 }
	{ memw(r29+12) = r27; memw(r18) = #0x1 }
	{ memw(r29+4) = r25; memw(r29+8) = r24 }
	{ memw(r29) = #0x1; call logmsg_function }
	{ memw(r18+24) = r20; r12 = r27; p0 = cmp.gt(r24,#0x0) }
	{ memb(r29+12) = r0.new; if (!p0) jump:nt 0002087C; r0 = p0 }

l00020824:
	{ r2 = memd(r29+40); r4 = #0x0 }

l00020828:
	{ p0 = cmp.gt(r25,#0x0); if (!p0.new) jump:nt 00020874; loop1(00020834,r25) }

l00020834:
	{ if (!p0) r7:r6 = combine(r2,r16); p0 = cmp.gt(r12,#0x0); r5 = r3; if (!p0.new) jump:nt 00020868 }

l00020844:
	{ loop0(00020848,r12) }
	{ r9 = memw(r6); r8 = memb(r7++#1) }
	{ r9 = add(r9,r8) }
	{ memw(r6++#4) = r9 }
	{ r13 = memw(r5) }
	{ memw(r5++#4) = r8.new; r8 = add(r13,mpyi(r8,r8)) }

l00020868:
	{ nop; nop; nop }

l00020874:
	{ if (!cmp.eq(r4.new,r24)) jump:t 00020828; r4 = add(r4,#0x1) }

l0002087C:
	{ memw(r29+36) = r24; r24 = r12; p0 = cmp.gt(r12,#0x0); r2 = mpyi(r24,r25) }

l00020880:
	{ memw(r29+36) = r24; r24 = r12; p0 = cmp.gt(r12,#0x0) }
	{ memw(r29+24) = r21; memw(r29+32) = r23; if (p0) r19 = add(r24,#0x0); r0 = p0 }
	{ memw(r29+28) = r2; memw(r29+20) = r22; if (p0) r26 = #0x3727C5AC; immext(#0x3727C580) }
	{ memw(r29+56) = r0; if (p0) r18 = #0x3F800000; immext(#0x3F800000); if (!p0) jump:nt 00020920 }

l000208B4:
	{ r2 = memd(r29+28); r20 = r16; r22 = asl(r24,#0x3); r21 += mpyi(r24,#0xC) }
	{ r17 = convert_w2sf(r2); r27 = addasl(r16,r24,#0x2) }

l000208C8:
	{ r2 = memw(r20); r1 = r17 }
	{ call fn00009610; r0 = convert_uw2sf(r2) }
	{ r23 = add(r20,r21); r1 = r17; r2 = add(r20,r22) }
	{ memw(r2) = r0; r2 = togglebit(r0,#0x1E) }
	{ r4 = memw(r20); r3 = memw(r27++#4) }
	{ r3 = convert_uw2sf(r4); r0 = convert_uw2sf(r3) }
	{ call fn00009610; r0 += sfmpy(r2,r3) }
	{ r2 = sfadd(r0,r26) }
	{ memw(r23) = r0; r0 = r2; call fn00009800 }
	{ r19 = r19; r20 = add(r20,#0x4); r1:r0 = combine(r0,r18); call fn00009610 }
	{ memw(r23) = r0; if (!p0.new) jump:nt 000208C8; p0 = cmp.eq(r11,#0x0) }

l00020920:
	{ r0 = memw(r29+48); r4 = r24 }
	{ p0 = r0; r27 = addasl(r16,r4,#0x4) }
	{ if (p0) r18 = #0x0; immext(#0x0); if (p0) jump:nt 00020948 }

l0002093C:
	{ r18 = #0x0; immext(#0x0) }
	{ jump 00020A04; r9 = r10 }

l00020948:
	{ r3 = asl(r4,#0x1); r2 += mpyi(r4,#0x3) }
	{ memb(r29+12) = r3.new; r17 = r18; r5 = #0x0; r3 = addasl(r16,r3,#0x2) }
	{ r3 = memw(r29+40) }
	{ memw(r29+44) = r2 }

l0002096C:
	{ memw(r29+40) = r5; r21 = #0x0; p0 = cmp.gt(r25,#0x0) }
	{ if (!p0) jump:nt 000209F8 }

l00020978:
	{ r0 = memw(r29+56) }
	{ if (!p0) r25:r24 = combine(r3,r4); if (p0.new) r26 = add(r3,#0x0); if (!p0.new) jump:nt 000209F0; p0 = r0 }

l0002098C:
	{ r19 = memd(r29+48); r20 = memd(r29+44); r22 = r4; r23 = addasl(r27,r4,#0x2) }

l00020998:
	{ r3 = memw(r19); r2 = memb(r25++#1) }
	{ r4 = memw(r20); r2 = convert_w2sf(r2) }
	{ r2 = sfsub(r2,r3) }
	{ memb(r27) = r16.new; r27 = add(r27,#0x4); r16 = sfmpy(r4,r2) }
	{ r1:r0 = combine(r18,r16) }
	{ r1:r0 = combine(r17,r16); r18 = r0; call fn00009600 }
	{ r17 = r0; r20 = add(r20,#0x4); r19 = add(r19,#0x4) }
	{ if (!cmp.eq(r22.new,#0x0)) jump:t 00020998; r22 = add(r22,#0xFFFFFFFF) }

l000209E0:
	{ r25 = memw(r29+52); r27 = r23; r4 = r24 }
	{ r3 = add(r3,r4) }

l000209F0:
	{ if (!cmp.eq(r21.new,r25)) jump:t 00020978; r21 = add(r21,#0x1) }

l000209F8:
	{ r2 = memd(r29+36); r5 = memd(r29+40) }

l000209FC:
	{ if (!cmp.eq(r5.new,r2)) jump:t 0002096C; r5 = add(r5,#0x1) }

l00020A04:
	{ r2 = memw(r29+28) }

l00020A08:
	{ if (!cmp.gt(r19.new,#0x0)) jump:nt 00020A74; r19 = mpyi(r2,r4) }

l00020A14:
	{ r0 = #0x38D1B717; immext(#0x38D1B700) }
	{ call fn00009600; r27 -= asl(r19,#0x2) }
	{ r2 = #0x437F0000; immext(#0x437F0000) }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r16 = r0; r20 = #0x0 }

l00020A38:
	{ r2 = memw(r27); r27 = add(r27,#0x4) }
	{ r2 = sfsub(r2,r18) }
	{ call fn00009620; r0 = sfmpy(r2,r16) }
	{ r3 = memd(r29+32); r19 = r19; r2 = convert_uw2sf(r0):chop }
	{ p1 = cmp.eq(r19,#0x0) }
	{ if (p2.new) r2 = #0xFFFFFFFF; p0 = cmp.gt(r2,#0xFFFFFFFF); p2 = cmp.gt(r2,#0xFF) }
	{ if (!p0) r2 = add(r20,#0x0) }
	{ memb(r3++#1) = r2 }
	{ memw(r29+32) = r3; if (!p1) jump:nt 00020A38 }

l00020A74:
	{ r3 = memd(r29+24); r4 = memd(r29+20); r0 = #0x0 }
	{ memw(r4+12) = #0xFFFFFF81 }
	{ memw(r4+4) = #0x1; memw(r4+8) = #0x1 }
	{ memw(r3+8) = #0x1; memw(r4) = #0x1 }
	{ memw(r3+12) = #0x1; memw(r3) = #0x1 }
	{ memw(r3+4) = #0xFFFFFF81 }
	{ r2 = memw(r4+16) }
	{ memw(r2) = r18 }
	{ r7 = memw(r3+16) }
	{ memw(r4+24) = #0x4; memw(r7) = r17 }
	{ memw(r3+24) = #0x4 }

l00020AA8:
	{ r19:r18 = memd(r29+96); r17:r16 = memd(r29+104) }
	{ r23:r22 = memd(r29+80); r21:r20 = memd(r29+88) }
	{ r27:r26 = memd(r29+64); r25:r24 = memd(r29+72) }
	{ dealloc_return }

;; check_qinstancenorm: 00020ABC
check_qinstancenorm proc
	{ allocframe(#0x0); r2 = r1 }
	{ if (cmp.eq(r3.new,#0x3)) jump:t 00020AD8; r3 = memw(r0+16); r1 = #0xFC }

l00020AD0:
	{ jump 00020AF0; r3 = add(PC,#0x1B) }

l00020AD8:
	{ r3 = memw(r0+20); r0 = #0x0; r1 = #0xFD }
	{ if (p0.new) dealloc_return; p0 = cmp.eq(r3,#0x3) }
00020AE4             60 42 00 00 03 C9 49 6A 6C C1 00 5A     `B....Ijl..Z

l00020AF0:
	{ r0 = #0xFFFFFFFF }
	{ dealloc_return }

;; execute_finstancenorm: 00020AF8
execute_finstancenorm proc
	{ allocframe(#0x68); r7 = r1 }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+72) = r23:r22; memd(r29+80) = r21:r20 }
	{ r2 = memw(r2) }
	{ memd(r29+64) = r25:r24; r6 = memw(r3) }
	{ memd(r29+96) = r17:r16; r5 = memw(r2) }
	{ r23 = memw(r2+4); r21 = memw(r2+8); p0 = cmp.gt(r5,#0x0) }
	{ r24 = memw(r2+12); r4 = mpyi(r5,r21) }
	{ memd(r29+56) = r27:r26; memd(r29+88) = r19:r18; r4 = mpyi(r4,r23) }
	{ memw(r29+20) = r23; memw(r29+28) = r5 }
	{ memw(r29+24) = r7 }
	{ r4 = memw(r6+20); r0 = mpyi(r4,r24) }
	{ if (!cmp.gtu(r3.new,r4)) jump:t 00020B74; r3 = asl(r0,#0x2) }

l00020B4C:
	{ r2 = r7; r1 = #0xD6; r3 = add(PC,#0x3E) }
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF }

l00020B60:
	{ r19:r18 = memd(r29+88); r17:r16 = memd(r29+96) }
	{ r23:r22 = memd(r29+72); r21:r20 = memd(r29+80) }
	{ r27:r26 = memd(r29+56); r25:r24 = memd(r29+64) }
	{ dealloc_return }

l00020B74:
	{ r2 = memw(r2+16); r16 = memw(r7+4); r0 = #0x0 }
	{ memw(r6+8) = r23; memw(r29+16) = r2 }
	{ memw(r6) = r5; memw(r6+24) = r3 }
	{ memw(r6+12) = r24; memw(r6+4) = r21 }
	{ if (!p0) jump:nt 00020B60 }

l00020B94:
	{ r3 += mpyi(r24,#0x3); r2 = mpyi(r23,r21) }
	{ memw(r29) = r6; r18 = #0x0; r4 = asl(r24,#0x1); r5 += mpyi(r24,#0xC) }
	{ memw(r29+44) = r5; r2 = convert_w2sf(r2); r6 = mpyi(r2,r24) }
	{ memw(r29+8) = r6; memw(r29+4) = r2; r27 = asl(r24,#0x2); r17 = addasl(r16,r4,#0x2) }
	{ r19 = memw(r29+4); r5 = asl(r24,#0x3); r22 = addasl(r16,r3,#0x2) }
	{ memw(r29+48) = r5; r1 += mpyi(r24,#0x14) }
	{ memw(r29+12) = r1 }

l00020BD8:
	{ r0 = memw(r7+4); r1 = #0x0; p0 = cmp.gt(r23,#0x0) }
	{ memw(r29+36) = r18; r2 = memd(r29+12); r3 = p0 }
	{ memw(r29+40) = r3; call fn000095F0 }
	{ r2 = memw(r29+8) }
	{ r0 = memd(r29+40); r4 = memd(r29+16); r3 = mpyi(r2,r18) }
	{ p0 = r0 }
	{ r18 = addasl(r4,r3,#0x2) }
	{ r3:r2 = combine(r18,#0x0); if (p0) jump:nt 00020D30 }

l00020C0C:
	{ r26 = r16; r25 = r24; p0 = cmp.gt(r24,#0x0) }
	{ r23 = #0x3727C5AC; immext(#0x3727C580) }
	{ memb(r29+8) = r0.new; if (!p0) jump:nt 00020C90; r0 = p0 }

l00020C2C:
	{ r0 = memw(r26); r1 = r19; call fn00009610 }

l00020C30:
	{ r0 = memw(r26); r1 = r19 }

l00020C38:
	{ r2 = memd(r29+48); r20 = r0; r1 = r19; r3 = add(r26,r27) }
	{ r2 = add(r26,r2) }
	{ memw(r2) = r20 }
	{ r0 = memw(r3); call fn00009610 }
	{ r7 = memw(r29+44); r2 = togglebit(r20,#0x1E) }
	{ r20 = add(r26,r7); r0 += sfmpy(r2,r20) }
	{ memw(r20) = r0; r0 = sfadd(r0,r23) }
	{ call fn00009800 }
	{ r1 = r0; r0 = #0x3F800000; immext(#0x3F800000); call fn00009610 }
	{ memw(r20) = r0; r26 = add(r26,#0x4); r25 = add(r25,#0xFFFFFFFF) }
	{ p0 = cmp.eq(r25,#0x0); if (!p0.new) jump:nt 00020C2C }

l00020C90:
	{ r14 = memw(r29+28); r0 = memw(r29+32) }
	{ r23 = memw(r29+20) }
	{ r0 = memw(r29+40); p1 = r0 }
	{ if (p0.new) r2 = memw(r29); if (!p0.new) jump:nt 00020D20; p0 = r0 }

l00020CB0:
	{ r3 = memw(r2+16); r2 = #0x0 }

l00020CB4:
	{ r4 = #0x0; if (!p0.new) jump:nt 00020D18; p0 = cmp.gt(r13,#0x0) }

l00020CB8:
	{ r4 = #0x0 }

l00020CBC:
	{ if (p1) r7:r6 = combine(r18,r17); if (p1) r8 = add(r22,#0x0); if (!p1) jump:nt 00020D10 }

l00020CC8:
	{ r5 = addasl(r3,r24,#0x2); loop0(00020CE0,r24) }
	{ nop; nop; nop; nop }
	{ r12 = memw(r6); r9 = memw(r7); r7 = add(r7,#0x4); r6 = add(r6,#0x4) }
	{ r13 = memw(r8); r8 = add(r8,#0x4); r9 = sfsub(r9,r12) }
	{ r9 = sfmpy(r9,r13) }
	{ memw(r3) = r9; r3 = add(r3,#0x4) }
	{ r3 = r5; r18 = addasl(r18,r24,#0x2) }

l00020D10:
	{ if (!cmp.eq(r4.new,r21)) jump:t 00020CBC; r4 = add(r4,#0x1) }

l00020D18:
	{ if (!cmp.eq(r2.new,r23)) jump:t 00020CB4; r2 = add(r2,#0x1) }

l00020D1C:
	{ if (!cmp.eq(r2.new,r23)) jump:t 00020CB8 }

l00020D20:
	{ r7 = memd(r29+24); r18 = memd(r29+36) }

l00020D24:
	{ if (!cmp.eq(r18.new,r14)) jump:t 00020BD8; r18 = add(r18,#0x1) }

l00020D30:
	{ if (!p0.new) jump:nt 00020D7C; p0 = cmp.gt(r13,#0x0); loop1(00020D38,r21) }

l00020D38:
	{ if (!p0) r5:r4 = combine(r16,r3); p0 = cmp.gt(r24,#0x0); if (!p0.new) jump:nt 00020D70 }

l00020D44:
	{ loop0(00020D48,r24) }
	{ r7 = memw(r5); r6 = memw(r4); r4 = add(r4,#0x4); r8 = add(r5,r27) }
	{ r7 = sfadd(r6,r7) }
	{ memw(r5) = r7; r5 = add(r5,#0x4) }
	{ r1 = memw(r8) }
	{ r1 += sfmpy(r6,r6) }
	{ memw(r8) = r1; nop }
	{ r3 = addasl(r3,r24,#0x2) }

l00020D70:
	{ nop; nop; nop }

l00020D7C:
	{ if (cmp.eq(r2.new,r23)) jump:nt 00020C0C; r2 = add(r2,#0x1) }

;; check_finstancenorm: 00020D88
;;   Called from:
;;     00020D7C (in execute_finstancenorm)
check_finstancenorm proc
	{ allocframe(#0x0); r2 = r1 }
	{ if (cmp.eq(r3.new,#0x1)) jump:t 00020DA4; r3 = memw(r0+16); r1 = #0x103 }

l00020D9C:
	{ jump 00020DBC; r3 = add(PC,#0xF) }

l00020DA4:
	{ r3 = memw(r0+20); r0 = #0x0; r1 = #0x104 }
	{ if (p0.new) dealloc_return; p0 = cmp.eq(r3,#0x1) }
00020DB0 55 42 00 00 03 C3 49 6A 06 C0 00 5A             UB....Ij...Z    

l00020DBC:
	{ r0 = #0xFFFFFFFF }
	{ dealloc_return }

;; errlog_function: 00020DC4
;;   Called from:
;;     000207B8 (in execute_qinstancenorm_ref)
;;     00020B58 (in execute_finstancenorm)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0x9501); immext(#0x9500) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }

;; logmsg_function: 00020DE8
;;   Called from:
;;     00020800 (in execute_qinstancenorm_ref)
logmsg_function proc
	{ allocframe(#0x8); r4 = #0x2 }
	{ if (cmp.gtu(r4,r3.new)) jump:t 00020E18; r3 = memw(r2+16) }

l00020DF8:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x11) }
	{ r6 = add(r29,#0x10); r1 = #0x74; r4 = add(PC,#0x9534); immext(#0x9500) }
	{ memw(r29+4) = r6; call logv }

l00020E18:
	{ dealloc_return }
00020E1C                                     00 00 00 00             ....

;; sub_int32_execute: 00020E20
sub_int32_execute proc
	{ allocframe(#0xA0); r5 = r0 }
	{ r3 = memw(r5+8); r2 = memw(r5+4) }
	{ memd(r29+144) = r19:r18; memd(r29+112) = r27:r26 }
	{ r19 = memw(r2+4); r26 = memw(r2) }
	{ memd(r29+128) = r23:r22; memd(r29+136) = r21:r20; r22 = #0x0 }
	{ r0 = memw(r26); r6 = memw(r26+4) }
	{ r4 = memw(r19+4); r8 = memw(r19); p1 = cmp.eq(r0,#0x1); p0 = cmp.eq(r6,#0x1) }
	{ r12 = memw(r26+8); r10 = p1 }
	{ memw(r29+92) = r4; r9 = memw(r19+8); p2 = cmp.eq(r12,#0x1); r18 = mux(p0,r4,r6) }
	{ r7 = memw(r26+12); r4 = mux(p1,r8,r0) }
	{ r0 = memw(r19+12); p1 = cmp.eq(r7,#0x1); r2 = mpyi(r4,r18) }
	{ memw(r29+76) = r6; memd(r29+120) = r25:r24; r20 = mux(p2,r9,r12) }
	{ r21 = mux(p1,r0,r7); r2 = mpyi(r2,r20) }
	{ memd(r29+152) = r17:r16; r25 = memw(r3); r6 = r1; r2 = mpyi(r2,r21) }
	{ memw(r29+72) = r8; memw(r29+96) = r10; r10 = p2 }
	{ memw(r29+84) = r10; memw(r29+68) = r9 }
	{ memw(r29+80) = r0; memw(r29+104) = r4; r16 = asl(r2,#0x2) }
	{ if (p0) jump:nt 00020ECC }

l00020EC8:
	{ r22 = mpyi(r7,r12) }

l00020ECC:
	{ r2 = r6; r1 = #0xBD; r0 = add(PC,#0x94DD); immext(#0x94C0) }
	{ memw(r29+64) = r12; r3 = memw(r19+16); r4 = add(PC,#0x94EC); immext(#0x94C0) }
	{ memw(r29+88) = r7; r23 = memw(r25+16); r27 = r5; r24 = r6 }
	{ r17 = memw(r26+16) }
	{ memw(r29) = r5; memw(r29+100) = r3; call logmsg_function }
	{ if (!cmp.gtu(r16,r2.new)) jump:t 00020F34; r2 = memw(r25+20); r1 = #0xBD }

l00020F18:
	{ r2 = r24; r0 = add(PC,#0x15) }
	{ r3 = add(PC,#0x94C6); immext(#0x94C0) }

l00020F28:
	{ call errlog_function }
	{ r0 = #0xFFFFFFFF; jump 00021160 }

l00020F34:
	{ r5 = memw(r26); r13 = memw(r19); r2 = r24 }
	{ r7 = memw(r26+8); r8 = memw(r26+12); p0 = cmp.eq(r5,r13) }
	{ r12 = memw(r19+12); r6 = memw(r26+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+52) = r16; memw(r29+48) = r25 }
	{ memw(r29+60) = r27; memw(r29+56) = r17 }
	{ if (p0) jump:nt 00020F7C }

l00020F70:
	{ if (p0.new) jump:nt 00020F7C; p0 = cmp.eq(r5,#0x1) }

l00020F74:
	{ p0 = cmp.eq(r13,#0x1); if (!p0.new) jump:nt 00020FB8 }

l00020F7C:
	{ if (p0.new) jump:nt 00020F8C; p0 = cmp.eq(r6,r3) }

l00020F80:
	{ if (p0.new) jump:nt 00020F8C; p0 = cmp.eq(r6,#0x1) }

l00020F84:
	{ nop; if (!p0.new) jump:nt 00020FB8; p0 = cmp.eq(r3,#0x1) }

l00020F8C:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 00020FA0 }

l00020F94:
	{ if (p0.new) jump:nt 00020FA0; p0 = cmp.eq(r7,#0x1) }

l00020F98:
	{ p0 = cmp.eq(r9,#0x1); if (!p0.new) jump:nt 00020FB8 }

l00020FA0:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 00020FEC }

l00020FA8:
	{ p0 = cmp.eq(r8,#0x1); if (p0.new) jump:nt 00020FEC }

l00020FB0:
	{ p0 = cmp.eq(r12,#0x1); if (p0.new) jump:nt 00020FEC }

l00020FB8:
	{ memw(r29+28) = r12; r1 = #0xBD; r0 = add(PC,#0x93F1); immext(#0x93C0) }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+20) = r3; r3 = add(PC,#0x9424); immext(#0x9400) }
	{ memw(r29+4) = r6; memw(r29+12) = r8 }
	{ memw(r29) = r5; jump 00020F28 }

l00020FEC:
	{ memw(r29+44) = r21; r24 = memw(r29+104); r19 = r2 }
	{ memw(r29+20) = r3; memw(r29+36) = r18; r0 = add(PC,#0x93B1); immext(#0x9380) }
	{ memw(r29+40) = r20; r1 = #0xBD; r4 = add(PC,#0x9420); immext(#0x9400) }
	{ memw(r29+28) = r12; memw(r29+32) = r24 }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; memw(r29+4) = r6 }
	{ call logmsg_function }
	{ r26 = memw(r29+100); r3 = memw(r29+48); if (p0.new) r14 = #0x0; p0 = cmp.gt(r24,#0x0) }
	{ r25 = memw(r29+56) }
	{ memb(r3+6) = r2.new; r2 = memw(r29+52) }
	{ memw(r3+4) = r18 }
	{ memw(r3+12) = r21; memw(r3+8) = r20 }
	{ if (p0) r12 = memw(r29+88); if (p0) r13 = memw(r29+80); if (!p0) jump:nt 00021138 }

l00021068:
	{ r7 = memd(r29+92); r5 = memd(r29+68); r6 = !cmp.eq(r13,00000001) }
	{ r2 = memd(r29+76); r0 = memd(r29+84); r9 = mpyi(r5,r7) }
	{ r4 = memd(r29+72); r3 = memd(r29+64); p0 = cmp.eq(r5,#0x1); p2 = r0 }
	{ r0 = memw(r29+96); p1 = cmp.eq(r4,#0x1); r9 = mpyi(r9,r13); r8 = mpyi(r3,r2) }
	{ if (p1) r9 = add(r14,#0x0); p2 = cmp.eq(r7,#0x1); r2 = mux(p2,#0x0,r12); r4 = mpyi(r13,r5) }
	{ r5 = !cmp.eq(r12,00000001); r3 = mux(p0,#0x0,r13); p0 = r0; r8 = mpyi(r8,r12) }
	{ if (p0) r8 = add(r14,#0x0); if (p2) r4 = add(r14,#0x0); r7 = #0x0 }

l000210C0:
	{ r14 = #0x0; r13:r12 = combine(r25,r26); if (!p0.new) jump:nt 00021128; p0 = cmp.gt(r10,#0x0) }

l000210C4:
	{ r14 = #0x0; r13:r12 = combine(r25,r26) }

l000210CC:
	{ r15 = r13; r28 = r12; if (!p0.new) jump:nt 00021118; p0 = cmp.gt(r12,#0x0) }

l000210D8:
	{ loop1(000210DC,r20) }
	{ if (!p0) r1:r0 = combine(r15,r28); r10 = r23; if (!p0.new) jump:nt 0002110C; p0 = cmp.gt(r13,#0x0) }

l000210E8:
	{ loop0(000210EC,r21) }
	{ r16 = memw(r0); r11 = memw(r1); r1 = addasl(r1,r5,#0x2); r0 = addasl(r0,r6,#0x2) }
	{ r11 = sub(r11,r16) }
	{ memw(r10++#4) = r11; nop }
	{ r23 = addasl(r23,r21,#0x2) }

l0002110C:
	{ nop; r15 = addasl(r15,r2,#0x2); r28 = addasl(r28,r3,#0x2) }

l00021118:
	{ if (!cmp.eq(r14.new,r18)) jump:t 000210CC; r14 = add(r14,#0x1); r12 = addasl(r12,r4,#0x2); r13 = addasl(r13,r22,#0x2) }

l00021128:
	{ if (!cmp.eq(r7.new,r24)) jump:t 000210C0; r7 = add(r7,#0x1); r26 = addasl(r26,r9,#0x2); r25 = addasl(r25,r8,#0x2) }

l0002112C:
	{ if (!cmp.eq(r7.new,r24)) jump:t 000210C4; r7 = add(r7,#0x1); r26 = addasl(r26,r9,#0x2) }

l00021138:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,#0x9271); immext(#0x9240) }

l0002113C:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,#0x31) }

l0002114C:
	{ r2 = r19; r1 = #0xBD; r4 = add(PC,#0xC) }
	{ call logmsg_function }
	{ r0 = #0x0 }

l00021160:
	{ r19:r18 = memd(r29+144); r17:r16 = memd(r29+152) }
	{ r23:r22 = memd(r29+128); r21:r20 = memd(r29+136) }
	{ r27:r26 = memd(r29+112); r25:r24 = memd(r29+120) }
	{ dealloc_return }

;; sub_int32_check: 00021174
sub_int32_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0x91FA); immext(#0x91C0) }
	{ r16 = r1; r1 = #0x37; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,#0x91CD); immext(#0x91C0) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,#0x2)) jump:t 000211C4; r2 = memw(r17+16); r1 = #0x38 }

l000211A8:
	{ r0 = add(PC,#0x31) }
	{ r3 = add(PC,#0x91CE); immext(#0x91C0) }

l000211B4:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l000211C4:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 000211E4; r2 = memw(r17+20); r1 = #0x39 }

l000211D4:
	{ r0 = add(PC,#0x5) }
	{ jump 000211B4; r3 = add(PC,#0x91B1); immext(#0x9180) }

l000211E4:
	{ r2 = r16; r1 = #0x3A; r0 = add(PC,#0x9171); immext(#0x9140) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,#0x91A9); immext(#0x9180) }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00021208
;;   Called from:
;;     00020F00 (in sub_int32_execute)
;;     00021030 (in sub_int32_execute)
;;     00021158 (in sub_int32_execute)
;;     00021194 (in sub_int32_check)
;;     000211F0 (in sub_int32_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00021228; r5 = memw(r2+16) }

l00021218:
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10) }

l00021228:
	{ dealloc_return }

;; errlog_function: 0002122C
;;   Called from:
;;     00020F28 (in sub_int32_execute)
;;     000211B4 (in sub_int32_check)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r3 = #0x0 }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; prelu_execute: 00021250
;;   Called from:
;;     0002124C (in errlog_function)
prelu_execute proc
	{ allocframe(#0x60); memd(r29-16) = r17:r16; r17:r16 = combine(r1,r0) }
	{ memd(r29+56) = r25:r24; r2 = memw(r16+4); r0 = #0x38D1B717; immext(#0x38D1B700) }
	{ r3 = memw(r16+8) }
	{ memd(r29+72) = r21:r20; memd(r29+80) = r19:r18 }
	{ r5 = memw(r3+8); r24 = memw(r2) }
	{ r3 = memw(r3+4); r25 = memw(r3) }
	{ r21 = memw(r2+4); r18 = memw(r2+8) }
	{ memw(r29+28) = r3; r3 = memw(r24+4) }
	{ memw(r29+36) = r3; r3 = memw(r24+12) }
	{ r4 = memw(r18+16) }

;; fn00021298: 00021298
;;   Called from:
;;     00021294 (in prelu_execute)
;;     000212EC (in fn00024D00)
;;     00024FD0 (in fn00024D00)
fn00021298 proc
	{ memw(r29+40) = r3; r3 = memw(r21+16) }
	{ r2 = memw(r2+12) }
	{ memd(r29+48) = r27:r26; r4 = memw(r4) }
	{ r2 = memw(r2+16); r26 = memw(r3) }
	{ memd(r29+64) = r23:r22; r7 = memw(r24+8) }
	{ r1 = sfsub(r4,r26) }
	{ memw(r29+44) = r7; r7 = memw(r24) }
	{ memw(r29+24) = r5 }
	{ memw(r29+32) = r7; r22 = memw(r24+16) }
	{ r19 = memw(r2); r23 = memw(r25+16); call fn00009600 }
	{ r2 = #0x437F0000; immext(#0x437F0000) }
	{ r20 = #0x0; immext(#0x0); r1:r0 = combine(r0,r2); call fn00009610 }

l000212EC:
	{ r20 = #0x0; immext(#0x0) }

l000212F4:
	{ r2 = sfsub(r20,r26) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r3 = #0x47800000; immext(#0x47800000); r4 = add(PC,#0x91EE); immext(#0x91C0) }
	{ r2 = r17; r1 = #0x47; r27 = convert_uw2sf(r0):chop; r3 = sfmpy(r19,r3) }
	{ memw(r29) = r16 }
	{ call logmsg_function; r26 = convert_sf2uw(r3) }
	{ r3 = memw(r21+16); r2 = memw(r18+16); r4 = add(PC,#0x91DA); immext(#0x91C0) }
	{ r3 = memw(r3); r2 = memw(r2); r1 = #0x4A }
	{ r2 = r17; r9:r8 = convert_sf2df(r3); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r2 = #0x3F800000; immext(#0x3F800000) }
	{ if (p0.new) r1 = #0x4B; if (!p0.new) jump:nt 00021384; p0 = sfcmp.gt(r19,r2) }

l0002136C:
	{ r3 = add(PC,#0x91B2); immext(#0x9180) }

l00021374:
	{ r2 = r17; call errlog_function }

l00021378:
	{ r2 = r17 }
	{ r0 = #0xFFFFFFFF; jump 00021558 }

l00021384:
	{ p1 = cmp.gt(r27,#0xFFFFFFFF); r1 = #0x4E; if (!p0.new) jump:nt 000213A4; p0 = sfcmp.gt(r20,r19) }

l00021394:
	{ r1 = #0x4C; jump 00021374; r3 = add(PC,#0x91A0); immext(#0x9180) }

l000213A4:
	{ r3 = memd(r29+32); r2 = memd(r29+36); p0 = cmp.gt(r27,#0xFF); r7:r6 = convert_sf2df(r19) }
	{ r5 = memd(r29+40); r0 = memd(r29+44); if (p0) r27 = #0xFFFFFFFF; r3 = mpyi(r2,r3) }
	{ if (!p1) r27 = #0x0; r3 = mpyi(r3,r0); r4 = add(PC,#0x918E); immext(#0x9180) }
	{ memd(r29) = r7:r6; memw(r29+8) = r26; r20 = and(r27,#0xFF) }
	{ r27 = r20; r2 = add(#0x8000,mpyi(r20,r26)); immext(#0x8000) }
	{ r2 = r17; r27 -= lsr(r2,#0x10) }
	{ memw(r29+65548) = #0xFFFFFF80; immext(#0x10000) }
	{ call logmsg_function }
	{ if (!cmp.gtu(r19,r2.new)) jump:t 0002141C; r2 = memw(r25+20); p0 = cmp.eq(r19,#0x0) }

l00021410:
	{ r1 = #0x50; jump 00021378; r3 = add(PC,#0x29) }

l0002141C:
	{ r3 = memw(r24+4); r2 = memw(r24) }
	{ memw(r25) = r2; memw(r25+4) = r3 }
	{ r6 = memw(r24+8) }
	{ memw(r25+8) = r6 }
	{ r7 = memw(r24+12); if (!p0) r24 = #0x0 }
	{ memw(r25+24) = r19; memw(r25+12) = r7; if (p0) jump:nt 0002149C }

l00021448:
	{ memb(r23) = r2.new; r2 = memb(r22); r3 = r27 }

l0002144C:
	{ memb(r23) = r2.new; r2 = memb(r22) }
	{ if (!cmp.gtu(r20,r2.new)) jump:t 00021490; r2 = memb(r22) }

l00021464:
	{ r2 = add(#0x8000,mpyi(r2,r26)); immext(#0x8000); r4 = add(PC,#0x23) }
	{ r2 = r17; r3 += lsr(r2,#0x10) }
	{ memw(r29+4) = r5; memw(r29+8) = r3 }
	{ memw(r29) = r24; call logmsg_function }
	{ if (!cmp.eq(r24.new,r19)) jump:t 00021448; r24 = add(r24,#0x1); r22 = add(r22,#0x1); r23 = add(r23,#0x1) }

l00021490:
	{ if (!cmp.eq(r24.new,r19)) jump:t 0002144C; r24 = add(r24,#0x1); r22 = add(r22,#0x1) }

l0002149C:
	{ r20 = memd(r29+28); r2 = memw(r21) }

l000214A0:
	{ r3 = memw(r21+4) }
	{ memw(r20) = r2; memw(r20+4) = r3 }
	{ r2 = memw(r21+8) }
	{ memw(r20+8) = r2 }
	{ r7 = memw(r21+12) }
	{ memw(r20+12) = r7 }
	{ r2 = memw(r21+24) }
	{ if (cmp.gtu(r2,r6.new)) jump:t 000214D0; r6 = memw(r20+20) }

l000214C8:
	{ r1 = memw(r21+16); r2 = memw(r21+24); call fn00009560 }

l000214D0:
	{ r4 = memd(r29+24); r2 = memw(r18) }
	{ r3 = memw(r18+4) }
	{ memw(r4) = r2; memw(r4+4) = r3 }
	{ r2 = memw(r18+8) }
	{ memw(r4+8) = r2 }
	{ r7 = memw(r18+12) }
	{ memw(r4+12) = r7 }
	{ r2 = memw(r18+24) }
	{ if (!cmp.gtu(r2,r6.new)) jump:t 00021500; r6 = memw(r4+20) }

l000214FC:
	{ r19 = add(r4,#0x10) }

l00021500:
	{ memw(r4+24) = r2; r0 = memw(r4+16); r19 = add(r4,#0x10) }
	{ r1 = memw(r18+16); r2 = memw(r18+24); call fn00009560 }
	{ r3 = memw(r20+16); r2 = memw(r19); r4 = add(PC,#0x9094); immext(#0x9080) }
	{ r3 = memw(r3); r2 = memw(r2); r1 = #0x61 }
	{ r2 = r17; r9:r8 = convert_sf2df(r3); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r2 = r17; r1 = #0x62; r4 = add(PC,#0x9081); immext(#0x9080) }
	{ memw(r29) = r16; call logmsg_function }
	{ r0 = #0x0 }

l00021558:
	{ r19:r18 = memd(r29+80); r17:r16 = memd(r29+88) }
	{ r23:r22 = memd(r29+64); r21:r20 = memd(r29+72) }
	{ r27:r26 = memd(r29+48); r25:r24 = memd(r29+56) }
	{ dealloc_return }
0002156C                                     00 C0 00 7F             ....

;; prelu_check: 00021570
prelu_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0x8F1A); immext(#0x8F00) }
	{ r17 = r0; r16 = r1; r1 = #0x69 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1 = #0x6A; r3 = add(PC,#0x8F15); immext(#0x8F00) }
	{ if (!cmp.eq(r2.new,#0x4)) jump:t 000215F8; r2 = memw(r17+16) }

l000215A4:
	{ r1 = #0x6B; r3 = add(PC,#0x10) }
	{ if (!cmp.eq(r2.new,#0x3)) jump:t 000215F8; r2 = memw(r17+20) }

l000215B8:
	{ r4 = memw(r17+4); r5 = #0x0 }

l000215BC:
	{ if (cmp.gt(r5.new,#0x2)) jump:nt 00021608; r5 = add(r5,#0x1); r2 = add(r2,#0x4) }

l000215CC:
	{ if (!cmp.eq(r6.new,#0x0)) jump:t 000215E4; r6 = memw(r4++#4); r3 = add(PC,#0x38) }

l000215DC:
	{ r1 = #0x6D }
	{ r6 = memw(r17+8); r3 = add(PC,#0x8EEB); immext(#0x8EC0) }

l000215E4:
	{ r6 = memw(r17+8); r3 = add(PC,#0x2B) }

l000215EC:
	{ if (!cmp.eq(r6.new,#0x0)) jump:t 000215BC; r6 = memw(r4+r2) }

l000215F8:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l00021608:
	{ r2 = r16; r1 = #0x70; r4 = add(PC,#0x8ECF); immext(#0x8EC0) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

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
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0002164C; r5 = memw(r2+16) }

l00021638:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x3B) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l0002164C:
	{ dealloc_return }

;; errlog_function: 00021650
;;   Called from:
;;     00021374 (in fn00021298)
;;     000215F8 (in prelu_check)
;;     00021640 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0x8E1F); immext(#0x8E00) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
00021674             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; prelu_execute: 00021680
prelu_execute proc
	{ allocframe(#0x38); memd(r29-16) = r17:r16; r4 = add(PC,#0x8FB5); immext(#0x8F80) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+40) = r19:r18; r7 = memw(r17+4); r1 = #0x38 }
	{ r5 = memw(r17+8); r2 = r16 }
	{ memd(r29+24) = r23:r22; memd(r29+32) = r21:r20 }
	{ r18 = memw(r7); r6 = memw(r7+4) }
	{ memd(r29+16) = r25:r24; r19 = memw(r5) }
	{ r3 = memw(r6+16) }
	{ memd(r29+8) = r27:r26; r23 = memw(r18+12) }
	{ r24 = memw(r18+8); r20 = memw(r18+16) }
	{ r26 = memw(r18+4); r25 = memw(r18) }
	{ r22 = memw(r3); r21 = memw(r19+16) }
	{ memw(r29) = r17; call logmsg_function }
	{ r2 = #0x0; immext(#0x0) }
	{ if (!p0.new) r3 = memw(r19+20); if (p0.new) r1 = #0x39; if (!p0.new) jump:nt 00021704; p0 = sfcmp.gt(r2,r22) }

l000216EC:
	{ r3 = add(PC,#0x8F61); immext(#0x8F40) }
	{ r2 = r16; call errlog_function }

l000216F8:
	{ r2 = r16 }
	{ r0 = #0xFFFFFFFF; jump 00021794 }

l00021704:
	{ r2 = mpyi(r26,r25) }
	{ r2 = mpyi(r2,r24) }
	{ r23 = mpyi(r2,r23) }
	{ if (!cmp.gtu(r2.new,r3)) jump:t 00021724; r2 = asl(r23,#0x2) }

l0002171C:
	{ jump 000216F8; r10 = #0x3A; r3 = add(PC,#0x4) }

l00021724:
	{ r4 = memw(r18+4); r3 = memw(r18); p0 = cmp.eq(r23,#0x0) }
	{ memw(r19) = r3; memw(r19+4) = r4 }
	{ r6 = memw(r18+8) }
	{ memw(r19+8) = r6 }
	{ r7 = memw(r18+12); r18 = #-0x1 }
	{ memw(r19+24) = r2; memw(r19+12) = r7; if (p0) jump:nt 00021778 }

l00021744:
	{ r19 = memw(r20) }
	{ r1:r0 = combine(r18,r19); call fn00009600 }
	{ r24 = r0; r2 = sfmpy(r22,r19) }
	{ r23 = add(r23,#0xFFFFFFFF); r1:r0 = combine(r18,r2); call fn000097B0 }
	{ memb(r21) = r2.new; r21 = add(r21,#0x4); r20 = add(r20,#0x4); r2 = sfadd(r24,r0) }

l00021778:
	{ r2 = r16; r1 = #0x42; r4 = add(PC,#0x8EF2); immext(#0x8EC0) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }

l00021794:
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ r27:r26 = memd(r29+8); r25:r24 = memd(r29+16) }
	{ dealloc_return }

;; prelu_check: 000217A8
prelu_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0x8E40); immext(#0x8E40) }
	{ r17 = r0; r16 = r1; r1 = #0x48 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,#0x2)) jump:t 000217E8; r2 = memw(r17+16); r1 = #0x49 }

l000217D4:
	{ r3 = add(PC,#0x2F) }
	{ r2 = r16; call errlog_function }

l000217DC:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l000217E8:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 00021800; r2 = memw(r17+20); r1 = #0x4A }

l000217F8:
	{ jump 000217DC; r3 = add(PC,#0x1A) }

l00021800:
	{ r2 = r16; r1 = #0x4B; r4 = add(PC,#0x8E1E); immext(#0x8E00) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00021820
;;   Called from:
;;     000216CC (in prelu_execute)
;;     00021788 (in prelu_execute)
;;     000217BC (in prelu_check)
;;     00021810 (in prelu_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00021844; r5 = memw(r2+16) }

l00021830:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x1F) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l00021844:
	{ dealloc_return }

;; errlog_function: 00021848
;;   Called from:
;;     000216F4 (in prelu_execute)
;;     000217D8 (in prelu_check)
;;     00021838 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r0 = add(PC,#0x8D83); immext(#0x8D80) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); r3 = #0x0 }
	{ dealloc_return }
0002186C                                     00 00 00 00             ....

;; sum_f_execute: 00021870
sum_f_execute proc
	{ allocframe(#0x30); r28 = #0x1 }
	{ r8 = memw(r0+4); r2 = memw(r0+8) }
	{ memd(r29+40) = r17:r16; r6 = memw(r0+16) }
	{ r14 = memw(r2); r7 = memw(r8); r2 = r1 }
	{ memd(r29+24) = r21:r20; memd(r29+32) = r19:r18; if (!p0.new) r17 = #0x1; p0 = cmp.eq(r6,#0x3) }
	{ r3 = memw(r7+12); r15 = memw(r7); if (!p0) r10 = #0x1; if (!p0) r1 = #0x1 }
	{ r5 = memw(r7+4); r4 = memw(r7+8); if (p0) r13:r12 = combine(r3,r15); if (p0) r18 = #0x0 }
	{ r7 = memw(r14+16); r6 = memw(r7+16) }
	{ memd(r29+8) = r25:r24; memd(r29+16) = r23:r22 }
	{ memd(r29) = r27:r26; if (p0) jump:nt 000218DC }

l000218D0:
	{ r9:r8 = combine(#0x1,#0x1); r13:r12 = combine(#0x1,#0x1) }
	{ jump fn00021A1C }

l000218DC:
	{ r1 = memw(r8+4); r10 = memw(r8+8); r9:r8 = combine(r4,r5) }
	{ r11 = memw(r1+12); r10 = memw(r10+16) }
	{ r10 = memw(r10); r1 = memw(r1+16) }
	{ if (!p0.new) r16 = add(r1,#0x0); if (!p0.new) r9:r8 = combine(r4,r5); p0 = cmp.eq(r11,#0x0); if (p0.new) jump:nt 00021958 }

l00021908:
	{ r13:r12 = combine(r3,r15); loop0(00021910,r11) }
	{ r17 = memw(r16++#4) }
	{ if (!cmp.gt(r17.new,#0x3)) jump:t 0002192C; r17 = add(r10,sub(#0x7F,r17)) }

l00021920:
	{ r12 = #0x1; r13 = #0x1; r9:r8 = combine(#0x1,#0x1) }

l0002192C:
	{ if (p0.new) r13 = #0x1; if (p0.new) jump:t 0002194C; p0 = cmp.eq(r9,#0x0) }

l00021934:
	{ if (p0.new) r9 = #0x1; if (p0.new) jump:t 0002194C; p0 = cmp.eq(r9,#0x1) }

l0002193C:
	{ if (p0.new) r8 = #0x1; if (p0.new) jump:t 0002194C; p0 = cmp.eq(r9,#0x2) }

l00021944:
	{ if (p0.new) r12 = #0x1; p0 = cmp.eq(r17,#0x3) }

l0002194C:
	{ nop; nop }
	{ r18 = r11 }

l00021958:
	{ if (cmp.eq(r0.new,#0x2)) jump:t 0002197C; r0 = memb(r0+32); r11 = r9; p0 = cmp.eq(r18,#0x0) }

l0002196C:
	{ r1 = r13; r28 = r12; r17 = r8 }
	{ jump fn00021A1C }

l0002197C:
	{ r0 = r13; r17:r16 = combine(r8,r12); if (!p0) r11 = add(r9,#0x0) }
	{ if (p0.new) r17:r16 = combine(r8,r12); if (!p0) r0 = add(r13,#0x0); if (p0) jump:nt 000219D8 }

l00021994:
	{ loop0(00021998,r18) }
	{ r18 = memw(r1++#4) }
	{ if (!cmp.gt(r18.new,#0x3)) jump:t 000219B4; r18 = add(r10,sub(#0x7F,r18)) }

l000219A8:
	{ r16 = #0x0; r0 = #0x0; r17 = #0x0 }
	{ jump 000219D0 }

l000219B4:
	{ if (p0.new) r0 = #0x0; if (p0.new) jump:t 000219D0; p0 = cmp.eq(r10,#0x0) }

l000219BC:
	{ if (p0.new) r11 = #0x0; if (p0.new) jump:t 000219D0; p0 = cmp.eq(r10,#0x1) }

l000219C4:
	{ if (p0.new) r17 = #0x0; if (p0.new) jump:t 000219D0; p0 = cmp.eq(r10,#0x2) }

l000219CC:
	{ r16 = #-0x1; p0 = cmp.eq(r18,#0x3) }

l000219D0:
	{ nop; nop }

l000219D8:
	{ p3 = cmp.eq(r0,#0x0); p0 = cmp.eq(r11,#0x0); p2 = cmp.eq(r17,#0x0); p1 = cmp.eq(r16,#0x0) }

l000219E0:
	{ p3 = cmp.eq(r0,#0x0); p0 = cmp.eq(r11,#0x0) }

;; fn000219E8: 000219E8
;;   Called from:
;;     000219D8 (in sum_f_execute)
;;     000219E0 (in gemaccb_asm)
fn000219E8 proc
	{ r1 = mux(p1,#0x1,r16); p1 = or(p1,p2) }
	{ r10 = mux(p1,#0x1,r16); if (!p2) r1 = add(r17,#0x0); p2 = or(p1,p0) }
	{ r17 = mux(p2,#0x1,r16); if (!p0) r1 = add(r11,#0x0); if (!p0) r10 = add(r1,#0x0); if (p3) jump:nt fn00021A1C }

;; fn00021A0C: 00021A0C
;;   Called from:
;;     000219FC (in fn000219E8)
;;     000219FC (in fn000219E8)
;;     000219FC (in fn000219E8)
;;     00021A1C (in fn00021A1C)
fn00021A0C proc
	{ r1 = r0; r10 = r1; r17 = r10; r28 = r17 }

;; fn00021A1C: 00021A1C
;;   Called from:
;;     000218D8 (in sum_f_execute)
;;     00021978 (in sum_f_execute)
;;     000219FC (in fn000219E8)
;;     00021A0C (in fn00021A0C)
fn00021A1C proc
	{ r11 = memw(r14+20); p0 = cmp.gt(r12,#0x0); r0 = mpyi(r13,r9) }
	{ r0 = mpyi(r0,r8) }
	{ r0 = mpyi(r0,r12) }
	{ if (!cmp.gtu(r0.new,r11)) jump:t 00021A58; r0 = asl(r0,#0x2) }

l00021A3C:
	{ r1 = #0xC6; r0 = add(PC,#0x16) }
	{ call errlog_function; r3 = add(PC,#0x8CA9); immext(#0x8C80) }
	{ r0 = #0xFFFFFFFF; jump 00021BB4 }

l00021A58:
	{ memw(r14+24) = r0; memw(r14+12) = r1; r0 = #0x0 }
	{ memw(r14+4) = r17; memw(r14+8) = r10; if (p0) r0 = #0x0 }
	{ memw(r14) = r28; if (p0) r1 = #0x0; immext(#0x0); if (!p0) jump:nt 00021BB4 }

l00021A80:
	{ p2 = cmp.eq(r8,#0x1); p1 = cmp.eq(r13,#0x1); p3 = cmp.eq(r9,#0x1); p0 = cmp.eq(r12,#0x1) }

l00021A88:
	{ p2 = cmp.eq(r8,#0x1); p1 = cmp.eq(r13,#0x1) }

;; fn00021A90: 00021A90
;;   Called from:
;;     00021A80 (in fn00021A1C)
;;     00021A88 (in gemaddvvm_asm)
fn00021A90 proc
	{ r14 = mux(p1,r3,#0x1); r28 = mux(p3,r4,#0x1); r2 = mux(p0,r15,#0x1) }
	{ r15 = mux(p2,r5,#0x1) }

;; fn00021AA0: 00021AA0
;;   Called from:
;;     00021A9C (in fn00021A90)
;;     00021A9C (in fn00021A90)
;;     00021BA8 (in gemaddvvm_asm)
fn00021AA0 proc
	{ p0 = cmp.gt(r8,#0x0); r10 = #0x0; if (!p0.new) jump:nt 00021BA8 }

l00021AA4:
	{ p0 = cmp.gt(r8,#0x0); r10 = #0x0 }

l00021AAC:
	{ p0 = cmp.gt(r9,#0x0); r11 = #0x0; if (!p0.new) jump:nt 00021BA0 }

l00021AB0:
	{ p0 = cmp.gt(r9,#0x0); r11 = #0x0 }

l00021AB8:
	{ p0 = cmp.gt(r13,#0x0); r17:r16 = combine(#0x0,r7); if (!p0.new) jump:nt 00021B98 }

l00021AC4:
	{ r18 = r1; if (!p0.new) jump:nt 00021B84; p0 = cmp.gt(r2,#0x0) }

l00021ACC:
	{ r19:r18 = combine(#0x0,r1) }

l00021AD0:
	{ p0 = cmp.gt(r15,#0x0); r22 = add(r19,r0); if (!p0.new) jump:nt 00021B7C }

l00021AD4:
	{ p0 = cmp.gt(r15,#0x0); r22 = add(r19,r0) }

l00021ADC:
	{ r21:r20 = combine(r10,#0x0) }
	{ r21 += mpyi(r22,r5) }

l00021AE4:
	{ p0 = cmp.gt(r28,#0x0); r24 = add(r21,r20); if (!p0.new) jump:nt 00021B74 }

l00021AF0:
	{ r23:r22 = combine(r11,#0x0); loop1(00021AFC,r28) }
	{ r23 += mpyi(r24,r4) }
	{ p0 = cmp.gt(r14,#0x0); r24 = r17; r25 = add(r23,r22); if (!p0.new) jump:nt 00021B68 }

l00021B0C:
	{ p0 = cmp.gtu(r14,#0x1); r26 = add(r14,#0xFFFFFFFF) }
	{ if (p0) r27 = add(r26,#0xFFFFFFFF); r24 += mpyi(r25,r3) }
	{ r25 = addasl(r6,r24,#0x2) }
	{ r24 = add(r25,#0x4) }
	{ r25 = memw(r25); if (p0) jump:nt 00021B34 }

l00021B2C:
	{ r24 = r25; jump 00021B64 }

l00021B34:
	{ r24 = memw(r24); r26 = add(r24,#0x4); p0 = cmp.gtu(r26,#0x1); loop0(00021B48,r27) }
	{ if (!p0) jump:nt 00021B60 }

l00021B48:
	{ r24 = memw(r26); r26 = add(r26,#0x4); r27 = r24; r18 = sfadd(r18,r25) }
	{ nop; r25 = r27 }

l00021B60:
	{ r18 = sfadd(r18,r25) }

l00021B64:
	{ r18 = sfadd(r18,r24) }

l00021B68:
	{ nop; nop; r22 = add(r22,#0x1) }

l00021B74:
	{ if (!cmp.eq(r20.new,r15)) jump:t 00021AE4; r20 = add(r20,#0x1) }

l00021B7C:
	{ if (!cmp.eq(r19.new,r2)) jump:t 00021AD0; r19 = add(r19,#0x1) }

l00021B80:
	{ if (!cmp.eq(r19.new,r2)) jump:t 00021AD4 }

l00021B84:
	{ memw(r16) = r18; r17 = r17; r16 = add(r16,#0x4) }

l00021B88:
	{ memw(r16) = r18; r17 = r17 }

l00021B8C:
	{ p0 = cmp.eq(r17,r13); if (!p0.new) jump:nt 00021AC4 }

l00021B94:
	{ r7 = addasl(r7,r13,#0x2) }

l00021B98:
	{ if (!cmp.eq(r11.new,r9)) jump:t 00021AB8; r11 = add(r11,#0x1) }

l00021BA0:
	{ if (!cmp.eq(r10.new,r8)) jump:t 00021AAC; r10 = add(r10,#0x1) }

l00021BA4:
	{ if (!cmp.eq(r10.new,r8)) jump:t 00021AB0 }

l00021BA8:
	{ if (!cmp.eq(r0.new,r12)) jump:t fn00021AA0; r0 = add(r0,#0x1) }

l00021BAC:
	{ if (!cmp.eq(r0.new,r12)) jump:t 00021AA4 }

l00021BB4:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ r27:r26 = memd(r29); r25:r24 = memd(r29+8) }
	{ dealloc_return }
00021BC8                         00 40 00 7F 00 C0 00 7F         .@......

;; sum_f_check: 00021BD0
sum_f_check proc
	{ allocframe(#0x10); memd(r29-16) = r17:r16; r4 = add(PC,#0x8AC3); immext(#0x8AC0) }
	{ r17 = r0; r16 = r1; r1 = #0x37 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r2 = memw(r17+16) }
	{ if (cmp.gtu(r3.new,r2)) jump:t 00021C1C; r3 = #0x4 }

l00021BFC:
	{ r1 = #0x38; r0 = add(PC,#0x0) }

l00021C04:
	{ r3 = add(PC,#0x8A9B); immext(#0x8A80) }

l00021C0C:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = #0xFFFFFFFF }

l00021C18:
	{ dealloc_return; r17:r16 = memd(r29+8) }

l00021C1C:
	{ r1 = #0x39; if (!p0.new) jump:nt 00021C30; p0 = cmp.eq(r2,#0x0) }

l00021C24:
	{ jump 00021C04; r0 = add(PC,#0x8A54); immext(#0x8A40) }

l00021C30:
	{ if (cmp.eq(r2.new,#0x1)) jump:t 00021C50; r2 = memw(r17+20); r1 = #0x3A }

l00021C40:
	{ r0 = add(PC,#0x3C) }
	{ jump 00021C0C; r3 = add(PC,#0x8A6A); immext(#0x8A40) }

l00021C50:
	{ r2 = r16; r1 = #0x3B; r4 = add(PC,#0x8A6E); immext(#0x8A40) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = #0x0 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00021C6C
;;   Called from:
;;     00021BE4 (in sum_f_check)
;;     00021C5C (in sum_f_check)
logmsg_function proc
	{ allocframe(#0x8); r3 = #0x2 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00021C90; r5 = memw(r2+16) }

l00021C7C:
	{ r5 = add(r29,#0x10); r3 = #0x2; r0 = add(PC,#0x0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); call logv }

l00021C90:
	{ dealloc_return }

;; errlog_function: 00021C94
;;   Called from:
;;     00021A44 (in fn00021A1C)
;;     00021C0C (in sum_f_check)
;;     00021C84 (in logmsg_function)
errlog_function proc
	{ allocframe(#0x8); r4 = r3; r3 = #0x0 }
	{ memb(r29+1) = r6.new; r6 = add(r29,#0x10); r5 = add(r29,#0x10); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gemacca_asm: 00021CC0
;;   Called from:
;;     00021CBC (in errlog_function)
gemacca_asm proc
	{ allocframe(#0x20); r8 = zxth(r2); r1 = lsr(r1,#0x1); r6 = lsr(r2,#0x10) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r7 = lsr(r2,#0x14); loop1(00021D20,r1) }
	{ memd(r29+16) = r21:r20; r10 = sub(#0x20,r8); r9 = asl(r8,#0x1); r13 = addasl(r0,r8,#0x1) }
	{ p2 = cmp.eq(r0,r0); r11 = sub(#0x10,r8); r6 = sub(r9,r6); M1 = r8 }
	{ r12 = r13; r13 = addasl(r13,r8,#0x1); M0 = r11 }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r19:r18 = combine(#0x0,#0x0); r17:r16 = combine(#0x0,#0x0); loop0(00021D40,r7) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r21:r20 = memd(r0++m0); r15:r14 = memd(r0+8) }
	{ r15:r14 = memd(r0+8); r21:r20 = memd(r0++m0); p2 = not(p2); r17:r16 += vraddub(r15:r14,r21:r20) }
	{ if (p2) r12 = add(r12,r10); if (!p2) r12 = add(r12,r8); r19:r18 += vraddub(r15:r14,r21:r20) }
	{ r0 = add(r0,r6); r17 = add(r19,r18); r16 = add(r17,r16) }
	{ r19:r18 = memd(r3); r17 = mpyi(r17,r4); r16 = mpyi(r16,r4) }
	{ r17 = add(r17,r19); r16 = add(r16,r18); r12 = r13 }
	{ memd(r3++#8) = r17:r16; p2 = or(p2,!p2); r13 = addasl(r13,r8,#0x1) }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r21:r20 = memd(r29+16) }
	{ dealloc_return }
00021D9C                                     00 00 00 00             ....

;; gemaccb_asm: 00021DA0
gemaccb_asm proc
	{ p0 = cmp.eq(r3,#0x0); r4 = #0x1010101; immext(#0x1010100); if (p0.new) jump:nt 000219E0; p0 = cmp.eq(r3,#-0x1); r2 = lsr(r2,#0x2) }

l00021DB0:
	{ if (!p0.new) jump:nt fn00021F70; p0 = cmp.eq(r0,r0); r5 = #0x10; if (p0) jump:nt 00021DC8; loop0(00021DC0,r2) }

l00021DC0:
	{ r1 = add(r1,#0x1); r0 = #0x10; if (p0.new) jump:t 00021EC0; p0 = cmp.gtu(r4,#0x2) }

l00021DC8:
	{ jump 00021E0C; r0 = r1 }
00021DCC                                     04 C0 01 28             ...(
00021DD0 E3 C3 65 19 A3 E0 21 1C 04 44 43 1C 22 C0 21 28 ..e...!..DC.".!(
00021DE0 00 C0 9F 52 00 00 00 00 00 00 00 00 00 00 00 00 ...R............
00021DF0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; l2pref: 00021E00
l2pref proc
	{ r2 = combine(r2.l,r1.l); r3 = zxth(r3) }
	{ l2fetch(r0,r3:r2) }

l00021E0C:
	{ jumpr r31 }
00021E10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gemaddvvm_asm: 00021E20
;;   Called from:
;;     0000D584 (in gemm_asm)
gemaddvvm_asm proc
	{ r0 = r0; r1 = #0x0; r4 = asl(r4,#0x2); r3 = lsr(r3,#0x1) }
	{ dcfetch(r0,#0x20); r1 = memb(r0+1); r5 = memuh(r0); r3 = add(r3,#0xFFFFFFFF); M0 = r4 }
	{ r0 = r0; r5 = #0x0; r11:r10 = memd(r0++#8); r8 = r2 }

l00021E48:
	{ r0 = r0; r2 = #0x30; r9 = memw(r29); if (!p0.new) jump:nt 00021E48; p0 = cmp.eq(r1,r6); if (p0.new) jump:nt 00021A88; p0 = cmp.eq(r10,#-0x1) }

l00021E58:
	{ r0 = r0; r8 = #0x32; if (!p0.new) jump:nt 00021E5C; p0 = cmp.eq(r1,r0); p2 = !cmp.eq(r9,00000000); if (p0.new) jump:nt 00021A9C; p0 = cmp.eq(r11,#-0x1) }
	{ if (p1.new) jump:nt 00021EFC; p1 = cmp.eq(r0,#0x2); if (p1.new) jump:nt 00021EF8; p1 = cmp.eq(r0,#0x2); r6 = #0x4; loop0(00021EA0,r3) }
	{ dcfetch(r0,#0x60); r0 = memb(r4); r2 = memuh(r0); jump 00021F8C; r10 = r2; if (!p0.new) jump:nt 00021E80; p0 = cmp.eq(r4,r3) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r0 = r0; r8 = #0x32; r11:r10 = memd(r0++#8); if (!p0.new) jump:nt 00021EA8; p0 = cmp.eq(r4,r6); jump 00021AB0; r8 = r2 }
	{ r0 = r0; r2 = #0x30; if (!p0.new) jump:nt 00021EB0; p0 = cmp.eq(r1,r6); jump 00021FC4; r10 = r5; if (p0.new) jump:nt 00021AF0; p0 = cmp.eq(r10,#-0x1) }
	{ r0 = r0; r8 = #0x32; if (!p0.new) jump:nt 00021EC4; p0 = cmp.eq(r1,r0); jump 00021AD0; r8 = r5; if (p0.new) jump:nt 00021B04; p0 = cmp.eq(r11,#-0x1) }
	{ dcfetch(r0,#0x60); r0 = memb(r4); r2 = memuh(r0); jump 00021FE4; r10 = r2; if (!p0.new) jump:nt 00021ED8; p0 = cmp.eq(r4,r3) }
	{ r0 = r0; r8 = #0x32; if (!p0.new) jump:nt 00021EE8; p0 = cmp.eq(r4,r6); jump 00021AF0; r8 = r2 }
	{ jump 00022000; r10 = r5; loop0(00021EF4,#0x5) }
	{ if (!p1.new) jump:t 00021FC4; p1 = cmp.gtu(r6,#0x8) }
	{ jump 00021B08; r9 = r8; if (!p1.new) jump:t 000221CC; p1 = cmp.gtu(r6,#0xA) }
	{ jump 00022014; r11 = r10; r6 = add(r6,r6) }
	{ r1 = add(r1,#0x1); r5 = #0x2 }
	{ r0 = r0; r5 = #0x2 }
	{ jumpr r31 }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gemmacbbw_asm: 00021F20
gemmacbbw_asm proc
	{ allocframe(#0x38); r4 = asl(r4,#0x2) }
	{ memw(r29+48) = r28; memd(r29) = r17:r16; r5 = zxth(r5); r28 = lsr(r5,#0x10) }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r3 = lsr(r3,#0x3); M1 = r4 }
	{ memd(r29+8) = r19:r18; M0 = r5 }
	{ memd(r29+32) = r25:r24; loop1(00021FE0,r3) }
	{ r3 = r2; r8 = asl(r5,#0x3) }
	{ memd(r29+40) = r27:r26 }
	{ r0 = add(r0,r0); r2 = #0x30; r13 = r0; r8 = sub(r8,r5); r6 = lsr(r28,#0x4) }

;; fn00021F70: 00021F70
;;   Called from:
;;     00021DB0 (in gemaccb_asm)
;;     00021F64 (in gemmacbbw_asm)
fn00021F70 proc
	{ r0 = add(r0,r0); r2 = #0x30 }
	{ r0 = add(r0,r0); r2 = #0x30; r4 = sub(#0x10,r8); r9 = r1 }
	{ r0 = add(r0,r0); r2 = #0x30; r5 = add(r5,r5); r7 = add(r13,#0x30); r8 += sub(r5,r28) }
	{ dcfetch(r7,#0x0); r2 = memb(r0+2); r9 = memuh(r0); r10 = add(r13,r5); r14 = lsr(r5,#0x1) }
	{ r3:r2 = combine(r7,#0x0); r9 = #0x0; r7 = add(r7,r14); r13 = addasl(r13,r5,#0x2) }
	{ r0 = add(r0,r0); r2 = #0x30; r23:r22 = memd(r0+8); r15 = sub(#0x20,r14) }
	{ r0 = add(r0,r0); r2 = #0x30; r17:r16 = memd(r0++m0) }
	{ r0 = add(r0,r0); r2 = #0x30; r21:r20 = memd(r0+8); r6 = add(r6,#0xFFFFFFFF); r11 = add(r10,r5) }
	{ r0 = add(r0,r0); r2 = #0x30; r19:r18 = memd(r0++m0); r12 = add(r11,r5); loop0(00021FE0,r6) }
	{ dcfetch(r10,#0x0); r2 = memb(r2+2); r9 = memuh(r0); if (p0.new) jump:t gemmpybbw_asm; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t gemmpybbw_asm; p0 = cmp.gtu(r6,#0xA) }

;; fn00021FF0: 00021FF0
;;   Called from:
;;     00021FDC (in fn00021F70)
;;     00021FE0 (in fn00021F70)
fn00021FF0 proc
	{ r7:r6 = combine(r7,#0x0); r9 = #0x0; r10 = add(r10,r14); if (p0.new) jump:t 000222F0; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 000222F0; p0 = cmp.gtu(r7,#0xB) }
	{ r21:r20 = memd(r0++m0); r23:r22 = memd(r0+8); if (p0.new) jump:t 00022300; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00022300; p0 = cmp.gtu(r0,#0x8) }
	{ r19:r18 = memd(r0++m0); r17:r16 = memd(r0+8); if (p0.new) jump:t 00022310; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00022310; p0 = cmp.gtu(r1,#0x9) }
	{ dcfetch(r11,#0x0); r11 = add(r11,r14); if (p0.new) jump:t 00022324; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00022324; p0 = cmp.gtu(r4,#0x8) }
	{ dcfetch(r12,#0x0); r12 = add(r12,r14); if (p0.new) jump:t 00022334; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00022334; p0 = cmp.gtu(r5,#0x9) }
	{ r21:r20 = memd(r0++m0); r19:r18 = memd(r0+8); if (p0.new) jump:t 00022344; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00022344; p0 = cmp.gtu(r6,#0xA) }
	{ r25:r24 = memd(r0++m0); r17:r16 = memd(r0+8); if (p0.new) jump:t 00022354; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022354; p0 = cmp.gtu(r7,#0xB) }
	{ r15 = r14; r14 = r15; if (p0.new) jump:t 00022368; p0 = cmp.gtu(r8,#0x8); if (p0.new) jump:t 00022368; p0 = cmp.gtu(r4,#0x8) }
	{ r23:r22 = memd(r0++m0); r25:r24 = memd(r0+8); if (p0.new) jump:t 00022378; p0 = cmp.gtu(r9,#0x9); if (p0.new) jump:t 00022378; p0 = cmp.gtu(r5,#0x9) }
	{ r27:r26 = memd(r0); r21:r20 = memd(r0+8); if (p0.new) jump:t 00022388; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00022388; p0 = cmp.gtu(r2,#0xA) }
	{ r0 = add(r0,r4); if (p0.new) jump:t 00022398; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022398; p0 = cmp.gtu(r3,#0xB) }
	{ dcfetch(r7,#0x0); r2 = memb(r0+2); r9 = memuh(r0); if (p0.new) jump:t 000223A8; p0 = cmp.gtu(r10,#0x8); if (p0.new) jump:t 000223A8; p0 = cmp.gtu(r6,#0x8) }
	{ r3:r2 = combine(r7,#0x0); r9 = #0x0; r7 = add(r7,r14); if (p0.new) jump:t 000223B8; p0 = cmp.gtu(r11,#0x9); if (p0.new) jump:t 000223B8; p0 = cmp.gtu(r7,#0x9) }
	{ r17:r16 = memd(r0++m0); r23:r22 = memd(r0+8); if (p0.new) jump:t 000223C8; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 000223C8; p0 = cmp.gtu(r8,#0xA) }
	{ r19:r18 = memd(r0++m0); r21:r20 = memd(r0+8); if (p0.new) jump:t 000223D8; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 000223D8; p0 = cmp.gtu(r9,#0xB) }
	{ r2 = and(r2,#0x1); r9 = #0x10; r7 = r13; if (p0.new) jump:t 000223DC; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 000223DC; p0 = cmp.gtu(r6,#0xA) }
	{ r7:r6 = combine(r7,#0x0); r9 = #0x0; r10 = add(r13,r5); if (p0.new) jump:t 000223EC; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 000223EC; p0 = cmp.gtu(r7,#0xB) }
	{ r21:r20 = memd(r0++m0); r23:r22 = memd(r0+8); if (p0.new) jump:t 000223FC; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 000223FC; p0 = cmp.gtu(r0,#0x8) }
	{ r19:r18 = memd(r0++m0); r17:r16 = memd(r0+8); if (p0.new) jump:t 0002240C; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 0002240C; p0 = cmp.gtu(r1,#0x9) }
	{ r0 = add(r0,r0); r3 = #0x32; r11 = add(r10,r5); if (p0.new) jump:t 00022420; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00022420; p0 = cmp.gtu(r4,#0x8) }
	{ r12 = add(r11,r5); if (p0.new) jump:t 00022430; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00022430; p0 = cmp.gtu(r5,#0x9) }
	{ r21:r20 = memd(r0++m0); r19:r18 = memd(r0+8); if (p0.new) jump:t 0002243C; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 0002243C; p0 = cmp.gtu(r6,#0xA) }
	{ r25:r24 = memd(r0++m0); r17:r16 = memd(r0+8); if (p0.new) jump:t 0002244C; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 0002244C; p0 = cmp.gtu(r7,#0xB) }
	{ r0 = add(r0,r0); r3 = #0x32; if (p0.new) jump:t 00022460; p0 = cmp.gtu(r8,#0x8); if (p0.new) jump:t 00022460; p0 = cmp.gtu(r4,#0x8) }
	{ r23:r22 = memd(r0++m0); r25:r24 = memd(r0+8); if (p0.new) jump:t 0002246C; p0 = cmp.gtu(r9,#0x9); if (p0.new) jump:t 0002246C; p0 = cmp.gtu(r5,#0x9) }
	{ r0 = add(r0,r0); r3 = #0x32; r21:r20 = memd(r0+8); if (p0.new) jump:t 0002247C; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 0002247C; p0 = cmp.gtu(r2,#0xA) }
	{ r0 = add(r0,r0); r3 = #0x32; r27:r26 = memd(r0); if (p0.new) jump:t 0002248C; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 0002248C; p0 = cmp.gtu(r3,#0xB) }
	{ r0 = add(r0,r0); r3 = #0x32; r0 = add(r0,r4); if (p0.new) jump:t 000224A0; p0 = cmp.gtu(r10,#0x8); if (p0.new) jump:t 000224A0; p0 = cmp.gtu(r6,#0x8) }
	{ r0 = add(r0,r0); r3 = #0x32; if (p0.new) jump:t 000224B0; p0 = cmp.gtu(r11,#0x9); if (p0.new) jump:t 000224B0; p0 = cmp.gtu(r7,#0x9) }
	{ r0 = add(r0,r8); if (p0.new) jump:t fn000224BC; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t fn000224BC; p0 = cmp.gtu(r8,#0xA) }
	{ r0 = add(r0,r0); r3 = #0x32; if (p0.new) jump:t 000224C8; p0 = cmp.gtu(r9,#0xB); r14 = lsr(r5,#0x1) }
	{ r0 = add(r0,r0); r3 = #0x32; r15 = sub(#0x20,r14); if (p0.new) jump:t 000224D4; p0 = cmp.gtu(r5,#0xB) }
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
	{ r11 = add(r11,r15); loop0(00022240,#0x2) }
	{ dcfetch(r11,#0x0) }
	{ r12 = add(r12,r15); r13 = addasl(r13,r5,#0x2) }
	{ dcfetch(r12,#0x0) }
	{ nop }
	{ dcfetch(r7,#0x0); r7 = add(r7,r14) }
	{ dcfetch(r10,#0x0); r10 = add(r10,r14) }
	{ dcfetch(r11,#0x0); r11 = add(r11,r14) }
	{ dcfetch(r12,#0x0); r12 = add(r12,r14) }
	{ dcfetch(r7,#0x0); r7 = add(r7,r15) }
	{ dcfetch(r10,#0x0); r10 = add(r10,r15) }
	{ dcfetch(r11,#0x0); r11 = add(r11,r15) }
	{ dcfetch(r12,#0x0); r12 = add(r12,r15) }
	{ r23:r22 = memd(r0+8) }
	{ dcfetch(r7,#0x0); r17:r16 = memd(r0++m0); r9 = r1 }
	{ r2 = and(r2,#0x1); r9 = #0x10; r21:r20 = memd(r0+8); r7 = add(r7,r14) }
	{ r3:r2 = combine(r7,#0x0); r9 = #0x0; r19:r18 = memd(r0++m0); loop0(000222AC,r6); immext(#0xFFFFFD40) }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return; r28 = memw(r29+48) }
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
	{ memw(r29+48) = r28; if (!p0.new) jump:nt fn000224BC; p0 = cmp.eq(r12,r12); r5 = zxth(r5); r28 = lsr(r5,#0x10) }

l000222F4:
	{ r4 = asl(r4,#0x2); M0 = r5 }
	{ memd(r29) = r17:r16; r8 = asl(r5,#0x3); r9 = lsr(r28,#0x4) }
	{ memd(r29+8) = r19:r18; r13 = r0; r8 = sub(r8,r5); r3 = lsr(r3,#0x3) }
	{ r9 = add(r9,#0xFFFFFFFF); r4 = sub(#0x10,r8); M1 = r4 }
	{ memd(r29+16) = r21:r20; r5 = add(r5,r5); r7 = add(r13,#0x50); r8 += sub(r5,r28) }
	{ memd(r29+32) = r25:r24; memd(r29+24) = r23:r22; r10 = add(r13,r5); loop1(000223A0,r3) }
	{ memd(r29+40) = r27:r26; r11 = add(r10,r5); r6 = r1; loop0(000223A0,r9) }
	{ dcfetch(r7,#0x0); r2 = memb(r0+2); r6 = memuh(r0); r12 = add(r11,r5); r14 = lsr(r5,#0x1) }
	{ r3:r2 = combine(r7,#0x0); r6 = #0x0; r15 = sub(#0x20,r14); r7 = add(r7,r14); r13 = addasl(r13,r5,#0x2) }
	{ r17:r16 = memd(r0++m0); r23:r22 = memd(r0+8); jump fn00022534; r12 = r12; jump 00022530; r12 = r12 }

l00022380:
	{ r19:r18 = memd(r0++m0); r21:r20 = memd(r0+8); jump fn0002254C; r12 = r12; jump 00022548; r12 = r12 }
00022390 00 40 00 7F 00 40 00 7F 00 40 00 7F 00 C0 00 7F .@...@...@......
000223A0 80 6A 16 19 81 6A 14 19 2A 42 06 29 00 C0 0A 94 .j...j..*B.)....
000223B0 80 6B 17 19 81 6B 15 19 0A 4E 0A F3 2B E7 06 28 .k...k...N..+..(
000223C0 80 68 10 19 81 68 12 19 36 40 C0 91 14 C0 C0 9D .h...h..6@......

l000223D0:
	{ r19:r18 = memd(r0++m0); r17:r16 = memd(r0+8); if (p0.new) jump:t fn000226D0; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t fn000226D0; p0 = cmp.gtu(r1,#0x9) }

l000223E0:
	{ dcfetch(r11,#0x0); r11 = add(r11,r14); if (p0.new) jump:t 000226E4; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 000226E4; p0 = cmp.gtu(r4,#0x8) }
	{ dcfetch(r12,#0x0); r12 = add(r12,r14); if (p0.new) jump:t 000226F4; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 000226F4; p0 = cmp.gtu(r5,#0x9) }
	{ r21:r20 = memd(r0++m0); r19:r18 = memd(r0+8); if (p0.new) jump:t 00022704; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00022704; p0 = cmp.gtu(r6,#0xA) }

l00022408:
	{ r21:r20 = memd(r0++m0); r19:r18 = memd(r0+8) }
	{ r25:r24 = memd(r0++m0); r17:r16 = memd(r0+8); if (p0.new) jump:t 00022714; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022714; p0 = cmp.gtu(r7,#0xB) }

l00022420:
	{ r15 = r14; r14 = r15; if (p0.new) jump:t 00022728; p0 = cmp.gtu(r8,#0x8); if (p0.new) jump:t 00022728; p0 = cmp.gtu(r4,#0x8) }
	{ r23:r22 = memd(r0++m0); r25:r24 = memd(r0+8); if (p0.new) jump:t 00022738; p0 = cmp.gtu(r9,#0x9); if (p0.new) jump:t 00022738; p0 = cmp.gtu(r5,#0x9) }
	{ r27:r26 = memd(r0); r21:r20 = memd(r0+8); if (p0.new) jump:t 00022748; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00022748; p0 = cmp.gtu(r2,#0xA) }
	{ r0 = add(r0,r4); if (p0.new) jump:t 00022758; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022758; p0 = cmp.gtu(r3,#0xB) }
	{ dcfetch(r7,#0x0); r2 = memb(r0+2); r6 = memuh(r0); if (p0.new) jump:t 00022768; p0 = cmp.gtu(r10,#0x8); if (p0.new) jump:t 00022768; p0 = cmp.gtu(r6,#0x8) }
	{ r3:r2 = combine(r7,#0x0); r6 = #0x0; r7 = add(r7,r14); if (p0.new) jump:t 00022778; p0 = cmp.gtu(r11,#0x9); if (p0.new) jump:t 00022778; p0 = cmp.gtu(r7,#0x9) }
	{ r17:r16 = memd(r0++m0); r23:r22 = memd(r0+8); if (p0.new) jump:t 00022788; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 00022788; p0 = cmp.gtu(r8,#0xA) }
	{ r19:r18 = memd(r0++m0); r21:r20 = memd(r0+8); if (p0.new) jump:t 00022798; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 00022798; p0 = cmp.gtu(r9,#0xB) }
	{ r2 = and(r2,#0x1); r6 = #0x10; r7 = r13; if (p0.new) jump:t 0002279C; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 0002279C; p0 = cmp.gtu(r6,#0xA) }
	{ r7:r6 = combine(r7,#0x0); r6 = #0x0; r10 = add(r13,r5); if (p0.new) jump:t fn000227AC; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t fn000227AC; p0 = cmp.gtu(r7,#0xB) }

;; fn000224BC: 000224BC
;;   Called from:
;;     000222E4 (in gemmpybbw_asm)
fn000224BC proc
	{ r21:r20 = memd(r0++m0); r23:r22 = memd(r0+8); if (p0.new) jump:t fn000227BC; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t fn000227BC; p0 = cmp.gtu(r0,#0x8) }

l000224CC:
	{ r19:r18 = memd(r0++m0); r17:r16 = memd(r0+8); if (p0.new) jump:t 000227CC; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 000227CC; p0 = cmp.gtu(r1,#0x9) }

;; fn000224D8: 000224D8
;;   Called from:
;;     00022888 (in fn00022884)
;;     00022888 (in fn00022878)
;;     00022888 (in fn00022884)
fn000224D8 proc
	{ r19:r18 = memd(r0++m0) }
	{ r11 = add(r10,r5); if (p0.new) jump:t 000227E0; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 000227E0; p0 = cmp.gtu(r4,#0x8) }

l000224E8:
	{ r12 = add(r11,r5); if (p0.new) jump:t 000227EC; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 000227EC; p0 = cmp.gtu(r5,#0x9) }
	{ r21:r20 = memd(r0++m0); r19:r18 = memd(r0+8); if (p0.new) jump:t 000227F8; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 000227F8; p0 = cmp.gtu(r6,#0xA) }

l000224F8:
	{ r21:r20 = memd(r0++m0); r19:r18 = memd(r0+8); if (p0.new) jump:t 000227FC; p0 = cmp.gtu(r0,#0xA) }

l00022500:
	{ r21:r20 = memd(r0++m0) }

l00022504:
	{ r25:r24 = memd(r0++m0); r17:r16 = memd(r0+8); if (p0.new) jump:t 00022808; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00022808; p0 = cmp.gtu(r7,#0xB) }

l00022514:
	{ r0 = add(r0,r0); r2 = #0x32; if (!p0.new) jump:nt 000226D4; p0 = cmp.eq(r0,r0); if (p0.new) jump:t 0002281C; p0 = cmp.gtu(r8,#0x8); if (p0.new) jump:t 0002281C; p0 = cmp.gtu(r4,#0x8) }
	{ r23:r22 = memd(r0++m0); r25:r24 = memd(r0+8); if (p0.new) jump:t 0002282C; p0 = cmp.gtu(r9,#0x9); if (p0.new) jump:t 0002282C; p0 = cmp.gtu(r5,#0x9) }

l0002252C:
	{ r23:r22 = memd(r0++m0); r25:r24 = memd(r0+8) }

;; fn00022534: 00022534
;;   Called from:
;;     00022370 (in gemmpybbw_asm)
;;     0002252C (in fn00022610)
fn00022534 proc
	{ r27:r26 = memd(r0); r21:r20 = memd(r0+8); if (p0.new) jump:t 0002283C; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 0002283C; p0 = cmp.gtu(r2,#0xA) }

;; fn00022544: 00022544
;;   Called from:
;;     00022534 (in fn00022534)
;;     00022534 (in fn00022534)
fn00022544 proc
	{ r0 = add(r0,r0); r2 = #0x32; r0 = add(r0,r4); if (p0.new) jump:t 0002284C; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 0002284C; p0 = cmp.gtu(r3,#0xB) }

;; fn0002254C: 0002254C
;;   Called from:
;;     00022380 (in gemsumb_asm)
;;     00022808 (in fn00022610)
fn0002254C proc
	{ r0 = add(r0,r0); r2 = #0x32; r0 = add(r0,r4) }
	{ r0 = add(r0,r0); r2 = #0x32; r0 = add(r0,r8); if (p0.new) jump:t 00022860; p0 = cmp.gtu(r10,#0x8); if (p0.new) jump:t 00022860; p0 = cmp.gtu(r6,#0x8) }

l00022564:
	{ r0 = add(r0,r0); r2 = #0x32; if (!p0.new) jump:nt 00022724; p0 = cmp.eq(r1,r1); if (p0.new) jump:t 00022870; p0 = cmp.gtu(r11,#0x9); if (p0.new) jump:t 00022870; p0 = cmp.gtu(r7,#0x9) }
	{ r0 = add(r0,r0); r2 = #0x32; if (!p0.new) jump:nt 00022738; p0 = cmp.eq(r2,r2); if (p0.new) jump:t 00022880; p0 = cmp.gtu(r4,#0xA); if (p0.new) jump:t 00022880; p0 = cmp.gtu(r8,#0xA) }

;; fn00022578: 00022578
;;   Called from:
;;     000227B0 (in fn000227AC)
fn00022578 proc
	{ r0 = add(r0,r0); r2 = #0x32; if (!p0.new) jump:nt 0002273C; p0 = cmp.eq(r2,r2); if (p0.new) jump:t fn00022884; p0 = cmp.gtu(r4,#0xA) }

l00022584:
	{ r0 = add(r0,r0); r2 = #0x32; if (!p0.new) jump:nt 00022748; p0 = cmp.eq(r3,r3); if (p0.new) jump:t 00022890; p0 = cmp.gtu(r5,#0xB); if (p0.new) jump:t 00022890; p0 = cmp.gtu(r9,#0xB) }

l00022590:
	{ r0 = add(r0,r0); r2 = #0x32 }
	{ r0 = add(r0,r0); r2 = #0x32; r14 = lsr(r5,#0x1); r13 = addasl(r13,r5,#0x2) }

;; fn0002259C: 0002259C
;;   Called from:
;;     00022594 (in fn000227C8)
;;     00022910 (in fn00022910)
fn0002259C proc
	{ r0 = add(r0,r0); r2 = #0x32 }
	{ r0 = add(r0,r0); r2 = #0x32; r15 = sub(#0x20,r14); loop0(000225C0,#0x3) }
	{ nop }
	{ nop; nop; nop; nop }
	{ dcfetch(r7,#0x0); r7 = add(r7,r14) }
	{ dcfetch(r10,#0x0); r10 = add(r10,r14) }

l000225CC:
	{ dcfetch(r10,#0x0) }

;; fn000225D0: 000225D0
;;   Called from:
;;     000225C4 (in fn0002259C)
;;     000225C8 (in fn0002259C)
fn000225D0 proc
	{ dcfetch(r11,#0x0); r11 = add(r11,r14) }
	{ dcfetch(r12,#0x0); r12 = add(r12,r14) }
	{ dcfetch(r7,#0x0); r7 = add(r7,r15) }

;; fn000225E8: 000225E8
;;   Called from:
;;     000225E0 (in fn000225D0)
;;     00022960 (in vmemcpy_asm)
fn000225E8 proc
	{ dcfetch(r10,#0x0); r10 = add(r10,r15) }

;; fn000225EC: 000225EC
;;   Called from:
;;     000225E8 (in fn000225E8)
;;     00022964 (in fn00022964)
fn000225EC proc
	{ dcfetch(r10,#0x0) }
	{ dcfetch(r11,#0x0); r11 = add(r11,r15) }

;; fn000225F8: 000225F8
;;   Called from:
;;     000225F0 (in fn000225EC)
;;     000225F0 (in fn000225EC)
;;     00022980 (in fn00022970)
fn000225F8 proc
	{ dcfetch(r12,#0x0); r12 = add(r12,r15) }
	{ dcfetch(r7,#0x0); r23:r22 = memd(r0+8); if (!p0.new) jump:nt fn000227C8; p0 = cmp.eq(r4,r4); r6 = r1 }

l00022604:
	{ dcfetch(r7,#0x0); r23:r22 = memd(r0+8); if (!p0.new) jump:nt 000227CC; p0 = cmp.eq(r4,r4) }

;; fn00022610: 00022610
;;   Called from:
;;     00022600 (in fn000225F8)
;;     00022604 (in fn000227BC)
;;     00022654 (in fn000227BC)
;;     00022800 (in fn000227BC)
;;     00022800 (in fn000227BC)
;;     00022808 (in fn000227BC)
;;     000229C8 (in fn000227BC)
fn00022610 proc
	{ r2 = and(r2,#0x1); r6 = #0x10; r17:r16 = memd(r0++m0); if (!p0.new) jump:nt 000227D8; p0 = cmp.eq(r5,r5); r7 = add(r7,r14) }

l00022620:
	{ r3:r2 = combine(r7,#0x0); r6 = #0x0; r21:r20 = memd(r0+8); if (!p0.new) jump:nt 000227EC; p0 = cmp.eq(r6,r6) }

l0002262C:
	{ r19:r18 = memd(r0++m0); if (!p0.new) jump:nt 000227F8; p0 = cmp.eq(r7,r7); loop0(000226FC,r9); immext(#0xFFFFFD40) }

l0002263C:
	{ nop; nop; nop }

l00022640:
	{ nop; nop }

l00022648:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }

l00022650:
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }

l00022654:
	{ r27:r26 = memd(r29+40) }

l00022658:
	{ r28 = memw(r29+48) }
	{ dealloc_return }

;; gemsuma_asm: 00022660
;;   Called from:
;;     0000D548 (in gemm_asm)
gemsuma_asm proc
	{ allocframe(#0x20); r8 = zxth(r2); r1 = lsr(r1,#0x1); r6 = lsr(r2,#0x10) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r7 = lsr(r2,#0x14); loop1(000226C0,r1) }
	{ memd(r29+16) = r21:r20; r10 = sub(#0x20,r8); r9 = asl(r8,#0x1); r13 = addasl(r0,r8,#0x1) }
	{ r11 = sub(#0x10,r8); r6 = sub(r9,r6); M1 = r8 }
	{ p2 = cmp.eq(r0,r0); r12 = r13; r13 = addasl(r13,r8,#0x1); M0 = r11 }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r19:r18 = combine(#0x0,#0x0); r17:r16 = combine(#0x0,#0x0); loop0(fn000226E0,r7) }

l000226C4:
	{ r19:r18 = combine(#0x0,#0x0); r17:r16 = combine(#0x0,#0x0) }

l000226C8:
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
;;     000226D8 (in fn000226D0)
;;     000226D8 (in gemsuma_asm)
fn000226E0 proc
	{ r21:r20 = memd(r0++m0); r15:r14 = memd(r0+8) }

;; fn000226E8: 000226E8
;;   Called from:
;;     000226E0 (in fn000226E0)
;;     000226E0 (in fn000226E0)
;;     000226E0 (in gemsuma_asm)
fn000226E8 proc
	{ r15:r14 = memd(r0+8); r21:r20 = memd(r0++m0); p2 = not(p2); r17:r16 += vraddub(r15:r14,r21:r20) }
	{ if (p2) r12 = add(r12,r10); if (!p2) r12 = add(r12,r8); r19:r18 += vraddub(r15:r14,r21:r20) }
	{ r0 = add(r0,r6); r17 = add(r19,r18); r16 = add(r17,r16) }
	{ r17 = mpyi(r17,r4); r16 = mpyi(r16,r4) }

l00022714:
	{ r17 = mpyi(r17,r4) }
	{ r17 = add(r17,r5); r16 = add(r16,r5); r12 = r13 }
	{ memd(r3++#8) = r17:r16; p2 = cmp.eq(r0,r0); r13 = addasl(r13,r8,#0x1) }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r21:r20 = memd(r29+16) }
	{ dealloc_return }
00022738                         00 00 00 00                     ....    

l0002273C:
	{ r0 = memw(r0); r0 = memw(r0) }

;; gemsumb_asm: 00022740
;;   Called from:
;;     0000D558 (in gemm_asm)
;;     0002273C (in fn00022578)
gemsumb_asm proc
	{ p0 = cmp.eq(r3,#0x0); r4 = #0x1010101; immext(#0x1010100); if (p0.new) jump:nt 00022380; p0 = cmp.eq(r3,#-0x1); r2 = lsr(r2,#0x2) }

l00022750:
	{ if (!p0.new) jump:nt fn00022910; p0 = cmp.eq(r0,r0); r5 = #0x10; if (p0) jump:nt 00022768; loop0(00022760,r2) }

l00022760:
	{ r1 = add(r1,#0x1); r0 = #0x10; if (p0.new) jump:t 00022860; p0 = cmp.gtu(r4,#0x2) }

l00022768:
	{ jump fn000227AC; r0 = r1 }
0002276C                                     E3 C3 65 19             ..e.
00022770 A3 60 21 1C 22 C0 21 28 00 C0 9F 52 00 00 00 00 .`!.".!(...R....

;; quantize_asm: 00022780
;;   Called from:
;;     000182C4 (in autorequantize_execute)
;;     00018644 (in requantize_execute)
quantize_asm proc
	{ if (p0.new) jump:nt 00022408; p0 = cmp.eq(r4,#-0x1); r5 = #0x7F; if (p0.new) jump:nt 000223D0; p0 = cmp.eq(r1,#-0x1) }

l0002278C:
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt 0002294C; p0 = cmp.eq(r8,r0); p0 = bitsclr(r4,r5) }
	{ r4 = add(r4,#0x7F); if (p0.new) jump:nt 000223E8; p0 = cmp.eq(r2,#-0x1) }
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt vmemcpy_asm; p0 = cmp.eq(r8,r1); jump 000224E8; r0 = r9; r4 = lsr(r4,#0x7) }

;; fn000227AC: 000227AC
;;   Called from:
;;     00022768 (in gemsumb_asm)
fn000227AC proc
	{ r1 = add(r1,#0x1); r0 = #0x10 }
	{ if (!p0) r4 = add(r4,#0xFFFFFFFF); if (p0.new) jump:t fn00022578; p0 = cmp.eq(r9,r0) }

;; fn000227B8: 000227B8
;;   Called from:
;;     000227B0 (in fn000227AC)
;;     00022B30 (in fn00022ED8)
fn000227B8 proc
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt 0002297C; p0 = cmp.eq(r8,r2); jump 00022500; r1 = r9 }

;; fn000227BC: 000227BC
;;   Called from:
;;     000224BC (in fn000224BC)
;;     000224BC (in fn000224BC)
fn000227BC proc
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt fn00022980; p0 = cmp.eq(r8,r2) }

l000227C0:
	{ r1 = add(r1,#0x1); r0 = #0x10 }

;; fn000227C4: 000227C4
;;   Called from:
;;     000227BC (in fn000227BC)
;;     000227C0 (in fn00022A30)
fn000227C4 proc
	{ if (p0.new) jump:t 0002258C; p0 = cmp.eq(r9,r1); loop0(000227E0,r4) }

;; fn000227C8: 000227C8
;;   Called from:
;;     00022600 (in fn000225F8)
;;     00022B44 (in fn00022A30)
;;     00022B44 (in fn00022A30)
fn000227C8 proc
	{ if (p0.new) jump:t 00022590; p0 = cmp.eq(r9,r1) }

l000227CC:
	{ nop }
	{ nop; nop; nop; nop }

l000227D8:
	{ nop; nop }

l000227E0:
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt 000229A4; p0 = cmp.eq(r8,r3); jump 0002252C; r2 = r9 }

l000227EC:
	{ jump 00022400; r5 = r4; if (p0.new) jump:t 000225B8; p0 = cmp.eq(r9,r2) }
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt 000229B4; p0 = cmp.eq(r8,r0); jump 00022540; r3 = r9 }

l000227F8:
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt 000229B8; p0 = cmp.eq(r8,r0) }

l000227FC:
	{ r1 = add(r1,#0x1); r0 = #0x10 }

l00022800:
	{ if (p0.new) jump:t 000225CC; p0 = cmp.eq(r9,r3) }

l00022804:
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt 000229C4; p0 = cmp.eq(r8,r1); jump fn0002254C; r0 = r9 }

l00022808:
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt 000229C8; p0 = cmp.eq(r8,r1) }

l00022810:
	{ jump 00022424; r7 = r6; if (p0.new) jump:t 000225D8; p0 = cmp.eq(r9,r0) }
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt 000229DC; p0 = cmp.eq(r8,r2); jump 00022560; r1 = r9 }
	{ r1 = add(r1,#0x1); r3 = #0x12; jump 0002297C; r10 = r11; if (p0.new) jump:t fn000225EC; p0 = cmp.eq(r9,r1) }
	{ r1 = add(r1,#0x1); r0 = #0x10; if (!p0.new) jump:nt 000229F4; p0 = cmp.eq(r8,r3); jump 0002257C; r2 = r9 }

l0002283C:
	{ if (p0) jumpr:nt r31 }
00022840 E6 62 29 1C 0B C5 E4 1F A7 C3 E9 1F E7 E3 29 1C .b)...........).
00022850 0A C7 E6 1F                                     ....            

l00022854:
	{ jump 000229AC; r10 = r11 }
00022858                         0C C0 83 28 00 C0 9F 52         ...(...R

l00022860:
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

l00022868:
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; fn00022878: 00022878
;;   Called from:
;;     00022874 (in fn00022E10)
;;     00022BF4 (in memconvert_hvx)
fn00022878 proc
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r11 = asl(r4,#0x4); r8 = mpyi(r3,r4) }

;; fn00022884: 00022884
;;   Called from:
;;     00022578 (in fn00022578)
;;     00022880 (in fn00022878)
fn00022884 proc
	{ r11 = asl(r4,#0x4) }
	{ r9 = #0x8000; immext(#0x8000); r8 = add(r8,#0xFF); if (p0.new) jump:nt fn000224D8; p0 = cmp.eq(r5,#-0x1) }

;; fn00022898: 00022898
;;   Called from:
;;     00022888 (in fn00022884)
;;     00022888 (in fn00022878)
;;     00022888 (in fn00022884)
fn00022898 proc
	{ r7 = r2; r3 = lsr(r3,#0x2); r8 = lsr(r8,#0x7) }
	{ r10 = combine(r11.l,r3.l); if (p0.new) jump:nt 000224F8; p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 000224F8; p0 = cmp.eq(r9,#-0x1) }

l000228B0:
	{ if (p0.new) jump:nt 00022508; p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 00022508; p0 = cmp.eq(r9,#-0x1) }
	{ l2fetch(r1,r11:r10); r6 = addasl(r2,r4,#0x2) }
	{ p3 = sp1loop0(000228C4,r8) }
	{ r1 = add(r1,#0x1); r7 = #0x10; if (p0.new) jump:t 00022618; p0 = cmp.eq(r8,r0) }
	{ r1 = add(r1,#0x1); r1 = #0x14; if (p0.new) r7 = add(r2,#0x0); p0 = cmp.eq(r6,r7); if (p0.new) jump:t 00022620; p0 = cmp.eq(r8,r1) }
	{ r1 = add(r1,#0x1); r7 = #0x10; if (!p0.new) jump:nt 000228DC; p0 = cmp.eq(r5,r0); if (p0.new) jump:t 00022634; p0 = cmp.eq(r8,r2) }
	{ r1 = add(r1,#0x1); r1 = #0x14; if (p0.new) r7 = add(r2,#0x0); p0 = cmp.eq(r6,r7); if (p0.new) jump:t 00022640; p0 = cmp.eq(r8,r3) }
	{ r1 = add(r1,#0x1); r7 = #0x10; if (!p0.new) jump:nt 000228F8; p0 = cmp.eq(r5,r1); jump 00022584; r11 = r10; if (p0.new) jump:nt fn0002254C; p0 = cmp.eq(r9,#-0x1) }
	{ r1 = add(r1,#0x1); r1 = #0x14; jump 00022594; r13 = r12; if (p0.new) r7 = add(r2,#0x0); p0 = cmp.eq(r6,r7) }

;; fn00022910: 00022910
;;   Called from:
;;     00022750 (in gemsumb_asm)
fn00022910 proc
	{ r1 = add(r1,#0x1); r1 = #0x14; jump fn0002259C; r13 = r12 }
00022918                         2C 40 A9 19 2D 40 A9 19         ,@..-@..

l00022920:
	{ r1 = add(r1,#0x1); r7 = #0x10; if (!p0.new) jump:nt 00022924; p0 = cmp.eq(r5,r2) }

l00022924:
	{ r1 = add(r1,#0x1); r7 = #0x10 }

l00022928:
	{ r1 = add(r1,#0x1); r1 = #0x14; if (!p0.new) jump:nt 0002292C; p0 = cmp.eq(r5,r3); if (p0.new) r7 = add(r2,#0x0); p0 = cmp.eq(r6,r7) }

l0002292C:
	{ r1 = add(r1,#0x1); r1 = #0x14; if (!p0.new) jump:nt 00022930; p0 = cmp.eq(r5,r3); if (p0.new) r7 = add(r2,#0x0) }

l00022930:
	{ r1 = add(r1,#0x1); r1 = #0x14; if (!p0.new) jump:nt 00022934; p0 = cmp.eq(r5,r3) }

l00022934:
	{ r1 = add(r1,#0x1); r1 = #0x14 }

l00022938:
	{ r9 = add(r9,#0x1); r0 = #0x1A; jump 00022A88; r7 = r6; if (p0.new) jump:nt 0002258C; p0 = cmp.eq(r9,#-0x1) }
00022944             00 C0 9F 52 00 00 00 00 00 00 00 00     ...R........
00022950 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

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
	{ if (p0.new) jump:nt fn000225E8; p0 = cmp.eq(r0,#-0x1); r7 = add(r2,r0); r8 = #0x1010101; immext(#0x1010100) }

;; fn00022964: 00022964
;;   Called from:
;;     0000D030 (in unpad2d)
fn00022964 proc
	{ if (p0.new) jump:nt fn000225EC; p0 = cmp.eq(r0,#-0x1); r7 = add(r2,r0); r8 = #0x1 }

;; fn00022970: 00022970
;;   Called from:
;;     00022960 (in vmemcpy_asm)
;;     00022964 (in fn00022964)
fn00022970 proc
	{ if (p0.new) jump:nt fn000225F8; p0 = cmp.eq(r7,#-0x1); r10 = add(r0,r2); r9 = add(r8,r8); r5 = and(r0,#0x7F) }

l0002297C:
	{ if (p0.new) jump:nt 00022604; p0 = cmp.eq(r7,#-0x1) }

;; fn00022980: 00022980
;;   Called from:
;;     000227BC (in fn000227BC)
;;     00022970 (in fn00022970)
;;     0002297C (in fn000227BC)
fn00022980 proc
	{ r7 = and(r7,#0x7F); r4 = and(r1,#0x7F); if (p0.new) jump:nt 000226C8; p0 = cmp.gt(r9,#-0x1); if (p0.new) jump:nt 000226C4; p0 = cmp.eq(r9,#-0x1) }

l00022990:
	{ jump 000229B0; r0 = #0x0; r3 = sub(r2,r7); r5 = add(r5,r2); r6 = sub(r4,r5) }
	{ r3 = add(r3,#0x7F); if (!p2.new) r9 = add(r8,#0x0); p2 = cmp.gt(r5,#0x7F); if (!p0.new) jump:t 00022664; p0 = cmp.gtu(r8,#0x0) }

l000229A4:
	{ r3 = add(r3,#0x7F); if (!p2.new) r9 = add(r8,#0x0); p2 = cmp.gt(r5,#0x7F) }

l000229AC:
	{ r3 = add(r3,#0x7F) }

l000229B0:
	{ r0 = r0; r1 = #0x0; p1 = cmp.gt(r6,#0xFFFFFFFF); if (p0.new) jump:nt 00022640; p0 = tstbit(r9,#0x0); r3 = lsr(r3,#0x7) }

l000229B8:
	{ r0 = r0; r1 = #0x0; p1 = cmp.gt(r6,#0xFFFFFFFF) }

l000229C0:
	{ if (p1) r1 = add(r1,#0x80); immext(#0x80); if (p0.new) jump:nt 00022650; p0 = cmp.eq(r9,#-0x1); loop0(000229E0,r3) }

l000229C4:
	{ if (p1) r1 = add(r1,#0x80); immext(#0x80); if (p0.new) jump:nt 00022654; p0 = cmp.eq(r9,#-0x1) }

l000229C8:
	{ if (p1) r1 = add(r1,#0x80); immext(#0x80) }

l000229D0:
	{ nop; nop; nop; nop }
	{ r1 = add(r1,#0x1); r1 = #0x10; jump 00022BA0; r1 = #0x21; if (p1.new) jump:nt 000229E4; p1 = cmp.gtu(r6,#0x1) }
000229EC                                     14 80 03 1E             ....
000229F0 22 C1 80 29 02 41 06 1B 41 C0 01 28 00 40 6A 70 "..).A..A..(.@jp
00022A00 02 C8 80 28                                     ...(            

l00022A04:
	{ jumpr r31 }
00022A08                         00 00 00 00 00 00 00 00         ........
00022A10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; memconvert_hvx: 00022A20
;;   Called from:
;;     00013754 (in concat_execute_slice_asm)
memconvert_hvx proc
	{ r6 = memw(r29); if (!p0.new) jump:nt 00022BF4; p0 = cmp.eq(r11,r11); r4 = combine(r4.l,r4.l); r3 = combine(r3.l,r3.l) }

;; fn00022A30: 00022A30
;;   Called from:
;;     00022A20 (in memconvert_hvx)
;;     00022DA8 (in fn00022D8C)
fn00022A30 proc
	{ jump 00022B44; r11 = #0xB; jump 00022A5C; r3 = #0x3; r9 = #0x1010101; immext(#0x1010100) }
00022A40 00 44 26 60 25 40 A3 19                         .D&`%@..        

l00022A48:
	{ r15 = r0; r12 = #0x80 }
	{ if (p0.new) jump:nt 000226D8; p0 = cmp.eq(r0,#-0x1); r14 = and(r0,#0x7F); r8 = add(r2,r0); r6 = r1 }

l00022A60:
	{ if (p0.new) jump:nt fn000226E8; p0 = cmp.eq(r8,#-0x1); r8 = and(r8,#0x7F); r13 = and(r6,#0x7F); r11 = add(r9,r9) }

l00022A70:
	{ r0 = r0; r6 = #0x0; jump 00022BFC; r11 = r12; if (p0.new) jump:nt fn000227B8; p0 = cmp.gt(r11,#-0x1); if (p0.new) jump:nt 000227B4; p0 = cmp.eq(r11,#-0x1) }
00022A80 07 4D 2E F3 0E 42 0E F3                         .M...B..        

l00022A88:
	{ jump 00022854; r6 = r5; jump 00022AA8; r0 = #0x0 }
00022A90 63 60 69 19 E1 7F 67 75 0A 42 28 F3             c`i...gu.B(.    

l00022A9C:
	{ jump 00022868; r7 = r5 }
00022AA0 48 46 44 19 26 4C 06 FB EA 4F 0A B0 E2 CF 4E 75 HFD.&L...O....Nu
00022AB0 49 47 44 19 0B 40 C9 74 C6 4B 4C 1F 4C C1 06 29 IGD..@.t.KL.L..)
00022AC0 2A 47 0A 8C 48 43 AB 19 E6 C6 A5 1F 49 44 AB 19 *G..HC......ID..
00022AD0 40 49 68 1F E7 C7 A5 1F 10 42 0A 60 48 46 44 19 @Ih......B.`HFD.
00022AE0 02 D8 8F 28 49 47 44 19 0F 40 60 70 21 38 50 38 ...(IGD..@`p!8P8
00022AF0 00 40 00 7F 00 40 00 7F 00 40 00 7F 00 C0 00 7F .@...@...@......
00022B00 C6 4B 4C 1F 4C C1 06 29 41 49 68 1F E6 C6 A5 1F .KL.L..)AIh.....
00022B10 07 41 43 1E E7 C7 A5 1F 48 46 44 19 02 41 07 1B .AC.....HFD..A..
00022B20 E0 E1 03 1E 49 87 44 19 00 4A 8B 1F 22 C1 8F 29 ....I.D..J.."..)

l00022B30:
	{ if (p0.new) jump:nt fn000227B8; p0 = cmp.eq(r0,#-0x1); r14 = and(r0,#0x7F); r8 = add(r2,r0); r6 = r1 }

l00022B40:
	{ if (p0.new) jump:nt fn000227C8; p0 = cmp.eq(r8,#-0x1); jump 000227C0; r9 = r8; r13 = and(r6,#0x7F); r11 = add(r9,r9) }

l00022B44:
	{ if (p0.new) jump:nt 000227CC; p0 = cmp.eq(r8,#-0x1); jump fn000227C4; r9 = r8; r13 = and(r6,#0x7F) }

l00022B50:
	{ r8 = and(r8,#0x7F); if (p0.new) jump:nt fn00022898; p0 = cmp.gt(r11,#-0x1); if (p0.new) jump:nt 00022894; p0 = cmp.eq(r11,#-0x1) }
	{ r0 = r0; r6 = #0x0; jump 00022CE8; r11 = r12; if (p1.new) jump:nt 00022B60; p1 = cmp.gtu(r7,#0x1) }
	{ r8 = r8; r15 = #0x8 }
	{ jumpr r31 }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

l00022B7C:
	{ r0 = memw(r0); r0 = memw(r0) }

;; avgpool_aligned_hvx: 00022B80
;;   Called from:
;;     00012004 (in avgpool_execute_slice_asm)
;;     00022B7C (in fn00022F2C)
avgpool_aligned_hvx proc
	{ r6 = memw(r29); r7 = sub(r5,r3); M0 = r2 }
	{ r6 = combine(r6.l,r6.l); r9 = #0x1010101; immext(#0x1010100); r7 = mpyi(r7,r2) }
	{ if (!p0.new) jump:nt fn00022D68; p0 = cmp.eq(r6,r6); r1 = add(r1,#0x80); r10 = r1; loop1(00022BC0,r4) }

l00022BA0:
	{ if (!p0.new) jump:nt fn00022D6C; p0 = cmp.eq(r6,r6); r1 = add(r1,#0x80); r10 = r1 }

;; fn00022BAC: 00022BAC
;;   Called from:
;;     00022B9C (in avgpool_aligned_hvx)
;;     00022BA0 (in fn000227BC)
fn00022BAC proc
	{ jump fn00022D6C; r6 = r6; loop0(00022BC0,r3) }
00022BB4             00 40 00 7F 00 40 00 7F 00 C0 00 7F     .@...@......
00022BC0 00 A2 89 19 42 C0 0A 2B 10 5F 03 60 0A 87 0A F3 ....B..+._.`....
00022BD0 00 C0 00 7F 18 5E 24 60 44 40 46 19 0A 40 61 70 .....^$`D@F..@ap
00022BE0 02 F0 E2 BF 45 41 46 19 01 50 01 B0 10 40 02 75 ....EAF..P...@.u
00022BF0 E0 C6 46 1F                                     ..F.            

l00022BF4:
	{ r1 = add(r1,#0x1); r0 = #0x12; jump fn00022878; r5 = r4; if (p0) jump:nt 00022BC0 }

l00022BFC:
	{ r1 = add(r1,#0x1); r0 = #0x12 }
00022C00 00 C0 9F 52                                     ...R            
00022C04             00 00 00 00 00 00 00 00 00 00 00 00     ............
00022C10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; avgpool_nonaligned_hvx: 00022C20
;;   Called from:
;;     00012018 (in avgpool_execute_slice_asm)
avgpool_nonaligned_hvx proc
	{ r6 = memw(r29); r7 = sub(r5,r3); M0 = r2 }
	{ r6 = combine(r6.l,r6.l); r9 = #0x1010101; immext(#0x1010100); r7 = mpyi(r7,r2) }
	{ if (!p0.new) jump:nt fn00022E08; p0 = cmp.eq(r6,r6); r1 = add(r1,#0x80); r10 = r1; loop1(00022C60,r4) }

l00022C4C:
	{ jump fn00022E0C; r6 = r6; loop0(00022C60,r3) }
00022C54             00 40 00 7F 00 40 00 7F 00 C0 00 7F     .@...@......
00022C60 E2 C0 0A 2B 00 C0 00 7F 00 A2 89 19 00 C0 00 7F ...+............
00022C70 00 5F 03 60 0A 87 0A F3 00 C0 00 7F 08 5E 24 60 ._.`.........^$`
00022C80 44 40 46 19 02 F0 E2 BF                         D@F.....        

l00022C88:
	{ r12 = and(r0,#0x7F); r11 = add(r2,#0x80); p0 = cmp.gt(r2,#0x0); if (!p0.new) jump:nt 00022D10; p0 = cmp.gtu(r6,#0x1) }

l00022C98:
	{ if (p0.new) jump:nt 00022920; p0 = cmp.eq(r0,#-0x1); if (p0) r11 = #0x80 }

l00022CA0:
	{ jump 00022924; r5 = r4; r12 = add(r12,r11); r13 = sub(#0x0,r0) }
00022CAC                                     45 C0 AC 19             E...
00022CB0 0A 49 00 5C E1 4F 4C 75 27 C3 6D 19 15 41 43 1E .I.\.OLu'.m..AC.
00022CC0 10 C0 43 1E 0A 40 61 70 E0 40 40 1C 07 C9 80 28 ..C..@ap.@@....(
00022CD0 C8 60 DF 5C 01 50 01 B0                         .`.\.P..        

l00022CD8:
	{ r1 = add(r1,#0x1); r0 = #0x18; if (!p0.new) jump:nt fn00022E98; p0 = cmp.eq(r1,r1) }

l00022CE0:
	{ jumpr r31 }
00022CE4             00 00 00 00 00 00 00 00 00 00 00 00     ............
00022CF0 00 00 00 00 00 00 00 00 00 00 00 00             ............    

l00022CFC:
	{ r0 = memw(r0); r0 = memw(r0) }

;; maxpool_aligned_hvx: 00022D00
;;   Called from:
;;     00015FD0 (in maxpool_execute_slice_asm)
;;     00022CFC (in fn00022D08)
maxpool_aligned_hvx proc
	{ r10 = r1; r7 = sub(r5,r3); M0 = r2 }

;; fn00022D08: 00022D08
;;   Called from:
;;     0002303C (in fn0002303C)
fn00022D08 proc
	{ r10 = r1 }

;; fn00022D0C: 00022D0C
;;   Called from:
;;     00022D00 (in maxpool_aligned_hvx)
;;     00022D08 (in fn00022D08)
fn00022D0C proc
	{ r1 = add(r1,#0x80); r7 = mpyi(r7,r2); loop1(fn00022D20,r4) }

l00022D10:
	{ r1 = add(r1,#0x80); r7 = mpyi(r7,r2) }

;; fn00022D18: 00022D18
;;   Called from:
;;     00022D0C (in fn00022D0C)
;;     00022D0C (in fn00022D0C)
;;     00022D10 (in fn00022FF8)
fn00022D18 proc
	{ if (!p0.new) jump:nt fn00022ED8; p0 = cmp.eq(r0,r0); loop0(fn00022D20,r3) }

;; fn00022D20: 00022D20
;;   Called from:
;;     00022D18 (in fn00022D18)
;;     00022D18 (in fn00022D18)
;;     00022E50 (in fn00022E50)
;;     00023054 (in fn00023054)
fn00022D20 proc
	{ r0 = r0; r10 = #0x30; jump 00022E60; r2 = r0 }
00022D28                         10 5F 03 60 0A 87 0A F3         ._.`....
00022D30 00 C0 00 7F 18 5E 24 60 0A 40 61 70 01 50 01 B0 .....^$`.@ap.P..
00022D40 02 F0 E2 BF EE 78 DF 5C 10 40 02 75 E0 40 40 1C .....x.\.@.u.@@.
00022D50 00 C1 20 29 00 C0 9F 52 00 00 00 00 00 00 00 00 .. )...R........

;; maxpool_nonaligned_hvx: 00022D60
;;   Called from:
;;     00015FD8 (in maxpool_execute_slice_asm)
maxpool_nonaligned_hvx proc
	{ r10 = r1; r7 = sub(r5,r3); M0 = r2 }

;; fn00022D68: 00022D68
;;   Called from:
;;     00022B9C (in avgpool_aligned_hvx)
fn00022D68 proc
	{ r10 = r1 }

;; fn00022D6C: 00022D6C
;;   Called from:
;;     00022BA0 (in fn000227BC)
;;     00022BB8 (in fn00022BAC)
;;     00022D60 (in fn00022D68)
;;     00022D68 (in fn00022D68)
fn00022D6C proc
	{ if (!p0.new) jump:nt fn00022F2C; p0 = cmp.eq(r0,r0); loop0(00022D80,r3) }

l00022D74:
	{ r1 = add(r1,#0x80); r7 = mpyi(r7,r2); loop1(00022D80,r4) }

l00022D80:
	{ r0 = r0; r10 = #0x30 }
	{ jump 00022EC4; r2 = r0; nop }

;; fn00022D8C: 00022D8C
;;   Called from:
;;     00023128 (in fn00023128)
;;     00023130 (in fn00023130)
;;     00023130 (in fn00023130)
fn00022D8C proc
	{ nop; r10 = add(r10,r7); loop0(00022D80,r3) }
	{ r11 = r2; r12 = and(r0,#0x7F); r2 = add(r2,#0xFFFFFF80); loop1(00022D80,r4) }
	{ if (p0.new) jump:nt fn00022A30; p0 = cmp.eq(r0,#-0x1); r13 = sub(#0x0,r0); if (p0.new) r11 = #0x80; p0 = cmp.gt(r2,#0x0) }

l00022DB8:
	{ if (!p0.new) jump:nt 00022A04; p0 = cmp.gtu(r13,#0x0); r12 = add(r12,r11) }

l00022DC0:
	{ if (p0.new) jump:nt 00022A48; p0 = cmp.eq(r12,#-0x1); p1 = cmp.gt(r12,#0x7F); if (p1.new) jump:nt 00022DD4 }

l00022DCC:
	{ jump 00022DEC; r0 = #0x0; jump 00022DF4; r1 = #0x1 }

l00022DD4:
	{ r9 = add(r9,#0x1); r0 = #0x8; if (!p0.new) jump:nt 00022F94; p0 = cmp.eq(r0,r0); r10 = r1 }

l00022DD8:
	{ r9 = add(r9,#0x1); r0 = #0x8; if (!p0.new) jump:nt fn00022F98; p0 = cmp.eq(r0,r0) }

;; fn00022DE0: 00022DE0
;;   Called from:
;;     00022DD4 (in fn00022D8C)
;;     00022E38 (in fn00022E3C)
;;     00022FA0 (in fn00022FA0)
fn00022DE0 proc
	{ r1 = add(r1,#0x1); r0 = #0x18; r1 = add(r1,#0x80); if (p0) jump:nt 00022D80 }

l00022DE8:
	{ r1 = add(r1,#0x1); r0 = #0x18 }

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
;;     00022FC8 (in fn00022FBC)
gemvmpybbw_asm proc
	{ allocframe(#0x10); r6 = memd(r29) }
	{ memd(r29) = r17:r16; r17 = #0x7F; r5 = asl(r5,#0x2) }

;; fn00022E08: 00022E08
;;   Called from:
;;     00022C3C (in avgpool_nonaligned_hvx)
;;     00023200 (in biasadd_relu_requant_nonaligned_hvx)
fn00022E08 proc
	{ memd(r29) = r17:r16; r17 = #0x7F }

;; fn00022E0C: 00022E0C
;;   Called from:
;;     00022C4C (in avgpool_nonaligned_hvx)
;;     0002317C (in fn0002313C)
;;     000231D8 (in fn0002313C)
fn00022E0C proc
	{ memd(r29) = r17:r16 }

;; fn00022E10: 00022E10
;;   Called from:
;;     00022D8C (in fn00022D8C)
;;     00022E08 (in fn00022E08)
;;     00022E0C (in fn00022E0C)
;;     0002313C (in fn0002313C)
fn00022E10 proc
	{ if (p0.new) jump:nt 00022A9C; p0 = cmp.eq(r5,#-0x1); if (!p0.new) jump:nt 00022E20; p0 = bitsclr(r5,r17) }

l00022E1C:
	{ jump 00022E20; r0 = r0 }

l00022E20:
	{ r8 = add(r2,#0x80); r7 = lsr(r6,#0x4) }
	{ dcfetch(r0,#0x80); r7 = add(r7,#0xFFFFFFFF) }
	{ r11:r10 = memd(r0++#8); r16 = #0x1010101; immext(#0x1010100); loop0(00022E80,r7) }

;; fn00022E38: 00022E38
;;   Called from:
;;     000229AC (in fn00022FF8)
;;     00022D18 (in fn00022D18)
;;     00022DD8 (in fn00022FA0)
;;     00022E04 (in fn00022E08)
;;     00022FEC (in fn00022FEC)
;;     00022FF0 (in fn00022FF4)
;;     00023000 (in fn00022FF8)
fn00022E38 proc
	{ r11:r10 = memd(r0++#8); r16 = #0x1 }

;; fn00022E3C: 00022E3C
;;   Called from:
;;     00022FF4 (in fn00022FF4)
fn00022E3C proc
	{ r11:r10 = memd(r0++#8) }

;; fn00022E40: 00022E40
;;   Called from:
;;     000224DC (in fn000224D8)
;;     00022E30 (in fn00022E10)
;;     00022E3C (in fn00022E3C)
fn00022E40 proc
	{ r2 = and(r2,#0x1); r2 = #0x10; r14 = #0x0; if (p0.new) jump:nt 00023108; p0 = cmp.gtu(r0,#0x0); if (p0.new) jump:nt 00022F08; p0 = cmp.gtu(r10,#0x0) }

;; fn00022E50: 00022E50
;;   Called from:
;;     000225CC (in fn00022610)
;;     00022604 (in fn000227BC)
;;     00022604 (in fn000227BC)
;;     00022630 (in fn00022610)
;;     000226C8 (in fn00022980)
;;     000226C8 (in gemsuma_asm)
;;     000227B8 (in fn000227BC)
;;     000227D8 (in fn00022610)
;;     00022864 (in fn0002254C)
;;     00022E40 (in fn00022E40)
;;     00022EC4 (in fn00022D6C)
;;     00022F58 (in fn00022F2C)
;;     000231B4 (in fn0002313C)
;;     000231C0 (in fn000231A0)
;;     000231CC (in fn000231CC)
fn00022E50 proc
	{ r13:r12 = memd(r0++#8); r2 = memb(r4+2); r8 = memuh(r0); if (p0.new) jump:t 00023158; p0 = cmp.gtu(r0,#0x1); if (p0.new) jump:t 00022F58; p0 = cmp.gtu(r11,#0x1) }

l00022E60:
	{ r2 = and(r2,#0x1); r2 = #0x10; r15 = #0x0; if (p0.new) jump:t 00023168; p0 = cmp.gtu(r0,#0x2); if (p0.new) jump:t 00022F68; p0 = cmp.gtu(r12,#0x2) }
	{ nop; nop; nop; nop }
	{ dcfetch(r0,#0x80); r2 = memb(r4+2); r8 = memuh(r0); if (p0.new) jump:t 00023188; p0 = cmp.gtu(r0,#0x3); if (p0.new) jump:t 00022F88; p0 = cmp.gtu(r13,#0x3) }

;; fn00022E88: 00022E88
;;   Called from:
;;     00023280 (in biasadd_relu_requant_nonaligned_hvx)
fn00022E88 proc
	{ dcfetch(r0,#0x80); r2 = memb(r4+2); r8 = memuh(r0) }
	{ r11:r10 = memd(r0++#8); r15:r14 += vraddub(r13:r12,r11:r10) }

;; fn00022E98: 00022E98
;;   Called from:
;;     00022CD8 (in fn0002300C)
;;     00022E90 (in fn00022E88)
fn00022E98 proc
	{ r2 = and(r2,#0x1); r2 = #0x10; if (p0.new) jump:t fn000231A0; p0 = cmp.gtu(r0,#0x0); if (p0.new) jump:t fn00022FA0; p0 = cmp.gtu(r10,#0x0) }

l00022EA4:
	{ r13:r12 = memd(r0++#8); r2 = memb(r4+2); r8 = memuh(r0); if (p0.new) jump:t 000231AC; p0 = cmp.gtu(r0,#0x1); if (p0.new) jump:t 00022FAC; p0 = cmp.gtu(r11,#0x1) }
	{ r2 = and(r2,#0x1); r2 = #0x10; if (p0.new) jump:t 000231BC; p0 = cmp.gtu(r0,#0x2); if (p0.new) jump:t fn00022FBC; p0 = cmp.gtu(r12,#0x2) }
	{ r2 = and(r2,#0x1); r8 = #0x10; if (p0.new) jump:t 000231C8; p0 = cmp.gtu(r0,#0x3); if (p0.new) jump:t 00022FC8; p0 = cmp.gtu(r13,#0x3) }

l00022EC4:
	{ r2 = and(r2,#0x1); r8 = #0x10; if (p0.new) jump:t fn000231CC; p0 = cmp.gtu(r0,#0x3) }

l00022ECC:
	{ r15:r14 += vraddub(r13:r12,r11:r10) }
	{ r14 = add(r14,r15) }
	{ r15 = mpyi(r1,r3); r14 = mpyi(r14,r3) }

;; fn00022ED8: 00022ED8
;;   Called from:
;;     000227CC (in fn000227C8)
;;     00022A50 (in fn00022D8C)
;;     00022A70 (in fn00022D8C)
;;     00022D18 (in fn00022D18)
;;     00022DE8 (in fn00022DE0)
;;     00022EDC (in fn00022D6C)
fn00022ED8 proc
	{ r15 = mpyi(r1,r3) }
	{ r1 = combine(r1.l,r1.l); r14 += mpyi(r15,r6) }
	{ if (p0.new) jump:nt 00022B30; p0 = cmp.eq(r1,#-0x1); if (p0.new) jump:nt 00022B30; p0 = cmp.eq(r14,#-0x1) }

l00022EEC:
	{ if (!p0.new) jump:nt 00022EF4; p0 = cmp.eq(r6,r4); jump 00022F34; r5 = r7 }
	{ r17:r16 = memd(r29); if (!p0.new) jump:nt 00022EFC; p0 = cmp.eq(r5,r4) }
	{ r8 = r8; r4 = #0x8 }
	{ dealloc_return }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; fn00022F10: 00022F10
;;   Called from:
;;     000232D0 (in biasadd_relu_requant_nonaligned_hvx)
fn00022F10 proc
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; fn00022F1C: 00022F1C
;;   Called from:
;;     00022F18 (in fn00022F10)
;;     00023248 (in biasadd_relu_requant_nonaligned_hvx)
fn00022F1C proc
	{ r0 = memw(r0); r0 = memw(r0) }

;; im2col33322_hvx: 00022F20
;;   Called from:
;;     00022F1C (in fn00022F1C)
im2col33322_hvx proc
	{ allocframe(#0x40) }
	{ r0 = r0; r3 = #0x0; r2 = vsplatb(r2) }

;; fn00022F2C: 00022F2C
;;   Called from:
;;     00022D6C (in fn00022D6C)
;;     00022F24 (in im2col33322_hvx)
fn00022F2C proc
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r7 = #0x20; if (p0.new) jump:nt 00022B7C; p0 = cmp.eq(r2,#-0x1) }

l00022F38:
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20 }
	{ memd(r29+40) = r27:r26; memd(r29+32) = r25:r24; r7 = #0x78; M1 = r7 }
	{ r7 = #0x1010101; immext(#0x1010100); r23 = #0x381; M0 = r7 }

l00022F58:
	{ r7 = #0x1 }

l00022F5C:
	{ r24 = #0x12A0; r8 = add(r7,r7); r10 = asl(r7,#0x3); r9 = asl(r7,#0x2) }
	{ r11 = add(r10,r10); r13 = asl(r10,#0x3); r12 = asl(r10,#0x2) }
	{ r5 = add(r5,r4); r3 = r4; r27 = add(r13,r13) }

;; fn00022F7C: 00022F7C
;;   Called from:
;;     000232EC (in biasadd_relu_requant_nonaligned_hvx)
fn00022F7C proc
	{ r5 = add(r5,r4); r3 = r4 }

;; fn00022F80: 00022F80
;;   Called from:
;;     00022F78 (in fn00022F7C)
;;     00022F7C (in fn00022F7C)
;;     000230E4 (in fn000230DC)
fn00022F80 proc
	{ r15 = add(r3,r3); r1 = add(r1,r24); r25 = and(r1,#0x60); r19 = r1 }
	{ r25 = sub(#0x0,r25); r15 = mpyi(r15,r23) }

l00022F94:
	{ r25 = sub(#0x0,r25) }

;; fn00022F98: 00022F98
;;   Called from:
;;     00022DD8 (in fn00022FA0)
;;     00022F90 (in fn00022F80)
;;     00022F94 (in fn00022D8C)
;;     00023100 (in im2col7732_asm)
;;     0002314C (in fn000238F0)
fn00022F98 proc
	{ if (!p0.new) jump:nt 00022DD8; p0 = cmp.gtu(r9,#0x9); r21 = add(r25,#0xFFFFFFF7); r22 = #0xFFFFFFE0; r15 = add(r0,r15) }

;; fn00022FA0: 00022FA0
;;   Called from:
;;     00022E98 (in fn00022E98)
fn00022FA0 proc
	{ if (!p0.new) jump:nt fn00022DE0; p0 = cmp.gtu(r9,#0x9); r21 = add(r25,#0xFFFFFFF7) }

;; fn00022FA8: 00022FA8
;;   Called from:
;;     00022FA0 (in fn00022FA0)
;;     00022FA0 (in fn00022FA0)
;;     00022FA0 (in fn00022FA0)
fn00022FA8 proc
	{ if (!p0.new) jump:nt 00022DE8; p0 = cmp.gtu(r6,#0x0); r16 = add(r15,r23) }

l00022FB0:
	{ if (!p0.new) jump:nt 00022DF4; p0 = cmp.gtu(r6,#0x1); r20 = add(r25,#0xFFFFFFEE); r17 = add(r16,r23) }

;; fn00022FB4: 00022FB4
;;   Called from:
;;     00023358 (in biasadd_relu_requant_nonaligned_hvx)
fn00022FB4 proc
	{ if (!p0.new) jump:nt fn00022DF8; p0 = cmp.gtu(r6,#0x1); r20 = add(r25,#0xFFFFFFEE) }

;; fn00022FBC: 00022FBC
;;   Called from:
;;     00022FB4 (in fn00022FB4)
;;     00022FB4 (in fn00022FB4)
;;     00022FB4 (in fn00022FB4)
fn00022FBC proc
	{ if (!p0.new) jump:nt gemvmpybbw_asm; p0 = cmp.gtu(r6,#0x2); r26 = #0x0; loop1(00022FE0,#0x8) }

l00022FC8:
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r0 = r0; r15 = #0x30 }
	{ r0 = r0; r0 = #0x31 }
	{ r0 = r0; r1 = #0x31; r18 = #0xFFFFFFE6 }

;; fn00022FEC: 00022FEC
;;   Called from:
;;     00022FF4 (in fn00022FBC)
;;     000233AC (in vmemset_asm)
fn00022FEC proc
	{ r0 = r0; r1 = #0x31 }
	{ if (!p0.new) jump:nt fn00022E38; p0 = cmp.gtu(r9,#0x4); loop0(00023020,#0x5) }

;; fn00022FF4: 00022FF4
;;   Called from:
;;     00023324 (in biasadd_relu_requant_nonaligned_hvx)
fn00022FF4 proc
	{ if (!p0.new) jump:nt fn00022E3C; p0 = cmp.gtu(r9,#0x4) }

;; fn00022FF8: 00022FF8
;;   Called from:
;;     00022FEC (in fn00022FEC)
;;     00022FF0 (in fn00022FF4)
;;     00022FF4 (in fn00022FF4)
fn00022FF8 proc
	{ if (!p0.new) jump:nt fn00022E40; p0 = cmp.gtu(r5,#0x5); if (p0.new) jump:nt 00022C88; p0 = cmp.gt(r10,#-0x1) }

l00023000:
	{ if (!p0.new) jump:nt 00022E4C; p0 = cmp.gtu(r4,#0x6); if (p0.new) jump:nt 00022C94; p0 = cmp.eq(r8,#-0x1) }
	{ jump 00022CD4; r5 = #0x25; if (!p0.new) jump:nt fn00022E50; p0 = cmp.gtu(r2,#0x4); if (p0.new) jump:nt 00022C9C; p0 = cmp.eq(r9,#-0x1) }

;; fn0002300C: 0002300C
;;   Called from:
;;     00023338 (in vmemset_asm)
fn0002300C proc
	{ jump 00022CD8; r5 = #0x25; if (!p0.new) jump:nt 00022E54; p0 = cmp.gtu(r2,#0x4) }
00023014             00 40 00 7F 00 40 00 7F 00 C0 00 7F     .@...@......
00023020 4B 40 AA 19 1A 44 1A B0 25 45 72 19 67 E6 E7 1E K@...D..%Er.g...

;; fn00023030: 00023030
;;   Called from:
;;     000233A0 (in vmemset_asm)
fn00023030 proc
	{ jump 00022CFC; r8 = #0x28; if (!p0.new) jump:nt 00022E7C; p0 = cmp.gtu(r2,#0x6); if (p0.new) jump:nt 00022CC4; p0 = cmp.gt(r8,#-0x1); if (p0.new) jump:nt 00022CC4; p0 = cmp.eq(r7,#-0x1) }

;; fn0002303C: 0002303C
;;   Called from:
;;     000231F8 (in biasadd_relu_requant_nonaligned_hvx)
fn0002303C proc
	{ jump fn00022D08; r8 = #0x28 }
00023040 4B 41 A9 19 24 44 72 19 67 65 E4 1E 07 F0 93 2B KA..$Dr.ge.....+
00023050 48 4A 00 5C                                     HJ.\            

;; fn00023054: 00023054
;;   Called from:
;;     00023440 (in gvmaccb_asm)
fn00023054 proc
	{ jump fn00022D20; r6 = #0x26; if (!p0.new) jump:nt 00022E9C; p0 = cmp.gtu(r2,#0x5); p2 = cmp.eq(r26,r24) }
00023060 4A 41 A7 19 4B 42 A8 19 26 46 72 19 27 E8 E7 1E JA..KB..&Fr.'...
00023070 4B 42 A9 19 24 44 72 19 67 65 E4 1E 07 F0 93 2B KB..$Dr.ge.....+
00023080 4B 42 AA 19 25 45 72 19 67 E6 E7 1E 4A 42 A7 19 KB..%Er.g...JB..
00023090 4B 43 A8 19 26 46 72 19 67 E8 E7 1E 4B 43 A9 19 KC..&Fr.g...KC..
000230A0 24 44 72 19 67 65 E4 1E 07 F0 93 2B 4B 43 AA 19 $Dr.ge.....+KC..
000230B0 1A 4C 1A B0 25 45 72 19 67 E6 E7 1E 4A 43 A7 19 .L..%Er.g...JC..
000230C0 4B 40 A8 19 26 46 72 19 67 E8 E7 1E 4B 80 A9 19 K@..&Fr.g...K...
000230D0 24 84 72 19 67 65 E4 1E 07 F0 93 2B             $.r.ge.....+    

;; fn000230DC: 000230DC
;;   Called from:
;;     000234D4 (in fn000234D4)
fn000230DC proc
	{ nop }
	{ r3 = add(r3,#0x1) }
	{ if (!p0.new) jump:t fn00022F80; p0 = cmp.eq(r3,r5) }

l000230E8:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return }
000230FC                                     00 00 00 00             ....

;; im2col7732_asm: 00023100
;;   Called from:
;;     00025228 (in fn00024D00)
im2col7732_asm proc
	{ allocframe(#0x40); r2 = vsplatb(r2) }

l00023108:
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r7 = #0xA0 }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20 }
	{ memd(r29+40) = r27:r26; memd(r29+32) = r25:r24; r7 = #0x20; M1 = r7 }
	{ r0 = r0; r3 = #0x0; r15 = #0xFFFFFFFE; M0 = r7 }

;; fn00023128: 00023128
;;   Called from:
;;     00023124 (in fn00022E40)
;;     000232E4 (in biasadd_relu_requant_nonaligned_hvx)
fn00023128 proc
	{ r0 = r0; r3 = #0x0; r15 = #0xFFFFFFFE }

l0002312C:
	{ r0 = r0; r3 = #0x0 }

;; fn00023130: 00023130
;;   Called from:
;;     00023128 (in fn00023128)
;;     0002312C (in biasadd_relu_requant_nonaligned_hvx)
fn00023130 proc
	{ r1 = add(r1,#0x1); r3 = #0x0; if (p0.new) jump:nt fn00022D8C; p0 = cmp.eq(r2,#-0x1); r15 += add(r4,r4) }

l00023138:
	{ r1 = add(r1,#0x1); r3 = #0x0 }

;; fn0002313C: 0002313C
;;   Called from:
;;     00023128 (in fn00023128)
;;     00023130 (in fn00023130)
;;     00023130 (in fn00023130)
fn0002313C proc
	{ r2 = and(r2,#0x1); r3 = #0x0; r16 = #0x2A0; r7 = #0x1010101; immext(#0x1010100) }
	{ r3 = add(r3,#-0x1); r3 = #0x0; r8 = add(r7,r7); r10 = asl(r7,#0x3); r9 = asl(r7,#0x2) }

l00023158:
	{ r3 = add(r3,#-0x1); r3 = #0x0 }

l0002315C:
	{ r4 = sxth(r4); r3 = #0x0; r11 = add(r10,r10); r13 = asl(r10,#0x3); r12 = asl(r10,#0x2) }
	{ r5 = sxtb(r5); r3 = #0x0; r23 = add(r4,r5); p0 = and(p0,p0); r14 = asl(r11,#0x3) }
	{ r6 = zxth(r6); r3 = #0x0; p2 = cmp.eq(r4,#0x0); if (p0.new) jump:nt fn00022E0C; p0 = cmp.eq(r8,#-0x1); r15 = mpyi(r15,r16) }

l0002318C:
	{ r7 = add(r7,#0xFF); r3 = #0x0; r0 = add(r0,r15); r22 = r4; p1 = and(p1,p1) }
	{ r6 = mux(p2,r14,r8); r18 = add(r1,#0x0); r15 = r0; r19 = #0xFFFFFFEB }

;; fn000231A0: 000231A0
;;   Called from:
;;     00022E98 (in fn00022E98)
fn000231A0 proc
	{ r6 = mux(p2,r14,r8); r18 = add(r1,#0x0); r15 = r0 }
	{ r16 = add(r15,#0x29A); r15 = add(r15,#0xFFFFFFFA); r21 = #0x0; loop1(000231C0,#0x7) }
	{ nop }
	{ r0 = r0; r15 = #0x0; r15 = add(r15,#0x60); if (p0.new) jump:nt fn00022E50; p0 = cmp.eq(r6,#-0x1) }

;; fn000231CC: 000231CC
;;   Called from:
;;     00023158 (in fn00022E50)
;;     000231C0 (in fn000231A0)
fn000231CC proc
	{ r0 = r0; r0 = #0x1; jump fn00022E0C; r15 = #0x2F; p3 = cmp.eq(r21,#0x5) }
000231D8                         10 4C 10 B0 11 40 00 78         .L...@.x
000231E0 66 4A 09 F4 22 EF E2 1E 46 46 0E F4 35 40 15 B0 fJ.."...FF..5@..
000231F0 22 C2 73 19 48 40 A7 19                         ".s.H@..        

l000231F8:
	{ if (!p0.new) jump:nt fn0002303C; p0 = cmp.gtu(r1,#0x2); r17 = #0xFFFFFFE6 }

l00023200:
	{ jump fn00022E08; r1 = #0x21; if (!p0.new) jump:nt 00023040; p0 = cmp.gtu(r1,#0x1); if (p0.new) jump:nt 00022E94; p0 = cmp.eq(r8,#-0x1); loop0(00023220,#0x4) }
00023210 00 40 00 7F 00 40 00 7F 00 40 00 7F 00 C0 00 7F .@...@...@......
00023220 4B 40 A9 19 22 42 71 19 05 F8 92 2B 4B 40 AA 19 K@.."Bq....+K@..
00023230 21 41 71 19 68 E1 E2 1E 4B 40 AB 19 22 42 71 19 !Aq.h...K@.."Bq.
00023240 08 F8 92 2B 48 40 AC 19                         ...+H@..        

l00023248:
	{ jump fn00022F1C; r1 = #0x21; if (!p0.new) jump:nt 00023088; p0 = cmp.gtu(r1,#0x1) }
00023250 4A 40 AD 19 22 42 71 19 0B E0 92 2B 4A 50 A7 19 J@.."Bq....+JP..
00023260 48 40 A7 19 21 41 71 19 4E E1 E2 1E 49 40 AE 19 H@..!Aq.N...I@..
00023270 D1 7C DF 78 22 42 71 19 0E D1 92 28 4B 80 A8 19 .|.x"Bq....(K...

l00023280:
	{ r8 = add(r8,r8); r2 = #0x39; jump fn00022E88; r1 = #0x21; if (!p0.new) jump:nt 000230C0; p0 = cmp.gtu(r1,#0x1) }
0002328C                                     0F 68 40 B0             .h@.
00023290 12 40 01 B0 06 40 68 70 15 C0 00 78 2B 42 20 69 .@...@hp...x+B i
000232A0 73 7D DF 78 4F 7F EF BF 50 D3 2F B0 00 C0 00 7F s}.xO...P./.....
000232B0 00 40 00 7F 00 40 00 7F 00 40 00 7F 00 C0 00 7F .@...@...@......
000232C0 49 50 A6 19 0F 4C 0F B0 E1 C0 0F 28 10 4C 10 B0 IP...L.....(.L..

l000232D0:
	{ r0 = r0; r0 = #0x1; jump fn00022F10; r15 = #0x2F }
000232D8                         A3 40 15 75 22 EF E2 1E         .@.u"...
000232E0 66 4A 09 F4                                     fJ..            

l000232E4:
	{ if (!p0.new) jump:nt fn00023128; p0 = cmp.gtu(r3,#0x2); r17 = #0xFFFFFFD6 }

l000232EC:
	{ if (!p0.new) jump:nt 0002312C; p0 = cmp.gtu(r1,#0x1); r21 = add(r21,#0x1); if (p0.new) jump:nt fn00022F7C; p0 = cmp.gt(r7,#-0x1); loop0(00023300,#0x4) }

l000232FC:
	{ nop }
	{ if (!p0.new) jump:nt 00023144; p0 = cmp.gtu(r1,#0x2); r17 = #0xFFFFFFE6 }
	{ jump fn00022F10; r1 = #0x21; if (!p0.new) jump:nt 00023148; p0 = cmp.gtu(r1,#0x1); if (p0.new) jump:nt 00022F9C; p0 = cmp.gt(r8,#-0x1) }
	{ r8 = add(r8,r8); r2 = #0x39; if (!p0.new) jump:nt 00023158; p0 = cmp.gtu(r1,#0x2); if (p0.new) jump:nt fn00022FA8; p0 = cmp.gt(r9,#-0x1) }
	{ jump 00022FF0; r1 = #0x21; if (!p0.new) jump:nt 00023160; p0 = cmp.gtu(r1,#0x1); if (p0.new) jump:nt fn00022FB4; p0 = cmp.gt(r10,#-0x1) }

l00023324:
	{ jump fn00022FF4; r1 = #0x21; if (!p0.new) jump:nt 00023164; p0 = cmp.gtu(r1,#0x1) }
0002332C                                     4B 51 AB 19             KQ..
00023330 22 42 71 19                                     "Bq.            

l00023334:
	{ r8 = add(r8,r8); r2 = #0x39 }
	{ jump fn0002300C; r1 = #0x21; if (!p0.new) jump:nt 00023178; p0 = cmp.gtu(r1,#0x1); if (p0.new) jump:nt 00022FCC; p0 = cmp.gt(r13,#-0x1) }
00023344             48 51 AC 19 49 51 AE 19 22 42 71 19     HQ..IQ.."Bq.

l00023350:
	{ r1 = add(r1,#0x1); r2 = #0x9 }
	{  }
	{ r0 = r0; r2 = #0x9; jump fn00022FB4; r1 = #0x21; r18 = add(r18,#0x120) }
00023364             48 91 A7 19 21 81 71 19 0E D0 92 2B     H...!.q....+
00023370 73 7D DF 78 0F 50 A0 B0                         s}.x.P..        

l00023378:
	{ r21 = #0x0; r18 = add(r1,#0x0) }
	{ r16 = add(r15,#0x29A); r15 = add(r15,#0xFFFFFFFA); r6 = mux(p1,r14,r8); loop1(000233A0,#0x7) }
	{ nop; nop; nop; nop }
	{ r0 = r0; r15 = #0x0; r15 = add(r15,#0x60); if (p0.new) jump:nt fn00023030; p0 = cmp.eq(r6,#-0x1) }

l000233AC:
	{ r0 = r0; r0 = #0x1; jump fn00022FEC; r15 = #0x2F; r16 = add(r16,#0x60) }
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
	{ r8 = r8; r2 = #0x9; jump fn00023054; r1 = #0x21 }
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
	{ jump fn000230DC; r1 = #0x21; r6 = mux(p0,r14,r6); r21 = add(r21,#0x1) }
000234E0 4B 55 A8 19 21 41 71 19 05 D9 92 28 4B 55 AA 19 KU..!Aq....(KU..
000234F0 05 F8 92 2B 4B 55 AB 19 12 50 12 B0 21 41 71 19 ...+KU...P..!Aq.
00023500 68 E1 E2 1E 48 55 AC 19 08 F8 92 2B 4A 55 AD 19 h...HU.....+JU..
00023510 49 55 AE 19 21 41 71 19 0B E1 E2 1E 0B F0 92 2B IU..!Aq........+
00023520 4A 56 A7 19 48 55 A7 19 21 41 71 19 2E E1 E2 1E JV..HU..!Aq.....

l00023530:
	{ r0 = r0; r2 = #0x39; jump 00023138; r1 = #0x21; if (p0.new) jump:nt 000231C4; p0 = cmp.gt(r9,#-0x1) }
0002353C                                     36 C0 16 B0             6...
00023540 02 42 62 6B 00 57 16 F2 01 C0 61 B4 28 50 FF 5C .Bbk.W....a.(P.\
00023550 C1 4D 56 75 00 68 40 B0 A0 CD 56 75 0D 1E 04 3E .MVu.h@...Vu...>
00023560 1F 1E 16 3E 98 40 DD 91 BA C0 DD 91 1E C0 1E 96 ...>.@..........
00023570 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; biasadd_relu_requant_nonaligned_hvx: 00023580
biasadd_relu_requant_nonaligned_hvx proc
	{ allocframe(#0x20); r14 = #0x1010101; immext(#0x1010100); r8 = add(r4,#0x7F) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r7 = r4; r15 = add(r14,r14) }
	{ r16 = #0x1; r6 = and(r4,#0x7F); if (p0.new) jump:nt 000231F8; p0 = cmp.eq(r5,#-0x1); r17 = asl(r4,#0x2) }

l000235AC:
	{ r6 = add(r6,#0xFFFFFF80); p2 = cmp.eq(r6,#0x0); r16 = combine(r17.l,r16.l); r8 = lsr(r8,#0x7) }
	{ if (p2) r6 = #0x0; r18 = addasl(r1,r4,#0x5); loop1(000235E0,r3) }
	{ l2fetch(r18,r17:r16); r10 = #0x8000; immext(#0x8000) }
	{ nop; nop; nop }
	{ r1 = add(r1,#0x1); r1 = #0x10; r9 = r2; loop0(00023600,r8) }
	{ r1 = add(r1,#0x1); r9 = #0x10; if (!p0.new) jump:nt 0002360C; p0 = cmp.eq(r0,r4); r18 = addasl(r18,r4,#0x2) }

l000235F8:
	{ r1 = add(r1,#0x1); r1 = #0x10; if (p0.new) jump:nt 00023248; p0 = cmp.eq(r10,#-0x1) }

l00023600:
	{ r1 = add(r1,#0x1); r9 = #0x10; if (!p0.new) jump:nt 00023620; p0 = cmp.eq(r1,r5); r12 = and(r0,#0x7F); if (p1.new) jump:t 00023350; p1 = cmp.eq(r15,r0) }

l0002360C:
	{ r1 = add(r1,#0x1); r9 = #0x10 }

l00023610:
	{ r1 = add(r1,#0x1); r1 = #0x10; r11 = r7; if (p0.new) jump:nt 00023260; p0 = cmp.eq(r10,#-0x1) }
	{ r1 = add(r1,#0x1); r9 = #0x10; if (!p0.new) jump:nt 00023640; p0 = cmp.eq(r2,r6); p0 = cmp.gt(r7,#0x7F); if (p1.new) jump:t 0002336C; p1 = cmp.eq(r15,r1) }

l00023620:
	{ r1 = add(r1,#0x1); r9 = #0x10; if (!p0.new) jump:nt 00023644; p0 = cmp.eq(r2,r6); p0 = cmp.gt(r7,#0x7F) }

l0002362C:
	{ r1 = add(r1,#0x1); r1 = #0x10; if (p0) r11 = #0x80; if (p0.new) jump:nt 00023280; p0 = cmp.eq(r10,#-0x1) }

l00023638:
	{ r1 = add(r1,#0x1); r9 = #0x10; if (!p0.new) jump:nt 0002365C; p0 = cmp.eq(r3,r7); jump 000232D0; r9 = r8; if (p1.new) jump:t 0002338C; p1 = cmp.eq(r15,r2) }

l00023644:
	{ r1 = add(r1,#0x1); r9 = #0x10 }

l00023648:
	{ if (p0.new) jump:nt 000232D0; p0 = cmp.eq(r0,#-0x1); r12 = add(r12,r11); if (p0.new) jump:nt 0002329C; p0 = cmp.eq(r10,#-0x1) }
	{ if (p0.new) jump:nt 000232DC; p0 = cmp.eq(r12,#-0x1); r5 = r14; r13 = sub(#0x0,r0); if (p1.new) jump:t 000233A8; p1 = cmp.eq(r15,r3) }

l0002365C:
	{ if (p0.new) jump:nt 000232E4; p0 = cmp.eq(r12,#-0x1); r5 = r14 }

l00023664:
	{ r1 = add(r1,#0x1); r1 = #0x10; if (p1.new) r5 = add(r15,#0x0); p1 = !cmp.gt(r12,0000007F) }
	{ jump 00023694; r0 = #0x0; jump 00023308; r11 = r10; r7 = add(r7,#0xFFFFFF80) }
0002367C                                     B5 41 AE 19             .A..
00023680 B4 40 AE 19 10 44 40 1C 40 C1 09 29 74 62 6F 19 .@...D@.@..)tbo.
00023690 AE CC CD 1F                                     ....            

l00023694:
	{ r1 = add(r1,#0x1); r1 = #0x10; if (p0.new) jump:nt 000232E4; p0 = cmp.eq(r10,#-0x1); if (p0.new) jump:nt 00023324; p0 = cmp.eq(r5,#-0x1) }

l000236A0:
	{ if (!p0.new) jump:nt 0002330C; p0 = cmp.gtu(r13,#0xE); if (p0.new) jump:nt 00023330; p0 = cmp.gt(r5,#-0x1) }
	{ r9 = add(r9,#0x1); r0 = #0x8 }
	{ r1 = add(r1,#0x1); r0 = #0x18; nop }
	{ r0 = add(r0,r6); r7 = r4; r1 = addasl(r1,r6,#0x2) }
	{ l2fetch(r18,r17:r16); nop; r1 = add(r1,#0xFFFFFF00) }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
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
	{ r7 = add(r2,r0); r8 = #0x1010101; immext(#0x1010100); r1 = vsplatb(r1) }
	{ if (p0.new) jump:nt 00023378; p0 = cmp.eq(r0,#-0x1); r7 = and(r7,#0x7F); r9 = add(r8,r8); if (p0.new) jump:nt 00023334; p0 = cmp.eq(r1,#-0x1) }

l00023700:
	{ if (p0.new) jump:nt 00023388; p0 = cmp.eq(r7,#-0x1); r5 = and(r0,#0x7F); if (p0.new) jump:nt 00023444; p0 = cmp.eq(r9,#-0x1); r2 -= add(r7,#0xFFFFFF81) }
	{ jump 00023730; r0 = #0x0; r5 = add(r5,r2); if (p0.new) jump:nt 00023458; p0 = cmp.gt(r9,#-0x1); r2 = lsr(r2,#0x7) }
	{ if (!p2.new) r9 = add(r8,#0x0); p2 = cmp.gt(r5,#0x7F); if (!p0.new) jump:t 000233E4; p0 = cmp.gtu(r8,#0x0); loop0(00023740,r2) }
	{ if (p0.new) jump:nt 000233C0; p0 = cmp.eq(r9,#-0x1); if (p0.new) jump:nt 000233C0; p0 = tstbit(r9,#0x0) }
	{ nop; nop }
	{ r1 = add(r1,#0x1); r0 = #0x18; jump 00023768; r0 = #0x0 }
	{ r8 = r8; r0 = #0x8 }
	{ jumpr r31 }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

l00023758:
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gvmaccimw_asm: 00023760
;;   Called from:
;;     0002375C (in fn00024D00)
gvmaccimw_asm proc
	{ r7 = memd(r29+4); r6 = memd(r29) }
	{ allocframe(#0x10); r9 = lsr(r5,#0x4) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r9 = add(r9,#0xFFFFFFFF) }
	{ nop; nop; nop }

l00023780:
	{ r6 = add(r6,#0xFFFFFFFF); loop1(000237A0,r2) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r0 = add(r0,r4); r10 = r0; loop0(000237C0,r9) }
	{ r13:r12 = memd(r10++#16); r15:r14 = memd(r10+8); r17:r16 = combine(#0x0,#0x0) }
	{ nop; nop }
	{ r13:r12 = memd(r10++#16); r15:r14 = memd(r10+8); r17:r16 += vraddub(r15:r14,r13:r12) }
	{ r17:r16 += vraddub(r15:r14,r13:r12) }
	{ r11 = memw(r1); r16 = add(r16,r17) }
	{ r11 += mpyi(r16,r7) }
	{ memw(r1++#4) = r11; nop; nop }
	{ r0 = add(r0,r3); if (!p1.new) jump:t 00023780; p1 = cmp.eq(r6,#0x0) }

l000237F0:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ dealloc_return }
000237F8                         00 00 00 00 00 00 00 00         ........

;; gvmaccb_asm: 00023800
gvmaccb_asm proc
	{ r4 = #0x1010101; immext(#0x1010100); if (p0.new) jump:nt 00023440; p0 = cmp.eq(r3,#-0x1); r2 = lsr(r2,#0x2) }

l00023810:
	{ r2 = add(r2,#0xFFFFFFFF); if (p0.new) jump:t 00023848; p0 = cmp.eq(r3,#0x0) }

l00023818:
	{ r1 = add(r1,#0x1); r0 = #0x10; r5 = #0x10; if (p0.new) jump:nt fn000238D8; p0 = cmp.gtu(r4,#0x2); loop0(00023840,r2) }

l00023828:
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r1 = add(r1,#0x1); r0 = #0x10; if (p0.new) jump:t gvmmacbbw_asm; p0 = cmp.gtu(r4,#0x2) }

l00023848:
	{ jump fn0002388C; r0 = r1 }
0002384C                                     04 C0 01 28             ...(
00023850 E3 C3 65 19 A3 E0 21 1C 04 44 43 1C 22 C0 21 28 ..e...!..DC.".!(
00023860 00 C0 9F 52 00 00 00 00 00 00 00 00 00 00 00 00 ...R............
00023870 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gvmaddvvm_asm: 00023880
gvmaddvvm_asm proc
	{ r0 = r0; r1 = #0x0; r11 = #0x80000000; immext(#0x80000000); r4 = asl(r4,#0x2) }

;; fn0002388C: 0002388C
;;   Called from:
;;     00023848 (in gvmaccb_asm)
;;     00023880 (in gvmaddvvm_asm)
fn0002388C proc
	{ r0 = r0; r1 = #0x0 }
	{ r6 = memd(r29); r3 = r3; r8 = r2; if (p0.new) jump:nt fn000234D4; p0 = cmp.eq(r11,#-0x1) }

;; fn0002389C: 0002389C
;;   Called from:
;;     0002388C (in fn0002388C)
;;     00023890 (in fn0002388C)
fn0002389C proc
	{ dcfetch(r0,#0x20); r0 = memb(r0); r5 = memuh(r0); r7 = #0x4; p2 = !cmp.eq(r6,00000000) }
	{ r10 = memw(r0++#4); if (p1.new) jump:nt fn00023934; p1 = cmp.eq(r0,#0x3); jump fn00023A70; r3 = #0x23; M0 = r4 }

l000238BC:
	{ r0 = r0; r2 = #0x30; if (!p0.new) jump:nt 000238BC; p0 = cmp.eq(r1,r4); if (p0.new) jump:nt 000234FC; p0 = cmp.eq(r10,#-0x1); loop0(000238E0,r3) }
	{ nop }
	{ nop; nop; nop; nop }

;; fn000238D8: 000238D8
;;   Called from:
;;     00023818 (in gvmaccb_asm)
fn000238D8 proc
	{ nop; nop }
	{ r0 = r0; r8 = #0x36; r10 = memw(r0++#4); if (!p0.new) jump:nt fn000238E4; p0 = cmp.eq(r1,r0); jump 000234E8; r5 = r2 }

;; fn000238E4: 000238E4
;;   Called from:
;;     000238E0 (in fn000238D8)
;;     000238F0 (in fn000238F0)
;;     000250E4 (in fn00024D00)
fn000238E4 proc
	{ r0 = r0; r8 = #0x36; r10 = memw(r0++#4); if (!p0.new) jump:nt 000238E8; p0 = cmp.eq(r1,r0) }

l000238E8:
	{ r0 = r0; r8 = #0x36; r10 = memw(r0++#4) }

;; fn000238F0: 000238F0
;;   Called from:
;;     000238E0 (in fn000238D8)
;;     000238E4 (in fn000238E4)
;;     000238E8 (in fn000238E4)
fn000238F0 proc
	{ dcfetch(r0,#0x40); r0 = memb(r4); r2 = memuh(r0); if (!p0.new) jump:nt fn000238F0; p0 = cmp.eq(r1,r4); if (p0.new) jump:nt 00023530; p0 = cmp.eq(r10,#-0x1) }

l00023900:
	{ r0 = r0; r8 = #0x6; if (!p0.new) jump:nt 00023904; p0 = cmp.eq(r1,r0); jump 00023508; r5 = r2; loop0(00023914,#0x5) }
	{ jump 0002351C; r5 = r2 }
	{ if (p1.new) jump:t 000237E0; p1 = cmp.gtu(r7,#0x6) }
	{ jump 00023524; r7 = r6; r7 = add(r7,r7) }
	{ r0 = r0; r5 = #0x2 }
	{ jumpr r31 }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; fn00023934: 00023934
;;   Called from:
;;     000238AC (in fn0002389C)
fn00023934 proc
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gvmmacbbw_asm: 00023940
;;   Called from:
;;     00023840 (in gvmaccb_asm)
;;     0002393C (in fn00023934)
gvmmacbbw_asm proc
	{ r7 = memd(r29+4); r6 = memd(r29); r5 = asl(r5,#0x2) }
	{ allocframe(#0x40); r8 = memw(r29+8) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; M0 = r5 }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r9 = lsr(r7,#0x4); r5 -= mpyi(r5,#0x3) }
	{ memd(r29+40) = r27:r26; memd(r29+32) = r25:r24; if (!p0.new) jump:nt 00023B3C; p0 = cmp.eq(r12,r12); r24 = add(r3,#0x3) }

l00023974:
	{ r9 = add(r9,#0xFFFFFFFF); r24 = lsr(r24,#0x2); M1 = r5 }

;; fn00023980: 00023980
;;   Called from:
;;     00023974 (in gvmmacbbw_asm)
;;     00023B04 (in fn00023A00)
fn00023980 proc
	{ r10 = r1; r8 = add(r8,#0xFFFFFFFF); r23 = r3; loop1(000239A0,r24) }
	{ nop; nop; nop; nop }
	{ dcfetch(r0,#0x40); r2 = memb(r0+2); r10 = memuh(r0) }
	{ dcfetch(r20,#0x40); deallocframe); r10 = memuh(r0); r20 = add(r0,r6); r21 = r0 }
	{ r0 = r0; r2 = #0x30; r15:r14 = memd(r21+8) }
	{ r0 = r0; r2 = #0x30; r13:r12 = memd(r21++#16); r22 = add(r20,r6); r11 = addasl(r20,r6,#0x1) }
	{ r0 = r0; r2 = #0x30; r17:r16 = memd(r20+8) }
	{ r0 = add(r0,r0); r2 = #0x30; r19:r18 = memd(r20++#16); r0 = addasl(r0,r6,#0x2); loop0(fn00023A00,r9) }
	{ nop; nop }
	{ nop; nop; nop; nop }

;; fn00023A00: 00023A00
;;   Called from:
;;     000239F0 (in fn00023980)
;;     00023DC0 (in gvmsumb_asm)
fn00023A00 proc
	{ dcfetch(r22,#0x40); r2 = memb(r0+2); r10 = memuh(r0); if (p0.new) jump:t 00023D00; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00023B00; p0 = cmp.gtu(r12,#0x8) }

l00023A10:
	{ dcfetch(r11,#0x40); deallocframe); r10 = memuh(r0); if (p0.new) jump:t 00023D10; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00023B10; p0 = cmp.gtu(r13,#0x9) }
	{ r13:r12 = memd(r22++#16); r19:r18 = memd(r22+8); if (p0.new) jump:t gvmsumimw_asm; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00023B20; p0 = cmp.gtu(r14,#0xA) }
	{ r15:r14 = memd(r11++#16); r17:r16 = memd(r11+8); if (p0.new) jump:t 00023D30; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00023B30; p0 = cmp.gtu(r15,#0xB) }
	{ dcfetch(r21,#0x40); r2 = memb(r0+2); r10 = memuh(r0); if (p0.new) jump:t 00023B44; p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 00023B44; p0 = cmp.gtu(r12,#0x8) }
	{ dcfetch(r20,#0x40); deallocframe); r10 = memuh(r0); if (p0.new) jump:t 00023B54; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 00023B54; p0 = cmp.gtu(r13,#0x9) }
	{ r13:r12 = memd(r21++#16); r15:r14 = memd(r21+8); if (p0.new) jump:t 00023D64; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00023D64; p0 = cmp.gtu(r2,#0xA) }

;; fn00023A70: 00023A70
;;   Called from:
;;     000238AC (in fn0002389C)
fn00023A70 proc
	{ r19:r18 = memd(r20++#16); r17:r16 = memd(r20+8); if (p0.new) jump:t fn00023D74; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t fn00023D74; p0 = cmp.gtu(r3,#0xB) }

l00023A80:
	{ dcfetch(r22,#0x40); r2 = memb(r0+2); r10 = memuh(r0); if (p0.new) jump:t 00023D80; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t fn00023B80; p0 = cmp.gtu(r12,#0x8) }
	{ dcfetch(r11,#0x40); deallocframe); r10 = memuh(r0); if (p0.new) jump:t 00023D90; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00023B90; p0 = cmp.gtu(r13,#0x9) }
	{ r13:r12 = memd(r22++#16); r19:r18 = memd(r22+8); if (p0.new) jump:t 00023DA0; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00023BA0; p0 = cmp.gtu(r14,#0xA) }
	{ r15:r14 = memd(r11++#16); r17:r16 = memd(r11+8); if (p0.new) jump:t fn00023DB0; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00023BB0; p0 = cmp.gtu(r15,#0xB) }
	{ r0 = r0; r2 = #0x32; p0 = cmp.gt(r23,#0x1); if (p0.new) jump:t 00023BC4; p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 00023BC4; p0 = cmp.gtu(r12,#0x8) }
	{ r0 = r0; r2 = #0x3A; p0 = cmp.gt(r23,#0x2); if (p0.new) jump:t 00023BD4; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 00023BD4; p0 = cmp.gtu(r13,#0x9) }
	{ if (p0.new) jump:t 00023DE4; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00023DE4; p0 = cmp.gtu(r2,#0xA) }
	{ r0 = r0; r2 = #0x3A; p0 = cmp.gt(r23,#0x3); if (p0.new) jump:t 00023DEC; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00023DEC; p0 = cmp.gtu(r3,#0xB) }
	{ r0 = r0; r2 = #0x3A; r10 = r1; r23 = add(r23,#0xFFFFFFFC) }

l00023B00:
	{ r0 = r0; r2 = #0x3A }
	{ p1 = cmp.eq(r8,#0x0); r0 = add(r0,r4); if (!p1.new) jump:t fn00023980 }

l00023B10:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return }
00023B24             00 00 00 00 00 00 00 00 00 00 00 00     ............
00023B30 00 00 00 00 00 00 00 00 00 00 00 00             ............    

l00023B3C:
	{ r0 = memw(r0); r0 = memw(r0) }

;; gvmmpybbw_asm: 00023B40
;;   Called from:
;;     00023B3C (in gvmmacbbw_asm)
gvmmpybbw_asm proc
	{ r7 = memd(r29+4); r6 = memd(r29); r5 = asl(r5,#0x2) }
	{ allocframe(#0x40); r8 = memw(r29+8); if (!p0.new) jump:nt gvmsumimw_asm; p0 = cmp.eq(r12,r12) }

l00023B54:
	{ memd(r29) = r17:r16; memd(r29+32) = r25:r24; M0 = r5 }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r24 = add(r3,#0x3); r9 = lsr(r7,#0x4) }
	{ memd(r29+40) = r27:r26; memd(r29+8) = r19:r18; r9 = add(r9,#0xFFFFFFFF); r24 = lsr(r24,#0x2) }
	{ nop }

;; fn00023B80: 00023B80
;;   Called from:
;;     00023B7C (in gvmmpybbw_asm)
;;     00023CF0 (in fn00023CD8)
fn00023B80 proc
	{ r10 = r1; r8 = add(r8,#0xFFFFFFFF); r23 = r3; loop1(00023BA0,r24) }
	{ nop; nop; nop; nop }
	{ dcfetch(r0,#0x40); r2 = memb(r0+2); r10 = memuh(r0); jump 00023D60; r12 = r12; r21 = r0 }
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
	{ r0 = r0; r2 = #0x3A; p0 = cmp.gt(r23,#0x3); if (p0.new) jump:t 00023FCC; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00023FCC; p0 = cmp.gtu(r3,#0xB) }

l00023CCC:
	{ r0 = r0; r2 = #0x3A; p0 = cmp.gt(r23,#0x3); if (p0.new) jump:t fn00023FD0; p0 = cmp.gtu(r1,#0xB) }

;; fn00023CD8: 00023CD8
;;   Called from:
;;     00023CC8 (in fn00023CC8)
;;     00023CCC (in gvconvsum2dbbb_asm)
fn00023CD8 proc
	{ r0 = r0; r2 = #0x3A; r10 = r1; r23 = add(r23,#0xFFFFFFFC) }
	{ p1 = cmp.eq(r8,#0x0); r0 = add(r0,r4); if (!p1.new) jump:t fn00023B80 }

l00023CF0:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }

l00023D00:
	{ dealloc_return }
00023D04             00 00 00 00 00 00 00 00 00 00 00 00     ............
00023D10 00 00 00 00 00 00 00 00                         ........        

;; fn00023D18: 00023D18
;;   Called from:
;;     000240D0 (in fn000240C4)
;;     000240D4 (in fn00023FD0)
fn00023D18 proc
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gvmsumimw_asm: 00023D20
;;   Called from:
;;     00023B48 (in gvmmpybbw_asm)
;;     00023D1C (in fn00023D18)
gvmsumimw_asm proc
	{ r7 = memd(r29+4); r6 = memd(r29) }
	{ allocframe(#0x10); r8 = memw(r29+8); r9 = lsr(r5,#0x4) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r9 = add(r9,#0xFFFFFFFF) }
	{ nop; nop }

;; fn00023D40: 00023D40
;;   Called from:
;;     00023BA0 (in fn00023B80)
;;     00023D38 (in gvmsumimw_asm)
;;     00023D70 (in fn00023D74)
;;     00023DA8 (in fn00023D98)
;;     00023DA8 (in fn00023D98)
;;     00023DB0 (in fn00023DB0)
;;     00024180 (in fn00024180)
;;     000241A0 (in gvconvsum2dbbb_asm)
fn00023D40 proc
	{ r6 = add(r6,#0xFFFFFFFF); loop1(00023D60,r2) }
	{ nop; nop }
	{ nop; nop; nop; nop }

l00023D60:
	{ r0 = add(r0,r4); r10 = r0; loop0(00023D80,r9) }
	{ r13:r12 = memd(r10++#16); r15:r14 = memd(r10+8); r17:r16 = combine(#0x0,#0x0) }

l00023D70:
	{ r13:r12 = memd(r10++#16); r15:r14 = memd(r10+8) }

;; fn00023D74: 00023D74
;;   Called from:
;;     00023A70 (in fn00023A70)
;;     00023A70 (in fn00023A70)
fn00023D74 proc
	{ r13:r12 = memd(r10++#16) }

;; fn00023D78: 00023D78
;;   Called from:
;;     00023D6C (in fn00023D40)
;;     00023D74 (in fn00023D74)
fn00023D78 proc
	{ nop; nop }
	{ r13:r12 = memd(r10++#16); r15:r14 = memd(r10+8); r17:r16 += vraddub(r15:r14,r13:r12) }
	{ r17:r16 += vraddub(r15:r14,r13:r12) }

l00023D90:
	{ r11 = r8; r16 = add(r16,r17) }

;; fn00023D98: 00023D98
;;   Called from:
;;     00023D90 (in fn00024180)
;;     00024180 (in fn00024180)
fn00023D98 proc
	{ r11 += mpyi(r16,r7) }
	{ memw(r1++#4) = r11; nop; nop }
	{ r0 = add(r0,r3); if (!p1.new) jump:t fn00023D40; p1 = cmp.eq(r6,#0x0) }

;; fn00023DB0: 00023DB0
;;   Called from:
;;     00023DA8 (in fn00023D98)
;;     00023DA8 (in fn00023D98)
fn00023DB0 proc
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }

l00023DB4:
	{ dealloc_return }
00023DB8                         00 00 00 00 00 00 00 00         ........

;; gvmsumb_asm: 00023DC0
;;   Called from:
;;     0001A5B4 (in supernode_execute_hvx_slice)
gvmsumb_asm proc
	{ p0 = cmp.eq(r3,#0x0); if (p0.new) jump:nt fn00023A00; p0 = cmp.eq(r3,#-0x1); r2 = lsr(r2,#0x2) }

l00023DCC:
	{ r2 = r2; r4 = #0x1010101; immext(#0x1010100); if (p0) jump:nt 00023E08 }

l00023DD8:
	{ r1 = add(r1,#0x1); r0 = #0x10; r5 = #0x10; if (p0.new) jump:nt fn00023E98; p0 = cmp.gtu(r4,#0x2); loop0(00023E00,r2) }

l00023DE8:
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r1 = add(r1,#0x1); r0 = #0x10; if (p0.new) jump:t 00023F00; p0 = cmp.gtu(r4,#0x2) }

l00023E08:
	{ jump fn00023E4C; r0 = r1 }
00023E0C                                     E3 C3 65 19             ..e.
00023E10 A3 60 21 1C 22 C0 21 28 00 C0 9F 52 00 00 00 00 .`!.".!(...R....

;; gvconvsum2dbbw_asm: 00023E20
gvconvsum2dbbw_asm proc
	{ dcfetch(r0,#0x0); r8 = memw(r29) }
	{ dcfetch(r0,#0x20); r6 = memw(r29+8); r5 = asl(r5,#0x2) }
	{ allocframe(#0x48); r11 = memw(r29+20) }
	{ memd(r29) = r17:r16; memd(r29+32) = r25:r24; r25 = lsr(r8,#0x10) }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r8 = mpy(r8.h,r8.l) }

;; fn00023E4C: 00023E4C
;;   Called from:
;;     00023E08 (in gvmsumb_asm)
;;     00023E48 (in gvconvsum2dbbw_asm)
fn00023E4C proc
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20 }
	{ memd(r29+40) = r27:r26; memd(r29+8) = r19:r18; M0 = r8 }
	{ r0 = r0; r11 = #0x0; r16 = #0x80000001; immext(#0x80000000); r12 = addasl(r8,r8,#0x1) }
	{ memw(r29+56) = r4; r12 = sub(#0x10,r12); if (p0.new) jump:nt fn00023CC8; p0 = cmp.eq(r0,#-0x1); r23 = mpyi(r6,r3) }

l00023E70:
	{ memw(r29+56) = r4; r12 = sub(#0x10,r12); if (p0.new) jump:nt 00023CCC; p0 = cmp.eq(r0,#-0x1) }

;; fn00023E7C: 00023E7C
;;   Called from:
;;     00023E6C (in fn00023E4C)
;;     00023E6C (in fn00023E4C)
;;     00023E70 (in gvconvsum2dbbb_asm)
fn00023E7C proc
	{ r6 = memw(r29+84); M1 = r12 }
	{ memw(r29+52) = r1; memw(r29+48) = r0; r12 = add(r12,#0x10); r13 = asl(r8,#0x1) }
	{ memw(r29+60) = r5; r13 = sub(r6,r3); r23 = sub(r23,r13); r6 = lsr(r6,#0x4) }

;; fn00023E98: 00023E98
;;   Called from:
;;     00023DD8 (in gvmsumb_asm)
;;     00023E9C (in fn00023E7C)
fn00023E98 proc
	{ memw(r29+60) = r5; r13 = sub(r6,r3) }
	{ memw(r29+64) = r28; r16 = memw(r29+104); p3 = cmp.gt(r8,#0x60); r6 = add(r6,#0xFFFFFFFF) }
	{ memw(r29+84) = r6; r0 = memb(r0); r0 = memuh(r1+2); if (p3) r12 = sub(r12,r8); r3 = mpyi(r3,r25) }
	{ memw(r29+92) -= #0x1; r11 = memw(r29+48) }
	{ memw(r29+48) += r3; r22 = memw(r29+56); r28 = add(r11,#0x40) }
	{ nop; nop; nop }
	{ r6 = memw(r29+88); r9 = memw(r29+52); r7 = #0x0 }
	{ dcfetch(r28,#0x0); r2 = memb(r0+2); r9 = memuh(r0); jump fn000240AC; r15 = r15 }
00023EF8                         10 44 26 60 1C 48 1C F3         .D&`.H..

l00023F00:
	{ r3:r2 = combine(r7,#0x0); r9 = #0x0; jump fn000240C4; r15 = r15 }
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
00023FA0 38 54 52 EA 82 E8 14 19 00 40 00 7F 00 C0 00 7F 8TR......@......
00023FB0 00 40 00 7F 00 40 00 7F 00 40 00 7F 00 C0 00 7F .@...@...@......
00023FC0 3A 4E 50 EA 83 68 0E 19 08 42 09 29             :NP..h...B.)    

l00023FCC:
	{ dcfetch(r28,#0x0) }

;; fn00023FD0: 00023FD0
;;   Called from:
;;     00023CCC (in gvconvsum2dbbb_asm)
;;     00023FCC (in fn00023CC8)
fn00023FD0 proc
	{ r3:r2 = combine(r7,#0x0); r9 = #0x0; r28 = add(r28,r8); if (p0.new) jump:t 000240D4; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 000242D4; p0 = cmp.gtu(r5,#0x9) }

l00023FE0:
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); if (p0.new) jump:t 000242E4; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 000242E4; p0 = cmp.gtu(r2,#0xA) }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t 000242F4; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000242F4; p0 = cmp.gtu(r3,#0xB) }
	{ dcfetch(r28,#0x0); r28 = add(r28,r8); if (p0.new) jump:t 00024300; p0 = cmp.gtu(r4,#0x8); r1:r0 += vraddub(r15:r14,r21:r20) }
	{ r2 = and(r2,#0x1); r9 = #0x10; r7 = add(r7,#0x1); if (p0.new) jump:t 00024310; p0 = cmp.gtu(r2,#0x8); r5:r4 += vraddub(r17:r16,r19:r18) }
	{ r7:r6 = combine(r7,#0x0); r9 = #0x0; p3 = cmp.eq(r7,#0x2); if (p0.new) jump:t 00024320; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00024320; p0 = cmp.gtu(r5,#0x9) }
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8); if (p0.new) jump:t 00024330; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024130; p0 = cmp.gtu(r14,#0xA) }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t 00024340; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024140; p0 = cmp.gtu(r15,#0xB) }
	{ if (p3) r28 = add(r28,r12); if (p3) r7 = #0x0; if (p0.new) jump:t 00024354; p0 = cmp.gtu(r4,#0x8); r25:r24 += vraddub(r19:r18,r21:r20) }
	{ r2 = and(r2,#0x1); r9 = #0x10; r11 = sub(r11,r13); if (p0.new) jump:t 00024164; p0 = cmp.gtu(r14,#0x8); r27:r26 += vraddub(r17:r16,r15:r14) }

;; fn00024070: 00024070
;;   Called from:
;;     00024214 (in gvconv2dbbw_asm)
fn00024070 proc
	{ dcfetch(r11,#0x40); deallocframe); r9 = memuh(r0); if (p0.new) jump:t fn00024174; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t fn00024374; p0 = cmp.gtu(r5,#0x9) }

l00024080:
	{ r7 = #0x0; r28 = add(r11,#0x40); if (p0.new) jump:t 00024384; p0 = cmp.gtu(r2,#0xA) }

l00024088:
	{ r7 = #0x0 }

;; fn0002408C: 0002408C
;;   Called from:
;;     00024088 (in fn00024460)
;;     00024488 (in fn00024374)
fn0002408C proc
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); r28 = add(r28,r8); if (p0.new) jump:t fn00024390; p0 = cmp.gtu(r0,#0xA) }

;; fn0002409C: 0002409C
;;   Called from:
;;     0002408C (in fn0002408C)
;;     0002408C (in fn0002408C)
fn0002409C proc
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t fn000243A0; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t fn000243A0; p0 = cmp.gtu(r3,#0xB) }

;; fn000240A4: 000240A4
;;   Called from:
;;     0002448C (in fn00024374)
fn000240A4 proc
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }

;; fn000240AC: 000240AC
;;   Called from:
;;     00023EEC (in fn00023E98)
;;     00023EF8 (in fn00023E98)
;;     0002409C (in fn000240A4)
;;     000240A4 (in fn000240A4)
;;     00024468 (in fn00024460)
fn000240AC proc
	{ r6 = memw(r29+112); r14 = memw(r29+108); r11 = sub(r11,r23) }
	{ r10 = memw(r29+96); r1 = add(r0,r1); r0 = r6 }

;; fn000240C4: 000240C4
;;   Called from:
;;     00023F00 (in gvmsumb_asm)
;;     000240B8 (in fn000240AC)
;;     000244AC (in fn000243A0)
fn000240C4 proc
	{ r15 = memw(r29+60); r28 = add(r11,#0x40); r0 += mpyi(r14,r1) }
	{ memw(r10++#4) = r0; if (p0.new) jump:nt fn00023D18; p0 = cmp.eq(r0,#-0x1) }

l000240D4:
	{ memw(r10++#4) = r0 }

;; fn000240D8: 000240D8
;;   Called from:
;;     00023CF8 (in fn00023CD8)
;;     000240D0 (in fn000240C4)
;;     000240D4 (in fn00023FD0)
fn000240D8 proc
	{ r0 = r0; r2 = #0x6; if (!p0.new) jump:nt fn000240D8; p0 = cmp.eq(r4,r0); p0 = cmp.gt(r22,#0x1); r2 = add(r2,r15) }

l000240E8:
	{ dcfetch(r11,#0x0); jump 00023D00; r12 = r0; r5 = add(r4,r5); r4 = r6 }
000240F8                         04 45 0E EF 04 C0 0B 94         .E......
00024100 25 40 A4 19 08 E4 8A AB 02 4F 02 FB 41 40 56 75 %@.......O..A@Vu
00024110 01 41 45 1C 52 C0 E2 28 18 40 66 70 19 59 18 F3 .AE.R..(.@fp.Y..
00024120 01 CE 20 1A 18 D9 0E EF 26 40 B8 19 0C 4C 21 1F .. .....&@...L!.
00024130 09 F8 8A AB 22 4F 02 FB 60 40 56 75 02 42 46 1C ...."O..`@Vu.BF.
00024140 52 C8 E2 28 1A 40 66 70 1B 5B 1A F3 22 CE 20 1A R..(.@fp.[..". .
00024150 1A DB 0E EF 27 40 BA 19 0C 4C 22 1F 08 FA 8A AB ....'@...L".....

l00024160:
	{ r0 = r0; r2 = #0xE; if (!p0.new) jump:nt 00024164; p0 = cmp.eq(r7,r3); if (p0) r2 = add(r2,r15) }

l00024164:
	{ r0 = r0; r2 = #0xE; if (!p0.new) jump:nt 00024168; p0 = cmp.eq(r7,r3) }

l00024168:
	{ r0 = r0; r2 = #0xE }

l0002416C:
	{ memw(r29+96) = r10; if (p1.new) jump:nt 00023D70; p1 = cmp.eq(r0,#0xE); r22 = add(r22,#0xFFFFFFFC) }

;; fn00024174: 00024174
;;   Called from:
;;     00024070 (in fn00024070)
fn00024174 proc
	{ memw(r29+96) = r10 }

;; fn00024178: 00024178
;;   Called from:
;;     00024174 (in fn00024174)
;;     00024174 (in fn00024174)
;;     00024174 (in fn00024174)
;;     000244F0 (in gvconvsum2dbbb_asm)
fn00024178 proc
	{ jump 00023D90; r12 = r3; p2 = cmp.gt(r22,#0x0); if (p2.new) jump:t 00023EE0 }

;; fn00024180: 00024180
;;   Called from:
;;     00023D84 (in fn00023D78)
;;     000244A8 (in fn000243A0)
fn00024180 proc
	{ jump fn00023D98; r12 = r3 }
00024184             E9 C2 9D 91 9C 59 FF 5C                 .....Y.\    

l0002418C:
	{ p1 = cmp.eq(r9,#0x0) }
	{ r16 = memd(r29+104); r6 = #0x4; loop0(00024198,#0x5) }
	{ if (!p1.new) jump:t 00023E70; p1 = cmp.gtu(r6,#0xC) }

l0002419C:
	{ jump 00023DB4; r13 = r12; r6 = add(r6,r6) }
000241A4             0C C0 30 28 0D 1E 04 3E 1F 1E 16 3E     ..0(...>...>
000241B0 98 40 DD 91 BA C0 DD 91 1C 42 9D 91 1E C0 1E 96 .@.......B......

;; gvconv2dbbw_asm: 000241C0
gvconv2dbbw_asm proc
	{ r7 = memd(r29+4); r6 = memd(r29) }
	{ r9 = memw(r29+12); r8 = memw(r29+8); r5 = asl(r5,#0x2) }
	{ r11 = memw(r29+20); r10 = memw(r29+16) }
	{ allocframe(#0x40); r12 = memw(r29+24) }
	{ memd(r29) = r17:r16; memd(r29+32) = r25:r24; r25 = lsr(r6,#0x10) }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r6 = mpy(r6.h,r6.l) }
	{ memd(r29+40) = r27:r26; memd(r29+8) = r19:r18; M0 = r6 }
	{ memw(r29+52) = r1; memw(r29+48) = r0 }
	{ r0 = r0; r11 = #0x0; r16 = #0x80000001; immext(#0x80000000); r1 = addasl(r6,r6,#0x1) }
	{ memw(r29+56) = r4; r1 = sub(#0x10,r1); if (p0.new) jump:nt fn00024070; p0 = cmp.eq(r0,#-0x1); r23 = mpyi(r8,r3) }

l00024224:
	{ p3 = cmp.gt(r6,#0x60); r1 = add(r1,#0x10); r13 = asl(r6,#0x1); M1 = r1 }
	{ memw(r29+60) = r5; r13 = sub(r7,r3); r23 = sub(r23,r13); r7 = lsr(r7,#0x4) }
	{ r0 = r0; r12 = #0x0; if (p3) r1 = sub(r1,r6); r7 = add(r7,#0xFFFFFFFF); r3 = mpyi(r3,r25) }
	{ nop; nop; nop }
	{ r11 = memw(r29+48); r9 = add(r9,#0xFFFFFFFF) }
	{ memw(r29+48) += r3; r22 = memw(r29+56); r26 = add(r11,#0x40) }
	{ nop; nop; nop }
	{ r24 = memw(r29+52); r27 = #0x0 }
	{ dcfetch(r26,#0x0); r2 = memb(r0+2); r8 = memuh(r1+2); loop1(000242C0,r8) }
	{ r3:r2 = combine(r7,#0x0); r8 = #0x1; r26 = add(r26,r6); loop0(00024320,r7) }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); jump fn00024460; r15 = r15 }
000242AC                                     E2 4F 4F 1F             .OO.
000242B0 30 40 CB 91 12 C0 CB 9D 00 40 00 7F 00 C0 00 7F 0@.......@......
000242C0 80 68 14 19 81 68 12 19 0A 42 18 29 00 C0 1A 94 .h...h...B.)....
000242D0 80 69 15 19                                     .i..            

l000242D4:
	{ r7:r6 = combine(r7,#0x0); r8 = #0x1; r26 = add(r26,r6); if (p0.new) jump:t fn000245D4; p0 = cmp.gtu(r3,#0x9) }

l000242E0:
	{ r27 = add(r27,#0x1); p3 = cmp.eq(r27,#0x1); if (p0.new) jump:t fn000245E0; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 000243E0; p0 = cmp.gtu(r14,#0xA) }

l000242F0:
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8); if (p3) r26 = add(r26,r1); if (p3) r27 = #0x0 }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t 00024600; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024400; p0 = cmp.gtu(r15,#0xB) }
	{ nop; nop; nop; nop }
	{ dcfetch(r26,#0x0); r2 = memb(r0+2); r8 = memuh(r1+2); if (p0.new) jump:t 00024424; p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 00024624; p0 = cmp.gtu(r4,#0x8) }
	{ r3:r2 = combine(r7,#0x0); r8 = #0x1; r26 = add(r26,r6); if (p0.new) jump:t 00024434; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 00024634; p0 = cmp.gtu(r5,#0x9) }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); if (p0.new) jump:t 00024644; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024644; p0 = cmp.gtu(r2,#0xA) }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t 00024654; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024654; p0 = cmp.gtu(r3,#0xB) }
	{ dcfetch(r26,#0x0); r2 = memb(r0+2); r8 = memuh(r1+2); if (p0.new) jump:t 00024660; p0 = cmp.gtu(r2,#0x8); if (p0.new) jump:t 00024660; p0 = cmp.gtu(r4,#0x8) }
	{ r7:r6 = combine(r7,#0x0); r8 = #0x1; r26 = add(r26,r6); if (p0.new) jump:t 00024670; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00024670; p0 = cmp.gtu(r5,#0x9) }

;; fn00024374: 00024374
;;   Called from:
;;     00024070 (in fn00024070)
fn00024374 proc
	{ r7:r6 = combine(r7,#0x0); r8 = #0x1; r26 = add(r26,r6); if (p0.new) jump:t 00024674; p0 = cmp.gtu(r3,#0x9) }

l00024380:
	{ r19:r18 = memd(r11+8); r27 = add(r27,#0x1); p3 = cmp.eq(r27,#0x1); if (p0.new) jump:t 00024480; p0 = cmp.gtu(r14,#0xA) }

;; fn00024390: 00024390
;;   Called from:
;;     0002408C (in fn0002408C)
;;     0002408C (in fn0002408C)
;;     00024380 (in fn00024374)
fn00024390 proc
	{ r21:r20 = memd(r11++m0); if (p3) r26 = add(r26,r1); if (p3) r27 = #0x0; if (p0.new) jump:t fn00024690; p0 = cmp.gtu(r0,#0xA) }

;; fn000243A0: 000243A0
;;   Called from:
;;     000240AC (in fn000240A4)
;;     00024390 (in fn00024390)
fn000243A0 proc
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t fn000246A0; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000244A0; p0 = cmp.gtu(r15,#0xB) }

l000243B0:
	{ r2 = and(r2,#0x1); r8 = #0x11; r11 = sub(r11,r13); if (p0.new) jump:t 000244B4; p0 = cmp.gtu(r14,#0x8); if (p0.new) jump:t 000246B4; p0 = cmp.gtu(r4,#0x8) }
	{ r26 = add(r11,#0x40); r27 = #0x0; if (p0.new) jump:t 000244C4; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 000246C4; p0 = cmp.gtu(r5,#0x9) }
	{ dcfetch(r26,#0x0); deallocframe); r8 = memuh(r1); r26 = add(r26,r6); loop0(00024320,r7) }

l000243E0:
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); if (p0.new) jump:t fn000246E4; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t fn000246E4; p0 = cmp.gtu(r2,#0xA) }

l000243F0:
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t 000246F4; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000246F4; p0 = cmp.gtu(r3,#0xB) }
	{ r0 = memw(r10++#4); r11 = sub(r11,r23) }
	{ r5 = memw(r29+60); r26 = add(r11,#0x40); if (p0.new) jump:nt 00024050; p0 = cmp.eq(r0,#-0x1) }
	{ r0 = r0; r2 = #0x6; if (!p0.new) jump:nt 00024414; p0 = cmp.eq(r4,r0); p0 = cmp.gt(r22,#0x1); r2 = add(r2,r5) }
	{ if (p0) r0 = memw(r10++#4); jump 0002403C; r12 = r0 }
	{ dcfetch(r11,#0x0); if (p0.new) jump:nt 00024074; p0 = cmp.eq(r0,#-0x1) }
	{ r0 = r0; r2 = #0xE; if (!p0.new) jump:nt 00024434; p0 = cmp.eq(r5,r1); p1 = cmp.gt(r22,#0x2); if (p0) r2 = add(r2,r5) }
	{ if (p1) r0 = memw(r10++#4); if (p1.new) jump:nt 00024044; p1 = cmp.eq(r0,#0xE) }
	{ dcfetch(r11,#0x20); jump 00024064; r12 = r1; if (p0.new) jump:nt 00024098; p0 = cmp.eq(r0,#-0x1) }
	{ r8 = r8; r2 = #0xE; if (!p0.new) jump:nt 0002445C; p0 = cmp.eq(r6,r2); p0 = cmp.gt(r22,#0x3); if (p1) r2 = add(r2,r5) }

;; fn00024460: 00024460
;;   Called from:
;;     000242A0 (in gvconv2dbbw_asm)
fn00024460 proc
	{ r8 = r8; r2 = #0xE; if (!p0.new) jump:nt 00024464; p0 = cmp.eq(r6,r2) }

l00024464:
	{ r8 = r8; r2 = #0xE }

l00024468:
	{ if (p0) r0 = memw(r10++#4); if (p1.new) jump:nt fn000240AC; p1 = cmp.eq(r0,#0xE) }

l00024470:
	{ jump 00024088; r12 = r2; r22 = add(r22,#0xFFFFFFFC); if (p0.new) jump:nt 000240BC; p0 = cmp.eq(r0,#-0x1) }
0002447C                                     02 45 02 FB             .E..

l00024480:
	{ r0 = r0; r2 = #0xE; if (!p0.new) jump:nt 00024484; p0 = cmp.eq(r7,r3) }

l00024484:
	{ r0 = r0; r2 = #0xE }

l00024488:
	{ if (p1.new) jump:nt fn0002408C; p1 = cmp.eq(r0,#0xE) }

l0002448C:
	{ jump fn000240A4; r12 = r3; p2 = cmp.gt(r22,#0x0); if (p2.new) jump:t 00024280 }
00024498                         E4 59 FF 5C 01 C0 09 75         .Y.\...u

l000244A0:
	{ r6 = #0x4; loop0(000244A8,#0x5) }
	{ if (!p1.new) jump:t fn00024180; p1 = cmp.gtu(r6,#0xC) }

l000244AC:
	{ jump fn000240C4; r13 = r12; r6 = add(r6,r6) }
000244B4             0C C0 2C 28 0D 1E 04 3E 1F 1E 16 3E     ..,(...>...>
000244C0 98 40 DD 91 BA C0 DD 91 1E C0 1E 96 00 00 00 00 .@..............
000244D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gvconvsum2dbbb_asm: 000244E0
;;   Called from:
;;     0001A65C (in supernode_execute_hvx_slice)
gvconvsum2dbbb_asm proc
	{ dcfetch(r0,#0x0); r9 = #0x20; r8 = #0x1010101; immext(#0x1010100) }
	{ if (p0.new) jump:nt fn00024178; p0 = cmp.eq(r9,#-0x1); r6 = #0x8000; immext(#0x8000); r9 = #0x40 }

l00024500:
	{ if (p0.new) jump:nt 0002418C; p0 = cmp.eq(r9,#-0x1); r9 = #0x60; if (p0.new) jump:nt 00024160; p0 = cmp.eq(r6,#-0x1); if (p0.new) jump:nt 00024264; p0 = cmp.eq(r8,#-0x1) }

l00024510:
	{ jump 00024538; r2 = #0x2; if (p0.new) jump:nt 0002419C; p0 = cmp.eq(r9,#-0x1); r8 = add(r8,r8) }
	{ jump 00024530; r3 = #0x3; jump 00024548; r3 = #0x3; r8 = add(r8,r8); if (!p0.new) jump:t 00024200; p0 = cmp.gtu(r8,#0x1) }
	{ dcfetch(r0,#0x20); r8 = add(r8,r8); if (!p0.new) jump:t 00024210; p0 = cmp.gtu(r8,#0x2) }
	{ r6 = memw(r29+4); r8 = memw(r29); if (!p0.new) jump:t 0002421C; p0 = cmp.gtu(r8,#0x3) }
	{ dcfetch(r0,#0x20); p0 = cmp.eq(r6,#0x1); r6 = mpy(r8.l,r6.l) }
	{ memw(r29+4) = r6 }
	{ r14 = memw(r29+36); r6 = memw(r29+8) }
	{ r0 = r0; r14 = #0x0; r15 = memw(r29+40) }
	{ allocframe(#0x48); r11 = memw(r29+20); if (p0.new) jump:nt 000241C4; p0 = cmp.eq(r15,#-0x1) }
	{ memd(r29) = r17:r16; memd(r29+32) = r25:r24; r25 = lsr(r8,#0x10) }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r8 = mpy(r8.h,r8.l) }
	{ memd(r29+40) = r27:r26; memd(r29+8) = r19:r18; r21 = #0x60; M0 = r8 }
	{ r0 = r0; r11 = #0x0; r16 = #0x80000001; immext(#0x80000000); r12 = addasl(r8,r8,#0x1) }
	{ memw(r29+56) = r4; r12 = sub(#0x10,r12); if (p0.new) jump:nt 00024400; p0 = cmp.eq(r0,#-0x1); r23 = mpyi(r6,r3) }
	{ r6 = memw(r29+84); if (p0) r21 = add(r21,#0xFFFFFFE0); M1 = r12 }
	{ memw(r29+68) = r21; p3 = cmp.gt(r6,#0xC0); r21 = asl(r8,#0x2) }
	{  }

l000245D0:
	{ memw(r29+52) = r1; memw(r29+48) = r0; r12 = add(r12,#0x10); r13 = asl(r8,#0x1) }

;; fn000245D4: 000245D4
;;   Called from:
;;     000242D4 (in fn00023FD0)
;;     000245D0 (in fn000249B0)
;;     000249BC (in fn000246BC)
fn000245D4 proc
	{ memw(r29+52) = r1; memw(r29+48) = r0; r12 = add(r12,#0x10) }

l000245DC:
	{ memw(r29+60) = r5; r13 = sub(r6,r3); r23 = sub(r23,r13); r6 = lsr(r6,#0x4) }

;; fn000245E0: 000245E0
;;   Called from:
;;     000242E0 (in fn00023FD0)
;;     000245DC (in fn000245D4)
fn000245E0 proc
	{ memw(r29+60) = r5; r13 = sub(r6,r3); r23 = sub(r23,r13) }
	{ memw(r29+64) = r28; r16 = memw(r29+104); p3 = cmp.gt(r8,#0x60); r6 = add(r6,#0xFFFFFFFF) }
	{ memw(r29+84) = r6; r0 = memb(r0); r0 = memuh(r1+2); r12 = sub(r12,r8); r3 = mpyi(r3,r25) }
	{ dcfetch(r0,#0x40); r21 = memw(r29+68) }
	{ nop; nop; nop }
	{ memw(r29+92) -= #0x1; r11 = memw(r29+48) }
	{ memw(r29+48) += r3; r22 = memw(r29+56); r28 = add(r11,r21) }
	{ nop; nop; nop }
	{ r6 = memw(r29+88); r9 = memw(r29+52); r7 = #0x0 }
	{ dcfetch(r28,#0x0); r2 = memb(r0+2); r9 = memuh(r0); jump fn0002480C; r15 = r15 }
00024658                         10 44 26 60 1C 48 1C F3         .D&`.H..

l00024660:
	{ r3:r2 = combine(r7,#0x0); r9 = #0x0; jump 00024824; r15 = r15 }
00024668                         00 40 00 7C 04 40 00 7C         .@.|.@.|
00024670 2E 40 CB 91                                     .@..            

l00024674:
	{ r21:r20 = memd(r11++m0) }

;; fn00024678: 00024678
;;   Called from:
;;     000245D4 (in fn000245D4)
;;     00024674 (in fn00024374)
;;     00024824 (in fn00024A00)
;;     000249EC (in fn000249EC)
;;     000249F0 (in fn000249EC)
;;     00024A10 (in fn00024A00)
;;     00024A10 (in fn00024A00)
fn00024678 proc
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8); r27:r26 = combine(#0x0,#0x0); r25:r24 = combine(#0x0,#0x0) }
	{ nop; nop }

l0002468C:
	{ nop }

;; fn00024690: 00024690
;;   Called from:
;;     00024390 (in fn00024390)
;;     0002468C (in fn000249B0)
fn00024690 proc
	{ nop; nop; nop; nop }

;; fn000246A0: 000246A0
;;   Called from:
;;     000243A0 (in fn000243A0)
;;     00024690 (in fn00024690)
fn000246A0 proc
	{ dcfetch(r28,#0x0); r6 = memw(r29+84); r28 = add(r28,r8); r1:r0 += vraddub(r15:r14,r21:r20) }
	{ p3 = cmp.eq(r7,#0x1); if (p0.new) jump:t fn000249B0; p0 = cmp.gtu(r4,#0x8); loop0(00024720,r6) }

;; fn000246BC: 000246BC
;;   Called from:
;;     000246B0 (in fn000246A0)
;;     000246B0 (in fn000246A0)
fn000246BC proc
	{ r2 = and(r2,#0x1); r9 = #0x10; if (p3) r28 = add(r28,r12); if (p0.new) jump:t 000249BC; p0 = cmp.gtu(r2,#0x8); r5:r4 += vraddub(r17:r16,r19:r18) }

l000246CC:
	{ r7:r6 = combine(r7,#0x0); r9 = #0x0; r7 = sub(#0x1,r7); if (p0.new) jump:t 000249CC; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 000249CC; p0 = cmp.gtu(r5,#0x9) }

l000246DC:
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8); if (p0.new) jump:t 000249DC; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 000247DC; p0 = cmp.gtu(r14,#0xA) }

;; fn000246E4: 000246E4
;;   Called from:
;;     000243E0 (in fn00023FD0)
;;     000243E0 (in fn00023FD0)
fn000246E4 proc
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8) }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t fn000249EC; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000247EC; p0 = cmp.gtu(r15,#0xB) }

l000246FC:
	{ if (p0.new) jump:t fn00024A00; p0 = cmp.gtu(r4,#0x8); r25:r24 += vraddub(r19:r18,r21:r20) }
	{ nop; nop; nop }
	{ nop; nop; nop; nop }
	{ dcfetch(r28,#0x0); r2 = memb(r0+2); r9 = memuh(r0); if (p0.new) jump:t 00024824; p0 = cmp.gtu(r14,#0x8); r27:r26 += vraddub(r17:r16,r15:r14) }
	{ r3:r2 = combine(r7,#0x0); r9 = #0x0; r28 = add(r28,r8); if (p0.new) jump:t 00024834; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t 00024A34; p0 = cmp.gtu(r5,#0x9) }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); if (p0.new) jump:t 00024A44; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024A44; p0 = cmp.gtu(r2,#0xA) }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t 00024A54; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024A54; p0 = cmp.gtu(r3,#0xB) }
	{ dcfetch(r28,#0x0); r28 = add(r28,r8); if (p0.new) jump:t 00024A60; p0 = cmp.gtu(r4,#0x8); r1:r0 += vraddub(r15:r14,r21:r20) }
	{ r2 = and(r2,#0x1); r9 = #0x10; p3 = cmp.eq(r7,#0x1); if (p0.new) jump:t 00024A70; p0 = cmp.gtu(r2,#0x8); r5:r4 += vraddub(r17:r16,r19:r18) }
	{ dcfetch(r28,#0x0); deallocframe); r9 = memuh(r0); if (p0.new) jump:t 00024A80; p0 = cmp.gtu(r3,#0x9); if (p0.new) jump:t 00024A80; p0 = cmp.gtu(r5,#0x9) }
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8); if (p0.new) jump:t 00024A90; p0 = cmp.gtu(r0,#0xA); if (p0.new) jump:t 00024890; p0 = cmp.gtu(r14,#0xA) }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t 00024AA0; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 000248A0; p0 = cmp.gtu(r15,#0xB) }
	{ if (p3) r28 = add(r28,r12); r7 = sub(#0x1,r7); if (p0.new) jump:t 00024AB4; p0 = cmp.gtu(r4,#0x8); r25:r24 += vraddub(r19:r18,r21:r20) }
	{ r2 = and(r2,#0x1); r9 = #0x10; r11 = sub(r11,r13); if (p0.new) jump:t 000248C4; p0 = cmp.gtu(r14,#0x8); r27:r26 += vraddub(r17:r16,r15:r14) }
	{ r3:r2 = combine(r7,#0x0); r9 = #0x0; r21 = memw(r29+68); if (p0.new) jump:t 000248D4; p0 = cmp.gtu(r15,#0x9); if (p0.new) jump:t fn00024AD4; p0 = cmp.gtu(r5,#0x9) }
	{ dcfetch(r11,#0x40); r7 = #0x0; r28 = add(r11,r21); if (p0.new) jump:t 00024AE4; p0 = cmp.gtu(r2,#0xA) }

l000247EC:
	{ dcfetch(r11,#0x40) }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); r28 = add(r28,r8); if (p0.new) jump:t fn00024AF4; p0 = cmp.gtu(r0,#0xA) }

l00024800:
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8); if (p0.new) jump:t 00024B04; p0 = cmp.gtu(r1,#0xB); if (p0.new) jump:t 00024B04; p0 = cmp.gtu(r3,#0xB) }

;; fn0002480C: 0002480C
;;   Called from:
;;     00024640 (in fn000245E0)
;;     0002464C (in fn000245E0)
fn0002480C proc
	{ r19:r18 = memd(r11++m0) }

l00024810:
	{ r6 = memw(r29+112); r14 = memw(r29+108); p0 = cmp.gt(r22,#0x1); r11 = sub(r11,r23) }
	{ r10 = memw(r29+96); r20 = #0x1; r1 = add(r0,r1); r0 = r6 }

l00024824:
	{ r10 = memw(r29+96); r20 = #0x1; r1 = add(r0,r1) }

l00024830:
	{ r21 = memw(r29+68); r15 = memw(r29+60); r16 = extractu(r2,#0x2,#0x2); r0 += mpyi(r14,r1) }
	{ memw(r10++#4) = r0; jump 00024A10; r1 = #0x31; if (p0.new) jump:nt 00024488; p0 = cmp.eq(r0,#-0x1); r16 &= asl(r20,r16) }
00024850 F0 40 50 8C 04 40 66 70 05 45 04 F3 00 C0 44 1C .@P..@fp.E....D.
00024860 04 45 0E EF 48 53 B0 19 0C 4C 20 1F 00 C0 52 1C .E..HS...L ...R.
00024870 25 40 A4 19 34 40 08 7A 41 40 56 75 E9 F1 03 1E %@..4@.zA@Vu....
00024880 A8 60 30 1C 1C 55 0B F3 01 41 45 1C 08 E4 8A AB .`0..U...AE.....
00024890 18 40 66 70 19 59 18 F3 05 41 52 1C 01 CE 20 1A .@fp.Y...AR... .
000248A0 18 59 0E EF 48 48 E8 1F 0C 4C 21 1F EA F1 03 1E .Y..HH...L!.....
000248B0 A9 65 30 1C 62 40 56 75 07 4F 02 F3 09 F8 8A AB .e0.b@Vu.O......
000248C0 B1 42 07 8D 26 40 B8 19 A8 48 C8 1F 00 C0 0B 94 .B..&@...H......
000248D0 02 4F 02 F3 49 49 E9 1F 02 42 46 1C 08 C0 C2 28 .O..II...BF....(
000248E0 91 51 54 C6 06 42 52 1C 22 4E 20 1A 04 C0 0B 94 .QT..BR."N .....
000248F0 F1 40 51 8C 1A 40 66 70 1B 5B 1A F3 A9 C9 C9 1F .@Q..@fp.[......

l00024900:
	{ jump fn00024AD4; r1 = #0x31; jump 00024518; r12 = r2; if (p0.new) jump:t 00024854; p0 = cmp.eq(r0,r6); r26 += mpyi(r14,r27) }
00024910 27 40 BA 19 49 53 B1 19 08 C0 0B 94 34 40 88 7A '@..IS......4@.z
00024920 07 4F 07 FB 03 43 47 1C 09 C8 C2 28 02 4F 02 FB .O...CG....(.O..
00024930 4A 4A EA 1F 07 43 52 1C 0A FA 8A AB B2 42 07 8D JJ...CR......B..
00024940 AB 67 30 1C 27 4F 07 FB 18 CA 9D A1 B3 42 07 8D .g0.'O.......B..
00024950 92 52 54 C6 34 40 08 7B AA CA CA 1F F2 40 52 8C .RT.4@.{.....@R.
00024960 93 53 54 C6 4B 4B EB 1F 43 CE 20 1A F3 40 53 8C .ST.KK..C. ..@S.
00024970 4A 53 B2 19 96 7F F6 BF 0C CC 23 1F 4B 53 B3 19 JS........#.KS..
00024980 22 4F 02 FB AB 4B CB 1F 0A D0 C2 28 5A 5B DF 5C "O...K.....(Z[.\
00024990 42 4F 02 FB 03 40 56 75 0B D8 C2 28 E9 42 9D 91 BO...@Vu...(.B..
000249A0 35 C2 9D 91 3E 59 FF 5C 01 C0 09 75 31 40 00 69 5...>Y.\...u1@.i

;; fn000249B0: 000249B0
;;   Called from:
;;     00024678 (in fn00024678)
;;     000246B0 (in fn000246A0)
;;     000246B0 (in fn000246A0)
;;     000246BC (in fn000246BC)
fn000249B0 proc
	{ r16 = memd(r29+104); r6 = #0x4 }
	{ if (!p1.new) jump:t 0002468C; p1 = cmp.gtu(r6,#0xC) }

l000249B8:
	{ jump 000245D0; r13 = r12; r6 = add(r6,r6) }

l000249BC:
	{ jump fn000245D4; r13 = r12 }
000249C0 0C C0 30 28 0D 1E 04 3E 1F 1E 16 3E             ..0(...>...>    

l000249CC:
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return; r28 = memw(r29+64) }
000249DC                                     00 00 00 00             ....

;; gvconv2dbbb_asm: 000249E0
;;   Called from:
;;     0001A6C4 (in supernode_execute_hvx_slice)
gvconv2dbbb_asm proc
	{ dcfetch(r0,#0x0); r9 = #0x20; r8 = #0x1010101; immext(#0x1010100) }

;; fn000249EC: 000249EC
;;   Called from:
;;     000246EC (in fn000246E4)
;;     000249E0 (in gvconv2dbbb_asm)
fn000249EC proc
	{ dcfetch(r0,#0x0) }
	{ if (p0.new) jump:nt fn00024678; p0 = cmp.eq(r9,#-0x1); r6 = #0x8000; immext(#0x8000); r9 = #0x40 }

;; fn00024A00: 00024A00
;;   Called from:
;;     000249EC (in fn000249EC)
;;     000249F0 (in fn000249EC)
fn00024A00 proc
	{ if (p0.new) jump:nt 0002468C; p0 = cmp.eq(r9,#-0x1); r9 = #0x60; if (p0.new) jump:nt 00024660; p0 = cmp.eq(r6,#-0x1); if (p0.new) jump:nt 00024764; p0 = cmp.eq(r8,#-0x1) }

l00024A10:
	{ dcfetch(r0,#0x20); jump 00024A38; r2 = #0x2; if (p0.new) jump:nt 0002469C; p0 = cmp.eq(r9,#-0x1); r8 = add(r8,r8) }
	{ jump 00024A34; r3 = #0x3; jump 00024A4C; r3 = #0x3; r8 = add(r8,r8); if (!p0.new) jump:t 00024704; p0 = cmp.gtu(r8,#0x1) }
	{ dcfetch(r0,#0x40); r8 = add(r8,r8); if (!p0.new) jump:t 00024714; p0 = cmp.gtu(r8,#0x2) }
	{ r7 = memd(r29+4); r6 = memd(r29); if (!p0.new) jump:t 00024720; p0 = cmp.gtu(r8,#0x3) }
	{ r9 = memw(r29+12); r8 = memw(r29+8); p0 = cmp.eq(r7,#0x1) }
	{ r11 = memw(r29+20); r10 = memw(r29+16); r7 = mpy(r7.l,r6.l) }
	{ r14 = memw(r29+28); r12 = memw(r29+24) }
	{ r0 = r0; r14 = #0x0; r15 = memw(r29+32); p3 = cmp.gt(r7,#0xC0) }
	{ allocframe(#0x48); r28 = #0x60; if (p0.new) jump:nt 000246D0; p0 = cmp.eq(r15,#-0x1) }
	{ memw(r29+68) = r28; if (p0) r28 = add(r28,#0xFFFFFFE0) }
	{ memd(r29) = r17:r16; memd(r29+32) = r25:r24; r25 = lsr(r6,#0x10) }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r6 = mpy(r6.h,r6.l) }
	{ memd(r29+40) = r27:r26; memd(r29+8) = r19:r18; M0 = r6 }
	{ memw(r29+52) = r1; memw(r29+48) = r0; r17 = asl(r6,#0x2) }
	{ if (!p3) r28 = add(r17,#0x0) }
	{ r0 = r0; r11 = #0x0; r16 = #0x80000001; immext(#0x80000000); r1 = addasl(r6,r6,#0x1) }
	{ memw(r29+56) = r4; r1 = sub(#0x10,r1); if (p0.new) jump:nt 0002491C; p0 = cmp.eq(r0,#-0x1); r23 = mpyi(r8,r3) }
	{ r1 = add(r1,#0x10); r13 = asl(r6,#0x1); M1 = r1 }

;; fn00024AD4: 00024AD4
;;   Called from:
;;     00024900 (in fn00024D00)
fn00024AD4 proc
	{ r1 = add(r1,#0x10); r13 = asl(r6,#0x1) }
	{ memw(r29+60) = r5; r13 = sub(r7,r3); r23 = sub(r23,r13); r7 = lsr(r7,#0x4) }
	{ r0 = r0; r12 = #0x0; r1 = sub(r1,r6); r7 = add(r7,#0xFFFFFFFF); r3 = mpyi(r3,r25) }

;; fn00024AF4: 00024AF4
;;   Called from:
;;     000247F0 (in fn000246E4)
fn00024AF4 proc
	{ r0 = r0; r12 = #0x0; r1 = sub(r1,r6) }

;; fn00024AFC: 00024AFC
;;   Called from:
;;     00024AEC (in fn00024AD4)
;;     00024AF4 (in fn00024AF4)
fn00024AFC proc
	{ nop }
	{ r11 = memw(r29+48); r9 = add(r9,#0xFFFFFFFF) }

l00024B04:
	{ r11 = memw(r29+48) }

;; fn00024B08: 00024B08
;;   Called from:
;;     00024B00 (in fn00024AFC)
;;     00024B00 (in fn00024AFC)
;;     00024B04 (in fn000246E4)
fn00024B08 proc
	{ memw(r29+48) += r3; r22 = memw(r29+56); r26 = add(r11,r28) }
	{ nop; nop; nop }
	{ r24 = memw(r29+52); p2 = !cmp.eq(r2,r2) }
	{ dcfetch(r26,#0x0); r2 = memb(r0+2); r8 = memuh(r1+2); loop1(00024B60,r8) }
	{ r3:r2 = combine(r7,#0x0); r8 = #0x1; r26 = add(r26,r6); loop0(00024BC0,r7) }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); jump fn00024D00; r15 = r15 }
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

;; fn00024D00: 00024D00
;;   Called from:
;;     00024B40 (in fn00024B08)
;;     00024B40 (in fn00024B08)
fn00024D00 proc
	{ if (p1.new) jump:nt 00024900; p1 = cmp.eq(r0,#0xE); if (!p0.new) jump:nt 00024F08; p0 = cmp.eq(r2,r1) }

l00024D08:
	{ if (p2) r0 = memw(r10++#4); if (!p0.new) jump:nt 00024D0C; p0 = cmp.eq(r6,r2); jump 00024920; r12 = r1; jump 00024998; r8 = r8 }
	{ dcfetch(r11,#0x20); if (p1.new) jump:nt 0002495C; p1 = cmp.eq(r0,#0xE); if (!p0.new) jump:nt 00024F24; p0 = cmp.eq(r2,r2); if (p0.new) jump:t 00024C68; p0 = cmp.eq(r0,r5) }
	{ jump 00024EFC; r1 = #0x31; jump 00024E78; r8 = r8; r21 = add(r2,r5); if (p0.new) jump:nt 00024974; p0 = cmp.eq(r0,#-0x1) }
	{ r17 = mux(p0,#0x1,#0x10); if (p0.new) jump:t 00024C8C; p0 = cmp.eq(r0,r6); r18 = extractu(r21,#0x2,#0x2) }
	{ if (!p0.new) jump:nt 00024D48; p0 = cmp.eq(r7,r3); jump 0002495C; r12 = r2; jump 000249D4; r9 = r9; r18 &= asl(r17,r18) }
	{ r0 = r0; r2 = #0xC; if (!p0.new) jump:nt 00024F60; p0 = cmp.eq(r2,r3); jump 000249E8; r10 = r10; r18 = vsplatb(r18) }
	{ dcfetch(r11,#0x40); jump 00024EB4; r9 = r9; r2 = add(r2,r5); if (p0.new) jump:nt 00024BF4; p0 = tstbit(r2,#0x0) }
	{ if (p1.new) jump:nt 000249F8; p1 = cmp.eq(r0,#0xE); if (p0) r21 = add(r21,r5); r22 = add(r22,#0xFFFFFFFC); if (p0.new) jump:t 00024CC8; p0 = cmp.eq(r0,r7) }
	{ jump 0002499C; r12 = r3; if (p1) r21 = add(r21,r5); r17 = mux(p1,#0x1,#0x10); r19 = extractu(r21,#0x2,#0x2) }
	{ jump 00024A28; r11 = r11; r17 = mux(p2,#0x1,#0x10); r19 &= asl(r17,r19); r20 = extractu(r21,#0x2,#0x2) }
	{ r8 = r8; r2 = #0xC; jump 00024EF8; r10 = r10; r20 &= asl(r17,r20); r19 = vsplatb(r19) }
	{ jump 00024F08; r11 = r11; if (p0) r2 = add(r2,r5); if (p0.new) jump:nt 00024C48; p0 = tstbit(r3,#0x0); r20 = vsplatb(r20) }
	{ r0 = r0; r2 = #0xC; if (p1) r2 = add(r2,r5); if (p0.new) jump:nt 00024C58; p0 = tstbit(r4,#0x0) }
	{ r8 = r8; r2 = #0xC; p3 = cmp.gt(r22,#0x0); if (p2) r2 = add(r2,r5); if (p3.new) jump:t 00024B20 }
	{ p1 = cmp.eq(r9,#0x0); if (!p1.new) jump:t 00024B00 }
	{ r6 = #0x4; loop0(00024DF0,#0x5) }
	{ if (!p1.new) jump:t 00024AC8; p1 = cmp.gtu(r6,#0xC) }
	{ jump 00024A0C; r13 = r12; r6 = add(r6,r6) }
	{ r0 = r0; r12 = #0x2 }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ r28 = memw(r29+68) }
	{ dealloc_return }
