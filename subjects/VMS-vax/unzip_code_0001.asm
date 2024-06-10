;;; Segment code (00001000)

l00011005:
	tstl	+40(r2)
	bneq	00011037

l0001100A:
	tstl	+48(r2)
	bneq	00011037

l0001100F:
	pushl	#00000000
	pushl	+0000C613(r2)
	pushal	+2B(r7)
	pushab	+05EB(r2)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)

l00011037:
	tstl	(r2)
	beql	00011060

l0001103B:
	pushl	+0000C61B(r2)
	calls	#01,000117A8
	movl	r0,r9
	beql	0001104E

l0001104B:
	brw	000110EF

l0001104E:
	calls	#00,00012A28
	movl	r0,r9
	cmpl	r9,#00000001
	bleq	00011060

l0001105D:
	brw	000110EF

l00011060:
	tstl	(r2)
	beql	00011067

l00011064:
	brw	00011110

l00011067:
	movl	+0000C61B(r2),r0
	cmpl	r0,#000101D0
	bgeq	0001107C

l00011077:
	movl	r0,r3
	brb	00011083

l0001107C:
	movl	#000101D0,r3

l00011083:
	pushl	r3
	calls	#01,000117A8
	movl	r0,r9
	bneq	000110EF

l0001108F:
	clrl	r4
	tstw	+0000C692(r2)
	beql	000110E7

l00011099:
	tstl	+5C(r2)
	bgtr	000110AA

l0001109E:
	bneq	000110E7

l000110A0:
	tstl	+48(r2)
	bneq	000110E7

l000110A5:
	tstl	+40(r2)
	bneq	000110E7

l000110AA:
	pushl	#00000001
	movzwl	+0000C692(r2),-(sp)
	calls	#02,0000DC74
	tstl	r0
	beql	000110E7

l000110BE:
	movzwl	#0401,-(sp)
	pushal	+0748(r6)
	pushab	+05EB(r2)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	#00000001,r4

l000110E7:
	movl	r4,r9
	cmpl	r9,#00000001
	bleq	00011110

l000110EF:
	pushl	+0000C617(r2)
	calls	#01,@0001925C
	tstl	+04(ap)
	beql	00011105

l00011101:
	movl	r9,r0
	ret

l00011105:
	movl	#00000001,+0000C6E9(r2)
	movl	#00000009,r0
	ret

l00011110:
	tstl	+5C(r2)
	bleq	0001112A

l00011115:
	tstl	(r2)
	bneq	0001112A

l00011119:
	pushl	+0000C617(r2)
	calls	#01,@0001925C
	movl	r9,r0
	ret

l0001112A:
	clrb	r3
	tstl	(r2)
	bneq	0001113B

l00011130:
	movzwl	+0000C682(r2),r0
	beql	0001113B

l00011139:
	incb	r3

l0001113B:
	cvtbl	r3,r4
	tstl	(r2)
	bneq	00011145

l00011142:
	brw	000111D9

l00011145:
	cmpw	+0000C682(r2),+0000C684(r2)
	bneq	00011155

l00011152:
	brw	000111D9

l00011155:
	blequ	0001119C

l00011157:
	movzwl	#0401,-(sp)
	movzwl	+0000C684(r2),-(sp)
	movzwl	+0000C682(r2),-(sp)
	pushl	+0000C613(r2)
	pushal	+0203(r6)
	pushab	+05EB(r2)
	calls	#05,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	#0000000B,r9
	movl	#00000001,r8
	brb	000111D9
00011199                            D5 50 01                      .P.    

l0001119C:
	movzwl	#0401,-(sp)
	movzwl	+0000C684(r2),-(sp)
	movzwl	+0000C682(r2),-(sp)
	pushl	+0000C613(r2)
	pushal	+0296(r6)
	pushab	+05EB(r2)
	calls	#05,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	#00000001,r9

l000111D9:
	tstl	r8
	beql	000111E0

l000111DD:
	brw	00011742

l000111E0:
	tstl	r4
	beql	00011213

l000111E4:
	movzwl	#0401,-(sp)
	pushl	+0000C613(r2)
	pushal	+034C(r6)
	pushab	+05EB(r2)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	#00000001,r9

l00011213:
	subl3	+0080(r2),+7C(r2),+0000C623(r2)
	movl	+0000C623(r2),r4
	bgeq	0001125C

l00011227:
	movzwl	#0401,-(sp)
	mnegl	r4,-(sp)
	pushl	+0000C613(r2)
	pushal	+04AF(r6)
	pushab	+05EB(r2)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	#00000002,r9
	brw	000112FA

l0001125C:
	tstl	r4
	bgtr	00011263

l00011260:
	brw	000112FA

l00011263:
	tstl	+0000C68E(r2)
	bneq	000112B8

l0001126B:
	tstl	+0000C68A(r2)
	beql	000112B8

l00011273:
	movzwl	#0401,-(sp)
	pushl	+0000C613(r2)
	pushal	+04FB(r6)
	pushab	+05EB(r2)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	+0000C623(r2),+0000C68E(r2)
	clrl	+0000C623(r2)
	movl	#00000002,r9
	brb	000112FA
000112B5                D5 50 01                              .P.        

l000112B8:
	movzwl	#0401,-(sp)
	cmpl	r4,#00000001
	bneq	000112C8

l000112C2:
	moval	+3B(r7),r3
	brb	000112CC

l000112C8:
	moval	+39(r7),r3

l000112CC:
	pushl	r3
	pushl	r4
	pushl	+0000C613(r2)
	pushal	+044E(r6)
	pushab	+05EB(r2)
	calls	#05,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	#00000001,r9

l000112FA:
	tstl	+0080(r2)
	beql	00011303

l00011300:
	brw	00011390

l00011303:
	tstl	+0000C68A(r2)
	beql	0001130E

l0001130B:
	brw	00011390

l0001130E:
	tstl	(r2)
	beql	00011348

l00011312:
	pushl	#00000000
	cmpl	+24(r2),#00000009
	bleq	00011320

l0001131A:
	moval	+4F(r7),r3
	brb	00011324

l00011320:
	moval	+4E(r7),r3

l00011324:
	pushl	r3
	pushal	+3C(r7)
	pushab	+05EB(r2)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	brb	00011374

l00011348:
	movzwl	#0401,-(sp)
	pushl	+0000C613(r2)
	pushal	+0548(r6)
	pushab	+05EB(r2)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)

l00011374:
	pushl	+0000C617(r2)
	calls	#01,@0001925C
	cmpl	r9,#00000001
	bleq	0001138C

l00011386:
	movl	r9,r0
	ret
0001138A                               D5 50                       .P    

l0001138C:
	movl	#00000001,r0
	ret

l00011390:
	addl3	+0000C68E(r2),+0000C623(r2),r3
	emul	#00000000,#00000000,r3,r0
	ediv	#00002000,r0,r1,r0
	movl	r0,r4
	subl3	r4,r3,r5
	tstl	r3
	bgeq	000113E2
	pushl	#00000001
	pushal	(r10)
	pushl	+0000C613(r2)
	pushal	(r11)
	pushab	+05EB(r2)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	#00000003,r0
	ret
	cmpl	r5,+0000C61F(r2)
	beql	00011440
	pushl	#00000000
	pushl	r5
	pushl	+0000C617(r2)
	calls	#03,@0001926C
	movl	r0,+0000C61F(r2)
	movzwl	#2000,-(sp)
	pushl	+0000C5F3(r2)
	pushl	+0000C617(r2)
	calls	#03,@00019270
	movl	r0,+0000C5FB(r2)
	bgtr	00011428
	movl	#00000033,r0
	ret
	addl3	+0000C5F3(r2),r4,+0000C5F7(r2)
	subl2	r4,+0000C5FB(r2)
	brb	00011462
	tstl	r0
	nop
	subl3	+0000C5F3(r2),+0000C5F7(r2),r0
	subl2	r4,r0
	addl2	r0,+0000C5FB(r2)
	addl3	+0000C5F3(r2),r4,+0000C5F7(r2)
	pushl	#00000004
	pushab	+0098(r2)
	calls	#02,0000D5BC
	tstl	r0
	beql	0001148D
	pushl	#00000004
	pushab	+0000C634(r2)
	pushab	+0098(r2)
	calls	#03,@000192D8
	tstl	r0
	bneq	0001148D
	brw	00011603
	movl	+0000C623(r2),r8
	clrl	+0000C623(r2)
	addl3	+0000C68E(r2),+0000C623(r2),r3
	emul	#00000000,#00000000,r3,r0
	ediv	#00002000,r0,r1,r0
	movl	r0,r4
	subl3	r4,r3,r5
	tstl	r3
	bgeq	000114EC
	pushl	#00000001
	pushal	(r10)
	pushl	+0000C613(r2)
	pushal	(r11)
	pushab	+05EB(r2)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	#00000003,r0
	ret
	cmpl	r5,+0000C61F(r2)
	beql	00011548
	pushl	#00000000
	pushl	r5
	pushl	+0000C617(r2)
	calls	#03,@0001926C
	movl	r0,+0000C61F(r2)
	movzwl	#2000,-(sp)
	pushl	+0000C5F3(r2)
	pushl	+0000C617(r2)
	calls	#03,@00019270
	movl	r0,+0000C5FB(r2)
	bgtr	00011532
	movl	#00000033,r0
	ret
	addl3	+0000C5F3(r2),r4,+0000C5F7(r2)
	subl2	r4,+0000C5FB(r2)
	brb	0001156A
	nop
	subl3	+0000C5F3(r2),+0000C5F7(r2),r0
	subl2	r4,r0
	addl2	r0,+0000C5FB(r2)
	addl3	+0000C5F3(r2),r4,+0000C5F7(r2)
	pushl	#00000004
	pushab	+0098(r2)
	calls	#02,0000D5BC
	tstl	r0
	beql	00011592
	pushl	#00000004
	pushab	+0000C634(r2)
	pushab	+0098(r2)
	calls	#03,@000192D8
	tstl	r0
	beql	000115D1
	movzwl	#0401,-(sp)
	pushal	(r10)
	pushl	+0000C613(r2)
	pushal	+0569(r6)
	pushab	+05EB(r2)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	pushl	+0000C617(r2)
	calls	#01,@0001925C
	movl	#00000003,r0
	ret
	movzwl	#0401,-(sp)
	mnegl	r8,-(sp)
	pushl	+0000C613(r2)
	pushal	+05B2(r6)
	pushab	+05EB(r2)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	#00000002,r9
	addl3	+0000C68E(r2),+0000C623(r2),r3
	emul	#00000000,#00000000,r3,r0
	ediv	#00002000,r0,r1,r0
	movl	r0,r4
	subl3	r4,r3,r5
	tstl	r3
	bgeq	00011655
	pushl	#00000001
	pushal	(r10)
	pushl	+0000C613(r2)
	pushal	(r11)
	pushab	+05EB(r2)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	movl	#00000003,r0
	ret
	cmpl	r5,+0000C61F(r2)
	beql	000116B0
	pushl	#00000000
	pushl	r5
	pushl	+0000C617(r2)
	calls	#03,@0001926C
	movl	r0,+0000C61F(r2)
	movzwl	#2000,-(sp)
	pushl	+0000C5F3(r2)
	pushl	+0000C617(r2)
	calls	#03,@00019270
	movl	r0,+0000C5FB(r2)
	bgtr	0001169B
	movl	#00000033,r0
	ret
	addl3	+0000C5F3(r2),r4,+0000C5F7(r2)
	subl2	r4,+0000C5FB(r2)
	brb	000116D2
	subl3	+0000C5F3(r2),+0000C5F7(r2),r0
	subl2	r4,r0
	addl2	r0,+0000C5FB(r2)
	addl3	+0000C5F3(r2),r4,+0000C5F7(r2)
	tstl	(r2)
	beql	000116FC
	calls	#00,00012D84
	movl	r0,r3
	cmpl	+24(r2),#00000009
	bleq	0001173A
	pushl	#00000000
	pushl	#00000001
	pushal	+53(r7)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	brb	0001173A
	nop
	tstl	+48(r2)
	beql	00011714
	pushal	-0C(fp)
	pushal	-08(fp)
	calls	#02,000173C8
	movl	r0,r3
	brb	0001173A
	nop
	tstl	+50(r2)
	beql	00011730
	tstl	+44(r2)
	bneq	00011730
	tstl	+0C(r2)
	bneq	00011730
	calls	#00,00016AE0
	movl	r0,r3
	brb	0001173A
	nop
	calls	#00,0000E3D8
	movl	r0,r3
	cmpl	r3,r9
	bleq	00011742
	movl	r3,r9

l00011742:
	pushl	+0000C617(r2)
	calls	#01,@0001925C
	tstl	+48(r2)
	beql	000117A3

l00011754:
	tstl	(r2)
	bneq	000117A3

l00011758:
	tstl	-0C(fp)
	beql	000117A3

l0001175D:
	pushl	-08(fp)
	pushl	+0000C613(r2)
	calls	#02,0000BDC0
	tstl	r0
	beql	000117A3

l00011771:
	movzwl	#0201,-(sp)
	pushl	+0000C613(r2)
	pushal	+55(r7)
	pushab	+05EB(r2)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r0
	calls	#04,(r0)
	tstl	r9
	bgtr	000117A3

l000117A0:
	movl	#00000001,r9

l000117A3:
	movl	r9,r0
	ret
000117A7                      01 FC 03                          ...      

;; fn000117AA: 000117AA
;;   Called from:
;;     00011041 (in fn00010E6A)
;;     00011085 (in fn00010E6A)
fn000117AA proc
	subl2	#0000001C,sp
	movab	FFFE67AC,r8
	movab	FFFFB3D0,r2
	movab	FFFF8DEC,r9
	clrl	r4
	movl	+0000C61B(r2),r5
	cmpl	r5,#00002000
	bleq	000117D7

l000117D4:
	brw	00011898

l000117D7:
	clrq	-(sp)
	pushl	+0000C617(r2)
	calls	#03,@0001926C
	pushl	+0000C61B(r2)
	pushl	+0000C5F3(r2)
	pushl	+0000C617(r2)
	calls	#03,@00019270
	movl	r0,+0000C5FB(r2)
	cmpl	+0000C5FB(r2),+0000C61B(r2)
	beql	00011816

l00011813:
	brw	00011A92

l00011816:
	addl3	+0000C5F3(r2),+0000C61B(r2),r3
	subl3	#00000016,r3,+0000C5F7(r2)
	cmpl	+0000C5F7(r2),+0000C5F3(r2)
	bgequ	0001183A

l00011837:
	brw	00011A92

l0001183A:
	tstl	r0

l0001183C:
	movl	+0000C5F7(r2),r3
	movzbl	(r3),r0
	cmpl	r0,#00000050
	bneq	00011880

l0001184F:
	pushl	#00000004
	pushab	+0000C639(r2)
	pushl	r3
	calls	#03,@000192D8
	tstl	r0
	bneq	00011880

l00011864:
	subl3	+0000C5F3(r2),+0000C5F7(r2),r0
	subl2	r0,+0000C5FB(r2)
	movl	#00000001,r4
	brw	00011A92
0001187D                                        D5 50 01              .P.

l00011880:
	decl	+0000C5F7(r2)
	cmpl	+0000C5F7(r2),+0000C5F3(r2)
	bgequ	0001183C

l00011893:
	brw	00011A92
00011896                   D5 50                               .P        

l00011898:
	emul	#00000000,#00000000,r5,r0
	ediv	#00002000,r0,r1,r0
	movl	r0,r7
	cmpl	r7,#00000012
	bgtr	000118B1
	brw	0001197C
	pushl	#00000000
	subl3	r7,r5,-(sp)
	pushl	+0000C617(r2)
	calls	#03,@0001926C
	movl	r0,+0000C61F(r2)
	pushl	r7
	pushl	+0000C5F3(r2)
	pushl	+0000C617(r2)
	calls	#03,@00019270
	movl	r0,+0000C5FB(r2)
	cmpl	+0000C5FB(r2),r7
	beql	000118F3
	brw	00011A92
	addl3	+0000C5F3(r2),r7,r3
	subl3	#00000016,r3,+0000C5F7(r2)
	cmpl	+0000C5F7(r2),+0000C5F3(r2)
	blssu	00011963
	movl	+0000C5F7(r2),r3
	movzbl	(r3),r0
	cmpl	r0,#00000050
	bneq	00011950
	pushl	#00000004
	pushab	+0000C639(r2)
	pushl	r3
	calls	#03,@000192D8
	tstl	r0
	bneq	00011950
	subl3	+0000C5F3(r2),+0000C5F7(r2),r0
	subl2	r0,+0000C5FB(r2)
	movl	#00000001,r4
	brb	00011963
	decl	+0000C5F7(r2)
	cmpl	+0000C5F7(r2),+0000C5F3(r2)
	bgequ	00011910
	pushl	#00000003
	pushl	+0000C5F3(r2)
	pushl	+0000C62B(r2)
	calls	#03,@000192C0
	brb	00011984
	tstl	r0
	subl3	r7,r5,+0000C61F(r2)
	subl3	r7,+04(ap),r3
	addl2	#00001FFF,r3
	divl3	#00002000,r3,r6
	movl	#00000001,r5
	tstl	r4
	beql	000119A2
	brw	00011A92
	cmpl	r5,r6
	bleq	000119AA
	brw	00011A92
	movab	@000192D8,r3
	tstl	r0
	nop
	addl2	#FFFFE000,+0000C61F(r2)
	pushl	#00000000
	pushl	+0000C61F(r2)
	pushl	+0000C617(r2)
	calls	#03,@0001926C
	movzwl	#2000,-(sp)
	pushl	+0000C5F3(r2)
	pushl	+0000C617(r2)
	calls	#03,@00019270
	movl	r0,+0000C5FB(r2)
	cmpl	+0000C5FB(r2),#00002000
	beql	00011A03
	brw	00011A92
	addl3	#00001FFF,+0000C5F3(r2),+0000C5F7(r2)
	cmpl	+0000C5F7(r2),+0000C5F3(r2)
	blssu	00011A6F
	movl	+0000C5F7(r2),r1
	movzbl	(r1),r0
	cmpl	r0,#00000050
	bneq	00011A5C
	pushl	#00000004
	pushab	+0000C639(r2)
	pushl	r1
	calls	#03,(r3)
	tstl	r0
	bneq	00011A5C
	subl3	+0000C5F3(r2),+0000C5F7(r2),r0
	subl2	r0,+0000C5FB(r2)
	movl	#00000001,r4
	brb	00011A6F
	decl	+0000C5F7(r2)
	cmpl	+0000C5F7(r2),+0000C5F3(r2)
	bgequ	00011A20
	pushl	#00000003
	pushl	+0000C5F3(r2)
	pushl	+0000C62B(r2)
	calls	#03,@000192C0
	incl	r5
	tstl	r4
	bneq	00011A92
	cmpl	r5,r6
	bgtr	00011A92
	brw	000119B4

l00011A92:
	tstl	r4
	bneq	00011AF4

l00011A96:
	tstl	+40(r2)
	bneq	00011A9F

l00011A9B:
	tstl	(r2)
	beql	00011ACA

l00011A9F:
	movzwl	#0401,-(sp)
	pushl	+0000C613(r2)
	pushal	+77(r8)
	pushab	+05EB(r2)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r3
	calls	#04,(r3)

l00011ACA:
	movzwl	#0401,-(sp)
	pushal	+0648(r9)
	pushab	+05EB(r2)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),r3
	calls	#04,(r3)
	movl	#00000002,r0
	ret

l00011AF4:
	subl3	+0000C5F3(r2),+0000C5F7(r2),r3
	addl3	+0000C61F(r2),r3,+7C(r2)
	pushl	#00000016
	pushal	-1A(fp)
	calls	#02,0000D5BC
	tstl	r0
	bneq	00011B1D

l00011B19:
	movl	#00000033,r0
	ret

l00011B1D:
	pushab	-16(fp)
	movab	0000E258,r4
	calls	#01,(r4)
	movw	r0,+0000C682(r2)
	pushab	-14(fp)
	calls	#01,(r4)
	movw	r0,+0000C684(r2)
	pushab	-12(fp)
	calls	#01,(r4)
	movw	r0,+0000C686(r2)
	pushab	-10(fp)
	calls	#01,(r4)
	movw	r0,+0000C688(r2)
	pushab	-0E(fp)
	calls	#01,0000E274
	movl	r0,+0000C68A(r2)
	pushab	-0A(fp)
	calls	#01,0000E274
	movl	r0,+0000C68E(r2)
	pushab	-06(fp)
	calls	#01,(r4)
	movw	r0,+0000C692(r2)
	addl3	+0000C68E(r2),+0000C68A(r2),+0080(r2)
	clrl	r0
	ret
00011B98                         1C 00 C2 04 5E 9E EF 2D         ....^..-
00011BA0 98 FE FF 52 9E EF 42 72 FE FF 53 D4 54 B5 E2 92 ...R..Br..S.T...
00011BB0 C6 00 00 13 4E D5 A2 5C 14 0C 12 47 D5 A2 48 12 ....N..\...G..H.
00011BC0 42 D5 A2 40 12 3D DD 01 3C E2 92 C6 00 00 7E FB B..@.=..<.....~.
00011BD0 02 EF 9E C0 FF FF D5 50 13 29 3C 8F 01 04 7E DF .......P.)<...~.
00011BE0 C3 48 07 9F C2 EB 05 FB 02 FF E2 76 00 00 DD 50 .H.........v...P
00011BF0 9F C2 EB 05 9F 62 D0 E2 39 C8 00 00 5C FB 04 6C .....b..9...\..l
00011C00 D0 01 54 D0 54 50 04 01 04 00                   ..T.TP....      

;; fn00011C0A: 00011C0A
;;   Called from:
;;     0000E5BC (in fn0000E3DA)
;;     00012E96 (in fn00012D86)
;;     0001748E (in fn000173CA)
fn00011C0A proc
	subl2	#00000004,sp
	movab	FFFFB3D0,r2
	calls	#00,00011CD8
	tstl	r0
	beql	00011C20

l00011C1F:
	ret

l00011C20:
	movl	+05E7(r2),r0
	movzbl	+0000C659(r2),ap
	cmpl	ap,#00000012
	bgequ	00011C3C

l00011C31:
	movb	+0000C659(r2),r1
	brb	00011C3F
00011C3A                               D5 50                       .P    

l00011C3C:
	movb	#12,r1

l00011C3F:
	movzbl	r1,+0C(r0)
	movl	+05E7(r2),ap
	bicb2	#10,+14(ap)
	tstl	+28(r2)
	beql	00011C95

l00011C51:
	movl	+05E7(r2),ap
	movl	+0C(ap),r0
	casel	r0,#00000000,#00000011
00011C5E                                           2E 00               ..
00011C60 37 00 2E 00 37 00 2E 00 37 00 37 00 37 00 37 00 7...7...7.7.7.7.
00011C70 2E 00 2E 00 37 00 37 00 37 00 37 00 2E 00 37 00 ....7.7.7.7...7.

l00011C80:
	movtc
	halt
	brb	00011C95
	jmp	400108D0
	tstl	r0
	movl	+05E7(r2),ap
	bisb2	#10,+14(ap)

l00011C95:
	bbc	#00000003,+0000C67A(r2),00011CCC

l00011C9D:
	movl	+05E7(r2),ap
	movl	+0C(ap),ap
	beql	00011CB7

l00011CA8:
	cmpl	ap,#00000006
	beql	00011CB7

l00011CAD:
	cmpl	ap,#0000000B
	beql	00011CB7

l00011CB2:
	cmpl	ap,#00000005
	bneq	00011CCC

l00011CB7:
	movl	+05E7(r2),ap
	bisb2	#20,+14(ap)
	movl	+05E7(r2),ap
	bicb2	#10,+14(ap)
	brb	00011CD5
00011CCB                                  01                        .    

l00011CCC:
	movl	+05E7(r2),ap
	bicb2	#20,+14(ap)

l00011CD5:
	clrl	r0
	ret
00011CD8                         1C 00                           ..      

;; fn00011CDA: 00011CDA
;;   Called from:
;;     00011C14 (in fn00011C0A)
fn00011CDA proc
	subl2	#00000030,sp
	movab	FFFFB3D0,r2
	pushl	#0000002A
	pushal	-2E(fp)
	calls	#02,0000D5BC
	tstl	r0
	bneq	00011CF8

l00011CF4:
	movl	#00000033,r0
	ret

l00011CF8:
	movb	-2E(fp),+0000C658(r2)
	movb	-2D(fp),+0000C659(r2)
	movb	-2C(fp),+0000C65A(r2)
	movb	-2B(fp),+0000C65B(r2)
	pushab	-2A(fp)
	movab	0000E258,r3
	calls	#01,(r3)
	movw	r0,+0000C65C(r2)
	pushab	-28(fp)
	calls	#01,(r3)
	movw	r0,+0000C65E(r2)
	pushab	-26(fp)
	calls	#01,(r3)
	movw	r0,+0000C660(r2)
	pushab	-24(fp)
	calls	#01,(r3)
	movw	r0,+0000C662(r2)
	pushab	-22(fp)
	movab	0000E274,r4
	calls	#01,(r4)
	movl	r0,+0000C664(r2)
	pushab	-1E(fp)
	calls	#01,(r4)
	movl	r0,+0000C668(r2)
	pushab	-1A(fp)
	calls	#01,(r4)
	movl	r0,+0000C66C(r2)
	pushab	-16(fp)
	calls	#01,(r3)
	movw	r0,+0000C670(r2)
	pushab	-14(fp)
	calls	#01,(r3)
	movw	r0,+0000C672(r2)
	pushab	-12(fp)
	calls	#01,(r3)
	movw	r0,+0000C674(r2)
	pushab	-10(fp)
	calls	#01,(r3)
	movw	r0,+0000C676(r2)
	pushab	-0E(fp)
	calls	#01,(r3)
	movw	r0,+0000C678(r2)
	pushab	-0C(fp)
	calls	#01,(r4)
	movl	r0,+0000C67A(r2)
	pushab	-08(fp)
	calls	#01,+00(r4)
	movl	r0,+0000C67E(r2)
	clrl	r0
	ret
00011DE0 1C 00 C2 20 5E 9E EF E5 95 FE FF 52 DD 1A DF AD ... ^......R....
00011DF0 E2 FB 02 EF C4 B7 FF FF D5 50 12 04 D0 33 50 04 .........P...3P.
00011E00 90 AD E2 E2 3E C6 00 00 90 AD E3 E2 3F C6 00 00 ....>.......?...
00011E10 9F AD E4 9E EF 3F C4 FF FF 54 FB 01 64 B0 50 E2 .....?...T..d.P.
00011E20 40 C6 00 00 9F AD E6 FB 01 64 B0 50 E2 42 C6 00 @........d.P.B..
00011E30 00 9F AD E8 FB 01 64 B0 50 E2 44 C6 00 00 9F AD ......d.P.D.....
00011E40 EA FB 01 64 B0 50 E2 46 C6 00 00 9F AD EC 9E EF ...d.P.F........
00011E50 20 C4 FF FF 53 FB 01 63 D0 50 E2 48 C6 00 00 9F  ...S..c.P.H....
00011E60 AD F0 FB 01 63 D0 50 E2 4C C6 00 00 9F AD F4 FB ....c.P.L.......
00011E70 01 63 D0 50 E2 50 C6 00 00 9F AD F8 FB 01 64 B0 .c.P.P........d.
00011E80 50 E2 54 C6 00 00 9F AD FA FB 01 64 B0 50 E2 56 P.T........d.P.V
00011E90 C6 00 00 D0 E2 4C C6 00 00 C2 84 00 D0 E2 50 C6 .....L........P.
00011EA0 00 00 C2 88 00 3C E2 40 C6 00 00 5C E1 03 5C 20 .....<.@...\..\ 
00011EB0 D0 C2 E7 05 5C D0 AC 08 E2 48 C6 00 00 D0 C2 E7 ....\....H......
00011EC0 05 5C D0 AC 04 E2 4C C6 00 00 D0 AC 04 C2 84 00 .\....L.........
00011ED0 D4 50 04 00 00 00                               .P....          

;; fn00011ED6: 00011ED6
;;   Called from:
;;     0000D7C7 (in fn0000D69A)
fn00011ED6 proc
	subl2	#00000004,sp
	movab	FFFFB3D0,r0
	bicl3	#FFFF0000,+0000C811(r0),r1
	bisl2	#00000002,r1
	xorl3	#00000001,r1,ap
	mull2	r1,ap
	extzv	#00000008,#18,ap,ap
	bicl3	#FFFFFF00,ap,r0
	ret
	prober	#00,+5E04(r2),@(sp)+

;; fn00011F06: 00011F06
;;   Called from:
;;     0000D7D8 (in fn0000D69A)
fn00011F06 proc
	subl2	#00000004,sp
	movab	FFFFB3D0,r2
	xorl3	+0000C809(r2),+04(ap),r0
	bicl2	#FFFFFF00,r0
	movl	+0000C5EB(r2),r3
	extzv	#00000008,#18,+0000C809(r2),r1
	xorl3	(r3)[r0],r1,+0000C809(r2)
	bicl3	#FFFFFF00,+0000C809(r2),r1
	addl2	r1,+0000C80D(r2)
	mull3	#08088405,+0000C80D(r2),r1
	incl	r1
	movl	r1,+0000C80D(r2)
	movzbl	+0000C810(r2),r0
	xorl2	+0000C811(r2),r0
	bicl2	#FFFFFF00,r0
	movl	+0000C5EB(r2),r3
	extzv	#00000008,#18,+0000C811(r2),r1
	xorl3	(r3)[r0],r1,+0000C811(r2)
	movl	+04(ap),r0
	ret
	bvc	00011F96
	subl2	#00000008,sp
	movab	FFFFB3D0,r2
	movl	#12345678,+0000C809(r2)
	movl	#23456789,+0000C80D(r2)
	movl	#34567890,+0000C811(r2)
	tstb	@+04(ap)
	bneq	00011FC9
	brw	00012059
	tstl	r0
	nop
	cvtbl	@+04(ap),r4
	xorl3	+0000C809(r2),r4,r0
	bicl2	#FFFFFF00,r0
	movl	+0000C5EB(r2),r3
	extzv	#00000008,#18,+0000C809(r2),r1
	xorl3	(r3)[r0],r1,+0000C809(r2)
	bicl3	#FFFFFF00,+0000C809(r2),r1
	addl2	r1,+0000C80D(r2)
	mull3	#08088405,+0000C80D(r2),r1
	incl	r1
	movl	r1,+0000C80D(r2)
	movzbl	+0000C810(r2),r0
	xorl2	+0000C811(r2),r0
	bicl2	#FFFFFF00,r0
	movl	+0000C5EB(r2),r3
	extzv	#00000008,#18,+0000C811(r2),r1
	xorl3	(r3)[r0],r1,+0000C811(r2)
	incl	+04(ap)
	tstb	@+04(ap)
	beql	00012059
	brw	00011FCC
	ret
	tstl	r0
	bvc	0001205E
	subl2	#00000014,sp
	movab	FFFFB3D0,r2
	movl	+05E7(r2),ap
	bicb2	#01,+14(ap)
	calls	#00,0000D564
	clrl	-0008(fp)
	decl	+0000C5FB(r2)
	blss	00012098
	movl	+0000C5F7(r2),ap
	incl	+0000C5F7(r2)
	movzbl	(ap),r3
	brb	000120A2
	tstl	r0
	calls	#00,0000D698
	movl	r0,r3
	cvtlw	r3,r4
	movl	-08(fp),ap
	movb	r4,-14(fp)[ap]
	aobleq	#0000000B,-08(fp),0001207C
	calls	#00,0000D50C
	movl	+05E7(r2),ap
	bisb2	#01,+14(ap)
	tstl	+78(r2)
	beql	0001212B
	clrl	+78(r2)
	tstl	+3C(r2)
	beql	00012110
	tstl	+0000C815(r2)
	bneq	0001212B
	movzbl	#51,-(sp)
	calls	#01,@000192B4
	movl	r0,+0000C815(r2)
	bneq	000120F0
	movl	#00000005,r0
	ret
	movzbl	#50,-(sp)
	pushl	+0000C801(r2)
	pushl	+0000C815(r2)
	calls	#03,@000192C0
	movl	#00000001,+0000C805(r2)
	brb	0001212B
	tstl	+0000C815(r2)
	beql	0001212B
	pushl	+0000C815(r2)
	calls	#01,@000192B0
	clrl	+0000C815(r2)
	tstl	+0000C815(r2)
	beql	0001214E
	pushal	-14(fp)
	calls	#01,000121E8
	tstl	r0
	bneq	00012142
	clrl	r0
	ret
	tstl	+0000C805(r2)
	beql	00012166
	movl	#00000001,r0
	ret
	movzbl	#51,-(sp)
	calls	#01,@000192B4
	movl	r0,+0000C815(r2)
	bneq	00012166
	movl	#00000005,r0
	ret
	clrl	-00000008(fp)
	pushab	+0000C701(r2)
	pushl	+0000C613(r2)
	movzbl	#51,-(sp)
	pushl	+0000C815(r2)
	pushal	-08(fp)
	pushab	(r2)
	movl	+0000C845(r2),ap
	calls	#06,(ap)
	movl	r0,r3
	cmpl	r3,#00000005
	bneq	000121B0
	pushl	+0000C815(r2)
	calls	#01,@000192B0
	clrl	+0000C815(r2)
	movl	#00000005,r0
	ret
	tstl	r3
	beql	000121C0
	movl	+0000C815(r2),ap
	clrb	(ap)
	clrl	-08(fp)
	pushal	-14(fp)
	calls	#01,000121E8
	tstl	r0
	bneq	000121CE
	clrl	r0
	ret
	cmpl	r3,#FFFFFFFE
	bneq	000121DE
	movl	#00000001,+0000C805(r2)
	tstl	-08(fp)
	bgtr	0001216C
	cvtbl	#01,r0
	ret
	bvc	fn000121EA

;; fn000121EA: 000121EA
fn000121EA proc
	subl2	#00000004,sp
	movab	FFFFB3D0,r3
	pushl	+0000C815(r3)
	pushl	+04(ap)
	calls	#02,00012250
	movl	r0,r4
	beql	0001224B

l00012206:
	pushl	+0000C815(r3)
	calls	#01,@000192C4
	incl	r0
	pushl	r0
	calls	#01,@000192B4
	movl	r0,r2
	bneq	00012227

l00012223:
	mnegl	#00000001,r0
	ret

l00012227:
	pushl	+0000C815(r3)
	pushl	r2
	calls	#02,0000E2A4
	pushl	r0
	pushl	+04(ap)
	calls	#02,00012250
	movl	r0,r4
	pushl	r2
	calls	#01,@000192B0

l0001224B:
	movl	r4,r0
	ret
0001224F                                              01                .
00012250 FC 03                                           ..              

