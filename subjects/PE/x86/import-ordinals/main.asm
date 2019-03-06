;;; Segment .text (00401000)

;; fn00401000: 00401000
;;   Called from:
;;     0040102A (in fn00401010)
fn00401000 proc
	mov	eax,00403378
	ret
00401006                   CC CC CC CC CC CC CC CC CC CC       ..........

;; fn00401010: 00401010
;;   Called from:
;;     0040107D (in fn00401040)
fn00401010 proc
	push	ebp
	mov	ebp,esp
	push	esi
	mov	esi,[ebp+08]
	push	01
	call	dword ptr [004020B4]
	add	esp,04
	lea	ecx,[ebp+0C]
	push	ecx
	push	00
	push	esi
	push	eax
	call	00401000
	push	dword ptr [eax+04]
	push	dword ptr [eax]
	call	dword ptr [004020B0]
	add	esp,18
	pop	esi
	pop	ebp
	ret

;; fn00401040: 00401040
;;   Called from:
;;     0040124E (in Win32CrtStartup)
fn00401040 proc
	push	ebp
	mov	ebp,esp
	push	ecx
	lea	ecx,[ebp-04]
	call	dword ptr [004020BC]
	push	03
	lea	ecx,[ebp-04]
	call	dword ptr [004020C0]
	push	01
	lea	ecx,[ebp-04]
	call	dword ptr [004020C4]
	push	05
	lea	ecx,[ebp-04]
	call	dword ptr [004020C8]
	mov	eax,[004020CC]
	push	dword ptr [ebp-04]
	push	dword ptr [eax]
	push	00402118
	call	00401010
	add	esp,0C
	xor	eax,eax
	mov	esp,ebp
	pop	ebp
	ret
0040108B                                  3B 0D 04 30 40            ;..0@
00401090 00 F2 75 02 F2 C3 F2 E9 5F 02 00 00 56 6A 01 E8 ..u....._...Vj..
004010A0 12 0B 00 00 E8 55 06 00 00 50 E8 3D 0B 00 00 E8 .....U...P.=....
004010B0 68 0B 00 00 8B F0 E8 CD 07 00 00 6A 01 89 06 E8 h..........j....
004010C0 E4 03 00 00 83 C4 0C 5E 84 C0 74 6C DB E2 E8 49 .......^..tl...I
004010D0 08 00 00 68 47 19 40 00 E8 6C 05 00 00 E8 18 06 ...hG.@..l......
004010E0 00 00 50 E8 DA 0A 00 00 59 59 85 C0 75 4A E8 11 ..P.....YY..uJ..
004010F0 06 00 00 E8 5D 06 00 00 85 C0 74 0B 68 88 18 40 ....].....t.h..@
00401100 00 E8 B6 0A 00 00 59 E8 25 06 00 00 E8 20 06 00 ......Y.%.... ..
00401110 00 E8 FA 05 00 00 E8 6D 07 00 00 50 E8 EF 0A 00 .......m...P....
00401120 00 59 E8 1F 0B 00 00 84 C0 74 05 E8 98 0A 00 00 .Y.......t......
00401130 E8 53 07 00 00 33 C0 C3 6A 07 E8 2E 06 00 00 CC .S...3..j.......
00401140 E8 F3 05 00 00 33 C0 C3 E8 82 07 00 00 E8 36 07 .....3........6.
00401150 00 00 50 E8 BE 0A 00 00 59 C3                   ..P.....Y.     

l0040115A:
	push	14
	push	00402508
	call	00401980
	push	01
	call	0040146F
	pop	ecx
	test	al,al
	jnz	00401179

l00401172:
	push	07
	call	0040176D

l00401179:
	xor	bl,bl
	mov	[ebp-19],bl
	and	dword ptr [ebp-04],00
	call	0040143A
	mov	[ebp-24],al
	mov	eax,[00403334]
	xor	ecx,ecx
	inc	ecx
	cmp	eax,ecx
	jz	00401172

l00401196:
	test	eax,eax
	jnz	004011E3

l0040119A:
	mov	[00403334],ecx
	push	004020F0
	push	004020E4
	call	00401BDA
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	004011C6

l004011B5:
	mov	dword ptr [ebp-04],FFFFFFFE
	mov	eax,000000FF
	jmp	004012C3

l004011C6:
	push	004020E0
	push	004020D8
	call	00401BD4
	pop	ecx
	pop	ecx
	mov	dword ptr [00403334],00000002
	jmp	004011E8

l004011E3:
	mov	bl,cl
	mov	[ebp-19],bl

l004011E8:
	push	dword ptr [ebp-24]
	call	004015C9
	pop	ecx
	call	00401761
	mov	esi,eax
	xor	edi,edi
	cmp	[esi],edi
	jz	00401218

l004011FE:
	push	esi
	call	0040153F
	pop	ecx
	test	al,al
	jz	00401218

l00401209:
	push	edi
	push	02
	push	edi
	mov	esi,[esi]
	mov	ecx,esi
	call	00401972
	call	esi

l00401218:
	call	00401767
	mov	esi,eax
	cmp	[esi],edi
	jz	00401236

l00401223:
	push	esi
	call	0040153F
	pop	ecx
	test	al,al
	jz	00401236

l0040122E:
	push	dword ptr [esi]
	call	00401C0A
	pop	ecx

l00401236:
	call	00401BF8
	mov	edi,eax
	call	00401BF2
	mov	esi,eax
	call	00401BCE
	push	eax
	push	dword ptr [edi]
	push	dword ptr [esi]
	call	00401040
	add	esp,0C
	mov	esi,eax
	call	0040188B
	test	al,al
	jnz	00401267

l00401261:
	push	esi
	call	00401BE0

l00401267:
	test	bl,bl
	jnz	00401270

l0040126B:
	call	00401BFE

l00401270:
	push	00
	push	01
	call	004015E6
	pop	ecx
	pop	ecx
	mov	dword ptr [ebp-04],FFFFFFFE
	mov	eax,esi
	jmp	004012C3
00401286                   8B 4D EC 8B 01 8B 00 89 45 E0       .M......E.
00401290 51 50 E8 19 09 00 00 59 59 C3 8B 65 E8 E8 E9 05 QP.....YY..e....
004012A0 00 00 84 C0 75 08 FF 75 E0 E8 38 09 00 00 80 7D ....u..u..8....}
004012B0 E7 00 75 05 E8 4B 09 00 00 C7 45 FC FE FF FF FF ..u..K....E.....
004012C0 8B 45 E0                                        .E.            

l004012C3:
	call	004019C6
	ret

;; Win32CrtStartup: 004012C9
Win32CrtStartup proc
	call	0040165E
	jmp	0040115A
