;;; Segment normal (0115)

;; reverse: 0115
;;   Called from:
;;     0114 (in fn0100)
;;     0135 (in init_fft)
reverse proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp3,gp0
	xorr	gp0,gp0
	lr	gp1,gp0
	lisp	gp4,#1

l011B:
	lr	gp2,gp3
	neg	gp1,gp1
	sar	gp2,gp1
	andm	gp2,#1
	jc	#2,0128

l0122:
	lisp	gp2,#3
	sr	gp2,gp1
	lr	gp5,gp4
	slr	gp5,gp2
	lr	gp2,gp5
	orr	gp0,gp2

l0128:
	aisp	gp1,#1
	cisp	gp1,#3
	ble	011B

l012B:
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; init_fft: 012E
;;   Called from:
;;     020D (in main)
init_fft proc
	sisp	gp15,#1
	pshm	gp14,gp14
	lr	gp14,gp15
	xorr	gp3,gp3

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

;; compute_output: 014E
;;   Called from:
;;     0211 (in main)
compute_output proc
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

;; fft: 016F
;;   Called from:
;;     020F (in main)
fft proc
	sisp	gp15,#0xF
	pshm	gp14,gp14
	lr	gp14,gp15
	stc	#1,2,gp14

l0174:
	l	gp6,2,gp14
	sll	gp6,#1
	st	gp6,1,gp14
	stc	#0,3,gp14
	l	gp7,3,gp14
	c	gp7,2,gp14
	jc	#6,01E3

l0181:
	l	gp2,2,gp14
	dsra	gp2,#0x10
	eflt	gp6,gp2
	efst	gp6,0xA,gp14

l0187:
	l	gp3,3,gp14
	dsra	gp3,#0x10
	eflt	gp3,gp3
	efm	gp3,0x8003
	efd	gp3,0xA,gp14
	lr	gp2,gp5
	dlr	gp0,gp3
	efst	gp3,0xD,gp14
	sjs	gp15,03A0
	efst	gp0,4,gp14
	efl	gp3,0xD,gp14
	lr	gp2,gp5
	dlr	gp0,gp3
	sjs	gp15,037C
	efst	gp0,7,gp14
	l	gp0,3,gp14
	cisp	gp0,#0x10
	jc	#6,01DC

l01A4:
	lb	gp14,#2
	ar	gp2,gp0
	lr	gp11,gp2
	sll	gp11,#1
	ar	gp11,gp2
	lim	gp13,#0x8030,gp11
	efl	gp8,4,gp14
	efm	gp8,gp13
	aim	gp11,#0x8060
	efl	gp2,7,gp14
	efm	gp2,gp11
	efsr	gp8,gp2
	efl	gp5,7,gp14
	efm	gp5,gp13
	efl	gp2,4,gp14
	efm	gp2,gp11
	efar	gp5,gp2
	lr	gp1,gp0
	sll	gp1,#1
	ar	gp1,gp0
	lim	gp12,#0x8030,gp1
	efl	gp2,gp12
	efsr	gp2,gp8
	efst	gp2,gp13
	aim	gp1,#0x8060
	efl	gp2,gp1
	efsr	gp2,gp5
	efst	gp2,gp11
	efa	gp8,gp12
	efst	gp8,gp12
	efa	gp5,gp1
	efst	gp5,gp1
	a	gp0,1,gp14
	cisp	gp0,#0x10
	blt	01A4

l01DC:
	incm	#1,3,gp14
	l	gp7,3,gp14
	c	gp7,2,gp14
	blt	0187

l01E3:
	l	gp8,1,gp14
	st	gp8,2,gp14
	cisp	gp8,#0x10
	jc	#1,0174

l01EA:
	lr	gp15,gp14
	popm	gp14,gp14
	aisp	gp15,#0xF
	urs	gp15

;; main: 01EE
main proc
	sisp	gp15,#1
	pshm	gp14,gp14
	lr	gp14,gp15
	xorr	gp3,gp3
	lim	gp0,#0x8000

l01F4:
	lr	gp2,gp3
	sll	gp2,#1
	ar	gp2,gp3
	ar	gp2,gp0
	efl	gp4,0x8006
	efst	gp4,gp2
	aisp	gp3,#1
	cisp	gp3,#7
	ble	01F4

l01FF:
	lisp	gp3,#8
	lim	gp0,#0x8000

l0202:
	lr	gp2,gp3
	sll	gp2,#1
	ar	gp2,gp3
	ar	gp2,gp0
	efl	gp4,0x8009
	efst	gp4,gp2
	aisp	gp3,#1
	cisp	gp3,#0xF
	ble	0202

