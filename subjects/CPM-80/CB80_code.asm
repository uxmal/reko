;;; Segment code (0100)
0100 C3 E1 0F 00 5A                                  ....Z           

l0105:
	jp	1729
0108                         00 17 2D 2D 2D 2D 2D 2D         ..------
0110 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D ----------------
0120 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D ----------------
0130 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 0D 0A 43 42 41 -----------..CBA
0140 53 49 43 20 43 6F 6D 70 69 6C 65 72 20 43 42 2D SIC Compiler CB-
0150 38 30 20 20 20 20 20 20 32 31 20 4D 61 79 20 38 80      21 May 8
0160 33 20 20 56 65 72 73 69 6F 6E 20 32 2E 30 0D 0A 3  Version 2.0..
0170 53 65 72 69 61 6C 20 4E 6F 2E 20 20 41 43 42 2D Serial No.  ACB-
0180 30 30 30 30 2D 30 30 30 30 37 32 20 20 20 41 6C 0000-000072   Al
0190 6C 20 72 69 67 68 74 73 20 72 65 73 65 72 76 65 l rights reserve
01A0 64 0D 0A 43 6F 70 79 72 69 67 68 74 20 28 63 29 d..Copyright (c)
01B0 20 31 39 38 32 2C 20 31 39 38 33 20 20 20 44 69  1982, 1983   Di
01C0 67 69 74 61 6C 20 52 65 73 65 61 72 63 68 2C 20 gital Research, 
01D0 49 6E 63 2E 0D 0A 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D Inc...----------
01E0 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D ----------------
01F0 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D 2D ----------------
0200 2D 2D 2D 2D 2D 2D 2D 0D 0A 24                   -------..$      

l020A:
	jp	172D
020D                                        00 00 00              ...
0210 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0220 00 C3 05 00                                     ....            

l0224:
	jp	177D
0227                      10 27 E8 03 64 00 0A 00 01        .'..d....
0230 00 04 43 42 38 30 63 6F 75 6C 64 20 6E 6F 74 20 ..CB80could not 
0240 6F 70 65 6E 20 66 69 6C 65 3A 20 24 69 6E 76 61 open file: $inva
0250 6C 69 64 20 66 69 6C 65 20 6E 61 6D 65 3A 20 24 lid file name: $
0260 66 61 74 61 6C 20 63 6F 6D 70 69 6C 65 72 20 65 fatal compiler e
0270 72 72 6F 72 20 24 64 69 73 6B 20 72 65 61 64 20 rror $disk read 
0280 65 72 72 6F 72 20 24 63 72 65 61 74 65 20 65 72 error $create er
0290 72 6F 72 3A 20 24 65 6E 64 20 6F 66 20 70 61 73 ror: $end of pas
02A0 73 20 24 64 69 73 6B 20 66 75 6C 6C 20 6F 6E 20 s $disk full on 
02B0 64 72 69 76 65 20 24 43 42 38 30 20 76 32 2E 30 drive $CB80 v2.0
02C0 20 63 6F 6D 70 69 6C 61 74 69 6F 6E 20 6F 66 20  compilation of 
02D0 24 69 6E 76 61 6C 69 64 20 63 6F 6D 6D 61 6E 64 $invalid command
02E0 20 6C 69 6E 65 24 63 6C 6F 73 65 20 6F 72 20 64  line$close or d
02F0 65 6C 65 74 65 20 65 72 72 6F 72 24 70 61 67 65 elete error$page
0300 20 24 73 79 6D 62 6F 6C 20 74 61 62 6C 65 20 6F  $symbol table o
0310 76 65 72 66 6C 6F 77 24 03 62 61 73 03 74 6D 70 verflow$.bas.tmp
0320 03 24 64 61 03 24 71 63 03 72 65 6C 03 6C 73 74 .$da.$qc.rel.lst
0330 03 24 70 61 69 6E 63 6C 75 64 65 73 20 6E 65 73 .$paincludes nes
0340 74 65 64 20 74 6F 20 64 65 65 70 3A 20 24 45 72 ted to deep: $Er
0350 72 6F 72 20 69 73 20 6E 65 61 72 20 73 6F 75 72 ror is near sour
0360 63 65 20 6C 69 6E 65 20 24 4D 69 73 73 69 6E 67 ce line $Missing
0370 20 73 6F 75 72 63 65 20 66 69 6C 65 20 6E 61 6D  source file nam
0380 65 24 04 0D 0A 0D 0A                            e$.....         

;; fn0387: 0387
;;   Called from:
;;     051D (in fn0483)
;;     063A (in fn061B)
;;     07AF (in fn075C)
;;     105B (in fn100A)
;;     125E (in fn1229)
fn0387 proc
	ld	hl,038F
	push	hl
	ld	hl,(1648)
	jp	(hl)
038F                                              C9                .

;; fn0390: 0390
;;   Called from:
;;     09A5 (in fn0990)
;;     0AF3 (in fn0990)
fn0390 proc
	ld	hl,1656
	ld	(hl),e
	dec	hl
	ld	(hl),b
	dec	hl
	ld	(hl),c
	dec	hl
	pop	de
	pop	bc
	ld	(hl),c
	push	de

l039D:
	ld	a,(1653)
	dec	a
	ld	(1653),a
	cp	a,FF
	jp	z,03BA

l03A9:
	ld	hl,(1654)
	ld	a,(1656)
	ld	(hl),a
	ld	hl,(1654)
	inc	hl
	ld	(1654),hl
	jp	039D

l03BA:
	ret

;; fn03BB: 03BB
;;   Called from:
;;     03DF (in fn03CB)
;;     03EE (in fn03E6)
fn03BB proc
	ld	hl,1657
	ld	(hl),c
	ld	a,(1657)
	sub	a,61
	ld	c,a
	ld	a,1A
	sub	a,c
	sbc	a,a
	cpl
	ret

;; fn03CB: 03CB
;;   Called from:
;;     09C1 (in fn0990)
fn03CB proc
	ld	hl,1658
	ld	(hl),c
	ld	a,(1658)
	sub	a,41
	ld	c,a
	ld	a,19
	sub	a,c
	sbc	a,a
	cpl
	push	af
	ld	hl,(1658)
	ld	c,l
	call	fn03BB
	pop	bc
	ld	c,b
	or	a,c
	ret

;; fn03E6: 03E6
;;   Called from:
;;     0B1C (in fn0990)
;;     0B55 (in fn0990)
fn03E6 proc
	ld	hl,1659
	ld	(hl),c
	ld	hl,(1659)
	ld	c,l
	call	fn03BB
	rra
	jp	nc,03FB

l03F5:
	ld	a,(1659)
	add	a,5F
	ret

l03FB:
	ld	a,(1659)
	ret
03FF                                              C9                .

;; fn0400: 0400
fn0400 proc
	ld	hl,(0006)
	ld	(1640),hl
	ld	hl,0080
	ld	(164E),hl
	ret

;; fn040D: 040D
;;   Called from:
;;     0C4E (in fn0BE4)
fn040D proc
	ld	hl,165E
	ld	(hl),e
	dec	hl
	ld	(hl),b
	dec	hl
	ld	(hl),c
	dec	hl
	pop	de
	pop	bc
	ld	(hl),b
	dec	hl
	ld	(hl),c
	push	de

l041C:
	ld	a,00
	ld	hl,165E
	cp	a,(hl)
	jp	nc,045A

