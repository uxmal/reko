;;; Segment .text (10001000)

;; fn10001000: 10001000
fn10001000 proc
	mov	edx,[esp+08]
	sub	esp,08
	lea	eax,[esp]
	push	eax
	lea	ecx,[esp+08]
	push	ecx
	push	10002144
	push	edx
	call	dword ptr [10002088]
	add	esp,10
	test	eax,eax
	jnz	10001027

l10001023:
	add	esp,08
	ret

l10001027:
	mov	eax,[esp]
	mov	ecx,[esp+04]
	add	ecx,eax
	push	ecx
	push	1000214C
	call	dword ptr [10002090]
	add	esp,08
	add	esp,08
	ret
10001043          CC CC CC CC CC CC CC CC CC CC CC CC CC    .............

;; fn10001050: 10001050
fn10001050 proc
	mov	edx,[esp+08]
	sub	esp,08
	lea	eax,[esp+04]
	push	eax
	lea	ecx,[esp+04]
	push	ecx
	push	10002150
	push	edx
	call	dword ptr [10002088]
	add	esp,10
	test	eax,eax
	jnz	10001078

l10001074:
	add	esp,08
	ret

l10001078:
	mov	eax,[esp]
	sub	eax,[esp+04]
	push	eax
	push	1000214C
	call	dword ptr [10002090]
	add	esp,08
	add	esp,08
	ret
10001092       CC CC CC CC CC CC CC CC CC CC CC CC CC CC   ..............

;; fn100010A0: 100010A0
fn100010A0 proc
	mov	edx,[esp+08]
	sub	esp,08
	lea	eax,[esp+04]
	push	eax
	lea	ecx,[esp+04]
	push	ecx
	push	10002158
	push	edx
	call	dword ptr [10002088]
	add	esp,10
	test	eax,eax
	jnz	100010C8

l100010C4:
	add	esp,08
	ret

l100010C8:
	mov	eax,[esp]
	cdq
	idiv	dword ptr [esp+04]
	push	eax
	push	1000214C
	call	dword ptr [10002090]
	add	esp,08
	add	esp,08
	ret
100010E3          CC CC CC CC CC CC CC CC CC CC CC CC CC    .............

;; fn100010F0: 100010F0
fn100010F0 proc
	mov	edx,[esp+08]
	sub	esp,08
	lea	eax,[esp+04]
	push	eax
	lea	ecx,[esp+04]
	push	ecx
	push	10002160
	push	edx
	call	dword ptr [10002088]
	add	esp,10
	test	eax,eax
	jnz	10001118

l10001114:
	add	esp,08
	ret

l10001118:
	fld	dword ptr [esp]
	sub	esp,08
	fdiv	dword ptr [esp+0C]
	fstp	double ptr [esp]
	push	10002168
	call	dword ptr [10002090]
	add	esp,0C
	add	esp,08
	ret
10001137                      CC CC CC CC CC CC CC CC CC        .........

;; py_unused: 10001140
py_unused proc
	mov	eax,[esp+08]
	push	1000216C
	push	eax
	call	dword ptr [10002088]
	add	esp,08
	test	eax,eax
	jnz	10001158

l10001157:
	ret

l10001158:
	mov	eax,[1000208C]
	add	dword ptr [eax],01
	mov	eax,[1000208C]
	ret
10001166                   CC CC CC CC CC CC CC CC CC CC       ..........

;; initpySample: 10001170
initpySample proc
	push	000003EF
	push	00
	push	00
	push	10003010
	push	10002174
	call	dword ptr [10002084]
	add	esp,14
	ret
1000118D                                        3B 0D 00              ;..
10001190 30 00 10 75 02 F3 C3 E9 23 03 00 00 56 68 80 00 0..u....#...Vh..
100011A0 00 00 FF 15 74 20 00 10 8B F0 56 FF 15 78 20 00 ....t ....V..x .
100011B0 10 85 F6 59 59 A3 B4 33 00 10 A3 B0 33 00 10 75 ...YY..3....3..u
100011C0 05 33 C0 40 5E C3 83 26 00 E8 B2 04 00 00 68 A4 .3.@^..&......h.
100011D0 16 00 10 E8 96 04 00 00 C7 04 24 C3 15 00 10 E8 ..........$.....
100011E0 8A 04 00 00 59 33 C0 5E C3                      ....Y3.^.      

;; fn100011E9: 100011E9
;;   Called from:
;;     100013EA (in fn10001388)
;;     1000141A (in fn10001388)
;;     1000143A (in fn10001388)
fn100011E9 proc
	mov	eax,[esp+08]
	push	ebp
	xor	ebp,ebp
	cmp	eax,ebp
	jnz	10001202

