;;; Segment .text (0000000000400480)

;; _start: 0000000000400480
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,F0
	push	rax
	push	rsp
	mov	r8,+00400780
	mov	rcx,+00400710
	mov	rdi,+00400660
	call	[0000000000600FF0]                                    ; [rip+00200B46]
	hlt
00000000004004AB                                  0F 1F 44 00 00            ..D..

;; deregister_tm_clones: 00000000004004B0
;;   Called from:
;;     000000000040052D (in __do_global_dtors_aux)
deregister_tm_clones proc
	push	rbp
	mov	eax,00601040
	cmp	rax,+00601040
	mov	rbp,rsp
	jz	00000000004004D8

l00000000004004C1:
	mov	eax,00000000
	test	rax,rax
	jz	00000000004004D8

l00000000004004CB:
	pop	rbp
	mov	edi,00601040
	jmp	rax
00000000004004D3          0F 1F 44 00 00                            ..D..       

l00000000004004D8:
	pop	rbp
	ret
00000000004004DA                               66 0F 1F 44 00 00           f..D..

;; register_tm_clones: 00000000004004E0
;;   Called from:
;;     0000000000400555 (in frame_dummy)
register_tm_clones proc
	mov	esi,00601040
	push	rbp
	sub	rsi,+00601040
	mov	rbp,rsp
	sar	rsi,03
	mov	rax,rsi
	shr	rax,3F
	add	rsi,rax
	sar	rsi,01
	jz	0000000000400518

l0000000000400503:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400518

l000000000040050D:
	pop	rbp
	mov	edi,00601040
	jmp	rax
0000000000400515                0F 1F 00                              ...       

l0000000000400518:
	pop	rbp
	ret
000000000040051A                               66 0F 1F 44 00 00           f..D..

;; __do_global_dtors_aux: 0000000000400520
__do_global_dtors_aux proc
	cmp	[0000000000601040],00                                  ; [rip+00200B19]
	jnz	0000000000400540

l0000000000400529:
	push	rbp
	mov	rbp,rsp
	call	00000000004004B0
	mov	[0000000000601040],01                                  ; [rip+00200B07]
	pop	rbp
	ret
000000000040053B                                  0F 1F 44 00 00            ..D..

l0000000000400540:
	ret
0000000000400542       0F 1F 40 00 66 2E 0F 1F 84 00 00 00 00 00   ..@.f.........

;; frame_dummy: 0000000000400550
frame_dummy proc
	push	rbp
	mov	rbp,rsp
	pop	rbp
	jmp	00000000004004E0
0000000000400557                      66 0F 1F 84 00 00 00 00 00        f........

;; my1: 0000000000400560
;;   Called from:
;;     0000000000400631 (in branches)
my1 proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,10
	mov	[rbp-04],edi
	mov	[rbp-08],esi
	movsxd	rdi,dword ptr [rbp-04]
	movsxd	rsi,dword ptr [rbp-08]
	call	0000000000400470
	mov	[rbp-10],rax
	mov	rax,[rbp-10]
	add	rsp,10
	pop	rbp
	ret
0000000000400589                            0F 1F 80 00 00 00 00          .......

;; my2: 0000000000400590
my2 proc
	push	rbp
	mov	rbp,rsp
	mov	al,sil
	mov	esi,00000001
	mov	[rbp-08],rdi
	mov	[rbp-09],al
	mov	rdi,[rbp-08]
	mov	al,[rbp-09]
	mov	[rdi],al
	mov	eax,esi
	pop	rbp
	ret

;; branches: 00000000004005B0
branches proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,20
	mov	[rbp-08],edi
	mov	[rbp-0C],esi
	mov	esi,[rbp-08]
	cmp	esi,[rbp-0C]
	jge	000000000040064F

l00000000004005CA:
	mov	eax,[rbp-08]
	shl	eax,01
	mov	ecx,[rbp-0C]
	shl	ecx,01
	cmp	eax,ecx
	jge	000000000040064F

l00000000004005DE:
	imul	eax,[rbp-08],03
	imul	ecx,[rbp-0C],03
	cmp	eax,ecx
	jge	000000000040064F

