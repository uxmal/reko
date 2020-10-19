;;; Segment normal (0115)

;; fn0115: 0115
;;   Called from:
;;     0135 (in fn011B)
fn0115 proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp3,gp0
	xorr	gp0,gp0
	lr	gp1,gp0
	lisp	gp4,#1

;; fn011B: 011B
;;   Called from:
;;     011A (in fn0115)
;;     011A (in fn0100)
fn011B proc
	lr	gp2,gp3
	neg	gp1,gp1
	sar	gp2,gp1
	andm	gp2,#1
	jc	#2,0140

l0121:
	lb	gp13,#0x28
	lisp	gp2,#3
	sr	gp2,gp1
	lr	gp5,gp4
	slr	gp5,gp2
	lr	gp2,gp5
	orr	gp0,gp2
	aisp	gp1,#1
	cisp	gp1,#3
	ble	011B

l012B:
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;;; ...end of image

l0132:
	lr	gp0,gp3
	st	gp3,1,gp14
	sjs	gp15,0115
	lr	gp1,gp0
	sll	gp1,#1
	ar	gp1,gp0
	l	gp3,1,gp14
	lr	gp2,gp3
	sll	gp2,#1
	ar	gp2,gp3
	efl	gp4,0x8000,gp2

l0140:
	l	gp0,0x9A41

l0141:
	efst	gp4,0x8030,gp1
	efl	gp4,0x8000
	efst	gp4,0x8060,gp1
	aisp	gp3,#1
	cisp	gp3,#0xF
	ble	0132

l014A:
	lr	gp15,gp14
	popm	gp14,gp14
	aisp	gp15,#1
	urs	gp15

;; fn014E: 014E
fn014E proc
	sisp	gp15,#2
	pshm	gp14,gp14
	lr	gp14,gp15
	xorr	gp9,gp9

l0152:
	lr	gp8,gp9
	sll	gp8,#1
	ar	gp8,gp9
	efl	gp5,0x8030,gp8
	efl	gp2,0x8060,gp8
	efmr	gp5,gp5
	efmr	gp2,gp2
	efar	gp5,gp2
	lr	gp2,gp7
	dlr	gp0,gp5
	dst	gp8,1,gp14
	sjs	gp15,0245
	l	gp8,1,gp14
	efst	gp0,0x8090,gp8
	l	gp9,2,gp14
	aisp	gp9,#1
	cisp	gp9,#0xF
	ble	0152

l016B:
	lr	gp15,gp14
	popm	gp14,gp14
	aisp	gp15,#2
	urs	gp15

;;; ...end of image

;; fn0226: 0226
;;   Called from:
;;     024E (in fn0245)
fn0226 proc
	l	gp5,gp3
	lr	gp4,gp1
	andm	gp1,#0xFF00
	andm	gp4,#0xFF
	tbr	#8,gp4
	bez	0231

l022F:
	orim	gp4,#0xFF00

l0231:
	tbr	#0xF,gp4
	bez	0236

l0233:
	aisp	gp4,#1
	orim	gp1,#0xFF

l0236:
	srl	gp4,#1
	andm	gp4,#0xFF
	aisp	gp4,#1
	andm	gp4,#0xFF
	st	gp4,1,gp5
	stc	#0,2,gp5
	lim	gp4,#0x4000
	st	gp4,gp5
	urs	gp15

;; fn0245: 0245
;;   Called from:
;;     0160 (in fn014E)
fn0245 proc
	sisp	gp15,#3
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp6,gp2
	dlr	gp4,gp0
	jc	#3,027A

l024B:
	lb	gp14,#0x6D
	lim	gp3,#1,gp14
	sjs	gp15,0226
	lr	gp10,gp2
	dlr	gp8,gp0
	lr	gp4,gp10
	dlr	gp2,gp8
	efa	gp2,0x800C
	efl	gp5,0x800F
	efdr	gp5,gp2
	efl	gp2,0x8012

l025B:
	efsr	gp2,gp5
	lr	gp7,gp10
	dlr	gp5,gp8
	efdr	gp5,gp2
	efar	gp2,gp5
	efm	gp2,0x8015
	efdr	gp8,gp2
	efar	gp2,gp8
	efm	gp2,0x8015
	dlr	gp0,gp2
	lr	gp2,gp4
	dlr	gp0,gp0
	efm	gp0,1,gp14
	jc	#0xF,025B

;;; ...end of image

;; fn0273: 0273
fn0273 proc
	sisp	gp15,#3
	pshm	gp14,gp14
	lr	gp14,gp15
	efst	gp0,1,gp14
	lr	gp7,gp2
	dlr	gp5,gp0

