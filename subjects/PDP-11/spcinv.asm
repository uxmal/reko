;;; Segment .text (0000)
0000 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
0020 00 02 00 02 00 00 00 00 A2 12 00 00 00 00 00 00 ................
0030 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00F0 FF 80 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0100 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...

;; fn0200: 0200
fn0200 proc
	mov	#0002,r2
	clr	r3
	mov	#0F9A,r0
	emt	#E9

l020C:
	emt	#E0
	bcs	020C

l0210:
	mov	r0,r1

l0212:
	emt	#E0
	bcs	0212

l0216:
	emt	#E0
	bcs	0216

l021A:
	cmp	r1,#0042
	beq	0236

l0220:
	dec	r2
	inc	r3
	cmp	r1,#0049
	beq	0240

l022A:
	dec	r2
	inc	r3
	cmp	r1,#0045
	beq	0240

l0234:
	br	0200

l0236:
	mov	#0FDA,r0
	emt	#E9

l023C:
	emt	#E0
	bcs	023C

l0240:
	mov	r2,0CC2(pc)
	mov	r3,0CC0(pc)
	bis	#1040,@#0024
	mov	#1166,r0
	mov	#1100,@r0
	mov	#1170,0002(r0)
	emt	#FD
	mov	0F10(pc),0850(pc)
	clr	08F6(pc)
	mov	#1166,r0
	mov	#0101,@r0
	mov	#115E,0002(r0)
	emt	#FD
	bcs	029E

l027A:
	mov	#1166,r0
	mov	#0801,@r0
	clr	0002(r0)
	mov	#0B5E,0004(r0)
	mov	#0001,0006(r0)
	clr	0008(r0)
	emt	#FD
	mov	#0601,r0
	emt	#FC

l029E:
	jsr	pc,0934(pc)

l02A2:
	jsr	pc,097A(pc)

l02A6:
	emt	#E0
	bcs	02FC

l02AA:
	tst	0C4E(pc)
	bne	02FC

l02B0:
	movb	r0,0017(pc)
	mov	#02C6,r1

l02B8:
	cmp	r0,(r1)+
	bne	02B8

l02BC:
	sub	#02C7,r1
	asl	r1
	jmp	@02CC(r1)
02C6                   2C 2E 20 51 1B 00 D8 02 E0 02       ,. Q......
02D0 E8 02 F2 02 AE 03 F8 02 F7 15 64 04 1C 0C 0E 01 ..........d.....
02E0 F7 15 56 04 14 0C 0A 01 37 0A 0E 0C F7 09 B2 03 ..V.....7.......
02F0 05 01 F7 00 30 0C 02 01 37 0A FE 0B             ....0...7...   

l02FC:
	mov	#1166,r0
	mov	#1100,@r0
	mov	#1170,0002(r0)
	emt	#FD
	mov	0E62(pc),r0
	sub	0E60(pc),r0
	bmi	0388

l0316:
	mov	0E58(pc),0E58(pc)
	add	#0001,0E52(pc)
	com	0BD0(pc)
	bne	032C

l0328:
	com	0BCC(pc)

l032C:
	tst	0BCC(pc)
	beq	0352

l0332:
	dec	0BC6(pc)
	bne	0370

l0338:
	tst	0BDE(pc)
	beq	03AE

l033E:
	jsr	pc,0144(pc)
	mov	#0002,0BCE(pc)
	jsr	pc,0124(pc)
	mov	#0078,0BC6(pc)

l0352:
	tst	0BA4(pc)
	bne	036C

l0358:
	cmp	0BBA(pc),#0008
	bcc	0370

l0360:
	dec	0BB4(pc)
	bgt	0370

l0366:
	mov	#0456,0B8E(pc)

l036C:
	jsr	pc,@0B8A(pc)

l0370:
	jsr	pc,012C(pc)
	jsr	pc,035E(pc)
	jsr	pc,042A(pc)
	jsr	pc,0618(pc)
	cmp	r4,#1178
	beq	0394

l0386:
	br	038E

l0388:
	cmp	r4,#1178
	beq	02A6

l038E:
	jsr	pc,0764(pc)

l0392:
	br	02A6

l0394:
	tst	0B7C(pc)
	bne	02A6

l039A:
	tst	0B66(pc)
	bne	0392

l03A0:
	tst	0B76(pc)
	ble	03AE

l03A6:
	inc	0B70(pc)
	jmp	FEF4(pc)

l03AE:
	cmp	07AA(pc),07AA(pc)
	blos	040A

l03B6:
	mov	07A2(pc),07A2(pc)
	mov	#1166,r0
	mov	#0101,@r0
	mov	#115E,0002(r0)
	emt	#FD
	bcc	03E6

l03CE:
	mov	#1166,r0
	mov	#0201,@r0
	mov	#115E,0002(r0)
	mov	#0001,0004(r0)
	emt	#FD
	bcs	040A

