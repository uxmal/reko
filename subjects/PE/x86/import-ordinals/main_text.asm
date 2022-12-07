;;; Segment .text (00401000)

;; fn00401000: 00401000
;;   Called from:
;;     0040102A (in fn00401010)
fn00401000 proc
	mov	eax,403378h
	ret
00401006                   CC CC CC CC CC CC CC CC CC CC       ..........

;; fn00401010: 00401010
;;   Called from:
;;     0040107D (in fn00401040)
fn00401010 proc
	push	ebp
	mov	ebp,esp
	push	esi
	mov	esi,[ebp+8h]
	push	1h
	call	dword ptr [4020B4h]
	add	esp,4h
	lea	ecx,[ebp+0Ch]
	push	ecx
	push	0h
	push	esi
	push	eax
	call	fn00401000
	push	dword ptr [eax+4h]
	push	dword ptr [eax]
	call	dword ptr [4020B0h]
	add	esp,18h
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
	lea	ecx,[ebp-4h]
	call	dword ptr [4020BCh]
	push	3h
	lea	ecx,[ebp-4h]
	call	dword ptr [4020C0h]
	push	1h
	lea	ecx,[ebp-4h]
	call	dword ptr [4020C4h]
	push	5h
	lea	ecx,[ebp-4h]
	call	dword ptr [4020C8h]
	mov	eax,[4020CCh]
	push	dword ptr [ebp-4h]
	push	dword ptr [eax]
	push	402118h
	call	fn00401010
	add	esp,0Ch
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
	push	14h
	push	402508h
	call	fn00401980
	push	1h
	call	fn0040146F
	pop	ecx
	test	al,al
	jnz	401179h

l00401172:
	push	7h
	call	fn0040176D

l00401179:
	xor	bl,bl
	mov	[ebp-19h],bl
	and	dword ptr [ebp-4h],0h
	call	fn0040143A
	mov	[ebp-24h],al
	mov	eax,[403334h]
	xor	ecx,ecx
	inc	ecx
	cmp	eax,ecx
	jz	401172h

l00401196:
	test	eax,eax
	jnz	4011E3h

l0040119A:
	mov	[403334h],ecx
	push	4020F0h
	push	4020E4h
	call	401BDAh
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	4011C6h

l004011B5:
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	mov	eax,0FFh
	jmp	4012C3h

l004011C6:
	push	4020E0h
	push	4020D8h
	call	401BD4h
	pop	ecx
	pop	ecx
	mov	dword ptr [403334h],2h
	jmp	4011E8h

l004011E3:
	mov	bl,cl
	mov	[ebp-19h],bl

l004011E8:
	push	dword ptr [ebp-24h]
	call	fn004015C9
	pop	ecx
	call	fn00401761
	mov	esi,eax
	xor	edi,edi
	cmp	[esi],edi
	jz	401218h

l004011FE:
	push	esi
	call	fn0040153F
	pop	ecx
	test	al,al
	jz	401218h

l00401209:
	push	edi
	push	2h
	push	edi
	mov	esi,[esi]
	mov	ecx,esi
	call	fn00401972
	call	esi

l00401218:
	call	fn00401767
	mov	esi,eax
	cmp	[esi],edi
	jz	401236h

l00401223:
	push	esi
	call	fn0040153F
	pop	ecx
	test	al,al
	jz	401236h

l0040122E:
	push	dword ptr [esi]
	call	401C0Ah
	pop	ecx

l00401236:
	call	401BF8h
	mov	edi,eax
	call	401BF2h
	mov	esi,eax
	call	401BCEh
	push	eax
	push	dword ptr [edi]
	push	dword ptr [esi]
	call	fn00401040
	add	esp,0Ch
	mov	esi,eax
	call	fn0040188B
	test	al,al
	jnz	401267h

l00401261:
	push	esi
	call	401BE0h

l00401267:
	test	bl,bl
	jnz	401270h

l0040126B:
	call	401BFEh

l00401270:
	push	0h
	push	1h
	call	fn004015E6
	pop	ecx
	pop	ecx
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	mov	eax,esi
	jmp	4012C3h
00401286                   8B 4D EC 8B 01 8B 00 89 45 E0       .M......E.
00401290 51 50 E8 19 09 00 00 59 59 C3 8B 65 E8 E8 E9 05 QP.....YY..e....
004012A0 00 00 84 C0 75 08 FF 75 E0 E8 38 09 00 00 80 7D ....u..u..8....}
004012B0 E7 00 75 05 E8 4B 09 00 00 C7 45 FC FE FF FF FF ..u..K....E.....
004012C0 8B 45 E0                                        .E.             

l004012C3:
	call	fn004019C6
	ret

