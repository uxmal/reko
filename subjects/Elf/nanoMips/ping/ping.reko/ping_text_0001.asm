;;; Segment .text (004000F0)

l004100FE:
	move	r8,r0
	bc	0040FB90
00410104             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __fixunsdfsi: 00410110
;;   Called from:
;;     0040944C (in printf_core)
__fixunsdfsi proc
	ext	r6,r5,00000004,0000000B
	addiu	r7,r0,000003FE
	move	r8,r4
	ext	r9,r5,00000000,00000014
	move	r4,r0
	srl	r5,r5,0000001F
	bgec	r7,r6,0041016E

l00410128:
	bnezc	r5,00410166

l0041012A:
	addiu	r7,r0,0000041E
	bltc	r7,r6,00410154

l00410132:
	addiu	r5,r0,00000433
	lui	r7,00000100
	subu	r5,r5,r6
	or	r7,r9,r7
	bgeic	r5,00000020,0041015A

l00410144:
	addiu	r4,r6,FFFFFBED
	srlv	r5,r8,r5
	sllv	r4,r7,r4
	or	r4,r4,r5
	jrc	ra

l00410154:
	addiu	r4,r5,FFFFFFFF
	jrc	ra

l0041015A:
	addiu	r4,r0,00000413
	subu	r4,r4,r6
	srlv	r4,r7,r4
	jrc	ra

l00410166:
	addiu	r7,r0,0000041D
	bltc	r7,r6,00410154

l0041016E:
	jrc	ra

;; __floatsidf: 00410170
;;   Called from:
;;     00409248 (in printf_core)
;;     0040B5A2 (in decfloat)
;;     0040B5EC (in decfloat)
;;     0040B828 (in decfloat)
;;     0040BB1A (in decfloat)
;;     0040BBC6 (in decfloat)
;;     0040BC04 (in decfloat)
;;     0040BD14 (in decfloat)
;;     0040C1C0 (in __floatscan)
;;     0040C3C4 (in __floatscan)
;;     0040C432 (in __floatscan)
;;     0040C52E (in __floatscan)
;;     0040C566 (in __floatscan)
;;     0040C5AA (in __floatscan)
;;     0040C654 (in __floatscan)
__floatsidf proc
	beqzc	r4,004101C4

l00410172:
	sra	r7,r4,0000001F
	srl	r8,r4,0000001F
	xor	r4,r4,r7
	addiu	r6,r0,0000041E
	subu	r4,r4,r7
	addiu	r5,r0,00000433
	clz	r9,r4
	subu	r6,r6,r9
	subu	r5,r5,r6
	bgeic	r5,00000020,004101B6

l00410194:
	li	r7,0000000B
	subu	r7,r7,r9
	srlv	r7,r4,r7
	sllv	r4,r4,r5

l004101A2:
	move	r5,r0
	ins	r5,r7,00000000,00000001
	ins	r5,r6,00000004,00000001
	ins	r5,r8,0000000F,00000001
	movep	r6,r7,r4,r5
	movep	r4,r5,r6,r7
	jrc	ra

l004101B6:
	addiu	r7,r0,00000413
	subu	r7,r7,r6
	sllv	r7,r4,r7
	move	r4,r0
	bc	004101A2

l004101C4:
	move	r7,r0
	move	r4,r0
	move	r6,r0
	move	r8,r0
	bc	004101A2
004101CE                                           00 00               ..

;; __floatunsidf: 004101D0
;;   Called from:
;;     00409456 (in printf_core)
;;     0040B5F2 (in decfloat)
;;     0040B994 (in decfloat)
;;     0040B9CE (in decfloat)
;;     0040BBCC (in decfloat)
;;     0040BCF8 (in decfloat)
;;     0040BD24 (in decfloat)
;;     0040C3D8 (in __floatscan)
__floatunsidf proc
	beqzc	r4,00410218

l004101D2:
	clz	r8,r4
	addiu	r6,r0,0000041E
	subu	r6,r6,r8
	addiu	r5,r0,00000433
	subu	r5,r5,r6
	bgeic	r5,00000020,0041020A

l004101E8:
	li	r7,0000000B
	subu	r7,r7,r8
	srlv	r7,r4,r7
	sllv	r4,r4,r5

