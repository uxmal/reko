;;; Segment seg000C1D6C (000C1D6C)

;; fn000C1D6C: 000C1D6C
;;   Called from:
;;     000C193F (in fn000C18E6)
fn000C1D6C proc
	enter	#8h
	mov.w	0Dh[fb],r0
	mov.w	5h[fb],-4h[fb]
	mov.w	7h[fb],-2h[fb]
	mov.w	9h[fb],-8h[fb]
	mov.w	0Bh[fb],-6h[fb]

l000C1D82:
	mov.w	r0,r1
	add.w:q	#0FFFFh,r0
	cmp.w:q	#0h,r1
	jeq	000C1DA8

l000C1D8A:
	mov.w	-8h[fb],a0
	mov.w	-6h[fb],a1
	add.w:q	#1h,-8h[fb]
	adcf.w	-6h[fb]
	lde.b	[a1a0],r1l
	mov.w	-4h[fb],a0
	mov.w	-2h[fb],a1
	ste.b	r1l,[a1a0]
	add.w:q	#1h,-4h[fb]
	adcf.w	-2h[fb]
	jmp.b	000C1D82

l000C1DA8:
	mov.w	5h[fb],r0
	mov.w	7h[fb],r2
	exitd

;; fn000C1DB0: 000C1DB0
;;   Called from:
;;     000C184E (in fn000C181E)
fn000C1DB0 proc
	enter	#4h
	mov.w	9h[fb],r0
	mov.w	5h[fb],-4h[fb]
	mov.w	7h[fb],-2h[fb]

l000C1DBE:
	mov.w	r0,r1
	add.w:q	#0FFFFh,r0
	cmp.w:q	#0h,r1
	jeq	000C1DDC

l000C1DC6:
	mov.w	r2,a0
	mov.b	a0,r1l
	mov.w	a0,r2
	mov.w	-4h[fb],a0
	mov.w	-2h[fb],a1
	ste.b	r1l,[a1a0]
	add.w:q	#1h,-4h[fb]
	adcf.w	-2h[fb]
	jmp.b	000C1DBE

l000C1DDC:
	mov.w	5h[fb],r0
	mov.w	7h[fb],r2
	exitd

;; fn000C1DE4: 000C1DE4
;;   Called from:
;;     000C1B01 (in fn000C1ACC)
fn000C1DE4 proc
	pushm	a1,a0,r3,r1
	mov.w	0Bh[sb],r1
	mov.w	0Dh[sb],r3
	mov.w:q	#0h,a0
	cmp.w	#8000h,r2
	jltu	000C1E00

l000C1DF4:
	xor.w	#1h,a0
	not.w	r2
	not.w	r0
	add.w:q	#1h,r0
	adcf.w	r2

l000C1E00:
	cmp.w	#8000h,r3
	jltu	000C1E12

l000C1E06:
	xor.w	#1h,a0
	not.w	r3
	not.w	r1
	add.w:q	#1h,r1
	adcf.w	r3

l000C1E12:
	push.w:s	a0
	cmp.w:q	#0h,r3
	jne	000C1E2D

l000C1E17:
	cmp.w:q	#0h,r2
	jeq	000C1E27

l000C1E1B:
	mov.w	r0,a1
	mov.w	r2,r0
	mov.w:q	#0h,r2
	divu.w	r1
	mov.w	r0,r3
	mov.w	a1,r0

l000C1E27:
	divu.w	r1
	mov.w	r3,r2
	jmp.b	000C1E55

l000C1E2D:
	mov.w:q	#0h,a0
	mov.w:q	#0h,a1

l000C1E31:
	cmp.w	r3,r2
	jleu	000C1E3C

l000C1E35:
	inc.w	a1
	shl:q	#0h,r1
	rolc.w	r3
	jpz	000C1E31

l000C1E3C:
	sub.w	r1,r0
	sbb.w	r3,r2
	jgeu	000C1E48

l000C1E42:
	add.w	r1,r0
	adc.w	r3,r2
	fclr	C

l000C1E48:
	rolc.w	a0
	shl:q	#-8h,r3
	rorc.w	r1
	dec.w	a1
	jpz	000C1E3C

l000C1E51:
	mov.w	a0,r0
	mov.w:q	#0h,r2

l000C1E55:
	mov.w	[sb],a0
	jeq	000C1E62

l000C1E5A:
	not.w	r0
	not.w	r2
	add.w:q	#1h,r0
	adcf.w	r2

l000C1E62:
	add:q	#2h,usp
	popm	a1,a0,r3,r1
	rts
