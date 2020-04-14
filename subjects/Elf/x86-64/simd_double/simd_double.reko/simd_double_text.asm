;;; Segment .text (0000000000000620)

;; _start: 0000000000000620
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,F0
	push	rax
	push	rsp
	lea	r8,[0000000000000AE0]                                  ; [rip+000004AA]
	lea	rcx,[0000000000000A70]                                 ; [rip+00000433]
	lea	rdi,[0000000000000898]                                 ; [rip+00000254]
	call	[0000000000200FE0]                                    ; [rip+00200996]
	hlt
000000000000064B                                  0F 1F 44 00 00            ..D..

;; deregister_tm_clones: 0000000000000650
;;   Called from:
;;     0000000000000703 (in __do_global_dtors_aux)
deregister_tm_clones proc
	lea	rdi,[0000000000201048]                                 ; [rip+002009F1]
	push	rbp
	lea	rax,[0000000000201048]                                 ; [rip+002009E9]
	cmp	rax,rdi
	mov	rbp,rsp
	jz	0000000000000680

l0000000000000667:
	mov	rax,[0000000000200FD8]                                 ; [rip+0020096A]
	test	rax,rax
	jz	0000000000000680

l0000000000000673:
	pop	rbp
	jmp	rax
0000000000000676                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000000680:
	pop	rbp
	ret
0000000000000682       0F 1F 40 00 66 2E 0F 1F 84 00 00 00 00 00   ..@.f.........

;; register_tm_clones: 0000000000000690
;;   Called from:
;;     0000000000000725 (in frame_dummy)
register_tm_clones proc
	lea	rdi,[0000000000201048]                                 ; [rip+002009B1]
	lea	rsi,[0000000000201048]                                 ; [rip+002009AA]
	push	rbp
	sub	rsi,rdi
	mov	rbp,rsp
	sar	rsi,03
	mov	rax,rsi
	shr	rax,3F
	add	rsi,rax
	sar	rsi,01
	jz	00000000000006D0

l00000000000006B8:
	mov	rax,[0000000000200FF0]                                 ; [rip+00200931]
	test	rax,rax
	jz	00000000000006D0

l00000000000006C4:
	pop	rbp
	jmp	rax
00000000000006C7                      66 0F 1F 84 00 00 00 00 00        f........

l00000000000006D0:
	pop	rbp
	ret
00000000000006D2       0F 1F 40 00 66 2E 0F 1F 84 00 00 00 00 00   ..@.f.........

;; __do_global_dtors_aux: 00000000000006E0
__do_global_dtors_aux proc
	cmp	[0000000000201048],00                                  ; [rip+00200961]
	jnz	0000000000000718

l00000000000006E9:
	cmp	[0000000000200FF8],00                                  ; [rip+00200907]
	push	rbp
	mov	rbp,rsp
	jz	0000000000000703

l00000000000006F7:
	mov	rdi,[0000000000201040]                                 ; [rip+00200942]
	call	0000000000000610

l0000000000000703:
	call	0000000000000650
	mov	[0000000000201048],01                                  ; [rip+00200939]
	pop	rbp
	ret
0000000000000711    0F 1F 80 00 00 00 00                          .......       

l0000000000000718:
	ret
000000000000071A                               66 0F 1F 44 00 00           f..D..

;; frame_dummy: 0000000000000720
frame_dummy proc
	push	rbp
	mov	rbp,rsp
	pop	rbp
	jmp	0000000000000690

;; _mm_malloc: 000000000000072A
;;   Called from:
;;     00000000000008B8 (in main)
;;     00000000000008D1 (in main)
;;     00000000000008EA (in main)
_mm_malloc proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,20
	mov	[rbp-18],rdi
	mov	[rbp-20],rsi
	cmp	qword ptr [rbp-20],01
	jnz	000000000000074F

l0000000000000741:
	mov	rax,[rbp-18]
	mov	rdi,rax
	call	00000000000005F0
	jmp	000000000000078B

l000000000000074F:
	cmp	qword ptr [rbp-20],02
	jz	000000000000075D

l0000000000000756:
	cmp	qword ptr [rbp-20],04
	jnz	0000000000000765

l000000000000075D:
	mov	qword ptr [rbp-20],+00000008

