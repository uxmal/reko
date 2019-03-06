;;; Segment .text (0000000140001000)

;; fn0000000140001000: 0000000140001000
;;   Called from:
;;     00000001400013D4 (in fn00000001400012BC)
fn0000000140001000 proc
	sub	rsp,+000000C8
	mov	rax,[0000000140003000]                                 ; [rip+00001FF2]
	xor	rax,rsp
	mov	[rsp+000000B8],rax
	movaps	xmm0,[0000000140002260]                             ; [rip+00001240]
	lea	r9,[0000000140002240]                                  ; [rip+00001219]
	mov	dword ptr [rsp+38],00000057
	lea	r8,[0000000140002250]                                  ; [rip+0000121A]
	mov	dword ptr [rsp+30],00000063
	lea	rcx,[0000000140002210]                                 ; [rip+000011CB]
	mov	edx,00000003
	movups	[rsp+20],xmm0
	call	0000000140001140
	lea	rax,[rsp+44]
	mov	[rsp+38],rax
	lea	r9,[rsp+78]
	lea	rax,[rsp+40]
	mov	[rsp+30],rax
	lea	r8,[rsp+58]
	lea	rax,[rsp+50]
	mov	[rsp+28],rax
	lea	rdx,[rsp+4C]
	lea	rax,[rsp+48]
	lea	rcx,[0000000140002228]                                 ; [rip+0000119B]
	mov	[rsp+20],rax
	call	00000001400010D0
	xor	eax,eax
	mov	rcx,[rsp+000000B8]
	xor	rcx,rsp
	call	00000001400011B0
	add	rsp,+000000C8
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
	mov	[rsp+08],rcx
	mov	[rsp+10],rdx
	mov	[rsp+18],r8
	mov	[rsp+20],r9
	push	rbx
	push	rsi
	push	rdi
	sub	rsp,30
	mov	rdi,rcx
	lea	rsi,[rsp+58]
	xor	ecx,ecx
	call	[0000000140002178]                                    ; [rip+0000107D]
	mov	rbx,rax
	call	00000001400010C0
	xor	r9d,r9d
	mov	[rsp+20],rsi
	mov	r8,rdi
	mov	rdx,rbx
	mov	rcx,[rax]
	call	[0000000140002168]                                    ; [rip+0000104E]
	add	rsp,30
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
	mov	[rsp+08],rcx
	mov	[rsp+10],rdx
	mov	[rsp+18],r8
	mov	[rsp+20],r9
	push	rbx
	push	rsi
	push	rdi
	sub	rsp,30
	mov	rdi,rcx
	lea	rsi,[rsp+58]
	mov	ecx,00000001
	call	[0000000140002178]                                    ; [rip+0000100A]
	mov	rbx,rax
	call	0000000140001130
	xor	r9d,r9d
	mov	[rsp+20],rsi
	mov	r8,rdi
	mov	rdx,rbx
	mov	rcx,[rax]
	call	[0000000140002170]                                    ; [rip+00000FE3]
	add	rsp,30
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
	repne jnz	00000001400011CC

l00000001400011BA:
	rol	rcx,10
	test	cx,FFFF
	repne jnz	00000001400011C8

l00000001400011C6:
	repne ret

l00000001400011C8:
	ror	rcx,10

l00000001400011CC:
	jmp	000000014000147C
00000001400011D1    CC CC CC                                      ...           

;; fn00000001400011D4: 00000001400011D4
fn00000001400011D4 proc
	push	rbx
	sub	rsp,20
	mov	ecx,00000001
	call	0000000140001DF2
	call	0000000140001920
	mov	ecx,eax
	call	0000000140001E28
	call	0000000140001E58
	mov	rbx,rax
	call	0000000140001ABC
	mov	ecx,00000001
	mov	[rbx],eax
	call	000000014000164C
	test	al,al
	jz	0000000140001279

l000000014000120D:
	call	0000000140001B5C
	lea	rcx,[0000000140001BA8]                                 ; [rip+0000098F]
	call	0000000140001854
	call	0000000140001918
	mov	ecx,eax
	call	0000000140001DFE
	test	eax,eax
	jnz	0000000140001284

l000000014000122E:
	call	0000000140001928
	call	0000000140001958
	test	eax,eax
	jz	0000000140001248

l000000014000123C:
	lea	rcx,[0000000140001ABC]                                 ; [rip+00000879]
	call	0000000140001DF8

l0000000140001248:
	call	0000000140001DD0
	call	0000000140001DD0
	call	0000000140001ABC
	mov	ecx,eax
	call	0000000140001E4C
	call	0000000140001938
	test	al,al
	jz	000000014000126C

l0000000140001267:
	call	0000000140001E04

l000000014000126C:
	call	0000000140001ABC
	xor	eax,eax
	add	rsp,20
	pop	rbx
	ret

l0000000140001279:
	mov	ecx,00000007
	call	0000000140001974
	int	03

l0000000140001284:
	mov	ecx,00000007
	call	0000000140001974
	int	03
	int	03

;; fn0000000140001290: 0000000140001290
;;   Called from:
;;     000000014000128F (in fn00000001400011D4)
fn0000000140001290 proc
	sub	rsp,28
	call	000000014000193C
	xor	eax,eax
	add	rsp,28
	ret

;; fn00000001400012A0: 00000001400012A0
fn00000001400012A0 proc
	sub	rsp,28
	call	0000000140001B14
	call	0000000140001ABC
	mov	ecx,eax
	add	rsp,28
	jmp	0000000140001E52
00000001400012B9                            CC CC CC                      ...   

;; fn00000001400012BC: 00000001400012BC
;;   Called from:
;;     0000000140001441 (in Win32CrtStartup)
fn00000001400012BC proc
	mov	[rsp+08],rbx
	mov	[rsp+10],rsi
	push	rdi
	sub	rsp,30
	mov	ecx,00000001
	call	0000000140001600
	test	al,al
	jnz	00000001400012E4

l00000001400012D9:
	mov	ecx,00000007
	call	0000000140001974
	int	03

l00000001400012E4:
	xor	sil,sil
	mov	[rsp+20],sil
	call	00000001400015C4
	mov	bl,al
	mov	ecx,[00000001400035B0]                                 ; [rip+000022B7]
	cmp	ecx,01
	jnz	0000000140001308

l00000001400012FE:
	mov	ecx,00000007
	call	0000000140001974

l0000000140001308:
	test	ecx,ecx
	jnz	0000000140001356

l000000014000130C:
	mov	[00000001400035B0],00000001                            ; [rip+0000229A]
	lea	rdx,[00000001400021D0]                                 ; [rip+00000EB3]
	lea	rcx,[00000001400021B8]                                 ; [rip+00000E94]
	call	0000000140001E16
	test	eax,eax
	jz	0000000140001337

l000000014000132D:
	mov	eax,000000FF
	jmp	0000000140001423

l0000000140001337:
	lea	rdx,[00000001400021B0]                                 ; [rip+00000E72]
	lea	rcx,[00000001400021A0]                                 ; [rip+00000E5B]
	call	0000000140001E10
	mov	[00000001400035B0],00000002                            ; [rip+0000225C]
	jmp	000000014000135E

l0000000140001356:
	mov	sil,01
	mov	[rsp+20],sil

l000000014000135E:
	mov	cl,bl
	call	00000001400017B4
	call	0000000140001964
	mov	rbx,rax
	cmp	qword ptr [rax],00
	jz	0000000140001395

l0000000140001373:
	mov	rcx,rax
	call	0000000140001718
	test	al,al
	jz	0000000140001395

l000000014000137F:
	mov	rbx,[rbx]
	mov	rcx,rbx
	call	0000000140001BF4
	xor	r8d,r8d
	lea	edx,[r8+02]
	xor	ecx,ecx
	call	rbx

l0000000140001395:
	call	000000014000196C
	mov	rbx,rax
	cmp	qword ptr [rax],00
	jz	00000001400013B7

l00000001400013A3:
	mov	rcx,rax
	call	0000000140001718
	test	al,al
	jz	00000001400013B7

l00000001400013AF:
	mov	rcx,[rbx]
	call	0000000140001E46

l00000001400013B7:
	call	0000000140001E34
	mov	rdi,rax
	call	0000000140001E2E
	mov	rbx,rax
	call	0000000140001E0A
	mov	r8,rax
	mov	rdx,[rdi]
	mov	ecx,[rbx]
	call	0000000140001000
	mov	ebx,eax
	call	0000000140001AC0
	test	al,al
	jnz	00000001400013EB

l00000001400013E4:
	mov	ecx,ebx
	call	0000000140001E1C

l00000001400013EB:
	test	sil,sil
	jnz	00000001400013F5

l00000001400013F0:
	call	0000000140001E3A

l00000001400013F5:
	xor	edx,edx
	mov	cl,01
	call	00000001400017D8
	mov	eax,ebx
	jmp	0000000140001423
0000000140001402       8B D8 E8 B7 06 00 00 84 C0 75 08 8B CB E8   .........u....
0000000140001410 0E 0A 00 00 CC 80 7C 24 20 00 75 05 E8 1F 0A 00 ......|$ .u.....
0000000140001420 00 8B C3                                        ...            

l0000000140001423:
	mov	rbx,[rsp+40]
	mov	rsi,[rsp+48]
	add	rsp,30
	pop	rdi
	ret
