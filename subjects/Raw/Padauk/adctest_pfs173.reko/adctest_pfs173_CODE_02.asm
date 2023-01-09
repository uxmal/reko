;;; Segment CODE_02 (0012)

;; fn0012: 0012
;;   Called from:
;;     0006 (in fn0000)
fn0012 proc
	mov	a,0x2
	mov	[0x0],a
	goto	0019

l0015:
	mov	a,0x0
	idxm	[[0x0]],a
	inc	[0x0]
	mov	a,0x2

l0019:
	add	a,0xB1
	ceqsn	a,[0x0]

l001B:
	goto	0015

l001C:
	goto	0011
001D                          0072 1F00 0072           .r...r
0020 0185 5440 5300 6034 3B05 1F02 1D03 5300 ..T@S.`4;.....S.
0028 6034 1F03 1700 1F02 5401 5300 6031 3F90 `4......T.S.`1?.
0030 6032 3B90 2A03 2C02 0073 1700 0073 007B `2;.*.,..s...s.{

;; fn0038: 0038
;;   Called from:
;;     0120 (in fn0112)
;;     012C (in fn0112)
fn0038 proc
	mov	a,[0x2]
	or	a,[0x3]
	ceqsn	a,0x0

l003B:
	goto	fn0038

l003C:
	set0	IO(0x4).6
	mov	a,[0x4]
	mov	[0xB3],a
	mov	a,[0x5]
	mov	[0xB4],a
	sl	[0xB3]
	slc	[0xB4]
	mov	a,[0xB3]
	mov	[0x0],a
	mov	a,[0xB4]
	or	a,0x2
	mov	[0x3],a
	mov	a,[0x0]
	mov	[0x2],a
	set1	IO(0x4).6
	mov	a,[0x5]
	mov	[0x0],a
	mov	a,[0x4]
	ret

;; fn004F: 004F
;;   Called from:
;;     0006 (in fn0000)
fn004F proc
	mov	a,0x34
	mov	IO(0x3),a
	call	0BED
	mov	IO(0xB),a
	call	0BEE
	mov	IO(0x63),a
	ret	0x0

l0056:
	mov	a,0x20
	mov	IO(0x30),a
	mov	a,0x1
	mov	IO(0x32),a
	mov	a,0x44
	mov	IO(0x33),a
	mov	a,0x80
	mov	IO(0x11),a
	mov	a,0x5F
	mov	[0x2],a
	mov	a,0xD5
	mov	[0x3],a
	mov	a,0x40
	mov	IO(0x4),a
	engint
	mov	a,0x0
	mov	IO(0x24),a
	mov	a,0x8

;; fn0068: 0068
;;   Called from:
;;     0067 (in fn0012)
fn0068 proc
	mov	IO(0x21),a
	mov	a,0xBC
	mov	IO(0x20),a
	mov	a,0xDE
	pushaf
	mov	a,IO(0x2)
	add	a,0xFF
	mov	[0x0],a
	mov	a,0x85
	idxm	[[0x0]],a
	call	fn0146
	mov	a,IO(0x2)
	add	a,0xFE
	mov	IO(0x2),a
	set1	IO(0x20).6

l0077:
	mov	a,IO(0x20)
	and	a,0x40
	cneqsn	a,0x0

l007A:
	goto	0077

l007B:
	mov	a,IO(0x22)
	mov	[0x12],a
	clear	[0x13]
	clear	[0x14]
	clear	[0x15]
	mov	a,0x50
	mov	[0xE],a
	mov	a,0xAB
	mov	[0xF],a
	mov	a,0x4
	mov	[0x10],a
	clear	[0x11]
	mov	a,0x6
	pushaf
	call	fn00CE
	mov	a,IO(0x2)
	add	a,0xFE
	mov	IO(0x2),a
	mov	a,[0x6]
	mov	[0xA],a
	mov	a,[0x7]
	mov	[0xB],a
	mov	a,[0x8]
	mov	[0xC],a
	mov	a,[0x9]
	mov	[0xD],a
	mov	a,[0xA]
	pushaf
	mov	a,IO(0x2)
	add	a,0xFF
	mov	[0x0],a
	mov	a,[0xB]
	idxm	[[0x0]],a
	mov	a,[0xC]
	pushaf
	inc	[0x0]
	inc	[0x0]
	mov	a,[0xD]
	idxm	[[0x0]],a
	mov	a,0xF9
	pushaf
	inc	[0x0]
	inc	[0x0]
	mov	a,0x85
	idxm	[[0x0]],a
	call	fn0146
	mov	a,IO(0x2)
	add	a,0xFA
	mov	IO(0x2),a
	set0	IO(0x11).0
	set0	IO(0x12).0
	set0	IO(0xD).0
	mov	a,0xA8
	mov	IO(0x20),a
	mov	a,0x1
	mov	[0x16],a
	mov	a,0x86
	mov	[0x17],a
	call	fn0112

l00B6:
	set1	IO(0x20).6

l00B7:
	mov	a,IO(0x20)
	and	a,0x40
	cneqsn	a,0x0

l00BA:
	goto	00B7

l00BB:
	mov	a,IO(0x22)
	clear	[0x0]
	pushaf
	mov	a,IO(0x2)
	add	a,0xFF
	xch	[0x0]
	idxm	[[0x0]],a
	mov	a,0x13
	pushaf
	inc	[0x0]
	inc	[0x0]
	mov	a,0x86
	idxm	[[0x0]],a
	call	fn0146
	mov	a,IO(0x2)
	add	a,0xFC
	mov	IO(0x2),a
	goto	00B6
00CD                          007A                     .z    

;; fn00CE: 00CE
;;   Called from:
;;     0089 (in fn0068)
fn00CE proc
	clear	[0xB3]
	clear	[0xB4]
	clear	[0xB5]
	clear	[0xB6]
	mov	a,0x20
	mov	[0x0],a

l00D4:
	mov	a,[0x11]
	mov	[0xB7],a
	clear	[0xB8]
	clear	[0xB9]
	clear	[0xBA]
	mov	a,0x7

l00DA:
	sr	[0xB7]
	dzsn	a

l00DC:
	goto	00DA

l00DD:
	mov	a,[0xB7]
	and	a,0x1
	sl	[0xE]
	slc	[0xF]
	slc	[0x10]
	slc	[0x11]
	sl	[0xB3]
	slc	[0xB4]
	slc	[0xB5]
	slc	[0xB6]
	cneqsn	a,0x0

l00E8:
	goto	00EB

l00E9:
	mov	a,0x1
	or	[0xB3],a

l00EB:
	mov	a,[0xB3]
	sub	a,[0x12]
	mov	a,[0xB4]
	subc	a,[0x13]
	mov	a,[0xB5]
	subc	a,[0x14]
	mov	a,[0xB6]
	subc	a,[0x15]
	t0sn	IO(0x0).1

l00F4:
	goto	00FF

l00F5:
	mov	a,[0x12]
	sub	[0xB3],a
	mov	a,[0x13]
	subc	[0xB4],a
	mov	a,[0x14]
	subc	[0xB5],a
	mov	a,[0x15]
	subc	[0xB6],a
	mov	a,0x1
	or	[0xE],a

l00FF:
	dzsn	[0x0]

l0100:
	goto	00D4

l0101:
	mov	a,IO(0x2)
	add	a,0xFC
	mov	[0x0],a
	idxm	a,[[0x0]]
	mov	[0x0],a
	mov	a,[0xE]
	idxm	[[0x0]],a
	inc	[0x0]
	mov	a,[0xF]
	idxm	[[0x0]],a
	inc	[0x0]
	mov	a,[0x10]
	idxm	[[0x0]],a
	inc	[0x0]
	mov	a,[0x11]
	idxm	[[0x0]],a
	ret

;; fn0112: 0112
;;   Called from:
;;     00B5 (in fn0068)
fn0112 proc
	mov	a,[0x16]
	mov	[0x18],a
	mov	a,[0x17]
	mov	[0x19],a

l0116:
	mov	a,[0x18]
	mov	[0x0],a
	mov	a,[0x19]
	call	fn059D
	cneqsn	a,0x0

l011B:
	goto	0129

l011C:
	inc	[0x18]
	addc	[0x19]
	mov	[0x4],a
	clear	[0x5]
	call	fn0038
	ceqsn	a,0xFF

l0122:
	goto	0116

l0123:
	mov	a,[0x0]
	ceqsn	a,0xFF

l0125:
	goto	0116

l0126:
	mov	a,0xFF
	mov	[0x0],a
	ret	0xFF

l0129:
	mov	a,0xA
	mov	[0x4],a
	clear	[0x5]
	goto	fn0038
012D                          007A 0182 50FC           .z..P.
0130 1700 0701 1704 2605 6038 007A 572E 172E ......&.`8.zW...
0138 5701 172F 2630 2631 1F1A 1732 1F1B 1733 W../&0&1...2...3
0140 1F1C 1734 1F1D 1735 61C7 007A           ...4...5a..z    

;; fn0146: 0146
;;   Called from:
;;     0072 (in fn0068)
;;     00A8 (in fn0068)
;;     00C8 (in fn0068)
fn0146 proc
	mov	a,IO(0x2)
	add	a,0xFC
	mov	[0x34],a
	clear	[0x35]
	mov	a,0x2E
	mov	[0x2E],a
	mov	a,0x1
	mov	[0x2F],a
	clear	[0x30]
	clear	[0x31]
	mov	a,IO(0x2)
	add	a,0xFC
	mov	[0x0],a
	idxm	a,[[0x0]]
	mov	[0x32],a
	inc	[0x0]
	idxm	a,[[0x0]]
	mov	[0x33],a
	call	fn01C7
	ret

;; fn015A: 015A
;;   Called from:
;;     0187 (in fn0178)
;;     020A (in fn01C7)
;;     02BD (in fn01C7)
;;     02F8 (in fn01C7)
;;     0316 (in fn01C7)
;;     033C (in fn01C7)
;;     034D (in fn01C7)
;;     0350 (in fn01C7)
;;     036B (in fn01C7)
;;     03B2 (in fn01C7)
;;     04D0 (in fn01C7)
;;     04DD (in fn01C7)
;;     04EA (in fn01C7)
;;     04F3 (in fn01C7)
;;     0512 (in fn01C7)
;;     0562 (in fn01C7)
;;     0566 (in fn01C7)
fn015A proc
	mov	a,[0x21]
	pushaf
	mov	a,IO(0x2)
	add	a,0xFF
	mov	[0x0],a
	mov	a,[0x22]
	idxm	[[0x0]],a
	mov	a,[0x2A]
	pushaf
	mov	a,0x72
	pushaf
	mov	a,IO(0x2)
	mov	[0x0],a
	dec	[0x0]
	mov	a,0x1
	idxm	[[0x0]],a
	mov	a,[0x1F]
	pushaf
	mov	a,IO(0x2)
	add	a,0xFF
	mov	[0x0],a
	mov	a,[0x20]
	idxm	[[0x0]],a
	ret
0172           0182 50FC 0102 2428 2029 007A     ..P...$( ).z

;; fn0178: 0178
;;   Called from:
;;     018F (in fn0189)
;;     0193 (in fn0189)
;;     054F (in fn01C7)
fn0178 proc
	mov	a,[0x2B]
	add	a,0x30
	mov	[0x0],a
	mov	a,0x39
	sub	a,[0x0]
	t1sn	IO(0x0).1

l017E:
	goto	0185

l017F:
	mov	a,0x7
	add	[0x0],a
	mov	a,[0x1E]
	cneqsn	a,0x0

l0183:
	goto	0185

l0184:
	set1	[0x0].5

l0185:
	mov	a,[0x0]
	mov	[0x2A],a
	goto	fn015A
0188 007A                                    .z              

;; fn0189: 0189
;;   Called from:
;;     0353 (in fn01C7)
;;     0356 (in fn01C7)
fn0189 proc
	mov	a,[0x2C]
	mov	[0x2B],a
	sr	[0x2B]
	sr	[0x2B]
	sr	[0x2B]
	sr	[0x2B]
	call	fn0178
	mov	a,[0x2C]
	and	a,0xF
	mov	[0x2B],a
	goto	fn0178
0194                     007A                        .z      

;; fn0195: 0195
;;   Called from:
;;     047B (in fn01C7)
fn0195 proc
	mov	a,[0x23]
	mov	[0xB3],a
	mov	a,[0x24]
	mov	[0xB4],a
	mov	a,[0x25]
	mov	[0xB5],a
	mov	a,[0x26]
	mov	[0xB6],a
	mov	a,[0x27]
	mov	[0x0],a
	mov	a,0x20
	mov	[0xB7],a

l01A1:
	sl	[0x0]
	mov	a,[0xB6]
	mov	[0xB8],a
	clear	[0xB9]
	clear	[0xBA]
	clear	[0xBB]
	mov	a,0x7

l01A8:
	sr	[0xB8]
	dzsn	a

l01AA:
	goto	01A8

l01AB:
	mov	a,[0xB8]
	and	a,0x1
	or	[0x0],a
	sl	[0xB3]
	slc	[0xB4]
	slc	[0xB5]
	slc	[0xB6]
	mov	a,[0x0]
	sub	a,[0x2D]
	t0sn	IO(0x0).1

l01B5:
	goto	01BA

l01B6:
	mov	a,[0x2D]
	sub	[0x0],a
	mov	a,0x1
	or	[0xB3],a

l01BA:
	dzsn	[0xB7]

l01BB:
	goto	01A1

l01BC:
	mov	a,[0xB3]
	mov	[0x23],a
	mov	a,[0xB4]
	mov	[0x24],a
	mov	a,[0xB5]
	mov	[0x25],a
	mov	a,[0xB6]
	mov	[0x26],a
	mov	a,[0x0]
	mov	[0x27],a
	ret

;; fn01C7: 01C7
;;   Called from:
;;     0158 (in fn0146)
fn01C7 proc
	mov	a,[0x2E]
	mov	[0x1F],a
	mov	a,[0x2F]
	mov	[0x20],a
	mov	a,[0x30]
	mov	[0x21],a
	mov	a,[0x31]
	mov	[0x22],a
	clear	[0x28]
	clear	[0x29]

l01D1:
	mov	a,[0x32]
	mov	[0x3C],a
	mov	a,[0x33]
	mov	[0x3D],a
	mov	a,[0x3C]
	mov	[0x0],a
	mov	a,[0x3D]
	call	fn059D
	pushaf
	mov	a,[0x3C]
	add	a,0x1
	mov	[0x32],a
	mov	a,[0x3D]
	addc	a
	mov	[0x33],a
	popaf
	mov	[0x3E],a
	cneqsn	a,0x0

l01E3:
	goto	0568

l01E4:
	mov	a,[0x3E]
	ceqsn	a,0x25

l01E6:
	goto	0564

l01E7:
	clear	[0x3F]
	clear	[0x40]
	clear	[0x41]
	clear	[0x42]
	clear	[0x43]
	clear	[0x44]
	clear	[0x45]
	clear	[0x46]
	clear	[0x47]
	clear	[0x48]
	clear	[0x49]
	mov	a,0xFF
	mov	[0x4A],a
	mov	a,0xFF
	mov	[0x4B],a
	mov	a,[0x32]
	mov	[0x4C],a
	mov	a,[0x33]
	mov	[0x4D],a

l01FA:
	mov	a,[0x4C]
	mov	[0x0],a
	mov	a,[0x4D]
	call	fn059D
	mov	[0x4E],a
	inc	[0x4C]
	addc	[0x4D]
	mov	a,[0x4C]
	mov	[0x32],a
	mov	a,[0x4D]
	mov	[0x33],a
	mov	a,[0x4E]
	ceqsn	a,0x25

l0207:
	goto	020C

l0208:
	mov	a,[0x4E]
	mov	[0x2A],a
	call	fn015A
	goto	01D1

l020C:
	mov	a,[0x4E]
	sub	a,0x30
	t0sn	IO(0x0).1

l020F:
	goto	0244

l0210:
	mov	a,0x39
	sub	a,[0x4E]
	t0sn	IO(0x0).1

l0213:
	goto	0244

l0214:
	mov	a,[0x4A]
	ceqsn	a,0xFF

l0216:
	goto	0232

l0217:
	mov	a,[0x4B]
	ceqsn	a,0xFF

l0219:
	goto	0232

l021A:
	mov	a,0xA
	mov	[0xA7],a
	clear	[0xA8]
	mov	a,[0x48]
	mov	[0xA9],a
	mov	a,[0x49]
	mov	[0xAA],a
	call	fn056C
	add	a,[0x4E]
	pushaf
	addc	[0x0]
	popaf
	sub	a,0x30
	mov	[0x48],a
	mov	a,[0x0]
	subc	a
	mov	[0x49],a
	mov	a,[0x48]
	or	a,[0x49]
	ceqsn	a,0x0

l022E:
	goto	01FA

l022F:
	mov	a,0x1
	mov	[0x40],a
	goto	01FA

l0232:
	mov	a,0xA
	mov	[0xA7],a
	clear	[0xA8]
	mov	a,[0x4A]
	mov	[0xA9],a
	mov	a,[0x4B]
	mov	[0xAA],a
	call	fn056C
	add	a,[0x4E]
	pushaf
	addc	[0x0]
	popaf
	sub	a,0x30
	mov	[0x4A],a
	mov	a,[0x0]
	subc	a
	mov	[0x4B],a
	goto	01FA

l0244:
	mov	a,[0x4E]
	ceqsn	a,0x2E

l0246:
	goto	0250

l0247:
	mov	a,[0x4A]
	ceqsn	a,0xFF

l0249:
	goto	01FA

l024A:
	mov	a,[0x4B]
	ceqsn	a,0xFF

l024C:
	goto	01FA

l024D:
	clear	[0x4A]
	clear	[0x4B]
	goto	01FA

l0250:
	mov	a,[0x4E]
	sub	a,0x61
	t0sn	IO(0x0).1

l0253:
	goto	025D

l0254:
	mov	a,0x7A
	sub	a,[0x4E]
	t0sn	IO(0x0).1

l0257:
	goto	025D

l0258:
	mov	a,0xDF
	and	[0x4E],a
	mov	a,0x1
	mov	[0x1E],a
	goto	025E

l025D:
	clear	[0x1E]

l025E:
	mov	a,[0x4E]
	cneqsn	a,0x20

l0260:
	goto	029B

l0261:
	mov	a,[0x4E]
	cneqsn	a,0x2B

l0263:
	goto	0298

l0264:
	mov	a,[0x4E]
	cneqsn	a,0x2D

l0266:
	goto	0295

l0267:
	mov	a,[0x4E]
	cneqsn	a,0x42

l0269:
	goto	029E

l026A:
	mov	a,[0x4E]
	cneqsn	a,0x43

l026C:
	goto	02A4

l026D:
	mov	a,[0x4E]
	cneqsn	a,0x44

l026F:
	goto	0358

l0270:
	mov	a,[0x4E]
	cneqsn	a,0x46

l0272:
	goto	0366

l0273:
	mov	a,[0x4E]
	cneqsn	a,0x48

l0275:
	goto	01FA

l0276:
	mov	a,[0x4E]
	cneqsn	a,0x49

l0278:
	goto	0358

l0279:
	mov	a,[0x4E]
	cneqsn	a,0x4A

l027B:
	goto	01FA

l027C:
	mov	a,[0x4E]
	cneqsn	a,0x4C

l027E:
	goto	02A1

l027F:
	mov	a,[0x4E]
	cneqsn	a,0x4F

l0281:
	goto	035D

l0282:
	mov	a,[0x4E]
	cneqsn	a,0x50

l0284:
	goto	033E

l0285:
	mov	a,[0x4E]
	cneqsn	a,0x53

l0287:
	goto	02BF

l0288:
	mov	a,[0x4E]
	cneqsn	a,0x54

l028A:
	goto	01FA

l028B:
	mov	a,[0x4E]
	cneqsn	a,0x55

l028D:
	goto	0360

l028E:
	mov	a,[0x4E]
	cneqsn	a,0x58

l0290:
	goto	0363

l0291:
	mov	a,[0x4E]
	cneqsn	a,0x5A

l0293:
	goto	01FA

l0294:
	goto	0369

l0295:
	mov	a,0x1
	mov	[0x3F],a
	goto	01FA

l0298:
	mov	a,0x1
	mov	[0x41],a
	goto	01FA

l029B:
	mov	a,0x1
	mov	[0x42],a
	goto	01FA

l029E:
	mov	a,0x1
	mov	[0x44],a
	goto	01FA

l02A1:
	mov	a,0x1
	mov	[0x45],a
	goto	01FA

l02A4:
	mov	a,[0x44]
	cneqsn	a,0x0

l02A6:
	goto	02B2

l02A7:
	mov	a,[0x34]
	sub	a,0x1
	mov	[0x0],a
	mov	a,[0x35]
	subc	a
	mov	[0x35],a
	xch	[0x0]
	mov	[0x34],a
	xch	[0x0]
	call	fn059D
	goto	02BC

l02B2:
	mov	a,[0x34]
	sub	a,0x2
	mov	[0x0],a
	mov	a,[0x35]
	subc	a
	mov	[0x35],a
	xch	[0x0]
	mov	[0x34],a
	xch	[0x0]
	call	fn059D

l02BC:
	mov	[0x2A],a
	call	fn015A
	goto	0371

l02BF:
	mov	a,[0x34]
	sub	a,0x2
	mov	[0x0],a
	mov	a,[0x35]
	subc	a
	mov	[0x35],a
	xch	[0x0]
	mov	[0x34],a
	xch	[0x0]
	call	fn05C0
	mov	[0x23],a
	pushaf
	mov	a,[0x0]
	mov	[0x24],a
	popaf
	mov	[0xAF],a
	mov	a,[0x0]
	mov	[0xB0],a
	call	fn0588
	mov	[0x4F],a
	mov	a,[0x0]
	mov	[0x50],a
	mov	a,[0x4A]
	ceqsn	a,0xFF

l02D7:
	goto	02DF

l02D8:
	mov	a,[0x4B]
	ceqsn	a,0xFF

l02DA:
	goto	02DF

l02DB:
	mov	a,[0x4F]
	mov	[0x4A],a
	mov	a,[0x50]
	mov	[0x4B],a

l02DF:
	mov	a,[0x3F]
	ceqsn	a,0x0

l02E1:
	goto	02FE

l02E2:
	mov	a,[0x4F]
	sub	a,[0x48]
	mov	a,[0x50]
	subc	a,[0x49]
	t1sn	IO(0x0).1

l02E7:
	goto	02FE

l02E8:
	mov	a,[0x48]
	sub	a,[0x4F]
	mov	[0x51],a
	mov	a,[0x49]
	subc	a,[0x50]
	mov	[0x52],a

l02EE:
	mov	a,[0x52]
	mov	[0x0],a
	mov	a,[0x51]
	dec	[0x51]
	subc	[0x52]
	or	a,[0x0]
	cneqsn	a,0x0

l02F5:
	goto	02FA

l02F6:
	mov	a,0x20
	mov	[0x2A],a
	call	fn015A
	goto	02EE

l02FA:
	mov	a,[0x51]
	mov	[0x48],a
	mov	a,[0x52]
	mov	[0x49],a

l02FE:
	mov	a,[0x4A]
	mov	[0x53],a
	mov	a,[0x4B]
	mov	[0x54],a

l0302:
	mov	a,[0x23]
	mov	[0x0],a
	mov	a,[0x24]
	call	fn059D
	mov	[0x55],a
	cneqsn	a,0x0

l0308:
	goto	0323

l0309:
	mov	a,0x0
	sub	a,[0x53]
	mov	a,0x0
	subc	a,[0x54]
	t0sn	IO(0x0).3

l030E:
	xor	a,0x80

l030F:
	sl	a
	t1sn	IO(0x0).1

l0311:
	goto	0323

l0312:
	dec	[0x53]
	subc	[0x54]
	mov	a,[0x55]
	mov	[0x2A],a
	call	fn015A
	mov	a,[0x23]
	pushaf
	mov	a,[0x24]
	mov	[0x0],a
	popaf
	add	a,0x1
	xch	[0x0]
	addc	a
	mov	[0x24],a
	mov	a,[0x0]
	mov	[0x23],a
	goto	0302

l0323:
	mov	a,[0x3F]
	cneqsn	a,0x0

l0325:
	goto	0371

l0326:
	mov	a,[0x4F]
	sub	a,[0x48]
	mov	a,[0x50]
	subc	a,[0x49]
	t1sn	IO(0x0).1

l032B:
	goto	0371

l032C:
	mov	a,[0x48]
	sub	a,[0x4F]
	mov	[0x56],a
	mov	a,[0x49]
	subc	a,[0x50]
	mov	[0x57],a

l0332:
	mov	a,[0x57]
	mov	[0x0],a
	mov	a,[0x56]
	dec	[0x56]
	subc	[0x57]
	or	a,[0x0]
	cneqsn	a,0x0

l0339:
	goto	036D

l033A:
	mov	a,0x20
	mov	[0x2A],a
	call	fn015A
	goto	0332

l033E:
	mov	a,[0x34]
	sub	a,0x2
	mov	[0x0],a
	mov	a,[0x35]
	subc	a
	mov	[0x35],a
	xch	[0x0]
	mov	[0x34],a
	xch	[0x0]
	call	fn05C0
	mov	[0x23],a
	mov	a,[0x0]
	mov	[0x24],a
	mov	a,0x30
	mov	[0x2A],a
	call	fn015A
	mov	a,0x78
	mov	[0x2A],a
	call	fn015A
	mov	a,[0x24]
	mov	[0x2C],a
	call	fn0189
	mov	a,[0x23]
	mov	[0x2C],a
	call	fn0189
	goto	0371

l0358:
	mov	a,0x1
	mov	[0x43],a
	mov	a,0xA
	mov	[0x47],a
	goto	0371

l035D:
	mov	a,0x8
	mov	[0x47],a
	goto	0371

l0360:
	mov	a,0xA
	mov	[0x47],a
	goto	0371

l0363:
	mov	a,0x10
	mov	[0x47],a
	goto	0371

l0366:
	mov	a,0x1
	mov	[0x46],a
	goto	0371

l0369:
	mov	a,[0x4E]
	mov	[0x2A],a
	call	fn015A
	goto	0371

l036D:
	mov	a,[0x56]
	mov	[0x48],a
	mov	a,[0x57]
	mov	[0x49],a

l0371:
	mov	a,[0x46]
	cneqsn	a,0x0

l0373:
	goto	03B4

l0374:
	mov	a,[0x34]
	sub	a,0x4
	mov	[0x0],a
	mov	a,[0x35]
	subc	a
	mov	[0x35],a
	xch	[0x0]
	mov	[0x34],a
	xch	[0x0]
	mov	[0x59],a
	mov	a,[0x0]
	mov	[0x58],a
	mov	[0x0],a
	mov	a,[0x59]
	call	fn05C0
	mov	[0x5A],a
	mov	a,[0x0]
	mov	[0x5B],a
	mov	a,[0x58]
	mov	[0x0],a
	mov	a,[0x59]
	inc	[0x0]
	addc	a
	inc	[0x0]
	addc	a
	call	fn05C0
	mov	[0x5C],a
	mov	a,[0x0]
	mov	[0x5D],a
	mov	a,[0x5A]
	mov	[0x23],a
	mov	a,[0x5B]
	mov	[0x24],a
	mov	a,[0x5C]
	mov	[0x25],a
	mov	a,[0x5D]
	mov	[0x26],a
	mov	a,0x1E
	mov	[0x23],a
	mov	a,0x86
	mov	[0x24],a

l039D:
	mov	a,[0x23]
	mov	[0x5E],a
	mov	a,[0x24]
	mov	[0x5F],a
	mov	a,[0x5E]
	add	a,0x1
	mov	[0x0],a
	mov	a,[0x5F]
	addc	a
	mov	[0x24],a
	mov	a,[0x0]
	mov	[0x23],a
	mov	a,[0x5E]
	mov	[0x0],a
	mov	a,[0x5F]
	call	fn059D
	mov	[0x0],a
	cneqsn	a,0x0

l03AF:
	goto	01D1

l03B0:
	mov	a,[0x0]
	mov	[0x2A],a
	call	fn015A
	goto	039D

l03B4:
	mov	a,[0x47]
	cneqsn	a,0x0

l03B6:
	goto	01D1

l03B7:
	mov	a,[0x44]
	cneqsn	a,0x0

l03B9:
	goto	03E9

l03BA:
	mov	a,[0x34]
	sub	a,0x1
	mov	[0x0],a
	mov	a,[0x35]
	subc	a
	mov	[0x35],a
	xch	[0x0]
	mov	[0x34],a
	xch	[0x0]
	call	fn059D
	mov	[0x60],a
	clear	[0x61]
	clear	[0x62]
	clear	[0x63]
	mov	a,[0x60]
	mov	[0x23],a
	mov	a,[0x61]
	mov	[0x24],a
	mov	a,[0x62]
	mov	[0x25],a
	mov	a,[0x63]
	mov	[0x26],a
	mov	a,[0x43]
	ceqsn	a,0x0

l03D2:
	goto	0445

l03D3:
	mov	a,[0x23]
	mov	[0x64],a
	mov	a,[0x24]
	mov	[0x65],a
	mov	a,[0x25]
	mov	[0x66],a
	mov	a,[0x26]
	mov	[0x67],a
	mov	a,[0x64]
	mov	[0x68],a
	clear	[0x69]
	clear	[0x6A]
	clear	[0x6B]
	mov	a,[0x68]
	mov	[0x23],a
	mov	a,[0x69]
	mov	[0x24],a
	mov	a,[0x6A]
	mov	[0x25],a
	mov	a,[0x6B]
	mov	[0x26],a
	goto	0445

l03E9:
	mov	a,[0x45]
	cneqsn	a,0x0

l03EB:
	goto	0412

l03EC:
	mov	a,[0x34]
	sub	a,0x4
	mov	[0x0],a
	mov	a,[0x35]
	subc	a
	mov	[0x35],a
	xch	[0x0]
	mov	[0x34],a
	xch	[0x0]
	mov	[0x6D],a
	mov	a,[0x0]
	mov	[0x6C],a
	mov	[0x0],a
	mov	a,[0x6D]
	call	fn05C0
	mov	[0x6E],a
	mov	a,[0x0]
	mov	[0x6F],a
	mov	a,[0x6C]
	mov	[0x0],a
	mov	a,[0x6D]
	inc	[0x0]
	addc	a
	inc	[0x0]
	addc	a
	call	fn05C0
	mov	[0x70],a
	mov	a,[0x0]
	mov	[0x71],a
	mov	a,[0x6E]
	mov	[0x23],a
	mov	a,[0x6F]
	mov	[0x24],a
	mov	a,[0x70]
	mov	[0x25],a
	mov	a,[0x71]
	mov	[0x26],a
	goto	0445

l0412:
	mov	a,[0x34]
	sub	a,0x2
	mov	[0x0],a
	mov	a,[0x35]
	subc	a
	mov	[0x35],a
	xch	[0x0]
	mov	[0x34],a
	xch	[0x0]
	call	fn05C0
	mov	[0x72],a
	mov	a,[0x0]
	mov	[0x73],a
	sl	a
	mov	a,0x0
	subc	a
	mov	[0x74],a
	mov	[0x75],a
	mov	a,[0x72]
	mov	[0x23],a
	mov	a,[0x73]
	mov	[0x24],a
	mov	a,[0x74]
	mov	[0x25],a
	mov	a,[0x75]
	mov	[0x26],a
	mov	a,[0x43]
	ceqsn	a,0x0

l042E:
	goto	0445

l042F:
	mov	a,[0x23]
	mov	[0x76],a
	mov	a,[0x24]
	mov	[0x77],a
	mov	a,[0x25]
	mov	[0x78],a
	mov	a,[0x26]
	mov	[0x79],a
	mov	a,[0x76]
	mov	[0x7A],a
	mov	a,[0x77]
	mov	[0x7B],a
	clear	[0x7C]
	clear	[0x7D]
	mov	a,[0x7A]
	mov	[0x23],a
	mov	a,[0x7B]
	mov	[0x24],a
	mov	a,[0x7C]
	mov	[0x25],a
	mov	a,[0x7D]
	mov	[0x26],a

l0445:
	mov	a,[0x43]
	cneqsn	a,0x0

l0447:
	goto	0471

l0448:
	mov	a,[0x23]
	mov	[0x7E],a
	mov	a,[0x24]
	mov	[0x7F],a
	mov	a,[0x25]
	mov	[0x80],a
	mov	a,[0x26]
	mov	[0x81],a
	sub	a,0x80
	t0sn	IO(0x0).1

l0452:
	goto	0470

l0453:
	mov	a,[0x23]
	mov	[0x82],a
	mov	a,[0x24]
	mov	[0x83],a
	mov	a,[0x25]
	mov	[0x84],a
	mov	a,[0x26]
	mov	[0x85],a
	mov	a,0x0
	sub	a,[0x82]
	mov	[0x86],a
	mov	a,0x0
	subc	a,[0x83]
	mov	[0x87],a
	mov	a,0x0
	subc	a,[0x84]
	mov	[0x88],a
	mov	a,0x0
	subc	a,[0x85]
	mov	[0x89],a
	mov	a,[0x86]
	mov	[0x23],a
	mov	a,[0x87]
	mov	[0x24],a
	mov	a,[0x88]
	mov	[0x25],a
	mov	a,[0x89]
	mov	[0x26],a
	goto	0471

l0470:
	clear	[0x43]

l0471:
	mov	a,0x1
	mov	[0x8A],a
	mov	a,0x3B
	mov	[0x8B],a
	clear	[0x8C]
	clear	[0x8D]
	clear	[0x8E]

l0478:
	clear	[0x27]
	mov	a,[0x47]
	mov	[0x2D],a
	call	fn0195
	mov	a,[0x8A]
	ceqsn	a,0x0

l047E:
	goto	0496

l047F:
	mov	a,[0x27]
	mov	[0x8F],a
	sl	[0x8F]
	sl	[0x8F]
	sl	[0x8F]
	sl	[0x8F]
	mov	a,[0x27]
	swap	a
	and	a,0xF
	or	a,[0x8F]
	mov	[0x90],a
	mov	a,[0x8B]
	mov	[0x0],a
	mov	a,[0x8C]
	call	fn059D
	or	a,[0x90]
	mov	[0x0],a
	mov	a,[0x8B]
	xch	[0x0]
	idxm	[[0x0]],a
	dec	[0x8B]
	subc	[0x8C]
	goto	049B

l0496:
	mov	a,[0x27]
	mov	[0x0],a
	mov	a,[0x8B]
	xch	[0x0]
	idxm	[[0x0]],a

l049B:
	inc	[0x8D]
	addc	[0x8E]
	mov	a,0x1
	xor	[0x8A],a
	mov	a,[0x23]
	mov	[0x91],a
	mov	a,[0x24]
	mov	[0x92],a
	mov	a,[0x25]
	mov	[0x93],a
	mov	a,[0x26]
	mov	[0x94],a
	mov	a,[0x91]
	or	a,[0x92]
	or	a,[0x93]
	or	a,[0x94]
	ceqsn	a,0x0

l04AC:
	goto	0478

l04AD:
	mov	a,[0x8B]
	mov	[0x95],a
	mov	a,[0x8C]
	mov	[0x96],a
	mov	a,[0x8D]
	mov	[0x97],a
	mov	a,[0x8E]
	mov	[0x98],a
	mov	a,[0x48]
	or	a,[0x49]
	ceqsn	a,0x0

l04B8:
	goto	04BC

l04B9:
	mov	a,0x1
	mov	[0x48],a
	clear	[0x49]

l04BC:
	mov	a,[0x40]
	ceqsn	a,0x0

l04BE:
	goto	04D8

l04BF:
	mov	a,[0x3F]
	ceqsn	a,0x0

l04C1:
	goto	04D8

l04C2:
	mov	a,[0x48]
	mov	[0x99],a
	mov	a,[0x49]
	mov	[0x9A],a

l04C6:
	mov	a,[0x97]
	add	a,0x1
	clear	[0x0]
	sub	a,[0x99]
	mov	a,[0x0]
	subc	a,[0x9A]
	t1sn	IO(0x0).1

l04CD:
	goto	04D4

l04CE:
	mov	a,0x20
	mov	[0x2A],a
	call	fn015A
	dec	[0x99]
	subc	[0x9A]
	goto	04C6

l04D4:
	mov	a,[0x99]
	mov	[0x48],a
	mov	a,[0x9A]
	mov	[0x49],a

l04D8:
	mov	a,[0x43]
	cneqsn	a,0x0

l04DA:
	goto	04E1

l04DB:
	mov	a,0x2D
	mov	[0x2A],a
	call	fn015A
	dec	[0x48]
	subc	[0x49]
	goto	04F6

l04E1:
	mov	a,[0x97]
	or	a,[0x98]
	cneqsn	a,0x0

l04E4:
	goto	04F6

l04E5:
	mov	a,[0x41]
	cneqsn	a,0x0

l04E7:
	goto	04EE

l04E8:
	mov	a,0x2B
	mov	[0x2A],a
	call	fn015A
	dec	[0x48]
	subc	[0x49]
	goto	04F6

l04EE:
	mov	a,[0x42]
	cneqsn	a,0x0

l04F0:
	goto	04F6

l04F1:
	mov	a,0x20
	mov	[0x2A],a
	call	fn015A
	dec	[0x48]
	subc	[0x49]

l04F6:
	mov	a,[0x3F]
	ceqsn	a,0x0

l04F8:
	goto	0514

l04F9:
	mov	a,[0x48]
	mov	[0x9B],a
	mov	a,[0x49]
	mov	[0x9C],a

l04FD:
	mov	a,[0x9B]
	mov	[0x9D],a
	mov	a,[0x9C]
	mov	[0x9E],a
	dec	[0x9B]
	subc	[0x9C]
	mov	a,[0x97]
	sub	a,[0x9D]
	mov	a,[0x98]
	subc	a,[0x9E]
	t1sn	IO(0x0).1

l0508:
	goto	0524

l0509:
	mov	a,[0x40]
	cneqsn	a,0x0

l050B:
	goto	050F

l050C:
	mov	a,0x30
	clear	[0x0]
	goto	0511

l050F:
	mov	a,0x20
	clear	[0x0]

l0511:
	mov	[0x2A],a
	call	fn015A
	goto	04FD

l0514:
	mov	a,[0x97]
	sub	a,[0x48]
	mov	a,[0x98]
	subc	a,[0x49]
	t1sn	IO(0x0).1

l0519:
	goto	0521

l051A:
	mov	a,[0x48]
	sub	a,[0x97]
	mov	[0x9F],a
	mov	a,[0x49]
	subc	a,[0x98]
	mov	[0xA0],a
	goto	0528

l0521:
	clear	[0x9F]
	clear	[0xA0]
	goto	0528

l0524:
	mov	a,[0x9B]
	mov	[0x9F],a
	mov	a,[0x9C]
	mov	[0xA0],a

l0528:
	mov	a,[0x95]
	mov	[0xA1],a
	mov	a,[0x96]
	mov	[0xA2],a
	mov	a,[0x97]
	mov	[0xA3],a
	mov	a,[0x98]
	mov	[0xA4],a

l0530:
	mov	a,[0xA4]
	mov	[0x0],a
	mov	a,[0xA3]
	dec	[0xA3]
	subc	[0xA4]
	or	a,[0x0]
	cneqsn	a,0x0

l0537:
	goto	0551

l0538:
	mov	a,0x1
	xor	[0x8A],a
	mov	a,[0x8A]
	ceqsn	a,0x0

l053C:
	goto	0547

l053D:
	inc	[0xA1]
	addc	[0xA2]
	mov	a,[0xA1]
	mov	[0x0],a
	mov	a,[0xA2]
	call	fn059D
	swap	a
	and	a,0xF
	mov	[0x27],a
	goto	054D

l0547:
	mov	a,[0xA1]
	mov	[0x0],a
	mov	a,[0xA2]
	call	fn059D
	and	a,0xF
	mov	[0x27],a

l054D:
	mov	a,[0x27]
	mov	[0x2B],a
	call	fn0178
	goto	0530

l0551:
	mov	a,[0x3F]
	cneqsn	a,0x0

l0553:
	goto	01D1

l0554:
	mov	a,[0x9F]
	mov	[0xA5],a
	mov	a,[0xA0]
	mov	[0xA6],a

l0558:
	mov	a,[0xA6]
	mov	[0x0],a
	mov	a,[0xA5]
	dec	[0xA5]
	subc	[0xA6]
	or	a,[0x0]
	cneqsn	a,0x0

l055F:
	goto	01D1

l0560:
	mov	a,0x20
	mov	[0x2A],a
	call	fn015A
	goto	0558

l0564:
	mov	a,[0x3E]
	mov	[0x2A],a
	call	fn015A
	goto	01D1

l0568:
	mov	a,[0x29]
	mov	[0x0],a
	mov	a,[0x28]
	ret

;; fn056C: 056C
;;   Called from:
;;     0221 (in fn01C7)
;;     0239 (in fn01C7)
fn056C proc
	mov	a,[0xA7]
	mov	[0xB1],a
	mov	a,[0xA9]
	mov	[0xB2],a
	call	fn05A9
	mov	[0xAB],a
	mov	a,[0x0]
	mov	[0xAC],a
	mov	[0xAD],a
	mov	a,[0xA7]
	mov	[0xB1],a
	mov	a,[0xAA]
	mov	[0xB2],a
	call	fn05A9
	mov	[0xAE],a
	mov	a,[0xA8]
	mov	[0xB1],a
	mov	a,[0xA9]
	mov	[0xB2],a
	call	fn05A9
	add	a,[0xAE]
	add	a,[0xAD]
	mov	[0xAC],a
	mov	a,[0xAB]
	mov	[0x0],a
	mov	a,[0xAC]
	xch	[0x0]
	ret

;; fn0588: 0588
;;   Called from:
;;     02D1 (in fn01C7)
fn0588 proc
	clear	[0xB3]
	clear	[0xB4]
	mov	a,[0xAF]
	mov	[0xB5],a
	mov	a,[0xB0]
	mov	[0xB6],a

l058E:
	mov	a,[0xB5]
	mov	[0x0],a
	mov	a,[0xB6]
	call	fn059D
	inc	[0xB5]
	addc	[0xB6]
	cneqsn	a,0x0

l0595:
	goto	0599

l0596:
	inc	[0xB3]
	addc	[0xB4]
	goto	058E

l0599:
	mov	a,[0xB4]
	mov	[0x0],a
	mov	a,[0xB3]
	ret

;; fn059D: 059D
;;   Called from:
;;     0119 (in fn0112)
;;     01D8 (in fn01C7)
;;     01FD (in fn01C7)
;;     02B0 (in fn01C7)
;;     02BB (in fn01C7)
;;     0305 (in fn01C7)
;;     03AC (in fn01C7)
;;     03C3 (in fn01C7)
;;     048D (in fn01C7)
;;     0542 (in fn01C7)
;;     054A (in fn01C7)
;;     0591 (in fn0588)
fn059D proc
	sub	a,0x80
	t1sn	IO(0x0).1

l059F:
	goto	05A2

l05A0:
	idxm	a,[[0x0]]
	ret

l05A2:
	xch	[0x0]
	pushaf
	mov	a,IO(0x2)
	add	a,0xFF
	xch	[0x0]
	idxm	[[0x0]],a
	ret

;; fn05A9: 05A9
;;   Called from:
;;     0570 (in fn056C)
;;     0579 (in fn056C)
;;     057F (in fn056C)
fn05A9 proc
	clear	[0xB3]
	clear	[0xB4]
	clear	[0x0]

l05AC:
	mov	a,[0x0]
	sub	a,0x8
	t1sn	IO(0x0).1

l05AF:
	goto	05BC

l05B0:
	sl	[0xB3]
	slc	[0xB4]
	mov	a,[0xB1]
	and	a,0x80
	cneqsn	a,0x0

l05B5:
	goto	05B9

l05B6:
	mov	a,[0xB2]
	add	[0xB3],a
	addc	[0xB4]

l05B9:
	sl	[0xB1]
	inc	[0x0]
	goto	05AC

l05BC:
	mov	a,[0xB4]
	mov	[0x0],a
	mov	a,[0xB3]
	ret

;; fn05C0: 05C0
;;   Called from:
;;     02C8 (in fn01C7)
;;     0347 (in fn01C7)
;;     0382 (in fn01C7)
;;     038D (in fn01C7)
;;     03FA (in fn01C7)
;;     0405 (in fn01C7)
;;     041B (in fn01C7)
fn05C0 proc
	sub	a,0x80
	t1sn	IO(0x0).1

l05C2:
	goto	05CA

l05C3:
	idxm	a,[[0x0]]
	pushaf
	inc	[0x0]
	idxm	a,[[0x0]]
	mov	[0x0],a
	popaf
	ret

l05CA:
	xch	[0x0]
	pushaf
	call	fn05CF
	mov	[0x0],a
	ret

;; fn05CF: 05CF
;;   Called from:
;;     05CC (in fn05C0)
fn05CF proc
	add	a,0x1
	pushaf
	mov	a,IO(0x2)
	add	a,0xFB
	xch	[0x0]
	idxm	[[0x0]],a
	popaf
	pushaf
	idxm	a,[[0x0]]
	addc	a
	xch	[0x0]
	add	a,0x4
	xch	[0x0]
	idxm	[[0x0]],a
	ret
