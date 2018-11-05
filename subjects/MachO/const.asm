;;; Segment __TEXT,__text (00000000)

;; fn00000000: 00000000
fn00000000 proc
	push	ebp
	mov	ebp,esp
	call	00000008

;; fn00000004: 00000004
fn00000004 proc
	add	[eax],al
	add	[eax],al
	pop	eax
	mov	eax,[eax+0000000B]
	mov	eax,[eax]
	pop	ebp
	ret
;;; Segment __IMPORT,__pointers (00000013)
__imp___f		; 00000013
	dd	0x00000000