004012D3          55 8B EC 6A 00 FF 15 10 20 40 00 FF 75    U..j.... @..u
004012E0 08 FF 15 1C 20 40 00 68 09 04 00 C0 FF 15 14 20 .... @.h....... 
004012F0 40 00 50 FF 15 18 20 40 00 5D C3 55 8B EC 81 EC @.P... @.].U....
00401300 24 03 00 00 6A 17 E8 35 09 00 00 85 C0 74 05 6A $...j..5.....t.j
00401310 02 59 CD 29 A3 18 31 40 00 89 0D 14 31 40 00 89 .Y.)..1@....1@..
00401320 15 10 31 40 00 89 1D 0C 31 40 00 89 35 08 31 40 ..1@....1@..5.1@
00401330 00 89 3D 04 31 40 00 66 8C 15 30 31 40 00 66 8C ..=.1@.f..01@.f.
00401340 0D 24 31 40 00 66 8C 1D 00 31 40 00 66 8C 05 FC .$1@.f...1@.f...
00401350 30 40 00 66 8C 25 F8 30 40 00 66 8C 2D F4 30 40 0@.f.%.0@.f.-.0@
00401360 00 9C 8F 05 28 31 40 00 8B 45 00 A3 1C 31 40 00 ....(1@..E...1@.
00401370 8B 45 04 A3 20 31 40 00 8D 45 08 A3 2C 31 40 00 .E.. 1@..E..,1@.
00401380 8B 85 DC FC FF FF C7 05 68 30 40 00 01 00 01 00 ........h0@.....
00401390 A1 20 31 40 00 A3 24 30 40 00 C7 05 18 30 40 00 . 1@..$0@....0@.
004013A0 09 04 00 C0 C7 05 1C 30 40 00 01 00 00 00 C7 05 .......0@.......
004013B0 28 30 40 00 01 00 00 00 6A 04 58 6B C0 00 C7 80 (0@.....j.Xk....
004013C0 2C 30 40 00 02 00 00 00 6A 04 58 6B C0 00 8B 0D ,0@.....j.Xk....
004013D0 04 30 40 00 89 4C 05 F8 6A 04 58 C1 E0 00 8B 0D .0@..L..j.X.....
004013E0 00 30 40 00 89 4C 05 F8 68 10 21 40 00 E8 E1 FE .0@..L..h.!@....
004013F0 FF FF 8B E5 5D C3                               ....].         

;; fn004013F6: 004013F6
;;   Called from:
;;     00401588 (in fn0040153F)
fn004013F6 proc
	push	ebp
	mov	ebp,esp
	mov	eax,[ebp+08]
	push	esi
	mov	ecx,[eax+3C]
	add	ecx,eax
	movzx	eax,word ptr [ecx+14]
	lea	edx,[ecx+18]
	add	edx,eax
	movzx	eax,word ptr [ecx+06]
	imul	esi,eax,28
	add	esi,edx
	cmp	edx,esi
	jz	00401431

l00401418:
	mov	ecx,[ebp+0C]

l0040141B:
	cmp	ecx,[edx+0C]
	jc	0040142A

l00401420:
	mov	eax,[edx+08]
	add	eax,[edx+0C]
	cmp	ecx,eax
	jc	00401436

l0040142A:
	add	edx,28
	cmp	edx,esi
	jnz	0040141B

l00401431:
	xor	eax,eax

l00401433:
	pop	esi
	pop	ebp
	ret

l00401436:
	mov	eax,edx
	jmp	00401433

;; fn0040143A: 0040143A
;;   Called from:
;;     00401182 (in Win32CrtStartup)
fn0040143A proc
	call	00401B98
	test	eax,eax
	jnz	00401446

l00401443:
	xor	al,al
	ret

l00401446:
	mov	eax,fs:[00000018]
	push	esi
	mov	esi,00403338
	mov	edx,[eax+04]
	jmp	0040145B

l00401457:
	cmp	edx,eax
	jz	0040146B

l0040145B:
	xor	eax,eax
	mov	ecx,edx
	lock
	cmpxchg	[esi],ecx
	test	eax,eax
	jnz	00401457

l00401467:
	xor	al,al
	pop	esi
	ret

l0040146B:
	mov	al,01
	pop	esi
	ret

;; fn0040146F: 0040146F
;;   Called from:
;;     00401168 (in Win32CrtStartup)
fn0040146F proc
	push	ebp
	mov	ebp,esp
	cmp	dword ptr [ebp+08],00
	jnz	0040147F

l00401478:
	mov	byte ptr [00403354],01

l0040147F:
	call	004019FE
	call	00401C46
	test	al,al
	jnz	00401491

l0040148D:
	xor	al,al
	pop	ebp
	ret

l00401491:
	call	00401C46
	test	al,al
	jnz	004014A4

l0040149A:
	push	00
	call	00401C46
	pop	ecx
	jmp	0040148D

l004014A4:
	mov	al,01
	pop	ebp
	ret
004014A8                         55 8B EC 83 EC 0C 56 8B         U.....V.
004014B0 75 08 85 F6 74 05 83 FE 01 75 7C E8 D8 06 00 00 u...t....u|.....
004014C0 85 C0 74 2A 85 F6 75 26 68 3C 33 40 00 E8 50 07 ..t*..u&h<3@..P.
004014D0 00 00 59 85 C0 74 04 32 C0 EB 57 68 48 33 40 00 ..Y..t.2..WhH3@.
004014E0 E8 3D 07 00 00 F7 D8 59 1A C0 FE C0 EB 44 A1 04 .=.....Y.....D..
004014F0 30 40 00 8D 75 F4 57 83 E0 1F BF 3C 33 40 00 6A 0@..u.W....<3@.j
00401500 20 59 2B C8 83 C8 FF D3 C8 33 05 04 30 40 00 89  Y+......3..0@..
00401510 45 F4 89 45 F8 89 45 FC A5 A5 A5 BF 48 33 40 00 E..E..E.....H3@.
00401520 89 45 F4 89 45 F8 8D 75 F4 89 45 FC B0 01 A5 A5 .E..E..u..E.....
00401530 A5 5F 5E 8B E5 5D C3 6A 05 E8 2F 02 00 00 CC    ._^..].j../....

;; fn0040153F: 0040153F
;;   Called from:
;;     004011FF (in Win32CrtStartup)
;;     00401224 (in Win32CrtStartup)
fn0040153F proc
	push	08
	push	00402528
	call	00401980
	and	dword ptr [ebp-04],00
	mov	eax,00005A4D
	cmp	[00400000],ax
	jnz	004015BA

l0040155D:
	mov	eax,[0040003C]
	cmp	dword ptr [eax+00400000],00004550
	jnz	004015BA

l0040156E:
	mov	ecx,0000010B
	cmp	[eax+00400018],cx
	jnz	004015BA

l0040157C:
	mov	eax,[ebp+08]
	mov	ecx,00400000
	sub	eax,ecx
	push	eax
	push	ecx
	call	004013F6
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	004015BA

l00401593:
	cmp	dword ptr [eax+24],00
	jl	004015BA

l00401599:
	mov	dword ptr [ebp-04],FFFFFFFE
	mov	al,01
	jmp	004015C3
004015A4             8B 45 EC 8B 00 33 C9 81 38 05 00 00     .E...3..8...
004015B0 C0 0F 94 C1 8B C1 C3 8B 65 E8                   ........e.     

l004015BA:
	mov	dword ptr [ebp-04],FFFFFFFE
	xor	al,al

l004015C3:
	call	004019C6
	ret

;; fn004015C9: 004015C9
;;   Called from:
;;     004011EB (in Win32CrtStartup)
fn004015C9 proc
	push	ebp
	mov	ebp,esp
	call	00401B98
	test	eax,eax
	jz	004015E4

l004015D5:
	cmp	byte ptr [ebp+08],00
	jnz	004015E4

l004015DB:
	xor	eax,eax
	mov	ecx,00403338
	xchg	[ecx],eax

l004015E4:
	pop	ebp
	ret

;; fn004015E6: 004015E6
;;   Called from:
;;     00401274 (in Win32CrtStartup)
fn004015E6 proc
	push	ebp
	mov	ebp,esp
	cmp	byte ptr [00403354],00
	jz	004015F8

l004015F2:
	cmp	byte ptr [ebp+0C],00
	jnz	0040160A