;; fn00012252: 00012252
;;   Called from:
;;     000121FD (in fn000121EA)
;;     0001223B (in fn000121EA)
fn00012252 proc
	subl2	#00000020,sp
	movab	FFFFB3D0,r2
	movl	+08(ap),r4
	movl	#12345678,+0000C809(r2)
	movl	#23456789,+0000C80D(r2)
	movl	#34567890,+0000C811(r2)
	tstb	(r4)
	bneq	00012288

l00012285:
	brw	00012312

l00012288:
	cvtbl	(r4),r5
	xorl3	+0000C809(r2),r5,r0
	bicl2	#FFFFFF00,r0
	movl	+0000C5EB(r2),r3
	extzv	#00000008,#18,+0000C809(r2),r1
	xorl3	(r3)[r0],r1,+0000C809(r2)
	bicl3	#FFFFFF00,+0000C809(r2),r1
	addl2	r1,+0000C80D(r2)
	mull3	#08088405,+0000C80D(r2),r1
	incl	r1
	movl	r1,+0000C80D(r2)
	movzbl	+0000C810(r2),r0
	xorl2	+0000C811(r2),r0
	bicl2	#FFFFFF00,r0
	movl	+0000C5EB(r2),r3
	extzv	#00000008,#18,+0000C811(r2),r1
	xorl3	(r3)[r0],r1,+0000C811(r2)
	incl	r4
	tstb	(r4)
	beql	00012312
	brw	00012288

l00012312:
	pushl	#0000000C
	pushl	+04(ap)
	pushal	-10(fp)
	calls	#03,@0001929C
	clrl	r7
	movab	-0010(fp),r4
	movzbl	(r4),r3
	bicl3	#FFFF0000,+0000C811(r2),r6
	bisl2	#00000002,r6
	xorl3	#00000001,r6,r0
	mull2	r6,r0
	extzv	#00000008,#18,r0,r0
	bicl2	#FFFFFF00,r0
	xorb3	r0,r3,(r4)
	movzbl	(r4),r5
	xorl3	+0000C809(r2),r5,r0
	bicl2	#FFFFFF00,r0
	movl	+0000C5EB(r2),r3
	extzv	#00000008,#18,+0000C809(r2),r1
	xorl3	(r3)[r0],r1,+0000C809(r2)
	bicl3	#FFFFFF00,+0000C809(r2),r1
	addl2	r1,+0000C80D(r2)
	mull3	#08088405,+0000C80D(r2),r1
	incl	r1
	movl	r1,+0000C80D(r2)
	movzbl	+0000C810(r2),r0
	xorl2	+0000C811(r2),r0
	bicl2	#FFFFFF00,r0
	movl	+0000C5EB(r2),r3
	extzv	#00000008,#18,+0000C811(r2),r1
	xorl3	(r3)[r0],r1,+0000C811(r2)
	incl	r4
	acbl	#0000000B,#00000001,r7,00012328
	movzbw	-05(fp),r9
	movzwl	r9,r5
	movl	+05E7(r2),r0
	bbc	#00000001,+14(r0),000123FC
	movzwl	+0000C644(r2),r3
	ashl	#F8,r3,r4
	brb	00012403
	tstl	r0
	nop
	movzbl	+0000C64B(r2),r4
	cmpl	r5,r4
	beql	0001240C
	mnegl	#00000001,r0
	ret
	movl	+0000C5FB(r2),r4
	movl	+0084(r2),r3
	cmpl	r4,r3
	bleq	00012424
	movl	r3,r1
	brb	00012427
	tstl	r0
	movl	r4,r1
	movl	r1,r3
	movl	+0000C5F7(r2),r5
	movl	r3,r0
	subl3	#00000001,r3,r6
	tstl	r0
	bneq	0001243F
	brw	000124F8
	nop
	movzbl	(r5),r3
	bicl3	#FFFF0000,+0000C811(r2),r8
	bisl2	#00000002,r8
	xorl3	#00000001,r8,r0
	mull2	r8,r0
	extzv	#00000008,#18,r0,r0
	bicl2	#FFFFFF00,r0
	xorb3	r0,r3,(r5)
	movzbl	(r5),r4
	xorl3	+0000C809(r2),r4,r0
	bicl2	#FFFFFF00,r0
	movl	+0000C5EB(r2),r3
	extzv	#00000008,#18,+0000C809(r2),r1
	xorl3	(r3)[r0],r1,+0000C809(r2)
	bicl3	#FFFFFF00,+0000C809(r2),r1
	addl2	r1,+0000C80D(r2)
	mull3	#08088405,+0000C80D(r2),r1
	incl	r1
	movl	r1,+0000C80D(r2)
	movzbl	+0000C810(r2),r0
	xorl2	+0000C811(r2),r0
	bicl2	#FFFFFF00,r0
	movl	+0000C5EB(r2),r3
	extzv	#00000008,#18,+0000C811(r2),r1
	xorl3	(r3)[r0],r1,+0000C811(r2)
	incl	r5
	movl	r6,r0
	decl	r6
	tstl	r0
	beql	000124F8
	brw	00012440
	clrl	r0
	ret
	halt
	ret
	halt

;; fn000124FE: 000124FE
;;   Called from:
;;     0000D72B (in fn0000D69A)
;;     000125CF (in fn000125CA)
;;     00012606 (in fn000125CA)
fn000124FE proc
	subl2	#00000004,sp
	movab	FFFF955C,r2

l00012503:
	polyf	-(r0),@+7E7C52FF(sp),#3F
	subw2	#0008,0001250E                                       ; @(pc)+
	subd2	@-21AF60FC(r11),@-5DAF2F81(sp)
	bgtr	00012503

l0001251B:
	movf	#0.8125,@+14A2(r0)
	movf	#0.75,-(ap)
	movaq	-(ap),-(sp)
	pushl	#00000008
	pushal	+18(r2)
	clrq	-(sp)
	pushal	+0A(r2)
	pushl	#00000027
	cvtwl	+08(r2),-(sp)
	pushl	#00000000
	calls	#0C,0001253B                                         ; @(pc)+
	halt
0001253C                                     DE FE 7F D0             ....
00012540 50 A2 14 E8 50 05 D0 A2 14 50 04 32 A2 0A A2 14 P...P....P.2....
00012550 E0 00 A2 14 05 D0 A2 14 50 04 D0 A2 18 A2 20 D0 ........P..... .
00012560 A2 1C A2 24 D5 AC 04 12 07 C8 02 A2 24 11 05 01 ...$........$...
00012570 CA 02 A2 24 7C 7E 7C 7E DD 08 DF A2 20 7C 7E DF ...$|~|~.... |~.
00012580 A2 0A DD 23 32 A2 08 7E DD 00 FB 0C 9F 00 DE FE ...#2..~........
00012590 7F D0 50 A2 14 E8 50 05 D0 A2 14 50 04 32 A2 0A ..P...P....P.2..
000125A0 A2 14 E0 00 A2 14 05 D0 A2 14 50 04 32 A2 08 7E ..........P.2..~
000125B0 FB 01 9F E0 DE FE 7F D0 50 A2 14 E8 50 05 D0 A2 ........P...P...
000125C0 14 50 04 98 8F 01 50 04 04 00                   .P....P...      

;; fn000125CA: 000125CA
fn000125CA proc
	subl2	#00000008,sp
	pushl	#00000000
	calls	#01,000124FC
	pushl	#00000001
	pushab	-05(fp)
	moval	+04(ap),r2
	pushl	(r2)
	calls	#03,@00019270
	cmpb	-05(fp),#0A
	beql	00012604

l000125EE:
	tstl	r0

l000125F0:
	pushl	#00000001
	pushab	-06(fp)
	pushl	(r2)
	calls	#03,@00019270
	cmpb	-06(fp),#0A
	bneq	000125F0

l00012604:
	pushl	#00000001
	calls	#01,000124FC
	cvtbl	-0005(fp),r0
	ret
00012613          01 FC 0F                                  ...          

;; fn00012616: 00012616
fn00012616 proc
	subl2	#0000000C,sp
	movab	FFFE682C,r8
	movab	@00019350,r7
	pushal	+0C(r8)
	pushl	#00000000
	calls	#01,@00019248
	pushl	r0
	calls	#02,@0001924C
	movl	r0,r6
	bneq	00012644

l00012641:
	clrl	r0
	ret

l00012644:
	pushl	@00019374
	calls	#01,@00019294
	moval	+0E(r8),r9
	movab	@00019258,+04(sp)
	movab	@00019294,r11
	movab	000124FC,r10
	movab	@00019254,r5
	movl	+0C(ap),r4
	moval	+08(ap),r3
	movl	+00(r3),+0000(sp)

l00012680:
	tstb	(r9)
	beql	0001268C

l00012684:
	pushl	(r7)
	pushl	r9
	calls	#02,@+0C(sp)

l0001268C:
	pushl	(r7)
	pushl	+04(ap)
	calls	#02,@+0C(sp)
	pushl	(r7)
	calls	#01,+00(r11)
	clrl	r2
	pushl	#00000000
	calls	#01,+0000(r10)

l000126A4:
	pushl	r6
	calls	#01,(r5)
	cvtlb	r0,r1
	cmpb	r1,#0D
	bneq	000126B4

l000126B1:
	movb	#0A,r1

l000126B4:
	cmpl	r2,r4
	bgeq	000126C3

l000126B9:
	movl	r2,r0
	incl	r2
	movb	r1,@+00(r3)[r0]

l000126C3:
	cmpb	r1,#0A
	bneq	000126A4

l000126C8:
	pushl	#00000001
	calls	#01,(r10)
	pushl	(r7)
	pushl	#0000000A
	calls	#02,@00019268
	pushl	(r7)
	calls	#01,(r11)
	moval	+0F(r8),r9
	subl3	#00000001,r2,r0
	cmpb	@+00(sp)[r0],#0A
	bneq	00012680

l000126EC:
	movl	+08(ap),r3
	clrb	(r3)[r0]
	pushl	r6
	calls	#01,@00019250
	movl	r3,r0
	ret
00012700 FC 0F                                           ..              

;; fn00012702: 00012702
;;   Called from:
;;     00008119 (in fn0000802E)
fn00012702 proc
	subl2	#00000010,sp
	movab	FFFFB3D0,r2
	clrl	+08(sp)
	clrl	r1
	movl	#00000001,r7
	clrl	r8
	movl	#00000001,r6
	clrl	r11
	clrq	r9
	clrl	+74(r2)
	movl	@+04(ap),r4
	movl	@+08(ap),+04(sp)
	subl3	#00000001,r4,(sp)
	bgtr	00012732

l0001272F:
	brw	0001295D

l00012732:
	addl2	#00000004,+04(sp)
	movl	+04(sp),r3
	cmpb	@+00(r3),#2D
	beql	00012743

l00012740:
	brw	0001295D

l00012743:
	nop

l00012744:
	addl3	#00000001,@+04(sp),r0
	movl	r0,r3
	addl3	#00000001,r0,r5
	cvtbl	(r3),r4
	bneq	00012758

l00012755:
	brw	00012948

l00012758:
	movl	r4,r0
	casel	r0,#0000002D,#0000004D
00012763          A9 00 D9 01 D9 01 D9 01 B1 00 C9 00 D9    .............
00012770 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 ................
00012780 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 01 E1 ................
00012790 00 D9 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 ................
000127A0 01 D9 01 4D 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 ...M............
000127B0 01 99 01 D9 01 D9 01 D9 01 D9 01 D9 01 D5 01 D9 ................
000127C0 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 01 D9 ................
000127D0 01 D9 01 D9 01 D9 01 D9 01 F5 00 D9 01 D9 01 D9 ................
000127E0 01 1D 01 35 01 D9 01 D9 01 D9 01 D9 01 D9 01 61 ...5...........a
000127F0 01 75 01 D9 01 AD 01 D9 01 D9 01 D9 01          .u...........   

l000127FD:
	Invalid
	nop
	tstl	r0
	jmp	40012700
	tstl	r0
	incl	r1
	brw	00012940
	tstl	r0
	nop
	tstl	r1
	beql	00012824
	mnegl	#00000002,+24(r2)
	clrl	r1
	brw	00012940
	tstl	r0
	nop
	movl	#00000001,+24(r2)
	brw	00012940
	nop
	tstl	r1
	beql	0001283C
	mnegl	#00000002,+24(r2)
	clrl	r1
	brw	00012940
	tstl	r0
	nop
	movl	#00000002,+24(r2)
	brw	00012940
	nop
	tstl	r1
	beql	00012850
	clrl	+10(r2)
	clrl	r1
	brw	00012940
	movl	#00000001,+10(r2)
	brw	00012940
	nop
	tstl	r1
	beql	00012864
	clrq	r7
	clrl	r1
	brw	00012940
	nop
	movl	#00000001,r9
	movl	#00000001,r7
	movl	#00000001,r8
	cmpl	+24(r2),#FFFFFFFF
	beql	0001287A
	brw	00012940
	clrl	+24(r2)
	brw	00012940
	tstl	r1
	beql	00012890
	mnegl	#00000002,+24(r2)
	clrl	r1
	brw	00012940
	tstl	r0
	nop
	movl	#00000005,+24(r2)
	brw	00012940
	nop
	tstl	r1
	beql	000128A8
	mnegl	#00000002,+24(r2)
	clrl	r1
	brw	00012940
	tstl	r0
	nop
	movl	#00000004,+24(r2)
	brw	00012940
	nop
	tstl	r1
	beql	000128BC
	clrl	+2C(r2)
	clrl	r1
	brw	00012940
	movl	#00000001,+2C(r2)
	brb	00012940
	tstl	r0
	tstl	r1
	beql	000128D0
	mnegl	#00000002,+24(r2)
	clrl	r1
	brb	00012940
	movl	#00000003,+24(r2)
	brb	00012940
	tstl	r0
	tstl	r1
	beql	000128E4
	clrl	r6
	clrl	r11
	clrl	r1
	brb	00012940
	movl	#00000001,r10
	movl	#00000001,r6
	movl	#00000001,r11
	cmpl	+24(r2),#FFFFFFFF
	bneq	00012940
	clrl	+24(r2)
	brb	00012940
	tstl	r1
	beql	00012908
	clrl	+48(r2)
	clrl	r1
	brb	00012940
	nop
	movl	#00000001,+48(r2)
	brb	00012940
	tstl	r0
	tstl	r1
	beql	0001291C
	mnegl	#00000002,+24(r2)
	clrl	r1
	brb	00012940
	movl	#0000000A,+24(r2)
	brb	00012940
	tstl	r0
	tstl	r1
	beql	00012930
	clrl	r1
	clrl	+5C(r2)
	brb	00012940
	nop
	movl	#00000001,+5C(r2)
	brb	00012940
	tstl	r0
	brb	00012940
	tstl	r0
	movl	#00000001,+08(sp)
	cvtbl	(r5)+,r4
	beql	00012948
	brw	00012758

l00012948:
	decl	(sp)
	bleq	0001295D

l0001294C:
	addl2	#00000004,+04(sp)
	movl	+04(sp),r3
	cmpb	@+00(r3),#2D
	bneq	0001295D

l0001295A:
	brw	00012744

l0001295D:
	movl	(sp),r3
	decl	(sp)
	tstl	r3
	beql	0001296B

l00012966:
	tstl	+08(sp)
	beql	0001297F

l0001296B:
	movl	(sp),@+04(ap)
	movl	+04(sp),@+08(ap)
	pushl	+08(sp)
	calls	#01,00008CCC
	ret

l0001297F:
	tstl	+2C(r2)
	beql	00012994

l00012984:
	pushl	#00000001
	calls	#01,@000192E4
	tstl	r0
	bneq	00012994

l00012991:
	clrl	+2C(r2)

l00012994:
	tstl	+24(r2)
	blss	000129A2

l00012999:
	tstl	(sp)
	bleq	000129A6

l0001299D:
	tstl	+24(r2)
	bneq	000129A6

l000129A2:
	movl	#00000003,+24(r2)

l000129A6:
	movl	+24(r2),r0
	casel	r0,#00000000,#0000000A
000129AE                                           22 00               ".
000129B0 2E 00 22 00 3A 00 3A 00 3A 00 6E 00 6E 00 6E 00 ..".:.:.:.n.n.n.
000129C0 6E 00                                           n.              

l000129C2:
	divd2	#0.5,#0011
	cvtfd	@+1750(r5),40012700
	tstl	r0
	movl	r8,+1C(r2)
	movl	r11,+44(r2)
	brb	00012A1C
	tstl	r0
	clrl	+1C(r2)
	clrl	+44(r2)
	clrl	+5C(r2)
	brb	00012A1C
	nop
	tstl	(sp)
	bleq	000129F4
	tstl	r9
	bneq	000129F4
	clrl	r3
	brb	000129F7
	movl	r7,r3
	movl	r3,+1C(r2)
	tstl	(sp)
	bleq	00012A08
	tstl	r10
	bneq	00012A08
	clrl	r3
	brb	00012A0B
	nop
	movl	r6,r3
	movl	r3,+44(r2)
	brb	00012A1C
	tstl	r0
	nop
	movl	r7,+1C(r2)
	movl	r11,+44(r2)
	movl	(sp),@+04(ap)
	movl	+04(sp),@+08(ap)
	clrl	r0
	ret
	xfc
	nop

;; fn00012A2A: 00012A2A
;;   Called from:
;;     0001104E (in fn00010E6A)
fn00012A2A proc
	subl2	#00000004,sp
	movab	FFFE6858,r5
	movab	FFFFB3D0,r2
	movab	FFFF9584,r4
	clrl	r8
	tstl	+1C(r2)
	beql	00012AAB

l00012A49:
	pushl	#00000000
	movzwl	+0000C688(r2),r0
	cmpl	r0,#00000001
	bneq	00012A5C

l00012A57:
	moval	(r4),r6
	brb	00012A5F

l00012A5C:
	moval	(r5),r6

l00012A5F:
	pushl	r6
	pushl	r0
	pushl	+0000C61B(r2)
	pushl	+0000C613(r2)
	pushl	+0000C613(r2)
	calls	#01,@000192C4
	cmpl	r0,#00000027
	bgeq	00012A88

l00012A81:
	moval	+01(r4),r3
	brb	00012A8C
00012A87                      01                                .        

l00012A88:
	moval	+27(r4),r3

l00012A8C:
	pushl	r3
	pushab	+05EB(r2)
	calls	#06,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)

l00012AAB:
	cmpl	+24(r2),#00000009
	bgtr	00012AB4

l00012AB1:
	brw	00012D34

l00012AB4:
	pushl	#00000000
	pushal	+40(r4)
	pushab	+05EB(r2)
	movab	@000192D0,r7
	calls	#02,(r7)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	pushl	#00000000
	pushal	+63(r4)
	pushab	+05EB(r2)
	calls	#02,(r7)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	pushl	#00000000
	pushl	+0080(r2)
	pushl	+0080(r2)
	pushl	+7C(r2)
	pushl	+7C(r2)
	pushal	+0085(r4)
	pushab	+05EB(r2)
	calls	#06,(r7)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	movzwl	+0000C682(r2),r6
	bneq	00012BA4

l00012B2D:
	pushl	#00000000
	pushl	+0000C68A(r2)
	pushl	+0000C68A(r2)
	movzwl	+0000C688(r2),r0
	cmpl	r0,#00000001
	bneq	00012B50

l00012B47:
	moval	+0A(r5),r3
	brb	00012B54
00012B4D                                        D5 50 01              .P.

l00012B50:
	moval	+02(r5),r3

l00012B54:
	pushl	r3
	pushl	r0
	pushal	+014C(r4)
	pushab	+05EB(r2)
	calls	#06,(r7)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	pushl	#00000000
	pushl	+0000C68E(r2)
	pushl	+0000C68E(r2)
	pushal	+0219(r4)
	pushab	+05EB(r2)
	calls	#04,(r7)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	brw	00012C5C
00012BA3          01                                        .            

l00012BA4:
	pushl	#00000000
	movzwl	+0000C686(r2),r0
	cmpl	r0,#00000001
	bneq	00012BB8

l00012BB2:
	moval	+14(r5),r3
	brb	00012BBC

l00012BB8:
	moval	+10(r5),r3

l00012BBC:
	pushl	r3
	pushl	r0
	movzwl	+0000C684(r2),ap
	incl	ap
	pushl	ap
	incl	r6
	pushl	r6
	pushal	+0247(r4)
	pushab	+05EB(r2)
	calls	#06,(r7)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	pushl	#00000000
	pushl	+0000C68A(r2)
	pushl	+0000C68A(r2)
	movzwl	+0000C688(r2),r0
	cmpl	r0,#00000001
	bneq	00012C0C

l00012C06:
	moval	+1F(r5),r3
	brb	00012C10

l00012C0C:
	moval	+17(r5),r3

l00012C10:
	pushl	r3
	pushl	r0
	pushal	+02D6(r4)
	pushab	+05EB(r2)
	calls	#06,(r7)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	pushl	#00000000
	pushl	+0000C68E(r2)
	pushl	+0000C68E(r2)
	pushal	+0359(r4)
	pushab	+05EB(r2)
	calls	#04,(r7)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)

l00012C5C:
	tstw	+0000C692(r2)
	bneq	00012C8C

l00012C64:
	pushl	#00000000
	pushal	+039E(r4)
	pushab	+05EB(r2)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	brw	00012D7E
00012C8A                               D5 50                       .P    

l00012C8C:
	pushl	#00000000
	movzwl	+0000C692(r2),-(sp)
	pushal	+03BE(r4)
	pushab	+05EB(r2)
	movab	@000192D0,r3
	calls	#03,(r3)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	pushl	#00000000
	pushal	+0408(r4)
	pushab	+05EB(r2)
	calls	#02,(r3)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	pushl	#00000001
	movzwl	+0000C692(r2),-(sp)
	calls	#02,0000DC74
	tstl	r0
	beql	00012CEF

l00012CEC:
	movl	#00000001,r8

l00012CEF:
	pushl	#00000000
	pushal	+0454(r4)
	pushab	+05EB(r2)
	calls	#02,(r3)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	tstl	r8
	beql	00012D7E

l00012D12:
	pushl	#00000000
	pushal	+04A0(r4)
	pushab	+05EB(r2)
	calls	#02,(r3)
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	brb	00012D7E
00012D33          01                                        .            

l00012D34:
	tstl	+5C(r2)
	beql	00012D7E

l00012D39:
	tstw	+0000C692(r2)
	beql	00012D7E

l00012D41:
	pushl	#00000001
	movzwl	+0000C692(r2),-(sp)
	calls	#02,0000DC74
	tstl	r0
	beql	00012D7E

l00012D55:
	movzwl	#0401,-(sp)
	pushal	+04C6(r4)
	pushab	+05EB(r2)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r2)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	movl	#00000001,r8

l00012D7E:
	movl	r8,r0
	ret
00012D82       D5 50 FC 0F                                 .P..          

;; fn00012D86: 00012D86
fn00012D86 proc
	subl2	#00000014,sp
	movab	FFFFB3D0,r3
	movab	FFFF9584,r11
	clrl	r5
	clrl	r10
	clrq	r6
	clrw	+02(sp)
	clrq	+04(sp)
	tstl	+64(r3)
	bleq	00012DCB

l00012DA8:
	ashl	#02,+64(r3),-(sp)
	calls	#01,@000192B4
	movl	r0,r6
	beql	00012DCB

l00012DB9:
	clrl	r2
	tstl	+64(r3)
	bleq	00012DCB

l00012DC0:
	clrl	(r6)[r2]
	incl	r2
	cmpl	r2,+64(r3)
	blss	00012DC0

l00012DCB:
	tstl	+68(r3)
	bleq	00012DF3

l00012DD0:
	ashl	#02,+68(r3),-(sp)
	calls	#01,@000192B4
	movl	r0,r7
	beql	00012DF3

l00012DE1:
	clrl	r2
	tstl	+68(r3)
	bleq	00012DF3

l00012DE8:
	clrl	(r7)[r2]
	incl	r2
	cmpl	r2,+68(r3)
	blss	00012DE8

l00012DF3:
	clrl	+28(r3)
	movab	+00A7(r3),+05E7(r3)
	movl	+05E7(r3),r2
	bicb2	#08,+14(r2)
	cmpl	+0000C67E(r3),#00000004
	bneq	00012E14

l00012E0F:
	movb	#04,r4
	brb	00012E16

l00012E14:
	clrb	r4

l00012E16:
	cvtbl	r4,-08(fp)
	clrl	r9
	movzwl	+0000C688(r3),r2
	cmpl	r9,r2
	blss	00012E2B

l00012E28:
	brw	000130CF

l00012E2B:
	movab	0000DC74,r8
	movab	00018C18,r4
	tstl	r0
	nop

l00012E3C:
	pushl	#00000004
	pushab	+0098(r3)
	calls	#02,0000D5BC
	tstl	r0
	bneq	00012E51

l00012E4D:
	movl	#00000033,r0
	ret

l00012E51:
	pushl	#00000004
	pushab	+0000C634(r3)
	pushab	+0098(r3)
	calls	#03,@000192D8
	tstl	r0
	beql	00012E96

l00012E68:
	movzwl	#0401,-(sp)
	pushl	r9
	pushal	FFFFB15C
	pushab	+05EB(r3)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r3)
	pushab	(r3)
	movl	+0000C839(r3),r2
	calls	#04,(r2)
	movl	#00000003,r0
	ret

l00012E96:
	calls	#00,00011C08
	tstl	r0
	beql	00012EA2

l00012EA1:
	ret

l00012EA2:
	pushl	#00000002
	movzwl	+0000C670(r3),-(sp)
	calls	#02,(r8)
	tstl	r0
	beql	00012EBB

l00012EB2:
	movl	r0,r10
	cmpl	r0,#00000001
	bleq	00012EBB

l00012EBA:
	ret

l00012EBB:
	tstl	+6C(r3)
	bneq	00012F3F

l00012EC0:
	subl3	#00000004,+0090(r3),ap
	clrl	r5
	addl3	#00000004,ap,r2
	tstl	(r2)
	beql	00012EFF

l00012ED0:
	pushl	+10(r3)
	pushl	(r2)
	pushab	+0000C701(r3)
	calls	#03,(r4)
	tstl	r0
	beql	00012EF8

l00012EE2:
	movl	#00000001,r5
	tstl	r6
	beql	00012EFF

l00012EE9:
	subl3	+0090(r3),r2,ap
	divl2	#00000004,ap
	movl	#00000001,(r6)[ap]
	brb	00012EFF

l00012EF8:
	addl2	#00000004,r2
	tstl	(r2)
	bneq	00012ED0

l00012EFF:
	tstl	r5
	beql	00012F3F

l00012F03:
	subl3	#00000004,+0094(r3),r2
	addl2	#00000004,r2
	tstl	(r2)
	beql	00012F3F

l00012F10:
	pushl	+10(r3)
	pushl	(r2)
	pushab	+0000C701(r3)
	calls	#03,(r4)
	tstl	r0
	beql	00012F38

l00012F22:
	clrl	r5
	tstl	r7
	beql	00012F3F

l00012F28:
	subl3	+0094(r3),r2,ap
	divl2	#00000004,ap
	movl	#00000001,(r7)[ap]
	brb	00012F3F
00012F37                      01                                .        

l00012F38:
	addl2	#00000004,r2
	tstl	(r2)
	bneq	00012F10

l00012F3F:
	tstl	+6C(r3)
	bneq	00012F4B

l00012F44:
	tstl	r5
	bneq	00012F4B

l00012F48:
	brw	0001307C

l00012F4B:
	movl	+24(r3),r0
	casel	r0,#00000001,#00000009
00012F53          21 00 21 00 73 00 73 00 73 00 C2 00 C2    !.!.s.s.s....
00012F60 00 C2 00 C2 00                                  .....           

l00012F65:
	bisb2	#00,#31
	bicw3	#0000,@+1750(r5),40012700
	tstl	r0
	calls	#00,00017770
	tstw	+0000C672(r3)
	beql	00012F9C
	pushl	#00000000
	movzwl	+0000C672(r3),-(sp)
	calls	#02,(r8)
	tstl	r0
	beql	00012F9C
	movl	r0,r10
	cmpl	r0,#00000001
	bleq	00012F9C
	ret
	tstw	+0000C674(r3)
	bneq	00012FA7
	brw	00013057
	pushl	#00000000
	movzwl	+0000C674(r3),-(sp)
	calls	#02,(r8)
	tstl	r0
	bneq	00012FBA
	brw	00013057
	movl	r0,r10
	cmpl	r0,#00000001
	bgtr	00012FC5
	brw	00013057
	ret
	calls	#00,00014810
	tstl	r0
	bneq	00012FD2
	brw	00013057
	movl	r0,r10
	cmpl	r0,#00000001
	bleq	00013057
	ret
	pushl	#00000000
	pushl	r9
	pushal	+04EC(r11)
	pushab	+05EB(r3)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r3)
	pushab	(r3)
	movl	+0000C839(r3),r2
	calls	#04,(r2)
	pushal	-08(fp)
	calls	#01,0001325C
	tstl	r0
	beql	00013057
	movl	r0,r10
	cmpl	r0,#00000001
	bleq	00013057
	ret
	tstw	+0000C672(r3)
	beql	00013036
	pushl	#00000000
	movzwl	+0000C672(r3),-(sp)
	calls	#02,(r8)
	tstl	r0
	beql	00013036
	movl	r0,r10
	cmpl	r0,#00000001
	bleq	00013036
	ret
	tstw	+0000C674(r3)
	beql	00013057
	pushl	#00000000
	movzwl	+0000C674(r3),-(sp)
	calls	#02,(r8)
	tstl	r0
	beql	00013057
	movl	r0,r10
	cmpl	r0,#00000001
	bleq	00013057
	ret
	addl2	+0000C668(r3),+08(sp)
	addl2	+0000C66C(r3),+04(sp)
	movzwl	+0000C65C(r3),r2
	blbc	r2,00013075
	subl2	#0000000C,+08(sp)
	incw	+02(sp)
	brb	000130BE
	tstl	r0

l0001307C:
	tstw	+0000C672(r3)
	beql	0001309D

l00013084:
	pushl	#00000000
	movzwl	+0000C672(r3),-(sp)
	calls	#02,(r8)
	tstl	r0
	beql	0001309D

l00013094:
	movl	r0,r10
	cmpl	r0,#00000001
	bleq	0001309D

l0001309C:
	ret

l0001309D:
	tstw	+0000C674(r3)
	beql	000130BE

l000130A5:
	pushl	#00000000
	movzwl	+0000C674(r3),-(sp)
	calls	#02,(r8)
	tstl	r0
	beql	000130BE

l000130B5:
	movl	r0,r10
	cmpl	r0,#00000001
	bleq	000130BE

l000130BD:
	ret

l000130BE:
	incl	r9
	movzwl	+0000C688(r3),r2
	cmpl	r9,r2
	bgeq	000130CF

l000130CC:
	brw	00012E3C

l000130CF:
	pushl	#00000004
	pushab	+0098(r3)
	calls	#02,0000D5BC
	tstl	r0
	bneq	000130E4

l000130E0:
	movl	#00000033,r0
	ret

l000130E4:
	pushl	#00000004
	pushab	+0000C639(r3)
	pushab	+0098(r3)
	calls	#03,@000192D8
	tstl	r0
	beql	00013126

l000130FB:
	movzwl	#0401,-(sp)
	pushal	FFFFB328
	pushab	+05EB(r3)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r3)
	pushab	(r3)
	movl	+0000C839(r3),r2
	calls	#04,(r2)
	movl	#00000001,r10

l00013126:
	tstl	+44(r3)
	beql	000131A4

l0001312B:
	moval	FFFE687D,r9
	pushl	+08(sp)
	pushl	+08(sp)
	calls	#02,00017660
	movl	r0,r5
	bgeq	0001314E

l00013144:
	moval	FFFE687E,r9
	mnegl	r5,r5

l0001314E:
	pushl	#00000000
	emul	#00000000,#00000000,r5,r0
	ediv	#0000000A,r0,r1,r0
	pushl	r0
	divl3	#0000000A,r5,-(sp)
	pushl	r9
	pushl	+18(sp)
	pushl	+18(sp)
	movzwl	+1A(sp),r4
	cmpl	r4,#00000001
	bneq	00013178
	moval	(r11),r8
	brb	0001317F
	tstl	r0
	moval	FFFE6880,r8
	pushl	r8
	pushl	r4
	pushal	+0528(r11)
	pushab	+05EB(r3)
	calls	#09,@000192D0
	pushl	r0
	pushab	+05EB(r3)
	pushab	(r3)
	movl	+0000C839(r3),r2
	calls	#04,(r2)

l000131A4:
	tstl	r6
	beql	000131F6

l000131A8:
	clrl	ap
	tstl	+64(r3)
	bleq	000131ED

l000131AF:
	nop

l000131B0:
	tstl	(r6)[ap]
	bneq	000131E5

l000131B5:
	movzwl	#0401,-(sp)
	movl	+0090(r3),r2
	pushl	(r2)[ap]
	pushal	FFFFB3A4
	pushab	+05EB(r3)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r3)
	pushab	(r3)
	movl	+0000C839(r3),r2
	calls	#04,(r2)

l000131E5:
	incl	ap
	cmpl	ap,+64(r3)
	blss	000131B0

l000131ED:
	pushl	r6
	calls	#01,@000192B0

l000131F6:
	tstl	r7
	beql	0001324A

l000131FA:
	clrl	ap
	tstl	+68(r3)
	bleq	00013241

l00013201:
	tstl	r0
	nop

l00013204:
	tstl	(r7)[ap]
	bneq	00013239

l00013209:
	movzwl	#0401,-(sp)
	movl	+0094(r3),r2
	pushl	(r2)[ap]
	pushal	FFFFB374
	pushab	+05EB(r3)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r3)
	pushab	(r3)
	movl	+0000C839(r3),r2
	calls	#04,(r2)

l00013239:
	incl	ap
	cmpl	ap,+68(r3)
	blss	00013204

l00013241:
	pushl	r7
	calls	#01,@000192B0

l0001324A:
	movzwl	+02(sp),r2
	bneq	00013258

l00013250:
	cmpl	r10,#00000001
	bgtr	00013258

l00013255:
	movl	#0000000B,r10

l00013258:
	movl	r10,r0
	ret
0001325C                                     FC 0F                   ..  

;; fn0001325E: 0001325E
fn0001325E proc
	movab	-00EC(sp),sp
	movab	FFFE6858,r8
	movab	FFFFB3D0,r5
	movab	FFFF9584,r6
	clrl	+08(sp)
	movl	@+04(ap),r2
	cmpl	+0000C67E(r5),r2
	beql	000132B7

l00013288:
	tstl	r2
	beql	000132B7

l0001328C:
	pushl	#00000000
	subl3	r2,+0000C67E(r5),-(sp)
	pushal	+06CF(r6)
	pushab	+05EB(r5)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)

l000132B7:
	addl3	#0000001E,+0000C67E(r5),r3
	movzwl	+0000C670(r5),r2
	addl2	r2,r3
	movzwl	+0000C672(r5),r2
	addl2	r2,r3
	movzwl	+0000C674(r5),r2
	addl2	r2,r3
	addl3	+0000C668(r5),r3,@+04(ap)
	pushl	#00000003
	movzwl	+0000C672(r5),-(sp)
	calls	#02,0000DC74
	movl	r0,+04(sp)
	beql	0001331C

