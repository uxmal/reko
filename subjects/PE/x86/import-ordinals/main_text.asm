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