l00000000004005EE:
	mov	eax,[rbp-08]
	shl	eax,02
	mov	ecx,[rbp-0C]
	shl	ecx,02
	cmp	eax,ecx
	jge	000000000040064F

l0000000000400602:
	mov	eax,00000002
	mov	ecx,[rbp-08]
	mov	[rbp-1C],eax
	mov	eax,ecx
	cdq
	mov	ecx,[rbp-1C]
	idiv	ecx
	mov	esi,[rbp-0C]
	mov	[rbp-20],eax
	mov	eax,esi
	cdq
	idiv	ecx
	mov	esi,[rbp-20]
	cmp	esi,eax
	jge	000000000040064F

l000000000040062B:
	mov	edi,[rbp-08]
	mov	esi,[rbp-0C]
	call	0000000000400560
	mov	[rbp-18],rax
	mov	rdi,[rbp-18]
	call	0000000000400450
	mov	dword ptr [rbp-04],00000000
	jmp	0000000000400656

l000000000040064F:
	mov	dword ptr [rbp-04],FFFFFFFF

l0000000000400656:
	mov	eax,[rbp-04]
	add	rsp,20
	pop	rbp
	ret
000000000040065F                                              90                .

;; main: 0000000000400660
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,40
	lea	rax,[rbp-20]
	mov	dword ptr [rbp-04],00000000
	mov	[rbp-08],edi
	mov	[rbp-10],rsi
	mov	rsi,[00400798]
	mov	[rbp-20],rsi
	mov	rsi,[004007A0]
	mov	[rbp-18],rsi
	mov	[rbp-28],rax
	mov	rax,[rbp-28]
	mov	r11,[rax]
	mov	edi,00000001
	mov	esi,00000005
	call	00000000004006F0
	mov	[rbp-30],rax
	mov	rax,[rbp-28]
	mov	r11,[rax+08]
	mov	rdi,[rbp-30]
	mov	esi,00000078
	call	00000000004006F0
	mov	esi,004007A8
	mov	edi,esi
	mov	[rbp-34],eax
	call	0000000000400460
	mov	rdi,[rbp-30]
	mov	[rbp-38],eax
	call	0000000000400450
	xor	eax,eax
	add	rsp,40
	pop	rbp
	ret
00000000004006E9                            0F 1F 80 00 00 00 00          .......

;; __llvm_retpoline_r11: 00000000004006F0
;;   Called from:
;;     00000000004006A7 (in main)
;;     00000000004006C1 (in main)
__llvm_retpoline_r11 proc
	call	0000000000400700

l00000000004006F5:
	pause
	jmp	00000000004006F5
00000000004006FC                                     0F 1F 40 00             ..@.

;; fn0000000000400700: 0000000000400700
;;   Called from:
;;     00000000004006F0 (in __llvm_retpoline_r11)
fn0000000000400700 proc
	mov	[rsp],r11
	ret
0000000000400705                66 2E 0F 1F 84 00 00 00 00 00 90      f..........

;; __libc_csu_init: 0000000000400710
__libc_csu_init proc
	push	r15
	push	r14
	mov	r15d,edi
	push	r13
	push	r12
	lea	r12,[0000000000600E10]                                 ; [rip+002006EE]
	push	rbp
	lea	rbp,[0000000000600E18]                                 ; [rip+002006EE]
	push	rbx
	mov	r14,rsi
	mov	r13,rdx
	sub	rbp,r12
	sub	rsp,08
	sar	rbp,03
	call	0000000000400428
	test	rbp,rbp
	jz	0000000000400766

l0000000000400746:
	xor	ebx,ebx
	nop	dword ptr [rax+rax+00000000]

l0000000000400750:
	mov	rdx,r13
	mov	rsi,r14
	mov	edi,r15d
	call	qword ptr [r12+rbx*8]
	add	rbx,01
	cmp	rbp,rbx
	jnz	0000000000400750

l0000000000400766:
	add	rsp,08
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
0000000000400775                90 66 2E 0F 1F 84 00 00 00 00 00      .f.........

;; __libc_csu_fini: 0000000000400780
__libc_csu_fini proc
	ret