0000000140001433          CC                                        .           

;; Win32CrtStartup: 0000000140001434
Win32CrtStartup proc
	sub	rsp,28
	call	000000014000186C
	add	rsp,28
	jmp	00000001400012BC
0000000140001446                   CC CC                               ..       

;; fn0000000140001448: 0000000140001448
;;   Called from:
;;     0000000140001543 (in fn000000014000147C)
fn0000000140001448 proc
	push	rbx
	sub	rsp,20
	mov	rbx,rcx
	xor	ecx,ecx
	call	[0000000140002068]                                    ; [rip+00000C0F]
	mov	rcx,rbx
	call	[0000000140002010]                                    ; [rip+00000BAE]
	call	[0000000140002060]                                    ; [rip+00000BF8]
	mov	rcx,rax
	mov	edx,C0000409
	add	rsp,20
	pop	rbx
	jmp	[0000000140002058]                                     ; [rip+00000BDC]

;; fn000000014000147C: 000000014000147C
;;   Called from:
;;     00000001400011CC (in fn00000001400011B0)
fn000000014000147C proc
	mov	[rsp+08],rcx
	sub	rsp,38
	mov	ecx,00000017
	call	0000000140001E76
	test	eax,eax
	jz	000000014000149A

l0000000140001493:
	mov	ecx,00000002
	int	29

l000000014000149A:
	lea	rcx,[00000001400030E0]                                 ; [rip+00001C3F]
	call	0000000140001550
	mov	rax,[rsp+38]
	mov	[00000001400031D8],rax                                 ; [rip+00001D26]
	lea	rax,[rsp+38]
	add	rax,08
	mov	[0000000140003178],rax                                 ; [rip+00001CB6]
	mov	rax,[00000001400031D8]                                 ; [rip+00001D0F]
	mov	[0000000140003050],rax                                 ; [rip+00001B80]
	mov	rax,[rsp+40]
	mov	[0000000140003160],rax                                 ; [rip+00001C84]
	mov	[0000000140003040],C0000409                            ; [rip+00001B5A]
	mov	[0000000140003044],00000001                            ; [rip+00001B54]
	mov	[0000000140003058],00000001                            ; [rip+00001B5E]
	mov	eax,00000008
	imul	rax,rax,00
	lea	rcx,[0000000140003060]                                 ; [rip+00001B56]
	mov	qword ptr [rcx+rax],+00000002
	mov	eax,00000008
	imul	rax,rax,00
	mov	rcx,[0000000140003000]                                 ; [rip+00001ADE]
	mov	[rsp+rax+20],rcx
	mov	eax,00000008
	imul	rax,rax,01
	mov	rcx,[0000000140003008]                                 ; [rip+00001AD1]
	mov	[rsp+rax+20],rcx
	lea	rcx,[0000000140002200]                                 ; [rip+00000CBD]
	call	0000000140001448
	add	rsp,38
	ret
000000014000154D                                        CC CC CC              ...

;; fn0000000140001550: 0000000140001550
;;   Called from:
;;     00000001400014A1 (in fn000000014000147C)
fn0000000140001550 proc
	push	rbx
	push	rsi
	push	rdi
	sub	rsp,40
	mov	rbx,rcx
	call	[0000000140002070]                                    ; [rip+00000B0F]
	mov	rsi,[rbx+000000F8]
	xor	edi,edi

l000000014000156A:
	xor	r8d,r8d
	lea	rdx,[rsp+60]
	mov	rcx,rsi
	call	[0000000140002000]                                    ; [rip+00000A85]
	test	rax,rax
	jz	00000001400015B9

l0000000140001580:
	and	qword ptr [rsp+38],00
	lea	rcx,[rsp+68]
	mov	rdx,[rsp+60]
	mov	r9,rax
	mov	[rsp+30],rcx
	mov	r8,rsi
	lea	rcx,[rsp+70]
	mov	[rsp+28],rcx
	xor	ecx,ecx
	mov	[rsp+20],rbx
	call	[0000000140002008]                                    ; [rip+00000A56]
	inc	edi
	cmp	edi,02
	jl	000000014000156A

l00000001400015B9:
	add	rsp,40
	pop	rdi
	pop	rsi
	pop	rbx
	ret
00000001400015C1    CC CC CC                                      ...           

;; fn00000001400015C4: 00000001400015C4
;;   Called from:
;;     00000001400012EC (in fn00000001400012BC)
fn00000001400015C4 proc
	sub	rsp,28
	call	0000000140001DC4
	test	eax,eax
	jz	00000001400015F2

l00000001400015D1:
	mov	rax,gs:[00000030]
	mov	rcx,[rax+08]
	jmp	00000001400015E5

l00000001400015E0:
	cmp	rcx,rax
	jz	00000001400015F9

l00000001400015E5:
	xor	eax,eax
	lock
	cmpxchg	[00000001400035B8],rcx                             ; [rip+00001FC8]
	jnz	00000001400015E0

l00000001400015F2:
	xor	al,al

l00000001400015F4:
	add	rsp,28
	ret

l00000001400015F9:
	mov	al,01
	jmp	00000001400015F4
00000001400015FD                                        CC CC CC              ...

;; fn0000000140001600: 0000000140001600
;;   Called from:
;;     00000001400012D0 (in fn00000001400012BC)
fn0000000140001600 proc
	push	rbx
	sub	rsp,20
	movzx	eax,[00000001400035F0]                               ; [rip+00001FE3]
	test	ecx,ecx
	mov	ebx,00000001
	cmovz	eax,ebx

l0000000140001617:
	mov	[00000001400035F0],al                                  ; [rip+00001FD3]
	call	0000000140001BFC
	call	0000000140001938
	test	al,al
	jnz	000000014000162F

l000000014000162B:
	xor	al,al
	jmp	0000000140001643

l000000014000162F:
	call	0000000140001938
	test	al,al
	jnz	0000000140001641

l0000000140001638:
	xor	ecx,ecx
	call	0000000140001938
	jmp	000000014000162B

l0000000140001641:
	mov	al,bl

l0000000140001643:
	add	rsp,20
	pop	rbx
	ret
0000000140001649                            CC CC CC                      ...   

;; fn000000014000164C: 000000014000164C
;;   Called from:
;;     0000000140001204 (in fn00000001400011D4)
fn000000014000164C proc
	mov	[rsp+08],rbx
	push	rbp
	mov	rbp,rsp
	sub	rsp,40
	mov	ebx,ecx
	cmp	ecx,01
	ja	000000014000170A

l0000000140001664:
	call	0000000140001DC4
	test	eax,eax
	jz	0000000140001698

l000000014000166D:
	test	ebx,ebx
	jnz	0000000140001698

l0000000140001671:
	lea	rcx,[00000001400035C0]                                 ; [rip+00001F48]
	call	0000000140001E5E
	test	eax,eax
	jz	0000000140001685

l0000000140001681:
	xor	al,al
	jmp	00000001400016FF

l0000000140001685:
	lea	rcx,[00000001400035D8]                                 ; [rip+00001F4C]
	call	0000000140001E5E
	test	eax,eax
	setz	al
	jmp	00000001400016FF

l0000000140001698:
	mov	rdx,[0000000140003000]                                 ; [rip+00001961]
	or	r8,FF
	mov	eax,edx
	mov	ecx,00000040
	and	eax,3F
	sub	ecx,eax
	mov	al,01
	ror	r8,cl
	xor	r8,rdx
	mov	[rbp-20],r8
	mov	[rbp-18],r8
	movups	xmm0,[rbp-20]
	mov	[rbp-10],r8
	movsd	xmm1,double ptr [rbp-10]
	movups	[00000001400035C0],xmm0                             ; [rip+00001EED]
	mov	[rbp-20],r8
	mov	[rbp-18],r8
	movups	xmm0,[rbp-20]
	mov	[rbp-10],r8
	movsd	[00000001400035D0],xmm1                              ; [rip+00001EE5]
	movsd	xmm1,double ptr [rbp-10]
	movups	[00000001400035D8],xmm0                             ; [rip+00001EE1]
	movsd	[00000001400035E8],xmm1                              ; [rip+00001EE9]

l00000001400016FF:
	mov	rbx,[rsp+50]
	add	rsp,40
	pop	rbp
	ret

l000000014000170A:
	mov	ecx,00000005
	call	0000000140001974
	int	03
	int	03
	int	03
	int	03

;; fn0000000140001718: 0000000140001718
;;   Called from:
;;     0000000140001376 (in fn00000001400012BC)
;;     00000001400013A6 (in fn00000001400012BC)
;;     0000000140001717 (in fn000000014000164C)
fn0000000140001718 proc
	sub	rsp,18
	mov	r8,rcx
	mov	eax,00005A4D
	cmp	[0000000140000000],ax                                  ; [rip-0000172B]
	jnz	00000001400017A6

l000000014000172D:
	movsx	rax,[000000014000003C]                               ; [rip-000016F8]
	lea	rdx,[0000000140000000]                                 ; [rip-0000173B]
	lea	rcx,[rax+rdx]
	cmp	dword ptr [rcx],00004550
	jnz	00000001400017A6

l0000000140001747:
	mov	eax,0000020B
	cmp	[rcx+18],ax
	jnz	00000001400017A6

