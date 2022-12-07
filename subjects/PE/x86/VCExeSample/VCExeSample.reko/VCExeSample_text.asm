;;; Segment .text (00401000)

;; main: 00401000
main proc
	push	ebp
	mov	ebp,esp
	push	ecx
	fld1
	fstp	dword ptr [esp]
	push	4020C8h
	mov	eax,[ebp+8h]
	push	eax
	mov	ecx,[ebp+0Ch]
	mov	edx,[ecx]
	push	edx
	call	test1
	add	esp,10h
	xor	eax,eax
	pop	ebp
	ret
00401024             CC CC CC CC CC CC CC CC CC CC CC CC     ............

;; test1: 00401030
;;   Called from:
;;     00401018 (in main)
;;     00401079 (in test2)
;;     0040109D (in test2)
test1 proc
	push	ebp
	mov	ebp,esp
	fld	dword ptr [ebp+14h]
	sub	esp,8h
	fstp	double ptr [esp]
	mov	eax,[ebp+10h]
	push	eax
	mov	ecx,[ebp+0Ch]
	push	ecx
	mov	edx,[ebp+8h]
	push	edx
	push	4020D0h
	call	dword ptr [4020A0h]
	add	esp,18h
	pop	ebp
	ret
00401058                         CC CC CC CC CC CC CC CC         ........

;; test2: 00401060
test2 proc
	push	ebp
	mov	ebp,esp
	push	ecx
	fld	dword ptr [4020F0h]
	fstp	dword ptr [esp]
	push	4020DCh
	push	2h
	push	4020E0h
	call	test1
	add	esp,10h
	cmp	dword ptr [ebp+8h],0h
	jnz	4010A5h

l00401087:
	push	ecx
	fld	dword ptr [4020ECh]
	fstp	dword ptr [esp]
	push	4020E4h
	push	6h
	push	4020E8h
	call	test1
	add	esp,10h

l004010A5:
	pop	ebp
	ret
004010A7                      CC CC CC CC CC CC CC CC CC        .........

;; indirect_call_test3: 004010B0
indirect_call_test3 proc
	push	ebp
	mov	ebp,esp
	push	3E8h
	mov	eax,[ebp+8h]
	push	eax
	mov	ecx,[ebp+8h]
	mov	edx,[ecx]
	mov	eax,[edx+4h]
	call	eax
	add	esp,8h
	pop	ebp
	ret
004010CB                                  CC CC CC CC CC            .....

;; test4: 004010D0
test4 proc
	push	ebp
	mov	ebp,esp
	mov	eax,[403018h]
	push	eax
	mov	ecx,[403018h]
	mov	edx,[ecx]
	mov	eax,[edx]
	call	eax
	add	esp,4h
	pop	ebp
	ret
004010EA                               CC CC CC CC CC CC           ......

;; test5: 004010F0
test5 proc
	push	ebp
	mov	ebp,esp
	push	ecx
	fld	dword ptr [4020F4h]
	fstp	dword ptr [esp]
	push	3E7h
	mov	eax,[403018h]
	push	eax
	mov	ecx,[403018h]
	mov	edx,[ecx]
	mov	eax,[edx+4h]
	call	eax
	add	esp,0Ch
	pop	ebp
	ret
0040111A                               CC CC CC CC CC CC           ......

;; test6: 00401120
;;   Called from:
;;     00401218 (in nested_if_blocks_test8)
test6 proc
	push	ebp
	mov	ebp,esp
	push	ecx
	mov	eax,[ebp+10h]
	push	eax
	mov	ecx,[ebp+0Ch]
	push	ecx
	mov	edx,[ebp+8h]
	push	edx
	mov	eax,[ebp+8h]
	mov	ecx,[eax]
	mov	edx,[ecx+8h]
	call	edx
	add	esp,0Ch
	mov	[ebp-4h],eax
	mov	eax,[ebp-4h]
	push	eax
	mov	ecx,[ebp+8h]
	push	ecx
	mov	edx,[ebp+8h]
	mov	eax,[edx]
	mov	ecx,[eax+4h]
	call	ecx
	add	esp,8h
	mov	esp,ebp
	pop	ebp
	ret
