;;; Segment .text (0000000140001000)

;; fn0000000140001000: 0000000140001000
;;   Called from:
;;     00000001400013D4 (in fn00000001400012BC)
fn0000000140001000 proc
	sub	rsp,+0C8h
	mov	rax,[0000000140003000]                                 ; [rip+00001FF2]
	xor	rax,rsp
	mov	[rsp+0B8h],rax
	movaps	xmm0,[0000000140002260]                             ; [rip+00001240]
	lea	r9,[0000000140002240]                                  ; [rip+00001219]
	mov	dword ptr [rsp+38h],57h
	lea	r8,[0000000140002250]                                  ; [rip+0000121A]
	mov	dword ptr [rsp+30h],63h
	lea	rcx,[0000000140002210]                                 ; [rip+000011CB]
	mov	edx,3h
	movups	[rsp+20h],xmm0
	call	fn0000000140001140
	lea	rax,[rsp+44h]
	mov	[rsp+38h],rax
	lea	r9,[rsp+78h]
	lea	rax,[rsp+40h]
	mov	[rsp+30h],rax
	lea	r8,[rsp+58h]
	lea	rax,[rsp+50h]
	mov	[rsp+28h],rax
	lea	rdx,[rsp+4Ch]
	lea	rax,[rsp+48h]
	lea	rcx,[0000000140002228]                                 ; [rip+0000119B]
	mov	[rsp+20h],rax
	call	fn00000001400010D0
	xor	eax,eax
	mov	rcx,[rsp+0B8h]
	xor	rcx,rsp
	call	fn00000001400011B0
	add	rsp,+0C8h
	ret
00000001400010B1    CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC  ...............

;; fn00000001400010C0: 00000001400010C0
;;   Called from:
;;     00000001400010FE (in fn00000001400010D0)
;;     0000000140001949 (in fn000000014000193C)
fn00000001400010C0 proc
	lea	rax,[0000000140003628]                                 ; [rip+00002561]
	ret
00000001400010C8                         CC CC CC CC CC CC CC CC         ........

;; fn00000001400010D0: 00000001400010D0
;;   Called from:
;;     0000000140001092 (in fn0000000140001000)
fn00000001400010D0 proc
	mov	[rsp+8h],rcx
	mov	[rsp+10h],rdx
	mov	[rsp+18h],r8
	mov	[rsp+20h],r9
	push	rbx
	push	rsi
	push	rdi
	sub	rsp,30h
	mov	rdi,rcx
	lea	rsi,[rsp+58h]
	xor	ecx,ecx
	call	[0000000140002178]                                    ; [rip+0000107D]
	mov	rbx,rax
	call	fn00000001400010C0
	xor	r9d,r9d
	mov	[rsp+20h],rsi
	mov	r8,rdi
	mov	rdx,rbx
	mov	rcx,[rax]
	call	[0000000140002168]                                    ; [rip+0000104E]
	add	rsp,30h
	pop	rdi
	pop	rsi
	pop	rbx
	ret
0000000140001122       CC CC CC CC CC CC CC CC CC CC CC CC CC CC   ..............

;; fn0000000140001130: 0000000140001130
;;   Called from:
;;     0000000140001171 (in fn0000000140001140)
;;     0000000140001940 (in fn000000014000193C)
fn0000000140001130 proc
	lea	rax,[0000000140003620]                                 ; [rip+000024E9]
	ret
0000000140001138                         CC CC CC CC CC CC CC CC         ........

;; fn0000000140001140: 0000000140001140
;;   Called from:
;;     000000014000104F (in fn0000000140001000)
fn0000000140001140 proc
	mov	[rsp+8h],rcx
	mov	[rsp+10h],rdx
	mov	[rsp+18h],r8
	mov	[rsp+20h],r9
	push	rbx
	push	rsi
	push	rdi
	sub	rsp,30h
	mov	rdi,rcx
	lea	rsi,[rsp+58h]
	mov	ecx,1h
	call	[0000000140002178]                                    ; [rip+0000100A]
	mov	rbx,rax
	call	fn0000000140001130
	xor	r9d,r9d
	mov	[rsp+20h],rsi
	mov	r8,rdi
	mov	rdx,rbx
	mov	rcx,[rax]
	call	[0000000140002170]                                    ; [rip+00000FE3]
	add	rsp,30h
	pop	rdi
	pop	rsi
	pop	rbx
	ret
0000000140001195                CC CC CC CC CC CC CC CC CC CC CC      ...........
00000001400011A0 CC CC CC CC CC CC 66 66 0F 1F 84 00 00 00 00 00 ......ff........

;; fn00000001400011B0: 00000001400011B0
;;   Called from:
;;     00000001400010A4 (in fn0000000140001000)
;;     0000000140001EF0 (in fn0000000140001E9C)
fn00000001400011B0 proc
	cmp	rcx,[0000000140003000]                                 ; [rip+00001E49]
	repne jnz	1400011CCh

l00000001400011BA:
	rol	rcx,10h
	test	cx,0FFFFh
	repne jnz	1400011C8h

l00000001400011C6:
	repne ret

l00000001400011C8:
	ror	rcx,10h

l00000001400011CC:
	jmp	fn000000014000147C
00000001400011D1    CC CC CC                                      ...            

;; fn00000001400011D4: 00000001400011D4
fn00000001400011D4 proc
	push	rbx
	sub	rsp,20h
	mov	ecx,1h
	call	140001DF2h
	call	fn0000000140001920
	mov	ecx,eax
	call	140001E28h
	call	140001E58h
	mov	rbx,rax
	call	fn0000000140001ABC
	mov	ecx,1h
	mov	[rbx],eax
	call	fn000000014000164C
	test	al,al
	jz	140001279h

l000000014000120D:
	call	fn0000000140001B5C
	lea	rcx,[fn0000000140001BA8]                               ; [rip+0000098F]
	call	fn0000000140001854
	call	fn0000000140001918
	mov	ecx,eax
	call	140001DFEh
	test	eax,eax
	jnz	140001284h