l03E6:
	mov	#1166,r0
	mov	#0901,@r0
	clr	0002(r0)
	mov	#0B5E,0004(r0)
	mov	#0001,0006(r0)
	clr	0008(r0)
	emt	#FD
	mov	#0601,r0
	emt	#FC

l040A:
	jsr	pc,0078(pc)
	movb	#0001,-(sp)
	movb	#0018,0001(sp)
	jsr	pc,069A(pc)
	jsr	r5,06C8(pc)
	mov	r4,-(r1)
	jsr	r5,06C2(pc)
	illegal
	mov	#0028,0AE8(pc)
	jsr	pc,06C4(pc)
	emt	#E0
	bcs	0432
	cmp	r0,#001B
	beq	0446
	cmp	r0,#000D
	bne	0432
	jmp	FE58(pc)
	mov	#1178,r4
	jsr	r5,069A(pc)
	mov	r4,(r1)+
	jsr	pc,06A2(pc)
	emt	#E8
	cmp	0ABC(pc),#0047
	bcc	049A
	inc	0AB4(pc)
	br	0470
	cmp	0AAE(pc),#0008
	blos	049A
	dec	0AA6(pc)

;; fn0470: 0470
fn0470 proc
	movb	0AA2(pc),-(sp)
	movb	#0018,0001(sp)
	jsr	pc,0638(pc)
	jsr	r5,0666(pc)
	mov	r4,-(r2)
	rts	pc

;; fn0486: 0486
fn0486 proc
	movb	0A8C(pc),-(sp)
	movb	#0018,0001(sp)
	jsr	pc,0622(pc)
	jsr	pc,05DC(pc)
	rts	pc
049A                               37 0A 5C 0A 87 00           7.\...

;; fn04A0: 04A0
fn04A0 proc
	bit	0A52(pc),0A52(pc)
	beq	04AA

l04A8:
	rts	pc

l04AA:
	mov	0A58(pc),r5

l04AE:
	movb	0EF0(r5),r3
	beq	0588

l04B4:
	mov	r3,0A54(pc)
	movb	0EF3(r5),r0
	movb	r3,-(sp)
	movb	r0,0001(sp)
	jsr	pc,05F0(pc)
	movb	#0020,(r4)+
	dec	r0
	tst	0A34(pc)
	bgt	04D8

l04D2:
	cmp	r0,#0004
	blos	0584

l04D8:
	cmp	r0,#0002
	beq	0584

l04DE:
	movb	r0,0EF3(r5)
	cmp	r0,#0016
	bcs	04EE

l04E8:
	jsr	pc,0190(pc)
	bne	0584

l04EE:
	mov	r0,r1
	inc	r1
	cmp	r0,#0003
	bne	0504

l04F8:
	tst	0A08(pc)
	ble	0504

l04FE:
	jsr	pc,00D2(pc)
	beq	0584

l0504:
	jsr	pc,0142(pc)
	beq	0584

l050A:
	clr	r2

l050C:
	cmp	r0,0DB8(r2)
	beq	0524

l0512:
	cmp	r1,0DB8(r2)
	bcs	05B4

l0518:
	beq	0524

l051A:
	tst	(r2)+
	cmp	r2,#000A
	ble	050C

l0522:
	br	05B4

l0524:
	mov	0DAC(r2),r0
	mov	#0008,r1

l052C:
	mov	@r0,r3
	beq	05AE

l0530:
	blt	058A

l0532:
	add	#0004,r3
	cmp	0EF0(r5),r3
	bhi	05AE

l053C:
	cmp	0EF0(r5),@r0
	bcs	05B4

l0542:
	mov	@r0,r1
	bis	#8000,@r0
	sub	0DAC(r2),r0
	mov	r0,09BA(pc)
	dec	0DC4(r0)
	movb	r1,-(sp)
	movb	0DB8(r2),0001(sp)
	jsr	pc,0556(pc)
	jsr	pc,0518(pc)
	add	0DD4(r2),05F2(pc)
	jsr	pc,05AC(pc)
	dec	09A2(pc)
	bgt	0584

l0574:
	tst	098C(pc)
	bgt	0584

l057A:
	clr	097C(pc)
	mov	#0005,0978(pc)

l0584:
	clrb	0EF0(r5)

l0588:
	br	05CA

l058A:
	movb	@r0,-(sp)
	movb	0DB8(r2),0001(sp)
	jsr	pc,0520(pc)
	jsr	pc,04DA(pc)
	clr	@r0
	movb	0EF0(r5),r3
	inc	r3
	movb	r3,-(sp)
	movb	0EF3(r5),0001(sp)
	jsr	pc,0508(pc)

l05AE:
	tst	(r0)+
	dec	r1
	bne	052C

l05B4:
	jsr	r5,0530(pc)
	mov	r4,(sp)+
	jsr	r5,052A(pc)
	mov	r4,-(r1)
	movb	0B97(pc),(r4)+
	jsr	r5,0520(pc)
	mov	r4,-(r1)