l020D:
	sjs	gp15,012E
	sjs	gp15,016F
	sjs	gp15,014E
	xorr	gp3,gp3

l0214:
	lr	gp2,gp3
	sll	gp2,#1
	ar	gp2,gp3
	efl	gp0,0x8090,gp2
	st	gp3,1,gp14
	sjs	gp15,04F6
	l	gp3,1,gp14
	aisp	gp3,#1
	cisp	gp3,#8
	ble	0214

l0222:
	lr	gp15,gp14
	popm	gp14,gp14
	aisp	gp15,#1
	urs	gp15

;; frex: 0226
;;   Called from:
;;     024E (in sqrt)
frex proc
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

;; sqrt: 0245
;;   Called from:
;;     0160 (in compute_output)
;;     02C6 (in asin)
sqrt proc
	sisp	gp15,#3
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp6,gp2
	dlr	gp4,gp0
	jc	#3,026D

l024C:
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
	jc	#0xF,026F

l026D:
	efl	gp0,0x8018

l026F:
	lr	gp15,gp14
	popm	gp14,gp14
	aisp	gp15,#3
	urs	gp15

;; auxasin: 0273
;;   Called from:
;;     02B8 (in asin)
;;     02C8 (in asin)
auxasin proc
	sisp	gp15,#3
	pshm	gp14,gp14
	lr	gp14,gp15
	efst	gp0,1,gp14
	lr	gp7,gp2
	dlr	gp5,gp0
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

;; asin: 0294
asin proc
	sisp	gp15,#1
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp7,gp2
	dlr	gp5,gp0
	efl	gp8,0x802A
	stc	#0,1,gp14
	lr	gp4,gp7
	dlr	gp2,gp5
	jc	#6,02A4

l02A1:
	lr	gp4,gp10
	dlr	gp2,gp8
	efsr	gp2,gp5

l02A4:
	efc	gp2,0x802D
	jc	#4,02DB

l02A8:
	dlr	gp5,gp5
	jc	#6,02B2

l02AB:
	efl	gp2,0x802A
	efsr	gp2,gp5
	lr	gp7,gp4
	dlr	gp5,gp2
	stc	#1,1,gp14

l02B2:
	efc	gp5,0x8030
	jc	#4,02BE

l02B6:
	lr	gp2,gp7
	dlr	gp0,gp5
	sjs	gp15,0273
	lr	gp10,gp2
	dlr	gp8,gp0
	jc	#0xF,02D2

l02BE:
	efl	gp2,0x802D
	efsr	gp2,gp5
	dlr	gp0,gp2
	lr	gp2,gp4
	dlr	gp0,gp0
	efm	gp0,0x8030
	sjs	gp15,0245
	sjs	gp15,0273
	lr	gp7,gp2
	dlr	gp5,gp0
	efar	gp5,gp0
	efl	gp2,0x8033
	lr	gp10,gp4
	dlr	gp8,gp2
	efsr	gp8,gp5

l02D2:
	l	gp11,1,gp14
	jc	#2,02DB

l02D6:
	efl	gp2,0x802A
	efsr	gp2,gp8
	lr	gp10,gp4
	dlr	gp8,gp2

l02DB:
	lr	gp2,gp10
	dlr	gp0,gp8
	lr	gp15,gp14
	popm	gp14,gp14
	aisp	gp15,#1
	urs	gp15

;; rsin: 02E1
rsin proc
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

;; rcos: 02FA
rcos proc
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

;; sincos: 0311
;;   Called from:
;;     039B (in sin)
;;     03BA (in cos)
sincos proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp7,gp4
	dlr	gp5,gp2
	lr	gp0,gp1
	lr	gp2,gp0
	jc	#6,031A

l0319:
	aisp	gp2,#7

l031A:
	andm	gp2,#0xFFF8
	lr	gp1,gp0
	sr	gp1,gp2
	pshm	gp1,gp4
	lr	gp2,gp1
	xorr	gp1,gp1
	xorr	gp3,gp3
	lim	gp4,#7
	dcr	gp1,gp3
	popm	gp1,gp4
	jc	#4,0375

l0328:
	l	gp2,0x32C,gp1
	jc	#0xF,#0,gp2
