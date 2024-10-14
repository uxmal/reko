;;; Segment .text (00401000)

;; fn00401000: 00401000
;;   Called from:
;;     00401253 (in Win32CrtStartup)
fn00401000 proc
	push	dword ptr [__imp__?exported_critical_section@@3U_RTL_CRITICAL_SECTION@@A]
	call	dword ptr [__imp__InitializeCriticalSection]
	mov	eax,[__imp__?exported_int@@3HA]
	push	dword ptr [eax]
	push	402108h
	call	fn00401060
	push	1h
	call	dword ptr [__imp__?slow_and_safe_increment@@YAHH@Z]
	push	eax
	push	402108h
	call	fn00401060
	mov	eax,[__imp__?exported_int@@3HA]
	push	dword ptr [eax]
	push	402108h
	call	fn00401060
	add	esp,1Ch
	xor	eax,eax
	ret
00401047                      CC CC CC CC CC CC CC CC CC        .........

;; fn00401050: 00401050
;;   Called from:
;;     0040107A (in fn00401060)
;;     0040173F (in fn0040173F)
fn00401050 proc
	mov	eax,403378h
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
	mov	esi,[ebp+8h]
	push	1h
	call	dword ptr [__imp____acrt_iob_func]
	add	esp,4h
	lea	ecx,[ebp+0Ch]
	push	ecx
	push	0h
	push	esi
	push	eax
	call	fn00401050
	push	dword ptr [eax+4h]
	push	dword ptr [eax]
	call	dword ptr [__imp____stdio_common_vfprintf]
	add	esp,18h
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
	push	14h
	push	4024D8h
	call	fn00401980
	push	1h
	call	fn00401474
	pop	ecx
	test	al,al
	jnz	40117Eh

l00401177:
	push	7h
	call	fn00401774

l0040117E:
	xor	bl,bl
	mov	[ebp-19h],bl
	and	dword ptr [ebp-4h],0h
	call	fn0040143F
	mov	[ebp-24h],al
	mov	eax,[403334h]
	xor	ecx,ecx
	inc	ecx
	cmp	eax,ecx
	jz	401177h

l0040119B:
	test	eax,eax
	jnz	4011E8h

l0040119F:
	mov	[403334h],ecx
	push	4020ECh
	push	4020E0h
	call	401BDCh
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	4011CBh

l004011BA:
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	mov	eax,0FFh
	jmp	4012C8h

l004011CB:
	push	4020DCh
	push	4020D4h
	call	401BD6h
	pop	ecx
	pop	ecx
	mov	dword ptr [403334h],2h
	jmp	4011EDh

l004011E8:
	mov	bl,cl
	mov	[ebp-19h],bl

l004011ED:
	push	dword ptr [ebp-24h]
	call	fn004015CE
	pop	ecx
	call	fn00401768
	mov	esi,eax
	xor	edi,edi
	cmp	[esi],edi
	jz	40121Dh

l00401203:
	push	esi
	call	fn00401544
	pop	ecx
	test	al,al
	jz	40121Dh

l0040120E:
	push	edi
	push	2h
	push	edi
	mov	esi,[esi]
	mov	ecx,esi
	call	fn00401976
	call	esi

l0040121D:
	call	fn0040176E
	mov	esi,eax
	cmp	[esi],edi
	jz	40123Bh

l00401228:
	push	esi
	call	fn00401544
	pop	ecx
	test	al,al
	jz	40123Bh

l00401233:
	push	dword ptr [esi]
	call	401C0Ch
	pop	ecx

l0040123B:
	call	401BFAh
	mov	edi,eax
	call	401BF4h
	mov	esi,eax
	call	401BD0h
	push	eax
	push	dword ptr [edi]
	push	dword ptr [esi]
	call	fn00401000
	add	esp,0Ch
	mov	esi,eax
	call	fn0040188F
	test	al,al
	jnz	40126Ch

