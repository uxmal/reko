;;; Segment __TEXT,__text (00000000)
00000000 55 89 E5 E8                                     U...            

;; f: 00000004
f proc
	add	[eax],al
	add	[eax],al
	pop	eax
	mov	eax,[eax+0Bh]
	mov	eax,[eax]
	pop	ebp
	ret
