;;; Segment .text (00000000004028A0)
00000000004028A0 50 B9 88 2C 41 00 BA A6 0E 00 00 BE 36 37 41 00 P..,A.......67A.
00000000004028B0 BF 98 3C 41 00 E8 96 FB FF FF 66 0F 1F 44 00 00 ..<A......f..D..

;; fn00000000004028C0: 00000000004028C0
fn00000000004028C0 proc
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbp
	mov	rbp,rsi
	push	rbx
	mov	ebx,edi
	sub	rsp,+388h
	mov	rdi,[rsi]
	mov	rax,fs:[0028h]
	mov	[rsp+378h],rax
	xor	eax,eax
	call	40D6A0h
	mov	esi,416919h
	mov	edi,6h
	call	402710h
	mov	esi,41381Ch
	mov	edi,413800h
	call	402340h
	mov	edi,413800h
	call	402300h
	mov	edi,40A200h
	mov	[000000000061A580],2h                                  ; [rip+00217C58]
	call	411EE0h
	mov	rax,8000000000000000h
	mov	[000000000061B030],0h                                  ; [rip+002186EF]
	mov	[000000000061B0D0],1h                                  ; [rip+00218788]
	mov	[000000000061B180],rax                                 ; [rip+00218831]
	mov	eax,[000000000061A56C]                                 ; [rip+00217C17]
	mov	[000000000061B190],+0h                                 ; [rip+00218830]
	mov	[000000000061B188],-1h                                 ; [rip+0021881D]
	mov	[000000000061B0F0],0h                                  ; [rip+0021877E]
	cmp	eax,2h
	jz	403203h

l000000000040297B:
	cmp	eax,3h
	jz	4029AFh

l0000000000402980:
	sub	eax,1h
	jz	40298Ah

l0000000000402985:
	call	402220h

l000000000040298A:
	mov	edi,1h
	call	402280h
	test	eax,eax
	jz	4037ECh

l000000000040299C:
	mov	[000000000061B150],2h                                  ; [rip+002187AA]
	mov	[000000000061B0F0],1h                                  ; [rip+00218743]
	jmp	4029C5h

l00000000004029AF:
	mov	esi,5h
	xor	edi,edi
	mov	[000000000061B150],0h                                  ; [rip+00218790]
	call	40E640h

l00000000004029C5:
	mov	edi,41382Eh
	mov	[000000000061B14C],0h                                  ; [rip+00218778]
	mov	[000000000061B148],0h                                  ; [rip+0021876A]
	mov	[000000000061B147],0h                                  ; [rip+00218762]
	mov	[000000000061B145],0h                                  ; [rip+00218759]
	mov	[000000000061B144],0h                                  ; [rip+00218751]
	mov	[000000000061B12C],0h                                  ; [rip+0021872F]
	mov	[000000000061B114],0h                                  ; [rip+00218710]
	mov	[000000000061B110],1h                                  ; [rip+00218702]
	mov	[000000000061B10E],0h                                  ; [rip+002186F9]
	mov	[000000000061B10D],0h                                  ; [rip+002186F1]
	mov	[000000000061B108],0h                                  ; [rip+002186E2]
	mov	[000000000061B100],+0h                                 ; [rip+002186CF]
	mov	[000000000061B0F8],+0h                                 ; [rip+002186BC]
	mov	[000000000061B17D],0h                                  ; [rip+0021873A]
	call	4021C0h
	test	rax,rax
	mov	r12,rax
	jz	402A7Fh

l0000000000402A50:
	mov	ecx,4h
	mov	edx,416460h
	mov	esi,416480h
	mov	rdi,rax
	call	409E50h
	test	eax,eax
	js	403786h

l0000000000402A6F:
	cdqe
	xor	edi,edi
	mov	esi,[416460h+rax*4]
	call	40E640h

l0000000000402A7F:
	mov	edi,41383Ch
	mov	[000000000061B0C8],+50h                                ; [rip+00218639]
	call	4021C0h
	mov	r12,rax
	lea	rax,[rsp+40h]
	test	r12,r12
	mov	[rsp+20h],rax
	jz	402AB1h

l0000000000402AA6:
	cmp	byte ptr [r12],0h
	jnz	4037B8h

l0000000000402AB1:
	mov	rdx,[rsp+20h]
	xor	eax,eax
	mov	esi,5413h
	mov	edi,1h
	call	4024B0h
	cmp	eax,0FFh
	jz	402ADDh

l0000000000402ACC:
	movzx	eax,word ptr [rsp+42h]
	test	ax,ax
	jz	402ADDh

l0000000000402AD6:
	mov	[000000000061B0C8],rax                                 ; [rip+002185EB]

l0000000000402ADD:
	mov	edi,413844h
	call	4021C0h
	test	rax,rax
	mov	r12,rax
	mov	[000000000061B0D8],+8h                                 ; [rip+002185E0]
	jz	402B22h

l0000000000402AFA:
	mov	rcx,[rsp+20h]
	xor	r8d,r8d
	xor	edx,edx
	xor	esi,esi
	mov	rdi,rax
	call	410E90h
	test	eax,eax
	jnz	4041BBh

l0000000000402B16:
	mov	rax,[rsp+40h]
	mov	[000000000061B0D8],rax                                 ; [rip+002185B6]

l0000000000402B22:
	xor	r14d,r14d
	xor	r13d,r13d
	xor	r12d,r12d
	nop	dword ptr [rax+rax+0h]

l0000000000402B30:
	lea	r8,[rsp+38h]
	mov	ecx,413080h
	mov	edx,415BC8h
	mov	rsi,rbp
	mov	edi,ebx
	mov	dword ptr [rsp+38h],0FFFFFFFFh
	call	4023B0h
	cmp	eax,0FFh
	jz	40321Eh

l0000000000402B5A:
	add	eax,83h
	cmp	eax,112h
	ja	4031F9h

l0000000000402B6A:
	jmp	qword ptr [412330h+rax*8]

l0000000000402B71:
	mov	[000000000061B145],1h                                  ; [rip+002185CD]

l0000000000402B78:
	mov	[000000000061B150],0h                                  ; [rip+002185CE]
	jmp	402B30h

l0000000000402B84:
	mov	r14d,1h
	jmp	402B30h

l0000000000402B8C:
	mov	[000000000061B114],1h                                  ; [rip+00218581]
	jmp	402B30h

l0000000000402B95:
	mov	[000000000061B140],0B0h                                ; [rip+002185A1]
	mov	[000000000061B134],0B0h                                ; [rip+0021858B]
	mov	[000000000061B138],+1h                                 ; [rip+00218584]
	mov	[000000000061A560],+1h                                 ; [rip+002179A1]
	jmp	402B30h

l0000000000402BC4:
	mov	[000000000061B150],0h                                  ; [rip+00218582]
	mov	[000000000061A569],0h                                  ; [rip+00217994]
	jmp	402B30h

l0000000000402BDA:
	cmp	[000000000061B150],0h                                  ; [rip+0021856F]
	mov	[000000000061B108],2h                                  ; [rip+0021851D]
	mov	[000000000061B148],0FFFFFFFFh                          ; [rip+00218553]
	jz	403C36h

l0000000000402BFB:
	mov	[000000000061B144],0h                                  ; [rip+00218542]
	mov	[000000000061B129],0h                                  ; [rip+00218520]
	mov	r13d,1h
	jmp	402B30h

l0000000000402C14:
	mov	[000000000061B10D],1h                                  ; [rip+002184F2]
	jmp	402B30h

l0000000000402C20:
	mov	[000000000061B14C],1h                                  ; [rip+00218522]
	jmp	402B30h

l0000000000402C2F:
	mov	esi,5h
	xor	edi,edi
	call	40E640h
	jmp	402B30h

l0000000000402C40:
	mov	[000000000061B108],2h                                  ; [rip+002184BE]
	jmp	402B30h

l0000000000402C4F:
	mov	[000000000061B17D],1h                                  ; [rip+00218527]
	jmp	402B30h

l0000000000402C5B:
	mov	[000000000061B148],1h                                  ; [rip+002184E3]
	mov	r13d,1h
	jmp	402B30h

l0000000000402C70:
	mov	[000000000061B148],0FFFFFFFFh                          ; [rip+002184CE]
	mov	r13d,1h
	jmp	402B30h

l0000000000402C85:
	mov	rcx,[rsp+20h]
	mov	rdi,[000000000061A640]                                 ; [rip+002179AF]
	xor	r8d,r8d
	xor	edx,edx
	xor	esi,esi
	call	410E90h
	test	eax,eax
	jnz	403BFDh

l0000000000402CA5:
	mov	rax,[rsp+40h]
	mov	[000000000061B0D8],rax                                 ; [rip+00218427]
	jmp	402B30h

l0000000000402CB6:
	mov	[000000000061B148],2h                                  ; [rip+00218488]
	mov	r13d,1h
	jmp	402B30h

l0000000000402CCB:
	mov	[000000000061B10E],1h                                  ; [rip+0021843C]
	jmp	402B30h

l0000000000402CD7:
	mov	esi,3h
	xor	edi,edi
	call	40E640h
	jmp	402B30h

l0000000000402CE8:
	xor	esi,esi
	xor	edi,edi
	call	40E640h
	jmp	402B30h

l0000000000402CF6:
	mov	[000000000061B110],5h                                  ; [rip+00218410]
	jmp	402B30h

l0000000000402D05:
	mov	edi,10h
	mov	r15,[000000000061A640]                                 ; [rip+0021792F]
	call	410C40h
	mov	rdx,[000000000061B100]                                 ; [rip+002183E3]
	mov	[rax],r15
	mov	[rax+8h],rdx
	mov	[000000000061B100],rax                                 ; [rip+002183D5]
	jmp	402B30h

l0000000000402D30:
	mov	[000000000061B110],3h                                  ; [rip+002183D6]
	jmp	402B30h

l0000000000402D3F:
	mov	[000000000061A568],0h                                  ; [rip+00217822]
	jmp	402B30h

l0000000000402D4B:
	mov	[000000000061B12C],3h                                  ; [rip+002183D7]
	jmp	402B30h

l0000000000402D5A:
	mov	[000000000061B130],1h                                  ; [rip+002183CF]
	jmp	402B30h

l0000000000402D66:
	mov	[000000000061B150],2h                                  ; [rip+002183E0]
	jmp	402B30h

l0000000000402D75:
	mov	edi,10h
	call	410C40h
	mov	rdx,[000000000061B100]                                 ; [rip+0021837A]
	mov	qword ptr [rax],+413864h
	mov	edi,10h
	mov	[000000000061B100],rax                                 ; [rip+00218367]
	mov	[rax+8h],rdx
	call	410C40h
	mov	rdx,[000000000061B100]                                 ; [rip+00218357]
	mov	qword ptr [rax],+413863h
	mov	[rax+8h],rdx
	mov	[000000000061B100],rax                                 ; [rip+00218345]
	jmp	402B30h

l0000000000402DC0:
	cmp	[000000000061B108],0h                                  ; [rip+00218341]
	jnz	402B30h

l0000000000402DCD:
	mov	[000000000061B108],1h                                  ; [rip+00218331]
	jmp	402B30h

l0000000000402DDC:
	cmp	[000000000061B150],0h                                  ; [rip+0021836D]
	jz	402B30h

l0000000000402DE9:
	mov	[000000000061B150],1h                                  ; [rip+0021835D]
	jmp	402B30h

l0000000000402DF8:
	xor	edi,edi
	call	409750h

l0000000000402DFF:
	mov	eax,[000000000061A56C]                                 ; [rip+00217767]
	mov	rcx,[000000000061A570]                                 ; [rip+00217764]
	cmp	eax,1h
	jz	403BF3h

l0000000000402E15:
	cmp	eax,2h
	mov	esi,41380Fh
	mov	eax,41380Eh
	cmovnz	rsi,rax

l0000000000402E26:
	mov	rdi,[000000000061A610]                                 ; [rip+002177E3]
	mov	qword ptr [rsp],+0h
	mov	r9d,4138BDh
	mov	r8d,4138CDh
	mov	edx,4137FCh
	xor	eax,eax
	call	410B30h
	xor	edi,edi
	call	4027F0h

l0000000000402E54:
	mov	r12,[000000000061A640]                                 ; [rip+002177E5]
	jmp	402B30h

l0000000000402E60:
	mov	r9,[000000000061A578]                                  ; [rip+00217711]
	mov	rsi,[000000000061A640]                                 ; [rip+002177D2]
	mov	r8d,4h
	mov	ecx,412F50h
	mov	edx,412F80h
	mov	edi,413883h
	call	40A120h
	mov	eax,[412F50h+rax*4]
	mov	[000000000061B14C],eax                                 ; [rip+002182B7]
	jmp	402B30h

l0000000000402E9A:
	mov	r9,[000000000061A578]                                  ; [rip+002176D7]
	mov	rsi,[000000000061A640]                                 ; [rip+00217798]
	mov	r8d,4h
	mov	ecx,412FB0h
	mov	edx,412FE0h
	mov	edi,41387Ch
	mov	r13d,1h
	call	40A120h
	mov	eax,[412FB0h+rax*4]
	mov	[000000000061B148],eax                                 ; [rip+00218273]
	jmp	402B30h

l0000000000402EDA:
	mov	[000000000061B140],90h                                 ; [rip+0021825C]
	mov	[000000000061B134],90h                                 ; [rip+00218246]
	mov	[000000000061B138],+1h                                 ; [rip+0021823F]
	mov	[000000000061A560],+1h                                 ; [rip+0021765C]
	jmp	402B30h

l0000000000402F09:
	mov	[000000000061B0F0],0h                                  ; [rip+002181E0]
	jmp	402B30h

l0000000000402F15:
	mov	r9,[000000000061A578]                                  ; [rip+0021765C]
	mov	rsi,[000000000061A640]                                 ; [rip+0021771D]
	mov	r8d,4h
	mov	ecx,416460h
	mov	edx,416480h
	mov	edi,4138ADh
	call	40A120h
	mov	esi,[416460h+rax*4]
	xor	edi,edi
	call	40E640h
	jmp	402B30h

l0000000000402F50:
	mov	r9,[000000000061A578]                                  ; [rip+00217621]
	mov	rsi,[000000000061A640]                                 ; [rip+002176E2]
	mov	r8d,4h
	mov	ecx,4136B0h
	mov	edx,4136C0h
	mov	edi,41389Bh
	call	40A120h
	mov	eax,[4136B0h+rax*4]
	mov	[000000000061B12C],eax                                 ; [rip+002181A7]
	jmp	402B30h

l0000000000402F8A:
	mov	edi,10h
	call	410C40h
	mov	rdx,[000000000061A640]                                 ; [rip+002176A5]
	mov	[rax],rdx
	mov	rdx,[000000000061B0F8]                                 ; [rip+00218153]
	mov	[000000000061B0F8],rax                                 ; [rip+0021814C]
	mov	[rax+8h],rdx
	jmp	402B30h

l0000000000402FB5:
	mov	[000000000061B10C],1h                                  ; [rip+00218150]
	jmp	402B30h

l0000000000402FC1:
	mov	[000000000061B150],0h                                  ; [rip+00218185]
	mov	r12d,413813h
	jmp	402B30h

l0000000000402FD6:
	mov	r9,[000000000061A578]                                  ; [rip+0021759B]
	mov	rsi,[000000000061A640]                                 ; [rip+0021765C]
	mov	r8d,4h
	mov	ecx,413010h
	mov	edx,413040h
	mov	edi,41388Ah
	call	40A120h
	mov	eax,[413010h+rax*4]
	mov	[000000000061B150],eax                                 ; [rip+00218145]
	jmp	402B30h

l0000000000403010:
	mov	[000000000061B12C],2h                                  ; [rip+00218112]
	jmp	402B30h

l000000000040301F:
	mov	[000000000061B110],4h                                  ; [rip+002180E7]
	jmp	402B30h

l000000000040302E:
	mov	rsi,[000000000061A640]                                 ; [rip+0021760B]
	test	rsi,rsi
	jz	403A7Ch

l000000000040303E:
	mov	r9,[000000000061A578]                                  ; [rip+00217533]
	mov	r8d,4h
	mov	ecx,412EC0h
	mov	edx,412F00h
	mov	edi,413893h
	call	40A120h
	mov	eax,[412EC0h+rax*4]
	cmp	eax,1h
	jz	403A7Ch

l000000000040306F:
	cmp	eax,2h
	jz	403A6Ah

l0000000000403078:
	mov	[000000000061B129],0h                                  ; [rip+002180AA]
	jmp	402B30h

l0000000000403084:
	mov	rdi,[000000000061A640]                                 ; [rip+002175B5]
	mov	edx,61B138h
	mov	esi,61B140h
	call	40C810h
	test	eax,eax
	jnz	4043C0h

l00000000004030A2:
	mov	eax,[000000000061B140]                                 ; [rip+00218098]
	mov	[000000000061B134],eax                                 ; [rip+00218086]
	mov	rax,[000000000061B138]                                 ; [rip+00218083]
	mov	[000000000061A560],rax                                 ; [rip+002174A4]
	jmp	402B30h

l00000000004030C1:
	mov	[000000000061B146],1h                                  ; [rip+0021807E]
	jmp	402B30h

l00000000004030CD:
	mov	[000000000061B150],3h                                  ; [rip+00218079]
	jmp	402B30h

l00000000004030DC:
	mov	rcx,[rsp+20h]
	mov	rdi,[000000000061A640]                                 ; [rip+00217558]
	xor	r8d,r8d
	xor	edx,edx
	xor	esi,esi
	call	410E90h
	test	eax,eax
	jnz	403100h

l00000000004030F8:
	cmp	qword ptr [rsp+40h],0h
	jnz	403134h

l0000000000403100:
	mov	rdi,[000000000061A640]                                 ; [rip+00217539]
	call	40E930h
	mov	edx,5h
	mov	r15,rax
	mov	esi,41384Ch
	xor	edi,edi
	call	402360h
	mov	rcx,r15
	mov	rdx,rax
	xor	esi,esi
	mov	edi,2h
	xor	eax,eax
	call	402770h

l0000000000403134:
	mov	rax,[rsp+40h]
	mov	[000000000061B0C8],rax                                 ; [rip+00217F88]
	jmp	402B30h

l0000000000403145:
	mov	[000000000061B148],3h                                  ; [rip+00217FF9]
	mov	r13d,1h
	jmp	402B30h

l000000000040315A:
	mov	[000000000061B14C],2h                                  ; [rip+00217FE8]
	jmp	402B30h

l0000000000403169:
	mov	[000000000061B148],4h                                  ; [rip+00217FD5]
	mov	r13d,1h
	jmp	402B30h

l000000000040317E:
	mov	[000000000061B144],1h                                  ; [rip+00217FBF]
	jmp	402B30h

l000000000040318A:
	mov	[000000000061B147],1h                                  ; [rip+00217FB6]
	jmp	402B30h

l0000000000403196:
	mov	[000000000061B0F0],1h                                  ; [rip+00217F53]
	jmp	402B30h

l00000000004031A2:
	mov	[000000000061B12C],1h                                  ; [rip+00217F80]
	jmp	402B30h

l00000000004031B1:
	mov	[000000000061B150],0h                                  ; [rip+00217F95]
	mov	[000000000061A568],0h                                  ; [rip+002173A6]
	jmp	402B30h

l00000000004031C7:
	mov	[000000000061B150],4h                                  ; [rip+00217F7F]
	jmp	402B30h

l00000000004031D6:
	mov	rbx,[000000000061A650]                                 ; [rip+00217473]
	mov	esi,415BF8h
	xor	edi,edi
	mov	edx,5h
	call	402360h
	mov	rsi,rbx
	mov	rdi,rax
	call	402520h

l00000000004031F9:
	mov	edi,2h
	call	409750h

l0000000000403203:
	mov	esi,5h
	xor	edi,edi
	mov	[000000000061B150],2h                                  ; [rip+00217F3C]
	call	40E640h
	jmp	4029C5h

l000000000040321E:
	cmp	[000000000061B138],0h                                  ; [rip+00217F12]
	jz	40398Ah

l000000000040322C:
	mov	rdx,[000000000061B0C8]                                 ; [rip+00217E95]
	mov	eax,1h
	cmp	rdx,2h
	ja	4037FBh

l0000000000403242:
	xor	edi,edi
	mov	[000000000061B020],rax                                 ; [rip+00217DD5]
	call	40E600h
	mov	rdi,rax
	mov	[000000000061B0E8],rax                                 ; [rip+00217E8E]
	call	40E630h
	cmp	eax,5h
	jz	40426Ch

l0000000000403268:
	mov	eax,[000000000061B12C]                                 ; [rip+00217EBE]
	cmp	eax,1h
	jbe	4032A9h

l0000000000403273:
	lea	r14,[rax+4138EDh]
	sub	rax,2h
	movzx	eax,byte ptr [rax+4138EFh]
	test	al,al
	jz	4032A9h

l0000000000403289:
	mov	rdi,[000000000061B0E8]                                 ; [rip+00217E58]
	add	r14,1h
	movsx	esi,al
	mov	edx,1h
	call	40E650h
	movzx	eax,byte ptr [r14]
	test	al,al
	jnz	403289h

l00000000004032A9:
	xor	edi,edi
	call	40E600h
	mov	edx,1h
	mov	esi,3Ah
	mov	rdi,rax
	mov	[000000000061B0E0],rax                                 ; [rip+00217E1C]
	call	40E650h
	cmp	[000000000061B130],0h                                  ; [rip+00217E60]
	jz	4032E2h

l00000000004032D2:
	cmp	[000000000061B150],0h                                  ; [rip+00217E77]
	jz	4032E2h

l00000000004032DB:
	mov	[000000000061B130],0h                                  ; [rip+00217E4E]

l00000000004032E2:
	mov	eax,[000000000061B14C]                                 ; [rip+00217E64]
	sub	eax,1h
	cmp	eax,1h
	jbe	403965h

l00000000004032F4:
	cmp	[000000000061B150],0h                                  ; [rip+00217E55]
	jz	40380Dh

l0000000000403301:
	cmp	[000000000061B129],0h                                  ; [rip+00217E21]
	mov	r12d,[000000000061A620]                                ; [rip+00217311]
	jnz	403AC5h

l0000000000403315:
	cmp	[000000000061B110],1h                                  ; [rip+00217DF4]
	jz	403A39h

l0000000000403322:
	cmp	[000000000061B10E],0h                                  ; [rip+00217DE5]
	jnz	4039EFh

l000000000040332F:
	mov	eax,[000000000061B148]                                 ; [rip+00217E13]
	cmp	eax,4h
	jz	40377Ah

l000000000040333E:
	cmp	eax,2h
	jz	40377Ah

l0000000000403347:
	cmp	[000000000061B150],0h                                  ; [rip+00217E02]
	jz	40377Ah

l0000000000403354:
	cmp	[000000000061B17D],0h                                  ; [rip+00217E22]
	jnz	40377Ah

l0000000000403361:
	cmp	[000000000061B144],0h                                  ; [rip+00217DDC]
	jnz	40377Ah

l000000000040336E:
	cmp	[000000000061B10E],0h                                  ; [rip+00217D99]
	mov	[000000000061B0C1],0h                                  ; [rip+00217D45]
	mov	eax,1h
	jnz	4033A0h

l0000000000403383:
	cmp	[000000000061B129],0h                                  ; [rip+00217D9F]
	jnz	4033A0h

l000000000040338C:
	cmp	[000000000061B12C],0h                                  ; [rip+00217D99]
	jnz	4033A0h

l0000000000403395:
	cmp	[000000000061B10C],0h                                  ; [rip+00217D70]
	jnz	4033A0h

l000000000040339E:
	xor	eax,eax

l00000000004033A0:
	mov	[000000000061B0C0],al                                  ; [rip+00217D1A]
	and	[000000000061B0C0],1h                                  ; [rip+00217D13]
	cmp	[000000000061B130],0h                                  ; [rip+00217D7C]
	jz	4033E8h

l00000000004033B6:
	mov	r8d,4021F0h
	mov	ecx,402640h
	xor	edx,edx
	xor	esi,esi
	mov	edi,61AFC0h
	call	4023F0h
	mov	r8d,4021F0h
	mov	ecx,402640h
	xor	edx,edx
	xor	esi,esi
	mov	edi,61AF60h
	call	4023F0h

l00000000004033E8:
	mov	r13d,ebx
	mov	edi,4B00h
	mov	[000000000061B1B8],+64h                                ; [rip+00217DBD]
	sub	r13d,r12d
	call	410C40h
	mov	[000000000061B1B0],+0h                                 ; [rip+00217DA2]
	mov	[000000000061B1C0],rax                                 ; [rip+00217DAB]
	call	404DD0h
	test	r13d,r13d
	jle	4042A0h

l0000000000403423:
	movsxd	rax,r12d
	lea	rbp,[rbp+rax*8+0h]

l000000000040342B:
	mov	rdi,[rbp+0h]
	xor	esi,esi
	add	r12d,1h
	mov	ecx,416919h
	mov	edx,1h
	add	rbp,8h
	call	407EA0h
	cmp	ebx,r12d
	jg	40342Bh

l000000000040344D:
	cmp	[000000000061B1B0],0h                                  ; [rip+00217D5B]
	jnz	404200h

l000000000040345B:
	mov	rax,[000000000061B190]                                 ; [rip+00217D2E]
	sub	r13d,1h
	mov	[rsp+18h],rax
	jg	4034D2h

l000000000040346D:
	jmp	404385h
0000000000403472       66 0F 1F 44 00 00                           f..D..        

l0000000000403478:
	mov	edx,5h
	mov	esi,415CE8h
	xor	edi,edi
	call	402360h
	movzx	edi,byte ptr [rsp+2Fh]
	mov	rdx,r14
	mov	rsi,rax
	call	405810h
	mov	rdi,r13
	call	4024E0h

l00000000004034A1:
	mov	rbx,[rsp+18h]
	mov	rdi,[rbx]
	call	4021F0h
	mov	rdi,[rbx+8h]
	call	4021F0h
	mov	rdi,rbx
	call	4021F0h
	mov	[000000000061B0D0],1h                                  ; [rip+00217C0A]

l00000000004034C6:
	mov	rax,[000000000061B190]                                 ; [rip+00217CC3]
	mov	[rsp+18h],rax

l00000000004034D2:
	cmp	qword ptr [rsp+18h],0h
	jz	4040B5h

l00000000004034DE:
	mov	rcx,[rsp+18h]
	cmp	[000000000061B1C8],0h                                  ; [rip+00217CDD]
	mov	rax,[rcx+18h]
	mov	[000000000061B190],rax                                 ; [rip+00217C9A]
	jz	403FB5h

l00000000004034FC:
	mov	r14,[rcx]
	test	r14,r14
	jz	403FC2h

l0000000000403508:
	mov	rax,[rsp+18h]
	movzx	ecx,byte ptr [rax+10h]
	mov	rbx,[rax+8h]
	mov	[rsp+2Fh],cl
	call	402230h
	mov	rdi,r14
	mov	dword ptr [rax],0h
	mov	r12,rax
	call	402320h
	test	rax,rax
	mov	r13,rax
	jz	404195h

l000000000040353B:
	cmp	[000000000061B1C8],0h                                  ; [rip+00217C85]
	jz	4035FFh

l0000000000403549:
	mov	rdi,rax
	call	402570h
	test	eax,eax
	mov	rdx,[rsp+20h]
	js	403E95h

l000000000040355E:
	mov	esi,eax
	mov	edi,1h
	call	402680h
	shr	eax,1Fh

l000000000040356D:
	test	al,al
	jnz	403478h

l0000000000403575:
	mov	rcx,[rsp+48h]
	mov	rdx,[rsp+40h]
	mov	edi,10h
	mov	[rsp+10h],rcx
	mov	[rsp+8h],rdx
	call	410C40h
	mov	rcx,[rsp+10h]
	mov	rdx,[rsp+8h]
	mov	rsi,rax
	mov	rdi,[000000000061B1C8]                                 ; [rip+00217C21]
	mov	rbp,rax
	mov	[rax],rcx
	mov	[rax+8h],rdx
	call	40BB50h
	test	rax,rax
	jz	4043BBh

l00000000004035BF:
	cmp	rbp,rax
	jnz	403EE2h

l00000000004035C8:
	mov	rax,[000000000061AF18]                                 ; [rip+00217949]
	mov	rdx,[000000000061AF20]                                 ; [rip+0021794A]
	sub	rdx,rax
	cmp	rdx,0Fh
	jle	404064h

l00000000004035E3:
	lea	rdx,[rax+10h]
	mov	[000000000061AF18],rdx                                 ; [rip+0021792A]
	mov	rdx,[rsp+40h]
	mov	[rax+8h],rdx
	mov	rdx,[rsp+48h]
	mov	[rax],rdx

l00000000004035FF:
	cmp	[000000000061B10E],0h                                  ; [rip+00217B08]
	jnz	403615h

l0000000000403608:
	cmp	[000000000061B0D0],0h                                  ; [rip+00217AC1]
	jz	4036D6h

l0000000000403615:
	cmp	[000000000061A3C0],0h                                  ; [rip+00216DA4]
	jnz	403646h

l000000000040361E:
	mov	rdi,[000000000061A610]                                 ; [rip+00216FEB]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	4045E4h

l0000000000403633:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],0Ah

l000000000040363E:
	add	[000000000061B018],1h                                  ; [rip+002179D2]

l0000000000403646:
	cmp	[000000000061B130],0h                                  ; [rip+00217AE3]
	mov	[000000000061A3C0],0h                                  ; [rip+00216D6C]
	jnz	403F2Eh

l000000000040365A:
	test	rbx,rbx
	mov	rdx,[000000000061B0E0]                                 ; [rip+00217A7C]
	mov	rdi,[000000000061A610]                                 ; [rip+00216FA5]
	cmovz	rbx,r14

l000000000040366F:
	xor	ecx,ecx
	mov	rsi,rbx
	call	4052D0h
	add	[000000000061B018],rax                                 ; [rip+00217998]
	cmp	[000000000061B130],0h                                  ; [rip+00217AA9]
	jz	4036B3h

l0000000000403689:
	mov	rax,[000000000061AF78]                                 ; [rip+002178E8]
	lea	rdx,[rax+8h]
	cmp	[000000000061AF80],rdx                                 ; [rip+002178E5]
	jc	40409Ah

l00000000004036A1:
	mov	rdx,[000000000061B018]                                 ; [rip+00217970]
	mov	[rax],rdx
	add	[000000000061AF78],8h                                  ; [rip+002178C5]

l00000000004036B3:
	mov	rcx,[000000000061A610]                                 ; [rip+00216F56]
	mov	edx,2h
	mov	esi,1h
	mov	edi,41393Bh
	call	4026C0h
	add	[000000000061B018],2h                                  ; [rip+00217942]

l00000000004036D6:
	call	404DD0h
	movzx	eax,byte ptr [rsp+2Fh]
	mov	qword ptr [rsp+8h],+0h
	mov	[rsp+10h],eax
	nop	dword ptr [rax]

l00000000004036F0:
	mov	dword ptr [r12],0h
	mov	rdi,r13
	call	402620h
	test	rax,rax
	mov	rbp,rax
	jz	403CC0h

l000000000040370C:
	lea	rbx,[rax+13h]
	mov	eax,[000000000061B108]                                 ; [rip+002179F2]
	cmp	eax,2h
	jz	403C78h

l000000000040371F:
	cmp	byte ptr [rbp+13h],2Eh
	jz	403C58h

l0000000000403729:
	test	eax,eax
	jnz	403C78h

l0000000000403731:
	mov	r15,[000000000061B0F8]                                 ; [rip+002179C0]
	test	r15,r15
	jnz	403755h

l000000000040373D:
	jmp	403C78h
0000000000403742       66 0F 1F 44 00 00                           f..D..        

l0000000000403748:
	mov	r15,[r15+8h]
	test	r15,r15
	jz	403C78h

l0000000000403755:
	mov	rdi,[r15]
	mov	edx,4h
	mov	rsi,rbx
	call	402470h
	test	eax,eax
	jnz	403748h

l0000000000403769:
	nop	dword ptr [rax+0h]

l0000000000403770:
	call	406490h
	jmp	4036F0h

l000000000040377A:
	mov	[000000000061B0C1],1h                                  ; [rip+00217940]
	jmp	40339Eh

l0000000000403786:
	mov	rdi,r12
	call	40E930h
	xor	edi,edi
	mov	r12,rax
	mov	edx,5h
	mov	esi,415B00h
	call	402360h
	mov	rcx,r12
	mov	rdx,rax
	xor	esi,esi
	xor	edi,edi
	xor	eax,eax
	call	402770h
	jmp	402A7Fh

l00000000004037B8:
	xor	r8d,r8d
	xor	edx,edx
	xor	esi,esi
	mov	rcx,rax
	mov	rdi,r12
	call	410E90h
	test	eax,eax
	jnz	403A93h

l00000000004037D2:
	mov	rax,[rsp+40h]
	test	rax,rax
	jz	403A93h

l00000000004037E0:
	mov	[000000000061B0C8],rax                                 ; [rip+002178E1]
	jmp	402AB1h

l00000000004037EC:
	mov	[000000000061B150],1h                                  ; [rip+0021795A]
	jmp	4029C5h

l00000000004037FB:
	mov	rax,rdx
	mov	ecx,3h
	xor	edx,edx
	div	rcx
	jmp	403242h

l000000000040380D:
	test	r12,r12
	jz	4045C6h

l0000000000403816:
	mov	r14d,412CA0h
	mov	r13d,6h
	jmp	40383Eh
0000000000403824             0F 1F 40 00                             ..@.        

l0000000000403828:
	mov	edi,2h
	call	40AB70h
	test	al,al
	jz	403301h

l000000000040383A:
	add	r12,6h

l000000000040383E:
	mov	rsi,r12
	mov	rdi,r14
	mov	rcx,r13

l0000000000403847:
	rep cmpsb

l0000000000403849:
	jz	403828h

l000000000040384B:
	cmp	byte ptr [r12],2Bh
	jz	4043D9h

l0000000000403856:
	mov	ecx,4h
	mov	edx,4136F0h
	mov	esi,413700h
	mov	rdi,r12
	call	409E50h
	test	rax,rax
	js	404564h

l0000000000403876:
	cmp	rax,1h
	jz	404549h

l0000000000403880:
	jle	404465h

l0000000000403886:
	cmp	rax,2h
	jz	4045F3h

l0000000000403890:
	cmp	rax,3h
	jnz	4038A8h

l0000000000403896:
	mov	edi,2h
	call	40AB70h
	test	al,al
	jnz	40460Eh

l00000000004038A8:
	mov	rdi,[000000000061A3D0]                                 ; [rip+00216B21]
	mov	esi,413766h
	call	402860h
	test	rax,rax
	jz	404484h

l00000000004038C2:
	mov	[000000000061A748],+5h                                 ; [rip+00216E7B]

l00000000004038CD:
	mov	r14,[000000000061A748]                                 ; [rip+00216E74]
	mov	r13d,61A760h
	mov	[000000000061A748],+0h                                 ; [rip+00216E63]
	mov	r12d,2000Eh

l00000000004038EB:
	mov	edi,r12d
	mov	[rsp+40h],r14
	call	402660h
	mov	rcx,[rsp+20h]
	xor	r9d,r9d
	xor	r8d,r8d
	mov	edx,0A1h
	mov	rsi,r13
	mov	rdi,rax
	call	40CDC0h
	cmp	rax,+0A0h
	ja	404356h

l000000000040391F:
	mov	rax,[rsp+40h]
	cmp	[000000000061A748],rax                                 ; [rip+00216E1D]
	cmovnc	rax,[000000000061A748]                              ; [rip+00216E15]

l0000000000403933:
	add	r12d,1h
	add	r13,+0A1h
	cmp	r12d,2001Ah
	mov	[000000000061A748],rax                                 ; [rip+00216DFC]
	jnz	4038EBh

l000000000040394E:
	cmp	rax,r14
	jc	4038CDh

l0000000000403957:
	test	rax,rax
	jnz	403301h

l0000000000403960:
	jmp	404361h

l0000000000403965:
	test	r13b,r13b
	jnz	4032F4h

l000000000040396E:
	cmp	[000000000061B150],0h                                  ; [rip+002177DB]
	jz	40380Dh

l000000000040397B:
	mov	[000000000061B148],4h                                  ; [rip+002177C3]
	jmp	4032F4h

l000000000040398A:
	mov	edi,4138E1h
	call	4021C0h
	mov	edx,61B138h
	mov	r15,rax
	mov	esi,61B140h
	mov	rdi,rax
	call	40C810h
	test	r15,r15
	jz	404531h

l00000000004039B2:
	mov	eax,[000000000061B140]                                 ; [rip+00217788]
	mov	[000000000061B134],eax                                 ; [rip+00217776]
	mov	rax,[000000000061B138]                                 ; [rip+00217773]
	mov	[000000000061A560],rax                                 ; [rip+00216B94]

l00000000004039CC:
	test	r14b,r14b
	jz	40322Ch

l00000000004039D5:
	mov	[000000000061B140],0h                                  ; [rip+00217761]
	mov	[000000000061B138],+400h                               ; [rip+0021774E]
	jmp	40322Ch

l00000000004039EF:
	xor	esi,esi
	mov	r8d,4049D0h
	mov	ecx,404990h
	mov	edx,404980h
	mov	edi,1Eh
	call	40B400h
	test	rax,rax
	mov	[000000000061B1C8],rax                                 ; [rip+002177B3]
	jz	4043BBh

l0000000000403A1B:
	mov	r8d,4021F0h
	mov	ecx,402640h
	xor	edx,edx
	xor	esi,esi
	mov	edi,61AF00h
	call	4023F0h
	jmp	40332Fh

l0000000000403A39:
	cmp	[000000000061B10D],0h                                  ; [rip+002176CD]
	mov	eax,2h
	jnz	403A5Fh

l0000000000403A47:
	cmp	[000000000061B12C],3h                                  ; [rip+002176DE]
	jz	403A5Fh

l0000000000403A50:
	cmp	[000000000061B150],1h                                  ; [rip+002176F9]
	sbb	eax,eax
	and	eax,0FEh
	add	eax,4h

l0000000000403A5F:
	mov	[000000000061B110],eax                                 ; [rip+002176AB]
	jmp	403322h

l0000000000403A6A:
	mov	edi,1h
	call	402280h
	test	eax,eax
	jz	403078h

l0000000000403A7C:
	mov	[000000000061B129],1h                                  ; [rip+002176A6]
	mov	[000000000061B0D8],+0h                                 ; [rip+0021764A]
	jmp	402B30h

l0000000000403A93:
	mov	rdi,r12
	call	40E930h
	xor	edi,edi
	mov	r12,rax
	mov	edx,5h
	mov	esi,415B48h
	call	402360h
	mov	rcx,r12
	mov	rdx,rax
	xor	esi,esi
	xor	edi,edi
	xor	eax,eax
	call	402770h
	jmp	402AB1h

l0000000000403AC5:
	mov	edi,41397Fh
	call	4021C0h
	test	rax,rax
	mov	[rsp+38h],rax
	jz	403AE2h

l0000000000403AD9:
	cmp	byte ptr [rax],0h
	jnz	4044FAh

l0000000000403AE2:
	cmp	[000000000061B129],0h                                  ; [rip+00217640]
	jz	403315h

l0000000000403AEF:
	mov	edi,0Dh
	call	404CD0h
	test	al,al
	jnz	403B2Bh

l0000000000403AFD:
	mov	edi,0Eh
	call	404CD0h
	test	al,al
	jz	403B14h

l0000000000403B0B:
	cmp	[000000000061B198],0h                                  ; [rip+00217686]
	jnz	403B2Bh

l0000000000403B14:
	mov	edi,0Ch
	call	404CD0h
	test	al,al
	jz	403B32h

l0000000000403B22:
	cmp	[000000000061B150],0h                                  ; [rip+00217627]
	jnz	403B32h

l0000000000403B2B:
	mov	[000000000061B115],1h                                  ; [rip+002175E3]

l0000000000403B32:
	mov	edi,1h
	call	402600h
	test	eax,eax
	js	403315h

l0000000000403B44:
	mov	edi,61B040h
	xor	r13d,r13d
	call	4025A0h

l0000000000403B51:
	mov	r14d,[r13+412CC0h]
	mov	rdx,[rsp+20h]
	xor	esi,esi
	mov	edi,r14d
	call	402290h
	cmp	qword ptr [rsp+40h],1h
	jz	403B7Ch

l0000000000403B6F:
	mov	esi,r14d
	mov	edi,61B040h
	call	402850h

l0000000000403B7C:
	add	r13,4h
	cmp	r13,30h
	jnz	403B51h

l0000000000403B86:
	lea	rdi,[rsp+48h]
	mov	esi,61B040h
	mov	ecx,20h

l0000000000403B95:
	rep movsd

l0000000000403B97:
	mov	dword ptr [rsp+0C8h],10000000h
	xor	r13b,r13b
	mov	r14d,4049B0h

l0000000000403BAB:
	mov	r15d,[r13+412CC0h]
	mov	edi,61B040h
	mov	esi,r15d
	call	4027E0h
	test	eax,eax
	jz	403BE4h

l0000000000403BC3:
	mov	rsi,[rsp+20h]
	cmp	r15d,14h
	mov	eax,4057F0h
	cmovnz	rax,r14

l0000000000403BD5:
	mov	edi,r15d
	xor	edx,edx
	mov	[rsp+40h],rax
	call	402290h

l0000000000403BE4:
	add	r13,4h
	cmp	r13,30h
	jnz	403BABh

l0000000000403BEE:
	jmp	403315h

l0000000000403BF3:
	mov	esi,413807h
	jmp	402E26h

l0000000000403BFD:
	mov	rdi,[000000000061A640]                                 ; [rip+00216A3C]
	call	40E930h
	xor	edi,edi
	mov	r15,rax
	mov	edx,5h
	mov	esi,413867h
	call	402360h
	mov	rcx,r15
	mov	rdx,rax
	xor	esi,esi
	mov	edi,2h
	xor	eax,eax
	call	402770h
	jmp	402CA5h

l0000000000403C36:
	mov	edi,1h
	call	402280h
	cmp	eax,1h
	sbb	eax,eax
	add	eax,2h
	mov	[000000000061B150],eax                                 ; [rip+00217502]
	jmp	402BFBh
0000000000403C53          0F 1F 44 00 00                            ..D..        

l0000000000403C58:
	test	eax,eax
	jz	403770h

l0000000000403C60:
	xor	eax,eax
	cmp	byte ptr [rbp+14h],2Eh
	setz	al
	cmp	byte ptr [rbp+rax+14h],0h
	jz	403770h

l0000000000403C74:
	nop	dword ptr [rax+0h]

l0000000000403C78:
	mov	r15,[000000000061B100]                                 ; [rip+00217481]
	test	r15,r15
	jnz	403C9Dh

l0000000000403C84:
	jmp	403E20h
0000000000403C89                            0F 1F 80 00 00 00 00          .......

l0000000000403C90:
	mov	r15,[r15+8h]
	test	r15,r15
	jz	403E20h

l0000000000403C9D:
	mov	rdi,[r15]
	mov	edx,4h
	mov	rsi,rbx
	call	402470h
	test	eax,eax
	jnz	403C90h

l0000000000403CB1:
	jmp	403770h
0000000000403CB6                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000403CC0:
	mov	edx,[r12]
	test	edx,edx
	jz	403CF3h

l0000000000403CC8:
	xor	edi,edi
	mov	edx,5h
	mov	esi,4139B1h
	call	402360h
	mov	edi,[rsp+10h]
	mov	rdx,r14
	mov	rsi,rax
	call	405810h
	cmp	dword ptr [r12],4Bh
	jz	403770h

l0000000000403CF3:
	mov	rdi,r13
	call	4024E0h
	test	eax,eax
	jnz	403EBCh

l0000000000403D03:
	call	404E80h
	cmp	[000000000061B10E],0h                                  ; [rip+002173FF]
	jnz	403EAAh

l0000000000403D15:
	mov	eax,[000000000061B150]                                 ; [rip+00217435]
	test	eax,eax
	jz	403D2Ch

l0000000000403D1F:
	cmp	[000000000061B144],0h                                  ; [rip+0021741E]
	jz	403E00h

l0000000000403D2C:
	cmp	[000000000061B130],0h                                  ; [rip+002173FD]
	jnz	403F8Dh

l0000000000403D39:
	mov	edx,5h
	xor	edi,edi
	mov	esi,4139DBh
	call	402360h
	mov	rsi,[000000000061A610]                                 ; [rip+002168BF]
	mov	rbx,rax
	mov	rdi,rax
	call	402520h
	mov	rdi,rbx
	call	402380h
	mov	rdi,[000000000061A610]                                 ; [rip+002168A5]
	add	[000000000061B018],rax                                 ; [rip+002172A6]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	4044EBh

l0000000000403D80:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],20h

l0000000000403D8B:
	mov	r8,[000000000061B138]                                  ; [rip+002173A6]
	mov	edx,[000000000061B140]                                 ; [rip+002173A8]
	lea	rsi,[rsp+0E0h]
	mov	rdi,[rsp+8h]
	mov	ecx,200h
	add	[000000000061B018],1h                                  ; [rip+00217266]
	call	40BD70h
	mov	rsi,[000000000061A610]                                 ; [rip+00216852]
	mov	rbx,rax
	mov	rdi,rax
	call	402520h
	mov	rdi,rbx
	call	402380h
	mov	rdi,[000000000061A610]                                 ; [rip+00216838]
	add	[000000000061B018],rax                                 ; [rip+00217239]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	4044DCh

l0000000000403DED:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],0Ah

l0000000000403DF8:
	add	[000000000061B018],1h                                  ; [rip+00217218]

l0000000000403E00:
	cmp	[000000000061B1B0],0h                                  ; [rip+002173A8]
	jz	4034A1h

l0000000000403E0E:
	call	4079F0h
	jmp	4034A1h
0000000000403E18                         0F 1F 84 00 00 00 00 00         ........

l0000000000403E20:
	movzx	eax,byte ptr [rbp+12h]
	xor	esi,esi
	sub	eax,1h
	cmp	al,0Dh
	ja	403E37h

l0000000000403E2D:
	movzx	eax,al
	mov	esi,[412C00h+rax*4]

l0000000000403E37:
	xor	edx,edx
	mov	rcx,r14
	mov	rdi,rbx
	call	407EA0h
	add	[rsp+8h],rax
	cmp	[000000000061B150],1h                                  ; [rip+00217300]
	jnz	403770h

l0000000000403E56:
	cmp	[000000000061B148],0FFh                                ; [rip+002172EB]
	jnz	403770h

l0000000000403E63:
	cmp	[000000000061B144],0h                                  ; [rip+002172DA]
	jnz	403770h

l0000000000403E70:
	cmp	[000000000061B10E],0h                                  ; [rip+00217297]
	jnz	403770h

l0000000000403E7D:
	call	404E80h
	call	4079F0h
	call	404DD0h
	nop	dword ptr [rax+0h]
	jmp	403770h

l0000000000403E95:
	mov	rsi,r14
	mov	edi,1h
	call	402610h
	shr	eax,1Fh
	jmp	40356Dh

l0000000000403EAA:
	movzx	esi,byte ptr [rsp+2Fh]
	mov	rdi,r14
	call	405090h
	jmp	403D15h

l0000000000403EBC:
	xor	edi,edi
	mov	edx,5h
	mov	esi,4139C6h
	call	402360h
	movzx	edi,byte ptr [rsp+2Fh]
	mov	rdx,r14
	mov	rsi,rax
	call	405810h
	jmp	403D03h

l0000000000403EE2:
	mov	rdi,rbp
	call	4021F0h
	mov	rdi,r14
	call	40EAB0h
	mov	edx,5h
	mov	rbx,rax
	mov	esi,415D10h
	xor	edi,edi
	call	402360h
	mov	rcx,rbx
	mov	rdx,rax
	xor	esi,esi
	xor	edi,edi
	xor	eax,eax
	call	402770h
	mov	rdi,r13
	call	4024E0h
	mov	[000000000061B030],2h                                  ; [rip+00217107]
	jmp	4034A1h

l0000000000403F2E:
	mov	rcx,[000000000061A610]                                 ; [rip+002166DB]
	mov	edx,2h
	mov	esi,1h
	mov	edi,413771h
	call	4026C0h
	add	[000000000061B018],2h                                  ; [rip+002170C7]
	cmp	[000000000061B130],0h                                  ; [rip+002171D8]
	jz	40365Ah

l0000000000403F5E:
	mov	rax,[000000000061AF78]                                 ; [rip+00217013]
	lea	rdx,[rax+8h]
	cmp	[000000000061AF80],rdx                                 ; [rip+00217010]
	jc	40407Fh

l0000000000403F76:
	mov	rdx,[000000000061B018]                                 ; [rip+0021709B]
	mov	[rax],rdx
	add	[000000000061AF78],8h                                  ; [rip+00216FF0]
	jmp	40365Ah

l0000000000403F8D:
	mov	rcx,[000000000061A610]                                 ; [rip+0021667C]
	mov	edx,2h
	mov	esi,1h
	mov	edi,413771h
	call	4026C0h
	add	[000000000061B018],2h                                  ; [rip+00217068]
	jmp	403D39h

l0000000000403FB5:
	mov	rax,[rsp+18h]
	mov	r14,[rax]
	jmp	403508h

l0000000000403FC2:
	mov	rax,[000000000061AF18]                                 ; [rip+00216F4F]
	mov	rdx,rax
	sub	rdx,[000000000061AF10]                                 ; [rip+00216F3D]
	cmp	edx,0Fh
	jbe	4044C3h

l0000000000403FDC:
	mov	rdx,[000000000061AF20]                                 ; [rip+00216F3D]
	sub	rdx,rax
	cmp	rdx,0F0h
	jge	404002h

l0000000000403FEC:
	mov	esi,0FFFFFFF0h
	mov	edi,61AF00h
	call	402720h
	mov	rax,[000000000061AF18]                                 ; [rip+00216F16]

l0000000000404002:
	lea	rdx,[rax-10h]
	mov	rsi,[rsp+20h]
	mov	rdi,[000000000061B1C8]                                 ; [rip+002171B6]
	mov	[000000000061AF18],rdx                                 ; [rip+00216EFF]
	mov	rdx,[rax-10h]
	mov	rax,[rax-8h]
	mov	[rsp+40h],rdx
	mov	[rsp+48h],rax
	call	40BB90h
	test	rax,rax
	jz	404287h

l0000000000404039:
	mov	rdi,rax
	call	4021F0h
	mov	rbx,[rsp+18h]
	mov	rdi,[rbx]
	call	4021F0h
	mov	rdi,[rbx+8h]
	call	4021F0h
	mov	rdi,rbx
	call	4021F0h
	jmp	4034C6h

l0000000000404064:
	mov	esi,10h
	mov	edi,61AF00h
	call	402720h
	mov	rax,[000000000061AF18]                                 ; [rip+00216E9E]
	jmp	4035E3h

l000000000040407F:
	mov	esi,8h
	mov	edi,61AF60h
	call	402720h
	mov	rax,[000000000061AF78]                                 ; [rip+00216EE3]
	jmp	403F76h

l000000000040409A:
	mov	esi,8h
	mov	edi,61AF60h
	call	402720h
	mov	rax,[000000000061AF78]                                 ; [rip+00216EC8]
	jmp	4036A1h

l00000000004040B5:
	cmp	[000000000061B129],0h                                  ; [rip+0021706D]
	jz	404152h

l00000000004040C2:
	cmp	[000000000061B128],0h                                  ; [rip+0021705F]
	jz	4040EDh

l00000000004040CB:
	cmp	[000000000061A3E0],2h                                  ; [rip+0021630D]
	jz	4042CBh

l00000000004040D9:
	mov	edi,61A3E0h
	call	406440h
	mov	edi,61A3F0h
	call	406440h

l00000000004040ED:
	mov	rdi,[000000000061A610]                                 ; [rip+0021651C]
	mov	ebx,412CC0h
	call	402820h
	jmp	40410Dh

l0000000000404100:
	add	rbx,4h
	cmp	rbx,+412CF0h
	jz	40412Ah

l000000000040410D:
	mov	ebp,[rbx]
	mov	edi,61B040h
	mov	esi,ebp
	call	4027E0h
	test	eax,eax
	jz	404100h

l000000000040411F:
	xor	esi,esi
	mov	edi,ebp
	call	402560h
	jmp	404100h

l000000000040412A:
	mov	ebx,[000000000061B034]                                 ; [rip+00216F04]
	test	ebx,ebx
	jz	404143h

l0000000000404134:
	mov	edi,13h
	call	4021E0h
	sub	ebx,1h
	jnz	404134h

l0000000000404143:
	mov	edi,[000000000061B038]                                 ; [rip+00216EEF]
	test	edi,edi
	jz	404152h

l000000000040414D:
	call	4021E0h

l0000000000404152:
	cmp	[000000000061B130],0h                                  ; [rip+00216FD7]
	jnz	40430Ch

l000000000040415F:
	mov	rbx,[000000000061B1C8]                                 ; [rip+00217062]
	test	rbx,rbx
	jz	4041F5h

l000000000040416F:
	mov	rdi,rbx
	call	40AFB0h
	test	rax,rax
	jz	4041EDh

l000000000040417C:
	mov	ecx,412CA7h
	mov	edx,5DCh
	mov	esi,413736h
	mov	edi,415D68h
	call	402450h

l0000000000404195:
	xor	edi,edi
	mov	edx,5h
	mov	esi,413998h
	call	402360h
	movzx	edi,byte ptr [rsp+2Fh]
	mov	rdx,r14
	mov	rsi,rax
	call	405810h
	jmp	4034A1h

l00000000004041BB:
	mov	rdi,r12
	call	40E930h
	xor	edi,edi
	mov	r12,rax
	mov	edx,5h
	mov	esi,415B88h
	call	402360h
	mov	rcx,r12
	mov	rdx,rax
	xor	esi,esi
	xor	edi,edi
	xor	eax,eax
	call	402770h
	jmp	402B22h

l00000000004041ED:
	mov	rdi,rbx
	call	40B640h

l00000000004041F5:
	mov	edi,[000000000061B030]                                 ; [rip+00216E35]
	call	4027F0h

l0000000000404200:
	call	404E80h
	cmp	[000000000061B10D],0h                                  ; [rip+00216F01]
	jz	404454h

l0000000000404212:
	cmp	[000000000061B1B0],0h                                  ; [rip+00216F96]
	jz	40345Bh

l0000000000404220:
	call	4079F0h
	cmp	[000000000061B190],0h                                  ; [rip+00216F63]
	jz	4044B5h

l0000000000404233:
	mov	rdi,[000000000061A610]                                 ; [rip+002163D6]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	4044A3h

l0000000000404248:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],0Ah

l0000000000404253:
	mov	rax,[000000000061B190]                                 ; [rip+00216F36]
	add	[000000000061B018],1h                                  ; [rip+00216DB6]
	mov	[rsp+18h],rax
	jmp	4034D2h

l000000000040426C:
	mov	rdi,[000000000061B0E8]                                 ; [rip+00216E75]
	mov	edx,1h
	mov	esi,20h
	call	40E650h
	jmp	403268h

l0000000000404287:
	mov	ecx,412CA7h
	mov	edx,59Dh
	mov	esi,413736h
	mov	edi,413992h
	call	402450h

l00000000004042A0:
	cmp	[000000000061B10D],0h                                  ; [rip+00216E66]
	jz	4043A5h

l00000000004042AD:
	mov	ecx,416919h
	mov	edx,1h
	mov	esi,3h
	mov	edi,413990h
	call	407EA0h
	jmp	40344Dh

l00000000004042CB:
	mov	rdi,[000000000061A3E8]                                 ; [rip+00216116]
	mov	edx,2h
	mov	esi,4139E1h
	call	402500h
	test	eax,eax
	jnz	4040D9h

l00000000004042E9:
	cmp	[000000000061A3F0],1h                                  ; [rip+002160FF]
	jnz	4040D9h

l00000000004042F7:
	mov	rax,[000000000061A3F8]                                 ; [rip+002160FA]
	cmp	byte ptr [rax],6Dh
	jnz	4040D9h

l0000000000404307:
	jmp	4040EDh

l000000000040430C:
	mov	esi,61AFC0h
	mov	edi,4139E4h
	call	405630h
	mov	esi,61AF60h
	mov	edi,4139EEh
	call	405630h
	mov	rdi,[000000000061B0E8]                                 ; [rip+00216DB7]
	call	40E630h
	mov	eax,eax
	mov	esi,415D40h
	mov	edi,1h
	mov	rdx,[416480h+rax*8]
	xor	eax,eax
	call	402730h
	jmp	40415Fh

l0000000000404356:
	mov	[000000000061A748],+0h                                 ; [rip+002163E7]

l0000000000404361:
	xor	edi,edi
	mov	edx,5h
	mov	esi,415C30h
	call	402360h
	xor	esi,esi
	mov	rdx,rax
	xor	edi,edi
	xor	eax,eax
	call	402770h
	jmp	403301h

l0000000000404385:
	test	rax,rax
	jz	4034D2h

l000000000040438E:
	cmp	qword ptr [rax+18h],0h
	jnz	4034D2h

l0000000000404399:
	mov	[000000000061B0D0],0h                                  ; [rip+00216D30]
	jmp	4034D2h

l00000000004043A5:
	mov	edx,1h
	xor	esi,esi
	mov	edi,413990h
	call	404D20h
	jmp	40344Dh

l00000000004043BB:
	call	410E50h

l00000000004043C0:
	mov	r8,[000000000061A640]                                  ; [rip+00216279]
	mov	esi,[rsp+38h]
	mov	ecx,413080h
	xor	edx,edx
	mov	edi,eax
	call	4112D0h

l00000000004043D9:
	add	r12,1h
	mov	esi,0Ah
	mov	rdi,r12
	call	4023D0h
	test	rax,rax
	mov	r14,rax
	jz	40444Fh

l00000000004043F2:
	lea	r13,[rax+1h]
	mov	esi,0Ah
	mov	rdi,r13
	call	4023D0h
	test	rax,rax
	jz	404438h

l0000000000404408:
	mov	rdi,r12
	call	40EC10h
	mov	edx,5h
	mov	r15,rax
	mov	esi,413900h
	xor	edi,edi
	call	402360h
	mov	rcx,r15
	mov	rdx,rax
	xor	esi,esi
	mov	edi,2h
	xor	eax,eax
	call	402770h

l0000000000404438:
	mov	byte ptr [r14],0h

l000000000040443C:
	mov	[000000000061A3D0],r12                                 ; [rip+00215F8D]
	mov	[000000000061A3D8],r13                                 ; [rip+00215F8E]
	jmp	4038A8h

l000000000040444F:
	mov	r13,r12
	jmp	40443Ch

l0000000000404454:
	mov	esi,1h
	xor	edi,edi
	call	405090h
	jmp	404212h

l0000000000404465:
	test	rax,rax
	jnz	4038A8h

l000000000040446E:
	mov	[000000000061A3D8],+41394Eh                            ; [rip+00215F5F]
	mov	[000000000061A3D0],+41394Eh                            ; [rip+00215F4C]

l0000000000404484:
	mov	rdi,[000000000061A3D8]                                 ; [rip+00215F4D]
	mov	esi,413766h
	call	402860h
	test	rax,rax
	jnz	4038C2h

l000000000040449E:
	jmp	403301h

l00000000004044A3:
	mov	esi,0Ah
	call	402400h
	nop	dword ptr [rax]
	jmp	404253h

l00000000004044B5:
	mov	qword ptr [rsp+18h],+0h
	jmp	4034D2h

l00000000004044C3:
	mov	ecx,412C38h
	mov	edx,3D5h
	mov	esi,413736h
	mov	edi,415C58h
	call	402450h

l00000000004044DC:
	mov	esi,0Ah
	call	402400h
	jmp	403DF8h

l00000000004044EB:
	mov	esi,20h
	call	402400h
	jmp	403D8Bh

l00000000004044FA:
	mov	rdi,rax
	mov	word ptr [rsp+30h],3F3Fh
	mov	byte ptr [rsp+32h],0h
	xor	r13d,r13d
	call	410E30h
	xor	edx,edx
	mov	[000000000061B118],rax                                 ; [rip+00216BFE]
	mov	[rsp+40h],rax

l000000000040451F:
	cmp	edx,5h
	ja	402985h

l0000000000404528:
	mov	eax,edx
	jmp	qword ptr [412BC8h+rax*8]

l0000000000404531:
	mov	edi,4138E4h
	call	4021C0h
	test	rax,rax
	jnz	4039B2h

l0000000000404544:
	jmp	4039CCh

l0000000000404549:
	mov	[000000000061A3D8],+413966h                            ; [rip+00215E84]
	mov	[000000000061A3D0],+413966h                            ; [rip+00215E71]
	jmp	404484h

l0000000000404564:
	mov	rdx,rax
	mov	rsi,r12
	mov	edi,41391Dh
	call	409F80h
	mov	rbx,[000000000061A650]                                 ; [rip+002160D5]
	mov	edx,5h
	mov	esi,413928h
	xor	edi,edi
	call	402360h
	mov	rsi,rbx
	mov	rdi,rax
	mov	ebx,413700h
	call	402520h

l000000000040459C:
	mov	rcx,[rbx]
	test	rcx,rcx
	jz	4031D6h

l00000000004045A8:
	mov	rdi,[000000000061A650]                                 ; [rip+002160A1]
	mov	edx,41393Eh
	mov	esi,1h
	xor	eax,eax
	add	rbx,8h
	call	402810h
	jmp	40459Ch

l00000000004045C6:
	mov	edi,4138F5h
	call	4021C0h
	mov	r12,rax
	test	rax,rax
	mov	eax,413827h
	cmovz	r12,rax

l00000000004045DF:
	jmp	403816h

l00000000004045E4:
	mov	esi,0Ah
	call	402400h
	jmp	40363Eh

l00000000004045F3:
	mov	[000000000061A3D0],+413975h                            ; [rip+00215DD2]
	mov	[000000000061A3D8],+413969h                            ; [rip+00215DCF]
	jmp	4038A8h

l000000000040460E:
	mov	rsi,[000000000061A3D0]                                 ; [rip+00215DBB]
	mov	edx,2h
	xor	edi,edi
	call	402360h
	mov	rsi,[000000000061A3D8]                                 ; [rip+00215DB0]
	mov	edx,2h
	xor	edi,edi
	mov	[000000000061A3D0],rax                                 ; [rip+00215D9A]
	call	402360h
	mov	[000000000061A3D8],rax                                 ; [rip+00215D96]
	jmp	4038A8h

l0000000000404647:
	mov	edx,5h
	mov	esi,415D98h
	xor	edi,edi
	call	402360h
	xor	esi,esi
	mov	rdx,rax
	xor	edi,edi
	xor	eax,eax
	call	402770h
	mov	rdi,[000000000061B118]                                 ; [rip+00216AAB]
	call	4021F0h
	mov	rdi,[000000000061B120]                                 ; [rip+00216AA7]

l0000000000404679:
	test	rdi,rdi
	jz	404787h

l0000000000404682:
	mov	r13,[rdi+20h]
	call	4021F0h
	mov	rdi,r13
	jmp	404679h

l0000000000404690:
	mov	rax,[rsp+38h]
	lea	rdx,[rax+1h]
	mov	[rsp+38h],rdx
	cmp	byte ptr [rax],3Dh
	mov	edx,5h
	jnz	404528h

l00000000004046AC:
	mov	rax,[rsp+40h]
	mov	rdi,[rsp+20h]
	lea	rcx,[r13+10h]
	lea	rsi,[rsp+38h]
	xor	dl,dl
	mov	[r13+18h],rax
	call	4049E0h
	cmp	al,1h
	sbb	edx,edx
	and	edx,5h
	jmp	40451Fh

l00000000004046D6:
	mov	rax,[rsp+38h]
	mov	edx,5h
	cmp	byte ptr [rax],0h
	jz	404528h

l00000000004046E9:
	lea	rdx,[rax+1h]
	mov	[rsp+38h],rdx
	movzx	eax,byte ptr [rax]
	mov	edx,2h
	mov	[rsp+31h],al
	jmp	404528h

l0000000000404703:
	mov	rax,[rsp+38h]
	movzx	ecx,byte ptr [rax]
	cmp	cl,2Ah
	jz	4047D4h

l0000000000404714:
	cmp	cl,3Ah
	jz	4047C6h

l000000000040471D:
	test	cl,cl
	jz	40478Eh

l0000000000404721:
	lea	rdx,[rax+1h]
	mov	[rsp+38h],rdx
	movzx	eax,byte ptr [rax]
	mov	edx,1h
	mov	[rsp+30h],al
	jmp	404528h

l000000000040473B:
	mov	rax,[rsp+38h]
	xor	r15d,r15d
	lea	rdx,[rax+1h]
	mov	[rsp+38h],rdx
	cmp	byte ptr [rax],3Dh
	mov	edx,5h
	jnz	404528h

l000000000040475A:
	jmp	404772h

l000000000040475C:
	lea	rdi,[rsp+30h]
	add	r15,1h
	call	402550h
	test	eax,eax
	jz	404828h

l0000000000404772:
	mov	rsi,[4135E0h+r15*8]
	movsxd	r14,r15d
	test	rsi,rsi
	jnz	40475Ch

l0000000000404782:
	jmp	404857h

l0000000000404787:
	mov	[000000000061B129],0h                                  ; [rip+0021699B]

l000000000040478E:
	cmp	[000000000061A450],6h                                  ; [rip+00215CBA]
	jnz	403AE2h

l000000000040479C:
	mov	rdi,[000000000061A458]                                 ; [rip+00215CB5]
	mov	edx,6h
	mov	esi,413989h
	call	402240h
	test	eax,eax
	jnz	403AE2h

l00000000004047BA:
	mov	[000000000061B198],1h                                  ; [rip+002169D7]
	jmp	403AE2h

l00000000004047C6:
	add	rax,1h
	mov	[rsp+38h],rax
	jmp	40451Fh

l00000000004047D4:
	mov	edi,28h
	call	410C40h
	mov	r13,rax
	mov	rax,[000000000061B120]                                 ; [rip+00216938]
	mov	rdi,[rsp+20h]
	lea	rsi,[rsp+38h]
	mov	edx,1h
	mov	rcx,r13
	add	qword ptr [rsp+38h],1h
	mov	[000000000061B120],r13                                 ; [rip+00216919]
	mov	[r13+20h],rax
	mov	rax,[rsp+40h]
	mov	[r13+8h],rax
	call	4049E0h
	cmp	al,1h
	sbb	edx,edx
	and	edx,2h
	add	edx,3h
	jmp	40451Fh

l0000000000404828:
	shl	r14,4h
	mov	rax,[rsp+40h]
	mov	rdi,[rsp+20h]
	lea	rcx,[r14+61A3E0h]
	lea	rsi,[rsp+38h]
	xor	edx,edx
	mov	[rcx+8h],rax
	call	4049E0h
	xor	edx,edx
	test	al,al
	jnz	404528h

l0000000000404857:
	lea	rdi,[rsp+30h]
	call	40E930h
	mov	edx,5h
	mov	r14,rax
	mov	esi,4139FBh
	xor	edi,edi
	call	402360h
	mov	rcx,r14
	mov	rdx,rax
	xor	esi,esi
	xor	edi,edi
	xor	eax,eax
	call	402770h
	mov	edx,5h
	jmp	404528h

;; fn0000000000404890: 0000000000404890
fn0000000000404890 proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,0F0h
	push	rax
	push	rsp
	mov	r8,+411ED0h
	mov	rcx,+411E60h
	mov	rdi,+4028C0h
	call	4024F0h
	hlt
00000000004048BA                               66 0F 1F 44 00 00           f..D..

;; fn00000000004048C0: 00000000004048C0
fn00000000004048C0 proc
	mov	eax,61A5FFh
	push	rbp
	sub	rax,+61A5F8h
	cmp	rax,0Eh
	mov	rbp,rsp
	ja	4048D7h

l00000000004048D5:
	pop	rbp
	ret

l00000000004048D7:
	mov	eax,0h
	test	rax,rax
	jz	4048D5h

l00000000004048E1:
	pop	rbp
	mov	edi,61A5F8h
	jmp	rax
00000000004048E9                            0F 1F 80 00 00 00 00          .......
00000000004048F0 B8 F8 A5 61 00 55 48 2D F8 A5 61 00 48 C1 F8 03 ...a.UH-..a.H...
0000000000404900 48 89 E5 48 89 C2 48 C1 EA 3F 48 01 D0 48 D1 F8 H..H..H..?H..H..
0000000000404910 75 02 5D C3 BA 00 00 00 00 48 85 D2 74 F4 5D 48 u.]......H..t.]H
0000000000404920 89 C6 BF F8 A5 61 00 FF E2 0F 1F 80 00 00 00 00 .....a..........
0000000000404930 80 3D 21 5D 21 00 00 75 11 55 48 89 E5 E8 7E FF .=!]!..u.UH...~.
0000000000404940 FF FF 5D C6 05 0E 5D 21 00 01 F3 C3 0F 1F 40 00 ..]...]!......@.
0000000000404950 48 83 3D A8 54 21 00 00 74 1E B8 00 00 00 00 48 H.=.T!..t......H
0000000000404960 85 C0 74 14 55 BF 00 9E 61 00 48 89 E5 FF D0 5D ..t.U...a.H....]
0000000000404970 E9 7B FF FF FF 0F 1F 00 E9 73 FF FF FF 0F 1F 00 .{.......s......
0000000000404980 48 8B 07 31 D2 48 F7 F6 48 89 D0 C3 0F 1F 40 00 H..1.H..H.....@.
0000000000404990 31 C0 48 8B 16 48 39 17 74 06 F3 C3 0F 1F 40 00 1.H..H9.t.....@.
00000000004049A0 48 8B 46 08 48 39 47 08 0F 94 C0 C3 0F 1F 40 00 H.F.H9G.......@.
00000000004049B0 8B 05 82 66 21 00 85 C0 75 06 89 3D 78 66 21 00 ...f!...u..=xf!.
00000000004049C0 F3 C3 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 ..fffff.........
00000000004049D0 E9 1B D8 FF FF 66 66 2E 0F 1F 84 00 00 00 00 00 .....ff.........

;; fn00000000004049E0: 00000000004049E0
;;   Called from:
;;     00000000004046C5 (in fn00000000004028C0)
;;     0000000000404814 (in fn00000000004028C0)
;;     0000000000404848 (in fn00000000004028C0)
fn00000000004049E0 proc
	push	r14
	mov	r8,[rsi]
	xor	eax,eax
	mov	r9,[rdi]
	xor	r10d,r10d
	xor	r11d,r11d
	push	r12
	mov	r12,7E000000000000h
	push	rbp
	mov	rbp,rcx
	push	rbx
	mov	ebx,1h

l0000000000404A06:
	cmp	eax,2h
	jz	404A59h

l0000000000404A0B:
	jbe	404B20h

l0000000000404A11:
	cmp	eax,3h
	jz	404AE0h

l0000000000404A1A:
	cmp	eax,4h
	nop	dword ptr [rax]
	jnz	404A70h

l0000000000404A22:
	movzx	eax,byte ptr [r8]
	lea	ecx,[rax-40h]
	cmp	cl,3Eh
	jbe	404AA0h

l0000000000404A2E:
	cmp	al,3Fh
	jz	404CB0h

l0000000000404A36:
	xor	eax,eax

l0000000000404A38:
	mov	[rdi],r9
	mov	[rsi],r8
	mov	[rbp+0h],r10
	pop	rbx
	pop	rbp
	pop	r12
	pop	r14
	ret
0000000000404A49                            0F 1F 80 00 00 00 00          .......

l0000000000404A50:
	lea	r11d,[rax+r11*8-30h]
	add	r8,1h

l0000000000404A59:
	movzx	eax,byte ptr [r8]
	lea	ecx,[rax-30h]
	cmp	cl,7h
	jbe	404A50h

l0000000000404A65:
	mov	[r9],r11b
	add	r10,1h
	add	r9,1h

l0000000000404A70:
	movzx	eax,byte ptr [r8]
	cmp	al,3Dh
	jz	404ABAh

l0000000000404A78:
	jle	404B40h

l0000000000404A7E:
	cmp	al,5Ch
	jz	404CA0h

l0000000000404A86:
	cmp	al,5Eh
	jnz	404B60h

l0000000000404A8E:
	add	r8,1h
	movzx	eax,byte ptr [r8]
	lea	ecx,[rax-40h]
	cmp	cl,3Eh
	ja	404A2Eh

l0000000000404A9E:
	nop

l0000000000404AA0:
	and	eax,1Fh
	add	r8,1h
	add	r10,1h
	mov	[r9],al
	movzx	eax,byte ptr [r8]
	add	r9,1h
	cmp	al,3Dh
	jnz	404A78h

l0000000000404ABA:
	test	dl,dl
	jz	404B60h

l0000000000404AC2:
	mov	eax,1h
	jmp	404A38h
0000000000404ACC                                     0F 1F 40 00             ..@.

l0000000000404AD0:
	shl	r11d,4h
	add	r8,1h
	lea	r11d,[rax+r11-57h]
	nop	dword ptr [rax]

l0000000000404AE0:
	movzx	eax,byte ptr [r8]
	lea	ecx,[rax-30h]
	cmp	cl,36h
	ja	404A65h

l0000000000404AF0:
	mov	r14,rbx
	shl	r14,cl
	test	r14d,7E0000h
	jnz	404B78h

l0000000000404AFF:
	test	r14,r12
	jnz	404AD0h

l0000000000404B04:
	test	r14d,3FFh
	jz	404A65h

l0000000000404B11:
	shl	r11d,4h
	add	r8,1h
	lea	r11d,[rax+r11-30h]
	jmp	404AE0h

l0000000000404B20:
	cmp	eax,1h
	jnz	404A70h

l0000000000404B29:
	movzx	eax,byte ptr [r8]
	cmp	al,78h
	ja	404C90h

l0000000000404B35:
	movzx	ecx,al
	jmp	qword ptr [411F40h+rcx*8]
0000000000404B3F                                              90                .

l0000000000404B40:
	test	al,al
	jz	404B48h

l0000000000404B44:
	cmp	al,3Ah
	jnz	404B60h

l0000000000404B48:
	mov	eax,5h

l0000000000404B4D:
	cmp	eax,6h
	setnz	al
	jmp	404A38h
0000000000404B58                         0F 1F 84 00 00 00 00 00         ........

l0000000000404B60:
	mov	[r9],al
	add	r8,1h
	add	r10,1h
	add	r9,1h
	jmp	404A70h
0000000000404B74             0F 1F 40 00                             ..@.        

l0000000000404B78:
	add	r8,1h
	shl	r11d,4h
	lea	r11d,[rax+r11-37h]
	movzx	eax,byte ptr [r8]
	lea	ecx,[rax-30h]
	cmp	cl,36h
	ja	404A65h

l0000000000404B95:
	jmp	404AF0h
0000000000404B9A                               66 0F 1F 44 00 00           f..D..
0000000000404BA0 B8 03 00 00 00 45 31 DB 0F 1F 84 00 00 00 00 00 .....E1.........

l0000000000404BB0:
	add	r8,1h
	cmp	eax,4h
	jbe	404A06h

l0000000000404BBD:
	jmp	404B4Dh
0000000000404BBF                                              90                .
0000000000404BC0 41 BB 1B 00 00 00 66 2E 0F 1F 84 00 00 00 00 00 A.....f.........

l0000000000404BD0:
	mov	[r9],r11b
	add	r10,1h
	add	r9,1h
	xor	eax,eax
	jmp	404BB0h
0000000000404BDF                                              90                .
0000000000404BE0 41 BB 20 00 00 00 EB E8 0F 1F 84 00 00 00 00 00 A. .............
0000000000404BF0 B8 06 00 00 00 EB B9 66 0F 1F 84 00 00 00 00 00 .......f........
0000000000404C00 44 8D 58 D0 B8 02 00 00 00 EB A5 0F 1F 44 00 00 D.X..........D..
0000000000404C10 41 BB 7F 00 00 00 EB B8 0F 1F 84 00 00 00 00 00 A...............
0000000000404C20 41 BB 07 00 00 00 EB A8 0F 1F 84 00 00 00 00 00 A...............
0000000000404C30 41 BB 08 00 00 00 EB 98 0F 1F 84 00 00 00 00 00 A...............
0000000000404C40 41 BB 09 00 00 00 EB 88 0F 1F 84 00 00 00 00 00 A...............
0000000000404C50 41 BB 0B 00 00 00 E9 75 FF FF FF 0F 1F 44 00 00 A......u.....D..
0000000000404C60 41 BB 0A 00 00 00 E9 65 FF FF FF 0F 1F 44 00 00 A......e.....D..
0000000000404C70 41 BB 0D 00 00 00 E9 55 FF FF FF 0F 1F 44 00 00 A......U.....D..
0000000000404C80 41 BB 0C 00 00 00 E9 45 FF FF FF 0F 1F 44 00 00 A......E.....D..

l0000000000404C90:
	mov	r11d,eax
	jmp	404BD0h
0000000000404C98                         0F 1F 84 00 00 00 00 00         ........

l0000000000404CA0:
	add	r8,1h
	jmp	404B29h
0000000000404CA9                            0F 1F 80 00 00 00 00          .......

l0000000000404CB0:
	mov	byte ptr [r9],7Fh
	add	r10,1h
	add	r9,1h
	jmp	404A70h
0000000000404CC1    66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00  ffffff.........

;; fn0000000000404CD0: 0000000000404CD0
;;   Called from:
;;     0000000000403AF4 (in fn00000000004028C0)
;;     0000000000403B02 (in fn00000000004028C0)
;;     0000000000403B19 (in fn00000000004028C0)
;;     0000000000406599 (in fn0000000000406540)
;;     00000000004066A1 (in fn0000000000406540)
;;     00000000004066D0 (in fn0000000000406540)
;;     0000000000406876 (in fn0000000000406540)
;;     0000000000406890 (in fn0000000000406540)
;;     00000000004068C6 (in fn0000000000406540)
;;     00000000004068FE (in fn0000000000406540)
;;     0000000000406927 (in fn0000000000406540)
;;     0000000000406940 (in fn0000000000406540)
;;     0000000000406967 (in fn0000000000406540)
;;     000000000040698A (in fn0000000000406540)
;;     00000000004069A6 (in fn0000000000406540)
;;     0000000000406A42 (in fn0000000000406A30)
;;     00000000004082D1 (in fn0000000000407EA0)
;;     00000000004082EA (in fn0000000000407EA0)
;;     0000000000408303 (in fn0000000000407EA0)
;;     000000000040831C (in fn0000000000407EA0)
;;     000000000040839E (in fn0000000000407EA0)
;;     0000000000408929 (in fn0000000000407EA0)
;;     0000000000408942 (in fn0000000000407EA0)
;;     000000000040895B (in fn0000000000407EA0)
fn0000000000404CD0 proc
	mov	edi,edi
	xor	eax,eax
	shl	rdi,4h
	mov	rdx,[rdi+61A3E0h]
	mov	rsi,[rdi+61A3E8h]
	test	rdx,rdx
	jz	404CFCh

l0000000000404CEB:
	cmp	rdx,1h
	jz	404D10h

l0000000000404CF1:
	cmp	rdx,2h
	mov	eax,1h
	jz	404D00h

l0000000000404CFC:
	ret
0000000000404CFE                                           66 90               f.

l0000000000404D00:
	mov	edi,413733h
	mov	ecx,2h

l0000000000404D0A:
	rep cmpsb

l0000000000404D0C:
	setnz	al
	ret

l0000000000404D10:
	cmp	byte ptr [rsi],30h
	setnz	al
	ret
0000000000404D17                      66 0F 1F 84 00 00 00 00 00        f........

;; fn0000000000404D20: 0000000000404D20
;;   Called from:
;;     00000000004043B1 (in fn00000000004028C0)
;;     00000000004050BB (in fn0000000000405090)
;;     00000000004050EC (in fn0000000000405090)
;;     000000000040517A (in fn0000000000405090)
fn0000000000404D20 proc
	push	r13
	mov	r13d,edx
	push	r12
	mov	r12,rsi
	push	rbp
	mov	rbp,rdi
	mov	edi,20h
	push	rbx
	sub	rsp,8h
	call	410C40h
	mov	rbx,rax
	xor	eax,eax
	test	r12,r12
	jz	404D4Fh

l0000000000404D47:
	mov	rdi,r12
	call	410E30h

l0000000000404D4F:
	mov	[rbx+8h],rax
	xor	eax,eax
	test	rbp,rbp
	jz	404D62h

l0000000000404D5A:
	mov	rdi,rbp
	call	410E30h

l0000000000404D62:
	mov	[rbx],rax
	mov	rax,[000000000061B190]                                 ; [rip+00216424]
	mov	[rbx+10h],r13b
	mov	[000000000061B190],rbx                                 ; [rip+00216419]
	mov	[rbx+18h],rax
	add	rsp,8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	ret
0000000000404D86                   66 2E 0F 1F 84 00 00 00 00 00       f.........

;; fn0000000000404D90: 0000000000404D90
;;   Called from:
;;     0000000000404DEF (in fn0000000000404DD0)
;;     0000000000405197 (in fn0000000000405090)
fn0000000000404D90 proc
	push	rbx
	mov	rbx,rdi
	mov	rdi,[rdi]
	call	4021F0h
	mov	rdi,[rbx+8h]
	call	4021F0h
	mov	rdi,[rbx+0A8h]
	cmp	rdi,+61A56Ah
	jz	404DC0h

l0000000000404DB5:
	pop	rbx
	jmp	4027D0h
0000000000404DBB                                  0F 1F 44 00 00            ..D..

l0000000000404DC0:
	pop	rbx
	ret
0000000000404DC2       66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00   fffff.........

;; fn0000000000404DD0: 0000000000404DD0
;;   Called from:
;;     0000000000403415 (in fn00000000004028C0)
;;     00000000004036D6 (in fn00000000004028C0)
;;     0000000000403E87 (in fn00000000004028C0)
fn0000000000404DD0 proc
	push	rbx
	xor	ebx,ebx
	cmp	[000000000061B1B0],0h                                  ; [rip+002163D5]
	jz	404DFDh

l0000000000404DDD:
	nop	dword ptr [rax]

l0000000000404DE0:
	mov	rax,[000000000061B1A8]                                 ; [rip+002163C1]
	mov	rdi,[rax+rbx*8]
	add	rbx,1h
	call	404D90h
	cmp	[000000000061B1B0],rbx                                 ; [rip+002163B5]
	ja	404DE0h

l0000000000404DFD:
	mov	[000000000061B1B0],+0h                                 ; [rip+002163A8]
	mov	[000000000061B17C],0h                                  ; [rip+0021636D]
	mov	[000000000061B178],0h                                  ; [rip+0021635F]
	mov	[000000000061B174],0h                                  ; [rip+00216351]
	mov	[000000000061B170],0h                                  ; [rip+00216343]
	mov	[000000000061B168],0h                                  ; [rip+00216331]
	mov	[000000000061B164],0h                                  ; [rip+00216323]
	mov	[000000000061B160],0h                                  ; [rip+00216315]
	mov	[000000000061B16C],0h                                  ; [rip+00216317]
	mov	[000000000061B15C],0h                                  ; [rip+002162FD]
	mov	[000000000061B158],0h                                  ; [rip+002162EF]
	mov	[000000000061B154],0h                                  ; [rip+002162E1]
	pop	rbx
	ret
0000000000404E75                66 66 2E 0F 1F 84 00 00 00 00 00      ff.........

;; fn0000000000404E80: 0000000000404E80
;;   Called from:
;;     0000000000403D03 (in fn00000000004028C0)
;;     0000000000403E7D (in fn00000000004028C0)
;;     0000000000404200 (in fn00000000004028C0)
fn0000000000404E80 proc
	push	rbp
	push	rbx
	sub	rsp,8h
	mov	rbx,[000000000061B1B0]                                 ; [rip+00216323]
	mov	rax,rbx
	mov	rbp,rbx
	shr	rax,1h
	add	rax,rbx
	cmp	rax,[000000000061B1A0]                                 ; [rip+00216300]
	ja	404F98h

l0000000000404EA6:
	test	rbp,rbp
	jz	404ED3h

l0000000000404EAB:
	mov	rax,[000000000061B1A8]                                 ; [rip+002162F6]
	mov	rdx,[000000000061B1C0]                                 ; [rip+00216307]
	lea	rcx,[rax+rbp*8]
	nop	dword ptr [rax]

l0000000000404EC0:
	mov	[rax],rdx
	add	rax,8h
	add	rdx,+0C0h
	cmp	rax,rcx
	jnz	404EC0h

l0000000000404ED3:
	cmp	[000000000061B148],0FFh                                ; [rip+0021626E]
	jz	404F8Ch

l0000000000404EE0:
	mov	edi,61A680h
	call	402510h
	test	eax,eax
	jz	404F40h

l0000000000404EEE:
	mov	r8d,[000000000061B148]                                 ; [rip+00216253]
	cmp	r8d,3h
	jz	404FDFh

l0000000000404EFF:
	mov	rsi,[000000000061B1B0]                                 ; [rip+002162AA]
	mov	rdi,[000000000061B1A8]                                 ; [rip+0021629B]
	test	rsi,rsi
	jz	404F33h

l0000000000404F12:
	mov	rdx,[000000000061B1C0]                                 ; [rip+002162A7]
	lea	rcx,[rdi+rsi*8]
	mov	rax,rdi

l0000000000404F20:
	mov	[rax],rdx
	add	rax,8h
	add	rdx,+0C0h
	cmp	rax,rcx
	jnz	404F20h

l0000000000404F33:
	mov	ecx,r8d
	mov	eax,1h
	jmp	404F54h
0000000000404F3D                                        0F 1F 00              ...

l0000000000404F40:
	mov	ecx,[000000000061B148]                                 ; [rip+00216202]
	mov	rsi,[000000000061B1B0]                                 ; [rip+00216263]
	mov	rdi,[000000000061B1A8]                                 ; [rip+00216254]

l0000000000404F54:
	xor	edx,edx
	cmp	ecx,4h
	cmovz	edx,[000000000061B14C]                               ; [rip+002161EC]

l0000000000404F60:
	cdqe
	movzx	r8d,[000000000061B10C]                               ; [rip+002161A2]
	add	edx,ecx
	lea	rdx,[rax+rdx*2]
	movzx	eax,[000000000061B147]                               ; [rip+002161D0]
	lea	rax,[rax+rdx*2]
	lea	rax,[r8+rax*2]
	mov	rdx,[412D00h+rax*8]
	call	40D690h

l0000000000404F8C:
	add	rsp,8h
	pop	rbx
	pop	rbp
	ret
0000000000404F93          0F 1F 44 00 00                            ..D..        

l0000000000404F98:
	mov	rdi,[000000000061B1A8]                                 ; [rip+00216209]
	call	4021F0h
	mov	rax,0AAAAAAAAAAAAAAAh
	cmp	rbx,rax
	ja	404FF8h

l0000000000404FB3:
	lea	rdi,[rbx+rbx*2]
	shl	rdi,3h
	call	410C40h
	mov	rbp,[000000000061B1B0]                                 ; [rip+002161E9]
	mov	[000000000061B1A8],rax                                 ; [rip+002161DA]
	lea	rax,[rbp+rbp*2+0h]
	mov	[000000000061B1A0],rax                                 ; [rip+002161C6]
	jmp	404EA6h

l0000000000404FDF:
	mov	ecx,412C95h
	mov	edx,0DDBh
	mov	esi,413736h
	mov	edi,41373Fh
	call	402450h

l0000000000404FF8:
	call	410E50h
	nop	dword ptr [rax]
	mov	rsi,[rsi]
	mov	rdi,[rdi]
	jmp	402550h
000000000040500B                                  0F 1F 44 00 00            ..D..
0000000000405010 48 89 F0 48 8B 37 48 8B 38 E9 32 D5 FF FF 66 90 H..H.7H.8.2...f.

l0000000000405020:
	push	rbp
	mov	rbp,rsi
	push	rbx
	mov	rbx,rdi
	sub	rsp,8h
	call	402230h
	mov	dword ptr [rax],0h
	add	rsp,8h
	mov	rdi,rbx
	pop	rbx
	mov	rsi,rbp
	pop	rbp
	jmp	402690h
0000000000405048                         0F 1F 84 00 00 00 00 00         ........
0000000000405050 48 8B 36 48 8B 3F EB C8 0F 1F 84 00 00 00 00 00 H.6H.?..........
0000000000405060 48 89 F0 48 8B 37 48 8B 38 EB B5 0F 1F 44 00 00 H..H.7H.8....D..
0000000000405070 48 89 F0 48 8B 37 48 8B 38 E9 52 57 00 00 66 90 H..H.7H.8.RW..f.
0000000000405080 48 8B 36 48 8B 3F E9 45 57 00 00 0F 1F 44 00 00 H.6H.?.EW....D..

;; fn0000000000405090: 0000000000405090
;;   Called from:
;;     0000000000403EB2 (in fn00000000004028C0)
;;     000000000040445B (in fn00000000004028C0)
fn0000000000405090 proc
	push	r15
	push	r14
	movzx	r14d,sil
	push	r13
	mov	r13,rdi
	push	r12
	push	rbp
	push	rbx
	sub	rsp,8h
	test	rdi,rdi
	jz	4050C0h

l00000000004050AA:
	cmp	[000000000061B1C8],0h                                  ; [rip+00216116]
	jz	4050C0h

l00000000004050B4:
	mov	rsi,rdi
	xor	edx,edx
	xor	edi,edi
	call	404D20h

l00000000004050C0:
	mov	rbx,[000000000061B1B0]                                 ; [rip+002160E9]
	lea	r12,[0FFFFFFF8h+rbx*8]
	jmp	405108h
00000000004050D1    0F 1F 80 00 00 00 00                          .......        

l00000000004050D8:
	cmp	byte ptr [r15],2Fh
	jnz	405160h

l00000000004050E2:
	mov	rsi,[rbp+8h]
	mov	edx,r14d
	mov	rdi,r15
	call	404D20h
	cmp	dword ptr [rbp+0A0h],9h
	jz	405194h

l00000000004050FE:
	nop

l0000000000405100:
	sub	rbx,1h
	sub	r12,8h

l0000000000405108:
	test	rbx,rbx
	jz	4051A8h

l0000000000405111:
	mov	rax,[000000000061B1A8]                                 ; [rip+00216090]
	mov	rbp,[rax]
	mov	eax,[rbp+0A0h]
	cmp	eax,9h
	jz	40512Ch

l0000000000405127:
	cmp	eax,3h
	jnz	405100h

l000000000040512C:
	test	r13,r13
	mov	r15,[rbp+0h]
	jz	4050E2h

l0000000000405135:
	mov	rdi,r15
	call	40A390h
	cmp	byte ptr [rax],2Eh
	jnz	4050D8h

l0000000000405142:
	xor	edx,edx
	cmp	byte ptr [rax+1h],2Eh
	setz	dl
	movzx	eax,byte ptr [rax+rdx+1h]
	cmp	al,2Fh
	jz	405100h

l0000000000405154:
	test	al,al
	jz	405100h

l0000000000405158:
	jmp	4050D8h
000000000040515D                                        0F 1F 00              ...

l0000000000405160:
	xor	edx,edx
	mov	rsi,r15
	mov	rdi,r13
	call	40A610h
	mov	rsi,[rbp+8h]
	mov	r15,rax
	mov	rdi,rax
	mov	edx,r14d
	call	404D20h
	mov	rdi,r15
	call	4021F0h
	cmp	dword ptr [rbp+0A0h],9h
	jnz	405100h

l0000000000405194:
	mov	rdi,rbp
	call	404D90h
	jmp	405100h
00000000004051A1    0F 1F 80 00 00 00 00                          .......        

l00000000004051A8:
	mov	rdi,[000000000061B1B0]                                 ; [rip+00216001]
	test	rdi,rdi
	jz	4051F7h

l00000000004051B4:
	mov	rsi,[000000000061B1A8]                                 ; [rip+00215FED]
	xor	edx,edx
	xor	eax,eax
	nop

l00000000004051C0:
	mov	rcx,[rsi+rax*8]
	cmp	dword ptr [rcx+0A0h],9h
	mov	[rsi+rdx*8],rcx
	setnz	cl
	add	rax,1h
	movzx	ecx,cl
	add	rdx,rcx
	cmp	rax,rdi
	jnz	4051C0h

l00000000004051E1:
	mov	[000000000061B1B0],rdx                                 ; [rip+00215FC8]
	add	rsp,8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret

l00000000004051F7:
	xor	edx,edx
	jmp	4051E1h
00000000004051FB                                  0F 1F 44 00 00            ..D..

;; fn0000000000405200: 0000000000405200
;;   Called from:
;;     0000000000407B2A (in fn00000000004079F0)
;;     0000000000407D66 (in fn00000000004079F0)
fn0000000000405200 proc
	push	rbp
	mov	rbp,rsi
	push	rbx
	mov	rbx,rdi
	sub	rsp,8h
	cmp	rdi,rsi
	jc	40526Bh

l0000000000405211:
	jmp	40529Ch
0000000000405216                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000405220:
	xor	edx,edx
	mov	rax,rbp
	lea	rsi,[rbx+1h]
	div	rcx
	xor	edx,edx
	mov	rdi,rax
	mov	rax,rsi
	div	rcx
	cmp	rdi,rax
	jbe	4052A8h

l000000000040523C:
	mov	rdi,[000000000061A610]                                 ; [rip+002153CD]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	4052B9h

l000000000040524D:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],9h

l0000000000405258:
	mov	rax,rbx
	xor	edx,edx
	add	rbx,rcx
	div	rcx
	sub	rbx,rdx

l0000000000405266:
	cmp	rbp,rbx
	jbe	40529Ch

l000000000040526B:
	mov	rcx,[000000000061B0D8]                                 ; [rip+00215E66]
	test	rcx,rcx
	jnz	405220h

l0000000000405277:
	add	rbx,1h

l000000000040527B:
	mov	rdi,[000000000061A610]                                 ; [rip+0021538E]
	mov	rdx,[rdi+28h]
	cmp	rdx,[rdi+30h]
	jnc	4052ADh

l000000000040528C:
	lea	rax,[rdx+1h]
	cmp	rbp,rbx
	mov	[rdi+28h],rax
	mov	byte ptr [rdx],20h
	ja	40526Bh

l000000000040529C:
	add	rsp,8h
	pop	rbx
	pop	rbp
	ret
00000000004052A3          0F 1F 44 00 00                            ..D..        

l00000000004052A8:
	mov	rbx,rsi
	jmp	40527Bh

l00000000004052AD:
	mov	esi,20h
	call	402400h
	jmp	405266h

l00000000004052B9:
	mov	esi,9h
	call	402400h
	mov	rcx,[000000000061B0D8]                                 ; [rip+00215E0E]
	jmp	405258h
00000000004052CC                                     0F 1F 40 00             ..@.

;; fn00000000004052D0: 00000000004052D0
;;   Called from:
;;     0000000000403674 (in fn00000000004028C0)
;;     0000000000405DE8 (in fn0000000000405D50)
;;     0000000000406745 (in fn0000000000406540)
;;     00000000004067EB (in fn0000000000406540)
fn00000000004052D0 proc
	push	rbp
	mov	r8,rdx
	mov	rbp,rsp
	push	r15
	push	r14
	mov	r14,rsi
	push	r13
	mov	r13,rdx
	mov	rdx,rsi
	mov	esi,2000h
	push	r12
	push	rbx
	sub	rsp,+2058h
	mov	[rbp-2078h],rdi
	lea	rdi,[rbp-2040h]
	mov	[rbp-2070h],rcx
	mov	rcx,-1h
	mov	rax,fs:[0028h]
	mov	[rbp-38h],rax
	xor	eax,eax
	call	40E6F0h
	mov	rbx,rax
	lea	rax,[rbp-2040h]
	cmp	rbx,+1FFFh
	mov	[rbp-2068h],rax
	ja	405590h

l0000000000405343:
	cmp	[000000000061B0F0],0h                                  ; [rip+00215DA6]
	jnz	405412h

l0000000000405350:
	cmp	qword ptr [rbp-2070h],0h
	jz	405379h

l000000000040535A:
	call	402370h
	cmp	rax,1h
	jbe	4053CDh

l0000000000405365:
	mov	rdi,[rbp-2068h]
	xor	edx,edx
	mov	rsi,rbx
	call	40D240h
	movsxd	r12,eax

l0000000000405379:
	mov	rcx,[rbp-2078h]
	test	rcx,rcx
	jz	405399h

l0000000000405385:
	mov	rdi,[rbp-2068h]
	mov	rdx,rbx
	mov	esi,1h
	call	4026C0h

l0000000000405399:
	mov	rax,[rbp-2070h]
	test	rax,rax
	jz	4053A8h

l00000000004053A5:
	mov	[rax],r12

l00000000004053A8:
	mov	rax,rbx
	mov	rbx,[rbp-38h]
	xor	rbx,fs:[0028h]
	jnz	40561Ch

l00000000004053BE:
	lea	rsp,[rbp-28h]
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	ret

l00000000004053CD:
	mov	r14,[rbp-2068h]
	lea	r13,[r14+rbx]
	cmp	r14,r13
	jnc	40560Ah

l00000000004053E1:
	call	402880h
	xor	r12d,r12d
	mov	rcx,[rax]
	mov	rax,r14
	nop

l00000000004053F0:
	movzx	edx,byte ptr [rax]
	movzx	edx,word ptr [rcx+rdx*2]
	and	dx,4000h
	cmp	dx,1h
	sbb	r12,0FFh
	add	rax,1h
	cmp	rax,r13
	jnz	4053F0h

l000000000040540D:
	jmp	405379h

l0000000000405412:
	call	402370h
	cmp	rax,1h
	jbe	4055D1h

l0000000000405421:
	mov	r15,[rbp-2068h]
	lea	r13,[r15+rbx]
	cmp	r15,r13
	jnc	405612h

l0000000000405435:
	mov	rbx,r15
	xor	r12d,r12d
	nop	dword ptr [rax+rax+0h]

l0000000000405440:
	movzx	eax,byte ptr [r15]
	cmp	al,3Fh
	jg	405520h

l000000000040544C:
	cmp	al,25h
	jge	405538h

l0000000000405454:
	lea	edx,[rax-20h]
	cmp	dl,3h
	jbe	405538h

l0000000000405460:
	mov	qword ptr [rbp-2050h],+0h
	nop	dword ptr [rax+rax+0h]

l0000000000405470:
	mov	rdx,r13
	lea	rcx,[rbp-2050h]
	lea	rdi,[rbp-2054h]
	sub	rdx,r15
	mov	rsi,r15
	call	4023C0h
	cmp	rax,0FFh
	mov	r14,rax
	jz	405551h

l0000000000405499:
	cmp	rax,0FEh
	jz	40556Bh

l00000000004054A3:
	mov	edi,[rbp-2054h]
	test	rax,rax
	mov	eax,1h
	cmovz	r14,rax

l00000000004054B5:
	call	402630h
	test	eax,eax
	js	405510h

l00000000004054BE:
	lea	rsi,[r15+r14]
	mov	rdx,rbx
	nop	dword ptr [rax]

l00000000004054C8:
	add	r15,1h
	movzx	ecx,byte ptr [r15-1h]
	add	rdx,1h
	cmp	r15,rsi
	mov	[rdx-1h],cl
	jnz	4054C8h

l00000000004054DD:
	cdqe
	add	rbx,r14
	add	r12,rax

l00000000004054E5:
	lea	rdi,[rbp-2050h]
	call	402830h
	test	eax,eax
	jz	405470h

l00000000004054F9:
	cmp	r15,r13
	jc	405440h

l0000000000405502:
	sub	rbx,[rbp-2068h]
	jmp	405379h
000000000040550E                                           66 90               f.

l0000000000405510:
	mov	byte ptr [rbx],3Fh
	add	r15,r14
	add	r12,1h
	add	rbx,1h
	jmp	4054E5h

l0000000000405520:
	cmp	al,41h
	jl	405460h

l0000000000405528:
	cmp	al,5Fh
	jle	405538h

l000000000040552C:
	lea	edx,[rax-61h]
	cmp	dl,1Dh
	ja	405460h

l0000000000405538:
	add	r15,1h
	mov	[rbx],al
	add	r12,1h
	add	rbx,1h
	cmp	r15,r13
	jc	405440h

l000000000040554F:
	jmp	405502h

l0000000000405551:
	add	r15,1h
	mov	byte ptr [rbx],3Fh
	add	r12,1h
	add	rbx,1h
	cmp	r15,r13
	jc	405440h

l0000000000405569:
	jmp	405502h

l000000000040556B:
	mov	r15,r13
	mov	byte ptr [rbx],3Fh
	add	r12,1h
	add	rbx,1h
	cmp	r15,r13
	jc	405440h

l0000000000405582:
	jmp	405502h
0000000000405587                      66 0F 1F 84 00 00 00 00 00        f........

l0000000000405590:
	lea	rax,[rbx+1Fh]
	lea	rsi,[rbx+1h]
	mov	r8,r13
	mov	rcx,-1h
	mov	rdx,r14
	and	rax,0F0h
	sub	rsp,rax
	lea	rax,[rsp+0Fh]
	mov	[rbp-2068h],rax
	and	qword ptr [rbp-2068h],0F0h
	mov	rdi,[rbp-2068h]
	call	40E6F0h
	jmp	405343h

l00000000004055D1:
	mov	r14,[rbp-2068h]
	lea	r12,[r14+rbx]
	cmp	r14,r12
	jnc	405602h

l00000000004055E1:
	call	402880h
	mov	rdx,r14

l00000000004055E9:
	movzx	esi,byte ptr [rdx]
	mov	rcx,[rax]
	test	byte ptr [rcx+rsi*2+1h],40h
	jnz	4055F9h

l00000000004055F6:
	mov	byte ptr [rdx],3Fh

l00000000004055F9:
	add	rdx,1h
	cmp	rdx,r12
	jnz	4055E9h

l0000000000405602:
	mov	r12,rbx
	jmp	405379h

l000000000040560A:
	xor	r12d,r12d
	jmp	405379h

l0000000000405612:
	xor	ebx,ebx
	xor	r12d,r12d
	jmp	405379h

l000000000040561C:
	call	4023A0h
0000000000405621    66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00  ffffff.........

;; fn0000000000405630: 0000000000405630
;;   Called from:
;;     0000000000404316 (in fn00000000004028C0)
;;     0000000000404325 (in fn00000000004028C0)
fn0000000000405630 proc
	push	r12
	push	rbp
	push	rbx
	mov	rax,[rsi+18h]
	mov	rbp,[rsi+10h]
	mov	r12,rax
	sub	r12,rbp
	shr	r12d,3h
	test	r12,r12
	jz	4056DBh

l000000000040564F:
	cmp	rax,rbp
	jz	4056E6h

l0000000000405658:
	movsxd	rcx,dword ptr [rsi+30h]
	mov	edx,ecx
	add	rax,rcx
	mov	rcx,[rsi+20h]
	not	edx
	movsxd	rdx,edx
	and	rdx,rax
	mov	rax,[rsi+8h]
	mov	rbx,rcx
	mov	[rsi+18h],rdx
	sub	rbx,rax
	sub	rdx,rax
	cmp	rdx,rbx
	jg	4056E0h

l0000000000405683:
	mov	rax,[rsi+18h]
	xor	ebx,ebx
	mov	[rsi+10h],rax
	mov	rsi,[000000000061A610]                                 ; [rip+00214F7C]
	call	402520h
	nop	dword ptr [rax+0h]

l00000000004056A0:
	mov	rdx,[rbp+rbx*8+0h]
	xor	eax,eax
	mov	esi,413759h
	mov	edi,1h
	add	rbx,1h
	call	402730h
	cmp	r12,rbx
	ja	4056A0h

l00000000004056BF:
	mov	rdi,[000000000061A610]                                 ; [rip+00214F4A]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	4056EFh

l00000000004056D0:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],0Ah

l00000000004056DB:
	pop	rbx
	pop	rbp
	pop	r12
	ret

l00000000004056E0:
	mov	[rsi+18h],rcx
	jmp	405683h

l00000000004056E6:
	or	byte ptr [rsi+50h],2h
	jmp	405658h

l00000000004056EF:
	pop	rbx
	pop	rbp
	pop	r12
	mov	esi,0Ah
	jmp	402400h
00000000004056FD                                        0F 1F 00              ...

;; fn0000000000405700: 0000000000405700
;;   Called from:
;;     00000000004057D8 (in fn00000000004057B0)
;;     0000000000407204 (in fn0000000000406B70)
;;     0000000000407238 (in fn0000000000406B70)
fn0000000000405700 proc
	push	rbp
	mov	rbp,rdi
	push	rbx
	mov	ebx,edx
	sub	rsp,8h
	test	rdi,rdi
	jz	405780h

l0000000000405710:
	xor	esi,esi
	call	40D420h
	mov	rsi,[000000000061A610]                                 ; [rip+00214EF2]
	sub	ebx,eax
	mov	eax,0h
	cmovs	ebx,eax

l0000000000405728:
	mov	rdi,rbp
	call	402520h
	mov	rdi,rbp
	movsxd	rbp,ebx
	call	402380h
	add	rbp,rax
	nop

l0000000000405740:
	mov	rdi,[000000000061A610]                                 ; [rip+00214EC9]
	mov	rcx,[rdi+28h]
	cmp	rcx,[rdi+30h]
	jnc	405799h

l0000000000405751:
	lea	rdx,[rcx+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rcx],20h

l000000000040575C:
	sub	ebx,1h
	cmp	ebx,0FFh
	jnz	405740h

l0000000000405764:
	mov	rax,[000000000061B018]                                 ; [rip+002158AD]
	lea	rax,[rbp+rax+1h]
	mov	[000000000061B018],rax                                 ; [rip+002158A1]
	add	rsp,8h
	pop	rbx
	pop	rbp
	ret
000000000040577E                                           66 90               f.

l0000000000405780:
	mov	rcx,rsi
	mov	edi,1h
	mov	esi,41375Eh
	xor	eax,eax
	movsxd	rbp,ebx
	call	402730h
	jmp	405764h

l0000000000405799:
	mov	esi,20h
	call	402400h
	jmp	40575Ch
00000000004057A5                66 66 2E 0F 1F 84 00 00 00 00 00      ff.........

;; fn00000000004057B0: 00000000004057B0
;;   Called from:
;;     00000000004071E3 (in fn0000000000406B70)
;;     0000000000407258 (in fn0000000000406B70)
fn00000000004057B0 proc
	push	rbx
	mov	eax,413764h
	mov	ebx,edi
	sub	rsp,10h
	test	dl,dl
	jz	4057CBh

l00000000004057C0:
	xor	eax,eax
	cmp	[000000000061B145],0h                                  ; [rip+0021597C]
	jz	4057E0h

l00000000004057CB:
	add	rsp,10h
	mov	edx,esi
	mov	rsi,rbx
	pop	rbx
	mov	rdi,rax
	jmp	405700h
00000000004057DD                                        0F 1F 00              ...

l00000000004057E0:
	mov	[rsp+0Ch],esi
	call	40C9B0h
	mov	esi,[rsp+0Ch]
	jmp	4057CBh
00000000004057EF                                              90                .
00000000004057F0 8B 05 42 58 21 00 85 C0 75 0F 8B 05 34 58 21 00 ..BX!...u...4X!.
0000000000405800 83 C0 01 89 05 2B 58 21 00 F3 C3 0F 1F 44 00 00 .....+X!.....D..

;; fn0000000000405810: 0000000000405810
;;   Called from:
;;     0000000000403494 (in fn00000000004028C0)
;;     0000000000403CE3 (in fn00000000004028C0)
;;     0000000000403ED8 (in fn00000000004028C0)
;;     00000000004041B1 (in fn00000000004028C0)
;;     0000000000407FD6 (in fn0000000000407EA0)
;;     0000000000408BF8 (in fn0000000000407EA0)
fn0000000000405810 proc
	push	r12
	mov	r12d,edi
	mov	rdi,rdx
	push	rbp
	push	rbx
	mov	rbx,rsi
	call	40EAB0h
	mov	rbp,rax
	call	402230h
	mov	esi,[rax]
	xor	edi,edi
	xor	eax,eax
	mov	rcx,rbp
	mov	rdx,rbx
	call	402770h
	test	r12b,r12b
	jz	405850h

l0000000000405840:
	mov	[000000000061B030],2h                                  ; [rip+002157E6]

l000000000040584A:
	pop	rbx
	pop	rbp
	pop	r12
	ret
000000000040584F                                              90                .

l0000000000405850:
	mov	eax,[000000000061B030]                                 ; [rip+002157DA]
	test	eax,eax
	jnz	40584Ah

l000000000040585A:
	pop	rbx
	pop	rbp
	mov	[000000000061B030],1h                                  ; [rip+002157CA]
	pop	r12
	ret
0000000000405869                            0F 1F 80 00 00 00 00          .......
0000000000405870 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
0000000000405880 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 0F 94 C2 .........A......
0000000000405890 41 83 F8 03 41 0F 94 C0 44 09 C2 08 C8 75 41 84 A...A...D....uA.
00000000004058A0 C0 75 0D 84 D2 B8 01 00 00 00 74 04 F3 C3 66 90 .u........t...f.
00000000004058B0 48 8B 4E 78 48 39 4F 78 48 8B 87 80 00 00 00 48 H.NxH9OxH......H
00000000004058C0 8B 96 80 00 00 00 7F 20 7C 26 29 C2 75 28 48 8B ....... |&).u(H.
00000000004058D0 36 48 8B 3F E9 77 CC FF FF 0F 1F 80 00 00 00 00 6H.?.w..........
00000000004058E0 84 D2 75 BB 0F 1F 40 00 B8 FF FF FF FF C3 66 90 ..u...@.......f.
00000000004058F0 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
0000000000405900 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 49 89 F1 ......D......I..
0000000000405910 83 F8 09 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 ............A...
0000000000405920 0F 94 C2 41 83 F8 03 40 0F 94 C6 09 F2 08 C8 75 ...A...@.......u
0000000000405930 3F 84 C0 75 13 84 D2 B8 01 00 00 00 74 0A 66 90 ?..u........t.f.
0000000000405940 F3 C3 66 0F 1F 44 00 00 48 8B 4F 68 49 39 49 68 ..f..D..H.OhI9Ih
0000000000405950 49 8B 41 70 48 8B 57 70 7F 1E 7C 24 29 C2 75 26 I.ApH.Wp..|$).u&
0000000000405960 48 8B 37 49 8B 39 E9 E5 CB FF FF 0F 1F 44 00 00 H.7I.9.......D..
0000000000405970 84 D2 75 BD 0F 1F 40 00 B8 FF FF FF FF C3 66 90 ..u...@.......f.
0000000000405980 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
0000000000405990 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 49 89 F1 ......D......I..
00000000004059A0 83 F8 09 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 ............A...
00000000004059B0 0F 94 C2 41 83 F8 03 40 0F 94 C6 09 F2 08 C8 75 ...A...@.......u
00000000004059C0 3F 84 C0 75 13 84 D2 B8 01 00 00 00 74 0A 66 90 ?..u........t.f.
00000000004059D0 F3 C3 66 0F 1F 44 00 00 48 8B 4F 58 49 39 49 58 ..f..D..H.OXI9IX
00000000004059E0 49 8B 41 60 48 8B 57 60 7F 1E 7C 24 29 C2 75 26 I.A`H.W`..|$).u&
00000000004059F0 48 8B 37 49 8B 39 E9 55 CB FF FF 0F 1F 44 00 00 H.7I.9.U.....D..
0000000000405A00 84 D2 75 BD 0F 1F 40 00 B8 FF FF FF FF C3 66 90 ..u...@.......f.
0000000000405A10 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
0000000000405A20 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
0000000000405A30 0F 94 C1 83 F8 03 0F 94 C2 41 83 F8 09 0F 94 C0 .........A......
0000000000405A40 41 83 F8 03 41 0F 94 C0 44 09 C0 08 CA 75 21 84 A...A...D....u!.
0000000000405A50 D2 74 2D 48 8B 4F 40 48 39 4E 40 48 8B 06 48 8B .t-H.O@H9N@H..H.
0000000000405A60 17 7F 15 7C 1F 48 89 D6 48 89 C7 E9 B0 F5 FF FF ...|.H..H.......
0000000000405A70 84 C0 75 DB 0F 1F 40 00 B8 FF FF FF FF C3 66 90 ..u...@.......f.
0000000000405A80 84 C0 74 CF B8 01 00 00 00 C3 66 0F 1F 44 00 00 ..t.......f..D..
0000000000405A90 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
0000000000405AA0 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 0F 94 C2 .........A......
0000000000405AB0 41 83 F8 03 41 0F 94 C0 44 09 C2 08 C8 75 21 84 A...A...D....u!.
0000000000405AC0 C0 75 0D 84 D2 B8 01 00 00 00 74 04 F3 C3 66 90 .u........t...f.
0000000000405AD0 48 8B 36 48 8B 3F E9 45 F5 FF FF 0F 1F 44 00 00 H.6H.?.E.....D..
0000000000405AE0 84 D2 75 DB B8 FF FF FF FF C3 66 0F 1F 44 00 00 ..u.......f..D..
0000000000405AF0 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
0000000000405B00 0F 94 C1 83 F8 03 0F 94 C2 41 83 F8 09 0F 94 C0 .........A......
0000000000405B10 41 83 F8 03 41 0F 94 C0 44 09 C0 08 CA 75 21 84 A...A...D....u!.
0000000000405B20 D2 74 0D 48 8B 36 48 8B 3F E9 22 CA FF FF 66 90 .t.H.6H.?."...f.
0000000000405B30 84 C0 74 EF B8 01 00 00 00 C3 66 0F 1F 44 00 00 ..t.......f..D..
0000000000405B40 84 C0 75 DB B8 FF FF FF FF C3 66 0F 1F 44 00 00 ..u.......f..D..
0000000000405B50 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 49 89 F1 ......D......I..
0000000000405B60 83 F8 09 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 ............A...
0000000000405B70 0F 94 C2 41 83 F8 03 40 0F 94 C6 09 F2 08 C8 75 ...A...@.......u
0000000000405B80 27 84 C0 75 13 84 D2 B8 01 00 00 00 74 0A 66 90 '..u........t.f.
0000000000405B90 F3 C3 66 0F 1F 44 00 00 48 8B 37 49 8B 39 E9 7D ..f..D..H.7I.9.}
0000000000405BA0 F4 FF FF 0F 1F 44 00 00 84 D2 75 D5 B8 FF FF FF .....D....u.....
0000000000405BB0 FF C3 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 ..fffff.........
0000000000405BC0 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 49 89 F1 ......D......I..
0000000000405BD0 83 F8 09 0F 94 C1 83 F8 03 0F 94 C2 41 83 F8 09 ............A...
0000000000405BE0 0F 94 C0 41 83 F8 03 40 0F 94 C6 09 F0 08 CA 75 ...A...@.......u
0000000000405BF0 1F 84 D2 74 0B 48 8B 37 49 8B 39 E9 50 C9 FF FF ...t.H.7I.9.P...
0000000000405C00 84 C0 74 F1 B8 01 00 00 00 C3 66 0F 1F 44 00 00 ..t.......f..D..
0000000000405C10 84 C0 75 DD B8 FF FF FF FF C3 66 0F 1F 44 00 00 ..u.......f..D..

;; fn0000000000405C20: 0000000000405C20
;;   Called from:
;;     0000000000405D05 (in fn0000000000405D00)
;;     0000000000405E0C (in fn0000000000405D50)
fn0000000000405C20 proc
	test	dil,dil
	jz	405C40h

l0000000000405C25:
	mov	eax,esi
	and	eax,0F000h
	cmp	eax,8000h
	jnz	405CA0h

l0000000000405C33:
	xor	eax,eax
	cmp	[000000000061B12C],3h                                  ; [rip+002154F0]
	jz	405CB0h

l0000000000405C3E:
	ret

l0000000000405C40:
	xor	eax,eax
	cmp	edx,5h
	jz	405C3Eh

l0000000000405C47:
	cmp	edx,9h
	setz	cl
	cmp	edx,3h
	setz	al
	or	ecx,eax

l0000000000405C55:
	test	cl,cl
	mov	eax,2Fh
	jnz	405C3Eh

l0000000000405C5E:
	cmp	[000000000061B12C],1h                                  ; [rip+002154C7]
	jz	405CF0h

l0000000000405C6B:
	test	dil,dil
	jz	405CC0h

l0000000000405C70:
	and	esi,0F000h
	mov	eax,40h
	cmp	esi,0A000h
	jz	405C3Eh

l0000000000405C83:
	cmp	esi,1000h
	mov	eax,7Ch
	jz	405C3Eh

l0000000000405C90:
	cmp	esi,0C000h
	setz	al
	jmp	405CE2h
0000000000405C9B                                  0F 1F 44 00 00            ..D..

l0000000000405CA0:
	cmp	eax,4000h
	setz	cl
	jmp	405C55h
0000000000405CAA                               66 0F 1F 44 00 00           f..D..

l0000000000405CB0:
	and	esi,49h
	cmp	esi,1h
	sbb	eax,eax
	not	eax
	and	eax,2Ah
	ret
0000000000405CBE                                           66 90               f.

l0000000000405CC0:
	cmp	edx,6h
	mov	eax,40h
	jz	405C3Eh

l0000000000405CCE:
	cmp	edx,1h
	mov	eax,7Ch
	jz	405C3Eh

l0000000000405CDC:
	cmp	edx,7h
	setz	al

l0000000000405CE2:
	neg	eax
	and	eax,3Dh
	ret
0000000000405CE8                         0F 1F 84 00 00 00 00 00         ........

l0000000000405CF0:
	xor	eax,eax
	ret
0000000000405CF3          66 66 66 66 2E 0F 1F 84 00 00 00 00 00    ffff.........

;; fn0000000000405D00: 0000000000405D00
;;   Called from:
;;     000000000040700D (in fn0000000000406B70)
;;     00000000004071BD (in fn0000000000406B70)
;;     000000000040793D (in fn0000000000407870)
fn0000000000405D00 proc
	push	rbx
	movzx	edi,dil
	call	405C20h
	test	al,al
	mov	ebx,eax
	jz	405D33h

l0000000000405D10:
	mov	rdi,[000000000061A610]                                 ; [rip+002148F9]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	405D3Ah

l0000000000405D21:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	[rax],bl

l0000000000405D2B:
	add	[000000000061B018],1h                                  ; [rip+002152E5]

l0000000000405D33:
	test	bl,bl
	setnz	al
	pop	rbx
	ret

l0000000000405D3A:
	movzx	esi,bl
	call	402400h
	jmp	405D2Bh
0000000000405D44             66 66 66 2E 0F 1F 84 00 00 00 00 00     fff.........

;; fn0000000000405D50: 0000000000405D50
;;   Called from:
;;     000000000040601B (in fn0000000000405ED0)
;;     0000000000407A72 (in fn00000000004079F0)
;;     0000000000407AF0 (in fn00000000004079F0)
;;     0000000000407B51 (in fn00000000004079F0)
;;     0000000000407B8D (in fn00000000004079F0)
;;     0000000000407D79 (in fn00000000004079F0)
fn0000000000405D50 proc
	push	rbp
	mov	rbp,rdi
	push	rbx
	xor	ebx,ebx
	sub	rsp,+2B8h
	mov	rax,fs:[0028h]
	mov	[rsp+2A8h],rax
	xor	eax,eax
	cmp	[000000000061B114],0h                                  ; [rip+0021539C]
	jz	405D92h

l0000000000405D7A:
	cmp	[000000000061B150],4h                                  ; [rip+002153CF]
	jz	405EA0h

l0000000000405D87:
	movsxd	rbx,[000000000061B178]                              ; [rip+002153EA]
	add	rbx,1h

l0000000000405D92:
	cmp	[000000000061B144],0h                                  ; [rip+002153AB]
	jz	405DB6h

l0000000000405D9B:
	cmp	[000000000061B150],4h                                  ; [rip+002153AE]
	jz	405E58h

l0000000000405DA8:
	movsxd	rax,[000000000061B174]                              ; [rip+002153C5]
	add	rax,1h

l0000000000405DB3:
	add	rbx,rax

l0000000000405DB6:
	cmp	[000000000061B17D],0h                                  ; [rip+002153C0]
	jz	405DD6h

l0000000000405DBF:
	cmp	[000000000061B150],4h                                  ; [rip+0021538A]
	jz	405E40h

l0000000000405DC8:
	movsxd	rax,[000000000061B16C]                              ; [rip+0021539D]
	add	rax,1h

l0000000000405DD3:
	add	rbx,rax

l0000000000405DD6:
	mov	rdx,[000000000061B0E8]                                 ; [rip+0021530B]
	mov	rsi,[rbp+0h]
	lea	rcx,[rsp+8h]
	xor	edi,edi
	call	4052D0h
	mov	eax,[000000000061B12C]                                 ; [rip+00215339]
	add	rbx,[rsp+8h]
	test	eax,eax
	jz	405E1Ch

l0000000000405DFC:
	movzx	edi,byte ptr [rbp+0B0h]
	mov	edx,[rbp+0A0h]
	mov	esi,[rbp+28h]
	call	405C20h
	test	al,al
	setnz	al
	movzx	eax,al
	add	rbx,rax

l0000000000405E1C:
	mov	rdx,[rsp+2A8h]
	xor	rdx,fs:[0028h]
	mov	rax,rbx
	jnz	405EBFh

l0000000000405E36:
	add	rsp,+2B8h
	pop	rbx
	pop	rbp
	ret

l0000000000405E40:
	mov	rdi,[rbp+0A8h]
	call	402380h
	add	rax,1h
	jmp	405DD3h
0000000000405E52       66 0F 1F 44 00 00                           f..D..        

l0000000000405E58:
	cmp	byte ptr [rbp+0B0h],0h
	mov	eax,2h
	jz	405DB3h

l0000000000405E6A:
	mov	rdi,[rbp+50h]
	mov	r8,[000000000061B138]                                  ; [rip+002152C3]
	lea	rsi,[rsp+10h]
	mov	edx,[000000000061B140]                                 ; [rip+002152C0]
	mov	ecx,200h
	call	40BD70h
	mov	rdi,rax
	call	402380h
	add	rax,1h
	jmp	405DB3h
0000000000405E9B                                  0F 1F 44 00 00            ..D..

l0000000000405EA0:
	mov	rdi,[rdi+18h]
	lea	rsi,[rsp+10h]
	call	40CD70h
	mov	rdi,rax
	call	402380h
	lea	rbx,[rax+1h]
	jmp	405D92h

l0000000000405EBF:
	call	4023A0h
0000000000405EC4             66 66 66 2E 0F 1F 84 00 00 00 00 00     fff.........

;; fn0000000000405ED0: 0000000000405ED0
;;   Called from:
;;     0000000000407A4D (in fn00000000004079F0)
;;     0000000000407CDF (in fn00000000004079F0)
fn0000000000405ED0 proc
	push	r14
	mov	rdx,[000000000061B1B0]                                 ; [rip+002152D7]
	mov	rax,[000000000061B020]                                 ; [rip+00215140]
	push	r13
	push	r12
	cmp	rax,rdx
	mov	r12d,edi
	push	rbp
	push	rbx
	mov	rbx,rdx
	cmovbe	rbx,rax

l0000000000405EF3:
	cmp	rbx,[000000000061A660]                                 ; [rip+00214766]
	jbe	4060C8h

l0000000000405F00:
	mov	rdx,rax
	mov	rdi,[000000000061B028]                                 ; [rip+0021511E]
	shr	rdx,1h
	cmp	rbx,rdx
	jc	406170h

l0000000000405F16:
	mov	rdx,0AAAAAAAAAAAAAAAh
	cmp	rax,rdx
	ja	40619Dh

l0000000000405F29:
	lea	rsi,[rax+rax*2]
	shl	rsi,3h
	call	410C90h
	mov	rbp,[000000000061B020]                                 ; [rip+002150E3]
	mov	[000000000061B028],rax                                 ; [rip+002150E4]

l0000000000405F44:
	mov	rax,[000000000061A660]                                 ; [rip+00214715]
	mov	rdi,rbp
	lea	rsi,[rbp+rax+1h]
	sub	rdi,rax
	mov	rcx,rsi
	imul	rcx,rdi
	cmp	rbp,rsi
	ja	40619Dh

l0000000000405F66:
	xor	edx,edx
	mov	rax,rcx
	div	rdi
	cmp	rsi,rax
	jnz	40619Dh

l0000000000405F77:
	shr	rcx,1h
	mov	rax,1FFFFFFFFFFFFFFFh
	cmp	rcx,rax
	ja	40619Dh

l0000000000405F8D:
	lea	rdi,[0000h+rcx*8]
	call	410C40h
	mov	rcx,[000000000061A660]                                 ; [rip+002146BF]
	cmp	rbp,rcx
	jbe	405FE4h

l0000000000405FA6:
	mov	rsi,[000000000061B028]                                 ; [rip+0021507B]
	lea	rdx,[rcx+rcx*2]
	lea	rdi,[rbp+rbp*2+0h]
	lea	rcx,[0008h+rcx*8]
	lea	rdx,[rsi+rdx*8]
	lea	rsi,[rsi+rdi*8]
	nop	word ptr cs:[rax+rax+0h]

l0000000000405FD0:
	mov	[rdx+10h],rax
	add	rdx,18h
	add	rax,rcx
	add	rcx,8h
	cmp	rdx,rsi
	jnz	405FD0h

l0000000000405FE4:
	xor	eax,eax
	test	rbx,rbx
	mov	[000000000061A660],rbp                                 ; [rip+00214670]
	mov	r8,[000000000061B1B0]                                  ; [rip+002151B9]
	mov	rsi,[000000000061B028]                                 ; [rip+0021502A]
	jnz	4060E0h

l0000000000406004:
	xor	ebp,ebp
	test	r8,r8
	jz	40612Dh

l000000000040600F:
	nop

l0000000000406010:
	mov	rax,[000000000061B1A8]                                 ; [rip+00215191]
	mov	rdi,[rax+rbp*8]
	call	405D50h
	test	rbx,rbx
	mov	r11,rax
	mov	r14,[000000000061B1B0]                                 ; [rip+00215183]
	jz	406120h

l0000000000406033:
	mov	r13,[000000000061B0C8]                                 ; [rip+0021508E]
	mov	rsi,[000000000061B028]                                 ; [rip+00214FE7]
	mov	ecx,1h
	lea	rdi,[r14-1h]
	jmp	4060A7h
000000000040604C                                     0F 1F 40 00             ..@.

l0000000000406050:
	lea	rax,[rdi+rcx]
	xor	edx,edx
	div	rcx
	xor	edx,edx
	mov	r8,rax
	mov	rax,rbp
	div	r8
	mov	r8,rcx
	mov	r10,rax

l000000000040606A:
	xor	eax,eax
	cmp	r9,r10
	setnz	al
	lea	rdx,[r11+rax*2]
	mov	rax,[rsi+10h]
	lea	rax,[rax+r10*8]
	mov	r9,[rax]
	cmp	rdx,r9
	jbe	40609Ah

l0000000000406086:
	mov	r10,rdx
	sub	r10,r9
	add	[rsi+8h],r10
	mov	[rax],rdx
	cmp	[rsi+8h],r13
	setc	byte ptr [rsi]

l000000000040609A:
	add	rsi,18h
	add	rcx,1h
	cmp	rbx,r8
	jbe	406120h

l00000000004060A7:
	cmp	byte ptr [rsi],0h
	lea	r9,[rcx-1h]
	mov	r8,rcx
	jz	40609Ah

l00000000004060B3:
	test	r12b,r12b
	jnz	406050h

l00000000004060B8:
	mov	rax,rbp
	xor	edx,edx
	mov	r8,rcx
	div	rcx
	mov	r10,rdx
	jmp	40606Ah

l00000000004060C8:
	xor	eax,eax
	test	rbx,rbx
	mov	r8,rdx
	mov	rsi,[000000000061B028]                                 ; [rip+00214F51]
	jz	406004h

l00000000004060DD:
	nop	dword ptr [rax]

l00000000004060E0:
	lea	rdi,[rax+1h]
	mov	rcx,[rsi+10h]
	mov	byte ptr [rsi],1h
	lea	rdx,[rdi+rdi*2]
	mov	[rsi+8h],rdx
	xor	edx,edx
	nop	dword ptr [rax]

l00000000004060F8:
	mov	qword ptr [rcx+rdx*8],+3h
	add	rdx,1h
	cmp	rdx,rax
	jbe	4060F8h

l0000000000406109:
	add	rsi,18h
	cmp	rdi,rbx
	jz	406004h

l0000000000406116:
	mov	rax,rdi
	jmp	4060E0h
000000000040611B                                  0F 1F 44 00 00            ..D..

l0000000000406120:
	add	rbp,1h
	cmp	rbp,r14
	jc	406010h

l000000000040612D:
	cmp	rbx,1h
	jbe	406164h

l0000000000406133:
	mov	rdx,[000000000061B028]                                 ; [rip+00214EEE]
	lea	rax,[rbx+rbx*2]
	shl	rax,3h
	cmp	byte ptr [rdx+rax-18h],0h
	jnz	406164h

l0000000000406149:
	add	rax,rdx
	jmp	40615Ah
000000000040614E                                           66 90               f.

l0000000000406150:
	sub	rax,18h
	cmp	byte ptr [rax-18h],0h
	jnz	406164h

l000000000040615A:
	sub	rbx,1h
	cmp	rbx,1h
	jnz	406150h

l0000000000406164:
	mov	rax,rbx
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	ret

l0000000000406170:
	mov	rax,555555555555555h
	cmp	rbx,rax
	ja	40619Dh

l000000000040617F:
	lea	rbp,[rbx+rbx]
	lea	rsi,[rbp+rbx+0h]
	shl	rsi,4h
	call	410C90h
	mov	[000000000061B028],rax                                 ; [rip+00214E90]
	jmp	405F44h

l000000000040619D:
	call	410E50h
	nop	word ptr cs:[rax+rax+0h]

;; fn00000000004061B0: 00000000004061B0
;;   Called from:
;;     00000000004061A2 (in fn0000000000405ED0)
;;     0000000000408A74 (in fn0000000000407EA0)
;;     0000000000408B14 (in fn0000000000407EA0)
fn00000000004061B0 proc
	push	rbp
	mov	ebp,edi
	push	rbx
	sub	rsp,28h
	mov	rax,fs:[0028h]
	mov	[rsp+18h],rax
	xor	eax,eax
	cmp	[000000000061B145],0h                                  ; [rip+00214F76]
	jz	406248h

l00000000004061D1:
	mov	r8d,ebp
	mov	ecx,41375Ah
	mov	edx,15h
	mov	esi,1h
	mov	rdi,rsp
	xor	eax,eax
	call	402890h
	mov	rbx,rsp
	mov	rax,rsp

l00000000004061F3:
	mov	ecx,[rax]
	add	rax,4h
	lea	edx,[rcx-1010101h]
	not	ecx
	and	edx,ecx
	and	edx,80808080h
	jz	4061F3h

l000000000040620B:
	mov	ecx,edx
	shr	ecx,10h
	test	edx,8080h
	cmovz	edx,ecx

l0000000000406219:
	lea	rcx,[rax+2h]
	cmovz	rax,rcx

l0000000000406221:
	add	dl,dl
	sbb	rax,3h
	mov	edx,eax
	sub	edx,ebx

l000000000040622B:
	mov	rsi,[rsp+18h]
	xor	rsi,fs:[0028h]
	mov	eax,edx
	jnz	406269h

l000000000040623D:
	add	rsp,28h
	pop	rbx
	pop	rbp
	ret
0000000000406244             0F 1F 40 00                             ..@.        

l0000000000406248:
	call	40C9B0h
	test	rax,rax
	mov	rdi,rax
	jz	4061D1h

l0000000000406259:
	xor	esi,esi
	call	40D420h
	xor	edx,edx
	test	eax,eax
	cmovns	edx,eax

l0000000000406267:
	jmp	40622Bh

l0000000000406269:
	call	4023A0h
000000000040626E                                           66 90               f.
0000000000406270 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 49 89 F1 ......D......I..
0000000000406280 83 F8 09 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 ............A...
0000000000406290 0F 94 C2 41 83 F8 03 40 0F 94 C6 09 F2 08 C8 75 ...A...@.......u
00000000004062A0 27 84 C0 75 13 84 D2 B8 01 00 00 00 74 0A 66 90 '..u........t.f.
00000000004062B0 F3 C3 66 0F 1F 44 00 00 48 8B 37 49 8B 39 E9 0D ..f..D..H.7I.9..
00000000004062C0 45 00 00 0F 1F 44 00 00 84 D2 75 D5 B8 FF FF FF E....D....u.....
00000000004062D0 FF C3 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 ..fffff.........
00000000004062E0 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
00000000004062F0 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 0F 94 C2 .........A......
0000000000406300 41 83 F8 03 41 0F 94 C0 44 09 C2 08 C8 75 21 84 A...A...D....u!.
0000000000406310 C0 75 0D 84 D2 B8 01 00 00 00 74 04 F3 C3 66 90 .u........t...f.
0000000000406320 48 8B 36 48 8B 3F E9 A5 44 00 00 0F 1F 44 00 00 H.6H.?..D....D..
0000000000406330 84 D2 75 DB B8 FF FF FF FF C3 66 0F 1F 44 00 00 ..u.......f..D..
0000000000406340 41 54 55 53 8B 87 A0 00 00 00 48 89 F3 44 8B 86 ATUS......H..D..
0000000000406350 A0 00 00 00 83 F8 09 0F 94 C1 83 F8 03 0F 94 C0 ................
0000000000406360 41 83 F8 09 0F 94 C2 41 83 F8 03 40 0F 94 C6 09 A......A...@....
0000000000406370 F2 08 C8 75 73 84 C0 75 17 84 D2 B8 01 00 00 00 ...us..u........
0000000000406380 74 0E 5B 5D 41 5C C3 66 0F 1F 84 00 00 00 00 00 t.[]A\.f........
0000000000406390 4C 8B 27 BE 2E 00 00 00 4C 89 E7 E8 70 C0 FF FF L.'.....L...p...
00000000004063A0 48 8B 1B BE 2E 00 00 00 48 89 C5 48 89 DF E8 5D H.......H..H...]
00000000004063B0 C0 FF FF BA 19 69 41 00 48 85 C0 48 0F 44 C2 48 .....iA.H..H.D.H
00000000004063C0 85 ED 48 0F 45 D5 48 89 C6 48 89 D7 E8 7F C1 FF ..H.E.H..H......
00000000004063D0 FF 85 C0 75 AD 48 89 DE 4C 89 E7 5B 5D 41 5C E9 ...u.H..L..[]A\.
00000000004063E0 6C C1 FF FF 0F 1F 40 00 84 D2 75 89 B8 FF FF FF l.....@...u.....
00000000004063F0 FF EB 8F 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 ...ffff.........

;; fn0000000000406400: 0000000000406400
;;   Called from:
;;     000000000040647C (in fn0000000000406440)
;;     00000000004067A0 (in fn0000000000406540)
fn0000000000406400 proc
	cmp	[000000000061A408],0h                                  ; [rip+00214000]
	jz	406418h

l000000000040640A:
	mov	edi,61A400h
	jmp	406440h
0000000000406411    0F 1F 80 00 00 00 00                          .......        

l0000000000406418:
	sub	rsp,8h
	mov	edi,61A3E0h
	call	406440h
	mov	edi,61A410h
	call	406440h
	mov	edi,61A3F0h
	add	rsp,8h
	jmp	406440h
000000000040643B                                  0F 1F 44 00 00            ..D..

;; fn0000000000406440: 0000000000406440
;;   Called from:
;;     00000000004040DE (in fn00000000004028C0)
;;     00000000004040E8 (in fn00000000004028C0)
;;     000000000040640F (in fn0000000000406400)
;;     0000000000406421 (in fn0000000000406400)
;;     000000000040642B (in fn0000000000406400)
;;     0000000000406439 (in fn0000000000406400)
;;     0000000000406511 (in fn0000000000406490)
;;     000000000040651B (in fn0000000000406490)
;;     00000000004066E8 (in fn0000000000406540)
;;     00000000004066F0 (in fn0000000000406540)
;;     00000000004066FA (in fn0000000000406540)
;;     00000000004067CB (in fn0000000000406540)
;;     0000000000406845 (in fn0000000000406540)
;;     000000000040684F (in fn0000000000406540)
;;     0000000000406A5D (in fn0000000000406A30)
;;     0000000000406A67 (in fn0000000000406A30)
;;     0000000000406A75 (in fn0000000000406A30)
fn0000000000406440 proc
	sub	rsp,18h
	cmp	[000000000061B128],0h                                  ; [rip+00214CDD]
	jz	406470h

l000000000040644D:
	mov	rsi,[rdi]
	mov	rcx,[000000000061A610]                                 ; [rip+002141B9]
	mov	edx,1h
	mov	rdi,[rdi+8h]
	add	rsp,18h
	jmp	4026C0h
0000000000406469                            0F 1F 80 00 00 00 00          .......

l0000000000406470:
	mov	[rsp+8h],rdi
	mov	[000000000061B128],1h                                  ; [rip+00214CAC]
	call	406400h
	mov	rdi,[rsp+8h]
	jmp	40644Dh
0000000000406488                         0F 1F 84 00 00 00 00 00         ........

;; fn0000000000406490: 0000000000406490
;;   Called from:
;;     0000000000403770 (in fn00000000004028C0)
;;     0000000000406781 (in fn0000000000406540)
fn0000000000406490 proc
	push	rbx
	add	rsp,80h
	jmp	4064EFh
0000000000406497                      66 0F 1F 84 00 00 00 00 00        f........

l00000000004064A0:
	mov	rdi,[000000000061A610]                                 ; [rip+00214169]
	call	402820h
	xor	edi,edi
	mov	rdx,rsp
	mov	esi,61B040h
	call	4021D0h
	mov	ebx,[000000000061B038]                                 ; [rip+00214B77]
	mov	eax,[000000000061B034]                                 ; [rip+00214B6D]
	test	eax,eax
	jz	406528h

l00000000004064CB:
	sub	eax,1h
	mov	ebx,13h
	mov	[000000000061B034],eax                                 ; [rip+00214B5B]

l00000000004064D9:
	mov	edi,ebx
	call	4021E0h
	xor	edx,edx
	mov	rsi,rsp
	mov	edi,2h
	call	4021D0h

l00000000004064EF:
	mov	eax,[000000000061B038]                                 ; [rip+00214B43]
	test	eax,eax
	jnz	406503h

l00000000004064F9:
	mov	eax,[000000000061B034]                                 ; [rip+00214B35]
	test	eax,eax
	jz	406538h

l0000000000406503:
	cmp	[000000000061B128],0h                                  ; [rip+00214C1E]
	jz	4064A0h

l000000000040650C:
	mov	edi,61A3E0h
	call	406440h
	mov	edi,61A3F0h
	call	406440h
	jmp	4064A0h
0000000000406525                0F 1F 00                              ...        

l0000000000406528:
	xor	esi,esi
	mov	edi,ebx
	call	402560h
	jmp	4064D9h
0000000000406533          0F 1F 44 00 00                            ..D..        

l0000000000406538:
	sub	rsp,80h
	pop	rbx
	ret
000000000040653E                                           66 90               f.

;; fn0000000000406540: 0000000000406540
;;   Called from:
;;     0000000000406FE2 (in fn0000000000406B70)
;;     000000000040719D (in fn0000000000406B70)
;;     000000000040791B (in fn0000000000407870)
fn0000000000406540 proc
	push	r15
	push	r14
	push	r13
	mov	r13,rcx
	push	r12
	push	rbp
	mov	rbp,rdi
	push	rbx
	mov	rbx,rdx
	sub	rsp,8h
	test	sil,sil
	mov	rdx,[rdi]
	mov	r12,[rdi+8h]
	jz	4069BDh

l0000000000406567:
	cmp	[000000000061B129],0h                                  ; [rip+00214BBB]
	jnz	406580h

l0000000000406570:
	xor	r14d,r14d
	jmp	4066FFh
0000000000406578                         0F 1F 84 00 00 00 00 00         ........

l0000000000406580:
	movzx	r14d,byte ptr [rbp+0B1h]
	mov	r15d,[rbp+0A4h]
	test	r14b,r14b
	jnz	4065B0h

l0000000000406594:
	mov	edi,0Ch
	call	404CD0h
	test	al,al
	mov	edx,0Ch
	jnz	4066B0h

l00000000004065AB:
	nop	dword ptr [rax+rax+0h]

l00000000004065B0:
	cmp	byte ptr [rbp+0B0h],0h
	jnz	406628h

l00000000004065B9:
	mov	eax,[rbp+0A0h]
	mov	edx,[412C60h+rax*4]
	cmp	edx,5h
	jnz	406680h

l00000000004065CF:
	mov	rdi,r12
	call	402380h
	mov	rbp,[000000000061B120]                                 ; [rip+00214B42]
	mov	r14,rax
	lea	r15,[r12+rax]
	test	rbp,rbp
	jz	406619h

l00000000004065EA:
	nop	word ptr [rax+rax+0h]

l00000000004065F0:
	mov	rdx,[rbp+0h]
	cmp	r14,rdx
	jc	406610h

l00000000004065F9:
	mov	rsi,[rbp+8h]
	mov	rdi,r15
	sub	rdi,rdx
	call	402240h
	test	eax,eax
	jz	4068A0h

l0000000000406610:
	mov	rbp,[rbp+20h]
	test	rbp,rbp
	jnz	4065F0h

l0000000000406619:
	mov	edx,5h
	jmp	4066B0h
0000000000406623          0F 1F 44 00 00                            ..D..        

l0000000000406628:
	mov	eax,r15d
	and	eax,0F000h
	cmp	eax,8000h
	jz	406868h

l000000000040663B:
	cmp	eax,4000h
	jz	4068E0h

l0000000000406646:
	cmp	eax,0A000h
	jz	406859h

l0000000000406651:
	cmp	eax,1000h
	mov	edx,8h
	jz	4066B0h

l000000000040665D:
	cmp	eax,0C000h
	mov	dl,9h
	jz	4066B0h

l0000000000406666:
	cmp	eax,6000h
	mov	dl,0Ah
	jz	4066B0h

l000000000040666F:
	xor	edx,edx
	cmp	eax,2000h
	setnz	dl
	lea	edx,[rdx+rdx+0Bh]
	jmp	4066B0h
000000000040667F                                              90                .

l0000000000406680:
	cmp	edx,7h
	setz	al
	and	r14d,eax

l0000000000406689:
	test	r14b,r14b
	jz	4066B0h

l000000000040668E:
	cmp	[000000000061B198],0h                                  ; [rip+00214B03]
	mov	edx,0Dh
	jnz	4066B0h

l000000000040669C:
	mov	edi,0Dh
	call	404CD0h
	cmp	al,1h
	sbb	edx,edx
	and	edx,0FAh
	add	edx,0Dh

l00000000004066B0:
	mov	ebp,edx
	shl	rbp,4h
	add	rbp,+61A3E0h
	nop	dword ptr [rax]

l00000000004066C0:
	cmp	qword ptr [rbp+8h],0h
	mov	edi,4h
	jz	406890h

l00000000004066D0:
	call	404CD0h
	test	al,al
	jnz	406840h

l00000000004066DD:
	mov	edi,61A3E0h
	mov	r14d,1h
	call	406440h
	mov	rdi,rbp
	call	406440h
	mov	edi,61A3F0h
	call	406440h

l00000000004066FF:
	test	rbx,rbx
	jz	4067D8h

l0000000000406708:
	cmp	[000000000061B130],0h                                  ; [rip+00214A21]
	jz	406732h

l0000000000406711:
	mov	rax,[rbx+18h]
	lea	rdx,[rax+8h]
	cmp	[rbx+20h],rdx
	jc	406820h

l0000000000406723:
	mov	rdx,[000000000061B018]                                 ; [rip+002148EE]
	mov	[rax],rdx
	add	qword ptr [rbx+18h],8h

l0000000000406732:
	mov	rdx,[000000000061B0E8]                                 ; [rip+002149AF]
	mov	rdi,[000000000061A610]                                 ; [rip+00213ED0]
	xor	ecx,ecx
	mov	rsi,r12
	call	4052D0h
	mov	rdx,rax
	add	rdx,[000000000061B018]                                 ; [rip+002148C4]
	cmp	[000000000061B130],0h                                  ; [rip+002149D5]
	mov	rbp,rax
	mov	[000000000061B018],rdx                                 ; [rip+002148B3]
	jz	406781h

l0000000000406767:
	mov	rax,[rbx+18h]
	lea	rcx,[rax+8h]
	cmp	[rbx+20h],rcx
	jc	406800h

l0000000000406779:
	mov	[rax],rdx
	add	qword ptr [rbx+18h],8h

l0000000000406781:
	call	406490h
	test	r14b,r14b
	jnz	4067A0h

l000000000040678B:
	add	rsp,8h
	mov	rax,rbp
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040679D                                        0F 1F 00              ...

l00000000004067A0:
	call	406400h
	mov	rcx,[000000000061B0C8]                                 ; [rip+0021491C]
	xor	edx,edx
	mov	rax,r13
	div	rcx
	xor	edx,edx
	mov	rsi,rax
	lea	rax,[rbp+r13-1h]
	div	rcx
	cmp	rsi,rax
	jz	40678Bh

l00000000004067C6:
	mov	edi,61A550h
	call	406440h
	jmp	40678Bh
00000000004067D2       66 0F 1F 44 00 00                           f..D..        

l00000000004067D8:
	mov	rdx,[000000000061B0E8]                                 ; [rip+00214909]
	mov	rdi,[000000000061A610]                                 ; [rip+00213E2A]
	xor	ecx,ecx
	mov	rsi,r12
	call	4052D0h
	mov	rbp,rax
	add	[000000000061B018],rax                                 ; [rip+0021481E]
	jmp	406781h
00000000004067FC                                     0F 1F 40 00             ..@.

l0000000000406800:
	mov	esi,8h
	mov	rdi,rbx
	call	402720h
	mov	rax,[rbx+18h]
	mov	rdx,[000000000061B018]                                 ; [rip+00214800]
	jmp	406779h
000000000040681D                                        0F 1F 00              ...

l0000000000406820:
	mov	esi,8h
	mov	rdi,rbx
	call	402720h
	mov	rax,[rbx+18h]
	jmp	406723h
0000000000406836                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000406840:
	mov	edi,61A3E0h
	call	406440h
	mov	edi,61A3F0h
	call	406440h
	jmp	4066DDh

l0000000000406859:
	mov	edx,7h
	jmp	406689h
0000000000406863          0F 1F 44 00 00                            ..D..        

l0000000000406868:
	test	r15d,800h
	jz	4068B8h

l0000000000406871:
	mov	edi,10h
	call	404CD0h
	test	al,al
	jz	4068B8h

l000000000040687F:
	mov	edx,10h
	jmp	4066B0h
0000000000406889                            0F 1F 80 00 00 00 00          .......

l0000000000406890:
	call	404CD0h
	mov	r14d,eax
	jmp	4066FFh
000000000040689D                                        0F 1F 00              ...

l00000000004068A0:
	test	rbp,rbp
	jz	406619h

l00000000004068A9:
	add	rbp,10h
	jmp	4066C0h
00000000004068B2       66 0F 1F 44 00 00                           f..D..        

l00000000004068B8:
	test	r15d,400h
	jz	40693Bh

l00000000004068C1:
	mov	edi,11h
	call	404CD0h
	test	al,al
	jz	40693Bh

l00000000004068CF:
	mov	edx,11h
	jmp	4066B0h
00000000004068D9                            0F 1F 80 00 00 00 00          .......

l00000000004068E0:
	mov	eax,r15d
	and	eax,202h
	cmp	eax,202h
	jz	4069A1h

l00000000004068F3:
	test	r15b,2h
	jz	406910h

l00000000004068F9:
	mov	edi,13h
	call	404CD0h
	test	al,al
	mov	edx,13h
	jnz	4066B0h

l0000000000406910:
	and	r15d,200h
	mov	edx,6h
	jz	4066B0h

l0000000000406922:
	mov	edi,12h
	call	404CD0h
	cmp	al,1h
	sbb	edx,edx
	and	edx,0F4h
	add	edx,12h
	jmp	4066B0h

l000000000040693B:
	mov	edi,15h
	call	404CD0h
	test	al,al
	jz	40695Ch

l0000000000406949:
	cmp	byte ptr [rbp+0B8h],0h
	jz	40695Ch

l0000000000406952:
	mov	edx,15h
	jmp	4066B0h

l000000000040695C:
	and	r15d,49h
	jz	40697Ah

l0000000000406962:
	mov	edi,0Eh
	call	404CD0h
	test	al,al
	jz	40697Ah

l0000000000406970:
	mov	edx,0Eh
	jmp	4066B0h

l000000000040697A:
	cmp	qword ptr [rbp+20h],1h
	jbe	4065CFh

l0000000000406985:
	mov	edi,16h
	call	404CD0h
	test	al,al
	jz	4065CFh

l0000000000406997:
	mov	edx,16h
	jmp	4066B0h

l00000000004069A1:
	mov	edi,14h
	call	404CD0h
	test	al,al
	mov	edx,14h
	jnz	4066B0h

l00000000004069B8:
	jmp	4068F3h

l00000000004069BD:
	cmp	[000000000061B129],0h                                  ; [rip+00214765]
	jnz	4069D8h

l00000000004069C6:
	mov	r12,rdx
	xor	r14d,r14d
	jmp	4066FFh
00000000004069D1    0F 1F 80 00 00 00 00                          .......        

l00000000004069D8:
	cmp	[000000000061B198],0h                                  ; [rip+002147B9]
	jz	406A08h

l00000000004069E1:
	cmp	byte ptr [rbp+0B1h],0h
	jz	406A20h

l00000000004069EA:
	mov	r15d,[rbp+0A4h]
	mov	r14d,1h

l00000000004069F7:
	xor	r14d,1h
	mov	r12,rdx
	jmp	4065B0h
0000000000406A03          0F 1F 44 00 00                            ..D..        

l0000000000406A08:
	movzx	r14d,byte ptr [rbp+0B1h]

l0000000000406A10:
	mov	r15d,[rbp+28h]
	jmp	4069F7h
0000000000406A16                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000406A20:
	xor	r14d,r14d
	jmp	406A10h
0000000000406A25                66 66 2E 0F 1F 84 00 00 00 00 00      ff.........

;; fn0000000000406A30: 0000000000406A30
;;   Called from:
;;     0000000000407892 (in fn0000000000407870)
;;     0000000000407C18 (in fn00000000004079F0)
fn0000000000406A30 proc
	cmp	[000000000061B129],0h                                  ; [rip+002146F2]
	jz	406A4Fh

l0000000000406A39:
	sub	rsp,8h
	mov	edi,4h
	call	404CD0h
	test	al,al
	jnz	406A58h

l0000000000406A4B:
	add	rsp,8h

l0000000000406A4F:
	ret
0000000000406A51    0F 1F 80 00 00 00 00                          .......        

l0000000000406A58:
	mov	edi,61A3E0h
	call	406440h
	mov	edi,61A420h
	call	406440h
	mov	edi,61A3F0h
	add	rsp,8h
	jmp	406440h
0000000000406A7A                               66 0F 1F 44 00 00           f..D..

;; fn0000000000406A80: 0000000000406A80
;;   Called from:
;;     0000000000406F31 (in fn0000000000406B70)
;;     00000000004073E4 (in fn0000000000406B70)
fn0000000000406A80 proc
	push	r14
	push	r13
	mov	r13d,ecx
	push	r12
	mov	r12,rdi
	push	rbp
	mov	rbp,rdx
	push	rbx
	mov	rbx,rsi
	sub	rsp,+110h
	mov	rax,fs:[0028h]
	mov	[rsp+108h],rax
	xor	eax,eax
	cmp	[000000000061A748],0h                                  ; [rip+00213C92]
	jz	406ADBh

l0000000000406AB8:
	mov	esi,413766h
	mov	rdi,rbx
	call	402860h
	test	rax,rax
	mov	r14,rax
	jz	406ADBh

l0000000000406ACD:
	mov	rdi,rbx
	call	402380h
	cmp	rax,65h
	jbe	406B20h

l0000000000406ADB:
	xor	r8d,r8d
	mov	rcx,rbp
	mov	r9d,r13d
	mov	rdx,rbx
	mov	esi,3E9h
	mov	rdi,r12
	call	410600h
	mov	rcx,[rsp+108h]
	xor	rcx,fs:[0028h]
	jnz	406B66h

l0000000000406B07:
	add	rsp,+110h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	ret
0000000000406B17                      66 0F 1F 84 00 00 00 00 00        f........

l0000000000406B20:
	mov	rdx,r14
	mov	rsi,rbx
	mov	ecx,105h
	sub	rdx,rbx
	mov	rdi,rsp
	mov	rbx,rsp
	call	402210h
	movsxd	rcx,dword ptr [rbp+10h]
	mov	rdi,rax
	lea	rdx,[rcx+rcx*4]
	shl	rdx,5h
	lea	rsi,[rcx+rdx+61A760h]
	call	402350h
	lea	rsi,[r14+2h]
	mov	rdi,rax
	call	402260h
	jmp	406ADBh

l0000000000406B66:
	call	4023A0h
0000000000406B6B                                  0F 1F 44 00 00            ..D..

;; fn0000000000406B70: 0000000000406B70
;;   Called from:
;;     0000000000407C28 (in fn00000000004079F0)
fn0000000000406B70 proc
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbp
	push	rbx
	mov	rbx,rdi
	sub	rsp,+12B8h
	mov	rax,fs:[0028h]
	mov	[rsp+12A8h],rax
	xor	eax,eax
	cmp	byte ptr [rdi+0B0h],0h
	jz	406E00h

l0000000000406BA4:
	lea	r13,[rsp+40h]
	lea	rdi,[rdi+10h]
	mov	rsi,r13
	call	40A600h
	cmp	[000000000061B17C],0h                                  ; [rip+002145C0]
	jnz	406E41h

l0000000000406BC2:
	mov	byte ptr [rsp+4Ah],0h

l0000000000406BC7:
	mov	eax,[000000000061B14C]                                 ; [rip+0021457F]
	cmp	eax,1h
	jz	406E70h

l0000000000406BD6:
	jc	407040h

l0000000000406BDC:
	cmp	eax,2h
	jnz	406E90h

l0000000000406BE5:
	mov	rax,[rbx+60h]
	mov	rdx,[rbx+58h]
	mov	[rsp+38h],rax
	mov	[rsp+30h],rdx

l0000000000406BF7:
	cmp	[000000000061B114],0h                                  ; [rip+00214516]
	lea	r12,[rsp+460h]
	mov	rbp,r12
	jnz	407060h

l0000000000406C0F:
	cmp	[000000000061B144],0h                                  ; [rip+0021452E]
	jz	406C8Fh

l0000000000406C18:
	cmp	byte ptr [rbx+0B0h],0h
	mov	r14d,413764h
	jnz	407288h

l0000000000406C2B:
	mov	r15d,[000000000061B174]                                ; [rip+00214542]
	xor	esi,esi
	mov	rdi,r14
	call	40D420h
	sub	r15d,eax
	test	r15d,r15d
	mov	eax,r15d
	jle	406F50h

l0000000000406C4B:
	sub	eax,1h
	mov	edx,eax
	lea	rcx,[rbp+rdx+1h]
	mov	rdx,rbp
	nop	dword ptr [rax+rax+0h]

l0000000000406C60:
	add	rdx,1h
	mov	byte ptr [rdx-1h],20h
	cmp	rdx,rcx
	jnz	406C60h

l0000000000406C6D:
	cdqe
	lea	rdx,[rbp+rax+1h]

l0000000000406C74:
	add	r14,1h
	movzx	eax,byte ptr [r14-1h]
	lea	rbp,[rdx+1h]
	test	al,al
	mov	[rbp-1h],al
	jnz	406F50h

l0000000000406C8C:
	mov	byte ptr [rdx],20h

l0000000000406C8F:
	cmp	byte ptr [rbx+0B0h],0h
	mov	eax,413764h
	jnz	407120h

l0000000000406CA1:
	mov	r9d,[000000000061B170]                                 ; [rip+002144C8]
	mov	rdi,rbp
	mov	[rsp],rax
	mov	r8,r13
	mov	ecx,413769h
	mov	rdx,-1h
	mov	esi,1h
	xor	eax,eax
	call	402890h
	mov	rdi,rbp
	call	402380h
	add	rbp,rax
	cmp	[000000000061B130],0h                                  ; [rip+00214454]
	jnz	4070F0h

l0000000000406CE2:
	cmp	[000000000061A569],0h                                  ; [rip+00213880]
	jnz	406D08h

l0000000000406CEB:
	cmp	[000000000061A568],0h                                  ; [rip+00213876]
	jnz	406D08h

l0000000000406CF4:
	cmp	[000000000061B146],0h                                  ; [rip+0021444B]
	jz	407148h

l0000000000406D01:
	nop	dword ptr [rax+0h]

l0000000000406D08:
	mov	rsi,[000000000061A610]                                 ; [rip+00213901]
	mov	rdi,r12
	sub	rbp,r12
	call	402520h
	add	[000000000061B018],rbp                                 ; [rip+002142F7]
	cmp	[000000000061A569],0h                                  ; [rip+00213841]
	jnz	407248h

l0000000000406D2E:
	cmp	[000000000061A568],0h                                  ; [rip+00213833]
	jnz	407210h

l0000000000406D3B:
	cmp	[000000000061B146],0h                                  ; [rip+00214404]
	jnz	4071D0h

l0000000000406D48:
	cmp	[000000000061B17D],0h                                  ; [rip+0021442E]
	mov	rbp,r12
	jnz	4071F5h

l0000000000406D58:
	cmp	byte ptr [rbx+0B0h],0h
	jz	406F60h

l0000000000406D65:
	mov	eax,[rbx+28h]
	and	eax,0B000h
	cmp	eax,2000h
	jz	4072B0h

l0000000000406D78:
	mov	rdi,[rbx+40h]
	mov	r8,[000000000061A560]                                  ; [rip+002137DD]
	lea	rsi,[rsp+70h]
	mov	edx,[000000000061B134]                                 ; [rip+002143A6]
	mov	ecx,1h
	call	40BD70h
	mov	r14,rax

l0000000000406D9B:
	mov	r13d,[000000000061B154]                                ; [rip+002143B2]
	xor	esi,esi
	mov	rdi,r14
	call	40D420h
	sub	r13d,eax
	test	r13d,r13d
	mov	eax,r13d
	jle	406DDCh

l0000000000406DB7:
	sub	eax,1h
	mov	edx,eax
	lea	rcx,[rbp+rdx+1h]
	mov	rdx,rbp
	nop	dword ptr [rax+0h]

l0000000000406DC8:
	add	rdx,1h
	mov	byte ptr [rdx-1h],20h
	cmp	rdx,rcx
	jnz	406DC8h

l0000000000406DD5:
	cdqe
	lea	rbp,[rbp+rax+1h]

l0000000000406DDC:
	add	r14,1h
	movzx	eax,byte ptr [r14-1h]
	lea	r13,[rbp+1h]
	test	al,al
	mov	[r13-1h],al
	jz	406E98h

l0000000000406DF5:
	mov	rbp,r13
	jmp	406DDCh
0000000000406DFA                               66 0F 1F 44 00 00           f..D..

l0000000000406E00:
	mov	eax,[rdi+0A0h]
	cmp	[000000000061B17C],0h                                  ; [rip+0021436F]
	lea	r13,[rsp+40h]
	mov	ecx,3F3Fh
	movzx	eax,byte ptr [rax+413728h]
	mov	[rsp+40h],al
	mov	rax,3F3F3F3F3F3F3F3Fh
	mov	[rsp+41h],rax
	mov	[r13+9h],cx
	mov	byte ptr [rsp+4Bh],0h
	jz	406BC2h

l0000000000406E41:
	mov	eax,[rbx+0B4h]
	cmp	eax,1h
	jz	407138h

l0000000000406E50:
	cmp	eax,2h
	jnz	406BC7h

l0000000000406E59:
	mov	eax,[000000000061B14C]                                 ; [rip+002142ED]
	mov	byte ptr [rsp+4Ah],2Bh
	cmp	eax,1h
	jnz	406BD6h

l0000000000406E6D:
	nop	dword ptr [rax]

l0000000000406E70:
	mov	rax,[rbx+80h]
	mov	rdx,[rbx+78h]
	mov	[rsp+38h],rax
	mov	[rsp+30h],rdx
	jmp	406BF7h
0000000000406E8A                               66 0F 1F 44 00 00           f..D..

l0000000000406E90:
	call	402220h
0000000000406E95                0F 1F 00                              ...        

l0000000000406E98:
	mov	byte ptr [rbp+0h],20h

l0000000000406E9C:
	lea	rdi,[rsp+30h]
	call	402200h
	mov	byte ptr [r13+0h],1h
	cmp	byte ptr [rbx+0B0h],0h
	jz	406F80h

l0000000000406EB8:
	test	rax,rax
	jz	407364h

l0000000000406EC1:
	mov	rdx,[000000000061B180]                                 ; [rip+002142B8]
	mov	rsi,[rsp+30h]
	mov	rdi,[000000000061B188]                                 ; [rip+002142B4]
	mov	rcx,[rsp+38h]
	cmp	rsi,rdx
	jg	407380h

l0000000000406EE2:
	jl	406EECh

l0000000000406EE4:
	cmp	edi,ecx
	js	407380h

l0000000000406EEC:
	mov	r8,rdi

l0000000000406EEF:
	lea	rdi,[rdx-0F0C2ACh]
	cmp	rdi,rsi
	jge	407268h

l0000000000406EFF:
	cmp	rdx,rsi
	mov	edi,1h
	jg	406F20h

l0000000000406F09:
	mov	dil,0h
	jl	406F20h

l0000000000406F0E:
	mov	edi,ecx
	sub	edi,r8d
	shr	edi,1Fh
	nop	word ptr cs:[rax+rax+0h]

l0000000000406F20:
	movsxd	rdi,edi
	mov	rdx,rax
	mov	rsi,[61A3D0h+rdi*8]
	mov	rdi,r13
	call	406A80h
	test	rax,rax
	jz	406F6Bh

l0000000000406F3B:
	add	rax,r13
	lea	r13,[rax+1h]
	mov	byte ptr [rax],20h
	mov	byte ptr [rax+1h],0h
	jmp	406FBCh
0000000000406F4B                                  0F 1F 44 00 00            ..D..

l0000000000406F50:
	mov	rdx,rbp
	jmp	406C74h
0000000000406F58                         0F 1F 84 00 00 00 00 00         ........

l0000000000406F60:
	mov	r14d,413764h
	jmp	406D9Bh

l0000000000406F6B:
	cmp	byte ptr [r13+0h],0h
	jz	406F3Bh

l0000000000406F72:
	cmp	byte ptr [rbx+0B0h],0h
	jnz	407364h

l0000000000406F7F:
	nop

l0000000000406F80:
	mov	r9d,413764h

l0000000000406F86:
	mov	r8d,[000000000061A3C4]                                 ; [rip+00213437]
	test	r8d,r8d
	js	4073B1h

l0000000000406F96:
	mov	rdi,r13
	mov	ecx,413779h
	mov	rdx,-1h
	mov	esi,1h
	xor	eax,eax
	call	402890h
	mov	rdi,r13
	call	402380h
	add	r13,rax

l0000000000406FBC:
	mov	rsi,[000000000061A610]                                 ; [rip+0021364D]
	sub	r13,r12
	mov	rdi,r12
	call	402520h
	mov	edx,61AFC0h
	xor	esi,esi
	mov	rcx,r13
	mov	rdi,rbx
	add	[000000000061B018],r13                                 ; [rip+00214036]
	call	406540h
	mov	edx,[rbx+0A0h]
	mov	rbp,rax
	cmp	edx,6h
	jz	407160h

l0000000000406FF9:
	mov	eax,[000000000061B12C]                                 ; [rip+0021412D]
	test	eax,eax
	jz	407012h

l0000000000407003:
	movzx	edi,byte ptr [rbx+0B0h]
	mov	esi,[rbx+28h]
	call	405D00h

l0000000000407012:
	mov	rax,[rsp+12A8h]
	xor	rax,fs:[0028h]
	jnz	40745Ah

l0000000000407029:
	add	rsp,+12B8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040703B                                  0F 1F 44 00 00            ..D..

l0000000000407040:
	mov	rax,[rbx+70h]
	mov	rdx,[rbx+68h]
	mov	[rsp+38h],rax
	mov	[rsp+30h],rdx
	jmp	406BF7h
0000000000407057                      66 0F 1F 84 00 00 00 00 00        f........

l0000000000407060:
	cmp	byte ptr [rbx+0B0h],0h
	mov	r9d,413764h
	jz	407085h

l000000000040706F:
	mov	rdi,[rbx+18h]
	test	rdi,rdi
	jz	407085h

l0000000000407078:
	lea	rsi,[rsp+70h]
	call	40CD70h
	mov	r9,rax

l0000000000407085:
	lea	r12,[rsp+460h]
	mov	r8d,[000000000061B178]                                 ; [rip+002140E4]
	mov	edx,0E3Bh
	mov	ecx,413779h
	mov	esi,1h
	xor	eax,eax
	mov	rdi,r12
	call	402890h
	mov	rdx,r12

l00000000004070B0:
	mov	ecx,[rdx]
	add	rdx,4h
	lea	eax,[rcx-1010101h]
	not	ecx
	and	eax,ecx
	and	eax,80808080h
	jz	4070B0h

l00000000004070C7:
	mov	ecx,eax
	lea	rbp,[rdx+2h]
	shr	ecx,10h
	test	eax,8080h
	cmovz	eax,ecx

l00000000004070D8:
	cmovnz	rbp,rdx

l00000000004070DC:
	add	al,al
	sbb	rbp,3h
	jmp	406C0Fh
00000000004070E7                      66 0F 1F 84 00 00 00 00 00        f........

l00000000004070F0:
	mov	rcx,[000000000061A610]                                 ; [rip+00213519]
	mov	edx,2h
	mov	esi,1h
	mov	edi,413771h
	call	4026C0h
	add	[000000000061B018],2h                                  ; [rip+00213F05]
	jmp	406CE2h
0000000000407118                         0F 1F 84 00 00 00 00 00         ........

l0000000000407120:
	mov	rdi,[rbx+20h]
	lea	rsi,[rsp+70h]
	call	40CD70h
	jmp	406CA1h
0000000000407133          0F 1F 44 00 00                            ..D..        

l0000000000407138:
	mov	byte ptr [rsp+4Ah],2Eh
	jmp	406BC7h
0000000000407142       66 0F 1F 44 00 00                           f..D..        

l0000000000407148:
	cmp	[000000000061B17D],0h                                  ; [rip+0021402E]
	jz	406D58h

l0000000000407155:
	jmp	406D08h
000000000040715A                               66 0F 1F 44 00 00           f..D..

l0000000000407160:
	cmp	qword ptr [rbx+8h],0h
	jz	407012h

l000000000040716B:
	mov	rcx,[000000000061A610]                                 ; [rip+0021349E]
	mov	edx,4h
	mov	esi,1h
	mov	edi,41377Eh
	call	4026C0h
	lea	rcx,[r13+rbp+4h]
	xor	edx,edx
	mov	esi,1h
	mov	rdi,rbx
	add	[000000000061B018],4h                                  ; [rip+00213E7B]
	call	406540h
	mov	edx,[000000000061B12C]                                 ; [rip+00213F84]
	test	edx,edx
	jz	407012h

l00000000004071B0:
	mov	esi,[rbx+0A4h]
	xor	edx,edx
	mov	edi,1h
	call	405D00h
	jmp	407012h
00000000004071C7                      66 0F 1F 84 00 00 00 00 00        f........

l00000000004071D0:
	movzx	edx,byte ptr [rbx+0B0h]
	mov	edi,[rbx+2Ch]
	mov	rbp,r12
	mov	esi,[000000000061B160]                                 ; [rip+00213F7D]
	call	4057B0h
	cmp	[000000000061B17D],0h                                  ; [rip+00213F8E]
	jz	406D58h

l00000000004071F5:
	mov	rdi,[rbx+0A8h]
	mov	edx,[000000000061B16C]                                 ; [rip+00213F6A]
	xor	esi,esi
	call	405700h
	jmp	406D58h
000000000040720E                                           66 90               f.

l0000000000407210:
	cmp	byte ptr [rbx+0B0h],0h
	mov	eax,[rbx+30h]
	mov	edi,413764h
	mov	edx,[000000000061B164]                                 ; [rip+00213F3F]
	mov	esi,eax
	jz	407238h

l0000000000407229:
	xor	edi,edi
	cmp	[000000000061B145],0h                                  ; [rip+00213F13]
	jz	407415h

l0000000000407238:
	call	405700h
	jmp	406D3Bh
0000000000407242       66 0F 1F 44 00 00                           f..D..        

l0000000000407248:
	movzx	edx,byte ptr [rbx+0B0h]
	mov	edi,[rbx+2Ch]
	mov	esi,[000000000061B168]                                 ; [rip+00213F10]
	call	4057B0h
	jmp	406D2Eh
0000000000407262       66 0F 1F 44 00 00                           f..D..        

l0000000000407268:
	mov	edi,0h
	jg	406F20h

l0000000000407273:
	cmp	r8d,ecx
	jns	406F20h

l000000000040727C:
	jmp	406EFFh
0000000000407281    0F 1F 80 00 00 00 00                          .......        

l0000000000407288:
	mov	rdi,[rbx+50h]
	mov	r8,[000000000061B138]                                  ; [rip+00213EA5]
	lea	rsi,[rsp+70h]
	mov	edx,[000000000061B140]                                 ; [rip+00213EA2]
	mov	ecx,200h
	call	40BD70h
	mov	r14,rax
	jmp	406C2Bh

l00000000004072B0:
	mov	rax,[rbx+38h]
	lea	rsi,[rsp+70h]
	mov	r13d,0FFFFFFFEh
	sub	r13d,[000000000061B15C]                                ; [rip+00213E96]
	sub	r13d,[000000000061B158]                                ; [rip+00213E8B]
	mov	rdi,rax
	movzx	eax,al
	add	r13d,[000000000061B154]                                ; [rip+00213E7A]
	shr	rdi,0Ch
	and	dil,0h
	or	edi,eax
	call	40CD70h
	mov	rdx,[rbx+38h]
	mov	r15,rax
	lea	rsi,[rsp+50h]
	mov	r14d,[000000000061B158]                                ; [rip+00213E5C]
	mov	rdi,rdx
	shr	rdx,8h
	mov	eax,edx
	shr	rdi,20h
	and	eax,0FFFh
	and	edi,0FFFFF000h
	or	edi,eax
	call	40CD70h
	xor	r8d,r8d
	test	r13d,r13d
	mov	r9,rax
	cmovns	r8d,r13d

l0000000000407328:
	add	r8d,[000000000061B15C]                                 ; [rip+00213E2D]
	mov	[rsp+8h],r15
	mov	[rsp],r14d
	mov	ecx,413774h
	mov	rdx,-1h
	mov	esi,1h
	mov	rdi,rbp
	xor	eax,eax
	call	402890h
	movsxd	rax,[000000000061B154]                              ; [rip+00213DFA]
	lea	r13,[rbp+rax+1h]
	jmp	406E9Ch

l0000000000407364:
	mov	rdi,[rsp+30h]
	lea	rsi,[rsp+50h]
	call	40CCD0h
	mov	r9,rax
	jmp	406F86h
000000000040737B                                  0F 1F 44 00 00            ..D..

l0000000000407380:
	mov	edi,61B180h
	mov	[rsp+10h],rax
	call	40AB30h
	mov	rdx,[000000000061B180]                                 ; [rip+00213DEA]
	mov	r8,[000000000061B188]                                  ; [rip+00213DEB]
	mov	rsi,[rsp+30h]
	mov	rcx,[rsp+38h]
	mov	rax,[rsp+10h]
	jmp	406EEFh

l00000000004073B1:
	lea	rdi,[rsp+28h]
	mov	[rsp+10h],r9
	mov	qword ptr [rsp+28h],+0h
	call	402200h
	test	rax,rax
	mov	r9,[rsp+10h]
	jz	407403h

l00000000004073D3:
	mov	rsi,[000000000061A3D0]                                 ; [rip+00212FF6]
	lea	rdi,[rsp+70h]
	xor	ecx,ecx
	mov	rdx,rax
	call	406A80h
	test	rax,rax
	mov	r8d,[000000000061A3C4]                                 ; [rip+00212FD1]
	mov	r9,[rsp+10h]
	jnz	407436h

l00000000004073FA:
	test	r8d,r8d
	jns	406F96h

l0000000000407403:
	mov	[000000000061A3C4],0h                                  ; [rip+00212FB7]
	xor	r8d,r8d
	jmp	406F96h

l0000000000407415:
	mov	edi,eax
	mov	[rsp+18h],rsi
	mov	[rsp+10h],edx
	call	40CB40h
	mov	rsi,[rsp+18h]
	mov	rdi,rax
	mov	edx,[rsp+10h]
	jmp	407238h

l0000000000407436:
	lea	rdi,[rsp+70h]
	xor	edx,edx
	mov	rsi,rax
	mov	[rsp+10h],r9
	call	40D240h
	mov	r9,[rsp+10h]
	mov	[000000000061A3C4],eax                                 ; [rip+00212F6F]
	mov	r8d,eax
	jmp	4073FAh

l000000000040745A:
	call	4023A0h
000000000040745F                                              90                .
0000000000407460 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
0000000000407470 0F 94 C1 83 F8 03 0F 94 C2 41 83 F8 09 0F 94 C0 .........A......
0000000000407480 41 83 F8 03 41 0F 94 C0 44 09 C0 08 CA 75 21 84 A...A...D....u!.
0000000000407490 D2 74 2D 48 8B 4F 40 48 39 4E 40 48 8B 06 48 8B .t-H.O@H9N@H..H.
00000000004074A0 17 7F 15 7C 1F 48 89 D6 48 89 C7 E9 A0 B0 FF FF ...|.H..H.......
00000000004074B0 84 C0 75 DB 0F 1F 40 00 B8 FF FF FF FF C3 66 90 ..u...@.......f.
00000000004074C0 84 C0 74 CF B8 01 00 00 00 C3 66 0F 1F 44 00 00 ..t.......f..D..
00000000004074D0 48 8B 4E 40 48 39 4F 40 48 8B 07 48 8B 16 7F 10 H.N@H9O@H..H....
00000000004074E0 7C 1E 48 89 D6 48 89 C7 E9 33 DB FF FF 0F 1F 00 |.H..H...3......
00000000004074F0 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407500 B8 01 00 00 00 C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407510 48 8B 4E 40 48 39 4F 40 48 8B 07 48 8B 16 7F 10 H.N@H9O@H..H....
0000000000407520 7C 1E 48 89 D6 48 89 C7 E9 23 B0 FF FF 0F 1F 00 |.H..H...#......
0000000000407530 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407540 B8 01 00 00 00 C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407550 48 8B 4F 40 48 39 4E 40 48 8B 06 48 8B 17 7F 10 H.O@H9N@H..H....
0000000000407560 7C 1E 48 89 D6 48 89 C7 E9 B3 DA FF FF 0F 1F 00 |.H..H..........
0000000000407570 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407580 B8 01 00 00 00 C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407590 48 8B 4F 40 48 39 4E 40 48 8B 06 48 8B 17 7F 10 H.O@H9N@H..H....
00000000004075A0 7C 1E 48 89 D6 48 89 C7 E9 A3 AF FF FF 0F 1F 00 |.H..H..........
00000000004075B0 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
00000000004075C0 B8 01 00 00 00 C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
00000000004075D0 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
00000000004075E0 0F 94 C1 83 F8 03 0F 94 C2 41 83 F8 09 0F 94 C0 .........A......
00000000004075F0 41 83 F8 03 41 0F 94 C0 44 09 C0 08 CA 75 21 84 A...A...D....u!.
0000000000407600 D2 74 2D 48 8B 4E 40 48 39 4F 40 48 8B 07 48 8B .t-H.N@H9O@H..H.
0000000000407610 16 7F 15 7C 1F 48 89 D6 48 89 C7 E9 00 DA FF FF ...|.H..H.......
0000000000407620 84 C0 75 DB 0F 1F 40 00 B8 FF FF FF FF C3 66 90 ..u...@.......f.
0000000000407630 84 C0 74 CF B8 01 00 00 00 C3 66 0F 1F 44 00 00 ..t.......f..D..
0000000000407640 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
0000000000407650 0F 94 C1 83 F8 03 0F 94 C2 41 83 F8 09 0F 94 C0 .........A......
0000000000407660 41 83 F8 03 41 0F 94 C0 44 09 C0 08 CA 75 21 84 A...A...D....u!.
0000000000407670 D2 74 2D 48 8B 4E 40 48 39 4F 40 48 8B 07 48 8B .t-H.N@H9O@H..H.
0000000000407680 16 7F 15 7C 1F 48 89 D6 48 89 C7 E9 C0 AE FF FF ...|.H..H.......
0000000000407690 84 C0 75 DB 0F 1F 40 00 B8 FF FF FF FF C3 66 90 ..u...@.......f.
00000000004076A0 84 C0 74 CF B8 01 00 00 00 C3 66 0F 1F 44 00 00 ..t.......f..D..
00000000004076B0 48 8B 4E 78 48 39 4F 78 48 8B 97 80 00 00 00 48 H.NxH9OxH......H
00000000004076C0 8B 86 80 00 00 00 7F 18 7C 26 29 D0 75 27 48 8B ........|&).u'H.
00000000004076D0 36 48 8B 3F E9 47 D9 FF FF 0F 1F 80 00 00 00 00 6H.?.G..........
00000000004076E0 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
00000000004076F0 B8 01 00 00 00 C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407700 48 89 F0 48 8B 96 80 00 00 00 48 8B 77 78 48 39 H..H......H.wxH9
0000000000407710 70 78 48 8B 8F 80 00 00 00 7F 15 7C 23 29 D1 75 pxH........|#).u
0000000000407720 25 48 8B 37 48 8B 38 E9 F4 D8 FF FF 0F 1F 40 00 %H.7H.8.......@.
0000000000407730 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407740 B8 01 00 00 00 C3 89 C8 C3 0F 1F 80 00 00 00 00 ................
0000000000407750 48 8B 4E 58 48 39 4F 58 48 8B 57 60 48 8B 46 60 H.NXH9OXH.W`H.F`
0000000000407760 7F 16 7C 1C 29 D0 75 1D 48 8B 36 48 8B 3F E9 AD ..|.).u.H.6H.?..
0000000000407770 D8 FF FF 0F 1F 44 00 00 B8 FF FF FF FF C3 66 90 .....D........f.
0000000000407780 B8 01 00 00 00 C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407790 48 89 F0 48 8B 56 60 48 8B 77 58 48 39 70 58 48 H..H.V`H.wXH9pXH
00000000004077A0 8B 4F 60 7F 1B 7C 29 29 D1 75 2B 48 8B 37 48 8B .O`..|)).u+H.7H.
00000000004077B0 38 E9 6A D8 FF FF 66 2E 0F 1F 84 00 00 00 00 00 8.j...f.........
00000000004077C0 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
00000000004077D0 B8 01 00 00 00 C3 89 C8 C3 0F 1F 80 00 00 00 00 ................
00000000004077E0 48 8B 4E 68 48 39 4F 68 48 8B 57 70 48 8B 46 70 H.NhH9OhH.WpH.Fp
00000000004077F0 7F 16 7C 1C 29 D0 75 1D 48 8B 36 48 8B 3F E9 1D ..|.).u.H.6H.?..
0000000000407800 D8 FF FF 0F 1F 44 00 00 B8 FF FF FF FF C3 66 90 .....D........f.
0000000000407810 B8 01 00 00 00 C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407820 48 89 F0 48 8B 56 70 48 8B 77 68 48 39 70 68 48 H..H.VpH.whH9phH
0000000000407830 8B 4F 70 7F 1B 7C 29 29 D1 75 2B 48 8B 37 48 8B .Op..|)).u+H.7H.
0000000000407840 38 E9 DA D7 FF FF 66 2E 0F 1F 84 00 00 00 00 00 8.....f.........
0000000000407850 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000407860 B8 01 00 00 00 C3 89 C8 C3 0F 1F 80 00 00 00 00 ................

;; fn0000000000407870: 0000000000407870
;;   Called from:
;;     0000000000407A8D (in fn00000000004079F0)
;;     0000000000407AE8 (in fn00000000004079F0)
;;     0000000000407B6A (in fn00000000004079F0)
;;     0000000000407C95 (in fn00000000004079F0)
;;     0000000000407D9A (in fn00000000004079F0)
fn0000000000407870 proc
	push	rbp
	mov	rbp,rsi
	push	rbx
	mov	rbx,rdi
	sub	rsp,+2A8h
	mov	rax,fs:[0028h]
	mov	[rsp+298h],rax
	xor	eax,eax
	call	406A30h
	cmp	[000000000061B114],0h                                  ; [rip+00213876]
	jnz	407970h

l00000000004078A4:
	cmp	[000000000061B144],0h                                  ; [rip+00213899]
	jz	4078E0h

l00000000004078AD:
	cmp	byte ptr [rbx+0B0h],0h
	mov	ecx,413764h
	jnz	4079C0h

l00000000004078BF:
	xor	edx,edx
	cmp	[000000000061B150],4h                                  ; [rip+00213888]
	mov	esi,413779h
	cmovnz	edx,[000000000061B174]                              ; [rip+002138A0]

l00000000004078D4:
	mov	edi,1h
	xor	eax,eax
	call	402730h

l00000000004078E0:
	cmp	[000000000061B17D],0h                                  ; [rip+00213896]
	jz	407911h

l00000000004078E9:
	xor	edx,edx
	cmp	[000000000061B150],4h                                  ; [rip+0021385E]
	mov	rcx,[rbx+0A8h]
	cmovnz	edx,[000000000061B16C]                              ; [rip+0021386C]

l0000000000407900:
	mov	esi,413779h
	mov	edi,1h
	xor	eax,eax
	call	402730h

l0000000000407911:
	mov	rcx,rbp
	xor	edx,edx
	xor	esi,esi
	mov	rdi,rbx
	call	406540h
	mov	rbp,rax
	mov	eax,[000000000061B12C]                                 ; [rip+00213803]
	test	eax,eax
	jz	407948h

l000000000040792D:
	movzx	edi,byte ptr [rbx+0B0h]
	mov	edx,[rbx+0A0h]
	mov	esi,[rbx+28h]
	call	405D00h
	movzx	eax,al
	add	rbp,rax

l0000000000407948:
	mov	rsi,[rsp+298h]
	xor	rsi,fs:[0028h]
	mov	rax,rbp
	jnz	4079E6h

l0000000000407962:
	add	rsp,+2A8h
	pop	rbx
	pop	rbp
	ret
000000000040796C                                     0F 1F 40 00             ..@.

l0000000000407970:
	cmp	byte ptr [rbx+0B0h],0h
	mov	ecx,413764h
	jz	407992h

l000000000040797E:
	mov	rdi,[rbx+18h]
	test	rdi,rdi
	jz	407992h

l0000000000407987:
	mov	rsi,rsp
	call	40CD70h
	mov	rcx,rax

l0000000000407992:
	xor	edx,edx
	cmp	[000000000061B150],4h                                  ; [rip+002137B5]
	mov	esi,413779h
	cmovnz	edx,[000000000061B178]                              ; [rip+002137D1]

l00000000004079A7:
	mov	edi,1h
	xor	eax,eax
	call	402730h
	jmp	4078A4h
00000000004079B8                         0F 1F 84 00 00 00 00 00         ........

l00000000004079C0:
	mov	rdi,[rbx+50h]
	mov	r8,[000000000061B138]                                  ; [rip+0021376D]
	mov	ecx,200h
	mov	edx,[000000000061B140]                                 ; [rip+0021376A]
	mov	rsi,rsp
	call	40BD70h
	mov	rcx,rax
	jmp	4078BFh

l00000000004079E6:
	call	4023A0h
00000000004079EB                                  0F 1F 44 00 00            ..D..

;; fn00000000004079F0: 00000000004079F0
;;   Called from:
;;     0000000000403E0E (in fn00000000004028C0)
;;     0000000000403E82 (in fn00000000004028C0)
;;     0000000000404220 (in fn00000000004028C0)
fn00000000004079F0 proc
	cmp	[000000000061B150],4h                                  ; [rip+00213759]
	ja	407A46h

l00000000004079F9:
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbp
	push	rbx
	sub	rsp,38h
	mov	eax,[000000000061B150]                                 ; [rip+00213743]
	jmp	qword ptr [412308h+rax*8]
0000000000407A14             0F 1F 40 00                             ..@.        

l0000000000407A18:
	mov	rdi,[000000000061A610]                                 ; [rip+00212BF1]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	407E39h

l0000000000407A2D:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],0Ah

l0000000000407A38:
	add	rsp,38h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15

l0000000000407A46:
	ret

l0000000000407A48:
	xor	edi,edi
	xor	r15d,r15d
	call	405ED0h
	lea	rdx,[rax+rax*2]
	mov	r13,rax
	mov	rax,[000000000061B028]                                 ; [rip+002135C8]
	lea	r14,[rax+rdx*8-18h]
	mov	rax,[000000000061B1A8]                                 ; [rip+0021373C]
	mov	rbx,[rax]
	mov	rdi,rbx
	call	405D50h
	mov	[rsp+8h],rax
	mov	rax,[r14+10h]
	xor	esi,esi
	mov	rdi,rbx
	mov	ebx,1h
	mov	r12,[rax]
	call	407870h
	cmp	[000000000061B1B0],1h                                  ; [rip+00213716]
	mov	rcx,[rsp+8h]
	ja	407B0Dh

l0000000000407AA1:
	jmp	407A18h
0000000000407AA6                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000407AB0:
	mov	rdi,[000000000061A610]                                 ; [rip+00212B59]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	407E27h

l0000000000407AC5:
	lea	rdx,[rax+1h]
	xor	r15d,r15d
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],0Ah

l0000000000407AD3:
	mov	rax,[000000000061B1A8]                                 ; [rip+002136CE]
	mov	rsi,r15
	mov	r12,[rax+rbx*8]
	add	rbx,1h
	mov	rdi,r12
	call	407870h
	mov	rdi,r12
	call	405D50h
	cmp	rbx,[000000000061B1B0]                                 ; [rip+002136B4]
	mov	rdx,[r14+10h]
	mov	rcx,rax
	mov	r12,[rdx+rbp*8]
	jnc	407A18h

l0000000000407B0D:
	xor	edx,edx
	mov	rax,rbx
	div	r13
	test	rdx,rdx
	mov	rbp,rdx
	jz	407AB0h

l0000000000407B1D:
	add	r12,r15
	lea	rdi,[rcx+r15]
	mov	rsi,r12
	mov	r15,r12
	call	405200h
	jmp	407AD3h

l0000000000407B31:
	cmp	[000000000061B1B0],0h                                  ; [rip+00213677]
	jz	407A18h

l0000000000407B3F:
	mov	rax,[000000000061B1A8]                                 ; [rip+00213662]
	xor	ebx,ebx
	xor	r12d,r12d
	mov	r13,[rax]
	mov	rdi,r13
	call	405D50h
	mov	rbp,rax
	nop	dword ptr [rax+0h]

l0000000000407B60:
	mov	rsi,r12
	mov	rdi,r13
	add	rbx,1h
	call	407870h
	cmp	rbx,[000000000061B1B0]                                 ; [rip+0021363A]
	mov	r12,rbp
	jnc	407A18h

l0000000000407B7F:
	mov	rax,[000000000061B1A8]                                 ; [rip+00213622]
	mov	r13,[rax+rbx*8]
	mov	rdi,r13
	call	405D50h
	test	rbx,rbx
	jz	407E00h

l0000000000407B9B:
	lea	r12,[rbp+2h]
	lea	rbp,[rax]
	cmp	rbp,[000000000061B0C8]                                 ; [rip+0021351E]
	jnc	407E10h

l0000000000407BB0:
	mov	r15d,20h
	mov	r14d,20h

l0000000000407BBC:
	mov	rdi,[000000000061A610]                                 ; [rip+00212A4D]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	407E8Bh

l0000000000407BD1:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],2Ch

l0000000000407BDC:
	mov	rdi,[000000000061A610]                                 ; [rip+00212A2D]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	407E7Eh

l0000000000407BF1:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	[rax],r14b
	jmp	407B60h

l0000000000407C01:
	xor	ebx,ebx
	cmp	[000000000061B1B0],0h                                  ; [rip+002135A5]
	jz	407A38h

l0000000000407C11:
	nop	dword ptr [rax+0h]

l0000000000407C18:
	call	406A30h
	mov	rax,[000000000061B1A8]                                 ; [rip+00213584]
	mov	rdi,[rax+rbx*8]
	call	406B70h
	mov	rdi,[000000000061A610]                                 ; [rip+002129DC]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	407E6Fh

l0000000000407C42:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],0Ah

l0000000000407C4D:
	add	[000000000061B018],1h                                  ; [rip+002133C3]
	add	rbx,1h
	cmp	[000000000061B1B0],rbx                                 ; [rip+00213550]
	ja	407C18h

l0000000000407C62:
	add	rsp,38h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	jmp	407A46h

l0000000000407C75:
	xor	ebx,ebx
	cmp	[000000000061B1B0],0h                                  ; [rip+00213531]
	jz	407A38h

l0000000000407C85:
	nop	dword ptr [rax]

l0000000000407C88:
	mov	rax,[000000000061B1A8]                                 ; [rip+00213519]
	xor	esi,esi
	mov	rdi,[rax+rbx*8]
	call	407870h
	mov	rdi,[000000000061A610]                                 ; [rip+0021296F]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	407E60h

l0000000000407CAF:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],0Ah

l0000000000407CBA:
	add	rbx,1h
	cmp	[000000000061B1B0],rbx                                 ; [rip+002134EB]
	ja	407C88h

l0000000000407CC7:
	add	rsp,38h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	jmp	407A46h

l0000000000407CDA:
	mov	edi,1h
	call	405ED0h
	lea	rdx,[rax+rax*2]
	mov	rcx,rax
	mov	rax,[000000000061B028]                                 ; [rip+00213336]
	lea	r15,[rax+rdx*8-18h]
	mov	rax,[000000000061B1B0]                                 ; [rip+002134B2]
	xor	edx,edx
	div	rcx
	test	rdx,rdx
	setnz	dl
	movzx	edx,dl
	add	rdx,rax
	mov	[rsp+18h],rdx
	jz	407A38h

l0000000000407D1A:
	lea	rax,[0000h+rdx*8]
	mov	qword ptr [rsp+28h],+0h
	mov	[rsp+20h],rax

l0000000000407D30:
	mov	rax,[rsp+28h]
	xor	ebp,ebp
	xor	ebx,ebx
	lea	r13,[0000h+rax*8]
	mov	r12,rax
	jmp	407D6Bh
0000000000407D46                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000407D50:
	mov	r14,[rsp+10h]
	mov	rdi,[rsp+8h]
	add	r14,rbx
	add	rdi,rbx
	mov	rsi,r14
	mov	rbx,r14
	call	405200h

l0000000000407D6B:
	mov	rax,[000000000061B1A8]                                 ; [rip+00213436]
	mov	r14,[rax+r13]
	mov	rdi,r14
	call	405D50h
	mov	[rsp+8h],rax
	mov	rax,[r15+10h]
	mov	rsi,rbx
	mov	rdi,r14
	mov	rcx,[rax+rbp]
	add	rbp,8h
	mov	[rsp+10h],rcx
	call	407870h
	add	r12,[rsp+18h]
	add	r13,[rsp+20h]
	cmp	r12,[000000000061B1B0]                                 ; [rip+00213400]
	jc	407D50h

l0000000000407DB2:
	mov	rdi,[000000000061A610]                                 ; [rip+00212857]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	407E51h

l0000000000407DC7:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],0Ah

l0000000000407DD2:
	add	qword ptr [rsp+28h],1h
	mov	rax,[rsp+18h]
	cmp	[rsp+28h],rax
	jnz	407D30h

l0000000000407DE8:
	add	rsp,38h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	jmp	407A46h
0000000000407DFB                                  0F 1F 44 00 00            ..D..

l0000000000407E00:
	add	rbp,rax
	jmp	407B60h
0000000000407E08                         0F 1F 84 00 00 00 00 00         ........

l0000000000407E10:
	mov	r15d,0Ah
	mov	r14d,0Ah
	xor	r12d,r12d
	mov	rbp,rax
	jmp	407BBCh

l0000000000407E27:
	mov	esi,0Ah
	xor	r15d,r15d
	call	402400h
	jmp	407AD3h

l0000000000407E39:
	add	rsp,38h
	mov	esi,0Ah
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	jmp	402400h

l0000000000407E51:
	mov	esi,0Ah
	call	402400h
	jmp	407DD2h

l0000000000407E60:
	mov	esi,0Ah
	call	402400h
	jmp	407CBAh

l0000000000407E6F:
	mov	esi,0Ah
	call	402400h
	jmp	407C4Dh

l0000000000407E7E:
	mov	esi,r15d
	call	402400h
	jmp	407B60h

l0000000000407E8B:
	mov	esi,2Ch
	call	402400h
	jmp	407BDCh
0000000000407E9A                               66 0F 1F 44 00 00           f..D..

;; fn0000000000407EA0: 0000000000407EA0
;;   Called from:
;;     0000000000403443 (in fn00000000004028C0)
;;     0000000000403E3F (in fn00000000004028C0)
;;     00000000004042C1 (in fn00000000004028C0)
fn0000000000407EA0 proc
	push	rbp
	mov	rbp,rsp
	push	r15
	push	r14
	push	r13
	mov	r13d,esi
	push	r12
	push	rbx
	mov	rbx,rdi
	sub	rsp,+378h
	mov	[rbp-384h],edx
	mov	rdx,rcx
	mov	rax,fs:[0028h]
	mov	[rbp-38h],rax
	xor	eax,eax
	mov	rcx,[000000000061B1B0]                                 ; [rip+002132D7]
	cmp	rcx,[000000000061B1B8]                                 ; [rip+002132D8]
	mov	r9,[000000000061B1C0]                                  ; [rip+002132D9]
	jz	408400h

l0000000000407EED:
	lea	rcx,[rcx+rcx*2]
	mov	esi,0C0h
	shl	rcx,6h
	lea	r14,[r9+rcx]
	test	r14b,1h
	mov	rdi,r14
	jnz	408A18h

l0000000000407F0B:
	test	dil,2h
	jnz	4089E0h

l0000000000407F15:
	test	dil,4h
	jnz	408A00h

l0000000000407F1F:
	mov	ecx,esi
	xor	eax,eax
	shr	ecx,3h
	test	sil,4h

l0000000000407F2A:
	rep stosq

l0000000000407F2D:
	jnz	408360h

l0000000000407F33:
	test	sil,2h
	jnz	408340h

l0000000000407F3D:
	and	esi,1h
	jnz	408338h

l0000000000407F46:
	cmp	byte ptr [rbp-384h],0h
	mov	qword ptr [r14+18h],+0h
	mov	[r14+0A0h],r13d
	jz	408220h

l0000000000407F62:
	mov	ecx,[000000000061B110]                                 ; [rip+002131A8]

l0000000000407F68:
	movzx	esi,byte ptr [rbx]
	mov	r12,rbx
	cmp	sil,2Fh
	jz	407F81h

l0000000000407F74:
	movzx	r15d,byte ptr [rdx]
	test	r15b,r15b
	jnz	408670h

l0000000000407F81:
	cmp	ecx,3h
	jc	407FFDh

l0000000000407F86:
	cmp	ecx,4h
	jbe	407FF0h

l0000000000407F8B:
	cmp	ecx,5h
	nop
	jnz	407FFDh

l0000000000407F92:
	lea	r15,[r14+10h]
	mov	rsi,r12
	mov	edi,1h
	mov	rdx,r15
	call	402610h
	mov	edx,eax
	mov	esi,1h
	test	edx,edx
	jz	408019h

l0000000000407FB1:
	xor	edi,edi
	mov	edx,5h
	mov	esi,413783h
	call	402360h
	mov	r15d,[rbp-384h]
	mov	rdx,r12
	mov	rsi,rax
	xor	r12d,r12d
	movzx	edi,r15b
	call	405810h
	test	r15b,r15b
	jnz	4081F4h

l0000000000407FE4:
	jmp	4081E1h
0000000000407FE9                            0F 1F 80 00 00 00 00          .......

l0000000000407FF0:
	cmp	byte ptr [rbp-384h],0h
	jnz	4088B0h

l0000000000407FFD:
	lea	r15,[r14+10h]

l0000000000408001:
	mov	rdx,r15
	mov	rsi,r12
	mov	edi,1h
	call	402390h
	xor	esi,esi
	mov	edx,eax

l0000000000408015:
	test	edx,edx
	jnz	407FB1h

l0000000000408019:
	cmp	r13d,5h
	mov	byte ptr [r14+0B0h],1h
	jz	408380h

l000000000040802B:
	mov	eax,[r14+28h]
	and	eax,0F000h
	cmp	eax,8000h
	jz	408380h

l000000000040803F:
	mov	ecx,[000000000061B150]                                 ; [rip+0021310B]
	test	ecx,ecx
	jz	408056h

l0000000000408049:
	cmp	[000000000061B17D],0h                                  ; [rip+0021312D]
	jz	4080EFh

l0000000000408056:
	mov	rax,[000000000061A670]                                 ; [rip+00212613]
	cmp	[r14+10h],rax
	mov	[rbp-390h],edx
	jz	408C52h

l000000000040806D:
	test	sil,sil
	mov	rdi,r12
	lea	rsi,[r14+0A8h]
	jz	408458h

l0000000000408080:
	call	411820h
	test	eax,eax
	mov	edx,[rbp-390h]
	js	40846Bh

l0000000000408093:
	mov	rdi,[r14+0A8h]
	mov	esi,4137B1h
	mov	ecx,0Ah

l00000000004080A4:
	rep cmpsb

l00000000004080A6:
	setnz	r13b

l00000000004080AA:
	mov	eax,[000000000061B150]                                 ; [rip+002130A0]
	test	eax,eax
	jz	408830h

l00000000004080B8:
	xor	ecx,ecx

l00000000004080BA:
	mov	esi,ecx
	xor	eax,eax
	or	sil,r13b
	jz	4080D9h

l00000000004080C3:
	xor	ecx,1h
	and	r13b,cl
	mov	eax,r13d
	cmovnz	esi,r13d

l00000000004080D0:
	shl	eax,1Fh
	sar	eax,1Fh
	add	eax,2h

l00000000004080D9:
	or	[000000000061B17C],sil                                  ; [rip+0021309C]
	test	edx,edx
	mov	[r14+0B4h],eax
	jnz	4084BFh

l00000000004080EF:
	mov	eax,[r14+28h]
	and	eax,0F000h
	cmp	eax,0A000h
	jz	408500h

l0000000000408103:
	cmp	eax,4000h
	jz	408978h

l000000000040810E:
	mov	r13d,[000000000061B150]                                ; [rip+0021303B]
	mov	dword ptr [r14+0A0h],5h

l0000000000408120:
	test	r13d,r13d
	mov	r12,[r14+50h]
	jz	408132h

l0000000000408129:
	cmp	[000000000061B144],0h                                  ; [rip+00213014]
	jz	4081A8h

l0000000000408132:
	mov	r8,[000000000061B138]                                  ; [rip+00212FFF]
	mov	edx,[000000000061B140]                                 ; [rip+00213001]
	lea	r15,[rbp-2D0h]
	mov	ecx,200h
	mov	rdi,r12
	mov	rsi,r15
	call	40BD70h
	xor	esi,esi
	mov	rdi,rax
	call	40D420h
	cmp	eax,[000000000061B174]                                 ; [rip+0021300E]
	jle	40816Eh

l0000000000408168:
	mov	[000000000061B174],eax                                 ; [rip+00213006]

l000000000040816E:
	mov	r13d,[000000000061B150]                                ; [rip+00212FDB]
	test	r13d,r13d
	jnz	4081A8h

l000000000040817A:
	cmp	[000000000061A569],0h                                  ; [rip+002123E8]
	jnz	408B10h

l0000000000408187:
	cmp	[000000000061A568],0h                                  ; [rip+002123DA]
	jnz	408A90h

l0000000000408194:
	cmp	[000000000061B146],0h                                  ; [rip+00212FAB]
	jnz	408A70h

l00000000004081A1:
	mov	r13d,[000000000061B150]                                ; [rip+00212FA8]

l00000000004081A8:
	cmp	[000000000061B17D],0h                                  ; [rip+00212FCE]
	jz	4081C9h

l00000000004081B1:
	mov	rdi,[r14+0A8h]
	call	402380h
	cmp	eax,[000000000061B16C]                                 ; [rip+00212FA9]
	jg	408730h

l00000000004081C9:
	test	r13d,r13d
	jz	40873Fh

l00000000004081D2:
	movzx	eax,[000000000061B114]                               ; [rip+00212F3B]
	test	al,al
	jnz	408640h

l00000000004081E1:
	mov	rdi,rbx
	call	410E30h
	add	[000000000061B1B0],1h                                  ; [rip+00212FBF]
	mov	[r14],rax

l00000000004081F4:
	mov	rbx,[rbp-38h]
	xor	rbx,fs:[0028h]
	mov	rax,r12
	jnz	408C76h

l000000000040820A:
	lea	rsp,[rbp-28h]
	pop	rbx
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	pop	rbp
	ret
0000000000408219                            0F 1F 80 00 00 00 00          .......

l0000000000408220:
	cmp	[000000000061B0C1],0h                                  ; [rip+00212E9A]
	jnz	407F62h

l000000000040822D:
	cmp	r13d,3h
	jz	408910h

l0000000000408237:
	movzx	eax,[000000000061B114]                               ; [rip+00212ED6]
	test	al,al
	jz	408818h

l0000000000408246:
	test	r13d,r13d
	setz	sil
	jz	408255h

l000000000040824F:
	cmp	r13d,6h
	jnz	408280h

l0000000000408255:
	mov	ecx,[000000000061B110]                                 ; [rip+00212EB5]
	cmp	ecx,5h
	jz	408BD0h

l0000000000408264:
	cmp	[000000000061B198],0h                                  ; [rip+00212F2D]
	jnz	407F68h

l0000000000408271:
	cmp	[000000000061B115],0h                                  ; [rip+00212E9D]
	jnz	407F68h

l000000000040827E:
	nop

l0000000000408280:
	test	al,al
	jnz	407F62h

l0000000000408288:
	cmp	[000000000061B0C0],0h                                  ; [rip+00212E31]
	jz	408825h

l0000000000408295:
	test	sil,sil
	jnz	407F62h

l000000000040829E:
	xor	r12d,r12d
	cmp	r13d,5h
	jnz	4081E1h

l00000000004082AB:
	cmp	[000000000061B12C],3h                                  ; [rip+00212E7A]
	jz	407F62h

l00000000004082B8:
	cmp	[000000000061B129],0h                                  ; [rip+00212E6A]
	jz	4081E1h

l00000000004082C5:
	mov	edi,0Eh
	mov	[rbp-390h],rdx
	call	404CD0h
	test	al,al
	mov	rdx,[rbp-390h]
	jnz	407F62h

l00000000004082E5:
	mov	edi,10h
	call	404CD0h
	test	al,al
	mov	rdx,[rbp-390h]
	jnz	407F62h

l00000000004082FE:
	mov	edi,11h
	call	404CD0h
	test	al,al
	mov	rdx,[rbp-390h]
	jnz	407F62h

l0000000000408317:
	mov	edi,15h
	call	404CD0h
	test	al,al
	mov	rdx,[rbp-390h]
	jnz	407F62h

l0000000000408330:
	jmp	4081E1h
0000000000408335                0F 1F 00                              ...        

l0000000000408338:
	mov	byte ptr [rdi],0h
	jmp	407F46h

l0000000000408340:
	xor	r8d,r8d
	add	rdi,2h
	mov	[rdi-2h],r8w
	and	esi,1h
	jz	407F46h

l0000000000408355:
	jmp	408338h
0000000000408357                      66 0F 1F 84 00 00 00 00 00        f........

l0000000000408360:
	mov	dword ptr [rdi],0h
	add	rdi,4h
	test	sil,2h
	jz	407F3Dh

l0000000000408374:
	jmp	408340h
0000000000408376                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000408380:
	cmp	[000000000061B129],0h                                  ; [rip+00212DA2]
	jz	40803Fh

l000000000040838D:
	mov	edi,15h
	mov	[rbp-388h],esi
	mov	[rbp-390h],edx
	call	404CD0h
	test	al,al
	mov	edx,[rbp-390h]
	mov	esi,[rbp-388h]
	jz	40803Fh

l00000000004083B7:
	mov	r13,[r14+10h]
	cmp	r13,[000000000061A678]                                 ; [rip+002122B6]
	jz	408C1Fh

l00000000004083C8:
	mov	[rbp-388h],esi
	mov	[rbp-390h],edx
	call	402230h
	mov	esi,[rbp-388h]
	mov	edx,[rbp-390h]
	mov	dword ptr [rax],5Fh
	mov	[000000000061A678],r13                                 ; [rip+00212286]

l00000000004083F2:
	mov	byte ptr [r14+0B8h],0h
	jmp	40803Fh
00000000004083FF                                              90                .

l0000000000408400:
	mov	rax,0AAAAAAAAAAAAAAh
	mov	rdi,[000000000061B1C0]                                 ; [rip+00212DAF]
	cmp	rcx,rax
	ja	408C7Bh

l000000000040841A:
	lea	rsi,[rcx+rcx*2]
	mov	[rbp-390h],rdx
	shl	rsi,7h
	call	410C90h
	shl	[000000000061B1B8],1h                                  ; [rip+00212D83]
	mov	[000000000061B1C0],rax                                 ; [rip+00212D84]
	mov	r9,rax
	mov	rcx,[000000000061B1B0]                                 ; [rip+00212D6A]
	mov	rdx,[rbp-390h]
	jmp	407EEDh
0000000000408452       66 0F 1F 44 00 00                           f..D..        

l0000000000408458:
	call	411840h
	test	eax,eax
	mov	edx,[rbp-390h]
	jns	408093h

l000000000040846B:
	mov	[rbp-390h],edx
	call	402230h
	mov	eax,[rax]
	mov	edx,[rbp-390h]
	cmp	eax,16h
	jz	40848Ch

l0000000000408483:
	cmp	eax,26h
	jnz	4089C0h

l000000000040848C:
	mov	rcx,[r14+10h]
	mov	[000000000061A670],rcx                                 ; [rip+002121D9]

l0000000000408497:
	cmp	eax,5Fh
	mov	qword ptr [r14+0A8h],+61A56Ah
	jz	4089D0h

l00000000004084AB:
	cmp	eax,3Dh
	jz	4089D0h

l00000000004084B4:
	mov	dword ptr [r14+0B4h],0h

l00000000004084BF:
	mov	rdi,r12
	call	40EAB0h
	mov	r13,rax
	call	402230h
	mov	esi,[rax]
	xor	edi,edi
	xor	eax,eax
	mov	rcx,r13
	mov	edx,415E54h
	call	402770h
	mov	eax,[r14+28h]
	and	eax,0F000h
	cmp	eax,0A000h
	jnz	408103h

l00000000004084F6:
	nop	word ptr cs:[rax+rax+0h]

l0000000000408500:
	mov	r13d,[000000000061B150]                                ; [rip+00212C49]
	test	r13d,r13d
	jz	408519h

l000000000040850C:
	cmp	[000000000061B115],0h                                  ; [rip+00212C02]
	jz	40862Ah

l0000000000408519:
	mov	rsi,[r14+40h]
	mov	rdi,r12
	call	409D20h
	test	rax,rax
	mov	r13,rax
	mov	[r14+8h],rax
	jz	408BDAh

l0000000000408535:
	cmp	byte ptr [r13+0h],2Fh
	jz	408C0Fh

l0000000000408540:
	mov	rdi,r12
	call	40A2B0h
	test	rax,rax
	mov	r15,rax
	mov	rdi,r13
	jz	408B98h

l0000000000408557:
	call	402380h
	lea	rdi,[r15+rax+2h]
	call	410C40h
	mov	rcx,rax
	xor	eax,eax
	cmp	byte ptr [r12+r15-1h],2Fh
	mov	rdi,rcx
	mov	rsi,r12
	mov	[rbp-390h],rcx
	setnz	al
	add	r15,rax
	mov	rdx,r15
	call	4026F0h
	mov	rsi,r13
	mov	rdi,rax
	call	402260h
	mov	rcx,[rbp-390h]
	mov	r13,rcx

l00000000004085A1:
	test	r13,r13
	jz	408607h

l00000000004085A6:
	cmp	[000000000061B12C],1h                                  ; [rip+00212B7F]
	jbe	408B80h

l00000000004085B3:
	lea	rdx,[rbp-380h]
	mov	rsi,r13
	mov	edi,1h
	call	402610h
	test	eax,eax
	jnz	408607h

l00000000004085CB:
	cmp	byte ptr [rbp-384h],0h
	mov	byte ptr [r14+0B1h],1h
	mov	eax,[rbp-368h]
	jz	408600h

l00000000004085E2:
	mov	edx,[000000000061B150]                                 ; [rip+00212B68]
	test	edx,edx
	jz	408600h

l00000000004085EC:
	mov	edx,eax
	and	edx,0F000h
	cmp	edx,4000h
	jz	408607h

l00000000004085FC:
	nop	dword ptr [rax+0h]

l0000000000408600:
	mov	[r14+0A4h],eax

l0000000000408607:
	mov	rdi,r13
	call	4021F0h
	mov	eax,[r14+28h]
	and	eax,0F000h
	cmp	eax,0A000h
	jnz	408103h

l0000000000408623:
	mov	r13d,[000000000061B150]                                ; [rip+00212B26]

l000000000040862A:
	mov	dword ptr [r14+0A0h],6h
	jmp	408120h
000000000040863A                               66 0F 1F 44 00 00           f..D..

l0000000000408640:
	mov	rdi,[r14+18h]
	lea	rsi,[rbp-2D0h]
	call	40CD70h
	mov	rdi,rax
	call	402380h
	cmp	eax,[000000000061B178]                                 ; [rip+00212B1A]
	jle	4081E1h

l0000000000408664:
	mov	[000000000061B178],eax                                 ; [rip+00212B0E]
	jmp	4081E1h
000000000040866F                                              90                .

l0000000000408670:
	mov	rdi,rbx
	mov	[rbp-394h],ecx
	mov	[rbp-388h],esi
	mov	[rbp-390h],rdx
	call	402380h
	mov	rdx,[rbp-390h]
	mov	r12,rax
	mov	rdi,rdx
	call	402380h
	lea	rax,[r12+rax+20h]
	mov	rdx,[rbp-390h]
	mov	esi,[rbp-388h]
	mov	ecx,[rbp-394h]
	and	rax,0F0h
	sub	rsp,rax
	lea	rax,[rsp+0Fh]
	and	rax,0F0h
	cmp	r15b,2Eh
	mov	r12,rax
	jz	408B30h

l00000000004086D2:
	mov	rsi,rdx
	nop	dword ptr [rax]

l00000000004086D8:
	add	rax,1h
	add	rsi,1h
	mov	[rax-1h],r15b
	movzx	r15d,byte ptr [rsi]
	test	r15b,r15b
	jnz	4086D8h

l00000000004086ED:
	cmp	rdx,rsi
	mov	rdi,rax
	jnc	408702h

l00000000004086F5:
	cmp	byte ptr [rsi-1h],2Fh
	jz	408702h

l00000000004086FB:
	add	rax,1h
	mov	byte ptr [rdi],2Fh

l0000000000408702:
	movzx	esi,byte ptr [rbx]

l0000000000408705:
	test	sil,sil
	jz	408724h

l000000000040870A:
	mov	rdx,rbx
	nop	dword ptr [rax]

l0000000000408710:
	add	rax,1h
	add	rdx,1h
	mov	[rax-1h],sil
	movzx	esi,byte ptr [rdx]
	test	sil,sil
	jnz	408710h

l0000000000408724:
	mov	byte ptr [rax],0h
	jmp	407F81h
000000000040872C                                     0F 1F 40 00             ..@.

l0000000000408730:
	test	r13d,r13d
	mov	[000000000061B16C],eax                                 ; [rip+00212A33]
	jnz	4081D2h

l000000000040873F:
	mov	rdi,[r14+20h]
	lea	rsi,[rbp-2F0h]
	call	40CD70h
	mov	rdi,rax
	call	402380h
	cmp	eax,[000000000061B170]                                 ; [rip+00212A13]
	jle	408765h

l000000000040875F:
	mov	[000000000061B170],eax                                 ; [rip+00212A0B]

l0000000000408765:
	mov	eax,[r14+28h]
	and	eax,0B000h
	cmp	eax,2000h
	jnz	408A30h

l0000000000408779:
	mov	rax,[r14+38h]
	lea	r15,[rbp-2D0h]
	mov	rsi,r15
	mov	rdi,rax
	shr	rax,8h
	shr	rdi,20h
	and	eax,0FFFh
	and	edi,0FFFFF000h
	or	edi,eax
	call	40CD70h
	mov	rdi,rax
	call	402380h
	cmp	eax,[000000000061B15C]                                 ; [rip+002129AA]
	jle	4087BAh

l00000000004087B4:
	mov	[000000000061B15C],eax                                 ; [rip+002129A2]

l00000000004087BA:
	mov	rax,[r14+38h]
	mov	rsi,r15
	mov	rdi,rax
	movzx	eax,al
	shr	rdi,0Ch
	and	dil,0h
	or	edi,eax
	call	40CD70h
	mov	rdi,rax
	call	402380h
	mov	edx,[000000000061B158]                                 ; [rip+00212974]
	cmp	eax,edx
	jle	4087F0h

l00000000004087E8:
	mov	[000000000061B158],eax                                 ; [rip+0021296A]
	mov	edx,eax

l00000000004087F0:
	mov	eax,[000000000061B15C]                                 ; [rip+00212966]
	lea	eax,[rdx+rax+2h]
	cmp	eax,[000000000061B154]                                 ; [rip+00212954]
	jle	4081D2h

l0000000000408806:
	mov	[000000000061B154],eax                                 ; [rip+00212948]
	jmp	4081D2h
0000000000408811    0F 1F 80 00 00 00 00                          .......        

l0000000000408818:
	cmp	[000000000061B0C0],0h                                  ; [rip+002128A1]
	jnz	408246h

l0000000000408825:
	xor	r12d,r12d
	jmp	4081E1h
000000000040882D                                        0F 1F 00              ...

l0000000000408830:
	mov	rax,[000000000061A668]                                 ; [rip+00211E31]
	cmp	[r14+10h],rax
	jz	408B40h

l0000000000408841:
	mov	[rbp-388h],edx
	call	402230h
	mov	rsi,r15
	mov	dword ptr [rax],0h
	mov	rdi,r12
	mov	[rbp-390h],rax
	call	409CC0h
	test	eax,eax
	mov	ecx,1h
	mov	r8,[rbp-390h]
	mov	edx,[rbp-388h]
	jg	4080BAh

l000000000040887E:
	mov	edx,[r8]
	cmp	edx,16h
	jz	408C3Bh

l000000000040888A:
	cmp	edx,26h
	jz	408C3Bh

l0000000000408893:
	cmp	edx,5Fh
	jz	408C3Bh

l000000000040889C:
	shr	eax,1Fh
	mov	edx,eax
	jmp	4080B8h
00000000004088A6                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l00000000004088B0:
	lea	r15,[r14+10h]
	mov	rsi,r12
	mov	edi,1h
	mov	[rbp-390h],ecx
	mov	rdx,r15
	call	402610h
	mov	ecx,[rbp-390h]
	mov	edx,eax
	mov	esi,1h
	cmp	ecx,3h
	jz	408015h

l00000000004088E0:
	test	eax,eax
	js	408B5Eh

l00000000004088E8:
	mov	eax,[r14+28h]
	and	eax,0F000h
	cmp	eax,4000h
	setnz	al

l00000000004088F9:
	test	al,al
	mov	esi,1h
	jz	408015h

l0000000000408906:
	jmp	408001h
000000000040890B                                  0F 1F 44 00 00            ..D..

l0000000000408910:
	cmp	[000000000061B129],0h                                  ; [rip+00212812]
	jz	408237h

l000000000040891D:
	mov	edi,13h
	mov	[rbp-390h],rdx
	call	404CD0h
	test	al,al
	mov	rdx,[rbp-390h]
	jnz	407F62h

l000000000040893D:
	mov	edi,12h
	call	404CD0h
	test	al,al
	mov	rdx,[rbp-390h]
	jnz	407F62h

l0000000000408956:
	mov	edi,14h
	call	404CD0h
	test	al,al
	mov	rdx,[rbp-390h]
	jnz	407F62h

l000000000040896F:
	jmp	408237h
0000000000408974             0F 1F 40 00                             ..@.        

l0000000000408978:
	cmp	byte ptr [rbp-384h],0h
	jz	4089A8h

l0000000000408981:
	cmp	[000000000061B10D],0h                                  ; [rip+00212785]
	jnz	4089A8h

l000000000040898A:
	mov	dword ptr [r14+0A0h],9h
	mov	r13d,[000000000061B150]                                ; [rip+002127B4]
	jmp	408120h
00000000004089A1    0F 1F 80 00 00 00 00                          .......        

l00000000004089A8:
	mov	dword ptr [r14+0A0h],3h
	mov	r13d,[000000000061B150]                                ; [rip+00212796]
	jmp	408120h
00000000004089BF                                              90                .

l00000000004089C0:
	cmp	eax,5Fh
	jnz	408497h

l00000000004089C9:
	jmp	40848Ch
00000000004089CE                                           66 90               f.

l00000000004089D0:
	xor	r13d,r13d
	jmp	4080AAh
00000000004089D8                         0F 1F 84 00 00 00 00 00         ........

l00000000004089E0:
	xor	r9d,r9d
	add	rdi,2h
	sub	esi,2h
	mov	[rdi-2h],r9w
	test	dil,4h
	jz	407F1Fh

l00000000004089F9:
	nop	dword ptr [rax+0h]

l0000000000408A00:
	mov	dword ptr [rdi],0h
	sub	esi,4h
	add	rdi,4h
	jmp	407F1Fh
0000000000408A12       66 0F 1F 44 00 00                           f..D..        

l0000000000408A18:
	mov	byte ptr [r14],0h
	lea	rdi,[r14+1h]
	mov	sil,0BFh
	jmp	407F0Bh
0000000000408A28                         0F 1F 84 00 00 00 00 00         ........

l0000000000408A30:
	mov	rdi,[r14+40h]
	mov	r8,[000000000061A560]                                  ; [rip+00211B25]
	lea	rsi,[rbp-2D0h]
	mov	edx,[000000000061B134]                                 ; [rip+002126EC]
	mov	ecx,1h
	call	40BD70h
	xor	esi,esi
	mov	rdi,rax
	call	40D420h
	cmp	eax,[000000000061B154]                                 ; [rip+002126F2]
	jle	4081D2h

l0000000000408A68:
	jmp	408806h
0000000000408A6D                                        0F 1F 00              ...

l0000000000408A70:
	mov	edi,[r14+2Ch]
	call	4061B0h
	cmp	eax,[000000000061B160]                                 ; [rip+002126E1]
	jle	4081A1h

l0000000000408A85:
	mov	[000000000061B160],eax                                 ; [rip+002126D5]
	jmp	4081A1h

l0000000000408A90:
	cmp	[000000000061B145],0h                                  ; [rip+002126AE]
	mov	r13d,[r14+30h]
	jz	408BA8h

l0000000000408AA1:
	mov	edx,15h
	mov	r8d,r13d
	mov	ecx,41375Ah
	mov	esi,1h
	mov	rdi,r15
	xor	eax,eax
	call	402890h
	mov	rdx,r15

l0000000000408AC0:
	mov	ecx,[rdx]
	add	rdx,4h
	lea	eax,[rcx-1010101h]
	not	ecx
	and	eax,ecx
	and	eax,80808080h
	jz	408AC0h

l0000000000408AD7:
	mov	ecx,eax
	shr	ecx,10h
	test	eax,8080h
	cmovz	eax,ecx

l0000000000408AE4:
	lea	rcx,[rdx+2h]
	cmovz	rdx,rcx

l0000000000408AEC:
	add	al,al
	sbb	rdx,3h
	sub	edx,r15d

l0000000000408AF5:
	cmp	[000000000061B164],edx                                 ; [rip+00212669]
	jge	408194h

l0000000000408B01:
	mov	[000000000061B164],edx                                 ; [rip+0021265D]
	jmp	408194h
0000000000408B0C                                     0F 1F 40 00             ..@.

l0000000000408B10:
	mov	edi,[r14+2Ch]
	call	4061B0h
	cmp	eax,[000000000061B168]                                 ; [rip+00212649]
	jle	408187h

l0000000000408B25:
	mov	[000000000061B168],eax                                 ; [rip+0021263D]
	jmp	408187h

l0000000000408B30:
	cmp	byte ptr [rdx+1h],0h
	jz	408705h

l0000000000408B3A:
	jmp	4086D2h
0000000000408B3F                                              90                .

l0000000000408B40:
	mov	[rbp-390h],edx
	call	402230h
	xor	ecx,ecx
	mov	dword ptr [rax],5Fh
	mov	edx,[rbp-390h]
	jmp	4080BAh

l0000000000408B5E:
	mov	[rbp-390h],eax
	call	402230h
	cmp	dword ptr [rax],2h
	mov	edx,[rbp-390h]
	setz	al
	jmp	4088F9h
0000000000408B7A                               66 0F 1F 44 00 00           f..D..

l0000000000408B80:
	cmp	[000000000061B115],0h                                  ; [rip+0021258E]
	jz	408607h

l0000000000408B8D:
	jmp	4085B3h
0000000000408B92       66 0F 1F 44 00 00                           f..D..        

l0000000000408B98:
	call	410E30h
	mov	r13,rax
	jmp	4085A1h
0000000000408BA5                0F 1F 00                              ...        

l0000000000408BA8:
	mov	edi,r13d
	call	40CB40h
	test	rax,rax
	mov	rdi,rax
	jz	408AA1h

l0000000000408BBC:
	xor	esi,esi
	call	40D420h
	xor	edx,edx
	test	eax,eax
	cmovns	edx,eax

l0000000000408BCA:
	jmp	408AF5h
0000000000408BCF                                              90                .

l0000000000408BD0:
	mov	ecx,5h
	jmp	407F68h

l0000000000408BDA:
	xor	edi,edi
	mov	edx,5h
	mov	esi,413794h
	call	402360h
	movzx	edi,byte ptr [rbp-384h]
	mov	rdx,r12
	mov	rsi,rax
	call	405810h
	mov	r13,[r14+8h]
	test	r13,r13
	jnz	408535h

l0000000000408C0A:
	jmp	408607h

l0000000000408C0F:
	mov	rdi,r13
	call	410E30h
	mov	r13,rax
	jmp	4085A1h

l0000000000408C1F:
	call	402230h
	mov	edx,[rbp-390h]
	mov	dword ptr [rax],5Fh
	mov	esi,[rbp-388h]
	jmp	4083F2h

l0000000000408C3B:
	mov	rdx,[r14+10h]
	shr	eax,1Fh
	xor	ecx,ecx
	mov	[000000000061A668],rdx                                 ; [rip+00211A1D]
	mov	edx,eax
	jmp	4080BAh

l0000000000408C52:
	call	402230h
	xor	r13d,r13d
	mov	dword ptr [rax],5Fh
	mov	edx,[rbp-390h]
	mov	qword ptr [r14+0A8h],+61A56Ah
	jmp	4080AAh

l0000000000408C76:
	call	4023A0h

l0000000000408C7B:
	call	410E50h
	mov	eax,[rdi+0A0h]
	mov	r8d,[rsi+0A0h]
	cmp	eax,9h
	setz	cl
	cmp	eax,3h
	setz	al
	cmp	r8d,9h
	setz	dl
	cmp	r8d,3h
	setz	r8b
	or	edx,r8d
	or	al,cl
	jnz	408CE8h

l0000000000408CAF:
	test	al,al
	jnz	408CC0h

l0000000000408CB3:
	test	dl,dl
	mov	eax,1h
	jz	408CC0h

l0000000000408CBC:
	ret
0000000000408CBE                                           66 90               f.

l0000000000408CC0:
	mov	rcx,[rsi+68h]
	cmp	[rdi+68h],rcx
	mov	rax,[rdi+70h]
	mov	rdx,[rsi+70h]
	jg	408CF0h

l0000000000408CD2:
	jl	408D00h

l0000000000408CD4:
	sub	edx,eax
	jnz	408D06h

l0000000000408CD8:
	mov	rsi,[rsi]
	mov	rdi,[rdi]
	jmp	405020h
0000000000408CE3          0F 1F 44 00 00                            ..D..        

l0000000000408CE8:
	test	dl,dl
	jnz	408CAFh

l0000000000408CEC:
	nop	dword ptr [rax+0h]

l0000000000408CF0:
	mov	eax,0FFFFFFFFh
	ret
0000000000408CF6                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000408D00:
	mov	eax,1h
	ret

l0000000000408D06:
	mov	eax,edx
	ret
0000000000408D09                            0F 1F 80 00 00 00 00          .......
0000000000408D10 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 49 89 F1 ......D......I..
0000000000408D20 83 F8 09 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 ............A...
0000000000408D30 0F 94 C2 41 83 F8 03 40 0F 94 C6 09 F2 08 C8 75 ...A...@.......u
0000000000408D40 3F 84 C0 75 13 84 D2 B8 01 00 00 00 74 0A 66 90 ?..u........t.f.
0000000000408D50 F3 C3 66 0F 1F 44 00 00 48 8B 4F 68 49 39 49 68 ..f..D..H.OhI9Ih
0000000000408D60 49 8B 41 70 48 8B 57 70 7F 1E 7C 24 29 C2 75 26 I.ApH.Wp..|$).u&
0000000000408D70 48 8B 37 49 8B 39 E9 A5 C2 FF FF 0F 1F 44 00 00 H.7I.9.......D..
0000000000408D80 84 D2 75 BD 0F 1F 40 00 B8 FF FF FF FF C3 66 90 ..u...@.......f.
0000000000408D90 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
0000000000408DA0 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
0000000000408DB0 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 0F 94 C2 .........A......
0000000000408DC0 41 83 F8 03 41 0F 94 C0 44 09 C2 08 C8 75 41 84 A...A...D....uA.
0000000000408DD0 C0 75 0D 84 D2 B8 01 00 00 00 74 04 F3 C3 66 90 .u........t...f.
0000000000408DE0 48 8B 4E 78 48 39 4F 78 48 8B 87 80 00 00 00 48 H.NxH9OxH......H
0000000000408DF0 8B 96 80 00 00 00 7F 20 7C 26 29 C2 75 28 48 8B ....... |&).u(H.
0000000000408E00 36 48 8B 3F E9 17 C2 FF FF 0F 1F 80 00 00 00 00 6H.?............
0000000000408E10 84 D2 75 BB 0F 1F 40 00 B8 FF FF FF FF C3 66 90 ..u...@.......f.
0000000000408E20 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
0000000000408E30 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 49 89 F1 ......D......I..
0000000000408E40 83 F8 09 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 ............A...
0000000000408E50 0F 94 C2 41 83 F8 03 40 0F 94 C6 09 F2 08 C8 75 ...A...@.......u
0000000000408E60 47 84 C0 75 13 84 D2 B8 01 00 00 00 74 0A 66 90 G..u........t.f.
0000000000408E70 F3 C3 66 0F 1F 44 00 00 48 8B 4F 78 49 39 49 78 ..f..D..H.OxI9Ix
0000000000408E80 49 8B 81 80 00 00 00 48 8B 97 80 00 00 00 7F 20 I......H....... 
0000000000408E90 7C 2E 29 C2 75 30 48 8B 37 49 8B 39 E9 7F C1 FF |.).u0H.7I.9....
0000000000408EA0 FF 0F 1F 80 00 00 00 00 84 D2 75 B5 0F 1F 40 00 ..........u...@.
0000000000408EB0 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000408EC0 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
0000000000408ED0 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
0000000000408EE0 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 0F 94 C2 .........A......
0000000000408EF0 41 83 F8 03 41 0F 94 C0 44 09 C2 08 C8 75 39 84 A...A...D....u9.
0000000000408F00 C0 75 0D 84 D2 B8 01 00 00 00 74 04 F3 C3 66 90 .u........t...f.
0000000000408F10 48 8B 4E 58 48 39 4F 58 48 8B 47 60 48 8B 56 60 H.NXH9OXH.G`H.V`
0000000000408F20 7F 1E 7C 2C 29 C2 75 2E 48 8B 36 48 8B 3F E9 ED ..|,).u.H.6H.?..
0000000000408F30 C0 FF FF 0F 1F 44 00 00 84 D2 75 C3 0F 1F 40 00 .....D....u...@.
0000000000408F40 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000408F50 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
0000000000408F60 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 49 89 F1 ......D......I..
0000000000408F70 83 F8 09 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 ............A...
0000000000408F80 0F 94 C2 41 83 F8 03 40 0F 94 C6 09 F2 08 C8 75 ...A...@.......u
0000000000408F90 3F 84 C0 75 13 84 D2 B8 01 00 00 00 74 0A 66 90 ?..u........t.f.
0000000000408FA0 F3 C3 66 0F 1F 44 00 00 48 8B 4F 58 49 39 49 58 ..f..D..H.OXI9IX
0000000000408FB0 49 8B 41 60 48 8B 57 60 7F 1E 7C 24 29 C2 75 26 I.A`H.W`..|$).u&
0000000000408FC0 48 8B 37 49 8B 39 E9 55 C0 FF FF 0F 1F 44 00 00 H.7I.9.U.....D..
0000000000408FD0 84 D2 75 BD 0F 1F 40 00 B8 FF FF FF FF C3 66 90 ..u...@.......f.
0000000000408FE0 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
0000000000408FF0 48 8B 4E 68 48 39 4F 68 48 8B 57 70 48 8B 46 70 H.NhH9OhH.WpH.Fp
0000000000409000 7F 16 7C 1C 29 D0 75 1D 48 8B 36 48 8B 3F E9 3D ..|.).u.H.6H.?.=
0000000000409010 95 FF FF 0F 1F 44 00 00 B8 FF FF FF FF C3 66 90 .....D........f.
0000000000409020 B8 01 00 00 00 C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000409030 48 89 F0 48 8B 96 80 00 00 00 48 8B 77 78 48 39 H..H......H.wxH9
0000000000409040 70 78 48 8B 8F 80 00 00 00 7F 15 7C 23 29 D1 75 pxH........|#).u
0000000000409050 25 48 8B 37 48 8B 38 E9 F4 94 FF FF 0F 1F 40 00 %H.7H.8.......@.
0000000000409060 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000409070 B8 01 00 00 00 C3 89 C8 C3 0F 1F 80 00 00 00 00 ................
0000000000409080 48 8B 4E 78 48 39 4F 78 48 8B 97 80 00 00 00 48 H.NxH9OxH......H
0000000000409090 8B 86 80 00 00 00 7F 18 7C 26 29 D0 75 27 48 8B ........|&).u'H.
00000000004090A0 36 48 8B 3F E9 A7 94 FF FF 0F 1F 80 00 00 00 00 6H.?............
00000000004090B0 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
00000000004090C0 B8 01 00 00 00 C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
00000000004090D0 48 89 F0 48 8B 56 70 48 8B 77 68 48 39 70 68 48 H..H.VpH.whH9phH
00000000004090E0 8B 4F 70 7F 1B 7C 29 29 D1 75 2B 48 8B 37 48 8B .Op..|)).u+H.7H.
00000000004090F0 38 E9 5A 94 FF FF 66 2E 0F 1F 84 00 00 00 00 00 8.Z...f.........
0000000000409100 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000409110 B8 01 00 00 00 C3 89 C8 C3 0F 1F 80 00 00 00 00 ................
0000000000409120 48 8B 4E 58 48 39 4F 58 48 8B 57 60 48 8B 46 60 H.NXH9OXH.W`H.F`
0000000000409130 7F 16 7C 1C 29 D0 75 1D 48 8B 36 48 8B 3F E9 0D ..|.).u.H.6H.?..
0000000000409140 94 FF FF 0F 1F 44 00 00 B8 FF FF FF FF C3 66 90 .....D........f.
0000000000409150 B8 01 00 00 00 C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000409160 48 89 F0 48 8B 56 60 48 8B 77 58 48 39 70 58 48 H..H.V`H.wXH9pXH
0000000000409170 8B 4F 60 7F 1B 7C 29 29 D1 75 2B 48 8B 37 48 8B .O`..|)).u+H.7H.
0000000000409180 38 E9 CA 93 FF FF 66 2E 0F 1F 84 00 00 00 00 00 8.....f.........
0000000000409190 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
00000000004091A0 B8 01 00 00 00 C3 89 C8 C3 0F 1F 80 00 00 00 00 ................
00000000004091B0 41 54 49 89 F4 BE 2E 00 00 00 55 48 89 FD 49 8B ATI.......UH..I.
00000000004091C0 3C 24 53 E8 48 92 FF FF 48 8B 7D 00 BE 2E 00 00 <$S.H...H.}.....
00000000004091D0 00 48 89 C3 E8 37 92 FF FF BA 19 69 41 00 48 85 .H...7.....iA.H.
00000000004091E0 C0 48 0F 44 C2 48 85 DB 48 0F 45 D3 48 89 C6 48 .H.D.H..H.E.H..H
00000000004091F0 89 D7 E8 29 BE FF FF 85 C0 74 05 5B 5D 41 5C C3 ...).....t.[]A\.
0000000000409200 5B 48 8B 75 00 49 8B 3C 24 5D 41 5C E9 0F BE FF [H.u.I.<$]A\....
0000000000409210 FF 66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 .ffffff.........
0000000000409220 41 54 49 89 FC 55 48 89 F5 BE 2E 00 00 00 53 48 ATI..UH.......SH
0000000000409230 8B 3F E8 D9 91 FF FF 48 8B 7D 00 BE 2E 00 00 00 .?.....H.}......
0000000000409240 48 89 C3 E8 C8 91 FF FF BA 19 69 41 00 48 85 C0 H.........iA.H..
0000000000409250 48 0F 44 C2 48 85 DB 48 0F 45 D3 48 89 C6 48 89 H.D.H..H.E.H..H.
0000000000409260 D7 E8 BA BD FF FF 85 C0 74 06 5B 5D 41 5C C3 90 ........t.[]A\..
0000000000409270 5B 48 8B 75 00 49 8B 3C 24 5D 41 5C E9 9F BD FF [H.u.I.<$]A\....
0000000000409280 FF 66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 .ffffff.........
0000000000409290 41 54 55 48 89 F5 53 8B 87 A0 00 00 00 48 89 FB ATUH..S......H..
00000000004092A0 44 8B 86 A0 00 00 00 83 F8 09 0F 94 C1 83 F8 03 D...............
00000000004092B0 0F 94 C0 41 83 F8 09 0F 94 C2 41 83 F8 03 40 0F ...A......A...@.
00000000004092C0 94 C6 09 F2 08 C8 75 68 84 C0 75 14 84 D2 B8 01 ......uh..u.....
00000000004092D0 00 00 00 74 0B 5B 5D 41 5C C3 66 0F 1F 44 00 00 ...t.[]A\.f..D..
00000000004092E0 48 8B 3B BE 2E 00 00 00 E8 23 91 FF FF 48 8B 7D H.;......#...H.}
00000000004092F0 00 BE 2E 00 00 00 49 89 C4 E8 12 91 FF FF BF 19 ......I.........
0000000000409300 69 41 00 48 85 C0 48 0F 44 C7 4D 85 E4 49 0F 45 iA.H..H.D.M..I.E
0000000000409310 FC 48 89 C6 E8 07 BD FF FF 85 C0 75 B8 48 8B 3B .H.........u.H.;
0000000000409320 48 8B 75 00 5B 5D 41 5C E9 F3 BC FF FF 0F 1F 00 H.u.[]A\........
0000000000409330 84 D2 75 94 B8 FF FF FF FF EB 9A 0F 1F 44 00 00 ..u..........D..
0000000000409340 41 54 55 48 89 FD 53 8B 87 A0 00 00 00 48 89 F3 ATUH..S......H..
0000000000409350 44 8B 86 A0 00 00 00 83 F8 09 0F 94 C1 83 F8 03 D...............
0000000000409360 0F 94 C0 41 83 F8 09 0F 94 C2 41 83 F8 03 40 0F ...A......A...@.
0000000000409370 94 C6 09 F2 08 C8 75 68 84 C0 75 14 84 D2 B8 01 ......uh..u.....
0000000000409380 00 00 00 74 0B 5B 5D 41 5C C3 66 0F 1F 44 00 00 ...t.[]A\.f..D..
0000000000409390 48 8B 3B BE 2E 00 00 00 E8 73 90 FF FF 48 8B 7D H.;......s...H.}
00000000004093A0 00 BE 2E 00 00 00 49 89 C4 E8 62 90 FF FF BF 19 ......I...b.....
00000000004093B0 69 41 00 48 85 C0 48 0F 44 C7 4D 85 E4 49 0F 45 iA.H..H.D.M..I.E
00000000004093C0 FC 48 89 C6 E8 57 BC FF FF 85 C0 75 B8 48 8B 3B .H...W.....u.H.;
00000000004093D0 48 8B 75 00 5B 5D 41 5C E9 43 BC FF FF 0F 1F 00 H.u.[]A\.C......
00000000004093E0 84 D2 75 94 B8 FF FF FF FF EB 9A 0F 1F 44 00 00 ..u..........D..
00000000004093F0 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 49 89 F1 ......D......I..
0000000000409400 83 F8 09 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 ............A...
0000000000409410 0F 94 C2 41 83 F8 03 40 0F 94 C6 09 F2 08 C8 75 ...A...@.......u
0000000000409420 47 84 C0 75 13 84 D2 B8 01 00 00 00 74 0A 66 90 G..u........t.f.
0000000000409430 F3 C3 66 0F 1F 44 00 00 48 8B 4F 78 49 39 49 78 ..f..D..H.OxI9Ix
0000000000409440 49 8B 81 80 00 00 00 48 8B 97 80 00 00 00 7F 20 I......H....... 
0000000000409450 7C 2E 29 C2 75 30 48 8B 37 49 8B 39 E9 EF 90 FF |.).u0H.7I.9....
0000000000409460 FF 0F 1F 80 00 00 00 00 84 D2 75 B5 0F 1F 40 00 ..........u...@.
0000000000409470 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000409480 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
0000000000409490 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
00000000004094A0 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 0F 94 C2 .........A......
00000000004094B0 41 83 F8 03 41 0F 94 C0 44 09 C2 08 C8 75 39 84 A...A...D....u9.
00000000004094C0 C0 75 0D 84 D2 B8 01 00 00 00 74 04 F3 C3 66 90 .u........t...f.
00000000004094D0 48 8B 4E 58 48 39 4F 58 48 8B 47 60 48 8B 56 60 H.NXH9OXH.G`H.V`
00000000004094E0 7F 1E 7C 2C 29 C2 75 2E 48 8B 36 48 8B 3F E9 5D ..|,).u.H.6H.?.]
00000000004094F0 90 FF FF 0F 1F 44 00 00 84 D2 75 C3 0F 1F 40 00 .....D....u...@.
0000000000409500 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
0000000000409510 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
0000000000409520 8B 87 A0 00 00 00 44 8B 86 A0 00 00 00 83 F8 09 ......D.........
0000000000409530 0F 94 C1 83 F8 03 0F 94 C0 41 83 F8 09 0F 94 C2 .........A......
0000000000409540 41 83 F8 03 41 0F 94 C0 44 09 C2 08 C8 75 39 84 A...A...D....u9.
0000000000409550 C0 75 0D 84 D2 B8 01 00 00 00 74 04 F3 C3 66 90 .u........t...f.
0000000000409560 48 8B 4E 68 48 39 4F 68 48 8B 47 70 48 8B 56 70 H.NhH9OhH.GpH.Vp
0000000000409570 7F 1E 7C 2C 29 C2 75 2E 48 8B 36 48 8B 3F E9 CD ..|,).u.H.6H.?..
0000000000409580 8F FF FF 0F 1F 44 00 00 84 D2 75 C3 0F 1F 40 00 .....D....u...@.
0000000000409590 B8 FF FF FF FF C3 66 2E 0F 1F 84 00 00 00 00 00 ......f.........
00000000004095A0 B8 01 00 00 00 C3 89 D0 C3 0F 1F 80 00 00 00 00 ................
00000000004095B0 41 54 49 89 FC 55 53 48 8B 2E BE 2E 00 00 00 48 ATI..USH.......H
00000000004095C0 89 EF E8 49 8E FF FF 4D 8B 24 24 BE 2E 00 00 00 ...I...M.$$.....
00000000004095D0 48 89 C3 4C 89 E7 E8 35 8E FF FF BA 19 69 41 00 H..L...5.....iA.
00000000004095E0 48 85 C0 48 0F 44 C2 48 85 DB 48 0F 45 D3 48 89 H..H.D.H..H.E.H.
00000000004095F0 C6 48 89 D7 E8 57 8F FF FF 85 C0 74 0B 5B 5D 41 .H...W.....t.[]A
0000000000409600 5C C3 66 0F 1F 44 00 00 5B 48 89 EF 4C 89 E6 5D \.f..D..[H..L..]
0000000000409610 41 5C E9 39 8F FF FF 66 0F 1F 84 00 00 00 00 00 A\.9...f........
0000000000409620 41 54 49 89 F4 BE 2E 00 00 00 55 53 48 8B 2F 48 ATI.......USH./H
0000000000409630 89 EF E8 D9 8D FF FF 4D 8B 24 24 BE 2E 00 00 00 .......M.$$.....
0000000000409640 48 89 C3 4C 89 E7 E8 C5 8D FF FF BA 19 69 41 00 H..L.........iA.
0000000000409650 48 85 C0 48 0F 44 C2 48 85 DB 48 0F 45 D3 48 89 H..H.D.H..H.E.H.
0000000000409660 C6 48 89 D7 E8 E7 8E FF FF 85 C0 74 0B 5B 5D 41 .H.........t.[]A
0000000000409670 5C C3 66 0F 1F 44 00 00 5B 48 89 EF 4C 89 E6 5D \.f..D..[H..L..]
0000000000409680 41 5C E9 C9 8E FF FF 66 0F 1F 84 00 00 00 00 00 A\.....f........
0000000000409690 41 54 55 53 8B 87 A0 00 00 00 48 89 FB 44 8B 86 ATUS......H..D..
00000000004096A0 A0 00 00 00 83 F8 09 0F 94 C1 83 F8 03 0F 94 C0 ................
00000000004096B0 41 83 F8 09 0F 94 C2 41 83 F8 03 40 0F 94 C7 09 A......A...@....
00000000004096C0 FA 08 C8 75 73 84 C0 75 17 84 D2 B8 01 00 00 00 ...us..u........
00000000004096D0 74 0E 5B 5D 41 5C C3 66 0F 1F 84 00 00 00 00 00 t.[]A\.f........
00000000004096E0 4C 8B 26 BE 2E 00 00 00 4C 89 E7 E8 20 8D FF FF L.&.....L... ...
00000000004096F0 48 8B 1B BE 2E 00 00 00 48 89 C5 48 89 DF E8 0D H.......H..H....
0000000000409700 8D FF FF BA 19 69 41 00 48 85 C0 48 0F 44 C2 48 .....iA.H..H.D.H
0000000000409710 85 ED 48 0F 45 D5 48 89 C6 48 89 D7 E8 2F 8E FF ..H.E.H..H.../..
0000000000409720 FF 85 C0 75 AD 48 89 DE 4C 89 E7 5B 5D 41 5C E9 ...u.H..L..[]A\.
0000000000409730 1C 8E FF FF 0F 1F 40 00 84 D2 75 89 B8 FF FF FF ......@...u.....
0000000000409740 FF EB 8F 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 ...ffff.........

;; fn0000000000409750: 0000000000409750
;;   Called from:
;;     0000000000402DFA (in fn00000000004028C0)
;;     00000000004031FE (in fn00000000004028C0)
fn0000000000409750 proc
	push	rbp
	mov	edx,5h
	push	rbx
	mov	ebx,edi
	sub	rsp,8h
	test	edi,edi
	mov	rbp,[000000000061B200]                                 ; [rip+00211A9A]
	jz	409794h

l0000000000409768:
	mov	esi,413D60h
	xor	edi,edi
	call	402360h
	mov	rdi,[000000000061A650]                                 ; [rip+00210ED5]
	mov	rdx,rax
	mov	rcx,rbp
	mov	esi,1h
	xor	eax,eax
	call	402810h

l000000000040978D:
	mov	edi,ebx
	call	4027F0h

l0000000000409794:
	xor	edi,edi
	mov	esi,413D88h
	call	402360h
	mov	rdx,rbp
	mov	rsi,rax
	mov	edi,1h
	xor	eax,eax
	call	402730h
	mov	rbp,[000000000061A610]                                 ; [rip+00210E57]
	mov	edx,5h
	xor	edi,edi
	mov	esi,413DB0h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210E34]
	mov	edx,5h
	xor	edi,edi
	mov	esi,413E40h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210E11]
	mov	edx,5h
	xor	edi,edi
	mov	esi,413E90h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210DEE]
	mov	edx,5h
	xor	edi,edi
	mov	esi,413FA0h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210DCB]
	mov	edx,5h
	xor	edi,edi
	mov	esi,4141E8h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210DA8]
	mov	edx,5h
	xor	edi,edi
	mov	esi,414398h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210D85]
	mov	edx,5h
	xor	edi,edi
	mov	esi,414540h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210D62]
	mov	edx,5h
	xor	edi,edi
	mov	esi,414580h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210D3F]
	mov	edx,5h
	xor	edi,edi
	mov	esi,414670h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210D1C]
	mov	edx,5h
	xor	edi,edi
	mov	esi,414780h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210CF9]
	mov	edx,5h
	xor	edi,edi
	mov	esi,414928h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210CD6]
	mov	edx,5h
	xor	edi,edi
	mov	esi,414AC0h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210CB3]
	mov	edx,5h
	xor	edi,edi
	mov	esi,414C28h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210C90]
	mov	edx,5h
	xor	edi,edi
	mov	esi,414DA0h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210C6D]
	mov	edx,5h
	xor	edi,edi
	mov	esi,414F58h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210C4A]
	mov	edx,5h
	xor	edi,edi
	mov	esi,415020h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210C27]
	mov	edx,5h
	xor	edi,edi
	mov	esi,4151F0h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210C04]
	mov	edx,5h
	xor	edi,edi
	mov	esi,4153F0h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210BE1]
	mov	edx,5h
	xor	edi,edi
	mov	esi,415480h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210BBE]
	mov	edx,5h
	xor	edi,edi
	mov	esi,4155E8h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210B9B]
	mov	edx,5h
	xor	edi,edi
	mov	esi,415748h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210B78]
	mov	edx,5h
	xor	edi,edi
	mov	esi,415778h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210B55]
	mov	edx,5h
	xor	edi,edi
	mov	esi,4157B0h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210B32]
	mov	edx,5h
	xor	edi,edi
	mov	esi,415850h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rbp,[000000000061A610]                                 ; [rip+00210B0F]
	mov	edx,5h
	xor	edi,edi
	mov	esi,415970h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	mov	rdi,[000000000061B200]                                 ; [rip+002116DC]
	call	40A390h
	xor	edi,edi
	mov	rbp,rax
	mov	edx,5h
	mov	esi,4137BBh
	call	402360h
	mov	ecx,4137D2h
	mov	rdx,rbp
	mov	rsi,rax
	mov	edi,1h
	xor	eax,eax
	call	402730h
	xor	edi,edi
	mov	edx,5h
	mov	esi,4137E8h
	call	402360h
	mov	ecx,415A08h
	mov	rsi,rax
	mov	edx,4137FCh
	mov	edi,1h
	xor	eax,eax
	call	402730h
	mov	rbp,[000000000061A610]                                 ; [rip+00210A8B]
	mov	edx,5h
	xor	edi,edi
	mov	esi,415A30h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	xor	esi,esi
	mov	edi,5h
	call	402710h
	test	rax,rax
	jz	409BC8h

l0000000000409BB2:
	mov	edx,3h
	mov	esi,41380Ah
	mov	rdi,rax
	call	402240h
	test	eax,eax
	jnz	409BFFh

l0000000000409BC8:
	mov	rdi,[000000000061B200]                                 ; [rip+00211631]
	call	40A390h
	xor	edi,edi
	mov	rbp,rax
	mov	edx,5h
	mov	esi,415AB8h
	call	402360h
	mov	rdx,rbp
	mov	rsi,rax
	mov	edi,1h
	xor	eax,eax
	call	402730h
	jmp	40978Dh

l0000000000409BFF:
	mov	rdi,[000000000061B200]                                 ; [rip+002115FA]
	call	40A390h
	xor	edi,edi
	mov	rbp,rax
	mov	edx,5h
	mov	esi,415A70h
	call	402360h
	mov	rdx,rbp
	mov	rsi,rax
	mov	edi,1h
	xor	eax,eax
	call	402730h
	jmp	409BC8h
0000000000409C33          66 2E 0F 1F 84 00 00 00 00 00 0F 1F 00    f............
0000000000409C40 53 31 F6 48 89 FB 48 83 EC 10 48 8D 54 24 08 E8 S1.H..H...H.T$..
0000000000409C50 7C 8A FF FF 85 C0 7F 31 EB 47 66 0F 1F 44 00 00 |......1.Gf..D..
0000000000409C60 8B 44 24 04 83 F8 01 74 0A 83 F8 04 74 05 83 F8 .D$....t....t...
0000000000409C70 20 75 3D 48 8D 54 24 08 BE 01 00 00 00 48 89 DF  u=H.T$......H..
0000000000409C80 E8 4B 8A FF FF 85 C0 7E 18 48 8B 7C 24 08 48 8D .K.....~.H.|$.H.
0000000000409C90 74 24 04 E8 F8 87 FF FF 85 C0 79 C4 B8 FF FF FF t$........y.....
0000000000409CA0 FF 48 83 C4 10 5B C3 66 0F 1F 84 00 00 00 00 00 .H...[.f........
0000000000409CB0 48 83 C4 10 B8 01 00 00 00 5B C3 0F 1F 44 00 00 H........[...D..

;; fn0000000000409CC0: 0000000000409CC0
;;   Called from:
;;     000000000040885F (in fn0000000000407EA0)
fn0000000000409CC0 proc
	mov	eax,[rsi+18h]
	and	eax,0F000h
	cmp	eax,0A000h
	jz	409D10h

l0000000000409CCF:
	sub	rsp,8h
	call	4024D0h
	test	eax,eax
	js	409CE8h

l0000000000409CDC:
	add	rsp,8h
	ret
0000000000409CE1    0F 1F 80 00 00 00 00                          .......        

l0000000000409CE8:
	call	402230h
	mov	eax,[rax]
	cmp	eax,5Fh
	jz	409D18h

l0000000000409CF4:
	cmp	eax,26h
	jz	409D18h

l0000000000409CF9:
	cmp	eax,16h
	jz	409D18h

l0000000000409CFE:
	cmp	eax,10h
	setnz	al
	movzx	eax,al
	neg	eax
	jmp	409CDCh
0000000000409D0B                                  0F 1F 44 00 00            ..D..

l0000000000409D10:
	xor	eax,eax
	ret
0000000000409D13          0F 1F 44 00 00                            ..D..        

l0000000000409D18:
	xor	eax,eax
	add	rsp,8h
	ret
0000000000409D1F                                              90                .

;; fn0000000000409D20: 0000000000409D20
;;   Called from:
;;     0000000000408520 (in fn0000000000407EA0)
fn0000000000409D20 proc
	push	r15
	lea	rax,[rsi+1h]
	mov	r15,7FFFFFFFFFFFFFFEh
	push	r14
	mov	r14,3FFFFFFFFFFFFFFFh
	push	r13
	mov	r13,rdi
	push	r12
	push	rbp
	push	rbx
	mov	ebx,401h
	sub	rsp,18h
	cmp	rsi,+400h
	cmovbe	rbx,rax

l0000000000409D59:
	nop	dword ptr [rax+0h]

l0000000000409D60:
	mov	rdi,rbx
	call	402640h
	test	rax,rax
	mov	rbp,rax
	jz	409DABh

l0000000000409D70:
	mov	rdx,rbx
	mov	rsi,rax
	mov	rdi,r13
	call	4022E0h
	test	rax,rax
	mov	r12,rax
	js	409DD8h

l0000000000409D86:
	cmp	rbx,r12
	ja	409E08h

l0000000000409D8B:
	mov	rdi,rbp
	call	4021F0h
	cmp	rbx,r14
	ja	409DC0h

l0000000000409D98:
	add	rbx,rbx
	mov	rdi,rbx
	call	402640h
	test	rax,rax
	mov	rbp,rax
	jnz	409D70h

l0000000000409DAB:
	xor	eax,eax

l0000000000409DAD:
	add	rsp,18h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
0000000000409DBC                                     0F 1F 40 00             ..@.

l0000000000409DC0:
	cmp	rbx,r15
	ja	409E20h

l0000000000409DC5:
	mov	rbx,7FFFFFFFFFFFFFFFh
	jmp	409D60h
0000000000409DD1    0F 1F 80 00 00 00 00                          .......        

l0000000000409DD8:
	call	402230h
	mov	edx,[rax]
	cmp	edx,22h
	jz	409D86h

l0000000000409DE4:
	mov	rdi,rbp
	mov	[rsp+0Ch],edx
	mov	[rsp],rax
	call	4021F0h
	mov	rax,[rsp]
	mov	edx,[rsp+0Ch]
	mov	[rax],edx
	xor	eax,eax
	jmp	409DADh
0000000000409E02       66 0F 1F 44 00 00                           f..D..        

l0000000000409E08:
	mov	byte ptr [rbp+0h],0h
	add	rsp,18h
	mov	rax,rbp
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret

l0000000000409E20:
	call	402230h
	mov	dword ptr [rax],0Ch
	add	rsp,18h
	xor	eax,eax
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
0000000000409E3C                                     0F 1F 40 00             ..@.
0000000000409E40 BF 01 00 00 00 E9 06 F9 FF FF 66 0F 1F 44 00 00 ..........f..D..

;; fn0000000000409E50: 0000000000409E50
;;   Called from:
;;     0000000000402A62 (in fn00000000004028C0)
;;     0000000000403868 (in fn00000000004028C0)
;;     000000000040A14C (in fn000000000040A120)
;;     000000000040C845 (in fn000000000040C810)
fn0000000000409E50 proc
	push	r15
	mov	r15,rsi
	push	r14
	push	r13
	push	r12
	mov	r12,rcx
	push	rbp
	mov	rbp,rdx
	push	rbx
	sub	rsp,28h
	mov	[rsp],rdi
	mov	[rsp+18h],rdx
	call	402380h
	mov	r14,[r15]
	test	r14,r14
	jz	409F72h

l0000000000409E81:
	mov	r13,rax
	mov	byte ptr [rsp+17h],0h
	mov	qword ptr [rsp+8h],-1h
	xor	ebx,ebx
	jmp	409EE8h
0000000000409E96                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000409EA0:
	mov	rax,[rsp+18h]
	test	rax,rax
	jz	409F50h

l0000000000409EAE:
	mov	rdi,[rsp+8h]
	mov	rdx,r12
	mov	rsi,rbp
	imul	rdi,r12
	add	rdi,rax
	call	402500h
	movzx	ecx,byte ptr [rsp+17h]
	test	eax,eax
	mov	eax,1h
	cmovnz	ecx,eax

l0000000000409ED4:
	mov	[rsp+17h],cl

l0000000000409ED8:
	add	rbx,1h
	add	rbp,r12
	mov	r14,[r15+rbx*8]
	test	r14,r14
	jz	409F28h

l0000000000409EE8:
	mov	rsi,[rsp]
	mov	rdx,r13
	mov	rdi,r14
	call	402240h
	test	eax,eax
	jnz	409ED8h

l0000000000409EFB:
	mov	rdi,r14
	call	402380h
	cmp	rax,r13
	jz	409F60h

l0000000000409F08:
	cmp	qword ptr [rsp+8h],0FFh
	jnz	409EA0h

l0000000000409F10:
	mov	[rsp+8h],rbx
	add	rbx,1h
	add	rbp,r12
	mov	r14,[r15+rbx*8]
	test	r14,r14
	jnz	409EE8h

l0000000000409F25:
	nop	dword ptr [rax]

l0000000000409F28:
	cmp	byte ptr [rsp+17h],0h
	mov	rax,-2h
	jnz	409F3Bh

l0000000000409F36:
	mov	rax,[rsp+8h]

l0000000000409F3B:
	add	rsp,28h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
0000000000409F4A                               66 0F 1F 44 00 00           f..D..

l0000000000409F50:
	mov	byte ptr [rsp+17h],1h
	jmp	409ED8h
0000000000409F57                      66 0F 1F 84 00 00 00 00 00        f........

l0000000000409F60:
	add	rsp,28h
	mov	rax,rbx
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret

l0000000000409F72:
	mov	qword ptr [rsp+8h],-1h
	jmp	409F36h
0000000000409F7D                                        0F 1F 00              ...

;; fn0000000000409F80: 0000000000409F80
;;   Called from:
;;     000000000040456F (in fn00000000004028C0)
;;     000000000040A16E (in fn000000000040A120)
fn0000000000409F80 proc
	push	r12
	cmp	rdx,0FFh
	mov	edx,5h
	push	rbp
	mov	rbp,rdi
	push	rbx
	mov	rbx,rsi
	jz	409FE0h

l0000000000409F95:
	mov	esi,415E18h
	xor	edi,edi
	call	402360h
	mov	r12,rax

l0000000000409FA4:
	mov	rsi,rbp
	mov	edi,1h
	call	40EBF0h
	mov	rdx,rbx
	mov	esi,6h
	xor	edi,edi
	mov	rbp,rax
	call	40E970h
	pop	rbx
	mov	r8,rbp
	mov	rdx,r12
	mov	rcx,rax
	pop	rbp
	pop	r12
	xor	esi,esi
	xor	edi,edi
	xor	eax,eax
	jmp	402770h
0000000000409FDB                                  0F 1F 44 00 00            ..D..

l0000000000409FE0:
	mov	esi,415DFDh
	xor	edi,edi
	call	402360h
	mov	r12,rax
	jmp	409FA4h
0000000000409FF1    66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00  ffffff.........

;; fn000000000040A000: 000000000040A000
;;   Called from:
;;     000000000040A17C (in fn000000000040A120)
fn000000000040A000 proc
	push	r15
	mov	r15,rdi
	xor	edi,edi
	push	r14
	xor	r14d,r14d
	push	r13
	mov	r13,rdx
	mov	edx,5h
	push	r12
	push	rbp
	mov	rbp,rsi
	mov	esi,415E35h
	push	rbx
	sub	rsp,8h
	mov	rbx,[000000000061A650]                                 ; [rip+00210623]
	call	402360h
	mov	rdi,rax
	mov	rsi,rbx
	xor	ebx,ebx
	call	402520h
	mov	r12,[r15]
	test	r12,r12
	jnz	40A086h

l000000000040A047:
	jmp	40A0D0h
000000000040A04C                                     0F 1F 40 00             ..@.

l000000000040A050:
	mov	rdi,r12
	add	rbx,1h
	mov	r14,rbp
	call	40EC10h
	mov	rdi,[000000000061A650]                                 ; [rip+002105EA]
	mov	rcx,rax
	mov	edx,415E4Ah
	xor	eax,eax
	mov	esi,1h
	add	rbp,r13
	call	402810h
	mov	r12,[r15+rbx*8]
	test	r12,r12
	jz	40A0D0h

l000000000040A086:
	test	rbx,rbx
	jz	40A050h

l000000000040A08B:
	mov	rdx,r13
	mov	rsi,rbp
	mov	rdi,r14
	call	402500h
	test	eax,eax
	jnz	40A050h

l000000000040A09D:
	mov	rdi,r12
	add	rbx,1h
	add	rbp,r13
	call	40EC10h
	mov	rdi,[000000000061A650]                                 ; [rip+0021059D]
	mov	rcx,rax
	mov	edx,415E52h
	xor	eax,eax
	mov	esi,1h
	call	402810h
	mov	r12,[r15+rbx*8]
	test	r12,r12
	jnz	40A086h

l000000000040A0D0:
	mov	rdi,[000000000061A650]                                 ; [rip+00210579]
	mov	rax,[rdi+28h]
	cmp	rax,[rdi+30h]
	jnc	40A0FBh

l000000000040A0E1:
	lea	rdx,[rax+1h]
	mov	[rdi+28h],rdx
	mov	byte ptr [rax],0Ah
	add	rsp,8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret

l000000000040A0FB:
	add	rsp,8h
	mov	esi,0Ah
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	jmp	402400h
000000000040A113          66 66 66 66 2E 0F 1F 84 00 00 00 00 00    ffff.........

;; fn000000000040A120: 000000000040A120
;;   Called from:
;;     0000000000402E83 (in fn00000000004028C0)
;;     0000000000402EC3 (in fn00000000004028C0)
;;     0000000000402F38 (in fn00000000004028C0)
;;     0000000000402F73 (in fn00000000004028C0)
;;     0000000000402FF9 (in fn00000000004028C0)
;;     000000000040305A (in fn00000000004028C0)
fn000000000040A120 proc
	push	r15
	mov	r15,r8
	push	r14
	mov	r14,rdi
	push	r13
	mov	r13,r9
	push	r12
	mov	r12,rsi
	mov	rdi,r12
	push	rbp
	mov	rbp,rcx
	mov	rcx,r8
	push	rbx
	mov	rbx,rdx
	mov	rdx,rbp
	mov	rsi,rbx
	sub	rsp,8h
	call	409E50h
	test	rax,rax
	js	40A165h

l000000000040A156:
	add	rsp,8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret

l000000000040A165:
	mov	rdx,rax
	mov	rsi,r12
	mov	rdi,r14
	call	409F80h
	mov	rdx,r15
	mov	rsi,rbp
	mov	rdi,rbx
	call	40A000h
	call	r13
	mov	rax,-1h
	jmp	40A156h
000000000040A18D                                        0F 1F 00              ...
000000000040A190 41 56 41 55 41 54 55 53 4C 8B 26 4D 85 E4 74 32 AVAUATUSL.&M..t2
000000000040A1A0 49 89 FE 49 89 CD 48 89 D3 48 8D 6E 08 EB 11 90 I..I..H..H.n....
000000000040A1B0 4C 8B 65 00 4C 01 EB 48 83 C5 08 4D 85 E4 74 12 L.e.L..H...M..t.
000000000040A1C0 4C 89 EA 48 89 DE 4C 89 F7 E8 32 83 FF FF 85 C0 L..H..L...2.....
000000000040A1D0 75 DE 5B 5D 4C 89 E0 41 5C 41 5D 41 5E C3 66 90 u.[]L..A\A]A^.f.
000000000040A1E0 48 89 3D F1 0F 21 00 C3 0F 1F 84 00 00 00 00 00 H.=..!..........
000000000040A1F0 40 88 3D D9 0F 21 00 C3 0F 1F 84 00 00 00 00 00 @.=..!..........
000000000040A200 55 53 48 83 EC 08 48 8B 3D 03 04 21 00 E8 6E 76 USH...H.=..!..nv
000000000040A210 00 00 85 C0 74 13 80 3D B3 0F 21 00 00 74 21 E8 ....t..=..!..t!.
000000000040A220 0C 80 FF FF 83 38 20 75 17 48 8B 3D 20 04 21 00 .....8 u.H.= .!.
000000000040A230 E8 4B 76 00 00 85 C0 75 4A 48 83 C4 08 5B 5D C3 .Kv....uJH...[].
000000000040A240 31 FF BA 05 00 00 00 BE 57 5E 41 00 E8 0F 81 FF 1.......W^A.....
000000000040A250 FF 48 8B 3D 80 0F 21 00 48 89 C3 48 85 FF 74 2E .H.=..!.H..H..t.
000000000040A260 E8 4B 48 00 00 48 89 C5 E8 C3 7F FF FF 8B 30 49 .KH..H........0I
000000000040A270 89 D8 48 89 E9 BA 63 5E 41 00 31 FF 31 C0 E8 ED ..H...c^A.1.1...
000000000040A280 84 FF FF 8B 3D F7 02 21 00 E8 C2 7F FF FF E8 9D ....=..!........
000000000040A290 7F FF FF 8B 30 48 89 D9 BA 54 5E 41 00 31 FF 31 ....0H...T^A.1.1
000000000040A2A0 C0 E8 CA 84 FF FF EB DB 0F 1F 84 00 00 00 00 00 ................

;; fn000000000040A2B0: 000000000040A2B0
;;   Called from:
;;     0000000000408543 (in fn0000000000407EA0)
fn000000000040A2B0 proc
	push	rbp
	xor	ebp,ebp
	push	rbx
	mov	rbx,rdi
	sub	rsp,8h
	cmp	byte ptr [rdi],2Fh
	setz	bpl
	call	40A390h
	mov	rcx,rax
	sub	rcx,rbx
	cmp	rcx,rbp
	jbe	40A2FDh

l000000000040A2D2:
	cmp	byte ptr [rax-1h],2Fh
	lea	rdx,[rcx-1h]
	jz	40A2EEh

l000000000040A2DC:
	jmp	40A2FDh
000000000040A2DE                                           66 90               f.

l000000000040A2E0:
	cmp	byte ptr [rbx+rdx-1h],2Fh
	lea	rax,[rdx-1h]
	jnz	40A2F3h

l000000000040A2EB:
	mov	rdx,rax

l000000000040A2EE:
	cmp	rbp,rdx
	jc	40A2E0h

l000000000040A2F3:
	add	rsp,8h
	mov	rax,rdx
	pop	rbx
	pop	rbp
	ret

l000000000040A2FD:
	mov	rdx,rcx
	jmp	40A2F3h
000000000040A302       66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00   fffff.........
000000000040A310 41 55 41 54 49 89 FC 55 53 48 83 EC 08 E8 8E FF AUATI..USH......
000000000040A320 FF FF 48 85 C0 4C 8D 68 01 48 89 C3 40 0F 94 C5 ..H..L.h.H..@...
000000000040A330 40 0F B6 FD 4C 01 EF E8 04 83 FF FF 48 85 C0 74 @...L.......H..t
000000000040A340 3F 48 89 DA 4C 89 E6 48 89 C7 E8 71 82 FF FF 40 ?H..L..H...q...@
000000000040A350 84 ED 48 89 C1 75 19 C6 04 19 00 48 89 C8 48 83 ..H..u.....H..H.
000000000040A360 C4 08 5B 5D 41 5C 41 5D C3 0F 1F 80 00 00 00 00 ..[]A\A]........
000000000040A370 C6 04 18 2E 4C 89 EB EB DE 0F 1F 80 00 00 00 00 ....L...........
000000000040A380 31 C0 EB DA 66 2E 0F 1F 84 00 00 00 00 00 66 90 1...f.........f.

;; fn000000000040A390: 000000000040A390
;;   Called from:
;;     0000000000405138 (in fn0000000000405090)
;;     0000000000409B24 (in fn0000000000409750)
;;     0000000000409BCF (in fn0000000000409750)
;;     0000000000409C06 (in fn0000000000409750)
;;     000000000040A2C2 (in fn000000000040A2B0)
;;     000000000040A64A (in fn000000000040A630)
fn000000000040A390 proc
	movzx	edx,byte ptr [rdi]
	mov	rax,rdi
	cmp	dl,2Fh
	jnz	40A3ACh

l000000000040A39B:
	nop	dword ptr [rax+rax+0h]

l000000000040A3A0:
	add	rax,1h
	movzx	edx,byte ptr [rax]
	cmp	dl,2Fh
	jz	40A3A0h

l000000000040A3AC:
	mov	ecx,edx
	xor	esi,esi
	mov	rdx,rax
	test	cl,cl
	jz	40A3F7h

l000000000040A3B7:
	nop	word ptr [rax+rax+0h]

l000000000040A3C0:
	add	rdx,1h
	movzx	ecx,byte ptr [rdx]
	test	cl,cl
	jz	40A3E5h

l000000000040A3CB:
	cmp	cl,2Fh
	jz	40A3F0h

l000000000040A3D0:
	test	sil,sil
	jz	40A3C0h

l000000000040A3D5:
	mov	rax,rdx
	add	rdx,1h
	movzx	ecx,byte ptr [rdx]
	xor	esi,esi
	test	cl,cl
	jnz	40A3CBh

l000000000040A3E5:
	ret
000000000040A3E7                      66 0F 1F 84 00 00 00 00 00        f........

l000000000040A3F0:
	mov	esi,1h
	jmp	40A3C0h

l000000000040A3F7:
	ret
000000000040A3F9                            0F 1F 80 00 00 00 00          .......

;; fn000000000040A400: 000000000040A400
;;   Called from:
;;     000000000040A655 (in fn000000000040A630)
fn000000000040A400 proc
	push	rbx
	mov	rbx,rdi
	call	402380h
	cmp	rax,1h
	jbe	40A41Ah

l000000000040A40F:
	cmp	byte ptr [rbx+rax-1h],2Fh
	lea	rdx,[rax-1h]
	jz	40A420h

l000000000040A41A:
	pop	rbx
	ret
000000000040A41C                                     0F 1F 40 00             ..@.

l000000000040A420:
	cmp	rdx,1h
	mov	rax,rdx
	jnz	40A40Fh

l000000000040A429:
	pop	rbx
	ret
000000000040A42B                                  0F 1F 44 00 00            ..D..

l000000000040A430:
	mov	eax,edi
	and	eax,0F000h
	cmp	eax,8000h
	jz	40A5B0h

l000000000040A442:
	cmp	eax,4000h
	jz	40A5C0h

l000000000040A44D:
	cmp	eax,6000h
	jz	40A5D0h

l000000000040A458:
	cmp	eax,2000h
	jz	40A5A0h

l000000000040A463:
	cmp	eax,0A000h
	jz	40A5E0h

l000000000040A46E:
	cmp	eax,1000h
	jz	40A5F0h

l000000000040A479:
	cmp	eax,0C000h
	mov	edx,73h
	mov	eax,3Fh
	cmovnz	edx,eax

l000000000040A48B:
	mov	eax,edi
	mov	[rsi],dl
	and	eax,100h
	cmp	eax,1h
	sbb	eax,eax
	and	eax,0BBh
	add	eax,72h
	mov	[rsi+1h],al
	mov	eax,edi
	and	eax,80h
	cmp	eax,1h
	sbb	eax,eax
	and	eax,0B6h
	add	eax,77h
	mov	[rsi+2h],al
	mov	eax,edi
	and	eax,40h
	cmp	eax,1h
	sbb	eax,eax
	test	edi,800h
	jz	40A590h

l000000000040A4CD:
	and	eax,0E0h
	add	eax,73h

l000000000040A4D3:
	mov	[rsi+3h],al
	mov	eax,edi
	and	eax,20h
	cmp	eax,1h
	sbb	eax,eax
	and	eax,0BBh
	add	eax,72h
	mov	[rsi+4h],al
	mov	eax,edi
	and	eax,10h
	cmp	eax,1h
	sbb	eax,eax
	and	eax,0B6h
	add	eax,77h
	mov	[rsi+5h],al
	mov	eax,edi
	and	eax,8h
	cmp	eax,1h
	sbb	eax,eax
	test	edi,400h
	jz	40A580h

l000000000040A50E:
	and	eax,0E0h
	add	eax,73h

l000000000040A514:
	mov	[rsi+6h],al
	mov	eax,edi
	and	eax,4h
	cmp	eax,1h
	sbb	eax,eax
	and	eax,0BBh
	add	eax,72h
	mov	[rsi+7h],al
	mov	eax,edi
	and	eax,2h
	cmp	eax,1h
	sbb	eax,eax
	and	eax,0B6h
	add	eax,77h
	test	edi,200h
	mov	[rsi+8h],al
	jz	40A560h

l000000000040A545:
	and	edi,1h
	mov	byte ptr [rsi+0Ah],20h
	mov	byte ptr [rsi+0Bh],0h
	cmp	edi,1h
	sbb	eax,eax
	and	eax,0E0h
	add	eax,74h
	mov	[rsi+9h],al
	ret
000000000040A55F                                              90                .

l000000000040A560:
	and	edi,1h
	mov	byte ptr [rsi+0Ah],20h
	mov	byte ptr [rsi+0Bh],0h
	cmp	edi,1h
	sbb	eax,eax
	and	eax,0B5h
	add	eax,78h
	mov	[rsi+9h],al
	ret
000000000040A57A                               66 0F 1F 44 00 00           f..D..

l000000000040A580:
	and	eax,0B5h
	add	eax,78h
	jmp	40A514h
000000000040A588                         0F 1F 84 00 00 00 00 00         ........

l000000000040A590:
	and	eax,0B5h
	add	eax,78h
	jmp	40A4D3h
000000000040A59B                                  0F 1F 44 00 00            ..D..

l000000000040A5A0:
	mov	edx,63h
	jmp	40A48Bh
000000000040A5AA                               66 0F 1F 44 00 00           f..D..

l000000000040A5B0:
	mov	edx,2Dh
	jmp	40A48Bh
000000000040A5BA                               66 0F 1F 44 00 00           f..D..

l000000000040A5C0:
	mov	edx,64h
	jmp	40A48Bh
000000000040A5CA                               66 0F 1F 44 00 00           f..D..

l000000000040A5D0:
	mov	edx,62h
	jmp	40A48Bh
000000000040A5DA                               66 0F 1F 44 00 00           f..D..

l000000000040A5E0:
	mov	edx,6Ch
	jmp	40A48Bh
000000000040A5EA                               66 0F 1F 44 00 00           f..D..

l000000000040A5F0:
	mov	edx,70h
	jmp	40A48Bh
000000000040A5FA                               66 0F 1F 44 00 00           f..D..

;; fn000000000040A600: 000000000040A600
;;   Called from:
;;     0000000000406BB0 (in fn0000000000406B70)
fn000000000040A600 proc
	mov	edi,[rdi+18h]
	jmp	40A430h
000000000040A608                         0F 1F 84 00 00 00 00 00         ........

;; fn000000000040A610: 000000000040A610
;;   Called from:
;;     0000000000405168 (in fn0000000000405090)
fn000000000040A610 proc
	sub	rsp,8h
	call	40A630h
	test	rax,rax
	jz	40A623h

l000000000040A61E:
	add	rsp,8h
	ret

l000000000040A623:
	call	410E50h
	nop	dword ptr [rax+rax+0h]

;; fn000000000040A630: 000000000040A630
;;   Called from:
;;     000000000040A614 (in fn000000000040A610)
;;     000000000040A628 (in fn000000000040A610)
fn000000000040A630 proc
	push	r15
	mov	r15,rdx
	push	r14
	xor	r14d,r14d
	push	r13
	push	r12
	mov	r12,rdi
	push	rbp
	mov	rbp,rsi
	push	rbx
	sub	rsp,18h
	call	40A390h
	mov	rbx,rax
	mov	rdi,rax
	call	40A400h
	mov	rdx,rbx
	sub	rdx,r12
	test	rax,rax
	lea	rsi,[rdx+rax]
	mov	[rsp],rsi
	jz	40A679h

l000000000040A66D:
	xor	r14d,r14d
	cmp	byte ptr [rbx+rax-1h],2Fh
	setnz	r14b

l000000000040A679:
	cmp	byte ptr [rbp+0h],2Fh
	mov	rbx,rbp
	jz	40A700h

l000000000040A682:
	mov	rdi,rbx
	call	402380h
	mov	rcx,[rsp]
	mov	[rsp+8h],rax
	lea	rdi,[rcx+r14+1h]
	add	rdi,rax
	call	402640h
	test	rax,rax
	mov	r13,rax
	jz	40A720h

l000000000040A6A8:
	mov	rdx,[rsp]
	mov	rdi,rax
	mov	rsi,r12
	call	402750h
	test	r15,r15
	mov	byte ptr [rax],2Fh
	lea	rdi,[rax+r14]
	jz	40A6D5h

l000000000040A6C3:
	xor	eax,eax
	cmp	byte ptr [rbp+0h],2Fh
	mov	rcx,rdi
	setz	al
	sub	rcx,rax
	mov	[r15],rcx

l000000000040A6D5:
	mov	rdx,[rsp+8h]
	mov	rsi,rbx
	call	402750h
	mov	byte ptr [rax],0h
	mov	rax,r13

l000000000040A6E8:
	add	rsp,18h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040A6F7                      66 0F 1F 84 00 00 00 00 00        f........

l000000000040A700:
	add	rbx,1h
	cmp	byte ptr [rbx],2Fh
	jnz	40A682h

l000000000040A70D:
	add	rbx,1h
	cmp	byte ptr [rbx],2Fh
	jnz	40A682h

l000000000040A71A:
	jmp	40A700h
000000000040A71C                                     0F 1F 40 00             ..@.

l000000000040A720:
	xor	eax,eax
	jmp	40A6E8h
000000000040A724             66 2E 0F 1F 84 00 00 00 00 00 66 90     f.........f.

;; fn000000000040A730: 000000000040A730
fn000000000040A730 proc
	mov	rdx,[rdi]
	movzx	ecx,byte ptr [rdx]
	test	cl,cl
	jz	40A7CDh

l000000000040A73E:
	xor	r8d,r8d
	xor	eax,eax
	xor	r10d,r10d
	jmp	40A776h
000000000040A748                         0F 1F 84 00 00 00 00 00         ........

l000000000040A750:
	movsx	esi,cl
	xor	r8d,r8d
	and	esi,0DFh
	sub	esi,41h
	cmp	esi,19h
	jbe	40A768h

l000000000040A761:
	cmp	cl,7Eh
	cmovnz	rax,r10

l000000000040A768:
	add	rdx,1h
	mov	[rdi],rdx
	movzx	ecx,byte ptr [rdx]
	test	cl,cl
	jz	40A7B0h

l000000000040A776:
	test	r8b,r8b
	jnz	40A750h

l000000000040A77B:
	cmp	cl,2Eh
	jz	40A7B8h

l000000000040A780:
	movsx	esi,cl
	lea	r9d,[rsi-30h]
	cmp	r9d,9h
	jbe	40A768h

l000000000040A78D:
	and	esi,0DFh
	sub	esi,41h
	cmp	esi,19h
	ja	40A761h

l000000000040A798:
	add	rdx,1h
	mov	[rdi],rdx
	movzx	ecx,byte ptr [rdx]
	test	cl,cl
	jnz	40A776h

l000000000040A7A6:
	nop	word ptr cs:[rax+rax+0h]

l000000000040A7B0:
	ret
000000000040A7B2       66 0F 1F 44 00 00                           f..D..        

l000000000040A7B8:
	test	rax,rax
	jz	40A7C8h

l000000000040A7BD:
	mov	r8d,1h
	jmp	40A768h
000000000040A7C5                0F 1F 00                              ...        

l000000000040A7C8:
	mov	rax,rdx
	jmp	40A7BDh

l000000000040A7CD:
	xor	eax,eax
	ret
000000000040A7D0 41 56 41 55 41 54 55 48 89 F5 53 48 89 FB 48 83 AVAUATUH..SH..H.
000000000040A7E0 EC 10 E8 69 7D FF FF 41 89 C5 31 C0 45 85 ED 74 ...i}..A..1.E..t
000000000040A7F0 47 0F B6 13 84 D2 0F 84 EB 02 00 00 0F B6 4D 00 G.............M.
000000000040A800 B0 01 84 C9 74 32 0F B6 C2 BE 2E 00 00 00 29 C6 ....t2........).
000000000040A810 75 0B 80 7B 01 00 B8 FF FF FF FF 74 1B 0F B6 C1 u..{.......t....
000000000040A820 BF 2E 00 00 00 29 C7 75 1F 80 7D 01 00 B8 01 00 .....).u..}.....
000000000040A830 00 00 75 14 0F 1F 40 00 48 83 C4 10 5B 5D 41 5C ..u...@.H...[]A\
000000000040A840 41 5D 41 5E C3 0F 1F 00 85 F6 75 11 80 7B 01 2E A]A^......u..{..
000000000040A850 75 0B 80 7B 02 00 B8 FF FF FF FF 74 DB 85 FF 0F u..{.......t....
000000000040A860 84 3B 01 00 00 80 FA 2E 0F 84 52 02 00 00 80 F9 .;........R.....
000000000040A870 2E 0F 84 3D 01 00 00 48 89 E7 48 89 1C 24 48 89 ...=...H..H..$H.
000000000040A880 6C 24 08 E8 A8 FE FF FF 48 8D 7C 24 08 49 89 C6 l$......H.|$.I..
000000000040A890 4D 89 F4 E8 98 FE FF FF 4D 85 F6 4C 0F 44 24 24 M.......M..L.D$$
000000000040A8A0 49 89 C2 49 29 EA 49 29 DC 48 85 C0 0F 84 3F 02 I..I).I).H....?.
000000000040A8B0 00 00 4D 39 D4 0F 84 D0 01 00 00 31 C9 45 31 C0 ..M9.......1.E1.
000000000040A8C0 41 BB FF FF FF FF 49 39 CA 0F 87 A2 00 00 00 E9 A.....I9........
000000000040A8D0 33 02 00 00 0F 1F 40 00 49 39 CA 44 0F BE 4C 0D 3.....@.I9.D..L.
000000000040A8E0 00 0F 86 ED 00 00 00 41 0F BE C1 83 E8 30 83 F8 .......A.....0..
000000000040A8F0 09 0F 86 DD 00 00 00 4D 39 C4 0F 84 18 02 00 00 .......M9.......
000000000040A900 42 0F B6 14 03 0F B6 F2 31 C0 8D 7E D0 83 FF 09 B.......1..~....
000000000040A910 76 13 89 F0 83 E0 DF 83 E8 41 83 F8 19 0F 87 45 v........A.....E
000000000040A920 01 00 00 89 F0 49 39 CA 0F 84 F1 01 00 00 44 0F .....I9.......D.
000000000040A930 B6 4C 0D 00 41 0F B6 F1 31 FF 8D 56 D0 83 FA 09 .L..A...1..V....
000000000040A940 76 1F 89 F2 89 F7 83 E2 DF 83 EA 41 83 FA 19 76 v..........A...v
000000000040A950 10 81 C6 00 01 00 00 41 80 F9 7E 89 F7 41 0F 44 .......A..~..A.D
000000000040A960 FB 39 F8 0F 85 17 01 00 00 49 83 C0 01 48 83 C1 .9.......I...H..
000000000040A970 01 4D 39 C4 0F 86 5E FF FF FF 42 0F B6 34 03 40 .M9...^...B..4.@
000000000040A980 0F BE C6 83 E8 30 83 F8 09 0F 86 49 FF FF FF 89 .....0.....I....
000000000040A990 F2 E9 6F FF FF FF 66 2E 0F 1F 84 00 00 00 00 00 ..o...f.........
000000000040A9A0 80 7D 01 2E 0F 85 BB FE FF FF 80 7D 02 00 0F 85 .}.........}....
000000000040A9B0 B1 FE FF FF 48 83 C4 10 B8 01 00 00 00 5B 5D 41 ....H........[]A
000000000040A9C0 5C 41 5D 41 5E C3 66 2E 0F 1F 84 00 00 00 00 00 \A]A^.f.........
000000000040A9D0 49 83 C0 01 42 0F B6 14 03 80 FA 30 74 F2 EB 0A I...B......0t...
000000000040A9E0 48 83 C1 01 44 0F BE 4C 0D 00 41 80 F9 30 74 F0 H...D..L..A..0t.
000000000040A9F0 0F BE C2 83 E8 30 83 F8 09 41 0F BE C1 0F 87 D8 .....0...A......
000000000040AA00 00 00 00 83 E8 30 83 F8 09 77 A9 31 C0 EB 13 90 .....0...w.1....
000000000040AA10 44 0F BE 4C 0D 00 41 0F BE F1 83 EE 30 83 FE 09 D..L..A.....0...
000000000040AA20 77 92 85 C0 75 06 0F BE C2 44 29 C8 49 83 C0 01 w...u....D).I...
000000000040AA30 48 83 C1 01 42 0F B6 14 03 0F BE F2 83 EE 30 83 H...B.........0.
000000000040AA40 FE 09 76 CC 0F BE 54 0D 00 83 EA 30 83 FA 09 0F ..v...T....0....
000000000040AA50 86 92 00 00 00 85 C0 0F 84 69 FE FF FF E9 D6 FD .........i......
000000000040AA60 FF FF 66 0F 1F 44 00 00 81 C6 00 01 00 00 80 FA ..f..D..........
000000000040AA70 7E 89 F0 41 0F 44 C3 E9 A9 FE FF FF 0F 1F 40 00 ~..A.D........@.
000000000040AA80 29 F8 41 0F 44 C5 E9 AD FD FF FF 4C 89 E2 48 89 ).A.D......L..H.
000000000040AA90 EE 48 89 DF E8 A7 77 FF FF 85 C0 4D 89 E2 0F 85 .H....w....M....
000000000040AAA0 17 FE FF FF 4C 8B 24 24 4C 8B 54 24 08 49 29 DC ....L.$$L.T$.I).
000000000040AAB0 49 29 EA E9 03 FE FF FF 0F 1F 84 00 00 00 00 00 I)..............
000000000040AAC0 48 83 C3 01 48 83 C5 01 80 F9 2E B8 FF FF FF FF H...H...........
000000000040AAD0 0F 84 A1 FD FF FF E9 5D FD FF FF 83 E8 30 83 F8 .......].....0..
000000000040AAE0 09 0F 87 DF FD FF FF B8 FF FF FF FF E9 47 FD FF .............G..
000000000040AAF0 FF 4C 8B 54 24 08 49 29 EA 4D 85 F6 0F 84 B9 FD .L.T$.I).M......
000000000040AB00 FF FF E9 AB FD FF FF 4D 39 C4 0F 87 61 FE FF FF .......M9...a...
000000000040AB10 44 89 E8 E9 20 FD FF FF 31 C0 E9 15 FE FF FF 31 D... ...1......1
000000000040AB20 FF E9 3B FE FF FF 66 2E 0F 1F 84 00 00 00 00 00 ..;...f.........

;; fn000000000040AB30: 000000000040AB30
;;   Called from:
;;     000000000040738A (in fn0000000000406B70)
fn000000000040AB30 proc
	push	rbx
	mov	rsi,rdi
	mov	rbx,rdi
	xor	edi,edi
	sub	rsp,10h
	call	4022F0h
	test	eax,eax
	jz	40AB67h

l000000000040AB46:
	xor	esi,esi
	mov	rdi,rsp
	call	402440h
	mov	rax,[rsp]
	mov	[rbx],rax
	mov	rax,[rsp+8h]
	imul	rax,rax,+3E8h
	mov	[rbx+8h],rax

l000000000040AB67:
	add	rsp,10h
	pop	rbx
	ret
000000000040AB6D                                        0F 1F 00              ...

;; fn000000000040AB70: 000000000040AB70
;;   Called from:
;;     000000000040382D (in fn00000000004028C0)
;;     000000000040389B (in fn00000000004028C0)
fn000000000040AB70 proc
	sub	rsp,8h
	xor	esi,esi
	call	402710h
	test	rax,rax
	jz	40ABB0h

l000000000040AB80:
	cmp	byte ptr [rax],43h
	jnz	40AB98h

l000000000040AB85:
	cmp	byte ptr [rax+1h],0h
	jnz	40AB98h

l000000000040AB8B:
	xor	eax,eax
	add	rsp,8h
	ret
000000000040AB92       66 0F 1F 44 00 00                           f..D..        

l000000000040AB98:
	mov	rsi,rax
	mov	edi,415E6Ah
	mov	ecx,6h

l000000000040ABA5:
	rep cmpsb

l000000000040ABA7:
	setnz	al
	add	rsp,8h
	ret
000000000040ABAF                                              90                .

l000000000040ABB0:
	mov	eax,1h
	add	rsp,8h
	ret
000000000040ABBA                               66 0F 1F 44 00 00           f..D..

;; fn000000000040ABC0: 000000000040ABC0
;;   Called from:
;;     000000000040B4C3 (in fn000000000040B400)
;;     000000000040B773 (in fn000000000040B710)
fn000000000040ABC0 proc
	cmp	rdi,9h
	ja	40AC4Fh

l000000000040ABCA:
	mov	edi,0Bh

l000000000040ABCF:
	mov	r9,0AAAAAAAAAAAAAAABh
	nop	dword ptr [rax+0h]

l000000000040ABE0:
	cmp	rdi,9h
	jbe	40AC48h

l000000000040ABE6:
	mov	rax,rdi
	mul	r9
	shr	rdx,1h
	lea	rax,[rdx+rdx*2]
	cmp	rdi,rax
	jz	40AC3Ah

l000000000040ABF8:
	mov	r8d,10h
	mov	esi,9h
	mov	ecx,3h
	jmp	40AC21h
000000000040AC0A                               66 0F 1F 44 00 00           f..D..

l000000000040AC10:
	xor	edx,edx
	mov	rax,rdi
	add	r8,8h
	div	rcx
	test	rdx,rdx
	jz	40AC3Ah

l000000000040AC21:
	add	rsi,r8
	add	rcx,2h
	cmp	rsi,rdi
	jc	40AC10h

l000000000040AC2D:
	xor	edx,edx
	mov	rax,rdi
	div	rcx
	test	rdx,rdx
	jnz	40AC44h

l000000000040AC3A:
	add	rdi,2h
	cmp	rdi,0FFh
	jnz	40ABE0h

l000000000040AC44:
	mov	rax,rdi
	ret

l000000000040AC48:
	mov	ecx,3h
	jmp	40AC2Dh

l000000000040AC4F:
	or	rdi,1h
	cmp	rdi,0FFh
	jnz	40ABCFh

l000000000040AC5D:
	jmp	40AC44h
000000000040AC5F                                              90                .
000000000040AC60 48 C1 CF 03 31 D2 48 89 F8 48 F7 F6 48 89 D0 C3 H...1.H..H..H...
000000000040AC70 48 39 F7 0F 94 C0 C3 66 0F 1F 84 00 00 00 00 00 H9.....f........

;; fn000000000040AC80: 000000000040AC80
;;   Called from:
;;     000000000040ACC4 (in fn000000000040ACB0)
;;     000000000040AE9D (in fn000000000040AE40)
;;     000000000040AF0E (in fn000000000040AE40)
fn000000000040AC80 proc
	push	rbx
	mov	rbx,rdi
	mov	rdi,rsi
	mov	rsi,[rbx+10h]
	call	qword ptr [rbx+30h]
	cmp	rax,[rbx+10h]
	jnc	40AC9Dh

l000000000040AC94:
	shl	rax,4h
	add	rax,[rbx]
	pop	rbx
	ret

l000000000040AC9D:
	call	402220h
000000000040ACA2       66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00   fffff.........

;; fn000000000040ACB0: 000000000040ACB0
;;   Called from:
;;     000000000040B911 (in fn000000000040B8F0)
;;     000000000040BA59 (in fn000000000040B8F0)
;;     000000000040BBA5 (in fn000000000040BB90)
fn000000000040ACB0 proc
	push	r14
	mov	r14,rdx
	push	r13
	mov	r13d,ecx
	push	r12
	mov	r12,rsi
	push	rbp
	mov	rbp,rdi
	push	rbx
	call	40AC80h
	mov	[r14],rax
	mov	rsi,[rax]
	mov	rbx,rax
	test	rsi,rsi
	jz	40AD4Fh

l000000000040ACD7:
	cmp	r12,rsi
	jz	40ACE9h

l000000000040ACDC:
	mov	rdi,r12
	call	qword ptr [rbp+38h]
	test	al,al
	jz	40AD46h

l000000000040ACE6:
	mov	rsi,[rbx]

l000000000040ACE9:
	test	r13b,r13b
	jz	40AD1Ch

l000000000040ACEE:
	mov	rax,[rbx+8h]
	test	rax,rax
	jz	40AD98h

l000000000040ACFB:
	mov	r9,[rax]
	mov	r10,[rax+8h]
	mov	[rbx],r9
	mov	[rbx+8h],r10
	mov	qword ptr [rax],+0h
	mov	rcx,[rbp+48h]
	mov	[rax+8h],rcx
	mov	[rbp+48h],rax

l000000000040AD1C:
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	mov	rax,rsi
	pop	r14
	ret
000000000040AD28                         0F 1F 84 00 00 00 00 00         ........

l000000000040AD30:
	mov	rsi,[rax]
	cmp	rsi,r12
	jz	40AD67h

l000000000040AD38:
	mov	rdi,r12
	call	qword ptr [rbp+38h]
	test	al,al
	jnz	40AD60h

l000000000040AD42:
	mov	rbx,[rbx+8h]

l000000000040AD46:
	mov	rax,[rbx+8h]
	test	rax,rax
	jnz	40AD30h

l000000000040AD4F:
	pop	rbx
	pop	rbp
	pop	r12
	xor	esi,esi
	pop	r13
	mov	rax,rsi
	pop	r14
	ret
000000000040AD5D                                        0F 1F 00              ...

l000000000040AD60:
	mov	rax,[rbx+8h]
	mov	rsi,[rax]

l000000000040AD67:
	test	r13b,r13b
	jz	40AD1Ch

l000000000040AD6C:
	mov	rcx,[rax+8h]
	mov	[rbx+8h],rcx
	mov	qword ptr [rax],+0h
	mov	rcx,[rbp+48h]
	mov	[rax+8h],rcx
	mov	[rbp+48h],rax
	mov	rax,rsi
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	ret
000000000040AD93          0F 1F 44 00 00                            ..D..        

l000000000040AD98:
	mov	qword ptr [rbx],+0h
	jmp	40AD1Ch
000000000040ADA4             66 66 66 2E 0F 1F 84 00 00 00 00 00     fff.........

;; fn000000000040ADB0: 000000000040ADB0
;;   Called from:
;;     000000000040B45F (in fn000000000040B400)
;;     000000000040B9CC (in fn000000000040B8F0)
;;     000000000040BC1C (in fn000000000040BB90)
fn000000000040ADB0 proc
	mov	rax,[rdi]
	cmp	rax,+415EE0h
	jz	40AE30h

l000000000040ADBB:
	movss	xmm0,dword ptr [rax+8h]
	ucomiss	xmm0,[0000000000415EF4]                            ; [rip+0000B12D]
	jbe	40AE20h

l000000000040ADC9:
	movss	xmm1,[0000000000415EF8]                              ; [rip+0000B127]
	ucomiss	xmm1,xmm0
	jbe	40AE20h

l000000000040ADD6:
	movss	xmm1,dword ptr [rax+0Ch]
	ucomiss	xmm1,[0000000000415EFC]                            ; [rip+0000B11A]
	jbe	40AE20h

l000000000040ADE4:
	movss	xmm1,dword ptr [rax]
	ucomiss	xmm1,[0000000000415F00]                            ; [rip+0000B111]
	jc	40AE20h

l000000000040ADF1:
	addss	xmm1,[0000000000415EF4]                              ; [rip+0000B0FB]
	movss	xmm2,dword ptr [rax+4h]
	ucomiss	xmm2,xmm1
	jbe	40AE20h

l000000000040AE03:
	movss	xmm3,[0000000000415F04]                              ; [rip+0000B0F9]
	ucomiss	xmm3,xmm2
	jc	40AE20h

l000000000040AE10:
	ucomiss	xmm0,xmm1
	mov	eax,1h
	ja	40AE35h

l000000000040AE1A:
	nop	word ptr [rax+rax+0h]

l000000000040AE20:
	mov	qword ptr [rdi],+415EE0h
	xor	eax,eax
	ret
000000000040AE2A                               66 0F 1F 44 00 00           f..D..

l000000000040AE30:
	mov	eax,1h

l000000000040AE35:
	ret
000000000040AE37                      66 0F 1F 84 00 00 00 00 00        f........

;; fn000000000040AE40: 000000000040AE40
;;   Called from:
;;     000000000040B80A (in fn000000000040B710)
;;     000000000040B829 (in fn000000000040B710)
;;     000000000040B83E (in fn000000000040B710)
fn000000000040AE40 proc
	push	r15
	mov	r15d,edx
	push	r14
	mov	r14,rsi
	push	r13
	push	r12
	mov	r12,rdi
	push	rbp
	push	rbx
	sub	rsp,8h
	mov	r13,[rsi]
	cmp	r13,[rsi+8h]
	jnc	40AEEFh

l000000000040AE64:
	nop	dword ptr [rax+0h]

l000000000040AE68:
	mov	rbp,[r13+0h]
	test	rbp,rbp
	jz	40AEE1h

l000000000040AE71:
	mov	rbx,[r13+8h]
	test	rbx,rbx
	jnz	40AE94h

l000000000040AE7A:
	jmp	40AED4h
000000000040AE7C                                     0F 1F 40 00             ..@.

l000000000040AE80:
	mov	rcx,[rax+8h]
	test	rdx,rdx
	mov	[rbx+8h],rcx
	mov	[rax+8h],rbx
	jz	40AED0h

l000000000040AE91:
	mov	rbx,rdx

l000000000040AE94:
	mov	rbp,[rbx]
	mov	rdi,r12
	mov	rsi,rbp
	call	40AC80h
	cmp	qword ptr [rax],0h
	mov	rdx,[rbx+8h]
	jnz	40AE80h

l000000000040AEAC:
	mov	[rax],rbp
	add	qword ptr [r12+18h],1h
	test	rdx,rdx
	mov	qword ptr [rbx],+0h
	mov	rax,[r12+48h]
	mov	[rbx+8h],rax
	mov	[r12+48h],rbx
	jnz	40AE91h

l000000000040AECF:
	nop

l000000000040AED0:
	mov	rbp,[r13+0h]

l000000000040AED4:
	test	r15b,r15b
	mov	qword ptr [r13+8h],+0h
	jz	40AF08h

l000000000040AEE1:
	add	r13,10h
	cmp	[r14+8h],r13
	ja	40AE68h

l000000000040AEEF:
	add	rsp,8h
	mov	eax,1h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040AF03          0F 1F 44 00 00                            ..D..        

l000000000040AF08:
	mov	rsi,rbp
	mov	rdi,r12
	call	40AC80h
	cmp	qword ptr [rax],0h
	mov	rbx,rax
	jz	40AF5Bh

l000000000040AF1C:
	mov	rax,[r12+48h]
	test	rax,rax
	jz	40AF66h

l000000000040AF26:
	mov	rdx,[rax+8h]
	mov	[r12+48h],rdx

l000000000040AF2F:
	mov	rdx,[rbx+8h]
	mov	[rax],rbp
	mov	[rax+8h],rdx
	mov	[rbx+8h],rax

l000000000040AF3E:
	mov	qword ptr [r13+0h],+0h
	sub	qword ptr [r14+18h],1h
	add	r13,10h
	cmp	[r14+8h],r13
	ja	40AE68h

l000000000040AF59:
	jmp	40AEEFh

l000000000040AF5B:
	mov	[rax],rbp
	add	qword ptr [r12+18h],1h
	jmp	40AF3Eh

l000000000040AF66:
	mov	edi,10h
	call	402640h
	test	rax,rax
	jnz	40AF2Fh

l000000000040AF75:
	add	rsp,8h
	xor	eax,eax
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040AF86                   66 2E 0F 1F 84 00 00 00 00 00       f.........
000000000040AF90 48 8B 47 10 C3 66 66 2E 0F 1F 84 00 00 00 00 00 H.G..ff.........
000000000040AFA0 48 8B 47 18 C3 66 66 2E 0F 1F 84 00 00 00 00 00 H.G..ff.........

;; fn000000000040AFB0: 000000000040AFB0
;;   Called from:
;;     0000000000404172 (in fn00000000004028C0)
fn000000000040AFB0 proc
	mov	rax,[rdi+20h]
	ret
000000000040AFB5                66 66 2E 0F 1F 84 00 00 00 00 00      ff.........
000000000040AFC0 48 8B 37 48 8B 7F 08 31 C0 48 39 FE 73 39 66 90 H.7H...1.H9.s9f.
000000000040AFD0 48 83 3E 00 74 26 48 8B 56 08 B9 01 00 00 00 48 H.>.t&H.V......H
000000000040AFE0 85 D2 74 11 0F 1F 40 00 48 8B 52 08 48 83 C1 01 ..t...@.H.R.H...
000000000040AFF0 48 85 D2 75 F3 48 39 C8 48 0F 42 C1 48 83 C6 10 H..u.H9.H.B.H...
000000000040B000 48 39 FE 72 CB F3 C3 F3 C3 0F 1F 80 00 00 00 00 H9.r............
000000000040B010 48 8B 0F 4C 8B 47 08 31 D2 31 F6 4C 39 C1 73 36 H..L.G.1.1.L9.s6
000000000040B020 48 83 39 00 74 27 48 8B 41 08 48 83 C6 01 48 83 H.9.t'H.A.H...H.
000000000040B030 C2 01 48 85 C0 74 16 66 0F 1F 84 00 00 00 00 00 ..H..t.f........
000000000040B040 48 8B 40 08 48 83 C2 01 48 85 C0 75 F3 48 83 C1 H.@.H...H..u.H..
000000000040B050 10 4C 39 C1 72 CA 31 C0 48 39 77 18 74 02 F3 C3 .L9.r.1.H9w.t...
000000000040B060 48 39 57 20 0F 94 C0 C3 0F 1F 84 00 00 00 00 00 H9W ............
000000000040B070 41 55 41 54 55 48 89 F5 53 31 DB 48 83 EC 08 4C AUATUH..S1.H...L
000000000040B080 8B 07 48 8B 77 08 48 8B 4F 20 4C 8B 67 10 4C 8B ..H.w.H.O L.g.L.
000000000040B090 6F 18 49 39 F0 73 3E 66 0F 1F 84 00 00 00 00 00 o.I9.s>f........
000000000040B0A0 49 83 38 00 74 26 49 8B 40 08 BA 01 00 00 00 48 I.8.t&I.@......H
000000000040B0B0 85 C0 74 11 0F 1F 40 00 48 8B 40 08 48 83 C2 01 ..t...@.H.@.H...
000000000040B0C0 48 85 C0 75 F3 48 39 D3 48 0F 42 DA 49 83 C0 10 H..u.H9.H.B.I...
000000000040B0D0 49 39 F0 72 CB BA 70 5E 41 00 BE 01 00 00 00 48 I9.r..p^A......H
000000000040B0E0 89 EF 31 C0 E8 27 77 FF FF 31 C0 4C 89 E1 BA 88 ..1..'w..1.L....
000000000040B0F0 5E 41 00 BE 01 00 00 00 48 89 EF E8 10 77 FF FF ^A......H....w..
000000000040B100 4D 85 ED 78 56 F2 49 0F 2A C5 4D 85 E4 F2 0F 59 M..xV.I.*.M....Y
000000000040B110 05 FB AD 00 00 78 69 F2 49 0F 2A CC F2 0F 5E C1 .....xi.I.*...^.
000000000040B120 4C 89 E9 48 89 EF BA B8 5E 41 00 BE 01 00 00 00 L..H....^A......
000000000040B130 B8 01 00 00 00 E8 D6 76 FF FF 48 83 C4 08 48 89 .......v..H...H.
000000000040B140 D9 48 89 EF 5B 5D 41 5C 41 5D BA A0 5E 41 00 BE .H..[]A\A]..^A..
000000000040B150 01 00 00 00 31 C0 E9 B5 76 FF FF 4C 89 E8 4C 89 ....1...v..L..L.
000000000040B160 EA 48 D1 E8 83 E2 01 48 09 D0 4D 85 E4 F2 48 0F .H.....H..M...H.
000000000040B170 2A C0 F2 0F 58 C0 F2 0F 59 05 92 AD 00 00 79 97 *...X...Y.....y.
000000000040B180 4C 89 E0 41 83 E4 01 48 D1 E8 4C 09 E0 F2 48 0F L..A...H..L...H.
000000000040B190 2A C8 F2 0F 58 C9 EB 84 0F 1F 84 00 00 00 00 00 *...X...........
000000000040B1A0 41 54 49 89 FC 55 48 89 F5 53 E8 D1 FA FF FF 48 ATI..UH..S.....H
000000000040B1B0 89 C3 48 8B 00 48 85 C0 74 23 48 89 C6 EB 04 90 ..H..H..t#H.....
000000000040B1C0 48 8B 33 48 39 F5 74 23 48 89 EF 41 FF 54 24 38 H.3H9.t#H..A.T$8
000000000040B1D0 84 C0 75 14 48 8B 5B 08 48 85 DB 75 E3 5B 5D 31 ..u.H.[.H..u.[]1
000000000040B1E0 C0 41 5C C3 0F 1F 40 00 48 8B 33 5B 5D 48 89 F0 .A\...@.H.3[]H..
000000000040B1F0 41 5C C3 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 A\.ffff.........
000000000040B200 48 83 EC 08 48 83 7F 20 00 74 2B 48 8B 17 48 8B H...H.. .t+H..H.
000000000040B210 4F 08 48 39 CA 72 12 EB 24 0F 1F 80 00 00 00 00 O.H9.r..$.......
000000000040B220 48 83 C2 10 48 39 CA 73 14 48 8B 02 48 85 C0 74 H...H9.s.H..H..t
000000000040B230 EF 48 83 C4 08 C3 31 C0 48 83 C4 08 C3 E8 DE 6F .H....1.H......o
000000000040B240 FF FF 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 ..fffff.........
000000000040B250 55 48 89 FD 53 48 89 F3 48 83 EC 08 E8 1F FA FF UH..SH..H.......
000000000040B260 FF 48 89 C1 48 89 C2 EB 10 0F 1F 80 00 00 00 00 .H..H...........
000000000040B270 48 8B 52 08 48 85 D2 74 0E 48 39 1A 75 F2 48 8B H.R.H..t.H9.u.H.
000000000040B280 42 08 48 85 C0 75 23 48 8B 55 08 EB 0B 0F 1F 00 B.H..u#H.U......
000000000040B290 48 8B 01 48 85 C0 75 0B 48 83 C1 10 48 39 D1 72 H..H..u.H...H9.r
000000000040B2A0 EF 31 C0 48 83 C4 08 5B 5D C3 48 8B 00 48 83 C4 .1.H...[].H..H..
000000000040B2B0 08 5B 5D C3 66 66 66 2E 0F 1F 84 00 00 00 00 00 .[].fff.........
000000000040B2C0 4C 8B 0F 31 C0 4C 39 4F 08 76 57 49 8B 09 48 85 L..1.L9O.vWI..H.
000000000040B2D0 C9 74 37 48 39 C2 76 48 48 89 0C C6 49 8B 49 08 .t7H9.vHH...I.I.
000000000040B2E0 4C 8D 40 01 4C 89 C0 48 85 C9 74 1E 0F 1F 40 00 L.@.L..H..t...@.
000000000040B2F0 48 39 D0 74 2B 4C 8B 01 48 83 C0 01 4C 89 44 C6 H9.t+L..H...L.D.
000000000040B300 F8 48 8B 49 08 48 85 C9 75 E6 49 83 C1 10 4C 39 .H.I.H..u.I...L9
000000000040B310 4F 08 77 B7 F3 C3 66 2E 0F 1F 84 00 00 00 00 00 O.w...f.........
000000000040B320 F3 C3 F3 C3 66 66 66 2E 0F 1F 84 00 00 00 00 00 ....fff.........
000000000040B330 41 57 49 89 FF 41 56 41 55 41 54 55 53 48 83 EC AWI..AVAUATUSH..
000000000040B340 08 4C 8B 37 4C 39 77 08 76 50 49 89 F4 49 89 D5 .L.7L9w.vPI..I..
000000000040B350 31 ED 49 8B 3E 48 85 FF 74 20 4C 89 F3 EB 04 90 1.I.>H..t L.....
000000000040B360 48 8B 3B 4C 89 EE 41 FF D4 84 C0 74 1B 48 8B 5B H.;L..A....t.H.[
000000000040B370 08 48 83 C5 01 48 85 DB 75 E6 49 83 C6 10 4D 39 .H...H..u.I...M9
000000000040B380 77 08 77 CE 0F 1F 40 00 48 83 C4 08 48 89 E8 5B w.w...@.H...H..[
000000000040B390 5D 41 5C 41 5D 41 5E 41 5F C3 31 ED EB EA 66 90 ]A\A]A^A_.1...f.
000000000040B3A0 44 0F B6 07 31 D2 45 84 C0 74 25 0F 1F 44 00 00 D...1.E..t%..D..
000000000040B3B0 48 89 D1 48 83 C7 01 48 C1 E1 05 48 29 D1 31 D2 H..H...H...H).1.
000000000040B3C0 49 8D 04 08 44 0F B6 07 48 F7 F6 45 84 C0 75 E0 I...D...H..E..u.
000000000040B3D0 48 89 D0 C3 66 66 66 2E 0F 1F 84 00 00 00 00 00 H...fff.........
000000000040B3E0 C7 07 00 00 00 00 C7 47 04 00 00 80 3F C7 47 08 .......G....?.G.
000000000040B3F0 CD CC 4C 3F C7 47 0C F4 FD B4 3F C6 47 10 00 C3 ..L?.G....?.G...

;; fn000000000040B400: 000000000040B400
;;   Called from:
;;     0000000000403A06 (in fn00000000004028C0)
fn000000000040B400 proc
	push	r15
	mov	eax,40AC60h
	mov	r15,rdi
	mov	edi,50h
	push	r14
	mov	r14,r8
	push	r13
	mov	r13,rdx
	push	r12
	mov	r12,rcx
	push	rbp
	mov	rbp,rsi
	push	rbx
	sub	rsp,8h
	test	rdx,rdx
	cmovz	r13,rax

l000000000040B42E:
	test	rcx,rcx
	mov	eax,40AC70h
	cmovz	r12,rax

l000000000040B43A:
	call	402640h
	test	rax,rax
	mov	rbx,rax
	jz	40B598h

l000000000040B44B:
	test	rbp,rbp
	mov	eax,415EE0h
	lea	rdi,[rbx+28h]
	cmovz	rbp,rax

l000000000040B45B:
	mov	[rbx+28h],rbp
	call	40ADB0h
	test	al,al
	jz	40B548h

l000000000040B46C:
	cmp	byte ptr [rbp+10h],0h
	movss	xmm1,dword ptr [rbp+8h]
	jnz	40B4C0h

l000000000040B477:
	test	r15,r15
	js	40B578h

l000000000040B480:
	cvtsi2ss	xmm0,r15

l000000000040B485:
	divss	xmm0,xmm1
	ucomiss	xmm0,[0000000000415F08]                            ; [rip+0000AA78]
	jnc	40B540h

l000000000040B496:
	ucomiss	xmm0,[0000000000415F0C]                            ; [rip+0000AA6F]
	jc	40B568h

l000000000040B4A3:
	subss	xmm0,[0000000000415F0C]                              ; [rip+0000AA61]
	mov	rax,8000000000000000h
	cvttss2si	r15,xmm0
	xor	r15,rax
	nop	dword ptr [rax]

l000000000040B4C0:
	mov	rdi,r15
	call	40ABC0h
	mov	rbp,rax
	mov	rax,1FFFFFFFFFFFFFFFh
	cmp	rbp,rax
	ja	40B540h

l000000000040B4DA:
	test	rbp,rbp
	mov	[rbx+10h],rbp
	jz	40B548h

l000000000040B4E3:
	mov	esi,10h
	mov	rdi,rbp
	call	402530h
	test	rax,rax
	mov	[rbx],rax
	jz	40B548h

l000000000040B4F8:
	shl	rbp,4h
	mov	qword ptr [rbx+18h],+0h
	mov	qword ptr [rbx+20h],+0h
	add	rax,rbp
	mov	[rbx+30h],r13
	mov	[rbx+38h],r12
	mov	[rbx+8h],rax
	mov	[rbx+40h],r14
	mov	rax,rbx
	mov	qword ptr [rbx+48h],+0h

l000000000040B52A:
	add	rsp,8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040B539                            0F 1F 80 00 00 00 00          .......

l000000000040B540:
	mov	qword ptr [rbx+10h],+0h

l000000000040B548:
	mov	rdi,rbx
	call	4021F0h
	add	rsp,8h
	xor	eax,eax
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040B561    0F 1F 80 00 00 00 00                          .......        

l000000000040B568:
	cvttss2si	r15,xmm0
	jmp	40B4C0h
000000000040B572       66 0F 1F 44 00 00                           f..D..        

l000000000040B578:
	mov	rax,r15
	and	r15d,1h
	shr	rax,1h
	or	rax,r15
	cvtsi2ss	xmm0,rax
	addss	xmm0,xmm0
	jmp	40B485h
000000000040B593          0F 1F 44 00 00                            ..D..        

l000000000040B598:
	xor	eax,eax
	jmp	40B52Ah
000000000040B59C                                     0F 1F 40 00             ..@.
000000000040B5A0 41 54 55 48 89 FD 53 4C 8B 27 4C 3B 67 08 73 73 ATUH..SL.'L;g.ss
000000000040B5B0 49 83 3C 24 00 74 62 49 8B 5C 24 08 48 8B 55 40 I.<$.tbI.\$.H.U@
000000000040B5C0 48 85 DB 75 0E EB 36 66 0F 1F 84 00 00 00 00 00 H..u..6f........
000000000040B5D0 48 89 C3 48 85 D2 74 09 48 8B 3B FF D2 48 8B 55 H..H..t.H.;..H.U
000000000040B5E0 40 48 8B 43 08 48 8B 4D 48 48 C7 03 00 00 00 00 @H.C.H.MHH......
000000000040B5F0 48 85 C0 48 89 4B 08 48 89 5D 48 75 D3 48 85 D2 H..H.K.H.]Hu.H..
000000000040B600 74 06 49 8B 3C 24 FF D2 49 C7 04 24 00 00 00 00 t.I.<$..I..$....
000000000040B610 49 C7 44 24 08 00 00 00 00 49 83 C4 10 4C 39 65 I.D$.....I...L9e
000000000040B620 08 77 8D 48 C7 45 18 00 00 00 00 48 C7 45 20 00 .w.H.E.....H.E .
000000000040B630 00 00 00 5B 5D 41 5C C3 0F 1F 84 00 00 00 00 00 ...[]A\.........

;; fn000000000040B640: 000000000040B640
;;   Called from:
;;     00000000004041F0 (in fn00000000004028C0)
fn000000000040B640 proc
	push	r12
	push	rbp
	mov	rbp,rdi
	push	rbx
	cmp	qword ptr [rdi+40h],0h
	jz	40B655h

l000000000040B64E:
	cmp	qword ptr [rdi+20h],0h
	jnz	40B6C6h

l000000000040B655:
	mov	rax,[rbp+8h]

l000000000040B659:
	mov	r12,[rbp+0h]
	cmp	r12,rax
	jnc	40B693h

l000000000040B662:
	nop	word ptr [rax+rax+0h]

l000000000040B668:
	mov	rdi,[r12+8h]
	test	rdi,rdi
	jnz	40B67Bh

l000000000040B672:
	jmp	40B689h
000000000040B674             0F 1F 40 00                             ..@.        

l000000000040B678:
	mov	rdi,rbx

l000000000040B67B:
	mov	rbx,[rdi+8h]
	call	4021F0h
	test	rbx,rbx
	jnz	40B678h

l000000000040B689:
	add	r12,10h
	cmp	[rbp+8h],r12
	ja	40B668h

l000000000040B693:
	mov	rdi,[rbp+48h]
	test	rdi,rdi
	jnz	40B6A3h

l000000000040B69C:
	jmp	40B6B1h
000000000040B69E                                           66 90               f.

l000000000040B6A0:
	mov	rdi,rbx

l000000000040B6A3:
	mov	rbx,[rdi+8h]
	call	4021F0h
	test	rbx,rbx
	jnz	40B6A0h

l000000000040B6B1:
	mov	rdi,[rbp+0h]
	call	4021F0h
	pop	rbx
	mov	rdi,rbp
	pop	rbp
	pop	r12
	jmp	4021F0h

l000000000040B6C6:
	mov	r12,[rdi]
	cmp	r12,[rdi+8h]
	jnc	40B693h

l000000000040B6CF:
	nop

l000000000040B6D0:
	mov	rdi,[r12]
	mov	rbx,r12
	test	rdi,rdi
	jnz	40B6E3h

l000000000040B6DC:
	jmp	40B6EFh
000000000040B6DE                                           66 90               f.

l000000000040B6E0:
	mov	rdi,[rbx]

l000000000040B6E3:
	call	qword ptr [rbp+40h]
	mov	rbx,[rbx+8h]
	test	rbx,rbx
	jnz	40B6E0h

l000000000040B6EF:
	mov	rax,[rbp+8h]
	add	r12,10h
	cmp	rax,r12
	ja	40B6D0h

l000000000040B6FC:
	jmp	40B659h
000000000040B701    66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00  ffffff.........

;; fn000000000040B710: 000000000040B710
;;   Called from:
;;     000000000040BA3F (in fn000000000040B8F0)
;;     000000000040BC7B (in fn000000000040BB90)
fn000000000040B710 proc
	push	r12
	push	rbp
	push	rbx
	mov	rbx,rdi
	sub	rsp,50h
	mov	rbp,[rdi+28h]
	cmp	byte ptr [rbp+10h],0h
	movss	xmm1,dword ptr [rbp+8h]
	jnz	40B770h

l000000000040B72A:
	test	rsi,rsi
	js	40B8D0h

l000000000040B733:
	cvtsi2ss	xmm0,rsi

l000000000040B738:
	divss	xmm0,xmm1
	ucomiss	xmm0,[0000000000415F08]                            ; [rip+0000A7C5]
	jnc	40B870h

l000000000040B749:
	ucomiss	xmm0,[0000000000415F0C]                            ; [rip+0000A7BC]
	jc	40B880h

l000000000040B756:
	subss	xmm0,[0000000000415F0C]                              ; [rip+0000A7AE]
	mov	rax,8000000000000000h
	cvttss2si	rsi,xmm0
	xor	rsi,rax

l000000000040B770:
	mov	rdi,rsi
	call	40ABC0h
	lea	rdx,[rax-1h]
	mov	r12,rax
	mov	rax,1FFFFFFFFFFFFFFEh
	cmp	rdx,rax
	ja	40B870h

l000000000040B792:
	cmp	[rbx+10h],r12
	jz	40B860h

l000000000040B79C:
	mov	esi,10h
	mov	rdi,r12
	call	402530h
	test	rax,rax
	mov	[rsp],rax
	jz	40B870h

l000000000040B7B6:
	mov	[rsp+10h],r12
	shl	r12,4h
	xor	edx,edx
	add	rax,r12
	mov	rsi,rbx
	mov	rdi,rsp
	mov	[rsp+8h],rax
	mov	rax,[rbx+30h]
	mov	[rsp+28h],rbp
	mov	qword ptr [rsp+18h],+0h
	mov	qword ptr [rsp+20h],+0h
	mov	[rsp+30h],rax
	mov	rax,[rbx+38h]
	mov	[rsp+38h],rax
	mov	rax,[rbx+40h]
	mov	[rsp+40h],rax
	mov	rax,[rbx+48h]
	mov	[rsp+48h],rax
	call	40AE40h
	test	al,al
	mov	ebp,eax
	jnz	40B890h

l000000000040B815:
	mov	rax,[rsp+48h]
	mov	edx,1h
	mov	rsi,rsp
	mov	rdi,rbx
	mov	[rbx+48h],rax
	call	40AE40h
	test	al,al
	jz	40B8EAh

l000000000040B836:
	xor	edx,edx
	mov	rsi,rsp
	mov	rdi,rbx
	call	40AE40h
	test	al,al
	jz	40B8EAh

l000000000040B84B:
	mov	rdi,[rsp]
	call	4021F0h
	add	rsp,50h
	mov	eax,ebp
	pop	rbx
	pop	rbp
	pop	r12
	ret
000000000040B85F                                              90                .

l000000000040B860:
	add	rsp,50h
	mov	ebp,1h
	pop	rbx
	mov	eax,ebp
	pop	rbp
	pop	r12
	ret

l000000000040B870:
	add	rsp,50h
	xor	ebp,ebp
	pop	rbx
	mov	eax,ebp
	pop	rbp
	pop	r12
	ret
000000000040B87D                                        0F 1F 00              ...

l000000000040B880:
	cvttss2si	rsi,xmm0
	jmp	40B770h
000000000040B88A                               66 0F 1F 44 00 00           f..D..

l000000000040B890:
	mov	rdi,[rbx]
	call	4021F0h
	mov	rax,[rsp]
	mov	[rbx],rax
	mov	rax,[rsp+8h]
	mov	[rbx+8h],rax
	mov	rax,[rsp+10h]
	mov	[rbx+10h],rax
	mov	rax,[rsp+18h]
	mov	[rbx+18h],rax
	mov	rax,[rsp+48h]
	mov	[rbx+48h],rax
	add	rsp,50h
	mov	eax,ebp
	pop	rbx
	pop	rbp
	pop	r12
	ret
000000000040B8CE                                           66 90               f.

l000000000040B8D0:
	mov	rax,rsi
	and	esi,1h
	shr	rax,1h
	or	rax,rsi
	cvtsi2ss	xmm0,rax
	addss	xmm0,xmm0
	jmp	40B738h

l000000000040B8EA:
	call	402220h
000000000040B8EF                                              90                .

;; fn000000000040B8F0: 000000000040B8F0
;;   Called from:
;;     000000000040BB5D (in fn000000000040BB50)
fn000000000040B8F0 proc
	push	r12
	push	rbp
	mov	rbp,rsi
	push	rbx
	sub	rsp,10h
	test	rsi,rsi
	jz	40BA67h

l000000000040B904:
	mov	r12,rdx
	lea	rdx,[rsp+8h]
	xor	ecx,ecx
	mov	rbx,rdi
	call	40ACB0h
	test	rax,rax
	jz	40B938h

l000000000040B91B:
	test	r12,r12
	jz	40B9B8h

l000000000040B924:
	mov	[r12],rax
	xor	eax,eax

l000000000040B92A:
	add	rsp,10h
	pop	rbx
	pop	rbp
	pop	r12
	ret
000000000040B933          0F 1F 44 00 00                            ..D..        

l000000000040B938:
	mov	rax,[rbx+18h]
	test	rax,rax
	js	40BA70h

l000000000040B945:
	cvtsi2ss	xmm0,rax

l000000000040B94A:
	mov	rax,[rbx+10h]
	mov	rdx,[rbx+28h]
	test	rax,rax
	js	40BA90h

l000000000040B95B:
	cvtsi2ss	xmm1,rax

l000000000040B960:
	mulss	xmm1,dword ptr [rdx+8h]
	ucomiss	xmm0,xmm1
	ja	40B9C8h

l000000000040B96A:
	mov	r12,[rsp+8h]
	cmp	qword ptr [r12],0h
	jz	40BAB0h

l000000000040B97A:
	mov	rax,[rbx+48h]
	test	rax,rax
	jz	40BB1Bh

l000000000040B987:
	mov	rdx,[rax+8h]
	mov	[rbx+48h],rdx

l000000000040B98F:
	mov	rdx,[r12+8h]
	mov	[rax],rbp
	mov	[rax+8h],rdx
	mov	[r12+8h],rax
	mov	eax,1h
	add	qword ptr [rbx+20h],1h
	add	rsp,10h
	pop	rbx
	pop	rbp
	pop	r12
	ret
000000000040B9B3          0F 1F 44 00 00                            ..D..        

l000000000040B9B8:
	add	rsp,10h
	xor	eax,eax
	pop	rbx
	pop	rbp
	pop	r12
	ret
000000000040B9C3          0F 1F 44 00 00                            ..D..        

l000000000040B9C8:
	lea	rdi,[rbx+28h]
	call	40ADB0h
	mov	rax,[rbx+10h]
	mov	rdx,[rbx+28h]
	test	rax,rax
	movss	xmm2,dword ptr [rdx+8h]
	js	40BAE7h

l000000000040B9E7:
	cvtsi2ss	xmm0,rax

l000000000040B9EC:
	mov	rax,[rbx+18h]
	test	rax,rax
	js	40BB01h

l000000000040B9F9:
	cvtsi2ss	xmm1,rax

l000000000040B9FE:
	movaps	xmm3,xmm2
	mulss	xmm3,xmm0
	ucomiss	xmm1,xmm3
	jbe	40B96Ah

l000000000040BA0E:
	cmp	byte ptr [rdx+10h],0h
	mulss	xmm0,dword ptr [rdx+0Ch]
	jnz	40BA1Dh

l000000000040BA19:
	mulss	xmm0,xmm2

l000000000040BA1D:
	ucomiss	xmm0,[0000000000415F08]                            ; [rip+0000A4E4]
	jnc	40BB2Eh

l000000000040BA2A:
	ucomiss	xmm0,[0000000000415F0C]                            ; [rip+0000A4DB]
	jnc	40BAC8h

l000000000040BA37:
	cvttss2si	rsi,xmm0

l000000000040BA3C:
	mov	rdi,rbx
	call	40B710h
	test	al,al
	jz	40BB2Eh

l000000000040BA4C:
	lea	rdx,[rsp+8h]
	xor	ecx,ecx
	mov	rsi,rbp
	mov	rdi,rbx
	call	40ACB0h
	test	rax,rax
	jz	40B96Ah

l000000000040BA67:
	call	402220h
000000000040BA6C                                     0F 1F 40 00             ..@.

l000000000040BA70:
	mov	rdx,rax
	and	eax,1h
	shr	rdx,1h
	or	rdx,rax
	cvtsi2ss	xmm0,rdx
	addss	xmm0,xmm0
	jmp	40B94Ah
000000000040BA8A                               66 0F 1F 44 00 00           f..D..

l000000000040BA90:
	mov	rcx,rax
	and	eax,1h
	shr	rcx,1h
	or	rcx,rax
	cvtsi2ss	xmm1,rcx
	addss	xmm1,xmm1
	jmp	40B960h
000000000040BAAA                               66 0F 1F 44 00 00           f..D..

l000000000040BAB0:
	mov	[r12],rbp
	mov	eax,1h
	add	qword ptr [rbx+20h],1h
	add	qword ptr [rbx+18h],1h
	jmp	40B92Ah

l000000000040BAC8:
	subss	xmm0,[0000000000415F0C]                              ; [rip+0000A43C]
	mov	rax,8000000000000000h
	cvttss2si	rsi,xmm0
	xor	rsi,rax
	jmp	40BA3Ch

l000000000040BAE7:
	mov	rcx,rax
	and	eax,1h
	shr	rcx,1h
	or	rcx,rax
	cvtsi2ss	xmm0,rcx
	addss	xmm0,xmm0
	jmp	40B9ECh

l000000000040BB01:
	mov	rcx,rax
	and	eax,1h
	shr	rcx,1h
	or	rcx,rax
	cvtsi2ss	xmm1,rcx
	addss	xmm1,xmm1
	jmp	40B9FEh

l000000000040BB1B:
	mov	edi,10h
	call	402640h
	test	rax,rax
	jnz	40B98Fh

l000000000040BB2E:
	mov	eax,0FFFFFFFFh
	jmp	40B92Ah
000000000040BB38                         0F 1F 84 00 00 00 00 00         ........
000000000040BB40 E9 AB FD FF FF 66 66 2E 0F 1F 84 00 00 00 00 00 .....ff.........

;; fn000000000040BB50: 000000000040BB50
;;   Called from:
;;     00000000004035B1 (in fn00000000004028C0)
fn000000000040BB50 proc
	push	rbx
	mov	rbx,rsi
	sub	rsp,10h
	lea	rdx,[rsp+8h]
	call	40B8F0h
	cmp	eax,0FFh
	jz	40BB80h

l000000000040BB67:
	test	eax,eax
	mov	rax,rbx
	cmovz	rax,[rsp+8h]

l000000000040BB72:
	add	rsp,10h
	pop	rbx
	ret
000000000040BB78                         0F 1F 84 00 00 00 00 00         ........

l000000000040BB80:
	xor	eax,eax
	jmp	40BB72h
000000000040BB84             66 66 66 2E 0F 1F 84 00 00 00 00 00     fff.........

;; fn000000000040BB90: 000000000040BB90
;;   Called from:
;;     000000000040402B (in fn00000000004028C0)
fn000000000040BB90 proc
	push	r12
	mov	ecx,1h
	push	rbp
	push	rbx
	mov	rbx,rdi
	sub	rsp,10h
	lea	rdx,[rsp+8h]
	call	40ACB0h
	test	rax,rax
	mov	rbp,rax
	jz	40BD08h

l000000000040BBB6:
	mov	rdx,[rsp+8h]
	sub	qword ptr [rbx+20h],1h
	cmp	qword ptr [rdx],0h
	jz	40BBD0h

l000000000040BBC6:
	add	rsp,10h
	pop	rbx
	pop	rbp
	pop	r12
	ret
000000000040BBCF                                              90                .

l000000000040BBD0:
	mov	rax,[rbx+18h]
	sub	rax,1h
	test	rax,rax
	mov	[rbx+18h],rax
	js	40BCE8h

l000000000040BBE5:
	cvtsi2ss	xmm0,rax

l000000000040BBEA:
	mov	rax,[rbx+10h]
	mov	rdx,[rbx+28h]
	test	rax,rax
	js	40BCC8h

l000000000040BBFB:
	cvtsi2ss	xmm1,rax

l000000000040BC00:
	mulss	xmm1,dword ptr [rdx]
	ucomiss	xmm1,xmm0
	ja	40BC18h

l000000000040BC09:
	add	rsp,10h
	mov	rax,rbp
	pop	rbx
	pop	rbp
	pop	r12
	ret
000000000040BC15                0F 1F 00                              ...        

l000000000040BC18:
	lea	rdi,[rbx+28h]
	call	40ADB0h
	mov	rdx,[rbx+10h]
	mov	rax,[rbx+28h]
	test	rdx,rdx
	js	40BD2Fh

l000000000040BC32:
	cvtsi2ss	xmm0,rdx

l000000000040BC37:
	mov	rdx,[rbx+18h]
	test	rdx,rdx
	js	40BD49h

l000000000040BC44:
	cvtsi2ss	xmm1,rdx

l000000000040BC49:
	movss	xmm2,dword ptr [rax]
	mulss	xmm2,xmm0
	ucomiss	xmm2,xmm1
	jbe	40BC09h

l000000000040BC56:
	cmp	byte ptr [rax+10h],0h
	mulss	xmm0,dword ptr [rax+4h]
	jnz	40BC66h

l000000000040BC61:
	mulss	xmm0,dword ptr [rax+8h]

l000000000040BC66:
	ucomiss	xmm0,[0000000000415F0C]                            ; [rip+0000A29F]
	jnc	40BD10h

l000000000040BC73:
	cvttss2si	rsi,xmm0

l000000000040BC78:
	mov	rdi,rbx
	call	40B710h
	mov	edx,eax
	mov	rax,rbp
	test	dl,dl
	jnz	40BBC6h

l000000000040BC8D:
	mov	rdi,[rbx+48h]
	test	rdi,rdi
	jnz	40BCA3h

l000000000040BC96:
	jmp	40BCB1h
000000000040BC98                         0F 1F 84 00 00 00 00 00         ........

l000000000040BCA0:
	mov	rdi,r12

l000000000040BCA3:
	mov	r12,[rdi+8h]
	call	4021F0h
	test	r12,r12
	jnz	40BCA0h

l000000000040BCB1:
	mov	qword ptr [rbx+48h],+0h
	mov	rax,rbp
	jmp	40BBC6h
000000000040BCC1    0F 1F 80 00 00 00 00                          .......        

l000000000040BCC8:
	mov	rcx,rax
	and	eax,1h
	shr	rcx,1h
	or	rcx,rax
	cvtsi2ss	xmm1,rcx
	addss	xmm1,xmm1
	jmp	40BC00h
000000000040BCE2       66 0F 1F 44 00 00                           f..D..        

l000000000040BCE8:
	mov	rdx,rax
	and	eax,1h
	shr	rdx,1h
	or	rdx,rax
	cvtsi2ss	xmm0,rdx
	addss	xmm0,xmm0
	jmp	40BBEAh
000000000040BD02       66 0F 1F 44 00 00                           f..D..        

l000000000040BD08:
	xor	eax,eax
	jmp	40BBC6h
000000000040BD0F                                              90                .

l000000000040BD10:
	subss	xmm0,[0000000000415F0C]                              ; [rip+0000A1F4]
	mov	rax,8000000000000000h
	cvttss2si	rsi,xmm0
	xor	rsi,rax
	jmp	40BC78h

l000000000040BD2F:
	mov	rcx,rdx
	and	edx,1h
	shr	rcx,1h
	or	rcx,rdx
	cvtsi2ss	xmm0,rcx
	addss	xmm0,xmm0
	jmp	40BC37h

l000000000040BD49:
	mov	rcx,rdx
	and	edx,1h
	shr	rcx,1h
	or	rcx,rdx
	cvtsi2ss	xmm1,rcx
	addss	xmm1,xmm1
	jmp	40BC49h
000000000040BD63          66 2E 0F 1F 84 00 00 00 00 00 0F 1F 00    f............

;; fn000000000040BD70: 000000000040BD70
;;   Called from:
;;     0000000000403DB2 (in fn00000000004028C0)
;;     0000000000405E85 (in fn0000000000405D50)
;;     0000000000406D93 (in fn0000000000406B70)
;;     00000000004072A3 (in fn0000000000406B70)
;;     00000000004079D9 (in fn0000000000407870)
;;     0000000000408151 (in fn0000000000407EA0)
;;     0000000000408A4D (in fn0000000000407EA0)
fn000000000040BD70 proc
	push	r15
	mov	eax,edx
	and	eax,3h
	push	r14
	mov	r14,rsi
	push	r13
	push	r12
	mov	r12,rdi
	push	rbp
	push	rbx
	mov	rbx,rcx
	sub	rsp,+0B8h
	mov	[rsp+38h],eax
	mov	eax,edx
	mov	[rsp+30h],rsi
	and	eax,20h
	mov	[rsp+20h],edx
	mov	[rsp+28h],r8
	mov	rcx,fs:[0028h]
	mov	[rsp+0A8h],rcx
	xor	ecx,ecx
	cmp	eax,1h
	mov	[rsp+58h],eax
	sbb	eax,eax
	mov	[rsp+24h],eax
	and	dword ptr [rsp+24h],0E8h
	add	dword ptr [rsp+24h],400h
	call	4022C0h
	mov	r15,[rax]
	mov	r13,rax
	mov	rdi,r15
	call	402380h
	mov	rbp,[r13+10h]
	mov	r11,rax
	mov	r13,[r13+8h]
	lea	rax,[rax-1h]
	mov	edx,1h
	cmp	rax,10h
	mov	rdi,r13
	mov	eax,413990h
	cmovnc	r11,rdx

l000000000040BE0A:
	cmovnc	r15,rax

l000000000040BE0E:
	mov	[rsp+50h],r11
	call	402380h
	cmp	rax,11h
	mov	eax,416919h
	mov	r11,[rsp+50h]
	cmovnc	r13,rax

l000000000040BE2A:
	mov	rax,r14
	add	rax,+288h
	cmp	[rsp+28h],rbx
	mov	[rsp+18h],rax
	ja	40C0E0h

l000000000040BE43:
	xor	edx,edx
	mov	rax,rbx
	div	qword ptr [rsp+28h]
	test	rdx,rdx
	mov	rcx,rax
	jz	40C2F0h

l000000000040BE59:
	mov	[rsp+68h],r12
	test	r12,r12
	fild	qword ptr [rsp+68h]
	js	40C5F0h

l000000000040BE6B:
	mov	[rsp+68h],rbx
	test	rbx,rbx
	fild	qword ptr [rsp+68h]
	js	40C608h

l000000000040BE7D:
	mov	rax,[rsp+28h]
	mov	[rsp+68h],rax
	test	rax,rax
	fild	qword ptr [rsp+68h]
	js	40C5E0h

l000000000040BE94:
	test	byte ptr [rsp+20h],10h
	fdivp	st(1),st(0)
	fmulp	st(1),st(0)
	jz	40C1B8h

l000000000040BEA3:
	fild	dword ptr [rsp+24h]
	xor	ebx,ebx
	fld	st(0)
	jmp	40BEB4h
000000000040BEAD                                        0F 1F 00              ...

l000000000040BEB0:
	fstp	st(1)
	fxch	st(0),st(2)

l000000000040BEB4:
	fld	st(0)
	add	ebx,1h
	fmul	st(0),st(2)
	fxch	st(0),st(3)
	fucomi	st(0),st(3)
	jc	40BED0h

l000000000040BEC1:
	cmp	ebx,8h
	jnz	40BEB0h

l000000000040BEC6:
	fstp	st(2)
	fstp	st(2)
	jmp	40BED4h
000000000040BECC                                     0F 1F 40 00             ..@.

l000000000040BED0:
	fstp	st(2)
	fstp	st(2)

l000000000040BED4:
	fdivrp	st(1),st(0)
	cmp	dword ptr [rsp+38h],1h
	fld	st(0)
	jz	40BF70h

l000000000040BEE3:
	fstp	st(0)
	fld	[0000000000415F90]                                     ; [rip+0000A0A5]
	fucomip	st(0),st(1)
	jbe	40C498h

l000000000040BEF3:
	fld	[0000000000415F0C]                                     ; [rip+0000A013]
	fxch	st(0),st(1)
	fucomi	st(0),st(1)
	jnc	40C660h

l000000000040BF03:
	fstp	st(1)
	fstcw	word ptr [rsp+66h]
	movzx	eax,word ptr [rsp+66h]
	or	ah,0Ch
	mov	[rsp+64h],ax
	fld	st(0)
	fldcw	word ptr [rsp+64h]
	fistp	qword ptr [rsp+68h]
	fldcw	word ptr [rsp+66h]
	mov	rax,[rsp+68h]

l000000000040BF29:
	mov	ecx,[rsp+38h]
	xor	edx,edx
	test	ecx,ecx
	jnz	40BF55h

l000000000040BF33:
	mov	[rsp+68h],rax
	test	rax,rax
	fild	qword ptr [rsp+68h]
	js	40C77Dh

l000000000040BF45:
	xor	ecx,ecx
	mov	edx,1h
	fucomip	st(0),st(1)
	setpe	cl
	cmovz	rdx,rcx

l000000000040BF55:
	add	rax,rdx
	mov	[rsp+68h],rax
	test	rax,rax
	fild	qword ptr [rsp+68h]
	js	40C758h

l000000000040BF6A:
	fxch	st(0),st(1)
	jmp	40BF72h
000000000040BF6E                                           66 90               f.

l000000000040BF70:
	fxch	st(0),st(1)

l000000000040BF72:
	mov	r14,[rsp+30h]
	mov	rdx,-1h
	mov	ecx,415F1Eh
	mov	esi,1h
	xor	eax,eax
	mov	[rsp+50h],r11
	mov	rdi,r14
	fstp	tword ptr [rsp+40h]
	fstp	tword ptr [rsp]
	call	402890h
	mov	rdi,r14
	call	402380h
	mov	r11,[rsp+50h]
	mov	edx,[rsp+58h]
	mov	r15,rax
	xor	eax,eax
	fld	tword ptr [rsp+40h]
	test	edx,edx
	lea	r14,[r11+1h]
	setz	al
	lea	rax,[r14+rax+1h]
	cmp	r15,rax
	ja	40BFF0h

l000000000040BFCB:
	test	byte ptr [rsp+20h],8h
	jz	40C208h

l000000000040BFD6:
	mov	rax,[rsp+30h]
	cmp	byte ptr [rax+r15-1h],30h
	jnz	40C210h

l000000000040BFE7:
	nop	word ptr [rax+rax+0h]

l000000000040BFF0:
	cmp	dword ptr [rsp+38h],1h
	fmul	[0000000000415F84]                                    ; [rip+00009F89]
	jz	40C0A0h

l000000000040C001:
	fld	[0000000000415F90]                                     ; [rip+00009F89]
	fucomip	st(0),st(1)
	jbe	40C0A0h

l000000000040C00F:
	fld	[0000000000415F0C]                                     ; [rip+00009EF7]
	fxch	st(0),st(1)
	fucomi	st(0),st(1)
	jnc	40C720h

l000000000040C01F:
	fstp	st(1)
	fstcw	word ptr [rsp+66h]
	movzx	eax,word ptr [rsp+66h]
	or	ah,0Ch
	mov	[rsp+64h],ax
	fld	st(0)
	fldcw	word ptr [rsp+64h]
	fistp	qword ptr [rsp+68h]
	fldcw	word ptr [rsp+66h]
	mov	rax,[rsp+68h]

l000000000040C045:
	mov	r14d,[rsp+38h]
	xor	edx,edx
	test	r14d,r14d
	jnz	40C080h

l000000000040C051:
	mov	[rsp+68h],rax
	test	rax,rax
	fild	qword ptr [rsp+68h]
	js	40C79Eh

l000000000040C063:
	xor	ecx,ecx
	mov	edx,1h
	fucomip	st(0),st(1)
	fstp	st(0)
	setpe	cl
	cmovz	rdx,rcx

l000000000040C075:
	jmp	40C082h
000000000040C077                      66 0F 1F 84 00 00 00 00 00        f........

l000000000040C080:
	fstp	st(0)

l000000000040C082:
	add	rax,rdx
	mov	[rsp+68h],rax
	test	rax,rax
	fild	qword ptr [rsp+68h]
	jns	40C0A0h

l000000000040C093:
	fadd	[0000000000415F08]                                    ; [rip+00009E6F]
	nop	dword ptr [rax+0h]

l000000000040C0A0:
	fdiv	[0000000000415F84]                                    ; [rip+00009EDE]
	mov	r14,[rsp+30h]
	mov	ecx,415F18h
	mov	rdx,-1h
	mov	esi,1h
	xor	eax,eax
	mov	rdi,r14
	fstp	tword ptr [rsp]
	call	402890h
	mov	rdi,r14
	xor	r14d,r14d
	call	402380h
	mov	r15,rax
	jmp	40C218h
000000000040C0DC                                     0F 1F 40 00             ..@.

l000000000040C0E0:
	test	rbx,rbx
	jz	40BE59h

l000000000040C0E9:
	mov	rax,[rsp+28h]
	xor	edx,edx
	div	rbx
	test	rdx,rdx
	mov	rsi,rax
	jnz	40BE59h

l000000000040C0FF:
	xor	edx,edx
	mov	rax,r12
	div	rsi
	lea	rdi,[rdx+rdx*4]
	mov	r10,rax
	xor	edx,edx
	lea	rax,[rdi+rdi]
	div	rsi
	add	rdx,rdx
	mov	edi,eax
	cmp	rsi,rdx
	jbe	40C6E0h

l000000000040C125:
	xor	ecx,ecx
	test	rdx,rdx
	setnz	cl

l000000000040C12D:
	mov	r9d,[rsp+20h]
	mov	r8,[rsp+18h]
	mov	ebx,0FFFFFFFFh
	and	r9d,10h
	jz	40C530h

l000000000040C146:
	mov	esi,[rsp+24h]
	cmp	r10,rsi
	jc	40C710h

l000000000040C153:
	xor	ebx,ebx
	mov	r12d,[rsp+24h]
	jmp	40C17Dh
000000000040C15C                                     0F 1F 40 00             ..@.

l000000000040C160:
	test	ecx,ecx
	setnz	cl
	movzx	ecx,cl

l000000000040C168:
	add	ebx,1h
	cmp	rsi,r8
	ja	40C49Fh

l000000000040C174:
	cmp	ebx,8h
	jz	40C768h

l000000000040C17D:
	mov	rax,r10
	xor	edx,edx
	div	rsi
	mov	r8,rax
	lea	eax,[rdx+rdx*4]
	xor	edx,edx
	mov	r10,r8
	lea	eax,[rdi+rax*2]
	mov	edi,ecx
	sar	edi,1h
	div	r12d
	lea	r14d,[rdi+rdx*2]
	mov	edi,eax
	add	ecx,r14d
	cmp	r12d,r14d
	ja	40C160h

l000000000040C1A8:
	cmp	r12d,ecx
	sbb	ecx,ecx
	not	ecx
	add	ecx,3h
	jmp	40C168h
000000000040C1B4             0F 1F 40 00                             ..@.        

l000000000040C1B8:
	cmp	dword ptr [rsp+38h],1h
	jz	40C1CDh

l000000000040C1BF:
	fld	[0000000000415F90]                                     ; [rip+00009DCB]
	fucomip	st(0),st(1)
	ja	40C408h

l000000000040C1CD:
	fstp	tword ptr [rsp]
	mov	rbx,[rsp+30h]
	mov	ecx,415F18h
	mov	rdx,-1h
	mov	esi,1h
	xor	eax,eax
	xor	r14d,r14d
	mov	rdi,rbx
	call	402890h
	mov	rdi,rbx
	mov	ebx,0FFFFFFFFh
	call	402380h
	mov	r15,rax
	jmp	40C218h
000000000040C205                0F 1F 00                              ...        

l000000000040C208:
	fstp	st(0)
	jmp	40C218h
000000000040C20C                                     0F 1F 40 00             ..@.

l000000000040C210:
	fstp	st(0)
	nop	word ptr [rax+rax+0h]

l000000000040C218:
	mov	r12,[rsp+18h]
	mov	rsi,[rsp+30h]
	mov	rdx,r15
	sub	r12,r15
	sub	r15,r14
	mov	rdi,r12
	call	402760h
	lea	r8,[r12+r15]

l000000000040C237:
	test	byte ptr [rsp+20h],4h
	jnz	40C318h

l000000000040C242:
	test	byte ptr [rsp+20h],80h
	jz	40C2BAh

l000000000040C249:
	cmp	ebx,0FFh
	jz	40C618h

l000000000040C252:
	mov	eax,[rsp+20h]
	mov	ecx,ebx
	and	eax,100h
	or	ecx,eax
	jz	40C2BAh

l000000000040C261:
	test	byte ptr [rsp+20h],40h
	jnz	40C3E8h

l000000000040C26C:
	test	ebx,ebx
	jz	40C7EBh

l000000000040C274:
	mov	rcx,[rsp+18h]
	lea	rdx,[rcx+1h]
	mov	ecx,[rsp+58h]
	test	ecx,ecx
	jz	40C3D0h

l000000000040C289:
	movsxd	rcx,ebx
	movzx	ecx,byte ptr [rcx+415F78h]

l000000000040C293:
	mov	rdi,[rsp+18h]
	mov	[rdi],cl

l000000000040C29A:
	test	eax,eax
	jz	40C7F5h

l000000000040C2A2:
	mov	eax,[rsp+58h]
	test	eax,eax
	jnz	40C3B8h

l000000000040C2AE:
	lea	rax,[rdx+1h]
	mov	byte ptr [rdx],42h
	mov	[rsp+18h],rax

l000000000040C2BA:
	mov	rax,[rsp+18h]
	mov	rdi,[rsp+0A8h]
	xor	rdi,fs:[0028h]
	mov	byte ptr [rax],0h
	mov	rax,r12
	jnz	40C7E6h

l000000000040C2DC:
	add	rsp,+0B8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040C2EE                                           66 90               f.

l000000000040C2F0:
	mov	r10,rax
	xor	edx,edx
	imul	r10,r12
	mov	rax,r10
	div	rcx
	cmp	rax,r12
	jnz	40BE59h

l000000000040C308:
	xor	ecx,ecx
	xor	edi,edi
	jmp	40C12Dh
000000000040C311    0F 1F 80 00 00 00 00                          .......        

l000000000040C318:
	sub	r8,r12
	mov	rdi,r13
	mov	r15,-1h
	mov	r14,r8
	call	402380h
	lea	rdi,[rsp+70h]
	mov	rsi,r12
	mov	ecx,29h
	mov	rdx,r14
	mov	[rsp+38h],rax
	call	402590h
	lea	r9,[r12+r14]
	mov	r12,r13
	mov	r13,[rsp+38h]
	jmp	40C36Ch
000000000040C355                0F 1F 00                              ...        

l000000000040C358:
	sub	r9,r13
	mov	rdx,r13
	mov	rsi,r12
	mov	rdi,r9
	call	4025C0h
	mov	r9,rax

l000000000040C36C:
	movzx	edx,byte ptr [rbp+0h]
	test	dl,dl
	jz	40C383h

l000000000040C374:
	cmp	dl,7Eh
	movzx	r15d,dl
	cmova	r15,r14

l000000000040C37F:
	add	rbp,1h

l000000000040C383:
	cmp	r15,r14
	lea	rax,[rsp+70h]
	cmova	r15,r14

l000000000040C38F:
	sub	r14,r15
	sub	r9,r15
	mov	rdx,r15
	lea	rsi,[rax+r14]
	mov	rdi,r9
	call	4025C0h
	test	r14,r14
	mov	r9,rax
	jnz	40C358h

l000000000040C3AC:
	mov	r12,rax
	jmp	40C242h
000000000040C3B4             0F 1F 40 00                             ..@.        

l000000000040C3B8:
	test	ebx,ebx
	jz	40C2AEh

l000000000040C3C0:
	mov	byte ptr [rdx],69h
	add	rdx,1h
	jmp	40C2AEh
000000000040C3CC                                     0F 1F 40 00             ..@.

l000000000040C3D0:
	cmp	ebx,1h
	mov	ecx,6Bh
	jnz	40C289h

l000000000040C3DE:
	jmp	40C293h
000000000040C3E3          0F 1F 44 00 00                            ..D..        

l000000000040C3E8:
	mov	rcx,[rsp+30h]
	lea	rsi,[rcx+289h]
	mov	byte ptr [rcx+288h],20h
	mov	[rsp+18h],rsi
	jmp	40C26Ch
000000000040C405                0F 1F 00                              ...        

l000000000040C408:
	fld	[0000000000415F0C]                                     ; [rip+00009AFE]
	fxch	st(0),st(1)
	fucomi	st(0),st(1)
	jnc	40C6A0h

l000000000040C418:
	fstp	st(1)
	fstcw	word ptr [rsp+66h]
	movzx	eax,word ptr [rsp+66h]
	or	ah,0Ch
	mov	[rsp+64h],ax
	fld	st(0)
	fldcw	word ptr [rsp+64h]
	fistp	qword ptr [rsp+68h]
	fldcw	word ptr [rsp+66h]
	mov	rax,[rsp+68h]

l000000000040C43E:
	mov	esi,[rsp+38h]
	xor	edx,edx
	test	esi,esi
	jnz	40C470h

l000000000040C448:
	mov	[rsp+68h],rax
	test	rax,rax
	fild	qword ptr [rsp+68h]
	js	40C772h

l000000000040C45A:
	xor	ecx,ecx
	mov	edx,1h
	fucomip	st(0),st(1)
	fstp	st(0)
	setpe	cl
	cmovz	rdx,rcx

l000000000040C46C:
	jmp	40C472h
000000000040C46E                                           66 90               f.

l000000000040C470:
	fstp	st(0)

l000000000040C472:
	add	rax,rdx
	mov	[rsp+68h],rax
	test	rax,rax
	fild	qword ptr [rsp+68h]
	jns	40C1CDh

l000000000040C487:
	fadd	[0000000000415F08]                                    ; [rip+00009A7B]
	jmp	40C1CDh
000000000040C492       66 0F 1F 44 00 00                           f..D..        

l000000000040C498:
	fld	st(0)
	jmp	40BF72h

l000000000040C49F:
	cmp	r8,9h
	ja	40C768h

l000000000040C4A9:
	cmp	dword ptr [rsp+38h],1h
	jz	40C7B0h

l000000000040C4B4:
	mov	r12d,[rsp+38h]
	test	ecx,ecx
	setg	dl
	test	r12d,r12d
	setz	sil
	and	edx,esi

l000000000040C4C7:
	test	dl,dl
	jz	40C788h

l000000000040C4CF:
	lea	edi,[rax+1h]
	cmp	edi,0Ah
	jz	40C7CEh

l000000000040C4DB:
	mov	rax,[rsp+30h]
	add	edi,30h
	mov	rdx,r11
	mov	rsi,r15
	mov	[rsp+5Ch],r9d
	mov	[rsp+40h],r10
	mov	[rsp+50h],r11
	lea	r8,[rax+287h]
	mov	[rax+287h],dil
	sub	r8,r11
	mov	rdi,r8
	call	4025C0h
	mov	r11,[rsp+50h]
	mov	r10,[rsp+40h]
	mov	r8,rax
	mov	r9d,[rsp+5Ch]
	xor	ecx,ecx
	xor	edi,edi
	nop	word ptr [rax+rax+0h]

l000000000040C530:
	cmp	dword ptr [rsp+38h],1h
	jz	40C6F0h

l000000000040C53B:
	mov	esi,[rsp+38h]
	xor	eax,eax
	test	esi,esi
	jnz	40C54Ch

l000000000040C545:
	add	ecx,edi
	test	ecx,ecx
	setg	al

l000000000040C54C:
	test	al,al
	jz	40C5A0h

l000000000040C550:
	add	r10,1h
	test	r9d,r9d
	jz	40C5A0h

l000000000040C559:
	mov	eax,[rsp+24h]
	cmp	rax,r10
	jnz	40C5A0h

l000000000040C562:
	cmp	ebx,8h
	jz	40C5A0h

l000000000040C567:
	add	ebx,1h
	test	byte ptr [rsp+20h],8h
	mov	r10d,1h
	jnz	40C5A0h

l000000000040C577:
	lea	rax,[r8-1h]
	mov	byte ptr [r8-1h],30h
	mov	rdx,r11
	mov	rsi,r15
	mov	[rsp+38h],r10
	sub	rax,r11
	mov	rdi,rax
	call	4025C0h
	mov	r10,[rsp+38h]
	mov	r8,rax
	nop

l000000000040C5A0:
	mov	r12,r8
	mov	rcx,0CCCCCCCCCCCCCCCDh
	nop	dword ptr [rax]

l000000000040C5B0:
	mov	rax,r10
	sub	r12,1h
	mul	rcx
	shr	rdx,3h
	lea	rax,[rdx+rdx*4]
	add	rax,rax
	sub	r10,rax
	add	r10d,30h
	test	rdx,rdx
	mov	[r12],r10b
	mov	r10,rdx
	jnz	40C5B0h

l000000000040C5D8:
	jmp	40C237h
000000000040C5DD                                        0F 1F 00              ...

l000000000040C5E0:
	fadd	[0000000000415F08]                                    ; [rip+00009922]
	jmp	40BE94h
000000000040C5EB                                  0F 1F 44 00 00            ..D..

l000000000040C5F0:
	fadd	[0000000000415F08]                                    ; [rip+00009912]
	mov	[rsp+68h],rbx
	test	rbx,rbx
	fild	qword ptr [rsp+68h]
	jns	40BE7Dh

l000000000040C608:
	fadd	[0000000000415F08]                                    ; [rip+000098FA]
	jmp	40BE7Dh
000000000040C613          0F 1F 44 00 00                            ..D..        

l000000000040C618:
	mov	rdx,[rsp+28h]
	cmp	rdx,1h
	jbe	40C7A9h

l000000000040C627:
	mov	r14d,[rsp+24h]
	mov	ebx,1h
	mov	eax,1h
	nop	word ptr cs:[rax+rax+0h]

l000000000040C640:
	imul	rax,r14
	cmp	rdx,rax
	jbe	40C252h

l000000000040C64D:
	add	ebx,1h
	cmp	ebx,8h
	jnz	40C640h

l000000000040C655:
	jmp	40C252h
000000000040C65A                               66 0F 1F 44 00 00           f..D..

l000000000040C660:
	fstcw	word ptr [rsp+66h]
	movzx	eax,word ptr [rsp+66h]
	fsubr	st(1),st(0)
	fxch	st(0),st(1)
	mov	rdx,8000000000000000h
	or	ah,0Ch
	mov	[rsp+64h],ax
	fldcw	word ptr [rsp+64h]
	fistp	qword ptr [rsp+68h]
	fldcw	word ptr [rsp+66h]
	mov	rax,[rsp+68h]
	xor	rax,rdx
	jmp	40BF29h
000000000040C698                         0F 1F 84 00 00 00 00 00         ........

l000000000040C6A0:
	fstcw	word ptr [rsp+66h]
	movzx	eax,word ptr [rsp+66h]
	fsubr	st(1),st(0)
	fxch	st(0),st(1)
	mov	rdx,8000000000000000h
	or	ah,0Ch
	mov	[rsp+64h],ax
	fldcw	word ptr [rsp+64h]
	fistp	qword ptr [rsp+68h]
	fldcw	word ptr [rsp+66h]
	mov	rax,[rsp+68h]
	xor	rax,rdx
	jmp	40C43Eh
000000000040C6D8                         0F 1F 84 00 00 00 00 00         ........

l000000000040C6E0:
	sbb	ecx,ecx
	not	ecx
	add	ecx,3h
	jmp	40C12Dh
000000000040C6EC                                     0F 1F 40 00             ..@.

l000000000040C6F0:
	mov	rax,r10
	movsxd	rcx,ecx
	and	eax,1h
	add	rax,rcx
	setnz	al
	movzx	eax,al
	add	edi,eax
	cmp	edi,5h
	setg	al
	jmp	40C54Ch
000000000040C70F                                              90                .

l000000000040C710:
	mov	r8,[rsp+18h]
	xor	ebx,ebx
	jmp	40C530h
000000000040C71C                                     0F 1F 40 00             ..@.

l000000000040C720:
	fstcw	word ptr [rsp+66h]
	movzx	eax,word ptr [rsp+66h]
	fsubr	st(1),st(0)
	fxch	st(0),st(1)
	mov	rdx,8000000000000000h
	or	ah,0Ch
	mov	[rsp+64h],ax
	fldcw	word ptr [rsp+64h]
	fistp	qword ptr [rsp+68h]
	fldcw	word ptr [rsp+66h]
	mov	rax,[rsp+68h]
	xor	rax,rdx
	jmp	40C045h

l000000000040C758:
	fadd	[0000000000415F08]                                    ; [rip+000097AA]
	fxch	st(0),st(1)
	jmp	40BF72h
000000000040C765                0F 1F 00                              ...        

l000000000040C768:
	mov	r8,[rsp+18h]
	jmp	40C530h

l000000000040C772:
	fadd	[0000000000415F08]                                    ; [rip+00009790]
	jmp	40C45Ah

l000000000040C77D:
	fadd	[0000000000415F08]                                    ; [rip+00009785]
	jmp	40BF45h

l000000000040C788:
	test	eax,eax
	jnz	40C4DBh

l000000000040C790:
	test	byte ptr [rsp+20h],8h
	jnz	40C7C2h

l000000000040C797:
	xor	edi,edi
	jmp	40C4DBh

l000000000040C79E:
	fadd	[0000000000415F08]                                    ; [rip+00009764]
	jmp	40C063h

l000000000040C7A9:
	xor	ebx,ebx
	jmp	40C252h

l000000000040C7B0:
	mov	edx,eax
	and	edx,1h
	add	edx,ecx
	cmp	edx,2h
	setg	dl
	jmp	40C4C7h

l000000000040C7C2:
	mov	r8,[rsp+18h]
	xor	edi,edi
	jmp	40C530h

l000000000040C7CE:
	lea	r10,[r8+1h]
	cmp	r10,0Ah
	jnz	40C7FFh

l000000000040C7D8:
	mov	r8,[rsp+18h]
	xor	ecx,ecx
	xor	edi,edi
	jmp	40C530h

l000000000040C7E6:
	call	4023A0h

l000000000040C7EB:
	mov	rdx,[rsp+18h]
	jmp	40C29Ah

l000000000040C7F5:
	mov	[rsp+18h],rdx
	jmp	40C2BAh

l000000000040C7FF:
	xor	ecx,ecx
	jmp	40C790h
000000000040C803          66 66 66 66 2E 0F 1F 84 00 00 00 00 00    ffff.........

;; fn000000000040C810: 000000000040C810
;;   Called from:
;;     0000000000403095 (in fn00000000004028C0)
;;     00000000004039A4 (in fn00000000004028C0)
fn000000000040C810 proc
	push	r13
	mov	r13,rsi
	push	r12
	mov	r12,rdx
	push	rbp
	push	rbx
	mov	rbx,rdi
	sub	rsp,18h
	test	rdi,rdi
	jz	40C910h

l000000000040C82C:
	xor	ebp,ebp
	cmp	byte ptr [rbx],27h
	jz	40C8A0h

l000000000040C833:
	mov	ecx,4h
	mov	edx,415F50h
	mov	esi,415F60h
	mov	rdi,rbx
	call	409E50h
	test	eax,eax
	js	40C8B0h

l000000000040C84E:
	cdqe
	mov	qword ptr [r12],+1h
	mov	edx,1h
	or	ebp,[415F50h+rax*4]

l000000000040C864:
	mov	[r13+0h],ebp
	xor	eax,eax

l000000000040C86A:
	test	rdx,rdx
	jnz	40C894h

l000000000040C86F:
	mov	edi,415F2Eh
	call	4021C0h
	cmp	rax,1h
	sbb	rax,rax
	and	eax,200h
	add	rax,+200h
	mov	[r12],rax
	mov	eax,4h

l000000000040C894:
	add	rsp,18h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	ret
000000000040C89F                                              90                .

l000000000040C8A0:
	add	rbx,1h
	mov	bpl,4h
	jmp	40C833h
000000000040C8A9                            0F 1F 80 00 00 00 00          .......

l000000000040C8B0:
	lea	rsi,[rsp+8h]
	xor	edx,edx
	mov	r8d,415F3Eh
	mov	rcx,r12
	mov	rdi,rbx
	call	411360h
	test	eax,eax
	jnz	40C968h

l000000000040C8D0:
	movzx	eax,byte ptr [rbx]
	sub	eax,30h
	cmp	al,9h
	jbe	40C907h

l000000000040C8DA:
	mov	rdx,[rsp+8h]
	cmp	rbx,rdx
	jnz	40C8F9h

l000000000040C8E4:
	jmp	40C980h
000000000040C8E9                            0F 1F 80 00 00 00 00          .......

l000000000040C8F0:
	cmp	rbx,rdx
	jz	40C980h

l000000000040C8F9:
	add	rbx,1h
	movzx	eax,byte ptr [rbx]
	sub	eax,30h
	cmp	al,9h
	ja	40C8F0h

l000000000040C907:
	mov	rdx,[r12]
	jmp	40C864h

l000000000040C910:
	mov	edi,4138E4h
	call	4021C0h
	test	rax,rax
	mov	rbx,rax
	jnz	40C82Ch

l000000000040C926:
	mov	edi,415F24h
	call	4021C0h
	test	rax,rax
	mov	rbx,rax
	jnz	40C82Ch

l000000000040C93C:
	mov	edi,415F2Eh
	call	4021C0h
	cmp	rax,1h
	sbb	rdx,rdx
	xor	ebp,ebp
	and	edx,200h
	add	rdx,+200h
	mov	[r12],rdx
	jmp	40C864h
000000000040C965                0F 1F 00                              ...        

l000000000040C968:
	mov	dword ptr [r13+0h],0h
	mov	rdx,[r12]
	jmp	40C86Ah
000000000040C979                            0F 1F 80 00 00 00 00          .......

l000000000040C980:
	cmp	byte ptr [rdx-1h],42h
	jz	40C998h

l000000000040C986:
	or	bpl,80h

l000000000040C98A:
	or	ebp,20h
	jmp	40C907h
000000000040C992       66 0F 1F 44 00 00                           f..D..        

l000000000040C998:
	or	ebp,180h
	cmp	byte ptr [rdx-2h],69h
	jnz	40C907h

l000000000040C9A8:
	jmp	40C98Ah
000000000040C9AA                               66 0F 1F 44 00 00           f..D..

;; fn000000000040C9B0: 000000000040C9B0
;;   Called from:
;;     00000000004057E4 (in fn00000000004057B0)
;;     0000000000406248 (in fn00000000004061B0)
fn000000000040C9B0 proc
	push	r12
	push	rbp
	mov	ebp,edi
	push	rbx
	mov	rbx,[000000000061B1F8]                                 ; [rip+0020E83B]
	test	rbx,rbx
	jnz	40C9D1h

l000000000040C9C2:
	jmp	40C9F0h
000000000040C9C4             0F 1F 40 00                             ..@.        

l000000000040C9C8:
	mov	rbx,[rbx+8h]
	test	rbx,rbx
	jz	40C9F0h

l000000000040C9D1:
	cmp	[rbx],ebp
	jnz	40C9C8h

l000000000040C9D5:
	xor	eax,eax
	cmp	byte ptr [rbx+10h],0h
	lea	rdx,[rbx+10h]
	pop	rbx
	pop	rbp
	pop	r12
	cmovnz	rax,rdx

l000000000040C9E7:
	ret
000000000040C9E8                         0F 1F 84 00 00 00 00 00         ........

l000000000040C9F0:
	mov	edi,ebp
	mov	r12d,416919h
	call	402330h
	test	rax,rax
	mov	edi,11h
	jz	40CA16h

l000000000040CA07:
	mov	r12,[rax]
	mov	rdi,r12
	call	402380h
	lea	rdi,[rax+11h]

l000000000040CA16:
	call	410C40h
	lea	rdi,[rax+10h]
	mov	[rax],ebp
	mov	rsi,r12
	mov	rbx,rax
	call	402260h
	mov	rax,[000000000061B1F8]                                 ; [rip+0020E7C5]
	mov	[000000000061B1F8],rbx                                 ; [rip+0020E7BE]
	mov	[rbx+8h],rax
	jmp	40C9D5h
000000000040CA40 41 54 49 89 FC 55 53 48 8B 1D AA E7 20 00 48 85 ATI..USH.... .H.
000000000040CA50 DB 74 3D 0F B6 2F EB 11 0F 1F 84 00 00 00 00 00 .t=../..........
000000000040CA60 48 8B 5B 08 48 85 DB 74 27 40 38 6B 10 75 F1 48 H.[.H..t'@8k.u.H
000000000040CA70 8D 7B 10 4C 89 E6 E8 D5 5A FF FF 85 C0 75 E1 48 .{.L....Z....u.H
000000000040CA80 89 D8 5B 5D 41 5C C3 66 0F 1F 84 00 00 00 00 00 ..[]A\.f........
000000000040CA90 48 8B 1D 59 E7 20 00 48 85 DB 74 34 41 0F B6 2C H..Y. .H..t4A..,
000000000040CAA0 24 EB 0E 0F 1F 44 00 00 48 8B 5B 08 48 85 DB 74 $....D..H.[.H..t
000000000040CAB0 1F 40 38 6B 10 75 F1 48 8D 7B 10 4C 89 E6 E8 8D .@8k.u.H.{.L....
000000000040CAC0 5A FF FF 85 C0 75 E1 5B 5D 31 C0 41 5C C3 66 90 Z....u.[]1.A\.f.
000000000040CAD0 4C 89 E7 E8 A8 5A FF FF 4C 89 E7 48 89 C5 E8 9D L....Z..L..H....
000000000040CAE0 58 FF FF 48 8D 78 11 E8 54 41 00 00 48 8D 78 10 X..H.x..TA..H.x.
000000000040CAF0 4C 89 E6 48 89 C3 E8 65 57 FF FF 48 85 ED 74 1C L..H...eW..H..t.
000000000040CB00 8B 45 10 89 03 48 8B 05 EC E6 20 00 48 89 1D E5 .E...H.... .H...
000000000040CB10 E6 20 00 48 89 43 08 E9 63 FF FF FF 48 8B 05 CD . .H.C..c...H...
000000000040CB20 E6 20 00 48 89 1D C6 E6 20 00 48 89 43 08 31 C0 . .H.... .H.C.1.
000000000040CB30 E9 4D FF FF FF 66 66 2E 0F 1F 84 00 00 00 00 00 .M...ff.........

;; fn000000000040CB40: 000000000040CB40
;;   Called from:
;;     0000000000407420 (in fn0000000000406B70)
;;     0000000000408BAB (in fn0000000000407EA0)
fn000000000040CB40 proc
	push	r12
	push	rbp
	mov	ebp,edi
	push	rbx
	mov	rbx,[000000000061B1E8]                                 ; [rip+0020E69B]
	test	rbx,rbx
	jnz	40CB61h

l000000000040CB52:
	jmp	40CB80h
000000000040CB54             0F 1F 40 00                             ..@.        

l000000000040CB58:
	mov	rbx,[rbx+8h]
	test	rbx,rbx
	jz	40CB80h

l000000000040CB61:
	cmp	[rbx],ebp
	jnz	40CB58h

l000000000040CB65:
	xor	eax,eax
	cmp	byte ptr [rbx+10h],0h
	lea	rdx,[rbx+10h]
	pop	rbx
	pop	rbp
	pop	r12
	cmovnz	rax,rdx

l000000000040CB77:
	ret
000000000040CB78                         0F 1F 84 00 00 00 00 00         ........

l000000000040CB80:
	mov	edi,ebp
	mov	r12d,416919h
	call	4023E0h
	test	rax,rax
	mov	edi,11h
	jz	40CBA6h

l000000000040CB97:
	mov	r12,[rax]
	mov	rdi,r12
	call	402380h
	lea	rdi,[rax+11h]

l000000000040CBA6:
	call	410C40h
	lea	rdi,[rax+10h]
	mov	[rax],ebp
	mov	rsi,r12
	mov	rbx,rax
	call	402260h
	mov	rax,[000000000061B1E8]                                 ; [rip+0020E625]
	mov	[000000000061B1E8],rbx                                 ; [rip+0020E61E]
	mov	[rbx+8h],rax
	jmp	40CB65h
000000000040CBD0 41 54 49 89 FC 55 53 48 8B 1D 0A E6 20 00 48 85 ATI..USH.... .H.
000000000040CBE0 DB 74 3D 0F B6 2F EB 11 0F 1F 84 00 00 00 00 00 .t=../..........
000000000040CBF0 48 8B 5B 08 48 85 DB 74 27 40 38 6B 10 75 F1 48 H.[.H..t'@8k.u.H
000000000040CC00 8D 7B 10 4C 89 E6 E8 45 59 FF FF 85 C0 75 E1 48 .{.L...EY....u.H
000000000040CC10 89 D8 5B 5D 41 5C C3 66 0F 1F 84 00 00 00 00 00 ..[]A\.f........
000000000040CC20 48 8B 1D B9 E5 20 00 48 85 DB 74 34 41 0F B6 2C H.... .H..t4A..,
000000000040CC30 24 EB 0E 0F 1F 44 00 00 48 8B 5B 08 48 85 DB 74 $....D..H.[.H..t
000000000040CC40 1F 40 38 6B 10 75 F1 48 8D 7B 10 4C 89 E6 E8 FD .@8k.u.H.{.L....
000000000040CC50 58 FF FF 85 C0 75 E1 5B 5D 31 C0 41 5C C3 66 90 X....u.[]1.A\.f.
000000000040CC60 4C 89 E7 E8 68 59 FF FF 4C 89 E7 48 89 C5 E8 0D L...hY..L..H....
000000000040CC70 57 FF FF 48 8D 78 11 E8 C4 3F 00 00 48 8D 78 10 W..H.x...?..H.x.
000000000040CC80 4C 89 E6 48 89 C3 E8 D5 55 FF FF 48 85 ED 74 1C L..H....U..H..t.
000000000040CC90 8B 45 10 89 03 48 8B 05 4C E5 20 00 48 89 1D 45 .E...H..L. .H..E
000000000040CCA0 E5 20 00 48 89 43 08 E9 63 FF FF FF 48 8B 05 2D . .H.C..c...H..-
000000000040CCB0 E5 20 00 48 89 1D 26 E5 20 00 48 89 43 08 31 C0 . .H..&. .H.C.1.
000000000040CCC0 E9 4D FF FF FF 66 2E 0F 1F 84 00 00 00 00 00 90 .M...f..........

;; fn000000000040CCD0: 000000000040CCD0
;;   Called from:
;;     000000000040736E (in fn0000000000406B70)
fn000000000040CCD0 proc
	test	rdi,rdi
	lea	rcx,[rsi+14h]
	mov	byte ptr [rsi+14h],0h
	mov	rsi,6666666666666667h
	js	40CD28h

l000000000040CCE7:
	nop	word ptr [rax+rax+0h]

l000000000040CCF0:
	mov	rax,rdi
	sub	rcx,1h
	imul	rsi
	mov	rax,rdi
	sar	rax,3Fh
	sar	rdx,2h
	sub	rdx,rax
	lea	rax,[rdx+rdx*4]
	add	rax,rax
	sub	rdi,rax
	add	edi,30h
	test	rdx,rdx
	mov	[rcx],dil
	mov	rdi,rdx
	jnz	40CCF0h

l000000000040CD20:
	mov	rax,rcx
	ret
000000000040CD24             0F 1F 40 00                             ..@.        

l000000000040CD28:
	mov	r8,rsi
	mov	esi,30h

l000000000040CD30:
	mov	rax,rdi
	sub	rcx,1h
	imul	r8
	mov	rax,rdi
	sar	rax,3Fh
	sar	rdx,2h
	sub	rdx,rax
	lea	rax,[rdx+rdx*4]
	lea	eax,[rsi+rax*2]
	sub	eax,edi
	test	rdx,rdx
	mov	rdi,rdx
	mov	[rcx],al
	jnz	40CD30h

l000000000040CD5B:
	mov	rax,rcx
	sub	rcx,1h
	mov	byte ptr [rax-1h],2Dh
	mov	rax,rcx
	ret
000000000040CD6A                               66 0F 1F 44 00 00           f..D..

;; fn000000000040CD70: 000000000040CD70
;;   Called from:
;;     0000000000405EA9 (in fn0000000000405D50)
;;     000000000040707D (in fn0000000000406B70)
;;     0000000000407129 (in fn0000000000406B70)
;;     00000000004072E4 (in fn0000000000406B70)
;;     0000000000407316 (in fn0000000000406B70)
;;     000000000040798A (in fn0000000000407870)
;;     000000000040864B (in fn0000000000407EA0)
;;     000000000040874A (in fn0000000000407EA0)
;;     000000000040879F (in fn0000000000407EA0)
;;     00000000004087D1 (in fn0000000000407EA0)
fn000000000040CD70 proc
	lea	rcx,[rsi+14h]
	mov	byte ptr [rsi+14h],0h
	mov	rsi,0CCCCCCCCCCCCCCCDh
	nop	word ptr [rax+rax+0h]

l000000000040CD88:
	mov	rax,rdi
	sub	rcx,1h
	mul	rsi
	shr	rdx,3h
	lea	rax,[rdx+rdx*4]
	add	rax,rax
	sub	rdi,rax
	add	edi,30h
	test	rdx,rdx
	mov	[rcx],dil
	mov	rdi,rdx
	jnz	40CD88h

l000000000040CDAE:
	mov	rax,rcx
	ret
000000000040CDB2       66 2E 0F 1F 84 00 00 00 00 00 0F 1F 40 00   f...........@.

;; fn000000000040CDC0: 000000000040CDC0
;;   Called from:
;;     000000000040390E (in fn00000000004028C0)
fn000000000040CDC0 proc
	push	r15
	push	r14
	push	r13
	mov	r13,rsi
	push	r12
	mov	r12,rcx
	push	rbp
	push	rbx
	mov	ebx,r9d
	sub	rsp,38h
	mov	[rsp+8h],rdi
	mov	[rsp+18h],rdx
	mov	[rsp+28h],r8d
	call	402380h
	test	bl,2h
	mov	r14,rax
	mov	rbp,rax
	jz	40CF60h

l000000000040CDFA:
	mov	qword ptr [rsp+10h],+0h

l000000000040CE03:
	mov	r15,rbp
	xor	ebp,ebp

l000000000040CE08:
	mov	rax,[r12]
	cmp	r15,rax
	jbe	40CF2Ah

l000000000040CE15:
	mov	r14,rax
	xor	ecx,ecx

l000000000040CE1A:
	mov	[r12],rax
	mov	eax,[rsp+28h]
	test	eax,eax
	jz	40CF4Ch

l000000000040CE2A:
	xor	r12d,r12d
	cmp	eax,1h
	jz	40CE3Eh

l000000000040CE32:
	mov	r12,rcx
	and	ecx,1h
	shr	r12,1h
	add	rcx,r12

l000000000040CE3E:
	xor	eax,eax
	test	bl,4h
	cmovnz	rcx,rax

l000000000040CE47:
	and	ebx,8h
	cmovnz	r12,rax

l000000000040CE4E:
	cmp	qword ptr [rsp+18h],0h
	jz	40CEF9h

l000000000040CE5A:
	mov	rax,[rsp+18h]
	test	rcx,rcx
	lea	rdx,[rcx-1h]
	lea	rbx,[r13+rax-1h]
	jz	40CE97h

l000000000040CE6D:
	cmp	r13,rbx
	jnc	40CE97h

l000000000040CE72:
	xor	eax,eax
	jmp	40CE89h
000000000040CE76                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l000000000040CE80:
	add	rax,1h
	cmp	r13,rbx
	jz	40CE97h

l000000000040CE89:
	add	r13,1h
	cmp	rdx,rax
	mov	byte ptr [r13-1h],20h
	jnz	40CE80h

l000000000040CE97:
	mov	rdx,rbx
	mov	rsi,[rsp+8h]
	mov	byte ptr [r13+0h],0h
	sub	rdx,r13
	mov	rdi,r13
	mov	[rsp+18h],rcx
	cmp	rdx,r14
	cmova	rdx,r14

l000000000040CEB6:
	call	402750h
	test	r12,r12
	mov	rdx,rax
	lea	rsi,[r12-1h]
	mov	rcx,[rsp+18h]
	jz	40CEF6h

l000000000040CECD:
	cmp	rbx,rax
	jbe	40CEF6h

l000000000040CED2:
	xor	eax,eax
	jmp	40CEE9h
000000000040CED6                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l000000000040CEE0:
	add	rax,1h
	cmp	rdx,rbx
	jz	40CEF6h

l000000000040CEE9:
	add	rdx,1h
	cmp	rsi,rax
	mov	byte ptr [rdx-1h],20h
	jnz	40CEE0h

l000000000040CEF6:
	mov	byte ptr [rdx],0h

l000000000040CEF9:
	add	rcx,r14
	add	r12,rcx

l000000000040CEFF:
	mov	rdi,[rsp+10h]
	call	4021F0h
	mov	rdi,rbp
	call	4021F0h
	add	rsp,38h
	mov	rax,r12
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040CF23          0F 1F 44 00 00                            ..D..        

l000000000040CF28:
	xor	ebp,ebp

l000000000040CF2A:
	cmp	r15,rax
	jnc	40D180h

l000000000040CF33:
	sub	rax,r15
	mov	rcx,rax
	mov	rax,r15
	mov	[r12],rax
	mov	eax,[rsp+28h]
	test	eax,eax
	jnz	40CE2Ah

l000000000040CF4C:
	mov	r12,rcx
	xor	ecx,ecx
	jmp	40CE3Eh
000000000040CF56                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l000000000040CF60:
	call	402370h
	cmp	rax,1h
	jbe	40CDFAh

l000000000040CF6F:
	mov	rsi,[rsp+8h]
	xor	edx,edx
	xor	edi,edi
	call	4022D0h
	cmp	rax,0FFh
	jnz	40CFA8h

l000000000040CF83:
	test	bl,1h
	jnz	40D14Dh

l000000000040CF8C:
	mov	qword ptr [rsp+10h],+0h
	xor	ebp,ebp
	mov	r12,-1h
	jmp	40CEFFh
000000000040CFA3          0F 1F 44 00 00                            ..D..        

l000000000040CFA8:
	add	rax,1h
	lea	r15,[0000h+rax*4]
	mov	[rsp+20h],rax
	mov	rdi,r15
	call	402640h
	test	rax,rax
	mov	[rsp+10h],rax
	jz	40D160h

l000000000040CFCF:
	mov	rdx,[rsp+20h]
	mov	rsi,[rsp+8h]
	mov	rdi,[rsp+10h]
	call	4022D0h
	test	rax,rax
	jz	40CE03h

l000000000040CFEC:
	mov	rax,[rsp+10h]
	mov	dword ptr [rax+r15-4h],0h
	mov	edi,[rax]
	test	edi,edi
	jz	40D106h

l000000000040D004:
	mov	r15,rax
	mov	byte ptr [rsp+2Fh],0h
	nop	dword ptr [rax+0h]

l000000000040D010:
	call	402840h
	test	eax,eax
	jnz	40D025h

l000000000040D019:
	mov	dword ptr [r15],0FFFDh
	mov	byte ptr [rsp+2Fh],1h

l000000000040D025:
	add	r15,4h
	mov	edi,[r15]
	test	edi,edi
	jnz	40D010h

l000000000040D030:
	mov	rsi,[rsp+20h]
	mov	rdi,[rsp+10h]
	call	4022B0h
	cmp	byte ptr [rsp+2Fh],0h
	movsxd	r15,eax
	jz	40D118h

l000000000040D04D:
	mov	rsi,[rsp+10h]
	xor	edx,edx
	xor	edi,edi
	call	4027C0h
	add	rax,1h
	mov	[rsp+20h],rax

l000000000040D064:
	mov	rdi,[rsp+20h]
	call	402640h
	test	rax,rax
	mov	rbp,rax
	jz	40D138h

l000000000040D07A:
	mov	rax,[r12]
	mov	[rsp+8h],rax
	mov	rax,[rsp+10h]
	mov	edi,[rax]
	test	edi,edi
	jz	40D173h

l000000000040D092:
	mov	r14,rax
	xor	r15d,r15d
	jmp	40D0BAh
000000000040D09A                               66 0F 1F 44 00 00           f..D..

l000000000040D0A0:
	cdqe
	add	rax,r15
	cmp	[rsp+8h],rax
	jc	40D0E0h

l000000000040D0AC:
	add	r14,4h
	mov	edi,[r14]
	mov	r15,rax
	test	edi,edi
	jz	40D0E0h

l000000000040D0BA:
	call	402630h
	cmp	eax,0FFh
	jnz	40D0A0h

l000000000040D0C4:
	mov	eax,1h
	mov	dword ptr [r14],0FFFDh
	add	rax,r15
	cmp	[rsp+8h],rax
	jnc	40D0ACh

l000000000040D0DA:
	nop	word ptr [rax+rax+0h]

l000000000040D0E0:
	mov	rdx,[rsp+20h]
	mov	rsi,[rsp+10h]
	mov	rdi,rbp
	mov	dword ptr [r14],0h
	call	4027C0h
	mov	[rsp+8h],rbp
	mov	r14,rax
	jmp	40CE08h

l000000000040D106:
	mov	rsi,[rsp+20h]
	mov	rdi,[rsp+10h]
	call	4022B0h
	movsxd	r15,eax

l000000000040D118:
	mov	rax,[r12]
	cmp	r15,rax
	jbe	40CF28h

l000000000040D125:
	lea	rax,[rbp+1h]
	mov	[rsp+20h],rax
	jmp	40D064h
000000000040D133          0F 1F 44 00 00                            ..D..        

l000000000040D138:
	test	bl,1h
	jnz	40CE08h

l000000000040D141:
	mov	r12,-1h
	jmp	40CEFFh

l000000000040D14D:
	mov	r15,r14
	mov	qword ptr [rsp+10h],+0h
	xor	ebp,ebp
	jmp	40CE08h

l000000000040D160:
	test	bl,1h
	jz	40CF8Ch

l000000000040D169:
	mov	r15,r14
	xor	ebp,ebp
	jmp	40CE08h

l000000000040D173:
	mov	r14,[rsp+10h]
	xor	r15d,r15d
	jmp	40D0E0h

l000000000040D180:
	mov	rax,r15
	xor	ecx,ecx
	jmp	40CE1Ah
000000000040D18A                               66 0F 1F 44 00 00           f..D..
000000000040D190 41 57 49 89 FF 41 56 41 55 41 54 45 31 E4 55 48 AWI..AVAUATE1.UH
000000000040D1A0 89 F5 53 48 83 EC 18 4C 8B 2E 89 54 24 08 89 4C ..SH...L...T$..L
000000000040D1B0 24 0C 4C 89 E8 EB 0C 66 0F 1F 84 00 00 00 00 00 $.L....f........
000000000040D1C0 4D 89 F4 48 8D 58 01 4C 89 E7 48 89 DE E8 0E 55 M..H.X.L..H....U
000000000040D1D0 FF FF 48 85 C0 49 89 C6 74 46 44 8B 4C 24 0C 44 ..H..I..tFD.L$.D
000000000040D1E0 8B 44 24 08 48 89 E9 4C 89 6D 00 48 89 DA 48 89 .D$.H..L.m.H..H.
000000000040D1F0 C6 4C 89 FF E8 C7 FB FF FF 48 83 F8 FF 74 31 48 .L.......H...t1H
000000000040D200 39 C3 76 BC 48 83 C4 18 4C 89 F0 5B 5D 41 5C 41 9.v.H...L..[]A\A
000000000040D210 5D 41 5E 41 5F C3 66 2E 0F 1F 84 00 00 00 00 00 ]A^A_.f.........
000000000040D220 4C 89 E7 E8 C8 4F FF FF EB DA 66 0F 1F 44 00 00 L....O....f..D..
000000000040D230 4C 89 F7 45 31 F6 E8 B5 4F FF FF EB C7 0F 1F 00 L..E1...O.......

;; fn000000000040D240: 000000000040D240
;;   Called from:
;;     0000000000405371 (in fn00000000004052D0)
;;     0000000000407445 (in fn0000000000406B70)
;;     000000000040D43E (in fn000000000040D420)
fn000000000040D240 proc
	push	r15
	mov	r15d,edx
	push	r14
	push	r13
	lea	r13,[rdi+rsi]
	push	r12
	push	rbp
	mov	rbp,rdi
	push	rbx
	sub	rsp,28h
	mov	rax,fs:[0028h]
	mov	[rsp+18h],rax
	xor	eax,eax
	call	402370h
	cmp	rax,1h
	jbe	40D370h

l000000000040D277:
	cmp	rbp,r13
	jnc	40D406h

l000000000040D280:
	mov	r14d,r15d
	xor	r12d,r12d
	and	r15d,1h
	and	r14d,2h
	jmp	40D2ACh

l000000000040D290:
	cmp	al,25h
	jge	40D29Bh

l000000000040D294:
	sub	eax,20h
	cmp	al,3h
	ja	40D2C3h

l000000000040D29B:
	add	rbp,1h
	add	r12d,1h

l000000000040D2A3:
	cmp	r13,rbp
	jbe	40D401h

l000000000040D2AC:
	movzx	eax,byte ptr [rbp+0h]
	cmp	al,3Fh
	jle	40D290h

l000000000040D2B4:
	cmp	al,41h
	jl	40D2C3h

l000000000040D2B8:
	cmp	al,5Fh
	jle	40D29Bh

l000000000040D2BC:
	sub	eax,61h
	cmp	al,1Dh
	jbe	40D29Bh

l000000000040D2C3:
	mov	qword ptr [rsp+10h],+0h
	jmp	40D2F4h
000000000040D2CE                                           66 90               f.

l000000000040D2D0:
	mov	edx,7FFFFFFFh
	sub	edx,r12d
	cmp	eax,edx
	jg	40D3C0h

l000000000040D2E0:
	add	r12d,eax

l000000000040D2E3:
	lea	rdi,[rsp+10h]
	add	rbp,rbx
	call	402830h
	test	eax,eax
	jnz	40D2A3h

l000000000040D2F4:
	mov	rdx,r13
	lea	rcx,[rsp+10h]
	lea	rdi,[rsp+0Ch]
	sub	rdx,rbp
	mov	rsi,rbp
	call	4023C0h
	cmp	rax,0FFh
	mov	rbx,rax
	jz	40D360h

l000000000040D315:
	cmp	rax,0FEh
	jz	40D3E8h

l000000000040D31F:
	mov	edi,[rsp+0Ch]
	test	rax,rax
	mov	eax,1h
	cmovz	rbx,rax

l000000000040D32F:
	call	402630h
	test	eax,eax
	jns	40D2D0h

l000000000040D338:
	test	r14d,r14d
	jnz	40D369h

l000000000040D33D:
	mov	edi,[rsp+0Ch]
	call	4022A0h
	test	eax,eax
	jnz	40D2E3h

l000000000040D34A:
	cmp	r12d,7FFFFFFFh
	jz	40D3C0h

l000000000040D353:
	add	r12d,1h
	jmp	40D2E3h
000000000040D359                            0F 1F 80 00 00 00 00          .......

l000000000040D360:
	test	r15d,r15d
	jz	40D29Bh

l000000000040D369:
	mov	eax,0FFFFFFFFh
	jmp	40D3C5h

l000000000040D370:
	cmp	rbp,r13
	jnc	40D406h

l000000000040D379:
	call	402880h
	mov	esi,r15d
	mov	rcx,[rax]
	xor	eax,eax
	and	esi,2h
	jmp	40D398h
000000000040D38B                                  0F 1F 44 00 00            ..D..

l000000000040D390:
	add	eax,1h

l000000000040D393:
	cmp	rbp,r13
	jz	40D3C5h

l000000000040D398:
	add	rbp,1h
	movzx	edx,byte ptr [rbp-1h]
	movzx	edx,word ptr [rcx+rdx*2]
	test	dh,40h
	jnz	40D3B2h

l000000000040D3A9:
	test	esi,esi
	jnz	40D369h

l000000000040D3AD:
	and	edx,2h
	jnz	40D393h

l000000000040D3B2:
	cmp	eax,7FFFFFFFh
	jnz	40D390h

l000000000040D3B9:
	nop	dword ptr [rax+0h]

l000000000040D3C0:
	mov	eax,7FFFFFFFh

l000000000040D3C5:
	mov	rsi,[rsp+18h]
	xor	rsi,fs:[0028h]
	jnz	40D40Ah

l000000000040D3D5:
	add	rsp,28h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040D3E4             0F 1F 40 00                             ..@.        

l000000000040D3E8:
	test	r15d,r15d
	jnz	40D369h

l000000000040D3F1:
	mov	rbp,r13
	add	r12d,1h
	cmp	r13,rbp
	ja	40D2ACh

l000000000040D401:
	mov	eax,r12d
	jmp	40D3C5h

l000000000040D406:
	xor	eax,eax
	jmp	40D3C5h

l000000000040D40A:
	nop	word ptr [rax+rax+0h]
	call	4023A0h
000000000040D415                66 66 2E 0F 1F 84 00 00 00 00 00      ff.........

;; fn000000000040D420: 000000000040D420
;;   Called from:
;;     0000000000405712 (in fn0000000000405700)
;;     000000000040625B (in fn00000000004061B0)
;;     0000000000406C37 (in fn0000000000406B70)
;;     0000000000406DA7 (in fn0000000000406B70)
;;     000000000040815B (in fn0000000000407EA0)
;;     0000000000408A57 (in fn0000000000407EA0)
;;     0000000000408BBE (in fn0000000000407EA0)
fn000000000040D420 proc
	push	rbp
	mov	ebp,esi
	push	rbx
	mov	rbx,rdi
	sub	rsp,8h
	call	402380h
	add	rsp,8h
	mov	rdi,rbx
	mov	edx,ebp
	pop	rbx
	pop	rbp
	mov	rsi,rax
	jmp	40D240h
000000000040D443          66 2E 0F 1F 84 00 00 00 00 00 0F 1F 00    f............

;; fn000000000040D450: 000000000040D450
;;   Called from:
;;     000000000040D4B6 (in fn000000000040D450)
;;     000000000040D4EF (in fn000000000040D450)
;;     000000000040D504 (in fn000000000040D450)
;;     000000000040D69A (in fn000000000040D690)
fn000000000040D450 proc
	push	r15
	push	r14
	push	r13
	push	r12
	push	rbp
	mov	rbp,rcx
	push	rbx
	mov	rbx,rdi
	sub	rsp,38h
	cmp	rsi,2h
	mov	[rsp],rsi
	mov	[rsp+18h],rdx
	ja	40D490h

l000000000040D473:
	jz	40D660h

l000000000040D479:
	add	rsp,38h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040D488                         0F 1F 84 00 00 00 00 00         ........

l000000000040D490:
	mov	rsi,[rsp]
	mov	rdx,[rsp+18h]
	mov	rax,rsi
	shr	rax,1h
	mov	r15,rax
	mov	[rsp+8h],rax
	lea	rax,[rdi+rax*8]
	sub	rsi,r15
	mov	rdi,rax
	mov	[rsp+28h],rax
	call	40D450h
	cmp	r15,1h
	jz	40D570h

l000000000040D4C5:
	mov	rax,[rsp]
	mov	r15,[rsp+18h]
	mov	rcx,rbp
	mov	rsi,[rsp+8h]
	shr	rax,2h
	mov	rdx,r15
	lea	r13,[rbx+rax*8]
	mov	r14,rax
	sub	rsi,rax
	mov	[rsp+20h],rax
	mov	rdi,r13
	call	40D450h
	mov	rdx,r15
	mov	rcx,rbp
	mov	rsi,r14
	mov	rdi,rbx
	lea	r15,[r15+8h]
	call	40D450h
	mov	r12,[rbx]
	mov	r13,[r13+0h]
	mov	qword ptr [rsp+10h],+0h
	jmp	40D53Bh
000000000040D51B                                  0F 1F 44 00 00            ..D..

l000000000040D520:
	add	r14,1h
	cmp	[rsp+8h],r14
	mov	[r15-8h],r13
	jz	40D634h

l000000000040D533:
	mov	r13,[rbx+r14*8]

l000000000040D537:
	add	r15,8h

l000000000040D53B:
	mov	rsi,r13
	mov	rdi,r12
	call	rbp
	test	eax,eax
	jg	40D520h

l000000000040D547:
	add	qword ptr [rsp+10h],1h
	mov	[r15-8h],r12
	mov	rax,[rsp+10h]
	cmp	[rsp+20h],rax
	jz	40D625h

l000000000040D561:
	mov	r12,[rbx+rax*8]
	jmp	40D537h
000000000040D567                      66 0F 1F 84 00 00 00 00 00        f........

l000000000040D570:
	mov	r12,[rbx]
	mov	rax,[rsp+18h]
	mov	[rax],r12

l000000000040D57B:
	mov	rax,[rsp+28h]
	mov	r14,[rsp+8h]
	mov	r15d,1h
	mov	qword ptr [rsp+10h],+0h
	mov	r13,[rax]
	jmp	40D5B7h
000000000040D599                            0F 1F 80 00 00 00 00          .......

l000000000040D5A0:
	add	r14,1h
	cmp	[rsp],r14
	mov	[rbx+r15*8-8h],r13
	jz	40D5F0h

l000000000040D5AF:
	mov	r13,[rbx+r14*8]

l000000000040D5B3:
	add	r15,1h

l000000000040D5B7:
	mov	rsi,r13
	mov	rdi,r12
	call	rbp
	test	eax,eax
	jg	40D5A0h

l000000000040D5C3:
	add	qword ptr [rsp+10h],1h
	mov	[rbx+r15*8-8h],r12
	mov	rax,[rsp+10h]
	cmp	[rsp+8h],rax
	jz	40D479h

l000000000040D5DE:
	mov	rcx,[rsp+18h]
	mov	r12,[rcx+rax*8]
	jmp	40D5B3h
000000000040D5E9                            0F 1F 80 00 00 00 00          .......

l000000000040D5F0:
	mov	rax,[rsp+10h]
	lea	rdi,[rbx+r15*8]
	mov	rcx,[rsp+18h]
	mov	r15,[rsp+8h]
	add	rsp,38h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	sub	r15,rax
	lea	rsi,[rcx+rax*8]
	pop	r14
	lea	rdx,[0000h+r15*8]
	pop	r15
	jmp	4025C0h

l000000000040D625:
	mov	rax,[rsp+8h]
	mov	[rsp+10h],r14
	mov	[rsp+20h],rax

l000000000040D634:
	mov	rax,[rsp+10h]
	mov	rdx,[rsp+20h]
	mov	rdi,r15
	sub	rdx,rax
	lea	rsi,[rbx+rax*8]
	shl	rdx,3h
	call	4025C0h
	mov	rax,[rsp+18h]
	mov	r12,[rax]
	jmp	40D57Bh
000000000040D65E                                           66 90               f.

l000000000040D660:
	mov	r13,[rdi+8h]
	mov	r12,[rdi]
	mov	rsi,r13
	mov	rdi,r12
	call	rcx
	test	eax,eax
	jle	40D479h

l000000000040D677:
	mov	[rbx],r13
	mov	[rbx+8h],r12
	add	rsp,38h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040D68D                                        0F 1F 00              ...

;; fn000000000040D690: 000000000040D690
;;   Called from:
;;     0000000000404F87 (in fn0000000000404E80)
fn000000000040D690 proc
	lea	rax,[rdi+rsi*8]
	mov	rcx,rdx
	mov	rdx,rax
	jmp	40D450h
000000000040D69F                                              90                .

;; fn000000000040D6A0: 000000000040D6A0
;;   Called from:
;;     00000000004028EC (in fn00000000004028C0)
fn000000000040D6A0 proc
	test	rdi,rdi
	push	rbx
	mov	rbx,rdi
	jz	40D713h

l000000000040D6A9:
	mov	esi,2Fh
	call	402410h
	test	rax,rax
	jz	40D703h

l000000000040D6B8:
	lea	rdx,[rax+1h]
	mov	rcx,rdx
	sub	rcx,rbx
	cmp	rcx,6h
	jle	40D703h

l000000000040D6C8:
	lea	rsi,[rax-6h]
	mov	edi,415FD8h
	mov	ecx,7h

l000000000040D6D6:
	rep cmpsb

l000000000040D6D8:
	jnz	40D703h

l000000000040D6DA:
	mov	ecx,3h
	mov	rsi,rdx
	mov	edi,415FE0h

l000000000040D6E7:
	rep cmpsb

l000000000040D6E9:
	mov	rbx,rdx
	seta	sil
	setc	cl
	cmp	sil,cl
	jnz	40D703h

l000000000040D6F8:
	lea	rbx,[rax+4h]
	mov	[000000000061A600],rbx                                 ; [rip+0020CEFD]

l000000000040D703:
	mov	[000000000061B200],rbx                                 ; [rip+0020DAF6]
	mov	[000000000061A648],rbx                                 ; [rip+0020CF37]
	pop	rbx
	ret

l000000000040D713:
	mov	rcx,[000000000061A650]                                 ; [rip+0020CF36]
	mov	edx,37h
	mov	esi,1h
	mov	edi,415FA0h
	call	402800h
	call	402220h
000000000040D733          66 2E 0F 1F 84 00 00 00 00 00 0F 1F 00    f............

;; fn000000000040D740: 000000000040D740
;;   Called from:
;;     000000000040E97E (in fn000000000040E970)
fn000000000040D740 proc
	sub	rsp,48h
	xor	eax,eax
	mov	rdx,rdi
	mov	ecx,7h
	mov	rdi,rsp
	cmp	esi,8h

l000000000040D754:
	rep stosq

l000000000040D757:
	jz	40D7A1h

l000000000040D759:
	mov	[rsp],esi
	mov	rax,[rsp]
	mov	[rdx],rax
	mov	rax,[rsp+8h]
	mov	[rdx+8h],rax
	mov	rax,[rsp+10h]
	mov	[rdx+10h],rax
	mov	rax,[rsp+18h]
	mov	[rdx+18h],rax
	mov	rax,[rsp+20h]
	mov	[rdx+20h],rax
	mov	rax,[rsp+28h]
	mov	[rdx+28h],rax
	mov	rax,[rsp+30h]
	mov	[rdx+30h],rax
	mov	rax,rdx
	add	rsp,48h
	ret

l000000000040D7A1:
	call	402220h
000000000040D7A6                   66 2E 0F 1F 84 00 00 00 00 00       f.........

;; fn000000000040D7B0: 000000000040D7B0
;;   Called from:
;;     000000000040DFF2 (in fn000000000040D8A0)
;;     000000000040E003 (in fn000000000040D8A0)
fn000000000040D7B0 proc
	push	r13
	mov	edx,5h
	push	r12
	mov	r12d,esi
	mov	rsi,rdi
	push	rbp
	mov	rbp,rdi
	xor	edi,edi
	push	rbx
	sub	rsp,8h
	call	402360h
	cmp	rax,rbp
	mov	rbx,rax
	jz	40D7E8h

l000000000040D7D7:
	add	rsp,8h
	mov	rax,rbx
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	ret
000000000040D7E5                0F 1F 00                              ...        

l000000000040D7E8:
	call	411900h
	movzx	edx,byte ptr [rax]
	and	edx,0DFh
	cmp	dl,55h
	jnz	40D850h

l000000000040D7F8:
	movzx	edx,byte ptr [rax+1h]
	and	edx,0DFh
	cmp	dl,54h
	jnz	40D838h

l000000000040D804:
	movzx	edx,byte ptr [rax+2h]
	and	edx,0DFh
	cmp	dl,46h
	jnz	40D838h

l000000000040D810:
	cmp	byte ptr [rax+3h],2Dh
	jnz	40D838h

l000000000040D816:
	cmp	byte ptr [rax+4h],38h
	jnz	40D838h

l000000000040D81C:
	cmp	byte ptr [rax+5h],0h
	jnz	40D838h

l000000000040D822:
	cmp	byte ptr [rbx],60h
	mov	eax,415FF1h
	mov	ebx,415FE4h
	cmovz	rbx,rax

l000000000040D833:
	jmp	40D7D7h
000000000040D835                0F 1F 00                              ...        

l000000000040D838:
	mov	ebx,415FEBh
	cmp	r12d,7h
	mov	eax,416DEAh
	cmovnz	rbx,rax

l000000000040D84A:
	jmp	40D7D7h
000000000040D84C                                     0F 1F 40 00             ..@.

l000000000040D850:
	cmp	dl,47h
	jnz	40D838h

l000000000040D855:
	movzx	edx,byte ptr [rax+1h]
	and	edx,0DFh
	cmp	dl,42h
	jnz	40D838h

l000000000040D861:
	cmp	byte ptr [rax+2h],31h
	jnz	40D838h

l000000000040D867:
	cmp	byte ptr [rax+3h],38h
	jnz	40D838h

l000000000040D86D:
	cmp	byte ptr [rax+4h],30h
	jnz	40D838h

l000000000040D873:
	cmp	byte ptr [rax+5h],33h
	jnz	40D838h

l000000000040D879:
	cmp	byte ptr [rax+6h],30h
	jnz	40D838h

l000000000040D87F:
	cmp	byte ptr [rax+7h],0h
	jnz	40D838h

l000000000040D885:
	mov	r13,rbx
	mov	eax,415FE8h
	mov	ebx,415FEDh
	cmp	byte ptr [r13+0h],60h
	cmovnz	rbx,rax

l000000000040D89B:
	jmp	40D7D7h

;; fn000000000040D8A0: 000000000040D8A0
;;   Called from:
;;     000000000040DC81 (in fn000000000040D8A0)
;;     000000000040E545 (in fn000000000040E450)
;;     000000000040E5BA (in fn000000000040E450)
;;     000000000040E755 (in fn000000000040E6F0)
fn000000000040D8A0 proc
	push	r15
	mov	r15,rcx
	push	r14
	mov	r14d,r8d
	push	r13
	mov	r13,rdx
	push	r12
	push	rbp
	push	rbx
	mov	ebx,r9d
	sub	rsp,+0C8h
	mov	rax,[rsp+100h]
	mov	[rsp+28h],rdi
	mov	[rsp+20h],rsi
	mov	[rsp+34h],r8d
	mov	[rsp+90h],r9d
	mov	[rsp+58h],rax
	mov	rax,[rsp+108h]
	mov	[rsp+70h],rax
	mov	rax,[rsp+110h]
	mov	[rsp+68h],rax
	mov	rax,fs:[0028h]
	mov	[rsp+0B8h],rax
	xor	eax,eax
	call	402370h
	mov	[rsp+78h],rax
	mov	eax,ebx
	shr	eax,1h
	and	eax,1h
	cmp	r14d,8h
	mov	[rsp+33h],al
	ja	40E280h

l000000000040D92D:
	mov	eax,r14d
	mov	r11,[rsp+20h]
	jmp	qword ptr [416020h+rax*8]
000000000040D93C                                     0F 1F 40 00             ..@.

l000000000040D940:
	mov	byte ptr [rsp+33h],0h
	mov	byte ptr [rsp+20h],0h
	xor	r14d,r14d
	mov	qword ptr [rsp+60h],+0h
	xor	ebx,ebx
	nop	dword ptr [rax+rax+0h]

l000000000040D960:
	movzx	eax,byte ptr [rsp+33h]
	mov	r9,r14
	xor	ebp,ebp
	mov	r14,r11
	mov	r8,r13
	xor	eax,1h
	mov	[rsp+38h],al
	movzx	eax,byte ptr [rsp+20h]
	xor	eax,1h
	mov	[rsp+95h],al
	cmp	rbp,r15
	setnz	al
	cmp	r15,0FFh
	jz	40DB76h

l000000000040D996:
	nop	word ptr cs:[rax+rax+0h]

l000000000040D9A0:
	test	al,al
	jz	40DB86h

l000000000040D9A8:
	test	r9,r9
	setnz	cl
	jz	40E0A0h

l000000000040D9B4:
	cmp	byte ptr [rsp+20h],0h
	jz	40E0A0h

l000000000040D9BF:
	lea	rax,[rbp+r9+0h]
	cmp	r15,rax
	jc	40E0A0h

l000000000040D9CD:
	lea	r13,[r8+rbp]
	mov	rsi,[rsp+60h]
	mov	rdx,r9
	mov	[rsp+50h],ecx
	mov	[rsp+48h],r8
	mov	rdi,r13
	mov	[rsp+40h],r9
	call	402500h
	test	eax,eax
	mov	r9,[rsp+40h]
	mov	r8,[rsp+48h]
	mov	ecx,[rsp+50h]
	jnz	40E0B0h

l000000000040DA05:
	cmp	byte ptr [rsp+33h],0h
	jnz	40DC40h

l000000000040DA10:
	mov	r11d,1h
	nop	word ptr cs:[rax+rax+0h]

l000000000040DA20:
	movzx	r12d,byte ptr [r13+0h]
	cmp	r12b,7Eh
	ja	40DE18h

l000000000040DA2F:
	movzx	eax,r12b
	jmp	qword ptr [416068h+rax*8]
000000000040DA3A                               66 0F 1F 44 00 00           f..D..
000000000040DA40 8B 44 24 34 83 F8 02 0F 84 E3 01 00 00 83 F8 03 .D$4............
000000000040DA50 0F 85 A2 00 00 00 F6 84 24 90 00 00 00 04 0F 84 ........$.......
000000000040DA60 94 00 00 00 48 8D 45 02 49 39 C7 0F 86 87 00 00 ....H.E.I9......
000000000040DA70 00 41 80 7C 28 01 3F 75 7F 41 0F B6 34 00 8D 4E .A.|(.?u.A..4..N
000000000040DA80 DF 80 F9 1D 77 72 BA 01 00 00 00 48 D3 E2 F7 C2 ....wr.....H....
000000000040DA90 C1 51 00 38 74 62 80 7C 24 33 00 0F 85 9F 01 00 .Q.8tb.|$3......
000000000040DAA0 00 4C 39 F3 73 09 48 8B 7C 24 28 C6 04 1F 3F 48 .L9.s.H.|$(...?H
000000000040DAB0 8D 53 01 49 39 D6 76 0A 48 8B 7C 24 28 C6 44 1F .S.I9.v.H.|$(.D.
000000000040DAC0 01 22 48 8D 53 02 49 39 D6 76 0A 48 8B 7C 24 28 ."H.S.I9.v.H.|$(
000000000040DAD0 C6 44 1F 02 22 48 8D 53 03 49 39 D6 76 0A 48 8B .D.."H.S.I9.v.H.
000000000040DAE0 7C 24 28 C6 44 1F 03 3F 48 83 C3 04 41 89 F4 48 |$(.D..?H...A..H
000000000040DAF0 89 C5 66 0F 1F 44 00 00                         ..f..D..        

l000000000040DAF8:
	cmp	byte ptr [rsp+38h],0h
	jz	40DB09h

l000000000040DAFF:
	cmp	byte ptr [rsp+95h],0h
	jnz	40DB2Eh

l000000000040DB09:
	mov	rdi,[rsp+58h]
	test	rdi,rdi
	jz	40DB2Eh

l000000000040DB13:
	mov	edx,r12d
	mov	ecx,r12d
	mov	eax,1h
	shr	dl,5h
	and	ecx,1Fh
	movzx	edx,dl
	shl	eax,cl
	test	[rdi+rdx*4],eax
	jnz	40DB33h

l000000000040DB2E:
	test	r11b,r11b
	jz	40DB50h

l000000000040DB33:
	cmp	byte ptr [rsp+33h],0h
	jnz	40DC40h

l000000000040DB3E:
	cmp	rbx,r14
	jnc	40DB4Ch

l000000000040DB43:
	mov	rax,[rsp+28h]
	mov	byte ptr [rax+rbx],5Ch

l000000000040DB4C:
	add	rbx,1h

l000000000040DB50:
	add	rbp,1h

l000000000040DB54:
	cmp	rbx,r14
	jnc	40DB62h

l000000000040DB59:
	mov	rax,[rsp+28h]
	mov	[rax+rbx],r12b

l000000000040DB62:
	add	rbx,1h
	cmp	rbp,r15
	setnz	al
	cmp	r15,0FFh
	jnz	40D9A0h

l000000000040DB76:
	cmp	byte ptr [r8+rbp],0h
	setnz	al
	test	al,al
	jnz	40D9A8h

l000000000040DB86:
	test	rbx,rbx
	mov	r11,r14
	mov	r13,r8
	jnz	40DBA3h

l000000000040DB91:
	cmp	dword ptr [rsp+34h],2h
	jnz	40DBA3h

l000000000040DB98:
	cmp	byte ptr [rsp+33h],0h
	jnz	40DC46h

l000000000040DBA3:
	cmp	byte ptr [rsp+33h],0h
	jnz	40DBE4h

l000000000040DBAA:
	cmp	qword ptr [rsp+60h],0h
	jz	40DBE4h

l000000000040DBB2:
	mov	rdx,[rsp+60h]
	movzx	eax,byte ptr [rdx]
	test	al,al
	jz	40DBE4h

l000000000040DBBE:
	mov	rcx,[rsp+28h]
	sub	rdx,rbx
	nop	word ptr cs:[rax+rax+0h]

l000000000040DBD0:
	cmp	r11,rbx
	jbe	40DBD8h

l000000000040DBD5:
	mov	[rcx+rbx],al

l000000000040DBD8:
	add	rbx,1h
	movzx	eax,byte ptr [rdx+rbx]
	test	al,al
	jnz	40DBD0h

l000000000040DBE4:
	cmp	rbx,r11
	mov	rax,rbx
	jnc	40DC86h

l000000000040DBF0:
	mov	rsi,[rsp+28h]
	mov	byte ptr [rsi+rbx],0h
	jmp	40DC86h
000000000040DBFE                                           66 90               f.
000000000040DC00 49 83 FF 01 0F 95 C0 49 83 FF FF 0F 84 5F 06 00 I......I....._..
000000000040DC10 00 84 C0 0F 85 DF FE FF FF 48 85 ED 0F 85 D6 FE .........H......
000000000040DC20 FF FF 83 7C 24 34 02 0F 85 CB FE FF FF 0F 1F 00 ...|$4..........
000000000040DC30 80 7C 24 33 00 0F 84 BD FE FF FF 0F 1F 44 00 00 .|$3.........D..

l000000000040DC40:
	mov	r11,r14
	mov	r13,r8

l000000000040DC46:
	mov	rax,[rsp+68h]
	mov	r9d,[rsp+90h]
	mov	rcx,r15
	mov	r8d,[rsp+34h]
	mov	rdi,[rsp+28h]
	mov	rdx,r13
	mov	qword ptr [rsp],+0h
	mov	rsi,r11
	mov	[rsp+10h],rax
	mov	rax,[rsp+70h]
	and	r9d,0FDh
	mov	[rsp+8h],rax
	call	40D8A0h

l000000000040DC86:
	mov	rsi,[rsp+0B8h]
	xor	rsi,fs:[0028h]
	jnz	40E423h

l000000000040DC9D:
	add	rsp,+0C8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040DCAF                                              90                .
000000000040DCB0 B8 72 00 00 00 83 7C 24 34 02 0F 84 2F 06 00 00 .r....|$4.../...
000000000040DCC0 80 7C 24 20 00 0F 84 2D FE FF FF 41 89 C4 E9 60 .|$ ...-...A...`
000000000040DCD0 FE FF FF 0F 1F 44 00 00 B8 62 00 00 00 EB E1 90 .....D...b......
000000000040DCE0 B8 66 00 00 00 EB D9 66 0F 1F 84 00 00 00 00 00 .f.....f........
000000000040DCF0 B8 76 00 00 00 EB C9 66 0F 1F 84 00 00 00 00 00 .v.....f........
000000000040DD00 B8 6E 00 00 00 EB AE 66 0F 1F 84 00 00 00 00 00 .n.....f........
000000000040DD10 B8 74 00 00 00 EB 9E 66 0F 1F 84 00 00 00 00 00 .t.....f........
000000000040DD20 80 7C 24 20 00 0F 84 15 05 00 00 80 7C 24 33 00 .|$ ........|$3.
000000000040DD30 0F 85 0A FF FF FF 4C 39 F3 73 09 48 8B 44 24 28 ......L9.s.H.D$(
000000000040DD40 C6 04 18 5C 48 8D 55 01 48 8D 43 01 49 39 D7 76 ...\H.U.H.C.I9.v
000000000040DD50 2F 41 0F B6 74 28 01 8D 56 D0 80 FA 09 77 21 49 /A..t(..V....w!I
000000000040DD60 39 C6 0F 87 98 05 00 00 48 8D 43 02 49 39 C6 76 9.......H.C.I9.v
000000000040DD70 0A 48 8B 44 24 28 C6 44 18 02 30 48 8D 43 03 90 .H.D$(.D..0H.C..
000000000040DD80 48 89 C3 41 BC 30 00 00 00 E9 7B FD FF FF 66 90 H..A.0....{...f.
000000000040DD90 B8 61 00 00 00 E9 26 FF FF FF 66 0F 1F 44 00 00 .a....&...f..D..
000000000040DDA0 80 7C 24 20 00 74 0F 80 7C 24 33 00 74 08 84 C9 .|$ .t..|$3.t...
000000000040DDB0 0F 85 9A FD FF FF 44 89 E0 E9 F7 FE FF FF 66 90 ......D.......f.
000000000040DDC0 83 7C 24 34 02 0F 85 2D FD FF FF 80 7C 24 33 00 .|$4...-....|$3.
000000000040DDD0 0F 85 6A FE FF FF 4C 39 F3 73 09 48 8B 44 24 28 ..j...L9.s.H.D$(
000000000040DDE0 C6 04 18 27 48 8D 43 01 49 39 C6 76 0A 48 8B 44 ...'H.C.I9.v.H.D
000000000040DDF0 24 28 C6 44 18 01 5C 48 8D 43 02 49 39 C6 76 0A $(.D..\H.C.I9.v.
000000000040DE00 48 8B 44 24 28 C6 44 18 02 27 48 83 C3 03 E9 E5 H.D$(.D..'H.....
000000000040DE10 FC FF FF 0F 1F 44 00 00                         .....D..        

l000000000040DE18:
	cmp	qword ptr [rsp+78h],1h
	jnz	40E0C0h

l000000000040DE24:
	mov	[rsp+50h],r8
	mov	[rsp+48h],r9
	mov	[rsp+40h],r11d
	call	402880h
	mov	rax,[rax]
	movzx	edx,r12b
	mov	r11d,[rsp+40h]
	mov	r9,[rsp+48h]
	mov	r8,[rsp+50h]
	movzx	edx,word ptr [rax+rdx*2]
	mov	eax,1h
	shr	dx,0Eh
	xor	edx,1h
	and	edx,1h

l000000000040DE61:
	and	dl,[rsp+20h]
	jz	40DAF8h

l000000000040DE6B:
	add	rax,rbp
	movzx	edi,byte ptr [rsp+33h]
	mov	rcx,[rsp+28h]
	jmp	40DEF0h
000000000040DE7A                               66 0F 1F 44 00 00           f..D..

l000000000040DE80:
	test	dil,dil
	jnz	40DC40h

l000000000040DE89:
	cmp	rbx,r14
	jnc	40DE92h

l000000000040DE8E:
	mov	byte ptr [rcx+rbx],5Ch

l000000000040DE92:
	lea	rsi,[rbx+1h]
	cmp	r14,rsi
	jbe	40DEAAh

l000000000040DE9B:
	mov	esi,r12d
	shr	sil,6h
	add	esi,30h
	mov	[rcx+rbx+1h],sil

l000000000040DEAA:
	lea	rsi,[rbx+2h]
	cmp	r14,rsi
	jbe	40DEC5h

l000000000040DEB3:
	mov	esi,r12d
	shr	sil,3h
	and	esi,7h
	add	esi,30h
	mov	[rcx+rbx+2h],sil

l000000000040DEC5:
	and	r12d,7h
	add	rbx,3h
	add	r12d,30h

l000000000040DED1:
	add	rbp,1h
	cmp	rax,rbp
	jbe	40DB54h

l000000000040DEDE:
	cmp	rbx,r14
	jnc	40DEE7h

l000000000040DEE3:
	mov	[rcx+rbx],r12b

l000000000040DEE7:
	movzx	r12d,byte ptr [r8+rbp]
	add	rbx,1h

l000000000040DEF0:
	test	dl,dl
	jnz	40DE80h

l000000000040DEF4:
	test	r11b,r11b
	jz	40DED1h

l000000000040DEF9:
	cmp	rbx,r14
	jnc	40DF02h

l000000000040DEFE:
	mov	byte ptr [rcx+rbx],5Ch

l000000000040DF02:
	add	rbx,1h
	xor	r11d,r11d
	jmp	40DED1h
000000000040DF0B                                  0F 1F 44 00 00            ..D..

l000000000040DF10:
	cmp	byte ptr [rsp+33h],0h
	jnz	40E408h

l000000000040DF1B:
	test	r11,r11
	jz	40E2D1h

l000000000040DF24:
	mov	rax,[rsp+28h]
	mov	byte ptr [rsp+20h],0h
	mov	r14d,1h
	mov	qword ptr [rsp+60h],+416DEAh
	mov	ebx,1h
	mov	byte ptr [rax],27h
	jmp	40D960h
000000000040DF4A                               66 0F 1F 44 00 00           f..D..

l000000000040DF50:
	cmp	byte ptr [rsp+33h],0h
	jnz	40E428h

l000000000040DF5B:
	test	r11,r11
	jz	40E2B3h

l000000000040DF64:
	mov	rax,[rsp+28h]
	mov	byte ptr [rsp+20h],1h
	mov	r14d,1h
	mov	qword ptr [rsp+60h],+415FEBh
	mov	ebx,1h
	mov	byte ptr [rax],22h
	jmp	40D960h
000000000040DF8A                               66 0F 1F 44 00 00           f..D..

l000000000040DF90:
	mov	byte ptr [rsp+33h],0h
	mov	byte ptr [rsp+20h],1h
	xor	r14d,r14d
	mov	qword ptr [rsp+60h],+0h
	xor	ebx,ebx
	jmp	40D960h
000000000040DFAD                                        0F 1F 00              ...

l000000000040DFB0:
	mov	byte ptr [rsp+33h],1h
	mov	byte ptr [rsp+20h],1h
	mov	r14d,1h
	mov	qword ptr [rsp+60h],+415FEBh
	xor	ebx,ebx
	mov	dword ptr [rsp+34h],3h
	jmp	40D960h
000000000040DFD8                         0F 1F 84 00 00 00 00 00         ........

l000000000040DFE0:
	jz	40E012h

l000000000040DFE2:
	mov	ebx,[rsp+34h]
	mov	edi,415FF5h
	mov	[rsp+20h],r11
	mov	esi,ebx
	call	40D7B0h
	mov	esi,ebx
	mov	edi,416DEAh
	mov	[rsp+70h],rax
	call	40D7B0h
	mov	r11,[rsp+20h]
	mov	[rsp+68h],rax

l000000000040E012:
	xor	ebx,ebx
	cmp	byte ptr [rsp+33h],0h
	jnz	40E044h

l000000000040E01B:
	mov	rdx,[rsp+70h]
	movzx	eax,byte ptr [rdx]
	test	al,al
	jz	40E044h

l000000000040E027:
	mov	rcx,[rsp+28h]
	nop	dword ptr [rax+0h]

l000000000040E030:
	cmp	rbx,r11
	jnc	40E038h

l000000000040E035:
	mov	[rcx+rbx],al

l000000000040E038:
	add	rbx,1h
	movzx	eax,byte ptr [rdx+rbx]
	test	al,al
	jnz	40E030h

l000000000040E044:
	mov	rbp,[rsp+68h]
	mov	[rsp+38h],r11
	mov	rdi,rbp
	call	402380h
	mov	[rsp+60h],rbp
	mov	r14,rax
	mov	byte ptr [rsp+20h],1h
	mov	r11,[rsp+38h]
	jmp	40D960h
000000000040E06D                                        0F 1F 00              ...

l000000000040E070:
	mov	byte ptr [rsp+33h],1h
	mov	byte ptr [rsp+20h],0h
	mov	r14d,1h
	mov	qword ptr [rsp+60h],+416DEAh
	xor	ebx,ebx
	mov	dword ptr [rsp+34h],2h
	jmp	40D960h
000000000040E098                         0F 1F 84 00 00 00 00 00         ........

l000000000040E0A0:
	lea	r13,[r8+rbp]
	xor	r11d,r11d
	jmp	40DA20h
000000000040E0AC                                     0F 1F 40 00             ..@.

l000000000040E0B0:
	xor	r11d,r11d
	jmp	40DA20h
000000000040E0B8                         0F 1F 84 00 00 00 00 00         ........

l000000000040E0C0:
	cmp	r15,0FFh
	mov	qword ptr [rsp+0B0h],+0h
	jz	40E285h

l000000000040E0D6:
	mov	esi,1h
	xor	eax,eax
	mov	[rsp+80h],rbx
	mov	[rsp+96h],r12b
	mov	[rsp+98h],r13
	mov	rbx,rax
	mov	[rsp+48h],rbp
	mov	[rsp+88h],r9
	mov	r12d,esi
	mov	[rsp+97h],r11b
	mov	[rsp+50h],r14
	mov	r13,r8
	mov	[rsp+40h],r15

l000000000040E11D:
	mov	rax,[rsp+48h]
	mov	rdx,[rsp+40h]
	lea	rcx,[rsp+0B0h]
	lea	rdi,[rsp+0ACh]
	lea	r14,[rbx+rax]
	lea	r15,[r13+r14+0h]
	sub	rdx,r14
	mov	rsi,r15
	call	4023C0h
	test	rax,rax
	mov	rbp,rax
	jz	40E30Eh

l000000000040E157:
	cmp	rax,0FFh
	jz	40E350h

l000000000040E161:
	cmp	rax,0FEh
	jz	40E391h

l000000000040E16B:
	cmp	byte ptr [rsp+33h],0h
	jz	40E1BFh

l000000000040E172:
	cmp	dword ptr [rsp+34h],2h
	jnz	40E1BFh

l000000000040E179:
	cmp	rax,1h
	jz	40E1BFh

l000000000040E17F:
	mov	edx,1h
	mov	eax,1h
	nop	dword ptr [rax+0h]

l000000000040E190:
	movzx	edi,byte ptr [r15+rdx]
	lea	ecx,[rdi-5Bh]
	cmp	cl,21h
	ja	40E1B6h

l000000000040E19D:
	mov	rsi,rax
	mov	rdi,20000002Bh
	shl	rsi,cl
	test	rsi,rdi
	jnz	40E260h

l000000000040E1B6:
	add	rdx,1h
	cmp	rdx,rbp
	jnz	40E190h

l000000000040E1BF:
	mov	edi,[rsp+0ACh]
	call	402840h
	lea	rdi,[rsp+0B0h]
	test	eax,eax
	mov	eax,0h
	cmovz	r12d,eax

l000000000040E1DE:
	add	rbx,rbp
	call	402830h
	test	eax,eax
	jz	40E11Dh

l000000000040E1EE:
	mov	esi,r12d
	movzx	r11d,byte ptr [rsp+97h]
	movzx	r12d,byte ptr [rsp+96h]
	mov	rax,rbx
	mov	rbp,[rsp+48h]
	mov	r9,[rsp+88h]
	mov	rbx,[rsp+80h]
	mov	r14,[rsp+50h]
	mov	edx,esi
	mov	r15,[rsp+40h]
	mov	r8,r13
	xor	edx,1h

l000000000040E22D:
	cmp	rax,1h
	jbe	40DE61h

l000000000040E237:
	and	dl,[rsp+20h]
	jmp	40DE6Bh
000000000040E240 F6 84 24 90 00 00 00 01 0F 84 AA F8 FF FF 48 83 ..$...........H.
000000000040E250 C5 01 E9 2F F7 FF FF 66 0F 1F 84 00 00 00 00 00 .../...f........

l000000000040E260:
	mov	r11,[rsp+50h]
	mov	r15,[rsp+40h]
	jmp	40DC46h
000000000040E26F                                              90                .
000000000040E270 41 80 78 01 00 0F 95 C0 E9 94 F9 FF FF 0F 1F 00 A.x.............

l000000000040E280:
	call	402220h

l000000000040E285:
	mov	rdi,r8
	mov	[rsp+50h],r9
	mov	[rsp+48h],r11d
	mov	[rsp+40h],r8
	call	402380h
	mov	r9,[rsp+50h]
	mov	r15,rax
	mov	r11d,[rsp+48h]
	mov	r8,[rsp+40h]
	jmp	40E0D6h

l000000000040E2B3:
	mov	byte ptr [rsp+20h],1h
	mov	r14d,1h
	mov	qword ptr [rsp+60h],+415FEBh
	mov	ebx,1h
	jmp	40D960h

l000000000040E2D1:
	mov	byte ptr [rsp+20h],0h
	mov	r14d,1h
	mov	qword ptr [rsp+60h],+416DEAh
	mov	ebx,1h
	jmp	40D960h
000000000040E2EF                                              80                .
000000000040E2F0 7C 24 33 00 0F 84 C6 F9 FF FF E9 41 F9 FF FF 90 |$3........A....
000000000040E300 48 8B 74 24 28 C6 04 06 30 E9 5A FA FF FF       H.t$(...0.Z...  

l000000000040E30E:
	mov	edx,r12d
	mov	rax,rbx
	mov	rbp,[rsp+48h]
	mov	r9,[rsp+88h]
	movzx	r11d,byte ptr [rsp+97h]
	mov	r8,r13
	mov	rbx,[rsp+80h]
	movzx	r12d,byte ptr [rsp+96h]
	xor	edx,1h
	mov	r14,[rsp+50h]
	mov	r15,[rsp+40h]
	jmp	40E22Dh

l000000000040E350:
	mov	rax,rbx
	mov	rbp,[rsp+48h]
	mov	r9,[rsp+88h]
	movzx	r12d,byte ptr [rsp+96h]
	movzx	r11d,byte ptr [rsp+97h]
	mov	r8,r13
	mov	rbx,[rsp+80h]
	mov	r14,[rsp+50h]
	mov	edx,1h
	mov	r15,[rsp+40h]
	jmp	40E22Dh

l000000000040E391:
	mov	r10,r15
	mov	r15,[rsp+40h]
	mov	rsi,r14
	mov	rax,rbx
	mov	r8,r13
	mov	rbp,[rsp+48h]
	mov	r9,[rsp+88h]
	movzx	r12d,byte ptr [rsp+96h]
	cmp	r15,rsi
	movzx	r11d,byte ptr [rsp+97h]
	mov	rbx,[rsp+80h]
	mov	r14,[rsp+50h]
	mov	r13,[rsp+98h]
	jbe	40E3FEh

l000000000040E3DB:
	cmp	byte ptr [r10],0h
	jnz	40E3F0h

l000000000040E3E1:
	jmp	40E3FEh
000000000040E3E3          0F 1F 44 00 00                            ..D..        

l000000000040E3E8:
	cmp	byte ptr [r13+rax+0h],0h
	jz	40E3FEh

l000000000040E3F0:
	add	rax,1h
	lea	rdx,[rbp+rax+0h]
	cmp	r15,rdx
	ja	40E3E8h

l000000000040E3FE:
	mov	edx,1h
	jmp	40E22Dh

l000000000040E408:
	mov	byte ptr [rsp+20h],0h
	mov	r14d,1h
	mov	qword ptr [rsp+60h],+416DEAh
	xor	ebx,ebx
	jmp	40D960h

l000000000040E423:
	call	4023A0h

l000000000040E428:
	mov	byte ptr [rsp+20h],1h
	mov	r14d,1h
	mov	qword ptr [rsp+60h],+415FEBh
	xor	ebx,ebx
	jmp	40D960h
000000000040E443          66 66 66 66 2E 0F 1F 84 00 00 00 00 00    ffff.........

;; fn000000000040E450: 000000000040E450
;;   Called from:
;;     000000000040E941 (in fn000000000040E930)
;;     000000000040E992 (in fn000000000040E970)
;;     000000000040EA8C (in fn000000000040EAB0)
;;     000000000040EBFC (in fn000000000040EBF0)
;;     000000000040EC21 (in fn000000000040EC10)
fn000000000040E450 proc
	push	r15
	movsxd	r15,edi
	push	r14
	push	r13
	push	r12
	push	rbp
	push	rbx
	mov	rbx,rcx
	sub	rsp,48h
	mov	[rsp+20h],rsi
	mov	[rsp+28h],rdx
	call	402230h
	mov	r13,rax
	mov	eax,[rax]
	test	r15d,r15d
	mov	r12,[000000000061A5D8]                                 ; [rip+0020C156]
	mov	[rsp+34h],eax
	js	40E5F7h

l000000000040E48C:
	cmp	r15d,[000000000061A5F0]                                ; [rip+0020C15D]
	jc	40E4FAh

l000000000040E495:
	lea	ebp,[r15+1h]
	mov	r14d,ebp
	mov	rsi,r14
	shl	rsi,4h
	cmp	r12,+61A5E0h
	jnz	40E5E0h

l000000000040E4B0:
	xor	edi,edi
	call	410C90h
	mov	rsi,[000000000061A5E0]                                 ; [rip+0020C122]
	mov	rdi,[000000000061A5E8]                                 ; [rip+0020C123]
	mov	r12,rax
	mov	[000000000061A5D8],rax                                 ; [rip+0020C109]
	mov	[rax],rsi
	mov	[rax+8h],rdi

l000000000040E4D6:
	mov	edi,[000000000061A5F0]                                 ; [rip+0020C114]
	mov	rdx,r14
	xor	esi,esi
	sub	rdx,rdi
	shl	rdi,4h
	shl	rdx,4h
	add	rdi,r12
	call	402480h
	mov	[000000000061A5F0],ebp                                 ; [rip+0020C0F6]

l000000000040E4FA:
	mov	rax,[rbx+30h]
	shl	r15,4h
	mov	ebp,[rbx+4h]
	add	r12,r15
	mov	r8d,[rbx]
	lea	r15,[rbx+8h]
	mov	r11,[r12]
	mov	r14,[r12+8h]
	mov	[rsp+10h],rax
	mov	rax,[rbx+28h]
	or	ebp,1h
	mov	rcx,[rsp+28h]
	mov	rdx,[rsp+20h]
	mov	r9d,ebp
	mov	rsi,r11
	mov	[rsp],r15
	mov	rdi,r14
	mov	[rsp+8h],rax
	mov	[rsp+38h],r11
	call	40D8A0h
	mov	r11,[rsp+38h]
	cmp	r11,rax
	ja	40E5BFh

l000000000040E554:
	lea	rsi,[rax+1h]
	cmp	r14,+61B220h
	mov	[r12],rsi
	jz	40E577h

l000000000040E565:
	mov	rdi,r14
	mov	[rsp+38h],rsi
	call	4021F0h
	mov	rsi,[rsp+38h]

l000000000040E577:
	mov	rdi,rsi
	mov	[rsp+38h],rsi
	call	410C40h
	mov	[r12+8h],rax
	mov	r14,rax
	mov	rax,[rbx+30h]
	mov	r8d,[rbx]
	mov	rcx,[rsp+28h]
	mov	r9d,ebp
	mov	rdx,[rsp+20h]
	mov	rsi,[rsp+38h]
	mov	rdi,r14
	mov	[rsp+10h],rax
	mov	rax,[rbx+28h]
	mov	[rsp],r15
	mov	[rsp+8h],rax
	call	40D8A0h

l000000000040E5BF:
	mov	eax,[rsp+34h]
	mov	[r13+0h],eax
	add	rsp,48h
	mov	rax,r14
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040E5D9                            0F 1F 80 00 00 00 00          .......

l000000000040E5E0:
	mov	rdi,r12
	call	410C90h
	mov	r12,rax
	mov	[000000000061A5D8],rax                                 ; [rip+0020BFE6]
	jmp	40E4D6h

l000000000040E5F7:
	call	402220h
000000000040E5FC                                     0F 1F 40 00             ..@.

;; fn000000000040E600: 000000000040E600
;;   Called from:
;;     000000000040324B (in fn00000000004028C0)
;;     00000000004032AB (in fn00000000004028C0)
fn000000000040E600 proc
	push	r12
	push	rbp
	mov	rbp,rdi
	push	rbx
	call	402230h
	mov	r12d,[rax]
	test	rbp,rbp
	mov	edi,61B320h
	mov	rbx,rax
	cmovnz	rdi,rbp

l000000000040E61E:
	mov	esi,38h
	call	410E00h
	mov	[rbx],r12d
	pop	rbx
	pop	rbp
	pop	r12
	ret

;; fn000000000040E630: 000000000040E630
;;   Called from:
;;     000000000040325A (in fn00000000004028C0)
;;     0000000000404331 (in fn00000000004028C0)
fn000000000040E630 proc
	test	rdi,rdi
	mov	eax,61B320h
	cmovnz	rax,rdi

l000000000040E63C:
	mov	eax,[rax]
	ret
000000000040E63F                                              90                .

;; fn000000000040E640: 000000000040E640
;;   Called from:
;;     00000000004029C0 (in fn00000000004028C0)
;;     0000000000402A7A (in fn00000000004028C0)
;;     0000000000402C36 (in fn00000000004028C0)
;;     0000000000402CDE (in fn00000000004028C0)
;;     0000000000402CEC (in fn00000000004028C0)
;;     0000000000402F46 (in fn00000000004028C0)
;;     0000000000403214 (in fn00000000004028C0)
fn000000000040E640 proc
	mov	eax,61B320h
	test	rdi,rdi
	cmovnz	rax,rdi

l000000000040E64C:
	mov	[rax],esi
	ret
000000000040E64F                                              90                .

;; fn000000000040E650: 000000000040E650
;;   Called from:
;;     000000000040329C (in fn00000000004028C0)
;;     00000000004032C4 (in fn00000000004028C0)
;;     000000000040427D (in fn00000000004028C0)
fn000000000040E650 proc
	test	rdi,rdi
	mov	eax,61B320h
	mov	ecx,esi
	cmovnz	rax,rdi

l000000000040E65E:
	shr	sil,5h
	and	ecx,1Fh
	movzx	esi,sil
	lea	rsi,[rax+rsi*4]
	mov	edi,[rsi+8h]
	mov	eax,edi
	shr	eax,cl
	xor	edx,eax
	and	eax,1h
	and	edx,1h
	shl	edx,cl
	xor	edx,edi
	mov	[rsi+8h],edx
	ret
000000000040E684             66 66 66 2E 0F 1F 84 00 00 00 00 00     fff.........
000000000040E690 48 85 FF B8 20 B3 61 00 48 0F 44 F8 8B 47 04 89 H... .a.H.D..G..
000000000040E6A0 77 04 C3 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 w..ffff.........

;; fn000000000040E6B0: 000000000040E6B0
fn000000000040E6B0 proc
	sub	rsp,8h
	mov	eax,61B320h
	test	rdi,rdi
	cmovz	rdi,rax

l000000000040E6C0:
	test	rsi,rsi
	mov	dword ptr [rdi],8h
	jz	40E6DDh

l000000000040E6CB:
	test	rdx,rdx
	jz	40E6DDh

l000000000040E6D0:
	mov	[rdi+28h],rsi
	mov	[rdi+30h],rdx
	add	rsp,8h
	ret

l000000000040E6DD:
	call	402220h
000000000040E6E2       66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00   fffff.........

;; fn000000000040E6F0: 000000000040E6F0
;;   Called from:
;;     0000000000405320 (in fn00000000004052D0)
;;     00000000004055C7 (in fn00000000004052D0)
fn000000000040E6F0 proc
	push	r15
	mov	eax,61B320h
	mov	r15,rcx
	push	r14
	mov	r14,rdx
	push	r13
	mov	r13,rsi
	push	r12
	push	rbp
	push	rbx
	mov	rbx,r8
	sub	rsp,28h
	test	r8,r8
	cmovz	rbx,rax

l000000000040E716:
	mov	[rsp+18h],rdi
	call	402230h
	mov	r12d,[rax]
	mov	rbp,rax
	mov	rax,[rbx+30h]
	mov	r9d,[rbx+4h]
	mov	rdi,[rsp+18h]
	mov	rcx,r15
	mov	rdx,r14
	mov	rsi,r13
	mov	[rsp+10h],rax
	mov	rax,[rbx+28h]
	mov	[rsp+8h],rax
	lea	rax,[rbx+8h]
	mov	[rsp],rax
	mov	r8d,[rbx]
	call	40D8A0h
	mov	[rbp+0h],r12d
	add	rsp,28h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040E76D                                        0F 1F 00              ...
000000000040E770 41 57 B8 20 B3 61 00 41 56 49 89 F6 41 55 49 89 AW. .a.AVI..AUI.
000000000040E780 FD 41 54 49 89 D4 55 53 48 89 CB 48 83 EC 48 48 .ATI..USH..H..HH
000000000040E790 85 C9 48 0F 44 D8 31 ED E8 93 3A FF FF 49 89 C7 ..H.D.1...:..I..
000000000040E7A0 8B 00 4D 85 E4 40 0F 94 C5 0B 6B 04 4C 8D 53 08 ..M..@....k.L.S.
000000000040E7B0 4C 89 F1 4C 89 EA 31 F6 89 44 24 1C 48 8B 43 30 L..L..1..D$.H.C0
000000000040E7C0 31 FF 4C 89 54 24 30 41 89 E9 48 89 44 24 10 48 1.L.T$0A..H.D$.H
000000000040E7D0 8B 43 28 4C 89 14 24 48 89 44 24 08 44 8B 03 E8 .C(L..$H.D$.D...
000000000040E7E0 BC F0 FF FF 48 8D 70 01 48 89 44 24 38 48 89 F7 ....H.p.H.D$8H..
000000000040E7F0 48 89 74 24 28 E8 46 24 00 00 48 89 C7 48 89 44 H.t$(.F$..H..H.D
000000000040E800 24 20 48 8B 43 30 4C 8B 54 24 30 48 8B 74 24 28 $ H.C0L.T$0H.t$(
000000000040E810 41 89 E9 4C 89 F1 4C 89 EA 48 89 44 24 10 48 8B A..L..L..H.D$.H.
000000000040E820 43 28 4C 89 14 24 48 89 44 24 08 44 8B 03 E8 6D C(L..$H.D$.D...m
000000000040E830 F0 FF FF 8B 44 24 1C 4D 85 E4 41 89 07 74 09 4C ....D$.M..A..t.L
000000000040E840 8B 5C 24 38 4D 89 1C 24 48 8B 44 24 20 48 83 C4 .\$8M..$H.D$ H..
000000000040E850 48 5B 5D 41 5C 41 5D 41 5E 41 5F C3 0F 1F 40 00 H[]A\A]A^A_...@.
000000000040E860 48 89 D1 31 D2 E9 06 FF FF FF 66 0F 1F 44 00 00 H..1......f..D..
000000000040E870 41 54 8B 05 78 BD 20 00 4C 8B 25 59 BD 20 00 55 AT..x. .L.%Y. .U
000000000040E880 83 F8 01 53 76 24 83 E8 02 4C 89 E3 48 C1 E0 04 ...Sv$...L..H...
000000000040E890 49 8D 6C 04 10 0F 1F 00 48 8B 7B 18 48 83 C3 10 I.l.....H.{.H...
000000000040E8A0 E8 4B 39 FF FF 48 39 EB 75 EE 49 8B 7C 24 08 48 .K9..H9.u.I.|$.H
000000000040E8B0 81 FF 20 B2 61 00 74 1B E8 33 39 FF FF 48 C7 05 .. .a.t..39..H..
000000000040E8C0 18 BD 20 00 00 01 00 00 48 C7 05 15 BD 20 00 20 .. .....H.... . 
000000000040E8D0 B2 61 00 49 81 FC E0 A5 61 00 74 13 4C 89 E7 E8 .a.I....a.t.L...
000000000040E8E0 0C 39 FF FF 48 C7 05 E9 BC 20 00 E0 A5 61 00 5B .9..H.... ...a.[
000000000040E8F0 5D C7 05 F5 BC 20 00 01 00 00 00 41 5C C3 66 90 ].... .....A\.f.
000000000040E900 B9 20 B3 61 00 48 C7 C2 FF FF FF FF E9 3F FB FF . .a.H.......?..
000000000040E910 FF 66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 .ffffff.........
000000000040E920 B9 20 B3 61 00 E9 26 FB FF FF 66 0F 1F 44 00 00 . .a..&...f..D..

;; fn000000000040E930: 000000000040E930
;;   Called from:
;;     0000000000403107 (in fn00000000004028C0)
;;     0000000000403789 (in fn00000000004028C0)
;;     0000000000403A96 (in fn00000000004028C0)
;;     0000000000403C04 (in fn00000000004028C0)
;;     00000000004041BE (in fn00000000004028C0)
;;     000000000040485C (in fn00000000004028C0)
fn000000000040E930 proc
	mov	rsi,rdi
	mov	ecx,61B320h
	mov	rdx,-1h
	xor	edi,edi
	jmp	40E450h
000000000040E946                   66 2E 0F 1F 84 00 00 00 00 00       f.........
000000000040E950 48 89 F2 B9 20 B3 61 00 48 89 FE 31 FF E9 EE FA H... .a.H..1....
000000000040E960 FF FF 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 ..fffff.........

;; fn000000000040E970: 000000000040E970
;;   Called from:
;;     0000000000409FBE (in fn0000000000409F80)
fn000000000040E970 proc
	push	rbp
	mov	rbp,rdx
	push	rbx
	mov	ebx,edi
	sub	rsp,48h
	mov	rdi,rsp
	call	40D740h
	mov	rcx,rsp
	mov	rsi,rbp
	mov	edi,ebx
	mov	rdx,-1h
	call	40E450h
	add	rsp,48h
	pop	rbx
	pop	rbp
	ret
000000000040E99E                                           66 90               f.
000000000040E9A0 41 54 49 89 CC 55 48 89 D5 53 89 FB 48 83 EC 40 ATI..UH..S..H..@
000000000040E9B0 48 89 E7 E8 88 ED FF FF 48 89 E1 4C 89 E2 48 89 H.......H..L..H.
000000000040E9C0 EE 89 DF E8 88 FA FF FF 48 83 C4 40 5B 5D 41 5C ........H..@[]A\
000000000040E9D0 C3 66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 .ffffff.........
000000000040E9E0 48 89 F2 89 FE 31 FF E9 84 FF FF FF 0F 1F 40 00 H....1........@.
000000000040E9F0 48 89 D1 48 89 F2 89 FE 31 FF E9 A1 FF FF FF 90 H..H....1.......

l000000000040EA00:
	sub	rsp,48h
	mov	rax,[000000000061B320]                                 ; [rip+0020C915]
	mov	r8d,edx
	shr	r8b,5h
	mov	ecx,edx
	mov	rdx,rsi
	movzx	r8d,r8b
	and	ecx,1Fh
	mov	rsi,rdi
	mov	[rsp],rax
	mov	rax,[000000000061B328]                                 ; [rip+0020C8FC]
	xor	edi,edi
	mov	[rsp+8h],rax
	mov	rax,[000000000061B330]                                 ; [rip+0020C8F6]
	mov	[rsp+10h],rax
	mov	rax,[000000000061B338]                                 ; [rip+0020C8F2]
	mov	[rsp+18h],rax
	mov	rax,[000000000061B340]                                 ; [rip+0020C8EE]
	mov	[rsp+20h],rax
	mov	rax,[000000000061B348]                                 ; [rip+0020C8EA]
	mov	r9d,[rsp+r8*4+8h]
	mov	[rsp+28h],rax
	mov	rax,[000000000061B350]                                 ; [rip+0020C8E1]
	mov	[rsp+30h],rax
	mov	eax,r9d
	shr	eax,cl
	xor	eax,1h
	and	eax,1h
	shl	eax,cl
	mov	rcx,rsp
	xor	eax,r9d
	mov	[rsp+r8*4+8h],eax
	call	40E450h
	add	rsp,48h
	ret
000000000040EA96                   66 2E 0F 1F 84 00 00 00 00 00       f.........
000000000040EAA0 40 0F BE D6 48 C7 C6 FF FF FF FF E9 50 FF FF FF @...H.......P...

;; fn000000000040EAB0: 000000000040EAB0
;;   Called from:
;;     0000000000403EED (in fn00000000004028C0)
;;     000000000040581D (in fn0000000000405810)
;;     00000000004084C2 (in fn0000000000407EA0)
fn000000000040EAB0 proc
	mov	edx,3Ah
	mov	rsi,-1h
	jmp	40EA00h
000000000040EAC1    66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00  ffffff.........
000000000040EAD0 BA 3A 00 00 00 E9 26 FF FF FF 66 0F 1F 44 00 00 .:....&...f..D..
000000000040EAE0 41 54 4D 89 C4 55 48 89 CD 53 89 FB 48 83 EC 40 ATM..UH..S..H..@
000000000040EAF0 48 8B 05 29 C8 20 00 48 89 E7 48 89 04 24 48 8B H..). .H..H..$H.
000000000040EB00 05 23 C8 20 00 48 89 44 24 08 48 8B 05 1F C8 20 .#. .H.D$.H.... 
000000000040EB10 00 48 89 44 24 10 48 8B 05 1B C8 20 00 48 89 44 .H.D$.H.... .H.D
000000000040EB20 24 18 48 8B 05 17 C8 20 00 48 89 44 24 20 48 8B $.H.... .H.D$ H.
000000000040EB30 05 13 C8 20 00 48 89 44 24 28 48 8B 05 0F C8 20 ... .H.D$(H.... 
000000000040EB40 00 48 89 44 24 30 E8 65 FB FF FF 48 89 E1 4C 89 .H.D$0.e...H..L.
000000000040EB50 E2 48 89 EE 89 DF E8 F5 F8 FF FF 48 83 C4 40 5B .H.........H..@[
000000000040EB60 5D 41 5C C3 66 66 66 2E 0F 1F 84 00 00 00 00 00 ]A\.fff.........
000000000040EB70 49 C7 C0 FF FF FF FF E9 64 FF FF FF 0F 1F 40 00 I.......d.....@.
000000000040EB80 48 89 D1 49 C7 C0 FF FF FF FF 48 89 F2 48 89 FE H..I......H..H..
000000000040EB90 31 FF E9 49 FF FF FF 66 0F 1F 84 00 00 00 00 00 1..I...f........
000000000040EBA0 49 89 C8 48 89 D1 48 89 F2 48 89 FE 31 FF E9 2D I..H..H..H..1..-
000000000040EBB0 FF FF FF 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 ...ffff.........
000000000040EBC0 B9 A0 A5 61 00 E9 86 F8 FF FF 66 0F 1F 44 00 00 ...a......f..D..
000000000040EBD0 48 89 F2 B9 A0 A5 61 00 48 89 FE 31 FF E9 6E F8 H.....a.H..1..n.
000000000040EBE0 FF FF 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00 ..fffff.........

;; fn000000000040EBF0: 000000000040EBF0
;;   Called from:
;;     0000000000409FAC (in fn0000000000409F80)
fn000000000040EBF0 proc
	mov	ecx,61A5A0h
	mov	rdx,-1h
	jmp	40E450h
000000000040EC01    66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00  ffffff.........

;; fn000000000040EC10: 000000000040EC10
;;   Called from:
;;     000000000040440B (in fn00000000004028C0)
;;     000000000040A05A (in fn000000000040A000)
;;     000000000040A0A7 (in fn000000000040A000)
fn000000000040EC10 proc
	mov	rsi,rdi
	mov	ecx,61A5A0h
	mov	rdx,-1h
	xor	edi,edi
	jmp	40E450h
000000000040EC26                   66 2E 0F 1F 84 00 00 00 00 00       f.........

;; fn000000000040EC30: 000000000040EC30
fn000000000040EC30 proc
	push	r12
	test	rdx,rdx
	push	rbp
	mov	rbp,rdi
	push	rbx
	lea	rbx,[rdx-1h]
	jz	40EC69h

l000000000040EC40:
	mov	r12,rsi
	call	402870h
	nop	dword ptr [rax+rax+0h]

l000000000040EC50:
	movzx	ecx,byte ptr [r12+rbx]
	mov	rdx,[rax]
	mov	edx,[rdx+rcx*4]
	mov	[rbp+rbx+0h],dl
	sub	rbx,1h
	cmp	rbx,0FFh
	jnz	40EC50h

l000000000040EC69:
	pop	rbx
	mov	rax,rbp
	pop	rbp
	pop	r12
	ret
000000000040EC71    66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00  ffffff.........

;; fn000000000040EC80: 000000000040EC80
;;   Called from:
;;     000000000040F15C (in fn000000000040ECD0)
fn000000000040EC80 proc
	push	r12
	test	rdx,rdx
	push	rbp
	mov	rbp,rdi
	push	rbx
	lea	rbx,[rdx-1h]
	jz	40ECB9h

l000000000040EC90:
	mov	r12,rsi
	call	4021A0h
	nop	dword ptr [rax+rax+0h]

l000000000040ECA0:
	movzx	ecx,byte ptr [r12+rbx]
	mov	rdx,[rax]
	mov	edx,[rdx+rcx*4]
	mov	[rbp+rbx+0h],dl
	sub	rbx,1h
	cmp	rbx,0FFh
	jnz	40ECA0h

l000000000040ECB9:
	pop	rbx
	mov	rax,rbp
	pop	rbp
	pop	r12
	ret
000000000040ECC1    66 66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00  ffffff.........

;; fn000000000040ECD0: 000000000040ECD0
;;   Called from:
;;     0000000000410619 (in fn0000000000410600)
fn000000000040ECD0 proc
	push	r15
	mov	r10,rdx
	push	r14
	push	r13
	push	r12
	mov	r12,rsi
	push	rbp
	push	rbx
	mov	rbx,rcx
	sub	rsp,+4D8h
	mov	rax,[r8+30h]
	mov	rsi,fs:[0028h]
	mov	[rsp+4C8h],rsi
	xor	esi,esi
	mov	esi,[r8+8h]
	mov	[rsp+10h],r8
	mov	[rsp+44h],r9d
	mov	[rsp+0Fh],dil
	mov	[rsp+48h],rax
	cmp	esi,0Ch
	mov	[rsp+40h],esi
	jle	40EEE0h

l000000000040ED26:
	sub	dword ptr [rsp+40h],0Ch

l000000000040ED2B:
	movzx	eax,byte ptr [rbx]
	xor	r13d,r13d
	test	al,al
	jz	40F053h

l000000000040ED39:
	lea	rsi,[rsp+0C1h]
	mov	r14,r10
	mov	[rsp+20h],rsi
	jmp	40ED85h
000000000040ED4B                                  0F 1F 44 00 00            ..D..

l000000000040ED50:
	mov	rdx,r14
	sub	rdx,r13
	cmp	rdx,1h
	jbe	40EE98h

l000000000040ED60:
	test	r12,r12
	jz	40ED6Dh

l000000000040ED65:
	mov	[r12],al
	add	r12,1h

l000000000040ED6D:
	add	r13,1h
	mov	r8,rbx

l000000000040ED74:
	movzx	eax,byte ptr [r8+1h]
	lea	rbx,[r8+1h]
	test	al,al
	jz	40F050h

l000000000040ED85:
	cmp	al,25h
	jnz	40ED50h

l000000000040ED89:
	movzx	r9d,byte ptr [rsp+0Fh]
	xor	eax,eax
	xor	r11d,r11d

l000000000040ED94:
	add	rbx,1h
	movzx	edi,byte ptr [rbx]
	cmp	dil,30h
	jz	40EDC0h

l000000000040EDA1:
	jg	40EDD0h

l000000000040EDA3:
	cmp	dil,23h
	jnz	40EDE8h

l000000000040EDA9:
	add	rbx,1h
	movzx	edi,byte ptr [rbx]
	mov	eax,1h
	cmp	dil,30h
	jnz	40EDA1h

l000000000040EDBB:
	nop	dword ptr [rax+rax+0h]

l000000000040EDC0:
	movsx	r11d,dil
	jmp	40ED94h
000000000040EDC6                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l000000000040EDD0:
	cmp	dil,5Eh
	jnz	40EE70h

l000000000040EDDA:
	mov	r9d,1h
	jmp	40ED94h
000000000040EDE2       66 0F 1F 44 00 00                           f..D..        

l000000000040EDE8:
	cmp	dil,2Dh
	jz	40EDC0h

l000000000040EDEE:
	movsx	edx,dil
	mov	ebp,0FFFFFFFFh
	sub	edx,30h
	cmp	edx,9h
	ja	40EE40h

l000000000040EDFF:
	xor	ebp,ebp
	jmp	40EE28h
000000000040EE03          0F 1F 44 00 00                            ..D..        

l000000000040EE08:
	movsx	edx,byte ptr [rbx]
	jz	40EE88h

l000000000040EE0D:
	lea	ecx,[rbp+rbp*4+0h]
	lea	ebp,[rdx+rcx*2-30h]

l000000000040EE15:
	add	rbx,1h
	movzx	edi,byte ptr [rbx]
	movsx	edx,dil
	sub	edx,30h
	cmp	edx,9h
	ja	40EE40h

l000000000040EE28:
	cmp	ebp,0CCCCCCCh
	jle	40EE08h

l000000000040EE30:
	mov	ebp,7FFFFFFFh
	jmp	40EE15h
000000000040EE37                      66 0F 1F 84 00 00 00 00 00        f........

l000000000040EE40:
	cmp	dil,45h
	jz	40EEC8h

l000000000040EE4A:
	xor	ecx,ecx
	cmp	dil,4Fh
	jz	40EEC8h

l000000000040EE52:
	cmp	dil,7Ah
	movsx	esi,dil
	ja	40FF4Dh

l000000000040EE60:
	movzx	edx,dil
	jmp	qword ptr [4164E8h+rdx*8]
000000000040EE6B                                  0F 1F 44 00 00            ..D..

l000000000040EE70:
	cmp	dil,5Fh
	jnz	40EDEEh

l000000000040EE7A:
	movsx	r11d,dil
	jmp	40ED94h
000000000040EE83          0F 1F 44 00 00                            ..D..        

l000000000040EE88:
	cmp	dl,37h
	jle	40EE0Dh

l000000000040EE8D:
	mov	ebp,7FFFFFFFh
	jmp	40EE15h
000000000040EE94             0F 1F 40 00                             ..@.        

l000000000040EE98:
	xor	eax,eax

l000000000040EE9A:
	mov	rsi,[rsp+4C8h]
	xor	rsi,fs:[0028h]
	jnz	4105EAh

l000000000040EEB1:
	add	rsp,+4D8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
000000000040EEC3          0F 1F 44 00 00                            ..D..        

l000000000040EEC8:
	movsx	ecx,dil
	add	rbx,1h
	movzx	edi,byte ptr [rbx]
	jmp	40EE52h
000000000040EED8                         0F 1F 84 00 00 00 00 00         ........

l000000000040EEE0:
	mov	esi,[rsp+40h]
	mov	eax,0Ch
	test	esi,esi
	cmovnz	eax,esi

l000000000040EEEE:
	mov	[rsp+40h],eax
	jmp	40ED2Bh
000000000040EEF7                      66 0F 1F 84 00 00 00 00 00        f........
000000000040EF00 83 F9 4F 0F 84 AF 01 00 00 45 31 FF 85 C9 C6 84 ..O......E1.....
000000000040EF10 24 B0 00 00 00 20 C6 84 24 B1 00 00 00 25 0F 85 $.... ..$....%..
000000000040EF20 CB 16 00 00 48 8D 84 24 B2 00 00 00 49 89 D8 C7 ....H..$....I...
000000000040EF30 44 24 18 00 00 00 00 48 8B 4C 24 10 40 88 38 48 D$.....H.L$.@.8H
000000000040EF40 8D 94 24 B0 00 00 00 48 8D BC 24 C0 00 00 00 C6 ..$....H..$.....
000000000040EF50 40 01 00 BE 00 04 00 00 44 89 4C 24 38 44 89 5C @.......D.L$8D.\
000000000040EF60 24 30 4C 89 44 24 28 E8 D4 37 FF FF 48 85 C0 4C $0L.D$(..7..H..L
000000000040EF70 8B 44 24 28 44 8B 5C 24 30 44 8B 4C 24 38 0F 84 .D$(D.\$0D.L$8..
000000000040EF80 F0 FD FF FF 48 8D 58 FF 31 C0 85 ED 0F 49 C5 4C ....H.X.1....I.L
000000000040EF90 89 F2 48 98 48 89 D9 48 39 D8 48 0F 43 C8 4C 29 ..H.H..H9.H.C.L)
000000000040EFA0 EA 48 39 D1 0F 83 EE FE FF FF 4D 85 E4 0F 84 87 .H9.......M.....
000000000040EFB0 00 00 00 48 39 C3 73 49 8B 44 24 18 85 C0 75 41 ...H9.sI.D$...uA
000000000040EFC0 48 63 ED 48 89 4C 24 30 44 89 4C 24 28 48 29 DD Hc.H.L$0D.L$(H).
000000000040EFD0 41 83 FB 30 4C 89 44 24 18 48 89 EA 0F 84 57 12 A..0L.D$.H....W.
000000000040EFE0 00 00 4C 89 E7 BE 20 00 00 00 49 01 EC E8 8E 34 ..L... ...I....4
000000000040EFF0 FF FF 48 8B 4C 24 30 44 8B 4C 24 28 4C 8B 44 24 ..H.L$0D.L$(L.D$
000000000040F000 18 45 84 FF 48 89 4C 24 28 4C 89 44 24 18 48 89 .E..H.L$(L.D$.H.
000000000040F010 DA 48 8B 74 24 20 4C 89 E7 0F 85 8E 01 00 00 45 .H.t$ L........E
000000000040F020 84 C9 0F 84 71 01 00 00 E8 53 FC FF FF 4C 8B 44 ....q....S...L.D
000000000040F030 24 18 48 8B 4C 24 28 49 01 DC 41 0F B6 40 01 49 $.H.L$(I..A..@.I
000000000040F040 01 CD 49 8D 58 01 84 C0 0F 85 37 FD FF FF 66 90 ..I.X.....7...f.

l000000000040F050:
	mov	r10,r14

l000000000040F053:
	test	r12,r12
	jz	40F191h

l000000000040F05C:
	test	r10,r10
	jz	40F191h

l000000000040F065:
	mov	byte ptr [r12],0h
	mov	rax,r13
	jmp	40EE9Ah
000000000040F072       66 0F 1F 44 00 00 85 C9 75 3C 84 C0 B8 01   f..D....u<....
000000000040F080 00 00 00 44 0F 45 C8 C6 84 24 B0 00 00 00 20 C6 ...D.E...$.... .
000000000040F090 84 24 B1 00 00 00 25 49 89 D8 45 31 FF C7 44 24 .$....%I..E1..D$
000000000040F0A0 18 00 00 00 00 48 8D 84 24 B2 00 00 00 E9 85 FE .....H..$.......
000000000040F0B0 FF FF                                           ..              

l000000000040F0B2:
	mov	rbx,r8
	nop	dword ptr [rax]
	lea	rax,[rbx-1h]
	mov	ecx,1h

l000000000040F0C1:
	mov	r15,rax
	lea	rax,[rax-1h]
	add	ecx,1h
	cmp	byte ptr [rax+1h],25h
	jnz	40F0C1h

l000000000040F0D1:
	movsxd	rcx,ecx
	mov	r8,rbx

l000000000040F0D7:
	xor	eax,eax
	test	ebp,ebp
	mov	rdx,r14
	cmovns	eax,ebp

l000000000040F0E1:
	cdqe
	cmp	rcx,rax
	mov	rbx,rax
	cmovnc	rbx,rcx

l000000000040F0ED:
	sub	rdx,r13
	cmp	rbx,rdx
	jnc	40EE98h

l000000000040F0F9:
	test	r12,r12
	jz	40F16Eh

l000000000040F0FE:
	cmp	rax,rcx
	jbe	40F144h

l000000000040F103:
	movsxd	rbp,ebp
	mov	[rsp+30h],rcx
	mov	[rsp+28h],r9d
	sub	rbp,rcx
	cmp	r11d,30h
	mov	[rsp+18h],r8
	mov	rdx,rbp
	jz	40F1C1h

l000000000040F125:
	mov	rdi,r12
	mov	esi,20h
	add	r12,rbp
	call	402480h
	mov	rcx,[rsp+30h]
	mov	r9d,[rsp+28h]
	mov	r8,[rsp+18h]

l000000000040F144:
	test	r9b,r9b
	mov	[rsp+28h],r8
	mov	rdx,rcx
	mov	[rsp+18h],rcx
	mov	rsi,r15
	mov	rdi,r12
	jz	40F180h

l000000000040F15C:
	call	40EC80h
	mov	rcx,[rsp+18h]
	mov	r8,[rsp+28h]

l000000000040F16B:
	add	r12,rcx

l000000000040F16E:
	add	r13,rbx
	jmp	40ED74h
000000000040F176                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l000000000040F180:
	call	4025C0h
	mov	r8,[rsp+28h]
	mov	rcx,[rsp+18h]
	jmp	40F16Bh

l000000000040F191:
	mov	rax,r13
	jmp	40EE9Ah
000000000040F199                            E8 22 34 FF FF 48 8B          ."4..H.
000000000040F1A0 4C 24 28 4C 8B 44 24 18 E9 8A FE FF FF E8 7E FA L$(L.D$.......~.
000000000040F1B0 FF FF 4C 8B 44 24 18 48 8B 4C 24 28 E9 76 FE FF ..L.D$.H.L$(.v..
000000000040F1C0 FF                                              .               

l000000000040F1C1:
	mov	rdi,r12
	mov	esi,30h
	add	r12,rbp
	call	402480h
	mov	r8,[rsp+18h]
	mov	r9d,[rsp+28h]
	mov	rcx,[rsp+30h]
	jmp	40F144h
000000000040F1E5                48 8B 44 24 10 BA 93 24 49 92 C7      H.D$...$I..
000000000040F1F0 44 24 18 01 00 00 00 8B 40 18 44 8D 40 06 44 89 D$......@.D.@.D.
000000000040F200 C0 F7 EA 44 89 C0 C1 F8 1F 44 01 C2 C1 FA 02 29 ...D.....D.....)
000000000040F210 C2 8D 04 D5 00 00 00 00 29 D0 41 29 C0 41 83 C0 ........).A).A..
000000000040F220 01 0F 1F 80 00 00 00 00 44 89 C0 31 FF C6 44 24 ........D..1..D$
000000000040F230 28 00 C1 E8 1F 41 89 C2 45 84 D2 0F 85 D7 02 00 (....A..E.......
000000000040F240 00 83 F9 4F 0F 85 CE 02 00 00 C6 84 24 B0 00 00 ...O........$...
000000000040F250 00 20 C6 84 24 B1 00 00 00 25 45 31 FF 88 8C 24 . ..$....%E1...$
000000000040F260 B2 00 00 00 89 F7 49 89 D8 48 8D 84 24 B3 00 00 ......I..H..$...
000000000040F270 00 E9 C1 FC FF FF 83 F9 45 0F 84 39 FE FF FF 48 ........E..9...H
000000000040F280 8B 44 24 10 C6 44 24 28 00 C7 44 24 18 03 00 00 .D$..D$(..D$....
000000000040F290 00 44 8B 40 1C 41 83 F8 FF 41 0F 9C C2 41 83 C0 .D.@.A...A...A..
000000000040F2A0 01 31 FF EB 93 83 F9 45 0F 84 0A FE FF FF 48 8B .1.....E......H.
000000000040F2B0 44 24 10 44 8B 40 0C 41 83 FB 2D C7 44 24 18 02 D$.D.@.A..-.D$..
000000000040F2C0 00 00 00 0F 84 5F FF FF FF 41 83 FB 30 0F 84 55 ....._...A..0..U
000000000040F2D0 FF FF FF 41 BB 5F 00 00 00 E9 4A FF FF FF 83 F9 ...A._....J.....
000000000040F2E0 45 0F 84 D1 FD FF FF 48 8B 44 24 10 C7 44 24 18 E......H.D$..D$.
000000000040F2F0 02 00 00 00 44 8B 40 0C E9 2B FF FF FF 45 31 FF ....D.@..+...E1.
000000000040F300 48 8B 44 24 10 8B 78 20 85 FF 0F 88 92 0F 00 00 H.D$..x ........
000000000040F310 4C 8B 50 28 BA C5 B3 A2 91 44 89 D0 F7 EA 44 89 L.P(.....D....D.
000000000040F320 D0 C1 F8 1F 89 44 24 18 46 8D 04 12 41 C1 F8 0B .....D$.F...A...
000000000040F330 41 29 C0 B8 89 88 88 88 41 F7 EA B8 89 88 88 88 A)......A.......
000000000040F340 42 8D 3C 12 C1 FF 05 2B 7C 24 18 F7 EF 8D 04 3A B.<....+|$.....:
000000000040F350 89 FA C1 FA 1F C1 F8 05 29 D0 BA 3C 00 00 00 0F ........)..<....
000000000040F360 AF C2 89 FA 29 C2 89 D0 BA 3C 00 00 00 0F AF FA ....)....<......
000000000040F370 44 89 D2 29 FA 49 83 FF 01 0F 84 D6 0F 00 00 0F D..).I..........
000000000040F380 82 6F 11 00 00 49 83 FF 02 0F 84 44 0F 00 00 49 .o...I.....D...I
000000000040F390 83 FF 03 0F 84 05 11 00 00 0F B6 3B 49 89 D8    ...........;I.. 

l000000000040F39F:
	cmp	dil,25h
	jnz	40F0B2h

l000000000040F3A9:
	mov	r15,r8
	mov	ecx,1h
	jmp	40F0D7h
000000000040F3B6                   84 C0 B8 01 00 00 00 44 0F 45       .......D.E
000000000040F3C0 C8 85 C9 0F 85 EF FC FF FF E9 B9 FC FF FF 83 F9 ................
000000000040F3D0 45 0F 84 E1 FC FF FF 48 8B 44 24 10 C7 44 24 18 E......H.D$..D$.
000000000040F3E0 01 00 00 00 44 8B 40 18 E9 3B FE FF FF 3C 01 48 ....D.@..;...<.H
000000000040F3F0 8B 7C 24 48 19 C9 83 C1 01 84 C0 B8 00 00 00 00 .|$H............
000000000040F400 44 0F 45 C8 48 85 FF 0F 84 35 11 00 00 89 4C 24 D.E.H....5....L$
000000000040F410 30 44 89 4C 24 28 44 89 5C 24 18 E8 60 2F FF FF 0D.L$(D.\$..`/..
000000000040F420 44 8B 5C 24 18 44 8B 4C 24 28 49 89 C7 8B 4C 24 D.\$.D.L$(I...L$
000000000040F430 30 31 C0 85 ED 4C 89 F2 0F 49 C5 48 98 49 39 C7 01...L...I.H.I9.
000000000040F440 49 89 C0 4D 0F 43 C7 4C 29 EA 49 39 D0 0F 83 45 I..M.C.L).I9...E
000000000040F450 FA FF FF 4D 85 E4 74 72 4C 39 F8 76 3F 48 63 ED ...M..trL9.v?Hc.
000000000040F460 4C 89 44 24 30 89 4C 24 28 4C 29 FD 41 83 FB 30 L.D$0.L$(L).A..0
000000000040F470 44 89 4C 24 18 48 89 EA 0F 84 ED 10 00 00 4C 89 D.L$.H........L.
000000000040F480 E7 BE 20 00 00 00 49 01 EC E8 F2 2F FF FF 4C 8B .. ...I..../..L.
000000000040F490 44 24 30 8B 4C 24 28 44 8B 4C 24 18 84 C9 4C 89 D$0.L$(D.L$...L.
000000000040F4A0 44 24 18 4C 89 FA 48 8B 74 24 48 4C 89 E7 0F 85 D$.L..H.t$HL....
000000000040F4B0 C9 0E 00 00 45 84 C9 0F 84 07 0E 00 00 E8 BE F7 ....E...........
000000000040F4C0 FF FF 4C 8B 44 24 18 4D 01 FC 4D 01 C5 49 89 D8 ..L.D$.M..M..I..
000000000040F4D0 E9 9F F8 FF FF 83 F9 45 0F 84 AF 0D 00 00 83 F9 .......E........
000000000040F4E0 4F 0F 84 D1 FB FF FF 48 8B 44 24 10 C7 44 24 18 O......H.D$..D$.
000000000040F4F0 04 00 00 00 44 8B 40 14 41 81 F8 94 F8 FF FF 41 ....D.@.A......A
000000000040F500 0F 9C C2 41 81 C0 6C 07 00 00 31 FF C6 44 24 28 ...A..l...1..D$(
000000000040F510 00 0F 1F 80 00 00 00 00 44 89 C0 48 8D 8C 24 D7 ........D..H..$.
000000000040F520 00 00 00 BE CD CC CC CC F7 D8 45 84 D2 44 0F 45 ..........E..D.E
000000000040F530 C0 EB 08 0F 1F 44 00 00 4C 89 F9 40 F6 C7 01 74 .....D..L..@...t
000000000040F540 08 C6 41 FF 3A 48 83 E9 01 44 89 C0 4C 8D 79 FF ..A.:H...D..L.y.
000000000040F550 F7 E6 C1 EA 03 8D 04 92 01 C0 41 29 C0 41 83 C0 ..........A).A..
000000000040F560 30 D1 FF 44 88 41 FF 41 89 D0 75 CC 85 D2 75 C8 0..D.A.A..u...u.
000000000040F570 8B 44 24 18 39 E8 0F 4C C5 45 84 D2 89 44 24 18 .D$.9..L.E...D$.
000000000040F580 0F 85 D2 09 00 00 80 7C 24 28 00 0F 85 AC 0A 00 .......|$(......
000000000040F590 00 41 83 FB 2D 0F 84 C2 0C 00 00 31 C0 C6 44 24 .A..-......1..D$
000000000040F5A0 30 00 45 31 D2 0F 1F 00 44 8B 44 24 18 48 8D B4 0.E1....D.D$.H..
000000000040F5B0 24 D7 00 00 00 48 89 74 24 28 41 29 C0 4C 89 F8 $....H.t$(A).L..
000000000040F5C0 48 29 F0 41 01 C0 45 85 C0 0F 8E 79 0A 00 00 41 H).A..E....y...A
000000000040F5D0 83 FB 5F 0F 84 29 0B 00 00 48 63 54 24 18 4C 89 .._..)...HcT$.L.
000000000040F5E0 F0 4C 29 E8 48 39 C2 0F 83 AB F8 FF FF 80 7C 24 .L).H9........|$
000000000040F5F0 30 00 0F 84 93 00 00 00 31 D2 85 ED B9 01 00 00 0.......1.......
000000000040F600 00 0F 49 D5 48 63 D2 48 85 D2 48 0F 45 CA 48 39 ..I.Hc.H..H.E.H9
000000000040F610 C8 0F 86 81 F8 FF FF 4D 85 E4 74 6C 48 83 FA 01 .......M..tlH...
000000000040F620 76 5E 8B 54 24 18 85 D2 75 56 48 63 ED 44 89 54 v^.T$...uVHc.D.T
000000000040F630 24 60 48 89 4C 24 58 48 83 ED 01 41 83 FB 30 44 $`H.L$XH...A..0D
000000000040F640 89 44 24 50 44 89 4C 24 38 44 89 5C 24 30 48 89 .D$PD.L$8D.\$0H.
000000000040F650 EA 0F 84 6F 0E 00 00 4C 89 E7 BE 20 00 00 00 49 ...o...L... ...I
000000000040F660 01 EC E8 19 2E FF FF 44 8B 54 24 60 48 8B 4C 24 .......D.T$`H.L$
000000000040F670 58 44 8B 44 24 50 44 8B 4C 24 38 44 8B 5C 24 30 XD.D$PD.L$8D.\$0
000000000040F680 45 88 14 24 49 83 C4 01 49 01 CD 4D 85 E4 49 63 E..$I...I..M..Ic
000000000040F690 E8 74 2A 49 63 E8 4C 89 E7 BE 30 00 00 00 48 89 .t*Ic.L...0...H.
000000000040F6A0 EA 44 89 4C 24 38 44 89 5C 24 30 E8 D0 2D FF FF .D.L$8D.\$0..-..
000000000040F6B0 44 8B 4C 24 38 44 8B 5C 24 30 49 01 EC 49 01 ED D.L$8D.\$0I..I..
000000000040F6C0 49 89 D8 31 C9 31 ED 48 8B 5C 24 28 49 89 CA 4C I..1.1.H.\$(I..L
000000000040F6D0 89 F0 4C 29 FB 48 39 CB 4C 0F 43 D3 4C 29 E8 49 ..L).H9.L.C.L).I
000000000040F6E0 39 C2 0F 83 B0 F7 FF FF 4D 85 E4 74 7C 48 39 CB 9.......M..t|H9.
000000000040F6F0 73 49 8B 44 24 18 85 C0 75 41 48 63 ED 4C 89 54 sI.D$...uAHc.L.T
000000000040F700 24 30 44 89 4C 24 28 48 29 DD 41 83 FB 30 4C 89 $0D.L$(H).A..0L.
000000000040F710 44 24 18 48 89 EA 0F 84 F9 0A 00 00 4C 89 E7 BE D$.H........L...
000000000040F720 20 00 00 00 49 01 EC E8 54 2D FF FF 4C 8B 54 24  ...I...T-..L.T$
000000000040F730 30 44 8B 4C 24 28 4C 8B 44 24 18 45 84 C9 4C 89 0D.L$(L.D$.E..L.
000000000040F740 54 24 28 4C 89 44 24 18 48 89 DA 4C 89 FE 4C 89 T$(L.D$.H..L..L.
000000000040F750 E7 0F 84 21 08 00 00 E8 24 F5 FF FF 4C 8B 44 24 ...!....$...L.D$
000000000040F760 18 4C 8B 54 24 28 49 01 DC 4D 01 D5 E9 03 F6 FF .L.T$(I..M......
000000000040F770 FF 83 F9 45 0F 84 3E F9 FF FF 4C 8B 54 24 10 41 ...E..>...L.T$.A
000000000040F780 B8 93 24 49 92 41 8B 42 18 8D 78 06 89 F8 41 F7 ..$I.A.B..x...A.
000000000040F790 E8 8D 04 3A 89 FA C1 FA 1F C1 F8 02 29 D0 8D 14 ...:........)...
000000000040F7A0 C5 00 00 00 00 29 C2 41 8B 42 1C 29 FA 8D 7C 02 .....).A.B.)..|.
000000000040F7B0 07 89 F8 41 F7 E8 44 8D 04 3A C1 FF 1F C7 44 24 ...A..D..:....D$
000000000040F7C0 18 02 00 00 00 41 C1 F8 02 41 29 F8 E9 57 FA FF .....A...A)..W..
000000000040F7D0 FF 83 F9 45 0F 84 DE F8 FF FF 48 8B 44 24 10 BA ...E......H.D$..
000000000040F7E0 93 24 49 92 8B 78 1C 2B 78 18 83 C7 07 89 F8 F7 .$I..x.+x.......
000000000040F7F0 EA EB C3 48 C7 44 24 18 DA 64 41 00 8B B4 24 10 ...H.D$..dA...$.
000000000040F800 05 00 00 41 0F B6 C1 4C 8B 44 24 10 44 8B 4C 24 ...A...L.D$.D.L$
000000000040F810 44 48 8B 4C 24 18 48 C7 C2 FF FF FF FF 89 C7 44 DH.L$.H........D
000000000040F820 89 5C 24 38 89 44 24 30 89 34 24 31 F6 E8 9E F4 .\$8.D$0.4$1....
000000000040F830 FF FF 49 89 C7 31 C0 85 ED 0F 49 C5 4D 89 F2 48 ..I..1....I.M..H
000000000040F840 98 49 39 C7 48 89 C6 49 0F 43 F7 4D 29 EA 4C 39 .I9.H..I.C.M).L9
000000000040F850 D6 48 89 74 24 28 0F 83 3C F6 FF FF 4D 85 E4 74 .H.t$(..<...M..t
000000000040F860 62 49 39 C7 44 8B 5C 24 38 73 2D 48 63 ED 4C 89 bI9.D.\$8s-Hc.L.
000000000040F870 54 24 38 4C 29 FD 41 83 FB 30 48 89 EA 0F 84 27 T$8L).A..0H....'
000000000040F880 0A 00 00 4C 89 E7 BE 20 00 00 00 49 01 EC E8 ED ...L... ...I....
000000000040F890 2B FF FF 4C 8B 54 24 38 8B 84 24 10 05 00 00 44 +..L.T$8..$....D
000000000040F8A0 8B 4C 24 44 4C 89 E6 4C 8B 44 24 10 48 8B 4C 24 .L$DL..L.D$.H.L$
000000000040F8B0 18 4C 89 D2 8B 7C 24 30 4D 01 FC 89 04 24 E8 0D .L...|$0M....$..
000000000040F8C0 F4 FF FF 4C 03 6C 24 28 49 89 D8 E9 A4 F4 FF FF ...L.l$(I.......
000000000040F8D0 83 F9 45 0F 84 DF F7 FF FF 48 8B 44 24 10 C7 44 ..E......H.D$..D
000000000040F8E0 24 18 02 00 00 00 44 8B 00 E9 3A F9 FF FF 48 C7 $.....D...:...H.
000000000040F8F0 44 24 18 6F 39 41 00 E9 00 FF FF FF BF 70 00 00 D$.o9A.......p..
000000000040F900 00 BE 70 00 00 00 41 BF 01 00 00 00 84 C0 BA 00 ..p...A.........
000000000040F910 00 00 00 B8 01 00 00 00 44 0F 45 CA 44 0F 45 F8 ........D.E.D.E.
000000000040F920 E9 E7 F5 FF FF 83 F9 45 0F 84 8A F7 FF FF 83 FD .......E........
000000000040F930 FF 0F 84 A3 0A 00 00 83 FD 08 0F 8F 99 0C 00 00 ................
000000000040F940 44 8B 84 24 10 05 00 00 89 EF 41 BA 67 66 66 66 D..$......A.gfff
000000000040F950 44 89 C0 83 C7 01 41 C1 F8 1F 41 F7 EA C1 FA 02 D.....A...A.....
000000000040F960 44 29 C2 83 FF 09 41 89 D0 75 E5 89 6C 24 18 E9 D)....A..u..l$..
000000000040F970 B4 F8 FF FF 83 F9 45 0F 84 3B F7 FF FF 48 8B 44 ......E..;...H.D
000000000040F980 24 10 C7 44 24 18 02 00 00 00 44 8B 40 04 E9 95 $..D$.....D.@...
000000000040F990 F8 FF FF 83 F9 45 0F 84 F1 08 00 00 48 8B 44 24 .....E......H.D$
000000000040F9A0 10 41 B8 1F 85 EB 51 41 BA 64 00 00 00 C7 44 24 .A....QA.d....D$
000000000040F9B0 18 02 00 00 00 8B 78 14 89 F8 41 F7 E8 89 F8 C1 ......x...A.....
000000000040F9C0 F8 1F 41 89 D0 41 C1 F8 05 41 29 C0 89 F8 45 0F ..A..A...A)...E.
000000000040F9D0 AF C2 44 29 C0 41 89 C0 0F 89 4A F8 FF FF F7 D8 ..D).A....J.....
000000000040F9E0 41 83 C0 64 81 FF 93 F8 FF FF 44 0F 4E C0 E9 35 A..d......D.N..5
000000000040F9F0 F8 FF FF 45 31 FF E9 11 FF FF FF 31 C0 85 ED 41 ...E1......1...A
000000000040FA00 BF 01 00 00 00 0F 49 C5 4C 89 F2 48 98 48 85 C0 ......I.L..H.H..
000000000040FA10 4C 0F 45 F8 4C 29 EA 49 39 D7 0F 83 78 F4 FF FF L.E.L).I9...x...
000000000040FA20 4D 85 E4 74 33 48 83 F8 01 76 24 48 63 ED 48 83 M..t3H...v$Hc.H.
000000000040FA30 ED 01 41 83 FB 30 48 89 EA 0F 84 EE 0A 00 00 4C ..A..0H........L
000000000040FA40 89 E7 BE 20 00 00 00 49 01 EC E8 31 2A FF FF 41 ... ...I...1*..A
000000000040FA50 C6 04 24 09 49 83 C4 01 4D 01 FD 49 89 D8 E9 11 ..$.I...M..I....
000000000040FA60 F3 FF FF 48 8B 74 24 10 48 8D 7C 24 70 44 89 4C ...H.t$.H.|$pD.L
000000000040FA70 24 28 44 89 5C 24 18 4C 8D BC 24 D7 00 00 00 48 $(D.\$.L..$....H
000000000040FA80 8B 06 48 89 44 24 70 48 8B 46 08 48 89 44 24 78 ..H.D$pH.F.H.D$x
000000000040FA90 48 8B 46 10 48 89 84 24 80 00 00 00 48 8B 46 18 H.F.H..$....H.F.
000000000040FAA0 48 89 84 24 88 00 00 00 48 8B 46 20 48 89 84 24 H..$....H.F H..$
000000000040FAB0 90 00 00 00 48 8B 46 28 48 89 84 24 98 00 00 00 ....H.F(H..$....
000000000040FAC0 48 8B 46 30 48 89 84 24 A0 00 00 00 E8 CF 2B FF H.F0H..$......+.
000000000040FAD0 FF 44 8B 5C 24 18 49 89 C0 44 8B 4C 24 28 49 C1 .D.\$.I..D.L$(I.
000000000040FAE0 E8 3F 48 89 C1 49 BA 67 66 66 66 66 66 66 66 44 .?H..I.gfffffffD
000000000040FAF0 89 C6 BF 30 00 00 00 66 0F 1F 84 00 00 00 00 00 ...0...f........
000000000040FB00 48 89 C8 49 83 EF 01 49 F7 EA 48 89 C8 48 C1 F8 H..I...I..H..H..
000000000040FB10 3F 48 C1 FA 02 48 29 C2 48 8D 04 92 48 01 C0 48 ?H...H).H...H..H
000000000040FB20 29 C1 48 89 C8 48 89 D1 89 FA 29 C2 83 C0 30 40 ).H..H....)...0@
000000000040FB30 84 F6 0F 44 D0 48 85 C9 41 88 17 75 C3 85 ED B8 ...D.H..A..u....
000000000040FB40 01 00 00 00 0F 4F C5 45 84 C0 89 44 24 18 0F 84 .....O.E...D$...
000000000040FB50 3D FA FF FF 41 83 FB 2D 0F 84 2E 04 00 00 B8 01 =...A..-........
000000000040FB60 00 00 00 C6 44 24 30 01 41 BA 2D 00 00 00 E9 35 ....D$0.A.-....5
000000000040FB70 FA FF FF 31 C0 85 ED 41 BF 01 00 00 00 0F 49 C5 ...1...A......I.
000000000040FB80 4C 89 F2 48 98 48 85 C0 4C 0F 45 F8 4C 29 EA 49 L..H.H..L.E.L).I
000000000040FB90 39 D7 0F 83 00 F3 FF FF 4D 85 E4 0F 84 B7 FE FF 9.......M.......
000000000040FBA0 FF 48 83 F8 01 76 24 48 63 ED 48 83 ED 01 41 83 .H...v$Hc.H...A.
000000000040FBB0 FB 30 48 89 EA 0F 84 5D 09 00 00 4C 89 E7 BE 20 .0H....]...L... 
000000000040FBC0 00 00 00 49 01 EC E8 B5 28 FF FF 41 C6 04 24 0A ...I....(..A..$.
000000000040FBD0 49 83 C4 01 E9 7F FE FF FF 83 F9 45 0F 84 D6 F4 I..........E....
000000000040FBE0 FF FF 48 8B 44 24 10 C7 44 24 18 02 00 00 00 44 ..H.D$..D$.....D
000000000040FBF0 8B 40 08 E9 30 F6 FF FF 83 F9 45 0F 84 B7 F4 FF .@..0.....E.....
000000000040FC00 FF 4C 8B 7C 24 10 41 8B 47 14 89 C2 89 44 24 28 .L.|$.A.G....D$(
000000000040FC10 C1 F8 1F 25 90 01 00 00 44 8D 54 10 9C 41 8B 57 ...%....D.T..A.W
000000000040FC20 18 45 8B 7F 1C 45 89 F8 89 54 24 18 41 29 D0 BA .E...E...T$.A)..
000000000040FC30 93 24 49 92 41 81 C0 7E 01 00 00 44 89 C0 F7 EA .$I.A..~...D....
000000000040FC40 42 8D 04 02 44 89 C2 C1 FA 1F C1 F8 02 29 D0 8D B...D........)..
000000000040FC50 14 C5 00 00 00 00 29 C2 44 89 F8 44 29 C0 44 8D ......).D..D).D.
000000000040FC60 44 10 03 45 85 C0 0F 88 88 07 00 00 41 F6 C2 03 D..E........A...
000000000040FC70 B8 93 FE FF FF 75 4E 44 89 D0 BA 1F 85 EB 51 F7 .....uND......Q.
000000000040FC80 EA 44 89 D0 C1 F8 1F 89 44 24 38 89 54 24 30 C1 .D......D$8.T$0.
000000000040FC90 FA 05 29 C2 B8 64 00 00 00 0F AF D0 B8 92 FE FF ..)..d..........
000000000040FCA0 FF 41 39 D2 75 1F 8B 44 24 30 C1 F8 07 2B 44 24 .A9.u..D$0...+D$
000000000040FCB0 38 69 C0 90 01 00 00 41 29 C2 41 83 FA 01 19 C0 8i.....A).A.....
000000000040FCC0 2D 6D 01 00 00 45 8D 14 07 BA 93 24 49 92 44 89 -m...E.....$I.D.
000000000040FCD0 D0 2B 44 24 18 44 8D B8 7E 01 00 00 44 89 F8 45 .+D$.D..~...D..E
000000000040FCE0 29 FA F7 EA 42 8D 04 3A 44 89 FA 41 BF 01 00 00 )...B..:D..A....
000000000040FCF0 00 C1 FA 1F C1 F8 02 29 D0 8D 14 C5 00 00 00 00 .......)........
000000000040FD00 29 C2 45 8D 54 12 03 45 85 D2 0F 88 43 08 00 00 ).E.T..E....C...
000000000040FD10 40 80 FF 47 0F 84 72 06 00 00 40 80 FF 67 0F 85 @..G..r...@..g..
000000000040FD20 05 06 00 00 8B 44 24 28 41 B8 1F 85 EB 51 8B 7C .....D$(A....Q.|
000000000040FD30 24 28 C7 44 24 18 02 00 00 00 41 F7 E8 89 F8 C1 $(.D$.....A.....
000000000040FD40 F8 1F C1 FA 05 29 C2 B8 64 00 00 00 0F AF D0 89 .....)..d.......
000000000040FD50 F8 29 D0 89 C7 44 01 FF 89 F8 41 F7 E8 89 F8 C1 .)...D....A.....
000000000040FD60 F8 1F 41 89 D0 41 C1 F8 05 41 29 C0 B8 64 00 00 ..A..A...A)..d..
000000000040FD70 00 44 0F AF C0 44 29 C7 41 89 F8 0F 89 A7 F4 FF .D...D).A.......
000000000040FD80 FF 8B 7C 24 28 B8 94 F8 FF FF 44 89 C2 44 29 F8 ..|$(.....D..D).
000000000040FD90 F7 DA 41 83 C0 64 39 C7 44 0F 4C C2 E9 87 F4 FF ..A..d9.D.L.....
000000000040FDA0 FF 0F 1F 80 00 00 00 00 85 C9 0F 85 08 F3 FF FF ................
000000000040FDB0 48 C7 44 24 18 D1 64 41 00 E9 3E FA FF FF 85 C9 H.D$..dA..>.....
000000000040FDC0 0F 85 F2 F2 FF FF 48 C7 44 24 18 C8 64 41 00 E9 ......H.D$..dA..
000000000040FDD0 28 FA FF FF 83 F9 4F 0F 84 DB F2 FF FF 83 F9 45 (.....O........E
000000000040FDE0 0F 84 D4 05 00 00 48 8B 44 24 10 BA 1F 85 EB 51 ......H.D$.....Q
000000000040FDF0 41 BA 64 00 00 00 8B 48 14 89 C8 F7 EA 89 C8 C1 A.d....H........
000000000040FE00 F8 1F C1 FA 05 29 C2 31 C0 44 8D 42 13 41 0F AF .....).1.D.B.A..
000000000040FE10 D2 39 D1 0F 88 45 07 00 00 81 F9 94 F8 FF FF C7 .9...E..........
000000000040FE20 44 24 18 02 00 00 00 41 0F 9C C2 41 29 C0 E9 D7 D$.....A...A)...
000000000040FE30 F6 FF FF 0F B6 43 01 3C 3A 0F 84 C7 04 00 00 48 .....C.<:......H
000000000040FE40 8D 53 01 41 BF 01 00 00 00 3C 7A 0F 85 67 F2 FF .S.A.....<z..g..
000000000040FE50 FF 48 89 D3 E9 A7 F4 FF FF 83 F9 45 0F 84 56 F2 .H.........E..V.
000000000040FE60 FF FF 48 8B 44 24 10 C6 44 24 28 00 C7 44 24 18 ..H.D$..D$(..D$.
000000000040FE70 02 00 00 00 44 8B 40 10 41 83 F8 FF 41 0F 9C C2 ....D.@.A...A...
000000000040FE80 41 83 C0 01 31 FF E9 AD F3 FF FF 85 C9 0F 85 07 A...1...........
000000000040FE90 04 00 00 85 ED 89 C8 41 BF 01 00 00 00 0F 49 C5 .......A......I.
000000000040FEA0 4C 89 F2 48 98 48 85 C0 4C 0F 45 F8 4C 29 EA 49 L..H.H..L.E.L).I
000000000040FEB0 39 D7 0F 83 E0 EF FF FF 4D 85 E4 0F 84 97 FB FF 9.......M.......
000000000040FEC0 FF 48 83 F8 01 76 27 48 63 ED 48 83 ED 01 41 83 .H...v'Hc.H...A.
000000000040FED0 FB 30 48 89 EA 0F 84 E6 06 00 00 4C 89 E7 BE 20 .0H........L... 
000000000040FEE0 00 00 00 49 01 EC E8 95 25 FF FF 0F B6 3B 41 88 ...I....%....;A.
000000000040FEF0 3C 24 49 83 C4 01 E9 5D FB FF FF 4C 8D 43 FF 0F <$I....]...L.C..
000000000040FF00 B6 7B FF E9 97 F4 FF FF 83 F9 45 0F 84 A7 F1 FF .{........E.....
000000000040FF10 FF 48 8B 44 24 10 44 8B 40 08 E9 98 F3 FF FF 83 .H.D$.D.@.......
000000000040FF20 F9 45 0F 84 90 F1 FF FF 44 8B 44 24 40 C7 44 24 .E......D.D$@.D$
000000000040FF30 18 02 00 00 00 E9 EE F2 FF FF 83 F9 45 0F 84 75 ............E..u
000000000040FF40 F1 FF FF 44 8B 44 24 40 E9 6A F3 FF FF          ...D.D$@.j...   

l000000000040FF4D:
	mov	r8,rbx
	jmp	40F39Fh
000000000040FF55                0F 1F 00 41 BA 2D 00 00 00 41 83      ...A.-...A.
000000000040FF60 FB 2D 74 2E B8 01 00 00 00 C6 44 24 30 01 E9 35 .-t.......D$0..5
000000000040FF70 F6 FF FF 0F 1F 44 00 00 E8 43 26 FF FF 4C 8B 54 .....D...C&..L.T
000000000040FF80 24 28 4C 8B 44 24 18 E9 DA F7 FF FF 41 BA 2D 00 $(L.D$......A.-.
000000000040FF90 00 00 31 C9 85 ED 41 B8 01 00 00 00 0F 49 CD 4C ..1...A......I.L
000000000040FFA0 89 F0 48 63 C9 48 85 C9 4C 0F 45 C1 4C 29 E8 49 ..Hc.H..L.E.L).I
000000000040FFB0 39 C0 0F 83 E0 EE FF FF 4D 85 E4 74 62 48 83 F9 9.......M..tbH..
000000000040FFC0 01 76 54 8B 7C 24 18 85 FF 75 4C 48 63 C5 4C 89 .vT.|$...uLHc.L.
000000000040FFD0 E7 BE 20 00 00 00 4C 8D 58 FF 44 89 54 24 58 4C .. ...L.X.D.T$XL
000000000040FFE0 89 44 24 50 48 89 4C 24 38 44 89 4C 24 30 4C 89 .D$PH.L$8D.L$0L.
000000000040FFF0 DA 4C 89 5C 24 28 E8 85 24 FF FF 4C 8B 5C 24 28 .L.\$(..$..L.\$(
0000000000410000 44 8B 54 24 58 4C 8B 44 24 50 48 8B 4C 24 38 44 D.T$XL.D$PH.L$8D
0000000000410010 8B 4C 24 30 4D 01 DC 45 88 14 24 49 83 C4 01 48 .L$0M..E..$I...H
0000000000410020 8D 84 24 D7 00 00 00 4D 01 C5 41 BB 2D 00 00 00 ..$....M..A.-...
0000000000410030 49 89 D8 48 89 44 24 28 E9 8A F6 FF FF 41 BA 2B I..H.D$(.....A.+
0000000000410040 00 00 00 E9 16 FF FF FF 80 7C 24 30 00 0F 84 17 .........|$0....
0000000000410050 02 00 00 45 31 C0 85 ED B9 01 00 00 00 44 0F 49 ...E1........D.I
0000000000410060 C5 4C 89 F0 4D 63 C0 4D 85 C0 49 0F 45 C8 4C 29 .L..Mc.M..I.E.L)
0000000000410070 E8 48 39 C1 0F 83 1E EE FF FF 4D 85 E4 74 75 49 .H9.......M..tuI
0000000000410080 83 F8 01 76 67 8B 44 24 18 85 C0 75 5F 48 63 C5 ...vg.D$...u_Hc.
0000000000410090 44 89 54 24 68 48 89 4C 24 60 48 83 E8 01 41 83 D.T$hH.L$`H...A.
00000000004100A0 FB 30 4C 89 44 24 58 48 89 44 24 30 44 89 4C 24 .0L.D$XH.D$0D.L$
00000000004100B0 50 44 89 5C 24 38 0F 84 D2 04 00 00 48 8B 54 24 PD.\$8......H.T$
00000000004100C0 30 4C 89 E7 BE 20 00 00 00 E8 B2 23 FF FF 4C 03 0L... .....#..L.
00000000004100D0 64 24 30 44 8B 54 24 68 48 8B 4C 24 60 4C 8B 44 d$0D.T$hH.L$`L.D
00000000004100E0 24 58 44 8B 4C 24 50 44 8B 5C 24 38 45 88 14 24 $XD.L$PD.\$8E..$
00000000004100F0 49 83 C4 01 49 01 CD 4C 89 C1 49 89 D8 E9 C5 F5 I...I..L..I.....
0000000000410100 FF FF 4C 89 F0 49 63 C8 4C 29 E8 48 39 C1 0F 83 ..L..Ic.L).H9...
0000000000410110 84 ED FF FF 4D 85 E4 74 45 48 89 CA 4C 89 E7 BE ....M..tEH..L...
0000000000410120 20 00 00 00 44 89 54 24 68 44 89 44 24 60 44 89  ...D.T$hD.D$`D.
0000000000410130 4C 24 58 44 89 5C 24 50 48 89 4C 24 38 E8 3E 23 L$XD.\$PH.L$8.>#
0000000000410140 FF FF 48 8B 4C 24 38 44 8B 54 24 68 44 8B 44 24 ..H.L$8D.T$hD.D$
0000000000410150 60 44 8B 4C 24 58 44 8B 5C 24 50 49 01 CC 49 01 `D.L$XD.\$PI..I.
0000000000410160 CD 41 39 E8 0F 8D 1A 01 00 00 31 C9 44 29 C5 0F .A9.......1.D)..
0000000000410170 49 CD 48 63 C9 80 7C 24 30 00 0F 84 FC 00 00 00 I.Hc..|$0.......
0000000000410180 48 85 C9 41 B8 01 00 00 00 4C 89 F0 4C 0F 45 C1 H..A.....L..L.E.
0000000000410190 4C 29 E8 49 39 C0 0F 83 FC EC FF FF 4D 85 E4 74 L).I9.......M..t
00000000004101A0 69 48 83 F9 01 76 5B 8B 74 24 18 85 F6 75 53 48 iH...v[.t$...uSH
00000000004101B0 63 C5 4C 89 E7 BE 20 00 00 00 48 83 E8 01 48 89 c.L... ...H...H.
00000000004101C0 4C 24 68 44 89 54 24 60 48 89 C2 4C 89 44 24 58 L$hD.T$`H..L.D$X
00000000004101D0 44 89 4C 24 50 44 89 5C 24 38 48 89 44 24 30 E8 D.L$PD.\$8H.D$0.
00000000004101E0 9C 22 FF FF 4C 03 64 24 30 48 8B 4C 24 68 44 8B ."..L.d$0H.L$hD.
00000000004101F0 54 24 60 4C 8B 44 24 58 44 8B 4C 24 50 44 8B 5C T$`L.D$XD.L$PD.\
0000000000410200 24 38 45 88 14 24 49 83 C4 01 4D 01 C5 49 89 D8 $8E..$I...M..I..
0000000000410210 E9 B2 F4 FF FF 4C 89 E7 BE 30 00 00 00 49 01 EC .....L...0...I..
0000000000410220 E8 5B 22 FF FF 4C 8B 44 24 18 44 8B 4C 24 28 4C .["..L.D$.D.L$(L
0000000000410230 8B 54 24 30 E9 02 F5 FF FF 4C 89 E7 BE 30 00 00 .T$0.....L...0..
0000000000410240 00 49 01 EC E8 37 22 FF FF 4C 8B 44 24 18 44 8B .I...7"..L.D$.D.
0000000000410250 4C 24 28 48 8B 4C 24 30 E9 A4 ED FF FF 48 8D 84 L$(H.L$0.....H..
0000000000410260 24 D7 00 00 00 48 89 44 24 28 31 C9 85 ED 49 89 $....H.D$(1...I.
0000000000410270 D8 0F 49 CD 48 63 C9 E9 4B F4 FF FF 49 89 D8 E9 ..I.Hc..K...I...
0000000000410280 43 F4 FF FF 31 C9 31 ED E9 E8 FE FF FF C7 44 24 C...1.1.......D$
0000000000410290 18 00 00 00 00 E9 B0 EF FF FF 49 89 D8 E9 07 F1 ..........I.....
00000000004102A0 FF FF 49 89 D8 E9 CA EA FF FF 4C 89 E7 BE 30 00 ..I.......L...0.
00000000004102B0 00 00 49 01 EC E8 C6 21 FF FF 4C 8B 54 24 38 E9 ..I....!..L.T$8.
00000000004102C0 D4 F5 FF FF E8 F7 22 FF FF 4C 8B 44 24 18 E9 F4 ......"..L.D$...
00000000004102D0 F1 FF FF 41 69 F8 10 27 00 00 41 B8 64 00 00 00 ...Ai..'..A.d...
00000000004102E0 41 C1 EA 1F 41 0F AF C0 C6 44 24 28 01 C7 44 24 A...A....D$(..D$
00000000004102F0 18 09 00 00 00 44 8D 04 07 BF 14 00 00 00 41 01 .....D........A.
0000000000410300 D0 E9 32 EF FF FF 48 8D 7B 02 41 BF 01 00 00 00 ..2...H.{.A.....
0000000000410310 48 89 FA 48 83 C7 01 0F B6 47 FF 49 83 C7 01 3C H..H.....G.I...<
0000000000410320 3A 0F 85 22 FB FF FF EB E7 44 89 D0 BA 93 24 49 :..".....D....$I
0000000000410330 92 C7 44 24 18 02 00 00 00 F7 EA 44 89 D0 C1 F8 ..D$.......D....
0000000000410340 1F 46 8D 04 12 41 C1 F8 02 41 29 C0 41 83 C0 01 .F...A...A).A...
0000000000410350 E9 D3 EE FF FF 41 BF 64 00 00 00 41 C1 EA 1F BF .....A.d...A....
0000000000410360 04 00 00 00 45 0F AF C7 C6 44 24 28 01 C7 44 24 ....E....D$(..D$
0000000000410370 18 06 00 00 00 41 01 C0 E9 BB EE FF FF E8 AE E8 .....A..........
0000000000410380 FF FF 4C 8B 44 24 18 E9 3B F1 FF FF 8B 7C 24 28 ..L.D$..;....|$(
0000000000410390 B8 94 F8 FF FF C6 44 24 28 00 44 29 F8 C7 44 24 ......D$(.D)..D$
00000000004103A0 18 04 00 00 00 39 C7 45 8D 84 3F 6C 07 00 00 41 .....9.E..?l...A
00000000004103B0 0F 9C C2 31 FF E9 7E EE FF FF C6 84 24 B0 00 00 ...1..~.....$...
00000000004103C0 00 20 C6 84 24 B1 00 00 00 25 45 31 FF C7 44 24 . ..$....%E1..D$
00000000004103D0 18 00 00 00 00 E9 83 EE FF FF 44 8B 84 24 10 05 ..........D..$..
00000000004103E0 00 00 BD 09 00 00 00 C7 44 24 18 09 00 00 00 E9 ........D$......
00000000004103F0 34 EE FF FF 41 83 EA 01 B8 6D 01 00 00 41 F6 C2 4...A....m...A..
0000000000410400 03 75 50 41 B8 1F 85 EB 51 44 89 D0 41 F7 E8 44 .uPA....QD..A..D
0000000000410410 89 D0 C1 F8 1F 89 44 24 30 41 89 D0 C1 FA 05 29 ......D$0A.....)
0000000000410420 C2 B8 64 00 00 00 0F AF D0 B8 6E 01 00 00 41 39 ..d.......n...A9
0000000000410430 D2 75 20 41 C1 F8 07 44 2B 44 24 30 45 69 C0 90 .u A...D+D$0Ei..
0000000000410440 01 00 00 45 29 C2 41 83 FA 01 19 C0 F7 D0 05 6E ...E).A........n
0000000000410450 01 00 00 45 8D 14 07 BA 93 24 49 92 41 BF FF FF ...E.....$I.A...
0000000000410460 FF FF 45 89 D0 44 2B 44 24 18 41 81 C0 7E 01 00 ..E..D+D$.A..~..
0000000000410470 00 44 89 C0 F7 EA 42 8D 04 02 44 89 C2 C1 FA 1F .D....B...D.....
0000000000410480 C1 F8 02 29 D0 8D 14 C5 00 00 00 00 29 C2 44 89 ...)........).D.
0000000000410490 D0 44 29 C0 44 8D 54 10 03 E9 72 F8 FF FF 85 D2 .D).D.T...r.....
00000000004104A0 0F 85 2D FE FF FF 85 C0 0F 85 A7 FE FF FF 41 C1 ..-...........A.
00000000004104B0 EA 1F 31 FF C6 44 24 28 01 C7 44 24 18 03 00 00 ..1..D$(..D$....
00000000004104C0 00 E9 72 ED FF FF 4C 89 E7 BE 30 00 00 00 49 01 ..r...L...0...I.
00000000004104D0 EC E8 AA 1F FF FF 44 8B 5C 24 30 44 8B 4C 24 38 ......D.\$0D.L$8
00000000004104E0 44 8B 44 24 50 48 8B 4C 24 58 44 8B 54 24 60 E9 D.D$PH.L$XD.T$`.
00000000004104F0 8C F1 FF FF BA 64 00 00 00 41 C1 EA 1F 31 FF 44 .....d...A...1.D
0000000000410500 0F AF C2 C6 44 24 28 01 C7 44 24 18 05 00 00 00 ....D$(..D$.....
0000000000410510 41 01 C0 E9 20 ED FF FF 4C 89 E7 BE 30 00 00 00 A... ...L...0...
0000000000410520 49 01 EC E8 58 1F FF FF E9 9E F6 FF FF 4C 89 E7 I...X........L..
0000000000410530 BE 30 00 00 00 49 01 EC E8 43 1F FF FF E9 0D F5 .0...I...C......
0000000000410540 FF FF 45 31 FF 48 C7 44 24 48 19 69 41 00 E9 DE ..E1.H.D$H.iA...
0000000000410550 EE FF FF 45 89 C2 45 30 FF E9 B2 F7 FF FF 31 C0 ...E..E0......1.
0000000000410560 45 85 C0 0F 9F C0 E9 AE F8 FF FF 4C 89 E7 BE 30 E..........L...0
0000000000410570 00 00 00 49 01 EC E8 05 1F FF FF 44 8B 4C 24 18 ...I.......D.L$.
0000000000410580 8B 4C 24 28 4C 8B 44 24 30 E9 0E EF FF FF 4C 89 .L$(L.D$0.....L.
0000000000410590 E7 48 89 C2 BE 30 00 00 00 E8 E2 1E FF FF 4C 03 .H...0........L.
00000000004105A0 64 24 30 44 8B 5C 24 38 44 8B 4C 24 50 4C 8B 44 d$0D.\$8D.L$PL.D
00000000004105B0 24 58 48 8B 4C 24 60 44 8B 54 24 68 E9 2B FB FF $XH.L$`D.T$h.+..
00000000004105C0 FF 4C 89 E7 BE 30 00 00 00 49 01 EC E8 AF 1E FF .L...0...I......
00000000004105D0 FF 0F B6 3B E9 15 F9 FF FF 44 8B 84 24 10 05 00 ...;.....D..$...
00000000004105E0 00 89 6C 24 18 E9 3E EC FF FF                   ..l$..>...      

l00000000004105EA:
	call	4023A0h
00000000004105EF                                              C7                .
00000000004105F0 44 24 18 00 00 00 00 E9 61 EC FF FF 0F 1F 40 00 D$......a.....@.

;; fn0000000000410600: 0000000000410600
;;   Called from:
;;     0000000000406AEF (in fn0000000000406A80)
fn0000000000410600 proc
	sub	rsp,18h
	mov	[rsp],r9d
	mov	r9d,r8d
	mov	r8,rcx
	mov	rcx,rdx
	mov	rdx,rsi
	mov	rsi,rdi
	xor	edi,edi
	call	40ECD0h
	add	rsp,18h
	ret
0000000000410623          66 2E 0F 1F 84 00 00 00 00 00 0F 1F 00    f............

;; fn0000000000410630: 0000000000410630
;;   Called from:
;;     0000000000410B1B (in fn0000000000410AC0)
fn0000000000410630 proc
	push	r15
	push	r14
	push	r13
	push	r12
	mov	r12,r9
	push	rbp
	mov	rbp,rdi
	push	rbx
	mov	rbx,r8
	sub	rsp,58h
	test	rsi,rsi
	jz	410A30h

l0000000000410650:
	mov	r9,rcx
	mov	r8,rdx
	mov	rcx,rsi
	mov	edx,4168C0h
	mov	esi,1h
	xor	eax,eax
	call	402810h

l000000000041066A:
	xor	edi,edi
	mov	edx,5h
	mov	esi,4168D3h
	call	402360h
	mov	r8d,7DDh
	mov	rcx,rax
	mov	edx,416BA0h
	mov	esi,1h
	mov	rdi,rbp
	xor	eax,eax
	call	402810h
	xor	edi,edi
	mov	edx,5h
	mov	esi,416930h
	call	402360h
	mov	rsi,rbp
	mov	rdi,rax
	call	402520h
	cmp	r12,9h
	ja	410A50h

l00000000004106BE:
	jmp	qword ptr [416B48h+riz*8]
00000000004106C6                   66 2E 0F 1F 84 00 00 00 00 00       f.........
00000000004106D0 4C 8B 4B 38 4C 8B 43 08 BA 05 00 00 00 48 8B 43 L.K8L.C......H.C
00000000004106E0 10 BE A0 6A 41 00 31 FF 4C 8B 6B 30 4C 8B 63 28 ...jA.1.L.k0L.c(
00000000004106F0 4C 8B 7B 20 4C 8B 73 18 4C 89 4C 24 40 4C 89 44 L.{ L.s.L.L$@L.D
0000000000410700 24 38 48 8B 1B 48 89 44 24 30 E8 51 1C FF FF 4C $8H..H.D$0.Q...L
0000000000410710 8B 4C 24 40 4C 8B 44 24 38 48 89 C2 4C 89 6C 24 .L$@L.D$8H..L.l$
0000000000410720 18 4C 89 64 24 10 48 89 D9 4C 89 7C 24 08 4C 89 .L.d$.H..L.|$.L.
0000000000410730 34 24 BE 01 00 00 00 4C 89 4C 24 20 4C 8B 4C 24 4$.....L.L$ L.L$
0000000000410740 30 48 89 EF 31 C0 E8 C5 20 FF FF 48 83 C4 58 5B 0H..1... ..H..X[
0000000000410750 5D 41 5C 41 5D 41 5E 41 5F C3 66 0F 1F 44 00 00 ]A\A]A^A_.f..D..
0000000000410760 4C 8B 53 40 4C 8B 4B 38 BA 05 00 00 00 48 8B 43 L.S@L.K8.....H.C
0000000000410770 10 4C 8B 43 08 BE D0 6A 41 00 4C 8B 6B 30 4C 8B .L.C...jA.L.k0L.
0000000000410780 63 28 4C 8B 7B 20 4C 8B 73 18 48 8B 1B 4C 89 54 c(L.{ L.s.H..L.T
0000000000410790 24 48 4C 89 4C 24 40 48 89 44 24 30 4C 89 44 24 $HL.L$@H.D$0L.D$
00000000004107A0 38                                              8               

l00000000004107A1:
	xor	edi,edi
	call	402360h
	mov	r9,[rsp+40h]
	mov	r10,[rsp+48h]
	mov	rcx,rbx
	mov	r8,[rsp+38h]
	mov	[rsp+18h],r13
	mov	rdx,rax
	mov	[rsp+10h],r12
	mov	[rsp+8h],r15
	mov	rdi,rbp
	mov	[rsp+20h],r9
	mov	r9,[rsp+30h]
	mov	esi,1h
	mov	[rsp],r14
	mov	[rsp+28h],r10
	xor	eax,eax
	call	402810h
	add	rsp,58h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
00000000004107FD                                        0F 1F 00              ...
0000000000410800 E8 1B 1A FF FF 0F 1F 00 48 8B 1B BA 05 00 00 00 ........H.......
0000000000410810 BE D7 68 41 00 31 FF E8 44 1B FF FF 48 83 C4 58 ..hA.1..D...H..X
0000000000410820 48 89 D9 48 89 EF 5B 5D 41 5C 41 5D 41 5E 41 5F H..H..[]A\A]A^A_
0000000000410830 48 89 C2 BE 01 00 00 00 31 C0 E9 D1 1F FF FF 90 H.......1.......
0000000000410840 4C 8B 63 08 48 8B 1B BA 05 00 00 00 BE E7 68 41 L.c.H.........hA
0000000000410850 00 31 FF E8 08 1B FF FF 48 83 C4 58 48 89 D9 48 .1......H..XH..H
0000000000410860 89 EF 5B 5D 4D 89 E0 48 89 C2 BE 01 00 00 00 41 ..[]M..H.......A
0000000000410870 5C 41 5D 41 5E 41 5F 31 C0 E9 92 1F FF FF 66 90 \A]A^A_1......f.
0000000000410880 4C 8B 6B 10 4C 8B 63 08 BA 05 00 00 00 48 8B 1B L.k.L.c......H..
0000000000410890 BE FE 68 41 00 31 FF E8 C4 1A FF FF 48 83 C4 58 ..hA.1......H..X
00000000004108A0 48 89 EF 4D 89 E0 48 89 D9 4D 89 E9 48 89 C2 5B H..M..H..M..H..[
00000000004108B0 5D 41 5C 41 5D 41 5E 41 5F BE 01 00 00 00 31 C0 ]A\A]A^A_.....1.
00000000004108C0 E9 4B 1F FF FF 0F 1F 00 4C 8B 73 18 4C 8B 6B 10 .K......L.s.L.k.
00000000004108D0 31 FF 4C 8B 63 08 48 8B 1B BA 05 00 00 00 BE 00 1.L.c.H.........
00000000004108E0 6A 41 00 E8 78 1A FF FF 4C 89 34 24 48 89 C2 4D jA..x...L.4$H..M
00000000004108F0 89 E9 4D 89 E0 48 89 D9 BE 01 00 00 00 48 89 EF ..M..H.......H..
0000000000410900 31 C0 E8 09 1F FF FF E9 3F FE FF FF 0F 1F 40 00 1.......?.....@.
0000000000410910 4C 8B 7B 20 4C 8B 73 18 31 FF 4C 8B 6B 10 4C 8B L.{ L.s.1.L.k.L.
0000000000410920 63 08 BA 05 00 00 00 48 8B 1B BE 20 6A 41 00 E8 c......H... jA..
0000000000410930 2C 1A FF FF 4C 89 7C 24 08 48 89 C2 4C 89 34 24 ,...L.|$.H..L.4$
0000000000410940 4D 89 E9 4D 89 E0 48 89 D9 BE 01 00 00 00 48 89 M..M..H.......H.
0000000000410950 EF 31 C0 E8 B8 1E FF FF E9 EE FD FF FF 0F 1F 00 .1..............
0000000000410960 4C 8B 43 08 4C 8B 63 28 31 FF 4C 8B 7B 20 4C 8B L.C.L.c(1.L.{ L.
0000000000410970 73 18 BA 05 00 00 00 4C 8B 6B 10 BE 48 6A 41 00 s......L.k..HjA.
0000000000410980 48 8B 1B 4C 89 44 24 30 E8 D3 19 FF FF 4C 8B 44 H..L.D$0.....L.D
0000000000410990 24 30 48 89 C2 4C 89 64 24 10 4C 89 7C 24 08 4C $0H..L.d$.L.|$.L
00000000004109A0 89 34 24 4D 89 E9 48 89 D9 BE 01 00 00 00 48 89 .4$M..H.......H.
00000000004109B0 EF 31 C0 E8 58 1E FF FF E9 8E FD FF FF 0F 1F 00 .1..X...........
00000000004109C0 4C 8B 4B 10 4C 8B 43 08 31 FF 4C 8B 6B 30 4C 8B L.K.L.C.1.L.k0L.
00000000004109D0 63 28 BA 05 00 00 00 4C 8B 7B 20 4C 8B 73 18 BE c(.....L.{ L.s..
00000000004109E0 70 6A 41 00 48 8B 1B 4C 89 4C 24 38 4C 89 44 24 pjA.H..L.L$8L.D$
00000000004109F0 30 E8 6A 19 FF FF 4C 8B 4C 24 38 4C 8B 44 24 30 0.j...L.L$8L.D$0
0000000000410A00 48 89 C2 4C 89 6C 24 18 4C 89 64 24 10 48 89 D9 H..L.l$.L.d$.H..
0000000000410A10 4C 89 7C 24 08 4C 89 34 24 BE 01 00 00 00 48 89 L.|$.L.4$.....H.
0000000000410A20 EF 31 C0 E8 E8 1D FF FF E9 1E FD FF FF 0F 1F 00 .1..............

l0000000000410A30:
	mov	r8,rcx
	mov	esi,1h
	mov	rcx,rdx
	xor	eax,eax
	mov	edx,4168CCh
	call	402810h
	jmp	41066Ah
0000000000410A4C                                     0F 1F 40 00             ..@.

l0000000000410A50:
	mov	r10,[rbx+40h]
	mov	r9,[rbx+38h]
	mov	edx,5h
	mov	rax,[rbx+10h]
	mov	r8,[rbx+8h]
	mov	esi,416B08h
	mov	r13,[rbx+30h]
	mov	r12,[rbx+28h]
	mov	r15,[rbx+20h]
	mov	r14,[rbx+18h]
	mov	[rsp+48h],r10
	mov	[rsp+40h],r9
	mov	[rsp+30h],rax
	mov	[rsp+38h],r8
	mov	rbx,[rbx]
	jmp	4107A1h
0000000000410A96                   66 2E 0F 1F 84 00 00 00 00 00       f.........
0000000000410AA0 45 31 C9 49 83 38 00 74 12 0F 1F 80 00 00 00 00 E1.I.8.t........
0000000000410AB0 49 83 C1 01 4B 83 3C C8 00 75 F5 E9 70 FB FF FF I...K.<..u..p...

;; fn0000000000410AC0: 0000000000410AC0
;;   Called from:
;;     0000000000410BA8 (in fn0000000000410B30)
fn0000000000410AC0 proc
	sub	rsp,58h
	xor	r9d,r9d
	jmp	410AF3h
0000000000410AC9                            0F 1F 80 00 00 00 00          .......

l0000000000410AD0:
	mov	r10d,eax
	add	r10,[r8+10h]
	add	eax,8h
	mov	[r8],eax
	mov	rax,[r10]
	test	rax,rax
	mov	[rsp+r9*8],rax
	jz	410B18h

l0000000000410AE9:
	add	r9,1h
	cmp	r9,0Ah
	jz	410B18h

l0000000000410AF3:
	mov	eax,[r8]
	cmp	eax,30h
	jc	410AD0h

l0000000000410AFB:
	mov	r10,[r8+8h]
	lea	rax,[r10+8h]
	mov	[r8+8h],rax
	mov	rax,[r10]
	test	rax,rax
	mov	[rsp+r9*8],rax
	jnz	410AE9h

l0000000000410B13:
	nop	dword ptr [rax+rax+0h]

l0000000000410B18:
	mov	r8,rsp
	call	410630h
	add	rsp,58h
	ret
0000000000410B25                66 66 2E 0F 1F 84 00 00 00 00 00      ff.........

;; fn0000000000410B30: 0000000000410B30
;;   Called from:
;;     0000000000402E48 (in fn00000000004028C0)
fn0000000000410B30 proc
	sub	rsp,+0D8h
	test	al,al
	mov	[rsp+40h],r8
	mov	[rsp+48h],r9
	jz	410B7Ch

l0000000000410B45:
	movaps	[rsp+50h],xmm0
	movaps	[rsp+60h],xmm1
	movaps	[rsp+70h],xmm2
	movaps	[rsp+80h],xmm3
	movaps	[rsp+90h],xmm4
	movaps	[rsp+0A0h],xmm5
	movaps	[rsp+0B0h],xmm6
	movaps	[rsp+0C0h],xmm7

l0000000000410B7C:
	lea	rax,[rsp+0E0h]
	lea	r8,[rsp+8h]
	mov	[rsp+10h],rax
	lea	rax,[rsp+20h]
	mov	dword ptr [rsp+8h],20h
	mov	dword ptr [rsp+0Ch],30h
	mov	[rsp+18h],rax
	call	410AC0h
	add	rsp,+0D8h
	ret
0000000000410BB5                66 66 2E 0F 1F 84 00 00 00 00 00      ff.........
0000000000410BC0 53 BA 05 00 00 00 BE 1A 69 41 00 31 FF E8 8E 17 S.......iA.1....
0000000000410BD0 FF FF BA D2 37 41 00 48 89 C6 BF 01 00 00 00 31 ....7A.H.......1
0000000000410BE0 C0 E8 4A 1B FF FF BA 05 00 00 00 BE E8 37 41 00 ..J..........7A.
0000000000410BF0 31 FF E8 69 17 FF FF B9 08 5A 41 00 48 89 C6 BA 1..i.....ZA.H...
0000000000410C00 FC 37 41 00 BF 01 00 00 00 31 C0 E8 20 1B FF FF .7A......1.. ...
0000000000410C10 48 8B 1D F9 99 20 00 BE 30 5A 41 00 31 FF BA 05 H.... ..0ZA.1...
0000000000410C20 00 00 00 E8 38 17 FF FF 48 89 DE 48 89 C7 5B E9 ....8...H..H..[.
0000000000410C30 EC 18 FF FF 66 2E 0F 1F 84 00 00 00 00 00 66 90 ....f.........f.

;; fn0000000000410C40: 0000000000410C40
;;   Called from:
;;     0000000000402D11 (in fn00000000004028C0)
;;     0000000000402D7A (in fn00000000004028C0)
;;     0000000000402D9D (in fn00000000004028C0)
;;     0000000000402F8F (in fn00000000004028C0)
;;     00000000004033FE (in fn00000000004028C0)
;;     000000000040358E (in fn00000000004028C0)
;;     00000000004047D9 (in fn00000000004028C0)
;;     0000000000404D38 (in fn0000000000404D20)
;;     0000000000404FBB (in fn0000000000404E80)
;;     0000000000405F95 (in fn0000000000405ED0)
;;     0000000000408561 (in fn0000000000407EA0)
;;     000000000040CA16 (in fn000000000040C9B0)
;;     000000000040CBA6 (in fn000000000040CB40)
;;     000000000040E57F (in fn000000000040E450)
;;     0000000000410DC4 (in fn0000000000410C90)
;;     0000000000410E0F (in fn0000000000410E00)
fn0000000000410C40 proc
	push	rbx
	mov	rbx,rdi
	call	402640h
	test	rax,rax
	jz	410C50h

l0000000000410C4E:
	pop	rbx
	ret

l0000000000410C50:
	test	rbx,rbx
	jz	410C4Eh

l0000000000410C55:
	call	410E50h
	nop	word ptr [rax+rax+0h]
	xor	edx,edx
	mov	rax,-1h
	div	rsi
	cmp	rax,rdi
	jc	410C7Ah

l0000000000410C71:
	imul	rdi,rsi
	jmp	410C40h

l0000000000410C7A:
	push	rax
	call	410E50h
	jmp	410C40h
0000000000410C85                66 66 2E 0F 1F 84 00 00 00 00 00      ff.........

;; fn0000000000410C90: 0000000000410C90
;;   Called from:
;;     0000000000405F31 (in fn0000000000405ED0)
;;     000000000040618C (in fn0000000000405ED0)
;;     0000000000408429 (in fn0000000000407EA0)
;;     000000000040E4B2 (in fn000000000040E450)
;;     000000000040E5E3 (in fn000000000040E450)
fn0000000000410C90 proc
	test	rsi,rsi
	push	rbx
	mov	rbx,rsi
	jz	410CB0h

l0000000000410C99:
	mov	rsi,rbx
	call	4026E0h
	test	rax,rax
	jz	410CBEh

l0000000000410CA6:
	pop	rbx
	ret
0000000000410CA8                         0F 1F 84 00 00 00 00 00         ........

l0000000000410CB0:
	test	rdi,rdi
	jz	410C99h

l0000000000410CB5:
	call	4021F0h
	xor	eax,eax
	pop	rbx
	ret

l0000000000410CBE:
	test	rbx,rbx
	jz	410CA6h

l0000000000410CC3:
	call	410E50h
	nop	dword ptr [rax+rax+0h]
	mov	rcx,rdx
	mov	rax,-1h
	xor	edx,edx
	div	rcx
	cmp	rax,rsi
	jc	410CEDh

l0000000000410CE4:
	imul	rsi,rcx
	jmp	410C90h

l0000000000410CED:
	push	rax
	call	410E50h
	nop	word ptr cs:[rax+rax+0h]
	test	rdi,rdi
	mov	r8,rdx
	mov	rcx,[rsi]
	jz	410D40h

l0000000000410D0B:
	xor	edx,edx
	mov	rax,0AAAAAAAAAAAAAAAAh
	div	r8
	cmp	rcx,rax
	jnc	410D5Ch

l0000000000410D1F:
	lea	rax,[rcx+1h]
	shr	rax,1h
	add	rcx,rax

l0000000000410D29:
	mov	[rsi],rcx
	imul	rcx,r8
	mov	rsi,rcx
	jmp	410C90h
0000000000410D38                         0F 1F 84 00 00 00 00 00         ........

l0000000000410D40:
	test	rcx,rcx
	jnz	410D29h

l0000000000410D45:
	xor	edx,edx
	mov	eax,80h
	xor	ecx,ecx
	div	r8
	test	rax,rax
	setz	cl
	add	rcx,rax
	jmp	410D29h

l0000000000410D5C:
	push	rax
	call	410E50h
	nop	word ptr cs:[rax+rax+0h]
	test	rdi,rdi
	mov	rax,[rsi]
	jz	410DA0h

l0000000000410D78:
	mov	rdx,0AAAAAAAAAAAAAAA9h
	cmp	rax,rdx
	ja	410DB7h

l0000000000410D87:
	lea	rdx,[rax+1h]
	shr	rdx,1h
	add	rax,rdx
	mov	[rsi],rax
	mov	rsi,rax
	jmp	410C90h
0000000000410D9C                                     0F 1F 40 00             ..@.

l0000000000410DA0:
	test	rax,rax
	mov	edx,80h
	cmovz	rax,rdx

l0000000000410DAC:
	mov	[rsi],rax
	mov	rsi,rax
	jmp	410C90h

l0000000000410DB7:
	push	rax
	call	410E50h
	nop	dword ptr [rax]
	push	rbx
	mov	rbx,rdi
	call	410C40h
	mov	rdx,rbx
	xor	esi,esi
	mov	rdi,rax
	pop	rbx
	jmp	402480h
0000000000410DD7                      66 0F 1F 84 00 00 00 00 00        f........
0000000000410DE0 48 83 EC 08 E8 47 17 FF FF 48 85 C0 74 05 48 83 H....G...H..t.H.
0000000000410DF0 C4 08 C3 E8 58 00 00 00 0F 1F 84 00 00 00 00 00 ....X...........

;; fn0000000000410E00: 0000000000410E00
;;   Called from:
;;     000000000040E623 (in fn000000000040E600)
;;     0000000000410E41 (in fn0000000000410E30)
fn0000000000410E00 proc
	push	rbp
	mov	rbp,rdi
	mov	rdi,rsi
	push	rbx
	mov	rbx,rsi
	sub	rsp,8h
	call	410C40h
	add	rsp,8h
	mov	rdx,rbx
	mov	rsi,rbp
	pop	rbx
	pop	rbp
	mov	rdi,rax
	jmp	4025C0h
0000000000410E28                         0F 1F 84 00 00 00 00 00         ........

;; fn0000000000410E30: 0000000000410E30
;;   Called from:
;;     000000000040450C (in fn00000000004028C0)
;;     0000000000404D4A (in fn0000000000404D20)
;;     0000000000404D5D (in fn0000000000404D20)
;;     00000000004081E4 (in fn0000000000407EA0)
;;     0000000000408B98 (in fn0000000000407EA0)
;;     0000000000408C12 (in fn0000000000407EA0)
fn0000000000410E30 proc
	push	rbx
	mov	rbx,rdi
	call	402380h
	mov	rdi,rbx
	lea	rsi,[rax+1h]
	pop	rbx
	jmp	410E00h
0000000000410E46                   66 2E 0F 1F 84 00 00 00 00 00       f.........

;; fn0000000000410E50: 0000000000410E50
;;   Called from:
;;     00000000004043BB (in fn00000000004028C0)
;;     0000000000404FF8 (in fn0000000000404E80)
;;     000000000040619D (in fn0000000000405ED0)
;;     0000000000408C7B (in fn0000000000407EA0)
;;     000000000040A623 (in fn000000000040A610)
;;     0000000000410C55 (in fn0000000000410C40)
;;     0000000000410C7B (in fn0000000000410C40)
;;     0000000000410CC3 (in fn0000000000410C90)
;;     0000000000410CEE (in fn0000000000410C90)
;;     0000000000410D5D (in fn0000000000410C90)
;;     0000000000410DB8 (in fn0000000000410C90)
fn0000000000410E50 proc
	sub	rsp,8h
	mov	edx,5h
	mov	esi,416BCFh
	xor	edi,edi
	call	402360h
	mov	edi,[000000000061A580]                                 ; [rip+00209715]
	mov	rcx,rax
	mov	edx,415E54h
	xor	esi,esi
	xor	eax,eax
	call	402770h
	call	402220h
0000000000410E81    66 2E 0F 1F 84 00 00 00 00 00 0F 1F 44 00 00  f...........D..

;; fn0000000000410E90: 0000000000410E90
;;   Called from:
;;     0000000000402B09 (in fn00000000004028C0)
;;     0000000000402C98 (in fn00000000004028C0)
;;     00000000004030EF (in fn00000000004028C0)
;;     00000000004037C5 (in fn00000000004028C0)
fn0000000000410E90 proc
	push	r15
	push	r14
	mov	r14d,edx
	push	r13
	push	r12
	push	rbp
	push	rbx
	sub	rsp,28h
	cmp	edx,24h
	ja	4112B0h

l0000000000410EAA:
	lea	rax,[rsp+18h]
	mov	rbp,rdi
	mov	r15,rsi
	test	rsi,rsi
	mov	r13,rcx
	mov	r12,r8
	cmovz	r15,rax

l0000000000410EC2:
	movzx	ebx,byte ptr [rdi]
	call	402880h
	mov	rdx,[rax]
	mov	rax,rbp
	jmp	410EDFh
0000000000410ED2       66 0F 1F 44 00 00                           f..D..        

l0000000000410ED8:
	add	rax,1h
	movzx	ebx,byte ptr [rax]

l0000000000410EDF:
	movzx	r9d,bl
	test	byte ptr [rdx+r9*2+1h],20h
	jnz	410ED8h

l0000000000410EEB:
	cmp	bl,2Dh
	jnz	410F08h

l0000000000410EF0:
	mov	eax,4h

l0000000000410EF5:
	add	rsp,28h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
0000000000410F04             0F 1F 40 00                             ..@.        

l0000000000410F08:
	call	402230h
	mov	edx,r14d
	mov	dword ptr [rax],0h
	mov	rsi,r15
	mov	rdi,rbp
	mov	[rsp+8h],rax
	call	4027A0h
	mov	r14,[r15]
	mov	rbx,rax
	mov	rcx,[rsp+8h]
	cmp	r14,rbp
	jz	410FE7h

l0000000000410F3A:
	mov	eax,[rcx]
	test	eax,eax
	jnz	410F68h

l0000000000410F40:
	xor	ebp,ebp

l0000000000410F42:
	test	r12,r12
	jz	410F4Fh

l0000000000410F47:
	movzx	edx,byte ptr [r14]
	test	dl,dl
	jnz	410F78h

l0000000000410F4F:
	mov	[r13+0h],rbx
	add	rsp,28h
	mov	eax,ebp
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
0000000000410F64             0F 1F 40 00                             ..@.        

l0000000000410F68:
	cmp	eax,22h
	mov	ebp,1h
	jnz	410EF0h

l0000000000410F76:
	jmp	410F42h

l0000000000410F78:
	movsx	esi,dl
	mov	rdi,r12
	mov	[rsp+8h],edx
	call	4023D0h
	test	rax,rax
	mov	edx,[rsp+8h]
	jz	411028h

l0000000000410F94:
	mov	esi,30h
	mov	rdi,r12
	mov	[rsp+8h],edx
	call	4023D0h
	test	rax,rax
	mov	edx,[rsp+8h]
	jz	410FCBh

l0000000000410FAE:
	movzx	eax,byte ptr [r14+1h]
	cmp	al,44h
	jz	411262h

l0000000000410FBB:
	cmp	al,69h
	jz	41124Ah

l0000000000410FC3:
	cmp	al,42h
	jz	411262h

l0000000000410FCB:
	mov	ecx,1h
	mov	eax,400h

l0000000000410FD5:
	sub	edx,42h
	cmp	dl,35h
	ja	411028h

l0000000000410FDD:
	movzx	edx,dl
	jmp	qword ptr [416C18h+rdx*8]

l0000000000410FE7:
	test	r12,r12
	jz	410EF0h

l0000000000410FF0:
	movzx	edx,byte ptr [rbp+0h]
	test	dl,dl
	jz	410EF0h

l0000000000410FFC:
	movsx	esi,dl
	mov	rdi,r12
	mov	[rsp+8h],edx
	xor	ebp,ebp
	mov	ebx,1h
	call	4023D0h
	test	rax,rax
	mov	edx,[rsp+8h]
	jnz	410F94h

l000000000041101F:
	jmp	410EF0h
0000000000411024             0F 1F 40 00                             ..@.        

l0000000000411028:
	mov	eax,ebp
	mov	[r13+0h],rbx
	or	eax,2h
	jmp	410EF5h
0000000000411036                   31 D2 09 D5 48 63 C9 49 01 CE       1...Hc.I..
0000000000411040 89 E8 83 C8 02 4D 89 37 41 80 3E 00 0F 45 E8 E9 .....M.7A.>..E..
0000000000411050 FB FE FF FF 48 85 DB 0F 88 26 02 00 00 48 01 DB ....H....&...H..
0000000000411060 31 D2 EB D4 48 B8 FF FF FF FF FF FF 7F 00 48 39 1...H.........H9
0000000000411070 C3 0F 87 0C 02 00 00 48 C1 E3 09 31 D2 EB B9 48 .......H...1...H
0000000000411080 63 F8 31 D2 48 C7 C0 FF FF FF FF 48 F7 F7 BE 07 c.1.H......H....
0000000000411090 00 00 00 31 D2 EB 0F 48 0F AF DF 45 31 C0 44 09 ...1...H...E1.D.
00000000004110A0 C2 83 EE 01 74 92 48 39 D8 73 EC 48 C7 C3 FF FF ....t.H9.s.H....
00000000004110B0 FF FF 41 B8 01 00 00 00 EB E4 48 63 F8 31 D2 48 ..A.......Hc.1.H
00000000004110C0 C7 C0 FF FF FF FF 48 F7 F7 BE 08 00 00 00 31 D2 ......H.......1.
00000000004110D0 EB 13 48 0F AF DF 45 31 C0 44 09 C2 83 EE 01 0F ..H...E1.D......
00000000004110E0 84 53 FF FF FF 48 39 D8 73 E8 48 C7 C3 FF FF FF .S...H9.s.H.....
00000000004110F0 FF 41 B8 01 00 00 00 EB E0 48 63 F0 31 D2 48 C7 .A.......Hc.1.H.
0000000000411100 C0 FF FF FF FF 48 F7 F6 BF 04 00 00 00 31 D2 48 .....H.......1.H
0000000000411110 39 D8 0F 82 59 01 00 00 48 0F AF DE 45 31 C0 44 9...Y...H...E1.D
0000000000411120 09 C2 83 EF 01 75 E8 E9 0C FF FF FF 48 63 F0 31 .....u......Hc.1
0000000000411130 D2 48 C7 C0 FF FF FF FF 48 F7 F6 BF 05 00 00 00 .H......H.......
0000000000411140 31 D2 EB 13 48 0F AF DE 45 31 C0 44 09 C2 83 EF 1...H...E1.D....
0000000000411150 01 0F 84 E1 FE FF FF 48 39 D8 73 E8 48 C7 C3 FF .......H9.s.H...
0000000000411160 FF FF FF 41 B8 01 00 00 00 EB E0 48 C7 C6 FF FF ...A.......H....
0000000000411170 FF FF 48 63 F8 31 D2 48 89 F0 48 F7 F7 48 39 C3 ..Hc.1.H..H..H9.
0000000000411180 0F 87 1D 01 00 00 48 0F AF DF 48 39 D8 0F 82 10 ......H...H9....
0000000000411190 01 00 00 48 0F AF DF 31 D2 E9 9A FE FF FF 48 C7 ...H...1......H.
00000000004111A0 C6 FF FF FF FF 48 63 F8 31 D2 48 89 F0 48 F7 F7 .....Hc.1.H..H..
00000000004111B0 48 39 C3 76 DE 48 89 F3 BA 01 00 00 00 E9 76 FE H9.v.H........v.
00000000004111C0 FF FF 48 63 F8 31 D2 48 C7 C0 FF FF FF FF 48 F7 ..Hc.1.H......H.
00000000004111D0 F7 BE 06 00 00 00 31 D2 EB 13 48 0F AF DF 45 31 ......1...H...E1
00000000004111E0 C0 44 09 C2 83 EE 01 0F 84 4B FE FF FF 48 39 D8 .D.......K...H9.
00000000004111F0 73 E8 48 C7 C3 FF FF FF FF 41 B8 01 00 00 00 EB s.H......A......
0000000000411200 E0 48 B8 FF FF FF FF FF FF 3F 00 48 39 C3 77 73 .H.......?.H9.ws
0000000000411210 48 C1 E3 0A 31 D2 E9 1D FE FF FF 48 63 F0 31 D2 H...1......Hc.1.
0000000000411220 48 C7 C0 FF FF FF FF 48 F7 F6 BF 03 00 00 00 31 H......H.......1
0000000000411230 D2 48 39 D8 72 5E 48 0F AF DE 45 31 C0 44 09 C2 .H9.r^H...E1.D..
0000000000411240 83 EF 01 75 EC E9 EE FD FF FF                   ...u......      

l000000000041124A:
	xor	ecx,ecx
	cmp	byte ptr [r14+2h],42h
	mov	eax,400h
	setz	cl
	lea	ecx,[rcx+rcx+1h]
	jmp	410FD5h

l0000000000411262:
	mov	ecx,2h
	mov	eax,3E8h
	jmp	410FD5h
0000000000411271    48 C7 C3 FF FF FF FF 41 B8 01 00 00 00 E9 9C  H......A.......
0000000000411280 FE FF FF 48 C7 C3 FF FF FF FF BA 01 00 00 00 E9 ...H............
0000000000411290 A4 FD FF FF 48 C7 C3 FF FF FF FF 41 B8 01 00 00 ....H......A....
00000000004112A0 00 EB 9A BA 01 00 00 00 48 89 F3 E9 88 FD FF FF ........H.......

l00000000004112B0:
	mov	ecx,416DC8h
	mov	edx,60h
	mov	esi,416BE0h
	mov	edi,416BF0h
	call	402450h
00000000004112C9                            0F 1F 80 00 00 00 00          .......

;; fn00000000004112D0: 00000000004112D0
;;   Called from:
;;     00000000004043D4 (in fn00000000004028C0)
fn00000000004112D0 proc
	push	r13
	movsxd	r10,esi
	push	r12
	mov	r12,r8
	push	rbp
	push	rbx
	sub	rsp,18h
	cmp	edi,3h
	mov	ebp,[000000000061A580]                                 ; [rip+00209297]
	ja	41133Bh

l00000000004112EB:
	cmp	edi,2h
	jnc	411334h

l00000000004112F0:
	sub	edi,1h
	mov	esi,416DECh
	jnz	41132Fh

l00000000004112FA:
	test	r10d,r10d
	js	411347h

l00000000004112FF:
	shl	r10,5h
	mov	ebx,416E09h
	mov	r13,[rcx+r10]

l000000000041130C:
	mov	edx,5h
	xor	edi,edi
	call	402360h
	mov	r9,r12
	mov	rdx,rax
	mov	r8,r13
	mov	rcx,rbx
	xor	esi,esi
	mov	edi,ebp
	xor	eax,eax
	call	402770h

l000000000041132F:
	call	402220h

l0000000000411334:
	mov	esi,416E10h
	jmp	4112FAh

l000000000041133B:
	cmp	edi,4h
	mov	esi,416DD1h
	jz	4112FAh

l0000000000411345:
	jmp	41132Fh

l0000000000411347:
	mov	ebx,416E09h
	mov	[rsp],dl
	mov	byte ptr [rsp+1h],0h
	sub	rbx,r10
	mov	r13,rsp
	jmp	41130Ch
000000000041135C                                     0F 1F 40 00             ..@.

;; fn0000000000411360: 0000000000411360
;;   Called from:
;;     000000000040C8C3 (in fn000000000040C810)
fn0000000000411360 proc
	push	r15
	push	r14
	mov	r14d,edx
	push	r13
	push	r12
	push	rbp
	push	rbx
	sub	rsp,28h
	cmp	edx,24h
	ja	411788h

l000000000041137A:
	lea	rax,[rsp+18h]
	mov	rbp,rdi
	mov	r15,rsi
	test	rsi,rsi
	mov	r13,rcx
	mov	r12,r8
	cmovz	r15,rax

l0000000000411392:
	movzx	ebx,byte ptr [rdi]
	call	402880h
	mov	rdx,[rax]
	mov	rax,rbp
	jmp	4113AFh
00000000004113A2       66 0F 1F 44 00 00                           f..D..        

l00000000004113A8:
	add	rax,1h
	movzx	ebx,byte ptr [rax]

l00000000004113AF:
	movzx	r9d,bl
	test	byte ptr [rdx+r9*2+1h],20h
	jnz	4113A8h

l00000000004113BB:
	cmp	bl,2Dh
	jnz	4113D8h

l00000000004113C0:
	mov	eax,4h

l00000000004113C5:
	add	rsp,28h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
00000000004113D4             0F 1F 40 00                             ..@.        

l00000000004113D8:
	call	402230h
	xor	ecx,ecx
	mov	edx,r14d
	mov	dword ptr [rax],0h
	mov	rsi,r15
	mov	rdi,rbp
	mov	[rsp+8h],rax
	call	402460h
	mov	r14,[r15]
	mov	rbx,rax
	mov	r8,[rsp+8h]
	cmp	r14,rbp
	jz	4114BFh

l000000000041140C:
	mov	eax,[r8]
	test	eax,eax
	jnz	411440h

l0000000000411413:
	xor	ebp,ebp

l0000000000411415:
	test	r12,r12
	jz	411422h

l000000000041141A:
	movzx	edx,byte ptr [r14]
	test	dl,dl
	jnz	411450h

l0000000000411422:
	mov	[r13+0h],rbx
	add	rsp,28h
	mov	eax,ebp
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
0000000000411437                      66 0F 1F 84 00 00 00 00 00        f........

l0000000000411440:
	cmp	eax,22h
	mov	ebp,1h
	jnz	4113C0h

l000000000041144E:
	jmp	411415h

l0000000000411450:
	movsx	esi,dl
	mov	rdi,r12
	mov	[rsp+8h],edx
	call	4023D0h
	test	rax,rax
	mov	edx,[rsp+8h]
	jz	411500h

l000000000041146C:
	mov	esi,30h
	mov	rdi,r12
	mov	[rsp+8h],edx
	call	4023D0h
	test	rax,rax
	mov	edx,[rsp+8h]
	jz	4114A3h

l0000000000411486:
	movzx	eax,byte ptr [r14+1h]
	cmp	al,44h
	jz	41173Ah

l0000000000411493:
	cmp	al,69h
	jz	411722h

l000000000041149B:
	cmp	al,42h
	jz	41173Ah

l00000000004114A3:
	mov	ecx,1h
	mov	eax,400h

l00000000004114AD:
	sub	edx,42h
	cmp	dl,35h
	ja	411500h

l00000000004114B5:
	movzx	edx,dl
	jmp	qword ptr [416E38h+rdx*8]

l00000000004114BF:
	test	r12,r12
	jz	4113C0h

l00000000004114C8:
	movzx	edx,byte ptr [rbp+0h]
	test	dl,dl
	jz	4113C0h

l00000000004114D4:
	movsx	esi,dl
	mov	rdi,r12
	mov	[rsp+8h],edx
	xor	ebp,ebp
	mov	ebx,1h
	call	4023D0h
	test	rax,rax
	mov	edx,[rsp+8h]
	jnz	41146Ch

l00000000004114F7:
	jmp	4113C0h
00000000004114FC                                     0F 1F 40 00             ..@.

l0000000000411500:
	mov	eax,ebp
	mov	[r13+0h],rbx
	or	eax,2h
	jmp	4113C5h
000000000041150E                                           31 D2               1.
0000000000411510 09 D5 48 63 C9 49 01 CE 89 E8 83 C8 02 4D 89 37 ..Hc.I.......M.7
0000000000411520 41 80 3E 00 0F 45 E8 E9 F6 FE FF FF 48 85 DB 0F A.>..E......H...
0000000000411530 88 26 02 00 00 48 01 DB 31 D2 EB D4 48 B8 FF FF .&...H..1...H...
0000000000411540 FF FF FF FF 7F 00 48 39 C3 0F 87 0C 02 00 00 48 ......H9.......H
0000000000411550 C1 E3 09 31 D2 EB B9 48 63 F8 31 D2 48 C7 C0 FF ...1...Hc.1.H...
0000000000411560 FF FF FF 48 F7 F7 BE 07 00 00 00 31 D2 EB 0F 48 ...H.......1...H
0000000000411570 0F AF DF 45 31 C0 44 09 C2 83 EE 01 74 92 48 39 ...E1.D.....t.H9
0000000000411580 D8 73 EC 48 C7 C3 FF FF FF FF 41 B8 01 00 00 00 .s.H......A.....
0000000000411590 EB E4 48 63 F8 31 D2 48 C7 C0 FF FF FF FF 48 F7 ..Hc.1.H......H.
00000000004115A0 F7 BE 08 00 00 00 31 D2 EB 13 48 0F AF DF 45 31 ......1...H...E1
00000000004115B0 C0 44 09 C2 83 EE 01 0F 84 53 FF FF FF 48 39 D8 .D.......S...H9.
00000000004115C0 73 E8 48 C7 C3 FF FF FF FF 41 B8 01 00 00 00 EB s.H......A......
00000000004115D0 E0 48 63 F0 31 D2 48 C7 C0 FF FF FF FF 48 F7 F6 .Hc.1.H......H..
00000000004115E0 BF 04 00 00 00 31 D2 48 39 D8 0F 82 59 01 00 00 .....1.H9...Y...
00000000004115F0 48 0F AF DE 45 31 C0 44 09 C2 83 EF 01 75 E8 E9 H...E1.D.....u..
0000000000411600 0C FF FF FF 48 63 F0 31 D2 48 C7 C0 FF FF FF FF ....Hc.1.H......
0000000000411610 48 F7 F6 BF 05 00 00 00 31 D2 EB 13 48 0F AF DE H.......1...H...
0000000000411620 45 31 C0 44 09 C2 83 EF 01 0F 84 E1 FE FF FF 48 E1.D...........H
0000000000411630 39 D8 73 E8 48 C7 C3 FF FF FF FF 41 B8 01 00 00 9.s.H......A....
0000000000411640 00 EB E0 48 C7 C6 FF FF FF FF 48 63 F8 31 D2 48 ...H......Hc.1.H
0000000000411650 89 F0 48 F7 F7 48 39 C3 0F 87 1D 01 00 00 48 0F ..H..H9.......H.
0000000000411660 AF DF 48 39 D8 0F 82 10 01 00 00 48 0F AF DF 31 ..H9.......H...1
0000000000411670 D2 E9 9A FE FF FF 48 C7 C6 FF FF FF FF 48 63 F8 ......H......Hc.
0000000000411680 31 D2 48 89 F0 48 F7 F7 48 39 C3 76 DE 48 89 F3 1.H..H..H9.v.H..
0000000000411690 BA 01 00 00 00 E9 76 FE FF FF 48 63 F8 31 D2 48 ......v...Hc.1.H
00000000004116A0 C7 C0 FF FF FF FF 48 F7 F7 BE 06 00 00 00 31 D2 ......H.......1.
00000000004116B0 EB 13 48 0F AF DF 45 31 C0 44 09 C2 83 EE 01 0F ..H...E1.D......
00000000004116C0 84 4B FE FF FF 48 39 D8 73 E8 48 C7 C3 FF FF FF .K...H9.s.H.....
00000000004116D0 FF 41 B8 01 00 00 00 EB E0 48 B8 FF FF FF FF FF .A.......H......
00000000004116E0 FF 3F 00 48 39 C3 77 73 48 C1 E3 0A 31 D2 E9 1D .?.H9.wsH...1...
00000000004116F0 FE FF FF 48 63 F0 31 D2 48 C7 C0 FF FF FF FF 48 ...Hc.1.H......H
0000000000411700 F7 F6 BF 03 00 00 00 31 D2 48 39 D8 72 5E 48 0F .......1.H9.r^H.
0000000000411710 AF DE 45 31 C0 44 09 C2 83 EF 01 75 EC E9 EE FD ..E1.D.....u....
0000000000411720 FF FF                                           ..              

l0000000000411722:
	xor	ecx,ecx
	cmp	byte ptr [r14+2h],42h
	mov	eax,400h
	setz	cl
	lea	ecx,[rcx+rcx+1h]
	jmp	4114ADh

l000000000041173A:
	mov	ecx,2h
	mov	eax,3E8h
	jmp	4114ADh
0000000000411749                            48 C7 C3 FF FF FF FF          H......
0000000000411750 41 B8 01 00 00 00 E9 9C FE FF FF 48 C7 C3 FF FF A..........H....
0000000000411760 FF FF BA 01 00 00 00 E9 A4 FD FF FF 48 C7 C3 FF ............H...
0000000000411770 FF FF FF 41 B8 01 00 00 00 EB 9A BA 01 00 00 00 ...A............
0000000000411780 48 89 F3 E9 88 FD FF FF                         H.......        

l0000000000411788:
	mov	ecx,416FE8h
	mov	edx,60h
	mov	esi,416BE0h
	mov	edi,416BF0h
	call	402450h
00000000004117A1    66 2E 0F 1F 84 00 00 00 00 00 0F 1F 44 00 00  f...........D..

;; fn00000000004117B0: 00000000004117B0
;;   Called from:
;;     000000000041182F (in fn0000000000411820)
;;     000000000041184F (in fn0000000000411840)
fn00000000004117B0 proc
	sub	rsp,8h
	test	edi,edi
	jz	411800h

l00000000004117B8:
	cmp	edi,0Ah
	mov	eax,edi
	jz	4117C8h

l00000000004117BF:
	add	rsp,8h
	ret
00000000004117C4             0F 1F 40 00                             ..@.        

l00000000004117C8:
	mov	rdx,[rsi]
	mov	edi,4137B1h
	mov	ecx,0Ah
	mov	rsi,rdx

l00000000004117D8:
	rep cmpsb

l00000000004117DA:
	jnz	4117BFh

l00000000004117DC:
	mov	rdi,rdx
	call	4027D0h
	call	402230h
	mov	dword ptr [rax],3Dh
	mov	eax,0FFFFFFFFh
	jmp	4117BFh
00000000004117F6                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000411800:
	call	402230h
	mov	dword ptr [rax],5Fh
	mov	eax,0FFFFFFFFh
	jmp	4117BFh
0000000000411812       66 66 66 66 66 2E 0F 1F 84 00 00 00 00 00   fffff.........

;; fn0000000000411820: 0000000000411820
;;   Called from:
;;     0000000000408080 (in fn0000000000407EA0)
fn0000000000411820 proc
	push	rbx
	mov	rbx,rsi
	call	4025E0h
	mov	rsi,rbx
	mov	edi,eax
	pop	rbx
	jmp	4117B0h
0000000000411834             66 66 66 2E 0F 1F 84 00 00 00 00 00     fff.........

;; fn0000000000411840: 0000000000411840
;;   Called from:
;;     0000000000408458 (in fn0000000000407EA0)
fn0000000000411840 proc
	push	rbx
	mov	rbx,rsi
	call	402540h
	mov	rsi,rbx
	mov	edi,eax
	pop	rbx
	jmp	4117B0h
0000000000411854             66 66 66 2E 0F 1F 84 00 00 00 00 00     fff.........
0000000000411860 53 48 89 F3 E8 B7 0B FF FF 48 89 DE 89 C7 5B E9 SH.......H....[.
0000000000411870 3C FF FF FF 66 2E 0F 1F 84 00 00 00 00 00 66 90 <...f.........f.

;; fn0000000000411880: 0000000000411880
fn0000000000411880 proc
	push	r12
	push	rbp
	mov	rbp,rdi
	push	rbx
	call	402270h
	mov	ebx,[rbp+0h]
	mov	rdi,rbp
	mov	r12,rax
	call	411D30h
	and	ebx,20h
	test	eax,eax
	setnz	dl
	test	ebx,ebx
	jnz	4118C0h

l00000000004118A6:
	test	dl,dl
	jz	4118B4h

l00000000004118AA:
	test	r12,r12
	mov	ebx,0FFFFFFFFh
	jz	4118E0h

l00000000004118B4:
	mov	eax,ebx
	pop	rbx
	pop	rbp
	pop	r12
	ret
00000000004118BB                                  0F 1F 44 00 00            ..D..

l00000000004118C0:
	test	dl,dl
	mov	ebx,0FFFFFFFFh
	jnz	4118B4h

l00000000004118C9:
	call	402230h
	mov	dword ptr [rax],0h
	mov	eax,ebx
	pop	rbx
	pop	rbp
	pop	r12
	ret
00000000004118DB                                  0F 1F 44 00 00            ..D..

l00000000004118E0:
	call	402230h
	xor	ebx,ebx
	cmp	dword ptr [rax],9h
	setnz	bl
	neg	ebx
	mov	eax,ebx
	pop	rbx
	pop	rbp
	pop	r12
	ret
00000000004118F6                   66 2E 0F 1F 84 00 00 00 00 00       f.........

;; fn0000000000411900: 0000000000411900
;;   Called from:
;;     000000000040D7E8 (in fn000000000040D7B0)
fn0000000000411900 proc
	push	r15
	mov	edi,0Eh
	push	r14
	push	r13
	push	r12
	push	rbp
	push	rbx
	sub	rsp,+0A8h
	mov	rax,fs:[0028h]
	mov	[rsp+98h],rax
	xor	eax,eax
	call	402660h
	mov	r14,[000000000061B358]                                 ; [rip+00209A23]
	test	rax,rax
	mov	rbx,rax
	mov	eax,416919h
	cmovz	rbx,rax

l0000000000411944:
	test	r14,r14
	jnz	41196Ah

l0000000000411949:
	jmp	4119D4h
000000000041194E                                           66 90               f.

l0000000000411950:
	mov	rdi,r14
	call	402380h
	lea	rbp,[r14+rax+1h]
	mov	rdi,rbp
	call	402380h
	lea	r14,[rbp+rax+1h]

l000000000041196A:
	movzx	ebp,byte ptr [r14]
	test	bpl,bpl
	jz	41199Ch

l0000000000411973:
	mov	rsi,r14
	mov	rdi,rbx
	call	402550h
	test	eax,eax
	jz	41198Fh

l0000000000411982:
	cmp	bpl,2Ah
	jnz	411950h

l0000000000411988:
	cmp	byte ptr [r14+1h],0h
	jnz	411950h

l000000000041198F:
	mov	rdi,r14
	call	402380h
	lea	rbx,[r14+rax+1h]

l000000000041199C:
	cmp	byte ptr [rbx],0h
	mov	eax,416FFCh
	cmovz	rbx,rax

l00000000004119A8:
	mov	rcx,[rsp+98h]
	xor	rcx,fs:[0028h]
	mov	rax,rbx
	jnz	411CFDh

l00000000004119C2:
	add	rsp,+0A8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret

l00000000004119D4:
	mov	edi,417002h
	call	4021C0h
	test	rax,rax
	mov	r15,rax
	jz	4119EFh

l00000000004119E6:
	cmp	byte ptr [rax],0h
	jnz	411C52h

l00000000004119EF:
	mov	eax,7h
	mov	r12d,8h
	mov	r15d,416FF3h

l0000000000411A00:
	cmp	byte ptr [r15+rax],2Fh
	mov	r13d,1h
	mov	dword ptr [rsp+8h],1h
	jz	411C42h

l0000000000411A19:
	add	r13,r12
	lea	rdi,[r13+0Eh]
	call	402640h
	test	rax,rax
	mov	rbp,rax
	jz	411CF2h

l0000000000411A31:
	mov	rdx,r12
	mov	rsi,r15
	mov	rdi,rax
	call	4025C0h
	mov	edx,[rsp+8h]
	test	edx,edx
	jz	411A4Dh

l0000000000411A47:
	mov	byte ptr [rbp+0h],2Fh

l0000000000411A4D:
	add	r13,rbp
	mov	rax,2E74657372616863h
	mov	esi,20000h
	mov	[r13+0h],rax
	mov	eax,73h
	mov	dword ptr [r13+8h],61696C61h
	mov	[r13+0Ch],ax
	mov	rdi,rbp
	xor	eax,eax
	call	402780h
	test	eax,eax
	mov	r12d,eax
	js	411C28h

l0000000000411A8A:
	mov	esi,413A21h
	mov	edi,eax
	call	402700h
	test	rax,rax
	mov	r15,rax
	jz	411C20h

l0000000000411AA2:
	lea	r12,[rsp+20h]
	mov	qword ptr [rsp+8h],+0h

l0000000000411AB0:
	mov	rax,[r15+8h]
	cmp	rax,[r15+10h]
	jnc	411C01h

l0000000000411ABE:
	lea	rdx,[rax+1h]
	mov	[r15+8h],rdx
	movzx	edi,byte ptr [rax]

l0000000000411AC9:
	cmp	edi,20h
	jz	411AB0h

l0000000000411ACE:
	lea	eax,[rdi-9h]
	cmp	eax,1h
	jbe	411AB0h

l0000000000411AD6:
	cmp	edi,23h
	jz	411C75h

l0000000000411ADF:
	mov	rsi,r15
	call	402670h
	lea	rcx,[rsp+60h]
	xor	eax,eax
	mov	rdx,r12
	mov	esi,417012h
	mov	rdi,r15
	call	4024A0h
	cmp	eax,1h
	jle	411C9Ch

l0000000000411B07:
	mov	rdx,r12

l0000000000411B0A:
	mov	ecx,[rdx]
	add	rdx,4h
	lea	eax,[rcx-1010101h]
	not	ecx
	and	eax,ecx
	and	eax,80808080h
	jz	411B0Ah

l0000000000411B21:
	mov	ecx,eax
	lea	r10,[rsp+60h]
	shr	ecx,10h
	test	eax,8080h
	cmovz	eax,ecx

l0000000000411B33:
	lea	rcx,[rdx+2h]
	cmovz	rdx,rcx

l0000000000411B3B:
	add	al,al
	sbb	rdx,3h
	sub	rdx,r12

l0000000000411B44:
	mov	ecx,[r10]
	add	r10,4h
	lea	eax,[rcx-1010101h]
	not	ecx
	and	eax,ecx
	and	eax,80808080h
	jz	411B44h

l0000000000411B5C:
	mov	ecx,eax
	shr	ecx,10h
	test	eax,8080h
	cmovz	eax,ecx

l0000000000411B69:
	lea	rcx,[r10+2h]
	cmovz	r10,rcx

l0000000000411B71:
	add	al,al
	lea	rax,[rsp+60h]
	sbb	r10,3h
	sub	r10,rax
	cmp	qword ptr [rsp+8h],0h
	lea	rax,[r10+rdx]
	jnz	411CBCh

l0000000000411B8F:
	lea	rcx,[rax+2h]
	lea	rdi,[rax+3h]
	mov	[rsp+18h],rdx
	mov	[rsp+10h],r10
	mov	[rsp+8h],rcx
	call	402640h
	mov	r10,[rsp+10h]
	mov	rdx,[rsp+18h]
	mov	r13,rax

l0000000000411BB8:
	test	r13,r13
	jz	411D02h

l0000000000411BC1:
	mov	r14,[rsp+8h]
	mov	rdi,-2h
	mov	rsi,r12
	sub	rdi,rdx
	sub	r14,r10
	add	rdi,r14
	add	rdi,r13
	call	402260h
	lea	rdi,[r13+r14-1h]
	lea	rsi,[rsp+60h]
	mov	r14,r13
	call	402260h
	mov	rax,[r15+8h]
	cmp	rax,[r15+10h]
	jc	411ABEh

l0000000000411C01:
	mov	rdi,r15
	call	4021B0h
	cmp	eax,0FFh
	mov	edi,eax
	jz	411C9Ch

l0000000000411C14:
	jmp	411AC9h
0000000000411C19                            0F 1F 80 00 00 00 00          .......

l0000000000411C20:
	mov	edi,r12d
	call	4024C0h

l0000000000411C28:
	mov	r14d,416919h

l0000000000411C2E:
	mov	rdi,rbp
	call	4021F0h

l0000000000411C36:
	mov	[000000000061B358],r14                                 ; [rip+0020971B]
	jmp	41196Ah

l0000000000411C42:
	xor	r13d,r13d
	mov	dword ptr [rsp+8h],0h
	jmp	411A19h

l0000000000411C52:
	mov	rdi,rax
	call	402380h
	test	rax,rax
	mov	r12,rax
	jz	411C42h

l0000000000411C62:
	lea	rax,[rax-1h]
	jmp	411A00h
0000000000411C6B                                  0F 1F 44 00 00            ..D..

l0000000000411C70:
	cmp	eax,0FFh
	jz	411C93h

l0000000000411C75:
	mov	rax,[r15+8h]
	cmp	rax,[r15+10h]
	jnc	411D1Dh

l0000000000411C83:
	lea	rdx,[rax+1h]
	mov	[r15+8h],rdx
	movzx	eax,byte ptr [rax]

l0000000000411C8E:
	cmp	eax,0Ah
	jnz	411C70h

l0000000000411C93:
	cmp	eax,0FFh
	jnz	411AB0h

l0000000000411C9C:
	mov	rdi,r15
	call	411D30h
	mov	rax,[rsp+8h]
	test	rax,rax
	jz	411C28h

l0000000000411CB2:
	mov	byte ptr [r14+rax],0h
	jmp	411C2Eh

l0000000000411CBC:
	add	rax,[rsp+8h]
	mov	rdi,r14
	mov	[rsp+18h],r10
	mov	[rsp+10h],rdx
	lea	rcx,[rax+2h]
	lea	rsi,[rax+3h]
	mov	[rsp+8h],rcx
	call	4026E0h
	mov	r10,[rsp+18h]
	mov	r13,rax
	mov	rdx,[rsp+10h]
	jmp	411BB8h

l0000000000411CF2:
	mov	r14d,416919h
	jmp	411C36h

l0000000000411CFD:
	call	4023A0h

l0000000000411D02:
	mov	rdi,r14
	mov	r14d,416919h
	call	4021F0h
	mov	rdi,r15
	call	411D30h
	jmp	411C2Eh

l0000000000411D1D:
	mov	rdi,r15
	call	4021B0h
	jmp	411C8Eh
0000000000411D2A                               66 0F 1F 44 00 00           f..D..

;; fn0000000000411D30: 0000000000411D30
;;   Called from:
;;     0000000000411895 (in fn0000000000411880)
;;     0000000000411C9F (in fn0000000000411900)
;;     0000000000411D13 (in fn0000000000411900)
fn0000000000411D30 proc
	push	r12
	push	rbp
	push	rbx
	mov	rbx,rdi
	call	4025F0h
	test	eax,eax
	mov	rdi,rbx
	js	411D9Fh

l0000000000411D43:
	call	4026B0h
	test	eax,eax
	jnz	411D80h

l0000000000411D4C:
	mov	rdi,rbx
	call	411DB0h
	test	eax,eax
	jz	411D9Ch

l0000000000411D58:
	call	402230h
	mov	r12d,[rax]
	mov	rdi,rbx
	mov	rbp,rax
	call	402310h
	test	r12d,r12d
	jz	411D79h

l0000000000411D70:
	mov	[rbp+0h],r12d
	mov	eax,0FFFFFFFFh

l0000000000411D79:
	pop	rbx
	pop	rbp
	pop	r12
	ret
0000000000411D7E                                           66 90               f.

l0000000000411D80:
	mov	rdi,rbx
	call	4025F0h
	xor	esi,esi
	mov	edx,1h
	mov	edi,eax
	call	402430h
	cmp	rax,0FFh
	jnz	411D4Ch

l0000000000411D9C:
	mov	rdi,rbx

l0000000000411D9F:
	pop	rbx
	pop	rbp
	pop	r12
	jmp	402310h
0000000000411DA8                         0F 1F 84 00 00 00 00 00         ........

;; fn0000000000411DB0: 0000000000411DB0
;;   Called from:
;;     0000000000411D4F (in fn0000000000411D30)
fn0000000000411DB0 proc
	test	rdi,rdi
	push	rbx
	mov	rbx,rdi
	jz	411DC2h

l0000000000411DB9:
	call	4026B0h
	test	eax,eax
	jnz	411DD0h

l0000000000411DC2:
	mov	rdi,rbx
	pop	rbx
	jmp	402650h
0000000000411DCB                                  0F 1F 44 00 00            ..D..

l0000000000411DD0:
	test	dword ptr [rbx],100h
	jz	411DC2h

l0000000000411DD8:
	mov	rdi,rbx
	mov	edx,1h
	xor	esi,esi
	call	411DF0h
	mov	rdi,rbx
	pop	rbx
	jmp	402650h

;; fn0000000000411DF0: 0000000000411DF0
;;   Called from:
;;     0000000000411DE2 (in fn0000000000411DB0)
fn0000000000411DF0 proc
	push	rbx
	mov	rbx,rdi
	sub	rsp,10h
	mov	rax,[rdi+8h]
	cmp	[rdi+10h],rax
	jz	411E10h

l0000000000411E02:
	add	rsp,10h
	mov	rdi,rbx
	pop	rbx
	jmp	402790h
0000000000411E0F                                              90                .

l0000000000411E10:
	mov	rax,[rdi+20h]
	cmp	[rdi+28h],rax
	jnz	411E02h

l0000000000411E1A:
	cmp	qword ptr [rdi+48h],0h
	jnz	411E02h

l0000000000411E21:
	mov	[rsp+0Ch],edx
	mov	[rsp],rsi
	call	4025F0h
	mov	edx,[rsp+0Ch]
	mov	rsi,[rsp]
	mov	edi,eax
	call	402430h
	cmp	rax,0FFh
	jz	411E4Fh

l0000000000411E43:
	and	dword ptr [rbx],0EFh
	mov	[rbx+90h],rax
	xor	eax,eax

l0000000000411E4F:
	add	rsp,10h
	pop	rbx
	ret
0000000000411E55                66 2E 0F 1F 84 00 00 00 00 00 90      f..........
0000000000411E60 41 57 41 89 FF 41 56 49 89 F6 41 55 49 89 D5 41 AWA..AVI..AUI..A
0000000000411E70 54 4C 8D 25 78 7F 20 00 55 48 8D 2D 78 7F 20 00 TL.%x. .UH.-x. .
0000000000411E80 53 4C 29 E5 31 DB 48 C1 FD 03 48 83 EC 08 E8 D5 SL).1.H...H.....
0000000000411E90 02 FF FF 48 85 ED 74 1E 0F 1F 84 00 00 00 00 00 ...H..t.........
0000000000411EA0 4C 89 EA 4C 89 F6 44 89 FF 41 FF 14 DC 48 83 C3 L..L..D..A...H..
0000000000411EB0 01 48 39 EB 75 EA 48 83 C4 08 5B 5D 41 5C 41 5D .H9.u.H...[]A\A]
0000000000411EC0 41 5E 41 5F C3 66 66 2E 0F 1F 84 00 00 00 00 00 A^A_.ff.........
0000000000411ED0 F3 C3 66 2E 0F 1F 84 00 00 00 00 00 0F 1F 40 00 ..f...........@.

;; fn0000000000411EE0: 0000000000411EE0
;;   Called from:
;;     0000000000402928 (in fn00000000004028C0)
fn0000000000411EE0 proc
	lea	rax,[000000000061A3A8]                                 ; [rip+002084C1]
	test	rax,rax
	jz	411EF6h

l0000000000411EEC:
	mov	rdx,[rax]

l0000000000411EEF:
	xor	esi,esi
	jmp	4027B0h

l0000000000411EF6:
	xor	edx,edx
	jmp	411EEFh
