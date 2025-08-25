;;; Segment .text (000000CC)
000000CC                                     00 00 00 00             ....
000000D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
000000E0 00 00 00 00 00 00 00 00 00 00 00 00             ............    

;; _entry: 000000EC
_entry proc
	adjspb	8
	movd	8(sp),0(sp)
	addr	12(sp),r0
	movd	r0,4(sp)

l000000F9:
	cmpqd	0,0(r0)
	addqd	4,r0
	bne	000000F9

l00000100:
	cmpd	r0,0(4(sp))
	bls	00000108

l00000106:
	addqd	-4,r0

l00000108:
	movd	r0,8(sp)
	movd	r0,@000115D4
	jsr	fn0000012C
	adjspb	H'F4
	movd	r0,tos
	jsr	fn000055C4
	movqd	1,r0
	addr	0(sp),r1
	svc
	ret	0
0000012A                               F2 F2                       ..    

;; fn0000012C: 0000012C
;;   Called from:
;;     00000111 (in _entry)
fn0000012C proc
	enter	[],8
	cmpqd	2,8(fp)
	ble	0000013A

l00000134:
	jsr	fn00000776

l0000013A:
	movqd	0,@00014C68
	movd	H'115F0,@00014C6C
	movd	H'115FC,@00014C70
	movd	8(fp),r0
	ashd	2,r0
	movd	12(fp),r1
	addd	r0,r1
	movqd	0,0(r1)
	addqd	4,12(fp)
	movd	12(fp),r0
	addqd	4,12(fp)
	movd	0(r0),-4(fp)
	movqd	0,-8(fp)
	movxbd	0(-4(fp)),r0
	cmpqd	0,r0
	beq	0000045D

l0000017D:
	movxbd	0(-4(fp)),r0
	cmpd	H'6D,r0
	beq	000001BB

l0000018A:
	blt	00000317

l0000018D:
	cmpd	H'36,r0
	beq	00000217

l00000196:
	blt	000001F6

l00000199:
	cmpd	H'32,r0
	beq	00000217

l000001A2:
	blt	000001D6

l000001A4:
	cmpd	H'30,r0
	beq	00000217

l000001AD:
	blt	000001CA

l000001AF:
	cmpd	H'2D,r0
	beq	0000044D

l000001B8:
	br	0000042D

l000001BB:
	cmpqd	1,-8(fp)
	beq	0000044D

l000001C1:
	addqd	1,@00014C40
	br	0000044D

l000001CA:
	cmpd	H'31,r0
	beq	00000217

l000001D3:
	br	0000042D

l000001D6:
	cmpd	H'34,r0
	beq	00000217

l000001DE:
	blt	000001EB

l000001E0:
	cmpd	H'33,r0
	beq	00000217

l000001E8:
	br	0000042D

l000001EB:
	cmpd	H'35,r0
	beq	00000217

l000001F3:
	br	0000042D

l000001F6:
	cmpd	H'66,r0
	beq	00000241

l000001FF:
	blt	000002D8

l00000202:
	cmpd	H'62,r0
	beq	00000260

l0000020B:
	blt	000002C0

l0000020E:
	cmpd	H'37,r0
	bne	0000042D

l00000217:
	movb	0(-4(fp)),@000115F8
	movb	0(-4(fp)),@00011603
	movqd	2,-8(fp)
	movd	H'115F0,@00014C6C
	movd	H'115FC,@00014C70
	br	0000044D

l00000241:
	movd	12(fp),r0
	addqd	4,12(fp)
	movd	0(r0),@00014C6C
	cmpqd	1,@000115DC
	bne	0000044D

l00000257:
	movqd	0,@000115DC
	br	0000044D

l00000260:
	movd	12(fp),r0
	addqd	4,12(fp)
	movd	0(r0),tos
	jsr	fn0000247C
	adjspb	H'FC
	movd	r0,@000115DC
	cmpd	r0,H'14
	bgt	00000288

l00000280:
	cmpqd	0,@000115DC
	blt	000002AB

l00000288:
	addr	@00000014,tos
	movd	H'11743,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	movqd	1,tos
	jsr	fn00001772
	adjspb	H'FC

l000002AB:
	cmpqd	0,@00014C28
	beq	0000044D

l000002B4:
	cmpqd	0,@00014C3C
	bne	0000044D

l000002BD:
	br	000003C7

l000002C0:
	cmpd	H'63,r0
	bne	0000042D

l000002C9:
	addqd	1,@00014C3C
	addqd	1,@00014C28
	br	0000044D

l000002D8:
	cmpd	H'6C,r0
	beq	000002F4

l000002E0:
	blt	0000042D

l000002E3:
	cmpd	H'68,r0
	bne	0000042D

l000002EC:
	cmpqd	1,-8(fp)
	bne	0000042D

l000002F2:
	br	000002F9

l000002F4:
	cmpqd	1,-8(fp)
	bne	0000030E

l000002F9:
	movb	0(-4(fp)),@000115F9
	movd	H'115F0,@00014C6C
	br	0000044D

l0000030E:
	addqd	1,@00014C5C
	br	0000044D

l00000317:
	cmpd	H'75,r0
	beq	00000340

l0000031F:
	blt	000003FB

l00000322:
	cmpd	H'72,r0
	beq	000003AF

l0000032B:
	blt	000003EA

l0000032E:
	cmpd	H'6F,r0
	bne	0000042D

l00000337:
	addqd	1,@00014C44
	br	0000044D

l00000340:
	movd	H'115E0,tos
	jsr	fn000037C0
	adjspb	H'FC
	movd	H'116D4,tos
	movd	H'115E0,tos
	jsr	fn00003650
	adjspb	H'F8
	movd	r0,@00014C68
	movd	@00014C68,r1
	cmpqd	0,r1
	bne	0000039A

l00000374:
	movd	H'115E0,tos
	movd	H'116D6,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	movqd	1,tos
	jsr	fn00001772
	adjspb	H'FC

l0000039A:
	movd	H'116FE,tos
	movd	@00014C68,tos
	jsr	fn000025E4
	adjspb	H'F8

l000003AF:
	addqd	1,@00014C28
	cmpqd	1,@000115DC
	beq	0000044D

l000003BE:
	cmpqd	0,@00014C3C
	bne	0000044D

l000003C7:
	movd	H'11717,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	1,tos
	jsr	fn00001772
	adjspb	H'FC
	br	0000044D

l000003EA:
	cmpd	H'74,r0
	bne	0000042D

l000003F2:
	addqd	1,@00014C34
	br	0000044D

l000003FB:
	cmpd	H'77,r0
	beq	00000415

l00000403:
	blt	0000041D

l00000405:
	cmpd	H'76,r0
	bne	0000042D

l0000040D:
	addqd	1,@00014C30
	br	0000044D

l00000415:
	addqd	1,@00014C50
	br	0000044D

l0000041D:
	cmpd	H'78,r0
	bne	0000042D

l00000425:
	addqd	1,@00014C2C
	br	0000044D

l0000042D:
	movxbd	0(-4(fp)),tos
	movd	H'11760,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	jsr	fn00000776

l0000044D:
	addqd	1,-4(fp)
	addqd	-1,-8(fp)
	movxbd	0(-4(fp)),r0
	cmpqd	0,r0
	bne	0000017D

l0000045D:
	cmpqd	0,@00014C28
	beq	00000600

l00000466:
	cmpqd	0,@00014C3C
	beq	00000487

l0000046E:
	cmpqd	0,@00014C68
	beq	00000487

l00000476:
	jsr	fn00000776
	movqd	1,tos
	jsr	fn00001772
	adjspb	H'FC

l00000487:
	movqd	1,tos
	movqd	2,tos
	jsr	fn00003514
	adjspb	H'F8
	cmpqd	1,r0
	beq	000004A9

l00000498:
	movd	H'14C9,tos
	movqd	2,tos
	jsr	fn00003514
	adjspb	H'F8

l000004A9:
	movqd	1,tos
	movqd	1,tos
	jsr	fn00003514
	adjspb	H'F8
	cmpqd	1,r0
	beq	000004CB

l000004BA:
	movd	H'14FD,tos
	movqd	1,tos
	jsr	fn00003514
	adjspb	H'F8

l000004CB:
	movqd	1,tos
	movqd	3,tos
	jsr	fn00003514
	adjspb	H'F8
	cmpqd	1,r0
	beq	000004ED

l000004DC:
	movd	H'14E3,tos
	movqd	3,tos
	jsr	fn00003514
	adjspb	H'F8

l000004ED:
	movd	H'11779,tos
	movd	@00014C6C,tos
	jsr	fn0000321C
	adjspb	H'F8
	cmpqd	0,r0
	bne	00000549

l00000507:
	cmpqd	0,@00014C3C
	bne	0000052F

l0000050F:
	movd	H'1177B,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	1,tos
	jsr	fn00001772
	adjspb	H'FC

l0000052F:
	movqd	1,tos
	jsr	fn000022A0
	adjspb	H'FC
	movd	r0,@00014C38
	movqd	1,@000115DC
	br	000005DE

l00000549:
	movqd	2,tos
	movd	@00014C6C,tos
	jsr	fn000038AC
	adjspb	H'F8
	movd	r0,@00014C38
	movd	@00014C38,r1
	cmpqd	0,r1
	ble	000005DE

l0000056B:
	movqd	2,tos
	movd	@00014C70,tos
	jsr	fn000038AC
	adjspb	H'F8
	movd	r0,@00014C38
	movd	@00014C38,r1
	cmpqd	0,r1
	ble	000005DE

l0000058D:
	cmpqd	0,@00014C3C
	beq	000005B8

l00000595:
	addr	@000001B6,tos
	movd	@00014C6C,tos
	jsr	fn00002550
	adjspb	H'F8
	movd	r0,@00014C38
	movd	@00014C38,r1
	cmpqd	0,r1
	ble	000005DE

l000005B8:
	movd	@00014C6C,tos
	movd	H'117A5,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	movqd	1,tos
	jsr	fn00001772
	adjspb	H'FC

l000005DE:
	cmpqd	0,@00014C3C
	bne	000005F4

l000005E6:
	cmpqd	0,@000115DC
	bne	000005F4

l000005EE:
	movqd	1,@000115DC

l000005F4:
	movd	12(fp),tos
	jsr	fn0000079D
	br	000006AE

l00000600:
	cmpqd	0,@00014C2C
	beq	000006E7

l00000609:
	movd	H'117BA,tos
	movd	@00014C6C,tos
	jsr	fn0000321C
	adjspb	H'F8
	cmpqd	0,r0
	bne	0000063C

l00000622:
	movqd	0,tos
	jsr	fn000022A0
	adjspb	H'FC
	movd	r0,@00014C38
	movqd	1,@000115DC
	br	000006A5

l0000063C:
	movqd	0,tos
	movd	@00014C6C,tos
	jsr	fn000038AC
	adjspb	H'F8
	movd	r0,@00014C38
	movd	@00014C38,r1
	cmpqd	0,r1
	ble	000006A5

l0000065E:
	movqd	0,tos
	movd	@00014C70,tos
	jsr	fn000038AC
	adjspb	H'F8
	movd	r0,@00014C38
	movd	@00014C38,r1
	cmpqd	0,r1
	ble	000006A5

l0000067F:
	movd	@00014C6C,tos
	movd	H'117BC,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	movqd	1,tos
	jsr	fn00001772
	adjspb	H'FC

l000006A5:
	movd	12(fp),tos
	jsr	fn00000FF5

l000006AE:
	adjspb	H'FC
	br	000006F5

l000006B4:
	movd	H'117D1,tos
	movd	@00014C6C,tos
	jsr	fn0000321C
	adjspb	H'F8
	cmpqd	0,r0
	bne	00000704

l000006CD:
	movqd	0,tos
	jsr	fn000022A0
	adjspb	H'FC
	movd	r0,@00014C38
	movqd	1,@000115DC
	br	0000076D

l000006E7:
	cmpqd	0,@00014C34
	bne	000006B4

l000006EF:
	jsr	fn00000776

l000006F5:
	movqd	0,tos
	jsr	fn00001772
	adjspb	H'FC
	exit	[]
	ret	0

l00000704:
	movqd	0,tos
	movd	@00014C6C,tos
	jsr	fn000038AC
	adjspb	H'F8
	movd	r0,@00014C38
	movd	@00014C38,r1
	cmpqd	0,r1
	ble	0000076D

l00000726:
	movqd	0,tos
	movd	@00014C70,tos
	jsr	fn000038AC
	adjspb	H'F8
	movd	r0,@00014C38
	movd	@00014C38,r1
	cmpqd	0,r1
	ble	0000076D

l00000747:
	movd	@00014C6C,tos
	movd	H'117D3,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	movqd	1,tos
	jsr	fn00001772
	adjspb	H'FC

l0000076D:
	jsr	fn0000127A
	br	000006F5

;; fn00000776: 00000776
;;   Called from:
;;     00000134 (in fn0000012C)
;;     00000447 (in fn0000012C)
;;     00000476 (in fn0000012C)
;;     000006EF (in fn0000012C)
fn00000776 proc
	enter	[],0
	movd	H'117E8,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	1,tos
	jsr	fn00001772
	adjspb	H'FC
	exit	[]
	ret	0

;; fn0000079D: 0000079D
;;   Called from:
;;     000005F7 (in fn0000012C)
fn0000079D proc
	enter	[r6,r7],H'104
	cmpqd	0,@00014C3C
	bne	000008BF

l000007AA:
	jsr	fn000009BD

l000007B0:
	jsr	fn00000AE7
	cmpqd	0,@00014C48
	beq	000007C9

l000007BE:
	movqd	0,tos
	jsr	fn00001772
	adjspb	H'FC

l000007C9:
	jsr	fn000009BD
	jsr	fn0000099F
	cmpqd	0,r0
	beq	000007B0

l000007D9:
	cmpqd	0,@00014C68
	beq	000008BF

l000007E2:
	movd	H'1183C,tos
	addr	-260(fp),tos
	jsr	fn000038D4
	adjspb	H'F8
	movd	H'115E0,tos
	addr	-260(fp),tos
	jsr	fn000038D4
	adjspb	H'F8
	movd	H'1184D,tos
	addr	-260(fp),tos
	jsr	fn000038D4
	adjspb	H'F8
	movd	H'115E0,tos
	addr	-260(fp),tos
	jsr	fn000038D4
	adjspb	H'F8
	movd	H'115E0,tos
	movd	H'115E0,tos
	movd	H'115E0,tos
	movd	H'115E0,tos
	movd	H'115E0,tos
	movd	H'115E0,tos
	movd	H'11852,tos
	addr	-260(fp),tos
	jsr	fn000031E0
	adjspb	H'E0
	movd	@00014C68,tos
	jsr	fn00004D67
	adjspb	H'FC
	addr	-260(fp),tos
	jsr	fn00003404
	adjspb	H'FC
	movd	@00014C68,tos
	movd	H'118A0,tos
	movd	H'115E0,tos
	jsr	fn0000366E
	adjspb	H'F4
	movd	H'14C08,tos
	movd	@00014C68,r0
	movxbd	13(r0),tos
	jsr	fn00003884
	adjspb	H'F8
	movd	@00014C18,@00014C64

l000008BF:
	addr	-60(fp),tos
	jsr	fn000017CF
	br	00000926

l000008CB:
	movd	0(8(fp)),r6
	movd	0(8(fp)),r7
	movxbd	0(r7),r0
	cmpqd	0,r0
	beq	000008ED

l000008DB:
	cmpb	0(r7),H'2F
	bne	000008E3

l000008E1:
	movd	r7,r6

l000008E3:
	addqd	1,r7
	movxbd	0(r7),r0
	cmpqd	0,r0
	bne	000008DB

l000008ED:
	cmpd	r6,0(8(fp))
	beq	00000909

l000008F3:
	movqb	0,0(r6)
	movd	0(8(fp)),tos
	jsr	fn00001D6C
	adjspb	H'FC
	movb	H'2F,0(r6)
	addqd	1,r6

l00000909:
	movd	r6,tos
	movd	8(fp),r0
	addqd	4,8(fp)
	movd	0(r0),tos
	jsr	fn00000B29
	adjspb	H'F8
	addr	-60(fp),tos
	jsr	fn00001D6C

l00000926:
	adjspb	H'FC
	cmpqd	0,0(8(fp))
	beq	00000938

l0000092F:
	cmpqd	0,@00014C48
	beq	000008CB

l00000938:
	jsr	fn000012F4
	jsr	fn000012F4
	jsr	fn00001D28
	cmpqd	1,@00014C5C
	bne	0000099B

l00000953:
	cmpqd	0,@00014C04
	beq	0000099B

l0000095C:
	movd	@00014C04,r0
	cmpqd	0,4(r0)
	beq	00000985

l00000967:
	movd	@00014C04,r0
	addr	8(r0),tos
	movd	H'118A2,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4

l00000985:
	movd	@00014C04,r0
	movd	108(r0),@00014C04
	cmpqd	0,@00014C04
	bne	0000095C

l0000099B:
	exit	[r6,r7]
	ret	0

;; fn0000099F: 0000099F
;;   Called from:
;;     000007CF (in fn0000079D)
;;     00001181 (in fn00000FF5)
;;     000012E5 (in fn0000127A)
fn0000099F proc
	enter	[],0
	movxbd	@00012204,r0
	cmpqd	0,r0
	bne	000009B9

l000009AD:
	jsr	fn00001C95
	movqd	1,r0

l000009B5:
	exit	[]
	ret	0

l000009B9:
	movqd	0,r0
	br	000009B5

;; fn000009BD: 000009BD
;;   Called from:
;;     000007AA (in fn0000079D)
;;     000007C9 (in fn0000079D)
;;     0000117B (in fn00000FF5)
;;     000012DF (in fn0000127A)
fn000009BD proc
	enter	[r7],8
	movd	H'12204,tos
	jsr	fn00001A4F
	adjspb	H'FC
	movxbd	@00012204,r0
	cmpqd	0,r0
	beq	00000AE3

l000009DB:
	movd	H'14C08,r7
	addr	-4(fp),tos
	movd	H'118B7,tos
	movd	H'12268,tos
	jsr	fn000026BD
	adjspb	H'F4
	movzwd	-4(fp),r0
	movw	r0,4(r7)
	addr	-4(fp),tos
	movd	H'118BA,tos
	movd	H'12270,tos
	jsr	fn000026BD
	adjspb	H'F4
	movzwd	-4(fp),r0
	movw	r0,8(r7)
	addr	-4(fp),tos
	movd	H'118BD,tos
	movd	H'12278,tos
	jsr	fn000026BD
	adjspb	H'F4
	movzwd	-4(fp),r0
	movw	r0,10(r7)
	addr	16(r7),tos
	movd	H'118C0,tos
	movd	H'12280,tos
	jsr	fn000026BD
	adjspb	H'F4
	addr	24(r7),tos
	movd	H'118C4,tos
	movd	H'1228C,tos
	jsr	fn000026BD
	adjspb	H'F4
	movd	H'14C4C,tos
	movd	H'118C8,tos
	movd	H'12298,tos
	jsr	fn000026BD
	adjspb	H'F4
	jsr	fn000015DA
	movd	r0,-8(fp)
	cmpd	@00014C4C,r0
	beq	00000ABA

l00000A9A:
	movd	H'118CB,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	2,tos
	jsr	fn00001772
	adjspb	H'FC

l00000ABA:
	cmpqd	0,@00014C68
	beq	00000AE3

l00000AC2:
	movd	H'1228C,tos
	movd	H'12204,tos
	movd	H'118E5,tos
	movd	@00014C68,tos
	jsr	fn000025E4
	adjspb	H'F0

l00000AE3:
	exit	[r7]
	ret	0

;; fn00000AE7: 00000AE7
;;   Called from:
;;     000007B0 (in fn0000079D)
;;     0000123F (in fn00000FF5)
;;     000012D9 (in fn0000127A)
fn00000AE7 proc
	enter	[],H'204
	cmpb	@000122A0,H'31
	beq	00000B25

l00000AF4:
	movd	@00014C18,-4(fp)
	addr	511(-4(fp)),-4(fp)
	movd	-4(fp),r0
	quod	H'200,r0
	movd	r0,-4(fp)
	br	00000B1D

l00000B10:
	addr	-516(fp),tos
	jsr	fn00001A4F
	adjspb	H'FC

l00000B1D:
	cmpqd	0,-4(fp)
	addqd	-1,-4(fp)
	blt	00000B10

l00000B25:
	exit	[]
	ret	0

;; fn00000B29: 00000B29
;;   Called from:
;;     00000914 (in fn0000079D)
;;     00000F7E (in fn00000B29)
fn00000B29 proc
	enter	[r6,r7],H'228
	movqd	0,tos
	movd	12(fp),tos
	jsr	fn000038AC
	adjspb	H'F8
	movd	r0,-4(fp)
	cmpqd	0,r0
	ble	00000B4E

l00000B42:
	movd	8(fp),tos
	movd	H'118EC,tos
	br	00000C1D

l00000B4E:
	movd	H'14C08,tos
	movd	-4(fp),tos
	jsr	fn00003884
	adjspb	H'F8
	cmpqd	0,@00014C68
	beq	00000B79

l00000B68:
	movd	8(fp),tos
	jsr	fn00001701
	adjspb	H'FC
	cmpqd	0,r0
	beq	00000D83

l00000B79:
	movd	8(fp),tos
	addr	@00000072,tos
	jsr	fn0000161F
	adjspb	H'F8
	cmpqd	0,r0
	beq	00000D83

l00000B8E:
	movzwd	@00014C0C,r0
	andd	H'F000,r0
	cmpd	r0,H'4000
	bne	00000BFF

l00000BA4:
	movqd	0,-540(fp)
	addr	-520(fp),r7
	movd	-540(fp),r0
	addqd	1,-540(fp)
	movd	8(fp),r1
	movd	r7,r2
	addqd	1,r7
	movb	r0[r1:b],0(r2)
	movxbd	0(r2),r3
	cmpqd	0,r3
	beq	00000BE2

l00000BC7:
	movd	-540(fp),r0
	addqd	1,-540(fp)
	movd	8(fp),r1
	movd	r7,r2
	addqd	1,r7
	movb	r0[r1:b],0(r2)
	movxbd	0(r2),r3
	cmpqd	0,r3
	bne	00000BC7

l00000BE2:
	addqd	-1,r7
	movd	r7,r0
	movb	H'2F,0(r0)
	addqd	1,r7
	movqd	0,-540(fp)
	movd	12(fp),tos
	jsr	fn00001D6C
	adjspb	H'FC
	br	00000FB7

l00000BFF:
	movzwd	@00014C0C,r0
	andd	H'F000,r0
	cmpd	r0,H'8000
	beq	00000C30

l00000C14:
	movd	8(fp),tos
	movd	H'11911,tos

l00000C1D:
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4

l00000C2C:
	exit	[r6,r7]
	ret	0

l00000C30:
	movd	H'14C08,tos
	jsr	fn00001532
	adjspb	H'FC
	movd	8(fp),r6
	movd	H'12204,r7
	movqd	0,-540(fp)
	movd	r6,r0
	addqd	1,r6
	movd	r7,r1
	addqd	1,r7
	movb	0(r0),0(r1)
	movxbd	0(r1),r2
	cmpqd	0,r2
	beq	00000C8C

l00000C60:
	cmpd	-540(fp),H'64
	bge	00000C8C

l00000C6A:
	addqd	1,-540(fp)
	movd	r6,r0
	addqd	1,r6
	movd	r7,r1
	addqd	1,r7
	movb	0(r0),0(r1)
	movxbd	0(r1),r2
	cmpqd	0,r2
	beq	00000C8C

l00000C82:
	cmpd	-540(fp),H'64
	blt	00000C6A

l00000C8C:
	cmpd	-540(fp),H'64
	blt	00000CB1

l00000C96:
	movd	8(fp),tos
	movd	H'11934,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	br	00000D83

l00000CB1:
	cmpqw	1,@00014C0E
	bge	00000E0E

l00000CBA:
	movqd	0,-552(fp)
	movd	@00014C04,-548(fp)
	cmpqd	0,-548(fp)
	beq	00000CF6

l00000CCC:
	cmpw	0(-548(fp)),@00014C0A
	bne	00000CE8

l00000CD7:
	cmpw	2(-548(fp)),@00014C08
	bne	00000CE8

l00000CE2:
	addqd	1,-552(fp)
	br	00000CF6

l00000CE8:
	movd	108(-548(fp)),-548(fp)
	cmpqd	0,-548(fp)
	bne	00000CCC

l00000CF6:
	cmpqd	0,-552(fp)
	beq	00000D8F

l00000CFD:
	addr	8(-548(fp)),tos
	movd	H'122A1,tos
	jsr	fn00003908
	adjspb	H'F8
	movb	H'31,@000122A0
	jsr	fn000015DA
	movd	r0,tos
	movd	H'1194C,tos
	movd	H'12298,tos
	jsr	fn000031E0
	adjspb	H'F4
	movd	H'12204,tos
	jsr	fn00001BA2
	adjspb	H'FC
	cmpqd	0,@00014C30
	beq	00000D7E

l00000D4C:
	movd	8(fp),tos
	movd	H'11950,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	addr	8(-548(fp)),tos
	movd	H'11956,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4

l00000D7E:
	addqd	-1,4(-548(fp))

l00000D83:
	movd	-4(fp),tos
	jsr	fn000050E0
	br	00000FEF

l00000D8F:
	addr	@00000070,tos
	jsr	fn00005130
	adjspb	H'FC
	movd	r0,-548(fp)
	cmpqd	0,r0
	bne	00000DCB

l00000DA4:
	cmpqd	0,@000115D8
	beq	00000E0E

l00000DAD:
	movd	H'11962,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	0,@000115D8
	br	00000E0E

l00000DCB:
	movd	@00014C04,108(-548(fp))
	movd	-548(fp),@00014C04
	movw	@00014C0A,0(-548(fp))
	movw	@00014C08,2(-548(fp))
	movxwd	@00014C0E,r0
	addqd	-1,r0
	movd	r0,4(-548(fp))
	movd	8(fp),tos
	addr	8(-548(fp)),tos
	jsr	fn00003908
	adjspb	H'F8