;; Win32CrtStartup: 004012C9
Win32CrtStartup proc
	call	fn0040165E
	jmp	40115Ah
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
	mov	eax,[ebp+8h]
	push	esi
	mov	ecx,[eax+3Ch]
	add	ecx,eax
	movzx	eax,word ptr [ecx+14h]
	lea	edx,[ecx+18h]
	add	edx,eax
	movzx	eax,word ptr [ecx+6h]
	imul	esi,eax,28h
	add	esi,edx
	cmp	edx,esi
	jz	401431h

l00401418:
	mov	ecx,[ebp+0Ch]

l0040141B:
	cmp	ecx,[edx+0Ch]
	jc	40142Ah

l00401420:
	mov	eax,[edx+8h]
	add	eax,[edx+0Ch]
	cmp	ecx,eax
	jc	401436h

l0040142A:
	add	edx,28h
	cmp	edx,esi
	jnz	40141Bh

l00401431:
	xor	eax,eax

l00401433:
	pop	esi
	pop	ebp
	ret

l00401436:
	mov	eax,edx
	jmp	401433h

;; fn0040143A: 0040143A
;;   Called from:
;;     00401182 (in Win32CrtStartup)
fn0040143A proc
	call	fn00401B98
	test	eax,eax
	jnz	401446h

l00401443:
	xor	al,al
	ret

l00401446:
	mov	eax,fs:[0018h]
	push	esi
	mov	esi,403338h
	mov	edx,[eax+4h]
	jmp	40145Bh

l00401457:
	cmp	edx,eax
	jz	40146Bh

l0040145B:
	xor	eax,eax
	mov	ecx,edx
	lock
	cmpxchg	[esi],ecx
	test	eax,eax
	jnz	401457h

l00401467:
	xor	al,al
	pop	esi
	ret

l0040146B:
	mov	al,1h
	pop	esi
	ret

;; fn0040146F: 0040146F
;;   Called from:
;;     00401168 (in Win32CrtStartup)
fn0040146F proc
	push	ebp
	mov	ebp,esp
	cmp	dword ptr [ebp+8h],0h
	jnz	40147Fh

l00401478:
	mov	byte ptr [403354h],1h

l0040147F:
	call	fn004019FE
	call	fn00401C46
	test	al,al
	jnz	401491h

l0040148D:
	xor	al,al
	pop	ebp
	ret

l00401491:
	call	fn00401C46
	test	al,al
	jnz	4014A4h

l0040149A:
	push	0h
	call	fn00401C46
	pop	ecx
	jmp	40148Dh

l004014A4:
	mov	al,1h
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
	push	8h
	push	402528h
	call	fn00401980
	and	dword ptr [ebp-4h],0h
	mov	eax,5A4Dh
	cmp	[400000h],ax
	jnz	4015BAh

l0040155D:
	mov	eax,[40003Ch]
	cmp	dword ptr [eax+400000h],4550h
	jnz	4015BAh

l0040156E:
	mov	ecx,10Bh
	cmp	[eax+400018h],cx
	jnz	4015BAh

l0040157C:
	mov	eax,[ebp+8h]
	mov	ecx,400000h
	sub	eax,ecx
	push	eax
	push	ecx
	call	fn004013F6
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	4015BAh

l00401593:
	cmp	dword ptr [eax+24h],0h
	jl	4015BAh

l00401599:
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	mov	al,1h
	jmp	4015C3h
004015A4             8B 45 EC 8B 00 33 C9 81 38 05 00 00     .E...3..8...
004015B0 C0 0F 94 C1 8B C1 C3 8B 65 E8                   ........e.      

l004015BA:
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	xor	al,al

l004015C3:
	call	fn004019C6
	ret

;; fn004015C9: 004015C9
;;   Called from:
;;     004011EB (in Win32CrtStartup)
fn004015C9 proc
	push	ebp
	mov	ebp,esp
	call	fn00401B98
	test	eax,eax
	jz	4015E4h

l004015D5:
	cmp	byte ptr [ebp+8h],0h
	jnz	4015E4h

l004015DB:
	xor	eax,eax
	mov	ecx,403338h
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
	cmp	byte ptr [403354h],0h
	jz	4015F8h

l004015F2:
	cmp	byte ptr [ebp+0Ch],0h
	jnz	40160Ah

l004015F8:
	push	dword ptr [ebp+8h]
	call	fn00401C46
	push	dword ptr [ebp+8h]
	call	fn00401C46
	pop	ecx
	pop	ecx

l0040160A:
	mov	al,1h
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
	sub	esp,14h
	and	dword ptr [ebp-0Ch],0h
	and	dword ptr [ebp-8h],0h
	mov	eax,[403004h]
	push	esi
	push	edi
	mov	edi,0BB40E64Eh
	mov	esi,0FFFF0000h
	cmp	eax,edi
	jz	40168Eh