l0000000140001752:
	sub	r8,rdx
	movzx	eax,word ptr [rcx+14]
	lea	rdx,[rcx+18]
	add	rdx,rax
	movzx	eax,word ptr [rcx+06]
	lea	rcx,[rax+rax*4]
	lea	r9,[rdx+rcx*8]

l000000014000176C:
	mov	[rsp],rdx
	cmp	rdx,r9
	jz	000000014000178D

l0000000140001775:
	mov	ecx,[rdx+0C]
	cmp	r8,rcx
	jc	0000000140001787

l000000014000177D:
	mov	eax,[rdx+08]
	add	eax,ecx
	cmp	r8,rax
	jc	000000014000178F

l0000000140001787:
	add	rdx,28
	jmp	000000014000176C

l000000014000178D:
	xor	edx,edx

l000000014000178F:
	test	rdx,rdx
	jnz	0000000140001798

l0000000140001794:
	xor	al,al
	jmp	00000001400017AC

l0000000140001798:
	cmp	dword ptr [rdx+24],00
	jge	00000001400017A2

l000000014000179E:
	xor	al,al
	jmp	00000001400017AC

l00000001400017A2:
	mov	al,01
	jmp	00000001400017AC

l00000001400017A6:
	xor	al,al
	jmp	00000001400017AC
00000001400017AA                               32 C0                       2.   

l00000001400017AC:
	add	rsp,18
	ret
00000001400017B1    CC CC CC                                      ...           

;; fn00000001400017B4: 00000001400017B4
;;   Called from:
;;     0000000140001360 (in fn00000001400012BC)
fn00000001400017B4 proc
	push	rbx
	sub	rsp,20
	mov	bl,cl
	call	0000000140001DC4
	xor	edx,edx
	test	eax,eax
	jz	00000001400017D2

l00000001400017C7:
	test	bl,bl
	jnz	00000001400017D2

l00000001400017CB:
	xchg	[00000001400035B8],rdx                                ; [rip+00001DE6]

l00000001400017D2:
	add	rsp,20
	pop	rbx
	ret

;; fn00000001400017D8: 00000001400017D8
;;   Called from:
;;     00000001400013F9 (in fn00000001400012BC)
fn00000001400017D8 proc
	push	rbx
	sub	rsp,20
	cmp	[00000001400035F0],00                                  ; [rip+00001E0B]
	mov	bl,cl
	jz	00000001400017ED

l00000001400017E9:
	test	dl,dl
	jnz	00000001400017FB

l00000001400017ED:
	mov	cl,bl
	call	0000000140001938
	mov	cl,bl
	call	0000000140001938

l00000001400017FB:
	mov	al,01
	add	rsp,20
	pop	rbx
	ret
0000000140001803          CC                                        .           

;; fn0000000140001804: 0000000140001804
;;   Called from:
;;     0000000140001858 (in fn0000000140001854)
fn0000000140001804 proc
	push	rbx
	sub	rsp,20
	mov	rdx,[0000000140003000]                                 ; [rip+000017EF]
	mov	rbx,rcx
	mov	ecx,edx
	xor	rdx,[00000001400035C0]                                 ; [rip+00001DA3]
	and	ecx,3F
	ror	rdx,cl
	cmp	rdx,FF
	jnz	0000000140001833

l0000000140001829:
	mov	rcx,rbx
	call	0000000140001E6A
	jmp	0000000140001842

l0000000140001833:
	mov	rdx,rbx
	lea	rcx,[00000001400035C0]                                 ; [rip+00001D83]
	call	0000000140001E64

l0000000140001842:
	xor	ecx,ecx
	test	eax,eax
	cmovz	rcx,rbx

l000000014000184A:
	mov	rax,rcx
	add	rsp,20
	pop	rbx
	ret
0000000140001853          CC                                        .           

;; fn0000000140001854: 0000000140001854
;;   Called from:
;;     0000000140001219 (in fn00000001400011D4)
fn0000000140001854 proc
	sub	rsp,28
	call	0000000140001804
	neg	rax
	sbb	eax,eax
	neg	eax
	dec	eax
	add	rsp,28
	ret
000000014000186B                                  CC                        .   

;; fn000000014000186C: 000000014000186C
;;   Called from:
;;     0000000140001438 (in Win32CrtStartup)
fn000000014000186C proc
	mov	[rsp+20],rbx
	push	rbp
	mov	rbp,rsp
	sub	rsp,20
	and	qword ptr [rbp+18],00
	mov	rbx,2B992DDFA232
	mov	rax,[0000000140003000]                                 ; [rip+00001771]
	cmp	rax,rbx
	jnz	0000000140001903

l0000000140001894:
	lea	rcx,[rbp+18]
	call	[0000000140002030]                                    ; [rip+00000792]
	mov	rax,[rbp+18]
	mov	[rbp+10],rax
	call	[0000000140002038]                                    ; [rip+0000078C]
	mov	eax,eax
	xor	[rbp+10],rax
	call	[0000000140002040]                                    ; [rip+00000788]
	mov	eax,eax
	lea	rcx,[rbp+20]
	xor	[rbp+10],rax
	call	[0000000140002048]                                    ; [rip+00000780]
	mov	eax,[rbp+20]
	lea	rcx,[rbp+10]
	shl	rax,20
	xor	rax,[rbp+20]
	xor	rax,[rbp+10]
	xor	rax,rcx
	mov	rcx,FFFFFFFFFFFF
	and	rax,rcx
	mov	rcx,2B992DDFA233
	cmp	rax,rbx
	cmovz	rax,rcx

l00000001400018FC:
	mov	[0000000140003000],rax                                 ; [rip+000016FD]

l0000000140001903:
	mov	rbx,[rsp+48]
	not	rax
	mov	[0000000140003008],rax                                 ; [rip+000016F6]
	add	rsp,20
	pop	rbp
	ret

;; fn0000000140001918: 0000000140001918
;;   Called from:
;;     000000014000121E (in fn00000001400011D4)
fn0000000140001918 proc
	mov	eax,00000001
	ret
000000014000191E                                           CC CC               ..

;; fn0000000140001920: 0000000140001920
;;   Called from:
;;     00000001400011E4 (in fn00000001400011D4)
fn0000000140001920 proc
	mov	eax,00004000
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
	mov	al,01
	ret
000000014000193B                                  CC                        .   

;; fn000000014000193C: 000000014000193C
;;   Called from:
;;     0000000140001294 (in fn0000000140001290)
fn000000014000193C proc
	sub	rsp,28
	call	0000000140001130
	or	qword ptr [rax],04
	call	00000001400010C0
	or	qword ptr [rax],02
	add	rsp,28
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
	mov	[rsp+08],rbx
	push	rbp
	lea	rbp,[rsp-000004C0]
	sub	rsp,+000005C0
	mov	ebx,ecx
	mov	ecx,00000017
	call	0000000140001E76
	test	eax,eax
	jz	000000014000199D

l0000000140001999:
	mov	ecx,ebx
	int	29

l000000014000199D:
	and	[0000000140003610],00                                  ; [rip+00001C6C]
	lea	rcx,[rbp-10]
	xor	edx,edx
	mov	r8d,000004D0
	call	0000000140001DE6
	lea	rcx,[rbp-10]
	call	[0000000140002070]                                    ; [rip+000006B1]
	mov	rbx,[rbp+000000E8]
	lea	rdx,[rbp+000004D8]
	mov	rcx,rbx
	xor	r8d,r8d
	call	[0000000140002000]                                    ; [rip+00000627]
	test	rax,rax
	jz	0000000140001A1A

l00000001400019DE:
	and	qword ptr [rsp+38],00
	lea	rcx,[rbp+000004E0]
	mov	rdx,[rbp+000004D8]
	mov	r9,rax
	mov	[rsp+30],rcx
	mov	r8,rbx
	lea	rcx,[rbp+000004E8]
	mov	[rsp+28],rcx
	lea	rcx,[rbp-10]
	mov	[rsp+20],rcx
	xor	ecx,ecx
	call	[0000000140002008]                                    ; [rip+000005EE]

l0000000140001A1A:
	mov	rax,[rbp+000004C8]
	lea	rcx,[rsp+50]
	mov	[rbp+000000E8],rax
	xor	edx,edx
	lea	rax,[rbp+000004C8]
	mov	r8d,00000098
	add	rax,08
	mov	[rbp+00000088],rax
	call	0000000140001DE6
	mov	rax,[rbp+000004C8]
	mov	[rsp+60],rax
	mov	dword ptr [rsp+50],40000015
	mov	dword ptr [rsp+54],00000001
	call	[0000000140002020]                                    ; [rip+000005B2]
	cmp	eax,01
	lea	rax,[rsp+50]
	mov	[rsp+40],rax
	lea	rax,[rbp-10]
	setz	bl
	mov	[rsp+48],rax
	xor	ecx,ecx
	call	[0000000140002068]                                    ; [rip+000005D9]
	lea	rcx,[rsp+40]
	call	[0000000140002010]                                    ; [rip+00000576]
	test	eax,eax
	jnz	0000000140001AA8

l0000000140001A9E:
	neg	bl
	sbb	eax,eax
	and	[0000000140003610],eax                                 ; [rip+00001B68]

