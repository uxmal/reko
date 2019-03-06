;;; Segment .text (00401000)

;; fn00401000: 00401000
;;   Called from:
;;     00401253 (in Win32CrtStartup)
fn00401000 proc
	push	dword ptr [00402004]
	call	dword ptr [00402010]
	mov	eax,[00402008]
	push	dword ptr [eax]
	push	00402108
	call	00401060
	push	01
	call	dword ptr [00402000]
	push	eax
	push	00402108
	call	00401060
	mov	eax,[00402008]
	push	dword ptr [eax]
	push	00402108
	call	00401060
	add	esp,1C
	xor	eax,eax
	ret
00401047                      CC CC CC CC CC CC CC CC CC        .........

;; fn00401050: 00401050
;;   Called from:
;;     0040107A (in fn00401060)
;;     0040173F (in fn0040173F)
fn00401050 proc
	mov	eax,00403378
	ret
00401056                   CC CC CC CC CC CC CC CC CC CC       ..........

;; fn00401060: 00401060
;;   Called from:
;;     00401018 (in fn00401000)
;;     0040102B (in fn00401000)
;;     0040103C (in fn00401000)
fn00401060 proc
	push	ebp
	mov	ebp,esp
	push	esi
	mov	esi,[ebp+08]
	push	01
	call	dword ptr [004020C8]
	add	esp,04
	lea	ecx,[ebp+0C]
	push	ecx
	push	00
	push	esi
	push	eax
	call	00401050
	push	dword ptr [eax+04]
	push	dword ptr [eax]
	call	dword ptr [004020C4]
	add	esp,18
	pop	esi
	pop	ebp
	ret
00401090 3B 0D 04 30 40 00 F2 75 02 F2 C3 F2 E9 5F 02 00 ;..0@..u....._..
004010A0 00 56 6A 01 E8 0F 0B 00 00 E8 55 06 00 00 50 E8 .Vj.......U...P.
004010B0 3A 0B 00 00 E8 65 0B 00 00 8B F0 E8 49 06 00 00 :....e......I...
004010C0 6A 01 89 06 E8 E4 03 00 00 83 C4 0C 5E 84 C0 74 j...........^..t
004010D0 6C DB E2 E8 48 08 00 00 68 4B 19 40 00 E8 6C 05 l...H...hK.@..l.
004010E0 00 00 E8 18 06 00 00 50 E8 D7 0A 00 00 59 59 85 .......P.....YY.
004010F0 C0 75 4A E8 14 06 00 00 E8 5F 06 00 00 85 C0 74 .uJ......_.....t
00401100 0B 68 09 17 40 00 E8 B3 0A 00 00 59 E8 93 0A 00 .h..@......Y....
00401110 00 E8 8E 0A 00 00 E8 FD 05 00 00 E8 E9 05 00 00 ................
00401120 50 E8 EC 0A 00 00 59 E8 1C 0B 00 00 84 C0 74 05 P.....Y.......t.
00401130 E8 95 0A 00 00 E8 CF 05 00 00 33 C0 C3 6A 07 E8 ..........3..j..
00401140 30 06 00 00 CC E8 F5 05 00 00 33 C0 C3 E8 81 07 0.........3.....
00401150 00 00 E8 B2 05 00 00 50 E8 BB 0A 00 00 59 C3    .......P.....Y.

l0040115F:
	push	14
	push	004024D8
	call	00401980
	push	01
	call	00401474
	pop	ecx
	test	al,al
	jnz	0040117E

l00401177:
	push	07
	call	00401774

l0040117E:
	xor	bl,bl
	mov	[ebp-19],bl
	and	dword ptr [ebp-04],00
	call	0040143F
	mov	[ebp-24],al
	mov	eax,[00403334]
	xor	ecx,ecx
	inc	ecx
	cmp	eax,ecx
	jz	00401177

l0040119B:
	test	eax,eax
	jnz	004011E8

l0040119F:
	mov	[00403334],ecx
	push	004020EC
	push	004020E0
	call	00401BDC
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	004011CB

l004011BA:
	mov	dword ptr [ebp-04],FFFFFFFE
	mov	eax,000000FF
	jmp	004012C8

l004011CB:
	push	004020DC
	push	004020D4
	call	00401BD6
	pop	ecx
	pop	ecx
	mov	dword ptr [00403334],00000002
	jmp	004011ED

l004011E8:
	mov	bl,cl
	mov	[ebp-19],bl

l004011ED:
	push	dword ptr [ebp-24]
	call	004015CE
	pop	ecx
	call	00401768
	mov	esi,eax
	xor	edi,edi
	cmp	[esi],edi
	jz	0040121D

l00401203:
	push	esi
	call	00401544
	pop	ecx
	test	al,al
	jz	0040121D

l0040120E:
	push	edi
	push	02
	push	edi
	mov	esi,[esi]
	mov	ecx,esi
	call	00401976
	call	esi

l0040121D:
	call	0040176E
	mov	esi,eax
	cmp	[esi],edi
	jz	0040123B

l00401228:
	push	esi
	call	00401544
	pop	ecx
	test	al,al
	jz	0040123B

l00401233:
	push	dword ptr [esi]
	call	00401C0C
	pop	ecx

l0040123B:
	call	00401BFA
	mov	edi,eax
	call	00401BF4
	mov	esi,eax
	call	00401BD0
	push	eax
	push	dword ptr [edi]
	push	dword ptr [esi]
	call	00401000
	add	esp,0C
	mov	esi,eax
	call	0040188F
	test	al,al
	jnz	0040126C

l00401266:
	push	esi
	call	00401BE2

l0040126C:
	test	bl,bl
	jnz	00401275

l00401270:
	call	00401C00

l00401275:
	push	00
	push	01
	call	004015EB
	pop	ecx
	pop	ecx
	mov	dword ptr [ebp-04],FFFFFFFE
	mov	eax,esi
	jmp	004012C8
0040128B                                  8B 4D EC 8B 01            .M...
00401290 8B 00 89 45 E0 51 50 E8 16 09 00 00 59 59 C3 8B ...E.QP.....YY..
004012A0 65 E8 E8 E8 05 00 00 84 C0 75 08 FF 75 E0 E8 35 e........u..u..5
004012B0 09 00 00 80 7D E7 00 75 05 E8 48 09 00 00 C7 45 ....}..u..H....E
004012C0 FC FE FF FF FF 8B 45 E0                         ......E.       

l004012C8:
	call	004019C6
	ret

;; Win32CrtStartup: 004012CE
Win32CrtStartup proc
	call	00401663
	jmp	0040115F

