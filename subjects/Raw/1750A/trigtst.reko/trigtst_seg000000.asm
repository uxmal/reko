;;; Segment seg000000 (00000000)

;;; ...end of image

;; fn0100: 0100
fn0100 proc
	lim	gp0,#0x8000
	lim	gp1,#0xE9
	lim	gp2,#0x4C2
	mov	gp0,gp2
	lim	gp0,#0x80E9
	lim	gp1,#0x53
	lim	gp2,#0x5B7
	mov	gp0,gp2
	sjs	gp15,0111

l00000110:
	br	00000110

;; fn0111: 0111
;;   Called from:
;;     0000010E (in fn0100)
fn0111 proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lim	gp0,#0x8000
	sjs	gp15,04B9
	efl	gp0,0x80CE
	sjs	gp15,01E6
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x800D
	sjs	gp15,04B9
	efl	gp0,0x80D1
	sjs	gp15,01E6
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x801A
	sjs	gp15,04B9
	efl	gp0,0x80D4
	sjs	gp15,01E6
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x8027
	sjs	gp15,04B9
	efl	gp0,0x80D7
	sjs	gp15,01E6
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x8036
	sjs	gp15,04B9
	efl	gp0,0x80DA
	sjs	gp15,032A
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x8042
	sjs	gp15,04B9
	efl	gp0,0x80DD
	sjs	gp15,032A
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x804F
	sjs	gp15,04B9
	efl	gp0,0x80E0
	sjs	gp15,032A
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x805C
	sjs	gp15,04B9
	efl	gp0,0x80E3
	sjs	gp15,032A
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x806B
	sjs	gp15,04B9
	efl	gp0,0x80E6
	sjs	gp15,032A
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x8076
	sjs	gp15,04B9
	efl	gp0,0x80CE
	sjs	gp15,032A
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x8082
	sjs	gp15,04B9
	efl	gp0,0x80DA
	sjs	gp15,034E
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x808E
	sjs	gp15,04B9
	efl	gp0,0x80DD
	sjs	gp15,034E
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x809B
	sjs	gp15,04B9
	efl	gp0,0x80E0
	sjs	gp15,034E
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x80A8
	sjs	gp15,04B9
	efl	gp0,0x80E3
	sjs	gp15,034E
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x80B7
	sjs	gp15,04B9
	efl	gp0,0x80E6
	sjs	gp15,034E
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lim	gp0,#0x80C2
	sjs	gp15,04B9
	efl	gp0,0x80CE
	sjs	gp15,034E
	sjs	gp15,04AE
	lisp	gp0,#0xA
	sjs	gp15,045A
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; fn01E6: 01E6
;;   Called from:
;;     00000119 (in fn0111)
;;     00000126 (in fn0111)
;;     00000133 (in fn0111)
;;     00000140 (in fn0111)
fn01E6 proc
	dlr	gp3,gp0
	lr	gp5,gp2
	efc	gp0,0x5AB
	bnz	000001EC

l000001EB:
	urs	gp15

l000001EC:
	bgt	000001EE

l000001ED:
	bpt

l000001EE:
	lr	gp6,gp4
	xbr	gp6
	sra	gp6,#8
	lim	gp13,#0xFF00
	lim	gp14,#0xFF
	andr	gp4,gp13
	lr	gp1,gp4
	efm	gp0,0x5AE
	efa	gp0,0x5B1
	dlr	gp7,gp3
	lr	gp9,gp5
	efdr	gp7,gp0
	efar	gp0,gp7
	dlr	gp10,gp3
	lr	gp12,gp5
	efdr	gp3,gp0
	pshm	gp2,gp2
	lr	gp2,gp1
	andr	gp1,gp13
	lim	gp2,#0xFFFE,gp2
	andr	gp2,gp14
	orr	gp1,gp2
	popm	gp2,gp2
	efar	gp0,gp3
	efdr	gp10,gp0
	efar	gp0,gp10
	lr	gp3,gp1
	andr	gp1,gp13
	lim	gp3,#0xFFFF,gp3
	andr	gp3,gp14
	orr	gp1,gp3
	tbr	#0xF,gp6
	bez	00000218

l00000215:
	efm	gp0,0x5B4
	aisp	gp6,#1

l00000218:
	disp	gp6,#2
	lr	gp3,gp1
	andr	gp1,gp13
	xbr	gp3
	sra	gp3,#8
	ar	gp3,gp6
	andr	gp3,gp14
	orr	gp1,gp3
	urs	gp15

