;;; Segment .init (0000000000001000)

;; _init: 0000000000001000
;;   Called from:
;;     00000000000012A8 (in __libc_csu_init)
_init proc
	sub	rsp,8h
	mov	rax,[__gmon_start___GOT]                               ; [rip+00002FDD]
	test	rax,rax
	jz	1012h

l0000000000001010:
	call	rax

l0000000000001012:
	add	rsp,8h
	ret