l000000014000122E:
	call	fn0000000140001928
	call	fn0000000140001958
	test	eax,eax
	jz	140001248h

l000000014000123C:
	lea	rcx,[fn0000000140001ABC]                               ; [rip+00000879]
	call	140001DF8h

l0000000140001248:
	call	fn0000000140001DD0
	call	fn0000000140001DD0
	call	fn0000000140001ABC
	mov	ecx,eax
	call	140001E4Ch
	call	fn0000000140001938
	test	al,al
	jz	14000126Ch

l0000000140001267:
	call	140001E04h

l000000014000126C:
	call	fn0000000140001ABC
	xor	eax,eax
	add	rsp,20h
	pop	rbx
	ret

l0000000140001279:
	mov	ecx,7h
	call	fn0000000140001974
	int	3h

l0000000140001284:
	mov	ecx,7h
	call	fn0000000140001974
	int	3h
	int	3h

;; fn0000000140001290: 0000000140001290
;;   Called from:
;;     000000014000128F (in fn00000001400011D4)
fn0000000140001290 proc
	sub	rsp,28h
	call	fn000000014000193C
	xor	eax,eax
	add	rsp,28h
	ret

;; fn00000001400012A0: 00000001400012A0
fn00000001400012A0 proc
	sub	rsp,28h
	call	fn0000000140001B14
	call	fn0000000140001ABC
	mov	ecx,eax
	add	rsp,28h
	jmp	140001E52h
00000001400012B9                            CC CC CC                      ...    

;; fn00000001400012BC: 00000001400012BC
;;   Called from:
;;     0000000140001441 (in Win32CrtStartup)
fn00000001400012BC proc
	mov	[rsp+8h],rbx
	mov	[rsp+10h],rsi
	push	rdi
	sub	rsp,30h
	mov	ecx,1h
	call	fn0000000140001600
	test	al,al
	jnz	1400012E4h

l00000001400012D9:
	mov	ecx,7h
	call	fn0000000140001974
	int	3h

l00000001400012E4:
	xor	sil,sil
	mov	[rsp+20h],sil
	call	fn00000001400015C4
	mov	bl,al
	mov	ecx,[00000001400035B0]                                 ; [rip+000022B7]
	cmp	ecx,1h
	jnz	140001308h

l00000001400012FE:
	mov	ecx,7h
	call	fn0000000140001974

l0000000140001308:
	test	ecx,ecx
	jnz	140001356h

l000000014000130C:
	mov	[00000001400035B0],1h                                  ; [rip+0000229A]
	lea	rdx,[00000001400021D0]                                 ; [rip+00000EB3]
	lea	rcx,[00000001400021B8]                                 ; [rip+00000E94]
	call	140001E16h
	test	eax,eax
	jz	140001337h

l000000014000132D:
	mov	eax,0FFh
	jmp	140001423h

l0000000140001337:
	lea	rdx,[00000001400021B0]                                 ; [rip+00000E72]
	lea	rcx,[00000001400021A0]                                 ; [rip+00000E5B]
	call	140001E10h
	mov	[00000001400035B0],2h                                  ; [rip+0000225C]
	jmp	14000135Eh

l0000000140001356:
	mov	sil,1h
	mov	[rsp+20h],sil

l000000014000135E:
	mov	cl,bl
	call	fn00000001400017B4
	call	fn0000000140001964
	mov	rbx,rax
	cmp	qword ptr [rax],0h
	jz	140001395h

l0000000140001373:
	mov	rcx,rax
	call	fn0000000140001718
	test	al,al
	jz	140001395h

l000000014000137F:
	mov	rbx,[rbx]
	mov	rcx,rbx
	call	fn0000000140001BF4
	xor	r8d,r8d
	lea	edx,[r8+2h]
	xor	ecx,ecx
	call	rbx

l0000000140001395:
	call	fn000000014000196C
	mov	rbx,rax
	cmp	qword ptr [rax],0h
	jz	1400013B7h

l00000001400013A3:
	mov	rcx,rax
	call	fn0000000140001718
	test	al,al
	jz	1400013B7h

l00000001400013AF:
	mov	rcx,[rbx]
	call	140001E46h

l00000001400013B7:
	call	140001E34h
	mov	rdi,rax
	call	140001E2Eh
	mov	rbx,rax
	call	140001E0Ah
	mov	r8,rax
	mov	rdx,[rdi]
	mov	ecx,[rbx]
	call	fn0000000140001000
	mov	ebx,eax
	call	fn0000000140001AC0
	test	al,al
	jnz	1400013EBh

l00000001400013E4:
	mov	ecx,ebx
	call	140001E1Ch

l00000001400013EB:
	test	sil,sil
	jnz	1400013F5h

l00000001400013F0:
	call	140001E3Ah

l00000001400013F5:
	xor	edx,edx
	mov	cl,1h
	call	fn00000001400017D8
	mov	eax,ebx
	jmp	140001423h
0000000140001402       8B D8 E8 B7 06 00 00 84 C0 75 08 8B CB E8   .........u....
0000000140001410 0E 0A 00 00 CC 80 7C 24 20 00 75 05 E8 1F 0A 00 ......|$ .u.....
0000000140001420 00 8B C3                                        ...             

l0000000140001423:
	mov	rbx,[rsp+40h]
	mov	rsi,[rsp+48h]
	add	rsp,30h
	pop	rdi
	ret
0000000140001433          CC                                        .            

;; Win32CrtStartup: 0000000140001434
Win32CrtStartup proc
	sub	rsp,28h
	call	fn000000014000186C
	add	rsp,28h
	jmp	fn00000001400012BC
0000000140001446                   CC CC                               ..        

