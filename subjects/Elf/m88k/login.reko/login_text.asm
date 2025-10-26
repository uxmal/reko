;;; Segment .text (00002570)

;; fn00002570: 00002570
fn00002570 proc
	or	r0,r0,r0
	or	r0,r0,r0
	ld	r2,r31,0x0
	addu	r3,r31,4
	lda	r4,r3[r2]
	br.n	00002590
	addu	r4,r4,4
0000258C                                     F4 00 58 00             ..X.

l00002590:
	subu	r31,r31,0x30
	or.u	r13,r0,6
	st.d	r22,r31,0x10
	st	r30,r31,0x20
	st	r21,r31,0xC
	st.d	r24,r31,0x18
	st	r1,r31,0x24
	st	r4,r13,0x6D84
	ld	r25,r0,r3
	addu	r30,r31,0x20
	or	r24,r0,r4
	or	r22,r0,r3
	or	r21,r0,r2
	bcnd.n	eq0,r25,00002650
	or	r23,r0,r5

l000025CC:
	or	r2,r0,r25
	bsr.n	fn000026A0
	or	r3,r0,0x2F
	or.u	r7,r0,2
	bcnd.n	ne0,r2,0000268C
	st	r2,r7,0x6004

l000025E4:
	st	r25,r7,0x6004

l000025E8:
	or.u	r7,r0,2
	ld	r12,r7,0x6004
	or.u	r5,r0,6
	or	r9,r5,0x6D88
	ld.b	r13,r0,r12
	bcnd.n	eq0,r13,00002644
	or	r8,r0,r7

l00002604:
	addu	r13,r9,0xFF
	cmp	r13,r9,r13
	bb0.n	0,r13,00002644
	or.u	r6,r0,6

l00002614:
	ld	r13,r8,0x6004
	or	r10,r6,0x6E87
	or	r7,r0,r8
	ld.bu	r12,r0,r13
	addu	r13,r13,1
	st	r13,r8,0x6004
	st.b	r12,r0,r9
	ld.b	r11,r0,r13
	addu	r9,r9,1
	bcnd.n	eq0,r11,00002644
	cmp	r13,r9,r10

l00002640:
	bb1	0x1F,r13,00002614

l00002644:
	or	r13,r5,0x6D88
	st.b	r0,r0,r9
	st	r13,r7,0x6004

l00002650:
	bcnd.n	eq0,r23,00002678
	or.u	r13,r0,0

l00002658:
	bsr.n	fn00002710
	or	r2,r0,r23

l00002660:
	bsr	fn00002558
	or	r3,r0,r22
	or	r4,r0,r24
	bsr.n	fn000028D0
	or	r2,r0,r21
	bsr	exit

l00002678:
	or	r13,r13,0
	bcnd.n	eq0,r13,00002660
	or	r2,r0,r24

l00002684:
	bsr.n	00000000
	subu	r1,r1,0x2C

l0000268C:
	addu	r13,r2,1
	br.n	000025E8
	st	r13,r7,0x6004
00002698                         F4 00 58 00 F4 00 58 00         ..X...X.

;; fn000026A0: 000026A0
;;   Called from:
;;     000025D0 (in fn00002570)
fn000026A0 proc
	subu	r31,r31,0x10
	st	r30,r0,r31
	st	r1,r31,0x4
	addu	r30,r31,0
	ext	r3,r3,3,0
	or	r7,r0,0
	ld.b	r8,r0,r2
	cmp	r9,r8,r3
	bb0	0,r9,000026D4
	bcnd.n	ne0,r8,000026B8
	addu	r2,r2,1
	br.n	000026DC
	addu	r31,r30,0
	br.n	000026C4
	or	r7,r0,r2
	or	r2,r0,r7
	ld	r30,r0,r31
	addu	r31,r31,0x10
	jmp	r1
	or	r0,r0,r0

;; fn000026F0: 000026F0
;;   Called from:
;;     00002840 (in fn00002810)
fn000026F0 proc
	subu	r31,r31,0x10
	st	r30,r0,r31
	addu	r30,r31,0
	st	r1,r31,0x4
	addu	r31,r30,0
	ld	r30,r0,r31
	addu	r31,r31,0x10
	jmp	r1

;; fn00002710: 00002710
;;   Called from:
;;     00002658 (in fn00002570)
;;     00002870 (in fn00002810)
fn00002710 proc
	subu	r31,r31,0x10
	st	r30,r0,r31
	or	r3,r0,0
	addu	r30,r31,0
	st	r1,r31,0x4
	bsr.n	__cxa_atexit
	or	r4,r0,0
	addu	r31,r30,0
	ld	r1,r31,0x4
	ld	r30,r0,r31
	addu	r31,r31,0x10
	jmp	r1

;; fn00002740: 00002740
;;   Called from:
;;     00002868 (in fn00002810)
fn00002740 proc
	or.u	r11,r0,5
	or	r12,r11,0x6BF8
	ld	r13,r12,0x4
	subu	r31,r31,0x20
	st	r30,r31,0x10
	st.d	r24,r31,0x8
	st	r1,r31,0x14
	addu	r30,r31,0x10
	bcnd.n	eq0,r13,00002774
	or	r25,r0,1

l00002768:
	addu	r25,r25,1
	ld	r13,r12[r25]
	bcnd	ne0,r13,00002768

l00002774:
	or	r13,r11,0x6BF8
	subu	r25,r25,1
	lda	r24,r13[r25]
	subu	r25,r25,1
	addu	r13,r25,1
	bcnd	eq0,r13,000027A4

l0000278C:
	ld	r12,r0,r24
	subu	r25,r25,1
	jsr.n	r12
	subu	r24,r24,4
	addu	r13,r25,1
	bcnd	ne0,r13,0000278C

l000027A4:
	subu	r31,r30,0x10
	ld	r1,r31,0x14
	ld	r30,r31,0x10
	ld.d	r24,r31,0x8
	addu	r31,r31,0x20
	jmp	r1
000027BC                                     F4 00 58 00             ..X.
000027C0 67 FF 00 20 27 DF 00 10 27 3F 00 0C 5D A0 00 05 g.. '...'?..]...
000027D0 24 3F 00 14 15 8D 6C 04 63 DF 00 10 EC 4C 00 07 $?....l.c....L..
000027E0 5B 2D 6C 04 F4 00 CC 0C 63 39 00 04 F5 A0 14 19 [-l.....c9......
000027F0 ED AD FF FD F5 80 58 0D 67 FE 00 10 14 3F 00 14 ......X.g....?..
00002800 17 DF 00 10 17 3F 00 0C 63 FF 00 20 F4 00 C0 01 .....?..c.. ....

;; fn00002810: 00002810
;;   Called from:
;;     00002560 (in fn00002558)
fn00002810 proc
	subu	r31,r31,0x10
	st	r30,r0,r31
	st	r1,r31,0x4
	or.u	r12,r0,2
	ld	r13,r12,0x600C
	or.u	r2,r0,2
	or.u	r3,r0,6
	addu	r30,r31,0
	or	r2,r2,0x602C
	bcnd.n	ne0,r13,00002878
	or	r3,r3,0x6D1C

l0000283C:
	or	r13,r0,1
	bsr.n	fn000026F0
	st	r13,r12,0x600C
	or.u	r13,r0,2
	ld	r12,r13,0x60E8
	bcnd.n	eq0,r12,00002868
	or	r2,r13,0x60E8

l00002858:
	or.u	r13,r0,0
	or	r13,r13,0
	bcnd	eq0,r13,00002868

l00002864:
	bsr	00000000

l00002868:
	bsr	fn00002740
	or.u	r2,r0,0
	bsr.n	fn00002710
	or	r2,r2,0x4BD0

l00002878:
	addu	r31,r30,0
	ld	r1,r31,0x4
	ld	r30,r0,r31
	addu	r31,r31,0x10
	jmp	r1
0000288C                                     F4 00 58 00             ..X.
00002890 5D 80 00 02 15 AC 60 10 67 FF 00 10 F7 C0 24 1F ].....`.g.....$.
000028A0 24 3F 00 04 ED AD 00 05 63 DF 00 00 59 A0 00 01 $?......c...Y...
000028B0 CF FF FF C4 25 AC 60 10 63 FE 00 00 14 3F 00 04 ....%.`.c....?..
000028C0 F7 C0 14 1F 63 FF 00 10 F4 00 C0 01 F4 00 58 00 ....c.........X.

;; fn000028D0: 000028D0
;;   Called from:
;;     0000266C (in fn00002570)
fn000028D0 proc
	subu	r31,r31,0x8B0
	st	r1,r31,0x34
	st	r30,r31,0x30
	st.d	r24,r31,0x28
	st.d	r22,r31,0x20
	st.d	r20,r31,0x18
	st.d	r18,r31,0x10
	st.d	r16,r31,0x8
	st.d	r14,r0,r31
	or.u	r13,r0,2
	ld	r12,r13,0x6000
	or.u	r11,r0,1
	addu	r30,r31,0x30
	or	r13,r11,0x5384
	or	r4,r0,0x20
	or	r24,r0,r2
	or	r22,r0,r3
	or	r2,r0,r13
	or	r3,r0,4
	addu	r17,r30,0x338
	st	r12,r30,0x870
	bsr.n	openlog
	or	r18,r0,0
	or	r2,r0,r17
	bsr.n	gethostname
	or	r3,r0,0x100
	or	r12,r0,0xA
	st	r0,r30,0x14
	st	r0,r30,0x10
	st	r0,r30,0xC
	st	r0,r30,0x18
	st	r0,r30,0x28
	st	r12,r30,0x2C
	or	r15,r0,3
	bcnd.n	lt0,r2,00003F64
	or	r21,r0,0

l00002960:
	or	r2,r0,r17
	bsr.n	strchr
	or	r3,r0,0x2E
	bcnd.n	eq0,r2,00002984
	or	r21,r0,r2

l00002974:
	addu	r21,r2,1
	ld.b	r13,r0,r21
	bcnd.n	ne0,r13,00003F50
	or	r2,r0,r21

l00002984:
	bsr.n	auth_open
	or.u	r20,r0,2
	bcnd.n	eq0,r2,00003F34
	st	r2,r20,0x601C

l00002994:
	or.u	r13,r0,1
	or.u	r11,r0,1
	or	r3,r13,0x5384
	bsr.n	auth_setoption
	or	r4,r11,0x4F98
	bsr.n	getuid
	or.u	r19,r0,1
	or	r16,r0,r2
	st	r0,r30,0x20
	st	r0,r30,0x1C

l000029BC:
	or	r2,r0,r24
	or	r3,r0,r22
	bsr.n	getopt
	or	r4,r19,0x4F9C
	addu	r13,r2,1
	bcnd.n	eq0,r13,00002CF4
	subu	r12,r2,0x4C

l000029D8:
	cmp	r13,r12,0x29
	bb0	5,r13,00002C7C

l000029E0:
	bsr.n	fn00002A90
	lda	r1,r1[r12]
	br	00002AA0
