;;; Segment code (0100)

;; CpmCom_Start: 0100
;;   Called from:
;;     00FF (in fn01BF)
CpmCom_Start proc
	jp	015A
0103          5A 33 45 4E 56 01 00 00 00 00 55 4E 41    Z3ENV.....UNA
0110 52 43 5A 31 33 08 00 FF 00 00 FF 00 00 43 4F 4D RCZ13........COM
0120 45 58 45 4F 42 4A 4F 56 3F 52 45 4C 3F 52 4C 49 EXEOBJOV?REL?RLI
0130 4E 54 53 59 53 42 41 44 4C 42 52 41 52 43 41 52 NTSYSBADLBRARCAR
0140 4B 3F 51 3F 3F 5A 3F 3F 59 3F 43 4F 4D 43 4F 4D K?Q??Z??Y?COMCOM
0150 43 4F 4D 43 4F 4D 43 4F 4D 00                   COMCOMCOM.      

l015A:
	ld	(01CA),sp
	call	fn01FB
	ld	sp,1742
	ld	hl,1742
	ld	bc,1200
	call	fn0F2A
	call	fn0226
	call	fn0602
	ld	hl,0003
	ld	b,l

l0177:
	call	fn052E
	cp	a,1A
	jr	z,0187

l017E:
	djnz	0177

l0180:
	call	fn052E
	cp	a,1A
	jr	nz,01CD

l0187:
	call	fn052E
	or	a,a
	jr	z,01AE

l018D:
	call	fn05A3
	call	fn05C2
	jr	nz,01A0

l0195:
	call	fn0BFD
	call	fn06B0
	call	fn06A5
	jr	nz,fn01BF

l01A0:
	ld	hl,17A7
	call	fn0F13
	call	fn0563
	ld	hl,0000
	jr	0180

l01AE:
	ld	hl,(1742)
	ld	a,h
	or	a,a
	jr	nz,01BC

l01B5:
	or	a,l
	ld	de,13BE
	jr	z,fn021B

l01BB:
	dec	a

l01BC:
	call	nz,fn0C6C

;; fn01BF: 01BF
;;   Called from:
;;     019E (in CpmCom_Start)
;;     01BC (in CpmCom_Start)
;;     01BC (in CpmCom_Start)
;;     0224 (in fn0221)
;;     0224 (in fn0221)
;;     0BA8 (in fn0A8F)
fn01BF proc
	call	fn04B8
	ld	a,(0115)
	or	a,a
	jp	z,0000

l01C9:
	ld	sp,0000
	ret

l01CD:
	call	fn055D
	call	fn052E

l01D3:
	cp	a,1A
	jr	nz,01CD

l01D7:
	call	fn052E
	push	af
	dec	a
	cp	a,09
	jr	nc,01F5

l01E0:
	ex	de,hl
	ld	hl,13B8
	ld	bc,0000
	call	fn0EBB
	ld	(hl),00
	ld	de,1383
	call	fn0CDB
	pop	af
	jr	018D

l01F5:
	call	fn055D
	pop	af
	jr	01D3

;; fn01FB: 01FB
;;   Called from:
;;     015E (in CpmCom_Start)
fn01FB proc
	xor	a,a
	ld	(04BF),a
	ld	(04CB),a
	ld	(0CF1),a
	ld	(176A),a
	ld	a,(0007)
	ld	hl,0115
	sub	a,(hl)
	ld	(0215),a
	ld	a,17

;; fn0214: 0214
;;   Called from:
;;     0660 (in fn0602)
;;     06C9 (in fn06B0)
fn0214 proc
	cp	a,00
	ret	c

;; fn0217: 0217
;;   Called from:
;;     0216 (in fn0214)
;;     0216 (in fn01FB)
fn0217 proc
	ld	de,1315
	pop	hl

;; fn021B: 021B
;;   Called from:
;;     01B9 (in CpmCom_Start)
;;     021A (in fn0217)
;;     02E5 (in fn0226)
;;     033D (in fn0226)
;;     0359 (in fn0226)
;;     055A (in fn0557)
;;     0631 (in fn0602)
;;     06BE (in fn06B0)
;;     0736 (in fn06B0)
;;     078D (in fn06B0)
;;     0824 (in fn06B0)
;;     0AC6 (in fn0AB3)
;;     0BE5 (in fn0A8F)
fn021B proc
	call	fn0D12

;; fn021E: 021E
;;   Called from:
;;     021B (in fn021B)
;;     021B (in fn021B)
;;     0511 (in fn04F3)
fn021E proc
	ld	de,1309

;; fn0221: 0221
;;   Called from:
;;     021E (in fn021E)
;;     04A8 (in fn0226)
fn0221 proc
	call	fn0D15
	jr	fn01BF

;; fn0226: 0226
;;   Called from:
;;     016D (in CpmCom_Start)
fn0226 proc
	ld	hl,(0109)
	ld	de,0029
	add	hl,de
	ld	a,(hl)
	ld	(1754),a
	inc	hl
	ld	a,(hl)
	ld	(1755),a
	ld	hl,(0109)
	ld	de,0032
	add	hl,de
	ld	a,(hl)
	dec	a
	ld	(176A),a
	ld	(0CF1),a
	ld	(176B),a
	ld	a,(011C)
	or	a,a
	jr	z,0255

l024E:
	xor	a,a
	ld	(0CF1),a
	ld	(176B),a

l0255:
	ld	(1769),a
	ld	a,(011B)
	or	a,a
	jr	z,0263

l025E:
	ld	a,FF
	ld	(1769),a

l0263:
	ld	hl,0081
	ld	a,(0080)
	or	a,a
	jp	z,03D0

l026D:
	ld	c,a
	ld	b,00
	call	fn0EE5
	or	a,a
	jp	z,03D0

l0277:
	cp	a,2F
	jp	z,03D0

l027C:
	ld	a,20

l027E:
	cpir

l0280:
	jr	nz,02B7

l0282:
	call	fn0EE5
	cp	a,2F
	jr	z,029C

l0289:
	or	a,a
	jr	z,02B7

l028C:
	ld	a,20

l028E:
	cpir

l0290:
	jr	nz,02B7

l0292:
	call	fn0EE5
	or	a,a
	jr	z,02B7

l0298:
	cp	a,2F
	jr	nz,029E

l029C:
	inc	hl
	ld	a,(hl)

l029E:
	or	a,a
	jr	z,02B7

l02A1:
	cp	a,4E
	call	z,fn03A2

l02A6:
	cp	a,50
	call	z,fn03BD

l02AB:
	cp	a,43
	call	z,fn03C1

l02B0:
	cp	a,45
	call	z,fn03C5

l02B5:
	jr	029C

l02B7:
	ld	a,(0069)
	ld	(1765),a
	ld	hl,144A
	ld	b,02
	ld	c,00
	call	fn0EB8
	ld	a,3A
	ld	(hl),a
	inc	hl
	xor	a,a
	ld	(hl),a
	ld	a,(005C)
	or	a,a
	jr	nz,02D9

l02D3:
	ld	c,19
	call	0005
	inc	a

l02D9:
	add	a,40
	ld	(1449),a
	ld	a,(007B)
	or	a,a
	ld	de,1402
	jp	nz,fn021B

l02E8:
	ld	a,(0079)
	ld	(1766),a
	ld	hl,1471
	ld	b,02
	ld	c,00
	call	fn0EB8
	ld	a,3A
	ld	(hl),a
	inc	hl
	xor	a,a
	ld	(hl),a
	ld	hl,006C
	ld	de,1778
	ldi
	ld	de,176D
	ld	bc,000B
	ld	a,2F
	cp	a,(hl)
	ld	a,20
	jr	z,0316

l0313:
	cp	a,(hl)
	jr	nz,031C

l0316:
	ld	h,d
	ld	l,e
	ld	(hl),3F
	inc	de
	dec	bc

l031C:
	ldir
031E                                           21 65               !e
0320 00 BE 20 0B                                     .. .            

l0324:
	ld	(hl),41
	inc	hl
	ld	(hl),52
	inc	hl
	ld	(hl),4B
	ld	(1751),a

l032F:
	ld	hl,005D
	cp	a,(hl)
	jp	z,03D0

l0336:
	push	hl
	call	fn06A8
	ld	de,132A

l033D:
	jp	z,fn021B

l0340:
	pop	de
	ld	hl,144E
	ld	c,20
	call	fn0D4F
	xor	a,a
	ld	(hl),a
	dec	a
	ld	(1767),a
	ld	hl,005C
	ld	a,(006B)
	or	a,a
	ld	de,13E0
	jp	nz,fn021B

l035C:
	ex	de,hl
	call	fn0AC9
	ld	c,0F
	call	fn04E9
	jr	nz,0379

l0367:
	ld	hl,1751
	or	a,(hl)
	ld	de,1348
	jr	z,033D

l0370:
	ld	(hl),00
	ld	hl,0067
	ld	(hl),43
	jr	032F

l0379:
	ld	(04BF),a
	ld	de,1438
	call	fn0D15
	ld	de,144E
	call	fn0CDE
	ld	a,(0116)
	or	a,a
	call	z,fn04AB

l038F:
	jr	nz,039E

;; fn0391: 0391
;;   Called from:
;;     038F (in fn0226)
;;     038F (in fn0226)
;;     0652 (in fn0602)
fn0391 proc
	ld	c,1F
	call	0005
	inc	hl
	inc	hl
	inc	hl
	ld	a,(hl)
	inc	a
	rrca
	rrca
	rrca

l039E:
	ld	(176C),a
	ret

;; fn03A2: 03A2
;;   Called from:
;;     02A3 (in fn0226)
fn03A2 proc
	push	af
	ld	a,(0CF1)
	or	a,a
	jr	z,03B2

