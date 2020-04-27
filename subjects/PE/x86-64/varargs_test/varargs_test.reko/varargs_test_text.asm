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
	movsxd	rax,[000000014000003C]                              ; [rip-000016F8]
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
	movsxd	rax,dword ptr [rcx+3C]
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
	movsxd	r10,dword ptr [r8+04]
	neg	eax
	add	r10,rcx
	movsxd	rcx,eax
	and	r10,rcx

l0000000140001EC7:
	movsxd	rax,r11d
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
