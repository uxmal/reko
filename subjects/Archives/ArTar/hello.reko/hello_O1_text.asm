;;; Segment .text (0000000000001080)

;; _start: 0000000000001080
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,0F0h
	push	rax
	push	rsp
	lea	r8,[__libc_csu_fini]                                   ; [rip+0000024A]
	lea	rcx,[__libc_csu_init]                                  ; [rip+000001E3]
	lea	rdi,[main]                                             ; [rip+0000013C]
	call	[0000000000003FE0]                                    ; [rip+00002F36]
	hlt
00000000000010AB                                  0F 1F 44 00 00            ..D..

;; deregister_tm_clones: 00000000000010B0
;;   Called from:
;;     0000000000001147 (in __do_global_dtors_aux)
deregister_tm_clones proc
	lea	rdi,[0000000000004048]                                 ; [rip+00002F91]
	lea	rax,[0000000000004048]                                 ; [rip+00002F8A]
	cmp	rax,rdi
	jz	10D8h

l00000000000010C3:
	mov	rax,[0000000000003FD8]                                 ; [rip+00002F0E]
	test	rax,rax
	jz	10D8h

l00000000000010CF:
	jmp	rax
00000000000010D1    0F 1F 80 00 00 00 00                          .......        

l00000000000010D8:
	ret
00000000000010D9                            0F 1F 80 00 00 00 00          .......

;; register_tm_clones: 00000000000010E0
;;   Called from:
;;     0000000000001160 (in frame_dummy)
register_tm_clones proc
	lea	rdi,[0000000000004048]                                 ; [rip+00002F61]
	lea	rsi,[0000000000004048]                                 ; [rip+00002F5A]
	sub	rsi,rdi
	mov	rax,rsi
	shr	rsi,3Fh
	sar	rax,3h
	add	rsi,rax
	sar	rsi,1h
	jz	1118h

l0000000000001104:
	mov	rax,[0000000000003FF0]                                 ; [rip+00002EE5]
	test	rax,rax
	jz	1118h

l0000000000001110:
	jmp	rax
0000000000001112       66 0F 1F 44 00 00                           f..D..        

l0000000000001118:
	ret
0000000000001119                            0F 1F 80 00 00 00 00          .......

;; __do_global_dtors_aux: 0000000000001120
__do_global_dtors_aux proc
	endbr64
	cmp	[0000000000004048],0h                                  ; [rip+00002F1D]
	jnz	1158h

l000000000000112D:
	push	rbp
	cmp	[0000000000003FF8],0h                                  ; [rip+00002EC2]
	mov	rbp,rsp
	jz	1147h

l000000000000113B:
	mov	rdi,[0000000000004040]                                 ; [rip+00002EFE]
	call	1070h

l0000000000001147:
	call	deregister_tm_clones
	mov	[0000000000004048],1h                                  ; [rip+00002EF5]
	pop	rbp
	ret
0000000000001155                0F 1F 00                              ...        

l0000000000001158:
	ret
0000000000001159                            0F 1F 80 00 00 00 00          .......

;; frame_dummy: 0000000000001160
;;   Called from:
;;     00000000000012BE (in __libc_csu_init)
frame_dummy proc
	endbr64
	jmp	register_tm_clones

;; Q_rsqrt: 0000000000001169
;;   Called from:
;;     000000000000121D (in main)
Q_rsqrt proc
	movss	dword ptr [rsp-4h],xmm0
	mov	rdx,[rsp-4h]
	sar	rdx,1h
	mov	eax,5F3759DFh
	sub	eax,edx
	movd	xmm2,eax
	mulss	xmm0,[0000000000002074]                              ; [rip+00000EEA]
	mulss	xmm0,xmm2
	mulss	xmm0,xmm2
	movss	xmm1,[0000000000002078]                              ; [rip+00000EDE]
	subss	xmm1,xmm0
	mulss	xmm1,xmm2
	movaps	xmm0,xmm1
	ret

;; lib_rsqrt: 00000000000011A6
;;   Called from:
;;     000000000000122E (in main)
lib_rsqrt proc
	pxor	xmm1,xmm1
	ucomiss	xmm1,xmm0
	ja	11C3h

l00000000000011AF:
	sqrtss	xmm0,xmm0
	movss	xmm1,[000000000000207C]                              ; [rip+00000EC1]
	divss	xmm1,xmm0
	movaps	xmm0,xmm1
	ret

l00000000000011C3:
	sub	rsp,8h
	call	1060h
	movss	xmm1,[000000000000207C]                              ; [rip+00000EA8]
	divss	xmm1,xmm0
	movaps	xmm0,xmm1
	add	rsp,8h
	ret

;; main: 00000000000011E0
main proc
	push	rbx
	sub	rsp,10h
	mov	rbx,rsi
	mov	rsi,[rsi+8h]
	lea	rdi,[0000000000002008]                                 ; [rip+00000E15]
	mov	eax,0h
	call	1040h
	lea	rdi,[0000000000002030]                                 ; [rip+00000E2C]
	call	1030h
	mov	rdi,[rbx+10h]
	mov	esi,0h
	call	1050h
	movss	dword ptr [rsp+8h],xmm0
	call	Q_rsqrt
	movss	dword ptr [rsp+0Ch],xmm0
	movss	xmm0,dword ptr [rsp+8h]
	call	lib_rsqrt
	movss	dword ptr [rsp+8h],xmm0
	pxor	xmm0,xmm0
	cvtss2sd	xmm0,dword ptr [rsp+0Ch]
	lea	rdi,[0000000000002051]                                 ; [rip+00000E07]
	mov	eax,1h
	call	1040h
	pxor	xmm0,xmm0
	cvtss2sd	xmm0,dword ptr [rsp+8h]
	lea	rdi,[0000000000002062]                                 ; [rip+00000DFD]
	mov	eax,1h
	call	1040h
	add	rsp,10h
	pop	rbx
	ret
0000000000001275                66 2E 0F 1F 84 00 00 00 00 00 90      f..........

;; __libc_csu_init: 0000000000001280
__libc_csu_init proc
	push	r15
	lea	r15,[0000000000003DD8]                                 ; [rip+00002B4F]
	push	r14
	mov	r14,rdx
	push	r13
	mov	r13,rsi
	push	r12
	mov	r12d,edi
	push	rbp
	lea	rbp,[0000000000003DE0]                                 ; [rip+00002B40]
	push	rbx
	sub	rbp,r15
	sub	rsp,8h
	call	_init
	sar	rbp,3h
	jz	12CEh

l00000000000012B3:
	xor	ebx,ebx
	nop	dword ptr [rax]

l00000000000012B8:
	mov	rdx,r14
	mov	rsi,r13
	mov	edi,r12d
	call	qword ptr [r15+rbx*8]
	add	rbx,1h
	cmp	rbp,rbx
	jnz	12B8h

l00000000000012CE:
	add	rsp,8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
00000000000012DD                                        0F 1F 00              ...

;; __libc_csu_fini: 00000000000012E0
__libc_csu_fini proc
	ret
