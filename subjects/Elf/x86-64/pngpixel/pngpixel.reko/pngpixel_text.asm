;;; Segment .text (0000000000400CD0)

;; _start: 0000000000400CD0
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,F0
	push	rax
	push	rsp
	mov	r8,+004017F0
	mov	rcx,+00401780
	mov	rdi,+004012F9
	call	0000000000400BC0
	hlt
0000000000400CFA                               66 0F 1F 44 00 00           f..D..

;; deregister_tm_clones: 0000000000400D00
;;   Called from:
;;     0000000000400D8D (in __do_global_dtors_aux)
deregister_tm_clones proc
	mov	eax,006020FF
	push	rbp
	sub	rax,+006020F8
	cmp	rax,0E
	mov	rbp,rsp
	jbe	0000000000400D30

l0000000000400D15:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400D30

l0000000000400D1F:
	pop	rbp
	mov	edi,006020F8
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
	mov	esi,006020F8
	push	rbp
	sub	rsi,+006020F8
	sar	rsi,03
	mov	rbp,rsp
	mov	rax,rsi
	shr	rax,3F
	add	rsi,rax
	sar	rsi,01
	jz	0000000000400D78

l0000000000400D63:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400D78

l0000000000400D6D:
	pop	rbp
	mov	edi,006020F8
	jmp	rax
0000000000400D75                0F 1F 00                              ...       

l0000000000400D78:
	pop	rbp
	ret
0000000000400D7A                               66 0F 1F 44 00 00           f..D..

;; __do_global_dtors_aux: 0000000000400D80
__do_global_dtors_aux proc
	cmp	[0000000000602108],00                                  ; [rip+00201381]
	jnz	0000000000400D9A

l0000000000400D89:
	push	rbp
	mov	rbp,rsp
	call	0000000000400D00
	pop	rbp
	mov	[0000000000602108],01                                  ; [rip+0020136E]

l0000000000400D9A:
	ret
0000000000400D9C                                     0F 1F 40 00             ..@.

;; frame_dummy: 0000000000400DA0
frame_dummy proc
	mov	edi,00601E10
	cmp	qword ptr [rdi],00
	jnz	0000000000400DB0

l0000000000400DAB:
	jmp	0000000000400D40
0000000000400DAD                                        0F 1F 00              ...

l0000000000400DB0:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400DAB

l0000000000400DBA:
	push	rbp
	mov	rbp,rsp
	call	rax
	pop	rbp
	jmp	0000000000400D40

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
	sub	rsp,30
	mov	[rbp-18],rdi
	mov	[rbp-1C],esi
	mov	[rbp-20],edx
	mov	[rbp-24],ecx
	mov	[rbp-28],r8d
	mov	eax,[rbp-1C]
	shr	eax,06
	imul	eax,[rbp-28]
	imul	eax,[rbp-24]
	mov	[rbp-08],eax
	mov	eax,[rbp-1C]
	and	eax,3F
	imul	eax,[rbp-28]
	mov	edx,eax
	mov	eax,[rbp-20]
	add	eax,edx
	imul	eax,[rbp-24]
	mov	[rbp-04],eax
	mov	eax,[rbp-08]
	shl	rax,03
	add	[rbp-18],rax
	mov	eax,[rbp-04]
	shr	eax,03
	mov	eax,eax
	add	[rbp-18],rax
	and	dword ptr [rbp-04],07
	cmp	dword ptr [rbp-24],10
	ja	0000000000400EC1

l0000000000400E2D:
	mov	eax,[rbp-24]
	mov	rax,[00401828+rax*8]
	jmp	rax

l0000000000400E3A:
	mov	rax,[rbp-18]
	movzx	eax,byte ptr [rax]
	movzx	edx,al
	mov	eax,00000007
	sub	eax,[rbp-04]
	mov	ecx,eax
	sar	edx,cl
	mov	eax,edx
	and	eax,01
	jmp	0000000000400EE7

