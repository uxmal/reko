;;; Segment .init (0000000000400428)

;; _init: 0000000000400428
;;   Called from:
;;     000000000040073C (in __libc_csu_init)
_init proc
	sub	rsp,08
	mov	rax,[0000000000600FF8]                                 ; [rip+00200BC5]
	test	rax,rax
	jz	000000000040043A

l0000000000400438:
	call	rax

l000000000040043A:
	add	rsp,08
	ret