l00401681:
	test	esi,eax
	jz	40168Eh

l00401685:
	not	eax
	mov	[403000h],eax
	jmp	4016F4h

l0040168E:
	lea	eax,[ebp-0Ch]
	push	eax
	call	dword ptr [40200Ch]
	mov	eax,[ebp-8h]
	xor	eax,[ebp-0Ch]
	mov	[ebp-4h],eax
	call	dword ptr [402020h]
	xor	[ebp-4h],eax
	call	dword ptr [402024h]
	xor	[ebp-4h],eax
	lea	eax,[ebp-14h]
	push	eax
	call	dword ptr [402028h]
	mov	ecx,[ebp-10h]
	lea	eax,[ebp-4h]
	xor	ecx,[ebp-14h]
	xor	ecx,[ebp-4h]
	xor	ecx,eax
	cmp	ecx,edi
	jnz	4016D6h

l004016CF:
	mov	ecx,0BB40E64Fh
	jmp	4016E6h

l004016D6:
	test	esi,ecx
	jnz	4016E6h

l004016DA:
	mov	eax,ecx
	or	eax,4711h
	shl	eax,10h
	or	ecx,eax

l004016E6:
	mov	[403004h],ecx
	not	ecx
	mov	[403000h],ecx

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
	mov	eax,403384h
	ret

;; fn00401767: 00401767
;;   Called from:
;;     00401218 (in Win32CrtStartup)
fn00401767 proc
	mov	eax,403380h
	ret

;; fn0040176D: 0040176D
;;   Called from:
;;     00401174 (in Win32CrtStartup)
fn0040176D proc
	push	ebp
	mov	ebp,esp
	sub	esp,324h
	push	ebx
	push	esi
	push	17h
	call	401C40h
	test	eax,eax
	jz	401788h

l00401783:
	mov	ecx,[ebp+8h]
	int	29h

l00401788:
	xor	esi,esi
	lea	eax,[ebp-324h]
	push	2CCh
	push	esi
	push	eax
	mov	[403368h],esi
	call	401BA4h
	add	esp,0Ch
	mov	[ebp-274h],eax
	mov	[ebp-278h],ecx
	mov	[ebp-27Ch],edx
	mov	[ebp-280h],ebx
	mov	[ebp-284h],esi
	mov	[ebp-288h],edi
	mov	[ebp-25Ch],ss
	mov	[ebp-268h],cs
	mov	[ebp-28Ch],ds
	mov	[ebp-290h],es
	mov	[ebp-294h],fs
	mov	[ebp-298h],gs
	pushf
	pop	dword ptr [ebp-264h]
	mov	eax,[ebp+4h]
	mov	[ebp-26Ch],eax
	lea	eax,[ebp+4h]
	mov	[ebp-260h],eax
	mov	dword ptr [ebp-324h],10001h
	mov	eax,[eax-4h]
	push	50h
	mov	[ebp-270h],eax
	lea	eax,[ebp-58h]
	push	esi
	push	eax
	call	401BA4h
	mov	eax,[ebp+4h]
	add	esp,0Ch
	mov	dword ptr [ebp-58h],40000015h
	mov	dword ptr [ebp-54h],1h
	mov	[ebp-4Ch],eax
	call	dword ptr [402004h]
	push	esi
	lea	ebx,[eax-1h]
	neg	ebx
	lea	eax,[ebp-58h]
	mov	[ebp-8h],eax
	lea	eax,[ebp-324h]
	sbb	bl,bl
	mov	[ebp-4h],eax
	inc	bl
	call	dword ptr [402010h]
	lea	eax,[ebp-8h]
	push	eax
	call	dword ptr [40201Ch]
	test	eax,eax
	jnz	401882h

l00401875:
	movzx	eax,bl
	neg	eax
	sbb	eax,eax
	and	[403368h],eax

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
	push	0h
	call	dword ptr [402000h]
	mov	ecx,eax
	test	ecx,ecx
	jnz	40189Ch

l00401899:
	xor	al,al
	ret

l0040189C:
	mov	eax,5A4Dh
	cmp	[ecx],ax
	jnz	401899h

l004018A6:
	mov	eax,[ecx+3Ch]
	add	eax,ecx
	cmp	dword ptr [eax],4550h
	jnz	401899h

l004018B3:
	mov	ecx,10Bh
	cmp	[eax+18h],cx
	jnz	401899h

l004018BE:
	cmp	dword ptr [eax+74h],0Eh
	jbe	401899h

l004018C4:
	cmp	dword ptr [eax+0E8h],0h
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
	jmp	dword ptr [4020D4h]
00401978                         CC CC CC CC CC CC CC CC         ........