032C                     0334 0338 0340 0348         .4.8.@.H
0330 0354 0358 0362 0368 8127 8705 70F0 034E .T.X.b.h.'..p..N
0338 8A20 804E 8702 8124 8700 BB05 70F0 0342 . .N...$....p..B
0340 8127 8705 7EF0 02FA 8152 8730 70F0 0377 .'..~....R.0p..w
0348 8A20 804E 8702 8124 8700 BB05 7EF0 02E1 . .N...$....~...
0350 8152 8730 70F0 0377 8127 8705 70F0 036E .R.0p..w.'..p..n
0358 8A20 804E 8702 8124 8700 BB05 7EF0 02FA . .N...$....~...
0360 70F0 0370 8127 8705 7EF0 02FA 70F0 0370 p..p.'..~...p..p
0368 8A20 804E 8702 8124 8700 BB05 7EF0 02E1 . .N...$....~...
0370 8A30 8051 BB30 70F0 0377                .0.Q.0p..w      

l0375:
	efl	gp3,0x8051
	lr	gp2,gp5
	dlr	gp0,gp3
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; sin: 037C
;;   Called from:
;;     019B (in fft)
sin proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp10,gp2
	dlr	gp8,gp0
	lr	gp7,gp10
	dlr	gp5,gp8
	jc	#6,0389

l0384:
	efl	gp2,0x8054
	lr	gp7,gp4
	dlr	gp5,gp2
	efsr	gp5,gp8

l0389:
	efm	gp5,0x8057
	efix	gp0,gp5
	eflt	gp2,gp0
	efsr	gp5,gp2
	lr	gp4,gp7
	dlr	gp2,gp5
	jc	#6,0396

l0392:
	efa	gp2,0x805A
	da	gp0,0x805D

l0396:
	dlr	gp8,gp8
	jc	#6,039B

l0399:
	da	gp0,0x805F

l039B:
	sjs	gp15,0311
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; cos: 03A0
;;   Called from:
;;     0193 (in fft)
cos proc
	pshm	gp14,gp14
	lr	gp14,gp15
	lr	gp7,gp2
	dlr	gp5,gp0
	jc	#6,03AB

l03A6:
	efl	gp2,0x8061
	efsr	gp2,gp5
	lr	gp7,gp4
	dlr	gp5,gp2

l03AB:
	efm	gp5,0x8064
	efix	gp0,gp5
	eflt	gp2,gp0
	efsr	gp5,gp2
	lr	gp4,gp7
	dlr	gp2,gp5
	jc	#6,03B8

l03B4:
	efa	gp2,0x8067
	da	gp0,0x806A

l03B8:
	da	gp0,0x806C
	sjs	gp15,0311
	lr	gp15,gp14
	popm	gp14,gp14
	urs	gp15

;; cvia: 03BF
;;   Called from:
;;     04E6 (in pr_dec_word)
cvia proc
	pshm	gp0,gp13
	lr	gp11,gp0
	lim	gp13,#0x2020
	st	gp13,gp11
	st	gp13,1,gp11
	st	gp13,2,gp11
	xorr	gp13,gp13
	st	gp13,3,gp11
	lim	gp10,#0x20
	orr	gp1,gp1
	bge	03D3

l03D0:
	lim	gp10,#0x2D
	neg	gp1,gp1

l03D3:
	xorr	gp9,gp9

l03D4:
	aisp	gp9,#1
	disp	gp1,#0xA
	orim	gp13,#0x30
	tbr	#0xF,gp9
	bez	03DE

l03DA:
	stlb	gp13,2,gp11
	jc	#0xF,03E1

l03DE:
	stub	gp13,2,gp11
	sisp	gp11,#1

l03E1:
	orr	gp1,gp1
	jc	#5,03D4

l03E4:
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

;; cvla: 03ED
;;   Called from:
;;     04F0 (in pr_long_word)
cvla proc
	pshm	gp0,gp12
	lr	gp11,gp0
	lim	gp10,#0x2020
	lisp	gp9,#6

l03F2:
	st	gp10,gp11
	aisp	gp11,#1
	soj	gp9,03F2

l03F7:
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
	bnz	0406

l0405:
	aisp	gp2,#1

l0406:
	dneg	gp1,gp1

l0407:
	xorr	gp7,gp7
	lisp	gp8,#0xA

l0409:
	aisp	gp9,#1
	dlr	gp4,gp1
	dlr	gp2,gp1
	ddr	gp2,gp7
	dmr	gp2,gp7
	dsr	gp4,gp2
	aim	gp5,#0x30
	tbr	#0xF,gp9
	bez	0417

l0413:
	stub	gp5,gp11
	sisp	gp11,#1
	br	0419

l0417:
	stlb	gp5,gp11

l0419:
	ddr	gp12,gp7
	jc	#5,0409

l041C:
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

;; cvfa: 0426
;;   Called from:
;;     04FA (in pr_fp_num)
cvfa proc
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

;; cvea: 0432
;;   Called from:
;;     0505 (in pr_efp_num)
cvea proc
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
;;     042E (in cvfa)
;;     043A (in cvea)
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
	jc	#0xF,04AB

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