l0425:
	ld	hl,(165C)
	ex	de,hl
	ld	c,1A
	call	0221
	ld	hl,(165A)
	ex	de,hl
	ld	c,14
	call	0221
	cp	a,00
	jp	nz,044D

l043C:
	ld	hl,165E
	dec	(hl)
	ld	de,0080
	ld	hl,(165C)
	add	hl,de
	ld	(165C),hl
	jp	0457

l044D:
	ld	hl,(165C)
	ld	(hl),1A
	ld	hl,165E
	ld	(hl),00

l0457:
	jp	041C

l045A:
	ret

;; fn045B: 045B
;;   Called from:
;;     1044 (in fn100A)
;;     1078 (in fn100A)
;;     1081 (in fn100A)
;;     11B5 (in fn100A)
;;     11C6 (in fn100A)
;;     11D9 (in fn100A)
;;     11F4 (in fn100A)
;;     1255 (in fn1229)
;;     126A (in fn1262)
;;     1279 (in fn1279)
;;     1292 (in fn1279)
;;     12D8 (in fn12D8)
;;     12E9 (in fn12D8)
;;     12F7 (in fn12D8)
fn045B proc
	ld	a,(0080)
	cp	a,00
	jp	nz,0466

l0463:
	ld	a,00
	ret

l0466:
	ld	hl,0080
	dec	(hl)
	ld	hl,(164E)
	inc	hl
	ld	(164E),hl
	ld	a,(hl)
	ret

;; fn0473: 0473
;;   Called from:
;;     04EA (in fn0483)
;;     0515 (in fn0483)
;;     051A (in fn0483)
;;     058D (in fn056B)
fn0473 proc
	ld	hl,165F
	ld	(hl),c
	ld	hl,(165F)
	ld	h,00
	ex	de,hl
	ld	c,02
	call	0221
	ret

;; fn0483: 0483
;;   Called from:
;;     0561 (in fn0534)
fn0483 proc
	ld	hl,1665
	ld	(hl),d
	dec	hl
	ld	(hl),e
	dec	hl
	ld	(hl),b
	dec	hl
	ld	(hl),c
	dec	hl
	pop	de
	pop	bc
	ld	(hl),b
	dec	hl
	ld	(hl),c
	push	de
	ld	de,0080
	ld	hl,(1664)
	add	hl,de
	dec	hl
	ld	c,07
	call	fn1326
	ex	de,hl
	ld	hl,1667
	ld	(hl),e

l04A6:
	ld	a,00
	ld	hl,1667
	cp	a,(hl)
	jp	nc,0523

l04AF:
	ld	hl,(1662)
	ex	de,hl
	ld	c,1A
	call	0221
	ld	hl,(1660)
	ex	de,hl
	ld	c,15
	call	0221
	cp	a,00
	jp	nz,04D7

l04C6:
	ld	hl,1667
	dec	(hl)
	ld	de,0080
	ld	hl,(1662)
	add	hl,de
	ld	(1662),hl
	jp	0520

l04D7:
	ld	hl,02A3
	ld	(1668),hl

l04DD:
	ld	hl,(1668)
	ld	a,(hl)
	cp	a,24
	jp	z,04F7

l04E6:
	ld	hl,(1668)
	ld	c,(hl)
	call	fn0473
	ld	hl,(1668)
	inc	hl
	ld	(1668),hl
	jp	04DD

l04F7:
	ld	hl,(1660)
	ld	a,(hl)
	cp	a,00
	jp	nz,050D

l0500:
	ld	de,0000
	ld	c,19
	call	0221
	inc	a
	ld	hl,(1660)
	ld	(hl),a

l050D:
	ld	hl,(1660)
	ld	a,61
	add	a,(hl)
	dec	a
	ld	c,a
	call	fn0473
	ld	c,3A
	call	fn0473
	call	fn0387

l0520:
	jp	04A6

l0523:
	ret

;; fn0524: 0524
;;   Called from:
;;     057B (in fn056B)
fn0524 proc
	ld	hl,166A
	ld	(hl),c
	ld	hl,(166A)
	ld	h,00
	ex	de,hl
	ld	c,05
	call	0221
	ret

;; fn0534: 0534
;;   Called from:
;;     059F (in fn056B)
fn0534 proc
	ld	hl,166B
	ld	(hl),c
	ld	hl,(14BE)
	ex	de,hl
	ld	hl,(14BC)
	add	hl,de
	ld	a,(166B)
	ld	(hl),a
	ld	hl,(14BE)
	inc	hl
	ld	(14BE),hl
	ex	de,hl
	ld	hl,0200
	call	fn1335
	jp	c,056A

l0555:
	ld	bc,14C0
	push	bc
	ld	hl,(14BC)
	ld	b,h
	ld	c,l
	ld	de,0200
	call	fn0483
	ld	hl,0000
	ld	(14BE),hl

l056A:
	ret

;; fn056B: 056B
;;   Called from:
;;     05E1 (in fn05CE)
;;     0610 (in fn05EF)
;;     0735 (in fn0722)
;;     0741 (in fn0722)
;;     0755 (in fn0745)
;;     0775 (in fn075C)
;;     077A (in fn075C)
;;     078D (in fn075C)
;;     0792 (in fn075C)
;;     07BC (in fn07B3)
;;     07CB (in fn07B3)
;;     07D0 (in fn07B3)
;;     07D8 (in fn07B3)
;;     07DD (in fn07B3)
;;     0816 (in fn0814)
;;     081B (in fn0814)
;;     1207 (in fn100A)
;;     123E (in fn1229)
;;     1243 (in fn1229)
;;     1252 (in fn1229)
fn056B proc
	ld	hl,166C
	ld	(hl),c
	ld	a,(14F3)
	cp	a,01
	jp	nz,0581

l0577:
	ld	hl,(166C)
	ld	c,l
	call	fn0524
	jp	05A2

l0581:
	ld	a,(14F3)
	cp	a,02
	jp	nz,0593

l0589:
	ld	hl,(166C)
	ld	c,l
	call	fn0473
	jp	05A2

l0593:
	ld	a,(14F3)
	cp	a,04
	jp	nz,05A2

l059B:
	ld	hl,(166C)
	ld	c,l
	call	fn0534

l05A2:
	ld	a,(166C)
	cp	a,0D
	jp	nz,05B2

l05AA:
	ld	hl,14F6
	ld	(hl),01
	jp	05CD

l05B2:
	ld	a,(166C)
	cp	a,0A
	jp	nz,05C9

l05BA:
	ld	a,(14F3)
	cp	a,00
	jp	z,05C6

l05C2:
	ld	hl,14FD
	inc	(hl)

l05C6:
	jp	05CD

l05C9:
	ld	hl,14F6
	inc	(hl)

l05CD:
	ret

;; fn05CE: 05CE
;;   Called from:
;;     062F (in fn061B)
;;     0780 (in fn075C)
;;     07A4 (in fn075C)
;;     07E3 (in fn07B3)
;;     07F8 (in fn07B3)
;;     0C0A (in fn0BE4)
;;     1058 (in fn100A)
;;     1231 (in fn1229)
fn05CE proc
	ld	hl,166E
	ld	(hl),b
	dec	hl
	ld	(hl),c

l05D4:
	ld	hl,(166D)
	ld	a,(hl)
	cp	a,24
	jp	z,05EE

l05DD:
	ld	hl,(166D)
	ld	c,(hl)
	call	fn056B
	ld	hl,(166D)
	inc	hl
	ld	(166D),hl
	jp	05D4

l05EE:
	ret