;; fn004012D8: 004012D8
fn004012D8 proc
	push	ebp
	mov	ebp,esp
	push	00
	call	dword ptr [00402038]
	push	dword ptr [ebp+08]
	call	dword ptr [0040203C]
	push	C0000409
	call	dword ptr [00402034]
	push	eax
	call	dword ptr [00402030]
	pop	ebp
	ret
00401300 55 8B EC 81 EC 24 03 00 00 6A 17 E8 32 09 00 00 U....$...j..2...
00401310 85 C0 74 05 6A 02 59 CD 29 A3 18 31 40 00 89 0D ..t.j.Y.)..1@...
00401320 14 31 40 00 89 15 10 31 40 00 89 1D 0C 31 40 00 .1@....1@....1@.
00401330 89 35 08 31 40 00 89 3D 04 31 40 00 66 8C 15 30 .5.1@..=.1@.f..0
00401340 31 40 00 66 8C 0D 24 31 40 00 66 8C 1D 00 31 40 1@.f..$1@.f...1@
00401350 00 66 8C 05 FC 30 40 00 66 8C 25 F8 30 40 00 66 .f...0@.f.%.0@.f
00401360 8C 2D F4 30 40 00 9C 8F 05 28 31 40 00 8B 45 00 .-.0@....(1@..E.
00401370 A3 1C 31 40 00 8B 45 04 A3 20 31 40 00 8D 45 08 ..1@..E.. 1@..E.
00401380 A3 2C 31 40 00 8B 85 DC FC FF FF C7 05 68 30 40 .,1@.........h0@
00401390 00 01 00 01 00 A1 20 31 40 00 A3 24 30 40 00 C7 ...... 1@..$0@..
004013A0 05 18 30 40 00 09 04 00 C0 C7 05 1C 30 40 00 01 ..0@........0@..
004013B0 00 00 00 C7 05 28 30 40 00 01 00 00 00 6A 04 58 .....(0@.....j.X
004013C0 6B C0 00 C7 80 2C 30 40 00 02 00 00 00 6A 04 58 k....,0@.....j.X
004013D0 6B C0 00 8B 0D 04 30 40 00 89 4C 05 F8 6A 04 58 k.....0@..L..j.X
004013E0 C1 E0 00 8B 0D 00 30 40 00 89 4C 05 F8 68 00 21 ......0@..L..h.!
004013F0 40 00 E8 E1 FE FF FF 8B E5 5D C3                @........].    

;; fn004013FB: 004013FB
;;   Called from:
;;     0040158D (in fn00401544)
fn004013FB proc
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
	jz	00401436

l0040141D:
	mov	ecx,[ebp+0C]

l00401420:
	cmp	ecx,[edx+0C]
	jc	0040142F

l00401425:
	mov	eax,[edx+08]
	add	eax,[edx+0C]
	cmp	ecx,eax
	jc	0040143B

l0040142F:
	add	edx,28
	cmp	edx,esi
	jnz	00401420

l00401436:
	xor	eax,eax

l00401438:
	pop	esi
	pop	ebp
	ret

l0040143B:
	mov	eax,edx
	jmp	00401438

;; fn0040143F: 0040143F
;;   Called from:
;;     00401187 (in Win32CrtStartup)
fn0040143F proc
	call	00401B98
	test	eax,eax
	jnz	0040144B

l00401448:
	xor	al,al
	ret

l0040144B:
	mov	eax,fs:[00000018]
	push	esi
	mov	esi,00403338
	mov	edx,[eax+04]
	jmp	00401460

l0040145C:
	cmp	edx,eax
	jz	00401470

l00401460:
	xor	eax,eax
	mov	ecx,edx
	lock
	cmpxchg	[esi],ecx
	test	eax,eax
	jnz	0040145C

l0040146C:
	xor	al,al
	pop	esi
	ret

l00401470:
	mov	al,01
	pop	esi
	ret

;; fn00401474: 00401474
;;   Called from:
;;     0040116D (in Win32CrtStartup)
fn00401474 proc
	push	ebp
	mov	ebp,esp
	cmp	dword ptr [ebp+08],00
	jnz	00401484

l0040147D:
	mov	byte ptr [00403354],01

l00401484:
	call	004019FE
	call	00401C48
	test	al,al
	jnz	00401496

l00401492:
	xor	al,al
	pop	ebp
	ret

l00401496:
	call	00401C48
	test	al,al
	jnz	004014A9

l0040149F:
	push	00
	call	00401C48
	pop	ecx
	jmp	00401492

l004014A9:
	mov	al,01
	pop	ebp
	ret

;; fn004014AD: 004014AD
fn004014AD proc
	push	ebp
	mov	ebp,esp
	sub	esp,0C
	push	esi
	mov	esi,[ebp+08]
	test	esi,esi
	jz	004014C0

l004014BB:
	cmp	esi,01
	jnz	0040153C

l004014C0:
	call	00401B98
	test	eax,eax
	jz	004014F3

l004014C9:
	test	esi,esi
	jnz	004014F3

l004014CD:
	push	0040333C
	call	00401C24
	pop	ecx
	test	eax,eax
	jz	004014E0

l004014DC:
	xor	al,al
	jmp	00401537

l004014E0:
	push	00403348
	call	00401C24
	neg	eax
	pop	ecx
	sbb	al,al
	inc	al
	jmp	00401537

l004014F3:
	mov	eax,[00403004]
	lea	esi,[ebp-0C]
	push	edi
	and	eax,1F
	mov	edi,0040333C
	push	20
	pop	ecx
	sub	ecx,eax
	or	eax,FF
	ror	eax,cl
	xor	eax,[00403004]
	mov	[ebp-0C],eax
	mov	[ebp-08],eax
	mov	[ebp-04],eax
	movsd
	movsd
	movsd
	mov	edi,00403348
	mov	[ebp-0C],eax
	mov	[ebp-08],eax
	lea	esi,[ebp-0C]
	mov	[ebp-04],eax
	mov	al,01
	movsd
	movsd
	movsd
	pop	edi

l00401537:
	pop	esi
	mov	esp,ebp
	pop	ebp
	ret

l0040153C:
	push	05
	call	00401774
	int	03

;; fn00401544: 00401544
;;   Called from:
;;     00401204 (in Win32CrtStartup)
;;     00401229 (in Win32CrtStartup)
;;     00401543 (in fn004014AD)
fn00401544 proc
	push	08
	push	004024F8
	call	00401980
	and	dword ptr [ebp-04],00
	mov	eax,00005A4D
	cmp	[00400000],ax
	jnz	004015BF

l00401562:
	mov	eax,[0040003C]
	cmp	dword ptr [eax+00400000],00004550
	jnz	004015BF

l00401573:
	mov	ecx,0000010B
	cmp	[eax+00400018],cx
	jnz	004015BF

l00401581:
	mov	eax,[ebp+08]
	mov	ecx,00400000
	sub	eax,ecx
	push	eax
	push	ecx
	call	004013FB
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	004015BF

l00401598:
	cmp	dword ptr [eax+24],00
	jl	004015BF

l0040159E:
	mov	dword ptr [ebp-04],FFFFFFFE
	mov	al,01
	jmp	004015C8
004015A9                            8B 45 EC 8B 00 33 C9          .E...3.
004015B0 81 38 05 00 00 C0 0F 94 C1 8B C1 C3 8B 65 E8    .8...........e.

l004015BF:
	mov	dword ptr [ebp-04],FFFFFFFE
	xor	al,al

l004015C8:
	call	004019C6
	ret

;; fn004015CE: 004015CE
;;   Called from:
;;     004011F0 (in Win32CrtStartup)
fn004015CE proc
	push	ebp
	mov	ebp,esp
	call	00401B98
	test	eax,eax
	jz	004015E9

l004015DA:
	cmp	byte ptr [ebp+08],00
	jnz	004015E9

l004015E0:
	xor	eax,eax
	mov	ecx,00403338
	xchg	[ecx],eax

l004015E9:
	pop	ebp
	ret

;; fn004015EB: 004015EB
;;   Called from:
;;     00401279 (in Win32CrtStartup)
fn004015EB proc
	push	ebp
	mov	ebp,esp
	cmp	byte ptr [00403354],00
	jz	004015FD

l004015F7:
	cmp	byte ptr [ebp+0C],00
	jnz	0040160F

l004015FD:
	push	dword ptr [ebp+08]
	call	00401C48
	push	dword ptr [ebp+08]
	call	00401C48
	pop	ecx
	pop	ecx

l0040160F:
	mov	al,01
	pop	ebp
	ret

;; fn00401613: 00401613
;;   Called from:
;;     00401654 (in fn0040164E)
fn00401613 proc
	push	ebp
	mov	ebp,esp
	mov	eax,[00403004]
	mov	ecx,eax
	xor	eax,[0040333C]
	and	ecx,1F
	push	dword ptr [ebp+08]
	ror	eax,cl
	cmp	eax,FF
	jnz	00401637

l00401630:
	call	00401C30
	jmp	00401642

l00401637:
	push	0040333C
	call	00401C2A
	pop	ecx

l00401642:
	neg	eax
	pop	ecx
	sbb	eax,eax
	not	eax
	and	eax,[ebp+08]
	pop	ebp
	ret

;; fn0040164E: 0040164E
fn0040164E proc
	push	ebp
	mov	ebp,esp
	push	dword ptr [ebp+08]
	call	00401613
	neg	eax
	pop	ecx
	sbb	eax,eax
	neg	eax
	dec	eax
	pop	ebp
	ret

;; fn00401663: 00401663
;;   Called from:
;;     004012CE (in Win32CrtStartup)
fn00401663 proc
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
	jz	00401693

l00401686:
	test	esi,eax
	jz	00401693

l0040168A:
	not	eax
	mov	[00403000],eax
	jmp	004016F9

l00401693:
	lea	eax,[ebp-0C]
	push	eax
	call	dword ptr [0040201C]
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
	jnz	004016DB

l004016D4:
	mov	ecx,BB40E64F
	jmp	004016EB

l004016DB:
	test	esi,ecx
	jnz	004016EB

l004016DF:
	mov	eax,ecx
	or	eax,00004711
	shl	eax,10
	or	ecx,eax

l004016EB:
	mov	[00403004],ecx
	not	ecx
	mov	[00403000],ecx

l004016F9:
	pop	edi
	pop	esi
	mov	esp,ebp
	pop	ebp
	ret

;; fn004016FF: 004016FF
fn004016FF proc
	xor	eax,eax
	inc	eax
	ret

;; fn00401703: 00401703
fn00401703 proc
	mov	eax,00004000
	ret

;; fn00401709: 00401709
fn00401709 proc
	xor	eax,eax
	ret

;; fn0040170C: 0040170C
fn0040170C proc
	push	00403358
	call	dword ptr [00402018]
	ret

;; fn00401718: 00401718
fn00401718 proc
	push	00030000
	push	00010000
	push	00
	call	00401C36
	add	esp,0C
	test	eax,eax
	jnz	00401731

l00401730:
	ret

l00401731:
	push	07
	call	00401774
	int	03

;; fn00401739: 00401739
;;   Called from:
;;     00401738 (in fn00401718)
;;     0040174D (in fn0040173F)
fn00401739 proc
	mov	eax,00403360
	ret

;; fn0040173F: 0040173F
fn0040173F proc
	call	00401050
	mov	ecx,[eax+04]
	or	dword ptr [eax],04
	mov	[eax+04],ecx
	call	00401739
	mov	ecx,[eax+04]
	or	dword ptr [eax],02
	mov	[eax+04],ecx
	ret

;; fn0040175C: 0040175C
fn0040175C proc
	xor	eax,eax
	cmp	[0040300C],eax
	setz	al
	ret

;; fn00401768: 00401768
;;   Called from:
;;     004011F6 (in Win32CrtStartup)
fn00401768 proc
	mov	eax,00403388
	ret

;; fn0040176E: 0040176E
;;   Called from:
;;     0040121D (in Win32CrtStartup)
fn0040176E proc
	mov	eax,00403384
	ret

;; fn00401774: 00401774
;;   Called from:
;;     00401179 (in Win32CrtStartup)
;;     0040153E (in fn004014AD)
;;     00401733 (in fn00401718)
fn00401774 proc
	push	ebp
	mov	ebp,esp
	sub	esp,00000324
	push	ebx
	push	esi
	push	17
	call	00401C42
	test	eax,eax
	jz	0040178F

l0040178A:
	mov	ecx,[ebp+08]
	int	29

l0040178F:
	xor	esi,esi
	lea	eax,[ebp-00000324]
	push	000002CC
	push	esi
	push	eax
	mov	[00403368],esi
	call	00401BA6
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
	call	00401BA6
	mov	eax,[ebp+04]
	add	esp,0C
	mov	dword ptr [ebp-58],40000015
	mov	dword ptr [ebp-54],00000001
	mov	[ebp-4C],eax
	call	dword ptr [00402014]
	push	esi
	lea	ebx,[eax-01]
	neg	ebx
	lea	eax,[ebp-58]
	mov	[ebp-08],eax
	lea	eax,[ebp-00000324]
	sbb	bl,bl
	mov	[ebp-04],eax
	inc	bl
	call	dword ptr [00402038]
	lea	eax,[ebp-08]
	push	eax
	call	dword ptr [0040203C]
	test	eax,eax
	jnz	00401889

l0040187C:
	movzx	eax,bl
	neg	eax
	sbb	eax,eax
	and	[00403368],eax

l00401889:
	pop	esi
	pop	ebx
	mov	esp,ebp
	pop	ebp
	ret

;; fn0040188F: 0040188F
;;   Called from:
;;     0040125D (in Win32CrtStartup)
fn0040188F proc
	push	00
	call	dword ptr [00402040]
	mov	ecx,eax
	test	ecx,ecx
	jnz	004018A0

l0040189D:
	xor	al,al
	ret

l004018A0:
	mov	eax,00005A4D
	cmp	[ecx],ax
	jnz	0040189D

l004018AA:
	mov	eax,[ecx+3C]
	add	eax,ecx
	cmp	dword ptr [eax],00004550
	jnz	0040189D

l004018B7:
	mov	ecx,0000010B
	cmp	[eax+18],cx
	jnz	0040189D

l004018C2:
	cmp	dword ptr [eax+74],0E
	jbe	0040189D

l004018C8:
	cmp	dword ptr [eax+000000E8],00
	setnz	al
	ret

;; fn004018D3: 004018D3
fn004018D3 proc
	push	004018DF
	call	dword ptr [00402038]
	ret
004018DF                                              55                U
004018E0 8B EC 8B 45 08 8B 00 81 38 63 73 6D E0 75 25 83 ...E....8csm.u%.
004018F0 78 10 03 75 1F 8B 40 14 3D 20 05 93 19 74 1B 3D x..u..@.= ...t.=
00401900 21 05 93 19 74 14 3D 22 05 93 19 74 0D 3D 00 40 !...t.="...t.=.@
00401910 99 01 74 06 33 C0 5D C2 04 00 E8 1D 03 00 00 CC ..t.3.].........