000029EC                                     C0 00 00 A4             ....
000029F0 C0 00 00 A3 C0 00 00 A2 C0 00 00 A1 C0 00 00 A0 ................
00002A00 C0 00 00 5A C0 00 00 9E C0 00 00 9D C0 00 00 9C ...Z............
00002A10 C0 00 00 9B C0 00 00 9A C0 00 00 99 C0 00 00 98 ................
00002A20 C0 00 00 97 C0 00 00 96 C0 00 00 95 C0 00 00 94 ................
00002A30 C0 00 00 93 C0 00 00 92 C0 00 00 91 C0 00 00 90 ................
00002A40 C0 00 00 8F C0 00 00 8E C0 00 00 8D C0 00 00 8C ................
00002A50 C0 00 00 11 C0 00 00 8A C0 00 00 5B C0 00 00 88 ...........[....
00002A60 C0 00 00 87 C0 00 00 86 C0 00 00 85 C0 00 00 84 ................
00002A70 C0 00 00 83 C0 00 00 82 C0 00 00 7E C0 00 00 80 ...........~....
00002A80 C0 00 00 7F C0 00 00 7E C0 00 00 7D C0 00 00 8D .......~...}....

;; fn00002A90: 00002A90
;;   Called from:
;;     000029E0 (in fn000028D0)
fn00002A90 proc
	jmp	r1
00002A94             59 80 00 01 C7 FF FF C9 25 9E 00 1C     Y.......%...

l00002AA0:
	bcnd.n	ne0,r16,00002B50
	or.u	r3,r0,1

l00002AA8:
	ld	r12,r30,0xC
	bcnd.n	eq0,r12,00002AC4
	or.u	r2,r0,1

l00002AB4:
	bsr.n	warnx
	or	r2,r2,0x4FB4
	bsr.n	fn00004780
	or	r2,r0,1

l00002AC4:
	or.u	r13,r0,6
	ld	r13,r13,0x6C08
	or	r10,r0,0
	or	r11,r0,0
	or	r3,r0,0
	st	r13,r30,0xC
	or	r2,r0,r13
	addu	r4,r30,0x38
	or	r13,r0,2
	addu	r5,r30,0x30
	st.d	r10,r30,0x38
	st.d	r10,r30,0x50
	st	r13,r30,0x38
	st.d	r10,r30,0x40
	st.d	r10,r30,0x48
	bsr.n	getaddrinfo
	st	r0,r30,0x3C
	bcnd.n	ne0,r2,00002B40
	or	r2,r0,r17

l00002B10:
	ld	r13,r30,0x30
	or	r4,r0,0x100
	bsr.n	strlcpy
	ld	r3,r13,0x18
	bsr.n	freeaddrinfo
	ld	r2,r30,0x30
	ld	r2,r20,0x601C
	ld	r4,r30,0xC
	or.u	r3,r0,1
	or	r3,r3,0x4FC8

l00002B38:
	bsr.n	auth_setoption
	subu	r1,r1,0x184

l00002B40:
	ld	r3,r30,0xC
	or	r4,r0,0x100
	bsr.n	strlcpy
	subu	r1,r1,0x28

l00002B50:
	or	r2,r0,1
	bsr.n	warnc
	or	r3,r3,0x4FA8
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0xC0
	bcnd.n	ne0,r16,00002BAC
	or.u	r3,r0,1

l00002B70:
	ld	r11,r30,0x10
	bcnd	eq0,r11,00002B8C

l00002B78:
	or.u	r2,r0,1
	bsr.n	warnx
	or	r2,r2,0x4FE0
	bsr.n	fn00004780
	or	r2,r0,1

l00002B8C:
	or.u	r13,r0,6
	ld	r13,r13,0x6C08
	ld	r2,r20,0x601C
	or.u	r3,r0,1
	st	r13,r30,0x10
	or	r3,r3,0x4FF4
	br.n	00002B38
	or	r4,r0,r13

l00002BAC:
	or	r2,r0,1
	bsr.n	warnc
	or	r3,r3,0x4FD4
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x54
	bcnd.n	ne0,r16,00002C58
	or.u	r3,r0,1

l00002BCC:
	ld	r2,r30,0x18
	bsr.n	free
	or.u	r23,r0,6
	bsr.n	strdup
	ld	r2,r23,0x6C08
	bcnd.n	eq0,r2,00002C48
	st	r2,r30,0x18

l00002BE8:
	ld	r2,r20,0x601C
	ld	r4,r30,0x18
	or.u	r3,r0,1
	bsr.n	auth_setoption
	or	r3,r3,0x500C
	bcnd	eq0,r21,00002C28

l00002C00:
	ld	r2,r23,0x6C08
	bsr.n	strchr
	or	r3,r0,0x2E
	bcnd.n	eq0,r2,00002C28
	or	r25,r0,r2

l00002C14:
	addu	r2,r2,1
	bsr.n	strcasecmp
	or	r3,r0,r21
	bcnd	ne0,r2,00002C28

l00002C24:
	st.b	r0,r0,r25

l00002C28:
	ld	r13,r23,0x6C08
	ld	r2,r20,0x601C
	or.u	r3,r0,1
	or.u	r11,r0,6
	or	r3,r3,0x5014
	or	r4,r0,r13
	br.n	00002B38
	st	r13,r11,0x6E88

l00002C48:
	bsr	warn
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x70

l00002C58:
	or	r2,r0,1
	bsr.n	warnc
	or	r3,r3,0x5000
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0xA4
	or	r13,r0,1
	br.n	000029BC
	st	r13,r30,0x20

l00002C7C:
	bcnd.n	eq0,r16,00002CAC
	or.u	r3,r0,1

l00002C84:
	or.u	r2,r0,1
	or.u	r5,r0,6
	or	r2,r2,0x5030
	or	r5,r5,0x6CC0
	or	r3,r0,1
	bsr.n	fwrite
	or	r4,r0,0x58
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x2F0

l00002CAC:
	or	r4,r0,r2
	or	r3,r3,0x5020
	or	r2,r0,3
	bsr.n	syslog
	subu	r1,r1,0x3C
	bcnd.n	ne0,r16,00002CDC
	or.u	r3,r0,1

l00002CC8:
	or.u	r13,r0,6
	ld	r11,r13,0x6C08
	or.u	r12,r0,2
	br.n	000029BC
	st	r11,r12,0x6028

l00002CDC:
	or	r2,r0,1
	bsr.n	warnc
	or	r3,r3,0x508C
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x2C

l00002CF4:
	or.u	r13,r0,6
	ld	r12,r13,0x6C0C
	or	r19,r0,1
	ld	r3,r22[r12]
	bcnd.n	eq0,r3,00002D18
	subu	r24,r24,r12

l00002D0C:
	or.u	r12,r0,2
	st	r3,r12,0x6024
	or	r19,r0,0

l00002D18:
	bsr	geteuid
	bcnd	ne0,r2,00003EAC

l00002D20:
	bsr	ttyname
	bcnd.n	eq0,r2,00002D34
	st	r2,r30,0x8

l00002D2C:
	ld.b	r13,r0,r2
	bcnd	ne0,r13,00002D58

l00002D34:
	addu	r25,r30,0x438
	or.u	r4,r0,1
	or.u	r5,r0,1
	or	r4,r4,0x5098
	or	r5,r5,0x50A0
	or	r2,r0,r25
	bsr.n	snprintf
	or	r3,r0,0x13
	st	r25,r30,0x8

l00002D58:
	ld	r2,r30,0x8
	bsr.n	strrchr
	or	r3,r0,0x2F
	or.u	r11,r0,6
	bcnd.n	eq0,r2,00003E9C
	st	r2,r11,0x6ED8

l00002D70:
	addu	r13,r2,1
	st	r13,r11,0x6ED8

l00002D78:
	or	r2,r0,4
	bsr.n	getrlimit
	addu	r3,r30,0x68
	bcnd.n	lt0,r2,00003E7C
	or.u	r3,r0,1

l00002D8C:
	or	r12,r0,0
	or	r13,r0,0
	addu	r3,r30,0x58
	or	r2,r0,4
	st.d	r12,r30,0x58
	bsr.n	setrlimit
	st.d	r12,r30,0x60
	bcnd.n	lt0,r2,00003E5C
	or.u	r3,r0,1

l00002DB0:
	or.u	r3,r0,0
	or	r3,r3,0x4260
	bsr.n	signal
	or	r2,r0,0xE
	cmp	r13,r24,1
	bb1.n	1,r13,00003E48
	or	r14,r0,1

l00002DCC:
	or	r3,r0,1
	bsr.n	signal
	or	r2,r0,3
	or	r3,r0,1
	bsr.n	signal
	or	r2,r0,2
	or	r3,r0,1
	bsr.n	signal
	or	r2,r0,1
	or	r3,r0,0
	or	r4,r0,0
	bsr.n	setpriority
	or	r2,r0,0
	bsr.n	login_getclass
	or	r2,r0,0
	or.u	r17,r0,2
	bcnd.n	eq0,r2,00003E30
	st	r2,r17,0x6018

l00002E14:
	ld	r2,r17,0x6018
	or.u	r3,r0,1
	or	r4,r0,0
	or	r5,r0,0x12C
	or	r6,r0,0
	or	r7,r0,0x12C
	bsr.n	login_getcapnum
	or	r3,r3,0x50F4
	or	r12,r0,r2
	or	r13,r0,r3
	ld	r2,r17,0x6018
	or.u	r3,r0,1
	or.u	r11,r0,2
	or	r3,r3,0x5104
	or	r4,r0,0
	or	r5,r0,0
	bsr.n	login_getcapstr
	st	r13,r11,0x6014
	bcnd.n	eq0,r2,00002FF4
	or	r24,r0,r2

l00002E64:
	or.u	r23,r0,1
	or	r2,r23,0x5110
	bsr.n	unsetenv
	or.u	r21,r0,1
	bsr.n	unsetenv
	or	r2,r21,0x511C
	ld.b	r13,r0,r24
	cmp	r13,r13,0x2F
	bb0.n	0,r13,00002EB0
	or.u	r3,r0,1

l00002E8C:
	or	r3,r3,0x5128
	or	r2,r0,3
	bsr.n	syslog
	or	r4,r0,r24
	or.u	r2,r0,1
	bsr.n	warnx
	or	r2,r2,0x5144
	bsr.n	fn00004780
	or	r2,r0,1

l00002EB0:
	or	r3,r0,0x2F
	bsr.n	strrchr
	or	r2,r0,r24
	ld	r13,r20,0x601C
	or	r3,r0,1
	addu	r22,r2,1
	bsr.n	auth_setstate
	or	r2,r0,r13
	ld	r12,r30,0x1C
	ld	r2,r20,0x601C
	bcnd.n	ne0,r12,00003E28
	or.u	r13,r0,1

l00002EE0:
	or.u	r13,r0,2
	ld	r5,r13,0x6024

l00002EE8:
	ld	r11,r30,0x1C
	bcnd.n	eq0,r11,00002EFC
	or	r6,r0,0

l00002EF4:
	or.u	r12,r0,2
	ld	r6,r12,0x6024

l00002EFC:
	or	r3,r0,r24
	or	r4,r0,r22
	bsr.n	auth_call
	or	r7,r0,0
	bsr.n	auth_getstate
	ld	r2,r20,0x601C
	mask	r2,r2,7
	bcnd.n	eq0,r2,00003E20
	or	r2,r0,1

l00002F20:
	or.u	r13,r0,2
	bsr.n	auth_setenv
	ld	r2,r13,0x601C
	bsr.n	getenv
	or	r2,r23,0x5110
	bcnd.n	eq0,r2,00002F54
	or	r25,r0,r2

l00002F3C:
	or.u	r3,r0,1
	or	r3,r3,0x5160
	bsr.n	strncmp
	or	r4,r0,5
	bcnd	ne0,r2,00002F54

l00002F50:
	st	r25,r30,0x14

