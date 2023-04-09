;;; Segment .text (00401000)

;; setInteger: 00401000
;;   Called from:
;;     00401060 (in setParameter)
setInteger proc
	push	ebp
	mov	ebp,esp
	mov	eax,[ebp+8h]
	mov	ecx,[ebp+0Ch]
	mov	[eax],ecx
	pop	ebp
	ret
0040100D                                        CC CC CC              ...

;; setFloat: 00401010
;;   Called from:
;;     0040107D (in setParameter)
setFloat proc
	push	ebp
	mov	ebp,esp
	mov	eax,[ebp+8h]
	fld	dword ptr [ebp+0Ch]
	fstp	dword ptr [eax+4h]
	pop	ebp
	ret
0040101E                                           CC CC               ..

;; setDouble: 00401020
;;   Called from:
;;     0040109C (in setParameter)
setDouble proc
	push	ebp
	mov	ebp,esp
	mov	eax,[ebp+8h]
	fld	double ptr [ebp+0Ch]
	fstp	double ptr [eax+8h]
	pop	ebp
	ret
0040102E                                           CC CC               ..

;; setParameter: 00401030
;;   Called from:
;;     00401143 (in main)
;;     00401194 (in main)
;;     004011E2 (in main)
setParameter proc
	push	ebp
	mov	ebp,esp
	sub	esp,10h
	mov	eax,[ebp+0Ch]
	mov	[ebp-10h],eax
	cmp	dword ptr [ebp-10h],0h
	jz	401050h

l00401042:
	cmp	dword ptr [ebp-10h],1h
	jz	40106Ah

l00401048:
	cmp	dword ptr [ebp-10h],2h
	jz	401087h

l0040104E:
	jmp	4010A4h

l00401050:
	mov	ecx,[ebp+10h]
	mov	[ebp-4h],ecx
	mov	edx,[ebp-4h]
	mov	eax,[edx]
	push	eax
	mov	ecx,[ebp+8h]
	push	ecx
	call	setInteger
	add	esp,8h
	jmp	4010A4h

l0040106A:
	mov	edx,[ebp+10h]
	mov	[ebp-8h],edx
	mov	eax,[ebp-8h]
	push	ecx
	fld	dword ptr [eax]
	fstp	dword ptr [esp]
	mov	ecx,[ebp+8h]
	push	ecx
	call	setFloat
	add	esp,8h
	jmp	4010A4h

l00401087:
	mov	edx,[ebp+10h]
	mov	[ebp-0Ch],edx
	mov	eax,[ebp-0Ch]
	sub	esp,8h
	fld	double ptr [eax]
	fstp	double ptr [esp]
	mov	ecx,[ebp+8h]
	push	ecx
	call	setDouble
	add	esp,0Ch

l004010A4:
	mov	esp,ebp
	pop	ebp
	ret
004010A8                         CC CC CC CC CC CC CC CC         ........

;; new_data_struct: 004010B0
;;   Called from:
;;     004010D6 (in main)
new_data_struct proc
	push	ebp
	mov	ebp,esp
	push	ecx
	push	10h
	call	401230h
	add	esp,4h
	mov	[ebp-4h],eax
	mov	eax,[ebp-4h]
	mov	esp,ebp
	pop	ebp
	ret
004010C8                         CC CC CC CC CC CC CC CC         ........

;; main: 004010D0
main proc
	push	ebp
	mov	ebp,esp
	sub	esp,18h
	call	new_data_struct
	mov	[ebp-4h],eax
	mov	dword ptr [ebp-8h],1h
	jmp	4010F0h

l004010E7:
	mov	eax,[ebp-8h]
	add	eax,1h
	mov	[ebp-8h],eax

l004010F0:
	mov	ecx,[ebp+8h]
	sub	ecx,1h
	cmp	[ebp-8h],ecx
	jge	4011EFh

l004010FF:
	push	4020D0h
	mov	edx,[ebp-8h]
	mov	eax,[ebp+0Ch]
	mov	ecx,[eax+edx*4]
	push	ecx
	call	401236h
	add	esp,8h
	test	eax,eax
	jnz	401150h

l0040111A:
	mov	edx,[ebp-8h]
	add	edx,1h
	mov	[ebp-8h],edx
	mov	eax,[ebp-8h]
	mov	ecx,[ebp+0Ch]
	mov	edx,[ecx+eax*4]
	push	edx
	call	dword ptr [40209Ch]
	add	esp,4h
	mov	[ebp-0Ch],eax
	lea	eax,[ebp-0Ch]
	push	eax
	push	0h
	mov	ecx,[ebp-4h]
	push	ecx
	call	setParameter
	add	esp,0Ch
	jmp	4011EAh

