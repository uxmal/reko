;;; Segment .text (10001000)

;; fn10001000: 10001000
fn10001000 proc
	mov	edx,[esp+8h]
	sub	esp,8h
	lea	eax,[esp]
	push	eax
	lea	ecx,[esp+8h]
	push	ecx
	push	10002144h
	push	edx
	call	dword ptr [__imp__PyArg_ParseTuple]
	add	esp,10h
	test	eax,eax
	jnz	10001027h

l10001023:
	add	esp,8h
	ret

l10001027:
	mov	eax,[esp]
	mov	ecx,[esp+4h]
	add	ecx,eax
	push	ecx
	push	1000214Ch
	call	dword ptr [__imp__Py_BuildValue]
	add	esp,8h
	add	esp,8h
	ret
10001043          CC CC CC CC CC CC CC CC CC CC CC CC CC    .............

;; fn10001050: 10001050
fn10001050 proc
	mov	edx,[esp+8h]
	sub	esp,8h
	lea	eax,[esp+4h]
	push	eax
	lea	ecx,[esp+4h]
	push	ecx
	push	10002150h
	push	edx
	call	dword ptr [__imp__PyArg_ParseTuple]
	add	esp,10h
	test	eax,eax
	jnz	10001078h

l10001074:
	add	esp,8h
	ret

l10001078:
	mov	eax,[esp]
	sub	eax,[esp+4h]
	push	eax
	push	1000214Ch
	call	dword ptr [__imp__Py_BuildValue]
	add	esp,8h
	add	esp,8h
	ret
10001092       CC CC CC CC CC CC CC CC CC CC CC CC CC CC   ..............

;; fn100010A0: 100010A0
fn100010A0 proc
	mov	edx,[esp+8h]
	sub	esp,8h
	lea	eax,[esp+4h]
	push	eax
	lea	ecx,[esp+4h]
	push	ecx
	push	10002158h
	push	edx
	call	dword ptr [__imp__PyArg_ParseTuple]
	add	esp,10h
	test	eax,eax
	jnz	100010C8h

l100010C4:
	add	esp,8h
	ret

l100010C8:
	mov	eax,[esp]
	cdq
	idiv	dword ptr [esp+4h]
	push	eax
	push	1000214Ch
	call	dword ptr [__imp__Py_BuildValue]
	add	esp,8h
	add	esp,8h
	ret
100010E3          CC CC CC CC CC CC CC CC CC CC CC CC CC    .............

;; fn100010F0: 100010F0
fn100010F0 proc
	mov	edx,[esp+8h]
	sub	esp,8h
	lea	eax,[esp+4h]
	push	eax
	lea	ecx,[esp+4h]
	push	ecx
	push	10002160h
	push	edx
	call	dword ptr [__imp__PyArg_ParseTuple]
	add	esp,10h
	test	eax,eax
	jnz	10001118h

l10001114:
	add	esp,8h
	ret

l10001118:
	fld	dword ptr [esp]
	sub	esp,8h
	fdiv	dword ptr [esp+0Ch]
	fstp	double ptr [esp]
	push	10002168h
	call	dword ptr [__imp__Py_BuildValue]
	add	esp,0Ch
	add	esp,8h
	ret
10001137                      CC CC CC CC CC CC CC CC CC        .........

;; py_unused: 10001140
py_unused proc
	mov	eax,[esp+8h]
	push	1000216Ch
	push	eax
	call	dword ptr [__imp__PyArg_ParseTuple]
	add	esp,8h
	test	eax,eax
	jnz	10001158h

l10001157:
	ret

l10001158:
	mov	eax,[__imp___Py_NoneStruct]
	add	dword ptr [eax],1h
	mov	eax,[__imp___Py_NoneStruct]
	ret
10001166                   CC CC CC CC CC CC CC CC CC CC       ..........

;; initpySample: 10001170
initpySample proc
	push	3EFh
	push	0h
	push	0h
	push	10003010h
	push	10002174h
	call	dword ptr [__imp__Py_InitModule4]
	add	esp,14h
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
	mov	eax,[esp+8h]
	push	ebp
	xor	ebp,ebp
	cmp	eax,ebp
	jnz	10001202h

l100011F4:
	cmp	[10003070h],ebp
	jle	10001233h

l100011FC:
	dec	dword ptr [10003070h]

l10001202:
	cmp	eax,1h
	mov	ecx,[__imp___adjust_fdiv]
	mov	ecx,[ecx]
	push	ebx
	push	esi
	push	edi
	mov	[100033A4h],ecx
	jnz	100012E8h