;; fn00401920: 00401920
fn00401920 proc
	push	ebx
	push	esi
	mov	esi,004024C8
	mov	ebx,004024C8
	cmp	esi,ebx
	jnc	00401948

l00401930:
	push	edi

l00401931:
	mov	edi,[esi]
	test	edi,edi
	jz	00401940

l00401937:
	mov	ecx,edi
	call	00401976
	call	edi

l00401940:
	add	esi,04
	cmp	esi,ebx
	jc	00401931

l00401947:
	pop	edi

l00401948:
	pop	esi
	pop	ebx
	ret
0040194B                                  53 56 BE D0 24            SV..$
00401950 40 00 BB D0 24 40 00 3B F3 73 18 57 8B 3E 85 FF @...$@.;.s.W.>..
00401960 74 09 8B CF E8 0D 00 00 00 FF D7 83 C6 04 3B F3 t.............;.
00401970 72 EA 5F 5E 5B C3                               r._^[.         

;; fn00401976: 00401976
;;   Called from:
;;     00401216 (in Win32CrtStartup)
;;     00401939 (in fn00401920)
fn00401976 proc
	jmp	dword ptr [004020D0]
0040197C                                     CC CC CC CC             ....

;; fn00401980: 00401980
;;   Called from:
;;     00401166 (in Win32CrtStartup)
;;     0040154B (in fn00401544)
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
;;     004012C8 (in Win32CrtStartup)
;;     004015C8 (in fn00401544)
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
004019E0 14 FF 75 10 FF 75 0C FF 75 08 68 90 10 40 00 68 ..u..u..u.h..@.h
004019F0 04 30 40 00 E8 B3 01 00 00 83 C4 18 5D C3       .0@.........]. 

