;;; Segment .init (0000000000402168)

;; _init: 0000000000402168
_init proc
	sub	rsp,08
	mov	rax,[0000000000619FF8]                                 ; [rip+00217E85]
	test	rax,rax
	jz	000000000040217D

l0000000000402178:
	call	00000000004025B0

l000000000040217D:
	add	rsp,08
	ret