l00000E0E:
	movd	@00014C18,r0
	addr	511(r0),r0
	quod	H'200,r0
	movd	r0,-8(fp)
	cmpqd	0,@00014C30
	beq	00000E5A

l00000E2A:
	movd	8(fp),tos
	movd	H'11988,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	movd	-8(fp),tos
	movd	H'1198E,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4

l00000E5A:
	jsr	fn000015DA
	movd	r0,tos
	movd	H'1199A,tos
	movd	H'12298,tos
	jsr	fn000031E0
	adjspb	H'F4
	movd	H'12204,tos
	jsr	fn00001BA2
	adjspb	H'FC
	br	00000E98

l00000E88:
	addr	-520(fp),tos
	jsr	fn00001BA2
	adjspb	H'FC
	addqd	-1,-8(fp)

l00000E98:
	addr	@00000200,tos
	addr	-520(fp),tos
	movd	-4(fp),tos
	jsr	fn000038C0
	adjspb	H'F4
	movd	r0,-540(fp)
	movd	-540(fp),r1
	cmpqd	0,r1
	bge	00000EBD

l00000EB8:
	cmpqd	0,-8(fp)
	blt	00000E88

l00000EBD:
	movd	-4(fp),tos
	jsr	fn000050E0
	adjspb	H'FC
	cmpqd	0,-8(fp)
	bne	00000ED4

l00000ECE:
	cmpqd	0,-540(fp)
	beq	00000EF4

l00000ED4:
	movd	8(fp),tos
	movd	H'1199E,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	br	00000EF4

l00000EEE:
	jsr	fn000012F4

l00000EF4:
	cmpqd	0,-8(fp)
	addqd	-1,-8(fp)
	blt	00000EEE

l00000EFC:
	br	00000C2C

l00000EFF:
	movzwd	-536(fp),r0
	cmpqd	0,r0
	beq	00000F36

l00000F08:
	addr	-534(fp),tos
	movd	H'11907,tos
	jsr	fn0000321C
	adjspb	H'F8
	cmpqd	0,r0
	beq	00000F36

l00000F1F:
	addr	-534(fp),tos
	movd	H'11909,tos
	jsr	fn0000321C
	adjspb	H'F8
	cmpqd	0,r0
	bne	00000F3D

l00000F36:
	addqd	1,-540(fp)
	br	00000FB7

l00000F3D:
	movd	r7,r6
	movqd	0,-544(fp)
	cmpd	-544(fp),H'E
	bge	00000F69

l00000F4D:
	addr	-534(fp),r0
	movd	-544(fp),r1
	movb	r0[r1:b],0(r6)
	addqd	1,r6
	addqd	1,-544(fp)
	cmpd	-544(fp),H'E
	blt	00000F4D

l00000F69:
	movqb	0,0(r6)
	movd	-4(fp),tos
	jsr	fn000050E0
	adjspb	H'FC
	movd	r7,tos
	addr	-520(fp),tos
	jsr	fn00000B29
	adjspb	H'F8
	movqd	0,tos
	movd	H'1190C,tos
	jsr	fn000038AC
	adjspb	H'F8
	movd	r0,-4(fp)
	addqd	1,-540(fp)
	movqd	0,tos
	movd	-540(fp),r0
	ashd	4,r0
	movd	r0,tos
	movd	-4(fp),tos
	jsr	fn00003898
	adjspb	H'F4

l00000FB7:
	addr	@00000010,tos
	addr	-536(fp),tos
	movd	-4(fp),tos
	jsr	fn000038C0
	adjspb	H'F4
	cmpqd	0,r0
	bge	00000FD7

l00000FCE:
	cmpqd	0,@00014C48
	beq	00000EFF

l00000FD7:
	movd	-4(fp),tos
	jsr	fn000050E0
	adjspb	H'FC
	movd	H'1190E,tos
	jsr	fn00001D6C

l00000FEF:
	adjspb	H'FC
	br	00000C2C

;; fn00000FF5: 00000FF5
;;   Called from:
;;     000006A8 (in fn0000012C)
fn00000FF5 proc
	enter	[],H'218
	br	0000117B

l00000FFC:
	addqd	4,-524(fp)
	cmpqd	0,0(-524(fp))
	bne	0000119F

l00001008:
	br	0000123F

l0000100B:
	movd	H'12204,tos
	movd	H'119D7,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	br	0000123F

l00001029:
	cmpqd	0,@00014C44
	bne	0000104E

l00001031:
	movzwd	@00014C12,tos
	movzwd	@00014C10,tos
	movd	H'12204,tos
	jsr	fn00001D80
	adjspb	H'F4

l0000104E:
	movd	@00014C18,-8(fp)
	movd	-8(fp),r0
	addr	511(r0),r0
	quod	H'200,r0
	movd	r0,-4(fp)
	cmpqd	0,@00014C30
	beq	00001133

l0000106F:
	movd	-4(fp),tos
	movd	-8(fp),tos
	movd	H'12204,tos
	movd	H'119F0,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'EC
	br	00001133

l00001093:
	movd	H'12204,tos
	movd	H'119B5,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	br	0000117B

l000010B1:
	addr	-520(fp),tos
	jsr	fn00001A4F
	adjspb	H'FC
	cmpd	-8(fp),H'200
	ble	000010EF

l000010C7:
	addr	@00000200,tos
	addr	-520(fp),tos
	movd	-528(fp),tos
	jsr	fn000055A4
	adjspb	H'F4
	cmpqd	0,r0
	ble	0000112D

l000010E1:
	movd	H'12204,tos
	movd	H'11A12,tos
	br	00001113

l000010EF:
	movd	-8(fp),tos
	addr	-520(fp),tos
	movd	-528(fp),tos
	jsr	fn000055A4
	adjspb	H'F4
	cmpqd	0,r0
	ble	0000112D

l00001107:
	movd	H'12204,tos
	movd	H'11A37,tos

l00001113:
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4
	movqd	2,tos
	jsr	fn00001772
	adjspb	H'FC

l0000112D:
	addr	-512(-8(fp)),-8(fp)

l00001133:
	cmpqd	0,-4(fp)
	addqd	-1,-4(fp)
	blt	000010B1

l0000113C:
	movd	-528(fp),tos
	jsr	fn000050E0
	adjspb	H'FC
	cmpqd	0,@00014C40
	bne	0000117B

l00001151:
	movqd	0,tos
	jsr	fn0000363C
	adjspb	H'FC
	movd	r0,-536(fp)
	movd	@00014C20,-532(fp)
	addr	-536(fp),tos
	movd	H'12204,tos
	jsr	fn0000398C
	adjspb	H'F8

l0000117B:
	jsr	fn000009BD
	jsr	fn0000099F
	cmpqd	0,r0
	bne	00001276

l0000118C:
	cmpqd	0,0(8(fp))
	beq	000011B8

l00001192:
	movd	8(fp),-524(fp)
	cmpqd	0,0(-524(fp))
	beq	0000123F

l0000119F:
	movd	H'12204,tos
	movd	0(-524(fp)),tos
	jsr	fn00001794
	adjspb	H'F8
	cmpqd	0,r0
	beq	00000FFC

l000011B8:
	movd	H'12204,tos
	addr	@00000078,tos
	jsr	fn0000161F
	adjspb	H'F8
	cmpqd	0,r0
	beq	0000123F

l000011D0:
	movd	H'12204,tos
	jsr	fn000013F9
	adjspb	H'FC
	cmpb	@000122A0,H'31
	bne	00001248

l000011E9:
	movd	H'12204,tos
	jsr	fn00003978
	adjspb	H'FC
	movd	H'12204,tos
	movd	H'122A1,tos
	jsr	fn0000253C
	adjspb	H'F8
	cmpqd	0,r0
	bgt	00001093

l00001212:
	cmpqd	0,@00014C30
	beq	0000117B

l0000121B:
	movd	H'122A1,tos
	movd	H'12204,tos
	movd	H'119C6,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F0
	br	0000117B

l0000123F:
	jsr	fn00000AE7
	br	0000117B

l00001248:
	movzwd	@00014C0C,r0
	andd	H'FFF,r0
	movd	r0,tos
	movd	H'12204,tos
	jsr	fn00002550
	adjspb	H'F8
	movd	r0,-528(fp)
	movd	-528(fp),r1
	cmpqd	0,r1
	ble	00001029

l00001273:
	br	0000100B

l00001276:
	exit	[]
	ret	0

;; fn0000127A: 0000127A
;;   Called from:
;;     0000076D (in fn0000012C)
fn0000127A proc
	enter	[],0
	br	000012DF

l00001280:
	cmpqd	0,@00014C30
	beq	00001297

l00001288:
	movd	H'14C08,tos
	jsr	fn0000132C
	adjspb	H'FC

l00001297:
	movd	H'12204,tos
	movd	H'11A5C,tos
	jsr	fn00002580
	adjspb	H'F8
	cmpb	@000122A0,H'31
	bne	000012CA

l000012B5:
	movd	H'122A1,tos
	movd	H'11A5F,tos
	jsr	fn00002580
	adjspb	H'F8

l000012CA:
	movd	H'11A6D,tos
	jsr	fn00002580
	adjspb	H'FC
	jsr	fn00000AE7

l000012DF:
	jsr	fn000009BD
	jsr	fn0000099F
	cmpqd	0,r0
	beq	00001280

l000012F0:
	exit	[]
	ret	0

;; fn000012F4: 000012F4
;;   Called from:
;;     00000938 (in fn0000079D)
;;     0000093E (in fn0000079D)
;;     00000EEE (in fn00000B29)
fn000012F4 proc
	enter	[],H'204
	addr	-512(fp),-516(fp)
	addr	0(fp),r0
	cmpd	-516(fp),r0
	bge	0000131B

l00001307:
	movd	-516(fp),r0
	addqd	1,-516(fp)
	movqb	0,0(r0)
	addr	0(fp),r0
	cmpd	-516(fp),r0
	blt	00001307

l0000131B:
	addr	-512(fp),tos
	jsr	fn00001BA2
	adjspb	H'FC
	exit	[]
	ret	0

;; fn0000132C: 0000132C
;;   Called from:
;;     0000128E (in fn0000127A)
;;     0000164B (in fn0000161F)
fn0000132C proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movd	r7,tos
	jsr	fn00001390
	adjspb	H'FC
	movzwd	10(r7),tos
	movzwd	8(r7),tos
	movd	H'11A6F,tos
	jsr	fn00002580
	adjspb	H'F4
	movd	16(r7),tos
	movd	H'11A77,tos
	jsr	fn00002580
	adjspb	H'F8
	addr	24(r7),tos
	jsr	fn00001D94
	adjspb	H'FC
	movd	r0,r6
	addr	20(r6),tos
	movd	r6,r0
	addqd	4,r0
	movd	r0,tos
	movd	H'11A7C,tos
	jsr	fn00002580
	adjspb	H'F4
	exit	[r6,r7]
	ret	0

;; fn00001390: 00001390
;;   Called from:
;;     00001334 (in fn0000132C)
fn00001390 proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movd	H'116B0,r6
	cmpd	r6,H'116D4
	bge	000013BE

l000013A4:
	movd	r7,tos
	movd	r6,r0
	addqd	4,r6
	movd	0(r0),tos
	jsr	fn000013C2
	adjspb	H'F8
	cmpd	r6,H'116D4
	blt	000013A4

l000013BE:
	exit	[r6,r7]
	ret	0

;; fn000013C2: 000013C2
;;   Called from:
;;     000013AD (in fn00001390)
fn000013C2 proc
	enter	[r6,r7],0
	movd	8(fp),r6
	movd	0(r6),r7

l000013CB:
	addqd	4,r6
	addqd	-1,r7
	cmpqd	0,r7
	bgt	000013E3

l000013D3:
	movd	r6,r0
	addqd	4,r6
	movzwd	4(12(fp)),r1
	andd	0(r0),r1
	cmpqd	0,r1
	beq	000013CB

l000013E3:
	movd	0(r6),tos
	movd	H'11A8E,tos
	jsr	fn00002580
	adjspb	H'F8
	exit	[r6,r7]
	ret	0

;; fn000013F9: 000013F9
;;   Called from:
;;     000011D6 (in fn00000FF5)
fn000013F9 proc
	enter	[r6,r7],4
	movd	8(fp),r7
	movd	r7,r6
	addqd	1,r6
	movxbd	0(r6),r0
	cmpqd	0,r0
	beq	000014C5

l0000140C:
	cmpb	0(r6),H'2F
	bne	000014BA

l00001413:
	movqb	0,0(r6)
	movqd	1,tos
	movd	r7,tos
	jsr	fn00003864
	adjspb	H'F8
	cmpqd	0,r0
	ble	000014B6

l00001428:
	jsr	fn000034FC
	cmpqd	0,r0
	bne	00001485

l00001433:
	movqd	0,tos
	movd	r7,tos
	movd	H'11A9C,tos
	movd	H'11A91,tos
	jsr	fn000034AC
	adjspb	H'F0
	movqd	0,tos
	movd	r7,tos
	movd	H'11AB1,tos
	movd	H'11AA2,tos
	jsr	fn000034AC
	adjspb	H'F0
	movd	H'11AB7,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	0,tos
	jsr	fn00001772
	adjspb	H'FC

l00001485:
	addr	-4(fp),tos
	jsr	fn00005588
	adjspb	H'FC
	cmpqd	0,r0
	ble	00001485

l00001495:
	cmpqd	0,@00014C44
	bne	000014B6

l0000149D:
	movzwd	@00014C12,tos
	movzwd	@00014C10,tos
	movd	r7,tos
	jsr	fn00001D80
	adjspb	H'F4

l000014B6:
	movb	H'2F,0(r6)

l000014BA:
	addqd	1,r6
	movxbd	0(r6),r0
	cmpqd	0,r0
	bne	0000140C

l000014C5:
	exit	[r6,r7]
	ret	0
000014C9                            82 00 00 DF B8 5F B9          ....._.
000014D0 7F AE C0 00 35 14 7C A5 F8 8F A8 C0 01 4C 48 92 ....5.|......LH.
000014E0 00 12 00 82 00 00 DF B8 DF B9 7F AE C0 00 35 14 ..............5.
000014F0 7C A5 F8 8F A8 C0 01 4C 48 92 00 12 00 82 00 00 |......LH.......
00001500 DF B8 DF B8 7F AE C0 00 35 14 7C A5 F8 8F A8 C0 ........5.|.....
00001510 01 4C 48 92 00 12 00 82 00 00 DF B8 E7 AD 0F 7F .LH.............
00001520 AE C0 00 35 14 7C A5 F8 8F A8 C0 01 4C 48 92 00 ...5.|......LH..
00001530 12 00                                           ..              

;; fn00001532: 00001532
;;   Called from:
;;     00000C36 (in fn00000B29)
fn00001532 proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movd	H'12204,r6
	cmpd	r6,H'12404
	bge	00001553

l00001546:
	movqb	0,0(r6)
	addqd	1,r6
	cmpd	r6,H'12404
	blt	00001546

l00001553:
	movzwd	4(r7),r0
	andd	H'FFF,r0
	movd	r0,tos
	movd	H'11AD0,tos
	movd	H'12268,tos
	jsr	fn000031E0
	adjspb	H'F4
	movzwd	8(r7),tos
	movd	H'11AD5,tos
	movd	H'12270,tos
	jsr	fn000031E0
	adjspb	H'F4
	movzwd	10(r7),tos
	movd	H'11ADA,tos
	movd	H'12278,tos
	jsr	fn000031E0
	adjspb	H'F4
	movd	16(r7),tos
	movd	H'11ADF,tos
	movd	H'12280,tos
	jsr	fn000031E0
	adjspb	H'F4
	movd	24(r7),tos
	movd	H'11AE6,tos
	movd	H'1228C,tos
	jsr	fn000031E0
	adjspb	H'F4
	exit	[r6,r7]
	ret	0

;; fn000015DA: 000015DA
;;   Called from:
;;     00000A89 (in fn000009BD)
;;     00000D18 (in fn00000B29)
;;     00000E5A (in fn00000B29)
fn000015DA proc
	enter	[r6,r7],0
	movd	H'12298,r6
	cmpd	r6,H'122A0
	bge	000015F9

l000015EB:
	movb	H'20,0(r6)
	addqd	1,r6
	cmpd	r6,H'122A0
	blt	000015EB

l000015F9:
	movqd	0,r7
	movd	H'12204,r6
	cmpd	r6,H'12404
	bge	00001619

l00001609:
	movxbd	0(r6),r0
	addd	r0,r7
	addqd	1,r6
	cmpd	r6,H'12404
	blt	00001609

l00001619:
	movd	r7,r0
	exit	[r6,r7]
	ret	0

;; fn0000161F: 0000161F
;;   Called from:
;;     00000B80 (in fn00000B29)
;;     000011C2 (in fn00000FF5)
fn0000161F proc
	enter	[],0
	cmpqd	0,@00014C50
	beq	00001674

l0000162B:
	movd	8(fp),tos
	movd	H'11AED,tos
	jsr	fn00002580
	adjspb	H'F8
	cmpqd	0,@00014C30
	beq	00001654

l00001645:
	movd	H'14C08,tos
	jsr	fn0000132C
	adjspb	H'FC

l00001654:
	movd	12(fp),tos
	movd	H'11AF1,tos
	jsr	fn00002580
	adjspb	H'F8
	jsr	fn0000167E
	cmpd	r0,H'79
	bne	0000167A

l00001674:
	movqd	1,r0

l00001676:
	exit	[]
	ret	0

l0000167A:
	movqd	0,r0
	br	00001676

;; fn0000167E: 0000167E
;;   Called from:
;;     00001666 (in fn0000161F)
fn0000167E proc
	enter	[],4
	addqd	-1,@0001204C
	cmpqd	0,@0001204C
	ble	000016A6

l0000168F:
	movd	H'1204C,tos
	jsr	fn000030D0
	adjspb	H'FC
	movxbd	r0,r0
	movb	r0,-1(fp)
	br	000016B6

l000016A6:
	movd	@00012050,r0
	addqd	1,@00012050
	movb	0(r0),-1(fp)

l000016B6:
	cmpb	-1(fp),H'A
	bne	000016C8

l000016BC:
	movb	H'6E,-1(fp)

l000016C0:
	movxbd	-1(fp),r0
	exit	[]
	ret	0

l000016C8:
	addqd	-1,@0001204C
	cmpqd	0,@0001204C
	ble	000016E7

l000016D6:
	movd	H'1204C,tos
	jsr	fn000030D0
	adjspb	H'FC
	br	000016F7

l000016E7:
	movd	@00012050,r0
	addqd	1,@00012050
	movzbd	0(r0),r0

l000016F7:
	cmpd	r0,H'A
	bne	000016C8

l000016FF:
	br	000016C0

;; fn00001701: 00001701
;;   Called from:
;;     00000B6B (in fn00000B29)
fn00001701 proc
	enter	[],H'6C
	movd	@00014C68,tos
	jsr	fn0000263C
	adjspb	H'FC
	movd	8(fp),tos
	jsr	fn000018B0
	adjspb	H'FC
	movd	r0,-108(fp)
	movd	-108(fp),r1
	cmpqd	0,r1
	bgt	00001768

l0000172C:
	movqd	0,tos
	movd	-108(fp),tos
	movd	@00014C68,tos
	jsr	fn0000236C
	adjspb	H'F4
	addr	-104(fp),tos
	addr	-100(fp),tos
	movd	H'11AF6,tos
	movd	@00014C68,tos
	jsr	fn000026A0
	adjspb	H'F0
	cmpd	@00014C20,-104(fp)
	ble	0000176E

l00001768:
	movqd	1,r0

l0000176A:
	exit	[]
	ret	0

l0000176E:
	movqd	0,r0
	br	0000176A

;; fn00001772: 00001772
;;   Called from:
;;     000002A2 (in fn0000012C)
;;     00000391 (in fn0000012C)
;;     000003DE (in fn0000012C)
;;     0000047E (in fn0000012C)
;;     00000526 (in fn0000012C)
;;     000005D5 (in fn0000012C)
;;     0000069C (in fn0000012C)
;;     000006F7 (in fn0000012C)
;;     00000764 (in fn0000012C)
;;     00000790 (in fn00000776)
;;     000007C0 (in fn0000079D)
;;     00000AB1 (in fn000009BD)
;;     00001124 (in fn00000FF5)
;;     0000147C (in fn000013F9)
;;     00001AC3 (in fn00001A4F)
;;     00001AFA (in fn00001A4F)
;;     00001B34 (in fn00001A4F)
;;     00001C02 (in fn00001BA2)
;;     00001C7E (in fn00001BA2)
;;     00001D05 (in fn00001C95)
fn00001772 proc
	enter	[],0
	movd	H'115E0,tos
	jsr	fn00003978
	adjspb	H'FC
	movd	8(fp),tos
	jsr	fn000055C4
	adjspb	H'FC
	exit	[]
	ret	0

;; fn00001794: 00001794
;;   Called from:
;;     000011AA (in fn00000FF5)
fn00001794 proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movd	12(fp),r6
	br	000017BB

l0000179F:
	movd	r7,r0
	addqd	1,r7
	movd	r6,r1
	addqd	1,r6
	cmpb	0(r0),0(r1)
	beq	000017BB

l000017AD:
	movqd	0,r0
	br	000017B7

l000017B1:
	cmpb	0(r6),H'2F
	seqd	r0

l000017B7:
	exit	[r6,r7]
	ret	0

l000017BB:
	movxbd	0(r7),r0
	cmpqd	0,r0
	bne	0000179F

l000017C3:
	movxbd	0(r6),r0
	cmpqd	0,r0
	bne	000017B1

l000017CB:
	movqd	1,r0
	br	000017B7

;; fn000017CF: 000017CF
;;   Called from:
;;     000008C2 (in fn0000079D)
fn000017CF proc
	enter	[],H'C
	addr	-12(fp),tos
	jsr	fn00002564
	adjspb	H'FC
	jsr	fn000034FC
	movd	r0,-4(fp)
	movd	-4(fp),r1
	cmpqd	0,r1
	bne	00001863

l000017EF:
	movqd	1,tos
	jsr	fn000050E0
	adjspb	H'FC
	movd	-8(fp),tos
	jsr	fn000022A0
	adjspb	H'FC
	movqd	0,tos
	movd	H'11B06,tos
	movd	H'11AFD,tos
	jsr	fn000034AC
	adjspb	H'F4
	movqd	0,tos
	movd	H'11B17,tos
	movd	H'11B0A,tos
	jsr	fn000034AC
	adjspb	H'F4
	movd	H'11B1B,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movd	H'11B28,tos
	jsr	fn00002580
	adjspb	H'FC
	movqd	1,tos
	jsr	fn000055C4
	adjspb	H'FC

l00001863:
	movqd	0,tos
	jsr	fn00005588
	adjspb	H'FC
	cmpqd	-1,r0
	bne	00001863

l00001872:
	addr	@00000032,tos
	movd	8(fp),tos
	movd	-12(fp),tos
	jsr	fn000038C0
	adjspb	H'F4
	br	00001889

l00001886:
	addqd	1,8(fp)

l00001889:
	cmpb	0(8(fp)),H'A
	bne	00001886

l00001890:
	movqb	0,0(8(fp))
	movd	-12(fp),tos
	jsr	fn000050E0
	adjspb	H'FC
	movd	-8(fp),tos
	jsr	fn000050E0
	adjspb	H'FC
	exit	[]
	ret	0

;; fn000018B0: 000018B0
;;   Called from:
;;     00001717 (in fn00001701)
fn000018B0 proc
	enter	[r7],4
	movqd	0,r7
	movd	8(fp),r0
	movxbd	r0[r7:b],r1
	cmpqd	0,r1
	beq	000018D6

l000018C0:
	movd	8(fp),r0
	cmpb	r0[r7:b],H'20
	beq	000018D6

l000018C9:
	addqd	1,r7
	movd	8(fp),r0
	movxbd	r0[r7:b],r1
	cmpqd	0,r1
	bne	000018C0

l000018D6:
	movd	@00014C64,tos
	movd	@00014C60,tos
	movd	r7,tos
	movd	8(fp),tos
	jsr	fn000018FA
	adjspb	H'F0
	movd	r0,-4(fp)
	movd	-4(fp),r0
	exit	[r7]
	ret	0

;; fn000018FA: 000018FA
;;   Called from:
;;     000018E7 (in fn000018B0)
fn000018FA proc
	enter	[r6,r7],H'D0
	movqd	0,@00014C74
	br	00001917

l00001906:
	movd	-204(fp),20(fp)
	br	00001917

l0000190D:
	cmpqd	0,r7
	bge	000019EA

l00001912:
	movd	-208(fp),16(fp)

l00001917:
	cmpd	16(fp),20(fp)
	bge	0000199B

l0000191E:
	movd	20(fp),r0
	subd	16(fp),r0
	quod	2,r0
	addd	16(fp),r0
	addr	-100(r0),-204(fp)
	cmpd	-204(fp),16(fp)
	bge	00001940

l0000193B:
	movd	16(fp),-204(fp)

l00001940:
	movqd	0,tos
	movd	-204(fp),tos
	movd	@00014C68,tos
	jsr	fn0000236C
	adjspb	H'F4
	movd	@00014C68,tos
	addr	@000000C8,tos
	movqd	1,tos
	addr	-200(fp),tos
	jsr	fn000022B4
	adjspb	H'F0
	addqd	1,@00014C74
	movqd	0,r7
	cmpd	r7,H'C8
	bge	00001994

l0000197E:
	cmpb	-200(fp)[r7:b],H'A
	beq	00001994

l00001986:
	addqd	1,-204(fp)
	addqd	1,r7
	cmpd	r7,H'C8
	blt	0000197E

l00001994:
	cmpd	-204(fp),20(fp)
	blt	000019A1

l0000199B:
	movqd	-1,r0

l0000199D:
	exit	[r6,r7]
	ret	0

l000019A1:
	movd	-204(fp),-208(fp)
	movd	r7,r6
	addqd	1,r7
	cmpd	r7,H'C8
	bge	000019C9

l000019B3:
	addqd	1,-208(fp)
	cmpb	-200(fp)[r7:b],H'A
	beq	000019C9