l0000000000400E5A:
	mov	rax,[rbp-18]
	movzx	eax,byte ptr [rax]
	movzx	edx,al
	mov	eax,00000006
	sub	eax,[rbp-04]
	mov	ecx,eax
	sar	edx,cl
	mov	eax,edx
	and	eax,03
	jmp	0000000000400EE7

l0000000000400E77:
	mov	rax,[rbp-18]
	movzx	eax,byte ptr [rax]
	movzx	edx,al
	mov	eax,00000004
	sub	eax,[rbp-04]
	mov	ecx,eax
	sar	edx,cl
	mov	eax,edx
	and	eax,0F
	jmp	0000000000400EE7

l0000000000400E94:
	mov	rax,[rbp-18]
	movzx	eax,byte ptr [rax]
	movzx	eax,al
	jmp	0000000000400EE7

l0000000000400EA0:
	mov	rax,[rbp-18]
	movzx	eax,byte ptr [rax]
	movzx	eax,al
	shl	eax,08
	mov	edx,eax
	mov	rax,[rbp-18]
	add	rax,01
	movzx	eax,byte ptr [rax]
	movzx	eax,al
	add	eax,edx
	jmp	0000000000400EE7

l0000000000400EC1:
	mov	rax,[0000000000602100]                                 ; [rip+00201238]
	mov	edx,[rbp-24]
	mov	esi,00401808
	mov	rdi,rax
	mov	eax,00000000
	call	0000000000400BF0
	mov	edi,00000001
	call	0000000000400C70

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
	sub	rsp,58
	mov	[rbp-58],rdi
	mov	[rbp-60],rsi
	mov	[rbp-68],rdx
	mov	[rbp-6C],ecx
	mov	rax,fs:[00000028]
	mov	[rbp-28],rax
	xor	eax,eax
	mov	rdx,[rbp-60]
	mov	rax,[rbp-58]
	mov	rsi,rdx
	mov	rdi,rax
	call	0000000000400B30
	movzx	eax,al
	mov	[rbp-40],eax
	mov	rdx,[rbp-60]
	mov	rax,[rbp-58]
	mov	rsi,rdx
	mov	rdi,rax
	call	0000000000400BB0
	movzx	eax,al
	cmp	eax,06
	ja	00000000004012C9

l0000000000400F4C:
	mov	eax,eax
	mov	rax,[00401958+rax*8]
	jmp	rax

l0000000000400F58:
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000001
	mov	ecx,edx
	mov	edx,00000000
	mov	rdi,rax
	call	0000000000400DC6
	mov	esi,eax
	mov	edi,004018B0
	mov	eax,00000000
	call	0000000000400B70
	jmp	00000000004012DA

l0000000000400F8D:
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000001
	mov	ecx,edx
	mov	edx,00000000
	mov	rdi,rax
	call	0000000000400DC6
	mov	[rbp-3C],eax
	mov	qword ptr [rbp-38],+00000000
	mov	dword ptr [rbp-48],00000000
	lea	rcx,[rbp-48]
	lea	rdx,[rbp-38]
	mov	rsi,[rbp-60]
	mov	rax,[rbp-58]
	mov	rdi,rax
	call	0000000000400CB0
	and	eax,08
	test	eax,eax
	jz	0000000000401137

l0000000000400FE1:
	mov	eax,[rbp-48]
	test	eax,eax
	jle	0000000000401137

l0000000000400FEC:
	mov	rax,[rbp-38]
	test	rax,rax
	jz	0000000000401137

l0000000000400FF9:
	mov	qword ptr [rbp-30],+00000000
	mov	dword ptr [rbp-44],00000000
	lea	rcx,[rbp-44]
	lea	rdx,[rbp-30]
	mov	rsi,[rbp-60]
	mov	rax,[rbp-58]
	mov	r8d,00000000
	mov	rdi,rax
	call	0000000000400B80
	and	eax,10
	test	eax,eax
	jz	00000000004010D1

l0000000000401031:
	mov	eax,[rbp-44]
	test	eax,eax
	jle	00000000004010D1

l000000000040103C:
	mov	rax,[rbp-30]
	test	rax,rax
	jz	00000000004010D1

