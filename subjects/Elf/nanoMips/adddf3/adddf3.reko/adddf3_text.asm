;;; Segment .text (08048000)

;; __adddf3: 08048000
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
	bnec	r5,r3,08048304

l08048042:
	move	r3,r11
	bgec	r0,r11,08048192

l08048048:
	bnec	r0,r10,0804810E

l0804804C:
	or	r7,r9,r6
	bnezc	r7,080480B4

l08048052:
	move	r10,r11
	bnec	r11,r2,0804827A

l08048058:
	or	r7,r8,r12
	bnec	r0,r7,0804827A

l08048060:
	move	r8,r0
	move	r12,r0

l08048064:
	bbeqzc	r8,00000017,0804807A

l08048068:
	addiu	r10,r10,00000001
	addiu	r7,r0,000007FF
	ins	r8,r0,00000007,00000001
	bnec	r10,r7,0804807A

l08048076:
	move	r8,r0
	move	r12,r0

l0804807A:
	sll	r6,r8,0000001D
	srl	r7,r12,00000003
	or	r7,r7,r6
	addiu	r6,r0,000007FF
	srl	r8,r8,00000003
	bnec	r10,r6,080480A0

l08048090:
	or	r6,r7,r8
	beqc	r0,r6,08048612

l08048098:
	lui	r6,00000080
	or	r8,r8,r6

l080480A0:
	move	r6,r0
	ins	r6,r8,00000000,00000001
	ins	r6,r10,00000004,00000001
	ins	r6,r5,0000000F,00000001
	movep	r8,r9,r6,r7
	movep	r4,r5,r9,r8
	jrc	ra

l080480B4:
	addiu	r7,r11,FFFFFFFF
	bnezc	r7,080480FA

l080480BA:
	addu	r6,r12,r6
	addu	r8,r8,r9
	sltu	r7,r6,r12
	addiu	r10,r0,00000001
	addu	r8,r8,r7
	move	r12,r6

l080480CC:
	bbeqzc	r8,00000017,0804827A

l080480D0:
	addiu	r10,r10,00000001
	addiu	r7,r0,000007FF
	beqc	r10,r7,08048060

l080480DA:
	move	r6,r8
	srl	r2,r12,00000001
	ins	r6,r0,00000007,00000001
	andi	r12,r12,00000001
	sll	r8,r6,0000001F
	or	r12,r2,r12
	or	r12,r8,r12
	srl	r8,r6,00000001
	bc	0804827A

l080480FA:
	bnec	r11,r2,0804812C

l080480FE:
	or	r7,r8,r12
	bnec	r0,r7,080482D2

l08048106:
	move	r8,r0
	move	r12,r0
	move	r10,r11
	bc	08048064

l0804810E:
	bnec	r13,r2,08048122

l08048112:
	or	r7,r8,r12
	bnec	r0,r7,080482D2

l0804811A:
	move	r8,r0
	move	r12,r0
	move	r10,r13
	bc	08048064

l08048122:
	lui	r7,00000800
	or	r9,r9,r7
	move	r7,r11

l0804812C:
	bgeic	r7,00000039,08048188

l08048130:
	bgeic	r7,00000020,08048168

l08048134:
	addiu	r10,r0,00000020
	srlv	r11,r6,r7
	subu	r10,r10,r7
	srlv	r7,r9,r7
	sllv	r4,r9,r10
	sllv	r6,r6,r10
	or	r4,r4,r11
	sltu	r6,r0,r6
	or	r6,r6,r4

l08048156:
	addu	r6,r6,r12
	addu	r7,r7,r8
	sltu	r8,r6,r12
	move	r10,r13
	addu	r8,r8,r7
	move	r12,r6
	bc	080480CC

l08048168:
	srlv	r4,r9,r7
	move	r10,r0
	beqic	r7,00000020,0804817A

l08048172:
	subu	r7,r0,r7
	sllv	r10,r9,r7

l0804817A:
	or	r6,r10,r6
	sltu	r6,r0,r6
	or	r6,r6,r4

l08048184:
	move	r7,r0
	bc	08048156

l08048188:
	or	r6,r9,r6
	sltu	r6,r0,r6
	bc	08048184

l08048192:
	beqc	r0,r11,08048240

l08048196:
	bnec	r0,r13,08048202

l0804819A:
	or	r7,r8,r12
	bnezc	r7,080481B4