;;; ...end of image

;; fn02BF: 02BF
;;   Called from:
;;     00000349 (in fn032A)
;;     00000368 (in fn034E)
fn02BF proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp7,gp4
	dlr	gp5,gp2
	lr	gp0,gp1
	lr	gp2,gp0
	jc	#6,00000325

l000002C6:
	lb	gp14,#0xC8
	aisp	gp2,#7
	andm	gp2,#0xFFF8

l000002CA:
	lr	gp1,gp0
	sr	gp1,gp2
	pshm	gp1,gp4
	lr	gp2,gp1
	xorr	gp1,gp1
	xorr	gp3,gp3
	lim	gp4,#7
	dcr	gp1,gp3
	popm	gp1,gp4

l000002D4:
	jc	#4,00000314

l000002D5:
	lb	gp15,#0x23
	l	gp2,0x2DA,gp1
	jc	#0xF,000002CA

;;; ...end of image

l000002E4:
	jc	#0xF,000002D4

;;; ...end of image

l000002F4:
	jc	#0xF,000002E4

;;; ...end of image

l00000304:
	jc	#0xF,000002F4

;;; ...end of image

l00000314:
	jc	#0xF,00000304

;;; ...end of image

l00000325:
	lr	gp2,gp5
	dlr	gp0,gp3
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; fn032A: 032A
;;   Called from:
;;     0000014D (in fn0111)
;;     0000015A (in fn0111)
;;     00000167 (in fn0111)
;;     00000174 (in fn0111)
;;     00000181 (in fn0111)
;;     0000018E (in fn0111)
fn032A proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp10,gp2
	dlr	gp8,gp0
	lr	gp7,gp10
	dlr	gp5,gp8
	jc	#6,00000390

l00000331:
	lb	gp15,#0x37
	efl	gp2,0x8122
	lr	gp7,gp4
	dlr	gp5,gp2
	efsr	gp5,gp8
	efm	gp5,0x8125
	efix	gp0,gp5
	eflt	gp2,gp0
	efsr	gp5,gp2
	lr	gp4,gp7
	dlr	gp2,gp5
	jc	#6,0000039E

l0000033F:
	lb	gp15,#0x44
	efa	gp2,0x8128
	da	gp0,0x812B
	dlr	gp8,gp8
	jc	#6,000003A5

l00000346:
	lb	gp15,#0x49
	da	gp0,0x812D
	sjs	gp15,02BF
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; fn034E: 034E
;;   Called from:
;;     0000019B (in fn0111)
;;     000001A8 (in fn0111)
;;     000001B5 (in fn0111)
;;     000001C2 (in fn0111)
;;     000001CF (in fn0111)
;;     000001DC (in fn0111)
fn034E proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp7,gp2
	dlr	gp5,gp0
	jc	#6,000003B2

l00000353:
	lb	gp15,#0x59
	efl	gp2,0x812F
	efsr	gp2,gp5
	lr	gp7,gp4
	dlr	gp5,gp2
	efm	gp5,0x8132
	efix	gp0,gp5
	eflt	gp2,gp0
	efsr	gp5,gp2
	lr	gp4,gp7
	dlr	gp2,gp5
	jc	#6,000003C0

l00000361:
	lb	gp15,#0x66
	efa	gp2,0x8135
	da	gp0,0x8138
	da	gp0,0x813A
	sjs	gp15,02BF
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;;; ...end of image

l00000390:
	jc	#5,000003E0

l00000391:
	lb	gp15,#0x82
	tbr	#0xF,gp9
	bez	00000397

l00000394:
	stub	gp10,2,gp11
	br	00000399

l00000397:
	stlb	gp10,2,gp11

l00000399:
	popm	gp0,gp13
	urs	gp15

;;; ...end of image

l0000039E:
	fab	gp12,#0x20
	lisp	gp9,#6

l03A0:
	st	gp10,gp11
	aisp	gp11,#1
	soj	gp9,03A0

l000003A5:
	sisp	gp11,#1
	stlb	gp9,gp11
	lim	gp10,#0x20
	orr	gp1,gp1
	bge	000003B5

l000003AC:
	lim	gp10,#0x2D
	cim	gp1,#0x8000
	bnz	000003B4

l000003B1:
	lr	gp2,gp2

;; fn000003B2: 000003B2
;;   Called from:
;;     00000352 (in fn034E)
;;     000003B1 (in fn032A)
fn000003B2 proc
	bnz	000003B4