l00002F54:
	bsr.n	getenv
	or	r2,r21,0x511C
	bcnd.n	eq0,r2,00002F68
	or.u	r13,r0,6

l00002F64:
	st	r2,r13,0x6E88

l00002F68:
	ld	r2,r20,0x601C
	bsr.n	auth_clroptions
	or.u	r24,r0,2
	ld	r11,r30,0x14
	bcnd.n	eq0,r11,00002F90
	or.u	r3,r0,1

l00002F80:
	ld	r2,r20,0x601C
	or	r3,r3,0x5168
	bsr.n	auth_setoption
	or	r4,r0,r11

l00002F90:
	ld	r12,r30,0x18
	bcnd.n	eq0,r12,00002FAC
	or.u	r3,r0,1

l00002F9C:
	ld	r2,r24,0x601C
	or	r3,r3,0x500C
	bsr.n	auth_setoption
	or	r4,r0,r12

l00002FAC:
	or.u	r13,r0,6
	ld	r4,r13,0x6E88
	bcnd.n	ne0,r4,00003E10
	or.u	r3,r0,1

l00002FBC:
	ld	r11,r30,0xC
	bcnd.n	eq0,r11,00002FD8
	or.u	r3,r0,1

l00002FC8:
	ld	r2,r24,0x601C
	or	r3,r3,0x4FC8
	bsr.n	auth_setoption
	or	r4,r0,r11

l00002FD8:
	ld	r12,r30,0x10
	bcnd.n	eq0,r12,00002FF4
	or.u	r3,r0,1

l00002FE4:
	ld	r2,r24,0x601C
	or	r3,r3,0x4FF4
	bsr.n	auth_setoption
	or	r4,r0,r12

l00002FF4:
	ld	r2,r20,0x601C
	or.u	r4,r0,1
	or	r4,r4,0x5174
	bsr.n	auth_setitem
	or	r3,r0,6
	or	r22,r0,0
	or.u	r21,r0,6
	or.u	r23,r0,6

l00003014:
	ld	r2,r20,0x601C
	bsr.n	auth_clean
	st	r0,r30,0x24
	ld	r2,r20,0x601C
	or.u	r13,r0,1
	bsr.n	auth_clroption
	or	r3,r13,0x517C
	ld	r2,r20,0x601C
	or.u	r11,r0,1
	bsr.n	auth_clroption
	or	r3,r11,0x5184
	bcnd	ne0,r19,00003E04

l00003044:
	bcnd.n	ne0,r14,00003DF4
	or.u	r12,r0,2

l0000304C:
	or.u	r13,r0,2
	ld	r2,r13,0x6024
	or	r3,r0,0x3A
	bsr.n	strchr
	or	r25,r0,r13
	bcnd.n	eq0,r2,00003070
	or	r24,r0,r2

l00003068:
	st.b	r0,r0,r2
	addu	r24,r2,1

l00003070:
	bcnd.n	eq0,r18,00003084
	or.u	r13,r0,2

l00003078:
	bsr.n	free
	or	r2,r0,r18
	or.u	r13,r0,2

l00003084:
	ld	r2,r13,0x601C
	ld	r4,r25,0x6024
	bsr.n	auth_setitem
	or	r3,r0,3
	bcnd.n	lt0,r2,00003DD4
	or.u	r3,r0,1

l0000309C:
	bsr.n	strdup
	ld	r2,r25,0x6024
	bcnd.n	eq0,r2,00003DD0
	or	r18,r0,r2

l000030AC:
	or.u	r11,r0,2
	ld	r2,r11,0x6024
	or	r3,r0,0x2F
	bsr.n	strchr
	or	r19,r0,0
	bcnd.n	eq0,r2,000030E8
	or	r25,r0,r2

l000030C8:
	or.u	r3,r0,1
	addu	r2,r2,1
	or	r3,r3,0x5194
	bsr.n	strncmp
	or	r4,r0,4
	bcnd	ne0,r2,000030E4

l000030E0:
	or	r19,r0,1

l000030E4:
	st.b	r0,r0,r25

l000030E8:
	or.u	r12,r0,2
	bsr.n	strlen
	ld	r2,r12,0x6024
	cmp	r2,r2,0x20
	bb1.n	0,r2,00003108
	or.u	r11,r0,2

l00003100:
	ld	r13,r11,0x6024
	st.b	r0,r13,0x20

l00003108:
	ld	r13,r21,0x6E8C
	bcnd.n	ne0,r13,00003D8C
	or.u	r13,r0,2

l00003114:
	or.u	r12,r0,2

l00003118:
	ld	r3,r12,0x6024
	or	r4,r0,0x402
	bsr.n	strlcpy
	addu	r2,r30,0x450
	or.u	r13,r0,2
	bsr.n	getpwnam
	ld	r2,r13,0x6024
	or	r3,r0,r2
	bcnd.n	ne0,r2,00003D5C
	st	r2,r23,0x6ED0

l00003140:
	ld	r13,r23,0x6ED0
	bcnd.n	eq0,r13,00003150
	or	r2,r0,0

l0000314C:
	ld	r2,r13,0x18

l00003150:
	bsr	login_getclass
	bcnd.n	eq0,r2,00003214
	st	r2,r17,0x6018

l0000315C:
	ld	r4,r30,0x14
	bsr.n	login_getstyle
	or	r3,r0,r24
	bcnd.n	eq0,r2,00003214
	or	r24,r0,r2

l00003170:
	ld	r2,r17,0x6018
	or.u	r11,r0,1
	or	r4,r0,0
	or	r5,r0,0xA
	or	r6,r0,0
	or	r7,r0,0xA
	bsr.n	login_getcapnum
	or	r3,r11,0x519C
	or	r12,r0,r2
	or	r13,r0,r3
	ld	r2,r17,0x6018
	or.u	r11,r0,1
	or	r3,r11,0x51A8
	or	r4,r0,0
	or	r5,r0,3
	or	r6,r0,0
	or	r7,r0,3
	bsr.n	login_getcapnum
	st	r13,r30,0x2C
	ld	r13,r23,0x6ED0
	bcnd.n	eq0,r13,000031D8
	or	r15,r0,r3

l000031C8:
	bcnd	eq0,r16,000031E0

l000031CC:
	ld	r13,r13,0x8
	cmp	r13,r16,r13
	bb0	0,r13,000031DC

l000031D8:
	st	r0,r30,0x1C

l000031DC:
	ld	r13,r23,0x6ED0

l000031E0:
	bcnd	eq0,r13,000031F0

l000031E4:
	ld	r13,r13,0x8
	bcnd	ne0,r13,000031F0

l000031EC:
	or	r19,r0,1

l000031F0:
	ld	r12,r30,0x1C
	bcnd	eq0,r12,00003C68

l000031F8:
	ld	r13,r23,0x6ED0
	bcnd	eq0,r13,00003214

l00003200:
	bcnd.n	eq0,r19,00003394
	or.u	r11,r0,6

l00003208:
	bsr.n	fn000040D0
	ld	r2,r11,0x6ED8
	bcnd	ne0,r2,00003394

l00003214:
	ld	r12,r30,0x28
	bb1.n	2,r12,0000338C
	or	r2,r0,0

l00003220:
	bcnd.n	ne0,r19,000032EC
	or.u	r13,r0,6

l00003228:
	ld	r2,r20,0x601C
	bcnd.n	ne0,r2,000032D0
	or.u	r13,r0,1

l00003234:
	or	r25,r13,0x51B8

l00003238:
	bsr.n	puts
	or	r2,r0,r25
	ld	r13,r21,0x6E8C
	ld	r12,r23,0x6ED0
	addu	r13,r13,1
	bcnd.n	ne0,r12,000032AC
	st	r13,r21,0x6E8C

l00003254:
	addu	r22,r22,1
	cmp	r13,r22,r15
	bb1.n	0x1B,r13,00003014
	or	r19,r0,1

l00003264:
	ld	r12,r30,0x2C
	cmp	r13,r22,r12
	bb0.n	0,r13,00003298
	or.u	r13,r0,2

l00003274:
	ld	r11,r30,0x2C
	subu	r2,r22,r15
	mul	r2,r2,r11
	extu	r13,r2,0xD,0x1F
	addu	r2,r2,r13
	bsr.n	sleep
	ext	r2,r2,2,1
	br.n	00003014
	or	r19,r0,1

l00003298:
	bsr.n	fn00004520
	ld	r2,r13,0x6024
	or	r2,r0,1
	bsr.n	fn00004740
	subu	r1,r1,0x38

l000032AC:
	ld	r2,r12,0x8
	or.u	r13,r0,6
	or.u	r12,r0,2
	or.u	r11,r0,6
	ld	r3,r13,0x6E88
	ld	r4,r12,0x6028
	ld	r5,r11,0x6ED8
	bsr.n	fn000047F0
	subu	r1,r1,0x7C

l000032D0:
	or.u	r3,r0,1
	bsr.n	auth_getvalue
	or	r3,r3,0x51C8
	bcnd.n	ne0,r2,00003238
	or	r25,r0,r2

l000032E4:
	br.n	00003234
	or.u	r13,r0,1

l000032EC:
	bsr.n	fn000040D0
	ld	r2,r13,0x6ED8
	bcnd.n	ne0,r2,00003228
	or.u	r2,r0,1

l000032FC:
	or	r2,r2,0x51D4
	bsr.n	warnx
	or	r3,r0,r18
	or.u	r11,r0,6
	ld	r13,r11,0x6E88
	bcnd.n	eq0,r13,0000336C
	or.u	r13,r0,2

l00003318:
	ld	r5,r13,0x6028
	bcnd.n	ne0,r5,0000332C
	or	r12,r0,r5

l00003324:
	or.u	r13,r0,1
	or	r5,r13,0x5224

l0000332C:
	bcnd.n	eq0,r12,00003364
	or.u	r13,r0,1

l00003334:
	or.u	r13,r0,1
	or	r6,r13,0x4BE8

l0000333C:
	or.u	r12,r0,6
	or.u	r13,r0,6
	ld	r7,r12,0x6E88
	ld	r8,r13,0x6ED8
	or.u	r3,r0,1
	or	r3,r3,0x4BEC
	or	r2,r0,5
	or	r4,r0,r18
	bsr.n	syslog
	subu	r1,r1,0x124

l00003364:
	br.n	0000333C
	or	r6,r13,0x5224

l0000336C:
	or.u	r13,r0,6
	ld	r5,r13,0x6ED8
	or.u	r3,r0,1
	or	r3,r3,0x4C14
	or	r2,r0,5
	or	r4,r0,r18
	bsr.n	syslog
	subu	r1,r1,0x14C

l0000338C:
	bsr.n	fn00004780
	subu	r1,r1,0x174

l00003394:
	bsr.n	alarm
	or	r2,r0,0
	bsr	endpwent
	ld	r13,r23,0x6ED0
	ld	r2,r17,0x6018
	or.u	r3,r0,1
	ld	r4,r13,0x24
	or	r3,r3,0x4ED0
	bsr.n	login_getcapstr
	or	r5,r0,r4
	ld.b	r13,r0,r2
	bcnd.n	ne0,r13,00003C30
	or	r22,r0,r2

l000033C8:
	or.u	r13,r0,1
	or	r22,r13,0x4C30

l000033D0:
	ld	r12,r30,0x20
	bcnd.n	ne0,r12,00003B98
	or	r2,r0,1

