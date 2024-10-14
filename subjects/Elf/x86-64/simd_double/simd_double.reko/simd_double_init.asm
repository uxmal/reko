;;; Segment .init (00000000000005A0)

;; _init: 00000000000005A0
;;   Called from:
;;     0000000000000A9C (in __libc_csu_init)
_init proc
	sub	rsp,8h
	mov	rax,[__gmon_start___GOT]                               ; [rip+00200A3D]
	test	rax,rax
	jz	05B2h

l00000000000005B0:
	call	rax

l00000000000005B2:
	add	rsp,8h
	ret
