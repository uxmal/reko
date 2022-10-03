;;; Segment __TEXT,__text (0000000100001778)

;; fn0000000100001778: 0000000100001778
fn0000000100001778 proc
	push	0h
	mov	rbp,rsp
	and	rsp,0F0h
	mov	rdi,[rbp+8h]
	lea	rsi,[rbp+10h]
	mov	edx,edi
	add	edx,1h
	shl	edx,3h
	add	rdx,rsi
	mov	rcx,rdx
	jmp	10000179Dh

l0000000100001799:
	add	rcx,8h

l000000010000179D:
	cmp	qword ptr [rcx],0h
	jnz	100001799h

l00000001000017A3:
	add	rcx,8h
	call	1000026A0h
	mov	edi,eax
	call	100004E14h
	hlt
00000001000017B4             55 48 89 E5 48 83 C7 68 48 83 C6 68     UH..H..hH..h
00000001000017C0 5D E9 3E 37 00 00 55 48 89 E5 48 8D 46 68 48 8D ].>7..UH..H.FhH.
00000001000017D0 77 68 48 89 C7 5D E9 29 37 00 00 55 48 89 E5 48 whH..].)7..UH..H
00000001000017E0 8B 47 60 48 8B 48 50 48 8B 56 60 4C 8B 42 50 49 .G`H.HPH.V`L.BPI
00000001000017F0 39 C8 7E 07 B8 FF FF FF FF 5D C3 49 39 C8 7D 07 9.~......].I9.}.
0000000100001800 B8 01 00 00 00 EB F2 48 8B 40 58 48 8B 4A 58 48 .......H.@XH.JXH
0000000100001810 39 C1 7F E0 48 39 C1 7C E7 48 89 F0 48 83 C0 68 9...H9.|.H..H..h
0000000100001820 48 89 FE 48 83 C6 68 48 89 C7 5D E9 D4 36 00 00 H..H..hH..]..6..
0000000100001830 55 48 89 E5 48 8B 47 60 48 8B 40 60 48 8B 4E 60 UH..H.G`H.@`H.N`
0000000100001840 48 8B 49 60 48 39 C1 7E 07 B8 FF FF FF FF EB 0A H.I`H9.~........
0000000100001850 48 39 C1 7D 07 B8 01 00 00 00 5D C3 48 89 F0 48 H9.}......].H..H
0000000100001860 83 C0 68 48 89 FE 48 83 C6 68 48 89 C7 5D E9 91 ..hH..H..hH..]..
0000000100001870 36 00 00 55 48 89 E5 48 8B 47 60 48 8B 48 40 48 6..UH..H.G`H.H@H
0000000100001880 8B 56 60 4C 8B 42 40 49 39 C8 7E 07 B8 FF FF FF .V`L.B@I9.~.....
0000000100001890 FF 5D C3 49 39 C8 7D 07 B8 01 00 00 00 EB F2 48 .].I9.}........H
00000001000018A0 8B 40 48 48 8B 4A 48 48 39 C1 7F E0 48 39 C1 7C .@HH.JHH9...H9.|
00000001000018B0 E7 48 89 F0 48 83 C0 68 48 89 FE 48 83 C6 68 48 .H..H..hH..H..hH
00000001000018C0 89 C7 5D E9 3C 36 00 00 55 48 89 E5 48 8B 47 60 ..].<6..UH..H.G`
00000001000018D0 48 8B 48 20 48 8B 56 60 4C 8B 42 20 49 39 C8 7E H.H H.V`L.B I9.~
00000001000018E0 07 B8 FF FF FF FF 5D C3 49 39 C8 7D 07 B8 01 00 ......].I9.}....
00000001000018F0 00 00 EB F2 48 8B 40 28 48 8B 4A 28 48 39 C1 7F ....H.@(H.J(H9..
0000000100001900 E0 48 39 C1 7C E7 48 89 F0 48 83 C0 68 48 89 FE .H9.|.H..H..hH..
0000000100001910 48 83 C6 68 48 89 C7 5D E9 E7 35 00 00 55 48 89 H..hH..]..5..UH.
0000000100001920 E5 48 8B 47 60 48 8B 48 30 48 8B 56 60 4C 8B 42 .H.G`H.H0H.V`L.B
0000000100001930 30 49 39 C8 7E 07 B8 FF FF FF FF 5D C3 49 39 C8 0I9.~......].I9.
0000000100001940 7D 07 B8 01 00 00 00 EB F2 48 8B 40 38 48 8B 4A }........H.@8H.J
0000000100001950 38 48 39 C1 7F E0 48 39 C1 7C E7 48 89 F0 48 83 8H9...H9.|.H..H.
0000000100001960 C0 68 48 89 FE 48 83 C6 68 48 89 C7 5D E9 92 35 .hH..H..hH..]..5
0000000100001970 00 00 55 48 89 E5 48 8B 47 60 48 8B 48 50 48 8B ..UH..H.G`H.HPH.
0000000100001980 56 60 4C 8B 42 50 49 39 C8 7E 07 B8 01 00 00 00 V`L.BPI9.~......
0000000100001990 5D C3 49 39 C8 7D 07 B8 FF FF FF FF EB F2 48 8B ].I9.}........H.
00000001000019A0 40 58 48 8B 4A 58 48 39 C1 7F E0 48 39 C1 7C E7 @XH.JXH9...H9.|.
00000001000019B0 48 83 C7 68 48 83 C6 68 5D E9 46 35 00 00 55 48 H..hH..h].F5..UH
00000001000019C0 89 E5 48 8B 47 60 48 8B 40 60 48 8B 4E 60 48 8B ..H.G`H.@`H.N`H.
00000001000019D0 49 60 48 39 C1 7E 07 B8 01 00 00 00 EB 0A 48 39 I`H9.~........H9
00000001000019E0 C1 7D 07 B8 FF FF FF FF 5D C3 48 83 C7 68 48 83 .}......].H..hH.
00000001000019F0 C6 68 5D E9 0C 35 00 00 55 48 89 E5 48 8B 47 60 .h]..5..UH..H.G`
0000000100001A00 48 8B 48 40 48 8B 56 60 4C 8B 42 40 49 39 C8 7E H.H@H.V`L.B@I9.~
0000000100001A10 07 B8 01 00 00 00 5D C3 49 39 C8 7D 07 B8 FF FF ......].I9.}....
0000000100001A20 FF FF EB F2 48 8B 40 48 48 8B 4A 48 48 39 C1 7F ....H.@HH.JHH9..
0000000100001A30 E0 48 39 C1 7C E7 48 83 C7 68 48 83 C6 68 5D E9 .H9.|.H..hH..h].
0000000100001A40 C0 34 00 00 55 48 89 E5 48 8B 47 60 48 8B 48 20 .4..UH..H.G`H.H 
0000000100001A50 48 8B 56 60 4C 8B 42 20 49 39 C8 7E 07 B8 01 00 H.V`L.B I9.~....
0000000100001A60 00 00 5D C3 49 39 C8 7D 07 B8 FF FF FF FF EB F2 ..].I9.}........
0000000100001A70 48 8B 40 28 48 8B 4A 28 48 39 C1 7F E0 48 39 C1 H.@(H.J(H9...H9.
0000000100001A80 7C E7 48 83 C7 68 48 83 C6 68 5D E9 74 34 00 00 |.H..hH..h].t4..
0000000100001A90 55 48 89 E5 48 8B 47 60 48 8B 48 30 48 8B 56 60 UH..H.G`H.H0H.V`
0000000100001AA0 4C 8B 42 30 49 39 C8 7E 07 B8 01 00 00 00 5D C3 L.B0I9.~......].
0000000100001AB0 49 39 C8 7D 07 B8 FF FF FF FF EB F2 48 8B 40 38 I9.}........H.@8
0000000100001AC0 48 8B 4A 38 48 39 C1 7F E0 48 39 C1 7C E7 48 83 H.J8H9...H9.|.H.
0000000100001AD0 C7 68 48 83 C6 68 5D E9 28 34 00 00 55 48 89 E5 .hH..h].(4..UH..
0000000100001AE0 48 8B 3F 66 8B 47 58 66 83 F8 07 75 04 31 C0 EB H.?f.GXf...u.1..
0000000100001AF0 47 48 8B 36 66 8B 4E 58 66 83 F9 07 74 EF 66 83 GH.6f.NXf...t.f.
0000000100001B00 F8 0A 74 36 66 83 F9 0A 74 30 66 39 C8 74 31 66 ..t6f...t0f9.t1f
0000000100001B10 83 7F 56 00 75 2A 8A 15 A4 4A 00 00 84 D2 75 20 ..V.u*...J....u 
0000000100001B20 66 83 F8 01 75 07 B8 01 00 00 00 EB 0B 66 83 F9 f...u........f..
0000000100001B30 01 75 0D B8 FF FF FF FF 5D C3 5D E9 74 FC FF FF .u......].].t...
0000000100001B40 48 8B 05 81 4A 00 00 5D FF E0                   H...J..]..      

;; fn0000000100001B4A: 0000000100001B4A
;;   Called from:
;;     0000000100002400 (in fn00000001000023B0)
;;     000000010000259D (in fn00000001000023B0)
fn0000000100001B4A proc
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbx
	sub	rsp,+128h
	mov	[rbp-108h],rsi
	mov	[rbp-128h],rdi
	mov	rax,[0000000100006030]                                 ; [rip+000044BD]
	mov	rax,[rax]
	mov	[rbp-30h],rax
	test	rsi,rsi
	jz	100002322h

l0000000100001B83:
	lea	rax,[0000000100006574]                                 ; [rip+000049EA]
	cmp	dword ptr [rax],0h
	jz	100001B9Bh

l0000000100001B8F:
	mov	dword ptr [rbp-114h],1h
	jmp	100001BBDh

l0000000100001B9B:
	lea	rax,[0000000100006578]                                 ; [rip+000049D6]
	cmp	dword ptr [rax],0h
	jnz	100001B8Fh

l0000000100001BA7:
	lea	rax,[0000000100006598]                                 ; [rip+000049EA]
	cmp	dword ptr [rax],0h
	jnz	100001B8Fh

l0000000100001BB3:
	mov	dword ptr [rbp-114h],0h

l0000000100001BBD:
	lea	rdi,[00000001000052A8]                                 ; [rip+000036E4]
	call	100004E56h
	test	rax,rax
	mov	qword ptr [rbp-0B0h],+0h
	mov	dword ptr [rbp-0DCh],0h
	mov	dword ptr [rbp-0E0h],0h
	mov	dword ptr [rbp-0E4h],0h
	mov	qword ptr [rbp-0C8h],+0h
	mov	qword ptr [rbp-0C0h],+0h
	mov	qword ptr [rbp-0D8h],+0h
	mov	qword ptr [rbp-0B8h],+0h
	mov	qword ptr [rbp-0D0h],+0h
	jz	100001E77h

l0000000100001C32:
	mov	rbx,rax
	mov	r14b,[rbx]
	test	r14b,r14b
	jz	100001E77h

l0000000100001C41:
	mov	rdi,rbx
	call	100004F22h
	lea	rdi,[rax+rax+2h]
	call	100004EAAh
	test	rax,rax
	jz	100002337h

l0000000100001C5C:
	mov	rdi,rax
	cmp	r14b,3Ah
	jnz	100001C74h

l0000000100001C65:
	mov	word ptr [rdi],3A30h
	mov	byte ptr [rdi+2h],0h
	lea	rax,[rdi+2h]
	jmp	100001C7Fh

l0000000100001C74:
	mov	[rdi],r14b
	mov	byte ptr [rdi+1h],0h
	lea	rax,[rdi+1h]

l0000000100001C7F:
	mov	cl,[rbx+1h]
	test	cl,cl
	jz	100001CBAh

l0000000100001C86:
	add	rbx,2h

l0000000100001C8A:
	cmp	byte ptr [rbx-2h],3Ah
	jnz	100001CA8h

l0000000100001C90:
	cmp	cl,3Ah
	jnz	100001CA8h

l0000000100001C95:
	mov	byte ptr [rax],30h
	mov	cl,[rbx-1h]
	mov	[rax+1h],cl
	mov	byte ptr [rax+3h],0h
	add	rax,2h
	jmp	100001CB1h

l0000000100001CA8:
	mov	[rax],cl
	mov	byte ptr [rax+2h],0h
	inc	rax

l0000000100001CB1:
	mov	cl,[rbx]
	inc	rbx
	test	cl,cl
	jnz	100001C8Ah

l0000000100001CBA:
	cmp	byte ptr [rax-1h],3Ah
	jnz	100001CC5h

l0000000100001CC0:
	mov	word ptr [rax],30h

l0000000100001CC5:
	lea	rax,[rbp-0D0h]
	mov	[rsp+20h],rax
	lea	rax,[rbp-0C0h]
	mov	[rsp+18h],rax
	lea	rax,[rbp-0B0h]
	mov	[rsp+10h],rax
	lea	rax,[rbp-0DCh]
	mov	[rsp+8h],rax
	lea	rax,[rbp-0E0h]
	mov	[rsp],rax
	lea	rsi,[00000001000052C1]                                 ; [rip+000035BA]
	lea	rdx,[rbp-0D8h]
	lea	rcx,[rbp-0B8h]
	lea	r8,[rbp-0C8h]
	lea	r9,[rbp-0E4h]
	xor	al,al
	call	100004EFEh
	lea	rcx,[0000000100006580]                                 ; [rip+0000484F]
	mov	dword ptr [rcx],1h
	cmp	eax,8h
	ja	100001DCBh

l0000000100001D40:
	mov	eax,eax
	lea	rcx,[000000010000238C]                                 ; [rip+00000643]
	movsxd	rax,dword ptr [rcx+rax*4]
	add	rax,rcx
	jmp	rax