;; fn05EF: 05EF
;;   Called from:
;;     0637 (in fn061B)
;;     06E2 (in fn06CE)
;;     07E9 (in fn07B3)
;;     080B (in fn07B3)
fn05EF proc
	ld	hl,1670
	ld	(hl),b
	dec	hl
	ld	(hl),c
	ld	hl,1671
	ld	(hl),01

l05FA:
	ld	hl,(166F)
	ld	a,(hl)
	ld	hl,1671
	cp	a,(hl)
	jp	c,061A

l0605:
	ld	hl,(1671)
	ld	h,00
	ex	de,hl
	ld	hl,(166F)
	add	hl,de
	ld	c,(hl)
	call	fn056B
	ld	hl,1671
	inc	(hl)
	jp	nz,05FA

l061A:
	ret

;; fn061B: 061B
;;   Called from:
;;     08AE (in fn082F)
;;     0968 (in fn0920)
;;     098C (in fn0920)
fn061B proc
	ld	hl,1675
	ld	(hl),d
	dec	hl
	ld	(hl),e
	dec	hl
	ld	(hl),b
	dec	hl
	ld	(hl),c
	ld	hl,14F3
	ld	(hl),02
	ld	hl,(1672)
	ld	b,h
	ld	c,l
	call	fn05CE
	ld	hl,(1674)
	ld	b,h
	ld	c,l
	call	fn05EF
	call	fn0387
	ret

;; fn063E: 063E
;;   Called from:
;;     06DC (in fn06CE)
fn063E proc
	ld	hl,1679
	ld	(hl),d
	dec	hl
	ld	(hl),e
	dec	hl
	ld	(hl),b
	dec	hl
	ld	(hl),c
	ld	hl,167B
	ld	(hl),00
	ld	hl,(1678)
	ld	(hl),00
	ld	hl,167A
	ld	(hl),00

l0657:
	ld	a,03
	ld	hl,167A
	cp	a,(hl)
	jp	c,06B6

l0660:
	ld	hl,167C
	ld	(hl),30

l0665:
	ld	hl,(167A)
	ld	h,00
	ld	bc,0227
	add	hl,hl
	add	hl,bc
	ld	de,1676
	call	fn1348
	jp	c,0697

l0678:
	ld	hl,(167A)
	ld	h,00
	ld	bc,0227
	add	hl,hl
	add	hl,bc
	ld	de,1676
	call	fn1348
	ex	de,hl
	dec	hl
	ld	(hl),e
	inc	hl
	ld	(hl),d
	ld	hl,167B
	ld	(hl),01
	inc	hl
	inc	(hl)
	jp	0665

l0697:
	ld	a,(167B)
	rra
	jp	nc,06AF

l069E:
	ld	hl,(1678)
	ld	a,(hl)
	inc	a
	ld	(hl),a
	ld	c,a
	ld	b,00
	ld	hl,(1678)
	add	hl,bc
	ld	a,(167C)
	ld	(hl),a

l06AF:
	ld	hl,167A
	inc	(hl)
	jp	nz,0657

l06B6:
	ld	de,0030
	ld	hl,(1676)
	add	hl,de
	push	hl
	ld	hl,(1678)
	ld	a,(hl)
	inc	a
	ld	(hl),a
	ld	c,a
	ld	b,00
	ld	hl,(1678)
	add	hl,bc
	pop	bc
	ld	(hl),c
	ret

;; fn06CE: 06CE
;;   Called from:
;;     0788 (in fn075C)
;;     07AC (in fn075C)
;;     0801 (in fn07B3)
;;     0C14 (in fn0BE4)
fn06CE proc
	ld	hl,167E
	ld	(hl),b
	dec	hl
	ld	(hl),c
	ld	hl,(167D)
	ld	b,h
	ld	c,l
	ld	de,162C
	call	fn063E
	ld	bc,162C
	call	fn05EF
	ret
06E6                   21 80 16 70 2B 71 2A 7F 16 7C       !..p+q*..|
06F0 E6 F8 1F 1F 1F 1F 4F CD 22 07 2A 7F 16 7C E6 0F ......O.".*..|..
0700 4F CD 22 07 2A 7F 16 7D E6 F8 1F 1F 1F 1F 4F CD O.".*..}......O.
0710 22 07 2A 7F 16 7D E6 0F 4F CD 22 07 0E 68 CD 6B ".*..}..O."..h.k
0720 05 C9                                           ..              

;; fn0722: 0722
fn0722 proc
	ld	hl,1681
	ld	(hl),c
	ld	a,09
	ld	hl,1681
	cp	a,(hl)
	jp	nc,073B

l072F:
	ld	a,(1681)
	add	a,57
	ld	c,a
	call	fn056B
	jp	0744

l073B:
	ld	a,(1681)
	add	a,30
	ld	c,a
	call	fn056B

l0744:
	ret

;; fn0745: 0745
;;   Called from:
;;     07F2 (in fn07B3)
fn0745 proc
	ld	hl,1682
	ld	(hl),c

l0749:
	ld	hl,1682
	ld	a,(14F6)
	cp	a,(hl)
	jp	nc,075B

l0753:
	ld	c,20
	call	fn056B
	jp	0749

l075B:
	ret

;; fn075C: 075C
;;   Called from:
;;     0C6E (in fn0BE4)
;;     0D91 (in fn0D84)
fn075C proc
	ld	hl,1686
	ld	(hl),d
	dec	hl
	ld	(hl),e
	dec	hl
	ld	(hl),b
	dec	hl
	ld	(hl),c
	ld	hl,14F3
	ld	(hl),02
	ld	a,(14F6)
	cp	a,01
	jp	z,077D

l0773:
	ld	c,0D
	call	fn056B
	ld	c,0A
	call	fn056B

l077D:
	ld	bc,0260
	call	fn05CE
	ld	hl,(1683)
	ld	b,h
	ld	c,l
	call	fn06CE
	ld	c,0D
	call	fn056B
	ld	c,0A
	call	fn056B
	ld	a,00
	ld	de,1685
	call	fn1353
	or	a,l
	jp	z,07AF

l07A1:
	ld	bc,034E
	call	fn05CE
	ld	hl,(1685)
	ld	b,h
	ld	c,l
	call	fn06CE

l07AF:
	call	fn0387
	ret

;; fn07B3: 07B3
;;   Called from:
;;     082B (in fn0814)
;;     1221 (in fn100A)
fn07B3 proc
	ld	a,(14F9)
	rra
	jp	nc,080E

l07BA:
	ld	c,0D
	call	fn056B

l07BF:
	ld	hl,14F7
	ld	a,(14FD)
	cp	a,(hl)
	jp	nc,07D6

l07C9:
	ld	c,0D
	call	fn056B
	ld	c,0A
	call	fn056B
	jp	07BF

l07D6:
	ld	c,0D
	call	fn056B
	ld	c,0A
	call	fn056B
	ld	bc,02B7
	call	fn05CE
	ld	bc,14FF
	call	fn05EF
	ld	a,(14F8)
	sub	a,0A
	ld	c,a
	call	fn0745
	ld	bc,02FC
	call	fn05CE
	ld	hl,(14FE)
	ld	c,l
	ld	b,00
	call	fn06CE
	ld	hl,14FE
	inc	(hl)
	ld	bc,0382
	call	fn05EF

l080E:
	ld	hl,14FD
	ld	(hl),03
	ret

