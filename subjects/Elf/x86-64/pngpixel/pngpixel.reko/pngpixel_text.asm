;;; Segment .text (0000000000400CD0)

;; _start: 0000000000400CD0
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,0F0h
	push	rax
	push	rsp
	mov	r8,+4017F0h
	mov	rcx,+401780h
	mov	rdi,+4012F9h
	call	400BC0h
	hlt
0000000000400CFA                               66 0F 1F 44 00 00           f..D..

;; deregister_tm_clones: 0000000000400D00
;;   Called from:
;;     0000000000400D8D (in __do_global_dtors_aux)
deregister_tm_clones proc
	mov	eax,6020FFh
	push	rbp
	sub	rax,+6020F8h
	cmp	rax,0Eh
	mov	rbp,rsp
	jbe	400D30h

l0000000000400D15:
	mov	eax,0h
	test	rax,rax
	jz	400D30h

l0000000000400D1F:
	pop	rbp
	mov	edi,6020F8h
	jmp	rax
0000000000400D27                      66 0F 1F 84 00 00 00 00 00        f........

l0000000000400D30:
	pop	rbp
	ret
0000000000400D32       0F 1F 40 00 66 2E 0F 1F 84 00 00 00 00 00   ..@.f.........

;; register_tm_clones: 0000000000400D40
;;   Called from:
;;     0000000000400DAB (in frame_dummy)
;;     0000000000400DC1 (in frame_dummy)
register_tm_clones proc
	mov	esi,6020F8h
	push	rbp
	sub	rsi,+6020F8h
	sar	rsi,3h
	mov	rbp,rsp
	mov	rax,rsi
	shr	rax,3Fh
	add	rsi,rax
	sar	rsi,1h
	jz	400D78h

l0000000000400D63:
	mov	eax,0h
	test	rax,rax
	jz	400D78h

l0000000000400D6D:
	pop	rbp
	mov	edi,6020F8h
	jmp	rax
0000000000400D75                0F 1F 00                              ...        

l0000000000400D78:
	pop	rbp
	ret
0000000000400D7A                               66 0F 1F 44 00 00           f..D..

;; __do_global_dtors_aux: 0000000000400D80
__do_global_dtors_aux proc
	cmp	[0000000000602108],0h                                  ; [rip+00201381]
	jnz	400D9Ah

l0000000000400D89:
	push	rbp
	mov	rbp,rsp
	call	deregister_tm_clones
	pop	rbp
	mov	[0000000000602108],1h                                  ; [rip+0020136E]

l0000000000400D9A:
	ret
0000000000400D9C                                     0F 1F 40 00             ..@.

;; frame_dummy: 0000000000400DA0
frame_dummy proc
	mov	edi,601E10h
	cmp	qword ptr [rdi],0h
	jnz	400DB0h

l0000000000400DAB:
	jmp	register_tm_clones
0000000000400DAD                                        0F 1F 00              ...

l0000000000400DB0:
	mov	eax,0h
	test	rax,rax
	jz	400DABh

l0000000000400DBA:
	push	rbp
	mov	rbp,rsp
	call	rax
	pop	rbp
	jmp	register_tm_clones

;; component: 0000000000400DC6
;;   Called from:
;;     0000000000400F72 (in print_pixel)
;;     0000000000400FA7 (in print_pixel)
;;     000000000040116F (in print_pixel)
;;     0000000000401191 (in print_pixel)
;;     00000000004011B2 (in print_pixel)
;;     00000000004011EC (in print_pixel)
;;     000000000040120D (in print_pixel)
;;     0000000000401244 (in print_pixel)
;;     0000000000401266 (in print_pixel)
;;     0000000000401288 (in print_pixel)
;;     00000000004012A9 (in print_pixel)
component proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,30h
	mov	[rbp-18h],rdi
	mov	[rbp-1Ch],esi
	mov	[rbp-20h],edx
	mov	[rbp-24h],ecx
	mov	[rbp-28h],r8d
	mov	eax,[rbp-1Ch]
	shr	eax,6h
	imul	eax,[rbp-28h]
	imul	eax,[rbp-24h]
	mov	[rbp-8h],eax
	mov	eax,[rbp-1Ch]
	and	eax,3Fh
	imul	eax,[rbp-28h]
	mov	edx,eax
	mov	eax,[rbp-20h]
	add	eax,edx
	imul	eax,[rbp-24h]
	mov	[rbp-4h],eax
	mov	eax,[rbp-8h]
	shl	rax,3h
	add	[rbp-18h],rax
	mov	eax,[rbp-4h]
	shr	eax,3h
	mov	eax,eax
	add	[rbp-18h],rax
	and	dword ptr [rbp-4h],7h
	cmp	dword ptr [rbp-24h],10h
	ja	400EC1h