;; fn004019FE: 004019FE
;;   Called from:
;;     00401484 (in fn00401474)
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
	call	00401C42
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
;;     0040143F (in fn0040143F)
;;     004014C0 (in fn004014AD)
;;     004015D1 (in fn004015CE)
fn00401B98 proc
	xor	eax,eax
	cmp	[00403014],eax
	setnz	al
	ret

;; fn00401BA4: 00401BA4
fn00401BA4 proc
	ret
00401BA5                CC FF 25 48 20 40 00 FF 25 4C 20      ..%H @..%L 
00401BB0 40 00 FF 25 AC 20 40 00 FF 25 A8 20 40 00 FF 25 @..%. @..%. @..%
00401BC0 64 20 40 00 FF 25 A0 20 40 00 FF 25 9C 20 40 00 d @..%. @..%. @.
00401BD0 FF 25 98 20 40 00 FF 25 7C 20 40 00 FF 25 B4 20 .%. @..%| @..%. 
00401BE0 40 00 FF 25 94 20 40 00 FF 25 90 20 40 00 FF 25 @..%. @..%. @..%
00401BF0 C0 20 40 00 FF 25 8C 20 40 00 FF 25 80 20 40 00 . @..%. @..%. @.
00401C00 FF 25 70 20 40 00 FF 25 6C 20 40 00 FF 25 A4 20 .%p @..%l @..%. 
00401C10 40 00 FF 25 5C 20 40 00 FF 25 54 20 40 00 FF 25 @..%\ @..%T @..%
00401C20 BC 20 40 00 FF 25 74 20 40 00 FF 25 78 20 40 00 . @..%t @..%x @.
00401C30 FF 25 B0 20 40 00 FF 25 84 20 40 00 FF 25 88 20 .%. @..%. @..%. 
00401C40 40 00                                           @.             
00401C42       FF 25 2C 20 40 00                           .%, @.       

;; fn00401C48: 00401C48
;;   Called from:
;;     00401489 (in fn00401474)
;;     00401496 (in fn00401474)
;;     004014A1 (in fn00401474)
;;     00401600 (in fn004015EB)
;;     00401608 (in fn004015EB)
fn00401C48 proc
	mov	al,01
	ret
;;; Segment .rdata (00402000)
__imp__?slow_and_safe_increment@@YAHH@Z		; 00402000
	dd	0x00002712
__imp__?exported_critical_section@@3U_RTL_CRITICAL_SECTION@@A		; 00402004
	dd	0x000026C2
__imp__?exported_int@@3HA		; 00402008
	dd	0x000026FC
0040200C                                     00 00 00 00             ....
__imp__InitializeCriticalSection		; 00402010
	dd	0x00002698
