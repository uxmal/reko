;;; Segment .text (0000000000400440)

;; _start: 0000000000400440
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,F0
	push	rax
	push	rsp
	mov	r8,+004005C0
	mov	rcx,+00400550
	mov	rdi,+0040053D
	call	0000000000400420
	hlt
000000000040046A                               66 0F 1F 44 00 00           f..D..

;; deregister_tm_clones: 0000000000400470
;;   Called from:
;;     00000000004004ED (in __do_global_dtors_aux)
deregister_tm_clones proc
	mov	eax,00601047
	push	rbp
	sub	rax,+00601040
	cmp	rax,0E
	mov	rbp,rsp
	ja	0000000000400487

l0000000000400485:
	pop	rbp
	ret

l0000000000400487:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400485

l0000000000400491:
	pop	rbp
	mov	edi,00601040
	jmp	rax
0000000000400499                            0F 1F 80 00 00 00 00          .......

;; register_tm_clones: 00000000004004A0
;;   Called from:
;;     0000000000400520 (in frame_dummy)
;;     0000000000400528 (in frame_dummy)
register_tm_clones proc
	mov	eax,00601040
	push	rbp
	sub	rax,+00601040
	sar	rax,03
	mov	rbp,rsp
	mov	rdx,rax
	shr	rdx,3F
	add	rax,rdx
	sar	rax,01
	jnz	00000000004004C4

l00000000004004C2:
	pop	rbp
	ret

l00000000004004C4:
	mov	edx,00000000
	test	rdx,rdx
	jz	00000000004004C2

l00000000004004CE:
	pop	rbp
	mov	rsi,rax
	mov	edi,00601040
	jmp	rdx
00000000004004D9                            0F 1F 80 00 00 00 00          .......

;; __do_global_dtors_aux: 00000000004004E0
__do_global_dtors_aux proc
	cmp	[0000000000601040],00                                  ; [rip+00200B59]
	jnz	00000000004004FA

l00000000004004E9:
	push	rbp
	mov	rbp,rsp
	call	0000000000400470
	pop	rbp
	mov	[0000000000601040],01                                  ; [rip+00200B46]

l00000000004004FA:
	ret
00000000004004FC                                     0F 1F 40 00             ..@.

;; frame_dummy: 0000000000400500
frame_dummy proc
	cmp	[0000000000600E20],00                                  ; [rip+00200918]
	jz	0000000000400528

l000000000040050A:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400528

l0000000000400514:
	push	rbp
	mov	edi,00600E20
	mov	rbp,rsp
	call	rax
	pop	rbp
	jmp	00000000004004A0
0000000000400525                0F 1F 00                              ...       

l0000000000400528:
	jmp	00000000004004A0

;; f: 000000000040052D
;;   Called from:
;;     0000000000400546 (in main)
f proc
	push	rbp
	mov	rbp,rsp
	mov	edi,00000078
	call	0000000000400410
	pop	rbp
	ret

;; main: 000000000040053D
main proc
	push	rbp
	mov	rbp,rsp
	mov	eax,00000000
	call	000000000040052D
	pop	rbp
	ret
000000000040054D                                        0F 1F 00              ...

;; __libc_csu_init: 0000000000400550
__libc_csu_init proc
	push	r15
	mov	r15d,edi
	push	r14
	mov	r14,rsi
	push	r13
	mov	r13,rdx
	push	r12
	lea	r12,[0000000000600E10]                                 ; [rip+002008A8]
	push	rbp
	lea	rbp,[0000000000600E18]                                 ; [rip+002008A8]
	push	rbx
	sub	rbp,r12
	xor	ebx,ebx
	sar	rbp,03
	sub	rsp,08
	call	00000000004003E0
	test	rbp,rbp
	jz	00000000004005A6

l0000000000400588:
	nop	dword ptr [rax+rax+00000000]

l0000000000400590:
	mov	rdx,r13
	mov	rsi,r14
	mov	edi,r15d
	call	qword ptr [r12+rbx*8]
	add	rbx,01
	cmp	rbx,rbp
	jnz	0000000000400590

l00000000004005A6:
	add	rsp,08
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
00000000004005B5                66 66 2E 0F 1F 84 00 00 00 00 00      ff.........

;; __libc_csu_fini: 00000000004005C0
__libc_csu_fini proc
	ret