;; fn0000000140001448: 0000000140001448
;;   Called from:
;;     0000000140001543 (in fn000000014000147C)
fn0000000140001448 proc
	push	rbx
	sub	rsp,20h
	mov	rbx,rcx
	xor	ecx,ecx
	call	[0000000140002068]                                    ; [rip+00000C0F]
	mov	rcx,rbx
	call	[0000000140002010]                                    ; [rip+00000BAE]
	call	[0000000140002060]                                    ; [rip+00000BF8]
	mov	rcx,rax
	mov	edx,0C0000409h
	add	rsp,20h
	pop	rbx
	jmp	[0000000140002058]                                     ; [rip+00000BDC]

;; fn000000014000147C: 000000014000147C
;;   Called from:
;;     00000001400011CC (in fn00000001400011B0)
fn000000014000147C proc
	mov	[rsp+8h],rcx
	sub	rsp,38h
	mov	ecx,17h
	call	140001E76h
	test	eax,eax
	jz	14000149Ah

l0000000140001493:
	mov	ecx,2h
	int	29h

l000000014000149A:
	lea	rcx,[00000001400030E0]                                 ; [rip+00001C3F]
	call	fn0000000140001550
	mov	rax,[rsp+38h]
	mov	[00000001400031D8],rax                                 ; [rip+00001D26]
	lea	rax,[rsp+38h]
	add	rax,8h
	mov	[0000000140003178],rax                                 ; [rip+00001CB6]
	mov	rax,[00000001400031D8]                                 ; [rip+00001D0F]
	mov	[0000000140003050],rax                                 ; [rip+00001B80]
	mov	rax,[rsp+40h]
	mov	[0000000140003160],rax                                 ; [rip+00001C84]
	mov	[0000000140003040],0C0000409h                          ; [rip+00001B5A]
	mov	[0000000140003044],1h                                  ; [rip+00001B54]
	mov	[0000000140003058],1h                                  ; [rip+00001B5E]
	mov	eax,8h
	imul	rax,rax,0h
	lea	rcx,[0000000140003060]                                 ; [rip+00001B56]
	mov	qword ptr [rcx+rax],+2h
	mov	eax,8h
	imul	rax,rax,0h
	mov	rcx,[0000000140003000]                                 ; [rip+00001ADE]
	mov	[rsp+rax+20h],rcx
	mov	eax,8h
	imul	rax,rax,1h
	mov	rcx,[0000000140003008]                                 ; [rip+00001AD1]
	mov	[rsp+rax+20h],rcx
	lea	rcx,[0000000140002200]                                 ; [rip+00000CBD]
	call	fn0000000140001448
	add	rsp,38h
	ret
000000014000154D                                        CC CC CC              ...

;; fn0000000140001550: 0000000140001550
;;   Called from:
;;     00000001400014A1 (in fn000000014000147C)
fn0000000140001550 proc
	push	rbx
	push	rsi
	push	rdi
	sub	rsp,40h
	mov	rbx,rcx
	call	[0000000140002070]                                    ; [rip+00000B0F]
	mov	rsi,[rbx+0F8h]
	xor	edi,edi

l000000014000156A:
	xor	r8d,r8d
	lea	rdx,[rsp+60h]
	mov	rcx,rsi
	call	[0000000140002000]                                    ; [rip+00000A85]
	test	rax,rax
	jz	1400015B9h

l0000000140001580:
	and	qword ptr [rsp+38h],0h
	lea	rcx,[rsp+68h]
	mov	rdx,[rsp+60h]
	mov	r9,rax
	mov	[rsp+30h],rcx
	mov	r8,rsi
	lea	rcx,[rsp+70h]
	mov	[rsp+28h],rcx
	xor	ecx,ecx
	mov	[rsp+20h],rbx
	call	[0000000140002008]                                    ; [rip+00000A56]
	inc	edi
	cmp	edi,2h
	jl	14000156Ah

l00000001400015B9:
	add	rsp,40h
	pop	rdi
	pop	rsi
	pop	rbx
	ret
00000001400015C1    CC CC CC                                      ...            

;; fn00000001400015C4: 00000001400015C4
;;   Called from:
;;     00000001400012EC (in fn00000001400012BC)
fn00000001400015C4 proc
	sub	rsp,28h
	call	fn0000000140001DC4
	test	eax,eax
	jz	1400015F2h

l00000001400015D1:
	mov	rax,gs:[0030h]
	mov	rcx,[rax+8h]
	jmp	1400015E5h

l00000001400015E0:
	cmp	rcx,rax
	jz	1400015F9h

l00000001400015E5:
	xor	eax,eax
	lock
	cmpxchg	[00000001400035B8],rcx                             ; [rip+00001FC8]
	jnz	1400015E0h

l00000001400015F2:
	xor	al,al

l00000001400015F4:
	add	rsp,28h
	ret

l00000001400015F9:
	mov	al,1h
	jmp	1400015F4h
00000001400015FD                                        CC CC CC              ...

;; fn0000000140001600: 0000000140001600
;;   Called from:
;;     00000001400012D0 (in fn00000001400012BC)
fn0000000140001600 proc
	push	rbx
	sub	rsp,20h
	movzx	eax,[00000001400035F0]                               ; [rip+00001FE3]
	test	ecx,ecx
	mov	ebx,1h
	cmovz	eax,ebx

l0000000140001617:
	mov	[00000001400035F0],al                                  ; [rip+00001FD3]
	call	fn0000000140001BFC
	call	fn0000000140001938
	test	al,al
	jnz	14000162Fh

l000000014000162B:
	xor	al,al
	jmp	140001643h

l000000014000162F:
	call	fn0000000140001938
	test	al,al
	jnz	140001641h

l0000000140001638:
	xor	ecx,ecx
	call	fn0000000140001938
	jmp	14000162Bh

l0000000140001641:
	mov	al,bl

l0000000140001643:
	add	rsp,20h
	pop	rbx
	ret
0000000140001649                            CC CC CC                      ...    