l100011F4:
	cmp	[10003070],ebp
	jle	10001233

l100011FC:
	dec	dword ptr [10003070]

l10001202:
	cmp	eax,01
	mov	ecx,[10002058]
	mov	ecx,[ecx]
	push	ebx
	push	esi
	push	edi
	mov	[100033A4],ecx
	jnz	100012E8

l1000121C:
	mov	eax,fs:[00000018]
	mov	edi,[eax+04]
	mov	ebx,[10002028]
	push	ebp
	mov	esi,100033AC
	jmp	1000124B

l10001233:
	xor	eax,eax
	jmp	10001384

l1000123A:
	cmp	eax,edi
	jz	10001255

l1000123E:
	push	000003E8
	call	dword ptr [1000202C]
	push	00

l1000124B:
	push	edi
	push	esi
	call	ebx
	test	eax,eax
	jnz	1000123A

l10001253:
	jmp	10001258

l10001255:
	xor	ebp,ebp
	inc	ebp

l10001258:
	mov	eax,[100033A8]
	test	eax,eax
	push	02
	pop	edi
	jz	1000126D

l10001264:
	push	1F
	call	100017BA
1000126B                                  EB 3C                     .<  

l1000126D:
	push	100020A8
	push	100020A0
	mov	dword ptr [100033A8],00000001
	call	100017B4
	test	eax,eax
	pop	ecx
	pop	ecx
	jz	10001293

l1000128C:
	xor	eax,eax
	jmp	10001381

l10001293:
	push	1000209C
	push	10002098
	call	100017AE
	pop	ecx
	mov	[100033A8],edi
	test	ebp,ebp
	pop	ecx
	jnz	100012B6

l100012AE:
	push	ebp
	push	esi
	call	dword ptr [10002030]

l100012B6:
	cmp	dword ptr [100033B8],00
	jz	100012DD

l100012BF:
	push	100033B8
	call	10001742
	test	eax,eax
	pop	ecx
	jz	100012DD

l100012CE:
	push	dword ptr [esp+1C]
	push	edi
	push	dword ptr [esp+1C]
	call	dword ptr [100033B8]

l100012DD:
	inc	dword ptr [10003070]
	jmp	1000137E

l100012E8:
	cmp	eax,ebp
	jnz	1000137E

l100012F0:
	mov	edi,[10002028]
	mov	esi,100033AC
	jmp	10001308

l100012FD:
	push	000003E8
	call	dword ptr [1000202C]

l10001308:
	push	ebp
	push	01
	push	esi
	call	edi
	test	eax,eax
	jnz	100012FD

l10001312:
	mov	eax,[100033A8]
	cmp	eax,02
	jz	10001326

l1000131C:
	push	1F
	call	100017BA
10001323          59 EB 58                                  Y.X         

l10001326:
	push	dword ptr [100033B4]
	mov	edi,[10002068]
	call	edi
	mov	ebx,eax
	test	ebx,ebx
	pop	ecx
	jz	1000136F

l1000133B:
	push	dword ptr [100033B0]
	call	edi
	pop	ecx
	mov	edi,eax
	jmp	10001350

l10001348:
	mov	eax,[edi]
	test	eax,eax
	jz	10001350

l1000134E:
	call	eax

l10001350:
	sub	edi,04
	cmp	edi,ebx
	jnc	10001348

l10001357:
	push	ebx
	call	dword ptr [1000206C]
	pop	ecx
	call	dword ptr [10002070]
	mov	[100033B0],eax
	mov	[100033B4],eax

l1000136F:
	push	00
	push	esi
	mov	[100033A8],ebp
	call	dword ptr [10002030]

l1000137E:
	xor	eax,eax
	inc	eax

l10001381:
	pop	edi
	pop	esi
	pop	ebx

l10001384:
	pop	ebp
	ret	000C

;; fn10001388: 10001388
;;   Called from:
;;     100014B6 (in DllMain)
fn10001388 proc
	push	10
	push	100021E8
	call	100017E8
	mov	edi,ecx
	mov	esi,edx
	mov	ebx,[ebp+08]
	xor	eax,eax
	inc	eax
	mov	[ebp-1C],eax
	xor	ecx,ecx
	mov	[ebp-04],ecx
	mov	[10003008],esi
	mov	[ebp-04],eax
	cmp	esi,ecx
	jnz	100013C3

l100013B3:
	cmp	[10003070],ecx
	jnz	100013C3

l100013BB:
	mov	[ebp-1C],ecx
	jmp	1000147A

