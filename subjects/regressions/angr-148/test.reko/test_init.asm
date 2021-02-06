;;; Segment .init (00000000004003E0)

;; _init: 00000000004003E0
;;   Called from:
;;     000000000040057E (in __libc_csu_init)
_init proc
	sub	rsp,8h
	mov	rax,[0000000000600FF8]                                 ; [rip+00200C0D]
	test	rax,rax
	jz	4003F5h

l00000000004003F0:
	call	400430h

l00000000004003F5:
	add	rsp,8h
	ret