0000000100001D52       48 C7 85 28 FF FF FF 00 00 00 00 48 C7 85   H..(.......H..
0000000100001D60 48 FF FF FF 00 00 00 00 48 C7 85 38 FF FF FF 00 H.......H..8....
0000000100001D70 00 00 00 C7 85 1C FF FF FF 00 00 00 00 C7 85 20 ............... 
0000000100001D80 FF FF FF 00 00 00 00 C7 85 24 FF FF FF 00 00 00 .........$......
0000000100001D90 00 48 C7 85 50 FF FF FF 00 00 00 00 48 C7 85 40 .H..P.......H..@
0000000100001DA0 FF FF FF 00 00 00 00 48 C7 85 30 FF FF FF 00 00 .......H..0.....
0000000100001DB0 00 00 48 8D 05 AB 47 00 00 83 38 00 75 0D 48 8D ..H...G...8.u.H.
0000000100001DC0 05 BB 47 00 00 C7 00 00 00 00 00                ..G........     

l0000000100001DCB:
	mov	rax,[rbp-0D8h]
	test	rax,rax
	jz	10000234Ah

l0000000100001DDB:
	mov	ecx,1h

l0000000100001DE0:
	add	rcx,rcx
	dec	rax
	lea	rcx,[rcx+rcx*4]
	jnz	100001DE0h

l0000000100001DEC:
	dec	rcx

l0000000100001DEF:
	mov	[rbp-0D8h],rcx
	mov	rax,[rbp-0B8h]
	test	rax,rax
	jz	100002351h

l0000000100001E06:
	mov	ecx,1h

l0000000100001E0B:
	add	rcx,rcx
	dec	rax
	lea	rcx,[rcx+rcx*4]
	jnz	100001E0Bh

l0000000100001E17:
	dec	rcx

l0000000100001E1A:
	mov	[rbp-0B8h],rcx
	mov	rax,[rbp-0C8h]
	test	rax,rax
	jz	100002358h

l0000000100001E31:
	mov	ecx,1h

l0000000100001E36:
	add	rcx,rcx
	dec	rax
	lea	rcx,[rcx+rcx*4]
	jnz	100001E36h

l0000000100001E42:
	dec	rcx

l0000000100001E45:
	mov	[rbp-0C8h],rcx
	mov	rax,[rbp-0B0h]
	test	rax,rax
	jz	10000235Fh

l0000000100001E5C:
	mov	ecx,1h

l0000000100001E61:
	add	rcx,rcx
	dec	rax
	lea	rcx,[rcx+rcx*4]
	jnz	100001E61h

l0000000100001E6D:
	dec	rcx

l0000000100001E70:
	mov	[rbp-0B0h],rcx

l0000000100001E77:
	xor	eax,eax
	mov	[rbp-110h],rax
	xor	ecx,ecx
	mov	[rbp-11Ch],ecx
	mov	[rbp-118h],ecx
	mov	rbx,[rbp-108h]
	mov	[rbp-0F0h],rax
	jmp	1000021AFh

l0000000100001EA1:
	mov	ax,[rbx+58h]
	cmp	ax,7h
	jz	100001EB1h

l0000000100001EAB:
	cmp	ax,0Ah
	jnz	100001EE5h

l0000000100001EB1:
	mov	edi,[rbx+38h]
	call	100004F16h
	lea	rsi,[rbx+68h]
	lea	rcx,[00000001000052F5]                                 ; [rip+00003431]
	mov	rdi,rcx
	mov	rdx,rax
	xor	al,al
	call	100004F58h
	mov	qword ptr [rbx+18h],+1h
	mov	[00000001000065D0],1h                                  ; [rip+000046F0]
	jmp	1000021ABh

l0000000100001EE5:
	cmp	qword ptr [rbp-128h],0h
	jnz	100001F0Ch

l0000000100001EEF:
	cmp	ax,1h
	jnz	100001F1Ah

l0000000100001EF5:
	mov	al,[00000001000065C0]                                  ; [rip+000046C5]

l0000000100001EFB:
	test	al,al
	jnz	100001F1Ah

l0000000100001EFF:
	mov	qword ptr [rbx+18h],+1h
	jmp	1000021ABh

l0000000100001F0C:
	cmp	byte ptr [rbx+68h],2Eh
	jnz	100001F1Ah

l0000000100001F12:
	mov	al,[00000001000065D1]                                  ; [rip+000046B9]
	jmp	100001EFBh

l0000000100001F1A:
	movzx	eax,word ptr [rbx+42h]
	cmp	rax,[rbp-0C0h]
	jbe	100001F2Eh

l0000000100001F27:
	mov	[rbp-0C0h],rax

l0000000100001F2E:
	lea	rax,[0000000100006588]                                 ; [rip+00004653]
	cmp	dword ptr [rax],0h
	jnz	100001F46h

l0000000100001F3A:
	lea	rax,[000000010000658C]                                 ; [rip+0000464B]
	cmp	dword ptr [rax],0h
	jz	100001F63h

l0000000100001F46:
	movzx	esi,word ptr [rbx+42h]
	lea	rdi,[rbx+68h]
	call	100004AFAh
	cmp	rax,[rbp-0C0h]
	jbe	100001F63h

l0000000100001F5C:
	mov	[rbp-0C0h],rax

l0000000100001F63:
	cmp	dword ptr [rbp-114h],0h
	jz	1000021A5h

l0000000100001F70:
	mov	r14,[rbx+60h]
	mov	rax,[r14+68h]
	cmp	rax,[rbp-0B8h]
	jbe	100001F88h

l0000000100001F81:
	mov	[rbp-0B8h],rax

l0000000100001F88:
	mov	rax,[r14+8h]
	cmp	rax,[rbp-0D8h]
	jbe	100001F9Ch

l0000000100001F95:
	mov	[rbp-0D8h],rax

l0000000100001F9C:
	movzx	eax,word ptr [r14+6h]
	cmp	rax,[rbp-0C8h]
	jbe	100001FB1h

l0000000100001FAA:
	mov	[rbp-0C8h],rax

l0000000100001FB1:
	mov	rax,[r14+60h]
	cmp	rax,[rbp-0B0h]
	jle	100001FC5h

l0000000100001FBE:
	mov	[rbp-0B0h],rax

l0000000100001FC5:
	mov	rax,[rbp-110h]
	add	rax,[r14+68h]
	mov	[rbp-110h],rax
	lea	rax,[0000000100006578]                                 ; [rip+0000459A]
	cmp	dword ptr [rax],0h
	jz	1000021A5h

l0000000100001FE7:
	lea	rax,[0000000100006584]                                 ; [rip+00004596]
	cmp	dword ptr [rax],0h
	mov	ecx,[r14+10h]
	jz	10000203Eh

l0000000100001FF7:
	lea	rax,[rbp-62h]
	mov	[rbp-0F8h],rax
	mov	rdi,rax
	mov	esi,0Dh
	lea	rax,[00000001000052FC]                                 ; [rip+000032EB]
	mov	rdx,rax
	xor	al,al
	call	100004EF8h
	mov	ecx,[r14+14h]
	lea	r15,[rbp-55h]
	mov	rdi,r15
	mov	esi,0Dh
	lea	rax,[00000001000052FC]                                 ; [rip+000032CA]
	mov	rdx,rax
	xor	al,al
	call	100004EF8h
	jmp	10000205Ch

l000000010000203E:
	mov	edi,ecx
	xor	esi,esi
	call	100004F4Ch
	mov	[rbp-0F8h],rax
	mov	edi,[r14+14h]
	xor	esi,esi
	call	100004E80h
	mov	r15,rax

l000000010000205C:
	mov	rdi,[rbp-0F8h]
	call	100004F22h
	mov	ecx,[rbp-0E4h]
	cmp	rax,rcx
	mov	[rbp-100h],rax
	jbe	100002080h

l000000010000207A:
	mov	[rbp-0E4h],eax

l0000000100002080:
	mov	rdi,r15
	call	100004F22h
	mov	ecx,[rbp-0E0h]
	cmp	rax,rcx
	mov	r12,rax
	jbe	10000209Dh

l0000000100002096:
	mov	[rbp-0E0h],r12d

l000000010000209D:
	lea	rax,[0000000100006568]                                 ; [rip+000044C4]
	cmp	dword ptr [rax],0h
	jnz	1000020ADh

l00000001000020A9:
	xor	eax,eax
	jmp	10000210Ah

l00000001000020AD:
	mov	edi,[r14+74h]
	call	100004E1Ah
	test	rax,rax
	jz	1000020D7h

l00000001000020BB:
	cmp	byte ptr [rax],0h
	jnz	1000020D7h

l00000001000020C0:
	mov	rdi,rax
	call	100004E2Ch
	lea	rax,[00000001000052FF]                                 ; [rip+00003230]
	mov	rdi,rax
	call	100004F10h

l00000001000020D7:
	mov	[rbp-0F0h],rax
	cmp	qword ptr [rbp-0F0h],0h
	jz	100002366h

l00000001000020EC:
	mov	rdi,[rbp-0F0h]
	call	100004F22h
	movsxd	rcx,dword ptr [rbp-0DCh]
	cmp	rax,rcx
	jbe	10000210Ah

l0000000100002104:
	mov	[rbp-0DCh],eax

l000000010000210A:
	add	r12,[rbp-100h]
	lea	rdi,[rax+24h]
	call	100004EAAh
	test	rax,rax
	jz	100002337h

l0000000100002124:
	mov	r13,rax
	lea	rdi,[r13+18h]
	mov	[r13+0h],rdi
	mov	rsi,[rbp-0F8h]
	call	100004F0Ah
	mov	rax,[rbp-100h]
	lea	rdi,[rax+r13+19h]
	mov	[r13+8h],rdi
	mov	rsi,r15
	call	100004F0Ah
	movzx	eax,word ptr [r14+4h]
	and	eax,0F000h
	cmp	eax,2000h
	jz	10000216Bh

l0000000100002164:
	cmp	eax,6000h
	jnz	100002175h

l000000010000216B:
	mov	dword ptr [rbp-11Ch],1h

l0000000100002175:
	lea	rax,[0000000100006568]                                 ; [rip+000043EC]
	cmp	dword ptr [rax],0h
	jz	1000021A1h

l0000000100002181:
	lea	rdi,[r13+1Ah]
	mov	[r13+10h],rdi
	mov	r14,[rbp-0F0h]
	mov	rsi,r14
	call	100004F0Ah
	mov	rdi,r14
	call	100004E2Ch

l00000001000021A1:
	mov	[rbx+20h],r13

l00000001000021A5:
	inc	dword ptr [rbp-118h]

l00000001000021AB:
	mov	rbx,[rbx+10h]

l00000001000021AF:
	test	rbx,rbx
	jnz	100001EA1h

l00000001000021B8:
	cmp	dword ptr [rbp-118h],0h
	jz	100002322h

l00000001000021C5:
	mov	rsi,[rbp-108h]
	mov	[rbp-0A8h],rsi
	mov	ecx,[rbp-118h]
	mov	[rbp-94h],ecx
	mov	eax,[rbp-0C0h]
	mov	[rbp-90h],eax
	cmp	dword ptr [rbp-114h],0h
	jz	1000022E2h

l00000001000021F8:
	mov	eax,[rbp-11Ch]
	mov	[rbp-98h],eax
	mov	rax,[rbp-110h]
	mov	[rbp-0A0h],rax
	mov	rcx,[rbp-0B8h]
	lea	rbx,[rbp-48h]
	lea	r14,[000000010000530D]                                 ; [rip+000030E9]
	mov	rdi,rbx
	mov	esi,18h
	mov	rdx,r14
	xor	al,al
	call	100004EF8h
	mov	rdi,rbx
	call	100004F22h
	mov	[rbp-8Ch],eax
	mov	eax,[rbp-0DCh]
	mov	[rbp-88h],eax
	mov	eax,[rbp-0D0h]
	mov	[rbp-84h],eax
	mov	eax,[rbp-0E0h]
	mov	[rbp-80h],eax
	mov	rcx,[rbp-0D8h]
	lea	rdx,[0000000100005311]                                 ; [rip+0000309E]
	mov	rdi,rbx
	mov	esi,18h
	xor	al,al
	call	100004EF8h
	mov	rdi,rbx
	call	100004F22h
	mov	[rbp-7Ch],eax
	mov	rcx,[rbp-0C8h]
	lea	rdx,[0000000100005316]                                 ; [rip+0000307B]
	mov	rdi,rbx
	mov	esi,18h
	xor	al,al
	call	100004EF8h
	mov	rdi,rbx
	call	100004F22h
	mov	[rbp-78h],eax
	mov	rcx,[rbp-0B0h]
	mov	rdi,rbx
	mov	esi,18h
	mov	rdx,r14
	xor	al,al
	call	100004EF8h
	mov	rdi,rbx
	call	100004F22h
	mov	[rbp-74h],eax
	mov	eax,[rbp-0E4h]
	mov	[rbp-70h],eax

l00000001000022E2:
	lea	rdi,[rbp-0A8h]
	call	[00000001000065D8]                                    ; [rip+000042E9]
	mov	[00000001000065E0],1h                                  ; [rip+000042EA]
	lea	rax,[0000000100006578]                                 ; [rip+0000427B]
	cmp	dword ptr [rax],0h
	jz	100002322h

l0000000100002302:
	mov	rbx,[rbp-108h]
	mov	rdi,[rbx+20h]
	call	100004E2Ch
	mov	rbx,[rbx+10h]
	mov	[rbp-108h],rbx
	test	rbx,rbx
	jnz	100002302h

l0000000100002322:
	mov	rax,[0000000100006030]                                 ; [rip+00003D07]
	mov	rax,[rax]
	cmp	rax,[rbp-30h]
	jz	100002379h

l0000000100002332:
	call	100004DC0h

l0000000100002337:
	mov	edi,1h
	lea	rsi,[00000001000052B5]                                 ; [rip+00002F72]
	xor	al,al
	call	100004E0Eh

l000000010000234A:
	xor	ecx,ecx
	jmp	100001DEFh

l0000000100002351:
	xor	ecx,ecx
	jmp	100001E1Ah

l0000000100002358:
	xor	ecx,ecx
	jmp	100001E45h

l000000010000235F:
	xor	ecx,ecx
	jmp	100001E70h

l0000000100002366:
	mov	edi,1h
	lea	rsi,[0000000100005301]                                 ; [rip+00002F8F]
	xor	al,al
	call	100004E0Eh

l0000000100002379:
	add	rsp,+128h
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	ret
000000010000238B                                  90 C6 F9 FF FF            .....
0000000100002390 D1 F9 FF FF DC F9 FF FF E7 F9 FF FF F1 F9 FF FF ................
00000001000023A0 FB F9 FF FF 05 FA FF FF 10 FA FF FF 1B FA FF FF ................

;; fn00000001000023B0: 00000001000023B0
;;   Called from:
;;     0000000100002F96 (in fn00000001000026A0)
fn00000001000023B0 proc
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbx
	sub	rsp,8h
	mov	ebx,edx
	mov	[rbp-30h],edi
	xor	eax,eax
	mov	cl,[00000001000065E1]                                  ; [rip+00004213]
	test	cl,cl
	lea	rdx,[0000000100001ADC]                                 ; [rip-000008FB]
	cmovnz	rdx,rax

l00000001000023DB:
	mov	rdi,rsi
	mov	esi,ebx
	call	100004E3Eh
	mov	r14,rax
	test	r14,r14
	jz	10000265Bh

l00000001000023F1:
	xor	esi,esi
	mov	rdi,r14
	call	100004E32h
	xor	edi,edi
	mov	rsi,rax
	call	100001B4Ah
	mov	al,[00000001000065C0]                                  ; [rip+000041B5]
	cmp	al,1h
	jz	10000266Eh

l0000000100002413:
	mov	al,[00000001000065E2]                                  ; [rip+000041C9]
	test	al,al
	jnz	10000242Bh

l000000010000241D:
	test	bl,8h
	jz	10000242Bh

l0000000100002422:
	mov	dword ptr [rbp-2Ch],100h
	jmp	100002432h

l000000010000242B:
	mov	dword ptr [rbp-2Ch],0h

l0000000100002432:
	mov	rdi,r14
	call	100004E44h
	test	rax,rax
	jz	100002619h

l0000000100002443:
	and	ebx,2h
	lea	r15,[00000001000054CC]                                 ; [rip+0000307F]

l000000010000244D:
	mov	r12,rax
	movzx	eax,word ptr [r12+58h]
	cmp	eax,0Ch
	jg	100002472h

l000000010000245B:
	dec	eax
	cmp	eax,6h
	ja	10000247Bh

l0000000100002462:
	lea	rcx,[0000000100002684]                                 ; [rip+0000021B]
	movsxd	rax,dword ptr [rcx+rax*4]
	add	rax,rcx
	jmp	rax

l0000000100002472:
	cmp	eax,0Dh
	jz	1000025D0h

l000000010000247B:
	mov	rdi,r14
	call	100004E44h
	test	rax,rax
	jnz	10000244Dh

l0000000100002488:
	jmp	100002619h

l000000010000248D:
	mov	rsi,r12
	add	rsi,68h
	xor	al,al
	lea	r12,[0000000100005323]                                 ; [rip+00002E86]
	mov	rdi,r12
	call	100004F58h
	lea	r12,[0000000100005340]                                 ; [rip+00002E94]
	mov	rdi,r12
	mov	rsi,r15
	call	100004E08h
	test	al,al
	jz	10000247Bh

l00000001000024BB:
	mov	[00000001000065D0],1h                                  ; [rip+0000410E]
	jmp	10000247Bh

l00000001000024C4:
	mov	edi,[r12+38h]
	call	100004F16h
	mov	rsi,r12
	add	rsi,68h
	lea	r12,[00000001000052F5]                                 ; [rip+00002E19]
	mov	rdi,r12

l00000001000024DF:
	mov	rdx,rax
	xor	al,al
	call	100004F58h
	jmp	1000024BBh

l00000001000024EB:
	cmp	word ptr [r12+56h],0h
	jz	10000250Ah

l00000001000024F4:
	cmp	byte ptr [r12+68h],2Eh
	jnz	10000250Ah

l00000001000024FC:
	mov	al,[00000001000065D1]                                  ; [rip+000040CF]
	cmp	al,1h
	jnz	10000247Bh

l000000010000250A:
	mov	al,[00000001000065E0]                                  ; [rip+000040D0]
	cmp	al,1h
	jnz	10000252Ch

l0000000100002514:
	mov	rsi,[r12+30h]
	xor	al,al
	lea	rcx,[0000000100005348]                                 ; [rip+00002E26]
	mov	rdi,rcx
	call	100004ECEh
	jmp	10000254Fh

l000000010000252C:
	cmp	dword ptr [rbp-30h],2h
	jl	10000254Fh

l0000000100002532:
	mov	rsi,[r12+30h]
	xor	al,al
	lea	rcx,[000000010000534E]                                 ; [rip+00002E0E]
	mov	rdi,rcx
	call	100004ECEh
	mov	[00000001000065E0],1h                                  ; [rip+00004091]

l000000010000254F:
	mov	rdi,r14
	mov	esi,[rbp-2Ch]
	call	100004E32h
	mov	r13,rax
	lea	rax,[0000000100005340]                                 ; [rip+00002DDC]
	mov	rdi,rax
	mov	rsi,r15
	call	100004E08h
	test	al,al
	jz	100002597h

l0000000100002573:
	test	ebx,ebx
	jz	100002597h

l0000000100002577:
	test	r13,r13
	jz	100002597h

l000000010000257C:
	mov	rax,r13

l000000010000257F:
	cmp	word ptr [rax+58h],0Dh
	jnz	10000258Eh

l0000000100002586:
	mov	qword ptr [rax+18h],+1h

l000000010000258E:
	mov	rax,[rax+10h]
	test	rax,rax
	jnz	10000257Fh

l0000000100002597:
	mov	rdi,r12
	mov	rsi,r13
	call	100001B4Ah
	test	r13,r13
	jz	10000247Bh

l00000001000025AB:
	mov	al,[00000001000065E2]                                  ; [rip+00004031]
	xor	al,1h
	test	al,1h
	jz	10000247Bh

l00000001000025BB:
	mov	rdi,r14
	mov	rsi,r12
	mov	edx,4h
	call	100004E4Ah
	jmp	10000247Bh

l00000001000025D0:
	lea	rax,[0000000100005340]                                 ; [rip+00002D69]
	mov	rdi,rax
	mov	rsi,r15
	call	100004E08h
	test	al,al
	jz	10000247Bh

l00000001000025EA:
	test	ebx,ebx
	jz	10000247Bh

l00000001000025F2:
	mov	edi,[r12+38h]
	test	edi,edi
	mov	eax,2h
	cmovz	edi,eax

l0000000100002601:
	call	100004F16h
	mov	rsi,r12
	add	rsi,68h
	lea	rdi,[00000001000052F5]                                 ; [rip+00002CE1]
	jmp	1000024DFh

l0000000100002619:
	call	100004DAEh
	mov	ebx,[rax]
	mov	rdi,r14
	call	100004E38h
	call	100004DAEh
	mov	[rax],ebx
	call	100004DAEh
	cmp	dword ptr [rax],0h
	jz	10000264Ch

l0000000100002639:
	mov	edi,1h
	lea	rsi,[0000000100005353]                                 ; [rip+00002D0E]
	xor	al,al
	call	100004E0Eh

l000000010000264C:
	add	rsp,8h
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	ret

l000000010000265B:
	mov	edi,1h
	lea	rsi,[000000010000531A]                                 ; [rip+00002CB3]
	xor	al,al
	call	100004E0Eh

l000000010000266E:
	mov	rdi,r14
	add	rsp,8h
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	jmp	100004E38h
l0000000100002684	dd	0xFFFFFE67
l0000000100002688	dd	0xFFFFFE09
l000000010000268C	dd	0xFFFFFDF7
l0000000100002690	dd	0xFFFFFE40
l0000000100002694	dd	0xFFFFFDF7
l0000000100002698	dd	0xFFFFFDF7
l000000010000269C	dd	0xFFFFFE40

;; fn00000001000026A0: 00000001000026A0
;;   Called from:
;;     00000001000017A7 (in fn0000000100001778)
fn00000001000026A0 proc
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbx
	sub	rsp,+648h
	mov	[rbp-658h],rsi
	mov	eax,edi
	mov	[rbp-64Ch],eax
	lea	rcx,[rbp-634h]
	mov	[rbp-640h],rcx
	test	eax,eax
	jg	1000026DAh

l00000001000026D5:
	call	10000488Bh

l00000001000026DA:
	xor	edi,edi
	lea	rsi,[0000000100005410]                                 ; [rip+00002D2D]
	call	100004EECh
	mov	edi,1h
	call	100004E92h
	test	eax,eax
	jz	100002759h

l00000001000026F6:
	mov	[00000001000062B0],50h                                 ; [rip+00003BB0]
	lea	rdi,[000000010000535D]                                 ; [rip+00002C56]
	call	100004E56h
	test	rax,rax
	jz	100002720h

l0000000100002711:
	cmp	byte ptr [rax],0h
	jz	100002720h

l0000000100002716:
	mov	rdi,rax
	call	100004E02h
	jmp	100002742h

l0000000100002720:
	mov	edi,1h
	mov	esi,40087468h
	lea	rdx,[rbp-30h]
	xor	al,al
	call	100004E8Ch
	cmp	eax,0FFh
	jz	100002748h

l000000010000273A:
	movzx	eax,word ptr [rbp-2Eh]
	test	eax,eax
	jz	100002748h

l0000000100002742:
	mov	[00000001000062B0],eax                                 ; [rip+00003B68]

l0000000100002748:
	lea	rax,[000000010000657C]                                 ; [rip+00003E2D]
	mov	dword ptr [rax],1h
	xor	ebx,ebx
	jmp	10000277Dh

l0000000100002759:
	lea	rdi,[000000010000535D]                                 ; [rip+00002BFD]
	call	100004E56h
	test	rax,rax
	jz	100002778h

l000000010000276A:
	mov	rdi,rax
	call	100004E02h
	mov	[00000001000062B0],eax                                 ; [rip+00003B38]

l0000000100002778:
	mov	ebx,1h

l000000010000277D:
	call	100004E74h
	test	eax,eax
	jz	1000027A8h

l0000000100002786:
	mov	r14d,10h
	xor	eax,eax
	mov	[rbp-650h],eax
	mov	r15d,eax
	mov	[rbp-644h],eax
	mov	[rbp-648h],eax
	jmp	100002B5Eh

l00000001000027A8:
	mov	[00000001000065D1],1h                                  ; [rip+00003E22]
	jmp	100002786h

l00000001000027B1:
	mov	ecx,eax
	add	ecx,0CFh
	cmp	ecx,47h
	ja	100002B56h

l00000001000027BF:
	jmp	100002CE9h

l00000001000027C4:
	lea	rax,[0000000100006578]                                 ; [rip+00003DAD]
	mov	dword ptr [rax],0h
	lea	rax,[00000001000065A8]                                 ; [rip+00003DD0]
	mov	dword ptr [rax],0h
	mov	ebx,1h
	jmp	100002B5Bh

l00000001000027E8:
	lea	rax,[000000010000657C]                                 ; [rip+00003D8D]
	mov	dword ptr [rax],0h
	lea	rax,[0000000100006588]                                 ; [rip+00003D8C]
	mov	dword ptr [rax],1h
	jmp	100002A95h

l0000000100002807:
	lea	rax,[0000000100006578]                                 ; [rip+00003D6A]
	mov	dword ptr [rax],0h
	lea	rax,[00000001000065A0]                                 ; [rip+00003D85]
	jmp	100002831h

l000000010000281D:
	lea	rax,[00000001000065A0]                                 ; [rip+00003D7C]

l0000000100002824:
	mov	dword ptr [rax],1h
	lea	rax,[0000000100006578]                                 ; [rip+00003D47]

l0000000100002831:
	mov	dword ptr [rax],0h
	xor	ebx,ebx
	jmp	100002B5Bh

l000000010000283E:
	lea	rax,[00000001000065A4]                                 ; [rip+00003D5F]
	mov	dword ptr [rax],1h
	lea	rax,[0000000100006560]                                 ; [rip+00003D0E]
	mov	dword ptr [rax],0h
	lea	rax,[0000000100006558]                                 ; [rip+00003CF9]
	jmp	100002A9Ch

l0000000100002864:
	lea	rax,[0000000100006558]                                 ; [rip+00003CED]
	mov	dword ptr [rax],1h
	lea	rax,[0000000100006560]                                 ; [rip+00003CE8]

l0000000100002878:
	mov	dword ptr [rax],0h
	lea	rax,[00000001000065A4]                                 ; [rip+00003D1F]
	jmp	100002A9Ch

l000000010000288A:
	lea	rax,[0000000100006560]                                 ; [rip+00003CCF]
	mov	dword ptr [rax],1h
	lea	rax,[0000000100006558]                                 ; [rip+00003CBA]
	jmp	100002878h

l00000001000028A0:
	lea	rax,[00000001000065AC]                                 ; [rip+00003D05]
	mov	dword ptr [rax],1h
	lea	rax,[000000010000659C]                                 ; [rip+00003CE8]
	jmp	100002A9Ch

l00000001000028B9:
	lea	rdi,[0000000100005340]                                 ; [rip+00002A80]
	lea	rsi,[00000001000054CC]                                 ; [rip+00002C05]
	call	100004E08h
	test	al,al
	jz	1000028E3h

l00000001000028D0:
	and	r14d,0FFFFFBEDh
	add	r14d,410h
	jmp	100002B5Bh

l00000001000028E3:
	or	r14d,1h
	jmp	100002B5Bh

l00000001000028EC:
	lea	rdi,[0000000100005365]                                 ; [rip+00002A72]
	lea	rsi,[0000000100005410]                                 ; [rip+00002B16]
	mov	edx,1h
	call	100004EE6h
	jmp	100002B5Bh

l0000000100002909:
	lea	rdi,[0000000100005340]                                 ; [rip+00002A30]
	lea	rsi,[00000001000054CC]                                 ; [rip+00002BB5]
	call	100004E08h
	and	r14d,0EDh
	add	r14d,2h
	test	al,al
	jz	100002B5Bh

l000000010000292C:
	and	r14d,0FFFFFBEEh
	jmp	100002B5Bh

l0000000100002938:
	and	r14d,0FFFFFBECh
	add	r14d,10h
	jmp	100002B5Bh

l0000000100002948:
	mov	[00000001000065E2],1h                                  ; [rip+00003C93]
	jmp	100002B5Bh

l0000000100002954:
	or	r14d,20h

l0000000100002958:
	mov	[00000001000065D1],1h                                  ; [rip+00003C72]
	jmp	100002B5Bh

l0000000100002964:
	mov	[00000001000065C0],1h                                  ; [rip+00003C55]
	mov	[00000001000065E2],0h                                  ; [rip+00003C70]
	jmp	100002B5Bh

l0000000100002977:
	mov	[00000001000065E1],1h                                  ; [rip+00003C63]
	lea	rdi,[0000000100005340]                                 ; [rip+000029BB]
	lea	rsi,[00000001000054CC]                                 ; [rip+00002B40]
	call	100004E08h
	test	al,al
	jz	100002B5Bh

l0000000100002999:
	mov	[00000001000065D1],1h                                  ; [rip+00003C31]
	or	r14d,20h
	jmp	100002B5Bh

l00000001000029A9:
	lea	rdi,[0000000100005340]                                 ; [rip+00002990]
	lea	rsi,[00000001000054CC]                                 ; [rip+00002B15]
	call	100004E08h
	test	al,al
	jz	100002B5Bh

l00000001000029C4:
	lea	rax,[000000010000656C]                                 ; [rip+00003BA1]

l00000001000029CB:
	mov	dword ptr [rax],1h
	jmp	100002A22h

l00000001000029D3:
	lea	rax,[0000000100006570]                                 ; [rip+00003B96]

l00000001000029DA:
	mov	dword ptr [rax],1h
	jmp	100002B5Bh

l00000001000029E5:
	lea	rax,[0000000100006574]                                 ; [rip+00003B88]
	jmp	1000029DAh

l00000001000029EE:
	lea	rax,[00000001000065A8]                                 ; [rip+00003BB3]
	jmp	100002824h

l00000001000029FA:
	lea	rax,[0000000100006584]                                 ; [rip+00003B83]
	mov	dword ptr [rax],1h
	lea	rdi,[0000000100005340]                                 ; [rip+00002932]
	lea	rsi,[00000001000054CC]                                 ; [rip+00002AB7]
	call	100004E08h
	test	al,al
	jz	100002B5Bh

l0000000100002A22:
	lea	rax,[0000000100006578]                                 ; [rip+00003B4F]
	mov	dword ptr [rax],1h
	lea	rax,[00000001000065A8]                                 ; [rip+00003B72]
	jmp	100002831h

l0000000100002A3B:
	lea	rdi,[0000000100005340]                                 ; [rip+000028FE]
	lea	rsi,[00000001000054CC]                                 ; [rip+00002A83]
	call	100004E08h
	test	al,al
	jz	100002B4Ah

l0000000100002A56:
	lea	rax,[0000000100006590]                                 ; [rip+00003B33]
	jmp	1000029CBh

l0000000100002A62:
	lea	rax,[000000010000659C]                                 ; [rip+00003B33]
	mov	dword ptr [rax],1h
	lea	rax,[00000001000065AC]                                 ; [rip+00003B36]
	jmp	1000029DAh

l0000000100002A7B:
	lea	rax,[000000010000657C]                                 ; [rip+00003AFA]
	mov	dword ptr [rax],1h

l0000000100002A88:
	lea	rax,[0000000100006588]                                 ; [rip+00003AF9]
	mov	dword ptr [rax],0h

l0000000100002A95:
	lea	rax,[000000010000658C]                                 ; [rip+00003AF0]

l0000000100002A9C:
	mov	dword ptr [rax],0h
	jmp	100002B5Bh

l0000000100002AA7:
	mov	dword ptr [rbp-650h],1h
	jmp	100002B5Bh

l0000000100002AB6:
	mov	r15d,1h
	jmp	100002B5Bh

l0000000100002AC1:
	lea	rax,[0000000100006598]                                 ; [rip+00003AD0]
	jmp	1000029DAh

l0000000100002ACD:
	lea	rax,[0000000100006594]                                 ; [rip+00003AC0]
	jmp	1000029DAh

l0000000100002AD9:
	mov	dword ptr [rbp-644h],1h
	jmp	100002B5Bh

l0000000100002AE5:
	mov	dword ptr [rbp-648h],1h
	jmp	100002B5Bh

l0000000100002AF1:
	lea	rax,[000000010000657C]                                 ; [rip+00003A84]
	jmp	100002A9Ch

l0000000100002AFA:
	lea	rax,[000000010000657C]                                 ; [rip+00003A7B]
	mov	dword ptr [rax],0h
	lea	rax,[0000000100006588]                                 ; [rip+00003A7A]
	mov	dword ptr [rax],0h
	lea	rax,[000000010000658C]                                 ; [rip+00003A71]
	jmp	1000029DAh

l0000000100002B20:
	lea	rax,[000000010000657C]                                 ; [rip+00003A55]
	mov	dword ptr [rax],0h
	jmp	100002A88h

l0000000100002B32:
	lea	rax,[000000010000655C]                                 ; [rip+00003A23]
	jmp	1000029DAh

l0000000100002B3E:
	lea	rax,[00000001000065B0]                                 ; [rip+00003A6B]
	jmp	1000029DAh

l0000000100002B4A:
	lea	rax,[0000000100006568]                                 ; [rip+00003A17]
	jmp	1000029DAh

l0000000100002B56:
	call	10000488Bh

l0000000100002B5B:
	mov	eax,r12d

l0000000100002B5E:
	mov	r12d,eax
	lea	rdx,[000000010000536E]                                 ; [rip+00002806]
	mov	edi,[rbp-64Ch]
	mov	rsi,[rbp-658h]
	call	100004E62h
	cmp	eax,30h
	jg	1000027B1h

l0000000100002B83:
	cmp	eax,0FFh
	jnz	100002B56h

l0000000100002B88:
	mov	rax,[0000000100006048]                                 ; [rip+000034B9]
	movsxd	rax,dword ptr [rax]
	mov	[rbp-660h],rax
	mov	rcx,[rbp-658h]
	lea	rcx,[rcx+rax*8]
	mov	[rbp-658h],rcx
	mov	ecx,[rbp-64Ch]
	sub	ecx,eax
	mov	[rbp-664h],ecx
	lea	rdi,[0000000100005365]                                 ; [rip+000027A5]
	call	100004E56h
	test	rax,rax
	jz	100002CFEh

l0000000100002BCE:
	mov	edi,1h
	call	100004E92h
	test	eax,eax
	jnz	100002BF1h

l0000000100002BDC:
	lea	rdi,[0000000100005396]                                 ; [rip+000027B3]
	call	100004E56h
	test	rax,rax
	jz	100002CFEh

l0000000100002BF1:
	lea	rdi,[00000001000053A5]                                 ; [rip+000027AD]
	call	100004E56h
	lea	rdi,[rbp-434h]
	mov	rsi,rax
	call	100004F2Eh
	cmp	eax,1h
	jnz	100002CFEh

l0000000100002C15:
	lea	rdi,[00000001000053AA]                                 ; [rip+0000278E]
	lea	r13,[rbp-640h]
	mov	rsi,r13
	call	100004F34h
	lea	rcx,[0000000100006538]                                 ; [rip+00003906]
	mov	[rcx],rax
	lea	rdi,[00000001000053AD]                                 ; [rip+00002771]
	mov	rsi,r13
	call	100004F34h
	lea	rcx,[0000000100006528]                                 ; [rip+000038DD]
	mov	[rcx],rax
	lea	rdi,[00000001000053B0]                                 ; [rip+0000275B]
	mov	rsi,r13
	call	100004F34h
	lea	rcx,[0000000100006540]                                 ; [rip+000038DC]
	mov	[rcx],rax
	lea	rdi,[00000001000053B3]                                 ; [rip+00002745]
	mov	rsi,r13
	call	100004F34h
	lea	rcx,[0000000100006550]                                 ; [rip+000038D3]
	mov	[rcx],rax
	lea	rdi,[00000001000053B6]                                 ; [rip+0000272F]
	mov	rsi,r13
	call	100004F34h
	lea	rcx,[0000000100006530]                                 ; [rip+0000389A]
	mov	[rcx],rax
	test	rax,rax
	jnz	100002CBBh

l0000000100002C9E:
	lea	rdi,[00000001000053B9]                                 ; [rip+00002714]
	lea	rsi,[rbp-640h]
	call	100004F34h
	lea	rcx,[0000000100006530]                                 ; [rip+00003878]
	mov	[rcx],rax

l0000000100002CBB:
	lea	rcx,[0000000100006538]                                 ; [rip+00003876]
	cmp	qword ptr [rcx],0h
	jz	100002CFEh

l0000000100002CC8:
	lea	rcx,[0000000100006528]                                 ; [rip+00003859]
	cmp	qword ptr [rcx],0h
	jz	100002CFEh

l0000000100002CD5:
	test	rax,rax
	jz	100002CFEh

l0000000100002CDA:
	lea	rax,[0000000100006564]                                 ; [rip+00003883]
	mov	dword ptr [rax],1h
	jmp	100002D0Ah

l0000000100002CE9:
	mov	eax,1h
	lea	rdx,[0000000100002FA8]                                 ; [rip+000002B3]
	movsxd	rcx,dword ptr [rdx+rcx*4]
	add	rcx,rdx
	jmp	rcx

l0000000100002CFE:
	lea	rax,[0000000100006564]                                 ; [rip+0000385F]
	cmp	dword ptr [rax],0h
	jz	100002D4Ch

l0000000100002D0A:
	lea	rax,[0000000100006580]                                 ; [rip+0000386F]
	mov	dword ptr [rax],1h
	mov	edi,2h
	lea	r13,[000000010000319B]                                 ; [rip+00000478]
	mov	rsi,r13
	call	100004EF2h
	mov	edi,3h
	mov	rsi,r13
	call	100004EF2h
	lea	rdi,[00000001000053BC]                                 ; [rip+0000267D]
	call	100004E56h
	mov	rdi,rax
	call	10000328Eh

l0000000100002D4C:
	lea	rax,[0000000100006578]                                 ; [rip+00003825]
	mov	eax,[rax]
	lea	rcx,[0000000100006574]                                 ; [rip+00003818]
	mov	ecx,[rcx]
	mov	edx,ecx
	or	edx,eax
	jnz	100002D97h

l0000000100002D64:
	lea	rdx,[0000000100006598]                                 ; [rip+0000382D]
	mov	edx,[rdx]
	or	edx,[rbp-644h]
	jnz	100002D97h

l0000000100002D75:
	lea	rdx,[00000001000065AC]                                 ; [rip+00003830]
	mov	edx,[rdx]
	or	edx,r15d
	jnz	100002D97h

l0000000100002D83:
	mov	edx,r14d
	or	edx,8h
	lea	rsi,[0000000100006564]                                 ; [rip+000037D4]
	cmp	dword ptr [rsi],0h
	cmovz	r14d,edx

l0000000100002D97:
	test	eax,eax
	jz	100002DB2h

l0000000100002D9B:
	mov	r13d,r14d
	or	r13d,80h
	cmp	dword ptr [rbp-648h],0h
	cmovz	r13d,r14d

l0000000100002DB0:
	jmp	100002DFAh

l0000000100002DB2:
	mov	dl,[00000001000065C0]                                  ; [rip+00003808]
	cmp	dl,1h
	jz	100002DD5h

l0000000100002DBD:
	lea	rdx,[00000001000065AC]                                 ; [rip+000037E8]
	cmp	dword ptr [rdx],0h
	jnz	100002DD5h

l0000000100002DC9:
	test	ecx,ecx
	setz	cl
	movzx	r13d,cl
	or	r14d,r13d

l0000000100002DD5:
	mov	r13d,r14d
	or	r13d,80h
	cmp	dword ptr [rbp-648h],0h
	cmovz	r13d,r14d

l0000000100002DEA:
	test	eax,eax
	jnz	100002DFAh

l0000000100002DEE:
	lea	rax,[0000000100006598]                                 ; [rip+000037A3]
	cmp	dword ptr [rax],0h
	jz	100002E3Ah

l0000000100002DFA:
	test	r12d,r12d
	jz	100002E0Fh

l0000000100002DFF:
	lea	rax,[0000000100006548]                                 ; [rip+00003742]
	mov	qword ptr [rax],+2h
	jmp	100002E3Ah

l0000000100002E0F:
	lea	rdi,[rbp-34h]
	lea	r14,[0000000100006548]                                 ; [rip+0000372E]
	mov	rsi,r14
	call	100004E50h
	mov	rax,[r14]
	mov	rcx,rax
	sar	rcx,3Fh
	shr	rcx,37h
	add	rcx,rax
	sar	rcx,9h
	mov	[r14],rcx

l0000000100002E3A:
	cmp	dword ptr [rbp-650h],0h
	jz	100002EB7h

l0000000100002E43:
	test	r15d,r15d
	jz	100002E54h

l0000000100002E48:
	lea	rax,[0000000100001830]                                 ; [rip-0000161F]
	jmp	100002F1Dh

l0000000100002E54:
	cmp	dword ptr [rbp-644h],0h
	jnz	100002E69h

l0000000100002E5D:
	lea	rax,[00000001000017C6]                                 ; [rip-0000169E]
	jmp	100002F1Dh

l0000000100002E69:
	lea	rax,[0000000100006558]                                 ; [rip+000036E8]
	cmp	dword ptr [rax],0h
	jz	100002E81h

l0000000100002E75:
	lea	rax,[00000001000018C8]                                 ; [rip-000015B4]
	jmp	100002F1Dh

l0000000100002E81:
	lea	rax,[00000001000065A4]                                 ; [rip+0000371C]
	cmp	dword ptr [rax],0h
	jz	100002E99h

l0000000100002E8D:
	lea	rax,[0000000100001873]                                 ; [rip-00001621]
	jmp	100002F1Dh

l0000000100002E99:
	lea	rax,[0000000100006560]                                 ; [rip+000036C0]
	cmp	dword ptr [rax],0h
	jz	100002EAEh

l0000000100002EA5:
	lea	rax,[00000001000017DB]                                 ; [rip-000016D1]
	jmp	100002F1Dh

l0000000100002EAE:
	lea	rax,[000000010000191D]                                 ; [rip-00001598]
	jmp	100002F1Dh

l0000000100002EB7:
	test	r15d,r15d
	jz	100002EC5h

l0000000100002EBC:
	lea	rax,[00000001000019BE]                                 ; [rip-00001505]
	jmp	100002F1Dh

l0000000100002EC5:
	cmp	dword ptr [rbp-644h],0h
	jnz	100002ED7h

l0000000100002ECE:
	lea	rax,[00000001000017B4]                                 ; [rip-00001721]
	jmp	100002F1Dh

l0000000100002ED7:
	lea	rax,[0000000100006558]                                 ; [rip+0000367A]
	cmp	dword ptr [rax],0h
	jz	100002EECh

l0000000100002EE3:
	lea	rax,[0000000100001A44]                                 ; [rip-000014A6]
	jmp	100002F1Dh

l0000000100002EEC:
	lea	rax,[00000001000065A4]                                 ; [rip+000036B1]
	cmp	dword ptr [rax],0h
	jz	100002F01h

l0000000100002EF8:
	lea	rax,[00000001000019F8]                                 ; [rip-00001507]
	jmp	100002F1Dh

l0000000100002F01:
	lea	rax,[0000000100006560]                                 ; [rip+00003658]
	cmp	dword ptr [rax],0h
	jz	100002F16h

l0000000100002F0D:
	lea	rax,[0000000100001972]                                 ; [rip-000015A2]
	jmp	100002F1Dh

l0000000100002F16:
	lea	rax,[0000000100001A90]                                 ; [rip-0000148D]

l0000000100002F1D:
	mov	[00000001000065C8],rax                                 ; [rip+000036A4]
	test	ebx,ebx
	jz	100002F31h

l0000000100002F28:
	lea	rax,[00000001000030C8]                                 ; [rip+00000199]
	jmp	100002F62h

l0000000100002F31:
	lea	rax,[0000000100006578]                                 ; [rip+00003640]
	cmp	dword ptr [rax],0h
	jz	100002F46h

l0000000100002F3D:
	lea	rax,[0000000100003BF3]                                 ; [rip+00000CAF]
	jmp	100002F62h

l0000000100002F46:
	lea	rax,[00000001000065A8]                                 ; [rip+0000365B]
	cmp	dword ptr [rax],0h
	jz	100002F5Bh

l0000000100002F52:
	lea	rax,[00000001000036BE]                                 ; [rip+00000765]
	jmp	100002F62h

l0000000100002F5B:
	lea	rax,[00000001000037EC]                                 ; [rip+0000088A]

l0000000100002F62:
	mov	[00000001000065D8],rax                                 ; [rip+0000366F]
	mov	rax,[rbp-660h]
	cmp	eax,[rbp-64Ch]
	jz	100002F87h

l0000000100002F78:
	mov	edi,[rbp-664h]
	mov	rsi,[rbp-658h]
	jmp	100002F93h

l0000000100002F87:
	mov	edi,1h
	lea	rsi,[00000001000062C0]                                 ; [rip+0000332D]

l0000000100002F93:
	mov	edx,r13d
	call	1000023B0h
	movzx	edi,[00000001000065D0]                               ; [rip+0000362E]
	call	100004E14h
	nop
l0000000100002FA8	dd	0xFFFFF81C
l0000000100002FAC	dd	0xFFFFFBAE
l0000000100002FB0	dd	0xFFFFFBAE
l0000000100002FB4	dd	0xFFFFFBAE
l0000000100002FB8	dd	0xFFFFFBAE
l0000000100002FBC	dd	0xFFFFFBAE
l0000000100002FC0	dd	0xFFFFFBAE
l0000000100002FC4	dd	0xFFFFFBAE
l0000000100002FC8	dd	0xFFFFFBAE
l0000000100002FCC	dd	0xFFFFFBAE
l0000000100002FD0	dd	0xFFFFFBAE
l0000000100002FD4	dd	0xFFFFFBAE
l0000000100002FD8	dd	0xFFFFFBAE
l0000000100002FDC	dd	0xFFFFFBAE
l0000000100002FE0	dd	0xFFFFFBAE
l0000000100002FE4	dd	0xFFFFFB96
l0000000100002FE8	dd	0xFFFFF9B0
l0000000100002FEC	dd	0xFFFFF840
l0000000100002FF0	dd	0xFFFFF85F
l0000000100002FF4	dd	0xFFFFFBAE
l0000000100002FF8	dd	0xFFFFFBAE
l0000000100002FFC	dd	0xFFFFF8F8
l0000000100003000	dd	0xFFFFF944
l0000000100003004	dd	0xFFFFF911
l0000000100003008	dd	0xFFFFFBAE
l000000010000300C	dd	0xFFFFFBAE
l0000000100003010	dd	0xFFFFFBAE
l0000000100003014	dd	0xFFFFF961
l0000000100003018	dd	0xFFFFFBAE
l000000010000301C	dd	0xFFFFFBAE
l0000000100003020	dd	0xFFFFFBA2
l0000000100003024	dd	0xFFFFF990
l0000000100003028	dd	0xFFFFFBAE
l000000010000302C	dd	0xFFFFF9A0
l0000000100003030	dd	0xFFFFFB0E
l0000000100003034	dd	0xFFFFFB25
l0000000100003038	dd	0xFFFFF8E2
l000000010000303C	dd	0xFFFFFBAE
l0000000100003040	dd	0xFFFFFB3D
l0000000100003044	dd	0xFFFFFBAE
l0000000100003048	dd	0xFFFFFBAE
l000000010000304C	dd	0xFFFFFBAE
l0000000100003050	dd	0xFFFFFBAE
l0000000100003054	dd	0xFFFFFBAE
l0000000100003058	dd	0xFFFFFBAE
l000000010000305C	dd	0xFFFFFBAE
l0000000100003060	dd	0xFFFFFBAE
l0000000100003064	dd	0xFFFFFBAE
l0000000100003068	dd	0xFFFFF9AC
l000000010000306C	dd	0xFFFFFB52
l0000000100003070	dd	0xFFFFF896
l0000000100003074	dd	0xFFFFF9BC
l0000000100003078	dd	0xFFFFFB8A
l000000010000307C	dd	0xFFFFF9CF
l0000000100003080	dd	0xFFFFFA01
l0000000100003084	dd	0xFFFFFA2B
l0000000100003088	dd	0xFFFFFA3D
l000000010000308C	dd	0xFFFFFBAE
l0000000100003090	dd	0xFFFFFBB6
l0000000100003094	dd	0xFFFFFA7A
l0000000100003098	dd	0xFFFFFA46
l000000010000309C	dd	0xFFFFFA52
l00000001000030A0	dd	0xFFFFFA93
l00000001000030A4	dd	0xFFFFFABA
l00000001000030A8	dd	0xFFFFFAD3
l00000001000030AC	dd	0xFFFFFAFF
l00000001000030B0	dd	0xFFFFFB19
l00000001000030B4	dd	0xFFFFFB31
l00000001000030B8	dd	0xFFFFF8BC
l00000001000030BC	dd	0xFFFFFB49
l00000001000030C0	dd	0xFFFFFB78
l00000001000030C4	dd	0xFFFFF875
00000001000030C8                         55 48 89 E5 41 56 53 48         UH..AVSH
00000001000030D0 89 FB 48 8D 3D 67 22 00 00 48 8D 35 EC 23 00 00 ..H.=g"..H.5.#..
00000001000030E0 E8 23 1D 00 00 84 C0 74 4C 48 8B 03 66 83 78 56 .#.....tLH..f.xV
00000001000030F0 00 74 42 48 8D 05 7E 34 00 00 83 38 00 75 0C 48 .tBH..~4...8.u.H
0000000100003100 8D 05 92 34 00 00 83 38 00 74 2A 48 8B 43 08 31 ...4...8.t*H.C.1
0000000100003110 D2 48 8D 0D 30 34 00 00 48 F7 31 48 85 D2 0F 95 .H..04..H.1H....
0000000100003120 C1 0F B6 F1 48 01 C6 48 8D 3D A7 23 00 00 30 C0 ....H..H.=.#..0.
0000000100003130 E8 99 1D 00 00 4C 8B 33 EB 23 49 83 7E 18 01 74 .....L.3.#I.~..t
0000000100003140 18 8B 73 2C 8B 53 1C 4C 89 F7 E8 5A 04 00 00 BF ..s,.S.L...Z....
0000000100003150 0A 00 00 00 E8 7B 1D 00 00 4D 8B 76 10 4D 85 F6 .....{...M.v.M..
0000000100003160 75 D8 5B 41 5E 5D C3 55 48 89 E5 48 83 EC 10 40 u.[A^].UH..H...@
0000000100003170 88 7D FF BF 01 00 00 00 48 8D 75 FF BA 01 00 00 .}......H.u.....
0000000100003180 00 E8 DE 1D 00 00 31 C0 48 83 C4 10 5D C3 55 48 ......1.H...].UH
0000000100003190 89 E5 E8 3D 1D 00 00 31 C0 5D C3 55 48 89 E5 41 ...=...1.].UH..A
00000001000031A0 56 53 89 FB 85 DB 48 8D 05 E1 FF FF FF 4C 8D 35 VS....H......L.5
00000001000031B0 B3 FF FF FF 4C 0F 44 F0 48 8D 05 71 33 00 00 48 ....L.D.H..q3..H
00000001000031C0 8B 38 BE 01 00 00 00 4C 89 F2 E8 77 1D 00 00 48 .8.....L...w...H
00000001000031D0 8D 05 6A 33 00 00 48 8B 38 BE 01 00 00 00 4C 89 ..j3..H.8.....L.
00000001000031E0 F2 E8 60 1D 00 00 31 F6 89 DF E8 03 1D 00 00 E8 ..`...1.........
00000001000031F0 74 1C 00 00 89 C7 89 DE 5B 41 5E 5D E9 97 1C 00 t.......[A^]....
0000000100003200 00                                              .               

;; fn0000000100003201: 0000000100003201
;;   Called from:
;;     00000001000036A8 (in fn00000001000035A9)
;;     0000000100004144 (in fn0000000100003AA8)
fn0000000100003201 proc
	push	rbp
	mov	rbp,rsp
	mov	eax,edi
	and	eax,0F000h
	lea	rcx,[000000010000659C]                                 ; [rip+00003389]
	cmp	dword ptr [rcx],0h
	jz	100003230h

l0000000100003218:
	cmp	eax,4000h
	jnz	10000328Ah

l000000010000321F:
	mov	edi,2Fh

l0000000100003224:
	call	100004ED4h
	mov	eax,1h
	pop	rbp
	ret

l0000000100003230:
	cmp	eax,3FFFh
	jg	100003245h

l0000000100003237:
	cmp	eax,1000h
	jnz	100003253h

l000000010000323E:
	mov	edi,7Ch
	jmp	100003224h

l0000000100003245:
	cmp	eax,9FFFh
	jg	100003260h

l000000010000324C:
	cmp	eax,4000h
	jz	10000321Fh

l0000000100003253:
	test	dil,49h
	jz	10000328Ah

l0000000100003259:
	mov	edi,2Ah
	jmp	100003224h

l0000000100003260:
	cmp	eax,0A000h
	jz	10000327Ch

l0000000100003267:
	cmp	eax,0C000h
	jz	100003283h

l000000010000326E:
	cmp	eax,0E000h
	jnz	100003253h

l0000000100003275:
	mov	edi,25h
	jmp	100003224h

l000000010000327C:
	mov	edi,40h
	jmp	100003224h

l0000000100003283:
	mov	edi,3Dh
	jmp	100003224h

l000000010000328A:
	xor	eax,eax
	pop	rbp
	ret

;; fn000000010000328E: 000000010000328E
;;   Called from:
;;     0000000100002D47 (in fn00000001000026A0)
fn000000010000328E proc
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbx
	sub	rsp,28h
	test	rdi,rdi
	lea	rax,[0000000100005410]                                 ; [rip+00002167]
	cmovnz	rax,rdi

l00000001000032AD:
	mov	rdi,rax
	lea	rax,[rax+1h]
	mov	[rbp-40h],rax
	call	100004F22h
	mov	[rbp-44h],eax
	xor	ebx,ebx
	lea	r14,[00000001000065F8]                                 ; [rip+0000332F]
	xor	r15d,r15d
	mov	[rbp-38h],r15

l00000001000032D0:
	mov	dword ptr [r14],0h
	cmp	[rbp-44h],r15d
	jg	1000032F6h

l00000001000032DD:
	mov	rax,[rbp-38h]
	lea	rax,[rax+rax+1h]
	lea	rcx,[0000000100005411]                                 ; [rip+00002124]
	mov	r12b,[r15+rcx]
	add	rax,rcx
	jmp	1000032FEh

l00000001000032F6:
	mov	rax,[rbp-40h]
	mov	r12b,[rax-1h]

l00000001000032FE:
	mov	[rbp-2Ah],r12b
	mov	al,[rax]
	mov	[rbp-29h],al
	xor	r13d,r13d
	jmp	100003318h

l000000010000330C:
	lea	rax,[r13+1h]
	mov	r12b,[rbp+r13-29h]
	mov	r13,rax

l0000000100003318:
	mov	al,r12b
	add	al,0D0h
	cmp	al,7h
	ja	100003353h

l0000000100003321:
	movsx	eax,r12b
	add	eax,0D0h
	mov	[r14+r13*4-8h],eax
	test	bx,bx
	jz	10000333Bh

l0000000100003332:
	mov	bx,1h
	jmp	1000033C0h

l000000010000333B:
	mov	rax,[0000000100006038]                                 ; [rip+00002CF6]
	mov	rsi,[rax]
	lea	rdi,[0000000100005428]                                 ; [rip+000020DC]
	call	100004E26h
	jmp	100003332h

l0000000100003353:
	mov	al,r12b
	add	al,9Fh
	cmp	al,7h
	ja	10000336Ah

l000000010000335C:
	movsx	eax,r12b
	add	eax,9Fh
	mov	[r14+r13*4-8h],eax
	jmp	1000033C0h

l000000010000336A:
	mov	al,r12b
	add	al,0BFh
	cmp	al,7h
	ja	100003388h

l0000000100003373:
	movsx	eax,r12b
	add	eax,0BFh
	mov	[r14+r13*4-8h],eax
	mov	dword ptr [r14],1h
	jmp	1000033C0h

l0000000100003388:
	cmp	r12b,78h
	setz	al
	movzx	edi,al
	call	100004DC6h
	test	eax,eax
	jnz	1000033B7h

l000000010000339B:
	mov	rax,[0000000100006038]                                 ; [rip+00002C96]
	mov	rdi,[rax]
	movsx	edx,r12b
	xor	al,al
	lea	rsi,[0000000100005477]                                 ; [rip+000020C5]
	call	100004E20h

l00000001000033B7:
	mov	dword ptr [r14+r13*4-8h],0FFFFFFFFh

l00000001000033C0:
	cmp	r13,1h
	jnz	10000330Ch

l00000001000033CA:
	add	qword ptr [rbp-40h],2h
	add	r14,0Ch
	inc	qword ptr [rbp-38h]
	add	r15,2h
	cmp	r15,16h
	jnz	1000032D0h

l00000001000033E5:
	add	rsp,28h
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	ret

;; fn00000001000033F4: 00000001000033F4
;;   Called from:
;;     00000001000034F3 (in fn00000001000034AC)
fn00000001000033F4 proc
	push	rbp
	mov	rbp,rsp
	push	rbx
	sub	rsp,8h
	mov	eax,edi
	lea	rbx,[rax+rax*2]
	lea	rax,[00000001000065F0]                                 ; [rip+000031E6]
	cmp	dword ptr [rax+rbx*4+8h],0h
	jz	10000342Ch

l0000000100003411:
	lea	rax,[0000000100006550]                                 ; [rip+00003138]
	mov	rdi,[rax]
	mov	esi,1h
	lea	rdx,[000000010000318E]                                 ; [rip-00000299]
	call	100004F46h

l000000010000342C:
	lea	rax,[00000001000065F0]                                 ; [rip+000031BD]
	mov	edx,[rax+rbx*4]
	cmp	edx,0FFh
	jz	100003465h

l000000010000343B:
	lea	rax,[0000000100006538]                                 ; [rip+000030F6]
	mov	rdi,[rax]
	xor	esi,esi
	call	100004F3Ah
	test	rax,rax
	jz	100003465h

l0000000100003451:
	mov	esi,1h
	lea	rdx,[000000010000318E]                                 ; [rip-000002CF]
	mov	rdi,rax
	call	100004F46h

l0000000100003465:
	lea	rax,[00000001000065F0]                                 ; [rip+00003184]
	mov	edx,[rax+rbx*4+4h]
	cmp	edx,0FFh
	jz	10000348Bh

l0000000100003475:
	lea	rax,[0000000100006528]                                 ; [rip+000030AC]
	mov	rdi,[rax]
	xor	esi,esi
	call	100004F3Ah
	test	rax,rax
	jnz	100003492h

l000000010000348B:
	add	rsp,8h
	pop	rbx
	pop	rbp
	ret

l0000000100003492:
	mov	esi,1h
	lea	rdx,[000000010000318E]                                 ; [rip-00000310]
	mov	rdi,rax
	add	rsp,8h
	pop	rbx
	pop	rbp
	jmp	100004F46h

;; fn00000001000034AC: 00000001000034AC
;;   Called from:
;;     0000000100003634 (in fn00000001000035A9)
;;     00000001000040CB (in fn0000000100003AA8)
fn00000001000034AC proc
	push	rbp
	mov	rbp,rsp
	mov	eax,edi
	and	eax,0F000h
	cmp	eax,0BFFFh
	jg	100003518h

l00000001000034BE:
	cmp	eax,9FFFh
	jg	10000350Ah

l00000001000034C5:
	cmp	eax,5FFFh
	jg	10000352Dh

l00000001000034CC:
	cmp	eax,1000h
	jz	100003526h

l00000001000034D3:
	cmp	eax,2000h
	jz	10000353Bh

l00000001000034DA:
	cmp	eax,4000h
	jnz	100003542h

l00000001000034E1:
	test	dil,2h
	jz	100003506h

l00000001000034E7:
	mov	eax,edi
	test	ah,2h
	jz	1000034FFh

l00000001000034EE:
	mov	edi,9h

l00000001000034F3:
	call	1000033F4h
	mov	eax,1h
	pop	rbp
	ret

l00000001000034FF:
	mov	edi,0Ah
	jmp	1000034F3h

l0000000100003506:
	xor	edi,edi
	jmp	1000034F3h

l000000010000350A:
	cmp	eax,0A000h
	jnz	100003542h

l0000000100003511:
	mov	edi,1h
	jmp	1000034F3h

l0000000100003518:
	cmp	eax,0C000h
	jnz	100003542h

l000000010000351F:
	mov	edi,2h
	jmp	1000034F3h

l0000000100003526:
	mov	edi,3h
	jmp	1000034F3h

l000000010000352D:
	cmp	eax,6000h
	jnz	100003542h

l0000000100003534:
	mov	edi,5h
	jmp	1000034F3h

l000000010000353B:
	mov	edi,6h
	jmp	1000034F3h

l0000000100003542:
	test	dil,49h
	jz	10000356Bh

l0000000100003548:
	mov	eax,edi
	test	ah,8h
	jz	100003556h

l000000010000354F:
	mov	edi,7h
	jmp	1000034F3h

l0000000100003556:
	mov	eax,edi
	test	ah,4h
	jz	100003564h

l000000010000355D:
	mov	edi,8h
	jmp	1000034F3h

l0000000100003564:
	mov	edi,4h
	jmp	1000034F3h

l000000010000356B:
	xor	eax,eax
	pop	rbp
	ret

;; fn000000010000356F: 000000010000356F
;;   Called from:
;;     0000000100003646 (in fn00000001000035A9)
;;     00000001000040DD (in fn0000000100003AA8)
;;     000000010000424D (in fn0000000100003AA8)
;;     0000000100004301 (in fn0000000100003AA8)
fn000000010000356F proc
	push	rbp
	mov	rbp,rsp
	lea	rax,[0000000100006588]                                 ; [rip+0000300E]
	cmp	dword ptr [rax],0h
	jnz	10000359Dh

l000000010000357F:
	lea	rax,[000000010000658C]                                 ; [rip+00003006]
	cmp	dword ptr [rax],0h
	jnz	10000359Dh

l000000010000358B:
	lea	rax,[000000010000657C]                                 ; [rip+00002FEA]
	cmp	dword ptr [rax],0h
	jnz	1000035A3h

l0000000100003597:
	pop	rbp
	jmp	100004715h

l000000010000359D:
	pop	rbp
	jmp	1000048AFh

l00000001000035A3:
	pop	rbp
	jmp	100004C44h

;; fn00000001000035A9: 00000001000035A9
fn00000001000035A9 proc
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r12
	push	rbx
	lea	rax,[0000000100006574]                                 ; [rip+00002FB9]
	cmp	dword ptr [rax],0h
	mov	rbx,[rdi+60h]
	mov	r14,rdx
	mov	r15,rdi
	jnz	1000035CEh

l00000001000035CA:
	xor	eax,eax
	jmp	1000035E0h

l00000001000035CE:
	mov	rdx,[rbx+8h]
	lea	rdi,[00000001000054AA]                                 ; [rip+00001ED1]
	xor	al,al
	call	100004ECEh

l00000001000035E0:
	mov	r12d,eax
	lea	rax,[0000000100006598]                                 ; [rip+00002FAE]
	cmp	dword ptr [rax],0h
	jz	10000361Fh

l00000001000035EF:
	mov	rax,[rbx+68h]
	lea	rcx,[0000000100006548]                                 ; [rip+00002F4E]
	cqo
	idiv	qword ptr [rcx]
	test	rdx,rdx
	setnz	cl
	movzx	edx,cl
	add	rdx,rax
	mov	esi,r14d
	lea	rdi,[00000001000054B1]                                 ; [rip+00001E9C]
	xor	al,al
	call	100004ECEh
	add	r12d,eax

l000000010000361F:
	lea	rax,[0000000100006564]                                 ; [rip+00002F3E]
	cmp	dword ptr [rax],0h
	jnz	100003630h

l000000010000362B:
	xor	r14b,r14b
	jmp	10000363Fh

l0000000100003630:
	movzx	edi,word ptr [rbx+4h]
	call	1000034ACh
	test	eax,eax
	setnz	r14b

l000000010000363F:
	mov	rdi,r15
	add	rdi,68h
	call	10000356Fh
	mov	r15d,eax
	lea	rax,[0000000100006564]                                 ; [rip+00002F0F]
	cmp	dword ptr [rax],0h
	jz	100003695h

l000000010000365A:
	test	r14b,1h
	jz	100003695h

l0000000100003660:
	lea	rax,[0000000100006530]                                 ; [rip+00002EC9]
	mov	rdi,[rax]
	lea	r14,[000000010000318E]                                 ; [rip-000004E3]
	mov	esi,1h
	mov	rdx,r14
	call	100004F46h
	lea	rax,[0000000100006540]                                 ; [rip+00002EBB]
	mov	rdi,[rax]
	mov	esi,1h
	mov	rdx,r14
	call	100004F46h

l0000000100003695:
	add	r15d,r12d
	lea	rax,[00000001000065AC]                                 ; [rip+00002F0D]
	cmp	dword ptr [rax],0h
	jz	1000036B2h

l00000001000036A4:
	movzx	edi,word ptr [rbx+4h]
	call	100003201h
	add	eax,r15d
	jmp	1000036B5h

l00000001000036B2:
	mov	eax,r15d

l00000001000036B5:
	pop	rbx
	pop	r12
	pop	r14
	pop	r15
	pop	rbp
	ret
00000001000036BE                                           55 48               UH
00000001000036C0 89 E5 41 57 41 56 41 55 41 54 53 48 83 EC 08 48 ..AWAVAUATSH...H
00000001000036D0 8B 1F 48 85 DB 0F 84 84 00 00 00 49 89 FE 45 31 ..H........I..E1
00000001000036E0 FF 4C 8D 25 CF 1D 00 00 48 83 7B 18 01 74 62 4D .L.%....H.{..tbM
00000001000036F0 63 EF 48 8D 7B 68 E8 27 18 00 00 49 01 C5 48 83 c.H.{h.'...I..H.
0000000100003700 7B 10 00 0F 95 C0 0F B6 C0 49 8D 44 45 00 48 8D {........I.DE.H.
0000000100003710 0D 9B 2B 00 00 8B 09 48 39 C8 72 0D BF 0A 00 00 ..+....H9.r.....
0000000100003720 00 E8 AE 17 00 00 45 31 FF 41 8B 76 2C 41 8B 56 ......E1.A.v,A.V
0000000100003730 1C 48 89 DF E8 70 FE FF FF 41 01 C7 48 83 7B 10 .H...p...A..H.{.
0000000100003740 00 74 0E 30 C0 4C 89 E7 E8 81 17 00 00 41 83 C7 .t.0.L.......A..
0000000100003750 02 48 8B 5B 10 48 85 DB 75 8E 45 85 FF 75 0F 48 .H.[.H..u.E..u.H
0000000100003760 83 C4 08 5B 41 5C 41 5D 41 5E 41 5F 5D C3 BF 0A ...[A\A]A^A_]...
0000000100003770 00 00 00 48 83 C4 08 5B 41 5C 41 5D 41 5E 41 5F ...H...[A\A]A^A_
0000000100003780 5D E9 4E 17 00 00                               ].N...          

;; fn0000000100003786: 0000000100003786
;;   Called from:
;;     0000000100004054 (in fn0000000100003AA8)
;;     000000010000431D (in fn0000000100003AA8)
fn0000000100003786 proc
	push	rbp
	mov	rbp,rsp
	push	rbx
	sub	rsp,8h
	lea	rax,[0000000100006570]                                 ; [rip+00002DDA]
	cmp	dword ptr [rax],0h
	mov	rdx,rsi
	jz	1000037D5h

l000000010000379E:
	lea	rbx,[rbp-0Dh]
	mov	esi,5h
	lea	rcx,[0000000100005410]                                 ; [rip+00001C62]
	mov	r8d,20h
	mov	r9d,7h
	mov	rdi,rbx
	call	100004E86h
	lea	rdi,[00000001000054BA]                                 ; [rip+00001CF1]
	xor	al,al
	mov	rsi,rbx
	call	100004ECEh
	jmp	1000037E5h

l00000001000037D5:
	mov	esi,edi
	lea	rdi,[00000001000054BF]                                 ; [rip+00001CE1]
	xor	al,al
	call	100004ECEh

l00000001000037E5:
	add	rsp,8h
	pop	rbx
	pop	rbp
	ret
00000001000037EC                                     55 48 89 E5             UH..
00000001000037F0 41 57 41 56 41 55 41 54 53 48 83 EC 38 48 8D 05 AWAVAUATSH..8H..
0000000100003800 7C 2D 00 00 83 38 00 B8 08 00 00 00 B9 01 00 00 |-...8..........
0000000100003810 00 0F 44 C8 89 4D D0 8B 47 14 3B 05 B4 2A 00 00 ..D..M..G.;..*..
0000000100003820 48 89 7D B0 7E 3B 89 05 A8 2A 00 00 48 89 F8 48 H.}.~;...*..H..H
0000000100003830 63 70 14 48 8B 3D 3E 2E 00 00 48 C1 E6 03 E8 9D cp.H.=>...H.....
0000000100003840 16 00 00 48 89 05 2E 2E 00 00 48 85 C0 75 12 31 ...H......H..u.1
0000000100003850 FF 30 C0 E8 FA 16 00 00 48 8B 7D B0 E8 67 F8 FF .0......H.}..g..
0000000100003860 FF 48 8B 45 B0 48 8B 00 48 85 C0 0F 84 15 02 00 .H.E.H..H.......
0000000100003870 00 31 C9 48 89 4D C0 48 83 78 18 01 74 18 48 8B .1.H.M.H.x..t.H.
0000000100003880 4D C0 48 63 C9 48 8B 15 EC 2D 00 00 48 89 04 CA M.Hc.H...-..H...
0000000100003890 FF C1 48 89 4D C0 48 8B 40 10 48 85 C0 75 D8 48 ..H.M.H.@.H..u.H
00000001000038A0 8D 05 CE 2C 00 00 83 38 00 48 8B 45 B0 8B 40 18 ...,...8.H.E..@.
00000001000038B0 74 0B 48 8B 4D B0 8B 49 2C 8D 44 08 01 48 8D 0D t.H.M..I,.D..H..
00000001000038C0 D4 2C 00 00 8B 09 85 C9 74 0B 48 8B 55 B0 8B 52 .,......t.H.U..R
00000001000038D0 1C 8D 44 10 01 8B 55 D0 01 D0 48 8D 35 CB 2C 00 ..D...U...H.5.,.
00000001000038E0 00 83 3E 00 40 0F 95 C6 40 0F B6 F6 01 C6 F7 DA ..>.@...@.......
00000001000038F0 89 55 CC 21 D6 89 75 BC 8D 14 36 48 8D 05 AE 29 .U.!..u...6H...)
0000000100003900 00 00 8B 00 39 D0 0F 8C 85 01 00 00 99 F7 7D BC ....9.........}.
0000000100003910 89 C6 89 75 D4 48 8B 45 C0 99 F7 FE 89 45 A8 89 ...u.H.E.....E..
0000000100003920 D0 89 45 A4 85 C0 0F 95 45 AC 48 8B 45 B0 48 8B ..E.....E.H.E.H.
0000000100003930 00 66 83 78 56 00 74 43 85 C9 0F 95 C0 48 8D 0D .f.xV.tC.....H..
0000000100003940 34 2C 00 00 83 39 00 75 04 3C 01 75 2E 48 8B 45 4,...9.u.<.u.H.E
0000000100003950 B0 48 8B 40 08 31 D2 48 8D 0D EA 2B 00 00 48 F7 .H.@.1.H...+..H.
0000000100003960 31 48 85 D2 0F 95 C1 0F B6 F1 48 01 C6 48 8D 3D 1H........H..H.=
0000000100003970 61 1B 00 00 30 C0 E8 53 15 00 00 0F B6 45 AC 03 a...0..S.....E..
0000000100003980 45 A8 89 45 AC 85 C0 0F 8E EA 00 00 00 83 7D A4 E..E..........}.
0000000100003990 00 0F 95 C0 0F B6 C0 03 45 A8 89 45 A4 31 C0 89 ........E..E.1..
00000001000039A0 C3 C7 45 A8 00 00 00 00 48 8D 05 F1 2B 00 00 83 ..E.....H...+...
00000001000039B0 38 00 0F 44 5D A8 44 8B 75 BC 45 31 FF 45 31 E4 8..D].D.u.E1.E1.
00000001000039C0 E9 8D 00 00 00 48 63 DB 48 8B 05 A9 2C 00 00 48 .....Hc.H...,..H
00000001000039D0 8B 3C D8 48 8B 45 B0 8B 70 2C 8B 50 1C E8 C7 FB .<.H.E..p,.P....
00000001000039E0 FF FF 48 8D 0D B7 2B 00 00 83 39 00 B9 01 00 00 ..H...+...9.....
00000001000039F0 00 0F 44 4D AC 01 CB 48 8B 4D C0 39 CB 7D 5D 41 ..DM...H.M.9.}]A
0000000100003A00 FF C7 44 01 E0 41 89 C4 EB 33 48 8D 05 8F 2B 00 ..D..A...3H...+.
0000000100003A10 00 83 38 00 74 06 44 3B 7D D4 7D 32 48 8D 05 5D ..8.t.D;}.}2H..]
0000000100003A20 2B 00 00 83 38 00 BF 20 00 00 00 41 BC 09 00 00 +...8.. ...A....
0000000100003A30 00 41 0F 44 FC E8 9A 14 00 00 45 89 EC 44 8B 6D .A.D......E..D.m
0000000100003A40 D0 47 8D 2C 2C 44 23 6D CC 45 39 F5 7E BC 44 03 .G.,,D#m.E9.~.D.
0000000100003A50 75 BC 44 3B 7D D4 0F 8C 69 FF FF FF BF 0A 00 00 u.D;}...i.......
0000000100003A60 00 E8 6E 14 00 00 8B 45 A8 FF C0 89 45 A8 39 45 ..n....E....E.9E
0000000100003A70 A4 0F 85 31 FF FF FF 48 83 C4 38 5B 41 5C 41 5D ...1...H..8[A\A]
0000000100003A80 41 5E 41 5F 5D C3 31 C0 48 89 45 C0 E9 0E FE FF A^A_].1.H.E.....
0000000100003A90 FF 48 8B 7D B0 48 83 C4 38 5B 41 5C 41 5D 41 5E .H.}.H..8[A\A]A^
0000000100003AA0 41 5F 5D E9 20 F6 FF FF                         A_]. ...        

;; fn0000000100003AA8: 0000000100003AA8
;;   Called from:
;;     00000001000040AF (in fn0000000100003AA8)
fn0000000100003AA8 proc
	push	rbp
	mov	rbp,rsp
	push	r14
	push	rbx
	sub	rsp,60h
	mov	rax,[0000000100006030]                                 ; [rip+00002576]
	mov	rax,[rax]
	mov	[rbp-18h],rax
	mov	[rbp-70h],rdi
	cmp	[00000001000062D8],0h                                  ; [rip+0000280C]
	jns	100003AE7h

l0000000100003ACE:
	mov	edi,39h
	call	100004EC8h
	cmp	byte ptr [rax],64h
	setz	al
	movzx	eax,al
	mov	[00000001000062D8],eax                                 ; [rip+000027F1]

l0000000100003AE7:
	cmp	[0000000100006680],0h                                  ; [rip+00002B91]
	jnz	100003AFFh

l0000000100003AF1:
	xor	edi,edi
	call	100004F40h
	mov	[0000000100006680],rax                                 ; [rip+00002B81]

l0000000100003AFF:
	lea	rax,[0000000100006594]                                 ; [rip+00002A8E]
	cmp	dword ptr [rax],0h
	jz	100003B22h

l0000000100003B0B:
	cmp	[00000001000062D8],0h                                  ; [rip+000027C6]
	lea	rax,[00000001000054ED]                                 ; [rip+000019D4]
	lea	rbx,[00000001000054E0]                                 ; [rip+000019C0]
	jmp	100003B9Fh

l0000000100003B22:
	lea	rdi,[0000000100005340]                                 ; [rip+00001817]
	lea	rsi,[00000001000054CC]                                 ; [rip+0000199C]
	call	100004E08h
	mov	rbx,[rbp-70h]
	lea	rcx,[rbx+0EFF100h]
	test	al,al
	mov	rax,[0000000100006680]                                 ; [rip+00002B37]
	jz	100003B71h

l0000000100003B4B:
	cmp	rcx,rax
	mov	ecx,[00000001000062D8]                                 ; [rip+00002784]
	jle	100003B6Dh

l0000000100003B56:
	cmp	rbx,rax
	jg	100003B6Dh

l0000000100003B5B:
	test	ecx,ecx

l0000000100003B5D:
	lea	rax,[0000000100005504]                                 ; [rip+000019A0]
	lea	rbx,[00000001000054FA]                                 ; [rip+0000198F]
	jmp	100003B9Fh

l0000000100003B6D:
	test	ecx,ecx
	jmp	100003B91h

l0000000100003B71:
	cmp	rcx,rax
	jle	100003B8Ah

l0000000100003B76:
	add	rax,+0EFF100h
	cmp	rbx,rax
	jge	100003B8Ah

l0000000100003B81:
	cmp	[00000001000062D8],0h                                  ; [rip+00002750]
	jmp	100003B5Dh

l0000000100003B8A:
	cmp	[00000001000062D8],0h                                  ; [rip+00002747]

l0000000100003B91:
	lea	rax,[0000000100005519]                                 ; [rip+00001981]
	lea	rbx,[000000010000550E]                                 ; [rip+0000196F]

l0000000100003B9F:
	cmovz	rbx,rax

l0000000100003BA3:
	lea	rdi,[rbp-70h]
	call	100004EA4h
	lea	r14,[rbp-68h]
	mov	esi,50h
	mov	rdi,r14
	mov	rdx,rbx
	mov	rcx,rax
	call	100004F1Ch
	mov	rax,[0000000100006040]                                 ; [rip+00002476]
	mov	rsi,[rax]
	mov	rdi,r14
	call	100004E26h
	mov	rax,[0000000100006030]                                 ; [rip+00002454]
	mov	rax,[rax]
	cmp	rax,[rbp-18h]
	jnz	100003BEEh

l0000000100003BE5:
	add	rsp,60h
	pop	rbx
	pop	r14
	pop	rbp
	ret

l0000000100003BEE:
	call	100004DC0h
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbx
	sub	rsp,+0CB8h
	mov	rax,[0000000100006030]                                 ; [rip+00002422]
	mov	rax,[rax]
	mov	[rbp-30h],rax
	mov	rax,[rdi]
	cmp	word ptr [rax+56h],0h
	mov	[rbp-0C90h],rdi
	jz	100003C6Fh

l0000000100003C26:
	lea	rax,[0000000100006578]                                 ; [rip+0000294B]
	cmp	dword ptr [rax],0h
	jnz	100003C3Eh

l0000000100003C32:
	lea	rax,[0000000100006598]                                 ; [rip+0000295F]
	cmp	dword ptr [rax],0h
	jz	100003C6Fh

l0000000100003C3E:
	mov	rdi,[rbp-0C90h]
	mov	rax,[rdi+8h]
	xor	edx,edx
	lea	rcx,[0000000100006548]                                 ; [rip+000028F6]
	div	qword ptr [rcx]
	test	rdx,rdx
	setnz	cl
	movzx	esi,cl
	add	rsi,rax
	lea	rdi,[00000001000054D5]                                 ; [rip+0000186D]
	xor	al,al
	call	100004ECEh

l0000000100003C6F:
	mov	dword ptr [rbp-0CC4h],0h
	lea	rbx,[00000001000062E0]                                 ; [rip+00002660]
	lea	r14,[0000000100006490]                                 ; [rip+00002809]
	mov	rax,[rbp-0C90h]
	mov	[rbp-0CB0h],rax
	jmp	1000046C1h

l0000000100003C9A:
	mov	rax,[rbp-0CB0h]
	cmp	qword ptr [rax+18h],1h
	jz	1000046B9h

l0000000100003CAC:
	lea	rax,[0000000100006574]                                 ; [rip+000028C1]
	cmp	dword ptr [rax],0h
	mov	rax,[rbp-0CB0h]
	mov	rax,[rax+60h]
	mov	[rbp-0C88h],rax
	jz	100003CF0h

l0000000100003CCA:
	mov	rax,[rbp-0C90h]
	mov	esi,[rax+2Ch]
	mov	rax,[rbp-0C88h]
	mov	rdx,[rax+8h]
	xor	al,al
	lea	rcx,[00000001000054AA]                                 ; [rip+000017C2]
	mov	rdi,rcx
	call	100004ECEh

l0000000100003CF0:
	lea	rax,[0000000100006598]                                 ; [rip+000028A1]
	cmp	dword ptr [rax],0h
	jz	100003D3Ah

l0000000100003CFC:
	mov	rax,[rbp-0C88h]
	mov	rax,[rax+68h]
	cqo
	lea	rcx,[0000000100006548]                                 ; [rip+00002838]
	idiv	qword ptr [rcx]
	test	rdx,rdx
	setnz	cl
	movzx	edx,cl
	add	rdx,rax
	mov	rax,[rbp-0C90h]
	mov	esi,[rax+1Ch]
	xor	al,al
	lea	rcx,[00000001000054B1]                                 ; [rip+0000177F]
	mov	rdi,rcx
	call	100004ECEh

l0000000100003D3A:
	mov	rax,[rbp-0C88h]
	movzx	edi,word ptr [rax+4h]
	lea	rax,[rbp-846h]
	mov	rsi,rax
	call	100004F28h
	mov	rax,[rbp-0CB0h]
	mov	rcx,[rax+20h]
	mov	[rbp-0CB8h],rcx
	mov	byte ptr [rbp-83Ch],0h
	lea	rcx,[rax+68h]
	mov	[rbp-0CC0h],rcx
	cmp	word ptr [rax+56h],0h
	jnz	100003D8Bh

l0000000100003D7F:
	mov	rax,rcx
	mov	[rbp-0CA8h],rax
	jmp	100003DC5h

l0000000100003D8B:
	mov	rax,[rbp-0CB0h]
	mov	rax,[rax+8h]
	mov	rcx,[rax+28h]
	lea	rdx,[rbp-0C46h]
	mov	[rbp-0CA8h],rdx
	xor	al,al
	mov	rdi,rdx
	mov	esi,400h
	lea	rdx,[0000000100005527]                                 ; [rip+0000176E]
	mov	r8,[rbp-0CC0h]
	call	100004EF8h

l0000000100003DC5:
	mov	rdi,[rbp-0CA8h]
	mov	esi,100h
	call	100004DE4h
	test	rax,rax
	mov	[rbp-0C98h],rax
	jz	100003E12h

l0000000100003DE2:
	xor	esi,esi
	mov	rdi,rax
	lea	rax,[rbp-0C78h]
	mov	rdx,rax
	call	100004DD2h
	cmp	eax,0FFh
	jnz	100003E12h

l0000000100003DFB:
	mov	rdi,[rbp-0C98h]
	call	100004DCCh
	mov	qword ptr [rbp-0C98h],+0h

l0000000100003E12:
	mov	rdi,[rbp-0CA8h]
	xor	esi,esi
	xor	edx,edx
	mov	ecx,1h
	call	100004E9Eh
	test	rax,rax
	mov	ecx,0h
	cmovns	rcx,rax

l0000000100003E33:
	mov	[rbp-0CA0h],rcx
	test	rcx,rcx
	mov	byte ptr [rbp-0C79h],0h
	jle	100003E4Fh

l0000000100003E46:
	mov	byte ptr [rbp-0C7Ah],40h
	jmp	100003E69h

l0000000100003E4F:
	cmp	qword ptr [rbp-0C98h],0h
	jz	100003E62h

l0000000100003E59:
	mov	byte ptr [rbp-0C7Ah],2Bh
	jmp	100003E69h

l0000000100003E62:
	mov	byte ptr [rbp-0C7Ah],20h

l0000000100003E69:
	lea	rax,[000000010000656C]                                 ; [rip+000026FC]
	mov	eax,[rax]
	test	eax,eax
	jz	100003EFBh

l0000000100003E7A:
	lea	rcx,[0000000100006590]                                 ; [rip+0000270F]
	cmp	dword ptr [rcx],0h
	jz	100003EC3h

l0000000100003E86:
	mov	rax,[rbp-0C90h]
	mov	ecx,[rax+30h]
	mov	rax,[rbp-0C88h]
	movzx	r8d,word ptr [rax+6h]
	xor	al,al
	lea	rdx,[000000010000568D]                                 ; [rip+000017E8]
	mov	rdi,rdx
	lea	rdx,[rbp-846h]
	mov	rsi,rdx
	lea	rdx,[rbp-0C7Ah]
	call	100004ECEh
	jmp	100003F87h

l0000000100003EC3:
	test	eax,eax
	jz	100003EFBh

l0000000100003EC7:
	mov	rax,[rbp-0C88h]
	movzx	r8d,word ptr [rax+6h]
	mov	rax,[rbp-0C90h]
	mov	r9d,[rax+28h]
	mov	ecx,[rax+30h]
	mov	rax,[rbp-0CB8h]
	mov	rax,[rax+8h]

l0000000100003EEC:
	mov	[rsp],rax
	xor	al,al
	lea	rdx,[0000000100005699]                                 ; [rip+000017A0]
	jmp	100003F6Eh

l0000000100003EFB:
	lea	rax,[0000000100006590]                                 ; [rip+0000268E]
	cmp	dword ptr [rax],0h
	jz	100003F2Dh

l0000000100003F07:
	mov	rax,[rbp-0C88h]
	movzx	r8d,word ptr [rax+6h]
	mov	rax,[rbp-0C90h]
	mov	ecx,[rax+30h]
	mov	r9d,[rax+38h]
	mov	rax,[rbp-0CB8h]
	mov	rax,[rax]
	jmp	100003EECh

l0000000100003F2D:
	mov	rax,[rbp-0C88h]
	movzx	r8d,word ptr [rax+6h]
	mov	rax,[rbp-0C90h]
	mov	r9d,[rax+38h]
	mov	edx,[rax+28h]
	mov	ecx,[rax+30h]
	mov	rax,[rbp-0CB8h]
	mov	rsi,[rax]
	mov	rax,[rax+8h]
	mov	[rsp+10h],rax
	mov	[rsp+8h],edx
	mov	[rsp],rsi
	xor	al,al
	lea	rdx,[00000001000056A9]                                 ; [rip+0000173B]

l0000000100003F6E:
	mov	rdi,rdx
	lea	rdx,[rbp-846h]
	mov	rsi,rdx
	lea	rdx,[rbp-0C7Ah]
	call	100004ECEh

l0000000100003F87:
	lea	rax,[0000000100006568]                                 ; [rip+000025DA]
	cmp	dword ptr [rax],0h
	jz	100003FB9h

l0000000100003F93:
	mov	rax,[rbp-0C90h]
	mov	esi,[rax+20h]
	mov	rax,[rbp-0CB8h]
	mov	rdx,[rax+10h]
	xor	al,al
	lea	rcx,[00000001000056BF]                                 ; [rip+0000170E]
	mov	rdi,rcx
	call	100004ECEh

l0000000100003FB9:
	mov	rax,[rbp-0C88h]
	movzx	eax,word ptr [rax+4h]
	and	eax,0F000h
	cmp	eax,2000h
	jz	100003FD7h

l0000000100003FD0:
	cmp	eax,6000h
	jnz	100004012h

l0000000100003FD7:
	mov	rax,[rbp-0C88h]
	mov	edx,[rax+18h]
	mov	esi,edx
	shr	esi,18h
	and	edx,0FFFFFFh
	cmp	edx,100h
	jc	100004007h

l0000000100003FF4:
	xor	al,al
	lea	rcx,[00000001000056C5]                                 ; [rip+000016C8]

l0000000100003FFD:
	mov	rdi,rcx
	call	100004ECEh
	jmp	100004059h

l0000000100004007:
	xor	al,al
	lea	rcx,[00000001000056D2]                                 ; [rip+000016C2]
	jmp	100003FFDh

l0000000100004012:
	mov	rax,[rbp-0C90h]
	cmp	dword ptr [rax+10h],0h
	mov	ecx,[rax+34h]
	mov	rax,[rbp-0C88h]
	mov	r8,[rax+60h]
	jz	10000404Eh

l000000010000402D:
	mov	esi,8h
	sub	esi,ecx
	xor	al,al
	lea	rdx,[00000001000056DC]                                 ; [rip+0000169F]
	mov	rdi,rdx
	lea	rdx,[0000000100005410]                                 ; [rip+000013C9]
	call	100004ECEh
	jmp	100004059h

l000000010000404E:
	mov	rdi,rcx
	mov	rsi,r8
	call	100003786h

l0000000100004059:
	lea	rax,[0000000100006558]                                 ; [rip+000024F8]
	cmp	dword ptr [rax],0h
	jz	100004072h

l0000000100004065:
	mov	rax,[rbp-0C88h]
	mov	rdi,[rax+20h]
	jmp	1000040AFh

l0000000100004072:
	lea	rax,[00000001000065A4]                                 ; [rip+0000252B]
	cmp	dword ptr [rax],0h
	jz	10000408Bh

l000000010000407E:
	mov	rax,[rbp-0C88h]
	mov	rdi,[rax+40h]
	jmp	1000040AFh

l000000010000408B:
	lea	rax,[0000000100006560]                                 ; [rip+000024CE]
	cmp	dword ptr [rax],0h
	jz	1000040A4h

l0000000100004097:
	mov	rax,[rbp-0C88h]
	mov	rdi,[rax+50h]
	jmp	1000040AFh

l00000001000040A4:
	mov	rax,[rbp-0C88h]
	mov	rdi,[rax+30h]

l00000001000040AF:
	call	100003AA8h
	lea	rax,[0000000100006564]                                 ; [rip+000024A9]
	cmp	dword ptr [rax],0h
	jz	1000040D6h

l00000001000040C0:
	mov	rax,[rbp-0C88h]
	movzx	edi,word ptr [rax+4h]
	call	1000034ACh
	mov	[rbp-0CC4h],eax

l00000001000040D6:
	mov	rdi,[rbp-0CC0h]
	call	10000356Fh
	lea	rax,[0000000100006564]                                 ; [rip+0000247B]
	cmp	dword ptr [rax],0h
	jz	10000412Dh

l00000001000040EE:
	cmp	dword ptr [rbp-0CC4h],0h
	jz	10000412Dh

l00000001000040F7:
	lea	rax,[0000000100006530]                                 ; [rip+00002432]
	mov	rdi,[rax]
	mov	esi,1h
	lea	rdx,[000000010000318E]                                 ; [rip-00000F7F]
	call	100004F46h
	lea	rax,[0000000100006540]                                 ; [rip+00002427]
	mov	rdi,[rax]
	mov	esi,1h
	lea	rdx,[000000010000318E]                                 ; [rip-00000F9A]
	call	100004F46h

l000000010000412D:
	lea	rax,[00000001000065AC]                                 ; [rip+00002478]
	cmp	dword ptr [rax],0h
	jz	100004149h

l0000000100004139:
	mov	rax,[rbp-0C88h]
	movzx	edi,word ptr [rax+4h]
	call	100003201h

l0000000100004149:
	mov	rax,[rbp-0C88h]
	movzx	eax,word ptr [rax+4h]
	and	eax,0F000h
	cmp	eax,0A000h
	jnz	100004252h

l0000000100004164:
	mov	rax,[rbp-0CB0h]
	cmp	word ptr [rax+56h],0h
	jnz	10000419Bh

l0000000100004172:
	xor	al,al
	lea	rcx,[rbp-431h]
	mov	rdi,rcx
	mov	esi,401h
	lea	rcx,[0000000100005524]                                 ; [rip+0000139A]
	mov	rdx,rcx
	mov	rcx,[rbp-0CC0h]
	call	100004EF8h
	jmp	1000041CEh

l000000010000419B:
	mov	rax,[rbp-0CB0h]
	mov	rax,[rax+8h]
	mov	rcx,[rax+28h]
	xor	al,al
	lea	rdx,[rbp-431h]
	mov	rdi,rdx
	mov	esi,401h
	lea	rdx,[0000000100005527]                                 ; [rip+00001365]
	mov	r8,[rbp-0CC0h]
	call	100004EF8h

l00000001000041CE:
	lea	rax,[rbp-431h]
	mov	rdi,rax
	lea	rax,[rbp-832h]
	mov	rsi,rax
	mov	edx,400h
	call	100004EDAh
	cmp	eax,0FFh
	jnz	100004227h

l00000001000041F1:
	call	100004DAEh
	mov	edi,[rax]
	call	100004F16h
	mov	rcx,[0000000100006038]                                 ; [rip+00001E34]
	mov	rdi,[rcx]
	lea	rcx,[000000010000552D]                                 ; [rip+0000131F]
	mov	rsi,rcx
	lea	rcx,[rbp-431h]
	mov	rdx,rcx
	mov	rcx,rax
	xor	al,al
	call	100004E20h
	jmp	100004252h

l0000000100004227:
	movsxd	rax,eax
	mov	byte ptr [rbp+rax-832h],0h
	xor	al,al
	lea	rcx,[000000010000553A]                                 ; [rip+000012FF]
	mov	rdi,rcx
	call	100004ECEh
	lea	rax,[rbp-832h]
	mov	rdi,rax
	call	10000356Fh

l0000000100004252:
	mov	edi,0Ah
	call	100004ED4h
	lea	rax,[00000001000065B0]                                 ; [rip+0000234D]
	cmp	dword ptr [rax],0h
	jz	10000434Eh

l000000010000426C:
	cmp	qword ptr [rbp-0CA0h],0h
	jz	10000434Eh

l000000010000427A:
	mov	rdi,[rbp-0CA0h]
	call	100004EAAh
	test	rax,rax
	jz	1000046F0h

l000000010000428F:
	mov	[rbp-0CB8h],rax
	mov	rdi,[rbp-0CA8h]
	mov	rsi,rax
	mov	rdx,[rbp-0CA0h]
	mov	ecx,1h
	call	100004E9Eh
	test	rax,rax
	jle	100004342h

l00000001000042BA:
	cmp	qword ptr [rbp-0CA0h],0h
	jle	100004342h

l00000001000042C4:
	mov	rax,[rbp-0CB8h]
	add	[rbp-0CA0h],rax
	mov	r15,rax

l00000001000042D5:
	xor	edx,edx
	xor	r8d,r8d
	mov	rdi,[rbp-0CA8h]
	mov	rsi,r15
	xor	ecx,ecx
	mov	r9d,1h
	call	100004E7Ah
	mov	r12,rax
	mov	edi,9h
	call	100004ED4h
	mov	rdi,r15
	call	10000356Fh
	mov	edi,9h
	call	100004ED4h
	mov	rax,[rbp-0C90h]
	mov	edi,[rax+34h]
	mov	rsi,r12
	call	100003786h
	mov	edi,0Ah
	call	100004ED4h
	mov	rdi,r15
	call	100004F22h
	lea	r15,[rax+r15+1h]
	cmp	r15,[rbp-0CA0h]
	jc	1000042D5h

l0000000100004342:
	mov	rdi,[rbp-0CB8h]
	call	100004E2Ch

l000000010000434E:
	cmp	qword ptr [rbp-0C98h],0h
	jz	1000046B9h

l000000010000435C:
	lea	rax,[000000010000655C]                                 ; [rip+000021F9]
	cmp	dword ptr [rax],0h
	jz	1000046ADh

l000000010000436C:
	mov	rax,[rbp-0C88h]
	movzx	eax,word ptr [rax+4h]
	mov	qword ptr [rbp-0C58h],+0h
	and	eax,0F000h
	cmp	eax,4000h
	setnz	al
	movzx	r15d,al
	inc	r15d
	mov	dword ptr [rbp-0C88h],0h
	xor	al,al
	jmp	100004686h

l00000001000043A7:
	mov	rdi,[rbp-0C58h]
	lea	rax,[rbp-0C5Ch]
	mov	rsi,rax
	call	100004DFCh
	test	eax,eax
	jnz	100004675h

l00000001000043C5:
	mov	rdi,[rbp-0C58h]
	lea	rax,[rbp-0C68h]
	mov	rsi,rax
	call	100004DDEh
	test	eax,eax
	jnz	100004675h

l00000001000043E3:
	mov	rdi,[rbp-0C58h]
	lea	rax,[rbp-0C70h]
	mov	rsi,rax
	call	100004DF0h
	test	eax,eax
	jnz	100004675h

l0000000100004401:
	mov	rdi,[rbp-0C58h]
	call	100004DF6h
	mov	r12,rax
	test	r12,r12
	jz	100004675h

l0000000100004419:
	mov	dword ptr [rbp-0C4Ch],0FFFFFFFFh
	mov	edi,105h
	call	100004EAAh
	test	rax,rax
	jz	1000046F0h

l0000000100004436:
	mov	r13,rax
	lea	rax,[0000000100006584]                                 ; [rip+00002144]
	cmp	dword ptr [rax],0h
	jnz	1000044F6h

l0000000100004449:
	mov	rdi,r12
	lea	rax,[rbp-0C50h]
	mov	rsi,rax
	lea	rax,[rbp-0C4Ch]
	mov	rdx,rax
	call	100004EB0h
	test	eax,eax
	jnz	1000044F6h

l000000010000446D:
	mov	eax,[rbp-0C4Ch]
	cmp	eax,1h
	jz	1000044B9h

l0000000100004478:
	test	eax,eax
	jnz	1000044F6h

l000000010000447C:
	mov	edi,[rbp-0C50h]
	call	100004E6Eh
	test	rax,rax
	jz	1000044F6h

l000000010000448C:
	mov	rax,[rax]
	mov	[rsp],rax
	xor	edx,edx
	xor	al,al
	mov	rdi,r13
	mov	esi,105h
	mov	ecx,105h
	lea	r8,[0000000100005546]                                  ; [rip+0000109B]
	lea	r9,[000000010000554C]                                  ; [rip+0000109A]
	call	100004DBAh
	jmp	100004533h

l00000001000044B9:
	mov	edi,[rbp-0C50h]
	call	100004E5Ch
	test	rax,rax
	jz	1000044F6h

l00000001000044C9:
	mov	rax,[rax]
	mov	[rsp],rax
	xor	edx,edx
	xor	al,al
	mov	rdi,r13
	mov	esi,105h
	mov	ecx,105h
	lea	r8,[0000000100005546]                                  ; [rip+0000105E]
	lea	r9,[0000000100005551]                                  ; [rip+00001062]
	call	100004DBAh
	jmp	100004533h

l00000001000044F6:
	mov	rdi,r12
	mov	rsi,r13
	call	100004EB6h
	test	eax,eax
	jz	100004533h

l0000000100004505:
	mov	rax,[0000000100006038]                                 ; [rip+00001B2C]
	mov	rsi,[rax]
	lea	rax,[0000000100005557]                                 ; [rip+00001041]
	mov	rdi,rax
	call	100004E26h
	mov	rax,4E574F4E4B4E553Ch
	mov	[r13+0h],rax
	mov	word ptr [r13+8h],3Eh

l0000000100004533:
	mov	rdi,r12
	call	100004DCCh
	mov	eax,[rbp-0C5Ch]
	cmp	eax,2h
	jz	100004554h

l0000000100004546:
	cmp	eax,1h
	jnz	10000455Dh

l000000010000454B:
	lea	r12,[000000010000557F]                                 ; [rip+0000102D]
	jmp	100004564h

l0000000100004554:
	lea	r12,[0000000100005585]                                 ; [rip+0000102A]
	jmp	100004564h

l000000010000455D:
	lea	r12,[000000010000558A]                                 ; [rip+00001026]

l0000000100004564:
	mov	rdi,[rbp-0C68h]
	mov	esi,10h
	call	100004DD8h
	test	eax,eax
	lea	rcx,[0000000100005592]                                 ; [rip+00001014]
	lea	rax,[0000000100005410]                                 ; [rip+00000E8B]
	cmovz	rcx,rax

l0000000100004589:
	xor	al,al
	lea	rdi,[000000010000559D]                                 ; [rip+0000100B]
	mov	esi,[rbp-0C88h]
	mov	rdx,r13
	mov	r8,r12
	call	100004ECEh
	mov	rdi,r13
	call	100004E2Ch
	xor	r12d,r12d
	mov	r13d,10h

l00000001000045B4:
	mov	esi,[r13+rbx-10h]
	mov	rdi,[rbp-0C70h]
	call	100004DEAh
	test	eax,eax
	jz	100004601h

l00000001000045C9:
	test	[r13+rbx+0h],r15d
	jz	100004601h

l00000001000045D0:
	test	r12d,r12d
	lea	rax,[000000010000564C]                                 ; [rip+00001072]
	mov	rsi,rax
	lea	rax,[0000000100005410]                                 ; [rip+00000E2C]
	cmovz	rsi,rax

l00000001000045E8:
	mov	rdx,[r13+rbx-8h]
	xor	al,al
	lea	rcx,[000000010000564E]                                 ; [rip+00001058]
	mov	rdi,rcx
	call	100004ECEh
	inc	r12d

l0000000100004601:
	add	r13,18h
	cmp	r13,+1A8h
	jnz	1000045B4h

l000000010000460E:
	mov	r13d,10h

l0000000100004614:
	mov	esi,[r13+r14-10h]
	mov	rdi,[rbp-0C68h]
	call	100004DD8h
	test	eax,eax
	jz	100004661h

l0000000100004629:
	test	[r13+r14+0h],r15d
	jz	100004661h

l0000000100004630:
	test	r12d,r12d
	lea	rax,[000000010000564C]                                 ; [rip+00001012]
	mov	rsi,rax
	lea	rax,[0000000100005410]                                 ; [rip+00000DCC]
	cmovz	rsi,rax

l0000000100004648:
	mov	rdx,[r13+r14-8h]
	xor	al,al
	lea	rcx,[000000010000564E]                                 ; [rip+00000FF8]
	mov	rdi,rcx
	call	100004ECEh
	inc	r12d

l0000000100004661:
	add	r13,18h
	cmp	r13,70h
	jnz	100004614h

l000000010000466B:
	mov	edi,0Ah
	call	100004ED4h

l0000000100004675:
	cmp	qword ptr [rbp-0C58h],0h
	setnz	al
	inc	dword ptr [rbp-0C88h]

l0000000100004686:
	movzx	esi,al
	shl	esi,1Fh
	sar	esi,1Fh
	mov	rdi,[rbp-0C98h]
	lea	rax,[rbp-0C58h]
	mov	rdx,rax
	call	100004DD2h
	test	eax,eax
	jz	1000043A7h

l00000001000046AD:
	mov	rdi,[rbp-0C98h]
	call	100004DCCh

l00000001000046B9:
	add	qword ptr [rbp-0CB0h],10h

l00000001000046C1:
	mov	rax,[rbp-0CB0h]
	mov	rax,[rax]
	mov	[rbp-0CB0h],rax
	test	rax,rax
	jnz	100003C9Ah

l00000001000046DB:
	mov	rax,[0000000100006030]                                 ; [rip+0000194E]
	mov	rax,[rax]
	cmp	rax,[rbp-30h]
	jz	100004703h

l00000001000046EB:
	call	100004DC0h

l00000001000046F0:
	mov	edi,1h
	lea	rsi,[00000001000052B5]                                 ; [rip+00000BB9]
	xor	al,al
	call	100004E0Eh

l0000000100004703:
	add	rsp,+0CB8h
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	ret

;; fn0000000100004715: 0000000100004715
;;   Called from:
;;     0000000100003598 (in fn000000010000356F)
fn0000000100004715 proc
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbx
	sub	rsp,+98h
	pxor	xmm0,xmm0
	movaps	[rbp-40h],xmm0
	movaps	[rbp-50h],xmm0
	movaps	[rbp-60h],xmm0
	movaps	[rbp-70h],xmm0
	movaps	[rbp-80h],xmm0
	movaps	[rbp-90h],xmm0
	movaps	[rbp-0A0h],xmm0
	movaps	[rbp-0B0h],xmm0
	mov	dword ptr [rbp-0B8h],0h
	lea	rbx,[rbp-0B0h]
	mov	r14,rdi
	jmp	10000482Fh

l000000010000476F:
	lea	rdi,[0000000100005524]                                 ; [rip+00000DAE]
	xor	al,al
	mov	rsi,r14
	call	100004ECEh
	add	eax,[rbp-0B8h]
	jmp	100004879h

l000000010000478B:
	pxor	xmm0,xmm0
	movaps	[rbp-40h],xmm0
	movaps	[rbp-50h],xmm0
	movaps	[rbp-60h],xmm0
	movaps	[rbp-70h],xmm0
	movaps	[rbp-80h],xmm0
	movaps	[rbp-90h],xmm0
	movaps	[rbp-0A0h],xmm0
	movaps	[rbp-0B0h],xmm0
	movzx	edi,byte ptr [r14]
	call	100004ED4h
	inc	dword ptr [rbp-0B8h]
	inc	r14
	jmp	10000482Fh

l00000001000047CC:
	mov	r12d,r15d
	mov	r13,r14

l00000001000047D2:
	movzx	edi,byte ptr [r13+0h]
	call	100004ED4h
	inc	r13
	dec	r12
	jnz	1000047D2h

l00000001000047E4:
	add	r14,r15
	movsxd	rdi,dword ptr [rbp-0B4h]
	cmp	rdi,7Fh
	ja	100004808h

l00000001000047F4:
	mov	r15,[0000000100006028]                                 ; [rip+0000182D]
	mov	eax,[r15+rdi*4+3Ch]
	shr	eax,12h
	and	eax,1h
	jmp	10000481Ah

l0000000100004808:
	mov	esi,40000h
	call	100004DB4h
	test	eax,eax
	setnz	al
	movzx	eax,al

l000000010000481A:
	test	eax,eax
	jz	10000482Fh

l000000010000481E:
	mov	edi,[rbp-0B4h]
	call	100004F5Eh
	add	[rbp-0B8h],eax

l000000010000482F:
	lea	r15,[rbp-0B4h]
	mov	rdi,r15
	mov	rsi,r14
	mov	edx,6h
	mov	rcx,rbx
	call	100004EBCh
	cmp	rax,0FEh
	jz	10000476Fh

l0000000100004853:
	mov	r15,rax
	cmp	r15,0FFh
	jz	10000478Bh

l0000000100004860:
	test	r15,r15
	jz	100004873h

l0000000100004865:
	test	r15d,r15d
	jg	1000047CCh

l000000010000486E:
	jmp	1000047E4h

l0000000100004873:
	mov	eax,[rbp-0B8h]

l0000000100004879:
	add	rsp,+98h
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	ret

;; fn000000010000488B: 000000010000488B
;;   Called from:
;;     00000001000026D5 (in fn00000001000026A0)
;;     0000000100002B56 (in fn00000001000026A0)
fn000000010000488B proc
	push	rbp
	mov	rbp,rsp
	mov	rax,[0000000100006038]                                 ; [rip+000017A2]
	mov	rsi,[rax]
	lea	rdi,[0000000100005730]                                 ; [rip+00000E90]
	call	100004E26h
	mov	edi,1h
	call	100004E14h

;; fn00000001000048AF: 00000001000048AF
;;   Called from:
;;     000000010000359E (in fn000000010000356F)
;;     00000001000048AA (in fn000000010000488B)
fn00000001000048AF proc
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbx
	sub	rsp,+98h
	pxor	xmm0,xmm0
	movaps	[rbp-40h],xmm0
	movaps	[rbp-50h],xmm0
	movaps	[rbp-60h],xmm0
	movaps	[rbp-70h],xmm0
	movaps	[rbp-80h],xmm0
	movaps	[rbp-90h],xmm0
	movaps	[rbp-0A0h],xmm0
	movaps	[rbp-0B0h],xmm0
	mov	dword ptr [rbp-0B8h],0h
	mov	rbx,rdi
	jmp	100004ABBh

l0000000100004902:
	pxor	xmm0,xmm0
	movaps	[rbp-40h],xmm0
	movaps	[rbp-50h],xmm0
	movaps	[rbp-60h],xmm0
	movaps	[rbp-70h],xmm0
	movaps	[rbp-80h],xmm0
	movaps	[rbp-90h],xmm0
	movaps	[rbp-0A0h],xmm0
	movaps	[rbp-0B0h],xmm0
	inc	rbx
	jmp	100004ABBh

l0000000100004937:
	mov	r14,rax
	cmp	r14,0FDh
	ja	100004A24h

l0000000100004944:
	movsxd	rdi,dword ptr [rbp-0B4h]
	cmp	rdi,7Fh
	ja	100004964h

l0000000100004951:
	mov	rax,[0000000100006028]                                 ; [rip+000016D0]
	mov	eax,[rax+rdi*4+3Ch]
	shr	eax,12h
	and	eax,1h
	jmp	100004976h

l0000000100004964:
	mov	esi,40000h
	call	100004DB4h
	test	eax,eax
	setnz	al
	movzx	eax,al

l0000000100004976:
	test	eax,eax
	jz	1000049B7h

l000000010000497A:
	mov	edi,[rbp-0B4h]
	cmp	edi,22h
	jz	1000049B7h

l0000000100004985:
	cmp	edi,5Ch
	jz	1000049B7h

l000000010000498A:
	test	r14d,r14d
	jle	1000049ADh

l000000010000498F:
	mov	r15d,r14d
	mov	r12,rbx

l0000000100004995:
	movzx	edi,byte ptr [r12]
	call	100004ED4h
	inc	r12
	dec	r15
	jnz	100004995h

l00000001000049A7:
	mov	edi,[rbp-0B4h]

l00000001000049AD:
	call	100004F5Eh
	jmp	100004AA2h

l00000001000049B7:
	cmp	r14,0FDh
	ja	100004A24h

l00000001000049BD:
	lea	rax,[000000010000658C]                                 ; [rip+00001BC8]
	cmp	dword ptr [rax],0h
	jz	100004A19h

l00000001000049C9:
	mov	eax,[rbp-0B4h]
	test	eax,eax
	js	100004A19h

l00000001000049D3:
	cmp	eax,0FFh
	jg	100004A19h

l00000001000049DA:
	movsx	esi,al
	lea	rax,[0000000100005770]                                 ; [rip+00000D8C]
	mov	rdi,rax
	mov	edx,13h
	call	100004EC2h
	test	rax,rax
	jz	100004A19h

l00000001000049F6:
	mov	r15,rax
	mov	edi,5Ch
	call	100004ED4h
	movsx	edi,byte ptr [r15+1h]
	call	100004ED4h
	add	dword ptr [rbp-0B8h],2h
	jmp	100004AA8h

l0000000100004A19:
	cmp	r14,0FDh
	ja	100004A24h

l0000000100004A1F:
	mov	eax,r14d
	jmp	100004A39h

l0000000100004A24:
	cmp	r14,0FFh
	jnz	100004A31h

l0000000100004A2A:
	mov	eax,1h
	jmp	100004A3Dh

l0000000100004A31:
	mov	rdi,rbx
	call	100004F22h

l0000000100004A39:
	test	eax,eax
	jle	100004AA8h

l0000000100004A3D:
	mov	r15d,eax
	lea	eax,[0000h+r15*4]
	mov	[rbp-0BCh],eax
	mov	r12,rbx

l0000000100004A51:
	movzx	r13d,byte ptr [r12]
	mov	edi,5Ch
	call	100004ED4h
	mov	al,r13b
	shr	al,6h
	movzx	edi,al
	add	edi,30h
	call	100004ED4h
	mov	al,r13b
	shr	al,3h
	movzx	edi,al
	and	edi,7h
	add	edi,30h
	call	100004ED4h
	and	r13d,7h
	mov	edi,r13d
	add	edi,30h
	call	100004ED4h
	inc	r12
	dec	r15
	jnz	100004A51h

l0000000100004A9C:
	mov	eax,[rbp-0BCh]

l0000000100004AA2:
	add	[rbp-0B8h],eax

l0000000100004AA8:
	cmp	r14,0FFh
	jz	100004902h

l0000000100004AB2:
	cmp	r14,0FEh
	jz	100004AE2h

l0000000100004AB8:
	add	rbx,r14

;; fn0000000100004ABB: 0000000100004ABB
;;   Called from:
;;     00000001000048FD (in fn00000001000048AF)
;;     00000001000048FD (in fn00000001000048AF)
fn0000000100004ABB proc
	lea	rdi,[rbp-0B4h]
	mov	rsi,rbx
	mov	edx,6h
	lea	rax,[rbp-0B0h]
	mov	rcx,rax
	call	100004EBCh
	test	rax,rax
	jnz	100004937h

l0000000100004AE2:
	mov	eax,[rbp-0B8h]
	add	rsp,+98h
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	ret

;; fn0000000100004AFA: 0000000100004AFA
;;   Called from:
;;     0000000100001F4E (in fn0000000100001B4A)
fn0000000100004AFA proc
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbx
	sub	rsp,+98h
	pxor	xmm0,xmm0
	movaps	[rbp-40h],xmm0
	movaps	[rbp-50h],xmm0
	movaps	[rbp-60h],xmm0
	movaps	[rbp-70h],xmm0
	movaps	[rbp-80h],xmm0
	movaps	[rbp-90h],xmm0
	movaps	[rbp-0A0h],xmm0
	movaps	[rbp-0B0h],xmm0
	movsxd	rbx,esi
	lea	eax,[0000h+rbx*4]
	mov	[rbp-0B8h],eax
	xor	r14d,r14d
	mov	r15,rbx
	mov	r12,rdi
	jmp	100004BECh

l0000000100004B59:
	movsxd	rax,dword ptr [rbp-0B8h]
	add	rax,r14
	jmp	100004C32h

l0000000100004B68:
	pxor	xmm0,xmm0
	movaps	[rbp-40h],xmm0
	movaps	[rbp-50h],xmm0
	movaps	[rbp-60h],xmm0
	movaps	[rbp-70h],xmm0
	movaps	[rbp-80h],xmm0
	movaps	[rbp-90h],xmm0
	movaps	[rbp-0A0h],xmm0
	movaps	[rbp-0B0h],xmm0
	dec	ebx
	add	dword ptr [rbp-0B8h],0FCh
	dec	r15
	inc	r12
	add	r14,4h
	jmp	100004BECh

l0000000100004BAA:
	movsxd	rdi,dword ptr [rbp-0B4h]
	cmp	rdi,7Fh
	ja	100004BCAh

l0000000100004BB7:
	mov	rax,[0000000100006028]                                 ; [rip+0000146A]
	mov	eax,[rax+rdi*4+3Ch]
	shr	eax,12h
	and	eax,1h
	jmp	100004BDCh

l0000000100004BCA:
	mov	esi,40000h
	call	100004DB4h
	test	eax,eax
	setnz	al
	movzx	eax,al

l0000000100004BDC:
	test	eax,eax
	jz	100004BE5h

l0000000100004BE0:
	inc	r14
	jmp	100004BE9h

l0000000100004BE5:
	lea	r14,[r14+r13*4]

l0000000100004BE9:
	add	r12,r13

l0000000100004BEC:
	test	ebx,ebx
	jz	100004C2Fh

l0000000100004BF0:
	lea	r13,[rbp-0B4h]
	mov	rdi,r13
	mov	rsi,r12
	mov	rdx,r15
	lea	r13,[rbp-0B0h]
	mov	rcx,r13
	call	100004EBCh
	cmp	rax,0FEh
	jz	100004B59h

l0000000100004C19:
	mov	r13,rax
	cmp	r13,0FFh
	jz	100004B68h

l0000000100004C26:
	test	r13,r13
	jnz	100004BAAh

l0000000100004C2F:
	mov	rax,r14

l0000000100004C32:
	add	rsp,+98h
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	ret

l0000000100004C44:
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbx
	sub	rsp,+98h
	pxor	xmm0,xmm0
	movaps	[rbp-40h],xmm0
	movaps	[rbp-50h],xmm0
	movaps	[rbp-60h],xmm0
	movaps	[rbp-70h],xmm0
	movaps	[rbp-80h],xmm0
	movaps	[rbp-90h],xmm0
	movaps	[rbp-0A0h],xmm0
	movaps	[rbp-0B0h],xmm0
	xor	ebx,ebx
	mov	r14,rdi
	jmp	100004D5Bh

l0000000100004C8F:
	mov	edi,3Fh
	call	100004ED4h
	mov	eax,ebx
	inc	eax
	jmp	100004D9Bh

l0000000100004CA2:
	mov	edi,3Fh
	call	100004ED4h
	add	r14,r15
	inc	ebx
	jmp	100004D5Bh

l0000000100004CB6:
	mov	edi,3Fh
	call	100004ED4h
	pxor	xmm0,xmm0
	movaps	[rbp-40h],xmm0
	movaps	[rbp-50h],xmm0
	movaps	[rbp-60h],xmm0
	movaps	[rbp-70h],xmm0
	movaps	[rbp-80h],xmm0
	movaps	[rbp-90h],xmm0
	movaps	[rbp-0A0h],xmm0
	movaps	[rbp-0B0h],xmm0
	inc	ebx
	inc	r14
	jmp	100004D5Bh

l0000000100004CF4:
	movsxd	rdi,dword ptr [rbp-0B4h]
	cmp	rdi,7Fh
	ja	100004D14h

l0000000100004D01:
	mov	rax,[0000000100006028]                                 ; [rip+00001320]
	mov	eax,[rax+rdi*4+3Ch]
	shr	eax,12h
	and	eax,1h
	jmp	100004D26h

l0000000100004D14:
	mov	esi,40000h
	call	100004DB4h
	test	eax,eax
	setnz	al
	movzx	eax,al

l0000000100004D26:
	test	eax,eax
	jz	100004CA2h

l0000000100004D2E:
	test	r15d,r15d
	jle	100004D4Bh

l0000000100004D33:
	mov	r12d,r15d
	mov	r13,r14

l0000000100004D39:
	movzx	edi,byte ptr [r13+0h]
	call	100004ED4h
	inc	r13
	dec	r12
	jnz	100004D39h

l0000000100004D4B:
	add	r14,r15
	mov	edi,[rbp-0B4h]
	call	100004F5Eh
	add	ebx,eax

l0000000100004D5B:
	lea	rdi,[rbp-0B4h]
	mov	rsi,r14
	mov	edx,6h
	lea	r15,[rbp-0B0h]
	mov	rcx,r15
	call	100004EBCh
	cmp	rax,0FEh
	jz	100004C8Fh

l0000000100004D83:
	mov	r15,rax
	cmp	r15,0FFh
	jz	100004CB6h

l0000000100004D90:
	test	r15,r15
	jnz	100004CF4h

l0000000100004D99:
	mov	eax,ebx

l0000000100004D9B:
	add	rsp,+98h
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	ret