l05CA:
	dec	r5
	blt	05D2

l05CE:
	jmp	FEDC(pc)

l05D2:
	rts	pc

;; fn05D4: 05D4
fn05D4 proc
	mov	r3,-(sp)
	sub	092A(pc),@sp
	cmp	(sp)+,#0004
	bhi	0648

l05E0:
	movb	0920(pc),-(sp)
	movb	r0,0001(sp)
	jsr	pc,04CA(pc)
	jsr	pc,048C(pc)
	jsr	pc,04A0(pc)
	clr	r3
	asl	r0
	rol	r3
	asl	r0
	rol	r3
	asl	r3
	mov	0F2A(r3),r0
	asr	090A(pc)
	bcs	060E

l060A:
	add	#00C8,r0

l060E:
	add	r0,054A(pc)
	movb	08EE(pc),-(sp)
	movb	#0004,0001(sp)
	jsr	pc,0496(pc)
	jsr	r5,04C4(pc)
	mov	r4,-(r1)
	movb	#0028,(r4)+
	mov	#0003,r3
	jsr	pc,052E(pc)
	movb	#0029,(r4)+
	jsr	r5,04AE(pc)
	mov	r4,-(r1)
	jsr	pc,04DA(pc)
	movb	#FFFF,08BF(pc)
	setflags	#04

l0648:
	rts	pc

;; fn064A: 064A
fn064A proc
	mov	08BA(pc),r2

l064E:
	cmp	r3,0EE6(r2)
	bne	0676

l0654:
	tst	0EEC(r2)
	bmi	0676

l065A:
	cmp	r1,0EE9(r2)
	beq	0670

l0660:
	cmp	r0,0EE9(r2)
	bne	0676

l0666:
	jsr	r5,047E(pc)
	mov	r4,(sp)+
	movb	#0020,(r4)+

l0670:
	clrb	0EE6(r2)
	br	067A

l0676:
	dec	r2
	bge	064E

l067A:
	rts	pc

;; fn067C: 067C
fn067C proc
	mov	r3,r1
	dec	r1
	asl	r1
	add	r0,r1
	tst	0E2A(r1)
	beq	06A0

l068A:
	dec	0E2A(r1)
	movb	0E2A(r1),r1
	movb	r3,-(sp)
	movb	r0,0001(sp)
	jsr	pc,041A(pc)
	movb	0EE0(r1),(r4)+

l06A0:
	rts	pc

;; fn06A2: 06A2
fn06A2 proc
	tst	0866(pc)
	beq	06D4

l06A8:
	mov	085A(pc),r5

l06AC:
	tst	0EF0(r5)
	bne	06D0

l06B2:
	mov	0860(pc),r0
	cmp	r0,#0008
	bcs	06D4

l06BC:
	add	#0002,r0
	movb	r0,0EF0(r5)
	movb	#0018,0EF3(r5)
	clr	083E(pc)
	br	06D4

l06D0:
	dec	r5
	bge	06AC

l06D4:
	rts	pc

;; fn06D6: 06D6
fn06D6 proc
	mov	082E(pc),r2

l06DA:
	movb	0EE6(r2),r3
	beq	07A0

l06E0:
	bit	0812(pc),0812(pc)
	bne	06F6

l06E8:
	tst	0EEC(r2)
	ble	07A0

l06EE:
	cmp	0804(pc),0804(pc)
	beq	07A0

l06F6:
	movb	0EE9(r2),r0
	tst	0EEC(r2)
	bmi	0710

l0700:
	movb	r3,-(sp)
	movb	r0,0001(sp)
	jsr	pc,03AC(pc)
	movb	#0020,(r4)+
	br	0724

l0710:
	inc	r3
	movb	r3,-(sp)
	movb	r0,0001(sp)
	jsr	pc,039A(pc)
	dec	r3
	bic	#0080,0EEC(r2)

l0724:
	inc	0EE9(r2)
	inc	r0
	cmp	r0,#0019
	beq	0784

l0730:
	cmp	r0,#0018
	beq	0744

l0736:
	cmp	r0,#0016
	bcs	078A

l073C:
	jsr	pc,FF3C(pc)
	beq	078A

l0742:
	br	0784

l0744:
	tst	07B4(pc)
	bne	078A

l074A:
	cmp	r3,07C8(pc)
	bcs	078A

l0750:
	sub	#0004,r3
	cmp	r3,07BE(pc)
	bhi	078A

l075A:
	movb	07B8(pc),-(sp)
	movb	#0018,0001(sp)
	jsr	pc,034E(pc)
	jsr	pc,0310(pc)
	clr	078A(pc)
	mov	#0005,0786(pc)
	dec	07A0(pc)
	jsr	r5,036A(pc)
	mov	r4,-(r1)
	jsr	pc,03B6(pc)

