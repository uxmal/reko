;;; Segment code (0100)

;; CpmCom_Start: 0100
CpmCom_Start proc
	nop
	nop
	nop
	ld	sp,0DA2
	call	fn0DA2
	ld	hl,(0001)
	ld	l,00
	ld	de,0A1E
	ld	bc,002D

l0114:
	ldir
0116                   FD 21 BC 0A CD C5 09 CD 3B 09       .!......;.

l0120:
	ld	hl,0120
	push	hl
	call	fn093B
	call	fn0945
	call	fn0A08
	cp	a,48
	jp	z,0299

l0132:
	cp	a,45
	jp	z,0873

l0137:
	cp	a,4D
	jp	z,08D9

l013C:
	ld	ix,0AB3
	cp	a,42
	jr	z,0186

l0144:
	cp	a,43
	jp	z,01BC

l0149:
	cp	a,47
	jp	z,01AF

l014E:
	cp	a,54
	jr	z,015D

l0152:
	cp	a,20
	jr	z,016A

l0156:
	ld	c,3F
	call	fn0A2A
	jr	0120

l015D:
	ld	iy,0B30
	call	fn09C5
	call	fn094F
	ld	a,e
	jr	nz,016C

l016A:
	ld	a,01

l016C:
	ld	(0AAC),a
	call	fn02C5
	call	fn01FF
	ld	a,(0AAC)
	dec	a
	ret	z

l017A:
	push	af
	ld	bc,0AAE

l017E:
	dec	bc
	ld	a,b
	or	a,c
	jr	nz,017E

l0183:
	pop	af
	jr	016C

l0186:
	ld	iy,0B41
	call	fn09C5
	call	fn094F
	ld	(0AB0),de

l0194:
	call	fn02C5
	ld	de,(0AB0)
	ld	a,h
	cp	a,d
	jr	nz,01A8

l019F:
	ld	a,l
	cp	a,e
	jr	nz,01A8

l01A3:
	call	fn02C5
	jr	fn01FF

l01A8:
	call	fn0A24
	jr	z,0194

l01AD:
	jr	01B7

l01AF:
	call	fn02C5
	call	fn0A24
	jr	z,01AF

l01B7:
	call	fn0A27
	jr	fn01FF

l01BC:
	push	ix
	call	fn093B
	ld	iy,0B00
	call	fn027A
	ld	de,(0ABA)
	call	fn099E
	call	fn0945
	call	fn094F
	jr	z,01DB

l01D7:
	ld	(0ABA),de

l01DB:
	ld	iy,0AE2
	ld	b,05

l01E1:
	call	fn093B
	call	fn027A
	ld	e,(ix)
	call	fn09A2
	call	fn0945
	call	fn094F
	jr	z,01F8

l01F5:
	ld	(ix),e

l01F8:
	inc	ix
	djnz	01E1

l01FC:
	pop	ix
	ret

;; fn01FF: 01FF
;;   Called from:
;;     0172 (in CpmCom_Start)
;;     01A6 (in CpmCom_Start)
;;     01BA (in CpmCom_Start)
;;     033D (in fn02C5)
fn01FF proc
	push	ix
	push	bc
	call	fn093B
	ld	iy,0AD6
	call	fn027A
	ld	de,(0AB8)
	call	fn099E
	call	fn027A
	pop	bc
	push	iy
	push	bc
	ld	iy,0C02
	ld	a,FC
	and	a,c
	srl	a
	ld	e,a
	ld	d,00
	add	iy,de
	ld	e,(iy)
	ld	d,(iy+01)
	sla	e
	rl	d
	ld	c,03

l0234:
	xor	a,a
	ld	b,05

l0237:
	sla	e
	rl	d
	rla
	djnz	0237

l023E:
	set	06,a
	push	bc
	call	fn0A19
	pop	bc
	dec	c
	jr	nz,0234

l0248:
	pop	bc
	ld	a,b
	sla	a
	sla	a
	sla	a
	ld	e,a
	ld	d,00
	ld	iy,0C72
	add	iy,de
	call	fn027F
	pop	iy
	ld	b,05

l0260:
	call	fn027A
	ld	e,(ix)
	call	fn09A2
	inc	ix
	djnz	0260

l026D:
	call	fn027A
	ld	de,(0ABA)
	call	fn099E
	pop	ix
	ret

;; fn027A: 027A
;;   Called from:
;;     01C5 (in CpmCom_Start)
;;     01E4 (in CpmCom_Start)
;;     0209 (in fn01FF)
;;     0213 (in fn01FF)
;;     0260 (in fn01FF)
;;     026D (in fn01FF)
fn027A proc
	ld	e,06
	jp	fn09C7

;; fn027F: 027F
;;   Called from:
;;     0259 (in fn01FF)
fn027F proc
	ld	e,08
	jp	fn09C7

;; fn0284: 0284
;;   Called from:
;;     02CB (in fn02C5)
fn0284 proc
	push	hl
	ld	hl,0A98
	ld	de,0A9A
	ld	bc,004E

l028E:
	lddr
0290 D1 21 4B 0A 73 23 72 EB C9                      .!K.s#r..       

l0299:
	call	fn093B
	ld	hl,0A4B
	ld	bc,0028

l02A2:
	ld	a,b
	or	a,c
	jr	z,02C1