l0467:
	efc	gp12,0x57C
	jc	#6,046F

l046B:
	efm	gp12,0x579
	aisp	gp2,#1
	br	0467

l046F:
	lim	gp0,#0x452D

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

l04AB:
	urs	gp15

;; putchar: 04AC
;;   Called from:
;;     04B5 (in puts)
;;     04BA (in puts)
;;     04CA (in pr_nibble)
putchar proc
	co	gp0
	urs	gp15

;; puts: 04AF
;;   Called from:
;;     04E8 (in pr_dec_word)
;;     04F2 (in pr_long_word)
;;     04FC (in pr_fp_num)
;;     0507 (in pr_efp_num)
puts proc
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

;; pr_nibble: 04C0
;;   Called from:
;;     04D1 (in pr_hex_byte)
;;     04D4 (in pr_hex_byte)
pr_nibble proc
	pshm	gp0,gp0
	andm	gp0,#0xF
	cisp	gp0,#9
	bgt	04C8

l04C5:
	aim	gp0,#0x30
	br	04CA

l04C8:
	aim	gp0,#0x37

l04CA:
	sjs	gp15,04AC
	popm	gp0,gp0
	urs	gp15

;; pr_hex_byte: 04CE
;;   Called from:
;;     04DB (in pr_hex_word)
;;     04DE (in pr_hex_word)
pr_hex_byte proc
	pshm	gp1,gp1
	lr	gp1,gp0
	srl	gp0,#4
	sjs	gp15,04C0
	lr	gp0,gp1
	sjs	gp15,04C0
	popm	gp1,gp1
	urs	gp15

;; pr_hex_word: 04D8
pr_hex_word proc
	pshm	gp1,gp1
	lr	gp1,gp0
	xbr	gp0
	sjs	gp15,04CE
	lr	gp0,gp1
	sjs	gp15,04CE
	popm	gp1,gp1
	urs	gp15

;; pr_dec_word: 04E2
pr_dec_word proc
	pshm	gp1,gp1
	lr	gp1,gp0
	lim	gp0,#0x806E
	sjs	gp15,03BF
	sjs	gp15,04AF
	popm	gp1,gp1
	urs	gp15

;; pr_long_word: 04EC
pr_long_word proc
	pshm	gp0,gp2
	dlr	gp1,gp0
	lim	gp0,#0x806E
	sjs	gp15,03ED
	sjs	gp15,04AF
	popm	gp0,gp2
	urs	gp15

;; pr_fp_num: 04F6
;;   Called from:
;;     021B (in main)
pr_fp_num proc
	pshm	gp0,gp2
	dlr	gp1,gp0
	lim	gp0,#0x806E
	sjs	gp15,0426
	sjs	gp15,04AF
	popm	gp0,gp2
	urs	gp15

;; pr_efp_num: 0500
pr_efp_num proc
	pshm	gp0,gp3
	lr	gp3,gp2
	dlr	gp1,gp0
	lim	gp0,#0x806E
	sjs	gp15,0432
	sjs	gp15,04AF
	popm	gp0,gp3
	urs	gp15
050B                0000 0000 0000 9B78 1102       .......x..
0510 F4F5 8000 0000 0000 4000 0001 0000 62E2 ........@.....b.
0518 EB01 1C43 6115 B502 73EA 4616 1E02 4F76 ...Ca...s.F...Ov
0520 4000 0000 0000 0000 0000 0000 BF6F C900 @............o..
0528 BC77 89B3 1102 A543 897F C701 607C B08C .w.....C....`|..
0530 2E01 770B 4000 0001 0000 0000 0000 0000 ..w.@...........
0538 4000 0001 0000 4000 0000 0000 6487 EB01 @.....@.....d...
0540 F22C B480 A5F2 AECD 5197 A2F8 4894 AD51 .,......Q...H..Q
0548 2EFD C6BD 6487 EB00 F22C AC60 45F5 BB35 ....d....,.`E..5
0550 40EB EDFB FA43 B10B 1FFF EEB2 4000 0001 @....C......@...
0558 0000 4000 0001 0000 0000 0000 0000 0000 ..@.............
0560 0000 0000 517C C301 9FFD 4000 0001 0000 ....Q|....@.....
0568 FFFF FFFF 0000 0004 0000 0000 0000 517C ..............Q|
0570 C301 9FFD 4000 0001 0000 FFFF FFFF 0000 ....@...........
0578 0002 5000 0004 0000 4000 0001 0000 8000 ..P.....@.......
0580 0000 0000 2320 6E6F 7420 6E6F 726D 616C ....# not normal
0588 2E20 2020 2020 2020 2000                .        .      