l00401266:
	push	esi
	call	401BE2h

l0040126C:
	test	bl,bl
	jnz	401275h

l00401270:
	call	401C00h

l00401275:
	push	0h
	push	1h
	call	fn004015EB
	pop	ecx
	pop	ecx
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	mov	eax,esi
	jmp	4012C8h
0040128B                                  8B 4D EC 8B 01            .M...
00401290 8B 00 89 45 E0 51 50 E8 16 09 00 00 59 59 C3 8B ...E.QP.....YY..
004012A0 65 E8 E8 E8 05 00 00 84 C0 75 08 FF 75 E0 E8 35 e........u..u..5
004012B0 09 00 00 80 7D E7 00 75 05 E8 48 09 00 00 C7 45 ....}..u..H....E
004012C0 FC FE FF FF FF 8B 45 E0                         ......E.        

l004012C8:
	call	fn004019C6
	ret

;; Win32CrtStartup: 004012CE
Win32CrtStartup proc
	call	fn00401663
	jmp	40115Fh

;; fn004012D8: 004012D8
fn004012D8 proc
	push	ebp
	mov	ebp,esp
	push	0h
	call	dword ptr [__imp__SetUnhandledExceptionFilter]
	push	dword ptr [ebp+8h]
	call	dword ptr [__imp__UnhandledExceptionFilter]
	push	0C0000409h
	call	dword ptr [__imp__GetCurrentProcess]
	push	eax
	call	dword ptr [__imp__TerminateProcess]
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
	jz	401436h

l0040141D:
	mov	ecx,[ebp+0Ch]

l00401420:
	cmp	ecx,[edx+0Ch]
	jc	40142Fh

l00401425:
	mov	eax,[edx+8h]
	add	eax,[edx+0Ch]
	cmp	ecx,eax
	jc	40143Bh

l0040142F:
	add	edx,28h
	cmp	edx,esi
	jnz	401420h

l00401436:
	xor	eax,eax

l00401438:
	pop	esi
	pop	ebp
	ret

l0040143B:
	mov	eax,edx
	jmp	401438h

;; fn0040143F: 0040143F
;;   Called from:
;;     00401187 (in Win32CrtStartup)
fn0040143F proc
	call	fn00401B98
	test	eax,eax
	jnz	40144Bh

l00401448:
	xor	al,al
	ret

l0040144B:
	mov	eax,fs:[0018h]
	push	esi
	mov	esi,403338h
	mov	edx,[eax+4h]
	jmp	401460h

l0040145C:
	cmp	edx,eax
	jz	401470h

l00401460:
	xor	eax,eax
	mov	ecx,edx
	lock
	cmpxchg	[esi],ecx
	test	eax,eax
	jnz	40145Ch

l0040146C:
	xor	al,al
	pop	esi
	ret

l00401470:
	mov	al,1h
	pop	esi
	ret

;; fn00401474: 00401474
;;   Called from:
;;     0040116D (in Win32CrtStartup)
fn00401474 proc
	push	ebp
	mov	ebp,esp
	cmp	dword ptr [ebp+8h],0h
	jnz	401484h

l0040147D:
	mov	byte ptr [403354h],1h

l00401484:
	call	fn004019FE
	call	fn00401C48
	test	al,al
	jnz	401496h

l00401492:
	xor	al,al
	pop	ebp
	ret

l00401496:
	call	fn00401C48
	test	al,al
	jnz	4014A9h

l0040149F:
	push	0h
	call	fn00401C48
	pop	ecx
	jmp	401492h

l004014A9:
	mov	al,1h
	pop	ebp
	ret

;; fn004014AD: 004014AD
fn004014AD proc
	push	ebp
	mov	ebp,esp
	sub	esp,0Ch
	push	esi
	mov	esi,[ebp+8h]
	test	esi,esi
	jz	4014C0h

l004014BB:
	cmp	esi,1h
	jnz	40153Ch