l080481A0:
	bnec	r10,r2,080481AE

l080481A4:
	or	r12,r9,r6
	move	r8,r0
	beqc	r0,r12,08048064

l080481AE:
	move	r8,r9
	move	r12,r6
	bc	0804827A

l080481B4:
	nor	r11,r0,r11
	bnec	r0,r11,080481CA

l080481BC:
	addu	r12,r12,r6
	addu	r8,r8,r9

l080481C2:
	sltu	r6,r12,r6
	addu	r8,r8,r6
	bc	080480CC

l080481CA:
	beqc	r10,r2,080481A4

l080481CE:
	bgeic	r11,00000039,08048236

l080481D2:
	bgeic	r11,00000020,08048214

l080481D6:
	li	r7,00000020
	srlv	r2,r12,r11
	subu	r7,r7,r11
	srlv	r11,r8,r11
	sllv	r4,r8,r7
	sllv	r12,r12,r7
	or	r4,r4,r2
	sltu	r12,r0,r12
	or	r12,r4,r12

l080481F8:
	addu	r12,r12,r6
	addu	r8,r11,r9
	bc	080481C2

l08048202:
	beqc	r10,r2,080481A4

l08048206:
	lui	r7,00000800
	subu	r11,r0,r11
	or	r8,r8,r7
	bc	080481CE

l08048214:
	srlv	r4,r8,r11
	move	r7,r0
	beqic	r11,00000020,08048226

l0804821E:
	subu	r11,r0,r11
	sllv	r7,r8,r11

l08048226:
	or	r12,r7,r12
	sltu	r12,r0,r12
	or	r12,r4,r12

l08048232:
	move	r11,r0
	bc	080481F8

l08048236:
	or	r12,r8,r12
	sltu	r12,r0,r12
	bc	08048232

l08048240:
	addiu	r10,r13,00000001
	andi	r7,r10,000007FF
	bgeic	r7,00000002,080482E2

l0804824C:
	or	r7,r8,r12
	bnec	r0,r13,08048298

l08048254:
	beqc	r0,r7,080485EC

l08048258:
	or	r7,r9,r6
	move	r10,r0
	beqzc	r7,0804827A

l08048260:
	addu	r6,r12,r6
	addu	r8,r8,r9
	sltu	r7,r6,r12
	move	r12,r6
	addu	r8,r8,r7
	bbeqzc	r8,00000017,0804827A

l08048272:
	ins	r8,r0,00000007,00000001
	addiu	r10,r0,00000001

l0804827A:
	andi	r7,r12,00000007
	beqc	r0,r7,08048064

l08048282:
	andi	r7,r12,0000000F
	beqic	r7,00000004,08048064

l0804828A:
	addiu	r6,r12,00000004
	sltu	r7,r6,r12
	move	r12,r6
	addu	r8,r8,r7
	bc	08048064

l08048298:
	beqc	r0,r7,080485F2

l0804829C:
	or	r6,r9,r6
	move	r10,r2
	beqzc	r6,0804827A

l080482A4:
	srl	r10,r8,00000003
	srl	r9,r9,00000003
	or	r9,r9,r10
	bbnezc	r9,00000013,080482D8

l080482B4:
	ext	r4,r4,00000000,0000001D
	sll	r7,r8,0000001D
	or	r7,r7,r4
	move	r3,r5

l080482C0:
	sll	r10,r10,00000003
	srl	r8,r7,0000001D
	or	r8,r8,r10
	sll	r12,r7,00000003

l080482D0:
	move	r5,r3

l080482D2:
	addiu	r10,r0,000007FF
	bc	0804827A

l080482D8:
	li	r10,000FFFFF
	li	r7,FFFFFFFF
	bc	080482C0

l080482E2:
	beqc	r10,r2,08048060

l080482E6:
	addu	r6,r12,r6
	addu	r8,r8,r9
	sltu	r7,r6,r12
	srl	r6,r6,00000001
	addu	r2,r8,r7
	sll	r8,r2,0000001F
	or	r12,r8,r6
	srl	r8,r2,00000001
	bc	0804827A

l08048304:
	move	r14,r11
	bgec	r0,r11,080483BE

l0804830A:
	bnec	r0,r10,08048384

l0804830E:
	or	r7,r9,r6
	beqc	r0,r7,08048052

