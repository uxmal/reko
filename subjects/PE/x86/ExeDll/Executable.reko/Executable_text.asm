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