l000033DC:
	bsr.n	calloc
	or	r3,r0,4
	or.u	r13,r0,6
	bcnd.n	eq0,r2,00003B88
	st	r2,r13,0x6D84

l000033F0:
	ld	r13,r23,0x6ED0
	or.u	r14,r0,1
	or	r2,r14,0x4C38
	ld	r3,r13,0x20
	bsr.n	setenv
	or	r4,r0,1
	addu	r2,r2,1
	bcnd.n	eq0,r2,0000343C
	or.u	r2,r0,1

l00003414:
	ld	r13,r23,0x6ED0
	or.u	r2,r0,1
	or	r2,r2,0x4C54
	ld	r3,r13,0x24
	bsr.n	setenv
	or	r4,r0,1
	addu	r2,r2,1
	bcnd.n	ne0,r2,00003450
	or.u	r13,r0,6

l00003438:
	or.u	r2,r0,1

l0000343C:
	bsr.n	warn
	or	r2,r2,0x4C40
	bsr.n	fn00004780
	or	r2,r0,1
	or.u	r13,r0,6

l00003450:
	ld.b	r12,r13,0x6E90
	bcnd.n	eq0,r12,00003B68
	or	r24,r13,0x6E90

l0000345C:
	ld	r13,r23,0x6ED0
	addu	r16,r30,0x228
	or.u	r4,r0,1
	ld	r6,r0,r13
	or.u	r5,r0,1
	or	r4,r4,0x4C5C
	or	r5,r5,0x4C64
	or	r3,r0,0x10A
	bsr.n	snprintf
	or	r2,r0,r16
	or.u	r2,r0,1
	or	r2,r2,0x4C70
	or	r3,r0,r24
	bsr.n	setenv
	or	r4,r0,0
	addu	r2,r2,1
	bcnd.n	eq0,r2,000034C8
	or.u	r2,r0,1

l000034A4:
	ld	r13,r23,0x6ED0
	or.u	r2,r0,1
	or	r2,r2,0x4C78
	ld	r3,r0,r13
	bsr.n	setenv
	or	r4,r0,1
	addu	r2,r2,1
	bcnd.n	ne0,r2,00003B1C
	or.u	r2,r0,1

l000034C8:
	bsr.n	warn
	or	r2,r2,0x4C40
	bsr.n	fn00004780
	or	r2,r0,1
	or.u	r11,r0,6

l000034DC:
	ld	r3,r11,0x6E88
	bcnd.n	ne0,r3,00003AEC
	or.u	r2,r0,1

l000034E8:
	or.u	r15,r0,2

l000034EC:
	ld	r3,r15,0x6028
	bcnd.n	ne0,r3,00003AC0
	or.u	r2,r0,1

l000034F8:
	ld	r3,r23,0x6ED0
	ld	r2,r17,0x6018
	or	r5,r0,4
	ld	r4,r3,0x8
	or.u	r21,r0,2
	bsr.n	setusercontext
	or.u	r24,r0,6
	bcnd.n	ne0,r2,00003AAC
	or.u	r2,r0,1

l0000351C:
	bsr.n	auth_setenv
	ld	r2,r20,0x601C
	bcnd	eq0,r19,00003AA0

l00003528:
	ld	r13,r24,0x6ED0
	bsr.n	setegid
	ld	r2,r13,0xC
	ld	r13,r24,0x6ED0
	bsr.n	seteuid
	ld	r2,r13,0x8
	ld	r13,r24,0x6ED0
	bsr.n	chdir
	ld	r2,r13,0x20
	bcnd.n	ne0,r2,00003A4C
	or	r18,r0,r2

l00003554:
	ld	r13,r23,0x6ED0
	or.u	r3,r0,1
	or	r3,r3,0x4CB4
	ld	r2,r13,0x24
	bsr.n	strcmp
	or	r21,r0,0
	bcnd	ne0,r2,00003A18

l00003570:
	or	r21,r0,1

l00003574:
	bsr.n	seteuid
	or	r2,r0,0
	bsr.n	setegid
	or	r2,r0,0
	ld	r2,r20,0x601C
	or.u	r3,r0,1
	bsr.n	auth_getvalue
	or	r3,r3,0x4CC4
	bcnd.n	eq0,r2,000035AC
	or	r25,r0,r2

l0000359C:
	or.u	r2,r0,1
	or	r2,r2,0x4CCC
	bsr.n	printf
	or	r3,r0,r25

l000035AC:
	or.u	r13,r0,2
	bsr.n	auth_check_expire
	ld	r2,r13,0x601C
	or	r24,r0,r2
	or	r25,r0,r3
	bcnd	lt0,r2,00003A00

l000035C4:
	bcnd	le0,r2,000039F0

l000035C8:
	bcnd.n	eq0,r21,00003998
	or.u	r3,r0,1

l000035D0:
	or	r3,r0,0

l000035D4:
	or	r2,r0,1
	bsr.n	signal
	addu	r24,r30,0xF8
	or	r4,r0,0x130
	or	r3,r0,0
	bsr.n	memset
	or	r2,r0,r24
	bsr.n	time
	addu	r2,r30,0x220
	or.u	r12,r0,2
	ld	r3,r12,0x6024
	addu	r2,r30,0x100
	bsr.n	strncpy
	or	r4,r0,0x20
	or.u	r13,r0,6
	ld	r3,r13,0x6E88
	bcnd.n	ne0,r3,0000398C
	or	r4,r0,0x100

l0000361C:
	or.u	r11,r0,6
	ld	r3,r11,0x6ED8
	or	r2,r0,r24
	bsr.n	strncpy
	or	r4,r0,8
	bsr.n	login
	or	r2,r0,r24
	bcnd	eq0,r21,0000397C

l0000363C:
	bsr.n	fn000042E0
	or	r2,r0,r21
	ld	r12,r23,0x6ED0
	or.u	r13,r0,6
	ld	r2,r13,0x6ED8
	ld	r4,r12,0xC
	bsr.n	login_fbtab
	ld	r3,r12,0x8
	or.u	r2,r0,1
	bsr.n	getgrnam
	or	r2,r2,0x4CE8
	bcnd	eq0,r2,0000396C

l0000366C:
	ld	r4,r2,0x8

l00003670:
	ld	r13,r23,0x6ED0
	ld	r2,r30,0x8
	bsr.n	chown
	ld	r3,r13,0x8
	bcnd	eq0,r19,000036E4

l00003684:
	ld	r12,r30,0x1C
	bcnd.n	ne0,r12,000036E4
	or.u	r11,r0,6

l00003690:
	ld	r13,r11,0x6E88
	bcnd.n	eq0,r13,0000394C
	or.u	r12,r0,2

l0000369C:
	ld	r6,r15,0x6028
	or.u	r13,r0,6
	ld	r4,r12,0x6024
	ld	r5,r13,0x6ED8
	bcnd.n	ne0,r6,000036BC
	or	r12,r0,r6

l000036B4:
	or.u	r13,r0,1
	or	r6,r13,0x5224

l000036BC:
	bcnd.n	eq0,r12,00003944
	or.u	r13,r0,1

l000036C4:
	or.u	r13,r0,1
	or	r7,r13,0x4BE8

l000036CC:
	or.u	r11,r0,6
	ld	r8,r11,0x6E88
	or.u	r3,r0,1
	or	r3,r3,0x4CEC
	bsr.n	syslog
	or	r2,r0,5

l000036E4:
	bcnd.n	eq0,r21,000038B4
	or	r4,r0,0

l000036EC:
	or	r3,r0,0
	bsr.n	signal
	or	r2,r0,0xE
	or	r3,r0,0
	bsr.n	signal
	or	r2,r0,3
	or	r3,r0,0
	bsr.n	signal
	or	r2,r0,1
	or	r3,r0,0
	bsr.n	signal
	or	r2,r0,2
	or	r3,r0,1
	bsr.n	signal
	or	r2,r0,0x12
	or	r3,r0,0x2F
	or	r13,r0,0x2D
	or	r2,r0,r22
	bsr.n	strrchr
	st.b	r13,r30,0x450
	bcnd.n	eq0,r2,000038AC
	addu	r3,r2,1

l00003744:
	addu	r2,r30,0x451
	bsr.n	strlcpy
	or	r4,r0,0x401
	ld	r13,r30,0x68
	bcnd.n	7,r13,00003774
	addu	r3,r30,0x68
	ld	r13,r30,0x6C
	bcnd	ne0,r13,00003774

l00003764:
	ld	r13,r30,0x70
	bcnd	7,r13,00003774
	ld	r13,r30,0x74
	bcnd	eq0,r13,00003784

l00003774:
	bsr.n	setrlimit
	or	r2,r0,4
	bcnd.n	lt0,r2,0000389C
	or.u	r3,r0,1

l00003784:
	ld	r11,r30,0x24
	bcnd.n	ne0,r11,00003890
	or.u	r2,r0,1

l00003790:
	ld	r2,r17,0x6018
	ld	r3,r23,0x6ED0
	bcnd.n	ne0,r19,000037A4
	or	r4,r0,0

l000037A0:
	ld	r4,r3,0x8

l000037A4:
	bsr.n	setusercontext
	or	r5,r0,0xFB
	bcnd.n	lt0,r2,0000387C
	or.u	r2,r0,1

l000037B4:
	bcnd.n	ne0,r18,00003848
	or.u	r2,r0,1

l000037BC:
	ld	r3,r17,0x6018
	ld	r2,r20,0x601C
	or.u	r12,r0,1
	or	r5,r12,0x5384
	bsr.n	auth_approval
	or	r4,r0,0
	bcnd	eq0,r2,00003818

l000037D8:
	bsr.n	closefrom
	or	r2,r0,3
	ld	r2,r20,0x601C
	bsr.n	auth_setstate
	or	r3,r0,1
	bsr.n	auth_close
	ld	r2,r20,0x601C
	addu	r3,r30,0x450
	or	r4,r0,0
	bsr.n	execlp
	or	r2,r0,r22
	or.u	r3,r0,1
	or	r3,r3,0x52F4
	or	r4,r0,r22
	bsr.n	err
	or	r2,r0,1

l00003818:
	bsr.n	auth_getstate
	ld	r2,r20,0x601C
	bb0	0,r2,0000383C

l00003824:
	or.u	r2,r0,1
	or	r2,r2,0x4D98

l0000382C:
	bsr	puts
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x64

l0000383C:
	or.u	r2,r0,1
	br.n	0000382C
	or	r2,r2,0x4DBC

l00003848:
	ld	r13,r23,0x6ED0
	or	r2,r2,0x4D80
	bsr.n	printf
	ld	r3,r13,0x20
	or.u	r2,r0,1
	bsr.n	puts
	or	r2,r2,0x4DD0
	or.u	r3,r0,1
	or	r2,r14,0x4C38
	or	r3,r3,0x4DEC
	or	r4,r0,1
	bsr.n	setenv
	subu	r1,r1,0xC0

l0000387C:
	bsr.n	warn
	or	r2,r2,0x4C98
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0xDC

l00003890:
	or	r2,r2,0x4D34
	bsr.n	puts
	subu	r1,r1,0x10C

l0000389C:
	or	r3,r3,0x4D10
	or	r2,r0,3
	bsr.n	syslog
	subu	r1,r1,0x128

l000038AC:
	br.n	00003744
	or	r3,r0,r22

l000038B4:
	ld	r2,r17,0x6018
	or.u	r3,r0,1
	or	r3,r3,0x4DF0
	bsr.n	login_getcapstr
	or	r5,r0,0
	bcnd	eq0,r2,000038D0

