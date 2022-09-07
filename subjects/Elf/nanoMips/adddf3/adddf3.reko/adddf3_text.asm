;;; Segment .text (0804D000)

;; __adddf3: 0804D000
__adddf3 proc
	ext	r10,r5,00000000,00000014
	ext	r9,r7,00000000,00000014
	sll	r10,r10,00000003
	srl	r8,r4,0000001D
	ext	r13,r5,00000004,0000000B
	or	r8,r8,r10
	srl	r3,r7,0000001F
	ext	r10,r7,00000004,0000000B
	srl	r5,r5,0000001F
	sll	r7,r9,00000003
	srl	r9,r6,0000001D
	sll	r12,r4,00000003
	or	r9,r9,r7
	sll	r6,r6,00000003
	subu	r11,r13,r10
	addiu	r2,r0,000007FF
	bnec	r5,r3,0804D304

l0804D042:
	move	r3,r11
	bgec	r0,r11,0804D192

l0804D048:
	bnec	r0,r10,0804D10E

l0804D04C:
	or	r7,r9,r6
	bnezc	r7,0804D0B4

l0804D052:
	move	r10,r11
	bnec	r11,r2,0804D27A

l0804D058:
	or	r7,r8,r12
	bnec	r0,r7,0804D27A

l0804D060:
	move	r8,r0
	move	r12,r0

l0804D064:
	bbeqzc	r8,00000017,0804D07A

l0804D068:
	addiu	r10,r10,00000001
	addiu	r7,r0,000007FF
	ins	r8,r0,00000007,00000001
	bnec	r10,r7,0804D07A

l0804D076:
	move	r8,r0
	move	r12,r0

l0804D07A:
	sll	r6,r8,0000001D
	srl	r7,r12,00000003
	or	r7,r7,r6
	addiu	r6,r0,000007FF
	srl	r8,r8,00000003
	bnec	r10,r6,0804D0A0

l0804D090:
	or	r6,r7,r8
	beqc	r0,r6,0804D612

l0804D098:
	lui	r6,00000080
	or	r8,r8,r6

l0804D0A0:
	move	r6,r0
	ins	r6,r8,00000000,00000001
	ins	r6,r10,00000004,00000001
	ins	r6,r5,0000000F,00000001
	movep	r8,r9,r6,r7
	movep	r4,r5,r9,r8
	jrc	ra

l0804D0B4:
	addiu	r7,r11,FFFFFFFF
	bnezc	r7,0804D0FA

l0804D0BA:
	addu	r6,r12,r6
	addu	r8,r8,r9
	sltu	r7,r6,r12
	addiu	r10,r0,00000001
	addu	r8,r8,r7
	move	r12,r6

l0804D0CC:
	bbeqzc	r8,00000017,0804D27A

l0804D0D0:
	addiu	r10,r10,00000001
	addiu	r7,r0,000007FF
	beqc	r10,r7,0804D060

l0804D0DA:
	move	r6,r8
	srl	r2,r12,00000001
	ins	r6,r0,00000007,00000001
	andi	r12,r12,00000001
	sll	r8,r6,0000001F
	or	r12,r2,r12
	or	r12,r8,r12
	srl	r8,r6,00000001
	bc	0804D27A

l0804D0FA:
	bnec	r11,r2,0804D12C

l0804D0FE:
	or	r7,r8,r12
	bnec	r0,r7,0804D2D2

l0804D106:
	move	r8,r0
	move	r12,r0
	move	r10,r11
	bc	0804D064

l0804D10E:
	bnec	r13,r2,0804D122

l0804D112:
	or	r7,r8,r12
	bnec	r0,r7,0804D2D2

l0804D11A:
	move	r8,r0
	move	r12,r0
	move	r10,r13
	bc	0804D064

l0804D122:
	lui	r7,00000800
	or	r9,r9,r7
	move	r7,r11

l0804D12C:
	bgeic	r7,00000039,0804D188

l0804D130:
	bgeic	r7,00000020,0804D168

l0804D134:
	addiu	r10,r0,00000020
	srlv	r11,r6,r7
	subu	r10,r10,r7
	srlv	r7,r9,r7
	sllv	r4,r9,r10
	sllv	r6,r6,r10
	or	r4,r4,r11
	sltu	r6,r0,r6
	or	r6,r6,r4

l0804D156:
	addu	r6,r6,r12
	addu	r7,r7,r8
	sltu	r8,r6,r12
	move	r10,r13
	addu	r8,r8,r7
	move	r12,r6
	bc	0804D0CC

l0804D168:
	srlv	r4,r9,r7
	move	r10,r0
	beqic	r7,00000020,0804D17A

l0804D172:
	subu	r7,r0,r7
	sllv	r10,r9,r7