l1000121C:
	mov	eax,fs:[0018h]
	mov	edi,[eax+4h]
	mov	ebx,[__imp__InterlockedCompareExchange]
	push	ebp
	mov	esi,100033ACh
	jmp	1000124Bh

l10001233:
	xor	eax,eax
	jmp	10001384h

l1000123A:
	cmp	eax,edi
	jz	10001255h

l1000123E:
	push	3E8h
	call	dword ptr [__imp__Sleep]
	push	0h

l1000124B:
	push	edi
	push	esi
	call	ebx
	test	eax,eax
	jnz	1000123Ah

l10001253:
	jmp	10001258h

l10001255:
	xor	ebp,ebp
	inc	ebp

l10001258:
	mov	eax,[100033A8h]
	test	eax,eax
	push	2h
	pop	edi
	jz	1000126Dh

l10001264:
	push	1Fh
	call	100017BAh
1000126B                                  EB 3C                     .<   

l1000126D:
	push	100020A8h
	push	100020A0h
	mov	dword ptr [100033A8h],1h
	call	100017B4h
	test	eax,eax
	pop	ecx
	pop	ecx
	jz	10001293h

l1000128C:
	xor	eax,eax
	jmp	10001381h

l10001293:
	push	1000209Ch
	push	10002098h
	call	100017AEh
	pop	ecx
	mov	[100033A8h],edi
	test	ebp,ebp
	pop	ecx
	jnz	100012B6h

l100012AE:
	push	ebp
	push	esi
	call	dword ptr [__imp__InterlockedExchange]

l100012B6:
	cmp	dword ptr [100033B8h],0h
	jz	100012DDh

l100012BF:
	push	100033B8h
	call	fn10001742
	test	eax,eax
	pop	ecx
	jz	100012DDh

l100012CE:
	push	dword ptr [esp+1Ch]
	push	edi
	push	dword ptr [esp+1Ch]
	call	dword ptr [100033B8h]

l100012DD:
	inc	dword ptr [10003070h]
	jmp	1000137Eh

l100012E8:
	cmp	eax,ebp
	jnz	1000137Eh

l100012F0:
	mov	edi,[__imp__InterlockedCompareExchange]
	mov	esi,100033ACh
	jmp	10001308h

l100012FD:
	push	3E8h
	call	dword ptr [__imp__Sleep]

l10001308:
	push	ebp
	push	1h
	push	esi
	call	edi
	test	eax,eax
	jnz	100012FDh

l10001312:
	mov	eax,[100033A8h]
	cmp	eax,2h
	jz	10001326h

l1000131C:
	push	1Fh
	call	100017BAh
10001323          59 EB 58                                  Y.X          

l10001326:
	push	dword ptr [100033B4h]
	mov	edi,[__imp___decode_pointer]
	call	edi
	mov	ebx,eax
	test	ebx,ebx
	pop	ecx
	jz	1000136Fh

l1000133B:
	push	dword ptr [100033B0h]
	call	edi
	pop	ecx
	mov	edi,eax
	jmp	10001350h

l10001348:
	mov	eax,[edi]
	test	eax,eax
	jz	10001350h

l1000134E:
	call	eax

l10001350:
	sub	edi,4h
	cmp	edi,ebx
	jnc	10001348h

l10001357:
	push	ebx
	call	dword ptr [__imp__free]
	pop	ecx
	call	dword ptr [__imp___encoded_null]
	mov	[100033B0h],eax
	mov	[100033B4h],eax

l1000136F:
	push	0h
	push	esi
	mov	[100033A8h],ebp
	call	dword ptr [__imp__InterlockedExchange]

l1000137E:
	xor	eax,eax
	inc	eax

l10001381:
	pop	edi
	pop	esi
	pop	ebx

l10001384:
	pop	ebp
	ret	0Ch

;; fn10001388: 10001388
;;   Called from:
;;     100014B6 (in DllMain)
fn10001388 proc
	push	10h
	push	100021E8h
	call	fn100017E8
	mov	edi,ecx
	mov	esi,edx
	mov	ebx,[ebp+8h]
	xor	eax,eax
	inc	eax
	mov	[ebp-1Ch],eax
	xor	ecx,ecx
	mov	[ebp-4h],ecx
	mov	[10003008h],esi
	mov	[ebp-4h],eax
	cmp	esi,ecx
	jnz	100013C3h

l100013B3:
	cmp	[10003070h],ecx
	jnz	100013C3h