;; fn0814: 0814
;;   Called from:
;;     0C04 (in fn0BE4)
;;     0C17 (in fn0BE4)
fn0814 proc
	ld	c,0D
	call	fn056B
	ld	c,0A
	call	fn056B
	ld	a,(14F7)
	sub	a,03
	ld	c,a
	ld	a,(14FD)
	cp	a,c
	jp	c,082E

l082B:
	call	fn07B3

l082E:
	ret

;; fn082F: 082F
;;   Called from:
;;     0C33 (in fn0BE4)
fn082F proc
	ld	hl,168D
	ld	(hl),e
	dec	hl
	ld	(hl),b
	dec	hl
	ld	(hl),c
	dec	hl
	pop	de
	pop	bc
	ld	(hl),b
	dec	hl
	ld	(hl),c
	dec	hl
	pop	bc
	ld	(hl),b
	dec	hl
	ld	(hl),c
	push	de
	ld	hl,168F
	ld	(hl),00
	inc	hl
	ld	(hl),00
	inc	hl
	ld	(hl),00
	ld	a,(168D)
	rlca
	rra
	jp	nc,085E

l0856:
	ld	hl,1690
	ld	(hl),01
	jp	087E

l085E:
	ld	a,(168D)
	rlca
	rlca
	rra
	jp	nc,086F

l0867:
	ld	hl,1691
	ld	(hl),01
	jp	087E

l086F:
	ld	a,(168D)
	rlca
	rlca
	rlca
	rra
	jp	nc,087E

l0879:
	ld	hl,168F
	ld	(hl),01

l087E:
	ld	a,(168D)
	rlca
	rlca
	rlca
	rlca
	rra
	jp	nc,088E

l0889:
	ld	hl,168E
	ld	(hl),01

l088E:
	ld	a,(168D)
	add	a,0F
	ld	(168D),a
	ld	hl,(1687)
	ld	b,h
	ld	c,l
	ld	hl,(1689)
	ex	de,hl
	call	fn0990
	cpl
	rra
	jp	nc,08B1

l08A7:
	ld	hl,(1687)
	ex	de,hl
	ld	bc,024C
	call	fn061B

l08B1:
	ld	a,(1690)
	rra
	jp	nc,08DD

l08B8:
	ld	hl,(168B)
	ld	a,(hl)
	cp	a,00
	jp	nz,08DA

l08C1:
	ld	a,(151D)
	cp	a,00
	jp	nz,08D3

l08C9:
	ld	hl,(168B)
	ld	a,(151F)
	ld	(hl),a
	jp	08DA

l08D3:
	ld	hl,(168B)
	ld	a,(151D)
	ld	(hl),a

l08DA:
	jp	fn0920

l08DD:
	ld	a,(1691)
	rra
	jp	nc,0900

l08E4:
	ld	a,(151E)
	cp	a,00
	jp	z,08F6

;; fn08EC: 08EC
;;   Called from:
;;     08E9 (in fn08EC)
;;     08E9 (in fn082F)
fn08EC proc
	ld	hl,(168B)
	ld	a,(151E)
	ld	(hl),a
	jp	fn08FD

l08F6:
	ld	hl,(168B)
	ld	a,(151F)
	ld	(hl),a

;; fn08FD: 08FD
;;   Called from:
;;     08F3 (in fn08EC)
;;     08FC (in fn08FD)
fn08FD proc
	jp	fn0920

l0900:
	ld	a,(168F)
	rra
	jp	nc,fn0920

l0907:
	ld	a,(1520)
	cp	a,00
	jp	z,0919

l090F:
	ld	hl,(168B)
	ld	a,(1520)
	ld	(hl),a
	jp	fn0920

l0919:
	ld	hl,(168B)
	ld	a,(151F)
	ld	(hl),a

;; fn0920: 0920
;;   Called from:
;;     08DA (in fn082F)
;;     08FD (in fn08FD)
;;     0904 (in fn082F)
;;     0916 (in fn082F)
;;     091F (in fn082F)
fn0920 proc
	ld	de,0080
	ld	c,1A
	call	0221
	ld	a,(168D)
	cp	a,00
	jp	nz,096E

l0930:
	ld	de,0000
	ld	c,0C
	call	0221
	sub	a,30
	sbc	a,a
	cpl
	ld	hl,168E
	and	a,(hl)
	rra
	jp	nc,0953

l0944:
	ld	bc,0007
	ld	hl,(168B)
	add	hl,bc
	ld	a,80
	or	a,(hl)
	ld	hl,(168B)
	add	hl,bc
	ld	(hl),a

l0953:
	ld	hl,(168B)
	ex	de,hl
	ld	c,0F
	call	0221
	cp	a,FF
	jp	nz,096B

l0961:
	ld	hl,(1687)
	ex	de,hl
	ld	bc,0236
	call	fn061B

l096B:
	jp	098F

l096E:
	ld	hl,(168B)
	ex	de,hl
	ld	c,13
	call	0221
	ld	hl,(168B)
	ex	de,hl
	ld	c,16
	call	0221
	cp	a,FF
	jp	nz,098F

l0985:
	ld	hl,(1687)
	ex	de,hl
	ld	bc,0287
	call	fn061B

l098F:
	ret

;; fn0990: 0990
;;   Called from:
;;     089F (in fn082F)
fn0990 proc
	ld	hl,1695
	ld	(hl),d
	dec	hl
	ld	(hl),e
	dec	hl
	ld	(hl),b
	dec	hl
	ld	(hl),c
	ld	c,0B
	push	bc
	ld	hl,(168B)
	inc	hl
	ld	b,h
	ld	c,l
	ld	e,20
	call	fn0390
	ld	hl,(1692)
	ld	a,(hl)
	ld	(1696),a
	ld	hl,(1692)
	inc	hl
	ld	(1692),hl
	inc	hl
	ld	a,(hl)
	cp	a,3A
	jp	nz,0A0C

l09BD:
	ld	hl,(1692)
	ld	c,(hl)
	call	fn03CB
	ld	hl,(1692)
	push	af
	ld	a,(hl)
	sub	a,40
	sub	a,01
	sbc	a,a
	pop	bc
	ld	c,b
	or	a,c
	push	af
	ld	a,02
	ld	hl,1696
	sub	a,(hl)
	sbc	a,a
	pop	bc
	ld	c,b
	and	a,c
	rra
	jp	nc,0A06

l09E0:
	ld	hl,(1692)
	ld	a,1F
	and	a,(hl)
	ld	hl,(168B)
	ld	(hl),a
	ld	hl,(168B)
	ld	a,10
	cp	a,(hl)
	jp	nc,09F6

l09F3:
	ld	a,00
	ret

l09F6:
	ld	hl,(1692)
	inc	hl
	inc	hl
	ld	(1692),hl
	ld	hl,1696
	dec	(hl)
	dec	(hl)
	jp	0A09

l0A06:
	ld	a,00
	ret

l0A09:
	jp	0A11

l0A0C:
	ld	hl,(168B)
	ld	(hl),00

l0A11:
	ld	hl,1697
	ld	(hl),00

l0A16:
	ld	a,07
	ld	hl,1697
	sub	a,(hl)
	sbc	a,a
	cpl
	push	af
	call	fn0B74
	pop	bc
	ld	c,b
	and	a,c
	rra
	jp	nc,0A53

l0A29:
	ld	hl,(1697)
	ld	h,00
	ex	de,hl
	ld	hl,(1692)
	add	hl,de
	push	hl
	ld	hl,(1697)
	ld	h,00
	ld	bc,0001
	add	hl,bc
	ex	de,hl
	ld	hl,(168B)
	add	hl,de
	pop	de
	ld	a,(de)
	ld	(hl),a
	cp	a,2A
	jp	nz,0A4D