l004014C0:
	call	fn00401B98
	test	eax,eax
	jz	4014F3h

l004014C9:
	test	esi,esi
	jnz	4014F3h

l004014CD:
	push	40333Ch
	call	401C24h
	pop	ecx
	test	eax,eax
	jz	4014E0h

l004014DC:
	xor	al,al
	jmp	401537h

l004014E0:
	push	403348h
	call	401C24h
	neg	eax
	pop	ecx
	sbb	al,al
	inc	al
	jmp	401537h

l004014F3:
	mov	eax,[403004h]
	lea	esi,[ebp-0Ch]
	push	edi
	and	eax,1Fh
	mov	edi,40333Ch
	push	20h
	pop	ecx
	sub	ecx,eax
	or	eax,0FFh
	ror	eax,cl
	xor	eax,[403004h]
	mov	[ebp-0Ch],eax
	mov	[ebp-8h],eax
	mov	[ebp-4h],eax
	movsd
	movsd
	movsd
	mov	edi,403348h
	mov	[ebp-0Ch],eax
	mov	[ebp-8h],eax
	lea	esi,[ebp-0Ch]
	mov	[ebp-4h],eax
	mov	al,1h
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
	push	5h
	call	fn00401774
	int	3h

;; fn00401544: 00401544
;;   Called from:
;;     00401204 (in Win32CrtStartup)
;;     00401229 (in Win32CrtStartup)
;;     00401543 (in fn004014AD)
fn00401544 proc
	push	8h
	push	4024F8h
	call	fn00401980
	and	dword ptr [ebp-4h],0h
	mov	eax,5A4Dh
	cmp	[400000h],ax
	jnz	4015BFh

l00401562:
	mov	eax,[40003Ch]
	cmp	dword ptr [eax+400000h],4550h
	jnz	4015BFh

l00401573:
	mov	ecx,10Bh
	cmp	[eax+400018h],cx
	jnz	4015BFh

l00401581:
	mov	eax,[ebp+8h]
	mov	ecx,400000h
	sub	eax,ecx
	push	eax
	push	ecx
	call	fn004013FB
	pop	ecx
	pop	ecx
	test	eax,eax
	jz	4015BFh

l00401598:
	cmp	dword ptr [eax+24h],0h
	jl	4015BFh

l0040159E:
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	mov	al,1h
	jmp	4015C8h
004015A9                            8B 45 EC 8B 00 33 C9          .E...3.
004015B0 81 38 05 00 00 C0 0F 94 C1 8B C1 C3 8B 65 E8    .8...........e. 

l004015BF:
	mov	dword ptr [ebp-4h],0FFFFFFFEh
	xor	al,al

l004015C8:
	call	fn004019C6
	ret

;; fn004015CE: 004015CE
;;   Called from:
;;     004011F0 (in Win32CrtStartup)
fn004015CE proc
	push	ebp
	mov	ebp,esp
	call	fn00401B98
	test	eax,eax
	jz	4015E9h

l004015DA:
	cmp	byte ptr [ebp+8h],0h
	jnz	4015E9h

l004015E0:
	xor	eax,eax
	mov	ecx,403338h
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
	cmp	byte ptr [403354h],0h
	jz	4015FDh

l004015F7:
	cmp	byte ptr [ebp+0Ch],0h
	jnz	40160Fh

l004015FD:
	push	dword ptr [ebp+8h]
	call	fn00401C48
	push	dword ptr [ebp+8h]
	call	fn00401C48
	pop	ecx
	pop	ecx

l0040160F:
	mov	al,1h
	pop	ebp
	ret

;; fn00401613: 00401613
;;   Called from:
;;     00401654 (in fn0040164E)
fn00401613 proc
	push	ebp
	mov	ebp,esp
	mov	eax,[403004h]
	mov	ecx,eax
	xor	eax,[40333Ch]
	and	ecx,1Fh
	push	dword ptr [ebp+8h]
	ror	eax,cl
	cmp	eax,0FFh
	jnz	401637h

