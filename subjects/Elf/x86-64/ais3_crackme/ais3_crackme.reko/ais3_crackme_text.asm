;;; Segment .text (0000000000400410)

;; _start: 0000000000400410
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,F0
	push	rax
	push	rsp
	mov	r8,+004006B0
	mov	rcx,+00400620
	mov	rdi,+004005C5
	call	0000000000400400
	hlt
000000000040043A                               90 90                       ..   

;; call_gmon_start: 000000000040043C
;;   Called from:
;;     00000000004003CC (in _init)
call_gmon_start proc
	sub	rsp,08
	mov	rax,[0000000000600FE0]                                 ; [rip+00200B99]
	test	rax,rax
	jz	000000000040044E

l000000000040044C:
	call	rax

l000000000040044E:
	add	rsp,08
	ret
0000000000400453          90 90 90 90 90 90 90 90 90 90 90 90 90    .............

;; deregister_tm_clones: 0000000000400460
;;   Called from:
;;     00000000004004DD (in __do_global_dtors_aux)
deregister_tm_clones proc
	mov	eax,0060103F
	push	rbp
	sub	rax,+00601038
	cmp	rax,0E
	mov	rbp,rsp
	ja	0000000000400477

l0000000000400475:
	pop	rbp
	ret

l0000000000400477:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400475

l0000000000400481:
	pop	rbp
	mov	edi,00601038
	jmp	rax
0000000000400489                            0F 1F 80 00 00 00 00          .......

;; register_tm_clones: 0000000000400490
;;   Called from:
;;     0000000000400510 (in frame_dummy)
;;     0000000000400518 (in frame_dummy)
register_tm_clones proc
	mov	eax,00601038
	push	rbp
	sub	rax,+00601038
	sar	rax,03
	mov	rbp,rsp
	mov	rdx,rax
	shr	rdx,3F
	add	rax,rdx
	sar	rax,01
	jnz	00000000004004B4

l00000000004004B2:
	pop	rbp
	ret

l00000000004004B4:
	mov	edx,00000000
	test	rdx,rdx
	jz	00000000004004B2

l00000000004004BE:
	pop	rbp
	mov	rsi,rax
	mov	edi,00601038
	jmp	rdx
00000000004004C9                            0F 1F 80 00 00 00 00          .......

;; __do_global_dtors_aux: 00000000004004D0
__do_global_dtors_aux proc
	cmp	[0000000000601038],00                                  ; [rip+00200B61]
	jnz	00000000004004EA

l00000000004004D9:
	push	rbp
	mov	rbp,rsp
	call	0000000000400460
	pop	rbp
	mov	[0000000000601038],01                                  ; [rip+00200B4E]

l00000000004004EA:
	ret
00000000004004EC                                     0F 1F 40 00             ..@.

;; frame_dummy: 00000000004004F0
frame_dummy proc
	cmp	[0000000000600E08],00                                  ; [rip+00200910]
	jz	0000000000400518

l00000000004004FA:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400518

l0000000000400504:
	push	rbp
	mov	edi,00600E08
	mov	rbp,rsp
	call	rax
	pop	rbp
	jmp	0000000000400490
0000000000400515                0F 1F 00                              ...       

l0000000000400518:
	jmp	0000000000400490
000000000040051D                                        90 90 90              ...

;; verify: 0000000000400520
;;   Called from:
;;     00000000004005F9 (in main)
verify proc
	push	rbp
	mov	rbp,rsp
	mov	[rbp-18],rdi
	mov	dword ptr [rbp-04],00000000
	jmp	00000000004005A1

l0000000000400531:
	mov	eax,[rbp-04]
	movsxd	rdx,eax
	mov	rax,[rbp-18]
	add	rax,rdx
	movzx	eax,byte ptr [rax]
	mov	edx,eax
	mov	eax,[rbp-04]
	xor	eax,edx
	mov	[rbp-05],al
	movzx	edx,byte ptr [rbp-05]
	mov	eax,[rbp-04]
	xor	eax,09
	and	eax,03
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	mov	edx,eax
	movzx	esi,byte ptr [rbp-05]
	mov	eax,[rbp-04]
	xor	eax,09
	and	eax,03
	mov	ecx,eax
	mov	eax,00000008
	sub	eax,ecx
	mov	ecx,eax
	sar	esi,cl
	mov	eax,esi
	or	eax,edx
	mov	[rbp-05],al
	add	byte ptr [rbp-05],08
	mov	eax,[rbp-04]
	cdqe
	movzx	eax,byte ptr [rax+00601020]
	cmp	al,[rbp-05]
	jz	000000000040059D

l0000000000400596:
	mov	eax,00000000
	jmp	00000000004005C3

l000000000040059D:
	add	dword ptr [rbp-04],01

l00000000004005A1:
	mov	eax,[rbp-04]
	movsxd	rdx,eax
	mov	rax,[rbp-18]
	add	rax,rdx
	movzx	eax,byte ptr [rax]
	test	al,al
	jnz	0000000000400531

l00000000004005B9:
	cmp	dword ptr [rbp-04],17
	setz	al
	movzx	eax,al

l00000000004005C3:
	pop	rbp
	ret

;; main: 00000000004005C5
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,10
	mov	[rbp-04],edi
	mov	[rbp-10],rsi
	cmp	dword ptr [rbp-04],02
	jz	00000000004005EB

l00000000004005DA:
	mov	edi,004006C8
	call	00000000004003F0
	mov	eax,FFFFFFFF
	jmp	000000000040061D

l00000000004005EB:
	mov	rax,[rbp-10]
	add	rax,08
	mov	rax,[rax]
	mov	rdi,rax
	call	0000000000400520
	test	eax,eax
	jz	000000000040060E

l0000000000400602:
	mov	edi,004006F0
	call	00000000004003F0
	jmp	0000000000400618

l000000000040060E:
	mov	edi,00400718
	call	00000000004003F0

l0000000000400618:
	mov	eax,00000000

l000000000040061D:
	leave
	ret
000000000040061F                                              90                .

;; __libc_csu_init: 0000000000400620
__libc_csu_init proc
	mov	[rsp-28],rbp
	mov	[rsp-20],r12
	lea	rbp,[0000000000600E00]                                 ; [rip+002007CF]
	lea	r12,[0000000000600DF8]                                 ; [rip+002007C0]
	mov	[rsp-18],r13
	mov	[rsp-10],r14
	mov	[rsp-08],r15
	mov	[rsp-30],rbx
	sub	rsp,38
	sub	rbp,r12
	mov	r13d,edi
	mov	r14,rsi
	sar	rbp,03
	mov	r15,rdx
	call	00000000004003C8
	test	rbp,rbp
	jz	0000000000400686

l000000000040066A:
	xor	ebx,ebx
	nop	dword ptr [rax+00]

l0000000000400670:
	mov	rdx,r15
	mov	rsi,r14
	mov	edi,r13d
	call	qword ptr [r12+rbx*8]
	add	rbx,01
	cmp	rbx,rbp
	jnz	0000000000400670

l0000000000400686:
	mov	rbx,[rsp+08]
	mov	rbp,[rsp+10]
	mov	r12,[rsp+18]
	mov	r13,[rsp+20]
	mov	r14,[rsp+28]
	mov	r15,[rsp+30]
	add	rsp,38
	ret
00000000004006A9                            0F 1F 80 00 00 00 00          .......

;; __libc_csu_fini: 00000000004006B0
__libc_csu_fini proc
	ret
00000000004006B2       90 90                                       ..           