;; fn000000014000164C: 000000014000164C
;;   Called from:
;;     0000000140001204 (in fn00000001400011D4)
fn000000014000164C proc
	mov	[rsp+8h],rbx
	push	rbp
	mov	rbp,rsp
	sub	rsp,40h
	mov	ebx,ecx
	cmp	ecx,1h
	ja	14000170Ah

l0000000140001664:
	call	fn0000000140001DC4
	test	eax,eax
	jz	140001698h

l000000014000166D:
	test	ebx,ebx
	jnz	140001698h

l0000000140001671:
	lea	rcx,[00000001400035C0]                                 ; [rip+00001F48]
	call	140001E5Eh
	test	eax,eax
	jz	140001685h

l0000000140001681:
	xor	al,al
	jmp	1400016FFh

l0000000140001685:
	lea	rcx,[00000001400035D8]                                 ; [rip+00001F4C]
	call	140001E5Eh
	test	eax,eax
	setz	al
	jmp	1400016FFh

l0000000140001698:
	mov	rdx,[0000000140003000]                                 ; [rip+00001961]
	or	r8,0FFh
	mov	eax,edx
	mov	ecx,40h
	and	eax,3Fh
	sub	ecx,eax
	mov	al,1h
	ror	r8,cl
	xor	r8,rdx
	mov	[rbp-20h],r8
	mov	[rbp-18h],r8
	movups	xmm0,[rbp-20h]
	mov	[rbp-10h],r8
	movsd	xmm1,double ptr [rbp-10h]
	movups	[00000001400035C0],xmm0                             ; [rip+00001EED]
	mov	[rbp-20h],r8
	mov	[rbp-18h],r8
	movups	xmm0,[rbp-20h]
	mov	[rbp-10h],r8
	movsd	[00000001400035D0],xmm1                              ; [rip+00001EE5]
	movsd	xmm1,double ptr [rbp-10h]
	movups	[00000001400035D8],xmm0                             ; [rip+00001EE1]
	movsd	[00000001400035E8],xmm1                              ; [rip+00001EE9]

l00000001400016FF:
	mov	rbx,[rsp+50h]
	add	rsp,40h
	pop	rbp
	ret

l000000014000170A:
	mov	ecx,5h
	call	fn0000000140001974
	int	3h
	int	3h
	int	3h
	int	3h

;; fn0000000140001718: 0000000140001718
;;   Called from:
;;     0000000140001376 (in fn00000001400012BC)
;;     00000001400013A6 (in fn00000001400012BC)
;;     0000000140001717 (in fn000000014000164C)
fn0000000140001718 proc
	sub	rsp,18h
	mov	r8,rcx
	mov	eax,5A4Dh
	cmp	[0000000140000000],ax                                  ; [rip-0000172B]
	jnz	1400017A6h

l000000014000172D:
	movsxd	rax,[000000014000003C]                              ; [rip-000016F8]
	lea	rdx,[0000000140000000]                                 ; [rip-0000173B]
	lea	rcx,[rax+rdx]
	cmp	dword ptr [rcx],4550h
	jnz	1400017A6h

l0000000140001747:
	mov	eax,20Bh
	cmp	[rcx+18h],ax
	jnz	1400017A6h

l0000000140001752:
	sub	r8,rdx
	movzx	eax,word ptr [rcx+14h]
	lea	rdx,[rcx+18h]
	add	rdx,rax
	movzx	eax,word ptr [rcx+6h]
	lea	rcx,[rax+rax*4]
	lea	r9,[rdx+rcx*8]

l000000014000176C:
	mov	[rsp],rdx
	cmp	rdx,r9
	jz	14000178Dh

l0000000140001775:
	mov	ecx,[rdx+0Ch]
	cmp	r8,rcx
	jc	140001787h

l000000014000177D:
	mov	eax,[rdx+8h]
	add	eax,ecx
	cmp	r8,rax
	jc	14000178Fh

l0000000140001787:
	add	rdx,28h
	jmp	14000176Ch

l000000014000178D:
	xor	edx,edx

l000000014000178F:
	test	rdx,rdx
	jnz	140001798h

l0000000140001794:
	xor	al,al
	jmp	1400017ACh

l0000000140001798:
	cmp	dword ptr [rdx+24h],0h
	jge	1400017A2h

l000000014000179E:
	xor	al,al
	jmp	1400017ACh

l00000001400017A2:
	mov	al,1h
	jmp	1400017ACh

l00000001400017A6:
	xor	al,al
	jmp	1400017ACh
00000001400017AA                               32 C0                       2.    

l00000001400017AC:
	add	rsp,18h
	ret
00000001400017B1    CC CC CC                                      ...            

;; fn00000001400017B4: 00000001400017B4
;;   Called from:
;;     0000000140001360 (in fn00000001400012BC)
fn00000001400017B4 proc
	push	rbx
	sub	rsp,20h
	mov	bl,cl
	call	fn0000000140001DC4
	xor	edx,edx
	test	eax,eax
	jz	1400017D2h

l00000001400017C7:
	test	bl,bl
	jnz	1400017D2h

l00000001400017CB:
	xchg	[00000001400035B8],rdx                                ; [rip+00001DE6]

l00000001400017D2:
	add	rsp,20h
	pop	rbx
	ret

;; fn00000001400017D8: 00000001400017D8
;;   Called from:
;;     00000001400013F9 (in fn00000001400012BC)
fn00000001400017D8 proc
	push	rbx
	sub	rsp,20h
	cmp	[00000001400035F0],0h                                  ; [rip+00001E0B]
	mov	bl,cl
	jz	1400017EDh

l00000001400017E9:
	test	dl,dl
	jnz	1400017FBh

l00000001400017ED:
	mov	cl,bl
	call	fn0000000140001938
	mov	cl,bl
	call	fn0000000140001938

l00000001400017FB:
	mov	al,1h
	add	rsp,20h
	pop	rbx
	ret