l0000000000400E2D:
	mov	eax,[rbp-24h]
	mov	rax,[401828h+rax*8]
	jmp	rax

l0000000000400E3A:
	mov	rax,[rbp-18h]
	movzx	eax,byte ptr [rax]
	movzx	edx,al
	mov	eax,7h
	sub	eax,[rbp-4h]
	mov	ecx,eax
	sar	edx,cl
	mov	eax,edx
	and	eax,1h
	jmp	400EE7h

l0000000000400E5A:
	mov	rax,[rbp-18h]
	movzx	eax,byte ptr [rax]
	movzx	edx,al
	mov	eax,6h
	sub	eax,[rbp-4h]
	mov	ecx,eax
	sar	edx,cl
	mov	eax,edx
	and	eax,3h
	jmp	400EE7h

l0000000000400E77:
	mov	rax,[rbp-18h]
	movzx	eax,byte ptr [rax]
	movzx	edx,al
	mov	eax,4h
	sub	eax,[rbp-4h]
	mov	ecx,eax
	sar	edx,cl
	mov	eax,edx
	and	eax,0Fh
	jmp	400EE7h

l0000000000400E94:
	mov	rax,[rbp-18h]
	movzx	eax,byte ptr [rax]
	movzx	eax,al
	jmp	400EE7h

l0000000000400EA0:
	mov	rax,[rbp-18h]
	movzx	eax,byte ptr [rax]
	movzx	eax,al
	shl	eax,8h
	mov	edx,eax
	mov	rax,[rbp-18h]
	add	rax,1h
	movzx	eax,byte ptr [rax]
	movzx	eax,al
	add	eax,edx
	jmp	400EE7h

l0000000000400EC1:
	mov	rax,[0000000000602100]                                 ; [rip+00201238]
	mov	edx,[rbp-24h]
	mov	esi,401808h
	mov	rdi,rax
	mov	eax,0h
	call	400BF0h
	mov	edi,1h
	call	400C70h

l0000000000400EE7:
	leave
	ret

;; print_pixel: 0000000000400EE9
;;   Called from:
;;     0000000000401645 (in main)
print_pixel proc
	push	rbp
	mov	rbp,rsp
	push	r13
	push	r12
	push	rbx
	sub	rsp,58h
	mov	[rbp-58h],rdi
	mov	[rbp-60h],rsi
	mov	[rbp-68h],rdx
	mov	[rbp-6Ch],ecx
	mov	rax,fs:[0028h]
	mov	[rbp-28h],rax
	xor	eax,eax
	mov	rdx,[rbp-60h]
	mov	rax,[rbp-58h]
	mov	rsi,rdx
	mov	rdi,rax
	call	400B30h
	movzx	eax,al
	mov	[rbp-40h],eax
	mov	rdx,[rbp-60h]
	mov	rax,[rbp-58h]
	mov	rsi,rdx
	mov	rdi,rax
	call	400BB0h
	movzx	eax,al
	cmp	eax,6h
	ja	4012C9h

l0000000000400F4C:
	mov	eax,eax
	mov	rax,[401958h+rax*8]
	jmp	rax

l0000000000400F58:
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,1h
	mov	ecx,edx
	mov	edx,0h
	mov	rdi,rax
	call	component
	mov	esi,eax
	mov	edi,4018B0h
	mov	eax,0h
	call	400B70h
	jmp	4012DAh

l0000000000400F8D:
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,1h
	mov	ecx,edx
	mov	edx,0h
	mov	rdi,rax
	call	component
	mov	[rbp-3Ch],eax
	mov	qword ptr [rbp-38h],+0h
	mov	dword ptr [rbp-48h],0h
	lea	rcx,[rbp-48h]
	lea	rdx,[rbp-38h]
	mov	rsi,[rbp-60h]
	mov	rax,[rbp-58h]
	mov	rdi,rax
	call	400CB0h
	and	eax,8h
	test	eax,eax
	jz	401137h