l027A:
	efm	gp5,1,gp14
	lr	gp13,gp7
	dlr	gp11,gp5
	efm	gp11,0x801B
	lr	gp10,gp7
	dlr	gp8,gp5
	efa	gp8,0x801E
	efa	gp5,0x8021
	efl	gp2,0x8024
	efdr	gp2,gp5
	efar	gp8,gp2
	efdr	gp11,gp8
	efa	gp11,0x8027
	efl	gp0,1,gp14
	efmr	gp0,gp11
	lr	gp15,gp14
	popm	gp14,gp14
	aisp	gp15,#3
	urs	gp15

;;; ...end of image

;; fn02E1: 02E1
fn02E1 proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp10,gp2
	dlr	gp8,gp0
	lr	gp7,gp10
	dlr	gp5,gp8
	efmr	gp5,gp8
	lr	gp4,gp7
	dlr	gp2,gp5
	efm	gp2,0x8036
	efa	gp2,0x8039
	efmr	gp2,gp5
	efa	gp2,0x803C
	efmr	gp2,gp5
	efa	gp2,0x803F
	efmr	gp8,gp2
	lr	gp2,gp10
	dlr	gp0,gp8
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; fn02FA: 02FA
fn02FA proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp7,gp2
	dlr	gp5,gp0
	efmr	gp5,gp0
	lr	gp4,gp7
	dlr	gp2,gp5
	efm	gp2,0x8042
	efa	gp2,0x8045
	efmr	gp2,gp5
	efa	gp2,0x8048
	efmr	gp2,gp5
	dlr	gp0,gp2
	lr	gp2,gp4
	dlr	gp0,gp0
	efa	gp0,0x804B
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; fn0311: 0311
;;   Called from:
;;     039B (in fn037C)
;;     03BA (in fn03A0)
fn0311 proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp7,gp4
	dlr	gp5,gp2
	lr	gp0,gp1
	lr	gp2,gp0
	jc	#6,0377

l0318:
	lb	gp15,#0x1A
	aisp	gp2,#7
	andm	gp2,#0xFFF8

l031C:
	lr	gp1,gp0
	sr	gp1,gp2
	pshm	gp1,gp4
	lr	gp2,gp1
	xorr	gp1,gp1
	xorr	gp3,gp3
	lim	gp4,#7
	dcr	gp1,gp3
	popm	gp1,gp4

l0326:
	jc	#4,0366

l0327:
	lb	gp15,#0x75
	l	gp2,0x32C,gp1
	jc	#0xF,031C

;;; ...end of image

l0336:
	jc	#0xF,0326

;;; ...end of image

l0346:
	jc	#0xF,0336

;;; ...end of image

l0356:
	jc	#0xF,0346

;;; ...end of image

l0366:
	jc	#0xF,0356

;;; ...end of image

l0377:
	lr	gp2,gp5
	dlr	gp0,gp3
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; fn037C: 037C
fn037C proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp10,gp2
	dlr	gp8,gp0
	lr	gp7,gp10
	dlr	gp5,gp8
	jc	#6,03E2

l0383:
	lb	gp15,#0x89
	efl	gp2,0x8054
	lr	gp7,gp4
	dlr	gp5,gp2
	efsr	gp5,gp8
	efm	gp5,0x8057
	efix	gp0,gp5
	eflt	gp2,gp0
	efsr	gp5,gp2
	lr	gp4,gp7
	dlr	gp2,gp5
	jc	#6,03F0

l0391:
	lb	gp15,#0x96
	efa	gp2,0x805A
	da	gp0,0x805D
	dlr	gp8,gp8
	jc	#6,03F7

l0398:
	lb	gp15,#0x9B
	da	gp0,0x805F
	sjs	gp15,0311
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; fn03A0: 03A0
fn03A0 proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp7,gp2
	dlr	gp5,gp0
	jc	#6,0404

l03A5:
	lb	gp15,#0xAB
	efl	gp2,0x8061
	efsr	gp2,gp5
	lr	gp7,gp4
	dlr	gp5,gp2
	efm	gp5,0x8064
	efix	gp0,gp5
	eflt	gp2,gp0
	efsr	gp5,gp2
	lr	gp4,gp7
	dlr	gp2,gp5
	jc	#6,0412

l03B3:
	lb	gp15,#0xB8
	efa	gp2,0x8067
	da	gp0,0x806A
	da	gp0,0x806C
	sjs	gp15,0311
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; fn03BF: 03BF
fn03BF proc
	pshm	gp0,gp13
	lr	gp11,gp0
	lim	gp13,#0x2020
	st	gp13,gp11
	st	gp13,1,gp11
	st	gp13,2,gp11
	xorr	gp13,gp13
	st	gp13,3,gp11

