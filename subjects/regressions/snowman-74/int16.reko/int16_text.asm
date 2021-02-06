;;; Segment .text (10071000)

;; get: 10071000
get proc
	mov	eax,[ecx]
	add	eax,[esp+4h]
	ret	4h
10071009                            CC CC CC CC CC CC CC          .......

;; DllMain: 10071010
DllMain proc
	mov	eax,1h
	ret	0Ch