l0000000000400FE1:
	mov	eax,[rbp-48h]
	test	eax,eax
	jle	401137h

l0000000000400FEC:
	mov	rax,[rbp-38h]
	test	rax,rax
	jz	401137h

l0000000000400FF9:
	mov	qword ptr [rbp-30h],+0h
	mov	dword ptr [rbp-44h],0h
	lea	rcx,[rbp-44h]
	lea	rdx,[rbp-30h]
	mov	rsi,[rbp-60h]
	mov	rax,[rbp-58h]
	mov	r8d,0h
	mov	rdi,rax
	call	400B80h
	and	eax,10h
	test	eax,eax
	jz	4010D1h

l0000000000401031:
	mov	eax,[rbp-44h]
	test	eax,eax
	jle	4010D1h

l000000000040103C:
	mov	rax,[rbp-30h]
	test	rax,rax
	jz	4010D1h

l0000000000401049:
	mov	eax,[rbp-44h]
	cmp	[rbp-3Ch],eax
	jnc	401063h

l0000000000401051:
	mov	rdx,[rbp-30h]
	mov	eax,[rbp-3Ch]
	add	rax,rdx
	movzx	eax,byte ptr [rax]
	movzx	esi,al
	jmp	401068h

l0000000000401063:
	mov	esi,0FFh

l0000000000401068:
	mov	rcx,[rbp-38h]
	mov	edx,[rbp-3Ch]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,rcx
	movzx	eax,byte ptr [rax+2h]
	movzx	edi,al
	mov	rcx,[rbp-38h]
	mov	edx,[rbp-3Ch]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,rcx
	movzx	eax,byte ptr [rax+1h]
	movzx	ecx,al
	mov	r8,[rbp-38h]
	mov	edx,[rbp-3Ch]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,r8
	movzx	eax,byte ptr [rax]
	movzx	edx,al
	mov	eax,[rbp-3Ch]
	mov	r9d,esi
	mov	r8d,edi
	mov	esi,eax
	mov	edi,4018B9h
	mov	eax,0h
	call	400B70h
	jmp	401150h

l00000000004010D1:
	mov	rcx,[rbp-38h]
	mov	edx,[rbp-3Ch]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,rcx
	movzx	eax,byte ptr [rax+2h]
	movzx	esi,al
	mov	rcx,[rbp-38h]
	mov	edx,[rbp-3Ch]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,rcx
	movzx	eax,byte ptr [rax+1h]
	movzx	ecx,al
	mov	rdi,[rbp-38h]
	mov	edx,[rbp-3Ch]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,rdi
	movzx	eax,byte ptr [rax]
	movzx	edx,al
	mov	eax,[rbp-3Ch]
	mov	r8d,esi
	mov	esi,eax
	mov	edi,4018D3h
	mov	eax,0h
	call	400B70h
	jmp	401150h

l0000000000401137:
	mov	eax,[rbp-3Ch]
	mov	esi,eax
	mov	edi,4018EAh
	mov	eax,0h
	call	400B70h
	jmp	4012DAh

l0000000000401150:
	jmp	4012DAh

l0000000000401155:
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,3h
	mov	ecx,edx
	mov	edx,2h
	mov	rdi,rax
	call	component
	mov	r12d,eax
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,3h
	mov	ecx,edx
	mov	edx,1h
	mov	rdi,rax
	call	component
	mov	ebx,eax
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,3h
	mov	ecx,edx
	mov	edx,0h
	mov	rdi,rax
	call	component
	mov	ecx,r12d
	mov	edx,ebx
	mov	esi,eax
	mov	edi,401906h
	mov	eax,0h
	call	400B70h
	jmp	4012DAh

l00000000004011D2:
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,2h
	mov	ecx,edx
	mov	edx,1h
	mov	rdi,rax
	call	component
	mov	ebx,eax
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,2h
	mov	ecx,edx
	mov	edx,0h
	mov	rdi,rax
	call	component
	mov	edx,ebx
	mov	esi,eax
	mov	edi,401914h
	mov	eax,0h
	call	400B70h
	jmp	4012DAh

