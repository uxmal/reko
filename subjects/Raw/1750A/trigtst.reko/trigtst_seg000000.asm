;;; Segment seg000000 (00000000)
00000000 0000 0000 0000 0000 0000 0000 0000 0000 ................
; ...

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
00000221      B2F2 9FEE 81EF 9A0E 0001 8172 8750   ...........r.P
00000228 CA5E 0001 81D7 87B5 CAB0 80E9 81A7 8785 .^..............
00000230 AA80 80EC AA50 80EF 8A20 80F2 DB25 AB82 .....P... ...%..
00000238 DBB8 AAB0 80F5 8A0E 0001 CB0B 81FE 8FEE ................
00000240 A2F2 7FF0 B2F0 9FEE 81EF 8172 8750 8A80 ...........r.P..
00000248 80F8 910E 0001 8147 8725 7060 0252 814A .......G.%p`.R.J
00000250 8728 BB25 FA20 80FB 7040 0289 8755 7060 .(.%. ..p@...Up`
00000258 0260 8A20 80F8 BB25 8174 8752 911E 0001 .`. ...%.t.R....
00000260 FA50 80FE 7040 026C 8127 8705 7EF0 0221 .P..p@.l.'..~..!
00000268 81A2 8780 70F0 0280 8A20 80FB BB25 8702 ....p.... ...%..
00000270 8124 8700 CA00 80FE 7EF0 01E6 7EF0 0221 .$......~...~..!
00000278 8172 8750 AB50 8A20 8101 81A4 8782 BB85 .r.P.P. ........
00000280 80BE 0001 7020 0289 8A20 80F8 BB28 81A4 ....p ... ...(..
00000288 8782 812A 8708 81FE 8FEE A2F0 7FF0 9FEE ...*............
00000290 81EF 81A2 8780 817A 8758 CB58 8147 8725 .......z.X.X.G.%
00000298 CA20 8104 AA20 8107 CB25 AA20 810A CB25 . ... ...%. ...%
000002A0 AA20 810D CB82 812A 8708 81FE 8FEE 7FF0 . .....*........
000002A8 9FEE 81EF 8172 8750 CB50 8147 8725 CA20 .....r.P.P.G.%. 
000002B0 8110 AA20 8113 CB25 AA20 8116 CB25 8702 ... ...%. ...%..
000002B8 8124 8700 AA00 8119 81FE 8FEE 7FF0      .$............  

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
000002D9      0000 02E2 02E6 02EE 02F6 0302 0306   ..............
000002E0 0310 0316 8127 8705                     .....'..        

l000002E4:
	jc	#0xF,000002D4
000002E5                          02FC 8A20 811C           ... ..
000002E8 8702 8124 8700 BB05 70F0 02F0 8127 8705 ...$....p....'..
000002F0 7EF0 02A8 8152 8730                     ~....R.0        

l000002F4:
	jc	#0xF,000002E4
000002F5                          0325 8A20 811C           .%. ..
000002F8 8702 8124 8700 BB05 7EF0 028F 8152 8730 ...$....~....R.0
00000300 70F0 0325 8127 8705                     p..%.'..        

l00000304:
	jc	#0xF,000002F4
00000305                          031C 8A20 811C           ... ..
00000308 8702 8124 8700 BB05 7EF0 02A8 70F0 031E ...$....~...p...
00000310 8127 8705 7EF0 02A8                     .'..~...        

l00000314:
	jc	#0xF,00000304
00000315                          031E 8A20 811C           ... ..
00000318 8702 8124 8700 BB05 7EF0 028F 8A30 811F ...$....~....0..
00000320 BB30 70F0 0325 8A30 811F                .0p..%.0..      

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
0000036D                          9F0D 81B0 85D0           ......
00000370 2020 90DB 0000 90DB 0001 90DB 0002 E5DD   ..............
00000378 90DB 0003 85A0 0020 E111 7B04 85A0 002D ....... ..{....-
00000380 B411 E599 A290 D219 4AD8 0030 57F9 7505 ........J..0W.u.
00000388 9CDB 0002 70F0 038F 9BDB 0002 B2B0 E111 ....p...........

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
0000039B                9F0C 81B0 85A0                 ......    

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
000003D4                     9F0E 81B0 87C1 E500         ........
000003D8 900B 0007 E5EE 8215 7EF0 03EC 8F0E 7FF0 ........~.......

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
000003FD                          0459                     .Y    

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
0000041D                          8500 452D                ..E-  

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
0000046E                               9F00 4A07             ..J.
00000470 000F F208 7904 4A01 0030 7403 4A01      ....y.J..0t.J.  

l00000477:
	lb	gp12,#0x37
	sjs	gp15,045A
	popm	gp0,gp0
	urs	gp15
0000047C                     9F11 8110 6130 7EF0         ....a0~.
00000480 046E 8101 7EF0 046E 8F11 7FF0 9F11 8110 .n..~..n........
00000488 EC00 7EF0 047C 8101 7EF0 047C 8F11 7FF0 ..~..|..~..|....
00000490 9F11 8110 8500 813C 7EF0 036D 7EF0 045D .......<~..m~..]
00000498 8F11 7FF0 9F02 8710 8500 813C 7EF0 039B ...........<~...
000004A0 7EF0 045D 8F02 7FF0 9F02 8710 8500 813C ~..]...........<
000004A8 7EF0 03D4 7EF0 045D 8F02 7FF0           ~...~..]....    

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
000004C2           0073 0071 0072 0074 0028 0031     .s.q.r.t.(.1
000004C8 002E 0030 0029 0020 003D 0020 0000 0073 ...0.). .=. ...s
000004D0 0071 0072 0074 0028 0034 002E 0030 0029 .q.r.t.(.4...0.)
000004D8 0020 003D 0020 0000 0073 0071 0072 0074 . .=. ...s.q.r.t
000004E0 0028 0039 002E 0030 0029 0020 003D 0020 .(.9...0.). .=. 
000004E8 0000 0073 0071 0072 0074 0028 0032 0035 ...s.q.r.t.(.2.5
000004F0 0036 002E 0030 0029 0020 003D 0020 0000 .6...0.). .=. ..
000004F8 0073 0069 006E 0028 0030 002E 0030 0029 .s.i.n.(.0...0.)
00000500 0020 003D 0020 0000 0073 0069 006E 0028 . .=. ...s.i.n.(
00000508 0050 0049 002F 0034 0029 0020 003D 0020 .P.I./.4.). .=. 
00000510 0000 0073 0069 006E 0028 0050 0049 002F ...s.i.n.(.P.I./
00000518 0032 0029 0020 003D 0020 0000 0073 0069 .2.). .=. ...s.i
00000520 006E 0028 0033 002A 0050 0049 002F 0034 .n.(.3.*.P.I./.4
00000528 0029 0020 003D 0020 0000 0073 0069 006E .). .=. ...s.i.n
00000530 0028 0050 0049 0029 0020 003D 0020 0000 .(.P.I.). .=. ..
00000538 0073 0069 006E 0028 0031 002E 0030 0029 .s.i.n.(.1...0.)
00000540 0020 003D 0020 0000 0063 006F 0073 0028 . .=. ...c.o.s.(
00000548 0030 002E 0030 0029 0020 003D 0020 0000 .0...0.). .=. ..
00000550 0063 006F 0073 0028 0050 0049 002F 0034 .c.o.s.(.P.I./.4
00000558 0029 0020 003D 0020 0000 0063 006F 0073 .). .=. ...c.o.s
00000560 0028 0050 0049 002F 0032 0029 0020 003D .(.P.I./.2.). .=
00000568 0020 0000 0063 006F 0073 0028 0033 002A . ...c.o.s.(.3.*
00000570 0050 0049 002F 0034 0029 0020 003D 0020 .P.I./.4.). .=. 
00000578 0000 0063 006F 0073 0028 0050 0049 0029 ...c.o.s.(.P.I.)
00000580 0020 003D 0020 0000 0063 006F 0073 0028 . .=. ...c.o.s.(
00000588 0031 002E 0030 0029 0020 003D 0020 0000 .1...0.). .=. ..
00000590 4000 0001 0000 4000 0003 0000 4800 0004 @.....@.....H...
00000598 0000 4000 0009 0000 0000 0000 0000 6487 ..@...........d.
000005A0 EB00 F22C 6487 EB01 F22C 4B65 F002 F5A1 ...,d....,Ke....
000005A8 6487 EE02 0B0A 0000 0000 0000 4B8A 5C00 d...........K.\.
000005B0 E5B4 6AD4 D4FF 024B 5A82 7900 99FE BF6F ..j....KZ.y....o
000005B8 C900 BC77 89B3 1102 A543 897F C701 607C ...w.....C....`|
000005C0 B08C 2E01 770B 4000 0001 0000 0000 0000 ....w.@.........
000005C8 0000 4000 0001 0000 4000 0000 0000 6487 ..@.....@.....d.
000005D0 EB01 F22C B480 A5F2 AECD 5197 A2F8 4894 ...,......Q...H.
000005D8 AD51 2EFD C6BD 6487 EB00 F22C AC60 45F5 .Q....d....,.`E.
000005E0 BB35 40EB EDFB FA43 B10B 1FFF EEB2 4000 .5@....C......@.
000005E8 0001 0000 4000 0001 0000 0000 0000 0000 ....@...........
000005F0 0000 0000 0000 517C C301 9FFD 4000 0001 ......Q|....@...
000005F8 0000 FFFF FFFF 0000 0004 0000 0000 0000 ................
00000600 517C C301 9FFD 4000 0001 0000 FFFF FFFF Q|....@.........
00000608 0000 0002 5000 0004 0000 4000 0001 0000 ....P.....@.....
00000610 8000 0000 0000 2320 6E6F 7420 6E6F 726D ......# not norm
00000618 616C 2E20 2020 2020 2020 2000 0000 0000 al.        .....
00000620 0000 0000 0000 0000 0000 0000 0000 0000 ................
; ...