l100013C3:
	cmp	esi,eax
	jz	100013CC

l100013C7:
	cmp	esi,02
	jnz	100013FA

l100013CC:
	mov	eax,[100020CC]
	cmp	eax,ecx
	jz	100013DD

l100013D5:
	push	edi
	push	esi
	push	ebx
	call	eax
	mov	[ebp-1C],eax

l100013DD:
	cmp	dword ptr [ebp-1C],00
	jz	1000147A

l100013E7:
	push	edi
	push	esi
	push	ebx
	call	100011E9
	mov	[ebp-1C],eax
	test	eax,eax
	jz	1000147A

l100013FA:
	push	edi
	push	esi
	push	ebx
	call	100017C6
	mov	[ebp-1C],eax
	cmp	esi,01
	jnz	1000142E

l1000140A:
	test	eax,eax
	jnz	1000142E

l1000140E:
	push	edi
	push	eax
	push	ebx
	call	100017C6
	push	edi
	push	00
	push	ebx
	call	100011E9
	mov	eax,[100020CC]
	test	eax,eax
	jz	1000142E

l10001428:
	push	edi
	push	00
	push	ebx
	call	eax

l1000142E:
	test	esi,esi
	jz	10001437

l10001432:
	cmp	esi,03
	jnz	1000147A

l10001437:
	push	edi
	push	esi
	push	ebx
	call	100011E9
	test	eax,eax
	jnz	10001446

l10001443:
	and	[ebp-1C],eax

l10001446:
	cmp	dword ptr [ebp-1C],00
	jz	1000147A

l1000144C:
	mov	eax,[100020CC]
	test	eax,eax
	jz	1000147A

l10001455:
	push	edi
	push	esi
	push	ebx
	call	eax
	mov	[ebp-1C],eax
	jmp	1000147A
1000145F                                              8B                .
10001460 45 EC 8B 08 8B 09 89 4D E0 50 51 E8 50 03 00 00 E......M.PQ.P...
10001470 59 59 C3 8B 65 E8 83 65 E4 00                   YY..e..e..     

l1000147A:
	and	dword ptr [ebp-04],00
	mov	dword ptr [ebp-04],FFFFFFFE
	call	10001493
	mov	eax,[ebp-1C]
	call	1000182D
	ret

;; fn10001493: 10001493
;;   Called from:
;;     10001485 (in fn10001388)
fn10001493 proc
	mov	dword ptr [10003008],FFFFFFFF
	ret

;; DllMain: 1000149E
DllMain proc
	cmp	dword ptr [esp+08],01
	jnz	100014AA

l100014A5:
	call	10001864

l100014AA:
	push	dword ptr [esp+04]
	mov	ecx,[esp+10]
	mov	edx,[esp+0C]
	call	10001388
	pop	ecx
	ret	000C
