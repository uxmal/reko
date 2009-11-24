;;; Different encodings of TEST

foo proc

	test ax,bx
	test ax,3
	test al,cl
	test al,3
	test ax,[si]
	test al,[si]
	test byte ptr [0x300],3
	test word ptr [0x302],5
	ret
foo endp

	