l0000000000401049:
	mov	eax,[rbp-44]
	cmp	[rbp-3C],eax
	jnc	0000000000401063

l0000000000401051:
	mov	rdx,[rbp-30]
	mov	eax,[rbp-3C]
	add	rax,rdx
	movzx	eax,byte ptr [rax]
	movzx	esi,al
	jmp	0000000000401068

l0000000000401063:
	mov	esi,000000FF

l0000000000401068:
	mov	rcx,[rbp-38]
	mov	edx,[rbp-3C]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,rcx
	movzx	eax,byte ptr [rax+02]
	movzx	edi,al
	mov	rcx,[rbp-38]
	mov	edx,[rbp-3C]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,rcx
	movzx	eax,byte ptr [rax+01]
	movzx	ecx,al
	mov	r8,[rbp-38]
	mov	edx,[rbp-3C]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,r8
	movzx	eax,byte ptr [rax]
	movzx	edx,al
	mov	eax,[rbp-3C]
	mov	r9d,esi
	mov	r8d,edi
	mov	esi,eax
	mov	edi,004018B9
	mov	eax,00000000
	call	0000000000400B70
	jmp	0000000000401150

l00000000004010D1:
	mov	rcx,[rbp-38]
	mov	edx,[rbp-3C]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,rcx
	movzx	eax,byte ptr [rax+02]
	movzx	esi,al
	mov	rcx,[rbp-38]
	mov	edx,[rbp-3C]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,rcx
	movzx	eax,byte ptr [rax+01]
	movzx	ecx,al
	mov	rdi,[rbp-38]
	mov	edx,[rbp-3C]
	mov	rax,rdx
	add	rax,rax
	add	rax,rdx
	add	rax,rdi
	movzx	eax,byte ptr [rax]
	movzx	edx,al
	mov	eax,[rbp-3C]
	mov	r8d,esi
	mov	esi,eax
	mov	edi,004018D3
	mov	eax,00000000
	call	0000000000400B70
	jmp	0000000000401150

l0000000000401137:
	mov	eax,[rbp-3C]
	mov	esi,eax
	mov	edi,004018EA
	mov	eax,00000000
	call	0000000000400B70
	jmp	00000000004012DA

l0000000000401150:
	jmp	00000000004012DA

l0000000000401155:
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000003
	mov	ecx,edx
	mov	edx,00000002
	mov	rdi,rax
	call	0000000000400DC6
	mov	r12d,eax
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000003
	mov	ecx,edx
	mov	edx,00000001
	mov	rdi,rax
	call	0000000000400DC6
	mov	ebx,eax
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000003
	mov	ecx,edx
	mov	edx,00000000
	mov	rdi,rax
	call	0000000000400DC6
	mov	ecx,r12d
	mov	edx,ebx
	mov	esi,eax
	mov	edi,00401906
	mov	eax,00000000
	call	0000000000400B70
	jmp	00000000004012DA

l00000000004011D2:
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000002
	mov	ecx,edx
	mov	edx,00000001
	mov	rdi,rax
	call	0000000000400DC6
	mov	ebx,eax
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000002
	mov	ecx,edx
	mov	edx,00000000
	mov	rdi,rax
	call	0000000000400DC6
	mov	edx,ebx
	mov	esi,eax
	mov	edi,00401914
	mov	eax,00000000
	call	0000000000400B70
	jmp	00000000004012DA

l000000000040122A:
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000004
	mov	ecx,edx
	mov	edx,00000003
	mov	rdi,rax
	call	0000000000400DC6
	mov	r13d,eax
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000004
	mov	ecx,edx
	mov	edx,00000002
	mov	rdi,rax
	call	0000000000400DC6
	mov	r12d,eax
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000004
	mov	ecx,edx
	mov	edx,00000001
	mov	rdi,rax
	call	0000000000400DC6
	mov	ebx,eax
	mov	edx,[rbp-40]
	mov	esi,[rbp-6C]
	mov	rax,[rbp-68]
	mov	r8d,00000004
	mov	ecx,edx
	mov	edx,00000000
	mov	rdi,rax
	call	0000000000400DC6
	mov	r8d,r13d
	mov	ecx,r12d
	mov	edx,ebx
	mov	esi,eax
	mov	edi,00401926
	mov	eax,00000000
	call	0000000000400B70
	jmp	00000000004012DA

