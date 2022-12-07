;;; Segment .text (00400220)

;; fn00400220: 00400220
;;   Called from:
;;     00403D54 (in fn00403CA0)
fn00400220 proc
	stwu	r1,-64(r1)
	mflr	r12
	stw	r12,72(r1)
	stw	r3,88(r1)
	stw	r4,92(r1)
	lwz	r4,68(r2)
	addi	r4,r4,+0000
	ori	r3,r4,0000
	bl	fn00400268
	nop
	addi	r5,r0,+0000
	stw	r5,56(r1)
	b	$00400254

l00400254:
	lwz	r3,56(r1)
	lwz	r12,72(r1)
	mtlr	r12
	addi	r1,r1,+0040
	blr

;; fn00400268: 00400268
;;   Called from:
;;     00400240 (in fn00400220)
fn00400268 proc
	stw	r3,24(r1)
	stw	r4,28(r1)
	stw	r5,32(r1)
	stw	r6,36(r1)
	stw	r7,40(r1)
	stw	r8,44(r1)
	stw	r9,48(r1)
	stw	r10,52(r1)
	stwu	r1,-72(r1)
	stmw	r29,56(r1)
	mflr	r12
	lwz	r31,72(r2)
	stw	r12,80(r1)
	stw	r3,96(r1)
	addi	r31,r31,+0020
	ori	r3,r31,0000
	bl	fn004002F8
	nop
	lwz	r4,96(r1)
	ori	r30,r3,0000
	ori	r3,r31,0000
	addi	r5,r1,+0060
	addi	r5,r5,+0004
	bl	fn004004B4
	nop
	ori	r29,r3,0000
	ori	r3,r30,0000
	ori	r4,r31,0000
	bl	fn00400430
	nop
	lwz	r12,80(r1)
	ori	r3,r29,0000
	mtlr	r12
	lmw	r29,56(r1)
	addi	r1,r1,+0048
	blr

;; fn004002F8: 004002F8
;;   Called from:
;;     004002A8 (in fn00400268)
;;     00403400 (in fn004033C4)
fn004002F8 proc
	stwu	r1,-64(r1)
	stmw	r30,56(r1)
	ori	r31,r3,0000
	mflr	r12
	lwz	r3,16(r31)
	stw	r12,72(r1)
	bl	fn00401474
	nop
	cmpwi	cr1,r3,+0000
	bne	cr1,$00400338

l00400320:
	lwz	r12,72(r1)
	lwz	r31,60(r1)
	mtlr	r12
	addi	r1,r1,+0040
	addi	r3,r0,+0000
	blr

l00400338:
	lwz	r11,72(r2)
	addi	r4,r11,+0020
	cmplw	cr1,r31,r4
	bne	cr1,$00400350

l00400348:
	addi	r10,r0,+0000
	b	$00400360

l00400350:
	addi	r11,r11,+0040
	cmplw	cr1,r31,r11
	bne	cr1,$00400418

l0040035C:
	addi	r10,r0,+0001

l00400360:
	lwz	r4,76(r2)
	lwz	r5,0(r4)
	addi	r5,r5,+0001
	stw	r5,0(r4)
	lwz	r6,12(r31)
	andi.	r6,r6,010C
	beq	$00400394

l0040037C:
	lwz	r12,72(r1)
	lwz	r31,60(r1)
	mtlr	r12
	addi	r1,r1,+0040
	addi	r3,r0,+0000
	blr

l00400394:
	lwz	r30,80(r2)
	rlwinm	r10,r10,02,00,1D
	lwzx	r11,r30,r10
	add	r30,r10,r30
	cmplwi	cr1,r11,0000
	bne	cr1,$004003E0

l004003AC:
	addi	r3,r0,+1000
	bl	fn00400F9C
	nop
	ori	r11,r3,0000
	cmplwi	cr1,r11,0000
	stw	r3,0(r30)
	bne	cr1,$004003E0

l004003C8:
	lwz	r12,72(r1)
	lmw	r30,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	addi	r3,r0,+0000
	blr

l004003E0:
	lwz	r12,72(r1)
	lwz	r4,12(r31)
	mtlr	r12
	ori	r4,r4,1102
	stw	r4,12(r31)
	stw	r11,8(r31)
	addi	r5,r0,+1000
	stw	r5,24(r31)
	stw	r5,4(r31)
	stw	r11,0(r31)
	lmw	r30,56(r1)
	addi	r1,r1,+0040
	addi	r3,r0,+0001
	blr

l00400418:
	lwz	r12,72(r1)
	lwz	r31,60(r1)
	mtlr	r12
	addi	r1,r1,+0040
	addi	r3,r0,+0000
	blr

;; fn00400430: 00400430
;;   Called from:
;;     004002D8 (in fn00400268)
;;     00403430 (in fn004033C4)
fn00400430 proc
	cmpwi	cr1,r3,+0000
	mflr	r12
	stwu	r1,-64(r1)
	stw	r12,72(r1)
	stw	r31,56(r1)
	ori	r31,r4,0000
	beq	cr1,$00400488

l0040044C:
	lwz	r4,12(r31)
	andi.	r4,r4,1000
	beq	$004004A0

l00400458:
	ori	r3,r31,0000
	bl	fn004012B0
	nop
	lwz	r4,12(r31)
	addi	r5,r0,+0000
	stw	r5,24(r31)
	stw	r5,0(r31)
	stw	r5,8(r31)
	addi	r6,r0,-1101
	and	r4,r4,r6
	stw	r4,12(r31)
	b	$004004A0

l00400488:
	lwz	r4,12(r31)
	andi.	r4,r4,1000
	beq	$004004A0

l00400494:
	ori	r3,r31,0000
	bl	fn004012B0
	nop

l004004A0:
	lwz	r12,72(r1)
	lwz	r31,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	blr

;; fn004004B4: 004004B4
;;   Called from:
;;     004002C4 (in fn00400268)
;;     0040335C (in fn004032FC)
;;     0040341C (in fn004033C4)
fn004004B4 proc
	stwu	r1,-704(r1)
	stmw	r13,624(r1)
	ori	r23,r4,0000
	stw	r5,736(r1)
	addi	r4,r0,+0000
	stw	r4,56(r1)
	lbz	r31,0(r23)
	mflr	r12
	extsb.	r31,r31
	ori	r22,r3,0000
	stw	r12,712(r1)
	addi	r23,r23,+0001
	addi	r6,r0,+0000
	stw	r6,60(r1)
	beq	$00400DEC

l004004F0:
	addi	r26,r1,+0040
	addi	r21,r1,+0240
	lwz	r14,84(r2)
	lwz	r16,88(r2)
	lwz	r13,84(r2)
	lwz	r17,92(r2)

l00400508:
	lwz	r4,56(r1)
	cmpwi	cr1,r4,-0001
	ble	cr1,$00400DEC

l00400514:
	cmpwi	cr1,r31,+001F
	ble	cr1,$00400544

l0040051C:
	cmpwi	cr1,r31,+0079
	bge	cr1,$00400544

l00400524:
	addi	r4,r14,+0004
	addi	r4,r4,-0020
	ori	r5,r31,0000
	lbzx	r4,r4,r5
	extsb	r4,r4
	andi.	r4,r4,000F
	ori	r10,r4,0000
	b	$00400548

l00400544:
	addi	r10,r0,+0000

l00400548:
	lwz	r5,60(r1)
	addi	r4,r14,+0004
	rlwinm	r10,r10,03,00,1C
	add	r10,r10,r5
	lbzx	r4,r4,r10
	extsb	r4,r4
	srawi	r4,r4,04
	extsb	r4,r4
	addi	r11,r4,+0000
	cmplwi	cr1,r11,0008
	stw	r4,60(r1)
	bge	cr1,$00400DDC

l00400578:
	rlwinm	r11,r11,02,00,1D
	bl	fn004005A0
	b	$004005B0