l0A4A:
	ld	a,00
	ret

l0A4D:
	call	fn0B91
	jp	0A16

l0A53:
	ld	a,01
	ld	hl,1696
	sub	a,(hl)
	sbc	a,a
	inc	hl
	ld	c,(hl)
	ld	b,00
	ld	hl,(1692)
	add	hl,bc
	push	af
	ld	a,(hl)
	sub	a,2E
	sub	a,01
	sbc	a,a
	pop	bc
	ld	c,b
	and	a,c
	rra
	jp	nc,0AA3

l0A70:
	call	fn0B91
	ld	a,03
	ld	hl,1696
	cp	a,(hl)
	jp	nc,0A7F

l0A7C:
	ld	a,00
	ret

l0A7F:
	ld	hl,(1696)
	push	hl
	ld	hl,(1697)
	ld	h,00
	ex	de,hl
	ld	hl,(1692)
	add	hl,de
	ld	bc,0009
	push	hl
	ld	hl,(168B)
	add	hl,bc
	ex	de,hl
	pop	bc
	pop	hl

l0A98:
	ld	a,(bc)
	ld	(de),a
	inc	bc
	inc	de
	dec	l
	jp	nz,0A98

l0AA0:
	jp	0AE5

l0AA3:
	ld	a,(1696)
	sub	a,00
	sub	a,01
	sbc	a,a
	ld	hl,(1697)
	ld	h,00
	ex	de,hl
	ld	hl,(1692)
	add	hl,de
	push	af
	ld	a,(hl)
	sub	a,2E
	sub	a,01
	sbc	a,a
	pop	bc
	ld	c,b
	or	a,c
	rra
	jp	nc,0AE2

l0AC3:
	ld	hl,(1694)
	ld	l,(hl)
	push	hl
	ld	hl,(1694)
	inc	hl
	ld	bc,0009
	push	hl
	ld	hl,(168B)
	add	hl,bc
	ex	de,hl
	pop	bc
	pop	hl

l0AD7:
	ld	a,(bc)
	ld	(de),a
	inc	bc
	inc	de
	dec	l
	jp	nz,0AD7

l0ADF:
	jp	0AE5

l0AE2:
	ld	a,00
	ret

l0AE5:
	ld	c,03
	push	bc
	ld	bc,000C
	ld	hl,(168B)
	add	hl,bc
	ld	b,h
	ld	c,l
	ld	e,00
	call	fn0390
	ld	bc,0020
	ld	hl,(168B)
	add	hl,bc
	ld	(hl),00
	ld	hl,1697
	ld	(hl),00

l0B04:
	ld	a,07
	ld	hl,1697
	cp	a,(hl)
	jp	c,0B38

l0B0D:
	ld	hl,(1697)
	ld	h,00
	ld	bc,0001
	add	hl,bc
	ex	de,hl
	ld	hl,(168B)
	add	hl,de
	ld	c,(hl)
	call	fn03E6
	ld	hl,(1697)
	ld	h,00
	ld	bc,0001
	add	hl,bc
	ex	de,hl
	ld	hl,(168B)
	add	hl,de
	ld	(hl),a
	ld	a,(1697)
	inc	a
	ld	(1697),a
	jp	nz,0B04

l0B38:
	ld	hl,1697
	ld	(hl),00

l0B3D:
	ld	a,02
	ld	hl,1697
	cp	a,(hl)
	jp	c,0B71

l0B46:
	ld	hl,(1697)
	ld	h,00
	ld	bc,0009
	add	hl,bc
	ex	de,hl
	ld	hl,(168B)
	add	hl,de
	ld	c,(hl)
	call	fn03E6
	ld	hl,(1697)
	ld	h,00
	ld	bc,0009
	add	hl,bc
	ex	de,hl
	ld	hl,(168B)
	add	hl,de
	ld	(hl),a
	ld	a,(1697)
	inc	a
	ld	(1697),a
	jp	nz,0B3D

l0B71:
	ld	a,01
	ret

;; fn0B74: 0B74
;;   Called from:
;;     0A1F (in fn0990)
fn0B74 proc
	ld	hl,(1697)
	ld	h,00
	ex	de,hl
	ld	hl,(1692)
	add	hl,de
	ld	a,(hl)
	sub	a,2E
	add	a,FF
	sbc	a,a
	push	af
	ld	a,(1696)
	sub	a,00
	add	a,FF
	sbc	a,a
	pop	bc
	ld	c,b
	and	a,c
	ret

;; fn0B91: 0B91
;;   Called from:
;;     0A4D (in fn0990)
;;     0A70 (in fn0990)
fn0B91 proc
	ld	hl,1697
	inc	(hl)
	dec	hl
	dec	(hl)
	ret
0B98                         21 99 16 70 2B 71 11 80         !..p+q..
0BA0 00 0E 1A CD 21 02 2A 98 16 EB 0E 10 CD 21 02 FE ....!.*......!..
0BB0 FF C2 C0 0B CD 14 08 01 E6 02 CD CE 05 CD 87 03 ................
0BC0 C9 21 9B 16 70 2B 71 3A 8B 13 E6 01 FE 00 CA D2 .!..p+q:........
0BD0 0B C9 11 80 00 0E 1A CD 21 02 2A 9A 16 EB 0E 13 ........!.*.....
0BE0 CD 21 02 C9                                     .!..            

;; fn0BE4: 0BE4
;;   Called from:
;;     1224 (in fn100A)
fn0BE4 proc
	ld	a,(138A)
	sub	a,01
	add	a,FF
	sbc	a,a
	push	af
	ld	a,(14F3)
	sub	a,02
	sub	a,01
	sbc	a,a
	pop	bc
	ld	c,b
	and	a,c
	rra
	jp	nc,0C1A

l0BFC:
	ld	a,(14F6)
	cp	a,01
	jp	z,0C07

l0C04:
	call	fn0814

l0C07:
	ld	bc,0296
	call	fn05CE
	ld	a,(138A)
	dec	a
	ld	c,a
	ld	b,00
	call	fn06CE
	call	fn0814

l0C1A:
	ld	hl,(1388)
	ld	sp,hl
	ld	a,(138A)
	add	a,30
	ld	(1645),a
	ld	bc,0231
	push	bc
	ld	bc,1642
	push	bc
	ld	e,10
	ld	bc,005C
	call	fn082F
	ld	bc,005C
	push	bc
	ld	bc,0108
	ld	de,0103
	call	fn1346
	ld	c,07
	call	fn1326
	ex	de,hl
	ld	hl,(0108)
	ld	b,h
	ld	c,l
	call	fn040D
	ld	a,(138A)
	sub	a,00
	sub	a,01
	sbc	a,a
	push	af
	ld	a,03
	ld	hl,138A
	sub	a,(hl)
	sbc	a,a
	pop	bc
	ld	c,b
	or	a,c
	rra
	jp	nc,0C71

l0C68:
	ld	de,0000
	ld	bc,0001
	call	fn075C

l0C71:
	ld	a,(138A)
	cp	a,01
	jp	nz,0C7C

l0C79:
	jp	020A

l0C7C:
	ld	a,(138A)
	cp	a,02
	jp	nz,0C87

l0C84:
	jp	0224

l0C87:
	ld	a,(138A)
	cp	a,03
	jp	nz,0C92

l0C8F:
	jp	0105

l0C92:
	ret