l00401630:
	call	401C30h
	jmp	401642h

l00401637:
	push	40333Ch
	call	401C2Ah
	pop	ecx

l00401642:
	neg	eax
	pop	ecx
	sbb	eax,eax
	not	eax
	and	eax,[ebp+8h]
	pop	ebp
	ret

;; fn0040164E: 0040164E
fn0040164E proc
	push	ebp
	mov	ebp,esp
	push	dword ptr [ebp+8h]
	call	fn00401613
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
	sub	esp,14h
	and	dword ptr [ebp-0Ch],0h
	and	dword ptr [ebp-8h],0h
	mov	eax,[403004h]
	push	esi
	push	edi
	mov	edi,0BB40E64Eh
	mov	esi,0FFFF0000h
	cmp	eax,edi
	jz	401693h

l00401686:
	test	esi,eax
	jz	401693h

l0040168A:
	not	eax
	mov	[403000h],eax
	jmp	4016F9h

l00401693:
	lea	eax,[ebp-0Ch]
	push	eax
	call	dword ptr [__imp__GetSystemTimeAsFileTime]
	mov	eax,[ebp-8h]
	xor	eax,[ebp-0Ch]
	mov	[ebp-4h],eax
	call	dword ptr [__imp__GetCurrentThreadId]
	xor	[ebp-4h],eax
	call	dword ptr [__imp__GetCurrentProcessId]
	xor	[ebp-4h],eax
	lea	eax,[ebp-14h]
	push	eax
	call	dword ptr [__imp__QueryPerformanceCounter]
	mov	ecx,[ebp-10h]
	lea	eax,[ebp-4h]
	xor	ecx,[ebp-14h]
	xor	ecx,[ebp-4h]
	xor	ecx,eax
	cmp	ecx,edi
	jnz	4016DBh

l004016D4:
	mov	ecx,0BB40E64Fh
	jmp	4016EBh

l004016DB:
	test	esi,ecx
	jnz	4016EBh

l004016DF:
	mov	eax,ecx
	or	eax,4711h
	shl	eax,10h
	or	ecx,eax

l004016EB:
	mov	[403004h],ecx
	not	ecx
	mov	[403000h],ecx

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
	mov	eax,4000h
	ret

;; fn00401709: 00401709
fn00401709 proc
	xor	eax,eax
	ret

;; fn0040170C: 0040170C
fn0040170C proc
	push	403358h
	call	dword ptr [__imp__InitializeSListHead]
	ret

;; fn00401718: 00401718
fn00401718 proc
	push	30000h
	push	10000h
	push	0h
	call	401C36h
	add	esp,0Ch
	test	eax,eax
	jnz	401731h

l00401730:
	ret

l00401731:
	push	7h
	call	fn00401774
	int	3h

;; fn00401739: 00401739
;;   Called from:
;;     00401738 (in fn00401718)
;;     0040174D (in fn0040173F)
fn00401739 proc
	mov	eax,403360h
	ret

;; fn0040173F: 0040173F
fn0040173F proc
	call	fn00401050
	mov	ecx,[eax+4h]
	or	dword ptr [eax],4h
	mov	[eax+4h],ecx
	call	fn00401739
	mov	ecx,[eax+4h]
	or	dword ptr [eax],2h
	mov	[eax+4h],ecx
	ret

;; fn0040175C: 0040175C
fn0040175C proc
	xor	eax,eax
	cmp	[40300Ch],eax
	setz	al
	ret

;; fn00401768: 00401768
;;   Called from:
;;     004011F6 (in Win32CrtStartup)
fn00401768 proc
	mov	eax,403388h
	ret

;; fn0040176E: 0040176E
;;   Called from:
;;     0040121D (in Win32CrtStartup)
fn0040176E proc
	mov	eax,403384h
	ret

