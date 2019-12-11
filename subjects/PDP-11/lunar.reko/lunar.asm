;;; Segment image (0000)
0000 5F 00 DC 00 00 02 E0 00 02 02 E0 00 04 02 E0 00 _...............
0010 06 02 E0 00 0A 02 E0 00 08 02 E0 00 00 00 00 00 ................
0020 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
0040 8A 13 C0 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0050 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
0090 00 00 00 00 00 00 CE 00 00 00 00 00 00 00 9E 00 ................
00A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00B0 B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00D0 DE 13 80 00 4A 14 80 00 32 02 E0 00             ....J...2...   

l00DC:
	reset
	mov	#182A,r0
	mov	#1FFC,r1
	sub	r0,r1
	asr	r1
	asr	r1

l00EC:
	mov	#F700,(r0)+
	clr	(r0)+
	dec	r1
	bgt	00EC

l00F6:
	mov	#34E0,r0
	mov	#3FFE,r1
	sub	r0,r1
	asr	r1
	asr	r1

l0104:
	mov	#F700,(r0)+
	clr	(r0)+
	dec	r1
	bgt	0104

l010E:
	clr	@#FFFE
	mov	#3FFE,sp
	mov	#013E,r0

l011A:
	mov	(r0)+,r1
	beq	0122

l011E:
	mov	(r0)+,@r1
	br	011A

l0122:
	jsr	pc,@#0128
	br	0122

;; fn0128: 0128
;;   Called from:
;;     0122 (in fn13AA)
;;     0BB8 (in fn0B06)
fn0128 proc
	wait
	mov	@#006C,r0
	beq	013C

l0130:
	clr	@#006C
	mov	r0,@#006E
	jsr	pc,@#053A

l013C:
	rts	pc