;; fn0C93: 0C93
;;   Called from:
;;     0E67 (in fn0E63)
fn0C93 proc
	ld	a,(138D)
	ld	hl,138C
	add	a,(hl)
	ld	c,(hl)
	ld	b,00
	ld	hl,138C
	add	hl,bc
	add	a,(hl)
	add	a,7F
	ret
0CA5                01 07 00 2A 21 15 09 3E F0 A6 E6      ...*!..>...
0CB0 F8 1F 1F 1F 1F C9 21 9C 16 71 01 07 00 2A 21 15 ......!..q...*!.
0CC0 09 3E 0F A6 F5 3A 9C 16 87 87 87 87 C1 48 B1 77 .>...:.......H.w
0CD0 C9 01 07 00 2A 21 15 09 3E 0E A6 B7 1F C9 21 9D ....*!..>.....!.
0CE0 16 71 01 07 00 2A 21 15 09 3E F1 A6 F5 3A 9D 16 .q...*!..>...:..
0CF0 87 C1 48 B1 77 C9 01 07 00 2A 21 15 09 3E 01 A6 ..H.w....*!..>..
0D00 C9 01 07 00 2A 21 15 09 3E 01 B6 77 C9 01 08 00 ....*!..>..w....
0D10 2A 21 15 09 3E F0 A6 E6 F8 1F 1F 1F 1F C9 21 9E *!..>.........!.
0D20 16 71 01 08 00 2A 21 15 09 3E 0F A6 F5 3A 9E 16 .q...*!..>...:..
0D30 87 87 87 87 C1 48 B1 77 C9 01 08 00 2A 21 15 09 .....H.w....*!..
0D40 7E C9 01 08 00 2A 21 15 09 3E 01 B6 77 C9 01 08 ~....*!..>..w...
0D50 00 2A 21 15 09 7E 0F C9 01 08 00 2A 21 15 09 3E .*!..~.....*!..>
0D60 02 B6 77 C9                                     ..w.            

;; fn0D64: 0D64
;;   Called from:
;;     0D84 (in fn0D84)
fn0D64 proc
	ld	bc,0009
	ld	hl,(1521)
	add	hl,bc
	ld	a,(hl)
	or	a,a
	rra
	ret

;; fn0D6F: 0D6F
fn0D6F proc
	ld	bc,0009
	ld	hl,(1521)
	add	hl,bc
	ld	a,02
	or	a,(hl)
	ld	(hl),a
	ret
0D7B                                  01 09 00 2A 21            ...*!
0D80 15 09 7E C9                                     ..~.            

;; fn0D84: 0D84
fn0D84 proc
	call	fn0D64
	rra
	jp	nc,0D94

l0D8B:
	ld	de,0000
	ld	bc,0002
	call	fn075C

l0D94:
	ld	bc,0009
	ld	hl,(1521)
	add	hl,bc
	ld	a,01
	or	a,(hl)
	ld	(hl),a
	ret
0DA0 01 09 00 2A 21 15 09 7E E6 FE 1F 1F C9 01 09 00 ...*!..~........
0DB0 2A 21 15 09 3E 04 B6 77 C9                      *!..>..w.       

;; fn0DB9: 0DB9
fn0DB9 proc
	ld	bc,0009
	ld	hl,(1521)
	add	hl,bc
	ld	a,(hl)
	add	a,FC
	rra
	rra
	rra
	ret
0DC7                      CD B9 0D C9                       ....     

;; fn0DCB: 0DCB
fn0DCB proc
	ld	bc,0009
	ld	hl,(1521)
	add	hl,bc
	ld	a,08
	or	a,(hl)
	ld	(hl),a
	ret
0DD7                      CD CB 0D C9 21 A0 16 70 2B        ....!..p+
0DE0 71 CD 6F 0D 01 03 00 2A 21 15 09 E5 2A 9F 16 EB q.o....*!...*...
0DF0 E1 73 23 72 C9 01 03 00 2A 21 15 09 5E 23 56 EB .s#r....*!..^#V.
0E00 C9 21 A2 16 70 2B 71 01 05 00 2A 21 15 09 E5 2A .!..p+q...*!...*
0E10 A1 16 EB E1 73 23 72 C9 01 05 00 2A 21 15 09 5E ....s#r....*!..^
0E20 23 56 EB C9 21 A4 16 70 2B 71 CD 84 0D 01 03 00 #V..!..p+q......
0E30 2A 21 15 09 E5 2A A3 16 EB E1 73 23 72 C9 01 03 *!...*....s#r...
0E40 00 2A 21 15 09 5E 23 56 EB C9 21 A5 16 71 01 08 .*!..^#V..!..q..
0E50 00 2A 21 15 09 3A A5 16 77 C9 01 08 00 2A 21 15 .*!..:..w....*!.
0E60 09 7E C9                                        .~.             

;; fn0E63: 0E63
fn0E63 proc
	ld	hl,16A6
	ld	(hl),c
	call	fn0C93
	ld	c,a
	ld	b,00
	ld	hl,152C
	add	hl,bc
	add	hl,bc
	ld	e,(hl)
	inc	hl
	ld	d,(hl)
	ex	de,hl
	ld	(1521),hl

l0E79:
	ld	a,00
	ld	de,1521
	call	fn1353
	or	a,l
	jp	z,0EA8

l0E85:
	ld	hl,(1521)
	inc	hl
	inc	hl
	ld	a,(16A6)
	cp	a,(hl)
	jp	nz,0E9B

l0E91:
	call	fn0EAB
	rra
	jp	nc,0E9B

l0E98:
	ld	a,01
	ret

l0E9B:
	ld	hl,(1521)
	ld	e,(hl)
	inc	hl
	ld	d,(hl)
	ex	de,hl
	ld	(1521),hl
	jp	0E79

l0EA8:
	ld	a,00
	ret

;; fn0EAB: 0EAB
;;   Called from:
;;     0E91 (in fn0E63)
fn0EAB proc
	ld	hl,16A7
	ld	(hl),00

l0EB0:
	ld	a,(138C)
	ld	hl,16A7
	cp	a,(hl)
	jp	c,0EE2

l0EBA:
	ld	hl,(16A7)
	ld	h,00
	ld	bc,000A
	add	hl,bc
	ex	de,hl
	ld	hl,(1521)
	add	hl,de
	push	hl
	ld	hl,(16A7)
	ld	h,00
	ld	bc,138C
	add	hl,bc
	pop	de
	ld	a,(de)
	cp	a,(hl)
	jp	z,0EDB

l0ED8:
	ld	a,00
	ret

l0EDB:
	ld	hl,16A7
	inc	(hl)
	jp	0EB0

l0EE2:
	ld	a,01
	ret
0EE5                3E 00 21 2A 15 BE D2 FC 0E 2A 2A      >.!*.....**
0EF0 15 4D CD 63 0E 1F D2 FC 0E 3E 01 C9 0E 00 CD 63 .M.c.....>.....c
0F00 0E C9 2A 2A 15 4D CD 63 0E C9 2A 23 15 22 21 15 ..**.M.c..*#."!.
0F10 CD B8 0F 11 23 15 CD 15 13 EB 2B 73 23 72 EB 11 ....#.....+s#r..
0F20 25 15 CD 56 13 D2 3A 0F 01 02 03 CD CE 05 CD 14 %..V..:.........
0F30 08 11 00 00 01 03 00 CD 5C 07 3A 8C 13 3C 6F 01 ........\.:..<o.
0F40 0A 00 E5 2A 21 15 09 EB 01 8C 13 E1 0A 12 03 13 ...*!...........
0F50 2D C2 4C 0F 01 05 00 2A 21 15 09 3E 00 77 23 36 -.L....*!..>.w#6
0F60 00 01 03 00 2A 21 15 09 77 23 36 00 01 07 00 2A ....*!..w#6....*
0F70 21 15 09 36 00 2A 21 15 03 09 36 00 2A 21 15 03 !..6.*!...6.*!..
0F80 09 36 00 2A 21 15 23 23 3A 2A 15 77 CD 93 0C 4F .6.*!.##:*.w...O
0F90 06 00 21 2C 15 09 09 E5 2A 21 15 E3 4E 23 46 E1 ..!,....*!..N#F.
0FA0 71 23 70 CD 93 0C 4F 06 00 21 2C 15 09 09 E5 2A q#p...O..!,....*
0FB0 21 15 EB E1 73 23 72 C9                         !...s#r.        