l08048316:
	addiu	r7,r11,FFFFFFFF
	bnezc	r7,08048340

l0804831C:
	subu	r6,r12,r6
	subu	r8,r8,r9
	sltu	r7,r12,r6
	addiu	r10,r0,00000001
	subu	r8,r8,r7
	move	r12,r6

l08048332:
	bbeqzc	r8,00000017,0804827A

l08048336:
	ext	r4,r8,00000000,00000017
	move	r11,r12
	move	r13,r10
	bc	08048548

l08048340:
	beqc	r11,r2,080480FE

l08048344:
	bgeic	r7,00000039,080483B4

l08048348:
	bgeic	r7,00000020,08048394

l0804834C:
	addiu	r10,r0,00000020
	srlv	r11,r6,r7
	subu	r10,r10,r7
	srlv	r7,r9,r7
	sllv	r4,r9,r10
	sllv	r6,r6,r10
	or	r4,r4,r11
	sltu	r6,r0,r6
	or	r6,r6,r4

l0804836E:
	subu	r6,r12,r6
	subu	r7,r8,r7
	sltu	r8,r12,r6
	move	r10,r13
	subu	r8,r7,r8
	move	r12,r6
	bc	08048332

l08048384:
	beqc	r13,r2,08048112

l08048388:
	lui	r7,00000800
	or	r9,r9,r7
	move	r7,r11
	bc	08048344

l08048394:
	srlv	r4,r9,r7
	move	r10,r0
	beqic	r7,00000020,080483A6

l0804839E:
	subu	r7,r0,r7
	sllv	r10,r9,r7

l080483A6:
	or	r6,r10,r6
	sltu	r6,r0,r6
	or	r6,r6,r4

l080483B0:
	move	r7,r0
	bc	0804836E

l080483B4:
	or	r6,r9,r6
	sltu	r6,r0,r6
	bc	080483B0

l080483BE:
	beqc	r0,r11,0804846E

l080483C2:
	bnec	r0,r13,08048430

l080483C6:
	or	r7,r8,r12
	bnezc	r7,080483DE

l080483CC:
	bnec	r10,r2,080483D8

l080483D0:
	or	r12,r9,r6
	beqc	r0,r12,080485F8

l080483D8:
	move	r8,r9
	move	r12,r6
	bc	080484C2

l080483DE:
	nor	r11,r0,r11
	bnec	r0,r11,080483FA

l080483E6:
	subu	r12,r6,r12
	subu	r8,r9,r8

l080483EE:
	sltu	r6,r6,r12
	move	r5,r3
	subu	r8,r8,r6
	bc	08048332

l080483FA:
	beqc	r10,r2,080483D0

l080483FE:
	bgeic	r11,00000039,08048464

l08048402:
	bgeic	r11,00000020,08048442

l08048406:
	li	r7,00000020
	srlv	r4,r12,r11
	subu	r7,r7,r11
	srlv	r11,r8,r11
	sllv	r5,r8,r7
	sllv	r12,r12,r7
	or	r5,r5,r4
	sltu	r12,r0,r12
	or	r12,r5,r12

l08048426:
	subu	r12,r6,r12
	subu	r8,r9,r11
	bc	080483EE

l08048430:
	beqc	r10,r2,080483D0

l08048434:
	lui	r7,00000800
	subu	r11,r0,r11
	or	r8,r8,r7
	bc	080483FE

l08048442:
	srlv	r5,r8,r11
	move	r7,r0
	beqic	r11,00000020,08048454

l0804844C:
	subu	r11,r0,r11
	sllv	r7,r8,r11

l08048454:
	or	r12,r7,r12
	sltu	r12,r0,r12
	or	r12,r5,r12

l08048460:
	move	r11,r0
	bc	08048426

l08048464:
	or	r12,r8,r12
	sltu	r12,r0,r12
	bc	08048460

l0804846E:
	addiu	r7,r13,00000001
	andi	r7,r7,000007FF
	bgeic	r7,00000002,08048524

l0804847A:
	or	r10,r8,r12
	or	r7,r9,r6
	bnec	r0,r13,080484D6

l08048486:
	bnec	r0,r10,0804849A

l0804848A:
	bnec	r0,r7,080483D8

l0804848E:
	move	r8,r0
	move	r12,r0

l08048492:
	move	r10,r0
	move	r5,r0
	bc	08048064

l0804849A:
	beqzc	r7,080484D2