l000019BF:
	addqd	1,r7
	cmpd	r7,H'C8
	blt	000019B3

l000019C9:
	movd	12(fp),tos
	movd	8(fp),tos
	addr	-200(fp),r0
	addd	r6,r0
	movd	r0,tos
	jsr	fn000019F1
	adjspb	H'F4
	movd	r0,r7
	cmpqd	0,r7
	ble	0000190D

l000019E7:
	br	00001906

l000019EA:
	movd	-204(fp),r0
	br	0000199D

;; fn000019F1: 000019F1
;;   Called from:
;;     000019D7 (in fn000018FA)
fn000019F1 proc
	enter	[r7],0
	cmpb	0(8(fp)),H'A
	beq	00001A06

l000019FB:
	movqd	2,tos
	jsr	fn000055C4
	adjspb	H'FC

l00001A06:
	movqd	0,r7
	cmpd	r7,16(fp)
	bge	00001A3E

l00001A0D:
	movd	8(fp),r0
	movd	r7,r1
	addqd	1,r1
	movd	12(fp),r2
	cmpb	r0[r1:b],r2[r7:b]
	bgt	00001A4B

l00001A1D:
	movd	8(fp),r0
	movd	r7,r1
	addqd	1,r1
	movd	12(fp),r2
	cmpb	r0[r1:b],r2[r7:b]
	bge	00001A37

l00001A2D:
	movqd	1,r0
	br	00001A33

l00001A31:
	movqd	0,r0

l00001A33:
	exit	[r7]
	ret	0

l00001A37:
	addqd	1,r7
	cmpd	r7,16(fp)
	blt	00001A0D

l00001A3E:
	movd	8(fp),r0
	movd	r7,r1
	addqd	1,r1
	cmpb	r0[r1:b],H'20
	beq	00001A31

l00001A4B:
	movqd	-1,r0
	br	00001A33

;; fn00001A4F: 00001A4F
;;   Called from:
;;     000009C6 (in fn000009BD)
;;     00000B14 (in fn00000AE7)
;;     000010B5 (in fn00000FF5)
fn00001A4F proc
	enter	[],8
	cmpd	@00014C54,@000115DC
	bge	00001A67

l00001A5E:
	cmpqd	0,@00014C58
	bne	00001B70

l00001A67:
	cmpqd	0,@00014C58
	bne	00001A7D

l00001A6F:
	cmpqd	0,@000115DC
	bne	00001A7D

l00001A77:
	addr	@00000014,-8(fp)
	br	00001A84

l00001A7D:
	movd	@000115DC,-8(fp)

l00001A84:
	movd	-8(fp),r0
	ashd	9,r0
	movd	r0,tos
	movd	H'12404,tos
	movd	@00014C38,tos
	jsr	fn000038C0
	adjspb	H'F4
	movd	r0,-4(fp)
	movd	-4(fp),r1
	cmpqd	0,r1
	ble	00001ACC

l00001AAC:
	movd	H'11B2B,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	3,tos
	jsr	fn00001772
	adjspb	H'FC

l00001ACC:
	cmpqd	0,@00014C58
	bne	fn00001B6A

l00001AD5:
	movd	-4(fp),r0
	remd	H'200,r0
	cmpqd	0,r0
	beq	00001B03

l00001AE3:
	movd	H'11B41,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	3,tos
	jsr	fn00001772
	adjspb	H'FC

l00001B03:
	movd	-4(fp),r0
	quod	H'200,r0
	movd	r0,-4(fp)
	cmpqd	0,@00014C28
	beq	00001B3D

l00001B18:
	cmpqd	1,-4(fp)
	beq	00001B3D

l00001B1D:
	movd	H'11B5C,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	4,tos
	jsr	fn00001772
	adjspb	H'FC

l00001B3D:
	cmpd	-4(fp),@000115DC
	beq	fn00001B6A

l00001B46:
	cmpqd	1,-4(fp)
	beq	fn00001B6A

l00001B4B:
	movd	-4(fp),tos
	movd	H'11B84,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F4

l00001B61:
	Nyi
	tbitb	92061697(r2),r2[r5:q]

;; fn00001B63: 00001B63
;;   Called from:
;;     00001B60 (in fn00001A4F)
;;     00001B62 (in fn00003514)
fn00001B63 proc
	movd	-4(fp),@000115DC

;; fn00001B6A: 00001B6A
;;   Called from:
;;     00001AD2 (in fn00001A4F)
;;     00001B44 (in fn00001A4F)
;;     00001B49 (in fn00001A4F)
;;     00001B63 (in fn00001B63)
;;     00001B63 (in fn00001B63)
fn00001B6A proc
	movqd	0,@00014C54

l00001B70:
	movqd	1,@00014C58
	movd	@00014C54,r0
	addqd	1,@00014C54
	ashd	9,r0
	addd	H'12404,r0
	movd	r0,tos
	movd	8(fp),tos
	jsr	fn00001D50
	adjspb	H'F8
	addr	@00000200,r0
	exit	[]
	ret	0

;; fn00001BA2: 00001BA2
;;   Called from:
;;     00000D3B (in fn00000B29)
;;     00000E7D (in fn00000B29)
;;     00000E8C (in fn00000B29)
;;     0000131F (in fn000012F4)
fn00001BA2 proc
	enter	[],0
	movqd	1,@00014C58
	cmpqd	0,@000115DC
	bne	00001BB9

l00001BB3:
	movqd	1,@000115DC

l00001BB9:
	cmpd	@00014C54,@000115DC
	blt	00001C11

l00001BC6:
	movd	@000115DC,r0
	ashd	9,r0
	movd	r0,tos
	movd	H'12404,tos
	movd	@00014C38,tos
	jsr	fn000055A4
	adjspb	H'F4
	cmpqd	0,r0
	ble	00001C0B

l00001BEB:
	movd	H'11B99,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	2,tos
	jsr	fn00001772
	adjspb	H'FC

l00001C0B:
	movqd	0,@00014C54

l00001C11:
	movd	8(fp),tos
	movd	@00014C54,r0
	addqd	1,@00014C54
	ashd	9,r0
	addd	H'12404,r0
	movd	r0,tos
	jsr	fn00001D50
	adjspb	H'F8
	cmpd	@00014C54,@000115DC
	blt	00001C8D

l00001C42:
	movd	@000115DC,r0
	ashd	9,r0
	movd	r0,tos
	movd	H'12404,tos
	movd	@00014C38,tos
	jsr	fn000055A4
	adjspb	H'F4
	cmpqd	0,r0
	ble	00001C87

l00001C67:
	movd	H'11BB0,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	2,tos
	jsr	fn00001772
	adjspb	H'FC

l00001C87:
	movqd	0,@00014C54

l00001C8D:
	addr	@00000200,r0
	exit	[]
	ret	0

;; fn00001C95: 00001C95
;;   Called from:
;;     000009AD (in fn0000099F)
fn00001C95 proc
	enter	[],0
	movqd	1,tos
	movxwd	H'FE00,tos
	movd	@00014C38,tos
	jsr	fn00003898
	adjspb	H'F4
	cmpd	@00014C54,@000115DC
	blt	00001D24

l00001CBB:
	movd	@000115DC,r0
	addqd	-1,r0
	movd	r0,@00014C54
	movd	@000115DC,r0
	ashd	9,r0
	movd	r0,tos
	movd	H'12404,tos
	movd	@00014C38,tos
	jsr	fn000038C0
	adjspb	H'F4
	cmpqd	0,r0
	ble	00001D0E

l00001CEE:
	movd	H'11BC7,tos
	movd	H'1206C,tos
	jsr	fn000025E4
	adjspb	H'F8
	movqd	4,tos
	jsr	fn00001772
	adjspb	H'FC

l00001D0E:
	movqd	1,tos
	movxwd	H'FE00,tos
	movd	@00014C38,tos
	jsr	fn00003898
	adjspb	H'F4

l00001D24:
	exit	[]
	ret	0

;; fn00001D28: 00001D28
;;   Called from:
;;     00000944 (in fn0000079D)
fn00001D28 proc
	enter	[],0
	movd	@000115DC,r0
	ashd	9,r0
	movd	r0,tos
	movd	H'12404,tos
	movd	@00014C38,tos
	jsr	fn000055A4
	adjspb	H'F4
	exit	[]
	ret	0

;; fn00001D50: 00001D50
;;   Called from:
;;     00001B91 (in fn00001B6A)
;;     00001B91 (in fn00001A4F)
;;     00001B91 (in fn00001B6A)
;;     00001C2C (in fn00001BA2)
fn00001D50 proc
	enter	[r5,r6,r7],0
	movd	8(fp),r7
	movd	12(fp),r6
	addr	@00000200,r5

l00001D5D:
	movb	0(r6),0(r7)
	addqd	1,r6
	addqd	1,r7
	acbd	-1,r5,00001D5D

l00001D68:
	exit	[r5,r6,r7]
	ret	0

;; fn00001D6C: 00001D6C
;;   Called from:
;;     000008FA (in fn0000079D)
;;     00000920 (in fn0000079D)
;;     00000BF3 (in fn00000B29)
;;     00000FE9 (in fn00000B29)
fn00001D6C proc
	addr	@0000000C,r0
	addr	4(sp),r1
	svc
	bfc	00001D7B

l00001D75:
	jump	@000055B8

l00001D7B:
	movqd	0,r0
	ret	0
00001D7F                                              F2                .

;; fn00001D80: 00001D80
;;   Called from:
;;     00001045 (in fn00000FF5)
;;     000014AD (in fn000013F9)
fn00001D80 proc
	addr	@00000010,r0
	addr	4(sp),r1
	svc
	bfc	00001D8F

l00001D89:
	jump	@000055B8

l00001D8F:
	movqd	0,r0
	ret	0
00001D93          F2                                        .            

;; fn00001D94: 00001D94
;;   Called from:
;;     00001369 (in fn0000132C)
fn00001D94 proc
	enter	[],0
	movd	8(fp),tos
	jsr	fn00001DB2
	adjspb	H'FC
	movd	r0,tos
	jsr	fn0000204C
	adjspb	H'FC
	exit	[]
	ret	0

;; fn00001DB2: 00001DB2
;;   Called from:
;;     00001D9A (in fn00001D94)
fn00001DB2 proc
	enter	[r4,r5,r6,r7],4
	jsr	fn000021C1
	movd	0(8(fp)),r0
	subd	@00011BE8,r0
	movd	r0,-4(fp)
	addr	-4(fp),tos
	jsr	fn00001EC8
	adjspb	H'FC
	movd	r0,r4
	movd	28(r4),r7
	addr	@00000077,r6
	addr	@0000012F,r5
	cmpd	20(r4),H'4A
	beq	00001DF3

l00001DEA:
	cmpd	20(r4),H'4B
	bne	00001E1F

l00001DF3:
	movd	20(r4),r0
	addr	-74(r0),r0
	ashd	3,r0
	movd	H'11C44,r1
	addd	r0,r1
	movd	0(r1),r6
	movd	20(r4),r0
	addr	-74(r0),r0
	ashd	3,r0
	movd	H'11C48,r1
	addd	r0,r1
	movd	0(r1),r5

l00001E1F:
	movd	r6,tos
	movd	r4,tos
	jsr	fn00001E7C
	adjspb	H'F8
	movd	r0,r6
	movd	r5,tos
	movd	r4,tos
	jsr	fn00001E7C
	adjspb	H'F8
	movd	r0,r5
	cmpqd	0,@00011BEC
	beq	00001E76

l00001E45:
	cmpd	r7,r6
	bgt	00001E52

l00001E49:
	cmpd	r7,r6
	bne	00001E76

l00001E4D:
	cmpqd	2,8(r4)
	bgt	00001E76

l00001E52:
	cmpd	r7,r5
	blt	00001E5F

l00001E56:
	cmpd	r7,r5
	bne	00001E76

l00001E5A:
	cmpqd	1,8(r4)
	ble	00001E76

l00001E5F:
	addr	3600(-4(fp)),-4(fp)
	addr	-4(fp),tos
	jsr	fn00001EC8
	adjspb	H'FC
	movd	r0,r4
	addqd	1,32(r4)

l00001E76:
	movd	r4,r0
	exit	[r4,r5,r6,r7]
	ret	0

;; fn00001E7C: 00001E7C
;;   Called from:
;;     00001E23 (in fn00001DB2)
;;     00001E32 (in fn00001DB2)
fn00001E7C proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movd	12(fp),r6
	cmpd	r6,H'3A
	blt	00001EAB

l00001E8D:
	movd	20(r7),r0
	remd	4,r0
	cmpqd	0,r0
	beq	00001EA1

l00001E9B:
	addr	@0000016D,r0
	br	00001EA5

l00001EA1:
	addr	@0000016E,r0

l00001EA5:
	addr	-365(r0),r0
	addd	r0,r6

l00001EAB:
	movd	r6,r0
	subd	28(r7),r0
	addd	24(r7),r0
	addr	700(r0),r0
	remd	7,r0
	movd	r6,r1
	subd	r0,r1
	movd	r1,r0
	exit	[r6,r7]
	ret	0

;; fn00001EC8: 00001EC8
;;   Called from:
;;     00001DCB (in fn00001DB2)
;;     00001E68 (in fn00001DB2)
fn00001EC8 proc
	enter	[r6,r7],8
	movd	0(8(fp)),r0
	remd	H'15180,r0
	movd	r0,-4(fp)
	movd	0(8(fp)),r0
	quod	H'15180,r0
	movd	r0,-8(fp)
	cmpqd	0,-4(fp)
	ble	00001EF6

l00001EEC:
	addd	H'15180,-4(fp)
	addqd	-1,-8(fp)

l00001EF6:
	movd	-4(fp),r0
	remd	H'3C,r0
	movd	r0,@00011C54
	movd	-4(fp),r0
	quod	H'3C,r0
	movd	r0,r6
	movd	r6,r0
	remd	H'3C,r0
	movd	r0,@00011C58
	quod	H'3C,r6
	movd	r6,@00011C5C
	movd	-8(fp),r0
	addd	H'700004,r0
	remd	7,r0
	movd	r0,@00011C6C
	cmpqd	0,-8(fp)
	bgt	00001FA6

l00001F4A:
	addr	@00000046,r6
	movd	r6,r0
	remd	4,r0
	cmpqd	0,r0
	beq	00001F61

l00001F5B:
	addr	@0000016D,r0
	br	00001F65

l00001F61:
	addr	@0000016E,r0

l00001F65:
	cmpd	-8(fp),r0
	blt	00001FD3

l00001F6B:
	movd	r6,r0
	remd	4,r0
	cmpqd	0,r0
	beq	00001F80

l00001F78:
	addr	-365(-8(fp)),-8(fp)
	br	00001F86

l00001F80:
	addr	-366(-8(fp)),-8(fp)

l00001F86:
	addqd	1,r6
	movd	r6,r0
	remd	4,r0
	cmpqd	0,r0
	beq	00001F9B

l00001F95:
	addr	@0000016D,r0
	br	00001F9F

l00001F9B:
	addr	@0000016E,r0

l00001F9F:
	cmpd	-8(fp),r0
	bge	00001F6B

l00001FA4:
	br	00001FD3

l00001FA6:
	addr	@00000046,r6
	cmpqd	0,-8(fp)
	ble	00001FD3

l00001FAF:
	movd	r6,r0
	addqd	-1,r0
	remd	4,r0
	cmpqd	0,r0
	beq	00001FC6

l00001FBE:
	addr	365(-8(fp)),-8(fp)
	br	00001FCC

l00001FC6:
	addr	366(-8(fp)),-8(fp)

l00001FCC:
	addqd	-1,r6
	cmpqd	0,-8(fp)
	bgt	00001FAF

l00001FD3:
	movd	r6,@00011C68
	movd	-8(fp),r7
	movd	r7,@00011C70
	movd	r6,r0
	remd	4,r0
	cmpqd	0,r0
	beq	00001FF5

l00001FEF:
	addr	@0000016D,r0
	br	00001FF9

l00001FF5:
	addr	@0000016E,r0

l00001FF9:
	cmpd	r0,H'16E
	bne	00002008

l00002001:
	addr	@0000001D,@00011C18

l00002008:
	movqd	0,r6
	cmpd	r7,@00011C14[r6:d]
	blt	00002025

l00002013:
	subd	@00011C14[r6:d],r7
	addqd	1,r6
	cmpd	r7,@00011C14[r6:d]
	bge	00002013

l00002025:
	addr	@0000001C,@00011C18
	movd	r7,r0
	addqd	1,r0
	movd	r0,@00011C60
	movd	r6,@00011C64
	movqd	0,@00011C74
	movd	H'11C54,r0
	exit	[r6,r7]
	ret	0

;; fn0000204C: 0000204C
;;   Called from:
;;     00001DA5 (in fn00001D94)
fn0000204C proc
	enter	[r5,r6,r7],0
	movd	H'11BF8,r7
	movd	H'11C80,r6
	movd	r6,r0
	addqd	1,r6
	movd	r7,r1
	addqd	1,r7
	movb	0(r0),0(r1)
	movxbd	0(r1),r2
	cmpqd	0,r2
	beq	00002083

l0000206F:
	movd	r6,r0
	addqd	1,r6
	movd	r7,r1
	addqd	1,r7
	movb	0(r0),0(r1)
	movxbd	0(r1),r2
	cmpqd	0,r2
	bne	0000206F

l00002083:
	movd	24(8(fp)),r0
	muld	3,r0
	addd	H'11C9A,r0
	movd	r0,r6
	movd	H'11BF8,r7
	movb	0(r6),0(r7)
	addqd	1,r6
	addqd	1,r7
	movb	0(r6),0(r7)
	addqd	1,r6
	addqd	1,r7
	movb	0(r6),0(r7)
	addqd	1,r6
	addqd	1,r7
	addqd	1,r7
	addr	16(8(fp)),r5
	movd	0(r5),r0
	muld	3,r0
	addd	H'11CB0,r0
	movd	r0,r6
	movb	0(r6),0(r7)
	addqd	1,r6
	addqd	1,r7
	movb	0(r6),0(r7)
	addqd	1,r6
	addqd	1,r7
	movb	0(r6),0(r7)
	addqd	1,r6
	addqd	1,r7
	addqd	-4,r5
	movd	r5,r0
	movd	0(r0),tos
	movd	r7,tos
	jsr	fn00002173
	adjspb	H'F8
	movd	r0,r7
	addqd	-4,r5
	movd	r5,r0
	movd	0(r0),r1
	addr	100(r1),tos
	movd	r7,tos
	jsr	fn00002173
	adjspb	H'F8
	movd	r0,r7
	addqd	-4,r5
	movd	r5,r0
	movd	0(r0),r1
	addr	100(r1),tos
	movd	r7,tos
	jsr	fn00002173
	adjspb	H'F8
	movd	r0,r7
	addqd	-4,r5
	movd	r5,r0
	movd	0(r0),r1
	addr	100(r1),tos
	movd	r7,tos
	jsr	fn00002173
	adjspb	H'F8
	movd	r0,r7
	cmpd	20(8(fp)),H'64
	blt	00002152

l0000214A:
	movb	H'32,1(r7)
	movb	H'30,2(r7)

l00002152:
	addqd	2,r7
	movd	20(8(fp)),r0
	addr	100(r0),tos
	movd	r7,tos
	jsr	fn00002173
	adjspb	H'F8
	movd	r0,r7
	movd	H'11BF8,r0
	exit	[r5,r6,r7]
	ret	0

;; fn00002173: 00002173
;;   Called from:
;;     000020ED (in fn0000204C)
;;     00002105 (in fn0000204C)
;;     0000211D (in fn0000204C)
;;     00002135 (in fn0000204C)
;;     0000215E (in fn0000204C)
fn00002173 proc
	enter	[r7],0
	movd	8(fp),r7
	addqd	1,r7
	cmpd	12(fp),H'A
	blt	000021BB

l00002184:
	movd	12(fp),r0
	quod	H'A,r0
	remd	H'A,r0
	addr	48(r0),r0
	movxbd	r0,r0
	movb	r0,0(r7)

l0000219E:
	addqd	1,r7
	movd	12(fp),r0
	remd	H'A,r0
	addr	48(r0),r0
	movxbd	r0,r0
	movb	r0,0(r7)
	addqd	1,r7
	movd	r7,r0
	exit	[r7]
	ret	0

l000021BB:
	movb	H'20,0(r7)
	br	0000219E

;; fn000021C1: 000021C1
;;   Called from:
;;     00001DB5 (in fn00001DB2)
fn000021C1 proc
	enter	[r5,r6,r7],4
	movd	H'11CD5,tos
	jsr	fn000035C8
	adjspb	H'FC
	movd	r0,r7
	movd	r7,r1
	cmpqd	0,r1
	beq	00002299

l000021DC:
	movxbd	0(r7),r0
	cmpqd	0,r0
	beq	00002299

l000021E5:
	movqd	3,r5
	movd	@00011BF0,r6

l000021ED:
	movxbd	0(r7),r0
	cmpqd	0,r0
	beq	00002203

l000021F5:
	movd	r7,r0
	addqd	1,r7
	movd	r6,r1
	addqd	1,r6
	movb	0(r0),0(r1)
	br	0000220B

l00002203:
	movd	r6,r0
	addqd	1,r6
	movb	H'20,0(r0)

l0000220B:
	acbd	-1,r5,000021ED

l0000220E:
	cmpb	0(r7),H'2D
	seqd	r0
	movd	r0,-4(fp)
	movd	-4(fp),r1
	cmpqd	0,r1
	beq	00002220

l0000221E:
	addqd	1,r7

l00002220:
	movqd	0,r5
	br	00002238

l00002224:
	movd	r5,r0
	muld	H'A,r0
	movxbd	0(r7),r1
	addd	r0,r1
	addr	-48(r1),r5
	addqd	1,r7

l00002238:
	cmpb	0(r7),H'30
	blt	00002244

l0000223E:
	cmpb	0(r7),H'39
	ble	00002224

l00002244:
	cmpqd	0,-4(fp)
	beq	0000224C

l00002249:
	negd	r5,r5

l0000224C:
	movd	r5,r0
	muld	H'E10,r0
	movd	r0,@00011BE8
	cmpqb	0,0(r7)
	sned	r0
	movd	r0,@00011BEC
	movd	@00011BEC,r1
	cmpqd	0,r1
	beq	00002299

l00002270:
	movd	@00011BF4,r6
	movqd	3,r5

l00002278:
	movxbd	0(r7),r0
	cmpqd	0,r0
	beq	0000228E

l00002280:
	movd	r7,r0
	addqd	1,r7
	movd	r6,r1
	addqd	1,r6
	movb	0(r0),0(r1)
	br	00002296

l0000228E:
	movd	r6,r0
	addqd	1,r6
	movb	H'20,0(r0)

l00002296:
	acbd	-1,r5,00002278

l00002299:
	exit	[r5,r6,r7]
	ret	0
0000229D                                        F2 F2 F2              ...

;; fn000022A0: 000022A0
;;   Called from:
;;     00000531 (in fn0000012C)
;;     00000624 (in fn0000012C)
;;     000006CF (in fn0000012C)
;;     000017FD (in fn000017CF)
fn000022A0 proc
	addr	@00000029,r0
	addr	4(sp),r1
	svc
	bfc	000022AF

l000022A9:
	jump	@000055B8

l000022AF:
	ret	0
000022B1    F2 F2 F2                                      ...            

;; fn000022B4: 000022B4
;;   Called from:
;;     00001965 (in fn000018FA)
fn000022B4 proc
	enter	[r5,r6,r7],4
	movd	20(fp),r7
	cmpqd	0,12(fp)
	bge	000022C4

l000022BF:
	cmpqd	0,16(fp)
	blt	000022EA

l000022C4:
	movqd	0,r0
	br	000022E6

l000022C8:
	movd	12(fp),tos
	movd	r6,r0
	addd	12(fp),r0
	addqd	-1,r0
	movd	r0,tos
	jsr	fn0000556C
	adjspb	H'F8
	movd	r0,-4(fp)
	movd	16(fp),r0
	subd	-4(fp),r0

l000022E6:
	exit	[r5,r6,r7]
	ret	0

l000022EA:
	movd	16(fp),r0
	muld	12(fp),r0
	movd	r0,r6

l000022F3:
	cmpqd	0,0(r7)
	blt	0000230D

l000022F8:
	movd	r7,tos
	jsr	fn000030D0
	adjspb	H'FC
	cmpqd	-1,r0
	beq	000022C8

l00002307:
	addqd	-1,4(r7)
	addqd	1,0(r7)

l0000230D:
	cmpd	r6,0(r7)
	bhs	00002316

l00002312:
	movd	r6,r5
	br	00002319

l00002316:
	movd	0(r7),r5

l00002319:
	movd	r5,tos
	movd	4(r7),tos
	movd	8(fp),tos
	jsr	fn000054D4
	adjspb	H'F4
	addd	r5,r0
	movd	r0,8(fp)
	subd	r5,0(r7)
	addd	r5,4(r7)
	movxbd	13(r7),r0
	movd	@00012190[r0:d],r1
	subd	4(r7),r1
	cmpqd	0,0(r7)
	ble	0000234C

l00002348:
	movqd	0,r0
	br	0000234F

l0000234C:
	movd	0(r7),r0

l0000234F:
	cmpd	r1,r0
	bge	0000235E

l00002353:
	movd	r7,tos
	jsr	fn000050AA
	adjspb	H'FC

l0000235E:
	subd	r5,r6
	cmpqd	0,r6
	bne	000022F3

l00002365:
	movd	16(fp),r0
	br	000022E6
0000236B                                  F2                        .    

;; fn0000236C: 0000236C
;;   Called from:
;;     00001738 (in fn00001701)
;;     0000194C (in fn000018FA)
fn0000236C proc
	enter	[r6,r7],8
	movd	8(fp),r7
	cbitb	4,12(r7)
	movxbd	12(r7),r0
	tbitd	0,r0
	bfc	00002426

l00002384:
	cmpqd	2,16(fp)
	ble	000023F5

l0000238A:
	cmpqd	0,8(r7)
	beq	000023F5

l00002390:
	movxbd	12(r7),r0
	tbitd	2,r0
	bfs	000023F5

