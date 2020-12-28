;;; Segment CODE_00 (0000)

;; fn0000: 0000
fn0000 proc
	ljmp	002C
;;; Segment CODE_01 (0003)

;; fn0003: 0003
;;   Called from:
;;     0022 (in fn0000)
;;     0027 (in fn0000)
fn0003 proc
	clr	A
	mov	R7,A
	mov	R6,A

l0006:
	clr	A
	mov	R5,A
	mov	R4,A

l0009:
	inc	R5
	cjne	R5,00,000E

l000D:
	inc	R4

l000E:
	cjne	R4,27,0009

l0011:
	cjne	R5,10,0009

l0014:
	inc	R7
	cjne	R7,00,0019

l0018:
	inc	R6

l0019:
	mov	A,R7
	xrl	A,0A
	orl	A,R6
	jnz	0006

l001F:
	ret
;;; Segment CODE_02 (0020)

l0020:
	setb	P2.0
	lcall	0003
	clr	P2.0
	lcall	0003
	sjmp	0020

l002C:
	mov	R0,7F
	clr	A

l002F:
	mov	@R0,A
	djnz	R0,002F

l0032:
	mov	[0081],07
	ljmp	0020
