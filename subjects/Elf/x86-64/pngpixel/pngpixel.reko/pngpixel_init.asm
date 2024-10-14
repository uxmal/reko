;;; Segment .init (0000000000400AE8)

;; _init: 0000000000400AE8
;;   Called from:
;;     00000000004017AC (in __libc_csu_init)
_init proc
	sub	rsp,8h
	mov	rax,[__gmon_start___GOT]                               ; [rip+00201505]
	test	rax,rax
	jz	400AFDh

l0000000000400AF8:
	call	400CC0h

l0000000000400AFD:
	add	rsp,8h
	ret