l000132FC:
	tstl	+0000C627(r5)
	beql	00013317

l00013304:
	pushl	+0000C627(r5)
	calls	#01,@000192B0
	clrl	+0000C627(r5)

l00013317:
	movl	+04(sp),+08(sp)

l0001331C:
	movl	+05E7(r5),r2
	movw	+0C(r2),+02(sp)
	movzbw	+0000C658(r5),-06(fp)
	movzbl	+0000C65B(r5),r2
	cmpl	r2,#00000012
	bgequ	00013344

l0001333A:
	movb	+0000C65B(r5),r4
	brb	00013347
00013343          01                                        .            

l00013344:
	movb	#12,r4

l00013347:
	movzbw	r4,r9
	movzbw	+0000C65A(r5),r10
	movzwl	+0000C65E(r5),r2
	cmpl	r2,#0000000B
	bgequ	00013368

l0001335D:
	movw	+0000C65E(r5),r3
	brb	0001336B
00013366                   D5 50                               .P        

l00013368:
	movw	#000B,r3

l0001336B:
	movw	r3,+10(sp)
	pushl	#00000000
	pushl	#00000002
	pushal	+2A(r8)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	calls	#00,00017770
	pushl	#00000000
	pushl	+0000C67E(r5)
	pushl	+0000C67E(r5)
	pushal	+0712(r6)
	pushab	+05EB(r5)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movzwl	+02(sp),r3
	cmpl	r3,#00000012
	blssu	000133DC

l000133C1:
	movzbl	+0000C659(r5),-(sp)
	pushal	+0705(r6)
	pushal	-3A(fp)
	calls	#03,@000192D0
	moval	-3A(fp),r4
	brb	000133E2

l000133DC:
	movl	+1444(r6)[r3],r4

l000133E2:
	pushl	#00000000
	pushl	r4
	pushal	+075B(r6)
	pushab	+05EB(r5)
	movab	@000192D0,r7
	calls	#03,(r7)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	clrl	r1
	movzwl	-06(fp),r0
	movzwl	#000A,r2
	beql	00013430

l00013417:
	cmpl	r2,#00000001
	beql	00013435

l0001341C:
	bgtr	00013430

l0001341E:
	cmpl	r2,r0
	beql	00013435

l00013423:
	bgtru	00013428

l00013425:
	subl2	r2,r1

l00013428:
	addl2	r0,r1
	brb	00013435
0001342D                                        D5 50 01              .P.

l00013430:
	ediv	r2,r0,r0,r1

l00013435:
	movzwl	r1,-(sp)
	movzwl	-06(fp),r2
	clrl	r1
	movl	r2,r0
	movl	#0000000A,r2
	bgeq	00013450

l00013446:
	cmpl	r2,r0
	bgtru	00013455

l0001344B:
	incl	r1
	brb	00013455
0001344F                                              01                .

l00013450:
	ediv	r2,r0,r1,r0

l00013455:
	pushl	r1
	pushal	+0793(r6)
	pushab	+05EB(r5)
	calls	#04,(r7)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movzwl	r9,r3
	cmpl	r3,#00000012
	blssu	00013494

l0001347C:
	movzbl	+0000C65B(r5),-(sp)
	pushal	+0705(r6)
	pushal	-3A(fp)
	calls	#03,(r7)
	moval	-3A(fp),r4
	brb	0001349A
00013493          01                                        .            

l00013494:
	movl	+1444(r6)[r3],r4

l0001349A:
	pushl	#00000000
	pushl	r4
	pushal	+07CE(r6)
	pushab	+05EB(r5)
	movab	@000192D0,r7
	calls	#03,(r7)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	clrl	r1
	movzwl	r10,r0
	movzwl	#000A,r2
	beql	000134E4

l000134CE:
	cmpl	r2,#00000001
	beql	000134E9

l000134D3:
	bgtr	000134E4

l000134D5:
	cmpl	r2,r0
	beql	000134E9

l000134DA:
	bgtru	000134DF

l000134DC:
	subl2	r2,r1

l000134DF:
	addl2	r0,r1
	brb	000134E9

l000134E4:
	ediv	r2,r0,r0,r1

l000134E9:
	movzwl	r1,-(sp)
	movzwl	r10,r2
	clrl	r1
	movl	r2,r0
	movl	#0000000A,r2
	bgeq	00013504

l000134F9:
	cmpl	r2,r0
	bgtru	00013509

l000134FE:
	incl	r1
	brb	00013509
00013502       D5 50                                       .P            

l00013504:
	ediv	r2,r0,r1,r0

l00013509:
	pushl	r1
	pushal	+0806(r6)
	pushab	+05EB(r5)
	calls	#04,(r7)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movzwl	+10(sp),r3
	cmpl	r3,#0000000B
	blssu	00013548

l00013531:
	movzwl	+0000C65E(r5),-(sp)
	pushal	+0705(r6)
	pushal	-3A(fp)
	calls	#03,(r7)
	moval	-3A(fp),r4
	brb	0001354E

l00013548:
	movl	+148C(r6)[r3],r4

l0001354E:
	pushl	#00000000
	pushl	r4
	pushal	+0841(r6)
	pushab	+05EB(r5)
	movab	@000192D0,r7
	calls	#03,(r7)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movzwl	+10(sp),r4
	cmpl	r4,#00000006
	bneq	000135F4

l0001357F:
	pushl	#00000000
	movzwl	+0000C65C(r5),r2
	bbc	#00000001,r2,00013594

l0001358C:
	movb	#38,r3
	brb	00013597
00013591    D5 50 01                                      .P.            

l00013594:
	movb	#34,r3

l00013597:
	cvtbl	r3,-(sp)
	pushal	+0879(r6)
	pushab	+05EB(r5)
	calls	#03,(r7)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	movzwl	+0000C65C(r5),r2
	bbc	#00000002,r2,000135CC

l000135C4:
	movb	#33,r3
	brb	000135CF
000135C9                            D5 50 01                      .P.    

l000135CC:
	movb	#32,r3

l000135CF:
	cvtbl	r3,-(sp)
	pushal	+08B2(r6)
	pushab	+05EB(r5)
	calls	#03,(r7)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brb	00013638
000135F1    D5 50 01                                      .P.            

l000135F4:
	cmpl	r4,#00000008
	bneq	00013638

l000135F9:
	movzwl	+0000C65C(r5),r2
	ashl	#FF,r2,r2
	bicl2	#FFFFFFFC,r2
	cvtlw	r2,-08(fp)
	pushl	#00000000
	movzwl	-08(fp),r2
	pushl	+14B8(r6)[r2]
	pushal	+08EA(r6)
	pushab	+05EB(r5)
	calls	#03,(r7)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)

l00013638:
	pushl	#00000000
	movzwl	+0000C65C(r5),r2
	blbc	r2,0001364C

l00013644:
	moval	(r6),r3
	brb	00013650
00013649                            D5 50 01                      .P.    

l0001364C:
	moval	+2D(r8),r3

l00013650:
	pushl	r3
	pushal	+0922(r6)
	pushab	+05EB(r5)
	movab	@000192D0,r4
	calls	#03,(r4)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	movzwl	+0000C65C(r5),r2
	bbc	#00000003,r2,0001368C

l00013683:
	moval	+35(r8),r3
	brb	00013690
00013689                            D5 50 01                      .P.    

l0001368C:
	moval	+32(r8),r3

l00013690:
	pushl	r3
	pushal	+0963(r6)
	pushab	+05EB(r5)
	calls	#03,(r4)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushal	-2A(fp)
	pushl	#00000000
	pushab	+0000C660(r5)
	pushab	+0000C662(r5)
	calls	#04,00015104
	pushl	#00000000
	pushal	-2A(fp)
	pushal	+099B(r6)
	pushab	+05EB(r5)
	calls	#03,(r4)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	pushl	+0000C664(r5)
	pushal	+09D3(r6)
	pushab	+05EB(r5)
	calls	#03,(r4)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	pushl	+0000C668(r5)
	pushal	+0A0E(r6)
	pushab	+05EB(r5)
	calls	#03,(r4)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	pushl	+0000C66C(r5)
	pushal	+0A4D(r6)
	pushab	+05EB(r5)
	calls	#03,(r4)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	movzwl	+0000C670(r5),-(sp)
	pushal	+0A8C(r6)
	pushab	+05EB(r5)
	calls	#03,(r4)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	movzwl	+0000C672(r5),-(sp)
	pushal	+0ACF(r6)
	pushab	+05EB(r5)
	calls	#03,(r4)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	movzwl	+0000C674(r5),-(sp)
	pushal	+0B0D(r6)
	pushab	+05EB(r5)
	calls	#03,(r4)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	movzwl	+0000C676(r5),r2
	incl	r2
	pushl	r2
	pushal	+0B50(r6)
	pushab	+05EB(r5)
	calls	#03,(r4)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000000
	movzwl	+0000C678(r5),r3
	blbc	r3,00013804

l000137FE:
	moval	+47(r8),r4
	brb	00013817

l00013804:
	bbc	#00000001,r3,00013810

l00013808:
	moval	+40(r8),r3
	brb	00013814
0001380E                                           D5 50               .P

l00013810:
	moval	+39(r8),r3

l00013814:
	movl	r3,r4

l00013817:
	pushl	r4
	pushal	+0B8D(r6)
	pushab	+05EB(r5)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movzwl	+0000C67C(r5),r2
	bicw3	#0000,r2,+0E(sp)
	movzwl	+02(sp),r9
	cmpl	r9,#00000002
	beql	00013854

l00013851:
	brw	00013958

l00013854:
	moval	-2A(fp),r3
	moval	-002A(fp),r2
	addl3	#00000001,r2,r4
	clrl	r0
	movab	-0014(fp),r2

l00013868:
	clrb	(r2)
	incl	r2
	aobleq	#0000000B,r0,00013868

l00013870:
	movzwl	+0E(sp),r7
	bbc	#00000008,r7,0001387D

l00013878:
	movb	#52,-14(fp)

l0001387D:
	bbc	#00000007,r7,0001388B

l00013881:
	movb	#57,-13(fp)
	movb	#44,-11(fp)

l0001388B:
	bbc	#00000006,r7,00013894

l0001388F:
	movb	#45,-12(fp)

l00013894:
	bbc	#00000005,r7,0001389D

l00013898:
	movb	#52,-10(fp)

l0001389D:
	bbc	#00000004,r7,000138AB

l000138A1:
	movb	#57,-0F(fp)
	movb	#44,-0D(fp)

l000138AB:
	bbc	#00000003,r7,000138B4

l000138AF:
	movb	#45,-0E(fp)

l000138B4:
	bbc	#00000002,r7,000138BD

l000138B8:
	movb	#52,-0C(fp)

l000138BD:
	bbc	#00000001,r7,000138CB

l000138C1:
	movb	#57,-0B(fp)
	movb	#44,-09(fp)

l000138CB:
	blbc	r7,000138D3

l000138CE:
	movb	#45,-0A(fp)

l000138D3:
	movb	#28,(r3)+
	clrl	r7
	clrl	r2
	cmpl	r7,#00000003
	bgeq	00013920

l000138DF:
	nop

l000138E0:
	clrl	r1
	tstl	r0

l000138E4:
	tstb	-14(fp)[r2]
	beql	000138EF

l000138EA:
	movb	-14(fp)[r2],(r3)+

l000138EF:
	incl	r1
	incl	r2
	cmpl	r1,#00000004
	blss	000138E4

l000138F8:
	movb	#2C,(r3)+
	tstl	r7
	bneq	00013919

l000138FF:
	movl	r3,r1
	incl	r3
	movb	(r4)+,(r1)
	cmpb	(r1),#2C
	beql	00013919

l0001390C:
	movl	r3,r1
	incl	r3
	movb	(r4)+,(r1)
	cmpb	(r1),#2C
	bneq	0001390C

l00013919:
	incl	r7
	cmpl	r7,#00000003
	blss	000138E0

l00013920:
	movl	r3,r2
	decl	r3
	clrb	(r2)
	movb	#29,(r3)
	pushl	#00000000
	pushal	-2A(fp)
	movzwl	+16(sp),-(sp)
	pushal	+0BC5(r6)
	pushab	+05EB(r5)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brw	00013C9E
00013957                      01                                .        

l00013958:
	cmpl	r9,#00000001
	beql	00013960

l0001395D:
	brw	00013A64

l00013960:
	movzwl	+0E(sp),r2
	bicl2	#FFFFF3FF,r2
	cmpl	r2,#00000400
	blss	00013990

l00013974:
	beql	00013988

l00013976:
	cmpl	r2,#00000800
	bneq	00013990

l0001397F:
	movb	#64,-2A(fp)
	brb	00013994
00013986                   D5 50                               .P        

l00013988:
	movb	#2D,-2A(fp)
	brb	00013994
0001398E                                           D5 50               .P

l00013990:
	movb	#3F,-2A(fp)

l00013994:
	movzwl	+0E(sp),r7
	bbc	#00000007,r7,000139A4

l0001399C:
	movb	#68,r3
	brb	000139A7
000139A2       D5 50                                       .P            

l000139A4:
	movb	#2D,r3

l000139A7:
	movb	r3,-29(fp)
	bbc	#00000006,r7,000139B8

l000139AF:
	movb	#73,r4
	brb	000139BB
000139B5                D5 50 01                              .P.        

l000139B8:
	movb	#2D,r4

l000139BB:
	movb	r4,-28(fp)
	bbc	#00000005,r7,000139CC

l000139C3:
	movb	#70,r3
	brb	000139CF
000139C9                            D5 50 01                      .P.    

l000139CC:
	movb	#2D,r3

l000139CF:
	movb	r3,-27(fp)
	bbc	#00000004,r7,000139E0

l000139D7:
	movb	#61,r4
	brb	000139E3
000139DD                                        D5 50 01              .P.

l000139E0:
	movb	#2D,r4

l000139E3:
	movb	r4,-26(fp)
	bbc	#00000003,r7,000139F4

l000139EB:
	movb	#72,r3
	brb	000139F7
000139F1    D5 50 01                                      .P.            

l000139F4:
	movb	#2D,r3

l000139F7:
	movb	r3,-25(fp)
	bbc	#00000002,r7,00013A08

l000139FF:
	movb	#77,r4
	brb	00013A0B
00013A05                D5 50 01                              .P.        

l00013A08:
	movb	#2D,r4

l00013A0B:
	movb	r4,-24(fp)
	bbc	#00000001,r7,00013A1C

l00013A13:
	movb	#65,r3
	brb	00013A1F
00013A19                            D5 50 01                      .P.    

l00013A1C:
	movb	#2D,r3

l00013A1F:
	movb	r3,-23(fp)
	blbc	r7,00013A2C

l00013A26:
	movb	#64,r4
	brb	00013A2F

l00013A2C:
	movb	#2D,r4

l00013A2F:
	movb	r4,-22(fp)
	clrb	-21(fp)
	pushl	#00000000
	pushal	-2A(fp)
	pushl	r7
	pushal	+0BFB(r6)
	pushab	+05EB(r5)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brw	00013C9E
00013A61    D5 50 01                                      .P.            

l00013A64:
	tstl	r9
	bneq	00013A6B

l00013A68:
	brw	00013C70

l00013A6B:
	cmpl	r9,#00000006
	bneq	00013A73

l00013A70:
	brw	00013C70

l00013A73:
	cmpl	r9,#0000000B
	bneq	00013A7B

l00013A78:
	brw	00013C70

l00013A7B:
	cmpl	r9,#0000000E
	bneq	00013A83

l00013A80:
	brw	00013C70

l00013A83:
	cmpl	r9,#0000000D
	bneq	00013A8B

l00013A88:
	brw	00013C70

l00013A8B:
	cmpl	r9,#00000004
	bneq	00013A93

l00013A90:
	brw	00013C70

l00013A93:
	cmpl	r9,#0000000F
	bneq	00013A9B

l00013A98:
	brw	00013C70

l00013A9B:
	movzwl	+0E(sp),r2
	bicl2	#FFFF0FFF,r2
	cmpl	r2,#00001000
	blss	00013B2C

l00013AAF:
	beql	00013B1C

l00013AB1:
	cmpl	r2,#00002000
	blss	00013B2C

l00013ABA:
	beql	00013B14

l00013ABC:
	cmpl	r2,#00004000
	blss	00013B2C

l00013AC5:
	beql	00013AF4

l00013AC7:
	cmpl	r2,#00006000
	blss	00013B2C

l00013AD0:
	beql	00013B0C

l00013AD2:
	cmpl	r2,#00008000
	blss	00013B2C

l00013ADB:
	beql	00013AFC

l00013ADD:
	cmpl	r2,#0000A000
	blss	00013B2C

l00013AE6:
	beql	00013B04

l00013AE8:
	cmpl	r2,#0000C000
	beql	00013B24

l00013AF1:
	brb	00013B2C
00013AF3          01                                        .            

l00013AF4:
	movb	#64,-2A(fp)
	brb	00013B30
00013AFB                                  01                        .    

l00013AFC:
	movb	#2D,-2A(fp)
	brb	00013B30
00013B02       D5 50                                       .P            

l00013B04:
	movb	#6C,-2A(fp)
	brb	00013B30
00013B0B                                  01                        .    

l00013B0C:
	movb	#62,-2A(fp)
	brb	00013B30
00013B13          01                                        .            

l00013B14:
	movb	#63,-2A(fp)
	brb	00013B30
00013B1B                                  01                        .    

l00013B1C:
	movb	#70,-2A(fp)
	brb	00013B30
00013B23          01                                        .            

l00013B24:
	movb	#73,-2A(fp)
	brb	00013B30
00013B2B                                  01                        .    

l00013B2C:
	movb	#3F,-2A(fp)

l00013B30:
	movzwl	+0E(sp),r7
	bbc	#00000008,r7,00013B40

l00013B38:
	movb	#72,r3
	brb	00013B43
00013B3E                                           D5 50               .P

l00013B40:
	movb	#2D,r3

l00013B43:
	movb	r3,-29(fp)
	bbc	#00000005,r7,00013B54

l00013B4B:
	movb	#72,r4
	brb	00013B57
00013B51    D5 50 01                                      .P.            

l00013B54:
	movb	#2D,r4

l00013B57:
	movb	r4,-26(fp)
	bbc	#00000002,r7,00013B68

l00013B5F:
	movb	#72,r3
	brb	00013B6B
00013B65                D5 50 01                              .P.        

l00013B68:
	movb	#2D,r3

l00013B6B:
	movb	r3,-23(fp)
	bbc	#00000007,r7,00013B7C

l00013B73:
	movb	#77,r4
	brb	00013B7F
00013B79                            D5 50 01                      .P.    

l00013B7C:
	movb	#2D,r4

l00013B7F:
	movb	r4,-28(fp)
	bbc	#00000004,r7,00013B90

l00013B87:
	movb	#77,r3
	brb	00013B93
00013B8D                                        D5 50 01              .P.

l00013B90:
	movb	#2D,r3

l00013B93:
	movb	r3,-25(fp)
	bbc	#00000001,r7,00013BA4

l00013B9B:
	movb	#77,r4
	brb	00013BA7
00013BA1    D5 50 01                                      .P.            

l00013BA4:
	movb	#2D,r4

l00013BA7:
	movb	r4,-22(fp)
	bbc	#00000006,r7,00013BC8

l00013BAF:
	bbc	#0000000B,r7,00013BBC

l00013BB3:
	movb	#73,r2
	brb	00013BC0
00013BB9                            D5 50 01                      .P.    

l00013BBC:
	movb	#78,r2

l00013BC0:
	movb	r2,-27(fp)
	brb	00013BDB
00013BC6                   D5 50                               .P        

l00013BC8:
	bbc	#0000000B,r7,00013BD4

l00013BCC:
	movb	#53,r2
	brb	00013BD7
00013BD2       D5 50                                       .P            

l00013BD4:
	movb	#2D,r2

l00013BD7:
	movb	r2,-27(fp)

l00013BDB:
	movzwl	+0E(sp),r3
	bbc	#00000003,r3,00013BFC

l00013BE3:
	bbc	#0000000A,r3,00013BF0

l00013BE7:
	movb	#73,r2
	brb	00013BF4
00013BED                                        D5 50 01              .P.

l00013BF0:
	movb	#78,r2

l00013BF4:
	movb	r2,-24(fp)
	brb	00013C0F
00013BFA                               D5 50                       .P    

l00013BFC:
	bbc	#0000000A,r3,00013C08

l00013C00:
	movb	#6C,r2
	brb	00013C0B
00013C06                   D5 50                               .P        

l00013C08:
	movb	#2D,r2

l00013C0B:
	movb	r2,-24(fp)

l00013C0F:
	movzwl	+0E(sp),r3
	blbc	r3,00013C2C

l00013C16:
	bbc	#00000009,r3,00013C20

l00013C1A:
	movb	#74,r2
	brb	00013C24

l00013C20:
	movb	#78,r2

l00013C24:
	movb	r2,-21(fp)
	brb	00013C3F
00013C2A                               D5 50                       .P    

l00013C2C:
	bbc	#00000009,r3,00013C38

l00013C30:
	movb	#54,r2
	brb	00013C3B
00013C36                   D5 50                               .P        

l00013C38:
	movb	#2D,r2

l00013C3B:
	movb	r2,-21(fp)

l00013C3F:
	clrb	-20(fp)
	pushl	#00000000
	pushal	-2A(fp)
	movzwl	+16(sp),-(sp)
	pushal	+0C31(r6)
	pushab	+05EB(r5)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brb	00013C9E
00013C6E                                           D5 50               .P

l00013C70:
	pushl	#00000000
	extzv	#00000008,#18,+0000C67A(r5),r2
	pushl	r2
	pushal	+0C67(r6)
	pushab	+05EB(r5)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)

l00013C9E:
	bicw3	#FF00,+0000C67A(r5),+0E(sp)
	movzwl	+0E(sp),r3
	bneq	00013CD8

l00013CAF:
	pushl	#00000000
	pushl	r3
	pushal	+0CA6(r6)
	pushab	+05EB(r5)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brw	00013D92
00013CD7                      01                                .        

l00013CD8:
	cmpl	r3,#00000001
	bneq	00013D08

l00013CDD:
	pushl	#00000000
	pushl	r3
	pushal	+0CE2(r6)
	pushab	+05EB(r5)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brw	00013D92
00013D05                D5 50 01                              .P.        

l00013D08:
	pushl	#00000000
	bbc	#00000005,r3,00013D14

l00013D0E:
	moval	+65(r8),r10
	brb	00013D17

l00013D14:
	moval	(r6),r10

l00013D17:
	pushl	r10
	bbc	#00000004,r3,00013D24

l00013D1D:
	moval	+60(r8),r9
	brb	00013D27
00013D23          01                                        .            

l00013D24:
	moval	(r6),r9

l00013D27:
	pushl	r9
	bbc	#00000003,r3,00013D34

l00013D2D:
	moval	+5B(r8),r7
	brb	00013D37
00013D33          01                                        .            

l00013D34:
	moval	(r6),r7

l00013D37:
	pushl	r7
	bbc	#00000002,r3,00013D44

l00013D3D:
	moval	+56(r8),r4
	brb	00013D47
00013D43          01                                        .            

l00013D44:
	moval	(r6),r4

l00013D47:
	pushl	r4
	bbc	#00000001,r3,00013D54

l00013D4D:
	moval	+51(r8),-3E(fp)
	brb	00013D58

l00013D54:
	moval	(r6),-3E(fp)

l00013D58:
	pushl	-3E(fp)
	blbc	r3,00013D68

l00013D5E:
	moval	+4C(r8),-42(fp)
	brb	00013D6C
00013D65                D5 50 01                              .P.        

l00013D68:
	moval	(r6),-42(fp)

l00013D6C:
	pushl	-42(fp)
	pushl	r3
	pushal	+0D23(r6)
	pushab	+05EB(r5)
	calls	#09,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)

l00013D92:
	movzwl	+0000C672(r5),r2
	bneq	00013D9E

l00013D9B:
	brw	000146B0

l00013D9E:
	movl	+0000C627(r5),r7
	movw	+0000C672(r5),+2A(sp)
	cmpl	+08(sp),#00000001
	bleq	00013DB8

l00013DB3:
	movl	+04(sp),r0
	ret

l00013DB8:
	tstl	+0000C627(r5)
	bneq	00013DC4

l00013DC0:
	movl	#00000002,r0
	ret

l00013DC4:
	pushl	#00000000
	pushal	+0DE8(r6)
	pushab	+05EB(r5)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movzwl	+2A(sp),r2
	cmpl	r2,#00000004
	bgequ	00013DF3

l00013DF0:
	brw	0001469C

l00013DF3:
	movab	0000E258,+20(sp)
	movab	@000192D0,r9
	movab	0000E274,+1C(sp)
	movab	@000192D8,+2C(sp)
	movab	@000192BC,+0018(sp)
	movab	@000192C4,+0014(sp)

l00013E24:
	pushab	(r7)
	calls	#01,@+24(sp)
	movw	r0,+12(sp)
	pushab	+02(r7)
	calls	#01,@+24(sp)
	movw	r0,+28(sp)
	addl2	#00000004,r7
	movzwl	+2A(sp),r0
	subl2	#00000004,r0
	cvtlw	r0,+2A(sp)
	cmpw	+28(sp),+2A(sp)
	blequ	00013E81

l00013E4E:
	movzwl	#0421,-(sp)
	movzwl	+2E(sp),-(sp)
	movzwl	+30(sp),-(sp)
	movzwl	+1E(sp),-(sp)
	pushal	+0D67(r6)
	pushab	+05EB(r5)
	calls	#05,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movw	+2A(sp),+28(sp)

l00013E81:
	movzwl	+12(sp),r2
	cmpl	r2,#00000007
	bgeq	00013E8D

l00013E8A:
	brw	00014080

l00013E8D:
	movl	r2,r0
	casel	r0,#00000007,#00000006
00013E94             38 01 EC 01 40 01 EC 01 EC 01 58 01     8...@.....X.

l00013EA0:
	cvtdb	#0.5625,#11
	cvtps	#0017,40012700,@-2EB0(r5),r0
	caseb	+0007(r8),#00,#18
	bpt
	brw	00014080
	bneq	00013EBD
	brw	00014030
	cmpl	r0,#00002605
	bgeq	00013EC9
	brw	00014080
	bneq	00013ECE
	brw	00014038
	cmpl	r0,#00004341
	bgeq	00013EDA
	brw	00014080
	bneq	00013EDF
	brw	00014068
	cmpl	r0,#00004453
	bgeq	00013EEB
	brw	00014080
	bneq	00013EF0
	brw	00013FE4
	cmpl	r0,#00004704
	bgeq	00013EFC
	brw	00014080
	casel	r0,#00004704,#0000000B
	movzwl	#0001,-(ap)
	nop
	Invalid
	Invalid
	Invalid
	Invalid
	Invalid
	Invalid
	Invalid
	Invalid
	Invalid
	Invalid
	Invalid
	bpt
	brw	00014080
	bneq	00013F39
	brw	00014070
	cmpl	r0,#00004C41
	bgeq	00013F45
	brw	00014080
	bneq	00013F4A
	brw	00013FDC
	cmpl	r0,#00004D49
	bgeq	00013F56
	brw	00014080
	bneq	00013F5B
	brw	00013FF4
	cmpl	r0,#00005356
	bgeq	00013F67
	brw	00014080
	bneq	00013F6C
	brw	00014060
	cmpl	r0,#00005455
	bgeq	00013F78
	brw	00014080
	bneq	00013F7D
	brw	00014028
	cmpl	r0,#00005855
	bgeq	00013F89
	brw	00014080
	beql	00014004
	cmpl	r0,#00006542
	bgeq	00013F97
	brw	00014080
	bneq	00013F9C
	brw	00014050
	cmpl	r0,#0000756E
	bgeq	00013FA8
	brw	00014080
	bneq	00013FAD
	brw	00014078
	cmpl	r0,#00007855
	bgeq	00013FB9
	brw	00014080
	beql	0001401C
	cmpl	r0,#0000FB4A
	bneq	00013FC7
	brw	00014058
	brw	00014080
	tstl	r0
	moval	+0E4D(r6),r3
	brw	00014085
	moval	+0E57(r6),r3
	brw	00014085
	moval	+0EE7(r6),r3
	brw	00014085
	moval	+0EF0(r6),r3
	brw	00014085
	moval	+0E5C(r6),r3
	brw	00014085
	moval	+0E73(r6),r3
	brw	00014085
	moval	+0E67(r6),r3
	brw	00014085
	moval	+0E80(r6),r3
	movzbl	+0000C659(r5),r0
	cmpl	r0,#00000003
	bneq	00014085
	addl2	#00000004,@+04(ap)
	brb	00014085
	nop
	moval	+0E99(r6),r3
	addl2	#00000004,@+04(ap)
	brb	00014085
	nop
	moval	+0EA6(r6),r3
	brb	00014085
	nop
	moval	+0EB5(r6),r3
	brb	00014085
	nop
	moval	+0ECC(r6),r3
	brb	00014085
	nop
	moval	+0EDC(r6),r3
	brb	00014085
	nop
	moval	+0EE3(r6),r3
	brb	00014085
	nop
	moval	+0F04(r6),r3
	brb	00014085
	nop
	moval	+0F09(r6),r3
	brb	00014085
	nop
	moval	+0F12(r6),r3
	brb	00014085
	nop
	moval	+0F19(r6),r3
	brb	00014085
	nop
	moval	+0F27(r6),r3
	brb	00014085
	nop
	moval	+0F37(r6),r3
	brb	00014085
	nop

l00014080:
	moval	+0F40(r6),r3
	pushl	#00000000
	movzwl	+2C(sp),-(sp)
	pushl	r3
	movzwl	+1E(sp),r4
	pushl	r4
	pushal	+0E17(r6)
	pushab	+05EB(r5)
	calls	#05,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movl	r4,r2
	cmpl	r2,#00000009
	bgeq	000140BB

l000140B8:
	brw	000145DC

l000140BB:
	bneq	000140C0

l000140BD:
	brw	00014154

l000140C0:
	cmpl	r2,#00002605
	bgeq	000140CC

l000140C9:
	brw	000145DC

l000140CC:
	bneq	000140D1

l000140CE:
	brw	00014404

l000140D1:
	cmpl	r2,#00004453
	bgeq	000140DD

l000140DA:
	brw	000145DC

l000140DD:
	bneq	000140E2

l000140DF:
	brw	000141A0

l000140E2:
	cmpl	r2,#00004B46
	bgeq	000140EE

l000140EB:
	brw	000145DC

l000140EE:
	bneq	000140F3

l000140F0:
	brw	00014588

l000140F3:
	cmpl	r2,#00004C41
	bgeq	000140FF

l000140FC:
	brw	000145DC

l000140FF:
	beql	00014154

l00014101:
	cmpl	r2,#00004D49
	bgeq	0001410D

l0001410A:
	brw	000145DC

l0001410D:
	bneq	00014112

l0001410F:
	brw	000141DC

l00014112:
	cmpl	r2,#00005356
	bgeq	0001411E

l0001411B:
	brw	000145DC

l0001411E:
	bneq	00014123

l00014120:
	brw	00014544

l00014123:
	cmpl	r2,#00005455
	bgeq	0001412F

l0001412C:
	brw	000145DC

l0001412F:
	bneq	00014134

l00014131:
	brw	0001432C

l00014134:
	cmpl	r2,#00006542
	bgeq	00014140

l0001413D:
	brw	000145DC

l00014140:
	bneq	00014145

l00014142:
	brw	000144AC

l00014145:
	cmpl	r2,#0000FB4A
	bneq	00014151

l0001414E:
	brw	00014504

l00014151:
	brw	000145DC

l00014154:
	movzwl	+28(sp),r0
	cmpl	r0,#00000004
	bgequ	00014160

l0001415D:
	brw	0001466A

l00014160:
	movzwl	+12(sp),r0
	cmpl	r0,#00000009
	bneq	00014170

l00014169:
	moval	+0F48(r6),r3
	brb	00014175

l00014170:
	moval	+1054(r6),r3

l00014175:
	pushl	#00000000
	pushl	r7
	calls	#01,@+24(sp)
	pushl	r0
	pushl	r3
	pushab	+05EB(r5)
	calls	#03,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	clrl	@+04(ap)
	brw	0001466A

l000141A0:
	movzwl	+28(sp),r0
	cmpl	r0,#00000004
	bgequ	000141AC

l000141A9:
	brw	0001466A

l000141AC:
	pushl	#00000000
	pushl	r7
	calls	#01,@+24(sp)
	pushl	r0
	pushal	+10A1(r6)
	pushab	+05EB(r5)
	calls	#03,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	clrl	@+04(ap)
	brw	0001466A
000141D9                            D5 50 01                      .P.    

l000141DC:
	movzwl	+28(sp),r10
	cmpl	r10,#00000008
	bgequ	000141E8

l000141E5:
	brw	0001466A

l000141E8:
	addl3	#00000004,r7,-(sp)
	calls	#01,@+24(sp)
	movzwl	r0,r2
	bicl3	#FFFFFFF8,r2,r4
	moval	-4A(fp),r2
	clrb	(r2)
	cmpl	r4,#00000003
	bleq	00014209

l00014206:
	movl	#00000003,r4

l00014209:
	pushl	#00000004
	pushal	+69(r8)
	pushl	r7
	calls	#03,@+38(sp)
	tstl	r0
	bneq	00014220

l00014218:
	moval	+6E(r8),r3
	brw	000142F5
0001421F                                              01                .

l00014220:
	pushl	#00000004
	pushal	+72(r8)
	pushl	r7
	calls	#03,@+38(sp)
	tstl	r0
	bneq	00014238

l0001422F:
	moval	+77(r8),r3
	brw	000142F5
00014236                   D5 50                               .P        

l00014238:
	pushl	#00000004
	pushal	+7E(r8)
	pushl	r7
	calls	#03,@+38(sp)
	tstl	r0
	bneq	00014250

l00014247:
	moval	+0083(r8),r3
	brw	000142F5
0001424F                                              01                .

l00014250:
	pushl	#00000004
	pushal	+008A(r8)
	pushl	r7
	calls	#03,@+38(sp)
	tstl	r0
	bneq	00014268

l00014260:
	moval	+008F(r8),r3
	brw	000142F5

l00014268:
	pushl	#00000004
	pushal	+0096(r8)
	pushl	r7
	calls	#03,@+38(sp)
	tstl	r0
	bneq	00014280

l00014278:
	moval	+009B(r8),r3
	brb	000142F5
0001427F                                              01                .

l00014280:
	pushl	#00000004
	pushal	+00A2(r8)
	pushl	r7
	calls	#03,@+38(sp)
	tstl	r0
	bneq	00014298

l00014290:
	moval	+00A7(r8),r3
	brb	000142F5
00014297                      01                                .        