l0784:
	clrb	0EE6(r2)
	br	07A0

l078A:
	jsr	r5,035A(pc)
	mov	r4,@(r3)+
	jsr	r5,0354(pc)
	mov	r4,-(r1)
	movb	09C2(pc),(r4)+
	jsr	r5,034A(pc)
	mov	r4,-(r1)

l07A0:
	dec	r2
	bge	06DA

l07A4:
	rts	pc

;; fn07A6: 07A6
fn07A6 proc
	dec	0774(pc)
	beq	07B0

l07AC:
	jmp	0102(pc)

l07B0:
	mov	0774(pc),0768(pc)
	clr	r5

l07B8:
	mov	0760(pc),r2
	mov	0DB8(r2),r0
	beq	0868

l07C2:
	tst	0738(pc)
	beq	07F4

l07C8:
	mov	0DAC(r2),r1
	mov	#0008,r3

l07D0:
	tst	@r1
	beq	07E8

l07D4:
	movb	@r1,-(sp)
	movb	r0,0001(sp)
	jsr	pc,02D8(pc)
	jsr	pc,0292(pc)
	tst	@r1
	bpl	07E8

l07E6:
	clr	@r1

l07E8:
	tst	(r1)+
	dec	r3
	bne	07D0

l07EE:
	inc	0DB8(r2)
	inc	r0

l07F4:
	mov	0DAC(r2),r1
	mov	#0008,r3

l07FC:
	tst	@r1
	ble	0840

l0800:
	add	071E(pc),@r1
	cmp	@r1,#0008
	blos	0810

l080A:
	cmp	@r1,#0048
	bcs	0814

l0810:
	mov	sp,06EC(pc)

l0814:
	cmp	r0,#0017
	bne	081E

l081A:
	mov	sp,06E4(pc)

l081E:
	cmp	r0,#0016
	bcs	0828

l0824:
	jsr	pc,0114(pc)

l0828:
	inc	r5
	movb	@r1,-(sp)
	movb	r0,0001(sp)
	jsr	pc,0282(pc)
	jsr	pc,0132(pc)
	jsr	r5,02AC(pc)
	mov	r4,@-(r3)
	br	085A

l0840:
	tst	@r1
	bge	085A

l0844:
	tst	06B6(pc)
	bne	085A

l084A:
	movb	@r1,-(sp)
	movb	r0,0001(sp)
	jsr	pc,0262(pc)
	jsr	pc,021C(pc)
	clr	@r1

l085A:
	tst	(r1)+
	dec	r3
	bne	07FC

l0860:
	tst	r5
	bne	0868

l0864:
	clr	0DB8(r2)

l0868:
	sub	#0002,06AE(pc)
	blt	087E

l0870:
	tst	r5
	bne	0878

l0874:
	jmp	FF40(pc)

l0878:
	mov	r5,06AC(pc)
	br	08B2

l087E:
	mov	#000A,0698(pc)
	com	0522(pc)
	tst	0672(pc)
	beq	0894

l088E:
	clr	066C(pc)
	br	08AA

l0894:
	tst	0668(pc)
	beq	08AA

l089A:
	neg	0684(pc)
	tst	0660(pc)
	bne	08AA

l08A4:
	mov	#0001,0654(pc)

l08AA:
	clr	0652(pc)
	clr	0650(pc)

l08B2:
	tst	065E(pc)
	beq	08CE

l08B8:
	bit	063A(pc),063A(pc)
	beq	08CE

l08C0:
	mov	0644(pc),r2

l08C4:
	tst	0EE6(r2)
	beq	08D0

l08CA:
	dec	r2
	bge	08C4

l08CE:
	rts	pc

l08D0:
	jsr	pc,01C0(pc)
	bit	0638(pc),r0
	bne	093A

l08DA:
	jsr	pc,01B6(pc)
	mov	0628(pc),r1
	bmi	08EE

l08E4:
	tst	0DC4(r1)
	ble	08EE

l08EA:
	asl	r0
	bcs	0904

l08EE:
	clr	r1
	asl	r0
	rol	r1
	asl	r0
	rol	r1
	asl	r0
	rol	r1
	asl	r1
	tst	0DC4(r1)
	ble	08D0

l0904:
	mov	#0DB8,r0

l0908:
	mov	-(r0),r3
	add	r1,r3
	tst	@r3
	ble	0908

l0910:
	movb	@r3,r3
	add	#0002,r3
	movb	r3,0EE6(r2)
	movb	000C(r0),0EE9(r2)
	movb	#0080,0EEC(r2)
	cmp	r0,#0DAC
	beq	0936

l092C:
	jsr	pc,0164(pc)
	bit	05DA(pc),r0
	bne	093A

l0936:
	inc	0EEC(r2)

l093A:
	rts	pc

;; fn093C: 093C
fn093C proc
	mov	r3,-(sp)
	mov	@r1,r3
	sub	#0001,r3
	tst	05DA(pc)
	bmi	094C

