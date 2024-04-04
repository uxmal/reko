;;; Segment .text (00009840)

l00019850:
	{ r7:r6 = vaslw(r25:r24,00000002) }
	{ if (!cmp.gtu(r3.new,r2)) jump:t 00019874; r3 = and(00000081,asl(r3,00000004)) }

l00019860:
	{ memw(r29+4) = r2; r1 = 000000F7; r3 = add(PC,0000003B) }
	{ memw(r29) = r7; r2 = r23 }
	{ jump 00019904 }

l00019874:
	{ r2 = memw(r29+124) }
	{ if (!cmp.gtu(r25,r4.new)) jump:t 0001989C; r4 = memw(r2+20); r5 = r2 }

l00019888:
	{ memw(r29+4) = r25; r1 = 000000FA; r3 = add(PC,0000002D) }
	{ memw(r29) = r4; r2 = r23 }
	{ jump 00019904 }

l0001989C:
	{ r2 = memw(r29+52); r1 = 000000FC; r3 = add(PC,0000ED6F) }
	{ if (!cmp.eq(r2.new,00000002)) jump:t 00019900; r2 = memw(r2) }

l000198B8:
	{ r2 = memw(r29+52); r1 = 000000FD; r3 = add(PC,00000028) }
	{ if (!cmp.eq(r2.new,00000002)) jump:t 00019900; r2 = memw(r2+12) }

l000198D0:
	{ r2 = memd(r29+48); r4 = 00000004; r3 = add(PC,00000021) }
	{ r1 = 000000FE }
	{ if (cmp.gtu(r4,r2.new)) jump:t 00019900; r2 = memw(r2+20) }

l000198E8:
	{ r2 = memw(r29+44); r1 = 000000FF; r3 = add(PC,00000017) }
	{ if (cmp.gtu(r2.new,00000006)) jump:t 00019910; r2 = memw(r2+20); p0 = cmp.gt(r24,00000000) }

l00019900:
	{ r2 = r23 }

l00019904:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00019CD4 }

l00019910:
	{ memw(r5+24) = r25; r2 = memw(r29+68); r0 = p0 }
	{ memw(r5) = r2; r23 = memd(r29+56); r2 = 00000000 }
	{ memd(r29+112) = r7:r6; r3 = memd(r29+84) }
	{ memw(r5+8) = r20; memw(r5+4) = r17; if (p0) r20 = add(r24,00000000) }
	{ memw(r29+20) = r0; memw(r5+12) = r24; if (!p0) jump:nt 00019974 }

l0001993C:
	{ r26 = memw(r29+40); r18 = max(r18,r2) }
	{ r2 = memb(r23++#1); r19 = r3 }
	{ r2 = sub(r2,r18) }
	{ r2 = convert_w2sf(r2) }
	{ r20 = add(r20,FFFFFFFF); call fn00009620; r0 = sfmpy(r3,r2) }
	{ memw(r26++#4) = r2.new; p0 = cmp.eq(r20,00000000); r3 = r19; r2 = convert_uw2sf(r0):chop }

l00019974:
	{ r1:r0 = memd(r29+112); r2 = memd(r29+40); r9 = r17; r14 = 00000000 }
	{ r18 = memd(r29+108); r3 = memd(r29+68) }
	{ r13 = memw(r29+60); r0 = memw(r29+20); p0 = cmp.gt(r3,00000000); r2 += add(r0,0000007F) }
	{ if (p0) r3 = memw(r29-128); if (p0) r12 = memw(r29+64); r2 = and(r2,FFFFFF80); p1 = r0 }
	{ memw(r29+92) = r2; if (p0) r8 = add(r9,FFFFFFFF); if (p0) r2 = sub(r22,r16); if (!p0) jump:nt 00019BD0 }

l000199B4:
	{ r5 = memd(r29+104); r4 = r18; p2 = cmp.gt(r12,000000FF); p3 = cmp.gt(r12,FFFFFFFF) }
	{ r1 = memw(r29+88); r14 = 00000000; r6 = 000000FF; r7 = mpyi(r22,r21) }
	{ r2 = memd(r29+76); r5 = 00000000; r3 = sub(r3,r13); r4 = add(r2,mpyi(r4,r5)) }
	{ p0 = cmp.gt(r1,000000FF); r4 += lsr(r4,0000001F); r7 = mpyi(r7,r24) }
	{ memb(r29+31) = r0.new; if (p0) r2 = add(r6,00000000); r0 = p3; r8 = add(r3,mpyi(r8,r2)) }
	{ r0 = memw(r29+124); if (!p2) r6 = zxtb(r12); p3 = cmp.gt(r1,FFFFFFFF) }
	{ memw(r29+52) = r5; r3 = 00000000; p0 = r0; r8 += lsr(r8,0000001F) }
	{ if (!p0) r6 = add(r3,00000000); if (!p3) r2 = add(r3,00000000); r5 = mpyi(r21,r24) }
	{ memb(r29+22) = r1.new; r1 = asr(r4,00000001) }
	{ memw(r29+56) = r3 }

l00019A30:
	{ if (p0.new) r3 = memw(r29+52); p0 = cmp.gt(r9,00000000); r15 = 00000000; if (!p0.new) jump:nt 00019BC0 }

l00019A40:
	{ r12 = mpyi(r3,r13) }
	{ memb(r29+16) = r3.new; r3 = mpyi(r3,r9) }

l00019A4C:
	{ if (p0.new) memw(r29+84) = r15; r9 = 00000000; if (p0.new) jump:nt 00019A60; p0 = cmp.gt(r10,00000000) }
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
	{ if (!cmp.eq(r15.new,r9)) jump:t 00019A4C; r15 = add(r15,00000001) }

l00019BC0:
	{ r3 = memd(r29+68); r4 = memd(r29+52) }

l00019BC4:
	{ r4 = add(r4,00000001) }
	{ memw(r29+52) = r4; if (!p0.new) jump:nt 00019A30; p0 = cmp.eq(r4,-00000001) }

l00019BD0:
	{ r2 = memw(r29+24); r16 = r9 }
	{ r19 = add(r14,r2) }
	{ call fn00009760 }
	{ r2 = memd(r29+28); r3 = memd(r29+20); r12 = r16 }
	{ r14 = memw(r29+40); r13 = memw(r29+36); p1 = r3; r4 = mpyi(r12,r18) }
	{ if (p0.new) r5 = memw(r29+68); if (!p0.new) jump:nt 00019C74; p0 = cmp.gt(r2,00000000) }

l00019C08:
	{ r3:r2 = combine(00000100,00000000); r4 = mpyi(r4,r5) }
	{ loop1(00019C18,r4) }
	{ if (p1) r5:r4 = combine(r13,r14) }
	{ if (p1) r7 = memw(r29+92); if (!p1) jump:nt 00019C68 }

l00019C24:
	{ r6 = mpyi(r3,r24); loop0(00019C30,r24) }
	{ r6 = addasl(r7,r6,00000002) }
	{ r8 = memw(r4++#4); r7 = 00000000 }
	{ r9 = memw(r6++#4) }
	{ r8 = add(r9,r8) }
	{ r8 = add(00008000,mpyi(r8,r0)) }
	{ if (!tstbit(r8.new,-00000001)) jump:nt 00019C5C; r8 = asrh(r8) }

l00019C54:
	{ if (!p0.new) r7 = 000000FF; p0 = cmp.gt(r2,r8) }

l00019C5C:
	{ memb(r5++#1) = r7; nop }
	{ r13 = add(r13,r24) }

l00019C68:
	{ nop; nop; r3 = add(r3,00000001) }

l00019C74:
	{ r6 = memd(r29+44); r4 = memd(r29+48); r1 = 00000152; r3 = convert_w2sf(r19) }
	{ r5 = memw(r29+32) }
	{ memw(r4+12) = 00000001; r2 = memw(r4+16) }
	{ memw(r4+4) = 00000001; memw(r4+8) = 00000001; r3 = 00000002; r5 = sfmpy(r5,r3) }
	{ memw(r2) = 00000000; memw(r4) = 00000001 }
	{ memw(r6) = 00000001; memw(r4+24) = 00000004; r4 = add(PC,0000E9B1) }
	{ memw(r6+8) = 00000001; r2 = memw(r6+16) }
	{ memw(r6+4) = 00000001; memw(r6+12) = 00000001 }
	{ memw(r6+24) = 00000004; memw(r2) = r5 }
	{ memw(r29+12) = r24; r2 = memw(r29+72) }
	{ memw(r29+8) = r18; r5 = memd(r29+68) }
	{ memw(r29) = r5; memw(r29+4) = r12 }
	{ call logmsg_function }
	{ r0 = 00000000 }

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
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00019D08; r5 = memw(r2+16) }

l00019CF8:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,0000000C) }
	{ memw(r29+4) = r6; call logv }

l00019D08:
	{ dealloc_return }

;; errlog_function: 00019D0C
;;   Called from:
;;     00019040 (in supernode_execute_hvx)
;;     00019418 (in supernode_check_ref)
;;     00019904 (in supernode_execute_ref)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000E7F4) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; supernode_execute_hvx_slice: 00019D30
supernode_execute_hvx_slice proc
	{ allocframe(+00000138); memd(r29-16) = r17:r16; r16 = r1 }
	{ memd(r29+272) = r25:r24; r3 = memw(r16) }
	{ r5 = memw(r16+4) }
	{ memd(r29+280) = r23:r22 }
	{ memw(r29+248) = r5; r2 = memw(r3+4) }
	{ r17 = memb(r3+32) }
	{ memd(r29+288) = r21:r20; memd(r29+296) = r19:r18 }
	{ r1 = memw(r2+4); r24 = memw(r2); p0 = cmp.eq(r17,00000000) }
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
	{ if (p0.new) r0 = add(r2,r27); if (p0.new) jump:nt 00019DEC; p0 = cmp.eq(r9,00000004) }

l00019DDC:
	{ r0 = 00000000; if (!p0.new) jump:nt 00019DF4; p0 = cmp.eq(r9,00000002) }

l00019DE4:
	{ r0 = r27 }
	{ r0 += add(r23,FFFFFFFF) }

l00019DEC:
	{ r1 = r27; call fn00009760 }

l00019DF4:
	{ if (p0.new) r1 = add(r18,00000000); r26 = r27; r25 = r0; p0 = cmp.eq(r17,00000002) }
	{ memb(r29+59) = r0.new; if (p0) r2 = sub(r21,r19); if (p0) jump:nt 00019E3C; r0 = p0 }

l00019E18:
	{ r1:r0 = combine(r18,r18) }
	{ r1 = memw(r29+220); r0 = 00000000 }
	{ if (!p0) r1:r0 = combine(r18,r21); if (!p0.new) jump:nt 00019E44; p0 = r1 }

l00019E30:
	{ jump 00019E40 }
00019E34             E0 5F 15 E2 06 C0 00 58                 ._.....X    

l00019E3C:
	{ r0 = add(r2,r18) }

l00019E40:
	{ call fn00009760 }

l00019E44:
	{ r4 = memw(r29+256); r3 = memw(r29+240); r7 = sub(r20,r23); r6 = add(r0,FFFFFFFF) }
	{ memw(r29+116) = r23; r2 = memw(r22+16); r17 = r19; r22 = add(r25,FFFFFFFF) }
	{ r4 = memw(r4+8); r3 = memw(r3+16); r23 = r6; r22 = add(r7,mpyi(r22,r26)) }
	{ memw(r29+240) = r6; r6 = memw(r29+224) }
	{ r2 = memw(r2); r1 = memw(r29+228) }
	{ r7 = memd(r29+72); r19 = memd(r29+64); r22 += lsr(r22,0000001F) }
	{ r27 = memw(r3); r9 = add(r19,0000001F); r3 = sub(r17,r21); r7 = mpyi(r19,r7) }
	{ r5 = memw(r1+16); r4 = memw(r4); r7 = mpyi(r7,r25); r23 = add(r3,mpyi(r23,r18)) }
	{ r8 = memw(r24+16); r6 = memw(r6+16); r24 = r0; r0 = sfsub(r2,r27) }
	{ memw(r29+128) = r26 }
	{ memw(r29+160) = r8; r8 = memw(r16+20); r23 += lsr(r23,0000001F) }
	{ memw(r29+56) = r16; r3 = memw(r4+16) }
	{ memw(r29+228) = r9; memw(r29+108) = r21 }
	{ memw(r29+196) = r8 }
	{ memw(r29+224) = r7; r26 = memw(r5) }
	{ r16 = memw(r6) }
	{ memw(r29+60) = r3; call fmaxf.1.0 }
	{ r2 = 437F0000 }
	{ r21 = 00000000; r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = sfsub(r21,r27) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r2 = 00000000; r0 = sfsub(r26,r16); r18 = convert_uw2sf(r0):chop }
	{ p1 = cmp.gt(r18,000000FF) }
	{ if (p1) r18 = FFFFFFFF; p0 = cmp.gt(r18,FFFFFFFF) }
	{ memb(r29+33) = r2.new; r2 = 00000000; if (!p0) r18 = add(r2,00000000); call fmaxf.1.0 }
	{ r2 = 00000000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = sfsub(r21,r16) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r2 = convert_uw2sf(r0):chop }
	{ nop; if (!p0.new) jump:t 00019F68; p0 = tstbit(r2,00000000) }

l00019F58:
	{ if (!p0.new) r3 = zxtb(r2); if (p0.new) r3 = 000000FF; p0 = cmp.gt(r2,000000FF) }
	{ memw(r29+132) = r3 }

l00019F68:
	{ r9 = memw(r29+128); r6 = memw(r29+244); p0 = cmp.eq(r17,00000007); r2 = mpyi(r20,r17) }
	{ memb(r29+41) = r8.new; r5 = memw(r29+112); r8 = asr(r23,00000001); r4 = add(00000003,mpyi(r24,r25)) }
	{ p1 = cmp.eq(r5,00000003); p2 = cmp.eq(r9,00000002); r0 = p0 }
	{ memw(r29+220) = r0; r2 = memw(r29+224); r7 = and(r3,FFFFFFF0); r21 = mpyi(r2,r6) }
	{ r0 = memw(r29+220); r5 = add(r5,0000000F); p0 = cmp.eq(r7,00000020); p3 = cmp.eq(r7,000000A0) }
	{ r4 = and(r4,3FFFFFFC); p0 = fastcorner9(p2,p0); r1 = mpyi(r24,r25) }
	{ memb(r29+26) = r6.new; r12 = memw(r29+228); p3 = fastcorner9(p2,p3); r6 = asr(r22,00000001) }
	{ r8 = r25; r5 = cmp.eq(r21,r7); r26 = and(r5,FFFFFFF0) }
	{ memw(r29+76) = r1; r6 = and(r18,000000FF); p0 = fastcorner9(p1,p0); r2 = mpyi(r2,r24) }
	{ r13 = and(r12,FFFFFFE0); r23 = r9; r3 = asl(r19,00000002); p1 = r0 }
	{ r25 = r24; p2 = cmp.eq(r17,00000003); r4 = asl(r4,00000002); p1 = fastcorner9(p1,p2) }
	{ memw(r29+96) = r5; memw(r29+244) = r13; p2 = cmp.eq(r20,00000007); p0 = fastcorner9(p2,p0) }
	{ p1 = cmp.eq(r20,00000003); p2 = fastcorner9(p2,p1) }
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
	{ r22 = memd(r29+116); r5 = 00000000; r0 = p3 }
	{ memw(r29+120) = r0; memw(r29+200) = r5; if (p3) jump:nt 0001A0A8 }

l0001A08C:
	{ memb(r29+50) = r5.new; r0 = memw(r29+120); r5 = mux(p0,00000001,00000000) }

l0001A090:
	{ memb(r29+50) = r5.new; r0 = memw(r29+120) }

l0001A09C:
	{ if (p0.new) memw(r29-20) = r6; if (!p0.new) r6 = memw(r29-92); if (!p0.new) jump:nt 0001A0D0 }

l0001A0A8:
	{ memw(r29+236) = r6; r5 = r25; r16 = r7; r6 = asl(r17,00000001) }
	{ memw(r29+232) = r8; r5 += add(r6,00000002); r7 = mpyi(r8,r16) }
	{ jump 0001A0E8; r5 = mpyi(r7,r5) }
0001A0CC                                     A5 3D 78 50             .=xP

l0001A0D0:
	{ memw(r29+232) = r8; r5 = add(r5,r22) }
	{ r6 = add(r6,r17); r5 = mpyi(r5,r26) }
	{ r6 = addasl(r24,r6,00000001) }
	{ r5 = mpyi(r5,r6) }

l0001A0E8:
	{ r6 = memw(r29+252) }
	{ r6 = memw(r6+4) }
	{ memw(r29+188) = r6; r6 += add(r3,0000007F) }
	{ memb(r29+42) = r3.new; r3 = and(r6,FFFFFF80) }
	{ r5 = memw(r29+256); r18 = and(r3,FFFFFF80) }
	{ r7 = add(r18,0000017F) }
	{ memb(r29+46) = r6.new; r5 = memw(r5+40); r6 = and(r7,FFFFFF80) }
	{ r27 = memw(r5) }
	{ r4 = memw(r29+244) }
	{ r19 = and(r6,FFFFFF80); p0 = cmp.eq(r19,r4) }
	{ r7 = add(r19,0000017F) }
	{ memb(r29+17) = r0.new; r3 = and(r7,FFFFFF80); r0 = p0 }
	{ r2 = memw(r29+60); r3 = and(r3,FFFFFF80) }
	{ if (p0) r3 = add(r2,00000000) }
	{ memw(r29+204) = r3; call fn00009530 }
	{ p0 = cmp.eq(r21,r16); r2 = mpyi(r24,r22) }
	{ memd(r29+48) = r1:r0; r3 = memd(r29+112) }
	{ r2 = mpyi(r2,r3) }
	{ memw(r29+80) = r2; if (!p0) jump:nt 0001A17C }

l0001A170:
	{ r0 = memd(r29+120); r4 = r23 }
	{ if (!p0.new) jump:nt 0001A1A0; p0 = r0 }

l0001A17C:
	{ r22 = memw(r29+232); r20 = 00000001; r26 = r16; r24 = r25 }
	{ r7 = 00000000; r4 = 00000001; r2 = 00000000; r17 = 00000001 }
	{ memw(r29+164) = r7; memw(r29+104) = r2 }

l0001A1A0:
	{ memw(r29+224) = r20; r2 = memw(r29+104); r3 = mpyi(r26,r20) }
	{ memw(r29+116) = r22; r0 = 00060000; r2 = add(r22,r2) }
	{ memw(r29+192) = r26; memw(r29+228) = r4; r2 = mpyi(r2,r26) }
	{ memw(r29+108) = r24; memw(r29+252) = r2; r2 = mpyi(r4,r2); r20 = mpyi(r3,r17) }
	{ r1 = asl(r2,00000002); r0 -= asl(r20,00000006) }
	{ call fn00009750 }
	{ r2 = memw(r29+240) }
	{ r2 = add(r2,r0) }
	{ r1:r0 = combine(r0,r2); call fn00009750 }
	{ r21 = memw(r29+248); r3 = add(r0,00000001); r2 = r25 }
	{ p1 = cmp.eq(r21,00000001); r4 = clrbit(r3,00000000); r2 += lsr(r2,0000001F) }
	{ p0 = cmp.eq(r4,00000002); r3 += lsr(r3,0000001F) }
	{ memb(r29+55) = r2.new; r2 = asr(r2,00000001); r25 -= asr(r2,00000001) }
	{ if (!p1) r25 = add(r2,00000000); r26 = p1 }
	{ memw(r29+176) = r25; memw(r29+256) = r26; if (p0) r24 = 00000002 }
	{ memw(r29+180) = r24; r1:r0 = combine(r24,r25) }
	{ call fn00009750; r20 = and(0000003D,lsr(r20,0000000B)) }
	{ memw(r29+212) = r0; r3 = 00000040; r2 = asr(r20,0000001F) }
	{ r20 += lsr(r2,0000001A) }
	{ r20 = r17; r2 = asr(r20,00000006) }
	{ r2 = combine(r3.l,r2.l) }
	{ r1:r0 = combine(r3,r2) }
	{ memd(r29+136) = r1:r0 }
	{ l2fetch(r27,r1:r0) }
	{ r1 = 00000000; r2 = 00000080; r0 = addasl(r18,r21,00000007) }
	{ memw(r29+208) = r0; r17 = 00000000; call fn000095F0 }
	{ if (!cmp.gt(r17.new,00000000)) jump:nt 0001A314; r2 = memw(r29+72); r7 = mpyi(r24,r25) }

l0001A29C:
	{ r0 = memw(r29+256); r2 = memw(r29+248) }
	{ memb(r29+43) = r6.new; r3 = memw(r29+220); r6 = add(r24,FFFFFFFF); r2 = asl(r2,00000005) }
	{ memw(r29+92) = r7; r3 = -00000001; r19 = addasl(r19,r2,00000002) }
	{ memw(r29+220) = r3 }
	{ r3 = memw(r29+128); r2 = memw(r29+112) }
	{ r0 = memw(r29+76); r1 = memw(r29+212); r2 = mpyi(r3,r2) }
	{ r4 = memw(r29+220); r6 = memw(r29+244); r1 += add(r20,00000001); r5 = mpyi(r2,r22) }
	{ memw(r29+84) = r17; memw(r29+88) = r18; r3 = add(0000003F,mpyi(r1,r5)) }
	{ memw(r29+156) = r5; r2 = memw(r29+80) }
	{ r2 = mpyi(r0,r6); r7 = mpyi(r2,r18) }
	{ memw(r29+152) = r7; r5 = memw(r29+160); r0 = asr(r3,0000001F); r4 = mpyi(r5,r4) }
	{ memb(r29+54) = r1.new; r3 += lsr(r0,0000001A); r1 = mpyi(r2,r18) }

l0001A314:
	{ memb(r29+54) = r1.new; r3 += lsr(r0,0000001A) }

l0001A320:
	{ r3 = 00000040; r2 = asr(r3,00000006) }
	{ r2 = combine(r3.l,r2.l) }
	{ r1:r0 = combine(r3,r2) }
	{ l2fetch(r5,r1:r0) }
	{ if (p0.new) r3 = memw(r29+88); if (!p0.new) jump:nt 0001A6F4; p0 = cmp.gt(r22,00000001) }

l0001A33C:
	{ memb(r29+37) = r2.new; r4 = memw(r29+80); r22 = 00000000; r2 = add(r20,00000001) }
	{ memb(r29+60) = r4.new; r4 = add(r2,mpyi(r4,r3)) }

l0001A358:
	{ p0 = cmp.gt(r24,00000000); r6 = r25; r23 = 00000000; if (!p0.new) jump:nt 0001A6E8 }
	{ r21 = memw(r29+220); r26 = memw(r29+92) }

l0001A370:
	{ r4 = memw(r29+220); r3 = memw(r29+172); p1 = cmp.eq(r23,00000000); r2 = mpyi(r6,r24) }
	{ r5 = memw(r29+200); r7 = memw(r29+212) }
	{ if (p1) r2 = 00000000; p0 = cmp.gt(r2,r26); r3 = add(r25,r4); p2 = cmp.eq(r3,r23) }
	{ r4 = mux(p0,00000001,00000000); if (!p1) r2 = add(r20,FFFFFFFF); if (p2) r3 = add(r4,00000000); if (p2) r17 = sub(r3,r21) }
	{ p0 = cmp.eq(r5,00000000); r4 = add(r4,r7); r25 = r20 }
	{ if (!p2) r3 = add(r4,r21); if (!p2) r17 = add(r4,00000000) }
	{ r6 = sub(r6,r17); r5 = add(r17,r20); if (!p0) jump:nt 0001A3D0 }

l0001A3C4:
	{ if (!p1.new) r9 = memw(r29-4); if (!p1.new) r8 = memw(r29-128); if (!p1.new) jump:t 0001A418; p1 = cmp.eq(r14,00000000) }

l0001A3D0:
	{ r5 = memw(r29+156); r4 = memw(r29+148); r25 = r20 }
	{ memw(r29+256) = r6; r6 = memw(r29+152); r4 = add(r4,r17) }
	{ r5 = memw(r29+160); r3 = mpyi(r5,r3); r4 = add(0000003F,mpyi(r4,r5)) }
	{ r7 = asr(r4,0000001F); r5 += add(r3,r6) }
	{ r4 += lsr(r7,0000001A) }
	{ r4 = 00000040; r7 = asr(r4,00000006) }
	{ r3 = combine(r4.l,r7.l) }
	{ r1:r0 = combine(r4,r3) }
	{ l2fetch(r5,r1:r0) }
	{ jump 0001A45C }

l0001A418:
	{ memw(r29+256) = r6; r6 = memw(r29+164); r4 = mpyi(r9,r8) }
	{ r7 = memw(r29+248); r6 = add(r20,r6) }
	{ r7 = 00000040; r6 = mpyi(r6,r7) }
	{ r4 = memw(r29+168); r3 = add(0000003F,mpyi(r4,r5)); r6 += mpyi(r3,r8) }
	{ r1 = asr(r3,0000001F) }
	{ r3 += lsr(r1,0000001A); r6 = add(r4,mpyi(r6,r9)) }
	{ r3 = asr(r3,00000006) }
	{ r3 = combine(r7.l,r3.l) }
	{ r1:r0 = combine(r7,r3) }
	{ l2fetch(r6,r1:r0) }

l0001A45C:
	{ r20 = memw(r29+240); if (p0) r3 = memw(r29-92); r6 = r16; if (!p0) jump:nt 0001A574 }

l0001A46C:
	{ r7 = memw(r29+252); r4 = memw(r29+248); if (p0.new) r16 = add(r6,00000000); p0 = cmp.eq(r22,00000000) }
	{ if (p0) r18 = add(r27,00000000); r20 = r7; r3 = add(r25,r3) }
	{ r3 = memw(r29+168); r5 = mpyi(r3,r4) }
	{ if (p0) r1 = add(r21,r5); if (!p0) jump:nt 0001A574; r20 = add(r3,mpyi(r20,r5)) }

l0001A49C:
	{ memb(r29+31) = r0.new; r0 = p2; r7 = add(PC,00010C74) }
	{ r3 = memw(r7-96); r4 = memw(r7-112) }
	{ r24 = memw(r7-64); r6 = memw(r7-80) }
	{ r0 = memw(r29+120) }
	{ if (!p0.new) r6 = add(r2,r21); if (!p0.new) r7 = sub(r17,r2); if (!p0.new) jump:nt 0001A510; p0 = r0 }

l0001A4E0:
	{ r7 = memw(r29+252); r8 = memw(r29+100) }
	{ r0 = memw(r29+240); r2 = memw(r29+236) }
	{ r6 = memw(r29+168); if (!p0.new) r3 = add(r4,00000000); if (!p0.new) r24 = add(r6,00000000); p0 = r8 }
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
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001A59C; r2 = memw(r29+248) }

l0001A580:
	{ r1:r0 = memd(r29+136); r3 = memw(r29+244) }
	{ if (!p0.new) r2 = 00000000; p0 = cmp.gt(r3,r2) }
	{ if (!p2) r2 = add(r22,00000000) }
	{ r2 = add(r27,mpyi(r2,r6)) }
	{ l2fetch(r2,r1:r0) }

l0001A59C:
	{ r18 = memw(r29+236); r16 = r6; r2 = r6; r24 = r22 }
	{ r3 = sub(00000000,r18); r24 = add(r27,mpyi(r24,r6)) }
	{ r1:r0 = combine(r19,r24); call gvmsumb_asm }
	{ r13 = memw(r29+252); r12 = memw(r29+228); p0 = cmp.eq(r22,00000000); r7 = mpyi(r18,r16) }
	{ if (!p0) r7 = memw(r29-48); if (p0) r1 = add(r27,00000000); r0 = mpyi(r13,r12) }
	{ if (!p0) r20 = add(r25,00000000); if (!p0) jump:nt 0001A664; r0 = add(r20,mpyi(r0,r21)) }

l0001A5EC:
	{ memb(r29+10) = r2.new; r2 = memw(r29+196); r20 = r25 }
	{ r2 = memw(r29+208) }
	{ memw(r29+24) = r2; memw(r29+36) = r5 }
	{ r14 = memw(r29+132); r4 = memw(r29+232) }
	{ memw(r29+12) = r17; memw(r29+20) = r19; r8 = sub(00000000,r14); r6 = mpyi(r21,r4) }
	{ r5 = memw(r29+244); r3 = memw(r29+192) }
	{ memw(r29+8) = r20; r13 = memw(r29+224); r3 = r13; r12 = combine(r12.l,r3.l) }
	{ memw(r29+4) = r13; r2 = memw(r29+204); r9 = mpyi(r6,r5) }
	{ memw(r29) = r12; r15 = memw(r29+184); r7 = mpyi(r7,r14) }
	{ memw(r29+28) = r8; r8 = memw(r29+216); r6 = addasl(r15,r6,00000002) }
	{ memw(r29+16) = r6; memw(r29+32) = r7; r2 += add(r9,r8) }
	{ call gvconvsum2dbbb_asm }
	{ jump 0001A6CC }

l0001A664:
	{ memb(r29+8) = r2.new; r2 = memw(r29+196) }
	{ r2 = memw(r29+188) }
	{ memw(r29+12) = r17; r5 = memw(r29+244); r7 = addasl(r2,r22,00000002); r6 = mpyi(r21,r4) }
	{ memw(r29+8) = r20; r3 = memw(r29+192); r8 = mpyi(r6,r5) }
	{ r2 = memw(r29+204); r1 = memw(r29+216); r3 = r13; r12 = combine(r12.l,r3.l) }
	{ memb(r29+1) = r13.new; r13 = memw(r29+224); r1 = r24; r9 = add(r22,r1) }
	{ memw(r29+28) = r7; r14 = memw(r29+184) }
	{ memw(r29) = r12; r6 = addasl(r14,r6,00000002) }
	{ memw(r29+16) = r6; call gvconv2dbbb_asm }

l0001A6CC:
	{ r24 = memw(r29+180); r25 = memw(r29+176); r21 = add(r17,r21) }
	{ r6 = memw(r29+256); r26 = sub(r26,r25) }
	{ if (!cmp.eq(r23.new,r24)) jump:t 0001A370; r23 = add(r23,00000001) }

l0001A6EC:
	{ if (cmp.gtu(r2,r22.new)) jump:t 0001A358; r22 = add(r22,00000020) }

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
	{ r18 = add(r18,00000001); r17 = r2 }
	{ memw(r16+8) = r17 }
	{ r5:r4 = memd(r29+48); r1 = 00000001; r0 = add(r16,00000034); r3:r2 = combine(r1,r0) }
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
	{ r2 = 38D1B717 }
	{ r1:r0 = combine(r0,r2); jump fn00009600 }
0001A7B8                         00 00 00 00 00 00 00 00         ........

;; avgpool_execute: 0001A7C0
avgpool_execute proc
	{ allocframe(000000C8); memd(r29+480) = r21:r20; r21 = r0 }
	{ memd(r29+184) = r19:r18; r2 = memw(r21+4) }
	{ r20 = memb(r21+32) }
	{ memd(r29+192) = r17:r16; r3 = memw(r21+8) }
	{ r17 = memw(r2+8); r19 = memw(r2); p0 = cmp.eq(r20,00000000) }
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
	{ if (p0.new) r3 = memw(r29+108); if (p0.new) jump:nt 0001A848; p0 = cmp.eq(r12,00000004) }

l0001A830:
	{ memb(r29+31) = r2.new; r2 = 00000000; if (!p0.new) jump:nt 0001A858; p0 = cmp.eq(r12,00000002) }

l0001A840:
	{ jump 0001A84C; r0 += add(r23,FFFFFFFF) }

l0001A848:
	{ r0 = sub(r2,r3) }

l0001A84C:
	{ r1 = r26; call fn00009760 }
	{ memw(r29+124) = r0 }

l0001A858:
	{ nop; if (p0.new) jump:nt 0001A894; p0 = cmp.eq(r12,00000004) }

l0001A860:
	{ if (p0.new) r1 = memw(r29+72); if (!p0.new) r0 = memw(r29-112); if (p0.new) jump:nt 0001A888; p0 = cmp.eq(r12,00000002) }

l0001A86C:
	{ memb(r29+17) = r2.new; r2 = 00000000 }
	{ if (p0.new) r1 = memw(r29+72); if (p0.new) r0 = add(r27,00000000); if (!p0.new) jump:nt 0001A8AC }

l0001A884:
	{ jump 0001A8A0 }

l0001A888:
	{ r0 = r1 }
	{ jump 0001A8A0; r0 += add(r27,FFFFFFFF) }

l0001A894:
	{ r3 = memd(r29+64); r1 = memd(r29+72) }
	{ r2 = add(r1,r27) }
	{ r0 = sub(r2,r3) }

l0001A8A0:
	{ call fn00009760 }
	{ memw(r29+68) = r0 }
	{ r2 = r22; r1 = 00000050; r4 = add(PC,0000E057) }

l0001A8AC:
	{ r2 = r22; r1 = 00000050; r4 = add(PC,00000017) }

l0001A8B8:
	{ r3 = memw(r19+16); r7 = memw(r16+16) }
	{ memw(r29+100) = r3; memw(r29+104) = r7 }
	{ memw(r29) = r21; call logmsg_function }
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0001A8E8; r2 = memw(r18) }

l0001A8D4:
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0001A8EC }

l0001A8DC:
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0001A8EC }

l0001A8E4:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001A908 }

l0001A8E8:
	{ r1 = 00000055; r3 = add(PC,0000E034) }

l0001A8EC:
	{ r1 = 00000055; r3 = add(PC,00000034) }

l0001A8F4:
	{ r2 = r22; call errlog_function }

l0001A8F8:
	{ r2 = r22 }
	{ r0 = FFFFFFFF; jump 0001AC2C }
0001A904             02 59 18 ED                             .Y..        

l0001A908:
	{ r4 = memd(r29+68); r3 = memd(r29+124); r1 = 00000057 }
	{ r7 = memw(r16+20); r2 = mpyi(r2,r3) }
	{ r2 = mpyi(r2,r4) }
	{ if (!cmp.gtu(r2.new,r7)) jump:t 0001A930; r2 = asl(r2,00000002) }

l0001A928:
	{ jump 0001A8F8; r3 = add(PC,00000010) }

l0001A930:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 0001A94C; r3 = memb(r21+32); p0 = cmp.gt(r24,00000000) }

l0001A940:
	{ r1 = 00000058; jump 0001A8F8; r3 = add(PC,00000006) }

l0001A94C:
	{ memw(r16+24) = r2; r2 = memd(r29+68); if (p0) r20 = 00000000 }
	{ memw(r29+128) = r26; r7 = memw(r29+124); if (p0) r26 = 3F800000 }
	{ memw(r29+12) = r22 }
	{ memw(r16+4) = r2; memw(r29+16) = r21 }
	{ memw(r16+8) = r7; memw(r16) = r24 }
	{ memw(r16+12) = r25; if (!p0) jump:nt 0001AC0C }

l0001A984:
	{ r4 = memd(r29+68); r2 = memd(r29+124); r18 = mpyi(r23,r25) }
	{ r7 = memw(r29+64); r8 = memw(r29+108) }
	{ r1 = memw(r29+72); r6 = memw(r29+128); r4 = add(r4,FFFFFFFF); r2 = add(r2,FFFFFFFF) }
	{ memw(r29+56) = r27; memw(r29+120) = r23; r5 = sub(r7,r27); r3 = sub(r8,r23) }
	{ r3 = sub(FFFFFFFF,r27); r4 = add(r5,mpyi(r4,r1)); r2 = add(r3,mpyi(r2,r6)) }
	{ memw(r29+48) = r3; r6 = 00000000; r3 = sub(FFFFFFFF,r23); r4 += lsr(r4,0000001F) }
	{ memw(r29+96) = r3; r5 = asl(r25,00000002); r2 += lsr(r2,0000001F) }
	{ memw(r29+92) = r5; memw(r29+60) = r6; r3 = 00000000; r6 = asr(r4,00000001) }
	{ memw(r29+44) = r6; memw(r29+32) = r3; r4 = sub(00000000,r6); r2 = asr(r2,00000001) }
	{ memw(r29+88) = r2; r2 = sub(00000000,r2); r3 = add(r6,sub(0000007F,r7)); r5 = add(r2,sub(0000007F,r8)) }
	{ memw(r29+20) = r3; memw(r29+24) = r4 }
	{ memw(r29+36) = r2; memw(r29+40) = r5 }

l0001AA08:
	{ memw(r29+28) = r24; r2 = memw(r29+68) }
	{ if (!p0.new) jump:nt 0001ABEC; p0 = cmp.gt(r2,00000001) }

l0001AA14:
	{ memb(r29+21) = r2.new; r2 = memw(r29+24) }
	{ r7 = memd(r29+20); r3 = memd(r29+32) }
	{ memw(r29+80) = r7; r7 = 00000000; r2 = mpyi(r3,r2) }
	{ memw(r29+76) = r7; memw(r29+52) = r2 }

l0001AA30:
	{ if (!cmp.gt(r2.new,00000001)) jump:nt 0001ABC8; r2 = memw(r29+124) }

l0001AA3C:
	{ r5 = memd(r29+76); r2 = memd(r29+72) }
	{ r7 = memd(r29+80); r1 = memd(r29+48); r2 = mpyi(r5,r2) }
	{ r3 = memd(r29+84); r4 = memd(r29+52) }
	{ r7 = memd(r29+40); r1 = 00000000; r4 = add(r5,r4); r5 = max(r7,r1) }
	{ memw(r29+132) = r1; r1 = memw(r29+44); r5 = sub(FFFFFFFF,r5); r3 = max(r3,r6) }
	{ memw(r29+144) = r5; r5 = memw(r29+124); r2 = sub(r2,r1) }
	{ memw(r29+136) = r7; r7 = memw(r29+64); r4 = mpyi(r4,r5) }
	{ r7 = memd(r29+56); r2 = add(r2,r7); r16 = max(r2,r6) }
	{ memw(r29+116) = r4; r1 = memd(r29+60); r27 = min(r7,r2) }
	{ r4 = memw(r29+120); r3 = add(r1,r3) }
	{ memb(r29+35) = r0.new; r0 = memw(r29+36); r3 = mpyi(r4,r3) }

l0001AAA8:
	{ if (p0.new) r2 = memw(r29-128); if (p0.new) r3 = memw(r29-124); p0 = cmp.gt(r25,00000000); if (!p0.new) jump:nt 0001AB94 }

l0001AAB8:
	{ r21 = 00000000; r7 = 00000000 }
	{ r4 = memw(r29+116); r1 = memw(r29+140) }
	{ r6 = memw(r29+136); r5 = memw(r29+96); r3 = add(r3,r4); r2 = mpyi(r3,r2) }
	{ r0 = memd(r29+88); r1 = memd(r29+112); r3 = mpyi(r3,r25); r4 = max(r1,r7) }
	{ r1 = memd(r29+100); r19 = memd(r29+92); r6 = add(r1,r4); r5 = max(r6,r5) }
	{ r0 = memw(r29+108); r5 = sub(FFFFFFFF,r5); r2 = sub(r2,r0) }
	{ r1 = memd(r29+120); r7 = memd(r29+104); r19 = add(r1,mpyi(r19,r6)); r24 = max(r2,r7) }
	{ r17 = sub(r5,r4); r2 = add(r2,r0); r23 = addasl(r7,r3,00000002) }
	{ r22 = min(r1,r2) }

l0001AB14:
	{ if (p0.new) r2 = memw(r29-112); p0 = cmp.gt(r27,r16); r1:r0 = combine(r20,r20); if (!p0.new) jump:nt 0001AB7C }

l0001AB24:
	{ r1:r0 = combine(r20,r20) }
	{ r2 = r19; r3 = sub(r2,r16) }
	{ loop1(0001AB34,r3) }
	{ p0 = cmp.gt(r22,r24); r5 = add(r17,FFFFFFFF); if (!p0.new) jump:nt 0001AB70; r3 = addasl(r2,r25,00000002) }

l0001AB44:
	{ r4 = memw(r2); p0 = cmp.gtu(r17,00000001); r1 = sfadd(r1,r26); loop0(0001AB58,r5) }
	{ if (!p0) jump:nt 0001AB6C }

l0001AB58:
	{ r4 = memw(r3); r1 = sfadd(r1,r26); r0 = sfadd(r0,r4) }
	{ nop; r3 = addasl(r3,r25,00000002) }

l0001AB6C:
	{ r0 = sfadd(r0,r4) }

l0001AB70:
	{ nop; nop; r2 = addasl(r2,r18,00000002) }

l0001AB7C:
	{ r21 = add(r21,00000001); call fn00009610 }
	{ memw(r23) = r0; r19 = add(r19,00000004); p0 = cmp.eq(r21,r25); r23 = add(r23,00000004) }
	{ if (!p0) jump:nt 0001AB14 }

l0001AB94:
	{ r6 = memw(r29+124); r4 = memw(r29+132) }
	{ r3 = memw(r29+140); r2 = memw(r29+128); r4 = add(r4,00000001) }
	{ memw(r29+132) = r4; r7 = memw(r29+136); p0 = cmp.eq(r4,r6); r3 = add(r3,r2) }
	{ memw(r29+140) = r3; r5 = sub(r7,r2) }
	{ memw(r29+136) = r5; if (!p0) jump:nt 0001AAA8 }

l0001ABC8:
	{ r6 = memd(r29+68); r4 = memd(r29+76) }
	{ r3 = memd(r29+84); r2 = memd(r29+72); r4 = add(r4,00000001) }
	{ memw(r29+76) = r4; r7 = memd(r29+80); p0 = cmp.eq(r4,r6); r3 = add(r3,r2) }
	{ r5 = sub(r7,r2) }
	{ memw(r29+80) = r5; memw(r29+84) = r3; if (!p0) jump:nt 0001AA30 }

l0001ABEC:
	{ r24 = memw(r29+28); r4 = memw(r29+32) }
	{ r3 = memd(r29+60); r2 = memd(r29+56); r4 = add(r4,00000001) }
	{ memw(r29+32) = r4; r3 = add(r3,r2); p0 = cmp.eq(r4,r24) }
	{ memw(r29+60) = r3; if (!p0) jump:nt 0001AA08 }

l0001AC0C:
	{ memb(r29) = r2.new; r2 = memw(r29+16); r4 = add(PC,0000DD48) }
	{ r2 = memw(r29+12) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0001AC2C:
	{ r19:r18 = memd(r29+184); r17:r16 = memd(r29+192) }
	{ r23:r22 = memd(r29+168); r21:r20 = memd(r29+176) }
	{ r27:r26 = memd(r29+152); r25:r24 = memd(r29+160) }
	{ dealloc_return }

;; avgpool_check: 0001AC40
avgpool_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000DC34) }
	{ r17 = r0; r16 = r1; r1 = 0000008C }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001AC80; r2 = memw(r17+16); r1 = 0000008D }

l0001AC6C:
	{ r3 = add(PC,00000025) }

l0001AC70:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001AC80:
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0001AC94; r2 = memw(r17+20); r1 = 0000008E }

l0001AC90:
	{ r3 = memw(r17+4) }

l0001AC94:
	{ jump 0001AC70; r3 = add(PC,0000DC10) }
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
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001AD2C; r5 = memw(r2+16) }

l0001AD18:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000001) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001AD2C:
	{ dealloc_return }

;; errlog_function: 0001AD30
;;   Called from:
;;     0001A8F4 (in avgpool_execute)
;;     0001AC70 (in avgpool_check)
;;     0001AD20 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000DB25) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001AD54             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; biasadd_f_execute: 0001AD60
biasadd_f_execute proc
	{ allocframe(00000038); memd(r29+496) = r17:r16; r3 = add(PC,0000DC81) }
	{ r1 = 00000045; r17:r16 = combine(r0,r1) }
	{ memd(r29+40) = r19:r18; r2 = memw(r17+4) }
	{ memd(r29+24) = r23:r22; memd(r29+32) = r21:r20 }
	{ memd(r29+16) = r25:r24; r5 = memw(r2+4) }
	{ memd(r29+8) = r27:r26 }
	{ if (!cmp.eq(r4.new,00000002)) jump:t 0001AE30; r4 = memw(r5+4) }

l0001AD94:
	{ r1 = 00000046; r3 = add(PC,00000011) }
	{ if (!cmp.eq(r4.new,00000002)) jump:t 0001AE30; r4 = memw(r5) }

l0001ADA8:
	{ r1 = 00000047; r3 = add(PC,0000003D) }
	{ if (!cmp.eq(r4.new,00000002)) jump:t 0001AE30; r4 = memw(r5+8) }

l0001ADBC:
	{ if (cmp.eq(r18.new,r4)) jump:t 0001ADDC; r18 = memw(r3+12) }

l0001ADC8:
	{ memw(r29+4) = r18; r1 = 00000049; r3 = add(PC,00000028) }
	{ memw(r29) = r4; r2 = r16 }
	{ jump 0001AE34 }

l0001ADDC:
	{ r24 = memw(r3+4); r23 = memw(r3); r4 = add(PC,0000DC28) }
	{ r6 = memw(r17+8); r22 = memw(r3+8); r2 = r16; r1 = 0000004C }
	{ r19 = memw(r5+16); r20 = memw(r3+16); r7 = mpyi(r23,r24) }
	{ r25 = memw(r6) }
	{ r3 = mpyi(r7,r22) }
	{ memw(r29) = r17; r21 = memw(r25+16) }
	{ r3 = mpyi(r3,r18) }
	{ call logmsg_function; r26 = asl(r3,00000002) }
	{ r1 = 0000004D; r3 = add(PC,0000DC02) }
	{ if (!cmp.gtu(r26,r2.new)) jump:t 0001AE40; r2 = memw(r25+20) }

l0001AE30:
	{ r2 = r16 }

l0001AE34:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001AEE4 }

l0001AE40:
	{ memw(r25) = r23; memw(r25+24) = r26; r2 = mpyi(r24,r23) }
	{ memw(r25+12) = r18; memw(r25+4) = r24; r3 = mpyi(r2,r22) }
	{ memw(r25+8) = r22 }
	{ r2 = 00000000; if (p0.new) jump:nt 0001AEC8; p0 = cmp.eq(r3,00000000); loop1(0001AE68,r3) }

l0001AE68:
	{ r6 = add(r20,r2); r5 = add(r19,r2); if (p0.new) jump:nt 0001AEBC; p0 = cmp.eq(r10,00000000) }

l0001AE74:
	{ r4 = add(r21,r2); p0 = cmp.gtu(r18,00000001); r3 = add(r2,00000004); r7 = add(r18,FFFFFFFF) }
	{ r6 = memw(r6); r5 = memw(r5) }
	{ if (!p0) jump:nt 0001AEB4; loop0(0001AE90,r7) }

l0001AE90:
	{ memb(r4) = r5.new; r1 = add(r19,r3); r6 = add(r20,r3); r5 = sfadd(r6,r5) }
	{ r6 = memw(r6); r5 = memw(r1); r4 = add(r21,r3) }
	{ nop; r3 = r7 }

l0001AEB4:
	{ memb(r4) = r3.new; r3 = sfadd(r6,r5) }

l0001AEBC:
	{ nop; r21 = addasl(r21,r18,00000002); r20 = addasl(r20,r18,00000002) }

l0001AEC0:
	{ nop; r21 = addasl(r21,r18,00000002) }

l0001AEC8:
	{ r2 = r16; r1 = 00000058; r4 = add(PC,0000DB64) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l0001AEE4:
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ r27:r26 = memd(r29+8); r25:r24 = memd(r29+16) }
	{ dealloc_return }
0001AEF8                         00 40 00 7F 00 C0 00 7F         .@......

;; biasadd_check: 0001AF00
biasadd_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000DA83) }
	{ r17 = r0; r16 = r1; r1 = 0000005E }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000004)) jump:t 0001AF40; r2 = memw(r17+16); r1 = 0000005F }

l0001AF2C:
	{ r3 = add(PC,00000034) }
	{ r2 = r16; call errlog_function }

l0001AF34:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001AF40:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001AF58; r2 = memw(r17+20); r1 = 00000060 }

l0001AF50:
	{ jump 0001AF34; r3 = add(PC,00000027) }

l0001AF58:
	{ r2 = r16; r1 = 00000061; r4 = add(PC,0000DA70) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001AF78
;;   Called from:
;;     0001AE14 (in biasadd_f_execute)
;;     0001AED8 (in biasadd_f_execute)
;;     0001AF14 (in biasadd_check)
;;     0001AF68 (in biasadd_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001AF9C; r5 = memw(r2+16) }

l0001AF88:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000020) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001AF9C:
	{ dealloc_return }

;; errlog_function: 0001AFA0
;;   Called from:
;;     0001AE34 (in biasadd_f_execute)
;;     0001AF30 (in biasadd_check)
;;     0001AF90 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000D9C4) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001AFC4             00 00 00 00 00 00 00 00 00 00 00 00     ............
0001AFD0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; concat_execute: 0001AFE0
concat_execute proc
	{ allocframe(00000040); memd(r29+472) = r23:r22; r4 = add(PC,0000DAE2) }
	{ r23 = r0 }
	{ memd(r29+40) = r21:r20; r21 = memw(r23+4) }
	{ r3 = memw(r23+8) }
	{ memd(r29+24) = r25:r24; memd(r29+48) = r19:r18; r1 = 00000045; r25 = r1 }
	{ r18 = memw(r3); r5 = memw(r21+4); r2 = r25 }
	{ memd(r29+16) = r27:r26; memd(r29+56) = r17:r16 }
	{ r17 = memw(r21); r16 = memw(r23+16) }
	{ r20 = memw(r5+4); r19 = memw(r5+8) }
	{ r22 = memw(r18+16); r27 = memw(r5) }
	{ memw(r29) = r23; call logmsg_function }
	{ r2 = memw(r17+16) }
	{ if (!cmp.eq(r2.new,00000006)) jump:t 0001B10C; r2 = memw(r2) }

l0001B040:
	{ memw(r29+12) = r23; r23 = r16; r24 = add(r21,00000004) }
	{ r5:r4 = combine(00000000,00000002); r2 = 00000000; p0 = cmp.gt(r23,00000000); r26 = 00000000 }
	{ if (p0) r3 = add(r24,00000000); if (!p0) r26 = 00000000; if (p0) jump:nt 0001B128; r17 = mpyi(r3,r27) }

l0001B064:
	{ memw(r18) = r27; memw(r29+8) = r25; if (p0) r27 = 00000000 }
	{ memw(r18+8) = r19; memw(r18+4) = r20 }
	{ memw(r18+24) = r2; memw(r18+12) = r26 }
	{ if (!p0) jump:nt 0001B0D8 }

l0001B080:
	{ r16 = memw(r24); p0 = cmp.gt(r17,00000000); r25 = r17; r18 = r22 }
	{ r2 = memw(r16+12); r20 = memw(r16+16); if (!p0) jump:nt 0001B0C0 }

l0001B098:
	{ r19 = asl(r2,00000002) }
	{ nop }

l0001B0A0:
	{ r1:r0 = combine(r20,r18); r2 = r19; call fn00009560 }
	{ r2 = memw(r16+12); r18 = addasl(r18,r26,00000002) }
	{ if (!cmp.eq(r25.new,00000001)) jump:t 0001B0A0; r25 = add(r25,FFFFFFFF); r20 = addasl(r20,r2,00000002) }

l0001B0C0:
	{ r21 = r24; r3 = add(r21,00000008); r22 = addasl(r22,r2,00000002) }

l0001B0C4:
	{ r21 = r24; r3 = add(r21,00000008) }

l0001B0CC:
	{ if (!cmp.eq(r27.new,r23)) jump:t 0001B080; r27 = add(r27,00000001); r24 = r3 }

l0001B0D8:
	{ memb(r29) = r2.new; r2 = memw(r29+12); r4 = add(PC,0000DA6A) }

l0001B0DC:
	{ memb(r29) = r2.new; r2 = memw(r29+12); r4 = add(PC,0000002A) }

l0001B0EC:
	{ r2 = memw(r29+8) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0001B0F8:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ r27:r26 = memd(r29+16); r25:r24 = memd(r29+24) }
	{ dealloc_return }

l0001B10C:
	{ r1 = 00000046; r3 = add(PC,0000D9CF) }

l0001B118:
	{ r2 = r25; call errlog_function }

l0001B11C:
	{ r2 = r25 }
	{ r0 = FFFFFFFF; jump 0001B0F8 }

l0001B128:
	{ loop0(0001B12C,r23) }
	{ r6 = memw(r3) }
	{ if (cmp.eq(r7.new,r19)) jump:t 0001B14C; r7 = memw(r6+8) }

l0001B13C:
	{ memw(r29) = r2; r1 = 00000049; r3 = add(PC,0000002E) }
	{ jump 0001B118 }

l0001B14C:
	{ if (cmp.eq(r7.new,r20)) jump:t 0001B168; r7 = memw(r6+4) }

l0001B158:
	{ memw(r29) = r2; r1 = 0000004C; r3 = add(PC,0000002B) }
	{ jump 0001B118 }

l0001B168:
	{ if (cmp.eq(r7.new,r27)) jump:t 0001B184; r7 = memw(r6) }

l0001B174:
	{ memw(r29) = r2; r1 = 0000004F; r3 = add(PC,00000029) }
	{ jump 0001B118 }

l0001B184:
	{ r6 = memw(r6+12); r5 = r3; r2 = add(r2,00000001); r7 = add(r5,00000008) }
	{ r26 = add(r6,r26); r3 = r7 }
	{ nop; r4 += mpyi(r17,r6) }
	{ r3 = memw(r18+20) }
	{ if (!cmp.gtu(r2.new,r3)) jump:t 0001B064; r2 = asl(r4,00000002) }

l0001B1B0:
	{ r1 = 00000055; jump 0001B11C; r3 = add(PC,00000008) }
0001B1BC                                     00 C0 00 7F             ....

;; concat_check: 0001B1C0
concat_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000D89A) }
	{ r17 = r0; r16 = r1; r1 = 0000006A }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r2 = memw(r17+16); r1 = 0000006B; r3 = add(PC,0000D896) }
	{ if (cmp.gtu(r4.new,r2)) jump:t 0001B24C; r4 = 00000004 }

l0001B1F8:
	{ r1 = 0000006C; r3 = add(PC,0000000F) }
	{ if (!cmp.eq(r4.new,00000002)) jump:t 0001B24C; r4 = memw(r17+20) }

l0001B20C:
	{ r4 = memw(r17+4) }
	{ if (!cmp.gtu(r2,r5.new)) jump:nt 0001B234; r5 = add(r5,00000001); r4 = add(r4,00000004) }

l0001B214:
	{ if (!cmp.gtu(r2,r5.new)) jump:nt 0001B238; r5 = add(r5,00000001) }

l0001B220:
	{ if (!cmp.eq(r6.new,00000001)) jump:t 0001B214; r6 = memw(r4); r3 = add(PC,00000037) }

l0001B230:
	{ r1 = 0000006E }

l0001B234:
	{ r2 = memw(r17+8); r1 = 00000071; r3 = add(PC,0000D86A) }

l0001B238:
	{ r2 = memw(r17+8); r1 = 00000071; r3 = add(PC,0000002A) }
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001B25C; r2 = memw(r2) }

l0001B24C:
	{ r2 = r16; call errlog_function }

l0001B250:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001B25C:
	{ r2 = r16; r1 = 00000073; r4 = add(PC,0000D84E) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001B27C
;;   Called from:
;;     0001B028 (in concat_execute)
;;     0001B0F0 (in concat_execute)
;;     0001B1D4 (in concat_check)
;;     0001B26C (in concat_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001B2A0; r5 = memw(r2+16) }

l0001B28C:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000034) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001B2A0:
	{ dealloc_return }

;; errlog_function: 0001B2A4
;;   Called from:
;;     0001B118 (in concat_execute)
;;     0001B24C (in concat_check)
;;     0001B294 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000D798) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001B2C8                         00 00 00 00 00 00 00 00         ........

;; conv2d_f_execute_ref: 0001B2D0
conv2d_f_execute_ref proc
	{ allocframe(+00000078) }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+112) = r17:r16; memd(r29+104) = r19:r18 }
	{ r16 = memw(r2); r18 = memb(r0+32) }
	{ r4 = memw(r2+8); r19 = memw(r2+4); p0 = cmp.eq(r18,00000000) }
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
	{ if (p0.new) r0 = add(r2,r3); if (p0.new) jump:nt 0001B35C; p0 = cmp.eq(r10,00000004) }

l0001B340:
	{ memw(r29+60) = r3; r20 = r1; p0 = cmp.eq(r18,00000001); r0 = 00000000 }
	{ if (!p0) jump:nt 0001B370 }

l0001B350:
	{ r3 = memd(r29+60); r1 = r20 }
	{ r0 = r3 }
	{ r0 += add(r24,FFFFFFFF) }

l0001B35C:
	{ memw(r29+60) = r3; r20 = r1; r22 = r4; r1 = r3 }
	{ call fn00009760 }
	{ r4 = r22 }

l0001B370:
	{ r2 = sub(r17,r27); if (p0.new) jump:nt 0001B3A4; p0 = cmp.eq(r10,00000004) }

l0001B378:
	{ memw(r29+64) = r0; if (!p0) r1:r0 = combine(r4,r4); if (p0.new) jump:nt 0001B39C; p0 = cmp.eq(r10,00000002) }

l0001B384:
	{ r1 = memd(r29+56); r0 = 00000000; r18 = r4 }
	{ if (!p0) r1:r0 = combine(r18,r17); if (!p0.new) jump:nt 0001B3B4; p0 = r1 }

l0001B398:
	{ jump 0001B3B0 }

l0001B39C:
	{ jump 0001B3AC; r0 += add(r17,FFFFFFFF) }

l0001B3A4:
	{ memw(r29+64) = r0; r1 = r4; r0 = add(r2,r4) }

l0001B3AC:
	{ r18 = r4 }

l0001B3B0:
	{ call fn00009760 }

l0001B3B4:
	{ r22 = memd(r29+44); r2 = memw(r19+16); r4 = add(PC,0000D854) }
	{ memw(r29+36) = r18; r19:r18 = combine(r18,r20); r1 = 0000005E }
	{ memw(r29+56) = r2; r7 = memw(r21+16); r2 = r18 }
	{ r16 = memw(r16+16); r3 = memw(r22+28) }
	{ memw(r29+40) = r0; memw(r29+4) = r3 }
	{ memw(r29+52) = r7; memw(r29+32) = r21 }
	{ memw(r29) = r22; call logmsg_function }
	{ memw(r29+12) = r23; r1 = 0000005F; r4 = add(PC,0000D83E) }
	{ memw(r29+4) = r17; r2 = r18 }
	{ memw(r29+8) = r24; r21 = memw(r29+20) }
	{ memw(r29) = r21; call logmsg_function }
	{ r20 = memd(r29+48); r2 = r18; r4 = add(PC,0000D833) }
	{ memw(r29+12) = r20; r1 = 00000060 }
	{ memw(r29+4) = r27; memw(r29+8) = r26 }
	{ memw(r29) = r25; call logmsg_function }
	{ r3 = memd(r29+60); r2 = r18; r4 = add(PC,0000D827) }
	{ memw(r29+4) = r3; r1 = 00000061 }
	{ memw(r29) = r19; call logmsg_function }
	{ r2 = r18; r1 = 00000062; r4 = add(PC,0000D81F) }
	{ r22 = memw(r29+40); r3 = memb(r22+32) }
	{ memw(r29) = r3; call logmsg_function }
	{ memw(r29) = r21; memw(r29+12) = r25; r4 = add(PC,0000D811) }
	{ r19 = memw(r29+64); r1 = 00000063 }
	{ memw(r29+4) = r22; memw(r29+8) = r19; r2 = r18 }
	{ call logmsg_function }
	{ r15 = r19; if (p0.new) jump:nt 0001B4B0; p0 = cmp.eq(r15,-00000001); r2 = mpyi(r21,r25) }

l0001B49C:
	{ r2 = r18; r1 = 00000065; r3 = add(PC,0000D800) }
	{ jump 0001B504 }

l0001B4B0:
	{ r3 = memd(r29+32); r6 = r21; r9 = r22 }
	{ memw(r29+16) = r18; r5 = r3 }
	{ r4 = memw(r3+20); r2 = mpyi(r2,r19) }
	{ r2 = mpyi(r2,r22) }
	{ if (!cmp.gtu(r2.new,r4)) jump:t 0001B4E8; r2 = asl(r2,00000002) }

l0001B4D4:
	{ memw(r29+4) = r2; r1 = 00000067; r3 = add(PC,00000021) }
	{ memw(r29) = r4; r2 = memd(r29+16) }
	{ jump 0001B504 }

l0001B4E8:
	{ r4 = memw(r29+28) }
	{ if (cmp.eq(r3.new,00000002)) jump:t 0001B510; r3 = memw(r4) }

l0001B4F8:
	{ r1 = 00000069; r3 = add(PC,00000017) }
	{ r2 = memw(r29+16) }

l0001B504:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001B728 }

l0001B510:
	{ if (cmp.eq(r3.new,00000002)) jump:t 0001B52C; r3 = memw(r4+12); p0 = cmp.gt(r6,00000000) }

l0001B520:
	{ r1 = 0000006A; jump 0001B504; r3 = add(PC,00000000) }

l0001B52C:
	{ memw(r5+4) = r9; memw(r5+24) = r2; if (p0) r3 = sub(r26,r24); if (p0) r2 = add(r15,FFFFFFFF) }
	{ memw(r5+8) = r15; memw(r5) = r6; if (p0) r4 = sub(r27,r17); if (p0) r7 = add(r9,FFFFFFFF) }
	{ memw(r5+12) = r25; if (!p0) jump:nt 0001B704 }

l0001B554:
	{ r6 = memd(r29+36); r5 = memd(r29+60) }
	{ memb(r29+6) = r3.new; r3 = 00000000; r7 = add(r4,mpyi(r7,r6)); r2 = add(r3,mpyi(r2,r5)) }
	{ r4 = 00000000; r3 = mpyi(r23,r25) }
	{ r7 += lsr(r7,0000001F); r2 += lsr(r2,0000001F) }
	{ r6 = asr(r7,00000001); r5 = mpyi(r5,r25) }
	{ memb(r29+12) = r2.new; r2 = asr(r2,00000001) }

l0001B594:
	{ p0 = cmp.gt(r9,00000000); if (!p0.new) jump:nt 0001B6F4 }

l0001B59C:
	{ memb(r29+11) = r7.new; r2 = memw(r29+24); r7 = 00000000 }
	{ memb(r29+8) = r2.new; r2 = mpyi(r2,r9) }

l0001B5B4:
	{ p0 = cmp.gt(r15,00000000); r13 = 00000000; if (!p0.new) jump:nt 0001B6DC }
	{ r7 = memd(r29+44); r2 = memd(r29+36) }
	{ r0 = memd(r29+28); r6 = memd(r29+32) }
	{ r7 = add(r7,r6); r2 = mpyi(r7,r2) }
	{ memb(r29+16) = r1.new; r14 = sub(r2,r0); r1 = mpyi(r7,r15) }

l0001B5DC:
	{ if (p0.new) r28 = memw(r29+56); p0 = cmp.gt(r25,00000000); if (!p0.new) jump:nt 0001B6D4 }
	{ r2 = memd(r29+64); r6 = memd(r29+60); r0 = 00000000 }
	{ r2 = add(r13,r2) }
	{ r6 = memw(r29+48); r2 = mpyi(r2,r25); r7 = mpyi(r13,r6) }
	{ r6 = memw(r29+52); r1 = sub(r7,r6) }
	{ r10 = addasl(r6,r2,00000002) }

l0001B60C:
	{ p0 = cmp.gt(r27,00000000); r7 = 00000000; r9 = r28; r11 = r4 }
	{ if (!p0) jump:nt 0001B6BC }

l0001B620:
	{ if (!cmp.gtu(r17,r2.new)) jump:nt 0001B6B0; r2 = add(r7,r14) }

l0001B62C:
	{ if (p0.new) r12 = add(r2,r8); if (p0.new) r21 = 00000000 }
	{ if (p0.new) r2 = add(r9,00000000); p0 = cmp.gt(r26,00000000); if (!p0.new) jump:nt 0001B6B0 }

l0001B640:
	{ r18 = mpyi(r12,r24); loop1(0001B648,r26) }
	{ if (!cmp.gtu(r24,r12.new)) jump:nt 0001B6A4; r12 = add(r21,r1) }

l0001B654:
	{ if (p0.new) r6 = add(r23,FFFFFFFF); p0 = cmp.gt(r12,FFFFFFFF) }
	{ r12 = add(r12,r18); if (!p0.new) jump:nt 0001B6A4; p0 = cmp.gt(r15,00000000) }

l0001B664:
	{ r22 = memw(r2); p0 = cmp.gtu(r23,00000001) }
	{ r12 = mpyi(r12,r23); loop0(0001B688,r6) }
	{ r12 = addasl(r2,r25,00000002); r19 = addasl(r16,r12,00000002) }
	{ r19 = memw(r19); r20 = add(r19,00000004) }
	{ if (!p0) jump:nt 0001B6A0 }

l0001B688:
	{ r22 = memw(r12); r19 = memw(r20); r12 = addasl(r12,r25,00000002); r11 += sfmpy(r19,r22) }
	{ nop; r20 = add(r20,00000004) }

l0001B6A0:
	{ r11 += sfmpy(r19,r22) }

l0001B6A4:
	{ nop; r21 = add(r21,00000001); r2 = addasl(r2,r3,00000002) }

l0001B6B0:
	{ if (!cmp.eq(r7.new,r27)) jump:t 0001B620; r7 = add(r7,00000001); r9 = addasl(r9,r5,00000002) }

l0001B6BC:
	{ memw(r10) = r11; r10 = add(r10,00000004); r28 = add(r28,00000004); r0 = add(r0,00000001) }

l0001B6C0:
	{ memw(r10) = r11; r10 = add(r10,00000004); r28 = add(r28,00000004) }

l0001B6CC:
	{ p0 = cmp.eq(r0,r25); if (!p0.new) jump:nt 0001B60C }

l0001B6D4:
	{ if (!cmp.eq(r13.new,r15)) jump:t 0001B5DC; r13 = add(r13,00000001) }

l0001B6E0:
	{ r9 = memw(r29+40) }
	{ r2 = add(r2,00000001) }
	{ memw(r29+44) = r2; p0 = cmp.eq(r2,r9); if (!p0.new) jump:nt 0001B5B4 }

l0001B6F4:
	{ r6 = memd(r29+20); r2 = memd(r29+24) }
	{ r2 = add(r2,00000001) }
	{ memw(r29+24) = r2; if (!p0.new) jump:nt 0001B594; p0 = cmp.eq(r2,-00000001) }

l0001B704:
	{ memw(r29+12) = r25; r1 = 00000093; r4 = add(PC,0000D5E9) }
	{ memw(r29) = r6; r2 = memd(r29+16) }
	{ memw(r29+4) = r9; memw(r29+8) = r15 }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0001B728:
	{ r19:r18 = memd(r29+104); r17:r16 = memd(r29+112) }
	{ r23:r22 = memd(r29+88); r21:r20 = memd(r29+96) }
	{ r27:r26 = memd(r29+72); r25:r24 = memd(r29+80) }
	{ dealloc_return }

;; conv2d_check_ref: 0001B73C
conv2d_check_ref proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000D433) }
	{ r17 = r0; r16 = r1; r1 = 0000009B }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001B778; r2 = memw(r17+16) }

l0001B764:
	{ r2 = memw(r17+28); r1 = 0000009C; r3 = add(PC,00000027) }
	{ memw(r29) = r2; jump 0001B78C }

l0001B778:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001B79C; r2 = memw(r17+20); r1 = 0000009D }

l0001B788:
	{ r3 = add(PC,0000001F) }

l0001B78C:
	{ r2 = r16; call errlog_function }

l0001B790:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001B79C:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001B7B4; r2 = memw(r17+4); r1 = 0000009E }

l0001B7AC:
	{ jump 0001B790; r3 = add(PC,00000012) }

l0001B7B4:
	{ if (!cmp.eq(r3.new,00000000)) jump:nt 0001B7DC; r3 = memw(r17+8); r4 = 00000000 }

l0001B7C4:
	{ r1 = 0000009F; jump 0001B790; r3 = add(PC,00000006) }

l0001B7D0:
	{ if (cmp.gtu(r4.new,00000004)) jump:nt 0001B7F8; r4 = add(r4,00000001); r2 = add(r2,00000004) }

l0001B7D4:
	{ if (cmp.gtu(r4.new,00000004)) jump:nt 0001B7FC; r4 = add(r4,00000001) }

l0001B7DC:
	{ if (!cmp.eq(r5.new,00000001)) jump:t 0001B7D0; r5 = memw(r2) }

l0001B7E0:
	{ if (!cmp.eq(r5.new,00000001)) jump:t 0001B7D4 }

l0001B7E8:
	{ memw(r29) = r4; r1 = 000000A2; r3 = add(PC,0000002F) }
	{ jump 0001B78C }

l0001B7F8:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001B814; r2 = memw(r3); r1 = 000000A7 }

l0001B7FC:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001B818; r2 = memw(r3) }

l0001B808:
	{ memw(r29) = 00000000; r3 = add(PC,0000001D) }
	{ jump 0001B78C }

l0001B814:
	{ r2 = r16; r1 = 000000AA; r4 = add(PC,0000D3DC) }

l0001B818:
	{ r2 = r16; r1 = 000000AA; r4 = add(PC,0000001C) }

l0001B824:
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
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
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001B858; r5 = memw(r2+16) }

l0001B844:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000011) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001B858:
	{ dealloc_return }

;; errlog_function: 0001B85C
;;   Called from:
;;     0001B504 (in conv2d_f_execute_ref)
;;     0001B78C (in conv2d_check_ref)
;;     0001B84C (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000D2F5) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; matmul_execute_ref: 0001B880
matmul_execute_ref proc
	{ allocframe(+00000058); r4 = add(PC,0000D52A) }
	{ r5 = memw(r0+8); r3 = memw(r0+4) }
	{ memd(r29+56) = r23:r22; memd(r29+80) = r17:r16; r1 = 0000004F; r16 = r1 }
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
	{ memw(r29+28) = r19; r1 = 00000052; r4 = add(PC,0000D4DE) }
	{ memw(r29+8) = r21; r2 = r16 }
	{ memw(r29+20) = r27; memw(r29+24) = r18 }
	{ memw(r29+12) = r20; memw(r29+16) = r25 }
	{ memw(r29) = r26; memw(r29+4) = r24 }
	{ call logmsg_function }
	{ p0 = cmp.eq(r24,00000001); r1 = 00000053; r3 = add(PC,0000D4D8) }
	{ if (p0) r1 = 00000054; if (!p0) jump:nt 0001B988 }

l0001B92C:
	{ if (p0.new) r1 = 00000055; p0 = cmp.eq(r27,00000001); r3 = add(PC,0000D4C0) }
	{ if (!p0) jump:nt 0001B988 }

l0001B940:
	{ if (p0.new) r1 = 00000056; p0 = cmp.eq(r26,00000001); r3 = add(PC,0000D4BE) }
	{ if (!p0) jump:nt 0001B988 }

l0001B954:
	{ p0 = cmp.eq(r25,00000001); r2 = mpyi(r26,r21); r3 = add(PC,0000D4AA) }
	{ if (p0) r4 = memw(r23+20); if (p0) r1 = 00000057; if (!p0) jump:nt 0001B988 }

l0001B970:
	{ r2 = mpyi(r2,r24); r3 = add(PC,0000D4A5) }
	{ r2 = mpyi(r2,r19) }
	{ if (!cmp.gtu(r2.new,r4)) jump:t 0001B998; r2 = asl(r2,00000002) }

l0001B988:
	{ r2 = r16; call errlog_function }

l0001B98C:
	{ r2 = r16 }

l0001B990:
	{ r0 = FFFFFFFF; jump 0001BA58 }

l0001B998:
	{ memw(r23+24) = r2; r28 = memw(r29+36); r0 = r22; r15 = r17 }
	{ memw(r23+12) = r19; memw(r23+4) = FFFFFF81 }
	{ memw(r23+8) = r21; memw(r23) = 00000001; if (!p0.new) r2 = 00000000; p0 = cmp.eq(r21,00000000) }
	{ if (!p0) r14 = add(r20,FFFFFFFF); if (!p0) r3 = 00000000; if (p0) jump:nt 0001BA40 }

l0001B9D0:
	{ r4 = r28; if (p0.new) jump:nt 0001BA38; p0 = cmp.eq(r11,00000000); r6 = mpyi(r2,r20) }

l0001B9DC:
	{ r5 = mpyi(r2,r19); loop1(0001B9EC,r19) }
	{ r6 = addasl(r15,r6,00000002); r5 = addasl(r0,r5,00000002) }
	{ r7 = r3; if (p0.new) jump:nt 0001BA2C; p0 = cmp.eq(r12,00000000); r12 = addasl(r4,r19,00000002) }

l0001B9F8:
	{ r8 = memw(r6); r9 = memw(r4); r13 = add(r6,00000004); r7 = r3 }
	{ if (!p0.new) jump:t 0001BA28; p0 = cmp.gtu(r12,-00000001); loop0(0001BA10,r14) }

l0001BA10:
	{ r9 = memw(r12); r8 = memw(r13); r12 = addasl(r12,r19,00000002); r7 += sfmpy(r8,r9) }
	{ nop; r13 = add(r13,00000004) }

l0001BA28:
	{ r7 += sfmpy(r8,r9) }

l0001BA2C:
	{ memw(r5) = r7; r4 = add(r4,00000004); nop; r5 = add(r5,00000004) }

l0001BA38:
	{ if (!cmp.eq(r2.new,r21)) jump:t 0001B9D0; r2 = add(r2,00000001) }

l0001BA40:
	{ r2 = r16; r1 = 00000068; r4 = add(PC,0000D3E6) }

l0001BA44:
	{ r2 = r16; r1 = 00000068; r4 = add(PC,00000026) }

l0001BA50:
	{ call logmsg_function }
	{ r0 = 00000000 }

l0001BA58:
	{ r19:r18 = memd(r29+72); r17:r16 = memd(r29+80) }
	{ r23:r22 = memd(r29+56); r21:r20 = memd(r29+64) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+48) }
	{ dealloc_return }

;; matmul_check_ref: 0001BA6C
matmul_check_ref proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000D2C8) }
	{ r17 = r0; r16 = r1; r1 = 0000006F }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1 = 00000070; r3 = add(PC,0000D2C4) }
	{ if (!cmp.eq(r2.new,00000004)) jump:t 0001BAD8; r2 = memw(r17+16) }

l0001BAA0:
	{ r1 = 00000071; r3 = add(PC,00000006) }
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0001BAD8; r2 = memw(r17+20) }

l0001BAB4:
	{ r1 = 00000072; r3 = add(PC,00000009) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0001BAD8; r2 = memw(r17+4) }

l0001BAC8:
	{ r1 = 00000073; r3 = add(PC,00000001) }
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001BAE8; r2 = memw(r17+8) }

l0001BAD8:
	{ r2 = r16; call errlog_function }

l0001BADC:
	{ r2 = r16 }

l0001BAE0:
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001BAE8:
	{ r2 = r16; r1 = 00000074; r4 = add(PC,0000D2AA) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001BB08
;;   Called from:
;;     0001B8DC (in matmul_execute_ref)
;;     0001B910 (in matmul_execute_ref)
;;     0001BA50 (in matmul_execute_ref)
;;     0001BA80 (in matmul_check_ref)
;;     0001BAF8 (in matmul_check_ref)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001BB2C; r5 = memw(r2+16) }

l0001BB18:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000002) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001BB2C:
	{ dealloc_return }

;; errlog_function: 0001BB30
;;   Called from:
;;     0001B988 (in matmul_execute_ref)
;;     0001BAD8 (in matmul_check_ref)
;;     0001BB20 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000D1E6) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001BB54             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; maxpool_execute: 0001BB60
maxpool_execute proc
	{ allocframe(000000A0); memd(r29+472) = r23:r22; r23:r22 = combine(r1,r0) }
	{ memd(r29+144) = r19:r18; r2 = memw(r22+4) }
	{ r18 = memb(r22+32) }
	{ memd(r29+136) = r21:r20; memd(r29+152) = r17:r16 }
	{ r3 = memw(r22+8); r20 = memw(r2+8); p0 = cmp.eq(r18,00000000) }
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
	{ if (p0.new) jump:nt 0001BBDC; p0 = cmp.eq(r10,00000004) }

l0001BBC8:
	{ r21 = 00000000; if (!p0.new) jump:nt 0001BBF4; p0 = cmp.eq(r10,00000002) }

l0001BBD0:
	{ r2 = memd(r29+36); r0 = r19 }
	{ jump 0001BBE8; r0 += add(r2,FFFFFFFF) }

l0001BBDC:
	{ r3 = memd(r29+32); r2 = memd(r29+36) }
	{ r2 = add(r19,r2) }
	{ r0 = sub(r2,r3) }

l0001BBE8:
	{ r1 = r19; call fn00009760 }
	{ r21 = r0 }

l0001BBF4:
	{ nop; if (p0.new) jump:nt 0001BC30; p0 = cmp.eq(r10,00000004) }

l0001BBFC:
	{ if (p0.new) r1 = memw(r29+76); if (p0.new) jump:nt 0001BC24; p0 = cmp.eq(r10,00000002) }

l0001BC04:
	{ memb(r29+18) = r2.new; r0 = memw(r29+104); r2 = 00000000 }
	{ if (p0.new) r1 = memw(r29+76); if (p0.new) r0 = add(r24,00000000); if (!p0.new) jump:nt 0001BC48 }

l0001BC20:
	{ jump 0001BC3C }

l0001BC24:
	{ r0 = r1 }
	{ jump 0001BC3C; r0 += add(r24,FFFFFFFF) }

l0001BC30:
	{ r3 = memd(r29+68); r1 = memd(r29+76) }
	{ r2 = add(r1,r24) }
	{ r0 = sub(r2,r3) }

l0001BC3C:
	{ call fn00009760 }
	{ memw(r29+72) = r0 }
	{ r2 = r23; r1 = 00000054; r4 = add(PC,0000D2A7) }

l0001BC48:
	{ r2 = r23; r1 = 00000054; r4 = add(PC,00000027) }

l0001BC54:
	{ r3 = memw(r17+16); r7 = memw(r16+16) }
	{ memw(r29+100) = r3; memw(r29+104) = r7 }
	{ memw(r29) = r22; call logmsg_function }
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0001BC84; r2 = memw(r26) }

l0001BC70:
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0001BC88 }

l0001BC78:
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0001BC88 }

l0001BC80:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001BCA4 }

l0001BC84:
	{ r1 = 00000059; r3 = add(PC,0000D281) }

l0001BC88:
	{ r1 = 00000059; r3 = add(PC,00000001) }

l0001BC90:
	{ r2 = r23; call errlog_function }

l0001BC94:
	{ r2 = r23 }
	{ r0 = FFFFFFFF; jump 0001BF7C }
0001BCA0 02 59 1B ED                                     .Y..            

l0001BCA4:
	{ r4 = memd(r29+72); r3 = memw(r16+20); r1 = 0000005B }
	{ r2 = mpyi(r2,r21) }
	{ r2 = mpyi(r2,r4) }
	{ if (!cmp.gtu(r2.new,r3)) jump:t 0001BCC8; r2 = asl(r2,00000002) }

l0001BCC0:
	{ jump 0001BC94; r3 = add(PC,00000021) }

l0001BCC8:
	{ if (!cmp.eq(r3.new,00000000)) jump:t 0001BCE4; r3 = memb(r22+32); p0 = cmp.gt(r27,00000000) }

l0001BCD8:
	{ r1 = 0000005C; jump 0001BC94; r3 = add(PC,00000017) }

l0001BCE4:
	{ memw(r16+24) = r2; r2 = memd(r29+72); if (p0) r1 = FF800000 }
	{ memw(r29+12) = r22; memw(r29+8) = r23 }
	{ memw(r16) = r27; memw(r16+4) = r2 }
	{ memw(r16+12) = r25; memw(r16+8) = r21 }
	{ if (p0) r12 = memw(r29+32); if (p0) r9 = memw(r29+68); if (!p0) jump:nt 0001BF5C }

l0001BD10:
	{ r4 = memd(r29+72); r2 = r21; r13 = r12; r0 = 00000000 }
	{ r5 = memd(r29+36); r7 = memd(r29+76); r4 = sub(r9,r24); r8 = add(r4,FFFFFFFF) }
	{ memw(r29+60) = r24; memw(r29+28) = r0; r3 = sub(r12,r5); r6 = mpyi(r5,r25) }
	{ r7 = sub(FFFFFFFF,r5); r8 = add(r4,mpyi(r8,r7)) }
	{ memw(r29+96) = r7; r3 = sub(FFFFFFFF,r24); r8 += lsr(r8,0000001F); r2 = add(r3,mpyi(r2,r19)) }
	{ memw(r29+52) = r3; r7 = 00000000; r3 = asl(r25,00000002); r2 += lsr(r2,0000001F) }
	{ memw(r29+92) = r3; r4 = 00000000; r3 = asr(r8,00000001) }
	{ memw(r29+48) = r3; memw(r29+64) = r7; r0 = sub(00000000,r3) }
	{ memw(r29+20) = r0; r7 = asr(r2,00000001); r2 = add(r3,sub(0000007F,r9)) }
	{ memw(r29+16) = r2; r2 = sub(00000000,r7); r0 = add(r7,sub(0000007F,r12)) }
	{ memw(r29+40) = r2; memw(r29+44) = r0 }

l0001BD88:
	{ memw(r29+24) = r27; r2 = memw(r29+72) }
	{ if (!p0.new) jump:nt 0001BF38; p0 = cmp.gt(r2,00000001) }

l0001BD94:
	{ memb(r29+22) = r2.new; r2 = memw(r29+20) }
	{ r0 = memd(r29+16); r3 = memd(r29+28) }
	{ memw(r29+84) = r0; r0 = 00000000; r2 = mpyi(r3,r2) }
	{ memw(r29+80) = r0; memw(r29+56) = r2 }

l0001BDB0:
	{ if (p0.new) r2 = memw(r29+76); if (p0.new) r9 = memw(r29+80); if (!p0.new) jump:nt 0001BF0C; p0 = cmp.gt(r13,00000001) }

l0001BDBC:
	{ r12 = memw(r29+84); r14 = memw(r29+52) }
	{ r8 = memw(r29+56); r3 = memw(r29+88); r2 = mpyi(r9,r2) }
	{ r15 = memw(r29+64); r14 = memw(r29+48); r8 = add(r9,r8); r12 = max(r12,r14) }
	{ r0 = memw(r29+60); r14 = memw(r29+68); r17 = sub(r2,r14); r3 = max(r3,r4) }
	{ r26 = sub(FFFFFFFF,r12); r8 = add(r17,r14); r3 = add(r15,r3); r20 = mpyi(r8,r21) }
	{ r28 = memw(r29+44); r16 = memw(r29+40); r9 = 00000000; r2 = max(r17,r4) }
	{ r15 = min(r0,r8); r12 = mpyi(r5,r3) }

l0001BE18:
	{ p0 = cmp.gt(r25,00000000); r8 = add(r9,r20); if (!p0.new) jump:nt 0001BEFC; r3 = mpyi(r9,r19) }

l0001BE28:
	{ r17 = memd(r29+100); r0 = memd(r29+96); r11 = mpyi(r8,r25); r14 = max(r16,r4) }
	{ r23 = add(r12,r14); r3 = sub(r3,r7) }
	{ r0 = 00000000; r3 = add(r3,r13); r22 = max(r3,r4); r10 = max(r28,r0) }
	{ r10 = memw(r29+92); r18 = sub(FFFFFFFF,r10) }
	{ r3 = memw(r29+104); r8 = sub(r18,r14); r10 = add(r17,mpyi(r10,r23)); r18 = min(r5,r3) }
	{ r27 = addasl(r3,r11,00000002) }

l0001BE68:
	{ p0 = cmp.gt(r15,r2); r3 = r1; r14 = sub(r26,r2); if (!p0.new) jump:nt 0001BEE4 }

l0001BE78:
	{ r3 = r1; r11 = r10; loop1(0001BE84,r14) }
	{ r24 = add(r8,FFFFFFFF); if (!p0.new) jump:nt 0001BED8; p0 = cmp.gt(r10,-00000001); r23 = addasl(r11,r25,00000002) }

l0001BE90:
	{ r14 = memw(r11); p0 = cmp.gtu(r8,00000001) }
	{ if (p0) r17 = add(r24,FFFFFFFF); if (!p0) r23 = add(r14,00000000); if (!p0) jump:nt 0001BED4 }

l0001BEA4:
	{ r23 = memw(r23); p0 = cmp.gtu(r24,00000001); r24 = addasl(r23,r25,00000002); loop0(0001BEB8,r17) }
	{ if (!p0) jump:nt 0001BED0 }

l0001BEB8:
	{ r23 = memw(r24); r17 = r23; r24 = addasl(r24,r25,00000002); r3 = sfmax(r14,r3) }
	{ nop; r14 = r17 }

l0001BED0:
	{ r3 = sfmax(r14,r3) }

l0001BED4:
	{ r3 = sfmax(r23,r3) }

l0001BED8:
	{ nop; nop; r11 = addasl(r11,r6,00000002) }

l0001BEE4:
	{ memw(r27) = r3; r27 = add(r27,00000004); r10 = add(r10,00000004); r0 = add(r0,00000001) }
	{ p0 = cmp.eq(r0,r25); if (!p0.new) jump:nt 0001BE68 }

l0001BEFC:
	{ if (!cmp.eq(r9.new,r21)) jump:t 0001BE18; r9 = add(r9,00000001); r28 = sub(r28,r19); r16 = add(r16,r19) }

l0001BF0C:
	{ r3 = memd(r29+88); r2 = memd(r29+76) }

l0001BF10:
	{ r8 = memw(r29+80); r0 = memw(r29+84); r3 = add(r3,r2) }
	{ memw(r29+88) = r3; r2 = memd(r29+72); r8 = add(r8,00000001); r3 = sub(r0,r2) }
	{ memw(r29+84) = r3; memw(r29+80) = r8; p0 = cmp.eq(r8,r2) }
	{ if (!p0) jump:nt 0001BDB0 }

l0001BF38:
	{ r27 = memw(r29+24); r8 = memw(r29+28) }
	{ r3 = memd(r29+64); r2 = memd(r29+60); r8 = add(r8,00000001) }
	{ memw(r29+28) = r8; p0 = cmp.eq(r8,r27); r3 = add(r3,r2) }
	{ memw(r29+64) = r3; if (!p0) jump:nt 0001BD88 }

l0001BF5C:
	{ memb(r29) = r2.new; r2 = memw(r29+12); r4 = add(PC,0000CFE1) }
	{ r2 = memw(r29+8) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0001BF7C:
	{ r19:r18 = memd(r29+144); r17:r16 = memd(r29+152) }
	{ r23:r22 = memd(r29+128); r21:r20 = memd(r29+136) }
	{ r27:r26 = memd(r29+112); r25:r24 = memd(r29+120) }
	{ dealloc_return }

;; maxpool_check: 0001BF90
maxpool_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000CED0) }
	{ r17 = r0; r16 = r1; r1 = 0000008B }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001BFD0; r2 = memw(r17+16); r1 = 0000008C }

l0001BFBC:
	{ r3 = add(PC,00000001) }

l0001BFC0:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001BFD0:
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0001BFE4; r2 = memw(r17+20); r1 = 0000008D }

l0001BFE0:
	{ r3 = memw(r17+4) }

l0001BFE4:
	{ jump 0001BFC0; r3 = add(PC,0000CEAC) }
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
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001C07C; r5 = memw(r2+16) }

l0001C068:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000001D) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001C07C:
	{ dealloc_return }

;; errlog_function: 0001C080
;;   Called from:
;;     0001BC90 (in maxpool_execute)
;;     0001BFC0 (in maxpool_check)
;;     0001C070 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000CDC1) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001C0A4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; relu_execute: 0001C0B0
relu_execute proc
	{ allocframe(00000028); memd(r29+496) = r17:r16; r4 = add(PC,0000CF66) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+24) = r19:r18; r2 = memw(r17+4); r1 = 00000036 }
	{ r3 = memw(r17+8) }
	{ memd(r29+16) = r21:r20; memd(r29+8) = r23:r22 }
	{ r22 = memw(r3); r18 = memw(r2) }
	{ r6 = memw(r18+12); r0 = memw(r18) }
	{ r5 = memw(r18+8); r7 = memw(r18+4) }
	{ r20 = memw(r22+16); r19 = memw(r18+16); r2 = mpyi(r7,r0) }
	{ memw(r29) = r17 }
	{ r2 = r16; r3 = mpyi(r2,r5) }
	{ r21 = mpyi(r3,r6) }
	{ call logmsg_function; r23 = asl(r21,00000002) }
	{ if (!cmp.gtu(r23,r2.new)) jump:t 0001C11C; r2 = memw(r22+20) }

l0001C108:
	{ r2 = r16; r1 = 00000037; r3 = add(PC,0000000A) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001C170 }

l0001C11C:
	{ r3 = memw(r18+4); r2 = memw(r18); p0 = cmp.eq(r21,00000000) }
	{ memw(r22) = r2; memw(r22+4) = r3 }
	{ r6 = memw(r18+8) }
	{ memw(r22+8) = r6 }
	{ r7 = memw(r18+12); r18 = -00000001 }
	{ memw(r22+24) = r23; memw(r22+12) = r7; if (p0) jump:nt 0001C158 }

l0001C13C:
	{ r0 = memw(r19); r1 = r18; r21 = add(r21,FFFFFFFF); call fn00009600 }
	{ memw(r20) = r0; r19 = add(r19,00000004); p0 = cmp.eq(r21,00000000); r20 = add(r20,00000004) }
	{ if (!p0) jump:nt 0001C13C }

l0001C158:
	{ r2 = r16; r1 = 0000003F; r4 = add(PC,0000CED5) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l0001C170:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; relu_check: 0001C17C
relu_check proc
	{ jump relu_check__merged; r2 = 00000000 }

;; relu_check__merged: 0001C180
;;   Called from:
;;     0001C17C (in relu_check)
;;     0001C348 (in reluX_check)
relu_check__merged proc
	{ allocframe(00000020); p0 = cmp.eq(r2,00000001); r4 = add(PC,0000CDE9) }
	{ r7 = 00000001; r5 = 00000002; r6 = add(PC,0000CE5E) }
	{ memd(r29+16) = r19:r18; memd(r29+24) = r17:r16; if (!p0) r4 = add(r6,00000000); r16 = r1 }
	{ r19 = p0 }
	{  }
	{ memw(r29+8) = r19; r17 = r0 }
	{ memw(r29) = r17; r18 = mux(p0,r5,r7); call logmsg_function }
	{ if (cmp.eq(r2.new,r18)) jump:t 0001C1E4; r2 = memw(r17+16) }

l0001C1CC:
	{ r0 = memd(r29+8); r2 = r16; r3 = add(PC,00000038) }
	{ if (!p0.new) r1 = 00000062; if (p0.new) r1 = 0000006B; jump 0001C20C; p0 = r0 }

l0001C1E4:
	{ r0 = memw(r29+8) }
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001C224; r2 = memw(r17+20); p1 = r0 }

l0001C1F8:
	{ if (!p1) r1 = 00000063; if (p1) r1 = 0000006C }
	{ r3 = add(PC,0000CD8F) }
	{ r2 = r16 }

l0001C20C:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001C250 }
0001C218                         35 43 00 00 83 5B 49 6A         5C...[Ij
0001C220 F8 FF FF 59                                     ...Y            

l0001C224:
	{ if (!p1) r1 = 00000064; if (p1) r1 = 0000006D; if (p1) jump:nt 0001C23C }

l0001C230:
	{ jump 0001C244; r4 = add(PC,0000CDD0) }

l0001C23C:
	{ r4 = add(PC,0000CD63) }

l0001C244:
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r0 = 00000000 }

l0001C250:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24) }
	{ dealloc_return }

;; reluX_execute: 0001C258
reluX_execute proc
	{ allocframe(00000030); memd(r29+496) = r17:r16; r4 = add(PC,0000CD5E) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+32) = r19:r18; r3 = memw(r17+4); r1 = 00000052 }
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
	{ call logmsg_function; r24 = asl(r22,00000002) }
	{ if (!cmp.gtu(r24,r2.new)) jump:t 0001C2D4; r2 = memw(r23+20) }

l0001C2BC:
	{ r2 = r16; r1 = 00000053; r3 = add(PC,00000016) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001C338 }

l0001C2D4:
	{ r3 = memw(r19+4); r2 = memw(r19); p0 = cmp.eq(r22,00000000) }
	{ memw(r23) = r2; memw(r23+4) = r3 }
	{ r6 = memw(r19+8) }
	{ memw(r23+8) = r6 }
	{ r7 = memw(r19+12); r19 = -00000001 }
	{ memw(r23+24) = r24; memw(r23+12) = r7; if (p0) jump:nt 0001C31C }

l0001C2F8:
	{ r0 = memw(r20); r1 = r19; call fn00009600 }
	{ r22 = add(r22,FFFFFFFF); r1:r0 = combine(r0,r18); call fn000097B0 }
	{ memw(r21) = r0; r20 = add(r20,00000004); p0 = cmp.eq(r22,00000000); r21 = add(r21,00000004) }
	{ if (!p0) jump:nt 0001C2F8 }

l0001C31C:
	{ r2 = r16; r1 = 0000005B; r4 = add(PC,0000CCC0) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l0001C338:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ dealloc_return; r25:r24 = memd(r29+8) }

;; reluX_check: 0001C348
reluX_check proc
	{ jump relu_check__merged; r2 = 00000001 }

;; logmsg_function: 0001C34C
;;   Called from:
;;     0001C0F4 (in relu_execute)
;;     0001C164 (in relu_execute)
;;     0001C1B4 (in relu_check__merged)
;;     0001C244 (in relu_check__merged)
;;     0001C2A8 (in reluX_execute)
;;     0001C32C (in reluX_execute)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001C370; r5 = memw(r2+16) }

l0001C35C:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000035) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001C370:
	{ dealloc_return }

;; errlog_function: 0001C374
;;   Called from:
;;     0001C110 (in relu_execute)
;;     0001C20C (in relu_check__merged)
;;     0001C2C8 (in reluX_execute)
;;     0001C364 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000CBD9) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001C398                         00 00 00 00 00 00 00 00         ........

;; softmax_execute: 0001C3A0
softmax_execute proc
	{ allocframe(+00000048) }
	{ r3 = memw(r0+4); r2 = memw(r0+8) }
	{ memd(r29+64) = r17:r16; memd(r29+48) = r21:r20 }
	{ r20 = memw(r3); r5 = memw(r2); r2 = r1 }
	{ memd(r29+40) = r23:r22; memd(r29+56) = r19:r18 }
	{ r4 = memw(r20+24); r7 = memw(r5+20) }
	{ memd(r29+24) = r27:r26; memd(r29+32) = r25:r24; if (p0.new) r1 = 00000040; p0 = cmp.gtu(r4,r7) }
	{ if (p0) jump:nt 0001C4F8 }

l0001C3D0:
	{ r2 = memw(r20+4); r4 = memw(r20) }
	{ memw(r29+8) = r2; r3 = memw(r20+8); r2 = mpyi(r2,r4) }
	{ r16 = memw(r20+12) }
	{ memw(r29) = r4; memw(r29+12) = r5 }
	{ memw(r29+4) = r3 }
	{ if (!cmp.gt(r25.new,00000000)) jump:nt 0001C4C8; r26 = memw(r20+16); r25 = mpyi(r2,r3) }

l0001C3F8:
	{ r2 = memd(r29+12); r21 = 00000000 }
	{ r22 = memw(r2+16) }

l0001C400:
	{ r27 = memw(r26); r19:r18 = combine(r23,r16); p0 = cmp.gt(r16,00000000) }
	{ r17 = r27; r0 = r27 }
	{ memb(r29+4) = r1.new; if (!p0) jump:nt 0001C4B4; r1 = p0 }

l0001C420:
	{ r1 = r17; call fn00009600 }

l0001C424:
	{ r1 = r17 }

l0001C428:
	{ if (!cmp.eq(r18.new,00000000)) jump:t 0001C50C; r18 = add(r18,FFFFFFFF); r17 = r0; r2 = add(r19,00000004) }

l0001C43C:
	{ if (p0.new) r24 = add(r16,00000000); if (p0.new) r19 = 00000000; if (!p0.new) jump:nt 0001C4B4; p0 = r0 }

l0001C44C:
	{ r18 = 00000000 }

l0001C454:
	{ r24 = add(r24,FFFFFFFF); call fn00009780; r0 = sfsub(r27,r17) }
	{ p0 = cmp.eq(r24,00000000); r2 = add(r22,r19) }
	{ memw(r2) = r0; if (p0) jump:nt 0001C484; r18 = sfadd(r18,r0) }

l0001C474:
	{ r19 = add(r19,00000004); r2 = add(r23,r19) }
	{ r27 = memw(r2); jump 0001C454 }

l0001C484:
	{ r1 = r18; r0 = 3F800000; call fn00009610 }
	{ r2 = memw(r29+16) }
	{ if (p0.new) r2 = add(r22,00000000); if (!p0.new) jump:nt 0001C4B4; p0 = r2 }

l0001C4A0:
	{ loop0(0001C4A4,r16) }
	{ r3 = memw(r2) }
	{ r3 = sfmpy(r0,r3) }
	{ memw(r2) = r3; r2 = add(r2,00000004); nop }

l0001C4B4:
	{ r26 = addasl(r26,r16,00000002); r23 = addasl(r23,r16,00000002) }
	{ if (!cmp.eq(r21.new,r25)) jump:t 0001C400; r21 = add(r21,00000001); r22 = addasl(r22,r16,00000002) }

l0001C4C8:
	{ r2 = memd(r29); r3 = memd(r29+12); r0 = 00000000 }

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
	{ call errlog_function; r3 = add(PC,0000CBD6) }
	{ r0 = FFFFFFFF; jump 0001C4E4 }

l0001C50C:
	{ r0 = memw(r19) }
	{ jump 0001C420; r11 = r2 }

;; softmax_check: 0001C514
softmax_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000CB45) }
	{ r17 = r0; r16 = r1; r1 = 0000005A }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1 = 0000005B; r3 = add(PC,0000CB42) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0001C5A8; r2 = memw(r17+4) }

l0001C548:
	{ r1 = 0000005C; r3 = add(PC,0000003A) }
	{ if (cmp.eq(r4.new,00000000)) jump:nt 0001C5A8; r4 = memw(r17+8) }

l0001C55C:
	{ r1 = 0000005D; r3 = add(PC,00000033) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0001C5A8; r2 = memw(r2) }

l0001C570:
	{ r1 = 0000005E; r3 = add(PC,0000002C) }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0001C5A8; r2 = memw(r4) }

l0001C584:
	{ r1 = 0000005F; r3 = add(PC,00000026) }
	{ if (!cmp.eq(r2.new,00000002)) jump:t 0001C5A8; r2 = memw(r17+16) }

l0001C598:
	{ r1 = 00000060; r3 = add(PC,00000012) }
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001C5B8; r2 = memw(r17+20) }

l0001C5A8:
	{ r2 = r16; call errlog_function }

l0001C5AC:
	{ r2 = r16 }

l0001C5B0:
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001C5B8:
	{ r2 = r16; r1 = 00000061; r4 = add(PC,0000CAFD) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001C5D8
;;   Called from:
;;     0001C528 (in softmax_check)
;;     0001C5C8 (in softmax_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001C5FC; r5 = memw(r2+16) }

l0001C5E8:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000016) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001C5FC:
	{ dealloc_return }

;; errlog_function: 0001C600
;;   Called from:
;;     0001C4F8 (in softmax_execute)
;;     0001C5A8 (in softmax_check)
;;     0001C5F0 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000CA3A) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001C624             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; lrn_8_execute_ref: 0001C630
lrn_8_execute_ref proc
	{ allocframe(00000088); r2 = r1 }
	{ r5 = memw(r0+4); r4 = memw(r0+8) }
	{ memd(r29+128) = r17:r16; memd(r29+120) = r19:r18 }
	{ r18 = memw(r5); r3 = memw(r4) }
	{ memd(r29+104) = r23:r22; memd(r29+112) = r21:r20 }
	{ r7 = memw(r18+24); r6 = memw(r3+20) }
	{ memd(r29+88) = r27:r26; memd(r29+96) = r25:r24; p0 = cmp.gtu(r7,r6) }
	{ r13 = memw(r5+8); r15 = memw(r5+4); if (!p0) jump:nt 0001C678 }

l0001C660:
	{ memw(r29+4) = r7; r1 = 000000A1; r3 = add(PC,0000CAE2) }
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
	{ if (p0.new) memw(r29+60) = r6; if (!p0.new) r1 = 000000A6; if (!p0.new) jump:nt 0001C768; p0 = cmp.eq(r3,00000002) }

l0001C700:
	{ memw(r29+64) = r7; memw(r29+44) = r8; p0 = cmp.gt(r0,00000000) }
	{ memw(r29+8) = r28; memw(r29+68) = r1; if (p0) r1 = 437F0000 }
	{ memw(r29+16) = r14; memw(r29+12) = r15 }
	{ memw(r29+24) = r0; memw(r29+20) = r13 }
	{ if (!p0) jump:nt 0001C9A0 }

l0001C730:
	{ r0 = sfsub(r4,r27) }
	{ memw(r29+40) = r0; call fn00009610 }
	{ r22 = r0; r7 = 00000000; r2 = 00000000; r20 = mpyi(r16,r19) }
	{ memw(r29+28) = r7; memw(r29+36) = r2 }

l0001C74C:
	{ if (!p0.new) jump:nt 0001C988; p0 = cmp.gt(r25,00000000) }

l0001C750:
	{ r2 = memd(r29+28); r3 = 00000000 }
	{ r2 = mpyi(r2,r17) }
	{ memw(r29+32) = r2 }

l0001C75C:
	{ if (!p0.new) r3 = add(r3,00000001); jump 0001C984; if (p0.new) jump:nt 0001C77C; p0 = cmp.gt(r8,00000000) }

l0001C768:
	{ r3 = add(PC,0000C9F4) }

l0001C770:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001C9C4 }
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
	{ if (!p0.new) jump:nt 0001C75C; p0 = cmp.eq(r3,-00000001) }

l0001C988:
	{ r7 = memd(r29+24); r3 = memd(r29+28) }
	{ r2 = memd(r29+36); r3 = r3 }
	{ memw(r29+28) = r3; r2 = add(r2,r17); p0 = cmp.eq(r3,r7) }
	{ memw(r29+36) = r2; if (!p0) jump:nt 0001C74C }

l0001C9A0:
	{ r16 = memd(r29+20); r1 = memd(r29+12) }
	{ r0 = memw(r29+8); r18 = add(r16,00000010); r2 = add(r1,00000010); call lrn_8_execute_ref.extracted_region }
	{ r0 = memd(r29+16); r2 = r18; r1 = r16; call lrn_8_execute_ref.extracted_region }
	{ r0 = 00000000 }

l0001C9C4:
	{ r19:r18 = memd(r29+120); r17:r16 = memd(r29+128) }
	{ r23:r22 = memd(r29+104); r21:r20 = memd(r29+112) }
	{ r27:r26 = memd(r29+88); r25:r24 = memd(r29+96) }
	{ dealloc_return }

;; lrn_check: 0001C9D8
lrn_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000C71D) }
	{ r17 = r0; r16 = r1; r1 = 000000CB }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,0000000E)) jump:t 0001CA18; r2 = memw(r17+16); r1 = 000000CC }

l0001CA04:
	{ r3 = add(PC,0000000C) }
	{ r2 = r16; call errlog_function }

l0001CA0C:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001CA18:
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001CA30; r2 = memw(r17+20); r1 = 000000CD }

l0001CA28:
	{ jump 0001CA0C; r3 = add(PC,0000003B) }

l0001CA30:
	{ r2 = r16; r1 = 000000CE; r4 = add(PC,0000C700) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001CA50
;;   Called from:
;;     0001C9EC (in lrn_check)
;;     0001CA40 (in lrn_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001CA74; r5 = memw(r2+16) }

l0001CA60:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000000) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001CA74:
	{ dealloc_return }

;; errlog_function: 0001CA78
;;   Called from:
;;     0001C770 (in lrn_8_execute_ref)
;;     0001CA08 (in lrn_check)
;;     0001CA68 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000C664) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; lrn_8_execute_ref.extracted_region: 0001CA9C
;;   Called from:
;;     0001C9A4 (in lrn_8_execute_ref)
;;     0001C9B4 (in lrn_8_execute_ref)
lrn_8_execute_ref.extracted_region proc
	{ allocframe(+00000000) }
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
	{ allocframe(+000000E0) }
	{ r3 = memw(r0+4); r2 = memw(r0+8) }
	{ memd(r29+216) = r17:r16; memd(r29+192) = r23:r22; r16 = r1 }
	{ r2 = memw(r2); r23 = memw(r3) }
	{ memd(r29+200) = r21:r20; memd(r29+208) = r19:r18 }
	{ r5 = memw(r23+24); r4 = memw(r2+20) }
	{ memd(r29+176) = r27:r26; memd(r29+184) = r25:r24; if (p0.new) r2 = add(r16,00000000); p0 = cmp.gtu(r5,r4) }
	{ if (p0) memw(r29) = r4; memw(r29+52) = r23; if (!p0) jump:nt 0001CB2C }

l0001CB18:
	{ memw(r29+4) = r5; r1 = 000000A3; r3 = add(PC,0000C6BD) }
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
	{ memw(r29+104) = r6; p0 = cmp.eq(r2,00000001); if (p0.new) r2 = add(r9,00000000); if (p0.new) r0 = 00000000 }
	{ if (p0) memw(r29+32) = r2; memw(r29+44) = r7; if (!p0) jump:nt 0001CC10 }

l0001CBA8:
	{ p0 = cmp.gt(r9,00000000) }
	{ if (!p0) jump:nt 0001CF94 }

l0001CBB0:
	{ r3 = togglebit(r3,0000001E); r1:r0 = convert_sf2df(r6) }
	{ memw(r29+100) = r3; r4 = mpyi(r24,r22) }
	{ memw(r29+152) = r4 }
	{ memd(r29+88) = r1:r0; r4 = 00000000; r1:r0 = convert_sf2df(r5) }
	{ memw(r29+40) = r4; r4 = 00000000 }
	{ memw(r29+116) = r4 }
	{ memd(r29+80) = r1:r0; r1:r0 = convert_sf2df(r3) }
	{ memd(r29+72) = r1:r0 }

l0001CBE0:
	{ if (p0.new) r3 = memw(r29+116); p0 = cmp.gt(r25,00000000); r9 = 00000000; if (!p0.new) jump:nt 0001CF7C }

l0001CBF0:
	{ r3 = mpyi(r3,r25) }
	{ memw(r29+36) = r3 }

l0001CBF8:
	{ memw(r29+60) = r9; if (p0.new) r3 = add(r9,00000001); p0 = cmp.gt(r24,00000000); if (p0.new) jump:nt 0001CC2C }

l0001CC08:
	{ r9 = add(r9,00000001); jump 0001CF74 }

l0001CC10:
	{ r2 = r16; r1 = 000000A8; r3 = add(PC,0000C5DF) }

l0001CC20:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001CF94 }

l0001CC2C:
	{ r6 = memd(r29+40); r2 = memd(r29+36); r19 = 00000000 }
	{ memw(r29+120) = r3; r5 = add(r6,r9); r2 = add(r9,r2) }
	{ memw(r29+56) = r5; r2 = mpyi(r2,r24) }
	{ memw(r29+48) = r2 }

l0001CC4C:
	{ memw(r29+128) = r19; if (p0.new) jump:nt 0001CC5C; p0 = cmp.gt(r14,00000000) }

l0001CC54:
	{ r19 = add(r19,00000001); jump 0001CF64 }

l0001CC5C:
	{ memb(r29+16) = r3.new; r2 = memw(r29+48); r3 = add(r19,00000001) }
	{ r3 = 00000000 }
	{ memb(r29+31) = r2.new; r2 = mpyi(r2,r22) }

l0001CC7C:
	{ memw(r29+136) = r3; r2 = memw(r12+12); r27 = r23; r1 = 00000069 }
	{ r5 = memw(r12+4); r6 = add(r2,FFFFFFFF); r4 = add(PC,0000C58F) }
	{ r3 = memw(r23+16); r7 = memw(r12+8); r6 += lsr(r6,0000001F) }
	{ memw(r29) = r5; memw(r29+8) = r2; r8 = add(r7,FFFFFFFF) }
	{ memw(r29+4) = r7; r17 = r5; r8 += lsr(r8,0000001F) }
	{ memw(r29+140) = r3; memw(r29+132) = r20; r18 = asr(r8,00000001); r17 += lsr(r17,0000001F) }
	{ memw(r29+16) = r18; r21 = asr(r17,00000001); r20 = asr(r6,00000001) }
	{  }
	{ memw(r29+12) = r21; memw(r29+20) = r20; r23 = r9 }
	{ call logmsg_function }
	{ r2 = memd(r29+120); r4 = r22; r5 = r23; r7:r6 = combine(r19,r20) }
	{ r26 = r5 }
	{ r26 -= asr(r17,00000001) }
	{ r2 += asr(r17,00000001) }
	{ r27 = memw(r29+136); r19 = 00000000 }
	{ r17 = memd(r29+104); r2 = memd(r29+108); r26 = r5; r21 = add(r27,00000001) }
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
	{  }
	{ memd(r29+16) = r7:r6 }
	{ memd(r29+8) = r5:r4; r19:r18 = convert_sf2df(r17); r4 = add(PC,0000C3B5) }
	{ memd(r29+24) = r19:r18; memd(r29) = r1:r0; r1 = 0000007C }
	{ call logmsg_function }
	{ r0 = r17; call fn00009790 }
	{ r2 = memw(r29+100) }
	{ call fn00009780; r0 = sfmpy(r0,r2) }
	{ r1 = 0000007F; r17 = r0; r4 = add(PC,0000C396) }
	{ r7:r6 = convert_sf2df(r17) }
	{ memd(r29) = r19:r18 }
	{ memd(r29+16) = r7:r6; r7:r6 = memd(r29+72) }
	{ memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r2 = memw(r29+124); r1 = 000000BD; r4 = add(PC,0000C2FB) }
	{ r5 = memw(r29+116); r3 = memw(r29+140) }
	{ r19 = memw(r29+128); r2 = add(r2,r27) }
	{ r2 = addasl(r3,r2,00000002) }
	{  }
	{ memw(r29+12) = r27; r0 = memw(r2) }
	{ memw(r29) = r5; memw(r29+8) = r19 }
	{ memw(r29+4) = r26; r17 = sfmpy(r17,r0) }
	{ r7:r6 = convert_sf2df(r17) }
	{ memd(r29+16) = r7:r6; call logmsg_function }
	{ memw(r20) = r17; r3 = r21; r9 = r26; r20 = add(r20,00000004) }
	{ r12 = memw(r29+112); if (!p0.new) jump:nt 0001CC7C; p0 = cmp.eq(r13,-00000001) }

l0001CF60:
	{ r7 = memd(r29+44); r19 = memd(r29+64) }

l0001CF64:
	{ if (p0.new) r2 = memw(r29+32); if (p0.new) r9 = memw(r29+120); p0 = cmp.eq(r19,r24); if (!p0.new) jump:nt 0001CC4C }

l0001CF74:
	{ p0 = cmp.eq(r9,r25); if (!p0.new) jump:nt 0001CBF8 }

l0001CF7C:
	{ r3 = memd(r29+40); r4 = memd(r29+116) }
	{ r3 = add(r3,r25); r4 = add(r4,00000001) }
	{ memw(r29+40) = r3; memw(r29+116) = r4; if (!p0.new) jump:nt 0001CBE0; p0 = cmp.eq(r4,-00000001) }

l0001CF90:
	{ r0 = 00000000 }

l0001CF94:
	{ r19:r18 = memd(r29+208); r17:r16 = memd(r29+216) }
	{ r23:r22 = memd(r29+192); r21:r20 = memd(r29+200) }
	{ r27:r26 = memd(r29+176); r25:r24 = memd(r29+184) }
	{ dealloc_return }

;; lrn_check: 0001CFA8
lrn_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000C1DF) }
	{ r17 = r0; r16 = r1; r1 = 000000CB }
	{ call logmsg_function }
	{ memw(r29) = r17 }
	{ if (cmp.eq(r2.new,0000000A)) jump:t 0001CFEC; r2 = memw(r17+16); r1 = 000000CC }

l0001CFD8:
	{ r3 = add(PC,00000008) }
	{ r2 = r16; call errlog_function }

l0001CFE0:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001CFEC:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001D008; r2 = memw(r17+20); r1 = 000000CE }

l0001CFFC:
	{ r1 = 000000CD; jump 0001CFE0; r3 = add(PC,00000037) }

l0001D008:
	{ r4 = add(PC,0000C1B8) }
	{ memw(r29) = r17 }
	{ call logmsg_function }
	{ r0 = 00000000 }
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
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001D044; r5 = memw(r2+16) }

l0001D034:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,0000003C) }
	{ memw(r29+4) = r6; call logv }

l0001D044:
	{ dealloc_return }

;; errlog_function: 0001D048
;;   Called from:
;;     0001CC20 (in lrn_f_execute)
;;     0001CFDC (in lrn_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000C124) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001D06C                                     00 00 00 00             ....

;; nn_variable_read: 0001D070
nn_variable_read proc
	{ allocframe(+00000000) }
	{ r6 = memw(r1+28) }
	{ if (p0.new) r6 = memw(r1+8); if (p0.new) r7 = memw(r29+16); p0 = cmp.eq(r6,0000003E); if (p0.new) jump:nt 0001D0A0 }

l0001D088:
	{ r1 = 0000007D; r3 = add(PC,0000C217) }
	{ r2 = r0; call errlog_function }

l0001D098:
	{ r2 = r0 }
	{ dealloc_return; r0 = -00000001 }

l0001D0A0:
	{ r2 = memw(r14+r2<<#2) }
	{ if (!cmp.gtu(r6.new,r7)) jump:t 0001D0BC; r6 = memw(r2+24) }

l0001D0B0:
	{ r1 = 0000007E; jump 0001D098; r3 = add(PC,00000002) }

l0001D0BC:
	{ r6 = memw(r2) }
	{ memw(r3) = r6 }
	{ memb(r4) = r6.new; r6 = memw(r2+4) }
	{ memw(r5) = r4; r3 = memd(r29+20) }
	{ memb(r7) = r1.new; r1 = memw(r2+12) }
	{ memb(r3) = r4.new }
	{ r2 = memw(r2+24); call fn00009560 }
	{ dealloc_return; r0 = 00000000 }

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
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000C18D) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; nn_variable_write: 0001D118
nn_variable_write proc
	{ allocframe(00000000); r6 = r2 }
	{ r7 = memw(r1+28) }
	{ if (p0.new) r7 = memw(r1+8); if (p0.new) r2 = memw(r29+16); p0 = cmp.eq(r7,0000003E); if (p0.new) jump:nt 0001D148 }

l0001D130:
	{ r1 = 00000094; r3 = add(PC,0000C16F) }
	{ r2 = r0; call errlog_function }

l0001D140:
	{ r2 = r0 }
	{ dealloc_return; r0 = -00000001 }

l0001D148:
	{ r6 = memw(r30+r6<<#2) }
	{ if (!cmp.gtu(r2,r7.new)) jump:t 0001D164; r7 = memw(r6+20) }

l0001D158:
	{ r1 = 00000095; jump 0001D140; r3 = add(PC,0000001A) }

l0001D164:
	{ r1 = memd(r29+12); r0 = memw(r6+16) }
	{ memw(r6) = r3; memw(r6+24) = r2 }
	{ r3 = memw(r29+8) }
	{ memw(r6+4) = r4; memw(r6+8) = r5 }
	{ memw(r6+12) = r3; call fn00009560 }
	{ dealloc_return; r0 = 00000000 }

;; assign_execute: 0001D180
assign_execute proc
	{ allocframe(00000020); memd(r29+496) = r17:r16; r4 = add(PC,0000C1DC) }
	{ r1 = 000000A3; r17:r16 = combine(r0,r1) }
	{ memd(r29+16) = r19:r18; r2 = memw(r17+16) }
	{ memw(r29+4) = r2; r2 = r16 }
	{ memw(r29) = r17; memd(r29+8) = r21:r20 }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0001D1F8; r2 = memw(r17+16) }

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
	{ r2 = memw(r17+16); r19 = add(r19,00000008) }
	{ if (cmp.gtu(r2,r18.new)) jump:t 0001D1B0; r18 = add(r18,00000002) }

l0001D1F8:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0001D278; r2 = memw(r17+20); r0 = 00000000; r19:r18 = combine(00000000,00000004) }

l0001D1FC:
	{ if (cmp.eq(r2.new,00000000)) jump:nt 0001D27C; r2 = memw(r17+20); r0 = 00000000 }

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
	{ r2 = memw(r17+20); r18 = add(r18,00000008); r19 = add(r19,00000004) }
	{ if (cmp.gtu(r2,r20.new)) jump:t 0001D20C; r20 = add(r20,00000001) }

l0001D25C:
	{ memw(r29) = r20; r1 = 000000AC; r3 = add(PC,0000C139) }

l0001D26C:
	{ r2 = r16; call errlog_function }
	{ r0 = FFFFFFFF }

l0001D278:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24) }

l0001D27C:
	{ dealloc_return; r21:r20 = memd(r29+8) }

l0001D280:
	{ memw(r29) = r18; r1 = 000000A6; r3 = add(PC,0000C0FE) }
	{ jump 0001D26C }

;; assign_check: 0001D294
assign_check proc
	{ allocframe(00000000); r2 = r1 }
	{ r3 = memw(r0+16) }
	{ if (p0.new) r1 = 000000B5; if (!p0.new) jump:t 0001D2B0; p0 = tstbit(r3,00000000) }

l0001D2A4:
	{ jump 0001D2C8; r3 = add(PC,0000C097) }

l0001D2B0:
	{ r4 = memw(r0+20); r0 = 00000000; r1 = 000000B6 }
	{ if (!cmp.gtu(r4,r3.new)) jump:t 0001D2D0; r3 = lsr(r3,00000001) }

l0001D2C4:
	{ r3 = add(PC,0000000E) }

l0001D2C8:
	{ call errlog_function }
	{ r0 = FFFFFFFF }

l0001D2D0:
	{ dealloc_return }

;; variable_execute: 0001D2D4
variable_execute proc
	{ allocframe(00000008); r2 = r1; r4 = add(PC,0000C04C) }
	{ memw(r29) = r0; r1 = 0000002F; call logmsg_function }
	{ dealloc_return; r0 = 00000000 }
0001D2EC                                     00 C0 00 7F             ....

;; variable_check: 0001D2F0
variable_check proc
	{ allocframe(00000020); memd(r29+496) = r17:r16; r4 = add(PC,0000BFF8) }
	{ r17 = r0; r16 = r1; r1 = 00000037 }
	{ memd(r29+8) = r21:r20; memd(r29+16) = r19:r18; r2 = r16 }
	{ memw(r29) = r17; call logmsg_function }
	{ r2 = memw(r17+20) }
	{ if (cmp.gtu(r3.new,r2)) jump:t 0001D3A4; r3 = memw(r17+16) }

l0001D324:
	{ if (p0) jump:nt 0001D398 }

l0001D328:
	{ r19:r18 = combine(00000000,00000000); r20 = 00000000 }
	{ nop; if (!p0.new) jump:nt 0001D398; p0 = cmp.gtu(r2,r12) }

l0001D334:
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
	{ if (!cmp.gtu(r2,r20.new)) jump:nt 0001D398; r20 = add(r20,00000001) }

l0001D380:
	{ r2 = memw(r17+20); r19 = add(r19,00000004) }

l0001D384:
	{ r1 = 0000003B; r3 = add(PC,0000BF8E) }

l0001D390:
	{ r18 = -00000001; r2 = r16; call errlog_function }

l0001D398:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = r18 }
	{ dealloc_return; r21:r20 = memd(r29+8) }

l0001D3A4:
	{ jump 0001D390; r1 = 00000038; r3 = add(PC,0000BF5E) }

;; variable_ctor: 0001D3B0
variable_ctor proc
	{ allocframe(00000020); memd(r29+488) = r19:r18; r18 = r0 }
	{ memd(r29+24) = r17:r16; r17 = r5 }
	{ r19 = memw(r29+44) }
	{ memw(r29+4) = r19; r6 = memd(r29+40) }
	{ memw(r29) = r6; call node_alloc_common }
	{ if (cmp.eq(r16.new,00000000)) jump:nt 0001D480; r16 = r0 }

l0001D3D8:
	{ memb(r29+2) = r0.new; if (p0) jump:nt 0001D3F8; r0 = p0 }

l0001D3E8:
	{ r2 = add(r2,0000007F) }
	{ r2 = lsr(r2,00000007) }
	{ r2 = mpyi(r2,r17) }
	{ r1 = asl(r2,00000007) }

l0001D3F8:
	{ r0 = 00000080; call fn00009550 }
	{ memw(r16+40) = r0; if (!p0.new) jump:nt 0001D41C; p0 = cmp.eq(r0,00000000) }

l0001D408:
	{ r2 = r18; r1 = 0000005F; r3 = add(PC,0000BED1) }
	{ call errlog_function }

l0001D41C:
	{ r0 = memw(r29+8) }
	{ if (p0.new) jump:nt 0001D474; p0 = r0 }

l0001D428:
	{ r2 = memw(r16+8); r4 = memw(r16+8); r5 = add(r17,FFFFFFFF); r3 = 00000000 }
	{ p0 = cmp.gtu(r17,00000001); loop0(0001D450,r5) }
	{ r4 = memw(r12+r3); r3 = add(r3,00000004) }
	{ memw(r4+16) = r2 }
	{ r4 = memw(r19++#8); if (!p0) jump:nt 0001D474 }

l0001D450:
	{ r5 = memw(r16+8); r4 = add(r4,0000007F) }
	{ r4 = and(r4,FFFFFF80) }
	{ r5 = memw(r28+r3); r3 = add(r3,00000004); r2 = add(r2,r4) }
	{ memw(r5+16) = r2 }
	{ r4 = memw(r19++#8); nop }

l0001D474:
	{ r19:r18 = memd(r29+16); r17:r16 = memd(r29+24); r0 = r16 }
	{ dealloc_return }

l0001D480:
	{ r2 = r18; r1 = 00000058; r3 = add(PC,0000BE4E) }
	{ r16 = 00000000; call errlog_function }
	{ jump 0001D474 }

;; variable_dtor: 0001D49C
variable_dtor proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000BE1C) }
	{ r17 = r0; r16 = r1; r1 = 0000006B }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r0.new,00000000)) jump:nt 0001D4C4; r0 = memw(r17+40) }

l0001D4C4:
	{ deallocframe; r17:r16 = memd(r29+8); r1:r0 = combine(r16,r17); jump node_free_common }

;; logmsg_function: 0001D4D0
;;   Called from:
;;     0001D1A0 (in assign_execute)
;;     0001D2E0 (in variable_execute)
;;     0001D30C (in variable_check)
;;     0001D4B0 (in variable_dtor)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001D4F4; r5 = memw(r2+16) }

l0001D4E0:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000025) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001D4F4:
	{ dealloc_return }

l0001D4F8:
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; reshape_execute: 0001D500
;;   Called from:
;;     0001D4FC (in logmsg_function)
reshape_execute proc
	{ allocframe(00000080); memd(r29+496) = r17:r16; r17:r16 = combine(r0,r1) }
	{ memd(r29+88) = r25:r24; r2 = memw(r17+4) }
	{ memd(r29+96) = r23:r22 }
	{ memd(r29+104) = r21:r20; r3 = memw(r17+8) }
	{ r23 = memw(r2+4); r24 = memw(r2) }
	{ memd(r29+112) = r19:r18; r25 = memw(r3) }
	{ r4 = memw(r24+4); r7 = memw(r24) }
	{ r22 = memw(r24+12); r21 = memw(r24+8); r2 = mpyi(r4,r7) }
	{ memw(r29+72) = r7; memd(r29+80) = r27:r26; r6 = mpyi(r2,r21) }
	{ memw(r29+68) = r4 }
	{ if (!cmp.gt(r2.new,00000006)) jump:t 0001D568; r2 = memw(r23+12); r18 = mpyi(r6,r22) }

l0001D55C:
	{ r4 = addasl(r3,r2,00000002) }
	{ r26 = memw(r4-16); jump 0001D578 }

l0001D568:
	{ if (p0.new) r3 = memw(r23+16); r27:r26 = combine(00000001,00000001); if (!p0.new) jump:nt 0001D580; p0 = cmp.gt(r2,00000004) }

l0001D574:
	{ r26 = 00000001 }

l0001D578:
	{ r3 = addasl(r3,r2,00000002) }
	{ r27 = memw(r3-12) }

l0001D580:
	{ if (p0.new) r3 = memw(r23+16); if (!p0.new) jump:nt 0001D594; p0 = cmp.gt(r2,00000002) }

l0001D588:
	{ r4 = addasl(r3,r2,00000002) }
	{ r19 = memw(r4-8); jump 0001D5A0 }

l0001D594:
	{ r20 = 00000001; r19 = 00000001; if (!p0.new) jump:nt 0001D5A8; p0 = cmp.gt(r2,00000000) }

l0001D59C:
	{ r3 = memw(r23+16); r19 = 00000001 }

l0001D5A0:
	{ r2 = addasl(r3,r2,00000002) }
	{ r20 = memw(r2-4) }

l0001D5A8:
	{ r2 = r16; r1 = 0000003F; r4 = add(PC,0000BEA7) }
	{ memw(r29) = r17; call logmsg_function }
	{ r2 = memw(r25+20); r1 = 00000041; r3 = add(PC,0000BEB0) }
	{ if (cmp.gtu(r4.new,r2)) jump:t 0001D770; r4 = memw(r24+24) }

l0001D5D8:
	{ r1 = 00000043; r2 = lsr(r27,0000001F); r3 = add(PC,00000026) }
	{ r2 += lsr(r26,0000001F) }
	{ r2 += lsr(r19,0000001F) }
	{ r2 += lsr(r20,0000001F) }
	{ r0 = r18; r2 = sub(00000000,r26) }
	{ r2 = mpyi(r27,r2) }
	{ memb(r29+14) = r18.new; p0 = cmp.gt(r27,FFFFFFFF); r18 = p0; r2 = mpyi(r2,r19) }
	{ memb(r29+15) = r18.new; p0 = cmp.gt(r19,FFFFFFFF); r18 = p0 }
	{ memb(r29+13) = r18.new; p0 = cmp.gt(r20,FFFFFFFF) }
	{ memw(r29+64) = r18; call fn00009760 }
	{ r0 = memw(r25+16); r3 = memw(r29+56); r18 = r0 }
	{ r3 = memw(r29+52); if (!p2.new) r26 = add(r18,00000000); p2 = r3 }
	{ memw(r25) = r26 }
	{ r3 = memw(r29+60); if (!p0.new) r19 = add(r18,00000000); p0 = r3 }
	{ memw(r25+8) = r19 }
	{ r3 = memw(r29+64); if (!p1.new) r27 = add(r18,00000000); p1 = r3 }
	{ memw(r25+4) = r27 }
	{ if (p0.new) r18 = add(r20,00000000); p0 = r3 }
	{ memw(r25+12) = r18 }
	{ r2 = memw(r24+24) }
	{ memw(r25+24) = r2 }
	{ r2 = memw(r24+24); r1 = memw(r24+16); call vmemcpy_asm }
	{ if (!cmp.eq(r2.new,00000006)) jump:t 0001D71C; r2 = memw(r17+20) }

l0001D6A4:
	{ r3 = memw(r17+4); r2 = memw(r17+8) }
	{ r2 = memw(r2+4); r4 = memw(r3+8); r3 = add(PC,0000BDEE) }
	{ r5 = memw(r4+4); r7 = memw(r4) }
	{ memw(r2) = r7; memw(r2+4) = r5 }
	{ r6 = memw(r4+12); r0 = memw(r4+8) }
	{ memw(r2+12) = r6; memw(r2+8) = r0 }
	{ r5 = memw(r4+24) }
	{ if (cmp.gtu(r5,r7.new)) jump:t 0001D770; r7 = memw(r2+20) }

l0001D6D4:
	{ r1 = memw(r4+16); r2 = memw(r4+24); call fn00009560 }
	{ r3 = memw(r17+4); r2 = memw(r17+8); r1 = 00000054 }
	{ r4 = memw(r3+12); r3 = add(PC,0000BDB2) }
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
	{ r3 = memw(r23+4); r2 = memw(r23); r4 = add(PC,0000BD89) }
	{ r6 = memw(r23+12); r5 = memw(r23+8); r1 = 0000005D }
	{ memw(r29+44) = r18; r7 = memd(r29+68) }
	{ memw(r29+28) = r6; memw(r29+40) = r19 }
	{ memw(r29+32) = r26; memw(r29+36) = r27 }
	{ memw(r29+20) = r3; memw(r29+24) = r5 }
	{ memw(r29+8) = r21; r3 = memd(r29+72) }
	{ memw(r29+12) = r22; memw(r29+16) = r2; r2 = r16 }
	{ memw(r29) = r3; memw(r29+4) = r7 }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0001D75C:
	{ r19:r18 = memd(r29+112); r17:r16 = memd(r29+120) }
	{ r23:r22 = memd(r29+96); r21:r20 = memd(r29+104) }
	{ r27:r26 = memd(r29+80); r25:r24 = memd(r29+88) }
	{ dealloc_return }

l0001D770:
	{ r2 = r16; call errlog_function }
	{ r0 = FFFFFFFF; jump 0001D75C }

;; reshape_check: 0001D780
reshape_check proc
	{ jump reshape_check__merged; r2 = 00000000 }

;; reshape_check__merged: 0001D784
;;   Called from:
;;     0001D780 (in reshape_check)
;;     0001D840 (in qreshape_check)
reshape_check__merged proc
	{ allocframe(00000028); p0 = cmp.eq(r2,00000001); r4 = add(PC,0000BC99) }
	{ memd(r29+24) = r19:r18; memd(r29+32) = r17:r16; if (p0) r19 = 0000006F; r17:r16 = combine(r0,r1) }
	{ if (p0) r1 = 0000006C; r18 = add(PC,0000BC9A) }
	{ memd(r29+8) = r23:r22; memd(r29+16) = r21:r20; if (p0) jump:nt 0001D7C4 }

l0001D7B0:
	{ r1 = 00000063; r21:r20 = combine(00000064,00000065); r19 = 00000066; r23:r22 = combine(00000002,00000001) }
	{ jump 0001D7DC }

l0001D7C4:
	{ r21:r20 = combine(0000006D,0000006E); r23:r22 = combine(00000004,00000003); r4 = add(PC,0000BC06) }
	{ r18 = add(PC,0000BC2F) }

l0001D7DC:
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,r23)) jump:t 0001D808; r2 = memw(r17+16); r1 = r21 }

l0001D7F4:
	{ r3 = add(PC,00000034) }
	{ r2 = r16; call errlog_function }

l0001D7FC:
	{ r2 = r16 }
	{ r0 = FFFFFFFF; jump 0001D834 }

l0001D808:
	{ if (cmp.eq(r2.new,r22)) jump:t 0001D820; r2 = memw(r17+20); r1 = r20 }

l0001D818:
	{ jump 0001D7FC; r3 = add(PC,0000001F) }

l0001D820:
	{ memw(r29) = r17; r4 = r18; r1 = r19; r2 = r16 }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0001D834:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; qreshape_check: 0001D840
qreshape_check proc
	{ jump reshape_check__merged; r2 = 00000001 }

;; logmsg_function: 0001D844
;;   Called from:
;;     0001D5B4 (in reshape_execute)
;;     0001D754 (in reshape_execute)
;;     0001D7DC (in reshape_check__merged)
;;     0001D82C (in reshape_check__merged)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001D868; r5 = memw(r2+16) }

l0001D854:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000001D) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001D868:
	{ dealloc_return }

;; errlog_function: 0001D86C
;;   Called from:
;;     0001D770 (in reshape_execute)
;;     0001D7F8 (in reshape_check__merged)
;;     0001D85C (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000BB41) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; slice_execute_f: 0001D890
slice_execute_f proc
	{ jump slice_impl; r2 = 00000004 }

;; slice_check_f: 0001D894
slice_check_f proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000BC5F) }
	{ r17 = r0; r16 = r1; r1 = 00000085 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001D8C8; r2 = memw(r17+16); r1 = 00000086 }

l0001D8C0:
	{ jump 0001D8E4; r3 = add(PC,0000000E) }

l0001D8C8:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001D8EC; r2 = memw(r17+20); r0 = 00000000; r1 = 00000087 }

l0001D8DC:
	{ r3 = add(PC,0000003D) }
	{ r2 = r16; call errlog_function }

l0001D8E4:
	{ r2 = r16 }

l0001D8E8:
	{ r0 = FFFFFFFF }

l0001D8EC:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; slice_execute_8: 0001D8F0
slice_execute_8 proc
	{ jump slice_impl; r2 = 00000001 }

;; slice_check_8: 0001D8F4
slice_check_8 proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000BBFF) }
	{ r17 = r0; r16 = r1; r1 = 0000008D }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001D928; r2 = memw(r17+16); r1 = 0000008E }

l0001D920:
	{ jump 0001D944; r3 = add(PC,0000002E) }

l0001D928:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001D94C; r2 = memw(r17+20); r0 = 00000000; r1 = 0000008F }

l0001D93C:
	{ r3 = add(PC,0000001D) }
	{ r2 = r16; call errlog_function }

l0001D944:
	{ r2 = r16 }

l0001D948:
	{ r0 = FFFFFFFF }

l0001D94C:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; slice_execute_q8: 0001D950
slice_execute_q8 proc
	{ allocframe(00000008); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
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
	{ deallocframe; r17:r16 = memd(r29); r2 = 00000001; r1:r0 = combine(r17,r16) }
	{ jump slice_impl }
0001D9C8                         00 40 00 7F 00 C0 00 7F         .@......

;; slice_check_q8: 0001D9D0
slice_check_q8 proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000BB23) }
	{ r17 = r0; r16 = r1; r1 = 00000095 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,0000000A)) jump:t 0001DA04; r2 = memw(r17+16); r1 = 00000096 }

l0001D9FC:
	{ jump 0001DA20; r3 = add(PC,00000012) }

l0001DA04:
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001DA28; r2 = memw(r17+20); r0 = 00000000; r1 = 00000097 }

l0001DA18:
	{ r3 = add(PC,00000001) }
	{ r2 = r16; call errlog_function }

l0001DA20:
	{ r2 = r16 }

l0001DA24:
	{ r0 = FFFFFFFF }

l0001DA28:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001DA2C
;;   Called from:
;;     0001D8A8 (in slice_check_f)
;;     0001D908 (in slice_check_8)
;;     0001D9E4 (in slice_check_q8)
;;     0001DAB4 (in slice_impl)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001DA50; r5 = memw(r2+16) }

l0001DA3C:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000020) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

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
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000BA84) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; slice_impl: 0001DA78
;;   Called from:
;;     0001D890 (in slice_execute_f)
;;     0001D8F0 (in slice_execute_8)
;;     0001D9C4 (in slice_execute_q8)
slice_impl proc
	{ allocframe(+00000058); r4 = add(PC,0000BAA9) }
	{ r5 = memw(r0+8); r3 = memw(r0+4) }
	{ memd(r29+80) = r17:r16; memd(r29+56) = r23:r22; r1 = 00000040; r17:r16 = combine(r1,r2) }
	{ r23 = memw(r5); r22 = memw(r3); r2 = r17 }
	{ memd(r29+64) = r21:r20; memd(r29+72) = r19:r18 }
	{ memd(r29+48) = r25:r24 }
	{ memd(r29+40) = r27:r26; r20 = memw(r3+8) }
	{ r18 = memw(r22+16); r24 = memw(r3+4) }
	{ memw(r29) = r0; r19 = memw(r23+16); call logmsg_function }
	{ r6 = memw(r20+4); r3 = memw(r20); r1 = 00000054 }
	{ r2 = memw(r24); r13 = memw(r22); p1 = cmp.eq(r6,FFFFFFFF); p0 = cmp.eq(r3,FFFFFFFF) }
	{ r14 = memw(r20+12); r8 = memw(r20+8) }
	{ r12 = memw(r24+4); r9 = memw(r22+4); if (!p0) r20 = add(r3,00000000); if (p0) r20 = sub(r13,r2) }
	{ r4 = memw(r22+8); p2 = cmp.eq(r8,FFFFFFFF); if (p1) r21 = sub(r9,r12); r3 = mpyi(r20,r16) }
	{ r6 = memw(r24+12); r5 = memw(r24+8); p0 = cmp.eq(r14,FFFFFFFF); if (!p1) r21 = add(r6,00000000) }
	{ r7 = memw(r22+12); if (p2) r26 = sub(r4,r5); r3 = mpyi(r3,r21) }
	{ if (!p0) r8 = add(r14,00000000); if (p0) r8 = sub(r7,r6); if (!p2) r26 = add(r8,00000000) }
	{ r3 = mpyi(r3,r26) }
	{ r14 = mpyi(r3,r8); r3 = add(PC,0000BA0F) }
	{ if (cmp.gtu(r14,r15.new)) jump:t 0001DBEC; r15 = memw(r23+20) }

l0001DB40:
	{ p0 = cmp.gt(r20,00000000); r1 = 00000055; r3 = add(PC,00000009) }
	{ if (p0) r1 = 00000056; if (!p0) jump:nt 0001DBEC }

l0001DB54:
	{ if (p1.new) r1 = 00000057; p1 = cmp.gt(r21,00000000); r3 = add(PC,0000B9FC) }
	{ memb(r29+6) = r0.new; if (!p1) jump:nt 0001DBEC; r0 = p1 }

l0001DB74:
	{ if (p1.new) r1 = 00000058; p1 = cmp.gt(r26,00000000); r3 = add(PC,0000002B) }
	{ memb(r29+8) = r0.new; if (!p1) jump:nt 0001DBEC; r0 = p1 }

l0001DB90:
	{ p1 = cmp.gt(r8,00000000); r3 = add(PC,0000001A) }
	{ if (p1) r1 = 00000059; if (!p1) jump:nt 0001DBEC }

l0001DBA0:
	{ r3 = add(PC,0000B9D1) }
	{ if (!cmp.gtu(r13,r15.new)) jump:t 0001DBEC; r15 = add(r20,r2) }

l0001DBB4:
	{ r1 = 0000005A; r3 = add(PC,00000010) }
	{ if (!cmp.gtu(r9,r13.new)) jump:t 0001DBEC; r13 = add(r21,r12) }

l0001DBC8:
	{ r1 = 0000005B; r3 = add(PC,0000000B) }
	{ if (!cmp.gtu(r4,r13.new)) jump:t 0001DBEC; r13 = add(r26,r5) }

l0001DBDC:
	{ r1 = 0000005C; r3 = add(PC,00000006) }
	{ if (cmp.gtu(r7,r13.new)) jump:t 0001DBFC; r13 = add(r8,r6) }

l0001DBEC:
	{ r2 = r17; call errlog_function }

l0001DBF0:
	{ r2 = r17 }

l0001DBF4:
	{ r0 = FFFFFFFF; jump 0001DCCC }

l0001DBFC:
	{ memw(r23+4) = r21; memw(r23+24) = r14; r0 = 00000000 }
	{ memw(r23+8) = r26; memw(r23) = r20 }
	{ memw(r23+12) = r8; if (!p0) jump:nt 0001DCCC }

l0001DC18:
	{ r12 = r7; r3 = mpyi(r7,r4); r2 = add(r12,mpyi(r2,r9)) }
	{ r7 = 00000000; r23 = mpyi(r7,r16); r13 = mpyi(r8,r26) }
	{ r9 = mpyi(r3,r9); r4 = add(r5,mpyi(r4,r2)) }
	{ r3 = r16; r17 = mpyi(r8,r16); r25 = mpyi(r3,r16) }
	{ memb(r29+7) = r2.new; r2 = mpyi(r13,r16); r12 = add(r6,mpyi(r12,r4)) }
	{ r3 = add(r18,mpyi(r3,r12)) }
	{ memw(r29+12) = r1 }

l0001DC5C:
	{ memw(r29+16) = r3; r0 = memd(r29+24); r27 = 00000000; r24 = r3 }
	{ memw(r29+20) = r7; p0 = r0 }
	{ if (!p0) jump:nt 0001DCB4 }

l0001DC74:
	{ r0 = memd(r29+32); r18 = r19; r16 = r24; r22 = r26 }
	{ if (!p0.new) jump:nt 0001DCA8; p0 = r0 }

l0001DC88:
	{ r16 = add(r16,r23); r2 = r17; r1:r0 = combine(r16,r18); call fn00009560 }
	{ if (!cmp.eq(r22.new,00000001)) jump:t 0001DC88; r22 = add(r22,FFFFFFFF); r18 = add(r18,r17) }

l0001DCA4:
	{ r19 = add(r19,r2) }

l0001DCA8:
	{ if (!cmp.eq(r27.new,r21)) jump:t 0001DC74; r27 = add(r27,00000001); r24 = add(r24,r25) }

l0001DCB4:
	{ r2 = memd(r29+12); r7 = memd(r29+20) }

l0001DCB8:
	{ r3 = memw(r29+16) }
	{ if (!cmp.eq(r7.new,r20)) jump:t 0001DC5C; r7 = add(r7,00000001); r3 = add(r3,r2) }

l0001DCCC:
	{ r19:r18 = memd(r29+72); r17:r16 = memd(r29+80) }
	{ r23:r22 = memd(r29+56); r21:r20 = memd(r29+64) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+48) }
	{ dealloc_return }

;; split_execute_f: 0001DCE0
split_execute_f proc
	{ r3 = memw(r0+20); jump split_impl; r2 = 00000004 }

;; split_check_f: 0001DCE8
split_check_f proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000B8E0) }
	{ r17 = r0; r16 = r1; r1 = 00000075 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000004)) jump:t 0001DD1C; r2 = memw(r17+16); r1 = 00000076 }

l0001DD14:
	{ jump 0001DD38; r3 = add(PC,0000000F) }

l0001DD1C:
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001DD40; r2 = memw(r17+20); r0 = 00000000; r1 = 00000077 }

l0001DD30:
	{ r3 = add(PC,0000003E) }
	{ r2 = r16; call errlog_function }

l0001DD38:
	{ r2 = r16 }

l0001DD3C:
	{ r0 = FFFFFFFF }

l0001DD40:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; qsplit_execute_8: 0001DD44
qsplit_execute_8 proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r16 = r0 }
	{ memd(r29) = r19:r18; r2 = memw(r16+4); r18 = r1 }
	{ r4 = memw(r16+8); r19 = memw(r16+20) }
	{ r3 = memw(r2+8); r17 = add(r19,FFFFFFFE) }
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
	{ r3 = memw(r3+12); r2 = addasl(r2,r19,00000002) }
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
	{ r1:r0 = combine(r18,r16) }
	{ r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
	{ deallocframe; jump split_impl }

;; qsplit_check: 0001DDE0
qsplit_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000B7E8) }
	{ r17 = r0; r16 = r1; r1 = 0000007D }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000008)) jump:t 0001DE14; r2 = memw(r17+16); r1 = 0000007E }

l0001DE0C:
	{ jump 0001DE30; r3 = add(PC,00000017) }

l0001DE14:
	{ if (cmp.gtu(r2.new,00000004)) jump:t 0001DE38; r2 = memw(r17+20); r0 = 00000000; r1 = 0000007F }

l0001DE28:
	{ r3 = add(PC,00000006) }
	{ r2 = r16; call errlog_function }

l0001DE30:
	{ r2 = r16 }

l0001DE34:
	{ r0 = FFFFFFFF }

l0001DE38:
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001DE3C
;;   Called from:
;;     0001DCFC (in split_check_f)
;;     0001DDF4 (in qsplit_check)
;;     0001DEE4 (in split_impl)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001DE60; r5 = memw(r2+16) }

l0001DE4C:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000025) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001DE60:
	{ dealloc_return }

;; errlog_function: 0001DE64
;;   Called from:
;;     0001DD34 (in split_check_f)
;;     0001DE2C (in qsplit_check)
;;     0001DE54 (in logmsg_function)
;;     0001DFF4 (in split_impl)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000B749) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; split_impl: 0001DE88
;;   Called from:
;;     0001DCE0 (in split_execute_f)
;;     0001DDD8 (in qsplit_execute_8)
split_impl proc
	{ allocframe(00000040); memd(r29+472) = r23:r22; r22 = r0 }
	{ memd(r29+40) = r21:r20; r4 = memw(r22+4) }
	{ memd(r29+48) = r19:r18; memd(r29+56) = r17:r16; r17:r16 = combine(r1,r3) }
	{ memd(r29+24) = r25:r24; r21 = memw(r4+4); r18 = r2 }
	{ memd(r29+16) = r27:r26; r23 = memw(r22+8) }
	{ r26 = memw(r21+8); r20 = memw(r21+12) }
	{ r25 = memw(r21); r27 = memw(r21+4); r1:r0 = combine(r16,r20); call fn00009750 }
	{ r2 = memw(r21+24); r19 = r0 }
	{ r1:r0 = combine(r16,r2); call fn00009760 }
	{ r2 = r17; r1 = 00000046; r4 = add(PC,0000B722) }
	{ memw(r29) = r22; r24 = memw(r21+16); r21 = r0; call logmsg_function }
	{ if (p0.new) jump:nt 0001DF24; p0 = cmp.eq(r8,00000002) }

l0001DEFC:
	{ if (!p1.new) jump:t 0001DF74; p1 = cmp.gt(r8,00000000); loop0(0001DF04,r16) }

l0001DF04:
	{ r4 = memw(r3) }
	{ if (!cmp.gtu(r21,r5.new)) jump:t 0001DF5C; r5 = memw(r4+20) }

l0001DF14:
	{ memw(r29) = r2; r1 = 0000004D; r3 = add(PC,0000003C) }
	{ jump 0001DFF4 }

l0001DF24:
	{ r3 = memw(r22+4); r2 = memw(r22+8); r0 = FFFFFFFF }
	{ r2 = memw(r2); r3 = memw(r3+4) }
	{ r5 = memw(r3+4); r4 = memw(r3) }
	{ memw(r2) = r4; memw(r2+4) = r5 }
	{ r1 = memw(r3+12); r7 = memw(r3+8) }
	{ memw(r2+12) = r1; memw(r2+8) = r7 }
	{ r4 = memw(r3+24) }
	{ if (cmp.gtu(r4,r6.new)) jump:t 0001E000; r6 = memw(r2+20) }

l0001DF50:
	{ r1 = memw(r3+16); r2 = memw(r3+24); call fn00009560 }
	{ jump 0001E000; r0 = 00000000 }

l0001DF5C:
	{ memw(r4+24) = r21; r2 = r2 }
	{ r4 = memw(r3++#4) }
	{ memw(r4+8) = r26; memw(r4+4) = r27 }
	{ memw(r4) = r25; memw(r4+12) = r19 }

l0001DF74:
	{ memb(r29+2) = r2.new; r1:r0 = combine(r16,r20); call fn00009770; r2 = p1 }
	{ if (p0.new) r20 = 00000000; r1 = 00000057 }
	{ r1 = memd(r29+8); r0 = 00000000; r2 = mpyi(r27,r26); r17 = mpyi(r19,r18) }
	{ if (!p0.new) jump:nt 0001E000; p0 = r1 }

l0001DFA4:
	{ r21 = mpyi(r17,r16); r2 = mpyi(r2,r25) }
	{ r22 = mpyi(r2,r19) }

l0001DFB0:
	{ r19 = r20; r25 = r22; if (!p0.new) jump:nt 0001DFE0; p0 = cmp.gt(r14,00000000) }

l0001DFB4:
	{ r19 = r20; r25 = r22 }

l0001DFBC:
	{ r2 = memw(r31+r20<<#2) }
	{ r18 = memw(r2+16); r19 = add(r24,mpyi(r19,r17)) }

l0001DFC8:
	{ r19 = add(r19,r21); r2 = r17; r1:r0 = combine(r19,r18); call fn00009560 }
	{ if (!cmp.eq(r25.new,00000001)) jump:t 0001DFC8; r25 = add(r25,FFFFFFFF); r18 = add(r18,r17) }

l0001DFE0:
	{ if (!cmp.eq(r20.new,r16)) jump:t 0001DFB0; r20 = add(r20,00000001) }

l0001DFE4:
	{ if (!cmp.eq(r20.new,r16)) jump:t 0001DFB4 }

l0001DFEC:
	{ r3 = add(PC,0000B631) }

l0001DFF4:
	{ r2 = r17; call errlog_function }
	{ r0 = FFFFFFFF }

l0001E000:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ r27:r26 = memd(r29+16); r25:r24 = memd(r29+24) }
	{ dealloc_return }
0001E014             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; tanh_execute: 0001E020
tanh_execute proc
	{ allocframe(00000028); memd(r29+496) = r17:r16; r4 = add(PC,0000B6A0) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+16) = r21:r20; r2 = memw(r17+4); r1 = 00000037 }
	{ r3 = memw(r17+8) }
	{ memd(r29+24) = r19:r18; memd(r29+8) = r23:r22 }
	{ r22 = memw(r3); r21 = memw(r2) }
	{ r6 = memw(r21+12); r0 = memw(r21) }
	{ r5 = memw(r21+8); r7 = memw(r21+4) }
	{ r19 = memw(r22+16); r18 = memw(r21+16); r2 = mpyi(r7,r0) }
	{ memw(r29) = r17 }
	{ r2 = r16; r3 = mpyi(r2,r5) }
	{ r20 = mpyi(r3,r6) }
	{ call logmsg_function; r23 = asl(r20,00000002) }
	{ if (!cmp.gtu(r23,r2.new)) jump:t 0001E08C; r2 = memw(r22+20) }

l0001E078:
	{ r2 = r16; r1 = 00000038; r3 = add(PC,00000023) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001E0E0 }

l0001E08C:
	{ r3 = memw(r21+4); r2 = memw(r21); p0 = cmp.eq(r20,00000000) }
	{ memw(r22) = r2; memw(r22+4) = r3 }
	{ r6 = memw(r21+8) }
	{ memw(r22+8) = r6 }
	{ r7 = memw(r21+12) }
	{ memw(r22+24) = r23; memw(r22+12) = r7; if (p0) jump:nt 0001E0C4 }

l0001E0AC:
	{ r0 = memw(r18); r20 = r20; call fn000097F0 }
	{ memw(r19) = r0; r18 = add(r18,00000004); p0 = cmp.eq(r20,00000000); r19 = add(r19,00000004) }
	{ if (!p0) jump:nt 0001E0AC }

l0001E0C4:
	{ r2 = r16; r1 = 00000040; r4 = add(PC,0000B621) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l0001E0E0:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; tanh_check: 0001E0EC
tanh_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000B589) }
	{ r17 = r0; r16 = r1; r1 = 00000046 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001E12C; r2 = memw(r17+16); r1 = 00000047 }

l0001E118:
	{ r3 = add(PC,00000037) }
	{ r2 = r16; call errlog_function }

l0001E120:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001E12C:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001E144; r2 = memw(r17+20); r1 = 00000048 }

l0001E13C:
	{ jump 0001E120; r3 = add(PC,00000022) }

l0001E144:
	{ r2 = r16; r1 = 00000049; r4 = add(PC,0000B566) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001E164
;;   Called from:
;;     0001E064 (in tanh_execute)
;;     0001E0D4 (in tanh_execute)
;;     0001E100 (in tanh_check)
;;     0001E154 (in tanh_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001E188; r5 = memw(r2+16) }

l0001E174:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000029) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001E188:
	{ dealloc_return }

;; errlog_function: 0001E18C
;;   Called from:
;;     0001E080 (in tanh_execute)
;;     0001E11C (in tanh_check)
;;     0001E17C (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000B4CD) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; sigmoid_execute: 0001E1B0
sigmoid_execute proc
	{ allocframe(00000028); memd(r29+496) = r17:r16; r4 = add(PC,0000B5B2) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+16) = r21:r20; r2 = memw(r17+4); r1 = 00000037 }
	{ r3 = memw(r17+8) }
	{ memd(r29+24) = r19:r18; memd(r29+8) = r23:r22 }
	{ r22 = memw(r3); r21 = memw(r2) }
	{ r6 = memw(r21+12); r0 = memw(r21) }
	{ r5 = memw(r21+8); r7 = memw(r21+4) }
	{ r19 = memw(r22+16); r18 = memw(r21+16); r2 = mpyi(r7,r0) }
	{ memw(r29) = r17 }
	{ r2 = r16; r3 = mpyi(r2,r5) }
	{ r20 = mpyi(r3,r6) }
	{ call logmsg_function; r23 = asl(r20,00000002) }
	{ if (!cmp.gtu(r23,r2.new)) jump:t 0001E21C; r2 = memw(r22+20) }

l0001E208:
	{ r2 = r16; r1 = 00000038; r3 = add(PC,00000038) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001E290 }

l0001E21C:
	{ r3 = memw(r21+4); r2 = memw(r21); p0 = cmp.eq(r20,00000000) }
	{ memw(r22) = r2; memw(r22+4) = r3 }
	{ r6 = memw(r21+8) }
	{ memw(r22+8) = r6 }
	{ r7 = memw(r21+12); if (!p0) r21 = 3F000000 }
	{ memw(r22+24) = r23; memw(r22+12) = r7; if (p0) jump:nt 0001E274 }

l0001E244:
	{ r22 = 3F800000 }

l0001E24C:
	{ r2 = memw(r18) }
	{ call fn000097F0; r0 = sfmpy(r2,r21) }
	{ r20 = r20; r18 = add(r18,00000004); r2 = sfadd(r0,r22) }
	{ p0 = cmp.eq(r20,00000000) }
	{ memb(r19) = r2.new; r19 = add(r19,00000004); if (!p0) jump:nt 0001E24C; r2 = sfmpy(r2,r21) }

l0001E274:
	{ r2 = r16; r1 = 00000040; r4 = add(PC,0000B516) }

l0001E278:
	{ r2 = r16; r1 = 00000040; r4 = add(PC,00000016) }

l0001E284:
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l0001E290:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; sigmoid_check: 0001E29C
sigmoid_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000B475) }
	{ r17 = r0; r16 = r1; r1 = 00000046 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001E2DC; r2 = memw(r17+16); r1 = 00000047 }

l0001E2C8:
	{ r3 = add(PC,00000026) }
	{ r2 = r16; call errlog_function }

l0001E2D0:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001E2DC:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001E2F4; r2 = memw(r17+20); r1 = 00000048 }

l0001E2EC:
	{ jump 0001E2D0; r3 = add(PC,00000011) }

l0001E2F4:
	{ r2 = r16; r1 = 00000049; r4 = add(PC,0000B455) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001E314
;;   Called from:
;;     0001E1F4 (in sigmoid_execute)
;;     0001E284 (in sigmoid_execute)
;;     0001E2B0 (in sigmoid_check)
;;     0001E304 (in sigmoid_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001E338; r5 = memw(r2+16) }

l0001E324:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000012) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001E338:
	{ dealloc_return }

;; errlog_function: 0001E33C
;;   Called from:
;;     0001E210 (in sigmoid_execute)
;;     0001E2CC (in sigmoid_check)
;;     0001E32C (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000B3B6) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; qtanh_execute_ref: 0001E360
qtanh_execute_ref proc
	{ allocframe(00000040); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
	{ memd(r29+16) = r27:r26; r2 = memw(r16+4) }
	{ r7 = memw(r16+8) }
	{ memd(r29+48) = r19:r18; memd(r29+32) = r23:r22; r18 = 437F0000 }
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
	{ r2 = r17; r1 = 00000042; r4 = add(PC,0000B53E) }
	{ memw(r29) = r16; r19 = r0; call logmsg_function }
	{ if (!cmp.gtu(r25,r2.new)) jump:t 0001E41C; r2 = memw(r27+20); p0 = cmp.eq(r25,00000000) }

l0001E404:
	{ r2 = r17; r1 = 00000043; r3 = add(PC,00000003) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001E4E8 }

l0001E41C:
	{ r3 = memw(r26+4); r2 = memw(r26) }
	{ memw(r27) = r2; memw(r27+4) = r3 }
	{ r6 = memw(r26+8) }
	{ memw(r27+8) = r6 }
	{ memw(r29+12) = r17; r7 = memw(r26+12); if (!p0) r17 = 3F000000 }
	{ memw(r27+24) = r25; memw(r27+12) = r7; if (p0) jump:nt 0001E49C }

l0001E450:
	{ r27 = 3F800000; r26 = 42FF0000 }

l0001E460:
	{ r2 = memb(r22++#1); r0 = r23 }
	{ r2 = convert_w2sf(r2) }
	{ call fn000097F0; r0 += sfmpy(r19,r2) }
	{ r25 = add(r25,FFFFFFFF); r2 = r17; r3 = sfadd(r0,r27) }
	{ p0 = cmp.eq(r25,00000000); r2 += sfmpy(r3,r26) }
	{ r3 = convert_sf2uw(r2) }
	{ if (p1.new) r3 = FFFFFFFF; p1 = sfcmp.gt(r2,r18) }
	{ memb(r24++#1) = r3; if (!p0) jump:nt 0001E460 }

l0001E49C:
	{ memw(r21+12) = 00000001; r2 = memw(r21+16); r4 = add(PC,0000B4B5) }
	{ memw(r21) = 00000001; memw(r21+8) = 00000001; r1 = 00000056 }
	{ memw(r2) = BF800000; memw(r21+4) = FFFFFF81 }
	{ memw(r21+24) = 00000004; r2 = memw(r29+12) }
	{ memw(r20+8) = 00000001; r3 = memw(r20+16) }
	{ memw(r20+4) = 00000001; memw(r20) = 00000001 }
	{ memw(r3) = 3F800000; memw(r20+12) = FFFFFF81 }
	{ memw(r29) = r16; memw(r20+24) = 00000004; call logmsg_function }
	{ r0 = 00000000 }

l0001E4E8:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ r27:r26 = memd(r29+16); r25:r24 = memd(r29+24) }
	{ dealloc_return }
0001E4FC                                     00 C0 00 7F             ....

;; qtanh_check: 0001E500
qtanh_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000B45E) }
	{ r17 = r0; r16 = r1; r1 = 00000094 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001E540; r2 = memw(r17+16); r1 = 00000095 }

l0001E52C:
	{ r3 = add(PC,0000000C) }
	{ r2 = r16; call errlog_function }

l0001E534:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001E540:
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001E558; r2 = memw(r17+20); r1 = 00000096 }

l0001E550:
	{ jump 0001E534; r3 = add(PC,00000037) }

l0001E558:
	{ r2 = r16; r1 = 00000097; r4 = add(PC,0000B43B) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }
0001E578                         00 40 00 7F 00 C0 00 7F         .@......

;; qtanh_execute_hvx: 0001E580
qtanh_execute_hvx proc
	{ allocframe(00000050); memd(r29+496) = r17:r16; r4 = add(PC,0000B39A) }
	{ r17:r16 = combine(r1,r0) }
	{ memd(r29+64) = r19:r18; r3 = memw(r16+4); r1 = 00000072 }
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
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001E60C; r2 = memb(r16+32); r1 = 00000073 }

l0001E5F8:
	{ r3 = add(PC,0000003D) }
	{ r2 = r17; call errlog_function }

l0001E600:
	{ r2 = r17 }
	{ r0 = FFFFFFFF; jump 0001E964 }

l0001E60C:
	{ r3 = memw(r18+20); r1 = 00000074; r2 = mpyi(r22,r27) }
	{ r2 = mpyi(r2,r20) }
	{ if (!cmp.gtu(r20.new,r3)) jump:t 0001E630; r20 = mpyi(r2,r26) }

l0001E628:
	{ jump 0001E600; r3 = add(PC,0000001F) }

l0001E630:
	{ memw(r29+12) = r2; r3 = memw(r21); r2 = 000000FF; r22 = add(r20,000000FF) }
	{ memb(r18+1) = r4.new; r4 = memw(r21+4); r0 = r25; r27 = and(r22,FFFFFF00) }
	{ memw(r18) = r3 }
	{ r2 = memw(r21+8); r1 = 00000000; r3 = add(r22,r0) }
	{ memw(r29+16) = r0; memw(r18+8) = r2; r2 = r27 }
	{ r7 = memw(r21+12) }
	{ memw(r18+24) = r20; memw(r18+12) = r7; r21 = and(r3,FFFFFF00); call fn000095F0 }
	{ call fn000095F0 }
	{ r2 = r27 }
	{ r1:r0 = combine(r23,r21); r2 = r20; call fn00009560 }
	{ r2 = C0800000; r1 = sfsub(r24,r19) }
	{ if (!p0.new) jump:nt 0001E76C; p0 = sfcmp.ge(r19,r2) }

l0001E6A0:
	{ r18 = 40800000 }
	{ if (!p0.new) jump:nt 0001E76C; p0 = sfcmp.ge(r18,r24) }

l0001E6B0:
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r2,r1); call fn00009610 }
	{ r23 = r0; r2 = sfadd(r19,r18) }
	{ r1:r0 = combine(r23,r2); call fn00009610 }
	{ r2 = 41700000; r18 = convert_uw2sf(r0):chop }
	{ r0 = r2; call fn000097C0 }
	{ r3 = BF800000; r2 = 41FF0000 }
	{ r25 = memw(r29+20); p0 = cmp.gt(r20,00000000); r3 = sfadd(r0,r3); r2 = sfmpy(r23,r2) }
	{ r3 = convert_uw2sf(r3):chop; r2 = sfmpy(r2,r0) }
	{ if (p0) r2 = memw(r29+12); if (!p0) jump:nt 0001E828; r4 = convert_uw2sf(r2):chop }

l0001E718:
	{ r5 = convert_uw2sf(r0):chop }
	{ if (!p0.new) r3 = add(r4,00000000); p0 = cmp.gt(r4,r3); r5 += lsr(r5,0000001F) }
	{ r3 = sxth(r3); r2 = r21; r6 = mpyi(r2,r26) }
	{ r4 = asr(r5,00000001); loop0(0001E738,r6) }
	{ r6 = memb(r2); r7 = r4; r5 = 000000FF }
	{ r6 = add(r6,r18) }
	{ r7 += mpyi(r6,r3) }
	{ r6 = asr(r7,0000000F) }
	{ if (!p0.new) r5 = add(r6,00000000); p0 = cmp.gt(r6,000000FF); if (p0.new) jump:nt 0001E760 }

l0001E758:
	{ if (!p0.new) r5 = 00000000; p0 = cmp.gt(r6,FFFFFFFF) }

l0001E760:
	{ memb(r2++#1) = r5; nop }
	{ jump 0001E828 }

l0001E76C:
	{ r0 = 38D1B717; call fn00009600 }
	{ r1 = 437F0000; call fn00009610 }
	{ r25 = memw(r29+20); r3 = add(r21,r22); if (!p0.new) jump:nt 0001E828; p0 = cmp.gt(r12,00000000) }

l0001E790:
	{ r22 = and(r3,FFFFFF00); loop0(0001E79C,r20) }
	{ r3:r2 = combine(r22,r21) }
	{ r4 = memb(r2++#1); r5 = r19 }
	{ r4 = convert_w2sf(r4) }
	{ r3 = add(r3,00000004); r5 += sfmpy(r0,r4) }
	{ r1 = 41000000; r0 = 00000017 }
	{ call fn00009600 }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = memd(r29+12); r23 = r0; r18 = r21 }
	{ r24 = 40800000; r19 = mpyi(r2,r26) }

l0001E7EC:
	{ r2 = memw(r22) }
	{ r2 = sfadd(r2,r24) }
	{ call fn00009620; r0 = sfmpy(r23,r2) }
	{ r2 = 000000FF; r3 = convert_uw2sf(r0):chop }
	{ if (!p0.new) r2 = add(r3,00000000); p0 = cmp.gt(r3,000000FF); if (p0.new) jump:nt 0001E818 }

l0001E810:
	{ if (!p0.new) r2 = 00000000; p0 = cmp.gt(r3,FFFFFFFF) }

l0001E818:
	{ memb(r18++#1) = r2; r22 = add(r22,00000004); r19 = add(r19,FFFFFFFF) }
	{ if (!p0.new) jump:nt 0001E7EC; p0 = cmp.eq(r27,00000001) }

l0001E828:
	{ r3 = 01010101; r2 = add(PC,0000C8E8) }
	{ r5 = 80808080; r4 = 04040404 }
	{ r7 = 07070707; r6 = F8F8F8F8 }
	{  }
	{  }
	{  }
	{  }
	{ p0 = cmp.gt(r27,00000000) }
	{  }
	{ r4 = memw(r2-48); if (!p0) jump:nt 0001E900 }

l0001E87C:
	{ r6 = 00000007; r2 = add(r27,0000007F); r7 = 00000000 }
	{ r5 = lsr(r2,00000007) }
	{ r2 = memw(r29+16); r3 = 00000003; loop0(0001E8A8,r5) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }

l0001E900:
	{ r0 = memd(r29+24); r1 = memd(r29+16); r2 = r20; call fn00009560 }
	{ r5 = memw(r29+28); r2 = memw(r25+16); r4 = add(PC,0000B045) }
	{ memw(r25+8) = 00000001; memw(r25+12) = FFFFFF81; r1 = 0000008E }
	{ memw(r25+4) = FFFFFF81; memw(r25) = 00000001 }
	{ memw(r25+24) = 00000004; memw(r2) = BF800000; r2 = r17 }
	{ memw(r5+8) = 00000001; r3 = memw(r5+16) }
	{ memw(r5+4) = 00000001; memw(r5) = 00000001 }
	{ memw(r3) = 3F800000; memw(r5+12) = FFFFFF81 }
	{ memw(r29) = r16; memw(r5+24) = 00000004; call logmsg_function }
	{ r0 = 00000000 }

l0001E964:
	{ r19:r18 = memd(r29+64); r17:r16 = memd(r29+72) }
	{ r23:r22 = memd(r29+48); r21:r20 = memd(r29+56) }
	{ r27:r26 = memd(r29+32); r25:r24 = memd(r29+40) }
	{ dealloc_return }

;; logmsg_function: 0001E978
;;   Called from:
;;     0001E3EC (in qtanh_execute_ref)
;;     0001E4D8 (in qtanh_execute_ref)
;;     0001E514 (in qtanh_check)
;;     0001E568 (in qtanh_check)
;;     0001E5E0 (in qtanh_execute_hvx)
;;     0001E954 (in qtanh_execute_hvx)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001E99C; r5 = memw(r2+16) }

l0001E988:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000003C) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001E99C:
	{ dealloc_return }

;; errlog_function: 0001E9A0
;;   Called from:
;;     0001E410 (in qtanh_execute_ref)
;;     0001E530 (in qtanh_check)
;;     0001E5FC (in qtanh_execute_hvx)
;;     0001E990 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000AF60) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001E9C4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; qsigmoid_execute_ref: 0001E9D0
qsigmoid_execute_ref proc
	{ allocframe(00000040); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
	{ memd(r29+16) = r27:r26; r2 = memw(r16+4) }
	{ r7 = memw(r16+8) }
	{ memd(r29+48) = r19:r18; memd(r29+32) = r23:r22; r18 = 437F0000 }
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
	{ r2 = r17; r1 = 00000042; r4 = add(PC,0000B0D1) }
	{ memw(r29) = r16; r19 = r0; call logmsg_function }
	{ if (!cmp.gtu(r25,r2.new)) jump:t 0001EA8C; r2 = memw(r27+20); p0 = cmp.eq(r25,00000000) }

l0001EA74:
	{ r2 = r17; r1 = 00000043; r3 = add(PC,00000019) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001EB54 }

l0001EA8C:
	{ r3 = memw(r26+4); r2 = memw(r26) }
	{ memw(r27) = r2; memw(r27+4) = r3 }
	{ r6 = memw(r26+8) }
	{ memw(r27+8) = r6 }
	{ memw(r29+12) = r17; r7 = memw(r26+12); if (!p0) r17 = 42FF0000 }
	{ memw(r27+24) = r25; memw(r27+12) = r7; if (p0) jump:nt 0001EB10 }

l0001EAC0:
	{ r27 = 3F000000; r26 = 3F800000 }

l0001EAD0:
	{ r2 = memb(r22++#1); r3 = r23 }
	{ r2 = convert_w2sf(r2) }
	{ r3 += sfmpy(r19,r2) }
	{ call fn000097F0; r0 = sfmpy(r3,r27) }
	{ r25 = add(r25,FFFFFFFF); r2 = r27; r3 = sfadd(r0,r26) }
	{ p0 = cmp.eq(r25,00000000); r2 += sfmpy(r3,r17) }
	{ r3 = convert_sf2uw(r2) }
	{ if (p1.new) r3 = FFFFFFFF; p1 = sfcmp.gt(r2,r18) }
	{ memb(r24++#1) = r3; if (!p0) jump:nt 0001EAD0 }

l0001EB10:
	{ memw(r21+12) = 00000001; r2 = memw(r21+16); r4 = add(PC,0000B047) }
	{ memw(r21) = 00000001; memw(r21+8) = 00000001; r1 = 00000057 }
	{ memw(r2) = 00000000; memw(r21+4) = 00000001 }
	{ memw(r21+24) = 00000004; r2 = memw(r29+12) }
	{ memw(r20+8) = 00000001; r3 = memw(r20+16) }
	{ memw(r20+4) = 00000001; memw(r20) = 00000001 }
	{ memw(r3) = 3F800000; memw(r20+12) = FFFFFF81 }
	{ memw(r29) = r16; memw(r20+24) = 00000004; call logmsg_function }
	{ r0 = 00000000 }

l0001EB54:
	{ r19:r18 = memd(r29+48); r17:r16 = memd(r29+56) }
	{ r23:r22 = memd(r29+32); r21:r20 = memd(r29+40) }
	{ r27:r26 = memd(r29+16); r25:r24 = memd(r29+24) }
	{ dealloc_return }
0001EB68                         00 40 00 7F 00 C0 00 7F         .@......

;; qsigmoid_check: 0001EB70
qsigmoid_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000AFF7) }
	{ r17 = r0; r16 = r1; r1 = 00000095 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001EBB0; r2 = memw(r17+16); r1 = 00000096 }

l0001EB9C:
	{ r3 = add(PC,00000028) }
	{ r2 = r16; call errlog_function }

l0001EBA4:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001EBB0:
	{ if (cmp.eq(r2.new,00000006)) jump:t 0001EBC8; r2 = memw(r17+20); r1 = 00000097 }

l0001EBC0:
	{ jump 0001EBA4; r3 = add(PC,00000013) }

l0001EBC8:
	{ r2 = r16; r1 = 00000098; r4 = add(PC,0000AFD7) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }
0001EBE8                         00 40 00 7F 00 C0 00 7F         .@......

;; qsigmoid_execute_hvx: 0001EBF0
qsigmoid_execute_hvx proc
	{ allocframe(00000050); memd(r29+496) = r17:r16; r4 = add(PC,0000AF2D) }
	{ r17:r16 = combine(r1,r0) }
	{ memd(r29+64) = r19:r18; r3 = memw(r16+4); r1 = 00000073 }
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
	{ if (!cmp.eq(r2.new,00000000)) jump:t 0001EC7C; r2 = memb(r16+32); r1 = 00000074 }

l0001EC68:
	{ r3 = add(PC,00000013) }
	{ r2 = r17; call errlog_function }

l0001EC70:
	{ r2 = r17 }
	{ r0 = FFFFFFFF; jump 0001EFD4 }

l0001EC7C:
	{ r3 = memw(r18+20); r1 = 00000075; r2 = mpyi(r22,r27) }
	{ r2 = mpyi(r2,r20) }
	{ if (!cmp.gtu(r20.new,r3)) jump:t 0001ECA0; r20 = mpyi(r2,r26) }

l0001EC98:
	{ jump 0001EC70; r3 = add(PC,00000035) }

l0001ECA0:
	{ memw(r29+12) = r2; r3 = memw(r21); r2 = 000000FF; r22 = add(r20,000000FF) }
	{ memb(r18+1) = r4.new; r4 = memw(r21+4); r0 = r25; r27 = and(r22,FFFFFF00) }
	{ memw(r18) = r3 }
	{ r2 = memw(r21+8); r1 = 00000000; r3 = add(r22,r0) }
	{ memw(r29+16) = r0; memw(r18+8) = r2; r2 = r27 }
	{ r7 = memw(r21+12) }
	{ memw(r18+24) = r20; memw(r18+12) = r7; r21 = and(r3,FFFFFF00); call fn000095F0 }
	{ call fn000095F0 }
	{ r2 = r27 }
	{ r1:r0 = combine(r23,r21); r2 = r20; call fn00009560 }
	{ r2 = C0800000; r1 = sfsub(r24,r19) }
	{ if (!p0.new) jump:nt 0001EDDC; p0 = sfcmp.ge(r19,r2) }

l0001ED10:
	{ r18 = 40800000 }
	{ if (!p0.new) jump:nt 0001EDDC; p0 = sfcmp.ge(r18,r24) }

l0001ED20:
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r2,r1); call fn00009610 }
	{ r23 = r0; r2 = sfadd(r19,r18) }
	{ r1:r0 = combine(r23,r2); call fn00009610 }
	{ r2 = 41700000; r18 = convert_uw2sf(r0):chop }
	{ r0 = r2; call fn000097C0 }
	{ r3 = BF800000; r2 = 41FF0000 }
	{ r25 = memw(r29+20); p0 = cmp.gt(r20,00000000); r3 = sfadd(r0,r3); r2 = sfmpy(r23,r2) }
	{ r3 = convert_uw2sf(r3):chop; r2 = sfmpy(r2,r0) }
	{ if (p0) r2 = memw(r29+12); if (!p0) jump:nt 0001EE98; r4 = convert_uw2sf(r2):chop }

l0001ED88:
	{ r5 = convert_uw2sf(r0):chop }
	{ if (!p0.new) r3 = add(r4,00000000); p0 = cmp.gt(r4,r3); r5 += lsr(r5,0000001F) }
	{ r3 = sxth(r3); r2 = r21; r6 = mpyi(r2,r26) }
	{ r4 = asr(r5,00000001); loop0(0001EDA8,r6) }
	{ r6 = memb(r2); r7 = r4; r5 = 000000FF }
	{ r6 = add(r6,r18) }
	{ r7 += mpyi(r6,r3) }
	{ r6 = asr(r7,0000000F) }
	{ if (!p0.new) r5 = add(r6,00000000); p0 = cmp.gt(r6,000000FF); if (p0.new) jump:nt 0001EDD0 }

l0001EDC8:
	{ if (!p0.new) r5 = 00000000; p0 = cmp.gt(r6,FFFFFFFF) }

l0001EDD0:
	{ memb(r2++#1) = r5; nop }
	{ jump 0001EE98 }

l0001EDDC:
	{ r0 = 38D1B717; call fn00009600 }
	{ r1 = 437F0000; call fn00009610 }
	{ r25 = memw(r29+20); r3 = add(r21,r22); if (!p0.new) jump:nt 0001EE98; p0 = cmp.gt(r12,00000000) }

l0001EE00:
	{ r22 = and(r3,FFFFFF00); loop0(0001EE0C,r20) }
	{ r3:r2 = combine(r22,r21) }
	{ r4 = memb(r2++#1); r5 = r19 }
	{ r4 = convert_w2sf(r4) }
	{ r3 = add(r3,00000004); r5 += sfmpy(r0,r4) }
	{ r1 = 41000000; r0 = 00000017 }
	{ call fn00009600 }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = memd(r29+12); r23 = r0; r18 = r21 }
	{ r24 = 40800000; r19 = mpyi(r2,r26) }

l0001EE5C:
	{ r2 = memw(r22) }
	{ r2 = sfadd(r2,r24) }
	{ call fn00009620; r0 = sfmpy(r23,r2) }
	{ r2 = 000000FF; r3 = convert_uw2sf(r0):chop }
	{ if (!p0.new) r2 = add(r3,00000000); p0 = cmp.gt(r3,000000FF); if (p0.new) jump:nt 0001EE88 }

l0001EE80:
	{ if (!p0.new) r2 = 00000000; p0 = cmp.gt(r3,FFFFFFFF) }

l0001EE88:
	{ memb(r18++#1) = r2; r22 = add(r22,00000004); r19 = add(r19,FFFFFFFF) }
	{ if (!p0.new) jump:nt 0001EE5C; p0 = cmp.eq(r27,00000001) }

l0001EE98:
	{ r3 = 01010101; r2 = add(PC,0000C278) }
	{ r5 = 02020202; r4 = 10101010 }
	{ r7 = 07070707; r6 = F8F8F8F8 }
	{  }
	{  }
	{  }
	{  }
	{ p0 = cmp.gt(r27,00000000) }
	{  }
	{ r4 = memw(r2-32); if (!p0) jump:nt 0001EF74 }

l0001EEEC:
	{ r6 = 00000007; r2 = add(r27,0000007F); r7 = 00000000 }
	{ r5 = lsr(r2,00000007) }
	{ r2 = memw(r29+16); r3 = 00000003; loop0(0001EF18,r5) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }

l0001EF74:
	{ r0 = memd(r29+24); r1 = memd(r29+16); r2 = r20; call fn00009560 }
	{ r5 = memw(r29+28); r2 = memw(r25+16); r4 = add(PC,0000ABD7) }
	{ memw(r25+8) = 00000001; memw(r25+12) = FFFFFF81; r1 = 0000008F }
	{ memw(r25+4) = FFFFFF81; memw(r25) = 00000001 }
	{ memw(r25+24) = 00000004; memw(r2) = 00000000; r2 = r17 }
	{ memw(r5+8) = 00000001; r3 = memw(r5+16) }
	{ memw(r5+4) = 00000001; memw(r5) = 00000001 }
	{ memw(r3) = 3F800000; memw(r5+12) = FFFFFF81 }
	{ memw(r29) = r16; memw(r5+24) = 00000004; call logmsg_function }
	{ r0 = 00000000 }

l0001EFD4:
	{ r19:r18 = memd(r29+64); r17:r16 = memd(r29+72) }
	{ r23:r22 = memd(r29+48); r21:r20 = memd(r29+56) }
	{ r27:r26 = memd(r29+32); r25:r24 = memd(r29+40) }
	{ dealloc_return }

;; logmsg_function: 0001EFE8
;;   Called from:
;;     0001EA5C (in qsigmoid_execute_ref)
;;     0001EB44 (in qsigmoid_execute_ref)
;;     0001EB84 (in qsigmoid_check)
;;     0001EBD8 (in qsigmoid_check)
;;     0001EC50 (in qsigmoid_execute_hvx)
;;     0001EFC4 (in qsigmoid_execute_hvx)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001F00C; r5 = memw(r2+16) }

l0001EFF8:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000000C) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001F00C:
	{ dealloc_return }

;; errlog_function: 0001F010
;;   Called from:
;;     0001EA80 (in qsigmoid_execute_ref)
;;     0001EBA0 (in qsigmoid_check)
;;     0001EC6C (in qsigmoid_execute_hvx)
;;     0001F000 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000AAF0) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001F034             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; add_f_execute: 0001F040
add_f_execute proc
	{ allocframe(000000A0); r5 = r0 }
	{ r3 = memw(r5+8); r2 = memw(r5+4) }
	{ memd(r29+144) = r19:r18; memd(r29+112) = r27:r26 }
	{ r19 = memw(r2+4); r26 = memw(r2) }
	{ memd(r29+128) = r23:r22; memd(r29+136) = r21:r20; r22 = 00000000 }
	{ r0 = memw(r26); r6 = memw(r26+4) }
	{ r4 = memw(r19+4); r8 = memw(r19); p1 = cmp.eq(r0,00000001); p0 = cmp.eq(r6,00000001) }
	{ r12 = memw(r26+8); r10 = p1 }
	{ memw(r29+92) = r4; r9 = memw(r19+8); p2 = cmp.eq(r12,00000001); r18 = mux(p0,r4,r6) }
	{ r7 = memw(r26+12); r4 = mux(p1,r8,r0) }
	{ r0 = memw(r19+12); p1 = cmp.eq(r7,00000001); r2 = mpyi(r4,r18) }
	{ memw(r29+76) = r6; memd(r29+120) = r25:r24; r20 = mux(p2,r9,r12) }
	{ r21 = mux(p1,r0,r7); r2 = mpyi(r2,r20) }
	{ memd(r29+152) = r17:r16; r25 = memw(r3); r6 = r1; r2 = mpyi(r2,r21) }
	{ memw(r29+72) = r8; memw(r29+96) = r10; r10 = p2 }
	{ memw(r29+84) = r10; memw(r29+68) = r9 }
	{ memw(r29+80) = r0; memw(r29+104) = r4; r16 = asl(r2,00000002) }
	{ if (p0) jump:nt 0001F0EC }

l0001F0E8:
	{ r22 = mpyi(r7,r12) }

l0001F0EC:
	{ r2 = r6; r1 = 000000BC; r0 = add(PC,0000AB22) }
	{ memw(r29+64) = r12; r3 = memw(r19+16); r4 = add(PC,0000AB31) }
	{ memw(r29+88) = r7; r23 = memw(r25+16); r27 = r5; r24 = r6 }
	{ r17 = memw(r26+16) }
	{ memw(r29) = r5; memw(r29+100) = r3; call logmsg_function }
	{ if (!cmp.gtu(r16,r2.new)) jump:t 0001F154; r2 = memw(r25+20); r1 = 000000BC }

l0001F138:
	{ r2 = r24; r0 = add(PC,0000001A) }
	{ r3 = add(PC,0000AB0B) }

l0001F148:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001F380 }

l0001F154:
	{ r5 = memw(r26); r13 = memw(r19); r2 = r24 }
	{ r7 = memw(r26+8); r8 = memw(r26+12); p0 = cmp.eq(r5,r13) }
	{ r12 = memw(r19+12); r6 = memw(r26+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+52) = r16; memw(r29+48) = r25 }
	{ memw(r29+60) = r27; memw(r29+56) = r17 }
	{ if (p0) jump:nt 0001F19C }

l0001F190:
	{ if (p0.new) jump:nt 0001F19C; p0 = cmp.eq(r5,00000002) }

l0001F194:
	{ p0 = cmp.eq(r13,00000001); if (!p0.new) jump:nt 0001F1D8 }

l0001F19C:
	{ if (p0.new) jump:nt 0001F1AC; p0 = cmp.eq(r6,-00000001) }

l0001F1A0:
	{ if (p0.new) jump:nt 0001F1AC; p0 = cmp.eq(r6,00000002) }

l0001F1A4:
	{ nop; if (!p0.new) jump:nt 0001F1D8; p0 = cmp.eq(r3,00000002) }

l0001F1AC:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 0001F1C0 }

l0001F1B4:
	{ if (p0.new) jump:nt 0001F1C0; p0 = cmp.eq(r7,00000002) }

l0001F1B8:
	{ p0 = cmp.eq(r9,00000001); if (!p0.new) jump:nt 0001F1D8 }

l0001F1C0:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 0001F20C }

l0001F1C8:
	{ p0 = cmp.eq(r8,00000001); if (p0.new) jump:nt 0001F20C }

l0001F1D0:
	{ p0 = cmp.eq(r12,00000001); if (p0.new) jump:nt 0001F20C }

l0001F1D8:
	{ memw(r29+28) = r12; r1 = 000000BC; r0 = add(PC,0000AA36) }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+20) = r3; r3 = add(PC,0000AA69) }
	{ memw(r29+4) = r6; memw(r29+12) = r8 }
	{ memw(r29) = r5; jump 0001F148 }

l0001F20C:
	{ memw(r29+44) = r21; r24 = memw(r29+104); r19 = r2 }
	{ memw(r29+20) = r3; memw(r29+36) = r18; r0 = add(PC,0000A9F6) }
	{ memw(r29+40) = r20; r1 = 000000BC; r4 = add(PC,0000AA65) }
	{ memw(r29+28) = r12; memw(r29+32) = r24 }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; memw(r29+4) = r6 }
	{ call logmsg_function }
	{ r26 = memw(r29+100); r3 = memw(r29+48); if (p0.new) r14 = 00000000; p0 = cmp.gt(r24,00000000) }
	{ r25 = memw(r29+56) }
	{ memb(r3+6) = r2.new; r2 = memw(r29+52) }
	{ memw(r3+4) = r18 }
	{ memw(r3+12) = r21; memw(r3+8) = r20 }
	{ if (p0) r12 = memw(r29+88); if (p0) r13 = memw(r29+80); if (!p0) jump:nt 0001F358 }

l0001F288:
	{ r7 = memd(r29+92); r5 = memd(r29+68); r6 = !cmp.eq(r13,00000001) }
	{ r2 = memd(r29+76); r0 = memd(r29+84); r9 = mpyi(r5,r7) }
	{ r4 = memd(r29+72); r3 = memd(r29+64); p0 = cmp.eq(r5,00000001); p2 = r0 }
	{ r0 = memw(r29+96); p1 = cmp.eq(r4,00000001); r9 = mpyi(r9,r13); r8 = mpyi(r3,r2) }
	{ if (p1) r9 = add(r14,00000000); p2 = cmp.eq(r7,00000001); r2 = cmp.eq(r12,00000000); r4 = mpyi(r13,r5) }
	{ p0 = r0; r8 = mpyi(r8,r12) }
	{ r5 = !cmp.eq(r12,00000001) }
	{ if (p0) r8 = add(r14,00000000); if (p2) r4 = add(r14,00000000); r7 = 00000000 }

l0001F2E0:
	{ r14 = 00000000; r13:r12 = combine(r25,r26); if (!p0.new) jump:nt 0001F348; p0 = cmp.gt(r10,00000000) }

l0001F2E4:
	{ r14 = 00000000; r13:r12 = combine(r25,r26) }

l0001F2EC:
	{ r15 = r13; r28 = r12; if (!p0.new) jump:nt 0001F338; p0 = cmp.gt(r12,00000000) }

l0001F2F8:
	{ loop1(0001F2FC,r20) }
	{ if (!p0) r1:r0 = combine(r15,r28); r10 = r23; if (!p0.new) jump:nt 0001F32C; p0 = cmp.gt(r13,00000000) }

l0001F308:
	{ loop0(0001F30C,r21) }
	{ r16 = memw(r0); r11 = memw(r1); r1 = addasl(r1,r5,00000002); r0 = addasl(r0,r6,00000002) }
	{ r11 = sfadd(r11,r16) }
	{ memw(r10) = r11; r10 = add(r10,00000004) }
	{ r23 = addasl(r23,r21,00000002) }

l0001F32C:
	{ nop; r15 = addasl(r15,r2,00000002); r28 = addasl(r28,r3,00000002) }

l0001F338:
	{ if (!cmp.eq(r14.new,r18)) jump:t 0001F2EC; r14 = add(r14,00000001); r12 = addasl(r12,r4,00000002); r13 = addasl(r13,r22,00000002) }

l0001F348:
	{ if (!cmp.eq(r7.new,r24)) jump:t 0001F2E0; r7 = add(r7,00000001); r26 = addasl(r26,r9,00000002); r25 = addasl(r25,r8,00000002) }

l0001F34C:
	{ if (!cmp.eq(r7.new,r24)) jump:t 0001F2E4; r7 = add(r7,00000001); r26 = addasl(r26,r9,00000002) }

l0001F358:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,0000A8B6) }

l0001F35C:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,00000036) }

l0001F36C:
	{ r2 = r19; r1 = 000000BC; r4 = add(PC,00000011) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0001F380:
	{ r19:r18 = memd(r29+144); r17:r16 = memd(r29+152) }
	{ r23:r22 = memd(r29+128); r21:r20 = memd(r29+136) }
	{ r27:r26 = memd(r29+112); r25:r24 = memd(r29+120) }
	{ dealloc_return }

;; add_f_check: 0001F394
add_f_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000A83F) }
	{ r16 = r1; r1 = 00000037; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,0000A810) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,00000004)) jump:t 0001F3E4; r2 = memw(r17+16); r1 = 00000038 }

l0001F3C8:
	{ r0 = add(PC,00000034) }
	{ r3 = add(PC,0000A813) }

l0001F3D4:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001F3E4:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001F404; r2 = memw(r17+20); r1 = 00000039 }

l0001F3F4:
	{ r0 = add(PC,00000008) }
	{ jump 0001F3D4; r3 = add(PC,0000A7F6) }

l0001F404:
	{ r2 = r16; r1 = 0000003A; r0 = add(PC,0000A7B4) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,0000A7EE) }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001F428
;;   Called from:
;;     0001F120 (in add_f_execute)
;;     0001F250 (in add_f_execute)
;;     0001F378 (in add_f_execute)
;;     0001F3B4 (in add_f_check)
;;     0001F410 (in add_f_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001F448; r5 = memw(r2+16) }

l0001F438:
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010) }

l0001F448:
	{ dealloc_return }

;; errlog_function: 0001F44C
;;   Called from:
;;     0001F148 (in add_f_execute)
;;     0001F3D4 (in add_f_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r3 = 00000000 }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; mul_f_execute: 0001F470
;;   Called from:
;;     0001F46C (in errlog_function)
mul_f_execute proc
	{ allocframe(000000A8); memd(r29+472) = r23:r22; r4 = add(PC,0000A8BA) }
	{  }
	{ memd(r29+120) = r27:r26; memd(r29+128) = r25:r24 }
	{ memd(r29+160) = r17:r16; r16 = r0; r2 = r23; r1 = 00000032 }
	{ memw(r29) = r16; r0 = add(PC,0000A840) }
	{ memd(r29+144) = r21:r20; memd(r29+152) = r19:r18 }
	{ call logmsg_function }
	{ r5 = r16 }
	{ r3 = memw(r5+8); r2 = memw(r5+4) }
	{ r16 = memw(r2) }
	{ r3 = memw(r3); r19 = memw(r2+4) }
	{ r1 = memw(r16); r7 = memw(r16+4) }
	{ r4 = memw(r19+4); r8 = memw(r19); p1 = cmp.eq(r1,00000001); p0 = cmp.eq(r7,00000001) }
	{ r12 = memw(r16+8) }
	{ memw(r29+96) = r4; r9 = memw(r19+8); p2 = cmp.eq(r12,00000001); r18 = mux(p0,r4,r7) }
	{ r6 = memw(r16+12); r4 = mux(p1,r8,r1); r0 = p1 }
	{ r1 = memw(r19+12); p1 = cmp.eq(r6,00000001); r2 = mpyi(r4,r18) }
	{ memw(r29+112) = r4; memw(r29+100) = r0; r20 = mux(p2,r9,r12); r0 = p2 }
	{ memw(r29+80) = r7; memw(r29+72) = r9; r21 = mux(p1,r1,r6); r2 = mpyi(r2,r20) }
	{ memw(r29+88) = r0; memw(r29+76) = r8; r2 = mpyi(r2,r21) }
	{ memw(r29+84) = r1; if (p0) jump:nt 0001F530; r17 = asl(r2,00000002) }

l0001F52C:
	{ r22 = mpyi(r6,r12) }

l0001F530:
	{ r2 = r23; r1 = 000000BC; r0 = add(PC,0000A808) }
	{ memw(r29+92) = r6; r27 = r23; r4 = add(PC,0000A817) }
	{ r6 = memw(r19+16) }
	{ memw(r29+68) = r12; r23 = memw(r3+16) }
	{ r26 = memw(r16+16); r25:r24 = combine(r3,r5) }
	{ memw(r29) = r5; memw(r29+108) = r6; call logmsg_function }
	{ if (!cmp.gtu(r17,r2.new)) jump:t 0001F598; r2 = memw(r25+20); r1 = 000000BC }

l0001F57C:
	{ r2 = r27; r0 = add(PC,00000000) }
	{ r3 = add(PC,0000A7F1) }

l0001F58C:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001F7AC }

l0001F598:
	{ r5 = memw(r16); r2 = memw(r19) }
	{ r7 = memw(r16+8); r8 = memw(r16+12); p0 = cmp.eq(r5,r2) }
	{ r12 = memw(r19+12); r6 = memw(r16+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+56) = r17; memw(r29+52) = r25 }
	{ memw(r29+64) = r24; memw(r29+60) = r26 }
	{ memw(r29+104) = r27; if (p0) jump:nt 0001F5D8 }

l0001F5D0:
	{ if (p0.new) jump:nt 0001F5D8; p0 = cmp.eq(r5,00000002) }

l0001F5D4:
	{ if (!p0.new) jump:nt 0001F614; p0 = cmp.eq(r2,00000002) }

l0001F5D8:
	{ nop; if (p0.new) jump:nt 0001F5E8; p0 = cmp.eq(r6,-00000001) }

l0001F5E0:
	{ if (p0.new) jump:nt 0001F5E8; p0 = cmp.eq(r6,00000002) }

l0001F5E4:
	{ if (!p0.new) jump:nt 0001F614; p0 = cmp.eq(r3,00000002) }

l0001F5E8:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 0001F5FC }

l0001F5F0:
	{ if (p0.new) jump:nt 0001F5FC; p0 = cmp.eq(r7,00000002) }

l0001F5F4:
	{ p0 = cmp.eq(r9,00000001); if (!p0.new) jump:nt 0001F614 }

l0001F5FC:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 0001F648 }

l0001F604:
	{ p0 = cmp.eq(r8,00000001); if (p0.new) jump:nt 0001F648 }

l0001F60C:
	{ p0 = cmp.eq(r12,00000001); if (p0.new) jump:nt 0001F648 }

l0001F614:
	{ memw(r29+28) = r12; r1 = 000000BC; r0 = add(PC,0000A724) }
	{ memw(r29+16) = r2; memw(r29+24) = r9 }
	{ memw(r29+4) = r6; memw(r29+20) = r3; r3 = add(PC,0000A757) }
	{ memw(r29+8) = r7; r2 = memd(r29+104) }
	{ memw(r29) = r5; memw(r29+12) = r8 }
	{ jump 0001F58C }

l0001F648:
	{ memw(r29+44) = r21; r19 = memd(r29+112); r0 = add(PC,0000A6F0) }
	{ memw(r29+32) = r19; r1 = 000000BC; r4 = add(PC,0000A75F) }
	{ memw(r29+36) = r18; memw(r29+40) = r20 }
	{ memw(r29+24) = r9; memw(r29+28) = r12 }
	{ memw(r29+16) = r2; memw(r29+20) = r3 }
	{ memw(r29+4) = r6; r2 = memd(r29+104) }
	{ memw(r29+8) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; call logmsg_function }
	{ r17 = memd(r29+108); r3 = memd(r29+52); if (p0.new) r14 = 00000000; p0 = cmp.gt(r19,00000000) }
	{ r24 = memw(r29+60) }
	{ memb(r3+6) = r2.new; r2 = memw(r29+56) }
	{ memw(r3+12) = r21; memw(r3+8) = r20 }
	{ if (p0) r12 = memw(r29+92); if (p0) r13 = memw(r29+84); if (!p0) jump:nt 0001F784 }

l0001F6B4:
	{ r7 = memd(r29+96); r5 = memd(r29+72); r6 = !cmp.eq(r13,00000001) }
	{ r2 = memd(r29+80); r0 = memd(r29+88); r9 = mpyi(r5,r7) }
	{ r4 = memd(r29+76); r3 = memd(r29+68); p0 = cmp.eq(r5,00000001); p2 = r0 }
	{ r0 = memw(r29+100); p1 = cmp.eq(r4,00000001); r9 = mpyi(r9,r13); r8 = mpyi(r3,r2) }
	{ if (p1) r9 = add(r14,00000000); p2 = cmp.eq(r7,00000001); r2 = cmp.eq(r12,00000000); r4 = mpyi(r13,r5) }
	{ p0 = r0; r8 = mpyi(r8,r12) }
	{ r5 = !cmp.eq(r12,00000001) }
	{ if (p0) r8 = add(r14,00000000); if (p2) r4 = add(r14,00000000); r7 = 00000000 }

l0001F70C:
	{ r14 = 00000000; r13:r12 = combine(r24,r17); if (!p0.new) jump:nt 0001F774; p0 = cmp.gt(r10,00000000) }

l0001F710:
	{ r14 = 00000000; r13:r12 = combine(r24,r17) }

l0001F718:
	{ r15 = r13; r28 = r12; if (!p0.new) jump:nt 0001F764; p0 = cmp.gt(r12,00000000) }

l0001F724:
	{ loop1(0001F728,r20) }
	{ if (!p0) r1:r0 = combine(r15,r28); r10 = r23; if (!p0.new) jump:nt 0001F758; p0 = cmp.gt(r13,00000000) }

l0001F734:
	{ loop0(0001F738,r21) }
	{ r16 = memw(r0); r11 = memw(r1); r1 = addasl(r1,r5,00000002); r0 = addasl(r0,r6,00000002) }
	{ r11 = sfmpy(r11,r16) }
	{ memw(r10) = r11; r10 = add(r10,00000004) }
	{ r23 = addasl(r23,r21,00000002) }

l0001F758:
	{ nop; r15 = addasl(r15,r2,00000002); r28 = addasl(r28,r3,00000002) }

l0001F764:
	{ if (!cmp.eq(r14.new,r18)) jump:t 0001F718; r14 = add(r14,00000001); r12 = addasl(r12,r4,00000002); r13 = addasl(r13,r22,00000002) }

l0001F774:
	{ if (!cmp.eq(r7.new,r19)) jump:t 0001F70C; r7 = add(r7,00000001); r17 = addasl(r17,r9,00000002); r24 = addasl(r24,r8,00000002) }

l0001F778:
	{ if (!cmp.eq(r7.new,r19)) jump:t 0001F710; r7 = add(r7,00000001); r17 = addasl(r17,r9,00000002) }

l0001F784:
	{ memb(r29) = r2.new; r2 = memw(r29+64); r0 = add(PC,0000A5B4) }

l0001F788:
	{ memb(r29) = r2.new; r2 = memw(r29+64); r0 = add(PC,00000034) }

l0001F798:
	{ r2 = memw(r29+104); r1 = 000000BC; r4 = add(PC,0000000F) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0001F7AC:
	{ r19:r18 = memd(r29+152); r17:r16 = memd(r29+160) }
	{ r23:r22 = memd(r29+136); r21:r20 = memd(r29+144) }
	{ r27:r26 = memd(r29+120); r25:r24 = memd(r29+128) }
	{ dealloc_return }

;; mul_f_check: 0001F7C0
mul_f_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000A52F) }
	{ r16 = r1; r1 = 00000038; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,0000A500) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,00000004)) jump:t 0001F810; r2 = memw(r17+16); r1 = 00000039 }

l0001F7F4:
	{ r0 = add(PC,00000024) }
	{ r3 = add(PC,0000A503) }

l0001F800:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001F810:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001F830; r2 = memw(r17+20); r1 = 0000003A }

l0001F820:
	{ r0 = add(PC,00000038) }
	{ jump 0001F800; r3 = add(PC,0000A4E6) }

l0001F830:
	{ r2 = r16; r1 = 0000003B; r0 = add(PC,0000A4A4) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,0000A4DE) }
	{ r0 = 00000000 }
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
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001F874; r5 = memw(r2+16) }

l0001F864:
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010) }

l0001F874:
	{ dealloc_return }

;; errlog_function: 0001F878
;;   Called from:
;;     0001F58C (in mul_f_execute)
;;     0001F800 (in mul_f_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r3 = 00000000 }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; neg_f_execute: 0001F8A0
;;   Called from:
;;     0001F89C (in errlog_function)
neg_f_execute proc
	{ allocframe(+00000000) }
	{ r5 = memw(r0+8); r7 = memw(r0+4) }
	{ r3 = memw(r7) }
	{ r7 = memw(r3+12); r2 = memw(r3+4) }
	{ r6 = memw(r3+8); r4 = memw(r3) }
	{ r4 = memw(r5); r2 = mpyi(r2,r4) }
	{ r6 = memw(r4+20); r2 = mpyi(r2,r6) }
	{ r2 = r1; r7 = mpyi(r2,r7) }
	{ if (cmp.gtu(r5.new,r6)) jump:t 0001F910; r5 = asl(r7,00000002) }

l0001F8D8:
	{ r6 = memw(r3+16); r2 = memw(r4+16); loop0(0001F8E0,r7) }
	{ r7 = memw(r6); r6 = add(r6,00000004) }
	{ r7 = togglebit(r7,0000001E) }
	{ memw(r2) = r7; r2 = add(r2,00000004); nop }
	{ r6 = memw(r3+4); r2 = memw(r3); r0 = 00000000 }
	{ memw(r4) = r2; memw(r4+4) = r6 }
	{ r1 = memw(r3+8) }
	{ memw(r4+8) = r1 }
	{ r7 = memw(r3+12) }
	{ memw(r4+24) = r5; memw(r4+12) = r7 }
	{ dealloc_return }

l0001F910:
	{ r1 = 00000036; call errlog_function; r3 = add(PC,0000A544) }
	{ dealloc_return; r0 = -00000001 }

;; neg_f_check: 0001F924
neg_f_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000A4F5) }
	{ r17 = r0; r16 = r1; r1 = 00000041 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001F964; r2 = memw(r17+16); r1 = 00000042 }

l0001F950:
	{ r3 = add(PC,00000019) }
	{ r2 = r16; call errlog_function }

l0001F958:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001F964:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001F97C; r2 = memw(r17+20); r1 = 00000043 }

l0001F974:
	{ jump 0001F958; r3 = add(PC,00000004) }

l0001F97C:
	{ r2 = r16; r1 = 00000044; r4 = add(PC,0000A4C8) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001F99C
;;   Called from:
;;     0001F938 (in neg_f_check)
;;     0001F98C (in neg_f_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001F9C0; r5 = memw(r2+16) }

l0001F9AC:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000016) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001F9C0:
	{ dealloc_return }

;; errlog_function: 0001F9C4
;;   Called from:
;;     0001F910 (in neg_f_execute)
;;     0001F954 (in neg_f_check)
;;     0001F9B4 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000A43A) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001F9E8                         00 00 00 00 00 00 00 00         ........

;; sub_f_execute: 0001F9F0
sub_f_execute proc
	{ allocframe(000000A0); r5 = r0 }
	{ r3 = memw(r5+8); r2 = memw(r5+4) }
	{ memd(r29+144) = r19:r18; memd(r29+112) = r27:r26 }
	{ r19 = memw(r2+4); r26 = memw(r2) }
	{ memd(r29+128) = r23:r22; memd(r29+136) = r21:r20; r22 = 00000000 }
	{ r0 = memw(r26); r6 = memw(r26+4) }
	{ r4 = memw(r19+4); r8 = memw(r19); p1 = cmp.eq(r0,00000001); p0 = cmp.eq(r6,00000001) }
	{ r12 = memw(r26+8); r10 = p1 }
	{ memw(r29+92) = r4; r9 = memw(r19+8); p2 = cmp.eq(r12,00000001); r18 = mux(p0,r4,r6) }
	{ r7 = memw(r26+12); r4 = mux(p1,r8,r0) }
	{ r0 = memw(r19+12); p1 = cmp.eq(r7,00000001); r2 = mpyi(r4,r18) }
	{ memw(r29+76) = r6; memd(r29+120) = r25:r24; r20 = mux(p2,r9,r12) }
	{ r21 = mux(p1,r0,r7); r2 = mpyi(r2,r20) }
	{ memd(r29+152) = r17:r16; r25 = memw(r3); r6 = r1; r2 = mpyi(r2,r21) }
	{ memw(r29+72) = r8; memw(r29+96) = r10; r10 = p2 }
	{ memw(r29+84) = r10; memw(r29+68) = r9 }
	{ memw(r29+80) = r0; memw(r29+104) = r4; r16 = asl(r2,00000002) }
	{ if (p0) jump:nt 0001FA9C }

l0001FA98:
	{ r22 = mpyi(r7,r12) }

l0001FA9C:
	{ r2 = r6; r1 = 000000BC; r0 = add(PC,0000A41C) }
	{ memw(r29+64) = r12; r3 = memw(r19+16); r4 = add(PC,0000A42B) }
	{ memw(r29+88) = r7; r23 = memw(r25+16); r27 = r5; r24 = r6 }
	{ r17 = memw(r26+16) }
	{ memw(r29) = r5; memw(r29+100) = r3; call logmsg_function }
	{ if (!cmp.gtu(r16,r2.new)) jump:t 0001FB04; r2 = memw(r25+20); r1 = 000000BC }

l0001FAE8:
	{ r2 = r24; r0 = add(PC,00000014) }
	{ r3 = add(PC,0000A405) }

l0001FAF8:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001FD30 }

l0001FB04:
	{ r5 = memw(r26); r13 = memw(r19); r2 = r24 }
	{ r7 = memw(r26+8); r8 = memw(r26+12); p0 = cmp.eq(r5,r13) }
	{ r12 = memw(r19+12); r6 = memw(r26+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+52) = r16; memw(r29+48) = r25 }
	{ memw(r29+60) = r27; memw(r29+56) = r17 }
	{ if (p0) jump:nt 0001FB4C }

l0001FB40:
	{ if (p0.new) jump:nt 0001FB4C; p0 = cmp.eq(r5,00000002) }

l0001FB44:
	{ p0 = cmp.eq(r13,00000001); if (!p0.new) jump:nt 0001FB88 }

l0001FB4C:
	{ if (p0.new) jump:nt 0001FB5C; p0 = cmp.eq(r6,-00000001) }

l0001FB50:
	{ if (p0.new) jump:nt 0001FB5C; p0 = cmp.eq(r6,00000002) }

l0001FB54:
	{ nop; if (!p0.new) jump:nt 0001FB88; p0 = cmp.eq(r3,00000002) }

l0001FB5C:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 0001FB70 }

l0001FB64:
	{ if (p0.new) jump:nt 0001FB70; p0 = cmp.eq(r7,00000002) }

l0001FB68:
	{ p0 = cmp.eq(r9,00000001); if (!p0.new) jump:nt 0001FB88 }

l0001FB70:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 0001FBBC }

l0001FB78:
	{ p0 = cmp.eq(r8,00000001); if (p0.new) jump:nt 0001FBBC }

l0001FB80:
	{ p0 = cmp.eq(r12,00000001); if (p0.new) jump:nt 0001FBBC }

l0001FB88:
	{ memw(r29+28) = r12; r1 = 000000BC; r0 = add(PC,0000A330) }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+20) = r3; r3 = add(PC,0000A363) }
	{ memw(r29+4) = r6; memw(r29+12) = r8 }
	{ memw(r29) = r5; jump 0001FAF8 }

l0001FBBC:
	{ memw(r29+44) = r21; r24 = memw(r29+104); r19 = r2 }
	{ memw(r29+20) = r3; memw(r29+36) = r18; r0 = add(PC,0000A2F0) }
	{ memw(r29+40) = r20; r1 = 000000BC; r4 = add(PC,0000A35F) }
	{ memw(r29+28) = r12; memw(r29+32) = r24 }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; memw(r29+4) = r6 }
	{ call logmsg_function }
	{ r26 = memw(r29+100); r3 = memw(r29+48); if (p0.new) r14 = 00000000; p0 = cmp.gt(r24,00000000) }
	{ r25 = memw(r29+56) }
	{ memb(r3+6) = r2.new; r2 = memw(r29+52) }
	{ memw(r3+4) = r18 }
	{ memw(r3+12) = r21; memw(r3+8) = r20 }
	{ if (p0) r12 = memw(r29+88); if (p0) r13 = memw(r29+80); if (!p0) jump:nt 0001FD08 }

l0001FC38:
	{ r7 = memd(r29+92); r5 = memd(r29+68); r6 = !cmp.eq(r13,00000001) }
	{ r2 = memd(r29+76); r0 = memd(r29+84); r9 = mpyi(r5,r7) }
	{ r4 = memd(r29+72); r3 = memd(r29+64); p0 = cmp.eq(r5,00000001); p2 = r0 }
	{ r0 = memw(r29+96); p1 = cmp.eq(r4,00000001); r9 = mpyi(r9,r13); r8 = mpyi(r3,r2) }
	{ if (p1) r9 = add(r14,00000000); p2 = cmp.eq(r7,00000001); r2 = cmp.eq(r12,00000000); r4 = mpyi(r13,r5) }
	{ p0 = r0; r8 = mpyi(r8,r12) }
	{ r5 = !cmp.eq(r12,00000001) }
	{ if (p0) r8 = add(r14,00000000); if (p2) r4 = add(r14,00000000); r7 = 00000000 }

l0001FC90:
	{ r14 = 00000000; r13:r12 = combine(r25,r26); if (!p0.new) jump:nt 0001FCF8; p0 = cmp.gt(r10,00000000) }

l0001FC94:
	{ r14 = 00000000; r13:r12 = combine(r25,r26) }

l0001FC9C:
	{ r15 = r13; r28 = r12; if (!p0.new) jump:nt 0001FCE8; p0 = cmp.gt(r12,00000000) }

l0001FCA8:
	{ loop1(0001FCAC,r20) }
	{ if (!p0) r1:r0 = combine(r15,r28); r10 = r23; if (!p0.new) jump:nt 0001FCDC; p0 = cmp.gt(r13,00000000) }

l0001FCB8:
	{ loop0(0001FCBC,r21) }
	{ r16 = memw(r0); r11 = memw(r1); r1 = addasl(r1,r5,00000002); r0 = addasl(r0,r6,00000002) }
	{ r11 = sfsub(r11,r16) }
	{ memw(r10) = r11; r10 = add(r10,00000004) }
	{ r23 = addasl(r23,r21,00000002) }

l0001FCDC:
	{ nop; r15 = addasl(r15,r2,00000002); r28 = addasl(r28,r3,00000002) }

l0001FCE8:
	{ if (!cmp.eq(r14.new,r18)) jump:t 0001FC9C; r14 = add(r14,00000001); r12 = addasl(r12,r4,00000002); r13 = addasl(r13,r22,00000002) }

l0001FCF8:
	{ if (!cmp.eq(r7.new,r24)) jump:t 0001FC90; r7 = add(r7,00000001); r26 = addasl(r26,r9,00000002); r25 = addasl(r25,r8,00000002) }

l0001FCFC:
	{ if (!cmp.eq(r7.new,r24)) jump:t 0001FC94; r7 = add(r7,00000001); r26 = addasl(r26,r9,00000002) }

l0001FD08:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,0000A1B0) }

l0001FD0C:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,00000030) }

l0001FD1C:
	{ r2 = r19; r1 = 000000BC; r4 = add(PC,0000000B) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l0001FD30:
	{ r19:r18 = memd(r29+144); r17:r16 = memd(r29+152) }
	{ r23:r22 = memd(r29+128); r21:r20 = memd(r29+136) }
	{ r27:r26 = memd(r29+112); r25:r24 = memd(r29+120) }
	{ dealloc_return }

;; sub_f_check: 0001FD44
sub_f_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000A139) }
	{ r16 = r1; r1 = 00000037; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,0000A10A) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,00000004)) jump:t 0001FD94; r2 = memw(r17+16); r1 = 00000038 }

l0001FD78:
	{ r0 = add(PC,0000002E) }
	{ r3 = add(PC,0000A10D) }

l0001FD84:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001FD94:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001FDB4; r2 = memw(r17+20); r1 = 00000039 }

l0001FDA4:
	{ r0 = add(PC,00000002) }
	{ jump 0001FD84; r3 = add(PC,0000A0F0) }

l0001FDB4:
	{ r2 = r16; r1 = 0000003A; r0 = add(PC,0000A0AE) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,0000A0E8) }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001FDD8
;;   Called from:
;;     0001FAD0 (in sub_f_execute)
;;     0001FC00 (in sub_f_execute)
;;     0001FD28 (in sub_f_execute)
;;     0001FD64 (in sub_f_check)
;;     0001FDC0 (in sub_f_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001FDF8; r5 = memw(r2+16) }

l0001FDE8:
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010) }

l0001FDF8:
	{ dealloc_return }

;; errlog_function: 0001FDFC
;;   Called from:
;;     0001FAF8 (in sub_f_execute)
;;     0001FD84 (in sub_f_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r3 = 00000000 }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; rank_execute: 0001FE20
;;   Called from:
;;     0001FE1C (in errlog_function)
rank_execute proc
	{ allocframe(00000018); memd(r29+496) = r17:r16; r4 = add(PC,0000A1C4) }
	{ r1 = 00000032; r17:r16 = combine(r1,r0) }
	{ memd(r29+8) = r19:r18; r2 = memw(r16+4) }
	{ r7 = memw(r16+8) }
	{ r5 = memw(r2+4); r2 = r17 }
	{ r3 = memw(r5+16); r19 = memw(r7) }
	{ memw(r29) = r16; r18 = memw(r3); call logmsg_function }
	{ if (cmp.gtu(r2.new,00000006)) jump:t 0001FE6C; r2 = memw(r19+20) }

l0001FE58:
	{ r2 = r17; r1 = 00000035; r3 = add(PC,00000027) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 0001FE9C }

l0001FE6C:
	{ r3 = memw(r19+16); r1 = 0000003B; r4 = add(PC,0000A19D) }
	{ memw(r19) = 00000001; memw(r19+12) = 00000001; r2 = r17 }
	{ memw(r19+24) = 00000004; memw(r19+8) = 00000001 }
	{ memw(r3) = r18; memw(r19+4) = FFFFFF81 }
	{ memw(r29) = r16; call logmsg_function }
	{ r0 = 00000000 }

l0001FE9C:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29+16) }
	{ dealloc_return }

;; rank_check: 0001FEA4
rank_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,0000A0F4) }
	{ r17 = r0; r16 = r1; r1 = 00000041 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000004)) jump:t 0001FEE4; r2 = memw(r17+16); r1 = 00000042 }

l0001FED0:
	{ r3 = add(PC,00000022) }
	{ r2 = r16; call errlog_function }

l0001FED8:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0001FEE4:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0001FEFC; r2 = memw(r17+20); r1 = 00000043 }

l0001FEF4:
	{ jump 0001FED8; r3 = add(PC,0000000D) }

l0001FEFC:
	{ r2 = r16; r1 = 00000044; r4 = add(PC,0000A0D1) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 0001FF1C
;;   Called from:
;;     0001FE44 (in rank_execute)
;;     0001FE90 (in rank_execute)
;;     0001FEB8 (in rank_check)
;;     0001FF0C (in rank_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0001FF40; r5 = memw(r2+16) }

l0001FF2C:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000016) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0001FF40:
	{ dealloc_return }

;; errlog_function: 0001FF44
;;   Called from:
;;     0001FE60 (in rank_execute)
;;     0001FED4 (in rank_check)
;;     0001FF34 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,0000A03A) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0001FF68                         00 00 00 00 00 00 00 00         ........

;; range_execute: 0001FF70
range_execute proc
	{ allocframe(00000028); memd(r29+496) = r17:r16; r4 = add(PC,0000A10E) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+8) = r23:r22; r2 = memw(r17+4); r1 = 00000038 }
	{ r3 = memw(r17+8) }
	{ memd(r29+16) = r21:r20; memd(r29+24) = r19:r18 }
	{ r0 = memw(r2+4); r5 = memw(r2+8) }
	{ r22 = memw(r3); r2 = memw(r2) }
	{ r5 = memw(r5+16); r7 = memw(r0+16) }
	{ r18 = memw(r22+16); r6 = memw(r2+16); r2 = r16 }
	{ r21 = memw(r5); r19 = memw(r7) }
	{ memw(r29) = r17; r20 = memw(r6); call logmsg_function }
	{ if (!p0.new) r3 = add(r20,00000000); if (!p0.new) r2 = 00000000; if (!p0.new) jump:nt 0001FFD0; p0 = cmp.gt(r11,-00000001) }

l0001FFBC:
	{  }

l0001FFC0:
	{ if (cmp.gtu(r19,r3.new)) jump:t 0001FFC0; r3 = add(r3,r21); r2 = add(r2,00000001) }

l0001FFD0:
	{ if (!p1.new) jump:nt 0001FFE0; p1 = cmp.gt(r12,-00000001) }

l0001FFD4:
	{ if (cmp.gt(r19,r3.new)) jump:t 0001FFD4; r3 = add(r3,r21); r2 = add(r2,00000001) }

l0001FFE0:
	{ r4 = memw(r22+20) }

l0001FFE4:
	{ if (!cmp.gtu(r3.new,r4)) jump:t 00020004; r3 = asl(r2,00000002) }

l0001FFF0:
	{ r2 = r16; r1 = 0000003F; r3 = add(PC,0000002A) }
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00020050 }

l00020004:
	{ memw(r22+4) = 00000001; memw(r22) = 00000001 }
	{ memw(r22+12) = r2; memw(r22+8) = 00000001 }
	{ memw(r22+24) = r3; if (!p0) jump:nt 00020030 }

l00020018:
	{ memw(r18++#4) = r20 }
	{ if (cmp.gtu(r19,r20.new)) jump:t 00020018; r20 = add(r20,r21) }

l00020028:
	{ memw(r18++#4) = r20; r20 = add(r20,r21) }

l00020030:
	{ if (p0.new) jump:nt 00020028; p0 = cmp.gt(r12,-00000001) }

l00020034:
	{ r2 = r16; r1 = 00000049; r4 = add(PC,0000A070) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l00020050:
	{ r19:r18 = memd(r29+24); r17:r16 = memd(r29+32) }
	{ r23:r22 = memd(r29+8); r21:r20 = memd(r29+16) }
	{ dealloc_return }

;; range_check: 0002005C
range_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00009FD5) }
	{ r17 = r0; r16 = r1; r1 = 0000004F }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000006)) jump:t 0002009C; r2 = memw(r17+16); r1 = 00000050 }

l00020088:
	{ r3 = add(PC,00000004) }
	{ r2 = r16; call errlog_function }

l00020090:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l0002009C:
	{ if (cmp.eq(r2.new,00000002)) jump:t 000200B4; r2 = memw(r17+20); r1 = 00000051 }

l000200AC:
	{ jump 00020090; r3 = add(PC,0000002F) }

l000200B4:
	{ r2 = r16; r1 = 00000052; r4 = add(PC,00009FB3) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 000200D4
;;   Called from:
;;     0001FFA8 (in range_execute)
;;     00020044 (in range_execute)
;;     00020070 (in range_check)
;;     000200C4 (in range_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000200F8; r5 = memw(r2+16) }

l000200E4:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000036) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l000200F8:
	{ dealloc_return }

;; errlog_function: 000200FC
;;   Called from:
;;     0001FFF8 (in range_execute)
;;     0002008C (in range_check)
;;     000200EC (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00009F1A) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; transpose_execute: 00020120
transpose_execute proc
	{ allocframe(+00000090); r2 = add(PC,0000A008) }
	{ memd(r29+136) = r17:r16; r5 = add(r29,00000040); r17:r16 = combine(r0,r1); r3 = add(r29,00000030) }
	{ memd(r29+128) = r19:r18; r12 = memw(r17+4); r14 = setbit(r3,00000004); r13 = setbit(r5,00000004) }
	{ memd(r29+120) = r21:r20; r1 = 00000042 }
	{ memd(r29+112) = r23:r22 }
	{ memd(r29+96) = r27:r26; r18 = memw(r12) }
	{ r4 = memw(r17+8) }
	{ memd(r29+104) = r25:r24; r9:r8 = memd(r2) }
	{ r20 = memw(r18+8); r21 = memw(r18+12) }
	{ r22 = memw(r18); r23 = memw(r18+4) }
	{ memw(r29+56) = r20; memw(r29+48) = r22; r26 = mpyi(r21,r20) }
	{  }
	{ memw(r14) = r23; r7:r6 = memd(r2+8) }
	{ memw(r13) = r26; r24 = memw(r4); r4 = add(PC,00009FB4) }
	{ memb(r29+16) = r27.new; r27 = mpyi(r26,r23) }
	{ memd(r29+80) = r9:r8 }
	{ memw(r29+72) = r21; r19 = memw(r24+16) }
	{ memd(r29+88) = r7:r6 }
	{ memw(r5+12) = 00000001; memw(r29+60) = r21 }
	{ memw(r29) = r17; call logmsg_function }
	{ r4 = add(PC,00009F98) }
	{ memw(r29+12) = r21 }
	{ memw(r29) = r22; r1 = 00000047 }
	{ memw(r29+4) = r23; memw(r29+8) = r20 }
	{ call logmsg_function }
	{ if (!cmp.eq(r2.new,00000002)) jump:t 000201F4; r2 = memw(r25) }

l000201E8:
	{ if (!cmp.eq(r2.new,00000002)) jump:t 000201F8 }

l000201F0:
	{ if (cmp.eq(r2.new,00000002)) jump:t 00020214 }

l000201F4:
	{ r1 = 0000004B; r3 = add(PC,00009F7E) }

l000201F8:
	{ r1 = 0000004B; r3 = add(PC,0000003E) }

l00020200:
	{ r2 = r16; call errlog_function }

l00020204:
	{ r2 = r16 }
	{ r0 = FFFFFFFF; jump 00020418 }
00020210 C2 C0 92 91                                     ....            

l00020214:
	{ if (!cmp.gtu(r2,r3.new)) jump:t 0002022C; r3 = memw(r24+20) }

l00020220:
	{ r1 = 0000004C; jump 00020204; r3 = add(PC,0000001F) }

l0002022C:
	{ r3 = memw(r25+24) }
	{ if (cmp.gtu(r4.new,r3)) jump:t 00020248; r4 = 00000014 }

l0002023C:
	{ r1 = 0000004E; jump 00020204; r3 = add(PC,00000011) }

l00020248:
	{ if (!cmp.eq(r5.new,00000002)) jump:t 00020298; r1 = 00000052; r5 = lsr(r3,00000002) }

l00020258:
	{ r4 = add(PC,0000000B) }
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
	{ if (!p0.new) jump:nt 000202B0; p0 = cmp.eq(r5,00000000) }

l000202A0:
	{ if (!p0.new) r4 = memw(r25+16) }
	{ r7:r6 = combine(00000001,00000002); r5 = 00000000; r8 = 00000003 }
	{ jump 00020320 }

l000202B0:
	{ r6 = r5; r3 = add(r29,00000050); p0 = cmp.gtu(r5,00000001); r2 = sub(00000004,r5) }
	{ r3 = addasl(r3,r2,00000002); loop0(000202CC,r6) }
	{ r5 = memw(r4++#4); if (!p0) jump:nt 000202D8 }

l000202CC:
	{ r5 = add(r5,r2) }
	{ memw(r3++#4) = r5; r5 = memw(r4++#4) }

l000202D8:
	{ r9 = add(r29,00000030); r2 = add(r5,r2) }
	{ memw(r3++#4) = r2; r2 = add(r29,00000050) }
	{ r6 = memd(r29+88); r5 = memd(r29+80); r3 = add(r29,00000040); r4 = setbit(r2,00000004) }
	{ r2 = memw(r18+24); r8 = memw(r29+92) }
	{ r13 = memw(r30+r6<<#2); r7 = memw(r4) }
	{ r22 = memw(r30+r5<<#2); r27 = memw(r22+r5<<#2) }
	{ r21 = memw(r30+r8<<#2); r20 = memw(r22+r6<<#2) }
	{ r23 = memw(r30+r7<<#2); r12 = memw(r22+r8<<#2) }
	{ r26 = memw(r30+r7<<#2) }

l00020320:
	{ memw(r24+12) = r21; memw(r24+24) = r2; r4 = add(PC,00009EAD) }
	{  }
	{ memw(r24+4) = r23; memw(r24+8) = r20; r1 = 00000089 }
	{ memw(r29+44) = r21; memw(r24) = r22; r25:r24 = combine(r13,r12) }
	{ memw(r29+36) = r23; memw(r29+40) = r20 }
	{ memw(r29+8) = r6; memw(r29+32) = r22 }
	{ memw(r29+24) = r13; memw(r29+28) = r12 }
	{ memw(r29+16) = r27; memw(r29+20) = r26 }
	{ memw(r29+4) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; call logmsg_function }
	{ r3:r2 = combine(00000000,00000000); if (!p0.new) jump:nt 000203FC; p0 = cmp.gt(r14,00000000); r1:r0 = vaslw(r25:r24,00000002) }

l00020380:
	{ r5:r4 = vaslw(r27:r26,00000002) }

l00020384:
	{ if (!p0.new) jump:nt 000203F0; p0 = cmp.gt(r15,00000000) }

l00020388:
	{  }

l0002038C:
	{ r8 = r6; if (!p0.new) jump:nt 000203E4; p0 = cmp.gt(r12,00000000); loop1(00020398,r20) }

l00020398:
	{ r9 = r8; r12 = r19; if (!p0.new) jump:nt 000203D8; p0 = cmp.gt(r13,00000000) }

l000203A4:
	{ loop0(000203C0,r21) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r13 = memw(r18+16) }
	{ r13 = memw(r28+r9); r9 = add(r9,r0) }
	{ memw(r12++#4) = r13; nop }
	{ r19 = addasl(r19,r21,00000002) }

l000203D8:
	{ nop; nop; r8 = add(r8,r1) }

l000203E4:
	{ if (!cmp.eq(r7.new,r23)) jump:t 0002038C; r7 = add(r7,00000001); r6 = add(r6,r4) }

l000203F0:
	{ if (!cmp.eq(r3.new,r22)) jump:t 00020384; r3 = add(r3,00000001); r2 = add(r2,r5) }

l000203F4:
	{ if (!cmp.eq(r3.new,r22)) jump:t 00020388; r3 = add(r3,00000001) }

l000203FC:
	{ r4 = add(PC,00009E0F) }

l00020400:
	{ r4 = add(PC,0000000F) }
	{ r1 = 00000096 }
	{ memw(r29) = r17; call logmsg_function }

l00020414:
	{ r0 = 00000000 }

l00020418:
	{ r19:r18 = memd(r29+128); r17:r16 = memd(r29+136) }
	{ r23:r22 = memd(r29+112); r21:r20 = memd(r29+120) }
	{ r27:r26 = memd(r29+96); r25:r24 = memd(r29+104) }
	{ dealloc_return }

;; transpose_check: 0002042C
transpose_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00009CA5) }
	{ r17 = r0; r16 = r1; r1 = 0000009C }
	{ call logmsg_function }
	{ memw(r29) = r17 }
	{ if (cmp.eq(r2.new,00000006)) jump:t 00020470; r2 = memw(r17+16); r1 = 0000009D }

l0002045C:
	{ r3 = add(PC,00000014) }
	{ r2 = r16; call errlog_function }

l00020464:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00020470:
	{ if (cmp.eq(r2.new,00000002)) jump:t 0002048C; r2 = memw(r17+20); r1 = 0000009F }

l00020480:
	{ r1 = 0000009E; jump 00020464; r3 = add(PC,0000003F) }

l0002048C:
	{ r4 = add(PC,00009C7F) }
	{ memw(r29) = r17 }
	{ call logmsg_function }
	{ r0 = 00000000 }
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
	{ allocframe(+00000008) }
	{ if (cmp.gtu(r3,r5.new)) jump:t 000204C8; r5 = memw(r2+16) }

l000204B8:
	{ r6 = add(r29,00000010); r5 = add(r29,00000010); r0 = add(PC,0000003E) }
	{ memw(r29+4) = r6; call logv }

l000204C8:
	{ dealloc_return }

;; errlog_function: 000204CC
;;   Called from:
;;     00020200 (in transpose_execute)
;;     00020460 (in transpose_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00009BE6) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; addn_execute: 000204F0
addn_execute proc
	{ allocframe(00000030); memd(r29+496) = r17:r16; r4 = add(PC,00009D94) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+32) = r19:r18; r18 = memw(r17+4); r1 = 00000039 }
	{ r3 = memw(r17+8); r2 = r16 }
	{ memd(r29+16) = r23:r22; memd(r29+24) = r21:r20 }
	{ r21 = memw(r3); r20 = memw(r18) }
	{ memd(r29+8) = r25:r24 }
	{ r22 = memw(r20+12); r23 = memw(r20+8) }
	{ r25 = memw(r20); r24 = memw(r20+4) }
	{ memw(r29) = r17; r19 = memw(r21+16); call logmsg_function }
	{ r2 = memw(r17+16) }
	{ if (cmp.gtu(r3.new,r2)) jump:t 000205A0; r3 = 00000002 }

l0002053C:
	{ r4 = add(r18,00000004) }

l00020540:
	{ if (!cmp.gtu(r2,r5.new)) jump:nt 000205A0; r5 = add(r5,00000001); r4 = add(r4,00000004) }

l00020550:
	{ r6 = memw(r4); r3 = add(PC,0000000F) }
	{ if (cmp.eq(r7.new,r25)) jump:t 00020564; r7 = memw(r6) }

l00020564:
	{ if (cmp.eq(r7.new,r24)) jump:t 00020578; r7 = memw(r6+4); r3 = add(PC,00009D37) }

l00020578:
	{ if (cmp.eq(r7.new,r23)) jump:t 0002058C; r7 = memw(r6+8); r3 = add(PC,00009D23) }

l0002058C:
	{ if (cmp.eq(r6.new,r22)) jump:t 00020540; r6 = memw(r6+12); r3 = add(PC,00009D0F) }

l000205A0:
	{ r5 = memw(r21+20); r2 = mpyi(r24,r25); r3 = add(PC,00009D0A) }
	{ r1 = 00000040; r2 = mpyi(r2,r23) }
	{ r4 = mpyi(r2,r22) }
	{ if (!cmp.gtu(r2.new,r5)) jump:t 000205D4; r2 = asl(r4,00000002) }

l000205C8:
	{ r2 = r16 }
	{ r0 = FFFFFFFF; jump 00020654 }

l000205D4:
	{ r3 = memw(r20); r5 = memw(r20+4); p0 = cmp.eq(r4,00000000) }
	{ memw(r21) = r3; memw(r21+4) = r5; if (!p0) r3 = 00000000 }
	{ r6 = memw(r20+8) }
	{ memw(r21+8) = r6 }
	{ r7 = memw(r20+12) }
	{ memw(r21+24) = r2; memw(r21+12) = r7; if (p0) jump:nt 00020638 }

l000205F8:
	{ r2 = 00000000; loop1(00020604,r4) }
	{ r6 = memw(r17+16); r4 = r2 }
	{ if (!p0.new) r5:r4 = combine(r18,r2); if (p0.new) jump:nt 0002062C; p0 = cmp.eq(r6,00000000) }

l00020610:
	{ loop0(00020614,r6) }
	{ r6 = memw(r5++#4) }
	{ r6 = memw(r6+16) }
	{ r6 = addasl(r6,r3,00000002) }
	{ r6 = memw(r6) }
	{ nop; r4 = sfadd(r4,r6) }

l0002062C:
	{ memw(r19) = r4; r3 = r3; nop; r19 = add(r19,00000004) }

l00020638:
	{ r2 = r16; r1 = 0000004C; r4 = add(PC,00009C80) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l00020654:
	{ r19:r18 = memd(r29+32); r17:r16 = memd(r29+40) }
	{ r23:r22 = memd(r29+16); r21:r20 = memd(r29+24) }
	{ dealloc_return; r25:r24 = memd(r29+8) }

;; addn_check: 00020664
addn_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00009BD5) }
	{ r17 = r0; r16 = r1; r1 = 00000052 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.gtu(r2.new,00000002)) jump:t 000206A4; r2 = memw(r17+16); r1 = 00000053 }

l00020690:
	{ r3 = add(PC,00000003) }
	{ r2 = r16; call errlog_function }

l00020698:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l000206A4:
	{ if (cmp.eq(r2.new,00000002)) jump:t 000206BC; r2 = memw(r17+20); r1 = 00000054 }

l000206B4:
	{ jump 00020698; r3 = add(PC,0000002E) }

l000206BC:
	{ r2 = r16; r1 = 00000055; r4 = add(PC,00009BB2) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 000206DC
;;   Called from:
;;     00020524 (in addn_execute)
;;     00020648 (in addn_execute)
;;     00020678 (in addn_check)
;;     000206CC (in addn_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00020700; r5 = memw(r2+16) }

l000206EC:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000035) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00020700:
	{ dealloc_return }

;; errlog_function: 00020704
;;   Called from:
;;     00020694 (in addn_check)
;;     000206F4 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00009B19) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
00020728                         00 00 00 00 00 00 00 00         ........
00020730 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; execute_qinstancenorm_ref: 00020740
execute_qinstancenorm_ref proc
	{ allocframe(+00000070) }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+72) = r25:r24; memd(r29+96) = r19:r18 }
	{ r4 = memw(r2) }
	{ memd(r29+104) = r17:r16; memd(r29+64) = r27:r26; r17 = r1 }
	{ memd(r29+88) = r21:r20; r25 = memw(r4+8) }
	{ r24 = memw(r4+4); r19 = memw(r4) }
	{ r27 = memw(r4+12); r5 = mpyi(r25,r19) }
	{ memd(r29+80) = r23:r22; r16 = memw(r17+4); r2 += mpyi(r27,00000014) }
	{  }
	{ r21 = memw(r3+8); r18 = memw(r3) }
	{ memw(r29+52) = r25; r22 = memw(r3+4); r3 = mpyi(r5,r24) }
	{ r23 = memw(r18+16); r26 = memw(r4+16) }
	{ call fn000095F0; r20 = mpyi(r3,r27) }
	{ if (!cmp.gtu(r20,r2.new)) jump:t 000207C8; r2 = memw(r18+20); r1 = 00000071 }

l000207B4:
	{ r3 = add(PC,00000016) }

l000207B8:
	{ r2 = r17; call errlog_function }
	{ r0 = FFFFFFFF; jump 00020AA8 }

l000207C8:
	{ memw(r29+40) = r26; if (!p0.new) r1 = 00000072; if (p0.new) jump:nt 000207E0; p0 = cmp.eq(r11,00000002) }

l000207D4:
	{ jump 000207B8; r3 = add(PC,00009B40) }

l000207E0:
	{ memw(r18+8) = r24; memw(r18+12) = r27; r2 = r17 }
	{ memw(r18+4) = r25 }
	{ memw(r29+12) = r27; memw(r18) = 00000001 }
	{ memw(r29+4) = r25; memw(r29+8) = r24 }
	{ memw(r29) = 00000001; call logmsg_function }
	{ memw(r18+24) = r20; r12 = r27; p0 = cmp.gt(r24,00000000) }
	{ memb(r29+12) = r0.new; if (!p0) jump:nt 0002087C; r0 = p0 }

l00020824:
	{ r2 = memd(r29+40); r4 = 00000000 }

l00020828:
	{ p0 = cmp.gt(r25,00000000); if (!p0.new) jump:nt 00020874; loop1(00020834,r25) }

l00020834:
	{ if (!p0) r7:r6 = combine(r2,r16); p0 = cmp.gt(r12,00000000); r5 = r3; if (!p0.new) jump:nt 00020868 }

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
	{ if (!cmp.eq(r4.new,r24)) jump:t 00020828; r4 = add(r4,00000001) }

l0002087C:
	{ memw(r29+36) = r24; r24 = r12; p0 = cmp.gt(r12,00000000); r2 = mpyi(r24,r25) }

l00020880:
	{ memw(r29+36) = r24; r24 = r12; p0 = cmp.gt(r12,00000000) }
	{ memw(r29+24) = r21; memw(r29+32) = r23; if (p0) r19 = add(r24,00000000); r0 = p0 }
	{ memw(r29+28) = r2; memw(r29+20) = r22; if (p0) r26 = 3727C5AC }
	{ memw(r29+56) = r0; if (p0) r18 = 3F800000; if (!p0) jump:nt 00020920 }

l000208B4:
	{ r2 = memd(r29+28); r20 = r16; r22 = asl(r24,00000003); r21 += mpyi(r24,0000000C) }
	{ r17 = convert_w2sf(r2); r27 = addasl(r16,r24,00000002) }

l000208C8:
	{ r2 = memw(r20); r1 = r17 }
	{ call fn00009610; r0 = convert_uw2sf(r2) }
	{ r23 = add(r20,r21); r1 = r17; r2 = add(r20,r22) }
	{ memw(r2) = r0; r2 = togglebit(r0,0000001E) }
	{ r4 = memw(r20); r3 = memw(r27++#4) }
	{ r3 = convert_uw2sf(r4); r0 = convert_uw2sf(r3) }
	{ call fn00009610; r0 += sfmpy(r2,r3) }
	{ r2 = sfadd(r0,r26) }
	{ memw(r23) = r0; r0 = r2; call fn00009800 }
	{ r19 = r19; r20 = add(r20,00000004); r1:r0 = combine(r0,r18); call fn00009610 }
	{ memw(r23) = r0; if (!p0.new) jump:nt 000208C8; p0 = cmp.eq(r27,00000001) }

l00020920:
	{ r0 = memw(r29+48); r4 = r24 }
	{ p0 = r0; r27 = addasl(r16,r4,00000004) }
	{ if (p0) r18 = 00000000; if (p0) jump:nt 00020948 }

l0002093C:
	{ r18 = 00000000 }
	{ jump 00020A04; r9 = r10 }

l00020948:
	{ r3 = asl(r4,00000001); r2 += mpyi(r4,00000003) }
	{ memb(r29+12) = r3.new; r17 = r18; r5 = 00000000; r3 = addasl(r16,r3,00000002) }
	{ r3 = memw(r29+40) }
	{ memw(r29+44) = r2 }

l0002096C:
	{ memw(r29+40) = r5; r21 = 00000000; p0 = cmp.gt(r25,00000000) }
	{ if (!p0) jump:nt 000209F8 }

l00020978:
	{ r0 = memw(r29+56) }
	{ if (!p0) r25:r24 = combine(r3,r4); if (p0.new) r26 = add(r3,00000000); if (!p0.new) jump:nt 000209F0; p0 = r0 }

l0002098C:
	{ r19 = memd(r29+48); r20 = memd(r29+44); r22 = r4; r23 = addasl(r27,r4,00000002) }

l00020998:
	{ r3 = memw(r19); r2 = memb(r25++#1) }
	{ r4 = memw(r20); r2 = convert_w2sf(r2) }
	{ r2 = sfsub(r2,r3) }
	{ memb(r27) = r16.new; r27 = add(r27,00000004); r16 = sfmpy(r4,r2) }
	{ r1:r0 = combine(r18,r16) }
	{ r1:r0 = combine(r17,r16); r18 = r0; call fn00009600 }
	{ r17 = r0; r20 = add(r20,00000004); r19 = add(r19,00000004) }
	{ if (!cmp.eq(r22.new,00000001)) jump:t 00020998; r22 = add(r22,FFFFFFFF) }

l000209E0:
	{ r25 = memw(r29+52); r27 = r23; r4 = r24 }
	{ r3 = add(r3,r4) }

l000209F0:
	{ if (!cmp.eq(r21.new,r25)) jump:t 00020978; r21 = add(r21,00000001) }

l000209F8:
	{ r2 = memd(r29+36); r5 = memd(r29+40) }

l000209FC:
	{ if (!cmp.eq(r5.new,r2)) jump:t 0002096C; r5 = add(r5,00000001) }

l00020A04:
	{ r2 = memw(r29+28) }

l00020A08:
	{ if (!cmp.gt(r19.new,00000000)) jump:nt 00020A74; r19 = mpyi(r2,r4) }

l00020A14:
	{ r0 = 38D1B717 }
	{ call fn00009600; r27 -= asl(r19,00000002) }
	{ r2 = 437F0000 }
	{ r1:r0 = combine(r0,r2); call fn00009610 }
	{ r16 = r0; r20 = 00000000 }

l00020A38:
	{ r2 = memw(r27); r27 = add(r27,00000004) }
	{ r2 = sfsub(r2,r18) }
	{ call fn00009620; r0 = sfmpy(r2,r16) }
	{ r3 = memd(r29+32); r19 = r19; r2 = convert_uw2sf(r0):chop }
	{ p1 = cmp.eq(r19,00000000) }
	{ if (p2.new) r2 = FFFFFFFF; p0 = cmp.gt(r2,FFFFFFFF); p2 = cmp.gt(r2,000000FF) }
	{ if (!p0) r2 = add(r20,00000000) }
	{ memb(r3++#1) = r2 }
	{ memw(r29+32) = r3; if (!p1) jump:nt 00020A38 }

l00020A74:
	{ r3 = memd(r29+24); r4 = memd(r29+20); r0 = 00000000 }
	{ memw(r4+12) = FFFFFF81 }
	{ memw(r4+4) = 00000001; memw(r4+8) = 00000001 }
	{ memw(r3+8) = 00000001; memw(r4) = 00000001 }
	{ memw(r3+12) = 00000001; memw(r3) = 00000001 }
	{ memw(r3+4) = FFFFFF81 }
	{ r2 = memw(r4+16) }
	{ memw(r2) = r18 }
	{ r7 = memw(r3+16) }
	{ memw(r4+24) = 00000004; memw(r7) = r17 }
	{ memw(r3+24) = 00000004 }

l00020AA8:
	{ r19:r18 = memd(r29+96); r17:r16 = memd(r29+104) }
	{ r23:r22 = memd(r29+80); r21:r20 = memd(r29+88) }
	{ r27:r26 = memd(r29+64); r25:r24 = memd(r29+72) }
	{ dealloc_return }

;; check_qinstancenorm: 00020ABC
check_qinstancenorm proc
	{ allocframe(00000000); r2 = r1 }
	{ if (cmp.eq(r3.new,00000006)) jump:t 00020AD8; r3 = memw(r0+16); r1 = 000000FC }

l00020AD0:
	{ jump 00020AF0; r3 = add(PC,0000001B) }

l00020AD8:
	{ r3 = memw(r0+20); r0 = 00000000; r1 = 000000FD }
	{ if (p0.new) dealloc_return; p0 = cmp.eq(r3,00000003) }
00020AE4             60 42 00 00 03 C9 49 6A 6C C1 00 5A     `B....Ijl..Z

l00020AF0:
	{ r0 = FFFFFFFF }
	{ dealloc_return }

;; execute_finstancenorm: 00020AF8
execute_finstancenorm proc
	{ allocframe(00000068); r7 = r1 }
	{ r3 = memw(r0+8); r2 = memw(r0+4) }
	{ memd(r29+72) = r23:r22; memd(r29+80) = r21:r20 }
	{ r2 = memw(r2) }
	{ memd(r29+64) = r25:r24; r6 = memw(r3) }
	{ memd(r29+96) = r17:r16; r5 = memw(r2) }
	{ r23 = memw(r2+4); r21 = memw(r2+8); p0 = cmp.gt(r5,00000000) }
	{ r24 = memw(r2+12); r4 = mpyi(r5,r21) }
	{ memd(r29+56) = r27:r26; memd(r29+88) = r19:r18; r4 = mpyi(r4,r23) }
	{ memw(r29+20) = r23; memw(r29+28) = r5 }
	{ memw(r29+24) = r7 }
	{ r4 = memw(r6+20); r0 = mpyi(r4,r24) }
	{ if (!cmp.gtu(r3.new,r4)) jump:t 00020B74; r3 = asl(r0,00000002) }

l00020B4C:
	{ r2 = r7; r1 = 000000D6; r3 = add(PC,0000003E) }
	{ call errlog_function }
	{ r0 = FFFFFFFF }

l00020B60:
	{ r19:r18 = memd(r29+88); r17:r16 = memd(r29+96) }
	{ r23:r22 = memd(r29+72); r21:r20 = memd(r29+80) }
	{ r27:r26 = memd(r29+56); r25:r24 = memd(r29+64) }
	{ dealloc_return }

l00020B74:
	{ r2 = memw(r2+16); r16 = memw(r7+4); r0 = 00000000 }
	{ memw(r6+8) = r23; memw(r29+16) = r2 }
	{ memw(r6) = r5; memw(r6+24) = r3 }
	{ memw(r6+12) = r24; memw(r6+4) = r21 }
	{ if (!p0) jump:nt 00020B60 }

l00020B94:
	{ r3 += mpyi(r24,00000003); r2 = mpyi(r23,r21) }
	{ memw(r29) = r6; r18 = 00000000; r4 = asl(r24,00000001); r5 += mpyi(r24,0000000C) }
	{ memw(r29+44) = r5; r2 = convert_w2sf(r2); r6 = mpyi(r2,r24) }
	{ memw(r29+8) = r6; memw(r29+4) = r2; r27 = asl(r24,00000002); r17 = addasl(r16,r4,00000002) }
	{ r19 = memw(r29+4); r5 = asl(r24,00000003); r22 = addasl(r16,r3,00000002) }
	{ memw(r29+48) = r5; r1 += mpyi(r24,00000014) }
	{ memw(r29+12) = r1 }

l00020BD8:
	{ r0 = memw(r7+4); r1 = 00000000; p0 = cmp.gt(r23,00000000) }
	{ memw(r29+36) = r18; r2 = memd(r29+12); r3 = p0 }
	{ memw(r29+40) = r3; call fn000095F0 }
	{ r2 = memw(r29+8) }
	{ r0 = memd(r29+40); r4 = memd(r29+16); r3 = mpyi(r2,r18) }
	{ p0 = r0 }
	{ r18 = addasl(r4,r3,00000002) }
	{ if (p0) jump:nt 00020D30 }

l00020C0C:
	{ r26 = r16; r25 = r24; p0 = cmp.gt(r24,00000000) }
	{ r23 = 3727C5AC }
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
	{ r7 = memw(r29+44); r2 = togglebit(r20,0000001E) }
	{ r20 = add(r26,r7); r0 += sfmpy(r2,r20) }
	{ memw(r20) = r0; r0 = sfadd(r0,r23) }
	{ call fn00009800 }
	{ r1 = r0; r0 = 3F800000; call fn00009610 }
	{ memw(r20) = r0; r26 = add(r26,00000004); r25 = add(r25,FFFFFFFF) }
	{ p0 = cmp.eq(r25,00000000); if (!p0.new) jump:nt 00020C2C }

l00020C90:
	{ r14 = memw(r29+28); r0 = memw(r29+32) }
	{ r23 = memw(r29+20) }
	{ r0 = memw(r29+40); p1 = r0 }
	{ if (p0.new) r2 = memw(r29); if (!p0.new) jump:nt 00020D20; p0 = r0 }

l00020CB0:
	{ r3 = memw(r2+16); r2 = 00000000 }

l00020CB4:
	{ r4 = 00000000; if (!p0.new) jump:nt 00020D18; p0 = cmp.gt(r13,00000000) }

l00020CB8:
	{ r4 = 00000000 }

l00020CBC:
	{ if (p1) r7:r6 = combine(r18,r17); if (p1) r8 = add(r22,00000000); if (!p1) jump:nt 00020D10 }

l00020CC8:
	{ r5 = addasl(r3,r24,00000002); loop0(00020CE0,r24) }
	{ nop; nop; nop; nop }
	{ r12 = memw(r6); r9 = memw(r7); r7 = add(r7,00000004); r6 = add(r6,00000004) }
	{ r13 = memw(r8); r8 = add(r8,00000004); r9 = sfsub(r9,r12) }
	{ r9 = sfmpy(r9,r13) }
	{ memw(r3) = r9; r3 = add(r3,00000004) }
	{ r3 = r5; r18 = addasl(r18,r24,00000002) }

l00020D10:
	{ if (!cmp.eq(r4.new,r21)) jump:t 00020CBC; r4 = add(r4,00000001) }

l00020D18:
	{ if (!cmp.eq(r2.new,r23)) jump:t 00020CB4; r2 = add(r2,00000001) }

l00020D1C:
	{ if (!cmp.eq(r2.new,r23)) jump:t 00020CB8 }

l00020D20:
	{ r7 = memd(r29+24); r18 = memd(r29+36) }

l00020D24:
	{ if (!cmp.eq(r18.new,r14)) jump:t 00020BD8; r18 = add(r18,00000001) }

l00020D30:
	{ if (!p0.new) jump:nt 00020D7C; p0 = cmp.gt(r13,00000000); loop1(00020D38,r21) }

l00020D38:
	{ if (!p0) r5:r4 = combine(r16,r3); p0 = cmp.gt(r24,00000000); if (!p0.new) jump:nt 00020D70 }

l00020D44:
	{ loop0(00020D48,r24) }
	{ r7 = memw(r5); r6 = memw(r4); r4 = add(r4,00000004); r8 = add(r5,r27) }
	{ r7 = sfadd(r6,r7) }
	{ memw(r5) = r7; r5 = add(r5,00000004) }
	{ r1 = memw(r8) }
	{ r1 += sfmpy(r6,r6) }
	{ memw(r8) = r1; nop }
	{ r3 = addasl(r3,r24,00000002) }

l00020D70:
	{ nop; nop; nop }

l00020D7C:
	{ if (cmp.eq(r2.new,r23)) jump:nt 00020C0C; r2 = add(r2,00000001) }

;; check_finstancenorm: 00020D88
;;   Called from:
;;     00020D7C (in execute_finstancenorm)
check_finstancenorm proc
	{ allocframe(00000000); r2 = r1 }
	{ if (cmp.eq(r3.new,00000002)) jump:t 00020DA4; r3 = memw(r0+16); r1 = 00000103 }

l00020D9C:
	{ jump 00020DBC; r3 = add(PC,0000000F) }

l00020DA4:
	{ r3 = memw(r0+20); r0 = 00000000; r1 = 00000104 }
	{ if (p0.new) dealloc_return; p0 = cmp.eq(r3,00000001) }
00020DB0 55 42 00 00 03 C3 49 6A 06 C0 00 5A             UB....Ij...Z    

l00020DBC:
	{ r0 = FFFFFFFF }
	{ dealloc_return }

;; errlog_function: 00020DC4
;;   Called from:
;;     000207B8 (in execute_qinstancenorm_ref)
;;     00020B58 (in execute_finstancenorm)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00009501) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }

;; logmsg_function: 00020DE8
;;   Called from:
;;     00020800 (in execute_qinstancenorm_ref)
logmsg_function proc
	{ allocframe(00000008); r4 = 00000002 }
	{ if (cmp.gtu(r4,r3.new)) jump:t 00020E18; r3 = memw(r2+16) }

l00020DF8:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000011) }
	{ r6 = add(r29,00000010); r1 = 00000074; r4 = add(PC,00009534) }
	{ memw(r29+4) = r6; call logv }

l00020E18:
	{ dealloc_return }
00020E1C                                     00 00 00 00             ....

;; sub_int32_execute: 00020E20
sub_int32_execute proc
	{ allocframe(000000A0); r5 = r0 }
	{ r3 = memw(r5+8); r2 = memw(r5+4) }
	{ memd(r29+144) = r19:r18; memd(r29+112) = r27:r26 }
	{ r19 = memw(r2+4); r26 = memw(r2) }
	{ memd(r29+128) = r23:r22; memd(r29+136) = r21:r20; r22 = 00000000 }
	{ r0 = memw(r26); r6 = memw(r26+4) }
	{ r4 = memw(r19+4); r8 = memw(r19); p1 = cmp.eq(r0,00000001); p0 = cmp.eq(r6,00000001) }
	{ r12 = memw(r26+8); r10 = p1 }
	{ memw(r29+92) = r4; r9 = memw(r19+8); p2 = cmp.eq(r12,00000001); r18 = mux(p0,r4,r6) }
	{ r7 = memw(r26+12); r4 = mux(p1,r8,r0) }
	{ r0 = memw(r19+12); p1 = cmp.eq(r7,00000001); r2 = mpyi(r4,r18) }
	{ memw(r29+76) = r6; memd(r29+120) = r25:r24; r20 = mux(p2,r9,r12) }
	{ r21 = mux(p1,r0,r7); r2 = mpyi(r2,r20) }
	{ memd(r29+152) = r17:r16; r25 = memw(r3); r6 = r1; r2 = mpyi(r2,r21) }
	{ memw(r29+72) = r8; memw(r29+96) = r10; r10 = p2 }
	{ memw(r29+84) = r10; memw(r29+68) = r9 }
	{ memw(r29+80) = r0; memw(r29+104) = r4; r16 = asl(r2,00000002) }
	{ if (p0) jump:nt 00020ECC }

l00020EC8:
	{ r22 = mpyi(r7,r12) }

l00020ECC:
	{ r2 = r6; r1 = 000000BD; r0 = add(PC,000094DD) }
	{ memw(r29+64) = r12; r3 = memw(r19+16); r4 = add(PC,000094EC) }
	{ memw(r29+88) = r7; r23 = memw(r25+16); r27 = r5; r24 = r6 }
	{ r17 = memw(r26+16) }
	{ memw(r29) = r5; memw(r29+100) = r3; call logmsg_function }
	{ if (!cmp.gtu(r16,r2.new)) jump:t 00020F34; r2 = memw(r25+20); r1 = 000000BD }

l00020F18:
	{ r2 = r24; r0 = add(PC,00000015) }
	{ r3 = add(PC,000094C6) }

l00020F28:
	{ call errlog_function }
	{ r0 = FFFFFFFF; jump 00021160 }

l00020F34:
	{ r5 = memw(r26); r13 = memw(r19); r2 = r24 }
	{ r7 = memw(r26+8); r8 = memw(r26+12); p0 = cmp.eq(r5,r13) }
	{ r12 = memw(r19+12); r6 = memw(r26+4) }
	{ r3 = memw(r19+4); r9 = memw(r19+8) }
	{ memw(r29+52) = r16; memw(r29+48) = r25 }
	{ memw(r29+60) = r27; memw(r29+56) = r17 }
	{ if (p0) jump:nt 00020F7C }

l00020F70:
	{ if (p0.new) jump:nt 00020F7C; p0 = cmp.eq(r5,00000002) }

l00020F74:
	{ p0 = cmp.eq(r13,00000001); if (!p0.new) jump:nt 00020FB8 }

l00020F7C:
	{ if (p0.new) jump:nt 00020F8C; p0 = cmp.eq(r6,-00000001) }

l00020F80:
	{ if (p0.new) jump:nt 00020F8C; p0 = cmp.eq(r6,00000002) }

l00020F84:
	{ nop; if (!p0.new) jump:nt 00020FB8; p0 = cmp.eq(r3,00000002) }

l00020F8C:
	{ p0 = cmp.eq(r7,r9); if (p0.new) jump:nt 00020FA0 }

l00020F94:
	{ if (p0.new) jump:nt 00020FA0; p0 = cmp.eq(r7,00000002) }

l00020F98:
	{ p0 = cmp.eq(r9,00000001); if (!p0.new) jump:nt 00020FB8 }

l00020FA0:
	{ p0 = cmp.eq(r8,r12); if (p0.new) jump:nt 00020FEC }

l00020FA8:
	{ p0 = cmp.eq(r8,00000001); if (p0.new) jump:nt 00020FEC }

l00020FB0:
	{ p0 = cmp.eq(r12,00000001); if (p0.new) jump:nt 00020FEC }

l00020FB8:
	{ memw(r29+28) = r12; r1 = 000000BD; r0 = add(PC,000093F1) }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+20) = r3; r3 = add(PC,00009424) }
	{ memw(r29+4) = r6; memw(r29+12) = r8 }
	{ memw(r29) = r5; jump 00020F28 }

l00020FEC:
	{ memw(r29+44) = r21; r24 = memw(r29+104); r19 = r2 }
	{ memw(r29+20) = r3; memw(r29+36) = r18; r0 = add(PC,000093B1) }
	{ memw(r29+40) = r20; r1 = 000000BD; r4 = add(PC,00009420) }
	{ memw(r29+28) = r12; memw(r29+32) = r24 }
	{ memw(r29+16) = r13; memw(r29+24) = r9 }
	{ memw(r29+8) = r7; memw(r29+12) = r8 }
	{ memw(r29) = r5; memw(r29+4) = r6 }
	{ call logmsg_function }
	{ r26 = memw(r29+100); r3 = memw(r29+48); if (p0.new) r14 = 00000000; p0 = cmp.gt(r24,00000000) }
	{ r25 = memw(r29+56) }
	{ memb(r3+6) = r2.new; r2 = memw(r29+52) }
	{ memw(r3+4) = r18 }
	{ memw(r3+12) = r21; memw(r3+8) = r20 }
	{ if (p0) r12 = memw(r29+88); if (p0) r13 = memw(r29+80); if (!p0) jump:nt 00021138 }

l00021068:
	{ r7 = memd(r29+92); r5 = memd(r29+68); r6 = !cmp.eq(r13,00000001) }
	{ r2 = memd(r29+76); r0 = memd(r29+84); r9 = mpyi(r5,r7) }
	{ r4 = memd(r29+72); r3 = memd(r29+64); p0 = cmp.eq(r5,00000001); p2 = r0 }
	{ r0 = memw(r29+96); p1 = cmp.eq(r4,00000001); r9 = mpyi(r9,r13); r8 = mpyi(r3,r2) }
	{ if (p1) r9 = add(r14,00000000); p2 = cmp.eq(r7,00000001); r2 = cmp.eq(r12,00000000); r4 = mpyi(r13,r5) }
	{ p0 = r0; r8 = mpyi(r8,r12) }
	{ r5 = !cmp.eq(r12,00000001) }
	{ if (p0) r8 = add(r14,00000000); if (p2) r4 = add(r14,00000000); r7 = 00000000 }

l000210C0:
	{ r14 = 00000000; r13:r12 = combine(r25,r26); if (!p0.new) jump:nt 00021128; p0 = cmp.gt(r10,00000000) }

l000210C4:
	{ r14 = 00000000; r13:r12 = combine(r25,r26) }

l000210CC:
	{ r15 = r13; r28 = r12; if (!p0.new) jump:nt 00021118; p0 = cmp.gt(r12,00000000) }

l000210D8:
	{ loop1(000210DC,r20) }
	{ if (!p0) r1:r0 = combine(r15,r28); r10 = r23; if (!p0.new) jump:nt 0002110C; p0 = cmp.gt(r13,00000000) }

l000210E8:
	{ loop0(000210EC,r21) }
	{ r16 = memw(r0); r11 = memw(r1); r1 = addasl(r1,r5,00000002); r0 = addasl(r0,r6,00000002) }
	{ r11 = sub(r11,r16) }
	{ memw(r10++#4) = r11; nop }
	{ r23 = addasl(r23,r21,00000002) }

l0002110C:
	{ nop; r15 = addasl(r15,r2,00000002); r28 = addasl(r28,r3,00000002) }

l00021118:
	{ if (!cmp.eq(r14.new,r18)) jump:t 000210CC; r14 = add(r14,00000001); r12 = addasl(r12,r4,00000002); r13 = addasl(r13,r22,00000002) }

l00021128:
	{ if (!cmp.eq(r7.new,r24)) jump:t 000210C0; r7 = add(r7,00000001); r26 = addasl(r26,r9,00000002); r25 = addasl(r25,r8,00000002) }

l0002112C:
	{ if (!cmp.eq(r7.new,r24)) jump:t 000210C4; r7 = add(r7,00000001); r26 = addasl(r26,r9,00000002) }

l00021138:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,00009271) }

l0002113C:
	{ memb(r29) = r2.new; r2 = memw(r29+60); r0 = add(PC,00000031) }

l0002114C:
	{ r2 = r19; r1 = 000000BD; r4 = add(PC,0000000C) }
	{ call logmsg_function }
	{ r0 = 00000000 }

l00021160:
	{ r19:r18 = memd(r29+144); r17:r16 = memd(r29+152) }
	{ r23:r22 = memd(r29+128); r21:r20 = memd(r29+136) }
	{ r27:r26 = memd(r29+112); r25:r24 = memd(r29+120) }
	{ dealloc_return }

;; sub_int32_check: 00021174
sub_int32_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,000091FA) }
	{ r16 = r1; r1 = 00000037; r17 = r0 }
	{ memw(r29) = r17; r2 = r16; r0 = add(PC,000091CD) }
	{ call logmsg_function }
	{ if (cmp.eq(r2.new,00000004)) jump:t 000211C4; r2 = memw(r17+16); r1 = 00000038 }

l000211A8:
	{ r0 = add(PC,00000031) }
	{ r3 = add(PC,000091CE) }

l000211B4:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l000211C4:
	{ if (cmp.eq(r2.new,00000002)) jump:t 000211E4; r2 = memw(r17+20); r1 = 00000039 }

l000211D4:
	{ r0 = add(PC,00000005) }
	{ jump 000211B4; r3 = add(PC,000091B1) }

l000211E4:
	{ r2 = r16; r1 = 0000003A; r0 = add(PC,00009171) }
	{ memw(r29) = r17; call logmsg_function; r4 = add(PC,000091A9) }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00021208
;;   Called from:
;;     00020F00 (in sub_int32_execute)
;;     00021030 (in sub_int32_execute)
;;     00021158 (in sub_int32_execute)
;;     00021194 (in sub_int32_check)
;;     000211F0 (in sub_int32_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00021228; r5 = memw(r2+16) }

l00021218:
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010) }

l00021228:
	{ dealloc_return }

;; errlog_function: 0002122C
;;   Called from:
;;     00020F28 (in sub_int32_execute)
;;     000211B4 (in sub_int32_check)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r3 = 00000000 }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; prelu_execute: 00021250
;;   Called from:
;;     0002124C (in errlog_function)
prelu_execute proc
	{ allocframe(00000060); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
	{ memd(r29+56) = r25:r24; r2 = memw(r16+4); r0 = 38D1B717 }
	{ r3 = memw(r16+8) }
	{ memd(r29+72) = r21:r20; memd(r29+80) = r19:r18 }
	{ r5 = memw(r3+8); r24 = memw(r2) }
	{ r3 = memw(r3+4); r25 = memw(r3) }
	{ r21 = memw(r2+4); r18 = memw(r2+8) }
	{ memw(r29+28) = r3; r3 = memw(r24+4) }
	{ memw(r29+36) = r3; r3 = memw(r24+12) }
	{ r4 = memw(r18+16) }
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
	{ r2 = 437F0000 }
	{ r20 = 00000000; r1:r0 = combine(r0,r2); call fn00009610 }
	{ r2 = sfsub(r20,r26) }
	{ call fn00009620; r0 = sfmpy(r2,r0) }
	{ r3 = 47800000; r4 = add(PC,000091EE) }
	{ r2 = r17; r1 = 00000047; r27 = convert_uw2sf(r0):chop; r3 = sfmpy(r19,r3) }
	{ memw(r29) = r16 }
	{ call logmsg_function; r26 = convert_sf2uw(r3) }
	{ r3 = memw(r21+16); r2 = memw(r18+16); r4 = add(PC,000091DA) }
	{ r3 = memw(r3); r2 = memw(r2); r1 = 0000004A }
	{ r2 = r17; r9:r8 = convert_sf2df(r3); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r2 = 3F800000 }
	{ if (p0.new) r1 = 0000004B; if (!p0.new) jump:nt 00021384; p0 = sfcmp.gt(r19,r2) }

l0002136C:
	{ r3 = add(PC,000091B2) }

l00021374:
	{ r2 = r17; call errlog_function }

l00021378:
	{ r2 = r17 }
	{ r0 = FFFFFFFF; jump 00021558 }

l00021384:
	{ p1 = cmp.gt(r27,FFFFFFFF); r1 = 0000004E; if (!p0.new) jump:nt 000213A4; p0 = sfcmp.gt(r20,r19) }

l00021394:
	{ r1 = 0000004C; jump 00021374; r3 = add(PC,000091A0) }

l000213A4:
	{ r3 = memd(r29+32); r2 = memd(r29+36); p0 = cmp.gt(r27,000000FF); r7:r6 = convert_sf2df(r19) }
	{ r5 = memd(r29+40); r0 = memd(r29+44); if (p0) r27 = FFFFFFFF; r3 = mpyi(r2,r3) }
	{ if (!p1) r27 = 00000000; r3 = mpyi(r3,r0); r4 = add(PC,0000918E) }
	{ memd(r29) = r7:r6; memw(r29+8) = r26; r20 = and(r27,000000FF) }
	{ r27 = r20; r2 = add(00008000,mpyi(r20,r26)) }
	{ r2 = r17; r27 -= lsr(r2,00000010) }
	{ memw(r29+12) = 00010000 }
	{ call logmsg_function }
	{ if (!cmp.gtu(r19,r2.new)) jump:t 0002141C; r2 = memw(r25+20); p0 = cmp.eq(r19,00000000) }

l00021410:
	{ r1 = 00000050; jump 00021378; r3 = add(PC,00000029) }

l0002141C:
	{ r3 = memw(r24+4); r2 = memw(r24) }
	{ memw(r25) = r2; memw(r25+4) = r3 }
	{ r6 = memw(r24+8) }
	{ memw(r25+8) = r6 }
	{ r7 = memw(r24+12); if (!p0) r24 = 00000000 }
	{ memw(r25+24) = r19; memw(r25+12) = r7; if (p0) jump:nt 0002149C }

l00021448:
	{ memb(r23) = r2.new; r2 = memb(r22); r3 = r27 }

l0002144C:
	{ memb(r23) = r2.new; r2 = memb(r22) }
	{ if (!cmp.gtu(r20,r2.new)) jump:t 00021490; r2 = memb(r22) }

l00021464:
	{ r2 = add(00008000,mpyi(r2,r26)); r4 = add(PC,00000023) }
	{ r2 = r17; r3 += lsr(r2,00000010) }
	{ memw(r29+4) = r5; memw(r29+8) = r3 }
	{ memw(r29) = r24; call logmsg_function }
	{ if (!cmp.eq(r24.new,r19)) jump:t 00021448; r24 = add(r24,00000001); r22 = add(r22,00000001); r23 = add(r23,00000001) }

l00021490:
	{ if (!cmp.eq(r24.new,r19)) jump:t 0002144C; r24 = add(r24,00000001); r22 = add(r22,00000001) }

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
	{ r19 = add(r4,00000010) }

l00021500:
	{ memw(r4+24) = r2; r0 = memw(r4+16); r19 = add(r4,00000010) }
	{ r1 = memw(r18+16); r2 = memw(r18+24); call fn00009560 }
	{ r3 = memw(r20+16); r2 = memw(r19); r4 = add(PC,00009094) }
	{ r3 = memw(r3); r2 = memw(r2); r1 = 00000061 }
	{ r2 = r17; r9:r8 = convert_sf2df(r3); r7:r6 = convert_sf2df(r2) }
	{ memd(r29) = r9:r8; memd(r29+8) = r7:r6 }
	{ call logmsg_function }
	{ r2 = r17; r1 = 00000062; r4 = add(PC,00009081) }
	{ memw(r29) = r16; call logmsg_function }
	{ r0 = 00000000 }

l00021558:
	{ r19:r18 = memd(r29+80); r17:r16 = memd(r29+88) }
	{ r23:r22 = memd(r29+64); r21:r20 = memd(r29+72) }
	{ r27:r26 = memd(r29+48); r25:r24 = memd(r29+56) }
	{ dealloc_return }
0002156C                                     00 C0 00 7F             ....

;; prelu_check: 00021570
prelu_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00008F1A) }
	{ r17 = r0; r16 = r1; r1 = 00000069 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r1 = 0000006A; r3 = add(PC,00008F15) }
	{ if (!cmp.eq(r2.new,00000008)) jump:t 000215F8; r2 = memw(r17+16) }

l000215A4:
	{ r1 = 0000006B; r3 = add(PC,00000010) }
	{ if (!cmp.eq(r2.new,00000006)) jump:t 000215F8; r2 = memw(r17+20) }

l000215B8:
	{ r4 = memw(r17+4); r5 = 00000000 }

l000215BC:
	{ if (cmp.gt(r5.new,00000004)) jump:nt 00021608; r5 = add(r5,00000001); r2 = add(r2,00000004) }

l000215CC:
	{ if (!cmp.eq(r6.new,00000000)) jump:t 000215E4; r6 = memw(r4++#4); r3 = add(PC,00000038) }

l000215DC:
	{ r1 = 0000006D }
	{ r6 = memw(r17+8); r3 = add(PC,00008EEB) }

l000215E4:
	{ r6 = memw(r17+8); r3 = add(PC,0000002B) }

l000215EC:
	{ if (!cmp.eq(r6.new,00000001)) jump:t 000215BC; r6 = memw(r4+r2) }

l000215F8:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00021608:
	{ r2 = r16; r1 = 00000070; r4 = add(PC,00008ECF) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00021628
;;   Called from:
;;     00021324 (in prelu_execute)
;;     00021354 (in prelu_execute)
;;     000213FC (in prelu_execute)
;;     00021484 (in prelu_execute)
;;     00021538 (in prelu_execute)
;;     0002154C (in prelu_execute)
;;     00021584 (in prelu_check)
;;     00021618 (in prelu_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 0002164C; r5 = memw(r2+16) }

l00021638:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000003B) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l0002164C:
	{ dealloc_return }

;; errlog_function: 00021650
;;   Called from:
;;     00021374 (in prelu_execute)
;;     000215F8 (in prelu_check)
;;     00021640 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00008E1F) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
00021674             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; prelu_execute: 00021680
prelu_execute proc
	{ allocframe(00000038); memd(r29+496) = r17:r16; r4 = add(PC,00008FB5) }
	{ r17:r16 = combine(r0,r1) }
	{ memd(r29+40) = r19:r18; r7 = memw(r17+4); r1 = 00000038 }
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
	{ r2 = 00000000 }
	{ if (!p0.new) r3 = memw(r19+20); if (p0.new) r1 = 00000039; if (!p0.new) jump:nt 00021704; p0 = sfcmp.gt(r2,r22) }

l000216EC:
	{ r3 = add(PC,00008F61) }
	{ r2 = r16; call errlog_function }

l000216F8:
	{ r2 = r16 }
	{ r0 = FFFFFFFF; jump 00021794 }

l00021704:
	{ r2 = mpyi(r26,r25) }
	{ r2 = mpyi(r2,r24) }
	{ r23 = mpyi(r2,r23) }
	{ if (!cmp.gtu(r2.new,r3)) jump:t 00021724; r2 = asl(r23,00000002) }

l0002171C:
	{ jump 000216F8; r1 = 0000003A; r3 = add(PC,00000004) }

l00021724:
	{ r4 = memw(r18+4); r3 = memw(r18); p0 = cmp.eq(r23,00000000) }
	{ memw(r19) = r3; memw(r19+4) = r4 }
	{ r6 = memw(r18+8) }
	{ memw(r19+8) = r6 }
	{ r7 = memw(r18+12); r18 = -00000001 }
	{ memw(r19+24) = r2; memw(r19+12) = r7; if (p0) jump:nt 00021778 }

l00021744:
	{ r19 = memw(r20) }
	{ r1:r0 = combine(r18,r19); call fn00009600 }
	{ r24 = r0; r2 = sfmpy(r22,r19) }
	{ r23 = add(r23,FFFFFFFF); r1:r0 = combine(r18,r2); call fn000097B0 }
	{ memb(r21) = r2.new; r21 = add(r21,00000004); r20 = add(r20,00000004); r2 = sfadd(r24,r0) }

l00021778:
	{ r2 = r16; r1 = 00000042; r4 = add(PC,00008EF2) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }

l00021794:
	{ r19:r18 = memd(r29+40); r17:r16 = memd(r29+48) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+32) }
	{ r27:r26 = memd(r29+8); r25:r24 = memd(r29+16) }
	{ dealloc_return }

;; prelu_check: 000217A8
prelu_check proc
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00008E40) }
	{ r17 = r0; r16 = r1; r1 = 00000048 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ if (cmp.eq(r2.new,00000004)) jump:t 000217E8; r2 = memw(r17+16); r1 = 00000049 }

l000217D4:
	{ r3 = add(PC,0000002F) }
	{ r2 = r16; call errlog_function }

l000217DC:
	{ r2 = r16 }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l000217E8:
	{ if (cmp.eq(r2.new,00000002)) jump:t 00021800; r2 = memw(r17+20); r1 = 0000004A }

l000217F8:
	{ jump 000217DC; r3 = add(PC,0000001A) }

l00021800:
	{ r2 = r16; r1 = 0000004B; r4 = add(PC,00008E1E) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00021820
;;   Called from:
;;     000216CC (in prelu_execute)
;;     00021788 (in prelu_execute)
;;     000217BC (in prelu_check)
;;     00021810 (in prelu_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00021844; r5 = memw(r2+16) }

l00021830:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,0000001F) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00021844:
	{ dealloc_return }

;; errlog_function: 00021848
;;   Called from:
;;     000216F4 (in prelu_execute)
;;     000217D8 (in prelu_check)
;;     00021838 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r0 = add(PC,00008D83) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); r3 = 00000000 }
	{ dealloc_return }
0002186C                                     00 00 00 00             ....

;; sum_f_execute: 00021870
sum_f_execute proc
	{ allocframe(+00000030); r28 = 00000001 }
	{ r8 = memw(r0+4); r2 = memw(r0+8) }
	{ memd(r29+40) = r17:r16; r6 = memw(r0+16) }
	{ r14 = memw(r2); r7 = memw(r8); r2 = r1 }
	{ memd(r29+24) = r21:r20; memd(r29+32) = r19:r18; if (!p0.new) r17 = 00000001; p0 = cmp.eq(r6,00000003) }
	{ r3 = memw(r7+12); r15 = memw(r7); if (!p0) r10 = 00000001; if (!p0) r1 = 00000001 }
	{ r5 = memw(r7+4); r4 = memw(r7+8); if (p0) r13:r12 = combine(r3,r15); if (p0) r18 = 00000000 }
	{ r7 = memw(r14+16); r6 = memw(r7+16) }
	{ memd(r29+8) = r25:r24; memd(r29+16) = r23:r22 }
	{ memd(r29) = r27:r26; if (p0) jump:nt 000218DC }

l000218D0:
	{ r9:r8 = combine(00000001,00000001); r13:r12 = combine(00000001,00000001) }
	{ jump 00021A1C }

l000218DC:
	{ r1 = memw(r8+4); r10 = memw(r8+8); r9:r8 = combine(r4,r5) }
	{ r11 = memw(r1+12); r10 = memw(r10+16) }
	{ r10 = memw(r10); r1 = memw(r1+16) }
	{ if (!p0.new) r16 = add(r1,00000000); if (!p0.new) r9:r8 = combine(r4,r5); p0 = cmp.eq(r11,00000000); if (p0.new) jump:nt 00021958 }

l00021908:
	{ r13:r12 = combine(r3,r15); loop0(00021910,r11) }
	{ r17 = memw(r16++#4) }
	{ if (!cmp.gt(r17.new,00000006)) jump:t 0002192C; r17 = add(r10,sub(0000007F,r17)) }

l00021920:
	{ r12 = 00000001; r13 = 00000001; r9:r8 = combine(00000001,00000001) }

l0002192C:
	{ if (p0.new) r13 = 00000001; if (p0.new) jump:t 0002194C; p0 = cmp.eq(r9,00000000) }

l00021934:
	{ if (p0.new) r9 = 00000001; if (p0.new) jump:t 0002194C; p0 = cmp.eq(r9,00000002) }

l0002193C:
	{ if (p0.new) r8 = 00000001; if (p0.new) jump:t 0002194C; p0 = cmp.eq(r9,00000004) }

l00021944:
	{ if (p0.new) r12 = 00000001; p0 = cmp.eq(r17,00000003) }

l0002194C:
	{ nop; nop }
	{ r18 = r11 }

l00021958:
	{ if (cmp.eq(r0.new,00000004)) jump:t 0002197C; r0 = memb(r0+32); r11 = r9; p0 = cmp.eq(r18,00000000) }

l0002196C:
	{ r1 = r13; r28 = r12; r17 = r8 }
	{ jump 00021A1C }

l0002197C:
	{ r0 = r13; r17:r16 = combine(r8,r12); if (!p0) r11 = add(r9,00000000) }
	{ if (p0.new) r17:r16 = combine(r8,r12); if (!p0) r0 = add(r13,00000000); if (p0) jump:nt 000219D8 }

l00021994:
	{ loop0(00021998,r18) }
	{ r18 = memw(r1++#4) }
	{ if (!cmp.gt(r18.new,00000006)) jump:t 000219B4; r18 = add(r10,sub(0000007F,r18)) }

l000219A8:
	{ r16 = 00000000; r0 = 00000000; r17 = 00000000 }
	{ jump 000219D0 }

l000219B4:
	{ if (p0.new) r0 = 00000000; if (p0.new) jump:t 000219D0; p0 = cmp.eq(r10,00000000) }

l000219BC:
	{ if (p0.new) r11 = 00000000; if (p0.new) jump:t 000219D0; p0 = cmp.eq(r10,00000002) }

l000219C4:
	{ if (p0.new) r17 = 00000000; if (p0.new) jump:t 000219D0; p0 = cmp.eq(r10,00000004) }

l000219CC:
	{ r16 = -00000001; p0 = cmp.eq(r18,00000003) }

l000219D0:
	{ nop; nop }

l000219D8:
	{ p3 = cmp.eq(r0,00000000); p0 = cmp.eq(r11,00000000); p2 = cmp.eq(r17,00000000); p1 = cmp.eq(r16,00000000) }
	{ p1 = or(p1,p2) }
	{ if (!p2) r1 = add(r17,00000000); p2 = or(p1,p0) }
	{ r17 = cmp.eq(r16,00000001); if (!p0) r1 = add(r11,00000000); if (!p0) r10 = add(r1,00000000); if (p3) jump:nt 00021A1C }

l00021A0C:
	{ r1 = r0; r10 = r1; r17 = r10; r28 = r17 }

l00021A1C:
	{ r11 = memw(r14+20); p0 = cmp.gt(r12,00000000); r0 = mpyi(r13,r9) }
	{ r0 = mpyi(r0,r8) }
	{ r0 = mpyi(r0,r12) }
	{ if (!cmp.gtu(r0.new,r11)) jump:t 00021A58; r0 = asl(r0,00000002) }

l00021A3C:
	{ r1 = 000000C6; r0 = add(PC,00000016) }
	{ call errlog_function; r3 = add(PC,00008CA9) }
	{ r0 = FFFFFFFF; jump 00021BB4 }

l00021A58:
	{ memw(r14+24) = r0; memw(r14+12) = r1; r0 = 00000000 }
	{ memw(r14+4) = r17; memw(r14+8) = r10; if (p0) r0 = 00000000 }
	{ memw(r14) = r28; if (p0) r1 = 00000000; if (!p0) jump:nt 00021BB4 }

l00021A80:
	{ p2 = cmp.eq(r8,00000001); p1 = cmp.eq(r13,00000001); p3 = cmp.eq(r9,00000001); p0 = cmp.eq(r12,00000001) }
	{  }
	{ r28 = !cmp.eq(r4,00000001) }
	{ r15 = cmp.eq(r5,00000001) }

l00021AA0:
	{ p0 = cmp.gt(r8,00000000); r10 = 00000000; if (!p0.new) jump:nt 00021BA8 }

l00021AA4:
	{ p0 = cmp.gt(r8,00000000); r10 = 00000000 }

l00021AAC:
	{ p0 = cmp.gt(r9,00000000); r11 = 00000000; if (!p0.new) jump:nt 00021BA0 }

l00021AB0:
	{ p0 = cmp.gt(r9,00000000); r11 = 00000000 }

l00021AB8:
	{ if (!p0.new) jump:nt 00021B98 }

l00021AC0:
	{ p0 = cmp.gt(r13,00000000) }

l00021AC4:
	{ r18 = r1; if (!p0.new) jump:nt 00021B84; p0 = cmp.gt(r2,00000000) }

l00021ACC:
	{  }

l00021AD0:
	{ p0 = cmp.gt(r15,00000000); r22 = add(r19,r0); if (!p0.new) jump:nt 00021B7C }

l00021AD4:
	{ p0 = cmp.gt(r15,00000000); r22 = add(r19,r0) }

l00021ADC:
	{  }
	{ r21 += mpyi(r22,r5) }

l00021AE4:
	{ p0 = cmp.gt(r28,00000000); r24 = add(r21,r20); if (!p0.new) jump:nt 00021B74 }

l00021AF0:
	{ loop1(00021AFC,r28) }
	{ r23 += mpyi(r24,r4) }
	{ p0 = cmp.gt(r14,00000000); r24 = r17; r25 = add(r23,r22); if (!p0.new) jump:nt 00021B68 }

l00021B0C:
	{ p0 = cmp.gtu(r14,00000001); r26 = add(r14,FFFFFFFF) }
	{ if (p0) r27 = add(r26,FFFFFFFF); r24 += mpyi(r25,r3) }
	{ r25 = addasl(r6,r24,00000002) }
	{ r24 = add(r25,00000004) }
	{ r25 = memw(r25); if (p0) jump:nt 00021B34 }

l00021B2C:
	{ r24 = r25; jump 00021B64 }

l00021B34:
	{ r24 = memw(r24); r26 = add(r24,00000004); p0 = cmp.gtu(r26,00000001); loop0(00021B48,r27) }
	{ if (!p0) jump:nt 00021B60 }

l00021B48:
	{ r24 = memw(r26); r26 = add(r26,00000004); r27 = r24; r18 = sfadd(r18,r25) }
	{ nop; r25 = r27 }

l00021B60:
	{ r18 = sfadd(r18,r25) }

l00021B64:
	{ r18 = sfadd(r18,r24) }

l00021B68:
	{ nop; nop; r22 = add(r22,00000001) }

l00021B74:
	{ if (!cmp.eq(r20.new,r15)) jump:t 00021AE4; r20 = add(r20,00000001) }

l00021B7C:
	{ if (!cmp.eq(r19.new,r2)) jump:t 00021AD0; r19 = add(r19,00000001) }

l00021B80:
	{ if (!cmp.eq(r19.new,r2)) jump:t 00021AD4 }

l00021B84:
	{ memw(r16) = r18; r17 = r17; r16 = add(r16,00000004) }

l00021B88:
	{ memw(r16) = r18; r17 = r17 }

l00021B8C:
	{ p0 = cmp.eq(r17,r13); if (!p0.new) jump:nt 00021AC4 }

l00021B94:
	{ r7 = addasl(r7,r13,00000002) }

l00021B98:
	{ if (!cmp.eq(r11.new,r9)) jump:t 00021AB8; r11 = add(r11,00000001) }

l00021BA0:
	{ if (!cmp.eq(r10.new,r8)) jump:t 00021AAC; r10 = add(r10,00000001) }

l00021BA4:
	{ if (!cmp.eq(r10.new,r8)) jump:t 00021AB0 }

l00021BA8:
	{ if (!cmp.eq(r0.new,r12)) jump:t 00021AA0; r0 = add(r0,00000001) }

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
	{ allocframe(00000010); memd(r29+496) = r17:r16; r4 = add(PC,00008AC3) }
	{ r17 = r0; r16 = r1; r1 = 00000037 }
	{ memw(r29) = r17; r2 = r16; call logmsg_function }
	{ r2 = memw(r17+16) }
	{ if (cmp.gtu(r3.new,r2)) jump:t 00021C1C; r3 = 00000004 }

l00021BFC:
	{ r1 = 00000038; r0 = add(PC,00000000) }

l00021C04:
	{ r3 = add(PC,00008A9B) }

l00021C0C:
	{ r2 = r16; call errlog_function }
	{ dealloc_return; r17:r16 = memd(r29+8); r0 = FFFFFFFF }

l00021C1C:
	{ r1 = 00000039; if (!p0.new) jump:nt 00021C30; p0 = cmp.eq(r2,00000000) }

l00021C24:
	{ jump 00021C04; r0 = add(PC,00008A54) }

l00021C30:
	{ if (cmp.eq(r2.new,00000002)) jump:t 00021C50; r2 = memw(r17+20); r1 = 0000003A }

l00021C40:
	{ r0 = add(PC,0000003C) }
	{ jump 00021C0C; r3 = add(PC,00008A6A) }

l00021C50:
	{ r2 = r16; r1 = 0000003B; r4 = add(PC,00008A6E) }
	{ memw(r29) = r17; call logmsg_function }
	{ r0 = 00000000 }
	{ dealloc_return; r17:r16 = memd(r29+8) }

;; logmsg_function: 00021C6C
;;   Called from:
;;     00021BE4 (in sum_f_check)
;;     00021C5C (in sum_f_check)
logmsg_function proc
	{ allocframe(00000008); r3 = 00000002 }
	{ if (cmp.gtu(r3,r5.new)) jump:t 00021C90; r5 = memw(r2+16) }

l00021C7C:
	{ r5 = add(r29,00000010); r3 = 00000002; r0 = add(PC,00000000) }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); call logv }

l00021C90:
	{ dealloc_return }

;; errlog_function: 00021C94
;;   Called from:
;;     00021A44 (in sum_f_execute)
;;     00021C0C (in sum_f_check)
;;     00021C84 (in logmsg_function)
errlog_function proc
	{ allocframe(00000008); r4 = r3; r3 = 00000000 }
	{ memb(r29+1) = r6.new; r6 = add(r29,00000010); r5 = add(r29,00000010); call logv }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }

;; gemacca_asm: 00021CC0
;;   Called from:
;;     00021CBC (in errlog_function)
gemacca_asm proc
	{ allocframe(+00000020); r8 = zxth(r2); r1 = lsr(r1,00000001); r6 = lsr(r2,00000010) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r7 = lsr(r2,00000014); loop1(00021D20,r1) }
	{ memd(r29+16) = r21:r20; r10 = sub(00000020,r8); r9 = asl(r8,00000001); r13 = addasl(r0,r8,00000001) }
	{ p2 = cmp.eq(r0,r0); r11 = sub(00000010,r8); r6 = sub(r9,r6); M1 = r8 }
	{ r12 = r13; r13 = addasl(r13,r8,00000001); M0 = r11 }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r19:r18 = combine(00000000,00000000); r17:r16 = combine(00000000,00000000); loop0(00021D40,r7) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r21:r20 = memd(r0++m0); r15:r14 = memd(r0+8) }
	{ r15:r14 = memd(r0+8); r21:r20 = memd(r0++m0); p2 = not(p2); r17:r16 += vraddub(r15:r14,r21:r20) }
	{ if (p2) r12 = add(r12,r10); if (!p2) r12 = add(r12,r8); r19:r18 += vraddub(r15:r14,r21:r20) }
	{ r0 = add(r0,r6); r17 = add(r19,r18); r16 = add(r17,r16) }
	{ r19:r18 = memd(r3); r17 = mpyi(r17,r4); r16 = mpyi(r16,r4) }
	{ r17 = add(r17,r19); r16 = add(r16,r18); r12 = r13 }
	{ memd(r3++#8) = r17:r16; p2 = or(p2,!p2); r13 = addasl(r13,r8,00000001) }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r21:r20 = memd(r29+16) }
	{ dealloc_return }
00021D9C                                     00 00 00 00             ....

;; gemaccb_asm: 00021DA0
gemaccb_asm proc
	{ r2 = lsr(r2,00000002) }
	{ p0 = cmp.eq(r3,00000000); r4 = 01010101 }
	{ r5 = 00000010; if (p0) jump:nt 00021DC8; loop0(00021DC0,r2) }

l00021DC0:
	{  }
	{  }

l00021DC8:
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ jumpr r31 }
00021DE4             00 00 00 00 00 00 00 00 00 00 00 00     ............
00021DF0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; l2pref: 00021E00
l2pref proc
	{ r2 = combine(r2.l,r1.l); r3 = zxth(r3) }
	{ l2fetch(r0,r3:r2) }
	{ jumpr r31 }
00021E10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gemaddvvm_asm: 00021E20
;;   Called from:
;;     0000D584 (in gemm_asm)
gemaddvvm_asm proc
	{ r4 = asl(r4,00000002); r3 = lsr(r3,00000001) }
	{ r3 = add(r3,FFFFFFFF); M0 = r4 }
	{ dcfetch(r0,00000020) }
	{ r11:r10 = memd(r0++#8); r8 = r2 }
	{  }
	{  }
	{ r9 = memw(r29) }
	{  }
	{ p2 = !cmp.eq(r9,00000000) }
	{  }
	{ r6 = 00000004; loop0(00021EA0,r3) }
	{  }
	{  }
	{  }
	{  }
	{ dcfetch(r0,00000060) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{ r11:r10 = memd(r0++#8) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ dcfetch(r0,00000060) }
	{  }
	{  }
	{  }
	{ loop0(00021EF4,00000005) }
	{  }
	{  }
	{  }
	{ r6 = add(r6,r6) }
	{  }
	{  }
	{ jumpr r31 }
00021F14             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; gemmacbbw_asm: 00021F20
gemmacbbw_asm proc
	{ allocframe(+00000038); r4 = asl(r4,00000002) }
	{ memw(r29+48) = r28; memd(r29) = r17:r16; r5 = zxth(r5); r28 = lsr(r5,00000010) }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r3 = lsr(r3,00000003); M1 = r4 }
	{ memd(r29+8) = r19:r18; M0 = r5 }
	{ memd(r29+32) = r25:r24; loop1(00021FE0,r3) }
	{ r3 = r2; r8 = asl(r5,00000003) }
	{ memd(r29+40) = r27:r26 }
	{ r13 = r0; r8 = sub(r8,r5); r6 = lsr(r28,00000004) }
	{ r4 = sub(00000010,r8); r9 = r1 }
	{ r5 = add(r5,r5); r7 = add(r13,00000030); r8 += sub(r5,r28) }
	{ r10 = add(r13,r5); r14 = lsr(r5,00000001) }
	{ dcfetch(r7,00000000) }
	{ r7 = add(r7,r14); r13 = addasl(r13,r5,00000002) }
	{ r23:r22 = memd(r0+8); r15 = sub(00000020,r14) }
	{ r17:r16 = memd(r0++m0) }
	{ r21:r20 = memd(r0+8); r6 = add(r6,FFFFFFFF); r11 = add(r10,r5) }
	{ r19:r18 = memd(r0++m0); r12 = add(r11,r5); loop0(00021FE0,r6) }
	{  }
	{  }
	{  }
	{ dcfetch(r10,00000000) }
	{  }
	{  }
	{ r10 = add(r10,r14) }
	{  }
	{  }
	{ r21:r20 = memd(r0++m0); r23:r22 = memd(r0+8) }
	{  }
	{  }
	{ r19:r18 = memd(r0++m0); r17:r16 = memd(r0+8) }
	{  }
	{  }
	{ dcfetch(r11,00000000); r11 = add(r11,r14) }
	{  }
	{  }
	{ dcfetch(r12,00000000); r12 = add(r12,r14) }
	{  }
	{  }
	{ r21:r20 = memd(r0++m0); r19:r18 = memd(r0+8) }
	{  }
	{  }
	{ r25:r24 = memd(r0++m0); r17:r16 = memd(r0+8) }
	{  }
	{  }
	{ r15 = r14; r14 = r15 }
	{  }
	{  }
	{ r23:r22 = memd(r0++m0); r25:r24 = memd(r0+8) }
	{  }
	{  }
	{ r27:r26 = memd(r0); r21:r20 = memd(r0+8) }
	{  }
	{  }
	{ r0 = add(r0,r4) }
	{  }
	{  }
	{  }
	{ dcfetch(r7,00000000) }
	{  }
	{  }
	{ r7 = add(r7,r14) }
	{  }
	{  }
	{ r17:r16 = memd(r0++m0); r23:r22 = memd(r0+8) }
	{  }
	{  }
	{ r19:r18 = memd(r0++m0); r21:r20 = memd(r0+8) }
	{  }
	{  }
	{ r7 = r13 }
	{  }
	{  }
	{ r10 = add(r13,r5) }
	{  }
	{  }
	{ r21:r20 = memd(r0++m0); r23:r22 = memd(r0+8) }
	{  }
	{  }
	{ r19:r18 = memd(r0++m0); r17:r16 = memd(r0+8) }
	{  }
	{  }
	{ r11 = add(r10,r5) }
	{  }
	{  }
	{ r12 = add(r11,r5) }
	{  }
	{  }
	{ r21:r20 = memd(r0++m0); r19:r18 = memd(r0+8) }
	{  }
	{  }
	{ r25:r24 = memd(r0++m0); r17:r16 = memd(r0+8) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r23:r22 = memd(r0++m0); r25:r24 = memd(r0+8) }
	{  }
	{  }
	{ r21:r20 = memd(r0+8) }
	{  }
	{  }
	{ r27:r26 = memd(r0) }
	{  }
	{  }
	{ r0 = add(r0,r4) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0 = add(r0,r8) }
	{ r14 = lsr(r5,00000001) }
	{  }
	{  }
	{ r15 = sub(00000020,r14) }
	{ r7 = add(r7,r14) }
	{ dcfetch(r7,00000000) }
	{ r10 = add(r10,r14) }
	{ dcfetch(r10,00000000) }
	{ r11 = add(r11,r14) }
	{ dcfetch(r11,00000000) }
	{ r12 = add(r12,r14) }
	{ dcfetch(r12,00000000) }
	{ r7 = add(r7,r15) }
	{ dcfetch(r7,00000000) }
	{ r10 = add(r10,r15) }
	{ dcfetch(r10,00000000) }
	{ r11 = add(r11,r15); loop0(00022240,00000002) }
	{ dcfetch(r11,00000000) }
	{ r12 = add(r12,r15); r13 = addasl(r13,r5,00000002) }
	{ dcfetch(r12,00000000) }
	{ nop }
	{ dcfetch(r7,00000000); r7 = add(r7,r14) }
	{ dcfetch(r10,00000000); r10 = add(r10,r14) }
	{ dcfetch(r11,00000000); r11 = add(r11,r14) }
	{ dcfetch(r12,00000000); r12 = add(r12,r14) }
	{ dcfetch(r7,00000000); r7 = add(r7,r15) }
	{ dcfetch(r10,00000000); r10 = add(r10,r15) }
	{ dcfetch(r11,00000000); r11 = add(r11,r15) }
	{ dcfetch(r12,00000000); r12 = add(r12,r15) }
	{ r23:r22 = memd(r0+8) }
	{ dcfetch(r7,00000000); r17:r16 = memd(r0++m0); r9 = r1 }
	{ r21:r20 = memd(r0+8); r7 = add(r7,r14) }
	{ r19:r18 = memd(r0++m0); loop0(000222AC,r6) }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return; r28 = memw(r29+48) }
000222C4             00 00 00 00 00 00 00 00 00 00 00 00     ............
000222D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gemmpybbw_asm: 000222E0
;;   Called from:
;;     0000D570 (in gemm_asm)
gemmpybbw_asm proc
	{ allocframe(+00000040) }
	{ r5 = zxth(r5); r28 = lsr(r5,00000010) }
	{ memw(r29+48) = r28 }
	{ r4 = asl(r4,00000002); M0 = r5 }
	{ memd(r29) = r17:r16; r8 = asl(r5,00000003); r9 = lsr(r28,00000004) }
	{ memd(r29+8) = r19:r18; r13 = r0; r8 = sub(r8,r5); r3 = lsr(r3,00000003) }
	{ r9 = add(r9,FFFFFFFF); r4 = sub(00000010,r8); M1 = r4 }
	{ memd(r29+16) = r21:r20; r5 = add(r5,r5); r7 = add(r13,00000050); r8 += sub(r5,r28) }
	{ memd(r29+32) = r25:r24; memd(r29+24) = r23:r22; r10 = add(r13,r5); loop1(000223A0,r3) }
	{ memd(r29+40) = r27:r26; r11 = add(r10,r5); r6 = r1; loop0(000223A0,r9) }
	{ r12 = add(r11,r5); r14 = lsr(r5,00000001) }
	{ dcfetch(r7,00000000) }
	{ r15 = sub(00000020,r14); r7 = add(r7,r14); r13 = addasl(r13,r5,00000002) }
	{  }
	{  }
	{ r17:r16 = memd(r0++m0); r23:r22 = memd(r0+8) }
	{  }
	{  }
	{ r19:r18 = memd(r0++m0); r21:r20 = memd(r0+8) }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{  }
	{ dcfetch(r10,00000000) }
	{  }
	{  }
	{ r10 = add(r10,r14) }
	{  }
	{  }
	{ r21:r20 = memd(r0++m0); r23:r22 = memd(r0+8) }
	{  }
	{  }
	{ r19:r18 = memd(r0++m0); r17:r16 = memd(r0+8) }
	{  }
	{  }
	{ dcfetch(r11,00000000); r11 = add(r11,r14) }
	{  }
	{  }
	{ dcfetch(r12,00000000); r12 = add(r12,r14) }
	{  }
	{  }
	{ r21:r20 = memd(r0++m0); r19:r18 = memd(r0+8) }
	{  }
	{  }
	{ r25:r24 = memd(r0++m0); r17:r16 = memd(r0+8) }
	{  }
	{  }
	{ r15 = r14; r14 = r15 }
	{  }
	{  }
	{ r23:r22 = memd(r0++m0); r25:r24 = memd(r0+8) }
	{  }
	{  }
	{ r27:r26 = memd(r0); r21:r20 = memd(r0+8) }
	{  }
	{  }
	{ r0 = add(r0,r4) }
	{  }
	{  }
	{  }
	{ dcfetch(r7,00000000) }
	{  }
	{  }
	{ r7 = add(r7,r14) }
	{  }
	{  }
	{ r17:r16 = memd(r0++m0); r23:r22 = memd(r0+8) }
	{  }
	{  }
	{ r19:r18 = memd(r0++m0); r21:r20 = memd(r0+8) }
	{  }
	{  }
	{ r7 = r13 }
	{  }
	{  }
	{ r10 = add(r13,r5) }
	{  }
	{  }
	{ r21:r20 = memd(r0++m0); r23:r22 = memd(r0+8) }
	{  }
	{  }
	{ r19:r18 = memd(r0++m0); r17:r16 = memd(r0+8) }
	{  }
	{  }
	{ r11 = add(r10,r5) }
	{  }
	{  }
	{ r12 = add(r11,r5) }
	{  }
	{  }
	{ r21:r20 = memd(r0++m0); r19:r18 = memd(r0+8) }
	{  }
	{  }
	{ r25:r24 = memd(r0++m0); r17:r16 = memd(r0+8) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r23:r22 = memd(r0++m0); r25:r24 = memd(r0+8) }
	{  }
	{  }
	{ r27:r26 = memd(r0); r21:r20 = memd(r0+8) }
	{  }
	{  }
	{ r0 = add(r0,r4) }
	{  }
	{  }
	{ r0 = add(r0,r8) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r14 = lsr(r5,00000001); r13 = addasl(r13,r5,00000002) }
	{ r15 = sub(00000020,r14); loop0(000225C0,00000003) }
	{ nop }
	{ nop; nop; nop; nop }
	{ dcfetch(r7,00000000); r7 = add(r7,r14) }
	{ dcfetch(r10,00000000); r10 = add(r10,r14) }
	{ dcfetch(r11,00000000); r11 = add(r11,r14) }
	{ dcfetch(r12,00000000); r12 = add(r12,r14) }
	{ dcfetch(r7,00000000); r7 = add(r7,r15) }
	{ dcfetch(r10,00000000); r10 = add(r10,r15) }
	{ dcfetch(r11,00000000); r11 = add(r11,r15) }
	{ dcfetch(r12,00000000); r12 = add(r12,r15) }
	{ r6 = r1 }
	{ dcfetch(r7,00000000); r23:r22 = memd(r0+8) }
	{ r7 = add(r7,r14) }
	{ r17:r16 = memd(r0++m0) }
	{  }
	{ r21:r20 = memd(r0+8) }
	{ loop0(000226FC,r9) }
	{ r19:r18 = memd(r0++m0) }
	{ nop; nop; nop }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ r28 = memw(r29+48) }
	{ dealloc_return }

;; gemsuma_asm: 00022660
;;   Called from:
;;     0000D548 (in gemm_asm)
gemsuma_asm proc
	{ allocframe(+00000020); r8 = zxth(r2); r1 = lsr(r1,00000001); r6 = lsr(r2,00000010) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r7 = lsr(r2,00000014); loop1(000226C0,r1) }
	{ memd(r29+16) = r21:r20; r10 = sub(00000020,r8); r9 = asl(r8,00000001); r13 = addasl(r0,r8,00000001) }
	{ r11 = sub(00000010,r8); r6 = sub(r9,r6); M1 = r8 }
	{ p2 = cmp.eq(r0,r0); r12 = r13; r13 = addasl(r13,r8,00000001); M0 = r11 }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r19:r18 = combine(00000000,00000000); r17:r16 = combine(00000000,00000000); loop0(000226E0,r7) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r21:r20 = memd(r0++m0); r15:r14 = memd(r0+8) }
	{ r15:r14 = memd(r0+8); r21:r20 = memd(r0++m0); p2 = not(p2); r17:r16 += vraddub(r15:r14,r21:r20) }
	{ if (p2) r12 = add(r12,r10); if (!p2) r12 = add(r12,r8); r19:r18 += vraddub(r15:r14,r21:r20) }
	{ r0 = add(r0,r6); r17 = add(r19,r18); r16 = add(r17,r16) }
	{ r17 = mpyi(r17,r4); r16 = mpyi(r16,r4) }
	{ r17 = add(r17,r5); r16 = add(r16,r5); r12 = r13 }
	{ memd(r3++#8) = r17:r16; p2 = cmp.eq(r0,r0); r13 = addasl(r13,r8,00000001) }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r21:r20 = memd(r29+16) }
	{ dealloc_return }
00022738                         00 00 00 00 00 00 00 00         ........

;; gemsumb_asm: 00022740
;;   Called from:
;;     0000D558 (in gemm_asm)
gemsumb_asm proc
	{ r2 = lsr(r2,00000002) }
	{ p0 = cmp.eq(r3,00000000); r4 = 01010101 }
	{ r5 = 00000010; if (p0) jump:nt 00022768; loop0(00022760,r2) }

l00022760:
	{  }
	{  }

l00022768:
	{  }
	{  }
	{  }
	{  }
	{ jumpr r31 }
0002277C                                     00 00 00 00             ....

;; quantize_asm: 00022780
;;   Called from:
;;     000182C4 (in autorequantize_execute)
;;     00018644 (in requantize_execute)
quantize_asm proc
	{  }
	{ r5 = 0000007F }
	{ p0 = bitsclr(r4,r5) }
	{  }
	{  }
	{ r4 = add(r4,0000007F) }
	{ r4 = lsr(r4,00000007) }
	{  }
	{  }
	{  }
	{ if (!p0) r4 = add(r4,FFFFFFFF) }
	{  }
	{  }
	{  }
	{ loop0(000227E0,r4) }
	{ nop }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ if (p0) jumpr:nt r31 }
00022840 E6 62 29 1C 0B C5 E4 1F A7 C3 E9 1F E7 E3 29 1C .b)...........).
00022850 0A C7 E6 1F AC CA CB 1F 0C C0 83 28 00 C0 9F 52 ...........(...R
00022860 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00022880 08 44 03 ED 4B C4 04 8C 28 40 A5 19 E8 5F 08 B0 .D..K...(@..._..
00022890 00 42 00 00 09 C0 00 78 28 47 08 8C 23 42 03 8C .B.....x(G..#B..
000228A0 07 C0 62 70 2A 40 A9 19 2B 40 A9 19 0A CB E3 F3 ..bp*@..+@......
000228B0 2C 40 A9 19 2D C0 A9 19 46 42 04 C4 00 CA 81 A6 ,@..-...FB......
000228C0 08 C0 A8 60 AA 60 28 1C 05 C1 07 29 AB 61 28 1C ...`.`(....).a(.
000228D0 00 47 06 F2 07 60 02 74 00 C1 41 29 AC 62 28 1C .G...`.t..A).b(.
000228E0 00 40 45 1C 05 C1 07 29 AD 63 28 1C 00 47 06 F2 .@E....).c(..G..
000228F0 07 60 02 74 01 C1 41 29 2B 40 A9 19 46 4B EA 1F .`.t..A)+@..FK..
00022900 01 41 45 1C 05 C1 07 29 00 47 06 F2 07 60 02 74 .AE....).G...`.t
00022910 47 4D EC 1F 02 C1 41 29 2C 40 A9 19 2D 40 A9 19 GM....A),@..-@..
00022920 02 42 45 1C 05 C1 07 29 00 47 06 F2 07 60 02 74 .BE....).G...`.t
00022930 03 43 45 1C 43 C1 41 29 2A 80 A9 19 A9 47 C6 1F .CE.C.A)*....G..
00022940 42 D9 A0 29 00 C0 9F 52 00 00 00 00 00 00 00 00 B..)...R........
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
	{ r7 = add(r2,r0); r8 = 01010101 }

;; fn00022964: 00022964
;;   Called from:
;;     0000D030 (in unpad2d)
fn00022964 proc
	{ r7 = add(r2,r0); r8 = 00000001 }
	{ r10 = add(r0,r2); r9 = add(r8,r8); r5 = and(r0,0000007F) }
	{  }
	{  }
	{ r7 = and(r7,0000007F); r4 = and(r1,0000007F) }
	{ r3 = sub(r2,r7); r5 = add(r5,r2); r6 = sub(r4,r5) }
	{  }
	{ r3 = add(r3,0000007F); if (!p2.new) r9 = add(r8,00000000); p2 = cmp.gt(r5,0000007F) }
	{ r3 = lsr(r3,00000007) }
	{ p1 = cmp.gt(r6,FFFFFFFF) }
	{ loop0(000229E0,r3) }
	{ if (p1) r1 = add(r1,00000080) }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0 = r10 }
	{ jumpr r31 }
00022A08                         00 00 00 00 00 00 00 00         ........
00022A10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; memconvert_hvx: 00022A20
;;   Called from:
;;     00013754 (in concat_execute_slice_asm)
memconvert_hvx proc
	{ r4 = combine(r4.l,r4.l); r3 = combine(r3.l,r3.l) }
	{ r6 = memw(r29) }
	{ r9 = 01010101 }
	{  }
	{ loop1(00022A80,r6) }
	{ r15 = r0; r12 = 00000080 }
	{ r14 = and(r0,0000007F); r8 = add(r2,r0); r6 = r1 }
	{ r8 = and(r8,0000007F); r13 = and(r6,0000007F); r11 = add(r9,r9) }
	{  }
	{  }
	{  }
	{  }
	{ r14 = add(r14,r2); r7 = sub(r13,r14) }
	{  }
	{  }
	{ r10 = sub(r2,r8); p1 = cmp.gt(r7,FFFFFFFF) }
	{  }
	{ p2 = cmp.gt(r14,0000007F); r10 = add(r10,0000007F); if (p1) r6 = add(r6,r12) }
	{  }
	{ if (!p2) r11 = add(r9,00000000) }
	{  }
	{ r10 = lsr(r10,00000007) }
	{  }
	{  }
	{  }
	{  }
	{ loop0(00022B00,r10) }
	{  }
	{  }
	{ r1 = add(r1,r2); r0 = add(r0,r5); r15 = r0 }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r14 = and(r0,0000007F); r8 = add(r2,r0); r6 = r1 }
	{ r13 = and(r6,0000007F); r11 = add(r9,r9) }
	{  }
	{  }
	{  }
	{ r8 = and(r8,0000007F) }
	{  }
	{  }
	{  }
	{  }
	{ jumpr r31 }
00022B70 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; avgpool_aligned_hvx: 00022B80
;;   Called from:
;;     00012004 (in avgpool_execute_slice_asm)
avgpool_aligned_hvx proc
	{ r6 = memw(r29); r7 = sub(r5,r3); M0 = r2 }
	{ r6 = combine(r6.l,r6.l); r9 = 01010101; r7 = mpyi(r7,r2) }
	{ r1 = add(r1,00000080); r10 = r1; loop1(00022BC0,r4) }
	{ loop0(00022BC0,r3) }
	{ nop; nop; nop }

l00022BC0:
	{  }
	{  }
	{ nop; r10 = add(r10,r7); loop0(00022BC0,r3) }
	{ loop1(00022BC0,r4) }
	{ r2 = add(r2,FFFFFF80); r10 = r1 }
	{  }
	{ p0 = !cmp.eq(r2,00000000); r1 = add(r1,00000080) }
	{ if (p0) jump:nt 00022BC0 }

l00022BFC:
	{  }
	{ jumpr r31 }
00022C04             00 00 00 00 00 00 00 00 00 00 00 00     ............
00022C10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; avgpool_nonaligned_hvx: 00022C20
;;   Called from:
;;     00012018 (in avgpool_execute_slice_asm)
avgpool_nonaligned_hvx proc
	{ r6 = memw(r29); r7 = sub(r5,r3); M0 = r2 }
	{ r6 = combine(r6.l,r6.l); r9 = 01010101; r7 = mpyi(r7,r2) }
	{ r1 = add(r1,00000080); r10 = r1; loop1(00022C60,r4) }
	{ loop0(00022C60,r3) }
	{ nop; nop; nop }

l00022C60:
	{  }
	{ nop }
	{  }
	{ nop }
	{ nop; r10 = add(r10,r7); loop0(00022C60,r3) }
	{ loop1(00022C60,r4) }
	{ r2 = add(r2,FFFFFF80) }
	{  }
	{ r12 = and(r0,0000007F); r11 = add(r2,00000080); p0 = cmp.gt(r2,00000000) }
	{ if (p0) r11 = 00000080 }
	{ r12 = add(r12,r11); r13 = sub(00000000,r0) }
	{  }
	{ p1 = cmp.gt(r12,0000007F); if (p1.new) jump:nt 00022CC4 }

l00022CBC:
	{  }
	{  }

l00022CC4:
	{ r10 = r1 }
	{  }
	{ r1 = add(r1,00000080); if (p0) jump:nt 00022C60 }

l00022CDC:
	{  }
	{ jumpr r31 }
00022CE4             00 00 00 00 00 00 00 00 00 00 00 00     ............
00022CF0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; maxpool_aligned_hvx: 00022D00
;;   Called from:
;;     00015FD0 (in maxpool_execute_slice_asm)
maxpool_aligned_hvx proc
	{ r10 = r1; r7 = sub(r5,r3); M0 = r2 }
	{ r1 = add(r1,00000080); r7 = mpyi(r7,r2); loop1(00022D20,r4) }
	{ loop0(00022D20,r3) }

l00022D20:
	{  }
	{  }
	{ nop; r10 = add(r10,r7); loop0(00022D20,r3) }
	{ r2 = add(r2,FFFFFF80); r1 = add(r1,00000080); r10 = r1; loop1(00022D20,r4) }
	{ p0 = !cmp.eq(r2,00000000); if (p0.new) jump:t 00022D20 }

l00022D50:
	{  }
	{ jumpr r31 }
00022D58                         00 00 00 00 00 00 00 00         ........

;; maxpool_nonaligned_hvx: 00022D60
;;   Called from:
;;     00015FD8 (in maxpool_execute_slice_asm)
maxpool_nonaligned_hvx proc
	{ r10 = r1; r7 = sub(r5,r3); M0 = r2 }
	{ loop0(00022D80,r3) }
	{ r1 = add(r1,00000080); r7 = mpyi(r7,r2); loop1(00022D80,r4) }

l00022D80:
	{  }
	{ nop }
	{ nop; r10 = add(r10,r7); loop0(00022D80,r3) }
	{ r11 = r2; r12 = and(r0,0000007F); r2 = add(r2,FFFFFF80); loop1(00022D80,r4) }
	{ r13 = sub(00000000,r0); if (p0.new) r11 = 00000080; p0 = cmp.gt(r2,00000000) }
	{ r12 = add(r12,r11) }
	{ p1 = cmp.gt(r12,0000007F); if (p1.new) jump:nt 00022DD4 }

l00022DCC:
	{  }
	{  }

l00022DD4:
	{ r10 = r1 }
	{  }
	{ r1 = add(r1,00000080); if (p0) jump:nt 00022D80 }

l00022DEC:
	{ jumpr r31 }
00022DF0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gemvmpybbw_asm: 00022E00
;;   Called from:
;;     000154C8 (in matmul_asm)
gemvmpybbw_asm proc
	{ allocframe(00000010); r6 = memd(r29) }
	{ memd(r29) = r17:r16; r17 = 0000007F; r5 = asl(r5,00000002) }
	{ if (!p0.new) jump:nt 00022E20; p0 = bitsclr(r5,r17) }

l00022E1C:
	{  }

l00022E20:
	{ r8 = add(r2,00000080); r7 = lsr(r6,00000004) }
	{ dcfetch(r0,00000080); r7 = add(r7,FFFFFFFF) }
	{ r11:r10 = memd(r0++#8); r16 = 01010101; loop0(00022E80,r7) }
	{  }
	{  }
	{ r14 = 00000000 }
	{  }
	{  }
	{  }
	{ r13:r12 = memd(r0++#8) }
	{  }
	{  }
	{ r15 = 00000000 }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{  }
	{ dcfetch(r0,00000080) }
	{ r11:r10 = memd(r0++#8); r15:r14 += vraddub(r13:r12,r11:r10) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r13:r12 = memd(r0++#8) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r15:r14 += vraddub(r13:r12,r11:r10) }
	{ r14 = add(r14,r15) }
	{ r15 = mpyi(r1,r3); r14 = mpyi(r14,r3) }
	{ r1 = combine(r1.l,r1.l); r14 += mpyi(r15,r6) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r17:r16 = memd(r29) }
	{  }
	{ dealloc_return }
00022F04             00 00 00 00 00 00 00 00 00 00 00 00     ............
00022F10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; im2col33322_hvx: 00022F20
im2col33322_hvx proc
	{ allocframe(+00000040) }
	{ r2 = vsplatb(r2) }
	{  }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r7 = 00000020 }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20 }
	{ memd(r29+40) = r27:r26; memd(r29+32) = r25:r24; r7 = 00000078; M1 = r7 }
	{ r7 = 01010101; r23 = 00000381; M0 = r7 }
	{ r24 = 000012A0; r8 = add(r7,r7); r10 = asl(r7,00000003); r9 = asl(r7,00000002) }
	{ r11 = add(r10,r10); r13 = asl(r10,00000003); r12 = asl(r10,00000002) }
	{ r5 = add(r5,r4); r3 = r4; r27 = add(r13,r13) }

l00022F80:
	{ r15 = add(r3,r3); r1 = add(r1,r24); r25 = and(r1,00000060); r19 = r1 }
	{ r25 = sub(00000000,r25); r15 = mpyi(r15,r23) }
	{ r21 = add(r25,FFFFFFF7); r22 = FFFFFFE0; r15 = add(r0,r15) }
	{ r16 = add(r15,r23) }
	{ r20 = add(r25,FFFFFFEE); r17 = add(r16,r23) }
	{ r26 = 00000000; loop1(00022FE0,00000008) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{ r18 = FFFFFFE6 }
	{ loop0(00023020,00000005) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ nop; nop; nop }
	{  }
	{ r26 = add(r26,00000020) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ p2 = cmp.eq(r26,r24); if (p2.new) jump:nt 000230E0 }

l0002305C:
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r26 = add(r26,00000060) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ nop }

l000230E0:
	{ r3 = add(r3,00000001) }
	{ if (!p0.new) jump:t 00022F80; p0 = cmp.eq(r3,-00000001) }

l000230E8:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return }
000230FC                                     00 00 00 00             ....

;; im2col7732_asm: 00023100
im2col7732_asm proc
	{ allocframe(+00000040); r2 = vsplatb(r2) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r7 = 000000A0 }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20 }
	{ memd(r29+40) = r27:r26; memd(r29+32) = r25:r24; r7 = 00000020; M1 = r7 }
	{ r15 = FFFFFFFE; M0 = r7 }
	{ r15 += add(r4,r4) }
	{  }
	{ r16 = 000002A0; r7 = 01010101 }
	{ r8 = add(r7,r7); r10 = asl(r7,00000003); r9 = asl(r7,00000002) }
	{ r11 = add(r10,r10); r13 = asl(r10,00000003); r12 = asl(r10,00000002) }
	{ r23 = add(r4,r5); p0 = and(p0,p0); r14 = asl(r11,00000003) }
	{ r15 = mpyi(r15,r16) }
	{ p2 = cmp.eq(r4,00000000) }
	{ r0 = add(r0,r15); r22 = r4; p1 = and(p1,p1) }

l0002319C:
	{ r6 = mux(p2,r14,r8); r18 = add(r1,00000000); r15 = r0; r19 = FFFFFFEB }
	{ r16 = add(r15,0000029A); r15 = add(r15,FFFFFFFA); r21 = 00000000; loop1(000231C0,00000007) }
	{ nop }
	{  }
	{ r15 = add(r15,00000060) }
	{ p3 = cmp.eq(r21,00000005) }
	{  }
	{ r6 = mux(p3,r9,r10); r17 = 00000000; r16 = add(r16,00000060) }
	{ r21 = add(r21,00000001); r6 = mux(p2,r14,r6) }
	{  }
	{ r17 = FFFFFFE6 }
	{ loop0(00023220,00000004) }
	{  }
	{  }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r17 = FFFFFFE6 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r21 = 00000000; r6 = r8; r18 = add(r1,00000000); r15 = add(r0,00000540) }
	{ r16 = add(r15,0000029A); r15 = add(r15,FFFFFFFA); r19 = FFFFFFEB; loop1(000232C0,00000007) }
	{ nop }
	{ nop; nop; nop; nop }
	{  }
	{ r15 = add(r15,00000060) }
	{ r16 = add(r16,00000060) }
	{  }
	{ p3 = cmp.eq(r21,00000005) }
	{ r17 = FFFFFFD6; r6 = mux(p3,r9,r10) }
	{ loop0(00023300,00000004) }
	{ r21 = add(r21,00000001) }
	{ nop }
	{ r17 = FFFFFFE6 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r18 = add(r18,00000120) }
	{  }
	{  }
	{  }
	{  }
	{ r21 = 00000000; r18 = add(r1,00000000); r15 = add(r0,00000A80); r19 = FFFFFFEB }
	{ r16 = add(r15,0000029A); r15 = add(r15,FFFFFFFA); r6 = mux(p1,r14,r8); loop1(000233A0,00000007) }
	{ nop; nop; nop; nop }
	{  }
	{ r15 = add(r15,00000060) }
	{ r16 = add(r16,00000060) }
	{  }
	{ r21 = add(r21,00000001); p3 = cmp.eq(r21,00000005) }
	{ r17 = FFFFFFAC }
	{  }
	{ r6 = mux(p3,r9,r10) }
	{ r17 = FFFFFFE6; r6 = mux(p1,r14,r6); loop0(00023400,00000004) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r18 = add(r18,00000120) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ nop }
	{  }
	{ r21 = 00000000; r18 = add(r1,00000000); r15 = add(r0,00000FC0) }
	{ r15 = add(r15,FFFFFFFA); r6 = mux(p0,r14,r8); loop1(000234A0,00000007) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{  }
	{ r15 = add(r15,00000060) }
	{ r17 = 00000002; p3 = cmp.eq(r21,00000005) }
	{  }
	{ loop0(000234E0,00000004) }
	{ r6 = mux(p3,r9,r10) }
	{ r17 = FFFFFFE6 }
	{  }
	{ r6 = mux(p0,r14,r6); r21 = add(r21,00000001) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r18 = add(r18,00000080) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r22 = add(r22,00000001) }
	{ r1 = add(r1,00004600); p0 = cmp.eq(r22,r23); p2 = and(p2,p2) }
	{ p0 = cmp.gt(r22,0000006D); r0 = add(r0,00000540); p1 = cmp.gt(r22,0000006E); if (!p0) jump:t 0002319C }

l0002355C:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return }
00023570 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; biasadd_relu_requant_nonaligned_hvx: 00023580
biasadd_relu_requant_nonaligned_hvx proc
	{ allocframe(+00000020); r14 = 01010101; r8 = add(r4,0000007F) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r7 = r4; r15 = add(r14,r14) }
	{ r17 = asl(r4,00000002) }
	{ r16 = 00000001; r6 = and(r4,0000007F) }
	{ r6 = add(r6,FFFFFF80); p2 = cmp.eq(r6,00000000); r16 = combine(r17.l,r16.l); r8 = lsr(r8,00000007) }
	{ if (p2) r6 = 00000000; r18 = addasl(r1,r4,00000005); loop1(000235E0,r3) }
	{ l2fetch(r18,r17:r16); r10 = 00008000 }
	{ nop; nop; nop }
	{ r9 = r2; loop0(00023600,r8) }
	{ r18 = addasl(r18,r4,00000002) }
	{  }
	{  }
	{  }
	{  }
	{ r12 = and(r0,0000007F) }
	{  }
	{  }
	{ r11 = r7 }
	{  }
	{ p0 = cmp.gt(r7,0000007F) }
	{  }
	{  }
	{ if (p0) r11 = 00000080 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r12 = add(r12,r11) }
	{  }
	{ r5 = r14; r13 = sub(00000000,r0) }
	{ if (p1.new) r5 = add(r15,00000000); p1 = !cmp.gt(r12,0000007F) }
	{ r7 = add(r7,FFFFFF80) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ nop }
	{ r0 = add(r0,r6); r7 = r4; r1 = addasl(r1,r6,00000002) }
	{ l2fetch(r18,r17:r16); nop; r1 = add(r1,FFFFFF00) }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ dealloc_return }
000236D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

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
	{ r7 = add(r2,r0); r8 = 01010101; r1 = vsplatb(r1) }
	{  }
	{ r7 = and(r7,0000007F); r9 = add(r8,r8) }
	{ r2 -= add(r7,FFFFFF81) }
	{ r5 = and(r0,0000007F) }
	{ r2 = lsr(r2,00000007) }
	{ r5 = add(r5,r2) }
	{ loop0(00023740,r2) }
	{ if (!p2.new) r9 = add(r8,00000000); p2 = cmp.gt(r5,0000007F) }
	{  }
	{  }
	{ nop; nop }
	{  }
	{  }
	{  }
	{ jumpr r31 }
00023750 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gvmaccimw_asm: 00023760
gvmaccimw_asm proc
	{ r7 = memd(r29+4); r6 = memd(r29) }
	{ allocframe(+00000010); r9 = lsr(r5,00000004) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r9 = add(r9,FFFFFFFF) }
	{ nop; nop; nop }

l00023780:
	{ r6 = add(r6,FFFFFFFF); loop1(000237A0,r2) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r0 = add(r0,r4); r10 = r0; loop0(000237C0,r9) }
	{ r13:r12 = memd(r10++#16); r15:r14 = memd(r10+8); r17:r16 = combine(00000000,00000000) }
	{ nop; nop }
	{ r13:r12 = memd(r10++#16); r15:r14 = memd(r10+8); r17:r16 += vraddub(r15:r14,r13:r12) }
	{ r17:r16 += vraddub(r15:r14,r13:r12) }
	{ r11 = memw(r1); r16 = add(r16,r17) }
	{ r11 += mpyi(r16,r7) }
	{ memw(r1++#4) = r11; nop; nop }
	{ r0 = add(r0,r3); if (!p1.new) jump:t 00023780; p1 = cmp.eq(r22,00000000) }

l000237F0:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ dealloc_return }
000237F8                         00 00 00 00 00 00 00 00         ........

;; gvmaccb_asm: 00023800
gvmaccb_asm proc
	{ r2 = lsr(r2,00000002) }
	{ r4 = 01010101 }
	{ r2 = add(r2,FFFFFFFF); if (p0.new) jump:t 00023848; p0 = cmp.eq(r3,00000000) }

l00023818:
	{ loop0(00023840,r2) }
	{ r5 = 00000010 }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{  }
	{  }

l00023848:
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ jumpr r31 }
00023864             00 00 00 00 00 00 00 00 00 00 00 00     ............
00023870 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gvmaddvvm_asm: 00023880
gvmaddvvm_asm proc
	{ r11 = 80000000; r4 = asl(r4,00000002) }
	{  }
	{ r6 = memd(r29); r3 = r3; r8 = r2 }
	{ r7 = 00000004; p2 = !cmp.eq(r6,00000000) }
	{ dcfetch(r0,00000020) }
	{ M0 = r4 }
	{  }
	{ r10 = memw(r0++#4) }
	{ loop0(000238E0,r3) }
	{  }
	{  }
	{ nop }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{ r10 = memw(r0++#4) }
	{  }
	{  }
	{  }
	{ dcfetch(r0,00000040) }
	{ loop0(00023914,00000005) }
	{  }
	{  }
	{  }
	{  }
	{ r7 = add(r7,r7) }
	{  }
	{ jumpr r31 }
00023928                         00 00 00 00 00 00 00 00         ........
00023930 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gvmmacbbw_asm: 00023940
gvmmacbbw_asm proc
	{ r7 = memd(r29+4); r6 = memd(r29); r5 = asl(r5,00000002) }
	{ allocframe(+00000040); r8 = memw(r29+8) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; M0 = r5 }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r9 = lsr(r7,00000004); r5 -= mpyi(r5,00000003) }
	{ r24 = add(r3,00000003) }
	{ memd(r29+40) = r27:r26; memd(r29+32) = r25:r24 }
	{ r9 = add(r9,FFFFFFFF); r24 = lsr(r24,00000002); M1 = r5 }

l00023980:
	{ r10 = r1; r8 = add(r8,FFFFFFFF); r23 = r3; loop1(000239A0,r24) }
	{ nop; nop; nop; nop }
	{  }
	{ dcfetch(r0,00000040) }
	{ r20 = add(r0,r6); r21 = r0 }
	{ dcfetch(r20,00000040) }
	{ r15:r14 = memd(r21+8) }
	{ r13:r12 = memd(r21++#16); r22 = add(r20,r6); r11 = addasl(r20,r6,00000001) }
	{ r17:r16 = memd(r20+8) }
	{ r19:r18 = memd(r20++#16); r0 = addasl(r0,r6,00000002); loop0(00023A00,r9) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{  }
	{ dcfetch(r22,00000040) }
	{  }
	{  }
	{  }
	{ dcfetch(r11,00000040) }
	{  }
	{  }
	{ r13:r12 = memd(r22++#16); r19:r18 = memd(r22+8) }
	{  }
	{  }
	{ r15:r14 = memd(r11++#16); r17:r16 = memd(r11+8) }
	{  }
	{  }
	{  }
	{ dcfetch(r21,00000040) }
	{  }
	{  }
	{  }
	{ dcfetch(r20,00000040) }
	{  }
	{  }
	{ r13:r12 = memd(r21++#16); r15:r14 = memd(r21+8) }
	{  }
	{  }
	{ r19:r18 = memd(r20++#16); r17:r16 = memd(r20+8) }
	{  }
	{  }
	{  }
	{ dcfetch(r22,00000040) }
	{  }
	{  }
	{  }
	{ dcfetch(r11,00000040) }
	{  }
	{  }
	{ r13:r12 = memd(r22++#16); r19:r18 = memd(r22+8) }
	{  }
	{  }
	{ r15:r14 = memd(r11++#16); r17:r16 = memd(r11+8) }
	{  }
	{  }
	{ p0 = cmp.gt(r23,00000001) }
	{  }
	{  }
	{ p0 = cmp.gt(r23,00000002) }
	{  }
	{  }
	{  }
	{  }
	{ p0 = cmp.gt(r23,00000003) }
	{ r10 = r1; r23 = add(r23,FFFFFFFC) }
	{ p1 = cmp.eq(r8,00000000); r0 = add(r0,r4); if (!p1.new) jump:t 00023980 }

l00023B10:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return }
00023B24             00 00 00 00 00 00 00 00 00 00 00 00     ............
00023B30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gvmmpybbw_asm: 00023B40
gvmmpybbw_asm proc
	{ r7 = memd(r29+4); r6 = memd(r29); r5 = asl(r5,00000002) }
	{  }
	{ allocframe(+00000040); r8 = memw(r29+8) }
	{ memd(r29) = r17:r16; memd(r29+32) = r25:r24; M0 = r5 }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r24 = add(r3,00000003); r9 = lsr(r7,00000004) }
	{ memd(r29+40) = r27:r26; memd(r29+8) = r19:r18; r9 = add(r9,FFFFFFFF); r24 = lsr(r24,00000002) }
	{ nop }

l00023B80:
	{ r10 = r1; r8 = add(r8,FFFFFFFF); r23 = r3; loop1(00023BA0,r24) }
	{ nop; nop; nop; nop }
	{ r21 = r0 }
	{  }
	{ dcfetch(r0,00000040) }
	{ r20 = add(r0,r6) }
	{  }
	{ dcfetch(r20,00000040) }
	{ r13:r12 = memd(r21++#16); r15:r14 = memd(r21+8); r22 = add(r20,r6); r11 = addasl(r20,r6,00000001) }
	{ r19:r18 = memd(r20++#16); r17:r16 = memd(r20+8); r0 = addasl(r0,r6,00000002); loop0(00023BE0,r9) }
	{  }
	{  }
	{  }
	{ dcfetch(r22,00000040) }
	{  }
	{  }
	{  }
	{ dcfetch(r11,00000040) }
	{  }
	{  }
	{ r13:r12 = memd(r22++#16); r19:r18 = memd(r22+8) }
	{  }
	{  }
	{ r15:r14 = memd(r11++#16); r17:r16 = memd(r11+8) }
	{  }
	{  }
	{  }
	{ dcfetch(r21,00000040) }
	{  }
	{  }
	{  }
	{ dcfetch(r20,00000040) }
	{  }
	{  }
	{ r13:r12 = memd(r21++#16); r15:r14 = memd(r21+8) }
	{  }
	{  }
	{ r19:r18 = memd(r20++#16); r17:r16 = memd(r20+8) }
	{  }
	{  }
	{  }
	{ dcfetch(r22,00000040) }
	{  }
	{  }
	{  }
	{ dcfetch(r11,00000040) }
	{  }
	{  }
	{ r13:r12 = memd(r22++#16); r19:r18 = memd(r22+8) }
	{  }
	{  }
	{ r15:r14 = memd(r11++#16); r17:r16 = memd(r11+8) }
	{  }
	{  }
	{ p0 = cmp.gt(r23,00000001) }
	{  }
	{  }
	{ p0 = cmp.gt(r23,00000002) }
	{  }
	{  }
	{  }
	{  }
	{ p0 = cmp.gt(r23,00000003) }
	{ r10 = r1; r23 = add(r23,FFFFFFFC) }
	{ p1 = cmp.eq(r8,00000000); r0 = add(r0,r4); if (!p1.new) jump:t 00023B80 }

l00023CF0:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return }
00023D04             00 00 00 00 00 00 00 00 00 00 00 00     ............
00023D10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gvmsumimw_asm: 00023D20
gvmsumimw_asm proc
	{ r7 = memd(r29+4); r6 = memd(r29) }
	{ allocframe(+00000010); r8 = memw(r29+8); r9 = lsr(r5,00000004) }
	{ memd(r29+8) = r19:r18; memd(r29) = r17:r16; r9 = add(r9,FFFFFFFF) }
	{ nop; nop }

l00023D40:
	{ r6 = add(r6,FFFFFFFF); loop1(00023D60,r2) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r0 = add(r0,r4); r10 = r0; loop0(00023D80,r9) }
	{ r13:r12 = memd(r10++#16); r15:r14 = memd(r10+8); r17:r16 = combine(00000000,00000000) }
	{ nop; nop }
	{ r13:r12 = memd(r10++#16); r15:r14 = memd(r10+8); r17:r16 += vraddub(r15:r14,r13:r12) }
	{ r17:r16 += vraddub(r15:r14,r13:r12) }
	{ r11 = r8; r16 = add(r16,r17) }
	{ r11 += mpyi(r16,r7) }
	{ memw(r1++#4) = r11; nop; nop }
	{ r0 = add(r0,r3); if (!p1.new) jump:t 00023D40; p1 = cmp.eq(r22,00000000) }

l00023DB0:
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ dealloc_return }
00023DB8                         00 00 00 00 00 00 00 00         ........

;; gvmsumb_asm: 00023DC0
;;   Called from:
;;     0001A5B4 (in supernode_execute_hvx_slice)
gvmsumb_asm proc
	{ r2 = lsr(r2,00000002) }
	{ p0 = cmp.eq(r3,00000000) }
	{ r2 = r2; r4 = 01010101; if (p0) jump:nt 00023E08 }

l00023DD8:
	{ loop0(00023E00,r2) }
	{ r5 = 00000010 }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{  }
	{  }

l00023E08:
	{  }
	{  }
	{  }
	{  }
	{ jumpr r31 }
00023E1C                                     00 00 00 00             ....

;; gvconvsum2dbbw_asm: 00023E20
gvconvsum2dbbw_asm proc
	{ dcfetch(r0,00000000); r8 = memw(r29) }
	{ dcfetch(r0,00000020); r6 = memw(r29+8); r5 = asl(r5,00000002) }
	{ allocframe(+00000048); r11 = memw(r29+20) }
	{ memd(r29) = r17:r16; memd(r29+32) = r25:r24; r25 = lsr(r8,00000010) }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r8 = mpy(r8.h,r8.l) }
	{ memd(r29+40) = r27:r26; memd(r29+8) = r19:r18; M0 = r8 }
	{ r16 = 80000001; r12 = addasl(r8,r8,00000001) }
	{ r23 = mpyi(r6,r3) }
	{ memw(r29+56) = r4; r12 = sub(00000010,r12) }
	{ r6 = memw(r29+84); M1 = r12 }
	{ memw(r29+52) = r1; memw(r29+48) = r0; r12 = add(r12,00000010); r13 = asl(r8,00000001) }
	{ memw(r29+60) = r5; r13 = sub(r6,r3); r23 = sub(r23,r13); r6 = lsr(r6,00000004) }
	{ memw(r29+64) = r28; r16 = memw(r29+104); p3 = cmp.gt(r8,00000060); r6 = add(r6,FFFFFFFF) }
	{ if (p3) r12 = sub(r12,r8); r3 = mpyi(r3,r25) }
	{ memw(r29+84) = r6 }

l00023EC0:
	{ memw(r29+92) -= 00000001; r11 = memw(r29+48) }
	{ memw(r29+48) += r3; r22 = memw(r29+56); r28 = add(r11,00000040) }
	{ nop; nop; nop }

l00023EE0:
	{ r6 = memw(r29+88); r9 = memw(r29+52); r7 = 00000000 }
	{  }
	{  }
	{ dcfetch(r28,00000000) }
	{ r28 = add(r28,r8); loop1(00023F40,r6) }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); r5:r4 = combine(00000000,00000000); r1:r0 = combine(00000000,00000000) }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8); r27:r26 = combine(00000000,00000000); r25:r24 = combine(00000000,00000000) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ dcfetch(r28,00000000); r6 = memw(r29+84); r7 = add(r7,00000001); r1:r0 += vraddub(r15:r14,r21:r20) }
	{ loop0(00023FC0,r6) }
	{ p3 = cmp.eq(r7,00000002); r28 = add(r28,r8) }
	{ r5:r4 += vraddub(r17:r16,r19:r18) }
	{ if (p3) r7 = 00000000 }
	{  }
	{  }
	{ if (p3) r28 = add(r28,r12) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8) }
	{  }
	{  }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ r25:r24 += vraddub(r19:r18,r21:r20) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ r27:r26 += vraddub(r17:r16,r15:r14) }
	{  }
	{ dcfetch(r28,00000000) }
	{  }
	{  }
	{ r28 = add(r28,r8) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8) }
	{  }
	{  }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ r1:r0 += vraddub(r15:r14,r21:r20) }
	{ dcfetch(r28,00000000); r28 = add(r28,r8) }
	{ r5:r4 += vraddub(r17:r16,r19:r18) }
	{ r7 = add(r7,00000001) }
	{  }
	{  }
	{ p3 = cmp.eq(r7,00000002) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8) }
	{  }
	{  }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ r25:r24 += vraddub(r19:r18,r21:r20) }
	{ if (p3) r28 = add(r28,r12); if (p3) r7 = 00000000 }
	{ r27:r26 += vraddub(r17:r16,r15:r14) }
	{ r11 = sub(r11,r13) }
	{  }
	{  }
	{  }
	{ dcfetch(r11,00000040) }
	{  }
	{ r7 = 00000000; r28 = add(r11,00000040) }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); r28 = add(r28,r8) }
	{  }
	{  }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ r6 = memw(r29+112); r14 = memw(r29+108); r11 = sub(r11,r23) }
	{ r10 = memw(r29+96); r1 = add(r0,r1); r0 = r6 }
	{ r15 = memw(r29+60); r28 = add(r11,00000040); r0 += mpyi(r14,r1) }
	{  }
	{ memw(r10++#4) = r0 }
	{ p0 = cmp.gt(r22,00000001); r2 = add(r2,r15) }
	{  }
	{ r5 = add(r4,r5); r4 = r6 }
	{ dcfetch(r11,00000000) }
	{ dcfetch(r11,00000020); r4 += mpyi(r14,r5) }
	{  }
	{ if (p0) memuh(r10++#4) = r4 }
	{ p1 = cmp.gt(r22,00000002); if (p0) r2 = add(r2,r15) }
	{  }
	{ r25 = add(r24,r25); r24 = r6 }
	{ r24 += mpyi(r14,r25) }
	{  }
	{  }
	{ if (p1) memuh(r10++#4) = r24 }
	{ p0 = cmp.gt(r22,00000003); if (p1) r2 = add(r2,r15) }
	{  }
	{ r27 = add(r26,r27); r26 = r6 }
	{ r26 += mpyi(r14,r27) }
	{  }
	{  }
	{ if (p0) memuh(r10++#4) = r26 }
	{ if (p0) r2 = add(r2,r15) }
	{  }
	{ r22 = add(r22,FFFFFFFC) }
	{ memw(r29+96) = r10 }
	{ p2 = cmp.gt(r22,00000000); if (p2.new) jump:t 00023EE0 }

l00024184:
	{ r9 = memw(r29+92) }
	{ p1 = cmp.eq(r9,00000000); if (!p1.new) jump:t 00023EC0 }

l00024190:
	{ r16 = memd(r29+104); r6 = 00000004; loop0(00024198,00000005) }
	{  }
	{ r6 = add(r6,r6) }
	{  }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return; r28 = memw(r29+64) }

;; gvconv2dbbw_asm: 000241C0
gvconv2dbbw_asm proc
	{ r7 = memd(r29+4); r6 = memd(r29) }
	{ r9 = memw(r29+12); r8 = memw(r29+8); r5 = asl(r5,00000002) }
	{ r11 = memw(r29+20); r10 = memw(r29+16) }
	{ allocframe(+00000040); r12 = memw(r29+24) }
	{ memd(r29) = r17:r16; memd(r29+32) = r25:r24; r25 = lsr(r6,00000010) }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r6 = mpy(r6.h,r6.l) }
	{ memd(r29+40) = r27:r26; memd(r29+8) = r19:r18; M0 = r6 }
	{ memw(r29+52) = r1; memw(r29+48) = r0 }
	{ r16 = 80000001; r1 = addasl(r6,r6,00000001) }
	{ r23 = mpyi(r8,r3) }
	{ memw(r29+56) = r4; r1 = sub(00000010,r1) }
	{ p3 = cmp.gt(r6,00000060); r1 = add(r1,00000010); r13 = asl(r6,00000001); M1 = r1 }
	{ memw(r29+60) = r5; r13 = sub(r7,r3); r23 = sub(r23,r13); r7 = lsr(r7,00000004) }
	{ if (p3) r1 = sub(r1,r6); r7 = add(r7,FFFFFFFF); r3 = mpyi(r3,r25) }
	{ nop; nop; nop }

l00024260:
	{ r11 = memw(r29+48); r9 = add(r9,FFFFFFFF) }
	{ memw(r29+48) += r3; r22 = memw(r29+56); r26 = add(r11,00000040) }
	{ nop; nop; nop }

l00024280:
	{ r24 = memw(r29+52); r27 = 00000000 }
	{ loop1(000242C0,r8) }
	{ dcfetch(r26,00000000) }
	{ r26 = add(r26,r6); loop0(00024320,r7) }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8) }
	{  }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ nop; nop }
	{  }
	{  }
	{  }
	{ dcfetch(r26,00000000) }
	{  }
	{  }
	{ r26 = add(r26,r6) }
	{  }
	{  }
	{ r27 = add(r27,00000001); p3 = cmp.eq(r27,00000001) }
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8); if (p3) r26 = add(r26,r1); if (p3) r27 = 00000000 }
	{  }
	{  }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{  }
	{ dcfetch(r26,00000000) }
	{  }
	{  }
	{ r26 = add(r26,r6) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8) }
	{  }
	{  }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{  }
	{  }
	{  }
	{ dcfetch(r26,00000000) }
	{  }
	{  }
	{ r26 = add(r26,r6) }
	{  }
	{ r19:r18 = memd(r11+8); r27 = add(r27,00000001); p3 = cmp.eq(r27,00000001) }
	{  }
	{ r21:r20 = memd(r11++m0); if (p3) r26 = add(r26,r1); if (p3) r27 = 00000000 }
	{  }
	{  }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{  }
	{  }
	{ r11 = sub(r11,r13) }
	{  }
	{  }
	{ r26 = add(r11,00000040); r27 = 00000000 }
	{ r26 = add(r26,r6); loop0(00024320,r7) }
	{ dcfetch(r26,00000000) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8) }
	{  }
	{  }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ r0 = memw(r10++#4); r11 = sub(r11,r23) }
	{  }
	{ r5 = memw(r29+60); r26 = add(r11,00000040) }
	{ p0 = cmp.gt(r22,00000001); r2 = add(r2,r5) }
	{  }
	{  }
	{ if (p0) r0 = memw(r10++#4) }
	{  }
	{ dcfetch(r11,00000000) }
	{ p1 = cmp.gt(r22,00000002); if (p0) r2 = add(r2,r5) }
	{  }
	{  }
	{ if (p1) r0 = memw(r10++#4) }
	{  }
	{  }
	{ dcfetch(r11,00000020) }
	{ p0 = cmp.gt(r22,00000003); if (p1) r2 = add(r2,r5) }
	{  }
	{  }
	{ if (p0) r0 = memw(r10++#4) }
	{  }
	{ r22 = add(r22,FFFFFFFC) }
	{ if (p0) r2 = add(r2,r5) }
	{  }
	{  }
	{ p2 = cmp.gt(r22,00000000); if (p2.new) jump:t 00024280 }

l00024498:
	{ p1 = cmp.eq(r9,00000000); if (!p1.new) jump:t 00024260 }

l000244A0:
	{ r6 = 00000004; loop0(000244A8,00000005) }
	{  }
	{ r6 = add(r6,r6) }
	{  }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return }
000244CC                                     00 00 00 00             ....
000244D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; gvconvsum2dbbb_asm: 000244E0
;;   Called from:
;;     0001A65C (in supernode_execute_hvx_slice)
gvconvsum2dbbb_asm proc
	{ dcfetch(r0,00000000); r9 = 00000020; r8 = 01010101 }
	{ r6 = 00008000; r9 = 00000040 }
	{  }
	{  }
	{ r9 = 00000060 }
	{ r8 = add(r8,r8) }
	{  }
	{  }
	{ r8 = add(r8,r8) }
	{  }
	{  }
	{ dcfetch(r0,00000020); r8 = add(r8,r8) }
	{  }
	{ r6 = memw(r29+4); r8 = memw(r29) }
	{ dcfetch(r0,00000020); p0 = cmp.eq(r6,00000001); r6 = mpy(r8.l,r6.l) }
	{ memw(r29+4) = r6 }
	{ r14 = memw(r29+36); r6 = memw(r29+8) }
	{ r15 = memw(r29+40) }
	{  }
	{ allocframe(+00000048); r11 = memw(r29+20) }
	{ memd(r29) = r17:r16; memd(r29+32) = r25:r24; r25 = lsr(r8,00000010) }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r8 = mpy(r8.h,r8.l) }
	{ memd(r29+40) = r27:r26; memd(r29+8) = r19:r18; r21 = 00000060; M0 = r8 }
	{ r16 = 80000001; r12 = addasl(r8,r8,00000001) }
	{ r23 = mpyi(r6,r3) }
	{ memw(r29+56) = r4; r12 = sub(00000010,r12) }
	{ r6 = memw(r29+84); if (p0) r21 = add(r21,FFFFFFE0); M1 = r12 }
	{ memw(r29+68) = r21; p3 = cmp.gt(r6,000000C0); r21 = asl(r8,00000002) }
	{  }
	{ memw(r29+52) = r1; memw(r29+48) = r0; r12 = add(r12,00000010); r13 = asl(r8,00000001) }
	{ memw(r29+60) = r5; r13 = sub(r6,r3); r23 = sub(r23,r13); r6 = lsr(r6,00000004) }
	{ memw(r29+64) = r28; r16 = memw(r29+104); p3 = cmp.gt(r8,00000060); r6 = add(r6,FFFFFFFF) }
	{ r12 = sub(r12,r8); r3 = mpyi(r3,r25) }
	{ memw(r29+84) = r6 }
	{ dcfetch(r0,00000040); r21 = memw(r29+68) }
	{ nop; nop; nop }

l00024620:
	{ memw(r29+92) -= 00000001; r11 = memw(r29+48) }
	{ memw(r29+48) += r3; r22 = memw(r29+56); r28 = add(r11,r21) }
	{ nop; nop; nop }

l00024640:
	{ r6 = memw(r29+88); r9 = memw(r29+52); r7 = 00000000 }
	{  }
	{  }
	{ dcfetch(r28,00000000) }
	{ r28 = add(r28,r8); loop1(000246A0,r6) }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); r5:r4 = combine(00000000,00000000); r1:r0 = combine(00000000,00000000) }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8); r27:r26 = combine(00000000,00000000); r25:r24 = combine(00000000,00000000) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{ dcfetch(r28,00000000); r6 = memw(r29+84); r28 = add(r28,r8); r1:r0 += vraddub(r15:r14,r21:r20) }
	{ loop0(00024720,r6) }
	{ p3 = cmp.eq(r7,00000001) }
	{ r5:r4 += vraddub(r17:r16,r19:r18) }
	{ if (p3) r28 = add(r28,r12) }
	{  }
	{  }
	{ r7 = sub(00000001,r7) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8) }
	{  }
	{  }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ r25:r24 += vraddub(r19:r18,r21:r20) }
	{ nop; nop; nop }
	{ nop; nop; nop; nop }
	{ r27:r26 += vraddub(r17:r16,r15:r14) }
	{  }
	{ dcfetch(r28,00000000) }
	{  }
	{  }
	{ r28 = add(r28,r8) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8) }
	{  }
	{  }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ r1:r0 += vraddub(r15:r14,r21:r20) }
	{ dcfetch(r28,00000000); r28 = add(r28,r8) }
	{ r5:r4 += vraddub(r17:r16,r19:r18) }
	{ p3 = cmp.eq(r7,00000001) }
	{  }
	{  }
	{  }
	{ dcfetch(r28,00000000) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8) }
	{  }
	{  }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ r25:r24 += vraddub(r19:r18,r21:r20) }
	{ if (p3) r28 = add(r28,r12); r7 = sub(00000001,r7) }
	{ r27:r26 += vraddub(r17:r16,r15:r14) }
	{ r11 = sub(r11,r13) }
	{  }
	{  }
	{ r21 = memw(r29+68) }
	{  }
	{ dcfetch(r11,00000040); r7 = 00000000; r28 = add(r11,r21) }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8); r28 = add(r28,r8) }
	{  }
	{  }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ r6 = memw(r29+112); r14 = memw(r29+108); p0 = cmp.gt(r22,00000001); r11 = sub(r11,r23) }
	{ r10 = memw(r29+96); r20 = 00000001; r1 = add(r0,r1); r0 = r6 }
	{ r21 = memw(r29+68); r15 = memw(r29+60); r16 = extractu(r2,00000002,00000002); r0 += mpyi(r14,r1) }
	{ r16 &= asl(r20,r16) }
	{  }
	{ memw(r10++#4) = r0 }
	{ r5 = add(r4,r5); r4 = r6; r16 = vsplatb(r16) }
	{ r4 += mpyi(r14,r5) }
	{  }
	{  }
	{  }
	{ p1 = cmp.gt(r22,00000002); r20 = mux(p0,00000001,00000010) }
	{  }
	{ r28 = add(r11,r21) }
	{ if (p0) memuh(r10++#4) = r4 }
	{ r25 = add(r24,r25); r24 = r6 }
	{  }
	{ r24 += mpyi(r14,r25) }
	{  }
	{  }
	{  }
	{ if (p1) memuh(r10++#4) = r24; r7 = add(r2,r15); p2 = cmp.gt(r22,00000003) }
	{ r17 = extractu(r7,00000002,00000002) }
	{  }
	{ dcfetch(r11,00000000) }
	{ r2 = add(r2,r15) }
	{  }
	{  }
	{ r17 &= asl(r20,r17) }
	{  }
	{ dcfetch(r11,00000020) }
	{ r27 = add(r26,r27); r26 = r6; r17 = vsplatb(r17) }
	{ r26 += mpyi(r14,r27) }
	{  }
	{  }
	{  }
	{  }
	{ dcfetch(r11,00000040) }
	{ if (p0) r7 = add(r7,r15); r20 = mux(p1,00000001,00000010) }
	{  }
	{ if (p0) r2 = add(r2,r15) }
	{  }
	{ if (p2) memuh(r10++#4) = r26 }
	{ r18 = extractu(r7,00000002,00000002) }
	{ memw(r29+96) = r10; if (p1) r7 = add(r7,r15) }
	{ r20 = mux(p2,00000001,00000010); r18 &= asl(r20,r18); r19 = extractu(r7,00000002,00000002) }
	{ r19 &= asl(r20,r19); r18 = vsplatb(r18) }
	{  }
	{ r19 = vsplatb(r19) }
	{ r22 = add(r22,FFFFFFFC) }
	{  }
	{ if (p1) r2 = add(r2,r15) }
	{  }
	{ p3 = cmp.gt(r22,00000000); if (p2) r2 = add(r2,r15); if (p3.new) jump:t 00024640 }

l0002499C:
	{ r21 = memw(r29+68); r9 = memw(r29+92) }
	{ p1 = cmp.eq(r9,00000000); if (!p1.new) jump:t 00024620 }

l000249AC:
	{ r16 = memd(r29+104); r6 = 00000004; loop0(000249B4,00000005) }
	{  }
	{ r6 = add(r6,r6) }
	{  }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ dealloc_return; r28 = memw(r29+64) }
000249DC                                     00 00 00 00             ....

;; gvconv2dbbb_asm: 000249E0
;;   Called from:
;;     0001A6C4 (in supernode_execute_hvx_slice)
gvconv2dbbb_asm proc
	{ dcfetch(r0,00000000); r9 = 00000020; r8 = 01010101 }
	{ r6 = 00008000; r9 = 00000040 }
	{  }
	{  }
	{ r9 = 00000060 }
	{ r8 = add(r8,r8) }
	{  }
	{ dcfetch(r0,00000020) }
	{  }
	{ r8 = add(r8,r8) }
	{  }
	{  }
	{ dcfetch(r0,00000040); r8 = add(r8,r8) }
	{  }
	{ r7 = memd(r29+4); r6 = memd(r29) }
	{ r9 = memw(r29+12); r8 = memw(r29+8); p0 = cmp.eq(r7,00000001) }
	{ r11 = memw(r29+20); r10 = memw(r29+16); r7 = mpy(r7.l,r6.l) }
	{ r14 = memw(r29+28); r12 = memw(r29+24) }
	{ r15 = memw(r29+32); p3 = cmp.gt(r7,000000C0) }
	{  }
	{ allocframe(+00000048); r28 = 00000060 }
	{ memw(r29+68) = r28; if (p0) r28 = add(r28,FFFFFFE0) }
	{ memd(r29) = r17:r16; memd(r29+32) = r25:r24; r25 = lsr(r6,00000010) }
	{ memd(r29+24) = r23:r22; memd(r29+16) = r21:r20; r6 = mpy(r6.h,r6.l) }
	{ memd(r29+40) = r27:r26; memd(r29+8) = r19:r18; M0 = r6 }
	{ memw(r29+52) = r1; memw(r29+48) = r0; r17 = asl(r6,00000002) }
	{ if (!p3) r28 = add(r17,00000000) }
	{ r16 = 80000001; r1 = addasl(r6,r6,00000001) }
	{ r23 = mpyi(r8,r3) }
	{ memw(r29+56) = r4; r1 = sub(00000010,r1) }
	{ r1 = add(r1,00000010); r13 = asl(r6,00000001); M1 = r1 }
	{ memw(r29+60) = r5; r13 = sub(r7,r3); r23 = sub(r23,r13); r7 = lsr(r7,00000004) }
	{ r1 = sub(r1,r6); r7 = add(r7,FFFFFFFF); r3 = mpyi(r3,r25) }
	{ nop }

l00024B00:
	{ r11 = memw(r29+48); r9 = add(r9,FFFFFFFF) }
	{ memw(r29+48) += r3; r22 = memw(r29+56); r26 = add(r11,r28) }
	{ nop; nop; nop }

l00024B20:
	{ r24 = memw(r29+52); p2 = !cmp.eq(r2,r2) }
	{ loop1(00024B60,r8) }
	{ dcfetch(r26,00000000) }
	{ r26 = add(r26,r6); loop0(00024BC0,r7) }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8) }
	{  }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ nop; nop }
	{  }
	{  }
	{  }
	{ dcfetch(r26,00000000) }
	{  }
	{  }
	{ r26 = add(r26,r6) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r19:r18 = memd(r11+8); if (p2) r26 = add(r26,r1); p2 = not(p2) }
	{  }
	{  }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ nop; nop }
	{ nop; nop; nop; nop }
	{  }
	{  }
	{  }
	{ dcfetch(r26,00000000) }
	{  }
	{  }
	{ r26 = add(r26,r6) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8) }
	{  }
	{  }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{  }
	{  }
	{  }
	{ dcfetch(r26,00000000) }
	{  }
	{  }
	{ r26 = add(r26,r6) }
	{  }
	{ dcfetch(r26,00000000); r19:r18 = memd(r11+8) }
	{ p2 = not(p2) }
	{ r21:r20 = memd(r11++m0); if (p2) r26 = add(r26,r1) }
	{  }
	{  }
	{ r15:r14 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{  }
	{  }
	{ r11 = sub(r11,r13) }
	{  }
	{  }
	{ r26 = add(r11,r28); p2 = !cmp.eq(r2,r2) }
	{ r26 = add(r26,r6); loop0(00024BC0,r7) }
	{ dcfetch(r26,00000000) }
	{  }
	{  }
	{ r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8) }
	{  }
	{  }
	{ r19:r18 = memd(r11++m0); r17:r16 = memd(r11+8) }
	{ r11 = sub(r11,r23); r16 = extractu(r2,00000002,00000002) }
	{ r0 = memw(r10++#4) }
	{  }
	{ r5 = memd(r29+60); r17 = 00000001; r26 = add(r11,r28) }
	{ p0 = cmp.gt(r22,00000001); r16 &= asl(r17,r16) }
	{  }
	{ r16 = vsplatb(r16) }
	{  }
	{ if (p0) r0 = memw(r10++#4) }
	{  }
	{  }
	{ dcfetch(r11,00000000); p1 = cmp.gt(r22,00000002) }
	{  }
	{  }
	{  }
	{ if (p1) r0 = memw(r10++#4) }
	{  }
	{ p2 = cmp.gt(r22,00000003) }
	{  }
	{  }
	{  }
	{  }
	{ if (p2) r0 = memw(r10++#4) }
	{  }
	{  }
	{  }
	{ dcfetch(r11,00000020) }
	{  }
	{ r21 = add(r2,r5) }
	{  }
	{ r18 = extractu(r21,00000002,00000002) }
	{ r17 = mux(p0,00000001,00000010) }
	{ r18 &= asl(r17,r18) }
	{  }
	{  }
	{ r18 = vsplatb(r18) }
	{  }
	{  }
	{  }
	{ r2 = add(r2,r5) }
	{ dcfetch(r11,00000040) }
	{  }
	{ if (p0) r21 = add(r21,r5); r22 = add(r22,FFFFFFFC) }
	{ if (p1) r21 = add(r21,r5); r17 = mux(p1,00000001,00000010); r19 = extractu(r21,00000002,00000002) }
	{ r17 = mux(p2,00000001,00000010); r19 &= asl(r17,r19); r20 = extractu(r21,00000002,00000002) }
	{ r20 &= asl(r17,r20); r19 = vsplatb(r19) }
	{  }
	{ r20 = vsplatb(r20) }
	{ if (p0) r2 = add(r2,r5) }
	{  }
	{ if (p1) r2 = add(r2,r5) }
	{ p3 = cmp.gt(r22,00000000); if (p2) r2 = add(r2,r5); if (p3.new) jump:t 00024B20 }

l00024DE0:
	{ p1 = cmp.eq(r9,00000000); if (!p1.new) jump:t 00024B00 }

l00024DE8:
	{ r6 = 00000004; loop0(00024DF0,00000005) }
	{  }
	{ r6 = add(r6,r6) }
	{  }
	{ r19:r18 = memd(r29+8); r17:r16 = memd(r29) }
	{ r23:r22 = memd(r29+24); r21:r20 = memd(r29+16) }
	{ r27:r26 = memd(r29+40); r25:r24 = memd(r29+32) }
	{ r28 = memw(r29+68) }
	{ dealloc_return }