l03A9:
	xor	a,a
	ld	(0CF1),a
	ld	(176B),a
	pop	af
	ret

l03B2:
	ld	a,(176A)
	ld	(0CF1),a
	ld	(176B),a
	pop	af
	ret

;; fn03BD: 03BD
;;   Called from:
;;     02A8 (in fn0226)
fn03BD proc
	ld	(1752),a
	ret

;; fn03C1: 03C1
;;   Called from:
;;     02AD (in fn0226)
fn03C1 proc
	ld	(1753),a
	ret

;; fn03C5: 03C5
;;   Called from:
;;     02B2 (in fn0226)
fn03C5 proc
	push	af
	ld	a,(1769)
	xor	a,FF
	ld	(1769),a
	pop	af
	ret

l03D0:
	ld	de,0F48
	call	fn0D15
	call	fn0CB5
	ld	de,0F89
	call	fn0D15
	call	fn04AB
	push	af
	or	a,a
	ld	de,0F9F
	call	z,fn0D15

l03EA:
	ld	de,0FA6
	call	fn0D15
	pop	af
	push	af
	or	a,a
	ld	de,1013
	call	z,fn0D15

l03F9:
	ld	a,(0117)
	or	a,a
	jr	nz,0404

l03FF:
	pop	af
	push	af
	or	a,a
	jr	nz,040A

l0404:
	ld	de,103B
	call	fn0D15

l040A:
	ld	de,1067
	call	fn0D15
	ld	a,(011C)
	or	a,a
	jr	nz,041C

l0416:
	ld	de,1101
	call	fn0D15

l041C:
	ld	de,1079
	call	fn0D15
	pop	af
	push	af
	or	a,a
	jr	nz,043F

l0427:
	ld	de,108F
	call	fn0D15
	ld	a,(011B)
	or	a,a
	jr	z,0439

l0433:
	ld	de,1101
	call	fn0D15

l0439:
	ld	de,10BA
	call	fn0D15

l043F:
	ld	de,1108
	call	fn0D15
	call	fn0CB5
	ld	de,111A
	call	fn0D15
	call	fn0CB5
	ld	de,115B
	call	fn0D15
	ld	a,(0117)
	or	a,a
	jr	nz,0462

l045D:
	pop	af
	push	af
	or	a,a
	jr	nz,0471

l0462:
	ld	de,1194
	call	fn0D15
	call	fn0CB5
	ld	de,119B
	call	fn0D15

l0471:
	pop	af
	or	a,a
	jr	nz,04A5

l0475:
	ld	de,11D5
	call	fn0D15
	call	fn0CB5
	ld	de,11DC
	call	fn0D15
	call	fn0CB5
	ld	de,121A
	call	fn0D15
	ld	de,1252
	call	fn0D15
	call	fn0CB5
	ld	de,1259
	call	fn0D15
	call	fn0CB5
	ld	de,129A
	call	fn0D15

l04A5:
	ld	de,12D7
	jp	fn0221

;; fn04AB: 04AB
;;   Called from:
;;     038C (in fn0226)
;;     03DF (in fn0226)
;;     0605 (in fn0602)
;;     06DD (in fn06B0)
;;     0BDD (in fn0A8F)
fn04AB proc
	push	hl
	ld	hl,(1754)
	ld	a,(hl)
	pop	hl
	or	a,a
	jr	nz,04B6

l04B4:
	inc	a
	ret

l04B6:
	xor	a,a
	ret

;; fn04B8: 04B8
;;   Called from:
;;     01BF (in fn01BF)
fn04B8 proc
	ld	de,005C
	call	fn0AC9
	ld	a,00
	or	a,a
	call	nz,fn04DC

;; fn04C4: 04C4
;;   Called from:
;;     04C1 (in fn04B8)
;;     04C1 (in fn04B8)
;;     0781 (in fn06B0)
fn04C4 proc
	ld	de,1778
	call	fn0AD2
	ld	a,00
	or	a,a
	call	nz,fn04DC

l04D0:
	inc	a
	ret	z

l04D2:
	ld	a,(04CB)
	or	a,a
	call	nz,fn0B4F

l04D9:
	xor	a,a
	inc	a
	ret

;; fn04DC: 04DC
;;   Called from:
;;     04C1 (in fn04B8)
;;     04CD (in fn04C4)
fn04DC proc
	ld	c,10
	call	0005
	inc	a
	ret

;; fn04E3: 04E3
;;   Called from:
;;     0708 (in fn06B0)
;;     072B (in fn06B0)
;;     0730 (in fn06B0)
;;     0ABD (in fn0AB3)
fn04E3 proc
	call	fn0AD2
	ld	de,1778

;; fn04E9: 04E9
;;   Called from:
;;     0362 (in fn0226)
;;     04E6 (in fn04E3)
fn04E9 proc
	call	0005
	inc	a
	ret

;; fn04EE: 04EE
;;   Called from:
;;     0547 (in fn0540)
;;     0547 (in fn0542)
;;     0703 (in fn06B0)
;;     0AB8 (in fn0AB3)
;;     0B81 (in fn0B4F)
fn04EE proc
	ld	c,1A
	call	0005

;; fn04F3: 04F3
;;   Called from:
;;     04F0 (in fn04EE)
;;     0719 (in fn06B0)
;;     0BCD (in fn0A8F)
;;     0BF1 (in fn0A8F)
;;     0CFC (in fn0CE1)
fn04F3 proc
	ld	c,0B
	call	0005
	or	a,a
	ret	z

l04FA:
	ld	c,01
	call	0005
	add	a,7F
	cp	a,13
	ld	c,01
	call	z,0005

l0508:
	add	a,7F
	cp	a,03
	jr	z,0511

l050E:
	cp	a,0B
	ret	nz

l0511:
	jp	fn021E

;; fn0514: 0514
;;   Called from:
;;     07BF (in fn06B0)
fn0514 proc
	exx

;; fn0515: 0515
;;   Called from:
;;     0514 (in fn0514)
;;     074A (in fn06B0)
;;     0757 (in fn06B0)
;;     0798 (in fn06B0)
;;     079C (in fn06B0)
;;     07A7 (in fn06B0)
;;     081A (in fn06B0)
fn0515 proc
	push	bc
	push	de
	push	hl
	ld	hl,17A7
	ld	b,04

l051D:
	ld	a,(hl)
	dec	(hl)
	or	a,a
	jr	nz,fn0531

l0522:
	inc	hl
	djnz	051D

l0525:
	ld	b,04

l0527:
	dec	hl
	ld	(hl),a
	djnz	0527

l052B:
	scf
	jr	053C

;; fn052E: 052E
;;   Called from:
;;     0177 (in CpmCom_Start)
;;     0180 (in CpmCom_Start)
;;     0187 (in CpmCom_Start)
;;     01D0 (in CpmCom_Start)
;;     01D7 (in CpmCom_Start)
;;     05B1 (in fn05A3)
fn052E proc
	push	bc
	push	de
	push	hl

;; fn0531: 0531
;;   Called from:
;;     0520 (in fn0515)
;;     0530 (in fn052E)
fn0531 proc
	ld	hl,(1767)
	inc	l
	call	z,fn0540

;; fn0538: 0538
;;   Called from:
;;     0535 (in fn0531)
;;     0535 (in fn0531)
;;     0535 (in fn0531)
;;     0535 (in fn0531)
fn0538 proc
	ld	(1767),hl
	ld	a,(hl)

l053C:
	pop	hl
	pop	de
	pop	bc
	ret

;; fn0540: 0540
;;   Called from:
;;     0535 (in fn0531)
;;     0535 (in fn0531)
fn0540 proc
	ld	c,14

;; fn0542: 0542
;;   Called from:
;;     059B (in fn0563)
fn0542 proc
	ld	de,0080
	push	de
	push	bc
	call	fn04EE
	call	fn0AC9
	ld	de,005C
	pop	bc
	call	0005
	pop	hl
	or	a,a
	ret	z

;; fn0557: 0557
;;   Called from:
;;     0556 (in fn0542)
;;     0556 (in fn0540)
;;     0561 (in fn055D)
;;     0565 (in fn0563)
;;     056D (in fn0563)
;;     0594 (in fn0563)
fn0557 proc
	ld	de,1364
	jp	fn021B

;; fn055D: 055D
;;   Called from:
;;     01CD (in CpmCom_Start)
;;     01F5 (in CpmCom_Start)
fn055D proc
	inc	hl
	ld	a,h
	or	a,l
	ret	nz

l0561:
	jr	fn0557

;; fn0563: 0563
;;   Called from:
;;     01A6 (in CpmCom_Start)
fn0563 proc
	ld	a,b
	or	a,a
	jr	nz,fn0557

l0567:
	ld	a,e
	ld	l,d
	ld	h,c
	add	a,a
	adc	hl,hl
	jr	c,fn0557

l056F:
	rra
	ex	de,hl
	ld	hl,1767
	add	a,(hl)
	ld	(hl),a
	inc	a
	jp	p,0580

l057A:
	ld	a,d
	or	a,e
	ret	z

l057D:
	dec	de
	jr	0583

l0580:
	add	a,7F
	ld	(hl),a

l0583:
	push	de
	ld	de,005C
	ld	c,24
	call	fn0AC9
	call	0005
	ld	hl,(007D)
	pop	de
	add	hl,de
	jr	c,fn0557

l0596:
	ld	(007D),hl
	ld	c,21
	call	fn0542
	ld	hl,007C
	inc	(hl)
	ret

;; fn05A3: 05A3
;;   Called from:
;;     018D (in CpmCom_Start)
fn05A3 proc
	ld	de,1799
	ld	b,1C
	cp	a,01
	push	af
	jr	nz,05B4

l05AD:
	ld	b,18
	jr	05B4