__imp__IsDebuggerPresent		; 00402014
	dd	0x00002B06
__imp__InitializeSListHead		; 00402018
	dd	0x00002AF0
__imp__GetSystemTimeAsFileTime		; 0040201C
	dd	0x00002AD6
__imp__GetCurrentThreadId		; 00402020
	dd	0x00002AC0
__imp__GetCurrentProcessId		; 00402024
	dd	0x00002AAA
__imp__QueryPerformanceCounter		; 00402028
	dd	0x00002A90
__imp__IsProcessorFeaturePresent		; 0040202C
	dd	0x00002A74
__imp__TerminateProcess		; 00402030
	dd	0x00002A60
__imp__GetCurrentProcess		; 00402034
	dd	0x00002A4C
__imp__SetUnhandledExceptionFilter		; 00402038
	dd	0x00002A2E
__imp__UnhandledExceptionFilter		; 0040203C
	dd	0x00002A12
__imp__GetModuleHandleW		; 00402040
	dd	0x00002B1A
00402044             00 00 00 00                             ....       
__imp__memset		; 00402048
	dd	0x0000274A
__imp___except_handler4_common		; 0040204C
	dd	0x00002754
00402050 00 00 00 00                                     ....           
__imp___set_new_mode		; 00402054
	dd	0x000028EC
00402058                         00 00 00 00                     ....   
__imp___configthreadlocale		; 0040205C
	dd	0x000028D6
00402060 00 00 00 00                                     ....           
__imp____setusermatherr		; 00402064
	dd	0x000027CE
00402068                         00 00 00 00                     ....   
__imp___c_exit		; 0040206C
	dd	0x0000289E
__imp___cexit		; 00402070
	dd	0x00002894
__imp___initialize_onexit_table		; 00402074
	dd	0x0000290C
__imp___register_onexit_function		; 00402078
	dd	0x00002928
__imp___initterm		; 0040207C
	dd	0x00002840
__imp____p___argv		; 00402080
	dd	0x00002886
__imp___controlfp_s		; 00402084
	dd	0x00002952
__imp__terminate		; 00402088
	dd	0x00002962
__imp____p___argc		; 0040208C
	dd	0x00002878
__imp___exit		; 00402090
	dd	0x00002862
__imp__exit		; 00402094
	dd	0x0000285A
__imp___get_initial_narrow_environment		; 00402098
	dd	0x0000281E
__imp___initialize_narrow_environment		; 0040209C
	dd	0x000027FC
__imp___configure_narrow_argv		; 004020A0
	dd	0x000027E2
__imp___register_thread_local_exe_atexit_callback		; 004020A4
	dd	0x000028A8
__imp___set_app_type		; 004020A8
	dd	0x000027BE
__imp___seh_filter_exe		; 004020AC
	dd	0x000027AC
__imp___crt_atexit		; 004020B0
	dd	0x00002944
__imp___initterm_e		; 004020B4
	dd	0x0000284C
004020B8                         00 00 00 00                     ....   
__imp____p__commode		; 004020BC
	dd	0x000028FC
__imp___set_fmode		; 004020C0
	dd	0x0000286A
__imp____stdio_common_vfprintf		; 004020C4
	dd	0x00002792
__imp____acrt_iob_func		; 004020C8
	dd	0x00002780