l0804D17A:
	or	r6,r10,r6
	sltu	r6,r0,r6
	or	r6,r6,r4

l0804D184:
	move	r7,r0
	bc	0804D156

l0804D188:
	or	r6,r9,r6
	sltu	r6,r0,r6
	bc	0804D184

l0804D192:
	beqc	r0,r11,0804D240

l0804D196:
	bnec	r0,r13,0804D202

l0804D19A:
	or	r7,r8,r12
	bnezc	r7,0804D1B4

l0804D1A0:
	bnec	r10,r2,0804D1AE

l0804D1A4:
	or	r12,r9,r6
	move	r8,r0
	beqc	r0,r12,0804D064

l0804D1AE:
	move	r8,r9
	move	r12,r6
	bc	0804D27A

l0804D1B4:
	nor	r11,r0,r11
	bnec	r0,r11,0804D1CA

l0804D1BC:
	addu	r12,r12,r6
	addu	r8,r8,r9

l0804D1C2:
	sltu	r6,r12,r6
	addu	r8,r8,r6
	bc	0804D0CC

l0804D1CA:
	beqc	r10,r2,0804D1A4

l0804D1CE:
	bgeic	r11,00000039,0804D236

l0804D1D2:
	bgeic	r11,00000020,0804D214

l0804D1D6:
	li	r7,00000020
	srlv	r2,r12,r11
	subu	r7,r7,r11
	srlv	r11,r8,r11
	sllv	r4,r8,r7
	sllv	r12,r12,r7
	or	r4,r4,r2
	sltu	r12,r0,r12
	or	r12,r4,r12

l0804D1F8:
	addu	r12,r12,r6
	addu	r8,r11,r9
	bc	0804D1C2

l0804D202:
	beqc	r10,r2,0804D1A4

l0804D206:
	lui	r7,00000800
	subu	r11,r0,r11
	or	r8,r8,r7
	bc	0804D1CE

l0804D214:
	srlv	r4,r8,r11
	move	r7,r0
	beqic	r11,00000020,0804D226

l0804D21E:
	subu	r11,r0,r11
	sllv	r7,r8,r11

l0804D226:
	or	r12,r7,r12
	sltu	r12,r0,r12
	or	r12,r4,r12

l0804D232:
	move	r11,r0
	bc	0804D1F8

l0804D236:
	or	r12,r8,r12
	sltu	r12,r0,r12
	bc	0804D232

l0804D240:
	addiu	r10,r13,00000001
	andi	r7,r10,000007FF
	bgeic	r7,00000002,0804D2E2

l0804D24C:
	or	r7,r8,r12
	bnec	r0,r13,0804D298

l0804D254:
	beqc	r0,r7,0804D5EC

l0804D258:
	or	r7,r9,r6
	move	r10,r0
	beqzc	r7,0804D27A

l0804D260:
	addu	r6,r12,r6
	addu	r8,r8,r9
	sltu	r7,r6,r12
	move	r12,r6
	addu	r8,r8,r7
	bbeqzc	r8,00000017,0804D27A

l0804D272:
	ins	r8,r0,00000007,00000001
	addiu	r10,r0,00000001

l0804D27A:
	andi	r7,r12,00000007
	beqc	r0,r7,0804D064

l0804D282:
	andi	r7,r12,0000000F
	beqic	r7,00000004,0804D064

l0804D28A:
	addiu	r6,r12,00000004
	sltu	r7,r6,r12
	move	r12,r6
	addu	r8,r8,r7
	bc	0804D064

l0804D298:
	beqc	r0,r7,0804D5F2

l0804D29C:
	or	r6,r9,r6
	move	r10,r2
	beqzc	r6,0804D27A

l0804D2A4:
	srl	r10,r8,00000003
	srl	r9,r9,00000003
	or	r9,r9,r10
	bbnezc	r9,00000013,0804D2D8

l0804D2B4:
	ext	r4,r4,00000000,0000001D
	sll	r7,r8,0000001D
	or	r7,r7,r4
	move	r3,r5

l0804D2C0:
	sll	r10,r10,00000003
	srl	r8,r7,0000001D
	or	r8,r8,r10
	sll	r12,r7,00000003

l0804D2D0:
	move	r5,r3

l0804D2D2:
	addiu	r10,r0,000007FF
	bc	0804D27A

l0804D2D8:
	li	r10,000FFFFF
	li	r7,FFFFFFFF
	bc	0804D2C0

l0804D2E2:
	beqc	r10,r2,0804D060

l0804D2E6:
	addu	r6,r12,r6
	addu	r8,r8,r9
	sltu	r7,r6,r12
	srl	r6,r6,00000001
	addu	r2,r8,r7
	sll	r8,r2,0000001F
	or	r12,r8,r6
	srl	r8,r2,00000001
	bc	0804D27A