l004015F8:
	push	dword ptr [ebp+08]
	call	00401C46
	push	dword ptr [ebp+08]
	call	00401C46
	pop	ecx
	pop	ecx

l0040160A:
	mov	al,01
	pop	ebp
	ret
0040160E                                           55 8B               U.
00401610 EC A1 04 30 40 00 8B C8 33 05 3C 33 40 00 83 E1 ...0@...3.<3@...
00401620 1F FF 75 08 D3 C8 83 F8 FF 75 07 E8 FE 05 00 00 ..u......u......
00401630 EB 0B 68 3C 33 40 00 E8 EC 05 00 00 59 F7 D8 59 ..h<3@......Y..Y
00401640 1B C0 F7 D0 23 45 08 5D C3 55 8B EC FF 75 08 E8 ....#E.].U...u..
00401650 BA FF FF FF F7 D8 59 1B C0 F7 D8 48 5D C3       ......Y....H]. 

;; fn0040165E: 0040165E
;;   Called from:
;;     004012C9 (in Win32CrtStartup)
fn0040165E proc
	push	ebp
	mov	ebp,esp
	sub	esp,14
	and	dword ptr [ebp-0C],00
	and	dword ptr [ebp-08],00
	mov	eax,[00403004]
	push	esi
	push	edi
	mov	edi,BB40E64E
	mov	esi,FFFF0000
	cmp	eax,edi
	jz	0040168E

l00401681:
	test	esi,eax
	jz	0040168E

l00401685:
	not	eax
	mov	[00403000],eax
	jmp	004016F4

l0040168E:
	lea	eax,[ebp-0C]
	push	eax
	call	dword ptr [0040200C]
	mov	eax,[ebp-08]
	xor	eax,[ebp-0C]
	mov	[ebp-04],eax
	call	dword ptr [00402020]
	xor	[ebp-04],eax
	call	dword ptr [00402024]
	xor	[ebp-04],eax
	lea	eax,[ebp-14]
	push	eax
	call	dword ptr [00402028]
	mov	ecx,[ebp-10]
	lea	eax,[ebp-04]
	xor	ecx,[ebp-14]
	xor	ecx,[ebp-04]
	xor	ecx,eax
	cmp	ecx,edi
	jnz	004016D6

l004016CF:
	mov	ecx,BB40E64F
	jmp	004016E6

l004016D6:
	test	esi,ecx
	jnz	004016E6

l004016DA:
	mov	eax,ecx
	or	eax,00004711
	shl	eax,10
	or	ecx,eax

l004016E6:
	mov	[00403004],ecx
	not	ecx
	mov	[00403000],ecx

l004016F4:
	pop	edi
	pop	esi
	mov	esp,ebp
	pop	ebp
	ret