l0000000000000765:
	mov	rdx,[rbp-18]
	mov	rcx,[rbp-20]
	lea	rax,[rbp-08]
	mov	rsi,rcx
	mov	rdi,rax
	call	0000000000000600
	test	eax,eax
	jnz	0000000000000786

l0000000000000780:
	mov	rax,[rbp-08]
	jmp	000000000000078B

l0000000000000786:
	mov	eax,00000000

l000000000000078B:
	leave
	ret

;; _mm_free: 000000000000078D
;;   Called from:
;;     0000000000000A3E (in main)
;;     0000000000000A4A (in main)
;;     0000000000000A56 (in main)
_mm_free proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,10
	mov	[rbp-08],rdi
	mov	rax,[rbp-08]
	mov	rdi,rax
	call	00000000000005D0
	nop
	leave
	ret

;; vec_add: 00000000000007A8
;;   Called from:
;;     00000000000009E9 (in main)
vec_add proc
	lea	r10,[rsp+08]
	and	rsp,E0
	push	qword ptr [r10-08]
	push	rbp
	mov	rbp,rsp
	push	r10
	sub	rsp,30
	mov	[rbp-00000098],rdi
	mov	[rbp-000000A0],rsi
	mov	[rbp-000000A8],rdx
	mov	[rbp-000000B0],rcx
	mov	rcx,[0000000000000B00]                                 ; [rip+0000031E]
	mov	rax,[rbp-00000098]
	mov	edx,00000000
	div	rcx
	mov	[rbp-20],rax
	mov	rax,[rbp-000000A0]
	mov	[rbp-28],rax
	mov	rax,[rbp-000000A8]
	mov	[rbp-30],rax
	mov	rax,[rbp-000000B0]
	mov	[rbp-38],rax
	mov	qword ptr [rbp-18],+00000000
	jmp	0000000000000881

l0000000000000820:
	mov	rax,[rbp-18]
	shl	rax,05
	mov	rdx,rax
	mov	rax,[rbp-38]
	add	rax,rdx
	vmovapd	ymm0,[rax]
	mov	rax,[rbp-18]
	shl	rax,05
	mov	rdx,rax
	mov	rax,[rbp-30]
	add	rax,rdx
	vmovapd	ymm1,[rax]
	mov	rax,[rbp-18]
	shl	rax,05
	mov	rdx,rax
	mov	rax,[rbp-28]
	add	rax,rdx
	vmovapd	[rbp-70],ymm1
	vmovapd	[rbp-00000090],ymm0
	vmovapd	ymm0,[rbp-70]
	vaddpd	ymm0,ymm0,[rbp-00000090]
	vmovapd	[rax],ymm0
	add	qword ptr [rbp-18],01

l0000000000000881:
	mov	rax,[rbp-18]
	cmp	rax,[rbp-20]
	jc	0000000000000820

l000000000000088B:
	nop
	add	rsp,30
	pop	r10
	pop	rbp
	lea	rsp,[r10-08]
	ret

;; main: 0000000000000898
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,50
	mov	qword ptr [rbp-28],+00000400
	mov	rax,[rbp-28]
	shl	rax,03
	mov	esi,00000020
	mov	rdi,rax
	call	000000000000072A
	mov	[rbp-30],rax
	mov	rax,[rbp-28]
	shl	rax,03
	mov	esi,00000020
	mov	rdi,rax
	call	000000000000072A
	mov	[rbp-38],rax
	mov	rax,[rbp-28]
	shl	rax,03
	mov	esi,00000020
	mov	rdi,rax
	call	000000000000072A
	mov	[rbp-40],rax
	mov	qword ptr [rbp-08],+00000000
	jmp	000000000000093F

l00000000000008FD:
	mov	rax,[rbp-08]
	lea	rdx,[00000000+rax*8]
	mov	rax,[rbp-30]
	lea	rcx,[rdx+rax]
	mov	rax,[rbp-08]
	test	rax,rax
	js	0000000000000921

l000000000000091A:
	vcvtsi2sd	xmm0,xmm0,rax
	jmp	0000000000000936

l0000000000000921:
	mov	rdx,rax
	shr	rdx,01
	and	eax,01
	or	rdx,rax
	vcvtsi2sd	xmm0,xmm0,rdx
	vaddsd	xmm0,xmm0,xmm0

l0000000000000936:
	vmovsd	double ptr [rcx],xmm0
	add	qword ptr [rbp-08],01

