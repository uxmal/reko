;;; Segment .text (00401D74)

;; _start: 00401D74
_start proc
	mov.l	(00401D7C),r0                                        ; @(04,pc)
	mov	r7,r4
	braf	r0
	mov	r9,r5
00401D7C                                     40 00 00 00             @...
00401D80 86 2F 0B C7 96 2F C6 2F 09 DC 22 4F 0C 3C 09 D0 ./..././.."O.<..
00401D90 09 D8 CE 09 CC 38 92 38 04 8B 26 4F F6 6C F6 69 .....8.8..&O.l.i
00401DA0 0B 00 F6 68 86 61 0B 41 09 00 F5 AF 92 38 09 00 ...h.a.A.....8..
00401DB0 30 34 01 00 FC 00 00 00 C4 FE FF FF             04..........    

;; ___start: 00401DBC
___start proc
	mov.l	r8,@-r15
	mova	(00401E94),r0                                         ; @(D4,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	(00401E94),r12                                       ; @(C8,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#FC,r15
	tst	r5,r5
	bf/s	00401DEE
	mov	r5,r8

l00401DD6:
	mov.l	(00401E98),r2                                        ; @(C0,pc)
	mov	#13,r1
	mov.l	(00401E9C),r7                                        ; @(C0,pc)
	mov	#02,r6
	mov	#04,r4
	add	r12,r7
	mov	#00,r5
	bsrf	r2
	mov.l	r1,@r15
	mov.l	(00401EA0),r0                                        ; @(B4,pc)
	bsrf	r0
	mov	#01,r4

l00401DEE:
	mov.l	(00401EA4),r0                                        ; @(B4,pc)
	mov.l	(00401EA8),r10                                       ; @(B4,pc)
	mov.l	@(r0,r12),r1
	mov	r12,r0
	mov.l	@(8,r5),r2
	mov.l	r5,@r1
	mov.l	@(r0,r10),r1
	mov.l	(00401EAC),r0                                        ; @(AC,pc)
	mov.l	r2,@r1
	mov.l	@r5,r1
	mov.l	@r1,r2
	tst	r2,r2
	bt	00401E74

l00401E08:
	add	r12,r0
	mov.l	@r0,r3
	mov.l	r2,@r3
	mov	r0,r3
	mov.l	@r1,r1

l00401E12:
	mov.b	@r1+,r2
	tst	r2,r2
	bf/s	00401E6A
	mov	r2,r0

l00401E1A:
	tst	r4,r4
	bt	00401E24

l00401E1E:
	mov.l	(00401EB0),r1                                        ; @(90,pc)
	bsrf	r1
	nop

l00401E24:
	mov.l	(00401EB4),r2                                        ; @(8C,pc)
	mov.l	(00401EB8),r9                                        ; @(90,pc)
	bsrf	r2
	add	r12,r9
	mov.l	(00401EBC),r0                                        ; @(8C,pc)
	mov.l	@(r0,r12),r11
	cmp/hs	r11,r9

l00401E32:
	bf	00401E7E

l00401E34:
	mov.l	(00401EC0),r4                                        ; @(88,pc)
	mov.l	(00401EC4),r1                                        ; @(8C,pc)
	mov.l	(00401EC8),r9                                        ; @(8C,pc)
	bsrf	r1
	add	r12,r4
	mov.l	(00401ECC),r0                                        ; @(8C,pc)
	add	r12,r9
	mov.l	@(r0,r12),r11
	cmp/hs	r11,r9

l00401E46:
	bf	00401E88

l00401E48:
	mov.l	(00401ED0),r2                                        ; @(84,pc)
	mov.l	(00401ED4),r0                                        ; @(88,pc)
	bsrf	r2
	mov.l	@(r0,r12),r4
	mov.l	(00401ED8),r0                                        ; @(84,pc)
	bsrf	r0
	nop
	mov	r12,r0
	mov.l	@(r0,r10),r1
	mov.l	@r8,r5
	mov.l	@r1,r6
	mov.l	(00401EDC),r1                                        ; @(7C,pc)
	bsrf	r1
	mov.l	@(4,r8),r4
	mov.l	(00401EE0),r2                                        ; @(78,pc)
	bsrf	r2
	mov	r0,r4

l00401E6A:
	cmp/eq	#2F,r0
	bf	00401E12

l00401E6E:
	mov.l	@r3,r2
	bra	00401E12
	mov.l	r1,@r2

l00401E74:
	mov.l	(00401EE4),r1                                        ; @(6C,pc)
	mov.l	@(r0,r12),r2
	add	r12,r1
	bra	00401E1A
	mov.l	r1,@r2

l00401E7E:
	mov.l	@r9+,r1
	jsr	@r1
	nop
	bra	00401E32
	cmp/hs	r11,r9

l00401E88:
	mov.l	@r9+,r1
	jsr	@r1
	nop
	bra	00401E46
	cmp/hs	r11,r9
00401E92       09 00 4C 33 01 00 64 FB FF FF 8C F3 FE FF   ..L3..d.......
00401EA0 BA F9 FF FF 30 01 00 00 58 01 00 00 14 01 00 00 ....0...X.......
00401EB0 74 FD FF FF 68 FE FF FF C4 FE FF FF 2C 01 00 00 t...h.......,...
00401EC0 A0 CB FE FF 5A FD FF FF C4 FE FF FF F8 00 00 00 ....Z...........
00401ED0 48 FD FF FF 08 01 00 00 AA F8 FF FF CC 26 00 00 H............&..
00401EE0 C2 FB FF FF E4 01 00 00 86 2F 96 2F C6 2F 1A DC ........./././..
00401EF0 19 C7 0C 3C E6 2F 22 4F F3 6E 18 D0 CC 01 18 21 ...<./"O.n.....!
00401F00 23 8B 01 E1 14 0C 16 D0 CE 01 18 21 05 89 15 D0 #..........!....
00401F10 15 D4 16 D5 CC 34 03 00 CC 35 15 D0 CE 01 18 21 .....4...5.....!
00401F20 07 89 15 D0 CE 01 18 21 03 89 12 D2 03 64 03 02 .......!.....d..
00401F30 CC 34 12 D8 CC 38 FC 78 82 69 97 60 08 20 04 8D .4...8.x.i.`. ..
00401F40 FC 78 0B 49 82 69 F9 AF 97 60 E3 6F 26 4F F6 6E .x.I.i...`.o&O.n
00401F50 F6 6C F6 69 0B 00 F6 68 88 32 01 00 E8 FF FF FF .l.i...h.2......
00401F60 78 01 00 00 BA FB FF FF E8 F6 FE FF C8 FF FF FF x...............
00401F70 28 01 00 00 56 F9 FF FF D4 FE FF FF C8 FE FF FF (...V...........
00401F80 86 2F 96 2F C6 2F 16 DC 15 C7 0C 3C E6 2F 22 4F ./././.....<./"O
00401F90 F3 6E 14 D0 CC 01 18 21 1B 8B 01 E1 14 0C 12 D0 .n.....!........
00401FA0 CE 01 18 21 03 89 11 D0 11 D4 03 00 CC 34 11 D8 ...!.........4..
00401FB0 CC 38 04 78 86 69 98 29 03 89 0B 49 86 69 FB AF .8.x.i.)...I.i..
00401FC0 98 29 0D D0 CE 01 18 21 03 89 0C D0 0C D4 03 00 .).....!........
00401FD0 CC 34 E3 6F 26 4F F6 6E F6 6C F6 69 0B 00 F6 68 .4.o&O.n.l.i...h
00401FE0 00 32 01 00 E9 FF FF FF 6C 01 00 00 46 FA FF FF .2......l...F...
00401FF0 C4 FF FF FF CC FE FF FF 44 01 00 00 26 F9 FF FF ........D...&...
00402000 E8 F6 FE FF                                     ....            

;; namecmp: 00402004
namecmp proc
	mov.l	r12,@-r15
	mova	(0040201C),r0                                         ; @(14,pc)
	mov.l	(0040201C),r12                                       ; @(10,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov.l	(00402020),r1                                        ; @(10,pc)
	add	#54,r5
	bsrf	r1
	add	#54,r4
	lds.l	@r15+,pr
	rts
0040201A                               F6 6C C4 31 01 00           .l.1..
00402020 02 F8 FF FF                                     ....            

;; revnamecmp: 00402024
revnamecmp proc
	mov.l	r12,@-r15
	mova	(00402044),r0                                         ; @(1C,pc)
	mov.l	(00402044),r12                                       ; @(18,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov	r5,r1
	add	#54,r1
	mov	r4,r5
	mov	r1,r4
	mov.l	(00402048),r1                                        ; @(10,pc)
	bsrf	r1
	add	#54,r5
	lds.l	@r15+,pr
	rts
00402040 F6 6C 09 00 9C 31 01 00 DC F7 FF FF             .l...1......    

;; modcmp: 0040204C
modcmp proc
	mov.l	r12,@-r15
	mova	(004020A8),r0                                         ; @(58,pc)
	mov.l	(004020A8),r12                                       ; @(54,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov	r5,r1
	add	#40,r1
	mov.l	@(16,r1),r7
	mov	r4,r1
	add	#40,r1
	mov.l	@(16,r1),r3
	mov.l	@(56,r7),r2
	mov.l	@(56,r3),r1
	mov.l	@(52,r7),r0
	cmp/gt	r1,r2
	bt/s	004020A0
	mov.l	@(52,r3),r6

l0040206E:
	cmp/ge	r1,r2
	bf	00402078

l00402072:
	cmp/hi	r6,r0
	bt/s	004020A0
	cmp/ge	r1,r2

l00402078:
	bf/s	004020A4
	cmp/gt	r1,r2

l0040207C:
	bt/s	00402082
	cmp/hs	r6,r0

l00402080:
	bf	004020A4

l00402082:
	mov.l	@(60,r7),r2
	mov.l	@(60,r3),r1
	cmp/gt	r1,r2
	bt/s	0040209A
	mov	#01,r0

l0040208C:
	cmp/ge	r1,r2
	bf/s	0040209A
	mov	#FF,r0

l00402092:
	mov.l	(004020AC),r1                                        ; @(18,pc)
	add	#54,r5
	bsrf	r1
	add	#54,r4

l0040209A:
	lds.l	@r15+,pr
	rts
0040209E                                           F6 6C               .l

l004020A0:
	bra	0040209A
	mov	#01,r0

l004020A4:
	bra	0040209A
	mov	#FF,r0
004020A8                         38 31 01 00 7E F7 FF FF         81..~...

;; revmodcmp: 004020B0
revmodcmp proc
	mov.l	r12,@-r15
	mova	(00402114),r0                                         ; @(60,pc)
	mov.l	(00402114),r12                                       ; @(5C,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov	r5,r1
	add	#40,r1
	mov.l	@(16,r1),r7
	mov	r4,r1
	add	#40,r1
	mov.l	@(16,r1),r3
	mov	r5,r6
	mov.l	@(56,r7),r2
	mov.l	@(56,r3),r1
	mov.l	@(52,r7),r0
	cmp/gt	r1,r2
	bt/s	0040210A
	mov.l	@(52,r3),r5

l004020D4:
	cmp/ge	r1,r2
	bf	004020DE

l004020D8:
	cmp/hi	r5,r0
	bt/s	0040210A
	cmp/ge	r1,r2

l004020DE:
	bf/s	0040210E
	cmp/gt	r1,r2

l004020E2:
	bt/s	004020E8
	cmp/hs	r5,r0

l004020E6:
	bf	0040210E

l004020E8:
	mov.l	@(60,r7),r2
	mov.l	@(60,r3),r1
	cmp/gt	r1,r2
	bt/s	00402104
	mov	#FF,r0

l004020F2:
	cmp/ge	r1,r2
	bf/s	00402104
	mov	#01,r0

l004020F8:
	mov.l	(00402118),r1                                        ; @(1C,pc)
	mov	r4,r5
	mov	r6,r4
	add	#54,r5
	bsrf	r1
	add	#54,r4

l00402104:
	lds.l	@r15+,pr
	rts
00402108                         F6 6C                           .l      

l0040210A:
	bra	00402104
	mov	#FF,r0

l0040210E:
	bra	00402104
	mov	#01,r0
00402112       09 00 CC 30 01 00 14 F7 FF FF               ...0......    

;; acccmp: 0040211C
acccmp proc
	mov.l	r12,@-r15
	mova	(00402178),r0                                         ; @(58,pc)
	mov.l	(00402178),r12                                       ; @(54,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov	r5,r1
	add	#40,r1
	mov.l	@(16,r1),r7
	mov	r4,r1
	add	#40,r1
	mov.l	@(16,r1),r3
	mov.l	@(44,r7),r2
	mov.l	@(44,r3),r1
	mov.l	@(40,r7),r0
	cmp/gt	r1,r2
	bt/s	00402170
	mov.l	@(40,r3),r6

l0040213E:
	cmp/ge	r1,r2
	bf	00402148

l00402142:
	cmp/hi	r6,r0
	bt/s	00402170
	cmp/ge	r1,r2

l00402148:
	bf/s	00402174
	cmp/gt	r1,r2

l0040214C:
	bt/s	00402152
	cmp/hs	r6,r0

l00402150:
	bf	00402174

l00402152:
	mov.l	@(48,r7),r2
	mov.l	@(48,r3),r1
	cmp/gt	r1,r2
	bt/s	0040216A
	mov	#01,r0

l0040215C:
	cmp/ge	r1,r2
	bf/s	0040216A
	mov	#FF,r0

l00402162:
	mov.l	(0040217C),r1                                        ; @(18,pc)
	add	#54,r5
	bsrf	r1
	add	#54,r4

l0040216A:
	lds.l	@r15+,pr
	rts
0040216E                                           F6 6C               .l

l00402170:
	bra	0040216A
	mov	#01,r0

l00402174:
	bra	0040216A
	mov	#FF,r0
00402178                         68 30 01 00 AE F6 FF FF         h0......

;; revacccmp: 00402180
revacccmp proc
	mov.l	r12,@-r15
	mova	(004021E4),r0                                         ; @(60,pc)
	mov.l	(004021E4),r12                                       ; @(5C,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov	r5,r1
	add	#40,r1
	mov.l	@(16,r1),r7
	mov	r4,r1
	add	#40,r1
	mov.l	@(16,r1),r3
	mov	r5,r6
	mov.l	@(44,r7),r2
	mov.l	@(44,r3),r1
	mov.l	@(40,r7),r0
	cmp/gt	r1,r2
	bt/s	004021DA
	mov.l	@(40,r3),r5

l004021A4:
	cmp/ge	r1,r2
	bf	004021AE

l004021A8:
	cmp/hi	r5,r0
	bt/s	004021DA
	cmp/ge	r1,r2

l004021AE:
	bf/s	004021DE
	cmp/gt	r1,r2

l004021B2:
	bt/s	004021B8
	cmp/hs	r5,r0

l004021B6:
	bf	004021DE

l004021B8:
	mov.l	@(48,r7),r2
	mov.l	@(48,r3),r1
	cmp/gt	r1,r2
	bt/s	004021D4
	mov	#FF,r0

l004021C2:
	cmp/ge	r1,r2
	bf/s	004021D4
	mov	#01,r0

l004021C8:
	mov.l	(004021E8),r1                                        ; @(1C,pc)
	mov	r4,r5
	mov	r6,r4
	add	#54,r5
	bsrf	r1
	add	#54,r4

l004021D4:
	lds.l	@r15+,pr
	rts
004021D8                         F6 6C                           .l      

l004021DA:
	bra	004021D4
	mov	#FF,r0

l004021DE:
	bra	004021D4
	mov	#01,r0
004021E2       09 00 FC 2F 01 00 44 F6 FF FF               .../..D...    

;; statcmp: 004021EC
statcmp proc
	mov.l	r12,@-r15
	mova	(0040224C),r0                                         ; @(5C,pc)
	mov.l	(0040224C),r12                                       ; @(58,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov	r5,r1
	add	#40,r1
	mov.l	@(16,r1),r2
	mov	r4,r1
	add	#40,r1
	mov.l	@(16,r1),r1
	add	#40,r2
	mov.l	@(4,r2),r7
	add	#40,r1
	mov.l	@(4,r1),r3
	mov.l	@(0,r2),r0
	cmp/gt	r3,r7
	bt/s	00402244
	mov.l	@(0,r1),r6

l00402212:
	cmp/ge	r3,r7
	bf	0040221C

l00402216:
	cmp/hi	r6,r0
	bt/s	00402244
	cmp/ge	r3,r7

l0040221C:
	bf/s	00402248
	cmp/gt	r3,r7

l00402220:
	bt/s	00402226
	cmp/hs	r6,r0

l00402224:
	bf	00402248

l00402226:
	mov.l	@(8,r2),r2
	mov.l	@(8,r1),r1
	cmp/gt	r1,r2
	bt/s	0040223E
	mov	#01,r0

l00402230:
	cmp/ge	r1,r2
	bf/s	0040223E
	mov	#FF,r0

l00402236:
	mov.l	(00402250),r1                                        ; @(18,pc)
	add	#54,r5
	bsrf	r1
	add	#54,r4

l0040223E:
	lds.l	@r15+,pr
	rts
00402242       F6 6C                                       .l            

l00402244:
	bra	0040223E
	mov	#01,r0

l00402248:
	bra	0040223E
	mov	#FF,r0
0040224C                                     94 2F 01 00             ./..
00402250 DA F5 FF FF                                     ....            

;; revstatcmp: 00402254
revstatcmp proc
	mov.l	r12,@-r15
	mova	(004022BC),r0                                         ; @(64,pc)
	mov.l	(004022BC),r12                                       ; @(60,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov	r5,r1
	add	#40,r1
	mov.l	@(16,r1),r2
	mov	r4,r1
	add	#40,r1
	mov.l	@(16,r1),r1
	add	#40,r2
	mov.l	@(4,r2),r7
	add	#40,r1
	mov.l	@(4,r1),r3
	mov	r5,r6
	mov.l	@(0,r2),r0
	cmp/gt	r3,r7
	bt/s	004022B2
	mov.l	@(0,r1),r5

l0040227C:
	cmp/ge	r3,r7
	bf	00402286

l00402280:
	cmp/hi	r5,r0
	bt/s	004022B2
	cmp/ge	r3,r7

l00402286:
	bf/s	004022B6
	cmp/gt	r3,r7

l0040228A:
	bt/s	00402290
	cmp/hs	r5,r0

l0040228E:
	bf	004022B6

l00402290:
	mov.l	@(8,r2),r2
	mov.l	@(8,r1),r1
	cmp/gt	r1,r2
	bt/s	004022AC
	mov	#FF,r0

l0040229A:
	cmp/ge	r1,r2
	bf/s	004022AC
	mov	#01,r0

l004022A0:
	mov.l	(004022C0),r1                                        ; @(1C,pc)
	mov	r4,r5
	mov	r6,r4
	add	#54,r5
	bsrf	r1
	add	#54,r4

l004022AC:
	lds.l	@r15+,pr
	rts
004022B0 F6 6C                                           .l              

l004022B2:
	bra	004022AC
	mov	#FF,r0

l004022B6:
	bra	004022AC
	mov	#01,r0
004022BA                               09 00 24 2F 01 00           ..$/..
004022C0 6C F5 FF FF                                     l...            

;; sizecmp: 004022C4
sizecmp proc
	mov.l	r12,@-r15
	mova	(00402314),r0                                         ; @(4C,pc)
	mov.l	(00402314),r12                                       ; @(48,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov	r5,r1
	add	#40,r1
	mov.l	@(16,r1),r1
	add	#40,r1
	mov.l	@(28,r1),r2
	mov.l	@(24,r1),r7
	mov	r4,r1
	add	#40,r1
	mov.l	@(16,r1),r1
	add	#40,r1
	mov.l	@(24,r1),r3
	mov.l	@(28,r1),r1
	cmp/gt	r1,r2
	bt/s	00402310
	cmp/ge	r1,r2

l004022EC:
	bf	004022F6

l004022EE:
	cmp/hi	r3,r7
	bt/s	0040230A
	mov	#01,r0

l004022F4:
	cmp/ge	r1,r2

l004022F6:
	bf/s	0040230A
	mov	#FF,r0

l004022FA:
	cmp/gt	r1,r2
	bt/s	00402302
	cmp/hs	r3,r7

l00402300:
	bf	0040230A

l00402302:
	mov.l	(00402318),r1                                        ; @(14,pc)
	add	#54,r5
	bsrf	r1
	add	#54,r4

l0040230A:
	lds.l	@r15+,pr
	rts
0040230E                                           F6 6C               .l

l00402310:
	bra	0040230A
	mov	#01,r0
00402314             CC 2E 01 00 0E F5 FF FF                 ........    

;; revsizecmp: 0040231C
revsizecmp proc
	mov.l	r12,@-r15
	mova	(00402374),r0                                         ; @(54,pc)
	mov.l	(00402374),r12                                       ; @(50,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov	r5,r1
	add	#40,r1
	mov.l	@(16,r1),r1
	add	#40,r1
	mov.l	@(28,r1),r2
	mov.l	@(24,r1),r6
	mov	r4,r1
	add	#40,r1
	mov.l	@(16,r1),r1
	add	#40,r1
	mov.l	@(24,r1),r7
	mov.l	@(28,r1),r1
	cmp/gt	r1,r2
	bt/s	0040236E
	mov	r5,r3

l00402344:
	cmp/ge	r1,r2
	bf	00402350

l00402348:
	cmp/hi	r7,r6
	bt/s	00402368
	mov	#FF,r0

l0040234E:
	cmp/ge	r1,r2

l00402350:
	bf/s	00402368
	mov	#01,r0

l00402354:
	cmp/gt	r1,r2
	bt/s	0040235C
	cmp/hs	r7,r6

l0040235A:
	bf	00402368

l0040235C:
	mov.l	(00402378),r1                                        ; @(18,pc)
	mov	r4,r5
	mov	r3,r4
	add	#54,r5
	bsrf	r1
	add	#54,r4

l00402368:
	lds.l	@r15+,pr
	rts
0040236C                                     F6 6C                   .l  

l0040236E:
	bra	00402368
	mov	#FF,r0
00402372       09 00 6C 2E 01 00 B0 F4 FF FF               ..l.......    

;; mastercmp: 0040237C
mastercmp proc
	mov.l	r12,@-r15
	mova	(00402408),r0                                         ; @(88,pc)
	mov.l	(00402408),r12                                       ; @(84,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov.l	@r4,r4
	mov	r4,r2
	add	#40,r2
	mov.w	@(8,r2),r0
	extu.w	r0,r7
	mov	r7,r0
	cmp/eq	#07,r0
	bt	00402400

l00402396:
	mov.l	@r5,r5
	mov	r5,r1
	add	#40,r1
	mov.w	@(8,r1),r0
	extu.w	r0,r1
	mov	r1,r0
	cmp/eq	#07,r0
	bt	00402400

l004023A6:
	mov	r7,r0
	cmp/eq	#0A,r0
	bt/s	004023B4
	mov	r1,r0

l004023AE:
	cmp/eq	#0A,r0
	bf/s	004023D2
	cmp/eq	r1,r7

l004023B4:
	mov	r1,r0
	cmp/eq	#0A,r0
	bf	00402404

l004023BA:
	mov	r7,r0
	cmp/eq	#0A,r0
	bf/s	004023CA
	mov	#FF,r7

l004023C2:
	mov.l	(0040240C),r1                                        ; @(48,pc)
	bsrf	r1
	nop
	mov	r0,r7

l004023CA:
	mov	r7,r0

l004023CC:
	lds.l	@r15+,pr
	rts
004023D0 F6 6C                                           .l              

l004023D2:
	bt	004023F4

l004023D4:
	mov.l	(00402410),r3                                        ; @(38,pc)
	mov	r12,r0
	mov.l	@(r0,r3),r3
	mov.l	@r3,r3
	tst	r3,r3
	bf	004023F4

l004023E0:
	mov.l	@(4,r2),r2
	tst	r2,r2
	bf	004023F4

l004023E6:
	mov	r7,r0
	cmp/eq	#01,r0
	bt	004023CC

l004023EC:
	mov	r1,r0
	cmp/eq	#01,r0
	bt/s	004023CA
	mov	#FF,r7

l004023F4:
	mov.l	(00402414),r0                                        ; @(1C,pc)
	mov.l	@(r0,r12),r0
	jsr	@r0
	nop
	bra	004023CA
	mov	r0,r7

l00402400:
	bra	004023CA
	mov	#00,r7

l00402404:
	bra	004023CA
	mov	#01,r7
00402408                         D8 2D 01 00 3C FC FF FF         .-..<...
00402410 F4 00 00 00 F4 01 00 00                         ........        

;; display: 00402418
display proc
	mov.l	r8,@-r15
	mova	(00402600),r0                                         ; @(1E4,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	r13,@-r15
	mov.l	r14,@-r15
	mov.l	(00402600),r12                                       ; @(1D4,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#94,r15
	add	#94,r15
	mov	#60,r0
	mov.l	r4,@(r0,r15)
	tst	r5,r5
	bt/s	004024B0
	mov	r5,r14

l0040243C:
	mov.l	(00402604),r0                                        ; @(1C4,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov	#01,r1
	bf/s	00402464
	mov.l	r1,@(40,r15)

l0040244A:
	mov.l	(00402608),r0                                        ; @(1BC,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf/s	00402466
	mov	#40,r1

l00402456:
	mov.l	(0040260C),r0                                        ; @(1B4,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov	#FF,r1
	negc	r1,r1
	mov.l	r1,@(40,r15)

l00402464:
	mov	#40,r1

l00402466:
	mov.l	(00402610),r4                                        ; @(1A8,pc)
	add	r15,r1
	mov	#00,r0
	mov	#00,r2
	mov	#00,r3
	mov	#00,r10
	mov.l	r0,@(16,r1)
	add	r12,r4
	mov.l	r0,@(12,r1)
	mov	r14,r8
	mov.l	r0,@(8,r1)
	mov.l	r0,@(4,r1)
	mov.l	r0,@(36,r15)
	mov	#64,r0
	mov.l	r2,@(0,r1)
	mov.l	r3,@(24,r1)
	mov.l	r10,@(20,r1)
	mov	#00,r1
	mov.l	r2,@(28,r15)
	mov.l	r3,@(24,r15)
	mov.l	r2,@(56,r15)
	mov.l	r3,@(60,r15)
	mov.l	r2,@(48,r15)
	mov.l	r3,@(52,r15)
	mov.l	r2,@(44,r15)
	mov.l	r3,@(16,r15)
	mov.l	r1,@(20,r15)
	mov.l	r2,@(8,r15)
	mov.l	r3,@(12,r15)
	mov.l	r4,@(r0,r15)

l004024A2:
	tst	r8,r8
	bt	004024AA

l004024A6:
	bra	004025C2
	mov	r8,r0

l004024AA:
	mov.l	@(28,r15),r2
	tst	r2,r2
	bf	004024B4

l004024B0:
	bra	0040298C
	nop

l004024B4:
	mov.w	(004025FA),r9                                        ; @(142,pc)
	mov	#4C,r0
	mov.l	@(r0,r15),r3
	add	r15,r9
	mov.l	@(40,r15),r1
	mov.l	r14,@r9
	tst	r1,r1
	mov.l	r2,@(20,r9)
	bt/s	00402598
	mov.l	r3,@(24,r9)

l004024C8:
	mov.l	@(16,r15),r1
	mov.l	(00402614),r0                                        ; @(148,pc)
	mov.l	@(8,r15),r2
	mov.l	r1,@(12,r9)
	mov.l	@(r0,r12),r1
	mov.l	r2,@(4,r9)
	mov.l	@(12,r15),r3
	mov.l	@(20,r15),r2
	mov.l	@r1,r1
	mov.l	r3,@(8,r9)
	tst	r1,r1
	bf/s	004024E6
	mov.l	r2,@(16,r9)

l004024E2:
	bra	004028DC
	mov.l	@(52,r15),r3

l004024E6:
	mov	#04,r1
	mov.l	r1,@(28,r9)
	mov	#44,r0

l004024EC:
	mov.w	(004025FA),r9                                        ; @(10A,pc)
	mov	#15,r5
	mov.l	@(r0,r15),r2
	mov	#48,r0
	mov.l	@(r0,r15),r3
	add	r15,r9
	mov.l	r2,@(32,r9)
	mov	#58,r0
	mov.l	@(48,r15),r2
	mov.w	(004025FC),r8                                        ; @(FA,pc)
	mov.l	(00402618),r1                                        ; @(114,pc)
	add	r15,r8
	mov.l	r3,@(36,r9)
	mov	r8,r4
	mov.l	(0040261C),r6                                        ; @(110,pc)
	mov.l	r2,@r15
	add	r12,r6
	bsrf	r1
	mov.l	@(r0,r15),r7
	mov.l	(00402620),r2                                        ; @(10C,pc)
	bsrf	r2
	mov	r8,r4
	mov.l	(00402624),r3                                        ; @(108,pc)
	mov	#15,r5
	mov.l	(00402628),r6                                        ; @(108,pc)
	mov	r8,r4
	mov.l	@(56,r15),r7
	add	r12,r6
	bsrf	r3
	mov.l	r0,@(40,r9)
	mov.l	(0040262C),r5                                        ; @(100,pc)
	bsrf	r5
	mov	r8,r4
	mov.l	r0,@(44,r9)
	mov.l	(00402614),r0                                        ; @(E0,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r10
	tst	r10,r10
	bf/s	00402540
	mov	#04,r1

l0040253C:
	bra	0040293C
	mov.l	@(44,r15),r2

l00402540:
	mov.l	r1,@(48,r9)
	mov	#50,r0

l00402544:
	mov.l	@(r0,r15),r2
	mov.l	@(24,r15),r3
	tst	r3,r3
	bf/s	00402552
	mov.l	r2,@(52,r9)

l0040254E:
	bra	00402986
	mov.l	@(24,r15),r3

l00402552:
	mov.l	(00402630),r10                                       ; @(DC,pc)
	mov	#15,r5
	mov.l	(00402634),r1                                        ; @(DC,pc)
	add	r12,r10
	mov.l	@(60,r15),r7
	mov	r10,r6
	bsrf	r1
	mov	r8,r4
	mov.l	(00402638),r2                                        ; @(D4,pc)
	bsrf	r2
	mov	r8,r4
	mov.l	(0040263C),r1                                        ; @(D0,pc)
	mov	r10,r6
	mov.l	r0,@(56,r9)
	mov	#40,r0
	mov.l	@(r0,r15),r7
	mov	#15,r5
	bsrf	r1
	mov	r8,r4
	mov.l	(00402640),r2                                        ; @(C4,pc)
	bsrf	r2
	mov	r8,r4
	mov.l	@(56,r9),r3
	mov	r0,r2
	mov.l	@(48,r9),r1
	add	r3,r2
	mov	r2,r7
	add	#01,r7
	cmp/ge	r1,r7
	bt/s	00402594
	mov.l	r0,@(60,r9)

l00402590:
	bra	0040297A
	sub	r0,r1

l00402594:
	add	#02,r2
	mov.l	r2,@(48,r9)

l00402598:
	mov.l	(00402644),r0                                        ; @(A8,pc)
	mov.w	(004025FA),r4                                        ; @(5C,pc)
	mov.l	@(r0,r12),r1
	jsr	@r1
	add	r15,r4
	mov.l	(00402648),r0                                        ; @(A4,pc)
	mov	#01,r1
	mov.l	r1,@(r0,r12)
	mov.l	(00402608),r0                                        ; @(5C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1

l004025B0:
	bf	004025B6

l004025B2:
	bra	0040298C
	nop

l004025B6:
	mov.l	(0040264C),r7                                        ; @(94,pc)
	bsrf	r7
	mov.l	@(20,r14),r4
	mov.l	@(8,r14),r14
	bra	004025B0
	tst	r14,r14

l004025C2:
	add	#40,r0
	mov.w	@(8,r0),r0
	extu.w	r0,r2
	mov	r2,r0
	cmp/eq	#07,r0
	bt/s	004025D4
	cmp/eq	#0A,r0

l004025D0:
	bf/s	0040265C
	mov	#60,r0

l004025D4:
	mov.l	(00402650),r1                                        ; @(78,pc)
	bsrf	r1
	mov.l	@(32,r8),r4
	mov.l	(00402654),r7                                        ; @(78,pc)
	mov	r0,r6
	mov	r8,r5
	mov	#64,r0
	mov.l	@(r0,r15),r4
	bsrf	r7
	add	#54,r5
	mov.l	(00402658),r0                                        ; @(6C,pc)
	mov	#01,r2
	mov	#00,r3
	mov	#01,r1
	mov.l	r2,@(12,r8)
	mov.l	r3,@(16,r8)
	mov.l	r1,@(r0,r12)

l004025F6:
	bra	004024A2
	mov.l	@(8,r8),r8
004025FA                               98 00 80 00 09 00           ......
00402600 E0 2B 01 00 40 01 00 00 74 01 00 00 60 01 00 00 .+..@...t...`...
00402610 A0 F3 FE FF B8 01 00 00 CE F2 FF FF B8 F3 FE FF ................
00402620 EC F7 FF FF B8 F2 FF FF A8 F3 FE FF D6 F7 FF FF ................
00402630 C0 F3 FE FF 7E F2 FF FF 9C F7 FF FF 68 F2 FF FF ....~.......h...
00402640 86 F7 FF FF F8 01 00 00 F0 01 00 00 30 F6 FF FF ............0...
00402650 82 F6 FF FF 48 F3 FF FF E8 01 00 00             ....H.......    

l0040265C:
	mov.l	@(r0,r15),r0
	tst	r0,r0
	bt	00402666

l00402662:
	bra	00402806
	mov	r8,r0

l00402666:
	mov	r2,r0
	cmp/eq	#01,r0
	mov.l	(0040284C),r0                                        ; @(1E0,pc)
	bf	00402672

l0040266E:
	bra	00402812
	mov.l	@(r0,r12),r1

l00402672:
	mov	#4C,r0

l00402674:
	mov.l	@(44,r8),r1
	mov.l	@(r0,r15),r2
	cmp/hs	r1,r2
	bt	0040267E

l0040267C:
	mov.l	r1,@(r0,r15)

l0040267E:
	mov.l	@(40,r15),r1
	tst	r1,r1
	bt	00402748

l00402684:
	mov	r8,r1
	add	#40,r1
	mov.l	@(16,r1),r9
	mov	r9,r1
	add	#40,r1
	mov.l	@(36,r1),r3
	cmp/gt	r3,r10
	bt/s	004026A4
	mov.l	@(32,r1),r6

l00402696:
	cmp/ge	r3,r10
	bf	004026A0

l0040269A:
	mov.l	@(52,r15),r2
	cmp/hs	r6,r2
	bt	004026A4

l004026A0:
	mov.l	r6,@(52,r15)
	mov	r3,r10

l004026A4:
	mov.l	@(16,r9),r1
	mov.l	@(48,r15),r4
	cmp/hi	r1,r4
	bt/s	004026BE
	mov.l	@(12,r9),r2

l004026AE:
	cmp/hs	r1,r4
	bf/s	004026BA
	mov	#58,r0

l004026B4:
	mov.l	@(r0,r15),r5
	cmp/hs	r2,r5
	bt	004026BE

l004026BA:
	mov.l	r2,@(r0,r15)
	mov.l	r1,@(48,r15)

l004026BE:
	mov.l	@(20,r9),r1
	mov.l	@(56,r15),r2
	cmp/hs	r1,r2
	bt	004026C8

l004026C6:
	mov.l	r1,@(56,r15)

l004026C8:
	mov	r9,r1
	add	#40,r1
	mov.l	@(28,r1),r2
	mov.l	@(44,r15),r4
	cmp/gt	r2,r4
	bt/s	004026E6
	mov.l	@(24,r1),r7

l004026D6:
	cmp/ge	r2,r4
	bf/s	004026E2
	mov	#54,r0

l004026DC:
	mov.l	@(r0,r15),r5
	cmp/hs	r7,r5
	bt	004026E6

l004026E2:
	mov.l	r7,@(r0,r15)
	mov.l	r2,@(44,r15)

l004026E6:
	mov.l	@(8,r9),r1
	mov.l	(00402850),r4                                        ; @(164,pc)
	mov.w	(00402846),r5                                        ; @(158,pc)
	and	r4,r1
	cmp/eq	r5,r1
	bf/s	00402724
	clrt

l004026F4:
	mov.l	@(32,r9),r5
	mov.w	(00402848),r4                                        ; @(14E,pc)
	mov	r5,r1
	mov.l	@(60,r15),r0
	shlr8	r1
	and	r4,r1
	cmp/ge	r1,r0
	bt	00402706

l00402704:
	mov.l	r1,@(60,r15)

l00402706:
	mov	r5,r1
	mov	#F4,r4
	shld	r4,r1
	mov.l	(00402854),r4                                        ; @(144,pc)
	extu.b	r5,r5
	mov	#40,r0
	and	r4,r1
	or	r5,r1
	mov.l	@(r0,r15),r5
	cmp/ge	r1,r5
	bt	0040271E

l0040271C:
	mov.l	r1,@(r0,r15)

l0040271E:
	mov.l	@(40,r15),r1
	mov.l	r1,@(24,r15)
	clrt

l00402724:
	mov.l	@(8,r15),r1
	mov.l	(00402858),r0                                        ; @(130,pc)
	addc	r6,r1
	mov.l	r1,@(8,r15)
	mov.l	@(12,r15),r1
	addc	r3,r1
	mov.l	@(16,r15),r3
	clrt
	mov.l	r1,@(12,r15)
	addc	r7,r3
	mov.l	@(20,r15),r1
	mov.l	r3,@(16,r15)
	addc	r2,r1
	mov.l	r1,@(20,r15)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	0040274C

l00402748:
	bra	004028D6
	mov.l	@(28,r15),r1

l0040274C:
	mov.l	(0040285C),r0                                        ; @(10C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00402826
	mov	#00,r5

l00402758:
	mov.l	(00402860),r6                                        ; @(104,pc)
	mov	r15,r13
	mov.l	(00402864),r3                                        ; @(104,pc)
	add	#68,r13
	mov.l	@(24,r9),r7
	add	r12,r6
	mov	#0C,r5
	bsrf	r3
	mov	r13,r4

l0040276A:
	mov.l	(0040285C),r0                                        ; @(F0,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00402836
	mov	#00,r5

l00402776:
	mov.l	(00402860),r6                                        ; @(E8,pc)
	mov	r15,r11
	mov.l	(00402868),r1                                        ; @(EC,pc)
	add	#74,r11
	mov.l	@(28,r9),r7
	add	r12,r6
	mov	#0C,r5
	bsrf	r1
	mov	r11,r4
	mov.l	r11,@(32,r15)

l0040278A:
	mov.l	(0040286C),r3                                        ; @(E0,pc)
	bsrf	r3
	mov	r13,r4
	mov	r0,r11
	mov	#50,r0
	mov.l	@(r0,r15),r4
	cmp/ge	r11,r4
	bt	0040279C

l0040279A:
	mov.l	r11,@(r0,r15)

l0040279C:
	mov.l	(00402870),r7                                        ; @(D0,pc)
	bsrf	r7
	mov.l	@(32,r15),r4
	mov	r0,r1
	mov	#48,r0
	mov.l	@(r0,r15),r0
	cmp/ge	r1,r0
	bt/s	004027B0
	mov	#48,r0

l004027AE:
	mov.l	r1,@(r0,r15)

l004027B0:
	mov.l	(00402874),r0                                        ; @(C0,pc)
	mov.l	@(r0,r12),r2
	mov.l	@r2,r0
	tst	r0,r0
	bt/s	004027E6
	mov	r11,r3

l004027BC:
	mov.l	(00402878),r3                                        ; @(B8,pc)
	add	#40,r9
	mov.l	(0040287C),r5                                        ; @(B8,pc)
	mov.l	@(44,r9),r4
	add	r12,r5
	bsrf	r3
	mov.l	r1,@(4,r15)
	mov.l	(00402880),r5                                        ; @(B4,pc)
	mov	r0,r4
	bsrf	r5
	mov.l	r0,@(36,r15)
	mov	#40,r7
	add	r15,r7
	mov.l	@(4,r7),r7
	cmp/ge	r0,r7
	bt/s	004027E4
	mov.l	@(4,r15),r1

l004027DE:
	mov	#40,r2
	add	r15,r2
	mov.l	r0,@(4,r2)

l004027E4:
	mov	r11,r3

l004027E6:
	mov	#40,r4
	add	r1,r3
	add	r15,r4
	mov.l	(00402884),r7                                        ; @(94,pc)
	mov.l	r3,@(28,r4)
	mov	r3,r4
	add	#12,r4
	bsrf	r7
	add	r0,r4
	tst	r0,r0
	bf/s	00402898
	mov	r0,r9

l004027FE:
	mov.l	(00402888),r0                                        ; @(88,pc)
	mov	#00,r5
	bsrf	r0
	mov	#01,r4

l00402806:
	add	#50,r0
	mov.b	@(4,r0),r0
	cmp/eq	#2E,r0
	bf	00402818

l0040280E:
	mov.l	(0040288C),r0                                        ; @(7C,pc)
	mov.l	@(r0,r12),r1

l00402812:
	mov.l	@r1,r1
	tst	r1,r1
	bt	0040281C

l00402818:
	bra	00402674
	mov	#4C,r0

l0040281C:
	mov	#01,r2
	mov	#00,r3
	mov.l	r2,@(12,r8)
	bra	004025F6
	mov.l	r3,@(16,r8)

l00402826:
	mov.l	(00402890),r7                                        ; @(68,pc)
	bsrf	r7
	mov.l	@(24,r9),r4
	tst	r0,r0
	bf/s	0040276A
	mov	r0,r13

l00402832:
	bra	00402758
	nop

l00402836:
	mov.l	(00402894),r2                                        ; @(5C,pc)
	bsrf	r2
	mov.l	@(28,r9),r4
	tst	r0,r0
	bf/s	0040278A
	mov.l	r0,@(32,r15)

l00402842:
	bra	00402776
	nop
00402846                   00 20 FF 0F 09 00 F4 00 00 00       . ........
00402850 00 B0 00 00 00 FF 0F 00 74 01 00 00 18 01 00 00 ........t.......
00402860 A8 F3 FE FF 76 F0 FF FF 58 F0 FF FF 74 F5 FF FF ....v...X...t...
00402870 62 F5 FF FF 1C 01 00 00 B6 F2 FF FF AC F3 FE FF b...............
00402880 32 F5 FF FF 74 F0 FF FF 5E F2 FF FF 5C 01 00 00 2...t...^...\...
00402890 98 EF FF FF D0 F2 FF FF                         ........        

l00402898:
	mov.l	(004029A4),r1                                        ; @(108,pc)
	mov	r0,r4
	add	#0C,r4
	mov.l	r4,@r0
	bsrf	r1
	mov	r13,r5
	mov	r11,r4
	mov.l	(004029A8),r3                                        ; @(100,pc)
	add	#0D,r4
	add	r9,r4
	mov.l	r4,@(4,r9)
	bsrf	r3
	mov.l	@(32,r15),r5
	mov.l	(004029AC),r0                                        ; @(F8,pc)
	mov.l	@(r0,r12),r2
	mov.l	@r2,r2
	tst	r2,r2
	bt/s	004028D2
	mov	#5C,r0

l004028BE:
	mov.l	@(r0,r15),r4
	mov.l	(004029B0),r7                                        ; @(EC,pc)
	add	#0E,r4
	add	r9,r4
	mov.l	r4,@(8,r9)
	bsrf	r7
	mov.l	@(36,r15),r5
	mov.l	(004029B4),r0                                        ; @(E4,pc)
	bsrf	r0
	mov.l	@(36,r15),r4

l004028D2:
	mov.l	r9,@(20,r8)
	mov.l	@(28,r15),r1

l004028D6:
	add	#01,r1
	bra	004025F6
	mov.l	r1,@(28,r15)

l004028DC:
	mov.l	(004029B8),r0                                        ; @(D8,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	mov	r1,r2
	add	#FF,r2
	cmp/gt	r2,r8
	mov	r2,r4
	subc	r5,r5
	mov.l	(004029BC),r2                                        ; @(CC,pc)
	cmp/gt	r1,r8
	subc	r7,r7
	clrt
	addc	r3,r4
	addc	r10,r5
	mov.w	(004029A2),r10                                       ; @(A6,pc)
	bsrf	r2
	mov	r1,r6
	mov.l	r1,@r15
	add	r15,r10
	mov.l	(004029C0),r1                                        ; @(BC,pc)
	mov	r0,r7
	mov.l	(004029C4),r6                                        ; @(BC,pc)
	mov	#15,r5
	mov	r10,r4
	bsrf	r1
	add	r12,r6
	mov.l	(004029C8),r2                                        ; @(B4,pc)
	bsrf	r2
	mov	r10,r4
	mov.l	(004029CC),r1                                        ; @(B4,pc)
	mov	r0,r3
	mov.l	r0,@(28,r9)
	mov	r12,r0
	mov.l	@(r0,r1),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00402938
	mov	r3,r2

l00402928:
	mov.l	(004029D0),r1                                        ; @(A4,pc)
	add	#FF,r2
	dmuls.l	r1,r2
	sts	mach,r1
	cmp/gt	r2,r8
	mov	r3,r0
	addc	r1,r0
	mov.l	r0,@(28,r9)

l00402938:
	bra	004024EC
	mov	#44,r0

l0040293C:
	mov	#54,r0
	mov.l	(004029D4),r1                                        ; @(94,pc)
	mov	#15,r5
	mov.l	(004029C4),r6                                        ; @(80,pc)
	mov	r8,r4
	mov.l	r2,@r15
	add	r12,r6
	bsrf	r1
	mov.l	@(r0,r15),r7
	mov.l	(004029D8),r2                                        ; @(88,pc)
	bsrf	r2
	mov	r8,r4
	mov.l	(004029CC),r1                                        ; @(74,pc)
	mov	r0,r3
	mov.l	r0,@(48,r9)
	mov	r12,r0
	mov.l	@(r0,r1),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00402976
	mov	r3,r2

l00402966:
	mov.l	(004029D0),r1                                        ; @(68,pc)
	add	#FF,r2
	dmuls.l	r1,r2
	sts	mach,r1
	cmp/gt	r2,r10
	mov	r3,r0
	addc	r1,r0
	mov.l	r0,@(48,r9)

l00402976:
	bra	00402544
	mov	#50,r0

l0040297A:
	add	#FE,r1
	cmp/ge	r1,r3
	bt	00402982

l00402980:
	mov.l	r1,@(56,r9)

l00402982:
	bra	00402598
	nop

l00402986:
	mov.l	r3,@(56,r9)
	bra	00402598
	mov.l	r3,@(60,r9)

l0040298C:
	add	#6C,r15
	add	#6C,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r14
	mov.l	@r15+,r13
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	rts
004029A0 F6 68 80 00 1C F0 FF FF 0E F0 FF FF 1C 01 00 00 .h..............
004029B0 F4 EF FF FF 1A F3 FF FF 3C 01 00 00 B6 F2 FF FF ........<.......
004029C0 D0 EE FF FF B0 F3 FE FF EE F3 FF FF 38 01 00 00 ............8...
004029D0 56 55 55 55 92 EE FF FF B0 F3 FF FF             VUUU........    

;; traverse: 004029DC
traverse proc
	mov.l	r8,@-r15
	mova	(00402B7C),r0                                         ; @(19C,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	r13,@-r15
	mov.l	r14,@-r15
	mov.l	(00402B7C),r12                                       ; @(18C,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#F8,r15
	mov.l	(00402B80),r0                                        ; @(188,pc)
	mov	r6,r10
	mov.l	r4,@(4,r15)
	mov	#00,r6
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf/s	00402A0A
	mov	r5,r4

l00402A06:
	mov.l	(00402B84),r6                                        ; @(17C,pc)
	add	r12,r6

l00402A0A:
	mov.l	(00402B88),r1                                        ; @(17C,pc)
	bsrf	r1
	mov	r10,r5
	tst	r0,r0
	mov	r0,r8
	bf/s	00402A1E
	mov	#00,r5

l00402A18:
	mov.l	(00402B8C),r0                                        ; @(170,pc)
	bsrf	r0
	mov	#01,r4

l00402A1E:
	mov.l	(00402B90),r1                                        ; @(170,pc)
	bsrf	r1
	mov	r0,r4
	mov	r0,r5
	mov.l	(00402B94),r0                                        ; @(16C,pc)
	bsrf	r0
	mov	#00,r4
	mov.l	(00402B98),r0                                        ; @(168,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r9
	tst	r9,r9
	bt/s	00402A52
	mov	r12,r0

l00402A38:
	mov.l	(00402B9C),r1                                        ; @(160,pc)
	bsrf	r1
	mov	r8,r4

l00402A3E:
	add	#08,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r14
	mov.l	@r15+,r13
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	rts
00402A50 F6 68                                           .h              

l00402A52:
	mov.l	(00402BA0),r11                                       ; @(14C,pc)
	mov.l	@(r0,r11),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf/s	00402A68
	mov	r10,r0

l00402A5E:
	mov	#08,r9
	tst	#08,r0
	bt/s	00402A68
	and	r10,r9

l00402A66:
	mov.w	(00402B78),r9                                        ; @(10E,pc)

l00402A68:
	mov.l	(00402BA4),r10                                       ; @(138,pc)
	mov.l	(00402BA8),r14                                       ; @(13C,pc)
	add	r12,r10

l00402A6E:
	mov.l	(00402BAC),r1                                        ; @(13C,pc)
	bsrf	r1
	mov	r8,r4
	tst	r0,r0
	bf/s	00402AA4
	mov	r0,r13

l00402A7A:
	mov.l	(00402BB0),r1                                        ; @(134,pc)
	bsrf	r1
	nop
	mov.l	(00402BB4),r1                                        ; @(130,pc)
	mov	r8,r4
	bsrf	r1
	mov.l	@r0,r9
	mov.l	(00402BB8),r1                                        ; @(12C,pc)
	bsrf	r1
	nop
	mov.l	(00402BBC),r1                                        ; @(12C,pc)
	bsrf	r1
	mov.l	r9,@r0
	mov.l	@r0,r1
	tst	r1,r1
	bt	00402A3E

l00402A9A:
	mov.l	(00402BC0),r5                                        ; @(124,pc)
	mov	#01,r4
	mov.l	(00402BC4),r0                                        ; @(124,pc)
	bsrf	r0
	add	r12,r5

l00402AA4:
	mov	r13,r3
	add	#40,r3
	mov.w	@(8,r3),r0
	mov	#06,r7
	extu.w	r0,r2
	add	#FF,r2
	cmp/hi	r7,r2
	bt	00402A6E

l00402AB4:
	mova	(00402AC0),r0                                         ; @(08,pc)
	add	r2,r2
	mov.w	@(r0,r2),r2
	braf	r2
	nop
00402ABE                                           09 00               ..
00402AC0 3C 00 10 00 B0 FF 20 00 B0 FF B0 FF 20 00 3E D0 <..... ..... .>.
00402AD0 D3 65 3E D4 54 75 03 00 CC 34 C8 AF 09 00 3C D1 .e>.Tu...4....<.
00402AE0 03 01 D8 54 3B D1 D3 65 3B D4 03 66 54 75 03 01 ...T;..e;..fTu..
00402AF0 CC 34 3A D0 01 E1 BA AF 16 0C 31 52 28 22 09 8D .4:.......1R("..
00402B00 D3 60 50 70 04 84 2E 88 04 8B 35 D0 CE 02 22 62 .`Pp......5..."b
00402B10 28 22 AC 89 33 D0 CE 02 22 62 28 22 08 8B A2 62 ("..3..."b("...b
00402B20 28 22 1D 8D F1 51 30 D4 30 D1 CC 34 03 01 D7 55 ("...Q0.0..4...U
00402B30 2F D1 93 65 03 01 83 64 03 65 02 2F 2D D0 03 00 /..e...d.e./-...
00402B40 D3 64 C3 60 BE 02 22 62 28 22 90 8F F2 63 38 23 .d.`.."b("...c8#
00402B50 8D 8D 04 E6 28 D1 D3 65 03 01 83 64 87 AF 09 00 ....(..e...d....
00402B60 01 E2 27 31 E4 8F E3 64 24 D1 D7 55 CC 34 03 01 ..'1...d$..U.4..
00402B70 22 2F F2 62 DC AF 22 2A 00 01 09 00 64 26 01 00 "/.b.."*....d&..
00402B80 84 01 00 00 9C D1 FE FF BC F2 FF FF 46 F0 FF FF ............F...
00402B90 54 F2 FF FF EC F9 FF FF F4 00 00 00 FE F2 FF FF T...............
00402BA0 04 01 00 00 F0 01 00 00 EC F3 FE FF 9C EF FF FF ................
00402BB0 D4 EC FF FF B4 F2 FF FF C6 EC FF FF C0 EC FF FF ................
00402BC0 F4 F3 FE FF C0 EF FF FF 56 EE FF FF C4 F3 FE FF ........V.......
00402BD0 78 F1 FF FF 3E EE FF FF A0 F3 FE FF E8 01 00 00 x...>...........
00402BE0 5C 01 00 00 D8 01 00 00 E4 F3 FE FF 38 EE FF FF \...........8...
00402BF0 40 F1 FF FF D6 F8 FF FF 94 EF FF FF F6 ED FF FF @...............

;; ls_main: 00402C00
ls_main proc
	mov.l	r8,@-r15
	mova	(00402E14),r0                                         ; @(210,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	r13,@-r15
	mov.l	r14,@-r15
	mov.l	(00402E14),r12                                       ; @(200,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#F0,r15
	mov.l	(00402E18),r0                                        ; @(1FC,pc)
	mov	r5,r11
	mov	r4,r9
	bsrf	r0
	mov.l	@r5,r4
	mov.l	(00402E1C),r1                                        ; @(1F8,pc)
	mov	#00,r4
	mov.l	(00402E20),r5                                        ; @(1F8,pc)
	bsrf	r1
	add	r12,r5
	mov.l	(00402E24),r7                                        ; @(1F4,pc)
	bsrf	r7
	mov	#01,r4
	tst	r0,r0
	bf	00402C3A

l00402C36:
	bra	00402D4E
	mov	#01,r2

l00402C3A:
	mov.l	(00402E28),r1                                        ; @(1EC,pc)
	mov	r15,r8
	add	#08,r8
	mov	r8,r6
	mov.l	(00402E2C),r5                                        ; @(1E8,pc)
	bsrf	r1
	mov	#01,r4
	tst	r0,r0
	bf/s	00402C5E
	mov	#01,r1

l00402C4E:
	mov.w	@(2,r8),r0
	extu.w	r0,r2
	tst	r2,r2
	bt/s	00402C5E
	mov	r12,r0

l00402C58:
	mov.l	(00402E30),r1                                        ; @(1D4,pc)
	mov.l	r2,@(r0,r1)
	mov	#01,r1

l00402C5E:
	mov.l	(00402E34),r0                                        ; @(1D4,pc)
	mov.l	@(r0,r12),r2
	mov.l	(00402E38),r0                                        ; @(1D4,pc)
	mov.l	r1,@r2
	mov.l	@(r0,r12),r2
	mov.l	r1,@r2

l00402C6A:
	mov.l	(00402E3C),r1                                        ; @(1D0,pc)
	bsrf	r1
	nop
	tst	r0,r0
	bf/s	00402C7E
	mov	#00,r13

l00402C76:
	mov.l	(00402E40),r0                                        ; @(1C8,pc)
	mov	#01,r2
	mov.l	@(r0,r12),r1
	mov.l	r2,@r1

l00402C7E:
	mov.l	(00402E44),r10                                       ; @(1C4,pc)
	mov	#10,r8
	mov.l	(00402E34),r14                                       ; @(1B0,pc)
	add	r12,r10
	mov.l	r10,@(4,r15)
	add	r12,r14
	mov.w	(00402E10),r10                                       ; @(182,pc)
	mov	r11,r5
	mov.l	(00402E48),r1                                        ; @(1B8,pc)
	mov	r9,r4
	bsrf	r1
	mov.l	@(4,r15),r6
	cmp/eq	#FF,r0
	bf/s	00402D56
	mov	#47,r1

l00402C9C:
	mov.l	(00402E4C),r0                                        ; @(1AC,pc)
	mov.l	@(r0,r12),r1
	mov.l	(00402E38),r0                                        ; @(194,pc)
	mov.l	@r1,r10
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf/s	00402CC2
	sub	r10,r9

l00402CAE:
	mov.l	(00402E50),r0                                        ; @(1A0,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	00402CC2

l00402CB8:
	mov.l	(00402E54),r0                                        ; @(198,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00402CDE

l00402CC2:
	mov.l	(00402E58),r4                                        ; @(194,pc)
	mov.l	(00402E5C),r7                                        ; @(194,pc)
	bsrf	r7
	add	r12,r4
	tst	r0,r0
	bt/s	00402CDE
	mov	r0,r4

l00402CD0:
	mov.l	(00402E60),r1                                        ; @(18C,pc)
	bsrf	r1
	nop
	mov.l	(00402E30),r1                                        ; @(158,pc)
	mov	r0,r2
	mov	r12,r0
	mov.l	r2,@(r0,r1)

l00402CDE:
	mov.l	(00402E64),r0                                        ; @(184,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r0
	cmp/eq	#FF,r0
	bf/s	00402CEC
	mov	#00,r2

l00402CEA:
	mov.l	r2,@r1

l00402CEC:
	mov.l	(00402E68),r0                                        ; @(178,pc)
	mov.l	(00402E6C),r14                                       ; @(17C,pc)
	mov.l	@(r0,r12),r1
	mov	r12,r0
	mov.l	@(r0,r14),r2
	mov.l	@r1,r1
	mov.l	@r2,r3
	tst	r1,r1
	mov.l	(00402E70),r2                                        ; @(170,pc)
	bt	00402D04

l00402D00:
	bra	00403068
	tst	r3,r3

l00402D04:
	tst	r3,r3
	bf	00402D4A

l00402D08:
	mov.l	(00402E74),r0                                        ; @(168,pc)
	mov.l	@(r0,r12),r7
	mov.l	@r7,r7
	tst	r7,r7
	bf	00402D32

l00402D12:
	mov.l	(00402E78),r0                                        ; @(164,pc)
	mov.l	@(r0,r12),r7
	mov.l	@r7,r7
	tst	r7,r7
	bf	00402D32

l00402D1C:
	mov.l	(00402E7C),r0                                        ; @(15C,pc)
	mov.l	@(r0,r12),r7
	mov.l	@r7,r7
	tst	r7,r7
	bf/s	00402D32
	mov	r12,r0

l00402D28:
	mov.l	@(r0,r2),r7
	tst	r7,r7
	bf/s	00402D32
	mov	#08,r7

l00402D30:
	or	r7,r8

l00402D32:
	mov.l	(00402E80),r0                                        ; @(14C,pc)
	mov.l	@(r0,r12),r7
	mov.l	@r7,r7
	tst	r7,r7
	bf	00402D4A

l00402D3C:
	mov.l	(00402E78),r0                                        ; @(138,pc)
	mov.l	@(r0,r12),r7
	mov.l	@r7,r7
	tst	r7,r7
	bf/s	00402D4A
	mov	#01,r0

l00402D48:
	or	r0,r8

l00402D4A:
	bra	0040306E
	nop

l00402D4E:
	mov.l	(00402E84),r0                                        ; @(134,pc)
	mov.l	@(r0,r12),r1
	bra	00402C6A
	mov.l	r2,@r1

l00402D56:
	mov	r0,r2
	add	#CF,r2
	cmp/hi	r1,r2
	bf	00402D62

l00402D5E:
	bra	0040304A
	nop

l00402D62:
	mova	(00402D6C),r0                                         ; @(08,pc)
	add	r2,r2
	mov.w	@(r0,r2),r1
	braf	r1
	nop
00402D6C                                     90 00 DE 02             ....
00402D70 DE 02 DE 02 DE 02 DE 02 DE 02 DE 02 DE 02 DE 02 ................
00402D80 DE 02 DE 02 DE 02 DE 02 DE 02 DE 02 00 02 06 02 ................
00402D90 1C 01 DE 02 DE 02 DC 01 DE 02 DE 02 DE 02 DE 02 ................
00402DA0 DE 02 E4 01 62 02 DE 02 7E 02 8A 02 DE 02 EE 01 ....b...~.......
00402DB0 AC 02 BA 02 DE 02 DE 02 C6 02 D8 02 DE 02 DE 02 ................
00402DC0 DE 02 DE 02 DE 02 DE 02 DE 02 DE 02 FC 01 1A 02 ................
00402DD0 C0 01 2A 02 DE 02 F4 01 40 01 4E 02 36 02 DE 02 ..*.....@.N.6...
00402DE0 3C 02 6E 01 9A 01 6E 02 84 02 90 02 96 02 A6 02 <.n...n.........
00402DF0 B4 02 C0 02 D0 01 DE 02 CC 02 A6 01 21 D0 01 E3 ............!...
00402E00 CE 01 14 D0 32 21 00 E1 CE 03 18 D0 5E A0 12 23 ....2!......^..#
00402E10 00 04 09 00 CC 23 01 00 F2 EC FF FF D0 EB FF FF .....#..........
00402E20 F0 F3 FE FF F2 EF FF FF F0 EA FF FF 68 74 08 40 ............ht.@
00402E30 F0 FF FF FF 8C 01 00 00 C4 01 00 00 E8 F0 FF FF ................
00402E40 5C 01 00 00 40 F4 FE FF 3A EF FF FF 88 01 00 00 \...@...:.......
00402E50 68 01 00 00 C0 01 00 00 64 F4 FE FF E6 EF FF FF h.......d.......
00402E60 32 EF FF FF 70 01 00 00 40 01 00 00 74 01 00 00 2...p...@...t...
00402E70 EC 01 00 00 60 01 00 00 C8 01 00 00 98 01 00 00 ....`...........
00402E80 F4 00 00 00 DC 01 00 00 CC D0 01 E3 CE 01 CC D0 ................
00402E90 32 21 00 E1 CE 03 CB D0 12 23 CE 03 CA D0 12 23 2!.......#.....#
00402EA0 CE 03 CA D0 12 23 CE 03 F0 AE 12 23 C8 D0 CE 01 .....#.....#....
00402EB0 12 60 FF 88 01 8D 01 E3 32 21 01 E3 C2 D0 CE 01 .`......2!......
00402EC0 32 21 00 E1 BE D0 CE 03 BE D0 12 23 CE 03 BF D0 2!.........#....
00402ED0 12 23 CE 03 B9 D0 E6 AF 12 23 BB D0 01 E3 CE 01 .#.......#......
00402EE0 B7 D0 32 21 00 E1 CE 03 B6 D0 12 23 CE 03 B7 D0 ..2!.......#....
00402EF0 12 23 CE 03 B1 D0 12 23 CE 03 B5 D0 12 23 FF E3 .#.....#.....#..
00402F00 CE 01 C3 AE 32 21 AE D0 01 E3 CE 01 AD D0 7A AF ....2!........z.
00402F10 32 21 AE D0 01 E3 CE 01 A9 D0 32 21 00 E1 CE 03 2!........2!....
00402F20 A8 D0 12 23 CE 03 A8 D0 D3 AF 12 23 A9 D0 01 E3 ...#.......#....
00402F30 CE 01 A9 D0 32 21 00 E3 E3 AF CE 01 A6 D0 01 E3 ....2!..........
00402F40 CE 01 A4 D0 F7 AF 32 21 A4 D0 01 E3 D9 AF CE 01 ......2!........
00402F50 EF E0 89 20 02 E8 99 AE 0B 28 A1 D0 F6 AF 01 E3 ... .....(......
00402F60 A0 D0 01 E3 CE 01 32 21 20 E1 1B 28 9E D0 ED AF ......2! ..(....
00402F70 01 E3 E2 63 00 E1 9D D0 01 E7 12 23 CE 03 72 23 ...c.......#..r#
00402F80 9B D0 91 AF CE 03 E2 63 00 E1 98 D0 12 23 CE 03 .......c.....#..
00402F90 97 D0 DA AF 12 23 97 D0 01 E3 CE 01 90 D0 CA AF .....#..........
00402FA0 32 21 95 D0 D2 AF 01 E3 94 D0 00 E3 01 ED CE 01 2!..............
00402FB0 93 D0 A2 21 CE 01 69 AE 32 21 91 D0 01 E3 00 ED ...!..i.2!......
00402FC0 CE 01 90 D0 32 21 00 E3 CE 01 5F AE 32 21 8C D0 ....2!...._.2!..
00402FD0 00 E3 CE 01 8B D0 B8 AF 32 21 8B D0 01 E1 CE 03 ........2!......
00402FE0 79 D0 12 23 CE 03 6C AF 12 23 88 D0 AE AF 01 E3 y..#..l..#......
00402FF0 87 D0 AB AF 01 E3 87 D0 A8 AF 01 E3 86 D0 A5 AF ................
00403000 01 E3 E2 61 01 E3 79 D0 32 21 00 E1 CE 03 B7 AF ...a..y.2!......
00403010 12 23 82 D0 9A AF 01 E3 81 D0 01 E1 36 AE 16 0C .#..........6...
00403020 80 D0 93 AF 01 E3 80 D0 90 AF 01 E3 7C D0 F5 AF ............|...
00403030 02 E1 7E D0 8A AF 01 E3 E2 63 00 E1 6B D0 12 23 ..~......c..k..#
00403040 E5 AF CE 03 40 E7 21 AE 7B 28                   ....@.!.{(      

l0040304A:
	mov.l	(00403230),r1                                        ; @(1E4,pc)
	bsrf	r1
	nop
	mov	r0,r6
	mov.l	(00403234),r0                                        ; @(1E0,pc)
	mov.w	(004031B4),r7                                        ; @(15C,pc)
	mov.l	@(r0,r12),r4
	mov.l	(00403238),r1                                        ; @(1DC,pc)
	add	r7,r4
	mov.l	(0040323C),r5                                        ; @(1DC,pc)
	bsrf	r1
	add	r12,r5
	mov.l	(00403240),r7                                        ; @(1DC,pc)
	bsrf	r7
	mov	#01,r4

l00403068:
	bf	0040306E

l0040306A:
	bra	00402D32
	nop

l0040306E:
	mov.l	(0040322C),r0                                        ; @(1BC,pc)
	mov.l	@(r0,r12),r7
	mov.l	@r7,r7
	tst	r7,r7
	bt	0040307C

l00403078:
	mov.w	(004031B6),r7                                        ; @(13A,pc)
	or	r7,r8

l0040307C:
	or	r3,r1
	tst	r1,r1
	bf/s	00403092
	tst	r13,r13

l00403084:
	mov.l	(00403224),r0                                        ; @(19C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	004030BE
	mov	r12,r0

l00403090:
	tst	r13,r13

l00403092:
	mov.l	(004031FC),r13                                       ; @(168,pc)
	bf/s	004030A4
	mov	#00,r4

l00403098:
	mov.l	(00403244),r1                                        ; @(1A8,pc)
	mov	r12,r0
	mov.l	r2,@r15
	bsrf	r1
	mov.l	@(r0,r13),r5
	mov.l	@r15,r2

l004030A4:
	mov	r12,r0
	mov.l	@(r0,r13),r7
	mov.w	(004031B8),r6                                        ; @(10C,pc)
	mov.l	@r7,r3
	mov	r3,r1
	shll	r1
	subc	r1,r1
	and	r6,r1
	add	r3,r1
	mov	#F7,r3
	shad	r3,r1
	mov.l	r1,@r7
	mov	r12,r0

l004030BE:
	mov.l	(0040321C),r1                                        ; @(15C,pc)
	mov.l	@(r0,r2),r2
	mov.l	@(r0,r1),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	0040316A
	mov	r2,r0

l004030CC:
	cmp/eq	#01,r0
	bt/s	0040313C
	cmp/eq	#02,r0

l004030D2:
	bt/s	00403146
	tst	r2,r2

l004030D6:
	mov.l	(00403248),r0                                        ; @(170,pc)
	bt	0040313E

l004030DA:
	mov.l	(004031C4),r0                                        ; @(E8,pc)
	mov.l	(0040324C),r2                                        ; @(16C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov.l	(00403250),r1                                        ; @(168,pc)
	bf/s	0040310E
	mov	r12,r0

l004030EA:
	mov.l	(004031CC),r1                                        ; @(E0,pc)
	mov.l	@(r0,r1),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov.l	(00403254),r1                                        ; @(160,pc)
	bf	0040310E

l004030F6:
	mov.l	@(r0,r14),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov.l	(00403258),r1                                        ; @(158,pc)
	bf	0040310E

l00403100:
	mov.l	(004031C0),r1                                        ; @(BC,pc)
	mov.l	@(r0,r1),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov.l	(0040325C),r1                                        ; @(150,pc)
	bf	0040310E

l0040310C:
	mov.l	(00403260),r1                                        ; @(150,pc)

l0040310E:
	mov.l	@(r0,r1),r1
	tst	r9,r9
	bt/s	004031A4
	mov.l	r1,@(r0,r2)

l00403116:
	mov.l	(00403264),r1                                        ; @(14C,pc)
	mov	r10,r5
	shll2	r5
	mov	r8,r6
	add	r11,r5
	bsrf	r1
	mov	r9,r4

l00403124:
	mov.l	(00403268),r0                                        ; @(140,pc)
	mov.l	@(r0,r12),r0
	add	#10,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r14
	mov.l	@r15+,r13
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	rts
0040313A                               F6 68                       .h    

l0040313C:
	mov.l	(0040326C),r0                                        ; @(12C,pc)

l0040313E:
	mov.l	@(r0,r12),r1

l00403140:
	mov.l	(00403270),r0                                        ; @(12C,pc)
	bra	004030DA
	mov.l	r1,@(r0,r12)

l00403146:
	mov.l	(004031D8),r0                                        ; @(90,pc)
	mov.l	(00403270),r2                                        ; @(124,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov.l	(00403274),r1                                        ; @(120,pc)
	bf/s	00403164
	mov	r12,r0

l00403156:
	mov.l	(004031D4),r1                                        ; @(7C,pc)
	mov.l	@(r0,r1),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov.l	(00403278),r1                                        ; @(118,pc)
	bf	00403164

l00403162:
	mov.l	(0040327C),r1                                        ; @(118,pc)

l00403164:
	mov.l	@(r0,r1),r1

l00403166:
	bra	004030DA
	mov.l	r1,@(r0,r2)

l0040316A:
	cmp/eq	#01,r0
	bt/s	0040317C
	cmp/eq	#02,r0

l00403170:
	bt/s	00403182
	tst	r2,r2

l00403174:
	mov.l	(00403280),r0                                        ; @(108,pc)
	bf	004030DA

l00403178:
	bra	00403140
	mov.l	@(r0,r12),r1

l0040317C:
	mov.l	(00403284),r0                                        ; @(104,pc)
	bra	00403140
	mov.l	@(r0,r12),r1

l00403182:
	mov.l	(004031D8),r0                                        ; @(54,pc)
	mov.l	(00403270),r2                                        ; @(E8,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov.l	(00403288),r1                                        ; @(F8,pc)
	bf/s	00403164
	mov	r12,r0

l00403192:
	mov.l	(004031D4),r1                                        ; @(40,pc)
	mov.l	@(r0,r1),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov.l	(0040328C),r1                                        ; @(F0,pc)
	bf	00403164

l0040319E:
	mov.l	(00403290),r1                                        ; @(F0,pc)
	bra	00403166
	mov.l	@(r0,r1),r1

l004031A4:
	mov.l	(00403294),r5                                        ; @(EC,pc)
	mov	r8,r6
	mov.l	(00403298),r7                                        ; @(EC,pc)
	add	r12,r5
	bsrf	r7
	mov	#01,r4
	bra	00403124
	nop
004031B4             B0 00 80 00 FF 01 09 00 C4 01 00 00     ............
004031C0 C0 01 00 00 DC 01 00 00 74 01 00 00 68 01 00 00 ........t...h...
004031D0 70 01 00 00 B4 01 00 00 D4 01 00 00 C8 01 00 00 p...............
004031E0 04 01 00 00 84 01 00 00 5C 01 00 00 80 01 00 00 ........\.......
004031F0 24 01 00 00 F4 00 00 00 40 01 00 00 3C 01 00 00 $.......@...<...
00403200 B8 01 00 00 38 01 00 00 18 01 00 00 D8 01 00 00 ....8...........
00403210 1C 01 00 00 10 01 00 00 98 01 00 00 48 01 00 00 ............H...
00403220 EC 01 00 00 60 01 00 00 90 01 00 00 50 01 00 00 ....`.......P...
00403230 F0 EB FF FF 54 01 00 00 E6 E9 FF FF 00 F4 FE FF ....T...........
00403240 C4 E9 FF FF 7E EC FF FF 4C 01 00 00 F8 01 00 00 ....~...L.......
00403250 A4 01 00 00 AC 01 00 00 7C 01 00 00 94 01 00 00 ........|.......
00403260 A0 01 00 00 B8 F8 FF FF E8 01 00 00 CC 01 00 00 ................
00403270 F4 01 00 00 00 01 00 00 9C 01 00 00 64 01 00 00 ............d...
00403280 20 01 00 00 0C 01 00 00 B0 01 00 00 F0 00 00 00  ...............
00403290 BC 01 00 00 F4 FF FF FF 2C F8 FF FF             ........,...    

;; safe_printpath: 0040329C
safe_printpath proc
	mov.l	r8,@-r15
	mova	(004032DC),r0                                         ; @(3C,pc)
	mov.l	r9,@-r15
	mov.l	r12,@-r15
	mov.l	(004032DC),r12                                       ; @(34,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov.l	(004032E0),r0                                        ; @(34,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r8
	tst	r8,r8
	bt/s	004032C8
	mov	r4,r9

l004032B6:
	mov.l	(004032E4),r1                                        ; @(2C,pc)
	bsrf	r1
	mov.l	@(28,r4),r4
	mov.l	(004032E8),r4                                        ; @(28,pc)
	mov	r0,r8
	mov.l	(004032EC),r1                                        ; @(28,pc)
	bsrf	r1
	add	r12,r4
	add	r0,r8

l004032C8:
	mov.l	(004032F0),r1                                        ; @(24,pc)
	mov	r9,r4
	bsrf	r1
	add	#54,r4
	add	r8,r0
	lds.l	@r15+,pr
	mov.l	@r15+,r12
	mov.l	@r15+,r9
	rts
004032DA                               F6 68 04 1F 01 00           .h....
004032E0 10 01 00 00 7C 0F 00 00 6C F4 FE FF 72 0F 00 00 ....|...l...r...
004032F0 68 0F 00 00                                     h...            

;; printescapedpath: 004032F4
printescapedpath proc
	mov.l	r8,@-r15
	mova	(00403334),r0                                         ; @(3C,pc)
	mov.l	r9,@-r15
	mov.l	r12,@-r15
	mov.l	(00403334),r12                                       ; @(34,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov.l	(00403338),r0                                        ; @(34,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r8
	tst	r8,r8
	bt/s	00403320
	mov	r4,r9

l0040330E:
	mov.l	(0040333C),r1                                        ; @(2C,pc)
	bsrf	r1
	mov.l	@(28,r4),r4
	mov.l	(00403340),r4                                        ; @(28,pc)
	mov	r0,r8
	mov.l	(00403344),r1                                        ; @(28,pc)
	bsrf	r1
	add	r12,r4
	add	r0,r8

l00403320:
	mov.l	(00403348),r1                                        ; @(24,pc)
	mov	r9,r4
	bsrf	r1
	add	#54,r4
	add	r8,r0
	lds.l	@r15+,pr
	mov.l	@r15+,r12
	mov.l	@r15+,r9
	rts
00403332       F6 68 AC 1E 01 00 10 01 00 00 E4 0F 00 00   .h............
00403340 6C F4 FE FF DA 0F 00 00 D0 0F 00 00             l...........    

;; printlink: 0040334C
printlink proc
	mov.l	r8,@-r15
	mova	(00403428),r0                                         ; @(D8,pc)
	mov.l	r9,@-r15
	mov.l	r12,@-r15
	mov.w	(0040341E),r1                                        ; @(C6,pc)
	mov.l	(00403428),r12                                       ; @(D0,pc)
	sts.l	pr,@-r15
	add	r0,r12
	sub	r1,r15
	mov	r4,r1
	add	#40,r1
	mov.l	@(4,r1),r1
	mov	r4,r7
	mov	r15,r9
	tst	r1,r1
	add	#54,r7
	bf/s	004033BE
	add	#04,r9

l00403370:
	mov.l	(0040342C),r6                                        ; @(B8,pc)
	mov	r9,r4
	mov.l	(00403430),r1                                        ; @(B8,pc)
	mov.w	(00403420),r5                                        ; @(A6,pc)
	bsrf	r1
	add	r12,r6

l0040337C:
	mov.w	(00403422),r8                                        ; @(A2,pc)
	mov	r9,r4
	mov.l	(00403434),r1                                        ; @(B0,pc)
	add	r15,r8
	mov.w	(00403424),r6                                        ; @(9C,pc)
	bsrf	r1
	mov	r8,r5
	cmp/eq	#FF,r0
	bf/s	004033D4
	mov	#00,r1

l00403390:
	mov.l	(00403438),r1                                        ; @(A4,pc)
	bsrf	r1
	nop
	mov.l	(0040343C),r1                                        ; @(A4,pc)
	bsrf	r1
	mov.l	@r0,r4
	mov	r0,r7
	mov.l	(00403440),r0                                        ; @(A0,pc)
	mov	r9,r6
	mov.w	(00403426),r1                                        ; @(80,pc)
	mov.l	@(r0,r12),r4
	mov.l	(00403444),r5                                        ; @(9C,pc)
	add	r1,r4
	mov.l	(00403448),r1                                        ; @(9C,pc)
	bsrf	r1
	add	r12,r5

l004033B0:
	mov.w	(0040341E),r7                                        ; @(6A,pc)
	add	r7,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r12
	mov.l	@r15+,r9
	rts
004033BC                                     F6 68                   .h  

l004033BE:
	mov.l	@(4,r4),r1
	mov	r9,r4
	mov.l	r7,@r15
	mov.l	(0040344C),r6                                        ; @(84,pc)
	mov.l	@(24,r1),r7
	mov.l	(00403450),r1                                        ; @(84,pc)
	mov.w	(00403420),r5                                        ; @(52,pc)
	bsrf	r1
	add	r12,r6
	bra	0040337C
	nop

l004033D4:
	mov.b	r1,@(r0,r8)
	mov.l	(00403454),r1                                        ; @(7C,pc)
	mov.l	(00403458),r4                                        ; @(7C,pc)
	bsrf	r1
	add	r12,r4
	mov.l	(0040345C),r0                                        ; @(7C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	004033F2

l004033E8:
	mov.l	(00403460),r0                                        ; @(74,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	004033FC

l004033F2:
	mov.l	(00403464),r1                                        ; @(70,pc)
	bsrf	r1
	mov	r8,r4
	bra	004033B0
	nop

l004033FC:
	mov.l	(00403468),r0                                        ; @(68,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403410

l00403406:
	mov.l	(0040346C),r1                                        ; @(64,pc)
	bsrf	r1
	mov	r8,r4
	bra	004033B0
	nop

l00403410:
	mov.l	(00403470),r1                                        ; @(5C,pc)
	mov	r8,r5
	mov.l	(0040342C),r4                                        ; @(14,pc)
	bsrf	r1
	add	r12,r4
	bra	004033B0
	nop
0040341E                                           0C 08               ..
00403420 01 04 08 04 00 04 B0 00 B8 1D 01 00 A4 F3 FE FF ................
00403430 64 E4 FF FF 32 E6 FF FF BE E3 FF FF C0 E8 FF FF d...2...........
00403440 54 01 00 00 78 F4 FE FF 98 E6 FF FF 70 F4 FE FF T...x.......p...
00403450 10 E4 FF FF 8A E5 FF FF 88 F4 FE FF 80 01 00 00 ................
00403460 24 01 00 00 40 0E 00 00 8C 01 00 00 EC 0E 00 00 $...@...........
00403470 4E E5 FF FF                                     N...            

;; printpath: 00403474
printpath proc
	mov.l	r12,@-r15
	mova	(004034AC),r0                                         ; @(34,pc)
	mov.l	(004034AC),r12                                       ; @(30,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov.l	(004034B0),r0                                        ; @(30,pc)
	mov	r4,r6
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	0040349C
	add	#54,r6

l0040348C:
	mov.l	@(28,r4),r5
	mov.l	(004034B4),r1                                        ; @(24,pc)
	mov.l	(004034B8),r4                                        ; @(24,pc)
	bsrf	r1
	add	r12,r4

l00403496:
	lds.l	@r15+,pr
	rts
0040349A                               F6 6C                       .l    

l0040349C:
	mov.l	(004034BC),r1                                        ; @(1C,pc)
	mov	r6,r5
	mov.l	(004034C0),r4                                        ; @(1C,pc)
	bsrf	r1
	add	r12,r4
	bra	00403496
	nop
004034AA                               09 00 34 1D 01 00           ..4...
004034B0 10 01 00 00 D2 E4 FF FF 70 F4 FE FF C2 E4 FF FF ........p.......
004034C0 A4 F3 FE FF                                     ....            

;; printtotal: 004034C4
printtotal proc
	mov.l	r8,@-r15
	mova	(00403580),r0                                         ; @(B8,pc)
	mov.l	r12,@-r15
	mov.l	(00403580),r12                                       ; @(B4,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#EC,r15
	mov.l	@r4,r1
	add	#40,r1
	mov.l	@(4,r1),r1
	tst	r1,r1
	bt	00403530

l004034DC:
	mov.l	(00403584),r0                                        ; @(A4,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	004034F0

l004034E6:
	mov.l	(00403588),r0                                        ; @(A0,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403530

l004034F0:
	mov.l	(0040358C),r0                                        ; @(98,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	0040353A
	mov	#07,r1

l004034FC:
	mov.l	r1,@(8,r15)
	mov	#20,r1
	mov.l	r1,@(4,r15)
	mov	r15,r8
	mov.l	(00403590),r1                                        ; @(88,pc)
	add	#0C,r8
	mov	#05,r5
	add	r12,r1
	mov.l	r1,@r15
	mov.l	(00403594),r1                                        ; @(84,pc)
	mov.l	@(12,r4),r6
	mov.l	@(16,r4),r7
	bsrf	r1
	mov	r8,r4
	cmp/eq	#FF,r0
	bf	00403526

l0040351C:
	mov.l	(00403598),r5                                        ; @(78,pc)
	mov	#01,r4
	mov.l	(0040359C),r2                                        ; @(78,pc)
	bsrf	r2
	add	r12,r5

l00403526:
	mov.l	(004035A0),r4                                        ; @(78,pc)
	mov	r8,r5
	mov.l	(004035A4),r1                                        ; @(78,pc)
	bsrf	r1
	add	r12,r4

l00403530:
	add	#14,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r12
	rts
00403538                         F6 68                           .h      

l0040353A:
	mov.l	(004035A8),r0                                        ; @(6C,pc)
	mov.l	(004035AC),r8                                        ; @(6C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	0040354A
	mov	#00,r7

l00403548:
	mov.l	(004035B0),r8                                        ; @(64,pc)

l0040354A:
	mov.l	(004035B4),r0                                        ; @(68,pc)
	add	r12,r8
	mov.l	@(8,r4),r5
	mov.l	@(r0,r12),r1
	mov.l	@(4,r4),r0
	mov.l	@r1,r3
	mov	r3,r6
	add	#FF,r6
	cmp/gt	r6,r7
	mov	r6,r2
	subc	r1,r1
	mov	r2,r4
	cmp/gt	r3,r7
	mov.l	(004035B8),r2                                        ; @(50,pc)
	subc	r7,r7
	clrt
	addc	r0,r4
	mov	r3,r6
	bsrf	r2
	addc	r1,r5
	mov	r1,r6
	mov.l	(004035BC),r1                                        ; @(44,pc)
	mov	r0,r5
	bsrf	r1
	mov	r8,r4
	bra	00403530
	nop
00403580 60 1C 01 00 74 01 00 00 60 01 00 00 B8 01 00 00 `...t...`.......
00403590 F0 F3 FE FF 10 E6 FF FF AC F4 FE FF 3E E5 FF FF ............>...
004035A0 BC F4 FE FF 38 E4 FF FF 38 01 00 00 A0 F4 FE FF ....8...8.......
004035B0 90 F4 FE FF 3C 01 00 00 2E E4 FF FF EC E3 FF FF ....<...........

;; __sputc.constprop.3: 004035C0
__sputc.constprop.3 proc
	mov.l	r12,@-r15
	mova	(00403610),r0                                         ; @(4C,pc)
	mov.l	(00403610),r12                                       ; @(48,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov.l	(00403614),r3                                        ; @(48,pc)
	mov	r12,r0
	mov.l	@(r0,r3),r2
	add	#40,r2
	mov.l	@(32,r2),r1
	add	#FF,r1
	cmp/pz	r1
	bt/s	004035EA
	mov.l	r1,@(32,r2)

l004035DC:
	mov.l	@(48,r2),r2
	cmp/ge	r2,r1
	bf	00403602

l004035E2:
	mov	r4,r0
	cmp/eq	#0A,r0
	bt	00403600

l004035E8:
	mov	r12,r0

l004035EA:
	mov.l	@(r0,r3),r1
	extu.b	r4,r0
	add	#40,r1
	mov.l	@(24,r1),r2
	mov	r2,r3
	add	#01,r3
	mov.l	r3,@(24,r1)
	mov.b	r4,@r2

l004035FA:
	lds.l	@r15+,pr
	rts
004035FE                                           F6 6C               .l

l00403600:
	mov	r12,r0

l00403602:
	mov.l	@(r0,r3),r5
	mov.l	(00403618),r1                                        ; @(10,pc)
	bsrf	r1
	add	#58,r5
	bra	004035FA
	nop
0040360E                                           09 00               ..
00403610 D0 1B 01 00 54 01 00 00 AE E4 FF FF             ....T.......    

;; printtime: 0040361C
printtime proc
	mov.l	r8,@-r15
	mova	(004036EC),r0                                         ; @(CC,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	(004036EC),r12                                       ; @(C0,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#F8,r15
	mov.l	(004036F0),r1                                        ; @(BC,pc)
	mov	r15,r11
	add	#08,r11
	mov.l	r5,@-r11
	mov.l	r4,@-r11
	bsrf	r1
	mov	r15,r4
	tst	r0,r0
	bf/s	00403648
	mov	r0,r8

l00403644:
	mov.l	(004036F4),r8                                        ; @(AC,pc)
	add	r12,r8

l00403648:
	mov	r8,r9
	mov	r8,r10
	add	#04,r9
	add	#0B,r10

l00403650:
	mov.l	(004036F8),r1                                        ; @(A4,pc)
	bsrf	r1
	mov.b	@r9+,r4
	cmp/eq	r10,r9
	bf	00403650

l0040365A:
	mov.l	(004036FC),r0                                        ; @(A0,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00403688
	clrt

l00403666:
	add	#18,r8

l00403668:
	mov.l	(00403700),r1                                        ; @(94,pc)
	bsrf	r1
	mov.b	@r9+,r4
	cmp/eq	r9,r8
	bf	00403668

l00403672:
	mov.l	(00403704),r1                                        ; @(90,pc)
	bsrf	r1
	mov	#20,r4
	add	#08,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	rts
00403686                   F6 68                               .h        

l00403688:
	mov.l	(00403708),r3                                        ; @(7C,pc)
	mov.l	@r11,r2
	add	r12,r3
	mov.l	(0040370C),r6                                        ; @(7C,pc)
	mov.l	@(4,r11),r1
	addc	r2,r6
	mov.l	@(4,r3),r7
	mov.l	@r3,r5
	mov	#00,r3
	addc	r1,r3
	cmp/ge	r7,r3
	bf/s	004036D2
	cmp/gt	r7,r3

l004036A2:
	bt/s	004036A8
	cmp/hs	r5,r6

l004036A6:
	bf	004036D2

l004036A8:
	mov.l	(00403710),r6                                        ; @(64,pc)
	clrt
	mov	#FF,r3
	addc	r6,r2
	addc	r3,r1
	cmp/ge	r1,r7
	bf/s	004036D2
	cmp/gt	r1,r7

l004036B8:
	bt/s	004036C4
	add	#10,r8

l004036BC:
	cmp/hs	r2,r5
	bf/s	004036D2
	add	#F0,r8

l004036C2:
	add	#10,r8

l004036C4:
	mov.l	(00403714),r1                                        ; @(4C,pc)
	bsrf	r1
	mov.b	@r10+,r4
	cmp/eq	r10,r8
	bf	004036C4

l004036CE:
	bra	00403672
	nop

l004036D2:
	mov.l	(00403718),r1                                        ; @(44,pc)
	mov	r8,r9
	mov	#20,r4
	bsrf	r1
	add	#14,r9
	add	#18,r8

l004036DE:
	mov.l	(0040371C),r1                                        ; @(3C,pc)
	bsrf	r1
	mov.b	@r9+,r4
	cmp/eq	r8,r9
	bf	004036DE

l004036E8:
	bra	00403672
	nop
004036EC                                     F4 1A 01 00             ....
004036F0 22 E5 FF FF C8 F4 FE FF 6A FF FF FF 90 01 00 00 ".......j.......
00403700 52 FF FF FF 48 FF FF FF 00 02 00 00 FF F0 EF 00 R...H...........
00403710 01 0F 10 FF F6 FE FF FF E4 FE FF FF DC FE FF FF ................

;; printtype: 00403720
printtype proc
	mov.l	r12,@-r15
	mova	(0040379C),r0                                         ; @(78,pc)
	mov.l	(0040379C),r12                                       ; @(74,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov.l	(004037A0),r1                                        ; @(74,pc)
	mov.l	(004037A4),r2                                        ; @(74,pc)
	and	r4,r1
	cmp/eq	r2,r1
	bt/s	00403784
	cmp/hi	r2,r1

l00403736:
	bt	00403756

l00403738:
	mov.w	(00403798),r2                                        ; @(5C,pc)
	cmp/eq	r2,r1
	bt	0040377A

l0040373E:
	mov.w	(0040379A),r2                                        ; @(58,pc)
	cmp/eq	r2,r1
	bt	0040376C

l00403744:
	mov	r4,r0
	tst	#49,r0
	bt/s	00403774
	mov	#00,r0

l0040374C:
	mov.l	(004037A8),r1                                        ; @(58,pc)
	bsrf	r1
	mov	#2A,r4
	bra	00403774
	mov	#01,r0

l00403756:
	mov.l	(004037AC),r2                                        ; @(54,pc)
	cmp/eq	r2,r1
	bt	0040378E

l0040375C:
	mov.l	(004037B0),r2                                        ; @(50,pc)
	cmp/eq	r2,r1
	bf	00403744

l00403762:
	mov.l	(004037B4),r1                                        ; @(50,pc)
	bsrf	r1
	mov	#25,r4
	bra	00403774
	mov	#01,r0

l0040376C:
	mov.l	(004037B8),r1                                        ; @(48,pc)
	bsrf	r1
	mov	#2F,r4
	mov	#01,r0

l00403774:
	lds.l	@r15+,pr
	rts
00403778                         F6 6C                           .l      

l0040377A:
	mov.l	(004037BC),r1                                        ; @(40,pc)
	bsrf	r1
	mov	#7C,r4
	bra	00403774
	mov	#01,r0

l00403784:
	mov.l	(004037C0),r1                                        ; @(38,pc)
	bsrf	r1
	mov	#40,r4
	bra	00403774
	mov	#01,r0

l0040378E:
	mov.l	(004037C4),r1                                        ; @(34,pc)
	bsrf	r1
	mov	#3D,r4
	bra	00403774
	mov	#01,r0
00403798                         00 10 00 40 44 1A 01 00         ...@D...
004037A0 00 F0 00 00 00 A0 00 00 6E FE FF FF 00 C0 00 00 ........n.......
004037B0 00 E0 00 00 58 FE FF FF 4E FE FF FF 40 FE FF FF ....X...N...@...
004037C0 36 FE FF FF 2C FE FF FF                         6...,...        

;; printaname: 004037C8
printaname proc
	mov.l	r8,@-r15
	mova	(00403914),r0                                         ; @(148,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	r13,@-r15
	mov.l	(00403914),r12                                       ; @(13C,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#EC,r15
	mov.l	(00403918),r0                                        ; @(138,pc)
	mov	r4,r1
	add	#40,r1
	mov.l	@(16,r1),r10
	mov	r4,r11
	mov.l	@(r0,r12),r1
	mov.l	@r1,r8
	tst	r8,r8
	bt/s	00403800
	mov	r6,r13

l004037F2:
	mov.l	(0040391C),r4                                        ; @(128,pc)
	mov.l	(00403920),r1                                        ; @(128,pc)
	add	r12,r4
	mov.l	@(12,r10),r6
	bsrf	r1
	mov.l	@(16,r10),r7
	mov	r0,r8

l00403800:
	mov.l	(00403924),r0                                        ; @(120,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00403852
	mov	r10,r7

l0040380C:
	mov.l	(00403928),r0                                        ; @(118,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	004038AC
	add	#40,r7

l00403818:
	mov	#07,r1
	mov.l	r1,@(8,r15)
	mov	#20,r1
	mov.l	r1,@(4,r15)
	mov	r15,r9
	mov.l	(0040392C),r1                                        ; @(108,pc)
	add	#0C,r9
	mov	#05,r5
	add	r12,r1
	mov.l	r1,@r15
	mov	r9,r4
	mov.l	(00403930),r1                                        ; @(100,pc)
	mov.l	@(24,r7),r6
	bsrf	r1
	mov.l	@(28,r7),r7
	cmp/eq	#FF,r0
	bf/s	00403846
	mov	r9,r6

l0040383C:
	mov.l	(00403934),r5                                        ; @(F4,pc)
	mov	#01,r4
	mov.l	(00403938),r2                                        ; @(F4,pc)
	bsrf	r2
	add	r12,r5

l00403846:
	mov.l	(0040393C),r4                                        ; @(F4,pc)
	mov.l	(00403940),r1                                        ; @(F4,pc)
	mov	r13,r5
	bsrf	r1
	add	r12,r4
	add	r0,r8

l00403852:
	mov.l	(00403944),r0                                        ; @(F0,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	00403866

l0040385C:
	mov.l	(00403948),r0                                        ; @(E8,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	004038F2

l00403866:
	mov.l	(0040394C),r2                                        ; @(E4,pc)
	bsrf	r2
	mov	r11,r4
	add	r0,r8

l0040386E:
	mov.l	(00403950),r0                                        ; @(E0,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	00403890

l00403878:
	mov.l	(00403954),r0                                        ; @(D8,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	0040389A
	mov	r8,r0

l00403884:
	mov.l	(00403958),r2                                        ; @(D0,pc)
	mov.l	@(8,r10),r1
	and	r2,r1
	mov.w	(00403912),r2                                        ; @(84,pc)
	cmp/eq	r2,r1
	bf	0040389A

l00403890:
	mov.l	(0040395C),r1                                        ; @(C8,pc)
	bsrf	r1
	mov.l	@(8,r10),r4
	add	r0,r8
	mov	r8,r0

l0040389A:
	add	#14,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r13
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	rts
004038AA                               F6 68                       .h    

l004038AC:
	mov.l	(00403960),r0                                        ; @(B0,pc)
	mov.l	(0040391C),r9                                        ; @(6C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	004038BA

l004038B8:
	mov.l	(00403964),r9                                        ; @(A8,pc)

l004038BA:
	add	r12,r9
	mov.l	(00403968),r0                                        ; @(A8,pc)
	mov.l	@(32,r7),r4
	mov.l	@(r0,r12),r1
	mov	#00,r0
	mov.l	@(36,r7),r5
	mov.l	@r1,r3
	mov	r3,r6
	add	#FF,r6
	cmp/gt	r6,r0
	subc	r1,r1
	cmp/gt	r3,r0
	mov	r6,r2
	subc	r7,r7
	clrt
	addc	r2,r4
	mov.l	(0040396C),r2                                        ; @(90,pc)
	mov	r3,r6
	bsrf	r2
	addc	r1,r5
	mov	r1,r7
	mov.l	(00403970),r1                                        ; @(88,pc)
	mov	r0,r6
	mov	r13,r5
	bsrf	r1
	mov	r9,r4
	bra	00403852
	add	r0,r8

l004038F2:
	mov.l	(00403974),r0                                        ; @(80,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00403908
	mov	r11,r4

l004038FE:
	mov.l	(00403978),r1                                        ; @(78,pc)
	bsrf	r1
	nop
	bra	0040386E
	add	r0,r8

l00403908:
	mov.l	(0040397C),r2                                        ; @(70,pc)
	bsrf	r2
	nop
	bra	0040386E
	add	r0,r8
00403912       00 40 CC 18 01 00 40 01 00 00 EC F4 FE FF   .@....@.......
00403920 6A E1 FF FF 60 01 00 00 B8 01 00 00 F0 F3 FE FF j...`...........
00403930 F2 E2 FF FF AC F4 FE FF 1E E2 FF FF F4 F4 FE FF ................
00403940 18 E1 FF FF 80 01 00 00 24 01 00 00 30 FA FF FF ........$...0...
00403950 C8 01 00 00 98 01 00 00 00 F0 00 00 8A FE FF FF ................
00403960 38 01 00 00 E4 F4 FE FF 3C 01 00 00 D2 E2 FF FF 8.......<.......
00403970 7A E0 FF FF 8C 01 00 00 F0 F9 FF FF 66 FB FF FF z...........f...

;; printscol: 00403980
printscol proc
	mov.l	r8,@-r15
	mova	(004039C0),r0                                         ; @(3C,pc)
	mov.l	r9,@-r15
	mov.l	r12,@-r15
	mov.l	(004039C0),r12                                       ; @(34,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov.l	@r4,r8
	mov	r4,r9

l00403992:
	tst	r8,r8
	bf	004039A0

l00403996:
	lds.l	@r15+,pr
	mov.l	@r15+,r12
	mov.l	@r15+,r9
	rts
0040399E                                           F6 68               .h

l004039A0:
	mov.l	@(16,r8),r1
	tst	r1,r1
	bf/s	004039AC
	mov.l	@(12,r8),r0

l004039A8:
	cmp/eq	#01,r0
	bt	004039BC

l004039AC:
	mov.l	(004039C4),r1                                        ; @(14,pc)
	mov	r8,r4
	mov.l	@(28,r9),r6
	bsrf	r1
	mov.l	@(40,r9),r5
	mov.l	(004039C8),r1                                        ; @(10,pc)
	bsrf	r1
	mov	#0A,r4

l004039BC:
	bra	00403992
	mov.l	@(8,r8),r8
004039C0 20 18 01 00 12 FE FF FF 04 FC FF FF              ...........    

;; printlong: 004039CC
printlong proc
	mov.l	r8,@-r15
	mova	(00403AB0),r0                                         ; @(E0,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	r13,@-r15
	mov.l	r14,@-r15
	mov.l	(00403AB0),r12                                       ; @(D0,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#D4,r15
	mov.l	(00403AB4),r2                                        ; @(CC,pc)
	mov	r4,r8
	bsrf	r2
	mov	#00,r4
	mov.l	(00403AB8),r2                                        ; @(C8,pc)
	add	r12,r2
	mov.l	r0,@r2
	mov.l	(00403ABC),r0                                        ; @(C8,pc)
	mov.l	r1,@(4,r2)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	00403A04

l004039FE:
	mov.l	(00403AC0),r6                                        ; @(C0,pc)
	bsrf	r6
	mov	r8,r4

l00403A04:
	mov.l	(00403AC4),r11                                       ; @(BC,pc)
	mov.l	@r8,r10
	add	r12,r11
	mov.w	(00403AAE),r14                                       ; @(A0,pc)

l00403A0C:
	tst	r10,r10
	bf	00403A24

l00403A10:
	add	#2C,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r14
	mov.l	@r15+,r13
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	rts
00403A22       F6 68                                       .h            

l00403A24:
	mov.l	@(16,r10),r1
	tst	r1,r1
	bf/s	00403A36
	mov.l	@(12,r10),r0

l00403A2C:
	cmp/eq	#01,r0
	bf/s	00403A38
	mov	r10,r1

l00403A32:
	bra	00403A0C
	mov.l	@(8,r10),r10

l00403A36:
	mov	r10,r1

l00403A38:
	add	#40,r1
	mov.l	@(16,r1),r9
	mov.l	@r11,r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403A52

l00403A44:
	mov.l	(00403AC8),r4                                        ; @(80,pc)
	mov.l	(00403ACC),r1                                        ; @(84,pc)
	add	r12,r4
	mov.l	@(12,r9),r6
	mov.l	@(16,r9),r7
	bsrf	r1
	mov.l	@(40,r8),r5

l00403A52:
	mov.l	(00403AD0),r0                                        ; @(7C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00403AF2
	mov	r9,r2

l00403A5E:
	mov.l	(00403AD4),r0                                        ; @(74,pc)
	add	#40,r2
	mov.l	@(32,r2),r1
	mov.l	@(36,r2),r5
	mov.l	@(r0,r12),r2
	mov.l	@r2,r2
	tst	r2,r2
	bf	00403A72

l00403A6E:
	bra	00403C02
	nop

l00403A72:
	mov	r1,r7
	mov	#E9,r2
	shld	r2,r7
	mov	#07,r2
	mov.l	r2,@(8,r15)
	mov	#20,r2
	mov.l	r2,@(4,r15)
	mov	r1,r6
	mov.l	(00403AD8),r2                                        ; @(54,pc)
	shll8	r5
	mov.l	(00403ADC),r1                                        ; @(54,pc)
	mov	r15,r13
	add	r5,r5
	add	r12,r2
	add	#10,r13
	shll8	r6
	or	r5,r7
	mov.l	r2,@r15
	add	r6,r6
	mov	#05,r5
	bsrf	r1
	mov	r13,r4
	cmp/eq	#FF,r0
	bf/s	00403AE8
	mov	r13,r6

l00403AA4:
	mov.l	(00403AE0),r5                                        ; @(38,pc)
	mov	#01,r4
	mov.l	(00403AE4),r2                                        ; @(38,pc)
	bsrf	r2
	add	r12,r5
	mov.b	r0,@r0
	mov.l	r3,@(0,r7)
	invalid
	mov.l	(00403E68),r15                                       ; @(3B0,pc)
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	mov.l	(00403B28),r15                                       ; @(58,pc)
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	mov	#8A,r0
	invalid
	invalid
	invalid
	mov.l	(00403DC0),r15                                       ; @(2D8,pc)
	invalid

l00403AE8:
	mov.l	(00403CEC),r4                                        ; @(200,pc)
	mov.l	(00403CF0),r1                                        ; @(204,pc)
	add	r12,r4
	bsrf	r1
	mov.l	@(28,r8),r5

l00403AF2:
	mov.l	(00403CF4),r2                                        ; @(200,pc)
	mov	r15,r1
	add	#18,r1
	mov	r1,r5
	mov.l	@(8,r9),r4
	bsrf	r2
	mov.l	r1,@(12,r15)
	mov.l	(00403CF8),r1                                        ; @(1F4,pc)
	mov.l	(00403CFC),r4                                        ; @(1F8,pc)
	mov.l	@(20,r10),r13
	add	r12,r4
	mov.l	@(20,r9),r7
	mov.l	@(44,r8),r6
	bsrf	r1
	mov.l	@(12,r15),r5
	mov.l	(00403D00),r0                                        ; @(1EC,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	mov.l	(00403D04),r1                                        ; @(1E8,pc)
	bf/s	00403B2E
	mov	r1,r4

l00403B1E:
	mov.l	(00403D08),r2                                        ; @(1E8,pc)
	mov.l	@r13,r6
	add	r12,r4
	mov.l	r1,@(12,r15)
	bsrf	r2
	mov.l	@(52,r8),r5
	mov.l	@(12,r15),r1
	mov	r1,r4

l00403B2E:
	mov.l	(00403D0C),r1                                        ; @(1DC,pc)
	add	r12,r4
	mov.l	@(4,r13),r6
	bsrf	r1
	mov.l	@(36,r8),r5
	mov.l	(00403D10),r0                                        ; @(1D4,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403B4E

l00403B42:
	mov.l	(00403D14),r4                                        ; @(1D0,pc)
	mov.l	(00403D18),r2                                        ; @(1D0,pc)
	add	r12,r4
	mov.l	@(8,r13),r6
	bsrf	r2
	mov.l	@(32,r8),r5

l00403B4E:
	mov.l	@(8,r9),r1
	mov.l	(00403D1C),r6                                        ; @(1C8,pc)
	and	r6,r1
	cmp/eq	r14,r1
	bf/s	00403C44
	mov	r9,r1

l00403B5A:
	mov.l	@(32,r9),r2
	mov	#F4,r6
	mov.w	(00403CE8),r1                                        ; @(186,pc)
	mov	r2,r3
	shlr8	r3
	and	r1,r3
	mov	r2,r1
	shld	r6,r1
	mov.l	(00403D20),r6                                        ; @(1B4,pc)
	extu.b	r2,r2
	mov.l	(00403D24),r4                                        ; @(1B4,pc)
	and	r6,r1
	or	r2,r1
	mov.l	r1,@(4,r15)
	mov	#00,r7
	mov.l	@(60,r8),r1
	cmp/gt	r3,r7
	mov.l	r7,@(8,r15)
	mov	r3,r6
	mov.l	r1,@r15
	subc	r7,r7
	mov.l	(00403D28),r1                                        ; @(1A0,pc)
	add	r12,r4
	bsrf	r1
	mov.l	@(56,r8),r5

l00403B8C:
	mov.l	(00403D2C),r0                                        ; @(19C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	00403B9A

l00403B96:
	bra	00403CA2
	nop

l00403B9A:
	mov.l	(00403D30),r6                                        ; @(194,pc)
	mov.l	@(40,r9),r4
	bsrf	r6
	mov.l	@(44,r9),r5

l00403BA2:
	mov.l	(00403D34),r0                                        ; @(190,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	00403BBA

l00403BAC:
	mov.l	(00403D38),r0                                        ; @(188,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	00403BBA

l00403BB6:
	bra	00403CCA
	mov	r10,r4

l00403BBA:
	mov.l	(00403D3C),r6                                        ; @(180,pc)
	bsrf	r6
	mov	r10,r4

l00403BC0:
	mov.l	(00403D40),r0                                        ; @(17C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf/s	00403BE0
	mov.l	@(8,r9),r4

l00403BCC:
	mov.l	(00403D44),r0                                        ; @(174,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403BE6

l00403BD6:
	mov.l	(00403D48),r1                                        ; @(170,pc)
	mov.w	(00403CEA),r2                                        ; @(10E,pc)
	and	r4,r1
	cmp/eq	r2,r1
	bf	00403BE6

l00403BE0:
	mov.l	(00403D4C),r6                                        ; @(168,pc)
	bsrf	r6
	nop

l00403BE6:
	mov.l	(00403D48),r2                                        ; @(160,pc)
	mov.l	@(8,r9),r1
	and	r2,r1
	mov.l	(00403D50),r2                                        ; @(160,pc)
	cmp/eq	r2,r1
	bf	00403BF8

l00403BF2:
	mov.l	(00403D54),r1                                        ; @(160,pc)
	bsrf	r1
	mov	r10,r4

l00403BF8:
	mov.l	(00403D58),r2                                        ; @(15C,pc)
	bsrf	r2
	mov	#0A,r4
	bra	00403A0C
	mov.l	@(8,r10),r10

l00403C02:
	mov.l	(00403D5C),r0                                        ; @(158,pc)
	mov.l	(00403D60),r13                                       ; @(158,pc)
	mov.l	@(r0,r12),r2
	mov.l	@r2,r2
	tst	r2,r2
	bf/s	00403C12
	mov	#00,r7

l00403C10:
	mov.l	(00403D64),r13                                       ; @(150,pc)

l00403C12:
	mov.l	(00403D68),r0                                        ; @(154,pc)
	add	r12,r13
	mov.l	@(r0,r12),r2
	mov.l	@r2,r3
	mov	r3,r2
	add	#FF,r2
	cmp/gt	r2,r7
	mov	r2,r4
	subc	r2,r2
	cmp/gt	r3,r7
	subc	r7,r7
	clrt
	addc	r1,r4
	addc	r2,r5
	mov.l	(00403D6C),r2                                        ; @(13C,pc)
	bsrf	r2
	mov	r3,r6
	mov	r1,r7
	mov.l	(00403D70),r1                                        ; @(138,pc)
	mov	r0,r6
	mov.l	@(28,r8),r5
	bsrf	r1
	mov	r13,r4
	bra	00403AF2
	nop

l00403C44:
	mov.l	(00403D74),r0                                        ; @(12C,pc)
	add	#40,r1
	mov.l	@(24,r1),r6
	mov.l	@(28,r1),r7
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00403C86
	mov	#07,r1

l00403C56:
	mov.l	r1,@(8,r15)
	mov	#20,r1
	mov.l	r1,@(4,r15)
	mov	r15,r13
	mov.l	(00403D78),r1                                        ; @(118,pc)
	add	#10,r13
	mov.l	(00403D7C),r2                                        ; @(118,pc)
	add	r12,r1
	mov.l	r1,@r15
	mov	#05,r5
	bsrf	r2
	mov	r13,r4
	cmp/eq	#FF,r0
	bf/s	00403C78
	mov	r13,r6

l00403C74:
	bra	00403AA4
	nop

l00403C78:
	mov.l	(00403D80),r1                                        ; @(104,pc)
	mov.l	(00403CEC),r4                                        ; @(70,pc)
	mov.l	@(48,r8),r5
	bsrf	r1
	add	r12,r4
	bra	00403B8C
	nop

l00403C86:
	mov.l	(00403D5C),r0                                        ; @(D4,pc)
	mov.l	(00403D60),r4                                        ; @(D4,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf/s	00403C98
	add	r12,r4

l00403C94:
	mov.l	(00403D64),r4                                        ; @(CC,pc)
	add	r12,r4

l00403C98:
	mov.l	(00403D84),r2                                        ; @(E8,pc)
	bsrf	r2
	mov.l	@(48,r8),r5
	bra	00403B8C
	nop

l00403CA2:
	mov.l	(00403D88),r0                                        ; @(E4,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00403CBE
	mov	r9,r1

l00403CAE:
	add	#40,r1
	mov.l	@(0,r1),r4
	mov.l	@(4,r1),r5
	mov.l	(00403D8C),r1                                        ; @(D4,pc)
	bsrf	r1
	nop
	bra	00403BA2
	nop

l00403CBE:
	mov.l	(00403D90),r2                                        ; @(D0,pc)
	mov.l	@(52,r9),r4
	bsrf	r2
	mov.l	@(56,r9),r5
	bra	00403BA2
	nop

l00403CCA:
	mov.l	(00403D94),r0                                        ; @(C8,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403CDE

l00403CD4:
	mov.l	(00403D98),r1                                        ; @(C0,pc)
	bsrf	r1
	nop
	bra	00403BC0
	nop

l00403CDE:
	mov.l	(00403D9C),r2                                        ; @(BC,pc)
	bsrf	r2
	nop
	bra	00403BC0
	nop
00403CE8                         FF 0F 00 40 F4 F4 FE FF         ...@....
00403CF0 76 DE FF FF A4 DD FF FF 58 DE FF FF FC F4 FE FF v.......X.......
00403D00 70 01 00 00 08 F5 FE FF 3E DE FF FF 30 DE FF FF p.......>...0...
00403D10 1C 01 00 00 10 F5 FE FF 1A DE FF FF 00 B0 00 00 ................
00403D20 00 FF 0F 00 18 F5 FE FF DC DD FF FF D4 01 00 00 ................
00403D30 7A FA FF FF 80 01 00 00 24 01 00 00 DC F6 FF FF z.......$.......
00403D40 C8 01 00 00 98 01 00 00 00 F0 00 00 3A FB FF FF ............:...
00403D50 00 A0 00 00 54 F7 FF FF C2 F9 FF FF 38 01 00 00 ....T.......8...
00403D60 E4 F4 FE FF EC F4 FE FF 3C 01 00 00 80 DF FF FF ........<.......
00403D70 28 DD FF FF B8 01 00 00 F0 F3 FE FF BA DE FF FF (...............
00403D80 E6 DC FF FF CA DC FF FF B4 01 00 00 62 F9 FF FF ............b...
00403D90 56 F9 FF FF 8C 01 00 00 1A F6 FF FF 90 F7 FF FF V...............

;; printcol: 00403DA0
printcol proc
	mov.l	r8,@-r15
	mova	(00403F28),r0                                         ; @(184,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	r13,@-r15
	mov.l	r14,@-r15
	mov.l	(00403F28),r12                                       ; @(174,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#EC,r15
	mov.l	(00403F2C),r0                                        ; @(170,pc)
	mov	r4,r13
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00403DCC
	mov.l	@(24,r4),r5

l00403DC6:
	mov.l	@(40,r4),r1
	sett
	addc	r1,r5

l00403DCC:
	mov.l	(00403F30),r0                                        ; @(160,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403DE6

l00403DD6:
	mov.l	(00403F34),r0                                        ; @(15C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403E2A

l00403DE0:
	mov.l	@(48,r13),r1

l00403DE2:
	sett
	addc	r1,r5

l00403DE6:
	mov.l	(00403F38),r0                                        ; @(150,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	00403DFA

l00403DF0:
	mov.l	(00403F3C),r0                                        ; @(148,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403DFC

l00403DFA:
	add	#01,r5

l00403DFC:
	mov.l	(00403F40),r11                                       ; @(140,pc)
	mov	r12,r0
	mov	r5,r10
	mov.l	@(r0,r11),r1
	add	#01,r10
	mov	r10,r2
	mov.l	@r1,r1
	add	r2,r2
	cmp/gt	r1,r2
	bf	00403E2E

l00403E10:
	mov.l	(00403F44),r1                                        ; @(130,pc)
	bsrf	r1
	mov	r13,r4

l00403E16:
	add	#14,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r14
	mov.l	@r15+,r13
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	rts
00403E28                         F6 68                           .h      

l00403E2A:
	bra	00403DE2
	mov.l	@(28,r13),r1

l00403E2E:
	mov.l	(00403F48),r8                                        ; @(118,pc)
	mov.l	@(20,r13),r5
	add	r12,r8
	mov.l	@r8,r1
	cmp/gt	r1,r5
	bf/s	00403E64
	mov	#00,r9

l00403E3C:
	mov.l	(00403F4C),r9                                        ; @(10C,pc)
	shll2	r5
	mov.l	(00403F50),r3                                        ; @(10C,pc)
	add	r12,r9
	bsrf	r3
	mov.l	@r9,r4
	tst	r0,r0
	bf	00403E5C

l00403E4C:
	mov.l	(00403F54),r0                                        ; @(104,pc)
	bsrf	r0
	mov	#00,r4
	mov.l	(00403F58),r1                                        ; @(104,pc)
	bsrf	r1
	mov	r13,r4
	bra	00403E16
	nop

l00403E5C:
	mov.l	@(20,r13),r1
	mov.l	r0,@r9
	mov.l	r1,@r8
	mov	#00,r9

l00403E64:
	mov.l	(00403F4C),r0                                        ; @(E4,pc)
	mov.l	@r13,r1
	mov.l	@(r0,r12),r2

l00403E6A:
	tst	r1,r1
	bf/s	00403EB4
	mov	r10,r5

l00403E70:
	mov	r12,r0
	mov.l	@(r0,r11),r1
	mov.l	(00403F5C),r0                                        ; @(E4,pc)
	mov.l	@r1,r6
	mov.l	@(r0,r12),r8
	jsr	@r8
	mov	r6,r4
	mov	r0,r11
	jsr	@r8
	mov	r0,r5
	mov.l	r0,@(12,r15)
	jsr	@r8
	mov	r9,r4
	mul.l	r11,r0
	sts	macl,r1
	cmp/eq	r1,r9
	bt/s	00403E96
	mov	r0,r8

l00403E94:
	add	#01,r8

l00403E96:
	mov.l	(00403F60),r1                                        ; @(C8,pc)
	mov	r13,r4
	bsrf	r1
	mov	#00,r10
	mov	r8,r3
	shll2	r3
	mov.l	r3,@(16,r15)
	cmp/gt	r10,r8

l00403EA6:
	bf/s	00403E16
	mov	r10,r0

l00403EAA:
	shll2	r0
	mov.l	r0,@(8,r15)
	mov	r10,r14
	bra	00403F04
	mov	#00,r2

l00403EB4:
	mov.l	@(16,r1),r3
	tst	r3,r3
	bf/s	00403EC0
	mov.l	@(12,r1),r0

l00403EBC:
	cmp/eq	#01,r0
	bt	00403EC8

l00403EC0:
	mov	r9,r0
	shll2	r0
	mov.l	r1,@(r0,r2)
	add	#01,r9

l00403EC8:
	bra	00403E6A
	mov.l	@(8,r1),r1

l00403ECC:
	mov.l	(00403F4C),r0                                        ; @(7C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@(8,r15),r0
	mov.l	@(r0,r1),r4
	mov.l	(00403F34),r0                                        ; @(5C,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00403F14
	mov.l	@(40,r13),r5

l00403EE0:
	mov.l	@(48,r13),r6

l00403EE2:
	add	r8,r14
	mov.l	(00403F64),r1                                        ; @(7C,pc)
	bsrf	r1
	mov.l	r2,@r15
	cmp/gt	r14,r9
	bf/s	00403F08
	mov.l	@r15,r2

l00403EF0:
	mov	r0,r1

l00403EF2:
	mov.l	@(12,r15),r0
	cmp/ge	r0,r1
	bf/s	00403F18
	mov	#20,r4

l00403EFA:
	mov.l	@(8,r15),r0
	add	#01,r2
	mov.l	@(16,r15),r1
	add	r1,r0
	mov.l	r0,@(8,r15)

l00403F04:
	cmp/ge	r11,r2
	bf	00403ECC

l00403F08:
	mov.l	(00403F68),r3                                        ; @(5C,pc)
	mov	#0A,r4
	bsrf	r3
	add	#01,r10
	bra	00403EA6
	cmp/gt	r10,r8

l00403F14:
	bra	00403EE2
	mov.l	@(28,r13),r6

l00403F18:
	mov.l	(00403F6C),r3                                        ; @(50,pc)
	mov.l	r1,@(4,r15)
	bsrf	r3
	mov.l	r2,@r15
	mov.l	@(4,r15),r1
	mov.l	@r15,r2
	bra	00403EF2
	add	#01,r1
00403F28                         B8 12 01 00 40 01 00 00         ....@...
00403F30 60 01 00 00 B8 01 00 00 C8 01 00 00 98 01 00 00 `...............
00403F40 A8 01 00 00 6A FB FF FF FC FF FF FF FC 01 00 00 ....j...........
00403F50 44 D9 FF FF 96 DE FF FF 28 FB FF FF 34 01 00 00 D.......(...4...
00403F60 26 F6 FF FF DE F8 FF FF B0 F6 FF FF A0 F6 FF FF &...............

;; printacol: 00403F70
printacol proc
	mov.l	r8,@-r15
	mova	(00404080),r0                                         ; @(10C,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	r13,@-r15
	mov.l	r14,@-r15
	mov.l	(00404080),r12                                       ; @(FC,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#FC,r15
	mov.l	(00404084),r0                                        ; @(F8,pc)
	mov	r4,r8
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00403F9C
	mov.l	@(24,r4),r5

l00403F96:
	mov.l	@(40,r4),r1
	sett
	addc	r1,r5

l00403F9C:
	mov.l	(00404088),r0                                        ; @(E8,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403FB6

l00403FA6:
	mov.l	(0040408C),r0                                        ; @(E4,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403FF6

l00403FB0:
	mov.l	@(48,r8),r1

l00403FB2:
	sett
	addc	r1,r5

l00403FB6:
	mov.l	(00404090),r0                                        ; @(D8,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bf	00403FCA

l00403FC0:
	mov.l	(00404094),r0                                        ; @(D0,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00403FCC

l00403FCA:
	add	#01,r5

l00403FCC:
	mov.l	(00404098),r0                                        ; @(C8,pc)
	add	#01,r5
	mov.l	@(r0,r12),r1
	mov.l	@r1,r7
	mov	r5,r1
	add	r1,r1
	cmp/gt	r7,r1
	bf	00403FFA

l00403FDC:
	mov.l	(0040409C),r0                                        ; @(BC,pc)
	bsrf	r0
	mov	r8,r4
	add	#04,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r14
	mov.l	@r15+,r13
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	rts
00403FF4             F6 68                                   .h          

l00403FF6:
	bra	00403FB2
	mov.l	@(28,r8),r1

l00403FFA:
	mov.l	(004040A0),r0                                        ; @(A4,pc)
	mov	r7,r4
	mov.l	(0040408C),r14                                       ; @(8C,pc)
	mov.l	@(r0,r12),r11
	jsr	@r11
	mov	#00,r10
	mov	r0,r5
	jsr	@r11
	mov	r0,r13
	mov.l	(004040A4),r1                                        ; @(94,pc)
	mov	r8,r4
	bsrf	r1
	mov.l	r0,@r15
	mov.l	@r8,r9

l00404016:
	tst	r9,r9
	bf/s	00404032
	mov	#0A,r4

l0040401C:
	mov.l	(004040A8),r1                                        ; @(88,pc)
	add	#04,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r14
	mov.l	@r15+,r13
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	braf	r1
	mov.l	@r15+,r8

l00404032:
	mov.l	@(16,r9),r1
	tst	r1,r1
	bf/s	0040403E
	mov.l	@(12,r9),r0

l0040403A:
	cmp/eq	#01,r0
	bt	0040406A

l0040403E:
	cmp/ge	r13,r10
	bf/s	0040404E
	mov	r12,r0

l00404044:
	mov.l	(004040AC),r1                                        ; @(64,pc)
	mov	#0A,r4
	bsrf	r1
	mov	#00,r10
	mov	r12,r0

l0040404E:
	mov.l	@(r0,r14),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	0040406E
	mov.l	@(40,r8),r5

l00404058:
	mov.l	@(48,r8),r6

l0040405A:
	mov.l	(004040B0),r1                                        ; @(54,pc)
	bsrf	r1
	mov	r9,r4
	mov	r0,r11
	mov.l	@r15,r0

l00404064:
	cmp/gt	r11,r0
	bt	00404072

l00404068:
	add	#01,r10

l0040406A:
	bra	00404016
	mov.l	@(8,r9),r9

l0040406E:
	bra	0040405A
	mov.l	@(28,r8),r6

l00404072:
	mov.l	(004040B4),r1                                        ; @(40,pc)
	mov	#20,r4
	bsrf	r1
	add	#01,r11
	bra	00404064
	mov.l	@r15,r0
0040407E                                           09 00               ..
00404080 60 11 01 00 40 01 00 00 60 01 00 00 B8 01 00 00 `...@...`.......
00404090 C8 01 00 00 98 01 00 00 A8 01 00 00 9E F9 FF FF ................
004040A0 34 01 00 00 B0 F4 FF FF 8E F5 FF FF 74 F5 FF FF 4...........t...
004040B0 68 F7 FF FF 46 F5 FF FF                         h...F...        

;; printstream: 004040B8
printstream proc
	mov.l	r8,@-r15
	mova	(00404184),r0                                         ; @(C8,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	r13,@-r15
	mov.l	(00404184),r12                                       ; @(BC,pc)
	mov.l	r14,@-r15
	add	r0,r12
	sts.l	pr,@-r15
	mov.l	(00404188),r0                                        ; @(B8,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r8
	tst	r8,r8
	bt/s	004040DE
	mov	r4,r11

l004040DA:
	mov.l	@(40,r4),r8
	add	#01,r8

l004040DE:
	mov.l	(0040418C),r0                                        ; @(AC,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	004040F8

l004040E8:
	mov.l	(00404190),r0                                        ; @(A4,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	0040412A

l004040F2:
	mov.l	@(48,r11),r1

l004040F4:
	sett
	addc	r1,r8

l004040F8:
	mov.l	(00404194),r0                                        ; @(98,pc)
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt	00404104

l00404102:
	add	#01,r8

l00404104:
	mov.l	(00404190),r13                                       ; @(88,pc)
	mov	#00,r10
	mov.l	(00404198),r14                                       ; @(8C,pc)
	add	r12,r13
	mov.l	@r11,r9
	add	r12,r14

l00404110:
	tst	r9,r9
	bf/s	0040412E
	mov	#0A,r4

l00404116:
	mov.l	(0040419C),r1                                        ; @(84,pc)
	lds.l	@r15+,pr
	mov.l	@r15+,r14
	mov.l	@r15+,r13
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	braf	r1
	mov.l	@r15+,r8

l0040412A:
	bra	004040F4
	mov.l	@(28,r11),r1

l0040412E:
	mov.l	@(16,r9),r1
	tst	r1,r1
	bf/s	0040413A
	mov.l	@(12,r9),r0

l00404136:
	cmp/eq	#01,r0
	bt	00404172

l0040413A:
	cmp/pl	r10
	bf/s	0040415E
	mov	#2C,r4

l00404140:
	mov.l	(004041A0),r1                                        ; @(5C,pc)
	bsrf	r1
	add	#02,r10
	mov.l	@(44,r9),r2
	mov	r10,r1
	add	r8,r1
	add	r2,r1
	mov.l	@r14,r2
	mov.l	@r2,r2
	cmp/ge	r2,r1
	bf/s	00404176
	mov	#0A,r4

l00404158:
	mov.l	(004041A4),r1                                        ; @(48,pc)
	bsrf	r1
	mov	#00,r10

l0040415E:
	mov.l	@r13,r1

l00404160:
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00404180
	mov.l	@(40,r11),r5

l00404168:
	mov.l	@(48,r11),r6

l0040416A:
	mov.l	(004041A8),r1                                        ; @(3C,pc)
	bsrf	r1
	mov	r9,r4
	add	r0,r10

l00404172:
	bra	00404110
	mov.l	@(8,r9),r9

l00404176:
	mov.l	(004041AC),r1                                        ; @(34,pc)
	bsrf	r1
	mov	#20,r4
	bra	00404160
	mov.l	@r13,r1

l00404180:
	bra	0040416A
	mov.l	@(28,r11),r6
00404184             5C 10 01 00 40 01 00 00 60 01 00 00     \...@...`...
00404190 B8 01 00 00 C8 01 00 00 A8 01 00 00 96 F4 FF FF ................
004041A0 7A F4 FF FF 62 F4 FF FF 58 F6 FF FF 44 F4 FF FF z...b...X...D...

;; printwc: 004041B0
printwc proc
	mov.l	r8,@-r15
	mova	(00404224),r0                                         ; @(70,pc)
	mov.l	r9,@-r15
	mov.l	r12,@-r15
	mov.l	(00404224),r12                                       ; @(68,pc)
	sts.l	pr,@-r15
	add	r0,r12
	add	#E0,r15
	mov.l	(00404228),r1                                        ; @(64,pc)
	mov	r5,r6
	mov	r4,r8
	mov	r4,r5
	bsrf	r1
	mov	r15,r4
	cmp/eq	#FF,r0
	mov	r15,r9
	bf/s	004041E2
	mov	r0,r6

l004041D4:
	mov	#00,r0

l004041D6:
	add	#20,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r12
	mov.l	@r15+,r9
	rts
004041E0 F6 68                                           .h              

l004041E2:
	tst	r8,r8
	bf/s	0040421C
	tst	r0,r0

l004041E8:
	bt/s	004041D4
	mov	r0,r1

l004041EC:
	add	#FF,r1
	mov	r1,r0
	mov.b	@(r0,r9),r2
	tst	r2,r2
	bf/s	004041FE
	tst	r1,r1

l004041F8:
	bt/s	004041D6
	mov	#00,r0

l004041FC:
	mov	r1,r6

l004041FE:
	mov.l	(0040422C),r1                                        ; @(2C,pc)
	mov	r12,r0
	mov	#01,r5
	mov.l	@(r0,r1),r7
	mov	r15,r4
	mov.l	(00404230),r1                                        ; @(24,pc)
	bsrf	r1
	add	#58,r7
	tst	r8,r8
	bt	004041D4

l00404212:
	mov.l	(00404234),r1                                        ; @(20,pc)
	bsrf	r1
	mov	r8,r4
	bra	004041D6
	nop

l0040421C:
	bt	00404212

l0040421E:
	bra	004041FE
	nop
00404222       09 00 BC 0F 01 00 A4 D5 FF FF 54 01 00 00   ..........T...
00404230 76 D7 FF FF C4 D6 FF FF                         v.......        

;; safe_print: 00404238
safe_print proc
	mov.l	r8,@-r15
	mova	(004042C4),r0                                         ; @(88,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r12,@-r15
	mov.l	(004042C4),r12                                       ; @(80,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov.l	(004042C8),r0                                        ; @(7C,pc)
	mov	r4,r8
	mov.l	@(r0,r12),r1
	mov.l	@r1,r1
	tst	r1,r1
	bt/s	00404258
	mov	#1D,r9

l00404256:
	mov	#1F,r9

l00404258:
	mov.l	(004042CC),r1                                        ; @(70,pc)
	bsrf	r1
	mov	r8,r4
	tst	r0,r0
	bt/s	00404280
	mov	r0,r2

l00404264:
	mov	r0,r5
	mov.l	(004042D0),r0                                        ; @(68,pc)
	add	r12,r0
	jsr	@r0
	mov	#FF,r4
	mov	#04,r1
	cmp/hi	r1,r0
	bt/s	00404280
	mov	r8,r6

l00404276:
	mov.l	(004042D4),r5                                        ; @(5C,pc)
	mov.l	(004042D8),r1                                        ; @(5C,pc)
	add	r12,r5
	bsrf	r1
	mov	#01,r4

l00404280:
	mov.l	(004042DC),r1                                        ; @(58,pc)
	mov	r2,r4
	shll2	r4
	bsrf	r1
	add	#01,r4
	tst	r0,r0
	bf/s	0040429A
	mov	r0,r10

l00404290:
	mov.l	(004042E0),r5                                        ; @(4C,pc)
	mov	#01,r4
	mov.l	(004042E4),r1                                        ; @(4C,pc)
	bsrf	r1
	add	r12,r5

l0040429A:
	mov.l	(004042E8),r1                                        ; @(4C,pc)
	mov	r9,r6
	mov	r8,r5
	bsrf	r1
	mov	r0,r4
	mov.l	(004042EC),r1                                        ; @(44,pc)
	mov	r0,r8
	mov.l	(004042F0),r4                                        ; @(44,pc)
	mov	r10,r5
	bsrf	r1
	add	r12,r4
	mov.l	(004042F4),r1                                        ; @(40,pc)
	bsrf	r1
	mov	r10,r4
	mov	r8,r0
	lds.l	@r15+,pr
	mov.l	@r15+,r12
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	rts
004042C2       F6 68 1C 0F 01 00 24 01 00 00 A6 DA FF FF   .h....$.......
004042D0 06 F3 FE FF 28 F5 FE FF D0 D5 FF FF E2 D5 FF FF ....(...........
004042E0 3C F5 FE FF B6 D5 FF FF A0 D8 FF FF B8 D6 FF FF <...............
004042F0 A4 F3 FE FF 36 D9 FF FF                         ....6...        

;; printescaped: 004042F8
printescaped proc
	mov.l	r8,@-r15
	mova	(004043F4),r0                                         ; @(F8,pc)
	mov.l	r9,@-r15
	mov.l	r10,@-r15
	mov.l	r11,@-r15
	mov.l	r12,@-r15
	mov.l	r13,@-r15
	mov.l	r14,@-r15
	mov.w	(004043EC),r1                                        ; @(E0,pc)
	mov.l	(004043F4),r12                                       ; @(E8,pc)
	sts.l	pr,@-r15
	add	r0,r12
	sub	r1,r15
	mov.l	(004043F8),r1                                        ; @(E4,pc)
	mov	r4,r8
	bsrf	r1
	mov	r8,r10
	mov.l	(004043FC),r3                                        ; @(E0,pc)
	sett
	mov	r15,r13
	addc	r0,r10
	add	#14,r13
	mov.w	(004043EE),r6                                        ; @(C6,pc)
	mov	#00,r5
	mov.w	(004043F0),r11                                       ; @(C4,pc)
	bsrf	r3
	mov	r13,r4
	mov.l	(00404400),r1                                        ; @(D0,pc)
	add	r15,r11
	mov.w	(004043EE),r6                                        ; @(B8,pc)
	mov	#00,r5
	bsrf	r1
	mov	r11,r4
	mov.l	(00404404),r3                                        ; @(C8,pc)
	mov	r15,r2
	mov	r15,r1
	add	#10,r2
	add	#D4,r1
	add	r12,r3
	mov.l	r2,@(4,r15)
	mov	#00,r9
	mov.l	r1,@(8,r15)
	mov.l	r3,@(12,r15)

l0040434E:
	cmp/hs	r10,r8

l00404350:
	bt/s	00404378
	mov	r10,r2

l00404354:
	mov.l	(00404408),r1                                        ; @(B0,pc)
	sub	r8,r2
	mov	r15,r4
	mov	r2,r6
	mov.l	r2,@r15
	mov	r13,r7
	mov	r8,r5
	bsrf	r1
	add	#10,r4
	tst	r0,r0
	mov	r0,r14
	bf/s	00404390
	mov.l	@r15,r2

l0040436E:
	mov.l	(0040440C),r3                                        ; @(9C,pc)
	mov	r11,r5
	bsrf	r3
	mov.l	@(16,r15),r4
	add	r0,r9

l00404378:
	mov	r9,r0
	mov.w	(004043EC),r7                                        ; @(6E,pc)
	add	r7,r15
	lds.l	@r15+,pr
	mov.l	@r15+,r14
	mov.l	@r15+,r13
	mov.l	@r15+,r12
	mov.l	@r15+,r11
	mov.l	@r15+,r10
	mov.l	@r15+,r9
	rts
0040438E                                           F6 68               .h

l00404390:
	cmp/eq	#FF,r0
	bf/s	004043AE
	mov	r11,r5

l00404396:
	mov.l	(00404410),r1                                        ; @(78,pc)
	bsrf	r1
	mov	#3F,r4
	mov.l	(00404414),r2                                        ; @(74,pc)
	add	r0,r9
	mov.w	(004043EE),r6                                        ; @(4A,pc)
	mov	#00,r5
	mov	r13,r4
	bsrf	r2
	add	#01,r8
	bra	00404350
	cmp/hs	r10,r8

l004043AE:
	cmp/eq	#FE,r0
	bf/s	004043C8
	mov.l	@(12,r15),r3

l004043B4:
	mov.l	@r3,r1
	mov.l	@r1,r1
	cmp/hi	r2,r1
	bf/s	004043E8
	mov	r11,r5

l004043BE:
	mov.l	(00404418),r1                                        ; @(58,pc)
	bsrf	r1
	mov	#3F,r4
	bra	00404378
	add	r0,r9

l004043C8:
	mov.l	@(16,r15),r2
	mov.l	(0040441C),r3                                        ; @(50,pc)
	mov	r2,r4
	bsrf	r3
	mov.l	r2,@r15
	tst	r0,r0
	bf/s	004043DA
	mov.l	@r15,r2

l004043D8:
	mov	#3F,r2

l004043DA:
	mov.l	(00404420),r1                                        ; @(44,pc)
	mov	r11,r5
	bsrf	r1
	mov	r2,r4
	add	r0,r9
	bra	0040434E
	add	r14,r8

l004043E8:
	bra	0040434E
	mov	r10,r8
004043EC                                     14 01 80 00             ....
004043F0 94 00 09 00 EC 0D 01 00 EA D9 FF FF 06 D5 FF FF ................
00404400 FA D4 FF FF D0 01 00 00 16 D8 FF FF 3A FE FF FF ............:...
00404410 14 FE FF FF 8A D4 FF FF EC FD FF FF CA D6 FF FF ................
00404420 CE FD FF FF                                     ....            

;; __sdivsi3: 00404424
__sdivsi3 proc
	mov	r4,r1
	mov	r5,r0
	tst	r0,r0
	bt	004044BE

l0040442C:
	mov	#00,r2
	div0s	r2,r1
	subc	r3,r3
	subc	r2,r1
	div0s	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	div1	r0,r3
	rotcl	r1
	addc	r2,r1
	rts
004044BC                                     13 60                   .`  

l004044BE:
	rts
004044C0 00 E0 09 00                                     ....            

;; fn004044C4: 004044C4
;;   Called from:
;;     004044F4 (in __udivsi3)
;;     00404500 (in __udivsi3)
fn004044C4 proc
	div1	r5,r4

;; fn004044C6: 004044C6
;;   Called from:
;;     004044F8 (in __udivsi3)
;;     00404504 (in __udivsi3)
fn004044C6 proc
	div1	r5,r4
	div1	r5,r4
	div1	r5,r4
	div1	r5,r4
	div1	r5,r4
	div1	r5,r4
	rts
004044D4             54 34                                   T4          

;; fn004044D6: 004044D6
;;   Called from:
;;     0040451A (in __udivsi3)
;;     0040451E (in __udivsi3)
;;     00404522 (in __udivsi3)
;;     00404526 (in __udivsi3)
fn004044D6 proc
	div1	r5,r4
	rotcl	r0
	div1	r5,r4
	rotcl	r0
	div1	r5,r4
	rotcl	r0
	rts
004044E4             54 34                                   T4          

;; __udivsi3: 004044E6
__udivsi3 proc
	sts.l	pr,@-r15
	extu.w	r5,r0
	cmp/eq	r5,r0
	bf/s	00404514
	div0u

l004044F0:
	swap.w	r4,r0
	shlr16	r4
	bsr	fn004044C4
	shll16	r5
	bsr	fn004044C6
	div1	r5,r4
	xtrct	r4,r0
	xtrct	r0,r4
	bsr	fn004044C4
	swap.w	r4,r4
	bsr	fn004044C6
	div1	r5,r4
	lds.l	@r15+,pr
	xtrct	r4,r0
	swap.w	r0,r0
	rotcl	r0
	rts
00404512       29 45                                       )E            

l00404514:
	mov	#00,r0
	xtrct	r4,r0
	xtrct	r0,r4
	bsr	fn004044D6
	rotcl	r0
	bsr	fn004044D6
	rotcl	r0
	bsr	fn004044D6
	rotcl	r0
	bsr	fn004044D6
	rotcl	r0
	lds.l	@r15+,pr
	rts
0040452E                                           24 40               $@

;; main: 00404530
main proc
	mov.l	r12,@-r15
	mova	(00404548),r0                                         ; @(14,pc)
	mov.l	(00404548),r12                                       ; @(10,pc)
	sts.l	pr,@-r15
	add	r0,r12
	mov.l	(0040454C),r1                                        ; @(10,pc)
	bsrf	r1
	nop
	lds.l	@r15+,pr
	rts
00404544             F6 6C 09 00 98 0C 01 00 C0 E6 FF FF     .l..........
