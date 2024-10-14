;;; Segment .init (0000000000402168)

;; _init: 0000000000402168
_init proc
	sub	rsp,8h
	mov	rax,[__gmon_start___GOT]                               ; [rip+00217E85]
	test	rax,rax
	jz	40217Dh

l0000000000402178:
	call	4025B0h

l000000000040217D:
	add	rsp,8h
	ret