l094A:
	dec	r3

l094C:
	asl	r3
	add	r0,r3
	sub	#0016,r3
	mov	#0007,-(sp)

l0958:
	clrb	0E40(r3)
	add	#0002,r3
	dec	@sp
	bne	0958

l0964:
	tst	(sp)+
	mov	(sp)+,r3
	rts	pc

;; fn096A: 096A
fn096A proc
	bit	#0001,r3
	bne	0984

l0970:
	tst	0436(pc)
	bne	098A

l0976:
	movb	#002F,07B1(pc)
	movb	#005C,07AF(pc)
	rts	pc

l0984:
	tst	0422(pc)
	bne	0976

l098A:
	movb	#005C,079D(pc)
	movb	#002F,079B(pc)
	rts	pc

;; fn0998: 0998
fn0998 proc
	mov	0568(pc),r2
	bgt	09E4

l099E:
	dec	057E(pc)
	bne	0A1E

l09A4:
	mov	#0064,0576(pc)
	mov	#0001,r2
	mov	r2,0560(pc)
	mov	#1134,00B2(pc)
	clr	r1
	jsr	pc,00D6(pc)
	asl	r0
	bcc	09D4

l09C2:
	asl	r0
	bcc	09D4

l09C6:
	inc	0548(pc)
	mov	#113D,009A(pc)
	mov	#000A,r1

l09D4:
	tst	r0
	bpl	09E4

l09D8:
	neg	0536(pc)
	add	r1,008A(pc)
	mov	#004A,r2

l09E4:
	tst	050E(pc)
	bne	0A18

l09EA:
	tst	050A(pc)
	bne	0A18

l09F0:
	tst	051E(pc)
	bmi	09FE

l09F6:
	cmp	r2,#0049
	beq	0A2A

l09FC:
	br	0A04

l09FE:
	cmp	r2,#0002
	beq	0A2A

l0A04:
	add	050A(pc),r2
	movb	r2,-(sp)
	movb	#0003,0001(sp)
	jsr	pc,00A2(pc)
	jsr	pc,0048(pc)

l0A18:
	mov	r2,04E8(pc)
	rts	pc

l0A1E:
	tst	r2
	beq	0A5E

l0A22:
	cmp	04FA(pc),#0028
	bgt	0A5E

l0A2A:
	movb	r2,-(sp)
	movb	#0003,0001(sp)
	jsr	pc,0080(pc)
	jsr	pc,003A(pc)
	movb	r2,-(sp)
	movb	#0004,0001(sp)
	jsr	pc,0070(pc)
	jsr	pc,002A(pc)
	clr	04B6(pc)
	tst	04C2(pc)
	bgt	0A5E

l0A54:
	clr	04A2(pc)
	mov	#0005,049E(pc)

l0A5E:
	rts	pc

;; fn0A60: 0A60
fn0A60 proc
	jsr	r5,0084(pc)
	mov	r4,-(r1)
	jsr	r5,007E(pc)
	halt
0A6C                                     77 09 78 00             w.x.
0A70 21 11 87 00                                     !...           

;; fn0A74: 0A74
fn0A74 proc
	jsr	r5,0070(pc)
	mov	r5,(r5)+
	rts	pc

;; fn0A7C: 0A7C
fn0A7C proc
	movb	04A6(pc),(r4)+
	jsr	r5,0064(pc)
	mov	r4,-(r1)
	jsr	r5,005E(pc)
	mov	r5,@pc
	jsr	r5,0058(pc)
	mov	r4,-(r1)
	rts	pc

;; fn0A94: 0A94
fn0A94 proc
	mov	001C(pc),r0
	swab	r0
	clrb	r0
	asl	r0
	add	0012(pc),r0
	asl	r0
	asl	r0
	add	000A(pc),r0
	add	#3619,r0
	mov	r0,0002(pc)
	rts	pc
0AB4             00 00                                   ..         

;; fn0AB6: 0AB6
fn0AB6 proc
	jsr	r5,002E(pc)
	mov	r4,@(sp)+
	mov	r0,-(sp)
	mov	r3,-(sp)
	movb	0007(sp),r0
	mov	#FFFE,r3
	jsr	pc,0094(pc)
	movb	#003B,(r4)+
	movb	0006(sp),r0
	mov	#FFFE,r3
	jsr	pc,0084(pc)
	movb	#0048,(r4)+
	mov	(sp)+,r3
	mov	(sp)+,r0
	mov	(sp)+,@sp
	rts	pc

;; fn0AE8: 0AE8
fn0AE8 proc
	mov	r0,-(sp)
	mov	(r5)+,r0

l0AEC:
	movb	(r0)+,(r4)+
	bne	0AEC

l0AF0:
	dec	r4
	mov	(sp)+,r0
	rts	r5