l05B1:
	call	fn052E

l05B4:
	ld	(de),a
	inc	de
	djnz	05B1

l05B8:
	pop	af
	ret	nz

l05BA:
	ld	hl,17A7
	ld	c,04

l05BF:
	ldir
05C1    C9                                            .              

;; fn05C2: 05C2
;;   Called from:
;;     0190 (in CpmCom_Start)
fn05C2 proc
	ld	de,179A
	ld	hl,1779
	ld	ix,176D
	ld	b,0B

l05CE:
	ld	a,(de)
	add	a,7F
	jr	z,05EB

l05D3:
	inc	de
	cp	a,21
	jr	c,05DC

l05D8:
	cp	a,7F
	jr	nz,05DE

l05DC:
	ld	a,24

l05DE:
	call	fn0F2F
	cp	a,2E
	jr	nz,05ED

l05E5:
	ld	a,b
	cp	a,04
	jr	c,05CE

l05EA:
	dec	de

l05EB:
	ld	a,20

l05ED:
	ld	(hl),a
	ld	a,(ix)
	inc	ix
	cp	a,3F
	jr	z,05F9

l05F7:
	cp	a,(hl)
	ret	nz

l05F9:
	inc	hl
	djnz	05CE

l05FC:
	ld	bc,1500
	jp	fn0F2A

;; fn0602: 0602
;;   Called from:
;;     0170 (in CpmCom_Start)
fn0602 proc
	ld	hl,1753
	call	fn04AB
	dec	a
	jr	nz,0613

l060B:
	ld	b,a
	ld	(hl),a
	ld	(1752),a
	ld	a,(0117)

l0613:
	ld	c,a
	ld	a,(1778)
	or	a,a
	jr	nz,062A

l061A:
	or	a,(hl)
	jr	z,067F

l061D:
	ld	de,1475
	call	fn0CDE
	ld	a,FE
	ld	(1778),a
	jr	065C

l062A:
	dec	a
	ld	c,a
	ld	a,b
	or	a,a
	ld	de,141E
	jp	z,fn021B

l0634:
	xor	a,a
	ld	(0CF1),a
	ld	a,c
	push	bc
	add	a,41
	ld	(1470),a
	ld	de,145B
	call	fn0CDE
	ld	c,19
	call	0005
	pop	de
	cp	a,e
	push	af
	ld	c,0E
	call	nz,0005

l0652:
	call	fn0391
	pop	af
	ld	e,a
	ld	c,0E
	call	nz,0005

l065C:
	ld	hl,1900
	ld	a,h
	call	fn0214
	ld	de,A001

l0666:
	ld	a,l
	ld	bc,0800

l066A:
	srl	c
	rra
	jr	nc,0675

l066F:
	ex	af,af'
	ld	a,c
	xor	a,d
	ld	c,a
	ex	af,af'
	xor	a,e

l0675:
	djnz	066A

l0677:
	ld	(hl),c
	dec	h
	ld	(hl),a
	inc	h
	inc	l
	jr	nz,0666

l067E:
	ret

l067F:
	or	a,c
	call	nz,fn06A5

l0683:
	ret	z

l0684:
	ld	de,011D

l0687:
	ld	hl,1775
	ld	b,03

l068C:
	ld	a,(de)
	or	a,a
	jr	z,06A0

l0690:
	cp	a,3F
	jr	z,0695

l0694:
	cp	a,(hl)

l0695:
	inc	de
	jr	z,069C

l0698:
	djnz	0695

l069A:
	jr	0687

l069C:
	inc	hl
	djnz	068C

l069F:
	ret

l06A0:
	dec	a
	ld	(1778),a
	ret

;; fn06A5: 06A5
;;   Called from:
;;     019B (in CpmCom_Start)
;;     0680 (in fn0602)
fn06A5 proc
	ld	hl,176D

;; fn06A8: 06A8
;;   Called from:
;;     0337 (in fn0226)
;;     06A5 (in fn06A5)
fn06A8 proc
	ld	bc,000B
	ld	a,3F

l06AD:
	cpir

l06AF:
	ret

;; fn06B0: 06B0
;;   Called from:
;;     0198 (in CpmCom_Start)
fn06B0 proc
	ld	a,(1778)
	or	a,a
	ret	z

l06B5:
	ld	b,a
	ld	a,(1799)
	cp	a,0A
	ld	de,148B
	jp	nc,fn021B

l06C1:
	ld	l,a
	ld	h,00
	ld	de,0A33
	add	hl,de
	ld	a,(hl)
	call	fn0214
	ld	hl,17B5
	ld	(hl),a
	ld	c,a
	inc	hl
	ld	a,(0215)
	ld	(hl),a
	inc	b
	jr	nz,06FD

l06D9:
	ld	a,(0118)
	or	a,a
	call	z,fn04AB

l06E0:
	jr	z,06E9

l06E2:
	add	a,c
	jr	c,06E9

l06E5:
	cp	a,(hl)
	jr	nc,06E9

l06E8:
	ld	(hl),a

l06E9:
	ld	a,(1752)
	or	a,a
	jr	nz,073C

l06EF:
	ld	hl,16AD
	ld	bc,4E2D
	call	fn0F2A
	call	fn0C67
	jr	073C

l06FD:
	inc	b
	jr	z,073C

l0700:
	ld	de,1A00
	call	fn04EE
	ld	c,11
	call	fn04E3
	jr	z,072E

l070D:
	ld	a,(1769)
	or	a,a
	jr	nz,0729

l0713:
	ld	de,14C0
	call	fn0D15

l0719:
	call	fn04F3
	jr	z,0719

l071E:
	ld	e,a
	call	fn0CE1
	ld	a,e
	call	fn0F2F
	cp	a,59
	ret	nz

l0729:
	ld	c,13
	call	fn04E3

l072E:
	ld	c,16
	call	fn04E3
	ld	de,14F5
	jp	z,fn021B

l0739:
	ld	(04CB),a

l073C:
	ld	a,(1799)
	cp	a,04
	jr	nc,0790

l0743:
	call	fn0A2A
	cp	a,03
	jr	z,0757

l074A:
	call	fn0515
	jr	c,075C

l074F:
	call	fn0A56
	jr	074A

l0754:
	call	fn0A3D

l0757:
	call	fn0515
	jr	nc,0754

l075C:
	call	fn0A68
	ld	a,(1778)
	inc	a
	ret	z

l0764:
	or	a,a
	ex	de,hl
	ld	hl,(17AF)
	sbc	hl,de
	ld	de,158F
	call	nz,fn0D1E

l0771:
	ld	hl,17B1
	call	fn0F13
	ld	a,b
	or	a,c
	or	a,d
	or	a,e
	ld	de,1593
	call	nz,fn0D1E

l0781:
	call	fn04C4
	ld	hl,04CB
	ld	(hl),00
	ret	nz

l078A:
	ld	de,1507
	jp	fn021B

l0790:
	jr	nz,07F0

l0792:
	ld	bc,03FF
	call	fn07E6
	call	fn0515
	ld	c,a
	call	fn0515
	or	a,c
	jr	z,07B2

l07A2:
	ld	b,04
	ld	a,d
	sub	a,b
	ld	d,a

l07A7:
	call	fn0515
	ld	(de),a
	inc	d
	djnz	07A7

l07AE:
	inc	e
	dec	c
	jr	nz,07A2

l07B2:
	call	fn0A2A
	push	hl

l07B6:
	exx
	xor	a,a

l07B8:
	ld	l,a
	pop	af
	srl	a
	jr	nz,07C8

l07BE:
	push	hl
	call	fn0514
	exx
	jr	c,07E1

l07C5:
	pop	hl
	scf
	rra

l07C8:
	push	af
	ld	h,1A
	jr	nc,07CF

l07CD:
	inc	h
	inc	h

l07CF:
	ld	a,(hl)
	inc	h
	ld	b,(hl)
	inc	b
	jr	nz,07DC

l07D5:
	cpl
	exx
	call	fn0A3D
	jr	07B6

l07DC:
	djnz	07E1

l07DE:
	or	a,a
	jr	nz,07B8

l07E1:
	pop	hl

l07E2:
	exx
	jp	075C

;; fn07E6: 07E6
;;   Called from:
;;     0795 (in fn06B0)
;;     085D (in fn06B0)
fn07E6 proc
	ld	hl,1A00
	ld	(hl),l
	ld	d,h
	ld	e,l
	inc	de

l07ED:
	ldir
07EF                                              C9                .

l07F0:
	ld	hl,08D3
	ld	(hl),64
	cp	a,08
	jr	nc,080F

l07F9:
	ld	de,09FB
	ld	bc,4FFF
	ld	hl,0985
	cp	a,06
	ld	a,55
	jr	z,0845

l0808:
	jr	c,083F

l080A:
	ld	hl,09B0
	jr	0845

l080F:
	jr	z,081A

l0811:
	ld	(hl),6C
	ld	bc,5FFF
	ld	a,20
	jr	082C

l081A:
	call	fn0515
	jr	c,0827

l081F:
	cp	a,0C
	ld	de,1522
	jp	nz,fn021B

l0827:
	ld	bc,2FFF
	ld	a,10

l082C:
	ld	(08F9),a
	ld	hl,0000
	ld	(17B7),hl
	ld	de,090E
	ld	hl,08EC
	ld	a,09
	jr	z,0845

l083F:
	ld	ix,0A56
	jr	0849

l0845:
	ld	ix,0A3D

l0849:
	ld	(0A28),ix
	ld	(08DC),hl
	ld	(0876),de
	ld	(17B9),a
	ld	a,b
	sub	a,03
	ld	(0957),a
	call	fn07E6
	ld	(17BA),bc
	dec	bc
	push	bc
	xor	a,a