l0000000140001AA8:
	mov	rbx,[rsp+000005D0]
	add	rsp,+000005C0
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
	sub	rsp,28
	xor	ecx,ecx
	call	[0000000140002018]                                    ; [rip+0000054C]
	mov	rcx,rax
	test	rax,rax
	jnz	0000000140001AD8

l0000000140001AD4:
	xor	al,al
	jmp	0000000140001B0F

l0000000140001AD8:
	mov	eax,00005A4D
	cmp	[rcx],ax
	jnz	0000000140001AD4

l0000000140001AE2:
	movsx	rax,dword ptr [rcx+3C]
	add	rax,rcx
	cmp	dword ptr [rax],00004550
	jnz	0000000140001AD4

l0000000140001AF1:
	mov	ecx,0000020B
	cmp	[rax+18],cx
	jnz	0000000140001AD4

l0000000140001AFC:
	cmp	dword ptr [rax+00000084],0E
	jbe	0000000140001AD4

l0000000140001B05:
	cmp	dword ptr [rax+000000F8],00
	setnz	al

l0000000140001B0F:
	add	rsp,28
	ret

;; fn0000000140001B14: 0000000140001B14
;;   Called from:
;;     00000001400012A4 (in fn00000001400012A0)
fn0000000140001B14 proc
	lea	rcx,[0000000140001B24]                                 ; [rip+00000009]
	jmp	[0000000140002068]                                     ; [rip+00000546]
0000000140001B22       CC CC                                       ..           

;; fn0000000140001B24: 0000000140001B24
fn0000000140001B24 proc
	sub	rsp,28
	mov	rax,[rcx]
	cmp	dword ptr [rax],E06D7363
	jnz	0000000140001B4F

l0000000140001B33:
	cmp	dword ptr [rax+18],04
	jnz	0000000140001B4F

l0000000140001B39:
	mov	ecx,[rax+20]
	lea	eax,[rcx+E66CFAE0]
	cmp	eax,02
	jbe	0000000140001B56

l0000000140001B47:
	cmp	ecx,01994000
	jz	0000000140001B56

l0000000140001B4F:
	xor	eax,eax
	add	rsp,28
	ret

l0000000140001B56:
	call	0000000140001E70
	int	03

;; fn0000000140001B5C: 0000000140001B5C
;;   Called from:
;;     000000014000120D (in fn00000001400011D4)
;;     0000000140001B5B (in fn0000000140001B24)
fn0000000140001B5C proc
	mov	[rsp+08],rbx
	mov	[rsp+10],rsi
	push	rdi
	sub	rsp,20
	lea	rbx,[0000000140002680]                                 ; [rip+00000B0E]
	lea	rsi,[0000000140002680]                                 ; [rip+00000B07]
	jmp	0000000140001B91

l0000000140001B7B:
	mov	rdi,[rbx]
	test	rdi,rdi
	jz	0000000140001B8D

l0000000140001B83:
	mov	rcx,rdi
	call	0000000140001BF4
	call	rdi

l0000000140001B8D:
	add	rbx,08

l0000000140001B91:
	cmp	rbx,rsi
	jc	0000000140001B7B

l0000000140001B96:
	mov	rbx,[rsp+30]
	mov	rsi,[rsp+38]
	add	rsp,20
	pop	rdi
	ret
0000000140001BA6                   CC CC                               ..       

;; fn0000000140001BA8: 0000000140001BA8
fn0000000140001BA8 proc
	mov	[rsp+08],rbx
	mov	[rsp+10],rsi
	push	rdi
	sub	rsp,20
	lea	rbx,[0000000140002690]                                 ; [rip+00000AD2]
	lea	rsi,[0000000140002690]                                 ; [rip+00000ACB]
	jmp	0000000140001BDD

l0000000140001BC7:
	mov	rdi,[rbx]
	test	rdi,rdi
	jz	0000000140001BD9

l0000000140001BCF:
	mov	rcx,rdi
	call	0000000140001BF4
	call	rdi

l0000000140001BD9:
	add	rbx,08

l0000000140001BDD:
	cmp	rbx,rsi
	jc	0000000140001BC7

l0000000140001BE2:
	mov	rbx,[rsp+30]
	mov	rsi,[rsp+38]
	add	rsp,20
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
	mov	[rsp+10],rbx
	mov	[rsp+18],rdi
	push	rbp
	mov	rbp,rsp
	sub	rsp,20
	and	dword ptr [rbp-18],00
	xor	ecx,ecx
	xor	eax,eax
	mov	[000000014000301C],00000002                            ; [rip+000013FC]
	cpuid
	mov	r8d,ecx
	mov	[0000000140003018],00000001                            ; [rip+000013E9]
	xor	ecx,444D4163
	mov	r9d,edx
	mov	r10d,edx
	xor	r9d,69746E65
	xor	r10d,49656E69
	xor	r8d,6C65746E
	or	r10d,r8d
	mov	r11d,ebx
	mov	r8d,[0000000140003614]                                 ; [rip+000019B7]
	xor	r11d,68747541
	or	r11d,r9d
	mov	edx,ebx
	or	r11d,ecx
	xor	edx,756E6547
	xor	ecx,ecx
	mov	edi,eax
	or	r10d,edx
	mov	eax,00000001
	cpuid
	mov	[rbp-10],eax
	mov	r9d,ecx
	mov	[rbp-08],r9d
	mov	ecx,eax
	mov	[rbp-0C],ebx
	mov	[rbp-04],edx
	test	r10d,r10d
	jnz	0000000140001CE9

l0000000140001C97:
	or	[0000000140003020],FF                                   ; [rip+00001381]
	or	r8d,04
	and	eax,0FFF3FF0
	mov	[0000000140003614],r8d                                 ; [rip+00001965]
	cmp	eax,000106C0
	jz	0000000140001CDE

l0000000140001CB6:
	cmp	eax,00020660
	jz	0000000140001CDE

l0000000140001CBD:
	cmp	eax,00020670
	jz	0000000140001CDE

l0000000140001CC4:
	add	eax,FFFCF9B0
	cmp	eax,20
	ja	0000000140001CE9

l0000000140001CCE:
	mov	rbx,100010001
	bt	rbx,rax
	jnc	0000000140001CE9

l0000000140001CDE:
	or	r8d,01
	mov	[0000000140003614],r8d                                 ; [rip+0000192B]

l0000000140001CE9:
	test	r11d,r11d
	jnz	0000000140001D07

l0000000140001CEE:
	and	ecx,0FF00F00
	cmp	ecx,00600F00
	jc	0000000140001D07

l0000000140001CFC:
	or	r8d,04
	mov	[0000000140003614],r8d                                 ; [rip+0000190D]

l0000000140001D07:
	mov	eax,00000007
	mov	[rbp-20],edx
	mov	[rbp-1C],r9d
	cmp	edi,eax
	jl	0000000140001D3B

l0000000140001D17:
	xor	ecx,ecx
	cpuid
	mov	[rbp-10],eax
	mov	[rbp-0C],ebx
	mov	[rbp-08],ecx
	mov	[rbp-04],edx
	mov	[rbp-18],ebx
	bt	ebx,09
	jnc	0000000140001D3B

l0000000140001D30:
	or	r8d,02
	mov	[0000000140003614],r8d                                 ; [rip+000018D9]

l0000000140001D3B:
	bt	r9d,14
	jnc	0000000140001DB0

l0000000140001D42:
	mov	[0000000140003018],00000002                            ; [rip+000012CC]
	mov	[000000014000301C],00000006                            ; [rip+000012C6]
	bt	r9d,1B
	jnc	0000000140001DB0

l0000000140001D5D:
	bt	r9d,1C
	jnc	0000000140001DB0

l0000000140001D64:
	xor	ecx,ecx
	xgetbv
	shl	rdx,20
	or	rdx,rax
	mov	[rbp+10],rdx
	mov	rax,[rbp+10]
	and	al,06
	cmp	al,06
	jnz	0000000140001DB0

l0000000140001D7E:
	mov	eax,[000000014000301C]                                 ; [rip+00001298]
	or	eax,08
	mov	[0000000140003018],00000003                            ; [rip+00001287]
	test	byte ptr [rbp-18],20
	mov	[000000014000301C],eax                                 ; [rip+00001281]
	jz	0000000140001DB0

l0000000140001D9D:
	or	eax,20
	mov	[0000000140003018],00000005                            ; [rip+0000126E]
	mov	[000000014000301C],eax                                 ; [rip+0000126C]

l0000000140001DB0:
	mov	rbx,[rsp+38]
	xor	eax,eax
	mov	rdi,[rsp+40]
	add	rsp,20
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
	ret	0000
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
	sub	rsp,28
	mov	r8,[r9+38]
	mov	rcx,rdx
	mov	rdx,r9
	call	0000000140001E9C
	mov	eax,00000001
	add	rsp,28
	ret
0000000140001E99                            CC CC CC                      ...   

;; fn0000000140001E9C: 0000000140001E9C
;;   Called from:
;;     0000000140001E8A (in fn0000000140001E7C)
fn0000000140001E9C proc
	push	rbx
	mov	r11d,[r8]
	mov	rbx,rdx
	and	r11d,F8
	mov	r9,rcx
	test	byte ptr [r8],04
	mov	r10,rcx
	jz	0000000140001EC7

l0000000140001EB4:
	mov	eax,[r8+08]
	movsx	r10,dword ptr [r8+04]
	neg	eax
	add	r10,rcx
	movsx	rcx,eax
	and	r10,rcx