l00401150:
	push	4020D4h
	mov	edx,[ebp-8h]
	mov	eax,[ebp+0Ch]
	mov	ecx,[eax+edx*4]
	push	ecx
	call	401236h
	add	esp,8h
	test	eax,eax
	jnz	40119Eh

l0040116B:
	mov	edx,[ebp-8h]
	add	edx,1h
	mov	[ebp-8h],edx
	mov	eax,[ebp-8h]
	mov	ecx,[ebp+0Ch]
	mov	edx,[ecx+eax*4]
	push	edx
	call	dword ptr [4020A0h]
	add	esp,4h
	fstp	dword ptr [ebp-10h]
	lea	eax,[ebp-10h]
	push	eax
	push	1h
	mov	ecx,[ebp-4h]
	push	ecx
	call	setParameter
	add	esp,0Ch
	jmp	4011EAh

l0040119E:
	push	4020D8h
	mov	edx,[ebp-8h]
	mov	eax,[ebp+0Ch]
	mov	ecx,[eax+edx*4]
	push	ecx
	call	401236h
	add	esp,8h
	test	eax,eax
	jnz	4011EAh

l004011B9:
	mov	edx,[ebp-8h]
	add	edx,1h
	mov	[ebp-8h],edx
	mov	eax,[ebp-8h]
	mov	ecx,[ebp+0Ch]
	mov	edx,[ecx+eax*4]
	push	edx
	call	dword ptr [4020A0h]
	add	esp,4h
	fstp	double ptr [ebp-18h]
	lea	eax,[ebp-18h]
	push	eax
	push	2h
	mov	ecx,[ebp-4h]
	push	ecx
	call	setParameter
	add	esp,0Ch

l004011EA:
	jmp	4010E7h

l004011EF:
	mov	edx,[ebp-4h]
	mov	eax,[edx]
	mov	[403018h],eax
	mov	ecx,[ebp-4h]
	mov	edx,[ecx+4h]
	mov	[40301Ch],edx
	mov	eax,[ebp-4h]
	mov	ecx,[eax+8h]
	mov	[403020h],ecx
	mov	edx,[eax+0Ch]
	mov	[403024h],edx
	mov	eax,[ebp-4h]
	push	eax
	call	dword ptr [4020A4h]
	add	esp,4h
	xor	eax,eax
	mov	esp,ebp
	pop	ebp
	ret
