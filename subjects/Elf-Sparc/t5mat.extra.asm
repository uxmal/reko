;;; Segment .text (08048000)
08048000 20 20 54 35 4D 41 54 00 00 00 00 02 00 00 00 03   T5MAT.........
08048010 00 00 03 F6 00 00 00 00 00 00 00 00 00 00 00 00 ................
08048020 00 00 00 00 00 00 00 00                         ........       

fn08048028()
	save	%sp,0xFFFFFF18,%sp
	sethi	0x00040258,%o0
	add	%o0,0x000001A0,%l7
	sethi	0x0004303C,%o0
	add	%o0,0x00000348,%l4
	sethi	0x00040276,%o0
	add	%o0,0x00000048,%o0
	st	%o0,[%sp+192]
	sethi	0x00040256,%o0
	add	%o0,0x000007FC,%l2
	sethi	0x00040258,%o0
	add	%o0,0x00000000,%l5
	sethi	0x00040268,%o0
	add	%o0,0x00000010,%lp
	ld	[%l5+0],%i4
	ld	[%lp+0],%o0
	add	%l7,0x00000000,%l3
	subcc	%i4,%o0,%g0
	be,a	080480D0
	ld	[%l3+12],%l0

l08048078:
	sub	%g0,%i4,%o2
	ld	[%lp+0],%o1
	or	%g0,0x00000006,%l1
	subcc	%o2,%o1,%g0
	be,a	08048290
	st	%i4,[%l5+0]

l08048090:
	or	%g0,0x00000001,%i4
	or	%g0,0x00000004,%i5

fn08048098()
	add	%l2,%l1,%o0
	add	%l3,%i5,%o1

fn080480A0()
	call	080480A0

fn080480A4()
	or	%g0,0x00000006,%o2
	ld	[%l3+%i5],%l0
	subcc	%l0,0x00000000,%g0
	ble	08048298
	add	%i5,0x00000004,%i5

fn080480B8()
	add	%i4,0x00000001,%i4
	subcc	%i4,0x0000001B,%g0
	ble	08048098
	add	%l1,0x00000006,%l1

l080480C8:
	ld	[%lp+0],%i4
	ld	[%l3+12],%l0

fn080480D0()
	ld	[%sp+192],%o0
	sll	%l0,0x00000002,%l0
	add	%o0,%l0,%l0
	st	%l0,[%sp+92]
	ld	[%l3+16],%l1
	or	%g0,%i1,%o1
	sll	%l1,0x00000002,%l1
	add	%o0,%l1,%l1
	st	%l1,[%sp+96]
	or	%g0,%i2,%o2
	ld	[%l3+20],%l0
	sll	%l0,0x00000002,%l0
	add	%o0,%l0,%l0
	st	%l0,[%sp+100]
	ld	[%l3+24],%l1
	sll	%l1,0x00000002,%l1
	add	%o0,%l1,%l1
	st	%l1,[%sp+104]
	ld	[%l3+28],%l0
	sll	%l0,0x00000002,%l0
	add	%o0,%l0,%l0
	st	%l0,[%sp+108]
	ld	[%l3+32],%l1
	sll	%l1,0x00000002,%l1
	add	%o0,%l1,%l1
	st	%l1,[%sp+112]
	ld	[%l3+36],%l0
	sll	%l0,0x00000002,%l0
	add	%l4,%l0,%l0
	st	%l0,[%sp+116]
	ld	[%l3+40],%l1
	sll	%l1,0x00000002,%l1
	add	%l4,%l1,%l1
	st	%l1,[%sp+120]
	ld	[%l3+44],%l0
	sll	%l0,0x00000002,%l0
	add	%l4,%l0,%l0
	st	%l0,[%sp+124]
	ld	[%l3+48],%l1
	sll	%l1,0x00000002,%l1
	add	%l4,%l1,%l1
	st	%l1,[%sp+128]
	ld	[%l3+52],%l0
	sll	%l0,0x00000002,%l0
	add	%l4,%l0,%l0
	st	%l0,[%sp+132]
	ld	[%l3+56],%l1
	sll	%l1,0x00000002,%l1
	add	%l4,%l1,%l1
	st	%l1,[%sp+136]
	ld	[%l3+60],%l0
	sll	%l0,0x00000002,%l0
	add	%l4,%l0,%l0
	st	%l0,[%sp+140]
	ld	[%l3+64],%l1
	sll	%l1,0x00000002,%l1
	add	%l4,%l1,%l1
	st	%l1,[%sp+144]
	ld	[%l3+68],%l0
	sll	%l0,0x00000002,%l0
	add	%l4,%l0,%l0
	st	%l0,[%sp+148]
	ld	[%l3+72],%l1
	sll	%l1,0x00000002,%l1
	add	%l4,%l1,%l1
	st	%l1,[%sp+152]
	ld	[%l3+76],%l0
	sll	%l0,0x00000002,%l0
	add	%l4,%l0,%l0
	st	%l0,[%sp+156]
	ld	[%l3+80],%l1
	sll	%l1,0x00000002,%l1
	add	%l4,%l1,%l1
	st	%l1,[%sp+160]
	ld	[%l3+84],%l0
	sll	%l0,0x00000002,%l0
	add	%l4,%l0,%l0
	st	%l0,[%sp+164]
	ld	[%l3+88],%l1
	sll	%l1,0x00000002,%l1
	add	%l4,%l1,%l1
	st	%l1,[%sp+168]
	ld	[%l3+92],%l0
	sll	%l0,0x00000002,%l0
	add	%l4,%l0,%l0
	st	%l0,[%sp+172]
	ld	[%l3+96],%l1
	sll	%l1,0x00000002,%l1
	add	%l4,%l1,%l1
	st	%l1,[%sp+176]
	ld	[%l3+100],%l0
	sll	%l0,0x00000002,%l0
	add	%l4,%l0,%l0
	st	%l0,[%sp+180]
	ld	[%l3+104],%l1
	sll	%l1,0x00000002,%l1
	add	%o0,%l1,%l1
	st	%l1,[%sp+184]
	ld	[%l3+108],%l0
	sll	%l0,0x00000002,%l0
	add	%o0,%l0,%l0
	st	%l0,[%sp+188]
	ld	[%l3+8],%l0
	ld	[%l3+4],%l1
	sll	%l0,0x00000002,%l0
	add	%o0,%l0,%o5
	sll	%l1,0x00000002,%l1
	or	%g0,%i0,%o0
	add	%l4,%l1,%o4
	call	08049764
	or	%g0,%i3,%o3
	st	%i4,[%l5+0]

fn08048290()
	jmpl	%i7,8,%g0
	restore	%g0,%g0,%g0

fn08048298()
	sll	%i4,0x00000002,%l1
	sethi	0x00040240,%o0
	add	%o0,0x00000020,%l3
	sub	%l1,%i4,%l1
	or	%g0,0x00000006,%o0
	add	%i6,0xFFFFFFE8,%l0
	sll	%l1,0x00000001,%l1
	st	%o0,[%l0+0]
	add	%l2,%l1,%l1
	add	%i6,0xFFFFFFE0,%l0
	st	%l1,[%l0+0]
	add	%i6,0xFFFFFFEC,%l0
	or	%g0,0x00000007,%l2
	st	%l2,[%l0+0]
	sethi	0x00040240,%l1
	add	%l7,0xFFFFFFE4,%o0
	add	%l3,0xFFFFFFF8,%o3
	add	%i6,0xFFFFFFE0,%o1
	add	%i6,0xFFFFFFE8,%o2
	or	%g0,0x00000018,%o4
	add	%i6,0xFFFFFFE4,%l0
	add	%l1,0x00000000,%l1

fn080482F0()
	call	080482F0

fn080482F4()
	st	%l1,[%l0+0]
	add	%l3,0xFFFFFFFC,%o0
	add	%l3,0x00000000,%o1
	add	%l7,0xFFFFFFE4,%o2

fn08048304()
	call	08048304

fn08048308()
	or	%g0,0x00000018,%o3
	ld	[%lp+0],%l0
	sub	%g0,%l0,%i4
	ba	08048290
	st	%i4,[%l5+0]