l03CC:
	lim	gp10,#0x20
	orr	gp1,gp1
	bge	03D3

l03D0:
	lim	gp10,#0x2D
	neg	gp1,gp1

l03D3:
	xorr	gp9,gp9
	aisp	gp9,#1
	disp	gp1,#0xA
	orim	gp13,#0x30
	tbr	#0xF,gp9
	bez	03DE

l03DA:
	stlb	gp13,2,gp11
	jc	#0xF,03CC

;;; ...end of image

l03DE:
	stub	gp13,2,gp11
	sisp	gp11,#1
	orr	gp1,gp1

;; fn03E2: 03E2
;;   Called from:
;;     0381 (in fn037C)
;;     03E1 (in fn03BF)
fn03E2 proc
	jc	#5,0432

l03E3:
	lb	gp15,#0xD4
	tbr	#0xF,gp9
	bez	03E9

l03E6:
	stub	gp10,2,gp11
	br	03EB

l03E9:
	stlb	gp10,2,gp11

l03EB:
	popm	gp0,gp13
	urs	gp15

;; fn03ED: 03ED
fn03ED proc
	pshm	gp0,gp12
	lr	gp11,gp0
	lim	gp10,#0x2020

l03F0:
	fab	gp12,#0x20

;; fn03F1: 03F1
;;   Called from:
;;     03EF (in fn03ED)
;;     03F0 (in fn037C)
fn03F1 proc
	lisp	gp9,#6

;; fn03F2: 03F2
;;   Called from:
;;     03F1 (in fn03F1)
;;     03F1 (in fn03F1)
fn03F2 proc
	st	gp10,gp11
	aisp	gp11,#1
	soj	gp9,03F2

;; fn03F7: 03F7
;;   Called from:
;;     0397 (in fn037C)
;;     03F5 (in fn03F2)
fn03F7 proc
	sisp	gp11,#1
	stlb	gp9,gp11
	lim	gp10,#0x20
	orr	gp1,gp1
	bge	0407

l03FE:
	lim	gp10,#0x2D
	cim	gp1,#0x8000
	bnz	0406

l0403:
	lr	gp2,gp2

;; fn0404: 0404
;;   Called from:
;;     03A4 (in fn03A0)
fn0404 proc
	bnz	0406

;; fn0405: 0405
;;   Called from:
;;     0404 (in fn0404)
;;     0404 (in fn03F7)
fn0405 proc
	aisp	gp2,#1

;; fn0406: 0406
;;   Called from:
;;     0402 (in fn03F7)
;;     0404 (in fn0404)
;;     0404 (in fn03F7)
;;     0405 (in fn0405)
fn0406 proc
	dneg	gp1,gp1

;; fn0407: 0407
;;   Called from:
;;     03FD (in fn03F7)
;;     0406 (in fn0406)
fn0407 proc
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

;; fn0412: 0412
;;   Called from:
;;     03B2 (in fn03A0)
fn0412 proc
	bez	0417

;; fn0413: 0413
;;   Called from:
;;     0412 (in fn0412)
;;     0412 (in fn0407)
fn0413 proc
	stub	gp5,gp11
	sisp	gp11,#1
	br	0419

;; fn0417: 0417
;;   Called from:
;;     0412 (in fn0412)
;;     0412 (in fn0407)
;;     0416 (in fn0413)
fn0417 proc
	stlb	gp5,gp11

l0419:
	ddr	gp12,gp7
	jc	#5,046A

l041B:
	dlb	gp12,#9
	aisp	gp9,#1
	tbr	#0xF,gp9
	bnz	0422

l041F:
	stlb	gp10,gp11
	br	0424

l0422:
	stub	gp10,gp11

l0424:
	popm	gp0,gp12
	urs	gp15

;; fn0426: 0426
;;   Called from:
;;     04FA (in fn04F6)
fn0426 proc
	pshm	gp0,gp14
	lr	gp11,gp0
	dlr	gp12,gp1
	xorr	gp0,gp0
	st	gp0,7,gp11
	xorr	gp14,gp14
	lisp	gp1,#6
	sjs	gp15,043E
	popm	gp0,gp14
	urs	gp15

;; fn0432: 0432
;;   Called from:
;;     03E2 (in fn03E2)
fn0432 proc
	pshm	gp0,gp14
	lr	gp11,gp0
	lr	gp14,gp3
	dlr	gp12,gp1
	xorr	gp0,gp0
	st	gp0,0xA,gp11
	lisp	gp1,#0xB
	sjs	gp15,043E
	popm	gp0,gp14
	urs	gp15