;; fn000003B3: 000003B3
;;   Called from:
;;     000003B2 (in fn000003B2)
;;     000003B2 (in fn000003B2)
fn000003B3 proc
	aisp	gp2,#1

;; fn000003B4: 000003B4
;;   Called from:
;;     000003AB (in fn032A)
;;     000003B0 (in fn032A)
;;     000003B2 (in fn000003B2)
;;     000003B2 (in fn000003B2)
;;     000003B3 (in fn000003B3)
fn000003B4 proc
	dneg	gp1,gp1

l000003B5:
	xorr	gp7,gp7
	lisp	gp8,#0xA
	aisp	gp9,#1
	dlr	gp4,gp1
	dlr	gp2,gp1
	ddr	gp2,gp7
	dmr	gp2,gp7
	dsr	gp4,gp2
	aim	gp5,#0x30
	tbr	#0xF,gp9

;; fn000003C0: 000003C0
;;   Called from:
;;     00000360 (in fn034E)
;;     000003BF (in fn000003B4)
fn000003C0 proc
	bez	000003C5

;; fn000003C1: 000003C1
;;   Called from:
;;     000003C0 (in fn000003C0)
;;     000003C0 (in fn000003C0)
fn000003C1 proc
	stub	gp5,gp11
	sisp	gp11,#1
	br	000003C7

;; fn000003C5: 000003C5
;;   Called from:
;;     000003C0 (in fn000003C0)
;;     000003C0 (in fn000003C0)
;;     000003C4 (in fn000003C1)
fn000003C5 proc
	stlb	gp5,gp11

l000003C7:
	ddr	gp12,gp7
	jc	#5,00000418

l000003C9:
	lb	gp15,#0xB7
	aisp	gp9,#1
	tbr	#0xF,gp9
	bnz	000003D0

l000003CD:
	stlb	gp10,gp11
	br	000003D2

l000003D0:
	stub	gp10,gp11

l000003D2:
	popm	gp0,gp12
	urs	gp15

;;; ...end of image

;; fn03E0: 03E0
;;   Called from:
;;     00000390 (in fn032A)
;;     000004B3 (in fn04AE)
fn03E0 proc
	pshm	gp0,gp14
	lr	gp11,gp0
	lr	gp14,gp3
	dlr	gp12,gp1
	xorr	gp0,gp0
	st	gp0,0xA,gp11
	lisp	gp1,#0xB
	sjs	gp15,03EC
	popm	gp0,gp14
	urs	gp15

;; fn03EC: 03EC
;;   Called from:
;;     000003E8 (in fn03E0)
fn03EC proc
	tbr	#0,gp12
	bez	000003FE

l000003EE:
	tbr	#1,gp12
	bnz	000003F5

l000003F0:
	lim	gp0,#0x2D20
	efm	gp12,0x610
	br	00000406

l000003F5:
	lim	gp10,#0x613
	lisp	gp12,#7
	cisp	gp1,#6
	bez	000003FB

l000003FA:
	lisp	gp12,#0xA

l000003FB:
	mov	gp11,gp10
	jc	#0xF,000003EC

;;; ...end of image

l000003FE:
	orr	gp0,gp12
	orr	gp0,gp13
	orr	gp0,gp14
	bez	00000404

l00000402:
	tbr	#1,gp12
	bez	000003F5

l00000404:
	lim	gp0,#0x2B20

l00000406:
	st	gp0,gp11
	xorr	gp2,gp2
	tbr	#8,gp13
	bnz	00000415

l0000040B:
	efc	gp12,0x60A
	blt	00000412

l0000040E:
	efd	gp12,0x60A
	aisp	gp2,#1
	br	0000040B

l00000412:
	lim	gp0,#0x452B
	br	0000041F

l00000415:
	efc	gp12,0x60D
	jc	#6,00000477

;; fn00000418: 00000418
;;   Called from:
;;     000003C8 (in fn000003C5)
;;     0000040A (in fn03EC)
fn00000418 proc
	dlb	gp12,#0x1D
	efm	gp12,0x60A
	aisp	gp2,#1
	br	00000415

;;; ...end of image

l0000041F:
	cisp	gp1,#6
	bez	00000424

l00000421:
	st	gp0,8,gp11
	br	00000426

l00000424:
	st	gp0,5,gp11

l00000426:
	disp	gp2,#0xA
	xbr	gp2
	orr	gp2,gp3
	orim	gp2,#0x3030
	cisp	gp1,#6
	bez	00000430

l0000042D:
	st	gp2,9,gp11
	br	00000432

