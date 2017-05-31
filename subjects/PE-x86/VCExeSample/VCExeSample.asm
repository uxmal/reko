;;; Segment .text (00401000)

;; main: 00401000
main proc
	push	ebp
	mov	ebp,esp
	push	ecx
	fld1	
	fstp	dword ptr [esp]
	push	004020C0
	mov	eax,[ebp+08]
	push	eax
	mov	ecx,[ebp+0C]
	mov	edx,[ecx]
	push	edx
	call	00401030
	add	esp,10
	xor	eax,eax
	pop	ebp
	ret	
00401024             CC CC CC CC CC CC CC CC CC CC CC CC     ............

;; test1: 00401030
test1 proc
	push	ebp
	mov	ebp,esp
	fld	dword ptr [ebp+14]
	sub	esp,08
	fstp	double ptr [esp]
	mov	eax,[ebp+10]
	push	eax
	mov	ecx,[ebp+0C]
	push	ecx
	mov	edx,[ebp+08]
	push	edx
	push	004020C8
	call	dword ptr [0040209C]
	add	esp,18
	pop	ebp
	ret	
00401058                         CC CC CC CC CC CC CC CC         ........

;; test2: 00401060
test2 proc
	push	ebp
	mov	ebp,esp
	push	ecx
	fld	dword ptr [004020E8]
	fstp	dword ptr [esp]
	push	004020D4
	push	02
	push	004020D8
	call	00401030
	add	esp,10
	cmp	dword ptr [ebp+08],00
	jnz	004010A5

l00401087:
	push	ecx
	fld	dword ptr [004020E4]
	fstp	dword ptr [esp]
	push	004020DC
	push	06
	push	004020E0
	call	00401030
	add	esp,10

l004010A5:
	pop	ebp
	ret	
004010A7                      CC CC CC CC CC CC CC CC CC        .........

;; indirect_call_test3: 004010B0
indirect_call_test3 proc
	push	ebp
	mov	ebp,esp
	push	000003E8
	mov	eax,[ebp+08]
	push	eax
	mov	ecx,[ebp+08]
	mov	edx,[ecx]
	mov	eax,[edx+04]
	call	eax
	add	esp,08
	pop	ebp
	ret	
004010CB                                  CC CC CC CC CC            .....

;; test4: 004010D0
test4 proc
	push	ebp
	mov	ebp,esp
	mov	eax,[00403018]
	push	eax
	mov	ecx,[00403018]
	mov	edx,[ecx]
	mov	eax,[edx]
	call	eax
	add	esp,04
	pop	ebp
	ret	
004010EA                               CC CC CC CC CC CC           ......

;; test5: 004010F0
test5 proc
	push	ebp
	mov	ebp,esp
	push	ecx
	fld	dword ptr [004020EC]
	fstp	dword ptr [esp]
	push	000003E7
	mov	eax,[00403018]
	push	eax
	mov	ecx,[00403018]
	mov	edx,[ecx]
	mov	eax,[edx+04]
	call	eax
	add	esp,0C
	pop	ebp
	ret	
0040111A                               CC CC CC CC CC CC           ......

;; test6: 00401120
test6 proc
	push	ebp
	mov	ebp,esp
	push	ecx
	mov	eax,[ebp+10]
	push	eax
	mov	ecx,[ebp+0C]
	push	ecx
	mov	edx,[ebp+08]
	push	edx
	mov	eax,[ebp+08]
	mov	ecx,[eax]
	mov	edx,[ecx+08]
	call	edx
	add	esp,0C
	mov	[ebp-04],eax
	mov	eax,[ebp-04]
	push	eax
	mov	ecx,[ebp+08]
	push	ecx
	mov	edx,[ebp+08]
	mov	eax,[edx]
	mov	ecx,[eax+04]
	call	ecx
	add	esp,08
	mov	esp,ebp
	pop	ebp
	ret	
00401159                            CC CC CC CC CC CC CC          .......

;; test7: 00401160
test7 proc
	push	ebp
	mov	ebp,esp
	fld1	
	fcomp	double ptr [ebp+08]
	fstsw	ax
	test	ah,05
	jpe	00401189

l0040116F:
	sub	esp,08
	fld	double ptr [ebp+08]
	fstp	double ptr [esp]
	mov	eax,[00403024]
	mov	edx,[eax]
	mov	ecx,[00403024]
	mov	eax,[edx]
	call	eax

l00401189:
	sub	esp,08
	fld	double ptr [ebp+08]
	fstp	double ptr [esp]
	push	0D
	mov	ecx,[00403024]
	mov	edx,[ecx]
	mov	ecx,[00403024]
	mov	eax,[edx+04]
	call	eax
	pop	ebp
	ret	
004011A9                            CC CC CC CC CC CC CC          .......

;; nested_if_blocks_test8: 004011B0
nested_if_blocks_test8 proc
	push	ebp
	mov	ebp,esp
	sub	esp,08
	fld	double ptr [ebp+08]
	fstp	double ptr [esp]
	push	FF
	mov	eax,[00403024]
	mov	edx,[eax]
	mov	ecx,[00403024]
	mov	eax,[edx+04]
	call	eax
	fstp	st(0)
	fld	double ptr [004020F8]
	fcomp	double ptr [ebp+08]
	fstsw	ax
	test	ah,44
	jpo	0040120D

l004011E2:
	fld	double ptr [004020F0]
	fcomp	double ptr [ebp+08]
	fstsw	ax
	test	ah,41
	jnz	0040120D