l0000239D:
	movd	0(r7),r6
	movd	12(fp),-4(fp)
	cmpqd	0,16(fp)
	bne	000023C7

l000023A9:
	movqd	1,tos
	movqd	0,tos
	movxbd	13(r7),tos
	jsr	fn00003898
	adjspb	H'F4
	movd	r0,-8(fp)
	movd	r6,r0
	subd	-8(fp),r0
	addd	r0,-4(fp)
	br	000023CA

l000023C7:
	subd	r6,12(fp)

l000023CA:
	movxbd	12(r7),r0
	tbitd	7,r0
	bfs	000023F5

l000023D6:
	cmpqd	0,r6
	bge	000023F5

l000023DA:
	cmpd	-4(fp),r6
	bgt	000023F5

l000023DF:
	movd	8(r7),r0
	subd	4(r7),r0
	cmpd	-4(fp),r0
	blt	000023F5

l000023EA:
	addd	-4(fp),4(r7)
	subd	-4(fp),0(r7)
	br	00002478

l000023F5:
	movxbd	12(r7),r0
	tbitd	7,r0
	bfc	0000240A

l00002401:
	movd	8(r7),4(r7)
	cbitb	0,12(r7)

l0000240A:
	movd	16(fp),tos
	movd	12(fp),tos
	movxbd	13(r7),tos
	jsr	fn00003898
	adjspb	H'F4
	movd	r0,-4(fp)
	movqd	0,0(r7)
	br	0000246D

l00002426:
	movxbd	12(r7),r0
	andd	H'82,r0
	cmpqd	0,r0
	beq	0000246D

l00002434:
	movd	r7,tos
	jsr	fn00004D67
	adjspb	H'FC
	movqd	0,0(r7)
	movxbd	12(r7),r0
	tbitd	7,r0
	bfc	00002457

l0000244E:
	cbitb	1,12(r7)
	movd	8(r7),4(r7)

l00002457:
	movd	16(fp),tos
	movd	12(fp),tos
	movxbd	13(r7),tos
	jsr	fn00003898
	adjspb	H'F4
	movd	r0,-4(fp)

l0000246D:
	cmpqd	-1,-4(fp)
	bne	00002478

l00002472:
	movqd	-1,r0

l00002474:
	exit	[r6,r7]
	ret	0

l00002478:
	movqd	0,r0
	br	00002474

;; fn0000247C: 0000247C
;;   Called from:
;;     00000269 (in fn0000012C)
fn0000247C proc
	enter	[r4,r5,r6,r7],0
	movd	8(fp),r7
	movqd	0,r4
	movxbd	0(r7),r5
	movd	r5,r0
	movxbd	73133(r0),r0
	tbitd	2,r0
	bfs	000024E8

l0000249A:
	br	000024A4

l0000249C:
	addqd	1,r7
	movd	r7,r0
	movxbd	0(r0),r5

l000024A4:
	movxbd	73133(r5),r0
	tbitd	3,r0
	bfs	0000249C

l000024B3:
	movd	r5,r0
	cmpd	r0,H'2B
	beq	000024C7

l000024BD:
	cmpd	r0,H'2D
	bne	000024CF

l000024C5:
	addqd	1,r4

l000024C7:
	addqd	1,r7
	movd	r7,r0
	movxbd	0(r0),r5

l000024CF:
	movxbd	73133(r5),r0
	tbitd	2,r0
	bfs	000024E8

l000024DE:
	movqd	0,r0
	br	000024E4

l000024E2:
	movd	r6,r0

l000024E4:
	exit	[r4,r5,r6,r7]
	ret	0

l000024E8:
	addr	@00000030,r0
	subd	r5,r0
	movd	r0,r6
	addqd	1,r7
	movd	r7,r0
	movxbd	0(r0),r5
	movd	r5,r1
	movxbd	73133(r1),r0
	tbitd	2,r0
	bfc	0000252F

l00002508:
	muld	H'A,r6
	addr	@00000030,r0
	subd	r5,r0
	addd	r0,r6
	addqd	1,r7
	movd	r7,r0
	movxbd	0(r0),r5
	movd	r5,r1
	movxbd	73133(r1),r0
	tbitd	2,r0
	bfs	00002508

l0000252F:
	cmpqd	0,r4
	bne	000024E2

l00002534:
	negd	r6,r0
	br	000024E4
0000253A                               F2 F2                       ..    

;; fn0000253C: 0000253C
;;   Called from:
;;     00001204 (in fn00000FF5)
fn0000253C proc
	addr	@00000009,r0
	addr	4(sp),r1
	svc
	bfc	0000254B

l00002545:
	jump	@000055B8

l0000254B:
	movqd	0,r0
	ret	0
0000254F                                              F2                .

;; fn00002550: 00002550
;;   Called from:
;;     0000059F (in fn0000012C)
;;     0000125D (in fn00000FF5)
fn00002550 proc
	addr	@00000008,r0
	addr	4(sp),r1
	svc
	bfc	0000255F

l00002559:
	jump	@000055B8

l0000255F:
	ret	0
00002561    F2 F2 F2                                      ...            

;; fn00002564: 00002564
;;   Called from:
;;     000017D5 (in fn000017CF)
fn00002564 proc
	addr	@0000002A,r0
	addr	4(sp),r1
	svc
	bfc	00002573

l0000256D:
	jump	@000055B8

l00002573:
	movd	4(sp),r2
	movd	r0,0(r2)
	movd	r1,4(r2)
	movqd	0,r0
	ret	0

;; fn00002580: 00002580
;;   Called from:
;;     000012A3 (in fn0000127A)
;;     000012C1 (in fn0000127A)
;;     000012D0 (in fn0000127A)
;;     0000134B (in fn0000132C)
;;     0000135D (in fn0000132C)
;;     00001383 (in fn0000132C)
;;     000013EC (in fn000013C2)
;;     00001634 (in fn0000161F)
;;     0000165D (in fn0000161F)
;;     0000184F (in fn000017CF)
fn00002580 proc
	enter	[r7],4
	addr	12(fp),-4(fp)
	movxbd	@00012068,r0
	sbitd	1,r0
	cmpqd	0,r0
	bne	000025B3

l00002599:
	movxbd	@00012068,r0
	sbitd	7,r0
	cmpqd	0,r0
	beq	000025D9

l000025AB:
	sbitb	1,@00012068

l000025B3:
	movd	H'1205C,tos
	movd	-4(fp),tos
	movd	8(fp),tos
	jsr	fn00003A45
	adjspb	H'F4
	movd	r0,r7
	movxbd	@00012068,r0
	tbitd	5,r0
	bfc	000025DF

l000025D9:
	movqd	-1,r0

l000025DB:
	exit	[r7]
	ret	0

l000025DF:
	movd	r7,r0
	br	000025DB
000025E3          F2                                        .            

;; fn000025E4: 000025E4
;;   Called from:
;;     00000297 (in fn0000012C)
;;     00000386 (in fn0000012C)
;;     000003A6 (in fn0000012C)
;;     000003D3 (in fn0000012C)
;;     0000043E (in fn0000012C)
;;     0000051B (in fn0000012C)
;;     000005CA (in fn0000012C)
;;     00000691 (in fn0000012C)
;;     00000759 (in fn0000012C)
;;     00000785 (in fn00000776)
;;     0000097C (in fn0000079D)
;;     00000AA6 (in fn000009BD)
;;     00000ADA (in fn000009BD)
;;     00000C23 (in fn00000B29)
;;     00000CA5 (in fn00000B29)
;;     00000D5B (in fn00000B29)
;;     00000D75 (in fn00000B29)
;;     00000DB9 (in fn00000B29)
;;     00000E39 (in fn00000B29)
;;     00000E51 (in fn00000B29)
;;     00000EE3 (in fn00000B29)
;;     0000101D (in fn00000FF5)
;;     00001087 (in fn00000FF5)
;;     000010A5 (in fn00000FF5)
;;     00001119 (in fn00000FF5)
;;     00001233 (in fn00000FF5)
;;     00001471 (in fn000013F9)
;;     00001840 (in fn000017CF)
;;     00001AB8 (in fn00001A4F)
;;     00001AEF (in fn00001A4F)
;;     00001B29 (in fn00001A4F)
;;     00001B5A (in fn00001A4F)
;;     00001BF7 (in fn00001BA2)
;;     00001C73 (in fn00001BA2)
;;     00001CFA (in fn00001C95)
fn000025E4 proc
	enter	[r7],4
	addr	16(fp),-4(fp)
	movxbd	12(8(fp)),r0
	sbitd	1,r0
	cmpqd	0,r0
	bne	00002611

l000025FB:
	movxbd	12(8(fp)),r0
	sbitd	7,r0
	cmpqd	0,r0
	beq	00002632

l0000260B:
	sbitb	1,12(8(fp))

l00002611:
	movd	8(fp),tos
	movd	-4(fp),tos
	movd	12(fp),tos
	jsr	fn00003A45
	adjspb	H'F4
	movd	r0,r7
	movxbd	12(8(fp)),r0
	tbitd	5,r0
	bfc	00002638

l00002632:
	movqd	-1,r0

l00002634:
	exit	[r7]
	ret	0

l00002638:
	movd	r7,r0
	br	00002634

;; fn0000263C: 0000263C
;;   Called from:
;;     0000170B (in fn00001701)
fn0000263C proc
	enter	[r7],0
	movd	8(fp),r7
	movd	r7,tos
	jsr	fn00004D67
	adjspb	H'FC
	movqd	0,tos
	movqd	0,tos
	movxbd	13(r7),tos
	jsr	fn00003898
	adjspb	H'F4
	movqd	0,0(r7)
	movd	8(r7),4(r7)
	andb	H'CF,12(r7)
	movxbd	12(r7),r0
	andd	H'80,r0
	cmpqd	0,r0
	beq	0000267B

l00002677:
	andb	H'FC,12(r7)

l0000267B:
	exit	[r7]
	ret	0
0000267F                                              F2                .
00002680 82 00 04 27 C6 0C 7C D7 C5 7C D7 C5 08 D7 A5 00 ...'..|..|......
00002690 01 20 4C 7F AE C0 00 26 FC 7C A5 F4 92 00 12 00 . L....&.|......

;; fn000026A0: 000026A0
;;   Called from:
;;     00001755 (in fn00001701)
fn000026A0 proc
	enter	[],4
	addr	16(fp),-4(fp)
	movd	-4(fp),tos
	movd	12(fp),tos
	movd	8(fp),tos
	jsr	fn000026FC
	adjspb	H'F4
	exit	[]
	ret	0

;; fn000026BD: 000026BD
;;   Called from:
;;     000009F0 (in fn000009BD)
;;     00000A0F (in fn000009BD)
;;     00000A2E (in fn000009BD)
;;     00000A4D (in fn000009BD)
;;     00000A65 (in fn000009BD)
;;     00000A80 (in fn000009BD)
fn000026BD proc
	enter	[r7],H'14
	movd	8(fp),r7
	addr	16(fp),-4(fp)
	movqb	1,-8(fp)
	movd	r7,-12(fp)
	movd	-12(fp),-16(fp)
	movd	r7,tos
	jsr	fn0000554C
	adjspb	H'FC
	movd	r0,-20(fp)
	movb	H'14,-7(fp)
	movd	-4(fp),tos
	movd	12(fp),tos
	addr	-20(fp),tos
	jsr	fn000026FC
	adjspb	H'F4
	exit	[r7]
	ret	0
000026F9                            F2 F2 F2                      ...    

;; fn000026FC: 000026FC
;;   Called from:
;;     000026B0 (in fn000026A0)
;;     000026EC (in fn000026BD)
fn000026FC proc
	enter	[r5,r6,r7],H'114
	movd	8(fp),r7
	movd	12(fp),r6
	movqd	0,-260(fp)
	br	00002923

l0000270D:
	movd	r7,tos
	jsr	fn000030D0
	adjspb	H'FC
	movd	r0,-268(fp)
	br	0000277A

l0000271F:
	addqd	-1,0(r7)
	cmpqd	0,0(r7)
	ble	00002738

l00002727:
	movd	r7,tos
	jsr	fn000030D0
	adjspb	H'FC
	movd	r0,-268(fp)
	br	00002744

l00002738:
	movd	4(r7),r1
	addqd	1,4(r7)
	movzbd	0(r1),-268(fp)

l00002744:
	movd	-268(fp),r1
	movxbd	73133(r1),r0
	tbitd	3,r0
	bfs	0000271F

l00002757:
	movd	r7,tos
	movd	-268(fp),tos
	jsr	fn00003930
	adjspb	H'F8
	cmpqd	-1,r0
	beq	00002968

l0000276B:
	br	00002923

l0000276E:
	movd	4(r7),r1
	addqd	1,4(r7)
	movzbd	0(r1),-268(fp)

l0000277A:
	movd	-268(fp),r1
	cmpd	r1,r5
	beq	00002923

l00002783:
	movd	r7,tos
	movd	-268(fp),tos
	jsr	fn00003930
	adjspb	H'F8
	cmpqd	-1,r0
	beq	00002968

l00002797:
	br	0000296E

l0000279A:
	cmpd	r5,H'2A
	bne	000027AE

l000027A2:
	movqd	0,-272(fp)
	movzbd	0(r6),r5
	addqd	1,r6
	br	000027B2

l000027AE:
	movqd	1,-272(fp)

l000027B2:
	movqd	0,-264(fp)
	movxbd	73133(r5),r0
	tbitd	2,r0
	bfc	000027EC

l000027C5:
	movd	-264(fp),r0
	muld	H'A,r0
	addd	r5,r0
	addr	-48(r0),-264(fp)
	movzbd	0(r6),r5
	addqd	1,r6
	movxbd	73133(r5),r0
	tbitd	2,r0
	bfs	000027C5

l000027EC:
	cmpqd	0,-264(fp)
	bne	000027FA

l000027F2:
	movd	H'7FFFFFFF,-264(fp)

l000027FA:
	movd	r5,-276(fp)
	movd	-276(fp),r0
	cmpd	r0,H'6C
	beq	00002814

l0000280A:
	cmpd	-276(fp),H'68
	bne	0000281A

l00002814:
	movzbd	0(r6),r5
	addqd	1,r6

l0000281A:
	cmpqd	0,r5
	beq	00002976

l0000281F:
	cmpd	r5,H'5B
	bne	0000283F

l00002827:
	addr	-256(fp),tos
	movd	r6,tos
	jsr	fn00002D77
	adjspb	H'F8
	movd	r0,r6
	movd	r6,r1
	cmpqd	0,r1
	beq	00002976

l0000283F:
	movxbd	73133(r5),r0
	tbitd	0,r0
	bfc	00002857

l0000284E:
	addr	@0000006C,-276(fp)
	addr	32(r5),r5

l00002857:
	cmpd	r5,H'63
	beq	000028B5

l00002860:
	cmpd	r5,H'5B
	beq	000028B5

l00002869:
	addqd	-1,0(r7)
	cmpqd	0,0(r7)
	ble	00002882

l00002871:
	movd	r7,tos
	jsr	fn000030D0
	adjspb	H'FC
	movd	r0,-268(fp)
	br	0000288E

l00002882:
	movd	4(r7),r1
	addqd	1,4(r7)
	movzbd	0(r1),-268(fp)

l0000288E:
	movd	-268(fp),r1
	movxbd	73133(r1),r0
	tbitd	3,r0
	bfs	00002869

l000028A1:
	movd	r7,tos
	movd	-268(fp),tos
	jsr	fn00003930
	adjspb	H'F8
	cmpqd	-1,r0
	beq	00002968

l000028B5:
	cmpd	r5,H'63
	beq	000028CD

l000028BD:
	cmpd	r5,H'73
	beq	000028CD

l000028C5:
	cmpd	r5,H'5B
	bne	000028E8

l000028CD:
	addr	16(fp),tos
	movd	r7,tos
	addr	-256(fp),tos
	movd	-264(fp),tos
	movd	r5,tos
	movd	-272(fp),tos
	jsr	fn00002C89
	br	00002901

l000028E8:
	addr	16(fp),tos
	movd	r7,tos
	movd	-276(fp),tos
	movd	-264(fp),tos
	movd	r5,tos
	movd	-272(fp),tos
	jsr	fn0000297A

l00002901:
	adjspb	H'E8
	movd	r0,-276(fp)
	movd	-276(fp),r1
	cmpqd	0,r1
	beq	00002916

l00002910:
	addd	-272(fp),-260(fp)

l00002916:
	cmpqd	0,16(fp)
	beq	00002968

l0000291C:
	cmpqd	0,-276(fp)
	beq	0000296E

l00002923:
	movd	r6,r0
	addqd	1,r6
	movzbd	0(r0),r5
	movd	r5,r1
	cmpqd	0,r1
	beq	0000296E

l00002931:
	movxbd	73133(r5),r0
	tbitd	3,r0
	bfs	0000271F

l00002941:
	cmpd	r5,H'25
	bne	0000295C

l00002949:
	movd	r6,r0
	addqd	1,r6
	movzbd	0(r0),r5
	movd	r5,r1
	cmpd	r1,H'25
	bne	0000279A

l0000295C:
	addqd	-1,0(r7)
	cmpqd	0,0(r7)
	ble	0000276E

l00002965:
	br	0000270D

l00002968:
	cmpqd	0,-260(fp)
	beq	00002976

l0000296E:
	movd	-260(fp),r0

l00002972:
	exit	[r5,r6,r7]
	ret	0

l00002976:
	movqd	-1,r0
	br	00002972

;; fn0000297A: 0000297A
;;   Called from:
;;     000028FB (in fn000026FC)
fn0000297A proc
	enter	[r4,r5,r6,r7],H'60
	movd	24(fp),r7
	addr	-64(fp),r6
	movqd	0,-68(fp)
	movqd	0,-72(fp)
	movqd	0,-76(fp)
	movqd	0,-80(fp)
	movqd	0,-84(fp)
	movqd	0,-88(fp)
	movd	12(fp),r0
	checkd	r0,@00011CD8,r0
	bfs	00002A68

l000029A9:
	cased	@000029B0[r0:d]