l00000000004012C9:
	mov	rax,[rbp-58]
	mov	esi,00401938
	mov	rdi,rax
	call	0000000000400B50

l00000000004012DA:
	mov	rax,[rbp-28]
	xor	rax,fs:[00000028]
	jz	00000000004012EE

l00000000004012E9:
	call	0000000000400B40

l00000000004012EE:
	add	rsp,58
	pop	rbx
	pop	r12
	pop	r13
	pop	rbp
	ret

;; main: 00000000004012F9
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,+000000A0
	mov	[rbp-00000094],edi
	mov	[rbp-000000A0],rsi
	mov	rax,fs:[00000028]
	mov	[rbp-08],rax
	xor	eax,eax
	mov	dword ptr [rbp-00000084],00000001
	cmp	dword ptr [rbp-00000094],04
	jnz	000000000040173F

l0000000000401337:
	mov	rax,[rbp-000000A0]
	add	rax,08
	mov	rax,[rax]
	mov	rdi,rax
	call	0000000000400C30
	mov	[rbp-28],rax
	mov	rax,[rbp-000000A0]
	add	rax,10
	mov	rax,[rax]
	mov	rdi,rax
	call	0000000000400C30
	mov	[rbp-20],rax
	mov	rax,[rbp-000000A0]
	add	rax,18
	mov	rax,[rax]
	mov	esi,00401990
	mov	rdi,rax
	call	0000000000400C40
	mov	[rbp-18],rax
	mov	qword ptr [rbp-40],+00000000
	cmp	qword ptr [rbp-18],00
	jz	0000000000401716

l000000000040139D:
	mov	ecx,00000000
	mov	edx,00000000
	mov	esi,00000000
	mov	edi,00401993
	call	0000000000400BD0
	mov	[rbp-38],rax
	mov	rax,[rbp-38]
	test	rax,rax
	jz	00000000004016F6

l00000000004013C7:
	mov	rax,[rbp-38]
	mov	rdi,rax
	call	0000000000400CA0
	mov	[rbp-30],rax
	mov	rax,[rbp-30]
	test	rax,rax
	jz	00000000004016C0

l00000000004013E4:
	mov	rax,[rbp-38]
	mov	rdx,[rbp-18]
	mov	rsi,rdx
	mov	rdi,rax
	call	0000000000400C90
	mov	rdx,[rbp-30]
	mov	rax,[rbp-38]
	mov	rsi,rdx
	mov	rdi,rax
	call	0000000000400C10
	mov	rdx,[rbp-30]
	mov	rax,[rbp-38]
	mov	rsi,rdx
	mov	rdi,rax
	call	0000000000400BE0
	mov	rdx,rax
	mov	rax,[rbp-38]
	mov	rsi,rdx
	mov	rdi,rax
	call	0000000000400C20
	mov	[rbp-40],rax
	mov	rax,[rbp-40]
	mov	[rbp-10],rax
	mov	rsi,[rbp-30]
	mov	rax,[rbp-38]
	lea	r9,[rbp-74]
	lea	r8,[rbp-78]
	lea	rcx,[rbp-7C]
	lea	rdx,[rbp-80]
	sub	rsp,08
	lea	rdi,[rbp-68]
	push	rdi
	lea	rdi,[rbp-6C]
	push	rdi
	lea	rdi,[rbp-70]
	push	rdi
	mov	rdi,rax
	call	0000000000400C50
	add	rsp,20
	test	eax,eax
	jz	00000000004016AF

l000000000040147A:
	mov	eax,[rbp-70]
	test	eax,eax
	jz	0000000000401488

l0000000000401481:
	cmp	eax,01
	jz	0000000000401491