;; fn0AF6: 0AF6
fn0AF6 proc
	mov	041C(pc),r0
	add	#0002,r0
	movb	r0,-(sp)
	movb	#0018,0001(sp)
	jsr	pc,FFAC(pc)
	movb	#0080,@r4
	mov	#1178,r0
	emt	#E9
	mov	#1178,r4
	rts	pc

;; fn0B1A: 0B1A
fn0B1A proc
	mov	003E(pc),r0
	mov	#0005,r3
	movb	#002B,-(sp)
	movb	#0002,0001(sp)
	jsr	pc,FF86(pc)
	jsr	r5,FFB4(pc)
	mov	r4,-(r1)
	jsr	pc,0026(pc)

;; fn0B3A: 0B3A
fn0B3A proc
	mov	03DC(pc),r0
	mov	#0003,r3
	movb	#001F,-(sp)
	movb	#0002,0001(sp)
	jsr	pc,FF66(pc)
	jsr	pc,000C(pc)
	jsr	r5,FF90(pc)
	mov	r4,-(r1)
	rts	pc
0B5C                                     00 00 00 00             ....

;; fn0B60: 0B60
fn0B60 proc
	mov	r1,-(sp)
	mov	r2,-(sp)
	mov	r5,-(sp)
	clr	-(sp)
	tst	r3
	bmi	0B74

l0B6C:
	mov	#0020,0056(pc)
	br	0B7C

l0B74:
	neg	r3
	mov	#0030,004C(pc)

l0B7C:
	mov	r3,r5
	beq	0BBE

l0B80:
	mov	#0BD4,r2
	asl	r5
	sub	r5,r2

l0B88:
	clr	r5
	mov	(r2)+,r1
	beq	0BBE

l0B8E:
	sub	r1,r0
	bcs	0B96

l0B92:
	inc	r5
	br	0B8E

l0B96:
	add	r1,r0
	tst	@sp
	bne	0BB4

l0B9C:
	tst	r5
	beq	0BA4

l0BA0:
	inc	@sp
	br	0BB4

l0BA4:
	cmp	r3,#0001
	beq	0BB4

l0BAA:
	tst	@r2
	beq	0BB4

l0BAE:
	movb	0016(pc),r5
	br	0BB8

l0BB4:
	add	#0030,r5

l0BB8:
	movb	r5,(r4)+
	dec	r3
	bne	0B88

l0BBE:
	tst	(sp)+
	mov	(sp)+,r5
	mov	(sp)+,r2
	mov	(sp)+,r1
	rts	pc
0BC8                         20 00 10 27 E8 03 64 00          ..'..d.
0BD0 0A 00 01 00 00 00                               ......         

;; fn0BD6: 0BD6
fn0BD6 proc
	mov	#0003,033E(pc)
	clr	FF7C(pc)
	mov	#F800,0328(pc)
	mov	#E000,0324(pc)
	mov	#0E56,r1
	mov	#0006,r0

l0BF4:
	mov	#000A,r2

l0BF8:
	movb	#0004,(r1)+
	dec	r2
	bne	0BF8

l0C00:
	add	#000A,r1
	dec	r0
	bne	0BF4

l0C08:
	mov	#1166,r0
	mov	#1100,@r0
	mov	#1170,0002(r0)
	emt	#FD
	mov	0556(pc),0556(pc)
	rts	pc

;; fn0C20: 0C20
fn0C20 proc
	mov	#0DB8,r1
	mov	#0006,r0
	mov	#0005,r2

l0C2C:
	mov	r2,(r1)+
	add	#0002,r2
	dec	r0
	bne	0C2C

l0C36:
	clr	r1
	mov	#0008,r0
	mov	#000B,r2

l0C40:
	mov	r2,0DE0(r1)
	mov	r2,0DF0(r1)
	mov	r2,0E00(r1)
	mov	r2,0E10(r1)
	mov	r2,0E20(r1)
	mov	r2,0E30(r1)
	mov	#0006,0DC4(r1)
	tst	(r1)+
	add	#0008,r2
	dec	r0
	bne	0C40

l0C68:
	mov	#0EE6,r1
	mov	#0010,r0

l0C70:
	clr	(r1)+
	dec	r0
	bne	0C70

l0C76:
	mov	#FFFF,028E(pc)
	mov	#0030,0292(pc)
	mov	#0002,028E(pc)
	mov	#0078,028A(pc)
	mov	#000A,0288(pc)
	mov	#0004,0284(pc)
	mov	#0004,0288(pc)
	mov	#0064,027A(pc)
	mov	#0001,0276(pc)
	asl	025E(pc)
	asl	025C(pc)
	bne	0CBA

l0CB6:
	asr	0256(pc)