000029B0 61 00 00 00 5D 00 00 00 5D 00 00 00 5D 00 00 00 a...]...]...]...
000029C0 BF 00 00 00 BF 00 00 00 BF 00 00 00 BF 00 00 00 ................
000029D0 BF 00 00 00 BF 00 00 00 BF 00 00 00 66 00 00 00 ............f...
000029E0 BF 00 00 00 BF 00 00 00 BF 00 00 00 BF 00 00 00 ................
000029F0 BF 00 00 00 61 00 00 00 BF 00 00 00 BF 00 00 00 ....a...........
00002A00 6B 00 00 00 EA 13 8F C0 BF B0 27 A9 0A EA 0A 27 k.........'....'
00002A10 A9 08 EA 05 27 A9 10 8F 7F 00 1F 78 00 6A 3C 17 ....'......x.j<.
00002A20 78 04 8F 78 04 CE 58 41 00 17 28 07 05 00 00 00 x..x..XA..(.....
00002A30 2B 0A 0E 07 05 00 00 00 2D 1A 3F 8F C0 BF AC 8F +.......-.?.....
00002A40 C7 10 8F 7F 00 1F 78 00 7A 26 D7 3D 7F AE C0 00 ......x.z&.=....
00002A50 30 D0 7C A5 FC 57 01 EA 21 D7 3D 7F AE C0 00 30 0.|..W..!.=....0
00002A60 D0 7C A5 FC 57 01 EA 43                         .|..W..C        

l00002A68:
	movqd	0,r0
	exit	[r4,r5,r6,r7]
	ret	0
00002A6E                                           17 78               .x
00002A70 04 8F 78 04 CE 58 41 00 8F C7 10 1F C0 10 6A 81 ..x..XA.......j.
00002A80 3C CE 1C 68 C0 01 1D AD 37 A0 00 00 00 02 8A 1B <..h....7.......
00002A90 07 25 00 00 00 10 1A 80 6D CE 1C 68 C0 01 1D AD .%......m..h....
00002AA0 37 A0 00 00 00 07 9A 80 5D CE 1C 68 C0 01 1D AD 7.......]..h....
00002AB0 37 A0 00 00 00 02 9A 07 27 A8 30 EA 1A CE 1C 68 7.......'.0....h
00002AC0 C0 01 1D AD 37 A0 00 00 00 00 9A 07 27 A8 37 EA ....7.......'.7.
00002AD0 06 27 A8 80 57 57 28 63 00 17 0E BF A4 07 09 DA .'..WW(c........
00002AE0 80 DB 1F C0 08 0A 17 1F C0 BF B0 1A 11 17 20 CE .............. .
00002AF0 23 C0 BF A8 03 C0 BF A4 17 06 BF A8 8F C0 BF BC #...............
00002B00 EA 80 88 1F C0 BF B0 0A 80 B3 07 2D 00 00 00 2E ...........-....
00002B10 1A 0D 1F C0 BF B8 8F C0 BF B8 0A 80 6E 07 2D 00 ............n.-.
00002B20 00 00 65 0A 0B 07 2D 00 00 00 45 1A 80 8F 1F C0 ..e...-...E.....
00002B30 BF BC 0A 80 88 1F C0 BF B4 8F C0 BF B4 1A 80 7D ...............}
00002B40 CE 1C 28 94 03 00 8F 30 8F 7F 00 1F 78 00 7A 11 ..(....0....x.z.
00002B50 D7 3D 7F AE C0 00 30 D0 7C A5 FC 57 01 EA 0C 17 .=....0.|..W....
00002B60 78 04 8F 78 04 CE 58 41 00 CE 1C 68 C0 01 1D AD x..x..XA...h....
00002B70 37 A0 00 00 00 02 8A 12 07 2D 00 00 00 2B 0A 0A 7........-...+..
00002B80 07 2D 00 00 00 2D 1A 34 CE 1C 28 94 03 00 8F 30 .-...-.4..(....0
00002B90 8F 7F 00 1F 78 00 7A 11 D7 3D 7F AE C0 00 30 D0 ....x.z..=....0.
00002BA0 7C A5 FC 57 01 EA 0C 17 78 04 8F 78 04 CE 58 41 |..W....x..x..XA
00002BB0 00 8F C7 10 1F C0 10 7A BE CA 1F C0 08 0A 80 9E .......z........
00002BC0 1F C0 BF BC 0A 80 97 1F C0 BF B0 0A 80 54 5C 70 .............T\p
00002BD0 00 E7 C5 40 7F AE C0 00 2E 38 7C A5 FC BE 04 06 ...@.....8|.....
00002BE0 BF A0 1F C0 BF AC 0A 09 BE 14 C6 BF A0 BF A0 07 ................
00002BF0 C5 14 00 00 00 6C 1A 16 0F 82 1C 00 17 80 1C 00 .....l..........
00002C00 17 40 7C BE 04 C2 BF A0 00 EA 80 52 0F 82 1C 00 .@|........R....
00002C10 17 80 1C 00 17 40 7C 3E 16 C2 BF A0 00 EA 3E 1F .....@|>......>.
00002C20 C0 BF AC 0A 13 07 C5 BF A8 80 00 00 00 0A 09 4E ...............N
00002C30 23 C6 BF A8 BF A8 07 C5 14 00 00 00 6C 0A 3A 07 #...........l.:.
00002C40 C5 14 00 00 00 68 1A 31 CE 1D C0 BF A8 0F 82 1C .....h.1........
00002C50 00 57 80 1C 00 57 48 7C 55 02 00 D7 3D D7 2D 7F .W...WH|U...=.-.
00002C60 AE C0 00 39 30 7C A5 F8 9F 07 1A 06 5F 80 1C 00 ...90|......_...
00002C70 17 C0 BF BC EA BD F6 0F 82 1C 00 17 80 1C 00 17 ................
00002C80 40 7C 17 C2 BF A8 00 EA 54                      @|......T       

;; fn00002C89: 00002C89
;;   Called from:
;;     000028E0 (in fn000026FC)
fn00002C89 proc
	enter	[r4,r5,r6,r7],H'C
	movd	8(fp),r7
	movd	12(fp),r6
	movd	16(fp),r5
	movd	20(fp),r4
	cmpqd	0,r7
	beq	00002CAA

l00002C9C:
	addqd	4,0(28(fp))
	movd	0(28(fp)),r0
	movd	-4(r0),-8(fp)
	br	00002CAD

l00002CAA:
	movqd	0,-8(fp)

l00002CAD:
	movd	-8(fp),-12(fp)
	cmpd	r6,H'63
	bne	00002CDB

l00002CB9:
	cmpd	r5,H'7FFFFFFF
	bne	00002CDB

l00002CC1:
	movqd	1,r5
	br	00002CDB

l00002CC5:
	cmpqd	0,r7
	beq	00002CD1

l00002CC9:
	movxbd	-4(fp),r0
	movb	r0,0(-8(fp))

l00002CD1:
	addqd	1,-8(fp)
	addqd	-1,r5
	cmpqd	0,r5
	bge	00002D37

l00002CDB:
	addqd	-1,0(24(fp))
	cmpqd	0,0(24(fp))
	ble	00002CF6

l00002CE5:
	movd	24(fp),tos
	jsr	fn000030D0
	adjspb	H'FC
	movd	r0,-4(fp)
	br	00002D03

l00002CF6:
	movd	4(24(fp)),r1
	addqd	1,4(24(fp))
	movzbd	0(r1),-4(fp)

l00002D03:
	movd	-4(fp),r1
	cmpqd	-1,r1
	beq	00002D37

l00002D0A:
	cmpd	r6,H'73
	bne	00002D22

l00002D12:
	movxbd	73133(-4(fp)),r0
	tbitd	3,r0
	bfs	00002D37

l00002D22:
	cmpd	r6,H'5B
	bne	00002CC5

l00002D2B:
	movd	-4(fp),r0
	movxbd	r4[r0:b],r1
	cmpqd	0,r1
	beq	00002CC5

l00002D37:
	cmpqd	-1,-4(fp)
	beq	00002D53

l00002D3C:
	cmpqd	0,r5
	bge	00002D57

l00002D40:
	movd	24(fp),tos
	movd	-4(fp),tos
	jsr	fn00003930
	adjspb	H'F8
	cmpqd	-1,r0
	bne	00002D57

l00002D53:
	movqd	0,0(28(fp))

l00002D57:
	cmpd	-8(fp),-12(fp)
	bne	00002D63

l00002D5D:
	movqd	0,r0

l00002D5F:
	exit	[r4,r5,r6,r7]
	ret	0

l00002D63:
	cmpqd	0,r7
	beq	00002D73

l00002D67:
	cmpd	r6,H'63
	beq	00002D73

l00002D6F:
	movqb	0,0(-8(fp))

l00002D73:
	movqd	1,r0
	br	00002D5F

;; fn00002D77: 00002D77
;;   Called from:
;;     0000282D (in fn000026FC)
fn00002D77 proc
	enter	[r4,r5,r6,r7],8
	movd	8(fp),r7
	movd	12(fp),r6
	movqd	0,-8(fp)
	cmpb	0(r7),H'5E
	bne	00002D8E

l00002D89:
	addqd	1,-8(fp)
	addqd	1,r7

l00002D8E:
	addr	@00000100,tos
	cmpqd	0,-8(fp)
	bne	00002D9B

l00002D97:
	movqd	1,r0
	br	00002D9D

l00002D9B:
	movqd	0,r0

l00002D9D:
	movd	r0,tos
	movd	r6,tos
	jsr	fn000031BC
	adjspb	H'F4
	movzbd	0(r7),r4
	movd	r4,r0
	cmpd	r0,H'5D
	beq	00002DC1

l00002DB8:
	cmpd	r4,H'2D
	bne	00002E1E

l00002DC1:
	movxbd	-8(fp),r0
	movb	r0,r6[r4:b]
	br	00002E13

l00002DCB:
	cmpqd	0,r4
	bne	00002DD5

l00002DCF:
	movqd	0,r0

l00002DD1:
	exit	[r4,r5,r6,r7]
	ret	0

l00002DD5:
	cmpd	r4,H'2D
	bne	00002E17

l00002DDD:
	movzbd	0(r7),-4(fp)
	movd	-4(fp),r0
	cmpd	r0,H'5D
	beq	00002E17

l00002DED:
	movzbd	-2(r7),r5
	movd	r5,r0
	cmpd	r0,-4(fp)
	bge	00002E17

l00002DF8:
	movd	-4(fp),r0
	subd	r5,r0
	addqd	1,r0
	movd	r0,tos
	movd	-8(fp),tos
	movd	r6,r0
	addd	r5,r0
	movd	r0,tos
	jsr	fn000031BC
	adjspb	H'F4

l00002E13:
	addqd	1,r7
	br	00002E1E

l00002E17:
	movxbd	-8(fp),r0
	movb	r0,r6[r4:b]

l00002E1E:
	movd	r7,r0
	addqd	1,r7
	movzbd	0(r0),r4
	movd	r4,r1
	cmpd	r1,H'5D
	bne	00002DCB

l00002E31:
	movd	r7,r0
	br	00002DD1
00002E36                   F2 F2 82 F0 1C D7 C1 08 5F C0       ........_.
00002E40 7C 5F C0 78 EA 04 8F 38 CE 9C 79 00 17 30 CE 1C |_.x...8..y..0..
00002E50 40 C0 01 1D AD 37 A0 00 00 00 03 8A 6B 17 30 07 @....7......k.0.
00002E60 05 00 00 00 2B 0A 0D 07 05 00 00 00 2D 1A 07 8F ....+.......-...
00002E70 C0 78 8F 38 5F 28 5F 20 DF C0 6C 5F C0 68 5F C0 .x.8_(_ ..l_.h_.
00002E80 64 EA 80 9F 07 35 00 00 00 2E 0A 80 96 1F C0 68 d....5.........h
00002E90 0A 80 53 07 35 00 00 00 30 1A 80 42 8F C0 64 EA ..S.5...0..B..d.
00002EA0 80 81 8F C7 7C 07 2D 0C CC CC CC DA 0B CE 63 A1 ....|.-.......c.
00002EB0 00 00 00 0A EA 24 07 C5 6C 0C CC CC CC DA 18 17 .....$..l.......
00002EC0 C0 6C CE 23 A0 00 00 00 0A 17 06 6C CE 23 A1 00 .l.#.......l.#..
00002ED0 00 00 0A EA 05 8F C0 7C 8F C7 64 1F C0 64 CA 44 .......|..d..d.D
00002EE0 8F C7 7C 07 2D 0C CC CC CC DA 10 CE 63 A1 00 00 ..|.-.......c...
00002EF0 00 0A 27 70 50 43 01 EA 29 07 C5 6C 0C CC CC CC ..'pPC..)..l....
00002F00 DA 1D 17 C0 6C CE 23 A0 00 00 00 0A 17 06 6C CE ....l.#.......l.
00002F10 23 A1 00 00 00 0A 27 70 50 03 01 EA 05 8F C0 7C #.....'pP......|
00002F20 17 38 8F 38 CE 9C 41 00 57 30 CE 1C 48 C0 01 1D .8.8..A.W0..H...
00002F30 AD 37 A0 00 00 00 02 8A BF 4D 07 35 00 00 00 2E .7.......M.5....
00002F40 1A 0B 1F C0 68 8F C0 68 0A BF 3C 1F 28 1A 17 BE ....h..h..<.(...
00002F50 04 A0 00 00 00 00 00 00 00 00 EA 06 BE 14 C0 70 ...............p
00002F60 92 0F 12 00 3E 03 2E 70 9F C0 6C DA 14 3E 03 C0 ....>..p..l..>..
00002F70 6C BE 30 C0 70 3E 83 20 BE 00 10 BE 04 06 70 07 l.0.p>. ......p.
00002F80 35 00 00 00 45 0A 0B 07 35 00 00 00 65 1A 80 77 5...E...5...e..w
00002F90 5F 28 5F 20 CE 1C 78 00 07 05 00 00 00 20 0A 14 _(_ ..x...... ..
00002FA0 07 05 00 00 00 2B 0A 0C 07 05 00 00 00 2D 1A 06 .....+.......-..
00002FB0 8F 20 8F 38 CE 9C 79 00 17 30 CE 1C 40 C0 01 1D . .8..y..0..@...
00002FC0 AD 37 A0 00 00 00 02 9A 3D 07 2D 00 00 04 00 DA .7......=.-.....
00002FD0 10 17 28 CE 23 A0 00 00 00 0A 03 30 67 41 50 8F ..(.#......0gAP.
00002FE0 38 17 38 CE 9C 41 00 57 30 CE 1C 48 C0 01 1D AD 8.8..A.W0..H....
00002FF0 37 A0 00 00 00 02 8A 53 1F 20 0A 07 23 2E 7C EA 7......S. ..#.|.
00003000 05 03 2E 7C 1F C0 7C 0A 80 BA 57 A1 00 01 1C E0 ...|..|...W.....
00003010 BE 04 C6 70 68 BE 08 6D 00 00 00 00 00 00 00 00 ...ph..m........
00003020 00 1A 33 BE 44 A3 3F F4 00 00 00 00 00 00 00 07 ..3.D.?.........
00003030 2D 00 01 1D 20 DA 19 BE 04 68 00 BE 30 68 00 BE -... ....h..0h..
00003040 44 03 08 67 69 08 07 2D 00 01 1D 20 CA 6B 57 A1 D..gi..-... .kW.
00003050 00 01 1C E0 97 C1 7C 17 30 1F 00 7A 11 4E A3 31 ......|.0..z.N.1
00003060 BE 04 A6 3F F0 00 00 00 00 00 00 68 07 35 00 00 ...?.......h.5..
00003070 02 00 7A 0B A7 A9 82 00 EA 05 67 69 08 B7 A1 00 ..z.......gi....
00003080 00 00 00 9A 07 BE 30 6E 00 68 4E 87 A1 FF 1F 30 ......0n.hN....0
00003090 1A 6A 17 C0 7C CE 23 A0 00 00 00 03 D7 05 1F C0 .j..|.#.........
000030A0 7C 7A 0C BE 04 C0 70 BE 20 C0 68 EA 06 BE 04 C0 |z....p. .h.....
000030B0 68 BE C4 05 7F AE C0 00 32 5C 7C A5 F4 BE 04 06 h.......2\|.....
000030C0 70 1F C0 78 1A BE 98 BE 04 C0 70 EA BE 95 F2 F2 p..x......p.....

;; fn000030D0: 000030D0
;;   Called from:
;;     00001695 (in fn0000167E)
;;     000016DC (in fn0000167E)
;;     000022FA (in fn000022B4)
;;     0000270F (in fn000026FC)
;;     00002729 (in fn000026FC)
;;     00002873 (in fn000026FC)
;;     00002CE8 (in fn00002C89)
fn000030D0 proc
	enter	[r6,r7],0
	movd	8(fp),r7
	cmpqd	0,8(r7)
	bne	000030E6

l000030DB:
	movd	r7,tos
	jsr	fn00005009
	adjspb	H'FC

l000030E6:
	movxbd	12(r7),r0
	tbitd	0,r0
	bfs	00003104

l000030F2:
	movxbd	12(r7),r0
	tbitd	7,r0
	bfc	000031B6

l000030FF:
	sbitb	0,12(r7)

l00003104:
	movxbd	12(r7),r0
	andd	H'44,r0
	cmpqd	0,r0
	beq	00003142

l00003112:
	movd	H'1204C,r6
	cmpd	r6,@0001218C
	bge	00003142

l00003120:
	movxbd	12(r6),r0
	tbitd	6,r0
	bfc	00003137

l0000312C:
	movd	r6,tos
	jsr	fn00004D67
	adjspb	H'FC

l00003137:
	addr	16(r6),r6
	cmpd	r6,@0001218C
	blt	00003120

l00003142:
	movd	8(r7),4(r7)
	movxbd	12(r7),r0
	tbitd	2,r0
	bfc	00003156

l00003152:
	movqd	1,r0
	br	00003166

l00003156:
	movxbd	13(r7),r0
	movd	@00012190[r0:d],r1
	subd	8(r7),r1
	movd	r1,r0

l00003166:
	movd	r0,tos
	movd	8(r7),tos
	movxbd	13(r7),tos
	jsr	fn000038C0
	adjspb	H'F4
	movd	r0,0(r7)
	addqd	-1,0(r7)
	cmpqd	0,0(r7)
	bgt	00003191

l00003183:
	movd	4(r7),r0
	addqd	1,4(r7)
	movzbd	0(r0),r0

l0000318D:
	exit	[r6,r7]
	ret	0

l00003191:
	cmpqd	-1,0(r7)
	beq	0000319D

l00003196:
	sbitb	5,12(r7)
	br	000031B3

l0000319D:
	sbitb	4,12(r7)
	movxbd	12(r7),r0
	tbitd	7,r0
	bfc	000031B3

l000031AE:
	cbitb	0,12(r7)

l000031B3:
	movqd	0,0(r7)

l000031B6:
	movqd	-1,r0
	br	0000318D
000031BA                               F2 F2                       ..    

;; fn000031BC: 000031BC
;;   Called from:
;;     00002DA1 (in fn00002D77)
;;     00002E0A (in fn00002D77)
fn000031BC proc
	enter	[r4,r5,r6,r7],0
	movd	8(fp),r7
	movxbd	12(fp),r6
	movd	16(fp),r5
	movd	r7,r4
	br	000031D2

l000031CD:
	movb	r6,0(r7)
	addqd	1,r7

l000031D2:
	addqd	-1,r5
	cmpqd	0,r5
	ble	000031CD

l000031D8:
	movd	r4,r0
	exit	[r4,r5,r6,r7]
	ret	0
000031DE                                           F2 F2               ..

;; fn000031E0: 000031E0
;;   Called from:
;;     0000085C (in fn0000079D)
;;     00000D2C (in fn00000B29)
;;     00000E6E (in fn00000B29)
;;     0000156B (in fn00001532)
;;     00001584 (in fn00001532)
;;     0000159D (in fn00001532)
;;     000015B5 (in fn00001532)
;;     000015CD (in fn00001532)
fn000031E0 proc
	enter	[r7],H'14
	movd	H'7FFFFFFF,-16(fp)
	movd	8(fp),-12(fp)
	movd	-12(fp),-8(fp)
	movqb	2,-4(fp)
	movb	H'14,-3(fp)
	addr	16(fp),-20(fp)
	addr	-16(fp),tos
	movd	-20(fp),tos
	movd	12(fp),tos
	jsr	fn00003A45
	adjspb	H'F4
	movd	r0,r7
	movqb	0,0(-12(fp))
	movd	r7,r0
	exit	[r7]
	ret	0
0000321B                                  F2                        .    

;; fn0000321C: 0000321C
;;   Called from:
;;     000004F9 (in fn0000012C)
;;     00000615 (in fn0000012C)
;;     000006C0 (in fn0000012C)
;;     00000F12 (in fn00000B29)
;;     00000F29 (in fn00000B29)
fn0000321C proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movd	12(fp),r6
	cmpd	r7,r6
	bne	0000324D

l00003229:
	movqd	0,r0
	br	0000323D

l0000322D:
	addqd	-1,r6
	movd	r6,r0
	movxbd	0(r7),r1
	movxbd	0(r0),r0
	subd	r0,r1
	movd	r1,r0

l0000323D:
	exit	[r6,r7]
	ret	0

l00003241:
	movd	r7,r0
	addqd	1,r7
	movxbd	0(r0),r1
	cmpqd	0,r1
	beq	00003229

l0000324D:
	movd	r6,r0
	addqd	1,r6
	cmpb	0(r7),0(r0)
	beq	00003241

l00003257:
	br	0000322D
00003259                            F2 F2 F2 82 80 04 D7          .......
00003260 C1 10 1F 38 0A 10 BE 08 C5 08 00 00 00 00 00 00 ...8............
00003270 00 00 1A 17 BE 04 C0 08 EA 0D BE 14 A0 47 DF FF .............G..
00003280 FF FF FF FF FF 92 01 12 00 E7 C5 7C BE C4 C5 08 ...........|....
00003290 7F AE C0 00 33 60 7C A5 F4 1F 38 DA 80 66 17 38 ....3`|...8..f.8
000032A0 03 C0 7C 07 05 00 00 04 00 7A 26 67 AD 22 C0 01 ..|......z&g."..
000032B0 22 00 BE 08 C5 08 00 00 00 00 00 00 00 00 CA BF "...............
000032C0 BC BE 04 A0 47 DF FF FF FF FF FF FF EA BF B9 07 ....G...........
000032D0 3D 00 00 00 1E 7A 19 BE 30 A6 41 D0 00 00 00 00 =....z..0.A.....
000032E0 00 00 08 E7 79 62 07 3D 00 00 00 1E 6A 6B 17 A0 ....yb.=....jk..
000032F0 00 00 00 01 4E 07 38 3E 03 00 BE 30 C0 08 EA BF ....N.8>...0....
00003300 87 17 38 03 C0 7C 07 05 FF FF FC 00 DA 17 67 AD ..8..|........g.
00003310 22 C0 01 22 00 BE 04 A0 00 00 00 00 00 00 00 00 ".."............
00003320 EA BF 65 07 3D FF FF FF E2 DA 19 BE 30 A6 3E 10 ..e.=.......0.>.
00003330 00 00 00 00 00 00 08 E7 79 1E 07 3D FF FF FF E2 ........y..=....
00003340 CA 6B 4E 23 38 57 A0 00 00 00 01 4E 47 00 3E 03 .kN#8W.....NG.>.
00003350 08 BE 84 C0 08 BE A0 00 BE 04 10 EA BF 2A F2 F2 .............*..
00003360 82 80 08 D7 C1 10 5F 78 00 BE 08 C5 08 00 00 00 ......_x........
00003370 00 00 00 00 00 1A 10 BE 04 C0 08 EA 06 BE 04 C0 ................
00003380 78 92 01 12 00 BE 08 C5 08 00 00 00 00 00 00 00 x...............
00003390 00 7A 09 BE 04 C6 08 78 EA 07 BE 14 C6 08 78 BE .z.....x......x.
000033A0 08 C5 78 3F F0 00 00 00 00 00 00 CA 1F 8F 78 00 ..x?..........x.
000033B0 BE 30 A6 3F E0 00 00 00 00 00 00 78 BE 08 C5 78 .0.?.......x...x
000033C0 3F F0 00 00 00 00 00 00 DA 65 BE 08 C5 78 3F E0 ?........e...x?.
000033D0 00 00 00 00 00 00 DA 18 8F 7F 00 BE 00 C6 78 78 ..............xx
000033E0 BE 08 C5 78 3F E0 00 00 00 00 00 00 CA 6C BE 08 ...x?........l..
000033F0 C5 08 00 00 00 00 00 00 00 00 6A BF 83 BE 14 C0 ..........j.....
00003400 78 EA BF 80                                     x...            

;; fn00003404: 00003404
;;   Called from:
;;     00000878 (in fn0000079D)
fn00003404 proc
	enter	[r6,r7],H'C
	jsr	fn000034FC
	movd	r0,-8(fp)
	movd	-8(fp),r1
	cmpqd	0,r1
	bne	00003444

l00003417:
	movqd	0,tos
	movd	8(fp),tos
	movd	H'11D33,tos
	movd	H'11D30,tos
	movd	H'11D28,tos
	jsr	fn000034AC
	adjspb	H'EC
	addr	@0000007F,tos
	jsr	fn000034F4
	adjspb	H'FC

l00003444:
	movqd	1,tos
	movqd	2,tos
	jsr	fn00003514
	adjspb	H'F8
	movd	r0,r7
	movqd	1,tos
	movqd	3,tos
	jsr	fn00003514
	adjspb	H'F8
	movd	r0,r6

l00003462:
	addr	-4(fp),tos
	jsr	fn00005588
	adjspb	H'FC
	movd	r0,-12(fp)
	movd	-12(fp),r1
	cmpd	r1,-8(fp)
	beq	0000347E

l00003479:
	cmpqd	-1,-12(fp)
	bne	00003462

l0000347E:
	movd	r7,tos
	movqd	2,tos
	jsr	fn00003514
	adjspb	H'F8
	movd	r6,tos
	movqd	3,tos
	jsr	fn00003514
	adjspb	H'F8
	cmpqd	-1,-12(fp)
	bne	000034A4

l0000349D:
	movd	-12(fp),r0

l000034A0:
	exit	[r6,r7]
	ret	0

l000034A4:
	movd	-4(fp),r0
	br	000034A0
000034A9                            F2 F2 F2                      ...    

;; fn000034AC: 000034AC
;;   Called from:
;;     00001443 (in fn000013F9)
;;     0000145C (in fn000013F9)
;;     00001814 (in fn000017CF)
;;     0000182B (in fn000017CF)
;;     0000342E (in fn00003404)
fn000034AC proc
	enter	[],0
	movd	@000115D4,tos
	addr	12(fp),tos
	movd	8(fp),tos
	jsr	fn000034C8
	adjspb	H'F4
	exit	[]
	ret	0

;; fn000034C8: 000034C8
;;   Called from:
;;     000034BB (in fn000034AC)
fn000034C8 proc
	enter	[],0
	movd	@000115D4,tos
	movd	12(fp),tos
	movd	8(fp),tos
	jsr	fn000034E4
	adjspb	H'F4
	exit	[]
	ret	0

;; fn000034E4: 000034E4
;;   Called from:
;;     000034D7 (in fn000034C8)
fn000034E4 proc
	addr	@0000003B,r0
	addr	4(sp),r1
	svc
	jump	@000055B8
000034F1    F2 F2 F2                                      ...            

;; fn000034F4: 000034F4
;;   Called from:
;;     0000343B (in fn00003404)
fn000034F4 proc
	addr	@00000001,r0
	addr	4(sp),r1
	svc
	bpt

;; fn000034FC: 000034FC
;;   Called from:
;;     00001428 (in fn000013F9)
;;     000017DE (in fn000017CF)
;;     00003407 (in fn00003404)
;;     000034FB (in fn000034F4)
fn000034FC proc
	addr	@00000002,r0
	addr	4(sp),r1
	svc
	bfc	0000350B

l00003505:
	jump	@000055B8

l0000350B:
	cmpqd	0,r1
	beq	00003511

l0000350F:
	movqd	0,r0

l00003511:
	ret	0
00003513          F2                                        .            

;; fn00003514: 00003514
;;   Called from:
;;     0000048B (in fn0000012C)
;;     000004A0 (in fn0000012C)
;;     000004AD (in fn0000012C)
;;     000004C2 (in fn0000012C)
;;     000004CF (in fn0000012C)
;;     000004E4 (in fn0000012C)
;;     00003448 (in fn00003404)
;;     00003457 (in fn00003404)
;;     00003482 (in fn00003404)
;;     0000348F (in fn00003404)
fn00003514 proc
	br	00003592

l00003517:
	cmpqd	0,8(fp)
	bge	00003525

l0000351C:
	cmpd	8(fp),H'14
	ble	00003537

l00003525:
	movd	12(fp),tos
	movd	8(fp),tos
	jsr	fn00003598
	adjspb	H'F8
	br	0000358E

l00003537:
	movd	8(fp),r0
	movd	@00014C78[r0:d],-4(fp)
	cmpqd	0,12(fp)
	beq	0000354C

l00003547:
	cmpqd	1,12(fp)
	bne	00003558

l0000354C:
	movd	8(fp),r0
	movqd	0,@00014C78[r0:d]
	br	0000356A

l00003558:
	movd	8(fp),r0
	movd	-6132(fp),1(fp)[r0:d]
	acbb	0,23(r7),00001B61

l00003566:
	addb	r6,H'AA
	addqb	-2,84727749(sb)

l0000356A:
	movd	12(fp),tos
	movd	8(fp),tos
	jsr	fn00003598
	adjspb	H'F8
	movd	r0,-8(fp)
	cmpd	-8(fp),H'35AA
	bne	00003589

l00003585:
	movd	-4(fp),-8(fp)

l00003589:
	movd	-8(fp),r0
	br	0000358E

l0000358E:
	exit	[]
	ret	0

l00003592:
	enter	[],8
	br	00003517

;; fn00003598: 00003598
;;   Called from:
;;     0000352B (in fn00003514)
;;     00003570 (in fn00003514)
fn00003598 proc
	addr	@00000030,r0
	addr	4(sp),r1
	svc
	bfc	000035A7

l000035A1:
	jump	@000055B8

l000035A7:
	ret	0
000035A9                            F2 62 0F 2F 18 17 C8          .b./...
000035B0 18 D7 05 4E 07 A0 02 17 40 C0 01 4C 78 7F 06 7C ...N....@..Lx..|
000035C0 A5 FC 6F 18 72 F0 32 04                         ..o.r.2.        

;; fn000035C8: 000035C8
;;   Called from:
;;     000021CA (in fn000021C1)
fn000035C8 proc
	enter	[r5,r6,r7],0
	movd	8(fp),r7
	movd	@000115D4,r5
	cmpqd	0,r5
	bne	000035E2

l000035D8:
	movqd	0,r0
	br	000035DE

l000035DC:
	movd	r6,r0

l000035DE:
	exit	[r5,r6,r7]
	ret	0

l000035E2:
	cmpqd	0,0(r5)
	beq	000035D8

l000035E7:
	movd	r5,r0
	addqd	4,r5
	movd	0(r0),tos
	movd	r7,tos
	jsr	fn00003603
	adjspb	H'F8
	movd	r0,r6
	movd	r6,r1
	cmpqd	0,r1
	beq	000035E2

l00003601:
	br	000035DC

;; fn00003603: 00003603
;;   Called from:
;;     000035F0 (in fn000035C8)
fn00003603 proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movd	12(fp),r6
	br	00003618

l0000360E:
	movd	r7,r0
	addqd	1,r7
	cmpb	0(r0),H'3D
	beq	00003630

l00003618:
	movd	r6,r0
	addqd	1,r6
	cmpb	0(r7),0(r0)
	beq	0000360E

l00003622:
	movxbd	0(r7),r0
	cmpqd	0,r0
	bne	00003636

l0000362A:
	cmpb	-1(r6),H'3D
	bne	00003636

l00003630:
	movd	r6,r0

l00003632:
	exit	[r6,r7]
	ret	0

l00003636:
	movqd	0,r0
	br	00003632
0000363A                               F2 F2                       ..    

;; fn0000363C: 0000363C
;;   Called from:
;;     00001153 (in fn00000FF5)
fn0000363C proc
	addr	@0000000D,r0
	addr	4(sp),r1
	svc
	movd	4(sp),r1
	cmpqd	0,r1
	beq	0000364D

l0000364A:
	movd	r0,0(r1)

l0000364D:
	ret	0
0000364F                                              F2                .

;; fn00003650: 00003650
;;   Called from:
;;     0000035B (in fn0000012C)
fn00003650 proc
	enter	[],0
	jsr	fn00003784
	movd	r0,tos
	movd	12(fp),tos
	movd	8(fp),tos
	jsr	fn00003694
	adjspb	H'F4
	exit	[]
	ret	0

;; fn0000366E: 0000366E
;;   Called from:
;;     00000893 (in fn0000079D)
fn0000366E proc
	enter	[r7],0
	movd	16(fp),r7
	movd	r7,tos
	jsr	fn00004CEB
	adjspb	H'FC
	movd	r7,tos
	movd	12(fp),tos
	movd	8(fp),tos
	jsr	fn00003694
	adjspb	H'F4
	exit	[r7]
	ret	0

;; fn00003694: 00003694
;;   Called from:
;;     00003661 (in fn00003650)
;;     00003687 (in fn0000366E)
fn00003694 proc
	enter	[r4,r5,r6,r7],0
	movd	16(fp),r7
	cmpqd	0,r7
	beq	00003765

l0000369F:
	cmpqd	0,8(fp)
	beq	00003765

l000036A5:
	movxbd	0(8(fp)),r0
	cmpqd	0,r0
	beq	00003765

l000036AF:
	cmpb	1(12(fp)),H'2B
	seqd	r0
	movd	r0,r6
	movxbd	0(12(fp)),r0
	cmpd	r0,H'61
	beq	000036E8

l000036C5:
	cmpd	r0,H'72
	beq	000036FC

l000036CD:
	cmpd	r0,H'77
	bne	00003765

l000036D6:
	cmpqd	0,r6
	beq	000036DE

l000036DA:
	movqd	2,r0
	br	000036E0

l000036DE:
	movqd	1,r0

l000036E0:
	ord	H'300,r0
	br	000036F8

l000036E8:
	cmpqd	0,r6
	beq	000036F0

l000036EC:
	movqd	2,r0
	br	000036F2

l000036F0:
	movqd	1,r0

l000036F2:
	ord	H'108,r0

l000036F8:
	movd	r0,r5
	br	00003706

l000036FC:
	cmpqd	0,r6
	beq	00003704

l00003700:
	movqd	2,r5
	br	00003706

l00003704:
	movqd	0,r5

l00003706:
	addr	@000001B6,tos
	movd	r5,tos
	movd	8(fp),tos
	jsr	fn000038AC
	adjspb	H'F4
	movd	r0,r4
	movd	r4,r1
	cmpqd	0,r1
	bgt	00003765

l00003721:
	movqd	0,0(r7)
	movxbd	r4,r0
	movb	r0,13(r7)
	cmpqd	0,r6
	beq	00003734

l0000372E:
	movb	H'80,12(r7)
	br	00003747

l00003734:
	cmpb	0(12(fp)),H'72
	bne	0000373F

l0000373B:
	movqd	1,r0
	br	00003741

l0000373F:
	movqd	2,r0

l00003741:
	movxbd	r0,r0
	movb	r0,12(r7)

l00003747:
	cmpb	0(12(fp)),H'61
	bne	0000376B

l0000374E:
	cmpqd	0,r6
	bne	0000376B

l00003752:
	movqd	2,tos
	movqd	0,tos
	movd	r4,tos
	jsr	fn00003898
	adjspb	H'F4
	cmpqd	0,r0
	ble	0000376B

l00003765:
	movqd	0,r0

l00003767:
	exit	[r4,r5,r6,r7]
	ret	0

l0000376B:
	movqd	0,4(r7)
	movqd	0,8(r7)
	movxbd	13(r7),r0
	movd	-6136(r7),1(fp)[r0:d]
	subw	56(23(sb)),r0
	br	00003767
00003781    F2 F2 F2                                      ...            

;; fn00003784: 00003784
;;   Called from:
;;     00003653 (in fn00003650)
fn00003784 proc
	enter	[r7],0
	movd	H'1204C,r7
	movxbd	12(r7),r0
	andd	H'83,r0
	cmpqd	0,r0
	beq	000037BA

l0000379B:
	cmpd	r7,@0001218C
	blt	000037A9

l000037A3:
	movqd	0,r0

l000037A5:
	exit	[r7]
	ret	0

l000037A9:
	addr	16(r7),r7
	movxbd	12(r7),r0
	andd	H'83,r0
	cmpqd	0,r0
	bne	0000379B

l000037BA:
	movd	r7,r0
	br	000037A5
000037BE                                           F2 F2               ..

;; fn000037C0: 000037C0
;;   Called from:
;;     00000346 (in fn0000012C)
fn000037C0 proc
	enter	[r6,r7],0
	movd	8(fp),r7
	jsr	fn00003878
	movd	r0,r6
	movd	8(fp),tos
	jsr	fn0000554C
	adjspb	H'FC
	addd	r0,r7
	br	00003805

l000037DE:
	addr	@0000000A,tos
	movd	r6,tos
	jsr	fn0000557C
	adjspb	H'F8
	addr	48(r0),r0
	movxbd	r0,r0
	movb	r0,0(r7)
	addr	@0000000A,tos
	movd	r6,tos
	jsr	fn0000556C
	adjspb	H'F8
	movd	r0,r6

l00003805:
	addqd	-1,r7
	movd	r7,r0
	cmpb	0(r0),H'58
	beq	000037DE

l0000380F:
	addqd	1,r7
	movd	r7,r0
	movxbd	0(r0),r1
	cmpqd	0,r1
	beq	00003821

l0000381B:
	movb	H'61,0(r7)
	br	0000384F

l00003821:
	movqd	0,tos
	movd	8(fp),tos
	jsr	fn00003864
	adjspb	H'F8
	cmpqd	0,r0
	bne	00003848

l00003833:
	br	00003844

l00003835:
	addqb	1,0(r7)
	movxbd	0(r7),r0
	cmpd	r0,H'7A
	ble	0000384F

l00003844:
	movqb	0,0(8(fp))

l00003848:
	movd	8(fp),r0
	exit	[r6,r7]
	ret	0

l0000384F:
	movqd	0,tos
	movd	8(fp),tos
	jsr	fn00003864
	adjspb	H'F8
	cmpqd	0,r0
	beq	00003835

l00003861:
	br	00003848
00003863          F2                                        .            

;; fn00003864: 00003864
;;   Called from:
;;     0000141A (in fn000013F9)
;;     00003826 (in fn000037C0)
;;     00003854 (in fn000037C0)
fn00003864 proc
	addr	@00000021,r0
	addr	4(sp),r1
	svc
	bfc	00003873

l0000386D:
	jump	@000055B8

l00003873:
	ret	0
00003875                F2 F2 F2                              ...        

;; fn00003878: 00003878
;;   Called from:
;;     000037C6 (in fn000037C0)
fn00003878 proc
	addr	@00000014,r0
	addr	4(sp),r1
	svc
	ret	0
00003881    F2 F2 F2                                      ...            

;; fn00003884: 00003884
;;   Called from:
;;     000008AC (in fn0000079D)
;;     00000B57 (in fn00000B29)
fn00003884 proc
	addr	@0000001C,r0
	addr	4(sp),r1
	svc
	bfc	00003893

l0000388D:
	jump	@000055B8

l00003893:
	movqd	0,r0
	ret	0
00003897                      F2                                .        

;; fn00003898: 00003898
;;   Called from:
;;     00000FAE (in fn00000B29)
;;     00001CA5 (in fn00001C95)
;;     00001D1B (in fn00001C95)
;;     000023B1 (in fn0000236C)
;;     00002414 (in fn0000236C)
;;     00002461 (in fn0000236C)
;;     00002655 (in fn0000263C)
;;     00003758 (in fn00003694)
fn00003898 proc
	addr	@00000013,r0
	addr	4(sp),r1
	svc
	bfc	000038A7

l000038A1:
	jump	@000055B8

l000038A7:
	ret	0
000038A9                            F2 F2 F2                      ...    

;; fn000038AC: 000038AC
;;   Called from:
;;     00000551 (in fn0000012C)
;;     00000573 (in fn0000012C)
;;     00000644 (in fn0000012C)
;;     00000666 (in fn0000012C)
;;     0000070C (in fn0000012C)
;;     0000072E (in fn0000012C)
;;     00000B32 (in fn00000B29)
;;     00000F8F (in fn00000B29)
;;     0000370F (in fn00003694)
fn000038AC proc
	addr	@00000005,r0
	addr	4(sp),r1
	svc
	bfc	000038BB

l000038B5:
	jump	@000055B8

l000038BB:
	ret	0
000038BD                                        F2 F2 F2              ...

;; fn000038C0: 000038C0
;;   Called from:
;;     00000EA3 (in fn00000B29)
;;     00000FC1 (in fn00000B29)
;;     0000187B (in fn000017CF)
;;     00001A99 (in fn00001A4F)
;;     00001CE1 (in fn00001C95)
;;     0000316F (in fn000030D0)
fn000038C0 proc
	addr	@00000003,r0
	addr	4(sp),r1
	svc
	bfc	000038CF

l000038C9:
	jump	@000055B8

l000038CF:
	ret	0
000038D1    F2 F2 F2                                      ...            

;; fn000038D4: 000038D4
;;   Called from:
;;     000007EC (in fn0000079D)
;;     000007FF (in fn0000079D)
;;     00000812 (in fn0000079D)
;;     00000825 (in fn0000079D)
fn000038D4 proc
	enter	[r5,r6,r7],0
	movd	8(fp),r7
	movd	12(fp),r6
	movd	r7,r5

l000038DF:
	movd	r7,r0
	addqd	1,r7
	movxbd	0(r0),r1
	cmpqd	0,r1
	bne	000038DF

l000038EB:
	addqd	-1,r7

l000038ED:
	movd	r6,r0
	addqd	1,r6
	movd	r7,r1
	addqd	1,r7
	movb	0(r0),0(r1)
	movxbd	0(r1),r2
	cmpqd	0,r2
	bne	000038ED

l00003901:
	movd	r5,r0
	exit	[r5,r6,r7]
	ret	0
00003907                      F2                                .        

;; fn00003908: 00003908
;;   Called from:
;;     00000D08 (in fn00000B29)
;;     00000E05 (in fn00000B29)
fn00003908 proc
	enter	[r5,r6,r7],0
	movd	8(fp),r7
	movd	12(fp),r6
	movd	r7,r5

l00003913:
	movd	r6,r0
	addqd	1,r6
	movd	r7,r1
	addqd	1,r7
	movb	0(r0),0(r1)
	movxbd	0(r1),r2
	cmpqd	0,r2
	bne	00003913

l00003927:
	movd	r5,r0
	exit	[r5,r6,r7]
	ret	0
0000392D                                        F2 F2 F2              ...

;; fn00003930: 00003930
;;   Called from:
;;     0000275D (in fn000026FC)
;;     00002789 (in fn000026FC)
;;     000028A7 (in fn000026FC)
;;     00002D46 (in fn00002C89)
fn00003930 proc
	enter	[r7],0
	movd	12(fp),r7
	cmpqd	-1,8(fp)
	beq	00003972

l0000393B:
	movxbd	12(r7),r0
	tbitd	0,r0
	bfc	0000394D

l00003947:
	cmpd	4(r7),8(r7)
	bgt	0000395B

l0000394D:
	cmpd	4(r7),8(r7)
	bne	00003972

l00003953:
	cmpqd	0,0(r7)
	bne	00003972

l00003958:
	addqd	1,4(r7)

l0000395B:
	movzbd	8(fp),r0
	addqd	-1,4(r7)
	movd	4(r7),r1
	movb	r0,0(r1)
	addqd	1,0(r7)
	movd	8(fp),r0

l0000396E:
	exit	[r7]
	ret	0

l00003972:
	movqd	-1,r0
	br	0000396E
00003976                   F2 F2                               ..        

;; fn00003978: 00003978
;;   Called from:
;;     000011EF (in fn00000FF5)
;;     0000177B (in fn00001772)
fn00003978 proc
	addr	@0000000A,r0
	addr	4(sp),r1
	svc
	bfc	00003987

l00003981:
	jump	@000055B8

l00003987:
	movqd	0,r0
	ret	0
0000398B                                  F2                        .    

;; fn0000398C: 0000398C
;;   Called from:
;;     00001172 (in fn00000FF5)
fn0000398C proc
	addr	@0000001E,r0
	addr	4(sp),r1
	svc
	bfc	0000399B

l00003995:
	jump	@000055B8

l0000399B:
	ret	0
0000399D                                        F2 F2 F2              ...
000039A0 82 00 08 17 80 08 00 2B A0 00 00 00 01 17 06 7C .......+.......|
000039B0 17 80 08 00 4E 07 A0 FF 4E 0B A0 00 00 00 1F 17 ....N...N.......
000039C0 06 78 17 C0 78 CE 33 A0 00 00 00 05 17 04 08 00 .x..x.3.........
000039D0 17 C0 78 CE 37 A0 00 00 00 05 4E 07 A0 01 03 C0 ..x.7.....N.....
000039E0 7C 27 40 30 92 00 12 00                         |'@0....        

;; fn000039E8: 000039E8
;;   Called from:
;;     00003AEB (in fn00003A45)
fn000039E8 proc
	enter	[r4,r5,r6,r7],0
	movd	8(fp),r7
	movd	12(fp),r6
	movd	16(fp),r5
	movd	20(fp),r4
	cmpb	13(r5),H'14
	beq	00003A2E

l000039FD:
	movd	0(r4),r0
	subd	4(r5),r0
	subd	r0,0(r5)
	movd	0(r4),4(r5)
	movd	r5,tos
	jsr	fn000050AA
	adjspb	H'FC
	movd	r5,tos
	movd	r6,tos
	movqd	1,tos
	movd	r7,tos
	jsr	fn00004B2C
	adjspb	H'F0
	movd	4(r5),0(r4)

l00003A2A:
	exit	[r4,r5,r6,r7]
	ret	0

l00003A2E:
	movd	r6,tos
	movd	r7,tos
	movd	0(r4),tos
	jsr	fn000054D4
	adjspb	H'F4
	addd	r6,r0
	movd	r0,0(r4)
	br	00003A2A

;; fn00003A45: 00003A45
;;   Called from:
;;     000025BF (in fn00002580)
;;     0000261A (in fn000025E4)
;;     00003206 (in fn000031E0)
fn00003A45 proc
	enter	[r4,r5,r6,r7],H'1E4
	movd	8(fp),r7
	movd	16(fp),r6
	movqd	0,-12(fp)
	movxbd	13(r6),-476(fp)
	movd	4(r6),-4(fp)
	cmpd	-476(fp),H'14
	bne	00003A89

l00003A66:
	movd	-4(fp),r0
	ord	H'7FFFFFFF,r0
	movd	r0,-8(fp)
	br	00003A95
00003A74             4E 1B A6 00 00 00 09 5C 17 C0 6C 23     N......\..l#
00003A80 C0 BE 30 17 06 64 EA 8A 54                      ..0..d..T       

l00003A89:
	movd	-476(fp),r0
	movd	@00012190[r0:d],-8(fp)

l00003A95:
	movxbd	0(r7),r4
	movd	r4,r0
	cmpqd	0,r0
	beq	00003B0D

l00003AA0:
	cmpd	r4,H'25
	beq	00003B0D

l00003AA9:
	movd	r7,r5

l00003AAB:
	addqd	1,r7
	movxbd	0(r7),r4
	movd	r4,r0
	cmpqd	0,r0
	beq	00003ABF

l00003AB7:
	cmpd	r4,H'25
	bne	00003AAB

l00003ABF:
	movd	r7,r0
	subd	r5,r0
	movd	r0,-476(fp)
	addd	-476(fp),-12(fp)
	movd	-4(fp),r0
	addd	-476(fp),r0
	movd	r0,-480(fp)
	movd	-480(fp),r1
	cmpd	r1,-8(fp)
	ble	00003AF6

l00003AE0:
	addr	-4(fp),tos
	movd	r6,tos
	movd	-476(fp),tos
	movd	r5,tos
	jsr	fn000039E8
	adjspb	H'F0
	br	00003B0D

l00003AF6:
	movd	-476(fp),tos
	movd	r5,tos
	movd	-4(fp),tos
	jsr	fn000054D4
	adjspb	H'F4
	movd	-480(fp),-4(fp)

l00003B0D:
	cmpqd	0,r4
	bne	00003B98

l00003B12:
	movd	-4(fp),r0
	subd	4(r6),r0
	movd	r0,-480(fp)
	subd	-480(fp),0(r6)
	movd	-4(fp),4(r6)
	movd	-4(fp),r0
	addd	0(r6),r0
	cmpd	r0,-8(fp)
	ble	00003B41

l00003B30:
	cmpb	13(r6),H'14
	beq	00003B41

l00003B36:
	movd	r6,tos
	jsr	fn000050AA
	adjspb	H'FC

l00003B41:
	movxbd	12(r6),r0
	andd	H'44,r0
	cmpqd	0,r0
	beq	00003B81

l00003B4F:
	movxbd	12(r6),r0
	tbitd	2,r0
	bfs	00003B76

l00003B5B:
	movd	-12(fp),tos
	addr	@0000000A,tos
	movd	-4(fp),r0
	subd	-12(fp),r0
	movd	r0,tos
	jsr	fn000054A8
	adjspb	H'F4
	cmpqd	0,r0
	beq	00003B81

l00003B76:
	movd	r6,tos
	jsr	fn00004ED8
	adjspb	H'FC

l00003B81:
	movxbd	12(r6),r0
	tbitd	5,r0
	bfc	00003B93

l00003B8D:
	movqd	-1,r0

l00003B8F:
	exit	[r4,r5,r6,r7]
	ret	0

l00003B93:
	movd	-12(fp),r0
	br	00003B8F

l00003B98:
	movqd	0,-36(fp)
	movqd	0,-436(fp)
	movqd	0,-428(fp)
	movqd	0,-20(fp)
	addqd	1,r7
	br	00004852
00003BAB                                  4E 1B A6 00 00            N....
00003BB0 00 01 5C EA 8C 9F 4E 1B A6 00 00 00 02 5C EA 8C ..\...N......\..
00003BC0 94 4E 1B A6 00 00 00 03 5C EA 8C 89 4E 1B A6 00 .N......\...N...
00003BD0 00 00 04 5C EA 8C 7E 4E 1B A6 00 00 00 06 5C EA ...\..~N......\.
00003BE0 1B 37 A6 00 00 00 06 5C 9A 18 0F C2 0C 17 C0 0C .7.....\........
00003BF0 17 46 7C 68 1F C0 68 7A 8C 5B 5F C0 68 EA 8C 55 .F|h..hz.[_.h..U
00003C00 0F C2 0C 17 C0 0C 17 46 7C 6C 1F C0 6C 7A 8C 45 .......F|l..lz.E
00003C10 4E 23 C6 6C 6C 4E 3B A6 00 00 00 02 5C EA 8C 35 N#.llN;.....\..5
00003C20 17 C0 5C 2B A0 00 00 00 44 1F 00 1A 0A 4E 1B A6 ..\+....D....N..
00003C30 00 00 00 05 5C 27 66 50 BE 20 EA 8B EE 4E 1B A6 ....\'fP. ...N..
00003C40 00 00 00 00 5C EA 8C 0D 17 C0 5C 2B A0 00 00 00 ....\.....\+....
00003C50 01 1F 00 0F C2 0C 17 C0 0C 17 46 7C BE 48 67 C1 ..........F|.Hg.
00003C60 BE 74 17 2E 70 1F C0 BE 48 7A 22 17 A6 00 01 1D .t..p...Hz".....
00003C70 70 BE 64 DF C0 BE 54 07 C5 BE 48 80 00 00 00 0A p.d...T...H.....
00003C80 80 B5 4E 23 C6 BE 48 BE 48 EA 2A 37 A6 00 00 00 ..N#..H.H.*7....
00003C90 01 5C 9A 0C 17 A6 00 01 1D 72 BE 64 EA 13 37 A6 .\.......r.d..7.
00003CA0 00 00 00 03 5C 9A 0E 17 A6 00 01 1D 74 BE 64 DF ....\.......t.d.
00003CB0 C0 BE 54 17 C6 BE 48 BE 20 07 C5 BE 20 00 00 00 ..T...H. ... ...
00003CC0 09 6A 86 1C 1F C0 BE 20 1A 0B 37 A6 00 00 00 06 .j..... ..7.....
00003CD0 5C 8A 11 27 80 BE 20 30 CE 1C 00 8F 2F 57 28 54 \..'.. 0..../W(T
00003CE0 02 00 37 A6 00 00 00 06 5C 9A 87 B1 17 C0 70 23 ..7.....\.....p#
00003CF0 28 57 C0 68 63 00 17 0E BE 20 1F 08 DA 87 9E 17 (W.hc.... ......
00003D00 C6 BE 20 64 17 C6 64 BE 4C EA 84 B0 17 C0 5C 2B .. d..d.L.....\+
00003D10 A0 00 00 00 01 1F 00 0F C2 0C 17 C0 0C 17 46 7C ..............F|
00003D20 BE 48 67 C1 BE 74 17 2E 70 37 A6 00 00 00 1F BE .Hg..t..p7......
00003D30 48 9A BF 82 E7 C5 BE 48 7F AE C0 00 39 A0 7C A5 H......H....9.|.
00003D40 FC CE 1C 00 8F 2F 57 28 54 02 00 EA BF 68 DF C3 ...../W(T....h..
00003D50 BE 28 5F C1 BE 2C EA 0B 27 AE 0F BE 28 DF C1 BE .(_..,..'...(...
00003D60 2C 17 C0 5C 2B A0 00 00 00 01 1F 00 0F C2 0C 17 ,..\+...........
00003D70 C0 0C 17 46 7C BE 48 07 25 00 00 00 58 1A 0C 17 ...F|.H.%...X...
00003D80 A6 00 01 1D 76 BE 34 EA 0A 17 A6 00 01 1D 87 BE ....v.4.........
00003D90 34 67 C1 BE 74 17 2E 70 17 C6 BE 48 BE 20 1F C0 4g..t..p...H. ..
00003DA0 BE 20 1A 8A BB 37 A6 00 00 00 06 5C 8A 8A E4 DF . ...7.....\....
00003DB0 C0 64 DF C0 BE 4C 4E 1B A6 00 00 00 09 5C EA 8A .d...LN......\..
00003DC0 D2 37 A6 00 00 00 06 5C 8A 05 5F C3 68 27 86 0C .7.....\.._.h'..
00003DD0 08 0C 17 C0 0C BE 04 46 78 BE 40 E7 C5 BE 38 E7 .......Fx.@...8.
00003DE0 C5 BE 3C 17 C0 68 8F 00 07 05 00 00 00 11 DA 09 ..<..h..........
00003DF0 17 C0 68 8F 00 EA 05 27 A8 11 D7 05 BE C4 C5 BE ..h....'........
00003E00 40 7F AE C0 00 48 DC 7C A5 EC 57 01 EA 0B 17 C0 @....H.|..W.....
00003E10 BE 20 8F 07 17 06 68 1F C0 BE 38 0A 0C 17 A6 00 . ....h...8.....
00003E20 01 1D 9E BE 64 EA 13 37 A6 00 00 00 01 5C 9A 10 ....d..7.....\..
00003E30 17 A6 00 01 1D A0 BE 64 DF C0 BE 54 EA 15 37 A6 .......d...T..7.
00003E40 00 00 00 03 5C 9A 0C 17 A6 00 01 1D A2 BE 64 EA ....\.........d.
00003E50 69 27 C6 BE 69 70 CE 1C 68 00 1F 00 0A 12 17 28 i'..ip..h......(
00003E60 8F 28 57 C0 70 8F C0 70 54 42 00 00 EA 0C 17 C0 .(W.p..pTB......
00003E70 70 8F C0 70 14 A2 30 00 1F C0 68 1A 0B 37 A6 00 p..p..0...h..7..
00003E80 00 00 04 5C 9A 0C 17 C0 70 8F C0 70 14 A2 2E 00 ...\....p..p....
00003E90 17 C6 68 BE 20 1F C0 BE 20 DA 28 CE 1C 68 00 1F ..h. ... .(..h..
00003EA0 00 0A 20 17 C0 70 8F C0 70 14 6A 00 00 8F 28 8F .. ..p..p.j...(.
00003EB0 C7 BE 20 1F C0 BE 20 DA 0A CE 1C 68 00 1F 00 1A .. ... ....h....
00003EC0 64 1F C0 BE 20 DA 14 17 C6 BE 20 60 17 C6 60 BE d... ..... `..`.
00003ED0 4C 4E 1B A6 00 00 00 08 5C 67 C1 BE 69 27 C6 BE LN......\g..i'..
00003EE0 5F BE 60 57 C0 BE 60 5C 48 00 BE 08 C5 BE 40 00 _.`W..`\H.....@.
00003EF0 00 00 00 00 00 00 00 0A 82 E8 17 C0 BE 3C 8F 07 .............<..
00003F00 17 06 BE 20 1F 00 7A 09 4E 23 C6 BE 20 BE 20 07 ... ..z.N#.. . .
00003F10 C5 BE 20 00 00 00 09 7A 35 17 C0 BE 20 CE 37 A0 .. ....z5... .7.
00003F20 00 00 00 0A 27 40 30 CE 1C 00 8F C7 BE 60 57 C0 ....'@0......`W.
00003F30 BE 60 54 02 00 17 C0 BE 20 CE 33 A0 00 00 00 0A .`T..... .3.....
00003F40 17 06 BE 20 07 05 00 00 00 09 6A 4F 27 80 BE 20 ... ......jO'.. 
00003F50 30 CE 1C 00 8F C7 BE 60 57 C0 BE 60 54 02 00 EA 0......`W..`T...
00003F60 82 80 37 A6 00 00 00 06 5C 8A 05 5F C3 68 27 86 ..7.....\.._.h'.
00003F70 0C 08 0C 17 C0 0C BE 04 46 78 BE 40 E7 C5 BE 38 ........Fx.@...8
00003F80 E7 C5 BE 3C 07 C5 68 00 00 00 3C DA 07 17 C0 68 ...<..h...<....h
00003F90 EA 05 27 A8 3C D7 05 BE C4 C5 BE 40 7F AE C0 00 ..'.<......@....
00003FA0 48 FB 7C A5 EC 57 01 1F C0 BE 38 0A 1C 4E 23 C0 H.|..W....8..N#.
00003FB0 68 07 C0 BE 3C 7A 12 04 6D 00 30 0A 0C 17 A6 00 h...<z..m.0.....
00003FC0 01 1D A4 BE 64 EA 13 37 A6 00 00 00 01 5C 9A 10 ....d..7.....\..
00003FD0 17 A6 00 01 1D A6 BE 64 DF C0 BE 54 EA 15 37 A6 .......d...T..7.
00003FE0 00 00 00 03 5C 9A 0C 17 A6 00 01 1D A8 BE 64 EA ....\.........d.
00003FF0 69 27 C6 BE 69 70 17 C6 BE 3C BE 20 5F C0 BE 30 i'..ip...<. _..0
00004000 1F C0 BE 20 DA 14 CE 1C 68 00 1F 00 0A 0C 07 C5 ... ....h.......
00004010 BE 30 00 00 00 11 CA 0E 17 C0 70 8F C0 70 14 A2 .0........p..p..
00004020 30 00 EA 14 8F C0 BE 30 17 28 8F 28 57 C0 70 8F 0......0.(.(W.p.
00004030 C0 70 54 42 00 00 8F C7 BE 20 1F C0 BE 20 CA 42 .pTB..... ... .B
00004040 37 A6 00 00 00 04 5C 8A 07 1F C0 68 DA 0C 17 C0 7.....\....h....
00004050 70 8F C0 70 14 A2 2E 00 07 C5 68 00 00 00 3C DA p..p......h...<.
00004060 09 17 C6 68 BE 20 EA 07 27 AE 3C BE 20 07 C6 68 ...h. ..'.<. ..h
00004070 BE 20 7A 82 26 4E 1B A6 00 00 00 08 5C 17 C0 68 . z.&N......\..h
00004080 23 C0 BE 20 17 06 60 17 C6 60 BE 4C EA 82 0C 37 #.. ..`..`.L...7
00004090 A6 00 00 00 06 5C 8A 07 5F C3 68 EA 0A 1F C0 68 .....\.._.h....h
000040A0 1A 05 DF C0 68 27 86 0C 08 0C 17 C0 0C BE 04 46 ....h'.........F
000040B0 78 BE 40 E7 C5 BE 38 E7 C5 BE 3C 07 C5 68 00 00 x.@...8...<..h..
000040C0 00 11 DA 07 17 C0 68 EA 05 27 A8 11 D7 05 BE C4 ......h..'......
000040D0 C5 BE 40 7F AE C0 00 48 DC 7C A5 EC 57 01 BE 08 ..@....H.|..W...
000040E0 C5 BE 40 00 00 00 00 00 00 00 00 1A 06 DF C0 BE ..@.............
000040F0 3C 17 C6 68 BE 20 37 A6 00 00 00 04 5C 8A 81 C3 <..h. 7.....\...
00004100 D7 2D 7F AE C0 00 55 4C 7C A5 FC 17 06 BE 24 07 .-....UL|.....$.
00004110 06 BE 20 DA 81 9B 17 C6 BE 24 BE 20 EA 81 92 CE .. ......$. ....
00004120 1C 20 EA 0E 0F C2 0C 17 C0 0C 17 40 7C CE 1C 00 . .........@|...
00004130 14 06 BE 69 67 C1 BE 69 57 28 8F 08 17 0E 70 EA ...ig..iW(....p.
00004140 83 5B 07 05 00 00 00 6F 0A 80 60 07 05 00 00 00 .[.....o..`.....
00004150 78 1A 83 49 17 A6 00 01 1D 98 BE 64 EA 80 70 0F x..I.......d..p.
00004160 C2 0C 17 C0 0C 57 41 7C 37 A6 00 00 00 06 5C 8A .....WA|7.....\.
00004170 11 D7 2D 7F AE C0 00 55 4C 7C A5 FC 03 28 EA 24 ..-....UL|...(.$
00004180 17 2E BE 20 17 C0 BE 20 8F C0 BE 20 CE 5C 40 00 ... ... ... .\@.
00004190 1F 08 0A 0A 8F C7 68 1F C0 68 7A 6A 17 C0 BE 20 ......h..hzj... 
000041A0 8F 07 17 06 70 EA 82 F5 37 A6 00 00 00 09 5C 8A ....p...7.....\.
000041B0 82 EB DF C0 64 DF C0 BE 4C 4E 1B A6 00 00 00 09 ....d...LN......
000041C0 5C EA 82 D9 17 A6 00 01 1D 9B BE 64 5F C1 BE 54 \..........d_..T
000041D0 EA 82 CA 8F C7 BE 60 17 C0 BE 60 14 A2 30 00 27 ......`...`..0.'
000041E0 C0 BE 5D 07 C0 BE 60 6A 6C 1F C0 BE 3C CA 11 BE ..]...`jl...<...
000041F0 08 C5 BE 40 00 00 00 00 00 00 00 00 1A 10 8F C7 ...@............
00004200 BE 60 17 C0 BE 60 14 A2 2B 00 EA 0E 8F C7 BE 60 .`...`..+......`
00004210 17 C0 BE 60 14 A2 2D 00 CE 1C 60 C0 01 1D AD 37 ...`..-...`....7
00004220 A0 00 00 00 00 9A 10 8F C7 BE 60 17 C0 BE 60 14 ..........`...`.
00004230 A2 45 00 EA 0E 8F C7 BE 60 17 C0 BE 60 14 A2 65 .E......`...`..e
00004240 00 27 C0 BE 5F 23 C0 BE 60 17 06 BE 50 03 C6 BE .'.._#..`...P...
00004250 50 BE 4C 4E 1B A6 00 00 00 07 5C EA 82 3F 8F C0 P.LN......\..?..
00004260 BE 3C 1F C0 BE 3C DA 14 CE 1C 68 00 1F 00 0A 0C .<...<....h.....
00004270 07 C5 BE 30 00 00 00 11 CA 0E 17 C0 70 8F C0 70 ...0........p..p
00004280 14 A2 30 00 EA 14 8F C0 BE 30 17 28 8F 28 57 C0 ..0......0.(.(W.
00004290 70 8F C0 70 54 42 00 00 8F C7 BE 20 1F C0 BE 20 p..pTB..... ... 
000042A0 7A BF BE 67 C1 BE 69 EA 81 F3 8F C7 BE 20 9F C0 z..g..i...... ..
000042B0 BE 20 6A 0E 17 C0 BE 20 8F 07 04 E5 28 30 0A 6C . j.... ....(0.l
000042C0 9F C6 BE 3C 6A BB 4A 07 C6 BE 3C 68 6A BB 42 17 ...<j.J...<hj.B.
000042D0 C0 BE 20 23 C0 BE 3C 17 06 68 EA BC CD 17 C6 BE .. #..<..h......
000042E0 20 BE 24 17 C0 BE 20 CE 33 A0 00 00 00 0A 17 06  .$... .3.......
000042F0 BE 20 17 C0 BE 20 CE 23 A0 00 00 00 0A 57 C0 BE . ... .#.....W..
00004300 24 63 00 67 48 30 CE 1C 08 8F 2F 57 28 54 02 00 $c.gH0..../W(T..
00004310 07 C5 BE 20 00 00 00 09 6A 45 EA B9 B9 03 C6 BE ... ....jE......
00004320 30 74 EA 82 53                                  0t..S           

l00004325:
	checkd	r0,@00011D68,r0
	bfs	000044E2

l0000432F:
	cased	@00004336[r0:d]
00004336                   92 F8 FF FF B3 01 00 00 B3 01       ..........
00004340 00 00 9D F8 FF FF B3 01 00 00 F0 FD FF FF B3 01 ................
00004350 00 00 B3 01 00 00 B3 01 00 00 B3 01 00 00 B2 F8 ................
00004360 FF FF 7C F8 FF FF B3 01 00 00 87 F8 FF FF A8 F8 ..|.............
00004370 FF FF B3 01 00 00 F1 F8 FF FF 06 F9 FF FF 06 F9 ................
00004380 FF FF 06 F9 FF FF 06 F9 FF FF 06 F9 FF FF 06 F9 ................
00004390 FF FF 06 F9 FF FF 06 F9 FF FF 06 F9 FF FF B3 01 ................
000043A0 00 00 B3 01 00 00 B3 01 00 00 B3 01 00 00 B3 01 ................
000043B0 00 00 B3 01 00 00 B3 01 00 00 B3 01 00 00 B3 01 ................
000043C0 00 00 B3 01 00 00 B3 01 00 00 92 FA FF FF B3 01 ................
000043D0 00 00 60 FD FF FF B3 01 00 00 B3 01 00 00 B3 01 ..`.............
000043E0 00 00 B3 01 00 00 B3 01 00 00 B3 01 00 00 B3 01 ................
000043F0 00 00 B3 01 00 00 B3 01 00 00 B3 01 00 00 B3 01 ................
00004400 00 00 B3 01 00 00 B3 01 00 00 B3 01 00 00 B3 01 ................
00004410 00 00 B3 01 00 00 29 FA FF FF B3 01 00 00 B3 01 ......).........
00004420 00 00 B3 01 00 00 B3 01 00 00 B3 01 00 00 B3 01 ................
00004430 00 00 B3 01 00 00 B3 01 00 00 B3 01 00 00 B3 01 ................
00004440 00 00 F5 FD FF FF 19 F9 FF FF 92 FA FF FF 33 FC ..............3.
00004450 FF FF 60 FD FF FF 16 F9 FF FF B3 01 00 00 B3 01 ..`.............
00004460 00 00 B3 01 00 00 0E F9 FF FF B3 01 00 00 B3 01 ................
00004470 00 00 1F FA FF FF B3 01 00 00 B3 01 00 00 B3 01 ................
00004480 00 00 30 FE FF FF B3 01 00 00 DD F9 FF FF B3 01 ..0.............
00004490 00 00 B3 01 00 00 29 FA FF FF 17 C0 70 23 28 17 ......).....p#(.
000044A0 06 BE 24 57 C0 BE 24 43 C0 BE 54 43 C0 BE 4C 17 ..$W..$C..TC..L.
000044B0 0E BE 30 47 C0 6C 7A BE 67 03 C6 6C 74 37 A6 00 ..0G.lz.g..lt7..
000044C0 00 00 05 5C 9A 23 37 A6 00 00 00 09 5C 9A B5 A7 ...\.#7.....\...
000044D0 17 C0 6C 23 C0 BE 30 03 06 64 17 C6 6C BE 30 EA ..l#..0..d..l.0.
000044E0 80 96                                           ..              

l000044E2:
	addqd	-1,r7
	br	00003A95
000044E7                      37 A6 00 00 00 02 5C 8A 80        7.....\..
000044F0 87 17 C0 6C 23 C0 BE 30 17 06 BE 20 07 05 00 00 ...l#..0... ....
00004500 00 14 7A 2A E7 C5 7C D7 35 E7 AD 14 D7 A5 00 01 ..z*..|.5.......
00004510 1D 38 7F AE C0 00 39 E8 7C A5 F0 27 86 BE 20 6C .8....9.|..'.. l
00004520 BE 20 07 C5 BE 20 00 00 00 14 6A 5A 17 C0 7C 03 . ... ....jZ..|.
00004530 C0 BE 20 17 06 BE 1C 57 C0 BE 1C 07 0E 78 7A 1C .. ....W.....xz.
00004540 E7 C5 7C D7 35 D7 C5 BE 20 D7 A5 00 01 1D 38 7F ..|.5... .....8.
00004550 AE C0 00 39 E8 7C A5 F0 EA 1D D7 C5 BE 20 D7 A5 ...9.|....... ..
00004560 00 01 1D 38 D7 C5 7C 7F AE C0 00 54 D4 7C A5 F4 ...8..|....T.|..
00004570 17 C6 BE 1C 7C 1F C0 BE 54 0A 80 48 17 C0 7C 03 ....|...T..H..|.
00004580 C0 BE 54 17 06 BE 20 57 C0 BE 20 07 0E 78 7A 1A ..T... W.. ..xz.
00004590 E7 C5 7C D7 35 D7 C5 BE 54 D7 C5 BE 64 7F AE C0 ..|.5...T...d...
000045A0 00 39 E8 7C A5 F0 EA 1B D7 C5 BE 54 D7 C5 BE 64 .9.|.......T...d
000045B0 D7 C5 7C 7F AE C0 00 54 D4 7C A5 F4 17 C6 BE 20 ..|....T.|..... 
000045C0 7C 37 A6 00 00 00 09 5C 9A 80 83 17 C6 64 BE 20 |7.....\.....d. 
000045D0 07 C5 BE 20 00 00 00 14 7A 2A E7 C5 7C D7 35 E7 ... ....z*..|.5.
000045E0 AD 14 D7 A5 00 01 1D 50 7F AE C0 00 39 E8 7C A5 .......P....9.|.
000045F0 F0 27 86 BE 20 6C BE 20 07 C5 BE 20 00 00 00 14 .'.. l. ... ....
00004600 6A 5A 17 C0 7C 03 C0 BE 20 17 06 BE 1C 57 C0 BE jZ..|... ....W..
00004610 1C 07 0E 78 7A 1C E7 C5 7C D7 35 D7 C5 BE 20 D7 ...xz...|.5... .
00004620 A5 00 01 1D 50 7F AE C0 00 39 E8 7C A5 F0 EA 1D ....P....9.|....
00004630 D7 C5 BE 20 D7 A5 00 01 1D 50 D7 C5 7C 7F AE C0 ... .....P..|...
00004640 00 54 D4 7C A5 F4 17 C6 BE 1C 7C 1F C0 BE 24 DA .T.|......|...$.
00004650 80 44 17 C0 7C 03 C0 BE 24 17 06 BE 20 57 C0 BE .D..|...$... W..
00004660 20 07 0E 78 7A 18 E7 C5 7C D7 35 D7 C5 BE 24 D7  ..xz...|.5...$.
00004670 2D 7F AE C0 00 39 E8 7C A5 F0 EA 19 D7 C5 BE 24 -....9.|.......$
00004680 D7 2D D7 C5 7C 7F AE C0 00 54 D4 7C A5 F4 17 C6 .-..|....T.|....
00004690 BE 20 7C 17 C0 5C 2B A0 00 00 01 84 1F 00 0A B3 . |..\+.........
000046A0 F7 37 A6 00 00 00 08 5C 9A 80 83 17 C6 60 BE 20 .7.....\.....`. 
000046B0 07 C5 BE 20 00 00 00 14 7A 2A E7 C5 7C D7 35 E7 ... ....z*..|.5.
000046C0 AD 14 D7 A5 00 01 1D 50 7F AE C0 00 39 E8 7C A5 .......P....9.|.
000046D0 F0 27 86 BE 20 6C BE 20 07 C5 BE 20 00 00 00 14 .'.. l. ... ....
000046E0 6A 5A 17 C0 7C 03 C0 BE 20 17 06 BE 1C 57 C0 BE jZ..|... ....W..
000046F0 1C 07 0E 78 7A 1C E7 C5 7C D7 35 D7 C5 BE 20 D7 ...xz...|.5... .
00004700 A5 00 01 1D 50 7F AE C0 00 39 E8 7C A5 F0 EA 1D ....P....9.|....
00004710 D7 C5 BE 20 D7 A5 00 01 1D 50 D7 C5 7C 7F AE C0 ... .....P..|...
00004720 00 54 D4 7C A5 F4 17 C6 BE 1C 7C 37 A6 00 00 00 .T.|......|7....
00004730 07 5C 9A 80 48 17 C0 7C 03 C0 BE 50 17 06 BE 20 .\..H..|...P... 
00004740 57 C0 BE 20 07 0E 78 7A 1A E7 C5 7C D7 35 D7 C5 W.. ..xz...|.5..
00004750 BE 50 D7 C5 BE 60 7F AE C0 00 39 E8 7C A5 F0 EA .P...`....9.|...
00004760 1B D7 C5 BE 50 D7 C5 BE 60 D7 C5 7C 7F AE C0 00 ....P...`..|....
00004770 54 D4 7C A5 F4 17 C6 BE 20 7C 37 A6 00 00 00 02 T.|..... |7.....
00004780 5C 9A B3 14 07 C6 6C BE 30 7A B3 0C 17 C0 6C 23 \.....l.0z....l#
00004790 C0 BE 30 17 06 BE 20 07 05 00 00 00 14 7A 2A E7 ..0... ......z*.
000047A0 C5 7C D7 35 E7 AD 14 D7 A5 00 01 1D 38 7F AE C0 .|.5........8...
000047B0 00 39 E8 7C A5 F0 27 86 BE 20 6C BE 20 07 C5 BE .9.|..'.. l. ...
000047C0 20 00 00 00 14 6A 5A 17 C0 7C 03 C0 BE 20 17 06  ....jZ..|... ..
000047D0 BE 1C 57 C0 BE 1C 07 0E 78 7A 1D E7 C5 7C D7 35 ..W.....xz...|.5
000047E0 D7 C5 BE 20 D7 A5 00 01 1D 38 7F AE C0 00 39 E8 ... .....8....9.
000047F0 7C A5 F0 EA B2 A2 D7 C5 BE 20 D7 A5 00 01 1D 38 |........ .....8
00004800 D7 C5 7C 7F AE C0 00 54 D4 7C A5 F4 17 C6 BE 1C ..|....T.|......
00004810 7C EA B2 84 17 C0 BE 20 CE 23 A0 00 00 00 0A 03 |...... .#......
00004820 20 27 46 50 BE 20 8F 38 CE 1C 79 00 17 20 CE 1C  'FP. .8..y.. ..
00004830 40 C0 01 1D AD 37 A0 00 00 00 02 8A 59 37 A6 00 @....7......Y7..
00004840 00 00 06 5C 9A 09 17 C6 BE 20 68 EA 07 17 C6 BE ...\..... h.....
00004850 20 6C                                            l              

l00004852:
	movxbd	0(r7),r4
	movd	r4,r0
	addqd	1,r7
	br	00004325
0000485D                                        17 C0 BE              ...
00004860 34 57 C0 BE 20 6B C0 BE 28 8F 2F 97 28 94 E2 01 4W.. k..(./.(...
00004870 00 17 C0 BE 20 4E 07 A0 FF 4E 0B A0 00 00 00 1F .... N...N......
00004880 4E 63 C0 BE 2C 4E 07 08 17 06 BE 20 1F 00 1A 4F Nc..,N..... ...O
00004890 37 A6 00 00 00 06 5C 9A 26 17 C0 70 23 28 57 C0 7.....\.&..p#(W.
000048A0 68 63 00 17 0E BE 20 1F 08 DA 14 17 C6 BE 20 64 hc.... ....... d
000048B0 17 C6 64 BE 4C 4E 1B A6 00 00 00 09 5C 37 A6 00 ..d.LN......\7..
000048C0 00 00 04 5C 9A BB D6 1F C0 BE 48 0A BB CF 17 20 ...\......H.... 
000048D0 07 05 00 00 00 58 0A B8 EE EA B8 69 82 00 00 5F .....X.....i..._
000048E0 B8 D7 C5 18 D7 C5 14 D7 C5 10 BE C4 C5 08 7F AE ................
000048F0 C0 00 49 1A 7C A5 E8 92 00 12 00 82 00 00 DF B8 ..I.|...........
00004900 D7 C5 18 D7 C5 14 D7 C5 10 BE C4 C5 08 7F AE C0 ................
00004910 00 49 1A 7C A5 E8 92 00 12 00 82 F0 14 D7 C1 14 .I.|............
00004920 97 A1 00 01 1E B0 17 C0 10 03 A0 00 01 1E B0 57 ...............W
00004930 01 BE 08 C5 08 00 00 00 00 00 00 00 00 DA 06 DF ................
00004940 00 EA 04 5F 00 17 04 18 00 57 80 18 00 1F 08 0A ..._.....W......
00004950 07 BE 14 C6 08 08 5C A8 C0 01 1E B0 5F 78 00 BE ......\....._x..
00004960 08 C5 08 00 00 00 00 00 00 00 00 0A 80 83 17 A1 ................
00004970 00 01 1F FC 8F 78 00 BE 08 C5 08 43 40 00 00 00 .....x.....C@...
00004980 00 00 00 DA 81 21 BE 08 C5 08 40 24 00 00 00 00 .....!....@$....
00004990 00 00 CA 11 BE 04 A6 40 24 00 00 00 00 00 00 78 .......@$......x
000049A0 EA 81 2D BE 08 C5 08 3F F0 00 00 00 00 00 00 DA ..-....?........
000049B0 3F BE 04 C0 08 BE 30 60 00 BE 08 05 40 24 00 00 ?.....0`....@$..
000049C0 00 00 00 00 DA 20 BE 30 66 00 08 E3 63 08 00 BE ..... .0f...c...
000049D0 04 C0 08 BE 30 60 00 BE 08 05 40 24 00 00 00 00 ....0`....@$....
000049E0 00 00 CA 64 17 20 27 61 0C 9F 40 08 CA 45 1F C0 ...d. 'a..@..E..
000049F0 1C 0A 05 43 79 00 07 2D 00 01 1E B0 CA 80 9E 07 ...Cy..-........
00004A00 2D 00 01 1F F8 7A 08 57 A1 00 01 1F F8 BE 08 C5 -....z.W........
00004A10 08 00 00 00 00 00 00 00 00 0A 0A 07 35 00 01 1E ............5...
00004A20 C1 CA 08 94 A3 30 00 EA 2A 3E 2B C1 08 57 20 67 .....0..*>+..W g
00004A30 48 30 CE 1C 08 94 03 00 3E 03 20 BE 84 C0 08 BE H0......>. .....
00004A40 90 00 BE B0 A0 40 24 00 00 00 00 00 00 BE 04 16 .....@$.........
00004A50 08 47 31 CA 25 97 29 04 75 00 35 CA 3C 07 35 00 .G1.%.).u.5.<.5.
00004A60 01 1E B0 1A 1A 54 A5 31 C0 01 1E B0 8F 78 00 1F .....T.1.....x..
00004A70 C0 1C 0A 25 8F 28 EA 21 8F 30 EA BF 93 94 A3 30 ...%.(.!.0.....0
00004A80 00 8F 37 17 36 7C 8C 80 7C 00 CE 1C 80 7C 00 07 ..7.6|..|....|..
00004A90 05 00 00 00 39 6A 48 5C 68 00 17 A0 00 01 1E B0 ....9jH\h.......
00004AA0 92 0F 12 00 BE 08 C3 08 00 CA 12 BE 20 66 00 08 ............ f..
00004AB0 C3 63 08 00 BE 08 C3 08 00 DA 72 17 20 27 61 0C .c........r. 'a.
00004AC0 9F 40 08 CA 61 EA BF 29 BE 04 C6 70 78 BE 04 C0 .@..a..)...px...
00004AD0 78 BE 30 A0 40 24 00 00 00 00 00 00 BE 04 06 70 x.0.@$.........p
00004AE0 BE 08 C6 70 08 7A 63 BE 04 C0 08 BE 20 C0 78 3E ...p.zc..... .x>
00004AF0 2B 06 6C 27 80 6C 30 CE 1C 00 94 03 00 8F 30 3E +.l'.l0.......0>
00004B00 03 C0 6C BE 30 C0 78 BE 10 06 08 8F 78 00 BE 08 ..l.0.x.....x...
00004B10 C5 78 40 24 00 00 00 00 00 00 7A BE D4 BE 20 A6 .x@$......z... .
00004B20 40 24 00 00 00 00 00 00 78 EA BF BE             @$......x...    

;; fn00004B2C: 00004B2C
;;   Called from:
;;     00003A1D (in fn000039E8)
fn00004B2C proc
	enter	[r4,r5,r6,r7],8
	movd	20(fp),r7
	cmpqd	0,12(fp)
	bge	00004B78

l00004B38:
	cmpqd	0,16(fp)
	bge	00004B78

l00004B3D:
	movxbd	12(r7),r0
	andd	H'12,r0
	cmpqd	2,r0
	bne	00004B69

l00004B4B:
	cmpqd	0,8(r7)
	beq	00004B69

l00004B50:
	cmpd	4(r7),8(r7)
	bne	00004B7E

l00004B56:
	cmpqd	0,0(r7)
	bne	00004B7E

l00004B5B:
	movxbd	12(r7),r0
	andd	H'44,r0
	cmpqd	0,r0
	bne	00004B7E

l00004B69:
	movd	r7,tos
	jsr	fn00004F65
	adjspb	H'FC
	cmpqd	0,r0
	beq	00004B7E

l00004B78:
	movqd	0,r0

l00004B7A:
	exit	[r4,r5,r6,r7]
	ret	0

l00004B7E:
	movxbd	13(r7),r0
	movd	@00012190[r0:d],-4(fp)
	movd	16(fp),r0
	muld	12(fp),r0
	movd	r0,r6
	cmpd	8(r7),4(r7)
	blt	00004C11

l00004B9A:
	movxbd	12(r7),r0
	tbitd	2,r0
	bfs	00004BAF

l00004BA6:
	cmpd	r6,H'400
	blo	00004C11

l00004BAF:
	movd	r6,tos
	movd	8(fp),tos
	movxbd	13(r7),tos
	jsr	fn000055A4
	adjspb	H'F4
	movd	r0,r5
	movd	r5,r1
	cmpd	r1,r6
	beq	00004BD8

l00004BC9:
	sbitb	5,12(r7)
	cmpqd	0,r5
	bgt	00004BD6

l00004BD2:
	movd	r5,r5
	br	00004BD8

l00004BD6:
	movqd	0,r5

l00004BD8:
	movd	r5,r0
	quod	12(fp),r0
	br	00004B7A

l00004BE1:
	movd	r7,tos
	jsr	fn00004ED8
	adjspb	H'FC
	cmpqd	-1,r0
	bne	00004C11

l00004BF0:
	movd	12(fp),tos
	movd	r6,r0
	addd	12(fp),r0
	addqd	-1,r0
	movd	r0,tos
	jsr	fn0000556C
	adjspb	H'F8
	movd	r0,-8(fp)
	movd	16(fp),r0
	subd	-8(fp),r0
	br	00004B7A

l00004C11:
	movd	4(r7),r4
	movd	r4,r0
	movd	-4(fp),r1
	subd	r0,r1
	movd	r1,r5
	movd	r5,r0
	cmpqd	0,r0
	bge	00004BE1

l00004C23:
	cmpd	r6,r5
	bhs	00004C2B

l00004C27:
	movd	r6,r5
	br	00004C2D

l00004C2B:
	movd	r5,r5

l00004C2D:
	movd	r5,tos
	movd	8(fp),tos
	movd	r4,tos
	jsr	fn000054D4
	adjspb	H'F4
	subd	r5,0(r7)
	addd	r5,4(r7)
	movxbd	13(r7),r0
	movd	@00012190[r0:d],r1
	subd	4(r7),r1
	cmpqd	0,0(r7)
	ble	00004C5A

l00004C56:
	movqd	0,r0
	br	00004C5D

l00004C5A:
	movd	0(r7),r0

l00004C5D:
	cmpd	r1,r0
	bge	00004C6C

l00004C61:
	movd	r7,tos
	jsr	fn000050AA
	adjspb	H'FC

l00004C6C:
	subd	r5,r6
	cmpqd	0,r6
	bne	00004CBA

l00004C73:
	movxbd	12(r7),r0
	andd	H'44,r0
	cmpqd	0,r0
	beq	00004CB4

l00004C81:
	movxbd	12(r7),r0
	tbitd	2,r0
	bfs	00004CA9

l00004C8D:
	movd	16(fp),r0
	muld	12(fp),r0
	movd	r0,tos
	addr	@0000000A,tos
	movd	8(r7),tos
	jsr	fn000054A8
	adjspb	H'F4
	cmpqd	0,r0
	beq	00004CB4

l00004CA9:
	movd	r7,tos
	jsr	fn00004ED8
	adjspb	H'FC

l00004CB4:
	movd	16(fp),r0
	br	00004B7A

l00004CBA:
	addd	r5,8(fp)
	br	00004C11

;; fn00004CC0: 00004CC0
;;   Called from:
;;     000055C4 (in fn000055C4)
fn00004CC0 proc
	enter	[r7],0
	movd	H'1204C,r7
	cmpd	r7,@0001218C
	bge	00004CE7

l00004CD1:
	movd	r7,tos
	jsr	fn00004CEB
	adjspb	H'FC
	addr	16(r7),r7
	cmpd	r7,@0001218C
	blt	00004CD1

l00004CE7:
	exit	[r7]
	ret	0

;; fn00004CEB: 00004CEB
;;   Called from:
;;     00003676 (in fn0000366E)
;;     00004CD3 (in fn00004CC0)
fn00004CEB proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movqd	-1,r6
	cmpqd	0,r7
	beq	00004D61

l00004CF8:
	movxbd	12(r7),r0
	andd	H'83,r0
	cmpqd	0,r0
	beq	00004D3C

l00004D06:
	movxbd	12(r7),r0
	tbitd	2,r0
	bfc	00004D16

l00004D12:
	movqd	0,r6
	br	00004D23

l00004D16:
	movd	r7,tos
	jsr	fn00004D67
	adjspb	H'FC
	movd	r0,r6

l00004D23:
	movxbd	13(r7),tos
	jsr	fn000050E0
	adjspb	H'FC
	cmpqd	0,r0
	ble	00004D3C

l00004D34:
	movqd	-1,r6
	movqd	2,@00012200

l00004D3C:
	movxbd	12(r7),r0
	tbitd	3,r0
	bfc	00004D57

l00004D48:
	movd	8(r7),tos
	jsr	fn000053C5
	adjspb	H'FC
	movqd	0,8(r7)

l00004D57:
	movqb	0,12(r7)
	movqd	0,0(r7)
	movd	8(r7),4(r7)

l00004D61:
	movd	r6,r0
	exit	[r6,r7]
	ret	0

;; fn00004D67: 00004D67
;;   Called from:
;;     0000086B (in fn0000079D)
;;     00002436 (in fn0000236C)
;;     00002644 (in fn0000263C)
;;     0000312E (in fn000030D0)
;;     00004D18 (in fn00004CEB)
fn00004D67 proc
	enter	[r7],0
	movd	8(fp),r7
	movxbd	12(r7),r0
	tbitd	1,r0
	bfs	00004D8A

l00004D79:
	movqd	0,0(r7)
	br	00004DBF

l00004D7F:
	movd	r7,tos
	jsr	fn00004ED8
	adjspb	H'FC

l00004D8A:
	movxbd	12(r7),r0
	tbitd	2,r0
	bfs	00004DAD

l00004D96:
	movxbd	12(r7),r0
	tbitd	1,r0
	bfc	00004DAD

l00004DA2:
	cmpqd	0,8(r7)
	beq	00004DAD

l00004DA7:
	cmpd	4(r7),8(r7)
	bgt	00004D7F

l00004DAD:
	movxbd	12(r7),r0
	tbitd	5,r0
	bfc	00004DBF

l00004DB9:
	movqd	-1,r0

l00004DBB:
	exit	[r7]
	ret	0

l00004DBF:
	movqd	0,r0
	br	00004DBB
00004DC3          82 80 04 D7 C1 0C CE 1C 78 0C 2B A0 00    ........x.+..
00004DD0 00 00 52 07 05 00 00 00 42 1A 3B CE 1C 78 0D 87 ..R.....B.;..x..
00004DE0 7F A8 04 C0 01 21 90 DA 80 87 17 78 04 8F 78 04 .....!.....x..x.
00004DF0 14 C2 08 00 CE 58 40 00 07 0D 00 00 00 0A 1A 80 .....X@.........
00004E00 D4 D7 3D 7F AE C0 00 4E D8 7C A5 FC 9F 07 1A 80 ..=....N.|......
00004E10 C4 EA 80 BB CE 1C 78 0C 2B A0 00 00 00 16 1F 03 ......x.+.......
00004E20 0A 80 72 CE 1C 78 0C 2B A0 00 00 00 12 1F 01 1A ..r..x.+........
00004E30 20 1F 78 08 0A 1B C7 7B 04 08 1A 25 1F 78 00 1A  .x....{...%.x..
00004E40 20 CE 1C 78 0C 2B A0 00 00 00 44 1F 00 1A 12 D7  ..x.+....D.....
00004E50 3D 7F AE C0 00 4F 65 7C A5 FC 1F 00 1A 80 70 CE =....Oe|......p.
00004E60 1C 78 0C 2B A0 00 00 00 44 1F 00 1A BF 5E D7 3D .x.+....D....^.=
00004E70 7F AE C0 00 4E D8 7C A5 FC 8F 7F 00 1F 78 00 7A ....N.|......x.z
00004E80 37 D7 3D CE D8 C5 08 7F AE C0 00 4D C3 7C A5 F8 7.=........M.|..
00004E90 EA 30 14 C6 08 7F 5F 78 00 DF B8 E7 C5 7F CE DC .0...._x........
00004EA0 7D 0D 7F AE C0 00 55 A4 7C A5 F4 9F 00 0A 25 4E }.....U.|.....%N
00004EB0 D8 A3 05 0C EA 18 17 78 04 8F 78 04 14 C2 08 00 .......x..x.....
00004EC0 CE 1C 78 0C 37 A0 00 00 00 05 9A 08 DF 07 92 01 ..x.7...........
00004ED0 12 00 CE 18 C0 08 EA 78                         .......x        

;; fn00004ED8: 00004ED8
;;   Called from:
;;     00003B78 (in fn00003A45)
;;     00004BE3 (in fn00004B2C)
;;     00004CAB (in fn00004B2C)
;;     00004D81 (in fn00004D67)
fn00004ED8 proc
	enter	[r5,r6,r7],4
	movd	8(fp),r7
	movd	8(r7),r6
	movd	r6,r0
	movd	4(r7),r1
	subd	r0,r1
	movd	r1,r5
	movd	r6,4(r7)
	movxbd	12(r7),r0
	andd	H'44,r0
	cmpqd	0,r0
	beq	00004F00

l00004EFB:
	movqd	0,0(r7)
	br	00004F11

l00004F00:
	movxbd	13(r7),r0
	movd	@00012190[r0:d],r1
	subd	8(r7),r1
	movd	r1,0(r7)

l00004F11:
	movxbd	13(r7),r0
	movd	@00012190[r0:d],r1
	subd	4(r7),r1
	cmpqd	0,0(r7)
	ble	00004F28

l00004F24:
	movqd	0,r0
	br	00004F2B

l00004F28:
	movd	0(r7),r0

l00004F2B:
	cmpd	r1,r0
	bge	00004F3A

l00004F2F:
	movd	r7,tos
	jsr	fn000050AA
	adjspb	H'FC

l00004F3A:
	cmpqd	0,r5
	bge	00004F61

l00004F3E:
	movd	r5,tos
	movd	r6,tos
	movxbd	13(r7),tos
	jsr	fn000055A4
	adjspb	H'F4
	movd	r0,-4(fp)
	cmpd	r5,r0
	beq	00004F61

l00004F56:
	sbitb	5,12(r7)
	movqd	-1,r0

l00004F5D:
	exit	[r5,r6,r7]
	ret	0

l00004F61:
	movqd	0,r0
	br	00004F5D

;; fn00004F65: 00004F65
;;   Called from:
;;     00004B6B (in fn00004B2C)
fn00004F65 proc
	enter	[r7],0
	movd	8(fp),r7
	movxbd	12(r7),r0
	andd	H'12,r0
	cmpqd	2,r0
	beq	00004FA5

l00004F79:
	movxbd	12(r7),r0
	andd	H'82,r0
	cmpqd	0,r0
	bne	00004F8D

l00004F87:
	movqd	-1,r0

l00004F89:
	exit	[r7]
	ret	0

l00004F8D:
	movxbd	12(r7),r0
	cbitd	4,r0
	sbitd	1,r0
	movxbd	r0,r0
	movb	r0,12(r7)

l00004FA5:
	cmpqd	0,8(r7)
	bne	00004FB5

l00004FAA:
	movd	r7,tos
	jsr	fn00005009
	adjspb	H'FC

l00004FB5:
	cmpd	4(r7),8(r7)
	bne	00005004

l00004FBC:
	movxbd	12(r7),r0
	andd	H'44,r0
	cmpqd	0,r0
	bne	00005004

l00004FCA:
	movxbd	13(r7),r0
	movd	@00012190[r0:d],r1
	subd	8(r7),r1
	movd	r1,0(r7)
	movxbd	13(r7),r0
	movd	@00012190[r0:d],r1
	subd	4(r7),r1
	cmpqd	0,0(r7)
	ble	00004FF2

l00004FEE:
	movqd	0,r0
	br	00004FF5

l00004FF2:
	movd	0(r7),r0

l00004FF5:
	cmpd	r1,r0
	bge	00005004

l00004FF9:
	movd	r7,tos
	jsr	fn000050AA
	adjspb	H'FC

l00005004:
	movqd	0,r0
	br	00004F89

;; fn00005009: 00005009
;;   Called from:
;;     000030DD (in fn000030D0)
;;     00004FAC (in fn00004F65)
fn00005009 proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movxbd	13(r7),r6
	movxbd	12(r7),r0
	tbitd	2,r0
	bfs	00005087

l00005020:
	cmpqd	2,r6
	ble	0000502E

l00005024:
	movd	@00012044[r6:d],8(r7)
	br	0000504B

l0000502E:
	addr	@00000408,tos
	jsr	fn00005130
	adjspb	H'FC
	movd	r0,8(r7)
	movd	8(r7),r1
	cmpqd	0,r1
	beq	00005087

l00005046:
	sbitb	3,12(r7)

l0000504B:
	movd	8(r7),r0
	addr	1024(r0),r0
	movxbd	13(r7),r1
	movd	r0,@00012190[r1:d]

l0000505D:
	movd	8(r7),4(r7)
	movd	r6,tos
	jsr	fn000050F4
	adjspb	H'FC
	cmpqd	0,r0
	beq	00005083

l00005070:
	movxbd	12(r7),r0
	andd	4,r0
	cmpqd	0,r0
	bne	00005083

l0000507E:
	sbitb	6,12(r7)

l00005083:
	exit	[r6,r7]
	ret	0

l00005087:
	movd	r6,r0
	ashd	3,r0
	addd	H'14CCC,r0
	movd	r0,8(r7)
	movd	8(r7),r1
	addr	8(r1),r1
	movxbd	13(r7),r0
	movd	r1,@00012190[r0:d]
	br	0000505D

;; fn000050AA: 000050AA
;;   Called from:
;;     00002355 (in fn000022B4)
;;     00003A0C (in fn000039E8)
;;     00003B38 (in fn00003A45)
;;     00004C63 (in fn00004B2C)
;;     00004F31 (in fn00004ED8)
;;     00004FFB (in fn00004F65)
fn000050AA proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movxbd	13(r7),r0
	movd	@00012190[r0:d],r1
	subd	4(r7),r1
	movd	r1,r6
	movd	r6,r0
	cmpqd	0,r0
	ble	000050D6

l000050C6:
	movxbd	13(r7),r0
	movd	@00012190[r0:d],4(r7)

l000050D2:
	exit	[r6,r7]
	ret	0

l000050D6:
	cmpd	r6,0(r7)
	bge	000050D2

l000050DB:
	movd	r6,0(r7)
	br	000050D2

;; fn000050E0: 000050E0
;;   Called from:
;;     00000D86 (in fn00000B29)
;;     00000EC0 (in fn00000B29)
;;     00000F6F (in fn00000B29)
;;     00000FDA (in fn00000B29)
;;     00001140 (in fn00000FF5)
;;     000017F1 (in fn000017CF)
;;     00001897 (in fn000017CF)
;;     000018A3 (in fn000017CF)
;;     00004D27 (in fn00004CEB)
fn000050E0 proc
	addr	@00000006,r0
	addr	4(sp),r1
	svc
	bfc	000050EF

l000050E9:
	jump	@000055B8

l000050EF:
	movqd	0,r0
	ret	0
000050F3          F2                                        .            

;; fn000050F4: 000050F4
;;   Called from:
;;     00005063 (in fn00005009)
fn000050F4 proc
	enter	[],H'14
	addr	-18(fp),tos
	movxwd	H'5401,tos
	movd	8(fp),tos
	jsr	fn0000511C
	adjspb	H'F4
	cmpqd	0,r0
	ble	00005115

l0000510F:
	movqd	0,r0

l00005111:
	exit	[]
	ret	0

l00005115:
	movqd	1,r0
	br	00005111
00005119                            F2 F2 F2                      ...    

;; fn0000511C: 0000511C
;;   Called from:
;;     00005102 (in fn000050F4)
fn0000511C proc
	addr	@00000036,r0
	addr	4(sp),r1
	svc
	bfc	0000512B

l00005125:
	jump	@000055B8

l0000512B:
	ret	0
0000512D                                        F2 F2 F2              ...

;; fn00005130: 00005130
;;   Called from:
;;     00000D93 (in fn00000B29)
;;     00005032 (in fn00005009)
fn00005130 proc
	enter	[r4,r5,r6,r7],H'C
	movqd	0,-4(fp)
	cmpqd	0,@000121E4
	bne	00005183

l0000513F:
	movd	H'121E8,r0
	sbitd	0,r0
	movd	r0,@000121E4
	movd	H'121E4,r0
	sbitd	0,r0
	movd	r0,@000121E8
	movd	H'121E8,@000121F0
	movd	H'121E4,@000121EC
	movd	@000121F0,@000121F8

l00005183:
	movqd	4,tos
	movd	8(fp),r0
	addqd	7,r0
	movd	r0,tos
	jsr	fn0000556C
	adjspb	H'F8
	movd	r0,r5
	movd	@000121EC,r7
	br	00005200

l000051A0:
	movd	r6,@000121F8
	movd	@000121F0,r0
	movd	r6,0(r0)
	movd	r6,r0
	addd	-8(fp),r0
	subd	-4(fp),r0
	addqd	-4,r0
	movd	r0,0(r6)
	movd	@000121F0,r0
	addqd	4,r0
	cmpd	r6,r0
	beq	000051E1

l000051C8:
	movd	@000121F0,r0
	movd	0(r0),r1
	sbitd	0,r1
	movd	@000121F0,r0
	movd	r1,0(r0)

l000051E1:
	movd	0(r6),@000121F0
	movd	H'121E4,r0
	sbitd	0,r0
	movd	@000121F0,r1
	movd	r0,0(r1)
	movd	r7,r6

l00005200:
	movqd	0,r4
	br	000053B9

l00005205:
	cmpd	r6,@000121F0
	bne	00005309

l0000520E:
	cmpd	r7,H'121E4
	bne	00005309

l00005217:
	addqd	1,r4
	movd	r4,r0
	cmpqd	1,r0
	bhs	000053B9

l00005220:
	movd	@000121F8,r7
	movqd	0,tos
	jsr	fn000054F8
	adjspb	H'FC
	movd	r0,r6
	movd	@000121F0,r0
	addqd	4,r0
	cmpd	r6,r0
	beq	0000528B

l00005240:
	addr	255(r5),r0
	quod	H'100,r0
	movd	r0,r4
	movd	r4,r0
	lshd	H'A,r0
	movd	r0,r4
	movqd	4,tos
	movd	r6,tos
	jsr	fn0000557C
	adjspb	H'F8
	cmpqd	0,r0
	beq	000052BF

l00005267:
	movqd	4,tos
	movd	r6,tos
	jsr	fn0000557C
	adjspb	H'F8
	movd	r0,-12(fp)
	movqd	4,r0
	subd	-12(fp),r0
	movd	r0,-4(fp)
	movd	r6,r0
	addd	-4(fp),r0
	movd	r0,r6
	addd	-4(fp),r4
	br	000052BF

l0000528B:
	movd	@000121F0,r0
	subd	@000121F8,r0
	quod	4,r0
	movd	r5,r1
	subd	r0,r1
	movd	r1,r4
	addr	@00000100,tos
	addr	256(r4),tos
	jsr	fn0000556C
	adjspb	H'F8
	movd	r0,r4
	movd	r4,r0
	lshd	H'A,r0
	movd	r0,r4

l000052BF:
	movd	r6,r0
	addd	r4,r0
	addqd	0,r0
	cmpd	r0,r6
	blo	00005309

l000052CA:
	movd	r4,-8(fp)
	cmpd	r4,H'7FFFFC00
	bls	000052EE

l000052D5:
	movd	H'7FFFFC00,tos
	jsr	fn000054F8
	adjspb	H'FC
	cmpqd	-1,r0
	beq	00005309

l000052E8:
	subd	H'7FFFFC00,r4

l000052EE:
	movd	r4,tos
	jsr	fn000054F8
	adjspb	H'FC
	cmpqd	-1,r0
	bne	000051A0

l000052FE:
	movd	r6,tos
	jsr	fn0000552F
	adjspb	H'FC

l00005309:
	movqd	0,r0

l0000530B:
	exit	[r4,r5,r6,r7]
	ret	0

l0000530F:
	movd	r5,r0
	ashd	2,r0
	movd	r7,r1
	addd	r0,r1
	movd	r1,@000121EC
	cmpd	r6,r1
	ble	0000533A

l00005323:
	movd	@000121EC,r0
	movd	0(r0),@000121F4
	movd	@000121EC,r0
	movd	0(r7),0(r0)

l0000533A:
	movd	@000121EC,r0
	sbitd	0,r0
	movd	r0,0(r7)
	cmpd	@000121F8,r7
	bne	0000535C

l00005352:
	movd	@000121EC,@000121F8

l0000535C:
	movd	r7,r0
	addqd	4,r0
	br	0000530B

l00005363:
	movd	r7,@000121EC
	movd	0(r6),0(r7)
	cmpd	@000121F8,r6
	bne	0000537B

l00005375:
	movd	r7,@000121F8

l0000537B:
	movd	0(r7),r6
	movd	r6,r0
	tbitd	0,0(r0)
	bfc	00005363

l00005389:
	movd	r5,r0
	ashd	2,r0
	movd	r7,r1
	addd	r0,r1
	cmpd	r6,r1
	blt	000053A6

l00005397:
	movd	r5,r0
	ashd	2,r0
	movd	r7,r1
	addd	r0,r1
	cmpd	r1,r7
	bge	0000530F

l000053A6:
	movd	r7,r6
	movd	0(r7),r0
	cbitd	0,r0
	movd	r0,r7
	cmpd	r7,r6
	ble	00005205

l000053B9:
	tbitd	0,0(r7)
	bfs	000053A6

l000053C2:
	br	0000537B

;; fn000053C5: 000053C5
;;   Called from:
;;     00004D4B (in fn00004CEB)
fn000053C5 proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movd	r7,r6
	addqd	-4,r6
	movd	r6,@000121EC
	movd	0(r6),r0
	cbitd	0,r0
	movd	r0,0(r6)
	cmpd	r0,@000121F0
	bne	000053F0

l000053EA:
	movd	r6,@000121F8

l000053F0:
	exit	[r6,r7]
	ret	0
000053F4             82 F0 04 97 C1 08 B7 A3 00 00 00 00     ............
00005400 7C 9A 0E D7 C5 08 7F AE C0 00 53 C5 7C A5 FC 17 |.........S.|...
00005410 70 7C 23 30 CE 33 A0 00 00 00 04 17 06 7C D7 C5 p|#0.3.......|..
00005420 0C 7F AE C0 00 51 30 7C A5 FC D7 01 1F 38 0A 80 .....Q0|.....8..
00005430 72 07 3E 08 0A 80 6C 5F BA 17 C0 0C 8F 01 D7 05 r.>...l_........
00005440 7F AE C0 00 55 6C 7C A5 F8 17 01 07 26 7C BA 05 ....Ul|.....&|..
00005450 17 26 7C 17 C0 7C 4E 17 A0 02 D7 05 D7 C5 08 D7 .&|..|N.........
00005460 3D 7F AE C0 00 54 D4 7C A5 F4 57 01 87 29 DA 32 =....T.|..W..).2
00005470 17 20 4E 17 A0 02 57 28 43 00 87 09 CA 24 17 20 . N...W(C....$. 
00005480 4E 17 A0 02 57 28 43 00 63 30 CE 73 A0 00 00 00 N...W(C.c0.s....
00005490 04 4E 47 A0 02 17 28 03 08 17 AA C0 01 21 F4 00 .NG...(......!..
000054A0 17 38 92 0F 12 00 F2 F2                         .8......        

;; fn000054A8: 000054A8
;;   Called from:
;;     00003B69 (in fn00003A45)
;;     00004C9C (in fn00004B2C)
fn000054A8 proc
	enter	[r5,r6,r7],0
	movd	8(fp),r7
	movxbd	12(fp),r6
	movd	16(fp),r5
	br	000054C8

l000054B7:
	movd	r7,r0
	addqd	1,r7
	cmpb	0(r0),r6
	bne	000054C8

l000054C0:
	addqd	-1,r7
	movd	r7,r0

l000054C4:
	exit	[r5,r6,r7]
	ret	0

l000054C8:
	addqd	-1,r5
	cmpqd	0,r5
	ble	000054B7

l000054CE:
	movqd	0,r0
	br	000054C4
000054D2       F2 F2                                       ..            

;; fn000054D4: 000054D4
;;   Called from:
;;     00002321 (in fn000022B4)
;;     00003A35 (in fn000039E8)
;;     00003AFF (in fn00003A45)
;;     00004C34 (in fn00004B2C)
fn000054D4 proc
	enter	[r4,r5,r6,r7],0
	movd	8(fp),r7
	movd	12(fp),r6
	movd	16(fp),r5
	movd	r7,r4
	br	000054EC

l000054E4:
	movb	0(r6),0(r7)
	addqd	1,r6
	addqd	1,r7

l000054EC:
	addqd	-1,r5
	cmpqd	0,r5
	ble	000054E4

l000054F2:
	movd	r4,r0
	exit	[r4,r5,r6,r7]
	ret	0

;; fn000054F8: 000054F8
;;   Called from:
;;     00005228 (in fn00005130)
;;     000052DB (in fn00005130)
;;     000052F0 (in fn00005130)
fn000054F8 proc
	cmpqd	0,@000121FC
	bne	0000550A

l00005500:
	addr	@00015584,@000121FC

l0000550A:
	addd	@000121FC,4(sp)
	addr	@00000011,r0
	addr	4(sp),r1
	svc
	bfc	00005520

l0000551A:
	jump	@000055B8

l00005520:
	movd	@000121FC,r0
	movd	4(sp),@000121FC
	ret	0

;; fn0000552F: 0000552F
;;   Called from:
;;     00005300 (in fn00005130)
fn0000552F proc
	addr	@00000011,r0
	addr	4(sp),r1
	svc
	bfc	0000553E

l00005538:
	jump	@000055B8

l0000553E:
	movd	4(sp),@000121FC
	movqd	0,r0
	ret	0
00005549                            F2 F2 F2                      ...    

;; fn0000554C: 0000554C
;;   Called from:
;;     000026D3 (in fn000026BD)
;;     000037D1 (in fn000037C0)
fn0000554C proc
	enter	[r6,r7],0
	movd	8(fp),r7
	movd	r7,r0
	addqd	1,r0
	movd	r0,r6

l00005558:
	movd	r7,r0
	addqd	1,r7
	movxbd	0(r0),r1
	cmpqd	0,r1
	bne	00005558

l00005564:
	movd	r7,r0
	subd	r6,r0
	exit	[r6,r7]
	ret	0

;; fn0000556C: 0000556C
;;   Called from:
;;     000022D4 (in fn000022B4)
;;     000037FA (in fn000037C0)
;;     00004BFC (in fn00004B2C)
;;     0000518C (in fn00005130)
;;     000052AC (in fn00005130)
fn0000556C proc
	movd	4(sp),r0
	movqd	0,r1
	deid	8(sp),r0
	movd	r1,r0
	ret	0
00005579                            F2 F2 F2                      ...    

;; fn0000557C: 0000557C
;;   Called from:
;;     000037E3 (in fn000037C0)
;;     00005259 (in fn00005130)
;;     0000526B (in fn00005130)
fn0000557C proc
	movd	4(sp),r0
	movqd	0,r1
	deid	8(sp),r0
	ret	0
00005587                      F2                                .        

;; fn00005588: 00005588
;;   Called from:
;;     00001488 (in fn000013F9)
;;     00001865 (in fn000017CF)
;;     00003465 (in fn00003404)
fn00005588 proc
	addr	@00000007,r0
	addr	4(sp),r1
	svc
	bfc	00005597

l00005591:
	jump	@000055B8

l00005597:
	cmpqd	0,4(sp)
	beq	000055A0

l0000559C:
	movd	r1,0(4(sp))

l000055A0:
	ret	0
000055A2       F2 F2                                       ..            

;; fn000055A4: 000055A4
;;   Called from:
;;     000010D3 (in fn00000FF5)
;;     000010FA (in fn00000FF5)
;;     00001BDE (in fn00001BA2)
;;     00001C5A (in fn00001BA2)
;;     00001D43 (in fn00001D28)
;;     00004BB8 (in fn00004B2C)
;;     00004F46 (in fn00004ED8)
fn000055A4 proc
	addr	@00000004,r0
	addr	4(sp),r1
	svc
	bfc	000055B3

l000055AD:
	jump	@000055B8

l000055B3:
	ret	0
000055B5                F2 F2 F2                              ...        

l000055B8:
	movd	r0,@00012200
	movqd	-1,r0
	ret	0
000055C2       F2 F2                                       ..            

;; fn000055C4: 000055C4
;;   Called from:
;;     0000011C (in _entry)
;;     00001787 (in fn00001772)
;;     0000185A (in fn000017CF)
;;     000019FD (in fn000019F1)
fn000055C4 proc
	jsr	fn00004CC0
	movqd	1,r0
	addr	4(sp),r1
	svc
	bpt
	bpt
	bpt
	bpt
