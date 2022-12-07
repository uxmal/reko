;;; Segment .init (00000000004003C8)

;; _init: 00000000004003C8
;;   Called from:
;;     0000000000400660 (in __libc_csu_init)
_init proc
	sub	rsp,8h
	call	call_gmon_start
	add	rsp,8h
	ret