0804831C                                     36 2F D3 42             6/.B
08048320 49 E3 D5 00 39 0D 18 AD 44 7F E6 66 2E 2A 79 02 I...9...D..f.*y.
08048330 23 FA A2 CB 35 3B E7 A2 48 C3 50 00 00 00 00 00 #...5;..H.P.....
08048340 38 7B A8 82 3F 80 00 00 3F 19 99 9A BF 80 00 00 8{..?...?.......
08048350 BF 19 99 9A 40 20 00 00 3F B3 33 33 41 90 00 00 ....@ ..?.33A...
08048360 41 E0 00 00 47 C3 50 00 36 AA 14 A9 29 C8 A6 1D A...G.P.6...)...
08048370 1F 00 26 9F 43 88 94 7B 36 C5 6C 99 37 27 C5 AC ..&.C..{6.l.7'..
08048380 C2 8C 00 00 43 16 00 00 23 9C 12 81 27 D7 77 D5 ....C...#...'.w.
08048390 AB 28 1E A5 AC 2E 1E 6A 31 06 D5 AE 34 26 7A 41 .(.....j1...4&zA
080483A0 B8 17 79 4E 3C 06 83 26 3F 77 A4 83 BE 5C 7D 3D ..yN<..&?w...\}=
080483B0 25 2F 94 D1 29 57 77 D5 AC 93 1A D1 AD 82 96 CF %/..)Ww.........
080483C0 32 28 8B 1A 35 26 7A 41 B8 E3 35 F5 3C 86 83 27 2(..5&zA..5.<..'
080483D0 3C 63 9C 52 40 A7 D9 E8 44 1A 68 52 3F 33 33 33 <c.R@...D.hR?333
080483E0 36 AA 15 85 3D AB 86 2B 29 48 A4 F5 1E 2A DE 52 6...=..+)H...*.R
080483F0 3A F0 D1 8F 43 8F 00 00 C3 16 00 00 3E 86 24 DD :...C.......>.$.
08048400 43 BA 91 F2 35 86 37 BD 24 13 81 6F 25 D9 B8 CF C...5.7.$..o%...
08048410 AC 0C 24 9D 2F 7C CD 75 B0 28 57 45 B2 AA 61 B1 ..$./|.u.(WE..a.
08048420 B8 B2 32 68 3D 56 24 62 40 A4 FB 44 3E 85 49 98 ..2h=V$b@..D>.I.
08048430 3C 62 66 9D 44 9E 60 00 45 46 F0 00 40 D2 F9 DB <bf.D.`.EF..@...
08048440 44 7A 00 00 48 43 50 00 3D 4C DD 94 4A 24 B5 A0 Dz..HCP.=L..J$..
08048450 31 EB 15 1C 25 74 0C E9 31 74 5F 93 26 2E 37 CD 1...%t..1t_.&.7.
08048460 19 E3 9B 5D 4B 46 5D 40 3E 9E 35 3F 4A 21 68 50 ...]KF]@>.5?J!hP
08048470 46 1C 40 00 3F 80 DB 8C 4A 1D 25 B0 40 FA 3D 71 F.@.?...J.%.@.=q
08048480 4A 18 FC 10 CB 46 5D 40 31 8C 88 05 37 5B F3 05 J....F]@1...7[..
08048490 44 FA 00 00 2E 26 77 50 45 3B 80 00 48 CC 7E 00 D....&wPE;..H.~.
080484A0 49 DB BA 00 3A CC 78 EA 30 81 54 7E 26 0D 85 68 I...:.x.0.T~&..h
080484B0 36 AA CA C4 2B DA 24 A3 C3 88 C7 AE 45 82 E0 00 6...+.$.....E...
080484C0 43 80 38 52 3A 83 12 6F C4 7A 00 00 C2 FA 00 00 C.8R:..o.z......
080484D0 B1 FE E0 66 39 9C FB D6 BC 72 37 95 B2 2E 38 3D ...f9....r7...8=
080484E0 B9 21 96 85 3E 62 E1 4E C1 05 8F 10 3A 9B 66 2B .!..>b.N....:.f+
080484F0 BA 0E 6D 3D 43 70 00 00 44 7C 00 00 40 A0 00 00 ..m=Cp..D|..@...
08048500 3F 86 66 66 3F 66 66 66 41 20 00 00 00 00 00 00 ?.ff?fffA ......
08048510 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

fn08048520()
	save	%sp,0xFFFFFEE8,%sp
	or	%g0,%i1,%o0
	st	%o0,[%sp+108]
	add	%i3,0xFFFFFFF4,%o0
	st	%i0,[%i6+68]
	st	%o0,[%sp+100]
	st	%i2,[%i6+76]
	ld	[%i6+116],%o0
	add	%o0,0xFFFFFFFC,%l3
	ld	[%i6+120],%o0
	add	%o0,0xFFFFFFFC,%i2
	ld	[%i6+124],%o0
	add	%o0,0xFFFFFFFC,%l4
	ld	[%i6+128],%o0
	add	%o0,0xFFFFFFFC,%g1
	ld	[%i6+132],%o0
	add	%o0,0xFFFFFFFC,%l7
	ld	[%i6+136],%o0
	add	%o0,0xFFFFFFFC,%l2
	ld	[%i6+140],%o0
	add	%o0,0xFFFFFFFC,%l0
	ld	[%i6+144],%o0
	add	%o0,0xFFFFFFFC,%i4
	ld	[%i6+148],%o0
	add	%o0,0xFFFFFFFC,%i1
	ld	[%i6+152],%o0
	add	%o0,0xFFFFFFFC,%i0
	ld	[%i6+156],%o0
	add	%o0,0xFFFFFFFC,%l1
	ld	[%i6+160],%o0
	add	%o0,0xFFFFFFFC,%i3
	ld	[%i6+164],%o0
	add	%o0,0xFFFFFFFC,%lp
	ld	[%i6+168],%o0
	add	%o0,0xFFFFFFFC,%o7
	ld	[%i6+172],%o0
	add	%o0,0xFFFFFFFC,%l5
	ld	[%i6+176],%o0
	add	%o0,0xFFFFFFFC,%o5
	ld	[%i6+180],%o0
	add	%o0,0xFFFFFFFC,%o1
	ld	[%i6+92],%o0
	ld	[%o0+0],%o0
	subcc	%o0,0x00000000,%g0
	bg	080486D8
	sethi	0x00040240,%o0

l080485D8:
	add	%o0,0x00000650,%o3
	sethi	0x00040258,%o0
	add	%o0,0x00000150,%o4
	ld	[%i6+76],%o0
	st	%o0,[%sp+104]
	ld	[%i6+68],%o0
	ld	[%o4+0],%o2
	ld	[%o0+0],%o0
	subcc	%o0,0x00000003,%g0
	st	%o0,[%sp+116]
	bge	08048620
	subcc	%o0,0x00000001,%g0

l08048608:
	be	080486E0
	subcc	%o0,0x00000002,%g0

l08048610:
	be,a	08048640
	sethi	0x00040258,%o0

l08048618:
	ba	080486E4
	ld	[%sp+104],%o0

l08048620:
	ld	[%sp+116],%o0
	subcc	%o0,0x00000003,%g0
	be	08049250
	subcc	%o0,0x00000004,%g0

l08048630:
	be,a	0804869C
	sethi	0x00040258,%o0

l08048638:
	ba	080486E4
	ld	[%sp+104],%o0

l08048640:
	add	%o0,0x00000348,%o2
	ld	[%sp+104],%o0
	ldf	[%o3+24],%f5
	ld	[%o0+0],%o5
	ld	[%sp+108],%o0
	ldf	[%o3+28],%f7
	ld	[%o0+0],%o1
	subcc	%o5,%o1,%g0
	bl	080486D8
	or	%g0,%o1,%g1

l08048668:
	subcc	%o1,%o5,%g0
	bg	080486D8
	sll	%o1,0x00000003,%o1

l08048674:
	stf	%f5,[%i6-124]
	stf	%f7,[%i6-128]
	ldf	[%o3+32],%f5
	ldf	[%o3+36],%f7
	ld	[%sp+100],%o0
	stf	%f5,[%i6-132]
	add	%o0,0x00000008,%l2
	stf	%f7,[%i6-136]
	ba	08048D44
	ld	[%l2+%o1],%o0

l0804869C:
	add	%o0,0x00000348,%o5
	ld	[%sp+104],%o0
	ld	[%sp+108],%o1
	ld	[%o0+0],%o0
	ld	[%o1+0],%o1
	subcc	%o0,%o1,%g0
	st	%o0,[%sp+92]
	bl	080486D8
	st	%o1,[%sp+112]

l080486C0:
	ld	[%sp+112],%o1
	ld	[%sp+92],%o0
	or	%g0,%o1,%i5
	subcc	%o1,%o0,%g0
	ble,a	08049590
	ldf	[%o3+200],%f5

l080486D8:
	jmpl	%i7,8,%g0
	restore	%g0,%g0,%g0

l080486E0:
	ld	[%sp+104],%o0

l080486E4:
	ld	[%o0+0],%g1
	ld	[%sp+108],%o0
	ldf	[%o3-12],%f5
	ld	[%o0+0],%o2
	subcc	%g1,%o2,%g0
	bl	080486D8
	or	%g0,%o2,%l1

l08048700:
	subcc	%o2,%g1,%g0
	ldf	[%o3-8],%f7
	bg	080486D8
	sll	%o2,0x00000003,%o2

l08048710:
	stf	%f5,[%i6-140]
	stf	%f7,[%i6-144]
	ldf	[%o3-4],%f5
	ldf	[%o3+0],%f7
	ld	[%sp+100],%o0
	stf	%f5,[%i6-148]
	add	%o0,0x00000008,%l0
	stf	%f7,[%i6-152]
	ba	08048750
	ld	[%l0+%o2],%o0

l08048738:
	stf	%f5,[%o5+%o0]

l0804873C:
	add	%l1,0x00000001,%l1
	subcc	%l1,%g1,%g0
	bg	080486D8
	add	%o2,0x00000008,%o2

l0804874C:
	ld	[%l0+%o2],%o0

l08048750:
	sll	%o0,0x00000002,%o0
	ldf	[%i6-144],%f7
	ldf	[%l4+%o0],%f5
	fcmpes	%f2,%f3
	fmovs	%f2,%f0
	ldf	[%i2+%o0],%f15
	fbg,a	08048770
	fmovs	%f3,%f0

l08048770:
	fmuls	%f0,%f0,%f2
	ldf	[%o3+4],%f9
	ldf	[%i6-148],%f7
	fmuls	%f0,%f3,%f3
	ldf	[%i6-152],%f11
	fmuls	%f2,%f4,%f2
	fmuls	%f0,%f0,%f4
	fsubs	%f5,%f3,%f3
	fmuls	%f4,%f0,%f4
	fsubs	%f3,%f2,%f2
	ldf	[%o3+8],%f11
	fmuls	%f4,%f5,%f4
	ldf	[%o3+12],%f7
	fmuls	%f7,%f3,%f3
	fsubs	%f2,%f4,%f2
	fadds	%f2,%f3,%f2
	ldf	[%o3+16],%f9
	fsubs	%f4,%f0,%f4
	stf	%f5,[%lp+%o0]
	ldf	[%o3+20],%f5
	fcmpes	%f2,%f4
	sethi	0x00000000,%g0
	fbul,a	080487D0
	fmovs	%f4,%f2

l080487D0:
	ldf	[%o3+24],%f7
	fmuls	%f2,%f3,%f2
	ldf	[%lp+%o0],%f9
	fsubs	%f4,%f2,%f2
	ldf	[%l3+%o0],%f7
	stf	%f5,[%lp+%o0]
	ldf	[%o3+32],%f5
	fcmpes	%f3,%f2
	ldf	[%o3+28],%f17
	fbug,a	08048858
	ldf	[%l5+%o0],%f3

l080487FC:
	ldf	[%l7+%o0],%f5
	ldf	[%i4+%o0],%f7
	fdivs	%f2,%f3,%f2
	ldf	[%o3+36],%f9
	ldf	[%o3+40],%f11
	ldf	[%l3+%o0],%f13
	fadds	%f6,%f5,%f5
	fadds	%f2,%f4,%f2
	ldf	[%o3+44],%f7
	fmuls	%f2,%f5,%f2
	ldf	[%o3+28],%f9
	fmuls	%f2,%f3,%f2
	fadds	%f2,%f4,%f2
	fcmpes	%f8,%f2
	fmovs	%f8,%f0
	fbul,a	08048840
	fmovs	%f2,%f0

l08048840:
	ldf	[%o3+36],%f7
	fadds	%f0,%f3,%f3
	ldf	[%o3+48],%f5
	ldf	[%l5+%o0],%f3
	fmuls	%f3,%f2,%f2
	fadds	%f2,%f4,%f8

l08048858:
	ldf	[%o3+28],%f7
	fsubs	%f3,%f1,%f3
	ldf	[%o3+52],%f5
	fmuls	%f1,%f2,%f0
	ldf	[%o3+56],%f9
	fmuls	%f3,%f4,%f3
	ldf	[%o3+28],%f13
	ldf	[%o3+60],%f5
	fadds	%f0,%f3,%f3
	ldf	[%o3+76],%f9
	fcmpes	%f7,%f2
	fdivs	%f0,%f3,%f1
	fbuge	080488F0
	ldf	[%l3+%o0],%f1

l08048890:
	fsubs	%f6,%f1,%f6
	ldf	[%o4+8],%f5
	ldf	[%o4+4],%f7
	fdivs	%f2,%f3,%f2
	ldf	[%i0+%o0],%f11
	fadds	%f5,%f4,%f4
	ldf	[%o3+80],%f7
	fmuls	%f6,%f3,%f3
	fdivs	%f2,%f4,%f2
	ldf	[%o3+12],%f11
	fdivs	%f3,%f8,%f3
	ldf	[%lp+%o0],%f13
	fmuls	%f2,%f1,%f2
	ldf	[%o7+%o0],%f9
	fadds	%f3,%f2,%f2
	ldf	[%o3+28],%f7
	fsubs	%f3,%f0,%f3
	fmuls	%f0,%f2,%f2
	fmuls	%f3,%f5,%f3
	fdivs	%f2,%f4,%f2
	fdivs	%f3,%f6,%f3
	fadds	%f2,%f3,%f2
	ba	0804896C
	stf	%f5,[%o1+%o0]

l080488F0:
	ldf	[%o3+68],%f5
	fmuls	%f7,%f2,%f2
	fmuls	%f7,%f7,%f5
	ldf	[%o3+64],%f7
	ldf	[%o3+72],%f9
	fsubs	%f3,%f2,%f2
	ldf	[%o3+28],%f19
	fmuls	%f5,%f4,%f4
	ldf	[%i0+%o0],%f13
	fsubs	%f9,%f1,%f3
	fadds	%f2,%f4,%f2
	ldf	[%o4+4],%f11
	fmuls	%f2,%f3,%f2
	ldf	[%o4+8],%f9
	fdivs	%f4,%f5,%f4
	ldf	[%o3+76],%f7
	fadds	%f6,%f3,%f3
	fdivs	%f2,%f8,%f2
	ldf	[%o7+%o0],%f11
	fdivs	%f4,%f3,%f3
	ldf	[%lp+%o0],%f13
	fmuls	%f3,%f1,%f3
	ldf	[%o3+12],%f9
	fadds	%f2,%f3,%f2
	fsubs	%f9,%f0,%f3
	fmuls	%f0,%f2,%f2
	fmuls	%f3,%f4,%f3
	fdivs	%f2,%f5,%f2
	fdivs	%f3,%f6,%f3
	fadds	%f2,%f3,%f2
	stf	%f5,[%o1+%o0]

l0804896C:
	ldf	[%o3+84],%f5
	ldf	[%i2+%o0],%f7
	fmuls	%f3,%f2,%f0
	ldf	[%o3+88],%f9
	ldf	[%o3+92],%f11
	fadds	%f0,%f4,%f6
	ldf	[%o3+28],%f7
	fcmpes	%f6,%f5
	sethi	0x00000000,%g0
	ldf	[%o3+180],%f9
	fbu,a	08048738
	ldf	[%i6-140],%f5

l0804899C:
	ldf	[%o3+28],%f5
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	fbuge,a	080489E8
	ldf	[%o3+100],%f5

l080489B0:
	ldf	[%o3+172],%f5
	fmuls	%f0,%f2,%f2
	fadds	%f2,%f3,%f5
	ldf	[%o3+172],%f5
	fmuls	%f4,%f2,%f2
	ldf	[%o3+176],%f7
	stf	%f11,[%i6-76]
	fmuls	%f5,%f5,%f5
	fsubs	%f3,%f2,%f2
	fdivs	%f2,%f5,%f2
	ldf	[%o3+84],%f7
	fmuls	%f2,%f3,%f2
	ba	0804873C
	stf	%f5,[%o5+%o0]

l080489E8:
	ldf	[%o3+96],%f7
	fmuls	%f6,%f3,%f3
	ldf	[%o3+104],%f9
	ldf	[%o3+136],%f11
	fmuls	%f6,%f5,%f5
	fnegs	%f3,%f3
	ldf	[%o3+128],%f1
	fnegs	%f5,%f5
	fadds	%f3,%f2,%f2
	fmuls	%f2,%f6,%f2
	ldf	[%o3+108],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f6,%f2
	ldf	[%o3+112],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f6,%f2
	ldf	[%o3+116],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f6,%f2
	ldf	[%o3+120],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f6,%f2
	ldf	[%o3+124],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f6,%f2
	ldf	[%o3+140],%f9
	fadds	%f5,%f4,%f4
	fadds	%f2,%f3,%f2
	fmuls	%f4,%f6,%f4
	ldf	[%o3+28],%f11
	fmuls	%f2,%f6,%f2
	ldf	[%o3+132],%f7
	fadds	%f2,%f0,%f2
	fmuls	%f2,%f6,%f2
	fadds	%f2,%f3,%f1
	ldf	[%o3+144],%f5
	fadds	%f4,%f2,%f2
	ldf	[%o3+148],%f7
	fmuls	%f2,%f6,%f2
	ldf	[%o3+152],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f6,%f2
	ldf	[%o3+156],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f6,%f2
	ldf	[%o3+160],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f6,%f2
	ldf	[%o3+164],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f6,%f2
	ldf	[%o3+168],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f6,%f4,%f3
	fmuls	%f4,%f1,%f4
	fmuls	%f2,%f6,%f2
	fadds	%f2,%f0,%f2
	fadds	%f3,%f5,%f0
	fmuls	%f2,%f0,%f2
	ldf	[%o3+84],%f11
	fmuls	%f0,%f0,%f3
	fsubs	%f2,%f4,%f2
	fdivs	%f2,%f3,%f2
	fmuls	%f2,%f5,%f2
	ba	0804873C
	stf	%f5,[%o5+%o0]

l08048AF0:
	ldf	[%o3+224],%f5
	fmuls	%f1,%f2,%f2
	ldf	[%o3+228],%f7
	ldf	[%o3+232],%f9
	fadds	%f2,%f3,%f2
	ldf	[%o3+28],%f11
	fmuls	%f2,%f1,%f2
	ldf	[%o3+236],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+240],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+244],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+248],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+252],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+256],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+260],%f7
	fadds	%f2,%f4,%f2
	ldf	[%o3+264],%f9
	fmuls	%f1,%f4,%f4
	fmuls	%f2,%f1,%f2
	fadds	%f4,%f5,%f4
	fadds	%f2,%f3,%f2
	fdivs	%f2,%f4,%f2
	ldf	[%o3+268],%f7
	fadds	%f2,%f3,%f0

l08048B7C:
	ldf	[%o3+280],%f5
	fmuls	%f0,%f2,%f2
	ldf	[%o3+28],%f7
	ldf	[%i2+%o0],%f9
	stf	%f5,[%l0+%o0]
	ldf	[%l5+%o0],%f5
	fsubs	%f3,%f2,%f2
	ldf	[%o3+284],%f11
	fmuls	%f4,%f2,%f1
	ldf	[%o3+288],%f7
	fcmpes	%f1,%f5
	sethi	0x00000000,%g0
	ldf	[%o3+296],%f9
	fbug,a	080491E0
	ldf	[%o3+60],%f5

l08048BB8:
	fmuls	%f1,%f3,%f3
	fmuls	%f1,%f1,%f5
	ldf	[%o3+292],%f5
	fmuls	%f1,%f1,%f0
	fadds	%f3,%f2,%f2
	fmuls	%f5,%f4,%f4
	fmuls	%f0,%f1,%f7
	ldf	[%o3+300],%f7
	fmuls	%f1,%f1,%f6
	fsubs	%f2,%f4,%f2
	ldf	[%o3+28],%f11
	fmuls	%f7,%f3,%f3
	stf	%f15,[%i6-156]
	ldf	[%o3+304],%f9
	fmuls	%f1,%f4,%f4
	fadds	%f2,%f3,%f2
	fadds	%f4,%f5,%f4
	ldf	[%o3+308],%f7
	fmuls	%f6,%f3,%f3
	ldf	[%o3+312],%f11
	fmuls	%f7,%f5,%f5
	fsubs	%f4,%f3,%f3
	fadds	%f3,%f5,%f3
	ldf	[%o3+316],%f9
	fcmpes	%f1,%f4
	sethi	0x00000000,%g0
	fdivs	%f2,%f3,%f2
	stf	%f5,[%i4+%o0]
	fbule,a	08048C58
	ldf	[%i2+%o0],%f13

l08048C30:
	ldf	[%o3+348],%f5
	fadds	%f1,%f2,%f2
	ldf	[%i4+%o0],%f9
	ldf	[%i2+%o0],%f13
	fmuls	%f2,%f2,%f3
	stf	%f5,[%i6-80]
	ldf	[%o3+352],%f5
	fmuls	%f3,%f2,%f2
	fsubs	%f4,%f2,%f2
	stf	%f5,[%i4+%o0]

l08048C58:
	ldf	[%o3+356],%f7
	fmuls	%f6,%f3,%f2
	fmuls	%f6,%f6,%f5
	ldf	[%o3+360],%f9
	stf	%f13,[%i6-160]
	ldf	[%o3+364],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f5,%f3,%f3
	ldf	[%o3+368],%f9
	fadds	%f2,%f3,%f2
	fcmpes	%f2,%f4
	fmovs	%f2,%f0
	fbg,a	08048C90
	fmovs	%f4,%f0

l08048C90:
	ldf	[%o2+0],%f11
	fmuls	%f5,%f0,%f5
	ldf	[%i4+%o0],%f5
	ldf	[%o3-8],%f7
	fadds	%f2,%f5,%f2
	stf	%f5,[%i4+%o0]
	ldf	[%l4+%o0],%f5
	fcmpes	%f2,%f3
	fmovs	%f2,%f0
	fbg,a	08048CBC
	fmovs	%f3,%f0

l08048CBC:
	ldf	[%o3-4],%f9
	fmuls	%f0,%f4,%f4
	ldf	[%o3+0],%f5
	fmuls	%f0,%f0,%f5
	ldf	[%o3+4],%f7
	fsubs	%f2,%f4,%f2
	fmuls	%f0,%f0,%f4
	fmuls	%f5,%f3,%f3
	fmuls	%f4,%f0,%f4
	fsubs	%f2,%f3,%f2
	ldf	[%o3+8],%f11
	fmuls	%f4,%f5,%f4
	ldf	[%o3+12],%f7
	fmuls	%f6,%f3,%f3
	fsubs	%f2,%f4,%f2
	fadds	%f2,%f3,%f2
	ldf	[%o3+16],%f9
	fsubs	%f4,%f0,%f4
	stf	%f5,[%lp+%o0]
	ldf	[%o3+20],%f5
	fcmpes	%f2,%f4
	sethi	0x00000000,%g0
	fbul,a	08048D1C
	fmovs	%f4,%f2

l08048D1C:
	ldf	[%i6-124],%f7
	fmuls	%f2,%f3,%f2
	add	%g1,0x00000001,%g1
	ldf	[%lp+%o0],%f9
	subcc	%g1,%o5,%g0
	add	%o1,0x00000008,%o1
	fsubs	%f4,%f2,%f2
	bg	080486D8
	stf	%f5,[%lp+%o0]

l08048D40:
	ld	[%l2+%o1],%o0

l08048D44:
	sll	%o0,0x00000002,%o0
	ldf	[%i6-132],%f7
	ldf	[%l3+%o0],%f5
	fcmpes	%f2,%f3
	ldf	[%i6-128],%f1
	ldf	[%i2+%o0],%f17
	ldf	[%o3+40],%f5
	fbug,a	08048DC4
	ldf	[%l5+%o0],%f15

l08048D68:
	ldf	[%l7+%o0],%f9
	ldf	[%i4+%o0],%f11
	fdivs	%f4,%f5,%f4
	ldf	[%l3+%o0],%f7
	fadds	%f3,%f2,%f2
	ldf	[%o3+44],%f13
	ldf	[%i6-136],%f7
	fmovs	%f3,%f5
	fadds	%f4,%f3,%f3
	fmuls	%f3,%f2,%f2
	ldf	[%o3+28],%f9
	fmuls	%f2,%f6,%f2
	ldf	[%i6-128],%f7
	fadds	%f2,%f4,%f2
	fcmpes	%f3,%f2
	fmovs	%f3,%f0
	fbul,a	08048DB0
	fmovs	%f2,%f0

l08048DB0:
	fadds	%f0,%f5,%f5
	ldf	[%o3+48],%f5
	ldf	[%l5+%o0],%f15
	fmuls	%f5,%f2,%f2
	fadds	%f2,%f4,%f0

l08048DC4:
	ldf	[%o3+52],%f5
	fmuls	%f7,%f2,%f1
	ldf	[%o3+28],%f7
	fsubs	%f3,%f7,%f3
	ldf	[%o3+56],%f9
	ldf	[%o3+60],%f5
	fmuls	%f3,%f4,%f3
	ldf	[%o3+76],%f11
	fcmpes	%f8,%f2
	fadds	%f1,%f3,%f3
	ldf	[%o3+80],%f13
	fbuge	08048E44
	fdivs	%f1,%f3,%f1

l08048DF8:
	ldf	[%i2+%o0],%f5
	ldf	[%o4+4],%f9
	fmuls	%f8,%f6,%f6
	ldf	[%o4+8],%f7
	fmuls	%f2,%f3,%f2
	fdivs	%f2,%f4,%f2
	ldf	[%i0+%o0],%f7
	fadds	%f3,%f5,%f3
	ldf	[%o3+200],%f9
	fadds	%f6,%f4,%f4
	ldf	[%o3+28],%f11
	fsubs	%f5,%f1,%f5
	fdivs	%f2,%f3,%f2
	fmuls	%f4,%f5,%f4
	fmuls	%f2,%f1,%f2
	fdivs	%f4,%f0,%f4
	fadds	%f4,%f2,%f2
	ba	08048EB8
	stf	%f5,[%o7+%o0]

l08048E44:
	ldf	[%o3+184],%f5
	fmuls	%f8,%f2,%f2
	ldf	[%o3+188],%f7
	fmuls	%f8,%f8,%f5
	ldf	[%o3+192],%f9
	fadds	%f2,%f3,%f2
	ldf	[%o3+76],%f13
	fmuls	%f8,%f8,%f3
	fmuls	%f5,%f4,%f4
	fmuls	%f3,%f8,%f3
	ldf	[%o3+196],%f11
	fsubs	%f2,%f4,%f2
	fmuls	%f3,%f5,%f3
	ldf	[%o3+28],%f9
	fsubs	%f4,%f1,%f4
	fadds	%f2,%f3,%f2
	ldf	[%o4+8],%f11
	fdivs	%f2,%f0,%f2
	ldf	[%i2+%o0],%f7
	fmuls	%f3,%f5,%f3
	fmuls	%f2,%f4,%f2
	ldf	[%i0+%o0],%f11
	fadds	%f5,%f6,%f5
	ldf	[%o4+4],%f9
	fdivs	%f3,%f4,%f3
	fdivs	%f3,%f5,%f3
	fmuls	%f3,%f1,%f3
	fadds	%f2,%f3,%f2
	stf	%f5,[%o7+%o0]

l08048EB8:
	ldf	[%i2+%o0],%f13
	ldf	[%o3+84],%f5
	fmuls	%f6,%f2,%f0
	ldf	[%o3+88],%f7
	ldf	[%o3+92],%f9
	fadds	%f0,%f3,%f1
	fcmpes	%f1,%f4
	sethi	0x00000000,%g0
	ldf	[%o3+180],%f7
	ldf	[%o3+172],%f9
	fbu,a	08048FB8
	ldf	[%o3+208],%f5

l08048EE8:
	ldf	[%o3+28],%f5
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+176],%f5
	fbuge	08048F20
	ldf	[%o3+28],%f11

l08048F00:
	fmuls	%f0,%f2,%f2
	fmuls	%f0,%f4,%f4
	fadds	%f2,%f3,%f2
	fadds	%f4,%f5,%f4
	fdivs	%f2,%f4,%f2
	stf	%f5,[%l1+%o0]
	ba	08048FD4
	ldf	[%l1+%o0],%f5

l08048F20:
	ldf	[%o3+96],%f7
	fmuls	%f1,%f3,%f3
	ldf	[%o3+100],%f5
	ldf	[%o3+104],%f9
	fnegs	%f3,%f3
	fadds	%f3,%f2,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+108],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+112],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+116],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+120],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+124],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+128],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+132],%f7
	fadds	%f2,%f4,%f2
	ldf	[%o3+168],%f9
	fmuls	%f1,%f4,%f4
	fmuls	%f2,%f1,%f2
	fadds	%f4,%f5,%f4
	fadds	%f2,%f3,%f2
	fdivs	%f2,%f4,%f2
	ldf	[%o3+204],%f7
	fadds	%f2,%f3,%f2
	stf	%f5,[%l1+%o0]
	ba	08048FD4
	ldf	[%l1+%o0],%f5

l08048FB8:
	fadds	%f1,%f2,%f2
	ldf	[%o3+212],%f7
	ldf	[%o3+216],%f9
	fmuls	%f2,%f3,%f2
	fadds	%f2,%f4,%f2
	stf	%f5,[%l1+%o0]
	ldf	[%l1+%o0],%f5

l08048FD4:
	ldf	[%o3+20],%f7
	stf	%f7,[%o2+0]
	stf	%f5,[%i3+%o0]
	ldf	[%l5+%o0],%f7
	ldf	[%o3+220],%f5
	fcmpes	%f3,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+56],%f9
	ldf	[%o3+28],%f7
	fbug,a	080491A8
	ldf	[%o3+84],%f5

l08049000:
	fsubs	%f3,%f7,%f3
	ldf	[%o3+52],%f5
	fmuls	%f7,%f2,%f0
	fmuls	%f3,%f4,%f3
	ldf	[%o3+84],%f5
	fadds	%f0,%f3,%f3
	ldf	[%o3+28],%f9
	fdivs	%f0,%f3,%f1
	fmuls	%f6,%f2,%f0
	fsubs	%f4,%f1,%f4
	ldf	[%o3+92],%f7
	ldf	[%o3+88],%f5
	fmuls	%f0,%f4,%f0
	fadds	%f0,%f2,%f1
	ldf	[%o3+172],%f9
	fcmpes	%f1,%f3
	sethi	0x00000000,%g0
	ldf	[%o3+180],%f7
	fbu,a	08049168
	ldf	[%o3+208],%f5

l08049050:
	ldf	[%o3+28],%f5
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+176],%f5
	fbuge	080490AC
	ldf	[%o3+28],%f11

l08049068:
	fmuls	%f0,%f2,%f2
	fmuls	%f0,%f4,%f4
	fadds	%f2,%f3,%f2
	fadds	%f4,%f5,%f4
	ldf	[%i1+%o0],%f7
	fdivs	%f2,%f4,%f2
	ldf	[%o3+20],%f11
	stf	%f5,[%i3+%o0]
	ldf	[%i3+%o0],%f5
	fsubs	%f3,%f2,%f2
	fcmpes	%f2,%f5
	sethi	0x00000000,%g0
	fbul,a	080490A0
	fmovs	%f5,%f2

l080490A0:
	stf	%f5,[%o2+0]
	ba	080491A8
	ldf	[%o3+84],%f5

l080490AC:
	ldf	[%o3+96],%f7
	fmuls	%f1,%f3,%f3
	ldf	[%o3+100],%f5
	ldf	[%o3+104],%f9
	fnegs	%f3,%f3
	fadds	%f3,%f2,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+108],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+112],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+116],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+120],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+124],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+128],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+132],%f7
	fadds	%f2,%f4,%f2
	ldf	[%o3+168],%f9
	fmuls	%f1,%f4,%f4
	fmuls	%f2,%f1,%f2
	fadds	%f4,%f5,%f4
	fadds	%f2,%f3,%f2
	ldf	[%i1+%o0],%f11
	fdivs	%f2,%f4,%f2
	ldf	[%o3+204],%f7
	fadds	%f2,%f3,%f2
	stf	%f5,[%i3+%o0]
	ldf	[%i3+%o0],%f5
	fsubs	%f5,%f2,%f2
	ldf	[%o3+20],%f7
	fcmpes	%f2,%f3
	sethi	0x00000000,%g0
	fbul,a	0804915C
	fmovs	%f3,%f2

l0804915C:
	stf	%f5,[%o2+0]
	ba	080491A8
	ldf	[%o3+84],%f5

l08049168:
	fadds	%f1,%f2,%f2
	ldf	[%o3+212],%f7
	ldf	[%o3+216],%f9
	fmuls	%f2,%f3,%f2
	fadds	%f2,%f4,%f2
	ldf	[%i1+%o0],%f7
	stf	%f5,[%i3+%o0]
	ldf	[%i3+%o0],%f5
	fsubs	%f3,%f2,%f2
	ldf	[%o3+20],%f9
	fcmpes	%f2,%f4
	sethi	0x00000000,%g0
	fbul,a	080491A0
	fmovs	%f4,%f2

l080491A0:
	stf	%f5,[%o2+0]
	ldf	[%o3+84],%f5

l080491A8:
	fmuls	%f6,%f2,%f0
	ldf	[%o3+28],%f9
	fcmpes	%f0,%f4
	ldf	[%o3+88],%f7
	ldf	[%o3+276],%f5
	ldf	[%o3+272],%f9
	fbuge	08048AF0
	fadds	%f0,%f3,%f1

l080491C8:
	fmuls	%f0,%f2,%f2
	fmuls	%f0,%f4,%f4
	ldf	[%o3+28],%f7
	fadds	%f2,%f3,%f2
	ba	08048B7C
	fdivs	%f4,%f2,%f0

l080491E0:
	fcmpes	%f1,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+324],%f7
	fbug,a	0804920C
	ldf	[%o3+328],%f5

l080491F4:
	ldf	[%o3+320],%f5
	fmuls	%f1,%f2,%f2
	fadds	%f2,%f3,%f2
	stf	%f5,[%i4+%o0]
	ba	08048C58
	ldf	[%i2+%o0],%f13

l0804920C:
	fcmpes	%f1,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+336],%f7
	fbug,a	08049238
	ldf	[%o3+340],%f5

l08049220:
	ldf	[%o3+332],%f5
	fmuls	%f1,%f2,%f2
	fadds	%f2,%f3,%f2
	stf	%f5,[%i4+%o0]
	ba	08048C58
	ldf	[%i2+%o0],%f13

l08049238:
	fmuls	%f1,%f2,%f2
	ldf	[%o3+344],%f7
	fadds	%f2,%f3,%f2
	stf	%f5,[%i4+%o0]
	ba	08048C58
	ldf	[%i2+%o0],%f13

l08049250:
	ld	[%sp+104],%o0
	ldf	[%o3+28],%f5
	ld	[%o0+0],%o5
	ld	[%sp+108],%o0
	ldf	[%o3+372],%f7
	ld	[%o0+0],%o1
	subcc	%o5,%o1,%g0
	bl	080486D8
	or	%g0,%o1,%l0

l08049274:
	subcc	%o1,%o5,%g0
	bg	080486D8
	sll	%o1,0x00000003,%o1

l08049280:
	stf	%f5,[%i6-108]
	stf	%f7,[%i6-112]
	ldf	[%o3+376],%f5
	ldf	[%o3+380],%f7
	ld	[%sp+100],%o0
	stf	%f5,[%i6-116]
	add	%o0,0x00000008,%o4
	stf	%f7,[%i6-120]
	ba	08049480
	ld	[%o4+%o1],%o0

l080492A8:
	ldf	[%l4+%o0],%f3
	ldf	[%o3+412],%f5
	fmuls	%f1,%f2,%f2
	ldf	[%o3+416],%f7
	ldf	[%i2+%o0],%f11
	fadds	%f2,%f3,%f6
	ldf	[%o3+84],%f9
	fmuls	%f5,%f4,%f4
	ldf	[%o3+424],%f7
	fmuls	%f6,%f3,%f3
	ldf	[%o3+428],%f11
	ldf	[%o3+420],%f5
	fadds	%f4,%f2,%f0
	fadds	%f3,%f5,%f3
	ldf	[%o3+432],%f5
	fmuls	%f3,%f6,%f3
	ldf	[%o3+436],%f9
	fmuls	%f6,%f4,%f4
	ldf	[%o3+444],%f11
	fadds	%f3,%f2,%f2
	ldf	[%o3+440],%f7
	fadds	%f4,%f3,%f3
	fmuls	%f2,%f0,%f2
	fmuls	%f3,%f6,%f3
	ldf	[%o3+448],%f9
	fadds	%f3,%f5,%f3
	fmuls	%f3,%f6,%f3
	ldf	[%o3+452],%f11
	fmuls	%f0,%f5,%f5
	fadds	%f2,%f3,%f2
	ldf	[%o3+456],%f7
	fmuls	%f6,%f3,%f3
	fadds	%f2,%f4,%f2
	fadds	%f5,%f3,%f3
	ldf	[%o3+28],%f9
	fadds	%f3,%f4,%f3
	ldf	[%o3+460],%f11
	fdivs	%f2,%f3,%f2
	ldf	[%i6-116],%f9
	fcmpes	%f1,%f4
	fadds	%f2,%f5,%f0
	ldf	[%o3+408],%f9
	fbule,a	0804937C
	stf	%f1,[%i1+%o0]

l08049358:
	ldf	[%l4+%o0],%f7
	ldf	[%o3+24],%f5
	fmuls	%f3,%f2,%f2
	fadds	%f2,%f4,%f2
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	fbul,a	08049378
	fmovs	%f2,%f0

l08049378:
	stf	%f1,[%i1+%o0]

l0804937C:
	ldf	[%i2+%o0],%f1

l08049380:
	ldf	[%o3+356],%f5
	fmuls	%f0,%f2,%f2
	fmuls	%f0,%f0,%f5
	ldf	[%o3+360],%f7
	ldf	[%o3+364],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f5,%f4,%f4
	ldf	[%o3+368],%f7
	fadds	%f2,%f4,%f2
	fcmpes	%f2,%f3
	fmovs	%f2,%f0
	fbg,a	080493B4
	fmovs	%f3,%f0

l080493B4:
	ldf	[%l3+%o0],%f7
	ldf	[%o3+32],%f5
	fcmpes	%f3,%f2
	ldf	[%o3+464],%f19
	fbuge,a	080493F0
	ldf	[%l5+%o0],%f17

l080493CC:
	ldf	[%l3+%o0],%f5
	ldf	[%o7+%o0],%f7
	fmuls	%f2,%f3,%f2
	ldf	[%o3+468],%f9
	fcmpes	%f2,%f4
	sethi	0x00000000,%g0
	fbug,a	0804954C
	ldf	[%l5+%o0],%f3

l080493EC:
	ldf	[%l5+%o0],%f17

l080493F0:
	ldf	[%i3+%o0],%f15
	fmuls	%f7,%f0,%f3
	ldf	[%i4+%o0],%f13
	fmuls	%f8,%f9,%f4
	ldf	[%i6-108],%f5
	fsubs	%f2,%f8,%f1
	fsubs	%f3,%f6,%f3
	fmuls	%f1,%f0,%f5
	ldf	[%g1+%o0],%f5
	fmuls	%f1,%f3,%f3
	fadds	%f4,%f5,%f4
	fadds	%f2,%f3,%f2
	fdivs	%f2,%f4,%f2
	ldf	[%i6-108],%f7
	fsubs	%f3,%f8,%f1
	stf	%f5,[%i0+%o0]
	ldf	[%l1+%o0],%f5
	fmuls	%f1,%f2,%f2
	ldf	[%i0+%o0],%f11
	fcmpes	%f2,%f5
	sethi	0x00000000,%g0
	fbul,a	0804944C
	fmovs	%f5,%f2

l0804944C:
	stf	%f5,[%i0+%o0]
	ldf	[%i0+%o0],%f3
	fsubs	%f1,%f7,%f3
	fmuls	%f1,%f9,%f2
	fmuls	%f3,%f0,%f3
	stf	%f5,[%l2+%o0]
	fadds	%f6,%f3,%f3
	stf	%f7,[%l7+%o0]

l0804946C:
	add	%l0,0x00000001,%l0
	subcc	%l0,%o5,%g0
	bg	080486D8
	add	%o1,0x00000008,%o1

l0804947C:
	ld	[%o4+%o1],%o0

l08049480:
	subcc	%o2,0x00000002,%g0
	be	080492A8
	sll	%o0,0x00000002,%o0

l0804948C:
	ldf	[%i6-112],%f5
	ldf	[%l4+%o0],%f1
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+408],%f7
	fbug,a	0804953C
	ldf	[%o3+404],%f5

l080494A8:
	ldf	[%i6-116],%f5
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	fbule,a	080494D4
	ldf	[%i6-120],%f5

l080494BC:
	ldf	[%o3+24],%f5
	fmuls	%f0,%f2,%f2
	fadds	%f2,%f3,%f2
	stf	%f5,[%i1+%o0]
	ba	08049380
	ldf	[%i2+%o0],%f1

l080494D4:
	fmuls	%f0,%f2,%f2
	ldf	[%o3+204],%f7
	fmuls	%f0,%f0,%f5
	ldf	[%o3+384],%f9
	fmuls	%f0,%f0,%f6
	fadds	%f2,%f3,%f2
	fmuls	%f0,%f0,%f3
	fmuls	%f5,%f4,%f4
	fmuls	%f3,%f0,%f3
	ldf	[%o3+388],%f11
	fadds	%f2,%f4,%f2
	fmuls	%f3,%f5,%f3
	ldf	[%o3+392],%f9
	fmuls	%f0,%f4,%f4
	fsubs	%f2,%f3,%f2
	ldf	[%o3+28],%f11
	fadds	%f4,%f5,%f4
	ldf	[%o3+396],%f7
	fmuls	%f6,%f3,%f3
	ldf	[%o3+400],%f11
	fsubs	%f4,%f3,%f3
	fdivs	%f2,%f3,%f2
	fadds	%f2,%f5,%f2
	stf	%f5,[%i1+%o0]
	ba	08049380
	ldf	[%i2+%o0],%f1

l0804953C:
	fdivs	%f0,%f2,%f2
	stf	%f5,[%i1+%o0]
	ba	08049380
	ldf	[%i2+%o0],%f1

l0804954C:
	ldf	[%i6-108],%f7
	fsubs	%f3,%f1,%f3
	ldf	[%i3+%o0],%f9
	ldf	[%i1+%o0],%f5
	fmuls	%f1,%f2,%f2
	fmuls	%f3,%f4,%f3
	fadds	%f2,%f3,%f2
	stf	%f5,[%i0+%o0]
	ldf	[%i0+%o0],%f3
	fsubs	%f1,%f4,%f3
	fmuls	%f1,%f9,%f2
	fmuls	%f3,%f0,%f3
	stf	%f5,[%l2+%o0]
	ldf	[%i4+%o0],%f5
	fadds	%f2,%f3,%f2
	ba	0804946C
	stf	%f5,[%l7+%o0]

l08049590:
	ld	[%sp+100],%o0
	ldf	[%o3+84],%f7
	add	%o0,0x00000008,%o0
	st	%o0,[%sp+96]
	stf	%f7,[%i6-96]
	stf	%f5,[%i6-92]
	ld	[%sp+112],%o0
	ldf	[%o3+28],%f7
	sll	%o0,0x00000003,%o1
	ldf	[%o3+88],%f5
	stf	%f7,[%i6-104]
	stf	%f5,[%i6-100]
	ba	08049654
	ld	[%sp+96],%o0

l080495C8:
	ldf	[%o3+184],%f5
	fmuls	%f7,%f2,%f2
	ldf	[%o3+188],%f7
	fmuls	%f7,%f7,%f5
	ldf	[%o3+192],%f9
	fadds	%f2,%f3,%f2
	ldf	[%o3+76],%f13
	fmuls	%f7,%f7,%f3
	fmuls	%f5,%f4,%f4
	fmuls	%f3,%f7,%f3
	ldf	[%o3+196],%f11
	fsubs	%f2,%f4,%f2
	fmuls	%f3,%f5,%f3
	ldf	[%i6-104],%f9
	fsubs	%f4,%f0,%f4
	fadds	%f2,%f3,%f2
	ldf	[%o4+8],%f11
	fdivs	%f2,%f8,%f2
	ldf	[%i2+%o0],%f7
	fmuls	%f3,%f5,%f3
	fmuls	%f2,%f4,%f2
	ldf	[%i0+%o0],%f11
	fadds	%f5,%f6,%f5
	ldf	[%o4+4],%f9
	fdivs	%f3,%f4,%f3
	fdivs	%f3,%f5,%f3
	fmuls	%f3,%f0,%f3
	fadds	%f2,%f3,%f2
	stf	%f5,[%o7+%o0]

l0804963C:
	ld	[%sp+92],%o0
	add	%i5,0x00000001,%i5
	subcc	%i5,%o0,%g0
	bg	080486D8
	add	%o1,0x00000008,%o1

l08049650:
	ld	[%sp+96],%o0

fn08049654()
	ld	[%o0+%o1],%o0
	sll	%o0,0x00000002,%o0
	ldf	[%i6-96],%f5
	ldf	[%i2+%o0],%f1
	fmuls	%f0,%f2,%f0
	ldf	[%i6-104],%f9
	fcmpes	%f0,%f4
	ldf	[%i6-100],%f7
	ldf	[%o3+276],%f5
	ldf	[%o3+272],%f9
	fbuge	0804969C
	fadds	%f0,%f3,%f1

fn08049684()
	fmuls	%f0,%f2,%f2
	fmuls	%f0,%f4,%f4
	ldf	[%i6-104],%f7
	fadds	%f2,%f3,%f2
	ba	08049728
	fdivs	%f4,%f2,%f0

fn0804969C()
	ldf	[%o3+224],%f5
	fmuls	%f1,%f2,%f2
	ldf	[%o3+228],%f7
	ldf	[%o3+232],%f9
	fadds	%f2,%f3,%f2
	ldf	[%o3+28],%f11
	fmuls	%f2,%f1,%f2
	ldf	[%o3+236],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+240],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+244],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+248],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+252],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+256],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+260],%f7
	fadds	%f2,%f4,%f2
	ldf	[%o3+264],%f9
	fmuls	%f1,%f4,%f4
	fmuls	%f2,%f1,%f2
	fadds	%f4,%f5,%f4
	fadds	%f2,%f3,%f2
	fdivs	%f2,%f4,%f2
	ldf	[%o3+268],%f7
	fadds	%f2,%f3,%f0

l08049728:
	ldf	[%o3+280],%f5
	fmuls	%f0,%f2,%f2
	ldf	[%o3+472],%f7
	ldf	[%l4+%o0],%f9
	stf	%f5,[%l0+%o0]
	ldf	[%l0+%o0],%f5
	fmuls	%f2,%f3,%f2
	fcmpes	%f4,%f2
	sethi	0x00000000,%g0
	fbule,a	08049760
	ldf	[%i2+%o0],%f3

l08049754:
	ldf	[%l0+%o0],%f5
	ldf	[%i2+%o0],%f3
	stf	%f5,[%l4+%o0]

l08049760:
	ldf	[%l4+%o0],%f7

fn08049764()
	ldf	[%o3-8],%f5
	fcmpes	%f3,%f2
	fmovs	%f3,%f0
	fbg,a	08049778
	fmovs	%f2,%f0

l08049778:
	ldf	[%o3-4],%f9
	fmuls	%f0,%f4,%f4
	ldf	[%o3+0],%f5
	fmuls	%f0,%f0,%f5
	ldf	[%o3+4],%f7
	fsubs	%f2,%f4,%f2
	fmuls	%f0,%f0,%f4
	fmuls	%f5,%f3,%f3
	fmuls	%f4,%f0,%f4
	fsubs	%f2,%f3,%f2
	ldf	[%o3+8],%f11
	fmuls	%f4,%f5,%f4
	ldf	[%o3+12],%f7
	fmuls	%f1,%f3,%f3
	fsubs	%f2,%f4,%f2
	fadds	%f2,%f3,%f2
	ldf	[%o3+16],%f9
	fsubs	%f4,%f0,%f4
	stf	%f5,[%lp+%o0]
	ldf	[%o3+20],%f5
	fcmpes	%f2,%f4
	sethi	0x00000000,%g0
	fbul,a	080497D8
	fmovs	%f4,%f2

l080497D8:
	ldf	[%o3+24],%f7
	fmuls	%f2,%f3,%f2
	ldf	[%lp+%o0],%f9
	fsubs	%f4,%f2,%f2
	ldf	[%i6-96],%f7
	fmuls	%f1,%f3,%f0
	stf	%f5,[%lp+%o0]
	ldf	[%i6-100],%f5
	fadds	%f0,%f2,%f1
	ldf	[%o3+92],%f9
	fcmpes	%f1,%f4
	sethi	0x00000000,%g0
	fbu,a	080498E4
	ldf	[%o3+208],%f5

l08049810:
	ldf	[%i6-104],%f5
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+172],%f9
	ldf	[%o3+180],%f7
	ldf	[%i6-104],%f11
	fbuge,a	08049850
	ldf	[%o3+100],%f5

l08049830:
	fmuls	%f0,%f4,%f4
	ldf	[%o3+176],%f5
	fmuls	%f0,%f2,%f2
	fadds	%f4,%f5,%f4
	fadds	%f2,%f3,%f2
	fdivs	%f2,%f4,%f2
	ba	080498FC
	stf	%f5,[%l1+%o0]

l08049850:
	ldf	[%o3+96],%f7
	fmuls	%f1,%f3,%f3
	ldf	[%o3+104],%f9
	ldf	[%o3+28],%f11
	fnegs	%f3,%f3
	fadds	%f3,%f2,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+108],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+112],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+116],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+120],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+124],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+128],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+132],%f7
	fadds	%f2,%f4,%f2
	ldf	[%o3+168],%f9
	fmuls	%f1,%f4,%f4
	fmuls	%f2,%f1,%f2
	fadds	%f4,%f5,%f4
	fadds	%f2,%f3,%f2
	fdivs	%f2,%f4,%f2
	ldf	[%o3+204],%f7
	fadds	%f2,%f3,%f2
	ba	080498FC
	stf	%f5,[%l1+%o0]

l080498E4:
	fadds	%f1,%f2,%f2
	ldf	[%o3+212],%f7
	ldf	[%o3+216],%f9
	fmuls	%f2,%f3,%f2
	fadds	%f2,%f4,%f2
	stf	%f5,[%l1+%o0]

l080498FC:
	subcc	%o2,0x00000002,%g0
	be,a	080499F0
	ldf	[%l4+%o0],%f3

l08049908:
	ldf	[%l0+%o0],%f7
	ldf	[%o3+472],%f5
	fmuls	%f3,%f2,%f2
	ldf	[%l4+%o0],%f9
	fcmpes	%f4,%f2
	sethi	0x00000000,%g0
	fbule,a	08049934
	ldf	[%l4+%o0],%f1

l08049928:
	ldf	[%l0+%o0],%f5
	stf	%f5,[%l4+%o0]
	ldf	[%l4+%o0],%f1

l08049934:
	ldf	[%o3+372],%f5
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+408],%f7
	fbug,a	080499E0
	ldf	[%o3+404],%f5

l0804994C:
	ldf	[%o3+376],%f5
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	fbule,a	08049978
	ldf	[%o3+380],%f5

l08049960:
	ldf	[%o3+24],%f5
	fmuls	%f0,%f2,%f2
	fadds	%f2,%f3,%f2
	stf	%f5,[%i1+%o0]
	ba	08049AC4
	ldf	[%i2+%o0],%f11

l08049978:
	fmuls	%f0,%f2,%f2
	ldf	[%o3+204],%f7
	fmuls	%f0,%f0,%f5
	ldf	[%o3+384],%f9
	fmuls	%f0,%f0,%f6
	fadds	%f2,%f3,%f2
	fmuls	%f0,%f0,%f3
	fmuls	%f5,%f4,%f4
	fmuls	%f3,%f0,%f3
	ldf	[%o3+388],%f11
	fadds	%f2,%f4,%f2
	fmuls	%f3,%f5,%f3
	ldf	[%o3+392],%f9
	fmuls	%f0,%f4,%f4
	fsubs	%f2,%f3,%f2
	ldf	[%o3+28],%f11
	fadds	%f4,%f5,%f4
	ldf	[%o3+396],%f7
	fmuls	%f6,%f3,%f3
	ldf	[%o3+400],%f11
	fsubs	%f4,%f3,%f3
	fdivs	%f2,%f3,%f2
	fadds	%f2,%f5,%f2
	stf	%f5,[%i1+%o0]
	ba	08049AC4
	ldf	[%i2+%o0],%f11

l080499E0:
	fdivs	%f0,%f2,%f2
	stf	%f5,[%i1+%o0]
	ba	08049AC4
	ldf	[%i2+%o0],%f11

l080499F0:
	ldf	[%o3+412],%f5
	fmuls	%f1,%f2,%f2
	ldf	[%o3+416],%f7
	ldf	[%i2+%o0],%f9
	fadds	%f2,%f3,%f6
	ldf	[%i6-96],%f11
	fmuls	%f4,%f5,%f4
	ldf	[%o3+424],%f7
	fmuls	%f6,%f3,%f3
	ldf	[%o3+428],%f11
	ldf	[%o3+420],%f5
	fadds	%f4,%f2,%f0
	fadds	%f3,%f5,%f3
	ldf	[%o3+432],%f5
	fmuls	%f3,%f6,%f3
	ldf	[%o3+436],%f9
	fmuls	%f6,%f4,%f4
	ldf	[%o3+444],%f11
	fadds	%f3,%f2,%f2
	ldf	[%o3+440],%f7
	fadds	%f4,%f3,%f3
	fmuls	%f2,%f0,%f2
	fmuls	%f3,%f6,%f3
	ldf	[%o3+448],%f9
	fadds	%f3,%f5,%f3
	fmuls	%f3,%f6,%f3
	ldf	[%o3+452],%f11
	fmuls	%f0,%f5,%f5
	fadds	%f2,%f3,%f2
	ldf	[%o3+456],%f7
	fmuls	%f6,%f3,%f3
	fadds	%f2,%f4,%f2
	fadds	%f5,%f3,%f3
	ldf	[%o3+28],%f9
	fadds	%f3,%f4,%f3
	ldf	[%o3+460],%f11
	fdivs	%f2,%f3,%f2
	ldf	[%o3+376],%f9
	fcmpes	%f1,%f4
	fadds	%f2,%f5,%f0
	ldf	[%o3+408],%f9
	fbule,a	08049AC0
	stf	%f1,[%i1+%o0]

l08049A9C:
	ldf	[%l4+%o0],%f7
	ldf	[%o3+24],%f5
	fmuls	%f3,%f2,%f2
	fadds	%f2,%f4,%f2
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	fbul,a	08049ABC
	fmovs	%f2,%f0

l08049ABC:
	stf	%f1,[%i1+%o0]

l08049AC0:
	ldf	[%i2+%o0],%f11

l08049AC4:
	ldf	[%l1+%o0],%f5
	ldf	[%o3+20],%f7
	stf	%f5,[%i3+%o0]
	stf	%f7,[%o5+0]
	ldf	[%o3+220],%f5
	ldf	[%l5+%o0],%f7
	fcmpes	%f3,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+56],%f9
	ldf	[%i6-104],%f7
	fbug,a	08049CA4
	ldf	[%l5+%o0],%f5

l08049AF4:
	ldf	[%l5+%o0],%f3
	fsubs	%f3,%f1,%f3
	ldf	[%o3+52],%f5
	fmuls	%f1,%f2,%f0
	fmuls	%f3,%f4,%f3
	ldf	[%i6-96],%f5
	fmuls	%f5,%f2,%f2
	fadds	%f0,%f3,%f3
	ldf	[%i6-104],%f9
	ldf	[%i6-100],%f11
	fdivs	%f0,%f3,%f0
	fsubs	%f4,%f0,%f4
	ldf	[%o3+92],%f7
	fmuls	%f2,%f4,%f0
	fadds	%f0,%f5,%f1
	ldf	[%o3+172],%f9
	fcmpes	%f1,%f3
	sethi	0x00000000,%g0
	ldf	[%i6-104],%f11
	ldf	[%o3+180],%f7
	fbu,a	08049C64
	ldf	[%o3+208],%f5

l08049B4C:
	ldf	[%i6-104],%f5
	fcmpes	%f0,%f2
	sethi	0x00000000,%g0
	fbuge,a	08049BA8
	ldf	[%o3+100],%f5

l08049B60:
	fmuls	%f0,%f4,%f4
	ldf	[%o3+176],%f5
	fmuls	%f0,%f2,%f2
	fadds	%f4,%f5,%f4
	fadds	%f2,%f3,%f2
	ldf	[%o3+20],%f11
	fdivs	%f2,%f4,%f2
	ldf	[%i1+%o0],%f7
	stf	%f5,[%i3+%o0]
	ldf	[%i3+%o0],%f5
	fsubs	%f3,%f2,%f2
	fcmpes	%f2,%f5
	sethi	0x00000000,%g0
	fbul,a	08049B9C
	fmovs	%f5,%f2

l08049B9C:
	stf	%f5,[%o5+0]
	ba	08049CA4
	ldf	[%l5+%o0],%f5

l08049BA8:
	ldf	[%o3+96],%f7
	fmuls	%f1,%f3,%f3
	ldf	[%o3+104],%f9
	ldf	[%o3+28],%f11
	fnegs	%f3,%f3
	fadds	%f3,%f2,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+108],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+112],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+116],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+120],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+124],%f7
	fadds	%f2,%f4,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+128],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f2,%f1,%f2
	ldf	[%o3+132],%f7
	fadds	%f2,%f4,%f2
	ldf	[%o3+168],%f9
	fmuls	%f1,%f4,%f4
	fmuls	%f2,%f1,%f2
	fadds	%f4,%f5,%f4
	fadds	%f2,%f3,%f2
	ldf	[%i1+%o0],%f11
	fdivs	%f2,%f4,%f2
	ldf	[%o3+204],%f7
	fadds	%f2,%f3,%f2
	stf	%f5,[%i3+%o0]
	ldf	[%i3+%o0],%f5
	fsubs	%f5,%f2,%f2
	ldf	[%o3+20],%f7
	fcmpes	%f2,%f3
	sethi	0x00000000,%g0
	fbul,a	08049C58
	fmovs	%f3,%f2

l08049C58:
	stf	%f5,[%o5+0]
	ba	08049CA4
	ldf	[%l5+%o0],%f5

l08049C64:
	fadds	%f1,%f2,%f2
	ldf	[%o3+212],%f7
	ldf	[%o3+216],%f9
	fmuls	%f2,%f3,%f2
	fadds	%f2,%f4,%f2
	ldf	[%i1+%o0],%f7
	stf	%f5,[%i3+%o0]
	ldf	[%i3+%o0],%f5
	fsubs	%f3,%f2,%f2
	ldf	[%o3+20],%f9
	fcmpes	%f2,%f4
	sethi	0x00000000,%g0
	fbul,a	08049C9C
	fmovs	%f4,%f2

l08049C9C:
	stf	%f5,[%o5+0]
	ldf	[%l5+%o0],%f5

l08049CA4:
	ldf	[%i6-104],%f7
	fsubs	%f3,%f2,%f2
	ldf	[%i2+%o0],%f9
	ldf	[%o3+284],%f11
	fmuls	%f4,%f2,%f1
	ldf	[%o3+288],%f7
	fcmpes	%f1,%f5
	sethi	0x00000000,%g0
	ldf	[%o3+296],%f9
	fbug,a	08049F5C
	ldf	[%o3+60],%f5

l08049CD0:
	fmuls	%f1,%f3,%f3
	fmuls	%f1,%f1,%f5
	ldf	[%o3+292],%f5
	fmuls	%f1,%f1,%f0
	fadds	%f3,%f2,%f2
	fmuls	%f5,%f4,%f4
	fmuls	%f0,%f1,%f7
	ldf	[%o3+300],%f7
	fmuls	%f1,%f1,%f6
	fsubs	%f2,%f4,%f2
	ldf	[%o3+28],%f11
	fmuls	%f7,%f3,%f3
	stf	%f15,[%i6-156]
	ldf	[%o3+304],%f9
	fmuls	%f1,%f4,%f4
	fadds	%f2,%f3,%f2
	fadds	%f4,%f5,%f4
	ldf	[%o3+308],%f7
	fmuls	%f6,%f3,%f3
	ldf	[%o3+312],%f11
	fmuls	%f7,%f5,%f5
	fsubs	%f4,%f3,%f3
	fadds	%f3,%f5,%f3
	ldf	[%o3+316],%f9
	fcmpes	%f1,%f4
	sethi	0x00000000,%g0
	fdivs	%f2,%f3,%f2
	stf	%f5,[%i4+%o0]
	fbule,a	08049D70
	ldf	[%i2+%o0],%f1

l08049D48:
	ldf	[%o3+348],%f5
	fadds	%f1,%f2,%f2
	ldf	[%i4+%o0],%f9
	ldf	[%i2+%o0],%f1
	fmuls	%f2,%f2,%f3
	stf	%f5,[%i6-84]
	ldf	[%o3+352],%f5
	fmuls	%f3,%f2,%f2
	fsubs	%f4,%f2,%f2
	stf	%f5,[%i4+%o0]

l08049D70:
	ldf	[%o3+356],%f5
	fmuls	%f0,%f2,%f2
	fmuls	%f0,%f0,%f5
	ldf	[%o3+360],%f7
	ldf	[%o3+364],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f5,%f4,%f4
	ldf	[%o3+368],%f7
	fadds	%f2,%f4,%f2
	fcmpes	%f2,%f3
	fmovs	%f2,%f0
	fbg,a	08049DA4
	fmovs	%f3,%f0

l08049DA4:
	ldf	[%o5+0],%f11
	fmuls	%f5,%f0,%f5
	ldf	[%i4+%o0],%f5
	ldf	[%o3+280],%f7
	fadds	%f2,%f5,%f2
	stf	%f5,[%i4+%o0]
	ldf	[%l7+%o0],%f5
	fcmpes	%f2,%f3
	sethi	0x00000000,%g0
	fbuge,a	08049DDC
	ldf	[%o3+280],%f5

l08049DD0:
	ldf	[%i4+%o0],%f5
	stf	%f5,[%l7+%o0]
	ldf	[%o3+280],%f5

l08049DDC:
	ldf	[%l7+%o0],%f7
	fcmpes	%f3,%f2
	sethi	0x00000000,%g0
	fbuge,a	08049DFC
	ldf	[%l5+%o0],%f5

l08049DF0:
	ldf	[%g1+%o0],%f5
	stf	%f5,[%l7+%o0]
	ldf	[%l5+%o0],%f5

l08049DFC:
	ldf	[%i6-104],%f7
	fsubs	%f3,%f2,%f2
	ldf	[%o3+476],%f11
	fmuls	%f2,%f3,%f0
	ldf	[%i4+%o0],%f7
	fmuls	%f3,%f5,%f3
	ldf	[%l7+%o0],%f5
	fmuls	%f3,%f0,%f3
	fcmpes	%f2,%f3
	sethi	0x00000000,%g0
	fbul,a	08049E2C
	fmovs	%f3,%f2

l08049E2C:
	stf	%f5,[%l7+%o0]
	ldf	[%o3+356],%f5
	ldf	[%i2+%o0],%f1
	fmuls	%f0,%f2,%f2
	fmuls	%f0,%f0,%f5
	ldf	[%o3+360],%f7
	ldf	[%o3+364],%f9
	fadds	%f2,%f3,%f2
	fmuls	%f5,%f4,%f4
	ldf	[%o3+368],%f7
	fadds	%f2,%f4,%f2
	fcmpes	%f2,%f3
	fmovs	%f2,%f0
	fbg,a	08049E68
	fmovs	%f3,%f0

l08049E68:
	ldf	[%o3+32],%f5
	ldf	[%l3+%o0],%f7
	fcmpes	%f3,%f2
	ldf	[%o3+464],%f13
	fbuge,a	08049EA4
	ldf	[%o3+280],%f5

l08049E80:
	ldf	[%l3+%o0],%f5
	ldf	[%o7+%o0],%f7
	fmuls	%f2,%f3,%f2
	ldf	[%o3+468],%f9
	fcmpes	%f2,%f4
	sethi	0x00000000,%g0
	ldf	[%o3+280],%f5
	fbug,a	0804A10C
	ldf	[%l5+%o0],%f3

l08049EA4:
	ldf	[%g1+%o0],%f7
	fcmpes	%f3,%f2
	sethi	0x00000000,%g0
	fbug,a	08049FCC
	ldf	[%l7+%o0],%f5

l08049EB8:
	ldf	[%o3+280],%f5
	ldf	[%l2+%o0],%f7
	fcmpes	%f3,%f2
	sethi	0x00000000,%g0
	fbug,a	08049FCC
	ldf	[%l7+%o0],%f5

l08049ED0:
	ldf	[%i3+%o0],%f7
	fmuls	%f3,%f0,%f3
	ldf	[%i4+%o0],%f9
	ldf	[%i6-104],%f5
	fsubs	%f3,%f4,%f3
	ldf	[%l5+%o0],%f11
	fmuls	%f5,%f6,%f4
	fsubs	%f2,%f5,%f1
	fmuls	%f1,%f0,%f5
	ldf	[%g1+%o0],%f5
	fmuls	%f1,%f3,%f3
	fadds	%f4,%f5,%f4
	fadds	%f2,%f3,%f2
	ldf	[%o3+480],%f11
	fdivs	%f2,%f4,%f1
	ldf	[%i0+%o0],%f7
	fsubs	%f1,%f3,%f3
	stf	%f7,[%i6-88]
	fabss	%f3,%f3
	fcmpes	%f3,%f5
	sethi	0x00000000,%g0
	fbu,a	08049FCC
	ldf	[%l7+%o0],%f5

l08049F2C:
	stf	%f3,[%i0+%o0]
	ldf	[%i0+%o0],%f3
	fmuls	%f1,%f6,%f2
	ldf	[%i3+%o0],%f7
	fsubs	%f1,%f3,%f3
	stf	%f5,[%l2+%o0]
	fmuls	%f3,%f0,%f3
	ldf	[%i4+%o0],%f5
	fadds	%f2,%f3,%f2
	stf	%f5,[%l7+%o0]
	ba	08049FE8
	ldf	[%i0+%o0],%f5

l08049F5C:
	fcmpes	%f1,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+324],%f7
	fbug,a	08049F88
	ldf	[%o3+328],%f5

l08049F70:
	ldf	[%o3+320],%f5
	fmuls	%f1,%f2,%f2
	fadds	%f2,%f3,%f2
	stf	%f5,[%i4+%o0]
	ba	08049D70
	ldf	[%i2+%o0],%f1

l08049F88:
	fcmpes	%f1,%f2
	sethi	0x00000000,%g0
	ldf	[%o3+336],%f7
	fbug,a	08049FB4
	ldf	[%o3+340],%f5

l08049F9C:
	ldf	[%o3+332],%f5
	fmuls	%f1,%f2,%f2
	fadds	%f2,%f3,%f2
	stf	%f5,[%i4+%o0]
	ba	08049D70
	ldf	[%i2+%o0],%f1

l08049FB4:
	fmuls	%f1,%f2,%f2
	ldf	[%o3+344],%f7
	fadds	%f2,%f3,%f2
	stf	%f5,[%i4+%o0]
	ba	08049D70
	ldf	[%i2+%o0],%f1

l08049FCC:
	ldf	[%i4+%o0],%f7
	fsubs	%f2,%f3,%f1
	ldf	[%i3+%o0],%f9
	fdivs	%f1,%f0,%f5
	fadds	%f4,%f5,%f4
	stf	%f9,[%i0+%o0]
	ldf	[%i0+%o0],%f5

l08049FE8:
	fmuls	%f2,%f6,%f2
	ldf	[%l5+%o0],%f1
	ldf	[%l7+%o0],%f7
	ldf	[%i2+%o0],%f15
	stf	%f5,[%l2+%o0]
	ldf	[%i6-104],%f5
	fsubs	%f2,%f0,%f2
	ldf	[%l2+%o0],%f9
	fmuls	%f0,%f4,%f4
	ldf	[%o3+40],%f11
	fmuls	%f2,%f3,%f2
	fadds	%f4,%f2,%f2
	ldf	[%l3+%o0],%f7
	stf	%f5,[%g1+%o0]
	ldf	[%o3+32],%f5
	fcmpes	%f3,%f2
	ldf	[%o3+28],%f17
	ldf	[%o3+36],%f9
	fbug,a	0804A08C
	ldf	[%l5+%o0],%f3

l0804A038:
	ldf	[%l7+%o0],%f5
	ldf	[%i4+%o0],%f7
	fdivs	%f2,%f3,%f2
	ldf	[%l3+%o0],%f13
	fadds	%f6,%f5,%f5
	fadds	%f2,%f4,%f2
	ldf	[%o3+44],%f7
	fmuls	%f2,%f5,%f2
	ldf	[%i6-104],%f9
	fmuls	%f2,%f3,%f2
	fadds	%f2,%f4,%f2
	fcmpes	%f8,%f2
	fmovs	%f8,%f0
	fbul,a	0804A074
	fmovs	%f2,%f0

l0804A074:
	ldf	[%o3+36],%f7
	fadds	%f0,%f3,%f3
	ldf	[%o3+48],%f5
	ldf	[%l5+%o0],%f3
	fmuls	%f3,%f2,%f2
	fadds	%f2,%f4,%f8

l0804A08C:
	ldf	[%o3+52],%f5
	fmuls	%f1,%f2,%f0
	ldf	[%i6-104],%f7
	fsubs	%f3,%f1,%f3
	ldf	[%o3+56],%f9
	ldf	[%o3+60],%f5
	fmuls	%f3,%f4,%f3
	ldf	[%o3+76],%f11
	fcmpes	%f7,%f2
	fadds	%f0,%f3,%f3
	ldf	[%o3+80],%f13
	fbuge	080495C8
	fdivs	%f0,%f3,%f0

l0804A0C0:
	ldf	[%i2+%o0],%f5
	ldf	[%o4+4],%f9
	fmuls	%f7,%f6,%f6
	ldf	[%o4+8],%f7
	fmuls	%f2,%f3,%f2
	fdivs	%f2,%f4,%f2
	ldf	[%i0+%o0],%f7
	fadds	%f3,%f5,%f3
	ldf	[%i6-92],%f9
	fadds	%f6,%f4,%f4
	ldf	[%i6-104],%f11
	fsubs	%f5,%f0,%f5
	fdivs	%f2,%f3,%f2
	fmuls	%f4,%f5,%f4
	fmuls	%f2,%f0,%f2
	fdivs	%f4,%f8,%f4
	fadds	%f4,%f2,%f2
	ba	0804963C
	stf	%f5,[%o7+%o0]

l0804A10C:
	ldf	[%i6-104],%f7
	fsubs	%f3,%f1,%f3
	ldf	[%i3+%o0],%f9
	ldf	[%i1+%o0],%f5
	fmuls	%f1,%f2,%f2
	fmuls	%f3,%f4,%f3
	fadds	%f2,%f3,%f2
	stf	%f5,[%i0+%o0]
	ldf	[%i0+%o0],%f3
	fsubs	%f1,%f4,%f3
	fmuls	%f1,%f6,%f2
	fmuls	%f3,%f0,%f3
	stf	%f5,[%l2+%o0]
	ldf	[%i4+%o0],%f5
	fadds	%f2,%f3,%f2
	stf	%f5,[%l7+%o0]
	ba	08049FE8
	ldf	[%i0+%o0],%f5
;;; Segment .data (0804B000)
0804B000 00 00 00 00 53 49 43 55 44 54 53 49 4D 4F 44 20 ....SICUDTSIMOD 
0804B010 4F 35 45 52 52 4F 4E 35 4D 41 58 20 4E 4F 35 4D O5ERRON5MAX NO5M
0804B020 41 58 4E 35 49 53 49 20 4E 35 49 44 49 20 4E 35 AXN5ISI N5IDI N5
0804B030 4E 41 4D 45 4E 35 41 4C 46 49 4E 35 50 49 20 20 NAMEN5ALFIN5PI  
0804B040 4E 35 48 4C 54 20 4E 35 48 47 54 20 4E 35 48 53 N5HLT N5HGT N5HS
0804B050 54 20 4E 35 48 4E 54 20 4E 35 48 4C 53 20 4E 35 T N5HNT N5HLS N5
0804B060 48 47 53 20 4E 35 54 4C 20 20 4E 35 54 47 20 20 HGS N5TL  N5TG  
0804B070 4E 35 53 41 54 20 4E 35 53 53 41 54 4E 35 52 4C N5SAT N5SSATN5RL
0804B080 49 20 4E 35 52 47 49 20 4E 35 41 49 52 46 4E 35 I N5RGI N5AIRFN5
0804B090 44 54 44 50 4E 35 44 52 44 50 42 35 4D 41 58 20 DTDPN5DRDPB5MAX 
0804B0A0 42 35 49 53 49 20 00 00 00 00 00 02 46 01 E8 85 B5ISI ......F...
0804B0B0 41 E7 AE 14 00 00 00 00 00 00 00 00 00 00 00 00 A...............
0804B0C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B0D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B0E0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B0F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B100 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B110 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B120 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B130 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B140 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B150 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B160 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B170 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B180 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B190 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B1A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B1B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B1C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804B1D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
;;; Segment .symtab (0804C000)
0804C000 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0804C010 00 00 00 01 00 00 00 00 00 00 00 00 04 00 FF F1 ................
0804C020 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 02 ................
0804C030 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 03 ................
0804C040 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 04 ................
0804C050 00 00 00 0B 00 00 00 00 00 00 00 18 01 00 00 04 ................
0804C060 00 00 00 1A 00 00 00 18 00 00 00 04 01 00 00 04 ................
0804C070 00 00 00 26 00 00 00 20 00 00 00 6C 01 00 00 04 ...&... ...l....
0804C080 00 00 00 32 00 00 00 90 00 00 00 04 01 00 00 04 ...2............
0804C090 00 00 00 43 00 00 00 98 00 00 00 04 01 00 00 04 ...C............
0804C0A0 00 00 00 54 00 00 00 A0 00 00 00 04 01 00 00 04 ...T............
0804C0B0 00 00 00 66 00 00 00 A8 00 00 00 04 01 00 00 04 ...f............
0804C0C0 00 00 00 73 00 00 00 B0 00 00 00 04 01 00 00 04 ...s............
0804C0D0 00 00 00 81 00 00 00 B8 00 00 00 04 01 00 00 04 ................
0804C0E0 00 00 00 93 00 00 00 C0 00 00 00 04 01 00 00 04 ................
0804C0F0 00 00 00 A3 00 00 00 C8 00 00 00 04 01 00 00 04 ................
0804C100 00 00 00 B4 00 00 00 D0 00 00 00 04 01 00 00 04 ................
0804C110 00 00 00 C2 00 00 00 D8 00 00 00 04 01 00 00 04 ................
0804C120 00 00 00 D3 00 00 00 E0 00 00 00 04 01 00 00 04 ................
0804C130 00 00 00 E1 00 00 00 E8 00 00 00 04 01 00 00 04 ................
0804C140 00 00 00 F3 00 00 00 F0 00 00 00 04 01 00 00 04 ................
0804C150 00 00 01 05 00 00 00 F8 00 00 00 04 01 00 00 04 ................
0804C160 00 00 01 13 00 00 01 00 00 00 00 04 01 00 00 04 ................
0804C170 00 00 01 24 00 00 01 08 00 00 00 04 01 00 00 04 ...$............
0804C180 00 00 01 36 00 00 01 10 00 00 00 04 01 00 00 04 ...6............
0804C190 00 00 01 48 00 00 01 18 00 00 00 04 01 00 00 04 ...H............
0804C1A0 00 00 01 5A 00 00 01 20 00 00 00 04 01 00 00 04 ...Z... ........
0804C1B0 00 00 01 6A 00 00 01 28 00 00 00 04 01 00 00 04 ...j...(........
0804C1C0 00 00 01 78 00 00 00 00 00 00 00 04 01 00 00 03 ...x............
0804C1D0 00 00 01 89 00 00 00 04 00 00 00 A2 01 00 00 03 ................
0804C1E0 00 00 01 98 00 00 00 A8 00 00 00 04 01 00 00 03 ................
0804C1F0 00 00 01 AA 00 00 00 AC 00 00 00 04 01 00 00 03 ................
0804C200 00 00 01 BB 00 00 00 B0 00 00 00 04 01 00 00 03 ................
0804C210 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 05 ................
0804C220 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 06 ................
0804C230 00 00 01 CC 00 00 00 08 00 00 03 20 11 00 FF F2 ........... ....
0804C240 00 00 01 D4 00 00 00 08 00 00 19 04 11 00 FF F2 ................
0804C250 00 00 01 DC 00 00 00 08 00 5B 8D 80 11 00 FF F2 .........[......
0804C260 00 00 01 E4 00 00 00 08 00 3D 09 00 11 00 FF F2 .........=......
0804C270 00 00 01 EC 00 00 00 08 00 2D C6 C0 11 00 FF F2 .........-......
0804C280 00 00 01 F4 00 00 00 08 00 06 1A 80 11 00 FF F2 ................
0804C290 00 00 01 FC 00 00 00 08 00 00 9C 40 11 00 FF F2 ...........@....
0804C2A0 00 00 02 04 00 00 00 08 00 7A 12 00 11 00 FF F2 .........z......
0804C2B0 00 00 02 0C 00 00 00 28 00 00 02 F4 12 00 00 02 .......(........
0804C2C0 00 00 02 13 00 00 00 00 00 00 00 00 10 00 00 00 ................
0804C2D0 00 00 02 1A 00 00 05 20 00 00 1C 34 12 00 00 02 ....... ...4....
0804C2E0 00 00 02 22 00 00 00 00 00 00 00 00 10 00 00 00 ..."............
0804C2F0 00 00 02 2A 00 00 00 00 00 00 00 00 10 00 00 00 ...*............
0804C300 00 2E 2F 74 35 6D 61 74 2E 66 00 47 50 42 2E 74 ../t5mat.f.GPB.t
0804C310 35 6D 61 74 2E 77 73 74 72 00 47 50 42 2E 74 35 5mat.wstr.GPB.t5
0804C320 6D 61 74 2E 69 00 47 50 42 2E 74 35 6D 61 74 2E mat.i.GPB.t5mat.
0804C330 6E 00 47 50 42 2E 74 35 6D 61 74 70 2E 63 70 73 n.GPB.t5matp.cps
0804C340 74 65 00 47 50 42 2E 74 35 6D 61 74 70 2E 78 61 te.GPB.t5matp.xa
0804C350 6D 6F 6C 00 47 50 42 2E 74 35 6D 61 74 70 2E 64 mol.GPB.t5matp.d
0804C360 6E 6F 6D 69 6E 00 47 50 42 2E 74 35 6D 61 74 70 nomin.GPB.t5matp
0804C370 2E 69 00 47 50 42 2E 74 35 6D 61 74 70 2E 78 31 .i.GPB.t5matp.x1
0804C380 00 47 50 42 2E 74 35 6D 61 74 70 2E 78 64 65 6E .GPB.t5matp.xden
0804C390 6F 6D 00 47 50 42 2E 74 35 6D 61 74 70 2E 70 62 om.GPB.t5matp.pb
0804C3A0 61 72 00 47 50 42 2E 74 35 6D 61 74 70 2E 64 65 ar.GPB.t5matp.de
0804C3B0 6C 74 61 00 47 50 42 2E 74 35 6D 61 74 70 2E 78 lta.GPB.t5matp.x
0804C3C0 32 00 47 50 42 2E 74 35 6D 61 74 70 2E 63 70 61 2.GPB.t5matp.cpa
0804C3D0 69 72 00 47 50 42 2E 74 35 6D 61 74 70 2E 78 33 ir.GPB.t5matp.x3
0804C3E0 00 47 50 42 2E 74 35 6D 61 74 70 2E 78 63 6F 6E .GPB.t5matp.xcon
0804C3F0 64 79 00 47 50 42 2E 74 35 6D 61 74 70 2E 64 74 dy.GPB.t5matp.dt
0804C400 73 73 61 74 00 47 50 42 2E 74 35 6D 61 74 70 2E ssat.GPB.t5matp.
0804C410 78 34 00 47 50 42 2E 74 35 6D 61 74 70 2E 6E 35 x4.GPB.t5matp.n5
0804C420 74 6E 78 00 47 50 42 2E 74 35 6D 61 74 70 2E 78 tnx.GPB.t5matp.x
0804C430 73 75 70 68 65 00 47 50 42 2E 74 35 6D 61 74 70 suphe.GPB.t5matp
0804C440 2E 64 64 65 6E 6F 6D 00 47 50 42 2E 74 35 6D 61 .ddenom.GPB.t5ma
0804C450 74 70 2E 78 6E 6F 6D 69 6E 00 47 50 42 2E 74 35 tp.xnomin.GPB.t5
0804C460 6D 61 74 70 2E 74 73 75 62 00 47 50 42 2E 74 35 matp.tsub.GPB.t5
0804C470 6D 61 74 70 2E 69 78 00 47 50 42 2E 74 35 6D 61 matp.ix.GPB.t5ma
0804C480 74 2E 6C 64 62 76 65 72 00 47 50 42 2E 74 35 6D t.ldbver.GPB.t5m
0804C490 61 74 2E 6E 61 6D 65 00 47 50 42 2E 74 35 6D 61 at.name.GPB.t5ma
0804C4A0 74 70 2E 69 61 63 63 75 72 00 47 50 42 2E 74 35 tp.iaccur.GPB.t5
0804C4B0 6D 61 74 70 2E 72 67 61 73 78 00 47 50 42 2E 74 matp.rgasx.GPB.t
0804C4C0 35 6D 61 74 70 2E 78 6D 6F 6C 6D 00 77 72 6E 63 5matp.xmolm.wrnc
0804C4D0 6F 6D 5F 00 77 72 6E 63 6F 32 5F 00 70 73 69 69 om_.wrnco2_.psii
0804C4E0 6E 63 5F 00 70 73 69 72 65 63 5F 00 70 73 69 63 nc_.psirec_.psic
0804C4F0 68 63 5F 00 70 73 69 6C 6F 63 5F 00 70 73 69 63 hc_.psiloc_.psic
0804C500 6F 63 5F 00 70 73 69 64 6F 63 5F 00 74 35 6D 61 oc_.psidoc_.t5ma
0804C510 74 5F 00 61 70 63 61 64 5F 00 74 35 6D 61 74 70 t_.apcad_.t5matp
0804C520 5F 00 5F 5F 73 5F 63 61 74 00 61 70 65 72 73 73 _.__s_cat.aperss
0804C530 5F 00 00 00 00 2C 00 00 04 09 00 00 00 1C 00 00 _....,..........
0804C540 00 30 00 00 04 0C 00 00 00 1C 00 00 00 34 00 00 .0...........4..
0804C550 26 09 FF FF FF FC 00 00 00 38 00 00 26 0C FF FF &........8..&...
0804C560 FF FC 00 00 00 3C 00 00 25 09 FF FF FF FC 00 00 .....<..%.......
0804C570 00 40 00 00 25 0C FF FF FF FC 00 00 00 48 00 00 .@..%........H..
0804C580 03 09 FF FF FF FE 00 00 00 4C 00 00 03 0C FF FF .........L......
0804C590 FF FE 00 00 00 50 00 00 03 09 00 00 00 00 00 00 .....P..........
0804C5A0 00 54 00 00 03 0C 00 00 00 00 00 00 00 58 00 00 .T...........X..
0804C5B0 23 09 00 00 00 08 00 00 00 5C 00 00 23 0C 00 00 #........\..#...
0804C5C0 00 08 00 00 00 A0 00 00 2C 07 00 00 00 00 00 00 ........,.......
0804C5D0 02 84 00 00 2D 07 00 00 00 00 00 00 02 9C 00 00 ....-...........
0804C5E0 02 09 00 00 00 10 00 00 02 A0 00 00 02 0C 00 00 ................
0804C5F0 00 10 00 00 02 D0 00 00 02 09 00 00 00 00 00 00 ................
0804C600 02 EC 00 00 02 0C 00 00 00 00 00 00 02 F0 00 00 ................
0804C610 2E 07 00 00 00 00 00 00 03 04 00 00 2F 07 00 00 ............/...
0804C620 00 00 00 00 05 D4 00 00 02 09 00 00 03 28 00 00 .............(..
0804C630 05 D8 00 00 02 0C 00 00 03 28 00 00 05 DC 00 00 .........(......
0804C640 03 09 00 00 00 A8 00 00 05 E0 00 00 03 0C 00 00 ................
0804C650 00 A8 00 00 06 14 00 00 04 09 00 00 00 F0 00 00 ................
0804C660 06 34 00 00 04 09 00 00 00 F0 00 00 06 40 00 00 .4...........@..
0804C670 04 0C 00 00 00 F0 00 00 06 9C 00 00 04 0C 00 00 ................
0804C680 00 F0                                           ..             