0000000140001803          CC                                        .            

;; fn0000000140001804: 0000000140001804
;;   Called from:
;;     0000000140001858 (in fn0000000140001854)
fn0000000140001804 proc
	push	rbx
	sub	rsp,20h
	mov	rdx,[0000000140003000]                                 ; [rip+000017EF]
	mov	rbx,rcx
	mov	ecx,edx
	xor	rdx,[00000001400035C0]                                 ; [rip+00001DA3]
	and	ecx,3Fh
	ror	rdx,cl
	cmp	rdx,0FFh
	jnz	140001833h

l0000000140001829:
	mov	rcx,rbx
	call	140001E6Ah
	jmp	140001842h

l0000000140001833:
	mov	rdx,rbx
	lea	rcx,[00000001400035C0]                                 ; [rip+00001D83]
	call	140001E64h

l0000000140001842:
	xor	ecx,ecx
	test	eax,eax
	cmovz	rcx,rbx

l000000014000184A:
	mov	rax,rcx
	add	rsp,20h
	pop	rbx
	ret
0000000140001853          CC                                        .            

;; fn0000000140001854: 0000000140001854
;;   Called from:
;;     0000000140001219 (in fn00000001400011D4)
fn0000000140001854 proc
	sub	rsp,28h
	call	fn0000000140001804
	neg	rax
	sbb	eax,eax
	neg	eax
	dec	eax
	add	rsp,28h
	ret
000000014000186B                                  CC                        .    

;; fn000000014000186C: 000000014000186C
;;   Called from:
;;     0000000140001438 (in Win32CrtStartup)
fn000000014000186C proc
	mov	[rsp+20h],rbx
	push	rbp
	mov	rbp,rsp
	sub	rsp,20h
	and	qword ptr [rbp+18h],0h
	mov	rbx,2B992DDFA232h
	mov	rax,[0000000140003000]                                 ; [rip+00001771]
	cmp	rax,rbx
	jnz	140001903h

l0000000140001894:
	lea	rcx,[rbp+18h]
	call	[0000000140002030]                                    ; [rip+00000792]
	mov	rax,[rbp+18h]
	mov	[rbp+10h],rax
	call	[0000000140002038]                                    ; [rip+0000078C]
	mov	eax,eax
	xor	[rbp+10h],rax
	call	[0000000140002040]                                    ; [rip+00000788]
	mov	eax,eax
	lea	rcx,[rbp+20h]
	xor	[rbp+10h],rax
	call	[0000000140002048]                                    ; [rip+00000780]
	mov	eax,[rbp+20h]
	lea	rcx,[rbp+10h]
	shl	rax,20h
	xor	rax,[rbp+20h]
	xor	rax,[rbp+10h]
	xor	rax,rcx
	mov	rcx,0FFFFFFFFFFFFh
	and	rax,rcx
	mov	rcx,2B992DDFA233h
	cmp	rax,rbx
	cmovz	rax,rcx

l00000001400018FC:
	mov	[0000000140003000],rax                                 ; [rip+000016FD]

l0000000140001903:
	mov	rbx,[rsp+48h]
	not	rax
	mov	[0000000140003008],rax                                 ; [rip+000016F6]
	add	rsp,20h
	pop	rbp
	ret

;; fn0000000140001918: 0000000140001918
;;   Called from:
;;     000000014000121E (in fn00000001400011D4)
fn0000000140001918 proc
	mov	eax,1h
	ret
000000014000191E                                           CC CC               ..

;; fn0000000140001920: 0000000140001920
;;   Called from:
;;     00000001400011E4 (in fn00000001400011D4)
fn0000000140001920 proc
	mov	eax,4000h
	ret
0000000140001926                   CC CC                               ..        

;; fn0000000140001928: 0000000140001928
;;   Called from:
;;     000000014000122E (in fn00000001400011D4)
fn0000000140001928 proc
	lea	rcx,[0000000140003600]                                 ; [rip+00001CD1]
	jmp	[0000000140002028]                                     ; [rip+000006F2]
0000000140001936                   CC CC                               ..        

;; fn0000000140001938: 0000000140001938
;;   Called from:
;;     000000014000125E (in fn00000001400011D4)
;;     0000000140001622 (in fn0000000140001600)
;;     000000014000162F (in fn0000000140001600)
;;     000000014000163A (in fn0000000140001600)
;;     00000001400017EF (in fn00000001400017D8)
;;     00000001400017F6 (in fn00000001400017D8)
fn0000000140001938 proc
	mov	al,1h
	ret
000000014000193B                                  CC                        .    

;; fn000000014000193C: 000000014000193C
;;   Called from:
;;     0000000140001294 (in fn0000000140001290)
fn000000014000193C proc
	sub	rsp,28h
	call	fn0000000140001130
	or	qword ptr [rax],4h
	call	fn00000001400010C0
	or	qword ptr [rax],2h
	add	rsp,28h
	ret
0000000140001957                      CC                                .        

;; fn0000000140001958: 0000000140001958
;;   Called from:
;;     0000000140001233 (in fn00000001400011D4)
fn0000000140001958 proc
	xor	eax,eax
	cmp	[0000000140003014],eax                                 ; [rip+000016B4]
	setz	al
	ret

;; fn0000000140001964: 0000000140001964
;;   Called from:
;;     0000000140001365 (in fn00000001400012BC)
fn0000000140001964 proc
	lea	rax,[0000000140003638]                                 ; [rip+00001CCD]
	ret

;; fn000000014000196C: 000000014000196C
;;   Called from:
;;     0000000140001395 (in fn00000001400012BC)
fn000000014000196C proc
	lea	rax,[0000000140003630]                                 ; [rip+00001CBD]
	ret