l0804D304:
	move	r14,r11
	bgec	r0,r11,0804D3BE

l0804D30A:
	bnec	r0,r10,0804D384

l0804D30E:
	or	r7,r9,r6
	beqc	r0,r7,0804D052

l0804D316:
	addiu	r7,r11,FFFFFFFF
	bnezc	r7,0804D340

l0804D31C:
	subu	r6,r12,r6
	subu	r8,r8,r9
	sltu	r7,r12,r6
	addiu	r10,r0,00000001
	subu	r8,r8,r7
	move	r12,r6

l0804D332:
	bbeqzc	r8,00000017,0804D27A

l0804D336:
	ext	r4,r8,00000000,00000017
	move	r11,r12
	move	r13,r10
	bc	0804D548

l0804D340:
	beqc	r11,r2,0804D0FE

l0804D344:
	bgeic	r7,00000039,0804D3B4

l0804D348:
	bgeic	r7,00000020,0804D394

l0804D34C:
	addiu	r10,r0,00000020
	srlv	r11,r6,r7
	subu	r10,r10,r7
	srlv	r7,r9,r7
	sllv	r4,r9,r10
	sllv	r6,r6,r10
	or	r4,r4,r11
	sltu	r6,r0,r6
	or	r6,r6,r4

l0804D36E:
	subu	r6,r12,r6
	subu	r7,r8,r7
	sltu	r8,r12,r6
	move	r10,r13
	subu	r8,r7,r8
	move	r12,r6
	bc	0804D332

l0804D384:
	beqc	r13,r2,0804D112

l0804D388:
	lui	r7,00000800
	or	r9,r9,r7
	move	r7,r11
	bc	0804D344

l0804D394:
	srlv	r4,r9,r7
	move	r10,r0
	beqic	r7,00000020,0804D3A6

l0804D39E:
	subu	r7,r0,r7
	sllv	r10,r9,r7

l0804D3A6:
	or	r6,r10,r6
	sltu	r6,r0,r6
	or	r6,r6,r4

l0804D3B0:
	move	r7,r0
	bc	0804D36E

l0804D3B4:
	or	r6,r9,r6
	sltu	r6,r0,r6
	bc	0804D3B0

l0804D3BE:
	beqc	r0,r11,0804D46E

l0804D3C2:
	bnec	r0,r13,0804D430

l0804D3C6:
	or	r7,r8,r12
	bnezc	r7,0804D3DE

l0804D3CC:
	bnec	r10,r2,0804D3D8

l0804D3D0:
	or	r12,r9,r6
	beqc	r0,r12,0804D5F8

l0804D3D8:
	move	r8,r9
	move	r12,r6
	bc	0804D4C2

l0804D3DE:
	nor	r11,r0,r11
	bnec	r0,r11,0804D3FA

l0804D3E6:
	subu	r12,r6,r12
	subu	r8,r9,r8

l0804D3EE:
	sltu	r6,r6,r12
	move	r5,r3
	subu	r8,r8,r6
	bc	0804D332

l0804D3FA:
	beqc	r10,r2,0804D3D0

l0804D3FE:
	bgeic	r11,00000039,0804D464

l0804D402:
	bgeic	r11,00000020,0804D442

l0804D406:
	li	r7,00000020
	srlv	r4,r12,r11
	subu	r7,r7,r11
	srlv	r11,r8,r11
	sllv	r5,r8,r7
	sllv	r12,r12,r7
	or	r5,r5,r4
	sltu	r12,r0,r12
	or	r12,r5,r12

l0804D426:
	subu	r12,r6,r12
	subu	r8,r9,r11
	bc	0804D3EE

l0804D430:
	beqc	r10,r2,0804D3D0

l0804D434:
	lui	r7,00000800
	subu	r11,r0,r11
	or	r8,r8,r7
	bc	0804D3FE

l0804D442:
	srlv	r5,r8,r11
	move	r7,r0
	beqic	r11,00000020,0804D454

l0804D44C:
	subu	r11,r0,r11
	sllv	r7,r8,r11

l0804D454:
	or	r12,r7,r12
	sltu	r12,r0,r12
	or	r12,r5,r12

l0804D460:
	move	r11,r0
	bc	0804D426

l0804D464:
	or	r12,r8,r12
	sltu	r12,r0,r12
	bc	0804D460

l0804D46E:
	addiu	r7,r13,00000001
	andi	r7,r7,000007FF
	bgeic	r7,00000002,0804D524

l0804D47A:
	or	r10,r8,r12
	or	r7,r9,r6
	bnec	r0,r13,0804D4D6

l0804D486:
	bnec	r0,r10,0804D49A

l0804D48A:
	bnec	r0,r7,0804D3D8

