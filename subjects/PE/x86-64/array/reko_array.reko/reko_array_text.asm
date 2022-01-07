;;; Segment .text (00000002121D1000)

;; reko_array_byref: 00000002121D1000
;;   Called from:
;;     00000002121D104E (in reko_array_01)
reko_array_byref proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,10h
	mov	[rbp+10h],rcx
	mov	dword ptr [rbp-4h],0h
	jmp	2121D102Bh

l00000002121D1015:
	mov	eax,[rbp-4h]
	movsxd	rdx,eax
	mov	rax,[rbp+10h]
	add	rax,rdx
	mov	edx,[rbp-4h]
	mov	[rax],dl
	add	dword ptr [rbp-4h],1h

l00000002121D102B:
	cmp	dword ptr [rbp-4h],1Fh
	jle	2121D1015h

l00000002121D1031:
	mov	eax,0h
	add	rsp,10h
	pop	rbp
	ret

;; reko_array_01: 00000002121D103C
reko_array_01 proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,20h
	lea	rax,[00000002121D5000]                                 ; [rip+00003FB5]
	mov	rcx,rax
	call	2121D1000h
	mov	eax,0h
	add	rsp,20h
	pop	rbp
	ret

;; reko_array_local: 00000002121D105E
reko_array_local proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,30h
	mov	dword ptr [rbp-4h],0h
	jmp	2121D1081h

l00000002121D106F:
	mov	eax,[rbp-4h]
	mov	edx,eax
	mov	eax,[rbp-4h]
	cdqe
	mov	[rbp+rax-30h],dl
	add	dword ptr [rbp-4h],1h

l00000002121D1081:
	mov	eax,[rbp-4h]
	cmp	eax,1Fh
	jbe	2121D106Fh

l00000002121D1089:
	mov	eax,0h
	add	rsp,30h
	pop	rbp
	ret
00000002121D1094             90 90 90 90 90 90 90 90 90 90 90 90     ............
00000002121D10A0 FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 ................
00000002121D10B0 FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 ................