013E                                           44 00               D.
0140 FF FF 46 00 BA FF 48 00 00 00 56 00 00 00 5A 00 ..F...H...V...Z.
0150 00 00 54 00 10 27 58 00 78 EC 5C 00 10 AA 5E 00 ..T..'X.x.\...^.
0160 D8 59 60 00 4B 00 AA 25 71 02 62 00 FF FF 68 00 .Y`.K..%q.b...h.
0170 30 75 70 00 00 00 6E 00 00 00 6C 00 00 00 84 00 0up...n...l.....
0180 00 00 94 00 00 00 96 00 CE 00 CE 00 28 14 98 00 ............(...
0190 00 00 9E 00 94 01 00 F4 F2 16 66 FF 40 00 C2 34 ..........f.@..4
01A0 00 00 BA 34 00 00 DA 34 00 00 CA 34 00 00 D2 34 ...4...4...4...4
01B0 00 00 8C 16 F2 16 8E 16 28 17 90 16 AC 17 92 16 ........(.......
01C0 C6 17 F4 16 F0 9D 0E 17 F0 9D 2A 17 F0 9D 46 17 ..........*...F.
01D0 F0 9D 62 17 F0 9D 7C 17 F0 9D 96 17 F0 9D AE 17 ..b...|.........
01E0 F0 9D C8 17 F0 9D E2 17 F0 9D FC 17 F0 9D 16 18 ................
01F0 F0 9D 9A 00 F4 01 9C 00 0A 00 C0 25 00 F7 00 00 ...........%....
0200 00 00 00 00 00 00 00 00 00 00 DF 15 CE 00 96 00 ................
0210 DF 15 28 14 CE 00 1F 0A 94 00 DF 15 F2 16 00 F4 ..(.............
0220 DF 15 40 00 66 FF C6 15 FE 3F 1F 0A FE FF 5F 00 ..@.f....?...._.
0230 22 01 1F 0A 94 00 26 10 66 10 C1 15 9E 16 5F 00 ".....&.f....._.
0240 3E 14                                           >.             

;; fn0242: 0242
;;   Called from:
;;     0440 (in fn03CE)
;;     058E (in fn053A)
;;     09F6 (in fn0856)
;;     109A (in fn103C)
fn0242 proc
	mov	r0,r4
	mov	r1,r5
	clr	@#0080
	clr	@#0082
	mov	#7FFF,@#0086

l0254:
	mov	(r4)+,pc
	clr	-(sp)
	jsr	pc,@#02C8
	br	027A
025E                                           E6 15               ..
0260 00 40 DF 09 C8 02 DF 27 82 00 84 00 02 04 0E 0A .@.....'........
0270 04 01 DF 20 84 00 01 04 0E 0A                   ... ......     

l027A:
	mov	r2,r0
	sub	@#0080,r2
	bpl	0288

l0282:
	neg	r2
	bis	#2000,@sp

l0288:
	bic	#FFC0,r2
	swab	r2
	ror	r2
	mov	r0,@#0080
	mov	r3,r1
	sub	@#0082,r3
	bpl	02A2

l029C:
	neg	r3
	bis	#0040,@sp

l02A2:
	bic	#FFC0,r3
	mov	r1,@#0082
	cmp	r1,@#0086
	bge	02B4

l02B0:
	mov	r1,@#0086

l02B4:
	bis	r2,r3
	bis	(sp)+,r3
	mov	r3,(r5)+
	br	0254
02BC                                     15 15 CA 01             ....
02C0 D5 15 00 F7 15 0A 87 00                         ........       

;; fn02C8: 02C8
;;   Called from:
;;     0258 (in fn0242)
fn02C8 proc
	movb	(r4)+,r0
	mov	@#004A,r1
	jsr	pc,@#125E
	mov	r2,-(sp)
	movb	@r4,r0
	mov	@#004C,r1
	jsr	pc,@#125E
	sub	r2,@sp
	movb	FFFF(r4),r0
	mov	@#004C,r1
	jsr	pc,@#125E
	mov	r2,-(sp)
	movb	(r4)+,r0
	mov	@#004A,r1
	jsr	pc,@#125E
	add	(sp)+,r2
	mov	(sp)+,r3
	neg	r3
	rts	pc

;; fn0300: 0300
;;   Called from:
;;     05A2 (in fn053A)
fn0300 proc
	tst	@#0068
	beq	03A8

l0306:
	mov	@#0064,r0
	mov	#2904,r1
	jsr	pc,@#114A
	mov	#0064,r0
	jsr	pc,@#126C
	mov	r3,@#0066
	mov	r3,r1
	mov	@#006E,r0
	jsr	pc,@#114A
	mov	#05DC,r0
	jsr	pc,@#126C
	sub	@#0068,r3
	bmi	0340

l0336:
	clr	r3
	mov	#F700,@#25C0
	br	0358

l0340:
	neg	r3
	tst	@#25C0
	beq	0358

l0348:
	cmp	r3,#07D0
	bge	0358

l034E:
	clr	@#25C0
	bis	#0000,@#F402

l0358:
	mov	r3,@#0068
	clr	r2
	mov	#000A,r0
	jsr	pc,@#126C
	add	#37DC,r3
	mov	r3,@#006A
	mov	@#0066,r0
	mov	#3ED7,r1
	jsr	pc,@#114A
	mov	r4,r0
	jsr	pc,@#126C
	mov	r3,@#004E
	mov	r3,r0
	mov	@#004A,r1
	jsr	pc,@#125E
	mov	r2,@#0050
	mov	@#004E,r0
	mov	@#004C,r1
	jsr	pc,@#125E
	sub	#0A6E,r2
	mov	r2,@#0052
	rts	pc

l03A8:
	mov	#F700,@#25C0
	clr	@#0066
	mov	#37DC,@#006A
	add	@#0068,@#006A
	clr	@#004E
	clr	@#0050
	mov	#F592,@#0052
	rts	pc

;; fn03CE: 03CE
;;   Called from:
;;     05E6 (in fn053A)
fn03CE proc
	mov	r0,r1
	mov	@#0064,r4
	asr	r4
	asr	r4
	asr	r4
	movb	2766(r4),r2
	inc	@#008A
	mov	@#008A,r3
	bic	#FFE0,r3
	movb	2773(r3),r3
	add	r3,r2
	add	r2,@#008C
	mov	@#008C,r3
	bic	#FFFC,r3
	mov	#000C,r4
	mov	#27B0,r5
	add	#2793,r3

l0408:
	movb	(r3)+,(r5)+
	movb	r2,(r5)+
	add	#0006,r5
	dec	r4
	bgt	0408

l0414:
	inc	@#0090
	bic	#FFFC,@#0090
	add	#0180,@#008E
	bic	#FC7F,@#008E
	mov	#8C54,@#27A8
	bis	@#0090,@#27A8
	bis	@#008E,@#27A8
	mov	#27A2,r0
	jmp	@#0242

;; fn0444: 0444
;;   Called from:
;;     0572 (in fn053A)
;;     09EE (in fn0856)
;;     0EA2 (in fn0E98)
;;     1090 (in fn103C)
;;     10A6 (in fn103C)
fn0444 proc
	mov	@#0046,r0
	bpl	0458

l044A:
	cmp	r0,#FF4C
	bgt	0464

l0450:
	add	#0168,r0
	bpl	0464

l0456:
	br	044A

l0458:
	cmp	r0,#00B4
	ble	0464

l045E:
	sub	#0168,r0
	bmi	0458

l0464:
	mov	r0,@#0046
	bpl	046E

l046A:
	add	#0168,r0

l046E:
	asl	r0
	mov	31DC(r0),@#004A
	cmp	r0,#021C
	blt	0480

l047C:
	sub	#02D0,r0

l0480:
	mov	3290(r0),@#004C
	rts	pc

;; fn0488: 0488
;;   Called from:
;;     05A6 (in fn053A)
fn0488 proc
	mov	@#0050,r1
	bpl	0490

l048E:
	neg	r1

l0490:
	mov	@#006E,r0
	jsr	pc,@#114A
	mov	#0BB8,r0
	jsr	pc,@#126C
	tst	@#0050
	bpl	04A8

l04A6:
	neg	r3

l04A8:
	mov	r3,r2
	asr	r3
	add	@#0054,r3
	add	r2,@#0054
	mov	r3,-(sp)
	mov	r3,r1
	bpl	04BC

l04BA:
	neg	r1

l04BC:
	mov	@#006E,r0
	jsr	pc,@#114A
	mov	#0258,r0
	add	@#0056,r3
	adc	r2
	jsr	pc,@#126C
	tst	(sp)+
	bpl	04D8

l04D6:
	neg	r3

l04D8:
	add	r3,@#005C
	mov	r2,@#0056
	mov	@#0052,r1
	bpl	04E8

l04E6:
	neg	r1

l04E8:
	mov	@#006E,r0
	jsr	pc,@#114A
	mov	#0BB8,r0
	jsr	pc,@#126C
	tst	@#0052
	bpl	0500

l04FE:
	neg	r3

l0500:
	mov	r3,r2
	asr	r3
	add	@#0058,r3
	add	r2,@#0058
	mov	r3,-(sp)
	mov	r3,r1
	bpl	0514

l0512:
	neg	r1

l0514:
	mov	@#006E,r0
	jsr	pc,@#114A
	mov	#0258,r0
	add	@#005A,r3
	adc	r2
	jsr	pc,@#126C
	tst	(sp)+
	bpl	0530

l052E:
	neg	r3

l0530:
	add	r3,@#005E
	mov	r2,@#005A
	rts	pc

;; fn053A: 053A
;;   Called from:
;;     0138 (in fn0128)
fn053A proc
	mov	#FFC0,@#0084
	mov	@#0044,r5
	clr	@#0044
	mov	@#006E,r0
	mov	r5,r1
	bpl	0552

l0550:
	neg	r1

l0552:
	jsr	pc,@#114A
	mov	#003C,r0
	add	@#0048,r3
	adc	r2
	jsr	pc,@#126C
	tst	r5
	bpl	056A

l0568:
	neg	r3

l056A:
	add	r3,@#0046
	mov	r2,@#0048
	jsr	pc,@#0444
	add	#0002,@#0092
	mov	@#0092,r1
	bic	#FFFD,r1
	mov	34AC(r1),r1
	mov	#26D8,r0
	mov	r1,-(sp)
	jsr	pc,@#0242
	mov	(sp)+,@#34BA
	mov	@#0086,@#0088
	mov	@#0060,@#0064
	jsr	pc,@#0300
	jsr	pc,@#0488
	mov	@#005C,r4
	add	#5780,r4
	asr	r4
	asr	r4
	asr	r4
	asr	r4
	asr	r4
	mov	r4,@#0072
	mov	r4,r5
	asl	r4
	add	#28F0,r4
	mov	(r4)+,r0
	add	@r4,r0
	asr	r0
	mov	r0,@#007C
	sub	@#005E,r0
	neg	r0
	mov	r0,@#007E
	tst	@#0066
	ble	05F2

l05E2:
	mov	#3588,r0
	jsr	pc,@#03CE
	mov	#3588,@#34C2
	br	05F6

l05F2:
	clr	@#34C2

l05F6:
	mov	@#0072,r4
	bmi	0668

l05FC:
	cmp	r4,#000A
	ble	0668

l0602:
	cmp	r4,#037A
	bge	0672

l0608:
	mov	@#005E,r4
	bmi	06B4

l060E:
	cmp	r4,#61A8
	bge	067C

l0614:
	cmp	r4,#01C2
	ble	06B4

l061A:
	mov	@#0072,@#34B4
	asr	r4
	asr	r4
	asr	r4
	asr	r4
	asr	r4
	add	#002B,r4
	mov	r4,@#34B6
	tst	@#009E
	beq	0640

l0638:
	jsr	pc,@#0CEC
	clr	@#009E

l0640:
	jsr	pc,@#1578
	mov	@#007E,r4
	sub	#0010,r4
	bpl	0666

l064E:
	mov	#0280,r0
	jsr	pc,@#0E06
	mov	sp,@#009E
	jsr	pc,@#0CEC
	clr	@#009E
	jsr	pc,@#0E32

l0666:
	rts	pc

l0668:
	mov	#000D,r0
	mov	#1FFC,r1
	br	0682

l0672:
	mov	#0377,r0
	mov	#201C,r1
	br	0682

l067C:
	mov	r4,r0
	mov	#2062,r1

l0682:
	asl	r0
	asl	r0
	asl	r0
	asl	r0
	asl	r0
	sub	#5780,r0
	mov	r0,@#005C
	mov	r1,@#34CA
	clr	@#0068
	clr	@#0054
	mov	@#005E,r3
	asr	r3
	asr	r3
	bmi	06AC

l06AA:
	neg	r3

l06AC:
	mov	r3,@#0058
	jmp	@#053A

l06B4:
	cmp	@#009E,@sp
	beq	0792

l06BA:
	mov	@#0072,r0
	sub	#0009,r0

l06C2:
	mov	r0,@#0074
	asl	r0
	asl	r0
	asl	r0
	asl	r0
	asl	r0
	sub	#5780,r0
	mov	r0,@#0076
	jsr	pc,@#0F04
	mov	@sp,@#009E
	mov	@#005C,r0
	sub	@#0076,r0

l06E8:
	mov	r0,r3
	asl	r0
	add	r0,r3
	asr	r3
	mov	r3,@#34B4
	clr	r2
	mov	#0030,r0
	jsr	pc,@#126C
	add	@#0074,r3
	mov	r3,@#0078
	asl	r3
	mov	r3,r4
	mov	#0030,r0
	sub	r2,r0
	mov	r2,r5
	mov	28F0(r4),r1
	jsr	pc,@#123A
	mov	r3,-(sp)
	mov	r5,r0
	mov	28F2(r4),r1
	jsr	pc,@#123A
	clr	r2
	mov	#0030,r0
	add	(sp)+,r3
	bpl	073A

l0730:
	neg	r3
	jsr	pc,@#126C
	neg	r3
	br	073E

l073A:
	jsr	pc,@#126C

l073E:
	mov	r3,r4
	asr	r3
	asr	r3
	mov	r3,@#007C
	jsr	pc,@#100C
	mov	r4,@#007A
	mov	@#005E,r0
	mov	r0,r3
	asl	r3
	add	r0,r3
	asr	r3
	add	#0017,r3
	mov	r3,@#34B6
	add	#0018,@#34B6
	sub	r4,r3
	mov	r3,r4
	bpl	0772

l0770:
	neg	r3

l0772:
	clr	r2
	mov	#0003,r0
	asl	r3
	jsr	pc,@#126C
	tst	r4
	bpl	0784

l0782:
	neg	r3

l0784:
	mov	r3,@#007E
	jsr	pc,@#0856
	jsr	pc,@#0A0A

;; fn0790: 0790
;;   Called from:
;;     078C (in fn053A)
;;     07F2 (in fn0856)
;;     0824 (in fn0856)
;;     082A (in fn0856)
;;     083E (in fn0856)
;;     0896 (in fn0856)
fn0790 proc
	rts	pc

l0792:
	mov	@#005C,r0
	sub	@#0076,r0
	cmp	r0,#001E
	ble	07AE

l07A0:
	cmp	r0,#0244
	blt	06E8

l07A6:
	mov	@#0072,r0
	dec	r0
	br	06C2

l07AE:
	mov	@#0072,r0
	sub	#0011,r0
	br	06C2
07B8                         D7 0B 00 00 E9 03 57 21         ......W!
07C0 1E 00 E6 06 17 0A C6 07 DF 15 44 24 CA 34 5F 00 ..........D$.4_.
07D0 0A 09 57 21 0F 00 DC 06 D7 27 58 00 A8 FD 35 07 ..W!.....'X...5.
07E0 DF 15 52 23 CA 34 1F 0A BA 34 DF 09 32 0E       ..R#.4...4..2. 

l07EE:
	cmp	r5,#001A
	bgt	0790

l07F4:
	cmp	r4,#FDA8
	ble	084A

l07FA:
	mov	#237E,@#34CA
	mov	#0003,-(sp)
	tst	@#0054
	bmi	080C

l080A:
	inc	@sp

l080C:
	mov	r0,-(sp)
	jsr	pc,@#0C90
	sub	#0010,@#34B6
	jsr	pc,@#0F04
	jsr	pc,@#0E32
	cmp	r5,#001A
	bgt	0790

l0826:
	tst	@#0066
	beq	0790

l082C:
	clr	-(sp)
	mov	r0,-(sp)
	jsr	pc,@#0C90
	mov	#23B8,@#34CA
	jsr	pc,@#0F04
	br	0790
0840 57 21 11 00 A5 06 17 21 A8 FD                   W!.....!..     

l084A:
	ble	0904

l084C:
	mov	#23FE,@#34CA
	jsr	pc,@#0E32

;; fn0856: 0856
;;   Called from:
;;     0788 (in fn053A)
fn0856 proc
	jsr	pc,@#1578
	mov	@#007E,r5
	bmi	08B2

l0860:
	cmp	r5,#0003
	ble	08B8

l0866:
	mov	@#0058,r4
	cmp	r4,#FDA8
	blt	089A

l0870:
	cmp	r4,#FED4
	blt	08A2

l0876:
	cmp	r4,#FF6A
	blt	08AA

l087C:
	cmp	@#34CA,#210A
	bne	0888

l0884:
	clr	@#34CA

l0888:
	mov	@#0078,r0
	mov	r0,-(sp)
	jsr	pc,@#0CCA
	mov	(sp)+,r1
	asl	r1
	jmp	@0FEC(r1)

l089A:
	mov	#20AC,@#34CA
	br	0888

l08A2:
	mov	#20D8,@#34CA
	br	0888

l08AA:
	mov	#210A,@#34CA
	br	0888

l08B2:
	cmp	r5,#FFF6
	ble	0904

l08B8:
	clr	@#0060
	mov	#01C2,@#25AA
	clr	@#34C2
	clr	@#34D2
	tst	r5
	beq	08D0

l08CE:
	bpl	0888

l08D0:
	mov	@#0058,r4
	cmp	r4,#FDA8
	ble	0904

l08DA:
	cmp	r4,#FED4
	ble	08FE

l08E0:
	cmp	r4,#FF6A
	ble	08F8

l08E6:
	cmp	r4,#FFB0
	ble	08F2

l08EC:
	mov	#214E,r0
	br	091E

l08F2:
	mov	#2176,r0
	br	091E

l08F8:
	mov	#21A4,r0
	br	091E

l08FE:
	mov	#21CE,r0
	br	091E

l0904:
	mov	#2212,@#34CA
	mov	#0020,r0
	jsr	pc,@#0E06
	jsr	pc,@#0F04
	clr	@#34BA
	jsr	pc,@#0E32

l091E:
	clr	@#34C2
	mov	r0,@#34CA
	cmp	@#0054,#0064
	bgt	0964

l092E:
	cmp	@#0054,#FF9C
	blt	0964

l0936:
	cmp	@#0046,#FFF1
	blt	096E

l093E:
	cmp	@#0046,#000F
	bgt	096E

l0946:
	mov	@#0078,r1
	asl	r1
	mov	28F2(r1),r0
	sub	28F0(r1),r0
	mov	r0,r2
	bpl	095A

l0958:
	neg	r2

l095A:
	cmp	r2,#0030
	bge	0978

l0960:
	jsr	pc,@#0B06

l0964:
	mov	@#0054,r0
	mov	#22C6,r1
	br	097C

l096E:
	mov	@#0046,r0
	mov	#2258,r1
	br	097C

l0978:
	mov	#2314,r1

l097C:
	mov	r1,@#34C2
	mov	@#0078,r1
	mov	#0003,-(sp)
	tst	r0
	bmi	098E

l098C:
	inc	@sp

l098E:
	mov	r1,-(sp)
	jsr	pc,@#0C90
	asl	r1
	mov	28F2(r1),r2
	sub	28F0(r1),r2
	mov	r2,r3
	asl	r3
	add	r2,r3
	asr	r3
	asr	r3
	sub	r3,r2
	mov	r2,r3
	asr	r3
	add	r3,r2
	bpl	09BE

l09B2:
	cmp	r2,#FFD3
	bge	09C8

l09B8:
	mov	#FFD3,r2
	br	09C8

l09BE:
	cmp	r2,#002D
	ble	09C8

l09C4:
	mov	#002D,r2

l09C8:
	mov	#005A,r3
	tst	r0
	bpl	09D2

l09D0:
	neg	r3

l09D2:
	add	r3,r2
	mov	r2,@#0046
	mov	@#0092,r1
	add	#0002,r1
	bic	#FFFD,r1
	mov	r1,@#0092
	mov	34AC(r1),r1
	mov	r1,-(sp)
	jsr	pc,@#0444
	mov	#26D8,r0
	jsr	pc,@#0242
	mov	(sp)+,@#34BA
	sub	#0007,@#34B6
	jsr	pc,@#13AA
	illegal

;; fn0A0A: 0A0A
;;   Called from:
;;     078C (in fn053A)
fn0A0A proc
	cmp	@#007E,#0096
	bge	0B00

l0A12:
	mov	#35CA,r5
	mov	@#0064,r4
	cmp	r4,#003F
	ble	0A24

l0A20:
	mov	#003F,r4

l0A24:
	rol	r4
	rol	r4
	rol	r4
	rol	r4
	bic	#FC7F,r4
	bis	#9C50,r4
	mov	r4,(r5)+
	cmp	@#0046,#002D
	bgt	0B00

l0A3E:
	cmp	@#0046,#FFD3
	blt	0B00

l0A46:
	mov	@#004A,r1
	bpl	0A4E

l0A4C:
	neg	r1

l0A4E:
	mov	@#34B6,r0
	sub	@#007A,r0
	mov	r0,r4
	jsr	pc,@#114A
	mov	@#004C,r0
	jsr	pc,@#126C
	add	r3,r4
	tst	@#004A
	bmi	0A6E

l0A6C:
	neg	r3

l0A6E:
	add	@#34B4,r3
	mov	r3,(r5)+
	mov	@#007A,(r5)+
	mov	#B000,(r5)+
	sub	#0096,r4
	bpl	0B00

l0A82:
	neg	r4
	mov	r4,r0
	mov	@#0064,r1
	jsr	pc,@#114A
	asr	r3
	asr	r3
	asr	r3

;; fn0A94: 0A94
;;   Called from:
;;     0A92 (in fn0A0A)
;;     0B0A (in fn0B06)
fn0A94 proc
	asr	r3
	beq	0B00

l0A98:
	mov	r3,-(sp)
	mov	#0960,r2
	asr	r2
	asr	r2
	cmp	r2,@sp
	bcc	0AA8

l0AA6:
	mov	r2,@sp

l0AA8:
	mov	@#00B0,r2
	mov	#FFC0,r3

l0AB0:
	add	@#0070,r2
	inc	r2
	bic	r3,r2
	movb	2766(r2),r0
	add	@#0052,r2
	bic	r3,r2
	bic	r3,r0
	swab	r0
	ror	r0
	com	r4
	bic	#DFFF,r4
	bis	#4000,r4
	bis	r4,r0
	movb	2766(r2),r1
	bic	r3,r1
	bis	r1,r0
	mov	r0,(r5)+
	add	#2040,r0
	bic	#C000,r0
	mov	r0,(r5)+
	dec	@sp
	bgt	0AB0

l0AEC:
	mov	#F700,(r5)+
	clr	@r5
	mov	r2,@#00B0

;; fn0AF6: 0AF6
;;   Called from:
;;     0AF2 (in fn0A94)
;;     0B66 (in fn0B06)
;;     0B70 (in fn0B06)
fn0AF6 proc
	tst	(sp)+

l0AF8:
	mov	#35CA,@#34D2
	rts	pc

l0B00:
	clr	@#34D2
	rts	pc

;; fn0B06: 0B06
;;   Called from:
;;     0960 (in fn0856)
fn0B06 proc
	jsr	pc,@#13AA
	sob	pc,0A94

l0B0C:
	mov	@#34B4,@#267A

l0B12:
	mov	@#34B6,@#267C
	mov	#2678,@#34D2

l0B1E:
	bit	#007F,@#0070
	bne	0B1E

l0B26:
	tst	@#07BA
	beq	0BD0

l0B2C:
	mov	@#2610,r3
	sub	@#34B4,r3
	mov	@#2612,r2
	sub	@#267C,r2
	add	#0003,r2
	mov	r2,-(sp)
	beq	0B48

l0B44:
	jsr	pc,@#0C36

l0B48:
	mov	@#2610,r3
	sub	@#267A,r3
	add	#0019,r3
	mov	r3,-(sp)
	clr	r2
	jsr	pc,@#0C36
	mov	#24A2,@#34CA
	jsr	pc,@#13AA
	sob	pc,0AF8

l0B68:
	clr	@#34CA
	jsr	pc,@#13AA
	sob	pc,0AF6

l0B72:
	neg	@sp
	mov	@sp,r3
	clr	r2
	jsr	pc,@#0C36
	mov	(sp)+,r3
	mov	(sp)+,r2
	neg	r2
	beq	0B88

l0B84:
	jsr	pc,@#0C36

l0B88:
	jsr	pc,@#13AA
	sob	pc,0B12

l0B8E:
	add	#0004,@#005E
	add	#07D0,@#0068
	clr	@#0058
	clr	@#006C
	mov	#3FFE,sp

l0BA6:
	clr	@#34CA
	clr	@#0046
	clr	@#0054
	mov	#001E,@#0060
	jsr	pc,@#0128
	tst	@#009E
	bne	0BA6

l0BC2:
	clr	@#34BA
	clr	@#34C2
	jsr	pc,@#13AA
	bpt

l0BD0:
	mov	#0001,-(sp)
	mov	@#0078,-(sp)
	jsr	pc,@#0C90
	mov	#FFE8,r2
	mov	#0030,r3
	mov	@#0070,r5
	ror	r5
	bcc	0BEE

l0BEC:
	neg	r3

l0BEE:
	mov	r3,-(sp)
	jsr	pc,@#0C36
	mov	(sp)+,r3
	clr	r2
	jsr	pc,@#0C36
	mov	@#267A,@#26AC
	mov	@#267C,@#26AE
	add	#0014,@#26AC
	mov	#26A8,@#34C2
	mov	@#0078,r0
	jsr	pc,@#0C72
	jsr	pc,@#0C72
	mov	#0002,-(sp)
	mov	r0,-(sp)
	jsr	pc,@#0C90
	mov	#24D6,@#34CA
	jsr	pc,@#13AA
	illegal

;; fn0C36: 0C36
;;   Called from:
;;     0B44 (in fn0B06)
;;     0B58 (in fn0B06)
;;     0B78 (in fn0B06)
;;     0B84 (in fn0B06)
;;     0BF0 (in fn0B06)
;;     0BF8 (in fn0B06)
fn0C36 proc
	mov	#0A80,r5
	tst	r3
	bpl	0C44

l0C3E:
	mov	#0AC0,r5
	neg	r3

l0C44:
	mov	r5,@#0C72
	clr	r5
	tst	r2
	beq	0C58

l0C4E:
	inc	r5
	mov	r2,r3
	bpl	0C58

l0C54:
	neg	r3
	neg	r5

l0C58:
	mov	@#267A,r0

l0C5C:
	jsr	pc,@#0C72
	mov	r0,@#267A
	add	r5,@#267C
	jsr	pc,@#0C76
	dec	r3
	bgt	0C5C

l0C70:
	rts	pc

;; fn0C72: 0C72
;;   Called from:
;;     0C18 (in fn0B06)
;;     0C1C (in fn0B06)
;;     0C5C (in fn0C36)
fn0C72 proc
	halt
0C74             87 00                                   ..         

;; fn0C76: 0C76
;;   Called from:
;;     0C68 (in fn0C36)
fn0C76 proc
	bit	#0007,@#0070
	beq	0C76

l0C7E:
	jsr	pc,@#1578

l0C82:
	bit	#0007,@#0070
	bne	0C82

l0C8A:
	jsr	pc,@#1578
	rts	pc

;; fn0C90: 0C90
;;   Called from:
;;     080E (in fn0856)
;;     0830 (in fn0856)
;;     0990 (in fn0856)
;;     0BD8 (in fn0B06)
;;     0C26 (in fn0B06)
;;     0E22 (in fn0E06)
fn0C90 proc
	mov	r4,-(sp)
	mov	0004(sp),r4
	asr	r4
	bcc	0CB2

l0C9A:
	asl	0006(sp)
	asl	0006(sp)
	asl	0006(sp)
	asl	0006(sp)
	bicb	#00F0,3013(r4)
	br	0CB8

l0CB2:
	bicb	#000F,3013(r4)

l0CB8:
	bisb	0006(sp),3013(r4)
	mov	(sp)+,r4
	mov	@sp,0004(sp)
	add	#0004,sp
	rts	pc

;; fn0CCA: 0CCA
;;   Called from:
;;     088E (in fn0856)
fn0CCA proc
	mov	r4,-(sp)
	mov	0004(sp),r4
	asr	r4
	movb	3013(r4),r4
	bcc	0CE0

l0CD8:
	asr	r4
	asr	r4
	asr	r4
	asr	r4

l0CE0:
	bic	#FEF0,r4
	mov	r4,0004(sp)
	mov	(sp)+,r4
	rts	pc

;; fn0CEC: 0CEC
;;   Called from:
;;     0638 (in fn053A)
;;     065A (in fn053A)
fn0CEC proc
	jsr	pc,@#0D3C
	mov	@#28F0,r4
	asr	r4
	asr	r4
	asr	r4
	asr	r4
	asr	r4
	add	#0017,r4
	mov	r4,(r5)+
	mov	r4,@#0082
	mov	#8C50,(r5)+
	mov	#28F0,r0

l0D10:
	add	#0008,r0
	mov	@r0,r4
	asr	r4
	asr	r4
	asr	r4
	asr	r4
	asr	r4
	add	#0017,r4
	jsr	pc,@#0D78
	br	0D10
0D2A                               D5 15 00 F7 0D 0A           ......
0D30 DF 15 2A 18 DA 34 DF 09 66 0D 87 00             ..*..4..f...   

;; fn0D3C: 0D3C
;;   Called from:
;;     0CEC (in fn0CEC)
;;     0F04 (in fn0F04)
fn0D3C proc
	mov	r2,-(sp)
	mov	r3,-(sp)
	mov	r0,-(sp)
	mov	r1,-(sp)
	mov	r4,-(sp)
	mov	#00E1,@#00A0
	clr	@#34DA
	clr	@#07BA
	mov	#182A,r5
	mov	#9854,(r5)+
	mov	#F0A0,(r5)+
	clr	(r5)+
	jmp	@000A(sp)

;; fn0D66: 0D66
fn0D66 proc
	mov	@sp,000C(sp)
	mov	(sp)+,r4
	mov	(sp)+,r4
	mov	(sp)+,r1
	mov	(sp)+,r0
	mov	(sp)+,r3
	mov	(sp)+,r2
	rts	pc

;; fn0D78: 0D78
;;   Called from:
;;     0D24 (in fn0CEC)
;;     0F9A (in fn0F04)
fn0D78 proc
	mov	#0200,-(sp)
	cmp	r4,#0400
	bcs	0D8C

l0D82:
	mov	#03FF,r4
	cmp	r4,@#0082
	beq	0D9C

l0D8C:
	tst	r4
	bpl	0D98

l0D90:
	clr	r4
	tst	@#0082
	beq	0D9C

l0D98:
	bis	#4000,@sp

l0D9C:
	dec	@#00A2
	bpl	0DD8

l0DA2:
	inc	@#00A4
	bic	#FFFC,@#00A4
	inc	@#00A4
	mov	@#00A4,@#00A2
	add	#0280,@#00A6
	bic	#FC7F,@#00A6
	inc	@#00A8
	bic	#FFFC,@#00A8
	mov	@#00A6,@r5
	bis	@#00A8,@r5
	bis	#8C04,(r5)+

l0DD8:
	sub	@#0082,r4
	bpl	0DEE

l0DDE:
	neg	r4
	bic	#FFC0,r4
	sub	r4,@#0082
	bis	#0040,r4
	br	0DF6

l0DEE:
	bic	#FFC0,r4
	add	r4,@#0082

l0DF6:
	bis	(sp)+,r4
	mov	r4,(r5)+
	dec	@#00A0
	bgt	0E04

l0E00:
	add	#0002,@sp

l0E04:
	rts	pc

;; fn0E06: 0E06
;;   Called from:
;;     0652 (in fn053A)
;;     090E (in fn0856)
fn0E06 proc
	mov	@#0072,r1
	mov	r1,r4
	asl	r1
	add	#28F0,r1
	mov	#0003,-(sp)
	mov	(r1)+,r5
	mov	r1,r3
	sub	@r1,r5
	bmi	0E20

l0E1E:
	inc	@sp

l0E20:
	mov	r4,-(sp)
	jsr	pc,@#0C90

l0E26:
	sub	r0,(r1)+
	sub	r0,-(r3)
	asr	r0
	neg	r0
	bne	0E26

l0E30:
	rts	pc

;; fn0E32: 0E32
;;   Called from:
;;     0662 (in fn053A)
;;     081C (in fn0856)
;;     0852 (in fn0856)
;;     091A (in fn0856)
fn0E32 proc
	clr	@#00AE
	clr	@#34C2
	clr	@#34D2
	bis	#0000,@#F402

l0E44:
	mov	#35CA,r5
	mov	@#00AC,r4
	inc	r4
	swab	r4
	ror	r4
	add	@r5,r4
	bic	#FC7F,r4
	bis	#9C50,r4
	mov	r4,(r5)+
	jsr	pc,@#0E98
	bis	#0000,@#F402
	sub	#000A,@#00AE
	jsr	pc,@#0E98
	mov	#F700,(r5)+
	clr	@r5
	mov	#35CA,@#34C2
	bis	#0000,@#F402
	add	#0021,@#00AE
	cmp	@#00AE,#00C0
	ble	0E44

l0E92:
	jsr	pc,@#13AA
	reset

;; fn0E98: 0E98
;;   Called from:
;;     0E5E (in fn0E32)
;;     0E6E (in fn0E32)
;;     0E96 (in fn0E32)
fn0E98 proc
	mov	#FFE2,@#0046
	mov	#00F1,-(sp)

l0EA2:
	jsr	pc,@#0444
	mov	FFFA(sp),r0
	asr	r0
	inc	r0
	add	@#0070,r0
	add	@#00AC,r0
	mov	r0,@#00AC
	bic	#FFE0,r0
	movb	2773(r0),r4
	add	@#00AE,r4
	bmi	0EFE

l0EC8:
	mov	r4,r0
	mov	@#004C,r1
	jsr	pc,@#125E
	add	@#34B4,r2
	bmi	0EFE

l0ED8:
	bis	#4000,r2
	mov	r2,(r5)+
	mov	r4,r0
	mov	@#004A,r1
	jsr	pc,@#125E
	add	@#34B6,r2
	bmi	0EFC

l0EEE:
	mov	r2,(r5)+

l0EF0:
	inc	@#0046
	dec	@sp
	bgt	0EA2

l0EF8:
	tst	(sp)+
	rts	pc

l0EFC:
	clr	-(r5)

l0EFE:
	clr	(r5)+
	clr	(r5)+
	br	0EF0

;; fn0F04: 0F04
;;   Called from:
;;     06D8 (in fn053A)
;;     0818 (in fn0856)
;;     083A (in fn0856)
;;     0912 (in fn0856)
fn0F04 proc
	jsr	pc,@#0D3C
	clr	@#00AA
	mov	@#0074,r0
	asl	r0
	add	#28F0,r0
	mov	@r0,r4
	jsr	pc,@#100C
	tst	r4
	bpl	0F24

l0F20:
	clr	r4
	br	0F2E

l0F24:
	cmp	r4,#0400
	bcs	0F2E

l0F2A:
	mov	#03FF,r4

l0F2E:
	mov	r4,(r5)+
	mov	r4,@#0082
	mov	#8C50,(r5)+

l0F38:
	mov	r4,-(sp)
	mov	(r0)+,r4
	jsr	pc,@#100C
	mov	r4,r1
	mov	(sp)+,r4
	clr	r2
	mov	r0,-(sp)
	mov	#000C,r0
	sub	r4,r1
	bpl	0F60

l0F50:
	sub	#0006,r1
	neg	r1
	mov	r1,r3
	jsr	pc,@#126C
	neg	r3
	br	0F6A

l0F60:
	add	#0006,r1
	mov	r1,r3
	jsr	pc,@#126C

l0F6A:
	mov	r3,r1
	mov	r0,r2
	mov	(sp)+,r0

l0F70:
	inc	@#00AA
	cmp	@#00AA,#0003
	blt	0F84

l0F7C:
	mov	#0ADF,@#0F70
	br	0F92

l0F84:
	cmp	@#00AA,#FFFD
	bgt	0F92

l0F8C:
	mov	#0A9F,@#0F70

l0F92:
	add	@#00AA,r4
	add	r1,r4
	mov	r4,-(sp)
	jsr	pc,@#0D78
	br	0FA2
0FA0 04 01                                           ..             

l0FA2:
	mov	(sp)+,r4
	dec	r2
	bgt	0F70

l0FA8:
	br	0F38
0FAA                               C2 17 74 00 CE 15           ..t...
0FB0 13 00 C0 15 18 00 83 10 C3 0C C3 65 F0 28 A6 10 ...........e.(..
0FC0 82 0A DF 09 CA 0C 84 15 C4 0C FC 09 FC 0F D3 0B ................
0FD0 C0 65 30 00 CE 0A F3 06 D6 0B D5 15 00 F7 0D 0A .e0.............
0FE0 DF 09 66 0D DF 15 2A 18 DA 34 87 00             ..f...*..4..   
l0FEC	dw	0x0790
l0FEE	dw	0x07EE
0FF0 20 08 40 08 40 08 D2 07 B8 07 B8 07 04 0E 1E 10  .@.@...........
1000 B8 10 28 10 32 10 E0 10 90 07 14 11             ..(.2.......   

;; fn100C: 100C
;;   Called from:
;;     0748 (in fn053A)
;;     0F18 (in fn0F04)
;;     0F3C (in fn0F04)
;;     105C (in fn103C)
;;     1066 (in fn103C)
fn100C proc
	mov	r4,-(sp)
	asl	r4
	add	(sp)+,r4
	asr	r4
	asr	r4
	asr	r4
	add	#0017,r4
	rts	pc
101E                                           DF 09               ..
1020 3C 10 00 00 17 00 E8 FF DF 09 3C 10 A6 FF 10 00 <.........<.....
1030 EE FF DF 09 3C 10 5A 00 10 00 EE FF             ....<.Z.....   

;; fn103C: 103C
fn103C proc
	cmp	r5,#1F96
	bhi	10B6

l1042:
	mov	r1,-(sp)
	mov	r2,-(sp)
	mov	r3,-(sp)
	mov	r4,-(sp)
	mov	r0,-(sp)
	mov	000A(sp),r0
	mov	#9800,(r5)+
	mov	@sp,(r5)+
	mov	@#0046,-(sp)
	mov	@r3,r4
	jsr	pc,@#100C
	mov	r4,-(sp)
	mov	0002(r3),r4
	jsr	pc,@#100C
	add	(sp)+,r4
	asr	r4
	mov	(r0)+,@#0046
	add	(r0)+,r4
	mov	@r0,@#0084
	mov	r4,(r5)+
	cmp	@r3,0002(r3)
	beq	1090

l1080:
	bhi	108A

l1082:
	add	#FFEA,@#0046
	br	1090

l108A:
	add	#0016,@#0046

l1090:
	jsr	pc,@#0444
	mov	r5,r1
	mov	#26D8,r0
	jsr	pc,@#0242
	clr	-(r5)
	clr	-(r5)
	mov	(sp)+,@#0046
	jsr	pc,@#0444
	mov	(sp)+,r0
	mov	(sp)+,r4
	mov	(sp)+,r3
	mov	(sp)+,r2
	mov	(sp)+,r1
	tst	(sp)+

l10B6:
	rts	pc
10B8                         57 21 C6 1F 2A 82 1F 10         W!..*...
10C0 AC 26 C4 12 DF 09 0C 10 01 11 C4 1C 02 00 DF 09 .&..............
10D0 0C 10 44 60 84 0C 1F 11 AE 26 C1 15 A8 26 13 01 ..D`.....&...&..
10E0 57 21 C2 1F 16 82 1F 10 DC 25 C4 12 DF 09 0C 10 W!.......%......
10F0 01 11 C4 1C 02 00 DF 09 0C 10 44 60 84 0C 1F 11 ..........D`....
1100 DE 25 C1 15 D8 25 44 14 17 21 00 F7 02 03 15 11 .%...%D..!......
1110 FA 01 87 00 DF 0B C6 07 17 03 17 20 19 00 14 07 ........... ....
1120 17 20 70 03 11 04 C4 1C 02 00 C4 22 01 04 C4 12 . p........"....
1130 DF 09 0C 10 1F 11 12 26 1F 10 10 26 9F 11 BA 07 .......&...&....
1140 D5 15 00 F7 D5 15 0C 26 87 00                   .......&..     

;; fn114A: 114A
;;   Called from:
;;     030E (in fn0300)
;;     0324 (in fn0300)
;;     0376 (in fn0300)
;;     0494 (in fn0488)
;;     04C0 (in fn0488)
;;     04EC (in fn0488)
;;     0518 (in fn0488)
;;     0552 (in fn053A)
;;     0A58 (in fn0A0A)
;;     0A8A (in fn0A0A)
;;     1246 (in fn123A)
;;     1252 (in fn123A)
fn114A proc
	clr	r3
	cmp	r1,r0
	bcc	115A

l1150:
	mov	r1,r2
	beq	119E

l1154:
	mov	r0,r1
	clrflags	#01
	br	115E

l115A:
	mov	r0,r2
	beq	119E

l115E:
	rol	r2
	bcs	11A0

l1162:
	rol	r2
	bcs	11A8

l1166:
	rol	r2
	bcs	11B2

l116A:
	rol	r2
	bcs	11BC

l116E:
	rol	r2
	bcs	11C6

l1172:
	rol	r2
	bcs	11D0

l1176:
	rol	r2
	bcs	11DA

l117A:
	rol	r2
	bcs	11E4

l117E:
	rol	r2
	bcs	11EE

l1182:
	rol	r2
	bcs	11F8

l1186:
	rol	r2
	bcs	1202

l118A:
	rol	r2
	bcs	120C

l118E:
	rol	r2
	bcs	1216

l1192:
	rol	r2
	bcs	1220

l1196:
	rol	r2
	bcs	122A

l119A:
	clr	r2
	mov	r1,r3

l119E:
	rts	pc

l11A0:
	mov	r1,r3
	asl	r3
	rol	r2
	bcc	11AC

l11A8:
	add	r1,r3
	adc	r2

l11AC:
	asl	r3
	rol	r2
	bcc	11B6

l11B2:
	add	r1,r3
	adc	r2

l11B6:
	asl	r3
	rol	r2
	bcc	11C0

l11BC:
	add	r1,r3
	adc	r2

l11C0:
	asl	r3
	rol	r2
	bcc	11CA

l11C6:
	add	r1,r3
	adc	r2

l11CA:
	asl	r3
	rol	r2
	bcc	11D4

l11D0:
	add	r1,r3
	adc	r2

l11D4:
	asl	r3
	rol	r2
	bcc	11DE

l11DA:
	add	r1,r3
	adc	r2

l11DE:
	asl	r3
	rol	r2
	bcc	11E8

l11E4:
	add	r1,r3
	adc	r2

l11E8:
	asl	r3
	rol	r2
	bcc	11F2

l11EE:
	add	r1,r3
	adc	r2

l11F2:
	asl	r3
	rol	r2
	bcc	11FC

l11F8:
	add	r1,r3
	adc	r2

l11FC:
	asl	r3
	rol	r2
	bcc	1206

l1202:
	add	r1,r3
	adc	r2

l1206:
	asl	r3
	rol	r2
	bcc	1210

l120C:
	add	r1,r3
	adc	r2

l1210:
	asl	r3
	rol	r2
	bcc	121A

l1216:
	add	r1,r3
	adc	r2

l121A:
	asl	r3
	rol	r2
	bcc	1224

l1220:
	add	r1,r3
	adc	r2

l1224:
	asl	r3
	rol	r2
	bcc	122E

l122A:
	add	r1,r3
	adc	r2

l122E:
	asl	r3
	rol	r2
	bcc	1238

l1234:
	add	r1,r3
	adc	r2

l1238:
	rts	pc

;; fn123A: 123A
;;   Called from:
;;     0716 (in fn053A)
;;     0722 (in fn053A)
;;     125E (in fn125E)
fn123A proc
	tst	r0
	bpl	124C

l123E:
	neg	r0
	tst	r1
	bpl	1252

l1244:
	neg	r1

l1246:
	jsr	pc,@#114A
	rts	pc

l124C:
	tst	r1
	bpl	1246

l1250:
	neg	r1

l1252:
	jsr	pc,@#114A
	neg	r3
	adc	r2
	neg	r2
	rts	pc

;; fn125E: 125E
;;   Called from:
;;     02CE (in fn02C8)
;;     02DA (in fn02C8)
;;     02E8 (in fn02C8)
;;     02F4 (in fn02C8)
;;     038A (in fn0300)
;;     039A (in fn0300)
;;     0ECE (in fn0E98)
;;     0EE4 (in fn0E98)
fn125E proc
	jsr	pc,@#123A
	asl	r3
	rol	r2
	asl	r3
	rol	r2
	rts	pc

;; fn126C: 126C
;;   Called from:
;;     0316 (in fn0300)
;;     032C (in fn0300)
;;     0362 (in fn0300)
;;     037C (in fn0300)
;;     049C (in fn0488)
;;     04CE (in fn0488)
;;     04F4 (in fn0488)
;;     0526 (in fn0488)
;;     0560 (in fn053A)
;;     06FA (in fn053A)
;;     0732 (in fn053A)
;;     073A (in fn053A)
;;     077A (in fn053A)
;;     0A60 (in fn0A0A)
;;     0F58 (in fn0F04)
;;     0F66 (in fn0F04)
;;     15AC (in fn1578)
;;     1610 (in fn15F2)
fn126C proc
	asl	r3
	rol	r2
	sub	r0,r2
	bpl	12F0

l1274:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	12FA

l127C:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	1304

l1284:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	130E

l128C:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	1318

l1294:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	1322

l129C:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	132C

l12A4:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	1336

l12AC:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	1340

l12B4:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	134A

l12BC:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	1354

l12C4:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	135E

l12CC:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	1368

l12D4:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	1372

l12DC:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	137C

l12E4:
	asl	r3
	rol	r2
	add	r0,r2
	bpl	1386

l12EC:
	add	r0,r2
	rts	pc

l12F0:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	127C

l12FA:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	1284

l1304:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	128C

l130E:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	1294

l1318:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	129C

l1322:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	12A4

l132C:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	12AC

l1336:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	12B4

l1340:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	12BC

l134A:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	12C4

l1354:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	12CC

l135E:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	12D4

l1368:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	12DC

l1372:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	12E4

l137C:
	inc	r3
	asl	r3
	rol	r2
	sub	r0,r2
	bmi	12EC

l1386:
	inc	r3

l1388:
	rts	pc
138A                               DF 55 40 00 66 FF           .U@.f.
1390 9F 0A 6C 00 9F 0A 70 00 DF 35 0F 00 70 00 04 02 ..l...p..5..p...
13A0 1F 0A 3A 15 1F 0A 3C 15 02 00                   ..:...<...     

;; fn13AA: 13AA
;;   Called from:
;;     0A04 (in fn0856)
;;     0B06 (in fn0B06)
;;     0B62 (in fn0B06)
;;     0B6C (in fn0B06)
;;     0B88 (in fn0B06)
;;     0BCA (in fn0B06)
;;     0C30 (in fn0B06)
;;     0E92 (in fn0E32)
;;     3554 (in fn34E0)
fn13AA proc
	mov	#F700,@#25C0
	mov	@0000(sp),r0
	mov	@#0070,r1
	mov	r0,r2
	bpl	13BE

l13BC:
	neg	r0

l13BE:
	add	#003C,r1
	dec	r0
	bgt	13BE

l13C6:
	wait
	jsr	pc,@#1578
	cmp	r1,@#0070
	bhi	13C6

l13D2:
	add	#0002,@sp
	tst	r2
	bmi	1388

l13DA:
	jmp	@#00DC
13DE                                           26 10               &.
13E0 66 10 C0 17 96 00 C1 1F 16 E0 1B 03 E0 17 00 F4 f...............
13F0 C8 65 02 00 E0 15 24 14 57 22 00 F7 0B 02 F1 0B .e....$.W"......
1400 02 00 0F 03 60 10 C8 65 04 00 E0 15 24 14 41 1C ....`..e....$.A.
1410 02 00 F2 01 1F 10 96 00 5F 10 00 F4 81 15 80 15 ........_.......
1420 02 00 07 14 01 14 E8 01 9F 0A 94 00 C1 17 94 00 ................
1430 C1 0C 41 1C 06 15 03 02 1F 0A 94 00 F7 01 DF 15 ..A.............
1440 28 14 CE 00 C0 15 CE 00 D7 01 26 10 66 10 C1 17 (.........&.f...
1450 94 00 5F 20 9A 00 06 03 DF 15 0F 00 9C 00 5F 10 .._ .........._.
1460 9A 00 28 01 DF 0A 9C 00 25 80 C1 0C 79 00 40 15 ..(.....%...y.@.
1470 C1 17 06 F4 C1 45 00 FC C1 65 0D 00 C0 17 AA 25 .....E...e.....%
1480 C0 0C C0 0C C0 0C C0 E7 AA 25 01 60 81 0C 81 0C .........%.`....
1490 81 0C 5F 10 AA 25 C1 E5 DB 01 81 0C 57 20 0A 00 .._..%......W ..
14A0 02 80 C1 15 0A 00 C1 25 64 00 02 04 C1 15 64 00 .......%d.....d.
14B0 5F 10 60 00 DF 55 01 00 00 F4 81 15 80 15 02 00 _.`..U..........
14C0 41 1C 06 15 C0 17 98 00 03 03 F0 45 08 00 02 00 A..........E....
14D0 5F 10 98 00 F1 55 18 00 02 00 EC 01 C0 17 98 00 _....U..........
14E0 E9 03 1F 0A 98 00 31 10 8C 16 F0 45 08 00 02 00 ......1....E....
14F0 E1 01 41 1C 06 15 5F 1C FE FF 44 00 5F 10 3A 15 ..A..._...D._.:.
1500 5F 10 3C 15 D7 01 9E 16 B2 16 C6 16 DA 16 F2 16 _.<.............
1510 0C 17 28 17 44 17 60 17 7A 17 94 17 AC 17 C6 17 ..(.D.`.z.......
1520 E0 17 FA 17 14 18 B0 34 C0 34 C8 34 D0 34 D8 34 .......4.4.4.4.4
1530 22 25 56 25 42 25 76 25 88 25 00 00 00 00 00 00 "%V%B%v%.%......
1540 DC 14 DC 14 DC 14 DC 14 C0 14 C0 14 C0 14 C0 14 ................
1550 C0 14 C0 14 C0 14 C0 14 C0 14 C0 14 C0 14 C0 14 ................
1560 22 14 22 14 22 14 22 14 22 14 F2 14 F2 14 F2 14 ".".".".".......
1570 F2 14 70 14 F2 14 F2 14                         ..p.....       

;; fn1578: 1578
;;   Called from:
;;     0640 (in fn053A)
;;     0856 (in fn0856)
;;     0C7E (in fn0C76)
;;     0C8A (in fn0C76)
;;     13C8 (in fn13AA)
fn1578 proc
	mov	r0,-(sp)
	mov	r1,-(sp)
	mov	r2,-(sp)
	mov	r3,-(sp)
	mov	r4,-(sp)
	mov	r5,-(sp)
	clr	r4

l1586:
	mov	168C(r4),r5
	beq	15CC

l158C:
	mov	1696(r4),r1
	mov	r5,r3
	add	#000A,r3
	mov	r3,0012(r1)
	mov	@FFFC(r5),r3
	mov	FFFE(r5),r0
	beq	15B8

l15A4:
	tst	r3
	bpl	15AA

l15A8:
	neg	r3

l15AA:
	clr	r2
	jsr	pc,@#126C
	tst	@FFFC(r5)
	bpl	15B8

l15B6:
	neg	r3

l15B8:
	mov	1696(r4),r5
	mov	r3,r0
	mov	#000A,r1
	add	r5,r1
	jsr	pc,@#1674
	tst	(r4)+
	br	1586

l15CC:
	cmp	@#0060,@#0062
	beq	15E4

l15D4:
	mov	#25B8,r1
	mov	@#0060,r0
	mov	r0,@#0062
	jsr	pc,@#1674

l15E4:
	mov	(sp)+,r5
	mov	(sp)+,r4
	mov	(sp)+,r3
	mov	(sp)+,r2
	mov	(sp)+,r1
	mov	(sp)+,r0
	rts	pc

;; fn15F2: 15F2
;;   Called from:
;;     1676 (in fn1674)
;;     167A (in fn1674)
fn15F2 proc
	movb	#0020,(r1)+
	cmp	r0,#2710
	bcc	163A

l15FC:
	movb	#0020,(r1)+
	clr	-(sp)

l1602:
	cmp	r0,#0064
	blt	1650

l1608:
	mov	r0,r3
	clr	r2
	mov	#0064,r0
	jsr	pc,@#126C
	asl	r3
	add	#2814,r3

l161A:
	jsr	pc,@#1658
	jsr	pc,@#1658
	mov	r2,r3
	asl	r3
	add	#2814,r3
	jsr	pc,@#1658
	mov	pc,@sp
	jsr	pc,@#1658
	add	#0002,sp
	rts	pc

l163A:
	mov	pc,-(sp)
	movb	#0030,(r1)+

l1640:
	cmp	r0,#2710
	bcs	1602

l1646:
	incb	FFFF(r1)
	sub	#2710,r0
	br	1640

l1650:
	mov	r0,r2
	mov	#2814,r3
	br	161A

;; fn1658: 1658
;;   Called from:
;;     161A (in fn15F2)
;;     161E (in fn15F2)
;;     162A (in fn15F2)
;;     1630 (in fn15F2)
fn1658 proc
	tst	0002(sp)
	bne	1670

l165E:
	cmpb	@r3,#0030
	bne	166C

l1664:
	movb	#0020,(r1)+
	inc	r3
	rts	pc

l166C:
	mov	sp,0002(sp)

l1670:
	movb	(r3)+,(r1)+
	rts	pc

;; fn1674: 1674
;;   Called from:
;;     15C4 (in fn1578)
;;     15E0 (in fn1578)
fn1674 proc
	tst	r0
	bpl	15F2

l1678:
	neg	r0
	jsr	pc,@#15F2
	mov	r1,r0

l1680:
	cmpb	#0020,-(r0)
	bne	1680

l1686:
	movb	#002D,@r0
	rts	pc
168C                                     F2 16 0C 17             ....
1690 28 17 44 17 00 00 9E 16 B2 16 C6 16 DA 16 A0 F0 (.D.............
16A0 F0 9E 00 00 DA 02 00 80 20 20 20 20 20 20 00 E0 ........      ..
16B0 FC 16 A0 F0 F0 9E FA 00 DA 02 00 80 20 20 20 20 ............    
16C0 20 20 00 E0 16 17 A0 F0 F0 9E F4 01 DA 02 00 80   ..............
16D0 20 20 20 20 20 20 00 E0 32 17 A0 F0 F0 9E EE 02       ..2.......
16E0 DA 02 00 80 20 20 20 20 20 20 00 E0 4E 17 7E 00 ....      ..N.~.
16F0 00 00 B0 F0 F0 9D 84 03 F7 00 00 80 20 48 45 49 ............ HEI
1700 47 48 54 20 00 F7 00 00 5E 00 00 00 B0 F0 F0 9D GHT ....^.......
1710 84 03 E1 00 00 80 20 41 4C 54 49 54 55 44 45 20 ...... ALTITUDE 
1720 00 F7 00 00 5C 00 00 00 B0 F0 F0 9D 84 03 CB 00 ....\...........
1730 00 80 20 44 49 53 54 41 4E 43 45 20 00 F7 00 00 .. DISTANCE ....
1740 68 00 0A 00 B0 F0 F0 9D 84 03 B5 00 00 80 20 46 h............. F
1750 55 45 4C 20 4C 45 46 54 00 F7 00 00 6A 00 00 00 UEL LEFT....j...
1760 B0 F0 F0 9D 84 03 9F 00 00 80 20 57 45 49 47 48 .......... WEIGH
1770 54 20 00 F7 00 00 66 00 00 00 B0 F0 F0 9D 84 03 T ....f.........
1780 89 00 00 80 20 54 48 52 55 53 54 20 00 F7 00 00 .... THRUST ....
1790 46 00 00 00 B0 F0 F0 9D 84 03 73 00 00 80 20 41 F.........s... A
17A0 4E 47 4C 45 00 F7 00 00 58 00 0A 00 B0 F0 F0 9D NGLE....X.......
17B0 84 03 5D 00 00 80 20 56 45 52 20 56 45 4C 00 F7 ..]... VER VEL..
17C0 00 00 54 00 0A 00 B0 F0 F0 9D 84 03 47 00 00 80 ..T.........G...
17D0 20 48 4F 52 20 56 45 4C 00 F7 00 00 52 00 F4 01  HOR VEL....R...
17E0 B0 F0 F0 9D 84 03 31 00 00 80 20 56 45 52 20 41 ......1... VER A
17F0 43 43 00 F7 00 00 50 00 F4 01 B0 F0 F0 9D 84 03 CC....P.........
1800 1B 00 00 80 20 48 4F 52 20 41 43 43 00 F7 00 00 .... HOR ACC....
1810 70 00 3C 00 B0 F0 F0 9D 84 03 05 00 00 80 20 53 p.<........... S
1820 45 43 4F 4E 44 53 00 F7 00 00 00 00 00 00 00 00 ECONDS..........
1830 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
1FF0 00 00 00 00 00 00 00 00 00 00 00 00 50 9F 1E 00 ............P...
2000 58 02 00 80 42 4F 59 2C 20 41 52 45 20 59 4F 55 X...BOY, ARE YOU
2010 D8 87 20 49 4E 45 50 54 00 F7 00 00 50 9F 0D 02 .. INEPT....P...
2020 58 02 00 80 20 20 59 4F 55 20 48 41 56 45 20 4A X...  YOU HAVE J
2030 55 53 54 20 43 52 41 53 48 45 44 00 50 9F 0D 02 UST CRASHED.P...
2040 3A 02 00 80 49 4E 54 4F 20 54 48 45 20 45 44 47 :...INTO THE EDG
2050 45 20 4F 46 20 54 48 45 20 4D 4F 4F 4E 00 00 F7 E OF THE MOON...
2060 00 00 50 9F 32 00 8A 02 00 80 53 4F 52 52 59 2C ..P.2.....SORRY,
2070 20 42 55 54 20 57 48 45 4E 20 59 4F 55 20 4C 4F  BUT WHEN YOU LO
2080 53 45 20 54 56 20 43 4F 56 45 52 41 47 45 2C 20 SE TV COVERAGE, 
2090 59 4F 55 20 41 4C 53 4F 20 4C 4F 53 45 20 59 4F YOU ALSO LOSE YO
20A0 55 52 20 46 55 45 4C 00 00 F7 00 00 58 9F 64 00 UR FUEL.....X.d.
20B0 02 00 00 80 54 4F 4F 20 46 41 53 54 2E 20 59 4F ....TOO FAST. YO
20C0 55 27 52 45 20 47 4F 49 4E 47 20 54 4F 20 43 52 U'RE GOING TO CR
20D0 41 53 48 00 00 F7 00 00 50 9F 64 00 BC 02 00 80 ASH.....P.d.....
20E0 42 45 54 54 45 52 20 53 54 41 52 54 20 53 4C 4F BETTER START SLO
20F0 57 49 4E 47 20 49 54 20 55 50 20 50 52 45 54 54 WING IT UP PRETT
2100 59 20 53 4F 4F 4E 00 F7 00 00 50 9F 64 00 02 00 Y SOON....P.d...
2110 00 80 54 41 4B 45 20 49 54 20 4E 49 43 45 20 41 ..TAKE IT NICE A
2120 4E 44 20 45 41 53 59 2E 20 41 20 50 45 52 46 45 ND EASY. A PERFE
2130 43 54 20 4C 41 4E 44 49 4E 47 20 49 53 20 55 4E CT LANDING IS UN
2140 44 45 52 20 38 20 46 50 53 00 00 F7 00 00 50 9F DER 8 FPS.....P.
2150 64 00 58 02 00 80 46 41 4E 54 41 53 54 49 43 2C d.X...FANTASTIC,
2160 20 41 20 50 45 52 46 45 43 54 20 4C 41 4E 44 49  A PERFECT LANDI
2170 4E 47 00 F7 00 00 50 9F 64 00 58 02 00 80 43 4F NG....P.d.X...CO
2180 4E 47 52 41 54 55 4C 41 54 49 4F 4E 53 20 4F 4E NGRATULATIONS ON
2190 20 41 20 47 4F 4F 44 20 4C 41 4E 44 49 4E 47 00  A GOOD LANDING.
21A0 00 F7 00 00 50 9F 64 00 58 02 00 80 54 48 45 20 ....P.d.X...THE 
21B0 4C 41 4E 44 49 4E 47 20 57 41 53 20 41 20 4C 49 LANDING WAS A LI
21C0 54 54 4C 45 20 46 41 53 54 00 00 F7 00 00 50 9F TTLE FAST.....P.
21D0 64 00 58 02 00 80 54 48 45 20 4C 41 4E 44 49 4E d.X...THE LANDIN
21E0 47 20 57 41 53 20 54 4F 4F 20 46 41 53 54 20 41 G WAS TOO FAST A
21F0 4E 44 20 44 41 4D 41 47 45 20 57 41 53 20 44 4F ND DAMAGE WAS DO
2200 4E 45 20 54 4F 20 54 48 45 20 53 48 49 50 00 F7 NE TO THE SHIP..
2210 00 00 58 9F 64 00 26 02 00 80 57 45 4C 4C 2C 20 ..X.d.&...WELL, 
2220 59 4F 55 20 43 45 52 54 41 49 4E 4C 59 20 42 4C YOU CERTAINLY BL
2230 45 57 20 54 48 41 54 20 4F 4E 45 2E 20 54 48 45 EW THAT ONE. THE
2240 52 45 20 57 45 52 45 20 4E 4F 20 53 55 52 56 49 RE WERE NO SURVI
2250 52 4F 52 53 00 F7 00 00 50 9F 64 00 3A 02 00 80 RORS....P.d.:...
2260 42 55 54 20 54 48 45 20 41 4E 47 4C 45 20 57 41 BUT THE ANGLE WA
2270 53 20 54 4F 4F 20 47 52 45 41 54 20 41 4E 44 20 S TOO GREAT AND 
2280 54 48 45 20 53 48 49 50 20 54 49 50 50 45 44 20 THE SHIP TIPPED 
2290 4F 56 45 52 50 9F 64 00 1C 02 00 80 53 4F 52 52 OVERP.d.....SORR
22A0 59 2C 20 42 55 54 20 54 48 45 52 45 20 57 45 52 Y, BUT THERE WER
22B0 45 20 98 87 4E 4F 50 87 20 53 55 52 56 49 56 4F E ..NOP. SURVIVO
22C0 52 53 00 F7 00 00 50 9F 64 00 3A 02 00 80 42 55 RS....P.d.:...BU
22D0 54 20 54 48 45 20 48 4F 52 49 5A 4F 4E 54 41 4C T THE HORIZONTAL
22E0 20 56 45 4C 4F 43 49 54 59 20 57 41 53 20 54 4F  VELOCITY WAS TO
22F0 4F 20 47 52 45 41 54 2C 20 41 4E 44 20 59 4F 55 O GREAT, AND YOU
2300 20 43 52 41 53 48 45 44 20 41 4E 59 57 41 59 00  CRASHED ANYWAY.
2310 00 E0 94 22 50 9F 64 00 3A 02 00 80 42 55 54 20 ..."P.d.:...BUT 
2320 54 48 45 20 54 45 52 52 41 49 4E 20 49 53 20 54 THE TERRAIN IS T
2330 4F 4F 20 52 4F 55 47 48 2C 20 41 4E 44 20 59 4F OO ROUGH, AND YO
2340 55 20 54 49 50 50 45 44 20 4F 56 45 52 00 00 E0 U TIPPED OVER...
2350 94 22 50 9F 64 00 3A 02 00 80 59 4F 55 20 4A 55 ."P.d.:...YOU JU
2360 53 54 20 43 52 41 53 48 45 44 20 49 4E 54 4F 20 ST CRASHED INTO 
2370 54 48 41 54 20 52 4F 43 4B 00 00 E0 94 22 50 9F THAT ROCK...."P.
2380 64 00 3A 02 00 80 59 4F 55 20 4A 55 53 54 20 43 d.:...YOU JUST C
2390 52 41 53 48 45 44 20 4F 4E 20 54 4F 50 20 4F 46 RASHED ON TOP OF
23A0 20 41 4E 20 4F 4C 44 20 4C 55 4E 41 52 20 4D 4F  AN OLD LUNAR MO
23B0 44 55 4C 45 00 E0 94 22 50 9F 32 00 03 00 00 80 DULE..."P.2.....
23C0 59 4F 55 20 48 41 56 45 20 4A 55 53 54 20 56 41 YOU HAVE JUST VA
23D0 50 4F 52 49 5A 45 44 20 41 20 50 52 45 56 49 4F PORIZED A PREVIO
23E0 55 53 4C 59 20 50 4C 41 4E 54 45 44 20 41 4D 45 USLY PLANTED AME
23F0 52 49 43 41 4E 20 46 4C 41 47 00 F7 00 00 50 9F RICAN FLAG....P.
2400 64 00 3A 02 00 80 4E 49 43 45 20 57 4F 52 4B 2E d.:...NICE WORK.
2410 20 59 4F 55 20 4A 55 53 54 20 43 52 41 53 48 45  YOU JUST CRASHE
2420 44 20 49 4E 54 4F 20 41 20 50 52 45 56 49 4F 55 D INTO A PREVIOU
2430 53 4C 59 20 43 52 41 53 48 45 44 20 53 48 49 50 SLY CRASHED SHIP
2440 00 E0 94 22 50 9F 0A 00 3A 02 00 80 57 65 6C 6C ..."P...:...Well
2450 2C 20 79 6F 75 27 76 65 20 6A 75 73 74 20 64 65 , you've just de
2460 73 74 72 6F 79 65 64 20 74 68 65 20 6F 6E 6C 79 stroyed the only
2470 20 4D 61 63 44 6F 6E 61 6C 64 27 73 50 9F 0A 00  MacDonald'sP...
2480 1C 02 00 80 6F 6E 20 74 68 65 20 6D 6F 6F 6E 2E ....on the moon.
2490 20 57 68 61 74 20 61 20 43 4C 4F 44 2E 00 00 F7  What a CLOD....
24A0 00 00 50 9F 96 00 02 00 00 80 B0 F0 54 57 4F 20 ..P.........TWO 
24B0 43 48 45 45 53 45 42 55 52 47 45 52 53 20 41 4E CHEESEBURGERS AN
24C0 44 20 41 20 42 49 47 20 4D 41 43 20 54 4F 20 47 D A BIG MAC TO G
24D0 4F 2E 00 F7 00 00 50 9F 32 00 02 00 00 80 B0 F0 O.....P.2.......
24E0 54 48 41 54 27 53 20 4F 4E 45 20 53 4D 41 4C 4C THAT'S ONE SMALL
24F0 20 53 54 45 50 20 46 4F 52 20 41 20 4D 41 4E 2C  STEP FOR A MAN,
2500 20 4F 4E 45 20 47 49 41 4E 54 20 4C 45 41 50 20  ONE GIANT LEAP 
2510 46 4F 52 20 4D 41 4E 4B 49 4E 44 2E 00 F7 00 00 FOR MANKIND.....
2520 F1 FF 10 98 A0 F0 B1 03 77 01 64 96 14 60 00 00 ........w.d..`..
2530 0C 40 08 00 0C 20 08 20 0C 40 08 20 00 F7 00 00 .@... . .@. ....
2540 9C FF 10 98 A0 F0 B1 03 4A 01 64 96 28 60 00 00 ........J.d.(`..
2550 00 E0 30 25 0F 00 10 98 A0 F0 BB 03 77 01 64 96 ..0%........w.d.
2560 14 40 00 00 0C 60 08 00 0C 00 08 20 0C 60 08 20 .@...`..... .`. 
2570 00 F7 00 00 64 00 10 98 A0 F0 BB 03 4A 01 64 96 ....d.......J.d.
2580 28 40 00 00 00 E0 64 25 F4 9D 80 F0 B3 03 BC 02 (@....d%........
2590 60 90 00 40 FA 20 03 00 FA 00 00 40 FA 20 03 00 `..@. .....@. ..
25A0 FA 00 00 40 FA 20 40 98 AC 03 00 00 00 90 2B 60 ...@. @.......+`
25B0 00 00 37 20 00 00 00 80 20 20 20 20 20 20 25 20 ..7 ....      % 
25C0 00 F7 00 00 58 9E 5E 01 BC 02 00 80 46 55 45 4C ....X.^.....FUEL
25D0 20 4C 4F 57 00 F7 00 00 D7 9D A0 F0 00 00 00 00  LOW............
25E0 00 88 48 27 08 63 06 42 06 40 02 41 04 41 06 43 ..H'.c.B.@.A.A.C
25F0 04 40 02 42 00 41 42 41 42 43 42 42 46 41 46 43 .@.B.ABABCBBFAFC
2600 44 40 44 41 44 61 48 61 00 F7 00 00 D4 9F A0 F0 D@DADaHa........
2610 00 00 00 00 00 88 00 09 00 8E 1E 40 00 5B 5E 40 ...........@.[^@
2620 00 7B D4 8A 00 06 08 40 00 43 48 40 80 29 00 90 .{.....@.CH@.)..
2630 49 20 00 00 00 F7 3C 26 00 88 00 0B D4 8F C3 28 I ....<&.......(
2640 8A 40 89 40 86 40 85 40 84 40 82 40 84 40 05 41 .@.@.@.@.@.@.@.A
2650 81 40 04 41 82 41 81 40 00 41 C1 40 C2 41 44 41 .@.A.A.@.A.@.ADA
2660 C1 40 45 41 C4 40 C2 40 C4 40 C5 40 C6 40 C9 40 .@EA.@.@.@.@.@.@
2670 CA 40 83 28 00 F7 00 00 D0 9D 00 00 00 00 00 88 .@.(............
2680 00 01 04 61 44 62 04 02 05 40 80 40 81 40 01 40 ...aDb...@.@.@.@
2690 81 60 00 61 C1 60 41 40 C1 40 80 40 41 00 83 42 .`.a.`A@.@.@A..B
26A0 C5 22 03 63 00 F7 00 00 D4 9A A0 F0 00 00 00 00 .".c............
26B0 00 88 12 40 D7 95 10 40 00 00 00 40 08 20 10 60 ...@...@...@. .`
26C0 00 00 D6 94 00 00 03 00 10 40 00 00 00 00 02 00 .........@......
26D0 10 60 00 00 00 F7 00 00 BC 02 80 F0 BC 02 54 8E .`............T.
26E0 56 02 FA 00 5E 02 F2 08 5E 02 F2 14 5E 02 FA 1D V...^...^...^...
26F0 5E 02 06 1D 5E 02 0E 14 5E 02 0E 08 5E 02 06 00 ^...^...^...^...
2700 5E 02 FA 00 56 02 EF 00 5E 02 EF F0 5E 02 11 F0 ^...V...^...^...
2710 5E 02 11 00 5E 02 EF 00 BC 02 54 8F 5E 02 E0 E8 ^...^.....T.^...
2720 56 02 11 00 5E 02 20 E8 BC 02 D4 8C 56 02 EF F2 V...^. .....V...
2730 5E 02 E4 EE 56 02 11 F2 5E 02 1C EE BC 02 54 8E ^...V...^.....T.
2740 56 02 24 E8 5E 02 1C E8 56 02 E4 E8 5E 02 DC E8 V.$.^...V...^...
2750 56 02 FD F0 5E 02 F9 EB 5E 02 07 EB 5E 02 03 F0 V...^...^...^...
2760 56 02 00 00 C0 02 00 E2 E1 E0 DE DC DA D7 D4 D1 V...............
2770 CE CB C8 00 01 03 06 04 03 01 FE FA F9 FB FE 02 ................
2780 03 05 06 02 01 FF FC FA FB FD 00 04 05 07 04 00 ................
2790 FF FD FF EC F0 F3 F6 F9 FC FE 00 02 04 07 0A 0D ................
27A0 10 14 BC 02 80 F0 BC 02 00 00 56 02 FA EB 5E 02 ..........V...^.
27B0 00 00 5E 02 FB EB 5E 02 00 00 5E 02 FC EB 5E 02 ..^...^...^...^.
27C0 00 00 5E 02 FD EB 5E 02 00 00 5E 02 FE EB 5E 02 ..^...^...^...^.
27D0 00 00 5E 02 FF EB 5E 02 00 00 5E 02 00 EB 5E 02 ..^...^...^...^.
27E0 00 00 5E 02 01 EB 5E 02 00 00 5E 02 02 EB 5E 02 ..^...^...^...^.
27F0 00 00 5E 02 03 EB 5E 02 00 00 5E 02 04 EB 5E 02 ..^...^...^...^.
2800 00 00 5E 02 05 EB 5E 02 00 00 5E 02 06 EB 56 02 ..^...^...^...V.
2810 00 00 C0 02 30 30 30 31 30 32 30 33 30 34 30 35 ....000102030405
2820 30 36 30 37 30 38 30 39 31 30 31 31 31 32 31 33 0607080910111213
2830 31 34 31 35 31 36 31 37 31 38 31 39 32 30 32 31 1415161718192021
2840 32 32 32 33 32 34 32 35 32 36 32 37 32 38 32 39 2223242526272829
2850 33 30 33 31 33 32 33 33 33 34 33 35 33 36 33 37 3031323334353637
2860 33 38 33 39 34 30 34 31 34 32 34 33 34 34 34 35 3839404142434445
2870 34 36 34 37 34 38 34 39 35 30 35 31 35 32 35 33 4647484950515253
2880 35 34 35 35 35 36 35 37 35 38 35 39 36 30 36 31 5455565758596061
2890 36 32 36 33 36 34 36 35 36 36 36 37 36 38 36 39 6263646566676869
28A0 37 30 37 31 37 32 37 33 37 34 37 35 37 36 37 37 7071727374757677
28B0 37 38 37 39 38 30 38 31 38 32 38 33 38 34 38 35 7879808182838485
28C0 38 36 38 37 38 38 38 39 39 30 39 31 39 32 39 33 8687888990919293
28D0 39 34 39 35 39 36 39 37 39 38 39 39 CE 02 CE 02 949596979899....
28E0 CE 02 EE 02 EE 02 EE 02 0E 03 0E 03 0E 03 2E 03 ................
28F0 0C 03 EE 02 BF 02 8A 02 6C 02 3F 02 F4 01 A4 01 ........l.?.....
2900 90 01 5E 01 0E 01 F6 00 C8 00 B4 00 6B 00 18 00 ..^.........k...
2910 36 00 35 00 33 00 52 00 50 00 4E 00 6D 00 6B 00 6.5.3.R.P.N.m.k.
2920 4A 00 48 00 46 00 45 00 63 00 62 00 80 00 7E 00 J.H.F.E.c.b...~.
2930 7D 00 7B 00 7A 00 98 00 B6 00 AE 00 A6 00 9E 00 }.{.z...........
2940 B5 00 AD 00 A5 00 9C 00 94 00 8C 00 83 00 5B 00 ..............[.
2950 53 00 4B 00 B6 00 A1 00 EC 00 78 00 43 00 AE 00 S.K.......x.C...
2960 99 00 C5 00 50 00 3B 00 62 03 7B 00 C9 00 FA 00 ....P.;.b.{.....
2970 13 01 2C 01 77 01 9A 01 43 02 9E 03 28 07 73 07 ..,.w...C...(.s.
2980 DD 07 47 08 D1 08 5B 09 E5 09 4F 0A B9 0A 43 0B ..G...[...O...C.
2990 CD 0B 38 0C 9A 0C 7D 0C 9F 0C 62 0C 44 0C 47 0C ..8...}...b.D.G.
29A0 49 0C 6C 0C AE 0C 51 0C B4 0C D6 0C 19 0D 3B 0D I.l...Q.......;.
29B0 3E 0D 60 0D C3 0D 65 0D 70 0D 7A 0D 65 0D 4F 0D >.`...e.p.z.e.O.
29C0 3A 0D 44 0D 2F 0D F9 0C E4 0C AE 0C B9 0C 83 0C :.D./...........
29D0 6E 0C 38 0C 23 0C 0D 0C F8 0B 02 0C 0D 0C 17 0C n.8.#...........
29E0 7B 0C 5F 0C 43 0C A7 0C 8A 0B 6E 0B 12 0B 96 0A {._.C.....n.....
29F0 BA 0A 9E 0A C1 0A A5 09 09 0A CD 09 D1 09 14 0A ................
2A00 18 0A 3C 0A A0 0A 04 0B 67 0B 2B 0B 8F 0A B3 0A ..<.....g.+.....
2A10 97 0B 5A 0B 3E 0C E2 0B 66 0B 2A 0B C7 0A A4 09 ..Z.>...f.*.....
2A20 C2 09 5F 09 5C 09 3A 09 17 08 94 07 B2 07 0F 08 .._.\.:.........
2A30 EC 07 8A 07 07 07 84 06 E2 06 1F 07 7C 06 5A 06 ............|.Z.
2A40 F7 05 94 05 72 06 4F 06 8C 06 EA 06 A7 06 64 06 ....r.O.......d.
2A50 82 06 3F 06 9C 05 7A 05 97 05 34 05 92 05 6F 06 ..?...z...4...o.
2A60 EC 05 4A 05 27 05 64 05 42 04 1F 04 7C 04 7A 04 ..J.'.d.B...|.z.
2A70 B7 04 94 04 B2 04 2F 04 0C 03 EA 02 C7 02 A5 03 ....../.........
2A80 44 04 44 05 03 06 23 07 42 07 42 08 21 09 C1 09 D.D...#.B.B.!...
2A90 40 0A C0 0A 1F 0B BF 0B 5E 0C 5E 0D FD 0D 7D 0E @.......^.^...}.
2AA0 7C 0F 1C 10 DB 10 5B 11 61 11 67 12 ED 12 13 13 |.....[.a.g.....
2AB0 79 13 BF 13 C5 13 8C 14 F2 14 D8 15 5E 16 E4 16 y...........^...
2AC0 6A 17 10 18 73 18 16 19 B8 19 3B 1A DD 1A 60 1B j...s.....;...`.
2AD0 E2 1B 85 1C 08 1D 8A 1D 0D 1E 6F 1E 12 1F 94 1F ..........o.....
2AE0 17 20 99 20 D0 20 E6 21 3D 22 93 22 49 23 C0 23 . . . .!="."I#.#
2AF0 96 24 AC 24 43 25 D9 26 CF 27 89 28 A3 29 1D 2A .$.$C%.&.'.(.).*
2B00 37 2B 30 2C 6A 2C 84 2C 7E 2D F8 2D D1 2E AB 2F 7+0,j,.,~-.-.../
2B10 45 30 3F 31 39 32 A5 32 11 33 3D 33 89 33 B6 33 E0?192.2.3=3.3.3
2B20 E2 33 2E 34 7A 34 A6 34 F3 34 3F 35 8B 35 F7 35 .3.4z4.4.4?5.5.5
2B30 63 36 8F 36 CD 36 AC 36 29 37 87 37 E4 37 82 38 c6.6.6.6)7.7.7.8
2B40 FF 38 9D 39 1A 3A 98 3A A2 39 0C 39 F6 37 60 37 .8.9.:.:.9.9.7`7
2B50 0A 37 74 36 5E 35 A8 34 52 34 3C 33 04 31 CD 2C .7t6^5.4R4<3.1.,
2B60 56 29 1F 27 C8 23 00 23 F8 21 F0 20 08 20 00 1F V).'.#.#.!. . ..
2B70 5A 1E 55 1D CF 1C 2A 1C A4 1B FF 1A 59 1A F4 19 Z.U...*.....Y...
2B80 4E 19 29 18 83 17 7E 16 18 16 73 15 8D 14 E8 13 N.)...~...s.....
2B90 03 13 68 12 2E 11 B4 0F FA 0E E0 0C C6 0B 6C 0A ..h...........l.
2BA0 52 0A 78 09 5E 09 A4 08 8A 08 30 07 24 07 D8 07 R.x.^.....0.$...
2BB0 6C 08 E0 08 34 09 08 09 DC 08 B0 08 E4 08 18 09 l...4...........
2BC0 6D 09 CF 09 71 0A 14 0B B6 0B 39 0C 9B 0C 3E 0D m...q.....9...>.
2BD0 A0 0D 02 0E 85 0E 27 0F AA 0F 0C 10 8F 10 11 11 ......'.........
2BE0 B3 11 56 12 D8 12 7B 13 FD 13 60 14 C2 14 45 15 ..V...{...`...E.
2BF0 A7 15 49 16 AC 16 4E 17 D1 17 33 18 96 18 F8 18 ..I...N...3.....
2C00 7A 19 1D 1A 9F 1A 02 1B A4 1B 47 1C A9 1C 0C 1D z.........G.....
2C10 A2 1B 99 1A 30 19 E6 17 FD 15 94 14 8A 13 A1 12 ....0...........
2C20 B7 11 AE 10 45 0F BB 0D 24 0C EC 0A 55 09 1D 08 ....E...$...U...
2C30 E5 05 2E 05 36 04 7F 03 47 02 90 00 40 00 F1 FF ....6...G...@...
2C40 81 FF 8E FF 9B FF C8 FF F6 FF 02 00 F1 FF 1D 00 ................
2C50 4A 00 38 00 45 00 52 00 5F 00 58 00 31 00 49 00 J.8.E.R._.X.1.I.
2C60 22 00 3B 00 33 00 2C 00 44 00 1D 00 16 00 0E 00 ".;.3.,.D.......
2C70 07 00 20 00 55 00 AB 00 00 01 36 01 CC 01 2B 02 .. .U.....6...+.
2C80 6A 02 A9 02 08 03 47 03 66 03 A5 03 E4 03 44 04 j.....G.f.....D.
2C90 A3 04 E2 04 41 05 80 05 BF 05 1E 06 3D 06 5C 06 ....A.......=.\.
2CA0 9C 06 DB 06 1A 07 59 07 98 07 D7 07 16 08 55 08 ......Y.......U.
2CB0 94 08 D4 08 13 09 52 09 B1 09 F0 09 2F 0A 6E 0A ......R...../.n.
2CC0 AD 0A CC 0A 0C 0B 2B 0B 6A 0B A9 0B 08 0C 27 0C ......+.j.....'.
2CD0 66 0C A5 0C E4 0C 44 0D 1C 0D B4 0C 8D 0B 65 0C f.....D.......e.
2CE0 3E 0D 76 0D 4E 0D E7 0C 9F 0C 78 0D 46 0C 54 0C >.v.N.....x.F.T.
2CF0 23 0C 11 0C 80 0B 13 0B A6 0A 39 0A AC 09 3F 09 #.........9...?.
2D00 45 09 6A 09 CF 09 14 0A 3A 0A 9F 0A A4 0A E9 0A E.j.....:.......
2D10 EE 0A D4 0A B4 0A 95 0A 55 0A 36 0A 16 0A F7 09 ........U.6.....
2D20 D7 09 B8 09 98 09 79 09 59 09 3A 09 3A 09 3B 09 ......y.Y.:.:.;.
2D30 3B 09 F8 08 B4 08 B0 08 2C 08 69 08 F0 07 18 08 ;.......,.i.....
2D40 60 08 C8 08 50 08 B8 07 E0 07 48 07 90 07 78 07 `...P.....H...x.
2D50 40 07 A8 06 50 06 D8 05 40 05 28 05 D0 04 B8 03 @...P...@.(.....
2D60 E0 03 08 04 70 04 18 04 20 04 C8 03 B0 02 18 03 ....p... .......
2D70 00 03 E8 02 70 02 57 01 2A 01 BC 01 EE 01 21 01 ....p.W.*.....!.
2D80 D3 00 A5 00 98 00 4A 00 9C 00 AE 00 E1 00 93 00 ......J.........
2D90 C6 FF D9 FF AB FF 5D FF 8F 00 C1 01 F3 02 A6 03 ......].........
2DA0 13 03 A1 02 8E 02 7C 02 6A 02 D7 01 45 01 93 01 ......|.j...E...
2DB0 A0 01 0E 01 DC 00 69 00 57 00 65 00 F3 FF C1 FF ......i.W.e.....
2DC0 2E 00 9B 00 89 00 B7 00 A4 00 32 00 1F 00 0D 00 ..........2.....
2DD0 7B 00 68 00 D6 00 E4 00 11 01 1F 01 8D 00 FA 00 {.h.............
2DE0 48 01 16 01 43 01 91 01 FF 01 6C 01 DA 00 C8 00 H...C.....l.....
2DF0 D4 00 A0 00 AC 00 98 00 84 00 70 00 5C 00 48 00 ..........p.\.H.
2E00 34 00 3F 00 5E 00 5D 00 5C 00 5A 00 59 00 78 00 4.?.^.].\.Z.Y.x.
2E10 77 00 75 00 74 00 53 00 51 00 50 00 4F 00 6E 00 w.u.t.S.Q.P.O.n.
2E20 6C 00 8B 00 8A 00 88 00 67 00 86 00 A5 00 83 00 l.......g.......
2E30 A2 00 C1 00 9F 00 B9 00 B3 00 8C 00 66 00 80 00 ............f...
2E40 99 00 93 00 AC 00 86 00 A0 00 99 00 B3 00 CC 00 ................
2E50 A6 00 A0 00 99 00 B3 00 AC 00 A6 00 A0 00 99 00 ................
2E60 7D 00 6E 00 5F 00 50 00 55 00 52 00 57 00 54 00 }.n._.P.U.R.W.T.
2E70 51 00 56 00 67 00 7D 00 8C 00 98 00 87 00 82 00 Q.V.g.}.........
2E80 7D 00 58 00 53 00 2E 00 28 00 23 00 1E 00 19 00 }.X.S...(.#.....
2E90 34 00 2F 00 2A 00 25 00 3F 00 3D 00 5A 00 78 00 4./.*.%.?.=.Z.x.
2EA0 75 00 73 00 70 00 4E 00 4B 00 68 00 46 00 43 00 u.s.p.N.K.h.F.C.
2EB0 21 00 3E 00 3C 00 39 00 37 00 34 00 51 00 6F 00 !.>.<.9.7.4.Q.o.
2EC0 8C 00 8A 00 A7 00 A5 00 A2 00 C0 00 89 00 B3 00 ................
2ED0 7C 00 E6 00 50 01 79 01 83 01 2D 01 96 01 20 01 |...P.y...-... .
2EE0 AA 01 D3 01 1D 02 A7 01 10 02 FA 01 C4 01 6D 01 ..............m.
2EF0 37 01 C1 00 2A 01 B4 01 FE 01 E7 01 D1 01 5B 01 7...*.........[.
2F00 04 01 4E 01 B8 01 A1 01 AB 01 55 01 3E 01 E8 00 ..N.......U.>...
2F10 92 00 1B 00 45 00 AF 00 18 01 02 01 CC 00 15 01 ....E...........
2F20 9F 01 A9 01 D2 01 9C 01 46 01 CF 01 19 02 23 02 ........F.....#.
2F30 1E 03 99 02 94 02 50 02 2B 02 86 02 02 02 FD 02 ......P.+.......
2F40 F8 02 F3 02 6B 03 E2 03 39 04 70 04 C7 04 1F 05 ....k...9.p.....
2F50 76 05 CD 05 44 06 9B 06 D3 06 2A 07 81 07 B8 07 v...D.....*.....
2F60 2F 08 87 08 BE 08 35 09 6C 09 A4 09 76 09 88 09 /.....5.l...v...
2F70 DB 09 4D 09 20 09 4C 09 58 09 64 09 91 09 BD 09 ..M. .L.X.d.....
2F80 C9 09 D5 09 02 0A 0E 0A 1A 0A 26 0A 53 0A 7F 0A ..........&.S...
2F90 AB 0A D7 0A 21 0B 4A 0B 74 0B DD 0A C7 0A 10 0B ....!.J.t.......
2FA0 FA 0A E3 0A CD 09 B6 0A C0 0A 09 0B 53 0B 1C 0B ............S...
2FB0 06 0B EF 09 D9 0A 42 0B 4C 0B D5 0A 3F 0B E8 0A ......B.L...?...
2FC0 D2 0A BB 0A 45 0A 6E 0A 57 09 C1 09 0A 0A F4 0A ....E.n.W.......
2FD0 DD 0A 07 0B F0 09 BA 09 83 09 AD 09 D6 09 40 09 ..............@.
2FE0 09 09 73 09 DC 09 C6 09 AF 09 F9 09 02 0A EC 09 ..s.............
2FF0 F5 09 1F 0A 68 0A 52 09 97 09 7C 09 61 08 47 08 ....h.R...|.a.G.
3000 6C 08 D1 07 B6 07 9C 08 61 08 46 09 AC 09 00 50 l.......a.F....P
3010 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 ................
3020 05 55 00 00 00 00 00 00 00 05 00 00 55 05 55 00 .U..........U.U.
3030 00 05 05 55 50 05 55 55 50 05 05 00 05 50 05 00 ...UP.UUP....P..
3040 05 00 00 55 00 05 55 05 55 05 05 50 55 55 55 05 ...U..U.U..PUUU.
3050 55 55 55 05 55 05 55 55 55 05 05 55 55 50 55 55 UUU.U.UUU..UUPUU
3060 55 50 00 55 05 05 55 55 55 55 05 55 05 50 05 55 UP.U..UUUU.U.P.U
3070 55 55 55 55 55 55 05 00 50 00 00 00 05 00 00 00 UUUUUU..P.......
3080 00 50 55 00 00 00 00 00 05 00 00 05 00 05 00 00 .PU.............
3090 05 05 00 00 00 00 00 00 05 00 00 00 00 00 00 50 ...............P
30A0 00 00 00 00 00 00 00 00 00 55 55 50 55 05 55 05 .........UUPU.U.
30B0 55 55 55 55 55 55 55 55 55 55 55 55 55 55 55 55 UUUUUUUUUUUUUUUU
30C0 55 55 00 00 00 50 55 55 00 00 00 00 00 00 00 00 UU...PUU........
30D0 05 00 05 00 05 00 00 00 00 00 00 55 55 55 55 55 ...........UUUUU
30E0 55 55 55 50 55 55 05 05 00 00 05 00 50 50 00 00 UUUPUU......PP..
30F0 00 00 00 00 50 50 05 00 50 05 05 55 05 00 05 05 ....PP..P..U....
3100 05 00 05 00 00 00 00 00 00 50 00 00 00 55 55 55 .........P...UUU
3110 55 55 55 55 50 50 55 55 55 55 50 55 55 55 05 55 UUUUPPUUUUPUUU.U
3120 50 50 55 55 55 55 55 55 50 55 55 55 55 55 55 55 PPUUUUUUPUUUUUUU
3130 55 50 55 05 05 05 50 50 55 55 55 55 55 55 55 00 UPU...PPUUUUUUU.
3140 00 00 00 00 00 00 00 50 50 00 00 00 00 00 00 50 .......PP......P
3150 00 00 00 00 00 05 00 00 05 00 00 00 00 00 00 00 ................
3160 00 50 55 00 00 00 00 00 00 00 00 55 05 00 00 00 .PU........U....
3170 00 00 76 06 50 55 50 00 50 50 00 55 55 00 05 00 ..v.PUP.PP.UU...
3180 50 05 55 50 00 50 50 05 00 00 50 05 00 50 50 00 P.UP.PP...P..PP.
3190 00 50 00 55 50 00 50 00 50 00 00 05 00 00 00 00 .P.UP.P.P.......
31A0 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 ................
31B0 50 05 00 00 00 55 00 50 05 00 00 50 55 55 50 55 P....U.P...PUUPU
31C0 55 55 55 55 55 50 50 55 05 55 55 55 55 00 55 55 UUUUUPPU.UUUU.UU
31D0 55 55 55 55 55 55 55 55 55 55 05 00 00 00 1E 01 UUUUUUUUUU......
31E0 3C 02 59 03 77 04 94 05 B1 06 CD 07 E8 08 03 0A <.Y.w...........
31F0 1D 0B 36 0C 4E 0D 66 0E 7C 0F 90 10 A4 11 B6 12 ..6.N.f.|.......
3200 C7 13 D6 14 E4 15 F0 16 FA 17 02 19 08 1A 0C 1B ................
3210 0E 1C 0E 1D 0C 1E 07 1F 00 20 F6 20 EA 21 DB 22 ......... . .!."
3220 CA 23 B5 24 9E 25 84 26 67 27 47 28 23 29 FD 29 .#.$.%.&g'G(#).)
3230 D3 2A A6 2B 75 2C 41 2D 0A 2E CE 2E 90 2F 4D 30 .*.+u,A-...../M0
3240 07 31 BD 31 6F 32 1D 33 C7 33 6D 34 0F 35 AD 35 .1.1o2.3.3m4.5.5
3250 46 36 DC 36 6D 37 FA 37 82 38 06 39 86 39 01 3A F6.6m7.7.8.9.9.:
3260 78 3A EA 3A 57 3B C0 3B 24 3C 83 3C DE 3C 34 3D x:.:W;.;$<.<.<4=
3270 85 3D D2 3D 19 3E 5C 3E 9A 3E D3 3E 07 3F 36 3F .=.=.>\>.>.>.?6?
3280 61 3F 86 3F A6 3F C2 3F D8 3F EA 3F F6 3F FE 3F a?.?.?.?.?.?.?.?
3290 00 40 FE 3F F6 3F EA 3F D8 3F C2 3F A6 3F 86 3F .@.?.?.?.?.?.?.?
32A0 61 3F 36 3F 07 3F D3 3E 9A 3E 5C 3E 19 3E D2 3D a?6?.?.>.>\>.>.=
32B0 85 3D 34 3D DE 3C 83 3C 24 3C C0 3B 57 3B EA 3A .=4=.<.<$<.;W;.:
32C0 78 3A 01 3A 86 39 06 39 82 38 FA 37 6D 37 DC 36 x:.:.9.9.8.7m7.6
32D0 46 36 AD 35 0F 35 6D 34 C7 33 1D 33 6F 32 BD 31 F6.5.5m4.3.3o2.1
32E0 07 31 4D 30 90 2F CE 2E 0A 2E 41 2D 75 2C A6 2B .1M0./....A-u,.+
32F0 D3 2A FD 29 23 29 47 28 67 27 84 26 9E 25 B5 24 .*.)#)G(g'.&.%.$
3300 CA 23 DB 22 EA 21 F6 20 00 20 07 1F 0C 1E 0E 1D .#.".!. . ......
3310 0E 1C 0C 1B 08 1A 02 19 FA 17 F0 16 E4 15 D6 14 ................
3320 C7 13 B6 12 A4 11 90 10 7C 0F 66 0E 4E 0D 36 0C ........|.f.N.6.
3330 1D 0B 03 0A E8 08 CD 07 B1 06 94 05 77 04 59 03 ............w.Y.
3340 3C 02 1E 01 00 00 E4 FE C6 FD A7 FC 8B FB 6E FA <.............n.
3350 51 F9 35 F8 18 F7 FD F5 E3 F4 CA F3 B2 F2 9C F1 Q.5.............
3360 86 F0 70 EF 5C EE 4A ED 3B EC 2A EB 1E EA 11 E9 ..p.\.J.;.*.....
3370 08 E8 00 E7 FA E5 F4 E4 F2 E3 F2 E2 F6 E1 F9 E0 ................
3380 02 E0 0A DF 16 DE 25 DD 38 DC 4B DB 62 DA 7C D9 ......%.8.K.b.|.
3390 9B D8 BB D7 DD D6 05 D6 2D D5 5C D4 8B D3 BF D2 ........-.\.....
33A0 F8 D1 32 D1 72 D0 B3 CF FB CE 45 CE 93 CD E5 CC ..2.r.....E.....
33B0 3B CC 95 CB F3 CA 55 CA BA C9 26 C9 95 C8 08 C8 ;.....U...&.....
33C0 7E C7 FA C6 7C C6 01 C6 8A C5 18 C5 AB C4 42 C4 ~...|.........B.
33D0 DE C3 7D C3 22 C3 CC C2 7B C2 30 C2 E7 C1 A4 C1 ..}."...{.0.....
33E0 68 C1 2F C1 F9 C0 CA C0 A1 C0 7C C0 5A C0 40 C0 h./.......|.Z.@.
33F0 28 C0 18 C0 0A C0 04 C0 00 C0 04 C0 0A C0 18 C0 (...............
3400 28 C0 40 C0 5A C0 7C C0 A1 C0 CA C0 F9 C0 2F C1 (.@.Z.|......./.
3410 68 C1 A4 C1 E7 C1 30 C2 7B C2 CC C2 22 C3 7D C3 h.....0.{...".}.
3420 DE C3 42 C4 AB C4 18 C5 8A C5 01 C6 7C C6 FA C6 ..B.........|...
3430 7E C7 08 C8 95 C8 26 C9 BA C9 55 CA F3 CA 95 CB ~.....&...U.....
3440 3B CC E5 CC 93 CD 45 CE FB CE B3 CF 72 D0 33 D1 ;.....E.....r.3.
3450 F8 D1 BF D2 8B D3 5C D4 2D D5 05 D6 DD D6 BB D7 ......\.-.......
3460 9B D8 7C D9 62 DA 4B DB 38 DC 25 DD 16 DE 0A DF ..|.b.K.8.%.....
3470 00 E0 F9 E0 F6 E1 F2 E2 F2 E3 F4 E4 FA E5 00 E7 ................
3480 08 E8 12 E9 1E EA 2A EB 3B EC 4A ED 5C EE 70 EF ......*.;.J.\.p.
3490 86 F0 9C F1 B2 F2 CA F3 E3 F4 FD F5 18 F7 35 F8 ..............5.
34A0 51 F9 6E FA 8B FB A7 FC C6 FD E4 FE E0 34 34 35 Q.n..........445
34B0 50 98 80 F0 00 00 00 00 00 F7 00 00 00 F7 00 00 P...............
34C0 00 F7 00 00 00 F7 00 00 00 F7 00 00 00 F7 00 00 ................
34D0 00 F7 00 00 00 F7 00 00 00 F7 00 00 00 F7 00 00 ................

;; fn34E0: 34E0
fn34E0 proc
	reset
	reset
	reset
	reset
	reset
	mov	#3FFE,sp
	clr	@#FFFE
	mov	#3546,@#0004
	clr	@#0006
	tst	@#FB8C
	clr	@#FB8E
	jsr	pc,@#355A
	mov	#0083,@#FB8E
	jsr	pc,@#355A
	mov	#0083,@#FB8E
	jsr	pc,@#355A
	mov	#004B,@#FB8E
	jsr	pc,@#355A
	mov	#002F,@#FB8E
	jsr	pc,@#355A
	mov	#0046,@#FB8E
	jsr	pc,@#355A
	mov	#000D,@#FB8E
	jsr	pc,@#355A
	br	3548
3546                   96 25                               .%       

l3548:
	mov	#0040,@#FF66
	mov	#3562,@#F400
	jsr	pc,@#13AA
	illegal

;; fn355A: 355A
;;   Called from:
;;     3504 (in fn34E0)
;;     350E (in fn34E0)
;;     3518 (in fn34E0)
;;     3522 (in fn34E0)
;;     352C (in fn34E0)
;;     3536 (in fn34E0)
;;     3540 (in fn34E0)
fn355A proc
	tstb	@#FB8C
	bpl	355A

l3560:
	rts	pc
3562       D0 9D 00 00 8A 02 A0 F0 00 80 20 20 20 20   ..........    
3570 20 20 20 20 20 20 20 20 20 20 20 20 47 20 20 52             G  R
3580 20 20 45 20 20 45 20 20 54 20 20 49 20 20 4E 20   E  E  T  I  N 
3590 20 47 20 20 53 0D 0A 20 20 20 20 20 20 20 20 20  G  S..         
35A0 20 20 20 20 20 20 20 2D 20 20 2D 20 20 2D 20 20        -  -  -  
35B0 2D 20 20 2D 20 20 2D 20 20 2D 20 20 2D 20 20 2D -  -  -  -  -  -
35C0 0D 0A 0A 0A 20 20 20 20 20 42 45 43 41 55 53 45 ....     BECAUSE
35D0 20 4F 46 20 43 48 41 4E 47 45 53 20 49 4E 20 54  OF CHANGES IN T
35E0 48 45 20 4E 41 54 49 4F 4E 41 4C 20 50 4F 4C 49 HE NATIONAL POLI
35F0 43 59 2C 20 54 48 45 20 4C 55 4E 41 52 0D 0A 45 CY, THE LUNAR..E
3600 58 50 4C 4F 52 41 54 4F 52 59 20 50 52 4F 47 52 XPLORATORY PROGR
3610 41 4D 20 49 53 20 4F 4E 43 45 20 41 47 41 49 4E AM IS ONCE AGAIN
3620 20 46 55 4E 43 54 49 4F 4E 49 4E 47 2C 20 41 4E  FUNCTIONING, AN
3630 44 20 59 4F 55 20 48 41 56 45 0D 0A 42 45 45 4E D YOU HAVE..BEEN
3640 20 43 48 4F 53 45 4E 20 41 53 20 54 48 45 20 50  CHOSEN AS THE P
3650 49 4C 4F 54 20 49 4E 20 43 4F 4D 4D 41 4E 44 20 ILOT IN COMMAND 
3660 4F 46 20 54 48 45 20 4C 55 4E 41 52 20 4D 4F 44 OF THE LUNAR MOD
3670 55 4C 45 2E 0D 0A 20 20 20 20 20 59 4F 55 20 4D ULE...     YOU M
3680 49 53 53 49 4F 4E 20 49 53 20 54 4F 20 4C 41 4E ISSION IS TO LAN
3690 44 20 4F 4E 20 00 18 80 B0 F0 4D 61 72 65 20 41 D ON .....Mare A
36A0 73 73 61 62 65 74 2C 00 10 80 A0 F0 20 41 20 42 ssabet,..... A B
36B0 41 52 52 45 4E 20 41 4E 44 0D 0A 48 41 52 53 48 ARREN AND..HARSH
36C0 20 57 41 53 54 45 4C 41 4E 44 20 42 45 4C 49 45  WASTELAND BELIE
36D0 56 45 44 20 49 4E 43 41 50 41 42 4C 45 20 4F 46 VED INCAPABLE OF
36E0 20 53 55 50 50 4F 52 54 49 4E 47 20 41 4E 59 20  SUPPORTING ANY 
36F0 4B 4E 4F 57 4E 20 46 4F 52 4D 20 4F 46 20 4C 49 KNOWN FORM OF LI
3700 46 45 2E 0D 0A 0D 0A 20 20 20 20 20 55 4E 46 4F FE.....     UNFO
3710 52 54 55 4E 41 54 45 4C 59 2C 20 54 48 45 20 53 RTUNATELY, THE S
3720 50 41 43 45 20 41 47 45 4E 43 59 20 43 48 4F 53 PACE AGENCY CHOS
3730 45 20 54 4F 20 55 53 45 20 41 4E 20 49 4E 46 45 E TO USE AN INFE
3740 52 49 4F 52 0D 0A 42 52 41 4E 44 20 4F 46 20 4D RIOR..BRAND OF M
3750 49 4E 49 43 4F 4D 50 55 54 45 52 20 49 4E 20 54 INICOMPUTER IN T
3760 48 45 20 4C 55 4E 41 52 20 4C 41 4E 44 45 52 2C HE LUNAR LANDER,
3770 20 41 4E 44 20 49 54 20 48 41 53 0D 0A 4A 55 53  AND IT HAS..JUS
3780 54 20 44 49 45 44 20 4F 4E 20 59 4F 55 2E 20 59 T DIED ON YOU. Y
3790 4F 55 20 4D 55 53 54 20 4E 4F 57 20 4C 41 4E 44 OU MUST NOW LAND
37A0 20 55 4E 44 45 52 20 4D 41 4E 55 41 4C 20 43 4F  UNDER MANUAL CO
37B0 4E 54 52 4F 4C 2E 0D 0A 0D 0A 20 20 20 20 20 20 NTROL.....      
37C0 20 20 20 20 2A 2A 2A 2A 2A 20 20 47 4F 4F 44 20     *****  GOOD 
37D0 4C 55 43 4B 20 20 2A 2A 2A 2A 2A 00 00 E0 62 35 LUCK  *****...b5