0040122D                                        CC CC CC              ...
00401230 FF 25 AC 20 40 00                               .%. @.          
00401236                   FF 25 98 20 40 00 68 C5 15 40       .%. @.h..@
00401240 00 E8 49 03 00 00 A1 4C 30 40 00 C7 04 24 3C 30 ..I....L0@...$<0
00401250 40 00 FF 35 48 30 40 00 A3 3C 30 40 00 68 2C 30 @..5H0@..<0@.h,0
00401260 40 00 68 30 30 40 00 68 28 30 40 00 FF 15 90 20 @.h00@.h(0@.... 
00401270 40 00 83 C4 14 85 C0 A3 38 30 40 00 7D 08 6A 08 @.......80@.}.j.
00401280 E8 65 02 00 00 59 C3 6A 10 68 48 21 40 00 E8 49 .e...Y.j.hH!@..I
00401290 04 00 00 33 DB 89 5D FC 64 A1 18 00 00 00 8B 70 ...3..].d......p
004012A0 04 89 5D E4 BF 84 33 40 00 53 56 57 FF 15 24 20 ..]...3@.SVW..$ 
004012B0 40 00 3B C3 74 19 3B C6 75 08 33 F6 46 89 75 E4 @.;.t.;.u.3.F.u.
004012C0 EB 10 68 E8 03 00 00 FF 15 28 20 40 00 EB DA 33 ..h......( @...3
004012D0 F6 46 A1 80 33 40 00 3B C6 75 0A 6A 1F E8 08 02 .F..3@.;.u.j....
004012E0 00 00 59 EB 3B A1 80 33 40 00 85 C0 75 2C 89 35 ..Y.;..3@...u,.5
004012F0 80 33 40 00 68 C8 20 40 00 68 C0 20 40 00 E8 D1 .3@.h. @.h. @...
00401300 03 00 00 59 59 85 C0 74 17 C7 45 FC FE FF FF FF ...YY..t..E.....
00401310 B8 FF 00 00 00 E9 DD 00 00 00 89 35 44 30 40 00 ...........5D0@.
00401320 A1 80 33 40 00 3B C6 75 1B 68 BC 20 40 00 68 B4 ..3@.;.u.h. @.h.
00401330 20 40 00 E8 96 03 00 00 59 59 C7 05 80 33 40 00  @......YY...3@.
00401340 02 00 00 00 39 5D E4 75 08 53 57 FF 15 2C 20 40 ....9].u.SW.., @
00401350 00 39 1D 90 33 40 00 74 19 68 90 33 40 00 E8 FF .9..3@.t.h.3@...
00401360 02 00 00 59 85 C0 74 0A 53 6A 02 53 FF 15 90 33 ...Y..t.Sj.S...3
00401370 40 00 A1 2C 30 40 00 8B 0D 7C 20 40 00 89 01 FF @..,0@...| @....
00401380 35 2C 30 40 00 FF 35 30 30 40 00 FF 35 28 30 40 5,0@..500@..5(0@
00401390 00 E8 3A FD FF FF 83 C4 0C A3 40 30 40 00 39 1D ..:.......@0@.9.
004013A0 34 30 40 00 75 37 50 FF 15 80 20 40 00 8B 45 EC 40@.u7P... @..E.
004013B0 8B 08 8B 09 89 4D E0 50 51 E8 2C 02 00 00 59 59 .....M.PQ.,...YY
004013C0 C3 8B 65 E8 8B 45 E0 A3 40 30 40 00 33 DB 39 1D ..e..E..@0@.3.9.
004013D0 34 30 40 00 75 07 50 FF 15 88 20 40 00 39 1D 44 40@.u.P... @.9.D
004013E0 30 40 00 75 06 FF 15 8C 20 40 00 C7 45 FC FE FF 0@.u.... @..E...
004013F0 FF FF A1 40 30 40 00 E8 25 03 00 00 C3 66 81 3D ...@0@..%....f.=
00401400 00 00 40 00 4D 5A 74 04 33 C0 EB 51 A1 3C 00 40 ..@.MZt.3..Q.<.@
00401410 00 81 B8 00 00 40 00 50 45 00 00 75 EB 0F B7 88 .....@.PE..u....
00401420 18 00 40 00 81 F9 0B 01 00 00 74 1B 81 F9 0B 02 ..@.......t.....
00401430 00 00 75 D4 83 B8 84 00 40 00 0E 76 CB 33 C9 39 ..u.....@..v.3.9
00401440 88 F8 00 40 00 EB 11 83 B8 74 00 40 00 0E 76 B8 ...@.....t.@..v.
00401450 33 C9 39 88 E8 00 40 00 0F 95 C1 8B C1 6A 01 A3 3.9...@......j..
00401460 34 30 40 00 FF 15 50 20 40 00 6A FF FF 15 4C 20 40@...P @.j...L 
00401470 40 00 59 59 A3 88 33 40 00 A3 8C 33 40 00 FF 15 @.YY..3@...3@...
00401480 48 20 40 00 8B 0D 54 30 40 00 89 08 FF 15 44 20 H @...T0@.....D 
00401490 40 00 8B 0D 50 30 40 00 89 08 A1 40 20 40 00 8B @...P0@....@ @..
004014A0 00 A3 7C 33 40 00 E8 F6 00 00 00 E8 D1 02 00 00 ..|3@...........
004014B0 83 3D 0C 30 40 00 00 75 0C 68 81 17 40 00 FF 15 .=.0@..u.h..@...
004014C0 3C 20 40 00 59 E8 8E 02 00 00 83 3D 08 30 40 00 < @.Y......=.0@.
004014D0 FF 75 09 6A FF FF 15 38 20 40 00 59 33 C0 C3 E8 .u.j...8 @.Y3...
004014E0 A0 02 00 00 E9 9E FD FF FF CC FF 25 94 20 40 00 ...........%. @.
004014F0 6A 14 68 68 21 40 00 E8 E0 01 00 00 FF 35 8C 33 j.hh!@.......5.3
00401500 40 00 8B 35 64 20 40 00 FF D6 59 89 45 E4 83 F8 @..5d @...Y.E...
00401510 FF 75 0C FF 75 08 FF 15 60 20 40 00 59 EB 61 6A .u..u...` @.Y.aj
00401520 08 E8 FE 02 00 00 59 83 65 FC 00 FF 35 8C 33 40 ......Y.e...5.3@
00401530 00 FF D6 89 45 E4 FF 35 88 33 40 00 FF D6 89 45 ....E..5.3@....E
00401540 E0 8D 45 E0 50 8D 45 E4 50 FF 75 08 E8 CD 02 00 ..E.P.E.P.u.....
00401550 00 89 45 DC FF 75 E4 8B 35 4C 20 40 00 FF D6 A3 ..E..u..5L @....
00401560 8C 33 40 00 FF 75 E0 FF D6 83 C4 1C A3 88 33 40 .3@..u........3@
00401570 00 C7 45 FC FE FF FF FF E8 09 00 00 00 8B 45 DC ..E...........E.
00401580 E8 9C 01 00 00 C3 6A 08 E8 8B 02 00 00 59 C3 FF ......j......Y..
00401590 74 24 04 E8 58 FF FF FF F7 D8 1B C0 F7 D8 59 48 t$..X.........YH
004015A0 C3 56 57 B8 38 21 40 00 BF 38 21 40 00 3B C7 8B .VW.8!@..8!@.;..
004015B0 F0 73 0F 8B 06 85 C0 74 02 FF D0 83 C6 04 3B F7 .s.....t......;.
004015C0 72 F1 5F 5E C3 56 57 B8 40 21 40 00 BF 40 21 40 r._^.VW.@!@..@!@
004015D0 00 3B C7 8B F0 73 0F 8B 06 85 C0 74 02 FF D0 83 .;...s.....t....
004015E0 C6 04 3B F7 72 F1 5F 5E C3 CC FF 25 84 20 40 00 ..;.r._^...%. @.
004015F0 8B 4C 24 04 66 81 39 4D 5A 74 03 33 C0 C3 8B 41 .L$.f.9MZt.3...A
00401600 3C 03 C1 81 38 50 45 00 00 75 F0 33 C9 66 81 78 <...8PE..u.3.f.x
00401610 18 0B 01 0F 94 C1 8B C1 C3 CC CC CC CC CC CC CC ................
00401620 8B 44 24 04 8B 48 3C 03 C8 0F B7 41 14 53 56 0F .D$..H<....A.SV.
00401630 B7 71 06 33 D2 85 F6 57 8D 44 08 18 76 1E 8B 7C .q.3...W.D..v..|
00401640 24 14 8B 48 0C 3B F9 72 09 8B 58 08 03 D9 3B FB $..H.;.r..X...;.
00401650 72 0C 83 C2 01 83 C0 28 3B D6 72 E6 33 C0 5F 5E r......(;.r.3._^
00401660 5B C3 6A 08 68 88 21 40 00 E8 6E 00 00 00 83 65 [.j.h.!@..n....e
00401670 FC 00 BA 00 00 40 00 52 E8 73 FF FF FF 59 85 C0 .....@.R.s...Y..
00401680 74 3D 8B 45 08 2B C2 50 52 E8 92 FF FF FF 59 59 t=.E.+.PR.....YY
00401690 85 C0 74 2B 8B 40 24 C1 E8 1F F7 D0 83 E0 01 C7 ..t+.@$.........
004016A0 45 FC FE FF FF FF EB 20 8B 45 EC 8B 00 8B 00 33 E...... .E.....3
004016B0 C9 3D 05 00 00 C0 0F 94 C1 8B C1 C3 8B 65 E8 C7 .=...........e..
004016C0 45 FC FE FF FF FF 33 C0 E8 54 00 00 00 C3 FF 25 E.....3..T.....%
004016D0 78 20 40 00 FF 25 54 20 40 00 CC CC 68 35 17 40 x @..%T @...h5.@
004016E0 00 64 FF 35 00 00 00 00 8B 44 24 10 89 6C 24 10 .d.5.....D$..l$.
004016F0 8D 6C 24 10 2B E0 53 56 57 A1 10 30 40 00 31 45 .l$.+.SVW..0@.1E
00401700 FC 33 C5 50 89 65 E8 FF 75 F8 8B 45 FC C7 45 FC .3.P.e..u..E..E.
00401710 FE FF FF FF 89 45 F8 8D 45 F0 64 A3 00 00 00 00 .....E..E.d.....
00401720 C3 8B 4D F0 64 89 0D 00 00 00 00 59 5F 5F 5E 5B ..M.d......Y__^[
00401730 8B E5 5D 51 C3 FF 74 24 10 FF 74 24 10 FF 74 24 ..]Q..t$..t$..t$
00401740 10 FF 74 24 10 68 2A 18 40 00 68 10 30 40 00 E8 ..t$.h*.@.h.0@..
00401750 E6 00 00 00 83 C4 18 C3 56 68 00 00 03 00 68 00 ........Vh....h.
00401760 00 01 00 33 F6 56 E8 DB 00 00 00 83 C4 0C 85 C0 ...3.V..........
00401770 74 0D 56 56 56 56 56 E8 C4 00 00 00 83 C4 14 5E t.VVVVV........^
00401780 C3 33 C0 C3 55 8B EC 83 EC 10 A1 10 30 40 00 83 .3..U.......0@..
00401790 65 F8 00 83 65 FC 00 53 57 BF 4E E6 40 BB 3B C7 e...e..SW.N.@.;.
004017A0 BB 00 00 FF FF 74 0D 85 C3 74 09 F7 D0 A3 14 30 .....t...t.....0
004017B0 40 00 EB 60 56 8D 45 F8 50 FF 15 10 20 40 00 8B @..`V.E.P... @..
004017C0 75 FC 33 75 F8 FF 15 14 20 40 00 33 F0 FF 15 18 u.3u.... @.3....
004017D0 20 40 00 33 F0 FF 15 1C 20 40 00 33 F0 8D 45 F0  @.3.... @.3..E.
004017E0 50 FF 15 20 20 40 00 8B 45 F4 33 45 F0 33 F0 3B P..  @..E.3E.3.;
004017F0 F7 75 07 BE 4F E6 40 BB EB 0B 85 F3 75 07 8B C6 .u..O.@.....u...
00401800 C1 E0 10 0B F0 89 35 10 30 40 00 F7 D6 89 35 14 ......5.0@....5.
00401810 30 40 00 5E 5F 5B C9 C3 FF 25 A8 20 40 00 FF 25 0@.^_[...%. @..%
00401820 58 20 40 00 FF 25 5C 20 40 00 3B 0D 10 30 40 00 X @..%\ @.;..0@.
00401830 75 02 F3 C3 E9 13 00 00 00 CC FF 25 68 20 40 00 u..........%h @.
00401840 FF 25 6C 20 40 00 FF 25 70 20 40 00 55 8B EC 81 .%l @..%p @.U...
00401850 EC 28 03 00 00 A3 60 31 40 00 89 0D 5C 31 40 00 .(....`1@...\1@.
00401860 89 15 58 31 40 00 89 1D 54 31 40 00 89 35 50 31 ..X1@...T1@..5P1
00401870 40 00 89 3D 4C 31 40 00 66 8C 15 78 31 40 00 66 @..=L1@.f..x1@.f
00401880 8C 0D 6C 31 40 00 66 8C 1D 48 31 40 00 66 8C 05 ..l1@.f..H1@.f..
00401890 44 31 40 00 66 8C 25 40 31 40 00 66 8C 2D 3C 31 D1@.f.%@1@.f.-<1
004018A0 40 00 9C 8F 05 70 31 40 00 8B 45 00 A3 64 31 40 @....p1@..E..d1@
004018B0 00 8B 45 04 A3 68 31 40 00 8D 45 08 A3 74 31 40 ..E..h1@..E..t1@
004018C0 00 8B 85 E0 FC FF FF C7 05 B0 30 40 00 01 00 01 ..........0@....
004018D0 00 A1 68 31 40 00 A3 64 30 40 00 C7 05 58 30 40 ..h1@..d0@...X0@
004018E0 00 09 04 00 C0 C7 05 5C 30 40 00 01 00 00 00 A1 .......\0@......
004018F0 10 30 40 00 89 85 D8 FC FF FF A1 14 30 40 00 89 .0@.........0@..
00401900 85 DC FC FF FF FF 15 30 20 40 00 A3 A8 30 40 00 .......0 @...0@.
00401910 6A 01 E8 39 00 00 00 59 6A 00 FF 15 00 20 40 00 j..9...Yj.... @.
00401920 68 DC 20 40 00 FF 15 04 20 40 00 83 3D A8 30 40 h. @.... @..=.0@
00401930 00 00 75 08 6A 01 E8 15 00 00 00 59 68 09 04 00 ..u.j......Yh...
00401940 C0 FF 15 08 20 40 00 50 FF 15 0C 20 40 00 C9 C3 .... @.P... @...
00401950 FF 25 74 20 40 00                               .%t @.          