;; fn0FB8: 0FB8
fn0FB8 proc
	ld	hl,(138C)
	ld	h,00
	ld	bc,000B
	add	hl,bc
	ld	a,l
	ret
0FC3          11 3C 00 2A 21 15 19 EB 3E 31 CD 3C 13    .<.*!...>1.<.
0FD0 01 0A 00 E5 2A 21 15 09 5E 16 00 E1 19 22 21 15 ....*!..^...."!.
0FE0 C9 31 88 13 21 00 00 39 22 88 13 CD 00 04 01 0A .1..!..9".......
0FF0 01 CD CE 05 CD 5B 04 32 A8 16 CD 62 12 21 FF 14 .....[.2...b.!..
1000 36 00                                           6.              

l1002:
	ld	a,(16A8)
	sub	a,20
	add	a,FF
	sbc	a,a

;; fn100A: 100A
fn100A proc
	push	af
	ld	a,(16A8)
	add	a,5F
	sub	a,5B
	add	a,FF
	sbc	a,a
	pop	bc
	ld	c,b
	and	a,c
	push	af
	ld	a,(16A8)
	sub	a,00
	add	a,FF
	sbc	a,a
	pop	bc
	ld	c,b
	and	a,c
	push	af
	ld	a,(14FF)
	sub	a,0E
	sbc	a,a
	pop	bc
	ld	c,b
	and	a,c
	rra
	jp	nc,104D

l1032:
	ld	a,(14FF)
	inc	a
	ld	(14FF),a
	ld	c,a
	ld	b,00
	ld	hl,14FF
	add	hl,bc
	ld	a,(16A8)
	ld	(hl),a
	call	fn045B
	ld	(16A8),a
	jp	1002

l104D:
	ld	a,(14FF)
	cp	a,00
	jp	nz,105E

l1055:
	ld	bc,0369
	call	fn05CE
	call	fn0387

l105E:
	ld	a,(16A8)
	add	a,5F
	sub	a,5B
	add	a,FF
	sbc	a,a
	push	af
	ld	a,(16A8)
	sub	a,00
	add	a,FF
	sbc	a,a
	pop	bc
	ld	c,b
	and	a,c
	rra
	jp	nc,1081

l1078:
	call	fn045B
	ld	(16A8),a
	jp	105E

l1081:
	call	fn045B
	ld	(16A8),a

l1087:
	ld	a,(16A8)
	add	a,5F
	sub	a,5D
	add	a,FF
	sbc	a,a
	push	af
	ld	a,(16A8)
	sub	a,00
	add	a,FF
	sbc	a,a
	pop	bc
	ld	c,b
	and	a,c
	rra
	jp	nc,11FD

l10A1:
	ld	a,(16A8)
	cp	a,20
	jp	nz,10AC

l10A9:
	call	fn1262

l10AC:
	ld	a,(16A8)
	cp	a,42
	jp	nz,10BC

l10B4:
	ld	hl,14F3
	ld	(hl),00
	jp	11B5

l10BC:
	ld	a,(16A8)
	cp	a,43
	jp	nz,10CD

l10C4:
	call	fn12D8
	ld	(151D),a
	jp	11B5

l10CD:
	ld	a,(16A8)
	cp	a,44
	jp	nz,10DE

l10D5:
	call	fn1279
	ld	(138B),a
	jp	11B5

l10DE:
	ld	a,(16A8)
	cp	a,46
	jp	nz,10EE

l10E6:
	ld	hl,14F3
	ld	(hl),04
	jp	11B5

l10EE:
	ld	a,(16A8)
	cp	a,49
	jp	nz,10FE

l10F6:
	ld	hl,14FA
	ld	(hl),01
	jp	11B5

l10FE:
	ld	a,(16A8)
	cp	a,4C
	jp	nz,110F

l1106:
	call	fn1279
	ld	(14F7),a
	jp	11B5

l110F:
	ld	a,(16A8)
	cp	a,4E
	jp	nz,111F

l1117:
	ld	hl,14FB
	ld	(hl),01
	jp	11B5

l111F:
	ld	a,(16A8)
	cp	a,4F
	jp	nz,112F

l1127:
	ld	hl,14EF
	ld	(hl),00
	jp	11B5

l112F:
	ld	a,(16A8)
	cp	a,50
	jp	nz,113F

l1137:
	ld	hl,14F3
	ld	(hl),01
	jp	11B5

l113F:
	ld	a,(16A8)
	cp	a,52
	jp	nz,1150

l1147:
	call	fn12D8
	ld	(1520),a
	jp	11B5

l1150:
	ld	a,(16A8)
	cp	a,53
	jp	nz,1160

l1158:
	ld	hl,14F0
	ld	(hl),01
	jp	11B5

l1160:
	ld	a,(16A8)
	cp	a,54
	jp	nz,1170

l1168:
	ld	hl,14F5
	ld	(hl),01
	jp	11B5

l1170:
	ld	a,(16A8)
	cp	a,55
	jp	nz,1180

l1178:
	ld	hl,1529
	ld	(hl),01
	jp	11B5

l1180:
	ld	a,(16A8)
	cp	a,56
	jp	nz,1190

l1188:
	ld	hl,14FC
	ld	(hl),01
	jp	11B5

l1190:
	ld	a,(16A8)
	cp	a,57
	jp	nz,11A1

l1198:
	call	fn1279
	ld	(14F8),a
	jp	11B5

l11A1:
	ld	a,(16A8)
	cp	a,58
	jp	nz,11B2

l11A9:
	call	fn12D8
	ld	(151E),a
	jp	11B5

l11B2:
	call	fn1229

l11B5:
	call	fn045B
	ld	(16A8),a
	call	fn1262
	ld	a,(16A8)
	cp	a,2C
	jp	nz,11CF

l11C6:
	call	fn045B
	ld	(16A8),a
	jp	11FA

l11CF:
	ld	a,(16A8)
	add	a,5F
	cp	a,5D
	jp	nz,11FA

l11D9:
	call	fn045B
	ld	(16A8),a
	call	fn1262
	ld	a,(16A8)
	add	a,5F
	cp	a,5B
	jp	z,11F4

l11EC:
	ld	hl,16A8
	ld	(hl),5D
	jp	11FA

l11F4:
	call	fn045B
	ld	(16A8),a

l11FA:
	jp	1087

l11FD:
	ld	a,(14F3)
	cp	a,01
	jp	nz,120F

l1205:
	ld	c,0C
	call	fn056B
	ld	hl,14F9
	ld	(hl),01

l120F:
	ld	a,(14F3)
	ld	(14F4),a
	ld	a,(14F7)
	inc	a
	ld	(14FD),a
	ld	hl,138A
	ld	(hl),01
	call	fn07B3
	call	fn0BE4
	ei
	hlt

;; fn1229: 1229
;;   Called from:
;;     11B2 (in fn100A)
;;     128A (in fn1279)
;;     12D1 (in fn1279)
;;     12F4 (in fn12D8)
;;     130E (in fn12D8)
fn1229 proc
	ld	hl,14F3
	ld	(hl),02
	ld	bc,02D1
	call	fn05CE
	ld	a,(16A8)
	cp	a,00
	jp	z,125E

l123C:
	ld	c,3E
	call	fn056B
	ld	c,20
	call	fn056B

l1246:
	ld	a,(16A8)
	cp	a,00
	jp	z,125E

l124E:
	ld	hl,(16A8)
	ld	c,l
	call	fn056B
	call	fn045B
	ld	(16A8),a
	jp	1246

l125E:
	call	fn0387
	ret

;; fn1262: 1262
;;   Called from:
;;     10A9 (in fn100A)
;;     11BB (in fn100A)
;;     11DF (in fn100A)
;;     127F (in fn1279)
;;     12C6 (in fn1279)
;;     12DE (in fn12D8)
fn1262 proc
	ld	a,(16A8)
	cp	a,20
	jp	nz,1278

l126A:
	call	fn045B
	ld	(16A8),a
	cp	a,20
	jp	nz,1278

l1275:
	jp	126A

l1278:
	ret

;; fn1279: 1279
;;   Called from:
;;     10D5 (in fn100A)
;;     1106 (in fn100A)
;;     1198 (in fn100A)
fn1279 proc
	call	fn045B
	ld	(16A8),a
	call	fn1262
	ld	a,(16A8)
	cp	a,28
	jp	z,128D

l128A:
	call	fn1229

l128D:
	ld	hl,16AA
	ld	(hl),00

l1292:
	call	fn045B
	ld	(16A9),a
	sub	a,30
	ld	c,a
	ld	a,09
	cp	a,c
	jp	c,12BE

l12A1:
	ld	a,(16AA)
	add	a,a
	add	a,a
	add	a,a
	push	af
	ld	a,(16AA)
	add	a,a
	pop	bc
	ld	c,b
	add	a,c
	push	af
	ld	a,(16A9)
	add	a,0F
	pop	bc
	ld	c,b
	add	a,c
	ld	(16AA),a
	jp	1292

l12BE:
	ld	a,(16A9)
	cp	a,20
	jp	nz,12C9

l12C6:
	call	fn1262

l12C9:
	ld	a,(16A9)
	cp	a,29
	jp	z,12D4

l12D1:
	call	fn1229

l12D4:
	ld	a,(16AA)
	ret

;; fn12D8: 12D8
;;   Called from:
;;     10C4 (in fn100A)
;;     1147 (in fn100A)
;;     11A9 (in fn100A)
fn12D8 proc
	call	fn045B
	ld	(16A8),a
	call	fn1262
	ld	a,(16A8)
	cp	a,28
	jp	nz,12F4

l12E9:
	call	fn045B
	add	a,1F
	ld	(16AB),a
	jp	12F7

l12F4:
	call	fn1229

l12F7:
	call	fn045B
	sub	a,29
	add	a,FF
	sbc	a,a
	push	af
	ld	a,10
	ld	hl,16AB
	sub	a,(hl)
	sbc	a,a
	pop	bc
	ld	c,b
	or	a,c
	rra
	jp	nc,1311

l130E:
	call	fn1229

l1311:
	ld	a,(16AB)
	ret

;; fn1315: 1315
fn1315 proc
	ex	de,hl
	ld	e,a
	ld	d,00
	ex	de,hl
	ld	a,(de)
	add	a,l
	ld	l,a
	inc	de
	ld	a,(de)
	adc	a,h
	ld	h,a
	ret
1322       5E 23 56 EB                                 ^#V.          

;; fn1326: 1326
;;   Called from:
;;     049E (in fn0483)
;;     0C45 (in fn0BE4)
fn1326 proc
	ld	a,h
	or	a,a
	rra
	ld	h,a
	ld	a,l
	rra
	ld	l,a
	dec	c
	jp	nz,fn1326

l1331:
	ret
1332       5F 16 00                                    _..           

;; fn1335: 1335
;;   Called from:
;;     054F (in fn0534)
fn1335 proc
	ld	a,e
	sub	a,l
	ld	l,a
	ld	a,d
	sbc	a,h
	ld	h,a
	ret

;; fn133C: 133C
fn133C proc
	ld	c,a
	ld	b,00
	ld	a,e
	sub	a,c
	ld	l,a
	ld	a,d
	sbc	a,b
	ld	h,a
	ret

;; fn1346: 1346
;;   Called from:
;;     0C40 (in fn0BE4)
fn1346 proc
	ld	l,c
	ld	h,b

;; fn1348: 1348
;;   Called from:
;;     0672 (in fn063E)
;;     0685 (in fn063E)
;;     1347 (in fn1346)
fn1348 proc
	ld	c,(hl)
	inc	hl
	ld	b,(hl)
	ld	a,(de)
	sub	a,c
	ld	l,a
	inc	de
	ld	a,(de)
	sbc	a,b
	ld	h,a
	ret

;; fn1353: 1353
;;   Called from:
;;     079A (in fn075C)
;;     0E7E (in fn0E63)
fn1353 proc
	ld	l,a
	ld	h,00

;; fn1356: 1356
;;   Called from:
;;     1354 (in fn1353)
fn1356 proc
	ld	a,(de)
	sub	a,l
	ld	l,a
	inc	de
	ld	a,(de)
	sbc	a,h
	ld	h,a
	ret
135E                                           00 00               ..
1360 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
1400 00 00 00 00 00 00 00 00 00 00 00                ...........     

;; fn140B: 140B
fn140B proc
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	ld	bc,0000
	nop
	ld	(bc),a
	nop
	nop
	ld	bc,5042
	nop
	nop
	nop
	nop
	nop
	ld	bc,0000
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop

;; fn156F: 156F
;;   Called from:
;;     156E (in fn140B)
fn156F proc
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop

;; fn15B0: 15B0
;;   Called from:
;;     15AF (in fn156F)
fn15B0 proc
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	inc	bc
	ld	l,a
	hlt
1645                30 05 00 00 00 00 00 05 00 1A 1A      0..........
1650 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A ................
1660 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A ................
1670 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A 1A ................
1680 CD EC 08 DA 94 14 CD 0B 14 2A 52 17 EB 3E 82 B7 .........*R..>..
1690 37 C3 2A 09 3A 25 17 B7 C2 39 15 21 26 17 0E 08 7.*.:%...9.!&...
16A0 E5 7E B7 CA F6 14 23 7E 23 56 2A E5 17 BD C2 F6 .~....#~#V*.....
16B0 14 7A BC C2 F6 14 E1 7E 3D C2 C4 14 F5 3D 32 25 .z.....~=....=2%
16C0 17 C3 DB 14 77 F5 CD 6F 15 FE 02 CA DB 14 3A 4C ....w..o......:L
16D0 17 B7 CA DB 14 CD B0 15 C3 2A 09 CD 55 10 F1 3C .........*..U..<
16E0 CD 45 10 21 68 15 CD 0A 10 2A E5 17 EB CD 6E 10 .E.!h....*....n.
16F0 CD 0B 13 C3 2A 09 E1 11 04 00 19 0D C2 A0 14 CD ....*...........