l0000000140001EC7:
	movsx	rax,r11d
	mov	rdx,[rax+r10]
	mov	rax,[rbx+10]
	mov	ecx,[rax+08]
	add	rcx,[rbx+08]
	test	byte ptr [rcx+03],0F
	jz	0000000140001EE9

l0000000140001EDF:
	movzx	eax,byte ptr [rcx+03]
	and	eax,F0
	add	r9,rax

l0000000140001EE9:
	xor	r9,rdx
	mov	rcx,r9
	pop	rbx
	jmp	00000001400011B0
0000000140001EF5                CC CC CC CC CC CC CC CC CC CC CC      ...........
0000000140001F00 CC CC CC CC CC CC 66 66 0F 1F 84 00 00 00 00 00 ......ff........

;; fn0000000140001F10: 0000000140001F10
fn0000000140001F10 proc
	jmp	rax

;; fn0000000140001F12: 0000000140001F12
fn0000000140001F12 proc
	push	rbp
	sub	rsp,20
	mov	rbp,rdx
	mov	rax,[rcx]
	mov	rdx,rcx
	mov	ecx,[rax]
	call	0000000140001DEC
	nop
	add	rsp,20
	pop	rbp
	ret
0000000140001F2F                                              CC                .

;; fn0000000140001F30: 0000000140001F30
fn0000000140001F30 proc
	push	rbp
	mov	rbp,rdx
	mov	rax,[rcx]
	xor	ecx,ecx
	cmp	dword ptr [rax],C0000005
	setz	cl
	mov	eax,ecx
	pop	rbp
	ret
0000000140001F47                      CC                                .       
;;; Segment .rdata (0000000140002000)
__imp__RtlLookupFunctionEntry		; 0000000140002000
	dq	0x0000000000002CA4
__imp__RtlVirtualUnwind		; 0000000140002008
	dq	0x0000000000002CBE
__imp__UnhandledExceptionFilter		; 0000000140002010
	dq	0x0000000000002CD2
__imp__GetModuleHandleW		; 0000000140002018
	dq	0x0000000000002DDA
__imp__IsDebuggerPresent		; 0000000140002020
	dq	0x0000000000002DC6
__imp__InitializeSListHead		; 0000000140002028
	dq	0x0000000000002DB0
__imp__GetSystemTimeAsFileTime		; 0000000140002030
	dq	0x0000000000002D96
__imp__GetCurrentThreadId		; 0000000140002038
	dq	0x0000000000002D80
__imp__GetCurrentProcessId		; 0000000140002040
	dq	0x0000000000002D6A
__imp__QueryPerformanceCounter		; 0000000140002048
	dq	0x0000000000002D50
__imp__IsProcessorFeaturePresent		; 0000000140002050
	dq	0x0000000000002D34
__imp__TerminateProcess		; 0000000140002058
	dq	0x0000000000002D20
__imp__GetCurrentProcess		; 0000000140002060
	dq	0x0000000000002D0C
__imp__SetUnhandledExceptionFilter		; 0000000140002068
	dq	0x0000000000002CEE
__imp__RtlCaptureContext		; 0000000140002070
	dq	0x0000000000002C90
0000000140002078                         00 00 00 00 00 00 00 00         ........
__imp__memset		; 0000000140002080
	dq	0x00000000000029D8
__imp____C_specific_handler		; 0000000140002088
	dq	0x00000000000029C0
0000000140002090 00 00 00 00 00 00 00 00                         ........       
__imp___set_new_mode		; 0000000140002098
	dq	0x0000000000002B7A
00000001400020A0 00 00 00 00 00 00 00 00                         ........       
__imp___configthreadlocale		; 00000001400020A8
	dq	0x0000000000002B64
00000001400020B0 00 00 00 00 00 00 00 00                         ........       
__imp____setusermatherr		; 00000001400020B8
	dq	0x0000000000002A5C
00000001400020C0 00 00 00 00 00 00 00 00                         ........       
__imp___cexit		; 00000001400020C8
	dq	0x0000000000002B22
__imp___register_onexit_function		; 00000001400020D0
	dq	0x0000000000002BB6
__imp___crt_atexit		; 00000001400020D8
	dq	0x0000000000002BD2
__imp__terminate		; 00000001400020E0
	dq	0x0000000000002BE0
__imp___set_app_type		; 00000001400020E8
	dq	0x0000000000002A4C
__imp___seh_filter_exe		; 00000001400020F0
	dq	0x0000000000002A3A
__imp___register_thread_local_exe_atexit_callback		; 00000001400020F8
	dq	0x0000000000002B36
__imp____p___argv		; 0000000140002100
	dq	0x0000000000002B14
__imp____p___argc		; 0000000140002108
	dq	0x0000000000002B06
__imp___c_exit		; 0000000140002110
	dq	0x0000000000002B2C
__imp___exit		; 0000000140002118
	dq	0x0000000000002AF0
__imp__exit		; 0000000140002120
	dq	0x0000000000002AE8
__imp___initterm_e		; 0000000140002128
	dq	0x0000000000002ADA
__imp___initterm		; 0000000140002130
	dq	0x0000000000002ACE
__imp___get_initial_narrow_environment		; 0000000140002138
	dq	0x0000000000002AAC
__imp___initialize_narrow_environment		; 0000000140002140
	dq	0x0000000000002A8A
__imp___configure_narrow_argv		; 0000000140002148
	dq	0x0000000000002A70
__imp___initialize_onexit_table		; 0000000140002150
	dq	0x0000000000002B9A
0000000140002158                         00 00 00 00 00 00 00 00         ........
__imp____p__commode		; 0000000140002160
	dq	0x0000000000002B8A
__imp____stdio_common_vfscanf		; 0000000140002168
	dq	0x0000000000002A20
__imp____stdio_common_vfprintf		; 0000000140002170
	dq	0x0000000000002A06
__imp____acrt_iob_func		; 0000000140002178
	dq	0x00000000000029F4
__imp___set_fmode		; 0000000140002180
	dq	0x0000000000002AF8