l100013BB:
	mov	[ebp-1Ch],ecx
	jmp	1000147Ah

l100013C3:
	cmp	esi,eax
	jz	100013CCh

l100013C7:
	cmp	esi,2h
	jnz	100013FAh

l100013CC:
	mov	eax,[100020CCh]
	cmp	eax,ecx
	jz	100013DDh

l100013D5:
	push	edi
	push	esi
	push	ebx
	call	eax
	mov	[ebp-1Ch],eax

l100013DD:
	cmp	dword ptr [ebp-1Ch],0h
	jz	1000147Ah

l100013E7:
	push	edi
	push	esi
	push	ebx
	call	fn100011E9
	mov	[ebp-1Ch],eax
	test	eax,eax
	jz	1000147Ah

l100013FA:
	push	edi
	push	esi
	push	ebx
	call	fn100017C6
	mov	[ebp-1Ch],eax
	cmp	esi,1h
	jnz	1000142Eh

l1000140A:
	test	eax,eax
	jnz	1000142Eh

l1000140E:
	push	edi
	push	eax
	push	ebx
	call	fn100017C6
	push	edi
	push	0h
	push	ebx
	call	fn100011E9
	mov	eax,[100020CCh]
	test	eax,eax
	jz	1000142Eh

l10001428:
	push	edi
	push	0h
	push	ebx
	call	eax

l1000142E:
	test	esi,esi
	jz	10001437h

l10001432:
	cmp	esi,3h
	jnz	1000147Ah

l10001437:
	push	edi
	push	esi
	push	ebx
	call	fn100011E9
	test	eax,eax
	jnz	10001446h

l10001443:
	and	[ebp-1Ch],eax

l10001446:
	cmp	dword ptr [ebp-1Ch],0h
	jz	1000147Ah

l1000144C:
	mov	eax,[100020CCh]
	test	eax,eax
	jz	1000147Ah

l10001455:
	push	edi
	push	esi
	push	ebx
	call	eax
	mov	[ebp-1Ch],eax
	jmp	1000147Ah
1000145F                                              8B                .
10001460 45 EC 8B 08 8B 09 89 4D E0 50 51 E8 50 03 00 00 E......M.PQ.P...
10001470 59 59 C3 8B 65 E8 83 65 E4 00                   YY..e..e..      

l1000147A:
	and	dword ptr [ebp-4h],0h
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	call	fn10001493
	mov	eax,[ebp-1Ch]
	call	fn1000182D
	ret

;; fn10001493: 10001493
;;   Called from:
;;     10001485 (in fn10001388)
fn10001493 proc
	mov	dword ptr [10003008h],0FFFFFFFFh
	ret

;; DllMain: 1000149E
DllMain proc
	cmp	dword ptr [esp+8h],1h
	jnz	100014AAh

l100014A5:
	call	fn10001864

l100014AA:
	push	dword ptr [esp+4h]
	mov	ecx,[esp+10h]
	mov	edx,[esp+0Ch]
	call	fn10001388
	pop	ecx
	ret	0Ch
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
;;   Called from:
;;     10001758 (in fn10001742)
fn100016D0 proc
	mov	ecx,[esp+4h]
	cmp	word ptr [ecx],5A4Dh
	jz	100016DEh

l100016DB:
	xor	eax,eax
	ret

l100016DE:
	mov	eax,[ecx+3Ch]
	add	eax,ecx
	cmp	dword ptr [eax],4550h
	jnz	100016DBh

l100016EB:
	xor	ecx,ecx
	cmp	word ptr [eax+18h],10Bh
	setz	cl
	mov	eax,ecx
	ret
100016F9                            CC CC CC CC CC CC CC          .......

;; fn10001700: 10001700
;;   Called from:
;;     10001769 (in fn10001742)
fn10001700 proc
	mov	eax,[esp+4h]
	mov	ecx,[eax+3Ch]
	add	ecx,eax
	movzx	eax,word ptr [ecx+14h]
	push	ebx
	push	esi
	movzx	esi,word ptr [ecx+6h]
	xor	edx,edx
	test	esi,esi
	push	edi
	lea	eax,[eax+ecx+18h]
	jbe	1000173Ch

l1000171E:
	mov	edi,[esp+14h]

l10001722:
	mov	ecx,[eax+0Ch]
	cmp	edi,ecx
	jc	10001732h

l10001729:
	mov	ebx,[eax+8h]
	add	ebx,ecx
	cmp	edi,ebx
	jc	1000173Eh

