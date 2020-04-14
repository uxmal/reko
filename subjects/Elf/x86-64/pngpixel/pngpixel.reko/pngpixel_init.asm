;;; Segment .init (0000000000400AE8)

;; _init: 0000000000400AE8
;;   Called from:
;;     00000000004017AC (in __libc_csu_init)
_init proc
	sub	rsp,08
	mov	rax,[0000000000601FF8]                                 ; [rip+00201505]
	test	rax,rax
	jz	0000000000400AFD

l0000000000400AF8:
	call	0000000000400CC0

l0000000000400AFD:
	add	rsp,08
	ret