;; fn00401980: 00401980
;;   Called from:
;;     00401161 (in Win32CrtStartup)
;;     00401546 (in fn0040153F)
fn00401980 proc
	push	4019DBh
	push	dword ptr fs:[0000h]
	mov	eax,[esp+10h]
	mov	[esp+10h],ebp
	lea	ebp,[esp+10h]
	sub	esp,eax
	push	ebx
	push	esi
	push	edi
	mov	eax,[403004h]
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
	repne ret

;; fn004019C6: 004019C6
;;   Called from:
;;     004012C3 (in Win32CrtStartup)
;;     004015C3 (in fn0040153F)
fn004019C6 proc
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
	and	dword ptr [40336Ch],0h
	sub	esp,28h
	push	ebx
	xor	ebx,ebx
	inc	ebx
	or	[403010h],ebx
	push	0Ah
	call	401C40h
	test	eax,eax
	jz	401B91h

l00401A24:
	and	dword ptr [ebp-10h],0h
	xor	eax,eax
	or	dword ptr [403010h],2h
	xor	ecx,ecx
	push	esi
	push	edi
	mov	[40336Ch],ebx
	lea	edi,[ebp-28h]
	push	ebx
	cpuid
	mov	esi,ebx
	pop	ebx
	mov	[edi],eax
	mov	[edi+4h],esi
	mov	[edi+8h],ecx
	mov	[edi+0Ch],edx
	mov	eax,[ebp-28h]
	mov	ecx,[ebp-1Ch]
	mov	[ebp-8h],eax
	xor	ecx,49656E69h
	mov	eax,[ebp-20h]
	xor	eax,6C65746Eh
	or	ecx,eax
	mov	eax,[ebp-24h]
	push	1h
	xor	eax,756E6547h
	or	ecx,eax
	pop	eax
	push	0h
	pop	ecx
	push	ebx
	cpuid
	mov	esi,ebx
	pop	ebx
	mov	[edi],eax
	mov	[edi+4h],esi
	mov	[edi+8h],ecx
	mov	[edi+0Ch],edx
	jnz	401ACEh

l00401A8B:
	mov	eax,[ebp-28h]
	and	eax,0FFF3FF0h
	cmp	eax,106C0h
	jz	401ABDh

l00401A9A:
	cmp	eax,20660h
	jz	401ABDh

l00401AA1:
	cmp	eax,20670h
	jz	401ABDh

l00401AA8:
	cmp	eax,30650h
	jz	401ABDh

l00401AAF:
	cmp	eax,30660h
	jz	401ABDh

l00401AB6:
	cmp	eax,30670h
	jnz	401ACEh

l00401ABD:
	mov	edi,[403370h]
	or	edi,1h
	mov	[403370h],edi
	jmp	401AD4h

l00401ACE:
	mov	edi,[403370h]

l00401AD4:
	cmp	dword ptr [ebp-8h],7h
	mov	eax,[ebp-1Ch]
	mov	[ebp-18h],eax
	mov	eax,[ebp-20h]
	mov	[ebp-4h],eax
	mov	[ebp-14h],eax
	jl	401B1Bh

l00401AE9:
	push	7h
	pop	eax
	xor	ecx,ecx
	push	ebx
	cpuid
	mov	esi,ebx
	pop	ebx
	lea	ebx,[ebp-28h]
	mov	[ebx],eax
	mov	[ebx+4h],esi
	mov	[ebx+8h],ecx
	mov	[ebx+0Ch],edx
	mov	eax,[ebp-24h]
	test	eax,200h
	mov	[ebp-10h],eax
	mov	eax,[ebp-4h]
	jz	401B1Bh

l00401B12:
	or	edi,2h
	mov	[403370h],edi

l00401B1B:
	pop	edi
	pop	esi
	test	eax,100000h
	jz	401B91h

l00401B24:
	or	dword ptr [403010h],4h
	mov	dword ptr [40336Ch],2h
	test	eax,8000000h
	jz	401B91h

l00401B3C:
	test	eax,10000000h
	jz	401B91h

l00401B43:
	xor	ecx,ecx
	xgetbv
	mov	[ebp-0Ch],eax
	mov	[ebp-8h],edx
	mov	eax,[ebp-0Ch]
	mov	ecx,[ebp-8h]
	and	eax,6h
	xor	ecx,ecx
	cmp	eax,6h
	jnz	401B91h

l00401B5E:
	test	ecx,ecx
	jnz	401B91h

l00401B62:
	mov	eax,[403010h]
	or	eax,8h
	mov	dword ptr [40336Ch],3h
	test	byte ptr [ebp-10h],20h
	mov	[403010h],eax
	jz	401B91h

l00401B7F:
	or	eax,20h
	mov	dword ptr [40336Ch],5h
	mov	[403010h],eax

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
	cmp	[403014h],eax
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
	mov	al,1h
	ret