l0CBA:
	mov	#1178,r4
	jsr	r5,FE26(pc)
	mov	r4,(r1)+
	jsr	r5,FE20(pc)
	mov	r4,-(r1)
	movb	#0002,-(sp)
	movb	#0018,0001(sp)
	jsr	pc,FDDE(pc)
	jsr	r5,FE0C(pc)
	mov	r4,-(r2)
	movb	#0002,-(sp)
	movb	#0002,0001(sp)
	jsr	pc,FDCA(pc)
	jsr	r5,FDF8(pc)
	illegal
	jsr	r5,FDF2(pc)
	mov	r4,-(r1)
	jsr	r5,FDEC(pc)
	illegal
	dec	r4
	jsr	pc,FDF2(pc)
	movb	#0001,-(sp)
	movb	#0016,0001(sp)
	jsr	pc,FDA4(pc)
	mov	#0E40,r0
	jsr	pc,007E(pc)
	movb	#0001,-(sp)
	movb	#0017,0001(sp)
	jsr	pc,FD8E(pc)
	mov	#0E41,r0
	jsr	pc,0068(pc)
	jsr	pc,FDC2(pc)
	jsr	pc,FDE2(pc)
	mov	FE22(pc),r0
	mov	#0005,r3
	movb	#003E,-(sp)
	movb	#0002,0001(sp)
	jsr	pc,FD68(pc)
	jsr	pc,FE0E(pc)
	jsr	pc,FDA0(pc)
	mov	#0DB8,r0
	mov	#0006,r2
	mov	004A(pc),r1
	mov	#0008,r3
	mov	r3,01A2(pc)
	movb	(r1)+,-(sp)
	movb	@r0,0001(sp)
	jsr	pc,FD42(pc)
	inc	r1
	jsr	pc,FBF0(pc)
	jsr	r5,FD6A(pc)
	mov	r4,@-(r3)
	dec	r3
	bne	0D6A
	mov	r0,-(sp)
	jsr	pc,FD6C(pc)
	mov	(sp)+,r0
	tst	(r0)+
	dec	r2
	bne	0D62
	com	0014(pc)
	rts	pc
	mov	#0046,r1
	movb	(r0)+,r2
	inc	r0
	movb	0EE0(r2),(r4)+
	dec	r1
	bne	0D9C
	rts	pc
	halt
	sxt	-(r0)
	sxt	0E00(r0)
	illegal
	illegal
	illegal
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	illegal
	illegal
	illegal
	illegal
	illegal
	reset
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	cmp	2A2B(r4),-(r0)
	illegal
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	rti
	halt
	sob	pc,0E8E
	wait
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	bge	0F36
	iot
	illegal
	jmp	-(r4)
	illegal
	swab	@r0
	bis	r1,(r3)+
	bic	@r5,r1
	cmp	r1,r5
	bic	@4156(r1),@r1
	bic	(r5)+,r4
	bis	@r5,(r2)+
	cmp	r0,-(r1)
	cmp	r0,-(r0)
	cmp	r0,-(r0)
	cmp	r0,-(r0)
	bic	@r0,r0
	div	-(r1),r5
	div	-(r5),r5
	cmp	r0,@2020(r2)
	cmp	r0,-(r0)
	bis	@r4,-(r0)
	add	@6572(r5),-(r3)
	cmp	r0,@2020(r2)
	cmp	r0,-(r0)
	cmp	r0,-(r0)
	cmp	r0,-(r0)
	bic	-(r0),-(r0)
	add	@(r5)+,@-(r1)
	add	(r5)+,@-(r0)
	ash	203A(r3),r1
	bis	r0,r0
	add	(r5)+,7373(r2)
	bis	@r0,-(r0)
	bis	(r1)+,r5
	bis	@r1,(r5)+
	cmp	r1,@sp
	add	@7020(r5),616C(r4)
	cmp	r1,@6761(r1)
	add	-(r5),-(r1)
	cmp	4520(r1),@-(sp)
	bic	@r5,(r3)+
	ash	-(r0),r0
	illegal
	ash	7469(r1),r5
	cmp	r0,@0020(r2)
	clr	@r5
	bis	r1,(r3)+
	bic	@r5,r1
	cmp	r1,r5
	bic	@4156(r1),@r1
	bic	(r5)+,r4
	bis	@r5,(r2)+
	cmp	r0,-(r1)
	bic	@r0,-(r0)
	add	@(r5)+,-(r5)
	add	@656E(r1),@-(r1)
	cmp	4920(r1),746E(r2)
	div	-(r5),r1
	add	(r5)+,@-(r5)
	add	-(r5),-(r4)
	ash	-(r1),r1
	cmp	6F20(r1),-(r5)
	cmp	r1,7845(r2)
	add	(r5)+,7472(r0)
	cmp	r0,@4228(pc)
	illegal
	illegal
	bit	@-(r0),@-(r1)
	bpl	101A
	clr	@r5
	bis	(r0)+,@r2
	add	(r5)+,@-(r0)
	cmp	@r0,-(r0)
	cmp	@r0,@-(r4)
	cmp	-(r0),-(r0)
	add	@6577(r5),@-(r4)
	add	@r5,7361(r2)
	cmp	r1,-(r5)
	bit	2922(r0),-(r2)
	add	@-(r4),-(r0)
	xor	-(r5),r5
	add	766F(r4),-(r0)
	div	-(r5),r5
	add	6665(r0),-(r0)
	bit	@-(r5),0A0D(r4)
	bis	(r0)+,@r2
	add	(r5)+,@-(r0)
	cmp	@r0,-(r0)
	cmp	@r0,@-(sp)
	cmp	-(r0),-(r0)
	add	@6577(r5),@-(r4)
	add	@r5,7361(r2)
	cmp	r1,-(r5)
	bit	@2922(r0),-(r2)
	add	@-(r4),-(r0)
	xor	-(r5),r5
	add	766F(r4),-(r0)
	div	-(r5),r5
	div	-(r0),r0
	add	@(r5)+,@-(r1)
	ash	@-(r0),r1
	mark	#3B
	clr	@r2
	div	(r0)+,r1
	div	-(r5),r5
	cmp	r1,6874(r3)
	cmp	r1,-(r5)
	mul	6361(r3),r1
	add	@r1,-(r5)
	div	-(r1),r1
	ash	-(r0),r0
	illegal
	ash	706F(r3),r1
	add	r4,-(r0)
	add	(r1)+,@-(sp)
	add	@(r0)+,-(r0)
	div	@-(r1),r1
	bit	@-(r5),-(r5)
	clr	@r5
	cmp	@r0,@r2
	cmp	@r1,(r1)+
	ash	-(r0),r0
	illegal
	illegal
	div	-(r0),r4
	illegal
	add	(r1)+,@-(sp)
	add	(r4)+,-(r0)
	add	@(r1)+,-(sp)
	add	@r5,-(r5)
	div	6F20(r4),r5
	add	@(r1)+,-(sp)
	illegal
	add	@(r0)+,@-(r0)
	illegal
	ash	-(r0),r0
	add	@6573(r5),@-(r0)
	add	7461(r0),-(r0)
	cmp	r1,-(r5)
	add	-(r5),@-(sp)
	illegal
	cmp	r1,6573(r4)
	div	6F69(r3),r5
	div	@-(sp),r5
	bit	@-(r4),@-(r1)
	clr	@r5
	bis	r0,@r2
	add	(r5)+,7373(r2)
	add	r4,-(r0)
	xor	@-(sp),r5
	add	@6874(r4),-(r0)
	div	-(r5),r1
	add	@-(r4),-(r0)
	xor	-(r5),r5
	ash	-(r0),r0
	illegal
	ash	706F(r3),r1
	ashc	-(r0),r4
	ash	@-(r1),r1
	add	@7475(r5),@-(r0)
	add	@(r0)+,-(r0)
	div	@-(r1),r1
	add	@3B67(r1),@-(r1)
	clr	@r5
	bic	-(r4),@r2
	div	@-(sp),r5
	div	-(r5),r1
	cmp	r1,7571(r4)
	div	-(r1),r1
	add	(r5)+,2072(r4)
	add	@6E20(r1),@-(r1)
	add	r5,-(r5)
	add	(r5)+,7473(r2)
	mul	-(r0),r0
	add	@7267(r5),6D61(r2)
	add	(r5)+,@-(r5)
	cmp	r1,6E61(r2)
	cmp	r1,-(r4)
	div	7365(r0),r1
	cmp	r1,4552(r3)
	bis	(r5)+,(r4)+
	bic	@7420(r1),(r2)+
	illegal
	ash	7261(r3),r1
	cmp	r1,6167(r4)
	add	(r5)+,@-(r5)
	cmp	r0,@1B80(r2)
	bit	@r1,@(r3)+
	jmp	@r2
	bis	@-(r4),@(r3)+
	jsr	r1,r1
	clr	r0
	illegal
	bis	@-(r4),@(r3)+
	halt
	cmp	r0,@r0
	cmp	2D20(r4),@-(r5)
	cmp	r0,@-(r5)
	jsr	r0,r0
	cmp	@4F2D(r4),-(r0)
	bis	0020(r0),@-(r5)
	cmp	r0,@r0
	bit	3D4F(r4),@-(r5)
	cmp	r0,@-(r5)
	jsr	r0,r0
	cmp	r0,@r0
	bit	3E2A(r4),-(r0)
	bit	@3C00(r0),@3C3C(sp)
	bit	2020(r4),@-(r2)
	cmp	@-(r0),r0
	cmp	@-(r0),@-(r2)
	cmp	@-(r0),@-(r2)
	cmp	r0,r0
	cmp	r0,-(r0)
	cmp	r0,-(r0)
	cmp	r4,r0
	jmp	@pc
	mov	@-(r2),@7943(r0)
	bit	@-(r2),sp
	mov	-(r4),@0000(r4)
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