l00014298:
	pushl	#00000004
	pushal	+00AE(r8)
	pushl	r7
	calls	#03,@+38(sp)
	tstl	r0
	bneq	000142B0

l000142A8:
	moval	+00B3(r8),r3
	brb	000142F5
000142AF                                              01                .

l000142B0:
	pushl	#00000004
	pushal	+00BA(r8)
	pushl	r7
	calls	#03,@+38(sp)
	tstl	r0
	bneq	000142F0

l000142C0:
	moval	+00BF(r8),r3
	cmpl	r10,#00000010
	blssu	000142F5

l000142CA:
	movb	#20,-4A(fp)
	movb	#28,-49(fp)
	pushl	#00000004
	addl3	#0000000C,r7,-(sp)
	moval	-4A(fp),r2
	addl3	#00000002,r2,-(sp)
	calls	#03,@000192C0
	movb	#29,-44(fp)
	clrb	-43(fp)
	brb	000142F5

l000142F0:
	moval	+00C7(r8),r3

l000142F5:
	pushl	#00000000
	pushal	-4A(fp)
	pushl	r3
	addl3	#00000006,r7,-(sp)
	calls	#01,@+30(sp)
	movzwl	r0,-(sp)
	pushl	+1044(r6)[r4]
	pushal	+0FCB(r6)
	pushab	+05EB(r5)
	calls	#06,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brw	0001466A

l0001432C:
	movzwl	+28(sp),r0
	cmpl	r0,#00000001
	bgequ	00014338

l00014335:
	brw	0001466A

l00014338:
	clrl	r4
	moval	-009A(fp),r2
	clrb	(r2)
	movzbl	(r7),r2
	blbc	r2,00014355

l00014347:
	pushal	+111C(r6)
	pushal	-009A(fp)
	calls	#02,@+20(sp)
	incl	r4

l00014355:
	movzbl	(r7),r2
	bbc	#00000001,r2,0001438D

l0001435C:
	pushal	-009A(fp)
	calls	#01,@+18(sp)
	movl	r0,r3
	tstl	r4
	beql	00014376

l0001436B:
	movl	r3,r2
	incl	r3
	movb	#2F,-009A(fp)[r2]

l00014376:
	pushal	+1129(r6)
	moval	-009A(fp),r2
	addl3	r3,r2,-(sp)
	calls	#02,@+20(sp)
	incl	r4
	addl2	#00000004,@+04(ap)

l0001438D:
	movzbl	(r7),r2
	bbc	#00000002,r2,000143C5

l00014394:
	pushal	-009A(fp)
	calls	#01,@+18(sp)
	movl	r0,r3
	tstl	r4
	beql	000143AE

l000143A3:
	movl	r3,r2
	incl	r3
	movb	#2F,-009A(fp)[r2]

l000143AE:
	pushal	+1130(r6)
	moval	-009A(fp),r2
	addl3	r3,r2,-(sp)
	calls	#02,@+20(sp)
	incl	r4
	addl2	#00000004,@+04(ap)

l000143C5:
	tstl	r4
	bgtr	000143CC

l000143C9:
	brw	0001466A

l000143CC:
	pushl	#00000000
	cmpl	r4,#00000001
	bneq	000143D8

l000143D3:
	moval	(r6),r3
	brb	000143DD

l000143D8:
	moval	+00CF(r8),r3

l000143DD:
	pushl	r3
	pushal	-009A(fp)
	pushal	+10EA(r6)
	pushab	+05EB(r5)
	calls	#04,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brw	0001466A
00014403          01                                        .            

l00014404:
	movzwl	+28(sp),r3
	cmpl	r3,#00000005
	bgequ	00014410

l0001440D:
	brw	0001466A

l00014410:
	pushl	#00000004
	pushal	+00D1(r8)
	pushl	r7
	calls	#03,@+38(sp)
	tstl	r0
	beql	00014423

l00014420:
	brw	0001466A

l00014423:
	movzbl	+04(r7),r4
	addl3	#0000000D,r4,r2
	cmpl	r3,r2
	bgequ	00014433

l00014430:
	brw	0001466A

l00014433:
	addl3	#00000005,r4,r3
	movzbl	(r7)[r3],r0
	cvtlb	r0,-009B(fp)
	clrb	(r7)[r3]
	pushl	#00000000
	addl3	#0000000C,r4,r2
	movzbl	(r7)[r2],-(sp)
	addl3	#0000000B,r4,r2
	movzbl	(r7)[r2],-(sp)
	addl3	#0000000A,r4,r2
	movzbl	(r7)[r2],-(sp)
	addl3	#00000009,r4,r2
	movzbl	(r7)[r2],-(sp)
	addl3	#00000008,r4,r2
	movzbl	(r7)[r2],-(sp)
	addl3	#00000007,r4,r2
	movzbl	(r7)[r2],-(sp)
	addl2	#00000006,r4
	movzbl	(r7)[r4],-(sp)
	cvtbl	-009B(fp),-(sp)
	addl3	#00000005,r7,-(sp)
	pushal	+1139(r6)
	pushab	+05EB(r5)
	calls	#0B,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movb	-009B(fp),(r7)[r3]
	brw	0001466A
000144AB                                  01                        .    

l000144AC:
	movzwl	+28(sp),r0
	cmpl	r0,#00000005
	bgequ	000144B8

l000144B5:
	brw	0001466A

l000144B8:
	pushl	#00000000
	movzbl	+04(r7),r2
	blbc	r2,000144C8

l000144C1:
	moval	+00D6(r8),r3
	brb	000144CB

l000144C8:
	moval	(r6),r3

l000144CB:
	pushl	r3
	pushl	r7
	calls	#01,@+28(sp)
	pushl	r0
	pushal	+11A0(r6)
	pushab	+05EB(r5)
	calls	#04,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movzbl	+04(r7),r2
	blbc	r2,000144FC

l000144F9:
	brw	0001466A

l000144FC:
	clrl	@+04(ap)
	brw	0001466A
00014502       D5 50                                       .P            

l00014504:
	movzwl	+28(sp),r0
	cmpl	r0,#00000004
	bgequ	00014510

l0001450D:
	brw	0001466A

l00014510:
	pushl	#00000000
	movzbl	+03(r7),-(sp)
	movzbl	+02(r7),-(sp)
	movzbl	+01(r7),-(sp)
	movzbl	(r7),-(sp)
	pushal	+11EF(r6)
	pushab	+05EB(r5)
	calls	#06,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brw	0001466A
00014541    D5 50 01                                      .P.            

l00014544:
	movzwl	+28(sp),r0
	cmpl	r0,#00000005
	bgequ	00014550

l0001454D:
	brw	0001466A

l00014550:
	pushl	#00000000
	movzbl	+04(r7),r3
	emul	#00000000,#00000000,r3,r0
	ediv	#0000000A,r0,r1,r0
	pushl	r0
	divl3	#0000000A,r3,-(sp)
	pushal	+1220(r6)
	pushab	+05EB(r5)
	calls	#04,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brw	0001466A
	tstl	r0

l00014588:
	movzwl	+28(sp),r0
	cmpl	r0,#00000013
	blssu	000145DC

l00014591:
	clrl	r2
	nop

l00014594:
	subl3	r2,#0000000F,r0
	movzbl	(r7)[r0],-(sp)
	pushal	+00D9(r8)
	ashl	#01,r2,r0
	pushab	-00BC(fp)[r0]
	calls	#03,(r9)
	aobleq	#0000000F,r2,00014594

l000145B0:
	clrb	-009C(fp)
	pushl	#00000000
	pushal	-00BC(fp)
	pushal	+124F(r6)
	pushab	+05EB(r5)
	calls	#03,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brw	0001466A
000145DA                               D5 50                       .P    

l000145DC:
	movzwl	+28(sp),r2
	bneq	000145E5

l000145E2:
	brw	0001466A

l000145E5:
	cmpl	r2,#00000018
	bgtru	00014610

l000145EA:
	pushl	#00000000
	pushal	+1290(r6)
	pushab	+05EB(r5)
	calls	#02,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movw	+28(sp),+26(sp)
	brb	00014633

l00014610:
	pushl	#00000000
	pushal	+1275(r6)
	pushab	+05EB(r5)
	calls	#02,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movw	#0014,+26(sp)

l00014633:
	clrw	r11
	tstw	+26(sp)
	beql	0001466A

l0001463A:
	tstl	r0

l0001463C:
	pushl	#00000000
	movzwl	r11,r0
	movzbl	(r7)[r0],-(sp)
	pushal	+1296(r6)
	pushab	+05EB(r5)
	calls	#03,(r9)
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r0
	calls	#04,(r0)
	incw	r11
	cmpw	r11,+26(sp)
	blssu	0001463C

l0001466A:
	pushl	#00000000
	pushl	#00000001
	pushal	+00DE(r8)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movzwl	+28(sp),r0
	addl2	r0,r7
	movzwl	+2A(sp),r2
	subl3	r0,r2,r0
	cvtlw	r0,+2A(sp)
	movzwl	r0,r0
	cmpl	r0,#00000004
	blssu	0001469C

l00014699:
	brw	00013E24

l0001469C:
	pushl	#00000000
	pushl	#00000001
	pushal	+00E0(r8)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)

l000146B0:
	bicl3	#FFFF3FFF,+0000C67A(r5),r2
	extzv	#0000000C,#14,r2,r2
	cvtlw	r2,+0E(sp)
	movzwl	r2,r9
	bbs	#00000003,r9,000146CF
	brw	00014776
	movzwl	+02(sp),r7
	cmpl	r7,#00000003
	beql	000146E2
	cmpl	r7,#00000006
	beql	000146E2
	cmpl	r7,#0000000B
	bneq	00014738
	pushl	#00000000
	movzwl	+12(sp),r3
	bbc	#00000002,r3,000146F4
	moval	+12EB(r6),r4
	brb	000146F9
	nop
	moval	+131A(r6),r4
	pushl	r4
	bicl2	#FFFFFFF3,r3
	pushl	r3
	pushal	+0E80(r6)
	movzwl	#5855,-(sp)
	pushal	+00E2(r8)
	pushal	+129C(r6)
	pushab	+05EB(r5)
	calls	#07,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	addl2	r3,@+04(ap)
	brb	00014776
	tstl	r7
	bneq	00014776
	bbs	#00000002,r9,00014776
	pushl	#00000000
	pushal	+131A(r6)
	pushl	#00000008
	pushal	+0E80(r6)
	movzwl	#5855,-(sp)
	pushal	+00E5(r8)
	pushal	+129C(r6)
	pushab	+05EB(r5)
	calls	#07,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	tstw	+0000C674(r5)
	bneq	000147A4
	pushl	#00000000
	pushal	+133D(r6)
	pushab	+05EB(r5)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	brb	00014808
	nop
	pushl	#00000000
	pushal	+135B(r6)
	pushab	+05EB(r5)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	pushl	#00000005
	movzwl	+0000C674(r5),-(sp)
	calls	#02,0000DC74
	tstl	r0
	beql	000147E5
	movl	r0,+08(sp)
	cmpl	r0,#00000001
	bleq	000147E5
	ret
	pushl	#00000000
	pushal	+13A8(r6)
	pushab	+05EB(r5)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r5)
	pushab	(r5)
	movl	+0000C839(r5),r2
	calls	#04,(r2)
	movl	+00000008(sp),r0
	ret
	xfc
	remque	+5E28(r2),@(sp)+

;; fn00014812: 00014812
fn00014812 proc
	subl2	#00000028,sp
	movab	FFFE6858,r5
	movab	FFFFB3D0,r4
	movab	FFFF9584,r8
	clrl	r11
	movzwl	+0000C65E(r4),r2
	cmpl	r2,#0000000B
	bgequ	00014844

l00014838:
	movw	+0000C65E(r4),r3
	brb	00014847
00014841    D5 50 01                                      .P.            

l00014844:
	movw	#000B,r3

l00014847:
	movw	r3,r6
	movl	+05E7(r4),r2
	movw	+0C(r2),r10
	movzbw	+0000C658(r4),r9
	movzwl	r6,ap
	mull3	#00000005,ap,r2
	pushab	+1519(r8)[r2]
	pushal	-25(fp)
	calls	#02,@000192BC
	cmpl	ap,#00000006
	bneq	000148AC

l00014875:
	movzwl	+0000C65C(r4),r2
	bbc	#00000001,r2,00014888

l00014880:
	movb	#38,r3
	brb	0001488B
00014885                D5 50 01                              .P.        

l00014888:
	movb	#34,r3

l0001488B:
	movb	r3,-24(fp)
	movzwl	+0000C65C(r4),r2
	bbc	#00000002,r2,000148A0

l0001489A:
	movb	#33,r2
	brb	000148A3
0001489F                                              01                .

l000148A0:
	movb	#32,r2

l000148A3:
	movb	r2,-22(fp)
	brb	000148EA
000148A9                            D5 50 01                      .P.    

l000148AC:
	cmpl	ap,#00000008
	bneq	000148D0

l000148B1:
	movzwl	+0000C65C(r4),r2
	ashl	#FF,r2,r2
	bicl2	#FFFFFFFC,r2
	movzwl	r2,r2
	movb	+14C8(r8)[r2],-22(fp)
	brb	000148EA

l000148D0:
	cmpl	ap,#0000000B
	blssu	000148EA

l000148D5:
	movzwl	+0000C65E(r4),-(sp)
	pushal	+00EC(r5)
	pushab	-24(fp)
	calls	#03,@000192D0

l000148EA:
	clrl	ap
	movab	-20(fp),r2

l000148F0:
	movb	#20,(r2)
	incl	r2
	aobleq	#0000000E,ap,000148F0

l000148F9:
	clrb	-11(fp)
	movzwl	+0000C67C(r4),r2
	bicw3	#0000,r2,r7
	movzwl	r10,r0
	casel	r0,#00000000,#0000000F
00014910 40 01 88 02 2C 00 B4 03 40 01 B4 03 40 01 B4 03 @...,...@...@...
00014920 B4 03 B4 03 B4 03 40 01 B4 03 40 01 40 01       ......@...@.@.  

l0001492E:
	Invalid
	cmpb	#03,#01
	jmp	40012700
	tstl	r0
	clrl	ap
	movab	-0010(fp),r2
	nop
	clrb	(r2)
	incl	r2
	aobleq	#0000000B,ap,00014944
	movzwl	r7,r3
	bbc	#00000008,r3,00014958
	movb	#52,-10(fp)
	bbc	#00000007,r3,00014966
	movb	#57,-0F(fp)
	movb	#44,-0D(fp)
	bbc	#00000006,r3,0001496F
	movb	#45,-0E(fp)
	bbc	#00000005,r3,00014978
	movb	#52,-0C(fp)
	bbc	#00000004,r3,00014986
	movb	#57,-0B(fp)
	movb	#44,-09(fp)
	bbc	#00000003,r3,0001498F
	movb	#45,-0A(fp)
	bbc	#00000002,r3,00014998
	movb	#52,-08(fp)
	bbc	#00000001,r3,000149A6
	movb	#57,-07(fp)
	movb	#44,-05(fp)
	blbc	r3,000149AE
	movb	#45,-06(fp)
	moval	-20(fp),r2
	clrl	r3
	clrl	r0
	cmpl	r3,#00000003
	bgeq	000149E3
	nop
	clrl	r1
	tstl	r0
	tstb	-10(fp)[r0]
	beql	000149CB
	movb	-10(fp)[r0],(r2)+
	incl	r1
	incl	r0
	cmpl	r1,#00000004
	blss	000149C0
	movl	r2,ap
	incl	r2
	movb	#2C,(ap)
	incl	r3
	cmpl	r3,#00000003
	blss	000149BC
	movb	#20,-(r2)
	moval	-20(fp),ap
	subl3	ap,r2,ap
	cmpl	ap,#0000000C
	blss	000149F6
	brw	00014EB9
	clrl	r1
	movzwl	r9,r0
	movzwl	#000A,r2
	beql	00014A18
	cmpl	r2,#00000001
	beql	00014A1D
	bgtr	00014A18
	cmpl	r2,r0
	beql	00014A1D
	bgtru	00014A11
	subl2	r2,r1
	addl2	r0,r1
	brb	00014A1D
	tstl	r0
	ediv	r2,r0,r0,r1
	movzwl	r1,-(sp)
	movzwl	r9,r2
	clrl	r1
	movl	r2,r0
	movl	#0000000A,r2
	bgeq	00014A38
	cmpl	r2,r0
	bgtru	00014A3D
	incl	r1
	brb	00014A3D
	tstl	r0
	ediv	r2,r0,r1,r0
	pushl	r1
	pushal	+00F1(r5)
	pushab	-14(fp)
	calls	#04,@000192D0
	brw	00014EB9
	bicw3	#FF00,+0000C67A(r4),r7
	clrl	r1
	movzwl	r9,r0
	movzwl	#000A,r2
	beql	00014A7C
	cmpl	r2,#00000001
	beql	00014A81
	bgtr	00014A7C
	cmpl	r2,r0
	beql	00014A81
	bgtru	00014A75
	subl2	r2,r1
	addl2	r0,r1
	brb	00014A81
	tstl	r0
	ediv	r2,r0,r0,r1
	movzwl	r1,-(sp)
	movzwl	r9,r2
	clrl	r1
	movl	r2,r0
	movl	#0000000A,r2
	bgeq	00014A9C
	cmpl	r2,r0
	bgtru	00014AA1
	incl	r1
	brb	00014AA1
	tstl	r0
	ediv	r2,r0,r1,r0
	pushl	r1
	pushal	+00F7(r5)
	pushal	-20(fp)
	calls	#04,@000192D0
	movzwl	r7,r0
	blbc	r0,00014ABC
	movb	#2D,r3
	brb	00014AC0
	movb	#77,r3
	movb	r3,-1E(fp)
	bbc	#00000001,r0,00014AD0
	movb	#68,ap
	brb	00014AD3
	tstl	r0
	movb	#2D,ap
	movb	ap,-1B(fp)
	bbc	#00000002,r0,00014AE4
	movb	#73,r3
	brb	00014AE7
	tstl	r0
	nop
	movb	#2D,r3
	movb	r3,-1A(fp)
	bbc	#00000005,r0,00014AF8
	movb	#61,ap
	brb	00014AFB
	tstl	r0
	nop
	movb	#2D,ap
	movb	ap,-1C(fp)
	bbc	#00000004,r0,00014B10
	movb	#64,-20(fp)
	movb	#78,-1D(fp)
	brb	00014B14
	nop
	movb	#2D,-20(fp)
	bbc	#00000003,r0,00014B20
	movb	#56,-20(fp)
	brw	00014EB9
	pushl	#0000002E
	pushab	+0000C701(r4)
	calls	#02,@000192DC
	movl	r0,r2
	bneq	00014B37
	brw	00014EB9
	incl	r2
	pushl	#00000003
	pushal	+0109(r5)
	pushl	r2
	movab	0000E2E8,r3
	calls	#03,(r3)
	tstl	r0
	beql	00014B8E
	pushl	#00000003
	pushal	+010D(r5)
	pushl	r2
	calls	#03,(r3)
	tstl	r0
	beql	00014B8E
	pushl	#00000003
	pushal	+0111(r5)
	pushl	r2
	calls	#03,(r3)
	tstl	r0
	beql	00014B8E
	pushl	#00000003
	pushal	+0115(r5)
	pushl	r2
	calls	#03,(r3)
	tstl	r0
	beql	00014B8E
	pushl	#00000003
	pushal	+0119(r5)
	pushl	r2
	calls	#03,(r3)
	tstl	r0
	beql	00014B8E
	brw	00014EB9
	movb	#78,-1D(fp)
	brw	00014EB9
	tstl	r0
	movzwl	r7,r2
	bicl2	#FFFFF3FF,r2
	cmpl	r2,#00000400
	blss	00014BC8
	beql	00014BC0
	cmpl	r2,#00000800
	bneq	00014BC8
	movb	#64,-20(fp)
	brb	00014BCC
	tstl	r0
	nop
	movb	#2D,-20(fp)
	brb	00014BCC
	tstl	r0
	movb	#3F,-20(fp)
	movzwl	r7,r0
	bbc	#00000007,r0,00014BDC
	movb	#68,r3
	brb	00014BDF
	tstl	r0
	nop
	movb	#2D,r3
	movb	r3,-1F(fp)
	bbc	#00000006,r0,00014BF0
	movb	#73,ap
	brb	00014BF3
	tstl	r0
	nop
	movb	#2D,ap
	movb	ap,-1E(fp)
	bbc	#00000005,r0,00014C04
	movb	#70,r3
	brb	00014C07
	tstl	r0
	nop
	movb	#2D,r3
	movb	r3,-1D(fp)
	bbc	#00000004,r0,00014C18
	movb	#61,ap
	brb	00014C1B
	tstl	r0
	nop
	movb	#2D,ap
	movb	ap,-1C(fp)
	bbc	#00000003,r0,00014C2C
	movb	#72,r3
	brb	00014C2F
	tstl	r0
	nop
	movb	#2D,r3
	movb	r3,-1B(fp)
	bbc	#00000002,r0,00014C40
	movb	#77,ap
	brb	00014C43
	tstl	r0
	nop
	movb	#2D,ap
	movb	ap,-1A(fp)
	bbc	#00000001,r0,00014C54
	movb	#65,r2
	brb	00014C57
	tstl	r0
	nop
	movb	#2D,r2
	movb	r2,-19(fp)
	blbc	r0,00014C64
	movb	#64,r3
	brb	00014C67
	movb	#2D,r3
	movb	r3,-18(fp)
	clrl	r1
	movzwl	r9,r0
	movzwl	#000A,r2
	beql	00014C8C
	cmpl	r2,#00000001
	beql	00014C91
	bgtr	00014C8C
	cmpl	r2,r0
	beql	00014C91
	bgtru	00014C86
	subl2	r2,r1
	addl2	r0,r1
	brb	00014C91
	nop
	ediv	r2,r0,r0,r1
	movzwl	r1,-(sp)
	movzwl	r9,r2
	clrl	r1
	movl	r2,r0
	movl	#0000000A,r2
	bgeq	00014CAC
	cmpl	r2,r0
	bgtru	00014CB1
	incl	r1
	brb	00014CB1
	tstl	r0
	ediv	r2,r0,r1,r0
	pushl	r1
	pushal	+011D(r5)
	pushab	-14(fp)
	calls	#04,@000192D0
	brw	00014EB9
	movzwl	r7,r2
	bicl2	#FFFF0FFF,r2
	cmpl	r2,#00001000
	blss	00014D54
	beql	00014D44
	cmpl	r2,#00002000
	blss	00014D54
	beql	00014D3C
	cmpl	r2,#00004000
	blss	00014D54
	beql	00014D1C
	cmpl	r2,#00006000
	blss	00014D54
	beql	00014D34
	cmpl	r2,#00008000
	blss	00014D54
	beql	00014D24
	cmpl	r2,#0000A000
	blss	00014D54
	beql	00014D2C
	cmpl	r2,#0000C000
	beql	00014D4C
	brb	00014D54
	nop
	movb	#64,-20(fp)
	brb	00014D58
	nop
	movb	#2D,-20(fp)
	brb	00014D58
	tstl	r0
	movb	#6C,-20(fp)
	brb	00014D58
	nop
	movb	#62,-20(fp)
	brb	00014D58
	nop
	movb	#63,-20(fp)
	brb	00014D58
	nop
	movb	#70,-20(fp)
	brb	00014D58
	nop
	movb	#73,-20(fp)
	brb	00014D58
	nop
	movb	#3F,-20(fp)
	movzwl	r7,r0
	bbc	#00000008,r0,00014D68
	movb	#72,r3
	brb	00014D6B
	tstl	r0
	nop
	movb	#2D,r3
	movb	r3,-1F(fp)
	bbc	#00000005,r0,00014D7C
	movb	#72,ap
	brb	00014D7F
	tstl	r0
	nop
	movb	#2D,ap
	movb	ap,-1C(fp)
	bbc	#00000002,r0,00014D90
	movb	#72,r3
	brb	00014D93
	tstl	r0
	nop
	movb	#2D,r3
	movb	r3,-19(fp)
	bbc	#00000007,r0,00014DA4
	movb	#77,ap
	brb	00014DA7
	tstl	r0
	nop
	movb	#2D,ap
	movb	ap,-1E(fp)
	bbc	#00000004,r0,00014DB8
	movb	#77,r3
	brb	00014DBB
	tstl	r0
	nop
	movb	#2D,r3
	movb	r3,-1B(fp)
	bbc	#00000001,r0,00014DCC
	movb	#77,ap
	brb	00014DCF
	tstl	r0
	nop
	movb	#2D,ap
	movb	ap,-18(fp)
	bbc	#00000006,r0,00014DF0
	bbc	#0000000B,r0,00014DE4
	movb	#73,r2
	brb	00014DE8
	tstl	r0
	nop
	movb	#78,r2
	movb	r2,-1D(fp)
	brb	00014E03
	tstl	r0
	bbc	#0000000B,r0,00014DFC
	movb	#53,r2
	brb	00014DFF
	tstl	r0
	movb	#2D,r2
	movb	r2,-1D(fp)
	movzwl	r7,r3
	bbc	#00000003,r3,00014E20
	bbc	#0000000A,r3,00014E14
	movb	#73,r2
	brb	00014E18
	movb	#78,r2
	movb	r2,-1A(fp)
	brb	00014E33
	tstl	r0
	bbc	#0000000A,r3,00014E2C
	movb	#53,r2
	brb	00014E2F
	tstl	r0
	movb	#2D,r2
	movb	r2,-1A(fp)
	movzwl	r7,r3
	blbc	r3,00014E50
	bbc	#00000009,r3,00014E44
	movb	#74,r2
	brb	00014E48
	nop
	movb	#78,r2
	movb	r2,-17(fp)
	brb	00014E63
	tstl	r0
	bbc	#00000009,r3,00014E5C
	movb	#54,r2
	brb	00014E5F
	tstl	r0
	movb	#2D,r2
	movb	r2,-17(fp)
	clrl	r1
	movzwl	r9,r0
	movzwl	#000A,r2
	beql	00014E84
	cmpl	r2,#00000001
	beql	00014E89
	bgtr	00014E84
	cmpl	r2,r0
	beql	00014E89
	bgtru	00014E7E
	subl2	r2,r1
	addl2	r0,r1
	brb	00014E89
	nop
	ediv	r2,r0,r0,r1
	movzwl	r1,-(sp)
	movzwl	r9,r2
	clrl	r1
	movl	r2,r0
	movl	#0000000A,r2
	bgeq	00014EA4
	cmpl	r2,r0
	bgtru	00014EA9
	incl	r1
	brb	00014EA9
	tstl	r0
	ediv	r2,r0,r1,r0
	pushl	r1
	pushal	+0123(r5)
	pushab	-14(fp)
	calls	#04,@000192D0
	pushl	#00000000
	pushl	+0000C66C(r4)
	movzwl	r10,r2
	ashl	#02,r2,r2
	pushab	+14CD(r8)[r2]
	pushal	-20(fp)
	pushal	+0129(r5)
	pushab	+05EB(r4)
	calls	#05,@000192D0
	pushl	r0
	pushab	+05EB(r4)
	pushab	(r4)
	movl	+0000C839(r4),r2
	calls	#04,(r2)
	pushl	#00000000
	movzwl	+0000C65C(r4),r2
	blbc	r2,00014F1C
	movzwl	+0000C678(r4),r2
	blbc	r2,00014F10
	movb	#54,r2
	brb	00014F14
	tstl	r0
	nop
	movb	#42,r2
	movb	r2,ap
	brb	00014F33
	tstl	r0
	nop
	movzwl	+0000C678(r4),r2
	blbc	r2,00014F2C
	movb	#74,r2
	brb	00014F30
	movb	#62,r2
	movb	r2,ap
	cvtbl	ap,-(sp)
	pushal	+0135(r5)
	pushab	+05EB(r4)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r4)
	pushab	(r4)
	movl	+0000C839(r4),r2
	calls	#04,(r2)
	movb	#01,r3
	tstw	+0000C672(r4)
	bneq	00014F7E
	bbc	#0000000F,+0000C67A(r4),00014F7C
	movzwl	r10,r2
	cmpl	r2,#00000003
	beql	00014F7E
	cmpl	r2,#00000006
	beql	00014F7E
	cmpl	r2,#0000000B
	beql	00014F7E
	clrb	r3
	cvtbl	r3,r2
	pushl	#00000000
	tstl	r2
	beql	00014FA4
	movzwl	+0000C65C(r4),r2
	bbc	#00000003,r2,00014F98
	movb	#58,r2
	brb	00014F9C
	movb	#78,r2
	movb	r2,r3
	brb	00014FBE
	tstl	r0
	nop
	movzwl	+0000C65C(r4),r2
	bbc	#00000003,r2,00014FB8
	movb	#6C,r2
	brb	00014FBB
	tstl	r0
	nop
	movb	#2D,r2
	movb	r2,r3
	cvtbl	r3,-(sp)
	pushal	+0138(r5)
	pushab	+05EB(r4)
	movab	@000192D0,ap
	calls	#03,(ap)
	pushl	r0
	pushab	+05EB(r4)
	pushab	(r4)
	movl	+0000C839(r4),r2
	calls	#04,(r2)
	cmpl	+24(r4),#00000004
	bneq	00015038
	movl	+0000C668(r4),r3
	movzwl	+0000C65C(r4),r2
	blbc	r2,00014FFF
	subl2	#0000000C,r3
	pushl	#00000000
	pushl	r3
	pushl	+0000C66C(r4)
	calls	#02,00017660
	addl2	#00000005,r0
	divl3	#0000000A,r0,-(sp)
	pushal	+013B(r5)
	pushab	+05EB(r4)
	calls	#03,(ap)
	pushl	r0
	pushab	+05EB(r4)
	pushab	(r4)
	movl	+0000C839(r4),r2
	calls	#04,(r2)
	brb	00015063
	tstl	r0
	cmpl	+24(r4),#00000005
	bneq	00015063
	pushl	#00000000
	pushl	+0000C668(r4)
	pushal	+0141(r5)
	pushab	+05EB(r4)
	calls	#03,(ap)
	pushl	r0
	pushab	+05EB(r4)
	pushab	(r4)
	movl	+0000C839(r4),r2
	calls	#04,(r2)
	pushl	#00000003
	movzwl	+0000C672(r4),-(sp)
	calls	#02,0000DC74
	movl	r0,r2
	beql	00015096
	tstl	+0000C627(r4)
	beql	00015093
	pushl	+0000C627(r4)
	calls	#01,@000192B0
	clrl	+0000C627(r4)
	movl	r2,r11
	pushl	#00000000
	pushal	-20(fp)
	pushl	#00000000
	pushab	+0000C660(r4)
	pushab	+0000C662(r4)
	calls	#04,00015104
	pushl	r0
	pushal	-25(fp)
	pushal	+0147(r5)
	pushab	+05EB(r4)
	calls	#04,@000192D0
	pushl	r0
	pushab	+05EB(r4)
	pushab	(r4)
	movl	+0000C839(r4),r2
	calls	#04,(r2)
	calls	#00,00017770
	tstw	+0000C674(r4)
	beql	000150FF
	pushl	#00000000
	movzwl	+0000C674(r4),-(sp)
	calls	#02,0000DC74
	tstl	r0
	beql	000150FF
	movl	r0,r11
	cmpl	r0,#00000001
	bleq	000150FF
	ret
	movl	r11,r0
	ret
	nop
	xfc
	remque	+5E08(r2),@(sp)+

;; fn00015106: 00015106
;;   Called from:
;;     000136C0 (in fn0001325E)
fn00015106 proc
	subl2	#00000008,sp
	movab	FFFFB3D0,r4
	movab	FFFF9584,r3
	movl	+04(ap),r0
	movzwl	(r0),r1
	movl	r1,r2
	ashl	#F7,r2,r0
	bicl2	#FFFFFF80,r0
	addl2	#00000050,r0
	cvtlw	r0,r11
	ashl	#FB,r2,r0
	bicl2	#FFFFFFF0,r0
	cvtlw	r0,r10
	bicw3	#FFE0,r1,r9
	movl	+08(ap),r0
	movzwl	(r0),r2
	movl	r2,r1
	ashl	#F5,r1,r0
	bicl2	#FFFFFFE0,r0
	cvtlw	r0,r8
	ashl	#FB,r1,r0
	bicl2	#FFFFFFC0,r0
	cvtlw	r0,r7
	bicl2	#FFFFFFE0,r2
	addw3	r2,r2,r6
	movzwl	r10,r2
	beql	00015189

l00015184:
	cmpl	r2,#0000000C
	blequ	000151A0

l00015189:
	pushl	r2
	pushal	+13F4(r3)
	pushal	-08(fp)
	calls	#03,@000192D0
	moval	-08(fp),r5
	brb	000151AC
0001519F                                              01                .

l000151A0:
	decl	r2
	ashl	#02,r2,r2
	movab	+1555(r3)[r2],r5

l000151AC:
	cmpl	+24(r4),#00000009
	bleq	000151DC

l000151B2:
	movzwl	r6,-(sp)
	movzwl	r7,-(sp)
	movzwl	r8,-(sp)
	movzwl	r9,-(sp)
	pushl	r5
	movzwl	r11,r0
	addl3	#0000076C,r0,-(sp)
	pushal	+140F(r3)
	pushl	+10(ap)
	calls	#08,@000192D0
	brb	0001524F
000151DB                                  01                        .    

l000151DC:
	tstl	+48(r4)
	beql	0001520C

l000151E1:
	movzwl	r6,-(sp)
	movzwl	r7,-(sp)
	movzwl	r8,-(sp)
	movzwl	r9,-(sp)
	movzwl	r10,-(sp)
	movzwl	r11,r0
	addl3	#0000076C,r0,-(sp)
	pushal	+1427(r3)
	pushl	+10(ap)
	calls	#08,@000192D0
	brb	0001524F
0001520B                                  01                        .    

l0001520C:
	movzwl	r7,-(sp)
	movzwl	r8,-(sp)
	clrl	r1
	movzwl	r11,r0
	movzwl	#0064,r2
	beql	00015234

l0001521E:
	cmpl	r2,#00000001
	beql	00015239

l00015223:
	bgtr	00015234

l00015225:
	cmpl	r2,r0
	beql	00015239

l0001522A:
	bgtru	0001522F

l0001522C:
	subl2	r2,r1

l0001522F:
	addl2	r0,r1
	brb	00015239

l00015234:
	ediv	r2,r0,r0,r1

l00015239:
	movzwl	r1,-(sp)
	pushl	r5
	movzwl	r9,-(sp)
	pushal	+13F9(r3)
	pushl	+10(ap)
	calls	#07,@000192D0

l0001524F:
	movl	+10(ap),r0
	ret
00015254             FC 00                                   ..          

;; fn00015256: 00015256
;;   Called from:
;;     00016804 (in fn000167AA)
;;     0001684F (in fn000167AA)
;;     000168A6 (in fn000167AA)
;;     000169B4 (in fn000167AA)
;;     000169FF (in fn000167AA)
fn00015256 proc
	subl2	#00000004,sp
	movab	FFFFB3D0,r2
	decl	+0000C5FB(r2)
	blss	0001527C