l0804849C:
	subu	r4,r12,r6
	subu	r7,r8,r9
	sltu	r10,r12,r4
	subu	r7,r7,r10
	bbeqzc	r7,00000017,080484C6

l080484B0:
	subu	r12,r6,r12
	subu	r8,r9,r8
	sltu	r6,r6,r12
	move	r10,r0
	subu	r8,r8,r6

l080484C2:
	move	r5,r3
	bc	0804827A

l080484C6:
	or	r12,r4,r7
	beqc	r0,r12,080485AC

l080484CE:
	move	r8,r7
	move	r12,r4

l080484D2:
	move	r10,r0
	bc	0804827A

l080484D6:
	bnec	r0,r10,080484E4

l080484DA:
	beqc	r0,r7,08048600

l080484DE:
	move	r8,r9
	move	r12,r6
	bc	080482D0

l080484E4:
	move	r10,r2
	beqc	r0,r7,0804827A

l080484EA:
	srl	r7,r8,00000003
	srl	r9,r9,00000003
	or	r9,r9,r7
	bbnezc	r9,00000013,0804851A

l080484FA:
	ext	r4,r4,00000000,0000001D
	sll	r12,r8,0000001D
	or	r4,r12,r4
	move	r14,r5

l08048508:
	sll	r7,r7,00000003
	srl	r8,r4,0000001D
	or	r8,r8,r7
	sll	r12,r4,00000003
	move	r5,r14
	bc	080482D2

l0804851A:
	li	r7,000FFFFF
	li	r4,FFFFFFFF
	bc	08048508

l08048524:
	subu	r11,r12,r6
	subu	r4,r8,r9
	sltu	r7,r12,r11
	subu	r4,r4,r7
	bbeqzc	r4,00000017,080485A4

l08048536:
	subu	r11,r6,r12
	subu	r8,r9,r8
	sltu	r6,r6,r11
	move	r5,r3
	subu	r4,r8,r6

l08048548:
	clz	r7,r4
	bnezc	r4,08048556

l0804854E:
	clz	r7,r11
	addiu	r7,r7,00000020

l08048556:
	addiu	r10,r7,FFFFFFF8
	bgeic	r10,00000020,080485B0

l0804855E:
	subu	r8,r0,r10
	sllv	r4,r4,r10
	srlv	r8,r11,r8
	sllv	r12,r11,r10
	or	r8,r8,r4

l08048572:
	bltc	r10,r13,080485E2

l08048576:
	subu	r10,r10,r13
	addiu	r6,r10,00000001
	bgeic	r6,00000020,080485BC

l08048582:
	li	r7,00000020
	srlv	r9,r12,r6
	subu	r7,r7,r6
	sllv	r4,r8,r7
	sllv	r7,r12,r7
	or	r4,r4,r9
	sltu	r7,r0,r7
	or	r12,r4,r7
	srlv	r8,r8,r6
	bc	080484D2

l080485A4:
	or	r12,r11,r4
	bnec	r0,r12,08048548

l080485AC:
	move	r8,r0
	bc	08048492

l080485B0:
	addiu	r8,r7,FFFFFFD8
	move	r12,r0
	sllv	r8,r11,r8
	bc	08048572

l080485BC:
	addiu	r10,r10,FFFFFFE1
	move	r7,r0
	srlv	r10,r8,r10
	beqic	r6,00000020,080485D2

l080485CA:
	subu	r6,r0,r6
	sllv	r7,r8,r6

l080485D2:
	or	r8,r12,r7
	sltu	r8,r0,r8
	or	r12,r10,r8
	move	r8,r0
	bc	080484D2

l080485E2:
	subu	r10,r13,r10
	ins	r8,r0,00000007,00000001
	bc	0804827A

l080485EC:
	move	r8,r9
	move	r12,r6
	bc	080484D2

l080485F2:
	move	r8,r9
	move	r12,r6
	bc	080482D2

l080485F8:
	move	r8,r0
	move	r5,r3
	bc	08048064

l08048600:
	move	r5,r0
	li	r8,007FFFFF
	addiu	r12,r0,FFFFFFF8
	move	r10,r2
	bc	08048064

l08048612:
	move	r8,r0
	bc	080480A0
08048618                         00 00 00 00 00 00 00 00         ........
08048620 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
08049E80 00 00 00 00 00 00 00 00                         ........        
