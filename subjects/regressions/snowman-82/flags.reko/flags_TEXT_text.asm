;;; Segment __TEXT,__text (0000000000000F9E)

;; foo: 0000000000000F9E
foo proc
	mov	ecx,[rdi]
	shr	ecx,0Ah
	xor	cl,sil
	jz	bar

l0000000000000FAC:
	mov	rax,+1h
	ret

;; bar: 0000000000000FB4
;;   Called from:
;;     0000000000000FA6 (in foo)
bar proc
	xor	rax,rax
	ret