;; fn043E: 043E
;;   Called from:
;;     042E (in fn0426)
;;     043A (in fn0432)
fn043E proc
	tbr	#0,gp12
	bez	0450

l0440:
	tbr	#1,gp12
	bnz	0447

l0442:
	lim	gp0,#0x2D20
	efm	gp12,0x57F
	br	0458

l0447:
	lim	gp10,#0x582
	lisp	gp12,#7
	cisp	gp1,#6
	bez	044D

l044C:
	lisp	gp12,#0xA

l044D:
	mov	gp11,gp10
	jc	#0xF,043E

;;; ...end of image

l0450:
	orr	gp0,gp12
	orr	gp0,gp13
	orr	gp0,gp14
	bez	0456

l0454:
	tbr	#1,gp12
	bez	0447

l0456:
	lim	gp0,#0x2B20

l0458:
	st	gp0,gp11
	xorr	gp2,gp2
	tbr	#8,gp13
	bnz	0467

l045D:
	efc	gp12,0x579
	blt	0464

l0460:
	efd	gp12,0x579
	aisp	gp2,#1
	br	045D

l0464:
	lim	gp0,#0x452B
	br	0471

;; fn0467: 0467
;;   Called from:
;;     046E (in fn046A)
fn0467 proc
	efc	gp12,0x57C
	jc	#6,04C9

;; fn046A: 046A
;;   Called from:
;;     041A (in fn0417)
;;     045C (in fn043E)
;;     0469 (in fn0467)
fn046A proc
	dlb	gp12,#0x6F
	efm	gp12,0x579
	aisp	gp2,#1
	br	0467

;;; ...end of image

l0471:
	cisp	gp1,#6
	bez	0476

l0473:
	st	gp0,8,gp11
	br	0478

l0476:
	st	gp0,5,gp11

l0478:
	disp	gp2,#0xA
	xbr	gp2
	orr	gp2,gp3
	orim	gp2,#0x3030
	cisp	gp1,#6
	bez	0482

l047F:
	st	gp2,9,gp11
	br	0484

l0482:
	st	gp2,6,gp11

l0484:
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

l0493:
	efm	gp12,0x579
	dlr	gp4,gp12
	lr	gp6,gp14
	efix	gp2,gp4
	eflt	gp4,gp2
	efsr	gp12,gp4
	aim	gp3,#0x30
	aisp	gp7,#1
	tbr	#0xF,gp7
	bnz	04A2

l049F:
	stub	gp3,1,gp11
	br	04A5

l04A2:
	stlb	gp3,1,gp11
	aisp	gp11,#1

l04A5:
	soj	gp1,0493

l04A7:
	lim	gp3,#0x2020
	st	gp3,1,gp11
	urs	gp15

;; fn04AC: 04AC
;;   Called from:
;;     04B5 (in fn04AF)
;;     04BA (in fn04AF)
;;     04CA (in fn0467)
;;     04CA (in fn04C0)
;;     04CA (in fn04C0)
fn04AC proc
	co	gp0
	urs	gp15

;; fn04AF: 04AF
;;   Called from:
;;     04FC (in fn04F6)
fn04AF proc
	pshm	gp9,gp12
	lr	gp11,gp0
	xorr	gp0,gp0

l04B2:
	lub	gp0,gp11
	bez	04BE

l04B5:
	sjs	gp15,04AC
	llb	gp0,gp11
	bez	04BE

l04BA:
	sjs	gp15,04AC
	aisp	gp11,#1
	br	04B2

l04BE:
	popm	gp9,gp12
	urs	gp15

;; fn04C0: 04C0
;;   Called from:
;;     04D1 (in fn04CE)
;;     04D4 (in fn04CE)
fn04C0 proc
	pshm	gp0,gp0
	andm	gp0,#0xF
	cisp	gp0,#9
	bgt	04C8

l04C5:
	aim	gp0,#0x30
	br	04CA

l04C8:
	aim	gp0,#0x37

l04C9:
	lb	gp12,#0x37

l04CA:
	sjs	gp15,04AC
	popm	gp0,gp0
	urs	gp15

;; fn04CE: 04CE
fn04CE proc
	pshm	gp1,gp1
	lr	gp1,gp0
	srl	gp0,#4
	sjs	gp15,04C0
	lr	gp0,gp1
	sjs	gp15,04C0
	popm	gp1,gp1
	urs	gp15

;;; ...end of image

;; fn04F6: 04F6
fn04F6 proc
	pshm	gp0,gp2
	dlr	gp1,gp0
	lim	gp0,#0x806E
	sjs	gp15,0426
	sjs	gp15,04AF
	popm	gp0,gp2
	urs	gp15

;;; ...end of image