l000000000040122A:
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,4h
	mov	ecx,edx
	mov	edx,3h
	mov	rdi,rax
	call	component
	mov	r13d,eax
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,4h
	mov	ecx,edx
	mov	edx,2h
	mov	rdi,rax
	call	component
	mov	r12d,eax
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,4h
	mov	ecx,edx
	mov	edx,1h
	mov	rdi,rax
	call	component
	mov	ebx,eax
	mov	edx,[rbp-40h]
	mov	esi,[rbp-6Ch]
	mov	rax,[rbp-68h]
	mov	r8d,4h
	mov	ecx,edx
	mov	edx,0h
	mov	rdi,rax
	call	component
	mov	r8d,r13d
	mov	ecx,r12d
	mov	edx,ebx
	mov	esi,eax
	mov	edi,401926h
	mov	eax,0h
	call	400B70h
	jmp	4012DAh

l00000000004012C9:
	mov	rax,[rbp-58h]
	mov	esi,401938h
	mov	rdi,rax
	call	400B50h

l00000000004012DA:
	mov	rax,[rbp-28h]
	xor	rax,fs:[0028h]
	jz	4012EEh

l00000000004012E9:
	call	400B40h

l00000000004012EE:
	add	rsp,58h
	pop	rbx
	pop	r12
	pop	r13
	pop	rbp
	ret

;; main: 00000000004012F9
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,+0A0h
	mov	[rbp-94h],edi
	mov	[rbp-0A0h],rsi
	mov	rax,fs:[0028h]
	mov	[rbp-8h],rax
	xor	eax,eax
	mov	dword ptr [rbp-84h],1h
	cmp	dword ptr [rbp-94h],4h
	jnz	40173Fh

l0000000000401337:
	mov	rax,[rbp-0A0h]
	add	rax,8h
	mov	rax,[rax]
	mov	rdi,rax
	call	400C30h
	mov	[rbp-28h],rax
	mov	rax,[rbp-0A0h]
	add	rax,10h
	mov	rax,[rax]
	mov	rdi,rax
	call	400C30h
	mov	[rbp-20h],rax
	mov	rax,[rbp-0A0h]
	add	rax,18h
	mov	rax,[rax]
	mov	esi,401990h
	mov	rdi,rax
	call	400C40h
	mov	[rbp-18h],rax
	mov	qword ptr [rbp-40h],+0h
	cmp	qword ptr [rbp-18h],0h
	jz	401716h

l000000000040139D:
	mov	ecx,0h
	mov	edx,0h
	mov	esi,0h
	mov	edi,401993h
	call	400BD0h
	mov	[rbp-38h],rax
	mov	rax,[rbp-38h]
	test	rax,rax
	jz	4016F6h

l00000000004013C7:
	mov	rax,[rbp-38h]
	mov	rdi,rax
	call	400CA0h
	mov	[rbp-30h],rax
	mov	rax,[rbp-30h]
	test	rax,rax
	jz	4016C0h

l00000000004013E4:
	mov	rax,[rbp-38h]
	mov	rdx,[rbp-18h]
	mov	rsi,rdx
	mov	rdi,rax
	call	400C90h
	mov	rdx,[rbp-30h]
	mov	rax,[rbp-38h]
	mov	rsi,rdx
	mov	rdi,rax
	call	400C10h
	mov	rdx,[rbp-30h]
	mov	rax,[rbp-38h]
	mov	rsi,rdx
	mov	rdi,rax
	call	400BE0h
	mov	rdx,rax
	mov	rax,[rbp-38h]
	mov	rsi,rdx
	mov	rdi,rax
	call	400C20h
	mov	[rbp-40h],rax
	mov	rax,[rbp-40h]
	mov	[rbp-10h],rax
	mov	rsi,[rbp-30h]
	mov	rax,[rbp-38h]
	lea	r9,[rbp-74h]
	lea	r8,[rbp-78h]
	lea	rcx,[rbp-7Ch]
	lea	rdx,[rbp-80h]
	sub	rsp,8h
	lea	rdi,[rbp-68h]
	push	rdi
	lea	rdi,[rbp-6Ch]
	push	rdi
	lea	rdi,[rbp-70h]
	push	rdi
	mov	rdi,rax
	call	400C50h
	add	rsp,20h
	test	eax,eax
	jz	4016AFh

l000000000040147A:
	mov	eax,[rbp-70h]
	test	eax,eax
	jz	401488h

l0000000000401481:
	cmp	eax,1h
	jz	401491h

l0000000000401486:
	jmp	40149Ah

