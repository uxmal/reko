;;; Segment CODE_00 (0000)

;; fn0000: 0000
fn0000 proc
	nop
	clear	[0x1]
	mov	a,0xB3
	add	a,0xA
	and	a,0xFE
	mov	IO(0x2),a
	call	fn004F
	goto	fn0012