l02A6:
	ld	e,(hl)
	inc	hl
	ld	d,(hl)
	inc	hl
	push	hl
	push	bc
	call	fn099E
	ld	a,20
	call	fn0A19
	pop	bc
	push	bc
	ld	a,c
	add	a,07
	call	z,fn093B

l02BC:
	pop	bc
	pop	hl
	dec	bc
	jr	02A2

l02C1:
	call	fn093B
	ret

;; fn02C5: 02C5
;;   Called from:
;;     016F (in CpmCom_Start)
;;     0194 (in CpmCom_Start)
;;     01A3 (in CpmCom_Start)
;;     01AF (in CpmCom_Start)
fn02C5 proc
	call	fn0E34
	ld	hl,(0ABA)
	call	fn0284
	ld	(0AB8),hl
	call	fn0E24
	ld	b,a
	or	a,a
	jp	z,032E

l02D9:
	bit	00,a
	jp	nz,0560

l02DE:
	add	a,0F
	jr	nz,02FA

l02E2:
	ld	a,b
	bit	04,a
	jp	nz,0361

l02E8:
	cp	a,20
	jp	z,039F

l02ED:
	cp	a,40
	jp	z,0424

l02F2:
	cp	a,60
	jp	z,0437

l02F7:
	jp	0560

l02FA:
	cp	a,08
	jr	nz,030C

l02FE:
	ld	a,b
	bit	04,a
	jp	z,046B

l0304:
	cp	a,98
	jp	z,0523

l0309:
	jp	0530

l030C:
	cp	a,0A
	jr	nz,0320

l0310:
	ld	a,b
	add	a,F0
	bit	04,a
	jp	nz,0450

l0318:
	cp	a,70
	jp	c,0560

l031D:
	jp	046B

l0320:
	ld	a,b
	cp	a,4C
	jp	z,03C3

l0326:
	cp	a,6C
	jp	z,03C9

l032B:
	jp	0560

l032E:
	set	04,(ix+01)
	ld	hl,(0AAA)
	ld	a,h
	or	a,l
	jr	nz,0340

l0339:
	pop	bc
	ld	bc,0429
	jp	fn01FF

l0340:
	inc	hl
	call	fn0755
	ld	a,d
	call	fn0E2C
	dec	hl
	ld	a,e
	call	fn0E2C
	dec	hl
	ld	a,(ix+01)
	call	fn0E2C
	dec	hl
	call	fn075C
	ld	hl,(0AAA)
	ld	bc,0429
	jp	0728

l0361:
	ld	b,00
	call	fn0E24
	ld	c,a
	ld	iy,0B96
	srl	c
	srl	c
	srl	c
	srl	c
	dec	c
	add	iy,bc
	inc	hl
	bit	01,c
	ld	a,(ix+01)
	ld	c,(iy)
	ld	b,(iy+01)
	jr	nz,0389

l0384:
	and	a,b
	jr	z,038E

l0387:
	jr	039A

l0389:
	and	a,b
	jr	nz,038E

l038C:
	jr	039A

l038E:
	ld	d,00
	call	fn0E24
	ld	e,a
	bit	07,e
	jr	z,0399

l0398:
	dec	d

l0399:
	add	hl,de

l039A:
	ld	b,00
	jp	0727

l039F:
	inc	hl
	call	fn03E9
	jr	nz,03AA

l03A5:
	call	fn03FD
	jr	03BD

l03AA:
	push	de
	call	fn0755
	ld	a,d
	call	fn0E2C
	dec	hl
	ld	a,e
	call	fn0E2C
	dec	hl
	call	fn075C
	pop	de
	ex	de,hl

l03BD:
	ld	bc,0273
	jp	0728

l03C3:
	inc	hl
	ld	bc,026F
	jr	03D7

l03C9:
	inc	hl
	call	fn0E24
	ld	e,a
	inc	hl
	call	fn0E24
	ld	d,a
	ex	de,hl
	ld	bc,0C6F

l03D7:
	call	fn03E9
	jr	nz,03E5

l03DC:
	push	bc
	call	fn03FD
	call	fn0440
	pop	bc
	ex	de,hl

l03E5:
	ex	de,hl
	jp	0728

;; fn03E9: 03E9
;;   Called from:
;;     03A0 (in fn02C5)
;;     03D7 (in fn02C5)
fn03E9 proc
	call	fn0E24
	ld	e,a
	inc	hl
	call	fn0E24
	ld	d,a
	inc	hl
	ld	a,00
	or	a,d
	ret	nz

l03F7:
	ld	a,04
	cp	a,e
	ret	c

l03FB:
	xor	a,a
	ret

;; fn03FD: 03FD
;;   Called from:
;;     03A5 (in fn02C5)
;;     03DD (in fn02C5)
fn03FD proc
	push	bc
	ld	d,00
	ld	a,e
	add	a,e
	add	a,e
	ld	e,a
	ld	iy,0A9B
	ld	bc,0419
	push	bc
	add	iy,de
	ld	a,(ix)
	ld	b,(ix+02)
	ld	c,(ix+03)
	jp	(iy-23)
041A                               77 00 DD 70 02 DD           w..p..
0420 71 03 C1 C9                                     q...            

l0424:
	call	fn0755
	inc	hl
	call	fn0E24
	ld	(ix+01),a
	call	fn0443
	ld	bc,04A5
	jp	0728