00401159                            CC CC CC CC CC CC CC          .......

;; test7: 00401160
test7 proc
	push	ebp
	mov	ebp,esp
	fld1
	fcomp	double ptr [ebp+8h]
	fstsw	ax
	test	ah,5h
	jpe	401189h

l0040116F:
	sub	esp,8h
	fld	double ptr [ebp+8h]
	fstp	double ptr [esp]
	mov	eax,[403034h]
	mov	edx,[eax]
	mov	ecx,[403034h]
	mov	eax,[edx]
	call	eax

l00401189:
	sub	esp,8h
	fld	double ptr [ebp+8h]
	fstp	double ptr [esp]
	push	0Dh
	mov	ecx,[403034h]
	mov	edx,[ecx]
	mov	ecx,[403034h]
	mov	eax,[edx+4h]
	call	eax
	pop	ebp
	ret
004011A9                            CC CC CC CC CC CC CC          .......

;; nested_if_blocks_test8: 004011B0
;;   Called from:
;;     00401316 (in loop_test11)
nested_if_blocks_test8 proc
	push	ebp
	mov	ebp,esp
	sub	esp,8h
	fld	double ptr [ebp+8h]
	fstp	double ptr [esp]
	push	0FFh
	mov	eax,[403034h]
	mov	edx,[eax]
	mov	ecx,[403034h]
	mov	eax,[edx+4h]
	call	eax
	fstp	st(0)
	fld	double ptr [402100h]
	fcomp	double ptr [ebp+8h]
	fstsw	ax
	test	ah,44h
	jpo	40120Dh

l004011E2:
	fld	double ptr [4020F8h]
	fcomp	double ptr [ebp+8h]
	fstsw	ax
	test	ah,41h
	jnz	40120Dh

l004011F2:
	sub	esp,8h
	fld	double ptr [ebp+8h]
	fstp	double ptr [esp]
	mov	ecx,[403034h]
	mov	edx,[ecx]
	mov	ecx,[403034h]
	mov	eax,[edx]
	call	eax

l0040120D:
	push	7h
	push	6h
	mov	ecx,[403018h]
	push	ecx
	call	test6
	add	esp,0Ch
	pop	ebp
	ret
00401222       CC CC CC CC CC CC CC CC CC CC CC CC CC CC   ..............

;; loop_test9: 00401230
;;   Called from:
;;     00401303 (in loop_test11)
loop_test9 proc
	push	ebp
	mov	ebp,esp
	sub	esp,0Ch
	mov	dword ptr [ebp-4h],0h
	jmp	401248h

l0040123F:
	mov	eax,[ebp-4h]
	add	eax,1h
	mov	[ebp-4h],eax

l00401248:
	fild	dword ptr [ebp-4h]
	fld	dword ptr [ebp+8h]
	sub	esp,8h
	fstp	double ptr [esp]
	mov	ecx,[ebp-4h]
	push	ecx
	mov	edx,[403034h]
	mov	eax,[edx]
	mov	ecx,[403034h]
	mov	edx,[eax+4h]
	fstp	double ptr [ebp-0Ch]
	call	edx
	fcomp	double ptr [ebp-0Ch]
	fstsw	ax
	test	ah,41h
	jnz	401294h

l00401278:
	fld	dword ptr [ebp+8h]
	sub	esp,8h
	fstp	double ptr [esp]
	mov	eax,[403034h]
	mov	edx,[eax]
	mov	ecx,[403034h]
	mov	eax,[edx]
	call	eax
	jmp	40123Fh

l00401294:
	mov	esp,ebp
	pop	ebp
	ret
00401298                         CC CC CC CC CC CC CC CC         ........

