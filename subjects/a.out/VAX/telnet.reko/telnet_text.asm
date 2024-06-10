;;; Segment .text (00000400)
00000400 00 0F 11 3D                                     ...=            

;; fn00000404: 00000404
fn00000404 proc
	movl	sp,r10
	addl3	#00000004,r10,r0
	movl	r0,r9
	movl	r9,r8

l00000411:
	tstl	(r9)+
	beql	00000417

l00000415:
	brb	00000411

l00000417:
	cmpl	r9,(r8)
	blss	0000041F

l0000041C:
	subl2	#00000004,r9

l0000041F:
	movl	r9,00003C04
	pushl	00003C04
	pushl	r8
	pushl	(r10)
	calls	#03,0000044C
	pushl	r0
	calls	#01,00003500
	ret
00000441    11 C1 00 00 00 11 01 04 11 FD 00 00 00        .............  

;; fn0000044E: 0000044E
;;   Called from:
;;     00000430 (in fn00000404)
fn0000044E proc
	pushal	00003E93
	pushal	00003E8C
	calls	#02,00001B60
	movl	r0,00005128
	tstl	r0
	bneq	00000488

l0000046C:
	pushal	00003E97
	pushal	000043FC
	calls	#02,000016A4
	pushl	#00000001
	calls	#01,00003500

l00000488:
	pushal	0000513C
	pushl	#40067408
	pushl	#00000000
	calls	#03,00003584
	pushal	0000512C
	pushl	#40067412
	pushl	#00000000
	calls	#03,00003584
	pushal	00005134
	pushl	#40067474
	pushl	#00000000
	calls	#03,00003584
	pushl	#00000000
	pushal	000043D4
	calls	#02,00001704
	pushl	#00000000
	pushal	000043E8
	calls	#02,00001704
	movl	+08(ap),r0
	movl	(r0),00004FA4
	cmpl	+04(ap),#00000001
	bleq	0000051C

l000004F6:
	pushal	00003EBC
	movl	+08(ap),r0
	pushl	+04(r0)
	calls	#02,00003514
	tstl	r0
	bneq	0000051C

l0000050E:
	movl	#00000001,00003CE4
	addl2	#00000004,+08(ap)
	decl	+04(ap)

l0000051C:
	cmpl	+04(ap),#00000001
	beql	00000549

l00000522:
	pushal	000050C8
	calls	#01,00001744
	tstl	r0
	beql	0000053C

l00000533:
	pushl	#00000000
	calls	#01,00003500

l0000053C:
	pushl	+08(ap)
	pushl	+04(ap)
	calls	#02,00000562

l00000549:
	pushal	000050C8
	calls	#01,00001744

l00000556:
	pushl	#00000001
	calls	#01,00000D3E
	brb	00000556
00000561    00 00 0C                                      ...            

;; fn00000564: 00000564
;;   Called from:
;;     00000542 (in fn0000044E)
fn00000564 proc
	tstl	00004F98
	beql	00000580

l0000056C:
	pushl	00005144
	pushal	00003EBF

l00000578:
	calls	#02,000016C8
	ret

l00000580:
	cmpl	+04(ap),#00000002
	bgeq	000005D8

l00000586:
	pushal	00003ED9
	pushal	00004FA8
	calls	#02,000035BC
	pushal	00003EE2
	calls	#01,000016C8
	pushal	00004FA8
	calls	#01,000029B0
	pushab	00004FA8[r0]
	calls	#01,0000162C
	calls	#00,0000081E
	movl	00005070,+04(ap)
	moval	00005078,+08(ap)

l000005D8:
	cmpl	+04(ap),#00000003
	bleq	000005EC

l000005DE:
	movl	+08(ap),r0
	pushl	(r0)
	pushal	00003EE8
	brb	00000578

l000005EC:
	movl	+08(ap),r0
	pushl	+04(r0)
	calls	#01,00001954
	movl	r0,r10
	beql	00000623

l000005FF:
	cvtlw	+08(r10),00005118
	pushl	+0C(r10)
	pushal	0000511C
	pushl	+10(r10)
	calls	#03,00003818
	movl	(r10),00005144
	brb	00000677

l00000623:
	cvtlw	#00000002,00005118
	movl	+08(ap),r0
	pushl	+04(r0)
	calls	#01,00002090
	movl	r0,0000511C
	cmpl	r0,#FFFFFFFF
	bneq	00000658

l00000648:
	movl	+08(ap),r0
	pushl	+04(r0)
	pushal	00003F04
	brw	00000578

l00000658:
	movl	+08(ap),r0
	pushl	+04(r0)
	pushal	00005148
	calls	#02,000035BC
	moval	00005148,00005144

l00000677:
	movl	00005128,r0
	cvtlw	+08(r0),0000511A
	cmpl	+04(ap),#00000003
	bneq	000006B6

l0000068C:
	movl	+08(ap),r0
	pushl	+08(r0)
	calls	#01,00001D98
	cvtlw	r0,0000511A
	movzwl	0000511A,-(sp)
	calls	#01,00002080
	cvtlw	r0,0000511A

l000006B6:
	pushl	#00000000
	pushl	#00000000
	pushl	#00000001
	pushl	#00000002
	calls	#04,000029A4
	movl	r0,00004F9C
	tstl	r0
	bgeq	000006DE

l000006D0:
	pushal	00003F2B
	calls	#01,000021E4
	ret

l000006DE:
	tstl	00003CE4
	beql	00000710

l000006E6:
	pushl	#00000000
	pushl	#00000000
	pushl	#00000001
	pushl	#0000FFFF
	pushl	00004F9C
	calls	#05,000017FC
	tstl	r0
	bgeq	00000710

l00000703:
	pushal	00003F3A
	calls	#01,000021E4

l00000710:
	pushal	00001416
	pushl	#00000002
	calls	#02,000029C4
	pushal	000013F6
	pushl	#0000000D
	calls	#02,000029C4
	pushal	00003F50
	calls	#01,000016C8
	pushl	#00000000
	pushl	#00000010
	pushal	00005118
	pushl	00004F9C
	calls	#04,00001698
	tstl	r0
	bgeq	0000076F

l00000756:
	pushal	00003F5B
	calls	#01,000021E4
	pushl	#00000000
	pushl	#00000002
	calls	#02,000029C4
	ret

l0000076F:
	incl	00004F98
	pushl	#00000000
	pushal	00003F6B
	pushal	000007C6
	calls	#03,000009D8
	pushal	000050F0
	calls	#01,00001744
	tstl	r0
	bneq	000007A8

l0000079B:
	pushl	00004F9C
	calls	#01,00000AF6

l000007A8:
	pushal	00003F72
	pushal	000043FC
	calls	#02,000016A4
	pushl	#00000001
	calls	#01,00003500
	ret
000007C5                00 00 00 D5 EF CA 47 00 00 13 15      ......G....
000007D0 DD EF 6E 49 00 00 DF EF BA 37 00 00 FB 02 EF E5 ..nI.....7......
000007E0 0E 00 00 11 0D DF EF BD 37 00 00 FB 01 EF D6 0E ........7.......
000007F0 00 00 98 EF F4 34 00 00 7E FB 01 EF 3C 0B 00 00 .....4..~...<...
00000800 DD 50 DF EF B0 37 00 00 FB 02 EF B9 0E 00 00 DF .P...7..........
00000810 EF D3 3B 00 00 FB 01 EF 0E 2C 00 00 04 00 00 0C ..;......,......

;; fn00000820: 00000820
;;   Called from:
;;     000005C1 (in fn00000564)
;;     00000E1F (in fn00000D40)
fn00000820 proc
	moval	00005078,r10
	clrl	00005070
	moval	00004FA8,r11
	brb	0000086B

l00000836:
	incl	r11

l00000838:
	cvtbl	(r11),r0
	bbs	#00000003,00004339[r0],00000836

l00000844:
	tstb	(r11)
	beql	0000086F

l00000848:
	movl	r11,(r10)+
	incl	00005070
	brb	00000861

l00000853:
	cvtbl	(r11),r0
	bbs	#00000003,00004339[r0],00000865

l0000085F:
	incl	r11

l00000861:
	tstb	(r11)
	bneq	00000853

l00000865:
	tstb	(r11)
	beql	0000086F

l00000869:
	clrb	(r11)+

l0000086B:
	tstb	(r11)
	bneq	00000838

l0000086F:
	clrl	(r10)+
	ret
