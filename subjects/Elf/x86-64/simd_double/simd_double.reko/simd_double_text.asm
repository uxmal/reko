;;; Segment .text (0000000000000620)

;; _start: 0000000000000620
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,0F0h
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
	jz	0680h

l0000000000000667:
	mov	rax,[0000000000200FD8]                                 ; [rip+0020096A]
	test	rax,rax
	jz	0680h

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
	sar	rsi,3h
	mov	rax,rsi
	shr	rax,3Fh
	add	rsi,rax
	sar	rsi,1h
	jz	06D0h

l00000000000006B8:
	mov	rax,[0000000000200FF0]                                 ; [rip+00200931]
	test	rax,rax
	jz	06D0h

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
	cmp	[0000000000201048],0h                                  ; [rip+00200961]
	jnz	0718h

l00000000000006E9:
	cmp	[0000000000200FF8],0h                                  ; [rip+00200907]
	push	rbp
	mov	rbp,rsp
	jz	0703h

l00000000000006F7:
	mov	rdi,[0000000000201040]                                 ; [rip+00200942]
	call	0610h

l0000000000000703:
	call	0650h
	mov	[0000000000201048],1h                                  ; [rip+00200939]
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
	jmp	0690h

;; _mm_malloc: 000000000000072A
;;   Called from:
;;     00000000000008B8 (in main)
;;     00000000000008D1 (in main)
;;     00000000000008EA (in main)
_mm_malloc proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,20h
	mov	[rbp-18h],rdi
	mov	[rbp-20h],rsi
	cmp	qword ptr [rbp-20h],1h
	jnz	074Fh

l0000000000000741:
	mov	rax,[rbp-18h]
	mov	rdi,rax
	call	05F0h
	jmp	078Bh

l000000000000074F:
	cmp	qword ptr [rbp-20h],2h
	jz	075Dh

l0000000000000756:
	cmp	qword ptr [rbp-20h],4h
	jnz	0765h

l000000000000075D:
	mov	qword ptr [rbp-20h],+8h

l0000000000000765:
	mov	rdx,[rbp-18h]
	mov	rcx,[rbp-20h]
	lea	rax,[rbp-8h]
	mov	rsi,rcx
	mov	rdi,rax
	call	0600h
	test	eax,eax
	jnz	0786h

l0000000000000780:
	mov	rax,[rbp-8h]
	jmp	078Bh

l0000000000000786:
	mov	eax,0h

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
	sub	rsp,10h
	mov	[rbp-8h],rdi
	mov	rax,[rbp-8h]
	mov	rdi,rax
	call	05D0h
	nop
	leave
	ret

;; vec_add: 00000000000007A8
;;   Called from:
;;     00000000000009E9 (in main)
vec_add proc
	lea	r10,[rsp+8h]
	and	rsp,0E0h
	push	qword ptr [r10-8h]
	push	rbp
	mov	rbp,rsp
	push	r10
	sub	rsp,30h
	mov	[rbp-98h],rdi
	mov	[rbp-0A0h],rsi
	mov	[rbp-0A8h],rdx
	mov	[rbp-0B0h],rcx
	mov	rcx,[0000000000000B00]                                 ; [rip+0000031E]
	mov	rax,[rbp-98h]
	mov	edx,0h
	div	rcx
	mov	[rbp-20h],rax
	mov	rax,[rbp-0A0h]
	mov	[rbp-28h],rax
	mov	rax,[rbp-0A8h]
	mov	[rbp-30h],rax
	mov	rax,[rbp-0B0h]
	mov	[rbp-38h],rax
	mov	qword ptr [rbp-18h],+0h
	jmp	0881h

l0000000000000820:
	mov	rax,[rbp-18h]
	shl	rax,5h
	mov	rdx,rax
	mov	rax,[rbp-38h]
	add	rax,rdx
	vmovapd	ymm0,[rax]
	mov	rax,[rbp-18h]
	shl	rax,5h
	mov	rdx,rax
	mov	rax,[rbp-30h]
	add	rax,rdx
	vmovapd	ymm1,[rax]
	mov	rax,[rbp-18h]
	shl	rax,5h
	mov	rdx,rax
	mov	rax,[rbp-28h]
	add	rax,rdx
	vmovapd	[rbp-70h],ymm1
	vmovapd	[rbp-90h],ymm0
	vmovapd	ymm0,[rbp-70h]
	vaddpd	ymm0,ymm0,[rbp-90h]
	vmovapd	[rax],ymm0
	add	qword ptr [rbp-18h],1h

l0000000000000881:
	mov	rax,[rbp-18h]
	cmp	rax,[rbp-20h]
	jc	0820h

l000000000000088B:
	nop
	add	rsp,30h
	pop	r10
	pop	rbp
	lea	rsp,[r10-8h]
	ret