0000000140002188                         00 00 00 00 00 00 00 00         ........
0000000140002190 D0 1D 00 40 01 00 00 00 10 1F 00 40 01 00 00 00 ...@.......@....
00000001400021A0 00 00 00 00 00 00 00 00 A0 12 00 40 01 00 00 00 ...........@....
00000001400021B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000001400021C0 D4 11 00 40 01 00 00 00 90 12 00 40 01 00 00 00 ...@.......@....
00000001400021D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
0000000140002200 40 30 00 40 01 00 00 00 E0 30 00 40 01 00 00 00 @0.@.....0.@....
0000000140002210 25 69 20 25 73 20 25 53 20 25 66 20 25 6C 66 20 %i %s %S %f %lf 
0000000140002220 25 63 20 25 43 00 00 00 25 69 20 25 33 30 73 20 %c %C...%i %30s 
0000000140002230 25 53 20 25 66 20 25 6C 66 20 25 63 20 25 43 00 %S %f %lf %c %C.
0000000140002240 48 00 65 00 6C 00 6C 00 6F 00 21 00 00 00 00 00 H.e.l.l.o.!.....
0000000140002250 68 65 6C 6C 6F 00 00 00 00 00 00 00 00 00 00 00 hello...........
0000000140002260 00 00 00 00 00 00 08 40 1F 85 EB 51 B8 1E 09 40 .......@...Q...@
0000000140002270 00 00 00 00 3A 1E BC 57 00 00 00 00 02 00 00 00 ....:..W........
0000000140002280 6E 00 00 00 74 23 00 00 74 17 00 00 00 00 00 00 n...t#..t.......
0000000140002290 3A 1E BC 57 00 00 00 00 0C 00 00 00 14 00 00 00 :..W............
00000001400022A0 E4 23 00 00 E4 17 00 00 00 00 00 00 3A 1E BC 57 .#..........:..W
00000001400022B0 00 00 00 00 0D 00 00 00 80 02 00 00 F8 23 00 00 .............#..
00000001400022C0 F8 17 00 00 00 00 00 00 3A 1E BC 57 00 00 00 00 ........:..W....
00000001400022D0 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000001400022E0 94 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000001400022F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
0000000140002330 00 00 00 00 00 00 00 00 00 30 00 40 01 00 00 00 .........0.@....
0000000140002340 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000000140002350 90 21 00 40 01 00 00 00 98 21 00 40 01 00 00 00 .!.@.....!.@....
0000000140002360 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000000140002370 00 01 00 00 52 53 44 53 82 9B AA CF 0E 32 38 49 ....RSDS.....28I
0000000140002380 80 B4 F6 58 31 8E 26 F4 03 00 00 00 44 3A 5C 64 ...X1.&.....D:\d
0000000140002390 65 76 5C 75 78 6D 61 6C 5C 72 65 6B 6F 5C 6D 61 ev\uxmal\reko\ma
00000001400023A0 73 74 65 72 5C 73 75 62 6A 65 63 74 73 5C 50 45 ster\subjects\PE
00000001400023B0 2D 78 38 36 2D 36 34 5C 76 61 72 61 72 67 73 5F -x86-64\varargs_
00000001400023C0 74 65 73 74 5C 78 36 34 5C 52 65 6C 65 61 73 65 test\x64\Release
00000001400023D0 5C 76 61 72 61 72 67 73 5F 74 65 73 74 2E 70 64 \varargs_test.pd
00000001400023E0 62 00 00 00 00 00 00 00 20 00 00 00 20 00 00 00 b....... ... ...
00000001400023F0 02 00 00 00 1E 00 00 00 47 43 54 4C 00 10 00 00 ........GCTL....
0000000140002400 00 0F 00 00 2E 74 65 78 74 24 6D 6E 00 00 00 00 .....text$mn....
0000000140002410 00 1F 00 00 12 00 00 00 2E 74 65 78 74 24 6D 6E .........text$mn
0000000140002420 24 30 30 00 12 1F 00 00 36 00 00 00 2E 74 65 78 $00.....6....tex
0000000140002430 74 24 78 00 00 20 00 00 90 01 00 00 2E 69 64 61 t$x.. .......ida
0000000140002440 74 61 24 35 00 00 00 00 90 21 00 00 10 00 00 00 ta$5.....!......
0000000140002450 2E 30 30 63 66 67 00 00 A0 21 00 00 08 00 00 00 .00cfg...!......
0000000140002460 2E 43 52 54 24 58 43 41 00 00 00 00 A8 21 00 00 .CRT$XCA.....!..
0000000140002470 08 00 00 00 2E 43 52 54 24 58 43 41 41 00 00 00 .....CRT$XCAA...
0000000140002480 B0 21 00 00 08 00 00 00 2E 43 52 54 24 58 43 5A .!.......CRT$XCZ
0000000140002490 00 00 00 00 B8 21 00 00 08 00 00 00 2E 43 52 54 .....!.......CRT
00000001400024A0 24 58 49 41 00 00 00 00 C0 21 00 00 08 00 00 00 $XIA.....!......
00000001400024B0 2E 43 52 54 24 58 49 41 41 00 00 00 C8 21 00 00 .CRT$XIAA....!..
00000001400024C0 08 00 00 00 2E 43 52 54 24 58 49 41 43 00 00 00 .....CRT$XIAC...
00000001400024D0 D0 21 00 00 08 00 00 00 2E 43 52 54 24 58 49 5A .!.......CRT$XIZ
00000001400024E0 00 00 00 00 D8 21 00 00 08 00 00 00 2E 43 52 54 .....!.......CRT
00000001400024F0 24 58 50 41 00 00 00 00 E0 21 00 00 08 00 00 00 $XPA.....!......
0000000140002500 2E 43 52 54 24 58 50 5A 00 00 00 00 E8 21 00 00 .CRT$XPZ.....!..
0000000140002510 08 00 00 00 2E 43 52 54 24 58 54 41 00 00 00 00 .....CRT$XTA....
0000000140002520 F0 21 00 00 10 00 00 00 2E 43 52 54 24 58 54 5A .!.......CRT$XTZ
0000000140002530 00 00 00 00 00 22 00 00 74 01 00 00 2E 72 64 61 ....."..t....rda
0000000140002540 74 61 00 00 74 23 00 00 04 03 00 00 2E 72 64 61 ta..t#.......rda
0000000140002550 74 61 24 7A 7A 7A 64 62 67 00 00 00 78 26 00 00 ta$zzzdbg...x&..
0000000140002560 08 00 00 00 2E 72 74 63 24 49 41 41 00 00 00 00 .....rtc$IAA....
0000000140002570 80 26 00 00 08 00 00 00 2E 72 74 63 24 49 5A 5A .&.......rtc$IZZ
0000000140002580 00 00 00 00 88 26 00 00 08 00 00 00 2E 72 74 63 .....&.......rtc
0000000140002590 24 54 41 41 00 00 00 00 90 26 00 00 08 00 00 00 $TAA.....&......
00000001400025A0 2E 72 74 63 24 54 5A 5A 00 00 00 00 98 26 00 00 .rtc$TZZ.....&..
00000001400025B0 F4 00 00 00 2E 78 64 61 74 61 00 00 8C 27 00 00 .....xdata...'..
00000001400025C0 8C 00 00 00 2E 69 64 61 74 61 24 32 00 00 00 00 .....idata$2....
00000001400025D0 18 28 00 00 18 00 00 00 2E 69 64 61 74 61 24 33 .(.......idata$3
00000001400025E0 00 00 00 00 30 28 00 00 90 01 00 00 2E 69 64 61 ....0(.......ida
00000001400025F0 74 61 24 34 00 00 00 00 C0 29 00 00 3C 04 00 00 ta$4.....)..<...
0000000140002600 2E 69 64 61 74 61 24 36 00 00 00 00 00 30 00 00 .idata$6.....0..
0000000140002610 40 00 00 00 2E 64 61 74 61 00 00 00 40 30 00 00 @....data...@0..
0000000140002620 00 06 00 00 2E 62 73 73 00 00 00 00 00 40 00 00 .....bss.....@..
0000000140002630 8C 01 00 00 2E 70 64 61 74 61 00 00 00 50 00 00 .....pdata...P..
0000000140002640 1C 00 00 00 2E 67 66 69 64 73 24 79 00 00 00 00 .....gfids$y....
0000000140002650 00 60 00 00 60 00 00 00 2E 72 73 72 63 24 30 31 .`..`....rsrc$01
0000000140002660 00 00 00 00 60 60 00 00 80 01 00 00 2E 72 73 72 ....``.......rsr
0000000140002670 63 24 30 32 00 00 00 00 00 00 00 00 00 00 00 00 c$02............
0000000140002680 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000000140002690 00 00 00 00 00 00 00 00 19 19 02 00 07 01 19 00 ................
00000001400026A0 7C 1E 00 00 B8 00 00 00 01 1B 04 00 1B 52 17 70 |............R.p
00000001400026B0 16 60 15 30 00 00 00 00 01 00 00 00 09 0F 06 00 .`.0............
00000001400026C0 0F 64 09 00 0F 34 08 00 0F 52 0B 70 E0 1D 00 00 .d...4...R.p....
00000001400026D0 01 00 00 00 EC 12 00 00 02 14 00 00 12 1F 00 00 ................
00000001400026E0 02 14 00 00 01 06 02 00 06 32 02 50 01 09 01 00 .........2.P....
00000001400026F0 09 62 00 00 01 08 04 00 08 72 04 70 03 60 02 30 .b.......r.p.`.0
0000000140002700 09 04 01 00 04 22 00 00 E0 1D 00 00 01 00 00 00 ....."..........
0000000140002710 1F 17 00 00 AA 17 00 00 30 1F 00 00 AA 17 00 00 ........0.......
0000000140002720 01 02 01 00 02 50 00 00 01 06 02 00 06 32 02 30 .....P.......2.0
0000000140002730 01 0D 04 00 0D 34 0A 00 0D 72 06 50 01 0D 04 00 .....4...r.P....
0000000140002740 0D 34 09 00 0D 32 06 50 01 15 05 00 15 34 BA 00 .4...2.P.....4..
0000000140002750 15 01 B8 00 06 50 00 00 01 0F 06 00 0F 64 07 00 .....P.......d..
0000000140002760 0F 34 06 00 0F 32 0B 70 01 12 06 00 12 74 08 00 .4...2.p.....t..
0000000140002770 12 34 07 00 12 32 0B 50 01 00 00 00 01 02 01 00 .4...2.P........
0000000140002780 02 30 00 00 01 04 01 00 04 42 00 00 B0 28 00 00 .0.......B...(..
0000000140002790 00 00 00 00 00 00 00 00 E2 29 00 00 80 20 00 00 .........)... ..
00000001400027A0 90 29 00 00 00 00 00 00 00 00 00 00 EC 2B 00 00 .)...........+..
00000001400027B0 60 21 00 00 F8 28 00 00 00 00 00 00 00 00 00 00 `!...(..........
00000001400027C0 0C 2C 00 00 C8 20 00 00 E8 28 00 00 00 00 00 00 .,... ...(......
00000001400027D0 00 00 00 00 2E 2C 00 00 B8 20 00 00 D8 28 00 00 .....,... ...(..
00000001400027E0 00 00 00 00 00 00 00 00 4E 2C 00 00 A8 20 00 00 ........N,... ..
00000001400027F0 C8 28 00 00 00 00 00 00 00 00 00 00 70 2C 00 00 .(..........p,..
0000000140002800 98 20 00 00 30 28 00 00 00 00 00 00 00 00 00 00 . ..0(..........
0000000140002810 EE 2D 00 00 00 20 00 00 00 00 00 00 00 00 00 00 .-... ..........
0000000140002820 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
l0000000140002830	dq	0x0000000000002CA4
l0000000140002838	dq	0x0000000000002CBE
l0000000140002840	dq	0x0000000000002CD2
l0000000140002848	dq	0x0000000000002DDA
l0000000140002850	dq	0x0000000000002DC6
l0000000140002858	dq	0x0000000000002DB0
l0000000140002860	dq	0x0000000000002D96
l0000000140002868	dq	0x0000000000002D80
l0000000140002870	dq	0x0000000000002D6A
l0000000140002878	dq	0x0000000000002D50
l0000000140002880	dq	0x0000000000002D34
l0000000140002888	dq	0x0000000000002D20
l0000000140002890	dq	0x0000000000002D0C
l0000000140002898	dq	0x0000000000002CEE
l00000001400028A0	dq	0x0000000000002C90
00000001400028A8                         00 00 00 00 00 00 00 00         ........
l00000001400028B0	dq	0x00000000000029D8
l00000001400028B8	dq	0x00000000000029C0
00000001400028C0 00 00 00 00 00 00 00 00                         ........       
l00000001400028C8	dq	0x0000000000002B7A
00000001400028D0 00 00 00 00 00 00 00 00                         ........       
l00000001400028D8	dq	0x0000000000002B64
00000001400028E0 00 00 00 00 00 00 00 00                         ........       
l00000001400028E8	dq	0x0000000000002A5C
00000001400028F0 00 00 00 00 00 00 00 00                         ........       
l00000001400028F8	dq	0x0000000000002B22
l0000000140002900	dq	0x0000000000002BB6
l0000000140002908	dq	0x0000000000002BD2
l0000000140002910	dq	0x0000000000002BE0
l0000000140002918	dq	0x0000000000002A4C
l0000000140002920	dq	0x0000000000002A3A
l0000000140002928	dq	0x0000000000002B36
l0000000140002930	dq	0x0000000000002B14
l0000000140002938	dq	0x0000000000002B06
l0000000140002940	dq	0x0000000000002B2C
l0000000140002948	dq	0x0000000000002AF0
l0000000140002950	dq	0x0000000000002AE8
l0000000140002958	dq	0x0000000000002ADA
l0000000140002960	dq	0x0000000000002ACE
l0000000140002968	dq	0x0000000000002AAC
l0000000140002970	dq	0x0000000000002A8A
l0000000140002978	dq	0x0000000000002A70
l0000000140002980	dq	0x0000000000002B9A
0000000140002988                         00 00 00 00 00 00 00 00         ........
l0000000140002990	dq	0x0000000000002B8A
l0000000140002998	dq	0x0000000000002A20
l00000001400029A0	dq	0x0000000000002A06
l00000001400029A8	dq	0x00000000000029F4
l00000001400029B0	dq	0x0000000000002AF8
00000001400029B8                         00 00 00 00 00 00 00 00         ........
00000001400029C0 08 00 5F 5F 43 5F 73 70 65 63 69 66 69 63 5F 68 ..__C_specific_h
00000001400029D0 61 6E 64 6C 65 72 00 00 3E 00 6D 65 6D 73 65 74 andler..>.memset
00000001400029E0 00 00 56 43 52 55 4E 54 49 4D 45 31 34 30 2E 64 ..VCRUNTIME140.d
00000001400029F0 6C 6C 00 00 00 00 5F 5F 61 63 72 74 5F 69 6F 62 ll....__acrt_iob
0000000140002A00 5F 66 75 6E 63 00 03 00 5F 5F 73 74 64 69 6F 5F _func...__stdio_
0000000140002A10 63 6F 6D 6D 6F 6E 5F 76 66 70 72 69 6E 74 66 00 common_vfprintf.
0000000140002A20 06 00 5F 5F 73 74 64 69 6F 5F 63 6F 6D 6D 6F 6E ..__stdio_common
0000000140002A30 5F 76 66 73 63 61 6E 66 00 00 40 00 5F 73 65 68 _vfscanf..@._seh
0000000140002A40 5F 66 69 6C 74 65 72 5F 65 78 65 00 42 00 5F 73 _filter_exe.B._s
0000000140002A50 65 74 5F 61 70 70 5F 74 79 70 65 00 09 00 5F 5F et_app_type...__
0000000140002A60 73 65 74 75 73 65 72 6D 61 74 68 65 72 72 00 00 setusermatherr..
0000000140002A70 18 00 5F 63 6F 6E 66 69 67 75 72 65 5F 6E 61 72 .._configure_nar
0000000140002A80 72 6F 77 5F 61 72 67 76 00 00 33 00 5F 69 6E 69 row_argv..3._ini
0000000140002A90 74 69 61 6C 69 7A 65 5F 6E 61 72 72 6F 77 5F 65 tialize_narrow_e
0000000140002AA0 6E 76 69 72 6F 6E 6D 65 6E 74 00 00 28 00 5F 67 nvironment..(._g
0000000140002AB0 65 74 5F 69 6E 69 74 69 61 6C 5F 6E 61 72 72 6F et_initial_narro
0000000140002AC0 77 5F 65 6E 76 69 72 6F 6E 6D 65 6E 74 00 36 00 w_environment.6.
0000000140002AD0 5F 69 6E 69 74 74 65 72 6D 00 37 00 5F 69 6E 69 _initterm.7._ini
0000000140002AE0 74 74 65 72 6D 5F 65 00 55 00 65 78 69 74 00 00 tterm_e.U.exit..
0000000140002AF0 23 00 5F 65 78 69 74 00 54 00 5F 73 65 74 5F 66 #._exit.T._set_f
0000000140002B00 6D 6F 64 65 00 00 04 00 5F 5F 70 5F 5F 5F 61 72 mode....__p___ar
0000000140002B10 67 63 00 00 05 00 5F 5F 70 5F 5F 5F 61 72 67 76 gc....__p___argv
0000000140002B20 00 00 16 00 5F 63 65 78 69 74 00 00 15 00 5F 63 ...._cexit...._c
0000000140002B30 5F 65 78 69 74 00 3D 00 5F 72 65 67 69 73 74 65 _exit.=._registe
0000000140002B40 72 5F 74 68 72 65 61 64 5F 6C 6F 63 61 6C 5F 65 r_thread_local_e
0000000140002B50 78 65 5F 61 74 65 78 69 74 5F 63 61 6C 6C 62 61 xe_atexit_callba
0000000140002B60 63 6B 00 00 08 00 5F 63 6F 6E 66 69 67 74 68 72 ck...._configthr
0000000140002B70 65 61 64 6C 6F 63 61 6C 65 00 16 00 5F 73 65 74 eadlocale..._set
0000000140002B80 5F 6E 65 77 5F 6D 6F 64 65 00 01 00 5F 5F 70 5F _new_mode...__p_
0000000140002B90 5F 63 6F 6D 6D 6F 64 65 00 00 34 00 5F 69 6E 69 _commode..4._ini
0000000140002BA0 74 69 61 6C 69 7A 65 5F 6F 6E 65 78 69 74 5F 74 tialize_onexit_t
0000000140002BB0 61 62 6C 65 00 00 3C 00 5F 72 65 67 69 73 74 65 able..<._registe
0000000140002BC0 72 5F 6F 6E 65 78 69 74 5F 66 75 6E 63 74 69 6F r_onexit_functio
0000000140002BD0 6E 00 1E 00 5F 63 72 74 5F 61 74 65 78 69 74 00 n..._crt_atexit.
0000000140002BE0 67 00 74 65 72 6D 69 6E 61 74 65 00 61 70 69 2D g.terminate.api-
0000000140002BF0 6D 73 2D 77 69 6E 2D 63 72 74 2D 73 74 64 69 6F ms-win-crt-stdio
0000000140002C00 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 61 70 69 2D -l1-1-0.dll.api-
0000000140002C10 6D 73 2D 77 69 6E 2D 63 72 74 2D 72 75 6E 74 69 ms-win-crt-runti
0000000140002C20 6D 65 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 61 70 me-l1-1-0.dll.ap
0000000140002C30 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 2D 6D 61 74 i-ms-win-crt-mat
0000000140002C40 68 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 00 61 70 h-l1-1-0.dll..ap
0000000140002C50 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 2D 6C 6F 63 i-ms-win-crt-loc
0000000140002C60 61 6C 65 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 00 ale-l1-1-0.dll..
0000000140002C70 61 70 69 2D 6D 73 2D 77 69 6E 2D 63 72 74 2D 68 api-ms-win-crt-h
0000000140002C80 65 61 70 2D 6C 31 2D 31 2D 30 2E 64 6C 6C 00 00 eap-l1-1-0.dll..
0000000140002C90 AE 04 52 74 6C 43 61 70 74 75 72 65 43 6F 6E 74 ..RtlCaptureCont
0000000140002CA0 65 78 74 00 B5 04 52 74 6C 4C 6F 6F 6B 75 70 46 ext...RtlLookupF
0000000140002CB0 75 6E 63 74 69 6F 6E 45 6E 74 72 79 00 00 BC 04 unctionEntry....
0000000140002CC0 52 74 6C 56 69 72 74 75 61 6C 55 6E 77 69 6E 64 RtlVirtualUnwind
0000000140002CD0 00 00 92 05 55 6E 68 61 6E 64 6C 65 64 45 78 63 ....UnhandledExc
0000000140002CE0 65 70 74 69 6F 6E 46 69 6C 74 65 72 00 00 52 05 eptionFilter..R.
0000000140002CF0 53 65 74 55 6E 68 61 6E 64 6C 65 64 45 78 63 65 SetUnhandledExce
0000000140002D00 70 74 69 6F 6E 46 69 6C 74 65 72 00 0F 02 47 65 ptionFilter...Ge
0000000140002D10 74 43 75 72 72 65 6E 74 50 72 6F 63 65 73 73 00 tCurrentProcess.
0000000140002D20 70 05 54 65 72 6D 69 6E 61 74 65 50 72 6F 63 65 p.TerminateProce
0000000140002D30 73 73 00 00 70 03 49 73 50 72 6F 63 65 73 73 6F ss..p.IsProcesso
0000000140002D40 72 46 65 61 74 75 72 65 50 72 65 73 65 6E 74 00 rFeaturePresent.
0000000140002D50 30 04 51 75 65 72 79 50 65 72 66 6F 72 6D 61 6E 0.QueryPerforman
0000000140002D60 63 65 43 6F 75 6E 74 65 72 00 10 02 47 65 74 43 ceCounter...GetC
0000000140002D70 75 72 72 65 6E 74 50 72 6F 63 65 73 73 49 64 00 urrentProcessId.
0000000140002D80 14 02 47 65 74 43 75 72 72 65 6E 74 54 68 72 65 ..GetCurrentThre
0000000140002D90 61 64 49 64 00 00 DD 02 47 65 74 53 79 73 74 65 adId....GetSyste
0000000140002DA0 6D 54 69 6D 65 41 73 46 69 6C 65 54 69 6D 65 00 mTimeAsFileTime.
0000000140002DB0 54 03 49 6E 69 74 69 61 6C 69 7A 65 53 4C 69 73 T.InitializeSLis
0000000140002DC0 74 48 65 61 64 00 6A 03 49 73 44 65 62 75 67 67 tHead.j.IsDebugg
0000000140002DD0 65 72 50 72 65 73 65 6E 74 00 6D 02 47 65 74 4D erPresent.m.GetM
0000000140002DE0 6F 64 75 6C 65 48 61 6E 64 6C 65 57 00 00 4B 45 oduleHandleW..KE
0000000140002DF0 52 4E 45 4C 33 32 2E 64 6C 6C 00 00             RNEL32.dll..   
;;; Segment .data (0000000140003000)
0000000140003000 32 A2 DF 2D 99 2B 00 00 CD 5D 20 D2 66 D4 FF FF 2..-.+...] .f...
0000000140003010 FF FF FF FF 01 00 00 00 01 00 00 00 02 00 00 00 ................
0000000140003020 2F 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 / ..............
0000000140003030 01 00 00 00 75 98 00 00 00 00 00 00 00 00 00 00 ....u...........
0000000140003040 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
;;; Segment .pdata (0000000140004000)
0000000140004000 00 10 00 00 B1 10 00 00 98 26 00 00 D0 10 00 00 .........&......
0000000140004010 22 11 00 00 A8 26 00 00 40 11 00 00 95 11 00 00 "....&..@.......
0000000140004020 A8 26 00 00 B0 11 00 00 D1 11 00 00 B8 26 00 00 .&...........&..
0000000140004030 D4 11 00 00 8F 12 00 00 28 27 00 00 90 12 00 00 ........('......
0000000140004040 A0 12 00 00 84 27 00 00 A0 12 00 00 B9 12 00 00 .....'..........
0000000140004050 84 27 00 00 BC 12 00 00 33 14 00 00 BC 26 00 00 .'......3....&..
0000000140004060 34 14 00 00 46 14 00 00 84 27 00 00 48 14 00 00 4...F....'..H...
0000000140004070 7C 14 00 00 28 27 00 00 7C 14 00 00 4D 15 00 00 |...('..|...M...
0000000140004080 EC 26 00 00 50 15 00 00 C1 15 00 00 F4 26 00 00 .&..P........&..
0000000140004090 C4 15 00 00 FD 15 00 00 84 27 00 00 00 16 00 00 .........'......
00000001400040A0 49 16 00 00 28 27 00 00 4C 16 00 00 15 17 00 00 I...('..L.......
00000001400040B0 30 27 00 00 18 17 00 00 B1 17 00 00 00 27 00 00 0'...........'..
00000001400040C0 B4 17 00 00 D8 17 00 00 28 27 00 00 D8 17 00 00 ........('......
00000001400040D0 03 18 00 00 28 27 00 00 04 18 00 00 53 18 00 00 ....('......S...
00000001400040E0 28 27 00 00 54 18 00 00 6B 18 00 00 84 27 00 00 ('..T...k....'..
00000001400040F0 6C 18 00 00 18 19 00 00 3C 27 00 00 3C 19 00 00 l.......<'..<...
0000000140004100 57 19 00 00 84 27 00 00 74 19 00 00 B9 1A 00 00 W....'..t.......
0000000140004110 48 27 00 00 C0 1A 00 00 14 1B 00 00 84 27 00 00 H'...........'..
0000000140004120 24 1B 00 00 5C 1B 00 00 84 27 00 00 5C 1B 00 00 $...\....'..\...
0000000140004130 A6 1B 00 00 58 27 00 00 A8 1B 00 00 F2 1B 00 00 ....X'..........
0000000140004140 58 27 00 00 FC 1B 00 00 C2 1D 00 00 68 27 00 00 X'..........h'..
0000000140004150 7C 1E 00 00 99 1E 00 00 84 27 00 00 9C 1E 00 00 |........'......
0000000140004160 F5 1E 00 00 7C 27 00 00 10 1F 00 00 12 1F 00 00 ....|'..........
0000000140004170 78 27 00 00 12 1F 00 00 30 1F 00 00 E4 26 00 00 x'......0....&..
0000000140004180 30 1F 00 00 48 1F 00 00 20 27 00 00             0...H... '..   
;;; Segment .gfids (0000000140005000)
0000000140005000 0A 00 00 00 0D 00 00 00 10 00 00 00 45 00 00 00 ............E...
0000000140005010 72 00 00 00 37 00 00 00 0B 00 00 00             r...7.......   
;;; Segment .rsrc (0000000140006000)
0000000140006000 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 ................
0000000140006010 18 00 00 00 18 00 00 80 00 00 00 00 00 00 00 00 ................
0000000140006020 00 00 00 00 00 00 01 00 01 00 00 00 30 00 00 80 ............0...
0000000140006030 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 ................
0000000140006040 09 04 00 00 48 00 00 00 60 60 00 00 7D 01 00 00 ....H...``..}...
0000000140006050 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000000140006060 3C 3F 78 6D 6C 20 76 65 72 73 69 6F 6E 3D 27 31 <?xml version='1
0000000140006070 2E 30 27 20 65 6E 63 6F 64 69 6E 67 3D 27 55 54 .0' encoding='UT
0000000140006080 46 2D 38 27 20 73 74 61 6E 64 61 6C 6F 6E 65 3D F-8' standalone=
0000000140006090 27 79 65 73 27 3F 3E 0D 0A 3C 61 73 73 65 6D 62 'yes'?>..<assemb
00000001400060A0 6C 79 20 78 6D 6C 6E 73 3D 27 75 72 6E 3A 73 63 ly xmlns='urn:sc
00000001400060B0 68 65 6D 61 73 2D 6D 69 63 72 6F 73 6F 66 74 2D hemas-microsoft-
00000001400060C0 63 6F 6D 3A 61 73 6D 2E 76 31 27 20 6D 61 6E 69 com:asm.v1' mani
00000001400060D0 66 65 73 74 56 65 72 73 69 6F 6E 3D 27 31 2E 30 festVersion='1.0
00000001400060E0 27 3E 0D 0A 20 20 3C 74 72 75 73 74 49 6E 66 6F '>..  <trustInfo
00000001400060F0 20 78 6D 6C 6E 73 3D 22 75 72 6E 3A 73 63 68 65  xmlns="urn:sche
0000000140006100 6D 61 73 2D 6D 69 63 72 6F 73 6F 66 74 2D 63 6F mas-microsoft-co
0000000140006110 6D 3A 61 73 6D 2E 76 33 22 3E 0D 0A 20 20 20 20 m:asm.v3">..    
0000000140006120 3C 73 65 63 75 72 69 74 79 3E 0D 0A 20 20 20 20 <security>..    
0000000140006130 20 20 3C 72 65 71 75 65 73 74 65 64 50 72 69 76   <requestedPriv
0000000140006140 69 6C 65 67 65 73 3E 0D 0A 20 20 20 20 20 20 20 ileges>..       
0000000140006150 20 3C 72 65 71 75 65 73 74 65 64 45 78 65 63 75  <requestedExecu
0000000140006160 74 69 6F 6E 4C 65 76 65 6C 20 6C 65 76 65 6C 3D tionLevel level=
0000000140006170 27 61 73 49 6E 76 6F 6B 65 72 27 20 75 69 41 63 'asInvoker' uiAc
0000000140006180 63 65 73 73 3D 27 66 61 6C 73 65 27 20 2F 3E 0D cess='false' />.
0000000140006190 0A 20 20 20 20 20 20 3C 2F 72 65 71 75 65 73 74 .      </request
00000001400061A0 65 64 50 72 69 76 69 6C 65 67 65 73 3E 0D 0A 20 edPrivileges>.. 
00000001400061B0 20 20 20 3C 2F 73 65 63 75 72 69 74 79 3E 0D 0A    </security>..
00000001400061C0 20 20 3C 2F 74 72 75 73 74 49 6E 66 6F 3E 0D 0A   </trustInfo>..
00000001400061D0 3C 2F 61 73 73 65 6D 62 6C 79 3E 0D 0A 00 00 00 </assembly>.....
;;; Segment .reloc (0000000140007000)
0000000140007000 00 20 00 00 1C 00 00 00 90 A1 98 A1 A8 A1 C0 A1 . ..............
0000000140007010 C8 A1 00 A2 08 A2 38 A3 50 A3 58 A3             ......8.P.X.   