004016FA                               33 C0 40 C3 B8 00           3.@...
00401700 40 00 00 C3 68 58 33 40 00 FF 15 08 20 40 00 C3 @...hX3@.... @..
00401710 68 00 00 03 00 68 00 00 01 00 6A 00 E8 13 05 00 h....h....j.....
00401720 00 83 C4 0C 85 C0 75 01 C3 6A 07 E8 3D 00 00 00 ......u..j..=...
00401730 CC C3 B8 60 33 40 00 C3 E8 C3 F8 FF FF 8B 48 04 ...`3@........H.
00401740 83 08 04 89 48 04 E8 E7 FF FF FF 8B 48 04 83 08 ....H.......H...
00401750 02 89 48 04 C3 33 C0 39 05 0C 30 40 00 0F 94 C0 ..H..3.9..0@....
00401760 C3                                              .              

;; fn00401761: 00401761
;;   Called from:
;;     004011F1 (in Win32CrtStartup)
fn00401761 proc
	mov	eax,00403384
	ret

;; fn00401767: 00401767
;;   Called from:
;;     00401218 (in Win32CrtStartup)
fn00401767 proc
	mov	eax,00403380
	ret

;; fn0040176D: 0040176D
;;   Called from:
;;     00401174 (in Win32CrtStartup)
fn0040176D proc
	push	ebp
	mov	ebp,esp
	sub	esp,00000324
	push	ebx
	push	esi
	push	17
	call	00401C40
	test	eax,eax
	jz	00401788

l00401783:
	mov	ecx,[ebp+08]
	int	29

l00401788:
	xor	esi,esi
	lea	eax,[ebp-00000324]
	push	000002CC
	push	esi
	push	eax
	mov	[00403368],esi
	call	00401BA4
	add	esp,0C
	mov	[ebp-00000274],eax
	mov	[ebp-00000278],ecx
	mov	[ebp-0000027C],edx
	mov	[ebp-00000280],ebx
	mov	[ebp-00000284],esi
	mov	[ebp-00000288],edi
	mov	[ebp-0000025C],ss
	mov	[ebp-00000268],cs
	mov	[ebp-0000028C],ds
	mov	[ebp-00000290],es
	mov	[ebp-00000294],fs
	mov	[ebp-00000298],gs
	pushf
	pop	dword ptr [ebp-00000264]
	mov	eax,[ebp+04]
	mov	[ebp-0000026C],eax
	lea	eax,[ebp+04]
	mov	[ebp-00000260],eax
	mov	dword ptr [ebp-00000324],00010001
	mov	eax,[eax-04]
	push	50
	mov	[ebp-00000270],eax
	lea	eax,[ebp-58]
	push	esi
	push	eax
	call	00401BA4
	mov	eax,[ebp+04]
	add	esp,0C
	mov	dword ptr [ebp-58],40000015
	mov	dword ptr [ebp-54],00000001
	mov	[ebp-4C],eax
	call	dword ptr [00402004]
	push	esi
	lea	ebx,[eax-01]
	neg	ebx
	lea	eax,[ebp-58]
	mov	[ebp-08],eax
	lea	eax,[ebp-00000324]
	sbb	bl,bl
	mov	[ebp-04],eax
	inc	bl
	call	dword ptr [00402010]
	lea	eax,[ebp-08]
	push	eax
	call	dword ptr [0040201C]
	test	eax,eax
	jnz	00401882

l00401875:
	movzx	eax,bl
	neg	eax
	sbb	eax,eax
	and	[00403368],eax

l00401882:
	pop	esi
	pop	ebx
	mov	esp,ebp
	pop	ebp
	ret
00401888                         33 C0 C3                        3..    

;; fn0040188B: 0040188B
;;   Called from:
;;     00401258 (in Win32CrtStartup)
fn0040188B proc
	push	00
	call	dword ptr [00402000]
	mov	ecx,eax
	test	ecx,ecx
	jnz	0040189C

l00401899:
	xor	al,al
	ret

l0040189C:
	mov	eax,00005A4D
	cmp	[ecx],ax
	jnz	00401899

l004018A6:
	mov	eax,[ecx+3C]
	add	eax,ecx
	cmp	dword ptr [eax],00004550
	jnz	00401899

l004018B3:
	mov	ecx,0000010B
	cmp	[eax+18],cx
	jnz	00401899

l004018BE:
	cmp	dword ptr [eax+74],0E
	jbe	00401899

l004018C4:
	cmp	dword ptr [eax+000000E8],00
	setnz	al
	ret
004018CF                                              68                h
004018D0 DB 18 40 00 FF 15 10 20 40 00 C3 55 8B EC 8B 45 ..@.... @..U...E
004018E0 08 8B 00 81 38 63 73 6D E0 75 25 83 78 10 03 75 ....8csm.u%.x..u
004018F0 1F 8B 40 14 3D 20 05 93 19 74 1B 3D 21 05 93 19 ..@.= ...t.=!...
00401900 74 14 3D 22 05 93 19 74 0D 3D 00 40 99 01 74 06 t.="...t.=.@..t.
00401910 33 C0 5D C2 04 00 E8 1F 03 00 00 CC 53 56 BE FC 3.].........SV..
00401920 24 40 00 BB FC 24 40 00 3B F3 73 18 57 8B 3E 85 $@...$@.;.s.W.>.
00401930 FF 74 09 8B CF E8 38 00 00 00 FF D7 83 C6 04 3B .t....8........;
00401940 F3 72 EA 5F 5E 5B C3 53 56 BE 04 25 40 00 BB 04 .r._^[.SV..%@...
00401950 25 40 00 3B F3 73 18 57 8B 3E 85 FF 74 09 8B CF %@.;.s.W.>..t...
00401960 E8 0D 00 00 00 FF D7 83 C6 04 3B F3 72 EA 5F 5E ..........;.r._^
00401970 5B C3                                           [.             

;; fn00401972: 00401972
;;   Called from:
;;     00401211 (in Win32CrtStartup)
fn00401972 proc
	jmp	dword ptr [004020D4]
00401978                         CC CC CC CC CC CC CC CC         ........

;; fn00401980: 00401980
;;   Called from:
;;     00401161 (in Win32CrtStartup)
;;     00401546 (in fn0040153F)
fn00401980 proc
	push	004019DB
	push	dword ptr fs:[00000000]
	mov	eax,[esp+10]
	mov	[esp+10],ebp
	lea	ebp,[esp+10]
	sub	esp,eax
	push	ebx
	push	esi
	push	edi
	mov	eax,[00403004]
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
	repne ret

;; fn004019C6: 004019C6
;;   Called from:
;;     004012C3 (in Win32CrtStartup)
;;     004015C3 (in fn0040153F)
fn004019C6 proc
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
	repne ret
004019DB                                  55 8B EC FF 75            U...u
004019E0 14 FF 75 10 FF 75 0C FF 75 08 68 8B 10 40 00 68 ..u..u..u.h..@.h
004019F0 04 30 40 00 E8 B1 01 00 00 83 C4 18 5D C3       .0@.........]. 

;; fn004019FE: 004019FE
;;   Called from:
;;     0040147F (in fn0040146F)
fn004019FE proc
	push	ebp
	mov	ebp,esp
	and	dword ptr [0040336C],00
	sub	esp,28
	push	ebx
	xor	ebx,ebx
	inc	ebx
	or	[00403010],ebx
	push	0A
	call	00401C40
	test	eax,eax
	jz	00401B91

l00401A24:
	and	dword ptr [ebp-10],00
	xor	eax,eax
	or	dword ptr [00403010],02
	xor	ecx,ecx
	push	esi
	push	edi
	mov	[0040336C],ebx
	lea	edi,[ebp-28]
	push	ebx
	cpuid
	mov	esi,ebx
	pop	ebx
	mov	[edi],eax
	mov	[edi+04],esi
	mov	[edi+08],ecx
	mov	[edi+0C],edx
	mov	eax,[ebp-28]
	mov	ecx,[ebp-1C]
	mov	[ebp-08],eax
	xor	ecx,49656E69
	mov	eax,[ebp-20]
	xor	eax,6C65746E
	or	ecx,eax
	mov	eax,[ebp-24]
	push	01
	xor	eax,756E6547
	or	ecx,eax
	pop	eax
	push	00
	pop	ecx
	push	ebx
	cpuid
	mov	esi,ebx
	pop	ebx
	mov	[edi],eax
	mov	[edi+04],esi
	mov	[edi+08],ecx
	mov	[edi+0C],edx
	jnz	00401ACE

l00401A8B:
	mov	eax,[ebp-28]
	and	eax,0FFF3FF0
	cmp	eax,000106C0
	jz	00401ABD

l00401A9A:
	cmp	eax,00020660
	jz	00401ABD

l00401AA1:
	cmp	eax,00020670
	jz	00401ABD

l00401AA8:
	cmp	eax,00030650
	jz	00401ABD

l00401AAF:
	cmp	eax,00030660
	jz	00401ABD

l00401AB6:
	cmp	eax,00030670
	jnz	00401ACE

l00401ABD:
	mov	edi,[00403370]
	or	edi,01
	mov	[00403370],edi
	jmp	00401AD4

l00401ACE:
	mov	edi,[00403370]

l00401AD4:
	cmp	dword ptr [ebp-08],07
	mov	eax,[ebp-1C]
	mov	[ebp-18],eax
	mov	eax,[ebp-20]
	mov	[ebp-04],eax
	mov	[ebp-14],eax
	jl	00401B1B

l00401AE9:
	push	07
	pop	eax
	xor	ecx,ecx
	push	ebx
	cpuid
	mov	esi,ebx
	pop	ebx
	lea	ebx,[ebp-28]
	mov	[ebx],eax
	mov	[ebx+04],esi
	mov	[ebx+08],ecx
	mov	[ebx+0C],edx
	mov	eax,[ebp-24]
	test	eax,00000200
	mov	[ebp-10],eax
	mov	eax,[ebp-04]
	jz	00401B1B

l00401B12:
	or	edi,02
	mov	[00403370],edi

l00401B1B:
	pop	edi
	pop	esi
	test	eax,00100000
	jz	00401B91

l00401B24:
	or	dword ptr [00403010],04
	mov	dword ptr [0040336C],00000002
	test	eax,08000000
	jz	00401B91

l00401B3C:
	test	eax,10000000
	jz	00401B91

l00401B43:
	xor	ecx,ecx
	xgetbv
	mov	[ebp-0C],eax
	mov	[ebp-08],edx
	mov	eax,[ebp-0C]
	mov	ecx,[ebp-08]
	and	eax,06
	xor	ecx,ecx
	cmp	eax,06
	jnz	00401B91

l00401B5E:
	test	ecx,ecx
	jnz	00401B91

l00401B62:
	mov	eax,[00403010]
	or	eax,08
	mov	dword ptr [0040336C],00000003
	test	byte ptr [ebp-10],20
	mov	[00403010],eax
	jz	00401B91

l00401B7F:
	or	eax,20
	mov	dword ptr [0040336C],00000005
	mov	[00403010],eax

l00401B91:
	xor	eax,eax
	pop	ebx
	mov	esp,ebp
	pop	ebp
	ret

;; fn00401B98: 00401B98
;;   Called from:
;;     0040143A (in fn0040143A)
;;     004015CC (in fn004015C9)
fn00401B98 proc
	xor	eax,eax
	cmp	[00403014],eax
	setnz	al
	ret
00401BA4             FF 25 34 20 40 00 FF 25 38 20 40 00     .%4 @..%8 @.
00401BB0 FF 25 A0 20 40 00 FF 25 70 20 40 00 FF 25 50 20 .%. @..%p @..%P 
00401BC0 40 00 FF 25 8C 20 40 00 FF 25 9C 20 40 00 FF 25 @..%. @..%. @..%
00401BD0 98 20 40 00 FF 25 94 20 40 00 FF 25 6C 20 40 00 . @..%. @..%l @.
00401BE0 FF 25 90 20 40 00 FF 25 74 20 40 00 FF 25 A8 20 .%. @..%t @..%. 
00401BF0 40 00 FF 25 68 20 40 00 FF 25 58 20 40 00 FF 25 @..%h @..%X @..%
00401C00 5C 20 40 00 FF 25 60 20 40 00 FF 25 64 20 40 00 \ @..%` @..%d @.
00401C10 FF 25 48 20 40 00 FF 25 40 20 40 00 FF 25 AC 20 .%H @..%@ @..%. 
00401C20 40 00 FF 25 78 20 40 00 FF 25 7C 20 40 00 FF 25 @..%x @..%| @..%
00401C30 80 20 40 00 FF 25 84 20 40 00 FF 25 88 20 40 00 . @..%. @..%. @.
00401C40 FF 25 2C 20 40 00                               .%, @.         