100014BF                                              55                U
100014C0 8B EC 81 EC 28 03 00 00 A3 80 31 00 10 89 0D 7C ....(.....1....|
100014D0 31 00 10 89 15 78 31 00 10 89 1D 74 31 00 10 89 1....x1....t1...
100014E0 35 70 31 00 10 89 3D 6C 31 00 10 66 8C 15 98 31 5p1...=l1..f...1
100014F0 00 10 66 8C 0D 8C 31 00 10 66 8C 1D 68 31 00 10 ..f...1..f..h1..
10001500 66 8C 05 64 31 00 10 66 8C 25 60 31 00 10 66 8C f..d1..f.%`1..f.
10001510 2D 5C 31 00 10 9C 8F 05 90 31 00 10 8B 45 00 A3 -\1......1...E..
10001520 84 31 00 10 8B 45 04 A3 88 31 00 10 8D 45 08 A3 .1...E...1...E..
10001530 94 31 00 10 8B 85 E0 FC FF FF C7 05 D0 30 00 10 .1...........0..
10001540 01 00 01 00 A1 88 31 00 10 A3 84 30 00 10 C7 05 ......1....0....
10001550 78 30 00 10 09 04 00 C0 C7 05 7C 30 00 10 01 00 x0........|0....
10001560 00 00 A1 00 30 00 10 89 85 D8 FC FF FF A1 04 30 ....0..........0
10001570 00 10 89 85 DC FC FF FF FF 15 14 20 00 10 A3 C8 ........... ....
10001580 30 00 10 6A 01 E8 6E 03 00 00 59 6A 00 FF 15 18 0..j..n...Yj....
10001590 20 00 10 68 D0 20 00 10 FF 15 1C 20 00 10 83 3D  ..h. ..... ...=
100015A0 C8 30 00 10 00 75 08 6A 01 E8 4A 03 00 00 59 68 .0...u.j..J...Yh
100015B0 09 04 00 C0 FF 15 20 20 00 10 50 FF 15 24 20 00 ......  ..P..$ .
100015C0 10 C9 C3 68 9C 33 00 10 E8 31 03 00 00 59 C3    ...h.3...1...Y.

;; fn100015CF: 100015CF
;;   Called from:
;;     10001672 (in fn1000166E)
fn100015CF proc
	push	14
	push	10002210
	call	100017E8
	push	dword ptr [100033B4]
	mov	esi,[10002068]
	call	esi
	pop	ecx
	mov	[ebp-1C],eax
	cmp	eax,FF
	jnz	100015FE

l100015F2:
	push	dword ptr [ebp+08]
	call	dword ptr [10002050]
	pop	ecx
	jmp	1000165F

l100015FE:
	push	08
	call	10001910
	pop	ecx
	and	dword ptr [ebp-04],00
	push	dword ptr [100033B4]
	call	esi
	mov	[ebp-1C],eax
	push	dword ptr [100033B0]
	call	esi
	mov	[ebp-20],eax
	lea	eax,[ebp-20]
	push	eax
	lea	eax,[ebp-1C]
	push	eax
	push	dword ptr [ebp+08]
	call	1000190A
	mov	[ebp-24],eax
	push	dword ptr [ebp-1C]
	mov	esi,[10002078]
	call	esi
	mov	[100033B4],eax
	push	dword ptr [ebp-20]
	call	esi
	add	esp,1C
	mov	[100033B0],eax
	mov	dword ptr [ebp-04],FFFFFFFE
	call	10001665
	mov	eax,[ebp-24]

l1000165F:
	call	1000182D
	ret

;; fn10001665: 10001665
;;   Called from:
;;     10001657 (in fn100015CF)
fn10001665 proc
	push	08
	call	10001904
	pop	ecx
	ret

;; fn1000166E: 1000166E
fn1000166E proc
	push	dword ptr [esp+04]
	call	100015CF
	neg	eax
	sbb	eax,eax
	neg	eax
	pop	ecx
	dec	eax
	ret

;; fn10001680: 10001680
fn10001680 proc
	push	esi
	push	edi
	mov	eax,100021D8
	mov	edi,100021D8
	cmp	eax,edi
	mov	esi,eax
	jnc	100016A1

l10001692:
	mov	eax,[esi]
	test	eax,eax
	jz	1000169A

l10001698:
	call	eax

l1000169A:
	add	esi,04
	cmp	esi,edi
	jc	10001692

l100016A1:
	pop	edi
	pop	esi
	ret
100016A4             56 57 B8 E0 21 00 10 BF E0 21 00 10     VW..!....!..
100016B0 3B C7 8B F0 73 0F 8B 06 85 C0 74 02 FF D0 83 C6 ;...s.....t.....
100016C0 04 3B F7 72 F1 5F 5E C3 CC CC CC CC CC CC CC CC .;.r._^.........

;; fn100016D0: 100016D0
;;   Called from:
;;     10001758 (in fn10001742)
fn100016D0 proc
	mov	ecx,[esp+04]
	cmp	word ptr [ecx],5A4D
	jz	100016DE

l100016DB:
	xor	eax,eax
	ret

l100016DE:
	mov	eax,[ecx+3C]
	add	eax,ecx
	cmp	dword ptr [eax],00004550
	jnz	100016DB

l100016EB:
	xor	ecx,ecx
	cmp	word ptr [eax+18],010B
	setz	cl
	mov	eax,ecx
	ret
100016F9                            CC CC CC CC CC CC CC          .......

;; fn10001700: 10001700
;;   Called from:
;;     10001769 (in fn10001742)
fn10001700 proc
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
	jbe	1000173C

l1000171E:
	mov	edi,[esp+14]

l10001722:
	mov	ecx,[eax+0C]
	cmp	edi,ecx
	jc	10001732

l10001729:
	mov	ebx,[eax+08]
	add	ebx,ecx
	cmp	edi,ebx
	jc	1000173E

l10001732:
	add	edx,01
	add	eax,28
	cmp	edx,esi
	jc	10001722

l1000173C:
	xor	eax,eax

l1000173E:
	pop	edi
	pop	esi
	pop	ebx
	ret

;; fn10001742: 10001742
;;   Called from:
;;     100012C4 (in fn100011E9)
fn10001742 proc
	push	08
	push	10002230
	call	100017E8
	and	dword ptr [ebp-04],00
	mov	edx,10000000
	push	edx
	call	100016D0
	pop	ecx
	test	eax,eax
	jz	1000179F

l10001762:
	mov	eax,[ebp+08]
	sub	eax,edx
	push	eax
	push	edx
	call	10001700
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	1000179F

l10001774:
	mov	eax,[eax+24]
	shr	eax,1F
	not	eax
	and	eax,01
	mov	dword ptr [ebp-04],FFFFFFFE
	jmp	100017A8
10001788                         8B 45 EC 8B 00 8B 00 33         .E.....3
10001790 C9 3D 05 00 00 C0 0F 94 C1 8B C1 C3 8B 65 E8    .=...........e.

l1000179F:
	mov	dword ptr [ebp-04],FFFFFFFE
	xor	eax,eax

l100017A8:
	call	1000182D
	ret
100017AE                                           FF 25               .%
100017B0 64 20 00 10                                     d ..           
100017B4             FF 25 60 20 00 10                       .%` ..     
100017BA                               FF 25 5C 20 00 10           .%\ ..
100017C0 FF 25 40 20 00 10                               .%@ ..         

;; fn100017C6: 100017C6
;;   Called from:
;;     100013FD (in fn10001388)
;;     10001411 (in fn10001388)
fn100017C6 proc
	cmp	dword ptr [esp+08],01
	jnz	100017E0

l100017CD:
	cmp	dword ptr [100020CC],00
	jnz	100017E0

l100017D6:
	push	dword ptr [esp+04]
	call	dword ptr [10002010]

l100017E0:
	xor	eax,eax
	inc	eax
	ret	000C
100017E6                   CC CC                               ..       

;; fn100017E8: 100017E8
;;   Called from:
;;     1000138F (in fn10001388)
;;     100015D6 (in fn100015CF)
;;     10001749 (in fn10001742)
fn100017E8 proc
	push	10001841
	push	dword ptr fs:[00000000]
	mov	eax,[esp+10]
	mov	[esp+10],ebp
	lea	ebp,[esp+10]
	sub	esp,eax
	push	ebx
	push	esi
	push	edi
	mov	eax,[10003000]
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

;; fn1000182D: 1000182D
;;   Called from:
;;     1000148D (in fn10001388)
;;     1000165F (in fn100015CF)
;;     100017A8 (in fn10001742)
fn1000182D proc
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
10001841    FF 74 24 10 FF 74 24 10 FF 74 24 10 FF 74 24  .t$..t$..t$..t$
10001850 10 68 8D 11 00 10 68 00 30 00 10 E8 B6 00 00 00 .h....h.0.......
10001860 83 C4 18 C3                                     ....           

;; fn10001864: 10001864
;;   Called from:
;;     100014A5 (in DllMain)
fn10001864 proc
	push	ebp
	mov	ebp,esp
	sub	esp,10
	mov	eax,[10003000]
	and	dword ptr [ebp-08],00
	and	dword ptr [ebp-04],00
	push	ebx
	push	edi
	mov	edi,BB40E64E
	cmp	eax,edi
	mov	ebx,FFFF0000
	jz	10001894

l10001887:
	test	ebx,eax
	jz	10001894

l1000188B:
	not	eax
	mov	[10003004],eax
	jmp	100018F4

l10001894:
	push	esi
	lea	eax,[ebp-08]
	push	eax
	call	dword ptr [10002000]
	mov	esi,[ebp-04]
	xor	esi,[ebp-08]
	call	dword ptr [10002004]
	xor	esi,eax
	call	dword ptr [10002008]
	xor	esi,eax
	call	dword ptr [1000200C]
	xor	esi,eax
	lea	eax,[ebp-10]
	push	eax
	call	dword ptr [10002034]
	mov	eax,[ebp-0C]
	xor	eax,[ebp-10]
	xor	esi,eax
	cmp	esi,edi
	jnz	100018DA

l100018D3:
	mov	esi,BB40E64F
	jmp	100018E5

l100018DA:
	test	ebx,esi
	jnz	100018E5

l100018DE:
	mov	eax,esi
	shl	eax,10
	or	esi,eax

l100018E5:
	mov	[10003000],esi
	not	esi
	mov	[10003004],esi
	pop	esi

l100018F4:
	pop	edi
	pop	ebx
	leave
	ret
100018F8                         FF 25 3C 20 00 10 FF 25         .%< ...%
10001900 7C 20 00 10 FF 25 44 20 00 10 FF 25 48 20 00 10 | ...%D ...%H ..
10001910 FF 25 4C 20 00 10 FF 25 54 20 00 10 00 00 00 00 .%L ...%T ......
