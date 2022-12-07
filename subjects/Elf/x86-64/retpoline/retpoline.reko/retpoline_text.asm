;;; Segment .text (0000000000400480)

;; _start: 0000000000400480
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,0F0h
	push	rax
	push	rsp
	mov	r8,+400780h
	mov	rcx,+400710h
	mov	rdi,+400660h
	call	[0000000000600FF0]                                    ; [rip+00200B46]
	hlt
00000000004004AB                                  0F 1F 44 00 00            ..D..

;; deregister_tm_clones: 00000000004004B0
;;   Called from:
;;     000000000040052D (in __do_global_dtors_aux)
deregister_tm_clones proc
	push	rbp
	mov	eax,601040h
	cmp	rax,+601040h
	mov	rbp,rsp
	jz	4004D8h

l00000000004004C1:
	mov	eax,0h
	test	rax,rax
	jz	4004D8h

l00000000004004CB:
	pop	rbp
	mov	edi,601040h
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
	mov	esi,601040h
	push	rbp
	sub	rsi,+601040h
	mov	rbp,rsp
	sar	rsi,3h
	mov	rax,rsi
	shr	rax,3Fh
	add	rsi,rax
	sar	rsi,1h
	jz	400518h

l0000000000400503:
	mov	eax,0h
	test	rax,rax
	jz	400518h

l000000000040050D:
	pop	rbp
	mov	edi,601040h
	jmp	rax
0000000000400515                0F 1F 00                              ...        

l0000000000400518:
	pop	rbp
	ret
000000000040051A                               66 0F 1F 44 00 00           f..D..

;; __do_global_dtors_aux: 0000000000400520
__do_global_dtors_aux proc
	cmp	[0000000000601040],0h                                  ; [rip+00200B19]
	jnz	400540h

l0000000000400529:
	push	rbp
	mov	rbp,rsp
	call	deregister_tm_clones
	mov	[0000000000601040],1h                                  ; [rip+00200B07]
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
	jmp	register_tm_clones
0000000000400557                      66 0F 1F 84 00 00 00 00 00        f........

;; my1: 0000000000400560
;;   Called from:
;;     0000000000400631 (in branches)
my1 proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,10h
	mov	[rbp-4h],edi
	mov	[rbp-8h],esi
	movsxd	rdi,dword ptr [rbp-4h]
	movsxd	rsi,dword ptr [rbp-8h]
	call	400470h
	mov	[rbp-10h],rax
	mov	rax,[rbp-10h]
	add	rsp,10h
	pop	rbp
	ret
0000000000400589                            0F 1F 80 00 00 00 00          .......

;; my2: 0000000000400590
my2 proc
	push	rbp
	mov	rbp,rsp
	mov	al,sil
	mov	esi,1h
	mov	[rbp-8h],rdi
	mov	[rbp-9h],al
	mov	rdi,[rbp-8h]
	mov	al,[rbp-9h]
	mov	[rdi],al
	mov	eax,esi
	pop	rbp
	ret

;; branches: 00000000004005B0
branches proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,20h
	mov	[rbp-8h],edi
	mov	[rbp-0Ch],esi
	mov	esi,[rbp-8h]
	cmp	esi,[rbp-0Ch]
	jge	40064Fh

l00000000004005CA:
	mov	eax,[rbp-8h]
	shl	eax,1h
	mov	ecx,[rbp-0Ch]
	shl	ecx,1h
	cmp	eax,ecx
	jge	40064Fh

l00000000004005DE:
	imul	eax,[rbp-8h],3h
	imul	ecx,[rbp-0Ch],3h
	cmp	eax,ecx
	jge	40064Fh

l00000000004005EE:
	mov	eax,[rbp-8h]
	shl	eax,2h
	mov	ecx,[rbp-0Ch]
	shl	ecx,2h
	cmp	eax,ecx
	jge	40064Fh

l0000000000400602:
	mov	eax,2h
	mov	ecx,[rbp-8h]
	mov	[rbp-1Ch],eax
	mov	eax,ecx
	cdq
	mov	ecx,[rbp-1Ch]
	idiv	ecx
	mov	esi,[rbp-0Ch]
	mov	[rbp-20h],eax
	mov	eax,esi
	cdq
	idiv	ecx
	mov	esi,[rbp-20h]
	cmp	esi,eax
	jge	40064Fh

l000000000040062B:
	mov	edi,[rbp-8h]
	mov	esi,[rbp-0Ch]
	call	my1
	mov	[rbp-18h],rax
	mov	rdi,[rbp-18h]
	call	400450h
	mov	dword ptr [rbp-4h],0h
	jmp	400656h

l000000000040064F:
	mov	dword ptr [rbp-4h],0FFFFFFFFh

l0000000000400656:
	mov	eax,[rbp-4h]
	add	rsp,20h
	pop	rbp
	ret
000000000040065F                                              90                .

;; main: 0000000000400660
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,40h
	lea	rax,[rbp-20h]
	mov	dword ptr [rbp-4h],0h
	mov	[rbp-8h],edi
	mov	[rbp-10h],rsi
	mov	rsi,[400798h]
	mov	[rbp-20h],rsi
	mov	rsi,[4007A0h]
	mov	[rbp-18h],rsi
	mov	[rbp-28h],rax
	mov	rax,[rbp-28h]
	mov	r11,[rax]
	mov	edi,1h
	mov	esi,5h
	call	__llvm_retpoline_r11
	mov	[rbp-30h],rax
	mov	rax,[rbp-28h]
	mov	r11,[rax+8h]
	mov	rdi,[rbp-30h]
	mov	esi,78h
	call	__llvm_retpoline_r11
	mov	esi,4007A8h
	mov	edi,esi
	mov	[rbp-34h],eax
	call	400460h
	mov	rdi,[rbp-30h]
	mov	[rbp-38h],eax
	call	400450h
	xor	eax,eax
	add	rsp,40h
	pop	rbp
	ret
00000000004006E9                            0F 1F 80 00 00 00 00          .......

;; __llvm_retpoline_r11: 00000000004006F0
;;   Called from:
;;     00000000004006A7 (in main)
;;     00000000004006C1 (in main)
__llvm_retpoline_r11 proc
	call	fn0000000000400700

l00000000004006F5:
	pause
	jmp	4006F5h
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
	sub	rsp,8h
	sar	rbp,3h
	call	_init
	test	rbp,rbp
	jz	400766h

l0000000000400746:
	xor	ebx,ebx
	nop	dword ptr [rax+rax+0h]

l0000000000400750:
	mov	rdx,r13
	mov	rsi,r14
	mov	edi,r15d
	call	qword ptr [r12+rbx*8]
	add	rbx,1h
	cmp	rbp,rbx
	jnz	400750h

l0000000000400766:
	add	rsp,8h
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