l0867:
	pop	bc
	push	bc
	push	af
	call	fn08CF
	pop	af
	inc	a
	jr	nz,0867

l0871:
	call	fn0A2A

l0874:
	exx

l0875:
	call	0000
	pop	bc
	jp	c,07E2

l087C:
	push	hl
	call	fn0905
	inc	b
	jr	nz,088A

l0883:
	inc	hl
	ld	a,(hl)

l0885:
	call	fn0A26
	jr	0874

l088A:
	dec	b
	ld	a,(hl)
	or	a,a
	push	af
	jr	nz,0895

l0890:
	ld	h,b
	ld	l,c
	call	fn0905

l0895:
	ld	d,01

l0897:
	ld	a,(hl)
	cp	a,1A
	jr	c,08A6

l089C:
	ld	(hl),d
	ld	d,a
	dec	hl
	ld	a,(hl)
	ld	(hl),e
	ld	e,a
	inc	hl
	ex	de,hl
	jr	0897

l08A6:
	inc	hl
	pop	af
	ld	a,(hl)
	push	af
	dec	hl
	push	de
	push	hl
	call	fn08CF
	pop	hl

l08B1:
	inc	hl
	ld	a,(hl)
	push	hl
	call	fn0A26
	exx
	pop	de
	pop	hl
	dec	h
	jr	z,08CA

l08BD:
	inc	h
	dec	de
	ld	a,(hl)
	ld	(hl),d
	ld	d,a
	dec	hl
	ld	a,(hl)
	ld	(hl),e
	ld	e,a
	inc	hl
	push	de
	jr	08B1

l08CA:
	pop	af
	jr	nz,0875

l08CD:
	jr	0885

;; fn08CF: 08CF
;;   Called from:
;;     086A (in fn06B0)
;;     08AD (in fn06B0)
fn08CF proc
	ld	hl,(17BA)
	bit	04,h
	ret	nz

l08D5:
	inc	hl
	ld	(17BA),hl
	push	af
	push	bc
	call	0000
	ex	(sp),hl
	call	fn0905
	ex	de,hl
	pop	hl
	dec	hl
	ld	(hl),e
	inc	hl
	ld	(hl),d
	inc	hl
	pop	af
	ld	(hl),a
	ret