l004101F6:
	move	r5,r0
	ins	r5,r7,00000000,00000001
	ins	r5,r6,00000004,00000001
	move	r6,r4
	ext	r7,r5,00000000,0000001F
	movep	r4,r5,r6,r7
	jrc	ra

l0041020A:
	addiu	r7,r0,00000413
	subu	r7,r7,r6
	sllv	r7,r4,r7
	move	r4,r0
	bc	004101F6

l00410218:
	move	r7,r0
	move	r4,r0
	move	r6,r0
	bc	004101F6

;; __truncdfsf2: 00410220
;;   Called from:
;;     00409FAC (in strtof_l)
;;     0040DB38 (in __isoc99_vfscanf)
__truncdfsf2 proc
	ext	r9,r5,00000000,00000014
	ext	r10,r5,00000004,0000000B
	srl	r7,r4,0000001D
	sll	r9,r9,00000003
	or	r9,r7,r9
	addiu	r7,r10,00000001
	andi	r7,r7,000007FF
	srl	r5,r5,0000001F
	sll	r6,r4,00000003
	bltic	r7,00000002,004102D4

l00410246:
	addiu	r8,r10,FFFFFC80
	addiu	r7,r0,000000FE
	bltc	r7,r8,004102F6

l00410252:
	bltc	r0,r8,004102B6

l00410256:
	addiu	r7,r0,FFFFFFE9
	bltc	r8,r7,00410332

l0041025E:
	li	r4,0000001E
	lui	r7,00000800
	subu	r4,r4,r8
	or	r9,r9,r7
	bgeic	r4,00000020,0041028E

l00410270:
	addiu	r10,r10,FFFFFC82
	srlv	r4,r6,r4
	sllv	r7,r6,r10
	sllv	r9,r9,r10
	sltu	r7,r0,r7
	or	r7,r7,r9
	or	r7,r7,r4

l0041028A:
	move	r8,r0
	bc	004102C8

l0041028E:
	addiu	r7,r0,FFFFFFFE
	move	r11,r0
	subu	r8,r7,r8
	srlv	r8,r9,r8
	beqic	r4,00000020,004102A8

l004102A0:
	addiu	r10,r10,FFFFFCA2
	sllv	r11,r9,r10

l004102A8:
	or	r7,r11,r6
	sltu	r7,r0,r7
	or	r7,r8,r7
	bc	0041028A

l004102B6:
	sll	r4,r4,00000006
	srl	r6,r6,0000001D
	sltu	r7,r0,r4
	or	r6,r6,r7
	sll	r7,r9,00000003
	or	r7,r7,r6

l004102C8:
	andi	r6,r7,00000007
	beqzc	r6,004102FC

l004102CC:
	andi	r6,r7,0000000F
	bneiuc	r6,00000004,00410336

l004102D2:
	bc	004102FC

l004102D4:
	or	r7,r9,r6
	bnec	r0,r10,004102E2

l004102DC:
	sltu	r7,r0,r7
	bc	0041028A

l004102E2:
	addiu	r8,r0,000000FF
	beqzc	r7,004102FC

l004102E8:
	sll	r9,r9,00000003
	lui	r7,00002000
	or	r7,r9,r7
	bc	004102C8

l004102F6:
	move	r7,r0
	addiu	r8,r0,000000FF

l004102FC:
	bbeqzc	r7,0000001A,00410310

l00410300:
	addiu	r8,r8,00000001
	addiu	r6,r0,000000FF
	ins	r7,r0,0000000A,00000001
	bnec	r8,r6,00410310

l0041030E:
	move	r7,r0

l00410310:
	addiu	r6,r0,000000FF
	srl	r7,r7,00000003
	bnec	r8,r6,00410322

l0041031A:
	beqzc	r7,00410322

l0041031C:
	lui	r6,00000400
	or	r7,r7,r6

l00410322:
	move	r4,r0
	ins	r4,r7,00000000,00000001
	ins	r4,r8,00000007,00000001
	ins	r4,r5,0000000F,00000001
	jrc	ra

l00410332:
	li	r7,00000001
	move	r8,r0

l00410336:
	addiu	r7,r7,00000004
	bc	004102FC
0041033A                               00 00 00 00 00 00           ......