l004011F2:
	sub	esp,08
	fld	double ptr [ebp+08]
	fstp	double ptr [esp]
	mov	ecx,[00403024]
	mov	edx,[ecx]
	mov	ecx,[00403024]
	mov	eax,[edx]
	call	eax

l0040120D:
	push	07
	push	06
	mov	ecx,[00403018]
	push	ecx
	call	00401120
	add	esp,0C
	pop	ebp
	ret	
00401222       CC CC CC CC CC CC CC CC CC CC CC CC CC CC   ..............

;; loop_test9: 00401230
loop_test9 proc
	push	ebp
	mov	ebp,esp
	sub	esp,0C
	mov	dword ptr [ebp-04],00000000
	jmp	00401248

l0040123F:
	mov	eax,[ebp-04]
	add	eax,01
	mov	[ebp-04],eax

l00401248:
	fild	dword ptr [ebp-04]
	fld	dword ptr [ebp+08]
	sub	esp,08
	fstp	double ptr [esp]
	mov	ecx,[ebp-04]
	push	ecx
	mov	edx,[00403024]
	mov	eax,[edx]
	mov	ecx,[00403024]
	mov	edx,[eax+04]
	fstp	double ptr [ebp-0C]
	call	edx
	fcomp	double ptr [ebp-0C]
	fstsw	ax
	test	ah,41
	jnz	00401294

l00401278:
	fld	dword ptr [ebp+08]
	sub	esp,08
	fstp	double ptr [esp]
	mov	eax,[00403024]
	mov	edx,[eax]
	mov	ecx,[00403024]
	mov	eax,[edx]
	call	eax
	jmp	0040123F

l00401294:
	mov	esp,ebp
	pop	ebp
	ret	
00401298                         CC CC CC CC CC CC CC CC         ........

;; const_div_test10: 004012A0
const_div_test10 proc
	push	ebp
	mov	ebp,esp
	mov	eax,0000000A
	mov	ecx,00000003
	mov	edx,[ebp+08]
	test	edx,edx
	jz	004012BA

l004012B4:
	xor	edx,edx
	div	ecx
	mov	ecx,edx

l004012BA:
	mov	[0040301C],ecx
	mov	[00403020],eax
	pop	ebp
	ret	
004012C7                      CC CC CC CC CC CC CC CC CC        .........

;; loop_test11: 004012D0
loop_test11 proc
	push	ebp
	mov	ebp,esp
	sub	esp,08
	mov	dword ptr [ebp-04],00000005

l004012DD:
	cmp	dword ptr [ebp-04],00
	jle	00401329

l004012E3:
	mov	eax,[ebp-04]
	and	eax,80000001
	jns	004012F2

l004012ED:
	dec	eax
	or	eax,FE
	inc	eax

l004012F2:
	test	eax,eax
	jnz	0040130D

l004012F6:
	fld	double ptr [ebp+08]
	fstp	dword ptr [ebp-08]
	fld	dword ptr [ebp-08]
	push	ecx
	fstp	dword ptr [esp]
	call	00401230
	add	esp,04
	jmp	0040131E

l0040130D:
	sub	esp,08
	fld	double ptr [ebp+08]
	fstp	double ptr [esp]
	call	004011B0
	add	esp,08

l0040131E:
	mov	ecx,[ebp-04]
	sub	ecx,01
	mov	[ebp-04],ecx
	jmp	004012DD

l00401329:
	mov	esp,ebp
	pop	ebp
	ret	