l0000000000401486:
	jmp	000000000040149A

l0000000000401488:
	mov	dword ptr [rbp-64],00000001
	jmp	00000000004014AB

l0000000000401491:
	mov	dword ptr [rbp-64],00000007
	jmp	00000000004014AB

l000000000040149A:
	mov	rax,[rbp-38]
	mov	esi,0040199A
	mov	rdi,rax
	call	0000000000400B50

l00000000004014AB:
	mov	rax,[rbp-38]
	mov	rdi,rax
	call	0000000000400B60
	mov	dword ptr [rbp-60],00000000
	jmp	0000000000401673

l00000000004014C3:
	mov	eax,[rbp-70]
	cmp	eax,01
	jnz	00000000004015CE

l00000000004014CF:
	cmp	dword ptr [rbp-60],01
	jle	00000000004014EF

l00000000004014D5:
	mov	eax,00000007
	sub	eax,[rbp-60]
	sar	eax,01
	mov	edx,00000001
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	sub	eax,01
	jmp	00000000004014F4

l00000000004014EF:
	mov	eax,00000007

l00000000004014F4:
	mov	edx,[rbp-60]
	mov	esi,edx
	and	esi,01
	mov	edx,[rbp-60]
	add	edx,01
	sar	edx,01
	mov	ecx,00000003
	sub	ecx,edx
	mov	edx,ecx
	mov	ecx,edx
	shl	esi,cl
	mov	edx,esi
	and	edx,07
	sub	eax,edx
	mov	edx,eax
	mov	eax,[rbp-80]
	add	edx,eax
	cmp	dword ptr [rbp-60],01
	jle	0000000000401531

l0000000000401525:
	mov	eax,00000007
	sub	eax,[rbp-60]
	sar	eax,01
	jmp	0000000000401536

l0000000000401531:
	mov	eax,00000003

l0000000000401536:
	mov	ecx,eax
	shr	edx,cl
	mov	eax,edx
	test	eax,eax
	jz	000000000040166E

l0000000000401544:
	mov	eax,[rbp-60]
	and	eax,01
	mov	edx,eax
	mov	eax,[rbp-60]
	add	eax,01
	sar	eax,01
	mov	ecx,00000003
	sub	ecx,eax
	mov	eax,ecx
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	and	eax,07
	mov	[rbp-58],eax
	mov	eax,[rbp-60]
	and	eax,01
	test	eax,eax
	setz	al
	movzx	edx,al
	mov	eax,[rbp-60]
	sar	eax,01
	mov	ecx,00000003
	sub	ecx,eax
	mov	eax,ecx
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	and	eax,07
	mov	[rbp-5C],eax
	mov	eax,00000007
	sub	eax,[rbp-60]
	sar	eax,01
	mov	edx,00000001
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	mov	[rbp-50],eax
	cmp	dword ptr [rbp-60],02
	jle	00000000004015C4

l00000000004015AF:
	mov	eax,[rbp-60]
	sub	eax,01
	sar	eax,01
	mov	edx,00000008
	mov	ecx,eax
	sar	edx,cl
	mov	eax,edx
	jmp	00000000004015C9

l00000000004015C4:
	mov	eax,00000008

l00000000004015C9:
	mov	[rbp-54],eax
	jmp	00000000004015E8

l00000000004015CE:
	mov	dword ptr [rbp-58],00000000
	mov	eax,[rbp-58]
	mov	[rbp-5C],eax
	mov	dword ptr [rbp-50],00000001
	mov	eax,[rbp-50]
	mov	[rbp-54],eax

l00000000004015E8:
	mov	eax,[rbp-5C]
	mov	[rbp-4C],eax
	jmp	0000000000401664

l00000000004015F0:
	mov	edi,004019B6
	call	0000000000400B20
	mov	rax,[rbp-38]
	mov	rcx,[rbp-10]
	mov	edx,00000000
	mov	rsi,rcx
	mov	rdi,rax
	call	0000000000400C00
	mov	eax,[rbp-4C]
	cmp	rax,[rbp-20]
	jnz	000000000040165E