;; main: 0000000000000898
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,50h
	mov	qword ptr [rbp-28h],+400h
	mov	rax,[rbp-28h]
	shl	rax,3h
	mov	esi,20h
	mov	rdi,rax
	call	072Ah
	mov	[rbp-30h],rax
	mov	rax,[rbp-28h]
	shl	rax,3h
	mov	esi,20h
	mov	rdi,rax
	call	072Ah
	mov	[rbp-38h],rax
	mov	rax,[rbp-28h]
	shl	rax,3h
	mov	esi,20h
	mov	rdi,rax
	call	072Ah
	mov	[rbp-40h],rax
	mov	qword ptr [rbp-8h],+0h
	jmp	093Fh

l00000000000008FD:
	mov	rax,[rbp-8h]
	lea	rdx,[0000h+rax*8]
	mov	rax,[rbp-30h]
	lea	rcx,[rdx+rax]
	mov	rax,[rbp-8h]
	test	rax,rax
	js	0921h

l000000000000091A:
	vcvtsi2sd	xmm0,xmm0,rax
	jmp	0936h

l0000000000000921:
	mov	rdx,rax
	shr	rdx,1h
	and	eax,1h
	or	rdx,rax
	vcvtsi2sd	xmm0,xmm0,rdx
	vaddsd	xmm0,xmm0,xmm0

l0000000000000936:
	vmovsd	double ptr [rcx],xmm0
	add	qword ptr [rbp-8h],1h

l000000000000093F:
	mov	rax,[rbp-8h]
	cmp	rax,[rbp-28h]
	jc	08FDh

l0000000000000949:
	mov	qword ptr [rbp-10h],+0h
	jmp	0998h

l0000000000000953:
	mov	rax,[rbp-10h]
	add	rax,1h
	mov	rdx,[rbp-10h]
	lea	rcx,[0000h+rdx*8]
	mov	rdx,[rbp-38h]
	add	rcx,rdx
	test	rax,rax
	js	097Ah

l0000000000000973:
	vcvtsi2sd	xmm0,xmm0,rax
	jmp	098Fh

l000000000000097A:
	mov	rdx,rax
	shr	rdx,1h
	and	eax,1h
	or	rdx,rax
	vcvtsi2sd	xmm0,xmm0,rdx
	vaddsd	xmm0,xmm0,xmm0

l000000000000098F:
	vmovsd	double ptr [rcx],xmm0
	add	qword ptr [rbp-10h],1h

l0000000000000998:
	mov	rax,[rbp-10h]
	cmp	rax,[rbp-28h]
	jc	0953h

l00000000000009A2:
	mov	qword ptr [rbp-18h],+0h
	jmp	09CCh

l00000000000009AC:
	mov	rax,[rbp-18h]
	lea	rdx,[0000h+rax*8]
	mov	rax,[rbp-40h]
	add	rax,rdx
	vxorpd	xmm0,xmm0,xmm0
	vmovsd	double ptr [rax],xmm0
	add	qword ptr [rbp-18h],1h

l00000000000009CC:
	mov	rax,[rbp-18h]
	cmp	rax,[rbp-28h]
	jc	09ACh

l00000000000009D6:
	mov	rcx,[rbp-38h]
	mov	rdx,[rbp-30h]
	mov	rsi,[rbp-40h]
	mov	rax,[rbp-28h]
	mov	rdi,rax
	call	07A8h
	mov	qword ptr [rbp-20h],+0h
	jmp	0A2Dh

l00000000000009F8:
	mov	rax,[rbp-20h]
	lea	rdx,[0000h+rax*8]
	mov	rax,[rbp-40h]
	add	rax,rdx
	mov	rax,[rax]
	mov	[rbp-48h],rax
	vmovsd	xmm0,double ptr [rbp-48h]
	lea	rdi,[0000000000000AF8]                                 ; [rip+000000DA]
	mov	eax,1h
	call	05E0h
	add	qword ptr [rbp-20h],1h

l0000000000000A2D:
	mov	rax,[rbp-20h]
	cmp	rax,[rbp-28h]
	jc	09F8h

l0000000000000A37:
	mov	rax,[rbp-30h]
	mov	rdi,rax
	call	078Dh
	mov	rax,[rbp-38h]
	mov	rdi,rax
	call	078Dh
	mov	rax,[rbp-40h]
	mov	rdi,rax
	call	078Dh
	mov	eax,0h
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
	sub	rsp,8h
	sar	rbp,3h
	call	05A0h
	test	rbp,rbp
	jz	0AC6h

l0000000000000AA6:
	xor	ebx,ebx
	nop	dword ptr [rax+rax+0h]

l0000000000000AB0:
	mov	rdx,r13
	mov	rsi,r14
	mov	edi,r15d
	call	qword ptr [r12+rbx*8]
	add	rbx,1h
	cmp	rbp,rbx
	jnz	0AB0h

l0000000000000AC6:
	add	rsp,8h
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