l000038CC:
	bsr	auth_cat

l000038D0:
	bsr	fn00004110
	or	r2,r0,r16
	bsr.n	stat
	addu	r3,r30,0x78
	bcnd	ne0,r2,000036EC

l000038E4:
	ld	r13,r30,0xC8
	ld	r12,r30,0xCC
	or	r13,r13,r12
	bcnd	eq0,r13,000036EC

l000038F4:
	ld	r12,r30,0xA8
	ld	r13,r30,0x98
	cmp	r12,r12,r13
	bb1.n	0,r12,00003928
	or.u	r13,r0,1

l00003908:
	bb1.n	0,r12,0000393C
	or.u	r13,r0,1

l00003910:
	ld	r13,r30,0xAC
	ld	r12,r30,0x9C
	cmp	r13,r13,r12
	bb1.n	0,r13,0000393C
	or.u	r13,r0,1

l00003924:
	or.u	r13,r0,1

l00003928:
	or	r3,r13,0x4DFC

l0000392C:
	or.u	r2,r0,1
	or	r2,r2,0x4E04
	bsr.n	printf
	subu	r1,r1,0x250

l0000393C:
	br.n	0000392C
	or	r3,r13,0x5224

l00003944:
	br.n	000036CC
	or	r7,r13,0x5224

l0000394C:
	or.u	r13,r0,6
	ld	r4,r12,0x6024
	ld	r5,r13,0x6ED8
	or.u	r3,r0,1
	or	r3,r3,0x4E18
	or	r2,r0,5
	bsr.n	syslog
	subu	r1,r1,0x288

l0000396C:
	or.u	r13,r0,6
	ld	r12,r13,0x6ED0
	ld	r4,r12,0xC
	br	00003670

l0000397C:
	ld	r13,r23,0x6ED0
	ld	r2,r13,0x8
	bsr.n	fn000049A0
	subu	r1,r1,0x350

l0000398C:
	addu	r2,r30,0x120
	bsr.n	strncpy
	subu	r1,r1,0x37C

l00003998:
	ld	r2,r17,0x6018
	or	r4,r0,0
	or.u	r5,r0,0x12
	or	r5,r5,0x7500
	or	r6,r0,r4
	or	r7,r0,r5
	bsr.n	login_getcaptime
	or	r3,r3,0x4CDC
	cmp	r13,r2,r24
	bb1	0,r13,000039D0

l000039C0:
	bb1.n	0x18,r13,000035D0
	cmp	r13,r3,r25

l000039C8:
	bb1.n	0x18,r13,000035D4
	or	r3,r0,0

l000039D0:
	ld	r2,r23,0x6ED0
	bsr.n	ctime
	addu	r2,r2,0x28
	or	r3,r0,r2
	or.u	r2,r0,1
	or	r2,r2,0x4E30
	bsr.n	printf
	subu	r1,r1,0x420

l000039F0:
	bcnd.n	ne0,r2,000035D4
	or	r3,r0,0

l000039F8:
	bcnd	eq0,r25,000035D4

l000039FC:
	br	000035C8

l00003A00:
	or.u	r2,r0,1
	bsr.n	puts
	or	r2,r2,0x4D98
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x448

l00003A18:
	ld	r2,r17,0x6018
	or.u	r3,r0,1
	or	r3,r3,0x4E54
	bsr.n	login_getcapbool
	or	r4,r0,0
	bcnd.n	ne0,r2,00003570
	or.u	r2,r0,1

l00003A34:
	or	r2,r2,0x4E60
	bsr.n	access
	or	r3,r0,0
	bcnd	ne0,r2,00003574

l00003A44:
	or	r21,r0,1
	br	00003574

l00003A4C:
	ld	r2,r21,0x6018
	or.u	r3,r0,1
	or	r3,r3,0x4E6C
	bsr.n	login_getcapbool
	or	r4,r0,0
	bcnd	ne0,r2,00003A80

l00003A64:
	or.u	r2,r0,1
	bsr.n	chdir
	or	r2,r2,0x4DEC
	bcnd.n	eq0,r2,00003554
	or	r2,r0,0

l00003A78:
	bsr.n	fn00004780
	subu	r1,r1,0x52C

l00003A80:
	ld	r13,r24,0x6ED0
	or.u	r2,r0,1
	or	r2,r2,0x4D80
	bsr.n	printf
	ld	r3,r13,0x20
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x3C

l00003AA0:
	ld	r2,r21,0x6018
	bsr.n	auth_checknologin
	subu	r1,r1,0x584

l00003AAC:
	bsr.n	warn
	or	r2,r2,0x4C98
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x5A4

l00003AC0:
	or	r2,r2,0x4C8C
	bsr.n	setenv
	or	r4,r0,1
	addu	r2,r2,1
	bcnd.n	ne0,r2,000034F8
	or.u	r2,r0,1

l00003AD8:
	bsr.n	warn
	or	r2,r2,0x4C40
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x5F4

l00003AEC:
	or	r2,r2,0x4C80
	bsr.n	setenv
	or	r4,r0,1
	addu	r2,r2,1
	bcnd.n	ne0,r2,000034EC
	or.u	r15,r0,2

l00003B04:
	or.u	r2,r0,1
	bsr.n	warn
	or	r2,r2,0x4C40
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x634

l00003B1C:
	ld	r13,r23,0x6ED0
	or.u	r2,r0,1
	or	r2,r2,0x4E78
	ld	r3,r0,r13
	bsr.n	setenv
	or	r4,r0,1
	addu	r2,r2,1
	bcnd.n	eq0,r2,000034C8
	or.u	r2,r0,1

l00003B40:
	or.u	r2,r0,1
	or	r2,r2,0x4E80
	or	r3,r0,r16
	bsr.n	setenv
	or	r4,r0,1
	addu	r2,r2,1
	bcnd.n	ne0,r2,000034DC
	or.u	r11,r0,6

l00003B60:
	br.n	000034C8
	or.u	r2,r0,1

l00003B68:
	or.u	r13,r0,6
	bsr.n	fn000046E0
	ld	r2,r13,0x6ED8
	or	r3,r0,r2
	or	r4,r0,0x40
	or	r2,r0,r24
	bsr.n	strlcpy
	subu	r1,r1,0x72C

l00003B88:
	or.u	r3,r0,1
	or	r3,r3,0x4E88

l00003B90:
	bsr.n	err
	or	r2,r0,1

l00003B98:
	or.u	r13,r0,6
	ld	r25,r13,0x6D84
	ld	r2,r0,r25
	bcnd.n	eq0,r2,00003BDC
	or	r24,r0,r25

l00003BAC:
	or.u	r21,r0,1

l00003BB0:
	or	r3,r21,0x4E90
	bsr.n	strncmp
	or	r4,r0,3
	or.u	r3,r0,1
	or	r3,r3,0x4E94
	bcnd.n	ne0,r2,00003BE4
	or	r4,r0,4

l00003BCC:
	addu	r24,r24,4
	ld	r13,r0,r24
	bcnd.n	ne0,r13,00003BB0
	or	r2,r0,r13

l00003BDC:
	br.n	000033F0
	st	r0,r0,r25

l00003BE4:
	bsr.n	strncmp
	ld	r2,r0,r24
	or.u	r3,r0,1
	or	r3,r3,0x4E9C
	bcnd.n	eq0,r2,00003BCC
	or	r4,r0,9

l00003BFC:
	bsr.n	strncmp
	ld	r2,r0,r24
	or.u	r3,r0,1
	or	r3,r3,0x4EA8
	bcnd.n	eq0,r2,00003BCC
	or	r4,r0,4

l00003C14:
	bsr.n	strncmp
	ld	r2,r0,r24
	bcnd	eq0,r2,00003BCC

l00003C20:
	ld	r13,r0,r24
	st	r13,r0,r25
	br.n	00003BCC
	addu	r25,r25,4

l00003C30:
	bsr	strlen
	cmp	r2,r2,0x3FF
	bb1.n	0xF,r2,000033D0
	or.u	r3,r0,1

l00003C40:
	or	r3,r3,0x4EB0
	or	r2,r0,3
	bsr.n	syslog
	or	r4,r0,r22
	or.u	r2,r0,1
	bsr.n	warnx
	or	r2,r2,0x4EC8
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x898

l00003C68:
	ld	r2,r17,0x6018
	or.u	r3,r0,1
	or	r4,r0,0
	or	r5,r0,0
	or	r6,r0,0
	or	r7,r0,0
	bsr.n	login_getcaptime
	or	r3,r3,0x4ED8
	or	r13,r0,r2
	or	r13,r13,r3
	bcnd.n	ne0,r13,00003D38
	st	r0,r30,0x24

l00003C98:
	or.u	r3,r0,0
	or	r3,r3,0x47C0
	bsr.n	signal
	or	r2,r0,1
	or.u	r13,r0,2
	ld	r12,r13,0x6018
	ld	r2,r20,0x601C
	or	r3,r0,r24
	ld	r5,r0,r12
	or	r4,r0,0
	bsr.n	auth_verify
	or	r6,r0,0
	bsr.n	auth_getstate
	ld	r2,r20,0x601C
	bb0.n	0,r2,00003D24
	st	r2,r30,0x28

l00003CD8:
	ld	r13,r30,0x24
	bcnd.n	eq0,r13,00003D24
	or	r11,r0,1

l00003CE4:
	st	r11,r30,0x28

l00003CE8:
	ld	r2,r20,0x601C
	or.u	r13,r0,1
	or	r4,r0,r24
	bsr.n	auth_setoption
	or	r3,r13,0x517C
	bcnd	ge0,r2,000031F8

l00003D00:
	or.u	r3,r0,1
	or	r3,r3,0x5190
	bsr.n	syslog
	or	r2,r0,3
	bsr.n	warn
	or	r2,r0,0
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0xB2C

l00003D24:
	ld	r12,r30,0x28
	mask	r13,r12,7
	bcnd.n	ne0,r13,00003CE8
	st	r0,r30,0x24

l00003D34:
	br	00003214

l00003D38:
	ld	r2,r20,0x601C
	or.u	r13,r0,1
	or.u	r11,r0,1
	or	r12,r0,1
	or	r3,r13,0x5184
	or	r4,r11,0x4F98
	st	r12,r30,0x24
	bsr.n	auth_setoption
	subu	r1,r1,0xC4

l00003D5C:
	bsr.n	auth_setpwd
	ld	r2,r20,0x601C
	bcnd.n	ge0,r2,00003140
	or.u	r3,r0,1

l00003D6C:
	or	r3,r3,0x5190
	bsr.n	syslog
	or	r2,r0,3
	bsr.n	warn
	or	r2,r0,0
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0xC4C

l00003D8C:
	ld	r3,r13,0x6024
	bsr.n	strcmp
	addu	r2,r30,0x450
	bcnd.n	eq0,r2,00003118
	or.u	r12,r0,2

l00003DA0:
	ld	r13,r23,0x6ED0
	ld	r12,r21,0x6E8C
	cmp	r13,r13,0
	extu	r13,r13,0xD,2
	cmp	r12,r12,r13
	bb1.n	0,r12,00003DC8
	addu	r2,r30,0x450
	or.u	r13,r0,6
	br.n	00003114
	st	r0,r13,0x6E8C
	bsr.n	fn00004520
	subu	r1,r1,0x14

l00003DD0:
	or.u	r3,r0,1