l000000000040161B:
	mov	eax,[rbp-58]
	mov	[rbp-48],eax
	mov	dword ptr [rbp-44],00000000
	jmp	0000000000401656

l000000000040162A:
	mov	eax,[rbp-48]
	cmp	rax,[rbp-28]
	jnz	000000000040164C

l0000000000401633:
	mov	rsi,[rbp-30]
	mov	rax,[rbp-38]
	mov	ecx,[rbp-44]
	mov	rdx,[rbp-10]
	mov	rdi,rax
	call	0000000000400EE9
	jmp	000000000040167F

l000000000040164C:
	mov	eax,[rbp-50]
	add	[rbp-48],eax
	add	dword ptr [rbp-44],01

l0000000000401656:
	mov	eax,[rbp-80]
	cmp	[rbp-48],eax
	jc	000000000040162A

l000000000040165E:
	mov	eax,[rbp-54]
	add	[rbp-4C],eax

l0000000000401664:
	mov	eax,[rbp-7C]
	cmp	[rbp-4C],eax
	jc	00000000004015F0

l000000000040166C:
	jmp	000000000040166F

l000000000040166E:
	nop

l000000000040166F:
	add	dword ptr [rbp-60],01

l0000000000401673:
	mov	eax,[rbp-60]
	cmp	eax,[rbp-64]
	jl	00000000004014C3

l000000000040167F:
	mov	qword ptr [rbp-40],+00000000
	mov	rax,[rbp-38]
	mov	rdx,[rbp-10]
	mov	rsi,rdx
	mov	rdi,rax
	call	0000000000400BA0
	mov	rax,[rbp-38]
	lea	rdx,[rbp-30]
	mov	rsi,rdx
	mov	rdi,rax
	call	0000000000400C60
	jmp	00000000004016DE

l00000000004016AF:
	mov	rax,[rbp-38]
	mov	esi,004019C3
	mov	rdi,rax
	call	0000000000400B50

l00000000004016C0:
	mov	rax,[0000000000602100]                                 ; [rip+00200A39]
	mov	rcx,rax
	mov	edx,0000002C
	mov	esi,00000001
	mov	edi,004019E8
	call	0000000000400C80

l00000000004016DE:
	lea	rax,[rbp-38]
	mov	edx,00000000
	mov	esi,00000000
	mov	rdi,rax
	call	0000000000400B90
	jmp	000000000040175D

l00000000004016F6:
	mov	rax,[0000000000602100]                                 ; [rip+00200A03]
	mov	rcx,rax
	mov	edx,0000002E
	mov	esi,00000001
	mov	edi,00401A18
	call	0000000000400C80
	jmp	000000000040175D

l0000000000401716:
	mov	rax,[rbp-000000A0]
	add	rax,18
	mov	rdx,[rax]
	mov	rax,[0000000000602100]                                 ; [rip+002009D5]
	mov	esi,00401A48
	mov	rdi,rax
	mov	eax,00000000
	call	0000000000400BF0
	jmp	000000000040175D

l000000000040173F:
	mov	rax,[0000000000602100]                                 ; [rip+002009BA]
	mov	rcx,rax
	mov	edx,00000027
	mov	esi,00000001
	mov	edi,00401A70
	call	0000000000400C80

l000000000040175D:
	mov	eax,[rbp-00000084]
	mov	rdi,[rbp-08]
	xor	rdi,fs:[00000028]
	jz	0000000000401777

l0000000000401772:
	call	0000000000400B40

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
	sub	rsp,08
	sar	rbp,03
	call	0000000000400AE8
	test	rbp,rbp
	jz	00000000004017D6

l00000000004017B6:
	xor	ebx,ebx
	nop	dword ptr [rax+rax+00000000]

l00000000004017C0:
	mov	rdx,r13
	mov	rsi,r14
	mov	edi,r15d
	call	qword ptr [r12+rbx*8]
	add	rbx,01
	cmp	rbx,rbp
	jnz	00000000004017C0

l00000000004017D6:
	add	rsp,08
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