l0437:
	call	fn0440
	ld	bc,04A9
	jp	0728

;; fn0440: 0440
;;   Called from:
;;     03E0 (in fn02C5)
;;     0437 (in fn02C5)
fn0440 proc
	call	fn0755

;; fn0443: 0443
;;   Called from:
;;     042E (in fn02C5)
fn0443 proc
	inc	hl
	call	fn0E24
	ld	e,a
	inc	hl
	call	fn0E24
	ld	d,a
	jp	fn075C

l0450:
	cp	a,90
	jr	nz,0460

l0454:
	ld	a,(ix+02)
	ld	(ix+04),a
	ld	bc,04D9
	jp	0727

l0460:
	ld	a,(ix+04)
	ld	(ix+02),a
	ld	c,D1
	jp	052B

l046B:
	ld	c,a
	srl	c
	srl	c
	srl	c
	srl	c
	add	a,0F
	cp	a,08
	ld	iy,0BB6
	jp	z,0864

l047F:
	ld	iy,0BBE
	jp	0864
0486                   DD 34 02 0E 65 18 13 DD 34 03       .4..e...4.
0490 0E 69 18 0C DD 35 02 0E 55 18 05 DD 35 03 0E 59 .i...5..U...5..Y
04A0 DD CB 01 8E 20 04 DD CB 01 CE DD CB 01 BE F2 5B .... ..........[
04B0 05 DD CB 01 FE C3 5B 05 01 85 04 C3 27 07 CD 55 ......[.....'..U
04C0 07 DD 7E 00 0E 8D 18 08 CD 55 07 DD 7E 01 0E 91 ..~......U..~...
04D0 C5 CD 2C 0E 2B CD 5C 07 C1 C3 5B 05 CD FD 04 DD ..,.+.\...[.....
04E0 77 00 CD 3E 07 CD 5C 07 01 95 04 C3 27 07 CD FD w..>..\.....'...
04F0 04 DD 77 01 CD 5C 07 01 99 04 C3 27 07 CD 55 07 ..w..\.....'..U.
0500 23 CD 24 0E C9 DD 7E 00 DD 77 02 0E C9 18 1C DD #.$...~..w......
0510 7E 00 DD 77 03 0E CD 18 12 DD 7E 02 DD 77 00 0E ~..w......~..w..
0520 D5 18 08                                        ...             

l0523:
	ld	a,(ix+03)
	ld	(ix),a
	ld	c,DD

l052B:
	call	fn073E
	jr	055B

l0530:
	ld	iy,0BA6
	ld	b,00
	cp	a,B8
	jr	nz,053C

l053A:
	res	05,a

l053C:
	rra
	rra
	rra
	rra
	add	a,0E
	bit	01,a
	ld	c,a
	add	iy,bc
	ld	c,(iy)
	ld	a,(iy+01)
	jr	nz,0555

l054F:
	cpl
	and	a,(ix+01)
	jr	0558

l0555:
	or	a,(ix+01)

l0558:
	ld	(ix+01),a

l055B:
	ld	b,04
	jp	0727

l0560:
	ld	bc,071E
	push	bc
	call	fn0E24
	ld	c,a
	srl	c
	push	af
	srl	c
	srl	c
	srl	c
	res	00,c
	pop	af
	ld	iy,0BE2
	jp	nc,0864

l057B:
	ld	iy,0BF2
	jp	0864
0582       CD 67 07 CB 27 CD 34 07 CD 4D 08 3E 08 C9   .g..'.4..M.>..
0590 CB 4F 28 10 CD 67 07 CD 2C 07 17 CD 34 07 CD 4D .O(..g..,...4..M
05A0 08 3E 9C C9 DD CB 01 B6 CD 7F 07 C5 47 DD 7E 00 .>..........G.~.
05B0 A0 CB 77 28 04 DD CB 01 F6 CD 3E 07 C1 3E 18 C9 ..w(......>..>..
05C0 CD 67 07 CB 3F CD 34 07 CD 4D 08 3E 80 C9 CD 67 .g..?.4..M.>...g
05D0 07 CD 2C 07 1F CD 34 07 CD 4D 08 3E A0 C9 CB 4F ..,...4..M.>...O
05E0 28 0C CD 7F 07 DD 7E 02 CD 4D 08 3E C0 C9 CD 7F (.....~..M.>....
05F0 07 DD 7E 03 CD 4D 08 3E C4 C9 CB 4F 28 0C CD 6D ..~..M.>...O(..m
0600 07 DD 77 02 CD 3E 07 3E 78 C9 CD 6D 07 DD 77 03 ..w..>.>x..m..w.
0610 CD 3E 07 3E 7C C9 CB 4F 28 27 CD 7F 07 3D CD 3E .>.>|..O('...=.>
0620 07 CD 4D 08 3E 50 C9 CB 4F 28 0D CD 7F 07 3C CD ..M.>P..O(....<.
0630 3E 07 CD 4D 08 3E 60 C9 CD 4E 06 CD 34 07 3E 48 >..M.>`..N..4.>H
0640 C9 DD 23 CD 4E 06 DD 2B CD 34 07 3E 4C C9 CD 90 ..#.N..+.4.>L...
0650 07 C5 47 DD 7E 02 90 3F C1 C9 DD CB 01 B6 CD 61 ..G.~..?.......a
0660 07 C5 47 DD 7E 00 CD 2C 07 3F DD CB 01 5E 20 03 ..G.~..,.?...^ .
0670 98 18 02 98 27 DD 77 00 3F CD 34 07 E6 C0 28 08 ....'.w.?.4...(.
0680 FE C0 28 04 DD CB 01 F6 C1 3E AC C9 CD 61 07 C5 ..(......>...a..
0690 47 DD 7E 00 B0 DD 77 00 CD 3E 07 C1 3E 88 C9 CD G.~...w..>..>...
06A0 61 07 C5 47 DD 7E 00 A0 DD 77 00 CD 3E 07 C1 3E a..G.~...w..>..>
06B0 04 C9 CD 61 07 C5 47 DD 7E 00 A8 DD 77 00 CD 3E ...a..G.~...w..>
06C0 07 C1 3E 5C C9 DD CB 01 B6 CD 61 07 C5 47 DD 7E ..>\......a..G.~
06D0 00 CD 2C 07 DD CB 01 5E 20 03 88 18 02 88 27 DD ..,....^ .....'.
06E0 77 00 CD 34 07 E6 C0 28 08 FE C0 28 04 DD CB 01 w..4...(...(....
06F0 F6 C1 3E 00 C9 CD 61 07 DD 7E 00 CD 4D 08 3E BC ..>...a..~..M.>.
0700 C9 CD 61 07 DD 77 00 CD 3E 07 3E 74 C9 CD 61 07 ..a..w..>.>t..a.
0710 C5 47 DD 7E 00 90 3F CD 34 07 C1 3E 44 C9 F5 3E .G.~..?.4..>D..>
0720 00 32 B2 0A F1 81 4F                            .2....O         

l0727:
	inc	hl

l0728:
	ld	(0ABA),hl
	ret
072C                                     37 DD CB 01             7...
0730 46 C0 3F C9 DD CB 01 86 30 04 DD CB 01 C6       F.?.....0.....  

;; fn073E: 073E
;;   Called from:
;;     052B (in fn02C5)
fn073E proc
	res	01,(ix+01)
	or	a,a
	jr	nz,0749

l0745:
	set	01,(ix+01)

l0749:
	res	07,(ix+01)
	bit	07,a
	ret	z

l0750:
	set	07,(ix+01)
	ret

;; fn0755: 0755
;;   Called from:
;;     0341 (in fn02C5)
;;     03AB (in fn02C5)
;;     0424 (in fn02C5)
;;     0440 (in fn0440)
fn0755 proc
	ld	d,01
	ld	e,(ix+04)
	ex	de,hl
	ret

;; fn075C: 075C
;;   Called from:
;;     0355 (in fn02C5)
;;     03B8 (in fn02C5)
;;     044D (in fn0440)
;;     044D (in fn0443)
fn075C proc
	ex	de,hl
	ld	(ix+04),e
	ret
0761    FD 21 54 0B 18 2D FD 21 64 0B 18 27 FD 21 74  .!T..-.!d..'.!t
0770 0B FE B6 20 02 3E 10 FE BE 20 19 3E 18 18 15 FD ... .>... .>....
0780 21 84 0B FE 96 28 05 1F E6 0C 18 08 3E 10 18 04 !....(......>...
0790 FD 21 8E 0B 1F E6 0E 06 00 4F FD 09 FD 4E 00 FD .!.......O...N..
07A0 46 01 C5 FD 21 CE 0B C3 64 08 23 CD 24 0E DD 86 F...!...d.#.$...
07B0 03 5F 23 CD 24 0E CE 00 57 C3 3C 08 23 CD 24 0E ._#.$...W.<.#.$.
07C0 18 7F 3E 01 32 B2 0A DD 7E 00 DD E5 D1 18 72 23 ..>.2...~.....r#
07D0 CD 24 0E 5F 16 00 18 64 23 CD 24 0E 5F 23 CD 24 .$._...d#.$._#.$
07E0 0E 57 18 58 DD 7E 02 18 03 DD 7E 03 23 C5 47 CD .W.X.~....~.#.G.
07F0 24 0E 80 C1 5F 16 00 18 43 23 CD 24 0E DD 86 02 $..._...C#.$....
0800 5F 16 00 FD 21 00 00 FD 19 FD 5E 00 FD 56 01 18 _...!.....^..V..
0810 2B 23 CD 24 0E 5F 16 00 FD 21 00 00 FD 19 FD 7E +#.$._...!.....~
0820 00 DD 86 03 5F FD 7E 01 CE 00 57 18 0F 23 CD 24 ...._.~...W..#.$
0830 0E DD 86 02 5F 23 CD 24 0E CE 00 57 EB CD 24 0E ...._#.$...W..$.
0840 EB C1 F5 3E 03 A0 4F CB 38 CB 38 F1 C9 EB F5 3A ...>..O.8.8....:
0850 B2 0A B7 28 09 3E 00 32 B2 0A F1 77 EB C9 F1 CD ...(.>.2...w....
0860 2C 0E EB C9                                     ,...            

l0864:
	ld	b,00
	add	iy,bc
	ld	c,(iy)
	ld	b,(iy+01)
	push	bc
	pop	iy
	jp	(iy+11)

l0873:
	ld	de,0B06
0874             06 0B CD BA 09 CD 4F 09 D5 CD 40 09     ......O...@.
0880 11 1D 0B CD BA 09 CD 4F 09 D5 C1 1E 04 AF       .......O......  

l088E:
	srl	b
	rr	c
	jr	nc,0895

l0894:
	inc	a

l0895:
	dec	e
	jr	nz,088E

l0898:
	or	a,a
	jr	z,089C

l089B:
	inc	bc

l089C:
	pop	hl
	dec	bc

l089E:
	push	bc
	call	fn093B
	call	fn0923
	call	fn0930
	push	hl
	ld	e,10

l08AB:
	call	fn0E24
	call	fn0929
	call	fn0933
	inc	hl
	dec	e
	jr	nz,08AB

l08B8:
	call	fn0930
	pop	hl
	ld	e,10

l08BE:
	call	fn0E24
	cp	a,20
	jr	c,08C9

l08C5:
	cp	a,80
	jr	c,08CB

l08C9:
	ld	a,2E

l08CB:
	call	fn0A19
	inc	hl
	dec	e
	jr	nz,08BE

l08D2:
	pop	bc
	ld	a,b
	or	a,c
	ret	z

l08D6:
	dec	bc
	jr	089E

l08D9:
	ld	de,0B06
	call	fn09BA
	call	fn094F
	ex	de,hl

l08E3:
	call	fn093B
	call	fn0923
	call	fn0930
	call	fn0E24
	call	fn0929
	call	fn0930

l08F5:
	call	fn0A08
	cp	a,2E
	ret	z

l08FB:
	cp	a,20
	jr	z,0920

l08FF:
	call	fn0975
	jr	z,08F5

l0904:
	push	hl
	ld	hl,0000

l0908:
	call	fn0997

l090B:
	call	fn0A08
	cp	a,0D
	jr	z,0919

l0912:
	call	fn0975
	jr	z,090B

l0917:
	jr	0908

l0919:
	push	hl
	pop	bc
	pop	hl
	ld	a,c
	call	fn0E2C

l0920:
	inc	hl
	jr	08E3

;; fn0923: 0923
;;   Called from:
;;     08A2 (in CpmCom_Start)
;;     08E6 (in CpmCom_Start)
fn0923 proc
	ex	de,hl
	call	fn099E
	ex	de,hl
	ret

;; fn0929: 0929
;;   Called from:
;;     08AE (in CpmCom_Start)
;;     08EF (in CpmCom_Start)
fn0929 proc
	push	de
	ld	e,a
	call	fn09A2
	pop	de
	ret

;; fn0930: 0930
;;   Called from:
;;     08A5 (in CpmCom_Start)
;;     08B8 (in CpmCom_Start)
;;     08E9 (in CpmCom_Start)
;;     08F2 (in CpmCom_Start)
fn0930 proc
	call	fn0933

;; fn0933: 0933
;;   Called from:
;;     08B1 (in CpmCom_Start)
;;     0930 (in fn0930)
;;     0930 (in fn0930)
fn0933 proc
	push	bc
	ld	c,20
	call	fn0A2A
	pop	bc
	ret

;; fn093B: 093B
;;   Called from:
;;     011D (in CpmCom_Start)
;;     0124 (in CpmCom_Start)
;;     0124 (in CpmCom_Start)
;;     01BE (in CpmCom_Start)
;;     01E1 (in CpmCom_Start)
;;     0202 (in fn01FF)
;;     0299 (in CpmCom_Start)
;;     02B9 (in CpmCom_Start)
;;     02C1 (in CpmCom_Start)
;;     089F (in CpmCom_Start)
;;     08E3 (in CpmCom_Start)
fn093B proc
	ld	c,0D
	call	fn0A2A

;; fn0940: 0940
;;   Called from:
;;     087D (in CpmCom_Start)
fn0940 proc
	ld	c,0A
	jp	fn0A2A

;; fn0945: 0945
;;   Called from:
;;     0127 (in CpmCom_Start)
;;     0127 (in CpmCom_Start)
;;     01CF (in CpmCom_Start)
;;     01ED (in CpmCom_Start)
fn0945 proc
	ld	c,20
	call	fn0A2A
	ld	c,3E
	jp	fn0A2A

;; fn094F: 094F
;;   Called from:
;;     0164 (in CpmCom_Start)
;;     018D (in CpmCom_Start)
;;     01D2 (in CpmCom_Start)
;;     01F0 (in CpmCom_Start)
;;     0879 (in CpmCom_Start)
;;     0886 (in CpmCom_Start)
;;     08DF (in CpmCom_Start)
fn094F proc
	call	fn0A08
	cp	a,20
	ret	z

l0955:
	ex	de,hl
	ld	hl,0000
	call	fn0975
	jr	z,fn094F

l095E:
	call	fn0997

l0961:
	call	fn0A08
	cp	a,0D
	jr	z,0972

l0968:
	call	fn0975
	jr	z,0961

l096D:
	call	fn0997
	jr	0961

l0972:
	or	a,a
	ex	de,hl
	ret

;; fn0975: 0975
;;   Called from:
;;     08FF (in CpmCom_Start)
;;     0912 (in CpmCom_Start)
;;     0959 (in fn094F)
;;     0968 (in fn094F)
fn0975 proc
	cp	a,30
	jr	c,0985

l0979:
	cp	a,3A
	jr	c,098C

l097D:
	cp	a,41
	jr	c,0985

l0981:
	cp	a,47
	jr	c,098C

l0985:
	ld	c,3F
	call	fn0A2A
	xor	a,a
	ret

l098C:
	sub	a,30
	cp	a,0A
	jr	c,0994

l0992:
	sub	a,07

l0994:
	cp	a,FF
	ret

;; fn0997: 0997
;;   Called from:
;;     0908 (in CpmCom_Start)
;;     095E (in fn094F)
;;     096D (in fn094F)
fn0997 proc
	add	hl,hl
	add	hl,hl
	add	hl,hl
	add	hl,hl
	or	a,l
	ld	l,a
	ret

;; fn099E: 099E
;;   Called from:
;;     01CC (in CpmCom_Start)
;;     0210 (in fn01FF)
;;     0274 (in fn01FF)
;;     02AC (in CpmCom_Start)
;;     0924 (in fn0923)
fn099E proc
	ld	a,d
	call	fn09A3

;; fn09A2: 09A2
;;   Called from:
;;     01EA (in CpmCom_Start)
;;     0266 (in fn01FF)
;;     092B (in fn0929)
;;     099F (in fn099E)
;;     09DF (in fn09C7)
;;     09EC (in fn09C7)
fn09A2 proc
	ld	a,e

;; fn09A3: 09A3
;;   Called from:
;;     099F (in fn099E)
fn09A3 proc
	push	af
	rrc	a
	rrc	a
	rrc	a
	rrc	a
	call	fn09B0
	pop	af

;; fn09B0: 09B0
;;   Called from:
;;     09AC (in fn09A2)
;;     09AC (in fn09A3)
;;     09AF (in fn09A2)
;;     09AF (in fn09A3)
fn09B0 proc
	add	a,0F
	add	a,90
	daa
	adc	a,40
	daa
	jr	fn0A19

;; fn09BA: 09BA
;;   Called from:
;;     0876 (in CpmCom_Start)
;;     0883 (in CpmCom_Start)
;;     08DC (in CpmCom_Start)
fn09BA proc
	push	iy
	push	de
	pop	iy
	call	fn09C5
	pop	iy
	ret

;; fn09C5: 09C5
;;   Called from:
;;     011A (in CpmCom_Start)
;;     0161 (in CpmCom_Start)
;;     018A (in CpmCom_Start)
;;     09BF (in fn09BA)
fn09C5 proc
	ld	e,00

;; fn09C7: 09C7
;;   Called from:
;;     027C (in fn027A)
;;     0281 (in fn027F)
;;     09C5 (in fn09C5)
fn09C7 proc
	ld	a,(iy)
	cp	a,5E
	ret	z

l09CD:
	cp	a,21
	jr	z,09F6

l09D1:
	push	de
	ld	hl,(0AB8)
	inc	hl
	cp	a,32
	jr	nz,09E4

l09DA:
	inc	hl
	call	fn0E24
	ld	e,a
	call	fn09A2
	jr	09F5

l09E4:
	cp	a,31
	jr	nz,09F1

l09E8:
	call	fn0E24
	ld	e,a
	call	fn09A2
	jr	09F5

l09F1:
	ld	c,a
	call	fn0A2A

l09F5:
	pop	de

l09F6:
	inc	iy
	dec	e
	jr	nz,fn09C7

l09FB:
	ret
09FC                                     C5 06 06 0E             ....
0A00 20 CD 2A 0A 10 FB C1 C9                          .*.....        

;; fn0A08: 0A08
;;   Called from:
;;     012A (in CpmCom_Start)
;;     012A (in CpmCom_Start)
;;     08F5 (in CpmCom_Start)
;;     090B (in CpmCom_Start)
;;     094F (in fn094F)
;;     0961 (in fn094F)
fn0A08 proc
	call	fn0A27
	cp	a,03
	jp	z,0A21

l0A10:
	cp	a,18
	jr	nz,fn0A19

l0A14:
	ld	sp,0DA0
	ld	a,23

;; fn0A19: 0A19
;;   Called from:
;;     0241 (in fn01FF)
;;     02B1 (in CpmCom_Start)
;;     08CB (in CpmCom_Start)
;;     09B8 (in fn09B0)
;;     0A12 (in fn0A08)
;;     0A17 (in fn0A08)
fn0A19 proc
	ld	c,a
	call	fn0A2A
	ret
0A1E                                           C3 00               ..
0A20 00                                              .               

l0A21:
	jp	0000

;; fn0A24: 0A24
;;   Called from:
;;     01A8 (in CpmCom_Start)
;;     01B2 (in CpmCom_Start)
fn0A24 proc
	jp	0000

;; fn0A27: 0A27
;;   Called from:
;;     01B7 (in CpmCom_Start)
;;     0A08 (in fn0A08)
fn0A27 proc
	jp	0000

;; fn0A2A: 0A2A
;;   Called from:
;;     0158 (in CpmCom_Start)
;;     0936 (in fn0933)
;;     093D (in fn093B)
;;     0942 (in fn093B)
;;     0942 (in fn0940)
;;     0947 (in fn0945)
;;     094C (in fn0945)
;;     0987 (in fn0975)
;;     09F2 (in fn09C7)
;;     0A1A (in fn0A19)
fn0A2A proc
	jp	0000
0A2D                                        C3 00 00              ...
0A30 C3 00 00 C3 00 00 C3 00 00 C3 00 00 C3 00 00 C3 ................
0A40 00 00 C3 00 00 C3 00 00 C3 00 00 00 00 00 00 00 ................
0A50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
0A90 00 00 00 00 00 00 00 00 00 00 00 C3 27 0A C3 19 ............'...
0AA0 0A C9 00 00 C9 00 00 C9 00 00 00 00 01 00 00 0A ................
0AB0 00 00 00 00 00 00 00 FF 00 10 00 10 2A 20 20 5A ............*  Z
0AC0 58 2F 36 35 20 36 35 78 78 20 53 49 4D 55 4C 41 X/65 65xx SIMULA
0AD0 54 4F 52 20 2A 5E 20 43 50 43 3A 20 20 49 4E 53 TOR *^ CPC:  INS
0AE0 3A 20 20 41 3A 2E 2E 2E 20 50 3A 2E 2E 2E 20 58 :  A:... P:... X
0AF0 3A 2E 2E 2E 20 59 3A 2E 2E 2E 20 53 50 3A 2E 2E :... Y:... SP:..
0B00 20 4E 50 43 3A 20 20 45 4E 54 45 52 20 53 54 41  NPC:  ENTER STA
0B10 52 54 20 41 44 44 52 45 53 53 3A 20 5E 20 45 4E RT ADDRESS: ^ EN
0B20 54 45 52 20 4E 4F 2E 20 42 59 54 45 53 3A 20 5E TER NO. BYTES: ^
0B30 20 45 4E 54 45 52 20 23 20 53 54 45 50 53 3A 20  ENTER # STEPS: 
0B40 5E 20 45 4E 54 45 52 20 42 52 4B 50 20 41 44 44 ^ ENTER BRKP ADD
0B50 52 3A 20 5E 0C 1A 04 0E 00 06 06 0B 0E 1E 08 22 R: ^..........."
0B60 12 2F 10 2B 00 00 04 0E 02 15 06 0B 0A 26 08 22 ./.+.........&."
0B70 12 2F 10 2B 00 06 04 0E 00 00 06 0B 0A 26 08 22 ./.+.........&."
0B80 12 2F 10 2B 04 0E 06 0B 08 22 10 2B 0A 26 00 06 ./.+.....".+.&..
0B90 04 0E 00 00 06 0B 26 80 1E 80 2E 40 32 40 0E 01 ......&....@2@..
0BA0 12 01 22 02 16 02 35 01 B1 01 3D 04 B9 04 41 40 .."...5...=...A@
0BB0 00 00 39 08 B5 08 C8 04 EE 04 BE 04 DC 04 9B 04 ..9.............
0BC0 0F 05 8D 04 86 04 19 05 05 05 94 04 B8 04 BC 07 ................
0BD0 C2 07 CF 07 D8 07 E4 07 E9 07 F9 07 11 08 2D 08 ..............-.
0BE0 AA 07 82 05 90 05 C0 05 CE 05 DE 05 FA 05 16 06 ................
0BF0 27 06 8C 06 9F 06 B2 06 C5 06 F5 06 01 07 0D 07 '...............
0C00 5A 06 83 04 C4 05 6C 06 63 08 73 08 B1 08 34 09 Z.....l.c.s...4.
0C10 A9 09 C5 09 0C 0A 4B 0A C3 0A D3 0A 83 0D 84 0D ......K.........
0C20 89 0D 96 0D B0 0D 18 0E 19 0E A3 10 B8 10 B9 10 ................
0C30 F2 15 C3 25 D8 25 D9 25 B0 29 72 2A 81 30 98 30 ...%.%.%.)r*.0.0
0C40 99 30 72 32 F0 39 41 3E 01 41 10 41 81 41 90 41 .0r2.9A>.A.A.A.A
0C50 EC 49 F2 49 89 4A 93 4A 43 4C A3 4C A4 4C A9 4C .I.I.J.JCL.L.L.L
0C60 81 4E 98 4E 99 4E 38 50 39 50 78 52 01 53 13 53 .N.N.N8P9PxR.S.S
0C70 21 53 20 24 31 20 20 20 20 21 20 23 24 31 20 20 !S $1    ! #$1  
0C80 20 21 20 24 32 31 20 20 21 21 20 24 31 20 20 20  ! $21  !! $1   
0C90 20 21 20 20 20 20 20 20 20 20 20 41 20 20 20 20  !         A    
0CA0 20 20 20 28 24 31 2C 58 29 21 20 28 24 31 29 2C    ($1,X)! ($1),
0CB0 59 21 20 24 31 2C 58 20 20 21 20 24 31 2C 59 20 Y! $1,X  ! $1,Y 
0CC0 20 21 20 24 32 31 2C 58 21 21 20 24 32 31 2C 59  ! $21,X!! $21,Y
0CD0 21 21 20 28 24 32 31 29 21 21 B6 17 45 CD B6 17 !! ($21)!!..E...
0CE0 C1 E1 1B 1A C9 E5 CD 54 0B E1 C4 CB 2F F5 11 16 .......T..../...
0CF0 3B 1A 3C 4F 1A 77 13 2B 0D C2 C1 2F F1 C9 FE 0D ;.<O.w.+.../....
0D00 C8 FE 3B C8 D1 C3 92 04 06 00 C5 E5 2B CD C7 0B ..;.........+...
0D10 FE 27 CA C6 30 FE 22 CA C6 30 FE 3C CA E4 30 FE .'..0."..0.<..0.
0D20 3E CA EF 30 FE 25 CA 1C 30 FE 2C CA F9 30 FE 20 >..0.%..0.,..0. 
0D30 CA F9 30 FE 09 CA F9 30 FE 3B CA F9 30 FE 21 CC ..0....0.;..0.!.
0D40 28 0B FE 0D CA 16 31 77 2B CD E9 0A C3 DD 2F 22 (.....1w+...../"
0D50 C4 30 C5 01 BA 30 C5 CD 62 26 EB 0E 00 3A 2A 3B .0...0..b&...:*;
0D60 FE 20 C2 7F 30 3A 20 3A FE 10 CA 77 30 FE 08 CA . ..0: :...w0...
0D70 57 30 01 F6 FF 50 58 09 13 DA 44 30 E5 EB 7C B5 W0...PX...D0..|.
0D80 C4 42 30 3E 3A C1 81 C3 AF 30 AF 29 CE 30 CD 9B .B0>:....0.).0..
0D90 30 3E 05 F5 AF 06 03 29 8F 05 C2 64 30 C6 30 CD 0>.....)...d0.0.
0DA0 9B 30                                           .0              

;; fn0DA2: 0DA2
;;   Called from:
;;     0106 (in CpmCom_Start)
fn0DA2 proc
	ld	a,(005D)
	cp	a,20
	ret	z

l0DA8:
	ld	de,006D
	ld	a,(de)
	cp	a,20
	jp	z,0DC9

l0DB1:
	ld	hl,0000

l0DB4:
	ld	a,(de)
	inc	de
	cp	a,20
	jp	z,0DC6

l0DBB:
	add	a,0F
	add	hl,hl
	add	hl,hl
	add	hl,hl
	add	hl,hl
	or	a,l
	ld	l,a
	jp	0DB4

l0DC6:
	ld	(0E7A),hl

l0DC9:
	ld	c,0F
	ld	de,005C
	call	0005
	inc	a
	jp	nz,0DDE

l0DD5:
	ld	de,0E40
	ld	c,09
	call	0005
	ret

l0DDE:
	ld	hl,(0E7A)
	ex	de,hl
	ld	hl,(0E78)
	add	hl,de
	ex	de,hl
	jp	0DEF

l0DEA:
	ld	a,01
	ld	(0E75),a

l0DEF:
	ld	hl,0080
	add	hl,de
	push	hl
	ld	c,1A
	call	0005
	ld	c,14
	ld	de,005C
	call	0005
	pop	de
	or	a,a
	jp	z,0DEA

l0E06:
	ld	de,0080
	ld	c,1A
	call	0005
	ld	c,10
	ld	de,005C
	call	0005
	ld	a,(0E75)
	or	a,a
	ret	nz

l0E1B:
	ld	de,0E5B
	ld	c,09
	call	0005
	ret

;; fn0E24: 0E24
;;   Called from:
;;     02D1 (in fn02C5)
;;     0363 (in fn02C5)
;;     0390 (in fn02C5)
;;     03CA (in fn02C5)
;;     03CF (in fn02C5)
;;     03E9 (in fn03E9)
;;     03EE (in fn03E9)
;;     0428 (in fn02C5)
;;     0444 (in fn0440)
;;     0444 (in fn0443)
;;     0449 (in fn0440)
;;     0449 (in fn0443)
;;     0564 (in fn02C5)
;;     08AB (in CpmCom_Start)
;;     08BE (in CpmCom_Start)
;;     08EC (in CpmCom_Start)
;;     09DB (in fn09C7)
;;     09E8 (in fn09C7)
fn0E24 proc
	call	fn0E35
	ld	a,(hl)
	ld	hl,(0E76)
	ret

;; fn0E2C: 0E2C
;;   Called from:
;;     0345 (in fn02C5)
;;     034A (in fn02C5)
;;     0351 (in fn02C5)
;;     03AF (in fn02C5)
;;     03B4 (in fn02C5)
;;     091D (in CpmCom_Start)
fn0E2C proc
	call	fn0E35
	ld	(hl),a
	ld	hl,(0E76)
	ret

;; fn0E34: 0E34
;;   Called from:
;;     02C5 (in fn02C5)
fn0E34 proc
	ret

;; fn0E35: 0E35
;;   Called from:
;;     0E24 (in fn0E24)
;;     0E2C (in fn0E2C)
fn0E35 proc
	ld	(0E76),hl
	push	de
	ex	de,hl
	ld	hl,(0E78)
	add	hl,de
	pop	de
	ret
0E40 0D 0A 2A 2A 2A 20 46 49 4C 45 20 4E 4F 54 20 46 ..*** FILE NOT F
0E50 4F 55 4E 44 20 2A 2A 2A 0D 0A 24 0D 0A 2A 2A 2A OUND ***..$..***
0E60 20 46 49 4C 45 20 49 53 20 45 4D 50 54 59 20 2A  FILE IS EMPTY *
0E70 2A 2A 0D 0A 24 00 00 00 7C 0E 00 10 5C 00 CD 05 **..$...|...\...