;; fn0000000140001974: 0000000140001974
;;   Called from:
;;     000000014000127E (in fn00000001400011D4)
;;     0000000140001289 (in fn00000001400011D4)
;;     00000001400012DE (in fn00000001400012BC)
;;     0000000140001303 (in fn00000001400012BC)
;;     000000014000170F (in fn000000014000164C)
fn0000000140001974 proc
	mov	[rsp+8h],rbx
	push	rbp
	lea	rbp,[rsp-4C0h]
	sub	rsp,+5C0h
	mov	ebx,ecx
	mov	ecx,17h
	call	140001E76h
	test	eax,eax
	jz	14000199Dh

l0000000140001999:
	mov	ecx,ebx
	int	29h

l000000014000199D:
	and	[0000000140003610],0h                                  ; [rip+00001C6C]
	lea	rcx,[rbp-10h]
	xor	edx,edx
	mov	r8d,4D0h
	call	140001DE6h
	lea	rcx,[rbp-10h]
	call	[0000000140002070]                                    ; [rip+000006B1]
	mov	rbx,[rbp+0E8h]
	lea	rdx,[rbp+4D8h]
	mov	rcx,rbx
	xor	r8d,r8d
	call	[0000000140002000]                                    ; [rip+00000627]
	test	rax,rax
	jz	140001A1Ah

l00000001400019DE:
	and	qword ptr [rsp+38h],0h
	lea	rcx,[rbp+4E0h]
	mov	rdx,[rbp+4D8h]
	mov	r9,rax
	mov	[rsp+30h],rcx
	mov	r8,rbx
	lea	rcx,[rbp+4E8h]
	mov	[rsp+28h],rcx
	lea	rcx,[rbp-10h]
	mov	[rsp+20h],rcx
	xor	ecx,ecx
	call	[0000000140002008]                                    ; [rip+000005EE]

l0000000140001A1A:
	mov	rax,[rbp+4C8h]
	lea	rcx,[rsp+50h]
	mov	[rbp+0E8h],rax
	xor	edx,edx
	lea	rax,[rbp+4C8h]
	mov	r8d,98h
	add	rax,8h
	mov	[rbp+88h],rax
	call	140001DE6h
	mov	rax,[rbp+4C8h]
	mov	[rsp+60h],rax
	mov	dword ptr [rsp+50h],40000015h
	mov	dword ptr [rsp+54h],1h
	call	[0000000140002020]                                    ; [rip+000005B2]
	cmp	eax,1h
	lea	rax,[rsp+50h]
	mov	[rsp+40h],rax
	lea	rax,[rbp-10h]
	setz	bl
	mov	[rsp+48h],rax
	xor	ecx,ecx
	call	[0000000140002068]                                    ; [rip+000005D9]
	lea	rcx,[rsp+40h]
	call	[0000000140002010]                                    ; [rip+00000576]
	test	eax,eax
	jnz	140001AA8h

l0000000140001A9E:
	neg	bl
	sbb	eax,eax
	and	[0000000140003610],eax                                 ; [rip+00001B68]

l0000000140001AA8:
	mov	rbx,[rsp+5D0h]
	add	rsp,+5C0h
	pop	rbp
	ret
0000000140001AB9                            CC CC CC                      ...    

;; fn0000000140001ABC: 0000000140001ABC
;;   Called from:
;;     00000001400011F8 (in fn00000001400011D4)
;;     0000000140001252 (in fn00000001400011D4)
;;     000000014000126C (in fn00000001400011D4)
;;     00000001400012A9 (in fn00000001400012A0)
fn0000000140001ABC proc
	xor	eax,eax
	ret
0000000140001ABF                                              CC                .

;; fn0000000140001AC0: 0000000140001AC0
;;   Called from:
;;     00000001400013DB (in fn00000001400012BC)
fn0000000140001AC0 proc
	sub	rsp,28h
	xor	ecx,ecx
	call	[0000000140002018]                                    ; [rip+0000054C]
	mov	rcx,rax
	test	rax,rax
	jnz	140001AD8h

l0000000140001AD4:
	xor	al,al
	jmp	140001B0Fh

l0000000140001AD8:
	mov	eax,5A4Dh
	cmp	[rcx],ax
	jnz	140001AD4h

l0000000140001AE2:
	movsxd	rax,dword ptr [rcx+3Ch]
	add	rax,rcx
	cmp	dword ptr [rax],4550h
	jnz	140001AD4h

l0000000140001AF1:
	mov	ecx,20Bh
	cmp	[rax+18h],cx
	jnz	140001AD4h

l0000000140001AFC:
	cmp	dword ptr [rax+84h],0Eh
	jbe	140001AD4h

l0000000140001B05:
	cmp	dword ptr [rax+0F8h],0h
	setnz	al

l0000000140001B0F:
	add	rsp,28h
	ret

;; fn0000000140001B14: 0000000140001B14
;;   Called from:
;;     00000001400012A4 (in fn00000001400012A0)
fn0000000140001B14 proc
	lea	rcx,[fn0000000140001B24]                               ; [rip+00000009]
	jmp	[0000000140002068]                                     ; [rip+00000546]
0000000140001B22       CC CC                                       ..            

;; fn0000000140001B24: 0000000140001B24
fn0000000140001B24 proc
	sub	rsp,28h
	mov	rax,[rcx]
	cmp	dword ptr [rax],0E06D7363h
	jnz	140001B4Fh

l0000000140001B33:
	cmp	dword ptr [rax+18h],4h
	jnz	140001B4Fh

l0000000140001B39:
	mov	ecx,[rax+20h]
	lea	eax,[rcx-19930520h]
	cmp	eax,2h
	jbe	140001B56h

l0000000140001B47:
	cmp	ecx,1994000h
	jz	140001B56h

l0000000140001B4F:
	xor	eax,eax
	add	rsp,28h
	ret

l0000000140001B56:
	call	140001E70h
	int	3h