l00003DD4:
	or	r3,r3,0x5190
	bsr.n	syslog
	or	r2,r0,3
	bsr.n	warn
	or	r2,r0,0
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0xD48

l00003DF4:
	ld	r2,r12,0x6014
	or	r14,r0,0
	bsr.n	alarm
	subu	r1,r1,0xDB8

l00003E04:
	st	r0,r30,0x1C
	bsr.n	fn00003F90
	subu	r1,r1,0xDCC

l00003E10:
	ld	r2,r24,0x601C
	or	r3,r3,0x5014
	bsr.n	auth_setoption
	subu	r1,r1,0xE64

l00003E20:
	bsr.n	fn00004780
	subu	r1,r1,0xF08

l00003E28:
	br.n	00002EE8
	or	r5,r13,0x515C

l00003E30:
	or.u	r2,r0,1
	bsr.n	warnx
	or	r2,r2,0x4EE8
	or	r2,r0,1
	bsr.n	fn00004780
	subu	r1,r1,0x1034

l00003E48:
	or.u	r11,r0,2
	ld	r2,r11,0x6014
	or	r14,r0,0
	bsr.n	alarm
	subu	r1,r1,0x1090

l00003E5C:
	or	r3,r3,0x50CC
	bsr.n	syslog
	or	r2,r0,3
	set	r12,r0,0xC,0x1F
	or	r13,r0,0
	st.d	r12,r30,0x68
	br.n	00002DB0
	st.d	r12,r30,0x70

l00003E7C:
	or	r3,r3,0x50AC
	bsr.n	syslog
	or	r2,r0,3
	set	r12,r0,0xC,0x1F
	or	r13,r0,0
	st.d	r12,r30,0x68
	br.n	00002D8C
	st.d	r12,r30,0x70

l00003E9C:
	ld	r12,r30,0x8
	or.u	r13,r0,6
	br.n	00002D78
	st	r12,r13,0x6ED8

l00003EAC:
	bsr.n	auth_close
	ld	r2,r20,0x601C
	bsr	closelog
	bsr.n	closefrom
	or	r2,r0,3
	or.u	r13,r0,1
	or	r13,r13,0x4F0C
	st	r13,r30,0x85C
	ld	r13,r30,0x20
	or.u	r12,r0,1
	or	r25,r12,0x4F10
	st	r25,r30,0x858
	bcnd.n	ne0,r13,00003EF4
	addu	r11,r30,0x860

l00003EE4:
	or.u	r13,r0,1
	or	r13,r13,0x4F1C
	st	r13,r30,0x860
	addu	r11,r30,0x864

l00003EF4:
	bcnd.n	eq0,r19,00003F24
	or.u	r13,r0,2

l00003EFC:
	st	r0,r0,r11
	addu	r3,r30,0x858
	bsr.n	execv
	or	r2,r0,r25
	or.u	r2,r0,1
	or	r2,r2,0x4F20
	bsr.n	warn
	or	r3,r0,r25
	bsr.n	_exit
	or	r2,r0,1

l00003F24:
	ld	r12,r13,0x6024
	st	r12,r0,r11
	br.n	00003EFC
	addu	r11,r11,4

l00003F34:
	or.u	r3,r0,1
	or	r3,r3,0x4F34
	bsr.n	syslog
	or	r2,r0,3
	or.u	r3,r0,1
	br.n	00003B90
	or	r3,r3,0x4F44

l00003F50:
	bsr.n	strchr
	or	r3,r0,0x2E
	bcnd	ne0,r2,00002984

l00003F5C:
	or	r21,r0,r17
	br	00002984

l00003F64:
	or.u	r3,r0,1
	or	r3,r3,0x4F6C
	bsr.n	syslog
	or	r2,r0,3
	or.u	r3,r0,1
	or	r3,r3,0x4F8C
	or	r2,r0,r17
	or	r4,r0,0x100
	bsr.n	strlcpy
	subu	r1,r1,0x1608
	or	r0,r0,r0

;; fn00003F90: 00003F90
;;   Called from:
;;     00003E08 (in fn000028D0)
fn00003F90 proc
	subu	r31,r31,0x30
	st.d	r22,r31,0x10
	st.d	r20,r31,0x8
	st	r30,r31,0x20
	st	r19,r31,0x4
	st.d	r24,r31,0x18
	st	r1,r31,0x24
	addu	r30,r31,0x20
	or.u	r19,r0,1
	or.u	r24,r0,6
	or.u	r21,r0,6
	or.u	r20,r0,2
	or.u	r22,r0,6
	or.u	r23,r0,6

;; fn00003FC8: 00003FC8
;;   Called from:
;;     00003FC4 (in fn00003F90)
;;     00003FC4 (in fn000028D0)
fn00003FC8 proc
	bsr.n	printf
	or	r2,r19,0x51F8
	or	r13,r21,0x6D3C
	st	r13,r24,0x6D80

l00003FD8:
	ld	r13,r22,0x6D18
	or	r12,r23,0x6C10
	or	r2,r0,r12
	bcnd.n	ne0,r13,0000409C
	or.u	r11,r0,6

l00003FEC:
	ld	r13,r12,0x4
	subu	r13,r13,1
	bcnd.n	lt0,r13,00004090
	st	r13,r12,0x4

l00003FFC:
	ld	r13,r11,0x6C10
	ld.bu	r25,r0,r13
	addu	r13,r13,1
	st	r13,r11,0x6C10

l0000400C:
	cmp	r13,r25,0xA
	bb0.n	0,r13,00004054
	addu	r12,r25,1

l00004018:
	bcnd	eq0,r12,00004040

l0000401C:
	ld	r12,r24,0x6D80
	or.u	r13,r0,6
	or	r13,r13,0x6D7D
	cmp	r13,r12,r13
	bb1.n	0x1F,r13,00003FD8
	addu	r11,r12,1

l00004034:
	st.b	r25,r0,r12
	br.n	00003FD8
	st	r11,r24,0x6D80

l00004040:
	bsr.n	fn00004520
	ld	r2,r20,0x6024
	or	r2,r0,0
	bsr.n	fn00004780
	subu	r1,r1,0x38

l00004054:
	ld	r12,r24,0x6D80
	or	r11,r21,0x6D3C
	cmp	r13,r12,r11
	bb1	0x1E,r13,fn00003FC8

l00004064:
	ld.b	r13,r21,0x6D3C
	cmp	r13,r13,0x2D
	bb0.n	0,r13,000040A4
	or.u	r2,r0,1

l00004074:
	or.u	r5,r0,6
	or	r5,r5,0x6CC0
	or	r3,r0,1
	or	r4,r0,0x24
	or	r2,r2,0x5200
	bsr.n	fwrite
	subu	r1,r1,0xC8

l00004090:
	bsr	__srget
	br.n	0000400C
	or	r25,r0,r2

l0000409C:
	bsr.n	getc
	subu	r1,r1,0x10

l000040A4:
	st.b	r0,r0,r12
	subu	r31,r30,0x20
	st	r11,r20,0x6024
	ld	r1,r31,0x24
	ld	r30,r31,0x20
	ld	r19,r31,0x4
	ld.d	r24,r31,0x18
	ld.d	r22,r31,0x10
	ld.d	r20,r31,0x8
	addu	r31,r31,0x30
	jmp	r1

;; fn000040D0: 000040D0
;;   Called from:
;;     00003208 (in fn000028D0)
;;     000032EC (in fn000028D0)
fn000040D0 proc
	subu	r31,r31,0x10
	st	r30,r0,r31
	st	r1,r31,0x4
	bsr.n	getttynam
	addu	r30,r31,0
	bcnd.n	eq0,r2,000040F8
	or	r12,r0,0

l000040EC:
	ld	r13,r2,0xC
	bb0	0,r13,000040F8

l000040F4:
	or	r12,r0,1

l000040F8:
	addu	r31,r30,0
	or	r2,r0,r12
	ld	r1,r31,0x4
	ld	r30,r0,r31
	addu	r31,r31,0x10
	jmp	r1

;; fn00004110: 00004110
;;   Called from:
;;     000038D0 (in fn000028D0)
fn00004110 proc
	subu	r31,r31,0x2050
	st	r1,r31,0x24
	st	r30,r31,0x20
	st	r21,r31,0xC
	st.d	r24,r31,0x18
	st.d	r22,r31,0x10
	or.u	r13,r0,2
	or.u	r21,r0,2
	ld	r2,r13,0x6018
	or.u	r4,r0,1
	ld	r13,r21,0x6000
	or	r4,r4,0x5228
	or.u	r3,r0,1
	addu	r30,r31,0x20
	or	r5,r0,r4
	or	r3,r3,0x5234
	st	r13,r30,0x2028
	bsr	login_getcapstr
	or	r3,r0,0
	bsr.n	open
	or	r4,r0,0
	bcnd.n	lt0,r2,000041F0
	or	r24,r0,r2

l0000416C:
	addu	r22,r30,0x18
	or.u	r11,r0,0
	or	r12,r0,0
	or	r13,r0,0
	or	r11,r11,0x4240
	addu	r3,r30,8
	or	r2,r0,2
	or	r4,r0,r22
	st.d	r12,r30,0x8
	st	r0,r30,0x10
	st	r11,r30,0x8
	st	r0,r30,0xC
	bsr.n	sigaction
	addu	r23,r30,0x28
	or	r3,r0,r23

l000041A8:
	or	r4,r0,0x2000
	bsr.n	read
	or	r2,r0,r24
	or	r25,r0,r2
	or	r4,r0,r2
	or	r3,r0,r23
	bcnd.n	le0,r25,000041D8
	or	r2,r0,1

l000041C8:
	bsr	write
	cmp	r2,r2,r25
	bb1.n	0x1F,r2,000041A8
	or	r3,r0,r23

l000041D8:
	or	r3,r0,r22
	or	r2,r0,2
	bsr.n	sigaction
	or	r4,r0,0
	bsr.n	close
	or	r2,r0,r24

l000041F0:
	ld	r12,r21,0x6000
	tb1	7,r0,0xFF
	ld	r13,r30,0x2028
	cmp	r13,r13,r12
	bb0	0,r13,00004214
	ld	r3,r30,0x2028
	or.u	r2,r0,1
	bsr.n	__stack_smash_handler
	or	r2,r2,0x523C
	subu	r31,r30,0x20
	ld	r1,r31,0x24
	ld	r30,r31,0x20
	ld	r21,r31,0xC
	ld.d	r24,r31,0x18
	ld.d	r22,r31,0x10
	addu	r31,r31,0x2050
	jmp	r1
	or	r0,r0,r0
	or	r0,r0,r0
	or	r0,r0,r0
	subu	r31,r31,0x10
	st	r30,r0,r31
	addu	r30,r31,0
	st	r1,r31,0x4
	addu	r31,r30,0
	ld	r30,r0,r31
	addu	r31,r31,0x10
	jmp	r1
	subu	r31,r31,0x420
	st	r1,r31,0x14
	or.u	r12,r0,2
	st	r30,r31,0x10
	st	r25,r31,0xC
	or.u	r13,r0,2
	addu	r30,r31,0x10
	ld	r5,r13,0x6014
	ld	r13,r12,0x6000
	addu	r25,r30,8
	or.u	r4,r0,1
	or	r4,r4,0x5244
	or	r3,r0,0x400
	or	r2,r0,r25
	st	r13,r30,0x408
	bsr	snprintf
	bsr.n	strlen
	or	r2,r0,r25
	or	r4,r0,r2
	or	r3,r0,r25
	bsr.n	write
	or	r2,r0,2
	or.u	r13,r0,2
	ld	r2,r13,0x6024
	bcnd	ne0,r2,000042CC
	bsr.n	_exit
	or	r2,r0,0
	bsr.n	fn00004520
	subu	r1,r1,0x10
	or	r0,r0,r0
	or	r0,r0,r0
	or	r0,r0,r0

