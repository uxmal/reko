;;; Segment code (0000)

;; fn0000: 0000
fn0000 proc
	jp	0100
0003          00 00 00 00 00 00 00 00 00 00 00 00 00    .............
0010 1A 3F 02 00 00 00 00 00 00 00 00 00 00 00 00 00 .?..............
0020 1A 4F 02 00 00 00 00 00 1F 00 00 00 00 00 00 00 .O..............
0030 1A 53 02 00 00 00 00 00 1A 57 02 00 00 00 00 00 .S.......W......
0040 1F 00 00 00 00 00 00 00 1A 60 02 00 00 00 00 00 .........`......
0050 1A 64 02 00 00 00 00 00 1A 68 02 00 00 00 00 00 .d.......h......
0060 1A 6C 02 00 00 00 00 00 1F 00 00 00 00 00 00 00 .l..............
0070 1A 32 02 00 00 00 00 00 1A 39 02 00 00 00 00 00 .2.......9......
0080 00 3E A0 FF EF C2 6E 04 37 C6 F7 37 C7 A5 37 C8 .>....n.7..7..7.
0090 00 37 C9 03 37 CD 03 37 CE F0 37 CB A0 37 CF 00 .7..7..7..7..7..
00A0 37 E4 35 37 D0 08 37 D1 08 37 E5 01 37 DA 40 37 7.57..7..7..7.@7
00B0 E9 01 EF E9 6E 08 EF E9 6E 20 37 EA 00 37 D2 C0 ....n...n 7..7..
00C0 37 DB 20 EF DB 6E E0 B8 E6 B9 E6 3A 20 FF 36 00 7. ..n.....: .6.
00D0 1C A4 02 EA 26 92 7F C0 FF CE F5 1C A4 02 B0 BF ....&...........
00E0 1C 00 00 1C A4 02 1C 23 08 1C 22 08 01 50 E3 00 .......#.."..P..
00F0 00 4D E7 ED 28 E3 00 00 2E 2F ED E5 2E EF ED 20 .M..(..../..... 

l0100:
	ld	(0000),a
	pop	bc
	ret
0106                   50 E3 00 00 4D E7 ED 28 E3 00       P...M..(..
0110 00 2E 2F ED E3 00 00 2E ED 26 EF ED 20 58 1E 27 ../......&.. X.'
0120 EB 2F BE B9 BF 1F B0 BF 37 C3 0F 1F 52 F2 04 4A ./......7...R..J
0130 4F BC F2 02 4A 4F BA 1C 00 00 5A 1F 1C 00 00 1F O...JO....Z.....
0140 1C 00 00 1F 97 B8 37 D3 4E 37 C2 10 1F 1C 00 00 ......7.N7......
0150 1F 1C 00 00 1F 1C 00 00 1F 1C 00 00 1F F2 02 2E ................
0160 A8 BF C6 0D 02 A8 C3 CE 08 03 00 00 1C A4 02 C8 ................
0170 EF 02 37 C3 0F 2F EB B8 BF 03 1E F2 02 4A E2 2E ..7../.......J..
0180 FE 66 FE D6 1C 73 02 92 C8 F4 37 D2 00 37 D3 B1 .f...s....7..7..
0190 1E 37 D3 4E 1E 54 3C 00 00 FE 14 16 DA FF 08 F2 .7.N.T<.........
01A0 2A 4A 08 51 3A 1E 00 FE 70 08 38 04 00 FE 59 59 *J.Q:...p.8...YY
01B0 3A 04 00 F9 70 F6 24 42 51 08 F2 26 4A 08 3A 1A :...p.$BQ..&J.:.
01C0 00 FE 70 08 38 04 00 FE 59 59 F0 06 29 F0 07 28 ..p.8...YY..)..(
01D0 51 50 F9 33 F8 32 3A 10 00 FE 70 08 38 04 00 FE QP.3.2:...p.8...
01E0 59 58 59 3A 04 00 F8 70 51 50 08 3A 0C 00 FE 70 YXY:...pQP.:...p
01F0 08 38 04 00 FE 59 58 59 3A 08 00 F8 70 51 50 08 .8...YXY:...pQP.
0200 3A 08 00 FE 70 08 38 04 00 FE 59 58 59 3A 0C 00 :...p.8...YXY:..
0210 F8 70 51 08 3A 02 00 FE 70 08 38 04 00 FE 59 59 .pQ.:...p.8...YY
0220 F4 EA 37 20 FE 65 F4 EB 26 F4 EC 26 F4 ED 26 FE ..7 .e..&..&..&.
0230 65 F4 EE 26 F4 EF 26 F4 F0 26 F4 F1 26 F0 EE 2E e..&..&..&..&...
0240 68 B9 F4 EE 26 F0 EF 2E 69 79 F4 EF 26 F0 F0 2E h...&...iy..&...
0250 69 37 F4 F0 26 F0 F1 2E 69 9E F4 F1 26 56 F0 F2 i7..&...i...&V..
0260 29 F0 F3 28 F0 F4 2D F0 F5 2C 5E 36 04 F9 A4 F8 )..(..-..,^6....
0270 A2 FA 71 8E CE F7 21 F0 E6 60 F4 FA 26 20 F0 E7 ..q...!..`..& ..
0280 61 F4 FB 26 25 F0 E8 61 F4 FC 26 24 F0 E9 61 F4 a..&%..a..&$..a.
0290 FD 26 F0 F2 2E F0 EE 60 29 F0 F3 2E F0 EF 61 28 .&.....`).....a(
02A0 F0 F4 2E F0 F0 61 2D F0 F5 2E F0 F1 61 2C F0 FA .....a-.....a,..
02B0 2E F9 65 F4 FA 26 F0 FB 2E F8 65 F4 FB 26 F0 FC ..e..&....e..&..
02C0 2E FD 65 F4 FC 26 F0 FD 2E FC 65 F4 FD 26 56 F0 ..e..&....e..&V.
02D0 F2 29 F0 F3 28 F0 F4 2D F0 F5 2C 5E 36 05 FC A7 .)..(..-..,^6...
02E0 FD A3 F8 A3 F9 A3 8E CE F5 21 F0 E2 60 29 20 F0 .........!..`) .
02F0 E3 61 28 25                                     .a(%           

;; fn02F4: 02F4
;;   Called from:
;;     1682 (in fn164F)
;;     16F7 (in fn164F)
fn02F4 proc
	adc	a,(ix-0x1C)
	ld	l,a
	ld	a,h
	adc	a,(ix-0x1B)
	ld	h,a
	ld	a,c
	xor	a,(ix-0x06)
	ld	c,a
	ld	a,b
	xor	a,(ix-0x05)
	ld	b,a
	ld	a,l
	xor	a,(ix-0x04)
	ld	l,a
	ld	a,h
	xor	a,(ix-0x03)
	ld	h,a
	ld	a,(ix-0x0A)
	add	a,c
	ld	(ix-0x0A),a
	ld	a,(ix-0x09)
	adc	a,b
	ld	(ix-0x09),a
	ld	a,(ix-0x08)
	adc	a,l
	ld	(ix-0x08),a
	ld	a,(ix-0x07)
	adc	a,h
	ld	(ix-0x07),a
	push	af
	ld	c,(ix-0x0A)
	ld	b,(ix-0x09)
	ld	l,(ix-0x08)
	ld	h,(ix-0x07)
	pop	af
	ld	a,04

l0341:
	sla	c
	rl	b
	adc	hl,hl
	dec	a
	jr	NZ,0341

l034A:
	ld	a,c
	add	a,(ix-0x22)

;; fn034E: 034E
;;   Called from:
;;     034B (in fn02F4)
fn034E proc
	ld	(ix-0x06),a

l0350:
	ld	a,a
	ld	a,b
	adc	a,(ix-0x21)
	ld	(ix-0x05),a
	ld	a,l
	adc	a,(ix-0x20)
	ld	(ix-0x04),a
	ld	a,h
	adc	a,(ix-0x1F)
	ld	(ix-0x03),a
	ld	a,(ix-0x0A)
	add	a,(ix-0x12)
	ld	c,a
	ld	a,(ix-0x09)
	adc	a,(ix-0x11)
	ld	b,a
	ld	a,(ix-0x08)
	adc	a,(ix-0x10)
	ld	l,a
	ld	a,(ix-0x07)
	adc	a,(ix-0x0F)
	ld	h,a
	ld	a,(ix-0x06)
	xor	a,c
	ld	(ix-0x06),a
	ld	a,(ix-0x05)
	xor	a,b
	ld	(ix-0x05),a
	ld	a,(ix-0x04)
	xor	a,l
	ld	(ix-0x04),a
	ld	a,(ix-0x03)

;; fn039D: 039D
fn039D proc
	xor	a,h
	ld	(ix-0x03),a
	push	af
	ld	c,(ix-0x0A)
	ld	b,(ix-0x09)
	ld	l,(ix-0x08)

;; fn03AA: 03AA
;;   Called from:
;;     2359 (in fn22A6)
;;     3EE0 (in fn3E2D)
fn03AA proc
	invalid
	ld	h,(ix-0x07)
	pop	af
	ld	a,05

;; fn03B2: 03B2
;;   Called from:
;;     034E (in fn034E)
;;     03B0 (in fn039D)
fn03B2 proc
	srl	h
	rr	l
	rr	b
	rr	c
	dec	a
	jr	NZ,03B2

l03BD:
	ld	a,c
	add	a,(ix-0x26)
	ld	c,a
	ld	a,b
	adc	a,(ix-0x25)
	ld	b,a
	ld	a,l
	adc	a,(ix-0x24)
	ld	l,a
	ld	a,h
	adc	a,(ix-0x23)
	ld	h,a
	ld	a,c
	xor	a,(ix-0x06)
	ld	c,a
	ld	a,b
	xor	a,(ix-0x05)
	ld	b,a
	ld	a,l
	xor	a,(ix-0x04)
	ld	l,a
	ld	a,h
	xor	a,(ix-0x03)
	ld	h,a
	ld	a,(ix-0x0E)
	add	a,c
	ld	(ix-0x0E),a
	ld	a,(ix-0x0D)
	adc	a,b
	ld	(ix-0x0D),a
	ld	a,(ix-0x0C)
	adc	a,l
	ld	(ix-0x0C),a
	ld	a,(ix-0x0B)
	adc	a,h
	ld	(ix-0x0B),a
	ld	a,(ix-0x16)
	add	a,FF
	ld	c,a
	ld	a,(ix-0x15)
	adc	a,FF
	ld	b,a
	ld	a,(ix-0x14)
	adc	a,FF
	ld	l,a
	ld	a,(ix-0x13)
	adc	a,FF
	ld	h,a
	ld	(ix-0x16),c
	ld	(ix-0x15),b
	ld	(ix-0x14),l
	ld	(ix-0x13),h
	ld	a,h
	or	a,l
	or	a,b
	or	a,c
	jp	NZ,0350

l0434:
	ld	hl,001C
	add	hl,sp
	ld	bc,0004

l043C:
	ldir

l043E:
	ex	de,hl
	ld	hl,(sp+0x24)
	ex	de,hl
	ld	hl,0018
	add	hl,sp
	ld	bc,0004

l044B:
	ldir

l044D:
	ld	sp,ix
	pop	ix
	ret
0451    54 3C 00 00 FE 14 16 DA FF 08 F2 2A 4A 08 51  T<.........*J.Q
0460 3A 0E 00 FE 70 08 38 04 00 FE 59 59 3A 04 00 F9 :...p.8...YY:...
0470 70 F6 20 42 51 08 F2 22 4A 08 3A 06 00 FE 70 08 p. BQ.."J.:...p.
0480 38 04 00 FE 59 59 F0 06 29 F0 07 28 51 50 F9 33 8...YY..)..(QP.3
0490 F8 32 3A 18 00 FE 70 08 38 04 00 FE 59 58 59 3A .2:...p.8...YXY:
04A0 04 00 F8 70 51 50 08 3A 1C 00 FE 70 08 38 04 00 ...pQP.:...p.8..
04B0 FE 59 58 59 3A 08 00 F8 70 51 50 08 3A 20 00 FE .YXY:...pQP.: ..
04C0 70 08 38 04 00 FE 59 58 59 3A 0C 00 F8 70 51 08 p.8...YXY:...pQ.
04D0 3A 12 00 FE 70 08 38 04 00 FE 59 59 F4 E2 37 20 :...p.8...YY..7 
04E0 FE 65 F4 E3 26 F4 E4 26 F4 E5 26 F4 DA 37       .e..&..&..&..7 

;; fn04EE: 04EE
fn04EE proc
	ld	a,b
	ld	(ix-0x25),37
	ld	(ix-0x24),EF
	ld	(ix-0x23),C6
	push	af
	ld	c,(ix-0x1A)
	ld	b,(ix-0x19)
	ld	l,(ix-0x18)
	ld	h,(ix-0x17)
	pop	af
	ld	a,04

l050B:
	sla	c
	rl	b
	adc	hl,hl
	dec	a
	jr	NZ,050B

l0514:
	ld	a,c
	add	a,(ix-0x0A)
	ld	(ix-0x04),a
	ld	a,b
	adc	a,(ix-0x09)
	ld	(ix-0x03),a
	ld	a,l
	adc	a,(ix-0x08)
	ld	(ix-0x02),a
	ld	a,h
	adc	a,(ix-0x07)
	ld	(ix-0x01),a
	ld	a,(ix-0x1A)
	add	a,(ix-0x26)
	ld	c,a
	ld	a,(ix-0x19)
	adc	a,(ix-0x25)
	ld	b,a
	ld	a,(ix-0x18)
	adc	a,(ix-0x24)
	ld	l,a
	ld	a,(ix-0x17)
	adc	a,(ix-0x23)
	ld	h,a
	ld	a,(ix-0x04)
	xor	a,c
	ld	(ix-0x04),a
	ld	a,(ix-0x03)
	xor	a,b
	ld	(ix-0x03),a
	ld	a,(ix-0x02)
	xor	a,l
	ld	(ix-0x02),a
	ld	a,(ix-0x01)
	xor	a,h
	ld	(ix-0x01),a
	push	af
	ld	c,(ix-0x1A)
	ld	b,(ix-0x19)
	ld	l,(ix-0x18)
	ld	h,(ix-0x17)
	pop	af
	ld	a,05

l057C:
	srl	h
	rr	l
	rr	b
	rr	c
	dec	a
	jr	NZ,057C

l0587:
	ld	a,c
	add	a,(ix-0x16)
	ld	c,a
	ld	a,b
	adc	a,(ix-0x15)
	ld	b,a
	ld	a,l
	adc	a,(ix-0x14)
	ld	l,a
	ld	a,h
	adc	a,(ix-0x13)
	ld	h,a
	ld	a,c
	xor	a,(ix-0x04)
	ld	c,a
	ld	a,b
	xor	a,(ix-0x03)
	ld	b,a
	ld	a,l
	xor	a,(ix-0x02)
	ld	l,a
	ld	a,h
	xor	a,(ix-0x01)
	ld	h,a
	ld	a,(ix-0x22)
	sub	a,c
	ld	(ix-0x22),a
	ld	a,(ix-0x21)
	sbc	a,b
	ld	(ix-0x21),a
	ld	a,(ix-0x20)
	sbc	a,l
	ld	(ix-0x20),a
	ld	a,(ix-0x1F)
	sbc	a,h
	ld	(ix-0x1F),a
	push	af
	ld	c,(ix-0x22)
	ld	b,(ix-0x21)
	ld	l,(ix-0x20)
	ld	h,(ix-0x1F)
	pop	af
	ld	a,04

l05DF:
	sla	c
	rl	b
	adc	hl,hl
	dec	a
	jr	NZ,05DF

l05E8:
	ld	a,c
	add	a,(ix-0x12)
	ld	(ix-0x04),a
	ld	a,b
	adc	a,(ix-0x11)
	ld	(ix-0x03),a
	ld	a,l
	adc	a,(ix-0x10)
	ld	(ix-0x02),a
	ld	a,h
	adc	a,(ix-0x0F)
	ld	(ix-0x01),a
	ld	a,(ix-0x22)

;; fn0607: 0607
fn0607 proc
	add	a,(ix-0x26)
	ld	c,a
	ld	a,(ix-0x21)

;; fn060E: 060E
;;   Called from:
;;     060B (in fn04EE)
;;     060B (in fn0607)
;;     0E0F (in fn0D00)
;;     0E0F (in fn0DFC)
fn060E proc
	adc	a,(ix-0x25)
	ld	b,a
	ld	a,(ix-0x20)
	adc	a,(ix-0x24)
	ld	l,a
	ld	a,(ix-0x1F)
	adc	a,(ix-0x23)
	ld	h,a
	ld	a,(ix-0x04)
	xor	a,c
	ld	(ix-0x04),a
	ld	a,(ix-0x03)
	xor	a,b
	ld	(ix-0x03),a
	ld	a,(ix-0x02)
	xor	a,l
	ld	(ix-0x02),a
	ld	a,(ix-0x01)
	xor	a,h
	ld	(ix-0x01),a
	push	af
	ld	c,(ix-0x22)
	ld	b,(ix-0x21)
	ld	l,(ix-0x20)
	ld	h,(ix-0x1F)
	pop	af
	ld	a,05

l0650:
	srl	h
	rr	l
	rr	b
	rr	c
	dec	a
	jr	NZ,0650

l065B:
	ld	a,c
	add	a,(ix-0x0E)
	ld	c,a
	ld	a,b
	adc	a,(ix-0x0D)
	ld	b,a
	ld	a,l
	adc	a,(ix-0x0C)
	ld	l,a
	ld	a,h
	adc	a,(ix-0x0B)
	ld	h,a
	ld	a,c
	xor	a,(ix-0x04)
	ld	c,a
	ld	a,b
	xor	a,(ix-0x03)
	ld	b,a
	ld	a,l
	xor	a,(ix-0x02)
	ld	l,a
	ld	a,h
	xor	a,(ix-0x01)
	ld	h,a
	ld	a,(ix-0x1A)
	sub	a,c
	ld	(ix-0x1A),a
	ld	a,(ix-0x19)
	sbc	a,b
	ld	(ix-0x19),a
	ld	a,(ix-0x18)
	sbc	a,l
	ld	(ix-0x18),a
	ld	a,(ix-0x17)
	sbc	a,h
	ld	(ix-0x17),a
	ld	a,(ix-0x26)
	add	a,47
	ld	(ix-0x26),a
	ld	a,(ix-0x25)
	adc	a,86
	ld	(ix-0x25),a
	ld	a,(ix-0x24)
	adc	a,C8
	ld	(ix-0x24),a
	ld	a,(ix-0x23)
	adc	a,61
	ld	(ix-0x23),a
	ld	a,(ix-0x1E)
	add	a,FF
	ld	c,a
	ld	a,(ix-0x1D)
	adc	a,FF
	ld	b,a
	ld	a,(ix-0x1C)
	adc	a,FF
	ld	l,a
	ld	a,(ix-0x1B)
	adc	a,FF
	ld	h,a
	ld	(ix-0x1E),c
	ld	(ix-0x1D),b
	ld	(ix-0x1C),l
	ld	(ix-0x1B),h
	ld	a,h
	or	a,l
	or	a,b
	or	a,c
	jp	NZ,060E

l06F2:
	ld	hl,000C
	add	hl,sp
	ld	bc,0004

l06FA:
	ldir

l06FC:
	ex	de,hl
	ld	hl,(sp+0x20)
	ex	de,hl
	ld	hl,0004
	add	hl,sp
	ld	bc,0004

l0709:
	ldir

l070B:
	ld	sp,ix
	pop	ix
	ret
070F                                              1E                .
0710 38 00 00 20 F9 66 C6 08 39 00 80 3A 23 08 FE 59 8.. .f..9..:#..Y
0720 1E 1A 00 01 00 00 00 00 00 00 00 00 00 00 00 00 ................
0730 00 1A 3F 02 00 00 00 00 00 00 00 00 00 00 00 00 ..?.............
0740 00 1A 4F 02 00 00 00 00 00 1F 00 00 00 00 00 00 ..O.............
0750 00 1A 53 02 00 00 00 00 00 1A 57 02 00 00 00 00 ..S.......W.....
0760 00 1F 00 00 00 00 00 00 00 1A 60 02 00 00 00 00 ..........`.....
0770 00 1A 64 02 00 00 00 00 00 1A 68 02 00 00 00 00 ..d.......h.....
0780 00 1A 6C 02 00 00 00 00 00 1F 00 00 00 00 00 00 ..l.............
0790 00 1A 32 02 00 00 00 00 00 1A 39 02 00 00 00 00 ..2.......9.....
07A0 00 00 3E A0 FF EF C2 6E 04 37 C6 F7 37 C7 A5 37 ..>....n.7..7..7
07B0 C8 00 37 C9 03 37 CD 03 37 CE F0 37 CB A0 37 CF ..7..7..7..7..7.
07C0 00 37 E4 35 37 D0 08 37 D1 08 37 E5 01 37 DA 40 .7.57..7..7..7.@
07D0 37 E9 01 EF E9 6E 08 EF E9 6E 20 37 EA 00 37 D2 7....n...n 7..7.
07E0 C0 37 DB 20 EF DB 6E E0 B8 E6 B9 E6 3A 20 FF 36 .7. ..n.....: .6
07F0 00 1C A4 02 EA 26 92 7F C0 FF CE F5 1C A4 02 B0 .....&..........
0800 BF 1C 00 00 1C                                  .....          

;; fn0805: 0805
;;   Called from:
;;     1BC1 (in fn1BA4)
fn0805 proc
	sla
	di
	call	0823
	call	0822
	halt
080E                                           50 E3               P.
0810 00 00 4D E7 ED 28 E3 00 00 2E 2F ED E5 2E EF ED ..M..(..../.....
0820 20 EB                                            .             

;; fn0822: 0822
;;   Called from:
;;     080A (in fn0805)
fn0822 proc
	nop

;; fn0823: 0823
;;   Called from:
;;     0807 (in fn0805)
;;     0822 (in fn0822)
fn0823 proc
	nop
	ld	a,a
	pop	bc
	ret
0827                      50 E3 00 00 4D E7 ED 28 E3        P...M..(.
0830 00 00 2E 2F ED E3 00 00 2E ED 26 EF ED 20 58 1E .../......&.. X.
0840 27 EB 2F BE B9 BF 1F B0 BF 37 C3 0F 1F 52 F2 04 './......7...R..
0850 4A 4F BC F2 02 4A 4F BA 1C 00 00 5A 1F 1C 00 00 JO...JO....Z....
0860 1F 1C 00 00 1F 97 B8 37 D3 4E 37 C2 10 1F 1C 00 .......7.N7.....
0870 00 1F 1C 00 00 1F 1C 00 00 1F 1C 00 00 1F F2 02 ................
0880 2E A8 BF C6 0D 02 A8 C3 CE 08 03 00 00 1C A4 02 ................
0890 C8 EF 02 37 C3 0F 2F EB B8 BF 03 1E F2 02 4A E2 ...7../.......J.
08A0 2E FE 66 FE D6 1C 73 02 92 C8 F4 37 D2 00 37 D3 ..f...s....7..7.
08B0 B1 1E 37 D3 4E 1E 54 3C 00 00 FE 14 16 DA FF 08 ..7.N.T<........
08C0 F2 2A 4A 08 51 3A 1E 00 FE 70 08 38 04 00 FE 59 .*J.Q:...p.8...Y
08D0 59 3A 04 00 F9 70 F6 24 42 51 08 F2 26 4A 08 3A Y:...p.$BQ..&J.:
08E0 1A 00 FE 70 08 38 04 00 FE 59 59 F0 06 29 F0 07 ...p.8...YY..)..
08F0 28 51 50 F9 33 F8 32 3A 10 00 FE 70 08 38 04 00 (QP.3.2:...p.8..
0900 FE 59 58 59 3A 04 00 F8 70 51 50 08 3A 0C 00 FE .YXY:...pQP.:...
0910 70 08 38 04 00 FE 59 58 59 3A 08 00 F8 70 51 50 p.8...YXY:...pQP
0920 08 3A 08 00 FE 70 08 38 04 00 FE 59 58 59 3A 0C .:...p.8...YXY:.
0930 00 F8 70 51 08 3A 02 00 FE 70 08 38 04 00 FE 59 ..pQ.:...p.8...Y
0940 59 F4 EA 37 20 FE 65 F4 EB 26 F4 EC 26 F4 ED 26 Y..7 .e..&..&..&
0950 FE 65 F4 EE 26 F4 EF 26 F4 F0 26 F4 F1 26 F0 EE .e..&..&..&..&..
0960 2E 68 B9 F4 EE 26 F0 EF 2E 69 79 F4 EF 26 F0 F0 .h...&...iy..&..
0970 2E 69 37 F4 F0 26 F0 F1 2E 69 9E F4 F1 26 56 F0 .i7..&...i...&V.
0980 F2 29 F0 F3 28 F0 F4 2D F0 F5 2C 5E 36 04 F9 A4 .)..(..-..,^6...
0990 F8 A2 FA 71 8E CE F7 21 F0 E6 60 F4 FA 26 20 F0 ...q...!..`..& .
09A0 E7 61 F4 FB 26 25 F0 E8 61 F4 FC 26 24 F0 E9 61 .a..&%..a..&$..a
09B0 F4 FD 26 F0 F2 2E F0 EE 60 29 F0 F3 2E F0 EF 61 ..&.....`).....a
09C0 28 F0 F4 2E F0 F0 61 2D F0 F5 2E F0 F1 61 2C F0 (.....a-.....a,.
09D0 FA 2E F9 65 F4 FA 26 F0 FB 2E F8 65 F4 FB 26 F0 ...e..&....e..&.
09E0 FC 2E FD 65 F4 FC 26 F0 FD 2E FC 65 F4 FD 26 56 ...e..&....e..&V
09F0 F0 F2 29 F0 F3 28 F0 F4 2D F0 F5 2C 5E 36 05 FC ..)..(..-..,^6..
0A00 A7 FD A3 F8 A3 F9 A3 8E CE F5 21 F0 E2 60 29 20 ..........!..`) 
0A10 F0 E3 61 28 25 F0 E4 61 2D 24 F0 E5 61 2C 21 F0 ..a(%..a-$..a,!.
0A20 FA 65 29 20 F0 FB 65 28 25 F0 FC 65 2D 24 F0 FD .e) ..e(%..e-$..
0A30 65 2C F0 F6 2E F9 60 F4 F6 26 F0 F7 2E F8 61 F4 e,....`..&....a.
0A40 F7 26 F0 F8 2E FD 61 F4 F8 26 F0 F9 2E FC 61 F4 .&....a..&....a.
0A50 F9 26 56 F0 F6 29 F0 F7 28 F0 F8 2D F0 F9 2C 5E .&V..)..(..-..,^
0A60 36 04 F9 A4 F8 A2 FA 71 8E CE F7 21 F0 DE 60 F4 6......q...!..`.
0A70 FA 26 20 F0 DF 61 F4 FB 26 25 F0 E0 61 F4 FC 26 .& ..a..&%..a..&
0A80 24 F0 E1 61 F4 FD 26 F0 F6 2E F0 EE 60 29 F0 F7 $..a..&.....`)..
0A90 2E F0 EF 61 28 F0 F8 2E F0 F0 61 2D F0 F9 2E F0 ...a(.....a-....
0AA0 F1 61 2C F0 FA 2E F9 65 F4 FA 26 F0 FB 2E F8 65 .a,....e..&....e
0AB0 F4 FB 26 F0 FC 2E FD 65 F4 FC 26 F0 FD 2E FC 65 ..&....e..&....e
0AC0 F4 FD 26 56 F0 F6 29 F0 F7 28 F0 F8 2D F0 F9 2C ..&V..)..(..-..,
0AD0 5E 36 05 FC A7 FD A3 F8 A3 F9 A3 8E CE F5 21 F0 ^6............!.
0AE0 DA 60 29 20 F0 DB 61 28 25 F0 DC 61 2D 24 F0 DD .`) ..a(%..a-$..
0AF0 61 2C 21 F0 FA 65 29 20 F0 FB 65 28 25 F0 FC 65 a,!..e) ..e(%..e
0B00 2D 24 F0 FD 65 2C F0 F2 2E F9 60 F4 F2 26 F0 F3 -$..e,....`..&..
0B10 2E F8 61 F4 F3 26 F0 F4 2E FD 61 F4 F4 26 F0 F5 ..a..&....a..&..
0B20 2E FC 61 F4 F5 26 F0 EA 2E 68 FF 29 F0 EB 2E 69 ..a..&...h.)...i
0B30 FF 28 F0 EC 2E 69 FF 2D F0 ED 2E 69 FF 2C F4 EA .(...i.-...i.,..
0B40 21 F4 EB 20 F4 EC 25 F4 ED 24 24 FD 66 F8 66 F9 !.. ..%..$$.f.f.
0B50 66 EB 50 03 CE 3A 1C 00 FE 70 38 04 00 FE 59 08 f.P..:...p8...Y.
0B60 F2 24 4A 08 3A 18 00 FE 70 38 04 00 FE 59 FC 3E .$J.:...p8...Y.>
0B70 5C 1E 54 3C 00 00 FE 14 16 DA FF 08 F2 2A 4A 08 \.T<.........*J.
0B80 51 3A 0E 00 FE 70 08 38 04 00 FE 59 59 3A 04 00 Q:...p.8...YY:..
0B90 F9 70 F6 20 42 51 08 F2 22 4A 08 3A 06 00 FE 70 .p. BQ.."J.:...p
0BA0 08 38 04 00 FE 59 59 F0 06 29 F0 07 28 51 50 F9 .8...YY..)..(QP.
0BB0 33 F8 32 3A 18 00 FE 70 08 38 04 00 FE 59 58 59 3.2:...p.8...YXY
0BC0 3A 04 00 F8 70 51 50 08 3A 1C 00 FE 70 08 38 04 :...pQP.:...p.8.
0BD0 00 FE 59 58 59 3A 08 00 F8 70 51 50 08 3A 20 00 ..YXY:...pQP.: .
0BE0 FE 70 08 38 04 00 FE 59 58 59 3A 0C 00 F8 70 51 .p.8...YXY:...pQ
0BF0 08 3A 12 00 FE 70 08 38 04 00 FE 59 59 F4 E2 37 .:...p.8...YY..7
0C00 20 FE 65 F4 E3 26 F4 E4 26 F4 E5 26 F4 DA 37 20  .e..&..&..&..7 
0C10 F4 DB 37 37 F4 DC 37 EF F4 DD 37 C6 56          ..77..7...7.V  

;; fn0C1D: 0C1D
fn0C1D proc
	ld	c,(ix-0x1A)
	ld	b,(ix-0x19)
	ld	l,(ix-0x18)
	ld	h,(ix-0x17)
	pop	af
	ld	a,04

l0C2C:
	sla	c
	rl	b
	adc	hl,hl
	dec	a
	jr	NZ,0C2C

;; fn0C35: 0C35
;;   Called from:
;;     0C33 (in fn0C1D)
;;     168F (in fn164F)
;;     1704 (in fn164F)
;;     1709 (in fn164F)
;;     187D (in fn1873)
fn0C35 proc
	ld	a,c
	add	a,(ix-0x0A)

;; fn0C39: 0C39
fn0C39 proc
	ld	(ix-0x04),a
	ld	a,b
	adc	a,(ix-0x09)
	ld	(ix-0x03),a
	ld	a,l
	adc	a,(ix-0x08)
	ld	(ix-0x02),a
	ld	a,h
	adc	a,(ix-0x07)
	ld	(ix-0x01),a
	ld	a,(ix-0x1A)
	add	a,(ix-0x26)
	ld	c,a
	ld	a,(ix-0x19)
	adc	a,(ix-0x25)
	ld	b,a
	ld	a,(ix-0x18)
	adc	a,(ix-0x24)
	ld	l,a
	ld	a,(ix-0x17)
	adc	a,(ix-0x23)
	ld	h,a
	ld	a,(ix-0x04)
	xor	a,c
	ld	(ix-0x04),a
	ld	a,(ix-0x03)
	xor	a,b
	ld	(ix-0x03),a
	ld	a,(ix-0x02)

;; fn0C80: 0C80
;;   Called from:
;;     0C7D (in fn0C39)
fn0C80 proc
	xor	a,l
	ld	(ix-0x02),a
	ld	a,(ix-0x01)
	xor	a,h
	ld	(ix-0x01),a
	push	af
	ld	c,(ix-0x1A)
	ld	b,(ix-0x19)
	ld	l,(ix-0x18)
	ld	h,(ix-0x17)
	pop	af
	ld	a,05

;; fn0C9D: 0C9D
;;   Called from:
;;     0C9B (in fn0C80)
;;     0C9B (in fn0C35)
fn0C9D proc
	srl	h
	rr	l
	rr	b
	rr	c
	dec	a
	jr	NZ,0C9D

l0CA8:
	ld	a,c
	add	a,(ix-0x16)
	ld	c,a
	ld	a,b
	adc	a,(ix-0x15)
	ld	b,a
	ld	a,l
	adc	a,(ix-0x14)
	ld	l,a

;; fn0CB7: 0CB7
fn0CB7 proc
	ld	a,h
	adc	a,(ix-0x13)
	ld	h,a
	ld	a,c
	xor	a,(ix-0x04)
	ld	c,a
	ld	a,b
	xor	a,(ix-0x03)
	ld	b,a
	ld	a,l
	xor	a,(ix-0x02)
	ld	l,a
	ld	a,h
	xor	a,(ix-0x01)
	ld	h,a
	ld	a,(ix-0x22)
	sub	a,c
	ld	(ix-0x22),a
	ld	a,(ix-0x21)
	sbc	a,b
	ld	(ix-0x21),a
	ld	a,(ix-0x20)
	sbc	a,l
	ld	(ix-0x20),a
	ld	a,(ix-0x1F)
	sbc	a,h
	ld	(ix-0x1F),a
	push	af
	ld	c,(ix-0x22)
	ld	b,(ix-0x21)
	ld	l,(ix-0x20)
	ld	h,(ix-0x1F)
	pop	af
	ld	a,04

;; fn0D00: 0D00
;;   Called from:
;;     0CFE (in fn0CB7)
;;     0CFE (in fn0C9D)
fn0D00 proc
	sla	c
	rl	b
	adc	hl,hl
	dec	a
	jr	NZ,0D00

l0D09:
	ld	a,c
	add	a,(ix-0x12)
	ld	(ix-0x04),a
	ld	a,b
	adc	a,(ix-0x11)
	ld	(ix-0x03),a
	ld	a,l
	adc	a,(ix-0x10)
	ld	(ix-0x02),a
	ld	a,h
	adc	a,(ix-0x0F)
	ld	(ix-0x01),a
	ld	a,(ix-0x22)
	add	a,(ix-0x26)
	ld	c,a
	ld	a,(ix-0x21)
	adc	a,(ix-0x25)
	ld	b,a
	ld	a,(ix-0x20)
	adc	a,(ix-0x24)
	ld	l,a
	ld	a,(ix-0x1F)
	adc	a,(ix-0x23)
	ld	h,a
	ld	a,(ix-0x04)
	xor	a,c
	ld	(ix-0x04),a
	ld	a,(ix-0x03)
	xor	a,b
	ld	(ix-0x03),a
	ld	a,(ix-0x02)
	xor	a,l
	ld	(ix-0x02),a
	ld	a,(ix-0x01)
	xor	a,h
	ld	(ix-0x01),a
	push	af
	ld	c,(ix-0x22)
	ld	b,(ix-0x21)
	ld	l,(ix-0x20)
	ld	h,(ix-0x1F)
	pop	af
	ld	a,05

l0D71:
	srl	h
	rr	l
	rr	b
	rr	c
	dec	a
	jr	NZ,0D71

l0D7C:
	ld	a,c
	add	a,(ix-0x0E)
	ld	c,a
	ld	a,b
	adc	a,(ix-0x0D)
	ld	b,a
	ld	a,l
	adc	a,(ix-0x0C)
	ld	l,a
	ld	a,h
	adc	a,(ix-0x0B)
	ld	h,a
	ld	a,c
	xor	a,(ix-0x04)
	ld	c,a
	ld	a,b
	xor	a,(ix-0x03)
	ld	b,a
	ld	a,l
	xor	a,(ix-0x02)
	ld	l,a
	ld	a,h
	xor	a,(ix-0x01)
	ld	h,a
	ld	a,(ix-0x1A)
	sub	a,c
	ld	(ix-0x1A),a
	ld	a,(ix-0x19)
	sbc	a,b
	ld	(ix-0x19),a
	ld	a,(ix-0x18)
	sbc	a,l
	ld	(ix-0x18),a
	ld	a,(ix-0x17)
	sbc	a,h
	ld	(ix-0x17),a
	ld	a,(ix-0x26)
	add	a,47
	ld	(ix-0x26),a
	ld	a,(ix-0x25)
	adc	a,86
	ld	(ix-0x25),a
	ld	a,(ix-0x24)
	adc	a,C8
	ld	(ix-0x24),a
	ld	a,(ix-0x23)
	adc	a,61
	ld	(ix-0x23),a
	ld	a,(ix-0x1E)
	add	a,FF
	ld	c,a
	ld	a,(ix-0x1D)
	adc	a,FF
	ld	b,a
	ld	a,(ix-0x1C)
	adc	a,FF
	ld	l,a
	ld	a,(ix-0x1B)
	adc	a,FF
	ld	h,a

;; fn0DFC: 0DFC
fn0DFC proc
	ld	(ix-0x1E),c
	ld	(ix-0x1D),b
	ld	(ix-0x1C),l
	ld	(ix-0x1B),h
	ld	a,h
	or	a,l
	or	a,b
	or	a,c
	jp	NZ,060E

;; fn0E13: 0E13
;;   Called from:
;;     0E0F (in fn0DFC)
;;     0E0F (in fn0D00)
fn0E13 proc
	ld	hl,000C
	add	hl,sp
	ld	bc,0004

l0E1B:
	ldir

l0E1D:
	ex	de,hl
	ld	hl,(sp+0x20)
	ex	de,hl
	ld	hl,0004
	add	hl,sp
	ld	bc,0004

l0E2A:
	ldir

l0E2C:
	ld	sp,ix
	pop	ix
	ret
0E30 1E 38 00 00 20 F9 66 C6 08 39 00                .8.. .f..9.    

l0E3B:
	inc	b
	ld	hl,0823

l0E3F:
	ldir

l0E41:
	ret
0E42       1A 00 01 00 00 00 00 00 00 00 00 00 00 00   ..............
0E50 00 00 1A 3F 02 00 00 00 00 00 00 00 00 00 00 00 ...?............
0E60 00 00 1A 4F 02 00 00 00 00 00 1F 00 00 00 00 00 ...O............
0E70 00 00 1A 53 02 00 00 00 00 00 1A 57 02 00 00 00 ...S.......W....
0E80 00 00 1F 00 00 00 00 00 00 00 1A 60 02 00 00 00 ...........`....
0E90 00 00 1A 64 02 00 00 00 00 00 1A 68 02 00 00 00 ...d.......h....
0EA0 00 00 1A 6C 02 00 00 00 00 00 1F 00 00 00 00 00 ...l............
0EB0 00 00 1A 32 02 00 00 00 00 00 1A 39 02 00 00 00 ...2.......9....
0EC0 00 00 00 3E A0 FF EF C2 6E 04 37 C6 F7 37 C7 A5 ...>....n.7..7..
0ED0 37 C8 00 37 C9 03 37 CD 03 37 CE F0 37 CB A0 37 7..7..7..7..7..7
0EE0 CF 00 37 E4 35 37 D0 08 37 D1 08 37 E5 01 37 DA ..7.57..7..7..7.
0EF0 40 37 E9 01 EF E9 6E 08 EF E9 6E 20 37 EA 00 37 @7....n...n 7..7
0F00 D2 C0 37 DB 20 EF DB 6E E0 B8 E6 B9 E6 3A 20 FF ..7. ..n.....: .
0F10 36 00 1C A4 02 EA 26 92 7F C0 FF CE F5 1C A4 02 6.....&.........
0F20 B0 BF 1C 00 00 1C A4 02 1C 89 1C 1C 29 19 01 50 ............)..P
0F30 E3 00 00 4D E7 ED 28 E3 00 00 2E 2F ED E5 2E EF ...M..(..../....
0F40 ED 20 EB 00 00 26 58 1E 50 E3 00 00 4D E7 ED 28 . ...&X.P...M..(
0F50 E3 00 00 2E 2F ED E3 00 00 2E ED 26 EF ED 20 58 ..../......&.. X
0F60 1E 27 EB 2F BE B9 BF 1F B0 BF 37 C3 0F 1F 52 F2 .'./......7...R.
0F70 04 4A 4F BC F2 02 4A 4F BA 1C 00 00 5A 1F 1C 00 .JO...JO....Z...
0F80 00 1F 1C 00 00 1F 97 B8 37 D3 4E 37 C2 10 1F 1C ........7.N7....
0F90 00 00 1F 1C 00 00                               ......         

l0F96:
	reti
0F97                      1C 00 00                          ...     

;; fn0F9A: 0F9A
fn0F9A proc
	reti
0F9B                                  1C 00 00 1F F2            .....
0FA0 02 2E A8 BF C6 0D 02 A8 C3 CE 08 03 00 00 1C A4 ................
0FB0 02 C8 EF 02 37 C3 0F 2F EB B8 BF 03 1E F2 02 4A ....7../.......J
0FC0 E2 2E FE 66 FE D6 1C 73 02 92 C8 F4 37 D2 00 37 ...f...s....7..7
0FD0 D3 B1 1E 37 D3 4E 1E 54 F2 08 4A 52 F2 06 4A 52 ...7.N.T..JR..JR
0FE0 1C A4 1B 16 04 00 52 F2 0C 4A 52 F2 0A 4A 52 1C ......R..JR..JR.
0FF0 A4 1B 16 04 00 58 F8 70 5C 1E 54 F2 0A 4A 52 F2 .....X.p\.T..JR.
1000 06 4A 52 1C A4 1B 16 04 00 52 F2 0A 4A 52 F2 0A .JR......R..JR..
1010 4A 52 1C A4 1B 16 04 00 58 21 FD 62 2D 20 FC 63 JR......X!.b- .c
1020 2C 5C 1E 54 3C 00 00 FE 14 F2 0A 4A F0 08 29 F0 ,\.T<......J..).
1030 09 28 08 F2 06 4A 08 F2 04 4D 52 51 50 55 1C A4 .(...J...MRQPU..
1040 1B 16 04 00 FD 31 FC 30 59 5A 50 52 51 1C A4 1B .....1.0YZPRQ...
1050 16 04 00 58 F8 70 FD 31 FC 30 F9 33 20 2A F9 66 ...X.p.1.0.3 *.f
1060 CE 03 2D C8 16 FE 65 FB 67 FA 63 EB 42 03 CC 6D ..-...e.g.c.B..m
1070 80 EB 4A 03 CD 35 01 C8 02 35 FF 5C 1E 54 3C 00 ..J..5...5.\.T<.
1080 00 FE 14 F0 0A 29 F0 0B 28 F2 08 4A 08 F2 06 4A .....)..(..J...J
1090 08 F2 04 4D 52 51 50 55 1C A4 1B 16 04 00 FD 31 ...MRQPU.......1
10A0 FC 30 59 5A 50 52 51 1C A4 1B 16 04 00 58 21 FD .0YZPRQ......X!.
10B0 62 29 20 FC 63 28 F9 33 20 2A F9 66 CE 03 2D C8 b) .c(.3 *.f..-.
10C0 16 FE 65 FB 67 FA 63 EB 9E 03 CC 6D 80 EB A6 03 ..e.g.c....m....
10D0 CD 35 01 C8 02 35 FF 5C 1E 54 F2 04 4A 52 F2 06 .5...5.\.T..JR..
10E0 4A 52 1C A4 1B 16 04 00 52 F2 08 4A 52 F2 0A 4A JR......R..JR..J
10F0 52 1C A4 1B 16 04 00 58 F8 70 5C 1E 54 3C 00 00 R......X.p\.T<..
1100 FE 14 16 F6 FF 08 F2 12 4A 08 F0 06 2E F4 FE 26 ........J......&
1110 F0 07 2E F4 FF 26 F2 08 4A 92 92 E2 29 92 E2 28 .....&..J...)..(
1120 F0 04 2E F4 FC 26 F0 05 2E F4 FD 26 F0 FC 2E 68 .....&.....&...h
1130 02 F4 FA 26 F0 FD 2E 69 00 F4 FB 26 F2 04 4A E2 ...&...i...&..J.
1140 2E 92 E2 2C 2D 21 FD 62 29 20 FC 63 28 F9 35 F8 ...,-!.b) .c(.5.
1150 34 FA 70 F8 70 FA 70 F8 70 FA 70 F8 70 FA 70 F8 4.p.p.p.p.p.p.p.
1160 70 FA 70 F8 70 FA 70 F8 70 FA 70 F8 70 FA 70 F8 p.p.p.p.p.p.p.p.
1170 70 FA 70 FA 70 FA 70 F8 70 FA 70 F8 70 FA 70 F8 p.p.p.p.p.p.p.p.
1180 70 FA 70 FA 70 F6 02 42 FB 35 FA 34 F0 F8 2E EA p.p.p..B.5.4....
1190 26 92 F0 F9 2E EA 26 3A 02 00 F9 70 E6 52 F2 08 &.....&:...p.R..
11A0 4A E2 29 92 E2 28 F2 06 4A E2 2E 92 E2 2C 2D 21 J.)..(..J....,-!
11B0 FD 62 29 20 FC 63 28 F9 35 F8 34 FA 70 F8 70 FA .b) .c(.5.4.p.p.
11C0 70 FA 70 FA 70 F8 70 FA 70 FA 70 FD 31 FC 30 5A p.p.p.p.p.p.1.0Z
11D0 52 EA 21 92 EA 20 3A 04 00 F9 70 E6 52 F2 06 4A R.!.. :...p.R..J
11E0 E2 2B 92 E2 2A 50 51 F2 06 4A 52 1C A4 1B 16 04 .+..*PQ..JR.....
11F0 00 08 58 F2 04 4A E2 2E 92 E2 2C 2D 51 52 50 1C ..X..J....,-QRP.
1200 A4 1B 16 04 00 59 F9 70 FE 65 FD 62 29 36 00 FC .....Y.p.e.b)6..
1210 63 28 5A 52 EA 21 92 EA 20 FC 3E 5C 1E 54 3C 00 c(ZR.!.. .>\.T<.
1220 00 FE 14 16 E8 FF F0 04 29 F0 05 28 E0 2E F4 EA ........)..(....
1230 26 90 E0 2E F4 EB 26 98 3A 02 00 F8 70 E6 52 F0 &.....&.:...p.R.
1240 EB 2E F0 EA 66 CE 0E 5A 52 E2 2B 92 E2 2A 22 FB ....f..ZR.+..*".
1250 66 EB 53 05 C6 08 F2 1E 4A 08 E1 2E F4 F0 26 91 f.S.....J.....&.
1260 E1 2E F4 F1 26 99 3A 02 00 F9 70 F6 10 42 F0 F1 ....&.:...p..B..
1270 2E F0 F0 66 CE 11 F2 10 4A E2 2E 92 E2 2C FC 66 ...f....J....,.f
1280 CE 05 35 00 1A 01 08 3A 04 00 F9 70 F6 0E 42 3A ..5....:...p..B:
1290 04 00 F8 70 F6 0C 42 F0 08 2E F4 F2 26 F0 09 2E ...p..B.....&...
12A0 F4 F3 26 5A 52 E2 2E F4 EE 26 92 E2 2E F4 EF 26 ..&ZR....&.....&
12B0 F2 0C 4A E2 2E F4 EC 26 92 E2 2E F4 ED 26 F0 F2 ..J....&.....&..
12C0 2E 68 02 F4 FA 26 F0 F3 2E 69 00 F4 FB 26 FE 65 .h...&...i...&.e
12D0 F0 EC 62 F4 FE 26 36 00 F0 ED 63 F4 FF 26 F0 EB ..b..&6...c..&..
12E0 2E F0 EA 66 EB 6B 06 CE F0 F1 2E F0 F0 66 EB 04 ...f.k.......f..
12F0 06 CE F2 0E 4A E2 29 92 E2 28 50 F2 08 4A 52 1C ....J.)..(P..JR.
1300 A4 1B 16 04 00 FD 31 FC 30 F2 10 4A E2 2B 92 E2 ......1.0..J.+..
1310 2A 50 F2 06 4A 52 51 1C A4 1B 16 04 00 08 58 23 *P..JRQ.......X#
1320 F9 62 CE 0A 22 F8 62 CE 05 35 02 1A 01 08 35 00 .b..".b..5....5.
1330 1A 01 08 51 F2 08 4A 52 F2 1A 4A 52 1C DE 1B 16 ...Q..JR..JR....
1340 04 00 FD 31 FC 30 59 F2 12 4A EA 21 92 EA 20 F2 ...1.0Y..J.!.. .
1350 10 4A E2 2E 92 E2 2C 2D 51 50 52 1C A4 1B 16 04 .J....,-QPR.....
1360 00 FD 31 FC 30 59 F2 0E 4A E2 2E 92 E2 2C 2D F8 ..1.0Y..J....,-.
1370 70 FE 65 FD 62 29 36 00 FC 63 28 08 E2 2B 92 E2 p.e.b)6..c(..+..
1380 2A 51 50 1C DE 1B 16 04 00 FD 31 FC 30 F2 0A 4A *QP.......1.0..J
1390 EA 21 92 EA 20 35 01 1A 01 08 F2 10 4A E2 2E F4 .!.. 5......J...
13A0 FC 26 92 E2 2E F4 FD 26 50 51 F2 08 4A 52 F2 0E .&.....&PQ..JR..
13B0 4A 52 1C A4 1B 16 04 00 F4 ED 24 F4 EC 25 59 58 JR........$..%YX
13C0 F0 EF 2E F0 EE 66 EB 38 07 CE F0 FD 2E F0 FC 66 .....f.8.......f
13D0 EB D4 06 CE F2 0E 4A E2 29 92 E2 28 50 F2 04 4A ......J.)..(P..J
13E0 52 1C A4 1B 16 04 00 FD 31 FC 30 F0 EC 2E F9 62 R.......1.0....b
13F0 CE 0C F0 ED 2E F8 62 CE 05 35 02 1A 01 08 35 00 ......b..5....5.
1400 1A 01 08 51 F2 04 4A 52 F2 1A 4A 52 1C DE 1B 16 ...Q..JR..JR....
1410 04 00 FD 31 FC 30 59 F2 0A 4A EA 21 92 EA 20 08 ...1.0Y..J.!.. .
1420 E2 2B 92 E2 2A 50 51 1C A4 1B 16 04 00 FD 31 FC .+..*PQ.......1.
1430 30 F2 0E 4A E2 2E 92 E2 2C 2D F8 70 FE 65 FD 62 0..J....,-.p.e.b
1440 29 36 00 FC 63 28 F2 10 4A E2 2B 92 E2 2A 51 50 )6..c(..J.+..*QP
1450 1C DE 1B 16 04 00 FD 31 FC 30 F2 12 4A EA 21 92 .......1.0..J.!.
1460 EA 20 35 01 1A 01 08 50 F2 08 4A 52 F2 0C 4A 52 . 5....P..JR..JR
1470 1C A4 1B 16 04 00 52 F2 18 4A 52 F2 08 4A 52 1C ......R..JR..JR.
1480 A4 1B 16 04 00 59 58 23 FD 62 F4 FC 26 22 FC 63 .....YX#.b..&".c
1490 F4 FD 26 F2 0E 4A E2 2B 92 E2 2A 50 51 F2 06 4A ..&..J.+..*PQ..J
14A0 52 1C A4 1B 16 04 00 08 58 F0 FD 2E F0 FC 66 CE R.......X.....f.
14B0 18 F0 EC 2E FB 62 CE 0C F0 ED 2E FA 62 CE 05 35 .....b......b..5
14C0 02 1A 01 08 35 00 1A 01 08 23 F0 EC 62 2B 22 F0 ....5....#..b+".
14D0 ED 63 2A 50 F2 16 4A 52 51 1C DE 1B 16 04 00 08 .c*P..JRQ.......
14E0 58 F2 12 4A EA 23 92 EA 22 5A 52 E2 2E 92 E2 2C X..J.#.."ZR....,
14F0 2D 50 51 52 1C A4 1B 16 04 00 08 58 F2 0C 4A E2 -PQR.......X..J.
1500 2E 92 E2 2C 2D F9 70 FE 65 FD 62 2B 36 00 FC 63 ...,-.p.e.b+6..c
1510 2A F9 35 F8 34 E2 29 92 E2 28 50 51 1C DE 1B 16 *.5.4.)..(PQ....
1520 04 00 FD 31 FC 30 F2 0A 4A EA                   ...1.0..J.     

l152A:
	ld	a,c
	inc	hl
	ld	(hl),b
	ld	l,01
	ld	sp,ix
	pop	ix
	ret
1534             54 3C 00 00 FE 14 16 D9 FF 3A 08 00     T<.......:..
1540 FE 70 FD 31 FC 30 F9 33 F8 32 50 51 F2 31 4A 52 .p.1.0.3.2PQ.1JR
1550 F2 31 4A 52 1C CD 03 16 06 00 58 3A 02 00 FE 70 .1JR......X:...p
1560 FD 33 FC 32 50 51 52 F2 37 4A 52 F2 37 4A 52 1C .3.2PQR.7JR.7JR.
1570 CD 03 16 06 00 59 58 F2 33 4A 52 51 50 1C EE 04 .....YX.3JRQP...
1580 16 06 00 F4 DA 25 F4 E9 25 F0 DA 2E FE 66 CE 04 .....%..%....f..
1590 2D 1A 35 0C F0 08 2E F4 F6 26 F0 09 2E F4 F7 26 -.5......&.....&
15A0 F0 06 2E F4 E7 26 F0 07 2E F4 E8 26 F0 04 2E F4 .....&.....&....
15B0 FE 26 F0 05 2E F4 FF 26 F2 25 4A E2 2E F4 FC 26 .&.....&.%J....&
15C0 92 E2 2E F4 FD 26 F0 0A 2E F4 FA 26 F0 0B 2E F4 .....&.....&....
15D0 FB 26 F2 1D 4A E2 2E F4 F8 26 92 E2 2E F4 F9 26 .&..J....&.....&
15E0 F0 FE 2E 68 02 F4 F4 26 F0 FF 2E 69 00 F4 F5 26 ...h...&...i...&
15F0 F0 F6 2E 68 02 F4 F2 26 F0 F7 2E 69 00 F4 F3 26 ...h...&...i...&
1600 F0 FA 2E 68 02 F4 F0 26 F0 FB 2E 69 00 F4 F1 26 ...h...&...i...&
1610 F0 E7 2E 68 02 F4 EE 26 F0 E8 2E 69 00 F4 EF 26 ...h...&...i...&
1620 F0 E9 2E 6A 02 EB DD 09 CE F2 19 4A E2 29 92 E2 ...j.......J.)..
1630 28 F2 15 4A E2 2B 92 E2 2A 21 FB 62 2B 20 FA 63 (..J.+..*!.b+ .c
1640 2A F2 0E 4A E2 2E 92 E2 2C 2D F0 F8 2E FD 62    *..J....,-....b

;; fn164F: 164F
fn164F proc
	ld	(ix-0x14),a
	ld	a,(ix-0x07)
	sbc	a,h
	ld	(ix-0x13),a
	ld	hl,(sp+0x1B)
	ld	a,(hl)
	inc	hl
	ld	h,(hl)
	ld	l,a
	ld	a,c
	sub	a,l
	ld	c,a
	ld	a,b
	sbc	a,h
	ld	b,a
	ld	a,(ix-0x08)
	sub	a,(ix-0x04)
	ld	l,a
	ld	a,(ix-0x07)
	sbc	a,(ix-0x03)
	ld	h,a
	push	de
	ex	de,hl
	ld	hl,(sp+0x15)
	ex	de,hl
	push	de
	push	bc
	push	hl
	call	02F4
	add	sp,0008
	ld	a,l
	or	a,a
	jr	NZ,1692

l168D:
	ld	l,03
	jp	0C35

l1692:
	ld	hl,(sp+0x17)
	ld	c,(hl)
	inc	hl
	ld	b,(hl)
	ld	hl,(sp+0x15)
	ld	e,(hl)
	inc	hl
	ld	d,(hl)
	ld	a,c
	sub	a,e
	ld	(ix-0x14),a
	ld	a,b
	sbc	a,d
	ld	(ix-0x13),a
	ld	hl,(sp+0x21)
	ld	e,(hl)
	inc	hl
	ld	d,(hl)
	ld	hl,(sp+0x0E)
	ld	a,(hl)
	inc	hl
	ld	h,(hl)
	ld	l,a
	ld	a,e
	sub	a,l
	ld	(ix-0x16),a
	ld	a,d
	sbc	a,h
	ld	(ix-0x15),a
	ld	hl,(sp+0x1B)
	ld	a,(hl)
	inc	hl
	ld	h,(hl)
	ld	l,a
	ld	a,c
	sub	a,l
	ld	c,a
	ld	a,b
	sbc	a,h
	ld	b,a
	ld	hl,(sp+0x25)
	ld	a,(hl)
	inc	hl
	ld	h,(hl)
	ld	l,a
	ld	a,e
	sub	a,l
	ld	e,a
	ld	a,d
	sbc	a,h
	ld	d,a
	ld	hl,(sp+0x13)
	push	hl
	ld	hl,(sp+0x13)
	push	hl
	push	bc
	push	de
	call	02F4
	add	sp,0008
	ld	a,l
	or	a,a
	jr	NZ,1707

l1702:
	ld	l,03
	jp	0C35

l1707:
	ld	l,00
	jp	0C35
170C                                     F0 0C 2E F4             ....
1710 EA 26 F0 0D 2E F4 EB 26 F0 EA 2E 68 02 F4 EC 26 .&.....&...h...&
1720 F0 EB 2E 69 00 F4 ED 26 F0 FC 2E F0 F8 62 EB 4A ...i...&.....b.J
1730 0A CE F0 FD 2E F0 F9 62 EB 4A 0A CE F2 1B 4A E2 .......b.J....J.
1740 29 92 E2 28 F2 19 4A E2 2B 92 E2 2A 21 FB 62 CE )..(..J.+..*!.b.
1750 28 20 FA 62 CE 23 F2 11 4A F0 FC 2E EA 26 92 F0 ( .b.#..J....&..
1760 FD 2E EA 26 F2 1B 4A E2 29 92 E2 28 F2 13 4A EA ...&..J.)..(..J.
1770 21 92 EA 20 35 02 1A 35 0C F2 21 4A E2 2E F4 FE !.. 5..5..!J....
1780 26 92 E2 2E F4 FF 26 F0 FC 2E F0 FE 62 EB A9 0A &.....&.....b...
1790 CE F0 FD 2E F0 FF 62 EB A9 0A CE F2 1B 4A E2 29 ......b......J.)
17A0 92 E2 28 F2 17 4A E2 2B 92 E2 2A 21 FB 62 CE 28 ..(..J.+..*!.b.(
17B0 20 FA 62 CE 23 F2 11 4A F0 FC 2E EA 26 92 F0 FD  .b.#..J....&...
17C0 2E EA 26 F2 1B 4A E2 29 92 E2 28 F2 13 4A EA 21 ..&..J.)..(..J.!
17D0 92 EA 20 35 02 1A 35 0C F2 0E 4A E2 2E F4 E7 26 .. 5..5...J....&
17E0 92 E2 2E F4 E8 26 F2 15 4A E2 29 92 E2 28 F0 E7 .....&..J.)..(..
17F0 2E F0 F8 62 EB 08 0B CE F0 E8 2E F0 F9 62 EB 08 ...b.........b..
1800 0B CE F2 19 4A E2 2B 92 E2 2A 21 FB 62 CE 28 20 ....J.+..*!.b.( 
1810 FA 62 CE 23 F2 11 4A F0 E7 2E EA 26 92 F0 E8 2E .b.#..J....&....
1820 EA 26 F2 15 4A E2 29 92 E2 28 F2 13 4A EA 21 92 .&..J.)..(..J.!.
1830 EA 20 35 02 1A 35 0C F0 E7 2E F0 FE 62 EB 51 0B . 5..5......b.Q.
1840 CE F0 E8 2E F0 FF 62 EB 51 0B CE F2 17 4A E2 2B ......b.Q....J.+
1850 92 E2 2A 21 FB 62 CE 28 20 FA 62 CE 23 F2 11 4A ..*!.b.( .b.#..J
1860 F0 E7 2E EA 26 92 F0 E8 2E EA 26 F2 15 4A E2 29 ....&.....&..J.)
1870 92 E2 28                                        ..(            

;; fn1873: 1873
fn1873 proc
	ld	hl,(sp+0x13)
	ld	(hl),c
	inc	hl
	ld	(hl),b
	ld	l,02
	jp	0C35
1880 F2 13 4A E2 2B 92 E2 2A 23 F9 62 F4 EE 26 22 F8 ..J.+..*#.b..&".
1890 63 F4 EF 26 F2 11 4A E2 29 92 E2 28 21 F0 E7 62 c..&..J.)..(!..b
18A0 F4 F8 26 20 F0 E8 63 F4 F9 26 F2 1B 4A E2 2E 92 ..& ..c..&..J...
18B0 E2 2C 2D 23 FD 62 2B 22 FC 63 2A 21 F0 FC 62 29 .,-#.b+".c*!..b)
18C0 20 F0 FD 63 28 F2 15 4A 52 F2 21 4A 52 51 50 1C  ..c(..JR.!JRQP.
18D0 F4 02 16 08 00 F4 D9 25 F2 13 4A E2 29 92 E2 28 .......%..J.)..(
18E0 F2 17 4A E2 2B 92 E2 2A 21 FB 62 F4 EC 26 20 FA ..J.+..*!.b..& .
18F0 63 F4 ED 26 F2 11 4A E2 2B 92 E2 2A F2 21 4A E2 c..&..J.+..*.!J.
1900 2E 92 E2 2C 2D 23 FD 62 F4 EA 26 22 FC 63 F4 EB ...,-#.b..&".c..
1910 26 F2 19 4A E2 2E 92 E2 2C 2D 21 FD 62 29 20 FC &..J....,-!.b) .
1920 63 28 F2 1D 4A E2 2E 92 E2 2C 2D 23 FD 62 2B 22 c(..J....,-#.b+"
1930 FC 63 2A F2 13 4A 52 F2 13 4A 52 50 51 1C F4 02 .c*..JR..JRPQ...
1940 16 08 00 F0 D9 2E FE 66 CE 04 FD 66 C6 04 35 00 .......f...f..5.
1950 C8 12 F0 D9 2E FE 66 C6 05 25 FE 66 CE 04 35 02 ......f..%.f..5.
1960 C8 02 35 01 FC 3E 5C 1E 54 3C 00 00 FE 14 16 EB ..5..>\.T<......
1970 FF F4 EB 37 01 F0 04 2E F4 FE 26 F0 05 2E F4 FF ...7......&.....
1980 26 F0 06 2E F4 FC 26 F0 07 2E F4 FD 26 F0 FC 2E &.....&.....&...
1990 68 02 F4 FA 26 F0 FD 2E 69 00 F4 FB 26 F4 ED 37 h...&...i...&..7
19A0 00 3A 00 00 F6 0D 42 F2 0F 4A E2 29 F0 FE 2E 68 .:....B..J.)...h
19B0 02 F4 F6 26 F0 FF 2E 69 00 F4 F7 26 F0 ED 2E F9 ...&...i...&....
19C0 62 EB EE 0C CF F2 13 4A E2 29 92 E2 28 F2 11 4A b......J.)..(..J
19D0 E2 2B 92 E2 2A F2 0D 4D F9 15 E5 2B F1 01 2A 21 .+..*..M...+..*!
19E0 FB 62 CE 23 20 FA 62 CE 1E F2 0B 4A E2 29 92 E2 .b.# .b....J.)..
19F0 28 95 95 E5 2B F1 01 2A 21 FB 62 CE 0A 20 FA 62 (...+..*!.b.. .b
1A00 CE 05 35 02 1A F8 0D F0 F8 2E 68 09 F4 F8 26 F0 ..5.......h...&.
1A10 F9 2E 69 00 F4 F9 26 F0 ED 87 1A 78 0C F4 ED 37 ..i...&....x...7
1A20 00 3A 00 00 F6 0D 42 F2 0F 4A E2 2B F0 ED 2E FB .:....B..J.+....
1A30 62 EB F5 0D CF F0 ED 29 30 00 90 32 00 51 50 1C b......)0..2.QP.
1A40 D2 1B 16 04 00 F4 EC 25 F2 11 4A E2 2B 92 E2 2A .......%..J.+..*
1A50 F0 F8 2E FB 60 F4 F4 26 F0 F9 2E FA 61 F4 F5 26 ....`..&....a..&
1A60 F2 09 4A 92 92 E2 29 92 E2 28 F2 0B 4A E2 2E F4 ..J...)..(..J...
1A70 F2 26 92 E2 2E F4 F3 26 21 F0 F2 62 F4 F0 26 20 .&.....&!..b..& 
1A80 F0 F3 63 F4 F1 26 F2 09 4A E2 2E F4 F4 26 92 E2 ..c..&..J....&..
1A90 2E F4 F5 26 F2 13 4A E2 29 92 E2 28 F0 F4 2E F9 ...&..J.)..(....
1AA0 62 F4 F4 26 F0 F5 2E F8 63 F4 F5 26 51 F0 EC 2B b..&....c..&Q..+
1AB0 32 00 FB 35 FA 34 FA 70 FA 70 FA 70 F9 70 59 F9 2..5.4.p.p.p.pY.
1AC0 70 F6 03 42 F2 03 4A 92 92 E2 2B 92 E2 2A 23 F0 p..B..J...+..*#.
1AD0 F2 62 2B 22 F0 F3 63 2A F2 03 4A E2 2E 92 E2 2C .b+"..c*..J....,
1AE0 F9 62 29 24 F8 63 28 F2 05 4A 52 F2 0B 4A 52 51 .b)$.c(..JR..JRQ
1AF0 50 1C 4E 03 16 08 00 F4 EE 25 25 FE 66 C6 04 35 P.N......%%.f..5
1B00 00 C8 24 F0 EE 2E FE 66 CE 04 F4 EB 37 02 F0 F8 ..$....f....7...
1B10 2E 68 09 F4 F8 26 F0 F9 2E 69 00 F4 F9 26 F0 ED .h...&...i...&..
1B20 87 1A F8 0C F0 EB 2D FC 3E 5C 1E 54 3C 00 00 FE ......-.>\.T<...
1B30 14 16 E9 FF F4 FF 37 00 3A 03 00 FE 70 F6 14 42 ......7.:...p..B
1B40 F0 08 2E F4 FB 26 F0 09 2E F4 FC 26 F0 FB 2E 68 .....&.....&...h
1B50 02 F4 F9 26 F0 FC 2E 69 00 F4 FA 26 F4 E9 37 00 ...&...i...&..7.
1B60 F4 F5 37 00 3A 00 00 F6 0E 42 F2 10 4A E2 2E F4 ..7.:....B..J...
1B70 F6 26 F0 F5 2E F0 F6 62 EB E0 0E CF 08 F2 14 4A .&.....b.......J
1B80 08 F2 12 4A E2 29 92 E2 28 F0 F5 2D 34 00 92 4D ...J.)..(..-4..M
1B90 F0 F6 2D 34 00 50 51 52 55 1C D2 1B 16 04 00 59 ..-4.PQRU......Y
1BA0 58 51 FD 33                                     XQ.3           

;; fn1BA4: 1BA4
fn1BA4 proc
	ld	d,h
	add	hl,hl
	add	hl,hl
	add	hl,hl
	add	hl,de
	pop	de
	add	hl,bc
	ld	iy,(sp+0x0E)
	add	iy,bc
	push	de
	push	hl
	push	iy
	ld	hl,(sp+0x23)
	push	hl
	ld	hl,(sp+0x23)
	push	hl
	call	0805
	add	sp,000A
	ld	e,l
	ld	a,03
	sub	a,e
	jr	C,1BF9

l1BCF:
	ld	d,00
	ld	hl,0EAB
	add	hl,de
	add	hl,de
	jp	(hl)
1BDA                               C8 1D C8 06                 .... 

;; fn1BDE: 1BDE
fn1BDE proc
	jr	1BE9
1BE0 C8 12 C8 15 35 01 1A 96 0F                      ....5....      

l1BE9:
	inc	(ix-0x17)
	ld	a,(ix-0x17)
	ld	(ix-0x01),a
	jr	1BF9
1BF4             35 02 1A 96 0F                          5....      

;; fn1BF9: 1BF9
;;   Called from:
;;     1BCD (in fn1BA4)
;;     1BF2 (in fn1BDE)
fn1BF9 proc
	ld	a,(ix-0x09)
	add	a,09
	ld	(ix-0x09),a
	ld	a,(ix-0x08)
	adc	a,00
	ld	(ix-0x08),a
	inc	(ix-0x0B)
	jp	0E3B
1C0F                                              F0                .
1C10 FF 2E 6A 03 C6 07 F0 FF 2E 6A 04 CE 05 35 01 1A ..j......j...5..
1C20 96 0F F2 12 4A 52 F2 1D 4A 52 1C 39 0C 16 04 00 ....JR..JR.9....
1C30 F4 EB 25 F2 1F 4A 52 F2 1F 4A 52 1C 39 0C 16 04 ..%..JR..JR.9...
1C40 00 F4 EA 25 F0 EB 2E 8E CE 04 36 01 C8 02 FE 65 ...%......6....e
1C50 F4 F6 26 F0 EA 2E 8E CE 04 36 01 C8 02 FE 65 29 ..&......6....e)
1C60 F0 FF 2E FE 66 CE 15 F0 F6 2E FE 66 CE 04 F9 66 ....f......f...f
1C70 C6 05 35 01 1A 96 0F 35 00 1A 96 0F F0 FF 2E 8E ..5....5........
1C80 CE 15 F0 F6 2E FE 66 CE 04                      ......f..      

;; fn1C89: 1C89
fn1C89 proc
	or	a,c
	jr	Z,1C92

l1C8D:
	ld	l,01
	jp	0F96

l1C92:
	ld	l,03
	jp	0F96
1C97                      F0 FF 2E 6A 02 CE 25 F0 F6        ...j..%..
1CA0 2E FE 66 CE 04 F9 66 C6 04 35 01 C8 18 F0 EB 2E ..f...f..5......
1CB0 FE 66 C6 07 F0 EA 2E FE 66 CE 04 35 03 C8 06 35 .f......f..5...5
1CC0 01 C8 02 35 01 FC 3E 5C 1E 54 3C 00 00 FE 14 16 ...5..>\.T<.....
1CD0 D2 FF F4 DA 37 00 F4 D2 37 00 3A 00 00 F6 1E 42 ....7...7.:....B
1CE0 35 00 F6 1C 42 F0 D2 2E F0 06 62 EB FB 11 CF F0 5...B.....b.....
1CF0 04 2E F0 F0 60 F4 EC 26 F0 05 2E F0 F1 61 F4 ED ....`..&.....a..
1D00 26 F0 04 2E F0 EE 60 F4 EA 26 F0 05 2E F0 EF 61 &.....`..&.....a
1D10 F4 EB 26 F0 EA 2E 68 02 F4 E8 26 F0 EB 2E 69 00 ..&...h...&...i.
1D20 F4 E9 26 F4 D4 37 00 3A 00 00 F6 14 42 35 00 F6 ..&..7.:....B5..
1D30 12 42 F2 16 4A E2 2E F4 E3 26 F0 D4 2E F0 E3 62 .B..J....&.....b
1D40 EB D5 11 CF F2 18 4A E2 2E F4 E1 26 92 E2 2E F4 ......J....&....
1D50 E2 26 F0 E1 2E F0 E4 60 F4 DF 26 F0 E2 2E F0 E5 .&.....`..&.....
1D60 61 F4 E0 26 F2 0D 4A E2 2E F4 DD 26 92 E2 2E F4 a..&..J....&....
1D70 DE 26 F0 DE AF EB AF 11 CE 36 64 F0 DD 67 36 00 .&.......6d..g6.
1D80 F0 DE 63 EB 5A 10 CC 6D 80 EB AF 11 C5 F2 0D 4A ..c.Z..m.......J
1D90 92 92 E2 29 92 E2 28 F8 AF EB AF 11 CE 36 64 F9 ...)..(......6d.
1DA0 67 36 00 F8 63 EB 7C 10 CC 6D 80 EB AF 11 C5 F4 g6..c.|..m......
1DB0 DD 37 01 F0 D4 29 30 00 90 F0 E3 2B 32 00 51 50 .7...)0....+2.QP
1DC0 1C D2 1B 16 04 00 F4 D6 25 FD 31 30 00 F9 35 F8 ........%.10..5.
1DD0 34 FA 70 FA 70 FA 70 F8 70 F6 0D 42 F2 0F 4D 08 4.p.p.p.p..B..M.
1DE0 F2 0D 4A 08 F9 15 E5 4A FC AF EB AF 11 CE 36 64 ..J....J......6d
1DF0 FD 67 36 00 FC 63 EB CD 10 CC 6D 80 EB AF 11 C5 .g6..c....m.....
1E00 F1 02 4A FC AF EB AF 11 CE 36 64 FD 67 36 00 FC ..J......6d.g6..
1E10 63 EB E8 10 CC 6D 80 EB AF 11 C5 F4 D3 37 01 3A c....m.......7.:
1E20 03 00 F6 0F 42 F0 D3 2E F0 06 62 EB 55 11 CF F0 ....B.....b.U...
1E30 D3 2E F0 D2 62 EB 3F 11 C6 F2 32 4D 08 F2 0F 4A ....b.?...2M...J
1E40 08 F9 15 F2 1A 4A E2 29 92 E2 28 F2 0D 4A F8 70 .....J.)..(..J.p
1E50 F0 E6 2E F9 60 29 F0 E7 2E F8 61 28 55 52 50 1C ....`)....a(URP.
1E60 FC 0D 16 06 00 8D CE 06 F4 DD 37 00 C8 16 F0 E1 ..........7.....
1E70 2E 68 03 F4 E1 26 F0 E2 2E 69 00 F4 E2 26 F0 D3 .h...&...i...&..
1E80 87 1A F6 10 F0 DD 2E FE 66 EB AF 11 C6 F0 DA 2B ........f......+
1E90 F0 DA 28 80 F2 35 4A 32 00 F9 70 F0 D2 2E EA 26 ..(..5J2..p....&
1EA0 20 29 86 F4 DD 26 F2 35 4A 30 00 F8 70 F0 D4 2E  )...&.5J0..p...
1EB0 EA 26 F0 DD 29 F0 DD 2E 86 F4 DF 26 F2 35 4A 30 .&..)......&.5J0
1EC0 00 F8 70 F0 D2 2E EA 26 F0 DF 29 F0 DF 2E 86 F4 ..p....&..).....
1ED0 DA 26 F2 35 4A 30 00 F8 70 F0 D6 2E EA 26 F0 E6 .&.5J0..p....&..
1EE0 2E 68 09 F4 E6 26 F0 E7 2E 69 00 F4 E7 26 F0 E4 .h...&...i...&..
1EF0 2E 68 09 F4 E4 26 F0 E5 2E 69 00 F4 E5 26 F0 D4 .h...&...i...&..
1F00 87 1A 03 10 F0 F0 2E 68 03 F4 F0 26 F0 F1 2E 69 .......h...&...i
1F10 00 F4 F1 26 F0 EE 2E 68 03 F4 EE 26 F0 EF 2E 69 ...&...h...&...i
1F20 00 F4 EF 26 F0 D2 87 1A B6 0F F4 D2 37 00 3A 00 ...&........7.:.
1F30 00 F6 0F 42 35 00 F6 12 42 F0 06 29 30 00 98 F0 ...B5...B..)0...
1F40 D2 2B 32 00 23 F9 62 22 F8 63 EB 21 12 CC 6D 80 .+2.#.b".c.!..m.
1F50 EB 16 15 CD F0 D2 2E 86 F4 E3 26 F4 E6 26 F0 04 ..........&..&..
1F60 2E F0 E1 60 F4 E8 26 F0 05 2E F0 E2 61 F4 E9 26 ...`..&.....a..&
1F70 F0 04 2E F0 E4 60 F4 EA 26 F0 05 2E F0 E5 61 F4 .....`..&.....a.
1F80 EB 26 F0 EA 2E 68 02 F4 EC 26 F0 EB 2E 69 00 F4 .&...h...&...i..
1F90 ED 26 F4 D5 37 00 3A 00 00 F6 1C 42 35 00 F6 1E .&..7.:....B5...
1FA0 42 F2 1A 4A F0 D5 2E E2 62 EB ED 14 CF F2 18 4A B..J....b......J
1FB0 E2 2E F4 DB 26 92 E2 2E F4 DC 26 F0 DB 2E F0 F0 ....&.....&.....
1FC0 60 F4 DB 26 F0 DC 2E F0 F1 61 F4 DC 26 F2 09 4A `..&.....a..&..J
1FD0 E2 2E F4 FE 26 92 E2 2E F4 FF 26 F0 FF AF EB C7 ....&.....&.....
1FE0 14 CE 36 64 F0 FE 67 36 00 F0 FF 63 EB C3 12 CC ..6d..g6...c....
1FF0 6D 80 EB C7 14 C5 F2 09 4A 92 92 E2 29 92 E2 28 m.......J...)..(
2000 F8 AF EB C7 14 CE 36 64 F9 67 36 00 F8 63 EB E5 ......6d.g6..c..
2010 12 CC 6D 80 EB C7 14 C5 F0 E6 29 F4 FE 21 30 00 ..m.......)..!0.
2020 F9 35 F8 34 FA 70 F8 70 F6 09 42 F0 DB 2E F4 FC .5.4.p.p..B.....
2030 26 F0 DC 2E F4 FD 26 F0 FE 2E F0 06 62 EB C7 14 &.....&.....b...
2040 CF F0 04 2E F0 DB 60 F4 FA 26 F0 05 2E F0 DC 61 ......`..&.....a
2050 F4 FB 26 F0 04 2E F0 FC 60 F4 F8 26 F0 05 2E F0 ..&.....`..&....
2060 FD 61 F4 F9 26 F0 F8 2E 68 02 F4 F6 26 F0 F9 2E .a..&...h...&...
2070 69 00 F4 F7 26 F4 D7 37 00 3A 00 00 F6 22 42 35 i...&..7.:..."B5
2080 00 F6 20 42 F2 24 4A F0 D7 2E E2 62 EB A1 14 CF .. B.$J....b....
2090 F2 26 4A E2 29 92 E2 28 21 F0 F2 60 F4 D8 26 20 .&J.)..(!..`..& 
20A0 F0 F3 61 F4 D9 26 F2 06 4A E2 29 92 E2 28 F8 AF ..a..&..J.)..(..
20B0 EB 7B 14 CE 36 64 F9 67 36 00 F8 63 EB 93 13 CC .{..6d.g6..c....
20C0 6D 80 EB 7B 14 C5 F2 06 4A 92 92 E2 29 92 E2 28 m..{....J...)..(
20D0 F8 AF EB 7B 14 CE 36 64 F9 67 36 00 F8 63 EB B5 ...{..6d.g6..c..
20E0 13 CC 6D 80 EB 7B 14 C5 38 01 01 3A 03 00 F6 06 ..m..{..8..:....
20F0 42 20 F0 06 62 EB 23 14 CF F2 32 4D 08 F2 06 4A B ..b.#...2M...J
2100 08 F9 15 F2 28 4A E2 2B 92 E2 2A F0 F4 2E FB 60 ....(J.+..*....`
2110 2B F0 F5 2E FA 61 2A F2 16 4A E2 2E 92 E2 2C 2D +....a*..J....,-
2120 F0 EE 2E FD 60 2D F0 EF 2E FC 61 2C 50 55 51 52 ....`-....a,PUQR
2130 1C FC 0D 16 06 00 58 8D CE 04 31 00 C8 14 F0 D8 ......X...1.....
2140 2E 68 03 F4 D8 26 F0 D9 2E 69 00 F4 D9 26 80 1A .h...&...i...&..
2150 C2 13 21 FE 66 EB 7B 14 C6 F0 DA 29 F0 DA 2B 83 ..!.f.{....)..+.
2160 F2 35 4A 30 00 F8 70 F0 D2 2E EA 26 23 29 86 F4 .5J0..p....&#)..
2170 DD 26 F2 35 4A 30 00 F8 70 F0 D5 2E EA 26 F0 DD .&.5J0..p....&..
2180 29 F0 DD 2E 86 F4 DF 26 F2 35 4A 30 00 F8 70 F0 )......&.5J0..p.
2190 FE 2E EA 26 F0 DF 29 F0 DF 2E 86 F4 DA 26 F2 35 ...&..)......&.5
21A0 4A 30 00 F8 70 F0 D7 2E EA 26 F0 F4 2E 68 09 F4 J0..p....&...h..
21B0 F4 26 F0 F5 2E 69 00 F4 F5 26 F0 F2 2E 68 09 F4 .&...i...&...h..
21C0 F2 26 F0 F3 2E 69 00 F4 F3 26 F0 D7 87 1A 55 13 .&...i...&....U.
21D0 F0 DB 2E 68 03 F4 DB 26 F0 DC 2E 69 00 F4 DC 26 ...h...&...i...&
21E0 F0 FC 2E 68 03 F4 FC 26 F0 FD 2E 69 00 F4 FD 26 ...h...&...i...&
21F0 F0 FE 87 1A 08 13 F0 EE 2E 68 09 F4 EE 26 F0 EF .........h...&..
2200 2E 69 00 F4 EF 26 F0 F0 2E 68 09 F4 F0 26 F0 F1 .i...&...h...&..
2210 2E 69 00 F4 F1 26 F0 D5 87 1A 72 12 F0 E1 2E 68 .i...&....r....h
2220 03 F4 E1 26 F0 E2 2E 69 00 F4 E2 26 F0 E4 2E 68 ...&...i...&...h
2230 03 F4 E4 26 F0 E5 2E 69 00 F4 E5 26 F0 E3 2E F4 ...&...i...&....
2240 D2 26 1A 0A 12 F0 DA 2D FC 3E 5C 1E 54 3C 00 00 .&.....-.>\.T<..
2250 FE 14 16 F7 FF F4 F7 37 00 F0 F7 2E F0 09 62 EB .......7......b.
2260 4B 16 CF F0 F7 2D FD A7 FD A7 34 00 FA 70 FD 31 K....-....4..p.1
2270 FC 30 F0 0A 2E F9 60 F4 FE 26 F0 0B 2E F8 61 F4 .0....`..&....a.
2280 FF 26 F2 10 4D F0 F7 2B 32 00 F9 15 E5 29 30 00 .&..M..+2....)0.
2290 F9 35 F8 34 FA 70 F8 70 FD 31 FC 30 F2 0D 4A F8 .5.4.p.p.1.0..J.
22A0 70 E2 29 92 E2 28                               p.)..(         

;; fn22A6: 22A6
fn22A6 proc
	ld	a,(ix-0x09)
	ld	(ix-0x04),a
	ld	(ix-0x03),00
	ex	de,hl
	ld	hl,(sp+0x05)
	ex	de,hl
	inc	de
	ld	hl,(sp+0x10)
	add	hl,de
	ld	e,(hl)
	ld	d,00
	ld	l,e
	ld	h,d
	add	hl,hl
	add	hl,hl
	add	hl,hl
	add	hl,de
	add	hl,bc
	ld	(sp+0x03),hl
	ld	hl,(sp+0x03)
	inc	hl
	inc	hl
	ld	a,(hl)
	ld	(ix-0x08),a
	inc	hl
	ld	a,(hl)
	ld	(ix-0x07),a
	ex	de,hl
	ld	hl,(sp+0x05)
	ex	de,hl
	inc	de
	inc	de
	ld	hl,(sp+0x10)
	add	hl,de
	ld	c,(hl)
	ld	b,00
	ld	l,c
	ld	h,b
	add	hl,hl
	add	hl,bc
	ld	c,l
	ld	b,h
	ld	hl,(sp+0x0D)
	add	hl,bc
	ld	c,(hl)
	inc	hl
	ld	b,(hl)
	ex	de,hl
	ld	hl,(sp+0x05)
	ex	de,hl
	inc	de
	inc	de
	inc	de
	ld	hl,(sp+0x10)
	add	hl,de
	ld	e,(hl)
	ld	d,00
	ld	l,e
	ld	h,d
	add	hl,hl
	add	hl,hl
	add	hl,hl
	add	hl,de
	add	hl,bc
	ld	(sp+0x05),hl
	ld	hl,(sp+0x05)
	inc	hl
	inc	hl
	ld	c,(hl)
	inc	hl
	ld	b,(hl)
	ld	a,(ix-0x08)
	sub	a,c
	ld	c,a
	ld	a,(ix-0x07)
	sbc	a,b
	ld	b,a
	ld	hl,(sp+0x03)
	ld	e,(hl)
	inc	hl
	ld	d,(hl)
	ld	hl,(sp+0x05)
	ld	a,(hl)
	inc	hl
	ld	h,(hl)
	ld	l,a
	ld	a,e
	sub	a,l
	ld	e,a
	ld	a,d
	sbc	a,h
	ld	d,a
	push	bc
	push	de
	call	03AA
	add	sp,0004
	ld	c,l
	ld	b,h
	ld	hl,(sp+0x07)
	ld	(hl),c
	inc	hl
	ld	(hl),b
	inc	(ix-0x09)
	inc	(ix-0x09)
	inc	(ix-0x09)
	inc	(ix-0x09)
	jp	152A
237A                               FC 3E 5C 1E 54 3C           .>\.T<
2380 00 00 FE 14 16 F2 FF F4 F2 37 FE F0 04 29 30 00 .........7...)0.
2390 F9 35 F8 34 FA 70 F8 70 38 00 80 F8 70 F6 0C 42 .5.4.p.p8...p..B
23A0 F0 05 29 30 00 F9 35 F8 34 FA 70 FA 70 FA 70 F8 ..)0..5.4.p.p.p.
23B0 70 F6 0A 42 F4 F3 37 00 3A 3E 83 F0 F3 2E E2 62 p..B..7.:>.....b
23C0 EB 6F 18 CF FE 65 F0 F2 62 F4 F2 26 36 1E F0 F3 .o...e..b..&6...
23D0 60 F4 FA 26 36 80 69 00 F4 FB 26 F2 08 4A F0 04 `..&6.i...&..J..
23E0 2E E2 62 EB 66 18 CE F0 F3 29 30 00 F9 33 F8 32 ..b.f....)0..3.2
23F0 91 3A 1E 80 F9 70 F0 05 2E E2 62 EB 66 18 CE F0 .:...p....b.f...
2400 F2 2D 34 00 F8 70 FD 31 FC 30 3A 1E 80 F8 70 F6 .-4..p.1.0:...p.
2410 08 42 F2 08 4A E2 2B 32 00 FB 35 FA 34 FA 70 F9 .B..J.+2..5.4.p.
2420 70 39 00 80 F9 70 E2 2B 92 E2 2A 90 3A 1E 80 F8 p9...p.+..*.:...
2430 70 F6 06 42 F2 06 4A E2 29 30 00 F9 35 F8 34 FA p..B..J.)0..5.4.
2440 70 FA 70 FA 70 F8 70 F9 70 FD 31 FC 30 3A 04 00 p.p.p.p.p.1.0:..
2450 F8 70 E2 2E F4 F6 26 92 E2 2E F4 F7 26 F0 F3 2D .p....&.....&..-
2460 FD A7 FD A7 34 00 FA 70 39 AE 81 F9 70 F6 02 42 ....4..p9...p..B
2470 F0 F7 2E F0 F6 66 EB 7A 17 C6 F2 0C 4A E2 2B 92 .....f.z....J.+.
2480 E2 2A F2 0A 4A F9 70 39 04 00 F9 70 E2 2B 92 E2 .*..J.p9...p.+..
2490 2A F2 02 4A E2 2E 92 E2 2C 2D F9 70 08 F2 04 4A *..J....,-.p...J
24A0 08 FE 67 F9 73 EB 66 18 CF 3A 06 00 F8 70 F0 04 ..g.s.f..:...p..
24B0 2E EA 26 F2 08 4A E2 29 30 00 F9 35 F8 34 FA 70 ..&..J.)0..5.4.p
24C0 F8 70 39 00 80 F9 70 E2 29 92 E2 28 F2 06 4A E2 .p9...p.)..(..J.
24D0 2B 32 00 FB 35 FA 34 FA 70 FA 70 FA 70 F9 70 F8 +2..5.4.p.p.p.p.
24E0 70 38 07 00 F8 70 F0 05 2E EA 26 F2 08 4A E2 29 p8...p....&..J.)
24F0 30 00 F9 35 F8 34 FA 70 F8 70 39 00 80 F9 70 E2 0..5.4.p.p9...p.
2500 29 92 E2 28 F2 06 4A E2 2B 32 00 FB 35 FA 34 FA )..(..J.+2..5.4.
2510 70 FA 70 FA 70 F9 70 F8 70 38 08 00 F8 70 EA 37 p.p.p.p.p8...p.7
2520 01 F2 08 4A E2 29 30 00 F9 35 F8 34 FA 70 F8 70 ...J.)0..5.4.p.p
2530 39 00 80 F9 70 E2 29 92 E2 28 F2 06 4A E2 2B 32 9...p.)..(..J.+2
2540 00 FB 35 FA 34 FA 70 FA 70 FA 70 F9 70 F8 70 92 ..5.4.p.p.p.p.p.
2550 92 92 92 FD 31 FC 30 F2 0C 4A E2 2B 92 E2 2A F2 ....1.0..J.+..*.
2560 0A 4A F9 70 39 04 00 F9 70 E2 2B 92 E2 2A F2 02 .J.p9...p.+..*..
2570 4A E2 2E 92 E2 2C 2D F9 70 08 23 E8 26 90 22 E8 J....,-.p.#.&.".
2580 26 F2 06 4A E2 2E F2 08 4A E2 28 56 96 50 96 1C &..J....J.(V.P..
2590 4F 16 16 02 00 F0 F3 87 F0 F3 87 1A 89 16 FC 3E O..............>
25A0 5C 1E 54 3C 00 00 FE 14 16 FB FF 31 00 33 01 F0 \.T<.......1.3..
25B0 06 2E F4 FE 26 F0 07 2E F4 FF 26 F2 03 4A EA 37 ....&.....&..J.7
25C0 00 F4 FD 37 02 F2 03 4A 92 EA 37 01 21 FE 66 CE ...7...J..7.!.f.
25D0 06 FB 66 EB 22 19 C6 30 00 F9 35 F8 34 FA 70 F8 ..f."..0..5.4.p.
25E0 70 FD 31 FC 30 F2 09 4A F8 70 E2 29 92 E2 28 32 p.1.0..J.p.)..(2
25F0 00 FB 35 FA 34 FA 70 FA 70 FA 70 F9 70 F8 70 08 ..5.4.p.p.p.p.p.
2600 3A 08 00 F9 70 E2 2E FE 66 CE 04 2D 1A 25 19 3A :...p...f..-.%.:
2610 06 00 F9 70 E2 29 3A 07 00 F9 70 E2 28 F8 33 F0 ...p.):...p.(.3.
2620 FD 2D F0 FD 2A 82 F0 FE 2E FD 60 F4 FB 26 F0 FF .-..*.....`..&..
2630 2E 69 00 F4 FC 26 5A 52 EA 21 22 2D 86 F4 FD 26 .i...&ZR.!"-...&
2640 F0 FE 2E FD 60 2D F0 FF 2E 69 00 2C EA 20 1A 9D ....`-...i.,. ..
2650 18 F0 FD 2D FC 3E 5C 1E 54 3C 00 00 FE 14 16 A4 ...-.>\.T<......
2660 FF 3A 48 00 FE 70 F6 5A 42 3D 04 00 08 F2 5A 4A .:H..p.ZB=....ZJ
2670 08 F9 15 ED 37 01 F5 01 37 00 F0 FE 2E 68 08 29 ....7...7....h.)
2680 F0 FF 2E 69 00 28 FE 65 E8 26 F2 5A 4A EA 37 46 ...i.(.e.&.ZJ.7F
2690 92 EA 37 00 F2 5A 4A 92 92 EA 37 28 92 EA 37 00 ..7..ZJ...7(..7.
26A0 3D 09 00 08 F2 5A 4A 08 F9 15 F0 FE 2E 68 0D 2D =....ZJ......h.-
26B0 F0 FF 2E 69 00 2C FE 65 EA 26 92 EA 26 F0 FE 2E ...i.,.e.&..&...
26C0 68 11 29 F0 FF 2E 69 00 28 FE 65 E8 26 ED 37 00 h.)...i.(.e.&.7.
26D0 F5 01 37 00 3D 0B 00 08 F2 5A 4A 08 F9 15 ED 37 ..7.=....ZJ....7
26E0 00 F5 01 37 00 3A 24 00 FE 70 08 3A 04 00 F9 70 ...7.:$..p.:...p
26F0 EA 37 00 92 EA 37 00 3A 08 00 F9 70 EA 37 00 FB .7...7.:...p.7..
2700 35 FA 34 EA 37 0A 92 EA 37 00 FB 35 FA 34 92 92 5.4.7...7..5.4..
2710 EA 37 0A 92 EA 37 00 3D 09 00 F9 15 3A 0D 00 F9 .7...7.=....:...
2720 70 EA 37 00 92 EA 37 00 3A 11 00 F9 70 EA 37 00 p.7...7.:...p.7.
2730 ED 37 14 F5 01 37 00 3A 0B 00 F9 70 EA 37 0A 92 .7...7.:...p.7..
2740 EA 37 00 3D 12 00 F9 15 3A 16 00 F9 70 EA 37 00 .7.=....:...p.7.
2750 92 EA 37 00 3A 1A 00 F9 70 EA 37 00 ED 37 1E F5 ..7.:...p.7..7..
2760 01 37 00 3A 14 00 F9 70 EA 37 1E 92 EA 37 00 3D .7.:...p.7...7.=
2770 1B 00 F9 15 3A 1F 00 F9 70 EA 37 00 92 EA 37 00 ....:...p.7...7.
2780 3A 23 00 F9 70 EA 37 00 ED 37 0A F5 01 37 00 3A :#..p.7..7...7.:
2790 1D 00 F9 70 EA 37 1E 92 EA 37 00 3A 00 00 FE 70 ...p.7...7.:...p
27A0 FD 31 FC 30 3A 04 00 F8 70 EA 37 00 92 EA 37 00 .1.0:...p.7...7.
27B0 3A 08 00 F8 70 EA 37 00 F9 35 F8 34 EA 37 32 92 :...p.7..5.4.72.
27C0 EA 37 00 F9 35 F8 34 92 92 EA 37 0A 92 EA 37 00 .7..5.4...7...7.
27D0 3D 09 00 F8 15 3A 0D 00 F8 70 EA 37 00 92 EA 37 =....:...p.7...7
27E0 00 3A 11 00 F8 70 EA 37 00 ED 37 3C F5 01 37 00 .:...p.7..7<..7.
27F0 3A 0B 00 F8 70 EA 37 0A 92 EA 37 00 3D 12 00 F8 :...p.7...7.=...
2800 15 3A 16 00 F8 70 FE 65 EA 26 92 EA 26 3A 1A 00 .:...p.e.&..&:..
2810 F8 70 EA 37 00 ED 37 3C F5 01 37 00 3A 14 00 F8 .p.7..7<..7.:...
2820 70 EA 37 1E 92 EA 37 00 3D 1B 00 F8 15 3A 1F 00 p.7...7.=....:..
2830 F8 70 FE 65 EA 26 92 EA 26 3A 23 00 F8 70 EA 37 .p.e.&..&:#..p.7
2840 00 ED 37 28 F5 01 37 00 3A 1D 00 F8 70 EA 37 1E ..7(..7.:...p.7.
2850 92 EA 37 00 F2 5A 4A F6 5A 42 3A 00 80 F0 FE 2E ..7..ZJ.ZB:.....
2860 EA 26 92 F0 FF 2E EA 26 3A 02 80 EA 37 02 EB 03 .&.....&:...7...
2870 80 41 3A 05 80 EA 37 04 EB 06 80 40 3A 08 80 EA .A:...7....@:...
2880 37 04 3A 1E 80 52 36 03 56 96 3A 00 80 52 1C 9A 7.:..R6.V.:..R..
2890 0F 16 05 00 FD 30 50 3A AE 81 52 50 96 3A 1E 80 .....0P:..RP.:..
28A0 52 36 03 56 96 3A 00 80 52 1C 1D 15 16 08 00 58 R6.V.:..R......X
28B0 3A 3E 83 EA 20 3A 00 00 52 1C 4F 16 16 02 00 3A :>.. :..R.O....:
28C0 1E 80 52 3A 00 80 52 1C 73 18 16 04 00 34 00 FC ..R:..R.s....4..
28D0 3E 5C 1E F2 02 48 F2 04 49 FE 65 2D F8 66 30 10 >\...H..I.e-.f0.
28E0 CE 05 30 08 21 FA 70 F9 A2 A2 CF 02 F9 70 18 F5 ..0.!.p......p..
28F0 1E 3A 03 00 FE 70 E2 2B 9A E2 2D 1C F1 1B 1A 2F .:...p.+..-..../
2900 1C F2 02 4A F2 04 49 1C FB 1B 1A 2F 1C F2 02 4A ...J..I..../...J
2910 F2 04 49 1A FB 1B 3A 03 00 FE 70 E2 2B 9A E2 2D ..I...:...p.+..-
2920 25 A0 FE 63 2C 23 A0 FE 63 2A 24 FA 65 A2 24 56 %..c,#..c*$.e.$V
2930 A2 CF 0A FE 62 FD 62 2D FE 63 FC 62 2C FA AF C6 ....b.b-.c.b,...
2940 0A FE 62 FB 62 2B FE 63 FA 62 2A 1C 54 1C 5E FE ..b.b+.c.b*.T.^.
2950 DF 28 FE 62 FD 62 2D FE 63 FC 62 2C 20 1E A2 08 .(.b.b-.c.b, ...
2960 FE DF FE 62 FD 62 2D FE 63 FC 62 2C 1E F2 02 4A ...b.b-.c.b,...J
2970 F2 04 49 C8 0E 3A 03 00 FE 70 E2 2B 9A E2 2D 34 ..I..:...p.+..-4
2980 00 FC 32 23 6C 80 FA 66 CE 12 30 10 FA 71 A2 FB ..2#l..f..0..q..
2990 62 CF 02 FB 60 0E FA 71 18 F4 2B 1E 30 09 25 FC b...`..q..+.0.%.
29A0 35 34 00 FD A3 FA 71 F9 73 CF 02 F9 70 0E A2 18 54....q.s...p...
29B0 F4 F8 A2 F8 32 2B 08 1E 38 00 00 20 F9 66 C6 08 ....2+..8.. .f..
29C0 39 3F 83 3A 89 1C FE 59 1E 1A 00 01 00 00 00 00 9?.:...Y........
29D0 00 00 00 00 00 00 00 00 00 1A 3F 02 00 00 00 00 ..........?.....
29E0 00 00 00 00 00 00 00 00 00 1A 4F 02 00 00 00 00 ..........O.....
29F0 00 1F 00 00 00 00 00 00 00 1A 53 02 00 00 00 00 ..........S.....
2A00 00 1A 57 02 00 00 00 00 00 1F 00 00 00 00 00 00 ..W.............
2A10 00 1A 60 02 00 00 00 00 00 1A 64 02 00 00 00 00 ..`.......d.....
2A20 00 1A 68 02 00 00 00 00 00 1A 6C 02 00 00 00 00 ..h.......l.....
2A30 00 1F 00 00 00 00 00 00 00 1A 32 02 00 00 00 00 ..........2.....
2A40 00 1A 39 02 00 00 00 00 00 00 3E A0 FF EF C2 6E ..9.......>....n
2A50 04 37 C6 F7 37 C7 A5 37 C8 00 37 C9 03 37 CD 03 .7..7..7..7..7..
2A60 37 CE F0 37 CB A0 37 CF 00 37 E4 35 37 D0 08 37 7..7..7..7.57..7
2A70 D1 08 37 E5 01 37 DA 40 37 E9 01 EF E9 6E 08 EF ..7..7.@7....n..
2A80 E9 6E 20 37 EA 00 37 D2 C0 37 DB 20 EF DB 6E E0 .n 7..7..7. ..n.
2A90 B8 E6 B9 E6 3A 20 FF 36 00 1C A4 02 EA 26 92 7F ....: .6.....&..
2AA0 C0 FF CE F5 1C A4 02 B0 BF 1C 00 00 1C A4 02 1C ................
2AB0 89 1C 1C 29 19 01 50 E3 00 00 4D E7 ED 28 E3 00 ...)..P...M..(..
2AC0 00 2E 2F ED E5 2E EF ED 20 EB 00 00 26 58 1E 50 ../..... ...&X.P
2AD0 E3 00 00 4D E7 ED 28 E3 00 00 2E 2F ED E3 00 00 ...M..(..../....
2AE0 2E ED 26 EF ED 20 58 1E 27 EB 2F BE B9 BF 1F B0 ..&.. X.'./.....
2AF0 BF 37 C3 0F 1F 52 F2 04 4A 4F BC F2 02 4A 4F BA .7...R..JO...JO.
2B00 1C 00 00 5A 1F 1C 00 00 1F 1C 00 00 1F 97 B8 37 ...Z...........7
2B10 D3 4E 37 C2 10 1F 1C 00 00 1F 1C 00 00 1F 1C 00 .N7.............
2B20 00 1F 1C 00 00 1F F2 02 2E A8 BF C6 0D 02 A8 C3 ................
2B30 CE 08 03 00 00 1C A4 02 C8 EF 02 37 C3 0F 2F EB ...........7../.
2B40 B8 BF 03 1E F2 02 4A E2 2E FE 66 FE D6 1C 73 02 ......J...f...s.
2B50 92 C8 F4 37 D2 00 37 D3 B1 1E 37 D3 4E 1E 54 F2 ...7..7...7.N.T.
2B60 08 4A 52 F2 06 4A 52 1C A4 1B 16 04 00 52 F2 0C .JR..JR......R..
2B70 4A 52 F2 0A 4A 52 1C A4 1B 16 04 00 58 F8 70 5C JR..JR......X.p\
2B80 1E 54 F2 0A 4A 52 F2 06 4A 52 1C A4 1B 16 04 00 .T..JR..JR......
2B90 52 F2 0A 4A 52 F2 0A 4A 52 1C A4 1B 16 04 00 58 R..JR..JR......X
2BA0 21 FD 62 2D 20 FC 63 2C 5C 1E 54 3C 00 00 FE 14 !.b- .c,\.T<....
2BB0 F2 0A 4A F0 08 29 F0 09 28 08 F2 06 4A 08 F2 04 ..J..)..(...J...
2BC0 4D 52 51 50 55 1C A4 1B 16 04 00 FD 31 FC 30 59 MRQPU.......1.0Y
2BD0 5A 50 52 51 1C A4 1B 16 04 00 58 F8 70 FD 31 FC ZPRQ......X.p.1.
2BE0 30 F9 33 20 2A F9 66 CE 03 2D C8 16 FE 65 FB 67 0.3 *.f..-...e.g
2BF0 FA 63 EB 42 03 CC 6D 80 EB 4A 03 CD 35 01 C8 02 .c.B..m..J..5...
2C00 35 FF 5C 1E 54 3C 00 00 FE 14 F0 0A 29 F0 0B 28 5.\.T<......)..(
2C10 F2 08 4A 08 F2 06 4A 08 F2 04 4D 52 51 50 55 1C ..J...J...MRQPU.
2C20 A4 1B 16 04 00 FD 31 FC 30 59 5A 50 52 51 1C A4 ......1.0YZPRQ..
2C30 1B 16 04 00 58 21 FD 62 29 20 FC 63 28 F9 33 20 ....X!.b) .c(.3 
2C40 2A F9 66 CE 03 2D C8 16 FE 65 FB 67 FA 63 EB 9E *.f..-...e.g.c..
2C50 03 CC 6D 80 EB A6 03 CD 35 01 C8 02 35 FF 5C 1E ..m.....5...5.\.
2C60 54 F2 04 4A 52 F2 06 4A 52 1C A4 1B 16 04 00 52 T..JR..JR......R
2C70 F2 08 4A 52 F2 0A 4A 52 1C A4 1B 16 04 00 58 F8 ..JR..JR......X.
2C80 70 5C 1E 54 3C 00 00 FE 14 16 F6 FF 08 F2 12 4A p\.T<..........J
2C90 08 F0 06 2E F4 FE 26 F0 07 2E F4 FF 26 F2 08 4A ......&.....&..J
2CA0 92 92 E2 29 92 E2 28 F0 04 2E F4 FC 26 F0 05 2E ...)..(.....&...
2CB0 F4 FD 26 F0 FC 2E 68 02 F4 FA 26 F0 FD 2E 69 00 ..&...h...&...i.
2CC0 F4 FB 26 F2 04 4A E2 2E 92 E2 2C 2D 21 FD 62 29 ..&..J....,-!.b)
2CD0 20 FC 63 28 F9 35 F8 34 FA 70 F8 70 FA 70 F8 70  .c(.5.4.p.p.p.p
2CE0 FA 70 F8 70 FA 70 F8 70 FA 70 F8 70 FA 70 F8 70 .p.p.p.p.p.p.p.p
2CF0 FA 70 F8 70 FA 70 F8 70 FA 70 FA 70 FA 70 F8 70 .p.p.p.p.p.p.p.p
2D00 FA 70 F8 70 FA 70 F8 70 FA 70 FA 70 F6 02 42 FB .p.p.p.p.p.p..B.
2D10 35 FA 34 F0 F8 2E EA 26 92 F0 F9 2E EA 26 3A 02 5.4....&.....&:.
2D20 00 F9 70 E6 52 F2 08 4A E2 29 92 E2 28 F2 06 4A ..p.R..J.)..(..J
2D30 E2 2E 92 E2 2C 2D 21 FD 62 29 20 FC 63 28 F9 35 ....,-!.b) .c(.5
2D40 F8 34 FA 70 F8 70 FA 70 FA 70 FA 70 F8 70 FA 70 .4.p.p.p.p.p.p.p
2D50 FA 70 FD 31 FC 30 5A 52 EA 21 92 EA 20 3A 04 00 .p.1.0ZR.!.. :..
2D60 F9 70 E6 52 F2 06 4A E2 2B 92 E2 2A 50 51 F2 06 .p.R..J.+..*PQ..
2D70 4A 52 1C A4 1B 16 04 00 08 58 F2 04 4A E2 2E 92 JR.......X..J...
2D80 E2 2C 2D 51 52 50 1C A4 1B 16 04 00 59 F9 70 FE .,-QRP......Y.p.
2D90 65 FD 62 29 36 00 FC 63 28 5A 52 EA 21 92 EA 20 e.b)6..c(ZR.!.. 
2DA0 FC 3E 5C 1E 54 3C 00 00 FE 14 16 E8 FF F0 04 29 .>\.T<.........)
2DB0 F0 05 28 E0 2E F4 EA 26 90 E0 2E F4 EB 26 98 3A ..(....&.....&.:
2DC0 02 00 F8 70 E6 52 F0 EB 2E F0 EA 66 CE 0E 5A 52 ...p.R.....f..ZR
2DD0 E2 2B 92 E2 2A 22 FB 66 EB 53 05 C6 08 F2 1E 4A .+..*".f.S.....J
2DE0 08 E1 2E F4 F0 26 91 E1 2E F4 F1 26 99 3A 02 00 .....&.....&.:..
2DF0 F9 70 F6 10 42 F0 F1 2E F0 F0 66 CE 11 F2 10 4A .p..B.....f....J
2E00 E2 2E 92 E2 2C FC 66 CE 05 35 00 1A 01 08 3A 04 ....,.f..5....:.
2E10 00 F9 70 F6 0E 42 3A 04 00 F8 70 F6 0C 42 F0 08 ..p..B:...p..B..
2E20 2E F4 F2 26 F0 09 2E F4 F3 26 5A 52 E2 2E F4 EE ...&.....&ZR....
2E30 26 92 E2 2E F4 EF 26 F2 0C 4A E2 2E F4 EC 26 92 &.....&..J....&.
2E40 E2 2E F4 ED 26 F0 F2 2E 68 02 F4 FA 26 F0 F3 2E ....&...h...&...
2E50 69 00 F4 FB 26 FE 65 F0 EC 62 F4 FE 26 36 00 F0 i...&.e..b..&6..
2E60 ED 63 F4 FF 26 F0 EB 2E F0 EA 66 EB 6B 06 CE F0 .c..&.....f.k...
2E70 F1 2E F0 F0 66 EB 04 06 CE F2 0E 4A E2 29 92 E2 ....f......J.)..
2E80 28 50 F2 08 4A 52 1C A4 1B 16 04 00 FD 31 FC 30 (P..JR.......1.0
2E90 F2 10 4A E2 2B 92 E2 2A 50 F2 06 4A 52 51 1C A4 ..J.+..*P..JRQ..
2EA0 1B 16 04 00 08 58 23 F9 62 CE 0A 22 F8 62 CE 05 .....X#.b..".b..
2EB0 35 02 1A 01 08 35 00 1A 01 08 51 F2 08 4A 52 F2 5....5....Q..JR.
2EC0 1A 4A 52 1C DE 1B 16 04 00 FD 31 FC 30 59 F2 12 .JR.......1.0Y..
2ED0 4A EA 21 92 EA 20 F2 10 4A E2 2E 92 E2 2C 2D 51 J.!.. ..J....,-Q
2EE0 50 52 1C A4 1B 16 04 00 FD 31 FC 30 59 F2 0E 4A PR.......1.0Y..J
2EF0 E2 2E 92 E2 2C 2D F8 70 FE 65 FD 62 29 36 00 FC ....,-.p.e.b)6..
2F00 63 28 08 E2 2B 92 E2 2A 51 50 1C DE 1B 16 04 00 c(..+..*QP......
2F10 FD 31 FC 30 F2 0A 4A EA 21 92 EA 20 35 01 1A 01 .1.0..J.!.. 5...
2F20 08 F2 10 4A E2 2E F4 FC 26 92 E2 2E F4 FD 26 50 ...J....&.....&P
2F30 51 F2 08 4A 52 F2 0E 4A 52 1C A4 1B 16 04 00 F4 Q..JR..JR.......
2F40 ED 24 F4 EC 25 59 58 F0 EF 2E F0 EE 66 EB 38 07 .$..%YX.....f.8.
2F50 CE F0 FD 2E F0 FC 66 EB D4 06 CE F2 0E 4A E2 29 ......f......J.)
2F60 92 E2 28 50 F2 04 4A 52 1C A4 1B 16 04 00 FD 31 ..(P..JR.......1
2F70 FC 30 F0 EC 2E F9 62 CE 0C F0 ED 2E F8 62 CE 05 .0....b......b..
2F80 35 02 1A 01 08 35 00 1A 01 08 51 F2 04 4A 52 F2 5....5....Q..JR.
2F90 1A 4A 52 1C DE 1B 16 04 00 FD 31 FC 30 59 F2 0A .JR.......1.0Y..
2FA0 4A EA 21 92 EA 20 08 E2 2B 92 E2 2A 50 51 1C A4 J.!.. ..+..*PQ..
2FB0 1B 16 04 00 FD 31 FC 30 F2 0E 4A E2 2E 92 E2 2C .....1.0..J....,
2FC0 2D F8 70 FE 65 FD 62 29 36 00 FC 63 28 F2 10 4A -.p.e.b)6..c(..J
2FD0 E2 2B 92 E2 2A 51 50 1C DE 1B 16 04 00 FD 31 FC .+..*QP.......1.
2FE0 30 F2 12 4A EA 21 92 EA 20 35 01 1A 01 08 50 F2 0..J.!.. 5....P.
2FF0 08 4A 52 F2 0C 4A 52 1C A4 1B 16 04 00 52 F2 18 .JR..JR......R..
3000 4A 52 F2 08 4A 52 1C A4 1B 16 04 00 59 58 23 FD JR..JR......YX#.
3010 62 F4 FC 26 22 FC 63 F4 FD 26 F2 0E 4A E2 2B 92 b..&".c..&..J.+.
3020 E2 2A 50 51 F2 06 4A 52 1C A4 1B 16 04 00 08 58 .*PQ..JR.......X
3030 F0 FD 2E F0 FC 66 CE 18 F0 EC 2E FB 62 CE 0C F0 .....f......b...
3040 ED 2E FA 62 CE 05 35 02 1A 01 08 35 00 1A 01 08 ...b..5....5....
3050 23 F0 EC 62 2B 22 F0 ED 63 2A 50 F2 16 4A 52 51 #..b+"..c*P..JRQ
3060 1C DE 1B 16 04 00 08 58 F2 12 4A EA 23 92 EA 22 .......X..J.#.."
3070 5A 52 E2 2E 92 E2 2C 2D 50 51 52 1C A4 1B 16 04 ZR....,-PQR.....
3080 00 08 58 F2 0C 4A E2 2E 92 E2 2C 2D F9 70 FE 65 ..X..J....,-.p.e
3090 FD 62 2B 36 00 FC 63 2A F9 35 F8 34 E2 29 92 E2 .b+6..c*.5.4.)..
30A0 28 50 51 1C DE 1B 16 04 00 FD 31 FC 30 F2 0A 4A (PQ.......1.0..J
30B0 EA 21 92 EA 20 35 01 FC 3E 5C 1E 54 3C 00 00 FE .!.. 5..>\.T<...
30C0 14 16 D9 FF 3A 08 00 FE 70 FD 31 FC 30 F9 33 F8 ....:...p.1.0.3.
30D0 32 50 51 F2 31 4A 52 F2 31 4A 52 1C CD 03 16 06 2PQ.1JR.1JR.....
30E0 00 58 3A 02 00 FE 70 FD 33 FC 32 50 51 52 F2 37 .X:...p.3.2PQR.7
30F0 4A 52 F2 37 4A 52 1C CD 03 16 06 00 59 58 F2 33 JR.7JR......YX.3
3100 4A 52 51 50 1C EE 04 16 06 00 F4 DA 25 F4 E9 25 JRQP........%..%
3110 F0 DA 2E FE 66 CE 04 2D 1A 35 0C F0 08 2E F4 F6 ....f..-.5......
3120 26 F0 09 2E F4 F7 26 F0 06 2E F4 E7 26 F0 07 2E &.....&.....&...
3130 F4 E8 26 F0 04 2E F4 FE 26 F0 05 2E F4 FF 26 F2 ..&.....&.....&.
3140 25 4A E2 2E F4 FC 26 92 E2 2E F4 FD 26 F0 0A 2E %J....&.....&...
3150 F4 FA 26 F0 0B 2E F4 FB 26 F2 1D 4A E2 2E F4 F8 ..&.....&..J....
3160 26 92 E2 2E F4 F9 26 F0 FE 2E 68 02 F4 F4 26 F0 &.....&...h...&.
3170 FF 2E 69 00 F4 F5 26 F0 F6 2E 68 02 F4 F2 26 F0 ..i...&...h...&.
3180 F7 2E 69 00 F4 F3 26 F0 FA 2E 68 02 F4 F0 26 F0 ..i...&...h...&.
3190 FB 2E 69 00 F4 F1 26 F0 E7 2E 68 02 F4 EE 26 F0 ..i...&...h...&.
31A0 E8 2E 69 00 F4 EF 26 F0 E9 2E 6A 02 EB DD 09 CE ..i...&...j.....
31B0 F2 19 4A E2 29 92 E2 28 F2 15 4A E2 2B 92 E2 2A ..J.)..(..J.+..*
31C0 21 FB 62 2B 20 FA 63 2A F2 0E 4A E2 2E 92 E2 2C !.b+ .c*..J....,
31D0 2D F0 F8 2E FD 62 F4 EC 26 F0 F9 2E FC 63 F4 ED -....b..&....c..
31E0 26 F2 1B 4A E2 2E 92 E2 2C 2D 21 FD 62 29 20 FC &..J....,-!.b) .
31F0 63 28 F0 F8 2E F0 FC 62 2D F0 F9 2E F0 FD 63 2C c(.....b-.....c,
3200 51 08 F2 15 4A 08 51 50 52 1C F4 02 16 08 00 25 Q...J.QPR......%
3210 FE 66 CE 05 35 03 1A 35 0C F2 17 4A E2 29 92 E2 .f..5..5...J.)..
3220 28 F2 15 4A E2 2B 92 E2 2A 21 FB 62 F4 EC 26 20 (..J.+..*!.b..& 
3230 FA 63 F4 ED 26 F2 21 4A E2 2B 92 E2 2A F2 0E 4A .c..&.!J.+..*..J
3240 E2 2E 92 E2 2C 2D 23 FD 62 F4 EA 26 22 FC 63 F4 ....,-#.b..&".c.
3250 EB 26 F2 1B 4A E2 2E 92 E2 2C 2D 21 FD 62 29 20 .&..J....,-!.b) 
3260 FC 63 28 F2 25 4A E2 2E 92 E2 2C 2D 23 FD 62 2B .c(.%J....,-#.b+
3270 22 FC 63 2A F2 13 4A 52 F2 13 4A 52 50 51 1C F4 ".c*..JR..JRPQ..
3280 02 16 08 00 25 FE 66 CE 05 35 03 1A 35 0C 35 00 ....%.f..5..5.5.
3290 1A 35 0C F0 0C 2E F4 EA 26 F0 0D 2E F4 EB 26 F0 .5......&.....&.
32A0 EA 2E 68 02 F4 EC 26 F0 EB 2E 69 00 F4 ED 26 F0 ..h...&...i...&.
32B0 FC 2E F0 F8 62 EB 4A 0A CE F0 FD 2E F0 F9 62 EB ....b.J.......b.
32C0 4A 0A CE F2 1B 4A E2 29 92 E2 28 F2 19 4A E2 2B J....J.)..(..J.+
32D0 92 E2 2A 21 FB 62 CE 28 20 FA 62 CE 23 F2 11 4A ..*!.b.( .b.#..J
32E0 F0 FC 2E EA 26 92 F0 FD 2E EA 26 F2 1B 4A E2 29 ....&.....&..J.)
32F0 92 E2 28 F2 13 4A EA 21 92 EA 20 35 02 1A 35 0C ..(..J.!.. 5..5.
3300 F2 21 4A E2 2E F4 FE 26 92 E2 2E F4 FF 26 F0 FC .!J....&.....&..
3310 2E F0 FE 62 EB A9 0A CE F0 FD 2E F0 FF 62 EB A9 ...b.........b..
3320 0A CE F2 1B 4A E2 29 92 E2 28 F2 17 4A E2 2B 92 ....J.)..(..J.+.
3330 E2 2A 21 FB 62 CE 28 20 FA 62 CE 23 F2 11 4A F0 .*!.b.( .b.#..J.
3340 FC 2E EA 26 92 F0 FD 2E EA 26 F2 1B 4A E2 29 92 ...&.....&..J.).
3350 E2 28 F2 13 4A EA 21 92 EA 20 35 02 1A 35 0C F2 .(..J.!.. 5..5..
3360 0E 4A E2 2E F4 E7 26 92 E2 2E F4 E8 26 F2 15 4A .J....&.....&..J
3370 E2 29 92 E2 28 F0 E7 2E F0 F8 62 EB 08 0B CE F0 .)..(.....b.....
3380 E8 2E F0 F9 62 EB 08 0B CE F2 19 4A E2 2B 92 E2 ....b......J.+..
3390 2A 21 FB 62 CE 28 20 FA 62 CE 23 F2 11 4A F0 E7 *!.b.( .b.#..J..
33A0 2E EA 26 92 F0 E8 2E EA 26 F2 15 4A E2 29 92 E2 ..&.....&..J.)..
33B0 28 F2 13 4A EA 21 92 EA 20 35 02 1A 35 0C F0 E7 (..J.!.. 5..5...
33C0 2E F0 FE 62 EB 51 0B CE F0 E8 2E F0 FF 62 EB 51 ...b.Q.......b.Q
33D0 0B CE F2 17 4A E2 2B 92 E2 2A 21 FB 62 CE 28 20 ....J.+..*!.b.( 
33E0 FA 62 CE 23 F2 11 4A F0 E7 2E EA 26 92 F0 E8 2E .b.#..J....&....
33F0 EA 26 F2 15 4A E2 29 92 E2 28 F2 13 4A EA 21 92 .&..J.)..(..J.!.
3400 EA 20 35 02 1A 35 0C F2 13 4A E2 2B 92 E2 2A 23 . 5..5...J.+..*#
3410 F9 62 F4 EE 26 22 F8 63 F4 EF 26 F2 11 4A E2 29 .b..&".c..&..J.)
3420 92 E2 28 21 F0 E7 62 F4 F8 26 20 F0 E8 63 F4 F9 ..(!..b..& ..c..
3430 26 F2 1B 4A E2 2E 92 E2 2C 2D 23 FD 62 2B 22 FC &..J....,-#.b+".
3440 63 2A 21 F0 FC 62 29 20 F0 FD 63 28 F2 15 4A 52 c*!..b) ..c(..JR
3450 F2 21 4A 52 51 50 1C F4 02 16 08 00 F4 D9 25 F2 .!JRQP........%.
3460 13 4A E2 29 92 E2 28 F2 17 4A E2 2B 92 E2 2A 21 .J.)..(..J.+..*!
3470 FB 62 F4 EC 26 20 FA 63 F4 ED 26 F2 11 4A E2 2B .b..& .c..&..J.+
3480 92 E2 2A F2 21 4A E2 2E 92 E2 2C 2D 23 FD 62 F4 ..*.!J....,-#.b.
3490 EA 26 22 FC 63 F4 EB 26 F2 19 4A E2 2E 92 E2 2C .&".c..&..J....,
34A0 2D 21 FD 62 29 20 FC 63 28 F2 1D 4A E2 2E 92 E2 -!.b) .c(..J....
34B0 2C 2D 23 FD 62 2B 22 FC 63 2A F2 13 4A 52 F2 13 ,-#.b+".c*..JR..
34C0 4A 52 50 51 1C F4 02 16 08 00 F0 D9 2E FE 66 CE JRPQ..........f.
34D0 04 FD 66 C6 04 35 00 C8 12 F0 D9 2E FE 66 C6 05 ..f..5.......f..
34E0 25 FE 66 CE 04 35 02 C8 02 35 01 FC 3E 5C 1E 54 %.f..5...5..>\.T
34F0 3C 00 00 FE 14 16 EB FF F4 EB 37 01 F0 04 2E F4 <.........7.....
3500 FE 26 F0 05 2E F4 FF 26 F0 06 2E F4 FC 26 F0 07 .&.....&.....&..
3510 2E F4 FD 26 F0 FC 2E 68 02 F4 FA 26 F0 FD 2E 69 ...&...h...&...i
3520 00 F4 FB 26 F4 ED 37 00 3A 00 00 F6 0D 42 F2 0F ...&..7.:....B..
3530 4A E2 29 F0 FE 2E 68 02 F4 F6 26 F0 FF 2E 69 00 J.)...h...&...i.
3540 F4 F7 26 F0 ED 2E F9 62 EB EE 0C CF F2 13 4A E2 ..&....b......J.
3550 29 92 E2 28 F2 11 4A E2 2B 92 E2 2A F2 0D 4D F9 )..(..J.+..*..M.
3560 15 E5 2B F1 01 2A 21 FB 62 CE 23 20 FA 62 CE 1E ..+..*!.b.# .b..
3570 F2 0B 4A E2 29 92 E2 28 95 95 E5 2B F1 01 2A 21 ..J.)..(...+..*!
3580 FB 62 CE 0A 20 FA 62 CE 05 35 02 1A F8 0D F0 F8 .b.. .b..5......
3590 2E 68 09 F4 F8 26 F0 F9 2E 69 00 F4 F9 26 F0 ED .h...&...i...&..
35A0 87 1A 78 0C F4 ED 37 00 3A 00 00 F6 0D 42 F2 0F ..x...7.:....B..
35B0 4A E2 2B F0 ED 2E FB 62 EB F5 0D CF F0 ED 29 30 J.+....b......)0
35C0 00 90 32 00 51 50 1C D2 1B 16 04 00 F4 EC 25 F2 ..2.QP........%.
35D0 11 4A E2 2B 92 E2 2A F0 F8 2E FB 60 F4 F4 26 F0 .J.+..*....`..&.
35E0 F9 2E FA 61 F4 F5 26 F2 09 4A 92 92 E2 29 92 E2 ...a..&..J...)..
35F0 28 F2 0B 4A E2 2E F4 F2 26 92 E2 2E F4 F3 26 21 (..J....&.....&!
3600 F0 F2 62 F4 F0 26 20 F0 F3 63 F4 F1 26 F2 09 4A ..b..& ..c..&..J
3610 E2 2E F4 F4 26 92 E2 2E F4 F5 26 F2 13 4A E2 29 ....&.....&..J.)
3620 92 E2 28 F0 F4 2E F9 62 F4 F4 26 F0 F5 2E F8 63 ..(....b..&....c
3630 F4 F5 26 51 F0 EC 2B 32 00 FB 35 FA 34 FA 70 FA ..&Q..+2..5.4.p.
3640 70 FA 70 F9 70 59 F9 70 F6 03 42 F2 03 4A 92 92 p.p.pY.p..B..J..
3650 E2 2B 92 E2 2A 23 F0 F2 62 2B 22 F0 F3 63 2A F2 .+..*#..b+"..c*.
3660 03 4A E2 2E 92 E2 2C F9 62 29 24 F8 63 28 F2 05 .J....,.b)$.c(..
3670 4A 52 F2 0B 4A 52 51 50 1C 4E 03 16 08 00 F4 EE JR..JRQP.N......
3680 25 25 FE 66 C6 04 35 00 C8 24 F0 EE 2E FE 66 CE %%.f..5..$....f.
3690 04 F4 EB 37 02 F0 F8 2E 68 09 F4 F8 26 F0 F9 2E ...7....h...&...
36A0 69 00 F4 F9 26 F0 ED 87 1A F8 0C F0 EB 2D FC 3E i...&........-.>
36B0 5C 1E 54 3C 00 00 FE 14 16 E9 FF F4 FF 37 00 3A \.T<.........7.:
36C0 03 00 FE 70 F6 14 42 F0 08 2E F4 FB 26 F0 09 2E ...p..B.....&...
36D0 F4 FC 26 F0 FB 2E 68 02 F4 F9 26 F0 FC 2E 69 00 ..&...h...&...i.
36E0 F4 FA 26 F4 E9 37 00 F4 F5 37 00 3A 00 00 F6 0E ..&..7...7.:....
36F0 42 F2 10 4A E2 2E F4 F6 26 F0 F5 2E F0 F6 62 EB B..J....&.....b.
3700 E0 0E CF 08 F2 14 4A 08 F2 12 4A E2 29 92 E2 28 ......J...J.)..(
3710 F0 F5 2D 34 00 92 4D F0 F6 2D 34 00 50 51 52 55 ..-4..M..-4.PQRU
3720 1C D2 1B 16 04 00 59 58 51 FD 33 FC 32 FA 70 FA ......YXQ.3.2.p.
3730 70 FA 70 F9 70 59 F8 70 F2 0E 4D F8 15 51 52 55 p.p.pY.p..M..QRU
3740 F2 23 4A 52 F2 23 4A 52 1C 05 08 16 0A 00 FD 33 .#JR.#JR.......3
3750 36 03 FB 62 C7 2A 32 00 3A AB 0E F9 70 F9 70 EA 6..b.*2.:...p.p.
3760 C8 C8 1D C8 06 C8 09 C8 12 C8 15 35 01 1A 96 0F ...........5....
3770 F0 E9 87 F0 E9 2E F4 FF 26 C8 05 35 02 1A 96 0F ........&..5....
3780 F0 F7 2E 68 09 F4 F7 26 F0 F8 2E 69 00 F4 F8 26 ...h...&...i...&
3790 F0 F5 87 1A 3B 0E F0 FF 2E 6A 03 C6 07 F0 FF 2E ....;....j......
37A0 6A 04 CE 05 35 01 1A 96 0F F2 12 4A 52 F2 1D 4A j...5......JR..J
37B0 52 1C 39 0C 16 04 00 F4 EB 25 F2 1F 4A 52 F2 1F R.9......%..JR..
37C0 4A 52 1C 39 0C 16 04 00 F4 EA 25 F0 EB 2E 8E CE JR.9......%.....
37D0 04 36 01 C8 02 FE 65 F4 F6 26 F0 EA 2E 8E CE 04 .6....e..&......
37E0 36 01 C8 02 FE 65 29 F0 FF 2E FE 66 CE 15 F0 F6 6....e)....f....
37F0 2E FE 66 CE 04 F9 66 C6 05 35 01 1A 96 0F 35 00 ..f...f..5....5.
3800 1A 96 0F F0 FF 2E 8E CE 15 F0 F6 2E FE 66 CE 04 .............f..
3810 F9 66 C6 05 35 01 1A 96 0F 35 03 1A 96 0F F0 FF .f..5....5......
3820 2E 6A 02 CE 25 F0 F6 2E FE 66 CE 04 F9 66 C6 04 .j..%....f...f..
3830 35 01 C8 18 F0 EB 2E FE 66 C6 07 F0 EA 2E FE 66 5.......f......f
3840 CE 04 35 03 C8 06 35 01 C8 02 35 01 FC 3E 5C 1E ..5...5...5..>\.
3850 54 3C 00 00 FE 14 16 D2 FF F4 DA 37 00 F4 D2 37 T<.........7...7
3860 00 3A 00 00 F6 1E 42 35 00 F6 1C 42 F0 D2 2E F0 .:....B5...B....
3870 06 62 EB FB 11 CF F0 04 2E F0 F0 60 F4 EC 26 F0 .b.........`..&.
3880 05 2E F0 F1 61 F4 ED 26 F0 04 2E F0 EE 60 F4 EA ....a..&.....`..
3890 26 F0 05 2E F0 EF 61 F4 EB 26 F0 EA 2E 68 02 F4 &.....a..&...h..
38A0 E8 26 F0 EB 2E 69 00 F4 E9 26 F4 D4 37 00 3A 00 .&...i...&..7.:.
38B0 00 F6 14 42 35 00 F6 12 42 F2 16 4A E2 2E F4 E3 ...B5...B..J....
38C0 26 F0 D4 2E F0 E3 62 EB D5 11 CF F2 18 4A E2 2E &.....b......J..
38D0 F4 E1 26 92 E2 2E F4 E2 26 F0 E1 2E F0 E4 60 F4 ..&.....&.....`.
38E0 DF 26 F0 E2 2E F0 E5 61 F4 E0 26 F2 0D 4A E2 2E .&.....a..&..J..
38F0 F4 DD 26 92 E2 2E F4 DE 26 F0 DE AF EB AF 11 CE ..&.....&.......
3900 36 64 F0 DD 67 36 00 F0 DE 63 EB 5A 10 CC 6D 80 6d..g6...c.Z..m.
3910 EB AF 11 C5 F2 0D 4A 92 92 E2 29 92 E2 28 F8 AF ......J...)..(..
3920 EB AF 11 CE 36 64 F9 67 36 00 F8 63 EB 7C 10 CC ....6d.g6..c.|..
3930 6D 80 EB AF 11 C5 F4 DD 37 01 F0 D4 29 30 00 90 m.......7...)0..
3940 F0 E3 2B 32 00 51 50 1C D2 1B 16 04 00 F4 D6 25 ..+2.QP........%
3950 FD 31 30 00 F9 35 F8 34 FA 70 FA 70 FA 70 F8 70 .10..5.4.p.p.p.p
3960 F6 0D 42 F2 0F 4D 08 F2 0D 4A 08 F9 15 E5 4A FC ..B..M...J....J.
3970 AF EB AF 11 CE 36 64 FD 67 36 00 FC 63 EB CD 10 .....6d.g6..c...
3980 CC 6D 80 EB AF 11 C5 F1 02 4A FC AF EB AF 11 CE .m.......J......
3990 36 64 FD 67 36 00 FC 63 EB E8 10 CC 6D 80 EB AF 6d.g6..c....m...
39A0 11 C5 F4 D3 37 01 3A 03 00 F6 0F 42 F0 D3 2E F0 ....7.:....B....
39B0 06 62 EB 55 11 CF F0 D3 2E F0 D2 62 EB 3F 11 C6 .b.U.......b.?..
39C0 F2 32 4D 08 F2 0F 4A 08 F9 15 F2 1A 4A E2 29 92 .2M...J.....J.).
39D0 E2 28 F2 0D 4A F8 70 F0 E6 2E F9 60 29 F0 E7 2E .(..J.p....`)...
39E0 F8 61 28 55 52 50 1C FC 0D 16 06 00 8D CE 06 F4 .a(URP..........
39F0 DD 37 00 C8 16 F0 E1 2E 68 03 F4 E1 26 F0 E2 2E .7......h...&...
3A00 69 00 F4 E2 26 F0 D3 87 1A F6 10 F0 DD 2E FE 66 i...&..........f
3A10 EB AF 11 C6 F0 DA 2B F0 DA 28 80 F2 35 4A 32 00 ......+..(..5J2.
3A20 F9 70 F0 D2 2E EA 26 20 29 86 F4 DD 26 F2 35 4A .p....& )...&.5J
3A30 30 00 F8 70 F0 D4 2E EA 26 F0 DD 29 F0 DD 2E 86 0..p....&..)....
3A40 F4 DF 26 F2 35 4A 30 00 F8 70 F0 D2 2E EA 26 F0 ..&.5J0..p....&.
3A50 DF 29 F0 DF 2E 86 F4 DA 26 F2 35 4A 30 00 F8 70 .)......&.5J0..p
3A60 F0 D6 2E EA 26 F0 E6 2E 68 09 F4 E6 26 F0 E7 2E ....&...h...&...
3A70 69 00 F4 E7 26 F0 E4 2E 68 09 F4 E4 26 F0 E5 2E i...&...h...&...
3A80 69 00 F4 E5 26 F0 D4 87 1A 03 10 F0 F0 2E 68 03 i...&.........h.
3A90 F4 F0 26 F0 F1 2E 69 00 F4 F1 26 F0 EE 2E 68 03 ..&...i...&...h.
3AA0 F4 EE 26 F0 EF 2E 69 00 F4 EF 26 F0 D2 87 1A B6 ..&...i...&.....
3AB0 0F F4 D2 37 00 3A 00 00 F6 0F 42 35 00 F6 12 42 ...7.:....B5...B
3AC0 F0 06 29 30 00 98 F0 D2 2B 32 00 23 F9 62 22 F8 ..)0....+2.#.b".
3AD0 63 EB 21 12 CC 6D 80 EB 16 15 CD F0 D2 2E 86 F4 c.!..m..........
3AE0 E3 26 F4 E6 26 F0 04 2E F0 E1 60 F4 E8 26 F0 05 .&..&.....`..&..
3AF0 2E F0 E2 61 F4 E9 26 F0 04 2E F0 E4 60 F4 EA 26 ...a..&.....`..&
3B00 F0 05 2E F0 E5 61 F4 EB 26 F0 EA 2E 68 02 F4 EC .....a..&...h...
3B10 26 F0 EB 2E 69 00 F4 ED 26 F4 D5 37 00 3A 00 00 &...i...&..7.:..
3B20 F6 1C 42 35 00 F6 1E 42 F2 1A 4A F0 D5 2E E2 62 ..B5...B..J....b
3B30 EB ED 14 CF F2 18 4A E2 2E F4 DB 26 92 E2 2E F4 ......J....&....
3B40 DC 26 F0 DB 2E F0 F0 60 F4 DB 26 F0 DC 2E F0 F1 .&.....`..&.....
3B50 61 F4 DC 26 F2 09 4A E2 2E F4 FE 26 92 E2 2E F4 a..&..J....&....
3B60 FF 26 F0 FF AF EB C7 14 CE 36 64 F0 FE 67 36 00 .&.......6d..g6.
3B70 F0 FF 63 EB C3 12 CC 6D 80 EB C7 14 C5 F2 09 4A ..c....m.......J
3B80 92 92 E2 29 92 E2 28 F8 AF EB C7 14 CE 36 64 F9 ...)..(......6d.
3B90 67 36 00 F8 63 EB E5 12 CC 6D 80 EB C7 14 C5 F0 g6..c....m......
3BA0 E6 29 F4 FE 21 30 00 F9 35 F8 34 FA 70 F8 70 F6 .)..!0..5.4.p.p.
3BB0 09 42 F0 DB 2E F4 FC 26 F0 DC 2E F4 FD 26 F0 FE .B.....&.....&..
3BC0 2E F0 06 62 EB C7 14 CF F0 04 2E F0 DB 60 F4 FA ...b.........`..
3BD0 26 F0 05 2E F0 DC 61 F4 FB 26 F0 04 2E F0 FC 60 &.....a..&.....`
3BE0 F4 F8 26 F0 05 2E F0 FD 61 F4 F9 26 F0 F8 2E 68 ..&.....a..&...h
3BF0 02 F4 F6 26 F0 F9 2E 69 00 F4 F7 26 F4 D7 37 00 ...&...i...&..7.
3C00 3A 00 00 F6 22 42 35 00 F6 20 42 F2 24 4A F0 D7 :..."B5.. B.$J..
3C10 2E E2 62 EB A1 14 CF F2 26 4A E2 29 92 E2 28 21 ..b.....&J.)..(!
3C20 F0 F2 60 F4 D8 26 20 F0 F3 61 F4 D9 26 F2 06 4A ..`..& ..a..&..J
3C30 E2 29 92 E2 28 F8 AF EB 7B 14 CE 36 64 F9 67 36 .)..(...{..6d.g6
3C40 00 F8 63 EB 93 13 CC 6D 80 EB 7B 14 C5 F2 06 4A ..c....m..{....J
3C50 92 92 E2 29 92 E2 28 F8 AF EB 7B 14 CE 36 64 F9 ...)..(...{..6d.
3C60 67 36 00 F8 63 EB B5 13 CC 6D 80 EB 7B 14 C5 38 g6..c....m..{..8
3C70 01 01 3A 03 00 F6 06 42 20 F0 06 62 EB 23 14 CF ..:....B ..b.#..
3C80 F2 32 4D 08 F2 06 4A 08 F9 15 F2 28 4A E2 2B 92 .2M...J....(J.+.
3C90 E2 2A F0 F4 2E FB 60 2B F0 F5 2E FA 61 2A F2 16 .*....`+....a*..
3CA0 4A E2 2E 92 E2 2C 2D F0 EE 2E FD 60 2D F0 EF 2E J....,-....`-...
3CB0 FC 61 2C 50 55 51 52 1C FC 0D 16 06 00 58 8D CE .a,PUQR......X..
3CC0 04 31 00 C8 14 F0 D8 2E 68 03 F4 D8 26 F0 D9 2E .1......h...&...
3CD0 69 00 F4 D9 26 80 1A C2 13 21 FE 66 EB 7B 14 C6 i...&....!.f.{..
3CE0 F0 DA 29 F0 DA 2B 83 F2 35 4A 30 00 F8 70 F0 D2 ..)..+..5J0..p..
3CF0 2E EA 26 23 29 86 F4 DD 26 F2 35 4A 30 00 F8 70 ..&#)...&.5J0..p
3D00 F0 D5 2E EA 26 F0 DD 29 F0 DD 2E 86 F4 DF 26 F2 ....&..)......&.
3D10 35 4A 30 00 F8 70 F0 FE 2E EA 26 F0 DF 29 F0 DF 5J0..p....&..)..
3D20 2E 86 F4 DA 26 F2 35 4A 30 00 F8 70 F0 D7 2E EA ....&.5J0..p....
3D30 26 F0 F4 2E 68 09 F4 F4 26 F0 F5 2E 69 00 F4 F5 &...h...&...i...
3D40 26 F0 F2 2E 68 09 F4 F2 26 F0 F3 2E 69 00 F4 F3 &...h...&...i...
3D50 26 F0 D7 87 1A 55 13 F0 DB 2E 68 03 F4 DB 26 F0 &....U....h...&.
3D60 DC 2E 69 00 F4 DC 26 F0 FC 2E 68 03 F4 FC 26 F0 ..i...&...h...&.
3D70 FD 2E 69 00 F4 FD 26 F0 FE 87 1A 08 13 F0 EE 2E ..i...&.........
3D80 68 09 F4 EE 26 F0 EF 2E 69 00 F4 EF 26 F0 F0 2E h...&...i...&...
3D90 68 09 F4 F0 26 F0 F1 2E 69 00 F4 F1 26 F0 D5 87 h...&...i...&...
3DA0 1A 72 12 F0 E1 2E 68 03 F4 E1 26 F0 E2 2E 69 00 .r....h...&...i.
3DB0 F4 E2 26 F0 E4 2E 68 03 F4 E4 26 F0 E5 2E 69 00 ..&...h...&...i.
3DC0 F4 E5 26 F0 E3 2E F4 D2 26 1A 0A 12 F0 DA 2D FC ..&.....&.....-.
3DD0 3E 5C 1E 54 3C 00 00 FE 14 16 F7 FF F4 F7 37 00 >\.T<.........7.
3DE0 F0 F7 2E F0 09 62 EB 4B 16 CF F0 F7 2D FD A7 FD .....b.K....-...
3DF0 A7 34 00 FA 70 FD 31 FC 30 F0 0A 2E F9 60 F4 FE .4..p.1.0....`..
3E00 26 F0 0B 2E F8 61 F4 FF 26 F2 10 4D F0 F7 2B 32 &....a..&..M..+2
3E10 00 F9 15 E5 29 30 00 F9 35 F8 34 FA 70 F8 70 FD ....)0..5.4.p.p.
3E20 31 FC 30 F2 0D 4A F8 70 E2 29 92 E2 28          1.0..J.p.)..(  

;; fn3E2D: 3E2D
fn3E2D proc
	ld	a,(ix-0x09)
	ld	(ix-0x04),a
	ld	(ix-0x03),00
	ex	de,hl
	ld	hl,(sp+0x05)
	ex	de,hl
	inc	de
	ld	hl,(sp+0x10)
	add	hl,de
	ld	e,(hl)
	ld	d,00
	ld	l,e
	ld	h,d
	add	hl,hl
	add	hl,hl
	add	hl,hl
	add	hl,de
	add	hl,bc
	ld	(sp+0x03),hl
	ld	hl,(sp+0x03)
	inc	hl
	inc	hl
	ld	a,(hl)
	ld	(ix-0x08),a
	inc	hl
	ld	a,(hl)
	ld	(ix-0x07),a
	ex	de,hl
	ld	hl,(sp+0x05)
	ex	de,hl
	inc	de
	inc	de
	ld	hl,(sp+0x10)
	add	hl,de
	ld	c,(hl)
	ld	b,00
	ld	l,c
	ld	h,b
	add	hl,hl
	add	hl,bc
	ld	c,l
	ld	b,h
	ld	hl,(sp+0x0D)
	add	hl,bc
	ld	c,(hl)
	inc	hl
	ld	b,(hl)
	ex	de,hl
	ld	hl,(sp+0x05)
	ex	de,hl
	inc	de
	inc	de
	inc	de
	ld	hl,(sp+0x10)
	add	hl,de
	ld	e,(hl)
	ld	d,00
	ld	l,e
	ld	h,d
	add	hl,hl
	add	hl,hl
	add	hl,hl
	add	hl,de
	add	hl,bc
	ld	(sp+0x05),hl
	ld	hl,(sp+0x05)
	inc	hl
	inc	hl
	ld	c,(hl)
	inc	hl
	ld	b,(hl)
	ld	a,(ix-0x08)
	sub	a,c
	ld	c,a
	ld	a,(ix-0x07)
	sbc	a,b
	ld	b,a
	ld	hl,(sp+0x03)
	ld	e,(hl)
	inc	hl
	ld	d,(hl)
	ld	hl,(sp+0x05)
	ld	a,(hl)
	inc	hl
	ld	h,(hl)
	ld	l,a
	ld	a,e
	sub	a,l
	ld	e,a
	ld	a,d
	sbc	a,h
	ld	d,a
	push	bc
	push	de
	call	03AA
	add	sp,0004
	ld	c,l
	ld	b,h
	ld	hl,(sp+0x07)
	ld	(hl),c
	inc	hl
	ld	(hl),b
	inc	(ix-0x09)
	inc	(ix-0x09)
	inc	(ix-0x09)
	inc	(ix-0x09)
	jp	152A
3F01    FC 3E 5C 1E 54 3C 00 00 FE 14 16 F2 FF F4 F2  .>\.T<.........
3F10 37 FE F0 04 29 30 00 F9 35 F8 34 FA 70 F8 70 38 7...)0..5.4.p.p8
3F20 00 80 F8 70 F6 0C 42 F0 05 29 30 00 F9 35 F8 34 ...p..B..)0..5.4
3F30 FA 70 FA 70 FA 70 F8 70 F6 0A 42 F4 F3 37 00 3A .p.p.p.p..B..7.:
3F40 3E 83 F0 F3 2E E2 62 EB 6F 18 CF FE 65 F0 F2 62 >.....b.o...e..b
3F50 F4 F2 26 36 1E F0 F3 60 F4 FA 26 36 80 69 00 F4 ..&6...`..&6.i..
3F60 FB 26 F2 08 4A F0 04 2E E2 62 EB 66 18 CE F0 F3 .&..J....b.f....
3F70 29 30 00 F9 33 F8 32 91 3A 1E 80 F9 70 F0 05 2E )0..3.2.:...p...
3F80 E2 62 EB 66 18 CE F0 F2 2D 34 00 F8 70 FD 31 FC .b.f....-4..p.1.
3F90 30 3A 1E 80 F8 70 F6 08 42 F2 08 4A E2 2B 32 00 0:...p..B..J.+2.
3FA0 FB 35 FA 34 FA 70 F9 70 39 00 80 F9 70 E2 2B 92 .5.4.p.p9...p.+.
3FB0 E2 2A 90 3A 1E 80 F8 70 F6 06 42 F2 06 4A E2 29 .*.:...p..B..J.)
3FC0 30 00 F9 35 F8 34 FA 70 FA 70 FA 70 F8 70 F9 70 0..5.4.p.p.p.p.p
3FD0 FD 31 FC 30 3A 04 00 F8 70 E2 2E F4 F6 26 92 E2 .1.0:...p....&..
3FE0 2E F4 F7 26 F0 F3 2D FD A7 FD A7 34 00 FA 70 39 ...&..-....4..p9
3FF0 AE 81 F9 70 F6 02 42 F0 F7 2E F0 F6 66 EB 7A 17 ...p..B.....f.z.
4000 C6 F2 0C 4A E2 2B 92 E2 2A F2 0A 4A F9 70 39 04 ...J.+..*..J.p9.
4010 00 F9 70 E2 2B 92 E2 2A F2 02 4A E2 2E 92 E2 2C ..p.+..*..J....,
4020 2D F9 70 08 F2 04 4A 08 FE 67 F9 73 EB 66 18 CF -.p...J..g.s.f..
4030 3A 06 00 F8 70 F0 04 2E EA 26 F2 08 4A E2 29 30 :...p....&..J.)0
4040 00 F9 35 F8 34 FA 70 F8 70 39 00 80 F9 70 E2 29 ..5.4.p.p9...p.)
4050 92 E2 28 F2 06 4A E2 2B 32 00 FB 35 FA 34 FA 70 ..(..J.+2..5.4.p
4060 FA 70 FA 70 F9 70 F8 70 38 07 00 F8 70 F0 05 2E .p.p.p.p8...p...
4070 EA 26 F2 08 4A E2 29 30 00 F9 35 F8 34 FA 70 F8 .&..J.)0..5.4.p.
4080 70 39 00 80 F9 70 E2 29 92 E2 28 F2 06 4A E2 2B p9...p.)..(..J.+
4090 32 00 FB 35 FA 34 FA 70 FA 70 FA 70 F9 70 F8 70 2..5.4.p.p.p.p.p
40A0 38 08 00 F8 70 EA 37 01 F2 08 4A E2 29 30 00 F9 8...p.7...J.)0..
40B0 35 F8 34 FA 70 F8 70 39 00 80 F9 70 E2 29 92 E2 5.4.p.p9...p.)..
40C0 28 F2 06 4A E2 2B 32 00 FB 35 FA 34 FA 70 FA 70 (..J.+2..5.4.p.p
40D0 FA 70 F9 70 F8 70 92 92 92 92 FD 31 FC 30 F2 0C .p.p.p.....1.0..
40E0 4A E2 2B 92 E2 2A F2 0A 4A F9 70 39 04 00 F9 70 J.+..*..J.p9...p
40F0 E2 2B 92 E2 2A F2 02 4A E2 2E 92 E2 2C 2D F9 70 .+..*..J....,-.p
4100 08 23 E8 26 90 22 E8 26 F2 06 4A E2 2E F2 08 4A .#.&.".&..J....J
4110 E2 28 56 96 50 96 1C 4F 16 16 02 00 F0 F3 87 F0 .(V.P..O........
4120 F3 87 1A 89 16 FC 3E 5C 1E 54 3C 00 00 FE 14 16 ......>\.T<.....
4130 FB FF 31 00 33 01 F0 06 2E F4 FE 26 F0 07 2E F4 ..1.3......&....
4140 FF 26 F2 03 4A EA 37 00 F4 FD 37 02 F2 03 4A 92 .&..J.7...7...J.
4150 EA 37 01 21 FE 66 CE 06 FB 66 EB 22 19 C6 30 00 .7.!.f...f."..0.
4160 F9 35 F8 34 FA 70 F8 70 FD 31 FC 30 F2 09 4A F8 .5.4.p.p.1.0..J.
4170 70 E2 29 92 E2 28 32 00 FB 35 FA 34 FA 70 FA 70 p.)..(2..5.4.p.p
4180 FA 70 F9 70 F8 70 08 3A 08 00 F9 70 E2 2E FE 66 .p.p.p.:...p...f
4190 CE 04 2D 1A 25 19 3A 06 00 F9 70 E2 29 3A 07 00 ..-.%.:...p.):..
41A0 F9 70 E2 28 F8 33 F0 FD 2D F0 FD 2A 82 F0 FE 2E .p.(.3..-..*....
41B0 FD 60 F4 FB 26 F0 FF 2E 69 00 F4 FC 26 5A 52 EA .`..&...i...&ZR.
41C0 21 22 2D 86 F4 FD 26 F0 FE 2E FD 60 2D F0 FF 2E !"-...&....`-...
41D0 69 00 2C EA 20 1A 9D 18 F0 FD 2D FC 3E 5C 1E 54 i.,. .....-.>\.T
41E0 3C 00 00 FE 14 16 A4 FF 3A 48 00 FE 70 F6 5A 42 <.......:H..p.ZB
41F0 3D 04 00 08 F2 5A 4A 08 F9 15 ED 37 01 F5 01 37 =....ZJ....7...7
4200 00 F0 FE 2E 68 08 29 F0 FF 2E 69 00 28 FE 65 E8 ....h.)...i.(.e.
4210 26 F2 5A 4A EA 37 46 92 EA 37 00 F2 5A 4A 92 92 &.ZJ.7F..7..ZJ..
4220 EA 37 28 92 EA 37 00 3D 09 00 08 F2 5A 4A 08 F9 .7(..7.=....ZJ..
4230 15 F0 FE 2E 68 0D 2D F0 FF 2E 69 00 2C FE 65 EA ....h.-...i.,.e.
4240 26 92 EA 26 F0 FE 2E 68 11 29 F0 FF 2E 69 00 28 &..&...h.)...i.(
4250 FE 65 E8 26 ED 37 00 F5 01 37 00 3D 0B 00 08 F2 .e.&.7...7.=....
4260 5A 4A 08 F9 15 ED 37 00 F5 01 37 00 3A 24 00 FE ZJ....7...7.:$..
4270 70 08 3A 04 00 F9 70 EA 37 00 92 EA 37 00 3A 08 p.:...p.7...7.:.
4280 00 F9 70 EA 37 00 FB 35 FA 34 EA 37 0A 92 EA 37 ..p.7..5.4.7...7
4290 00 FB 35 FA 34 92 92 EA 37 0A 92 EA 37 00 3D 09 ..5.4...7...7.=.
42A0 00 F9 15 3A 0D 00 F9 70 EA 37 00 92 EA 37 00 3A ...:...p.7...7.:
42B0 11 00 F9 70 EA 37 00 ED 37 14 F5 01 37 00 3A 0B ...p.7..7...7.:.
42C0 00 F9 70 EA 37 0A 92 EA 37 00 3D 12 00 F9 15 3A ..p.7...7.=....:
42D0 16 00 F9 70 EA 37 00 92 EA 37 00 3A 1A 00 F9 70 ...p.7...7.:...p
42E0 EA 37 00 ED 37 1E F5 01 37 00 3A 14 00 F9 70 EA .7..7...7.:...p.
42F0 37 1E 92 EA 37 00 3D 1B 00 F9 15 3A 1F 00 F9 70 7...7.=....:...p
4300 EA 37 00 92 EA 37 00 3A 23 00 F9 70 EA 37 00 ED .7...7.:#..p.7..
4310 37 0A F5 01 37 00 3A 1D 00 F9 70 EA 37 1E 92 EA 7...7.:...p.7...
4320 37 00 3A 00 00 FE 70 FD 31 FC 30 3A 04 00 F8 70 7.:...p.1.0:...p
4330 EA 37 00 92 EA 37 00 3A 08 00 F8 70 EA 37 00 F9 .7...7.:...p.7..
4340 35 F8 34 EA 37 32 92 EA 37 00 F9 35 F8 34 92 92 5.4.72..7..5.4..
4350 EA 37 0A 92 EA 37 00 3D 09 00 F8 15 3A 0D 00 F8 .7...7.=....:...
4360 70 EA 37 00 92 EA 37 00 3A 11 00 F8 70 EA 37 00 p.7...7.:...p.7.
4370 ED 37 3C F5 01 37 00 3A 0B 00 F8 70 EA 37 0A 92 .7<..7.:...p.7..
4380 EA 37 00 3D 12 00 F8 15 3A 16 00 F8 70 FE 65 EA .7.=....:...p.e.
4390 26 92 EA 26 3A 1A 00 F8 70 EA 37 00 ED 37 3C F5 &..&:...p.7..7<.
43A0 01 37 00 3A 14 00 F8 70 EA 37 1E 92 EA 37 00 3D .7.:...p.7...7.=
43B0 1B 00 F8 15 3A 1F 00 F8 70 FE 65 EA 26 92 EA 26 ....:...p.e.&..&
43C0 3A 23 00 F8 70 EA 37 00 ED 37 28 F5 01 37 00 3A :#..p.7..7(..7.:
43D0 1D 00 F8 70 EA 37 1E 92 EA 37 00 F2 5A 4A F6 5A ...p.7...7..ZJ.Z
43E0 42 3A 00 80 F0 FE 2E EA 26 92 F0 FF 2E EA 26 3A B:......&.....&:
43F0 02 80 EA 37 02 EB 03 80 41 3A 05 80 EA 37 04 EB ...7....A:...7..
4400 06 80 40 3A 08 80 EA 37 04 3A 1E 80 52 36 03 56 ..@:...7.:..R6.V
4410 96 3A 00 80 52 1C 9A 0F 16 05 00 FD 30 50 3A AE .:..R.......0P:.
4420 81 52 50 96 3A 1E 80 52 36 03 56 96 3A 00 80 52 .RP.:..R6.V.:..R
4430 1C 1D 15 16 08 00 58 3A 3E 83 EA 20 3A 00 00 52 ......X:>.. :..R
4440 1C 4F 16 16 02 00 3A 1E 80 52 3A 00 80 52 1C 73 .O....:..R:..R.s
4450 18 16 04 00 34 00 FC 3E 5C 1E F2 02 48 F2 04 49 ....4..>\...H..I
4460 FE 65 2D F8 66 30 10 CE 05 30 08 21 FA 70 F9 A2 .e-.f0...0.!.p..
4470 A2 CF 02 F9 70 18 F5 1E 3A 03 00 FE 70 E2 2B 9A ....p...:...p.+.
4480 E2 2D 1C F1 1B 1A 2F 1C F2 02 4A F2 04 49 1C FB .-..../...J..I..
4490 1B 1A 2F 1C F2 02 4A F2 04 49 1A FB 1B 3A 03 00 ../...J..I...:..
44A0 FE 70 E2 2B 9A E2 2D 25 A0 FE 63 2C 23 A0 FE 63 .p.+..-%..c,#..c
44B0 2A 24 FA 65 A2 24 56 A2 CF 0A FE 62 FD 62 2D FE *$.e.$V....b.b-.
44C0 63 FC 62 2C FA AF C6 0A FE 62 FB 62 2B FE 63 FA c.b,.....b.b+.c.
44D0 62 2A 1C 54 1C 5E FE DF 28 FE 62 FD 62 2D FE 63 b*.T.^..(.b.b-.c
44E0 FC 62 2C 20 1E A2 08 FE DF FE 62 FD 62 2D FE 63 .b, ......b.b-.c
44F0 FC 62 2C 1E F2 02 4A F2 04 49 C8 0E 3A 03 00 FE .b,...J..I..:...
4500 70 E2 2B 9A E2 2D 34 00 FC 32 23 6C 80 FA 66 CE p.+..-4..2#l..f.
4510 12 30 10 FA 71 A2 FB 62 CF 02 FB 60 0E FA 71 18 .0..q..b...`..q.
4520 F4 2B 1E 30 09 25 FC 35 34 00 FD A3 FA 71 F9 73 .+.0.%.54....q.s
4530 CF 02 F9 70 0E A2 18 F4 F8 A2 F8 32 2B 08 1E 38 ...p.......2+..8
4540 00 00 20 F9 66 C6 08 39 3F 83 3A 89 1C FE 59 1E .. .f..9?.:...Y.
4550 1A 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
4560 1A 3F 02 00 00 00 00 00 00 00 00 00 00 00 00 00 .?..............
4570 1A 4F 02 00 00 00 00 00 1F 00 00 00 00 00 00 00 .O..............
4580 1A 53 02 00 00 00 00 00 1A 57 02 00 00 00 00 00 .S.......W......
4590 1F 00 00 00 00 00 00 00 1A 60 02 00 00 00 00 00 .........`......
45A0 1A 64 02 00 00 00 00 00 1A 68 02 00 00 00 00 00 .d.......h......
45B0 1A 6C 02 00 00 00 00 00 1F 00 00 00 00 00 00 00 .l..............
45C0 1A 32 02 00 00 00 00 00 1A 39 02 00 00 00 00 00 .2.......9......
45D0 00 3E A0 FF EF C2 6E 04 37 C6 F7 37 C7 A5 37 C8 .>....n.7..7..7.
45E0 00 37 C9 03 37 CD 03 37 CE F0 37 CB A0 37 CF 00 .7..7..7..7..7..
45F0 37 E4 35 37 D0 08 37 D1 08 37 E5 01 37 DA 40 37 7.57..7..7..7.@7
4600 E9 01 EF E9 6E 08 EF E9 6E 20 37 EA 00 37 D2 C0 ....n...n 7..7..
4610 37 DB 20 EF DB 6E E0 B8 E6 B9 E6 3A 20 FF 36 00 7. ..n.....: .6.
4620 1C A4 02 EA 26 92 7F C0 FF CE F5 1C A4 02 B0 BF ....&...........
4630 1C 00 00 1C A4 02 1C B7 0C 1C CD 03 01 50 E3 00 .............P..
4640 00 4D E7 ED 28 E3 00 00 2E 2F ED E5 2E EF ED 20 .M..(..../..... 
4650 EB 00 00 26 58 1E 50 E3 00 00 4D E7 ED 28 E3 00 ...&X.P...M..(..
4660 00 2E 2F ED E3 00 00 2E ED 26 EF ED 20 58 1E 27 ../......&.. X.'
4670 EB 2F BE B9 BF 1F B0 BF 37 C3 0F 1F 52 F2 04 4A ./......7...R..J
4680 4F BC F2 02 4A 4F BA 1C 00 00 5A 1F 1C 00 00 1F O...JO....Z.....
4690 1C 00 00 1F 97 B8 37 D3 4E 37 C2 10 1F 1C 00 00 ......7.N7......
46A0 1F 1C 00 00 1F 1C 00 00 1F 1C 00 00 1F F2 02 2E ................
46B0 A8 BF C6 0D 02 A8 C3 CE 08 03 00 00 1C A4 02 C8 ................
46C0 EF 02 37 C3 0F 2F EB B8 BF 03 1E F2 02 4A E2 2E ..7../.......J..
46D0 FE 66 FE D6 1C 73 02 92 C8 F4 37 D2 00 37 D3 B1 .f...s....7..7..
46E0 1E 37 D3 4E 1E F2 04 4A E2 29 30 00 F2 02 4A F8 .7.N...J.)0...J.
46F0 70 52 1C 00 00 16 02 00 1E 3A 02 00 FE 70 1E 58 pR.......:...p.X
4700 5A 52 50 E2 29 92 E2 28 50 1C 00 00 16 02 00 1E ZRP.)..(P.......
4710 3A 00 00 1E 3A 04 00 FE 70 3D 02 00 FE 15 E5 2E :...:...p=......
4720 E2 60 29 F1 01 2E 92 E2 61 28 50 1C 00 00 16 02 .`).....a(P.....
4730 00 1E 3A 00 00 1E 54 3C 00 00 FE 14 F2 06 4A E2 ..:...T<......J.
4740 29 92 E2 28 E0 2E 29 30 00 F0 04 2E F9 60 29 F0 )..(..)0.....`).
4750 05 2E F8 61 28 F2 08 4A E2 2D 34 00 F8 70 5C 1E ...a(..J.-4..p\.
4760 3D 02 00 FE 15 E5 2D 34 00 1E 3D 02 00 FE 15 E5 =.....-4..=.....
4770 2D 34 00 1E 58 5A 52 50 E2 2D 34 00 1E 35 00 1E -4..XZRP.-4..5..
4780 58 5A 52 50 E2 29 30 00 50 1C 00 00 16 02 00 1E XZRP.)0.P.......
4790 54 3C 00 00 FE 14 16 FC FF F2 08 4A E2 29 30 00 T<.........J.)0.
47A0 F0 06 2E F9 60 29 F0 07 2E F8 61 28 08 F2 0C 4A ....`)....a(...J
47B0 08 50 3A 02 00 FE 70 08 38 04 00 FE 59 58 20 A2 .P:...p.8...YX .
47C0 FE 63 2B 2A 21 F0 FC 60 2D 20 F0 FD 61 2C 23 F0 .c+*!..`- ..a,#.
47D0 FE 61 22 F0 FF 61 FC 3E 5C 1E 54 F2 04 4A E2 29 .a"..a.>\.T..J.)
47E0 92 E2 28 92 E2 2B 92 E2 2A F9 35 F8 34 5C 1E 3A ..(..+..*.5.4\.:
47F0 04 00 FE 70 3D 02 00 FE 15 E5 2E E2 60 29 F1 01 ...p=.......`)..
4800 2E 92 E2 61 28 F9 35 F8 34 1E 54 3C 00 00 FE 14 ...a(.5.4.T<....
4810 16 FE FF 3A E3 04 EB 00 80 42 3A 01 00 EB 02 80 ...:.....B:.....
4820 42 1C F5 02 52 1C F5 02 52 1C D7 02 16 04 00 3A B...R...R......:
4830 E5 04 52 F2 0A 4A 52 F2 0A 4A 52 1C F9 02 16 06 ..R..JR..JR.....
4840 00 EE 42 85 CE 05 84 EB B0 04 C6 F0 FE 2E 6A 6F ..B...........jo
4850 CE 07 F0 FF 2E FE 66 C6 15 F0 FE 2E 6A 72 EB F2 ......f.....jr..
4860 03 CE F0 FF 2E FE 66 EB 7C 04 C6 1A F2 03 E3 00 ......f.|.......
4870 80 48 E0 2E FE 66 EB 67 04 C6 2B 32 00 50 51 E3 .H...f.g..+2.PQ.
4880 00 80 4A 52 1C B2 03 16 04 00 58 24 FD 66 CE 10 ..JR......X$.f..
4890 50 3A EA 04 52 3A 01 00 52 1C A8 02 16 04 00 58 P:..R:..R......X
48A0 90 1A 35 04 3A 00 80 3D 02 80 21 E2 62 ED 26 20 ..5.:..=..!.b.& 
48B0 92 E2 63 95 ED 26 1A F2 03 E3 00 80 4A 52 1C 37 ..c..&......JR.7
48C0 03 16 02 00 FD 31 FC 30 EB 10 80 40 FE 65 F9 67 .....1.0...@.e.g
48D0 F8 63 EB 9B 04 CC 6D 80 EB F2 03 C5 3A 03 05 52 .c....m.....:..R
48E0 3A 01 00 52 1C A8 02 16 04 00 1A F2 03 38 14 00 :..R.........8..
48F0 F9 33 F8 32 98 22 FB 66 C6 19 50 1C 07 06 58 85 .3.2.".f..P...X.
4900 CE EE 84 CE EB 50 3A 00 00 52 1C 00 00 16 02 00 .....P:..R......
4910 58 C8 DD 3A 00 00 52 1C 1E 05 16 02 00 1A B0 04 X..:..R.........
4920 2B 00 72 3A 6F 3A 00 61 72 69 74 68 6D 65 74 69 +.r:o:.arithmeti
4930 63 3A 20 75 6E 6B 6E 6F 77 6E 20 6B 65 79 2E 00 c: unknown key..
4940 61 72 69 74 68 6D 65 74 69 63 3A 20 69 6E 76 61 arithmetic: inva
4950 6C 69 64 20 72 61 6E 67 65 2E 00 3A 06 80 3D 04 lid range..:..=.
4960 80 E5 2E E2 60 29 F1 01 2E 92 E2 61 28 FE 65 F9 ....`).....a(.e.
4970 67 F8 63 EB 3C 05 CC 6D 80 EB 68 05 CD 3A 91 05 g.c.<..m..h..:..
4980 52 1C 43 03 16 02 00 FE 65 3D 04 80 E5 67 F1 01 R.C.....e=...g..
4990 63 EB 5A 05 CC 6D 80 EB 68 05 CD 3A B4 05 52 1C c.Z..m..h..:..R.
49A0 43 03 16 02 00 3A 03 00 FE 70 E2 2E 9A E2 66 CE C....:...p....f.
49B0 12 3A E8 05 52 1C 43 03 16 02 00 1C 40 03 25 FE .:..R.C.....@.%.
49C0 66 C6 F8 3A 05 06 52 1C 43 03 16 02 00 1E 0A 0A f..:..R.C.......
49D0 52 69 67 68 74 73 20 25 64 3B 20 57 72 6F 6E 67 Rights %d; Wrong
49E0 73 20 25 64 3B 20 53 63 6F 72 65 20 25 64 25 25 s %d; Score %d%%
49F0 00 0A 54 6F 74 61 6C 20 74 69 6D 65 20 25 6C 64 ..Total time %ld
4A00 20 73 65 63 6F 6E 64 73 3B 20 25 2E 31 66 20 73  seconds; %.1f s
4A10 65 63 6F 6E 64 73 20 70 65 72 20 70 72 6F 62 6C econds per probl
4A20 65 6D 0A 0A 00 50 72 65 73 73 20 52 45 54 55 52 em...Press RETUR
4A30 4E 20 74 6F 20 63 6F 6E 74 69 6E 75 65 2E 2E 2E N to continue...
4A40 0A 00 0A 00 54 3C 00 00 FE 14 16 DE FF 3A 00 00 ....T<.......:..
4A50 F6 18 42 35 00 F6 16 42 35 00 F6 1A 42 1C D3 02 ..B5...B5...B...
4A60 FD 31 FC 30 E3 02 80 4A 52 50 1C FE 0B 16 04 00 .1.0...JRP......
4A70 FD 31 FC 30 E3 00 80 4A F8 70 E2 29 F4 DE 21 F4 .1.0...J.p.)..!.
4A80 DF 37 00 F0 DE 2E 6A 2F CE 0B F0 DF 2E FE 66 CE .7....j/......f.
4A90 04 36 01 C8 02 FE 65 29 F9 A8 CE 19 E3 10 80 49 .6....e).......I
4AA0 91 50 3A 01 00 52 F2 04 4A 52 51 1C C2 09 16 06 .P:..R..JRQ.....
4AB0 00 58 F6 1A 42 F0 DE 2E 6A 78 CE 0B F0 DF 2E FE .X..B...jx......
4AC0 66 CE 04 36 01 C8 02 FE 65 F4 FF 26 F0 DE 2E 6A f..6....e..&...j
4AD0 2D CE 0B F0 DF 2E FE 66 CE 04 36 01 C8 02 FE 65 -......f..6....e
4AE0 F4 FE 26 F4 FD 21 F0 DE 2E 6A 2B CE 0B F0 DF 2E ..&..!...j+.....
4AF0 FE 66 CE 04 36 01 C8 02 FE 65 F4 FC 26 E3 10 80 .f..6....e..&...
4B00 48 90 F0 FC 2E FE 66 CE 1E F0 FE 2E FE 66 EB 10 H.....f......f..
4B10 07 CE F0 FD 2E FE 66 EB 5C 07 CE F0 FF 2E FE 66 ......f.\......f
4B20 EB 36 07 CE 1A B1 07 3A 00 00 08 5A 52 51 52 50 .6.....:...ZRQRP
4B30 1C C2 09 16 06 00 F6 16 42 F0 F4 2E F0 F8 60 2D ........B.....`-
4B40 F0 F5 2E F0 F9 61 2C F6 18 42 1A B1 07 3A 00 00 .....a,..B...:..
4B50 08 5A 52 51 52 50 1C C2 09 16 06 00 F6 18 42 F0 .ZRQRP........B.
4B60 F8 2E F0 F6 60 2D F0 F9 2E F0 F7 61 2C F6 16 42 ....`-.....a,..B
4B70 1A B1 07 3A 00 00 08 5A 52 51 52 50 1C C2 09 16 ...:...ZRQRP....
4B80 06 00 F6 16 42 F2 1A 4A 52 F2 18 4A 52 1C D0 0B ....B..JR..JR...
4B90 16 04 00 F6 18 42 1A B1 07 3A 01 00 FD 31 FC 30 .....B...:...1.0
4BA0 5A 52 50 52 E3 10 80 4A 52 1C C2 09 16 06 00 92 ZRPR...JR.......
4BB0 F6 1A 42 E3 10 80 48 90 3A 00 00 08 5A 52 51 52 ..B...H.:...ZRQR
4BC0 50 1C C2 09 16 06 00 F6 18 42 F2 18 4A 52 F2 1C P........B..JR..
4BD0 4A 52 1C D0 0B 16 04 00 52 1C D3 02 08 F2 1C 4A JR......R......J
4BE0 52 51 1C FE 0B 16 04 00 58 F8 70 F6 16 42 F0 F7 RQ......X.p..B..
4BF0 AF EB C0 06 CE F0 F5 AF EB C0 06 CE 3A DE 08 52 ............:..R
4C00 1C 43 03 16 02 00 3A 00 00 52 1C 9D 03 16 02 00 .C....:..R......
4C10 3A 02 00 FE 70 F6 1C 42 F2 1C 4A 38 00 00 50 38 :...p..B..J8..P8
4C20 14 00 50 52 1C 53 03 16 06 00 FD 31 24 F9 66 CE ..PR.S.....1$.f.
4C30 10 3A EC 08 52 1C 43 03 16 02 00 3A FF FF 1A DA .:..R.C....:....
4C40 08 F2 1C 4A FD 31 FC 30 E0 2E FE 66 C6 10 50 56 ...J.1.0...f..PV
4C50 96 1C 2D 03 96 58 24 FD 66 C6 03 90 C8 EA E0 2E ..-..X$.f.......
4C60 2A 50 51 96 1C 23 03 96 58 24 FD 66 CE 0D 3A EE *PQ..#..X$.f..:.
4C70 08 52 1C 43 03 16 02 00 1A DB 07 50 1C 37 03 16 .R.C.......P.7..
4C80 02 00 FD 31 FC 30 F0 F6 2E F9 62 CE 20 F0 F7 2E ...1.0....b. ...
4C90 F8 62 CE 19 3A 05 09 52 1C 43 03 16 02 00 3D 04 .b..:..R.C....=.
4CA0 80 E5 87 EB D7 08 CE F1 01 87 1A D7 08 3A 0D 09 .............:..
4CB0 52 1C 43 03 16 02 00 3D 06 80 E5 87 CE 03 F1 01 R.C....=........
4CC0 87 3A 01 00 FD 31 FC 30 5A 52 50 52 F2 1E 4A 52 .:...1.0ZRPR..JR
4CD0 1C 14 09 16 06 00 F0 FF 2E FE 66 CE 07 F0 FC 2E ..........f.....
4CE0 FE 66 C6 18 3A 00 00 FD 31 FC 30 5A 52 50 52 F2 .f..:...1.0ZRPR.
4CF0 1A 4A 52 1C 14 09 16 06 00 1A DB 07 3A 00 00 FD .JR.........:...
4D00 31 FC 30 5A 52 50 52 F2 1C 4A 52 1C 14 09 16 06 1.0ZRPR..JR.....
4D10 00 1A DB 07 3A 00 00 FC 3E 5C 1E 25 64 20 25 63 ....:...>\.%d %c
4D20 20 25 64 20 3D 20 20 20 00 0A 00 50 6C 65 61 73  %d =   ...Pleas
4D30 65 20 74 79 70 65 20 61 20 6E 75 6D 62 65 72 2E e type a number.
4D40 0A 00 52 69 67 68 74 21 0A 00 57 68 61 74 3F 0A ..Right!..What?.
4D50 00 54 3C 00 00 FE 14 16 F8 FF F2 0E 4A 52 1C 59 .T<.........JR.Y
4D60 0B 16 02 00 52 3A 0C 00 52 1C BC 02 16 02 00 58 ....R:..R......X
4D70 EE 42 24 FD 66 EB BE 09 C6 F0 F8 2E 68 04 F4 FA .B$.f.......h...
4D80 26 F0 F9 2E 69 00 F4 FB 26 3A 0C 80 F9 A4 F8 A2 &...i...&:......
4D90 F9 A4 F8 A2 F8 70 08 F2 10 4A 08 FB A4 FA A2 F9 .....p...J......
4DA0 70 F6 06 42 F2 06 4A E2 2E F4 FC 26 92 E2 2E F4 p..B..J....&....
4DB0 FD 26 F2 02 4A F0 FC 2E EA 26 92 F0 FD 2E EA 26 .&..J....&.....&
4DC0 F2 06 4A F0 F8 2E EA 26 92 F0 F9 2E EA 26 3A 08 ..J....&.....&:.
4DD0 80 F8 70 F9 70 52 5D 5A 52 92 92 EA 37 05 92 EA ..p.pR]ZR...7...
4DE0 37 00 E5 4A 38 05 00 F8 70 ED 25 F5 01 24 5A 52 7..J8...p.%..$ZR
4DF0 F0 04 2E EA 26 92 F0 05 2E EA 26 FC 3E 5C 1E 54 ....&.....&.>\.T
4E00 3C 00 00 FE 14 16 F4 FF F2 12 4A 52 1C 59 0B 16 <.........JR.Y..
4E10 02 00 52 1C D3 02 F4 FF 24 F4 FE 25 59 38 08 80 ..R.....$..%Y8..
4E20 FB A4 FA A2 FB A4 FA A2 F4 FC 23 F4 FD 22 F2 08 ..........#.."..
4E30 4A F8 70 08 F2 14 4A 08 FB A4 FA A2 F9 70 F6 06 J.p...J......p..
4E40 42 F2 06 4A E2 29 92 E2 28 F2 10 4A F8 70 50 51 B..J.)..(..J.pPQ
4E50 52 F2 10 4A 52 1C FE 0B 16 04 00 59 58 F6 0A 42 R..JR......YX..B
4E60 F0 FE 2E F0 04 62 F0 FF 2E F0 05 63 EB 35 0A CC .....b.....c.5..
4E70 6D 80 EB 3F 0A CD F2 0A 4A 1A 2C 0B F0 FE 2E F0 m..?....J.,.....
4E80 04 62 F4 FE 26 F0 FF 2E F0 05 63 F4 FF 26 36 0C .b..&.....c..&6.
4E90 F0 FC 60 2D 36 80 F0 FD 61 2C F9 70 08 FB 35 FA ..`-6...a,.p..5.
4EA0 34 E2 2E 92 E2 2C 2D F6 08 42 24 FD 66 EB 1B 0B 4....,-..B$.f...
4EB0 C6 F2 08 4D 95 95 E5 2E F4 F8 26 F1 01 2E F4 F9 ...M......&.....
4EC0 26 F0 FC 2E 68 04 F4 F6 26 F0 FD 2E 69 00 F4 F7 &...h...&...i...
4ED0 26 F0 FE 2E F0 F8 62 F0 FF 2E F0 F9 63 EB A6 0A &.....b.....c...
4EE0 CC 6D 80 EB 01 0B CD F2 08 4A E2 2E F4 F4 26 92 .m.......J....&.
4EF0 E2 2E F4 F5 26 98 F2 06 4A EA 21 92 EA 20 E5 4A ....&...J.!.. .J
4F00 9A ED 25 F5 01 24 FE 65 FD 67 FC 63 EB D5 0A CC ..%..$.e.g.c....
4F10 6D 80 EB FD 0A C5 F2 02 4A E2 29 92 E2 28 FB 35 m.......J.)..(.5
4F20 FA 34 E2 2E 92 E2 2C 2D 50 51 52 1C C2 02 16 02 .4....,-PQR.....
4F30 00 59 58 21 E9 26 91 20 E9 26 5A 52 C8 2B F0 FE .YX!.&. .&ZR.+..
4F40 2E F0 F8 62 F4 FE 26 F0 FF 2E F0 F9 63 F4 FF 26 ...b..&.....c..&
4F50 08 F2 02 4A 08 1A 60 0A 3A 30 0B 52 3A 01 00 52 ...J..`.:0.R:..R
4F60 1C A8 02 16 04 00 3A 01 00 FC 3E 5C 1E 61 72 69 ......:...>\.ari
4F70 74 68 6D 65 74 69 63 3A 20 62 75 67 3A 20 69 6E thmetic: bug: in
4F80 63 6F 6E 73 69 73 74 65 6E 74 20 70 65 6E 61 6C consistent penal
4F90 74 69 65 73 2E 00 54 3C 00 00 FE 14 38 00 00 F0 ties..T<....8...
4FA0 05 2E F0 04 66 C6 17 F2 04 4A 52 E3 00 80 4A 52 ....f....JR...JR
4FB0 1C B2 03 16 04 00 FD 31 24 28 FD 66 CE 10 50 3A .......1$(.f..P:
4FC0 A3 0B 52 3A 01 00 52 1C A8 02 16 04 00 58 3A 00 ..R:..R......X:.
4FD0 80 21 E2 62 29 20 92 E2 63 28 F9 35 F8 34 5C 1E .!.b) ..c(.5.4\.
4FE0 61 72 69 74 68 6D 65 74 69 63 3A 20 62 75 67 3A arithmetic: bug:
4FF0 20 6F 70 20 25 63 20 6E 6F 74 20 69 6E 20 64 65  op %c not in de
5000 66 61 75 6C 74 6B 65 79 73 20 25 73 00 F2 02 48 faultkeys %s...H
5010 F2 04 49 FE 65 2D F8 66 30 10 CE 05 30 08 21 FA ..I.e-.f0...0.!.
5020 70 F9 A2 A2 CF 02 F9 70 18 F5 1E 3A 03 00 FE 70 p......p...:...p
5030 E2 2B 9A E2 2D 1C 1D 0C 1A 5B 0C F2 02 4A F2 04 .+..-....[...J..
5040 49 1C 27 0C 1A 5B 0C F2 02 4A F2 04 49 1A 27 0C I.'..[...J..I.'.
5050 3A 03 00 FE 70 E2 2B 9A E2 2D 25 A0 FE 63 2C 23 :...p.+..-%..c,#
5060 A0 FE 63 2A 24 FA 65 A2 24 56 A2 CF 0A FE 62 FD ..c*$.e.$V....b.
5070 62 2D FE 63 FC 62 2C FA AF C6 0A FE 62 FB 62 2B b-.c.b,.....b.b+
5080 FE 63 FA 62 2A 1C 80 0C 5E FE DF 28 FE 62 FD 62 .c.b*...^..(.b.b
5090 2D FE 63 FC 62 2C 20 1E A2 08 FE DF FE 62 FD 62 -.c.b, ......b.b
50A0 2D FE 63 FC 62 2C 1E F2 02 4A F2 04 49 C8 0E 3A -.c.b,...J..I..:
50B0 03 00 FE 70 E2 2B 9A E2 2D 34 00 FC 32 23 6C 80 ...p.+..-4..2#l.
50C0 FA 66 CE 12 30 10 FA 71 A2 FB 62 CF 02 FB 60 0E .f..0..q..b...`.
50D0 FA 71 18 F4 2B 1E 30 09 25 FC 35 34 00 FD A3 FA .q..+.0.%.54....
50E0 71 F9 73 CF 02 F9 70 0E A2 18 F4 F8 A2 F8 32 2B q.s...p.......2+
50F0 08 1E 0A 00 38 02 00 20 F9 66 C6 08 39 10 80 3A ....8.. .f..9..:
5100 B5 0C FE 59 1E 1A 00 01 00 00 00 00 00 00 00 00 ...Y............
5110 00 00 00 00 00 1A 3F 02 00 00 00 00 00 00 00 00 ......?.........
5120 00 00 00 00 00 1A 4F 02 00 00 00 00 00 1F 00 00 ......O.........
5130 00 00 00 00 00 1A 53 02 00 00 00 00 00 1A 57 02 ......S.......W.
5140 00 00 00 00 00 1F 00 00 00 00 00 00 00 1A 60 02 ..............`.
5150 00 00 00 00 00 1A 64 02 00 00 00 00 00 1A 68 02 ......d.......h.
5160 00 00 00 00 00 1A 6C 02 00 00 00 00 00 1F 00 00 ......l.........
5170 00 00 00 00 00 1A 32 02 00 00 00 00 00 1A 39 02 ......2.......9.
5180 00 00 00 00 00 00 3E A0 FF EF C2 6E 04 37 C6 F7 ......>....n.7..
5190 37 C7 A5 37 C8 00 37 C9 03 37 CD 03 37 CE F0 37 7..7..7..7..7..7
51A0 CB A0 37 CF 00 37 E4 35 37 D0 08 37 D1 08 37 E5 ..7..7.57..7..7.
51B0 01 37 DA 40 37 E9 01 EF E9 6E 08 EF E9 6E 20 37 .7.@7....n...n 7
51C0 EA 00 37 D2 C0 37 DB 20 EF DB 6E E0 B8 E6 B9 E6 ..7..7. ..n.....
51D0 3A 20 FF 36 00 1C A4 02 EA 26 92 7F C0 FF CE F5 : .6.....&......
51E0 1C A4 02 B0 BF 1C 00 00 1C A4 02 1C B7 0C 1C CD ................
51F0 03 01 50 E3 00 00 4D E7 ED 28 E3 00 00 2E 2F ED ..P...M..(..../.
5200 E5 2E EF ED 20 EB 00 00 26 58 1E 50 E3 00 00 4D .... ...&X.P...M
5210 E7 ED 28 E3 00 00 2E 2F ED E3 00 00 2E ED 26 EF ..(..../......&.
5220 ED 20 58 1E 27 EB 2F BE B9 BF 1F B0 BF 37 C3 0F . X.'./......7..
5230 1F 52 F2 04 4A 4F BC F2 02 4A 4F BA 1C 00 00 5A .R..JO...JO....Z
5240 1F 1C 00 00 1F 1C 00 00 1F 97 B8 37 D3 4E 37 C2 ...........7.N7.
5250 10 1F 1C 00 00 1F 1C 00 00 1F 1C 00 00 1F 1C 00 ................
5260 00 1F F2 02 2E A8 BF C6 0D 02 A8 C3 CE 08 03 00 ................
5270 00 1C A4 02 C8 EF 02 37 C3 0F 2F EB B8 BF 03 1E .......7../.....
5280 F2 02 4A E2 2E FE 66 FE D6 1C 73 02 92 C8 F4 37 ..J...f...s....7
5290 D2 00 37 D3 B1 1E 37 D3 4E 1E F2 04 4A E2 29 30 ..7...7.N...J.)0
52A0 00 F2 02 4A F8 70 52 1C 00 00 16 02 00 1E 3A 02 ...J.pR.......:.
52B0 00 FE 70 1E 58 5A 52 50 E2 29 92 E2 28 50 1C 00 ..p.XZRP.)..(P..
52C0 00 16 02 00 1E 3A 00 00 1E 3A 04 00 FE 70 3D 02 .....:...:...p=.
52D0 00 FE 15 E5 2E E2 60 29 F1 01 2E 92 E2 61 28 50 ......`).....a(P
52E0 1C 00 00 16 02 00 1E 3A 00 00 1E 54 3C 00 00 FE .......:...T<...
52F0 14 F2 06 4A E2 29 92 E2 28 E0 2E 29 30 00 F0 04 ...J.)..(..)0...
5300 2E F9 60 29 F0 05 2E F8 61 28 F2 08 4A E2 2D 34 ..`)....a(..J.-4
5310 00 F8 70 5C 1E 3D 02 00 FE 15 E5 2D 34 00 1E 3D ..p\.=.....-4..=
5320 02 00 FE 15 E5 2D 34 00 1E 58 5A 52 50 E2 2D 34 .....-4..XZRP.-4
5330 00 1E 35 00 1E 58 5A 52 50 E2 29 30 00 50 1C 00 ..5..XZRP.)0.P..
5340 00 16 02 00 1E 54 3C 00 00 FE 14 16 FC FF F2 08 .....T<.........
5350 4A E2 29 30 00 F0 06 2E F9 60 29 F0 07 2E F8 61 J.)0.....`)....a
5360 28 08 F2 0C 4A 08 50 3A 02 00 FE 70 08 38 04 00 (...J.P:...p.8..
5370 FE 59 58 20 A2 FE 63 2B 2A 21 F0 FC 60 2D 20 F0 .YX ..c+*!..`- .
5380 FD 61 2C 23 F0 FE 61 22 F0 FF 61 FC 3E 5C 1E 54 .a,#..a"..a.>\.T
5390 F2 04 4A E2 29 92 E2 28 92 E2 2B 92 E2 2A F9 35 ..J.)..(..+..*.5
53A0 F8 34 5C 1E 3A 04 00 FE 70 3D 02 00 FE 15 E5 2E .4\.:...p=......
53B0 E2 60 29 F1 01 2E 92 E2 61 28 F9 35 F8 34 1E 54 .`).....a(.5.4.T
53C0 3C 00 00 FE 14 16 FE FF 3A E3 04 EB 00 80 42 3A <.......:.....B:
53D0 01 00 EB 02 80 42 1C F5 02 52 1C F5 02 52 1C D7 .....B...R...R..
53E0 02 16 04 00 3A E5 04 52 F2 0A 4A 52 F2 0A 4A 52 ....:..R..JR..JR
53F0 1C F9 02 16 06 00 EE 42 85 CE 05 84 EB B0 04 C6 .......B........
5400 F0 FE 2E 6A 6F CE 07 F0 FF 2E FE 66 C6 15 F0 FE ...jo......f....
5410 2E 6A 72 EB F2 03 CE F0 FF 2E FE 66 EB 7C 04 C6 .jr........f.|..
5420 1A F2 03 E3 00 80 48 E0 2E FE 66 EB 67 04 C6 2B ......H...f.g..+
5430 32 00 50 51 E3 00 80 4A 52 1C B2 03 16 04 00 58 2.PQ...JR......X
5440 24 FD 66 CE 10 50 3A EA 04 52 3A 01 00 52 1C A8 $.f..P:..R:..R..
5450 02 16 04 00 58 90 1A 35 04 3A 00 80 3D 02 80 21 ....X..5.:..=..!
5460 E2 62 ED 26 20 92 E2 63 95 ED 26 1A F2 03 E3 00 .b.& ..c..&.....
5470 80 4A 52 1C 37 03 16 02 00 FD 31 FC 30 EB 10 80 .JR.7.....1.0...
5480 40 FE 65 F9 67 F8 63 EB 9B 04 CC 6D 80 EB F2 03 @.e.g.c....m....
5490 C5 3A 03 05 52 3A 01 00 52 1C A8 02 16 04 00 1A .:..R:..R.......
54A0 F2 03 38 14 00 F9 33 F8 32 98 22 FB 66 C6 19 50 ..8...3.2.".f..P
54B0 1C 07 06 58 85 CE EE 84 CE EB 50 3A 00 00 52 1C ...X......P:..R.
54C0 00 00 16 02 00 58 C8 DD 3A 00 00 52 1C 1E 05 16 .....X..:..R....
54D0 02 00 1A B0 04 2B 00 72 3A 6F 3A 00 61 72 69 74 .....+.r:o:.arit
54E0 68 6D 65 74 69 63 3A 20 75 6E 6B 6E 6F 77 6E 20 hmetic: unknown 
54F0 6B 65 79 2E 00 61 72 69 74 68 6D 65 74 69 63 3A key..arithmetic:
5500 20 69 6E 76 61 6C 69 64 20 72 61 6E 67 65 2E 00  invalid range..
5510 3A 06 80 3D 04 80 E5 2E E2 60 29 F1 01 2E 92 E2 :..=.....`).....
5520 61 28 FE 65 F9 67 F8 63 EB 3C 05 CC 6D 80 EB 68 a(.e.g.c.<..m..h
5530 05 CD 3A 91 05 52 1C 43 03 16 02 00 FE 65 3D 04 ..:..R.C.....e=.
5540 80 E5 67 F1 01 63 EB 5A 05 CC 6D 80 EB 68 05 CD ..g..c.Z..m..h..
5550 3A B4 05 52 1C 43 03 16 02 00 3A 03 00 FE 70 E2 :..R.C....:...p.
5560 2E 9A E2 66 CE 12 3A E8 05 52 1C 43 03 16 02 00 ...f..:..R.C....
5570 1C 40 03 25 FE 66 C6 F8 3A 05 06 52 1C 43 03 16 .@.%.f..:..R.C..
5580 02 00 1E 0A 0A 52 69 67 68 74 73 20 25 64 3B 20 .....Rights %d; 
5590 57 72 6F 6E 67 73 20 25 64 3B 20 53 63 6F 72 65 Wrongs %d; Score
55A0 20 25 64 25 25 00 0A 54 6F 74 61 6C 20 74 69 6D  %d%%..Total tim
55B0 65 20 25 6C 64 20 73 65 63 6F 6E 64 73 3B 20 25 e %ld seconds; %
55C0 2E 31 66 20 73 65 63 6F 6E 64 73 20 70 65 72 20 .1f seconds per 
55D0 70 72 6F 62 6C 65 6D 0A 0A 00 50 72 65 73 73 20 problem...Press 
55E0 52 45 54 55 52 4E 20 74 6F 20 63 6F 6E 74 69 6E RETURN to contin
55F0 75 65 2E 2E 2E 0A 00 0A 00 54 3C 00 00 FE 14 16 ue.......T<.....
5600 DE FF 3A 00 00 F6 18 42 35 00 F6 16 42 35 00 F6 ..:....B5...B5..
5610 1A 42 1C D3 02 FD 31 FC 30 E3 02 80 4A 52 50 1C .B....1.0...JRP.
5620 FE 0B 16 04 00 FD 31 FC 30 E3 00 80 4A F8 70 E2 ......1.0...J.p.
5630 29 F4 DE 21 F4 DF 37 00 F0 DE 2E 6A 2F CE 0B F0 )..!..7....j/...
5640 DF 2E FE 66 CE 04 36 01 C8 02 FE 65 29 F9 A8 CE ...f..6....e)...
5650 19 E3 10 80 49 91 50 3A 01 00 52 F2 04 4A 52 51 ....I.P:..R..JRQ
5660 1C C2 09 16 06 00 58 F6 1A 42 F0 DE 2E 6A 78 CE ......X..B...jx.
5670 0B F0 DF 2E FE 66 CE 04 36 01 C8 02 FE 65 F4 FF .....f..6....e..
5680 26 F0 DE 2E 6A 2D CE 0B F0 DF 2E FE 66 CE 04 36 &...j-......f..6
5690 01 C8 02 FE 65 F4 FE 26 F4 FD 21 F0 DE 2E 6A 2B ....e..&..!...j+
56A0 CE 0B F0 DF 2E FE 66 CE 04 36 01 C8 02 FE 65 F4 ......f..6....e.
56B0 FC 26 E3 10 80 48 90 F0 FC 2E FE 66 CE 1E F0 FE .&...H.....f....
56C0 2E FE 66 EB 10 07 CE F0 FD 2E FE 66 EB 5C 07 CE ..f........f.\..
56D0 F0 FF 2E FE 66 EB 36 07 CE 1A B1 07 3A 00 00 08 ....f.6.....:...
56E0 5A 52 51 52 50 1C C2 09 16 06 00 F6 16 42 F0 F4 ZRQRP........B..
56F0 2E F0 F8 60 2D F0 F5 2E F0 F9 61 2C F6 18 42 1A ...`-.....a,..B.
5700 B1 07 3A 00 00 08 5A 52 51 52 50 1C C2 09 16 06 ..:...ZRQRP.....
5710 00 F6 18 42 F0 F8 2E F0 F6 60 2D F0 F9 2E F0 F7 ...B.....`-.....
5720 61 2C F6 16 42 1A B1 07 3A 00 00 08 5A 52 51 52 a,..B...:...ZRQR
5730 50 1C C2 09 16 06 00 F6 16 42 F2 1A 4A 52 F2 18 P........B..JR..
5740 4A 52 1C D0 0B 16 04 00 F6 18 42 1A B1 07 3A 01 JR........B...:.
5750 00 FD 31 FC 30 5A 52 50 52 E3 10 80 4A 52 1C C2 ..1.0ZRPR...JR..
5760 09 16 06 00 92 F6 1A 42 E3 10 80 48 90 3A 00 00 .......B...H.:..
5770 08 5A 52 51 52 50 1C C2 09 16 06 00 F6 18 42 F2 .ZRQRP........B.
5780 18 4A 52 F2 1C 4A 52 1C D0 0B 16 04 00 52 1C D3 .JR..JR......R..
5790 02 08 F2 1C 4A 52 51 1C FE 0B 16 04 00 58 F8 70 ....JRQ......X.p
57A0 F6 16 42 F0 F7 AF EB C0 06 CE F0 F5 AF EB C0 06 ..B.............
57B0 CE 3A DE 08 52 1C 43 03 16 02 00 3A 00 00 52 1C .:..R.C....:..R.
57C0 9D 03 16 02 00 3A 02 00 FE 70 F6 1C 42 F2 1C 4A .....:...p..B..J
57D0 38 00 00 50 38 14 00 50 52 1C 53 03 16 06 00 FD 8..P8..PR.S.....
57E0 31 24 F9 66 CE 10 3A EC 08 52 1C 43 03 16 02 00 1$.f..:..R.C....
57F0 3A FF FF 1A DA 08 F2 1C 4A FD 31 FC 30 E0 2E FE :.......J.1.0...
5800 66 C6 10 50 56 96 1C 2D 03 96 58 24 FD 66 C6 03 f..PV..-..X$.f..
5810 90 C8 EA E0 2E 2A 50 51 96 1C 23 03 96 58 24 FD .....*PQ..#..X$.
5820 66 CE 0D 3A EE 08 52 1C 43 03 16 02 00 1A DB 07 f..:..R.C.......
5830 50 1C 37 03 16 02 00 FD 31 FC 30 F0 F6 2E F9 62 P.7.....1.0....b
5840 CE 20 F0 F7 2E F8 62 CE 19 3A 05 09 52 1C 43 03 . ....b..:..R.C.
5850 16 02 00 3D 04 80 E5 87 EB D7 08 CE F1 01 87 1A ...=............
5860 D7 08 3A 0D 09 52 1C 43 03 16 02 00 3D 06 80 E5 ..:..R.C....=...
5870 87 CE 03 F1 01 87 3A 01 00 FD 31 FC 30 5A 52 50 ......:...1.0ZRP
5880 52 F2 1E 4A 52 1C 14 09 16 06 00 F0 FF 2E FE 66 R..JR..........f
5890 CE 07 F0 FC 2E FE 66 C6 18 3A 00 00 FD 31 FC 30 ......f..:...1.0
58A0 5A 52 50 52 F2 1A 4A 52 1C 14 09 16 06 00 1A DB ZRPR..JR........
58B0 07 3A 00 00 FD 31 FC 30 5A 52 50 52 F2 1C 4A 52 .:...1.0ZRPR..JR
58C0 1C 14 09 16 06 00 1A DB 07 3A 00 00 FC 3E 5C 1E .........:...>\.
58D0 25 64 20 25 63 20 25 64 20 3D 20 20 20 00 0A 00 %d %c %d =   ...
58E0 50 6C 65 61 73 65 20 74 79 70 65 20 61 20 6E 75 Please type a nu
58F0 6D 62 65 72 2E 0A 00 52 69 67 68 74 21 0A 00 57 mber...Right!..W
5900 68 61 74 3F 0A 00 54 3C 00 00 FE 14 16 F8 FF F2 hat?..T<........
5910 0E 4A 52 1C 59 0B 16 02 00 52 3A 0C 00 52 1C BC .JR.Y....R:..R..
5920 02 16 02 00 58 EE 42 24 FD 66 EB BE 09 C6 F0 F8 ....X.B$.f......
5930 2E 68 04 F4 FA 26 F0 F9 2E 69 00 F4 FB 26 3A 0C .h...&...i...&:.
5940 80 F9 A4 F8 A2 F9 A4 F8 A2 F8 70 08 F2 10 4A 08 ..........p...J.
5950 FB A4 FA A2 F9 70 F6 06 42 F2 06 4A E2 2E F4 FC .....p..B..J....
5960 26 92 E2 2E F4 FD 26 F2 02 4A F0 FC 2E EA 26 92 &.....&..J....&.
5970 F0 FD 2E EA 26 F2 06 4A F0 F8 2E EA 26 92 F0 F9 ....&..J....&...
5980 2E EA 26 3A 08 80 F8 70 F9 70 52 5D 5A 52 92 92 ..&:...p.pR]ZR..
5990 EA 37 05 92 EA 37 00 E5 4A 38 05 00 F8 70 ED 25 .7...7..J8...p.%
59A0 F5 01 24 5A 52 F0 04 2E EA 26 92 F0 05 2E EA 26 ..$ZR....&.....&
59B0 FC 3E 5C 1E 54 3C 00 00 FE 14 16 F4 FF F2 12 4A .>\.T<.........J
59C0 52 1C 59 0B 16 02 00 52 1C D3 02 F4 FF 24 F4 FE R.Y....R.....$..
59D0 25 59 38 08 80 FB A4 FA A2 FB A4 FA A2 F4 FC 23 %Y8............#
59E0 F4 FD 22 F2 08 4A F8 70 08 F2 14 4A 08 FB A4 FA .."..J.p...J....
59F0 A2 F9 70 F6 06 42 F2 06 4A E2 29 92 E2 28 F2 10 ..p..B..J.)..(..
5A00 4A F8 70 50 51 52 F2 10 4A 52 1C FE 0B 16 04 00 J.pPQR..JR......
5A10 59 58 F6 0A 42 F0 FE 2E F0 04 62 F0 FF 2E F0 05 YX..B.....b.....
5A20 63 EB 35 0A CC 6D 80 EB 3F 0A CD F2 0A 4A 1A 2C c.5..m..?....J.,
5A30 0B F0 FE 2E F0 04 62 F4 FE 26 F0 FF 2E F0 05 63 ......b..&.....c
5A40 F4 FF 26 36 0C F0 FC 60 2D 36 80 F0 FD 61 2C F9 ..&6...`-6...a,.
5A50 70 08 FB 35 FA 34 E2 2E 92 E2 2C 2D F6 08 42 24 p..5.4....,-..B$
5A60 FD 66 EB 1B 0B C6 F2 08 4D 95 95 E5 2E F4 F8 26 .f......M......&
5A70 F1 01 2E F4 F9 26 F0 FC 2E 68 04 F4 F6 26 F0 FD .....&...h...&..
5A80 2E 69 00 F4 F7 26 F0 FE 2E F0 F8 62 F0 FF 2E F0 .i...&.....b....
5A90 F9 63 EB A6 0A CC 6D 80 EB 01 0B CD F2 08 4A E2 .c....m.......J.
5AA0 2E F4 F4 26 92 E2 2E F4 F5 26 98 F2 06 4A EA 21 ...&.....&...J.!
5AB0 92 EA 20 E5 4A 9A ED 25 F5 01 24 FE 65 FD 67 FC .. .J..%..$.e.g.
5AC0 63 EB D5 0A CC 6D 80 EB FD 0A C5 F2 02 4A E2 29 c....m.......J.)
5AD0 92 E2 28 FB 35 FA 34 E2 2E 92 E2 2C 2D 50 51 52 ..(.5.4....,-PQR
5AE0 1C C2 02 16 02 00 59 58 21 E9 26 91 20 E9 26 5A ......YX!.&. .&Z
5AF0 52 C8 2B F0 FE 2E F0 F8 62 F4 FE 26 F0 FF 2E F0 R.+.....b..&....
5B00 F9 63 F4 FF 26 08 F2 02 4A 08 1A 60 0A 3A 30 0B .c..&...J..`.:0.
5B10 52 3A 01 00 52 1C A8 02 16 04 00 3A 01 00 FC 3E R:..R......:...>
5B20 5C 1E 61 72 69 74 68 6D 65 74 69 63 3A 20 62 75 \.arithmetic: bu
5B30 67 3A 20 69 6E 63 6F 6E 73 69 73 74 65 6E 74 20 g: inconsistent 
5B40 70 65 6E 61 6C 74 69 65 73 2E 00 54 3C 00 00 FE penalties..T<...
5B50 14 38 00 00 F0 05 2E F0 04 66 C6 17 F2 04 4A 52 .8.......f....JR
5B60 E3 00 80 4A 52 1C B2 03 16 04 00 FD 31 24 28 FD ...JR.......1$(.
5B70 66 CE 10 50 3A A3 0B 52 3A 01 00 52 1C A8 02 16 f..P:..R:..R....
5B80 04 00 58 3A 00 80 21 E2 62 29 20 92 E2 63 28 F9 ..X:..!.b) ..c(.
5B90 35 F8 34 5C 1E 61 72 69 74 68 6D 65 74 69 63 3A 5.4\.arithmetic:
5BA0 20 62 75 67 3A 20 6F 70 20 25 63 20 6E 6F 74 20  bug: op %c not 
5BB0 69 6E 20 64 65 66 61 75 6C 74 6B 65 79 73 20 25 in defaultkeys %
5BC0 73 00 F2 02 48 F2 04 49 FE 65 2D F8 66 30 10 CE s...H..I.e-.f0..
5BD0 05 30 08 21 FA 70 F9 A2 A2 CF 02 F9 70 18 F5 1E .0.!.p......p...
5BE0 3A 03 00 FE 70 E2 2B 9A E2 2D 1C 1D 0C 1A 5B 0C :...p.+..-....[.
5BF0 F2 02 4A F2 04 49 1C 27 0C 1A 5B 0C F2 02 4A F2 ..J..I.'..[...J.
5C00 04 49 1A 27 0C 3A 03 00 FE 70 E2 2B 9A E2 2D 25 .I.'.:...p.+..-%
5C10 A0 FE 63 2C 23 A0 FE 63 2A 24 FA 65 A2 24 56 A2 ..c,#..c*$.e.$V.
5C20 CF 0A FE 62 FD 62 2D FE 63 FC 62 2C FA AF C6 0A ...b.b-.c.b,....
5C30 FE 62 FB 62 2B FE 63 FA 62 2A 1C 80 0C 5E FE DF .b.b+.c.b*...^..
5C40 28 FE 62 FD 62 2D FE 63 FC 62 2C 20 1E A2 08 FE (.b.b-.c.b, ....
5C50 DF FE 62 FD 62 2D FE 63 FC 62 2C 1E F2 02 4A F2 ..b.b-.c.b,...J.
5C60 04 49 C8 0E 3A 03 00 FE 70 E2 2B 9A E2 2D 34 00 .I..:...p.+..-4.
5C70 FC 32 23 6C 80 FA 66 CE 12 30 10 FA 71 A2 FB 62 .2#l..f..0..q..b
5C80 CF 02 FB 60 0E FA 71 18 F4 2B 1E 30 09 25 FC 35 ...`..q..+.0.%.5
5C90 34 00 FD A3 FA 71 F9 73 CF 02 F9 70 0E A2 18 F4 4....q.s...p....
5CA0 F8 A2 F8 32 2B 08 1E 0A 00 38 02 00 20 F9 66 C6 ...2+....8.. .f.
5CB0 08 39 10 80 3A B5 0C FE 59 1E                   .9..:...Y.     