;; fn0000000140001B5C: 0000000140001B5C
;;   Called from:
;;     000000014000120D (in fn00000001400011D4)
;;     0000000140001B5B (in fn0000000140001B24)
fn0000000140001B5C proc
	mov	[rsp+8h],rbx
	mov	[rsp+10h],rsi
	push	rdi
	sub	rsp,20h
	lea	rbx,[0000000140002680]                                 ; [rip+00000B0E]
	lea	rsi,[0000000140002680]                                 ; [rip+00000B07]
	jmp	140001B91h

l0000000140001B7B:
	mov	rdi,[rbx]
	test	rdi,rdi
	jz	140001B8Dh

l0000000140001B83:
	mov	rcx,rdi
	call	fn0000000140001BF4
	call	rdi

l0000000140001B8D:
	add	rbx,8h

l0000000140001B91:
	cmp	rbx,rsi
	jc	140001B7Bh

l0000000140001B96:
	mov	rbx,[rsp+30h]
	mov	rsi,[rsp+38h]
	add	rsp,20h
	pop	rdi
	ret
0000000140001BA6                   CC CC                               ..        

;; fn0000000140001BA8: 0000000140001BA8
fn0000000140001BA8 proc
	mov	[rsp+8h],rbx
	mov	[rsp+10h],rsi
	push	rdi
	sub	rsp,20h
	lea	rbx,[0000000140002690]                                 ; [rip+00000AD2]
	lea	rsi,[0000000140002690]                                 ; [rip+00000ACB]
	jmp	140001BDDh

l0000000140001BC7:
	mov	rdi,[rbx]
	test	rdi,rdi
	jz	140001BD9h

l0000000140001BCF:
	mov	rcx,rdi
	call	fn0000000140001BF4
	call	rdi

l0000000140001BD9:
	add	rbx,8h

l0000000140001BDD:
	cmp	rbx,rsi
	jc	140001BC7h

l0000000140001BE2:
	mov	rbx,[rsp+30h]
	mov	rsi,[rsp+38h]
	add	rsp,20h
	pop	rdi
	ret
0000000140001BF2       CC CC                                       ..            

;; fn0000000140001BF4: 0000000140001BF4
;;   Called from:
;;     0000000140001385 (in fn00000001400012BC)
;;     0000000140001B86 (in fn0000000140001B5C)
;;     0000000140001BD2 (in fn0000000140001BA8)
fn0000000140001BF4 proc
	jmp	[0000000140002190]                                     ; [rip+00000595]
0000000140001BFB                                  CC                        .    

;; fn0000000140001BFC: 0000000140001BFC
;;   Called from:
;;     000000014000161D (in fn0000000140001600)
fn0000000140001BFC proc
	mov	[rsp+10h],rbx
	mov	[rsp+18h],rdi
	push	rbp
	mov	rbp,rsp
	sub	rsp,20h
	and	dword ptr [rbp-18h],0h
	xor	ecx,ecx
	xor	eax,eax
	mov	[000000014000301C],2h                                  ; [rip+000013FC]
	cpuid
	mov	r8d,ecx
	mov	[0000000140003018],1h                                  ; [rip+000013E9]
	xor	ecx,444D4163h
	mov	r9d,edx
	mov	r10d,edx
	xor	r9d,69746E65h
	xor	r10d,49656E69h
	xor	r8d,6C65746Eh
	or	r10d,r8d
	mov	r11d,ebx
	mov	r8d,[0000000140003614]                                 ; [rip+000019B7]
	xor	r11d,68747541h
	or	r11d,r9d
	mov	edx,ebx
	or	r11d,ecx
	xor	edx,756E6547h
	xor	ecx,ecx
	mov	edi,eax
	or	r10d,edx
	mov	eax,1h
	cpuid
	mov	[rbp-10h],eax
	mov	r9d,ecx
	mov	[rbp-8h],r9d
	mov	ecx,eax
	mov	[rbp-0Ch],ebx
	mov	[rbp-4h],edx
	test	r10d,r10d
	jnz	140001CE9h

l0000000140001C97:
	or	[0000000140003020],0FFh                                 ; [rip+00001381]
	or	r8d,4h
	and	eax,0FFF3FF0h
	mov	[0000000140003614],r8d                                 ; [rip+00001965]
	cmp	eax,106C0h
	jz	140001CDEh

l0000000140001CB6:
	cmp	eax,20660h
	jz	140001CDEh

l0000000140001CBD:
	cmp	eax,20670h
	jz	140001CDEh

l0000000140001CC4:
	add	eax,0FFFCF9B0h
	cmp	eax,20h
	ja	140001CE9h

l0000000140001CCE:
	mov	rbx,100010001h
	bt	rbx,rax
	jnc	140001CE9h

l0000000140001CDE:
	or	r8d,1h
	mov	[0000000140003614],r8d                                 ; [rip+0000192B]

l0000000140001CE9:
	test	r11d,r11d
	jnz	140001D07h

l0000000140001CEE:
	and	ecx,0FF00F00h
	cmp	ecx,600F00h
	jc	140001D07h

l0000000140001CFC:
	or	r8d,4h
	mov	[0000000140003614],r8d                                 ; [rip+0000190D]

l0000000140001D07:
	mov	eax,7h
	mov	[rbp-20h],edx
	mov	[rbp-1Ch],r9d
	cmp	edi,eax
	jl	140001D3Bh

l0000000140001D17:
	xor	ecx,ecx
	cpuid
	mov	[rbp-10h],eax
	mov	[rbp-0Ch],ebx
	mov	[rbp-8h],ecx
	mov	[rbp-4h],edx
	mov	[rbp-18h],ebx
	bt	ebx,9h
	jnc	140001D3Bh

l0000000140001D30:
	or	r8d,2h
	mov	[0000000140003614],r8d                                 ; [rip+000018D9]

l0000000140001D3B:
	bt	r9d,14h
	jnc	140001DB0h

l0000000140001D42:
	mov	[0000000140003018],2h                                  ; [rip+000012CC]
	mov	[000000014000301C],6h                                  ; [rip+000012C6]
	bt	r9d,1Bh
	jnc	140001DB0h