l00015268:
	movl	+0000C5F7(r2),r0
	incl	+0000C5F7(r2)
	movzbl	(r0),r5
	brb	00015286
0001527A                               D5 50                       .P    

l0001527C:
	calls	#00,0000D698
	movl	r0,r5

l00015286:
	addl3	#00000001,r5,r7
	clrl	r4
	decl	+0000C5FB(r2)
	blss	000152A8

l00015294:
	movl	+0000C5F7(r2),r0
	incl	+0000C5F7(r2)
	movzbl	(r0),r5
	brb	000152B2
000152A6                   D5 50                               .P        

l000152A8:
	calls	#00,0000D698
	movl	r0,r5

l000152B2:
	movl	r5,r3
	bicl3	#FFFFFFF0,r3,r6
	incl	r6
	bicl3	#FFFFFF0F,r3,r0
	extzv	#00000004,#1C,r0,r0
	addl3	#00000001,r0,r3
	addl3	r4,r3,r0
	cmpl	r0,+08(ap)
	blequ	000152DE
	movl	#00000004,r0
	ret
	movl	+0004(ap),r1
	nop
	movl	r4,r0
	incl	r4
	movl	r6,(r1)[r0]
	decl	r3
	bneq	000152E4
	decl	r7
	bneq	0001528C
	cmpl	r4,+08(ap)
	beql	00015300
	movb	#04,r3
	brb	00015302
	clrb	r3
	cvtbl	r3,r0
	ret
	tstl	r0
	xfc
	remque	+5E34(r2),@(sp)+

;; fn0001530A: 0001530A
;;   Called from:
;;     0001691C (in fn000167AA)
fn0001530A proc
	subl2	#00000034,sp
	movab	FFFFB3D0,r5
	clrl	r9
	clrq	r3
	movl	#00000001,+28(sp)
	moval	+10(ap),+18(sp)
	movl	@+18(sp),r6
	movzwl	00007CA4[r6],(sp)
	moval	+14(ap),+1C(sp)
	movl	@+1C(sp),r6
	movzwl	00007CA4[r6],+04(sp)
	moval	+18(ap),+20(sp)
	movl	@+20(sp),r6
	movzwl	00007CA4[r6],+08(sp)
	movl	+0088(r5),+0C(sp)
	bgtr	0001535C

l00015359:
	brw	00015851

l0001535C:
	movab	0000D698,r11
	movab	0000B070,+24(sp)
	movab	@0001927C,+14(sp)
	movab	@0001929C,+0010(sp)
	cmpl	r3,#00000001
	bgequ	000153B8

l00015381:
	tstl	r0
	nop

l00015384:
	decl	+0000C5FB(r5)
	blss	000153A0

l0001538C:
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	000153A6
0001539E                                           D5 50               .P

l000153A0:
	calls	#00,(r11)
	movl	r0,r7

l000153A6:
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,#00000001
	blssu	00015384

l000153B8:
	blbs	r4,000153BE

l000153BB:
	brw	000154E8

l000153BE:
	extzv	#00000001,#1F,r4,r4
	decl	r3
	decl	+0C(sp)
	cmpl	r3,@+18(sp)
	bgequ	00015405
	tstl	r0
	decl	+0000C5FB(r5)
	blss	000153EC
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	000153F2
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,@+18(sp)
	blssu	000153D0
	movl	r4,r6
	bicl3	r6,(sp),r6
	mull2	#00000006,r6
	addl3	+04(ap),r6,r8
	movzbl	(r8),r2
	movl	r2,r6
	cmpl	r6,#00000010
	bgtru	00015422
	brw	000154A8
	tstl	r0
	cmpl	r2,#00000063
	bneq	00015431
	movl	#00000001,r0
	ret
	movzbl	+01(r8),r1
	movl	r1,r6
	subl3	r6,#00000020,r0
	extzv	r6,r0,r4,r4
	subl2	r1,r3
	subl2	#00000010,r2
	cmpl	r3,r2
	bgequ	00015480
	decl	+0000C5FB(r5)
	blss	00015468
	movl	+0000C5F7(r5),r0
	incl	+0000C5F7(r5)
	movzbl	(r0),r6
	brb	0001546E
	tstl	r0
	calls	#00,(r11)
	movl	r0,r6
	movl	r6,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,r2
	blssu	0001544C
	mcoml	r4,r0
	movzwl	00007CA4[r2],r6
	mcoml	r0,r0
	bicl3	r0,r6,r0
	mull2	#00000006,r0
	addl3	+02(r8),r0,r8
	movzbl	(r8),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	000154A8
	brw	00015424
	movzbl	+01(r8),r0
	movl	r0,r7
	subl3	r7,#00000020,r6
	extzv	r7,r6,r4,r4
	subl2	r0,r3
	movl	r9,r6
	incl	r9
	movb	+02(r8),+05EB(r5)[r6]
	cmpl	r9,#00008000
	beql	000154D3
	brw	00015849
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r5)
	calls	#03,@+30(sp)
	clrl	+28(sp)
	clrl	r9
	brw	00015849
	nop

l000154E8:
	extzv	#00000001,#1F,r4,r4
	decl	r3
	cmpl	r3,#00000007
	bgequ	00015528
	decl	+0000C5FB(r5)
	blss	00015510
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	00015516
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,#00000007
	blssu	000154F4
	bicl3	#FFFFFF80,r4,r10
	extzv	#00000007,#19,r4,r4
	subl2	#00000007,r3
	cmpl	r3,@+20(sp)
	bgequ	00015575
	tstl	r0
	decl	+0000C5FB(r5)
	blss	0001555C
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	00015562
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,@+20(sp)
	blssu	00015540
	movl	r4,r6
	bicl3	r6,+08(sp),r6
	mull2	#00000006,r6
	addl3	+0C(ap),r6,r8
	movzbl	(r8),r2
	movl	r2,r6
	cmpl	r6,#00000010
	bgtru	00015593
	brw	00015618
	nop
	cmpl	r2,#00000063
	bneq	000155A1
	movl	#00000001,r0
	ret
	movzbl	+01(r8),r1
	movl	r1,r6
	subl3	r6,#00000020,r0
	extzv	r6,r0,r4,r4
	subl2	r1,r3
	subl2	#00000010,r2
	cmpl	r3,r2
	bgequ	000155F0
	decl	+0000C5FB(r5)
	blss	000155D8
	movl	+0000C5F7(r5),r0
	incl	+0000C5F7(r5)
	movzbl	(r0),r6
	brb	000155DE
	tstl	r0
	calls	#00,(r11)
	movl	r0,r6
	movl	r6,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,r2
	blssu	000155BC
	mcoml	r4,r0
	movzwl	00007CA4[r2],r6
	mcoml	r0,r0
	bicl3	r0,r6,r0
	mull2	#00000006,r0
	addl3	+02(r8),r0,r8
	movzbl	(r8),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	00015618
	brw	00015594
	movzbl	+01(r8),r0
	movl	r0,r7
	subl3	r7,#00000020,r6
	extzv	r7,r6,r4,r4
	subl2	r0,r3
	subl3	r10,r9,r7
	movzwl	+02(r8),r6
	subl3	r6,r7,r10
	cmpl	r3,@+1C(sp)
	bgequ	00015675
	tstl	r0
	nop
	decl	+0000C5FB(r5)
	blss	0001565C
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	00015662
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,@+1C(sp)
	blssu	00015640
	movl	r4,r6
	bicl3	r6,+04(sp),r6
	mull2	#00000006,r6
	addl3	+08(ap),r6,r8
	movzbl	(r8),r2
	movl	r2,r6
	cmpl	r6,#00000010
	bgtru	00015693
	brw	00015718
	nop
	cmpl	r2,#00000063
	bneq	000156A1
	movl	#00000001,r0
	ret
	movzbl	+01(r8),r1
	movl	r1,r6
	subl3	r6,#00000020,r0
	extzv	r6,r0,r4,r4
	subl2	r1,r3
	subl2	#00000010,r2
	cmpl	r3,r2
	bgequ	000156F0
	decl	+0000C5FB(r5)
	blss	000156D8
	movl	+0000C5F7(r5),r0
	incl	+0000C5F7(r5)
	movzbl	(r0),r6
	brb	000156DE
	tstl	r0
	calls	#00,(r11)
	movl	r0,r6
	movl	r6,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,r2
	blssu	000156BC
	mcoml	r4,r0
	movzwl	00007CA4[r2],r6
	mcoml	r0,r0
	bicl3	r0,r6,r0
	mull2	#00000006,r0
	addl3	+02(r8),r0,r8
	movzbl	(r8),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	00015718
	brw	00015694
	movzbl	+01(r8),r0
	movl	r0,r7
	subl3	r7,#00000020,r6
	extzv	r7,r6,r4,r4
	subl2	r0,r3
	movzwl	+02(r8),+2C(sp)
	tstl	r2
	beql	00015784
	cmpl	r3,#00000008
	bgequ	00015770
	tstl	r0
	nop
	decl	+0000C5FB(r5)
	blss	00015758
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	0001575E
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,#00000008
	blssu	0001573C
	bicl3	#FFFFFF00,r4,r6
	addl2	r6,+2C(sp)
	extzv	#00000008,#18,r4,r4
	subl2	#00000008,r3
	subl2	+2C(sp),+0000000C(sp)
	bicl2	#FFFF8000,r10
	movl	r10,r0
	cmpl	r0,r9
	blequ	000157A0
	movl	r10,r6
	brb	000157A3
	movl	r9,r6
	subl3	r6,#00008000,r2
	movl	r2,r0
	cmpl	r0,+2C(sp)
	blequ	000157BC
	movl	+2C(sp),r1
	brb	000157BF
	tstl	r0
	movl	r2,r1
	movl	r1,r2
	subl2	r2,+2C(sp)
	tstl	+28(sp)
	beql	000157EC
	cmpl	r9,r10
	bgtru	000157EC
	pushl	r2
	pushl	#00000000
	movab	+05EB(r5),r6
	addl3	r9,r6,-(sp)
	calls	#03,@+20(sp)
	addl2	r2,r9
	addl2	r2,r10
	brb	00015827
	tstl	r0
	nop
	subl3	r10,r9,r0
	cmpl	r0,r2
	blssu	00015810
	pushl	r2
	movab	+05EB(r5),r6
	addl3	r10,r6,-(sp)
	addl3	r9,r6,-(sp)
	calls	#03,@+1C(sp)
	addl2	r2,r9
	addl2	r2,r10
	brb	00015827
	movl	r9,r1
	incl	r9
	movl	r10,r0
	incl	r10
	movb	+05EB(r5)[r0],+05EB(r5)[r1]
	decl	r2
	bneq	00015810
	cmpl	r9,#00008000
	bneq	00015841
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r5)
	calls	#03,@+30(sp)
	clrl	+28(sp)
	clrl	r9
	tstl	+2C(sp)
	beql	00015849
	brw	0001578C
	tstl	+0C(sp)
	bleq	00015851
	brw	0001537C

l00015851:
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r5)
	calls	#03,0000B070
	addl3	+0084(r5),+0000C5FB(r5),r2
	extzv	#00000003,#1D,r3,r4
	addl2	r4,r2
	beql	0001588F
	subl3	+0084(r5),+0000C64C(r5),r2
	subl2	+0000C5FB(r5),r2
	subl3	r4,r2,+008C(r5)
	movl	#00000005,r0
	ret
	clrl	r0
	ret
	tstl	r0
	xfc
	remque	+5E34(r2),@(sp)+

;; fn00015896: 00015896
;;   Called from:
;;     00016985 (in fn000167AA)
fn00015896 proc
	subl2	#00000034,sp
	movab	FFFFB3D0,r5
	clrl	r9
	clrq	r3
	movl	#00000001,+28(sp)
	moval	+10(ap),+18(sp)
	movl	@+18(sp),r6
	movzwl	00007CA4[r6],(sp)
	moval	+14(ap),+1C(sp)
	movl	@+1C(sp),r6
	movzwl	00007CA4[r6],+04(sp)
	moval	+18(ap),+20(sp)
	movl	@+20(sp),r6
	movzwl	00007CA4[r6],+08(sp)
	movl	+0088(r5),+0C(sp)
	bgtr	000158E8

l000158E5:
	brw	00015DDD

l000158E8:
	movab	0000D698,r11
	movab	0000B070,+24(sp)
	movab	@0001927C,+14(sp)
	movab	@0001929C,+0010(sp)
	cmpl	r3,#00000001
	bgequ	00015944

l0001590D:
	tstl	r0
	nop

l00015910:
	decl	+0000C5FB(r5)
	blss	0001592C

l00015918:
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	00015932
0001592A                               D5 50                       .P    

l0001592C:
	calls	#00,(r11)
	movl	r0,r7

l00015932:
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,#00000001
	blssu	00015910

l00015944:
	blbs	r4,0001594A

l00015947:
	brw	00015A74

l0001594A:
	extzv	#00000001,#1F,r4,r4
	decl	r3
	decl	+0C(sp)
	cmpl	r3,@+18(sp)
	bgequ	00015991
	tstl	r0
	decl	+0000C5FB(r5)
	blss	00015978
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	0001597E
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,@+18(sp)
	blssu	0001595C
	movl	r4,r6
	bicl3	r6,(sp),r6
	mull2	#00000006,r6
	addl3	+04(ap),r6,r8
	movzbl	(r8),r2
	movl	r2,r6
	cmpl	r6,#00000010
	bgtru	000159AE
	brw	00015A34
	tstl	r0
	cmpl	r2,#00000063
	bneq	000159BD
	movl	#00000001,r0
	ret
	movzbl	+01(r8),r1
	movl	r1,r6
	subl3	r6,#00000020,r0
	extzv	r6,r0,r4,r4
	subl2	r1,r3
	subl2	#00000010,r2
	cmpl	r3,r2
	bgequ	00015A0C
	decl	+0000C5FB(r5)
	blss	000159F4
	movl	+0000C5F7(r5),r0
	incl	+0000C5F7(r5)
	movzbl	(r0),r6
	brb	000159FA
	tstl	r0
	calls	#00,(r11)
	movl	r0,r6
	movl	r6,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,r2
	blssu	000159D8
	mcoml	r4,r0
	movzwl	00007CA4[r2],r6
	mcoml	r0,r0
	bicl3	r0,r6,r0
	mull2	#00000006,r0
	addl3	+02(r8),r0,r8
	movzbl	(r8),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	00015A34
	brw	000159B0
	movzbl	+01(r8),r0
	movl	r0,r7
	subl3	r7,#00000020,r6
	extzv	r7,r6,r4,r4
	subl2	r0,r3
	movl	r9,r6
	incl	r9
	movb	+02(r8),+05EB(r5)[r6]
	cmpl	r9,#00008000
	beql	00015A5F
	brw	00015DD5
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r5)
	calls	#03,@+30(sp)
	clrl	+28(sp)
	clrl	r9
	brw	00015DD5
	nop

l00015A74:
	extzv	#00000001,#1F,r4,r4
	decl	r3
	cmpl	r3,#00000006
	bgequ	00015AB4
	decl	+0000C5FB(r5)
	blss	00015A9C
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	00015AA2
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,#00000006
	blssu	00015A80
	bicl3	#FFFFFFC0,r4,r10
	extzv	#00000006,#1A,r4,r4
	subl2	#00000006,r3
	cmpl	r3,@+20(sp)
	bgequ	00015B01
	tstl	r0
	decl	+0000C5FB(r5)
	blss	00015AE8
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	00015AEE
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,@+20(sp)
	blssu	00015ACC
	movl	r4,r6
	bicl3	r6,+08(sp),r6
	mull2	#00000006,r6
	addl3	+0C(ap),r6,r8
	movzbl	(r8),r2
	movl	r2,r6
	cmpl	r6,#00000010
	bgtru	00015B1F
	brw	00015BA4
	nop
	cmpl	r2,#00000063
	bneq	00015B2D
	movl	#00000001,r0
	ret
	movzbl	+01(r8),r1
	movl	r1,r6
	subl3	r6,#00000020,r0
	extzv	r6,r0,r4,r4
	subl2	r1,r3
	subl2	#00000010,r2
	cmpl	r3,r2
	bgequ	00015B7C
	decl	+0000C5FB(r5)
	blss	00015B64
	movl	+0000C5F7(r5),r0
	incl	+0000C5F7(r5)
	movzbl	(r0),r6
	brb	00015B6A
	tstl	r0
	calls	#00,(r11)
	movl	r0,r6
	movl	r6,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,r2
	blssu	00015B48
	mcoml	r4,r0
	movzwl	00007CA4[r2],r6
	mcoml	r0,r0
	bicl3	r0,r6,r0
	mull2	#00000006,r0
	addl3	+02(r8),r0,r8
	movzbl	(r8),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	00015BA4
	brw	00015B20
	movzbl	+01(r8),r0
	movl	r0,r7
	subl3	r7,#00000020,r6
	extzv	r7,r6,r4,r4
	subl2	r0,r3
	subl3	r10,r9,r7
	movzwl	+02(r8),r6
	subl3	r6,r7,r10
	cmpl	r3,@+1C(sp)
	bgequ	00015C01
	tstl	r0
	nop
	decl	+0000C5FB(r5)
	blss	00015BE8
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	00015BEE
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,@+1C(sp)
	blssu	00015BCC
	movl	r4,r6
	bicl3	r6,+04(sp),r6
	mull2	#00000006,r6
	addl3	+08(ap),r6,r8
	movzbl	(r8),r2
	movl	r2,r6
	cmpl	r6,#00000010
	bgtru	00015C1F
	brw	00015CA4
	nop
	cmpl	r2,#00000063
	bneq	00015C2D
	movl	#00000001,r0
	ret
	movzbl	+01(r8),r1
	movl	r1,r6
	subl3	r6,#00000020,r0
	extzv	r6,r0,r4,r4
	subl2	r1,r3
	subl2	#00000010,r2
	cmpl	r3,r2
	bgequ	00015C7C
	decl	+0000C5FB(r5)
	blss	00015C64
	movl	+0000C5F7(r5),r0
	incl	+0000C5F7(r5)
	movzbl	(r0),r6
	brb	00015C6A
	tstl	r0
	calls	#00,(r11)
	movl	r0,r6
	movl	r6,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,r2
	blssu	00015C48
	mcoml	r4,r0
	movzwl	00007CA4[r2],r6
	mcoml	r0,r0
	bicl3	r0,r6,r0
	mull2	#00000006,r0
	addl3	+02(r8),r0,r8
	movzbl	(r8),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	00015CA4
	brw	00015C20
	movzbl	+01(r8),r0
	movl	r0,r7
	subl3	r7,#00000020,r6
	extzv	r7,r6,r4,r4
	subl2	r0,r3
	movzwl	+02(r8),+2C(sp)
	tstl	r2
	beql	00015D10
	cmpl	r3,#00000008
	bgequ	00015CFC
	tstl	r0
	nop
	decl	+0000C5FB(r5)
	blss	00015CE4
	movl	+0000C5F7(r5),r6
	incl	+0000C5F7(r5)
	movzbl	(r6),r7
	brb	00015CEA
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,#00000008
	blssu	00015CC8
	bicl3	#FFFFFF00,r4,r6
	addl2	r6,+2C(sp)
	extzv	#00000008,#18,r4,r4
	subl2	#00000008,r3
	subl2	+2C(sp),+0000000C(sp)
	bicl2	#FFFF8000,r10
	movl	r10,r0
	cmpl	r0,r9
	blequ	00015D2C
	movl	r10,r6
	brb	00015D2F
	movl	r9,r6
	subl3	r6,#00008000,r2
	movl	r2,r0
	cmpl	r0,+2C(sp)
	blequ	00015D48
	movl	+2C(sp),r1
	brb	00015D4B
	tstl	r0
	movl	r2,r1
	movl	r1,r2
	subl2	r2,+2C(sp)
	tstl	+28(sp)
	beql	00015D78
	cmpl	r9,r10
	bgtru	00015D78
	pushl	r2
	pushl	#00000000
	movab	+05EB(r5),r6
	addl3	r9,r6,-(sp)
	calls	#03,@+20(sp)
	addl2	r2,r9
	addl2	r2,r10
	brb	00015DB3
	tstl	r0
	nop
	subl3	r10,r9,r0
	cmpl	r0,r2
	blssu	00015D9C
	pushl	r2
	movab	+05EB(r5),r6
	addl3	r10,r6,-(sp)
	addl3	r9,r6,-(sp)
	calls	#03,@+1C(sp)
	addl2	r2,r9
	addl2	r2,r10
	brb	00015DB3
	movl	r9,r1
	incl	r9
	movl	r10,r0
	incl	r10
	movb	+05EB(r5)[r0],+05EB(r5)[r1]
	decl	r2
	bneq	00015D9C
	cmpl	r9,#00008000
	bneq	00015DCD
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r5)
	calls	#03,@+30(sp)
	clrl	+28(sp)
	clrl	r9
	tstl	+2C(sp)
	beql	00015DD5
	brw	00015D18
	tstl	+0C(sp)
	bleq	00015DDD
	brw	00015908

l00015DDD:
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r5)
	calls	#03,0000B070
	addl3	+0084(r5),+0000C5FB(r5),r2
	extzv	#00000003,#1D,r3,r4
	addl2	r4,r2
	beql	00015E1B
	subl3	+0084(r5),+0000C64C(r5),r2
	subl2	+0000C5FB(r5),r2
	subl3	r4,r2,+008C(r5)
	movl	#00000005,r0
	ret
	clrl	r0
	ret
	tstl	r0
	xfc
	remque	+5E2C(r2),@(sp)+

;; fn00015E22: 00015E22
;;   Called from:
;;     00016A65 (in fn000167AA)
fn00015E22 proc
	subl2	#0000002C,sp
	movab	FFFFB3D0,r6
	clrl	r9
	clrq	r3
	movl	#00000001,+20(sp)
	moval	+0C(ap),+14(sp)
	movl	@+14(sp),r5
	movzwl	00007CA4[r5],(sp)
	moval	+10(ap),+18(sp)
	movl	@+18(sp),r5
	movzwl	00007CA4[r5],+04(sp)
	movl	+0088(r6),+08(sp)
	bgtr	00015E62

l00015E5F:
	brw	000162A1

l00015E62:
	movab	0000D698,r11
	movab	0000B070,+1C(sp)
	movab	@0001927C,+10(sp)
	movab	@0001929C,+0000000C(sp)
	cmpl	r4,#00000001
	bgequ	00015EC0

l00015E89:
	tstl	r0
	nop

l00015E8C:
	decl	+0000C5FB(r6)
	blss	00015EA8

l00015E94:
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00015EAE
00015EA6                   D5 50                               .P        

l00015EA8:
	calls	#00,(r11)
	movl	r0,r7

l00015EAE:
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,#00000001
	blssu	00015E8C

l00015EC0:
	blbc	r3,00015F38

l00015EC3:
	extzv	#00000001,#1F,r3,r3
	decl	r4
	decl	+08(sp)
	cmpl	r4,#00000008
	bgequ	00015F08
	tstl	r0
	decl	+0000C5FB(r6)
	blss	00015EF0
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00015EF6
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,#00000008
	blssu	00015ED4
	movl	r9,r5
	incl	r9
	movb	r3,+05EB(r6)[r5]
	cmpl	r9,#00008000
	bneq	00015F2D
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r6)
	calls	#03,@+28(sp)
	clrl	+20(sp)
	clrl	r9
	extzv	#00000008,#18,r3,r3
	subl2	#00000008,r4
	brw	00016299

l00015F38:
	extzv	#00000001,#1F,r3,r3
	decl	r4
	cmpl	r4,#00000007
	bgequ	00015F78
	decl	+0000C5FB(r6)
	blss	00015F60
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00015F66
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,#00000007
	blssu	00015F44
	bicl3	#FFFFFF80,r3,r10
	extzv	#00000007,#19,r3,r3
	subl2	#00000007,r4
	cmpl	r4,@+18(sp)
	bgequ	00015FC5
	tstl	r0
	decl	+0000C5FB(r6)
	blss	00015FAC
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00015FB2
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,@+18(sp)
	blssu	00015F90
	movl	r3,r5
	bicl3	r5,+04(sp),r5
	mull2	#00000006,r5
	addl3	+08(ap),r5,r8
	movzbl	(r8),r2
	movl	r2,r5
	cmpl	r5,#00000010
	bgtru	00015FE3
	brw	00016068
	nop
	cmpl	r2,#00000063
	bneq	00015FF1
	movl	#00000001,r0
	ret
	movzbl	+01(r8),r1
	movl	r1,r5
	subl3	r5,#00000020,r0
	extzv	r5,r0,r3,r3
	subl2	r1,r4
	subl2	#00000010,r2
	cmpl	r4,r2
	bgequ	00016040
	decl	+0000C5FB(r6)
	blss	00016028
	movl	+0000C5F7(r6),r0
	incl	+0000C5F7(r6)
	movzbl	(r0),r5
	brb	0001602E
	tstl	r0
	calls	#00,(r11)
	movl	r0,r5
	movl	r5,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,r2
	blssu	0001600C
	mcoml	r3,r0
	movzwl	00007CA4[r2],r5
	mcoml	r0,r0
	bicl3	r0,r5,r0
	mull2	#00000006,r0
	addl3	+02(r8),r0,r8
	movzbl	(r8),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	00016068
	brw	00015FE4
	movzbl	+01(r8),r0
	movl	r0,r7
	subl3	r7,#00000020,r5
	extzv	r7,r5,r3,r3
	subl2	r0,r4
	subl3	r10,r9,r7
	movzwl	+02(r8),r5
	subl3	r5,r7,r10
	cmpl	r4,@+14(sp)
	bgequ	000160C5
	tstl	r0
	nop
	decl	+0000C5FB(r6)
	blss	000160AC
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	000160B2
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,@+14(sp)
	blssu	00016090
	movl	r3,r5
	bicl3	r5,(sp),r5
	mull2	#00000006,r5
	addl3	+04(ap),r5,r8
	movzbl	(r8),r2
	movl	r2,r5
	cmpl	r5,#00000010
	bgtru	000160E2
	brw	00016168
	tstl	r0
	cmpl	r2,#00000063
	bneq	000160F1
	movl	#00000001,r0
	ret
	movzbl	+01(r8),r1
	movl	r1,r5
	subl3	r5,#00000020,r0
	extzv	r5,r0,r3,r3
	subl2	r1,r4
	subl2	#00000010,r2
	cmpl	r4,r2
	bgequ	00016140
	decl	+0000C5FB(r6)
	blss	00016128
	movl	+0000C5F7(r6),r0
	incl	+0000C5F7(r6)
	movzbl	(r0),r5
	brb	0001612E
	tstl	r0
	calls	#00,(r11)
	movl	r0,r5
	movl	r5,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,r2
	blssu	0001610C
	mcoml	r3,r0
	movzwl	00007CA4[r2],r5
	mcoml	r0,r0
	bicl3	r0,r5,r0
	mull2	#00000006,r0
	addl3	+02(r8),r0,r8
	movzbl	(r8),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	00016168
	brw	000160E4
	movzbl	+01(r8),r0
	movl	r0,r7
	subl3	r7,#00000020,r5
	extzv	r7,r5,r3,r3
	subl2	r0,r4
	movzwl	+02(r8),+24(sp)
	tstl	r2
	beql	000161D4
	cmpl	r4,#00000008
	bgequ	000161C0
	tstl	r0
	nop
	decl	+0000C5FB(r6)
	blss	000161A8
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	000161AE
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,#00000008
	blssu	0001618C
	bicl3	#FFFFFF00,r3,r5
	addl2	r5,+24(sp)
	extzv	#00000008,#18,r3,r3
	subl2	#00000008,r4
	subl2	+24(sp),+00000008(sp)
	bicl2	#FFFF8000,r10
	movl	r10,r0
	cmpl	r0,r9
	blequ	000161F0
	movl	r10,r5
	brb	000161F3
	movl	r9,r5
	subl3	r5,#00008000,r2
	movl	r2,r0
	cmpl	r0,+24(sp)
	blequ	0001620C
	movl	+24(sp),r1
	brb	0001620F
	tstl	r0
	movl	r2,r1
	movl	r1,r2
	subl2	r2,+24(sp)
	tstl	+20(sp)
	beql	0001623C
	cmpl	r9,r10
	bgtru	0001623C
	pushl	r2
	pushl	#00000000
	movab	+05EB(r6),r5
	addl3	r9,r5,-(sp)
	calls	#03,@+1C(sp)
	addl2	r2,r9
	addl2	r2,r10
	brb	00016277
	tstl	r0
	nop
	subl3	r10,r9,r0
	cmpl	r0,r2
	blssu	00016260
	pushl	r2
	movab	+05EB(r6),r5
	addl3	r10,r5,-(sp)
	addl3	r9,r5,-(sp)
	calls	#03,@+18(sp)
	addl2	r2,r9
	addl2	r2,r10
	brb	00016277
	movl	r9,r1
	incl	r9
	movl	r10,r0
	incl	r10
	movb	+05EB(r6)[r0],+05EB(r6)[r1]
	decl	r2
	bneq	00016260
	cmpl	r9,#00008000
	bneq	00016291
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r6)
	calls	#03,@+28(sp)
	clrl	+20(sp)
	clrl	r9
	tstl	+24(sp)
	beql	00016299
	brw	000161DC
	tstl	+08(sp)
	bleq	000162A1
	brw	00015E84

l000162A1:
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r6)
	calls	#03,0000B070
	addl3	+0084(r6),+0000C5FB(r6),r2
	extzv	#00000003,#1D,r4,r3
	addl2	r3,r2
	beql	000162DF
	subl3	+0084(r6),+0000C64C(r6),r2
	subl2	+0000C5FB(r6),r2
	subl3	r3,r2,+008C(r6)
	movl	#00000005,r0
	ret
	clrl	r0
	ret
	tstl	r0
	xfc
	remque	+5E2C(r2),@(sp)+

;; fn000162E6: 000162E6
;;   Called from:
;;     00016ABD (in fn000167AA)
fn000162E6 proc
	subl2	#0000002C,sp
	movab	FFFFB3D0,r6
	clrl	r9
	clrq	r3
	movl	#00000001,+20(sp)
	moval	+0C(ap),+14(sp)
	movl	@+14(sp),r5
	movzwl	00007CA4[r5],(sp)
	moval	+10(ap),+18(sp)
	movl	@+18(sp),r5
	movzwl	00007CA4[r5],+04(sp)
	movl	+0088(r6),+08(sp)
	bgtr	00016326

l00016323:
	brw	00016765

l00016326:
	movab	0000D698,r11
	movab	0000B070,+1C(sp)
	movab	@0001927C,+10(sp)
	movab	@0001929C,+0000000C(sp)
	cmpl	r4,#00000001
	bgequ	00016384

l0001634D:
	tstl	r0
	nop

l00016350:
	decl	+0000C5FB(r6)
	blss	0001636C

l00016358:
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00016372
0001636A                               D5 50                       .P    

l0001636C:
	calls	#00,(r11)
	movl	r0,r7

l00016372:
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,#00000001
	blssu	00016350

l00016384:
	blbc	r3,000163FC

l00016387:
	extzv	#00000001,#1F,r3,r3
	decl	r4
	decl	+08(sp)
	cmpl	r4,#00000008
	bgequ	000163CC
	tstl	r0
	decl	+0000C5FB(r6)
	blss	000163B4
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	000163BA
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,#00000008
	blssu	00016398
	movl	r9,r5
	incl	r9
	movb	r3,+05EB(r6)[r5]
	cmpl	r9,#00008000
	bneq	000163F1
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r6)
	calls	#03,@+28(sp)
	clrl	+20(sp)
	clrl	r9
	extzv	#00000008,#18,r3,r3
	subl2	#00000008,r4
	brw	0001675D

l000163FC:
	extzv	#00000001,#1F,r3,r3
	decl	r4
	cmpl	r4,#00000006
	bgequ	0001643C
	decl	+0000C5FB(r6)
	blss	00016424
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	0001642A
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,#00000006
	blssu	00016408
	bicl3	#FFFFFFC0,r3,r10
	extzv	#00000006,#1A,r3,r3
	subl2	#00000006,r4
	cmpl	r4,@+18(sp)
	bgequ	00016489
	tstl	r0
	decl	+0000C5FB(r6)
	blss	00016470
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00016476
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,@+18(sp)
	blssu	00016454
	movl	r3,r5
	bicl3	r5,+04(sp),r5
	mull2	#00000006,r5
	addl3	+08(ap),r5,r8
	movzbl	(r8),r2
	movl	r2,r5
	cmpl	r5,#00000010
	bgtru	000164A7
	brw	0001652C
	nop
	cmpl	r2,#00000063
	bneq	000164B5
	movl	#00000001,r0
	ret
	movzbl	+01(r8),r1
	movl	r1,r5
	subl3	r5,#00000020,r0
	extzv	r5,r0,r3,r3
	subl2	r1,r4
	subl2	#00000010,r2
	cmpl	r4,r2
	bgequ	00016504
	decl	+0000C5FB(r6)
	blss	000164EC
	movl	+0000C5F7(r6),r0
	incl	+0000C5F7(r6)
	movzbl	(r0),r5
	brb	000164F2
	tstl	r0
	calls	#00,(r11)
	movl	r0,r5
	movl	r5,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,r2
	blssu	000164D0
	mcoml	r3,r0
	movzwl	00007CA4[r2],r5
	mcoml	r0,r0
	bicl3	r0,r5,r0
	mull2	#00000006,r0
	addl3	+02(r8),r0,r8
	movzbl	(r8),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	0001652C
	brw	000164A8
	movzbl	+01(r8),r0
	movl	r0,r7
	subl3	r7,#00000020,r5
	extzv	r7,r5,r3,r3
	subl2	r0,r4
	subl3	r10,r9,r7
	movzwl	+02(r8),r5
	subl3	r5,r7,r10
	cmpl	r4,@+14(sp)
	bgequ	00016589
	tstl	r0
	nop
	decl	+0000C5FB(r6)
	blss	00016570
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00016576
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,@+14(sp)
	blssu	00016554
	movl	r3,r5
	bicl3	r5,(sp),r5
	mull2	#00000006,r5
	addl3	+04(ap),r5,r8
	movzbl	(r8),r2
	movl	r2,r5
	cmpl	r5,#00000010
	bgtru	000165A6
	brw	0001662C
	tstl	r0
	cmpl	r2,#00000063
	bneq	000165B5
	movl	#00000001,r0
	ret
	movzbl	+01(r8),r1
	movl	r1,r5
	subl3	r5,#00000020,r0
	extzv	r5,r0,r3,r3
	subl2	r1,r4
	subl2	#00000010,r2
	cmpl	r4,r2
	bgequ	00016604
	decl	+0000C5FB(r6)
	blss	000165EC
	movl	+0000C5F7(r6),r0
	incl	+0000C5F7(r6)
	movzbl	(r0),r5
	brb	000165F2
	tstl	r0
	calls	#00,(r11)
	movl	r0,r5
	movl	r5,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,r2
	blssu	000165D0
	mcoml	r3,r0
	movzwl	00007CA4[r2],r5
	mcoml	r0,r0
	bicl3	r0,r5,r0
	mull2	#00000006,r0
	addl3	+02(r8),r0,r8
	movzbl	(r8),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	0001662C
	brw	000165A8
	movzbl	+01(r8),r0
	movl	r0,r7
	subl3	r7,#00000020,r5
	extzv	r7,r5,r3,r3
	subl2	r0,r4
	movzwl	+02(r8),+24(sp)
	tstl	r2
	beql	00016698
	cmpl	r4,#00000008
	bgequ	00016684
	tstl	r0
	nop
	decl	+0000C5FB(r6)
	blss	0001666C
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00016672
	tstl	r0
	calls	#00,(r11)
	movl	r0,r7
	movl	r7,r0
	ashl	r4,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r4
	cmpl	r4,#00000008
	blssu	00016650
	bicl3	#FFFFFF00,r3,r5
	addl2	r5,+24(sp)
	extzv	#00000008,#18,r3,r3
	subl2	#00000008,r4
	subl2	+24(sp),+00000008(sp)
	bicl2	#FFFF8000,r10
	movl	r10,r0
	cmpl	r0,r9
	blequ	000166B4
	movl	r10,r5
	brb	000166B7
	movl	r9,r5
	subl3	r5,#00008000,r2
	movl	r2,r0
	cmpl	r0,+24(sp)
	blequ	000166D0
	movl	+24(sp),r1
	brb	000166D3
	tstl	r0
	movl	r2,r1
	movl	r1,r2
	subl2	r2,+24(sp)
	tstl	+20(sp)
	beql	00016700
	cmpl	r9,r10
	bgtru	00016700
	pushl	r2
	pushl	#00000000
	movab	+05EB(r6),r5
	addl3	r9,r5,-(sp)
	calls	#03,@+1C(sp)
	addl2	r2,r9
	addl2	r2,r10
	brb	0001673B
	tstl	r0
	nop
	subl3	r10,r9,r0
	cmpl	r0,r2
	blssu	00016724
	pushl	r2
	movab	+05EB(r6),r5
	addl3	r10,r5,-(sp)
	addl3	r9,r5,-(sp)
	calls	#03,@+18(sp)
	addl2	r2,r9
	addl2	r2,r10
	brb	0001673B
	movl	r9,r1
	incl	r9
	movl	r10,r0
	incl	r10
	movb	+05EB(r6)[r0],+05EB(r6)[r1]
	decl	r2
	bneq	00016724
	cmpl	r9,#00008000
	bneq	00016755
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r6)
	calls	#03,@+28(sp)
	clrl	+20(sp)
	clrl	r9
	tstl	+24(sp)
	beql	0001675D
	brw	000166A0
	tstl	+08(sp)
	bleq	00016765
	brw	00016348