;; fn000042E0: 000042E0
;;   Called from:
;;     0000363C (in fn000028D0)
fn000042E0 proc
	subu	r31,r31,0x140
	st.d	r22,r31,0x10
	or.u	r23,r0,2
	ld	r13,r23,0x6000
	st.d	r24,r31,0x18
	or	r25,r0,r2
	or.u	r2,r0,1
	st	r30,r31,0x20
	or	r2,r2,0x5268
	addu	r30,r31,0x20
	or	r3,r0,2
	or	r4,r0,0
	st	r1,r31,0x24
	st	r21,r31,0xC
	st	r13,r30,0x118
	bsr	open
	bcnd.n	lt0,r2,000043B8
	or	r24,r0,r2

l00004328:
	or.u	r22,r0,6
	ld	r13,r22,0x6ED0
	or	r4,r0,0
	or	r5,r0,0x110
	or	r2,r0,0
	bsr.n	__muldi3
	ld	r3,r13,0x8
	or	r4,r0,r2
	or	r5,r0,r3
	or	r6,r0,0
	bsr.n	lseek
	or	r2,r0,r24
	bcnd.n	eq0,r25,000043E8
	addu	r25,r30,8

l00004360:
	addu	r21,r30,0x10
	or	r4,r0,0x110
	or	r3,r0,0
	bsr.n	memset
	or	r2,r0,r25
	bsr.n	time
	or	r2,r0,r25
	or.u	r13,r0,6
	ld	r3,r13,0x6ED8
	or	r2,r0,r21
	bsr.n	strncpy
	or	r4,r0,8
	or.u	r13,r0,6
	ld	r3,r13,0x6E88
	bcnd.n	ne0,r3,000043DC
	or	r4,r0,0x100

l000043A0:
	or	r3,r0,r25
	or	r2,r0,r24
	bsr.n	write
	or	r4,r0,0x110
	bsr.n	close
	or	r2,r0,r24

l000043B8:
	ld	r12,r23,0x6000
	tb1	7,r0,0xFF
	ld	r13,r30,0x118
	cmp	r13,r13,r12
	bb0	2,r13,000044F4
	ld	r3,r30,0x118
	or.u	r2,r0,1
	bsr.n	__stack_smash_handler
	or	r2,r2,0x527C

l000043DC:
	addu	r2,r30,0x18
	bsr.n	strncpy
	subu	r1,r1,0x48

l000043E8:
	or	r2,r0,r24
	or	r3,r0,r25
	bsr.n	read
	or	r4,r0,0x110
	cmp	r2,r2,0x110
	bb0.n	0,r2,00004434
	addu	r21,r30,0x10

l00004404:
	ld	r13,r22,0x6ED0
	or	r4,r0,0
	or	r5,r0,0x110
	or	r2,r0,0
	bsr.n	__muldi3
	ld	r3,r13,0x8
	or	r4,r0,r2
	or	r5,r0,r3
	or	r6,r0,0
	or	r2,r0,r24
	bsr.n	lseek
	subu	r1,r1,0xD0

l00004434:
	ld	r13,r30,0x8
	ld	r12,r30,0xC
	or	r13,r13,r12
	bcnd	eq0,r13,00004404

l00004444:
	bsr.n	ctime
	or	r2,r0,r25
	or	r4,r0,r2
	or.u	r2,r0,1
	or	r3,r0,0x13
	bsr.n	printf
	or	r2,r2,0x5288
	addu	r21,r30,0x10
	or.u	r2,r0,1
	or	r2,r2,0x529C
	or	r3,r0,8
	bsr.n	printf
	or	r4,r0,r21
	ld.b	r13,r30,0x18
	bcnd.n	ne0,r13,000044E0
	or.u	r2,r0,1

l00004484:
	or.u	r13,r0,6
	ld	r12,r13,0x6D18
	bcnd.n	ne0,r12,000044CC
	or.u	r11,r0,6

l00004494:
	or	r3,r11,0x6C68
	ld	r13,r3,0x8
	subu	r13,r13,1
	bcnd.n	lt0,r13,000044C0
	st	r13,r3,0x8

l000044A8:
	ld	r13,r11,0x6C68
	or	r12,r0,0xA
	st.b	r12,r0,r13
	addu	r13,r13,1
	br.n	00004404
	st	r13,r11,0x6C68

l000044C0:
	or	r2,r0,0xA
	bsr.n	__swbuf
	subu	r1,r1,0xC8

l000044CC:
	or.u	r3,r0,6
	or	r3,r3,0x6C68
	or	r2,r0,0xA
	bsr.n	putc
	subu	r1,r1,0xDC

l000044E0:
	or	r2,r2,0x52A4
	addu	r4,r30,0x18
	or	r3,r0,0x100
	bsr.n	printf
	subu	r1,r1,0x70
	subu	r31,r30,0x20
	ld	r1,r31,0x24
	ld	r30,r31,0x20
	ld	r21,r31,0xC
	ld.d	r24,r31,0x18
	ld.d	r22,r31,0x10
	addu	r31,r31,0x140
	jmp	r1
00004514             F4 00 58 00 F4 00 58 00 F4 00 58 00     ..X...X...X.

;; fn00004520: 00004520
;;   Called from:
;;     00003298 (in fn000028D0)
;;     00004040 (in fn00003FC8)
;;     000047E4 (in fn00004780)
fn00004520 proc
	subu	r31,r31,0x50
	st	r30,r31,0x30
	addu	r30,r31,0x30
	st.d	r24,r31,0x28
	or.u	r3,r0,1
	addu	r24,r30,8
	st.d	r22,r31,0x20
	or	r3,r3,0x5340
	or	r22,r0,r2
	or	r4,r0,0x10
	or	r2,r0,r24
	or.u	r25,r0,6
	st	r1,r31,0x34
	bsr.n	memcpy
	st	r21,r31,0x1C
	ld	r5,r25,0x6E8C
	bcnd.n	eq0,r5,000046B4
	or.u	r23,r0,6

l00004568:
	ld	r13,r23,0x6E88
	bcnd.n	eq0,r13,00004640
	cmp	r13,r5,1

l00004574:
	bb0.n	1,r13,00004638
	or.u	r13,r0,1

l0000457C:
	or.u	r13,r0,1
	or	r6,r13,0x52B0

l00004584:
	or.u	r21,r0,2
	ld	r7,r21,0x6028
	bcnd.n	ne0,r7,0000459C
	or	r12,r0,r7

l00004594:
	or.u	r13,r0,1
	or	r7,r13,0x5224

l0000459C:
	bcnd.n	eq0,r12,00004630
	or.u	r13,r0,1

l000045A4:
	or.u	r13,r0,1
	or	r8,r13,0x4BE8

l000045AC:
	ld	r9,r23,0x6E88
	or.u	r4,r0,1
	or	r4,r4,0x52B4
	or	r2,r0,5
	bsr.n	syslog_r
	or	r3,r0,r24
	ld	r5,r25,0x6E8C
	cmp	r13,r5,1
	bb0.n	0,r13,00004628
	or.u	r13,r0,1

l000045D4:
	or.u	r13,r0,1
	or	r6,r13,0x52B0

l000045DC:
	ld	r7,r21,0x6028
	bcnd.n	ne0,r7,000045F0
	or	r12,r0,r7

l000045E8:
	or.u	r13,r0,1
	or	r7,r13,0x5224

l000045F0:
	bcnd.n	eq0,r12,00004620
	or.u	r13,r0,1

l000045F8:
	or.u	r13,r0,1
	or	r8,r13,0x4BE8

l00004600:
	ld	r9,r23,0x6E88
	or.u	r4,r0,1
	or	r3,r0,r24
	or	r4,r4,0x52D4
	or	r2,r0,0x55
	st	r22,r0,r31
	bsr.n	syslog_r
	addu	r1,r1,0x94

l00004620:
	br.n	00004600
	or	r8,r13,0x5224

l00004628:
	br.n	000045DC
	or	r6,r13,0x5224

l00004630:
	br.n	000045AC
	or	r8,r13,0x5224

l00004638:
	br.n	00004584
	or	r6,r13,0x5224

l00004640:
	bb0.n	0,r13,000046AC
	or.u	r13,r0,1

l00004648:
	or.u	r13,r0,1
	or	r6,r13,0x52B0

l00004650:
	or.u	r23,r0,6
	ld	r7,r23,0x6ED8
	or.u	r4,r0,1
	or	r4,r4,0x52F8
	or	r2,r0,5
	bsr.n	syslog_r
	or	r3,r0,r24
	ld	r5,r25,0x6E8C
	cmp	r13,r5,1
	bb0.n	0,r13,000046A4
	or.u	r13,r0,1

l0000467C:
	or.u	r13,r0,1
	or	r6,r13,0x52B0

l00004684:
	ld	r7,r23,0x6ED8
	or.u	r4,r0,1
	or	r3,r0,r24
	or	r4,r4,0x5314
	or	r8,r0,r22
	or	r2,r0,0x55
	bsr.n	syslog_r
	addu	r1,r1,0x10

l000046A4:
	br.n	00004684
	or	r6,r13,0x5224

l000046AC:
	br.n	00004650
	or	r6,r13,0x5224

l000046B4:
	subu	r31,r30,0x30
	ld	r1,r31,0x34
	ld	r30,r31,0x30
	ld	r21,r31,0x1C
	ld.d	r24,r31,0x28
	ld.d	r22,r31,0x20
	addu	r31,r31,0x50
	jmp	r1
000046D4             F4 00 58 00 F4 00 58 00 F4 00 58 00     ..X...X...X.

;; fn000046E0: 000046E0
;;   Called from:
;;     00003B6C (in fn000028D0)
fn000046E0 proc
	subu	r31,r31,0x10
	st	r30,r0,r31
	st	r1,r31,0x4
	bcnd.n	eq0,r2,00004708
	addu	r30,r31,0

l000046F4:
	bsr	getttynam
	bcnd	eq0,r2,00004708

l000046FC:
	ld	r2,r2,0x8
	br.n	0000472C
	addu	r31,r30,0

l00004708:
	or.u	r13,r0,2
	or.u	r4,r0,1
	ld	r2,r13,0x6018
	or	r4,r4,0x5334
	or.u	r3,r0,1
	or	r3,r3,0x5338
	bsr.n	login_getcapstr
	or	r5,r0,r4
	addu	r31,r30,0

l0000472C:
	ld	r1,r31,0x4
	ld	r30,r0,r31
	addu	r31,r31,0x10
	jmp	r1
0000473C                                     F4 00 58 00             ..X.

;; fn00004740: 00004740
;;   Called from:
;;     000032A4 (in fn000028D0)
fn00004740 proc
	or.u	r13,r0,2
	ld	r12,r13,0x601C
	subu	r31,r31,0x20
	st	r30,r31,0x10
	st	r25,r31,0xC
	addu	r30,r31,0x10
	or	r25,r0,r2
	st	r1,r31,0x14
	bsr.n	auth_close
	or	r2,r0,r12
	bsr.n	sleep
	or	r2,r0,5
	bsr.n	exit
	or	r2,r0,r25
	or	r0,r0,r0
	or	r0,r0,r0