l0000000000401488:
	mov	dword ptr [rbp-64h],1h
	jmp	4014ABh

l0000000000401491:
	mov	dword ptr [rbp-64h],7h
	jmp	4014ABh

l000000000040149A:
	mov	rax,[rbp-38h]
	mov	esi,40199Ah
	mov	rdi,rax
	call	400B50h

l00000000004014AB:
	mov	rax,[rbp-38h]
	mov	rdi,rax
	call	400B60h
	mov	dword ptr [rbp-60h],0h
	jmp	401673h

l00000000004014C3:
	mov	eax,[rbp-70h]
	cmp	eax,1h
	jnz	4015CEh

l00000000004014CF:
	cmp	dword ptr [rbp-60h],1h
	jle	4014EFh

l00000000004014D5:
	mov	eax,7h
	sub	eax,[rbp-60h]
	sar	eax,1h
	mov	edx,1h
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	sub	eax,1h
	jmp	4014F4h

l00000000004014EF:
	mov	eax,7h

l00000000004014F4:
	mov	edx,[rbp-60h]
	mov	esi,edx
	and	esi,1h
	mov	edx,[rbp-60h]
	add	edx,1h
	sar	edx,1h
	mov	ecx,3h
	sub	ecx,edx
	mov	edx,ecx
	mov	ecx,edx
	shl	esi,cl
	mov	edx,esi
	and	edx,7h
	sub	eax,edx
	mov	edx,eax
	mov	eax,[rbp-80h]
	add	edx,eax
	cmp	dword ptr [rbp-60h],1h
	jle	401531h

l0000000000401525:
	mov	eax,7h
	sub	eax,[rbp-60h]
	sar	eax,1h
	jmp	401536h

l0000000000401531:
	mov	eax,3h

l0000000000401536:
	mov	ecx,eax
	shr	edx,cl
	mov	eax,edx
	test	eax,eax
	jz	40166Eh

l0000000000401544:
	mov	eax,[rbp-60h]
	and	eax,1h
	mov	edx,eax
	mov	eax,[rbp-60h]
	add	eax,1h
	sar	eax,1h
	mov	ecx,3h
	sub	ecx,eax
	mov	eax,ecx
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	and	eax,7h
	mov	[rbp-58h],eax
	mov	eax,[rbp-60h]
	and	eax,1h
	test	eax,eax
	setz	al
	movzx	edx,al
	mov	eax,[rbp-60h]
	sar	eax,1h
	mov	ecx,3h
	sub	ecx,eax
	mov	eax,ecx
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	and	eax,7h
	mov	[rbp-5Ch],eax
	mov	eax,7h
	sub	eax,[rbp-60h]
	sar	eax,1h
	mov	edx,1h
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	mov	[rbp-50h],eax
	cmp	dword ptr [rbp-60h],2h
	jle	4015C4h

l00000000004015AF:
	mov	eax,[rbp-60h]
	sub	eax,1h
	sar	eax,1h
	mov	edx,8h
	mov	ecx,eax
	sar	edx,cl
	mov	eax,edx
	jmp	4015C9h

l00000000004015C4:
	mov	eax,8h

l00000000004015C9:
	mov	[rbp-54h],eax
	jmp	4015E8h

l00000000004015CE:
	mov	dword ptr [rbp-58h],0h
	mov	eax,[rbp-58h]
	mov	[rbp-5Ch],eax
	mov	dword ptr [rbp-50h],1h
	mov	eax,[rbp-50h]
	mov	[rbp-54h],eax

l00000000004015E8:
	mov	eax,[rbp-5Ch]
	mov	[rbp-4Ch],eax
	jmp	401664h

l00000000004015F0:
	mov	edi,4019B6h
	call	400B20h
	mov	rax,[rbp-38h]
	mov	rcx,[rbp-10h]
	mov	edx,0h
	mov	rsi,rcx
	mov	rdi,rax
	call	400C00h
	mov	eax,[rbp-4Ch]
	cmp	rax,[rbp-20h]
	jnz	40165Eh

l000000000040161B:
	mov	eax,[rbp-58h]
	mov	[rbp-48h],eax
	mov	dword ptr [rbp-44h],0h
	jmp	401656h

l000000000040162A:
	mov	eax,[rbp-48h]
	cmp	rax,[rbp-28h]
	jnz	40164Ch