l00016765:
	pushl	#00000000
	pushl	r9
	pushab	+05EB(r6)
	calls	#03,0000B070
	addl3	+0084(r6),+0000C5FB(r6),r2
	extzv	#00000003,#1D,r4,r3
	addl2	r3,r2
	beql	000167A3
	subl3	+0084(r6),+0000C64C(r6),r2
	subl2	+0000C5FB(r6),r2
	subl3	r3,r2,+008C(r6)
	movl	#00000005,r0
	ret
	clrl	r0
	ret
	tstl	r0
	bvc	fn000167AA

;; fn000167AA: 000167AA
fn000167AA proc
	movab	-041C(sp),sp
	movab	FFFFB3D0,r4
	movab	FFFFAB0C,r2
	movl	#00000007,-18(fp)
	addl3	+0084(r4),+0000C5FB(r4),ap
	cmpl	ap,#00030D40
	bleq	000167DC

l000167D4:
	movb	#08,r0
	brb	000167DF
000167D9                            D5 50 01                      .P.    

l000167DC:
	movb	#07,r0

l000167DF:
	cvtbl	r0,-1C(fp)
	clrl	+0000C819(r4)
	movzwl	+0000C640(r4),ap
	bbs	#00000002,ap,000167F7

l000167F4:
	brw	000169AC

l000167F7:
	movl	#00000009,-14(fp)
	movzwl	#0100,-(sp)
	pushal	-041C(fp)
	calls	#02,00015254
	movl	r0,r3
	beql	00016812

l0001680E:
	movl	r3,r0
	ret

l00016812:
	pushal	-14(fp)
	pushal	-08(fp)
	clrq	-(sp)
	movzwl	#0100,-(sp)
	movzwl	#0100,-(sp)
	pushal	-041C(fp)
	calls	#07,0001878C
	movl	r0,r3
	beql	00016847

l00016834:
	cmpl	r3,#00000001
	bneq	00016843

l00016839:
	pushl	-08(fp)
	calls	#01,00018BF4

l00016843:
	movl	r3,r0
	ret

l00016847:
	movzbl	#40,-(sp)
	pushal	-041C(fp)
	calls	#02,00015254
	movl	r0,r3
	beql	0001685D

l00016859:
	movl	r3,r0
	ret

l0001685D:
	pushal	-18(fp)
	pushal	-0C(fp)
	pushal	+0100(r2)
	pushal	+0080(r2)
	pushl	#00000000
	movzbl	#40,-(sp)
	pushal	-041C(fp)
	calls	#07,0001878C
	movl	r0,r3
	beql	0001689E

l00016881:
	cmpl	r3,#00000001
	bneq	00016890

l00016886:
	pushl	-0C(fp)
	calls	#01,00018BF4

l00016890:
	pushl	-08(fp)
	calls	#01,00018BF4
	movl	r3,r0
	ret

l0001689E:
	movzbl	#40,-(sp)
	pushal	-041C(fp)
	calls	#02,00015254
	movl	r0,r3
	beql	000168B4

l000168B0:
	movl	r3,r0
	ret

l000168B4:
	movzwl	+0000C640(r4),ap
	bbc	#00000001,ap,00016928

l000168BF:
	pushal	-1C(fp)
	pushal	-10(fp)
	pushal	+0100(r2)
	pushal	+0200(r2)
	pushl	#00000000
	movzbl	#40,-(sp)
	pushal	-041C(fp)
	calls	#07,0001878C
	movl	r0,r3
	beql	0001690A

l000168E3:
	cmpl	r3,#00000001
	bneq	000168F2

l000168E8:
	pushl	-10(fp)
	calls	#01,00018BF4

l000168F2:
	pushl	-0C(fp)
	calls	#01,00018BF4
	pushl	-08(fp)
	calls	#01,00018BF4
	movl	r3,r0
	ret

l0001690A:
	pushl	-1C(fp)
	pushl	-18(fp)
	pushl	-14(fp)
	pushl	-10(fp)
	pushl	-0C(fp)
	pushl	-08(fp)
	calls	#06,00015308
	movl	r0,r3
	brb	0001698D
00016926                   D5 50                               .P        

l00016928:
	pushal	-1C(fp)
	pushal	-10(fp)
	pushal	+0100(r2)
	pushal	+0180(r2)
	pushl	#00000000
	movzbl	#40,-(sp)
	pushal	-041C(fp)
	calls	#07,0001878C
	movl	r0,r3
	beql	00016973

l0001694C:
	cmpl	r3,#00000001
	bneq	0001695B

l00016951:
	pushl	-10(fp)
	calls	#01,00018BF4

l0001695B:
	pushl	-0C(fp)
	calls	#01,00018BF4
	pushl	-08(fp)
	calls	#01,00018BF4
	movl	r3,r0
	ret

l00016973:
	pushl	-1C(fp)
	pushl	-18(fp)
	pushl	-14(fp)
	pushl	-10(fp)
	pushl	-0C(fp)
	pushl	-08(fp)
	calls	#06,00015894
	movl	r0,r3

l0001698D:
	pushl	-10(fp)
	movab	00018BF4,ap
	calls	#01,(ap)
	pushl	-0C(fp)
	calls	#01,(ap)
	pushl	-08(fp)
	calls	#01,(ap)
	brw	00016AD9
000169A9                            D5 50 01                      .P.    

l000169AC:
	movzbl	#40,-(sp)
	pushal	-041C(fp)
	calls	#02,00015254
	movl	r0,r3
	beql	000169C2

l000169BE:
	movl	r3,r0
	ret

l000169C2:
	pushal	-18(fp)
	pushal	-0C(fp)
	pushal	+0100(r2)
	pushal	(r2)
	pushl	#00000000
	movzbl	#40,-(sp)
	pushal	-041C(fp)
	calls	#07,0001878C
	movl	r0,r3
	beql	000169F7

l000169E4:
	cmpl	r3,#00000001
	bneq	000169F3

l000169E9:
	pushl	-0C(fp)
	calls	#01,00018BF4

l000169F3:
	movl	r3,r0
	ret

l000169F7:
	movzbl	#40,-(sp)
	pushal	-041C(fp)
	calls	#02,00015254
	movl	r0,r3
	beql	00016A0D

l00016A09:
	movl	r3,r0
	ret

l00016A0D:
	movzwl	+0000C640(r4),ap
	bbc	#00000001,ap,00016A70

l00016A18:
	pushal	-1C(fp)
	pushal	-10(fp)
	pushal	+0100(r2)
	pushal	+0200(r2)
	pushl	#00000000
	movzbl	#40,-(sp)
	pushal	-041C(fp)
	calls	#07,0001878C
	movl	r0,r3
	beql	00016A59

l00016A3C:
	cmpl	r3,#00000001
	bneq	00016A4B

l00016A41:
	pushl	-10(fp)
	calls	#01,00018BF4

l00016A4B:
	pushl	-0C(fp)
	calls	#01,00018BF4
	movl	r3,r0
	ret

l00016A59:
	pushl	-1C(fp)
	pushl	-18(fp)
	pushl	-10(fp)
	pushl	-0C(fp)
	calls	#04,00015E20
	movl	r0,r3
	brb	00016AC5
00016A6F                                              01                .

l00016A70:
	pushal	-1C(fp)
	pushal	-10(fp)
	pushal	+0100(r2)
	pushal	+0180(r2)
	pushl	#00000000
	movzbl	#40,-(sp)
	pushal	-041C(fp)
	calls	#07,0001878C
	movl	r0,r3
	beql	00016AB1

l00016A94:
	cmpl	r3,#00000001
	bneq	00016AA3

l00016A99:
	pushl	-10(fp)
	calls	#01,00018BF4

l00016AA3:
	pushl	-0C(fp)
	calls	#01,00018BF4
	movl	r3,r0
	ret

l00016AB1:
	pushl	-1C(fp)
	pushl	-18(fp)
	pushl	-10(fp)
	pushl	-0C(fp)
	calls	#04,000162E4
	movl	r0,r3

l00016AC5:
	pushl	-10(fp)
	calls	#01,00018BF4
	pushl	-0C(fp)
	calls	#01,00018BF4

l00016AD9:
	movl	r3,r0
	ret
