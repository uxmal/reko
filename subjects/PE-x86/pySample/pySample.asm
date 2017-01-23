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
100015C0 10 C9 C3 68 9C 33 00 10 E8 31 03 00 00 59 C3 6A ...h.3...1...Y.j
100015D0 14 68 10 22 00 10 E8 0D 02 00 00 FF 35 B4 33 00 .h."........5.3.
100015E0 10 8B 35 68 20 00 10 FF D6 59 89 45 E4 83 F8 FF ..5h ....Y.E....
100015F0 75 0C FF 75 08 FF 15 50 20 00 10 59 EB 61 6A 08 u..u...P ..Y.aj.
10001600 E8 0B 03 00 00 59 83 65 FC 00 FF 35 B4 33 00 10 .....Y.e...5.3..
10001610 FF D6 89 45 E4 FF 35 B0 33 00 10 FF D6 89 45 E0 ...E..5.3.....E.
10001620 8D 45 E0 50 8D 45 E4 50 FF 75 08 E8 DA 02 00 00 .E.P.E.P.u......
10001630 89 45 DC FF 75 E4 8B 35 78 20 00 10 FF D6 A3 B4 .E..u..5x ......
10001640 33 00 10 FF 75 E0 FF D6 83 C4 1C A3 B0 33 00 10 3...u........3..
10001650 C7 45 FC FE FF FF FF E8 09 00 00 00 8B 45 DC E8 .E...........E..
10001660 C9 01 00 00 C3 6A 08 E8 98 02 00 00 59 C3 FF 74 .....j......Y..t
10001670 24 04 E8 58 FF FF FF F7 D8 1B C0 F7 D8 59 48 C3 $..X.........YH.
10001680 56 57 B8 D8 21 00 10 BF D8 21 00 10 3B C7 8B F0 VW..!....!..;...
10001690 73 0F 8B 06 85 C0 74 02 FF D0 83 C6 04 3B F7 72 s.....t......;.r
100016A0 F1 5F 5E C3 56 57 B8 E0 21 00 10 BF E0 21 00 10 ._^.VW..!....!..
100016B0 3B C7 8B F0 73 0F 8B 06 85 C0 74 02 FF D0 83 C6 ;...s.....t.....
100016C0 04 3B F7 72 F1 5F 5E C3 CC CC CC CC CC CC CC CC .;.r._^.........

