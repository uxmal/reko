;;; Segment .text (0000000000400410)

;; _start: 0000000000400410
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,0F0h
	push	rax
	push	rsp
	mov	r8,+4006B0h
	mov	rcx,+400620h
	mov	rdi,+4005C5h
	call	400400h
	hlt
000000000040043A                               90 90                       ..    

;; call_gmon_start: 000000000040043C
;;   Called from:
;;     00000000004003CC (in _init)
call_gmon_start proc
	sub	rsp,8h
	mov	rax,[0000000000600FE0]                                 ; [rip+00200B99]
	test	rax,rax
	jz	40044Eh

l000000000040044C:
	call	rax

l000000000040044E:
	add	rsp,8h
	ret
0000000000400453          90 90 90 90 90 90 90 90 90 90 90 90 90    .............

;; deregister_tm_clones: 0000000000400460
;;   Called from:
;;     00000000004004DD (in __do_global_dtors_aux)
deregister_tm_clones proc
	mov	eax,60103Fh
	push	rbp
	sub	rax,+601038h
	cmp	rax,0Eh
	mov	rbp,rsp
	ja	400477h

l0000000000400475:
	pop	rbp
	ret

l0000000000400477:
	mov	eax,0h
	test	rax,rax
	jz	400475h

l0000000000400481:
	pop	rbp
	mov	edi,601038h
	jmp	rax
0000000000400489                            0F 1F 80 00 00 00 00          .......

;; register_tm_clones: 0000000000400490
;;   Called from:
;;     0000000000400510 (in frame_dummy)
;;     0000000000400518 (in frame_dummy)
register_tm_clones proc
	mov	eax,601038h
	push	rbp
	sub	rax,+601038h
	sar	rax,3h
	mov	rbp,rsp
	mov	rdx,rax
	shr	rdx,3Fh
	add	rax,rdx
	sar	rax,1h
	jnz	4004B4h

l00000000004004B2:
	pop	rbp
	ret

l00000000004004B4:
	mov	edx,0h
	test	rdx,rdx
	jz	4004B2h

l00000000004004BE:
	pop	rbp
	mov	rsi,rax
	mov	edi,601038h
	jmp	rdx
00000000004004C9                            0F 1F 80 00 00 00 00          .......

;; __do_global_dtors_aux: 00000000004004D0
__do_global_dtors_aux proc
	cmp	[0000000000601038],0h                                  ; [rip+00200B61]
	jnz	4004EAh

l00000000004004D9:
	push	rbp
	mov	rbp,rsp
	call	400460h
	pop	rbp
	mov	[0000000000601038],1h                                  ; [rip+00200B4E]

l00000000004004EA:
	ret
00000000004004EC                                     0F 1F 40 00             ..@.

;; frame_dummy: 00000000004004F0
frame_dummy proc
	cmp	[0000000000600E08],0h                                  ; [rip+00200910]
	jz	400518h

l00000000004004FA:
	mov	eax,0h
	test	rax,rax
	jz	400518h

l0000000000400504:
	push	rbp
	mov	edi,600E08h
	mov	rbp,rsp
	call	rax
	pop	rbp
	jmp	400490h
0000000000400515                0F 1F 00                              ...        

l0000000000400518:
	jmp	400490h
000000000040051D                                        90 90 90              ...

;; verify: 0000000000400520
;;   Called from:
;;     00000000004005F9 (in main)
verify proc
	push	rbp
	mov	rbp,rsp
	mov	[rbp-18h],rdi
	mov	dword ptr [rbp-4h],0h
	jmp	4005A1h

l0000000000400531:
	mov	eax,[rbp-4h]
	movsxd	rdx,eax
	mov	rax,[rbp-18h]
	add	rax,rdx
	movzx	eax,byte ptr [rax]
	mov	edx,eax
	mov	eax,[rbp-4h]
	xor	eax,edx
	mov	[rbp-5h],al
	movzx	edx,byte ptr [rbp-5h]
	mov	eax,[rbp-4h]
	xor	eax,9h
	and	eax,3h
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	mov	edx,eax
	movzx	esi,byte ptr [rbp-5h]
	mov	eax,[rbp-4h]
	xor	eax,9h
	and	eax,3h
	mov	ecx,eax
	mov	eax,8h
	sub	eax,ecx
	mov	ecx,eax
	sar	esi,cl
	mov	eax,esi
	or	eax,edx
	mov	[rbp-5h],al
	add	byte ptr [rbp-5h],8h
	mov	eax,[rbp-4h]
	cdqe
	movzx	eax,byte ptr [rax+601020h]
	cmp	al,[rbp-5h]
	jz	40059Dh

l0000000000400596:
	mov	eax,0h
	jmp	4005C3h

l000000000040059D:
	add	dword ptr [rbp-4h],1h

l00000000004005A1:
	mov	eax,[rbp-4h]
	movsxd	rdx,eax
	mov	rax,[rbp-18h]
	add	rax,rdx
	movzx	eax,byte ptr [rax]
	test	al,al
	jnz	400531h

l00000000004005B9:
	cmp	dword ptr [rbp-4h],17h
	setz	al
	movzx	eax,al

l00000000004005C3:
	pop	rbp
	ret

;; main: 00000000004005C5
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,10h
	mov	[rbp-4h],edi
	mov	[rbp-10h],rsi
	cmp	dword ptr [rbp-4h],2h
	jz	4005EBh

l00000000004005DA:
	mov	edi,4006C8h
	call	4003F0h
	mov	eax,0FFFFFFFFh
	jmp	40061Dh

l00000000004005EB:
	mov	rax,[rbp-10h]
	add	rax,8h
	mov	rax,[rax]
	mov	rdi,rax
	call	400520h
	test	eax,eax
	jz	40060Eh

l0000000000400602:
	mov	edi,4006F0h
	call	4003F0h
	jmp	400618h

l000000000040060E:
	mov	edi,400718h
	call	4003F0h

l0000000000400618:
	mov	eax,0h

l000000000040061D:
	leave
	ret
000000000040061F                                              90                .

;; __libc_csu_init: 0000000000400620
__libc_csu_init proc
	mov	[rsp-28h],rbp
	mov	[rsp-20h],r12
	lea	rbp,[0000000000600E00]                                 ; [rip+002007CF]
	lea	r12,[0000000000600DF8]                                 ; [rip+002007C0]
	mov	[rsp-18h],r13
	mov	[rsp-10h],r14
	mov	[rsp-8h],r15
	mov	[rsp-30h],rbx
	sub	rsp,38h
	sub	rbp,r12
	mov	r13d,edi
	mov	r14,rsi
	sar	rbp,3h
	mov	r15,rdx
	call	4003C8h
	test	rbp,rbp
	jz	400686h

l000000000040066A:
	xor	ebx,ebx
	nop	dword ptr [rax+0h]

l0000000000400670:
	mov	rdx,r15
	mov	rsi,r14
	mov	edi,r13d
	call	qword ptr [r12+rbx*8]
	add	rbx,1h
	cmp	rbx,rbp
	jnz	400670h

l0000000000400686:
	mov	rbx,[rsp+8h]
	mov	rbp,[rsp+10h]
	mov	r12,[rsp+18h]
	mov	r13,[rsp+20h]
	mov	r14,[rsp+28h]
	mov	r15,[rsp+30h]
	add	rsp,38h
	ret
00000000004006A9                            0F 1F 80 00 00 00 00          .......

;; __libc_csu_fini: 00000000004006B0
__libc_csu_fini proc
	ret
00000000004006B2       90 90                                       ..            