004020CC                                     00 00 00 00             ....
004020D0 A4 1B 40 00 00 00 00 00 4D 11 40 00 00 00 00 00 ..@.....M.@.....
004020E0 00 00 00 00 A1 10 40 00 45 11 40 00 00 00 00 00 ......@.E.@.....
004020F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00402100 18 30 40 00 68 30 40 00 25 64 0A 00 00 00 00 00 .0@.h0@.%d......
00402110 00 00 00 00 7B 9D 19 58 00 00 00 00 02 00 00 00 ....{..X........
00402120 63 00 00 00 E4 21 00 00 E4 13 00 00 00 00 00 00 c....!..........
00402130 7B 9D 19 58 00 00 00 00 0C 00 00 00 14 00 00 00 {..X............
00402140 48 22 00 00 48 14 00 00 00 00 00 00 7B 9D 19 58 H"..H.......{..X
00402150 00 00 00 00 0D 00 00 00 68 02 00 00 5C 22 00 00 ........h...\"..
00402160 5C 14 00 00 00 00 00 00 7B 9D 19 58 00 00 00 00 \.......{..X....
00402170 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00402180 5C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 \...............
00402190 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
004021B0 00 00 00 00 00 00 00 00 00 00 00 00 04 30 40 00 .............0@.
004021C0 E0 21 40 00 01 00 00 00 D0 20 40 00 00 00 00 00 .!@...... @.....
004021D0 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 ................
004021E0 DB 19 00 00 52 53 44 53 70 9A E2 EA 0A 9E 1B 45 ....RSDSp......E
004021F0 88 F8 F1 43 38 CA A9 69 03 00 00 00 44 3A 5C 64 ...C8..i....D:\d
00402200 65 76 5C 75 78 6D 61 6C 5C 72 65 6B 6F 5C 6D 61 ev\uxmal\reko\ma
00402210 73 74 65 72 5C 73 75 62 6A 65 63 74 73 5C 50 45 ster\subjects\PE
00402220 2D 78 38 36 5C 45 78 65 44 6C 6C 5C 73 72 63 5C -x86\ExeDll\src\
00402230 52 65 6C 65 61 73 65 5C 45 78 65 63 75 74 61 62 Release\Executab
00402240 6C 65 2E 70 64 62 00 00 00 00 00 00 21 00 00 00 le.pdb......!...
00402250 21 00 00 00 02 00 00 00 1F 00 00 00 47 43 54 4C !...........GCTL
00402260 00 10 00 00 4B 0C 00 00 2E 74 65 78 74 24 6D 6E ....K....text$mn
00402270 00 00 00 00 00 20 00 00 D0 00 00 00 2E 69 64 61 ..... .......ida
00402280 74 61 24 35 00 00 00 00 D0 20 00 00 04 00 00 00 ta$5..... ......
00402290 2E 30 30 63 66 67 00 00 D4 20 00 00 04 00 00 00 .00cfg... ......
004022A0 2E 43 52 54 24 58 43 41 00 00 00 00 D8 20 00 00 .CRT$XCA..... ..
004022B0 04 00 00 00 2E 43 52 54 24 58 43 41 41 00 00 00 .....CRT$XCAA...
004022C0 DC 20 00 00 04 00 00 00 2E 43 52 54 24 58 43 5A . .......CRT$XCZ
004022D0 00 00 00 00 E0 20 00 00 04 00 00 00 2E 43 52 54 ..... .......CRT
004022E0 24 58 49 41 00 00 00 00 E4 20 00 00 04 00 00 00 $XIA..... ......
004022F0 2E 43 52 54 24 58 49 41 41 00 00 00 E8 20 00 00 .CRT$XIAA.... ..
00402300 04 00 00 00 2E 43 52 54 24 58 49 41 43 00 00 00 .....CRT$XIAC...
00402310 EC 20 00 00 04 00 00 00 2E 43 52 54 24 58 49 5A . .......CRT$XIZ
00402320 00 00 00 00 F0 20 00 00 04 00 00 00 2E 43 52 54 ..... .......CRT
00402330 24 58 50 41 00 00 00 00 F4 20 00 00 04 00 00 00 $XPA..... ......
00402340 2E 43 52 54 24 58 50 5A 00 00 00 00 F8 20 00 00 .CRT$XPZ..... ..
00402350 04 00 00 00 2E 43 52 54 24 58 54 41 00 00 00 00 .....CRT$XTA....
00402360 FC 20 00 00 04 00 00 00 2E 43 52 54 24 58 54 5A . .......CRT$XTZ
00402370 00 00 00 00 00 21 00 00 E0 00 00 00 2E 72 64 61 .....!.......rda
00402380 74 61 00 00 E0 21 00 00 04 00 00 00 2E 72 64 61 ta...!.......rda
00402390 74 61 24 73 78 64 61 74 61 00 00 00 E4 21 00 00 ta$sxdata....!..
004023A0 E0 02 00 00 2E 72 64 61 74 61 24 7A 7A 7A 64 62 .....rdata$zzzdb
004023B0 67 00 00 00 C4 24 00 00 04 00 00 00 2E 72 74 63 g....$.......rtc
004023C0 24 49 41 41 00 00 00 00 C8 24 00 00 04 00 00 00 $IAA.....$......
004023D0 2E 72 74 63 24 49 5A 5A 00 00 00 00 CC 24 00 00 .rtc$IZZ.....$..
004023E0 04 00 00 00 2E 72 74 63 24 54 41 41 00 00 00 00 .....rtc$TAA....
004023F0 D0 24 00 00 08 00 00 00 2E 72 74 63 24 54 5A 5A .$.......rtc$TZZ
00402400 00 00 00 00 D8 24 00 00 3C 00 00 00 2E 78 64 61 .....$..<....xda
00402410 74 61 24 78 00 00 00 00 14 25 00 00 A0 00 00 00 ta$x.....%......
00402420 2E 69 64 61 74 61 24 32 00 00 00 00 B4 25 00 00 .idata$2.....%..
00402430 14 00 00 00 2E 69 64 61 74 61 24 33 00 00 00 00 .....idata$3....
00402440 C8 25 00 00 D0 00 00 00 2E 69 64 61 74 61 24 34 .%.......idata$4
00402450 00 00 00 00 98 26 00 00 96 04 00 00 2E 69 64 61 .....&.......ida
00402460 74 61 24 36 00 00 00 00 00 30 00 00 18 00 00 00 ta$6.....0......
00402470 2E 64 61 74 61 00 00 00 18 30 00 00 74 03 00 00 .data....0..t...
00402480 2E 62 73 73 00 00 00 00 00 40 00 00 20 00 00 00 .bss.....@.. ...
00402490 2E 67 66 69 64 73 24 79 00 00 00 00 00 50 00 00 .gfids$y.....P..
004024A0 60 00 00 00 2E 72 73 72 63 24 30 31 00 00 00 00 `....rsrc$01....
004024B0 60 50 00 00 80 01 00 00 2E 72 73 72 63 24 30 32 `P.......rsrc$02
004024C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
004024D0 00 00 00 00 00 00 00 00 FE FF FF FF 00 00 00 00 ................
004024E0 CC FF FF FF 00 00 00 00 FE FF FF FF 8B 12 40 00 ..............@.
004024F0 9F 12 40 00 00 00 00 00 FE FF FF FF 00 00 00 00 ..@.............
00402500 D8 FF FF FF 00 00 00 00 FE FF FF FF A9 15 40 00 ..............@.
00402510 BC 15 40 00 D8 25 00 00 00 00 00 00 00 00 00 00 ..@..%..........
00402520 B4 26 00 00 10 20 00 00 C8 25 00 00 00 00 00 00 .&... ...%......
00402530 00 00 00 00 36 27 00 00 00 20 00 00 10 26 00 00 ....6'... ...&..
00402540 00 00 00 00 00 00 00 00 6E 27 00 00 48 20 00 00 ........n'..H ..
00402550 84 26 00 00 00 00 00 00 00 00 00 00 6E 29 00 00 .&..........n)..
00402560 BC 20 00 00 34 26 00 00 00 00 00 00 00 00 00 00 . ..4&..........
00402570 8E 29 00 00 6C 20 00 00 2C 26 00 00 00 00 00 00 .)..l ..,&......
00402580 00 00 00 00 B0 29 00 00 64 20 00 00 24 26 00 00 .....)..d ..$&..
00402590 00 00 00 00 00 00 00 00 D0 29 00 00 5C 20 00 00 .........)..\ ..
004025A0 1C 26 00 00 00 00 00 00 00 00 00 00 F2 29 00 00 .&...........)..
004025B0 54 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 T ..............
004025C0 00 00 00 00 00 00 00 00                         ........       
l004025C8	dd	0x00002712
l004025CC	dd	0x000026C2
l004025D0	dd	0x000026FC
004025D4             00 00 00 00                             ....       
l004025D8	dd	0x00002698
l004025DC	dd	0x00002B06
l004025E0	dd	0x00002AF0
l004025E4	dd	0x00002AD6
l004025E8	dd	0x00002AC0
l004025EC	dd	0x00002AAA
l004025F0	dd	0x00002A90
l004025F4	dd	0x00002A74
l004025F8	dd	0x00002A60
l004025FC	dd	0x00002A4C
l00402600	dd	0x00002A2E
l00402604	dd	0x00002A12
l00402608	dd	0x00002B1A
0040260C                                     00 00 00 00             ....
l00402610	dd	0x0000274A
l00402614	dd	0x00002754
00402618                         00 00 00 00                     ....   
l0040261C	dd	0x000028EC
00402620 00 00 00 00                                     ....           
l00402624	dd	0x000028D6
00402628                         00 00 00 00                     ....   
l0040262C	dd	0x000027CE
00402630 00 00 00 00                                     ....           
l00402634	dd	0x0000289E
l00402638	dd	0x00002894
l0040263C	dd	0x0000290C
l00402640	dd	0x00002928
l00402644	dd	0x00002840
l00402648	dd	0x00002886
l0040264C	dd	0x00002952
l00402650	dd	0x00002962
l00402654	dd	0x00002878
l00402658	dd	0x00002862
l0040265C	dd	0x0000285A
l00402660	dd	0x0000281E
l00402664	dd	0x000027FC
l00402668	dd	0x000027E2
l0040266C	dd	0x000028A8
l00402670	dd	0x000027BE
l00402674	dd	0x000027AC
l00402678	dd	0x00002944
l0040267C	dd	0x0000284C
00402680 00 00 00 00                                     ....           
l00402684	dd	0x000028FC
l00402688	dd	0x0000286A
l0040268C	dd	0x00002792
l00402690	dd	0x00002780
00402694             00 00 00 00 47 03 49 6E 69 74 69 61     ....G.Initia
004026A0 6C 69 7A 65 43 72 69 74 69 63 61 6C 53 65 63 74 lizeCriticalSect
004026B0 69 6F 6E 00 4B 45 52 4E 45 4C 33 32 2E 64 6C 6C ion.KERNEL32.dll
004026C0 00 00 00 00 3F 65 78 70 6F 72 74 65 64 5F 63 72 ....?exported_cr
004026D0 69 74 69 63 61 6C 5F 73 65 63 74 69 6F 6E 40 40 itical_section@@
004026E0 33 55 5F 52 54 4C 5F 43 52 49 54 49 43 41 4C 5F 3U_RTL_CRITICAL_
004026F0 53 45 43 54 49 4F 4E 40 40 41 00 00 01 00 3F 65 SECTION@@A....?e
00402700 78 70 6F 72 74 65 64 5F 69 6E 74 40 40 33 48 41 xported_int@@3HA
00402710 00 00 02 00 3F 73 6C 6F 77 5F 61 6E 64 5F 73 61 ....?slow_and_sa
00402720 66 65 5F 69 6E 63 72 65 6D 65 6E 74 40 40 59 41 fe_increment@@YA
00402730 48 48 40 5A 00 00 44 79 6E 61 6D 69 63 4C 69 62 HH@Z..DynamicLib
00402740 72 61 72 79 2E 64 6C 6C 00 00 48 00 6D 65 6D 73 rary.dll..H.mems
00402750 65 74 00 00 35 00 5F 65 78 63 65 70 74 5F 68 61 et..5._except_ha
00402760 6E 64 6C 65 72 34 5F 63 6F 6D 6D 6F 6E 00 56 43 ndler4_common.VC
00402770 52 55 4E 54 49 4D 45 31 34 30 2E 64 6C 6C 00 00 RUNTIME140.dll..
00402780 00 00 5F 5F 61 63 72 74 5F 69 6F 62 5F 66 75 6E ..__acrt_iob_fun
00402790 63 00 03 00 5F 5F 73 74 64 69 6F 5F 63 6F 6D 6D c...__stdio_comm
004027A0 6F 6E 5F 76 66 70 72 69 6E 74 66 00 42 00 5F 73 on_vfprintf.B._s
004027B0 65 68 5F 66 69 6C 74 65 72 5F 65 78 65 00 44 00 eh_filter_exe.D.
004027C0 5F 73 65 74 5F 61 70 70 5F 74 79 70 65 00 2E 00 _set_app_type...
004027D0 5F 5F 73 65 74 75 73 65 72 6D 61 74 68 65 72 72 __setusermatherr
004027E0 00 00 19 00 5F 63 6F 6E 66 69 67 75 72 65 5F 6E ...._configure_n
004027F0 61 72 72 6F 77 5F 61 72 67 76 00 00 35 00 5F 69 arrow_argv..5._i
00402800 6E 69 74 69 61 6C 69 7A 65 5F 6E 61 72 72 6F 77 nitialize_narrow
00402810 5F 65 6E 76 69 72 6F 6E 6D 65 6E 74 00 00 2A 00 _environment..*.
00402820 5F 67 65 74 5F 69 6E 69 74 69 61 6C 5F 6E 61 72 _get_initial_nar
00402830 72 6F 77 5F 65 6E 76 69 72 6F 6E 6D 65 6E 74 00 row_environment.
00402840 38 00 5F 69 6E 69 74 74 65 72 6D 00 39 00 5F 69 8._initterm.9._i
00402850 6E 69 74 74 65 72 6D 5F 65 00 58 00 65 78 69 74 nitterm_e.X.exit
00402860 00 00 25 00 5F 65 78 69 74 00 54 00 5F 73 65 74 ..%._exit.T._set
00402870 5F 66 6D 6F 64 65 00 00 05 00 5F 5F 70 5F 5F 5F _fmode....__p___
00402880 61 72 67 63 00 00 06 00 5F 5F 70 5F 5F 5F 61 72 argc....__p___ar
00402890 67 76 00 00 17 00 5F 63 65 78 69 74 00 00 16 00 gv...._cexit....
004028A0 5F 63 5F 65 78 69 74 00 3F 00 5F 72 65 67 69 73 _c_exit.?._regis
004028B0 74 65 72 5F 74 68 72 65 61 64 5F 6C 6F 63 61 6C ter_thread_local
004028C0 5F 65 78 65 5F 61 74 65 78 69 74 5F 63 61 6C 6C _exe_atexit_call
004028D0 62 61 63 6B 00 00 08 00 5F 63 6F 6E 66 69 67 74 back...._configt
004028E0 68 72 65 61 64 6C 6F 63 61 6C 65 00 16 00 5F 73 hreadlocale..._s
004028F0 65 74 5F 6E 65 77 5F 6D 6F 64 65 00 01 00 5F 5F et_new_mode...__
00402900 70 5F 5F 63 6F 6D 6D 6F 64 65 00 00 36 00 5F 69 p__commode..6._i
00402910 6E 69 74 69 61 6C 69 7A 65 5F 6F 6E 65 78 69 74 nitialize_onexit
00402920 5F 74 61 62 6C 65 00 00 3E 00 5F 72 65 67 69 73 _table..>._regis
00402930 74 65 72 5F 6F 6E 65 78 69 74 5F 66 75 6E 63 74 ter_onexit_funct
00402940 69 6F 6E 00 1F 00 5F 63 72 74 5F 61 74 65 78 69 ion..._crt_atexi
00402950 74 00 1D 00 5F 63 6F 6E 74 72 6F 6C 66 70 5F 73 t..._controlfp_s
00402960 00 00 6A 00 74 65 72 6D 69 6E 61 74 65 00 61 70 ..j.terminate.ap
00402970 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 2D 73 74 64 i-ms-win-crt-std
00402980 69 6F 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 61 70 io-l1-1-0.dll.ap
00402990 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 2D 72 75 6E i-ms-win-crt-run
004029A0 74 69 6D 65 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 time-l1-1-0.dll.
004029B0 61 70 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 2D 6D api-ms-win-crt-m
004029C0 61 74 68 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 00 ath-l1-1-0.dll..
004029D0 61 70 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 2D 6C api-ms-win-crt-l
004029E0 6F 63 61 6C 65 2D 6C 31 2D 31 2D 30 2E 64 6C 6C ocale-l1-1-0.dll
004029F0 00 00 61 70 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 ..api-ms-win-crt
00402A00 2D 68 65 61 70 2D 6C 31 2D 31 2D 30 2E 64 6C 6C -heap-l1-1-0.dll
00402A10 00 00 82 05 55 6E 68 61 6E 64 6C 65 64 45 78 63 ....UnhandledExc
00402A20 65 70 74 69 6F 6E 46 69 6C 74 65 72 00 00 43 05 eptionFilter..C.
00402A30 53 65 74 55 6E 68 61 6E 64 6C 65 64 45 78 63 65 SetUnhandledExce
00402A40 70 74 69 6F 6E 46 69 6C 74 65 72 00 09 02 47 65 ptionFilter...Ge
00402A50 74 43 75 72 72 65 6E 74 50 72 6F 63 65 73 73 00 tCurrentProcess.
00402A60 61 05 54 65 72 6D 69 6E 61 74 65 50 72 6F 63 65 a.TerminateProce
00402A70 73 73 00 00 6D 03 49 73 50 72 6F 63 65 73 73 6F ss..m.IsProcesso
00402A80 72 46 65 61 74 75 72 65 50 72 65 73 65 6E 74 00 rFeaturePresent.
00402A90 2D 04 51 75 65 72 79 50 65 72 66 6F 72 6D 61 6E -.QueryPerforman
00402AA0 63 65 43 6F 75 6E 74 65 72 00 0A 02 47 65 74 43 ceCounter...GetC
00402AB0 75 72 72 65 6E 74 50 72 6F 63 65 73 73 49 64 00 urrentProcessId.
00402AC0 0E 02 47 65 74 43 75 72 72 65 6E 74 54 68 72 65 ..GetCurrentThre
00402AD0 61 64 49 64 00 00 D6 02 47 65 74 53 79 73 74 65 adId....GetSyste
00402AE0 6D 54 69 6D 65 41 73 46 69 6C 65 54 69 6D 65 00 mTimeAsFileTime.
00402AF0 4B 03 49 6E 69 74 69 61 6C 69 7A 65 53 4C 69 73 K.InitializeSLis
00402B00 74 48 65 61 64 00 67 03 49 73 44 65 62 75 67 67 tHead.g.IsDebugg
00402B10 65 72 50 72 65 73 65 6E 74 00 67 02 47 65 74 4D erPresent.g.GetM
00402B20 6F 64 75 6C 65 48 61 6E 64 6C 65 57 00 00       oduleHandleW.. 
;;; Segment .data (00403000)
00403000 B1 19 BF 44 4E E6 40 BB FF FF FF FF 01 00 00 00 ...DN.@.........
00403010 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 ................
00403020 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00403380 00 00 00 00 00 00 00 00 00 00 00 00             ............   
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
00406000 00 10 00 00 2C 01 00 00 02 30 08 30 0D 30 14 30 ....,....0.0.0.0
00406010 21 30 27 30 31 30 38 30 51 30 6B 30 86 30 92 30 !0'01080Q0k0.0.0
00406020 D9 30 02 31 62 31 90 31 A1 31 A6 31 AB 31 CC 31 .0.1b1.1.1.1.1.1
00406030 D1 31 DE 31 DF 32 E8 32 F3 32 FA 32 1A 33 20 33 .1.1.2.2.2.2.3 3
00406040 26 33 2C 33 32 33 38 33 3F 33 46 33 4D 33 54 33 &3,32383?3F3M3T3
00406050 5B 33 62 33 69 33 71 33 79 33 81 33 8D 33 96 33 [3b3i3q3y3.3.3.3
00406060 9B 33 A1 33 AB 33 B5 33 C5 33 D5 33 E5 33 EE 33 .3.3.3.3.3.3.3.3
00406070 53 34 7F 34 CE 34 E1 34 F4 34 00 35 10 35 21 35 S4.4.4.4.4.5.5!5
00406080 47 35 5C 35 63 35 69 35 7B 35 85 35 E3 35 F0 35 G5\5c5i5{5.5.5.5
00406090 17 36 1F 36 38 36 72 36 8D 36 99 36 A8 36 B1 36 .6.686r6.6.6.6.6
004060A0 BE 36 ED 36 F5 36 0D 37 13 37 3A 37 60 37 69 37 .6.6.6.7.7:7`7i7
004060B0 6F 37 A0 37 4B 38 6A 38 74 38 85 38 93 38 D4 38 o7.7K8j8t8.8.8.8
004060C0 DA 38 23 39 28 39 4E 39 53 39 78 39 81 39 9E 39 .8#9(9N9S9x9.9.9
004060D0 EB 39 F0 39 03 3A 11 3A 2C 3A 37 3A BF 3A C8 3A .9.9.:.:,:7:.:.:
004060E0 D0 3A 17 3B 26 3B 2D 3B 63 3B 6C 3B 79 3B 84 3B .:.;&;-;c;l;y;.;
004060F0 8D 3B 9C 3B A8 3B AE 3B B4 3B BA 3B C0 3B C6 3B .;.;.;.;.;.;.;.;
00406100 CC 3B D2 3B D8 3B DE 3B E4 3B EA 3B F0 3B F6 3B .;.;.;.;.;.;.;.;
00406110 FC 3B 02 3C 08 3C 0E 3C 14 3C 1A 3C 20 3C 26 3C .;.<.<.<.<.< <&<
00406120 2C 3C 32 3C 38 3C 3E 3C 44 3C 00 00 00 20 00 00 ,<2<8<><D<... ..
00406130 24 00 00 00 D0 30 D8 30 E4 30 E8 30 00 31 04 31 $....0.0.0.0.1.1
00406140 BC 31 C0 31 C8 31 EC 34 F0 34 0C 35 10 35 00 00 .1.1.1.4.4.5.5..
