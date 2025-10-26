;;; Segment .init (00002558)

;; fn00002558: 00002558
;;   Called from:
;;     00002660 (in fn00002570)
fn00002558 proc
	subu	r31,r31,0x10
	st	r1,r31,0x0
	bsr	fn00002810
	ld	r1,r31,0x0
	jmp.n	r1
	addu	r31,r31,0x10
