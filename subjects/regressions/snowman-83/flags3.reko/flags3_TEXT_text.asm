;;; Segment __TEXT,__text (0000000000000FA8)

;; foo: 0000000000000FA8
foo proc
	mov	eax,[rdi]
	cmp	eax,101h
	mov	ecx,100h
	cmovge	eax,ecx

l0000000000000FB7:
	ret