l0000000140001D5D:
	bt	r9d,1Ch
	jnc	140001DB0h

l0000000140001D64:
	xor	ecx,ecx
	xgetbv
	shl	rdx,20h
	or	rdx,rax
	mov	[rbp+10h],rdx
	mov	rax,[rbp+10h]
	and	al,6h
	cmp	al,6h
	jnz	140001DB0h

l0000000140001D7E:
	mov	eax,[000000014000301C]                                 ; [rip+00001298]
	or	eax,8h
	mov	[0000000140003018],3h                                  ; [rip+00001287]
	test	byte ptr [rbp-18h],20h
	mov	[000000014000301C],eax                                 ; [rip+00001281]
	jz	140001DB0h

l0000000140001D9D:
	or	eax,20h
	mov	[0000000140003018],5h                                  ; [rip+0000126E]
	mov	[000000014000301C],eax                                 ; [rip+0000126C]

l0000000140001DB0:
	mov	rbx,[rsp+38h]
	xor	eax,eax
	mov	rdi,[rsp+40h]
	add	rsp,20h
	pop	rbp
	ret
0000000140001DC2       CC CC                                       ..            

;; fn0000000140001DC4: 0000000140001DC4
;;   Called from:
;;     00000001400015C8 (in fn00000001400015C4)
;;     0000000140001664 (in fn000000014000164C)
;;     00000001400017BC (in fn00000001400017B4)
fn0000000140001DC4 proc
	xor	eax,eax
	cmp	[0000000140003030],eax                                 ; [rip+00001264]
	setnz	al
	ret

;; fn0000000140001DD0: 0000000140001DD0
;;   Called from:
;;     0000000140001248 (in fn00000001400011D4)
;;     000000014000124D (in fn00000001400011D4)
fn0000000140001DD0 proc
	ret	0h
0000000140001DD3          CC CC CC CC CC CC CC CC CC CC CC CC CC    .............
0000000140001DE0 FF 25 A2 02 00 00 FF 25 94 02 00 00 FF 25 FE 02 .%.....%.....%..
0000000140001DF0 00 00 FF 25 F0 02 00 00 FF 25 BA 02 00 00 FF 25 ...%.....%.....%
0000000140001E00 44 03 00 00 FF 25 36 03 00 00 FF 25 28 03 00 00 D....%6....%(...
0000000140001E10 FF 25 1A 03 00 00 FF 25 0C 03 00 00 FF 25 FE 02 .%.....%.....%..
0000000140001E20 00 00 FF 25 F0 02 00 00 FF 25 52 03 00 00 FF 25 ...%.....%R....%
0000000140001E30 D4 02 00 00 FF 25 C6 02 00 00 FF 25 88 02 00 00 .....%.....%....
0000000140001E40 FF 25 CA 02 00 00 FF 25 AC 02 00 00 FF 25 56 02 .%.....%.....%V.
0000000140001E50 00 00 FF 25 40 02 00 00 FF 25 02 03 00 00 FF 25 ...%@....%.....%
0000000140001E60 EC 02 00 00 FF 25 66 02 00 00 FF 25 68 02 00 00 .....%f....%h...
0000000140001E70 FF 25 6A 02 00 00 FF 25 D4 01 00 00             .%j....%....    

;; fn0000000140001E7C: 0000000140001E7C
fn0000000140001E7C proc
	sub	rsp,28h
	mov	r8,[r9+38h]
	mov	rcx,rdx
	mov	rdx,r9
	call	fn0000000140001E9C
	mov	eax,1h
	add	rsp,28h
	ret
0000000140001E99                            CC CC CC                      ...    

;; fn0000000140001E9C: 0000000140001E9C
;;   Called from:
;;     0000000140001E8A (in fn0000000140001E7C)
fn0000000140001E9C proc
	push	rbx
	mov	r11d,[r8]
	mov	rbx,rdx
	and	r11d,0F8h
	mov	r9,rcx
	test	byte ptr [r8],4h
	mov	r10,rcx
	jz	140001EC7h

l0000000140001EB4:
	mov	eax,[r8+8h]
	movsxd	r10,dword ptr [r8+4h]
	neg	eax
	add	r10,rcx
	movsxd	rcx,eax
	and	r10,rcx

l0000000140001EC7:
	movsxd	rax,r11d
	mov	rdx,[rax+r10]
	mov	rax,[rbx+10h]
	mov	ecx,[rax+8h]
	add	rcx,[rbx+8h]
	test	byte ptr [rcx+3h],0Fh
	jz	140001EE9h

l0000000140001EDF:
	movzx	eax,byte ptr [rcx+3h]
	and	eax,0F0h
	add	r9,rax

l0000000140001EE9:
	xor	r9,rdx
	mov	rcx,r9
	pop	rbx
	jmp	fn00000001400011B0
0000000140001EF5                CC CC CC CC CC CC CC CC CC CC CC      ...........
0000000140001F00 CC CC CC CC CC CC 66 66 0F 1F 84 00 00 00 00 00 ......ff........

;; fn0000000140001F10: 0000000140001F10
fn0000000140001F10 proc
	jmp	rax

;; fn0000000140001F12: 0000000140001F12
fn0000000140001F12 proc
	push	rbp
	sub	rsp,20h
	mov	rbp,rdx
	mov	rax,[rcx]
	mov	rdx,rcx
	mov	ecx,[rax]
	call	140001DECh
	nop
	add	rsp,20h
	pop	rbp
	ret
0000000140001F2F                                              CC                .

;; fn0000000140001F30: 0000000140001F30
fn0000000140001F30 proc
	push	rbp
	mov	rbp,rdx
	mov	rax,[rcx]
	xor	ecx,ecx
	cmp	dword ptr [rax],0C0000005h
	setz	cl
	mov	eax,ecx
	pop	rbp
	ret
0000000140001F47                      CC                                .        