l00000430:
	st	gp2,6,gp11

l00000432:
	xorr	gp7,gp7
	xorr	gp2,gp2
	dlr	gp4,gp12
	lr	gp6,gp14
	efix	gp2,gp4
	eflt	gp4,gp2
	efsr	gp12,gp4
	aim	gp3,#0x30
	stlb	gp3,gp11
	lim	gp3,#0x2E
	stub	gp3,1,gp11

l0441:
	efm	gp12,0x60A
	dlr	gp4,gp12
	lr	gp6,gp14
	efix	gp2,gp4
	eflt	gp4,gp2
	efsr	gp12,gp4
	aim	gp3,#0x30
	aisp	gp7,#1
	tbr	#0xF,gp7
	bnz	00000450

l0000044D:
	stub	gp3,1,gp11
	br	00000453

l00000450:
	stlb	gp3,1,gp11
	aisp	gp11,#1

l00000453:
	soj	gp1,0441

l00000455:
	lim	gp3,#0x2020
	st	gp3,1,gp11
	urs	gp15

;; fn045A: 045A
;;   Called from:
;;     0000011E (in fn0111)
;;     0000012B (in fn0111)
;;     00000138 (in fn0111)
;;     00000145 (in fn0111)
;;     00000152 (in fn0111)
;;     0000015F (in fn0111)
;;     0000016C (in fn0111)
;;     00000179 (in fn0111)
;;     00000186 (in fn0111)
;;     00000193 (in fn0111)
;;     000001A0 (in fn0111)
;;     000001AD (in fn0111)
;;     000001BA (in fn0111)
;;     000001C7 (in fn0111)
;;     000001D4 (in fn0111)
;;     000001E1 (in fn0111)
;;     00000463 (in fn045D)
;;     00000468 (in fn045D)
;;     00000478 (in fn00000418)
;;     000004BD (in fn04B9)
fn045A proc
	co	gp0
	urs	gp15

;; fn045D: 045D
;;   Called from:
;;     000004B5 (in fn04AE)
fn045D proc
	pshm	gp9,gp12
	lr	gp11,gp0
	xorr	gp0,gp0

l00000460:
	lub	gp0,gp11
	bez	0000046C

l00000463:
	sjs	gp15,045A
	llb	gp0,gp11
	bez	0000046C

l00000468:
	sjs	gp15,045A
	aisp	gp11,#1
	br	00000460

l0000046C:
	popm	gp9,gp12
	urs	gp15

;;; ...end of image

l00000477:
	lb	gp12,#0x37
	sjs	gp15,045A
	popm	gp0,gp0
	urs	gp15

;;; ...end of image

;; fn04AE: 04AE
;;   Called from:
;;     0000011B (in fn0111)
;;     00000128 (in fn0111)
;;     00000135 (in fn0111)
;;     00000142 (in fn0111)
;;     0000014F (in fn0111)
;;     0000015C (in fn0111)
;;     00000169 (in fn0111)
;;     00000176 (in fn0111)
;;     00000183 (in fn0111)
;;     00000190 (in fn0111)
;;     0000019D (in fn0111)
;;     000001AA (in fn0111)
;;     000001B7 (in fn0111)
;;     000001C4 (in fn0111)
;;     000001D1 (in fn0111)
;;     000001DE (in fn0111)
fn04AE proc
	pshm	gp0,gp3
	lr	gp3,gp2
	dlr	gp1,gp0
	lim	gp0,#0x813C
	sjs	gp15,03E0
	sjs	gp15,045D
	popm	gp0,gp3
	urs	gp15

;; fn04B9: 04B9
;;   Called from:
;;     00000115 (in fn0111)
;;     00000122 (in fn0111)
;;     0000012F (in fn0111)
;;     0000013C (in fn0111)
;;     00000149 (in fn0111)
;;     00000156 (in fn0111)
;;     00000163 (in fn0111)
;;     00000170 (in fn0111)
;;     0000017D (in fn0111)
;;     0000018A (in fn0111)
;;     00000197 (in fn0111)
;;     000001A4 (in fn0111)
;;     000001B1 (in fn0111)
;;     000001BE (in fn0111)
;;     000001CB (in fn0111)
;;     000001D8 (in fn0111)
fn04B9 proc
	lr	gp1,gp0

l000004BA:
	l	gp0,gp1
	bez	000004C1

l000004BD:
	sjs	gp15,045A
	aisp	gp1,#1
	br	000004BA

l000004C1:
	urs	gp15

;;; ...end of image
