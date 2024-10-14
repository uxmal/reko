;;; Segment .init (0000000000400428)

;; _init: 0000000000400428
;;   Called from:
;;     000000000040073C (in __libc_csu_init)
_init proc
	sub	rsp,8h
	mov	rax,[__gmon_start___GOT]                               ; [rip+00200BC5]
	test	rax,rax
	jz	40043Ah

l0000000000400438:
	call	rax

l000000000040043A:
	add	rsp,8h
	ret