l0000000000401633:
	mov	rsi,[rbp-30h]
	mov	rax,[rbp-38h]
	mov	ecx,[rbp-44h]
	mov	rdx,[rbp-10h]
	mov	rdi,rax
	call	print_pixel
	jmp	40167Fh

l000000000040164C:
	mov	eax,[rbp-50h]
	add	[rbp-48h],eax
	add	dword ptr [rbp-44h],1h

l0000000000401656:
	mov	eax,[rbp-80h]
	cmp	[rbp-48h],eax
	jc	40162Ah

l000000000040165E:
	mov	eax,[rbp-54h]
	add	[rbp-4Ch],eax

l0000000000401664:
	mov	eax,[rbp-7Ch]
	cmp	[rbp-4Ch],eax
	jc	4015F0h

l000000000040166C:
	jmp	40166Fh

l000000000040166E:
	nop

l000000000040166F:
	add	dword ptr [rbp-60h],1h

l0000000000401673:
	mov	eax,[rbp-60h]
	cmp	eax,[rbp-64h]
	jl	4014C3h

l000000000040167F:
	mov	qword ptr [rbp-40h],+0h
	mov	rax,[rbp-38h]
	mov	rdx,[rbp-10h]
	mov	rsi,rdx
	mov	rdi,rax
	call	400BA0h
	mov	rax,[rbp-38h]
	lea	rdx,[rbp-30h]
	mov	rsi,rdx
	mov	rdi,rax
	call	400C60h
	jmp	4016DEh

l00000000004016AF:
	mov	rax,[rbp-38h]
	mov	esi,4019C3h
	mov	rdi,rax
	call	400B50h

l00000000004016C0:
	mov	rax,[0000000000602100]                                 ; [rip+00200A39]
	mov	rcx,rax
	mov	edx,2Ch
	mov	esi,1h
	mov	edi,4019E8h
	call	400C80h

l00000000004016DE:
	lea	rax,[rbp-38h]
	mov	edx,0h
	mov	esi,0h
	mov	rdi,rax
	call	400B90h
	jmp	40175Dh

l00000000004016F6:
	mov	rax,[0000000000602100]                                 ; [rip+00200A03]
	mov	rcx,rax
	mov	edx,2Eh
	mov	esi,1h
	mov	edi,401A18h
	call	400C80h
	jmp	40175Dh

l0000000000401716:
	mov	rax,[rbp-0A0h]
	add	rax,18h
	mov	rdx,[rax]
	mov	rax,[0000000000602100]                                 ; [rip+002009D5]
	mov	esi,401A48h
	mov	rdi,rax
	mov	eax,0h
	call	400BF0h
	jmp	40175Dh

l000000000040173F:
	mov	rax,[0000000000602100]                                 ; [rip+002009BA]
	mov	rcx,rax
	mov	edx,27h
	mov	esi,1h
	mov	edi,401A70h
	call	400C80h

l000000000040175D:
	mov	eax,[rbp-84h]
	mov	rdi,[rbp-8h]
	xor	rdi,fs:[0028h]
	jz	401777h

l0000000000401772:
	call	400B40h

l0000000000401777:
	leave
	ret
0000000000401779                            0F 1F 80 00 00 00 00          .......

;; __libc_csu_init: 0000000000401780
__libc_csu_init proc
	push	r15
	push	r14
	mov	r15d,edi
	push	r13
	push	r12
	lea	r12,[0000000000601E00]                                 ; [rip+0020066E]
	push	rbp
	lea	rbp,[0000000000601E08]                                 ; [rip+0020066E]
	push	rbx
	mov	r14,rsi
	mov	r13,rdx
	sub	rbp,r12
	sub	rsp,8h
	sar	rbp,3h
	call	_init
	test	rbp,rbp
	jz	4017D6h

l00000000004017B6:
	xor	ebx,ebx
	nop	dword ptr [rax+rax+0h]

l00000000004017C0:
	mov	rdx,r13
	mov	rsi,r14
	mov	edi,r15d
	call	qword ptr [r12+rbx*8]
	add	rbx,1h
	cmp	rbx,rbp
	jnz	4017C0h

l00000000004017D6:
	add	rsp,8h
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
00000000004017E5                90 66 2E 0F 1F 84 00 00 00 00 00      .f.........

;; __libc_csu_fini: 00000000004017F0
__libc_csu_fini proc
	ret