;; const_div_test10: 004012A0
const_div_test10 proc
	push	ebp
	mov	ebp,esp
	mov	eax,0Ah
	mov	ecx,3h
	mov	edx,[ebp+8h]
	test	edx,edx
	jz	4012BAh

l004012B4:
	xor	edx,edx
	div	ecx
	mov	ecx,edx

l004012BA:
	mov	[40302Ch],ecx
	mov	[403030h],eax
	pop	ebp
	ret
004012C7                      CC CC CC CC CC CC CC CC CC        .........

;; loop_test11: 004012D0
loop_test11 proc
	push	ebp
	mov	ebp,esp
	sub	esp,8h
	mov	dword ptr [ebp-4h],5h

l004012DD:
	cmp	dword ptr [ebp-4h],0h
	jle	401329h

l004012E3:
	mov	eax,[ebp-4h]
	and	eax,80000001h
	jns	4012F2h

l004012ED:
	dec	eax
	or	eax,0FEh
	inc	eax

l004012F2:
	test	eax,eax
	jnz	40130Dh

l004012F6:
	fld	double ptr [ebp+8h]
	fstp	dword ptr [ebp-8h]
	fld	dword ptr [ebp-8h]
	push	ecx
	fstp	dword ptr [esp]
	call	loop_test9
	add	esp,4h
	jmp	40131Eh

l0040130D:
	sub	esp,8h
	fld	double ptr [ebp+8h]
	fstp	double ptr [esp]
	call	nested_if_blocks_test8
	add	esp,8h

l0040131E:
	mov	ecx,[ebp-4h]
	sub	ecx,1h
	mov	[ebp-4h],ecx
	jmp	4012DDh

l00401329:
	mov	esp,ebp
	pop	ebp
	ret
0040132D                                        CC CC CC              ...

;; nested_structs_test12: 00401330
;;   Called from:
;;     00401367 (in nested_structs_test13)
nested_structs_test12 proc
	push	ebp
	mov	ebp,esp
	mov	eax,[ebp+8h]
	mov	dword ptr [eax],1h
	mov	ecx,[ebp+8h]
	mov	dword ptr [ecx+4h],2h
	mov	edx,[ebp+8h]
	mov	dword ptr [edx+8h],3h
	mov	eax,[ebp+8h]
	mov	dword ptr [eax+0Ch],4h
	pop	ebp
	ret
0040135C                                     CC CC CC CC             ....

;; nested_structs_test13: 00401360
nested_structs_test13 proc
	push	ebp
	mov	ebp,esp
	mov	eax,[ebp+8h]
	push	eax
	call	nested_structs_test12
	add	esp,4h
	pop	ebp
	ret
00401371    CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC  ...............

;; gbl_nested_structs_test14: 00401380
gbl_nested_structs_test14 proc
	push	ebp
	mov	ebp,esp
	mov	dword ptr [40301Ch],5h
	mov	dword ptr [403020h],6h
	mov	dword ptr [403024h],7h
	mov	dword ptr [403028h],8h
	pop	ebp
	ret
004013AD                                        CC CC CC              ...

;; double_return_test15: 004013B0
double_return_test15 proc
	push	ebp
	mov	ebp,esp
	fld	double ptr [ebp+8h]
	pop	ebp
	ret
