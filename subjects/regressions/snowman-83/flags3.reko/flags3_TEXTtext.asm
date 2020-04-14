;;; Segment __TEXT,__text (0000000000000FA8)

;; foo: 0000000000000FA8
foo proc
	mov	eax,[rdi]
	cmp	eax,00000101
	mov	ecx,00000100
	cmovge	eax,ecx

l0000000000000FB7:
	ret
