;;; Segment .text (10071000)

;; get: 10071000
get proc
	mov	eax,[ecx]
	add	eax,[esp+04]
	ret	0004
10071009                            CC CC CC CC CC CC CC          .......

;; DllMain: 10071010
DllMain proc
	mov	eax,00000001
	ret	000C
