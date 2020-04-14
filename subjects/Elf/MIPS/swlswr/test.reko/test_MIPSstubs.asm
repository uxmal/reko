;;; Segment .MIPS.stubs (000009D0)

;; __libc_start_main: 000009D0
__libc_start_main proc
	lw	r25,-7FF0(r28)
	or	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000000C

;; memset: 000009E0
memset proc
	lw	r25,-7FF0(r28)
	or	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000000B

;; calloc: 000009F0
calloc proc
	lw	r25,-7FF0(r28)
	or	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000009
	nop
	nop
	nop
	nop