00400584             48 00 00 84 48 00 00 9C 48 00 01 2C     H...H...H..,
00400590 48 00 01 60 48 00 01 64 48 00 01 94 48 00 01 A0 H..`H..dH...H...

;; fn004005A0: 004005A0
;;   Called from:
;;     0040057C (in fn004004B4)
fn004005A0 proc
	mflr	r0
	add	r0,r0,r11
	mtlr	r0
	blr

l004005B0:
	lwz	r4,0(r16)
	andi.	r5,r31,00FF
	rlwinm	r5,r5,01,00,1E
	lhzx	r4,r4,r5
	andi.	r4,r4,8000
	beq	$004005EC

l004005C8:
	addi	r4,r1,+0038
	stw	r4,616(r1)
	lwz	r5,616(r1)
	ori	r4,r22,0000
	ori	r3,r31,0000
	bl	fn00400E04
	lbz	r31,0(r23)
	addi	r23,r23,+0001
	extsb	r31,r31

l004005EC:
	addi	r4,r1,+0038
	stw	r4,616(r1)
	lwz	r5,616(r1)
	ori	r4,r22,0000
	ori	r3,r31,0000
	bl	fn00400E04
	b	$00400DDC
00400608                         39 E0 00 00 3A 60 00 00         9...:`..
00400610 3A 80 00 00 3B 00 00 00 3B C0 00 00 3B 80 FF FF :...;...;...;...
00400620 48 00 07 BC 63 EB 00 00 39 6B FF E0 28 8B 00 11 H...c...9k..(...
00400630 40 84 07 AC 55 6B 10 3A 48 00 00 49 48 00 00 54 @...Uk.:H..IH..T
00400640 48 00 07 9C 48 00 07 98 48 00 00 50 48 00 07 90 H...H...H..PH...
00400650 48 00 07 8C 48 00 07 88 48 00 07 84 48 00 07 80 H...H...H...H...
00400660 48 00 07 7C 48 00 07 78 48 00 00 38 48 00 07 70 H..|H..xH..8H..p
00400670 48 00 00 38 48 00 07 68 48 00 07 64 48 00 00 34 H..8H..hH..dH..4

;; fn00400680: 00400680
fn00400680 proc
	mflr	r0
	add	r0,r0,r11
	mtlr	r0
	blr
00400690 63 DE 00 02 48 00 07 48 63 DE 00 80 48 00 07 40 c...H..Hc...H..@
004006A0 63 DE 00 01 48 00 07 38 63 DE 00 04 48 00 07 30 c...H..8c...H..0
004006B0 63 DE 00 08 48 00 07 28 2C 9F 00 2A 40 86 00 24 c...H..(,..*@..$
004006C0 38 61 02 E0 48 00 08 B1 60 74 00 00 2C 94 00 00 8a..H...`t..,...
004006D0 40 84 07 0C 63 DE 00 04 7E 94 00 D0 48 00 07 00 @...c...~...H...
004006E0 1E 94 00 0A 7F FF A2 14 3A 9F FF D0 48 00 06 F0 ........:...H...
004006F0 3B 80 00 00 48 00 06 E8 2C 9F 00 2A 40 86 00 20 ;...H...,..*@.. 
00400700 38 61 02 E0 48 00 08 71 60 7C 00 00 2C 9C 00 00 8a..H..q`|..,...
00400710 40 84 06 CC 3B 80 FF FF 48 00 06 C4 1F 9C 00 0A @...;...H.......
00400720 7F FF E2 14 3B 9F FF D0 48 00 06 B4 2C 9F 00 68 ....;...H...,..h
00400730 40 86 06 AC 63 DE 00 20 48 00 06 A4 2C 9F 00 74 @...c.. H...,..t
00400740 40 86 00 10 8B F7 00 00 3A F7 00 01 7F FF 07 74 @.......:......t
00400750 63 EB 00 00 39 6B FF BB 28 8B 00 34 40 84 05 98 c...9k..(..4@...
00400760 55 6B 10 3A 48 00 00 D5 48 00 00 E0 48 00 05 88 Uk.:H...H...H...
00400770 48 00 00 D8 48 00 05 80 48 00 05 7C 48 00 05 78 H...H...H..|H..x
00400780 48 00 05 74 48 00 05 70 48 00 05 6C 48 00 05 68 H..tH..pH..lH..h
00400790 48 00 05 64 48 00 05 60 48 00 05 5C 48 00 05 58 H..dH..`H..\H..X
004007A0 48 00 05 54 48 00 05 50 48 00 05 4C 48 00 05 48 H..TH..PH..LH..H
004007B0 48 00 05 44 48 00 03 10 48 00 05 3C 48 00 05 38 H..DH...H..<H..8
004007C0 48 00 05 34 48 00 05 30 48 00 05 2C 48 00 05 28 H..4H..0H..,H..(
004007D0 48 00 05 24 48 00 05 20 48 00 05 1C 48 00 05 18 H..$H.. H...H...
004007E0 48 00 00 78 48 00 00 90 48 00 04 2C 48 00 04 28 H..xH...H..,H..(
004007F0 48 00 04 24 48 00 05 00 48 00 00 7C 48 00 04 F8 H..$H...H..|H...
00400800 48 00 04 F4 48 00 04 F0 48 00 04 EC 48 00 00 74 H...H...H...H..t
00400810 48 00 00 A4 48 00 00 B4 48 00 04 DC 48 00 04 D8 H...H...H...H...
00400820 48 00 00 BC 48 00 04 D0 48 00 01 2C 48 00 04 C8 H...H...H..,H...
00400830 48 00 01 2C 48 00 02 88                         H..,H...        

;; fn00400838: 00400838
fn00400838 proc
	mflr	r0
	add	r0,r0,r11
	mtlr	r0
	blr
00400848                         3B FF 00 20 7F FF 07 74         ;.. ...t
00400850 39 E0 00 01 48 00 03 C0 38 61 02 E0 48 00 07 19 9...H...8a..H...
00400860 7C 63 07 74 98 7A 00 00 63 5D 00 00 3B 60 00 01 |c.t.z..c]..;`..
00400870 48 00 04 84 63 DE 00 40 3B 60 00 0A 48 00 02 74 H...c..@;`..H..t
00400880 38 61 02 E0 48 00 06 F1 73 C4 00 20 60 6B 00 00 8a..H...s.. `k..
00400890 41 82 00 14 80 81 00 38 7C 84 07 34 B0 8B 00 00 A......8|..4....
004008A0 48 00 00 0C 80 81 00 38 90 8B 00 00 3A 60 00 01 H......8....:`..
004008B0 48 00 04 44 73 C4 00 80 3B 60 00 08 41 82 02 34 H..Ds...;`..A..4
004008C0 63 DE 02 00 48 00 02 2C 3B 80 00 08 38 80 FF DF c...H..,;...8...
004008D0 7F DE 20 38 63 DE 00 10 48 00 01 EC 38 61 02 E0 .. 8c...H...8a..
004008E0 48 00 06 95 60 7D 00 00 28 9D 00 00 40 86 00 0C H...`}..(...@...
004008F0 80 8D 00 00 60 9D 00 00 2C 9C FF FF 40 86 00 14 ....`...,...@...
00400900 3C 80 7F FF 60 84 FF FF 60 8B 00 00 48 00 00 08 <...`...`...H...
00400910 63 8B 00 00 61 64 00 00 2C 84 00 00 39 6B FF FF c...ad..,...9k..
00400920 63 AA 00 00 41 86 00 28 39 5D FF FF 8C 8A 00 01 c...A..(9]......
00400930 2C 84 00 00 41 86 00 18 61 64 00 00 2C 84 00 00 ,...A...ad..,...
00400940 39 6B FF FF 40 86 FF E8 39 4A 00 01 7F 7D 50 50 9k..@...9J...}PP
00400950 48 00 03 A4 3B 60 00 0A 48 00 01 98 88 97 00 00 H...;`..H.......
00400960 3A F7 00 01 7C 84 07 74 60 8B 00 00 2C 8B 00 63 :...|..t`...,..c
00400970 41 86 00 14 2C 8B 00 73 41 86 00 48 3A 60 00 01 A...,..sA..H:`..
00400980 48 00 03 74 38 61 02 E0 48 00 06 01 B0 61 02 6C H..t8a..H....a.l
00400990 A8 81 02 6C 3B 41 00 40 63 43 00 00 70 84 FF FF ...l;A.@cC..p...
004009A0 48 00 0B B5 60 00 00 00 60 7B 00 00 2C 9B 00 00 H...`...`{..,...
004009B0 63 5D 00 00 40 84 03 40 3A 60 00 01 48 00 03 38 c]..@..@:`..H..8
004009C0 38 61 02 E0 48 00 05 B1 60 7D 00 00 28 9D 00 00 8a..H...`}..(...
004009D0 40 86 00 48 81 4D 00 00 39 60 00 00 88 8A 00 00 @..H.M..9`......
004009E0 61 5D 00 00 2C 84 00 00 41 86 00 14 39 6B 00 01 a]..,...A...9k..
004009F0 7C 8A 58 AE 2C 84 00 00 40 86 FF F4 2C 9C FF FF |.X.,...@...,...
00400A00 61 7B 00 00 40 85 02 F0 7C 9C D8 00 40 84 02 E8 a{..@...|...@...
00400A10 63 9B 00 00 48 00 02 E0 2C 9C FF FF 40 85 00 0C c...H...,...@...
00400A20 63 9F 00 00 48 00 00 08 3B E0 02 00 63 A4 00 00 c...H...;...c...
00400A30 63 E5 00 00 3B 41 00 40 63 43 00 00 48 00 0A 69 c...;A.@cC..H..i
00400A40 60 00 00 00 73 C4 00 80 60 7B 00 00 63 5D 00 00 `...s...`{..c]..
00400A50 41 82 00 5C 63 E4 00 00 2C 84 00 00 3B FF FF FF A..\c...,...;...
00400A60 63 4B 00 00 41 86 00 44 39 6B FF FF 8D 4B 00 01 cK..A..D9k...K..
00400A70 2C 8A 00 00 41 86 00 34 80 90 00 00 71 4A 00 FF ,...A..4....qJ..
00400A80 55 4A 08 3C 7C 84 52 2E 70 84 80 00 41 82 00 08 UJ.<|.R.p...A...
00400A90 39 6B 00 01 63 E4 00 00 2C 84 00 00 3B FF FF FF 9k..c...,...;...
00400AA0 40 86 FF CC 39 6B 00 01 7F 7A 58 50 2C 9B 00 00 @...9k...zXP,...
00400AB0 40 84 02 44 3A 60 00 01 48 00 02 3C 3A 40 00 27 @..D:`..H..<:@.'
00400AC0 48 00 00 08 3A 40 00 07 73 C4 00 80 3B 60 00 10 H...:@..s...;`..
00400AD0 41 82 00 20 7E 45 07 74 38 A5 00 51 7C A5 07 74 A.. ~E.t8..Q|..t
00400AE0 98 B5 00 01 3B 00 00 02 38 80 00 30 98 95 00 00 ....;...8..0....
00400AF0 73 C4 00 20 41 82 00 30 73 DF 00 40 41 82 00 14 s.. A..0s..@A...
00400B00 38 61 02 E0 48 00 04 71 7C 63 07 34 48 00 00 34 8a..H..q|c.4H..4
00400B10 38 61 02 E0 48 00 04 61 7C 63 07 34 70 63 FF FF 8a..H..a|c.4pc..
00400B20 48 00 00 20 73 DF 00 40 41 82 00 10 38 61 02 E0 H.. s..@A...8a..
00400B30 48 00 04 45 48 00 00 0C 38 61 02 E0 48 00 04 39 H..EH...8a..H..9
00400B40 2C 9F 00 00 41 86 00 18 2C 83 00 00 40 84 00 10 ,...A...,...@...
00400B50 7D 43 00 D0 63 DE 01 00 48 00 00 08 60 6A 00 00 }C..c...H...`j..
00400B60 2C 9C 00 00 40 84 00 0C 3B 80 00 01 48 00 00 0C ,...@...;...H...
00400B70 38 80 FF F7 7F DE 20 38 28 8A 00 00 40 86 00 08 8..... 8(...@...
00400B80 3B 00 00 00 39 3A 01 FF 61 3D 00 00 63 84 00 00 ;...9:..a=..c...
00400B90 2C 84 00 00 3B 9C FF FF 41 85 00 0C 28 8A 00 00 ,...;...A...(...
00400BA0 41 86 00 38 63 64 00 00 7C AA 23 96 7C A5 21 D6 A..8cd..|.#.|.!.
00400BB0 7C A5 50 50 39 65 00 30 2C 8B 00 39 7D 4A 23 96 |.PP9e.0,..9}J#.
00400BC0 40 85 00 08 7D 6B 92 14 7D 6B 07 74 99 7D 00 00 @...}k..}k.t.}..
00400BD0 3B BD FF FF 4B FF FF B8 73 C4 02 00 7F 7D 48 50 ;...K...s....}HP
00400BE0 3B BD 00 01 41 82 01 10 88 9D 00 00 7C 84 07 74 ;...A.......|..t
00400BF0 2C 84 00 30 40 86 00 0C 2C 9B 00 00 40 86 00 F8 ,..0@...,...@...
00400C00 3B BD FF FF 3B 7B 00 01 38 80 00 30 98 9D 00 00 ;...;{..8..0....
00400C10 48 00 00 E4 2C 9C 00 00 63 DE 00 40 63 5D 00 00 H...,...c..@c]..
00400C20 40 84 00 0C 3B 80 00 06 48 00 00 18 2C 9C 00 00 @...;...H...,...
00400C30 40 86 00 10 2C 9F 00 67 40 86 00 08 3B 80 00 01 @...,..g@...;...
00400C40 80 61 02 E0 83 71 00 00 63 44 00 00 63 86 00 00 .a...q..cD..c...
00400C50 61 E7 00 00 63 6C 00 00 63 E5 00 00 48 00 32 B5 a...cl..c...H.2.
00400C60 80 41 00 14 73 D9 00 80 80 81 02 E0 38 84 00 08 .A..s.......8...
00400C70 90 81 02 E0 41 82 00 20 2C 9C 00 00 40 86 00 18 ....A.. ,...@...
00400C80 83 71 00 0C 63 43 00 00 63 6C 00 00 48 00 32 85 .q..cC..cl..H.2.
00400C90 80 41 00 14 2C 9F 00 67 40 86 00 20 2C 99 00 00 .A..,..g@.. ,...
00400CA0 40 86 00 18 83 F1 00 04 63 43 00 00 63 EC 00 00 @.......cC..c...
00400CB0 48 00 32 61 80 41 00 14 88 9A 00 00 7C 84 07 74 H.2a.A......|..t
00400CC0 2C 84 00 2D 40 86 00 0C 63 DE 01 00 3B BA 00 01 ,..-@...c...;...
00400CD0 88 9D 00 00 39 60 00 00 2C 84 00 00 41 86 00 14 ....9`..,...A...
00400CE0 39 6B 00 01 7C 9D 58 AE 2C 84 00 00 40 86 FF F4 9k..|.X.,...@...
00400CF0 61 7B 00 00 2C 93 00 00 40 86 00 E4 73 C4 00 40 a{..,...@...s..@
00400D00 41 82 00 48 73 C4 01 00 41 82 00 14 3B 00 00 01 A..Hs...A...;...
00400D10 38 80 00 2D 98 95 00 00 48 00 00 30 73 C4 00 01 8..-....H..0s...
00400D20 41 82 00 14 3B 00 00 01 38 80 00 2B 98 95 00 00 A...;...8..+....
00400D30 48 00 00 18 73 C4 00 02 41 82 00 10 3B 00 00 01 H...s...A...;...
00400D40 38 80 00 20 98 95 00 00 73 C4 00 0C 7F 3B A0 50 8.. ....s....;.P
00400D50 7F 38 C8 50 40 82 00 18 63 24 00 00 62 C5 00 00 .8.P@...c$..b...
00400D60 38 C1 00 38 38 60 00 20 48 00 01 35 63 04 00 00 8..88`. H..5c...
00400D70 62 C5 00 00 3A A1 02 40 62 A3 00 00 3B E1 00 38 b...:..@b...;..8
00400D80 63 E6 00 00 48 00 01 7D 73 C4 00 08 41 82 00 20 c...H..}s...A.. 
00400D90 73 C4 00 04 40 82 00 18 63 24 00 00 62 C5 00 00 s...@...c$..b...
00400DA0 63 E6 00 00 38 60 00 30 48 00 00 F5 63 A3 00 00 c...8`.0H...c...
00400DB0 63 64 00 00 62 C5 00 00 63 E6 00 00 48 00 01 45 cd..b...c...H..E
00400DC0 73 C4 00 04 41 82 00 18 63 24 00 00 62 C5 00 00 s...A...c$..b...
00400DD0 63 E6 00 00 38 60 00 20 48 00 00 C5             c...8`. H...    

l00400DDC:
	lbz	r31,0(r23)
	addi	r23,r23,+0001
	extsb.	r31,r31
	bne	$00400508

l00400DEC:
	lwz	r12,712(r1)
	lwz	r3,56(r1)
	mtlr	r12
	lmw	r13,624(r1)
	addi	r1,r1,+02C0
	blr

;; fn00400E04: 00400E04
;;   Called from:
;;     004005DC (in fn004004B4)
;;     00400600 (in fn004004B4)
;;     00400ED8 (in fn00400E9C)
;;     00400F4C (in fn00400F00)
fn00400E04 proc
	stwu	r1,-64(r1)
	stw	r31,56(r1)
	ori	r31,r5,0000
	lwz	r5,4(r4)
	mflr	r12
	addi	r5,r5,-0001
	cmpwi	cr1,r5,-0001
	stw	r12,72(r1)
	stw	r5,4(r4)
	ble	cr1,$00400E5C

l00400E2C:
	lwz	r5,0(r4)
	extsb	r3,r3
	stb	r3,0(r5)
	lwz	r6,0(r4)
	lbz	r7,0(r6)
	addi	r6,r6,+0001
	stw	r6,0(r4)
	extsb	r7,r7
	andi.	r7,r7,00FF
	extsh	r7,r7
	ori	r11,r7,0000
	b	$00400E68

l00400E5C:
	bl	fn00401598
	nop
	ori	r11,r3,0000

l00400E68:
	cmpwi	cr1,r11,-0001
	bne	cr1,$00400E7C

l00400E70:
	addi	r4,r0,-0001
	stw	r4,0(r31)
	b	$00400E88

l00400E7C:
	lwz	r4,0(r31)
	addi	r4,r4,+0001
	stw	r4,0(r31)

l00400E88:
	lwz	r12,72(r1)
	lwz	r31,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	blr

;; fn00400E9C: 00400E9C
fn00400E9C proc
	stwu	r1,-72(r1)
	stmw	r28,56(r1)
	ori	r31,r4,0000
	ori	r4,r31,0000
	cmpwi	cr1,r4,+0000
	mflr	r12
	ori	r28,r3,0000
	ori	r29,r5,0000
	ori	r30,r6,0000
	stw	r12,80(r1)
	addi	r31,r31,-0001
	ble	cr1,$00400EEC

l00400ECC:
	ori	r3,r28,0000
	ori	r4,r29,0000
	ori	r5,r30,0000
	bl	fn00400E04
	ori	r4,r31,0000
	cmpwi	cr1,r4,+0000
	addi	r31,r31,-0001
	bgt	cr1,$00400ECC

l00400EEC:
	lwz	r12,80(r1)
	lmw	r28,56(r1)
	mtlr	r12
	addi	r1,r1,+0048
	blr

;; fn00400F00: 00400F00
fn00400F00 proc
	stwu	r1,-72(r1)
	stmw	r28,56(r1)
	ori	r31,r4,0000
	ori	r4,r31,0000
	cmpwi	cr1,r4,+0000
	mflr	r12
	ori	r30,r3,0000
	ori	r28,r5,0000
	ori	r29,r6,0000
	stw	r12,80(r1)
	addi	r31,r31,-0001
	ble	cr1,$00400F60

l00400F30:
	ori	r4,r30,0000
	lbz	r4,0(r4)
	ori	r5,r29,0000
	extsb	r4,r4
	addi	r30,r30,+0001
	ori	r3,r4,0000
	ori	r4,r28,0000
	bl	fn00400E04
	ori	r4,r31,0000
	cmpwi	cr1,r4,+0000
	addi	r31,r31,-0001
	bgt	cr1,$00400F30

l00400F60:
	lwz	r12,80(r1)
	lmw	r28,56(r1)
	mtlr	r12
	addi	r1,r1,+0048
	blr

;; fn00400F74: 00400F74
fn00400F74 proc
	lwz	r4,0(r3)
	addi	r4,r4,+0004
	stw	r4,0(r3)
	lwz	r3,-4(r4)
	blr

;; fn00400F88: 00400F88
fn00400F88 proc
	lwz	r4,0(r3)
	addi	r4,r4,+0004
	stw	r4,0(r3)
	lha	r3,-4(r4)
	blr

;; fn00400F9C: 00400F9C
;;   Called from:
;;     004003B0 (in fn004002F8)
;;     004029B4 (in fn0040298C)
;;     00403504 (in fn00403450)
fn00400F9C proc
	lwz	r4,100(r2)
	mflr	r12
	lwz	r4,0(r4)
	stwu	r1,-56(r1)
	stw	r12,64(r1)
	bl	fn00400FC4
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	blr

;; fn00400FC4: 00400FC4
;;   Called from:
;;     00400FB0 (in fn00400F9C)
fn00400FC4 proc
	stwu	r1,-64(r1)
	stmw	r30,56(r1)
	ori	r31,r3,0000
	ori	r30,r4,0000
	addi	r4,r0,-0020
	cmplw	cr1,r31,r4
	mflr	r12
	stw	r12,72(r1)
	ble	cr1,$00400FF8

l00400FE8:
	lmw	r30,56(r1)
	addi	r1,r1,+0040
	addi	r3,r0,+0000
	blr

l00400FF8:
	addi	r31,r31,+0007
	addi	r4,r0,-0008
	and	r31,r31,r4

l00401004:
	ori	r3,r31,0000
	bl	fn0040105C
	cmplwi	cr1,r3,0000
	bne	cr1,$00401048

l00401014:
	cmpwi	cr1,r30,+0000
	beq	cr1,$00401048

l0040101C:
	ori	r3,r31,0000
	bl	fn004017F0
	nop
	cmpwi	cr1,r3,+0000
	bne	cr1,$00401004

l00401030:
	lwz	r12,72(r1)
	lmw	r30,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	addi	r3,r0,+0000
	blr

l00401048:
	lwz	r12,72(r1)
	lmw	r30,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	blr

;; fn0040105C: 0040105C
;;   Called from:
;;     00401008 (in fn00400FC4)
fn0040105C proc
	mflr	r12
	stwu	r1,-72(r1)
	stw	r12,80(r1)
	stmw	r29,56(r1)
	ori	r30,r3,0000
	ori	r3,r30,0000
	bl	fn00401F40
	nop
	ori	r31,r3,0000
	cmplwi	cr1,r31,0000
	bne	cr1,$004010D8

l00401088:
	ori	r3,r30,0000
	bl	fn00401ADC
	nop
	cmpwi	cr1,r3,-0001
	beq	cr1,$004010C0

l0040109C:
	ori	r3,r30,0000
	bl	fn00401F40
	nop
	ori	r31,r3,0000
	cmplwi	cr1,r31,0000
	bne	cr1,$004010D8

l004010B4:
	bl	fn00401AB4
	nop
	b	$004010D8

l004010C0:
	lwz	r12,80(r1)
	lmw	r30,60(r1)
	mtlr	r12
	addi	r1,r1,+0048
	addi	r3,r0,+0000
	blr

l004010D8:
	lwz	r4,0(r31)
	lwz	r5,4(r31)
	lwz	r4,4(r4)
	addi	r29,r0,-0004
	and	r4,r4,r29
	and	r5,r5,r29
	subf	r4,r5,r4
	subf	r4,r30,r4
	cmplwi	cr1,r4,0008
	beq	cr1,$00401128

l00401100:
	ori	r3,r31,0000
	ori	r4,r30,0000
	bl	fn0040115C
	cmplwi	cr1,r3,0000
	beq	cr1,$00401128

l00401114:
	lwz	r4,4(r3)
	addi	r5,r0,-0003
	and	r4,r4,r5
	ori	r4,r4,0001
	stw	r4,4(r3)

l00401128:
	lwz	r12,80(r1)
	lwz	r4,4(r31)
	mtlr	r12
	lwz	r5,0(r31)
	lwz	r6,104(r2)
	and	r4,r4,r29
	stw	r4,4(r31)
	stw	r5,4(r6)
	and	r4,r4,r29
	addi	r3,r4,+0008
	lmw	r29,56(r1)
	addi	r1,r1,+0048
	blr

;; fn0040115C: 0040115C
;;   Called from:
;;     00401108 (in fn0040105C)
fn0040115C proc
	stwu	r1,-72(r1)
	stmw	r29,56(r1)
	ori	r31,r3,0000
	ori	r29,r4,0000
	lwz	r4,0(r31)
	lwz	r5,4(r31)
	lwz	r4,4(r4)
	addi	r30,r0,-0004
	and	r5,r5,r30
	and	r4,r4,r30
	subf	r4,r5,r4
	addi	r4,r4,-0008
	cmplw	cr1,r4,r29
	mflr	r12
	stw	r12,80(r1)
	ble	cr1,$004011E4

l0040119C:
	bl	fn004019CC
	nop
	cmplwi	cr1,r3,0000
	beq	cr1,$004011E4

l004011AC:
	lwz	r4,4(r31)
	lwz	r12,80(r1)
	and	r30,r4,r30
	add	r29,r30,r29
	addi	r5,r29,+0008
	stw	r5,4(r3)
	stw	r3,8(r29)
	mtlr	r12
	lwz	r6,0(r31)
	stw	r6,0(r3)
	stw	r3,0(r31)
	lmw	r29,56(r1)
	addi	r1,r1,+0048
	blr

l004011E4:
	lwz	r12,80(r1)
	lmw	r29,56(r1)
	mtlr	r12
	addi	r1,r1,+0048
	addi	r3,r0,+0000
	blr

;; fn004011FC: 004011FC
;;   Called from:
;;     004013CC (in fn00401380)
;;     004013F8 (in fn00401380)
;;     00402AA4 (in fn00402A24)
fn004011FC proc
	stwu	r1,-64(r1)
	stw	r31,56(r1)
	ori	r31,r3,0000
	cmplwi	cr1,r31,0000
	mflr	r12
	stw	r12,72(r1)
	bne	cr1,$00401234

l00401218:
	addi	r3,r0,+0000
	bl	fn00401380
	lwz	r12,72(r1)
	lwz	r31,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	blr

l00401234:
	ori	r3,r31,0000
	bl	fn004012B0
	cmpwi	cr1,r3,+0000
	beq	cr1,$0040125C

l00401244:
	lwz	r12,72(r1)
	lwz	r31,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	addi	r3,r0,-0001
	blr

l0040125C:
	lwz	r4,12(r31)
	andi.	r4,r4,4000
	beq	$00401298

l00401268:
	lwz	r3,16(r31)
	bl	fn004020BC
	nop
	cmpwi	cr1,r3,+0000
	addi	r3,r0,-0001
	bne	cr1,$00401284

l00401280:
	addi	r3,r0,+0000

l00401284:
	lwz	r12,72(r1)
	lwz	r31,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	blr

l00401298:
	lwz	r12,72(r1)
	lwz	r31,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	addi	r3,r0,+0000
	blr

;; fn004012B0: 004012B0
;;   Called from:
;;     0040045C (in fn00400430)
;;     00400498 (in fn00400430)
;;     00401238 (in fn004011FC)
;;     004031B8 (in fn00403170)
;;     004034B8 (in fn00403450)
fn004012B0 proc
	stwu	r1,-72(r1)
	stmw	r29,56(r1)
	ori	r31,r3,0000
	lwz	r11,12(r31)
	mflr	r12
	andi.	r4,r11,0003
	cmpwi	cr1,r4,+0002
	stw	r12,80(r1)
	addi	r29,r0,+0000
	bne	cr1,$00401334

l004012D8:
	andi.	r11,r11,0108
	beq	$00401334

l004012E0:
	lwz	r4,8(r31)
	lwz	r30,0(r31)
	subf.	r30,r4,r30
	ble	$00401334

l004012F0:
	lwz	r3,16(r31)
	ori	r5,r30,0000
	bl	fn00402248
	nop
	cmp	cr1,r3,r30
	bne	cr1,$00401324

l00401308:
	lwz	r11,12(r31)
	andi.	r4,r11,0080
	beq	$00401334

l00401314:
	addi	r4,r0,-0003
	and	r11,r11,r4
	stw	r11,12(r31)
	b	$00401334

l00401324:
	lwz	r4,12(r31)
	addi	r29,r0,-0001
	ori	r4,r4,0020
	stw	r4,12(r31)

l00401334:
	lwz	r12,80(r1)
	lwz	r4,8(r31)
	mtlr	r12
	ori	r3,r29,0000
	addi	r5,r0,+0000
	stw	r5,4(r31)
	stw	r4,0(r31)
	lmw	r29,56(r1)
	addi	r1,r1,+0048
	blr

;; fn0040135C: 0040135C
fn0040135C proc
	mflr	r12
	stwu	r1,-56(r1)
	stw	r12,64(r1)
	addi	r3,r0,+0001
	bl	fn00401380
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	blr

;; fn00401380: 00401380
;;   Called from:
;;     0040121C (in fn004011FC)
;;     0040136C (in fn0040135C)
fn00401380 proc
	stwu	r1,-80(r1)
	stmw	r27,56(r1)
	lwz	r27,108(r2)
	lwz	r4,72(r2)
	lwz	r5,0(r27)
	ori	r31,r4,0000
	cmplw	cr1,r5,r4
	mflr	r12
	ori	r30,r3,0000
	stw	r12,88(r1)
	addi	r29,r0,+0000
	addi	r28,r0,+0000
	blt	cr1,$00401418

l004013B4:
	cmpwi	cr1,r30,+0001
	bne	cr1,$004013E0

l004013BC:
	lwz	r4,12(r31)
	andi.	r4,r4,0083
	beq	$004013E0

l004013C8:
	ori	r3,r31,0000
	bl	fn004011FC
	cmpwi	cr1,r3,-0001
	beq	cr1,$00401408

l004013D8:
	addi	r29,r29,+0001
	b	$00401408

l004013E0:
	cmpwi	cr1,r30,+0000
	bne	cr1,$00401408

l004013E8:
	lwz	r4,12(r31)
	andi.	r4,r4,0002
	beq	$00401408

l004013F4:
	ori	r3,r31,0000
	bl	fn004011FC
	cmpwi	cr1,r3,-0001
	bne	cr1,$00401408

l00401404:
	addi	r28,r0,-0001

l00401408:
	lwz	r4,0(r27)
	addi	r31,r31,+0020
	cmplw	cr1,r4,r31
	bge	cr1,$004013B4

l00401418:
	cmpwi	cr1,r30,+0001
	ori	r3,r29,0000
	beq	cr1,$00401428

l00401424:
	ori	r3,r28,0000

l00401428:
	lwz	r12,88(r1)
	lmw	r27,56(r1)
	mtlr	r12
	addi	r1,r1,+0050
	blr
0040143C                                     7D 88 02 A6             }...
00401440 94 21 FF C8 91 81 00 40 4B FF FF 15 80 82 00 70 .!.....@K......p
00401450 88 84 00 00 7C 84 07 75 41 82 00 0C 48 00 0F 3D ....|..uA...H..=
00401460 60 00 00 00 81 81 00 40 38 21 00 38 7D 88 03 A6 `......@8!.8}...
00401470 4E 80 00 20                                     N..             

;; fn00401474: 00401474
;;   Called from:
;;     00400310 (in fn004002F8)
;;     0040166C (in fn00401598)
fn00401474 proc
	lwz	r4,116(r2)
	ori	r5,r3,0000
	lwz	r4,0(r4)
	cmplw	cr1,r4,r5
	bgt	cr1,$00401490

l00401488:
	addi	r3,r0,+0000
	blr

l00401490:
	lwz	r4,120(r2)
	lbzx	r3,r4,r3
	extsb	r3,r3
	andi.	r3,r3,0040
	blr

;; fn004014A4: 004014A4
fn004014A4 proc
	ori	r10,r3,0000
	cmplwi	cr1,r10,0000
	addi	r3,r0,+0000
	beq	cr1,$004014C4

l004014B4:
	cmplwi	cr1,r5,0000
	bne	cr1,$004014C4

l004014BC:
	addi	r3,r0,+0000
	blr

l004014C4:
	cmplwi	cr1,r10,0000
	bne	cr1,$004014F4

l004014CC:
	ori	r11,r4,0000

l004014D0:
	ori	r5,r11,0000
	lhz	r5,0(r5)
	addi	r11,r11,+0002
	cmplwi	cr1,r5,0000
	bne	cr1,$004014D0

l004014E4:
	subf	r4,r4,r11
	srawi	r4,r4,01
	addi	r3,r4,-0001
	blr

l004014F4:
	cmplwi	cr1,r5,0000
	ble	cr1,$00401550

l004014FC:
	mtctr	r5

l00401500:
	lhz	r11,0(r4)
	cmplwi	cr1,r11,0100
	bge	cr1,$00401538

l0040150C:
	andi.	r11,r11,00FF
	extsb	r11,r11
	stbx	r11,r10,r3
	ori	r5,r4,0000
	lhz	r5,0(r5)
	addi	r4,r4,+0002
	cmplwi	cr1,r5,0000
	beq	cr1,$0040154C

l0040152C:
	addi	r3,r3,+0001
	bdnz	$00401500

l00401534:
	b	$00401550

l00401538:
	lwz	r4,124(r2)
	addi	r5,r0,+002A
	stw	r5,0(r4)
	addi	r3,r0,-0001
	blr

l0040154C:
	blr

l00401550:
	blr

;; fn00401554: 00401554
fn00401554 proc
	cmplwi	cr1,r3,0000
	andi.	r4,r4,FFFF
	bne	cr1,$00401568

l00401560:
	addi	r3,r0,+0000
	blr

l00401568:
	cmplwi	cr1,r4,00FF
	ble	cr1,$00401584

l00401570:
	lwz	r4,124(r2)
	addi	r5,r0,+002A
	stw	r5,0(r4)
	addi	r3,r0,-0001
	blr

l00401584:
	andi.	r4,r4,00FF
	extsb	r4,r4
	stb	r4,0(r3)
	addi	r3,r0,+0001
	blr

;; fn00401598: 00401598
;;   Called from:
;;     00400E5C (in fn00400E04)
;;     004033A4 (in fn004032FC)
fn00401598 proc
	stwu	r1,-80(r1)
	stmw	r27,60(r1)
	ori	r31,r4,0000
	lwz	r10,12(r31)
	mflr	r12
	ori	r11,r10,0000
	andi.	r4,r11,0082
	ori	r27,r3,0000
	stw	r12,88(r1)
	lwz	r30,16(r31)
	beq	$00401774

l004015C4:
	andi.	r4,r11,0040
	bne	$00401774

l004015CC:
	andi.	r11,r11,0001
	beq	$00401620

l004015D4:
	lwz	r4,12(r31)
	addi	r9,r0,+0000
	ori	r11,r4,0000
	andi.	r5,r11,0010
	stw	r9,4(r31)
	beq	$00401608

l004015EC:
	lwz	r10,12(r31)
	lwz	r4,8(r31)
	addi	r5,r0,-0002
	stw	r4,0(r31)
	and	r10,r10,r5
	stw	r10,12(r31)
	b	$00401624

l00401608:
	ori	r11,r11,0020
	stw	r11,12(r31)
	lmw	r27,60(r1)
	addi	r1,r1,+0050
	addi	r3,r0,-0001
	blr

l00401620:
	addi	r9,r0,+0000

l00401624:
	ori	r10,r10,0002
	stw	r10,12(r31)
	addi	r4,r0,-0011
	and	r10,r10,r4
	stw	r10,12(r31)
	lwz	r10,12(r31)
	stw	r9,4(r31)
	andi.	r10,r10,010C
	addi	r28,r0,+0000
	bne	$00401688

l0040164C:
	lwz	r11,72(r2)
	addi	r4,r11,+0020
	cmplw	cr1,r31,r4
	beq	cr1,$00401668

l0040165C:
	addi	r11,r11,+0040
	cmplw	cr1,r31,r11
	bne	cr1,$0040167C

l00401668:
	ori	r3,r30,0000
	bl	fn00401474
	nop
	cmpwi	cr1,r3,+0000
	bne	cr1,$00401688

l0040167C:
	ori	r3,r31,0000
	bl	fn0040298C
	nop

l00401688:
	lwz	r4,12(r31)
	andi.	r4,r4,0108
	beq	$0040170C

l00401694:
	lwz	r29,0(r31)
	lwz	r4,8(r31)
	lwz	r5,24(r31)
	subf.	r29,r4,r29
	addi	r4,r4,+0001
	stw	r4,0(r31)
	addi	r5,r5,-0001
	stw	r5,4(r31)
	ble	$004016D4

l004016B8:
	lwz	r4,8(r31)
	ori	r3,r30,0000
	ori	r5,r29,0000
	bl	fn00402248
	nop
	ori	r28,r3,0000
	b	$004016FC

l004016D4:
	lwz	r4,120(r2)
	lbzx	r4,r4,r30
	extsb	r4,r4
	andi.	r4,r4,0020
	beq	$004016FC

l004016E8:
	ori	r3,r30,0000
	addi	r4,r0,+0000
	addi	r5,r0,+0002
	bl	fn004026B8
	nop

l004016FC:
	lwz	r4,8(r31)
	extsb	r5,r27
	stb	r5,0(r4)
	b	$00401730

l0040170C:
	ori	r3,r30,0000
	addi	r4,r1,+0038
	extsb	r6,r27
	addi	r29,r0,+0001
	stb	r6,56(r1)
	addi	r5,r0,+0001
	bl	fn00402248
	nop
	ori	r28,r3,0000

l00401730:
	cmp	cr1,r28,r29
	beq	cr1,$0040175C

l00401738:
	lwz	r12,88(r1)
	lwz	r4,12(r31)
	mtlr	r12
	ori	r4,r4,0020
	stw	r4,12(r31)
	lmw	r27,60(r1)
	addi	r1,r1,+0050
	addi	r3,r0,-0001
	blr

l0040175C:
	lwz	r12,88(r1)
	andi.	r3,r27,00FF
	mtlr	r12
	lmw	r27,60(r1)
	addi	r1,r1,+0050
	blr

l00401774:
	ori	r11,r11,0020
	stw	r11,12(r31)
	lmw	r27,60(r1)
	addi	r1,r1,+0050
	addi	r3,r0,-0001
	blr

;; fn0040178C: 0040178C
fn0040178C proc
	lwz	r4,128(r2)
	lwz	r5,0(r4)
	stw	r3,0(r4)
	ori	r3,r5,0000
	blr
004017A0 80 82 00 80 80 64 00 00 4E 80 00 20 28 83 00 00 .....d..N.. (...
004017B0 7D 88 02 A6 94 21 FF C8 91 81 00 40 41 86 00 18 }....!.....@A...
004017C0 80 62 00 84 80 82 00 88 38 A0 00 72 48 00 12 59 .b......8..rH..Y
004017D0 60 00 00 00 38 60 00 00 4B FF FF B5 81 81 00 40 `...8`..K......@
004017E0 38 21 00 38 7D 88 03 A6 38 60 00 00 4E 80 00 20 8!.8}...8`..N.. 

;; fn004017F0: 004017F0
;;   Called from:
;;     00401020 (in fn00400FC4)
fn004017F0 proc
	lwz	r4,128(r2)
	mflr	r12
	lwz	r4,0(r4)
	stwu	r1,-56(r1)
	cmplwi	cr1,r4,0000
	stw	r12,64(r1)
	beq	cr1,$00401838

l0040180C:
	lwz	r11,128(r2)
	lwz	r12,0(r11)
	bl	fn00403F10
	lwz	r2,20(r1)
	cmpwi	cr1,r3,+0000
	beq	cr1,$00401838

l00401824:
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	addi	r3,r0,+0001
	blr

l00401838:
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	addi	r3,r0,+0000
	blr
0040184C                                     94 21 FF A8             .!..
00401850 BF 41 00 3C 83 E2 00 8C 83 62 00 90 80 9F 00 00 .A.<.....b......
00401860 7D 88 02 A6 28 84 00 00 80 BB 00 00 91 81 00 60 }...(..........`
00401870 60 BA 00 00 40 86 00 90 38 60 02 00 48 00 28 29 `...@...8`..H.()
00401880 80 41 00 14 90 7F 00 00 80 7F 00 00 28 83 00 00 .A..........(...
00401890 40 86 00 1C 3B 80 FD A3 B3 80 0A F0 48 00 26 E9 @...;.......H.&.
004018A0 80 41 00 14 80 7F 00 00 48 00 00 08 3B 80 FD A3 .A......H...;...
004018B0 48 00 28 0D 80 41 00 14 83 FF 00 00 39 20 00 20 H.(..A......9 . 
004018C0 7D 29 03 A6 81 5F 00 00 39 60 00 00 39 20 00 00 })..._..9`..9 ..
004018D0 7D 2A 59 2E 39 6B 00 04 7D 2A 59 2E 39 6B 00 04 }*Y.9k..}*Y.9k..
004018E0 7D 2A 59 2E 39 6B 00 04 7D 2A 59 2E 39 6B 00 04 }*Y.9k..}*Y.9k..
004018F0 42 00 FF E0 80 82 00 94 38 A0 00 20 90 A4 00 00 B.......8.. ....
00401900 48 00 00 08 3B 80 FD A3 38 60 10 00 38 80 10 00 H...;...8`..8...
00401910 90 9B 00 00 48 00 27 F1 80 41 00 14 60 7F 00 00 ....H.'..A..`...
00401920 28 9F 00 00 40 86 00 10 B3 80 0A F0 48 00 26 59 (...@.......H.&Y
00401930 80 41 00 14 80 9B 00 00 60 83 00 00 48 00 27 C9 .A......`...H.'.
00401940 80 41 00 14 60 7E 00 00 28 9E 00 00 40 86 00 10 .A..`~..(...@...
00401950 B3 80 0A F0 48 00 26 31 80 41 00 14 28 9F 00 00 ....H.&1.A..(...
00401960 41 86 00 10 63 E3 00 00 48 00 27 CD 80 41 00 14 A...c...H.'..A..
00401970 28 9E 00 00 41 86 00 10 63 C3 00 00 48 00 27 B9 (...A...c...H.'.
00401980 80 41 00 14 38 60 00 04 4B FF F6 15 60 00 00 00 .A..8`..K...`...
00401990 60 7F 00 00 28 9F 00 00 40 86 00 10 B3 80 0A F0 `...(...@.......
004019A0 48 00 25 E5 80 41 00 14 63 E3 00 00 48 00 11 F5 H.%..A..c...H...
004019B0 60 00 00 00 81 81 00 60 7D 88 03 A6 93 5B 00 00 `......`}....[..
004019C0 BB 41 00 3C 38 21 00 58 4E 80 00 20             .A.<8!.XN..     

;; fn004019CC: 004019CC
;;   Called from:
;;     0040119C (in fn0040115C)
;;     00402D18 (in fn00402CE0)
;;     00402D30 (in fn00402CE0)
;;     00402D48 (in fn00402CE0)
fn004019CC proc
	stwu	r1,-64(r1)
	stw	r31,56(r1)
	lwz	r31,104(r2)
	mflr	r12
	lwz	r4,8(r31)
	stw	r12,72(r1)
	cmplwi	cr1,r4,0000
	bne	cr1,$00401A10

l004019EC:
	bl	fn00401A34
	cmpwi	cr1,r3,+0000
	bne	cr1,$00401A10

l004019F8:
	lwz	r12,72(r1)
	lwz	r31,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	addi	r3,r0,+0000
	blr

l00401A10:
	lwz	r4,8(r31)
	lwz	r12,72(r1)
	lwz	r5,0(r4)
	mtlr	r12
	stw	r5,8(r31)
	ori	r3,r4,0000
	lwz	r31,56(r1)
	addi	r1,r1,+0040
	blr

;; fn00401A34: 00401A34
;;   Called from:
;;     004019EC (in fn004019CC)
fn00401A34 proc
	mflr	r12
	stwu	r1,-56(r1)
	stw	r12,64(r1)
	addi	r3,r0,+1000
	bl	fn00404104
	lwz	r2,20(r1)
	cmplwi	cr1,r3,0000
	bne	cr1,$00401A68

l00401A54:
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	addi	r3,r0,+0000
	blr

l00401A68:
	lwz	r4,104(r2)
	ori	r10,r3,0000
	stw	r3,8(r4)
	addi	r3,r3,+0FF8
	cmplw	cr1,r3,r10
	addi	r11,r10,+0008
	ble	cr1,$00401A98

l00401A84:
	stw	r11,0(r10)
	ori	r10,r11,0000
	cmplw	cr1,r3,r10
	addi	r11,r11,+0008
	bgt	cr1,$00401A84

l00401A98:
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	addi	r4,r0,+0000
	stw	r4,0(r3)
	addi	r3,r0,+0001
	blr

;; fn00401AB4: 00401AB4
;;   Called from:
;;     004010B4 (in fn0040105C)
;;     00401EB4 (in fn00401DD8)
;;     00402BD0 (in fn00402BA0)
fn00401AB4 proc
	mflr	r12
	stwu	r1,-56(r1)
	stw	r12,64(r1)
	addi	r3,r0,+0012
	bl	fn00402C4C
	nop
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	blr

;; fn00401ADC: 00401ADC
;;   Called from:
;;     0040108C (in fn0040105C)
fn00401ADC proc
	stwu	r1,-88(r1)
	stmw	r25,60(r1)
	lwz	r25,152(r2)
	lwz	r28,148(r2)
	lwz	r11,4(r25)
	lwz	r4,0(r28)
	ori	r30,r11,0000
	lwz	r11,4(r25)
	mflr	r12
	cmp	cr1,r11,r4
	ori	r26,r3,0000
	stw	r12,96(r1)
	addi	r26,r26,+0008
	addi	r26,r26,+0007
	addi	r27,r0,-0001
	addi	r5,r0,-0008
	and	r26,r26,r5
	bge	cr1,$00401BA0

l00401B24:
	rlwinm	r31,r11,04,00,1B
	lwz	r29,140(r2)

l00401B2C:
	lwz	r4,0(r29)
	lwz	r4,0(r4)
	lwzx	r4,r4,r31
	cmplwi	cr1,r4,0000
	beq	cr1,$00401B54

l00401B40:
	ori	r3,r30,0000
	ori	r4,r26,0000
	bl	fn00401DD8
	cmpwi	cr1,r3,-0001
	bne	cr1,$00401B80

l00401B54:
	lwz	r4,0(r29)
	lwz	r4,0(r4)
	lwzx	r4,r4,r31
	cmplwi	cr1,r4,0000
	beq	cr1,$00401B9C

l00401B68:
	lwz	r4,0(r28)
	addi	r30,r30,+0001
	cmp	cr1,r30,r4
	addi	r31,r31,+0010
	blt	cr1,$00401B2C

l00401B7C:
	b	$00401BA0

l00401B80:
	lwz	r12,96(r1)
	stw	r30,4(r25)
	mtlr	r12
	lmw	r25,60(r1)
	addi	r1,r1,+0058
	addi	r3,r0,+0000
	blr

l00401B9C:
	ori	r27,r30,0000

l00401BA0:
	cmpwi	cr1,r27,-0001
	lwz	r29,140(r2)
	lwz	r11,0(r28)
	lwz	r9,0(r29)
	bne	cr1,$00401CC8

l00401BB4:
	cmplwi	cr1,r9,0000
	addi	r11,r11,+0020
	rlwinm	r31,r11,04,00,1B
	beq	cr1,$00401BD8

l00401BC4:
	ori	r3,r9,0000
	ori	r4,r31,0000
	bl	fn0040408C
	lwz	r2,20(r1)
	lwz	r9,0(r29)

l00401BD8:
	cmplwi	cr1,r9,0000
	beq	cr1,$00401BF4

l00401BE0:
	lwz	r4,152(r2)
	lwz	r4,0(r4)
	lhz	r4,0(r4)
	cmplwi	cr1,r4,0000
	beq	cr1,$00401C74

l00401BF4:
	ori	r3,r31,0000
	bl	fn004040A4
	lwz	r2,20(r1)
	ori	r31,r3,0000
	cmplwi	cr1,r31,0000
	bne	cr1,$00401C24

l00401C0C:
	lwz	r12,96(r1)
	lmw	r25,60(r1)
	mtlr	r12
	addi	r1,r1,+0058
	addi	r3,r0,-0001
	blr

l00401C24:
	ori	r3,r31,0000
	bl	fn004040BC
	lwz	r2,20(r1)
	lwz	r9,0(r29)
	cmplwi	cr1,r9,0000
	beq	cr1,$00401C6C

l00401C3C:
	lwz	r4,0(r28)
	lwz	r3,0(r9)
	stw	r4,56(r1)
	lwz	r4,0(r31)
	lwz	r5,56(r1)
	rlwinm	r5,r5,04,00,1B
	bl	fn004040D4
	lwz	r2,20(r1)
	lwz	r4,0(r29)
	ori	r3,r4,0000
	bl	fn004040EC
	lwz	r2,20(r1)

l00401C6C:
	stw	r31,0(r29)
	lwz	r9,0(r29)

l00401C74:
	lwz	r9,0(r9)
	lwz	r4,0(r28)
	addi	r11,r0,+0000
	rlwinm	r10,r4,04,00,1B
	add	r10,r10,r9
	addi	r9,r0,+0020
	mtctr	r9
	addi	r9,r0,+0000

l00401C94:
	stwx	r9,r10,r11
	addi	r11,r11,+0004
	stwx	r9,r10,r11
	addi	r11,r11,+0004
	stwx	r9,r10,r11
	addi	r11,r11,+0004
	stwx	r9,r10,r11
	addi	r11,r11,+0004
	bdnz	$00401C94

l00401CB8:
	lwz	r4,0(r28)
	ori	r27,r4,0000
	addi	r4,r4,+0020
	stw	r4,0(r28)

l00401CC8:
	ori	r3,r27,0000
	ori	r4,r26,0000
	bl	fn00401CE8
	lwz	r12,96(r1)
	lmw	r25,60(r1)
	mtlr	r12
	addi	r1,r1,+0058
	blr

;; fn00401CE8: 00401CE8
;;   Called from:
;;     00401CD0 (in fn00401ADC)
fn00401CE8 proc
	stwu	r1,-72(r1)
	stmw	r29,56(r1)
	ori	r29,r4,0000
	lwz	r4,144(r2)
	addi	r5,r0,-1000
	lwz	r4,0(r4)
	mflr	r12
	addi	r31,r4,+0FFF
	and	r31,r31,r5
	cmplw	cr1,r29,r31
	ori	r30,r3,0000
	stw	r12,80(r1)
	ble	cr1,$00401D20

l00401D1C:
	ori	r31,r29,0000

l00401D20:
	addi	r3,r31,+0008
	bl	fn00404104
	lwz	r2,20(r1)
	ori	r10,r3,0000
	cmplwi	cr1,r10,0000
	beq	cr1,$00401DC0

l00401D38:
	ori	r9,r10,0000
	andi.	r4,r9,0003
	lwz	r5,140(r2)
	lwz	r11,0(r5)
	lwz	r11,0(r11)
	beq	$00401D6C

l00401D50:
	rlwinm	r8,r30,04,00,1B
	addi	r9,r9,+0007
	addi	r4,r0,-0008
	and	r9,r9,r4
	stwx	r9,r11,r8
	add	r8,r8,r11
	b	$00401D78

l00401D6C:
	rlwinm	r8,r30,04,00,1B
	stwx	r10,r11,r8
	add	r8,r8,r11

l00401D78:
	ori	r3,r30,0000
	ori	r4,r29,0000
	stw	r10,12(r8)
	stw	r31,8(r8)
	addi	r5,r0,+0000
	stw	r5,4(r8)
	bl	fn00401DD8
	cmpwi	cr1,r3,+0000
	beq	cr1,$00401DA8

l00401D9C:
	ori	r3,r30,0000
	bl	fn00401ED4
	b	$00401DC0

l00401DA8:
	lwz	r12,80(r1)
	lmw	r29,56(r1)
	mtlr	r12
	addi	r1,r1,+0048
	addi	r3,r0,+0000
	blr

l00401DC0:
	lwz	r12,80(r1)
	lmw	r29,56(r1)
	mtlr	r12
	addi	r1,r1,+0048
	addi	r3,r0,-0001
	blr

;; fn00401DD8: 00401DD8
;;   Called from:
;;     00401B48 (in fn00401ADC)
;;     00401D90 (in fn00401CE8)
fn00401DD8 proc
	stwu	r1,-80(r1)
	stmw	r28,60(r1)
	lwz	r29,140(r2)
	rlwinm	r31,r3,04,00,1B
	lwz	r5,0(r29)
	addi	r30,r4,+0007
	lwz	r5,0(r5)
	addi	r7,r0,-0008
	add	r11,r5,r31
	lwz	r8,4(r11)
	lwz	r9,8(r11)
	and	r30,r30,r7
	subf	r10,r8,r9
	cmplw	cr1,r30,r10
	mflr	r12
	lwzx	r3,r5,r31
	stw	r12,88(r1)
	add	r8,r8,r3
	ble	cr1,$00401E88

l00401E24:
	subf	r10,r10,r30
	add	r9,r10,r9
	addi	r28,r9,+0008
	and	r28,r28,r7
	ori	r4,r28,0000
	bl	fn0040411C
	lwz	r2,20(r1)
	lwz	r4,152(r2)
	lwz	r10,0(r29)
	lwz	r4,0(r4)
	lwz	r10,0(r10)
	lhz	r4,0(r4)
	add	r11,r10,r31
	cmplwi	cr1,r4,0000
	beq	cr1,$00401E78

l00401E60:
	lwz	r12,88(r1)
	lmw	r28,60(r1)
	mtlr	r12
	addi	r1,r1,+0050
	addi	r3,r0,-0001
	blr

l00401E78:
	stw	r28,8(r11)
	lwz	r8,4(r11)
	lwzx	r10,r10,r31
	add	r8,r8,r10

l00401E88:
	lwz	r4,4(r11)
	ori	r3,r8,0000
	add	r4,r4,r30
	stw	r4,56(r1)
	lwz	r5,56(r1)
	ori	r4,r30,0000
	stw	r5,4(r11)
	bl	fn00402CE0
	nop
	cmpwi	cr1,r3,+0000
	beq	cr1,$00401EBC

l00401EB4:
	bl	fn00401AB4
	nop

l00401EBC:
	lwz	r12,88(r1)
	lmw	r28,60(r1)
	mtlr	r12
	addi	r1,r1,+0050
	addi	r3,r0,+0000
	blr

;; fn00401ED4: 00401ED4
;;   Called from:
;;     00401DA0 (in fn00401CE8)
fn00401ED4 proc
	stwu	r1,-64(r1)
	stmw	r30,56(r1)
	lwz	r30,140(r2)
	rlwinm	r31,r3,04,00,1B
	lwz	r11,0(r30)
	mflr	r12
	lwz	r10,0(r11)
	stw	r12,72(r1)
	lwzx	r4,r10,r31
	add	r10,r10,r31
	cmplwi	cr1,r4,0000
	beq	cr1,$00401F14

l00401F04:
	lwz	r3,12(r10)
	bl	fn00404134
	lwz	r2,20(r1)
	lwz	r11,0(r30)

l00401F14:
	lwz	r12,72(r1)
	lwz	r11,0(r11)
	mtlr	r12
	addi	r4,r0,+0000
	stwx	r4,r11,r31
	add	r31,r11,r31
	stw	r4,4(r31)
	stw	r4,8(r31)
	lmw	r30,56(r1)
	addi	r1,r1,+0040
	blr

;; fn00401F40: 00401F40
;;   Called from:
;;     00401074 (in fn0040105C)
;;     004010A0 (in fn0040105C)
fn00401F40 proc
	lwz	r7,104(r2)
	stwu	r1,-16(r1)
	lwz	r5,4(r7)
	addi	r6,r7,+000C
	cmplw	cr1,r5,r6
	stw	r13,12(r1)
	addi	r4,r0,+0000
	lwz	r11,4(r7)
	beq	cr1,$00401FDC

l00401F64:
	addi	r6,r0,-0004

l00401F68:
	lwz	r5,4(r11)
	andi.	r5,r5,0003
	cmplwi	cr1,r5,0001
	bne	cr1,$00401FC0

l00401F78:
	lwz	r10,0(r11)
	lwz	r5,4(r11)
	lwz	r9,4(r10)
	and	r5,r5,r6
	and	r8,r9,r6
	subf	r8,r5,r8
	addi	r8,r8,-0008
	cmplw	cr1,r8,r3
	bge	cr1,$00401FD4

l00401F9C:
	andi.	r9,r9,0003
	cmplwi	cr1,r9,0001
	bne	cr1,$00401FC0

l00401FA8:
	lwz	r5,0(r10)
	stw	r5,0(r11)
	lwz	r8,8(r7)
	stw	r8,0(r10)
	stw	r10,8(r7)
	b	$00401F78

l00401FC0:
	lwz	r11,0(r11)
	addi	r5,r7,+000C
	cmplw	cr1,r11,r5
	bne	cr1,$00401F68

l00401FD0:
	b	$00401FE0

l00401FD4:
	ori	r4,r11,0000
	b	$004020AC

l00401FDC:
	addi	r6,r0,-0004

l00401FE0:
	lwz	r5,4(r7)
	lwz	r11,0(r7)
	stw	r5,8(r1)
	lwz	r8,8(r1)
	addi	r5,r7,+0004
	cmplw	cr1,r8,r11
	beq	cr1,$004020AC

l00401FFC:
	lwz	r8,4(r11)
	andi.	r8,r8,0003
	cmplwi	cr1,r8,0001
	bne	cr1,$00402064

l0040200C:
	lwz	r10,0(r11)
	lwz	r8,4(r11)
	lwz	r9,4(r10)
	and	r8,r8,r6
	and	r12,r9,r6
	subf	r12,r8,r12
	addi	r12,r12,-0008
	cmplw	cr1,r12,r3
	bge	cr1,$00402078

l00402030:
	andi.	r9,r9,0003
	cmplwi	cr1,r9,0001
	ori	r8,r10,0000
	bne	cr1,$00402064

l00402040:
	lwz	r9,0(r10)
	stw	r9,0(r11)
	lwz	r12,8(r7)
	stw	r12,0(r10)
	stw	r10,8(r7)
	lwz	r13,0(r5)
	cmplw	cr1,r13,r8
	bne	cr1,$0040200C

l00402060:
	b	$00402080

l00402064:
	lwz	r11,0(r11)
	lwz	r8,0(r5)
	cmplw	cr1,r8,r11
	bne	cr1,$00401FFC

l00402074:
	b	$004020AC

l00402078:
	ori	r4,r11,0000
	b	$004020AC

l00402080:
	stw	r11,4(r7)
	lwz	r5,0(r11)
	lwz	r7,4(r11)
	lwz	r5,4(r5)
	and	r7,r7,r6
	and	r6,r5,r6
	subf	r6,r7,r6
	addi	r6,r6,-0008
	cmplw	cr1,r6,r3
	blt	cr1,$004020AC

l004020A8:
	ori	r4,r11,0000

l004020AC:
	ori	r3,r4,0000
	lwz	r13,12(r1)
	addi	r1,r1,+0010
	blr

;; fn004020BC: 004020BC
;;   Called from:
;;     0040126C (in fn004011FC)
fn004020BC proc
	lwz	r4,116(r2)
	stwu	r1,-152(r1)
	lwz	r4,0(r4)
	stmw	r29,136(r1)
	ori	r29,r3,0000
	ori	r5,r29,0000
	cmplw	cr1,r5,r4
	mflr	r12
	stw	r12,160(r1)
	bge	cr1,$00402220

l004020E4:
	lwz	r11,120(r2)
	lbzx	r11,r11,r29
	extsb	r11,r11
	andi.	r4,r11,0001
	beq	$00402220

l004020F8:
	andi.	r11,r11,0040
	bne	$00402210

l00402100:
	addi	r31,r1,+0038
	addi	r11,r0,+0000
	addi	r9,r0,+0005
	mtctr	r9
	addi	r30,r0,+0000

l00402114:
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	bdnz	$00402114

l00402138:
	lwz	r4,156(r2)
	rlwinm	r5,r29,02,00,1D
	lwzx	r4,r4,r5
	ori	r3,r31,0000
	extsh	r4,r4
	sth	r4,24(r31)
	bl	fn00404074
	lwz	r2,20(r1)
	cmpwi	cr1,r3,+0000
	beq	cr1,$00402184

l00402160:
	lwz	r12,160(r1)
	lwz	r4,124(r2)
	mtlr	r12
	lmw	r29,136(r1)
	addi	r1,r1,+0098
	addi	r5,r0,+0005
	stw	r5,0(r4)
	addi	r3,r0,-0001
	blr

l00402184:
	addi	r11,r0,+0000
	addi	r9,r0,+0005
	mtctr	r9

l00402190:
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	bdnz	$00402190

l004021B4:
	lwz	r4,160(r2)
	rlwinm	r29,r29,01,00,1E
	lhax	r4,r4,r29
	ori	r3,r31,0000
	sth	r4,22(r31)
	bl	fn00403F3C
	lwz	r2,20(r1)
	cmpwi	cr1,r3,+0000
	beq	cr1,$004021F8

l004021D8:
	bl	fn00403104
	nop
	lwz	r12,160(r1)
	lmw	r29,136(r1)
	mtlr	r12
	addi	r1,r1,+0098
	addi	r3,r0,-0001
	blr

l004021F8:
	lwz	r12,160(r1)
	lmw	r29,136(r1)
	mtlr	r12
	addi	r1,r1,+0098
	addi	r3,r0,+0000
	blr

l00402210:
	lmw	r29,136(r1)
	addi	r1,r1,+0098
	addi	r3,r0,+0000
	blr

l00402220:
	lwz	r4,124(r2)
	lwz	r5,164(r2)
	lmw	r29,136(r1)
	addi	r1,r1,+0098
	addi	r6,r0,+0009
	stw	r6,0(r4)
	addi	r7,r0,+0000
	stw	r7,0(r5)
	addi	r3,r0,-0001
	blr

;; fn00402248: 00402248
;;   Called from:
;;     004012F8 (in fn004012B0)
;;     004016C4 (in fn00401598)
;;     00401724 (in fn00401598)
;;     00402B7C (in fn00402A24)
;;     00403698 (in fn00403618)
fn00402248 proc
	lwz	r6,116(r2)
	ori	r10,r3,0000
	lwz	r6,0(r6)
	ori	r7,r10,0000
	cmplw	cr1,r7,r6
	mflr	r12
	stwu	r1,-160(r1)
	stw	r12,168(r1)
	stmw	r29,144(r1)
	bge	cr1,$00402374

l00402270:
	lwz	r11,120(r2)
	lbzx	r11,r11,r10
	extsb	r11,r11
	andi.	r6,r11,0001
	beq	$00402374

l00402284:
	andi.	r6,r11,0010
	bne	$00402374

l0040228C:
	andi.	r6,r11,0040
	beq	$004022E4

l00402294:
	lwz	r6,156(r2)
	rlwinm	r10,r10,02,00,1D
	lwzx	r31,r6,r10
	ori	r29,r5,0000
	lwz	r30,4(r31)
	stw	r4,16(r31)
	stw	r29,12(r31)
	lwz	r30,16(r30)
	ori	r3,r31,0000
	ori	r12,r30,0000
	bl	fn00403F10
	lwz	r2,20(r1)
	ori	r30,r3,0000
	extsh.	r3,r30
	beq	$004022D8

l004022D0:
	lha	r3,2(r31)
	b	$00402334

l004022D8:
	lwz	r11,12(r31)
	subf	r11,r11,r29
	b	$00402334

l004022E4:
	andi.	r11,r11,0020
	lwz	r6,156(r2)
	rlwinm	r10,r10,02,00,1D
	lwzx	r6,r6,r10
	addi	r31,r1,+0040
	stw	r4,32(r31)
	stw	r5,36(r31)
	extsh	r6,r6
	sth	r6,24(r31)
	addi	r9,r0,+0000
	stw	r9,46(r31)
	beq	$00402320

l00402314:
	addi	r4,r0,+0002
	sth	r4,44(r31)
	b	$00402324

l00402320:
	sth	r9,44(r31)

l00402324:
	ori	r3,r31,0000
	bl	fn0040405C
	lwz	r2,20(r1)
	lwz	r11,40(r31)

l00402334:
	cmpwi	cr1,r3,+0000
	bne	cr1,$00402354

l0040233C:
	lwz	r12,168(r1)
	ori	r3,r11,0000
	mtlr	r12
	lmw	r29,144(r1)
	addi	r1,r1,+00A0
	blr

l00402354:
	bl	fn00403104
	nop
	lwz	r12,168(r1)
	lmw	r29,144(r1)
	mtlr	r12
	addi	r1,r1,+00A0
	addi	r3,r0,-0001
	blr

l00402374:
	lwz	r4,124(r2)
	lwz	r5,164(r2)
	addi	r1,r1,+00A0
	addi	r6,r0,+0009
	stw	r6,0(r4)
	addi	r7,r0,+0000
	stw	r7,0(r5)
	addi	r3,r0,-0001
	blr

;; fn00402398: 00402398
fn00402398 proc
	stwu	r1,-72(r1)
	stmw	r29,56(r1)
	lwz	r29,108(r2)
	lwz	r4,72(r2)
	lwz	r5,0(r29)
	addi	r31,r4,+0060
	cmplw	cr1,r5,r31
	mflr	r12
	addi	r30,r0,+0000
	stw	r12,80(r1)
	blt	cr1,$004023EC

l004023C4:
	ori	r3,r31,0000
	bl	fn00403170
	nop
	cmpwi	cr1,r3,-0001
	beq	cr1,$004023DC

l004023D8:
	addi	r30,r30,+0001

l004023DC:
	lwz	r4,0(r29)
	addi	r31,r31,+0020
	cmplw	cr1,r4,r31
	bge	cr1,$004023C4

l004023EC:
	lwz	r12,80(r1)
	ori	r3,r30,0000
	mtlr	r12
	lmw	r29,56(r1)
	addi	r1,r1,+0048
	blr

;; fn00402404: 00402404
;;   Called from:
;;     00403D40 (in fn00403CA0)
fn00402404 proc
	mflr	r12
	lwz	r3,168(r2)
	lwz	r4,172(r2)
	stwu	r1,-64(r1)
	stw	r12,72(r1)
	stw	r31,60(r1)
	bl	fn0040262C
	lwz	r3,176(r2)
	lwz	r4,180(r2)
	bl	fn0040262C
	addi	r3,r0,-5E53
	addi	r4,r0,+0000
	bl	fn00404044
	lwz	r2,20(r1)
	ori	r31,r3,0000
	addi	r3,r0,-5761
	addi	r4,r0,+0001
	bl	fn00404044
	lwz	r2,20(r1)
	cmplw	cr1,r31,r3
	beq	cr1,$00402480

l00402458:
	addi	r4,r1,+0038
	addis	r3,r0,+7379
	ori	r3,r3,7376
	bl	fn00403F9C
	lwz	r2,20(r1)
	cmpwi	cr1,r3,+0000
	bne	cr1,$00402480

l00402474:
	lwz	r4,184(r2)
	lwz	r5,56(r1)
	stw	r5,0(r4)

l00402480:
	lwz	r12,72(r1)
	lwz	r31,60(r1)
	mtlr	r12
	addi	r1,r1,+0040
	addi	r3,r0,+0000
	blr
00402498                         7D 88 02 A6 94 21 FF C8         }....!..
004024A0 91 81 00 40 38 60 00 00 38 80 00 00 38 A0 00 01 ...@8`..8...8...
004024B0 48 00 00 41 81 81 00 40 38 21 00 38 7D 88 03 A6 H..A...@8!.8}...
004024C0 4E 80 00 20 7D 88 02 A6 94 21 FF C8 91 81 00 40 N.. }....!.....@
004024D0 38 60 00 00 38 80 00 01 38 A0 00 01 48 00 00 15 8`..8...8...H...
004024E0 81 81 00 40 38 21 00 38 7D 88 03 A6 4E 80 00 20 ...@8!.8}...N.. 

;; fn004024F0: 004024F0
;;   Called from:
;;     00403588 (in fn00403574)
;;     004035B4 (in fn004035A0)
fn004024F0 proc
	cmpwi	cr1,r4,+0000
	mflr	r12
	lwz	r4,112(r2)
	stwu	r1,-64(r1)
	stmw	r30,56(r1)
	ori	r30,r3,0000
	ori	r31,r5,0000
	stw	r12,72(r1)
	extsb	r5,r31
	stb	r5,0(r4)
	bne	cr1,$00402544

l0040251C:
	lwz	r3,188(r2)
	lwz	r3,0(r3)
	cmplwi	cr1,r3,0000
	beq	cr1,$00402538

l0040252C:
	lwz	r4,192(r2)
	lwz	r4,0(r4)
	bl	fn0040262C

l00402538:
	lwz	r3,196(r2)
	lwz	r4,200(r2)
	bl	fn0040262C

l00402544:
	lwz	r3,204(r2)
	lwz	r4,208(r2)
	bl	fn0040262C
	cmpwi	cr1,r31,+0000
	bne	cr1,$00402574

l00402558:
	lwz	r11,212(r2)
	lwz	r11,0(r11)
	cmplwi	cr1,r11,0000
	beq	cr1,$0040256C

l00402568:
	stw	r30,14(r11)

l0040256C:
	bl	fn00403248
	nop

l00402574:
	lwz	r12,72(r1)
	lmw	r30,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	blr
00402588                         94 21 FF B0 BF 41 00 38         .!...A.8
00402590 60 7B 00 00 61 23 00 00 28 83 00 00 7D 88 02 A6 `{..a#..(...}...
004025A0 60 9A 00 00 60 BE 00 00 60 DF 00 00 60 FC 00 00 `...`...`...`...
004025B0 61 1D 00 00 61 44 00 00 91 81 00 58 41 86 00 08 a...aD.....XA...
004025C0 48 00 00 6D 63 C3 00 00 63 E4 00 00 48 00 00 61 H..mc...c...H..a
004025D0 80 62 00 C4 80 82 00 C8 48 00 00 55 63 83 00 00 .b......H..Uc...
004025E0 63 A4 00 00 48 00 00 49 80 62 00 CC 80 82 00 D0 c...H..I.b......
004025F0 48 00 00 3D 81 62 00 D4 81 6B 00 00 28 8B 00 00 H..=.b...k..(...
00402600 41 86 00 08 93 6B 00 0E 2C 9A 00 00 40 86 00 0C A....k..,...@...
00402610 48 00 0C 39 60 00 00 00 81 81 00 58 BB 41 00 38 H..9`......X.A.8
00402620 7D 88 03 A6 38 21 00 50 4E 80 00 20             }...8!.PN..     

;; fn0040262C: 0040262C
;;   Called from:
;;     0040241C (in fn00402404)
;;     00402428 (in fn00402404)
;;     00402534 (in fn004024F0)
;;     00402540 (in fn004024F0)
;;     0040254C (in fn004024F0)
fn0040262C proc
	stwu	r1,-72(r1)
	stmw	r29,56(r1)
	ori	r30,r3,0000
	ori	r29,r4,0000
	cmplw	cr1,r29,r30
	mflr	r12
	stw	r12,80(r1)
	ble	cr1,$0040267C

l0040264C:
	lwz	r31,0(r30)
	cmplwi	cr1,r31,0000
	beq	cr1,$00402670

l00402658:
	addi	r4,r0,-0001
	cmplw	cr1,r31,r4
	beq	cr1,$00402670

l00402664:
	ori	r12,r31,0000
	bl	fn00403F10
	lwz	r2,20(r1)

l00402670:
	addi	r30,r30,+0004
	cmplw	cr1,r29,r30
	bgt	cr1,$0040264C

l0040267C:
	lwz	r12,80(r1)
	lmw	r29,56(r1)
	mtlr	r12
	addi	r1,r1,+0048
	blr
00402690 7D 88 02 A6 94 21 FF C8 91 81 00 40 38 60 00 02 }....!.....@8`..
004026A0 48 00 05 AD 60 00 00 00 81 81 00 40 38 21 00 38 H...`......@8!.8
004026B0 7D 88 03 A6 4E 80 00 20                         }...N..         

;; fn004026B8: 004026B8
;;   Called from:
;;     004016F4 (in fn00401598)
fn004026B8 proc
	stwu	r1,-160(r1)
	stmw	r26,136(r1)
	ori	r29,r4,0000
	lwz	r4,116(r2)
	ori	r28,r3,0000
	lwz	r4,0(r4)
	ori	r6,r28,0000
	cmplw	cr1,r4,r6
	mflr	r12
	stw	r12,168(r1)
	ble	cr1,$00402964

l004026E4:
	lwz	r11,120(r2)
	lbzx	r11,r11,r28
	extsb	r11,r11
	andi.	r4,r11,0001
	beq	$00402964

l004026F8:
	andi.	r11,r11,0040
	bne	$00402964

l00402700:
	ori	r11,r5,0000
	cmpwi	cr1,r11,+0000
	beq	cr1,$00402738

l0040270C:
	cmpwi	cr1,r11,+0001
	beq	cr1,$00402744

l00402714:
	cmpwi	cr1,r11,+0002
	beq	cr1,$004027A8

l0040271C:
	lwz	r4,124(r2)
	lmw	r28,144(r1)
	addi	r1,r1,+00A0
	addi	r5,r0,+0016
	stw	r5,0(r4)
	addi	r3,r0,-0001
	blr

l00402738:
	addi	r3,r0,+0000
	addi	r30,r0,+0000
	b	$00402808

l00402744:
	addi	r31,r1,+0038
	addi	r11,r0,+0000
	addi	r9,r0,+0005
	mtctr	r9
	addi	r30,r0,+0000

l00402758:
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	bdnz	$00402758

l0040277C:
	lwz	r4,156(r2)
	rlwinm	r5,r28,02,00,1D
	lwzx	r4,r4,r5
	ori	r3,r31,0000
	extsh	r4,r4
	sth	r4,24(r31)
	bl	fn00403FE4
	lwz	r2,20(r1)
	lwz	r31,46(r31)
	add	r29,r29,r31
	b	$00402808

l004027A8:
	addi	r31,r1,+0038
	addi	r11,r0,+0000
	addi	r9,r0,+0005
	mtctr	r9
	addi	r30,r0,+0000

l004027BC:
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	bdnz	$004027BC

l004027E0:
	lwz	r4,156(r2)
	rlwinm	r5,r28,02,00,1D
	lwzx	r4,r4,r5
	ori	r3,r31,0000
	extsh	r4,r4
	sth	r4,24(r31)
	bl	fn00403FFC
	lwz	r2,20(r1)
	lwz	r31,28(r31)
	add	r29,r29,r31

l00402808:
	cmpwi	cr1,r3,+0000
	bne	cr1,$00402920

l00402810:
	addi	r31,r1,+0038
	addi	r11,r0,+0000
	addi	r9,r0,+0005
	mtctr	r9

l00402820:
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	bdnz	$00402820

l00402844:
	lwz	r27,156(r2)
	rlwinm	r28,r28,02,00,1D
	lwzx	r4,r27,r28
	ori	r3,r31,0000
	stw	r29,46(r31)
	extsh	r4,r4
	sth	r4,24(r31)
	add	r27,r28,r27
	addi	r26,r0,+0001
	sth	r26,44(r31)
	bl	fn00404014
	lwz	r2,20(r1)
	cmpwi	cr1,r3,-0027
	bne	cr1,$00402920

l0040287C:
	addi	r11,r0,+0000
	addi	r9,r0,+0005
	mtctr	r9

l00402888:
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	bdnz	$00402888

l004028AC:
	lwz	r4,0(r27)
	ori	r3,r31,0000
	stw	r29,28(r31)
	extsh	r4,r4
	sth	r4,24(r31)
	bl	fn0040402C
	lwz	r2,20(r1)
	cmpwi	cr1,r3,+0000
	bne	cr1,$00402920

l004028D0:
	addi	r11,r0,+0000
	addi	r9,r0,+0005
	mtctr	r9

l004028DC:
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	bdnz	$004028DC

l00402900:
	lwz	r27,0(r27)
	ori	r3,r31,0000
	sth	r26,44(r31)
	stw	r29,46(r31)
	extsh	r27,r27
	sth	r27,24(r31)
	bl	fn00404014
	lwz	r2,20(r1)

l00402920:
	cmpwi	cr1,r3,+0000
	beq	cr1,$00402948

l00402928:
	bl	fn00403104
	nop
	lwz	r12,168(r1)
	lmw	r26,136(r1)
	mtlr	r12
	addi	r1,r1,+00A0
	addi	r3,r0,-0001
	blr

l00402948:
	lwz	r12,168(r1)
	addi	r4,r1,+0038
	mtlr	r12
	lwz	r3,46(r4)
	lmw	r26,136(r1)
	addi	r1,r1,+00A0
	blr

l00402964:
	lwz	r4,124(r2)
	lwz	r5,164(r2)
	lmw	r28,144(r1)
	addi	r1,r1,+00A0
	addi	r6,r0,+0009
	stw	r6,0(r4)
	addi	r7,r0,+0000
	stw	r7,0(r5)
	addi	r3,r0,-0001
	blr

;; fn0040298C: 0040298C
;;   Called from:
;;     00401680 (in fn00401598)
fn0040298C proc
	lwz	r4,76(r2)
	mflr	r12
	lwz	r5,0(r4)
	stwu	r1,-64(r1)
	stw	r12,72(r1)
	stw	r31,56(r1)
	ori	r31,r3,0000
	addi	r5,r5,+0001
	stw	r5,0(r4)
	addi	r3,r0,+1000
	bl	fn00400F9C
	nop
	ori	r4,r3,0000
	cmplwi	cr1,r4,0000
	stw	r3,8(r31)
	beq	cr1,$004029E4

l004029CC:
	lwz	r4,12(r31)
	addi	r5,r0,+1000
	stw	r5,24(r31)
	ori	r4,r4,0008
	stw	r4,12(r31)
	b	$00402A00

l004029E4:
	lwz	r4,12(r31)
	addi	r6,r31,+0014
	stw	r6,8(r31)
	ori	r4,r4,0004
	stw	r4,12(r31)
	addi	r5,r0,+0001
	stw	r5,24(r31)

l00402A00:
	lwz	r12,72(r1)
	lwz	r4,8(r31)
	mtlr	r12
	addi	r5,r0,+0000
	stw	r5,4(r31)
	stw	r4,0(r31)
	lwz	r31,56(r1)
	addi	r1,r1,+0040
	blr

;; fn00402A24: 00402A24
fn00402A24 proc
	stwu	r1,-600(r1)
	stmw	r27,576(r1)
	ori	r28,r4,0000
	lwz	r4,212(r2)
	mflr	r12
	lwz	r4,0(r4)
	ori	r27,r3,0000
	cmplwi	cr1,r4,0000
	ori	r29,r5,0000
	stw	r12,608(r1)
	beq	cr1,$00402AB0

l00402A50:
	lwz	r31,72(r2)
	lwz	r4,76(r31)
	andi.	r4,r4,010C
	bne	$00402A7C

l00402A60:
	addi	r4,r31,+0040
	ori	r3,r4,0000
	addi	r4,r0,+0000
	addi	r5,r0,+0004
	addi	r6,r0,+0000
	bl	fn00403450
	nop

l00402A7C:
	lwz	r4,216(r2)
	ori	r5,r27,0000
	ori	r6,r28,0000
	ori	r7,r29,0000
	addi	r30,r31,+0040
	ori	r3,r30,0000
	addi	r4,r4,+0000
	bl	fn004033C4
	nop
	ori	r3,r30,0000
	bl	fn004011FC
	nop
	b	$00402B84

l00402AB0:
	addi	r31,r1,+0038
	ori	r4,r31,0000
	addis	r3,r0,+6164
	ori	r3,r3,6472
	bl	fn00403F9C
	lwz	r2,20(r1)
	cmpwi	cr1,r3,+0000
	bne	cr1,$00402B04

l00402AD0:
	ori	r3,r31,0000
	addi	r4,r0,+001D
	bl	fn00403FB4
	lwz	r2,20(r1)
	cmplwi	cr1,r3,0000
	bne	cr1,$00402AF4

l00402AE8:
	lwz	r11,288(r0)
	andi.	r11,r11,00FF
	b	$00402B04

l00402AF4:
	lbz	r11,3071(r0)
	extsb	r11,r11
	andi.	r11,r11,00FF
	b	$00402B04

l00402B04:
	andi.	r11,r11,0020
	beq	$00402B4C

l00402B0C:
	lwz	r4,216(r2)
	ori	r5,r27,0000
	ori	r6,r28,0000
	ori	r7,r29,0000
	addi	r31,r1,+0040
	ori	r3,r31,0000
	addi	r4,r4,+0000
	bl	fn004032FC
	nop
	ori	r3,r31,0000
	bl	fn004032AC
	nop
	ori	r3,r31,0000
	bl	fn00403FCC
	lwz	r2,20(r1)
	b	$00402B84

l00402B4C:
	lwz	r4,216(r2)
	ori	r5,r27,0000
	ori	r6,r28,0000
	ori	r7,r29,0000
	addi	r31,r1,+0040
	ori	r3,r31,0000
	addi	r4,r4,+0000
	bl	fn004032FC
	nop
	ori	r5,r3,0000
	ori	r4,r31,0000
	addi	r3,r0,+0002
	bl	fn00402248
	nop

l00402B84:
	bl	fn0040326C
	nop
	lwz	r12,608(r1)
	lmw	r27,576(r1)
	mtlr	r12
	addi	r1,r1,+0258
	blr

;; fn00402BA0: 00402BA0
;;   Called from:
;;     00403214 (in fn00403170)
;;     00403A38 (in fn00403A0C)
fn00402BA0 proc
	cmplwi	cr1,r3,0000
	mflr	r12
	stwu	r1,-64(r1)
	stw	r12,72(r1)
	stmw	r30,56(r1)
	beq	cr1,$00402C38

l00402BB8:
	lwzu	r31,-8(r3)
	addi	r30,r0,-0004
	lwz	r4,4(r31)
	and	r4,r4,r30
	cmplw	cr1,r4,r3
	beq	cr1,$00402BD8

l00402BD0:
	bl	fn00401AB4
	nop

l00402BD8:
	lwz	r4,4(r31)
	lwz	r10,220(r2)
	addi	r5,r0,-0003
	and	r4,r4,r5
	ori	r11,r4,0001
	stw	r11,4(r31)
	lwz	r10,0(r10)
	addi	r6,r0,-0001
	cmplw	cr1,r10,r6
	beq	cr1,$00402C38

l00402C00:
	lwz	r9,104(r2)
	lwz	r4,4(r9)
	lwz	r4,4(r4)
	cmplw	cr1,r4,r11
	ble	cr1,$00402C38

l00402C14:
	lwz	r4,0(r31)
	and	r11,r11,r30
	lwz	r4,4(r4)
	and	r30,r4,r30
	subf	r30,r11,r30
	addi	r30,r30,-0008
	cmplw	cr1,r30,r10
	blt	cr1,$00402C38

l00402C34:
	stw	r31,4(r9)

l00402C38:
	lwz	r12,72(r1)
	lmw	r30,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	blr

;; fn00402C4C: 00402C4C
;;   Called from:
;;     00401AC4 (in fn00401AB4)
fn00402C4C proc
	mflr	r12
	stwu	r1,-64(r1)
	stw	r12,72(r1)
	stw	r31,56(r1)
	ori	r31,r3,0000
	bl	fn004035CC
	nop
	ori	r3,r31,0000
	bl	fn00403618
	nop
	lwz	r11,224(r2)
	addi	r3,r0,+00FF
	lwz	r12,0(r11)
	bl	fn00403F10
	lwz	r2,20(r1)
	lwz	r12,72(r1)
	lwz	r31,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	blr
00402C9C                                     28 84 00 00             (...
00402CA0 7D 88 02 A6 94 21 FF C8 91 81 00 40 41 86 00 28 }....!.....@A..(
00402CB0 70 65 00 07 40 82 00 20 70 85 00 07 40 82 00 18 pe..@.. p...@...
00402CC0 48 00 00 21 81 81 00 40 38 21 00 38 7D 88 03 A6 H..!...@8!.8}...
00402CD0 4E 80 00 20 38 21 00 38 38 60 FF FF 4E 80 00 20 N.. 8!.88`..N.. 

;; fn00402CE0: 00402CE0
;;   Called from:
;;     00401EA4 (in fn00401DD8)
fn00402CE0 proc
	mflr	r12
	stwu	r1,-112(r1)
	stmw	r26,84(r1)
	ori	r26,r3,0000
	ori	r27,r4,0000
	stw	r12,120(r1)
	addi	r31,r1,+0038
	ori	r5,r31,0000
	stw	r5,72(r1)
	addi	r4,r0,+0000
	stw	r4,0(r31)
	stw	r4,4(r31)
	stw	r4,8(r31)
	stw	r4,12(r31)
	bl	fn004019CC
	nop
	ori	r4,r3,0000
	cmplwi	cr1,r4,0000
	stw	r3,0(r31)
	beq	cr1,$00403014

l00402D30:
	bl	fn004019CC
	nop
	ori	r4,r3,0000
	cmplwi	cr1,r4,0000
	stw	r3,4(r31)
	beq	cr1,$00403014

l00402D48:
	bl	fn004019CC
	nop
	ori	r4,r3,0000
	cmplwi	cr1,r4,0000
	stw	r3,8(r31)
	beq	cr1,$00403014

l00402D60:
	ori	r3,r26,0000
	addi	r29,r1,+004C
	ori	r4,r29,0000
	bl	fn004036B0
	nop
	cmpwi	cr1,r3,+0000
	bne	cr1,$00402D9C

l00402D7C:
	lwz	r4,76(r1)
	lwz	r4,4(r4)
	andi.	r4,r4,0003
	cmplwi	cr1,r4,0002
	bne	cr1,$00403014

l00402D90:
	lwz	r4,76(r1)
	ori	r30,r4,0000
	b	$00402DB0

l00402D9C:
	lwz	r4,72(r1)
	lwz	r5,0(r4)
	addi	r4,r4,+0004
	stw	r4,72(r1)
	ori	r30,r5,0000

l00402DB0:
	addi	r11,r3,+0003
	cmplwi	cr1,r11,0004
	stw	r26,4(r30)
	addi	r4,r0,-0003
	and	r4,r26,r4
	ori	r4,r4,0001
	stw	r4,4(r30)
	stw	r30,0(r26)
	bge	cr1,$00402DFC

l00402DD4:
	rlwinm	r11,r11,02,00,1D
	bl	fn00402DEC
	b	$00402E94
00402DE0 48 00 00 EC 48 00 01 5C 48 00 01 84             H...H..\H...    

;; fn00402DEC: 00402DEC
;;   Called from:
;;     00402DD8 (in fn00402CE0)
fn00402DEC proc
	mflr	r0
	add	r0,r0,r11
	mtlr	r0
	blr

l00402DFC:
	lwz	r4,76(r1)
	lwz	r4,4(r4)
	andi.	r4,r4,0003
	cmplwi	cr1,r4,0002
	bne	cr1,$00403014

l00402E10:
	lwz	r4,76(r1)
	lwz	r31,104(r2)
	lwz	r5,0(r4)
	addi	r6,r31,+000C
	cmplw	cr1,r5,r6
	bne	cr1,$00402E4C

l00402E28:
	lwz	r4,4(r30)
	lwz	r6,16(r31)
	addi	r29,r0,-0004
	and	r4,r4,r29
	add	r11,r4,r27
	cmplw	cr1,r6,r11
	bge	cr1,$00402E50

l00402E44:
	stw	r11,16(r31)
	b	$00402E50

l00402E4C:
	addi	r29,r0,-0004

l00402E50:
	ori	r3,r30,0000
	ori	r4,r27,0000
	addi	r28,r1,+0048
	ori	r6,r28,0000
	bl	fn00403070
	lwz	r4,76(r1)
	lwz	r5,0(r4)
	lwz	r6,4(r4)
	lwz	r5,4(r5)
	ori	r3,r4,0000
	and	r5,r5,r29
	and	r6,r6,r29
	subf	r4,r6,r5
	ori	r5,r30,0000
	ori	r6,r28,0000
	bl	fn00403070
	b	$00402FB8

l00402E94:
	addi	r4,r1,+0048
	stw	r4,80(r1)
	lwz	r31,104(r2)
	lwz	r6,80(r1)
	ori	r3,r30,0000
	ori	r4,r27,0000
	add	r5,r27,r26
	stw	r5,16(r31)
	addi	r5,r31,+000C
	bl	fn00403070
	stw	r30,4(r31)
	stw	r30,0(r31)
	addi	r29,r0,-0004
	b	$00402FB8
00402ECC                                     83 E2 00 68             ...h
00402ED0 80 9F 00 10 38 64 FF FF 63 A4 00 00 48 00 07 D5 ....8d..c...H...
00402EE0 60 00 00 00 2C 83 00 01 41 86 00 0C 4B FF EB C9 `...,...A...K...
00402EF0 60 00 00 00 80 81 00 4C 38 E1 00 48 80 A4 00 00 `......L8..H....
00402F00 80 C4 00 04 80 A5 00 04 60 83 00 00 7F 7B D2 14 ........`....{..
00402F10 93 7F 00 10 39 1F 00 0C 91 1E 00 00 3B A0 FF FC ....9.......;...
00402F20 7C C6 E8 38 7C A5 E8 38 7C A6 28 50 60 E6 00 00 |..8|..8|.(P`...
00402F30 60 A4 00 00 63 C5 00 00 48 00 01 39 48 00 00 7C `...c...H..9H..|
00402F40 83 E2 00 68 38 A1 00 48 90 A1 00 50 80 BF 00 00 ...h8..H...P....
00402F50 80 C1 00 50 63 C3 00 00 63 64 00 00 48 00 01 15 ...Pc...cd..H...
00402F60 93 DF 00 00 3B A0 FF FC 48 00 00 50 80 81 00 4C ....;...H..P...L
00402F70 83 E2 00 68 80 84 00 00 38 DF 00 0C 60 85 00 00 ...h....8...`...
00402F80 7C 85 30 40 40 86 00 20 80 81 00 4C 3B A0 FF FC |.0@@.. ...L;...
00402F90 80 84 00 04 7C 84 E8 38 7F 64 DA 14 93 7F 00 10 ....|..8.d......
00402FA0 48 00 00 18 63 C3 00 00 63 64 00 00 38 C1 00 48 H...c...cd..8..H
00402FB0 48 00 00 C1 3B A0 FF FC                         H...;...        

l00402FB8:
	lwz	r4,4(r31)
	lwz	r4,4(r4)
	and	r4,r4,r29
	cmplw	cr1,r4,r26
	ble	cr1,$00402FFC

l00402FCC:
	lwz	r4,0(r30)
	lwz	r5,220(r2)
	lwz	r4,4(r4)
	lwz	r6,4(r30)
	lwz	r5,0(r5)
	and	r4,r4,r29
	and	r29,r6,r29
	subf	r4,r29,r4
	addi	r4,r4,-0008
	cmplw	cr1,r4,r5
	blt	cr1,$00402FFC

l00402FF8:
	stw	r30,4(r31)

l00402FFC:
	lwz	r12,120(r1)
	lmw	r26,84(r1)
	mtlr	r12
	addi	r1,r1,+0070
	addi	r3,r0,+0000
	blr

l00403014:
	lwz	r4,72(r1)
	lwz	r11,0(r4)
	cmplwi	cr1,r11,0000
	beq	cr1,$00403058

l00403024:
	lwz	r4,104(r2)
	addi	r10,r4,+0008

l0040302C:
	lwz	r4,0(r10)
	stw	r4,0(r11)
	lwz	r5,72(r1)
	lwz	r11,0(r5)
	addi	r5,r5,+0004
	stw	r5,72(r1)
	lwz	r6,72(r1)
	stw	r11,0(r10)
	lwz	r11,0(r6)
	cmplwi	cr1,r11,0000
	bne	cr1,$0040302C

l00403058:
	lwz	r12,120(r1)
	lmw	r26,84(r1)
	mtlr	r12
	addi	r1,r1,+0070
	addi	r3,r0,-0001
	blr

;; fn00403070: 00403070
;;   Called from:
;;     00402E60 (in fn00402CE0)
;;     00402E8C (in fn00402CE0)
;;     00402EB8 (in fn00402CE0)
fn00403070 proc
	lwz	r9,4(r3)
	andi.	r7,r9,0003
	cmplwi	cr1,r7,0002
	beq	cr1,$004030FC

l00403080:
	lwz	r10,4(r5)
	andi.	r7,r10,0003
	cmplwi	cr1,r7,0002
	bne	cr1,$004030B4

l00403090:
	addi	r6,r0,-0004
	and	r9,r9,r6
	add	r4,r9,r4
	stw	r4,4(r5)
	addi	r7,r0,-0002
	and	r4,r4,r7
	ori	r4,r4,0002
	stw	r4,4(r5)
	b	$004030FC

l004030B4:
	addi	r7,r0,-0004
	and	r9,r9,r7
	add	r4,r9,r4
	and	r10,r10,r7
	ori	r11,r4,0000
	cmplw	cr1,r10,r11
	beq	cr1,$004030FC

l004030D0:
	lwz	r4,0(r6)
	addi	r7,r0,-0002
	lwz	r8,0(r4)
	and	r7,r11,r7
	addi	r4,r4,+0004
	stw	r4,0(r6)
	stw	r11,4(r8)
	ori	r7,r7,0002
	stw	r7,4(r8)
	stw	r8,0(r3)
	ori	r3,r8,0000

l004030FC:
	stw	r5,0(r3)
	blr

;; fn00403104: 00403104
;;   Called from:
;;     004021D8 (in fn004020BC)
;;     00402354 (in fn00402248)
;;     00402928 (in fn004026B8)
;;     00403848 (in fn00403744)
;;     0040392C (in fn00403898)
;;     004039AC (in fn00403898)
fn00403104 proc
	lwz	r4,164(r2)
	lwz	r8,228(r2)
	extsh	r3,r3
	addi	r8,r8,+0000
	addi	r9,r8,+00B8
	ori	r5,r3,0000
	stw	r5,0(r4)
	addi	r10,r0,+0000
	ori	r11,r8,0000

l00403128:
	lha	r4,0(r11)
	cmp	cr1,r4,r3
	beq	cr1,$00403148

l00403134:
	addi	r11,r11,+0004
	cmplw	cr1,r11,r9
	addi	r10,r10,+0001
	blt	cr1,$00403128

l00403144:
	b	$00403160

l00403148:
	rlwinm	r10,r10,02,00,1D
	addi	r8,r8,+0002
	lhzx	r8,r8,r10
	lwz	r4,124(r2)
	stw	r8,0(r4)
	b	$0040316C

l00403160:
	lwz	r4,124(r2)
	addi	r5,r0,+0016
	stw	r5,0(r4)

l0040316C:
	blr

;; fn00403170: 00403170
;;   Called from:
;;     004023C8 (in fn00402398)
fn00403170 proc
	stwu	r1,-64(r1)
	stmw	r30,56(r1)
	ori	r31,r3,0000
	lwz	r11,12(r31)
	mflr	r12
	andi.	r4,r11,0040
	stw	r12,72(r1)
	addi	r30,r0,-0001
	beq	$004031AC

l00403194:
	addi	r4,r0,+0000
	stw	r4,12(r31)
	lmw	r30,56(r1)
	addi	r1,r1,+0040
	addi	r3,r0,-0001
	blr

l004031AC:
	andi.	r11,r11,0083
	beq	$00403228

l004031B4:
	ori	r3,r31,0000
	bl	fn004012B0
	nop
	ori	r30,r3,0000
	ori	r3,r31,0000
	bl	fn00403A0C
	nop
	lwz	r3,16(r31)
	bl	fn00403898
	nop
	cmpwi	cr1,r3,+0000
	bge	cr1,$004031F0

l004031E4:
	addi	r30,r0,-0001
	addi	r11,r0,+0000
	b	$0040322C

l004031F0:
	lwz	r3,28(r31)
	cmplwi	cr1,r3,0000
	beq	cr1,$00403228

l004031FC:
	bl	fn00403744
	nop
	cmpwi	cr1,r3,+0000
	beq	cr1,$00403210

l0040320C:
	addi	r30,r0,-0001

l00403210:
	lwz	r3,28(r31)
	bl	fn00402BA0
	nop
	addi	r11,r0,+0000
	stw	r11,28(r31)
	b	$0040322C

l00403228:
	addi	r11,r0,+0000

l0040322C:
	lwz	r12,72(r1)
	ori	r3,r30,0000
	mtlr	r12
	stw	r11,12(r31)
	lmw	r30,56(r1)
	addi	r1,r1,+0040
	blr

;; fn00403248: 00403248
;;   Called from:
;;     0040256C (in fn004024F0)
fn00403248 proc
	mflr	r12
	stwu	r1,-56(r1)
	stw	r12,64(r1)
	bl	fn00403F84
	lwz	r2,20(r1)
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	blr

;; fn0040326C: 0040326C
;;   Called from:
;;     00402B84 (in fn00402A24)
fn0040326C proc
	mflr	r12
	stwu	r1,-56(r1)
	stw	r12,64(r1)
	addi	r3,r0,+000A
	bl	fn00403618
	nop
	addi	r3,r0,+0016
	bl	fn00403B08
	nop
	addi	r3,r0,+0003
	bl	fn004035A0
	nop
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	blr

;; fn004032AC: 004032AC
;;   Called from:
;;     00402B34 (in fn00402A24)
;;     004037A8 (in fn00403744)
fn004032AC proc
	cmplwi	cr1,r3,0000
	beq	cr1,$004032F8

l004032B4:
	lbz	r4,0(r3)
	extsb.	r4,r4
	beq	$004032F8

l004032C0:
	ori	r11,r3,0000
	lbz	r10,0(r3)

l004032C8:
	lbzu	r4,1(r11)
	stb	r10,0(r11)
	cmplwi	cr1,r4,0000
	ori	r10,r4,0000
	bne	cr1,$004032C8

l004032DC:
	subf	r10,r3,r11
	cmpwi	cr1,r10,+00FF
	addi	r9,r0,+00FF
	bge	cr1,$004032F0

l004032EC:
	ori	r9,r10,0000

l004032F0:
	extsb	r9,r9
	stb	r9,0(r3)

l004032F8:
	blr

;; fn004032FC: 004032FC
;;   Called from:
;;     00402B28 (in fn00402A24)
;;     00402B68 (in fn00402A24)
fn004032FC proc
	mflr	r12
	stw	r3,24(r1)
	stw	r5,32(r1)
	stw	r6,36(r1)
	stw	r7,40(r1)
	stw	r4,28(r1)
	stw	r8,44(r1)
	stw	r9,48(r1)
	stw	r10,52(r1)
	stwu	r1,-96(r1)
	stw	r12,104(r1)
	stmw	r30,88(r1)
	stw	r4,124(r1)
	addi	r31,r1,+0038
	stw	r3,8(r31)
	stw	r3,0(r31)
	ori	r3,r31,0000
	addi	r5,r1,+007C
	addi	r5,r5,+0004
	addi	r6,r0,+0042
	stw	r6,12(r31)
	addis	r7,r0,+7FFF
	ori	r7,r7,FFFF
	stw	r7,4(r31)
	bl	fn004004B4
	nop
	lwz	r4,4(r31)
	ori	r30,r3,0000
	addi	r4,r4,-0001
	ori	r5,r4,0000
	cmpwi	cr1,r5,-0001
	stw	r4,4(r31)
	ble	cr1,$0040339C

l00403380:
	lwz	r4,0(r31)
	addi	r5,r0,+0000
	stb	r5,0(r4)
	lwz	r6,0(r31)
	addi	r6,r6,+0001
	stw	r6,0(r31)
	b	$004033AC

l0040339C:
	ori	r4,r31,0000
	addi	r3,r0,+0000
	bl	fn00401598
	nop

l004033AC:
	lwz	r12,104(r1)
	ori	r3,r30,0000
	mtlr	r12
	lmw	r30,88(r1)
	addi	r1,r1,+0060
	blr

;; fn004033C4: 004033C4
;;   Called from:
;;     00402A98 (in fn00402A24)
fn004033C4 proc
	mflr	r12
	stw	r3,24(r1)
	stw	r4,28(r1)
	stw	r5,32(r1)
	stw	r6,36(r1)
	stw	r7,40(r1)
	stw	r8,44(r1)
	stw	r9,48(r1)
	stw	r10,52(r1)
	stwu	r1,-72(r1)
	stw	r12,80(r1)
	stmw	r29,56(r1)
	ori	r31,r3,0000
	ori	r3,r31,0000
	stw	r4,100(r1)
	bl	fn004002F8
	nop
	lwz	r4,100(r1)
	ori	r30,r3,0000
	ori	r3,r31,0000
	addi	r5,r1,+0064
	addi	r5,r5,+0004
	bl	fn004004B4
	nop
	ori	r29,r3,0000
	ori	r3,r30,0000
	ori	r4,r31,0000
	bl	fn00400430
	nop
	lwz	r12,80(r1)
	ori	r3,r29,0000
	mtlr	r12
	lmw	r29,56(r1)
	addi	r1,r1,+0048
	blr

;; fn00403450: 00403450
;;   Called from:
;;     00402A74 (in fn00402A24)
fn00403450 proc
	stwu	r1,-80(r1)
	stmw	r27,56(r1)
	ori	r30,r5,0000
	cmpwi	cr1,r30,+0004
	mflr	r12
	ori	r31,r3,0000
	ori	r29,r4,0000
	ori	r28,r6,0000
	stw	r12,88(r1)
	addi	r27,r0,+0000
	beq	cr1,$004034B4

l0040347C:
	cmplwi	cr1,r28,0000
	beq	cr1,$004034A4

l00403484:
	addis	r4,r0,+7FFF
	ori	r4,r4,FFFF
	cmplw	cr1,r28,r4
	bgt	cr1,$004034A4

l00403494:
	cmpwi	cr1,r30,+0000
	beq	cr1,$004034B4

l0040349C:
	cmpwi	cr1,r30,+0040
	beq	cr1,$004034B4

l004034A4:
	lmw	r27,56(r1)
	addi	r1,r1,+0050
	addi	r3,r0,-0001
	blr

l004034B4:
	ori	r3,r31,0000
	bl	fn004012B0
	nop
	ori	r3,r31,0000
	bl	fn00403A0C
	nop
	andi.	r30,r30,0004
	lwz	r11,12(r31)
	addi	r4,r0,-390D
	and	r11,r11,r4
	stw	r11,12(r31)
	beq	$004034F8

l004034E4:
	ori	r11,r11,0004
	stw	r11,12(r31)
	addi	r29,r31,+0014
	addi	r28,r0,+0001
	b	$00403548

l004034F8:
	cmplwi	cr1,r29,0000
	bne	cr1,$00403540

l00403500:
	ori	r3,r28,0000
	bl	fn00400F9C
	nop
	ori	r29,r3,0000
	cmplwi	cr1,r29,0000
	bne	cr1,$00403530

l00403518:
	lwz	r4,76(r2)
	addi	r27,r0,-0001
	lwz	r5,0(r4)
	addi	r5,r5,+0001
	stw	r5,0(r4)
	b	$0040355C

l00403530:
	lwz	r4,12(r31)
	ori	r4,r4,0008
	stw	r4,12(r31)
	b	$00403548

l00403540:
	ori	r11,r11,0100
	stw	r11,12(r31)

l00403548:
	stw	r29,8(r31)
	stw	r28,24(r31)
	stw	r29,0(r31)
	addi	r4,r0,+0000
	stw	r4,4(r31)

l0040355C:
	lwz	r12,88(r1)
	ori	r3,r27,0000
	mtlr	r12
	lmw	r27,56(r1)
	addi	r1,r1,+0050
	blr

;; fn00403574: 00403574
;;   Called from:
;;     00403D5C (in fn00403CA0)
fn00403574 proc
	mflr	r12
	stwu	r1,-56(r1)
	stw	r12,64(r1)
	addi	r4,r0,+0000
	addi	r5,r0,+0000
	bl	fn004024F0
	nop
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	blr

;; fn004035A0: 004035A0
;;   Called from:
;;     00403294 (in fn0040326C)
;;     00403BE8 (in fn00403B08)
fn004035A0 proc
	mflr	r12
	stwu	r1,-56(r1)
	stw	r12,64(r1)
	addi	r4,r0,+0001
	addi	r5,r0,+0000
	bl	fn004024F0
	nop
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	blr

;; fn004035CC: 004035CC
;;   Called from:
;;     00402C60 (in fn00402C4C)
fn004035CC proc
	mflr	r12
	stwu	r1,-56(r1)
	stw	r12,64(r1)
	addi	r3,r0,+00FC
	bl	fn00403618
	lwz	r4,232(r2)
	lwz	r4,0(r4)
	cmplwi	cr1,r4,0000
	beq	cr1,$00403600

l004035F0:
	lwz	r11,232(r2)
	lwz	r12,0(r11)
	bl	fn00403F10
	lwz	r2,20(r1)

l00403600:
	addi	r3,r0,+00FF
	bl	fn00403618
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	blr

;; fn00403618: 00403618
;;   Called from:
;;     00402C6C (in fn00402C4C)
;;     0040327C (in fn0040326C)
;;     004035DC (in fn004035CC)
;;     00403604 (in fn004035CC)
fn00403618 proc
	mflr	r12
	lwz	r8,236(r2)
	stwu	r1,-64(r1)
	stw	r12,72(r1)
	addi	r8,r8,+0000
	addi	r9,r8,+0058
	addi	r10,r0,+0000
	ori	r11,r8,0000

l00403638:
	lwz	r4,0(r11)
	cmp	cr1,r4,r3
	beq	cr1,$00403654

l00403644:
	addi	r11,r11,+0008
	cmplw	cr1,r11,r9
	addi	r10,r10,+0001
	blt	cr1,$00403638

l00403654:
	rlwinm	r11,r10,03,00,1C
	lwzx	r4,r8,r11
	cmp	cr1,r4,r3
	bne	cr1,$004036A0

l00403664:
	addi	r4,r8,+0004
	lwzx	r4,r4,r11
	lbz	r5,0(r4)
	stb	r5,56(r1)
	lbz	r6,56(r1)
	addi	r5,r0,+0000
	cmpwi	cr1,r6,+0000
	beq	cr1,$00403694

l00403684:
	addi	r5,r5,+0001
	lbzx	r6,r4,r5
	cmpwi	cr1,r6,+0000
	bne	cr1,$00403684

l00403694:
	addi	r3,r0,+0002
	bl	fn00402248
	nop

l004036A0:
	lwz	r12,72(r1)
	addi	r1,r1,+0040
	mtlr	r12
	blr

;; fn004036B0: 004036B0
;;   Called from:
;;     00402D6C (in fn00402CE0)
fn004036B0 proc
	lwz	r10,104(r2)
	lwz	r11,0(r10)
	addi	r5,r10,+000C
	cmplw	cr1,r11,r5
	bne	cr1,$004036CC

l004036C4:
	addi	r3,r0,-0003
	blr

l004036CC:
	lwz	r5,4(r11)
	addi	r9,r0,-0004
	and	r5,r5,r9
	cmplw	cr1,r5,r3
	ble	cr1,$004036E8

l004036E0:
	addi	r3,r0,-0001
	blr

l004036E8:
	lwz	r10,16(r10)
	and	r10,r10,r9
	cmplw	cr1,r10,r3
	bgt	cr1,$00403700

l004036F8:
	addi	r3,r0,-0002
	blr

l00403700:
	ori	r10,r11,0000

l00403704:
	lwz	r11,0(r10)
	lwz	r5,4(r11)
	and	r5,r5,r9
	cmplw	cr1,r5,r3
	bgt	cr1,$00403720

l00403718:
	ori	r10,r11,0000
	b	$00403704

l00403720:
	stw	r10,0(r4)
	lwz	r10,4(r10)
	addi	r11,r0,+0000
	and	r9,r10,r9
	cmplw	cr1,r9,r3
	beq	cr1,$0040373C

l00403738:
	addi	r11,r0,+0001

l0040373C:
	ori	r3,r11,0000
	blr

;; fn00403744: 00403744
;;   Called from:
;;     004031FC (in fn00403170)
fn00403744 proc
	lbz	r4,0(r3)
	mflr	r12
	extsb.	r4,r4
	stwu	r1,-408(r1)
	stw	r12,416(r1)
	stmw	r29,392(r1)
	beq	$00403880

l00403760:
	lbz	r4,0(r3)
	addi	r11,r0,+0000
	cmpwi	cr1,r4,+0000
	beq	cr1,$00403780

l00403770:
	addi	r11,r11,+0001
	lbzx	r4,r3,r11
	cmpwi	cr1,r4,+0000
	bne	cr1,$00403770

l00403780:
	cmplwi	cr1,r11,0100
	bge	cr1,$00403880

l00403788:
	addi	r29,r1,+0038
	addi	r11,r0,+0000

l00403790:
	lbzx	r4,r3,r11
	cmpwi	cr1,r4,+0000
	stbx	r4,r29,r11
	addi	r11,r11,+0001
	bne	cr1,$00403790

l004037A4:
	ori	r3,r29,0000
	bl	fn004032AC
	nop
	addi	r31,r1,+0138
	addi	r11,r0,+0000
	addi	r9,r0,+0005
	mtctr	r9
	addi	r30,r0,+0000

l004037C4:
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	bdnz	$004037C4

l004037E8:
	ori	r3,r31,0000
	stw	r29,18(r31)
	bl	fn00403F54
	lwz	r2,20(r1)
	cmpwi	cr1,r3,+0000
	bne	cr1,$00403840

l00403800:
	addi	r11,r0,+0000
	addi	r9,r0,+0005
	mtctr	r9

l0040380C:
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	bdnz	$0040380C

l00403830:
	ori	r3,r31,0000
	stw	r29,18(r31)
	bl	fn00403F6C
	lwz	r2,20(r1)

l00403840:
	cmpwi	cr1,r3,+0000
	beq	cr1,$00403868

l00403848:
	bl	fn00403104
	nop
	lwz	r12,416(r1)
	lmw	r29,392(r1)
	mtlr	r12
	addi	r1,r1,+0198
	addi	r3,r0,-0001
	blr

l00403868:
	lwz	r12,416(r1)
	lmw	r29,392(r1)
	mtlr	r12
	addi	r1,r1,+0198
	addi	r3,r0,+0000
	blr

l00403880:
	lwz	r4,124(r2)
	addi	r1,r1,+0198
	addi	r5,r0,+0002
	stw	r5,0(r4)
	addi	r3,r0,-0001
	blr

;; fn00403898: 00403898
;;   Called from:
;;     004031D4 (in fn00403170)
fn00403898 proc
	lwz	r4,116(r2)
	stwu	r1,-152(r1)
	lwz	r4,0(r4)
	stmw	r28,136(r1)
	ori	r29,r3,0000
	ori	r5,r29,0000
	cmplw	cr1,r4,r5
	mflr	r12
	addi	r3,r0,+0000
	stw	r12,160(r1)
	ble	cr1,$004039E4

l004038C4:
	lwz	r28,120(r2)
	lbzx	r11,r28,r29
	add	r28,r29,r28
	extsb	r11,r11
	andi.	r4,r11,0001
	beq	$004039E4

l004038DC:
	andi.	r11,r11,0040
	bne	$0040399C

l004038E4:
	lwz	r10,156(r2)
	rlwinm	r9,r29,02,00,1D
	lwzx	r11,r10,r9
	cmpwi	cr1,r11,-0001
	bne	cr1,$00403904

l004038F8:
	addi	r30,r0,+0000
	stwx	r30,r10,r9
	b	$004039A0

l00403904:
	addi	r31,r1,+0038
	ori	r3,r31,0000
	extsh	r11,r11
	sth	r11,24(r31)
	addi	r30,r0,+0000
	stw	r30,18(r31)
	bl	fn00403F24
	lwz	r2,20(r1)
	cmpwi	cr1,r3,+0000
	beq	cr1,$0040394C

l0040392C:
	bl	fn00403104
	nop
	lwz	r12,160(r1)
	lmw	r28,136(r1)
	mtlr	r12
	addi	r1,r1,+0098
	addi	r3,r0,-0001
	blr

l0040394C:
	addi	r11,r0,+0000
	addi	r9,r0,+0005
	mtctr	r9

l00403958:
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	stwx	r30,r31,r11
	addi	r11,r11,+0004
	bdnz	$00403958

l0040397C:
	lwz	r4,160(r2)
	rlwinm	r29,r29,01,00,1E
	lhax	r4,r4,r29
	ori	r3,r31,0000
	sth	r4,22(r31)
	bl	fn00403F3C
	lwz	r2,20(r1)
	b	$004039A0

l0040399C:
	addi	r30,r0,+0000

l004039A0:
	cmpwi	cr1,r3,+0000
	stb	r30,0(r28)
	beq	cr1,$004039CC

l004039AC:
	bl	fn00403104
	nop
	lwz	r12,160(r1)
	lmw	r28,136(r1)
	mtlr	r12
	addi	r1,r1,+0098
	addi	r3,r0,-0001
	blr

l004039CC:
	lwz	r12,160(r1)
	lmw	r28,136(r1)
	mtlr	r12
	addi	r1,r1,+0098
	addi	r3,r0,+0000
	blr

l004039E4:
	lwz	r4,124(r2)
	lwz	r5,164(r2)
	lmw	r28,136(r1)
	addi	r1,r1,+0098
	addi	r6,r0,+0009
	stw	r6,0(r4)
	addi	r7,r0,+0000
	stw	r7,0(r5)
	addi	r3,r0,-0001
	blr

;; fn00403A0C: 00403A0C
;;   Called from:
;;     004031C8 (in fn00403170)
;;     004034C4 (in fn00403450)
fn00403A0C proc
	stwu	r1,-64(r1)
	stw	r31,56(r1)
	ori	r31,r3,0000
	lwz	r11,12(r31)
	mflr	r12
	andi.	r4,r11,0083
	stw	r12,72(r1)
	beq	$00403A60

l00403A2C:
	andi.	r11,r11,0008
	beq	$00403A60

l00403A34:
	lwz	r3,8(r31)
	bl	fn00402BA0
	nop
	lwz	r4,12(r31)
	addi	r5,r0,+0000
	stw	r5,0(r31)
	stw	r5,8(r31)
	stw	r5,4(r31)
	addi	r6,r0,-0009
	and	r4,r4,r6
	stw	r4,12(r31)

l00403A60:
	lwz	r12,72(r1)
	lwz	r31,56(r1)
	mtlr	r12
	addi	r1,r1,+0040
	blr
00403A74             2C 83 00 16 7D 88 02 A6 94 21 FF C0     ,...}....!..
00403A80 91 81 00 48 93 E1 00 38 60 9F 00 00 41 86 00 2C ...H...8`...A..,
00403A90 2C 83 00 0F 41 86 00 24 2C 83 00 02 41 86 00 1C ,...A..$,...A...
00403AA0 2C 83 00 08 41 86 00 14 2C 83 00 04 41 86 00 0C ,...A...,...A...
00403AB0 2C 83 00 0B 40 86 00 30 48 00 01 69 28 83 00 00 ,...@..0H..i(...
00403AC0 41 86 00 24 81 81 00 48 80 83 00 08 7D 88 03 A6 A..$...H....}...
00403AD0 93 E3 00 08 60 83 00 00 83 E1 00 38 38 21 00 40 ....`......88!.@
00403AE0 4E 80 00 20 81 81 00 48 80 82 00 7C 7D 88 03 A6 N.. ...H...|}...
00403AF0 83 E1 00 38 38 21 00 40 38 A0 00 16 90 A4 00 00 ...88!.@8.......
00403B00 38 60 FF FF 4E 80 00 20                         8`..N..         

;; fn00403B08: 00403B08
;;   Called from:
;;     00403288 (in fn0040326C)
fn00403B08 proc
	stwu	r1,-72(r1)
	stmw	r29,56(r1)
	ori	r30,r3,0000
	addi	r11,r30,-0002
	cmplwi	cr1,r11,0015
	mflr	r12
	stw	r12,80(r1)
	bge	cr1,$00403B94

l00403B28:
	rlwinm	r11,r11,02,00,1D
	bl	fn00403B84
	b	$00403BAC
00403B34             48 00 00 60 48 00 00 74 48 00 00 58     H..`H..tH..X
00403B40 48 00 00 54 48 00 00 50 48 00 00 64 48 00 00 48 H..TH..PH..dH..H
00403B50 48 00 00 44 48 00 00 58 48 00 00 3C 48 00 00 38 H..DH..XH..<H..8
00403B60 48 00 00 34 48 00 00 48 48 00 00 2C 48 00 00 28 H..4H..HH..,H..(
00403B70 48 00 00 24 48 00 00 20 48 00 00 1C 48 00 00 18 H..$H.. H...H...
00403B80 48 00 00 2C                                     H..,            

;; fn00403B84: 00403B84
;;   Called from:
;;     00403B2C (in fn00403B08)
fn00403B84 proc
	mflr	r0
	add	r0,r0,r11
	mtlr	r0
	blr

l00403B94:
	lwz	r12,80(r1)
	lmw	r30,60(r1)
	mtlr	r12
	addi	r1,r1,+0048
	addi	r3,r0,-0001
	blr

l00403BAC:
	ori	r3,r30,0000
	bl	fn00403C20
	ori	r29,r3,0000
	lwz	r31,8(r29)
	cmplwi	cr1,r31,0001
	bne	cr1,$00403BDC

l00403BC4:
	lwz	r12,80(r1)
	lmw	r29,56(r1)
	mtlr	r12
	addi	r1,r1,+0048
	addi	r3,r0,+0000
	blr

l00403BDC:
	cmplwi	cr1,r31,0000
	bne	cr1,$00403BF0

l00403BE4:
	addi	r3,r0,+0003
	bl	fn004035A0
	nop

l00403BF0:
	ori	r3,r30,0000
	ori	r12,r31,0000
	addi	r4,r0,+0000
	stw	r4,8(r29)
	bl	fn00403F10
	lwz	r2,20(r1)
	lwz	r12,80(r1)
	lmw	r29,56(r1)
	mtlr	r12
	addi	r1,r1,+0048
	addi	r3,r0,+0000
	blr

;; fn00403C20: 00403C20
;;   Called from:
;;     00403BB0 (in fn00403B08)
fn00403C20 proc
	lwz	r4,240(r2)
	lwz	r10,244(r2)
	lwz	r4,0(r4)
	ori	r11,r10,0000
	mulli	r4,r4,+000C
	add	r10,r4,r10

l00403C38:
	lwz	r4,4(r11)
	cmp	cr1,r4,r3
	beq	cr1,$00403C50

l00403C44:
	addi	r11,r11,+000C
	cmplw	cr1,r10,r11
	bgt	cr1,$00403C38

l00403C50:
	lwz	r4,4(r11)
	ori	r10,r11,0000
	cmp	cr1,r4,r3
	beq	cr1,$00403C64

l00403C60:
	addi	r10,r0,+0000

l00403C64:
	ori	r3,r10,0000
	blr

;; Win32CrtStartup: 00403C6C
Win32CrtStartup proc
	mflr	r12
	stwu	r1,-56(r1)
	stw	r12,64(r1)
	bl	fn00403DD8
	nop
	lwz	r4,340(r2)
	stw	r3,0(r4)
	bl	fn00403CA0
	nop
	lwz	r12,64(r1)
	addi	r1,r1,+0038
	mtlr	r12
	blr

;; fn00403CA0: 00403CA0
;;   Called from:
;;     00403C88 (in Win32CrtStartup)
fn00403CA0 proc
	lwz	r4,212(r2)
	stwu	r1,-112(r1)
	lwz	r4,0(r4)
	stmw	r29,100(r1)
	cmplwi	cr1,r4,0000
	mflr	r12
	lwz	r29,344(r2)
	stw	r12,120(r1)
	stw	r13,96(r1)
	addi	r31,r0,+0000
	stw	r31,0(r29)
	bne	cr1,$00403D38

l00403CD0:
	lwz	r4,348(r2)
	addi	r5,r0,+0001
	stw	r5,0(r4)
	lwz	r6,2320(r0)
	addi	r3,r1,+0038
	stw	r6,0(r3)
	lwz	r7,2324(r0)
	stw	r7,4(r3)
	lwz	r8,2328(r0)
	stw	r8,8(r3)
	lwz	r9,2332(r0)
	stw	r9,12(r3)
	lwz	r10,2336(r0)
	stw	r10,16(r3)
	lwz	r11,2340(r0)
	stw	r11,20(r3)
	lwz	r12,2344(r0)
	stw	r12,24(r3)
	lwz	r13,2348(r0)
	stw	r13,28(r3)
	bl	fn00403D7C
	lwz	r5,352(r2)
	addi	r4,r1,+0058
	stw	r3,0(r4)
	stw	r31,4(r4)
	stw	r4,0(r5)

l00403D38:
	lwz	r30,348(r2)
	lwz	r31,352(r2)
	bl	fn00402404
	nop
	lwz	r3,0(r30)
	lwz	r4,0(r31)
	lwz	r5,0(r29)
	bl	fn00400220
	nop
	bl	fn00403574
	nop
	lwz	r12,120(r1)
	lwz	r13,96(r1)
	mtlr	r12
	lmw	r29,100(r1)
	addi	r1,r1,+0070
	blr

;; fn00403D7C: 00403D7C
;;   Called from:
;;     00403D20 (in fn00403CA0)
fn00403D7C proc
	cmplwi	cr1,r3,0000
	beq	cr1,$00403DCC

l00403D84:
	lbz	r8,0(r3)
	cmplwi	cr1,r8,0000
	beq	cr1,$00403DCC

l00403D90:
	ori	r11,r8,0000
	cmpwi	cr1,r11,+0000
	addi	r9,r3,+0001
	ori	r10,r3,0000
	beq	cr1,$00403DC4

l00403DA4:
	addi	r10,r3,-0001
	addi	r9,r9,-0001

l00403DAC:
	addi	r11,r11,-0001
	cmpwi	cr1,r11,+0000
	lbzu	r4,1(r9)
	stbu	r4,1(r10)
	bne	cr1,$00403DAC

l00403DC0:
	addi	r10,r10,+0001

l00403DC4:
	addi	r4,r0,+0000
	stb	r4,0(r10)

l00403DCC:
	blr
00403DD0 60 83 00 00 4E 80 00 20                         `...N..         

;; fn00403DD8: 00403DD8
;;   Called from:
;;     00403C78 (in Win32CrtStartup)
fn00403DD8 proc
	ori	r3,r1,0000
	blr
00403DE0 60 43 00 00 4E 80 00 20 94 21 FE F4 93 E1 00 20 `C..N.. .!..... 
00403DF0 BD A1 00 24 D9 C1 00 70 D9 E1 00 78 DA 01 00 80 ...$...p...x....
00403E00 DA 21 00 88 DA 41 00 90 DA 61 00 98 DA 81 00 A0 .!...A...a......
00403E10 DA A1 00 A8 DA C1 00 B0 DA E1 00 B8 DB 01 00 C0 ................
00403E20 DB 21 00 C8 DB 41 00 D0 DB 61 00 D8 DB 81 00 E0 .!...A...a......
00403E30 DB A1 00 E8 DB C1 00 F0 DB E1 00 F8 7C A8 02 A6 ............|...
00403E40 90 A1 01 14 90 41 01 20 60 9F 00 00 80 63 00 00 .....A. `....c..
00403E50 48 00 00 85 7C 69 03 A6 80 84 00 14 28 04 00 00 H...|i......(...
00403E60 41 82 00 08 60 82 00 00 4E 80 04 21 80 41 01 20 A...`...N..!.A. 
00403E70 48 00 00 65 83 E1 00 20 B9 A1 00 24 C9 C1 00 70 H..e... ...$...p
00403E80 C9 E1 00 78 CA 01 00 80 CA 21 00 88 CA 41 00 90 ...x.....!...A..
00403E90 CA 61 00 98 CA 81 00 A0 CA A1 00 A8 CA C1 00 B0 .a..............
00403EA0 CA E1 00 B8 CB 01 00 C0 CB 21 00 C8 CB 41 00 D0 .........!...A..
00403EB0 CB 61 00 D8 CB 81 00 E0 CB A1 00 E8 CB C1 00 F0 .a..............
00403EC0 CB E1 00 F8 80 A1 01 14 7C A8 03 A6 30 21 01 0C ........|...0!..
00403ED0 4E 80 00 20                                     N..             

;; fn00403ED4: 00403ED4
fn00403ED4 proc
	lwz	r5,356(r2)
	stw	r3,4(r5)
	blr
00403EE0 60 9F 00 00 80 C4 00 14 28 06 00 00 41 82 00 08 `.......(...A...
00403EF0 60 C2 00 00 7C 05 20 40 41 82 00 08 60 A4 00 00 `...|. @A...`...
00403F00 60 81 00 00 7C 69 03 A6 4E 80 04 20 00 00 00 00 `...|i..N.. ....

;; fn00403F10: 00403F10
;;   Called from:
;;     00401814 (in fn004017F0)
;;     004022BC (in fn00402248)
;;     00402668 (in fn0040262C)
;;     00402C80 (in fn00402C4C)
;;     004035F8 (in fn004035CC)
;;     00403C00 (in fn00403B08)
fn00403F10 proc
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00403F24: 00403F24
;;   Called from:
;;     0040391C (in fn00403898)
fn00403F24 proc
	lwz	r12,336(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00403F3C: 00403F3C
;;   Called from:
;;     004021C8 (in fn004020BC)
;;     00403990 (in fn00403898)
fn00403F3C proc
	lwz	r12,284(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00403F54: 00403F54
;;   Called from:
;;     004037F0 (in fn00403744)
fn00403F54 proc
	lwz	r12,332(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00403F6C: 00403F6C
;;   Called from:
;;     00403838 (in fn00403744)
fn00403F6C proc
	lwz	r12,328(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00403F84: 00403F84
;;   Called from:
;;     00403254 (in fn00403248)
fn00403F84 proc
	lwz	r12,260(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00403F9C: 00403F9C
;;   Called from:
;;     00402464 (in fn00402404)
;;     00402AC0 (in fn00402A24)
fn00403F9C proc
	lwz	r12,296(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00403FB4: 00403FB4
;;   Called from:
;;     00402AD8 (in fn00402A24)
fn00403FB4 proc
	lwz	r12,324(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00403FCC: 00403FCC
;;   Called from:
;;     00402B40 (in fn00402A24)
fn00403FCC proc
	lwz	r12,320(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00403FE4: 00403FE4
;;   Called from:
;;     00402794 (in fn004026B8)
fn00403FE4 proc
	lwz	r12,316(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00403FFC: 00403FFC
;;   Called from:
;;     004027F8 (in fn004026B8)
fn00403FFC proc
	lwz	r12,312(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00404014: 00404014
;;   Called from:
;;     0040286C (in fn004026B8)
;;     00402918 (in fn004026B8)
fn00404014 proc
	lwz	r12,308(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn0040402C: 0040402C
;;   Called from:
;;     004028C0 (in fn004026B8)
fn0040402C proc
	lwz	r12,304(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00404044: 00404044
;;   Called from:
;;     00402434 (in fn00402404)
;;     00402448 (in fn00402404)
fn00404044 proc
	lwz	r12,300(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn0040405C: 0040405C
;;   Called from:
;;     00402328 (in fn00402248)
fn0040405C proc
	lwz	r12,292(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00404074: 00404074
;;   Called from:
;;     00402150 (in fn004020BC)
fn00404074 proc
	lwz	r12,288(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn0040408C: 0040408C
;;   Called from:
;;     00401BCC (in fn00401ADC)
fn0040408C proc
	lwz	r12,276(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn004040A4: 004040A4
;;   Called from:
;;     00401BF8 (in fn00401ADC)
fn004040A4 proc
	lwz	r12,264(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn004040BC: 004040BC
;;   Called from:
;;     00401C28 (in fn00401ADC)
fn004040BC proc
	lwz	r12,256(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn004040D4: 004040D4
;;   Called from:
;;     00401C54 (in fn00401ADC)
fn004040D4 proc
	lwz	r12,272(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn004040EC: 004040EC
;;   Called from:
;;     00401C64 (in fn00401ADC)
fn004040EC proc
	lwz	r12,268(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00404104: 00404104
;;   Called from:
;;     00401A44 (in fn00401A34)
;;     00401D24 (in fn00401CE8)
fn00404104 proc
	lwz	r12,252(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn0040411C: 0040411C
;;   Called from:
;;     00401E38 (in fn00401DD8)
fn0040411C proc
	lwz	r12,280(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00

;; fn00404134: 00404134
;;   Called from:
;;     00401F08 (in fn00401ED4)
fn00404134 proc
	lwz	r12,248(r2)
	lwz	r0,0(r12)
	stw	r2,20(r1)
	mtctr	r0
	lwz	r2,4(r12)
	bcctr	14,00
0040414C                                     00 00 00 00             ....