;; fn00004780: 00004780
;;   Called from:
;;     00002ABC (in fn000028D0)
;;     00002B60 (in fn000028D0)
;;     00002B84 (in fn000028D0)
;;     00002BBC (in fn000028D0)
;;     00002C50 (in fn000028D0)
;;     00002C68 (in fn000028D0)
;;     00002CA4 (in fn000028D0)
;;     00002CEC (in fn000028D0)
;;     00002EA8 (in fn000028D0)
;;     0000338C (in fn000028D0)
;;     00003444 (in fn000028D0)
;;     000034D0 (in fn000028D0)
;;     00003834 (in fn000028D0)
;;     00003888 (in fn000028D0)
;;     00003A10 (in fn000028D0)
;;     00003A78 (in fn000028D0)
;;     00003A98 (in fn000028D0)
;;     00003AB8 (in fn000028D0)
;;     00003AE4 (in fn000028D0)
;;     00003B14 (in fn000028D0)
;;     00003C60 (in fn000028D0)
;;     00003D1C (in fn000028D0)
;;     00003D84 (in fn000028D0)
;;     00003DEC (in fn000028D0)
;;     00003E20 (in fn000028D0)
;;     00003E40 (in fn000028D0)
;;     0000404C (in fn00003FC8)
;;     0000477C (in fn00004740)
fn00004780 proc
	subu	r31,r31,0x20
	st	r30,r31,0x10
	st	r25,r31,0xC
	st	r1,r31,0x14
	or.u	r13,r0,2
	ld	r13,r13,0x601C
	addu	r30,r31,0x10
	bcnd.n	ne0,r13,000047AC
	or	r25,r0,r2

l000047A4:
	bsr.n	exit
	or	r2,r0,r25

l000047AC:
	or	r2,r0,r13
	bsr.n	auth_close
	subu	r1,r1,0x14
	or	r0,r0,r0
	or	r0,r0,r0
	or.u	r13,r0,2
	ld	r2,r13,0x6024
	subu	r31,r31,0x10
	st	r30,r0,r31
	st	r1,r31,0x4
	bcnd.n	ne0,r2,000047E4
	addu	r30,r31,0

l000047DC:
	bsr.n	_exit
	or	r2,r0,0

l000047E4:
	bsr.n	fn00004520
	subu	r1,r1,0x10
	or	r0,r0,r0

;; fn000047F0: 000047F0
;;   Called from:
;;     000032C8 (in fn000028D0)
fn000047F0 proc
	subu	r31,r31,0x170
	st	r19,r31,0x4
	or.u	r19,r0,2
	ld	r13,r19,0x6000
	st.d	r24,r31,0x18
	or	r24,r0,r2
	or.u	r2,r0,1
	st.d	r20,r31,0x8
	st	r30,r31,0x20
	st.d	r22,r31,0x10
	addu	r30,r31,0x20
	or	r22,r0,r3
	or	r20,r0,r4
	or	r2,r2,0x5350
	or	r3,r0,2
	or	r4,r0,0x180
	st	r1,r31,0x24
	st	r13,r30,0x140
	bsr.n	open
	or	r21,r0,r5
	bcnd.n	lt0,r2,fn00004928
	or	r25,r0,r2

;; fn00004848: 00004848
;;   Called from:
;;     00004844 (in fn000047F0)
;;     00004844 (in fn00004780)
fn00004848 proc
	or	r4,r0,0
	or	r5,r0,0x138
	or	r3,r0,r24
	bsr.n	__muldi3
	or	r2,r0,0
	or	r4,r0,r2
	or	r5,r0,r3
	or	r6,r0,0
	or	r2,r0,r25
	bsr.n	lseek
	addu	r23,r30,8
	or	r2,r0,r25
	or	r3,r0,r23
	bsr.n	read
	or	r4,r0,0x138
	cmp	r2,r2,0x138
	bb0.n	1,r2,0000495C
	or	r2,r0,r23

l00004890:
	or	r3,r0,0

l00004894:
	bsr.n	memset
	or	r4,r0,0x138

l0000489C:
	or	r4,r0,0
	or	r5,r0,0x138
	or	r3,r0,r24
	bsr.n	__muldi3
	or	r2,r0,0
	or	r4,r0,r2
	or	r5,r0,r3
	or	r6,r0,0
	bsr.n	lseek
	or	r2,r0,r25
	ld	r13,r30,0x138
	addu	r2,r30,0x130
	addu	r13,r13,1
	bsr.n	time
	st	r13,r30,0x138
	or	r2,r0,r23
	or	r3,r0,r21
	bsr.n	strncpy
	or	r4,r0,8
	bcnd.n	eq0,r22,00004954
	addu	r2,r30,0x30

l000048F0:
	or	r3,r0,r22
	bsr.n	strncpy
	or	r4,r0,0x100

l000048FC:
	bcnd	eq0,r20,0000494C

l00004900:
	addu	r2,r30,0x10
	or	r3,r0,r20
	bsr.n	strncpy
	or	r4,r0,0x20

l00004910:
	or	r3,r0,r23
	or	r2,r0,r25
	bsr.n	write
	or	r4,r0,0x138
	bsr.n	close
	or	r2,r0,r25

;; fn00004928: 00004928
;;   Called from:
;;     00004844 (in fn000047F0)
;;     00004844 (in fn00004780)
fn00004928 proc
	ld	r12,r19,0x6000
	tb1	7,r0,0xFF
	ld	r13,r30,0x140
	cmp	r13,r13,r12
	bb0	0,r13,00004978
	ld	r3,r30,0x140
	or.u	r2,r0,1
	bsr.n	__stack_smash_handler
	or	r2,r2,0x5368

l0000494C:
	br.n	00004910
	st.b	r0,r30,0x10

l00004954:
	br.n	000048FC
	st.b	r0,r30,0x30

l0000495C:
	ld	r13,r30,0x130
	ld	r12,r30,0x134
	or	r13,r13,r12
	bcnd	ne0,r13,0000489C

l0000496C:
	or	r2,r0,r23
	br.n	00004894
	or	r3,r0,0
00004978                         67 FE 00 20 14 3F 00 24         g.. .?.$
00004980 17 DF 00 20 16 7F 00 04 13 1F 00 18 12 DF 00 10 ... ............
00004990 12 9F 00 08 63 FF 01 70 F4 00 C0 01 F4 00 58 00 ....c..p......X.

;; fn000049A0: 000049A0
;;   Called from:
;;     00003984 (in fn000028D0)
fn000049A0 proc
	subu	r31,r31,0x170
	st.d	r22,r31,0x10
	or.u	r22,r0,2
	ld	r13,r22,0x6000
	st	r30,r31,0x20
	addu	r30,r31,0x20
	st.d	r24,r31,0x18
	addu	r24,r30,8
	or	r3,r0,0
	or	r4,r0,0x138
	or	r23,r0,r2
	or	r2,r0,r24
	st	r1,r31,0x24
	st	r21,r31,0xC
	st	r13,r30,0x140
	bsr.n	memset
	or	r21,r0,0
	or.u	r2,r0,1
	or	r2,r2,0x5350
	or	r3,r0,2
	bsr.n	open
	or	r4,r0,0
	bcnd.n	lt0,r2,00004A48
	or	r25,r0,r2

l00004A00:
	or	r4,r0,0
	or	r5,r0,0x138
	or	r3,r0,r23
	bsr.n	__muldi3
	or	r2,r0,0
	or	r4,r0,r2
	or	r5,r0,r3
	or	r6,r0,0
	bsr.n	lseek
	or	r2,r0,r25
	or	r2,r0,r25
	or	r3,r0,r24
	bsr.n	read
	or	r4,r0,0x138
	cmp	r2,r2,0x138
	bb0	0,r2,00004A70

l00004A40:
	bsr.n	close
	or	r2,r0,r25

l00004A48:
	ld	r12,r22,0x6000
	tb1	7,r0,0xFF
	ld	r13,r30,0x140
	cmp	r13,r13,r12
	bb0.n	2,r13,00004BA4
	or	r2,r0,r21
	ld	r3,r30,0x140
	or.u	r2,r0,1
	bsr.n	__stack_smash_handler
	or	r2,r2,0x5378

l00004A70:
	ld	r3,r30,0x138
	bcnd.n	eq0,r3,00004A40
	cmp	r13,r3,1

l00004A7C:
	bb0.n	2,r13,00004B94
	or	r21,r0,1

l00004A84:
	bsr.n	ctime
	addu	r2,r30,0x130
	or	r4,r0,r2
	or.u	r2,r0,1
	or	r3,r0,0x13
	bsr.n	printf
	or	r2,r2,0x538C
	or.u	r2,r0,1
	or	r2,r2,0x53AC
	or	r3,r0,8
	bsr.n	printf
	or	r4,r0,r24
	ld.b	r13,r30,0x30
	bcnd.n	eq0,r13,00004AEC
	or.u	r13,r0,6

l00004AC0:
	ld.b	r13,r30,0x10
	bcnd.n	eq0,r13,00004B80
	addu	r4,r30,0x30

l00004ACC:
	or.u	r2,r0,1
	addu	r4,r30,0x10
	addu	r6,r30,0x30
	or	r3,r0,0x20
	or	r5,r0,0x100
	bsr.n	printf
	or	r2,r2,0x53B8
	or.u	r13,r0,6

l00004AEC:
	ld	r12,r13,0x6D18
	bcnd.n	ne0,r12,00004B6C
	or.u	r11,r0,6

l00004AF8:
	or	r3,r11,0x6C68
	ld	r13,r3,0x8
	subu	r13,r13,1
	bcnd.n	lt0,r13,00004B60
	st	r13,r3,0x8

l00004B0C:
	ld	r13,r11,0x6C68
	or	r12,r0,0xA
	st.b	r12,r0,r13
	addu	r13,r13,1
	st	r13,r11,0x6C68
	or	r4,r0,0
	or	r5,r0,0x138
	or	r3,r0,r23
	or	r2,r0,0
	bsr.n	__muldi3
	st	r0,r30,0x138
	or	r4,r0,r2
	or	r5,r0,r3
	or	r6,r0,0
	bsr.n	lseek
	or	r2,r0,r25
	or	r2,r0,r25
	or	r3,r0,r24
	or	r4,r0,0x138
	bsr.n	write
	subu	r1,r1,0x120

l00004B60:
	or	r2,r0,0xA
	bsr.n	__swbuf
	subu	r1,r1,0x4C

l00004B6C:
	or.u	r3,r0,6
	or	r3,r3,0x6C68
	or	r2,r0,0xA
	bsr.n	putc
	subu	r1,r1,0x60

l00004B80:
	or.u	r2,r0,1
	or	r2,r2,0x52A4
	or	r3,r0,0x100
	bsr.n	printf
	subu	r1,r1,0xAC

l00004B94:
	or.u	r2,r0,1
	or	r2,r2,0x53C8
	bsr.n	printf
	subu	r1,r1,0x120
	subu	r31,r30,0x20
	ld	r1,r31,0x24
	ld	r30,r31,0x20
	ld	r21,r31,0xC
	ld.d	r24,r31,0x18
	ld.d	r22,r31,0x10
	addu	r31,r31,0x170
	jmp	r1
00004BC4             F4 00 58 00 F4 00 58 00 F4 00 58 00     ..X...X...X.