08EC                                     7D 2D B7 20             }-. 
08F0 14 7C 25 11 BA 17 28 0A FE 10 28 09 A4 20 06 11 .|%...(...(.. ..
0900 B9 17 EB 34 EB                                  ...4.           

;; fn0905: 0905
;;   Called from:
;;     087D (in fn06B0)
;;     0892 (in fn06B0)
;;     08DF (in fn08CF)
fn0905 proc
	ld	d,h
	ld	e,l
	add	hl,hl
	add	hl,de
	ld	de,1A01
	add	hl,de
	ret
090E                                           21 B7               !.
0910 17 35 23 7E 23 46 21 FF 7F CB 3F 28 4D CB 1C CB .5#~#F!...?(M...
0920 1D 10 F6 CB 3C CB 1D 38 FA 32 B8 17 7C 3D B5 C0 ....<..8.2..|=..
0930 21 B9 17 4E 36 09 2B 77 2B 7E E6 07 28 14 47 AF !..N6.+w+~..(.G.
0940 77 81 10 FD 1F 1F 1F E6 0F 47 C5 CD 14 05 D9 C1 w........G......
0950 10 F8 21 00 1D 01 FF 2C CD E9 07 21 01 01 22 BA ..!....,...!..".
0960 17 E1 E3 21 FF FF E3 E5 18 A4 C5 E5 CD 14 05 D9 ...!............
0970 E1 C1 D8 CB 58 20 04 37 1F 18 A2 6C 67 78 D6 08 ....X .7...lgx..
0980 47 20 E7 18 9E 11 00 00 6F 62 09 CB DC CB 2C CB G ......ob....,.
0990 1D 4C 7D ED 5A 38 0F 06 0C CB 39 1F 30 03 EB 19 .L}.Z8....9.0...
09A0 EB 29 10 F5 EB 29 17 29 17 29 17 29 17 6C 18 17 .)...).).).).l..
09B0 6F 26 00 09 54 5D 29 29 19 29 29 19 29 19 29 19 o&..T])).)).).).
09C0 29 29 29 29 29 19 7C E6 0F 67 E5 CD 05 09 D1 7E ))))).|..g.....~
09D0 B7 C8 01 00 4A E5 E1 EB E5 29 09 5E 23 56 7A B3 ....J....).^#Vz.
09E0 20 F4 E3 1E 65 19 CB A4 E5 CD 05 09 D1 7E B7 28  ...e........~.(
09F0 04 EB 23 18 F1 E3 72 2B 73 E1 C9 CD 14 05 D9 D8 ..#...r+s.......
0A00 5F 21 B9 17 CB 0E 38 07 2B 7E E6 0F EB 67 C9 D5 _!....8.+~...g..
0A10 CD 14 05 D9 E1 D8 32 B8 17 E6 F0 17 67 ED 6A ED ......2.....g.j.
0A20 6A ED 6A ED 6A C9                               j.j.j.          

;; fn0A26: 0A26
;;   Called from:
;;     0885 (in fn06B0)
;;     08B4 (in fn06B0)
fn0A26 proc
	exx
	jp	0000

;; fn0A2A: 0A2A
;;   Called from:
;;     0743 (in fn06B0)
;;     07B2 (in fn06B0)
;;     0871 (in fn06B0)
fn0A2A proc
	ld	hl,(17B4)
	ld	l,00
	ex	de,hl
	ld	h,e
	ld	l,e
	ld	b,e
	ret
0A34             1A 1A 1A 1E 6A 6A 6A 4A 7A              ....jjjJz   

;; fn0A3D: 0A3D
;;   Called from:
;;     0754 (in fn06B0)
;;     07D7 (in fn06B0)
fn0A3D proc
	djnz	0A50

l0A3F:
	ld	b,a
	or	a,a
	jr	nz,0A4B

l0A43:
	ld	a,90
	jr	fn0A56

l0A47:
	ld	a,c
	call	fn0A56

l0A4B:
	djnz	0A47

l0A4D:
	ret

l0A4E:
	inc	b
	ret

l0A50:
	inc	b
	cp	a,90
	jr	z,0A4E

l0A55:
	ld	c,a

;; fn0A56: 0A56
;;   Called from:
;;     074F (in fn06B0)
;;     0A45 (in fn0A3D)
;;     0A48 (in fn0A3D)
;;     0A55 (in fn0A3D)
fn0A56 proc
	ld	(de),a
	xor	a,l
	ld	l,a
	ld	a,h
	ld	h,18
	xor	a,(hl)
	inc	h
	ld	h,(hl)
	ld	l,a
	inc	e
	ret	nz

l0A62:
	inc	d
	ld	a,(17B6)
	cp	a,d
	ret	nz

;; fn0A68: 0A68
;;   Called from:
;;     075C (in fn06B0)
;;     0A67 (in fn0A56)
fn0A68 proc
	push	hl
	ld	hl,(17B4)
	xor	a,a
	ld	l,a
	ex	de,hl
	sbc	hl,de
	jr	z,0A8D

l0A73:
	push	bc
	ld	b,h
	ld	c,l
	ld	hl,(17B1)
	sbc	hl,bc
	ld	(17B1),hl
	jr	nc,0A87

l0A80:
	ld	hl,(17B3)
	dec	hl
	ld	(17B3),hl

l0A87:
	push	de
	call	fn0A8F
	pop	de
	pop	bc

l0A8D:
	pop	hl
	ret

;; fn0A8F: 0A8F
;;   Called from:
;;     0A88 (in fn0A68)
fn0A8F proc
	ld	a,(04CB)
	or	a,a
	jp	z,0B9A

l0A96:
	ld	h,d
	ld	l,e
	add	hl,bc
	jr	0A9F

l0A9B:
	ld	(hl),1A
	inc	hl
	inc	bc

l0A9F:
	ld	a,l
	add	a,7F
	jr	nz,0A9B

l0AA4:
	or	a,b
	jr	z,0AB1

l0AA7:
	push	bc
	call	fn0AB3
	call	fn0AB3
	pop	bc
	djnz	0AA7

l0AB1:
	or	a,c
	ret	z

;; fn0AB3: 0AB3
;;   Called from:
;;     0AA8 (in fn0A8F)
;;     0AAB (in fn0A8F)
;;     0AB2 (in fn0A8F)
fn0AB3 proc
	ld	hl,0080
	add	hl,de
	push	hl
	call	fn04EE
	ld	c,15
	call	fn04E3
	pop	de
	dec	a
	ret	z

l0AC3:
	ld	de,14E8
	jp	fn021B

;; fn0AC9: 0AC9
;;   Called from:
;;     035D (in fn0226)
;;     04BB (in fn04B8)
;;     054A (in fn0540)
;;     054A (in fn0542)
;;     0589 (in fn0563)
fn0AC9 proc
	push	af
	push	bc
	push	de
	push	hl
	ld	a,(1765)
	jr	0AD9

;; fn0AD2: 0AD2
;;   Called from:
;;     04C7 (in fn04C4)
;;     04E3 (in fn04E3)
;;     0B84 (in fn0B4F)
fn0AD2 proc
	push	af
	push	bc
	push	de
	push	hl
	ld	a,(1766)

l0AD9:
	ld	e,a
	ld	c,20
	call	0005
	pop	hl
	pop	de
	pop	bc
	pop	af
	ret

;; fn0AE4: 0AE4
;;   Called from:
;;     0C59 (in fn0BFD)
fn0AE4 proc
	push	de
	ld	c,(hl)
	inc	hl
	ld	b,(hl)
	ld	a,b
	or	a,c
	jr	z,0B32

l0AEC:
	ld	a,b
	srl	a
	cp	a,62
	jr	nc,0B32

l0AF3:
	sub	a,14
	jr	nc,0AF9

l0AF7:
	add	a,64

l0AF9:
	ld	(de),a
	inc	de
	ld	a,c
	rr	b
	rla
	rla
	rla
	rla
	add	a,0F
	ld	(de),a
	inc	de
	ld	a,c
	add	a,1F
	ld	(de),a
	inc	de
	inc	hl
	inc	hl
	ld	b,(hl)
	dec	hl
	ld	c,(hl)
	ld	a,b
	rra
	rra
	rra
	add	a,1F
	ld	(de),a
	inc	de
	ld	a,b
	add	a,07
	rl	c
	rla
	rl	c
	rla
	rl	c
	rla
	ld	(de),a
	pop	de
	ld	b,05

l0B28:
	ld	a,(de)
	call	fn0B3C
	ld	(de),a
	inc	de
	djnz	0B28

l0B30:
	xor	a,a
	ret

l0B32:
	pop	de
	xor	a,a
	ld	b,05

l0B36:
	ld	(de),a
	inc	de
	djnz	0B36

l0B3A:
	dec	a
	ret

;; fn0B3C: 0B3C
;;   Called from:
;;     0B29 (in fn0AE4)
fn0B3C proc
	push	bc
	ld	b,FF

l0B3F:
	inc	b
	sub	a,0A
	jr	nc,0B3F

l0B44:
	add	a,0A
	ld	c,a
	ld	a,b
	add	a,a
	add	a,a
	add	a,a
	add	a,a
	add	a,c
	pop	bc
	ret

;; fn0B4F: 0B4F
;;   Called from:
;;     04D6 (in fn04C4)
fn0B4F proc
	ld	a,(17AB)
	or	a,a
	ret	z

l0B54:
	push	bc
	push	de
	push	hl
	ld	c,30
	call	0005
	ld	a,l
	or	a,a
	jr	z,0B96

l0B60:
	ld	a,h
	cp	a,53
	jr	z,0B69

l0B65:
	cp	a,44
	jr	nz,0B96

l0B69:
	ld	b,05
	ld	c,00
	ld	hl,175B
	call	fn0F2A
	ld	hl,1756
	ld	de,1760
	ld	bc,0005

l0B7C:
	ldir
0B7E                                           11 56               .V
0B80 17 CD EE 04 CD D2 0A 11 78 17 AF 32 84 17 32 98 ........x..2..2.
0B90 17 0E 67 CD 05 00                               ..g...          

l0B96:
	pop	hl
	pop	de
	pop	bc
	ret

l0B9A:
	ld	a,(1753)
	or	a,a
	ret	nz

l0B9F:
	ld	a,(1752)
	or	a,a
	jr	nz,0BE8

l0BA5:
	ld	a,(de)
	cp	a,1A
	jp	z,fn01BF

l0BAB:
	push	bc
	inc	a
	add	a,7F
	cp	a,21
	dec	a
	jr	c,0BBF

l0BB4:
	call	fn0CA3

l0BB7:
	inc	de
	pop	bc
	dec	bc
	ld	a,b
	or	a,c
	jr	nz,0BA5

l0BBE:
	ret

l0BBF:
	cp	a,09
	jr	z,0BB4

l0BC3:
	jr	c,0BB7

l0BC5:
	cp	a,0D
	jr	nc,0BB7

l0BC9:
	call	fn0CE1
	push	de
	call	fn04F3
	pop	de
	ld	hl,1750
	inc	(hl)
	jr	z,0BB7

l0BD7:
	ld	a,(0119)
	cp	a,(hl)
	jr	nz,0BB7

l0BDD:
	call	fn04AB
	jr	z,0BB7

l0BE2:
	ld	de,1546
	jp	fn021B

l0BE8:
	ex	de,hl

l0BE9:
	ld	e,(hl)
	push	hl
	push	bc
	ld	c,05
	call	0005
	call	fn04F3
	pop	bc
	pop	hl
	inc	hl
	dec	bc
	ld	a,b
	or	a,c
	jr	nz,0BE9

l0BFC:
	ret

;; fn0BFD: 0BFD
;;   Called from:
;;     0195 (in CpmCom_Start)
fn0BFD proc
	ld	hl,(1742)
	ld	a,h
	or	a,l
	inc	hl
	ld	(1742),hl
	call	z,fn0D28

l0C09:
	ld	de,17A7
	push	de
	ld	hl,174A
	call	fn0F1B
	ld	de,17B1
	push	de
	ld	hl,1744
	call	fn0F1B
	ld	hl,16AD
	ld	de,1779
	ld	c,00
	call	fn0D4F
	pop	de
	push	de
	call	fn0EA8
	call	fn0D62
	call	fn0D94
	pop	bc
	pop	de
	call	fn0DC0
	ld	a,(17AB)
	or	a,a
	jr	nz,0C51

l0C3E:
	ld	b,12
	call	fn0F28
	push	hl
	ld	hl,1756
	ld	b,0F
	ld	c,00
	call	fn0F2A
	pop	hl
	jr	0C64

l0C51:
	push	hl
	push	de
	ld	hl,17AB
	ld	de,1756
	call	fn0AE4
	pop	de
	pop	hl
	call	fn0E11
	call	fn0E47

l0C64:
	call	fn0E77

;; fn0C67: 0C67
;;   Called from:
;;     06F8 (in fn06B0)
fn0C67 proc
	ld	de,16AD
	jr	fn0C9F

;; fn0C6C: 0C6C
;;   Called from:
;;     01BC (in CpmCom_Start)
fn0C6C proc
	ld	hl,16AD
	ld	de,(1742)
	call	fn0E9C
	ld	de,1744
	push	de
	call	fn0EA8
	ld	de,(1748)
	call	fn0D8D
	ld	b,0D
	call	fn0F28
	pop	bc
	ld	de,174A
	call	fn0DC0
	ld	b,14
	call	fn0F28
	ld	de,(174E)
	call	fn0E87
	ld	de,1656

;; fn0C9F: 0C9F
;;   Called from:
;;     0C6A (in fn0C67)
;;     0C6A (in fn0BFD)
;;     0C9C (in fn0C6C)
fn0C9F proc
	ld	(hl),00
	jr	fn0CDE

;; fn0CA3: 0CA3
;;   Called from:
;;     0BB4 (in fn0A8F)
;;     0CE3 (in fn0CE1)
;;     0CE8 (in fn0CE1)
;;     0D33 (in fn0D28)
;;     0D49 (in fn0D28)
fn0CA3 proc
	cp	a,07
	jr	nz,fn0CAC

l0CA7:
	ld	hl,011A
	and	a,(hl)
	ret	z

;; fn0CAC: 0CAC
;;   Called from:
;;     0CA5 (in fn0CA3)
;;     0CAB (in fn0CA3)
;;     0CD5 (in fn0CB5)
fn0CAC proc
	push	de
	ld	e,a
	ld	c,02
	call	0005
	pop	de
	ret

;; fn0CB5: 0CB5
;;   Called from:
;;     03D6 (in fn0226)
;;     0445 (in fn0226)
;;     044E (in fn0226)
;;     0468 (in fn0226)
;;     047B (in fn0226)
;;     0484 (in fn0226)
;;     0493 (in fn0226)
;;     049C (in fn0226)
fn0CB5 proc
	ld	de,1300
	ld	a,(de)
	cp	a,20
	jr	nz,0CD1

l0CBD:
	push	de
	call	fn0F38
	jr	z,0CC6

l0CC3:
	inc	hl
	jr	0CC9

l0CC6:
	ld	hl,0F48

l0CC9:
	pop	de
	push	de
	ld	bc,0008

l0CCE:
	ldir
0CD0 D1                                              .               

l0CD1:
	ld	a,(de)
	cp	a,20
	ret	z

l0CD5:
	call	fn0CAC
	inc	de
	jr	0CD1

;; fn0CDB: 0CDB
;;   Called from:
;;     01EF (in CpmCom_Start)
fn0CDB proc
	call	fn0CE1

;; fn0CDE: 0CDE
;;   Called from:
;;     0385 (in fn0226)
;;     0620 (in fn0602)
;;     0642 (in fn0602)
;;     0CA1 (in fn0C9F)
;;     0CDB (in fn0CDB)
;;     0D26 (in fn0D1E)
fn0CDE proc
	call	fn0D15

;; fn0CE1: 0CE1
;;   Called from:
;;     071F (in fn06B0)
;;     0BC9 (in fn0A8F)
;;     0CDB (in fn0CDB)
;;     0CDE (in fn0CDE)
;;     0D12 (in fn0D12)
;;     0D3C (in fn0D28)
;;     0D41 (in fn0D28)
fn0CE1 proc
	ld	a,0D
	call	fn0CA3
	ld	a,0A
	call	fn0CA3
	ld	hl,176B
	dec	(hl)
	ret	nz

l0CF0:
	ld	a,00
	or	a,a
	ret	z

l0CF4:
	ld	(hl),a
	push	de
	ld	de,159A
	call	fn0D15

l0CFC:
	call	fn04F3
	jr	z,0CFC

l0D01:
	push	af
	ld	de,15A3
	call	fn0D15
	pop	af
	pop	de
	xor	a,20
	ret	nz

l0D0D:
	inc	a
	ld	(176B),a
	ret

;; fn0D12: 0D12
;;   Called from:
;;     021B (in fn021B)
;;     021B (in fn021B)
fn0D12 proc
	call	fn0CE1

;; fn0D15: 0D15
;;   Called from:
;;     0221 (in fn0221)
;;     0221 (in fn0221)
;;     037F (in fn0226)
;;     03D3 (in fn0226)
;;     03DC (in fn0226)
;;     03E7 (in fn0226)
;;     03ED (in fn0226)
;;     03F6 (in fn0226)
;;     0407 (in fn0226)
;;     040D (in fn0226)
;;     0419 (in fn0226)
;;     041F (in fn0226)
;;     042A (in fn0226)
;;     0436 (in fn0226)
;;     043C (in fn0226)
;;     0442 (in fn0226)
;;     044B (in fn0226)
;;     0454 (in fn0226)
;;     0465 (in fn0226)
;;     046E (in fn0226)
;;     0478 (in fn0226)
;;     0481 (in fn0226)
;;     048A (in fn0226)
;;     0490 (in fn0226)
;;     0499 (in fn0226)
;;     04A2 (in fn0226)
;;     0716 (in fn06B0)
;;     0CDE (in fn0CDE)
;;     0CF9 (in fn0CE1)
;;     0D05 (in fn0CE1)
;;     0D12 (in fn0D12)
;;     0D22 (in fn0D1E)
fn0D15 proc
	ld	a,(de)
	or	a,a
	ret	z

l0D18:
	illegal
	and	a,e
	inc	c
	inc	de
	jr	fn0D15

;; fn0D1E: 0D1E
;;   Called from:
;;     076E (in fn06B0)
;;     077E (in fn06B0)
fn0D1E proc
	push	de
	ld	de,1565
	call	fn0D15
	pop	de
	jr	fn0CDE

;; fn0D28: 0D28
;;   Called from:
;;     0C06 (in fn0BFD)
fn0D28 proc
	ld	de,1607
	push	de
	ld	a,(de)

l0D2D:
	cp	a,3D
	jr	nz,0D33

l0D31:
	ld	a,20

l0D33:
	call	fn0CA3
	inc	de
	ld	a,(de)
	or	a,a
	jr	nz,0D2D

l0D3B:
	pop	de
	call	fn0CE1

l0D3F:
	ld	a,(de)
	or	a,a
	jr	z,fn0CE1

l0D43:
	cp	a,20
	jr	z,0D49

l0D47:
	ld	a,3D

l0D49:
	call	fn0CA3
	inc	de
	jr	0D3F

;; fn0D4F: 0D4F
;;   Called from:
;;     0346 (in fn0226)
;;     0C25 (in fn0BFD)
fn0D4F proc
	ld	b,0C

l0D51:
	ld	a,b
	cp	a,04
	ld	a,2E
	jr	z,0D5D

l0D58:
	ld	a,(de)
	inc	de
	cp	a,c
	jr	z,0D5F

l0D5D:
	ld	(hl),a
	inc	hl

l0D5F:
	djnz	0D51

l0D61:
	ret

;; fn0D62: 0D62
;;   Called from:
;;     0C2D (in fn0BFD)
fn0D62 proc
	push	hl
	ld	hl,(17B1)
	ld	a,(17B3)
	ld	de,03FF
	add	hl,de
	adc	a,00
	rra
	rr	h
	rra
	rr	h
	add	a,3F
	ld	l,h
	ld	h,a
	ld	a,(176C)
	dec	a
	ld	e,a
	ld	d,00
	add	hl,de
	cpl
	and	a,l
	ld	e,a
	ld	d,h
	ld	hl,(1748)
	add	hl,de
	ld	(1748),hl
	pop	hl

;; fn0D8D: 0D8D
;;   Called from:
;;     0C81 (in fn0C6C)
fn0D8D proc
	call	fn0E9C
	ld	(hl),6B
	inc	hl
	ret

;; fn0D94: 0D94
;;   Called from:
;;     0C30 (in fn0BFD)
fn0D94 proc
	call	fn0F26
	ex	de,hl
	ld	hl,15D7
	ld	a,(1799)
	push	af
	ld	bc,0008
	cp	a,03
	jr	c,0DB7

l0DA6:
	add	hl,bc
	jr	z,0DB7

l0DA9:
	add	hl,bc
	cp	a,04
	jr	z,0DB7

l0DAE:
	add	hl,bc
	cp	a,09
	jr	c,0DB7

l0DB3:
	add	hl,bc
	jr	z,0DB7

l0DB6:
	add	hl,bc

l0DB7:
	ldir
0DB9                            EB F1                         ..     

;; fn0DBB: 0DBB
;;   Called from:
;;     0E67 (in fn0E47)
fn0DBB proc
	ld	b,03
	jp	fn0EA4

;; fn0DC0: 0DC0
;;   Called from:
;;     0C35 (in fn0BFD)
;;     0C8D (in fn0C6C)
fn0DC0 proc
	push	de
	push	bc
	call	fn0EA8
	pop	de
	ex	(sp),hl
	push	de
	call	fn0F13
	ld	h,b
	ld	l,c
	push	de
	pop	ix
	add	ix,ix
	adc	hl,hl
	add	ix,de
	adc	hl,bc
	add	ix,ix
	adc	hl,hl
	add	ix,ix
	adc	hl,hl
	add	ix,ix
	adc	hl,hl
	add	ix,de
	adc	hl,bc
	add	ix,ix
	adc	hl,hl
	add	ix,ix
	adc	hl,hl
	ex	(sp),hl
	call	fn0F13
	push	ix
	pop	hl
	ld	a,b
	or	a,c
	or	a,d
	or	a,e
	jr	z,0E08

l0DFD:
	ld	a,65

l0DFF:
	dec	a
	sbc	hl,de
	ex	(sp),hl
	sbc	hl,bc
	ex	(sp),hl
	jr	nc,0DFF

l0E08:
	pop	hl
	pop	hl
	call	fn0EA2
	ld	(hl),25
	inc	hl
	ret

;; fn0E11: 0E11
;;   Called from:
;;     0C5E (in fn0BFD)
fn0E11 proc
	ld	a,(17AB)
	add	a,1F
	call	fn0EA2
	ld	(hl),20
	inc	hl
	ex	de,hl
	ld	hl,(17AB)
	push	hl
	add	hl,hl
	add	hl,hl
	add	hl,hl
	ld	a,h
	add	a,0F
	cp	a,0D
	jr	c,0E2C

l0E2B:
	xor	a,a

l0E2C:
	ld	c,a
	ld	b,00
	ld	hl,15B0
	add	hl,bc
	add	hl,bc
	add	hl,bc
	ld	c,03

l0E37:
	ldir
0E39                            EB 36 20 23 F1 CB 3F          .6 #..?
0E40 C6 50                                           .P              

;; fn0E42: 0E42
;;   Called from:
;;     0E70 (in fn0E47)
fn0E42 proc
	ld	bc,0230
	jr	fn0EB8

;; fn0E47: 0E47
;;   Called from:
;;     0C61 (in fn0BFD)
fn0E47 proc
	ex	de,hl
	ld	hl,(17AD)
	ld	a,h
	rra
	rra
	rra
	add	a,1F
	ld	b,61
	jr	z,0E5F

l0E55:
	cp	a,0C
	jr	c,0E61

l0E59:
	ld	b,70
	sub	a,0C
	jr	nz,0E61

l0E5F:
	ld	a,0C

l0E61:
	push	bc
	add	hl,hl
	add	hl,hl
	add	hl,hl
	push	hl
	ex	de,hl
	call	fn0DBB
	ld	(hl),3A
	inc	hl
	pop	af
	add	a,3F
	call	fn0E42
	pop	af
	ld	(hl),a
	inc	hl
	ret

;; fn0E77: 0E77
;;   Called from:
;;     0C64 (in fn0BFD)
fn0E77 proc
	call	fn0F26
	ld	de,(17AF)
	push	hl
	ld	hl,(174E)
	add	hl,de
	ld	(174E),hl
	pop	hl

;; fn0E87: 0E87
;;   Called from:
;;     0C99 (in fn0C6C)
fn0E87 proc
	call	fn0E8B
	ld	d,e

;; fn0E8B: 0E8B
;;   Called from:
;;     0E87 (in fn0E77)
;;     0E87 (in fn0E87)
;;     0E8A (in fn0E77)
;;     0E8A (in fn0E87)
fn0E8B proc
	ld	(hl),d
	rld
	call	fn0E92
	ld	a,d

;; fn0E92: 0E92
;;   Called from:
;;     0E8E (in fn0E8B)
;;     0E91 (in fn0E8B)
fn0E92 proc
	or	a,F0
	daa
	cp	a,60
	sbc	a,1F
	ld	(hl),a
	inc	hl
	ret

;; fn0E9C: 0E9C
;;   Called from:
;;     0C73 (in fn0C6C)
;;     0D8D (in fn0D62)
;;     0D8D (in fn0D8D)
fn0E9C proc
	ld	b,05
	ld	c,20
	jr	fn0EBB

;; fn0EA2: 0EA2
;;   Called from:
;;     0E0A (in fn0DC0)
;;     0E16 (in fn0E11)
fn0EA2 proc
	ld	b,04

;; fn0EA4: 0EA4
;;   Called from:
;;     0DBD (in fn0D94)
;;     0DBD (in fn0DBB)
;;     0EA2 (in fn0EA2)
fn0EA4 proc
	ld	c,20
	jr	fn0EB8

;; fn0EA8: 0EA8
;;   Called from:
;;     0C2A (in fn0BFD)
;;     0C7A (in fn0C6C)
;;     0DC2 (in fn0DC0)
fn0EA8 proc
	ld	bc,0920
	push	de
	exx
	pop	hl
	ld	e,(hl)
	inc	hl
	ld	d,(hl)
	inc	hl
	ld	a,(hl)
	inc	hl
	ld	h,(hl)
	ex	de,hl
	jr	fn0EC0

;; fn0EB8: 0EB8
;;   Called from:
;;     02C4 (in fn0226)
;;     02F5 (in fn0226)
;;     0E45 (in fn0E11)
;;     0E45 (in fn0E42)
;;     0EA6 (in fn0EA4)
fn0EB8 proc
	ld	e,a
	ld	d,00

;; fn0EBB: 0EBB
;;   Called from:
;;     01E7 (in CpmCom_Start)
;;     0EA0 (in fn0E9C)
;;     0EB9 (in fn0EB8)
fn0EBB proc
	push	de
	exx
	pop	hl
	xor	a,a
	ld	d,a

;; fn0EC0: 0EC0
;;   Called from:
;;     0EB6 (in fn0EA8)
;;     0EBF (in fn0EBB)
;;     0EBF (in fn0EBB)
fn0EC0 proc
	ld	e,a
	ld	c,0A
	scf
	push	af

;; fn0EC5: 0EC5
;;   Called from:
;;     0EC4 (in fn0EC0)
;;     0EC4 (in fn0EC0)
fn0EC5 proc
	call	fn0EEF
	or	a,30
	exx
	djnz	0ED3

l0ECD:
	ld	(hl),a
	inc	hl

l0ECF:
	pop	af
	jr	nc,0ECD

l0ED2:
	ret

l0ED3:
	push	af
	exx
	ld	a,h
	or	a,l
	or	a,d
	or	a,e
	jr	nz,fn0EC5

l0EDB:
	exx
	or	a,c
	jr	z,0ECF

l0EDF:
	ld	(hl),a
	inc	hl
	djnz	0EDF

l0EE3:
	jr	0ECF

;; fn0EE5: 0EE5
;;   Called from:
;;     0270 (in fn0226)
;;     0282 (in fn0226)
;;     0292 (in fn0226)
fn0EE5 proc
	ld	a,(hl)
	inc	hl
	dec	bc
	cp	a,20
	jr	z,fn0EE5

l0EEC:
	dec	hl
	inc	bc
	ret

;; fn0EEF: 0EEF
;;   Called from:
;;     0EC5 (in fn0EC5)
fn0EEF proc
	ld	a,d
	or	a,e
	jr	z,fn0EFB

l0EF3:
	xor	a,a
	call	fn0EF7

;; fn0EF7: 0EF7
;;   Called from:
;;     0EF4 (in fn0EEF)
;;     0EF4 (in fn0EEF)
fn0EF7 proc
	ex	de,hl
	or	a,a
	jr	nz,fn0F00

;; fn0EFB: 0EFB
;;   Called from:
;;     0EF1 (in fn0EEF)
;;     0EF9 (in fn0EF7)
fn0EFB proc
	ld	a,h
	cp	a,c
	jr	c,0F04

l0EFF:
	xor	a,a

;; fn0F00: 0F00
;;   Called from:
;;     0EF9 (in fn0EF7)
;;     0EFF (in fn0EFB)
fn0F00 proc
	ld	b,10
	jr	fn0F09

l0F04:
	ld	h,l
	ld	l,00
	ld	b,08

;; fn0F09: 0F09
;;   Called from:
;;     0F02 (in fn0F00)
;;     0F07 (in fn0EFB)
fn0F09 proc
	add	hl,hl
	rla
	cp	a,c
	jr	c,0F10

l0F0E:
	sub	a,c
	inc	l

l0F10:
	djnz	fn0F09

l0F12:
	ret

;; fn0F13: 0F13
;;   Called from:
;;     01A3 (in CpmCom_Start)
;;     0774 (in fn06B0)
;;     0DC8 (in fn0DC0)
;;     0DF1 (in fn0DC0)
fn0F13 proc
	ld	e,(hl)
	inc	hl
	ld	d,(hl)
	inc	hl
	ld	c,(hl)
	inc	hl
	ld	b,(hl)
	ret

;; fn0F1B: 0F1B
;;   Called from:
;;     0C10 (in fn0BFD)
;;     0C1A (in fn0BFD)
fn0F1B proc
	ld	b,04
	or	a,a

l0F1E:
	ld	a,(de)
	adc	a,(hl)
	ld	(hl),a
	inc	hl
	inc	de
	djnz	0F1E

l0F25:
	ret

;; fn0F26: 0F26
;;   Called from:
;;     0D94 (in fn0D94)
;;     0E77 (in fn0E77)
fn0F26 proc
	ld	b,02

;; fn0F28: 0F28
;;   Called from:
;;     0C40 (in fn0BFD)
;;     0C86 (in fn0C6C)
;;     0C92 (in fn0C6C)
fn0F28 proc
	ld	c,20

;; fn0F2A: 0F2A
;;   Called from:
;;     016A (in CpmCom_Start)
;;     05FF (in fn05C2)
;;     06F5 (in fn06B0)
;;     0B70 (in fn0B4F)
;;     0C4B (in fn0BFD)
;;     0F28 (in fn0F26)
;;     0F28 (in fn0F28)
fn0F2A proc
	ld	(hl),c
	inc	hl
	djnz	fn0F2A

l0F2E:
	ret

;; fn0F2F: 0F2F
;;   Called from:
;;     05DE (in fn05C2)
;;     0723 (in fn06B0)
fn0F2F proc
	cp	a,61
	ret	c

l0F32:
	cp	a,7B
	ret	nc

l0F35:
	add	a,E0
	ret

;; fn0F38: 0F38
;;   Called from:
;;     0CBE (in fn0CB5)
fn0F38 proc
	push	de
	ld	hl,(0109)
	ld	de,0024
	add	hl,de
	ld	a,(hl)
	inc	hl
	ld	h,(hl)
	ld	l,a
	pop	de
	ld	a,h
	or	a,l
	ret
0F48                         55 4E 41 52 43 5A 20 20         UNARCZ  
0F50 20 5A 43 50 52 33 20 41 72 63 68 69 76 65 20 46  ZCPR3 Archive F
0F60 69 6C 65 20 45 78 74 72 61 63 74 6F 72 20 20 20 ile Extractor   
0F70 56 65 72 73 69 6F 6E 20 31 2E 33 0D 0A 55 73 61 Version 1.3..Usa
0F80 67 65 3A 0D 0A 20 20 20 00 20 7B 64 69 72 3A 7D ge:..   . {dir:}
0F90 61 72 63 66 69 6C 65 7B 2E 74 79 70 7D 20 00 7B arcfile{.typ} .{
0FA0 64 69 72 3A 7D 00 7B 61 66 6E 2E 61 66 74 7D 20 dir:}.{afn.aft} 
0FB0 7B 7B 2F 7D 6F 70 74 69 6F 6E 73 7D 0D 0A 41 6E {{/}options}..An
0FC0 20 61 6D 62 69 67 75 6F 75 73 20 6F 75 74 70 75  ambiguous outpu
0FD0 74 20 66 69 6C 65 6E 61 6D 65 20 28 6F 72 20 6E t filename (or n
0FE0 6F 20 66 69 6C 65 6E 61 6D 65 29 0D 0A 20 69 6D o filename).. im
0FF0 70 6C 69 65 73 20 61 72 63 68 69 76 65 20 64 69 plies archive di
1000 72 65 63 74 6F 72 79 20 64 69 73 70 6C 61 79 2E rectory display.
1010 0D 0A 00 41 6E 20 6F 75 74 70 75 74 20 44 55 20 ...An output DU 
1020 69 6D 70 6C 69 65 73 20 66 69 6C 65 20 65 78 74 implies file ext
1030 72 61 63 74 69 6F 6E 2E 0D 0A 00 41 6E 20 75 6E raction....An un
1040 61 6D 62 69 67 75 6F 75 73 20 66 69 6C 65 6E 61 ambiguous filena
1050 6D 65 20 69 6D 70 6C 69 65 73 20 74 79 70 65 2D me implies type-
1060 6F 75 74 2E 0D 0A 00 4F 70 74 69 6F 6E 73 3A 0D out....Options:.
1070 0A 20 20 20 4E 20 20 20 00 50 61 67 65 20 73 63 .   N   .Page sc
1080 72 65 65 6E 20 6F 75 74 70 75 74 2E 0D 0A 00 20 reen output.... 
1090 20 20 43 20 20 20 43 68 65 63 6B 20 76 61 6C 69   C   Check vali
10A0 64 69 74 79 20 6F 66 20 61 72 63 68 69 76 65 2E dity of archive.
10B0 0D 0A 20 20 20 45 20 20 20 00 45 72 61 73 65 20 ..   E   .Erase 
10C0 65 78 69 73 74 69 6E 67 20 66 69 6C 65 73 20 77 existing files w
10D0 69 74 68 6F 75 74 20 61 73 6B 69 6E 67 2E 0D 0A ithout asking...
10E0 20 20 20 50 20 20 20 53 65 6E 64 20 6F 75 74 70    P   Send outp
10F0 75 74 20 74 6F 20 70 72 69 6E 74 65 72 2E 0D 0A ut to printer...
1100 00 44 6F 6E 27 74 20 00 45 78 61 6D 70 6C 65 73 .Don't .Examples
1110 3A 0D 0A 20 20 20 42 30 3E 00 20 41 33 3A 53 41 :..   B0>. A3:SA
1120 56 45 2E 41 52 4B 20 2A 2E 2A 20 20 3B 20 6C 69 VE.ARK *.*  ; li
1130 73 74 20 61 6C 6C 20 66 69 6C 65 73 20 69 6E 20 st all files in 
1140 61 72 63 68 69 76 65 20 53 41 56 45 20 69 6E 20 archive SAVE in 
1150 41 33 0D 0A 20 20 20 41 30 3E 00 20 53 41 56 45 A3..   A0>. SAVE
1160 20 2A 2E 44 4F 43 20 4E 20 20 20 20 20 3B 20 6C  *.DOC N     ; l
1170 69 73 74 20 6A 75 73 74 20 44 4F 43 20 66 69 6C ist just DOC fil
1180 65 73 20 77 69 74 68 20 6E 6F 20 70 61 75 73 65 es with no pause
1190 73 0D 0A 00 20 20 20 41 30 3E 00 20 53 41 56 45 s...   A0>. SAVE
11A0 20 52 45 41 44 2E 4D 45 20 20 20 20 20 3B 20 74  READ.ME     ; t
11B0 79 70 65 20 74 68 65 20 66 69 6C 65 20 52 45 41 ype the file REA
11C0 44 2E 4D 45 20 74 6F 20 74 68 65 20 73 63 72 65 D.ME to the scre
11D0 65 6E 0D 0A 00 20 20 20 41 30 3E 00 20 53 41 56 en...   A0>. SAV
11E0 45 20 43 33 3A 20 20 20 20 20 20 20 20 20 3B 20 E C3:         ; 
11F0 65 78 74 72 61 63 74 20 61 6C 6C 20 66 69 6C 65 extract all file
1200 73 20 74 6F 20 64 69 72 65 63 74 6F 72 79 20 43 s to directory C
1210 33 0D 0A 20 20 20 41 31 3E 00 20 53 41 56 45 20 3..   A1>. SAVE 
1220 42 3A 2A 2E 44 4F 43 20 20 20 20 20 3B 20 65 78 B:*.DOC     ; ex
1230 74 72 61 63 74 20 44 4F 43 20 66 69 6C 65 73 20 tract DOC files 
1240 74 6F 20 64 69 72 65 63 74 6F 72 79 20 42 31 0D to directory B1.
1250 0A 00 20 20 20 41 30 3E 00 20 53 41 56 45 20 50 ..   A0>. SAVE P
1260 52 4E 2E 44 41 54 20 50 20 20 20 3B 20 73 65 6E RN.DAT P   ; sen
1270 64 20 74 68 65 20 66 69 6C 65 20 50 52 4E 2E 44 d the file PRN.D
1280 41 54 20 74 6F 20 74 68 65 20 70 72 69 6E 74 65 AT to the printe
1290 72 0D 0A 20 20 20 41 30 3E 00 20 53 41 56 45 20 r..   A0>. SAVE 
12A0 2A 2E 2A 20 43 20 20 20 20 20 20 20 3B 20 63 68 *.* C       ; ch
12B0 65 63 6B 20 76 61 6C 69 64 69 74 79 20 6F 66 20 eck validity of 
12C0 61 6C 6C 20 66 69 6C 65 73 20 69 6E 20 61 72 63 all files in arc
12D0 68 69 76 65 0D 0A 00 43 6F 70 79 72 69 67 68 74 hive...Copyright
12E0 20 31 39 38 36 2C 20 31 39 38 37 20 62 79 20 52  1986, 1987 by R
12F0 6F 62 65 72 74 20 41 2E 20 46 72 65 65 64 2E 00 obert A. Freed..
1300 20 20 20 20 20 20 20 20 20 07 20 20 41 62 6F 72          .  Abor
1310 74 65 64 21 00 20 20 4E 6F 74 20 65 6E 6F 75 67 ted!.  Not enoug
1320 68 20 6D 65 6D 6F 72 79 2C 00 20 20 41 6D 62 69 h memory,.  Ambi
1330 67 75 6F 75 73 20 61 72 63 68 69 76 65 20 66 69 guous archive fi
1340 6C 65 6E 61 6D 65 2C 00 20 20 43 61 6E 6E 6F 74 lename,.  Cannot
1350 20 66 69 6E 64 20 61 72 63 68 69 76 65 20 66 69  find archive fi
1360 6C 65 2C 00 20 20 49 6E 76 61 6C 69 64 20 61 72 le,.  Invalid ar
1370 63 68 69 76 65 20 66 69 6C 65 20 66 6F 72 6D 61 chive file forma
1380 74 2C 00 07 20 20 57 61 72 6E 69 6E 67 3A 20 42 t,..  Warning: B
1390 61 64 20 61 72 63 68 69 76 65 20 66 69 6C 65 20 ad archive file 
13A0 68 65 61 64 65 72 2C 20 62 79 74 65 73 20 73 6B header, bytes sk
13B0 69 70 70 65 64 20 3D 20 30 30 30 30 30 00 20 20 ipped = 00000.  
13C0 4E 6F 20 6D 61 74 63 68 69 6E 67 20 66 69 6C 65 No matching file
13D0 28 73 29 20 69 6E 20 61 72 63 68 69 76 65 2C 00 (s) in archive,.
13E0 20 20 49 6E 76 61 6C 69 64 20 61 72 63 68 69 76   Invalid archiv
13F0 65 20 66 69 6C 65 20 64 69 72 65 63 74 6F 72 79 e file directory
1400 2C 00 20 20 49 6E 76 61 6C 69 64 20 6F 75 74 70 ,.  Invalid outp
1410 75 74 20 64 69 72 65 63 74 6F 72 79 2C 00 20 20 ut directory,.  
1420 4E 6F 20 64 69 73 6B 20 6F 75 74 70 75 74 20 61 No disk output a
1430 6C 6C 6F 77 65 64 2C 00 20 20 41 72 63 68 69 76 llowed,.  Archiv
1440 65 20 46 69 6C 65 20 3D 20 41 30 30 3A 00 46 49 e File = A00:.FI
1450 4C 45 4E 41 4D 45 2E 41 52 43 00 20 20 4F 75 74 LENAME.ARC.  Out
1460 70 75 74 20 44 69 72 65 63 74 6F 72 79 20 3D 20 put Directory = 
1470 41 30 30 3A 00 20 20 43 68 65 63 6B 69 6E 67 20 A00:.  Checking 
1480 61 72 63 68 69 76 65 2E 2E 2E 00 20 20 43 61 6E archive....  Can
1490 6E 6F 74 20 65 78 74 72 61 63 74 20 66 69 6C 65 not extract file
14A0 20 28 6E 65 65 64 20 6E 65 77 65 72 20 76 65 72  (need newer ver
14B0 73 69 6F 6E 20 6F 66 20 55 4E 41 52 43 3F 29 00 sion of UNARC?).
14C0 07 20 20 52 65 70 6C 61 63 65 20 65 78 69 73 74 .  Replace exist
14D0 69 6E 67 20 6F 75 74 70 75 74 20 66 69 6C 65 20 ing output file 
14E0 28 79 2F 6E 29 3F 20 00 20 20 44 69 73 6B 20 66 (y/n)? .  Disk f
14F0 75 6C 6C 2C 00 20 20 44 69 72 65 63 74 6F 72 79 ull,.  Directory
1500 20 66 75 6C 6C 2C 00 20 20 43 61 6E 6E 6F 74 20  full,.  Cannot 
1510 63 6C 6F 73 65 20 6F 75 74 70 75 74 20 66 69 6C close output fil
1520 65 00 20 20 49 6E 63 6F 6D 70 61 74 69 62 6C 65 e.  Incompatible
1530 20 63 72 75 6E 63 68 65 64 20 66 69 6C 65 20 66  crunched file f
1540 6F 72 6D 61 74 00 20 20 54 79 70 65 6F 75 74 20 ormat.  Typeout 
1550 6C 69 6E 65 20 6C 69 6D 69 74 20 65 78 63 65 65 line limit excee
1560 64 65 64 2C 00 07 20 20 57 61 72 6E 69 6E 67 3A ded,..  Warning:
1570 20 45 78 74 72 61 63 74 65 64 20 66 69 6C 65 20  Extracted file 
1580 68 61 73 20 69 6E 63 6F 72 72 65 63 74 20 00 43 has incorrect .C
1590 52 43 00 6C 65 6E 67 74 68 00 20 20 5B 6D 6F 72 RC.length.  [mor
15A0 65 5D 00 0D 20 20 20 20 20 20 20 20 20 09 0D 00 e]..         ...
15B0 3F 3F 3F 4A 61 6E 46 65 62 4D 61 72 41 70 72 4D ???JanFebMarAprM
15C0 61 79 4A 75 6E 4A 75 6C 41 75 67 53 65 70 4F 63 ayJunJulAugSepOc
15D0 74 4E 6F 76 44 65 63 55 6E 70 61 63 6B 65 64 20 tNovDecUnpacked 
15E0 50 61 63 6B 65 64 20 53 71 75 65 65 7A 65 64 43 Packed SqueezedC
15F0 72 75 6E 63 68 65 64 53 71 75 61 73 68 65 64 55 runchedSquashedU
1600 6E 6B 6E 6F 77 6E 21 4E 61 6D 65 3D 3D 3D 3D 3D nknown!Name=====
1610 3D 3D 3D 20 20 3D 4C 65 6E 67 74 68 20 20 44 69 ===  =Length  Di
1620 73 6B 20 20 3D 4D 65 74 68 6F 64 3D 20 56 65 72 sk  =Method= Ver
1630 20 3D 53 74 6F 72 65 64 20 53 61 76 65 64 20 3D  =Stored Saved =
1640 3D 3D 44 61 74 65 3D 3D 20 3D 54 69 6D 65 3D 20 ==Date== =Time= 
1650 20 43 52 43 3D 00 20 20 20 20 20 20 20 20 3D 3D  CRC=.        ==
1660 3D 3D 20 20 3D 3D 3D 3D 3D 3D 3D 20 20 3D 3D 3D ==  =======  ===
1670 3D 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 =               
1680 3D 3D 3D 3D 3D 3D 3D 20 20 3D 3D 3D 20 20 20 20 =======  ===    
1690 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20                 
16A0 3D 3D 3D 3D 0D 0A 54 6F 74 61 6C 20 20 00 00 00 ====..Total  ...
16B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