l000000000000093F:
	mov	rax,[rbp-08]
	cmp	rax,[rbp-28]
	jc	00000000000008FD

l0000000000000949:
	mov	qword ptr [rbp-10],+00000000
	jmp	0000000000000998

l0000000000000953:
	mov	rax,[rbp-10]
	add	rax,01
	mov	rdx,[rbp-10]
	lea	rcx,[00000000+rdx*8]
	mov	rdx,[rbp-38]
	add	rcx,rdx
	test	rax,rax
	js	000000000000097A

l0000000000000973:
	vcvtsi2sd	xmm0,xmm0,rax
	jmp	000000000000098F

l000000000000097A:
	mov	rdx,rax
	shr	rdx,01
	and	eax,01
	or	rdx,rax
	vcvtsi2sd	xmm0,xmm0,rdx
	vaddsd	xmm0,xmm0,xmm0

l000000000000098F:
	vmovsd	double ptr [rcx],xmm0
	add	qword ptr [rbp-10],01

l0000000000000998:
	mov	rax,[rbp-10]
	cmp	rax,[rbp-28]
	jc	0000000000000953

l00000000000009A2:
	mov	qword ptr [rbp-18],+00000000
	jmp	00000000000009CC

l00000000000009AC:
	mov	rax,[rbp-18]
	lea	rdx,[00000000+rax*8]
	mov	rax,[rbp-40]
	add	rax,rdx
	vxorpd	xmm0,xmm0,xmm0
	vmovsd	double ptr [rax],xmm0
	add	qword ptr [rbp-18],01

l00000000000009CC:
	mov	rax,[rbp-18]
	cmp	rax,[rbp-28]
	jc	00000000000009AC

l00000000000009D6:
	mov	rcx,[rbp-38]
	mov	rdx,[rbp-30]
	mov	rsi,[rbp-40]
	mov	rax,[rbp-28]
	mov	rdi,rax
	call	00000000000007A8
	mov	qword ptr [rbp-20],+00000000
	jmp	0000000000000A2D

l00000000000009F8:
	mov	rax,[rbp-20]
	lea	rdx,[00000000+rax*8]
	mov	rax,[rbp-40]
	add	rax,rdx
	mov	rax,[rax]
	mov	[rbp-48],rax
	vmovsd	xmm0,double ptr [rbp-48]
	lea	rdi,[0000000000000AF8]                                 ; [rip+000000DA]
	mov	eax,00000001
	call	00000000000005E0
	add	qword ptr [rbp-20],01

l0000000000000A2D:
	mov	rax,[rbp-20]
	cmp	rax,[rbp-28]
	jc	00000000000009F8

l0000000000000A37:
	mov	rax,[rbp-30]
	mov	rdi,rax
	call	000000000000078D
	mov	rax,[rbp-38]
	mov	rdi,rax
	call	000000000000078D
	mov	rax,[rbp-40]
	mov	rdi,rax
	call	000000000000078D
	mov	eax,00000000
	leave
	ret
0000000000000A62       66 2E 0F 1F 84 00 00 00 00 00 0F 1F 40 00   f...........@.

;; __libc_csu_init: 0000000000000A70
__libc_csu_init proc
	push	r15
	push	r14
	mov	r15d,edi
	push	r13
	push	r12
	lea	r12,[0000000000200DE8]                                 ; [rip+00200366]
	push	rbp
	lea	rbp,[0000000000200DF0]                                 ; [rip+00200366]
	push	rbx
	mov	r14,rsi
	mov	r13,rdx
	sub	rbp,r12
	sub	rsp,08
	sar	rbp,03
	call	00000000000005A0
	test	rbp,rbp
	jz	0000000000000AC6

l0000000000000AA6:
	xor	ebx,ebx
	nop	dword ptr [rax+rax+00000000]

l0000000000000AB0:
	mov	rdx,r13
	mov	rsi,r14
	mov	edi,r15d
	call	qword ptr [r12+rbx*8]
	add	rbx,01
	cmp	rbp,rbx
	jnz	0000000000000AB0

l0000000000000AC6:
	add	rsp,08
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
0000000000000AD5                90 66 2E 0F 1F 84 00 00 00 00 00      .f.........

;; __libc_csu_fini: 0000000000000AE0
__libc_csu_fini proc
	ret