l10001732:
	add	edx,1h
	add	eax,28h
	cmp	edx,esi
	jc	10001722h

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
	push	8h
	push	10002230h
	call	fn100017E8
	and	dword ptr [ebp-4h],0h
	mov	edx,10000000h
	push	edx
	call	fn100016D0
	pop	ecx
	test	eax,eax
	jz	1000179Fh

l10001762:
	mov	eax,[ebp+8h]
	sub	eax,edx
	push	eax
	push	edx
	call	fn10001700
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	1000179Fh

l10001774:
	mov	eax,[eax+24h]
	shr	eax,1Fh
	not	eax
	and	eax,1h
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	jmp	100017A8h
10001788                         8B 45 EC 8B 00 8B 00 33         .E.....3
10001790 C9 3D 05 00 00 C0 0F 94 C1 8B C1 C3 8B 65 E8    .=...........e. 

l1000179F:
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	xor	eax,eax

l100017A8:
	call	fn1000182D
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
	cmp	dword ptr [esp+8h],1h
	jnz	100017E0h

l100017CD:
	cmp	dword ptr [100020CCh],0h
	jnz	100017E0h

l100017D6:
	push	dword ptr [esp+4h]
	call	dword ptr [__imp__DisableThreadLibraryCalls]

l100017E0:
	xor	eax,eax
	inc	eax
	ret	0Ch
100017E6                   CC CC                               ..        

;; fn100017E8: 100017E8
;;   Called from:
;;     1000138F (in fn10001388)
;;     10001749 (in fn10001742)
fn100017E8 proc
	push	10001841h
	push	dword ptr fs:[0000h]
	mov	eax,[esp+10h]
	mov	[esp+10h],ebp
	lea	ebp,[esp+10h]
	sub	esp,eax
	push	ebx
	push	esi
	push	edi
	mov	eax,[10003000h]
	xor	[ebp-4h],eax
	xor	eax,ebp
	push	eax
	mov	[ebp-18h],esp
	push	dword ptr [ebp-8h]
	mov	eax,[ebp-4h]
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	mov	[ebp-8h],eax
	lea	eax,[ebp-10h]
	mov	fs:[0000h],eax
	ret

;; fn1000182D: 1000182D
;;   Called from:
;;     1000148D (in fn10001388)
;;     100017A8 (in fn10001742)
fn1000182D proc
	mov	ecx,[ebp-10h]
	mov	fs:[0000h],ecx
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
	sub	esp,10h
	mov	eax,[10003000h]
	and	dword ptr [ebp-8h],0h
	and	dword ptr [ebp-4h],0h
	push	ebx
	push	edi
	mov	edi,0BB40E64Eh
	cmp	eax,edi
	mov	ebx,0FFFF0000h
	jz	10001894h

l10001887:
	test	ebx,eax
	jz	10001894h

l1000188B:
	not	eax
	mov	[10003004h],eax
	jmp	100018F4h

l10001894:
	push	esi
	lea	eax,[ebp-8h]
	push	eax
	call	dword ptr [__imp__GetSystemTimeAsFileTime]
	mov	esi,[ebp-4h]
	xor	esi,[ebp-8h]
	call	dword ptr [__imp__GetCurrentProcessId]
	xor	esi,eax
	call	dword ptr [__imp__GetCurrentThreadId]
	xor	esi,eax
	call	dword ptr [__imp__GetTickCount]
	xor	esi,eax
	lea	eax,[ebp-10h]
	push	eax
	call	dword ptr [__imp__QueryPerformanceCounter]
	mov	eax,[ebp-0Ch]
	xor	eax,[ebp-10h]
	xor	esi,eax
	cmp	esi,edi
	jnz	100018DAh

l100018D3:
	mov	esi,0BB40E64Fh
	jmp	100018E5h

l100018DA:
	test	ebx,esi
	jnz	100018E5h

l100018DE:
	mov	eax,esi
	shl	eax,10h
	or	esi,eax

l100018E5:
	mov	[10003000h],esi
	not	esi
	mov	[10003004h],esi
	pop	esi

l100018F4:
	pop	edi
	pop	ebx
	leave
	ret
100018F8                         FF 25 3C 20 00 10 FF 25         .%< ...%
10001900 7C 20 00 10 FF 25 44 20 00 10 FF 25 48 20 00 10 | ...%D ...%H ..
10001910 FF 25 4C 20 00 10 FF 25 54 20 00 10 00 00 00 00 .%L ...%T ......