;; fn00401774: 00401774
;;   Called from:
;;     00401179 (in Win32CrtStartup)
;;     0040153E (in fn004014AD)
;;     00401733 (in fn00401718)
fn00401774 proc
	push	ebp
	mov	ebp,esp
	sub	esp,324h
	push	ebx
	push	esi
	push	17h
	call	401C42h
	test	eax,eax
	jz	40178Fh

l0040178A:
	mov	ecx,[ebp+8h]
	int	29h

l0040178F:
	xor	esi,esi
	lea	eax,[ebp-324h]
	push	2CCh
	push	esi
	push	eax
	mov	[403368h],esi
	call	401BA6h
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
	call	401BA6h
	mov	eax,[ebp+4h]
	add	esp,0Ch
	mov	dword ptr [ebp-58h],40000015h
	mov	dword ptr [ebp-54h],1h
	mov	[ebp-4Ch],eax
	call	dword ptr [__imp__IsDebuggerPresent]
	push	esi
	lea	ebx,[eax-1h]
	neg	ebx
	lea	eax,[ebp-58h]
	mov	[ebp-8h],eax
	lea	eax,[ebp-324h]
	sbb	bl,bl
	mov	[ebp-4h],eax
	inc	bl
	call	dword ptr [__imp__SetUnhandledExceptionFilter]
	lea	eax,[ebp-8h]
	push	eax
	call	dword ptr [__imp__UnhandledExceptionFilter]
	test	eax,eax
	jnz	401889h

l0040187C:
	movzx	eax,bl
	neg	eax
	sbb	eax,eax
	and	[403368h],eax

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
	push	0h
	call	dword ptr [__imp__GetModuleHandleW]
	mov	ecx,eax
	test	ecx,ecx
	jnz	4018A0h

l0040189D:
	xor	al,al
	ret

l004018A0:
	mov	eax,5A4Dh
	cmp	[ecx],ax
	jnz	40189Dh

l004018AA:
	mov	eax,[ecx+3Ch]
	add	eax,ecx
	cmp	dword ptr [eax],4550h
	jnz	40189Dh

l004018B7:
	mov	ecx,10Bh
	cmp	[eax+18h],cx
	jnz	40189Dh

l004018C2:
	cmp	dword ptr [eax+74h],0Eh
	jbe	40189Dh

l004018C8:
	cmp	dword ptr [eax+0E8h],0h
	setnz	al
	ret

;; fn004018D3: 004018D3
fn004018D3 proc
	push	4018DFh
	call	dword ptr [__imp__SetUnhandledExceptionFilter]
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
	mov	esi,4024C8h
	mov	ebx,4024C8h
	cmp	esi,ebx
	jnc	401948h

l00401930:
	push	edi

l00401931:
	mov	edi,[esi]
	test	edi,edi
	jz	401940h

l00401937:
	mov	ecx,edi
	call	fn00401976
	call	edi

l00401940:
	add	esi,4h
	cmp	esi,ebx
	jc	401931h

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
	jmp	dword ptr [4020D0h]
0040197C                                     CC CC CC CC             ....

;; fn00401980: 00401980
;;   Called from:
;;     00401166 (in Win32CrtStartup)
;;     0040154B (in fn00401544)
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
;;     004012C8 (in Win32CrtStartup)
;;     004015C8 (in fn00401544)
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
004019E0 14 FF 75 10 FF 75 0C FF 75 08 68 90 10 40 00 68 ..u..u..u.h..@.h
004019F0 04 30 40 00 E8 B3 01 00 00 83 C4 18 5D C3       .0@.........].  

;; fn004019FE: 004019FE
;;   Called from:
;;     00401484 (in fn00401474)
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
	call	401C42h
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
;;     0040143F (in fn0040143F)
;;     004014C0 (in fn004014AD)
;;     004015D1 (in fn004015CE)
fn00401B98 proc
	xor	eax,eax
	cmp	[403014h],eax
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
	mov	al,1h
	ret