0040132D                                        CC CC CC              ...
00401330 68 B9 16 40 00 E8 49 03 00 00 A1 4C 30 40 00 C7 h..@..I....L0@..
00401340 04 24 3C 30 40 00 FF 35 48 30 40 00 A3 3C 30 40 .$<0@..5H0@..<0@
00401350 00 68 2C 30 40 00 68 30 30 40 00 68 28 30 40 00 .h,0@.h00@.h(0@.
00401360 FF 15 90 20 40 00 83 C4 14 85 C0 A3 38 30 40 00 ... @.......80@.
00401370 7D 08 6A 08 E8 65 02 00 00 59 C3 6A 10 68 68 21 }.j..e...Y.j.hh!
00401380 40 00 E8 55 04 00 00 33 DB 89 5D FC 64 A1 18 00 @..U...3..].d...
00401390 00 00 8B 70 04 89 5D E4 BF 84 33 40 00 53 56 57 ...p..]...3@.SVW
004013A0 FF 15 24 20 40 00 3B C3 74 19 3B C6 75 08 33 F6 ..$ @.;.t.;.u.3.
004013B0 46 89 75 E4 EB 10 68 E8 03 00 00 FF 15 28 20 40 F.u...h......( @
004013C0 00 EB DA 33 F6 46 A1 80 33 40 00 3B C6 75 0A 6A ...3.F..3@.;.u.j
004013D0 1F E8 08 02 00 00 59 EB 3B A1 80 33 40 00 85 C0 ......Y.;..3@...
004013E0 75 2C 89 35 80 33 40 00 68 B8 20 40 00 68 B0 20 u,.5.3@.h. @.h. 
004013F0 40 00 E8 DD 03 00 00 59 59 85 C0 74 17 C7 45 FC @......YY..t..E.
00401400 FE FF FF FF B8 FF 00 00 00 E9 DD 00 00 00 89 35 ...............5
00401410 44 30 40 00 A1 80 33 40 00 3B C6 75 1B 68 AC 20 D0@...3@.;.u.h. 
00401420 40 00 68 A4 20 40 00 E8 A2 03 00 00 59 59 C7 05 @.h. @......YY..
00401430 80 33 40 00 02 00 00 00 39 5D E4 75 08 53 57 FF .3@.....9].u.SW.
00401440 15 2C 20 40 00 39 1D 90 33 40 00 74 19 68 90 33 ., @.9..3@.t.h.3
00401450 40 00 E8 0B 03 00 00 59 85 C0 74 0A 53 6A 02 53 @......Y..t.Sj.S
00401460 FF 15 90 33 40 00 A1 2C 30 40 00 8B 0D 7C 20 40 ...3@..,0@...| @
00401470 00 89 01 FF 35 2C 30 40 00 FF 35 30 30 40 00 FF ....5,0@..500@..
00401480 35 28 30 40 00 E8 76 FB FF FF 83 C4 0C A3 40 30 5(0@..v.......@0
00401490 40 00 39 1D 34 30 40 00 75 37 50 FF 15 80 20 40 @.9.40@.u7P... @
004014A0 00 8B 45 EC 8B 08 8B 09 89 4D E0 50 51 E8 2C 02 ..E......M.PQ.,.
004014B0 00 00 59 59 C3 8B 65 E8 8B 45 E0 A3 40 30 40 00 ..YY..e..E..@0@.
004014C0 33 DB 39 1D 34 30 40 00 75 07 50 FF 15 88 20 40 3.9.40@.u.P... @
004014D0 00 39 1D 44 30 40 00 75 06 FF 15 8C 20 40 00 C7 .9.D0@.u.... @..
004014E0 45 FC FE FF FF FF A1 40 30 40 00 E8 31 03 00 00 E......@0@..1...
004014F0 C3 66 81 3D 00 00 40 00 4D 5A 74 04 33 C0 EB 51 .f.=..@.MZt.3..Q
00401500 A1 3C 00 40 00 81 B8 00 00 40 00 50 45 00 00 75 .<.@.....@.PE..u
00401510 EB 0F B7 88 18 00 40 00 81 F9 0B 01 00 00 74 1B ......@.......t.
00401520 81 F9 0B 02 00 00 75 D4 83 B8 84 00 40 00 0E 76 ......u.....@..v
00401530 CB 33 C9 39 88 F8 00 40 00 EB 11 83 B8 74 00 40 .3.9...@.....t.@
00401540 00 0E 76 B8 33 C9 39 88 E8 00 40 00 0F 95 C1 8B ..v.3.9...@.....
00401550 C1 6A 01 A3 34 30 40 00 FF 15 40 20 40 00 6A FF .j..40@...@ @.j.
00401560 FF 15 3C 20 40 00 59 59 A3 88 33 40 00 A3 8C 33 ..< @.YY..3@...3
00401570 40 00 FF 15 38 20 40 00 8B 0D 54 30 40 00 89 08 @...8 @...T0@...
00401580 FF 15 4C 20 40 00 8B 0D 50 30 40 00 89 08 A1 68 ..L @...P0@....h
00401590 20 40 00 8B 00 A3 7C 33 40 00 E8 F6 00 00 00 E8  @....|3@.......
004015A0 DD 02 00 00 83 3D 0C 30 40 00 00 75 0C 68 81 18 .....=.0@..u.h..
004015B0 40 00 FF 15 6C 20 40 00 59 E8 9A 02 00 00 83 3D @...l @.Y......=
004015C0 08 30 40 00 FF 75 09 6A FF FF 15 70 20 40 00 59 .0@..u.j...p @.Y
004015D0 33 C0 C3 E8 AC 02 00 00 E9 9E FD FF FF CC FF 25 3..............%
004015E0 94 20 40 00                                     . @.           

;; fn004015E4: 004015E4
fn004015E4 proc
	push	14
	push	00402188
	call	004017DC
	push	dword ptr [0040338C]
	mov	esi,[00402054]
	call	esi
	pop	ecx
	mov	[ebp-1C],eax
	cmp	eax,FF
	jnz	00401613

l00401607:
	push	dword ptr [ebp+08]
	call	dword ptr [00402050]
	pop	ecx
	jmp	00401674

l00401613:
	push	08
	call	00401924
	pop	ecx
	and	dword ptr [ebp-04],00
	push	dword ptr [0040338C]
	call	esi
	mov	[ebp-1C],eax
	push	dword ptr [00403388]
	call	esi
	mov	[ebp-20],eax
	lea	eax,[ebp-20]
	push	eax
	lea	eax,[ebp-1C]
	push	eax
	push	dword ptr [ebp+08]
	call	0040191E
	mov	[ebp-24],eax
	push	dword ptr [ebp-1C]
	mov	esi,[0040203C]
	call	esi
	mov	[0040338C],eax
	push	dword ptr [ebp-20]
	call	esi
	add	esp,1C
	mov	[00403388],eax
	mov	dword ptr [ebp-04],FFFFFFFE
	call	0040167A
	mov	eax,[ebp-24]

l00401674:
	call	00401821
	ret	

;; fn0040167A: 0040167A
fn0040167A proc
	push	08
	call	00401918
	pop	ecx
	ret	

;; fn00401683: 00401683
fn00401683 proc
	push	dword ptr [esp+04]
	call	004015E4
	neg	eax
	sbb	eax,eax
	neg	eax
	pop	ecx
	dec	eax
	ret	

;; fn00401695: 00401695
fn00401695 proc
	push	esi
	push	edi
	mov	eax,00402158
	mov	edi,00402158
	cmp	eax,edi
	mov	esi,eax
	jnc	004016B6

l004016A7:
	mov	eax,[esi]
	test	eax,eax
	jz	004016AF

l004016AD:
	call	eax

l004016AF:
	add	esi,04
	cmp	esi,edi
	jc	004016A7

l004016B6:
	pop	edi
	pop	esi
	ret	
004016B9                            56 57 B8 60 21 40 00          VW.`!@.
004016C0 BF 60 21 40 00 3B C7 8B F0 73 0F 8B 06 85 C0 74 .`!@.;...s.....t
004016D0 02 FF D0 83 C6 04 3B F7 72 F1 5F 5E C3 CC FF 25 ......;.r._^...%
004016E0 84 20 40 00 CC CC CC CC CC CC CC CC CC CC CC CC . @.............

;; fn004016F0: 004016F0
fn004016F0 proc
	mov	ecx,[esp+04]
	cmp	word ptr [ecx],5A4D
	jz	004016FE

l004016FB:
	xor	eax,eax
	ret	

l004016FE:
	mov	eax,[ecx+3C]
	add	eax,ecx
	cmp	dword ptr [eax],00004550
	jnz	004016FB

l0040170B:
	xor	ecx,ecx
	cmp	word ptr [eax+18],010B
	setz	cl
	mov	eax,ecx
	ret	
00401719                            CC CC CC CC CC CC CC          .......

;; fn00401720: 00401720
fn00401720 proc
	mov	eax,[esp+04]
	mov	ecx,[eax+3C]
	add	ecx,eax
	movzx	eax,word ptr [ecx+14]
	push	ebx
	push	esi
	movzx	esi,word ptr [ecx+06]
	xor	edx,edx
	test	esi,esi
	push	edi
	lea	eax,[eax+ecx+18]
	jbe	0040175C

l0040173E:
	mov	edi,[esp+14]

l00401742:
	mov	ecx,[eax+0C]
	cmp	edi,ecx
	jc	00401752

l00401749:
	mov	ebx,[eax+08]
	add	ebx,ecx
	cmp	edi,ebx
	jc	0040175E

l00401752:
	add	edx,01
	add	eax,28
	cmp	edx,esi
	jc	00401742

l0040175C:
	xor	eax,eax

l0040175E:
	pop	edi
	pop	esi
	pop	ebx
	ret	

;; fn00401762: 00401762
fn00401762 proc
	push	08
	push	004021A8
	call	004017DC
	and	dword ptr [ebp-04],00
	mov	edx,00400000
	push	edx
	call	004016F0
	pop	ecx
	test	eax,eax
	jz	004017BF

l00401782:
	mov	eax,[ebp+08]
	sub	eax,edx
	push	eax
	push	edx
	call	00401720
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	004017BF

l00401794:
	mov	eax,[eax+24]
	shr	eax,1F
	not	eax
	and	eax,01
	mov	dword ptr [ebp-04],FFFFFFFE
	jmp	004017C8
004017A8                         8B 45 EC 8B 00 8B 00 33         .E.....3
004017B0 C9 3D 05 00 00 C0 0F 94 C1 8B C1 C3 8B 65 E8    .=...........e.

l004017BF:
	mov	dword ptr [ebp-04],FFFFFFFE
	xor	eax,eax

l004017C8:
	call	00401821
	ret	
004017CE                                           FF 25               .%
004017D0 78 20 40 00 FF 25 74 20 40 00 CC CC             x @..%t @...   

;; fn004017DC: 004017DC
fn004017DC proc
	push	00401835
	push	dword ptr fs:[00000000]
	mov	eax,[esp+10]
	mov	[esp+10],ebp
	lea	ebp,[esp+10]
	sub	esp,eax
	push	ebx
	push	esi
	push	edi
	mov	eax,[00403010]
	xor	[ebp-04],eax
	xor	eax,ebp
	push	eax
	mov	[ebp-18],esp
	push	dword ptr [ebp-08]
	mov	eax,[ebp-04]
	mov	dword ptr [ebp-04],FFFFFFFE
	mov	[ebp-08],eax
	lea	eax,[ebp-10]
	mov	fs:[00000000],eax
	ret	

;; fn00401821: 00401821
fn00401821 proc
	mov	ecx,[ebp-10]
	mov	fs:[00000000],ecx
	pop	ecx
	pop	edi
	pop	edi
	pop	esi
	pop	ebx
	mov	esp,ebp
	pop	ebp
	push	ecx
	ret	
00401835                FF 74 24 10 FF 74 24 10 FF 74 24      .t$..t$..t$
00401840 10 FF 74 24 10 68 2A 19 40 00 68 10 30 40 00 E8 ..t$.h*.@.h.0@..
00401850 E6 00 00 00 83 C4 18 C3                         ........       

;; fn00401858: 00401858
fn00401858 proc
	push	esi
	push	00030000
	push	00010000
	xor	esi,esi
	push	esi
	call	00401946
	add	esp,0C
	test	eax,eax
	jz	0040187F

l00401872:
	push	esi
	push	esi
	push	esi
	push	esi
	push	esi
	call	00401940
	add	esp,14

l0040187F:
	pop	esi
	ret	

;; fn00401881: 00401881
fn00401881 proc
	xor	eax,eax
	ret	

;; fn00401884: 00401884
fn00401884 proc
	push	ebp
	mov	ebp,esp
	sub	esp,10
	mov	eax,[00403010]
	and	dword ptr [ebp-08],00
	and	dword ptr [ebp-04],00
	push	ebx
	push	edi
	mov	edi,BB40E64E
	cmp	eax,edi
	mov	ebx,FFFF0000
	jz	004018B4

l004018A7:
	test	ebx,eax
	jz	004018B4

l004018AB:
	not	eax
	mov	[00403014],eax
	jmp	00401914

l004018B4:
	push	esi
	lea	eax,[ebp-08]
	push	eax
	call	dword ptr [00402010]
	mov	esi,[ebp-04]
	xor	esi,[ebp-08]
	call	dword ptr [00402014]
	xor	esi,eax
	call	dword ptr [00402018]
	xor	esi,eax
	call	dword ptr [0040201C]
	xor	esi,eax
	lea	eax,[ebp-10]
	push	eax
	call	dword ptr [00402020]
	mov	eax,[ebp-0C]
	xor	eax,[ebp-10]
	xor	esi,eax
	cmp	esi,edi
	jnz	004018FA

l004018F3:
	mov	esi,BB40E64F
	jmp	00401905

l004018FA:
	test	ebx,esi
	jnz	00401905

l004018FE:
	mov	eax,esi
	shl	eax,10
	or	esi,eax

l00401905:
	mov	[00403010],esi
	not	esi
	mov	[00403014],esi
	pop	esi

l00401914:
	pop	edi
	pop	ebx
	leave	
	ret	
00401918                         FF 25 44 20 40 00 FF 25         .%D @..%
00401920 48 20 40 00 FF 25 98 20 40 00 3B 0D 10 30 40 00 H @..%. @.;..0@.
00401930 75 02 F3 C3 E9 13 00 00 00 CC FF 25 58 20 40 00 u..........%X @.
00401940 FF 25 5C 20 40 00 FF 25 60 20 40 00 55 8B EC 81 .%\ @..%` @.U...
00401950 EC 28 03 00 00 A3 60 31 40 00 89 0D 5C 31 40 00 .(....`1@...\1@.
00401960 89 15 58 31 40 00 89 1D 54 31 40 00 89 35 50 31 ..X1@...T1@..5P1
00401970 40 00 89 3D 4C 31 40 00 66 8C 15 78 31 40 00 66 @..=L1@.f..x1@.f
00401980 8C 0D 6C 31 40 00 66 8C 1D 48 31 40 00 66 8C 05 ..l1@.f..H1@.f..
00401990 44 31 40 00 66 8C 25 40 31 40 00 66 8C 2D 3C 31 D1@.f.%@1@.f.-<1
004019A0 40 00 9C 8F 05 70 31 40 00 8B 45 00 A3 64 31 40 @....p1@..E..d1@
004019B0 00 8B 45 04 A3 68 31 40 00 8D 45 08 A3 74 31 40 ..E..h1@..E..t1@
004019C0 00 8B 85 E0 FC FF FF C7 05 B0 30 40 00 01 00 01 ..........0@....
004019D0 00 A1 68 31 40 00 A3 64 30 40 00 C7 05 58 30 40 ..h1@..d0@...X0@
004019E0 00 09 04 00 C0 C7 05 5C 30 40 00 01 00 00 00 A1 .......\0@......
004019F0 10 30 40 00 89 85 D8 FC FF FF A1 14 30 40 00 89 .0@.........0@..
00401A00 85 DC FC FF FF FF 15 30 20 40 00 A3 A8 30 40 00 .......0 @...0@.
00401A10 6A 01 E8 39 00 00 00 59 6A 00 FF 15 00 20 40 00 j..9...Yj.... @.
00401A20 68 00 21 40 00 FF 15 04 20 40 00 83 3D A8 30 40 h.!@.... @..=.0@
00401A30 00 00 75 08 6A 01 E8 15 00 00 00 59 68 09 04 00 ..u.j......Yh...
00401A40 C0 FF 15 08 20 40 00 50 FF 15 0C 20 40 00 C9 C3 .... @.P... @...
00401A50 FF 25 64 20 40 00                               .%d @.         
;;; Segment .rdata (00402000)
__imp__SetUnhandledExceptionFilter		; 00402000
	dd	0x00002520
__imp__UnhandledExceptionFilter		; 00402004
	dd	0x00002504
__imp__GetCurrentProcess		; 00402008
	dd	0x000024F0
__imp__TerminateProcess		; 0040200C
	dd	0x000024DC
__imp__GetSystemTimeAsFileTime		; 00402010
	dd	0x000024C2
__imp__GetCurrentProcessId		; 00402014
	dd	0x000024AC
__imp__GetCurrentThreadId		; 00402018
	dd	0x00002496
__imp__GetTickCount		; 0040201C
	dd	0x00002486
__imp__QueryPerformanceCounter		; 00402020
	dd	0x0000246C
__imp__InterlockedCompareExchange		; 00402024
	dd	0x0000244E
__imp__Sleep		; 00402028
	dd	0x00002446
__imp__InterlockedExchange		; 0040202C
	dd	0x00002430
__imp__IsDebuggerPresent		; 00402030
	dd	0x0000253E
00402034             00 00 00 00                             ....       
__imp____p__fmode		; 00402038
	dd	0x00002370
__imp___encode_pointer		; 0040203C
	dd	0x0000237E
__imp____set_app_type		; 00402040
	dd	0x00002390
__imp___unlock		; 00402044
	dd	0x000023A2
__imp____dllonexit		; 00402048
	dd	0x000023AC
__imp____p__commode		; 0040204C
	dd	0x00002360
__imp___onexit		; 00402050
	dd	0x000023C2
__imp___decode_pointer		; 00402054
	dd	0x000023CC
__imp___except_handler4_common		; 00402058
	dd	0x000023DE
__imp___invoke_watson		; 0040205C
	dd	0x000023F8
__imp___controlfp_s		; 00402060
	dd	0x0000240A
__imp___crt_debugger_hook		; 00402064
	dd	0x0000241A
__imp___adjust_fdiv		; 00402068
	dd	0x00002350
__imp____setusermatherr		; 0040206C
	dd	0x0000233C
__imp___configthreadlocale		; 00402070
	dd	0x00002326
__imp___initterm_e		; 00402074
	dd	0x00002318
__imp___initterm		; 00402078
	dd	0x0000230C
__imp____initenv		; 0040207C
	dd	0x00002300
__imp__exit		; 00402080
	dd	0x000022F8
__imp___XcptFilter		; 00402084
	dd	0x000022EA
__imp___exit		; 00402088
	dd	0x000022E2
__imp___cexit		; 0040208C
	dd	0x000022D8
__imp____getmainargs		; 00402090
	dd	0x000022C8
__imp___amsg_exit		; 00402094
	dd	0x000022BA
__imp___lock		; 00402098
	dd	0x000023BA
__imp__printf		; 0040209C
	dd	0x000022A4
004020A0 00 00 00 00 00 00 00 00 30 13 40 00 00 00 00 00 ........0.@.....
004020B0 00 00 00 00 F1 14 40 00 00 00 00 00 00 00 00 00 ......@.........
004020C0 74 65 73 74 31 32 33 00 25 73 20 25 64 20 25 73 test123.%s %d %s
004020D0 20 25 66 00 33 00 00 00 31 00 00 00 37 00 00 00  %f.3...1...7...
004020E0 35 00 00 00 6F 12 09 41 9E EF 83 40 66 06 7A 44 5...o..A...@f.zD
004020F0 00 00 00 00 00 10 74 40 00 00 00 00 00 C0 5E 40 ......t@......^@
00402100 58 30 40 00 B0 30 40 00 48 00 00 00 00 00 00 00 X0@..0@.H.......
00402110 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00402140 00 00 00 00 10 30 40 00 50 21 40 00 01 00 00 00 .....0@.P!@.....
00402150 35 18 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5...............
00402160 00 00 00 00 00 00 00 00 FE FF FF FF 00 00 00 00 ................
00402170 D0 FF FF FF 00 00 00 00 FE FF FF FF A1 14 40 00 ..............@.
00402180 B5 14 40 00 00 00 00 00 FE FF FF FF 00 00 00 00 ..@.............
00402190 CC FF FF FF 00 00 00 00 FE FF FF FF 00 00 00 00 ................
004021A0 7A 16 40 00 00 00 00 00 FE FF FF FF 00 00 00 00 z.@.............
004021B0 D8 FF FF FF 00 00 00 00 FE FF FF FF A8 17 40 00 ..............@.
004021C0 BC 17 40 00 38 22 00 00 00 00 00 00 00 00 00 00 ..@.8"..........
004021D0 AE 22 00 00 38 20 00 00 00 22 00 00 00 00 00 00 ."..8 ..."......
004021E0 00 00 00 00 52 25 00 00 00 20 00 00 00 00 00 00 ....R%... ......
004021F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
l00402200	dd	0x00002520
l00402204	dd	0x00002504
l00402208	dd	0x000024F0
l0040220C	dd	0x000024DC
l00402210	dd	0x000024C2
l00402214	dd	0x000024AC
l00402218	dd	0x00002496
l0040221C	dd	0x00002486
l00402220	dd	0x0000246C
l00402224	dd	0x0000244E
l00402228	dd	0x00002446
l0040222C	dd	0x00002430
l00402230	dd	0x0000253E
00402234             00 00 00 00                             ....       
l00402238	dd	0x00002370
l0040223C	dd	0x0000237E
l00402240	dd	0x00002390
l00402244	dd	0x000023A2
l00402248	dd	0x000023AC
l0040224C	dd	0x00002360
l00402250	dd	0x000023C2
l00402254	dd	0x000023CC
l00402258	dd	0x000023DE
l0040225C	dd	0x000023F8
l00402260	dd	0x0000240A
l00402264	dd	0x0000241A
l00402268	dd	0x00002350
l0040226C	dd	0x0000233C
l00402270	dd	0x00002326
l00402274	dd	0x00002318
l00402278	dd	0x0000230C
l0040227C	dd	0x00002300
l00402280	dd	0x000022F8
l00402284	dd	0x000022EA
l00402288	dd	0x000022E2
l0040228C	dd	0x000022D8
l00402290	dd	0x000022C8
l00402294	dd	0x000022BA
l00402298	dd	0x000023BA
l0040229C	dd	0x000022A4
004022A0 00 00 00 00 37 05 70 72 69 6E 74 66 00 00 4D 53 ....7.printf..MS
004022B0 56 43 52 38 30 2E 64 6C 6C 00 18 01 5F 61 6D 73 VCR80.dll..._ams
004022C0 67 5F 65 78 69 74 00 00 A0 00 5F 5F 67 65 74 6D g_exit....__getm
004022D0 61 69 6E 61 72 67 73 00 2F 01 5F 63 65 78 69 74 ainargs./._cexit
004022E0 00 00 7F 01 5F 65 78 69 74 00 67 00 5F 58 63 70 ...._exit.g._Xcp
004022F0 74 46 69 6C 74 65 72 00 D6 04 65 78 69 74 00 00 tFilter...exit..
00402300 A1 00 5F 5F 69 6E 69 74 65 6E 76 00 0A 02 5F 69 ..__initenv..._i
00402310 6E 69 74 74 65 72 6D 00 0B 02 5F 69 6E 69 74 74 nitterm..._initt
00402320 65 72 6D 5F 65 00 3F 01 5F 63 6F 6E 66 69 67 74 erm_e.?._configt
00402330 68 72 65 61 64 6C 6F 63 61 6C 65 00 E9 00 5F 5F hreadlocale...__
00402340 73 65 74 75 73 65 72 6D 61 74 68 65 72 72 00 00 setusermatherr..
00402350 11 01 5F 61 64 6A 75 73 74 5F 66 64 69 76 00 00 .._adjust_fdiv..
00402360 CC 00 5F 5F 70 5F 5F 63 6F 6D 6D 6F 64 65 00 00 ..__p__commode..
00402370 D0 00 5F 5F 70 5F 5F 66 6D 6F 64 65 00 00 6D 01 ..__p__fmode..m.
00402380 5F 65 6E 63 6F 64 65 5F 70 6F 69 6E 74 65 72 00 _encode_pointer.
00402390 E6 00 5F 5F 73 65 74 5F 61 70 70 5F 74 79 70 65 ..__set_app_type
004023A0 00 00 ED 03 5F 75 6E 6C 6F 63 6B 00 97 00 5F 5F ...._unlock...__
004023B0 64 6C 6C 6F 6E 65 78 69 74 00 7C 02 5F 6C 6F 63 dllonexit.|._loc
004023C0 6B 00 22 03 5F 6F 6E 65 78 69 74 00 63 01 5F 64 k."._onexit.c._d
004023D0 65 63 6F 64 65 5F 70 6F 69 6E 74 65 72 00 76 01 ecode_pointer.v.
004023E0 5F 65 78 63 65 70 74 5F 68 61 6E 64 6C 65 72 34 _except_handler4
004023F0 5F 63 6F 6D 6D 6F 6E 00 11 02 5F 69 6E 76 6F 6B _common..._invok
00402400 65 5F 77 61 74 73 6F 6E 00 00 42 01 5F 63 6F 6E e_watson..B._con
00402410 74 72 6F 6C 66 70 5F 73 00 00 4E 01 5F 63 72 74 trolfp_s..N._crt
00402420 5F 64 65 62 75 67 67 65 72 5F 68 6F 6F 6B 00 00 _debugger_hook..
00402430 29 02 49 6E 74 65 72 6C 6F 63 6B 65 64 45 78 63 ).InterlockedExc
00402440 68 61 6E 67 65 00 56 03 53 6C 65 65 70 00 26 02 hange.V.Sleep.&.
00402450 49 6E 74 65 72 6C 6F 63 6B 65 64 43 6F 6D 70 61 InterlockedCompa
00402460 72 65 45 78 63 68 61 6E 67 65 00 00 A3 02 51 75 reExchange....Qu
00402470 65 72 79 50 65 72 66 6F 72 6D 61 6E 63 65 43 6F eryPerformanceCo
00402480 75 6E 74 65 72 00 DF 01 47 65 74 54 69 63 6B 43 unter...GetTickC
00402490 6F 75 6E 74 00 00 46 01 47 65 74 43 75 72 72 65 ount..F.GetCurre
004024A0 6E 74 54 68 72 65 61 64 49 64 00 00 43 01 47 65 ntThreadId..C.Ge
004024B0 74 43 75 72 72 65 6E 74 50 72 6F 63 65 73 73 49 tCurrentProcessI
004024C0 64 00 CA 01 47 65 74 53 79 73 74 65 6D 54 69 6D d...GetSystemTim
004024D0 65 41 73 46 69 6C 65 54 69 6D 65 00 5E 03 54 65 eAsFileTime.^.Te
004024E0 72 6D 69 6E 61 74 65 50 72 6F 63 65 73 73 00 00 rminateProcess..
004024F0 42 01 47 65 74 43 75 72 72 65 6E 74 50 72 6F 63 B.GetCurrentProc
00402500 65 73 73 00 6E 03 55 6E 68 61 6E 64 6C 65 64 45 ess.n.UnhandledE
00402510 78 63 65 70 74 69 6F 6E 46 69 6C 74 65 72 00 00 xceptionFilter..
00402520 4A 03 53 65 74 55 6E 68 61 6E 64 6C 65 64 45 78 J.SetUnhandledEx
00402530 63 65 70 74 69 6F 6E 46 69 6C 74 65 72 00 39 02 ceptionFilter.9.
00402540 49 73 44 65 62 75 67 67 65 72 50 72 65 73 65 6E IsDebuggerPresen
00402550 74 00 4B 45 52 4E 45 4C 33 32 2E 64 6C 6C 00 00 t.KERNEL32.dll..
00402560 00 00 00 00 1A 60 8B 58 00 00 00 00 EC 25 00 00 .....`.X.....%..
00402570 01 00 00 00 0A 00 00 00 0A 00 00 00 88 25 00 00 .............%..
00402580 B0 25 00 00 D8 25 00 00 A0 12 00 00 D0 12 00 00 .%...%..........
00402590 30 12 00 00 B0 11 00 00 60 10 00 00 B0 10 00 00 0.......`.......
004025A0 D0 10 00 00 F0 10 00 00 20 11 00 00 60 11 00 00 ........ ...`...
004025B0 FC 25 00 00 0D 26 00 00 19 26 00 00 24 26 00 00 .%...&...&..$&..
004025C0 3B 26 00 00 41 26 00 00 47 26 00 00 4D 26 00 00 ;&..A&..G&..M&..
004025D0 53 26 00 00 59 26 00 00 00 00 01 00 02 00 03 00 S&..Y&..........
004025E0 04 00 05 00 06 00 07 00 08 00 09 00 56 43 45 78 ............VCEx
004025F0 65 53 61 6D 70 6C 65 2E 65 78 65 00 63 6F 6E 73 eSample.exe.cons
00402600 74 5F 64 69 76 5F 74 65 73 74 31 30 00 6C 6F 6F t_div_test10.loo
00402610 70 5F 74 65 73 74 31 31 00 6C 6F 6F 70 5F 74 65 p_test11.loop_te
00402620 73 74 39 00 6E 65 73 74 65 64 5F 69 66 5F 62 6C st9.nested_if_bl
00402630 6F 63 6B 73 5F 74 65 73 74 38 00 74 65 73 74 32 ocks_test8.test2
00402640 00 74 65 73 74 33 00 74 65 73 74 34 00 74 65 73 .test3.test4.tes
00402650 74 35 00 74 65 73 74 36 00 74 65 73 74 37 00    t5.test6.test7.
;;; Segment .data (00403000)
00403000 FF FF FF FF FF FF FF FF FE FF FF FF 01 00 00 00 ................
00403010 4E E6 40 BB B1 19 BF 44                         N.@....D       
l00403018	dd	0x00000000
0040301C                                     00 00 00 00             ....
; ...
l00403024	dd	0x00000000
00403028                         00 00 00 00 00 00 00 00         ........
00403030 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00403390 00 00 00 00                                     ....           
;;; Segment .rsrc (00404000)
00404000 00 00 00 00 00 00 00 00 04 00 00 00 00 00 01 00 ................
00404010 18 00 00 00 18 00 00 80 00 00 00 00 00 00 00 00 ................
00404020 04 00 00 00 00 00 01 00 01 00 00 00 30 00 00 80 ............0...
00404030 00 00 00 00 00 00 00 00 04 00 00 00 00 00 01 00 ................
00404040 09 04 00 00 48 00 00 00 58 40 00 00 52 01 00 00 ....H...X@..R...
00404050 E4 04 00 00 00 00 00 00 3C 61 73 73 65 6D 62 6C ........<assembl
00404060 79 20 78 6D 6C 6E 73 3D 22 75 72 6E 3A 73 63 68 y xmlns="urn:sch
00404070 65 6D 61 73 2D 6D 69 63 72 6F 73 6F 66 74 2D 63 emas-microsoft-c
00404080 6F 6D 3A 61 73 6D 2E 76 31 22 20 6D 61 6E 69 66 om:asm.v1" manif
00404090 65 73 74 56 65 72 73 69 6F 6E 3D 22 31 2E 30 22 estVersion="1.0"
004040A0 3E 0D 0A 20 20 3C 64 65 70 65 6E 64 65 6E 63 79 >..  <dependency
004040B0 3E 0D 0A 20 20 20 20 3C 64 65 70 65 6E 64 65 6E >..    <dependen
004040C0 74 41 73 73 65 6D 62 6C 79 3E 0D 0A 20 20 20 20 tAssembly>..    
004040D0 20 20 3C 61 73 73 65 6D 62 6C 79 49 64 65 6E 74   <assemblyIdent
004040E0 69 74 79 20 74 79 70 65 3D 22 77 69 6E 33 32 22 ity type="win32"
004040F0 20 6E 61 6D 65 3D 22 4D 69 63 72 6F 73 6F 66 74  name="Microsoft
00404100 2E 56 43 38 30 2E 43 52 54 22 20 76 65 72 73 69 .VC80.CRT" versi
00404110 6F 6E 3D 22 38 2E 30 2E 35 30 36 30 38 2E 30 22 on="8.0.50608.0"
00404120 20 70 72 6F 63 65 73 73 6F 72 41 72 63 68 69 74  processorArchit
00404130 65 63 74 75 72 65 3D 22 78 38 36 22 20 70 75 62 ecture="x86" pub
00404140 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 22 31 66 63 licKeyToken="1fc
00404150 38 62 33 62 39 61 31 65 31 38 65 33 62 22 3E 3C 8b3b9a1e18e3b"><
00404160 2F 61 73 73 65 6D 62 6C 79 49 64 65 6E 74 69 74 /assemblyIdentit
00404170 79 3E 0D 0A 20 20 20 20 3C 2F 64 65 70 65 6E 64 y>..    </depend
00404180 65 6E 74 41 73 73 65 6D 62 6C 79 3E 0D 0A 20 20 entAssembly>..  
00404190 3C 2F 64 65 70 65 6E 64 65 6E 63 79 3E 0D 0A 3C </dependency>..<
004041A0 2F 61 73 73 65 6D 62 6C 79 3E 50 41             /assembly>PA   