;; fn100016D0: 100016D0
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
;;; Segment .rdata (10002000)
l10002000	dd	0x000025CC
l10002004	dd	0x000025B6
l10002008	dd	0x000025A0
l1000200C	dd	0x00002590
l10002010	dd	0x0000255A
l10002014	dd	0x00002546
l10002018	dd	0x00002528
l1000201C	dd	0x0000250C
l10002020	dd	0x000024F8
l10002024	dd	0x000024E4
l10002028	dd	0x000024C6
l1000202C	dd	0x000024BE
l10002030	dd	0x000024A8
l10002034	dd	0x00002576
10002038                         00 00 00 00                     ....   
l1000203C	dd	0x0000241E
l10002040	dd	0x0000240C
l10002044	dd	0x00002458
l10002048	dd	0x00002462
l1000204C	dd	0x00002470
l10002050	dd	0x00002478
l10002054	dd	0x0000248E
l10002058	dd	0x000023FC
l1000205C	dd	0x000023EE
l10002060	dd	0x000023E0
l10002064	dd	0x000023D4
l10002068	dd	0x000023C2
l1000206C	dd	0x000023BA
l10002070	dd	0x000023AA
l10002074	dd	0x0000239C
l10002078	dd	0x0000238A
l1000207C	dd	0x00002434
10002080 00 00 00 00                                     ....           
l10002084	dd	0x0000236A
l10002088	dd	0x00002356
l1000208C	dd	0x00002344
l10002090	dd	0x00002334
10002094             00 00 00 00 00 00 00 00 00 00 00 00     ............
100020A0 00 00 00 00 9C 11 00 10 00 00 00 00 00 00 00 00 ................
100020B0 00 00 00 00 FB 2D AA 56 00 00 00 00 02 00 00 00 .....-.V........
100020C0 54 00 00 00 80 21 00 00 80 0F 00 00 00 00 00 00 T....!..........
100020D0 78 30 00 10 D0 30 00 10 75 6E 75 73 65 64 00 00 x0...0..unused..
100020E0 66 64 69 76 28 61 2C 20 62 29 20 3D 20 61 20 2F fdiv(a, b) = a /
100020F0 20 62 00 00 66 64 69 76 00 00 00 00 64 69 76 28  b..fdiv....div(
10002100 61 2C 20 62 29 20 3D 20 61 20 2F 20 62 00 00 00 a, b) = a / b...
10002110 64 69 76 00 64 69 66 28 61 2C 20 62 29 20 3D 20 div.dif(a, b) = 
10002120 61 20 2D 20 62 00 00 00 64 69 66 00 73 75 6D 28 a - b...dif.sum(
10002130 61 2C 20 62 29 20 3D 20 61 20 2B 20 62 00 00 00 a, b) = a + b...
10002140 73 75 6D 00 69 69 3A 73 75 6D 00 00 69 00 00 00 sum.ii:sum..i...
10002150 69 69 3A 64 69 66 00 00 69 69 3A 64 69 76 00 00 ii:dif..ii:div..
10002160 66 66 3A 66 64 69 76 00 66 00 00 00 3A 75 6E 75 ff:fdiv.f...:unu
10002170 73 65 64 00 70 79 53 61 6D 70 6C 65 00 00 00 00 sed.pySample....
10002180 52 53 44 53 5B A4 84 A8 81 67 DC 4F BA 00 17 0B RSDS[....g.O....
10002190 02 F1 8A 98 01 00 00 00 63 3A 5C 67 61 6D 65 73 ........c:\games
100021A0 5C 73 65 76 65 72 65 6E 63 65 20 2D 20 62 6C 61 \severence - bla
100021B0 64 65 20 6F 66 20 64 61 72 6B 6E 65 73 73 5C 73 de of darkness\s
100021C0 72 63 5C 62 69 6E 5C 70 79 53 61 6D 70 6C 65 2E rc\bin\pySample.
100021D0 70 64 62 00 00 00 00 00 00 00 00 00 00 00 00 00 pdb.............
100021E0 00 00 00 00 00 00 00 00 FE FF FF FF 00 00 00 00 ................
100021F0 D0 FF FF FF 00 00 00 00 FE FF FF FF 00 00 00 00 ................
10002200 93 14 00 10 00 00 00 00 5F 14 00 10 73 14 00 10 ........_...s...
10002210 FE FF FF FF 00 00 00 00 CC FF FF FF 00 00 00 00 ................
10002220 FE FF FF FF 00 00 00 00 65 16 00 10 00 00 00 00 ........e.......
10002230 FE FF FF FF 00 00 00 00 D8 FF FF FF 00 00 00 00 ................
10002240 FE FF FF FF 88 17 00 10 9C 17 00 10 20 23 00 00 ............ #..
10002250 00 00 00 00 00 00 00 00 7C 23 00 00 84 20 00 00 ........|#... ..
10002260 D8 22 00 00 00 00 00 00 00 00 00 00 82 24 00 00 ."...........$..
10002270 3C 20 00 00 9C 22 00 00 00 00 00 00 00 00 00 00 < ..."..........
10002280 E6 25 00 00 00 20 00 00 00 00 00 00 00 00 00 00 .%... ..........
10002290 00 00 00 00 00 00 00 00 00 00 00 00             ............   
l1000229C	dd	0x000025CC
l100022A0	dd	0x000025B6
l100022A4	dd	0x000025A0
l100022A8	dd	0x00002590
l100022AC	dd	0x0000255A
l100022B0	dd	0x00002546
l100022B4	dd	0x00002528
l100022B8	dd	0x0000250C
l100022BC	dd	0x000024F8
l100022C0	dd	0x000024E4
l100022C4	dd	0x000024C6
l100022C8	dd	0x000024BE
l100022CC	dd	0x000024A8
l100022D0	dd	0x00002576
100022D4             00 00 00 00                             ....       
l100022D8	dd	0x0000241E
l100022DC	dd	0x0000240C
l100022E0	dd	0x00002458
l100022E4	dd	0x00002462
l100022E8	dd	0x00002470
l100022EC	dd	0x00002478
l100022F0	dd	0x0000248E
l100022F4	dd	0x000023FC
l100022F8	dd	0x000023EE
l100022FC	dd	0x000023E0
l10002300	dd	0x000023D4
l10002304	dd	0x000023C2
l10002308	dd	0x000023BA
l1000230C	dd	0x000023AA
l10002310	dd	0x0000239C
l10002314	dd	0x0000238A
l10002318	dd	0x00002434
1000231C                                     00 00 00 00             ....
l10002320	dd	0x0000236A
l10002324	dd	0x00002356
l10002328	dd	0x00002344
l1000232C	dd	0x00002334
10002330 00 00 00 00 6A 01 50 79 5F 42 75 69 6C 64 56 61 ....j.Py_BuildVa
10002340 6C 75 65 00 A6 01 5F 50 79 5F 4E 6F 6E 65 53 74 lue..._Py_NoneSt
10002350 72 75 63 74 00 00 01 00 50 79 41 72 67 5F 50 61 ruct....PyArg_Pa
10002360 72 73 65 54 75 70 6C 65 00 00 81 01 50 79 5F 49 rseTuple....Py_I
10002370 6E 69 74 4D 6F 64 75 6C 65 34 00 00 70 79 74 68 nitModule4..pyth
10002380 6F 6E 31 35 2E 64 6C 6C 00 00 6D 01 5F 65 6E 63 on15.dll..m._enc
10002390 6F 64 65 5F 70 6F 69 6E 74 65 72 00 8D 02 5F 6D ode_pointer..._m
100023A0 61 6C 6C 6F 63 5F 63 72 74 00 6E 01 5F 65 6E 63 alloc_crt.n._enc
100023B0 6F 64 65 64 5F 6E 75 6C 6C 00 ED 04 66 72 65 65 oded_null...free
100023C0 00 00 63 01 5F 64 65 63 6F 64 65 5F 70 6F 69 6E ..c._decode_poin
100023D0 74 65 72 00 0A 02 5F 69 6E 69 74 74 65 72 6D 00 ter..._initterm.
100023E0 0B 02 5F 69 6E 69 74 74 65 72 6D 5F 65 00 18 01 .._initterm_e...
100023F0 5F 61 6D 73 67 5F 65 78 69 74 00 00 11 01 5F 61 _amsg_exit...._a
10002400 64 6A 75 73 74 5F 66 64 69 76 00 00 6B 00 5F 5F djust_fdiv..k.__
10002410 43 70 70 58 63 70 74 46 69 6C 74 65 72 00 4E 01 CppXcptFilter.N.
10002420 5F 63 72 74 5F 64 65 62 75 67 67 65 72 5F 68 6F _crt_debugger_ho
10002430 6F 6B 00 00 8D 00 5F 5F 63 6C 65 61 6E 5F 74 79 ok....__clean_ty
10002440 70 65 5F 69 6E 66 6F 5F 6E 61 6D 65 73 5F 69 6E pe_info_names_in
10002450 74 65 72 6E 61 6C 00 00 ED 03 5F 75 6E 6C 6F 63 ternal...._unloc
10002460 6B 00 97 00 5F 5F 64 6C 6C 6F 6E 65 78 69 74 00 k...__dllonexit.
10002470 7C 02 5F 6C 6F 63 6B 00 22 03 5F 6F 6E 65 78 69 |._lock."._onexi
10002480 74 00 4D 53 56 43 52 38 30 2E 64 6C 6C 00 76 01 t.MSVCR80.dll.v.
10002490 5F 65 78 63 65 70 74 5F 68 61 6E 64 6C 65 72 34 _except_handler4
100024A0 5F 63 6F 6D 6D 6F 6E 00 29 02 49 6E 74 65 72 6C _common.).Interl
100024B0 6F 63 6B 65 64 45 78 63 68 61 6E 67 65 00 56 03 ockedExchange.V.
100024C0 53 6C 65 65 70 00 26 02 49 6E 74 65 72 6C 6F 63 Sleep.&.Interloc
100024D0 6B 65 64 43 6F 6D 70 61 72 65 45 78 63 68 61 6E kedCompareExchan
100024E0 67 65 00 00 5E 03 54 65 72 6D 69 6E 61 74 65 50 ge..^.TerminateP
100024F0 72 6F 63 65 73 73 00 00 42 01 47 65 74 43 75 72 rocess..B.GetCur
10002500 72 65 6E 74 50 72 6F 63 65 73 73 00 6E 03 55 6E rentProcess.n.Un
10002510 68 61 6E 64 6C 65 64 45 78 63 65 70 74 69 6F 6E handledException
10002520 46 69 6C 74 65 72 00 00 4A 03 53 65 74 55 6E 68 Filter..J.SetUnh
10002530 61 6E 64 6C 65 64 45 78 63 65 70 74 69 6F 6E 46 andledExceptionF
10002540 69 6C 74 65 72 00 39 02 49 73 44 65 62 75 67 67 ilter.9.IsDebugg
10002550 65 72 50 72 65 73 65 6E 74 00 8B 00 44 69 73 61 erPresent...Disa
10002560 62 6C 65 54 68 72 65 61 64 4C 69 62 72 61 72 79 bleThreadLibrary
10002570 43 61 6C 6C 73 00 A3 02 51 75 65 72 79 50 65 72 Calls...QueryPer
10002580 66 6F 72 6D 61 6E 63 65 43 6F 75 6E 74 65 72 00 formanceCounter.
10002590 DF 01 47 65 74 54 69 63 6B 43 6F 75 6E 74 00 00 ..GetTickCount..
100025A0 46 01 47 65 74 43 75 72 72 65 6E 74 54 68 72 65 F.GetCurrentThre
100025B0 61 64 49 64 00 00 43 01 47 65 74 43 75 72 72 65 adId..C.GetCurre
100025C0 6E 74 50 72 6F 63 65 73 73 49 64 00 CA 01 47 65 ntProcessId...Ge
100025D0 74 53 79 73 74 65 6D 54 69 6D 65 41 73 46 69 6C tSystemTimeAsFil
100025E0 65 54 69 6D 65 00 4B 45 52 4E 45 4C 33 32 2E 64 eTime.KERNEL32.d
100025F0 6C 6C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ll..............
10002600 00 00 00 00 FB 2D AA 56 00 00 00 00 32 26 00 00 .....-.V....2&..
10002610 01 00 00 00 01 00 00 00 01 00 00 00 28 26 00 00 ............(&..
10002620 2C 26 00 00 30 26 00 00 70 11 00 00 3F 26 00 00 ,&..0&..p...?&..
10002630 00 00 70 79 53 61 6D 70 6C 65 2E 64 6C 6C 00 69 ..pySample.dll.i
10002640 6E 69 74 70 79 53 61 6D 70 6C 65 00             nitpySample.   
;;; Segment .data (10003000)
10003000 4E E6 40 BB B1 19 BF 44 FF FF FF FF FF FF FF FF N.@....D........
l10003010		dd	0x10002140
	dd	0x10001000
	dd	0x00000001
	dd	0x1000212C
	dd	0x10002128
	dd	0x10001050
	dd	0x00000001
	dd	0x10002114
	dd	0x10002110
	dd	0x100010A0
	dd	0x00000001
	dd	0x100020FC
	dd	0x100020F4
	dd	0x100010F0
	dd	0x00000001
	dd	0x100020E0
	dd	0x00000000
	dd	0x00000000
	dd	0x00000000
	dd	0x00000000
;;; Segment .rsrc (10004000)
10004000 00 00 00 00 00 00 00 00 04 00 00 00 00 00 01 00 ................
10004010 18 00 00 00 18 00 00 80 00 00 00 00 00 00 00 00 ................
10004020 04 00 00 00 00 00 01 00 02 00 00 00 30 00 00 80 ............0...
10004030 00 00 00 00 00 00 00 00 04 00 00 00 00 00 01 00 ................
10004040 09 04 00 00 48 00 00 00 58 40 00 00 52 01 00 00 ....H...X@..R...
10004050 E4 04 00 00 00 00 00 00 3C 61 73 73 65 6D 62 6C ........<assembl
10004060 79 20 78 6D 6C 6E 73 3D 22 75 72 6E 3A 73 63 68 y xmlns="urn:sch
10004070 65 6D 61 73 2D 6D 69 63 72 6F 73 6F 66 74 2D 63 emas-microsoft-c
10004080 6F 6D 3A 61 73 6D 2E 76 31 22 20 6D 61 6E 69 66 om:asm.v1" manif
10004090 65 73 74 56 65 72 73 69 6F 6E 3D 22 31 2E 30 22 estVersion="1.0"
100040A0 3E 0D 0A 20 20 3C 64 65 70 65 6E 64 65 6E 63 79 >..  <dependency
100040B0 3E 0D 0A 20 20 20 20 3C 64 65 70 65 6E 64 65 6E >..    <dependen
100040C0 74 41 73 73 65 6D 62 6C 79 3E 0D 0A 20 20 20 20 tAssembly>..    
100040D0 20 20 3C 61 73 73 65 6D 62 6C 79 49 64 65 6E 74   <assemblyIdent
100040E0 69 74 79 20 74 79 70 65 3D 22 77 69 6E 33 32 22 ity type="win32"
100040F0 20 6E 61 6D 65 3D 22 4D 69 63 72 6F 73 6F 66 74  name="Microsoft
10004100 2E 56 43 38 30 2E 43 52 54 22 20 76 65 72 73 69 .VC80.CRT" versi
10004110 6F 6E 3D 22 38 2E 30 2E 35 30 36 30 38 2E 30 22 on="8.0.50608.0"
10004120 20 70 72 6F 63 65 73 73 6F 72 41 72 63 68 69 74  processorArchit
10004130 65 63 74 75 72 65 3D 22 78 38 36 22 20 70 75 62 ecture="x86" pub
10004140 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 22 31 66 63 licKeyToken="1fc
10004150 38 62 33 62 39 61 31 65 31 38 65 33 62 22 3E 3C 8b3b9a1e18e3b"><
10004160 2F 61 73 73 65 6D 62 6C 79 49 64 65 6E 74 69 74 /assemblyIdentit
10004170 79 3E 0D 0A 20 20 20 20 3C 2F 64 65 70 65 6E 64 y>..    </depend
10004180 65 6E 74 41 73 73 65 6D 62 6C 79 3E 0D 0A 20 20 entAssembly>..  
10004190 3C 2F 64 65 70 65 6E 64 65 6E 63 79 3E 0D 0A 3C </dependency>..<
100041A0 2F 61 73 73 65 6D 62 6C 79 3E 50 41             /assembly>PA   
;;; Segment .reloc (10005000)
10005000 00 10 00 00 24 01 00 00 11 30 18 30 32 30 38 30 ....$....0.02080
10005010 62 30 69 30 81 30 87 30 B2 30 B9 30 D2 30 D8 30 b0i0.0.0.0.0.0.0
10005020 02 31 09 31 26 31 2C 31 45 31 4C 31 59 31 61 31 .1.1&1,1E1L1Y1a1
10005030 7A 31 7F 31 85 31 8F 31 A4 31 AD 31 B6 31 BB 31 z1.1.1.1.1.1.1.1
10005040 CF 31 DB 31 F6 31 FE 31 07 32 12 32 27 32 2D 32 .1.1.1.1.2.2'2-2
10005050 45 32 59 32 6E 32 73 32 79 32 94 32 99 32 A5 32 E2Y2n2s2y2.2.2.2
10005060 B2 32 B8 32 C0 32 D9 32 DF 32 F2 32 F7 32 04 33 .2.2.2.2.2.2.2.3
10005070 13 33 28 33 2E 33 3D 33 5A 33 61 33 66 33 6B 33 .3(3.3=3Z3a3f3k3
10005080 74 33 7A 33 8B 33 A8 33 B5 33 CD 33 20 34 4D 34 t3z3.3.3.3.3 4M4
10005090 95 34 C9 34 CF 34 D5 34 DB 34 E1 34 E7 34 EE 34 .4.4.4.4.4.4.4.4
100050A0 F5 34 FC 34 03 35 0A 35 11 35 18 35 20 35 28 35 .4.4.5.5.5.5 5(5
100050B0 30 35 3C 35 45 35 4A 35 50 35 5A 35 63 35 6E 35 05<5E5J5P5Z5c5n5
100050C0 7A 35 7F 35 8F 35 94 35 9A 35 A0 35 B6 35 BD 35 z5.5.5.5.5.5.5.5
100050D0 C4 35 D2 35 DD 35 E3 35 F7 35 0C 36 17 36 38 36 .5.5.5.5.5.6.686
100050E0 3F 36 4C 36 83 36 88 36 A7 36 AC 36 45 37 53 37 ?6L6.6.6.6.6E7S7
100050F0 B0 37 B6 37 BC 37 C2 37 CF 37 DC 37 E9 37 06 38 .7.7.7.7.7.7.7.8
10005100 52 38 57 38 6B 38 8E 38 9B 38 A7 38 AF 38 B7 38 R8W8k8.8.8.8.8.8
10005110 C3 38 E7 38 EF 38 FA 38 00 39 06 39 0C 39 12 39 .8.8.8.8.9.9.9.9
10005120 18 39 00 00 00 20 00 00 1C 00 00 00 A4 30 D0 30 .9... .......0.0
10005130 D4 30 00 32 08 32 0C 32 28 32 44 32 48 32 00 00 .0.2.2.2(2D2H2..
10005140 00 30 00 00 28 00 00 00 10 30 14 30 1C 30 20 30 .0..(....0.0.0 0
10005150 24 30 2C 30 30 30 34 30 3C 30 40 30 44 30 4C 30 $0,00040<0@0D0L0
10005160 60 30 64 30 6C 30 00 00 00 00 00 00 00 00 00 00 `0d0l0..........
10005170 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
10005190 00 00 00 00                                     ....           