;; fn00401C46: 00401C46
;;   Called from:
;;     00401484 (in fn0040146F)
;;     00401491 (in fn0040146F)
;;     0040149C (in fn0040146F)
;;     004015FB (in fn004015E6)
;;     00401603 (in fn004015E6)
fn00401C46 proc
	mov	al,01
	ret
;;; Segment .rdata (00402000)
__imp__GetModuleHandleW		; 00402000
	dd	0x00002AA8
__imp__IsDebuggerPresent		; 00402004
	dd	0x00002A94
__imp__InitializeSListHead		; 00402008
	dd	0x00002A7E
__imp__GetSystemTimeAsFileTime		; 0040200C
	dd	0x00002A64
__imp__SetUnhandledExceptionFilter		; 00402010
	dd	0x000029BC
__imp__GetCurrentProcess		; 00402014
	dd	0x000029DA
__imp__TerminateProcess		; 00402018
	dd	0x000029EE
__imp__UnhandledExceptionFilter		; 0040201C
	dd	0x000029A0
__imp__GetCurrentThreadId		; 00402020
	dd	0x00002A4E
__imp__GetCurrentProcessId		; 00402024
	dd	0x00002A38
__imp__QueryPerformanceCounter		; 00402028
	dd	0x00002A1E
__imp__IsProcessorFeaturePresent		; 0040202C
	dd	0x00002A02
00402030 00 00 00 00                                     ....           
__imp__memset		; 00402034
	dd	0x000026D8
__imp___except_handler4_common		; 00402038
	dd	0x000026E2
0040203C                                     00 00 00 00             ....
__imp___set_new_mode		; 00402040
	dd	0x0000287A
00402044             00 00 00 00                             ....       
__imp___configthreadlocale		; 00402048
	dd	0x00002864
0040204C                                     00 00 00 00             ....
__imp____setusermatherr		; 00402050
	dd	0x0000275C
00402054             00 00 00 00                             ....       
__imp____p___argv		; 00402058
	dd	0x00002814
__imp___cexit		; 0040205C
	dd	0x00002822
__imp___c_exit		; 00402060
	dd	0x0000282C
__imp___register_thread_local_exe_atexit_callback		; 00402064
	dd	0x00002836
__imp____p___argc		; 00402068
	dd	0x00002806
__imp___initterm_e		; 0040206C
	dd	0x000027DA
__imp___set_app_type		; 00402070
	dd	0x0000274C
__imp___exit		; 00402074
	dd	0x000027F0
__imp___initialize_onexit_table		; 00402078
	dd	0x0000289A
__imp___register_onexit_function		; 0040207C
	dd	0x000028B6
__imp___crt_atexit		; 00402080
	dd	0x000028D2
__imp___controlfp_s		; 00402084
	dd	0x000028E0
__imp__terminate		; 00402088
	dd	0x000028F0
__imp___configure_narrow_argv		; 0040208C
	dd	0x00002770
__imp__exit		; 00402090
	dd	0x000027E8
__imp___initterm		; 00402094
	dd	0x000027CE
__imp___get_initial_narrow_environment		; 00402098
	dd	0x000027AC
__imp___initialize_narrow_environment		; 0040209C
	dd	0x0000278A
__imp___seh_filter_exe		; 004020A0
	dd	0x0000273A
004020A4             00 00 00 00                             ....       
__imp___set_fmode		; 004020A8
	dd	0x000027F8
__imp____p__commode		; 004020AC
	dd	0x0000288A
__imp____stdio_common_vfprintf		; 004020B0
	dd	0x00002720
__imp____acrt_iob_func		; 004020B4
	dd	0x0000270E
004020B8                         00 00 00 00                     ....   
__imp__driver.dll_1		; 004020BC
	dd	0x80000001
__imp__driver.dll_2		; 004020C0
	dd	0x80000002
__imp__driver.dll_4		; 004020C4
	dd	0x80000004
__imp__driver.dll_3		; 004020C8
	dd	0x80000003
__imp__driver.dll_11		; 004020CC
	dd	0x8000000B