00000872       00 08 DD 00 FB 01 EF 77 01 00 00 D0 50 5B   .......w....P[
00000880 DD 12 DD 00 FB 02 EF FD 0D 00 00 DF EF AB 48 00 ..............H.
00000890 00 DD 8F 08 74 06 40 DD 00 FB 03 EF E4 2C 00 00 ....t.@......,..
000008A0 DF EF 86 48 00 00 DD 8F 12 74 06 40 DD 00 FB 03 ...H.....t.@....
000008B0 EF CF 2C 00 00 DF EF 79 48 00 00 DD 8F 74 74 06 ..,....yH....tt.
000008C0 40 DD 00 FB 03 EF BA 2C 00 00 DD 5B FB 01 EF 21 @......,...[...!
000008D0 01 00 00 04 00 08 DD 00 FB 01 EF 15 01 00 00 D5 ................
000008E0 EF B3 46 00 00 13 45 DD 02 DD EF AD 46 00 00 FB ..F...E.....F...
000008F0 02 EF 1A 0F 00 00 DF EF D7 36 00 00 FB 01 EF C5 .........6......
00000900 0D 00 00 DD EF 93 46 00 00 FB 01 EF 84 2C 00 00 ......F......,..
00000910 D4 EF 82 46 00 00 DE EF 7C 44 00 00 5B 11 04 94 ...F....|D..[...
00000920 6B D6 5B D1 5B 8F 98 4A 00 00 19 F3 04 00 00 00 k.[.[..J........
00000930 DD 00 DF EF AF 36 00 00 DF EF 96 FF FF FF FB 03 .....6..........
00000940 EF 93 00 00 00 DD 00 FB 01 EF B2 2B 00 00 04 00 ...........+....
00000950 00 0C D1 AC 04 01 12 45 DF EF 8D 36 00 00 FB 01 .......E...6....
00000960 EF 63 0D 00 00 DE EF 85 34 00 00 5B 11 17 DD AB .c......4..[....
00000970 04 DD 6B DD 08 DF EF 9E 36 00 00 FB 04 EF 46 0D ..k.....6.....F.
00000980 00 00 C0 0C 5B D5 6B 12 E5 04 D5 5B 12 3E DD 5A ....[.k....[.>.Z
00000990 DF EF B0 36 00 00 FB 02 EF 2B 0D 00 00 F5 AC 04 ...6.....+......
000009A0 01 04 C0 04 AC 08 D0 AC 08 50 D0 60 5A DD 5A FB .........P.`Z.Z.
000009B0 01 EF D6 09 00 00 D0 50 5B D1 5B 8F FF FF FF FF .......P[.[.....
000009C0 12 C8 DD 5A DF EF 60 36 00 00 11 CA DD AB 04 DF ...Z..`6........
000009D0 EF 8B 36 00 00 11 BF 00 00 0C                   ..6.......      

;; fn000009DA: 000009DA
;;   Called from:
;;     00000783 (in fn00000564)
fn000009DA proc
	clrl	r10
	addl3	#00000008,ap,r11
	brb	000009E4

l000009E2:
	incl	r10

l000009E4:
	tstl	(r11)+
	bneq	000009E2

l000009E8:
	addl3	#00000008,ap,-(sp)
	pushl	r10
	calls	#02,@+04(ap)
	ret
000009F3          00 00 08                                  ...          

;; fn000009F6: 000009F6
;;   Called from:
;;     00000B11 (in fn00000AF8)
;;     00000D36 (in fn00000AF8)
;;     00000D45 (in fn00000D40)
;;     00000DFE (in fn00000D40)
;;     000014DE (in fn0000149A)
fn000009F6 proc
	subl2	#00000018,sp
	movl	+04(ap),r11
	cmpl	00003E84,r11
	bneq	00000A0A

l00000A06:
	movl	r11,r0
	ret

l00000A0A:
	movl	00003E84,-18(fp)
	movl	r11,00003E84
	movc3	#0006,0000513C,-0E(fp)
	movl	r11,r0
	cmpl	r0,#00000000
	beql	00000A38
	cmpl	r0,#00000001
	bneq	00000A32
	brw	00000AC3
	cmpl	r0,#00000002
	beql	00000A2F
	ret
	clrl	-14(fp)
	moval	0000512C,-04(fp)
	moval	00005134,-08(fp)
	pushl	-08(fp)
	pushl	#80067475
	cvtbl	000043E6,-(sp)
	calls	#03,00003584
	pushl	-04(fp)
	pushl	#80067411
	cvtbl	000043E6,-(sp)
	calls	#03,00003584
	pushal	-0E(fp)
	pushl	#80067409
	cvtbl	000043E6,-(sp)
	calls	#03,00003584
	pushal	-14(fp)
	pushl	#8004667E
	cvtbl	000043E6,-(sp)
	calls	#03,00003584
	pushal	-14(fp)
	pushl	#8004667E
	cvtbl	000043FA,-(sp)
	calls	#03,00003584
	movl	-18(fp),r0
	ret
	bisw2	#0002,-0A(fp)
	cmpl	r11,#00000001
	bneq	00000AD2
	bicw2	#0018,-0A(fp)
	brb	00000AD6
	bisw2	#0018,-0A(fp)
	mnegb	#01,-0B(fp)
	movb	-0B(fp),-0C(fp)
	moval	00003E74,-04(fp)
	moval	00003E7C,-08(fp)
	movl	#00000001,-14(fp)
	brw	00000A4B
	halt
	prober	+5E14(r2),@(r8)+,000043E6

;; fn00000AF8: 00000AF8
;;   Called from:
;;     000007A1 (in fn00000564)
fn00000AF8 proc
	subl2	#00000014,sp
	cvtbl	000043E6,-04(fp)
	cvtbl	000043FA,-08(fp)
	movl	#00000001,-0C(fp)
	pushl	#00000002
	calls	#01,000009F4
	pushal	-0C(fp)
	pushl	#8004667E
	pushl	+04(ap)
	calls	#03,00003584

l00000B2B:
	clrl	-10(fp)
	clrl	-14(fp)
	subl3	00003CBC,00003CB8,r0
	beql	00000B4A

l00000B3F:
	ashl	+04(ap),#00000001,r0
	bisl2	r0,-14(fp)
	brb	00000B53

l00000B4A:
	ashl	-04(fp),#00000001,r0
	bisl2	r0,-10(fp)

l00000B53:
	subl3	00003CB4,00003CB0,r0
	beql	00000B6C

l00000B61:
	ashl	-08(fp),#00000001,r0
	bisl2	r0,-14(fp)
	brb	00000B75

l00000B6C:
	ashl	+04(ap),#00000001,r0
	bisl2	r0,-10(fp)

l00000B75:
	tstl	00005974
	bgeq	00000B88

l00000B7D:
	tstl	00005978
	bgeq	00000B88

l00000B85:
	brw	00000D34

l00000B88:
	pushl	#00000000
	pushl	#00000000
	pushal	-14(fp)
	pushal	-10(fp)
	pushl	#00000010
	calls	#05,000016F8
	tstl	-10(fp)
	bneq	00000BB1

l00000BA0:
	tstl	-14(fp)
	bneq	00000BB1

l00000BA5:
	pushl	#00000005
	calls	#01,0000181C
	brw	00000B2B

l00000BB1:
	ashl	+04(ap),#00000001,r0
	mcoml	r0,r0
	bicl3	r0,-10(fp),r0
	beql	00000C08

l00000BC0:
	pushl	#00000400
	pushal	00005168
	pushl	+04(ap)
	calls	#03,000035B4
	movl	r0,00005974
	tstl	r0
	bgeq	00000BF2

l00000BE1:
	cmpl	0000597C,#00000023
	bneq	00000BF2

l00000BEA:
	clrl	00005974
	brb	00000C08

l00000BF2:
	tstl	00005974
	bgtr	00000BFD

l00000BFA:
	brw	00000D34

l00000BFD:
	moval	00005168,00005568

l00000C08:
	ashl	-04(fp),#00000001,r0
	mcoml	r0,r0
	bicl3	r0,-10(fp),r0
	beql	00000C5F

l00000C17:
	pushl	#00000400
	pushal	00005570
	pushl	-04(fp)
	calls	#03,000035B4
	movl	r0,00005978
	tstl	r0
	bgeq	00000C49

l00000C38:
	cmpl	0000597C,#00000023
	bneq	00000C49

l00000C41:
	clrl	00005978
	brb	00000C5F

l00000C49:
	tstl	00005978
	bgtr	00000C54

l00000C51:
	brw	00000D34

l00000C54:
	moval	00005570,00005970

l00000C5F:
	tstl	00005978
	bleq	00000CAB

l00000C67:
	subl3	00003CB8,#00004998,r0
	cmpl	r0,#00000002
	blss	00000CAB

l00000C78:
	movzbl	@00005970,r10
	incl	00005970
	decl	00005978
	extzv	#00000000,#07,r10,r0
	cvtbl	00003CEC,r1
	cmpl	r0,r1
	bneq	00000D0E

l00000C9C:
	pushl	#00000000
	calls	#01,00000D3E
	clrl	00005978

l00000CAB:
	ashl	+04(ap),#00000001,r0
	mcoml	r0,r0
	bicl3	r0,-14(fp),r0
	beql	00000CD2

l00000CBA:
	subl3	00003CBC,00003CB8,r0
	bleq	00000CD2

l00000CC8:
	pushl	+04(ap)
	calls	#01,00001498

l00000CD2:
	tstl	00005974
	bleq	00000CE1

l00000CDA:
	calls	#00,00000E7C

l00000CE1:
	ashl	-08(fp),#00000001,r0
	mcoml	r0,r0
	bicl3	r0,-14(fp),r0
	bneq	00000CF3

l00000CF0:
	brw	00000B2B

l00000CF3:
	subl3	00003CB4,00003CB0,r0
	bleq	00000CF0

l00000D01:
	pushl	-08(fp)
	calls	#01,00001436
	brw	00000B2B

l00000D0E:
	cmpl	r10,#000000FF
	bneq	00000D24

l00000D17:
	cvtlb	r10,@00003CB8
	incl	00003CB8

l00000D24:
	cvtlb	r10,@00003CB8
	incl	00003CB8
	brw	00000C5F

l00000D34:
	pushl	#00000000
	calls	#01,000009F4
	ret
00000D3E                                           00 08               ..

;; fn00000D40: 00000D40
;;   Called from:
;;     00000558 (in fn0000044E)
;;     00000C9E (in fn00000AF8)
fn00000D40 proc
	subl2	#00000008,sp
	pushl	#00000000
	calls	#01,000009F4
	movl	r0,-04(fp)
	tstl	+04(ap)
	bneq	00000D80

l00000D55:
	decl	000043E8
	blss	00000D6F

l00000D5D:
	movl	000043EC,r0
	incl	000043EC
	cvtlb	#0000000A,(r0)
	brb	00000D8B

l00000D6F:
	pushal	000043E8
	pushl	#0000000A
	calls	#02,000032CC
	brb	00000D8B

l00000D80:
	pushl	#00000000
	pushl	#00000002
	calls	#02,000029C4

l00000D8B:
	pushl	00004FA4
	pushal	00004064
	calls	#02,000016C8
	pushal	00004FA8
	calls	#01,0000162C
	tstl	r0
	bneq	00000E17

l00000DAF:
	bbc	#00000004,000043E4,00000DDF

l00000DB7:
	pushal	000043D4
	calls	#01,00001620
	decl	000043E8
	blss	00000E06

l00000DCC:
	movl	000043EC,r0
	incl	000043EC
	cvtlb	#0000000A,(r0)
	cvtbl	(r0),r0

l00000DDF:
	tstl	+04(ap)
	bneq	00000E05

l00000DE4:
	tstl	00004F98
	bneq	00000DFB

l00000DEC:
	pushl	#00000001
	pushal	000050C8
	calls	#02,0000176C

l00000DFB:
	pushl	-04(fp)
	calls	#01,000009F4

l00000E05:
	ret

l00000E06:
	pushal	000043E8
	pushl	#0000000A
	calls	#02,000032CC
	brb	00000DDF

l00000E17:
	tstb	00004FA8
	beql	00000DDF

l00000E1F:
	calls	#00,0000081E
	pushl	00005078
	calls	#01,0000138C
	movl	r0,r11
	cmpl	r11,#FFFFFFFF
	bneq	00000E57

l00000E3F:
	pushal	00004069
	brb	00000E4D

l00000E47:
	pushal	0000407D

l00000E4D:
	calls	#01,000016C8
	brw	00000D8B

l00000E57:
	tstl	r11
	beql	00000E47

l00000E5B:
	pushal	00005078
	pushl	00005070
	calls	#02,@+08(r11)
	cmpl	+08(r11),#00000550
	beql	00000E78

l00000E75:
	brw	00000DDF

l00000E78:
	brw	00000D8B
00000E7B                                  00 00 08                  ...  

;; fn00000E7E: 00000E7E
;;   Called from:
;;     00000CDA (in fn00000AF8)
fn00000E7E proc
	brw	00001066

l00000E81:
	movzbl	@00005568,r11
	incl	00005568
	decl	00005974
	movl	00003E88,r0
	casel	r0,#00000000,#00000005
00000E9F                                              0F                .
00000EA0 00 4C 00 B7 00 F6 00 28 01                      .L.....(.       

l00000EA9:
	subd3	#0.5625,#36.0,@+01(r8)
	cmpl	r11,#000000FF
	bneq	00000EC1

l00000EB7:
	movl	#00000001,00003E88

l00000EBE:
	brw	00001066

l00000EC1:
	cvtlb	r11,@00003CB0
	incl	00003CB0
	cmpl	r11,#0000000D
	bneq	00000EBE

l00000ED3:
	tstl	00003CE8
	beql	00000EBE

l00000EDB:
	cvtlb	#0000000A,@00003CB0
	incl	00003CB0
	brw	00001066
00000EEB                                  D0 5B 50 CF 50            .[P.P
00000EF0 8F F1 00 00 00 0D 6A 01 47 00 6A 01 6A 01 6A 01 ......j.G.j.j.j.
00000F00 6A 01 6A 01 6A 01 6A 01 6A 01 1F 00 29 00 33 00 j.j.j.j.j...).3.
00000F10 3D 00 31 4B 01 D0 02 EF 6C 2F 00 00 31 47 01 D0 =.1K....l/..1G..
00000F20 03 EF 62 2F 00 00 31 3D 01 D0 04 EF 58 2F 00 00 ..b/..1=....X/..
00000F30 31 33 01 D0 05 EF 4E 2F 00 00 31 29 01 DD 00 DD 13....N/..1)....
00000F40 8F 10 74 04 80 98 EF AF 34 00 00 7E FB 03 EF 31 ..t.....4..~...1
00000F50 26 00 00 31 0A 01 95 4B EF 3B 3E 00 00 12 05 D0 &..1...K.;>.....
00000F60 01 50 11 02 D4 50 DD 50 DD 5B DF EF 60 2D 00 00 .P...P.P.[..`-..
00000F70 DF EF 19 31 00 00 FB 04 EF C5 05 00 00 95 4B EF ...1..........K.
00000F80 14 3E 00 00 13 03 31 D7 00 DD 5B FB 01 EF E0 00 .>....1...[.....
00000F90 00 00 31 CB 00 98 4B EF FC 3D 00 00 7E DD 5B DF ..1...K..=..~.[.
00000FA0 EF 33 2D 00 00 DF EF E9 30 00 00 FB 04 EF 90 05 .3-.....0.......
00000FB0 00 00 95 4B EF DF 3D 00 00 13 D7 DD 5B FB 01 EF ...K..=.....[...
00000FC0 1E 01 00 00 31 99 00 95 4B EF CA 3E 00 00 12 05 ....1...K..>....
00000FD0 D0 01 50 11 02 D4 50 DD 50 DD 5B DF EF DF 2C 00 ..P...P.P.[...,.
00000FE0 00 DF EF B2 30 00 00 FB 04 EF 54 05 00 00 95 4B ....0.....T....K
00000FF0 EF A3 3E 00 00 12 69 DD 5B FB 01 EF 44 01 00 00 ..>...i.[...D...
00001000 11 5E 98 4B EF 8F 3E 00 00 7E DD 5B DF EF B6 2C .^.K..>..~.[...,
00001010 00 00 DF EF 86 30 00 00 FB 04 EF 23 05 00 00 95 .....0.....#....
00001020 4B EF 72 3E 00 00 13 38 94 4B EF 69 3E 00 00 DD K.r>...8.K.i>...
00001030 5B DF EF A1 2C 00 00 DD EF 7B 2C 00 00 FB 03 EF [...,....{,.....
00001040 D4 19 00 00 C0 03 EF 6D 2C 00 00 DD 5B DF EF 85 .......m,...[...
00001050 2C 00 00 DF EF 4A 30 00 00 FB 03 EF E2 04 00 00 ,....J0.........
00001060 D4 EF 22 2E 00 00                               .."...          

l00001066:
	tstl	00005974
	bleq	00001071

l0000106E:
	brw	00000E81

l00001071:
	ret
00001072       00 00 C2 04 5E D0 AC 04 50 D1 50 01 13 40   ....^...P.P..@
00001080 D1 50 03 13 44 D1 50 06 13 00 DE EF 38 2C 00 00 .P..D.P.....8,..
00001090 AD FC DD AC 04 DD AD FC DD EF 1A 2C 00 00 FB 03 ...........,....
000010A0 EF 73 19 00 00 C0 03 EF 0C 2C 00 00 DD AC 04 DD .s.......,......
000010B0 AD FC DF EF F0 2F 00 00 FB 03 EF 83 04 00 00 04 ...../..........
000010C0 DD 01 FB 01 EF 2B F9 FF FF DE EF C9 3C 00 00 50 .....+......<..P
000010D0 D0 AC 04 51 F6 01 41 60 DE EF E2 2B 00 00 AD FC ...Q..A`...+....
000010E0 11 B0 00 00 C2 04 5E D0 AC 04 50 D1 50 01 13 3B ......^...P.P..;
000010F0 D1 50 03 13 3F DE EF CD 2B 00 00 AD FC DD AC 04 .P..?...+.......
00001100 DD AD FC DD EF AF 2B 00 00 FB 03 EF 08 19 00 00 ......+.........
00001110 C0 03 EF A1 2B 00 00 DD AC 04 DD AD FC DF EF 8A ....+...........
00001120 2F 00 00 FB 03 EF 18 04 00 00 04 DD 02 FB 01 EF /...............
00001130 C0 F8 FF FF DE EF 5E 3C 00 00 50 D0 AC 04 51 94 ......^<..P...Q.
00001140 41 60 11 B1 00 00 C2 04 5E D0 AC 04 50 D1 50 01 A`......^...P.P.
00001150 13 40 D1 50 03 13 5C D1 50 06 12 00 DE EF 76 2B .@.P..\.P.....v+
00001160 00 00 AD FC DD AC 04 DD AD FC DD EF 48 2B 00 00 ............H+..
00001170 FB 03 EF A1 18 00 00 C0 03 EF 3A 2B 00 00 DD AC ..........:+....
00001180 04 DD AD FC DF EF 28 2F 00 00 FB 03 EF B1 03 00 ......(/........
00001190 00 04 DD 02 FB 01 EF 59 F8 FF FF DE EF 2F 2B 00 .......Y...../+.
000011A0 00 AD FC DE EF EF 3B 00 00 50 D0 AC 04 51 94 41 ......;..P...Q.A
000011B0 60 11 B1 DE EF 17 2B 00 00 AD FC 11 A7 00 00 08 `.....+.........
000011C0 C2 34 5E D1 AC 04 02 15 0A D0 AC 08 50 D0 A0 04 .4^.........P...
000011D0 5B 11 1B DF EF DE 2E 00 00 FB 01 EF E8 04 00 00 [...............
000011E0 DF AD CE FB 01 EF 42 04 00 00 DE AD CE 5B 95 6B ......B......[.k
000011F0 13 07 90 6B EF F3 2A 00 00 98 EF ED 2A 00 00 7E ...k..*.....*..~
00001200 FB 01 EF 35 01 00 00 DD 50 DF EF BF 2E 00 00 FB ...5....P.......
00001210 02 EF B2 04 00 00 DF EF CC 31 00 00 FB 01 EF 07 .........1......
00001220 22 00 00 04 00 00 D5 EF B4 2A 00 00 12 05 D0 01 "........*......
00001230 50 11 02 D4 50 D0 50 EF A4 2A 00 00 D5 50 13 09 P...P.P..*...P..
00001240 DE EF BF 2E 00 00 50 11 07 DE EF BB 2E 00 00 50 ......P........P
00001250 DD 50 DF EF 91 2E 00 00 FB 02 EF 69 04 00 00 DF .P.........i....
00001260 EF 83 31 00 00 FB 01 EF BE 21 00 00 04 00 00 00 ..1......!......
00001270 D5 EF 72 2A 00 00 12 05 D0 01 50 11 02 D4 50 D0 ..r*......P...P.
00001280 50 EF 62 2A 00 00 D5 50 13 09 DE EF A2 2E 00 00 P.b*...P........
00001290 50 11 07 DE EF 9E 2E 00 00 50 DD 50 DF EF 6D 2E P........P.P..m.
000012A0 00 00 FB 02 EF 1F 04 00 00 DF EF 39 31 00 00 FB ...........91...
000012B0 01 EF 74 21 00 00 04 00 00 00 D5 EF 24 2A 00 00 ..t!........$*..
000012C0 12 05 D0 01 50 11 02 D4 50 D0 50 EF 14 2A 00 00 ....P...P.P..*..
000012D0 D5 50 13 09 DE EF 86 2E 00 00 50 11 07 DE EF 82 .P........P.....
000012E0 2E 00 00 50 DD 50 DF EF 50 2E 00 00 FB 02 EF D5 ...P.P..P.......
000012F0 03 00 00 DF EF EF 30 00 00 FB 01 EF 2A 21 00 00 ......0.....*!..
00001300 D5 EF DE 29 00 00 13 32 D5 EF 8E 3C 00 00 15 2A ...)...2...<...*
00001310 DD 00 DD 00 DD 01 DD 8F FF FF 00 00 DD EF 7A 3C ..............z<
00001320 00 00 FB 05 EF D3 04 00 00 D5 50 18 0D DF EF 37 ..........P....7
00001330 2E 00 00 FB 01 EF AA 0E 00 00 04 00 00 08 D0 AC ................
00001340 04 5B D1 5B 8F 7F 00 00 00 12 08 DE EF 2F 2E 00 .[.[........./..
00001350 00 50 04 D1 5B 20 19 0F F6 5B EF 21 86 00 00 94 .P..[ ...[.!....
00001360 EF 1C 86 00 00 11 1D 90 8F 5E EF 11 86 00 00 C1 .........^......
00001370 8F 40 00 00 00 5B 50 F6 50 EF 03 86 00 00 94 EF .@...[P.P.......
00001380 FE 85 00 00 DE EF F6 85 00 00 50 04 C0 0F       ..........P...  

;; fn0000138E: 0000138E
;;   Called from:
;;     00000E2C (in fn00000D40)
fn0000138E proc
	subl2	#00000004,sp
	movl	+04(ap),r11
	clrl	-04(fp)
	clrl	r6
	clrl	r7
	moval	00003DF0,r8
	brb	000013E3

l000013A5:
	movl	r11,r9
	brb	000013B4

l000013AA:
	tstb	(r9)
	bneq	000013B2

l000013AE:
	movl	r8,r0
	ret

l000013B2:
	incl	r9

l000013B4:
	cmpb	(r9),(r10)+
	beql	000013AA

l000013B9:
	tstb	(r9)
	bneq	000013E0

l000013BD:
	subl3	r11,r9,r0
	cmpl	r0,-04(fp)
	bleq	000013D4

l000013C7:
	subl3	r11,r9,-04(fp)
	movl	#00000001,r6
	movl	r8,r7
	brb	000013E0

l000013D4:
	subl3	r11,r9,r0
	cmpl	r0,-04(fp)
	bneq	000013E0

l000013DE:
	incl	r6

l000013E0:
	addl2	#0000000C,r8

l000013E3:
	movl	(r8),r10
	bneq	000013A5

l000013E8:
	cmpl	r6,#00000001
	bleq	000013F1

l000013ED:
	mnegl	#00000001,r0
	ret

l000013F1:
	movl	r7,r0
	ret
000013F5                00 00 00 DD 00 FB 01 EF F3 F5 FF      ...........
00001400 FF DD 8F FF FF FF FF DF EF E3 3C 00 00 FB 02 EF ..........<.....
00001410 58 03 00 00 04 00 00 00 DD 00 FB 01 EF D3 F5 FF X...............
00001420 FF DD 8F FF FF FF FF DF EF 9B 3C 00 00 FB 02 EF ..........<.....
00001430 38 03 00 00 04 00 00 00                         8.......        

;; fn00001438: 00001438
;;   Called from:
;;     00000D04 (in fn00000AF8)
fn00001438 proc
	subl2	#00000004,sp
	subl3	00003CB4,00003CB0,r0
	movl	r0,-04(fp)
	bleq	00001463

l0000144D:
	pushl	r0
	pushl	00003CB4
	pushl	+04(ap)
	calls	#03,000038B4
	movl	r0,-04(fp)

l00001463:
	tstl	-04(fp)
	bgeq	00001469

l00001468:
	ret

l00001469:
	addl2	-04(fp),00003CB4
	cmpl	00003CB4,00003CB0
	bneq	00001497

l0000147E:
	moval	00004598,00003CB0
	movl	00003CB0,r0
	movl	r0,00003CB4

l00001497:
	ret
00001498                         00 00                           ..      

;; fn0000149A: 0000149A
;;   Called from:
;;     00000CCB (in fn00000AF8)
fn0000149A proc
	subl2	#00000004,sp
	subl3	00003CBC,00003CB8,r0
	movl	r0,-04(fp)
	bleq	000014C5

l000014AF:
	pushl	r0
	pushl	00003CBC
	pushl	+04(ap)
	calls	#03,000038B4
	movl	r0,-04(fp)

l000014C5:
	tstl	-04(fp)
	bgeq	00001512

l000014CA:
	cmpl	0000597C,#00000037
	beql	0000150F

l000014D3:
	cmpl	0000597C,#00000023
	beql	0000150F

l000014DC:
	pushl	#00000000
	calls	#01,000009F4
	pushl	00005144
	calls	#01,000021E4
	pushl	+04(ap)
	calls	#01,00003594
	pushl	#FFFFFFFF
	pushal	000050F0
	calls	#02,0000176C

l0000150F:
	clrl	-04(fp)

l00001512:
	addl2	-04(fp),00003CBC
	cmpl	00003CBC,00003CB8
	bneq	00001540

l00001527:
	moval	00004998,00003CB8
	movl	00003CB8,r0
	movl	r0,00003CBC

l00001540:
	ret
00001541    00 00 00 D5 EF 96 27 00 00 12 01 04 DD AC 04  ......'........
00001550 DF EF 2D 2C 00 00 FB 02 EF 6B 01 00 00 D1 AC 08 ..-,.....k......
00001560 8F C0 38 00 00 12 0A DE EF 1A 2C 00 00 AC 08 11 ..8.......,.....
00001570 44 D1 AC 08 8F C8 38 00 00 12 0A DE EF 09 2C 00 D.....8.......,.
00001580 00 AC 08 11 30 D1 AC 08 8F D0 38 00 00 12 0A DE ....0.....8.....
00001590 EF FA 2B 00 00 AC 08 11 1C D1 AC 08 8F D8 38 00 ..+...........8.
000015A0 00 12 0A DE EF EB 2B 00 00 AC 08 11 08 DE EF E6 ......+.........
000015B0 2B 00 00 AC 08 D1 AC 0C 15 18 16 D0 AC 0C 50 DD +.............P.
000015C0 40 EF 92 26 00 00 DD AC 08 DF EF CE 2B 00 00 11 @..&........+...
000015D0 0C DD AC 0C DD AC 08 DF EF C6 2B 00 00 FB 03 EF ..........+.....
000015E0 E4 00 00 00 91 BC 04 3C 12 0E DF EF B9 2B 00 00 .......<.....+..
000015F0 FB 01 EF D1 00 00 00 04 D5 AC 10 13 09 DE EF B1 ................
00001600 2B 00 00 50 11 07 DE EF AE 2B 00 00 50 DD 50 DF +..P.....+..P.P.
00001610 EF 97 2B 00 00 FB 02 EF AC 00 00 00 04 00 00 00 ..+.............
00001620 00 08                                           ..              

;; fn00001622: 00001622
;;   Called from:
;;     00000DBD (in fn00000D40)
fn00001622 proc
	movl	+04(ap),r11
	bicw2	#0030,+10(r11)
	ret
0000162B                                  00 00 0C                  ...  

;; fn0000162E: 0000162E
;;   Called from:
;;     000005BA (in fn00000564)
;;     00000DA4 (in fn00000D40)
fn0000162E proc
	movl	+04(ap),r10
	brb	0000163B

l00001634:
	tstl	r11
	blss	0000166A

l00001638:
	cvtlb	r11,(r10)+

l0000163B:
	decl	000043D4
	blss	00001655

l00001643:
	movl	000043D8,r0
	incl	000043D8
	movzbl	(r0),r0
	brb	00001662

l00001655:
	pushal	000043D4
	calls	#01,00001E38

l00001662:
	movl	r0,r11
	cmpl	r0,#0000000A
	bneq	00001634

l0000166A:
	tstl	r11
	bgeq	00001677

l0000166E:
	cmpl	r10,+04(ap)
	bneq	00001677

l00001674:
	clrl	r0
	ret

l00001677:
	clrb	(r10)+
	movl	+04(ap),r0
	ret
0000167E                                           00 00               ..
00001680 17 EF 4A 22 00 00 00 00 00 00 BC 25 1F F2 04 00 ..J".......%....

l00001690:
	jmp	000038D0
00001696                   00 00 00 00                         ....      

;; fn0000169A: 0000169A
;;   Called from:
;;     0000074B (in fn00000564)
fn0000169A proc
	chmk	#0062
	blssu	00001690

l000016A0:
	ret
000016A1    00 00 00 00 00                                .....          

;; fn000016A6: 000016A6
;;   Called from:
;;     00000478 (in fn0000044E)
;;     000007B4 (in fn00000564)
fn000016A6 proc
	pushl	+04(ap)
	addl3	#0000000C,ap,-(sp)
	pushl	+08(ap)
	calls	#03,00002B60
	movl	+04(ap),r0
	bbc	#00000005,+10(r0),000016C5

l000016C0:
	mnegl	#00000001,r0
	brb	000016C7

l000016C5:
	clrl	r0

l000016C7:
	ret
000016C8                         00 00                           ..      

;; fn000016CA: 000016CA
;;   Called from:
;;     00000578 (in fn00000564)
;;     0000059F (in fn00000564)
;;     00000734 (in fn00000564)
;;     00000D97 (in fn00000D40)
;;     00000E4D (in fn00000D40)
fn000016CA proc
	pushal	000043E8
	addl3	#00000008,ap,-(sp)
	pushl	+04(ap)
	calls	#03,00002B60
	bbc	#00000005,000043F8,000016EB

l000016E6:
	mnegl	#00000001,r0
	brb	000016ED

l000016EB:
	clrl	r0

l000016ED:
	ret
000016EE                                           00 00               ..

l000016F0:
	jmp	000038D0
000016F6                   00 00 00 00                         ....      

;; fn000016FA: 000016FA
;;   Called from:
;;     00000B94 (in fn00000AF8)
fn000016FA proc
	chmk	#005D
	blssu	000016F0

l00001700:
	ret
00001701    00 00 00 00 08                                .....          

;; fn00001706: 00001706
;;   Called from:
;;     000004CF (in fn0000044E)
;;     000004DE (in fn0000044E)
fn00001706 proc
	movl	+04(ap),r11
	tstl	+08(r11)
	beql	0000171E

l0000170F:
	bbc	#00000003,+10(r11),0000171E

l00001714:
	pushl	+08(r11)
	calls	#01,000036DE

l0000171E:
	bicw2	#008C,+10(r11)
	movl	+08(ap),+08(r11)
	bneq	00001734

l0000172B:
	bisw2	#0004,+10(r11)
	clrl	+0C(r11)
	brb	0000173F

l00001734:
	movl	+08(r11),+04(r11)
	cvtwl	#0400,+0C(r11)

l0000173F:
	clrl	(r11)
	ret
00001742       00 00 00 00                                 ....          

;; fn00001746: 00001746
;;   Called from:
;;     00000528 (in fn0000044E)
;;     0000054F (in fn0000044E)
;;     00000790 (in fn00000564)
fn00001746 proc
	pushl	#00000000
	calls	#01,00001934
	movl	r0,r1
	movl	+04(ap),r0
	movl	+0C(fp),(r0)
	movl	+10(fp),+04(r0)
	movl	r1,+08(r0)
	clrl	+0C(r0)
	clrl	r0
	ret
00001769                            00 00 00 00 00                .....  

;; fn0000176E: 0000176E
;;   Called from:
;;     00000DF4 (in fn00000D40)
;;     00001508 (in fn0000149A)
fn0000176E proc
	movl	+08(ap),r0
	movl	+04(ap),r1
	tstl	(r1)
	beql	000017DD

l0000177A:
	bitw	#0001,+06(fp)
	beql	00001790

l00001780:
	movl	r0,+14(fp)
	bitw	#0002,+06(fp)
	beql	0000179A

l0000178A:
	movl	r1,+18(fp)
	brb	0000179A

l00001790:
	bitw	#0002,+06(fp)
	beql	0000179A

l00001796:
	movl	r1,+14(fp)

l0000179A:
	cmpl	(r1),+0C(fp)
	beql	000017AB

l000017A0:
	blssu	000017DD

l000017A2:
	movl	#0000137A,+10(fp)
	ret

l000017AB:
	cmpb	@+10(fp),000042C2
	bneq	000017C0

l000017B5:
	movab	000017C9,+10(fp)
	brw	000017C8

l000017C0:
	movab	000017CC,+10(fp)

l000017C8:
	ret
000017C9                            C0 08 5E DD 5E DD A1          ..^.^..
000017D0 08 DD A1 0C DD 5E BC 8F 8B 00 17 B1 04          .....^.......   

l000017DD:
	pushl	#0000000E
	pushl	#00003EB4
	pushl	#00000002
	calls	#03,000038B4
	halt
000017EF                                              00                .
000017F0 00 00 00 00                                     ....            

l000017F4:
	jmp	000038D0
000017FA                               00 00 00 00                 ....  

;; fn000017FE: 000017FE
;;   Called from:
;;     000006F8 (in fn00000564)
fn000017FE proc
	chmk	#0069
	blssu	000017F4

l00001804:
	ret
00001805                00 00 00 17 EF C2 20 00 00 00 00      ...... ....
00001810 00 00 BC 8F 86 00 1F F0 04 00 00 00 00 08       ..............  

;; fn0000181E: 0000181E
;;   Called from:
;;     00000BA7 (in fn00000AF8)
fn0000181E proc
	subl2	#0000003C,sp
	moval	-14(fp),r0
	movl	r0,r11
	tstl	+04(ap)
	bneq	0000182E

l0000182D:
	ret

l0000182E:
	clrl	+04(r11)
	movl	+04(r11),(r11)
	clrl	+0C(r11)
	movl	+0C(r11),+08(r11)
	pushal	-24(fp)
	pushl	r11
	pushl	#00000000
	calls	#03,00001920
	tstl	r0
	bgeq	00001850

l0000184F:
	ret

l00001850:
	clrl	-3C(fp)
	clrl	-34(fp)
	movl	-34(fp),-38(fp)
	pushl	#00000000
	calls	#01,00001934
	movl	r0,-04(fp)
	movl	+04(ap),+08(r11)
	tstl	-1C(fp)
	bneq	00001877

l00001872:
	tstl	-18(fp)
	beql	000018A2

l00001877:
	cmpl	-1C(fp),+08(r11)
	bgtr	0000188C

l0000187E:
	cmpl	-1C(fp),+08(r11)
	bneq	00001893

l00001885:
	cmpl	-18(fp),+0C(r11)
	bleq	00001893

l0000188C:
	subl2	+08(r11),-1C(fp)
	brb	000018A2

l00001893:
	moval	-1C(fp),r0
	movq	(r0),+08(r11)
	movl	#00000001,-1C(fp)
	clrl	-18(fp)

l000018A2:
	moval	0000190C,-30(fp)
	clrl	-28(fp)
	movl	-28(fp),-2C(fp)
	pushal	-3C(fp)
	pushal	-30(fp)
	pushl	#0000000E
	calls	#03,000029FC
	clrl	00009984
	pushl	#00000000
	pushl	r11
	pushl	#00000000
	calls	#03,00001920
	brb	000018E6

l000018D6:
	bicl3	#00002000,-04(fp),-(sp)
	calls	#01,00001948

l000018E6:
	tstl	00009984
	beql	000018D6

l000018EE:
	pushl	#00000000
	pushal	-3C(fp)
	pushl	#0000000E
	calls	#03,000029FC
	pushl	#00000000
	pushal	-24(fp)
	pushl	#00000000
	calls	#03,00001920
	ret
0000190B                                  00 00 00 D0 01            .....
00001910 EF 6F 80 00 00 04 00 00                         .o......        

;; fn00001918: 00001918
;;   Called from:
;;     00001926 (in fn00001922)
fn00001918 proc
	jmp	000038D0
0000191E                                           00 00               ..
00001920 00 00                                           ..              

;; fn00001922: 00001922
;;   Called from:
;;     00001844 (in fn0000181E)
;;     000018CD (in fn0000181E)
;;     00001903 (in fn0000181E)
fn00001922 proc
	chmk	#0053
	blssu	fn00001918

l00001928:
	ret
00001929                            00 00 00                      ...    

;; fn0000192C: 0000192C
;;   Called from:
;;     0000193A (in fn00001936)
fn0000192C proc
	jmp	000038D0
00001932       00 00 00 00                                 ....          

;; fn00001936: 00001936
;;   Called from:
;;     00001748 (in fn00001746)
;;     0000185D (in fn0000181E)
fn00001936 proc
	chmk	#006D
	blssu	fn0000192C

l0000193C:
	ret
0000193D                                        00 00 00              ...

l00001940:
	jmp	000038D0
00001946                   00 00 00 00                         ....      

;; fn0000194A: 0000194A
;;   Called from:
;;     000018DF (in fn0000181E)
fn0000194A proc
	chmk	#006F
	blssu	00001940

l00001950:
	ret
00001951    00 00 00 00 0E                                .....          

;; fn00001956: 00001956
;;   Called from:
;;     000005F3 (in fn00000564)
fn00001956 proc
	movl	+04(ap),r11
	pushl	#00000000
	calls	#01,000019A8

l00001963:
	calls	#00,00001A0A
	movl	r0,r10
	beql	0000199A

l0000196F:
	pushl	r11
	pushl	(r10)
	calls	#02,00003514
	tstl	r0
	beql	0000199A

l0000197E:
	movl	+04(r10),r9
	brb	00001987

l00001984:
	addl2	#00000004,r9

l00001987:
	tstl	(r9)
	beql	00001963

l0000198B:
	pushl	r11
	pushl	(r9)
	calls	#02,00003514
	tstl	r0
	bneq	00001984

l0000199A:
	calls	#00,000019E4
	movl	r10,r0
	ret
000019A5                00 00 00 00 00                        .....      

;; fn000019AA: 000019AA
;;   Called from:
;;     0000195C (in fn00001956)
fn000019AA proc
	tstl	000042F4
	bneq	000019CE

l000019B2:
	pushal	000042FC
	pushal	000042E8
	calls	#02,00001F50
	movl	r0,000042F4
	brb	000019DB

l000019CE:
	pushl	000042F4
	calls	#01,00002968

l000019DB:
	bisl2	+04(ap),000042F8
	ret
000019E4             00 00                                   ..          

;; fn000019E6: 000019E6
;;   Called from:
;;     0000199A (in fn00001956)
fn000019E6 proc
	tstl	000042F4
	beql	00001A09

l000019EE:
	tstl	000042F8
	bneq	00001A09

l000019F6:
	pushl	000042F4
	calls	#01,000034A6
	clrl	000042F4

l00001A09:
	ret
00001A0A                               00 0C                       ..    

;; fn00001A0C: 00001A0C
;;   Called from:
;;     00001963 (in fn00001956)
fn00001A0C proc
	subl2	#00000004,sp
	tstl	000042F4
	bneq	00001A36

l00001A17:
	pushal	000042FE
	pushal	000042E8
	calls	#02,00001F50
	movl	r0,000042F4
	bneq	00001A36

l00001A33:
	clrl	r0
	ret

l00001A36:
	pushl	000042F4
	pushl	#00000400
	pushal	00009988
	calls	#03,00001DEC
	movl	r0,-04(fp)
	beql	00001A33

l00001A55:
	cmpb	@-04(fp),#23
	beql	00001A36

l00001A5B:
	pushal	00004300
	pushl	-04(fp)
	calls	#02,00001B32
	movl	r0,r11
	beql	00001A36

l00001A70:
	clrb	(r11)
	pushal	00004303
	pushl	-04(fp)
	calls	#02,00001B32
	movl	r0,r11
	beql	00001A36

l00001A87:
	clrb	(r11)+
	moval	00009D8C,00009DAC
	pushl	-04(fp)
	calls	#01,00002090
	movl	r0,@00009DAC
	movl	#00000004,00009DA8
	movl	#00000002,00009DA4
	brb	00001AB7

l00001AB5:
	incl	r11

l00001AB7:
	cmpb	(r11),#20
	beql	00001AB5

l00001ABC:
	cmpb	(r11),#09
	beql	00001AB5

l00001AC1:
	movl	r11,00009D9C
	moval	00009DB0,00009DA0
	movl	00009DA0,r10
	pushal	00004306
	pushl	r11
	calls	#02,00001B32
	movl	r0,r11
	bneq	00001B22

l00001AEE:
	brb	00001B24

l00001AF0:
	incl	r11
	brb	00001B24

l00001AF4:
	tstb	(r11)
	beql	00001B28

l00001AF8:
	cmpb	(r11),#20
	beql	00001AF0

l00001AFD:
	cmpb	(r11),#09
	beql	00001AF0

l00001B02:
	cmpl	r10,#00009A38
	bgeq	00001B0E

l00001B0B:
	movl	r11,(r10)+

l00001B0E:
	pushal	00004309
	pushl	r11
	calls	#02,00001B32
	movl	r0,r11
	beql	00001B24

l00001B22:
	clrb	(r11)+

l00001B24:
	tstl	r11
	bneq	00001AF4

l00001B28:
	clrl	(r10)
	moval	00009D9C,r0
	ret
00001B32       00 0C                                       ..            

;; fn00001B34: 00001B34
;;   Called from:
;;     00001A64 (in fn00001A0C)
;;     00001A7B (in fn00001A0C)
;;     00001AE2 (in fn00001A0C)
;;     00001B16 (in fn00001A0C)
fn00001B34 proc
	subl2	#00000004,sp
	movl	+04(ap),r11
	brb	00001B55

l00001B3D:
	movl	+08(ap),r10
	brb	00001B4F

l00001B43:
	cmpb	(r10),-01(fp)
	bneq	00001B4D

l00001B49:
	movl	r11,r0
	ret

l00001B4D:
	incl	r10

l00001B4F:
	tstb	(r10)
	bneq	00001B43

l00001B53:
	incl	r11

l00001B55:
	movb	(r11),-01(fp)
	bneq	00001B3D

l00001B5B:
	brw	00001A33
00001B5E                                           00 00               ..
00001B60 00 0C                                           ..              

;; fn00001B62: 00001B62
;;   Called from:
;;     0000045A (in fn0000044E)
fn00001B62 proc
	pushl	#00000000
	calls	#01,00001BC8

l00001B6B:
	calls	#00,00001C2A
	movl	r0,r11
	beql	00001BBA

l00001B77:
	pushl	(r11)
	pushl	+04(ap)
	calls	#02,00003514
	tstl	r0
	beql	00001BA4

l00001B87:
	movl	+04(r11),r10
	brb	00001B90

l00001B8D:
	addl2	#00000004,r10

l00001B90:
	tstl	(r10)
	beql	00001B6B

l00001B94:
	pushl	(r10)
	pushl	+04(ap)
	calls	#02,00003514
	tstl	r0
	bneq	00001B8D

l00001BA4:
	tstl	+08(ap)
	beql	00001BBA

l00001BA9:
	pushl	+08(ap)
	pushl	+0C(r11)
	calls	#02,00003514
	tstl	r0
	bneq	00001B6B

l00001BBA:
	calls	#00,00001C04
	movl	r11,r0
	ret
00001BC5                00 00 00 00 00                        .....      

;; fn00001BCA: 00001BCA
;;   Called from:
;;     00001B64 (in fn00001B62)
fn00001BCA proc
	tstl	0000431C
	bneq	00001BEE

l00001BD2:
	pushal	00004324
	pushal	0000430C
	calls	#02,00001F50
	movl	r0,0000431C
	brb	00001BFB

l00001BEE:
	pushl	0000431C
	calls	#01,00002968

l00001BFB:
	bisl2	+04(ap),00004320
	ret
00001C04             00 00                                   ..          

;; fn00001C06: 00001C06
;;   Called from:
;;     00001BBA (in fn00001B62)
fn00001C06 proc
	tstl	0000431C
	beql	00001C29

l00001C0E:
	tstl	00004320
	bneq	00001C29

l00001C16:
	pushl	0000431C
	calls	#01,000034A6
	clrl	0000431C

l00001C29:
	ret
00001C2A                               00 0C                       ..    

;; fn00001C2C: 00001C2C
;;   Called from:
;;     00001B6B (in fn00001B62)
fn00001C2C proc
	subl2	#00000004,sp
	tstl	0000431C
	bneq	00001C56

l00001C37:
	pushal	00004326
	pushal	0000430C
	calls	#02,00001F50
	movl	r0,0000431C
	bneq	00001C56

l00001C53:
	clrl	r0
	ret

l00001C56:
	pushl	0000431C
	pushl	#00000400
	pushal	00009E3C
	calls	#03,00001DEC
	movl	r0,-04(fp)
	beql	00001C53

l00001C75:
	cmpb	@-04(fp),#23
	beql	00001C56

l00001C7B:
	pushal	00004328
	pushl	-04(fp)
	calls	#02,00001D6A
	movl	r0,r11
	beql	00001C56

l00001C90:
	clrb	(r11)
	movl	-04(fp),0000A240
	pushal	0000432B
	pushl	-04(fp)
	calls	#02,00001D6A
	movl	r0,-04(fp)
	tstl	r0
	beql	00001C56

l00001CB2:
	clrb	@-04(fp)

l00001CB5:
	incl	-04(fp)
	cmpb	@-04(fp),#20
	beql	00001CB5

l00001CBE:
	cmpb	@-04(fp),#09
	beql	00001CB5

l00001CC4:
	pushal	0000432E
	pushl	-04(fp)
	calls	#02,00001D6A
	movl	r0,r11
	bneq	00001CDC

l00001CD9:
	brw	00001C56

l00001CDC:
	clrb	(r11)+
	pushl	-04(fp)
	calls	#01,00001D98
	movzwl	r0,-(sp)
	calls	#01,00002080
	movl	r0,0000A248
	movl	r11,0000A24C
	moval	0000A250,0000A244
	movl	0000A244,r10
	pushal	00004331
	pushl	r11
	calls	#02,00001D6A
	movl	r0,r11
	bneq	00001D5A

l00001D26:
	brb	00001D5C

l00001D28:
	incl	r11
	brb	00001D5C

l00001D2C:
	tstb	(r11)
	beql	00001D60

l00001D30:
	cmpb	(r11),#20
	beql	00001D28

l00001D35:
	cmpb	(r11),#09
	beql	00001D28

l00001D3A:
	cmpl	r10,#00009ED8
	bgeq	00001D46

l00001D43:
	movl	r11,(r10)+

l00001D46:
	pushal	00004334
	pushl	r11
	calls	#02,00001D6A
	movl	r0,r11
	beql	00001D5C

l00001D5A:
	clrb	(r11)+

l00001D5C:
	tstl	r11
	bneq	00001D2C

l00001D60:
	clrl	(r10)
	moval	0000A240,r0
	ret
00001D6A                               00 0C                       ..    

;; fn00001D6C: 00001D6C
;;   Called from:
;;     00001C84 (in fn00001C2C)
;;     00001CA3 (in fn00001C2C)
;;     00001CCD (in fn00001C2C)
;;     00001D1A (in fn00001C2C)
;;     00001D4E (in fn00001C2C)
fn00001D6C proc
	subl2	#00000004,sp
	movl	+04(ap),r11
	brb	00001D8D

l00001D75:
	movl	+08(ap),r10
	brb	00001D87

l00001D7B:
	cmpb	(r10),-01(fp)
	bneq	00001D85

l00001D81:
	movl	r11,r0
	ret

l00001D85:
	incl	r10

l00001D87:
	tstb	(r10)
	bneq	00001D7B

l00001D8B:
	incl	r11

l00001D8D:
	movb	(r11),-01(fp)
	bneq	00001D75

l00001D93:
	brw	00001C53
00001D96                   00 00 00 0E                         ....      

;; fn00001D9A: 00001D9A
;;   Called from:
;;     00000693 (in fn00000564)
;;     00001CE1 (in fn00001C2C)
fn00001D9A proc
	movl	+04(ap),r11
	clrl	r10
	clrl	r9
	brb	00001DC1

l00001DA4:
	cmpb	(r11),#39
	bgtr	00001DDD

l00001DA9:
	mull3	#0000000A,r10,r0
	cvtbl	(r11)+,r1
	addl2	r0,r1
	subl3	#00000030,r1,r10
	brb	00001DD8

l00001DB9:
	incl	r9

l00001DBB:
	incl	r11
	brb	00001DD8

l00001DBF:
	incl	r11

l00001DC1:
	cvtbl	(r11),r0
	cmpl	r0,#00000009
	beql	00001DBF

l00001DC9:
	cmpl	r0,#00000020
	beql	00001DBF

l00001DCE:
	cmpl	r0,#0000002B
	beql	00001DBB

l00001DD3:
	cmpl	r0,#0000002D
	beql	00001DB9

l00001DD8:
	cmpb	(r11),#30
	bgeq	00001DA4

l00001DDD:
	tstl	r9
	beql	00001DE6

l00001DE1:
	mnegl	r10,r0
	brb	00001DE9

l00001DE6:
	movl	r10,r0

l00001DE9:
	ret
00001DEA                               00 00 00 0E                 ....  

;; fn00001DEE: 00001DEE
;;   Called from:
;;     00001A48 (in fn00001A0C)
;;     00001C68 (in fn00001C2C)
fn00001DEE proc
	movl	+0C(ap),r11
	movl	+04(ap),r9

l00001DF6:
	decl	+08(ap)
	bleq	00001E21

l00001DFB:
	decl	(r11)
	blss	00001E0B

l00001DFF:
	movl	+04(r11),r0
	incl	+04(r11)
	movzbl	(r0),r0
	brb	00001E14

l00001E0B:
	pushl	r11
	calls	#01,00001E38

l00001E14:
	movl	r0,r10
	blss	00001E21

l00001E19:
	cvtlb	r10,(r9)+
	cmpl	r10,#0000000A
	bneq	00001DF6

l00001E21:
	tstl	r10
	bgeq	00001E2E

l00001E25:
	cmpl	r9,+04(ap)
	bneq	00001E2E

l00001E2B:
	clrl	r0
	ret

l00001E2E:
	clrb	(r9)+
	movl	+04(ap),r0
	ret
00001E35                00 00 00 00 08                        .....      

;; fn00001E3A: 00001E3A
;;   Called from:
;;     0000165B (in fn0000162E)
;;     00001E0D (in fn00001DEE)
fn00001E3A proc
	movab	-44(sp),sp
	movl	+04(ap),r11
	bbc	#00000008,+10(r11),00001E4B

l00001E47:
	bisw2	#0001,+10(r11)

l00001E4B:
	blbs	+10(r11),00001E53

l00001E4F:
	mnegl	#00000001,r0
	ret

l00001E53:
	bitw	#0050,+10(r11)
	bneq	00001E4F

l00001E5B:
	brb	00001EA8

l00001E5D:
	pushal	-44(fp)
	cvtbl	+12(r11),-(sp)
	calls	#02,0000353C
	tstl	r0
	blss	00001E74

l00001E6F:
	tstl	-14(fp)
	bgtr	00001E7C

l00001E74:
	cvtwl	#0400,-04(fp)
	brb	00001E81

l00001E7C:
	movl	-14(fp),-04(fp)

l00001E81:
	cmpl	r11,#00003FD4
	bneq	00001E94

l00001E8A:
	moval	00005980,+08(r11)
	brb	00001EC5

l00001E94:
	pushl	-04(fp)
	calls	#01,000035D4
	movl	r0,+08(r11)
	bneq	00001EC1

l00001EA4:
	bisw2	#0004,+10(r11)

l00001EA8:
	tstl	+08(r11)
	bneq	00001ECA

l00001EAD:
	bbc	#00000002,+10(r11),00001E5D

l00001EB2:
	cvtbl	+12(r11),r0
	movab	0000A2DC[r0],+08(r11)
	brb	00001EA8

l00001EC1:
	bisw2	#0008,+10(r11)

l00001EC5:
	movl	-04(fp),+0C(r11)

l00001ECA:
	cmpl	r11,#00003FD4
	bneq	00001EFD

l00001ED3:
	bbc	#00000007,000043F8,00001EE8

l00001EDB:
	pushal	000043E8
	calls	#01,0000342A

l00001EE8:
	bbc	#00000007,0000440C,00001EFD

l00001EF0:
	pushal	000043FC
	calls	#01,0000342A

l00001EFD:
	bbc	#00000002,+10(r11),00001F07

l00001F02:
	movl	#00000001,r0
	brb	00001F0B

l00001F07:
	movl	+0C(r11),r0

l00001F0B:
	pushl	r0
	pushl	+08(r11)
	cvtbl	+12(r11),-(sp)
	calls	#03,000035B4
	movl	r0,(r11)
	movl	+08(r11),+04(r11)
	decl	(r11)
	bgeq	00001F48

l00001F27:
	cmpl	(r11),#FFFFFFFF
	bneq	00001F3F

l00001F30:
	bisw2	#0010,+10(r11)
	bbc	#00000008,+10(r11),00001F43

l00001F39:
	bicw2	#0001,+10(r11)
	brb	00001F43

l00001F3F:
	bisw2	#0020,+10(r11)

l00001F43:
	clrl	(r11)
	brw	00001E4F

l00001F48:
	movzbl	@+04(r11),r0
	incl	+04(r11)
	ret
00001F50 00 0F                                           ..              

;; fn00001F52: 00001F52
;;   Called from:
;;     000019BE (in fn000019AA)
;;     00001A23 (in fn00001A0C)
;;     00001BDE (in fn00001BCA)
;;     00001C43 (in fn00001C2C)
fn00001F52 proc
	movl	+08(ap),r11
	moval	000043D4,r8

l00001F5D:
	bitw	#0103,+10(r8)
	beql	00001F74

l00001F65:
	addl2	#00000014,r8
	cmpl	r8,00004564
	blss	00001F5D

l00001F71:
	clrl	r0
	ret

l00001F74:
	cmpb	+01(r11),#2B
	bneq	00001F7F

l00001F7A:
	movl	#00000001,r0
	brb	00001F81

l00001F7F:
	clrl	r0

l00001F81:
	movl	r0,r9
	cmpb	(r11),#77
	bneq	00001FB5

l00001F8A:
	pushl	#000001B6
	pushl	+04(ap)
	calls	#02,00002078
	movl	r0,r10
	tstl	r9
	bneq	00001FA4

l00001FA1:
	brw	00002041

l00001FA4:
	tstl	r10
	blss	00001FA1

l00001FA8:
	pushl	r10
	calls	#01,00003594
	pushl	#00000002
	brb	00002034

l00001FB5:
	cmpb	(r11),#61
	bneq	00002027

l00001FBB:
	tstl	r9
	beql	00001FC4

l00001FBF:
	movl	#00000002,r0
	brb	00001FC7

l00001FC4:
	movl	#00000001,r0

l00001FC7:
	pushl	r0
	pushl	+04(ap)
	calls	#02,000035A4
	movl	r0,r10
	bgeq	00002014

l00001FD8:
	cmpl	0000597C,#00000002
	bneq	00002014

l00001FE1:
	pushl	#000001B6
	pushl	+04(ap)
	calls	#02,00002078
	movl	r0,r10
	tstl	r9
	beql	00002014

l00001FF8:
	tstl	r10
	blss	00002014

l00001FFC:
	pushl	r10
	calls	#01,00003594
	pushl	#00000002
	pushl	+04(ap)
	calls	#02,000035A4
	movl	r0,r10

l00002014:
	tstl	r10
	blss	00002041

l00002018:
	pushl	#00000002
	pushl	#00000000
	pushl	r10
	calls	#03,00002A10
	brb	00002041

l00002027:
	tstl	r9
	beql	00002030

l0000202B:
	movl	#00000002,r0
	brb	00002032

l00002030:
	clrl	r0

l00002032:
	pushl	r0

l00002034:
	pushl	+04(ap)
	calls	#02,000035A4
	movl	r0,r10

l00002041:
	tstl	r10
	bgeq	00002048

l00002045:
	brw	00001F71

l00002048:
	clrl	(r8)
	cvtlb	r10,+12(r8)
	tstl	r9
	beql	00002059

l00002052:
	bbss	#00000008,+10(r8),00002069

l00002057:
	brb	00002069

l00002059:
	cmpb	(r11),#72
	beql	00002065

l0000205F:
	bisw2	#0002,+10(r8)
	brb	00002069

l00002065:
	bisw2	#0001,+10(r8)

l00002069:
	movl	r8,r0
	ret
0000206D                                        00 00 00              ...

;; fn00002070: 00002070
;;   Called from:
;;     0000207C (in fn0000207A)
fn00002070 proc
	jmp	000038D0
00002076                   00 00 00 00                         ....      

;; fn0000207A: 0000207A
;;   Called from:
;;     00001F93 (in fn00001F52)
;;     00001FEA (in fn00001F52)
fn0000207A proc
	chmk	#0008
	blssu	fn00002070

l0000207E:
	ret
0000207F                                              00                .
00002080 00 00                                           ..              

;; fn00002082: 00002082
;;   Called from:
;;     000006A8 (in fn00000564)
;;     00001CEB (in fn00001C2C)
fn00002082 proc
	rotl	#08,+04(ap),r0
	movb	+05(ap),r0
	movzwl	r0,r0
	ret
0000208F                                              00                .
00002090 00 0F                                           ..              

;; fn00002092: 00002092
;;   Called from:
;;     00000631 (in fn00000564)
;;     00001A97 (in fn00001A0C)
fn00002092 proc
	subl2	#00000018,sp
	movl	+04(ap),r11
	moval	-14(fp),-18(fp)

l0000209E:
	clrl	r10
	movl	#0000000A,r9
	cmpb	(r11),#30
	bneq	000020AD

l000020A8:
	movl	#00000008,r9
	incl	r11

l000020AD:
	cmpb	(r11),#78
	beql	000020B9

l000020B3:
	cmpb	(r11),#58
	bneq	00002116

l000020B9:
	movl	#00000010,r9
	brb	00002114

l000020BE:
	mull3	r9,r10,r0
	cvtbl	-01(fp),r1
	subl2	#00000030,r1
	brb	00002110

l000020CB:
	cvtbl	#41,r1
	brb	00002109

l000020D1:
	cvtbl	-01(fp),r0
	bbs	#00000002,00004339[r0],000020BE

l000020DE:
	cmpl	r9,#00000010
	bneq	0000211C

l000020E3:
	cvtbl	r0,r0
	bitb	#44,00004339[r0]
	beql	0000211C

l000020F1:
	cvtbl	-01(fp),r0
	addl2	#0000000A,r0
	cvtbl	-01(fp),r1
	bbc	#00000001,00004339[r1],000020CB

l00002105:
	cvtbl	#61,r1

l00002109:
	subl2	r1,r0
	ashl	#04,r10,r1

l00002110:
	addl3	r1,r0,r10

l00002114:
	incl	r11

l00002116:
	movb	(r11),-01(fp)
	bneq	000020D1

l0000211C:
	cmpb	(r11),#2E
	bneq	0000213C

l00002121:
	moval	-04(fp),r0
	cmpl	-18(fp),r0
	blss	0000212F

l0000212B:
	mnegl	#00000001,r0
	ret

l0000212F:
	movl	r10,@-18(fp)
	addl2	#00000004,-18(fp)
	incl	r11
	brw	0000209E

l0000213C:
	tstb	(r11)
	beql	0000214C

l00002140:
	cvtbl	(r11),r0
	bbc	#00000003,00004339[r0],0000212B

l0000214C:
	movl	r10,@-18(fp)
	addl2	#00000004,-18(fp)
	moval	-14(fp),r0
	subl3	r0,-18(fp),r0
	divl3	#00000004,r0,r8
	movl	r8,r0
	casel	r0,#00000001,#00000003
00002168                         0A 00 1E 00 2F 00               ..../.  

l0000216E:
	divf3	#0.5,#2.25,@-30(r9)
	xorw3	-04A522A6(ap),#0001,000021D0
	movl	r0,r10
	movl	r10,r0
	ret
00002186                   78 18 AD EC 50 EF 00 18 AD F0       x...P.....
00002190 51 C9 51 50 5A 11 DF 78 18 AD EC 50 9A AD F0 51 Q.QPZ..x...P...Q
000021A0 78 10 51 51 C8 51 50 EF 00 10 AD F4 51 11 E2 78 x.QQ.QP.....Q..x
000021B0 18 AD EC 50 9A AD F0 51 78 10 51 51 C8 51 50 9A ...P...Qx.QQ.QP.
000021C0 AD F4 51 78 08 51 51 C8 51 50 9A AD F8 51 11 C1 ..Qx.QQ.QP...Q..
000021D0 00 00 9C 8F F8 AC 04 50 F0 50 10 08 50 90 AC 07 .......P.P..P...
000021E0 50 04 00 00 00 08                               P.....          

;; fn000021E6: 000021E6
;;   Called from:
;;     000006D6 (in fn00000564)
;;     00000709 (in fn00000564)
;;     0000075C (in fn00000564)
;;     000014EB (in fn0000149A)
fn000021E6 proc
	subl2	#00000020,sp
	moval	-20(fp),r11
	tstl	+04(ap)
	beql	0000221A

l000021F2:
	tstb	@+04(ap)
	beql	0000221A

l000021F7:
	movl	+04(ap),(r11)
	pushl	+04(ap)
	calls	#01,000029B0
	movl	r0,+04(r11)
	addl2	#00000008,r11
	moval	000043BC,(r11)
	movl	#00000002,+04(r11)
	addl2	#00000008,r11

l0000221A:
	cmpl	0000597C,000043D0
	bgeq	00002238

l00002227:
	movl	0000597C,r0
	movl	0000227C[r0],r0
	brb	0000223F

l00002238:
	moval	000043BF,r0

l0000223F:
	movl	r0,(r11)
	pushl	(r11)
	calls	#01,000029B0
	movl	r0,+04(r11)
	addl2	#00000008,r11
	moval	000043CD,(r11)
	movl	#00000001,+04(r11)
	moval	-20(fp),r0
	subl3	r0,r11,r0
	divl2	#00000008,r0
	addl3	#00000001,r0,-(sp)
	pushal	-20(fp)
	pushl	#00000002
	calls	#03,000038C4
	ret
00002279                            00 00 00 94 1F 00 00          .......
00002280 9C 1F 00 00 A6 1F 00 00 C0 1F 00 00 D0 1F 00 00 ................
00002290 E8 1F 00 00 F2 1F 00 00 0C 20 00 00 1E 20 00 00 ......... ... ..
000022A0 30 20 00 00 40 20 00 00 4C 20 00 00 5E 20 00 00 0 ..@ ..L ..^ ..
000022B0 6E 20 00 00 80 20 00 00 8C 20 00 00 A2 20 00 00 n ... ... ... ..
000022C0 B4 20 00 00 C0 20 00 00 D2 20 00 00 E1 20 00 00 . ... ... ... ..
000022D0 F1 20 00 00 00 21 00 00 11 21 00 00 25 21 00 00 . ...!...!..%!..
000022E0 39 21 00 00 4A 21 00 00 59 21 00 00 68 21 00 00 9!..J!..Y!..h!..
000022F0 80 21 00 00 8D 21 00 00 A3 21 00 00 B2 21 00 00 .!...!...!...!..
00002300 BE 21 00 00 D1 21 00 00 E2 21 00 00 F8 21 00 00 .!...!...!...!..
00002310 12 22 00 00 30 22 00 00 4F 22 00 00 6C 22 00 00 ."..0"..O"..l"..
00002320 7D 22 00 00 9C 22 00 00 B3 22 00 00 CA 22 00 00 }"..."..."..."..
00002330 E4 22 00 00 06 23 00 00 24 23 00 00 54 23 00 00 ."...#..$#..T#..
00002340 6B 23 00 00 8A 23 00 00 9A 23 00 00 B1 23 00 00 k#...#...#...#..
00002350 D5 23 00 00 F6 23 00 00 0F 24 00 00 29 24 00 00 .#...#...$..)$..
00002360 45 24 00 00 5D 24 00 00 7E 24 00 00 A0 24 00 00 E$..]$..~$...$..
00002370 B5 24 00 00 C8 24 00 00 EA 24 00 00 FD 24 00 00 .$...$...$...$..
00002380 0A 25 00 00 1E 25 00 00 32 25 00 00 45 25 00 00 .%...%..2%..E%..
00002390 54 25 00 00 45 72 72 6F 72 20 30 00 4E 6F 74 20 T%..Error 0.Not 
000023A0 6F 77 6E 65 72 00 4E 6F 20 73 75 63 68 20 66 69 owner.No such fi
000023B0 6C 65 20 6F 72 20 64 69 72 65 63 74 6F 72 79 00 le or directory.
000023C0 4E 6F 20 73 75 63 68 20 70 72 6F 63 65 73 73 00 No such process.
000023D0 49 6E 74 65 72 72 75 70 74 65 64 20 73 79 73 74 Interrupted syst
000023E0 65 6D 20 63 61 6C 6C 00 49 2F 4F 20 65 72 72 6F em call.I/O erro
000023F0 72 00 4E 6F 20 73 75 63 68 20 64 65 76 69 63 65 r.No such device
00002400 20 6F 72 20 61 64 64 72 65 73 73 00 41 72 67 20  or address.Arg 
00002410 6C 69 73 74 20 74 6F 6F 20 6C 6F 6E 67 00 45 78 list too long.Ex
00002420 65 63 20 66 6F 72 6D 61 74 20 65 72 72 6F 72 00 ec format error.
00002430 42 61 64 20 66 69 6C 65 20 6E 75 6D 62 65 72 00 Bad file number.
00002440 4E 6F 20 63 68 69 6C 64 72 65 6E 00 4E 6F 20 6D No children.No m
00002450 6F 72 65 20 70 72 6F 63 65 73 73 65 73 00 4E 6F ore processes.No
00002460 74 20 65 6E 6F 75 67 68 20 63 6F 72 65 00 50 65 t enough core.Pe
00002470 72 6D 69 73 73 69 6F 6E 20 64 65 6E 69 65 64 00 rmission denied.
00002480 42 61 64 20 61 64 64 72 65 73 73 00 42 6C 6F 63 Bad address.Bloc
00002490 6B 20 64 65 76 69 63 65 20 72 65 71 75 69 72 65 k device require
000024A0 64 00 4D 6F 75 6E 74 20 64 65 76 69 63 65 20 62 d.Mount device b
000024B0 75 73 79 00 46 69 6C 65 20 65 78 69 73 74 73 00 usy.File exists.
000024C0 43 72 6F 73 73 2D 64 65 76 69 63 65 20 6C 69 6E Cross-device lin
000024D0 6B 00 4E 6F 20 73 75 63 68 20 64 65 76 69 63 65 k.No such device
000024E0 00 4E 6F 74 20 61 20 64 69 72 65 63 74 6F 72 79 .Not a directory
000024F0 00 49 73 20 61 20 64 69 72 65 63 74 6F 72 79 00 .Is a directory.
00002500 49 6E 76 61 6C 69 64 20 61 72 67 75 6D 65 6E 74 Invalid argument
00002510 00 46 69 6C 65 20 74 61 62 6C 65 20 6F 76 65 72 .File table over
00002520 66 6C 6F 77 00 54 6F 6F 20 6D 61 6E 79 20 6F 70 flow.Too many op
00002530 65 6E 20 66 69 6C 65 73 00 4E 6F 74 20 61 20 74 en files.Not a t
00002540 79 70 65 77 72 69 74 65 72 00 54 65 78 74 20 66 ypewriter.Text f
00002550 69 6C 65 20 62 75 73 79 00 46 69 6C 65 20 74 6F ile busy.File to
00002560 6F 20 6C 61 72 67 65 00 4E 6F 20 73 70 61 63 65 o large.No space
00002570 20 6C 65 66 74 20 6F 6E 20 64 65 76 69 63 65 00  left on device.
00002580 49 6C 6C 65 67 61 6C 20 73 65 65 6B 00 52 65 61 Illegal seek.Rea
00002590 64 2D 6F 6E 6C 79 20 66 69 6C 65 20 73 79 73 74 d-only file syst
000025A0 65 6D 00 54 6F 6F 20 6D 61 6E 79 20 6C 69 6E 6B em.Too many link
000025B0 73 00 42 72 6F 6B 65 6E 20 70 69 70 65 00 41 72 s.Broken pipe.Ar
000025C0 67 75 6D 65 6E 74 20 74 6F 6F 20 6C 61 72 67 65 gument too large
000025D0 00 52 65 73 75 6C 74 20 74 6F 6F 20 6C 61 72 67 .Result too larg
000025E0 65 00 4F 70 65 72 61 74 69 6F 6E 20 77 6F 75 6C e.Operation woul
000025F0 64 20 62 6C 6F 63 6B 00 4F 70 65 72 61 74 69 6F d block.Operatio
00002600 6E 20 6E 6F 77 20 69 6E 20 70 72 6F 67 72 65 73 n now in progres
00002610 73 00 4F 70 65 72 61 74 69 6F 6E 20 61 6C 72 65 s.Operation alre
00002620 61 64 79 20 69 6E 20 70 72 6F 67 72 65 73 73 00 ady in progress.
00002630 53 6F 63 6B 65 74 20 6F 70 65 72 61 74 69 6F 6E Socket operation
00002640 20 6F 6E 20 6E 6F 6E 2D 73 6F 63 6B 65 74 00 44  on non-socket.D
00002650 65 73 74 69 6E 61 74 69 6F 6E 20 61 64 64 72 65 estination addre
00002660 73 73 20 72 65 71 75 69 72 65 64 00 4D 65 73 73 ss required.Mess
00002670 61 67 65 20 74 6F 6F 20 6C 6F 6E 67 00 50 72 6F age too long.Pro
00002680 74 6F 63 6F 6C 20 77 72 6F 6E 67 20 74 79 70 65 tocol wrong type
00002690 20 66 6F 72 20 73 6F 63 6B 65 74 00 50 72 6F 74  for socket.Prot
000026A0 6F 63 6F 6C 20 6E 6F 74 20 61 76 61 69 6C 61 62 ocol not availab
000026B0 6C 65 00 50 72 6F 74 6F 63 6F 6C 20 6E 6F 74 20 le.Protocol not 
000026C0 73 75 70 70 6F 72 74 65 64 00 53 6F 63 6B 65 74 supported.Socket
000026D0 20 74 79 70 65 20 6E 6F 74 20 73 75 70 70 6F 72  type not suppor
000026E0 74 65 64 00 4F 70 65 72 61 74 69 6F 6E 20 6E 6F ted.Operation no
000026F0 74 20 73 75 70 70 6F 72 74 65 64 20 6F 6E 20 73 t supported on s
00002700 6F 63 6B 65 74 00 50 72 6F 74 6F 63 6F 6C 20 66 ocket.Protocol f
00002710 61 6D 69 6C 79 20 6E 6F 74 20 73 75 70 70 6F 72 amily not suppor
00002720 74 65 64 00 41 64 64 72 65 73 73 20 66 61 6D 69 ted.Address fami
00002730 6C 79 20 6E 6F 74 20 73 75 70 70 6F 72 74 65 64 ly not supported
00002740 20 62 79 20 70 72 6F 74 6F 63 6F 6C 20 66 61 6D  by protocol fam
00002750 69 6C 79 00 41 64 64 72 65 73 73 20 61 6C 72 65 ily.Address alre
00002760 61 64 79 20 69 6E 20 75 73 65 00 43 61 6E 27 74 ady in use.Can't
00002770 20 61 73 73 69 67 6E 20 72 65 71 75 65 73 74 65  assign requeste
00002780 64 20 61 64 64 72 65 73 73 00 4E 65 74 77 6F 72 d address.Networ
00002790 6B 20 69 73 20 64 6F 77 6E 00 4E 65 74 77 6F 72 k is down.Networ
000027A0 6B 20 69 73 20 75 6E 72 65 61 63 68 61 62 6C 65 k is unreachable
000027B0 00 4E 65 74 77 6F 72 6B 20 64 72 6F 70 70 65 64 .Network dropped
000027C0 20 63 6F 6E 6E 65 63 74 69 6F 6E 20 6F 6E 20 72  connection on r
000027D0 65 73 65 74 00 53 6F 66 74 77 61 72 65 20 63 61 eset.Software ca
000027E0 75 73 65 64 20 63 6F 6E 6E 65 63 74 69 6F 6E 20 used connection 
000027F0 61 62 6F 72 74 00 43 6F 6E 6E 65 63 74 69 6F 6E abort.Connection
00002800 20 72 65 73 65 74 20 62 79 20 70 65 65 72 00 4E  reset by peer.N
00002810 6F 20 62 75 66 66 65 72 20 73 70 61 63 65 20 61 o buffer space a
00002820 76 61 69 6C 61 62 6C 65 00 53 6F 63 6B 65 74 20 vailable.Socket 
00002830 69 73 20 61 6C 72 65 61 64 79 20 63 6F 6E 6E 65 is already conne
00002840 63 74 65 64 00 53 6F 63 6B 65 74 20 69 73 20 6E cted.Socket is n
00002850 6F 74 20 63 6F 6E 6E 65 63 74 65 64 00 43 61 6E ot connected.Can
00002860 27 74 20 73 65 6E 64 20 61 66 74 65 72 20 73 6F 't send after so
00002870 63 6B 65 74 20 73 68 75 74 64 6F 77 6E 00 54 6F cket shutdown.To
00002880 6F 20 6D 61 6E 79 20 72 65 66 65 72 65 6E 63 65 o many reference
00002890 73 3A 20 63 61 6E 27 74 20 73 70 6C 69 63 65 00 s: can't splice.
000028A0 43 6F 6E 6E 65 63 74 69 6F 6E 20 74 69 6D 65 64 Connection timed
000028B0 20 6F 75 74 00 43 6F 6E 6E 65 63 74 69 6F 6E 20  out.Connection 
000028C0 72 65 66 75 73 65 64 00 54 6F 6F 20 6D 61 6E 79 refused.Too many
000028D0 20 6C 65 76 65 6C 73 20 6F 66 20 73 79 6D 62 6F  levels of symbo
000028E0 6C 69 63 20 6C 69 6E 6B 73 00 46 69 6C 65 20 6E lic links.File n
000028F0 61 6D 65 20 74 6F 6F 20 6C 6F 6E 67 00 48 6F 73 ame too long.Hos
00002900 74 20 69 73 20 64 6F 77 6E 00 48 6F 73 74 20 69 t is down.Host i
00002910 73 20 75 6E 72 65 61 63 68 61 62 6C 65 00 44 69 s unreachable.Di
00002920 72 65 63 74 6F 72 79 20 6E 6F 74 20 65 6D 70 74 rectory not empt
00002930 79 00 54 6F 6F 20 6D 61 6E 79 20 70 72 6F 63 65 y.Too many proce
00002940 73 73 65 73 00 54 6F 6F 20 6D 61 6E 79 20 75 73 sses.Too many us
00002950 65 72 73 00 44 69 73 63 20 71 75 6F 74 61 20 65 ers.Disc quota e
00002960 78 63 65 65 64 65 64 00 00 08                   xceeded...      

;; fn0000296A: 0000296A
;;   Called from:
;;     000019D4 (in fn000019AA)
;;     00001BF4 (in fn00001BCA)
fn0000296A proc
	movl	+04(ap),r11
	pushl	r11
	calls	#01,0000342A
	pushl	#00000000
	pushl	#00000000
	cvtbl	+12(r11),-(sp)
	calls	#03,00002A10
	clrl	(r11)
	movl	+08(r11),+04(r11)
	bicw2	#0030,+10(r11)
	bbc	#00000008,+10(r11),0000299A

l00002996:
	bicw2	#0003,+10(r11)

l0000299A:
	ret
0000299B                                  00                        .    

l0000299C:
	jmp	000038D0
000029A2       00 00 00 00                                 ....          

;; fn000029A6: 000029A6
;;   Called from:
;;     000006BE (in fn00000564)
fn000029A6 proc
	chmk	#0061
	blssu	0000299C

l000029AC:
	ret
000029AD                                        00 00 00              ...
000029B0 00 0C                                           ..              

;; fn000029B2: 000029B2
;;   Called from:
;;     000005AC (in fn00000564)
;;     000021FE (in fn000021E6)
;;     00002244 (in fn000021E6)
fn000029B2 proc
	movl	+04(ap),r11
	clrl	r10
	brb	000029BC

l000029BA:
	incl	r10

l000029BC:
	tstb	(r11)+
	bneq	000029BA

l000029C0:
	movl	r10,r0
	ret
000029C4             00 00                                   ..          

;; fn000029C6: 000029C6
;;   Called from:
;;     00000718 (in fn00000564)
;;     00000727 (in fn00000564)
;;     00000767 (in fn00000564)
;;     00000D84 (in fn00000D40)
fn000029C6 proc
	subl2	#00000018,sp
	movl	+08(ap),-18(fp)
	clrl	-10(fp)
	movl	-10(fp),-14(fp)
	pushal	-0C(fp)
	pushal	-18(fp)
	pushl	+04(ap)
	calls	#03,000029FC
	tstl	r0
	bgeq	000029EE

l000029EA:
	mnegl	#00000001,r0
	ret

l000029EE:
	movl	-0C(fp),r0
	ret
000029F3          00                                        .            

;; fn000029F4: 000029F4
;;   Called from:
;;     00002A02 (in fn000029FE)
fn000029F4 proc
	jmp	000038D0
000029FA                               00 00 00 00                 ....  

;; fn000029FE: 000029FE
;;   Called from:
;;     000018BA (in fn0000181E)
;;     000018F5 (in fn0000181E)
;;     000029DF (in fn000029C6)
fn000029FE proc
	chmk	#006C
	blssu	fn000029F4

l00002A04:
	ret
00002A05                00 00 00                              ...        

l00002A08:
	jmp	000038D0
00002A0E                                           00 00               ..
00002A10 00 00                                           ..              

;; fn00002A12: 00002A12
;;   Called from:
;;     0000201E (in fn00001F52)
;;     0000297F (in fn0000296A)
fn00002A12 proc
	chmk	#0013
	blssu	00002A08

l00002A16:
	ret
00002A17                      00 00 00 C2 14 5E 99 8F 42        .....^..B
00002A20 AD FC D0 AC 04 AD F0 32 8F FF 7F AD EC DF AD EC .......2........
00002A30 C1 0C 5C 7E DD AC 08 FB 03 EF 22 01 00 00 D7 AD ..\~......".....
00002A40 EC 19 0B D0 AD F0 50 D6 AD F0 94 60 11 0C DF AD ......P....`....
00002A50 EC DD 00 FB 02 EF 72 08 00 00 D0 AC 04 50 04 00 ......r......P..
00002A60 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F ................
00002A70 10 11 12 13 14 15 16 17 18 19 1A 1B 1C 1D 1E 1F ................
00002A80 20 21 22 23 24 00 26 27 28 29 2A 2B 2C 2D 2E 2F  !"#$.&'()*+,-./
00002A90 30 31 32 33 34 35 36 37 38 39 3A 3B 3C 3D 3E 3F 0123456789:;<=>?
00002AA0 40 41 42 43 44 45 46 47 48 49 4A 4B 4C 4D 4E 4F @ABCDEFGHIJKLMNO
00002AB0 50 51 52 53 54 55 56 57 58 59 5A 5B 5C 5D 5E 5F PQRSTUVWXYZ[\]^_
00002AC0 60 61 62 63 64 65 66 67 68 69 6A 6B 6C 6D 6E 6F `abcdefghijklmno
00002AD0 70 71 72 73 74 75 76 77 78 79 7A 7B 7C 7D 7E 7F pqrstuvwxyz{|}~.
00002AE0 80 81 82 83 84 85 86 87 88 89 8A 8B 8C 8D 8E 8F ................
00002AF0 90 91 92 93 94 95 96 97 98 99 9A 9B 9C 9D 9E 9F ................
00002B00 A0 A1 A2 A3 A4 A5 A6 A7 A8 A9 AA AB AC AD AE AF ................
00002B10 B0 B1 B2 B3 B4 B5 B6 B7 B8 B9 BA BB BC BD BE BF ................
00002B20 C0 C1 C2 C3 C4 C5 C6 C7 C8 C9 CA CB CC CD CE CF ................
00002B30 D0 D1 D2 D3 D4 D5 D6 D7 D8 D9 DA DB DC DD DE DF ................
00002B40 E0 E1 E2 E3 E4 E5 E6 E7 E8 E9 EA EB EC ED EE EF ................
00002B50 F0 F1 F2 F3 F4 F5 F6 F7 F8 F9 FA FB FC FD FE FF ................
00002B60 C0 0F                                           ..              

;; fn00002B62: 00002B62
;;   Called from:
;;     000016B0 (in fn000016A6)
;;     000016D7 (in fn000016CA)
fn00002B62 proc
	brw	00002BEB

l00002B65:
	clrl	r4
	brb	00002BA0

l00002B69:
	movzbl	(r1)+,r2
	tstb	00002A60[r2]
	beql	00002BD9

;; fn00002B75: 00002B75
;;   Called from:
;;     00002B73 (in fn00002B91)
;;     00002E7F (in fn00002B62)
fn00002B75 proc
	pushr	#0003
	pushl	-04(fp)
	pushl	r2
	calls	#02,000032CC
	tstl	r0
	bgeq	00002B8C

l00002B87:
	bbcs	#0000001F,-10(fp),00002B8C

l00002B8C:
	incl	-10(fp)
	popr	#0003

;; fn00002B91: 00002B91
;;   Called from:
;;     00002C1A (in fn00002B62)
;;     00002E5D (in fn00002B62)
;;     00002E6C (in fn00002B62)
fn00002B91 proc
	movab	00002A60,r3
	movq	@-04(fp),r4
	bbs	#0000001F,r4,00002B65

l00002BA0:
	addl2	r0,-10(fp)
	clrl	r2
	tstl	r0
	bleq	00002BC9

l00002BAA:
	tstl	r4
	bleq	00002BC9

l00002BAE:
	movzbl	(r1)+,r3
	tstb	00002A60[r3]
	bneq	00002BC1

l00002BBA:
	mnegl	#00000001,r2
	decl	r1
	brb	00002BC9

l00002BC1:
	movb	r3,(r5)+
	decl	r4
	sobgtr	r0,00002BAA

l00002BC9:
	movq	r4,@-04(fp)
	subl2	r0,-10(fp)
	bbs	#00000001,r2,00002BD8

l00002BD5:
	sobgeq	r0,00002B69

l00002BD8:
	rsb

l00002BD9:
	incl	r0
	decl	r1
	movl	#00000002,r2
	rsb

l00002BE1:
	bbcs	#0000001F,-10(fp),00002BE6

l00002BE6:
	movl	-10(fp),r0
	ret

l00002BEB:
	movab	-0100(sp),sp
	movl	+04(ap),r11
	movl	+0C(ap),-04(fp)
	movl	+08(ap),ap
	clrl	-10(fp)

l00002C00:
	movzwl	#FFFF,r0
	movl	r11,r1
	movq	@-04(fp),r4
	subl3	r1,r5,r2
	blss	00002C1A

l00002C12:
	cmpl	r0,r2
	bleq	00002C1A

l00002C17:
	movl	r2,r0

l00002C1A:
	bsbw	fn00002B91
	movl	r1,r11
	bbc	#00000001,r2,00002C00

l00002C24:
	tstb	(r11)+
	beql	00002BE6

l00002C28:
	movl	sp,r5
	clrq	r9
	clrq	r6
	movzbl	(r11)+,r0
	caseb	r0,#20,#58
00002C37                      9C 02 B2 00 B2 00 A6 02 B2        .........
00002C40 00 B2 00 B2 00 B2 00 B2 00 B2 00 EC 02 AC 02 B2 ................
00002C50 00 B2 02 E1 02 B2 00 B8 02 C3 02 C3 02 C3 02 C3 ................
00002C60 02 C3 02 C3 02 C3 02 C3 02 C3 02 B2 00 B2 00 B2 ................
00002C70 00 B2 00 B2 00 B2 00 B2 00 B2 00 B2 00 B2 00 9F ................
00002C80 01 BD 00 B2 00 BD 00 B2 00 B2 00 B2 00 B2 00 B2 ................
00002C90 00 B2 00 B2 00 04 01 B2 00 B2 00 B2 00 B2 00 B2 ................
00002CA0 00 73 01 B2 00 B2 00 13 01 B2 00 B2 00 B2 00 B2 .s..............
00002CB0 00 B2 00 B2 00 B2 00 B2 00 B2 00 B2 00 90 02 9F ................
00002CC0 01 1E 04 07 03 E4 04 F8 FF B2 00 B2 00 B2 00 F8 ................
00002CD0 FF B2 00 B2 00 04 01 B2 00 B2 00 B2 00 CA 00 B2 ................
00002CE0 00 73 01 B2 00 B2 00                            .s.....         

l00002CE7:
	beql	00002CEA

l00002CE9:
	movb	r0,(r5)+

l00002CEA:
	Invalid

l00002CEC:
	bneq	00002CF1

l00002CEE:
	brw	00002BE1

l00002CF1:
	brw	00002ECD
00002CF4             C8 8F 40 00 00 00 5A 8C 20 50 31 31     ..@...Z. P11
00002D00 FF D0 58 50 E0 01 5A 03 CE 01 50 D0 8C 52 3A 00 ..XP..Z...P..R:.
00002D10 50 62 D0 51 55 D0 52 51 31 21 01 30 31 32 33 34 Pb.QU.RQ1!.01234
00002D20 35 36 37 38 39 61 62 63 64 65 66 30 31 32 33 34 56789abcdef01234
00002D30 35 36 37 38 39 41 42 43 44 45 46 D0 1E 52 D0 03 56789ABCDEF..R..
00002D40 53 9E EF D4 FF FF FF 57 11 18 D0 1C 52 D0 04 53 S......W....R..S
00002D50 9E EF C5 FF FF FF 57 E1 06 5A 07 9E EF CA FF FF ......W..Z......
00002D60 FF 57 CE 53 56 D4 51 C0 04 55 D0 8C 50 EF 52 53 .W.SV.Q..U..P.RS
00002D70 50 51 90 41 67 85 F1 00 56 52 F1 FF 7C 56 94 65 PQ.Ag...VR..|V.e
00002D80 3B 30 0B AE 04 E1 05 5A 7A D5 AC FC 13 75 D1 53 ;0.....Zz....u.S
00002D90 04 12 12 90 8F 78 50 E1 06 5A 04 90 8F 58 50 90 .....xP..Z...XP.
00002DA0 50 71 D0 02 57 90 30 71 11 59 CA 8F 90 00 00 00 Pq..W.0q.Y......
00002DB0 5A EF 01 1F 6C 50 F9 50 0A 6E 34 0A 6E AE 08 20 Z...lP.P.n4.n.. 
00002DC0 0A AE 08 0A 6E E9 8C 16 20 01 EF F7 00 00 00 0A ....n... .......
00002DD0 6E 11 0B AA 01 00 F9 8C 0A 6E 18 02 D6 57 38 0A n........n...W8.
00002DE0 6E EF ED FF FF FF AE 08 3B 20 0B AE 08 D5 57 12 n.......; ....W.
00002DF0 12 E1 04 5A 05 90 2B 71 11 07 E1 07 5A 05 90 20 ...Z..+q....Z.. 
00002E00 71 D6 57 E0 01 5A 03 D0 01 58 C3 51 55 56 C2 57 q.W..Z...X.QUV.W
00002E10 56 C2 56 58 15 26 C0 57 51 DD 51 9E 48 A5 20 52 V.VX.&.WQ.Q.H. R
00002E20 C2 5D 52 19 03 C2 52 58 28 56 61 48 61 2C 00 61 .]R...RX(VaHa,.a
00002E30 30 58 BE 00 C3 57 8E 51 C1 56 53 55             0X...W.Q.VSU    

l00002E3C:
	subl3	r1,r5,r8
	subl3	r8,r9,r0
	bleq	00002E66

l00002E46:
	bbs	#00000003,r10,00002E66

l00002E4A:
	bbs	#00000002,r10,00002E52

l00002E4E:
	bsbb	fn00002EA2
	brb	00002E66

l00002E52:
	movl	r7,r0
	bleq	00002E60

l00002E57:
	subl2	r0,r8
	subl2	r0,r9
	bsbw	fn00002B91

l00002E60:
	subl3	r8,r9,r0
	bsbb	fn00002E9D

l00002E66:
	subl2	r8,r9
	movl	r8,r0

l00002E6C:
	bsbw	fn00002B91

l00002E6F:
	bbc	#00000001,r2,00002E90

l00002E73:
	decl	r0
	movzbl	(r1)+,r2
	movq	@-04(fp),r4
	sobgeq	r4,00002E84

l00002E7F:
	bsbw	fn00002B75
	brb	00002E6F

l00002E84:
	movb	r2,(r5)+
	incl	-10(fp)
	movq	r4,@-04(fp)
	brb	00002E6C

l00002E90:
	movl	r9,r0
	bgtr	00002E98

l00002E95:
	brw	00002C00

l00002E98:
	bsbb	fn00002EA2
	brw	00002C00

;; fn00002E9D: 00002E9D
;;   Called from:
;;     00002E64 (in fn00002B62)
fn00002E9D proc
	movb	#30,r2
	brb	fn00002EA5

;; fn00002EA2: 00002EA2
;;   Called from:
;;     00002E4E (in fn00002B62)
;;     00002E98 (in fn00002B62)
fn00002EA2 proc
	movb	#20,r2

;; fn00002EA5: 00002EA5
;;   Called from:
;;     00002EA0 (in fn00002E9D)
;;     00002EA2 (in fn00002EA2)
fn00002EA5 proc
	subl2	r0,r9
	pushl	r1
	movl	r0,r7
	subl2	r0,sp
	movc5
	halt
	addd2	r2,r7
	cvtld	@+5057(r0),@+515E(r0)
	bsbw	fn00002B91
	addl2	r7,sp
	movl	(sp)+,r1
	rsb
	bvc	00002E98
	xorb2	r0,@(r0)+
	movf	(r5)+,@+515E(r0)

l00002ECD:
	movl	sp,r1
	brw	00002E3C
00002ED3          C8 8F 80 00 00 00 5A 31 52 FD C8 20 5A    ......Z1R.. Z
00002EE0 31 4C FD C8 10 5A 31 46 FD C8 08 5A 31 40 FD E0 1L...Z1F...Z1@..
00002EF0 00 5A 07 E0 01 5A 12 C8 04 5A E0 01 5A 0B DE 49 .Z...Z...Z..Z..I
00002F00 69 59 3E 49 A0 D0 59 11 09 DE 48 68 58 3E 48 A0 iY>I..Y...HhX>H.
00002F10 D0 58 C8 01 5A 31 17 FD D4 58 C8 02 5A CA 01 5A .X..Z1...X..Z..Z
00002F20 31 0C FD E0 01 5A 0D D0 8C 59 18 E6 CC 08 5A CE 1....Z...Y....Z.
00002F30 59 59 11 DE D0 8C 58 18 D9 CE 58 58 11 D4 E0 01 YY....X...XX....
00002F40 5A 03 D0 06 58 30 EB 01 C1 AD F8 58 57 D0 57 56 Z...X0.....XW.WV
00002F50 18 02 D4 57 D1 57 1F 15 03 D0 1F 57 C3 11 57 50 ...W.W.....W..WP
00002F60 F8 50 11 6E 05 57 AE 10 1C 23 F3 00 56 1F D0 57 .P.n.W...#..V..W
00002F70 50 D6 AD F8 D6 57 F8 50 01 EF 48 FF FF FF 00 57 P....W.P..H....W
00002F80 AE 10 78 8F FF 57 50 88 AD EF 40 AE 10 9E AE 10 ..x..WP...@.....
00002F90 51 D0 57 56 9F EF 53 FE FF FF 9F A1 30 9E A1 30 Q.WV..S.....0..0
00002FA0 53 90 03 83 2C 00 61 8F 91 56 63 94 63 38 56 61 S...,.a..Vc.c8Va
00002FB0 A1 30 A1 10 C3 56 55 51 D0 6E 53 D4 57 E9 AD EF .0...VUQ.nS.W...
00002FC0 05 90 2D 83 D6 57 D0 AD F8 50 14 05 90 30 83 11 ..-..W...P...0..
00002FD0 1F D1 50 56 15 03 D0 56 50 C2 50 56 C2 50 AD F8 ..PV...VP.PV.P..
00002FE0 28 50 61 63 D0 AD F8 50 15 06 2C 00 61 30 50 63 (Pac...P..,.a0Pc
00002FF0 D0 58 50 14 08 E0 05 5A 04 E3 09 5A 03 90 2E 83 .XP....Z...Z....
00003000 CE AD F8 50 15 11 D1 50 58 15 03 D0 58 50 C2 50 ...P...PX...XP.P
00003010 58 2C 00 61 30 50 63 D0 58 50 D1 50 56 15 03 D0 X,.a0Pc.XP.PV...
00003020 56 50 C2 50 58 28 50 61 63 D0 58 50 15 19 9E 40 VP.PX(Pac.XP...@
00003030 A3 20 52 C2 5D 52 19 06 C2 52 50 D0 50 58 C2 50 . R.]R...RP.PX.P
00003040 58 2C 00 61 30 50 63 D0 53 55 BA 02 05 03 44 65 X,.a0Pc.SU....De
00003050 42 2B 04 92 00 D6 58 E0 01 5A 03 D0 07 58 30 D2 B+....X..Z...X0.
00003060 00 D0 58 57 D1 57 1F 15 03 D0 1F 57 C3 11 57 50 ..XW.W.....W..WP
00003070 F8 50 11 6E 05 57 AE 10 1C 1E D6 AD F8 C3 01 57 .P.n.W.........W
00003080 50 F8 50 01 EF 3D FE FF FF 00 57 AE 10 78 8F FF P.P..=....W..x..
00003090 57 50 88 AD EF 40 AE 10 E0 08 5A 3D 9E AE 10 51 WP...@....Z=...Q
000030A0 10 26 D0 51 56 C3 01 AD F4 50 F9 50 02 6E 38 02 .&.QV....P.P.n8.
000030B0 6E EF 97 FF FF FF 65 D0 56 51 E0 06 5A 03 31 2C n.....e.VQ..Z.1,
000030C0 FD 8C 20 A5 FC 31 25 FD D0 57 56 D7 58 D0 AD F8 .. ..1%..WV.X...
000030D0 AD F4 D0 01 AD F8 31 C1 FE C1 03 AD F8 50 19 30 ......1......P.0
000030E0 C2 03 50 D1 50 58 14 28 D0 57 56 C2 50 58 9E AE ..P.PX.(.WV.PX..
000030F0 10 51 30 A5 FE E0 05 5A 10 E0 09 5A 0C 91 75 30 .Q0....Z...Z..u0
00003100 13 FB 91 65 2E 13 02 D6 55 E1 08 5A 95 31 DD FC ...e....U..Z.1..
00003110 9E AE 10 51 30 B1 FF E4 08 5A DA E0 01 5A 03 D0 ...Q0....Z...Z..
00003120 06 58 D5 58 14 03 D0 01 58 E2 08 5A 03 31 2E FF .X.X....X..Z.1..
00003130 31 2B FF 94 AD EF 70 8C 55 12 03 31 9F 00 14 06 1+....p.U..1....
00003140 72 55 55 96 AD EF EF 07 08 55 52 9E A2 80 52 C4 rUU......UR...R.
00003150 3B 52 19 05 9E C2 C4 00 52 9E A2 9E 52 C6 8F C4 ;R......R...R...
00003160 00 00 00 52 30 85 00 71 50 55 14 02 D6 52 D0 52 ...R0..qPU...R.R
00003170 AD F8 CE 52 52 D1 52 1D 15 0A 64 EF E4 00 00 00 ...RR.R...d.....
00003180 55 C2 10 52 C0 09 52 16 EF 5F 00 00 00 74 50 54 U..R..R.._...tPT
00003190 55 50 55 F9 50 09 AE 10 F8 08 09 AE 10 00 11 AE UPU.P...........
000031A0 04 74 EF B5 00 00 00 00 55 50 55 F9 50 08 AE 10 .t......UPU.P...
000031B0 D0 58 57 9F EF 9C FE FF FF D1 8E 6E 15 04 C0 AD .XW........n....
000031C0 F8 57 D1 57 11 19 09 71 55 00 19 04 88 10 AE 0C .W.W...qU.......
000031D0 20 08 AE 10 11 AE 04 88 AD EF AE 0C 05 D4 50 D0  .............P.
000031E0 01 AD F8 11 AE 00 00 00 00 00 01 00 70 08 50 D4 ............p.P.
000031F0 54 7E EF 4D 00 00 00 53 D5 52 18 07 CE 52 52 E2 T~.M...S.R...RR.
00003200 06 52 00 E1 54 52 03 64 63 50 C0 08 53 F3 05 54 .R..TR.dcP..S..T
00003210 F2 E5 06 52 13 67 50 08 50 D1 1C 52 12 07 C0 EF ...R.gP.P..R....
00003220 C4 FF FF FF 51 CE 52 52 C1 28 52 53 E1 53 EF 41 ....Q.RR.(RS.S.A
00003230 00 00 00 07 C2 EF AE FF FF FF 51 9A 42 EF 62 00 ..........Q.B.b.
00003240 00 00 54 05 20 42 00 00 00 00 00 00 C8 43 00 00 ..T. B.......C..
00003250 00 00 00 00 1C 47 00 40 00 00 00 00 BE 4D 20 BC .....G.@.....M .
00003260 00 00 00 00 0E 5B C9 1B 04 BF 00 00 9D 75 AD C5 .....[.......u..
00003270 2B A8 B6 70 50 42 3C F2 87 00 00 00 F6 7F 56 76 +..pPB<.......Vv
00003280 D3 88 B5 62 BA F5 32 3E 0E 48 DB 51 53 27 B1 EF ...b..2>.H.QS'..
00003290 EB A5 07 49 5B D9 0F 13 CD FF BF 97 FD BC B6 23 ...I[..........#
000032A0 2C 3B 0A CD 00 00 00 00 00 00 00 00 00 00 00 00 ,;..............
000032B0 00 00 00 00 00 00 00 00 00 00 00 00 00 A0 C8 3A ...............:
000032C0 84 E4 DC 92 9B 00 C0 58 AE 18 EF 00 00 0F       .......X......  

;; fn000032CE: 000032CE
;;   Called from:
;;     00000D77 (in fn00000D40)
;;     00000E0E (in fn00000D40)
;;     00002B7C (in fn00002B75)
fn000032CE proc
	movab	-48(sp),sp
	movl	+08(ap),r11
	bbc	#00000008,+10(r11),000032E3

l000032DB:
	bisw2	#0002,+10(r11)
	bicw2	#0011,+10(r11)

l000032E3:
	bbc	#00000001,+10(r11),000032EB

l000032E8:
	brw	0000337D

l000032EB:
	mnegl	#00000001,r0
	ret

l000032EF:
	clrl	r9
	movl	r9,r8
	brb	00003311

l000032F6:
	cvtlb	+04(ap),-01(fp)
	movl	#00000001,r8
	pushl	r8
	pushal	-01(fp)
	cvtbl	+12(r11),-(sp)
	calls	#03,000038B4
	movl	r0,r9

l00003311:
	clrl	(r11)
	brw	00003419

l00003316:
	bbs	#00000002,+10(r11),000032F6

l0000331B:
	movl	+08(r11),r10
	beql	00003324

l00003321:
	brw	000033EC

l00003324:
	pushal	-48(fp)
	cvtbl	+12(r11),-(sp)
	calls	#02,0000353C
	tstl	r0
	blss	0000333B

l00003336:
	tstl	-18(fp)
	bgtr	00003343

l0000333B:
	cvtwl	#0400,-08(fp)
	brb	00003348

l00003343:
	movl	-18(fp),-08(fp)

l00003348:
	cmpl	r11,#00003FE8
	bneq	000033C3

l00003351:
	cvtbl	000043FA,-(sp)
	calls	#01,00003544
	tstl	r0
	beql	00003368

l00003363:
	bbss	#00000007,+10(r11),00003368

l00003368:
	moval	00007980,+08(r11)
	moval	00007980,+04(r11)
	movl	-08(fp),+0C(r11)

l0000337D:
	bbc	#00000007,+10(r11),00003316

l00003382:
	movl	+08(r11),r10
	cvtlb	+04(ap),@+04(r11)
	incl	+04(r11)
	addl3	+0C(r11),r10,r0
	cmpl	+04(r11),r0
	bgeq	000033A2

l00003399:
	cmpl	+04(ap),#0000000A
	beql	000033A2

l0000339F:
	brw	000032EF

l000033A2:
	subl3	r10,+04(r11),r0
	movl	r0,r8
	pushl	r0
	pushl	r10
	cvtbl	+12(r11),-(sp)
	calls	#03,000038B4
	movl	r0,r9
	movl	r10,+04(r11)
	brw	00003311

l000033C3:
	pushl	-08(fp)
	calls	#01,000035D4
	movl	r0,r10
	movl	r10,+08(r11)
	bneq	000033DC

l000033D6:
	bisw2	#0004,+10(r11)
	brb	0000337D

l000033DC:
	bisw2	#0008,+10(r11)
	movl	-08(fp),+0C(r11)
	clrl	r9
	movl	r9,r8
	brb	0000340C

l000033EC:
	subl3	r10,+04(r11),r9
	movl	r9,r8
	bleq	0000340C

l000033F6:
	movl	r10,+04(r11)
	pushl	r9
	pushl	r10
	cvtbl	+12(r11),-(sp)
	calls	#03,000038B4
	movl	r0,r9

l0000340C:
	subl3	#00000001,+0C(r11),(r11)
	cvtlb	+04(ap),(r10)+
	movl	r10,+04(r11)

l00003419:
	cmpl	r8,r9
	beql	00003425

l0000341E:
	bisw2	#0020,+10(r11)
	brw	000032EB

l00003425:
	movl	+04(ap),r0
	ret
0000342A                               00 0E                       ..    

;; fn0000342C: 0000342C
;;   Called from:
;;     00001EE1 (in fn00001E3A)
;;     00001EF6 (in fn00001E3A)
;;     00002970 (in fn0000296A)
;;     000034BE (in fn000034A8)
fn0000342C proc
	movl	+04(ap),r11
	cvtwl	+10(r11),r0
	bicl2	#FFFFFFF9,r0
	cmpl	r0,#00000002
	bneq	00003480

l00003440:
	movl	+08(r11),r10
	beql	00003480

l00003446:
	subl3	r10,+04(r11),r9
	bleq	00003480

l0000344D:
	movl	r10,+04(r11)
	bitw	#0084,+10(r11)
	beql	0000345D

l00003459:
	clrl	r0
	brb	00003461

l0000345D:
	movl	+0C(r11),r0

l00003461:
	movl	r0,(r11)
	pushl	r9
	pushl	r10
	cvtbl	+12(r11),-(sp)
	calls	#03,000038B4
	cmpl	r0,r9
	beql	00003480

l00003478:
	bisw2	#0020,+10(r11)
	mnegl	#00000001,r0
	ret

l00003480:
	clrl	r0
	ret
00003483          00 00 08                                  ...          

;; fn00003486: 00003486
;;   Called from:
;;     00003502 (in fn00003502)
fn00003486 proc
	moval	000043D4,r11
	brb	0000349B

l0000348F:
	pushl	r11
	calls	#01,000034A6
	addl2	#00000014,r11

l0000349B:
	cmpl	r11,00004564
	blss	0000348F

l000034A4:
	ret
000034A5                00 00 0C                              ...        

;; fn000034A8: 000034A8
;;   Called from:
;;     000019FC (in fn000019E6)
;;     00001C1C (in fn00001C06)
;;     00003491 (in fn00003486)
fn000034A8 proc
	movl	+04(ap),r11
	mnegl	#00000001,r10
	bitw	#0103,+10(r11)
	beql	000034E9

l000034B7:
	bbs	#00000006,+10(r11),000034E9

l000034BC:
	pushl	r11
	calls	#01,0000342A
	movl	r0,r10
	cvtbl	+12(r11),-(sp)
	calls	#01,00003594
	tstl	r0
	bgeq	000034DA

l000034D7:
	mnegl	#00000001,r10

l000034DA:
	bbc	#00000003,+10(r11),000034E9

l000034DF:
	pushl	+08(r11)
	calls	#01,000036DE

l000034E9:
	clrl	(r11)
	clrl	+08(r11)
	clrl	+04(r11)
	clrl	+0C(r11)
	clrw	+10(r11)
	clrb	+12(r11)
	movl	r10,r0
	ret
000034FE                                           00 00               ..
00003500 00 00                                           ..              

;; fn00003502: 00003502
;;   Called from:
;;     00000439 (in fn00000404)
;;     00000481 (in fn0000044E)
;;     00000535 (in fn0000044E)
;;     000007BD (in fn00000564)
fn00003502 proc
	calls	#00,00003484
	pushl	+04(ap)
	calls	#01,000038DC
	ret
00003514             00 0C                                   ..          

;; fn00003516: 00003516
;;   Called from:
;;     00000503 (in fn0000044E)
;;     00001973 (in fn00001956)
;;     0000198F (in fn00001956)
;;     00001B7C (in fn00001B62)
;;     00001B99 (in fn00001B62)
;;     00001BAF (in fn00001B62)
fn00003516 proc
	movl	+04(ap),r11
	movl	+08(ap),r10

l0000351E:
	cmpb	(r11),(r10)+
	bneq	0000352A

l00003523:
	tstb	(r11)+
	bneq	0000351E

l00003527:
	clrl	r0
	ret

l0000352A:
	cvtbl	(r11),r0
	cvtbl	-(r10),r1
	subl2	r1,r0
	ret

l00003534:
	jmp	000038D0
0000353A                               00 00 00 00                 ....  

;; fn0000353E: 0000353E
;;   Called from:
;;     00001E64 (in fn00001E3A)
;;     0000332B (in fn000032CE)
fn0000353E proc
	chmk	#003E
	blssu	00003534

l00003542:
	ret
00003543          00 00 00                                  ...          

;; fn00003546: 00003546
;;   Called from:
;;     00003358 (in fn000032CE)
fn00003546 proc
	subl2	#00000008,sp
	pushal	-06(fp)
	pushl	+04(ap)
	calls	#02,00003564
	tstl	r0
	bgeq	0000355D

l0000355A:
	clrl	r0
	ret

l0000355D:
	movl	#00000001,r0
	ret
00003561    00 00 00 00 00                                .....          

;; fn00003566: 00003566
;;   Called from:
;;     0000354F (in fn00003546)
fn00003566 proc
	pushl	+08(ap)
	pushl	#40067408
	pushl	+04(ap)
	calls	#03,00003584
	ret
0000357A                               00 00                       ..    

;; fn0000357C: 0000357C
;;   Called from:
;;     00003588 (in fn00003586)
fn0000357C proc
	jmp	000038D0
00003582       00 00 00 00                                 ....          

;; fn00003586: 00003586
;;   Called from:
;;     00000496 (in fn0000044E)
;;     000004AB (in fn0000044E)
;;     000004C0 (in fn0000044E)
;;     00000B24 (in fn00000AF8)
;;     00003572 (in fn00003566)
fn00003586 proc
	chmk	#0036
	blssu	fn0000357C

l0000358A:
	ret
0000358B                                  00                        .    

l0000358C:
	jmp	000038D0
00003592       00 00 00 00                                 ....          

;; fn00003596: 00003596
;;   Called from:
;;     000014F5 (in fn0000149A)
;;     00001FAA (in fn00001F52)
;;     00001FFE (in fn00001F52)
;;     000034CC (in fn000034A8)
fn00003596 proc
	chmk	#0006
	blssu	0000358C

l0000359A:
	ret
0000359B                                  00                        .    

l0000359C:
	jmp	000038D0
000035A2       00 00 00 00                                 ....          

;; fn000035A6: 000035A6
;;   Called from:
;;     00001FCC (in fn00001F52)
;;     0000200A (in fn00001F52)
;;     00002037 (in fn00001F52)
fn000035A6 proc
	chmk	#0005
	blssu	0000359C

l000035AA:
	ret
000035AB                                  00                        .    

l000035AC:
	jmp	000038D0
000035B2       00 00 00 00                                 ....          

;; fn000035B6: 000035B6
;;   Called from:
;;     00000BCF (in fn00000AF8)
;;     00000C26 (in fn00000AF8)
;;     00001F14 (in fn00001E3A)
fn000035B6 proc
	chmk	#0003
	blssu	000035AC

l000035BA:
	ret
000035BB                                  00 00 0E                  ...  

;; fn000035BE: 000035BE
;;   Called from:
;;     00000592 (in fn00000564)
;;     00000665 (in fn00000564)
fn000035BE proc
	movl	+04(ap),r11
	movl	+08(ap),r10
	movl	r11,r9

l000035C9:
	movb	(r10)+,(r11)+
	bneq	000035C9

l000035CE:
	movl	r9,r0
	ret
000035D2       00 00 00 0F                                 ....          

;; fn000035D6: 000035D6
;;   Called from:
;;     00001E97 (in fn00001E3A)
;;     000033C6 (in fn000032CE)
fn000035D6 proc
	movl	+04(ap),r11
	clrl	r9
	addl2	#00000004,r11
	addl3	#00000003,r11,r0
	bicl3	#00000003,r0,r11
	subl3	#00000001,r11,r0
	extzv	#00000002,#1E,r0,r8
	brb	000035F4
	incl	r9
	extzv	#00000001,#1F,r8,r8
	bneq	000035F2
	tstl	0000A2F0[r9]
	bneq	0000360D
	pushl	r9
	calls	#01,00003638
	movl	0000A2F0[r9],r10
	bneq	0000361A
	clrl	r0
	ret
	movl	0000A2F0[r9],r0
	movl	(r0),0000A2F0[r9]
	movb	#FF,(r10)
	cvtlb	r9,+01(r10)
	addl3	#00000004,r10,r0
	ret
	halt
	addb2	#0F,@+04AC(r0)
	Reserved
	tstl	0000A2F0[r11]
	beql	00003648
	ret
	pushl	#00000000
	calls	#01,0000387C
	movl	r0,r10
	bitl	#000003FF,r10
	beql	00003671
	extzv	#00000000,#0A,r10,r0
	subl3	r0,#00000400,-(sp)
	calls	#01,0000387C
	cmpl	r11,#00000008
	bgtr	0000367B
	movl	#0000000B,r0
	brb	0000367F
	addl3	#00000003,r11,r0
	movl	r0,r9
	addl3	#00000003,r11,r0
	subl3	r0,r9,r0
	ashl	r0,#00000001,r8
	cmpl	r9,r11
	bgeq	00003696
	movl	r11,r9
	ashl	r9,#00000001,-(sp)
	calls	#01,0000387C
	movl	r0,r10
	cmpl	r10,#FFFFFFFF
	bneq	000036AE
	ret
	bitl	#00000007,r10
	beql	000036BD
	addl3	#00000008,r10,r0
	bicl3	#00000007,r0,r10
	decl	r8
	movl	r10,0000A2F0[r11]
	addl3	#00000003,r11,r0
	ashl	r0,#00000001,r0
	movl	r0,r7
	brb	000036D9
	addl3	r7,r10,(r10)
	addl2	r7,r10
	sobgtr	r8,000036D2
	ret
	halt
	halt
	prober	@+04AC(r5),#0012,#01

;; fn000036E0: 000036E0
;;   Called from:
;;     00001717 (in fn00001706)
;;     000034E2 (in fn000034A8)
fn000036E0 proc
	tstl	+04(ap)
	bneq	000036E6

l000036E5:
	ret

l000036E6:
	subl3	#00000004,+04(ap),r10
	movzbl	(r10),r0
	cmpl	r0,#000000FF
	beql	000036F8

l000036F7:
	ret

l000036F8:
	movzbl	+01(r10),r11
	movl	0000A2F0[r11],(r10)
	movl	r10,0000A2F0[r11]
	ret
0000370D                                        00 00 0C              ...
00003710 C2 0C 5E D4 AD F4 D5 AC 04 12 0B DD AC 08 FB 01 ..^.............
00003720 EF AF FE FF FF 04 C3 04 AC 04 AD FC 9A BD FC 50 ...............P
00003730 D1 50 8F FF 00 00 00 12 0D D6 AD F4 D0 AD FC 50 .P.............P
00003740 9A A0 01 5A 11 28 DD 01 DD AD FC FB 02 EF 92 00 ...Z.(..........
00003750 00 00 D0 50 5A 18 17 DD EF 2F 0E 00 00 DD AD FC ...PZ..../......
00003760 FB 02 EF 7D 00 00 00 D0 50 5A 18 02 D4 5A C1 03 ...}....PZ...Z..
00003770 5A 50 78 50 01 50 C3 04 50 5B D5 AD F4 13 19 D1 ZPxP.P..P[......
00003780 AC 08 5B 1A 13 EF 01 1F 5B 50 C2 04 50 D1 AC 08 ..[.....[P..P...
00003790 50 1B 05 D0 AC 04 50 04 DD AC 08 FB 01 EF 32 FE P.....P.......2.
000037A0 FF FF D0 50 AD F8 12 03 D4 50 04 D1 AC 04 AD F8 ...P.....P......
000037B0 13 1E D1 AC 08 5B 1E 06 D0 AC 08 50 11 03 D0 5B .....[.....P...[
000037C0 50 DD 50 DD AD F8 DD AC 04 FB 03 EF 48 00 00 00 P.P.........H...
000037D0 D5 AD F4 13 0A DD AC 04 FB 01 EF FF FE FF FF D0 ................
000037E0 AD F8 50 04 00 0E D4 5A D4 59 D0 4A EF FF 6A 00 ..P....Z.Y.J..j.
000037F0 00 5B 11 15 D1 59 AC 08 13 13 D1 5B AC 04 12 04 .[...Y.....[....
00003800 D0 5A 50 04 D6 59 D0 6B 5B D5 5B 12 E7 D6 5A D1 .ZP..Y.k[.[...Z.
00003810 5A 1E 19 D4 CE 01 50 04 40 00                   Z.....P.@.      

;; fn0000381A: 0000381A
;;   Called from:
;;     00000613 (in fn00000564)
fn0000381A proc
	movl	+04(ap),r1
	movl	+08(ap),r3
	movl	+0C(ap),r6
	cmpl	r1,r3
	bgtr	00003835

l0000382B:
	blss	00003844

l0000382D:
	ret

l0000382E:
	subl2	r0,r6
	movc3	r0,(r1),(r3)

l00003835:
	movzwl	#FFFF,r0
	cmpl	r6,r0
	bgtr	0000382E

l0000383F:
	movc3	r6,(r1),(r3)
	ret

l00003844:
	addl2	r6,r1
	addl2	r6,r3
	movzwl	#FFFF,r0
	brb	00003869

l00003851:
	subl2	r0,r6
	subl2	r0,r1
	subl2	r0,r3
	movc3	r0,(r1),(r3)
	movzwl	#FFFF,r0
	subl2	r0,r1
	subl2	r0,r3

l00003869:
	cmpl	r6,r0
	bgtr	00003851

l0000386E:
	subl2	r6,r1
	subl2	r6,r3
	movc3	r6,(r1),(r3)
	ret
	halt
	halt
	halt
	halt
	halt
	addl3	00004594,+04(ap),-(sp)
	pushl	#00000001
	movl	ap,r3
	movl	sp,ap
	chmk	#0011
	blssu	000038A3
	movl	00004594,r0
	addl2	+04(r3),00004594
	ret
	jmp	000038D0
	halt
	halt
	halt

l000038AC:
	jmp	000038D0
000038B2       00 00 00 00                                 ....          

;; fn000038B6: 000038B6
;;   Called from:
;;     00001458 (in fn00001438)
;;     000014BA (in fn0000149A)
;;     000017E7 (in fn0000176E)
;;     00003307 (in fn000032CE)
;;     000033B2 (in fn000032CE)
;;     00003402 (in fn000032CE)
;;     0000346C (in fn0000342C)
fn000038B6 proc
	chmk	#0004
	blssu	000038AC

l000038BA:
	ret
000038BB                                  00                        .    

l000038BC:
	jmp	000038D0
000038C2       00 00 00 00                                 ....          

;; fn000038C6: 000038C6
;;   Called from:
;;     00002271 (in fn000021E6)
fn000038C6 proc
	chmk	#0079
	blssu	000038BC

l000038CC:
	ret
000038CD                                        00 00 00              ...

l000038D0:
	movl	r0,0000597C
	mnegl	#00000001,r0
	ret
000038DB                                  00 00 00                  ...  

;; fn000038DE: 000038DE
;;   Called from:
;;     0000350C (in fn00003502)
fn000038DE proc
	chmk	#0001
	halt
000038E1    00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ...............
000038F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