004013B8                         CC CC CC CC CC CC CC CC         ........
004013C0 68 95 17 40 00 E8 95 03 00 00 A1 60 30 40 00 C7 h..@.......`0@..
004013D0 04 24 4C 30 40 00 FF 35 5C 30 40 00 A3 4C 30 40 .$L0@..5\0@..L0@
004013E0 00 68 3C 30 40 00 68 40 30 40 00 68 38 30 40 00 .h<0@.h@0@.h80@.
004013F0 FF 15 94 20 40 00 83 C4 14 85 C0 A3 48 30 40 00 ... @.......H0@.
00401400 7D 08 6A 08 E8 AB 02 00 00 59 C3 6A 10 68 78 21 }.j......Y.j.hx!
00401410 40 00 E8 F1 04 00 00 33 DB 89 5D FC 64 A1 18 00 @......3..].d...
00401420 00 00 8B 70 04 89 5D E4 BF 9C 33 40 00 53 56 57 ...p..]...3@.SVW
00401430 FF 15 24 20 40 00 3B C3 74 19 3B C6 75 08 33 F6 ..$ @.;.t.;.u.3.
00401440 46 89 75 E4 EB 10 68 E8 03 00 00 FF 15 28 20 40 F.u...h......( @
00401450 00 EB DA 33 F6 46 A1 98 33 40 00 3B C6 75 0A 6A ...3.F..3@.;.u.j
00401460 1F E8 4E 02 00 00 59 EB 3B A1 98 33 40 00 85 C0 ..N...Y.;..3@...
00401470 75 2C 89 35 98 33 40 00 68 C0 20 40 00 68 B4 20 u,.5.3@.h. @.h. 
00401480 40 00 E8 7B 04 00 00 59 59 85 C0 74 17 C7 45 FC @..{...YY..t..E.
00401490 FE FF FF FF B8 FF 00 00 00 E9 DD 00 00 00 89 35 ...............5
004014A0 54 30 40 00 A1 98 33 40 00 3B C6 75 1B 68 B0 20 T0@...3@.;.u.h. 
004014B0 40 00 68 A8 20 40 00 E8 40 04 00 00 59 59 C7 05 @.h. @..@...YY..
004014C0 98 33 40 00 02 00 00 00 39 5D E4 75 08 53 57 FF .3@.....9].u.SW.
004014D0 15 2C 20 40 00 39 1D A8 33 40 00 74 19 68 A8 33 ., @.9..3@.t.h.3
004014E0 40 00 E8 59 03 00 00 59 85 C0 74 0A 53 6A 02 53 @..Y...Y..t.Sj.S
004014F0 FF 15 A8 33 40 00 A1 3C 30 40 00 8B 0D 80 20 40 ...3@..<0@.... @
00401500 00 89 01 FF 35 3C 30 40 00 FF 35 40 30 40 00 FF ....5<0@..5@0@..
00401510 35 38 30 40 00 E8 E6 FA FF FF 83 C4 0C A3 50 30 580@..........P0
00401520 40 00 39 1D 44 30 40 00 75 37 50 FF 15 84 20 40 @.9.D0@.u7P... @
00401530 00 8B 45 EC 8B 08 8B 09 89 4D E0 50 51 E8 78 02 ..E......M.PQ.x.
00401540 00 00 59 59 C3 8B 65 E8 8B 45 E0 A3 50 30 40 00 ..YY..e..E..P0@.
00401550 33 DB 39 1D 44 30 40 00 75 07 50 FF 15 8C 20 40 3.9.D0@.u.P... @
00401560 00 39 1D 54 30 40 00 75 06 FF 15 90 20 40 00 C7 .9.T0@.u.... @..
00401570 45 FC FE FF FF FF A1 50 30 40 00 E8 CD 03 00 00 E......P0@......
00401580 C3 66 81 3D 00 00 40 00 4D 5A 74 04 33 C0 EB 4D .f.=..@.MZt.3..M
00401590 A1 3C 00 40 00 8D 80 00 00 40 00 81 38 50 45 00 .<.@.....@..8PE.
004015A0 00 75 E9 0F B7 48 18 81 F9 0B 01 00 00 74 1B 81 .u...H.......t..
004015B0 F9 0B 02 00 00 75 D5 83 B8 84 00 00 00 0E 76 CC .....u........v.
004015C0 33 C9 39 88 F8 00 00 00 EB 0E 83 78 74 0E 76 BC 3.9........xt.v.
004015D0 33 C9 39 88 E8 00 00 00 0F 95 C1 8B C1 6A 01 A3 3.9..........j..
004015E0 44 30 40 00 FF 15 40 20 40 00 6A FF FF 15 3C 20 D0@...@ @.j...< 
004015F0 40 00 59 59 A3 A0 33 40 00 A3 A4 33 40 00 FF 15 @.YY..3@...3@...
00401600 38 20 40 00 8B 0D 68 30 40 00 89 08 FF 15 4C 20 8 @...h0@.....L 
00401610 40 00 8B 0D 64 30 40 00 89 08 A1 6C 20 40 00 8B @...d0@....l @..
00401620 00 A3 94 33 40 00 E8 46 01 00 00 E8 7D 03 00 00 ...3@..F....}...
00401630 83 3D 0C 30 40 00 00 75 0C 68 AD 19 40 00 FF 15 .=.0@..u.h..@...
00401640 70 20 40 00 59 E8 3A 03 00 00 83 3D 08 30 40 00 p @.Y.:....=.0@.
00401650 FF 75 09 6A FF FF 15 74 20 40 00 59 33 C0 C3 E8 .u.j...t @.Y3...
00401660 4C 03 00 00 E9 A2 FD FF FF 8B 44 24 04 8B 00 81 L.........D$....
00401670 38 63 73 6D E0 75 2A 83 78 10 03 75 24 8B 40 14 8csm.u*.x..u$.@.
00401680 3D 20 05 93 19 74 15 3D 21 05 93 19 74 0E 3D 22 = ...t.=!...t.="
00401690 05 93 19 74 07 3D 00 40 99 01 75 05 E8 A3 03 00 ...t.=.@..u.....
004016A0 00 33 C0 C2 04 00 68 69 16 40 00 FF 15 20 20 40 .3....hi.@...  @
004016B0 00 33 C0 C3 FF 25 98 20 40 00 6A 14 68 98 21 40 .3...%. @.j.h.!@
004016C0 00 E8 42 02 00 00 FF 35 A4 33 40 00 8B 35 58 20 ..B....5.3@..5X 
004016D0 40 00 FF D6 59 89 45 E4 83 F8 FF 75 0C FF 75 08 @...Y.E....u..u.
004016E0 FF 15 54 20 40 00 59 EB 67 6A 08 E8 66 03 00 00 ..T @.Y.gj..f...
004016F0 59 83 65 FC 00 FF 35 A4 33 40 00 FF D6 89 45 E4 Y.e...5.3@....E.
00401700 FF 35 A0 33 40 00 FF D6 59 59 89 45 E0 8D 45 E0 .5.3@...YY.E..E.
00401710 50 8D 45 E4 50 FF 75 08 8B 35 3C 20 40 00 FF D6 P.E.P.u..5< @...
00401720 59 50 E8 29 03 00 00 89 45 DC FF 75 E4 FF D6 A3 YP.)....E..u....
00401730 A4 33 40 00 FF 75 E0 FF D6 83 C4 14 A3 A0 33 40 .3@..u........3@
00401740 00 C7 45 FC FE FF FF FF E8 09 00 00 00 8B 45 DC ..E...........E.
00401750 E8 F8 01 00 00 C3 6A 08 E8 ED 02 00 00 59 C3 FF ......j......Y..
00401760 74 24 04 E8 52 FF FF FF F7 D8 1B C0 F7 D8 59 48 t$..R.........YH
00401770 C3 56 57 B8 68 21 40 00 BF 68 21 40 00 3B C7 8B .VW.h!@..h!@.;..
00401780 F0 73 0F 8B 06 85 C0 74 02 FF D0 83 C6 04 3B F7 .s.....t......;.
00401790 72 F1 5F 5E C3 56 57 B8 70 21 40 00 BF 70 21 40 r._^.VW.p!@..p!@
004017A0 00 3B C7 8B F0 73 0F 8B 06 85 C0 74 02 FF D0 83 .;...s.....t....
004017B0 C6 04 3B F7 72 F1 5F 5E C3 CC FF 25 88 20 40 00 ..;.r._^...%. @.
004017C0 8B 4C 24 04 66 81 39 4D 5A 74 03 33 C0 C3 8B 41 .L$.f.9MZt.3...A
004017D0 3C 03 C1 81 38 50 45 00 00 75 F0 33 C9 66 81 78 <...8PE..u.3.f.x
004017E0 18 0B 01 0F 94 C1 8B C1 C3 CC CC CC CC CC CC CC ................
004017F0 8B 44 24 04 8B 48 3C 03 C8 0F B7 41 14 53 56 0F .D$..H<....A.SV.
00401800 B7 71 06 33 D2 85 F6 57 8D 44 08 18 76 1E 8B 7C .q.3...W.D..v..|
00401810 24 14 8B 48 0C 3B F9 72 09 8B 58 08 03 D9 3B FB $..H.;.r..X...;.
00401820 72 0C 83 C2 01 83 C0 28 3B D6 72 E6 33 C0 5F 5E r......(;.r.3._^
00401830 5B C3 CC CC CC CC CC CC CC CC CC CC CC CC CC CC [...............
00401840 55 8B EC 6A FE 68 B8 21 40 00 68 61 19 40 00 64 U..j.h.!@.ha.@.d
00401850 A1 00 00 00 00 50 83 EC 08 53 56 57 A1 10 30 40 .....P...SVW..0@
00401860 00 31 45 F8 33 C5 50 8D 45 F0 64 A3 00 00 00 00 .1E.3.P.E.d.....
00401870 89 65 E8 C7 45 FC 00 00 00 00 68 00 00 40 00 E8 .e..E.....h..@..
00401880 3C FF FF FF 83 C4 04 85 C0 74 55 8B 45 08 2D 00 <........tU.E.-.
00401890 00 40 00 50 68 00 00 40 00 E8 52 FF FF FF 83 C4 .@.Ph..@..R.....
004018A0 08 85 C0 74 3B 8B 40 24 C1 E8 1F F7 D0 83 E0 01 ...t;.@$........
004018B0 C7 45 FC FE FF FF FF 8B 4D F0 64 89 0D 00 00 00 .E......M.d.....
004018C0 00 59 5F 5E 5B 8B E5 5D C3 8B 45 EC 8B 08 8B 01 .Y_^[..]..E.....
004018D0 33 D2 3D 05 00 00 C0 0F 94 C2 8B C2 C3 8B 65 E8 3.=...........e.
004018E0 C7 45 FC FE FF FF FF 33 C0 8B 4D F0 64 89 0D 00 .E.....3..M.d...
004018F0 00 00 00 59 5F 5E 5B 8B E5 5D C3 CC FF 25 7C 20 ...Y_^[..]...%| 
00401900 40 00 FF 25 78 20 40 00 68 61 19 40 00 64 FF 35 @..%x @.ha.@.d.5
00401910 00 00 00 00 8B 44 24 10 89 6C 24 10 8D 6C 24 10 .....D$..l$..l$.
00401920 2B E0 53 56 57 A1 10 30 40 00 31 45 FC 33 C5 50 +.SVW..0@.1E.3.P
00401930 89 65 E8 FF 75 F8 8B 45 FC C7 45 FC FE FF FF FF .e..u..E..E.....
00401940 89 45 F8 8D 45 F0 64 A3 00 00 00 00 C3 8B 4D F0 .E..E.d.......M.
00401950 64 89 0D 00 00 00 00 59 5F 5F 5E 5B 8B E5 5D 51 d......Y__^[..]Q
00401960 C3 FF 74 24 10 FF 74 24 10 FF 74 24 10 FF 74 24 ..t$..t$..t$..t$
00401970 10 68 5C 1A 40 00 68 10 30 40 00 E8 EC 00 00 00 .h\.@.h.0@......
00401980 83 C4 18 C3 56 68 00 00 03 00 68 00 00 01 00 33 ....Vh....h....3
00401990 F6 56 E8 E1 00 00 00 83 C4 0C 85 C0 74 0D 56 56 .V..........t.VV
004019A0 56 56 56 E8 CA 00 00 00 83 C4 14 5E C3 33 C0 C3 VVV........^.3..
004019B0 55 8B EC 83 EC 10 A1 10 30 40 00 83 65 F8 00 83 U.......0@..e...
004019C0 65 FC 00 53 57 BF 4E E6 40 BB 3B C7 BB 00 00 FF e..SW.N.@.;.....
004019D0 FF 74 0D 85 C3 74 09 F7 D0 A3 14 30 40 00 EB 60 .t...t.....0@..`
004019E0 56 8D 45 F8 50 FF 15 0C 20 40 00 8B 75 FC 33 75 V.E.P... @..u.3u
004019F0 F8 FF 15 10 20 40 00 33 F0 FF 15 14 20 40 00 33 .... @.3.... @.3
00401A00 F0 FF 15 18 20 40 00 33 F0 8D 45 F0 50 FF 15 1C .... @.3..E.P...
00401A10 20 40 00 8B 45 F4 33 45 F0 33 F0 3B F7 75 07 BE  @..E.3E.3.;.u..
00401A20 4F E6 40 BB EB 0B 85 F3 75 07 8B C6 C1 E0 10 0B O.@.....u.......
00401A30 F0 89 35 10 30 40 00 F7 D6 89 35 14 30 40 00 5E ..5.0@....5.0@.^
00401A40 5F 5B C9 C3 FF 25 44 20 40 00 FF 25 48 20 40 00 _[...%D @..%H @.
00401A50 FF 25 9C 20 40 00 FF 25 50 20 40 00 3B 0D 10 30 .%. @..%P @.;..0
00401A60 40 00 75 02 F3 C3 E9 13 00 00 00 CC FF 25 5C 20 @.u..........%\ 
00401A70 40 00 FF 25 60 20 40 00 FF 25 64 20 40 00 55 8B @..%` @..%d @.U.
00401A80 EC 81 EC 28 03 00 00 A3 78 31 40 00 89 0D 74 31 ...(....x1@...t1
00401A90 40 00 89 15 70 31 40 00 89 1D 6C 31 40 00 89 35 @...p1@...l1@..5
00401AA0 68 31 40 00 89 3D 64 31 40 00 66 8C 15 90 31 40 h1@..=d1@.f...1@
00401AB0 00 66 8C 0D 84 31 40 00 66 8C 1D 60 31 40 00 66 .f...1@.f..`1@.f
00401AC0 8C 05 5C 31 40 00 66 8C 25 58 31 40 00 66 8C 2D ..\1@.f.%X1@.f.-
00401AD0 54 31 40 00 9C 8F 05 88 31 40 00 8B 45 00 A3 7C T1@.....1@..E..|
00401AE0 31 40 00 8B 45 04 A3 80 31 40 00 8D 45 08 A3 8C 1@..E...1@..E...
00401AF0 31 40 00 8B 85 E0 FC FF FF C7 05 C8 30 40 00 01 1@..........0@..
00401B00 00 01 00 A1 80 31 40 00 A3 7C 30 40 00 C7 05 70 .....1@..|0@...p
00401B10 30 40 00 09 04 00 C0 C7 05 74 30 40 00 01 00 00 0@.......t0@....
00401B20 00 A1 10 30 40 00 89 85 D8 FC FF FF A1 14 30 40 ...0@.........0@
00401B30 00 89 85 DC FC FF FF FF 15 30 20 40 00 A3 C0 30 .........0 @...0
00401B40 40 00 6A 01 E8 39 00 00 00 59 6A 00 FF 15 20 20 @.j..9...Yj...  
00401B50 40 00 68 08 21 40 00 FF 15 00 20 40 00 83 3D C0 @.h.!@.... @..=.
00401B60 30 40 00 00 75 08 6A 01 E8 15 00 00 00 59 68 09 0@..u.j......Yh.
00401B70 04 00 C0 FF 15 04 20 40 00 50 FF 15 08 20 40 00 ...... @.P... @.
00401B80 C9 C3 FF 25 68 20 40 00                         ...%h @.        