004020D0 00 00 00 00 31 17 40 00 00 00 00 00 48 11 40 00 ....1.@.....H.@.
004020E0 00 00 00 00 00 00 00 00 9C 10 40 00 40 11 40 00 ..........@.@.@.
004020F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00402110 18 30 40 00 68 30 40 00 6D 61 74 68 3A 20 63 6F .0@.h0@.math: co
00402120 75 6E 74 3A 20 25 64 2C 20 61 63 63 75 6D 20 25 unt: %d, accum %
00402130 64 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 d...............
00402140 00 00 00 00 1E 14 99 58 00 00 00 00 02 00 00 00 .......X........
00402150 66 00 00 00 14 22 00 00 14 14 00 00 00 00 00 00 f...."..........
00402160 1E 14 99 58 00 00 00 00 0C 00 00 00 14 00 00 00 ...X............
00402170 7C 22 00 00 7C 14 00 00 00 00 00 00 1E 14 99 58 |"..|..........X
00402180 00 00 00 00 0D 00 00 00 68 02 00 00 90 22 00 00 ........h...."..
00402190 90 14 00 00 00 00 00 00 1E 14 99 58 00 00 00 00 ...........X....
004021A0 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
004021B0 5C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 \...............
004021C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
004021E0 00 00 00 00 00 00 00 00 00 00 00 00 04 30 40 00 .............0@.
004021F0 10 22 40 00 01 00 00 00 D4 20 40 00 00 00 00 00 ."@...... @.....
00402200 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 ................
00402210 DB 19 00 00 52 53 44 53 9A 54 4E 86 28 61 EB 44 ....RSDS.TN.(a.D
00402220 86 89 94 68 97 67 8E 40 01 00 00 00 44 3A 5C 64 ...h.g.@....D:\d
00402230 65 76 5C 75 78 6D 61 6C 5C 72 65 6B 6F 5C 6D 61 ev\uxmal\reko\ma
00402240 73 74 65 72 5C 73 75 62 6A 65 63 74 73 5C 50 45 ster\subjects\PE
00402250 5C 78 38 36 5C 69 6D 70 6F 72 74 2D 6F 72 64 69 \x86\import-ordi
00402260 6E 61 6C 73 5C 73 72 63 5C 52 65 6C 65 61 73 65 nals\src\Release
00402270 5C 6D 61 69 6E 2E 70 64 62 00 00 00 00 00 00 00 \main.pdb.......
00402280 20 00 00 00 20 00 00 00 00 00 00 00 1F 00 00 00  ... ...........
00402290 47 43 54 4C 00 10 00 00 49 0C 00 00 2E 74 65 78 GCTL....I....tex
004022A0 74 24 6D 6E 00 00 00 00 00 20 00 00 D4 00 00 00 t$mn..... ......
004022B0 2E 69 64 61 74 61 24 35 00 00 00 00 D4 20 00 00 .idata$5..... ..
004022C0 04 00 00 00 2E 30 30 63 66 67 00 00 D8 20 00 00 .....00cfg... ..
004022D0 04 00 00 00 2E 43 52 54 24 58 43 41 00 00 00 00 .....CRT$XCA....
004022E0 DC 20 00 00 04 00 00 00 2E 43 52 54 24 58 43 41 . .......CRT$XCA
004022F0 41 00 00 00 E0 20 00 00 04 00 00 00 2E 43 52 54 A.... .......CRT
00402300 24 58 43 5A 00 00 00 00 E4 20 00 00 04 00 00 00 $XCZ..... ......
00402310 2E 43 52 54 24 58 49 41 00 00 00 00 E8 20 00 00 .CRT$XIA..... ..
00402320 04 00 00 00 2E 43 52 54 24 58 49 41 41 00 00 00 .....CRT$XIAA...
00402330 EC 20 00 00 04 00 00 00 2E 43 52 54 24 58 49 41 . .......CRT$XIA
00402340 43 00 00 00 F0 20 00 00 04 00 00 00 2E 43 52 54 C.... .......CRT
00402350 24 58 49 5A 00 00 00 00 F4 20 00 00 04 00 00 00 $XIZ..... ......
00402360 2E 43 52 54 24 58 50 41 00 00 00 00 F8 20 00 00 .CRT$XPA..... ..
00402370 04 00 00 00 2E 43 52 54 24 58 50 5A 00 00 00 00 .....CRT$XPZ....
00402380 FC 20 00 00 04 00 00 00 2E 43 52 54 24 58 54 41 . .......CRT$XTA
00402390 00 00 00 00 00 21 00 00 10 00 00 00 2E 43 52 54 .....!.......CRT
004023A0 24 58 54 5A 00 00 00 00 10 21 00 00 00 01 00 00 $XTZ.....!......
004023B0 2E 72 64 61 74 61 00 00 10 22 00 00 04 00 00 00 .rdata..."......
004023C0 2E 72 64 61 74 61 24 73 78 64 61 74 61 00 00 00 .rdata$sxdata...
004023D0 14 22 00 00 E4 02 00 00 2E 72 64 61 74 61 24 7A .".......rdata$z
004023E0 7A 7A 64 62 67 00 00 00 F8 24 00 00 04 00 00 00 zzdbg....$......
004023F0 2E 72 74 63 24 49 41 41 00 00 00 00 FC 24 00 00 .rtc$IAA.....$..
00402400 04 00 00 00 2E 72 74 63 24 49 5A 5A 00 00 00 00 .....rtc$IZZ....
00402410 00 25 00 00 04 00 00 00 2E 72 74 63 24 54 41 41 .%.......rtc$TAA
00402420 00 00 00 00 04 25 00 00 04 00 00 00 2E 72 74 63 .....%.......rtc
00402430 24 54 5A 5A 00 00 00 00 08 25 00 00 3C 00 00 00 $TZZ.....%..<...
00402440 2E 78 64 61 74 61 24 78 00 00 00 00 44 25 00 00 .xdata$x....D%..
00402450 A0 00 00 00 2E 69 64 61 74 61 24 32 00 00 00 00 .....idata$2....
00402460 E4 25 00 00 14 00 00 00 2E 69 64 61 74 61 24 33 .%.......idata$3
00402470 00 00 00 00 F8 25 00 00 D4 00 00 00 2E 69 64 61 .....%.......ida
00402480 74 61 24 34 00 00 00 00 CC 26 00 00 FE 03 00 00 ta$4.....&......
00402490 2E 69 64 61 74 61 24 36 00 00 00 00 00 30 00 00 .idata$6.....0..
004024A0 18 00 00 00 2E 64 61 74 61 00 00 00 18 30 00 00 .....data....0..
004024B0 70 03 00 00 2E 62 73 73 00 00 00 00 00 40 00 00 p....bss.....@..
004024C0 20 00 00 00 2E 67 66 69 64 73 24 79 00 00 00 00  ....gfids$y....
004024D0 00 50 00 00 60 00 00 00 2E 72 73 72 63 24 30 31 .P..`....rsrc$01
004024E0 00 00 00 00 60 50 00 00 80 01 00 00 2E 72 73 72 ....`P.......rsr
004024F0 63 24 30 32 00 00 00 00 00 00 00 00 00 00 00 00 c$02............
00402500 00 00 00 00 00 00 00 00 FE FF FF FF 00 00 00 00 ................
00402510 CC FF FF FF 00 00 00 00 FE FF FF FF 86 12 40 00 ..............@.
00402520 9A 12 40 00 00 00 00 00 FE FF FF FF 00 00 00 00 ..@.............
00402530 D8 FF FF FF 00 00 00 00 FE FF FF FF A4 15 40 00 ..............@.
00402540 B7 15 40 00 B4 26 00 00 00 00 00 00 00 00 00 00 ..@..&..........
00402550 CC 26 00 00 BC 20 00 00 2C 26 00 00 00 00 00 00 .&... ..,&......
00402560 00 00 00 00 FC 26 00 00 34 20 00 00 A0 26 00 00 .....&..4 ...&..
00402570 00 00 00 00 00 00 00 00 FC 28 00 00 A8 20 00 00 .........(... ..
00402580 50 26 00 00 00 00 00 00 00 00 00 00 1C 29 00 00 P&...........)..
00402590 58 20 00 00 48 26 00 00 00 00 00 00 00 00 00 00 X ..H&..........
004025A0 3E 29 00 00 50 20 00 00 40 26 00 00 00 00 00 00 >)..P ..@&......
004025B0 00 00 00 00 5E 29 00 00 48 20 00 00 38 26 00 00 ....^)..H ..8&..
004025C0 00 00 00 00 00 00 00 00 80 29 00 00 40 20 00 00 .........)..@ ..
004025D0 F8 25 00 00 00 00 00 00 00 00 00 00 BC 2A 00 00 .%...........*..
004025E0 00 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 . ..............
004025F0 00 00 00 00 00 00 00 00                         ........       
l004025F8	dd	0x00002AA8
l004025FC	dd	0x00002A94
l00402600	dd	0x00002A7E
l00402604	dd	0x00002A64
l00402608	dd	0x000029BC
l0040260C	dd	0x000029DA
l00402610	dd	0x000029EE
l00402614	dd	0x000029A0
l00402618	dd	0x00002A4E
l0040261C	dd	0x00002A38
l00402620	dd	0x00002A1E
l00402624	dd	0x00002A02
00402628                         00 00 00 00                     ....   
l0040262C	dd	0x000026D8
l00402630	dd	0x000026E2
00402634             00 00 00 00                             ....       
l00402638	dd	0x0000287A
0040263C                                     00 00 00 00             ....
l00402640	dd	0x00002864
00402644             00 00 00 00                             ....       
l00402648	dd	0x0000275C
0040264C                                     00 00 00 00             ....
l00402650	dd	0x00002814
l00402654	dd	0x00002822
l00402658	dd	0x0000282C
l0040265C	dd	0x00002836
l00402660	dd	0x00002806
l00402664	dd	0x000027DA
l00402668	dd	0x0000274C
l0040266C	dd	0x000027F0
l00402670	dd	0x0000289A
l00402674	dd	0x000028B6
l00402678	dd	0x000028D2
l0040267C	dd	0x000028E0
l00402680	dd	0x000028F0
l00402684	dd	0x00002770
l00402688	dd	0x000027E8
l0040268C	dd	0x000027CE
l00402690	dd	0x000027AC
l00402694	dd	0x0000278A
l00402698	dd	0x0000273A
0040269C                                     00 00 00 00             ....
l004026A0	dd	0x000027F8
l004026A4	dd	0x0000288A
l004026A8	dd	0x00002720
l004026AC	dd	0x0000270E
004026B0 00 00 00 00                                     ....           
l004026B4	dd	0x80000001
l004026B8	dd	0x80000002
l004026BC	dd	0x80000004
l004026C0	dd	0x80000003
l004026C4	dd	0x8000000B
004026C8                         00 00 00 00 64 72 69 76         ....driv
004026D0 65 72 2E 64 6C 6C 00 00 48 00 6D 65 6D 73 65 74 er.dll..H.memset
004026E0 00 00 35 00 5F 65 78 63 65 70 74 5F 68 61 6E 64 ..5._except_hand
004026F0 6C 65 72 34 5F 63 6F 6D 6D 6F 6E 00 56 43 52 55 ler4_common.VCRU
00402700 4E 54 49 4D 45 31 34 30 2E 64 6C 6C 00 00 00 00 NTIME140.dll....
00402710 5F 5F 61 63 72 74 5F 69 6F 62 5F 66 75 6E 63 00 __acrt_iob_func.
00402720 03 00 5F 5F 73 74 64 69 6F 5F 63 6F 6D 6D 6F 6E ..__stdio_common
00402730 5F 76 66 70 72 69 6E 74 66 00 42 00 5F 73 65 68 _vfprintf.B._seh
00402740 5F 66 69 6C 74 65 72 5F 65 78 65 00 44 00 5F 73 _filter_exe.D._s
00402750 65 74 5F 61 70 70 5F 74 79 70 65 00 2E 00 5F 5F et_app_type...__
00402760 73 65 74 75 73 65 72 6D 61 74 68 65 72 72 00 00 setusermatherr..
00402770 19 00 5F 63 6F 6E 66 69 67 75 72 65 5F 6E 61 72 .._configure_nar
00402780 72 6F 77 5F 61 72 67 76 00 00 35 00 5F 69 6E 69 row_argv..5._ini
00402790 74 69 61 6C 69 7A 65 5F 6E 61 72 72 6F 77 5F 65 tialize_narrow_e
004027A0 6E 76 69 72 6F 6E 6D 65 6E 74 00 00 2A 00 5F 67 nvironment..*._g
004027B0 65 74 5F 69 6E 69 74 69 61 6C 5F 6E 61 72 72 6F et_initial_narro
004027C0 77 5F 65 6E 76 69 72 6F 6E 6D 65 6E 74 00 38 00 w_environment.8.
004027D0 5F 69 6E 69 74 74 65 72 6D 00 39 00 5F 69 6E 69 _initterm.9._ini
004027E0 74 74 65 72 6D 5F 65 00 58 00 65 78 69 74 00 00 tterm_e.X.exit..
004027F0 25 00 5F 65 78 69 74 00 54 00 5F 73 65 74 5F 66 %._exit.T._set_f
00402800 6D 6F 64 65 00 00 05 00 5F 5F 70 5F 5F 5F 61 72 mode....__p___ar
00402810 67 63 00 00 06 00 5F 5F 70 5F 5F 5F 61 72 67 76 gc....__p___argv
00402820 00 00 17 00 5F 63 65 78 69 74 00 00 16 00 5F 63 ...._cexit...._c
00402830 5F 65 78 69 74 00 3F 00 5F 72 65 67 69 73 74 65 _exit.?._registe
00402840 72 5F 74 68 72 65 61 64 5F 6C 6F 63 61 6C 5F 65 r_thread_local_e
00402850 78 65 5F 61 74 65 78 69 74 5F 63 61 6C 6C 62 61 xe_atexit_callba
00402860 63 6B 00 00 08 00 5F 63 6F 6E 66 69 67 74 68 72 ck...._configthr
00402870 65 61 64 6C 6F 63 61 6C 65 00 16 00 5F 73 65 74 eadlocale..._set
00402880 5F 6E 65 77 5F 6D 6F 64 65 00 01 00 5F 5F 70 5F _new_mode...__p_
00402890 5F 63 6F 6D 6D 6F 64 65 00 00 36 00 5F 69 6E 69 _commode..6._ini
004028A0 74 69 61 6C 69 7A 65 5F 6F 6E 65 78 69 74 5F 74 tialize_onexit_t
004028B0 61 62 6C 65 00 00 3E 00 5F 72 65 67 69 73 74 65 able..>._registe
004028C0 72 5F 6F 6E 65 78 69 74 5F 66 75 6E 63 74 69 6F r_onexit_functio
004028D0 6E 00 1F 00 5F 63 72 74 5F 61 74 65 78 69 74 00 n..._crt_atexit.
004028E0 1D 00 5F 63 6F 6E 74 72 6F 6C 66 70 5F 73 00 00 .._controlfp_s..
004028F0 6A 00 74 65 72 6D 69 6E 61 74 65 00 61 70 69 2D j.terminate.api-
00402900 6D 73 2D 77 69 6E 2D 63 72 74 2D 73 74 64 69 6F ms-win-crt-stdio
00402910 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 61 70 69 2D -l1-1-0.dll.api-
00402920 6D 73 2D 77 69 6E 2D 63 72 74 2D 72 75 6E 74 69 ms-win-crt-runti
00402930 6D 65 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 61 70 me-l1-1-0.dll.ap
00402940 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 2D 6D 61 74 i-ms-win-crt-mat
00402950 68 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 00 61 70 h-l1-1-0.dll..ap
00402960 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 2D 6C 6F 63 i-ms-win-crt-loc
00402970 61 6C 65 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 00 ale-l1-1-0.dll..
00402980 61 70 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 2D 68 api-ms-win-crt-h
00402990 65 61 70 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 00 eap-l1-1-0.dll..
004029A0 82 05 55 6E 68 61 6E 64 6C 65 64 45 78 63 65 70 ..UnhandledExcep
004029B0 74 69 6F 6E 46 69 6C 74 65 72 00 00 43 05 53 65 tionFilter..C.Se
004029C0 74 55 6E 68 61 6E 64 6C 65 64 45 78 63 65 70 74 tUnhandledExcept
004029D0 69 6F 6E 46 69 6C 74 65 72 00 09 02 47 65 74 43 ionFilter...GetC
004029E0 75 72 72 65 6E 74 50 72 6F 63 65 73 73 00 61 05 urrentProcess.a.
004029F0 54 65 72 6D 69 6E 61 74 65 50 72 6F 63 65 73 73 TerminateProcess
00402A00 00 00 6D 03 49 73 50 72 6F 63 65 73 73 6F 72 46 ..m.IsProcessorF
00402A10 65 61 74 75 72 65 50 72 65 73 65 6E 74 00 2D 04 eaturePresent.-.
00402A20 51 75 65 72 79 50 65 72 66 6F 72 6D 61 6E 63 65 QueryPerformance
00402A30 43 6F 75 6E 74 65 72 00 0A 02 47 65 74 43 75 72 Counter...GetCur
00402A40 72 65 6E 74 50 72 6F 63 65 73 73 49 64 00 0E 02 rentProcessId...
00402A50 47 65 74 43 75 72 72 65 6E 74 54 68 72 65 61 64 GetCurrentThread
00402A60 49 64 00 00 D6 02 47 65 74 53 79 73 74 65 6D 54 Id....GetSystemT
00402A70 69 6D 65 41 73 46 69 6C 65 54 69 6D 65 00 4B 03 imeAsFileTime.K.
00402A80 49 6E 69 74 69 61 6C 69 7A 65 53 4C 69 73 74 48 InitializeSListH
00402A90 65 61 64 00 67 03 49 73 44 65 62 75 67 67 65 72 ead.g.IsDebugger
00402AA0 50 72 65 73 65 6E 74 00 67 02 47 65 74 4D 6F 64 Present.g.GetMod
00402AB0 75 6C 65 48 61 6E 64 6C 65 57 00 00 4B 45 52 4E uleHandleW..KERN
00402AC0 45 4C 33 32 2E 64 6C 6C 00 00                   EL32.dll..     
;;; Segment .data (00403000)
00403000 B1 19 BF 44 4E E6 40 BB FF FF FF FF 01 00 00 00 ...DN.@.........
00403010 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 ................
00403020 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00403380 00 00 00 00 00 00 00 00                         ........       
;;; Segment .gfids (00404000)
00404000 0A 00 00 00 0D 00 00 00 10 00 00 00 45 00 00 00 ............E...
00404010 73 00 00 00 34 00 00 00 0C 00 00 00 0A 00 00 00 s...4...........
;;; Segment .rsrc (00405000)
00405000 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 ................
00405010 18 00 00 00 18 00 00 80 00 00 00 00 00 00 00 00 ................
00405020 00 00 00 00 00 00 01 00 01 00 00 00 30 00 00 80 ............0...
00405030 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 ................
00405040 09 04 00 00 48 00 00 00 60 50 00 00 7D 01 00 00 ....H...`P..}...
00405050 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00405060 3C 3F 78 6D 6C 20 76 65 72 73 69 6F 6E 3D 27 31 <?xml version='1
00405070 2E 30 27 20 65 6E 63 6F 64 69 6E 67 3D 27 55 54 .0' encoding='UT
00405080 46 2D 38 27 20 73 74 61 6E 64 61 6C 6F 6E 65 3D F-8' standalone=
00405090 27 79 65 73 27 3F 3E 0D 0A 3C 61 73 73 65 6D 62 'yes'?>..<assemb
004050A0 6C 79 20 78 6D 6C 6E 73 3D 27 75 72 6E 3A 73 63 ly xmlns='urn:sc
004050B0 68 65 6D 61 73 2D 6D 69 63 72 6F 73 6F 66 74 2D hemas-microsoft-
004050C0 63 6F 6D 3A 61 73 6D 2E 76 31 27 20 6D 61 6E 69 com:asm.v1' mani
004050D0 66 65 73 74 56 65 72 73 69 6F 6E 3D 27 31 2E 30 festVersion='1.0
004050E0 27 3E 0D 0A 20 20 3C 74 72 75 73 74 49 6E 66 6F '>..  <trustInfo
004050F0 20 78 6D 6C 6E 73 3D 22 75 72 6E 3A 73 63 68 65  xmlns="urn:sche
00405100 6D 61 73 2D 6D 69 63 72 6F 73 6F 66 74 2D 63 6F mas-microsoft-co
00405110 6D 3A 61 73 6D 2E 76 33 22 3E 0D 0A 20 20 20 20 m:asm.v3">..    
00405120 3C 73 65 63 75 72 69 74 79 3E 0D 0A 20 20 20 20 <security>..    
00405130 20 20 3C 72 65 71 75 65 73 74 65 64 50 72 69 76   <requestedPriv
00405140 69 6C 65 67 65 73 3E 0D 0A 20 20 20 20 20 20 20 ileges>..       
00405150 20 3C 72 65 71 75 65 73 74 65 64 45 78 65 63 75  <requestedExecu
00405160 74 69 6F 6E 4C 65 76 65 6C 20 6C 65 76 65 6C 3D tionLevel level=
00405170 27 61 73 49 6E 76 6F 6B 65 72 27 20 75 69 41 63 'asInvoker' uiAc
00405180 63 65 73 73 3D 27 66 61 6C 73 65 27 20 2F 3E 0D cess='false' />.
00405190 0A 20 20 20 20 20 20 3C 2F 72 65 71 75 65 73 74 .      </request
004051A0 65 64 50 72 69 76 69 6C 65 67 65 73 3E 0D 0A 20 edPrivileges>.. 
004051B0 20 20 20 3C 2F 73 65 63 75 72 69 74 79 3E 0D 0A    </security>..
004051C0 20 20 3C 2F 74 72 75 73 74 49 6E 66 6F 3E 0D 0A   </trustInfo>..
004051D0 3C 2F 61 73 73 65 6D 62 6C 79 3E 0D 0A 00 00 00 </assembly>.....
;;; Segment .reloc (00406000)
00406000 00 10 00 00 28 01 00 00 01 30 1B 30 36 30 49 30 ....(....0.060I0
00406010 54 30 5F 30 6A 30 6F 30 79 30 8D 30 D4 30 FD 30 T0_0j0o0y0.0.0.0
00406020 5D 31 8B 31 9C 31 A1 31 A6 31 C7 31 CC 31 D9 31 ]1.1.1.1.1.1.1.1
00406030 DA 32 E3 32 EE 32 F5 32 15 33 1B 33 21 33 27 33 .2.2.2.2.3.3!3'3
00406040 2D 33 33 33 3A 33 41 33 48 33 4F 33 56 33 5D 33 -333:3A3H3O3V3]3
00406050 64 33 6C 33 74 33 7C 33 88 33 91 33 96 33 9C 33 d3l3t3|3.3.3.3.3
00406060 A6 33 B0 33 C0 33 D0 33 E0 33 E9 33 4E 34 7A 34 .3.3.3.3.3.3N4z4
00406070 C9 34 DC 34 EF 34 FB 34 0B 35 1C 35 42 35 57 35 .4.4.4.4.5.5B5W5
00406080 5E 35 64 35 76 35 80 35 DE 35 EB 35 12 36 1A 36 ^5d5v5.5.5.5.6.6
00406090 33 36 6D 36 88 36 94 36 A3 36 AC 36 B9 36 E8 36 36m6.6.6.6.6.6.6
004060A0 F0 36 05 37 0B 37 33 37 59 37 62 37 68 37 99 37 .6.7.737Y7b7h7.7
004060B0 44 38 63 38 6D 38 7E 38 8F 38 D0 38 D6 38 1F 39 D8c8m8~8.8.8.8.9
004060C0 24 39 4A 39 4F 39 74 39 81 39 9E 39 EB 39 F0 39 $9J9O9t9.9.9.9.9
004060D0 03 3A 11 3A 2C 3A 37 3A BF 3A C8 3A D0 3A 17 3B .:.:,:7:.:.:.:.;
004060E0 26 3B 2D 3B 63 3B 6C 3B 79 3B 84 3B 8D 3B 9C 3B &;-;c;l;y;.;.;.;
004060F0 A6 3B AC 3B B2 3B B8 3B BE 3B C4 3B CA 3B D0 3B .;.;.;.;.;.;.;.;
00406100 D6 3B DC 3B E2 3B E8 3B EE 3B F4 3B FA 3B 00 3C .;.;.;.;.;.;.;.<
00406110 06 3C 0C 3C 12 3C 18 3C 1E 3C 24 3C 2A 3C 30 3C .<.<.<.<.<$<*<0<
00406120 36 3C 3C 3C 42 3C 00 00 00 20 00 00 24 00 00 00 6<<<B<... ..$...
00406130 D4 30 DC 30 E8 30 EC 30 10 31 14 31 EC 31 F0 31 .0.0.0.0.1.1.1.1
00406140 F8 31 1C 35 20 35 3C 35 40 35 00 00             .1.5 5<5@5..   