00016ADD                                        00 00 00              ...
00016AE0 FC 0F 9E AE 98 5E 9E EF E4 48 FE FF 53 9E EF 99 .....^...H..S...
00016AF0 42 FE FF 5B D4 5A D4 AE 18 D4 52 D1 A3 50 01 15 B..[.Z....R..P..
00016B00 02 D6 52 D0 52 AE 04 D4 AE 10 7C AE 08 9E AD DD ..R.R.....|.....
00016B10 C3 E7 05 D1 A3 40 02 18 70 D5 A3 28 13 36 DD 00 .....@..p..(.6..
00016B20 C1 AE 08 AE 08 52 DD 42 CB D0 00 DD 42 CB CC 00 .....R.B....B...
00016B30 DF CB DC 00 9F C3 EB 05 FB 04 FF 91 27 00 00 DD ............'...
00016B40 50 9F C3 EB 05 9F 63 D0 E3 39 C8 00 00 52 FB 04 P.....c..9...R..
00016B50 62 11 36 01 DD 00 C1 AE 08 AE 08 52 DD 42 CB D0 b.6........R.B..
00016B60 00 DD 42 CB CC 00 DF EF 3C FE FC FF 9F C3 EB 05 ..B.....<.......
00016B70 FB 04 FF 59 27 00 00 DD 50 9F C3 EB 05 9F 63 D0 ...Y'...P.....c.
00016B80 E3 39 C8 00 00 52 FB 04 62 D4 AE 1C 3C E3 88 C6 .9...R..b...<...
00016B90 00 00 52 D1 AE 1C 52 1F 03 31 7A 05 9E EF D2 70 ..R...R..1z....p
00016BA0 FF FF AE 20 9E EF 6E 20 00 00 59 9E FF 1F 27 00 ... ..n ..Y...'.
00016BB0 00 CE 2C 00 DD 04 9F C3 98 00 FB 02 EF FB 69 FF ..,...........i.
00016BC0 FF D5 50 12 04 D0 33 50 04 DD 04 9F E3 34 C6 00 ..P...3P.....4..
00016BD0 00 9F C3 98 00 FB 03 FF FC 26 00 00 D5 50 13 57 .........&...P.W
00016BE0 3C 8F 01 04 7E DD AE 20 DF EF 6E 45 FE FF 9F C3 <...~.. ..nE....
00016BF0 EB 05 FB 03 FF D7 26 00 00 DD 50 9F C3 EB 05 9F ......&...P.....
00016C00 63 D0 E3 39 C8 00 00 52 FB 04 62 3C 8F 01 04 7E c..9...R..b<...~
00016C10 DF EF 32 11 FF FF 9F C3 EB 05 FB 02 FF AF 26 00 ..2...........&.
00016C20 00 DD 50 9F C3 EB 05 9F 63 D0 E3 39 C8 00 00 52 ..P.....c..9...R
00016C30 FB 04 62 D0 03 50 04 FB 00 EF CA AF FF FF D5 50 ..b..P.........P
00016C40 13 01 04 DD 02 3C E3 70 C6 00 00 7E FB 02 BE 28 .....<.p...~...(
00016C50 D5 50 13 0A D0 50 AE 18 D1 50 01 15 01 04 D5 E3 .P...P...P......
00016C60 27 C6 00 00 13 13 DD E3 27 C6 00 00 FB 01 FF 3D '.......'......=
00016C70 26 00 00 D4 E3 27 C6 00 00 DD 03 3C E3 72 C6 00 &....'.....<.r..
00016C80 00 7E FB 02 BE 28 D5 50 13 0A D0 50 AE 18 D1 50 .~...(.P...P...P
00016C90 01 15 01 04 D5 A3 6C 12 62 C3 04 C3 90 00 5C D4 ......l.b.....\.
00016CA0 5A C1 04 5C 52 D5 62 13 22 D5 50 01 DD A3 10 DD Z..\R.b.".P.....
00016CB0 62 9F E3 01 C7 00 00 FB 03 69 D5 50 13 06 D0 01 b........i.P....
00016CC0 5A 11 08 01 C0 04 52 D5 62 12 E1 D5 5A 13 2C C3 Z.....R.b...Z.,.
00016CD0 04 C3 94 00 52 C0 04 52 D5 62 13 1F DD A3 10 DD ....R..R.b......
00016CE0 62 9F E3 01 C7 00 00 FB 03 69 D5 50 13 06 D4 5A b........i.P...Z
00016CF0 11 09 D5 50 C0 04 52 D5 62 12 E1 D5 A3 6C 12 07 ...P..R.b....l..
00016D00 D5 5A 12 03 31 D9 03 3C E3 62 C6 00 00 54 D0 54 .Z..1..<.b...T.T
00016D10 5C 78 8F F7 5C 52 CA 8F 80 FF FF FF 52 C0 8F 50 \x..\R......R..P
00016D20 00 00 00 52 D4 51 D0 52 50 D0 8F 64 00 00 00 52 ...R.Q.RP..d...R
00016D30 13 16 D1 52 01 13 16 14 0F D1 52 50 13 0F 1A 03 ...R......RP....
00016D40 C2 52 51 C0 50 51 11 05 7B 52 50 50 51 B0 51 AE .RQ.PQ..{RPPQ.Q.
00016D50 26 78 8F FB 5C 52 CA 8F F0 FF FF FF 52 F7 52 AE &x..\R......R.R.
00016D60 30 AB 8F E0 FF 54 AE 32 3C E3 60 C6 00 00 50 78 0....T.2<.`...Px
00016D70 8F F5 50 52 CA 8F E0 FF FF FF 52 F7 52 AE 14 78 ..PR......R.R..x
00016D80 8F FB 50 52 CA 8F C0 FF FF FF 52 F7 52 AE 16 11 ..PR......R.R...
00016D90 2A D5 50 01 3C AE 30 AE 34 B0 AE 26 AE 30 B0 AE *.P.<.0.4..&.0..
00016DA0 32 AE 26 B0 AE 34 AE 32 11 11 D5 50 3C AE 30 AE 2.&..4.2...P<.0.
00016DB0 34 B0 AE 32 AE 30 B0 AE 34 AE 32 D0 E3 68 C6 00 4..2.0..4.2..h..
00016DC0 00 AE 28 3C E3 5C C6 00 00 52 E9 52 04 C2 0C AE ..(<.\...R.R....
00016DD0 28 D0 E3 6C C6 00 00 54 D0 AE 28 57 D5 54 12 08 (..l...T..(W.T..
00016DE0 D4 58 31 0A 01 D5 50 01 D1 54 8F 80 84 1E 00 1A .X1...P..T......
00016DF0 03 31 88 00 D4 51 D0 54 50 D0 8F E8 03 00 00 52 .1...Q.TP......R
00016E00 18 0A D1 52 50 1A 0A D6 51 11 06 01 7B 52 50 51 ...RP...Q...{RPQ
00016E10 50 D0 51 AE 38 D1 54 57 1F 2E C3 57 54 52 EF 01 P.Q.8.TW...WTR..
00016E20 1F AE 38 5C C0 5C 52 D4 51 D0 52 50 D0 AE 38 52 ..8\.\R.Q.RP..8R
00016E30 18 0A D1 52 50 1A 0A D6 51 11 06 01 7B 52 50 51 ...RP...Q...{RPQ
00016E40 50 D0 51 55 11 2E D5 50 C3 54 57 52 EF 01 1F AE P.QU...P.TWR....
00016E50 38 5C C0 5C 52 D4 51 D0 52 50 D0 AE 38 52 18 0C 8\.\R.Q.RP..8R..
00016E60 D1 52 50 1A 0C D6 51 11 08 D5 50 01 7B 52 50 51 .RP...Q...P.{RPQ
00016E70 50 CE 51 55 D0 55 58 11 76 D5 50 01 D0 54 AE 38 P.QU.UX.v.P..T.8
00016E80 D1 54 57 1F 37 C3 57 54 52 C4 8F E8 03 00 00 52 .TW.7.WTR......R
00016E90 EF 01 1F AE 38 5C C0 5C 52 D4 51 D0 52 50 D0 AE ....8\.\R.Q.RP..
00016EA0 38 52 18 0C D1 52 50 1A 0C D6 51 11 08 D5 50 01 8R...RP...Q...P.
00016EB0 7B 52 50 51 50 D0 51 56 11 32 D5 50 C3 54 57 52 {RPQP.QV.2.P.TWR
00016EC0 C4 8F E8 03 00 00 52 EF 01 1F AE 38 5C C0 5C 52 ......R....8\.\R
00016ED0 D4 51 D0 52 50 D0 AE 38 52 18 09 D1 52 50 1A 09 .Q.RP..8R...RP..
00016EE0 D6 51 11 05 7B 52 50 51 50 CE 51 56 D0 56 58 D0 .Q..{RPQP.QV.VX.
00016EF0 58 55 18 10 90 2D 56 CE 55 52 C0 05 52 C7 0A 52 XU...-V.UR..R..R
00016F00 55 11 0A 01 90 20 56 C0 05 55 C6 0A 55 3C E3 5E U.... V..U..U<.^
00016F10 C6 00 00 5C D1 5C 0B 1E 0B B0 E3 5E C6 00 00 54 ...\.\.....^...T
00016F20 11 05 D5 50 B0 0B 54 3C 54 AE 34 78 03 AE 34 52 ...P..T<T.4x..4R
00016F30 9F 42 CB 27 02 DF AD D5 FB 02 FF 7D 23 00 00 D1 .B.'.......}#...
00016F40 AE 34 08 12 1F 3C E3 5C C6 00 00 52 78 8F FF 52 .4...<.\...Rx..R
00016F50 52 CA 8F FC FF FF FF 52 90 42 CB 22 02 AD DA 11 R......R.B."....
00016F60 1D D5 50 01 D1 AE 34 0B 1F 14 3C E3 5E C6 00 00 ..P...4...<.^...
00016F70 7E DF EF 38 FA FC FF 9F AD D9 FB 03 BE 38 D1 55 ~..8.........8.U
00016F80 8F 64 00 00 00 12 0D DF AB 07 DF AD F2 FB 02 BE .d..............
00016F90 34 11 0F 01 DD 55 98 56 7E DF 6B DF AD F2 FB 04 4....U.V~.k.....
00016FA0 BE 3C D5 AE 04 13 65 DD 00 D0 C3 E7 05 5C E1 04 .<....e......\..
00016FB0 AC 14 09 90 8F 5E 54 11 06 D5 50 01 90 20 54 98 .....^T...P.. T.
00016FC0 54 7E DD E3 64 C6 00 00 3C AE 22 7E 3C AE 24 7E T~..d...<."~<.$~
00016FD0 3C AE 3A 7E 3C AE 4A 7E 3C AE 4C 7E DF AD F2 DD <.:~<.J~<.L~....
00016FE0 AE 4C DF AD D5 DD E3 6C C6 00 00 DF CB FF 00 9F .L.....l........
00016FF0 C3 EB 05 FB 0D BE 64 DD 50 9F C3 EB 05 9F 63 D0 ......d.P.....c.
00017000 E3 39 C8 00 00 52 FB 04 62 11 53 01 DD 00 D0 C3 .9...R..b.S.....
00017010 E7 05 5C E1 04 AC 14 08 90 8F 5E 54 11 05 D5 50 ..\.......^T...P
00017020 90 20 54 98 54 7E 3C AE 1E 7E 3C AE 20 7E 3C AE . T.T~<..~<. ~<.
00017030 36 7E 3C AE 46 7E 3C AE 48 7E DD E3 6C C6 00 00 6~<.F~<.H~..l...
00017040 DF CB B8 01 9F C3 EB 05 FB 09 BE 54 DD 50 9F C3 ...........T.P..
00017050 EB 05 9F 63 D0 E3 39 C8 00 00 52 FB 04 62 9F C3 ...c..9...R..b..
00017060 EB 05 9F E3 01 C7 00 00 FB 02 EF 21 98 FF FF D0 ...........!....
00017070 50 52 DD 00 DD 52 FB 01 FF 47 22 00 00 DD 50 DD PR...R...G"...P.
00017080 52 9F 63 D0 E3 39 C8 00 00 52 FB 04 62 DD 00 DD R.c..9...R..b...
00017090 01 DF EF 23 F9 FC FF 9F 63 D0 E3 39 C8 00 00 52 ...#....c..9...R
000170A0 FB 04 62 D5 A3 40 12 08 90 05 54 11 05 D5 50 01 ..b..@....T...P.
000170B0 94 54 98 54 7E 3C E3 74 C6 00 00 7E FB 02 BE 28 .T.T~<.t...~...(
000170C0 D5 50 13 0A D0 50 AE 18 D1 50 01 15 01 04 C0 E3 .P...P...P......
000170D0 6C C6 00 00 AE 0C C0 AE 28 AE 08 D6 AE 10 11 23 l.......(......#
000170E0 B5 E3 74 C6 00 00 13 1B DD 00 3C E3 74 C6 00 00 ..t.......<.t...
000170F0 7E FB 02 BE 28 D5 50 13 0A D0 50 AE 18 D1 50 01 ~...(.P...P...P.
00017100 15 01 04 D6 AE 1C 3C E3 88 C6 00 00 52 D1 AE 1C ......<.....R...
00017110 52 1E 03 31 9E FA D1 A3 40 02 19 03 31 E5 01 D0 R..1....@...1...
00017120 AE 0C 55 D0 AE 08 57 D5 55 12 05 D4 58 31 F7 00 ..U...W.U...X1..
00017130 D1 55 8F 80 84 1E 00 1B 7F D4 51 D0 55 50 D0 8F .U........Q.UP..
00017140 E8 03 00 00 52 18 09 D1 52 50 1A 09 D6 51 11 05 ....R...RP...Q..
00017150 7B 52 50 51 50 D0 51 6E D1 55 57 1F 2B C3 57 55 {RPQP.Qn.UW.+.WU
00017160 54 EF 01 1F 6E 52 C0 52 54 D4 51 D0 54 50 D0 6E T...nR.RT.Q.TP.n
00017170 52 18 09 D1 52 50 1A 09 D6 51 11 05 7B 52 50 51 R...RP...Q..{RPQ
00017180 50 D0 51 56 11 2A D5 50 C3 55 57 54 EF 01 1F 6E P.QV.*.P.UWT...n
00017190 52 C0 52 54 D4 51 D0 54 50 D0 6E 52 18 0A D1 52 R.RT.Q.TP.nR...R
000171A0 50 1A 0A D6 51 11 06 01 7B 52 50 51 50 CE 51 56 P...Q...{RPQP.QV
000171B0 D0 56 58 11 72 D5 50 01 D0 55 6E D1 55 57 1F 34 .VX.r.P..Un.UW.4
000171C0 C3 57 55 54 C4 8F E8 03 00 00 54 EF 01 1F 6E 52 .WUT......T...nR
000171D0 C0 52 54 D4 51 D0 54 50 D0 6E 52 18 0B D1 52 50 .RT.Q.TP.nR...RP
000171E0 1A 0B D6 51 11 07 D5 50 7B 52 50 51 50 D0 51 56 ...Q...P{RPQP.QV
000171F0 11 32 D5 50 C3 55 57 54 C4 8F E8 03 00 00 54 EF .2.P.UWT......T.
00017200 01 1F 6E 52 C0 52 54 D4 51 D0 54 50 D0 6E 52 18 ..nR.RT.Q.TP.nR.
00017210 0B D1 52 50 1A 0B D6 51 11 07 D5 50 7B 52 50 51 ..RP...Q...P{RPQ
00017220 50 CE 51 56 D0 56 58 D0 58 54 18 10 90 2D 55 CE P.QV.VX.XT...-U.
00017230 54 52 C0 05 52 C7 0A 52 54 11 0A 01 90 20 55 C0 TR..R..RT.... U.
00017240 05 54 C6 0A 54 D1 54 8F 64 00 00 00 12 12 DF AB .T..T.T.d.......
00017250 07 DF AD F2 FB 02 FF 75 20 00 00 11 14 D5 50 01 .......u .....P.
00017260 DD 54 98 55 7E DF 6B DF AD F2 FB 04 FF 5F 20 00 .T.U~.k......_ .
00017270 00 D5 AE 04 13 4A DD 00 D1 AE 14 01 12 0A DE EF .....J..........
00017280 32 F7 FC FF 54 11 08 01 DE EF 26 F7 FC FF 54 DD 2...T.....&...T.
00017290 54 DD AE 18 DF AD F2 DD AE 18 DD AE 20 DF CB 38 T........... ..8
000172A0 01 9F C3 EB 05 FB 07 FF 24 20 00 00 DD 50 9F C3 ........$ ...P..
000172B0 EB 05 9F 63 D0 E3 39 C8 00 00 52 FB 04 62 11 44 ...c..9...R..b.D
000172C0 DD 00 D1 AE 14 01 12 0C DE EF EB F6 FC FF 54 11 ..............T.
000172D0 0A D5 50 01 DE EF DD F6 FC FF 54 DD 54 DD AE 18 ..P.......T.T...
000172E0 DD AE 18 DF CB DC 01 9F C3 EB 05 FB 05 FF DE 1F ................
000172F0 00 00 DD 50 9F C3 EB 05 9F 63 D0 E3 39 C8 00 00 ...P.....c..9...
00017300 52 FB 04 62 DD 04 9F C3 98 00 FB 02 EF AB 62 FF R..b..........b.
00017310 FF D5 50 12 04 D0 33 50 04 DD 04 9F E3 39 C6 00 ..P...3P.....9..
00017320 00 9F C3 98 00 FB 03 FF AC 1F 00 00 D5 50 13 2C .............P.,
00017330 3C 8F 01 04 7E DF EF ED 3F FE FF 9F C3 EB 05 FB <...~...?.......
00017340 02 FF 8A 1F 00 00 DD 50 9F C3 EB 05 9F 63 D0 E3 .......P.....c..
00017350 39 C8 00 00 52 FB 04 62 D0 01 AE 18 D5 AE 10 12 9...R..b........
00017360 0A D1 AE 18 01 14 04 D0 0B AE 18 D0 AE 18 50 04 ..............P.
00017370 1C 00 C2 04 5E 9E EF 55 40 FE FF 52 9F E2 01 C7 ....^..U@..R....
00017380 00 00 FB 01 FF 3B 1F 00 00 94 54 D5 50 13 34 D7 .....;....T.P.4.
00017390 50 90 40 E2 01 C7 00 00 53 91 53 2F 13 23 D0 C2 P.@.....S.S/.#..
000173A0 E7 05 5C D5 AC 0C 12 1B DD 2F 9F E2 01 C7 00 00 ..\....../......
000173B0 FB 02 FF A9 1E 00 00 D5 50 12 08 91 53 8F 5C 12 ........P...S.\.
000173C0 02 96 54 98 54 50 04 01 FC 0F                   ..T.TP....      

;; fn000173CA: 000173CA
fn000173CA proc
	subl2	#0000001C,sp
	movab	00007D48,r10
	movab	FFFFB15C,r7
	movab	FFFFB3D0,r3
	clrl	r6
	clrl	r11
	clrl	@+04(ap)
	clrl	@+08(ap)
	movab	-19(fp),+05E7(r3)
	clrl	r8
	movzwl	+0000C688(r3),r2
	cmpl	r8,r2
	blssu	00017403

l00017400:
	brw	000175F6

l00017403:
	movab	0000DC74,r9
	movab	00018C18,r5
	tstl	r0
	nop

l00017414:
	pushl	#00000004
	pushab	+0098(r3)
	calls	#02,0000D5BC
	tstl	r0
	bneq	00017429

l00017425:
	movl	#00000033,r0
	ret

l00017429:
	pushl	#00000004
	pushab	+0000C634(r3)
	pushab	+0098(r3)
	calls	#03,@000192D8
	tstl	r0
	beql	0001748E

l00017440:
	movzwl	#0401,-(sp)
	pushl	r8
	pushal	(r7)
	pushab	+05EB(r3)
	calls	#03,@000192D0
	pushl	r0
	pushab	+05EB(r3)
	pushab	(r3)
	movl	+0000C839(r3),r2
	calls	#04,(r2)
	movzwl	#0401,-(sp)
	pushal	(r10)
	pushab	+05EB(r3)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r3)
	pushab	(r3)
	movl	+0000C839(r3),r2
	calls	#04,(r2)
	movl	#00000003,r0
	ret

l0001748E:
	calls	#00,00011C08
	tstl	r0
	beql	0001749A

l00017499:
	ret

l0001749A:
	pushl	#00000002
	movzwl	+0000C670(r3),-(sp)
	calls	#02,(r9)
	tstl	r0
	beql	000174B3

l000174AA:
	movl	r0,r11
	cmpl	r0,#00000001
	bleq	000174B3

l000174B2:
	ret

l000174B3:
	tstl	+0000C627(r3)
	beql	000174CE

l000174BB:
	pushl	+0000C627(r3)
	calls	#01,@000192B0
	clrl	+0000C627(r3)

l000174CE:
	pushl	#00000003
	movzwl	+0000C672(r3),-(sp)
	calls	#02,(r9)
	tstl	r0
	beql	000174E7

l000174DE:
	movl	r0,r11
	cmpl	r0,#00000001
	bleq	000174E7

l000174E6:
	ret

l000174E7:
	tstl	+6C(r3)
	bneq	0001754B

l000174EC:
	subl3	#00000004,+0090(r3),r0
	clrl	r6
	addl3	#00000004,r0,r2
	tstl	(r2)
	beql	0001751B

l000174FC:
	pushl	+10(r3)
	pushl	(r2)
	pushab	+0000C701(r3)
	calls	#03,(r5)
	tstl	r0
	beql	00017514

l0001750E:
	movl	#00000001,r6
	brb	0001751B
00017513          01                                        .            

l00017514:
	addl2	#00000004,r2
	tstl	(r2)
	bneq	000174FC

l0001751B:
	tstl	r6
	beql	0001754B

l0001751F:
	subl3	#00000004,+0094(r3),r2
	addl2	#00000004,r2
	tstl	(r2)
	beql	0001754B

l0001752C:
	pushl	+10(r3)
	pushl	(r2)
	pushab	+0000C701(r3)
	calls	#03,(r5)
	tstl	r0
	beql	00017544

l0001753E:
	clrl	r6
	brb	0001754B
00017542       D5 50                                       .P            

l00017544:
	addl2	#00000004,r2
	tstl	(r2)
	bneq	0001752C

l0001754B:
	tstl	+6C(r3)
	bneq	00017554

l00017550:
	tstl	r6
	beql	000175C4

l00017554:
	pushab	+0000C701(r3)
	calls	#01,@000192C4
	clrb	r4
	tstl	r0
	beql	0001759B

l00017567:
	decl	r0
	movb	+0000C701(r3)[r0],r2
	cmpb	r2,#2F
	beql	00017599

l00017576:
	movl	+05E7(r3),r0
	tstl	+0C(r0)
	bneq	0001759B

l00017580:
	pushl	#0000002F
	pushab	+0000C701(r3)
	calls	#02,@00019260
	tstl	r0
	bneq	0001759B

l00017593:
	cmpb	r2,#5C
	bneq	0001759B

l00017599:
	incb	r4

l0001759B:
	cvtbl	r4,r2
	bneq	000175C4

l000175A0:
	movzwl	+0000C660(r3),-(sp)
	movzwl	+0000C662(r3),-(sp)
	calls	#02,0000BC88
	movl	+04(ap),r2
	cmpl	(r2),r0
	bgequ	000175C1

l000175BE:
	movl	r0,(r2)

l000175C1:
	incl	@+08(ap)

l000175C4:
	tstw	+0000C674(r3)
	beql	000175E5

l000175CC:
	pushl	#00000000
	movzwl	+0000C674(r3),-(sp)
	calls	#02,(r9)
	tstl	r0
	beql	000175E5

l000175DC:
	movl	r0,r11
	cmpl	r0,#00000001
	bleq	000175E5

l000175E4:
	ret

l000175E5:
	incl	r8
	movzwl	+0000C688(r3),r2
	cmpl	r8,r2
	bgequ	000175F6

l000175F3:
	brw	00017414

l000175F6:
	pushl	#00000004
	pushab	+0098(r3)
	calls	#02,0000D5BC
	tstl	r0
	bneq	0001760B

l00017607:
	movl	#00000033,r0
	ret

l0001760B:
	pushl	#00000004
	pushab	+0000C639(r3)
	pushab	+0098(r3)
	calls	#03,@000192D8
	tstl	r0
	beql	0001764D

l00017622:
	movzwl	#0401,-(sp)
	pushal	FFFFB328
	pushab	+05EB(r3)
	calls	#02,@000192D0
	pushl	r0
	pushab	+05EB(r3)
	pushab	(r3)
	movl	+0000C839(r3),r2
	calls	#04,(r2)
	movl	#00000001,r11

l0001764D:
	tstl	@+08(ap)
	bneq	0001765A

l00017652:
	cmpl	r11,#00000001
	bgtr	0001765A

l00017657:
	movl	#0000000B,r11

l0001765A:
	movl	r11,r0
	ret
0001765E                                           D5 50               .P
00017660 7C 00                                           |.              

;; fn00017662: 00017662
;;   Called from:
;;     00013138 (in fn00012D86)
fn00017662 proc
	subl2	#00000004,sp
	tstl	+04(ap)
	bneq	0001766D

l0001766A:
	clrl	r0
	ret

l0001766D:
	movl	+04(ap),r4
	cmpl	r4,#001E8480
	bgtru	0001767D

l0001767A:
	brw	000176FC

l0001767D:
	clrl	r1
	movl	r4,r0
	movl	#000003E8,r2
	bgeq	00017694

l0001768B:
	cmpl	r2,r0
	bgtru	00017699

l00017690:
	incl	r1
	brb	00017699

l00017694:
	ediv	r2,r0,r1,r0

l00017699:
	movl	r1,r6
	movl	+08(ap),r3
	cmpl	r4,r3
	blssu	000176D0

l000176A5:
	subl3	r3,r4,r2
	extzv	#00000001,#1F,r6,r0
	addl2	r0,r2
	clrl	r1
	movl	r2,r0
	movl	r6,r2
	bgeq	000176C4
	cmpl	r2,r0
	bgtru	000176C9
	incl	r1
	brb	000176C9
	ediv	r2,r0,r1,r0
	movl	r1,r5
	brb	000176F8
	tstl	r0

l000176D0:
	subl2	r4,r3
	extzv	#00000001,#1F,r6,r0
	addl2	r0,r3
	clrl	r1
	movl	r3,r0
	movl	r6,r2
	bgeq	000176F0
	cmpl	r2,r0
	bgtru	000176F5
	incl	r1
	brb	000176F5
	tstl	r0
	ediv	r2,r0,r1,r0
	mnegl	r1,r5
	movl	r5,r0
	ret

l000176FC:
	movl	r4,r6
	movl	+08(ap),r3
	cmpl	r4,r3
	blssu	0001773C

l00017708:
	subl3	r3,r4,r2
	mull2	#000003E8,r2
	extzv	#00000001,#1F,r6,r0
	addl2	r0,r2
	clrl	r1
	movl	r2,r0
	movl	r6,r2
	bgeq	00017730
	cmpl	r2,r0
	bgtru	00017735
	incl	r1
	brb	00017735
	tstl	r0
	ediv	r2,r0,r1,r0
	movl	r1,r5
	brb	0001776C
	tstl	r0

l0001773C:
	subl2	r4,r3
	mull2	#000003E8,r3
	extzv	#00000001,#1F,r6,r0
	addl2	r0,r3
	clrl	r1
	movl	r3,r0
	movl	r6,r2
	bgeq	00017764
	cmpl	r2,r0
	bgtru	00017769
	incl	r1
	brb	00017769
	tstl	r0
	nop
	ediv	r2,r0,r1,r0
	mnegl	r1,r5
	movl	r5,r0
	ret
	prober	#00,+5E04(r2),@(sp)+

;; fn00017772: 00017772
;;   Called from:
;;     00013382 (in fn0001325E)
fn00017772 proc
	subl2	#00000004,sp
	movab	FFFE69A8,r3
	movab	FFFFB3D0,r2
	pushab	+05EB(r2)
	pushab	+0000C701(r2)
	calls	#02,00010890
	movl	r0,ap
	pushl	#00000000
	pushl	ap
	calls	#01,@000192C4
	pushl	r0
	pushl	ap
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	pushl	#00000000
	pushl	#00000001
	pushal	+12(r3)
	pushab	(r2)
	movl	+0000C839(r2),ap
	calls	#04,(ap)
	ret
000177C6                   00 00 FC 0F                         ....      

;; fn000177CA: 000177CA
;;   Called from:
;;     00017F37 (in fn00017DEE)
fn000177CA proc
	subl2	#0000000C,sp
	movab	FFFFB3D0,r6
	movl	+0000C831(r6),r4
	movl	+0000C835(r6),r3
	movl	+0000C82D(r6),r8
	movl	+0C(ap),r5
	movzwl	00007CA4[r5],(sp)
	movl	+0010(ap),r5
	movzwl	00007CA4[r5],+0004(sp)
	cmpl	r3,+0C(ap)
	bgequ	00017852

l0001780A:
	tstl	r0

l0001780C:
	decl	+0000C5FB(r6)
	blss	00017828

l00017814:
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00017832
00017826                   D5 50                               .P        

l00017828:
	calls	#00,0000D698
	movl	r0,r7

l00017832:
	movl	r7,r5
	cmpl	r5,#FFFFFFFF
	bneq	00017842

l0001783E:
	movl	#00000001,r0
	ret

l00017842:
	ashl	r3,r5,r5
	bisl2	r5,r4
	addl2	#00000008,r3
	cmpl	r3,+0C(ap)
	blssu	0001780C

l00017852:
	mcoml	r4,r5
	bicl3	r5,(sp),r5
	mull2	#00000006,r5
	addl3	+04(ap),r5,r9
	movzbl	(r9),r2
	movl	r2,r5
	cmpl	r5,#00000010
	bgtru	0001786F

l0001786C:
	brw	00017906

l0001786F:
	movab	0000D698,r7
	tstl	r0
	cmpl	r2,#00000063
	bneq	00017885

l00017881:
	movl	#00000001,r0
	ret

l00017885:
	movzbl	+01(r9),r1
	movl	r1,r0
	subl3	r0,#00000020,r5
	extzv	r0,r5,r4,r4
	subl2	r1,r3
	subl2	#00000010,r2
	cmpl	r3,r2
	bgequ	000178E1
	decl	+0000C5FB(r6)
	blss	000178BC
	movl	+0000C5F7(r6),r0
	incl	+0000C5F7(r6)
	movzbl	(r0),r5
	brb	000178C2
	tstl	r0
	calls	#00,(r7)
	movl	r0,r5
	movl	r5,r0
	cmpl	r0,#FFFFFFFF
	bneq	000178D2
	movl	#00000001,r0
	ret
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,r2
	blssu	000178A0
	movzwl	00007CA4[r2],r5
	mcoml	r5,r5
	bicl3	r5,r4,r5
	mull2	#00000006,r5
	addl3	+02(r9),r5,r9
	movzbl	(r9),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	00017906
	brw	00017878

l00017906:
	movzbl	+01(r9),r0
	movl	r0,r7
	subl3	r7,#00000020,r5
	extzv	r7,r5,r4,r4
	subl2	r0,r3
	cmpl	r2,#00000010
	bneq	00017964
	movl	r8,r5
	incl	r8
	movb	+02(r9),+05EB(r6)[r5]
	cmpl	r8,#00008000
	beql	00017936
	brw	00017804
	tstl	+0000C6C5(r6)
	beql	00017950
	pushl	r8
	pushab	+05EB(r6)
	calls	#02,00010848
	brb	0001795F
	tstl	r0
	nop
	pushl	#00000000
	pushl	r8
	pushab	+05EB(r6)
	calls	#03,0000B070
	clrl	r8
	brw	00017804
	cmpl	r2,#0000000F
	bneq	0001796C
	brw	00017C14
	cmpl	r3,r2
	bgequ	000179B9
	tstl	r0
	nop
	decl	+0000C5FB(r6)
	blss	00017990
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	0001799A
	tstl	r0
	calls	#00,0000D698
	movl	r0,r7
	movl	r7,r5
	cmpl	r5,#FFFFFFFF
	bneq	000179AA
	movl	#00000001,r0
	ret
	ashl	r3,r5,r5
	bisl2	r5,r4
	addl2	#00000008,r3
	cmpl	r3,r2
	blssu	00017974
	movzwl	+02(r9),r7
	movzwl	00007CA4[r2],r5
	mcoml	r5,r5
	bicl3	r5,r4,r5
	addl3	r5,r7,r11
	subl3	r2,#00000020,r5
	extzv	r2,r5,r4,r4
	subl2	r2,r3
	cmpl	r3,+10(ap)
	bgequ	00017A2A
	tstl	r0
	decl	+0000C5FB(r6)
	blss	00017A00
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00017A0A
	tstl	r0
	calls	#00,0000D698
	movl	r0,r7
	movl	r7,r5
	cmpl	r5,#FFFFFFFF
	bneq	00017A1A
	movl	#00000001,r0
	ret
	ashl	r3,r5,r5
	bisl2	r5,r4
	addl2	#00000008,r3
	cmpl	r3,+10(ap)
	blssu	000179E4
	mcoml	r4,r5
	bicl3	r5,+04(sp),r5
	mull2	#00000006,r5
	addl3	+08(ap),r5,r9
	movzbl	(r9),r2
	movl	r2,r5
	cmpl	r5,#00000010
	bgtru	00017A48
	brw	00017ADE
	movab	0000D698,r7
	nop
	cmpl	r2,#00000063
	bneq	00017A5D
	movl	#00000001,r0
	ret
	movzbl	+01(r9),r1
	movl	r1,r0
	subl3	r0,#00000020,r5
	extzv	r0,r5,r4,r4
	subl2	r1,r3
	subl2	#00000010,r2
	cmpl	r3,r2
	bgequ	00017AB9
	decl	+0000C5FB(r6)
	blss	00017A94
	movl	+0000C5F7(r6),r0
	incl	+0000C5F7(r6)
	movzbl	(r0),r5
	brb	00017A9A
	tstl	r0
	calls	#00,(r7)
	movl	r0,r5
	movl	r5,r0
	cmpl	r0,#FFFFFFFF
	bneq	00017AAA
	movl	#00000001,r0
	ret
	ashl	r3,r0,r0
	bisl2	r0,r4
	addl2	#00000008,r3
	cmpl	r3,r2
	blssu	00017A78
	movzwl	00007CA4[r2],r5
	mcoml	r5,r5
	bicl3	r5,r4,r5
	mull2	#00000006,r5
	addl3	+02(r9),r5,r9
	movzbl	(r9),r2
	movl	r2,r0
	cmpl	r0,#00000010
	blequ	00017ADE
	brw	00017A50
	movzbl	+01(r9),r0
	movl	r0,r7
	subl3	r7,#00000020,r5
	extzv	r7,r5,r4,r4
	subl2	r0,r3
	cmpl	r3,r2
	bgequ	00017B3D
	tstl	r0
	decl	+0000C5FB(r6)
	blss	00017B14
	movl	+0000C5F7(r6),r5
	incl	+0000C5F7(r6)
	movzbl	(r5),r7
	brb	00017B1E
	tstl	r0
	calls	#00,0000D698
	movl	r0,r7
	movl	r7,r5
	cmpl	r5,#FFFFFFFF
	bneq	00017B2E
	movl	#00000001,r0
	ret
	ashl	r3,r5,r5
	bisl2	r5,r4
	addl2	#00000008,r3
	cmpl	r3,r2
	blssu	00017AF8
	movzwl	+02(r9),r5
	subl3	r5,r8,r7
	movzwl	00007CA4[r2],r5
	mcoml	r5,r5
	bicl3	r5,r4,r5
	subl3	r5,r7,r10
	subl3	r2,#00000020,r5
	extzv	r2,r5,r4,r4
	subl2	r2,r3
	bicl2	#FFFF8000,r10
	movl	r10,r0
	cmpl	r0,r8
	blequ	00017B78
	movl	r10,r5
	brb	00017B7B
	movl	r8,r5
	subl3	r5,#00008000,r2
	movl	r2,r0
	cmpl	r0,r11
	blequ	00017B90
	movl	r11,r1
	brb	00017B93
	movl	r2,r1
	movl	r1,r2
	subl2	r2,r11
	subl3	r10,r8,r0
	cmpl	r0,r2
	blssu	00017BC0
	pushl	r2
	movab	+05EB(r6),r0
	addl3	r10,r0,-(sp)
	addl3	r8,r0,-(sp)
	calls	#03,@0001929C
	addl2	r2,r8
	addl2	r2,r10
	brb	00017BD7
	movl	r8,r1
	incl	r8
	movl	r10,r0
	incl	r10
	movb	+05EB(r6)[r0],+05EB(r6)[r1]
	decl	r2
	bneq	00017BC0
	cmpl	r8,#00008000
	bneq	00017C09
	tstl	+0000C6C5(r6)
	beql	00017BF8
	pushl	r8
	pushab	+05EB(r6)
	calls	#02,00010848
	brb	00017C07
	nop
	pushl	#00000000
	pushl	r8
	pushab	+05EB(r6)
	calls	#03,0000B070
	clrl	r8
	tstl	r11
	beql	00017C10
	brw	00017B64
	brw	00017804
	nop
	movl	r8,+0000C82D(r6)
	movl	r4,+0000C831(r6)
	movl	r3,+0000C835(r6)
	clrl	r0
	ret
	xfc
	halt
	subl2	#00000004,sp
	movab	FFFFB3D0,r4
	movl	+0000C831(r4),r2
	movl	+0000C835(r4),ap
	movl	+0000C82D(r4),r6
	bicl3	#FFFFFFF8,ap,r7
	subl3	r7,#00000020,r3
	extzv	r7,r3,r2,r2
	subl2	r7,ap
	cmpl	ap,#00000010
	bgequ	00017CAD
	tstl	r0
	decl	+0000C5FB(r4)
	blss	00017C84
	movl	+0000C5F7(r4),r3
	incl	+0000C5F7(r4)
	movzbl	(r3),r5
	brb	00017C8E
	tstl	r0
	calls	#00,0000D698
	movl	r0,r5
	movl	r5,r3
	cmpl	r3,#FFFFFFFF
	bneq	00017C9E
	movl	#00000001,r0
	ret
	ashl	ap,r3,r3
	bisl2	r3,r2
	addl2	#00000008,ap
	cmpl	ap,#00000010
	blssu	00017C68
	bicl3	#FFFF0000,r2,r7
	extzv	#00000010,#10,r2,r2
	subl2	#00000010,ap
	cmpl	ap,#00000010
	bgequ	00017D09
	tstl	r0
	decl	+0000C5FB(r4)
	blss	00017CE0
	movl	+0000C5F7(r4),r3
	incl	+0000C5F7(r4)
	movzbl	(r3),r5
	brb	00017CEA
	tstl	r0
	calls	#00,0000D698
	movl	r0,r5
	movl	r5,r3
	cmpl	r3,#FFFFFFFF
	bneq	00017CFA
	movl	#00000001,r0
	ret
	ashl	ap,r3,r3
	bisl2	r3,r2
	addl2	#00000008,ap
	cmpl	ap,#00000010
	blssu	00017CC4
	mcoml	r2,r3
	bicl2	#FFFF0000,r3
	cmpl	r7,r3
	beql	00017D1C
	movl	#00000001,r0
	ret
	extzv	#00000010,#10,r2,r2
	subl2	#00000010,ap
	movl	r7,r3
	decl	r7
	tstl	r3
	bneq	00017D30
	brw	00017DD1
	movab	0000D698,r5
	nop
	cmpl	ap,#00000008
	bgequ	00017D81
	tstl	r0
	nop
	decl	+0000C5FB(r4)
	blss	00017D5C
	movl	+0000C5F7(r4),r0
	incl	+0000C5F7(r4)
	movzbl	(r0),r3
	brb	00017D62
	tstl	r0
	calls	#00,(r5)
	movl	r0,r3
	movl	r3,r0
	cmpl	r0,#FFFFFFFF
	bneq	00017D72
	movl	#00000001,r0
	ret
	ashl	ap,r0,r0
	bisl2	r0,r2
	addl2	#00000008,ap
	cmpl	ap,#00000008
	blssu	00017D40
	movl	r6,r3
	incl	r6
	movb	r2,+05EB(r4)[r3]
	cmpl	r6,#00008000
	bneq	00017DBD
	tstl	+0000C6C5(r4)
	beql	00017DAC
	pushl	r6
	pushab	+05EB(r4)
	calls	#02,00010848
	brb	00017DBB
	pushl	#00000000
	pushl	r6
	pushab	+05EB(r4)
	calls	#03,0000B070
	clrl	r6
	extzv	#00000008,#18,r2,r2
	subl2	#00000008,ap
	movl	r7,r3
	decl	r7
	tstl	r3
	beql	00017DD1
	brw	00017D38
	movl	r6,+0000C82D(r4)
	movl	r2,+0000C831(r4)
	movl	ap,+0000C835(r4)
	clrl	r0
	ret
	tstl	r0
	nop
	Invalid

;; fn00017DEE: 00017DEE
fn00017DEE proc
	movab	-0488(sp),sp
	movab	FFFFB3D0,r3
	movab	FFFFB014,r5
	tstl	+0000C81D(r3)
	beql	00017E0C

l00017E09:
	brw	00017F1F

l00017E0C:
	clrl	r0
	moval	-0484(fp),r2
	nop

l00017E14:
	movl	#00000008,(r2)
	addl2	#00000004,r2
	aobleq	#0000008F,r0,00017E14

l00017E22:
	cmpl	r0,#00000100
	bgeq	00017E3D

l00017E2B:
	nop

l00017E2C:
	movl	#00000009,-0484(fp)[r0]
	incl	r0
	cmpl	r0,#00000100
	blss	00017E2C

l00017E3D:
	cmpl	r0,#00000118
	bgeq	00017E59

l00017E46:
	tstl	r0

l00017E48:
	movl	#00000007,-0484(fp)[r0]
	incl	r0
	cmpl	r0,#00000118
	blss	00017E48

l00017E59:
	cmpl	r0,#00000120
	bgeq	00017E75

l00017E62:
	tstl	r0

l00017E64:
	movl	#00000008,-0484(fp)[r0]
	incl	r0
	cmpl	r0,#00000120
	blss	00017E64

l00017E75:
	movl	#00000007,+0000C825(r3)
	pushab	+0000C825(r3)
	pushab	+0000C81D(r3)
	pushal	+008A(r5)
	pushal	+4C(r5)
	movzwl	#0101,-(sp)
	movzwl	#0120,-(sp)
	pushal	-0484(fp)
	calls	#07,0001878C
	tstl	r0
	beql	00017EAF

l00017EA8:
	clrl	+0000C81D(r3)
	ret

l00017EAF:
	clrl	r0
	moval	-00000484(fp),r2

l00017EB8:
	movl	#00000005,(r2)
	addl2	#00000004,r2
	aobleq	#0000001D,r0,00017EB8

l00017EC2:
	movl	#00000005,+0000C829(r3)
	pushab	+0000C829(r3)
	pushab	+0000C821(r3)
	pushal	+0104(r5)
	pushal	+00C8(r5)
	pushl	#00000000
	pushl	#0000001E
	pushal	-0484(fp)
	calls	#07,0001878C
	movl	r0,r6
	cmpl	r6,#00000001
	bleq	00017F1F

l00017EF4:
	movl	+0000C81D(r3),r4
	movl	r4,ap
	beql	00017F15

l00017F00:
	subl2	#00000006,ap
	movl	+02(ap),r2
	pushl	ap
	calls	#01,@000192B0
	movl	r2,ap
	bneq	00017F00

l00017F15:
	clrl	+0000C81D(r3)
	movl	r6,r0
	ret

l00017F1F:
	pushl	+0000C829(r3)
	pushl	+0000C825(r3)
	pushl	+0000C821(r3)
	pushl	+0000C81D(r3)
	calls	#04,000177C8
	clrl	r2
	tstl	r0
	beql	00017F46

l00017F44:
	incl	r2

l00017F46:
	movl	r2,r0
	ret
00017F4A                               D5 50 FC 0F 9E CE           .P....
00017F50 C0 FA 5E 9E EF 77 34 FE FF 55 9E EF B4 30 FE FF ..^..w4..U...0..
00017F60 5B D0 E5 31 C8 00 00 53 D0 E5 35 C8 00 00 52 D1 [..1...S..5...R.
00017F70 52 05 1E 45 D7 E5 FB C5 00 00 19 14 D0 E5 F7 C5 R..E............
00017F80 00 00 54 D6 E5 F7 C5 00 00 9A 64 56 11 0C D5 50 ..T.......dV...P
00017F90 FB 00 EF 01 57 FF FF D0 50 56 D0 56 54 D1 54 8F ....W...PV.VT.T.
00017FA0 FF FF FF FF 12 04 D0 01 50 04 78 52 54 54 C8 54 ........P.xRTT.T
00017FB0 53 C0 08 52 D1 52 05 1F BB CB 8F E0 FF FF FF 53 S..R.R.........S
00017FC0 54 C1 8F 01 01 00 00 54 AE 20 EF 05 1B 53 53 C2 T......T. ...SS.
00017FD0 05 52 D1 52 05 1E 46 01 D7 E5 FB C5 00 00 19 14 .R.R..F.........
00017FE0 D0 E5 F7 C5 00 00 54 D6 E5 F7 C5 00 00 9A 64 56 ......T.......dV
00017FF0 11 0C D5 50 FB 00 EF 9D 56 FF FF D0 50 56 D0 56 ...P....V...PV.V
00018000 54 D1 54 8F FF FF FF FF 12 04 D0 01 50 04 78 52 T.T.........P.xR
00018010 54 54 C8 54 53 C0 08 52 D1 52 05 1F BB CB 8F E0 TT.TS..R.R......
00018020 FF FF FF 53 54 C1 01 54 AE 1C EF 05 1B 53 53 C2 ...ST..T.....SS.
00018030 05 52 D1 52 04 1E 46 01 D7 E5 FB C5 00 00 19 14 .R.R..F.........
00018040 D0 E5 F7 C5 00 00 54 D6 E5 F7 C5 00 00 9A 64 56 ......T.......dV
00018050 11 0C D5 50 FB 00 EF 3D 56 FF FF D0 50 56 D0 56 ...P...=V...PV.V
00018060 54 D1 54 8F FF FF FF FF 12 04 D0 01 50 04 78 52 T.T.........P.xR
00018070 54 54 C8 54 53 C0 08 52 D1 52 04 1F BB CB 8F F0 TT.TS..R.R......
00018080 FF FF FF 53 58 C0 04 58 EF 04 1C 53 53 C2 04 52 ...SX..X...SS..R
00018090 D1 AE 20 8F 20 01 00 00 1A 06 D1 AE 1C 20 1B 04 .. . ........ ..
000180A0 D0 01 50 04 D4 57 D5 58 13 71 9E EF E8 55 FF FF ..P..W.X.q...U..
000180B0 56 D5 50 01 D1 52 03 1E 44 D5 50 01 D7 E5 FB C5 V.P..R..D.P.....
000180C0 00 00 19 14 D0 E5 F7 C5 00 00 5C D6 E5 F7 C5 00 ..........\.....
000180D0 00 9A 6C 54 11 08 D5 50 FB 00 66 D0 50 54 D0 54 ..lT...P..f.PT.T
000180E0 5C D1 5C 8F FF FF FF FF 12 04 D0 01 50 04 78 52 \.\.........P.xR
000180F0 5C 5C C8 5C 53 C0 08 52 D1 52 03 1F BF D0 47 6B \\.\S..R.R....Gk
00018100 54 CB 8F F8 FF FF FF 53 44 CD EC FA EF 03 1D 53 T......SD......S
00018110 53 C2 03 52 D6 57 D1 57 58 1F 99 D1 57 13 1E 10 S..R.W.WX...W...
00018120 D0 47 6B 54 D4 44 CD EC FA D6 57 D1 57 13 1F F0 .GkT.D....W.W...
00018130 D0 07 AD F0 DF AD F0 DF AD F8 7C 7E DD 13 DD 13 ..........|~....
00018140 DF CD EC FA FB 07 EF 41 06 00 00 D0 50 58 D5 AD .......A....PX..
00018150 F0 12 03 D0 01 58 D5 58 13 27 D1 58 01 12 1E D0 .....X.X.'.X....
00018160 AD F8 56 D0 56 5C 13 15 C2 06 5C D0 AC 02 54 DD ..V.V\....\...T.
00018170 5C FB 01 FF 38 11 00 00 D0 54 5C 12 EB D0 58 50 \...8....T\...XP
00018180 04 C1 AE 20 AE 1C 5A D0 AD F0 54 3C 44 EF 12 FB ... ..Z...T<D...
00018190 FE FF AE 24 D4 59 D4 56 D5 5A 12 03 31 52 02 9E ...$.Y.V.Z..1R..
000181A0 EF F3 54 FF FF 58 D5 50 D1 52 AD F0 1E 44 D5 50 ..T..X.P.R...D.P
000181B0 D7 E5 FB C5 00 00 19 14 D0 E5 F7 C5 00 00 5C D6 ..............\.
000181C0 E5 F7 C5 00 00 9A 6C 54 11 08 D5 50 FB 00 68 D0 ......lT...P..h.
000181D0 50 54 D0 54 5C D1 5C 8F FF FF FF FF 12 04 D0 01 PT.T\.\.........
000181E0 50 04 78 52 5C 5C C8 5C 53 C0 08 52 D1 52 AD F0 P.xR\\.\S..R.R..
000181F0 1F BE D2 53 54 CB 54 AE 24 54 C4 06 54 C1 AD F8 ...ST.T.$T..T...
00018200 54 AD F4 D0 AD F4 50 9A A0 01 57 C3 57 20 54 EF T.....P...W.W T.
00018210 57 54 53 53 C2 57 52 3C A0 02 57 D1 57 10 1E 14 WTSS.WR<..W.W...
00018220 D0 56 54 D6 56 D0 57 59 D0 59 44 CD EC FA 31 B8 .VT.V.WY.YD...1.
00018230 01 D5 50 01 D1 57 10 13 03 31 90 00 D1 52 02 1E ..P..W...1...R..
00018240 44 D5 50 01 D7 E5 FB C5 00 00 19 14 D0 E5 F7 C5 D.P.............
00018250 00 00 5C D6 E5 F7 C5 00 00 9A 6C 54 11 08 D5 50 ..\.......lT...P
00018260 FB 00 68 D0 50 54 D0 54 5C D1 5C 8F FF FF FF FF ..h.PT.T\.\.....
00018270 12 04 D0 01 50 04 78 52 5C 5C C8 5C 53 C0 08 52 ....P.xR\\.\S..R
00018280 D1 52 02 1F BF CB 8F FC FF FF FF 53 57 C0 03 57 .R.........SW..W
00018290 EF 02 1E 53 53 C2 02 52 C1 56 57 5C D1 5C 5A 1B ...SS..R.VW\.\Z.
000182A0 04 D0 01 50 04 D0 57 54 D7 57 D5 54 12 03 31 38 ...P..WT.W.T..18
000182B0 01 D5 50 01 D0 56 5C D6 56 D0 59 4C CD EC FA D0 ..P..V\.V.YL....
000182C0 57 5C D7 57 D5 5C 12 EC 31 1E 01 01 D1 57 11 13 W\.W.\..1....W..
000182D0 03 31 8C 00 D1 52 03 1E 44 D5 50 01 D7 E5 FB C5 .1...R..D.P.....
000182E0 00 00 19 14 D0 E5 F7 C5 00 00 5C D6 E5 F7 C5 00 ..........\.....
000182F0 00 9A 6C 54 11 08 D5 50 FB 00 68 D0 50 54 D0 54 ..lT...P..h.PT.T
00018300 5C D1 5C 8F FF FF FF FF 12 04 D0 01 50 04 78 52 \.\.........P.xR
00018310 5C 5C C8 5C 53 C0 08 52 D1 52 03 1F BF CB 8F F8 \\.\S..R.R......
00018320 FF FF FF 53 57 C0 03 57 EF 03 1D 53 53 C2 03 52 ...SW..W...SS..R
00018330 C1 56 57 5C D1 5C 5A 1B 04 D0 01 50 04 D0 57 54 .VW\.\Z....P..WT
00018340 D7 57 D5 54 13 15 D5 50 D0 56 5C D6 56 D4 4C CD .W.T...P.V\.V.L.
00018350 EC FA D0 57 5C D7 57 D5 5C 12 ED D4 59 31 89 00 ...W\.W.\...Y1..
00018360 D1 52 07 1E 44 D5 50 01 D7 E5 FB C5 00 00 19 14 .R..D.P.........
00018370 D0 E5 F7 C5 00 00 5C D6 E5 F7 C5 00 00 9A 6C 54 ......\.......lT
00018380 11 08 D5 50 FB 00 68 D0 50 54 D0 54 5C D1 5C 8F ...P..h.PT.T\.\.
00018390 FF FF FF FF 12 04 D0 01 50 04 78 52 5C 5C C8 5C ........P.xR\\.\
000183A0 53 C0 08 52 D1 52 07 1F BF CB 8F 80 FF FF FF 53 S..R.R.........S
000183B0 57 C0 0B 57 EF 07 19 53 53 C2 07 52 C1 56 57 5C W..W...SS..R.VW\
000183C0 D1 5C 5A 1B 04 D0 01 50 04 D0 57 54 D7 57 D5 54 .\Z....P..WT.W.T
000183D0 13 15 D5 50 D0 56 5C D6 56 D4 4C CD EC FA D0 57 ...P.V\.V.L....W
000183E0 5C D7 57 D5 5C 12 ED D4 59 D1 56 5A 1E 03 31 B7 \.W.\...Y.VZ..1.
000183F0 FD D0 AD F8 56 D0 56 5C 13 17 D5 50 C2 06 5C D0 ....V.V\...P..\.
00018400 AC 02 54 DD 5C FB 01 FF A4 0E 00 00 D0 54 5C 12 ..T.\........T\.
00018410 EB D0 53 E5 31 C8 00 00 D0 52 E5 35 C8 00 00 D0 ..S.1....R.5....
00018420 CB 40 01 AD F0 DF AD F0 DF AD F8 DF CB 8A 00 DF .@..............
00018430 AB 4C 3C 8F 01 01 7E DD AE 34 DF CD EC FA FB 07 .L<...~..4......
00018440 EF 47 03 00 00 D0 50 54 D5 AD F0 12 03 D0 01 54 .G....PT.......T
00018450 D5 54 13 45 D1 54 01 12 3C D5 A5 40 12 16 DD 01 .T.E.T..<..@....
00018460 DD 15 DF EF 54 E5 FC FF 9F 65 D0 E5 39 C8 00 00 ....T....e..9...
00018470 52 FB 04 62 D0 AD F8 53 D0 53 5C 13 18 D5 50 01 R..b...S.S\...P.
00018480 C2 06 5C D0 AC 02 52 DD 5C FB 01 FF 20 0E 00 00 ..\...R.\... ...
00018490 D0 52 5C 12 EB D0 54 50 04 D0 CB 44 01 AD EC DF .R\...TP...D....
000184A0 AD EC DF AD F4 DF CB 04 01 DF CB C8 00 DD 00 DD ................
000184B0 AE 30 DE CD EC FA 53 78 02 AE 38 52 C1 52 53 7E .0....Sx..8R.RS~
000184C0 FB 07 EF C5 02 00 00 D0 50 54 D5 AD EC 12 4A D1 ........PT....J.
000184D0 AE 20 8F 01 01 00 00 1B 40 D5 A5 40 12 16 DD 01 . ......@..@....
000184E0 DD 15 DF EF EA E4 FC FF 9F 65 D0 E5 39 C8 00 00 .........e..9...
000184F0 52 FB 04 62 D0 AD F8 53 D0 53 5C 13 18 D5 50 01 R..b...S.S\...P.
00018500 C2 06 5C D0 AC 02 52 DD 5C FB 01 FF A0 0D 00 00 ..\...R.\.......
00018510 D0 52 5C 12 EB D0 01 50 04 D1 54 01 12 02 D4 54 .R\....P..T....T
00018520 D5 54 13 25 D0 AD F8 53 D0 53 5C 13 18 D5 50 01 .T.%...S.S\...P.
00018530 C2 06 5C D0 AC 02 52 DD 5C FB 01 FF 70 0D 00 00 ..\...R.\...p...
00018540 D0 52 5C 12 EB D0 54 50 04 DD AD EC DD AD F0 DD .R\...TP........
00018550 AD F4 DD AD F8 FB 04 EF 6C F2 FF FF D5 50 13 04 ........l....P..
00018560 D0 01 50 04 D0 AD F8 53 D0 53 5C 13 18 D5 50 01 ..P....S.S\...P.
00018570 C2 06 5C D0 AC 02 52 DD 5C FB 01 FF 30 0D 00 00 ..\...R.\...0...
00018580 D0 52 5C 12 EB D0 AD F4 52 D0 52 5C 13 1B D5 50 .R\.....R.R\...P
00018590 C2 06 5C D0 AC 02 CD E8 FA DD 5C FB 01 FF 0E 0D ..\.......\.....
000185A0 00 00 D0 CD E8 FA 5C 12 E7 D4 50 04 7C 00       ......\...P.|.  

;; fn000185AE: 000185AE
;;   Called from:
;;     000186D5 (in fn000186B2)
fn000185AE proc
	subl2	#00000004,sp
	movab	FFFFB3D0,r4
	movl	+0000C831(r4),r3
	movl	+0000C835(r4),r2
	cmpl	r2,#00000001
	bgequ	00018611

l000185CB:
	nop

l000185CC:
	decl	+0000C5FB(r4)
	blss	000185E8

l000185D4:
	movl	+0000C5F7(r4),r0
	incl	+0000C5F7(r4)
	movzbl	(r0),r5
	brb	000185F2
000185E6                   D5 50                               .P        

l000185E8:
	calls	#00,0000D698
	movl	r0,r5

l000185F2:
	movl	r5,r0
	cmpl	r0,#FFFFFFFF
	bneq	00018602

l000185FE:
	movl	#00000001,r0
	ret

l00018602:
	ashl	r2,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r2
	cmpl	r2,#00000001
	blssu	000185CC

l00018611:
	bicl3	#FFFFFFFE,r3,@+04(ap)
	extzv	#00000001,#1F,r3,r3
	decl	r2
	cmpl	r2,#00000002
	bgequ	0001866D
	tstl	r0
	decl	+0000C5FB(r4)
	blss	00018644
	movl	+0000C5F7(r4),r0
	incl	+0000C5F7(r4)
	movzbl	(r0),r5
	brb	0001864E
	tstl	r0
	calls	#00,0000D698
	movl	r0,r5
	movl	r5,r0
	cmpl	r0,#FFFFFFFF
	bneq	0001865E
	movl	#00000001,r0
	ret
	ashl	r2,r0,r0
	bisl2	r0,r3
	addl2	#00000008,r2
	cmpl	r2,#00000002
	blssu	00018628
	bicl3	#FFFFFFFC,r3,r6
	extzv	#00000002,#1E,r3,r3
	subl2	#00000002,r2
	movl	r3,+0000C831(r4)
	movl	r2,+0000C835(r4)
	cmpl	r6,#00000002
	bneq	00018696
	calls	#00,00017F4C
	ret
	tstl	r6
	bneq	000186A0
	calls	#00,00017C2C
	ret
	cmpl	r6,#00000001
	bneq	000186AB
	calls	#00,00017DEC
	ret
	cvtbl	#02,r0
	ret
	prober	#00,+5E08(r2),@(sp)+

;; fn000186B2: 000186B2
fn000186B2 proc
	subl2	#00000008,sp
	movab	FFFFB3D0,r2
	clrl	+0000C82D(r2)
	clrq	+0000C831(r2)
	clrl	r3
	tstl	r0

l000186CC:
	clrl	+0000C819(r2)
	pushal	-08(fp)
	calls	#01,000185AC
	tstl	r0
	beql	000186DF

l000186DE:
	ret

l000186DF:
	cmpl	+0000C819(r2),r3
	blequ	000186EF

l000186E8:
	movl	+0000C819(r2),r3

l000186EF:
	tstl	-08(fp)
	beql	000186CC

l000186F4:
	tstl	+0000C6C5(r2)
	beql	00018710

l000186FC:
	pushl	+0000C82D(r2)
	pushab	+05EB(r2)
	calls	#02,00010848
	brb	00018723
0001870F                                              01                .

l00018710:
	pushl	#00000000
	pushl	+0000C82D(r2)
	pushab	+05EB(r2)
	calls	#03,0000B070

l00018723:
	clrl	r0
	ret
00018726                   D5 50 1C 00                         .P..      

;; fn0001872A: 0001872A
;;   Called from:
;;     00010D63 (in fn000108D2)
fn0001872A proc
	subl2	#0000000C,sp
	movab	FFFFB3D0,r3
	tstl	+0000C81D(r3)
	beql	00018787

l0001873C:
	movl	+0000C821(r3),r4
	movl	r4,ap
	beql	0001875D

l00018748:
	subl2	#00000006,ap
	movl	+02(ap),r2
	pushl	ap
	calls	#01,@000192B0
	movl	r2,ap
	bneq	00018748

l0001875D:
	movl	+0000C81D(r3),r4
	movl	r4,ap
	beql	00018781

l00018769:
	tstl	r0
	nop

l0001876C:
	subl2	#00000006,ap
	movl	+02(ap),r2
	pushl	ap
	calls	#01,@000192B0
	movl	r2,ap
	bneq	0001876C

l00018781:
	clrq	+0000C81D(r3)

l00018787:
	clrl	r0
	ret
0001878A                               D5 50 FC 0F                 .P..  

;; fn0001878E: 0001878E
;;   Called from:
;;     00016828 (in fn000167AA)
;;     00016875 (in fn000167AA)
;;     000168D7 (in fn000167AA)
;;     00016940 (in fn000167AA)
;;     000169D8 (in fn000167AA)
;;     00016A30 (in fn000167AA)
;;     00016A88 (in fn000167AA)
;;     00017E9D (in fn00017DEE)
;;     00017EE5 (in fn00017DEE)
fn0001878E proc
	movab	-05D0(sp),sp
	moval	-008C(fp),r2
	addl3	#00000004,r2,+24(sp)
	cmpl	+08(ap),#00000100
	blequ	000187B4

l000187A7:
	movl	+04(ap),r2
	movl	+0400(r2),r3
	brb	000187B7
000187B2       D5 50                                       .P            

l000187B4:
	movl	#00000010,r3

l000187B7:
	movl	r3,+1C(sp)
	movzbl	#44,-(sp)
	pushl	#00000000
	pushal	-48(fp)
	calls	#03,@0001927C
	movl	+04(ap),r2
	movl	+0008(ap),r11

l000187D4:
	movl	(r2),r3
	incl	-48(fp)[r3]
	addl2	#00000004,r2
	decl	r11
	bneq	000187D4

l000187E2:
	cmpl	-48(fp),+08(ap)
	bneq	000187F2

l000187E9:
	clrl	@+18(ap)
	clrl	@+1C(ap)
	clrl	r0
	ret

l000187F2:
	cvtwl	#0001,r10
	nop

l000187F8:
	tstl	-48(fp)[r10]
	bneq	00018805

l000187FE:
	incl	r10
	cmpl	r10,#00000010
	blequ	000187F8

l00018805:
	movl	r10,r9
	movl	+1C(ap),r2
	cmpl	(r2),r10
	bgequ	00018814

l00018811:
	movl	r10,(r2)

l00018814:
	movl	#00000010,r11
	beql	00018826

l00018819:
	tstl	r0
	nop

l0001881C:
	tstl	-48(fp)[r11]
	bneq	00018826

l00018822:
	decl	r11
	bneq	0001881C

l00018826:
	movl	r11,+0C(sp)
	movl	+1C(ap),r2
	cmpl	(r2),r11
	blequ	00018836

l00018833:
	movl	r11,(r2)

l00018836:
	ashl	r10,#00000001,+04(sp)
	cmpl	r10,r11
	bgequ	00018859

l00018840:
	subl2	-48(fp)[r10],+04(sp)
	bgeq	0001884C

l00018848:
	movl	#00000002,r0
	ret

l0001884C:
	incl	r10
	ashl	#01,+04(sp),+04(sp)
	cmpl	r10,r11
	blssu	00018840

l00018859:
	subl2	-48(fp)[r11],+04(sp)
	bgeq	00018865

l00018861:
	movl	#00000002,r0
	ret

l00018865:
	addl2	+04(sp),-48(fp)[r11]
	clrl	r10
	clrl	-058C(fp)
	moval	-48(fp),r3
	addl3	#00000004,r3,r2
	moval	-0590(fp),r3
	addl3	#00000008,r3,r5
	decl	r11
	beql	00018892

l00018886:
	tstl	r0

l00018888:
	addl2	(r2)+,r10
	movl	r10,(r5)+
	decl	r11
	bneq	00018888

l00018892:
	movzwl	#0480,-(sp)
	pushl	#00000000
	pushal	-054C(fp)
	calls	#03,@0001927C
	movl	+0004(ap),r2
	clrl	r11
	nop

l000188AC:
	movl	(r2)+,r10
	beql	000188C2

l000188B1:
	movl	-0590(fp)[r10],r3
	incl	-0590(fp)[r10]
	movl	r11,-054C(fp)[r3]

l000188C2:
	incl	r11
	cmpl	r11,+08(ap)
	blssu	000188AC

l000188CA:
	moval	+08(ap),+08(sp)
	movl	+0C(sp),r2
	movl	-0590(fp)[r2],@+08(sp)
	clrl	r11
	clrl	-0590(fp)
	moval	-054C(fp),r8
	mnegl	#00000001,+30(sp)
	movl	+24(sp),r2
	clrl	-04(r2)
	clrl	r6
	clrl	-00CC(fp)
	clrl	r7
	clrl	+2C(sp)
	cmpl	r9,+0C(sp)
	bleq	00018904

l00018901:
	brw	00018BDA

l00018904:
	moval	+1C(ap),+28(sp)
	movab	@000192B4,+00000018(sp)

l00018914:
	movl	-48(fp)[r9],+10(sp)
	movl	+10(sp),r2
	decl	+10(sp)
	tstl	r2
	bneq	00018928

l00018925:
	brw	00018BCF

l00018928:
	movl	+30(sp),r2
	addl3	@+24(sp)[r2],r6,r2
	cmpl	r9,r2
	bgtr	0001893A

l00018937:
	brw	00018AB5

l0001893A:
	addl3	#00000001,+10(sp),+20(sp)

l00018940:
	movl	+30(sp),r2
	incl	+30(sp)
	addl2	@+24(sp)[r2],r6
	subl3	r6,+0C(sp),+2C(sp)
	movl	+2C(sp),r2
	movl	+28(sp),r3
	cmpl	r2,@+00(r3)
	blequ	00018968

l00018960:
	movl	@+00(r3),r4
	brb	0001896C
00018966                   D5 50                               .P        

l00018968:
	movl	+2C(sp),r4

l0001896C:
	movl	r4,+2C(sp)
	subl3	r6,r9,r10
	movl	r10,r2
	ashl	r2,#00000001,+34(sp)
	movl	+34(sp),r2
	cmpl	r2,+20(sp)
	blequ	000189C2

l00018986:
	addl3	#00000001,+10(sp),r2
	subl2	r2,+34(sp)
	moval	-48(fp),r4
	ashl	#02,r9,r3
	addl2	r4,r3
	incl	r10
	cmpl	r10,+2C(sp)
	bgequ	000189C2

l000189A2:
	tstl	r0

l000189A4:
	ashl	#01,+34(sp),+34(sp)
	movl	+34(sp),r2
	addl2	#00000004,r3
	cmpl	r2,(r3)
	blequ	000189C2

l000189B6:
	subl2	(r3),+34(sp)
	incl	r10
	cmpl	r10,+2C(sp)
	blssu	000189A4

l000189C2:
	addl3	r10,r6,r2
	cmpl	r2,+1C(sp)
	blequ	000189D7

l000189CC:
	cmpl	r6,+1C(sp)
	bgequ	000189D7

l000189D2:
	subl3	r6,+1C(sp),r10

l000189D7:
	ashl	r10,#00000001,+2C(sp)
	movl	+30(sp),r2
	movl	r10,@+24(sp)[r2]
	addl3	#00000001,+2C(sp),r2
	mull3	#00000006,r2,-(sp)
	calls	#01,@+1C(sp)
	movl	r0,r7
	bneq	00018A21

l000189F7:
	tstl	+30(sp)
	beql	00018A1D

l000189FC:
	movl	-00CC(fp),r4
	movl	r4,r2
	beql	00018A1D

l00018A06:
	tstl	r0

l00018A08:
	subl2	#00000006,r2
	movl	+02(r2),r3
	pushl	r2
	calls	#01,@000192B0
	movl	r3,r2
	bneq	00018A08

l00018A1D:
	movl	#00000003,r0
	ret

l00018A21:
	addl3	#00000001,+2C(sp),r3
	addl2	00007BE9,r3
	movab	00007BE9,r2
	movl	r3,(r2)
	addl3	#00000006,r7,@+18(ap)
	movab	+02(r7),+18(ap)
	clrl	@+18(ap)
	addl2	#00000006,r7
	movl	+30(sp),r2
	movl	r7,-00CC(fp)[r2]
	tstl	r2
	beql	00018AA3

l00018A55:
	movl	r11,-0590(fp)[r2]
	subl3	#00000001,+30(sp),r4
	moval	@+24(sp)[r4],r3
	movb	(r3),-0595(fp)
	addl3	#00000010,r10,r2
	cvtlb	r2,-0596(fp)
	movl	r7,-0594(fp)
	subl3	(r3),r6,r3
	subl3	r3,#00000020,r5
	ashl	r6,#00000001,r2
	decl	r2
	mcoml	r2,r2
	bicl3	r2,r11,r2
	extzv	r3,r5,r2,r10
	mull3	#00000006,r10,r2
	movl	-00CC(fp)[r4],r3
	movc3	#0006,-0596(fp),(r3)[r2]

l00018AA3:
	movl	+30(sp),r2
	addl3	@+24(sp)[r2],r6,r2
	cmpl	r9,r2
	bleq	00018AB5

l00018AB2:
	brw	00018940

l00018AB5:
	subl3	r6,r9,r2
	cvtlb	r2,-0595(fp)
	moval	-054C(fp),r3
	ashl	#02,@+08(sp),r2
	addl2	r2,r3
	cmpl	r8,r3
	blssu	00018AD8

l00018AD0:
	movb	#63,-0596(fp)
	brb	00018B2A

l00018AD8:
	movl	(r8),r4
	movl	+0C(ap),r0
	cmpl	r4,r0
	bgequ	00018B0C

l00018AE4:
	cmpl	r4,#00000100
	bgequ	00018AF4

l00018AED:
	movb	#10,r5
	brb	00018AF7
00018AF2       D5 50                                       .P            

l00018AF4:
	movb	#0F,r5

l00018AF7:
	movb	r5,-0596(fp)
	movl	r8,r3
	addl2	#00000004,r8
	movw	(r3),-0594(fp)
	brb	00018B2A
00018B09                            D5 50 01                      .P.    

l00018B0C:
	subl2	r0,r4
	moval	@+14(ap),r2
	movaw	(r2)[r4],r2
	movb	(r2),-0596(fp)
	subl3	r0,(r8)+,r3
	moval	@+10(ap),r4
	movw	(r4)[r3],-0594(fp)

l00018B2A:
	subl3	r6,r9,r2
	ashl	r2,#00000001,+34(sp)
	subl3	r6,#00000020,r2
	extzv	r6,r2,r11,r10
	cmpl	r10,+2C(sp)
	bgequ	00018B59
	tstl	r0
	mull3	#00000006,r10,r2
	movc3	#0006,-0596(fp),(r7)[r2]
	addl2	+34(sp),r10
	cmpl	r10,+2C(sp)
	blssu	00018B44
	subl3	#00000001,r9,r2
	ashl	r2,#00000001,r10
	mcoml	r11,r2
	bicl3	r2,r10,r2
	beql	00018B7D
	tstl	r0
	xorl2	r10,r11
	extzv	#00000001,#1F,r10,r10
	mcoml	r11,r2
	bicl3	r2,r10,r2
	bneq	00018B6C
	xorl2	r10,r11
	ashl	r6,#00000001,r2
	decl	r2
	mcoml	r2,r2
	bicl3	r2,r11,r2
	movl	+30(sp),r3
	cmpl	r2,-0590(fp)[r3]
	beql	00018BC1
	tstl	r0
	nop
	decl	+30(sp)
	movl	+30(sp),r2
	subl2	@+24(sp)[r2],r6
	ashl	r6,#00000001,r2
	decl	r2
	mcoml	r2,r2
	bicl3	r2,r11,r2
	movl	+30(sp),r3
	cmpl	r2,-0590(fp)[r3]
	bneq	00018B9C
	movl	+10(sp),r2
	decl	+10(sp)
	tstl	r2
	beql	00018BCF
	brw	00018928

l00018BCF:
	incl	r9
	cmpl	r9,+0C(sp)
	bgtr	00018BDA

l00018BD7:
	brw	00018914

l00018BDA:
	movl	@+24(sp),@+1C(ap)
	clrb	r3
	tstl	+04(sp)
	beql	00018BEE

l00018BE6:
	cmpl	+0C(sp),#00000001
	beql	00018BEE

l00018BEC:
	incb	r3

l00018BEE:
	cvtbl	r3,r0
	ret
00018BF2       D5 50 0C 00                                 .P..          

;; fn00018BF6: 00018BF6
;;   Called from:
;;     0001683C (in fn000167AA)
;;     00016889 (in fn000167AA)
;;     00016893 (in fn000167AA)
;;     000168EB (in fn000167AA)
;;     000168F5 (in fn000167AA)
;;     000168FF (in fn000167AA)
;;     00016954 (in fn000167AA)
;;     0001695E (in fn000167AA)
;;     00016968 (in fn000167AA)
;;     000169EC (in fn000167AA)
;;     00016A44 (in fn000167AA)
;;     00016A4E (in fn000167AA)
;;     00016A9C (in fn000167AA)
;;     00016AA6 (in fn000167AA)
;;     00016AC8 (in fn000167AA)
;;     00016AD2 (in fn000167AA)
fn00018BF6 proc
	subl2	#00000004,sp
	movl	+04(ap),r2
	beql	00018C15

l00018BFF:
	nop

l00018C00:
	subl2	#00000006,r2
	movl	+02(r2),r3
	pushl	r2
	calls	#01,@000192B0
	movl	r3,r2
	bneq	00018C00

l00018C15:
	clrl	r0
	ret
00018C18                         00 00 C2 04 5E DD AC 0C         ....^...
00018C20 DD AC 04 DD AC 08 FB 03 AF 0E D4 51 D1 50 01 12 ...........Q.P..
00018C30 02 D6 51 D0 51 50 04 01 FC 0F                   ..Q.QP....      

;; fn00018C3A: 00018C3A
;;   Called from:
;;     00018C77 (in fn00018C3A)
;;     00018CA5 (in fn00018C3A)
;;     00018E34 (in fn00018C3A)
;;     00018E6B (in fn00018C3A)
;;     00018F0D (in fn00018C3A)
fn00018C3A proc
	subl2	#00000004,sp
	movab	@0001934C,r10
	movl	+04(ap),r2
	incl	+04(ap)
	movzbl	(r2),r4
	bneq	00018C60

l00018C50:
	movzbl	@+08(ap),r2
	clrl	r3
	tstl	r2
	bneq	00018C5C

l00018C5A:
	incl	r3

l00018C5C:
	movl	r3,r0
	ret

l00018C60:
	cmpl	r4,#0000003F
	bneq	00018C86

l00018C65:
	movl	+08(ap),r2
	tstb	(r2)
	beql	00018C80

l00018C6D:
	pushl	+0C(ap)
	incl	r2
	pushl	r2
	pushl	+04(ap)
	calls	#03,00018C38
	movl	r0,r2
	brb	00018C82

l00018C80:
	clrl	r2

l00018C82:
	movl	r2,r0
	ret

l00018C86:
	cmpl	r4,#0000002A
	bneq	00018CBE

l00018C8B:
	movzbl	@+04(ap),r2
	bneq	00018C95

l00018C91:
	movl	#00000001,r0
	ret

l00018C95:
	tstb	@+08(ap)
	beql	00018CBA

l00018C9A:
	tstl	r0

l00018C9C:
	pushl	+0C(ap)
	pushl	+08(ap)
	pushl	+04(ap)
	calls	#03,00018C38
	movl	r0,r4
	beql	00018CB2

l00018CAE:
	movl	r4,r0
	ret

l00018CB2:
	incl	+08(ap)
	tstb	@+08(ap)
	bneq	00018C9C

l00018CBA:
	movl	#00000002,r0
	ret

l00018CBE:
	cmpl	r4,#0000005B
	beql	00018CCA

l00018CC7:
	brw	00018E7E

l00018CCA:
	movzbl	@+08(ap),r2
	bneq	00018CD3

l00018CD0:
	clrl	r0
	ret

l00018CD3:
	movl	+04(ap),r5
	movb	#01,r6
	movzbl	(r5),r2
	cmpl	r2,#00000021
	beql	00018CED

l00018CE2:
	cmpl	r2,#0000005E
	beql	00018CED

l00018CEB:
	clrb	r6

l00018CED:
	cvtbl	r6,r8
	addl3	r5,r8,+04(ap)
	movl	+04(ap),r7
	clrl	r3
	tstb	(r7)
	beql	00018D2B

l00018CFF:
	nop

l00018D00:
	tstl	r3
	beql	00018D08

l00018D04:
	clrl	r3
	brb	00018D25

l00018D08:
	movzbl	(r7),r2
	cmpl	r2,#0000005C
	bneq	00018D1C

l00018D14:
	movl	#00000001,r3
	brb	00018D25
00018D19                            D5 50 01                      .P.    

l00018D1C:
	cmpl	r2,#0000005D
	beql	00018D2B

l00018D25:
	incl	r7
	tstb	(r7)
	bneq	00018D00

l00018D2B:
	movzbl	(r7),r2
	cmpl	r2,#0000005D
	beql	00018D3A

l00018D37:
	clrl	r0
	ret

l00018D3A:
	clrl	r4
	movl	+04(ap),r5
	movzbl	(r5),r2
	clrl	r3
	cmpl	r2,#0000002D
	bneq	00018D4C

l00018D4A:
	incl	r3

l00018D4C:
	movl	r3,r9
	cmpl	r5,r7
	blssu	00018D57

l00018D54:
	brw	00018E5B

l00018D57:
	moval	+000C(ap),r11
	movab	@00019298,r6
	nop

l00018D64:
	tstl	r9
	bneq	00018D7C

l00018D68:
	movzbl	@+04(ap),r2
	cmpl	r2,#0000005C
	bneq	00018D7C

l00018D75:
	movl	#00000001,r9
	brw	00018E4F
00018D7B                                  01                        .    

l00018D7C:
	tstl	r9
	bneq	00018D94

l00018D80:
	movl	+04(ap),r0
	movzbl	(r0),r2
	cmpl	r2,#0000002D
	bneq	00018D94

l00018D8C:
	movzbl	-(r0),r4
	brw	00018E4F
00018D92       D5 50                                       .P            

l00018D94:
	tstl	(r11)
	beql	00018DC0

l00018D98:
	movl	+08(ap),r2
	movzbl	(r2),r3
	bicl3	#FFFFFF80,r3,r2
	bbc	#00000000,(r10)[r2],00018DB8

l00018DAC:
	cvtbl	r3,-(sp)
	calls	#01,(r6)
	cvtlb	r0,r1
	brb	00018DBB
00018DB7                      01                                .        

l00018DB8:
	cvtlb	r3,r1

l00018DBB:
	movb	r1,r3
	brb	00018DC4

l00018DC0:
	movb	@+08(ap),r3

l00018DC4:
	movzbl	r3,r5
	movl	+04(ap),r1
	addl3	#00000001,r1,r2
	movzbl	(r2),r2
	cmpl	r2,#0000002D
	beql	00018E4B

l00018DD7:
	tstl	r4
	beql	00018DE0

l00018DDB:
	movl	r4,r0
	brb	00018DE3

l00018DE0:
	movzbl	(r1),r0

l00018DE3:
	movl	r0,r4
	movzbl	(r1),r2
	cmpl	r4,r2
	bgtru	00018E4B

l00018DEE:
	movl	+0000(r11),r3
	nop

l00018DF4:
	tstl	r3
	beql	00018E18

l00018DF8:
	bicl3	#FFFFFF80,r4,r0
	bbc	#00000000,(r10)[r0],00018E10

l00018E05:
	cvtbl	r4,-(sp)
	calls	#01,(r6)
	cvtlb	r0,r2
	brb	00018E13

l00018E10:
	cvtlb	r4,r2

l00018E13:
	cvtbl	r2,r1
	brb	00018E1B

l00018E18:
	movl	r4,r1

l00018E1B:
	cmpl	r1,r5
	bneq	00018E40

l00018E20:
	tstl	r8
	beql	00018E28

l00018E24:
	clrl	r2
	brb	00018E3C

l00018E28:
	pushl	+0C(ap)
	addl3	#00000001,+08(ap),-(sp)
	addl3	#00000001,r7,-(sp)
	calls	#03,00018C38
	movl	r0,r2

l00018E3C:
	movl	r2,r0
	ret

l00018E40:
	incl	r4
	movzbl	@+04(ap),r0
	cmpl	r4,r0
	blequ	00018DF4

l00018E4B:
	clrl	r9
	clrl	r4

l00018E4F:
	incl	+04(ap)
	cmpl	+04(ap),r7
	bgequ	00018E5B

l00018E58:
	brw	00018D64

l00018E5B:
	tstl	r8
	beql	00018E78

l00018E5F:
	pushl	+0C(ap)
	addl3	#00000001,+08(ap),-(sp)
	addl3	#00000001,r7,-(sp)
	calls	#03,00018C38
	movl	r0,r2
	brb	00018E7A
00018E75                D5 50 01                              .P.        

l00018E78:
	clrl	r2

l00018E7A:
	movl	r2,r0
	ret

l00018E7E:
	cmpl	r4,#0000005C
	bneq	00018E96

l00018E87:
	movl	+04(ap),r2
	incl	+04(ap)
	movzbl	(r2),r4
	bneq	00018E96

l00018E93:
	clrl	r0
	ret

l00018E96:
	tstl	+0C(ap)
	beql	00018EC4

l00018E9B:
	movzbl	r4,r5
	bicl3	#FFFFFF80,r5,r2
	bbc	#00000000,(r10)[r2],00018EBC

l00018EAB:
	cvtbl	r5,-(sp)
	calls	#01,@00019298
	cvtlb	r0,r3
	brb	00018EBF
00018EBA                               D5 50                       .P    

l00018EBC:
	cvtlb	r5,r3

l00018EBF:
	movb	r3,r5
	brb	00018EC7

l00018EC4:
	movb	r4,r5

l00018EC7:
	tstl	+0C(ap)
	beql	00018EF8

l00018ECC:
	movl	+08(ap),r2
	movzbl	(r2),r4
	bicl3	#FFFFFF80,r4,r2
	bbc	#00000000,(r10)[r2],00018EF0

l00018EE0:
	cvtbl	r4,-(sp)
	calls	#01,@00019298
	cvtlb	r0,r3
	brb	00018EF3
00018EEF                                              01                .

l00018EF0:
	cvtlb	r4,r3

l00018EF3:
	movb	r3,r4
	brb	00018EFC

l00018EF8:
	movb	@+08(ap),r4

l00018EFC:
	cmpb	r5,r4
	bneq	00018F18

l00018F01:
	pushl	+0C(ap)
	incl	+08(ap)
	pushl	+08(ap)
	pushl	+04(ap)
	calls	#03,00018C38
	movl	r0,r2
	brb	00018F1A
00018F17                      01                                .        

l00018F18:
	clrl	r2

l00018F1A:
	movl	r2,r0
	ret
00018F1E                                           D5 50               .P
00018F20 00 00                                           ..              

;; fn00018F22: 00018F22
;;   Called from:
;;     00010AC8 (in fn000108D2)
;;     00010B78 (in fn000108D2)
;;     00010E9A (in fn00010E6A)
fn00018F22 proc
	subl2	#00000004,sp
	tstb	@+04(ap)
	beql	00018F5E

l00018F2A:
	tstl	r0

l00018F2C:
	movl	+04(ap),r0
	movb	(r0),r1
	cmpb	r1,#5C
	bneq	00018F48

l00018F39:
	incl	r0
	tstb	(r0)
	beql	00018F48

l00018F3F:
	movl	r0,+04(ap)
	brb	00018F56
00018F45                D5 50 01                              .P.        

l00018F48:
	cmpb	r1,#25
	beql	00018F52

l00018F4D:
	cmpb	r1,#2A
	bneq	00018F56

l00018F52:
	movl	#00000001,r0
	ret

l00018F56:
	incl	+04(ap)
	tstb	@+04(ap)
	bneq	00018F2C

l00018F5E:
	clrl	r0
	ret
00018F61    00 00 00 3C 00                                ...<.          

;; fn00018F66: 00018F66
;;   Called from:
;;     00009135 (in fn00008EC2)
;;     0000915E (in fn00008EC2)
;;     00009216 (in fn00008EC2)
;;     00009243 (in fn00008EC2)
;;     0000950C (in fn00008EC2)
;;     00009617 (in fn00008EC2)
;;     0000975A (in fn00009746)
;;     000098CC (in fn000098B6)
fn00018F66 proc
	movab	-00A0(sp),sp
	movc5
	halt
	cvtld	#00000000,#6.0
	Invalid
	halt
	Invalid
	xorw3	@-600F5261(r8),-14(fp),@+04AC(fp)
	calls	#03,@00019320
	pushl	sp
	pushab	+08(sp)
	pushab	-1C(fp)
	calls	#03,00018FA2                                         ; @(pc)+
	bgeq	00018F82
	Invalid
	pushaq	#00000004
	Invalid

;; fn00018FA9: 00018FA9
;;   Called from:
;;     0000917B (in fn00008EC2)
;;     00009260 (in fn00008EC2)
;;     00009523 (in fn00008EC2)
;;     00009797 (in fn00009746)
;;     00009899 (in fn00009746)
fn00018FA9 proc
	movab	@00019320,r6
	movab	-00A8(sp),sp
	movc5
	halt
	cvtld	#00000000,#6.0
	Invalid
	halt
	Invalid
	xorw3	@-600F5261(r8),-14(fp),@+04AC(fp)
	calls	#03,(r6)
	pushl	sp
	pushab	+10(sp)
	pushab	-1C(fp)
	calls	#03,00018FE8                                         ; @(pc)+
	bgeq	00018FC8
	Invalid
	pushaq	@+5250(r0)
	pushab	-14(fp)
	pushl	+08(ap)
	calls	#02,@0001931C
	cmpb	(ap),#03
	blssu	00019025
	tstl	+0C(ap)
	beql	00019025
	pushab	+08(sp)
	pushab	+08(sp)
	pushl	+08(ap)
	calls	#03,(r6)
	cmpw	+04(sp),-14(fp)
	bgequ	00019020
	movw	+04(sp),@+0C(ap)
	brb	00019025
	movw	-14(fp),@+0C(ap)
	movl	r2,r0
	ret
	Invalid

;; fn0001902B: 0001902B
;;   Called from:
;;     00008F9A (in fn00008EC2)
fn0001902B proc
	movab	@00019320,r6
	movab	-00BC(sp),sp
	movc5
	halt
	cvtld	#00000000,#6.0
	xorw3	-7070(r8),r3,-38(fp)
	movab	@00019328,-28(fp)
	movab	@00019324,-24(fp)
	tstb	(ap)
	beql	0001906D
	tstl	+04(ap)
	beql	0001906D
	pushab	-04(fp)
	pushab	-08(fp)
	pushl	+04(ap)
	calls	#03,(r6)
	movab	-08(fp),-2C(fp)
	movl	+08(ap),-34(fp)
	cmpb	(ap),#03
	blssu	0001907C
	tstl	+0C(ap)
	bneq	00019086
	cmpb	(ap),#04
	blssu	00019096
	tstl	+10(ap)
	beql	00019096
	movc5
	halt
	cvtld	#00000000,#3.0
	xorw3	-1B52FB30(r4),@(sp)+,-1C(fp)
	xorw3	+1F036C91(r0),#000A,@+0CAC(r5)
	beql	000190A5
	movl	+0C(ap),-18(fp)
	cmpb	(ap),#04
	blssu	000190B4
	tstl	+10(ap)
	beql	000190B4
	movl	+10(ap),-14(fp)
	cmpb	(ap),#05
	blssu	000190CA
	tstl	+14(ap)
	beql	000190CA
	pushab	-0C(fp)
	pushab	-10(fp)
	pushl	+14(ap)
	calls	#03,(r6)
	pushl	sp
	pushab	+08(sp)
	pushab	-38(fp)
	calls	#03,000190D5                                         ; @(pc)+
	bgeq	000190B5
	Invalid
	pushaq	#00000004
	movzwl	#0000,@(sp)+
	mnegl	(r0),@6E01BD42
	halt
	bvc	00019094
	bbsc	@(r0)+,#54,00019099
	bbsc	@(sp)+,@00019328,000190A1
	sobgeq	@(sp)+,000190F6
	cmpc3	#0002,#00,#00
	Invalid
	rsb
	movl	+04(ap),-10(fp)
	pushl	sp
	pushab	+08(sp)
	pushab	-1C(fp)
	calls	#03,00019116                                         ; @(pc)+
	bgeq	000190F6
	Invalid
	pushaq	#00000004
	movzwl	#0000,@(sp)+
	mnegl	(r0),@6E01BD83
	halt
	bvc	000190D5
	bbsc	@(r0)+,#55,000190DA
	bbsc	@(sp)+,@00019328,000190E2
	sobgeq	@(sp)+,00019137
	blbs	#00000001,0001913B
	halt
	xorw3	@-600F5261(r8),-14(fp),@+04AC(fp)
	calls	#03,@00019320
	pushl	sp
	pushab	+08(sp)
	pushab	-1C(fp)
	calls	#03,00019159                                         ; @(pc)+
	bgeq	00019139
	Invalid
	pushaq	#00000004
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	Invalid
	halt
	Invalid
	halt
	bisb2	#02,#00
	halt
	Invalid
	halt
	halt
	bisb2	#01,#00
	halt
	ret
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	Invalid
	halt
	nop
	halt
	halt
	halt
	prober	#04,#0000,#00
	cvtbd	#00,#0.5
	halt
	Invalid
	halt
	Invalid
	halt
	halt
	bgtr	0001925E
	halt
	halt
	Invalid
	halt
	halt
	Invalid
	halt
	halt
	Invalid
	halt
	halt
	Invalid
	halt
	halt
	muld2	#0.5625,#0000
	halt
	bbsc	#00000006,#00,00019280
	movp	#0004,#00,#00
	movc5
	ldpctx
	halt
	halt
	insqhi
	rei
	halt
	halt
	Invalid
	halt
	muld2	#0.5,#0000
	halt
	Invalid
	halt
	ret
	bpt
	halt
	halt
	Invalid
	halt
	halt
	Invalid
	halt
	bvc	000192A8
	halt
	halt
	muld2	#0.75,#0000
	halt
	bbsc	#00000003,#00,000192B0
	Invalid
	halt
	halt
	xfc
	bpt
	halt
	halt
	ret
	ret
	halt
	halt
	Invalid
	halt
	Invalid
	halt
	halt
	Invalid
	Invalid
	halt
	mulb2	#02,#00
	halt
	Invalid
	halt
	Invalid
	halt
	Invalid
	halt
	movpsl
	halt
	halt
	halt
	halt
	cvtpt	#0000,#00,#00,#0022
	halt
	halt
	halt
	addp4	#0000,#00,#0000,#1E
	halt
	halt
	halt
	bvc	000192FB
	halt
	halt
	bgtru	000192FF
	halt
	halt
	bgeq	00019303
	halt
	halt
	jsb	#00
	halt
	halt
	bgtr	0001930B
	halt
	halt
	bsbb	0001930F
	halt
	halt
	bneq	00019313
	halt
	crc	#00,#00000000,#0000,#02
	halt
	halt
	halt
	Invalid
	halt
	addd2	#0.875,#0000
	halt
	Invalid
	halt
	Invalid
	halt
	cvtps	#0009,#00,#0000,+0003(r0)
	halt
	Invalid
	bsbb	0001933E
	halt
	halt
	halt
	rsb
	halt
	halt
	Invalid
	halt
	Invalid
	halt
	halt
	halt
	halt
	halt
	Invalid
	Invalid
	nop
	jsb	#00
	halt
	nop
	bvc	0001935F
	halt
	nop
	bgtru	00019363
	halt
	nop
	bgeq	00019367
	halt
	nop
	addp4	#0000,#00,#0001,#1E
	halt
	halt
	addp4	#000A,#00,#0000,#38
	Invalid
	nop
	halt
	halt
	halt
	halt
	Invalid
	rei
	halt
	probew	#00,#0000,#00
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	Invalid
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	svpctx
	cvtfd	r8[r1],r2[r3]
	Invalid
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	ldpctx
	cvtbf	r2[r2][r9],r4
	Invalid
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	ldpctx
	cvtwf	r4,r2[r8]
	Invalid
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	prober	#00,#0000,#00
	nop
	halt
	halt
	halt
	Invalid
	Invalid
	Invalid
	insqhi
	Invalid
	addd2	0.6875[r9],#0000
	muld2	0.6875[r9],#0000
	cvtdb	0.6875[r9],#00
	cvtbd	03[r9],#0.5
	Invalid
	Invalid
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
	halt