l0804D48E:
	move	r8,r0
	move	r12,r0

l0804D492:
	move	r10,r0
	move	r5,r0
	bc	0804D064

l0804D49A:
	beqzc	r7,0804D4D2

l0804D49C:
	subu	r4,r12,r6
	subu	r7,r8,r9
	sltu	r10,r12,r4
	subu	r7,r7,r10
	bbeqzc	r7,00000017,0804D4C6

l0804D4B0:
	subu	r12,r6,r12
	subu	r8,r9,r8
	sltu	r6,r6,r12
	move	r10,r0
	subu	r8,r8,r6

l0804D4C2:
	move	r5,r3
	bc	0804D27A

l0804D4C6:
	or	r12,r4,r7
	beqc	r0,r12,0804D5AC

l0804D4CE:
	move	r8,r7
	move	r12,r4

l0804D4D2:
	move	r10,r0
	bc	0804D27A

l0804D4D6:
	bnec	r0,r10,0804D4E4

l0804D4DA:
	beqc	r0,r7,0804D600

l0804D4DE:
	move	r8,r9
	move	r12,r6
	bc	0804D2D0

l0804D4E4:
	move	r10,r2
	beqc	r0,r7,0804D27A

l0804D4EA:
	srl	r7,r8,00000003
	srl	r9,r9,00000003
	or	r9,r9,r7
	bbnezc	r9,00000013,0804D51A

l0804D4FA:
	ext	r4,r4,00000000,0000001D
	sll	r12,r8,0000001D
	or	r4,r12,r4
	move	r14,r5

l0804D508:
	sll	r7,r7,00000003
	srl	r8,r4,0000001D
	or	r8,r8,r7
	sll	r12,r4,00000003
	move	r5,r14
	bc	0804D2D2

l0804D51A:
	li	r7,000FFFFF
	li	r4,FFFFFFFF
	bc	0804D508

l0804D524:
	subu	r11,r12,r6
	subu	r4,r8,r9
	sltu	r7,r12,r11
	subu	r4,r4,r7
	bbeqzc	r4,00000017,0804D5A4

l0804D536:
	subu	r11,r6,r12
	subu	r8,r9,r8
	sltu	r6,r6,r11
	move	r5,r3
	subu	r4,r8,r6

l0804D548:
	clz	r7,r4
	bnezc	r4,0804D556

l0804D54E:
	clz	r7,r11
	addiu	r7,r7,00000020

l0804D556:
	addiu	r10,r7,FFFFFFF8
	bgeic	r10,00000020,0804D5B0

l0804D55E:
	subu	r8,r0,r10
	sllv	r4,r4,r10
	srlv	r8,r11,r8
	sllv	r12,r11,r10
	or	r8,r8,r4

l0804D572:
	bltc	r10,r13,0804D5E2

l0804D576:
	subu	r10,r10,r13
	addiu	r6,r10,00000001
	bgeic	r6,00000020,0804D5BC

l0804D582:
	li	r7,00000020
	srlv	r9,r12,r6
	subu	r7,r7,r6
	sllv	r4,r8,r7
	sllv	r7,r12,r7
	or	r4,r4,r9
	sltu	r7,r0,r7
	or	r12,r4,r7
	srlv	r8,r8,r6
	bc	0804D4D2

l0804D5A4:
	or	r12,r11,r4
	bnec	r0,r12,0804D548

l0804D5AC:
	move	r8,r0
	bc	0804D492

l0804D5B0:
	addiu	r8,r7,FFFFFFD8
	move	r12,r0
	sllv	r8,r11,r8
	bc	0804D572

l0804D5BC:
	addiu	r10,r10,FFFFFFE1
	move	r7,r0
	srlv	r10,r8,r10
	beqic	r6,00000020,0804D5D2

l0804D5CA:
	subu	r6,r0,r6
	sllv	r7,r8,r6

l0804D5D2:
	or	r8,r12,r7
	sltu	r8,r0,r8
	or	r12,r10,r8
	move	r8,r0
	bc	0804D4D2

l0804D5E2:
	subu	r10,r13,r10
	ins	r8,r0,00000007,00000001
	bc	0804D27A

l0804D5EC:
	move	r8,r9
	move	r12,r6
	bc	0804D4D2

l0804D5F2:
	move	r8,r9
	move	r12,r6
	bc	0804D2D2

l0804D5F8:
	move	r8,r0
	move	r5,r3
	bc	0804D064

l0804D600:
	move	r5,r0
	li	r8,007FFFFF
	addiu	r12,r0,FFFFFFF8
	move	r10,r2
	bc	0804D064

l0804D612:
	move	r8,r0
	bc	0804D0A0
0804D618                         00 00 00 00 00 00 00 00         ........
0804D620 00 00 00 00 00 00 00 00                         ........        
