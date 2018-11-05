;;; Segment privileged_functions (00000000)
00000000 2C 02 00 20 09 80 00 00 01 80 00 00 05 80 00 00 ,.. ............
00000010 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000020 00 00 00 00 00 00 00 00 00 00 00 00 15 17 00 00 ................
00000030 00 00 00 00 00 00 00 00 89 16 00 00 E5 16 00 00 ................
00000040 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000050 00 00 00 00 09 81 00 00 70                      ........p      

;; prvUnlockQueue: 00000059
prvUnlockQueue proc
	stmeq	r6,{r0,r2,r4-r5,r7-r8,r10}

l0000005D:
	ldrbls	r8,[r10,#&CF0]!

l00000061:
	strbvs	r4,[r0],-#&5F8

l00000065:
	Invalid

l00000069:
	msrge	spsr,#&BDD

l0000006D:
	ldrbths	r0,[r1],#&5B1

l00000071:
	mvneq	r0,r6,lsl #&A

l00000075:
	Invalid

l00000079:
	blvs	$FEC5AF49

l0000007D:
	adcslo	r5,r1,r10,ror #&16

l00000081:
	blgt	$FFC001A1

l00000085:
	Invalid
	ldrsbeq	r0,[ip,-r0]!
	mvns	r8,#&F00
	ldrhteq	r5,[r2],#&C2
	svc	#&D1F12B
	ldrbmi	r8,[r8,#&523]!
	Invalid
	ldmibvs	r0,{r1,r3-r7,fp}^
	ldrbtmi	r9,[r8],#&5FA
	adcseq	r6,r2,r0,asr #8
	blhs	$FF745965
	ldreq	r10,[r1,#&369]!
	streq	r1,[r6,-#&F1]
	teq	ip,#&38
	blvs	$FEC9738D
	blpl	$01A4AF8D
	strheq	r3,[r6],-#&1
	ldrshteq	r10,[pc],#&80                                     ; 00000151
	bicseq	pc,r0,r8,lsr #8
	blvs	$FFC005C9
	Invalid
	Invalid
	strhi	pc,[r3,-#&FD1]!
	ldclt	p4,c4,[r0,-#&3E0]!
	stmeq	r0,{r3,r5-r7,ip-lr}
	ldrshtvc	r6,[r10],#&20

;; prvCopyDataToQueue: 000000ED
prvCopyDataToQueue proc
	strheq	r0,[r6],-#&45

l000000F1:
	stmhs	fp!,{r2-r3,r5-r6,r8,r10,sp,pc}

l000000F5:
	strhteq	r2,[r8],-#&69

l000000F9:
	bicseq	r3,r0,lr,lsr #2

l000000FD:
	rsbvc	r10,r3,r5,lsr r5

l00000101:
	subeq	r1,r6,#&BD00000

l00000105:
	adcsge	r7,r9,r6,asr #&1C

l00000109:
	blpl	$FFC02AB1

l0000010D:
	ldrshhs	r10,[r8,-#&3A]!

l00000111:
	bleq	$01A18AC9

l00000115:
	movtge	r9,#&2344

l00000119:
	bicshs	r1,r3,#&180000

l0000011D:
	eorslo	r0,r5,r8,ror #2

l00000121:
	strbge	r10,[r0,-#&346]!

l00000125:
	adcs	r7,sp,r3,rrx
	blmi	$FFC02AD1

l0000012D:
	msr	spsr,#&2FA
	Invalid

l00000135:
	blhi	$01104EDD

l00000139:
	rsbeq	lr,r0,#&8000001

l0000013D:
	bne	$01A1908D

l00000141:
	rsbeq	lr,r0,#&40000004

l00000145:
	bicseq	r0,r0,lr,lsr #&E

l00000149:
	strge	r0,[r0,-#&35]!

l0000014D:
	rrxseq	r7,r3,#1

l00000151:
	strbge	r3,[r6,-#&35]

l00000155:
	ldreq	r7,[sp,#&63]!

l00000159:
	strhteq	r0,[r5],-#&19

l0000015D:
	rscvs	ip,r7,r0,lsr #&1C

l00000161:
	ldrbvc	r0,[r0,#&168]!

l00000165:
	Invalid

l00000169:
	rsceq	ip,r7,#&600000

;; prvCopyDataFromQueue: 0000016D
prvCopyDataFromQueue proc
	bleq	$FEC5AB25

l00000171:
	asrsgt	r1,r6,#&20

l00000175:
	eretne

l00000179:
	crc32wgt	r10,r2,r4

l0000017D:
	rorseq	r2,r0,#8

l00000181:
	rsbne	ip,r0,r8,ror #2

l00000185:
	beq	$0118647D

l00000189:
	ldrshtvc	r1,[r10],#&C0

l0000018D:
	Invalid

;; xQueueGenericSend: 00000191
xQueueGenericSend proc
	subeq	pc,r7,r9,ror #1

l00000195:
	ldrteq	r8,[r0],#&425

l00000199:
	crc32cweq	r8,r6,r6

l0000019D:
	stmge	r6,{r1,r4,r7-ip}

l000001A1:
	Invalid
	stmdbeq	r0,{r4,r7-r10,sp}^
	ldrshteq	r0,[r10],#&20
	ldmdbeq	ip,{r4-r7,r9-fp,sp}^
	ldrbtls	lr,[r9],#&2F0
	svc	#&3044F8
	ldrthi	r0,[pc],#&82B                                       ; 000009EC
	strls	r4,[r0],#&4F8
	svc	#&3045F8
	ldrthi	r0,[pc],#&82B                                       ; 000009F8
	stmeq	r0,{r3-r8,r10,lr}
	ldrsheq	pc,[r9]!
	adceq	r0,r8,r9,lsr #5
	ldrshteq	ip,[pc]                                           ; 000001DD
	ldmdbeq	r1,{r3,r5,ip,lr}^
	rscsge	ip,r9,#&F000
	bls	$01AF8F95
	ldmdbeq	r0,{r1,r6,r8-r10,ip}^
	ldrshths	lr,[r9],#&20
	mvnslo	pc,#&118
	bllo	$FFC005F5
	stmdaeq	r5!,{r1-r8}
	rscsge	fp,r9,#&F00
	bls	$01AF8FB1
	sbcseq	r2,r3,#&42
	bicseq	r1,r0,pc,lsr #&1C
	umlalseq	r9,r3,lr,r6
	sbcseq	ip,r1,#&B400
	ldrbls	r0,[r0,r8,lsr #1]!
	stmdbeq	r7,{r0-r10,lr-pc}^
	ldrsheq	ip,[r9,#&A0]!
	smlalsne	r0,r1,r9,r4
	blle	$FFC00229
	svc	#&4620FE
	ldrshteq	r1,[pc],#&67                                      ; 00000298
	ldrshteq	r1,[lr],#&E0
	svcmi	#&D1E028
	ldmgt	r3,{r4-r7,pc}^
	svclt	#&3000F8
	svclt	#&8F4FF3
	Invalid
	smlalttpl	r3,r6,r7,r10
	svc	#&462046
	mvnsvs	r4,#&F70
	ldmlo	r9!,{r1,r3,r5-r6,r8-r9,fp,sp-pc}
	ldrhthi	r4,[r0],#&F1
	bne	$012C47A9
	svcmi	#&F3BF60
	svcvs	#&F3BF8F
	mvnsge	r0,#&8F0000
	strteq	r0,[r0],-#&1F9
	Invalid
	Invalid
	strbeq	r3,[r6],-#&F9
	Invalid
	svc	#&462087
	ldrshteq	lr,[lr],#&A7
	ldrshteq	pc,[sp],#&20
	Invalid
	streq	pc,[r7],#&E8
	strdeq	r2,r3,[r0],-r1
	ldrshteq	ip,[lr],#&20
	bics	sp,r1,#&2800
	ldrteq	r0,[pc],#&E7                                        ; 0000038C
	rscvc	r0,r0,sp,ror #1

;; xQueuePeekFromISR: 000002A5
xQueuePeekFromISR proc
	ldrhne	lr,[r3,#&F5]!

l000002A9:
	svclt	#&F04F85

l000002AD:
	mvnsne	r8,r3,lsl #6

l000002B1:
	svcvs	#&F3BF88

l000002B5:
	svcmi	#&F3BF8F

l000002B9:
	blne	$01AE10FD

l000002BD:
	strbhi	r1,[r6,-#&8B9]

l000002C1:
	Invalid

l000002C5:
	Invalid

l000002C9:
	svcmi	#&F7FF68

l000002CD:
	ldrsheq	lr,[r0,-#&6F]!

l000002D1:
	mvnsne	r8,r0,lsr #&A

l000002D5:
	Invalid

;; xQueueGenericReceive: 000002D9
xQueueGenericReceive proc
	subeq	pc,r7,r9,ror #1

l000002DD:
	ldrteq	r8,[r0],#&425

l000002E1:
	crc32cweq	r8,r6,r6

l000002E5:
	svchs	#&469992

l000002E9:
	ldmdblo	r8,{r1-r2,r6,r8-ip,lr-pc}^

l000002ED:
	stmdbeq	r0,{r0,r7,r10-fp}^

l000002F1:
	mvnsge	r4,#&F

l000002F5:
	strlo	r0,[fp,-fp,rrx #1]!

l000002F9:
	ldmibpl	r0,{r4,r6-r7,fp}^

l000002FD:
	svc	#&4620F9
	ldrshteq	r10,[lr],#&A7

l00000305:
	ldrsheq	fp,[sp,#&20]!

l00000309:
	ldrblo	r0,[r0,#&825]!

l0000030D:
	Invalid

l00000311:
	bicseq	r4,r1,lr,lsr #&1A

l00000315:
	strtmi	r0,[fp],-#&9B

l00000319:
	Invalid

l0000031D:
	ubfxmi	r0,r0,#&11,#&11

l00000321:
	mvnsvc	r0,#&F9

l00000325:
	Invalid

l00000329:
	ldrbtmi	r9,[r8],#&4F9

l0000032D:
	stmdaeq	fp!,{r4-r5,r8-pc}

l00000331:
	ldrbtmi	r8,[r8],#&4BF

l00000335:
	ldrbmi	r9,[r8,#&470]!

l00000339:
	stmdaeq	fp!,{r4-r5,r8-pc}

l0000033D:
	ldrbmi	r8,[r8,#&4BF]!

l00000341:
	ldrblo	r0,[r0,#&870]!

l00000345:
	adceq	r0,r9,#&4000003E

l00000349:
	ldrbeq	r0,[r0,#&A8]!

l0000034D:
	Invalid

l00000351:
	svc	#&4620D0
	ldrshteq	r8,[lr],#&7

l00000359:
	ldmdbeq	sp,{r4-r7,fp,pc}^

l0000035D:
	mvnsge	r0,#&F000

l00000361:
	ldmeq	r1!,{r0-r1,r3,r5-r6,r8-r9,fp-pc}

l00000365:
	Invalid

l00000369:
	mvnshs	r0,r7,ror #&11

l0000036D:
	msrls	spsr,#&3F9

l00000371:
	ldreq	r0,[r9],#&1B3

l00000375:
	strdeq	r2,r3,[r0],-r1

l00000379:
	ldrshths	r3,[lr]

l0000037D:
	blvs	$FFE0009D

l00000381:
	mvnsvc	r0,#&FE

l00000385:
	Invalid

l00000389:
	ldrsbthi	r4,[r0],#&F1

l0000038D:
	rscseq	ip,r8,r3,asr r8

l00000391:
	svcmi	#&F3BF30

l00000395:
	svcvs	#&F3BF8F

l00000399:
	rsceq	fp,r7,#&23C00000

l0000039D:
	mvnsle	r0,r8,lsr #1

l000003A1:
	stmdbeq	r7,{r1-r7,r10-sp,pc}^

l000003A5:
	ldrshteq	r0,[r9],#&40

l000003A9:
	Invalid

l000003AD:
	orrpl	pc,r7,r8,ror #1

l000003B1:
	strb	r2,[r6,-#&46]
	ldmible	r7,{r3,r5-r6,r8-pc}^

l000003B9:
	ldrshteq	fp,[r1],#&9E

l000003BD:
	bicshs	r1,r1,#&3C000000

l000003C1:
	strtge	r0,[lr],-r8

l000003C5:
	movshs	r4,#&18C00

l000003C9:
	ldmeq	fp!,{r0,r3,r5-r6,r8-r9}

l000003CD:
	ldrsheq	pc,[r8]!

l000003D1:
	Invalid

l000003D5:
	stmeq	r7,{r3,r5-r7,ip-pc}

l000003D9:
	ldrshtvs	ip,[r8],#&E0

l000003DD:
	Invalid
	ldrb	r0,[r0,#&8FE]!
	mvnvs	ip,#&F8000000
	rsbeq	lr,r0,r10,ror #&A
	ldrbeq	lr,[r0],#&D2B
	strdeq	r2,r3,[r0],-r1
	ldrshteq	r1,[lr],#&20
	svcmi	#&D0E728
	ldmeq	r2,{r4-r7,pc}
	svclt	#&601A4B
	svclt	#&8F4FF3
	mcrle	p15,#4,r6,c15,c3,#7
	rscsne	r0,r1,r7,ror #9
	mvnseq	r0,#0
	Invalid
	Invalid
	ldrshtvs	r5,[pc],#&A0                                      ; 000004C5
	strbteq	sp,[r7],#&160
	rscne	r0,r0,sp,ror #1

;; uxQueueMessagesWaiting: 00000429
uxQueueMessagesWaiting proc
	stmeq	r6,{r0,r2,r4-r5,r7,r10}

l0000042D:
	ldrbtge	r10,[r8],#&4F0

l00000431:
	Invalid

l00000435:
	strdne	r2,r3,[r6],-#&8

l00000439:
	ldmlo	pc!,{r0,r2-r5,r7}

;; uxQueueSpacesAvailable: 0000043D
uxQueueSpacesAvailable proc
	stmeq	r6,{r0,r2,r4-r5,r7-r8,r10}

l00000441:
	ldmdbge	r8,{r4-r7,r9,fp-ip,pc}^

l00000445:
	strbths	lr,[fp],-#&C6B

l00000449:
	mvnslt	r0,r10,lsl r8

l0000044D:
	stmlo	r6,{r3-r7,sp}

l00000451:
	ldrheq	r0,[pc,sp]!                                         ; 00000459

;; vQueueDelete: 00000455
vQueueDelete proc
	Invalid

;; xQueueGenericSendFromISR: 00000459
xQueueGenericSendFromISR proc
	ldrhne	lr,[r3,#&F5]!

l0000045D:
	svclt	#&F04F86

l00000461:
	mvnsne	r8,r4,lsl #8

l00000465:
	svcvs	#&F3BF88

l00000469:
	svcmi	#&F3BF8F

l0000046D:
	strbtgt	r8,[fp],-#&58F

l00000471:
	strbeq	r10,[r2,-#&56B]

l00000475:
	msreq	cpsr,#&2D3

l00000479:
	Invalid

l0000047D:
	Invalid
	ldrbmi	r9,[r8,#&BD]!
	strbvs	r1,[r6],-#&740
	strbeq	r1,[r6,-#&AB2]
	Invalid
	Invalid
	ldrtvs	r0,[r4],-#&1D0
	ldrbmi	r8,[r8,#&5B2]!
	strthi	r0,[r0],-r0
	Invalid
	strhteq	r6,[r10],-#&BD
	ldrbeq	pc,[r0,#&82B]
	strdeq	r2,r3,[r0],-r1
	ldrshteq	fp,[sp],#&40
	sbcseq	pc,r0,r8,lsr #4
	bicseq	pc,r0,pc,lsr #&20
	Invalid
	ldmlo	pc!,{r0-r2,r5-r7}

;; xQueueGiveFromISR: 000004C5
xQueueGiveFromISR proc
	ldrhne	lr,[r3,#&F5]!

l000004C9:
	svclt	#&F04F84

l000004CD:
	mvnsne	r8,r3,lsl #6

l000004D1:
	svcvs	#&F3BF88

l000004D5:
	svcmi	#&F3BF8F

l000004D9:
	msrgt	spsr,#&28F

l000004DD:
	vmlseq.f32	s19,s4,s23

l000004E1:
	ldrbmi	r9,[r8,#&D2]!

l000004E5:
	blpl	$00C809AD

l000004E9:
	bpl	$018E0FB9

l000004ED:
	bicseq	r0,r0,ip,lsl fp

l000004F1:
	adcshi	r5,r2,r3,lsr fp

l000004F5:
	ldrsheq	r4,[r0,-r8]!

l000004F9:
	mvnsne	r8,r0,lsr #8

l000004FD:
	adcseq	r3,sp,r8,lsl #&11

l00000501:
	mvnsne	r8,r0,lsr #8

l00000505:
	movsmi	r3,#&880000

l00000509:
	Invalid
	ldceq	p4,c2,[r0,-#&340]!
	mvnshi	r0,#&46
	Invalid
	stc	p0,c0,[sp],-#&340
	stmdahs	r0!,{r4,r6-r8}
	Invalid

;; xQueueReceiveFromISR: 00000525
xQueueReceiveFromISR proc
	svc	#&41F0E9
	svcmi	#&8611F3

l0000052D:
	strhi	fp,[r4],-#&FF0

l00000531:
	svclt	#&8811F3

l00000535:
	svclt	#&8F6FF3

l00000539:
	strhi	r4,[pc],#&FF3                                        ; 00001534

l0000053D:
	adcshs	r2,r9,fp,ror #8

l00000541:
	mvnsne	r8,r6,asr #&C

l00000545:
	Invalid
	subls	r0,r6,r1,lsl #&F
	ldrshls	r4,[r0],-#&48
	svc	#&B26D46
	ldrsheq	r0,[lr,#&A7]!
	Invalid
	bicseq	r0,r0,r3,ror #&10
	Invalid
	ldrsheq	r4,[r0,-#&48]
	mvnsne	r8,r0,lsr #&C
	Invalid
	rsbeq	r3,r9,r1,lsl #&17
	ldrbeq	pc,[r0,fp,lsr #14]
	Invalid
	ldrshteq	r4,[sp],#&E0
	ldmdblt	r0,{r3,r5,r8,ip-pc}^
	Invalid
	stmdagt	r0!,{r4,r6-r8}
	Invalid
	adcshi	r0,pc,r7,ror #1

;; xQueueIsQueueEmptyFromISR: 00000595
xQueueIsQueueEmptyFromISR proc
	rscshi	fp,r10,fp,rrx

l00000599:
	strdvc	r4,r5,[r9],-r0

l0000059D:
	movshi	r0,#&47

;; xQueueIsQueueFullFromISR: 000005A1
xQueueIsQueueFullFromISR proc
	rsbgt	ip,fp,fp,rrx

l000005A5:
	rscshi	fp,r10,r10,lsl r0

l000005A9:
	strdvc	r4,r5,[r9],-r0

l000005AD:
	adcshi	r0,pc,r7,asr #&20

;; uxQueueMessagesWaitingFromISR: 000005B1
uxQueueMessagesWaitingFromISR proc
	subne	r7,r7,fp,rrx

;; xQueueGetMutexHolder: 000005B5
xQueueGetMutexHolder proc
	Invalid

l000005B9:
	mvnshs	sp,#&F00

l000005BD:
	ldrtvs	r2,[r9],#&368

l000005C1:
	Invalid
	strdne	r2,r3,[r6],-#&F
	Invalid
	ldrshths	pc,[pc]                                           ; 000005D5
	adcsvc	r1,sp,r6,asr #&20

;; xQueueTakeMutexRecursive: 000005D5
xQueueTakeMutexRecursive proc
	strbteq	r4,[r8],-#&5B5

l000005D9:
	subeq	r0,r6,r6,asr #&1C

l000005DD:
	ldrbhi	r10,[sp,#&CF0]!

l000005E1:
	sbcseq	r0,r0,r2,asr #&14

l000005E5:
	stmne	r6,{r0-r1,r5,r9,ip-sp}^

l000005E9:
	svc	#&462046
	ldrshtne	r7,[lr],#&47

l000005F1:
	strheq	lr,[r8,-#&31]!

l000005F5:
	rsbvc	lr,r0,r3,lsr r3

l000005F9:
	nop
	movt	r0,#&4368
	poplo	{r5-r6,ip-lr}

;; xQueueGiveMutexRecursive: 00000605
xQueueGiveMutexRecursive proc
	strbteq	r4,[r8],-#&5B5

l00000609:
	ldrbls	r0,[r0,#&46]!

l0000060D:
	strdeq	r8,r9,[r2,-#&5D]

l00000611:
	stmdalo	r0!,{r4,r6-r7}

l00000615:
	strheq	lr,[r8,-#&3D]!

l00000619:
	bleq	$0183930D

l0000061D:
	stmdalo	r0!,{r0,r4-r5,r7-r8}

l00000621:
	bne	$0118891D

l00000625:
	svc	#&461946
	ldrsheq	fp,[sp,#&27]!

l0000062D:
	adcsvc	r3,sp,r0,lsr #&10

;; xQueueGenericReset: 00000631
xQueueGenericReset proc
	Invalid

l00000635:
	streq	pc,[r5,-r6,asr #30]!

l00000639:
	ldrshteq	r9,[pc],#&E0                                      ; 00000721

l0000063D:
	rsb	r2,ip,#&84000000
	rsbeq	r2,r8,#&6B

l00000645:
	mvnsle	r0,#&EC000003

l00000649:
	subeq	r0,r4,#&68000000

l0000064D:
	rsbvs	r10,r3,#&11

l00000651:
	ldrbtmi	r8,[r8],#&460

l00000655:
	rsbge	lr,r0,r0,asr r3

l00000659:
	ldrbmi	r8,[r8,#&460]!

l0000065D:
	movshs	fp,#&500

l00000661:
	ldreq	r1,[r9,r9,ror #22]!

l00000665:
	Invalid

l00000669:
	ldrteq	r7,[sp],#&20

l0000066D:
	Invalid

l00000671:
	ldrshteq	sp,[ip],#&40

l00000675:
	svcmi	#&D0F528

l00000679:
	beq	$014A0A41

l0000067D:
	svclt	#&601A4B

l00000681:
	svclt	#&8F4FF3

l00000685:
	Invalid

l00000689:
	Invalid

l0000068D:
	ldrteq	r7,[sp],#&20

l00000691:
	Invalid

l00000695:
	ldrbteq	r1,[lr],#&CF0

l00000699:
	Invalid

l0000069D:
	Invalid

l000006A1:
	Invalid

l000006A5:
	ldrteq	r7,[sp],#&20

l000006A9:
	rscvc	r0,r0,sp,ror #1

;; xQueueGenericCreate: 000006AD
xQueueGenericCreate proc
	strheq	r0,[r6],-#&65

l000006B1:
	ldmdbmi	r0,{r0-r1,r3-r8}^

l000006B5:
	Invalid

l000006B9:
	ldrbteq	r3,[r8],#&8F0

l000006BD:
	ldrpl	r4,[r1,#&846]!

l000006C1:
	ldmdbmi	r1,{r0,r4-r5,r7}^

l000006C5:
	strbt	r0,[r0],-r3
	ereteq

l000006CD:
	svc	#&462021
	ldrshths	r10,[pc],#&E7                                     ; 000007C0

l000006D5:
	adcshs	r7,sp,r6,asr #&20

l000006D9:
	rscne	pc,r7,r0,ror #&A

;; xQueueCreateMutex: 000006DD
xQueueCreateMutex proc
	strheq	r0,[r6],-#&25

l000006E1:
	svc	#&200121
	ldrbteq	lr,[pc],#&2F7                                      ; 000009E4

l000006E9:
	adcseq	r3,r1,r6,asr #&10

l000006ED:
	msreq	spsr,#&323

l000006F1:
	bne	$01831479

l000006F5:
	svc	#&461946
	ldrshths	r4,[sp],#&A7

l000006FD:
	Invalid

;; prvInitialiseNewTask: 00000701
prvInitialiseNewTask proc
	Invalid

l00000705:
	strbhs	r9,[r6,-#&99C]

l00000709:
	rscshi	r0,r1,sp,ror #4

l0000070D:
	teqls	fp,#&C0000010

l00000711:
	ldreq	r0,[r10,#&A46]

l00000715:
	andhi	r8,r5,fp,ror #7

l00000719:
	svcmi	#&1E4B46

l0000071D:
	ldrbhs	sp,[r10,-#&2EA]!

l00000721:
	andeq	r0,r5,#&3C00000

l00000725:
	ldrbtpl	r0,[r1],#&431

l00000729:
	rscseq	r2,r0,r0,lsl #4

l0000072D:
	rsbseq	r5,r8,r2,asr #&1C

l00000731:
	msrne	spsr,#&1F8

l00000735:
	Invalid

l00000739:
	Invalid
	stmdahs	r10!,{r0,r4,r6-r8}
	strhteq	r0,[r2],-#&1F
	sub	r1,r6,#&980000
	strbteq	r10,[r5],-#&264
	strhi	r2,[r0],-#&4F1
	uqsub8	r5,r0,r8
	ldrbgt	r0,[r0,r5,ror #14]!
	ldmdblo	r1,{r0,r2-r7,r10}^
	mvnsgt	r0,#0
	rscseq	ip,r1,#&3F40000
	rsbhs	r10,r3,#&C000000
	Invalid
	ldrhs	r2,[sp],-#&99
	rsbeq	r6,r4,r3,ror #8
	Invalid
	strbhi	r5,[r6],-#&366
	bmi	$01819B65
	stmhs	r6,{r1-r2,r6,r8,lr}
	Invalid
	ldrshhs	r0,[fp],sp
	Invalid
	Invalid
	Invalid

;; prvAddNewTaskToReadyList: 00000799
prvAddNewTaskToReadyList proc
	stchs	p0,c15,[r1,-#&3A4]

l0000079D:
	strbeq	r0,[r6,-ip,asr #10]

l000007A1:
	mvnshs	lr,#&F0000

l000007A5:
	teqhs	r3,#&1A

l000007A9:
	rsbeq	r6,r8,r0,ror #6

l000007AD:
	bicsvs	r3,r0,#&2B

l000007B1:
	ldm	r3!,{r0-r3,r5-r6,r8-r9,fp,sp}
	ldmdbeq	r1,{r2-r3,r5-r6,r10}^

l000007B9:
	msr	cpsr,r6
	msrhi	spsr,#&26F

l000007C1:
	rschi	r0,fp,r0,asr #&20

l000007C5:
	mrseq	r11_usr,r0

l000007C9:
	rschi	r0,fp,r2,lsr r6

l000007CD:
	ldrbths	r0,[r1],#&500

l000007D1:
	rsbge	lr,r7,#&4000000

l000007D5:
	blhi	$FFC02579

l000007D9:
	ldmib	r0,{r0,r2-r10}^
	msrvs	spsr,#&3FE

l000007E1:
	bl	$01A192AD
	bls	$01B3519D

l000007E9:
	svcmi	#&D20742

l000007ED:
	ldmne	r2,{r4-r7,pc}^

l000007F1:
	svclt	#&601A4B

l000007F5:
	svclt	#&8F4FF3

l000007F9:
	Invalid

l000007FD:
	orrvs	pc,r1,#&E8

l00000801:
	blle	$01B3A9A9

l00000805:
	ldmdbeq	r1,{r2-r3,r5-r6,r10}^

l00000809:
	stmls	r2,{r1-r2,r8-r9,pc}

l0000080D:
	msrle	spsr,#&5BF

l00000811:
	msrhs	spsr,#&5E7

l00000815:
	stcgt	p1,c0,[fp],-#&1A0

l00000819:
	ldmdbeq	r1,{r0,r4,r6-r7,r10}^

l0000081D:
	strbeq	r3,[r6,-r6]

l00000821:
	ldrbteq	r5,[sp],#&6F0

l00000825:
	streq	r3,[r8],-#&F1

l00000829:
	Invalid

l0000082D:
	ldrbteq	r5,[sp],#&F0

l00000831:
	strdmi	r4,r5,[r7],-r1

l00000835:
	blmi	$FFC02555

l00000839:
	Invalid

l0000083D:
	ldrbteq	r4,[sp],#&8F0

l00000841:
	Invalid

l00000845:
	ldrbtgt	r4,[sp],#&4F0

l00000849:
	stm	r0,{r3-r7,r10-fp,sp-lr}
	msrlt	spsr,#&76C

l00000851:
	ldrtgt	r0,[pc],#&E7                                        ; 00000940

l00000855:
	strteq	r0,[r0]

l00000859:
	rscvc	r0,r0,sp,ror #1

;; prvAddCurrentTaskToDelayedList.isra.0: 0000085D
prvAddCurrentTaskToDelayedList.isra.0 proc
	strbeq	r1,[ip,-#&4B5]

l00000861:
	rscshi	sp,r8,r6,asr #8

l00000865:
	strbths	r6,[r8],-#&60

l00000869:
	ldmibvs	r0,{r4-r5,r8-r10}^

l0000086D:
	ldrsheq	r3,[r9,sp]!

l00000871:
	msr	spsr,#&122
	bhi	$01B32E39

l00000879:
	rsceq	r2,r10,#1

l0000087D:
	strblo	lr,[r7,-#&303]!

l00000881:
	Invalid

l00000885:
	bleq	$01897D95

l00000889:
	ldrdvs	lr,pc,[lr,-#&8]!

l0000088D:
	ldreq	r2,[r1,-r8,ror #8]!

l00000891:
	ldrbtle	r3,[sp],#&CF0

l00000895:
	ldcls	p4,c8,[r0,-#&3E0]!

l00000899:
	ldrtgt	r3,[pc],#&842                                       ; 000010E3

l0000089D:
	ldrshvc	r8,[r0],-#&48

l000008A1:
	strhvs	r2,[pc,-#&D]!                                       ; 000008B6

l000008A5:
	rscvc	fp,r8,r8,ror #&1A

l000008A9:
	ldreq	r2,[r1,-r0,asr #8]!

l000008AD:
	ldrtgt	r2,[sp],#&EF0

l000008B1:
	stchs	p0,c0,[r0]!

;; xTaskCreate: 000008B5
xTaskCreate proc
	subhi	pc,r7,r9,ror #1

l000008B9:
	adcsls	r8,r0,r6,asr #8

l000008BD:
	stmhi	r6,{r9-r10,ip}^

l000008C1:
	subeq	r9,r6,r6,asr #&14

l000008C5:
	ldrsht	r3,[pc],#&20                                        ; 000008ED
	stmvs	r6,{r0,r4-r5,r7-r8,r10}

l000008CD:
	Invalid

l000008D1:
	stmle	r6,{r0-r7,r10}

l000008D5:
	streq	r0,[r7,-#&B1]!

l000008D9:
	ldrhi	r0,[sp],#&D65

l000008DD:
	ldrsheq	r6,[r0,-#&58]!

l000008E1:
	orrspl	r0,sp,#&9500

l000008E5:
	stmmi	r6,{r1-r2,r6,r9,ip-sp}^

l000008E9:
	movteq	r4,#&6046

l000008ED:
	umullseq	r0,r4,r7,r2

l000008F1:
	ldrbeq	pc,[r7,#&F95]!

l000008F5:
	svc	#&4620FF
	Invalid

l000008FD:
	Invalid

l00000901:
	svcmi	#&87F0E8

l00000905:
	ldrteq	pc,[r0],-#&FF0

l00000909:
	Invalid
	subeq	r2,r6,r7,lsl #&11
	svcmi	#&FF36F0
	Invalid
	movsmi	r0,#&E7

;; xTaskCreateRestricted: 0000091D
xTaskCreateRestricted proc
	Invalid
	strbhi	r0,[r6,-#&4B5]
	svceq	#&2068B0
	svc	#&F00046
	strdgt	r0,r1,[r6],-#&5E
	strhvs	r0,[r6,-r1]!
	ldrbvs	r8,[r8,#&69]!
	rsbhs	lr,r8,#&80000001
	rscsne	sp,r8,r9,lsl #9
	smulttvs	r5,r0,r1
	orrseq	r0,r0,r8,ror #4
	ldmdbne	r8,{r0-r2,r4,r7,r10,ip,lr}^
	rscseq	ip,r8,fp,lsl #&1A
	svc	#&9403E0
	ldmdbhs	lr,{r0-r2,r4-r7,r10,ip,lr-pc}^
	Invalid
	strbeq	r3,[r6,-#&FF]
	svcmi	#&BDF0B0
	blx	$00C4092D
	svc	#&F04FE7
	subhs	r7,r7,r0,lsr r0

;; vTaskAllocateMPURegions: 00000971
vTaskAllocateMPURegions proc
	strteq	r0,[r3],-#&B1

l00000975:
	subeq	r1,r6,r0,lsr r10

l00000979:
	movseq	lr,#&F000

l0000097D:
	rsbeq	r5,r8,fp,asr #&10

l00000981:
	bne	$00C01A15

l00000985:
	ldrb	r0,[r0,#&46]!
	ldrtgt	r0,[pc],#&BD                                        ; 00000A4E

l0000098D:
	svcmi	#&200000

;; vTaskStartScheduler: 00000991
vTaskStartScheduler proc
	strdne	r0,r1,[r3]

l00000995:
	subhi	r1,ip,#&5000000B

l00000999:
	ldreq	r0,[r3],#&B0

l0000099D:
	strdeq	r8,r9,[r3,-r1,ror #17]

l000009A1:
	mlaeq	r2,r3,fp,r3

l000009A5:
	svceq	#&490F23

l000009A9:
	mvnshi	pc,#&120

l000009AD:
	strdeq	r0,r1,[r8,-pc,ror #3]!

l000009B1:
	ldrsbtne	r0,[r0],#&20

l000009B5:
	svclt	#&F04FBD

l000009B9:
	mvnsne	r8,r3,lsl #6

l000009BD:
	svcvs	#&F3BF88

l000009C1:
	svcmi	#&F3BF8F

l000009C5:
	svc	#&F04F8F
	strtgt	r0,[r3],-#&32

l000009CD:
	Invalid

l000009D1:
	rscshi	ip,r8,r7,ror #8

l000009D5:
	Invalid

l000009D9:
	subeq	r1,r0,r8,ror #1

l000009DD:
	ldrtgt	lr,[ip],#&8F0

l000009E1:
	stcvc	p0,c0,[r0]

l000009E5:
	stchs	p0,c0,[r0,-#&288]

l000009E9:
	svcmi	#&85

;; vTaskEndScheduler: 000009ED
vTaskEndScheduler proc
	movhi	fp,#&3FF0

l000009F1:
	svclt	#&8811F3

l000009F5:
	svclt	#&8F6FF3

l000009F9:
	strdeq	r4,r5,[pc],r3                                       ; 00000A01

l000009FD:
	bpl	$012C128D

l00000A01:
	ldrbge	r0,[r0,#&67]!

l00000A05:
	ldrtgt	r0,[pc],#&BD                                        ; 00000ACA

l00000A09:
	nopeq

;; vTaskSuspendAll: 00000A0D
vTaskSuspendAll proc
	Invalid

l00000A11:
	eorsgt	r0,r3,#&C

l00000A15:
	ldrshtvc	r8,[r0],-#&C8

l00000A19:
	ldrtgt	r0,[pc],#&47                                        ; 00000A68

l00000A1D:
	msreq	cpsr,r0

;; xTaskGetTickCount: 00000A21
xTaskGetTickCount proc
	rscshi	sp,r8,fp,asr #6

l00000A25:
	strbgt	r7,[r7]

l00000A29:
	msreq	cpsr,r0

;; xTaskGetTickCountFromISR: 00000A2D
xTaskGetTickCountFromISR proc
	rscshi	sp,r8,fp,asr #6

l00000A31:
	strbgt	r7,[r7]

l00000A35:
	msreq	cpsr,r0

;; uxTaskGetNumberOfTasks: 00000A39
uxTaskGetNumberOfTasks proc
	rsbvc	r1,r8,fp,asr #&10

l00000A3D:
	ldrtgt	r0,[pc],#&47                                        ; 00000A8C

l00000A41:
	stmdaeq	r0!,}

;; pcTaskGetName: 00000A45
pcTaskGetName proc
	ldrhtvc	r5,[r0],-#&41

l00000A49:
	stmpl	fp,{r0-r2,r6,r9}

l00000A4D:
	eorsvc	r5,r0,r8,ror #8

l00000A51:
	ldrtgt	r0,[pc],#&47                                        ; 00000AA0

l00000A55:
	Invalid

;; xTaskGenericNotify: 00000A59
xTaskGenericNotify proc
	Invalid

l00000A5D:
	strbne	r0,[r6,-#&F46]

l00000A61:
	ldmibhi	r0,{r1-r2,r6,r8-r10}^

l00000A65:
	movslo	r0,#&FD00

l00000A69:
	rsbeq	r2,r0,#&B8000001

l00000A6D:
	ldrbtvs	r9,[r8],#&623

l00000A71:
	ldrhi	r6,[lr],-r0

l00000A75:
	ldrt	r6,[r0],-#&4F8
	Invalid

l00000A7D:
	rsceq	sp,r8,#&360

l00000A81:
	streq	r3,[ip],-#&AF0

l00000A85:
	stmdblo	ip!,{r1,r9}

l00000A89:
	Invalid

l00000A8D:
	bicseq	r0,r0,ip,lsr #&14

l00000A91:
	Invalid

l00000A95:
	Invalid
	strheq	r3,[lr,-#&3D]!
	mvnseq	r0,ip,lsr #6
	Invalid
	ldrbths	r0,[r1],#&6D1
	stmlo	sp,{r0-r2,r9-r10,ip}
	ldrbmi	r0,[r0,r6,asr #14]!
	strble	pc,[ip,-#&FC]!
	strbeq	r7,[r0,#&CF8]!
	streq	r0,[r2],-#&8F1
	ldrshteq	r0,[r3],#&A
	movmi	r8,#&EB
	andeq	r0,r3,#&EA0
	stmdblo	r0,{r0-r1,r3,r5-r7,pc}
	strbeq	lr,[r7,-r6,asr #22]!
	blvs	$FFF04699
	blle	$01B3D47D
	stmle	r2,{r2-r3,r5-r6,r9,fp-ip,pc}
	ldrsbthi	r4,[r0],#&F9
	bne	$012C3031
	svcmi	#&F3BF60
	svcvs	#&F3BF8F
	svcpl	#&F0078F
	Invalid
	svcne	#&6E33BD
	strbtgt	r3,[r6],-r3
	strgt	r0,[r4,-r7,ror #1]!
	ldrtgt	r0,[pc],#&E7                                        ; 00000BF0
	strteq	r0,[r0]
	Invalid

;; xTaskGenericNotifyFromISR: 00000B0D
xTaskGenericNotifyFromISR proc
	svc	#&41F0E9
	svcmi	#&8511F3

l00000B15:
	strhi	fp,[r4],-#&FF0

l00000B19:
	svclt	#&8811F3

l00000B1D:
	svclt	#&8F6FF3

l00000B21:
	bleq	$FE3D4AF5

l00000B25:
	stcne	p4,c0,[lr],-#&2C4

l00000B29:
	eorls	r0,r3,r0,ror #4

l00000B2D:
	strdeq	r6,r7,[r0,-#&48]

l00000B31:
	ldrbtvs	r8,[r8],#&3A

l00000B35:
	movseq	lr,#&30000000

l00000B39:
	svcle	#&D8062A

l00000B3D:
	bhs	$FFC016E5

l00000B41:
	andeq	r0,r2,#&C000000

l00000B45:
	bicseq	r3,r0,ip,lsr #&1A

l00000B49:
	beq	$00B010E9

l00000B4D:
	strhi	r0,[r0,-#&1D0]!

l00000B51:
	Invalid

l00000B55:
	orreq	pc,r1,#&E8

l00000B59:
	msreq	cpsr,#&16E

l00000B5D:
	moveq	r0,#&31F1

l00000B61:
	bne	$FF47DD01

l00000B65:
	strble	r0,[r6],-lr

l00000B69:
	teqlt	r0,#&F800

l00000B6D:
	ldmdblo	r1,{r0,r4-r5,r7}^

l00000B71:
	ldmdbpl	r1,{r0,r9-r10}^

l00000B75:
	bllt	$FFC0277D

l00000B79:
	blx	$01A1DB6D
	bls	$01B37935

l00000B81:
	ldrbeq	lr,[r9],r2

l00000B85:
	Invalid
	strbhi	r1,[r0,-#&8B1]!
	Invalid
	orreq	pc,r1,#&E8
	Invalid
	rsceq	sp,r7,r6,ror #&C
	strdmi	r2,r3,[r8],-r1
	Invalid
	Invalid
	strbeq	r8,[r0],-pc
	strdeq	r0,r1,[r3],-r1
	strne	r8,[r0],-#&EB
	movteq	r4,#&6143
	Invalid
	ldrbls	r0,[r0,r7,ror #14]!
	Invalid
	strbtgt	ip,[r7],r0
	Invalid
	ldrtgt	r0,[pc],#&E7                                        ; 00000CBC
	stchs	p0,c0,[r0]!

;; xTaskNotifyWait: 00000BD5
xTaskNotifyWait proc
	svcne	#&41F0E9

l00000BD9:
	subhi	r1,r6,ip,asr #&A

l00000BDD:
	svcne	#&460E46

l00000BE1:
	ldmibgt	r0,{r1-r2,r6,r8-r10}^

l00000BE5:
	rsbls	r6,r8,#&C000000F

l00000BE9:
	eoreq	r6,r0,#&F8000000

l00000BED:
	bicseq	r0,r0,r10,lsr #&12

l00000BF1:
	beq	$01A19079

l00000BF5:
	stmdbeq	r10,{r1-r3,r5-r6,r9,sp}^

l00000BF9:
	msrvs	spsr,#&A02

l00000BFD:
	ldrbtvs	r8,[r8],#&368

l00000C01:
	ldreq	sp,[r9,r0,lsl #30]!

l00000C05:
	Invalid

l00000C09:
	ldrbne	fp,[ip,#&6F0]!

l00000C0D:
	blne	$01A19AD9

l00000C11:
	msrvs	spsr,#&B6E

l00000C15:
	ldrbtvs	r9,[r8],#&368

l00000C19:
	bne	$00AC10E1

l00000C1D:
	msrvs	cpsr,#&1D0

l00000C21:
	ereths

l00000C25:
	stmdbne	r1,{r1,r3,r5-r7,r9-r10}

l00000C29:
	msrvs	cpsr,#&66

l00000C2D:
	ldrbtvs	r8,[r8],#&368

l00000C31:
	Invalid

l00000C35:
	Invalid

l00000C39:
	stmlo	r1,{r3,r5-r7,ip-pc}

l00000C3D:
	Invalid

l00000C41:
	ldrshthi	r4,[r0],#&FE

l00000C45:
	bne	$012C2195

l00000C49:
	svcmi	#&F3BF60

l00000C4D:
	svcvs	#&F3BF8F

l00000C51:
	rsceq	sp,r7,pc,lsl #&F

l00000C55:
	strbtgt	lr,[r7],#&825

l00000C59:
	strteq	r0,[r0]

l00000C5D:
	Invalid

;; vTaskNotifyGiveFromISR: 00000C61
vTaskNotifyGiveFromISR proc
	svc	#&43F8E9
	svcmi	#&8611F3

l00000C69:
	movhi	fp,#&3FF0

l00000C6D:
	svclt	#&8811F3

l00000C71:
	svclt	#&8F6FF3

l00000C75:
	addeq	r4,pc,#&3CC

l00000C79:
	ldrbtvs	r9,[r8],#&23

l00000C7D:
	ldrbtvs	r8,[r8],#&50

l00000C81:
	stc	p3,c0,[lr,-#&C0]!
	ldrheq	r0,[r3,-r2]!

l00000C89:
	msreq	spsr,#&32D

l00000C8D:
	ldrsbne	r8,[r3,#&60]!

l00000C91:
	Invalid
	stmhi	pc,{r0-r1,r7-r8,fp-ip}
	Invalid
	movtge	r0,#&6430
	ldmdblo	r1,{r0,r4-r5,r7}^
	ldmdbpl	r1,{r0,r8-r10}^
	mvnshs	r0,r0,lsl #&E
	rsb	r7,r8,#&3EC00
	bls	$01B37A69
	bicseq	lr,r9,r2,asr #&14
	rscseq	fp,r1,r3,lsr #&10
	ldmdbgt	r0,{r0-r3,r8,fp-ip}^
	Invalid
	Invalid
	addeq	pc,r3,r8,ror #&11
	stmdami	r9,{r0,r4-r7,r10,sp}
	ldrblo	r0,[r0,#&746]!
	blx	$01B390C9
	strbeq	r8,[r0,-pc,ror #10]
	strdeq	r0,r1,[r3],-r1
	strne	r8,[r0,-#&EB]
	movteq	r4,#&6943
	Invalid
	svc	#&F00767
	Invalid
	ldmdbgt	r0!,{r3-r7,ip,pc}
	ldrtgt	r0,[pc],#&E7                                        ; 00000DE8
	eorvc	r0,r0,r0

;; ulTaskNotifyTake: 00000D01
ulTaskNotifyTake proc
	Invalid

l00000D05:
	strbeq	r0,[r6,-r6,asr #26]

l00000D09:
	mvnsvs	r3,#&F000000

l00000D0D:
	msrhs	spsr,#&B68

l00000D11:
	msrvs	cpsr,#&1B9

l00000D15:
	ldrbtvs	r8,[r8],#&368

l00000D19:
	ldreq	fp,[r9,r0,lsr #10]!

l00000D1D:
	Invalid

l00000D21:
	mvnsvs	r2,#&F0000

l00000D25:
	stcne	p13,c1,[lr,-#&1A0]!

l00000D29:
	movsvs	r5,#&B100000

l00000D2D:
	bne	$0079B6D5

l00000D31:
	msrvs	cpsr,#&66

l00000D35:
	ldrbtvs	r8,[r8],#&368

l00000D39:
	ldmiblo	r0,{r5,r8-r10}^

l00000D3D:
	strdvc	r2,r3,[r6],-#&8C

l00000D41:
	msrvs	cpsr,#&BD

l00000D45:
	Invalid
	svc	#&4628E7
	svcmi	#&FD86F7
	ldrbeq	r8,[r2],-#&F0
	svclt	#&601A4B
	svclt	#&8F4FF3
	stcle	p15,c6,[pc],#&3CC                                    ; 00001131
	ldrtgt	r0,[pc],#&E7                                        ; 00000E50
	strteq	r0,[r0]
	Invalid

;; xTaskIncrementTick: 00000D6D
xTaskIncrementTick proc
	mcrrlo	p0,#&E,pc,r7,c9

l00000D71:
	Invalid

l00000D75:
	Invalid

l00000D79:
	ldrsbthi	sp,[r8],#&41

l00000D7D:
	ldrtgt	r0,[r7],-#&170

l00000D81:
	Invalid

l00000D85:
	rsbhs	lr,lr,#&E4000002

l00000D89:
	msrhs	spsr,#&26F

l00000D8D:
	ldrbtls	sp,[r8],#&467

l00000D91:
	ldrtgt	r0,[r3],-#&130

l00000D95:
	teq	r0,#&F8000000
	rsbeq	r1,r8,lr,ror #&16

l00000D9D:
	svcmi	#&D1522B

l00000DA1:
	ldrtgt	pc,[r3],-#&FF0

l00000DA5:
	ldrtle	r8,[r0],-#&4F8

l00000DA9:
	ldrshteq	r8,[r0],-#&48

l00000DAD:
	sublo	r9,r2,r6,lsr #&1E

l00000DB1:
	ldrsbeq	r4,[r0,#&F3]!

l00000DB5:
	rscslt	sp,r8,r9,lsl #&1E

l00000DB9:
	mvn	r2,#2
	vstmdble	r8!,{d29-d31}

l00000DC1:
	strbeq	r6,[r10,-#&B68]!

l00000DC5:
	svcls	#&A24F1

l00000DC9:
	sbcspl	r4,r3,r2,asr #&10

l00000DCD:
	ldrblt	r0,[r0,r6,asr #14]!

l00000DD1:
	strbeq	r10,[ip,-#&BFA]!

l00000DD5:
	bleq	$0000F1A1

l00000DD9:
	ldrhlt	r0,[r0,#&71]!

l00000DDD:
	rsb	lr,ip,#&FA0000
	rscseq	r0,r10,pc,ror #&12

l00000DE5:
	Invalid

l00000DE9:
	mrspl	r11_usr,r0

l00000DED:
	rschi	r0,fp,r6,asr #&10

l00000DF1:
	strbeq	lr,[r7,-r0,lsl #6]!

l00000DF5:
	mvnsvs	r7,#&F000

l00000DF9:
	blle	$01B3B7A1

l00000DFD:
	stmhs	r2,{r2-r3,r5-r6,r9,fp-ip,pc}

l00000E01:
	msr	cpsr,#&1BF
	rsbeq	r1,r8,lr,ror #&16

l00000E09:
	svcmi	#&D1D72B

l00000E0D:
	ldrtgt	pc,[r3],-#&FF0

l00000E11:
	teqvs	r0,#&F8000000

l00000E15:
	msreq	spsr,#&B68

l00000E19:
	streq	r8,[r3],-#&3EB

l00000E1D:
	blls	$000E1DD1

l00000E21:
	stmdahs	fp!,{r3,r5-r6,r9}

l00000E25:
	strtle	r0,[r6],-#&1BF

l00000E29:
	ldrshteq	r9,[r0],-#&8

l00000E2D:
	lsrseq	r1,fp,#&10

l00000E31:
	stclt	p0,c3,[r6,-#&98]

l00000E35:
	strle	pc,[r7],#&E8

l00000E39:
	ldrshteq	r9,[r0],-#&88

l00000E3D:
	ldrtgt	r0,[r3],-#&126

l00000E41:
	Invalid
	rsbeq	lr,lr,r7,ror #7
	blle	$01A37AE9
	strbtgt	r5,[r10],-#&B68
	ldrtle	r8,[r0],-#&4F8
	svcls	#&3084F8
	ldmibge	r3,{r1,r6,r8-r9,fp-ip,lr-pc}^
	ldrbthi	ip,[r8],#&4E7
	strbtgt	sp,[r7],#&730
	stcgt	p0,c0,[r0]
	stchs	p0,c0,[r0]!

;; xTaskResumeAll: 00000E6D
xTaskResumeAll proc
	movtlo	pc,#&10E9

l00000E71:
	mvnshi	r0,ip,asr #&E

l00000E75:
	Invalid

l00000E79:
	ldrtgt	r0,[fp],-#&130

l00000E7D:
	ldrtle	r8,[r0],-#&CF8

l00000E81:
	ldrsheq	r8,[r0],-#&C8

l00000E85:
	bicshs	r4,r1,#&2D0

l00000E89:
	blmi	$00AC1031

l00000E8D:
	strteq	r0,[r6],-#&1D0

l00000E91:
	vmovne	s15,r0

l00000E95:
	stcle	p3,c6,[lr,-#&380]!

l00000E99:
	ldrbths	r0,[r1],#&568

l00000E9D:
	ldmdblo	r1,{r3,r8,r10}^

l00000EA1:
	Invalid

l00000EA5:
	Invalid

l00000EA9:
	ldmdb	r10,{r4-r7,r9,fp,lr}^
	strbteq	lr,[pc],-ip                                        ; 00000EB5

l00000EB1:
	ldrshteq	r0,[r3],#&A

l00000EB5:
	movne	r8,#&EB

l00000EB9:
	strbeq	r4,[r6,-r3,asr #2]

l00000EBD:
	mov	r8,#&EB
	ldrbne	r0,[r0,#&767]!

l00000EC5:
	b	$01A19EB5
00000EC9                            6C DB 6C 9A 42 28 BF          l.l.B(.
00000ED0 C4 F8 90 60 A3 6D 00 2B DD D1 35 B1 E3 6E 1B 68 ...`.m.+..5..n.h
00000EE0 3B BB 4F F0 FF 33 C4 F8 84 30 D4 F8 98 50 4D B1 ;.O..3...0...PM.
00000EF0 01 26 FF F7 3B FF 08 B1 C4 F8 90 60 01 3D F8 D1 .&..;......`.=..
00000F00 C4 F8 98 50 D4 F8 90 30 6B B1 4F F0 80 52 0D 4B ...P...0k.O..R.K
00000F10 1A 60 BF F3 4F 8F BF F3 6F 8F 01 24 07 F0 48 FB .`..O...o..$..H.
00000F20 20 46 BD E8 F0 81 00 24 07 F0 42 FB 20 46 BD E8  F.....$..B. F..
00000F30 F0 81 E3 6E DB 68 DB 68 5B 6A C4 F8 84 30 D4 E7 ...n.h.h[j...0..
00000F40 C4 00 00 20 04 ED 00 E0 08                      ... .....      

;; vTaskDelay: 00000F49
vTaskDelay proc
	svcmi	#&B940B5

l00000F4D:
	ldmeq	r2,{r4-r7,pc}^

l00000F51:
	svclt	#&601A4B

l00000F55:
	svclt	#&8F4FF3

l00000F59:
	stmeq	pc,{r0-r1,r4-fp,sp-lr}

l00000F5D:
	suble	r0,r10,#&2F40000

l00000F61:
	ldrsheq	r8,[r0,-r8]!

l00000F65:
	Invalid

l00000F69:
	Invalid

l00000F6D:
	Invalid

l00000F71:
	b	$00A01375
00000F75                D0 08 BD 04 ED 00 E0 C4 00 00 20      .......... 
00000F80 14                                              .              

;; vTaskDelayUntil: 00000F81
vTaskDelayUntil proc
	adcsle	r1,r5,#&4A

l00000F85:
	movteq	r8,#&CF8

l00000F89:
	eorsgt	r0,r4,#&1A

l00000F8D:
	suble	r8,r0,#&F800

l00000F91:
	stmdbne	r0!,{r3-r7,pc}

l00000F95:
	vmlseq.f32	s19,s4,s8

l00000F99:
	vmoveq.i8	d18,r8

l00000F9D:
	svc	#&6001D8
	ldrshtge	r6,[pc],#&47                                      ; 00000FF0

l00000FA5:
	ldrhthi	r4,[r0],#&F9

l00000FA9:
	bne	$012C3CF9

l00000FAD:
	svcmi	#&F3BF60

l00000FB1:
	svcvs	#&F3BF8F

l00000FB5:
	blhi	$FEF451F9

l00000FB9:
	bhi	$FF6014C9

l00000FBD:
	bicseq	lr,r2,r2,asr #&1C

l00000FC1:
	svc	#&1A8860
	svc	#&FC4AF7
	ldrshteq	r5,[pc],#&7                                       ; 00000FD8

l00000FCD:
	sbcsne	lr,r0,r8,lsr #&14

l00000FD1:
	ldrtgt	r0,[pc],#&BD                                        ; 00001096

l00000FD5:
	strteq	r0,[r0]

l00000FD9:
	rscne	r0,r0,sp,ror #1

;; vTaskPlaceOnEventList: 00000FDD
vTaskPlaceOnEventList proc
	strbeq	r0,[r6],-#&CB5

l00000FE1:
	stmlo	r8!,{r0-r1,r3,r6,r8,fp-ip,lr}

l00000FE5:
	mvnsls	r0,r1,lsr r7

l00000FE9:
	stclt	p0,c2,[r6,-#&3E4]

l00000FED:
	strblo	r1,[r0],-#&E8

l00000FF1:
	ldrtgt	r0,[pc],#&E4                                        ; 000010DD

l00000FF5:
	stmdalo	r0!,}

;; vTaskPlaceOnUnorderedEventList: 00000FF9
vTaskPlaceOnUnorderedEventList proc
	Invalid

l00000FFD:
	rscseq	r4,r0,fp,asr #2

;; fn00000FFF: 00000FFF
fn00000FFF proc
	stcpl	p0,c0,[r1,-#&3C0]

;; fn00001001: 00001001
fn00001001 proc
	blpl	$01A1850D

l00001003:
	stmge	r8,{r3,r5-r6,r8-r9,fp-ip,lr}^

;; fn00001005: 00001005
fn00001005 proc
	msreq	spsr,#&968

l00001007:
	ldmdblo	r1,{r0-r1,r5-r6,r8-r9}^

;; fn00001009: 00001009
fn00001009 proc
	Invalid

l0000100B:
	rscsvc	r0,r0,r1,lsl #&E

;; fn0000100D: 0000100D
fn0000100D proc
	ldrshths	r7,[r9]

l0000100F:
	stclt	p0,c2,[r6,-#&3E4]

;; fn00001011: 00001011
fn00001011 proc
	stmdblo	r8,{r1-r2,r6,r8,r10-sp,pc}^

l00001013:
	smlaltths	r3,r0,r8,r8

;; fn00001015: 00001015
fn00001015 proc
	strbtgt	r2,[r4],#&140

l00001017:
	andeq	ip,r0,r4,ror #9

;; fn00001019: 00001019
fn00001019 proc
	Invalid

l0000101B:
	movsgt	pc,#&200000

;; xTaskRemoveFromEventList: 0000101D
xTaskRemoveFromEventList proc
	Invalid

l0000101F:
	stcle	p6,c1,[ip,-#&1A0]

;; fn00001021: 00001021
fn00001021 proc
	strbeq	sp,[r8,-#&D4C]!

l00001023:
	ldmdblo	r1,{r3,r5-r6,r8,r10}^

;; fn00001025: 00001025
fn00001025 proc
	Invalid

l00001027:
	strbeq	r3,[r6,-r6]

;; fn00001029: 00001029
fn00001029 proc
	ldmibhi	r0,{r1-r2,r6,r8-r10}^

l0000102B:
	ldrbtle	r8,[r9],#&9F0

;; fn0000102D: 0000102D
fn0000102D proc
	Invalid

l0000102F:
	bl	$00C24417

;; fn00001031: 00001031
fn00001031 proc
	ldreq	lr,[r9,#&B30]!

l00001035:
	strdlo	r2,r3,[r6],-r1

l00001037:
	strbeq	r3,[r6,-r6]

;; fn00001039: 00001039
fn00001039 proc
	mvnshi	r0,r6,asr #&E

l0000103B:
	ldrsheq	r8,[r9,#&10]!

;; fn0000103D: 0000103D
fn0000103D proc
	stmda	r3!,{r0,r3-r8}

l0000103F:
	strb	lr,[ip,-r3,lsr #16]!

;; fn00001041: 00001041
fn00001041 proc
	msrhi	spsr,#&76C

l00001045:
	ldmdbeq	r1,{r6,r10}^

l00001047:
	strdeq	r0,r1,[r2],-r1

;; fn00001049: 00001049
fn00001049 proc
	rschi	r0,fp,r2

l0000104B:
	bllo	$000213FF

;; fn0000104D: 0000104D
fn0000104D proc
	mrslo	r11_usr,r0

l0000104F:
	subeq	r3,r6,#&C0000010

;; fn00001051: 00001051
fn00001051 proc
	rschi	r0,fp,r6,asr #4

l00001053:
	mov	r8,#&EB

l00001055:
	strbeq	lr,[r7,-r0,lsl #6]!
	mvnsvs	r4,#&F0000

l0000105B:
	b	$01A1A047
0000105F                                              6C                l
00001060 DB 6C 9A 42 86 BF 01 20 C4 F8 90 00 00 20 F8 BD .l.B... ..... ..
00001070 31 46 04 F1 58 00 07 F0 3B F9 EF E7 C4 00 00 20 1F..X...;...... 
00001080 F8                                              .              

;; xTaskRemoveFromUnorderedEventList: 00001081
xTaskRemoveFromUnorderedEventList proc
	Invalid

l00001085:
	rscseq	r4,r0,r8,ror #2

l00001089:
	strbteq	r0,[r0],-r1

l0000108D:
	Invalid

l00001091:
	svceq	#&F956F0

l00001095:
	strbeq	r3,[r6,-ip,asr #16]

l00001099:
	Invalid
	Invalid
	mvnseq	r0,#&38000000
	ldmdbeq	r1,{r1,r4-r7,r10}^
	mvnhi	r0,#0
	mvnhi	r0,#3
	Invalid
	sub	r3,r6,#&8000
	ldmibne	r0,{r0-r2,r5-r6,r8-r10}^
	Invalid
	bls	$01B37E79
	ldmhs	pc!,{r1,r6,r9-r10,pc}
	rscsls	ip,r8,r6,asr #8
	Invalid
	ldrtgt	r0,[pc],#&BD                                        ; 00001196
	eorne	r0,r0,r0

;; vTaskSwitchContext: 000010D9
vTaskSwitchContext proc
	Invalid

l000010DD:
	adcsgt	ip,r9,#&C0000000

l000010E1:
	teqle	r0,#&F8

l000010E5:
	mvnshi	fp,#&BC000001

l000010E9:
	movsgt	sp,#&3CC00

l000010ED:
	moveq	r1,#&3FF1

l000010F1:
	blls	$000E20A5

l000010F5:
	tsteq	r8,#0

l000010F9:
	rsbne	ip,r8,r6,asr #2

l000010FD:
	stmls	r8,{r0-r1,r4-r5,r8,fp,lr}^

l00001101:
	stmeq	r0!,{r1,r6,r8,lr-pc}

l00001105:
	blgt	$01A13809

l00001109:
	rorsgt	r0,r8,#&10

l0000110D:
	rsbvc	r5,r0,r0,ror #6

l00001111:
	eorgt	r0,r3,#&C0000011

l00001115:
	ldrshtvc	r9,[r0],-#&8

l00001119:
	ldrtgt	r0,[pc],#&47                                        ; 00001168

l0000111D:
	strteq	r0,[r0]

;; uxTaskResetEventItemValue: 00001121
uxTaskResetEventItemValue proc
	bpl	$01A17655

l00001125:
	stmhi	r8!,{r3,r5-r6,r8-r9,fp-ip,lr}

l00001129:
	msrgt	spsr,#&B6B

l0000112D:
	movls	r0,#&32F1

l00001131:
	strbgt	r7,[r7],-#&63

l00001135:
	msreq	cpsr,r0

;; xTaskGetCurrentTaskHandle: 00001139
xTaskGetCurrentTaskHandle proc
	rsbvc	r5,r8,fp,asr #&10

l0000113D:
	ldrtgt	r0,[pc],#&47                                        ; 0000118C

l00001141:
	nopeq

;; vTaskSetTimeOutState: 00001145
vTaskSetTimeOutState proc
	ldrbtls	sp,[r8],#&34B

l00001149:
	rscshi	sp,r8,r0,lsr #6

l0000114D:
	Invalid

l00001151:
	strbgt	r7,[r7]

l00001155:
	eorvc	r0,r0,r0

;; xTaskCheckForTimeOut: 00001159
xTaskCheckForTimeOut proc
	Invalid

l0000115D:
	bleq	$FFC02E7D

l00001161:
	Invalid

l00001165:
	rscshi	sp,r8,r8,ror #6

l00001169:
	ldrbtls	sp,[r8],#&350

l0000116D:
	Invalid

l00001171:
	ldrbhi	r0,[r0,#&142]

l00001175:
	sbcslo	r1,r2,#&80000010

l00001179:
	tstls	r10,r8,ror #&12

l0000117D:
	sbcspl	r0,r2,#&1080

l00001181:
	msrle	cpsr,#&1B

l00001185:
	tstle	r0,#&F8000000

l00001189:
	eorseq	r8,r0,#&F8

l0000118D:
	strbthi	r3,[r0],-#&244

l00001191:
	streq	r0,[r0,-r8,ror #21]

l00001195:
	ldmdbhs	r10,{r4-r7,r10-fp}^

l00001199:
	asrseq	r7,r6,#&20

l0000119D:
	ldrbeq	r0,[r0,r5,lsr #14]!

l000011A1:
	strdvc	r2,r3,[r6],-#&8A

l000011A5:
	ldrtgt	r0,[pc],#&BD                                        ; 0000126A

l000011A9:
	msreq	cpsr,r0

;; vTaskMissedYield: 000011AD
vTaskMissedYield proc
	movtgt	r0,#&B222

l000011B1:
	Invalid

l000011B5:
	ldrtgt	r0,[pc],#&47                                        ; 00001204

l000011B9:
	eoreq	r0,r0,r0

;; vTaskPriorityInherit: 000011BD
vTaskPriorityInherit proc
	Invalid
	movtgt	r2,#&C1B5
	rsble	r6,r8,#&C0000006
	subne	r9,r2,#&B0000001
	ldrdeq	r8,r9,[fp],-#&22
	sbcsvs	r0,fp,#&2A000000
	rsbgt	sp,ip,#&80000006
	andhi	r0,r2,#&1000000F
	movteq	r1,#&DB63
	andmi	r8,r3,#&AC000003
	mvnhi	r0,#&1AC00000
	movteq	r9,#&2A03
	blle	$01A1A135
	Invalid
	ldrbths	r0,[r1],#&BD
	stmlo	r6,{r0-r2,r9-r10}
	svcls	#&F00746
	Invalid
	rschi	r0,fp,#&C0000006
	mvnhi	r0,#&3000000
	msrlo	spsr,#&B03
	msr	cpsr,#&1B9
	rscseq	r0,r10,#&C000001B
	rsceq	r2,r10,#&C8000003
	msreq	r10_usr,r2
	strbtle	r6,[r8],-#&223
	rscle	r7,r0,#&F800
	movtls	r3,#&696C
	Invalid
	rschi	r0,fp,#&30000000
	strbeq	pc,[r4,-#&200]!
	mov	r8,#&EB
	Invalid
	ldrbpl	r0,[r0,#&740]!
	strbgt	r7,[r7],-#&B8
	stcgt	p0,c0,[r0]
	eoreq	r0,r0,r0

;; xTaskPriorityDisinherit: 00001251
xTaskPriorityDisinherit proc
	Invalid
	msrgt	spsr,#&1B5
	ereteq
	movtgt	r9,#&213B
	bleq	$FF4013FD
	Invalid
	ldrbths	r0,[r1],#&BD
	stmlo	r6,{r0-r2,r10}
	ldrbvs	r0,[r0,#&746]!
	ldrsh	r7,[r9,r8]!
	Invalid
	andeq	r8,r3,#&C000003A
	blls	$000E2235
	rorseq	r4,r8,#6
	rsbeq	sp,pc,r0,lsr #6
	mvnshs	r0,#&8000003E
	smlattle	r1,r10,r1,r0
	Invalid
	msrge	cpsr,#&14A
	Invalid
	strbeq	r0,[r8,-#&BE0]
	ldmiblo	r6,{r1,r3-r9}^
	msrgt	spsr,#&346
	moveq	r0,#&72F1
	strmi	r8,[r3],-fp
	andeq	r0,r6,r10,ror #&1D
	strge	r8,[r0,-fp,ror #7]
	strbeq	sp,[r7,-r3,ror #12]!
	ldmdbhs	r8,{r4-r7,r9-r10,ip}^
	adcseq	pc,sp,r6,asr #&10
	strbgt	r7,[r7],-#&20
	stcgt	p0,c0,[r0]
	strteq	r0,[r0]

;; pvTaskIncrementMutexHeldCount: 000012D5
pvTaskIncrementMutexHeldCount proc
	bne	$01A17C09

l000012D9:
	bgt	$01A179A5

l000012DD:
	bgt	$00C81899

l000012E1:
	rsbvc	r5,r8,r5,ror #&10

l000012E5:
	ldrtgt	r0,[pc],#&47                                        ; 00001334

l000012E9:
	eoreq	r0,r0,r0

l000012ED:
	svcle	#0

;; prvRestoreContextOfFirstTask: 000012F1
prvRestoreContextOfFirstTask proc
	Invalid

l000012F5:
	rsbhi	r0,r8,r8,rrx

l000012F9:
	Invalid

l000012FD:
	stmeq	r8!,{r0-r1,r3,r6,r8,fp-ip}

l00001301:
	ldrbteq	r0,[r1],#&168

l00001305:
	rscshs	sp,r8,r1,lsl #&1E

l00001309:
	Invalid
	Invalid
	Invalid
	ldrbtne	r8,[r3],#&30F
	ldmibeq	r3,{r3,r7,pc}^
	rscseq	r4,r0,r8,lsl #&1F
	mvnsne	r8,r0
	rscseq	r6,r0,#&220
	svcge	#&47700E
	stmgt	r0,{r0-r1,r4-r7}
	nophi

;; prvSVCHandler: 00001335
prvSVCHandler proc
	rscseq	r1,r8,#&A4000001

l00001339:
	eorne	r0,fp,ip,lsr r1

l0000133D:
	sbcseq	r0,r3,#&340000

l00001341:
	svc	#&D1062B
	Invalid

l00001349:
	strdhi	r0,r1,[r1,-r0,ror #3]

l0000134D:
	Invalid

l00001351:
	strbeq	r7,[r7,-r7,asr #32]

l00001355:
	msrmi	spsr,#&34A

l00001359:
	movtne	r3,#&3EF0

l0000135D:
	svcmi	#&E7C760

l00001361:
	ldrbeq	r8,[r2],-#&F0

l00001365:
	svclt	#&601A4B

l00001369:
	svclt	#&8F4FF3

l0000136D:
	strdvc	r6,r7,[pc],r3                                       ; 00001375

l00001371:
	Invalid

l00001375:
	strbteq	r0,[r0],#&ED

l00001379:
	mvneq	r0,sp,ror #1

;; pxPortInitialiseStack: 0000137D
pxPortInitialiseStack proc
	ldmeq	r4!,{r0-r1,r3,r5,ip-sp}

l00001381:
	svcmi	#&2302BF

l00001385:
	svcmi	#&7580F0

l00001389:
	stmdane	r4,{r4-r7}

l0000138D:
	strhtmi	r0,[r3],-#&3F

l00001391:
	strdhs	r2,r3,[ip,-r8,ror #1]!

l00001395:
	strdge	r0,r1,[r1],-r0

l00001399:
	strdeq	r4,r5,[r2],-r1

l0000139D:
	andmi	r2,r0,r9,ror #5

l000013A1:
	strdmi	r0,r1,[ip],-#&C8

l000013A5:
	ldrshtlo	r4,[ip],-#&48

l000013A9:
	strhvc	r1,[r6],-#&C

l000013AD:
	Invalid

;; xPortStartScheduler: 000013B1
xPortStartScheduler proc
	bne	$FED1D4E5

l000013B5:
	submi	r4,r9,#&1A00

l000013B9:
	bne	$000A1391

l000013BD:
	rsbmi	r1,r8,#&60000

l000013C1:
	bne	$010A1389

l000013C5:
	msrlt	spsr,#&B60

l000013C9:
	stmne	pc!,{r0,r2,r4-r7}

l000013CD:
	svcne	#&F644D0

l000013D1:
	eoreq	r0,r1,r5,ror #&E

l000013D5:
	strbmi	r4,[ip],-r0

l000013D9:
	strbhs	r4,[fp,-#&74A]

l000013DD:
	stmne	r0!,{r5-r6,r8,ip}

l000013E1:
	subeq	sp,r8,r0,rrx

l000013E5:
	rsbhi	r0,r8,r8,rrx

l000013E9:
	addvs	r0,r8,#&F30000

l000013ED:
	svclt	#&B661B6

l000013F1:
	svclt	#&8F4FF3

l000013F5:
	strdeq	r6,r7,[pc],r3                                       ; 000013FD

l000013F9:
	ldrsbtvc	r0,[pc],#&F                                       ; 00001410

l000013FD:
	Invalid

l00001401:
	svclo	#&493F48

l00001405:
	andsmi	r0,r10,fp,asr #&12

l00001409:
	Invalid

l0000140D:
	strbvs	r1,[r0,-#&A29]!

l00001411:
	streq	r4,[r3,-#&D9]!

l00001415:
	mvneq	r0,r2,lsr #4

l00001419:
	Invalid

l0000141D:
	svcmi	#&4299D0

l00001421:
	Invalid
	movtmi	r3,#&B8D8
	strlo	r4,[r2,-r10,ror #5]
	stmeq	ip,{r0,r3,r6,fp-sp}^
	submi	r3,fp,r10,lsl r4
	Invalid
	stmne	r0!,{r0,r3,r5,r9,sp}
	sbcsmi	r4,r9,r0,ror #&18
	eoreq	r0,r2,#&8C00000
	svcne	#&3201E0
	ldmibls	r0,{r1,r3,r5,r9,lr}^
	mvnmi	r4,#&108
	svchs	#&D8F803
	rscmi	r4,r10,#&2C000001
	svchs	#&4B2F02
	stmhs	sp,{r0,r3,r6,r10-fp,sp}
	tstmi	r10,#&120000
	Invalid
	strbteq	r2,[r0],-#&A29
	sbcsmi	r3,r9,r0,ror #&10
	eoreq	r0,r2,#&8C00000
	svcne	#&3201E0
	ldmibls	r0,{r1,r3,r5,r10-fp,sp}^
	mvnmi	r4,#&108
	ldrbhs	pc,[r8],r3
	rscmi	r4,r10,#&48
	eormi	r0,r3,r0,lsl #&A
	mcrrne	p0,#2,r2,lr,c2
	strbhs	r2,[sp],-#&44C
	strbhs	r3,[r0,-#&49]!
	svcne	#&330160
	rscmi	r4,r10,#&AC
	bhi	$FF405CB1
	sbcshs	pc,r9,r2,asr #&10
	mvnmi	r4,#&A0000004
	svcne	#&4A1803
	bleq	$018061DD
	rscshi	r4,r4,r8,ror #6
	rsbpl	r0,r0,#&CC00
	teqmi	ip,#&F800
	andmi	r0,r3,#&3C000000
	ldrshhi	r0,[ip,-r8]!
	svc	#&4B1AE7
	Invalid
	smlalttgt	r1,r10,r7,r10
	strble	r1,[r8,-r7,ror #21]
	Invalid
	strbge	r1,[r10],-#&AE7
	smlalttle	r1,r8,r7,r10
	adcshs	r0,pc,r7,ror #1
	rscls	r0,r0,sp,ror #1
	strbtne	r0,[r0],#&ED
	rscne	r0,r0,r0,ror #1
	Invalid
	eoreq	r0,r0,r0
	andeq	r0,r0,r0
	Invalid
	mvneq	r0,sp,ror #1
	andeq	r0,r6,r0,lsl #&E
	andge	r0,r0,r0,lsl #1
	mvneq	r0,sp,ror #1
	andeq	r0,r5,r0,lsl #&E
	eoreq	r0,r0,r0
	msreq	cpsr,r2
	movne	r0,#&1700
	Invalid
	ldrsheq	pc,[pc,-pc]                                        ; 00001531
	ldrhs	r0,[r3]
	svclo	#&E000ED
	svclo	#&130000
	svclo	#&60700
	svclo	#&50700
	stmdbeq	r1,{r8-r10}
	stmdbeq	r5,{r8-r10}
	stmdbeq	r6,{r8-r10}
	andvc	r0,r1,r0,lsl #&E

;; vPortEndScheduler: 00001551
vPortEndScheduler proc
	adcslo	r0,pc,r7,asr #&20

;; vPortStoreTaskMPUSettings: 00001555
vPortStoreTaskMPUSettings proc
	strhmi	r0,[r9,-r4]!

l00001559:
	ldreq	r4,[fp,#&BD0]!

l0000155D:
	Invalid
	strbmi	r0,[r8,-#&BB1]!
	movne	r1,#&20F0
	msrhi	cpsr,#&43
	sbcsmi	r6,r9,r0,ror #&1E
	eoreq	r0,r3,#&8800000
	svcne	#&3301E0
	ldrbls	r1,[r0],#&72B
	rscmi	r4,r10,#&108
	blpl	$FF63F591
	rsbmi	r8,r8,#0
	movne	r0,#&21F0
	Invalid
	blxeq	r5
	strdeq	r0,r1,[r1],-r1
	svcle	#&8F1
	ldrsbtvc	r3,[ip],#&1
	rscsne	r4,r0,r7,asr #&A
	msrhi	spsr,#&403
	Invalid
	blls	$FF9FBE41
	ldrbtne	r4,[r0],#&200
	eoreq	r2,fp,#2
	sbcsmi	r4,r9,r0,ror #&16
	eoreq	r0,r4,#&8800000
	svcne	#&3401E0
	bicsls	r0,r0,#&2C0000
	rscmi	r4,r10,#&108
	bicshs	pc,r8,#&20000
	strbtmi	r4,[r10],#&34B
	rsbgt	r4,r0,r4,lsl #8
	blx	$0130A17F
	subhs	r2,fp,#&7000000E
	ldrbtne	r4,[r0],#&349
	andshs	ip,r10,r2,lsl #&12
	strbtlo	r0,[r0],-r9
	streq	r4,[r3,-#&D9]!
	mvneq	r0,r2,lsr #4
	qasxhs	r1,r10,r2
	svcmi	#&428BD0
	Invalid
	movtmi	r1,#&B7D3
	stmdbne	r2,{r1,r3,r5-r7,r9,lr}
	movtmi	r1,#&9A4B
	stmdbgt	r4,{r4-r8,r10,ip}
	eormi	r2,r9,#&1A
	Invalid
	Invalid
	mvneq	r0,r3,lsr #4
	eorne	r1,r10,#&C8
	svcmi	#&4299D0
	Invalid
	movtmi	r1,#&B2D8
	strne	r4,[r2],-r10
	strne	r0,[r3,-r4,lsr #32]!
	rsbgt	r0,r1,#&21000000
	msrgt	spsr,#&360
	rsblo	r8,r1,r1,ror #2
	Invalid
	bleq	$FF9F8B79
	stmdbeq	r7,{r1,r3,r6,r8,ip-pc}^
	beq	$FF9E76E5
	beq	$FF9F0F8D
	stmdbeq	r7,{r1,r3,r6,r8-r9,fp,sp-pc}^
	mvneq	sp,r10,asr #6
	svclo	#&30700
	andeq	r0,r3,r0,lsl #&E
	eoreq	r0,r0,r0
	eoreq	r0,r0,r0,lsr #&20
	eoreq	r0,r0,r0
	msreq	cpsr,r2
	svclo	#&10700
	stmdbeq	r1,{r8-r10}
	stmdbeq	r3,{r8-r10}
	svc	#&10700

;; xPortPendSVHandler: 00001689
xPortPendSVHandler proc
	strne	r0,[r0],#&9F3

l0000168D:
	svc	#&681A4B
	Invalid

l00001695:
	andne	pc,pc,r9,ror #5

l00001699:
	stmdbeq	r9,{r5-r6,r8,r10-fp,sp}^

l0000169D:
	svclt	#&F04F40

l000016A1:
	mvnsne	r8,r0

l000016A5:
	ldrbne	pc,[r7,r8,lsl #31]!

l000016A9:
	ldrshteq	r4,[r0],#&FD

l000016AD:
	mvnsne	r8,r0

l000016B1:
	stmdbeq	r8,{r3,r7-r8,r10-sp,pc}^

l000016B5:
	stmeq	r8!,{r6,r8,fp-ip}

l000016B9:
	ldrbteq	r0,[r1],#&168

l000016BD:
	mrslt	r10_usr,r1

l000016C1:
	andge	pc,pc,#&E8

l000016C5:
	andlt	pc,pc,r8,ror #1

l000016C9:
	movhi	pc,#&F8E8

l000016CD:
	Invalid

l000016D1:
	strdvc	r0,r1,[r8],r3

l000016D5:
	svcge	#&BF0047

l000016D9:
	svcge	#&8000F3

l000016DD:
	stmgt	r0,{r0-r1,r4-r7}

l000016E1:
	eorne	r0,r0,r0

;; xPortSysTickHandler: 000016E5
xPortSysTickHandler proc
	ldrhne	lr,[r3,#&F5]!

l000016E9:
	svclt	#&F04F84

l000016ED:
	mvnsne	r8,r3,lsl #6

l000016F1:
	svcvs	#&F3BF88

l000016F5:
	svcmi	#&F3BF8F

l000016F9:
	ldrblo	pc,[r7,pc,lsl #31]!

l000016FD:
	svcmi	#&B118FB

l00001701:
	subseq	r8,r2,#&F0

l00001705:
	strbthi	r1,[r0],-#&A4B

l00001709:
	Invalid

l0000170D:
	ldrteq	r0,[pc],#&BD                                        ; 000017D2

l00001711:
	Invalid

;; vPortSVCHandler: 00001715
vPortSVCHandler proc
	Invalid

l00001719:
	ldmdbeq	r3,{r0-r5,r7-fp,sp-pc}^

l0000171D:
	ldmibeq	r3,{r7-fp,sp-pc}^

l00001721:
	stmdbeq	r6,{r7-r10}^

l00001725:
	Invalid

l00001729:
	rscne	r0,r0,sp,ror #1

;; pvPortMalloc: 0000172D
pvPortMalloc proc
	movtmi	r0,#&64B5

l00001731:
	adcshs	r1,pc,r7,lsl #&18

l00001735:
	stmdaeq	r4,{r4-r10}

l00001739:
	Invalid

l0000173D:
	bne	$012C5729

l00001741:
	adcsmi	r10,r1,r8,ror #&14

l00001745:
	cmple	r1,#&C8000003

l00001749:
	strtne	ip,[r5],-#&F8

l0000174D:
	stmeq	r2,{r2,r6,r10-fp,pc}^

l00001751:
	Invalid

l00001755:
	msrgt	spsr,#&9D2

l00001759:
	mcrrhi	p0,#&F,ip,r5,c8

l0000175D:
	ldrbhi	pc,[r7,#&F18]!

l00001761:
	strdne	r2,r3,[r6],-#&B

l00001765:
	svc	#&2400BD
	ldrshths	r8,[fp],#&7

l0000176D:
	movseq	r1,#&46

l00001771:
	andhs	r0,r2,#&F100

l00001775:
	bne	$0008373D

l00001779:
	rsclo	lr,r7,r0,ror #6

l0000177D:
	eorvc	r0,r0,r2

;; vPortFree: 00001781
vPortFree proc
	adcseq	r0,pc,r7,asr #&20

;; vPortInitialiseBlocks: 00001785
vPortInitialiseBlocks proc
	movtgt	r0,#&B222

l00001789:
	Invalid

l0000178D:
	adcslo	r0,pc,r7,asr #&20

l00001791:
	Invalid

;; xPortGetFreeHeapSize: 00001795
xPortGetFreeHeapSize proc
	rscsgt	sp,r8,fp,asr #6

l00001799:
	strbtlt	ip,[r5],r5

l0000179D:
	eorsvc	r0,r0,r0,ror #8

l000017A1:
	adcslo	r0,pc,r7,asr #&20

l000017A5:
	eorne	r0,r0,r2

;; xEventGroupCreate: 000017A9
xEventGroupCreate proc
	svc	#&2018B5
	ldrbteq	fp,[pc],#&EF7                                      ; 000026AC

l000017B1:
	adcseq	r2,r1,r6,asr #&20

l000017B5:
	ldrbteq	r4,[r8],#&23

l000017B9:
	ldmibhi	r0,{r0-r1,r3-r5,r9-r10}^

l000017BD:
	strdne	r2,r3,[r6],-#&D

l000017C1:
	Invalid

;; xEventGroupWaitBits: 000017C5
xEventGroupWaitBits proc
	strbeq	pc,[r1],-r9

l000017C9:
	stceq	p15,c1,[r6,-#&118]

l000017CD:
	svc	#&469046
	ldrbtlo	r1,[r9],#&CF7

l000017D5:
	Invalid

l000017D9:
	ldmdblt	r0,{r1,r6,r8,r10-fp}^

l000017DD:
	andeq	r0,pc,#&F1

l000017E1:
	strbeq	r2,[r10,#&4D0]!

l000017E5:
	svc	#&603505
	ldrshths	r4,[fp],#&7

l000017ED:
	Invalid
	strbteq	r3,[r10],#&581
	ldrbeq	pc,[r0],r3
	Invalid
	ldrsbteq	fp,[r1],#&80
	adcseq	r0,pc,pc,lsl #&18
	rscshi	r4,r0,r1,lsr #&1E
	ldmibhs	r9!,{r0,r4-r6,r8-r10,lr-pc}
	addslo	r0,r10,r3,asr #&C
	Invalid
	ldmibhs	r7,{r0-r1,r3-pc}^
	svcmi	#&B938FB
	cmpne	r2,#&F0
	svclt	#&601A4B
	svclt	#&8F4FF3
	svc	#&8F6FF3
	mvnshi	r7,#&F70000
	strbeq	r0,[r6],-r1
	svcvc	#&F024D5
	Invalid
	rscshi	r4,r0,r1,lsl #3
	strbteq	lr,[r7],r1
	ldrbtlo	r9,[lr],#&8F0
	ldrhs	r6,[r9,#&F68]!
	ldmdblt	r0,{r1,r6,r8,r10}^
	andeq	r0,pc,#&F1
	strbeq	r2,[r10,#&4D0]!
	strbteq	r3,[r0],-r5
	ldrbths	r10,[lr],#&8F0
	stclt	p15,c7,[r0,-#&3C0]
	strlo	pc,[r1,#&E8]
	Invalid
	strbteq	lr,[r7],#&FD1
	rscvc	r0,r0,sp,ror #1

;; xEventGroupClearBits: 00001875
xEventGroupClearBits proc
	mcrreq	p6,#&B,r0,r6,c5

l00001879:
	Invalid

l0000187D:
	strbhs	r3,[r8,-#&5FE]!

l00001881:
	strlo	r0,[r4],-#&4EA

l00001885:
	mvnsls	r0,#&6000000

l00001889:
	strdvc	r2,r3,[r6],-#&8E

l0000188D:
	Invalid

;; xEventGroupSetBits: 00001891
xEventGroupSetBits proc
	mcrreq	p5,#&B,r0,r6,c5

l00001895:
	ldmiblt	r7,{r1-r2,r6,r8-pc}^

l00001899:
	stmhs	r8!,{r3-r8,fp,sp}

l0000189D:
	Invalid

l000018A1:
	strbhi	r2,[r3],-r6

l000018A5:
	rsbhs	r2,r0,#&108000

l000018A9:
	stceq	p0,c0,[r7],-#&340

l000018AD:
	strbeq	r0,[r2,-r0,ror #21]

l000018B1:
	Invalid

l000018B5:
	Invalid

l000018B9:
	svc	#&7100F0
	ldmibhs	fp,{r0-r2,r4-r7,sp-pc}^

l000018C1:
	subhs	r10,r2,r8,ror #&C

l000018C5:
	sbcsls	r0,r0,r6,asr #&18

l000018C9:
	movne	r1,#&8E8

l000018CD:
	msrhs	spsr,#&F0

l000018D1:
	bl	$010A1899
	Invalid

l000018D9:
	ldrbge	lr,[r0],lr

l000018DD:
	Invalid
	stmlo	r3,{r0,r4,r6-pc}^
	svc	#&602940
	ldmdbhs	r10,{r0-r2,r4-r7,lr-pc}^
	svcmi	#&BDF868
	Invalid
	Invalid

;; xEventGroupSync: 000018F9
xEventGroupSync proc
	stmhi	r1,{r0,r3,r5-r7,ip-pc}

l000018FD:
	strbne	r0,[r6],-r6

l00001901:
	svc	#&461F46
	ldrshmi	r8,[r8,#&27]!

l00001909:
	stmhs	r8!,{r1-r2,r6,r10-fp,sp}

l0000190D:
	svc	#&430C46
	Invalid

l00001915:
	smlatths	r3,r10,r4,r0

l00001919:
	Invalid

l0000191D:
	ldrbge	pc,[r7,#&F68]!

l00001921:
	stclt	p0,c2,[r6,-#&3E8]

l00001925:
	blo	$FE07DCCD

l00001929:
	rscsge	r4,r0,r6,asr #&C

l0000192D:
	svc	#&1D2861
	svc	#&FB62F7
	ldmdblo	r10,{r0-r2,r4-r7,r9,fp-ip,pc}^

l00001939:
	ldrhthi	r4,[r0],#&F9

l0000193D:
	bne	$012C5E8D

l00001941:
	svcmi	#&F3BF60

l00001945:
	svcvs	#&F3BF8F

l00001949:
	ldmib	r7,{r0-r3,r7-pc}^
0000194D                                        FB 83 01              ...
00001950 04 46 09 D5 24 F0 7F 44 20 46 BD E8 F0 81 2B 68 .F..$..D F....+h
00001960 23 EA 06 06 2E 60 DA E7 06 F0 06 FE 2C 68 36 EA #....`......,h6.
00001970 04 03 04 BF 24 EA 06 06 2E 60 06 F0 19 FE 24 F0 ....$....`....$.
00001980 7F 44 E9 E7 04 ED 00 E0 EF                      .D.......      

;; xEventGroupGetBitsFromISR: 00001989
xEventGroupGetBitsFromISR proc
	svcmi	#&8311F3

l0000198D:
	andhi	fp,r2,#&3C0

l00001991:
	svclt	#&8811F3

l00001995:
	svclt	#&8F6FF3

l00001999:
	orrhi	r4,pc,#&3CC

l0000199D:
	Invalid

l000019A1:
	subne	r7,r7,r8,rrx

;; vEventGroupDelete: 000019A5
vEventGroupDelete proc
	svc	#&4604B5
	mvnsvs	r3,#&F7

l000019AD:
	svcmi	#&B13B68

l000019B1:
	ldrshths	r0,[r1]

l000019B5:
	mvnsvs	pc,#&1A4

l000019B9:
	Invalid

l000019BD:
	sbcshs	pc,r1,fp,lsr #&E

l000019C1:
	Invalid

l000019C5:
	Invalid

l000019C9:
	svcmi	#&F7FF40

l000019CD:
	svc	#&BF00BA

;; vEventGroupSetBitsCallback: 000019D1
vEventGroupSetBitsCallback proc
	ldmlo	pc!,{r0-r2,r4-r7,r9-ip,lr}

;; vEventGroupClearBitsCallback: 000019D5
vEventGroupClearBitsCallback proc
	stceq	p4,c0,[r6,-#&2D4]

l000019D9:
	Invalid

l000019DD:
	msrhs	spsr,#&3FD

l000019E1:
	movhs	r0,#&35EA

l000019E5:
	stmdblo	r8,{r5-r6,r8,r10-sp,pc}^

l000019E9:
	mvns	r0,r0,asr #&C
	ldrhteq	r0,[pc],#&D                                        ; 00001A02

l000019F1:
	andeq	r0,r0,r0

l000019F5:
	andeq	r0,r0,r0

l000019F9:
	andeq	r0,r0,r0

l000019FD:
	andeq	r0,r0,r0

l00001A01:
	andeq	r0,r0,r0

l00001A05:
	andeq	r0,r0,r0

l00001A09:
	andeq	r0,r0,r0

l00001A0D:
	andeq	r0,r0,r0

l00001A11:
	andeq	r0,r0,r0

l00001A15:
	andeq	r0,r0,r0

l00001A19:
	andeq	r0,r0,r0

l00001A1D:
	andeq	r0,r0,r0

l00001A21:
	andeq	r0,r0,r0

l00001A25:
	andeq	r0,r0,r0

l00001A29:
	andeq	r0,r0,r0

l00001A2D:
	andeq	r0,r0,r0

l00001A31:
	andeq	r0,r0,r0

l00001A35:
	andeq	r0,r0,r0

l00001A39:
	andeq	r0,r0,r0

l00001A3D:
	andeq	r0,r0,r0

l00001A41:
	andeq	r0,r0,r0

l00001A45:
	andeq	r0,r0,r0

l00001A49:
	andeq	r0,r0,r0

l00001A4D:
	andeq	r0,r0,r0

l00001A51:
	andeq	r0,r0,r0

l00001A55:
	andeq	r0,r0,r0

l00001A59:
	andeq	r0,r0,r0

l00001A5D:
	andeq	r0,r0,r0

l00001A61:
	andeq	r0,r0,r0

l00001A65:
	andeq	r0,r0,r0

l00001A69:
	andeq	r0,r0,r0

l00001A6D:
	andeq	r0,r0,r0

l00001A71:
	andeq	r0,r0,r0

l00001A75:
	andeq	r0,r0,r0

l00001A79:
	andeq	r0,r0,r0

l00001A7D:
	andeq	r0,r0,r0

l00001A81:
	andeq	r0,r0,r0

l00001A85:
	andeq	r0,r0,r0

l00001A89:
	andeq	r0,r0,r0

l00001A8D:
	andeq	r0,r0,r0

l00001A91:
	andeq	r0,r0,r0

l00001A95:
	andeq	r0,r0,r0

l00001A99:
	andeq	r0,r0,r0

l00001A9D:
	andeq	r0,r0,r0

l00001AA1:
	andeq	r0,r0,r0

l00001AA5:
	andeq	r0,r0,r0

l00001AA9:
	andeq	r0,r0,r0

l00001AAD:
	andeq	r0,r0,r0

l00001AB1:
	andeq	r0,r0,r0

l00001AB5:
	andeq	r0,r0,r0

l00001AB9:
	andeq	r0,r0,r0

l00001ABD:
	andeq	r0,r0,r0

l00001AC1:
	andeq	r0,r0,r0

l00001AC5:
	andeq	r0,r0,r0

l00001AC9:
	andeq	r0,r0,r0

l00001ACD:
	andeq	r0,r0,r0

l00001AD1:
	andeq	r0,r0,r0

l00001AD5:
	andeq	r0,r0,r0

l00001AD9:
	andeq	r0,r0,r0

l00001ADD:
	andeq	r0,r0,r0

l00001AE1:
	andeq	r0,r0,r0

l00001AE5:
	andeq	r0,r0,r0

l00001AE9:
	andeq	r0,r0,r0

l00001AED:
	andeq	r0,r0,r0

l00001AF1:
	andeq	r0,r0,r0

l00001AF5:
	andeq	r0,r0,r0

l00001AF9:
	andeq	r0,r0,r0

l00001AFD:
	andeq	r0,r0,r0

l00001B01:
	andeq	r0,r0,r0

l00001B05:
	andeq	r0,r0,r0

l00001B09:
	andeq	r0,r0,r0

l00001B0D:
	andeq	r0,r0,r0

l00001B11:
	andeq	r0,r0,r0

l00001B15:
	andeq	r0,r0,r0

l00001B19:
	andeq	r0,r0,r0

l00001B1D:
	andeq	r0,r0,r0

l00001B21:
	andeq	r0,r0,r0

l00001B25:
	andeq	r0,r0,r0

l00001B29:
	andeq	r0,r0,r0

l00001B2D:
	andeq	r0,r0,r0

l00001B31:
	andeq	r0,r0,r0

l00001B35:
	andeq	r0,r0,r0

l00001B39:
	andeq	r0,r0,r0

l00001B3D:
	andeq	r0,r0,r0

l00001B41:
	andeq	r0,r0,r0

l00001B45:
	andeq	r0,r0,r0

l00001B49:
	andeq	r0,r0,r0

l00001B4D:
	andeq	r0,r0,r0

l00001B51:
	andeq	r0,r0,r0

l00001B55:
	andeq	r0,r0,r0

l00001B59:
	andeq	r0,r0,r0

l00001B5D:
	andeq	r0,r0,r0

l00001B61:
	andeq	r0,r0,r0

l00001B65:
	andeq	r0,r0,r0

l00001B69:
	andeq	r0,r0,r0

l00001B6D:
	andeq	r0,r0,r0

l00001B71:
	andeq	r0,r0,r0

l00001B75:
	andeq	r0,r0,r0

l00001B79:
	andeq	r0,r0,r0

l00001B7D:
	andeq	r0,r0,r0

l00001B81:
	andeq	r0,r0,r0

l00001B85:
	andeq	r0,r0,r0

l00001B89:
	andeq	r0,r0,r0

l00001B8D:
	andeq	r0,r0,r0

l00001B91:
	andeq	r0,r0,r0

l00001B95:
	andeq	r0,r0,r0

l00001B99:
	andeq	r0,r0,r0

l00001B9D:
	andeq	r0,r0,r0

l00001BA1:
	andeq	r0,r0,r0

l00001BA5:
	andeq	r0,r0,r0

l00001BA9:
	andeq	r0,r0,r0

l00001BAD:
	andeq	r0,r0,r0

l00001BB1:
	andeq	r0,r0,r0

l00001BB5:
	andeq	r0,r0,r0

l00001BB9:
	andeq	r0,r0,r0

l00001BBD:
	andeq	r0,r0,r0

l00001BC1:
	andeq	r0,r0,r0

l00001BC5:
	andeq	r0,r0,r0

l00001BC9:
	andeq	r0,r0,r0

l00001BCD:
	andeq	r0,r0,r0

l00001BD1:
	andeq	r0,r0,r0

l00001BD5:
	andeq	r0,r0,r0

l00001BD9:
	andeq	r0,r0,r0

l00001BDD:
	andeq	r0,r0,r0

l00001BE1:
	andeq	r0,r0,r0

l00001BE5:
	andeq	r0,r0,r0

l00001BE9:
	andeq	r0,r0,r0

l00001BED:
	andeq	r0,r0,r0

l00001BF1:
	andeq	r0,r0,r0

l00001BF5:
	andeq	r0,r0,r0

l00001BF9:
	andeq	r0,r0,r0

l00001BFD:
	andeq	r0,r0,r0

l00001C01:
	andeq	r0,r0,r0

l00001C05:
	andeq	r0,r0,r0

l00001C09:
	andeq	r0,r0,r0

l00001C0D:
	andeq	r0,r0,r0

l00001C11:
	andeq	r0,r0,r0

l00001C15:
	andeq	r0,r0,r0

l00001C19:
	andeq	r0,r0,r0

l00001C1D:
	andeq	r0,r0,r0

l00001C21:
	andeq	r0,r0,r0

l00001C25:
	andeq	r0,r0,r0

l00001C29:
	andeq	r0,r0,r0

l00001C2D:
	andeq	r0,r0,r0

l00001C31:
	andeq	r0,r0,r0

l00001C35:
	andeq	r0,r0,r0

l00001C39:
	andeq	r0,r0,r0

l00001C3D:
	andeq	r0,r0,r0

l00001C41:
	andeq	r0,r0,r0

l00001C45:
	andeq	r0,r0,r0

l00001C49:
	andeq	r0,r0,r0

l00001C4D:
	andeq	r0,r0,r0

l00001C51:
	andeq	r0,r0,r0

l00001C55:
	andeq	r0,r0,r0

l00001C59:
	andeq	r0,r0,r0

l00001C5D:
	andeq	r0,r0,r0

l00001C61:
	andeq	r0,r0,r0

l00001C65:
	andeq	r0,r0,r0

l00001C69:
	andeq	r0,r0,r0

l00001C6D:
	andeq	r0,r0,r0

l00001C71:
	andeq	r0,r0,r0

l00001C75:
	andeq	r0,r0,r0

l00001C79:
	andeq	r0,r0,r0

l00001C7D:
	andeq	r0,r0,r0

l00001C81:
	andeq	r0,r0,r0

l00001C85:
	andeq	r0,r0,r0

l00001C89:
	andeq	r0,r0,r0

l00001C8D:
	andeq	r0,r0,r0

l00001C91:
	andeq	r0,r0,r0

l00001C95:
	andeq	r0,r0,r0

l00001C99:
	andeq	r0,r0,r0

l00001C9D:
	andeq	r0,r0,r0

l00001CA1:
	andeq	r0,r0,r0

l00001CA5:
	andeq	r0,r0,r0

l00001CA9:
	andeq	r0,r0,r0

l00001CAD:
	andeq	r0,r0,r0

l00001CB1:
	andeq	r0,r0,r0

l00001CB5:
	andeq	r0,r0,r0

l00001CB9:
	andeq	r0,r0,r0

l00001CBD:
	andeq	r0,r0,r0

l00001CC1:
	andeq	r0,r0,r0

l00001CC5:
	andeq	r0,r0,r0

l00001CC9:
	andeq	r0,r0,r0

l00001CCD:
	andeq	r0,r0,r0

l00001CD1:
	andeq	r0,r0,r0

l00001CD5:
	andeq	r0,r0,r0

l00001CD9:
	andeq	r0,r0,r0

l00001CDD:
	andeq	r0,r0,r0

l00001CE1:
	andeq	r0,r0,r0

l00001CE5:
	andeq	r0,r0,r0

l00001CE9:
	andeq	r0,r0,r0

l00001CED:
	andeq	r0,r0,r0

l00001CF1:
	andeq	r0,r0,r0

l00001CF5:
	andeq	r0,r0,r0

l00001CF9:
	andeq	r0,r0,r0

l00001CFD:
	andeq	r0,r0,r0

l00001D01:
	andeq	r0,r0,r0

l00001D05:
	andeq	r0,r0,r0

l00001D09:
	andeq	r0,r0,r0

l00001D0D:
	andeq	r0,r0,r0

l00001D11:
	andeq	r0,r0,r0

l00001D15:
	andeq	r0,r0,r0

l00001D19:
	andeq	r0,r0,r0

l00001D1D:
	andeq	r0,r0,r0

l00001D21:
	andeq	r0,r0,r0

l00001D25:
	andeq	r0,r0,r0

l00001D29:
	andeq	r0,r0,r0

l00001D2D:
	andeq	r0,r0,r0

l00001D31:
	andeq	r0,r0,r0

l00001D35:
	andeq	r0,r0,r0

l00001D39:
	andeq	r0,r0,r0

l00001D3D:
	andeq	r0,r0,r0

l00001D41:
	andeq	r0,r0,r0

l00001D45:
	andeq	r0,r0,r0

l00001D49:
	andeq	r0,r0,r0

l00001D4D:
	andeq	r0,r0,r0

l00001D51:
	andeq	r0,r0,r0

l00001D55:
	andeq	r0,r0,r0

l00001D59:
	andeq	r0,r0,r0

l00001D5D:
	andeq	r0,r0,r0

l00001D61:
	andeq	r0,r0,r0

l00001D65:
	andeq	r0,r0,r0

l00001D69:
	andeq	r0,r0,r0

l00001D6D:
	andeq	r0,r0,r0

l00001D71:
	andeq	r0,r0,r0

l00001D75:
	andeq	r0,r0,r0

l00001D79:
	andeq	r0,r0,r0

l00001D7D:
	andeq	r0,r0,r0

l00001D81:
	andeq	r0,r0,r0

l00001D85:
	andeq	r0,r0,r0

l00001D89:
	andeq	r0,r0,r0

l00001D8D:
	andeq	r0,r0,r0

l00001D91:
	andeq	r0,r0,r0

l00001D95:
	andeq	r0,r0,r0

l00001D99:
	andeq	r0,r0,r0

l00001D9D:
	andeq	r0,r0,r0

l00001DA1:
	andeq	r0,r0,r0

l00001DA5:
	andeq	r0,r0,r0

l00001DA9:
	andeq	r0,r0,r0

l00001DAD:
	andeq	r0,r0,r0

l00001DB1:
	andeq	r0,r0,r0

l00001DB5:
	andeq	r0,r0,r0

l00001DB9:
	andeq	r0,r0,r0

l00001DBD:
	andeq	r0,r0,r0

l00001DC1:
	andeq	r0,r0,r0

l00001DC5:
	andeq	r0,r0,r0

l00001DC9:
	andeq	r0,r0,r0

l00001DCD:
	andeq	r0,r0,r0

l00001DD1:
	andeq	r0,r0,r0

l00001DD5:
	andeq	r0,r0,r0

l00001DD9:
	andeq	r0,r0,r0

l00001DDD:
	andeq	r0,r0,r0

l00001DE1:
	andeq	r0,r0,r0

l00001DE5:
	andeq	r0,r0,r0

l00001DE9:
	andeq	r0,r0,r0

l00001DED:
	andeq	r0,r0,r0

l00001DF1:
	andeq	r0,r0,r0

l00001DF5:
	andeq	r0,r0,r0

l00001DF9:
	andeq	r0,r0,r0

l00001DFD:
	andeq	r0,r0,r0

l00001E01:
	andeq	r0,r0,r0

l00001E05:
	andeq	r0,r0,r0

l00001E09:
	andeq	r0,r0,r0

l00001E0D:
	andeq	r0,r0,r0

l00001E11:
	andeq	r0,r0,r0

l00001E15:
	andeq	r0,r0,r0

l00001E19:
	andeq	r0,r0,r0

l00001E1D:
	andeq	r0,r0,r0

l00001E21:
	andeq	r0,r0,r0

l00001E25:
	andeq	r0,r0,r0

l00001E29:
	andeq	r0,r0,r0

l00001E2D:
	andeq	r0,r0,r0

l00001E31:
	andeq	r0,r0,r0

l00001E35:
	andeq	r0,r0,r0

l00001E39:
	andeq	r0,r0,r0

l00001E3D:
	andeq	r0,r0,r0

l00001E41:
	andeq	r0,r0,r0

l00001E45:
	andeq	r0,r0,r0

l00001E49:
	andeq	r0,r0,r0

l00001E4D:
	andeq	r0,r0,r0

l00001E51:
	andeq	r0,r0,r0

l00001E55:
	andeq	r0,r0,r0

l00001E59:
	andeq	r0,r0,r0

l00001E5D:
	andeq	r0,r0,r0

l00001E61:
	andeq	r0,r0,r0

l00001E65:
	andeq	r0,r0,r0

l00001E69:
	andeq	r0,r0,r0

l00001E6D:
	andeq	r0,r0,r0

l00001E71:
	andeq	r0,r0,r0

l00001E75:
	andeq	r0,r0,r0

l00001E79:
	andeq	r0,r0,r0

l00001E7D:
	andeq	r0,r0,r0

l00001E81:
	andeq	r0,r0,r0

l00001E85:
	andeq	r0,r0,r0

l00001E89:
	andeq	r0,r0,r0

l00001E8D:
	andeq	r0,r0,r0

l00001E91:
	andeq	r0,r0,r0

l00001E95:
	andeq	r0,r0,r0

l00001E99:
	andeq	r0,r0,r0

l00001E9D:
	andeq	r0,r0,r0

l00001EA1:
	andeq	r0,r0,r0

l00001EA5:
	andeq	r0,r0,r0

l00001EA9:
	andeq	r0,r0,r0

l00001EAD:
	andeq	r0,r0,r0

l00001EB1:
	andeq	r0,r0,r0

l00001EB5:
	andeq	r0,r0,r0

l00001EB9:
	andeq	r0,r0,r0

l00001EBD:
	andeq	r0,r0,r0

l00001EC1:
	andeq	r0,r0,r0

l00001EC5:
	andeq	r0,r0,r0

l00001EC9:
	andeq	r0,r0,r0

l00001ECD:
	andeq	r0,r0,r0

l00001ED1:
	andeq	r0,r0,r0

l00001ED5:
	andeq	r0,r0,r0

l00001ED9:
	andeq	r0,r0,r0

l00001EDD:
	andeq	r0,r0,r0

l00001EE1:
	andeq	r0,r0,r0

l00001EE5:
	andeq	r0,r0,r0

l00001EE9:
	andeq	r0,r0,r0

l00001EED:
	andeq	r0,r0,r0

l00001EF1:
	andeq	r0,r0,r0

l00001EF5:
	andeq	r0,r0,r0

l00001EF9:
	andeq	r0,r0,r0

l00001EFD:
	andeq	r0,r0,r0

l00001F01:
	andeq	r0,r0,r0

l00001F05:
	andeq	r0,r0,r0

l00001F09:
	andeq	r0,r0,r0

l00001F0D:
	andeq	r0,r0,r0

l00001F11:
	andeq	r0,r0,r0

l00001F15:
	andeq	r0,r0,r0

l00001F19:
	andeq	r0,r0,r0

l00001F1D:
	andeq	r0,r0,r0

l00001F21:
	andeq	r0,r0,r0

l00001F25:
	andeq	r0,r0,r0

l00001F29:
	andeq	r0,r0,r0

l00001F2D:
	andeq	r0,r0,r0

l00001F31:
	andeq	r0,r0,r0

l00001F35:
	andeq	r0,r0,r0

l00001F39:
	andeq	r0,r0,r0

l00001F3D:
	andeq	r0,r0,r0

l00001F41:
	andeq	r0,r0,r0

l00001F45:
	andeq	r0,r0,r0

l00001F49:
	andeq	r0,r0,r0

l00001F4D:
	andeq	r0,r0,r0

l00001F51:
	andeq	r0,r0,r0

l00001F55:
	andeq	r0,r0,r0

l00001F59:
	andeq	r0,r0,r0

l00001F5D:
	andeq	r0,r0,r0

l00001F61:
	andeq	r0,r0,r0

l00001F65:
	andeq	r0,r0,r0

l00001F69:
	andeq	r0,r0,r0

l00001F6D:
	andeq	r0,r0,r0

l00001F71:
	andeq	r0,r0,r0

l00001F75:
	andeq	r0,r0,r0

l00001F79:
	andeq	r0,r0,r0

l00001F7D:
	andeq	r0,r0,r0

l00001F81:
	andeq	r0,r0,r0

l00001F85:
	andeq	r0,r0,r0

l00001F89:
	andeq	r0,r0,r0

l00001F8D:
	andeq	r0,r0,r0

l00001F91:
	andeq	r0,r0,r0

l00001F95:
	andeq	r0,r0,r0

l00001F99:
	andeq	r0,r0,r0

l00001F9D:
	andeq	r0,r0,r0

l00001FA1:
	andeq	r0,r0,r0

l00001FA5:
	andeq	r0,r0,r0

l00001FA9:
	andeq	r0,r0,r0

l00001FAD:
	andeq	r0,r0,r0

l00001FB1:
	andeq	r0,r0,r0

l00001FB5:
	andeq	r0,r0,r0

l00001FB9:
	andeq	r0,r0,r0

l00001FBD:
	andeq	r0,r0,r0

l00001FC1:
	andeq	r0,r0,r0

l00001FC5:
	andeq	r0,r0,r0

l00001FC9:
	andeq	r0,r0,r0

l00001FCD:
	andeq	r0,r0,r0

l00001FD1:
	andeq	r0,r0,r0

l00001FD5:
	andeq	r0,r0,r0

l00001FD9:
	andeq	r0,r0,r0

l00001FDD:
	andeq	r0,r0,r0

l00001FE1:
	andeq	r0,r0,r0

l00001FE5:
	andeq	r0,r0,r0

l00001FE9:
	andeq	r0,r0,r0

l00001FED:
	andeq	r0,r0,r0

l00001FF1:
	andeq	r0,r0,r0

l00001FF5:
	andeq	r0,r0,r0

l00001FF9:
	andeq	r0,r0,r0

l00001FFD:
	andeq	r0,r0,r0

l00002001:
	andeq	r0,r0,r0

l00002005:
	andeq	r0,r0,r0

l00002009:
	andeq	r0,r0,r0

l0000200D:
	andeq	r0,r0,r0

l00002011:
	andeq	r0,r0,r0

l00002015:
	andeq	r0,r0,r0

l00002019:
	andeq	r0,r0,r0

l0000201D:
	andeq	r0,r0,r0

l00002021:
	andeq	r0,r0,r0

l00002025:
	andeq	r0,r0,r0

l00002029:
	andeq	r0,r0,r0

l0000202D:
	andeq	r0,r0,r0

l00002031:
	andeq	r0,r0,r0

l00002035:
	andeq	r0,r0,r0

l00002039:
	andeq	r0,r0,r0

l0000203D:
	andeq	r0,r0,r0

l00002041:
	andeq	r0,r0,r0

l00002045:
	andeq	r0,r0,r0

l00002049:
	andeq	r0,r0,r0

l0000204D:
	andeq	r0,r0,r0

l00002051:
	andeq	r0,r0,r0

l00002055:
	andeq	r0,r0,r0

l00002059:
	andeq	r0,r0,r0

l0000205D:
	andeq	r0,r0,r0

l00002061:
	andeq	r0,r0,r0

l00002065:
	andeq	r0,r0,r0

l00002069:
	andeq	r0,r0,r0

l0000206D:
	andeq	r0,r0,r0

l00002071:
	andeq	r0,r0,r0

l00002075:
	andeq	r0,r0,r0

l00002079:
	andeq	r0,r0,r0

l0000207D:
	andeq	r0,r0,r0

l00002081:
	andeq	r0,r0,r0

l00002085:
	andeq	r0,r0,r0

l00002089:
	andeq	r0,r0,r0

l0000208D:
	andeq	r0,r0,r0

l00002091:
	andeq	r0,r0,r0

l00002095:
	andeq	r0,r0,r0

l00002099:
	andeq	r0,r0,r0

l0000209D:
	andeq	r0,r0,r0

l000020A1:
	andeq	r0,r0,r0

l000020A5:
	andeq	r0,r0,r0

l000020A9:
	andeq	r0,r0,r0

l000020AD:
	andeq	r0,r0,r0

l000020B1:
	andeq	r0,r0,r0

l000020B5:
	andeq	r0,r0,r0

l000020B9:
	andeq	r0,r0,r0

l000020BD:
	andeq	r0,r0,r0

l000020C1:
	andeq	r0,r0,r0

l000020C5:
	andeq	r0,r0,r0

l000020C9:
	andeq	r0,r0,r0

l000020CD:
	andeq	r0,r0,r0

l000020D1:
	andeq	r0,r0,r0

l000020D5:
	andeq	r0,r0,r0

l000020D9:
	andeq	r0,r0,r0

l000020DD:
	andeq	r0,r0,r0

l000020E1:
	andeq	r0,r0,r0

l000020E5:
	andeq	r0,r0,r0

l000020E9:
	andeq	r0,r0,r0

l000020ED:
	andeq	r0,r0,r0

l000020F1:
	andeq	r0,r0,r0

l000020F5:
	andeq	r0,r0,r0

l000020F9:
	andeq	r0,r0,r0

l000020FD:
	andeq	r0,r0,r0

l00002101:
	andeq	r0,r0,r0

l00002105:
	andeq	r0,r0,r0

l00002109:
	andeq	r0,r0,r0

l0000210D:
	andeq	r0,r0,r0

l00002111:
	andeq	r0,r0,r0

l00002115:
	andeq	r0,r0,r0

l00002119:
	andeq	r0,r0,r0

l0000211D:
	andeq	r0,r0,r0

l00002121:
	andeq	r0,r0,r0

l00002125:
	andeq	r0,r0,r0

l00002129:
	andeq	r0,r0,r0

l0000212D:
	andeq	r0,r0,r0

l00002131:
	andeq	r0,r0,r0

l00002135:
	andeq	r0,r0,r0

l00002139:
	andeq	r0,r0,r0

l0000213D:
	andeq	r0,r0,r0

l00002141:
	andeq	r0,r0,r0

l00002145:
	andeq	r0,r0,r0

l00002149:
	andeq	r0,r0,r0

l0000214D:
	andeq	r0,r0,r0

l00002151:
	andeq	r0,r0,r0

l00002155:
	andeq	r0,r0,r0

l00002159:
	andeq	r0,r0,r0

l0000215D:
	andeq	r0,r0,r0

l00002161:
	andeq	r0,r0,r0

l00002165:
	andeq	r0,r0,r0

l00002169:
	andeq	r0,r0,r0

l0000216D:
	andeq	r0,r0,r0

l00002171:
	andeq	r0,r0,r0

l00002175:
	andeq	r0,r0,r0

l00002179:
	andeq	r0,r0,r0

l0000217D:
	andeq	r0,r0,r0

l00002181:
	andeq	r0,r0,r0

l00002185:
	andeq	r0,r0,r0

l00002189:
	andeq	r0,r0,r0

l0000218D:
	andeq	r0,r0,r0

l00002191:
	andeq	r0,r0,r0

l00002195:
	andeq	r0,r0,r0

l00002199:
	andeq	r0,r0,r0

l0000219D:
	andeq	r0,r0,r0

l000021A1:
	andeq	r0,r0,r0

l000021A5:
	andeq	r0,r0,r0

l000021A9:
	andeq	r0,r0,r0

l000021AD:
	andeq	r0,r0,r0

l000021B1:
	andeq	r0,r0,r0

l000021B5:
	andeq	r0,r0,r0

l000021B9:
	andeq	r0,r0,r0

l000021BD:
	andeq	r0,r0,r0

l000021C1:
	andeq	r0,r0,r0

l000021C5:
	andeq	r0,r0,r0

l000021C9:
	andeq	r0,r0,r0

l000021CD:
	andeq	r0,r0,r0

l000021D1:
	andeq	r0,r0,r0

l000021D5:
	andeq	r0,r0,r0

l000021D9:
	andeq	r0,r0,r0

l000021DD:
	andeq	r0,r0,r0

l000021E1:
	andeq	r0,r0,r0

l000021E5:
	andeq	r0,r0,r0

l000021E9:
	andeq	r0,r0,r0

l000021ED:
	andeq	r0,r0,r0

l000021F1:
	andeq	r0,r0,r0

l000021F5:
	andeq	r0,r0,r0

l000021F9:
	andeq	r0,r0,r0

l000021FD:
	andeq	r0,r0,r0

l00002201:
	andeq	r0,r0,r0

l00002205:
	andeq	r0,r0,r0

l00002209:
	andeq	r0,r0,r0

l0000220D:
	andeq	r0,r0,r0

l00002211:
	andeq	r0,r0,r0

l00002215:
	andeq	r0,r0,r0

l00002219:
	andeq	r0,r0,r0

l0000221D:
	andeq	r0,r0,r0

l00002221:
	andeq	r0,r0,r0

l00002225:
	andeq	r0,r0,r0

l00002229:
	andeq	r0,r0,r0

l0000222D:
	andeq	r0,r0,r0

l00002231:
	andeq	r0,r0,r0

l00002235:
	andeq	r0,r0,r0

l00002239:
	andeq	r0,r0,r0

l0000223D:
	andeq	r0,r0,r0

l00002241:
	andeq	r0,r0,r0

l00002245:
	andeq	r0,r0,r0

l00002249:
	andeq	r0,r0,r0

l0000224D:
	andeq	r0,r0,r0

l00002251:
	andeq	r0,r0,r0

l00002255:
	andeq	r0,r0,r0

l00002259:
	andeq	r0,r0,r0

l0000225D:
	andeq	r0,r0,r0

l00002261:
	andeq	r0,r0,r0

l00002265:
	andeq	r0,r0,r0

l00002269:
	andeq	r0,r0,r0

l0000226D:
	andeq	r0,r0,r0

l00002271:
	andeq	r0,r0,r0

l00002275:
	andeq	r0,r0,r0

l00002279:
	andeq	r0,r0,r0

l0000227D:
	andeq	r0,r0,r0

l00002281:
	andeq	r0,r0,r0

l00002285:
	andeq	r0,r0,r0

l00002289:
	andeq	r0,r0,r0

l0000228D:
	andeq	r0,r0,r0

l00002291:
	andeq	r0,r0,r0

l00002295:
	andeq	r0,r0,r0

l00002299:
	andeq	r0,r0,r0

l0000229D:
	andeq	r0,r0,r0

l000022A1:
	andeq	r0,r0,r0

l000022A5:
	andeq	r0,r0,r0

l000022A9:
	andeq	r0,r0,r0

l000022AD:
	andeq	r0,r0,r0

l000022B1:
	andeq	r0,r0,r0

l000022B5:
	andeq	r0,r0,r0

l000022B9:
	andeq	r0,r0,r0

l000022BD:
	andeq	r0,r0,r0

l000022C1:
	andeq	r0,r0,r0

l000022C5:
	andeq	r0,r0,r0

l000022C9:
	andeq	r0,r0,r0

l000022CD:
	andeq	r0,r0,r0

l000022D1:
	andeq	r0,r0,r0

l000022D5:
	andeq	r0,r0,r0

l000022D9:
	andeq	r0,r0,r0

l000022DD:
	andeq	r0,r0,r0

l000022E1:
	andeq	r0,r0,r0

l000022E5:
	andeq	r0,r0,r0

l000022E9:
	andeq	r0,r0,r0

l000022ED:
	andeq	r0,r0,r0

l000022F1:
	andeq	r0,r0,r0

l000022F5:
	andeq	r0,r0,r0

l000022F9:
	andeq	r0,r0,r0

l000022FD:
	andeq	r0,r0,r0

l00002301:
	andeq	r0,r0,r0

l00002305:
	andeq	r0,r0,r0

l00002309:
	andeq	r0,r0,r0

l0000230D:
	andeq	r0,r0,r0

l00002311:
	andeq	r0,r0,r0

l00002315:
	andeq	r0,r0,r0

l00002319:
	andeq	r0,r0,r0

l0000231D:
	andeq	r0,r0,r0

l00002321:
	andeq	r0,r0,r0

l00002325:
	andeq	r0,r0,r0

l00002329:
	andeq	r0,r0,r0

l0000232D:
	andeq	r0,r0,r0

l00002331:
	andeq	r0,r0,r0

l00002335:
	andeq	r0,r0,r0

l00002339:
	andeq	r0,r0,r0

l0000233D:
	andeq	r0,r0,r0

l00002341:
	andeq	r0,r0,r0

l00002345:
	andeq	r0,r0,r0

l00002349:
	andeq	r0,r0,r0

l0000234D:
	andeq	r0,r0,r0

l00002351:
	andeq	r0,r0,r0

l00002355:
	andeq	r0,r0,r0

l00002359:
	andeq	r0,r0,r0

l0000235D:
	andeq	r0,r0,r0

l00002361:
	andeq	r0,r0,r0

l00002365:
	andeq	r0,r0,r0

l00002369:
	andeq	r0,r0,r0

l0000236D:
	andeq	r0,r0,r0

l00002371:
	andeq	r0,r0,r0

l00002375:
	andeq	r0,r0,r0

l00002379:
	andeq	r0,r0,r0

l0000237D:
	andeq	r0,r0,r0

l00002381:
	andeq	r0,r0,r0

l00002385:
	andeq	r0,r0,r0

l00002389:
	andeq	r0,r0,r0

l0000238D:
	andeq	r0,r0,r0

l00002391:
	andeq	r0,r0,r0

l00002395:
	andeq	r0,r0,r0

l00002399:
	andeq	r0,r0,r0

l0000239D:
	andeq	r0,r0,r0

l000023A1:
	andeq	r0,r0,r0

l000023A5:
	andeq	r0,r0,r0

l000023A9:
	andeq	r0,r0,r0

l000023AD:
	andeq	r0,r0,r0

l000023B1:
	andeq	r0,r0,r0

l000023B5:
	andeq	r0,r0,r0

l000023B9:
	andeq	r0,r0,r0

l000023BD:
	andeq	r0,r0,r0

l000023C1:
	andeq	r0,r0,r0

l000023C5:
	andeq	r0,r0,r0

l000023C9:
	andeq	r0,r0,r0

l000023CD:
	andeq	r0,r0,r0

l000023D1:
	andeq	r0,r0,r0

l000023D5:
	andeq	r0,r0,r0

l000023D9:
	andeq	r0,r0,r0

l000023DD:
	andeq	r0,r0,r0

l000023E1:
	andeq	r0,r0,r0

l000023E5:
	andeq	r0,r0,r0

l000023E9:
	andeq	r0,r0,r0

l000023ED:
	andeq	r0,r0,r0

l000023F1:
	andeq	r0,r0,r0

l000023F5:
	andeq	r0,r0,r0

l000023F9:
	andeq	r0,r0,r0

l000023FD:
	andeq	r0,r0,r0

l00002401:
	andeq	r0,r0,r0

l00002405:
	andeq	r0,r0,r0

l00002409:
	andeq	r0,r0,r0

l0000240D:
	andeq	r0,r0,r0

l00002411:
	andeq	r0,r0,r0

l00002415:
	andeq	r0,r0,r0

l00002419:
	andeq	r0,r0,r0

l0000241D:
	andeq	r0,r0,r0

l00002421:
	andeq	r0,r0,r0

l00002425:
	andeq	r0,r0,r0

l00002429:
	andeq	r0,r0,r0

l0000242D:
	andeq	r0,r0,r0

l00002431:
	andeq	r0,r0,r0

l00002435:
	andeq	r0,r0,r0

l00002439:
	andeq	r0,r0,r0

l0000243D:
	andeq	r0,r0,r0

l00002441:
	andeq	r0,r0,r0

l00002445:
	andeq	r0,r0,r0

l00002449:
	andeq	r0,r0,r0

l0000244D:
	andeq	r0,r0,r0

l00002451:
	andeq	r0,r0,r0

l00002455:
	andeq	r0,r0,r0

l00002459:
	andeq	r0,r0,r0

l0000245D:
	andeq	r0,r0,r0

l00002461:
	andeq	r0,r0,r0

l00002465:
	andeq	r0,r0,r0

l00002469:
	andeq	r0,r0,r0

l0000246D:
	andeq	r0,r0,r0

l00002471:
	andeq	r0,r0,r0

l00002475:
	andeq	r0,r0,r0

l00002479:
	andeq	r0,r0,r0

l0000247D:
	andeq	r0,r0,r0

l00002481:
	andeq	r0,r0,r0

l00002485:
	andeq	r0,r0,r0

l00002489:
	andeq	r0,r0,r0

l0000248D:
	andeq	r0,r0,r0

l00002491:
	andeq	r0,r0,r0

l00002495:
	andeq	r0,r0,r0

l00002499:
	andeq	r0,r0,r0

l0000249D:
	andeq	r0,r0,r0

l000024A1:
	andeq	r0,r0,r0

l000024A5:
	andeq	r0,r0,r0

l000024A9:
	andeq	r0,r0,r0

l000024AD:
	andeq	r0,r0,r0

l000024B1:
	andeq	r0,r0,r0

l000024B5:
	andeq	r0,r0,r0

l000024B9:
	andeq	r0,r0,r0

l000024BD:
	andeq	r0,r0,r0

l000024C1:
	andeq	r0,r0,r0

l000024C5:
	andeq	r0,r0,r0

l000024C9:
	andeq	r0,r0,r0

l000024CD:
	andeq	r0,r0,r0

l000024D1:
	andeq	r0,r0,r0

l000024D5:
	andeq	r0,r0,r0

l000024D9:
	andeq	r0,r0,r0

l000024DD:
	andeq	r0,r0,r0

l000024E1:
	andeq	r0,r0,r0

l000024E5:
	andeq	r0,r0,r0

l000024E9:
	andeq	r0,r0,r0

l000024ED:
	andeq	r0,r0,r0

l000024F1:
	andeq	r0,r0,r0

l000024F5:
	andeq	r0,r0,r0

l000024F9:
	andeq	r0,r0,r0

l000024FD:
	andeq	r0,r0,r0

l00002501:
	andeq	r0,r0,r0

l00002505:
	andeq	r0,r0,r0

l00002509:
	andeq	r0,r0,r0

l0000250D:
	andeq	r0,r0,r0

l00002511:
	andeq	r0,r0,r0

l00002515:
	andeq	r0,r0,r0

l00002519:
	andeq	r0,r0,r0

l0000251D:
	andeq	r0,r0,r0

l00002521:
	andeq	r0,r0,r0

l00002525:
	andeq	r0,r0,r0

l00002529:
	andeq	r0,r0,r0

l0000252D:
	andeq	r0,r0,r0

l00002531:
	andeq	r0,r0,r0

l00002535:
	andeq	r0,r0,r0

l00002539:
	andeq	r0,r0,r0

l0000253D:
	andeq	r0,r0,r0

l00002541:
	andeq	r0,r0,r0

l00002545:
	andeq	r0,r0,r0

l00002549:
	andeq	r0,r0,r0

l0000254D:
	andeq	r0,r0,r0

l00002551:
	andeq	r0,r0,r0

l00002555:
	andeq	r0,r0,r0

l00002559:
	andeq	r0,r0,r0

l0000255D:
	andeq	r0,r0,r0

l00002561:
	andeq	r0,r0,r0

l00002565:
	andeq	r0,r0,r0

l00002569:
	andeq	r0,r0,r0

l0000256D:
	andeq	r0,r0,r0

l00002571:
	andeq	r0,r0,r0

l00002575:
	andeq	r0,r0,r0

l00002579:
	andeq	r0,r0,r0

l0000257D:
	andeq	r0,r0,r0

l00002581:
	andeq	r0,r0,r0

l00002585:
	andeq	r0,r0,r0

l00002589:
	andeq	r0,r0,r0

l0000258D:
	andeq	r0,r0,r0

l00002591:
	andeq	r0,r0,r0

l00002595:
	andeq	r0,r0,r0

l00002599:
	andeq	r0,r0,r0

l0000259D:
	andeq	r0,r0,r0

l000025A1:
	andeq	r0,r0,r0

l000025A5:
	andeq	r0,r0,r0

l000025A9:
	andeq	r0,r0,r0

l000025AD:
	andeq	r0,r0,r0

l000025B1:
	andeq	r0,r0,r0

l000025B5:
	andeq	r0,r0,r0

l000025B9:
	andeq	r0,r0,r0

l000025BD:
	andeq	r0,r0,r0

l000025C1:
	andeq	r0,r0,r0

l000025C5:
	andeq	r0,r0,r0

l000025C9:
	andeq	r0,r0,r0

l000025CD:
	andeq	r0,r0,r0

l000025D1:
	andeq	r0,r0,r0

l000025D5:
	andeq	r0,r0,r0

l000025D9:
	andeq	r0,r0,r0

l000025DD:
	andeq	r0,r0,r0

l000025E1:
	andeq	r0,r0,r0

l000025E5:
	andeq	r0,r0,r0

l000025E9:
	andeq	r0,r0,r0

l000025ED:
	andeq	r0,r0,r0

l000025F1:
	andeq	r0,r0,r0

l000025F5:
	andeq	r0,r0,r0

l000025F9:
	andeq	r0,r0,r0

l000025FD:
	andeq	r0,r0,r0

l00002601:
	andeq	r0,r0,r0

l00002605:
	andeq	r0,r0,r0

l00002609:
	andeq	r0,r0,r0

l0000260D:
	andeq	r0,r0,r0

l00002611:
	andeq	r0,r0,r0

l00002615:
	andeq	r0,r0,r0

l00002619:
	andeq	r0,r0,r0

l0000261D:
	andeq	r0,r0,r0

l00002621:
	andeq	r0,r0,r0

l00002625:
	andeq	r0,r0,r0

l00002629:
	andeq	r0,r0,r0

l0000262D:
	andeq	r0,r0,r0

l00002631:
	andeq	r0,r0,r0

l00002635:
	andeq	r0,r0,r0

l00002639:
	andeq	r0,r0,r0

l0000263D:
	andeq	r0,r0,r0

l00002641:
	andeq	r0,r0,r0

l00002645:
	andeq	r0,r0,r0

l00002649:
	andeq	r0,r0,r0

l0000264D:
	andeq	r0,r0,r0

l00002651:
	andeq	r0,r0,r0

l00002655:
	andeq	r0,r0,r0

l00002659:
	andeq	r0,r0,r0

l0000265D:
	andeq	r0,r0,r0

l00002661:
	andeq	r0,r0,r0

l00002665:
	andeq	r0,r0,r0

l00002669:
	andeq	r0,r0,r0

l0000266D:
	andeq	r0,r0,r0

l00002671:
	andeq	r0,r0,r0

l00002675:
	andeq	r0,r0,r0

l00002679:
	andeq	r0,r0,r0

l0000267D:
	andeq	r0,r0,r0

l00002681:
	andeq	r0,r0,r0

l00002685:
	andeq	r0,r0,r0

l00002689:
	andeq	r0,r0,r0

l0000268D:
	andeq	r0,r0,r0

l00002691:
	andeq	r0,r0,r0

l00002695:
	andeq	r0,r0,r0

l00002699:
	andeq	r0,r0,r0

l0000269D:
	andeq	r0,r0,r0

l000026A1:
	andeq	r0,r0,r0

l000026A5:
	andeq	r0,r0,r0

l000026A9:
	andeq	r0,r0,r0

l000026AD:
	andeq	r0,r0,r0

l000026B1:
	andeq	r0,r0,r0

l000026B5:
	andeq	r0,r0,r0

l000026B9:
	andeq	r0,r0,r0

l000026BD:
	andeq	r0,r0,r0

l000026C1:
	andeq	r0,r0,r0

l000026C5:
	andeq	r0,r0,r0

l000026C9:
	andeq	r0,r0,r0

l000026CD:
	andeq	r0,r0,r0

l000026D1:
	andeq	r0,r0,r0

l000026D5:
	andeq	r0,r0,r0

l000026D9:
	andeq	r0,r0,r0

l000026DD:
	andeq	r0,r0,r0

l000026E1:
	andeq	r0,r0,r0

l000026E5:
	andeq	r0,r0,r0

l000026E9:
	andeq	r0,r0,r0

l000026ED:
	andeq	r0,r0,r0

l000026F1:
	andeq	r0,r0,r0

l000026F5:
	andeq	r0,r0,r0

l000026F9:
	andeq	r0,r0,r0

l000026FD:
	andeq	r0,r0,r0

l00002701:
	andeq	r0,r0,r0

l00002705:
	andeq	r0,r0,r0

l00002709:
	andeq	r0,r0,r0

l0000270D:
	andeq	r0,r0,r0

l00002711:
	andeq	r0,r0,r0

l00002715:
	andeq	r0,r0,r0

l00002719:
	andeq	r0,r0,r0

l0000271D:
	andeq	r0,r0,r0

l00002721:
	andeq	r0,r0,r0

l00002725:
	andeq	r0,r0,r0

l00002729:
	andeq	r0,r0,r0

l0000272D:
	andeq	r0,r0,r0

l00002731:
	andeq	r0,r0,r0

l00002735:
	andeq	r0,r0,r0

l00002739:
	andeq	r0,r0,r0

l0000273D:
	andeq	r0,r0,r0

l00002741:
	andeq	r0,r0,r0

l00002745:
	andeq	r0,r0,r0

l00002749:
	andeq	r0,r0,r0

l0000274D:
	andeq	r0,r0,r0

l00002751:
	andeq	r0,r0,r0

l00002755:
	andeq	r0,r0,r0

l00002759:
	andeq	r0,r0,r0

l0000275D:
	andeq	r0,r0,r0

l00002761:
	andeq	r0,r0,r0

l00002765:
	andeq	r0,r0,r0

l00002769:
	andeq	r0,r0,r0

l0000276D:
	andeq	r0,r0,r0

l00002771:
	andeq	r0,r0,r0

l00002775:
	andeq	r0,r0,r0

l00002779:
	andeq	r0,r0,r0

l0000277D:
	andeq	r0,r0,r0

l00002781:
	andeq	r0,r0,r0

l00002785:
	andeq	r0,r0,r0

l00002789:
	andeq	r0,r0,r0

l0000278D:
	andeq	r0,r0,r0

l00002791:
	andeq	r0,r0,r0

l00002795:
	andeq	r0,r0,r0

l00002799:
	andeq	r0,r0,r0

l0000279D:
	andeq	r0,r0,r0

l000027A1:
	andeq	r0,r0,r0

l000027A5:
	andeq	r0,r0,r0

l000027A9:
	andeq	r0,r0,r0

l000027AD:
	andeq	r0,r0,r0

l000027B1:
	andeq	r0,r0,r0

l000027B5:
	andeq	r0,r0,r0

l000027B9:
	andeq	r0,r0,r0

l000027BD:
	andeq	r0,r0,r0

l000027C1:
	andeq	r0,r0,r0

l000027C5:
	andeq	r0,r0,r0

l000027C9:
	andeq	r0,r0,r0

l000027CD:
	andeq	r0,r0,r0

l000027D1:
	andeq	r0,r0,r0

l000027D5:
	andeq	r0,r0,r0

l000027D9:
	andeq	r0,r0,r0

l000027DD:
	andeq	r0,r0,r0

l000027E1:
	andeq	r0,r0,r0

l000027E5:
	andeq	r0,r0,r0

l000027E9:
	andeq	r0,r0,r0

l000027ED:
	andeq	r0,r0,r0

l000027F1:
	andeq	r0,r0,r0

l000027F5:
	andeq	r0,r0,r0

l000027F9:
	andeq	r0,r0,r0

l000027FD:
	andeq	r0,r0,r0

l00002801:
	andeq	r0,r0,r0

l00002805:
	andeq	r0,r0,r0

l00002809:
	andeq	r0,r0,r0

l0000280D:
	andeq	r0,r0,r0

l00002811:
	andeq	r0,r0,r0

l00002815:
	andeq	r0,r0,r0

l00002819:
	andeq	r0,r0,r0

l0000281D:
	andeq	r0,r0,r0

l00002821:
	andeq	r0,r0,r0

l00002825:
	andeq	r0,r0,r0

l00002829:
	andeq	r0,r0,r0

l0000282D:
	andeq	r0,r0,r0

l00002831:
	andeq	r0,r0,r0

l00002835:
	andeq	r0,r0,r0

l00002839:
	andeq	r0,r0,r0

l0000283D:
	andeq	r0,r0,r0

l00002841:
	andeq	r0,r0,r0

l00002845:
	andeq	r0,r0,r0

l00002849:
	andeq	r0,r0,r0

l0000284D:
	andeq	r0,r0,r0

l00002851:
	andeq	r0,r0,r0

l00002855:
	andeq	r0,r0,r0

l00002859:
	andeq	r0,r0,r0

l0000285D:
	andeq	r0,r0,r0

l00002861:
	andeq	r0,r0,r0

l00002865:
	andeq	r0,r0,r0

l00002869:
	andeq	r0,r0,r0

l0000286D:
	andeq	r0,r0,r0

l00002871:
	andeq	r0,r0,r0

l00002875:
	andeq	r0,r0,r0

l00002879:
	andeq	r0,r0,r0

l0000287D:
	andeq	r0,r0,r0

l00002881:
	andeq	r0,r0,r0

l00002885:
	andeq	r0,r0,r0

l00002889:
	andeq	r0,r0,r0

l0000288D:
	andeq	r0,r0,r0

l00002891:
	andeq	r0,r0,r0

l00002895:
	andeq	r0,r0,r0

l00002899:
	andeq	r0,r0,r0

l0000289D:
	andeq	r0,r0,r0

l000028A1:
	andeq	r0,r0,r0

l000028A5:
	andeq	r0,r0,r0

l000028A9:
	andeq	r0,r0,r0

l000028AD:
	andeq	r0,r0,r0

l000028B1:
	andeq	r0,r0,r0

l000028B5:
	andeq	r0,r0,r0

l000028B9:
	andeq	r0,r0,r0

l000028BD:
	andeq	r0,r0,r0

l000028C1:
	andeq	r0,r0,r0

l000028C5:
	andeq	r0,r0,r0

l000028C9:
	andeq	r0,r0,r0

l000028CD:
	andeq	r0,r0,r0

l000028D1:
	andeq	r0,r0,r0

l000028D5:
	andeq	r0,r0,r0

l000028D9:
	andeq	r0,r0,r0

l000028DD:
	andeq	r0,r0,r0

l000028E1:
	andeq	r0,r0,r0

l000028E5:
	andeq	r0,r0,r0

l000028E9:
	andeq	r0,r0,r0

l000028ED:
	andeq	r0,r0,r0

l000028F1:
	andeq	r0,r0,r0

l000028F5:
	andeq	r0,r0,r0

l000028F9:
	andeq	r0,r0,r0

l000028FD:
	andeq	r0,r0,r0

l00002901:
	andeq	r0,r0,r0

l00002905:
	andeq	r0,r0,r0

l00002909:
	andeq	r0,r0,r0

l0000290D:
	andeq	r0,r0,r0

l00002911:
	andeq	r0,r0,r0

l00002915:
	andeq	r0,r0,r0

l00002919:
	andeq	r0,r0,r0

l0000291D:
	andeq	r0,r0,r0

l00002921:
	andeq	r0,r0,r0

l00002925:
	andeq	r0,r0,r0

l00002929:
	andeq	r0,r0,r0

l0000292D:
	andeq	r0,r0,r0

l00002931:
	andeq	r0,r0,r0

l00002935:
	andeq	r0,r0,r0

l00002939:
	andeq	r0,r0,r0

l0000293D:
	andeq	r0,r0,r0

l00002941:
	andeq	r0,r0,r0

l00002945:
	andeq	r0,r0,r0

l00002949:
	andeq	r0,r0,r0

l0000294D:
	andeq	r0,r0,r0

l00002951:
	andeq	r0,r0,r0

l00002955:
	andeq	r0,r0,r0

l00002959:
	andeq	r0,r0,r0

l0000295D:
	andeq	r0,r0,r0

l00002961:
	andeq	r0,r0,r0

l00002965:
	andeq	r0,r0,r0

l00002969:
	andeq	r0,r0,r0

l0000296D:
	andeq	r0,r0,r0

l00002971:
	andeq	r0,r0,r0

l00002975:
	andeq	r0,r0,r0

l00002979:
	andeq	r0,r0,r0

l0000297D:
	andeq	r0,r0,r0

l00002981:
	andeq	r0,r0,r0

l00002985:
	andeq	r0,r0,r0

l00002989:
	andeq	r0,r0,r0

l0000298D:
	andeq	r0,r0,r0

l00002991:
	andeq	r0,r0,r0

l00002995:
	andeq	r0,r0,r0

l00002999:
	andeq	r0,r0,r0

l0000299D:
	andeq	r0,r0,r0

l000029A1:
	andeq	r0,r0,r0

l000029A5:
	andeq	r0,r0,r0

l000029A9:
	andeq	r0,r0,r0

l000029AD:
	andeq	r0,r0,r0

l000029B1:
	andeq	r0,r0,r0

l000029B5:
	andeq	r0,r0,r0

l000029B9:
	andeq	r0,r0,r0

l000029BD:
	andeq	r0,r0,r0

l000029C1:
	andeq	r0,r0,r0

l000029C5:
	andeq	r0,r0,r0

l000029C9:
	andeq	r0,r0,r0

l000029CD:
	andeq	r0,r0,r0

l000029D1:
	andeq	r0,r0,r0

l000029D5:
	andeq	r0,r0,r0

l000029D9:
	andeq	r0,r0,r0

l000029DD:
	andeq	r0,r0,r0

l000029E1:
	andeq	r0,r0,r0

l000029E5:
	andeq	r0,r0,r0

l000029E9:
	andeq	r0,r0,r0

l000029ED:
	andeq	r0,r0,r0

l000029F1:
	andeq	r0,r0,r0

l000029F5:
	andeq	r0,r0,r0

l000029F9:
	andeq	r0,r0,r0

l000029FD:
	andeq	r0,r0,r0

l00002A01:
	andeq	r0,r0,r0

l00002A05:
	andeq	r0,r0,r0

l00002A09:
	andeq	r0,r0,r0

l00002A0D:
	andeq	r0,r0,r0

l00002A11:
	andeq	r0,r0,r0

l00002A15:
	andeq	r0,r0,r0

l00002A19:
	andeq	r0,r0,r0

l00002A1D:
	andeq	r0,r0,r0

l00002A21:
	andeq	r0,r0,r0

l00002A25:
	andeq	r0,r0,r0

l00002A29:
	andeq	r0,r0,r0

l00002A2D:
	andeq	r0,r0,r0

l00002A31:
	andeq	r0,r0,r0

l00002A35:
	andeq	r0,r0,r0

l00002A39:
	andeq	r0,r0,r0

l00002A3D:
	andeq	r0,r0,r0

l00002A41:
	andeq	r0,r0,r0

l00002A45:
	andeq	r0,r0,r0

l00002A49:
	andeq	r0,r0,r0

l00002A4D:
	andeq	r0,r0,r0

l00002A51:
	andeq	r0,r0,r0

l00002A55:
	andeq	r0,r0,r0

l00002A59:
	andeq	r0,r0,r0

l00002A5D:
	andeq	r0,r0,r0

l00002A61:
	andeq	r0,r0,r0

l00002A65:
	andeq	r0,r0,r0

l00002A69:
	andeq	r0,r0,r0

l00002A6D:
	andeq	r0,r0,r0

l00002A71:
	andeq	r0,r0,r0

l00002A75:
	andeq	r0,r0,r0

l00002A79:
	andeq	r0,r0,r0

l00002A7D:
	andeq	r0,r0,r0

l00002A81:
	andeq	r0,r0,r0

l00002A85:
	andeq	r0,r0,r0

l00002A89:
	andeq	r0,r0,r0

l00002A8D:
	andeq	r0,r0,r0

l00002A91:
	andeq	r0,r0,r0

l00002A95:
	andeq	r0,r0,r0

l00002A99:
	andeq	r0,r0,r0

l00002A9D:
	andeq	r0,r0,r0

l00002AA1:
	andeq	r0,r0,r0

l00002AA5:
	andeq	r0,r0,r0

l00002AA9:
	andeq	r0,r0,r0

l00002AAD:
	andeq	r0,r0,r0

l00002AB1:
	andeq	r0,r0,r0

l00002AB5:
	andeq	r0,r0,r0

l00002AB9:
	andeq	r0,r0,r0

l00002ABD:
	andeq	r0,r0,r0

l00002AC1:
	andeq	r0,r0,r0

l00002AC5:
	andeq	r0,r0,r0

l00002AC9:
	andeq	r0,r0,r0

l00002ACD:
	andeq	r0,r0,r0

l00002AD1:
	andeq	r0,r0,r0

l00002AD5:
	andeq	r0,r0,r0

l00002AD9:
	andeq	r0,r0,r0

l00002ADD:
	andeq	r0,r0,r0

l00002AE1:
	andeq	r0,r0,r0

l00002AE5:
	andeq	r0,r0,r0

l00002AE9:
	andeq	r0,r0,r0

l00002AED:
	andeq	r0,r0,r0

l00002AF1:
	andeq	r0,r0,r0

l00002AF5:
	andeq	r0,r0,r0

l00002AF9:
	andeq	r0,r0,r0

l00002AFD:
	andeq	r0,r0,r0

l00002B01:
	andeq	r0,r0,r0

l00002B05:
	andeq	r0,r0,r0

l00002B09:
	andeq	r0,r0,r0

l00002B0D:
	andeq	r0,r0,r0

l00002B11:
	andeq	r0,r0,r0

l00002B15:
	andeq	r0,r0,r0

l00002B19:
	andeq	r0,r0,r0

l00002B1D:
	andeq	r0,r0,r0

l00002B21:
	andeq	r0,r0,r0

l00002B25:
	andeq	r0,r0,r0

l00002B29:
	andeq	r0,r0,r0

l00002B2D:
	andeq	r0,r0,r0

l00002B31:
	andeq	r0,r0,r0

l00002B35:
	andeq	r0,r0,r0

l00002B39:
	andeq	r0,r0,r0

l00002B3D:
	andeq	r0,r0,r0

l00002B41:
	andeq	r0,r0,r0

l00002B45:
	andeq	r0,r0,r0

l00002B49:
	andeq	r0,r0,r0

l00002B4D:
	andeq	r0,r0,r0

l00002B51:
	andeq	r0,r0,r0

l00002B55:
	andeq	r0,r0,r0

l00002B59:
	andeq	r0,r0,r0

l00002B5D:
	andeq	r0,r0,r0

l00002B61:
	andeq	r0,r0,r0

l00002B65:
	andeq	r0,r0,r0

l00002B69:
	andeq	r0,r0,r0

l00002B6D:
	andeq	r0,r0,r0

l00002B71:
	andeq	r0,r0,r0

l00002B75:
	andeq	r0,r0,r0

l00002B79:
	andeq	r0,r0,r0

l00002B7D:
	andeq	r0,r0,r0

l00002B81:
	andeq	r0,r0,r0

l00002B85:
	andeq	r0,r0,r0

l00002B89:
	andeq	r0,r0,r0

l00002B8D:
	andeq	r0,r0,r0

l00002B91:
	andeq	r0,r0,r0

l00002B95:
	andeq	r0,r0,r0

l00002B99:
	andeq	r0,r0,r0

l00002B9D:
	andeq	r0,r0,r0

l00002BA1:
	andeq	r0,r0,r0

l00002BA5:
	andeq	r0,r0,r0

l00002BA9:
	andeq	r0,r0,r0

l00002BAD:
	andeq	r0,r0,r0

l00002BB1:
	andeq	r0,r0,r0

l00002BB5:
	andeq	r0,r0,r0

l00002BB9:
	andeq	r0,r0,r0

l00002BBD:
	andeq	r0,r0,r0

l00002BC1:
	andeq	r0,r0,r0

l00002BC5:
	andeq	r0,r0,r0

l00002BC9:
	andeq	r0,r0,r0

l00002BCD:
	andeq	r0,r0,r0

l00002BD1:
	andeq	r0,r0,r0

l00002BD5:
	andeq	r0,r0,r0

l00002BD9:
	andeq	r0,r0,r0

l00002BDD:
	andeq	r0,r0,r0

l00002BE1:
	andeq	r0,r0,r0

l00002BE5:
	andeq	r0,r0,r0

l00002BE9:
	andeq	r0,r0,r0

l00002BED:
	andeq	r0,r0,r0

l00002BF1:
	andeq	r0,r0,r0

l00002BF5:
	andeq	r0,r0,r0

l00002BF9:
	andeq	r0,r0,r0

l00002BFD:
	andeq	r0,r0,r0

l00002C01:
	andeq	r0,r0,r0

l00002C05:
	andeq	r0,r0,r0

l00002C09:
	andeq	r0,r0,r0

l00002C0D:
	andeq	r0,r0,r0

l00002C11:
	andeq	r0,r0,r0

l00002C15:
	andeq	r0,r0,r0

l00002C19:
	andeq	r0,r0,r0

l00002C1D:
	andeq	r0,r0,r0

l00002C21:
	andeq	r0,r0,r0

l00002C25:
	andeq	r0,r0,r0

l00002C29:
	andeq	r0,r0,r0

l00002C2D:
	andeq	r0,r0,r0

l00002C31:
	andeq	r0,r0,r0

l00002C35:
	andeq	r0,r0,r0

l00002C39:
	andeq	r0,r0,r0

l00002C3D:
	andeq	r0,r0,r0

l00002C41:
	andeq	r0,r0,r0

l00002C45:
	andeq	r0,r0,r0

l00002C49:
	andeq	r0,r0,r0

l00002C4D:
	andeq	r0,r0,r0

l00002C51:
	andeq	r0,r0,r0

l00002C55:
	andeq	r0,r0,r0

l00002C59:
	andeq	r0,r0,r0

l00002C5D:
	andeq	r0,r0,r0

l00002C61:
	andeq	r0,r0,r0

l00002C65:
	andeq	r0,r0,r0

l00002C69:
	andeq	r0,r0,r0

l00002C6D:
	andeq	r0,r0,r0

l00002C71:
	andeq	r0,r0,r0

l00002C75:
	andeq	r0,r0,r0

l00002C79:
	andeq	r0,r0,r0

l00002C7D:
	andeq	r0,r0,r0

l00002C81:
	andeq	r0,r0,r0

l00002C85:
	andeq	r0,r0,r0

l00002C89:
	andeq	r0,r0,r0

l00002C8D:
	andeq	r0,r0,r0

l00002C91:
	andeq	r0,r0,r0

l00002C95:
	andeq	r0,r0,r0

l00002C99:
	andeq	r0,r0,r0

l00002C9D:
	andeq	r0,r0,r0

l00002CA1:
	andeq	r0,r0,r0

l00002CA5:
	andeq	r0,r0,r0

l00002CA9:
	andeq	r0,r0,r0

l00002CAD:
	andeq	r0,r0,r0

l00002CB1:
	andeq	r0,r0,r0

l00002CB5:
	andeq	r0,r0,r0

l00002CB9:
	andeq	r0,r0,r0

l00002CBD:
	andeq	r0,r0,r0

l00002CC1:
	andeq	r0,r0,r0

l00002CC5:
	andeq	r0,r0,r0

l00002CC9:
	andeq	r0,r0,r0

l00002CCD:
	andeq	r0,r0,r0

l00002CD1:
	andeq	r0,r0,r0

l00002CD5:
	andeq	r0,r0,r0

l00002CD9:
	andeq	r0,r0,r0

l00002CDD:
	andeq	r0,r0,r0

l00002CE1:
	andeq	r0,r0,r0

l00002CE5:
	andeq	r0,r0,r0

l00002CE9:
	andeq	r0,r0,r0

l00002CED:
	andeq	r0,r0,r0

l00002CF1:
	andeq	r0,r0,r0

l00002CF5:
	andeq	r0,r0,r0

l00002CF9:
	andeq	r0,r0,r0

l00002CFD:
	andeq	r0,r0,r0

l00002D01:
	andeq	r0,r0,r0

l00002D05:
	andeq	r0,r0,r0

l00002D09:
	andeq	r0,r0,r0

l00002D0D:
	andeq	r0,r0,r0

l00002D11:
	andeq	r0,r0,r0

l00002D15:
	andeq	r0,r0,r0

l00002D19:
	andeq	r0,r0,r0

l00002D1D:
	andeq	r0,r0,r0

l00002D21:
	andeq	r0,r0,r0

l00002D25:
	andeq	r0,r0,r0

l00002D29:
	andeq	r0,r0,r0

l00002D2D:
	andeq	r0,r0,r0

l00002D31:
	andeq	r0,r0,r0

l00002D35:
	andeq	r0,r0,r0

l00002D39:
	andeq	r0,r0,r0

l00002D3D:
	andeq	r0,r0,r0

l00002D41:
	andeq	r0,r0,r0

l00002D45:
	andeq	r0,r0,r0

l00002D49:
	andeq	r0,r0,r0

l00002D4D:
	andeq	r0,r0,r0

l00002D51:
	andeq	r0,r0,r0

l00002D55:
	andeq	r0,r0,r0

l00002D59:
	andeq	r0,r0,r0

l00002D5D:
	andeq	r0,r0,r0

l00002D61:
	andeq	r0,r0,r0

l00002D65:
	andeq	r0,r0,r0

l00002D69:
	andeq	r0,r0,r0

l00002D6D:
	andeq	r0,r0,r0

l00002D71:
	andeq	r0,r0,r0

l00002D75:
	andeq	r0,r0,r0

l00002D79:
	andeq	r0,r0,r0

l00002D7D:
	andeq	r0,r0,r0

l00002D81:
	andeq	r0,r0,r0

l00002D85:
	andeq	r0,r0,r0

l00002D89:
	andeq	r0,r0,r0

l00002D8D:
	andeq	r0,r0,r0

l00002D91:
	andeq	r0,r0,r0

l00002D95:
	andeq	r0,r0,r0

l00002D99:
	andeq	r0,r0,r0

l00002D9D:
	andeq	r0,r0,r0

l00002DA1:
	andeq	r0,r0,r0

l00002DA5:
	andeq	r0,r0,r0

l00002DA9:
	andeq	r0,r0,r0

l00002DAD:
	andeq	r0,r0,r0

l00002DB1:
	andeq	r0,r0,r0

l00002DB5:
	andeq	r0,r0,r0

l00002DB9:
	andeq	r0,r0,r0

l00002DBD:
	andeq	r0,r0,r0

l00002DC1:
	andeq	r0,r0,r0

l00002DC5:
	andeq	r0,r0,r0

l00002DC9:
	andeq	r0,r0,r0

l00002DCD:
	andeq	r0,r0,r0

l00002DD1:
	andeq	r0,r0,r0

l00002DD5:
	andeq	r0,r0,r0

l00002DD9:
	andeq	r0,r0,r0

l00002DDD:
	andeq	r0,r0,r0

l00002DE1:
	andeq	r0,r0,r0

l00002DE5:
	andeq	r0,r0,r0

l00002DE9:
	andeq	r0,r0,r0

l00002DED:
	andeq	r0,r0,r0

l00002DF1:
	andeq	r0,r0,r0

l00002DF5:
	andeq	r0,r0,r0

l00002DF9:
	andeq	r0,r0,r0

l00002DFD:
	andeq	r0,r0,r0

l00002E01:
	andeq	r0,r0,r0

l00002E05:
	andeq	r0,r0,r0

l00002E09:
	andeq	r0,r0,r0

l00002E0D:
	andeq	r0,r0,r0

l00002E11:
	andeq	r0,r0,r0

l00002E15:
	andeq	r0,r0,r0

l00002E19:
	andeq	r0,r0,r0

l00002E1D:
	andeq	r0,r0,r0

l00002E21:
	andeq	r0,r0,r0

l00002E25:
	andeq	r0,r0,r0

l00002E29:
	andeq	r0,r0,r0

l00002E2D:
	andeq	r0,r0,r0

l00002E31:
	andeq	r0,r0,r0

l00002E35:
	andeq	r0,r0,r0

l00002E39:
	andeq	r0,r0,r0

l00002E3D:
	andeq	r0,r0,r0

l00002E41:
	andeq	r0,r0,r0

l00002E45:
	andeq	r0,r0,r0

l00002E49:
	andeq	r0,r0,r0

l00002E4D:
	andeq	r0,r0,r0

l00002E51:
	andeq	r0,r0,r0

l00002E55:
	andeq	r0,r0,r0

l00002E59:
	andeq	r0,r0,r0

l00002E5D:
	andeq	r0,r0,r0

l00002E61:
	andeq	r0,r0,r0

l00002E65:
	andeq	r0,r0,r0

l00002E69:
	andeq	r0,r0,r0

l00002E6D:
	andeq	r0,r0,r0

l00002E71:
	andeq	r0,r0,r0

l00002E75:
	andeq	r0,r0,r0

l00002E79:
	andeq	r0,r0,r0

l00002E7D:
	andeq	r0,r0,r0

l00002E81:
	andeq	r0,r0,r0

l00002E85:
	andeq	r0,r0,r0

l00002E89:
	andeq	r0,r0,r0

l00002E8D:
	andeq	r0,r0,r0

l00002E91:
	andeq	r0,r0,r0

l00002E95:
	andeq	r0,r0,r0

l00002E99:
	andeq	r0,r0,r0

l00002E9D:
	andeq	r0,r0,r0

l00002EA1:
	andeq	r0,r0,r0

l00002EA5:
	andeq	r0,r0,r0

l00002EA9:
	andeq	r0,r0,r0

l00002EAD:
	andeq	r0,r0,r0

l00002EB1:
	andeq	r0,r0,r0

l00002EB5:
	andeq	r0,r0,r0

l00002EB9:
	andeq	r0,r0,r0

l00002EBD:
	andeq	r0,r0,r0

l00002EC1:
	andeq	r0,r0,r0

l00002EC5:
	andeq	r0,r0,r0

l00002EC9:
	andeq	r0,r0,r0

l00002ECD:
	andeq	r0,r0,r0

l00002ED1:
	andeq	r0,r0,r0

l00002ED5:
	andeq	r0,r0,r0

l00002ED9:
	andeq	r0,r0,r0

l00002EDD:
	andeq	r0,r0,r0

l00002EE1:
	andeq	r0,r0,r0

l00002EE5:
	andeq	r0,r0,r0

l00002EE9:
	andeq	r0,r0,r0

l00002EED:
	andeq	r0,r0,r0

l00002EF1:
	andeq	r0,r0,r0

l00002EF5:
	andeq	r0,r0,r0

l00002EF9:
	andeq	r0,r0,r0

l00002EFD:
	andeq	r0,r0,r0

l00002F01:
	andeq	r0,r0,r0

l00002F05:
	andeq	r0,r0,r0

l00002F09:
	andeq	r0,r0,r0

l00002F0D:
	andeq	r0,r0,r0

l00002F11:
	andeq	r0,r0,r0

l00002F15:
	andeq	r0,r0,r0

l00002F19:
	andeq	r0,r0,r0

l00002F1D:
	andeq	r0,r0,r0

l00002F21:
	andeq	r0,r0,r0

l00002F25:
	andeq	r0,r0,r0

l00002F29:
	andeq	r0,r0,r0

l00002F2D:
	andeq	r0,r0,r0

l00002F31:
	andeq	r0,r0,r0

l00002F35:
	andeq	r0,r0,r0

l00002F39:
	andeq	r0,r0,r0

l00002F3D:
	andeq	r0,r0,r0

l00002F41:
	andeq	r0,r0,r0

l00002F45:
	andeq	r0,r0,r0

l00002F49:
	andeq	r0,r0,r0

l00002F4D:
	andeq	r0,r0,r0

l00002F51:
	andeq	r0,r0,r0

l00002F55:
	andeq	r0,r0,r0

l00002F59:
	andeq	r0,r0,r0

l00002F5D:
	andeq	r0,r0,r0

l00002F61:
	andeq	r0,r0,r0

l00002F65:
	andeq	r0,r0,r0

l00002F69:
	andeq	r0,r0,r0

l00002F6D:
	andeq	r0,r0,r0

l00002F71:
	andeq	r0,r0,r0

l00002F75:
	andeq	r0,r0,r0

l00002F79:
	andeq	r0,r0,r0

l00002F7D:
	andeq	r0,r0,r0

l00002F81:
	andeq	r0,r0,r0

l00002F85:
	andeq	r0,r0,r0

l00002F89:
	andeq	r0,r0,r0

l00002F8D:
	andeq	r0,r0,r0

l00002F91:
	andeq	r0,r0,r0

l00002F95:
	andeq	r0,r0,r0

l00002F99:
	andeq	r0,r0,r0

l00002F9D:
	andeq	r0,r0,r0

l00002FA1:
	andeq	r0,r0,r0

l00002FA5:
	andeq	r0,r0,r0

l00002FA9:
	andeq	r0,r0,r0

l00002FAD:
	andeq	r0,r0,r0

l00002FB1:
	andeq	r0,r0,r0

l00002FB5:
	andeq	r0,r0,r0

l00002FB9:
	andeq	r0,r0,r0

l00002FBD:
	andeq	r0,r0,r0

l00002FC1:
	andeq	r0,r0,r0

l00002FC5:
	andeq	r0,r0,r0

l00002FC9:
	andeq	r0,r0,r0

l00002FCD:
	andeq	r0,r0,r0

l00002FD1:
	andeq	r0,r0,r0

l00002FD5:
	andeq	r0,r0,r0

l00002FD9:
	andeq	r0,r0,r0

l00002FDD:
	andeq	r0,r0,r0

l00002FE1:
	andeq	r0,r0,r0

l00002FE5:
	andeq	r0,r0,r0

l00002FE9:
	andeq	r0,r0,r0

l00002FED:
	andeq	r0,r0,r0

l00002FF1:
	andeq	r0,r0,r0

l00002FF5:
	andeq	r0,r0,r0

l00002FF9:
	andeq	r0,r0,r0

l00002FFD:
	andeq	r0,r0,r0

l00003001:
	andeq	r0,r0,r0

l00003005:
	andeq	r0,r0,r0

l00003009:
	andeq	r0,r0,r0

l0000300D:
	andeq	r0,r0,r0

l00003011:
	andeq	r0,r0,r0

l00003015:
	andeq	r0,r0,r0

l00003019:
	andeq	r0,r0,r0

l0000301D:
	andeq	r0,r0,r0

l00003021:
	andeq	r0,r0,r0

l00003025:
	andeq	r0,r0,r0

l00003029:
	andeq	r0,r0,r0

l0000302D:
	andeq	r0,r0,r0

l00003031:
	andeq	r0,r0,r0

l00003035:
	andeq	r0,r0,r0

l00003039:
	andeq	r0,r0,r0

l0000303D:
	andeq	r0,r0,r0

l00003041:
	andeq	r0,r0,r0

l00003045:
	andeq	r0,r0,r0

l00003049:
	andeq	r0,r0,r0

l0000304D:
	andeq	r0,r0,r0

l00003051:
	andeq	r0,r0,r0

l00003055:
	andeq	r0,r0,r0

l00003059:
	andeq	r0,r0,r0

l0000305D:
	andeq	r0,r0,r0

l00003061:
	andeq	r0,r0,r0

l00003065:
	andeq	r0,r0,r0

l00003069:
	andeq	r0,r0,r0

l0000306D:
	andeq	r0,r0,r0

l00003071:
	andeq	r0,r0,r0

l00003075:
	andeq	r0,r0,r0

l00003079:
	andeq	r0,r0,r0

l0000307D:
	andeq	r0,r0,r0

l00003081:
	andeq	r0,r0,r0

l00003085:
	andeq	r0,r0,r0

l00003089:
	andeq	r0,r0,r0

l0000308D:
	andeq	r0,r0,r0

l00003091:
	andeq	r0,r0,r0

l00003095:
	andeq	r0,r0,r0

l00003099:
	andeq	r0,r0,r0

l0000309D:
	andeq	r0,r0,r0

l000030A1:
	andeq	r0,r0,r0

l000030A5:
	andeq	r0,r0,r0

l000030A9:
	andeq	r0,r0,r0

l000030AD:
	andeq	r0,r0,r0

l000030B1:
	andeq	r0,r0,r0

l000030B5:
	andeq	r0,r0,r0

l000030B9:
	andeq	r0,r0,r0

l000030BD:
	andeq	r0,r0,r0

l000030C1:
	andeq	r0,r0,r0

l000030C5:
	andeq	r0,r0,r0

l000030C9:
	andeq	r0,r0,r0

l000030CD:
	andeq	r0,r0,r0

l000030D1:
	andeq	r0,r0,r0

l000030D5:
	andeq	r0,r0,r0

l000030D9:
	andeq	r0,r0,r0

l000030DD:
	andeq	r0,r0,r0

l000030E1:
	andeq	r0,r0,r0

l000030E5:
	andeq	r0,r0,r0

l000030E9:
	andeq	r0,r0,r0

l000030ED:
	andeq	r0,r0,r0

l000030F1:
	andeq	r0,r0,r0

l000030F5:
	andeq	r0,r0,r0

l000030F9:
	andeq	r0,r0,r0

l000030FD:
	andeq	r0,r0,r0

l00003101:
	andeq	r0,r0,r0

l00003105:
	andeq	r0,r0,r0

l00003109:
	andeq	r0,r0,r0

l0000310D:
	andeq	r0,r0,r0

l00003111:
	andeq	r0,r0,r0

l00003115:
	andeq	r0,r0,r0

l00003119:
	andeq	r0,r0,r0

l0000311D:
	andeq	r0,r0,r0

l00003121:
	andeq	r0,r0,r0

l00003125:
	andeq	r0,r0,r0

l00003129:
	andeq	r0,r0,r0

l0000312D:
	andeq	r0,r0,r0

l00003131:
	andeq	r0,r0,r0

l00003135:
	andeq	r0,r0,r0

l00003139:
	andeq	r0,r0,r0

l0000313D:
	andeq	r0,r0,r0

l00003141:
	andeq	r0,r0,r0

l00003145:
	andeq	r0,r0,r0

l00003149:
	andeq	r0,r0,r0

l0000314D:
	andeq	r0,r0,r0

l00003151:
	andeq	r0,r0,r0

l00003155:
	andeq	r0,r0,r0

l00003159:
	andeq	r0,r0,r0

l0000315D:
	andeq	r0,r0,r0

l00003161:
	andeq	r0,r0,r0

l00003165:
	andeq	r0,r0,r0

l00003169:
	andeq	r0,r0,r0

l0000316D:
	andeq	r0,r0,r0

l00003171:
	andeq	r0,r0,r0

l00003175:
	andeq	r0,r0,r0

l00003179:
	andeq	r0,r0,r0

l0000317D:
	andeq	r0,r0,r0

l00003181:
	andeq	r0,r0,r0

l00003185:
	andeq	r0,r0,r0

l00003189:
	andeq	r0,r0,r0

l0000318D:
	andeq	r0,r0,r0

l00003191:
	andeq	r0,r0,r0

l00003195:
	andeq	r0,r0,r0

l00003199:
	andeq	r0,r0,r0

l0000319D:
	andeq	r0,r0,r0

l000031A1:
	andeq	r0,r0,r0

l000031A5:
	andeq	r0,r0,r0

l000031A9:
	andeq	r0,r0,r0

l000031AD:
	andeq	r0,r0,r0

l000031B1:
	andeq	r0,r0,r0

l000031B5:
	andeq	r0,r0,r0

l000031B9:
	andeq	r0,r0,r0

l000031BD:
	andeq	r0,r0,r0

l000031C1:
	andeq	r0,r0,r0

l000031C5:
	andeq	r0,r0,r0

l000031C9:
	andeq	r0,r0,r0

l000031CD:
	andeq	r0,r0,r0

l000031D1:
	andeq	r0,r0,r0

l000031D5:
	andeq	r0,r0,r0

l000031D9:
	andeq	r0,r0,r0

l000031DD:
	andeq	r0,r0,r0

l000031E1:
	andeq	r0,r0,r0

l000031E5:
	andeq	r0,r0,r0

l000031E9:
	andeq	r0,r0,r0

l000031ED:
	andeq	r0,r0,r0

l000031F1:
	andeq	r0,r0,r0

l000031F5:
	andeq	r0,r0,r0

l000031F9:
	andeq	r0,r0,r0

l000031FD:
	andeq	r0,r0,r0

l00003201:
	andeq	r0,r0,r0

l00003205:
	andeq	r0,r0,r0

l00003209:
	andeq	r0,r0,r0

l0000320D:
	andeq	r0,r0,r0

l00003211:
	andeq	r0,r0,r0

l00003215:
	andeq	r0,r0,r0

l00003219:
	andeq	r0,r0,r0

l0000321D:
	andeq	r0,r0,r0

l00003221:
	andeq	r0,r0,r0

l00003225:
	andeq	r0,r0,r0

l00003229:
	andeq	r0,r0,r0

l0000322D:
	andeq	r0,r0,r0

l00003231:
	andeq	r0,r0,r0

l00003235:
	andeq	r0,r0,r0

l00003239:
	andeq	r0,r0,r0

l0000323D:
	andeq	r0,r0,r0

l00003241:
	andeq	r0,r0,r0

l00003245:
	andeq	r0,r0,r0

l00003249:
	andeq	r0,r0,r0

l0000324D:
	andeq	r0,r0,r0

l00003251:
	andeq	r0,r0,r0

l00003255:
	andeq	r0,r0,r0

l00003259:
	andeq	r0,r0,r0

l0000325D:
	andeq	r0,r0,r0

l00003261:
	andeq	r0,r0,r0

l00003265:
	andeq	r0,r0,r0

l00003269:
	andeq	r0,r0,r0

l0000326D:
	andeq	r0,r0,r0

l00003271:
	andeq	r0,r0,r0

l00003275:
	andeq	r0,r0,r0

l00003279:
	andeq	r0,r0,r0

l0000327D:
	andeq	r0,r0,r0

l00003281:
	andeq	r0,r0,r0

l00003285:
	andeq	r0,r0,r0

l00003289:
	andeq	r0,r0,r0

l0000328D:
	andeq	r0,r0,r0

l00003291:
	andeq	r0,r0,r0

l00003295:
	andeq	r0,r0,r0

l00003299:
	andeq	r0,r0,r0

l0000329D:
	andeq	r0,r0,r0

l000032A1:
	andeq	r0,r0,r0

l000032A5:
	andeq	r0,r0,r0

l000032A9:
	andeq	r0,r0,r0

l000032AD:
	andeq	r0,r0,r0

l000032B1:
	andeq	r0,r0,r0

l000032B5:
	andeq	r0,r0,r0

l000032B9:
	andeq	r0,r0,r0

l000032BD:
	andeq	r0,r0,r0

l000032C1:
	andeq	r0,r0,r0

l000032C5:
	andeq	r0,r0,r0

l000032C9:
	andeq	r0,r0,r0

l000032CD:
	andeq	r0,r0,r0

l000032D1:
	andeq	r0,r0,r0

l000032D5:
	andeq	r0,r0,r0

l000032D9:
	andeq	r0,r0,r0

l000032DD:
	andeq	r0,r0,r0

l000032E1:
	andeq	r0,r0,r0

l000032E5:
	andeq	r0,r0,r0

l000032E9:
	andeq	r0,r0,r0

l000032ED:
	andeq	r0,r0,r0

l000032F1:
	andeq	r0,r0,r0

l000032F5:
	andeq	r0,r0,r0

l000032F9:
	andeq	r0,r0,r0

l000032FD:
	andeq	r0,r0,r0

l00003301:
	andeq	r0,r0,r0

l00003305:
	andeq	r0,r0,r0

l00003309:
	andeq	r0,r0,r0

l0000330D:
	andeq	r0,r0,r0

l00003311:
	andeq	r0,r0,r0

l00003315:
	andeq	r0,r0,r0

l00003319:
	andeq	r0,r0,r0

l0000331D:
	andeq	r0,r0,r0

l00003321:
	andeq	r0,r0,r0

l00003325:
	andeq	r0,r0,r0

l00003329:
	andeq	r0,r0,r0

l0000332D:
	andeq	r0,r0,r0

l00003331:
	andeq	r0,r0,r0

l00003335:
	andeq	r0,r0,r0

l00003339:
	andeq	r0,r0,r0

l0000333D:
	andeq	r0,r0,r0

l00003341:
	andeq	r0,r0,r0

l00003345:
	andeq	r0,r0,r0

l00003349:
	andeq	r0,r0,r0

l0000334D:
	andeq	r0,r0,r0

l00003351:
	andeq	r0,r0,r0

l00003355:
	andeq	r0,r0,r0

l00003359:
	andeq	r0,r0,r0

l0000335D:
	andeq	r0,r0,r0

l00003361:
	andeq	r0,r0,r0

l00003365:
	andeq	r0,r0,r0

l00003369:
	andeq	r0,r0,r0

l0000336D:
	andeq	r0,r0,r0

l00003371:
	andeq	r0,r0,r0

l00003375:
	andeq	r0,r0,r0

l00003379:
	andeq	r0,r0,r0

l0000337D:
	andeq	r0,r0,r0

l00003381:
	andeq	r0,r0,r0

l00003385:
	andeq	r0,r0,r0

l00003389:
	andeq	r0,r0,r0

l0000338D:
	andeq	r0,r0,r0

l00003391:
	andeq	r0,r0,r0

l00003395:
	andeq	r0,r0,r0

l00003399:
	andeq	r0,r0,r0

l0000339D:
	andeq	r0,r0,r0

l000033A1:
	andeq	r0,r0,r0

l000033A5:
	andeq	r0,r0,r0

l000033A9:
	andeq	r0,r0,r0

l000033AD:
	andeq	r0,r0,r0

l000033B1:
	andeq	r0,r0,r0

l000033B5:
	andeq	r0,r0,r0

l000033B9:
	andeq	r0,r0,r0

l000033BD:
	andeq	r0,r0,r0

l000033C1:
	andeq	r0,r0,r0

l000033C5:
	andeq	r0,r0,r0

l000033C9:
	andeq	r0,r0,r0

l000033CD:
	andeq	r0,r0,r0

l000033D1:
	andeq	r0,r0,r0

l000033D5:
	andeq	r0,r0,r0

l000033D9:
	andeq	r0,r0,r0

l000033DD:
	andeq	r0,r0,r0

l000033E1:
	andeq	r0,r0,r0

l000033E5:
	andeq	r0,r0,r0

l000033E9:
	andeq	r0,r0,r0

l000033ED:
	andeq	r0,r0,r0

l000033F1:
	andeq	r0,r0,r0

l000033F5:
	andeq	r0,r0,r0

l000033F9:
	andeq	r0,r0,r0

l000033FD:
	andeq	r0,r0,r0

l00003401:
	andeq	r0,r0,r0

l00003405:
	andeq	r0,r0,r0

l00003409:
	andeq	r0,r0,r0

l0000340D:
	andeq	r0,r0,r0

l00003411:
	andeq	r0,r0,r0

l00003415:
	andeq	r0,r0,r0

l00003419:
	andeq	r0,r0,r0

l0000341D:
	andeq	r0,r0,r0

l00003421:
	andeq	r0,r0,r0

l00003425:
	andeq	r0,r0,r0

l00003429:
	andeq	r0,r0,r0

l0000342D:
	andeq	r0,r0,r0

l00003431:
	andeq	r0,r0,r0

l00003435:
	andeq	r0,r0,r0

l00003439:
	andeq	r0,r0,r0

l0000343D:
	andeq	r0,r0,r0

l00003441:
	andeq	r0,r0,r0

l00003445:
	andeq	r0,r0,r0

l00003449:
	andeq	r0,r0,r0

l0000344D:
	andeq	r0,r0,r0

l00003451:
	andeq	r0,r0,r0

l00003455:
	andeq	r0,r0,r0

l00003459:
	andeq	r0,r0,r0

l0000345D:
	andeq	r0,r0,r0

l00003461:
	andeq	r0,r0,r0

l00003465:
	andeq	r0,r0,r0

l00003469:
	andeq	r0,r0,r0

l0000346D:
	andeq	r0,r0,r0

l00003471:
	andeq	r0,r0,r0

l00003475:
	andeq	r0,r0,r0

l00003479:
	andeq	r0,r0,r0

l0000347D:
	andeq	r0,r0,r0

l00003481:
	andeq	r0,r0,r0

l00003485:
	andeq	r0,r0,r0

l00003489:
	andeq	r0,r0,r0

l0000348D:
	andeq	r0,r0,r0

l00003491:
	andeq	r0,r0,r0

l00003495:
	andeq	r0,r0,r0

l00003499:
	andeq	r0,r0,r0

l0000349D:
	andeq	r0,r0,r0

l000034A1:
	andeq	r0,r0,r0

l000034A5:
	andeq	r0,r0,r0

l000034A9:
	andeq	r0,r0,r0

l000034AD:
	andeq	r0,r0,r0

l000034B1:
	andeq	r0,r0,r0

l000034B5:
	andeq	r0,r0,r0

l000034B9:
	andeq	r0,r0,r0

l000034BD:
	andeq	r0,r0,r0

l000034C1:
	andeq	r0,r0,r0

l000034C5:
	andeq	r0,r0,r0

l000034C9:
	andeq	r0,r0,r0

l000034CD:
	andeq	r0,r0,r0

l000034D1:
	andeq	r0,r0,r0

l000034D5:
	andeq	r0,r0,r0

l000034D9:
	andeq	r0,r0,r0

l000034DD:
	andeq	r0,r0,r0

l000034E1:
	andeq	r0,r0,r0

l000034E5:
	andeq	r0,r0,r0

l000034E9:
	andeq	r0,r0,r0

l000034ED:
	andeq	r0,r0,r0

l000034F1:
	andeq	r0,r0,r0

l000034F5:
	andeq	r0,r0,r0

l000034F9:
	andeq	r0,r0,r0

l000034FD:
	andeq	r0,r0,r0

l00003501:
	andeq	r0,r0,r0

l00003505:
	andeq	r0,r0,r0

l00003509:
	andeq	r0,r0,r0

l0000350D:
	andeq	r0,r0,r0

l00003511:
	andeq	r0,r0,r0

l00003515:
	andeq	r0,r0,r0

l00003519:
	andeq	r0,r0,r0

l0000351D:
	andeq	r0,r0,r0

l00003521:
	andeq	r0,r0,r0

l00003525:
	andeq	r0,r0,r0

l00003529:
	andeq	r0,r0,r0

l0000352D:
	andeq	r0,r0,r0

l00003531:
	andeq	r0,r0,r0

l00003535:
	andeq	r0,r0,r0

l00003539:
	andeq	r0,r0,r0

l0000353D:
	andeq	r0,r0,r0

l00003541:
	andeq	r0,r0,r0

l00003545:
	andeq	r0,r0,r0

l00003549:
	andeq	r0,r0,r0

l0000354D:
	andeq	r0,r0,r0

l00003551:
	andeq	r0,r0,r0

l00003555:
	andeq	r0,r0,r0

l00003559:
	andeq	r0,r0,r0

l0000355D:
	andeq	r0,r0,r0

l00003561:
	andeq	r0,r0,r0

l00003565:
	andeq	r0,r0,r0

l00003569:
	andeq	r0,r0,r0

l0000356D:
	andeq	r0,r0,r0

l00003571:
	andeq	r0,r0,r0

l00003575:
	andeq	r0,r0,r0

l00003579:
	andeq	r0,r0,r0

l0000357D:
	andeq	r0,r0,r0

l00003581:
	andeq	r0,r0,r0

l00003585:
	andeq	r0,r0,r0

l00003589:
	andeq	r0,r0,r0

l0000358D:
	andeq	r0,r0,r0

l00003591:
	andeq	r0,r0,r0

l00003595:
	andeq	r0,r0,r0

l00003599:
	andeq	r0,r0,r0

l0000359D:
	andeq	r0,r0,r0

l000035A1:
	andeq	r0,r0,r0

l000035A5:
	andeq	r0,r0,r0

l000035A9:
	andeq	r0,r0,r0

l000035AD:
	andeq	r0,r0,r0

l000035B1:
	andeq	r0,r0,r0

l000035B5:
	andeq	r0,r0,r0

l000035B9:
	andeq	r0,r0,r0

l000035BD:
	andeq	r0,r0,r0

l000035C1:
	andeq	r0,r0,r0

l000035C5:
	andeq	r0,r0,r0

l000035C9:
	andeq	r0,r0,r0

l000035CD:
	andeq	r0,r0,r0

l000035D1:
	andeq	r0,r0,r0

l000035D5:
	andeq	r0,r0,r0

l000035D9:
	andeq	r0,r0,r0

l000035DD:
	andeq	r0,r0,r0

l000035E1:
	andeq	r0,r0,r0

l000035E5:
	andeq	r0,r0,r0

l000035E9:
	andeq	r0,r0,r0

l000035ED:
	andeq	r0,r0,r0

l000035F1:
	andeq	r0,r0,r0

l000035F5:
	andeq	r0,r0,r0

l000035F9:
	andeq	r0,r0,r0

l000035FD:
	andeq	r0,r0,r0

l00003601:
	andeq	r0,r0,r0

l00003605:
	andeq	r0,r0,r0

l00003609:
	andeq	r0,r0,r0

l0000360D:
	andeq	r0,r0,r0

l00003611:
	andeq	r0,r0,r0

l00003615:
	andeq	r0,r0,r0

l00003619:
	andeq	r0,r0,r0

l0000361D:
	andeq	r0,r0,r0

l00003621:
	andeq	r0,r0,r0

l00003625:
	andeq	r0,r0,r0

l00003629:
	andeq	r0,r0,r0

l0000362D:
	andeq	r0,r0,r0

l00003631:
	andeq	r0,r0,r0

l00003635:
	andeq	r0,r0,r0

l00003639:
	andeq	r0,r0,r0

l0000363D:
	andeq	r0,r0,r0

l00003641:
	andeq	r0,r0,r0

l00003645:
	andeq	r0,r0,r0

l00003649:
	andeq	r0,r0,r0

l0000364D:
	andeq	r0,r0,r0

l00003651:
	andeq	r0,r0,r0

l00003655:
	andeq	r0,r0,r0

l00003659:
	andeq	r0,r0,r0

l0000365D:
	andeq	r0,r0,r0

l00003661:
	andeq	r0,r0,r0

l00003665:
	andeq	r0,r0,r0

l00003669:
	andeq	r0,r0,r0

l0000366D:
	andeq	r0,r0,r0

l00003671:
	andeq	r0,r0,r0

l00003675:
	andeq	r0,r0,r0

l00003679:
	andeq	r0,r0,r0

l0000367D:
	andeq	r0,r0,r0

l00003681:
	andeq	r0,r0,r0

l00003685:
	andeq	r0,r0,r0

l00003689:
	andeq	r0,r0,r0

l0000368D:
	andeq	r0,r0,r0

l00003691:
	andeq	r0,r0,r0

l00003695:
	andeq	r0,r0,r0

l00003699:
	andeq	r0,r0,r0

l0000369D:
	andeq	r0,r0,r0

l000036A1:
	andeq	r0,r0,r0

l000036A5:
	andeq	r0,r0,r0

l000036A9:
	andeq	r0,r0,r0

l000036AD:
	andeq	r0,r0,r0

l000036B1:
	andeq	r0,r0,r0

l000036B5:
	andeq	r0,r0,r0

l000036B9:
	andeq	r0,r0,r0

l000036BD:
	andeq	r0,r0,r0

l000036C1:
	andeq	r0,r0,r0

l000036C5:
	andeq	r0,r0,r0

l000036C9:
	andeq	r0,r0,r0

l000036CD:
	andeq	r0,r0,r0

l000036D1:
	andeq	r0,r0,r0

l000036D5:
	andeq	r0,r0,r0

l000036D9:
	andeq	r0,r0,r0

l000036DD:
	andeq	r0,r0,r0

l000036E1:
	andeq	r0,r0,r0

l000036E5:
	andeq	r0,r0,r0

l000036E9:
	andeq	r0,r0,r0

l000036ED:
	andeq	r0,r0,r0

l000036F1:
	andeq	r0,r0,r0

l000036F5:
	andeq	r0,r0,r0

l000036F9:
	andeq	r0,r0,r0

l000036FD:
	andeq	r0,r0,r0

l00003701:
	andeq	r0,r0,r0

l00003705:
	andeq	r0,r0,r0

l00003709:
	andeq	r0,r0,r0

l0000370D:
	andeq	r0,r0,r0

l00003711:
	andeq	r0,r0,r0

l00003715:
	andeq	r0,r0,r0

l00003719:
	andeq	r0,r0,r0

l0000371D:
	andeq	r0,r0,r0

l00003721:
	andeq	r0,r0,r0

l00003725:
	andeq	r0,r0,r0

l00003729:
	andeq	r0,r0,r0

l0000372D:
	andeq	r0,r0,r0

l00003731:
	andeq	r0,r0,r0

l00003735:
	andeq	r0,r0,r0

l00003739:
	andeq	r0,r0,r0

l0000373D:
	andeq	r0,r0,r0

l00003741:
	andeq	r0,r0,r0

l00003745:
	andeq	r0,r0,r0

l00003749:
	andeq	r0,r0,r0

l0000374D:
	andeq	r0,r0,r0

l00003751:
	andeq	r0,r0,r0

l00003755:
	andeq	r0,r0,r0

l00003759:
	andeq	r0,r0,r0

l0000375D:
	andeq	r0,r0,r0

l00003761:
	andeq	r0,r0,r0

l00003765:
	andeq	r0,r0,r0

l00003769:
	andeq	r0,r0,r0

l0000376D:
	andeq	r0,r0,r0

l00003771:
	andeq	r0,r0,r0

l00003775:
	andeq	r0,r0,r0

l00003779:
	andeq	r0,r0,r0

l0000377D:
	andeq	r0,r0,r0

l00003781:
	andeq	r0,r0,r0

l00003785:
	andeq	r0,r0,r0

l00003789:
	andeq	r0,r0,r0

l0000378D:
	andeq	r0,r0,r0

l00003791:
	andeq	r0,r0,r0

l00003795:
	andeq	r0,r0,r0

l00003799:
	andeq	r0,r0,r0

l0000379D:
	andeq	r0,r0,r0

l000037A1:
	andeq	r0,r0,r0

l000037A5:
	andeq	r0,r0,r0

l000037A9:
	andeq	r0,r0,r0

l000037AD:
	andeq	r0,r0,r0

l000037B1:
	andeq	r0,r0,r0

l000037B5:
	andeq	r0,r0,r0

l000037B9:
	andeq	r0,r0,r0

l000037BD:
	andeq	r0,r0,r0

l000037C1:
	andeq	r0,r0,r0

l000037C5:
	andeq	r0,r0,r0

l000037C9:
	andeq	r0,r0,r0

l000037CD:
	andeq	r0,r0,r0

l000037D1:
	andeq	r0,r0,r0

l000037D5:
	andeq	r0,r0,r0

l000037D9:
	andeq	r0,r0,r0

l000037DD:
	andeq	r0,r0,r0

l000037E1:
	andeq	r0,r0,r0

l000037E5:
	andeq	r0,r0,r0

l000037E9:
	andeq	r0,r0,r0

l000037ED:
	andeq	r0,r0,r0

l000037F1:
	andeq	r0,r0,r0

l000037F5:
	andeq	r0,r0,r0

l000037F9:
	andeq	r0,r0,r0

l000037FD:
	andeq	r0,r0,r0

l00003801:
	andeq	r0,r0,r0

l00003805:
	andeq	r0,r0,r0

l00003809:
	andeq	r0,r0,r0

l0000380D:
	andeq	r0,r0,r0

l00003811:
	andeq	r0,r0,r0

l00003815:
	andeq	r0,r0,r0

l00003819:
	andeq	r0,r0,r0

l0000381D:
	andeq	r0,r0,r0

l00003821:
	andeq	r0,r0,r0

l00003825:
	andeq	r0,r0,r0

l00003829:
	andeq	r0,r0,r0

l0000382D:
	andeq	r0,r0,r0

l00003831:
	andeq	r0,r0,r0

l00003835:
	andeq	r0,r0,r0

l00003839:
	andeq	r0,r0,r0

l0000383D:
	andeq	r0,r0,r0

l00003841:
	andeq	r0,r0,r0

l00003845:
	andeq	r0,r0,r0

l00003849:
	andeq	r0,r0,r0

l0000384D:
	andeq	r0,r0,r0

l00003851:
	andeq	r0,r0,r0

l00003855:
	andeq	r0,r0,r0

l00003859:
	andeq	r0,r0,r0

l0000385D:
	andeq	r0,r0,r0

l00003861:
	andeq	r0,r0,r0

l00003865:
	andeq	r0,r0,r0

l00003869:
	andeq	r0,r0,r0

l0000386D:
	andeq	r0,r0,r0

l00003871:
	andeq	r0,r0,r0

l00003875:
	andeq	r0,r0,r0

l00003879:
	andeq	r0,r0,r0

l0000387D:
	andeq	r0,r0,r0

l00003881:
	andeq	r0,r0,r0

l00003885:
	andeq	r0,r0,r0

l00003889:
	andeq	r0,r0,r0

l0000388D:
	andeq	r0,r0,r0

l00003891:
	andeq	r0,r0,r0

l00003895:
	andeq	r0,r0,r0

l00003899:
	andeq	r0,r0,r0

l0000389D:
	andeq	r0,r0,r0

l000038A1:
	andeq	r0,r0,r0

l000038A5:
	andeq	r0,r0,r0

l000038A9:
	andeq	r0,r0,r0

l000038AD:
	andeq	r0,r0,r0

l000038B1:
	andeq	r0,r0,r0

l000038B5:
	andeq	r0,r0,r0

l000038B9:
	andeq	r0,r0,r0

l000038BD:
	andeq	r0,r0,r0

l000038C1:
	andeq	r0,r0,r0

l000038C5:
	andeq	r0,r0,r0

l000038C9:
	andeq	r0,r0,r0

l000038CD:
	andeq	r0,r0,r0

l000038D1:
	andeq	r0,r0,r0

l000038D5:
	andeq	r0,r0,r0

l000038D9:
	andeq	r0,r0,r0

l000038DD:
	andeq	r0,r0,r0

l000038E1:
	andeq	r0,r0,r0

l000038E5:
	andeq	r0,r0,r0

l000038E9:
	andeq	r0,r0,r0

l000038ED:
	andeq	r0,r0,r0

l000038F1:
	andeq	r0,r0,r0

l000038F5:
	andeq	r0,r0,r0

l000038F9:
	andeq	r0,r0,r0

l000038FD:
	andeq	r0,r0,r0

l00003901:
	andeq	r0,r0,r0

l00003905:
	andeq	r0,r0,r0

l00003909:
	andeq	r0,r0,r0

l0000390D:
	andeq	r0,r0,r0

l00003911:
	andeq	r0,r0,r0

l00003915:
	andeq	r0,r0,r0

l00003919:
	andeq	r0,r0,r0

l0000391D:
	andeq	r0,r0,r0

l00003921:
	andeq	r0,r0,r0

l00003925:
	andeq	r0,r0,r0

l00003929:
	andeq	r0,r0,r0

l0000392D:
	andeq	r0,r0,r0

l00003931:
	andeq	r0,r0,r0

l00003935:
	andeq	r0,r0,r0

l00003939:
	andeq	r0,r0,r0

l0000393D:
	andeq	r0,r0,r0

l00003941:
	andeq	r0,r0,r0

l00003945:
	andeq	r0,r0,r0

l00003949:
	andeq	r0,r0,r0

l0000394D:
	andeq	r0,r0,r0

l00003951:
	andeq	r0,r0,r0

l00003955:
	andeq	r0,r0,r0

l00003959:
	andeq	r0,r0,r0

l0000395D:
	andeq	r0,r0,r0

l00003961:
	andeq	r0,r0,r0

l00003965:
	andeq	r0,r0,r0

l00003969:
	andeq	r0,r0,r0

l0000396D:
	andeq	r0,r0,r0

l00003971:
	andeq	r0,r0,r0

l00003975:
	andeq	r0,r0,r0

l00003979:
	andeq	r0,r0,r0

l0000397D:
	andeq	r0,r0,r0

l00003981:
	andeq	r0,r0,r0

l00003985:
	andeq	r0,r0,r0

l00003989:
	andeq	r0,r0,r0

l0000398D:
	andeq	r0,r0,r0

l00003991:
	andeq	r0,r0,r0

l00003995:
	andeq	r0,r0,r0

l00003999:
	andeq	r0,r0,r0

l0000399D:
	andeq	r0,r0,r0

l000039A1:
	andeq	r0,r0,r0

l000039A5:
	andeq	r0,r0,r0

l000039A9:
	andeq	r0,r0,r0

l000039AD:
	andeq	r0,r0,r0

l000039B1:
	andeq	r0,r0,r0

l000039B5:
	andeq	r0,r0,r0

l000039B9:
	andeq	r0,r0,r0

l000039BD:
	andeq	r0,r0,r0

l000039C1:
	andeq	r0,r0,r0

l000039C5:
	andeq	r0,r0,r0

l000039C9:
	andeq	r0,r0,r0

l000039CD:
	andeq	r0,r0,r0

l000039D1:
	andeq	r0,r0,r0

l000039D5:
	andeq	r0,r0,r0

l000039D9:
	andeq	r0,r0,r0

l000039DD:
	andeq	r0,r0,r0

l000039E1:
	andeq	r0,r0,r0

l000039E5:
	andeq	r0,r0,r0

l000039E9:
	andeq	r0,r0,r0

l000039ED:
	andeq	r0,r0,r0

l000039F1:
	andeq	r0,r0,r0

l000039F5:
	andeq	r0,r0,r0

l000039F9:
	andeq	r0,r0,r0

l000039FD:
	andeq	r0,r0,r0

l00003A01:
	andeq	r0,r0,r0

l00003A05:
	andeq	r0,r0,r0

l00003A09:
	andeq	r0,r0,r0

l00003A0D:
	andeq	r0,r0,r0

l00003A11:
	andeq	r0,r0,r0

l00003A15:
	andeq	r0,r0,r0

l00003A19:
	andeq	r0,r0,r0

l00003A1D:
	andeq	r0,r0,r0

l00003A21:
	andeq	r0,r0,r0

l00003A25:
	andeq	r0,r0,r0

l00003A29:
	andeq	r0,r0,r0

l00003A2D:
	andeq	r0,r0,r0

l00003A31:
	andeq	r0,r0,r0

l00003A35:
	andeq	r0,r0,r0

l00003A39:
	andeq	r0,r0,r0

l00003A3D:
	andeq	r0,r0,r0

l00003A41:
	andeq	r0,r0,r0

l00003A45:
	andeq	r0,r0,r0

l00003A49:
	andeq	r0,r0,r0

l00003A4D:
	andeq	r0,r0,r0

l00003A51:
	andeq	r0,r0,r0

l00003A55:
	andeq	r0,r0,r0

l00003A59:
	andeq	r0,r0,r0

l00003A5D:
	andeq	r0,r0,r0

l00003A61:
	andeq	r0,r0,r0

l00003A65:
	andeq	r0,r0,r0

l00003A69:
	andeq	r0,r0,r0

l00003A6D:
	andeq	r0,r0,r0

l00003A71:
	andeq	r0,r0,r0

l00003A75:
	andeq	r0,r0,r0

l00003A79:
	andeq	r0,r0,r0

l00003A7D:
	andeq	r0,r0,r0

l00003A81:
	andeq	r0,r0,r0

l00003A85:
	andeq	r0,r0,r0

l00003A89:
	andeq	r0,r0,r0

l00003A8D:
	andeq	r0,r0,r0

l00003A91:
	andeq	r0,r0,r0

l00003A95:
	andeq	r0,r0,r0

l00003A99:
	andeq	r0,r0,r0

l00003A9D:
	andeq	r0,r0,r0

l00003AA1:
	andeq	r0,r0,r0

l00003AA5:
	andeq	r0,r0,r0

l00003AA9:
	andeq	r0,r0,r0

l00003AAD:
	andeq	r0,r0,r0

l00003AB1:
	andeq	r0,r0,r0

l00003AB5:
	andeq	r0,r0,r0

l00003AB9:
	andeq	r0,r0,r0

l00003ABD:
	andeq	r0,r0,r0

l00003AC1:
	andeq	r0,r0,r0

l00003AC5:
	andeq	r0,r0,r0

l00003AC9:
	andeq	r0,r0,r0

l00003ACD:
	andeq	r0,r0,r0

l00003AD1:
	andeq	r0,r0,r0

l00003AD5:
	andeq	r0,r0,r0

l00003AD9:
	andeq	r0,r0,r0

l00003ADD:
	andeq	r0,r0,r0

l00003AE1:
	andeq	r0,r0,r0

l00003AE5:
	andeq	r0,r0,r0

l00003AE9:
	andeq	r0,r0,r0

l00003AED:
	andeq	r0,r0,r0

l00003AF1:
	andeq	r0,r0,r0

l00003AF5:
	andeq	r0,r0,r0

l00003AF9:
	andeq	r0,r0,r0

l00003AFD:
	andeq	r0,r0,r0

l00003B01:
	andeq	r0,r0,r0

l00003B05:
	andeq	r0,r0,r0

l00003B09:
	andeq	r0,r0,r0

l00003B0D:
	andeq	r0,r0,r0

l00003B11:
	andeq	r0,r0,r0

l00003B15:
	andeq	r0,r0,r0

l00003B19:
	andeq	r0,r0,r0

l00003B1D:
	andeq	r0,r0,r0

l00003B21:
	andeq	r0,r0,r0

l00003B25:
	andeq	r0,r0,r0

l00003B29:
	andeq	r0,r0,r0

l00003B2D:
	andeq	r0,r0,r0

l00003B31:
	andeq	r0,r0,r0

l00003B35:
	andeq	r0,r0,r0

l00003B39:
	andeq	r0,r0,r0

l00003B3D:
	andeq	r0,r0,r0

l00003B41:
	andeq	r0,r0,r0

l00003B45:
	andeq	r0,r0,r0

l00003B49:
	andeq	r0,r0,r0

l00003B4D:
	andeq	r0,r0,r0

l00003B51:
	andeq	r0,r0,r0

l00003B55:
	andeq	r0,r0,r0

l00003B59:
	andeq	r0,r0,r0

l00003B5D:
	andeq	r0,r0,r0

l00003B61:
	andeq	r0,r0,r0

l00003B65:
	andeq	r0,r0,r0

l00003B69:
	andeq	r0,r0,r0

l00003B6D:
	andeq	r0,r0,r0

l00003B71:
	andeq	r0,r0,r0

l00003B75:
	andeq	r0,r0,r0

l00003B79:
	andeq	r0,r0,r0

l00003B7D:
	andeq	r0,r0,r0

l00003B81:
	andeq	r0,r0,r0

l00003B85:
	andeq	r0,r0,r0

l00003B89:
	andeq	r0,r0,r0

l00003B8D:
	andeq	r0,r0,r0

l00003B91:
	andeq	r0,r0,r0

l00003B95:
	andeq	r0,r0,r0

l00003B99:
	andeq	r0,r0,r0

l00003B9D:
	andeq	r0,r0,r0

l00003BA1:
	andeq	r0,r0,r0

l00003BA5:
	andeq	r0,r0,r0

l00003BA9:
	andeq	r0,r0,r0

l00003BAD:
	andeq	r0,r0,r0

l00003BB1:
	andeq	r0,r0,r0

l00003BB5:
	andeq	r0,r0,r0

l00003BB9:
	andeq	r0,r0,r0

l00003BBD:
	andeq	r0,r0,r0

l00003BC1:
	andeq	r0,r0,r0

l00003BC5:
	andeq	r0,r0,r0

l00003BC9:
	andeq	r0,r0,r0

l00003BCD:
	andeq	r0,r0,r0

l00003BD1:
	andeq	r0,r0,r0

l00003BD5:
	andeq	r0,r0,r0

l00003BD9:
	andeq	r0,r0,r0

l00003BDD:
	andeq	r0,r0,r0

l00003BE1:
	andeq	r0,r0,r0

l00003BE5:
	andeq	r0,r0,r0

l00003BE9:
	andeq	r0,r0,r0

l00003BED:
	andeq	r0,r0,r0

l00003BF1:
	andeq	r0,r0,r0

l00003BF5:
	andeq	r0,r0,r0

l00003BF9:
	andeq	r0,r0,r0

l00003BFD:
	andeq	r0,r0,r0

l00003C01:
	andeq	r0,r0,r0

l00003C05:
	andeq	r0,r0,r0

l00003C09:
	andeq	r0,r0,r0

l00003C0D:
	andeq	r0,r0,r0

l00003C11:
	andeq	r0,r0,r0

l00003C15:
	andeq	r0,r0,r0

l00003C19:
	andeq	r0,r0,r0

l00003C1D:
	andeq	r0,r0,r0

l00003C21:
	andeq	r0,r0,r0

l00003C25:
	andeq	r0,r0,r0

l00003C29:
	andeq	r0,r0,r0

l00003C2D:
	andeq	r0,r0,r0

l00003C31:
	andeq	r0,r0,r0

l00003C35:
	andeq	r0,r0,r0

l00003C39:
	andeq	r0,r0,r0

l00003C3D:
	andeq	r0,r0,r0

l00003C41:
	andeq	r0,r0,r0

l00003C45:
	andeq	r0,r0,r0

l00003C49:
	andeq	r0,r0,r0

l00003C4D:
	andeq	r0,r0,r0

l00003C51:
	andeq	r0,r0,r0

l00003C55:
	andeq	r0,r0,r0

l00003C59:
	andeq	r0,r0,r0

l00003C5D:
	andeq	r0,r0,r0

l00003C61:
	andeq	r0,r0,r0

l00003C65:
	andeq	r0,r0,r0

l00003C69:
	andeq	r0,r0,r0

l00003C6D:
	andeq	r0,r0,r0

l00003C71:
	andeq	r0,r0,r0

l00003C75:
	andeq	r0,r0,r0

l00003C79:
	andeq	r0,r0,r0

l00003C7D:
	andeq	r0,r0,r0

l00003C81:
	andeq	r0,r0,r0

l00003C85:
	andeq	r0,r0,r0

l00003C89:
	andeq	r0,r0,r0

l00003C8D:
	andeq	r0,r0,r0

l00003C91:
	andeq	r0,r0,r0

l00003C95:
	andeq	r0,r0,r0

l00003C99:
	andeq	r0,r0,r0

l00003C9D:
	andeq	r0,r0,r0

l00003CA1:
	andeq	r0,r0,r0

l00003CA5:
	andeq	r0,r0,r0

l00003CA9:
	andeq	r0,r0,r0

l00003CAD:
	andeq	r0,r0,r0

l00003CB1:
	andeq	r0,r0,r0

l00003CB5:
	andeq	r0,r0,r0

l00003CB9:
	andeq	r0,r0,r0

l00003CBD:
	andeq	r0,r0,r0

l00003CC1:
	andeq	r0,r0,r0

l00003CC5:
	andeq	r0,r0,r0

l00003CC9:
	andeq	r0,r0,r0

l00003CCD:
	andeq	r0,r0,r0

l00003CD1:
	andeq	r0,r0,r0

l00003CD5:
	andeq	r0,r0,r0

l00003CD9:
	andeq	r0,r0,r0

l00003CDD:
	andeq	r0,r0,r0

l00003CE1:
	andeq	r0,r0,r0

l00003CE5:
	andeq	r0,r0,r0

l00003CE9:
	andeq	r0,r0,r0

l00003CED:
	andeq	r0,r0,r0

l00003CF1:
	andeq	r0,r0,r0

l00003CF5:
	andeq	r0,r0,r0

l00003CF9:
	andeq	r0,r0,r0

l00003CFD:
	andeq	r0,r0,r0

l00003D01:
	andeq	r0,r0,r0

l00003D05:
	andeq	r0,r0,r0

l00003D09:
	andeq	r0,r0,r0

l00003D0D:
	andeq	r0,r0,r0

l00003D11:
	andeq	r0,r0,r0

l00003D15:
	andeq	r0,r0,r0

l00003D19:
	andeq	r0,r0,r0

l00003D1D:
	andeq	r0,r0,r0

l00003D21:
	andeq	r0,r0,r0

l00003D25:
	andeq	r0,r0,r0

l00003D29:
	andeq	r0,r0,r0

l00003D2D:
	andeq	r0,r0,r0

l00003D31:
	andeq	r0,r0,r0

l00003D35:
	andeq	r0,r0,r0

l00003D39:
	andeq	r0,r0,r0

l00003D3D:
	andeq	r0,r0,r0

l00003D41:
	andeq	r0,r0,r0

l00003D45:
	andeq	r0,r0,r0

l00003D49:
	andeq	r0,r0,r0

l00003D4D:
	andeq	r0,r0,r0

l00003D51:
	andeq	r0,r0,r0

l00003D55:
	andeq	r0,r0,r0

l00003D59:
	andeq	r0,r0,r0

l00003D5D:
	andeq	r0,r0,r0

l00003D61:
	andeq	r0,r0,r0

l00003D65:
	andeq	r0,r0,r0

l00003D69:
	andeq	r0,r0,r0

l00003D6D:
	andeq	r0,r0,r0

l00003D71:
	andeq	r0,r0,r0

l00003D75:
	andeq	r0,r0,r0

l00003D79:
	andeq	r0,r0,r0

l00003D7D:
	andeq	r0,r0,r0

l00003D81:
	andeq	r0,r0,r0

l00003D85:
	andeq	r0,r0,r0

l00003D89:
	andeq	r0,r0,r0

l00003D8D:
	andeq	r0,r0,r0

l00003D91:
	andeq	r0,r0,r0

l00003D95:
	andeq	r0,r0,r0

l00003D99:
	andeq	r0,r0,r0

l00003D9D:
	andeq	r0,r0,r0

l00003DA1:
	andeq	r0,r0,r0

l00003DA5:
	andeq	r0,r0,r0

l00003DA9:
	andeq	r0,r0,r0

l00003DAD:
	andeq	r0,r0,r0

l00003DB1:
	andeq	r0,r0,r0

l00003DB5:
	andeq	r0,r0,r0

l00003DB9:
	andeq	r0,r0,r0

l00003DBD:
	andeq	r0,r0,r0

l00003DC1:
	andeq	r0,r0,r0

l00003DC5:
	andeq	r0,r0,r0

l00003DC9:
	andeq	r0,r0,r0

l00003DCD:
	andeq	r0,r0,r0

l00003DD1:
	andeq	r0,r0,r0

l00003DD5:
	andeq	r0,r0,r0

l00003DD9:
	andeq	r0,r0,r0

l00003DDD:
	andeq	r0,r0,r0

l00003DE1:
	andeq	r0,r0,r0

l00003DE5:
	andeq	r0,r0,r0

l00003DE9:
	andeq	r0,r0,r0

l00003DED:
	andeq	r0,r0,r0

l00003DF1:
	andeq	r0,r0,r0

l00003DF5:
	andeq	r0,r0,r0

l00003DF9:
	andeq	r0,r0,r0

l00003DFD:
	andeq	r0,r0,r0

l00003E01:
	andeq	r0,r0,r0

l00003E05:
	andeq	r0,r0,r0

l00003E09:
	andeq	r0,r0,r0

l00003E0D:
	andeq	r0,r0,r0

l00003E11:
	andeq	r0,r0,r0

l00003E15:
	andeq	r0,r0,r0

l00003E19:
	andeq	r0,r0,r0

l00003E1D:
	andeq	r0,r0,r0

l00003E21:
	andeq	r0,r0,r0

l00003E25:
	andeq	r0,r0,r0

l00003E29:
	andeq	r0,r0,r0

l00003E2D:
	andeq	r0,r0,r0

l00003E31:
	andeq	r0,r0,r0

l00003E35:
	andeq	r0,r0,r0

l00003E39:
	andeq	r0,r0,r0

l00003E3D:
	andeq	r0,r0,r0

l00003E41:
	andeq	r0,r0,r0

l00003E45:
	andeq	r0,r0,r0

l00003E49:
	andeq	r0,r0,r0

l00003E4D:
	andeq	r0,r0,r0

l00003E51:
	andeq	r0,r0,r0

l00003E55:
	andeq	r0,r0,r0

l00003E59:
	andeq	r0,r0,r0

l00003E5D:
	andeq	r0,r0,r0

l00003E61:
	andeq	r0,r0,r0

l00003E65:
	andeq	r0,r0,r0

l00003E69:
	andeq	r0,r0,r0

l00003E6D:
	andeq	r0,r0,r0

l00003E71:
	andeq	r0,r0,r0

l00003E75:
	andeq	r0,r0,r0

l00003E79:
	andeq	r0,r0,r0

l00003E7D:
	andeq	r0,r0,r0

l00003E81:
	andeq	r0,r0,r0

l00003E85:
	andeq	r0,r0,r0

l00003E89:
	andeq	r0,r0,r0

l00003E8D:
	andeq	r0,r0,r0

l00003E91:
	andeq	r0,r0,r0

l00003E95:
	andeq	r0,r0,r0

l00003E99:
	andeq	r0,r0,r0

l00003E9D:
	andeq	r0,r0,r0

l00003EA1:
	andeq	r0,r0,r0

l00003EA5:
	andeq	r0,r0,r0

l00003EA9:
	andeq	r0,r0,r0

l00003EAD:
	andeq	r0,r0,r0

l00003EB1:
	andeq	r0,r0,r0

l00003EB5:
	andeq	r0,r0,r0

l00003EB9:
	andeq	r0,r0,r0

l00003EBD:
	andeq	r0,r0,r0

l00003EC1:
	andeq	r0,r0,r0

l00003EC5:
	andeq	r0,r0,r0

l00003EC9:
	andeq	r0,r0,r0

l00003ECD:
	andeq	r0,r0,r0

l00003ED1:
	andeq	r0,r0,r0

l00003ED5:
	andeq	r0,r0,r0

l00003ED9:
	andeq	r0,r0,r0

l00003EDD:
	andeq	r0,r0,r0

l00003EE1:
	andeq	r0,r0,r0

l00003EE5:
	andeq	r0,r0,r0

l00003EE9:
	andeq	r0,r0,r0

l00003EED:
	andeq	r0,r0,r0

l00003EF1:
	andeq	r0,r0,r0

l00003EF5:
	andeq	r0,r0,r0

l00003EF9:
	andeq	r0,r0,r0

l00003EFD:
	andeq	r0,r0,r0

l00003F01:
	andeq	r0,r0,r0

l00003F05:
	andeq	r0,r0,r0

l00003F09:
	andeq	r0,r0,r0

l00003F0D:
	andeq	r0,r0,r0

l00003F11:
	andeq	r0,r0,r0

l00003F15:
	andeq	r0,r0,r0

l00003F19:
	andeq	r0,r0,r0

l00003F1D:
	andeq	r0,r0,r0

l00003F21:
	andeq	r0,r0,r0

l00003F25:
	andeq	r0,r0,r0

l00003F29:
	andeq	r0,r0,r0

l00003F2D:
	andeq	r0,r0,r0

l00003F31:
	andeq	r0,r0,r0

l00003F35:
	andeq	r0,r0,r0

l00003F39:
	andeq	r0,r0,r0

l00003F3D:
	andeq	r0,r0,r0

l00003F41:
	andeq	r0,r0,r0

l00003F45:
	andeq	r0,r0,r0

l00003F49:
	andeq	r0,r0,r0

l00003F4D:
	andeq	r0,r0,r0

l00003F51:
	andeq	r0,r0,r0

l00003F55:
	andeq	r0,r0,r0

l00003F59:
	andeq	r0,r0,r0

l00003F5D:
	andeq	r0,r0,r0

l00003F61:
	andeq	r0,r0,r0

l00003F65:
	andeq	r0,r0,r0

l00003F69:
	andeq	r0,r0,r0

l00003F6D:
	andeq	r0,r0,r0

l00003F71:
	andeq	r0,r0,r0

l00003F75:
	andeq	r0,r0,r0

l00003F79:
	andeq	r0,r0,r0

l00003F7D:
	andeq	r0,r0,r0

l00003F81:
	andeq	r0,r0,r0

l00003F85:
	andeq	r0,r0,r0

l00003F89:
	andeq	r0,r0,r0

l00003F8D:
	andeq	r0,r0,r0

l00003F91:
	andeq	r0,r0,r0

l00003F95:
	andeq	r0,r0,r0

l00003F99:
	andeq	r0,r0,r0

l00003F9D:
	andeq	r0,r0,r0

l00003FA1:
	andeq	r0,r0,r0

l00003FA5:
	andeq	r0,r0,r0

l00003FA9:
	andeq	r0,r0,r0

l00003FAD:
	andeq	r0,r0,r0

l00003FB1:
	andeq	r0,r0,r0

l00003FB5:
	andeq	r0,r0,r0

l00003FB9:
	andeq	r0,r0,r0

l00003FBD:
	andeq	r0,r0,r0

l00003FC1:
	andeq	r0,r0,r0

l00003FC5:
	andeq	r0,r0,r0

l00003FC9:
	andeq	r0,r0,r0

l00003FCD:
	andeq	r0,r0,r0

l00003FD1:
	andeq	r0,r0,r0

l00003FD5:
	andeq	r0,r0,r0

l00003FD9:
	andeq	r0,r0,r0

l00003FDD:
	andeq	r0,r0,r0

l00003FE1:
	andeq	r0,r0,r0

l00003FE5:
	andeq	r0,r0,r0

l00003FE9:
	andeq	r0,r0,r0

l00003FED:
	andeq	r0,r0,r0

l00003FF1:
	andeq	r0,r0,r0

l00003FF5:
	andeq	r0,r0,r0

l00003FF9:
	andeq	r0,r0,r0

l00003FFD:
	andeq	r0,r0,r0

l00004001:
	andeq	r0,r0,r0

l00004005:
	andeq	r0,r0,r0

l00004009:
	andeq	r0,r0,r0

l0000400D:
	andeq	r0,r0,r0

l00004011:
	andeq	r0,r0,r0

l00004015:
	andeq	r0,r0,r0

l00004019:
	andeq	r0,r0,r0

l0000401D:
	andeq	r0,r0,r0

l00004021:
	andeq	r0,r0,r0

l00004025:
	andeq	r0,r0,r0

l00004029:
	andeq	r0,r0,r0

l0000402D:
	andeq	r0,r0,r0

l00004031:
	andeq	r0,r0,r0

l00004035:
	andeq	r0,r0,r0

l00004039:
	andeq	r0,r0,r0

l0000403D:
	andeq	r0,r0,r0

l00004041:
	andeq	r0,r0,r0

l00004045:
	andeq	r0,r0,r0

l00004049:
	andeq	r0,r0,r0

l0000404D:
	andeq	r0,r0,r0

l00004051:
	andeq	r0,r0,r0

l00004055:
	andeq	r0,r0,r0

l00004059:
	andeq	r0,r0,r0

l0000405D:
	andeq	r0,r0,r0

l00004061:
	andeq	r0,r0,r0

l00004065:
	andeq	r0,r0,r0

l00004069:
	andeq	r0,r0,r0

l0000406D:
	andeq	r0,r0,r0

l00004071:
	andeq	r0,r0,r0

l00004075:
	andeq	r0,r0,r0

l00004079:
	andeq	r0,r0,r0

l0000407D:
	andeq	r0,r0,r0

l00004081:
	andeq	r0,r0,r0

l00004085:
	andeq	r0,r0,r0

l00004089:
	andeq	r0,r0,r0

l0000408D:
	andeq	r0,r0,r0

l00004091:
	andeq	r0,r0,r0

l00004095:
	andeq	r0,r0,r0

l00004099:
	andeq	r0,r0,r0

l0000409D:
	andeq	r0,r0,r0

l000040A1:
	andeq	r0,r0,r0

l000040A5:
	andeq	r0,r0,r0

l000040A9:
	andeq	r0,r0,r0

l000040AD:
	andeq	r0,r0,r0

l000040B1:
	andeq	r0,r0,r0

l000040B5:
	andeq	r0,r0,r0

l000040B9:
	andeq	r0,r0,r0

l000040BD:
	andeq	r0,r0,r0

l000040C1:
	andeq	r0,r0,r0

l000040C5:
	andeq	r0,r0,r0

l000040C9:
	andeq	r0,r0,r0

l000040CD:
	andeq	r0,r0,r0

l000040D1:
	andeq	r0,r0,r0

l000040D5:
	andeq	r0,r0,r0

l000040D9:
	andeq	r0,r0,r0

l000040DD:
	andeq	r0,r0,r0

l000040E1:
	andeq	r0,r0,r0

l000040E5:
	andeq	r0,r0,r0

l000040E9:
	andeq	r0,r0,r0

l000040ED:
	andeq	r0,r0,r0

l000040F1:
	andeq	r0,r0,r0

l000040F5:
	andeq	r0,r0,r0

l000040F9:
	andeq	r0,r0,r0

l000040FD:
	andeq	r0,r0,r0

l00004101:
	andeq	r0,r0,r0

l00004105:
	andeq	r0,r0,r0

l00004109:
	andeq	r0,r0,r0

l0000410D:
	andeq	r0,r0,r0

l00004111:
	andeq	r0,r0,r0

l00004115:
	andeq	r0,r0,r0

l00004119:
	andeq	r0,r0,r0

l0000411D:
	andeq	r0,r0,r0

l00004121:
	andeq	r0,r0,r0

l00004125:
	andeq	r0,r0,r0

l00004129:
	andeq	r0,r0,r0

l0000412D:
	andeq	r0,r0,r0

l00004131:
	andeq	r0,r0,r0

l00004135:
	andeq	r0,r0,r0

l00004139:
	andeq	r0,r0,r0

l0000413D:
	andeq	r0,r0,r0

l00004141:
	andeq	r0,r0,r0

l00004145:
	andeq	r0,r0,r0

l00004149:
	andeq	r0,r0,r0

l0000414D:
	andeq	r0,r0,r0

l00004151:
	andeq	r0,r0,r0

l00004155:
	andeq	r0,r0,r0

l00004159:
	andeq	r0,r0,r0

l0000415D:
	andeq	r0,r0,r0

l00004161:
	andeq	r0,r0,r0

l00004165:
	andeq	r0,r0,r0

l00004169:
	andeq	r0,r0,r0

l0000416D:
	andeq	r0,r0,r0

l00004171:
	andeq	r0,r0,r0

l00004175:
	andeq	r0,r0,r0

l00004179:
	andeq	r0,r0,r0

l0000417D:
	andeq	r0,r0,r0

l00004181:
	andeq	r0,r0,r0

l00004185:
	andeq	r0,r0,r0

l00004189:
	andeq	r0,r0,r0

l0000418D:
	andeq	r0,r0,r0

l00004191:
	andeq	r0,r0,r0

l00004195:
	andeq	r0,r0,r0

l00004199:
	andeq	r0,r0,r0

l0000419D:
	andeq	r0,r0,r0

l000041A1:
	andeq	r0,r0,r0

l000041A5:
	andeq	r0,r0,r0

l000041A9:
	andeq	r0,r0,r0

l000041AD:
	andeq	r0,r0,r0

l000041B1:
	andeq	r0,r0,r0

l000041B5:
	andeq	r0,r0,r0

l000041B9:
	andeq	r0,r0,r0

l000041BD:
	andeq	r0,r0,r0

l000041C1:
	andeq	r0,r0,r0

l000041C5:
	andeq	r0,r0,r0

l000041C9:
	andeq	r0,r0,r0

l000041CD:
	andeq	r0,r0,r0

l000041D1:
	andeq	r0,r0,r0

l000041D5:
	andeq	r0,r0,r0

l000041D9:
	andeq	r0,r0,r0

l000041DD:
	andeq	r0,r0,r0

l000041E1:
	andeq	r0,r0,r0

l000041E5:
	andeq	r0,r0,r0

l000041E9:
	andeq	r0,r0,r0

l000041ED:
	andeq	r0,r0,r0

l000041F1:
	andeq	r0,r0,r0

l000041F5:
	andeq	r0,r0,r0

l000041F9:
	andeq	r0,r0,r0

l000041FD:
	andeq	r0,r0,r0

l00004201:
	andeq	r0,r0,r0

l00004205:
	andeq	r0,r0,r0

l00004209:
	andeq	r0,r0,r0

l0000420D:
	andeq	r0,r0,r0

l00004211:
	andeq	r0,r0,r0

l00004215:
	andeq	r0,r0,r0

l00004219:
	andeq	r0,r0,r0

l0000421D:
	andeq	r0,r0,r0

l00004221:
	andeq	r0,r0,r0

l00004225:
	andeq	r0,r0,r0

l00004229:
	andeq	r0,r0,r0

l0000422D:
	andeq	r0,r0,r0

l00004231:
	andeq	r0,r0,r0

l00004235:
	andeq	r0,r0,r0

l00004239:
	andeq	r0,r0,r0

l0000423D:
	andeq	r0,r0,r0

l00004241:
	andeq	r0,r0,r0

l00004245:
	andeq	r0,r0,r0

l00004249:
	andeq	r0,r0,r0

l0000424D:
	andeq	r0,r0,r0

l00004251:
	andeq	r0,r0,r0

l00004255:
	andeq	r0,r0,r0

l00004259:
	andeq	r0,r0,r0

l0000425D:
	andeq	r0,r0,r0

l00004261:
	andeq	r0,r0,r0

l00004265:
	andeq	r0,r0,r0

l00004269:
	andeq	r0,r0,r0

l0000426D:
	andeq	r0,r0,r0

l00004271:
	andeq	r0,r0,r0

l00004275:
	andeq	r0,r0,r0

l00004279:
	andeq	r0,r0,r0

l0000427D:
	andeq	r0,r0,r0

l00004281:
	andeq	r0,r0,r0

l00004285:
	andeq	r0,r0,r0

l00004289:
	andeq	r0,r0,r0

l0000428D:
	andeq	r0,r0,r0

l00004291:
	andeq	r0,r0,r0

l00004295:
	andeq	r0,r0,r0

l00004299:
	andeq	r0,r0,r0

l0000429D:
	andeq	r0,r0,r0

l000042A1:
	andeq	r0,r0,r0

l000042A5:
	andeq	r0,r0,r0

l000042A9:
	andeq	r0,r0,r0

l000042AD:
	andeq	r0,r0,r0

l000042B1:
	andeq	r0,r0,r0

l000042B5:
	andeq	r0,r0,r0

l000042B9:
	andeq	r0,r0,r0

l000042BD:
	andeq	r0,r0,r0

l000042C1:
	andeq	r0,r0,r0

l000042C5:
	andeq	r0,r0,r0

l000042C9:
	andeq	r0,r0,r0

l000042CD:
	andeq	r0,r0,r0

l000042D1:
	andeq	r0,r0,r0

l000042D5:
	andeq	r0,r0,r0

l000042D9:
	andeq	r0,r0,r0

l000042DD:
	andeq	r0,r0,r0

l000042E1:
	andeq	r0,r0,r0

l000042E5:
	andeq	r0,r0,r0

l000042E9:
	andeq	r0,r0,r0

l000042ED:
	andeq	r0,r0,r0

l000042F1:
	andeq	r0,r0,r0

l000042F5:
	andeq	r0,r0,r0

l000042F9:
	andeq	r0,r0,r0

l000042FD:
	andeq	r0,r0,r0

l00004301:
	andeq	r0,r0,r0

l00004305:
	andeq	r0,r0,r0

l00004309:
	andeq	r0,r0,r0

l0000430D:
	andeq	r0,r0,r0

l00004311:
	andeq	r0,r0,r0

l00004315:
	andeq	r0,r0,r0

l00004319:
	andeq	r0,r0,r0

l0000431D:
	andeq	r0,r0,r0

l00004321:
	andeq	r0,r0,r0

l00004325:
	andeq	r0,r0,r0

l00004329:
	andeq	r0,r0,r0

l0000432D:
	andeq	r0,r0,r0

l00004331:
	andeq	r0,r0,r0

l00004335:
	andeq	r0,r0,r0

l00004339:
	andeq	r0,r0,r0

l0000433D:
	andeq	r0,r0,r0

l00004341:
	andeq	r0,r0,r0

l00004345:
	andeq	r0,r0,r0

l00004349:
	andeq	r0,r0,r0

l0000434D:
	andeq	r0,r0,r0

l00004351:
	andeq	r0,r0,r0

l00004355:
	andeq	r0,r0,r0

l00004359:
	andeq	r0,r0,r0

l0000435D:
	andeq	r0,r0,r0

l00004361:
	andeq	r0,r0,r0

l00004365:
	andeq	r0,r0,r0

l00004369:
	andeq	r0,r0,r0

l0000436D:
	andeq	r0,r0,r0

l00004371:
	andeq	r0,r0,r0

l00004375:
	andeq	r0,r0,r0

l00004379:
	andeq	r0,r0,r0

l0000437D:
	andeq	r0,r0,r0

l00004381:
	andeq	r0,r0,r0

l00004385:
	andeq	r0,r0,r0

l00004389:
	andeq	r0,r0,r0

l0000438D:
	andeq	r0,r0,r0

l00004391:
	andeq	r0,r0,r0

l00004395:
	andeq	r0,r0,r0

l00004399:
	andeq	r0,r0,r0

l0000439D:
	andeq	r0,r0,r0

l000043A1:
	andeq	r0,r0,r0

l000043A5:
	andeq	r0,r0,r0

l000043A9:
	andeq	r0,r0,r0

l000043AD:
	andeq	r0,r0,r0

l000043B1:
	andeq	r0,r0,r0

l000043B5:
	andeq	r0,r0,r0

l000043B9:
	andeq	r0,r0,r0

l000043BD:
	andeq	r0,r0,r0

l000043C1:
	andeq	r0,r0,r0

l000043C5:
	andeq	r0,r0,r0

l000043C9:
	andeq	r0,r0,r0

l000043CD:
	andeq	r0,r0,r0

l000043D1:
	andeq	r0,r0,r0

l000043D5:
	andeq	r0,r0,r0

l000043D9:
	andeq	r0,r0,r0

l000043DD:
	andeq	r0,r0,r0

l000043E1:
	andeq	r0,r0,r0

l000043E5:
	andeq	r0,r0,r0

l000043E9:
	andeq	r0,r0,r0

l000043ED:
	andeq	r0,r0,r0

l000043F1:
	andeq	r0,r0,r0

l000043F5:
	andeq	r0,r0,r0

l000043F9:
	andeq	r0,r0,r0

l000043FD:
	andeq	r0,r0,r0

l00004401:
	andeq	r0,r0,r0

l00004405:
	andeq	r0,r0,r0

l00004409:
	andeq	r0,r0,r0

l0000440D:
	andeq	r0,r0,r0

l00004411:
	andeq	r0,r0,r0

l00004415:
	andeq	r0,r0,r0

l00004419:
	andeq	r0,r0,r0

l0000441D:
	andeq	r0,r0,r0

l00004421:
	andeq	r0,r0,r0

l00004425:
	andeq	r0,r0,r0

l00004429:
	andeq	r0,r0,r0

l0000442D:
	andeq	r0,r0,r0

l00004431:
	andeq	r0,r0,r0

l00004435:
	andeq	r0,r0,r0

l00004439:
	andeq	r0,r0,r0

l0000443D:
	andeq	r0,r0,r0

l00004441:
	andeq	r0,r0,r0

l00004445:
	andeq	r0,r0,r0

l00004449:
	andeq	r0,r0,r0

l0000444D:
	andeq	r0,r0,r0

l00004451:
	andeq	r0,r0,r0

l00004455:
	andeq	r0,r0,r0

l00004459:
	andeq	r0,r0,r0

l0000445D:
	andeq	r0,r0,r0

l00004461:
	andeq	r0,r0,r0

l00004465:
	andeq	r0,r0,r0

l00004469:
	andeq	r0,r0,r0

l0000446D:
	andeq	r0,r0,r0

l00004471:
	andeq	r0,r0,r0

l00004475:
	andeq	r0,r0,r0

l00004479:
	andeq	r0,r0,r0

l0000447D:
	andeq	r0,r0,r0

l00004481:
	andeq	r0,r0,r0

l00004485:
	andeq	r0,r0,r0

l00004489:
	andeq	r0,r0,r0

l0000448D:
	andeq	r0,r0,r0

l00004491:
	andeq	r0,r0,r0

l00004495:
	andeq	r0,r0,r0

l00004499:
	andeq	r0,r0,r0

l0000449D:
	andeq	r0,r0,r0

l000044A1:
	andeq	r0,r0,r0

l000044A5:
	andeq	r0,r0,r0

l000044A9:
	andeq	r0,r0,r0

l000044AD:
	andeq	r0,r0,r0

l000044B1:
	andeq	r0,r0,r0

l000044B5:
	andeq	r0,r0,r0

l000044B9:
	andeq	r0,r0,r0

l000044BD:
	andeq	r0,r0,r0

l000044C1:
	andeq	r0,r0,r0

l000044C5:
	andeq	r0,r0,r0

l000044C9:
	andeq	r0,r0,r0

l000044CD:
	andeq	r0,r0,r0

l000044D1:
	andeq	r0,r0,r0

l000044D5:
	andeq	r0,r0,r0

l000044D9:
	andeq	r0,r0,r0

l000044DD:
	andeq	r0,r0,r0

l000044E1:
	andeq	r0,r0,r0

l000044E5:
	andeq	r0,r0,r0

l000044E9:
	andeq	r0,r0,r0

l000044ED:
	andeq	r0,r0,r0

l000044F1:
	andeq	r0,r0,r0

l000044F5:
	andeq	r0,r0,r0

l000044F9:
	andeq	r0,r0,r0

l000044FD:
	andeq	r0,r0,r0

l00004501:
	andeq	r0,r0,r0

l00004505:
	andeq	r0,r0,r0

l00004509:
	andeq	r0,r0,r0

l0000450D:
	andeq	r0,r0,r0

l00004511:
	andeq	r0,r0,r0

l00004515:
	andeq	r0,r0,r0

l00004519:
	andeq	r0,r0,r0

l0000451D:
	andeq	r0,r0,r0

l00004521:
	andeq	r0,r0,r0

l00004525:
	andeq	r0,r0,r0

l00004529:
	andeq	r0,r0,r0

l0000452D:
	andeq	r0,r0,r0

l00004531:
	andeq	r0,r0,r0

l00004535:
	andeq	r0,r0,r0

l00004539:
	andeq	r0,r0,r0

l0000453D:
	andeq	r0,r0,r0

l00004541:
	andeq	r0,r0,r0

l00004545:
	andeq	r0,r0,r0

l00004549:
	andeq	r0,r0,r0

l0000454D:
	andeq	r0,r0,r0

l00004551:
	andeq	r0,r0,r0

l00004555:
	andeq	r0,r0,r0

l00004559:
	andeq	r0,r0,r0

l0000455D:
	andeq	r0,r0,r0

l00004561:
	andeq	r0,r0,r0

l00004565:
	andeq	r0,r0,r0

l00004569:
	andeq	r0,r0,r0

l0000456D:
	andeq	r0,r0,r0

l00004571:
	andeq	r0,r0,r0

l00004575:
	andeq	r0,r0,r0

l00004579:
	andeq	r0,r0,r0

l0000457D:
	andeq	r0,r0,r0

l00004581:
	andeq	r0,r0,r0

l00004585:
	andeq	r0,r0,r0

l00004589:
	andeq	r0,r0,r0

l0000458D:
	andeq	r0,r0,r0

l00004591:
	andeq	r0,r0,r0

l00004595:
	andeq	r0,r0,r0

l00004599:
	andeq	r0,r0,r0

l0000459D:
	andeq	r0,r0,r0

l000045A1:
	andeq	r0,r0,r0

l000045A5:
	andeq	r0,r0,r0

l000045A9:
	andeq	r0,r0,r0

l000045AD:
	andeq	r0,r0,r0

l000045B1:
	andeq	r0,r0,r0

l000045B5:
	andeq	r0,r0,r0

l000045B9:
	andeq	r0,r0,r0

l000045BD:
	andeq	r0,r0,r0

l000045C1:
	andeq	r0,r0,r0

l000045C5:
	andeq	r0,r0,r0

l000045C9:
	andeq	r0,r0,r0

l000045CD:
	andeq	r0,r0,r0

l000045D1:
	andeq	r0,r0,r0

l000045D5:
	andeq	r0,r0,r0

l000045D9:
	andeq	r0,r0,r0

l000045DD:
	andeq	r0,r0,r0

l000045E1:
	andeq	r0,r0,r0

l000045E5:
	andeq	r0,r0,r0

l000045E9:
	andeq	r0,r0,r0

l000045ED:
	andeq	r0,r0,r0

l000045F1:
	andeq	r0,r0,r0

l000045F5:
	andeq	r0,r0,r0

l000045F9:
	andeq	r0,r0,r0

l000045FD:
	andeq	r0,r0,r0

l00004601:
	andeq	r0,r0,r0

l00004605:
	andeq	r0,r0,r0

l00004609:
	andeq	r0,r0,r0

l0000460D:
	andeq	r0,r0,r0

l00004611:
	andeq	r0,r0,r0

l00004615:
	andeq	r0,r0,r0

l00004619:
	andeq	r0,r0,r0

l0000461D:
	andeq	r0,r0,r0

l00004621:
	andeq	r0,r0,r0

l00004625:
	andeq	r0,r0,r0

l00004629:
	andeq	r0,r0,r0

l0000462D:
	andeq	r0,r0,r0

l00004631:
	andeq	r0,r0,r0

l00004635:
	andeq	r0,r0,r0

l00004639:
	andeq	r0,r0,r0

l0000463D:
	andeq	r0,r0,r0

l00004641:
	andeq	r0,r0,r0

l00004645:
	andeq	r0,r0,r0

l00004649:
	andeq	r0,r0,r0

l0000464D:
	andeq	r0,r0,r0

l00004651:
	andeq	r0,r0,r0

l00004655:
	andeq	r0,r0,r0

l00004659:
	andeq	r0,r0,r0

l0000465D:
	andeq	r0,r0,r0

l00004661:
	andeq	r0,r0,r0

l00004665:
	andeq	r0,r0,r0

l00004669:
	andeq	r0,r0,r0

l0000466D:
	andeq	r0,r0,r0

l00004671:
	andeq	r0,r0,r0

l00004675:
	andeq	r0,r0,r0

l00004679:
	andeq	r0,r0,r0

l0000467D:
	andeq	r0,r0,r0

l00004681:
	andeq	r0,r0,r0

l00004685:
	andeq	r0,r0,r0

l00004689:
	andeq	r0,r0,r0

l0000468D:
	andeq	r0,r0,r0

l00004691:
	andeq	r0,r0,r0

l00004695:
	andeq	r0,r0,r0

l00004699:
	andeq	r0,r0,r0

l0000469D:
	andeq	r0,r0,r0

l000046A1:
	andeq	r0,r0,r0

l000046A5:
	andeq	r0,r0,r0

l000046A9:
	andeq	r0,r0,r0

l000046AD:
	andeq	r0,r0,r0

l000046B1:
	andeq	r0,r0,r0

l000046B5:
	andeq	r0,r0,r0

l000046B9:
	andeq	r0,r0,r0

l000046BD:
	andeq	r0,r0,r0

l000046C1:
	andeq	r0,r0,r0

l000046C5:
	andeq	r0,r0,r0

l000046C9:
	andeq	r0,r0,r0

l000046CD:
	andeq	r0,r0,r0

l000046D1:
	andeq	r0,r0,r0

l000046D5:
	andeq	r0,r0,r0

l000046D9:
	andeq	r0,r0,r0

l000046DD:
	andeq	r0,r0,r0

l000046E1:
	andeq	r0,r0,r0

l000046E5:
	andeq	r0,r0,r0

l000046E9:
	andeq	r0,r0,r0

l000046ED:
	andeq	r0,r0,r0

l000046F1:
	andeq	r0,r0,r0

l000046F5:
	andeq	r0,r0,r0

l000046F9:
	andeq	r0,r0,r0

l000046FD:
	andeq	r0,r0,r0

l00004701:
	andeq	r0,r0,r0

l00004705:
	andeq	r0,r0,r0

l00004709:
	andeq	r0,r0,r0

l0000470D:
	andeq	r0,r0,r0

l00004711:
	andeq	r0,r0,r0

l00004715:
	andeq	r0,r0,r0

l00004719:
	andeq	r0,r0,r0

l0000471D:
	andeq	r0,r0,r0

l00004721:
	andeq	r0,r0,r0

l00004725:
	andeq	r0,r0,r0

l00004729:
	andeq	r0,r0,r0

l0000472D:
	andeq	r0,r0,r0

l00004731:
	andeq	r0,r0,r0

l00004735:
	andeq	r0,r0,r0

l00004739:
	andeq	r0,r0,r0

l0000473D:
	andeq	r0,r0,r0

l00004741:
	andeq	r0,r0,r0

l00004745:
	andeq	r0,r0,r0

l00004749:
	andeq	r0,r0,r0

l0000474D:
	andeq	r0,r0,r0

l00004751:
	andeq	r0,r0,r0

l00004755:
	andeq	r0,r0,r0

l00004759:
	andeq	r0,r0,r0

l0000475D:
	andeq	r0,r0,r0

l00004761:
	andeq	r0,r0,r0

l00004765:
	andeq	r0,r0,r0

l00004769:
	andeq	r0,r0,r0

l0000476D:
	andeq	r0,r0,r0

l00004771:
	andeq	r0,r0,r0

l00004775:
	andeq	r0,r0,r0

l00004779:
	andeq	r0,r0,r0

l0000477D:
	andeq	r0,r0,r0

l00004781:
	andeq	r0,r0,r0

l00004785:
	andeq	r0,r0,r0

l00004789:
	andeq	r0,r0,r0

l0000478D:
	andeq	r0,r0,r0

l00004791:
	andeq	r0,r0,r0

l00004795:
	andeq	r0,r0,r0

l00004799:
	andeq	r0,r0,r0

l0000479D:
	andeq	r0,r0,r0

l000047A1:
	andeq	r0,r0,r0

l000047A5:
	andeq	r0,r0,r0

l000047A9:
	andeq	r0,r0,r0

l000047AD:
	andeq	r0,r0,r0

l000047B1:
	andeq	r0,r0,r0

l000047B5:
	andeq	r0,r0,r0

l000047B9:
	andeq	r0,r0,r0

l000047BD:
	andeq	r0,r0,r0

l000047C1:
	andeq	r0,r0,r0

l000047C5:
	andeq	r0,r0,r0

l000047C9:
	andeq	r0,r0,r0

l000047CD:
	andeq	r0,r0,r0

l000047D1:
	andeq	r0,r0,r0

l000047D5:
	andeq	r0,r0,r0

l000047D9:
	andeq	r0,r0,r0

l000047DD:
	andeq	r0,r0,r0

l000047E1:
	andeq	r0,r0,r0

l000047E5:
	andeq	r0,r0,r0

l000047E9:
	andeq	r0,r0,r0

l000047ED:
	andeq	r0,r0,r0

l000047F1:
	andeq	r0,r0,r0

l000047F5:
	andeq	r0,r0,r0

l000047F9:
	andeq	r0,r0,r0

l000047FD:
	andeq	r0,r0,r0

l00004801:
	andeq	r0,r0,r0

l00004805:
	andeq	r0,r0,r0

l00004809:
	andeq	r0,r0,r0

l0000480D:
	andeq	r0,r0,r0

l00004811:
	andeq	r0,r0,r0

l00004815:
	andeq	r0,r0,r0

l00004819:
	andeq	r0,r0,r0

l0000481D:
	andeq	r0,r0,r0

l00004821:
	andeq	r0,r0,r0

l00004825:
	andeq	r0,r0,r0

l00004829:
	andeq	r0,r0,r0

l0000482D:
	andeq	r0,r0,r0

l00004831:
	andeq	r0,r0,r0

l00004835:
	andeq	r0,r0,r0

l00004839:
	andeq	r0,r0,r0

l0000483D:
	andeq	r0,r0,r0

l00004841:
	andeq	r0,r0,r0

l00004845:
	andeq	r0,r0,r0

l00004849:
	andeq	r0,r0,r0

l0000484D:
	andeq	r0,r0,r0

l00004851:
	andeq	r0,r0,r0

l00004855:
	andeq	r0,r0,r0

l00004859:
	andeq	r0,r0,r0

l0000485D:
	andeq	r0,r0,r0

l00004861:
	andeq	r0,r0,r0

l00004865:
	andeq	r0,r0,r0

l00004869:
	andeq	r0,r0,r0

l0000486D:
	andeq	r0,r0,r0

l00004871:
	andeq	r0,r0,r0

l00004875:
	andeq	r0,r0,r0

l00004879:
	andeq	r0,r0,r0

l0000487D:
	andeq	r0,r0,r0

l00004881:
	andeq	r0,r0,r0

l00004885:
	andeq	r0,r0,r0

l00004889:
	andeq	r0,r0,r0

l0000488D:
	andeq	r0,r0,r0

l00004891:
	andeq	r0,r0,r0

l00004895:
	andeq	r0,r0,r0

l00004899:
	andeq	r0,r0,r0

l0000489D:
	andeq	r0,r0,r0

l000048A1:
	andeq	r0,r0,r0

l000048A5:
	andeq	r0,r0,r0

l000048A9:
	andeq	r0,r0,r0

l000048AD:
	andeq	r0,r0,r0

l000048B1:
	andeq	r0,r0,r0

l000048B5:
	andeq	r0,r0,r0

l000048B9:
	andeq	r0,r0,r0

l000048BD:
	andeq	r0,r0,r0

l000048C1:
	andeq	r0,r0,r0

l000048C5:
	andeq	r0,r0,r0

l000048C9:
	andeq	r0,r0,r0

l000048CD:
	andeq	r0,r0,r0

l000048D1:
	andeq	r0,r0,r0

l000048D5:
	andeq	r0,r0,r0

l000048D9:
	andeq	r0,r0,r0

l000048DD:
	andeq	r0,r0,r0

l000048E1:
	andeq	r0,r0,r0

l000048E5:
	andeq	r0,r0,r0

l000048E9:
	andeq	r0,r0,r0

l000048ED:
	andeq	r0,r0,r0

l000048F1:
	andeq	r0,r0,r0

l000048F5:
	andeq	r0,r0,r0

l000048F9:
	andeq	r0,r0,r0

l000048FD:
	andeq	r0,r0,r0

l00004901:
	andeq	r0,r0,r0

l00004905:
	andeq	r0,r0,r0

l00004909:
	andeq	r0,r0,r0

l0000490D:
	andeq	r0,r0,r0

l00004911:
	andeq	r0,r0,r0

l00004915:
	andeq	r0,r0,r0

l00004919:
	andeq	r0,r0,r0

l0000491D:
	andeq	r0,r0,r0

l00004921:
	andeq	r0,r0,r0

l00004925:
	andeq	r0,r0,r0

l00004929:
	andeq	r0,r0,r0

l0000492D:
	andeq	r0,r0,r0

l00004931:
	andeq	r0,r0,r0

l00004935:
	andeq	r0,r0,r0

l00004939:
	andeq	r0,r0,r0

l0000493D:
	andeq	r0,r0,r0

l00004941:
	andeq	r0,r0,r0

l00004945:
	andeq	r0,r0,r0

l00004949:
	andeq	r0,r0,r0

l0000494D:
	andeq	r0,r0,r0

l00004951:
	andeq	r0,r0,r0

l00004955:
	andeq	r0,r0,r0

l00004959:
	andeq	r0,r0,r0

l0000495D:
	andeq	r0,r0,r0

l00004961:
	andeq	r0,r0,r0

l00004965:
	andeq	r0,r0,r0

l00004969:
	andeq	r0,r0,r0

l0000496D:
	andeq	r0,r0,r0

l00004971:
	andeq	r0,r0,r0

l00004975:
	andeq	r0,r0,r0

l00004979:
	andeq	r0,r0,r0

l0000497D:
	andeq	r0,r0,r0

l00004981:
	andeq	r0,r0,r0

l00004985:
	andeq	r0,r0,r0

l00004989:
	andeq	r0,r0,r0

l0000498D:
	andeq	r0,r0,r0

l00004991:
	andeq	r0,r0,r0

l00004995:
	andeq	r0,r0,r0

l00004999:
	andeq	r0,r0,r0

l0000499D:
	andeq	r0,r0,r0

l000049A1:
	andeq	r0,r0,r0

l000049A5:
	andeq	r0,r0,r0

l000049A9:
	andeq	r0,r0,r0

l000049AD:
	andeq	r0,r0,r0

l000049B1:
	andeq	r0,r0,r0

l000049B5:
	andeq	r0,r0,r0

l000049B9:
	andeq	r0,r0,r0

l000049BD:
	andeq	r0,r0,r0

l000049C1:
	andeq	r0,r0,r0

l000049C5:
	andeq	r0,r0,r0

l000049C9:
	andeq	r0,r0,r0

l000049CD:
	andeq	r0,r0,r0

l000049D1:
	andeq	r0,r0,r0

l000049D5:
	andeq	r0,r0,r0

l000049D9:
	andeq	r0,r0,r0

l000049DD:
	andeq	r0,r0,r0

l000049E1:
	andeq	r0,r0,r0

l000049E5:
	andeq	r0,r0,r0

l000049E9:
	andeq	r0,r0,r0

l000049ED:
	andeq	r0,r0,r0

l000049F1:
	andeq	r0,r0,r0

l000049F5:
	andeq	r0,r0,r0

l000049F9:
	andeq	r0,r0,r0

l000049FD:
	andeq	r0,r0,r0

l00004A01:
	andeq	r0,r0,r0

l00004A05:
	andeq	r0,r0,r0

l00004A09:
	andeq	r0,r0,r0

l00004A0D:
	andeq	r0,r0,r0

l00004A11:
	andeq	r0,r0,r0

l00004A15:
	andeq	r0,r0,r0

l00004A19:
	andeq	r0,r0,r0

l00004A1D:
	andeq	r0,r0,r0

l00004A21:
	andeq	r0,r0,r0

l00004A25:
	andeq	r0,r0,r0

l00004A29:
	andeq	r0,r0,r0

l00004A2D:
	andeq	r0,r0,r0

l00004A31:
	andeq	r0,r0,r0

l00004A35:
	andeq	r0,r0,r0

l00004A39:
	andeq	r0,r0,r0

l00004A3D:
	andeq	r0,r0,r0

l00004A41:
	andeq	r0,r0,r0

l00004A45:
	andeq	r0,r0,r0

l00004A49:
	andeq	r0,r0,r0

l00004A4D:
	andeq	r0,r0,r0

l00004A51:
	andeq	r0,r0,r0

l00004A55:
	andeq	r0,r0,r0

l00004A59:
	andeq	r0,r0,r0

l00004A5D:
	andeq	r0,r0,r0

l00004A61:
	andeq	r0,r0,r0

l00004A65:
	andeq	r0,r0,r0

l00004A69:
	andeq	r0,r0,r0

l00004A6D:
	andeq	r0,r0,r0

l00004A71:
	andeq	r0,r0,r0

l00004A75:
	andeq	r0,r0,r0

l00004A79:
	andeq	r0,r0,r0

l00004A7D:
	andeq	r0,r0,r0

l00004A81:
	andeq	r0,r0,r0

l00004A85:
	andeq	r0,r0,r0

l00004A89:
	andeq	r0,r0,r0

l00004A8D:
	andeq	r0,r0,r0

l00004A91:
	andeq	r0,r0,r0

l00004A95:
	andeq	r0,r0,r0

l00004A99:
	andeq	r0,r0,r0

l00004A9D:
	andeq	r0,r0,r0

l00004AA1:
	andeq	r0,r0,r0

l00004AA5:
	andeq	r0,r0,r0

l00004AA9:
	andeq	r0,r0,r0

l00004AAD:
	andeq	r0,r0,r0

l00004AB1:
	andeq	r0,r0,r0

l00004AB5:
	andeq	r0,r0,r0

l00004AB9:
	andeq	r0,r0,r0

l00004ABD:
	andeq	r0,r0,r0

l00004AC1:
	andeq	r0,r0,r0

l00004AC5:
	andeq	r0,r0,r0

l00004AC9:
	andeq	r0,r0,r0

l00004ACD:
	andeq	r0,r0,r0

l00004AD1:
	andeq	r0,r0,r0

l00004AD5:
	andeq	r0,r0,r0

l00004AD9:
	andeq	r0,r0,r0

l00004ADD:
	andeq	r0,r0,r0

l00004AE1:
	andeq	r0,r0,r0

l00004AE5:
	andeq	r0,r0,r0

l00004AE9:
	andeq	r0,r0,r0

l00004AED:
	andeq	r0,r0,r0

l00004AF1:
	andeq	r0,r0,r0

l00004AF5:
	andeq	r0,r0,r0

l00004AF9:
	andeq	r0,r0,r0

l00004AFD:
	andeq	r0,r0,r0

l00004B01:
	andeq	r0,r0,r0

l00004B05:
	andeq	r0,r0,r0

l00004B09:
	andeq	r0,r0,r0

l00004B0D:
	andeq	r0,r0,r0

l00004B11:
	andeq	r0,r0,r0

l00004B15:
	andeq	r0,r0,r0

l00004B19:
	andeq	r0,r0,r0

l00004B1D:
	andeq	r0,r0,r0

l00004B21:
	andeq	r0,r0,r0

l00004B25:
	andeq	r0,r0,r0

l00004B29:
	andeq	r0,r0,r0

l00004B2D:
	andeq	r0,r0,r0

l00004B31:
	andeq	r0,r0,r0

l00004B35:
	andeq	r0,r0,r0

l00004B39:
	andeq	r0,r0,r0

l00004B3D:
	andeq	r0,r0,r0

l00004B41:
	andeq	r0,r0,r0

l00004B45:
	andeq	r0,r0,r0

l00004B49:
	andeq	r0,r0,r0

l00004B4D:
	andeq	r0,r0,r0

l00004B51:
	andeq	r0,r0,r0

l00004B55:
	andeq	r0,r0,r0

l00004B59:
	andeq	r0,r0,r0

l00004B5D:
	andeq	r0,r0,r0

l00004B61:
	andeq	r0,r0,r0

l00004B65:
	andeq	r0,r0,r0

l00004B69:
	andeq	r0,r0,r0

l00004B6D:
	andeq	r0,r0,r0

l00004B71:
	andeq	r0,r0,r0

l00004B75:
	andeq	r0,r0,r0

l00004B79:
	andeq	r0,r0,r0

l00004B7D:
	andeq	r0,r0,r0

l00004B81:
	andeq	r0,r0,r0

l00004B85:
	andeq	r0,r0,r0

l00004B89:
	andeq	r0,r0,r0

l00004B8D:
	andeq	r0,r0,r0

l00004B91:
	andeq	r0,r0,r0

l00004B95:
	andeq	r0,r0,r0

l00004B99:
	andeq	r0,r0,r0

l00004B9D:
	andeq	r0,r0,r0

l00004BA1:
	andeq	r0,r0,r0

l00004BA5:
	andeq	r0,r0,r0

l00004BA9:
	andeq	r0,r0,r0

l00004BAD:
	andeq	r0,r0,r0

l00004BB1:
	andeq	r0,r0,r0

l00004BB5:
	andeq	r0,r0,r0

l00004BB9:
	andeq	r0,r0,r0

l00004BBD:
	andeq	r0,r0,r0

l00004BC1:
	andeq	r0,r0,r0

l00004BC5:
	andeq	r0,r0,r0

l00004BC9:
	andeq	r0,r0,r0

l00004BCD:
	andeq	r0,r0,r0

l00004BD1:
	andeq	r0,r0,r0

l00004BD5:
	andeq	r0,r0,r0

l00004BD9:
	andeq	r0,r0,r0

l00004BDD:
	andeq	r0,r0,r0

l00004BE1:
	andeq	r0,r0,r0

l00004BE5:
	andeq	r0,r0,r0

l00004BE9:
	andeq	r0,r0,r0

l00004BED:
	andeq	r0,r0,r0

l00004BF1:
	andeq	r0,r0,r0

l00004BF5:
	andeq	r0,r0,r0

l00004BF9:
	andeq	r0,r0,r0

l00004BFD:
	andeq	r0,r0,r0

l00004C01:
	andeq	r0,r0,r0

l00004C05:
	andeq	r0,r0,r0

l00004C09:
	andeq	r0,r0,r0

l00004C0D:
	andeq	r0,r0,r0

l00004C11:
	andeq	r0,r0,r0

l00004C15:
	andeq	r0,r0,r0

l00004C19:
	andeq	r0,r0,r0

l00004C1D:
	andeq	r0,r0,r0

l00004C21:
	andeq	r0,r0,r0

l00004C25:
	andeq	r0,r0,r0

l00004C29:
	andeq	r0,r0,r0

l00004C2D:
	andeq	r0,r0,r0

l00004C31:
	andeq	r0,r0,r0

l00004C35:
	andeq	r0,r0,r0

l00004C39:
	andeq	r0,r0,r0

l00004C3D:
	andeq	r0,r0,r0

l00004C41:
	andeq	r0,r0,r0

l00004C45:
	andeq	r0,r0,r0

l00004C49:
	andeq	r0,r0,r0

l00004C4D:
	andeq	r0,r0,r0

l00004C51:
	andeq	r0,r0,r0

l00004C55:
	andeq	r0,r0,r0

l00004C59:
	andeq	r0,r0,r0

l00004C5D:
	andeq	r0,r0,r0

l00004C61:
	andeq	r0,r0,r0

l00004C65:
	andeq	r0,r0,r0

l00004C69:
	andeq	r0,r0,r0

l00004C6D:
	andeq	r0,r0,r0

l00004C71:
	andeq	r0,r0,r0

l00004C75:
	andeq	r0,r0,r0

l00004C79:
	andeq	r0,r0,r0

l00004C7D:
	andeq	r0,r0,r0

l00004C81:
	andeq	r0,r0,r0

l00004C85:
	andeq	r0,r0,r0

l00004C89:
	andeq	r0,r0,r0

l00004C8D:
	andeq	r0,r0,r0

l00004C91:
	andeq	r0,r0,r0

l00004C95:
	andeq	r0,r0,r0

l00004C99:
	andeq	r0,r0,r0

l00004C9D:
	andeq	r0,r0,r0

l00004CA1:
	andeq	r0,r0,r0

l00004CA5:
	andeq	r0,r0,r0

l00004CA9:
	andeq	r0,r0,r0

l00004CAD:
	andeq	r0,r0,r0

l00004CB1:
	andeq	r0,r0,r0

l00004CB5:
	andeq	r0,r0,r0

l00004CB9:
	andeq	r0,r0,r0

l00004CBD:
	andeq	r0,r0,r0

l00004CC1:
	andeq	r0,r0,r0

l00004CC5:
	andeq	r0,r0,r0

l00004CC9:
	andeq	r0,r0,r0

l00004CCD:
	andeq	r0,r0,r0

l00004CD1:
	andeq	r0,r0,r0

l00004CD5:
	andeq	r0,r0,r0

l00004CD9:
	andeq	r0,r0,r0

l00004CDD:
	andeq	r0,r0,r0

l00004CE1:
	andeq	r0,r0,r0

l00004CE5:
	andeq	r0,r0,r0

l00004CE9:
	andeq	r0,r0,r0

l00004CED:
	andeq	r0,r0,r0

l00004CF1:
	andeq	r0,r0,r0

l00004CF5:
	andeq	r0,r0,r0

l00004CF9:
	andeq	r0,r0,r0

l00004CFD:
	andeq	r0,r0,r0

l00004D01:
	andeq	r0,r0,r0

l00004D05:
	andeq	r0,r0,r0

l00004D09:
	andeq	r0,r0,r0

l00004D0D:
	andeq	r0,r0,r0

l00004D11:
	andeq	r0,r0,r0

l00004D15:
	andeq	r0,r0,r0

l00004D19:
	andeq	r0,r0,r0

l00004D1D:
	andeq	r0,r0,r0

l00004D21:
	andeq	r0,r0,r0

l00004D25:
	andeq	r0,r0,r0

l00004D29:
	andeq	r0,r0,r0

l00004D2D:
	andeq	r0,r0,r0

l00004D31:
	andeq	r0,r0,r0

l00004D35:
	andeq	r0,r0,r0

l00004D39:
	andeq	r0,r0,r0

l00004D3D:
	andeq	r0,r0,r0

l00004D41:
	andeq	r0,r0,r0

l00004D45:
	andeq	r0,r0,r0

l00004D49:
	andeq	r0,r0,r0

l00004D4D:
	andeq	r0,r0,r0

l00004D51:
	andeq	r0,r0,r0

l00004D55:
	andeq	r0,r0,r0

l00004D59:
	andeq	r0,r0,r0

l00004D5D:
	andeq	r0,r0,r0

l00004D61:
	andeq	r0,r0,r0

l00004D65:
	andeq	r0,r0,r0

l00004D69:
	andeq	r0,r0,r0

l00004D6D:
	andeq	r0,r0,r0

l00004D71:
	andeq	r0,r0,r0

l00004D75:
	andeq	r0,r0,r0

l00004D79:
	andeq	r0,r0,r0

l00004D7D:
	andeq	r0,r0,r0

l00004D81:
	andeq	r0,r0,r0

l00004D85:
	andeq	r0,r0,r0

l00004D89:
	andeq	r0,r0,r0

l00004D8D:
	andeq	r0,r0,r0

l00004D91:
	andeq	r0,r0,r0

l00004D95:
	andeq	r0,r0,r0

l00004D99:
	andeq	r0,r0,r0

l00004D9D:
	andeq	r0,r0,r0

l00004DA1:
	andeq	r0,r0,r0

l00004DA5:
	andeq	r0,r0,r0

l00004DA9:
	andeq	r0,r0,r0

l00004DAD:
	andeq	r0,r0,r0

l00004DB1:
	andeq	r0,r0,r0

l00004DB5:
	andeq	r0,r0,r0

l00004DB9:
	andeq	r0,r0,r0

l00004DBD:
	andeq	r0,r0,r0

l00004DC1:
	andeq	r0,r0,r0

l00004DC5:
	andeq	r0,r0,r0

l00004DC9:
	andeq	r0,r0,r0

l00004DCD:
	andeq	r0,r0,r0

l00004DD1:
	andeq	r0,r0,r0

l00004DD5:
	andeq	r0,r0,r0

l00004DD9:
	andeq	r0,r0,r0

l00004DDD:
	andeq	r0,r0,r0

l00004DE1:
	andeq	r0,r0,r0

l00004DE5:
	andeq	r0,r0,r0

l00004DE9:
	andeq	r0,r0,r0

l00004DED:
	andeq	r0,r0,r0

l00004DF1:
	andeq	r0,r0,r0

l00004DF5:
	andeq	r0,r0,r0

l00004DF9:
	andeq	r0,r0,r0

l00004DFD:
	andeq	r0,r0,r0

l00004E01:
	andeq	r0,r0,r0

l00004E05:
	andeq	r0,r0,r0

l00004E09:
	andeq	r0,r0,r0

l00004E0D:
	andeq	r0,r0,r0

l00004E11:
	andeq	r0,r0,r0

l00004E15:
	andeq	r0,r0,r0

l00004E19:
	andeq	r0,r0,r0

l00004E1D:
	andeq	r0,r0,r0

l00004E21:
	andeq	r0,r0,r0

l00004E25:
	andeq	r0,r0,r0

l00004E29:
	andeq	r0,r0,r0

l00004E2D:
	andeq	r0,r0,r0

l00004E31:
	andeq	r0,r0,r0

l00004E35:
	andeq	r0,r0,r0

l00004E39:
	andeq	r0,r0,r0

l00004E3D:
	andeq	r0,r0,r0

l00004E41:
	andeq	r0,r0,r0

l00004E45:
	andeq	r0,r0,r0

l00004E49:
	andeq	r0,r0,r0

l00004E4D:
	andeq	r0,r0,r0

l00004E51:
	andeq	r0,r0,r0

l00004E55:
	andeq	r0,r0,r0

l00004E59:
	andeq	r0,r0,r0

l00004E5D:
	andeq	r0,r0,r0

l00004E61:
	andeq	r0,r0,r0

l00004E65:
	andeq	r0,r0,r0

l00004E69:
	andeq	r0,r0,r0

l00004E6D:
	andeq	r0,r0,r0

l00004E71:
	andeq	r0,r0,r0

l00004E75:
	andeq	r0,r0,r0

l00004E79:
	andeq	r0,r0,r0

l00004E7D:
	andeq	r0,r0,r0

l00004E81:
	andeq	r0,r0,r0

l00004E85:
	andeq	r0,r0,r0

l00004E89:
	andeq	r0,r0,r0

l00004E8D:
	andeq	r0,r0,r0

l00004E91:
	andeq	r0,r0,r0

l00004E95:
	andeq	r0,r0,r0

l00004E99:
	andeq	r0,r0,r0

l00004E9D:
	andeq	r0,r0,r0

l00004EA1:
	andeq	r0,r0,r0

l00004EA5:
	andeq	r0,r0,r0

l00004EA9:
	andeq	r0,r0,r0

l00004EAD:
	andeq	r0,r0,r0

l00004EB1:
	andeq	r0,r0,r0

l00004EB5:
	andeq	r0,r0,r0

l00004EB9:
	andeq	r0,r0,r0

l00004EBD:
	andeq	r0,r0,r0

l00004EC1:
	andeq	r0,r0,r0

l00004EC5:
	andeq	r0,r0,r0

l00004EC9:
	andeq	r0,r0,r0

l00004ECD:
	andeq	r0,r0,r0

l00004ED1:
	andeq	r0,r0,r0

l00004ED5:
	andeq	r0,r0,r0

l00004ED9:
	andeq	r0,r0,r0

l00004EDD:
	andeq	r0,r0,r0

l00004EE1:
	andeq	r0,r0,r0

l00004EE5:
	andeq	r0,r0,r0

l00004EE9:
	andeq	r0,r0,r0

l00004EED:
	andeq	r0,r0,r0

l00004EF1:
	andeq	r0,r0,r0

l00004EF5:
	andeq	r0,r0,r0

l00004EF9:
	andeq	r0,r0,r0

l00004EFD:
	andeq	r0,r0,r0

l00004F01:
	andeq	r0,r0,r0

l00004F05:
	andeq	r0,r0,r0

l00004F09:
	andeq	r0,r0,r0

l00004F0D:
	andeq	r0,r0,r0

l00004F11:
	andeq	r0,r0,r0

l00004F15:
	andeq	r0,r0,r0

l00004F19:
	andeq	r0,r0,r0

l00004F1D:
	andeq	r0,r0,r0

l00004F21:
	andeq	r0,r0,r0

l00004F25:
	andeq	r0,r0,r0

l00004F29:
	andeq	r0,r0,r0

l00004F2D:
	andeq	r0,r0,r0

l00004F31:
	andeq	r0,r0,r0

l00004F35:
	andeq	r0,r0,r0

l00004F39:
	andeq	r0,r0,r0

l00004F3D:
	andeq	r0,r0,r0

l00004F41:
	andeq	r0,r0,r0

l00004F45:
	andeq	r0,r0,r0

l00004F49:
	andeq	r0,r0,r0

l00004F4D:
	andeq	r0,r0,r0

l00004F51:
	andeq	r0,r0,r0

l00004F55:
	andeq	r0,r0,r0

l00004F59:
	andeq	r0,r0,r0

l00004F5D:
	andeq	r0,r0,r0

l00004F61:
	andeq	r0,r0,r0

l00004F65:
	andeq	r0,r0,r0

l00004F69:
	andeq	r0,r0,r0

l00004F6D:
	andeq	r0,r0,r0

l00004F71:
	andeq	r0,r0,r0

l00004F75:
	andeq	r0,r0,r0

l00004F79:
	andeq	r0,r0,r0

l00004F7D:
	andeq	r0,r0,r0

l00004F81:
	andeq	r0,r0,r0

l00004F85:
	andeq	r0,r0,r0

l00004F89:
	andeq	r0,r0,r0

l00004F8D:
	andeq	r0,r0,r0

l00004F91:
	andeq	r0,r0,r0

l00004F95:
	andeq	r0,r0,r0

l00004F99:
	andeq	r0,r0,r0

l00004F9D:
	andeq	r0,r0,r0

l00004FA1:
	andeq	r0,r0,r0

l00004FA5:
	andeq	r0,r0,r0

l00004FA9:
	andeq	r0,r0,r0

l00004FAD:
	andeq	r0,r0,r0

l00004FB1:
	andeq	r0,r0,r0

l00004FB5:
	andeq	r0,r0,r0

l00004FB9:
	andeq	r0,r0,r0

l00004FBD:
	andeq	r0,r0,r0

l00004FC1:
	andeq	r0,r0,r0

l00004FC5:
	andeq	r0,r0,r0

l00004FC9:
	andeq	r0,r0,r0

l00004FCD:
	andeq	r0,r0,r0

l00004FD1:
	andeq	r0,r0,r0

l00004FD5:
	andeq	r0,r0,r0

l00004FD9:
	andeq	r0,r0,r0

l00004FDD:
	andeq	r0,r0,r0

l00004FE1:
	andeq	r0,r0,r0

l00004FE5:
	andeq	r0,r0,r0

l00004FE9:
	andeq	r0,r0,r0

l00004FED:
	andeq	r0,r0,r0

l00004FF1:
	andeq	r0,r0,r0

l00004FF5:
	andeq	r0,r0,r0

l00004FF9:
	andeq	r0,r0,r0

l00004FFD:
	andeq	r0,r0,r0

l00005001:
	andeq	r0,r0,r0

l00005005:
	andeq	r0,r0,r0

l00005009:
	andeq	r0,r0,r0

l0000500D:
	andeq	r0,r0,r0

l00005011:
	andeq	r0,r0,r0

l00005015:
	andeq	r0,r0,r0

l00005019:
	andeq	r0,r0,r0

l0000501D:
	andeq	r0,r0,r0

l00005021:
	andeq	r0,r0,r0

l00005025:
	andeq	r0,r0,r0

l00005029:
	andeq	r0,r0,r0

l0000502D:
	andeq	r0,r0,r0

l00005031:
	andeq	r0,r0,r0

l00005035:
	andeq	r0,r0,r0

l00005039:
	andeq	r0,r0,r0

l0000503D:
	andeq	r0,r0,r0

l00005041:
	andeq	r0,r0,r0

l00005045:
	andeq	r0,r0,r0

l00005049:
	andeq	r0,r0,r0

l0000504D:
	andeq	r0,r0,r0

l00005051:
	andeq	r0,r0,r0

l00005055:
	andeq	r0,r0,r0

l00005059:
	andeq	r0,r0,r0

l0000505D:
	andeq	r0,r0,r0

l00005061:
	andeq	r0,r0,r0

l00005065:
	andeq	r0,r0,r0

l00005069:
	andeq	r0,r0,r0

l0000506D:
	andeq	r0,r0,r0

l00005071:
	andeq	r0,r0,r0

l00005075:
	andeq	r0,r0,r0

l00005079:
	andeq	r0,r0,r0

l0000507D:
	andeq	r0,r0,r0

l00005081:
	andeq	r0,r0,r0

l00005085:
	andeq	r0,r0,r0

l00005089:
	andeq	r0,r0,r0

l0000508D:
	andeq	r0,r0,r0

l00005091:
	andeq	r0,r0,r0

l00005095:
	andeq	r0,r0,r0

l00005099:
	andeq	r0,r0,r0

l0000509D:
	andeq	r0,r0,r0

l000050A1:
	andeq	r0,r0,r0

l000050A5:
	andeq	r0,r0,r0

l000050A9:
	andeq	r0,r0,r0

l000050AD:
	andeq	r0,r0,r0

l000050B1:
	andeq	r0,r0,r0

l000050B5:
	andeq	r0,r0,r0

l000050B9:
	andeq	r0,r0,r0

l000050BD:
	andeq	r0,r0,r0

l000050C1:
	andeq	r0,r0,r0

l000050C5:
	andeq	r0,r0,r0

l000050C9:
	andeq	r0,r0,r0

l000050CD:
	andeq	r0,r0,r0

l000050D1:
	andeq	r0,r0,r0

l000050D5:
	andeq	r0,r0,r0

l000050D9:
	andeq	r0,r0,r0

l000050DD:
	andeq	r0,r0,r0

l000050E1:
	andeq	r0,r0,r0

l000050E5:
	andeq	r0,r0,r0

l000050E9:
	andeq	r0,r0,r0

l000050ED:
	andeq	r0,r0,r0

l000050F1:
	andeq	r0,r0,r0

l000050F5:
	andeq	r0,r0,r0

l000050F9:
	andeq	r0,r0,r0

l000050FD:
	andeq	r0,r0,r0

l00005101:
	andeq	r0,r0,r0

l00005105:
	andeq	r0,r0,r0

l00005109:
	andeq	r0,r0,r0

l0000510D:
	andeq	r0,r0,r0

l00005111:
	andeq	r0,r0,r0

l00005115:
	andeq	r0,r0,r0

l00005119:
	andeq	r0,r0,r0

l0000511D:
	andeq	r0,r0,r0

l00005121:
	andeq	r0,r0,r0

l00005125:
	andeq	r0,r0,r0

l00005129:
	andeq	r0,r0,r0

l0000512D:
	andeq	r0,r0,r0

l00005131:
	andeq	r0,r0,r0

l00005135:
	andeq	r0,r0,r0

l00005139:
	andeq	r0,r0,r0

l0000513D:
	andeq	r0,r0,r0

l00005141:
	andeq	r0,r0,r0

l00005145:
	andeq	r0,r0,r0

l00005149:
	andeq	r0,r0,r0

l0000514D:
	andeq	r0,r0,r0

l00005151:
	andeq	r0,r0,r0

l00005155:
	andeq	r0,r0,r0

l00005159:
	andeq	r0,r0,r0

l0000515D:
	andeq	r0,r0,r0

l00005161:
	andeq	r0,r0,r0

l00005165:
	andeq	r0,r0,r0

l00005169:
	andeq	r0,r0,r0

l0000516D:
	andeq	r0,r0,r0

l00005171:
	andeq	r0,r0,r0

l00005175:
	andeq	r0,r0,r0

l00005179:
	andeq	r0,r0,r0

l0000517D:
	andeq	r0,r0,r0

l00005181:
	andeq	r0,r0,r0

l00005185:
	andeq	r0,r0,r0

l00005189:
	andeq	r0,r0,r0

l0000518D:
	andeq	r0,r0,r0

l00005191:
	andeq	r0,r0,r0

l00005195:
	andeq	r0,r0,r0

l00005199:
	andeq	r0,r0,r0

l0000519D:
	andeq	r0,r0,r0

l000051A1:
	andeq	r0,r0,r0

l000051A5:
	andeq	r0,r0,r0

l000051A9:
	andeq	r0,r0,r0

l000051AD:
	andeq	r0,r0,r0

l000051B1:
	andeq	r0,r0,r0

l000051B5:
	andeq	r0,r0,r0

l000051B9:
	andeq	r0,r0,r0

l000051BD:
	andeq	r0,r0,r0

l000051C1:
	andeq	r0,r0,r0

l000051C5:
	andeq	r0,r0,r0

l000051C9:
	andeq	r0,r0,r0

l000051CD:
	andeq	r0,r0,r0

l000051D1:
	andeq	r0,r0,r0

l000051D5:
	andeq	r0,r0,r0

l000051D9:
	andeq	r0,r0,r0

l000051DD:
	andeq	r0,r0,r0

l000051E1:
	andeq	r0,r0,r0

l000051E5:
	andeq	r0,r0,r0

l000051E9:
	andeq	r0,r0,r0

l000051ED:
	andeq	r0,r0,r0

l000051F1:
	andeq	r0,r0,r0

l000051F5:
	andeq	r0,r0,r0

l000051F9:
	andeq	r0,r0,r0

l000051FD:
	andeq	r0,r0,r0

l00005201:
	andeq	r0,r0,r0

l00005205:
	andeq	r0,r0,r0

l00005209:
	andeq	r0,r0,r0

l0000520D:
	andeq	r0,r0,r0

l00005211:
	andeq	r0,r0,r0

l00005215:
	andeq	r0,r0,r0

l00005219:
	andeq	r0,r0,r0

l0000521D:
	andeq	r0,r0,r0

l00005221:
	andeq	r0,r0,r0

l00005225:
	andeq	r0,r0,r0

l00005229:
	andeq	r0,r0,r0

l0000522D:
	andeq	r0,r0,r0

l00005231:
	andeq	r0,r0,r0

l00005235:
	andeq	r0,r0,r0

l00005239:
	andeq	r0,r0,r0

l0000523D:
	andeq	r0,r0,r0

l00005241:
	andeq	r0,r0,r0

l00005245:
	andeq	r0,r0,r0

l00005249:
	andeq	r0,r0,r0

l0000524D:
	andeq	r0,r0,r0

l00005251:
	andeq	r0,r0,r0

l00005255:
	andeq	r0,r0,r0

l00005259:
	andeq	r0,r0,r0

l0000525D:
	andeq	r0,r0,r0

l00005261:
	andeq	r0,r0,r0

l00005265:
	andeq	r0,r0,r0

l00005269:
	andeq	r0,r0,r0

l0000526D:
	andeq	r0,r0,r0

l00005271:
	andeq	r0,r0,r0

l00005275:
	andeq	r0,r0,r0

l00005279:
	andeq	r0,r0,r0

l0000527D:
	andeq	r0,r0,r0

l00005281:
	andeq	r0,r0,r0

l00005285:
	andeq	r0,r0,r0

l00005289:
	andeq	r0,r0,r0

l0000528D:
	andeq	r0,r0,r0

l00005291:
	andeq	r0,r0,r0

l00005295:
	andeq	r0,r0,r0

l00005299:
	andeq	r0,r0,r0

l0000529D:
	andeq	r0,r0,r0

l000052A1:
	andeq	r0,r0,r0

l000052A5:
	andeq	r0,r0,r0

l000052A9:
	andeq	r0,r0,r0

l000052AD:
	andeq	r0,r0,r0

l000052B1:
	andeq	r0,r0,r0

l000052B5:
	andeq	r0,r0,r0

l000052B9:
	andeq	r0,r0,r0

l000052BD:
	andeq	r0,r0,r0

l000052C1:
	andeq	r0,r0,r0

l000052C5:
	andeq	r0,r0,r0

l000052C9:
	andeq	r0,r0,r0

l000052CD:
	andeq	r0,r0,r0

l000052D1:
	andeq	r0,r0,r0

l000052D5:
	andeq	r0,r0,r0

l000052D9:
	andeq	r0,r0,r0

l000052DD:
	andeq	r0,r0,r0

l000052E1:
	andeq	r0,r0,r0

l000052E5:
	andeq	r0,r0,r0

l000052E9:
	andeq	r0,r0,r0

l000052ED:
	andeq	r0,r0,r0

l000052F1:
	andeq	r0,r0,r0

l000052F5:
	andeq	r0,r0,r0

l000052F9:
	andeq	r0,r0,r0

l000052FD:
	andeq	r0,r0,r0

l00005301:
	andeq	r0,r0,r0

l00005305:
	andeq	r0,r0,r0

l00005309:
	andeq	r0,r0,r0

l0000530D:
	andeq	r0,r0,r0

l00005311:
	andeq	r0,r0,r0

l00005315:
	andeq	r0,r0,r0

l00005319:
	andeq	r0,r0,r0

l0000531D:
	andeq	r0,r0,r0

l00005321:
	andeq	r0,r0,r0

l00005325:
	andeq	r0,r0,r0

l00005329:
	andeq	r0,r0,r0

l0000532D:
	andeq	r0,r0,r0

l00005331:
	andeq	r0,r0,r0

l00005335:
	andeq	r0,r0,r0

l00005339:
	andeq	r0,r0,r0

l0000533D:
	andeq	r0,r0,r0

l00005341:
	andeq	r0,r0,r0

l00005345:
	andeq	r0,r0,r0

l00005349:
	andeq	r0,r0,r0

l0000534D:
	andeq	r0,r0,r0

l00005351:
	andeq	r0,r0,r0

l00005355:
	andeq	r0,r0,r0

l00005359:
	andeq	r0,r0,r0

l0000535D:
	andeq	r0,r0,r0

l00005361:
	andeq	r0,r0,r0

l00005365:
	andeq	r0,r0,r0

l00005369:
	andeq	r0,r0,r0

l0000536D:
	andeq	r0,r0,r0

l00005371:
	andeq	r0,r0,r0

l00005375:
	andeq	r0,r0,r0

l00005379:
	andeq	r0,r0,r0

l0000537D:
	andeq	r0,r0,r0

l00005381:
	andeq	r0,r0,r0

l00005385:
	andeq	r0,r0,r0

l00005389:
	andeq	r0,r0,r0

l0000538D:
	andeq	r0,r0,r0

l00005391:
	andeq	r0,r0,r0

l00005395:
	andeq	r0,r0,r0

l00005399:
	andeq	r0,r0,r0

l0000539D:
	andeq	r0,r0,r0

l000053A1:
	andeq	r0,r0,r0

l000053A5:
	andeq	r0,r0,r0

l000053A9:
	andeq	r0,r0,r0

l000053AD:
	andeq	r0,r0,r0

l000053B1:
	andeq	r0,r0,r0

l000053B5:
	andeq	r0,r0,r0

l000053B9:
	andeq	r0,r0,r0

l000053BD:
	andeq	r0,r0,r0

l000053C1:
	andeq	r0,r0,r0

l000053C5:
	andeq	r0,r0,r0

l000053C9:
	andeq	r0,r0,r0

l000053CD:
	andeq	r0,r0,r0

l000053D1:
	andeq	r0,r0,r0

l000053D5:
	andeq	r0,r0,r0

l000053D9:
	andeq	r0,r0,r0

l000053DD:
	andeq	r0,r0,r0

l000053E1:
	andeq	r0,r0,r0

l000053E5:
	andeq	r0,r0,r0

l000053E9:
	andeq	r0,r0,r0

l000053ED:
	andeq	r0,r0,r0

l000053F1:
	andeq	r0,r0,r0

l000053F5:
	andeq	r0,r0,r0

l000053F9:
	andeq	r0,r0,r0

l000053FD:
	andeq	r0,r0,r0

l00005401:
	andeq	r0,r0,r0

l00005405:
	andeq	r0,r0,r0

l00005409:
	andeq	r0,r0,r0

l0000540D:
	andeq	r0,r0,r0

l00005411:
	andeq	r0,r0,r0

l00005415:
	andeq	r0,r0,r0

l00005419:
	andeq	r0,r0,r0

l0000541D:
	andeq	r0,r0,r0

l00005421:
	andeq	r0,r0,r0

l00005425:
	andeq	r0,r0,r0

l00005429:
	andeq	r0,r0,r0

l0000542D:
	andeq	r0,r0,r0

l00005431:
	andeq	r0,r0,r0

l00005435:
	andeq	r0,r0,r0

l00005439:
	andeq	r0,r0,r0

l0000543D:
	andeq	r0,r0,r0

l00005441:
	andeq	r0,r0,r0

l00005445:
	andeq	r0,r0,r0

l00005449:
	andeq	r0,r0,r0

l0000544D:
	andeq	r0,r0,r0

l00005451:
	andeq	r0,r0,r0

l00005455:
	andeq	r0,r0,r0

l00005459:
	andeq	r0,r0,r0

l0000545D:
	andeq	r0,r0,r0

l00005461:
	andeq	r0,r0,r0

l00005465:
	andeq	r0,r0,r0

l00005469:
	andeq	r0,r0,r0

l0000546D:
	andeq	r0,r0,r0

l00005471:
	andeq	r0,r0,r0

l00005475:
	andeq	r0,r0,r0

l00005479:
	andeq	r0,r0,r0

l0000547D:
	andeq	r0,r0,r0

l00005481:
	andeq	r0,r0,r0

l00005485:
	andeq	r0,r0,r0

l00005489:
	andeq	r0,r0,r0

l0000548D:
	andeq	r0,r0,r0

l00005491:
	andeq	r0,r0,r0

l00005495:
	andeq	r0,r0,r0

l00005499:
	andeq	r0,r0,r0

l0000549D:
	andeq	r0,r0,r0

l000054A1:
	andeq	r0,r0,r0

l000054A5:
	andeq	r0,r0,r0

l000054A9:
	andeq	r0,r0,r0

l000054AD:
	andeq	r0,r0,r0

l000054B1:
	andeq	r0,r0,r0

l000054B5:
	andeq	r0,r0,r0

l000054B9:
	andeq	r0,r0,r0

l000054BD:
	andeq	r0,r0,r0

l000054C1:
	andeq	r0,r0,r0

l000054C5:
	andeq	r0,r0,r0

l000054C9:
	andeq	r0,r0,r0

l000054CD:
	andeq	r0,r0,r0

l000054D1:
	andeq	r0,r0,r0

l000054D5:
	andeq	r0,r0,r0

l000054D9:
	andeq	r0,r0,r0

l000054DD:
	andeq	r0,r0,r0

l000054E1:
	andeq	r0,r0,r0

l000054E5:
	andeq	r0,r0,r0

l000054E9:
	andeq	r0,r0,r0

l000054ED:
	andeq	r0,r0,r0

l000054F1:
	andeq	r0,r0,r0

l000054F5:
	andeq	r0,r0,r0

l000054F9:
	andeq	r0,r0,r0

l000054FD:
	andeq	r0,r0,r0

l00005501:
	andeq	r0,r0,r0

l00005505:
	andeq	r0,r0,r0

l00005509:
	andeq	r0,r0,r0

l0000550D:
	andeq	r0,r0,r0

l00005511:
	andeq	r0,r0,r0

l00005515:
	andeq	r0,r0,r0

l00005519:
	andeq	r0,r0,r0

l0000551D:
	andeq	r0,r0,r0

l00005521:
	andeq	r0,r0,r0

l00005525:
	andeq	r0,r0,r0

l00005529:
	andeq	r0,r0,r0

l0000552D:
	andeq	r0,r0,r0

l00005531:
	andeq	r0,r0,r0

l00005535:
	andeq	r0,r0,r0

l00005539:
	andeq	r0,r0,r0

l0000553D:
	andeq	r0,r0,r0

l00005541:
	andeq	r0,r0,r0

l00005545:
	andeq	r0,r0,r0

l00005549:
	andeq	r0,r0,r0

l0000554D:
	andeq	r0,r0,r0

l00005551:
	andeq	r0,r0,r0

l00005555:
	andeq	r0,r0,r0

l00005559:
	andeq	r0,r0,r0

l0000555D:
	andeq	r0,r0,r0

l00005561:
	andeq	r0,r0,r0

l00005565:
	andeq	r0,r0,r0

l00005569:
	andeq	r0,r0,r0

l0000556D:
	andeq	r0,r0,r0

l00005571:
	andeq	r0,r0,r0

l00005575:
	andeq	r0,r0,r0

l00005579:
	andeq	r0,r0,r0

l0000557D:
	andeq	r0,r0,r0

l00005581:
	andeq	r0,r0,r0

l00005585:
	andeq	r0,r0,r0

l00005589:
	andeq	r0,r0,r0

l0000558D:
	andeq	r0,r0,r0

l00005591:
	andeq	r0,r0,r0

l00005595:
	andeq	r0,r0,r0

l00005599:
	andeq	r0,r0,r0

l0000559D:
	andeq	r0,r0,r0

l000055A1:
	andeq	r0,r0,r0

l000055A5:
	andeq	r0,r0,r0

l000055A9:
	andeq	r0,r0,r0

l000055AD:
	andeq	r0,r0,r0

l000055B1:
	andeq	r0,r0,r0

l000055B5:
	andeq	r0,r0,r0

l000055B9:
	andeq	r0,r0,r0

l000055BD:
	andeq	r0,r0,r0

l000055C1:
	andeq	r0,r0,r0

l000055C5:
	andeq	r0,r0,r0

l000055C9:
	andeq	r0,r0,r0

l000055CD:
	andeq	r0,r0,r0

l000055D1:
	andeq	r0,r0,r0

l000055D5:
	andeq	r0,r0,r0

l000055D9:
	andeq	r0,r0,r0

l000055DD:
	andeq	r0,r0,r0

l000055E1:
	andeq	r0,r0,r0

l000055E5:
	andeq	r0,r0,r0

l000055E9:
	andeq	r0,r0,r0

l000055ED:
	andeq	r0,r0,r0

l000055F1:
	andeq	r0,r0,r0

l000055F5:
	andeq	r0,r0,r0

l000055F9:
	andeq	r0,r0,r0

l000055FD:
	andeq	r0,r0,r0

l00005601:
	andeq	r0,r0,r0

l00005605:
	andeq	r0,r0,r0

l00005609:
	andeq	r0,r0,r0

l0000560D:
	andeq	r0,r0,r0

l00005611:
	andeq	r0,r0,r0

l00005615:
	andeq	r0,r0,r0

l00005619:
	andeq	r0,r0,r0

l0000561D:
	andeq	r0,r0,r0

l00005621:
	andeq	r0,r0,r0

l00005625:
	andeq	r0,r0,r0

l00005629:
	andeq	r0,r0,r0

l0000562D:
	andeq	r0,r0,r0

l00005631:
	andeq	r0,r0,r0

l00005635:
	andeq	r0,r0,r0

l00005639:
	andeq	r0,r0,r0

l0000563D:
	andeq	r0,r0,r0

l00005641:
	andeq	r0,r0,r0

l00005645:
	andeq	r0,r0,r0

l00005649:
	andeq	r0,r0,r0

l0000564D:
	andeq	r0,r0,r0

l00005651:
	andeq	r0,r0,r0

l00005655:
	andeq	r0,r0,r0

l00005659:
	andeq	r0,r0,r0

l0000565D:
	andeq	r0,r0,r0

l00005661:
	andeq	r0,r0,r0

l00005665:
	andeq	r0,r0,r0

l00005669:
	andeq	r0,r0,r0

l0000566D:
	andeq	r0,r0,r0

l00005671:
	andeq	r0,r0,r0

l00005675:
	andeq	r0,r0,r0

l00005679:
	andeq	r0,r0,r0

l0000567D:
	andeq	r0,r0,r0

l00005681:
	andeq	r0,r0,r0

l00005685:
	andeq	r0,r0,r0

l00005689:
	andeq	r0,r0,r0

l0000568D:
	andeq	r0,r0,r0

l00005691:
	andeq	r0,r0,r0

l00005695:
	andeq	r0,r0,r0

l00005699:
	andeq	r0,r0,r0

l0000569D:
	andeq	r0,r0,r0

l000056A1:
	andeq	r0,r0,r0

l000056A5:
	andeq	r0,r0,r0

l000056A9:
	andeq	r0,r0,r0

l000056AD:
	andeq	r0,r0,r0

l000056B1:
	andeq	r0,r0,r0

l000056B5:
	andeq	r0,r0,r0

l000056B9:
	andeq	r0,r0,r0

l000056BD:
	andeq	r0,r0,r0

l000056C1:
	andeq	r0,r0,r0

l000056C5:
	andeq	r0,r0,r0

l000056C9:
	andeq	r0,r0,r0

l000056CD:
	andeq	r0,r0,r0

l000056D1:
	andeq	r0,r0,r0

l000056D5:
	andeq	r0,r0,r0

l000056D9:
	andeq	r0,r0,r0

l000056DD:
	andeq	r0,r0,r0

l000056E1:
	andeq	r0,r0,r0

l000056E5:
	andeq	r0,r0,r0

l000056E9:
	andeq	r0,r0,r0

l000056ED:
	andeq	r0,r0,r0

l000056F1:
	andeq	r0,r0,r0

l000056F5:
	andeq	r0,r0,r0

l000056F9:
	andeq	r0,r0,r0

l000056FD:
	andeq	r0,r0,r0

l00005701:
	andeq	r0,r0,r0

l00005705:
	andeq	r0,r0,r0

l00005709:
	andeq	r0,r0,r0

l0000570D:
	andeq	r0,r0,r0

l00005711:
	andeq	r0,r0,r0

l00005715:
	andeq	r0,r0,r0

l00005719:
	andeq	r0,r0,r0

l0000571D:
	andeq	r0,r0,r0

l00005721:
	andeq	r0,r0,r0

l00005725:
	andeq	r0,r0,r0

l00005729:
	andeq	r0,r0,r0

l0000572D:
	andeq	r0,r0,r0

l00005731:
	andeq	r0,r0,r0

l00005735:
	andeq	r0,r0,r0

l00005739:
	andeq	r0,r0,r0

l0000573D:
	andeq	r0,r0,r0

l00005741:
	andeq	r0,r0,r0

l00005745:
	andeq	r0,r0,r0

l00005749:
	andeq	r0,r0,r0

l0000574D:
	andeq	r0,r0,r0

l00005751:
	andeq	r0,r0,r0

l00005755:
	andeq	r0,r0,r0

l00005759:
	andeq	r0,r0,r0

l0000575D:
	andeq	r0,r0,r0

l00005761:
	andeq	r0,r0,r0

l00005765:
	andeq	r0,r0,r0

l00005769:
	andeq	r0,r0,r0

l0000576D:
	andeq	r0,r0,r0

l00005771:
	andeq	r0,r0,r0

l00005775:
	andeq	r0,r0,r0

l00005779:
	andeq	r0,r0,r0

l0000577D:
	andeq	r0,r0,r0

l00005781:
	andeq	r0,r0,r0

l00005785:
	andeq	r0,r0,r0

l00005789:
	andeq	r0,r0,r0

l0000578D:
	andeq	r0,r0,r0

l00005791:
	andeq	r0,r0,r0

l00005795:
	andeq	r0,r0,r0

l00005799:
	andeq	r0,r0,r0

l0000579D:
	andeq	r0,r0,r0

l000057A1:
	andeq	r0,r0,r0

l000057A5:
	andeq	r0,r0,r0

l000057A9:
	andeq	r0,r0,r0

l000057AD:
	andeq	r0,r0,r0

l000057B1:
	andeq	r0,r0,r0

l000057B5:
	andeq	r0,r0,r0

l000057B9:
	andeq	r0,r0,r0

l000057BD:
	andeq	r0,r0,r0

l000057C1:
	andeq	r0,r0,r0

l000057C5:
	andeq	r0,r0,r0

l000057C9:
	andeq	r0,r0,r0

l000057CD:
	andeq	r0,r0,r0

l000057D1:
	andeq	r0,r0,r0

l000057D5:
	andeq	r0,r0,r0

l000057D9:
	andeq	r0,r0,r0

l000057DD:
	andeq	r0,r0,r0

l000057E1:
	andeq	r0,r0,r0

l000057E5:
	andeq	r0,r0,r0

l000057E9:
	andeq	r0,r0,r0

l000057ED:
	andeq	r0,r0,r0

l000057F1:
	andeq	r0,r0,r0

l000057F5:
	andeq	r0,r0,r0

l000057F9:
	andeq	r0,r0,r0

l000057FD:
	andeq	r0,r0,r0

l00005801:
	andeq	r0,r0,r0

l00005805:
	andeq	r0,r0,r0

l00005809:
	andeq	r0,r0,r0

l0000580D:
	andeq	r0,r0,r0

l00005811:
	andeq	r0,r0,r0

l00005815:
	andeq	r0,r0,r0

l00005819:
	andeq	r0,r0,r0

l0000581D:
	andeq	r0,r0,r0

l00005821:
	andeq	r0,r0,r0

l00005825:
	andeq	r0,r0,r0

l00005829:
	andeq	r0,r0,r0

l0000582D:
	andeq	r0,r0,r0

l00005831:
	andeq	r0,r0,r0

l00005835:
	andeq	r0,r0,r0

l00005839:
	andeq	r0,r0,r0

l0000583D:
	andeq	r0,r0,r0

l00005841:
	andeq	r0,r0,r0

l00005845:
	andeq	r0,r0,r0

l00005849:
	andeq	r0,r0,r0

l0000584D:
	andeq	r0,r0,r0

l00005851:
	andeq	r0,r0,r0

l00005855:
	andeq	r0,r0,r0

l00005859:
	andeq	r0,r0,r0

l0000585D:
	andeq	r0,r0,r0

l00005861:
	andeq	r0,r0,r0

l00005865:
	andeq	r0,r0,r0

l00005869:
	andeq	r0,r0,r0

l0000586D:
	andeq	r0,r0,r0

l00005871:
	andeq	r0,r0,r0

l00005875:
	andeq	r0,r0,r0

l00005879:
	andeq	r0,r0,r0

l0000587D:
	andeq	r0,r0,r0

l00005881:
	andeq	r0,r0,r0

l00005885:
	andeq	r0,r0,r0

l00005889:
	andeq	r0,r0,r0

l0000588D:
	andeq	r0,r0,r0

l00005891:
	andeq	r0,r0,r0

l00005895:
	andeq	r0,r0,r0

l00005899:
	andeq	r0,r0,r0

l0000589D:
	andeq	r0,r0,r0

l000058A1:
	andeq	r0,r0,r0

l000058A5:
	andeq	r0,r0,r0

l000058A9:
	andeq	r0,r0,r0

l000058AD:
	andeq	r0,r0,r0

l000058B1:
	andeq	r0,r0,r0

l000058B5:
	andeq	r0,r0,r0

l000058B9:
	andeq	r0,r0,r0

l000058BD:
	andeq	r0,r0,r0

l000058C1:
	andeq	r0,r0,r0

l000058C5:
	andeq	r0,r0,r0

l000058C9:
	andeq	r0,r0,r0

l000058CD:
	andeq	r0,r0,r0

l000058D1:
	andeq	r0,r0,r0

l000058D5:
	andeq	r0,r0,r0

l000058D9:
	andeq	r0,r0,r0

l000058DD:
	andeq	r0,r0,r0

l000058E1:
	andeq	r0,r0,r0

l000058E5:
	andeq	r0,r0,r0

l000058E9:
	andeq	r0,r0,r0

l000058ED:
	andeq	r0,r0,r0

l000058F1:
	andeq	r0,r0,r0

l000058F5:
	andeq	r0,r0,r0

l000058F9:
	andeq	r0,r0,r0

l000058FD:
	andeq	r0,r0,r0

l00005901:
	andeq	r0,r0,r0

l00005905:
	andeq	r0,r0,r0

l00005909:
	andeq	r0,r0,r0

l0000590D:
	andeq	r0,r0,r0

l00005911:
	andeq	r0,r0,r0

l00005915:
	andeq	r0,r0,r0

l00005919:
	andeq	r0,r0,r0

l0000591D:
	andeq	r0,r0,r0

l00005921:
	andeq	r0,r0,r0

l00005925:
	andeq	r0,r0,r0

l00005929:
	andeq	r0,r0,r0

l0000592D:
	andeq	r0,r0,r0

l00005931:
	andeq	r0,r0,r0

l00005935:
	andeq	r0,r0,r0

l00005939:
	andeq	r0,r0,r0

l0000593D:
	andeq	r0,r0,r0

l00005941:
	andeq	r0,r0,r0

l00005945:
	andeq	r0,r0,r0

l00005949:
	andeq	r0,r0,r0

l0000594D:
	andeq	r0,r0,r0

l00005951:
	andeq	r0,r0,r0

l00005955:
	andeq	r0,r0,r0

l00005959:
	andeq	r0,r0,r0

l0000595D:
	andeq	r0,r0,r0

l00005961:
	andeq	r0,r0,r0

l00005965:
	andeq	r0,r0,r0

l00005969:
	andeq	r0,r0,r0

l0000596D:
	andeq	r0,r0,r0

l00005971:
	andeq	r0,r0,r0

l00005975:
	andeq	r0,r0,r0

l00005979:
	andeq	r0,r0,r0

l0000597D:
	andeq	r0,r0,r0

l00005981:
	andeq	r0,r0,r0

l00005985:
	andeq	r0,r0,r0

l00005989:
	andeq	r0,r0,r0

l0000598D:
	andeq	r0,r0,r0

l00005991:
	andeq	r0,r0,r0

l00005995:
	andeq	r0,r0,r0

l00005999:
	andeq	r0,r0,r0

l0000599D:
	andeq	r0,r0,r0

l000059A1:
	andeq	r0,r0,r0

l000059A5:
	andeq	r0,r0,r0

l000059A9:
	andeq	r0,r0,r0

l000059AD:
	andeq	r0,r0,r0

l000059B1:
	andeq	r0,r0,r0

l000059B5:
	andeq	r0,r0,r0

l000059B9:
	andeq	r0,r0,r0

l000059BD:
	andeq	r0,r0,r0

l000059C1:
	andeq	r0,r0,r0

l000059C5:
	andeq	r0,r0,r0

l000059C9:
	andeq	r0,r0,r0

l000059CD:
	andeq	r0,r0,r0

l000059D1:
	andeq	r0,r0,r0

l000059D5:
	andeq	r0,r0,r0

l000059D9:
	andeq	r0,r0,r0

l000059DD:
	andeq	r0,r0,r0

l000059E1:
	andeq	r0,r0,r0

l000059E5:
	andeq	r0,r0,r0

l000059E9:
	andeq	r0,r0,r0

l000059ED:
	andeq	r0,r0,r0

l000059F1:
	andeq	r0,r0,r0

l000059F5:
	andeq	r0,r0,r0

l000059F9:
	andeq	r0,r0,r0

l000059FD:
	andeq	r0,r0,r0

l00005A01:
	andeq	r0,r0,r0

l00005A05:
	andeq	r0,r0,r0

l00005A09:
	andeq	r0,r0,r0

l00005A0D:
	andeq	r0,r0,r0

l00005A11:
	andeq	r0,r0,r0

l00005A15:
	andeq	r0,r0,r0

l00005A19:
	andeq	r0,r0,r0

l00005A1D:
	andeq	r0,r0,r0

l00005A21:
	andeq	r0,r0,r0

l00005A25:
	andeq	r0,r0,r0

l00005A29:
	andeq	r0,r0,r0

l00005A2D:
	andeq	r0,r0,r0

l00005A31:
	andeq	r0,r0,r0

l00005A35:
	andeq	r0,r0,r0

l00005A39:
	andeq	r0,r0,r0

l00005A3D:
	andeq	r0,r0,r0

l00005A41:
	andeq	r0,r0,r0

l00005A45:
	andeq	r0,r0,r0

l00005A49:
	andeq	r0,r0,r0

l00005A4D:
	andeq	r0,r0,r0

l00005A51:
	andeq	r0,r0,r0

l00005A55:
	andeq	r0,r0,r0

l00005A59:
	andeq	r0,r0,r0

l00005A5D:
	andeq	r0,r0,r0

l00005A61:
	andeq	r0,r0,r0

l00005A65:
	andeq	r0,r0,r0

l00005A69:
	andeq	r0,r0,r0

l00005A6D:
	andeq	r0,r0,r0

l00005A71:
	andeq	r0,r0,r0

l00005A75:
	andeq	r0,r0,r0

l00005A79:
	andeq	r0,r0,r0

l00005A7D:
	andeq	r0,r0,r0

l00005A81:
	andeq	r0,r0,r0

l00005A85:
	andeq	r0,r0,r0

l00005A89:
	andeq	r0,r0,r0

l00005A8D:
	andeq	r0,r0,r0

l00005A91:
	andeq	r0,r0,r0

l00005A95:
	andeq	r0,r0,r0

l00005A99:
	andeq	r0,r0,r0

l00005A9D:
	andeq	r0,r0,r0

l00005AA1:
	andeq	r0,r0,r0

l00005AA5:
	andeq	r0,r0,r0

l00005AA9:
	andeq	r0,r0,r0

l00005AAD:
	andeq	r0,r0,r0

l00005AB1:
	andeq	r0,r0,r0

l00005AB5:
	andeq	r0,r0,r0

l00005AB9:
	andeq	r0,r0,r0

l00005ABD:
	andeq	r0,r0,r0

l00005AC1:
	andeq	r0,r0,r0

l00005AC5:
	andeq	r0,r0,r0

l00005AC9:
	andeq	r0,r0,r0

l00005ACD:
	andeq	r0,r0,r0

l00005AD1:
	andeq	r0,r0,r0

l00005AD5:
	andeq	r0,r0,r0

l00005AD9:
	andeq	r0,r0,r0

l00005ADD:
	andeq	r0,r0,r0

l00005AE1:
	andeq	r0,r0,r0

l00005AE5:
	andeq	r0,r0,r0

l00005AE9:
	andeq	r0,r0,r0

l00005AED:
	andeq	r0,r0,r0

l00005AF1:
	andeq	r0,r0,r0

l00005AF5:
	andeq	r0,r0,r0

l00005AF9:
	andeq	r0,r0,r0

l00005AFD:
	andeq	r0,r0,r0

l00005B01:
	andeq	r0,r0,r0

l00005B05:
	andeq	r0,r0,r0

l00005B09:
	andeq	r0,r0,r0

l00005B0D:
	andeq	r0,r0,r0

l00005B11:
	andeq	r0,r0,r0

l00005B15:
	andeq	r0,r0,r0

l00005B19:
	andeq	r0,r0,r0

l00005B1D:
	andeq	r0,r0,r0

l00005B21:
	andeq	r0,r0,r0

l00005B25:
	andeq	r0,r0,r0

l00005B29:
	andeq	r0,r0,r0

l00005B2D:
	andeq	r0,r0,r0

l00005B31:
	andeq	r0,r0,r0

l00005B35:
	andeq	r0,r0,r0

l00005B39:
	andeq	r0,r0,r0

l00005B3D:
	andeq	r0,r0,r0

l00005B41:
	andeq	r0,r0,r0

l00005B45:
	andeq	r0,r0,r0

l00005B49:
	andeq	r0,r0,r0

l00005B4D:
	andeq	r0,r0,r0

l00005B51:
	andeq	r0,r0,r0

l00005B55:
	andeq	r0,r0,r0

l00005B59:
	andeq	r0,r0,r0

l00005B5D:
	andeq	r0,r0,r0

l00005B61:
	andeq	r0,r0,r0

l00005B65:
	andeq	r0,r0,r0

l00005B69:
	andeq	r0,r0,r0

l00005B6D:
	andeq	r0,r0,r0

l00005B71:
	andeq	r0,r0,r0

l00005B75:
	andeq	r0,r0,r0

l00005B79:
	andeq	r0,r0,r0

l00005B7D:
	andeq	r0,r0,r0

l00005B81:
	andeq	r0,r0,r0

l00005B85:
	andeq	r0,r0,r0

l00005B89:
	andeq	r0,r0,r0

l00005B8D:
	andeq	r0,r0,r0

l00005B91:
	andeq	r0,r0,r0

l00005B95:
	andeq	r0,r0,r0

l00005B99:
	andeq	r0,r0,r0

l00005B9D:
	andeq	r0,r0,r0

l00005BA1:
	andeq	r0,r0,r0

l00005BA5:
	andeq	r0,r0,r0

l00005BA9:
	andeq	r0,r0,r0

l00005BAD:
	andeq	r0,r0,r0

l00005BB1:
	andeq	r0,r0,r0

l00005BB5:
	andeq	r0,r0,r0

l00005BB9:
	andeq	r0,r0,r0

l00005BBD:
	andeq	r0,r0,r0

l00005BC1:
	andeq	r0,r0,r0

l00005BC5:
	andeq	r0,r0,r0

l00005BC9:
	andeq	r0,r0,r0

l00005BCD:
	andeq	r0,r0,r0

l00005BD1:
	andeq	r0,r0,r0

l00005BD5:
	andeq	r0,r0,r0

l00005BD9:
	andeq	r0,r0,r0

l00005BDD:
	andeq	r0,r0,r0

l00005BE1:
	andeq	r0,r0,r0

l00005BE5:
	andeq	r0,r0,r0

l00005BE9:
	andeq	r0,r0,r0

l00005BED:
	andeq	r0,r0,r0

l00005BF1:
	andeq	r0,r0,r0

l00005BF5:
	andeq	r0,r0,r0

l00005BF9:
	andeq	r0,r0,r0

l00005BFD:
	andeq	r0,r0,r0

l00005C01:
	andeq	r0,r0,r0

l00005C05:
	andeq	r0,r0,r0

l00005C09:
	andeq	r0,r0,r0

l00005C0D:
	andeq	r0,r0,r0

l00005C11:
	andeq	r0,r0,r0

l00005C15:
	andeq	r0,r0,r0

l00005C19:
	andeq	r0,r0,r0

l00005C1D:
	andeq	r0,r0,r0

l00005C21:
	andeq	r0,r0,r0

l00005C25:
	andeq	r0,r0,r0

l00005C29:
	andeq	r0,r0,r0

l00005C2D:
	andeq	r0,r0,r0

l00005C31:
	andeq	r0,r0,r0

l00005C35:
	andeq	r0,r0,r0

l00005C39:
	andeq	r0,r0,r0

l00005C3D:
	andeq	r0,r0,r0

l00005C41:
	andeq	r0,r0,r0

l00005C45:
	andeq	r0,r0,r0

l00005C49:
	andeq	r0,r0,r0

l00005C4D:
	andeq	r0,r0,r0

l00005C51:
	andeq	r0,r0,r0

l00005C55:
	andeq	r0,r0,r0

l00005C59:
	andeq	r0,r0,r0

l00005C5D:
	andeq	r0,r0,r0

l00005C61:
	andeq	r0,r0,r0

l00005C65:
	andeq	r0,r0,r0

l00005C69:
	andeq	r0,r0,r0

l00005C6D:
	andeq	r0,r0,r0

l00005C71:
	andeq	r0,r0,r0

l00005C75:
	andeq	r0,r0,r0

l00005C79:
	andeq	r0,r0,r0

l00005C7D:
	andeq	r0,r0,r0

l00005C81:
	andeq	r0,r0,r0

l00005C85:
	andeq	r0,r0,r0

l00005C89:
	andeq	r0,r0,r0

l00005C8D:
	andeq	r0,r0,r0

l00005C91:
	andeq	r0,r0,r0

l00005C95:
	andeq	r0,r0,r0

l00005C99:
	andeq	r0,r0,r0

l00005C9D:
	andeq	r0,r0,r0

l00005CA1:
	andeq	r0,r0,r0

l00005CA5:
	andeq	r0,r0,r0

l00005CA9:
	andeq	r0,r0,r0

l00005CAD:
	andeq	r0,r0,r0

l00005CB1:
	andeq	r0,r0,r0

l00005CB5:
	andeq	r0,r0,r0

l00005CB9:
	andeq	r0,r0,r0

l00005CBD:
	andeq	r0,r0,r0

l00005CC1:
	andeq	r0,r0,r0

l00005CC5:
	andeq	r0,r0,r0

l00005CC9:
	andeq	r0,r0,r0

l00005CCD:
	andeq	r0,r0,r0

l00005CD1:
	andeq	r0,r0,r0

l00005CD5:
	andeq	r0,r0,r0

l00005CD9:
	andeq	r0,r0,r0

l00005CDD:
	andeq	r0,r0,r0

l00005CE1:
	andeq	r0,r0,r0

l00005CE5:
	andeq	r0,r0,r0

l00005CE9:
	andeq	r0,r0,r0

l00005CED:
	andeq	r0,r0,r0

l00005CF1:
	andeq	r0,r0,r0

l00005CF5:
	andeq	r0,r0,r0

l00005CF9:
	andeq	r0,r0,r0

l00005CFD:
	andeq	r0,r0,r0

l00005D01:
	andeq	r0,r0,r0

l00005D05:
	andeq	r0,r0,r0

l00005D09:
	andeq	r0,r0,r0

l00005D0D:
	andeq	r0,r0,r0

l00005D11:
	andeq	r0,r0,r0

l00005D15:
	andeq	r0,r0,r0

l00005D19:
	andeq	r0,r0,r0

l00005D1D:
	andeq	r0,r0,r0

l00005D21:
	andeq	r0,r0,r0

l00005D25:
	andeq	r0,r0,r0

l00005D29:
	andeq	r0,r0,r0

l00005D2D:
	andeq	r0,r0,r0

l00005D31:
	andeq	r0,r0,r0

l00005D35:
	andeq	r0,r0,r0

l00005D39:
	andeq	r0,r0,r0

l00005D3D:
	andeq	r0,r0,r0

l00005D41:
	andeq	r0,r0,r0

l00005D45:
	andeq	r0,r0,r0

l00005D49:
	andeq	r0,r0,r0

l00005D4D:
	andeq	r0,r0,r0

l00005D51:
	andeq	r0,r0,r0

l00005D55:
	andeq	r0,r0,r0

l00005D59:
	andeq	r0,r0,r0

l00005D5D:
	andeq	r0,r0,r0

l00005D61:
	andeq	r0,r0,r0

l00005D65:
	andeq	r0,r0,r0

l00005D69:
	andeq	r0,r0,r0

l00005D6D:
	andeq	r0,r0,r0

l00005D71:
	andeq	r0,r0,r0

l00005D75:
	andeq	r0,r0,r0

l00005D79:
	andeq	r0,r0,r0

l00005D7D:
	andeq	r0,r0,r0

l00005D81:
	andeq	r0,r0,r0

l00005D85:
	andeq	r0,r0,r0

l00005D89:
	andeq	r0,r0,r0

l00005D8D:
	andeq	r0,r0,r0

l00005D91:
	andeq	r0,r0,r0

l00005D95:
	andeq	r0,r0,r0

l00005D99:
	andeq	r0,r0,r0

l00005D9D:
	andeq	r0,r0,r0

l00005DA1:
	andeq	r0,r0,r0

l00005DA5:
	andeq	r0,r0,r0

l00005DA9:
	andeq	r0,r0,r0

l00005DAD:
	andeq	r0,r0,r0

l00005DB1:
	andeq	r0,r0,r0

l00005DB5:
	andeq	r0,r0,r0

l00005DB9:
	andeq	r0,r0,r0

l00005DBD:
	andeq	r0,r0,r0

l00005DC1:
	andeq	r0,r0,r0

l00005DC5:
	andeq	r0,r0,r0

l00005DC9:
	andeq	r0,r0,r0

l00005DCD:
	andeq	r0,r0,r0

l00005DD1:
	andeq	r0,r0,r0

l00005DD5:
	andeq	r0,r0,r0

l00005DD9:
	andeq	r0,r0,r0

l00005DDD:
	andeq	r0,r0,r0

l00005DE1:
	andeq	r0,r0,r0

l00005DE5:
	andeq	r0,r0,r0

l00005DE9:
	andeq	r0,r0,r0

l00005DED:
	andeq	r0,r0,r0

l00005DF1:
	andeq	r0,r0,r0

l00005DF5:
	andeq	r0,r0,r0

l00005DF9:
	andeq	r0,r0,r0

l00005DFD:
	andeq	r0,r0,r0

l00005E01:
	andeq	r0,r0,r0

l00005E05:
	andeq	r0,r0,r0

l00005E09:
	andeq	r0,r0,r0

l00005E0D:
	andeq	r0,r0,r0

l00005E11:
	andeq	r0,r0,r0

l00005E15:
	andeq	r0,r0,r0

l00005E19:
	andeq	r0,r0,r0

l00005E1D:
	andeq	r0,r0,r0

l00005E21:
	andeq	r0,r0,r0

l00005E25:
	andeq	r0,r0,r0

l00005E29:
	andeq	r0,r0,r0

l00005E2D:
	andeq	r0,r0,r0

l00005E31:
	andeq	r0,r0,r0

l00005E35:
	andeq	r0,r0,r0

l00005E39:
	andeq	r0,r0,r0

l00005E3D:
	andeq	r0,r0,r0

l00005E41:
	andeq	r0,r0,r0

l00005E45:
	andeq	r0,r0,r0

l00005E49:
	andeq	r0,r0,r0

l00005E4D:
	andeq	r0,r0,r0

l00005E51:
	andeq	r0,r0,r0

l00005E55:
	andeq	r0,r0,r0

l00005E59:
	andeq	r0,r0,r0

l00005E5D:
	andeq	r0,r0,r0

l00005E61:
	andeq	r0,r0,r0

l00005E65:
	andeq	r0,r0,r0

l00005E69:
	andeq	r0,r0,r0

l00005E6D:
	andeq	r0,r0,r0

l00005E71:
	andeq	r0,r0,r0

l00005E75:
	andeq	r0,r0,r0

l00005E79:
	andeq	r0,r0,r0

l00005E7D:
	andeq	r0,r0,r0

l00005E81:
	andeq	r0,r0,r0

l00005E85:
	andeq	r0,r0,r0

l00005E89:
	andeq	r0,r0,r0

l00005E8D:
	andeq	r0,r0,r0

l00005E91:
	andeq	r0,r0,r0

l00005E95:
	andeq	r0,r0,r0

l00005E99:
	andeq	r0,r0,r0

l00005E9D:
	andeq	r0,r0,r0

l00005EA1:
	andeq	r0,r0,r0

l00005EA5:
	andeq	r0,r0,r0

l00005EA9:
	andeq	r0,r0,r0

l00005EAD:
	andeq	r0,r0,r0

l00005EB1:
	andeq	r0,r0,r0

l00005EB5:
	andeq	r0,r0,r0

l00005EB9:
	andeq	r0,r0,r0

l00005EBD:
	andeq	r0,r0,r0

l00005EC1:
	andeq	r0,r0,r0

l00005EC5:
	andeq	r0,r0,r0

l00005EC9:
	andeq	r0,r0,r0

l00005ECD:
	andeq	r0,r0,r0

l00005ED1:
	andeq	r0,r0,r0

l00005ED5:
	andeq	r0,r0,r0

l00005ED9:
	andeq	r0,r0,r0

l00005EDD:
	andeq	r0,r0,r0

l00005EE1:
	andeq	r0,r0,r0

l00005EE5:
	andeq	r0,r0,r0

l00005EE9:
	andeq	r0,r0,r0

l00005EED:
	andeq	r0,r0,r0

l00005EF1:
	andeq	r0,r0,r0

l00005EF5:
	andeq	r0,r0,r0

l00005EF9:
	andeq	r0,r0,r0

l00005EFD:
	andeq	r0,r0,r0

l00005F01:
	andeq	r0,r0,r0

l00005F05:
	andeq	r0,r0,r0

l00005F09:
	andeq	r0,r0,r0

l00005F0D:
	andeq	r0,r0,r0

l00005F11:
	andeq	r0,r0,r0

l00005F15:
	andeq	r0,r0,r0

l00005F19:
	andeq	r0,r0,r0

l00005F1D:
	andeq	r0,r0,r0

l00005F21:
	andeq	r0,r0,r0

l00005F25:
	andeq	r0,r0,r0

l00005F29:
	andeq	r0,r0,r0

l00005F2D:
	andeq	r0,r0,r0

l00005F31:
	andeq	r0,r0,r0

l00005F35:
	andeq	r0,r0,r0

l00005F39:
	andeq	r0,r0,r0

l00005F3D:
	andeq	r0,r0,r0

l00005F41:
	andeq	r0,r0,r0

l00005F45:
	andeq	r0,r0,r0

l00005F49:
	andeq	r0,r0,r0

l00005F4D:
	andeq	r0,r0,r0

l00005F51:
	andeq	r0,r0,r0

l00005F55:
	andeq	r0,r0,r0

l00005F59:
	andeq	r0,r0,r0

l00005F5D:
	andeq	r0,r0,r0

l00005F61:
	andeq	r0,r0,r0

l00005F65:
	andeq	r0,r0,r0

l00005F69:
	andeq	r0,r0,r0

l00005F6D:
	andeq	r0,r0,r0

l00005F71:
	andeq	r0,r0,r0

l00005F75:
	andeq	r0,r0,r0

l00005F79:
	andeq	r0,r0,r0

l00005F7D:
	andeq	r0,r0,r0

l00005F81:
	andeq	r0,r0,r0

l00005F85:
	andeq	r0,r0,r0

l00005F89:
	andeq	r0,r0,r0

l00005F8D:
	andeq	r0,r0,r0

l00005F91:
	andeq	r0,r0,r0

l00005F95:
	andeq	r0,r0,r0

l00005F99:
	andeq	r0,r0,r0

l00005F9D:
	andeq	r0,r0,r0

l00005FA1:
	andeq	r0,r0,r0

l00005FA5:
	andeq	r0,r0,r0

l00005FA9:
	andeq	r0,r0,r0

l00005FAD:
	andeq	r0,r0,r0

l00005FB1:
	andeq	r0,r0,r0

l00005FB5:
	andeq	r0,r0,r0

l00005FB9:
	andeq	r0,r0,r0

l00005FBD:
	andeq	r0,r0,r0

l00005FC1:
	andeq	r0,r0,r0

l00005FC5:
	andeq	r0,r0,r0

l00005FC9:
	andeq	r0,r0,r0

l00005FCD:
	andeq	r0,r0,r0

l00005FD1:
	andeq	r0,r0,r0

l00005FD5:
	andeq	r0,r0,r0

l00005FD9:
	andeq	r0,r0,r0

l00005FDD:
	andeq	r0,r0,r0

l00005FE1:
	andeq	r0,r0,r0

l00005FE5:
	andeq	r0,r0,r0

l00005FE9:
	andeq	r0,r0,r0

l00005FED:
	andeq	r0,r0,r0

l00005FF1:
	andeq	r0,r0,r0

l00005FF5:
	andeq	r0,r0,r0

l00005FF9:
	andeq	r0,r0,r0

l00005FFD:
	andeq	r0,r0,r0

l00006001:
	andeq	r0,r0,r0

l00006005:
	andeq	r0,r0,r0

l00006009:
	andeq	r0,r0,r0

l0000600D:
	andeq	r0,r0,r0

l00006011:
	andeq	r0,r0,r0

l00006015:
	andeq	r0,r0,r0

l00006019:
	andeq	r0,r0,r0

l0000601D:
	andeq	r0,r0,r0

l00006021:
	andeq	r0,r0,r0

l00006025:
	andeq	r0,r0,r0

l00006029:
	andeq	r0,r0,r0

l0000602D:
	andeq	r0,r0,r0

l00006031:
	andeq	r0,r0,r0

l00006035:
	andeq	r0,r0,r0

l00006039:
	andeq	r0,r0,r0

l0000603D:
	andeq	r0,r0,r0

l00006041:
	andeq	r0,r0,r0

l00006045:
	andeq	r0,r0,r0

l00006049:
	andeq	r0,r0,r0

l0000604D:
	andeq	r0,r0,r0

l00006051:
	andeq	r0,r0,r0

l00006055:
	andeq	r0,r0,r0

l00006059:
	andeq	r0,r0,r0

l0000605D:
	andeq	r0,r0,r0

l00006061:
	andeq	r0,r0,r0

l00006065:
	andeq	r0,r0,r0

l00006069:
	andeq	r0,r0,r0

l0000606D:
	andeq	r0,r0,r0

l00006071:
	andeq	r0,r0,r0

l00006075:
	andeq	r0,r0,r0

l00006079:
	andeq	r0,r0,r0

l0000607D:
	andeq	r0,r0,r0

l00006081:
	andeq	r0,r0,r0

l00006085:
	andeq	r0,r0,r0

l00006089:
	andeq	r0,r0,r0

l0000608D:
	andeq	r0,r0,r0

l00006091:
	andeq	r0,r0,r0

l00006095:
	andeq	r0,r0,r0

l00006099:
	andeq	r0,r0,r0

l0000609D:
	andeq	r0,r0,r0

l000060A1:
	andeq	r0,r0,r0

l000060A5:
	andeq	r0,r0,r0

l000060A9:
	andeq	r0,r0,r0

l000060AD:
	andeq	r0,r0,r0

l000060B1:
	andeq	r0,r0,r0

l000060B5:
	andeq	r0,r0,r0

l000060B9:
	andeq	r0,r0,r0

l000060BD:
	andeq	r0,r0,r0

l000060C1:
	andeq	r0,r0,r0

l000060C5:
	andeq	r0,r0,r0

l000060C9:
	andeq	r0,r0,r0

l000060CD:
	andeq	r0,r0,r0

l000060D1:
	andeq	r0,r0,r0

l000060D5:
	andeq	r0,r0,r0

l000060D9:
	andeq	r0,r0,r0

l000060DD:
	andeq	r0,r0,r0

l000060E1:
	andeq	r0,r0,r0

l000060E5:
	andeq	r0,r0,r0

l000060E9:
	andeq	r0,r0,r0

l000060ED:
	andeq	r0,r0,r0

l000060F1:
	andeq	r0,r0,r0

l000060F5:
	andeq	r0,r0,r0

l000060F9:
	andeq	r0,r0,r0

l000060FD:
	andeq	r0,r0,r0

l00006101:
	andeq	r0,r0,r0

l00006105:
	andeq	r0,r0,r0

l00006109:
	andeq	r0,r0,r0

l0000610D:
	andeq	r0,r0,r0

l00006111:
	andeq	r0,r0,r0

l00006115:
	andeq	r0,r0,r0

l00006119:
	andeq	r0,r0,r0

l0000611D:
	andeq	r0,r0,r0

l00006121:
	andeq	r0,r0,r0

l00006125:
	andeq	r0,r0,r0

l00006129:
	andeq	r0,r0,r0

l0000612D:
	andeq	r0,r0,r0

l00006131:
	andeq	r0,r0,r0

l00006135:
	andeq	r0,r0,r0

l00006139:
	andeq	r0,r0,r0

l0000613D:
	andeq	r0,r0,r0

l00006141:
	andeq	r0,r0,r0

l00006145:
	andeq	r0,r0,r0

l00006149:
	andeq	r0,r0,r0

l0000614D:
	andeq	r0,r0,r0

l00006151:
	andeq	r0,r0,r0

l00006155:
	andeq	r0,r0,r0

l00006159:
	andeq	r0,r0,r0

l0000615D:
	andeq	r0,r0,r0

l00006161:
	andeq	r0,r0,r0

l00006165:
	andeq	r0,r0,r0

l00006169:
	andeq	r0,r0,r0

l0000616D:
	andeq	r0,r0,r0

l00006171:
	andeq	r0,r0,r0

l00006175:
	andeq	r0,r0,r0

l00006179:
	andeq	r0,r0,r0

l0000617D:
	andeq	r0,r0,r0

l00006181:
	andeq	r0,r0,r0

l00006185:
	andeq	r0,r0,r0

l00006189:
	andeq	r0,r0,r0

l0000618D:
	andeq	r0,r0,r0

l00006191:
	andeq	r0,r0,r0

l00006195:
	andeq	r0,r0,r0

l00006199:
	andeq	r0,r0,r0

l0000619D:
	andeq	r0,r0,r0

l000061A1:
	andeq	r0,r0,r0

l000061A5:
	andeq	r0,r0,r0

l000061A9:
	andeq	r0,r0,r0

l000061AD:
	andeq	r0,r0,r0

l000061B1:
	andeq	r0,r0,r0

l000061B5:
	andeq	r0,r0,r0

l000061B9:
	andeq	r0,r0,r0

l000061BD:
	andeq	r0,r0,r0

l000061C1:
	andeq	r0,r0,r0

l000061C5:
	andeq	r0,r0,r0

l000061C9:
	andeq	r0,r0,r0

l000061CD:
	andeq	r0,r0,r0

l000061D1:
	andeq	r0,r0,r0

l000061D5:
	andeq	r0,r0,r0

l000061D9:
	andeq	r0,r0,r0

l000061DD:
	andeq	r0,r0,r0

l000061E1:
	andeq	r0,r0,r0

l000061E5:
	andeq	r0,r0,r0

l000061E9:
	andeq	r0,r0,r0

l000061ED:
	andeq	r0,r0,r0

l000061F1:
	andeq	r0,r0,r0

l000061F5:
	andeq	r0,r0,r0

l000061F9:
	andeq	r0,r0,r0

l000061FD:
	andeq	r0,r0,r0

l00006201:
	andeq	r0,r0,r0

l00006205:
	andeq	r0,r0,r0

l00006209:
	andeq	r0,r0,r0

l0000620D:
	andeq	r0,r0,r0

l00006211:
	andeq	r0,r0,r0

l00006215:
	andeq	r0,r0,r0

l00006219:
	andeq	r0,r0,r0

l0000621D:
	andeq	r0,r0,r0

l00006221:
	andeq	r0,r0,r0

l00006225:
	andeq	r0,r0,r0

l00006229:
	andeq	r0,r0,r0

l0000622D:
	andeq	r0,r0,r0

l00006231:
	andeq	r0,r0,r0

l00006235:
	andeq	r0,r0,r0

l00006239:
	andeq	r0,r0,r0

l0000623D:
	andeq	r0,r0,r0

l00006241:
	andeq	r0,r0,r0

l00006245:
	andeq	r0,r0,r0

l00006249:
	andeq	r0,r0,r0

l0000624D:
	andeq	r0,r0,r0

l00006251:
	andeq	r0,r0,r0

l00006255:
	andeq	r0,r0,r0

l00006259:
	andeq	r0,r0,r0

l0000625D:
	andeq	r0,r0,r0

l00006261:
	andeq	r0,r0,r0

l00006265:
	andeq	r0,r0,r0

l00006269:
	andeq	r0,r0,r0

l0000626D:
	andeq	r0,r0,r0

l00006271:
	andeq	r0,r0,r0

l00006275:
	andeq	r0,r0,r0

l00006279:
	andeq	r0,r0,r0

l0000627D:
	andeq	r0,r0,r0

l00006281:
	andeq	r0,r0,r0

l00006285:
	andeq	r0,r0,r0

l00006289:
	andeq	r0,r0,r0

l0000628D:
	andeq	r0,r0,r0

l00006291:
	andeq	r0,r0,r0

l00006295:
	andeq	r0,r0,r0

l00006299:
	andeq	r0,r0,r0

l0000629D:
	andeq	r0,r0,r0

l000062A1:
	andeq	r0,r0,r0

l000062A5:
	andeq	r0,r0,r0

l000062A9:
	andeq	r0,r0,r0

l000062AD:
	andeq	r0,r0,r0

l000062B1:
	andeq	r0,r0,r0

l000062B5:
	andeq	r0,r0,r0

l000062B9:
	andeq	r0,r0,r0

l000062BD:
	andeq	r0,r0,r0

l000062C1:
	andeq	r0,r0,r0

l000062C5:
	andeq	r0,r0,r0

l000062C9:
	andeq	r0,r0,r0

l000062CD:
	andeq	r0,r0,r0

l000062D1:
	andeq	r0,r0,r0

l000062D5:
	andeq	r0,r0,r0

l000062D9:
	andeq	r0,r0,r0

l000062DD:
	andeq	r0,r0,r0

l000062E1:
	andeq	r0,r0,r0

l000062E5:
	andeq	r0,r0,r0

l000062E9:
	andeq	r0,r0,r0

l000062ED:
	andeq	r0,r0,r0

l000062F1:
	andeq	r0,r0,r0

l000062F5:
	andeq	r0,r0,r0

l000062F9:
	andeq	r0,r0,r0

l000062FD:
	andeq	r0,r0,r0

l00006301:
	andeq	r0,r0,r0

l00006305:
	andeq	r0,r0,r0

l00006309:
	andeq	r0,r0,r0

l0000630D:
	andeq	r0,r0,r0

l00006311:
	andeq	r0,r0,r0

l00006315:
	andeq	r0,r0,r0

l00006319:
	andeq	r0,r0,r0

l0000631D:
	andeq	r0,r0,r0

l00006321:
	andeq	r0,r0,r0

l00006325:
	andeq	r0,r0,r0

l00006329:
	andeq	r0,r0,r0

l0000632D:
	andeq	r0,r0,r0

l00006331:
	andeq	r0,r0,r0

l00006335:
	andeq	r0,r0,r0

l00006339:
	andeq	r0,r0,r0

l0000633D:
	andeq	r0,r0,r0

l00006341:
	andeq	r0,r0,r0

l00006345:
	andeq	r0,r0,r0

l00006349:
	andeq	r0,r0,r0

l0000634D:
	andeq	r0,r0,r0

l00006351:
	andeq	r0,r0,r0

l00006355:
	andeq	r0,r0,r0

l00006359:
	andeq	r0,r0,r0

l0000635D:
	andeq	r0,r0,r0

l00006361:
	andeq	r0,r0,r0

l00006365:
	andeq	r0,r0,r0

l00006369:
	andeq	r0,r0,r0

l0000636D:
	andeq	r0,r0,r0

l00006371:
	andeq	r0,r0,r0

l00006375:
	andeq	r0,r0,r0

l00006379:
	andeq	r0,r0,r0

l0000637D:
	andeq	r0,r0,r0

l00006381:
	andeq	r0,r0,r0

l00006385:
	andeq	r0,r0,r0

l00006389:
	andeq	r0,r0,r0

l0000638D:
	andeq	r0,r0,r0

l00006391:
	andeq	r0,r0,r0

l00006395:
	andeq	r0,r0,r0

l00006399:
	andeq	r0,r0,r0

l0000639D:
	andeq	r0,r0,r0

l000063A1:
	andeq	r0,r0,r0

l000063A5:
	andeq	r0,r0,r0

l000063A9:
	andeq	r0,r0,r0

l000063AD:
	andeq	r0,r0,r0

l000063B1:
	andeq	r0,r0,r0

l000063B5:
	andeq	r0,r0,r0

l000063B9:
	andeq	r0,r0,r0

l000063BD:
	andeq	r0,r0,r0

l000063C1:
	andeq	r0,r0,r0

l000063C5:
	andeq	r0,r0,r0

l000063C9:
	andeq	r0,r0,r0

l000063CD:
	andeq	r0,r0,r0

l000063D1:
	andeq	r0,r0,r0

l000063D5:
	andeq	r0,r0,r0

l000063D9:
	andeq	r0,r0,r0

l000063DD:
	andeq	r0,r0,r0

l000063E1:
	andeq	r0,r0,r0

l000063E5:
	andeq	r0,r0,r0

l000063E9:
	andeq	r0,r0,r0

l000063ED:
	andeq	r0,r0,r0

l000063F1:
	andeq	r0,r0,r0

l000063F5:
	andeq	r0,r0,r0

l000063F9:
	andeq	r0,r0,r0

l000063FD:
	andeq	r0,r0,r0

l00006401:
	andeq	r0,r0,r0

l00006405:
	andeq	r0,r0,r0

l00006409:
	andeq	r0,r0,r0

l0000640D:
	andeq	r0,r0,r0

l00006411:
	andeq	r0,r0,r0

l00006415:
	andeq	r0,r0,r0

l00006419:
	andeq	r0,r0,r0

l0000641D:
	andeq	r0,r0,r0

l00006421:
	andeq	r0,r0,r0

l00006425:
	andeq	r0,r0,r0

l00006429:
	andeq	r0,r0,r0

l0000642D:
	andeq	r0,r0,r0

l00006431:
	andeq	r0,r0,r0

l00006435:
	andeq	r0,r0,r0

l00006439:
	andeq	r0,r0,r0

l0000643D:
	andeq	r0,r0,r0

l00006441:
	andeq	r0,r0,r0

l00006445:
	andeq	r0,r0,r0

l00006449:
	andeq	r0,r0,r0

l0000644D:
	andeq	r0,r0,r0

l00006451:
	andeq	r0,r0,r0

l00006455:
	andeq	r0,r0,r0

l00006459:
	andeq	r0,r0,r0

l0000645D:
	andeq	r0,r0,r0

l00006461:
	andeq	r0,r0,r0

l00006465:
	andeq	r0,r0,r0

l00006469:
	andeq	r0,r0,r0

l0000646D:
	andeq	r0,r0,r0

l00006471:
	andeq	r0,r0,r0

l00006475:
	andeq	r0,r0,r0

l00006479:
	andeq	r0,r0,r0

l0000647D:
	andeq	r0,r0,r0

l00006481:
	andeq	r0,r0,r0

l00006485:
	andeq	r0,r0,r0

l00006489:
	andeq	r0,r0,r0

l0000648D:
	andeq	r0,r0,r0

l00006491:
	andeq	r0,r0,r0

l00006495:
	andeq	r0,r0,r0

l00006499:
	andeq	r0,r0,r0

l0000649D:
	andeq	r0,r0,r0

l000064A1:
	andeq	r0,r0,r0

l000064A5:
	andeq	r0,r0,r0

l000064A9:
	andeq	r0,r0,r0

l000064AD:
	andeq	r0,r0,r0

l000064B1:
	andeq	r0,r0,r0

l000064B5:
	andeq	r0,r0,r0

l000064B9:
	andeq	r0,r0,r0

l000064BD:
	andeq	r0,r0,r0

l000064C1:
	andeq	r0,r0,r0

l000064C5:
	andeq	r0,r0,r0

l000064C9:
	andeq	r0,r0,r0

l000064CD:
	andeq	r0,r0,r0

l000064D1:
	andeq	r0,r0,r0

l000064D5:
	andeq	r0,r0,r0

l000064D9:
	andeq	r0,r0,r0

l000064DD:
	andeq	r0,r0,r0

l000064E1:
	andeq	r0,r0,r0

l000064E5:
	andeq	r0,r0,r0

l000064E9:
	andeq	r0,r0,r0

l000064ED:
	andeq	r0,r0,r0

l000064F1:
	andeq	r0,r0,r0

l000064F5:
	andeq	r0,r0,r0

l000064F9:
	andeq	r0,r0,r0

l000064FD:
	andeq	r0,r0,r0

l00006501:
	andeq	r0,r0,r0

l00006505:
	andeq	r0,r0,r0

l00006509:
	andeq	r0,r0,r0

l0000650D:
	andeq	r0,r0,r0

l00006511:
	andeq	r0,r0,r0

l00006515:
	andeq	r0,r0,r0

l00006519:
	andeq	r0,r0,r0

l0000651D:
	andeq	r0,r0,r0

l00006521:
	andeq	r0,r0,r0

l00006525:
	andeq	r0,r0,r0

l00006529:
	andeq	r0,r0,r0

l0000652D:
	andeq	r0,r0,r0

l00006531:
	andeq	r0,r0,r0

l00006535:
	andeq	r0,r0,r0

l00006539:
	andeq	r0,r0,r0

l0000653D:
	andeq	r0,r0,r0

l00006541:
	andeq	r0,r0,r0

l00006545:
	andeq	r0,r0,r0

l00006549:
	andeq	r0,r0,r0

l0000654D:
	andeq	r0,r0,r0

l00006551:
	andeq	r0,r0,r0

l00006555:
	andeq	r0,r0,r0

l00006559:
	andeq	r0,r0,r0

l0000655D:
	andeq	r0,r0,r0

l00006561:
	andeq	r0,r0,r0

l00006565:
	andeq	r0,r0,r0

l00006569:
	andeq	r0,r0,r0

l0000656D:
	andeq	r0,r0,r0

l00006571:
	andeq	r0,r0,r0

l00006575:
	andeq	r0,r0,r0

l00006579:
	andeq	r0,r0,r0

l0000657D:
	andeq	r0,r0,r0

l00006581:
	andeq	r0,r0,r0

l00006585:
	andeq	r0,r0,r0

l00006589:
	andeq	r0,r0,r0

l0000658D:
	andeq	r0,r0,r0

l00006591:
	andeq	r0,r0,r0

l00006595:
	andeq	r0,r0,r0

l00006599:
	andeq	r0,r0,r0

l0000659D:
	andeq	r0,r0,r0

l000065A1:
	andeq	r0,r0,r0

l000065A5:
	andeq	r0,r0,r0

l000065A9:
	andeq	r0,r0,r0

l000065AD:
	andeq	r0,r0,r0

l000065B1:
	andeq	r0,r0,r0

l000065B5:
	andeq	r0,r0,r0

l000065B9:
	andeq	r0,r0,r0

l000065BD:
	andeq	r0,r0,r0

l000065C1:
	andeq	r0,r0,r0

l000065C5:
	andeq	r0,r0,r0

l000065C9:
	andeq	r0,r0,r0

l000065CD:
	andeq	r0,r0,r0

l000065D1:
	andeq	r0,r0,r0

l000065D5:
	andeq	r0,r0,r0

l000065D9:
	andeq	r0,r0,r0

l000065DD:
	andeq	r0,r0,r0

l000065E1:
	andeq	r0,r0,r0

l000065E5:
	andeq	r0,r0,r0

l000065E9:
	andeq	r0,r0,r0

l000065ED:
	andeq	r0,r0,r0

l000065F1:
	andeq	r0,r0,r0

l000065F5:
	andeq	r0,r0,r0

l000065F9:
	andeq	r0,r0,r0

l000065FD:
	andeq	r0,r0,r0

l00006601:
	andeq	r0,r0,r0

l00006605:
	andeq	r0,r0,r0

l00006609:
	andeq	r0,r0,r0

l0000660D:
	andeq	r0,r0,r0

l00006611:
	andeq	r0,r0,r0

l00006615:
	andeq	r0,r0,r0

l00006619:
	andeq	r0,r0,r0

l0000661D:
	andeq	r0,r0,r0

l00006621:
	andeq	r0,r0,r0

l00006625:
	andeq	r0,r0,r0

l00006629:
	andeq	r0,r0,r0

l0000662D:
	andeq	r0,r0,r0

l00006631:
	andeq	r0,r0,r0

l00006635:
	andeq	r0,r0,r0

l00006639:
	andeq	r0,r0,r0

l0000663D:
	andeq	r0,r0,r0

l00006641:
	andeq	r0,r0,r0

l00006645:
	andeq	r0,r0,r0

l00006649:
	andeq	r0,r0,r0

l0000664D:
	andeq	r0,r0,r0

l00006651:
	andeq	r0,r0,r0

l00006655:
	andeq	r0,r0,r0

l00006659:
	andeq	r0,r0,r0

l0000665D:
	andeq	r0,r0,r0

l00006661:
	andeq	r0,r0,r0

l00006665:
	andeq	r0,r0,r0

l00006669:
	andeq	r0,r0,r0

l0000666D:
	andeq	r0,r0,r0

l00006671:
	andeq	r0,r0,r0

l00006675:
	andeq	r0,r0,r0

l00006679:
	andeq	r0,r0,r0

l0000667D:
	andeq	r0,r0,r0

l00006681:
	andeq	r0,r0,r0

l00006685:
	andeq	r0,r0,r0

l00006689:
	andeq	r0,r0,r0

l0000668D:
	andeq	r0,r0,r0

l00006691:
	andeq	r0,r0,r0

l00006695:
	andeq	r0,r0,r0

l00006699:
	andeq	r0,r0,r0

l0000669D:
	andeq	r0,r0,r0

l000066A1:
	andeq	r0,r0,r0

l000066A5:
	andeq	r0,r0,r0

l000066A9:
	andeq	r0,r0,r0

l000066AD:
	andeq	r0,r0,r0

l000066B1:
	andeq	r0,r0,r0

l000066B5:
	andeq	r0,r0,r0

l000066B9:
	andeq	r0,r0,r0

l000066BD:
	andeq	r0,r0,r0

l000066C1:
	andeq	r0,r0,r0

l000066C5:
	andeq	r0,r0,r0

l000066C9:
	andeq	r0,r0,r0

l000066CD:
	andeq	r0,r0,r0

l000066D1:
	andeq	r0,r0,r0

l000066D5:
	andeq	r0,r0,r0

l000066D9:
	andeq	r0,r0,r0

l000066DD:
	andeq	r0,r0,r0

l000066E1:
	andeq	r0,r0,r0

l000066E5:
	andeq	r0,r0,r0

l000066E9:
	andeq	r0,r0,r0

l000066ED:
	andeq	r0,r0,r0

l000066F1:
	andeq	r0,r0,r0

l000066F5:
	andeq	r0,r0,r0

l000066F9:
	andeq	r0,r0,r0

l000066FD:
	andeq	r0,r0,r0

l00006701:
	andeq	r0,r0,r0

l00006705:
	andeq	r0,r0,r0

l00006709:
	andeq	r0,r0,r0

l0000670D:
	andeq	r0,r0,r0

l00006711:
	andeq	r0,r0,r0

l00006715:
	andeq	r0,r0,r0

l00006719:
	andeq	r0,r0,r0

l0000671D:
	andeq	r0,r0,r0

l00006721:
	andeq	r0,r0,r0

l00006725:
	andeq	r0,r0,r0

l00006729:
	andeq	r0,r0,r0

l0000672D:
	andeq	r0,r0,r0

l00006731:
	andeq	r0,r0,r0

l00006735:
	andeq	r0,r0,r0

l00006739:
	andeq	r0,r0,r0

l0000673D:
	andeq	r0,r0,r0

l00006741:
	andeq	r0,r0,r0

l00006745:
	andeq	r0,r0,r0

l00006749:
	andeq	r0,r0,r0

l0000674D:
	andeq	r0,r0,r0

l00006751:
	andeq	r0,r0,r0

l00006755:
	andeq	r0,r0,r0

l00006759:
	andeq	r0,r0,r0

l0000675D:
	andeq	r0,r0,r0

l00006761:
	andeq	r0,r0,r0

l00006765:
	andeq	r0,r0,r0

l00006769:
	andeq	r0,r0,r0

l0000676D:
	andeq	r0,r0,r0

l00006771:
	andeq	r0,r0,r0

l00006775:
	andeq	r0,r0,r0

l00006779:
	andeq	r0,r0,r0

l0000677D:
	andeq	r0,r0,r0

l00006781:
	andeq	r0,r0,r0

l00006785:
	andeq	r0,r0,r0

l00006789:
	andeq	r0,r0,r0

l0000678D:
	andeq	r0,r0,r0

l00006791:
	andeq	r0,r0,r0

l00006795:
	andeq	r0,r0,r0

l00006799:
	andeq	r0,r0,r0

l0000679D:
	andeq	r0,r0,r0

l000067A1:
	andeq	r0,r0,r0

l000067A5:
	andeq	r0,r0,r0

l000067A9:
	andeq	r0,r0,r0

l000067AD:
	andeq	r0,r0,r0

l000067B1:
	andeq	r0,r0,r0

l000067B5:
	andeq	r0,r0,r0

l000067B9:
	andeq	r0,r0,r0

l000067BD:
	andeq	r0,r0,r0

l000067C1:
	andeq	r0,r0,r0

l000067C5:
	andeq	r0,r0,r0

l000067C9:
	andeq	r0,r0,r0

l000067CD:
	andeq	r0,r0,r0

l000067D1:
	andeq	r0,r0,r0

l000067D5:
	andeq	r0,r0,r0

l000067D9:
	andeq	r0,r0,r0

l000067DD:
	andeq	r0,r0,r0

l000067E1:
	andeq	r0,r0,r0

l000067E5:
	andeq	r0,r0,r0

l000067E9:
	andeq	r0,r0,r0

l000067ED:
	andeq	r0,r0,r0

l000067F1:
	andeq	r0,r0,r0

l000067F5:
	andeq	r0,r0,r0

l000067F9:
	andeq	r0,r0,r0

l000067FD:
	andeq	r0,r0,r0

l00006801:
	andeq	r0,r0,r0

l00006805:
	andeq	r0,r0,r0

l00006809:
	andeq	r0,r0,r0

l0000680D:
	andeq	r0,r0,r0

l00006811:
	andeq	r0,r0,r0

l00006815:
	andeq	r0,r0,r0

l00006819:
	andeq	r0,r0,r0

l0000681D:
	andeq	r0,r0,r0

l00006821:
	andeq	r0,r0,r0

l00006825:
	andeq	r0,r0,r0

l00006829:
	andeq	r0,r0,r0

l0000682D:
	andeq	r0,r0,r0

l00006831:
	andeq	r0,r0,r0

l00006835:
	andeq	r0,r0,r0

l00006839:
	andeq	r0,r0,r0

l0000683D:
	andeq	r0,r0,r0

l00006841:
	andeq	r0,r0,r0

l00006845:
	andeq	r0,r0,r0

l00006849:
	andeq	r0,r0,r0

l0000684D:
	andeq	r0,r0,r0

l00006851:
	andeq	r0,r0,r0

l00006855:
	andeq	r0,r0,r0

l00006859:
	andeq	r0,r0,r0

l0000685D:
	andeq	r0,r0,r0

l00006861:
	andeq	r0,r0,r0

l00006865:
	andeq	r0,r0,r0

l00006869:
	andeq	r0,r0,r0

l0000686D:
	andeq	r0,r0,r0

l00006871:
	andeq	r0,r0,r0

l00006875:
	andeq	r0,r0,r0

l00006879:
	andeq	r0,r0,r0

l0000687D:
	andeq	r0,r0,r0

l00006881:
	andeq	r0,r0,r0

l00006885:
	andeq	r0,r0,r0

l00006889:
	andeq	r0,r0,r0

l0000688D:
	andeq	r0,r0,r0

l00006891:
	andeq	r0,r0,r0

l00006895:
	andeq	r0,r0,r0

l00006899:
	andeq	r0,r0,r0

l0000689D:
	andeq	r0,r0,r0

l000068A1:
	andeq	r0,r0,r0

l000068A5:
	andeq	r0,r0,r0

l000068A9:
	andeq	r0,r0,r0

l000068AD:
	andeq	r0,r0,r0

l000068B1:
	andeq	r0,r0,r0

l000068B5:
	andeq	r0,r0,r0

l000068B9:
	andeq	r0,r0,r0

l000068BD:
	andeq	r0,r0,r0

l000068C1:
	andeq	r0,r0,r0

l000068C5:
	andeq	r0,r0,r0

l000068C9:
	andeq	r0,r0,r0

l000068CD:
	andeq	r0,r0,r0

l000068D1:
	andeq	r0,r0,r0

l000068D5:
	andeq	r0,r0,r0

l000068D9:
	andeq	r0,r0,r0

l000068DD:
	andeq	r0,r0,r0

l000068E1:
	andeq	r0,r0,r0

l000068E5:
	andeq	r0,r0,r0

l000068E9:
	andeq	r0,r0,r0

l000068ED:
	andeq	r0,r0,r0

l000068F1:
	andeq	r0,r0,r0

l000068F5:
	andeq	r0,r0,r0

l000068F9:
	andeq	r0,r0,r0

l000068FD:
	andeq	r0,r0,r0

l00006901:
	andeq	r0,r0,r0

l00006905:
	andeq	r0,r0,r0

l00006909:
	andeq	r0,r0,r0

l0000690D:
	andeq	r0,r0,r0

l00006911:
	andeq	r0,r0,r0

l00006915:
	andeq	r0,r0,r0

l00006919:
	andeq	r0,r0,r0

l0000691D:
	andeq	r0,r0,r0

l00006921:
	andeq	r0,r0,r0

l00006925:
	andeq	r0,r0,r0

l00006929:
	andeq	r0,r0,r0

l0000692D:
	andeq	r0,r0,r0

l00006931:
	andeq	r0,r0,r0

l00006935:
	andeq	r0,r0,r0

l00006939:
	andeq	r0,r0,r0

l0000693D:
	andeq	r0,r0,r0

l00006941:
	andeq	r0,r0,r0

l00006945:
	andeq	r0,r0,r0

l00006949:
	andeq	r0,r0,r0

l0000694D:
	andeq	r0,r0,r0

l00006951:
	andeq	r0,r0,r0

l00006955:
	andeq	r0,r0,r0

l00006959:
	andeq	r0,r0,r0

l0000695D:
	andeq	r0,r0,r0

l00006961:
	andeq	r0,r0,r0

l00006965:
	andeq	r0,r0,r0

l00006969:
	andeq	r0,r0,r0

l0000696D:
	andeq	r0,r0,r0

l00006971:
	andeq	r0,r0,r0

l00006975:
	andeq	r0,r0,r0

l00006979:
	andeq	r0,r0,r0

l0000697D:
	andeq	r0,r0,r0

l00006981:
	andeq	r0,r0,r0

l00006985:
	andeq	r0,r0,r0

l00006989:
	andeq	r0,r0,r0

l0000698D:
	andeq	r0,r0,r0

l00006991:
	andeq	r0,r0,r0

l00006995:
	andeq	r0,r0,r0

l00006999:
	andeq	r0,r0,r0

l0000699D:
	andeq	r0,r0,r0

l000069A1:
	andeq	r0,r0,r0

l000069A5:
	andeq	r0,r0,r0

l000069A9:
	andeq	r0,r0,r0

l000069AD:
	andeq	r0,r0,r0

l000069B1:
	andeq	r0,r0,r0

l000069B5:
	andeq	r0,r0,r0

l000069B9:
	andeq	r0,r0,r0

l000069BD:
	andeq	r0,r0,r0

l000069C1:
	andeq	r0,r0,r0

l000069C5:
	andeq	r0,r0,r0

l000069C9:
	andeq	r0,r0,r0

l000069CD:
	andeq	r0,r0,r0

l000069D1:
	andeq	r0,r0,r0

l000069D5:
	andeq	r0,r0,r0

l000069D9:
	andeq	r0,r0,r0

l000069DD:
	andeq	r0,r0,r0

l000069E1:
	andeq	r0,r0,r0

l000069E5:
	andeq	r0,r0,r0

l000069E9:
	andeq	r0,r0,r0

l000069ED:
	andeq	r0,r0,r0

l000069F1:
	andeq	r0,r0,r0

l000069F5:
	andeq	r0,r0,r0

l000069F9:
	andeq	r0,r0,r0

l000069FD:
	andeq	r0,r0,r0

l00006A01:
	andeq	r0,r0,r0

l00006A05:
	andeq	r0,r0,r0

l00006A09:
	andeq	r0,r0,r0

l00006A0D:
	andeq	r0,r0,r0

l00006A11:
	andeq	r0,r0,r0

l00006A15:
	andeq	r0,r0,r0

l00006A19:
	andeq	r0,r0,r0

l00006A1D:
	andeq	r0,r0,r0

l00006A21:
	andeq	r0,r0,r0

l00006A25:
	andeq	r0,r0,r0

l00006A29:
	andeq	r0,r0,r0

l00006A2D:
	andeq	r0,r0,r0

l00006A31:
	andeq	r0,r0,r0

l00006A35:
	andeq	r0,r0,r0

l00006A39:
	andeq	r0,r0,r0

l00006A3D:
	andeq	r0,r0,r0

l00006A41:
	andeq	r0,r0,r0

l00006A45:
	andeq	r0,r0,r0

l00006A49:
	andeq	r0,r0,r0

l00006A4D:
	andeq	r0,r0,r0

l00006A51:
	andeq	r0,r0,r0

l00006A55:
	andeq	r0,r0,r0

l00006A59:
	andeq	r0,r0,r0

l00006A5D:
	andeq	r0,r0,r0

l00006A61:
	andeq	r0,r0,r0

l00006A65:
	andeq	r0,r0,r0

l00006A69:
	andeq	r0,r0,r0

l00006A6D:
	andeq	r0,r0,r0

l00006A71:
	andeq	r0,r0,r0

l00006A75:
	andeq	r0,r0,r0

l00006A79:
	andeq	r0,r0,r0

l00006A7D:
	andeq	r0,r0,r0

l00006A81:
	andeq	r0,r0,r0

l00006A85:
	andeq	r0,r0,r0

l00006A89:
	andeq	r0,r0,r0

l00006A8D:
	andeq	r0,r0,r0

l00006A91:
	andeq	r0,r0,r0

l00006A95:
	andeq	r0,r0,r0

l00006A99:
	andeq	r0,r0,r0

l00006A9D:
	andeq	r0,r0,r0

l00006AA1:
	andeq	r0,r0,r0

l00006AA5:
	andeq	r0,r0,r0

l00006AA9:
	andeq	r0,r0,r0

l00006AAD:
	andeq	r0,r0,r0

l00006AB1:
	andeq	r0,r0,r0

l00006AB5:
	andeq	r0,r0,r0

l00006AB9:
	andeq	r0,r0,r0

l00006ABD:
	andeq	r0,r0,r0

l00006AC1:
	andeq	r0,r0,r0

l00006AC5:
	andeq	r0,r0,r0

l00006AC9:
	andeq	r0,r0,r0

l00006ACD:
	andeq	r0,r0,r0

l00006AD1:
	andeq	r0,r0,r0

l00006AD5:
	andeq	r0,r0,r0

l00006AD9:
	andeq	r0,r0,r0

l00006ADD:
	andeq	r0,r0,r0

l00006AE1:
	andeq	r0,r0,r0

l00006AE5:
	andeq	r0,r0,r0

l00006AE9:
	andeq	r0,r0,r0

l00006AED:
	andeq	r0,r0,r0

l00006AF1:
	andeq	r0,r0,r0

l00006AF5:
	andeq	r0,r0,r0

l00006AF9:
	andeq	r0,r0,r0

l00006AFD:
	andeq	r0,r0,r0

l00006B01:
	andeq	r0,r0,r0

l00006B05:
	andeq	r0,r0,r0

l00006B09:
	andeq	r0,r0,r0

l00006B0D:
	andeq	r0,r0,r0

l00006B11:
	andeq	r0,r0,r0

l00006B15:
	andeq	r0,r0,r0

l00006B19:
	andeq	r0,r0,r0

l00006B1D:
	andeq	r0,r0,r0

l00006B21:
	andeq	r0,r0,r0

l00006B25:
	andeq	r0,r0,r0

l00006B29:
	andeq	r0,r0,r0

l00006B2D:
	andeq	r0,r0,r0

l00006B31:
	andeq	r0,r0,r0

l00006B35:
	andeq	r0,r0,r0

l00006B39:
	andeq	r0,r0,r0

l00006B3D:
	andeq	r0,r0,r0

l00006B41:
	andeq	r0,r0,r0

l00006B45:
	andeq	r0,r0,r0

l00006B49:
	andeq	r0,r0,r0

l00006B4D:
	andeq	r0,r0,r0

l00006B51:
	andeq	r0,r0,r0

l00006B55:
	andeq	r0,r0,r0

l00006B59:
	andeq	r0,r0,r0

l00006B5D:
	andeq	r0,r0,r0

l00006B61:
	andeq	r0,r0,r0

l00006B65:
	andeq	r0,r0,r0

l00006B69:
	andeq	r0,r0,r0

l00006B6D:
	andeq	r0,r0,r0

l00006B71:
	andeq	r0,r0,r0

l00006B75:
	andeq	r0,r0,r0

l00006B79:
	andeq	r0,r0,r0

l00006B7D:
	andeq	r0,r0,r0

l00006B81:
	andeq	r0,r0,r0

l00006B85:
	andeq	r0,r0,r0

l00006B89:
	andeq	r0,r0,r0

l00006B8D:
	andeq	r0,r0,r0

l00006B91:
	andeq	r0,r0,r0

l00006B95:
	andeq	r0,r0,r0

l00006B99:
	andeq	r0,r0,r0

l00006B9D:
	andeq	r0,r0,r0

l00006BA1:
	andeq	r0,r0,r0

l00006BA5:
	andeq	r0,r0,r0

l00006BA9:
	andeq	r0,r0,r0

l00006BAD:
	andeq	r0,r0,r0

l00006BB1:
	andeq	r0,r0,r0

l00006BB5:
	andeq	r0,r0,r0

l00006BB9:
	andeq	r0,r0,r0

l00006BBD:
	andeq	r0,r0,r0

l00006BC1:
	andeq	r0,r0,r0

l00006BC5:
	andeq	r0,r0,r0

l00006BC9:
	andeq	r0,r0,r0

l00006BCD:
	andeq	r0,r0,r0

l00006BD1:
	andeq	r0,r0,r0

l00006BD5:
	andeq	r0,r0,r0

l00006BD9:
	andeq	r0,r0,r0

l00006BDD:
	andeq	r0,r0,r0

l00006BE1:
	andeq	r0,r0,r0

l00006BE5:
	andeq	r0,r0,r0

l00006BE9:
	andeq	r0,r0,r0

l00006BED:
	andeq	r0,r0,r0

l00006BF1:
	andeq	r0,r0,r0

l00006BF5:
	andeq	r0,r0,r0

l00006BF9:
	andeq	r0,r0,r0

l00006BFD:
	andeq	r0,r0,r0

l00006C01:
	andeq	r0,r0,r0

l00006C05:
	andeq	r0,r0,r0

l00006C09:
	andeq	r0,r0,r0

l00006C0D:
	andeq	r0,r0,r0

l00006C11:
	andeq	r0,r0,r0

l00006C15:
	andeq	r0,r0,r0

l00006C19:
	andeq	r0,r0,r0

l00006C1D:
	andeq	r0,r0,r0

l00006C21:
	andeq	r0,r0,r0

l00006C25:
	andeq	r0,r0,r0

l00006C29:
	andeq	r0,r0,r0

l00006C2D:
	andeq	r0,r0,r0

l00006C31:
	andeq	r0,r0,r0

l00006C35:
	andeq	r0,r0,r0

l00006C39:
	andeq	r0,r0,r0

l00006C3D:
	andeq	r0,r0,r0

l00006C41:
	andeq	r0,r0,r0

l00006C45:
	andeq	r0,r0,r0

l00006C49:
	andeq	r0,r0,r0

l00006C4D:
	andeq	r0,r0,r0

l00006C51:
	andeq	r0,r0,r0

l00006C55:
	andeq	r0,r0,r0

l00006C59:
	andeq	r0,r0,r0

l00006C5D:
	andeq	r0,r0,r0

l00006C61:
	andeq	r0,r0,r0

l00006C65:
	andeq	r0,r0,r0

l00006C69:
	andeq	r0,r0,r0

l00006C6D:
	andeq	r0,r0,r0

l00006C71:
	andeq	r0,r0,r0

l00006C75:
	andeq	r0,r0,r0

l00006C79:
	andeq	r0,r0,r0

l00006C7D:
	andeq	r0,r0,r0

l00006C81:
	andeq	r0,r0,r0

l00006C85:
	andeq	r0,r0,r0

l00006C89:
	andeq	r0,r0,r0

l00006C8D:
	andeq	r0,r0,r0

l00006C91:
	andeq	r0,r0,r0

l00006C95:
	andeq	r0,r0,r0

l00006C99:
	andeq	r0,r0,r0

l00006C9D:
	andeq	r0,r0,r0

l00006CA1:
	andeq	r0,r0,r0

l00006CA5:
	andeq	r0,r0,r0

l00006CA9:
	andeq	r0,r0,r0

l00006CAD:
	andeq	r0,r0,r0

l00006CB1:
	andeq	r0,r0,r0

l00006CB5:
	andeq	r0,r0,r0

l00006CB9:
	andeq	r0,r0,r0

l00006CBD:
	andeq	r0,r0,r0

l00006CC1:
	andeq	r0,r0,r0

l00006CC5:
	andeq	r0,r0,r0

l00006CC9:
	andeq	r0,r0,r0

l00006CCD:
	andeq	r0,r0,r0

l00006CD1:
	andeq	r0,r0,r0

l00006CD5:
	andeq	r0,r0,r0

l00006CD9:
	andeq	r0,r0,r0

l00006CDD:
	andeq	r0,r0,r0

l00006CE1:
	andeq	r0,r0,r0

l00006CE5:
	andeq	r0,r0,r0

l00006CE9:
	andeq	r0,r0,r0

l00006CED:
	andeq	r0,r0,r0

l00006CF1:
	andeq	r0,r0,r0

l00006CF5:
	andeq	r0,r0,r0

l00006CF9:
	andeq	r0,r0,r0

l00006CFD:
	andeq	r0,r0,r0

l00006D01:
	andeq	r0,r0,r0

l00006D05:
	andeq	r0,r0,r0

l00006D09:
	andeq	r0,r0,r0

l00006D0D:
	andeq	r0,r0,r0

l00006D11:
	andeq	r0,r0,r0

l00006D15:
	andeq	r0,r0,r0

l00006D19:
	andeq	r0,r0,r0

l00006D1D:
	andeq	r0,r0,r0

l00006D21:
	andeq	r0,r0,r0

l00006D25:
	andeq	r0,r0,r0

l00006D29:
	andeq	r0,r0,r0

l00006D2D:
	andeq	r0,r0,r0

l00006D31:
	andeq	r0,r0,r0

l00006D35:
	andeq	r0,r0,r0

l00006D39:
	andeq	r0,r0,r0

l00006D3D:
	andeq	r0,r0,r0

l00006D41:
	andeq	r0,r0,r0

l00006D45:
	andeq	r0,r0,r0

l00006D49:
	andeq	r0,r0,r0

l00006D4D:
	andeq	r0,r0,r0

l00006D51:
	andeq	r0,r0,r0

l00006D55:
	andeq	r0,r0,r0

l00006D59:
	andeq	r0,r0,r0

l00006D5D:
	andeq	r0,r0,r0

l00006D61:
	andeq	r0,r0,r0

l00006D65:
	andeq	r0,r0,r0

l00006D69:
	andeq	r0,r0,r0

l00006D6D:
	andeq	r0,r0,r0

l00006D71:
	andeq	r0,r0,r0

l00006D75:
	andeq	r0,r0,r0

l00006D79:
	andeq	r0,r0,r0

l00006D7D:
	andeq	r0,r0,r0

l00006D81:
	andeq	r0,r0,r0

l00006D85:
	andeq	r0,r0,r0

l00006D89:
	andeq	r0,r0,r0

l00006D8D:
	andeq	r0,r0,r0

l00006D91:
	andeq	r0,r0,r0

l00006D95:
	andeq	r0,r0,r0

l00006D99:
	andeq	r0,r0,r0

l00006D9D:
	andeq	r0,r0,r0

l00006DA1:
	andeq	r0,r0,r0

l00006DA5:
	andeq	r0,r0,r0

l00006DA9:
	andeq	r0,r0,r0

l00006DAD:
	andeq	r0,r0,r0

l00006DB1:
	andeq	r0,r0,r0

l00006DB5:
	andeq	r0,r0,r0

l00006DB9:
	andeq	r0,r0,r0

l00006DBD:
	andeq	r0,r0,r0

l00006DC1:
	andeq	r0,r0,r0

l00006DC5:
	andeq	r0,r0,r0

l00006DC9:
	andeq	r0,r0,r0

l00006DCD:
	andeq	r0,r0,r0

l00006DD1:
	andeq	r0,r0,r0

l00006DD5:
	andeq	r0,r0,r0

l00006DD9:
	andeq	r0,r0,r0

l00006DDD:
	andeq	r0,r0,r0

l00006DE1:
	andeq	r0,r0,r0

l00006DE5:
	andeq	r0,r0,r0

l00006DE9:
	andeq	r0,r0,r0

l00006DED:
	andeq	r0,r0,r0

l00006DF1:
	andeq	r0,r0,r0

l00006DF5:
	andeq	r0,r0,r0

l00006DF9:
	andeq	r0,r0,r0

l00006DFD:
	andeq	r0,r0,r0

l00006E01:
	andeq	r0,r0,r0

l00006E05:
	andeq	r0,r0,r0

l00006E09:
	andeq	r0,r0,r0

l00006E0D:
	andeq	r0,r0,r0

l00006E11:
	andeq	r0,r0,r0

l00006E15:
	andeq	r0,r0,r0

l00006E19:
	andeq	r0,r0,r0

l00006E1D:
	andeq	r0,r0,r0

l00006E21:
	andeq	r0,r0,r0

l00006E25:
	andeq	r0,r0,r0

l00006E29:
	andeq	r0,r0,r0

l00006E2D:
	andeq	r0,r0,r0

l00006E31:
	andeq	r0,r0,r0

l00006E35:
	andeq	r0,r0,r0

l00006E39:
	andeq	r0,r0,r0

l00006E3D:
	andeq	r0,r0,r0

l00006E41:
	andeq	r0,r0,r0

l00006E45:
	andeq	r0,r0,r0

l00006E49:
	andeq	r0,r0,r0

l00006E4D:
	andeq	r0,r0,r0

l00006E51:
	andeq	r0,r0,r0

l00006E55:
	andeq	r0,r0,r0

l00006E59:
	andeq	r0,r0,r0

l00006E5D:
	andeq	r0,r0,r0

l00006E61:
	andeq	r0,r0,r0

l00006E65:
	andeq	r0,r0,r0

l00006E69:
	andeq	r0,r0,r0

l00006E6D:
	andeq	r0,r0,r0

l00006E71:
	andeq	r0,r0,r0

l00006E75:
	andeq	r0,r0,r0

l00006E79:
	andeq	r0,r0,r0

l00006E7D:
	andeq	r0,r0,r0

l00006E81:
	andeq	r0,r0,r0

l00006E85:
	andeq	r0,r0,r0

l00006E89:
	andeq	r0,r0,r0

l00006E8D:
	andeq	r0,r0,r0

l00006E91:
	andeq	r0,r0,r0

l00006E95:
	andeq	r0,r0,r0

l00006E99:
	andeq	r0,r0,r0

l00006E9D:
	andeq	r0,r0,r0

l00006EA1:
	andeq	r0,r0,r0

l00006EA5:
	andeq	r0,r0,r0

l00006EA9:
	andeq	r0,r0,r0

l00006EAD:
	andeq	r0,r0,r0

l00006EB1:
	andeq	r0,r0,r0

l00006EB5:
	andeq	r0,r0,r0

l00006EB9:
	andeq	r0,r0,r0

l00006EBD:
	andeq	r0,r0,r0

l00006EC1:
	andeq	r0,r0,r0

l00006EC5:
	andeq	r0,r0,r0

l00006EC9:
	andeq	r0,r0,r0

l00006ECD:
	andeq	r0,r0,r0

l00006ED1:
	andeq	r0,r0,r0

l00006ED5:
	andeq	r0,r0,r0

l00006ED9:
	andeq	r0,r0,r0

l00006EDD:
	andeq	r0,r0,r0

l00006EE1:
	andeq	r0,r0,r0

l00006EE5:
	andeq	r0,r0,r0

l00006EE9:
	andeq	r0,r0,r0

l00006EED:
	andeq	r0,r0,r0

l00006EF1:
	andeq	r0,r0,r0

l00006EF5:
	andeq	r0,r0,r0

l00006EF9:
	andeq	r0,r0,r0

l00006EFD:
	andeq	r0,r0,r0

l00006F01:
	andeq	r0,r0,r0

l00006F05:
	andeq	r0,r0,r0

l00006F09:
	andeq	r0,r0,r0

l00006F0D:
	andeq	r0,r0,r0

l00006F11:
	andeq	r0,r0,r0

l00006F15:
	andeq	r0,r0,r0

l00006F19:
	andeq	r0,r0,r0

l00006F1D:
	andeq	r0,r0,r0

l00006F21:
	andeq	r0,r0,r0

l00006F25:
	andeq	r0,r0,r0

l00006F29:
	andeq	r0,r0,r0

l00006F2D:
	andeq	r0,r0,r0

l00006F31:
	andeq	r0,r0,r0

l00006F35:
	andeq	r0,r0,r0

l00006F39:
	andeq	r0,r0,r0

l00006F3D:
	andeq	r0,r0,r0

l00006F41:
	andeq	r0,r0,r0

l00006F45:
	andeq	r0,r0,r0

l00006F49:
	andeq	r0,r0,r0

l00006F4D:
	andeq	r0,r0,r0

l00006F51:
	andeq	r0,r0,r0

l00006F55:
	andeq	r0,r0,r0

l00006F59:
	andeq	r0,r0,r0

l00006F5D:
	andeq	r0,r0,r0

l00006F61:
	andeq	r0,r0,r0

l00006F65:
	andeq	r0,r0,r0

l00006F69:
	andeq	r0,r0,r0

l00006F6D:
	andeq	r0,r0,r0

l00006F71:
	andeq	r0,r0,r0

l00006F75:
	andeq	r0,r0,r0

l00006F79:
	andeq	r0,r0,r0

l00006F7D:
	andeq	r0,r0,r0

l00006F81:
	andeq	r0,r0,r0

l00006F85:
	andeq	r0,r0,r0

l00006F89:
	andeq	r0,r0,r0

l00006F8D:
	andeq	r0,r0,r0

l00006F91:
	andeq	r0,r0,r0

l00006F95:
	andeq	r0,r0,r0

l00006F99:
	andeq	r0,r0,r0

l00006F9D:
	andeq	r0,r0,r0

l00006FA1:
	andeq	r0,r0,r0

l00006FA5:
	andeq	r0,r0,r0

l00006FA9:
	andeq	r0,r0,r0

l00006FAD:
	andeq	r0,r0,r0

l00006FB1:
	andeq	r0,r0,r0

l00006FB5:
	andeq	r0,r0,r0

l00006FB9:
	andeq	r0,r0,r0

l00006FBD:
	andeq	r0,r0,r0

l00006FC1:
	andeq	r0,r0,r0

l00006FC5:
	andeq	r0,r0,r0

l00006FC9:
	andeq	r0,r0,r0

l00006FCD:
	andeq	r0,r0,r0

l00006FD1:
	andeq	r0,r0,r0

l00006FD5:
	andeq	r0,r0,r0

l00006FD9:
	andeq	r0,r0,r0

l00006FDD:
	andeq	r0,r0,r0

l00006FE1:
	andeq	r0,r0,r0

l00006FE5:
	andeq	r0,r0,r0

l00006FE9:
	andeq	r0,r0,r0

l00006FED:
	andeq	r0,r0,r0

l00006FF1:
	andeq	r0,r0,r0

l00006FF5:
	andeq	r0,r0,r0

l00006FF9:
	andeq	r0,r0,r0

l00006FFD:
	andeq	r0,r0,r0

l00007001:
	andeq	r0,r0,r0

l00007005:
	andeq	r0,r0,r0

l00007009:
	andeq	r0,r0,r0

l0000700D:
	andeq	r0,r0,r0

l00007011:
	andeq	r0,r0,r0

l00007015:
	andeq	r0,r0,r0

l00007019:
	andeq	r0,r0,r0

l0000701D:
	andeq	r0,r0,r0

l00007021:
	andeq	r0,r0,r0

l00007025:
	andeq	r0,r0,r0

l00007029:
	andeq	r0,r0,r0

l0000702D:
	andeq	r0,r0,r0

l00007031:
	andeq	r0,r0,r0

l00007035:
	andeq	r0,r0,r0

l00007039:
	andeq	r0,r0,r0

l0000703D:
	andeq	r0,r0,r0

l00007041:
	andeq	r0,r0,r0

l00007045:
	andeq	r0,r0,r0

l00007049:
	andeq	r0,r0,r0

l0000704D:
	andeq	r0,r0,r0

l00007051:
	andeq	r0,r0,r0

l00007055:
	andeq	r0,r0,r0

l00007059:
	andeq	r0,r0,r0

l0000705D:
	andeq	r0,r0,r0

l00007061:
	andeq	r0,r0,r0

l00007065:
	andeq	r0,r0,r0

l00007069:
	andeq	r0,r0,r0

l0000706D:
	andeq	r0,r0,r0

l00007071:
	andeq	r0,r0,r0

l00007075:
	andeq	r0,r0,r0

l00007079:
	andeq	r0,r0,r0

l0000707D:
	andeq	r0,r0,r0

l00007081:
	andeq	r0,r0,r0

l00007085:
	andeq	r0,r0,r0

l00007089:
	andeq	r0,r0,r0

l0000708D:
	andeq	r0,r0,r0

l00007091:
	andeq	r0,r0,r0

l00007095:
	andeq	r0,r0,r0

l00007099:
	andeq	r0,r0,r0

l0000709D:
	andeq	r0,r0,r0

l000070A1:
	andeq	r0,r0,r0

l000070A5:
	andeq	r0,r0,r0

l000070A9:
	andeq	r0,r0,r0

l000070AD:
	andeq	r0,r0,r0

l000070B1:
	andeq	r0,r0,r0

l000070B5:
	andeq	r0,r0,r0

l000070B9:
	andeq	r0,r0,r0

l000070BD:
	andeq	r0,r0,r0

l000070C1:
	andeq	r0,r0,r0

l000070C5:
	andeq	r0,r0,r0

l000070C9:
	andeq	r0,r0,r0

l000070CD:
	andeq	r0,r0,r0

l000070D1:
	andeq	r0,r0,r0

l000070D5:
	andeq	r0,r0,r0

l000070D9:
	andeq	r0,r0,r0

l000070DD:
	andeq	r0,r0,r0

l000070E1:
	andeq	r0,r0,r0

l000070E5:
	andeq	r0,r0,r0

l000070E9:
	andeq	r0,r0,r0

l000070ED:
	andeq	r0,r0,r0

l000070F1:
	andeq	r0,r0,r0

l000070F5:
	andeq	r0,r0,r0

l000070F9:
	andeq	r0,r0,r0

l000070FD:
	andeq	r0,r0,r0

l00007101:
	andeq	r0,r0,r0

l00007105:
	andeq	r0,r0,r0

l00007109:
	andeq	r0,r0,r0

l0000710D:
	andeq	r0,r0,r0

l00007111:
	andeq	r0,r0,r0

l00007115:
	andeq	r0,r0,r0

l00007119:
	andeq	r0,r0,r0

l0000711D:
	andeq	r0,r0,r0

l00007121:
	andeq	r0,r0,r0

l00007125:
	andeq	r0,r0,r0

l00007129:
	andeq	r0,r0,r0

l0000712D:
	andeq	r0,r0,r0

l00007131:
	andeq	r0,r0,r0

l00007135:
	andeq	r0,r0,r0

l00007139:
	andeq	r0,r0,r0

l0000713D:
	andeq	r0,r0,r0

l00007141:
	andeq	r0,r0,r0

l00007145:
	andeq	r0,r0,r0

l00007149:
	andeq	r0,r0,r0

l0000714D:
	andeq	r0,r0,r0

l00007151:
	andeq	r0,r0,r0

l00007155:
	andeq	r0,r0,r0

l00007159:
	andeq	r0,r0,r0

l0000715D:
	andeq	r0,r0,r0

l00007161:
	andeq	r0,r0,r0

l00007165:
	andeq	r0,r0,r0

l00007169:
	andeq	r0,r0,r0

l0000716D:
	andeq	r0,r0,r0

l00007171:
	andeq	r0,r0,r0

l00007175:
	andeq	r0,r0,r0

l00007179:
	andeq	r0,r0,r0

l0000717D:
	andeq	r0,r0,r0

l00007181:
	andeq	r0,r0,r0

l00007185:
	andeq	r0,r0,r0

l00007189:
	andeq	r0,r0,r0

l0000718D:
	andeq	r0,r0,r0

l00007191:
	andeq	r0,r0,r0

l00007195:
	andeq	r0,r0,r0

l00007199:
	andeq	r0,r0,r0

l0000719D:
	andeq	r0,r0,r0

l000071A1:
	andeq	r0,r0,r0

l000071A5:
	andeq	r0,r0,r0

l000071A9:
	andeq	r0,r0,r0

l000071AD:
	andeq	r0,r0,r0

l000071B1:
	andeq	r0,r0,r0

l000071B5:
	andeq	r0,r0,r0

l000071B9:
	andeq	r0,r0,r0

l000071BD:
	andeq	r0,r0,r0

l000071C1:
	andeq	r0,r0,r0

l000071C5:
	andeq	r0,r0,r0

l000071C9:
	andeq	r0,r0,r0

l000071CD:
	andeq	r0,r0,r0

l000071D1:
	andeq	r0,r0,r0

l000071D5:
	andeq	r0,r0,r0

l000071D9:
	andeq	r0,r0,r0

l000071DD:
	andeq	r0,r0,r0

l000071E1:
	andeq	r0,r0,r0

l000071E5:
	andeq	r0,r0,r0

l000071E9:
	andeq	r0,r0,r0

l000071ED:
	andeq	r0,r0,r0

l000071F1:
	andeq	r0,r0,r0

l000071F5:
	andeq	r0,r0,r0

l000071F9:
	andeq	r0,r0,r0

l000071FD:
	andeq	r0,r0,r0

l00007201:
	andeq	r0,r0,r0

l00007205:
	andeq	r0,r0,r0

l00007209:
	andeq	r0,r0,r0

l0000720D:
	andeq	r0,r0,r0

l00007211:
	andeq	r0,r0,r0

l00007215:
	andeq	r0,r0,r0

l00007219:
	andeq	r0,r0,r0

l0000721D:
	andeq	r0,r0,r0

l00007221:
	andeq	r0,r0,r0

l00007225:
	andeq	r0,r0,r0

l00007229:
	andeq	r0,r0,r0

l0000722D:
	andeq	r0,r0,r0

l00007231:
	andeq	r0,r0,r0

l00007235:
	andeq	r0,r0,r0

l00007239:
	andeq	r0,r0,r0

l0000723D:
	andeq	r0,r0,r0

l00007241:
	andeq	r0,r0,r0

l00007245:
	andeq	r0,r0,r0

l00007249:
	andeq	r0,r0,r0

l0000724D:
	andeq	r0,r0,r0

l00007251:
	andeq	r0,r0,r0

l00007255:
	andeq	r0,r0,r0

l00007259:
	andeq	r0,r0,r0

l0000725D:
	andeq	r0,r0,r0

l00007261:
	andeq	r0,r0,r0

l00007265:
	andeq	r0,r0,r0

l00007269:
	andeq	r0,r0,r0

l0000726D:
	andeq	r0,r0,r0

l00007271:
	andeq	r0,r0,r0

l00007275:
	andeq	r0,r0,r0

l00007279:
	andeq	r0,r0,r0

l0000727D:
	andeq	r0,r0,r0

l00007281:
	andeq	r0,r0,r0

l00007285:
	andeq	r0,r0,r0

l00007289:
	andeq	r0,r0,r0

l0000728D:
	andeq	r0,r0,r0

l00007291:
	andeq	r0,r0,r0

l00007295:
	andeq	r0,r0,r0

l00007299:
	andeq	r0,r0,r0

l0000729D:
	andeq	r0,r0,r0

l000072A1:
	andeq	r0,r0,r0

l000072A5:
	andeq	r0,r0,r0

l000072A9:
	andeq	r0,r0,r0

l000072AD:
	andeq	r0,r0,r0

l000072B1:
	andeq	r0,r0,r0

l000072B5:
	andeq	r0,r0,r0

l000072B9:
	andeq	r0,r0,r0

l000072BD:
	andeq	r0,r0,r0

l000072C1:
	andeq	r0,r0,r0

l000072C5:
	andeq	r0,r0,r0

l000072C9:
	andeq	r0,r0,r0

l000072CD:
	andeq	r0,r0,r0

l000072D1:
	andeq	r0,r0,r0

l000072D5:
	andeq	r0,r0,r0

l000072D9:
	andeq	r0,r0,r0

l000072DD:
	andeq	r0,r0,r0

l000072E1:
	andeq	r0,r0,r0

l000072E5:
	andeq	r0,r0,r0

l000072E9:
	andeq	r0,r0,r0

l000072ED:
	andeq	r0,r0,r0

l000072F1:
	andeq	r0,r0,r0

l000072F5:
	andeq	r0,r0,r0

l000072F9:
	andeq	r0,r0,r0

l000072FD:
	andeq	r0,r0,r0

l00007301:
	andeq	r0,r0,r0

l00007305:
	andeq	r0,r0,r0

l00007309:
	andeq	r0,r0,r0

l0000730D:
	andeq	r0,r0,r0

l00007311:
	andeq	r0,r0,r0

l00007315:
	andeq	r0,r0,r0

l00007319:
	andeq	r0,r0,r0

l0000731D:
	andeq	r0,r0,r0

l00007321:
	andeq	r0,r0,r0

l00007325:
	andeq	r0,r0,r0

l00007329:
	andeq	r0,r0,r0

l0000732D:
	andeq	r0,r0,r0

l00007331:
	andeq	r0,r0,r0

l00007335:
	andeq	r0,r0,r0

l00007339:
	andeq	r0,r0,r0

l0000733D:
	andeq	r0,r0,r0

l00007341:
	andeq	r0,r0,r0

l00007345:
	andeq	r0,r0,r0

l00007349:
	andeq	r0,r0,r0

l0000734D:
	andeq	r0,r0,r0

l00007351:
	andeq	r0,r0,r0

l00007355:
	andeq	r0,r0,r0

l00007359:
	andeq	r0,r0,r0

l0000735D:
	andeq	r0,r0,r0

l00007361:
	andeq	r0,r0,r0

l00007365:
	andeq	r0,r0,r0

l00007369:
	andeq	r0,r0,r0

l0000736D:
	andeq	r0,r0,r0

l00007371:
	andeq	r0,r0,r0

l00007375:
	andeq	r0,r0,r0

l00007379:
	andeq	r0,r0,r0

l0000737D:
	andeq	r0,r0,r0

l00007381:
	andeq	r0,r0,r0

l00007385:
	andeq	r0,r0,r0

l00007389:
	andeq	r0,r0,r0

l0000738D:
	andeq	r0,r0,r0

l00007391:
	andeq	r0,r0,r0

l00007395:
	andeq	r0,r0,r0

l00007399:
	andeq	r0,r0,r0

l0000739D:
	andeq	r0,r0,r0

l000073A1:
	andeq	r0,r0,r0

l000073A5:
	andeq	r0,r0,r0

l000073A9:
	andeq	r0,r0,r0

l000073AD:
	andeq	r0,r0,r0

l000073B1:
	andeq	r0,r0,r0

l000073B5:
	andeq	r0,r0,r0

l000073B9:
	andeq	r0,r0,r0

l000073BD:
	andeq	r0,r0,r0

l000073C1:
	andeq	r0,r0,r0

l000073C5:
	andeq	r0,r0,r0

l000073C9:
	andeq	r0,r0,r0

l000073CD:
	andeq	r0,r0,r0

l000073D1:
	andeq	r0,r0,r0

l000073D5:
	andeq	r0,r0,r0

l000073D9:
	andeq	r0,r0,r0

l000073DD:
	andeq	r0,r0,r0

l000073E1:
	andeq	r0,r0,r0

l000073E5:
	andeq	r0,r0,r0

l000073E9:
	andeq	r0,r0,r0

l000073ED:
	andeq	r0,r0,r0

l000073F1:
	andeq	r0,r0,r0

l000073F5:
	andeq	r0,r0,r0

l000073F9:
	andeq	r0,r0,r0

l000073FD:
	andeq	r0,r0,r0

l00007401:
	andeq	r0,r0,r0

l00007405:
	andeq	r0,r0,r0

l00007409:
	andeq	r0,r0,r0

l0000740D:
	andeq	r0,r0,r0

l00007411:
	andeq	r0,r0,r0

l00007415:
	andeq	r0,r0,r0

l00007419:
	andeq	r0,r0,r0

l0000741D:
	andeq	r0,r0,r0

l00007421:
	andeq	r0,r0,r0

l00007425:
	andeq	r0,r0,r0

l00007429:
	andeq	r0,r0,r0

l0000742D:
	andeq	r0,r0,r0

l00007431:
	andeq	r0,r0,r0

l00007435:
	andeq	r0,r0,r0

l00007439:
	andeq	r0,r0,r0

l0000743D:
	andeq	r0,r0,r0

l00007441:
	andeq	r0,r0,r0

l00007445:
	andeq	r0,r0,r0

l00007449:
	andeq	r0,r0,r0

l0000744D:
	andeq	r0,r0,r0

l00007451:
	andeq	r0,r0,r0

l00007455:
	andeq	r0,r0,r0

l00007459:
	andeq	r0,r0,r0

l0000745D:
	andeq	r0,r0,r0

l00007461:
	andeq	r0,r0,r0

l00007465:
	andeq	r0,r0,r0

l00007469:
	andeq	r0,r0,r0

l0000746D:
	andeq	r0,r0,r0

l00007471:
	andeq	r0,r0,r0

l00007475:
	andeq	r0,r0,r0

l00007479:
	andeq	r0,r0,r0

l0000747D:
	andeq	r0,r0,r0

l00007481:
	andeq	r0,r0,r0

l00007485:
	andeq	r0,r0,r0

l00007489:
	andeq	r0,r0,r0

l0000748D:
	andeq	r0,r0,r0

l00007491:
	andeq	r0,r0,r0

l00007495:
	andeq	r0,r0,r0

l00007499:
	andeq	r0,r0,r0

l0000749D:
	andeq	r0,r0,r0

l000074A1:
	andeq	r0,r0,r0

l000074A5:
	andeq	r0,r0,r0

l000074A9:
	andeq	r0,r0,r0

l000074AD:
	andeq	r0,r0,r0

l000074B1:
	andeq	r0,r0,r0

l000074B5:
	andeq	r0,r0,r0

l000074B9:
	andeq	r0,r0,r0

l000074BD:
	andeq	r0,r0,r0

l000074C1:
	andeq	r0,r0,r0

l000074C5:
	andeq	r0,r0,r0

l000074C9:
	andeq	r0,r0,r0

l000074CD:
	andeq	r0,r0,r0

l000074D1:
	andeq	r0,r0,r0

l000074D5:
	andeq	r0,r0,r0

l000074D9:
	andeq	r0,r0,r0

l000074DD:
	andeq	r0,r0,r0

l000074E1:
	andeq	r0,r0,r0

l000074E5:
	andeq	r0,r0,r0

l000074E9:
	andeq	r0,r0,r0

l000074ED:
	andeq	r0,r0,r0

l000074F1:
	andeq	r0,r0,r0

l000074F5:
	andeq	r0,r0,r0

l000074F9:
	andeq	r0,r0,r0

l000074FD:
	andeq	r0,r0,r0

l00007501:
	andeq	r0,r0,r0

l00007505:
	andeq	r0,r0,r0

l00007509:
	andeq	r0,r0,r0

l0000750D:
	andeq	r0,r0,r0

l00007511:
	andeq	r0,r0,r0

l00007515:
	andeq	r0,r0,r0

l00007519:
	andeq	r0,r0,r0

l0000751D:
	andeq	r0,r0,r0

l00007521:
	andeq	r0,r0,r0

l00007525:
	andeq	r0,r0,r0

l00007529:
	andeq	r0,r0,r0

l0000752D:
	andeq	r0,r0,r0

l00007531:
	andeq	r0,r0,r0

l00007535:
	andeq	r0,r0,r0

l00007539:
	andeq	r0,r0,r0

l0000753D:
	andeq	r0,r0,r0

l00007541:
	andeq	r0,r0,r0

l00007545:
	andeq	r0,r0,r0

l00007549:
	andeq	r0,r0,r0

l0000754D:
	andeq	r0,r0,r0

l00007551:
	andeq	r0,r0,r0

l00007555:
	andeq	r0,r0,r0

l00007559:
	andeq	r0,r0,r0

l0000755D:
	andeq	r0,r0,r0

l00007561:
	andeq	r0,r0,r0

l00007565:
	andeq	r0,r0,r0

l00007569:
	andeq	r0,r0,r0

l0000756D:
	andeq	r0,r0,r0

l00007571:
	andeq	r0,r0,r0

l00007575:
	andeq	r0,r0,r0

l00007579:
	andeq	r0,r0,r0

l0000757D:
	andeq	r0,r0,r0

l00007581:
	andeq	r0,r0,r0

l00007585:
	andeq	r0,r0,r0

l00007589:
	andeq	r0,r0,r0

l0000758D:
	andeq	r0,r0,r0

l00007591:
	andeq	r0,r0,r0

l00007595:
	andeq	r0,r0,r0

l00007599:
	andeq	r0,r0,r0

l0000759D:
	andeq	r0,r0,r0

l000075A1:
	andeq	r0,r0,r0

l000075A5:
	andeq	r0,r0,r0

l000075A9:
	andeq	r0,r0,r0

l000075AD:
	andeq	r0,r0,r0

l000075B1:
	andeq	r0,r0,r0

l000075B5:
	andeq	r0,r0,r0

l000075B9:
	andeq	r0,r0,r0

l000075BD:
	andeq	r0,r0,r0

l000075C1:
	andeq	r0,r0,r0

l000075C5:
	andeq	r0,r0,r0

l000075C9:
	andeq	r0,r0,r0

l000075CD:
	andeq	r0,r0,r0

l000075D1:
	andeq	r0,r0,r0

l000075D5:
	andeq	r0,r0,r0

l000075D9:
	andeq	r0,r0,r0

l000075DD:
	andeq	r0,r0,r0

l000075E1:
	andeq	r0,r0,r0

l000075E5:
	andeq	r0,r0,r0

l000075E9:
	andeq	r0,r0,r0

l000075ED:
	andeq	r0,r0,r0

l000075F1:
	andeq	r0,r0,r0

l000075F5:
	andeq	r0,r0,r0

l000075F9:
	andeq	r0,r0,r0

l000075FD:
	andeq	r0,r0,r0

l00007601:
	andeq	r0,r0,r0

l00007605:
	andeq	r0,r0,r0

l00007609:
	andeq	r0,r0,r0

l0000760D:
	andeq	r0,r0,r0

l00007611:
	andeq	r0,r0,r0

l00007615:
	andeq	r0,r0,r0

l00007619:
	andeq	r0,r0,r0

l0000761D:
	andeq	r0,r0,r0

l00007621:
	andeq	r0,r0,r0

l00007625:
	andeq	r0,r0,r0

l00007629:
	andeq	r0,r0,r0

l0000762D:
	andeq	r0,r0,r0

l00007631:
	andeq	r0,r0,r0

l00007635:
	andeq	r0,r0,r0

l00007639:
	andeq	r0,r0,r0

l0000763D:
	andeq	r0,r0,r0

l00007641:
	andeq	r0,r0,r0

l00007645:
	andeq	r0,r0,r0

l00007649:
	andeq	r0,r0,r0

l0000764D:
	andeq	r0,r0,r0

l00007651:
	andeq	r0,r0,r0

l00007655:
	andeq	r0,r0,r0

l00007659:
	andeq	r0,r0,r0

l0000765D:
	andeq	r0,r0,r0

l00007661:
	andeq	r0,r0,r0

l00007665:
	andeq	r0,r0,r0

l00007669:
	andeq	r0,r0,r0

l0000766D:
	andeq	r0,r0,r0

l00007671:
	andeq	r0,r0,r0

l00007675:
	andeq	r0,r0,r0

l00007679:
	andeq	r0,r0,r0

l0000767D:
	andeq	r0,r0,r0

l00007681:
	andeq	r0,r0,r0

l00007685:
	andeq	r0,r0,r0

l00007689:
	andeq	r0,r0,r0

l0000768D:
	andeq	r0,r0,r0

l00007691:
	andeq	r0,r0,r0

l00007695:
	andeq	r0,r0,r0

l00007699:
	andeq	r0,r0,r0

l0000769D:
	andeq	r0,r0,r0

l000076A1:
	andeq	r0,r0,r0

l000076A5:
	andeq	r0,r0,r0

l000076A9:
	andeq	r0,r0,r0

l000076AD:
	andeq	r0,r0,r0

l000076B1:
	andeq	r0,r0,r0

l000076B5:
	andeq	r0,r0,r0

l000076B9:
	andeq	r0,r0,r0

l000076BD:
	andeq	r0,r0,r0

l000076C1:
	andeq	r0,r0,r0

l000076C5:
	andeq	r0,r0,r0

l000076C9:
	andeq	r0,r0,r0

l000076CD:
	andeq	r0,r0,r0

l000076D1:
	andeq	r0,r0,r0

l000076D5:
	andeq	r0,r0,r0

l000076D9:
	andeq	r0,r0,r0

l000076DD:
	andeq	r0,r0,r0

l000076E1:
	andeq	r0,r0,r0

l000076E5:
	andeq	r0,r0,r0

l000076E9:
	andeq	r0,r0,r0

l000076ED:
	andeq	r0,r0,r0

l000076F1:
	andeq	r0,r0,r0

l000076F5:
	andeq	r0,r0,r0

l000076F9:
	andeq	r0,r0,r0

l000076FD:
	andeq	r0,r0,r0

l00007701:
	andeq	r0,r0,r0

l00007705:
	andeq	r0,r0,r0

l00007709:
	andeq	r0,r0,r0

l0000770D:
	andeq	r0,r0,r0

l00007711:
	andeq	r0,r0,r0

l00007715:
	andeq	r0,r0,r0

l00007719:
	andeq	r0,r0,r0

l0000771D:
	andeq	r0,r0,r0

l00007721:
	andeq	r0,r0,r0

l00007725:
	andeq	r0,r0,r0

l00007729:
	andeq	r0,r0,r0

l0000772D:
	andeq	r0,r0,r0

l00007731:
	andeq	r0,r0,r0

l00007735:
	andeq	r0,r0,r0

l00007739:
	andeq	r0,r0,r0

l0000773D:
	andeq	r0,r0,r0

l00007741:
	andeq	r0,r0,r0

l00007745:
	andeq	r0,r0,r0

l00007749:
	andeq	r0,r0,r0

l0000774D:
	andeq	r0,r0,r0

l00007751:
	andeq	r0,r0,r0

l00007755:
	andeq	r0,r0,r0

l00007759:
	andeq	r0,r0,r0

l0000775D:
	andeq	r0,r0,r0

l00007761:
	andeq	r0,r0,r0

l00007765:
	andeq	r0,r0,r0

l00007769:
	andeq	r0,r0,r0

l0000776D:
	andeq	r0,r0,r0

l00007771:
	andeq	r0,r0,r0

l00007775:
	andeq	r0,r0,r0

l00007779:
	andeq	r0,r0,r0

l0000777D:
	andeq	r0,r0,r0

l00007781:
	andeq	r0,r0,r0

l00007785:
	andeq	r0,r0,r0

l00007789:
	andeq	r0,r0,r0

l0000778D:
	andeq	r0,r0,r0

l00007791:
	andeq	r0,r0,r0

l00007795:
	andeq	r0,r0,r0

l00007799:
	andeq	r0,r0,r0

l0000779D:
	andeq	r0,r0,r0

l000077A1:
	andeq	r0,r0,r0

l000077A5:
	andeq	r0,r0,r0

l000077A9:
	andeq	r0,r0,r0

l000077AD:
	andeq	r0,r0,r0

l000077B1:
	andeq	r0,r0,r0

l000077B5:
	andeq	r0,r0,r0

l000077B9:
	andeq	r0,r0,r0

l000077BD:
	andeq	r0,r0,r0

l000077C1:
	andeq	r0,r0,r0

l000077C5:
	andeq	r0,r0,r0

l000077C9:
	andeq	r0,r0,r0

l000077CD:
	andeq	r0,r0,r0

l000077D1:
	andeq	r0,r0,r0

l000077D5:
	andeq	r0,r0,r0

l000077D9:
	andeq	r0,r0,r0

l000077DD:
	andeq	r0,r0,r0

l000077E1:
	andeq	r0,r0,r0

l000077E5:
	andeq	r0,r0,r0

l000077E9:
	andeq	r0,r0,r0

l000077ED:
	andeq	r0,r0,r0

l000077F1:
	andeq	r0,r0,r0

l000077F5:
	andeq	r0,r0,r0

l000077F9:
	andeq	r0,r0,r0

l000077FD:
	andeq	r0,r0,r0

l00007801:
	andeq	r0,r0,r0

l00007805:
	andeq	r0,r0,r0

l00007809:
	andeq	r0,r0,r0

l0000780D:
	andeq	r0,r0,r0

l00007811:
	andeq	r0,r0,r0

l00007815:
	andeq	r0,r0,r0

l00007819:
	andeq	r0,r0,r0

l0000781D:
	andeq	r0,r0,r0

l00007821:
	andeq	r0,r0,r0

l00007825:
	andeq	r0,r0,r0

l00007829:
	andeq	r0,r0,r0

l0000782D:
	andeq	r0,r0,r0

l00007831:
	andeq	r0,r0,r0

l00007835:
	andeq	r0,r0,r0

l00007839:
	andeq	r0,r0,r0

l0000783D:
	andeq	r0,r0,r0

l00007841:
	andeq	r0,r0,r0

l00007845:
	andeq	r0,r0,r0

l00007849:
	andeq	r0,r0,r0

l0000784D:
	andeq	r0,r0,r0

l00007851:
	andeq	r0,r0,r0

l00007855:
	andeq	r0,r0,r0

l00007859:
	andeq	r0,r0,r0

l0000785D:
	andeq	r0,r0,r0

l00007861:
	andeq	r0,r0,r0

l00007865:
	andeq	r0,r0,r0

l00007869:
	andeq	r0,r0,r0

l0000786D:
	andeq	r0,r0,r0

l00007871:
	andeq	r0,r0,r0

l00007875:
	andeq	r0,r0,r0

l00007879:
	andeq	r0,r0,r0

l0000787D:
	andeq	r0,r0,r0

l00007881:
	andeq	r0,r0,r0

l00007885:
	andeq	r0,r0,r0

l00007889:
	andeq	r0,r0,r0

l0000788D:
	andeq	r0,r0,r0

l00007891:
	andeq	r0,r0,r0

l00007895:
	andeq	r0,r0,r0

l00007899:
	andeq	r0,r0,r0

l0000789D:
	andeq	r0,r0,r0

l000078A1:
	andeq	r0,r0,r0

l000078A5:
	andeq	r0,r0,r0

l000078A9:
	andeq	r0,r0,r0

l000078AD:
	andeq	r0,r0,r0

l000078B1:
	andeq	r0,r0,r0

l000078B5:
	andeq	r0,r0,r0

l000078B9:
	andeq	r0,r0,r0

l000078BD:
	andeq	r0,r0,r0

l000078C1:
	andeq	r0,r0,r0

l000078C5:
	andeq	r0,r0,r0

l000078C9:
	andeq	r0,r0,r0

l000078CD:
	andeq	r0,r0,r0

l000078D1:
	andeq	r0,r0,r0

l000078D5:
	andeq	r0,r0,r0

l000078D9:
	andeq	r0,r0,r0

l000078DD:
	andeq	r0,r0,r0

l000078E1:
	andeq	r0,r0,r0

l000078E5:
	andeq	r0,r0,r0

l000078E9:
	andeq	r0,r0,r0

l000078ED:
	andeq	r0,r0,r0

l000078F1:
	andeq	r0,r0,r0

l000078F5:
	andeq	r0,r0,r0

l000078F9:
	andeq	r0,r0,r0

l000078FD:
	andeq	r0,r0,r0

l00007901:
	andeq	r0,r0,r0

l00007905:
	andeq	r0,r0,r0

l00007909:
	andeq	r0,r0,r0

l0000790D:
	andeq	r0,r0,r0

l00007911:
	andeq	r0,r0,r0

l00007915:
	andeq	r0,r0,r0

l00007919:
	andeq	r0,r0,r0

l0000791D:
	andeq	r0,r0,r0

l00007921:
	andeq	r0,r0,r0

l00007925:
	andeq	r0,r0,r0

l00007929:
	andeq	r0,r0,r0

l0000792D:
	andeq	r0,r0,r0

l00007931:
	andeq	r0,r0,r0

l00007935:
	andeq	r0,r0,r0

l00007939:
	andeq	r0,r0,r0

l0000793D:
	andeq	r0,r0,r0

l00007941:
	andeq	r0,r0,r0

l00007945:
	andeq	r0,r0,r0

l00007949:
	andeq	r0,r0,r0

l0000794D:
	andeq	r0,r0,r0

l00007951:
	andeq	r0,r0,r0

l00007955:
	andeq	r0,r0,r0

l00007959:
	andeq	r0,r0,r0

l0000795D:
	andeq	r0,r0,r0

l00007961:
	andeq	r0,r0,r0

l00007965:
	andeq	r0,r0,r0

l00007969:
	andeq	r0,r0,r0

l0000796D:
	andeq	r0,r0,r0

l00007971:
	andeq	r0,r0,r0

l00007975:
	andeq	r0,r0,r0

l00007979:
	andeq	r0,r0,r0

l0000797D:
	andeq	r0,r0,r0

l00007981:
	andeq	r0,r0,r0

l00007985:
	andeq	r0,r0,r0

l00007989:
	andeq	r0,r0,r0

l0000798D:
	andeq	r0,r0,r0

l00007991:
	andeq	r0,r0,r0

l00007995:
	andeq	r0,r0,r0

l00007999:
	andeq	r0,r0,r0

l0000799D:
	andeq	r0,r0,r0

l000079A1:
	andeq	r0,r0,r0

l000079A5:
	andeq	r0,r0,r0

l000079A9:
	andeq	r0,r0,r0

l000079AD:
	andeq	r0,r0,r0

l000079B1:
	andeq	r0,r0,r0

l000079B5:
	andeq	r0,r0,r0

l000079B9:
	andeq	r0,r0,r0

l000079BD:
	andeq	r0,r0,r0

l000079C1:
	andeq	r0,r0,r0

l000079C5:
	andeq	r0,r0,r0

l000079C9:
	andeq	r0,r0,r0

l000079CD:
	andeq	r0,r0,r0

l000079D1:
	andeq	r0,r0,r0

l000079D5:
	andeq	r0,r0,r0

l000079D9:
	andeq	r0,r0,r0

l000079DD:
	andeq	r0,r0,r0

l000079E1:
	andeq	r0,r0,r0

l000079E5:
	andeq	r0,r0,r0

l000079E9:
	andeq	r0,r0,r0

l000079ED:
	andeq	r0,r0,r0

l000079F1:
	andeq	r0,r0,r0

l000079F5:
	andeq	r0,r0,r0

l000079F9:
	andeq	r0,r0,r0

l000079FD:
	andeq	r0,r0,r0

l00007A01:
	andeq	r0,r0,r0

l00007A05:
	andeq	r0,r0,r0

l00007A09:
	andeq	r0,r0,r0

l00007A0D:
	andeq	r0,r0,r0

l00007A11:
	andeq	r0,r0,r0

l00007A15:
	andeq	r0,r0,r0

l00007A19:
	andeq	r0,r0,r0

l00007A1D:
	andeq	r0,r0,r0

l00007A21:
	andeq	r0,r0,r0

l00007A25:
	andeq	r0,r0,r0

l00007A29:
	andeq	r0,r0,r0

l00007A2D:
	andeq	r0,r0,r0

l00007A31:
	andeq	r0,r0,r0

l00007A35:
	andeq	r0,r0,r0

l00007A39:
	andeq	r0,r0,r0

l00007A3D:
	andeq	r0,r0,r0

l00007A41:
	andeq	r0,r0,r0

l00007A45:
	andeq	r0,r0,r0

l00007A49:
	andeq	r0,r0,r0

l00007A4D:
	andeq	r0,r0,r0

l00007A51:
	andeq	r0,r0,r0

l00007A55:
	andeq	r0,r0,r0

l00007A59:
	andeq	r0,r0,r0

l00007A5D:
	andeq	r0,r0,r0

l00007A61:
	andeq	r0,r0,r0

l00007A65:
	andeq	r0,r0,r0

l00007A69:
	andeq	r0,r0,r0

l00007A6D:
	andeq	r0,r0,r0

l00007A71:
	andeq	r0,r0,r0

l00007A75:
	andeq	r0,r0,r0

l00007A79:
	andeq	r0,r0,r0

l00007A7D:
	andeq	r0,r0,r0

l00007A81:
	andeq	r0,r0,r0

l00007A85:
	andeq	r0,r0,r0

l00007A89:
	andeq	r0,r0,r0

l00007A8D:
	andeq	r0,r0,r0

l00007A91:
	andeq	r0,r0,r0

l00007A95:
	andeq	r0,r0,r0

l00007A99:
	andeq	r0,r0,r0

l00007A9D:
	andeq	r0,r0,r0

l00007AA1:
	andeq	r0,r0,r0

l00007AA5:
	andeq	r0,r0,r0

l00007AA9:
	andeq	r0,r0,r0

l00007AAD:
	andeq	r0,r0,r0

l00007AB1:
	andeq	r0,r0,r0

l00007AB5:
	andeq	r0,r0,r0

l00007AB9:
	andeq	r0,r0,r0

l00007ABD:
	andeq	r0,r0,r0

l00007AC1:
	andeq	r0,r0,r0

l00007AC5:
	andeq	r0,r0,r0

l00007AC9:
	andeq	r0,r0,r0

l00007ACD:
	andeq	r0,r0,r0

l00007AD1:
	andeq	r0,r0,r0

l00007AD5:
	andeq	r0,r0,r0

l00007AD9:
	andeq	r0,r0,r0

l00007ADD:
	andeq	r0,r0,r0

l00007AE1:
	andeq	r0,r0,r0

l00007AE5:
	andeq	r0,r0,r0

l00007AE9:
	andeq	r0,r0,r0

l00007AED:
	andeq	r0,r0,r0

l00007AF1:
	andeq	r0,r0,r0

l00007AF5:
	andeq	r0,r0,r0

l00007AF9:
	andeq	r0,r0,r0

l00007AFD:
	andeq	r0,r0,r0

l00007B01:
	andeq	r0,r0,r0

l00007B05:
	andeq	r0,r0,r0

l00007B09:
	andeq	r0,r0,r0

l00007B0D:
	andeq	r0,r0,r0

l00007B11:
	andeq	r0,r0,r0

l00007B15:
	andeq	r0,r0,r0

l00007B19:
	andeq	r0,r0,r0

l00007B1D:
	andeq	r0,r0,r0

l00007B21:
	andeq	r0,r0,r0

l00007B25:
	andeq	r0,r0,r0

l00007B29:
	andeq	r0,r0,r0

l00007B2D:
	andeq	r0,r0,r0

l00007B31:
	andeq	r0,r0,r0

l00007B35:
	andeq	r0,r0,r0

l00007B39:
	andeq	r0,r0,r0

l00007B3D:
	andeq	r0,r0,r0

l00007B41:
	andeq	r0,r0,r0

l00007B45:
	andeq	r0,r0,r0

l00007B49:
	andeq	r0,r0,r0

l00007B4D:
	andeq	r0,r0,r0

l00007B51:
	andeq	r0,r0,r0

l00007B55:
	andeq	r0,r0,r0

l00007B59:
	andeq	r0,r0,r0

l00007B5D:
	andeq	r0,r0,r0

l00007B61:
	andeq	r0,r0,r0

l00007B65:
	andeq	r0,r0,r0

l00007B69:
	andeq	r0,r0,r0

l00007B6D:
	andeq	r0,r0,r0

l00007B71:
	andeq	r0,r0,r0

l00007B75:
	andeq	r0,r0,r0

l00007B79:
	andeq	r0,r0,r0

l00007B7D:
	andeq	r0,r0,r0

l00007B81:
	andeq	r0,r0,r0

l00007B85:
	andeq	r0,r0,r0

l00007B89:
	andeq	r0,r0,r0

l00007B8D:
	andeq	r0,r0,r0

l00007B91:
	andeq	r0,r0,r0

l00007B95:
	andeq	r0,r0,r0

l00007B99:
	andeq	r0,r0,r0

l00007B9D:
	andeq	r0,r0,r0

l00007BA1:
	andeq	r0,r0,r0

l00007BA5:
	andeq	r0,r0,r0

l00007BA9:
	andeq	r0,r0,r0

l00007BAD:
	andeq	r0,r0,r0

l00007BB1:
	andeq	r0,r0,r0

l00007BB5:
	andeq	r0,r0,r0

l00007BB9:
	andeq	r0,r0,r0

l00007BBD:
	andeq	r0,r0,r0

l00007BC1:
	andeq	r0,r0,r0

l00007BC5:
	andeq	r0,r0,r0

l00007BC9:
	andeq	r0,r0,r0

l00007BCD:
	andeq	r0,r0,r0

l00007BD1:
	andeq	r0,r0,r0

l00007BD5:
	andeq	r0,r0,r0

l00007BD9:
	andeq	r0,r0,r0

l00007BDD:
	andeq	r0,r0,r0

l00007BE1:
	andeq	r0,r0,r0

l00007BE5:
	andeq	r0,r0,r0

l00007BE9:
	andeq	r0,r0,r0

l00007BED:
	andeq	r0,r0,r0

l00007BF1:
	andeq	r0,r0,r0

l00007BF5:
	andeq	r0,r0,r0

l00007BF9:
	andeq	r0,r0,r0

l00007BFD:
	andeq	r0,r0,r0

l00007C01:
	andeq	r0,r0,r0

l00007C05:
	andeq	r0,r0,r0

l00007C09:
	andeq	r0,r0,r0

l00007C0D:
	andeq	r0,r0,r0

l00007C11:
	andeq	r0,r0,r0

l00007C15:
	andeq	r0,r0,r0

l00007C19:
	andeq	r0,r0,r0

l00007C1D:
	andeq	r0,r0,r0

l00007C21:
	andeq	r0,r0,r0

l00007C25:
	andeq	r0,r0,r0

l00007C29:
	andeq	r0,r0,r0

l00007C2D:
	andeq	r0,r0,r0

l00007C31:
	andeq	r0,r0,r0

l00007C35:
	andeq	r0,r0,r0

l00007C39:
	andeq	r0,r0,r0

l00007C3D:
	andeq	r0,r0,r0

l00007C41:
	andeq	r0,r0,r0

l00007C45:
	andeq	r0,r0,r0

l00007C49:
	andeq	r0,r0,r0

l00007C4D:
	andeq	r0,r0,r0

l00007C51:
	andeq	r0,r0,r0

l00007C55:
	andeq	r0,r0,r0

l00007C59:
	andeq	r0,r0,r0

l00007C5D:
	andeq	r0,r0,r0

l00007C61:
	andeq	r0,r0,r0

l00007C65:
	andeq	r0,r0,r0

l00007C69:
	andeq	r0,r0,r0

l00007C6D:
	andeq	r0,r0,r0

l00007C71:
	andeq	r0,r0,r0

l00007C75:
	andeq	r0,r0,r0

l00007C79:
	andeq	r0,r0,r0

l00007C7D:
	andeq	r0,r0,r0

l00007C81:
	andeq	r0,r0,r0

l00007C85:
	andeq	r0,r0,r0

l00007C89:
	andeq	r0,r0,r0

l00007C8D:
	andeq	r0,r0,r0

l00007C91:
	andeq	r0,r0,r0

l00007C95:
	andeq	r0,r0,r0

l00007C99:
	andeq	r0,r0,r0

l00007C9D:
	andeq	r0,r0,r0

l00007CA1:
	andeq	r0,r0,r0

l00007CA5:
	andeq	r0,r0,r0

l00007CA9:
	andeq	r0,r0,r0

l00007CAD:
	andeq	r0,r0,r0

l00007CB1:
	andeq	r0,r0,r0

l00007CB5:
	andeq	r0,r0,r0

l00007CB9:
	andeq	r0,r0,r0

l00007CBD:
	andeq	r0,r0,r0

l00007CC1:
	andeq	r0,r0,r0

l00007CC5:
	andeq	r0,r0,r0

l00007CC9:
	andeq	r0,r0,r0

l00007CCD:
	andeq	r0,r0,r0

l00007CD1:
	andeq	r0,r0,r0

l00007CD5:
	andeq	r0,r0,r0

l00007CD9:
	andeq	r0,r0,r0

l00007CDD:
	andeq	r0,r0,r0

l00007CE1:
	andeq	r0,r0,r0

l00007CE5:
	andeq	r0,r0,r0

l00007CE9:
	andeq	r0,r0,r0

l00007CED:
	andeq	r0,r0,r0

l00007CF1:
	andeq	r0,r0,r0

l00007CF5:
	andeq	r0,r0,r0

l00007CF9:
	andeq	r0,r0,r0

l00007CFD:
	andeq	r0,r0,r0

l00007D01:
	andeq	r0,r0,r0

l00007D05:
	andeq	r0,r0,r0

l00007D09:
	andeq	r0,r0,r0

l00007D0D:
	andeq	r0,r0,r0

l00007D11:
	andeq	r0,r0,r0

l00007D15:
	andeq	r0,r0,r0

l00007D19:
	andeq	r0,r0,r0

l00007D1D:
	andeq	r0,r0,r0

l00007D21:
	andeq	r0,r0,r0

l00007D25:
	andeq	r0,r0,r0

l00007D29:
	andeq	r0,r0,r0

l00007D2D:
	andeq	r0,r0,r0

l00007D31:
	andeq	r0,r0,r0

l00007D35:
	andeq	r0,r0,r0

l00007D39:
	andeq	r0,r0,r0

l00007D3D:
	andeq	r0,r0,r0

l00007D41:
	andeq	r0,r0,r0

l00007D45:
	andeq	r0,r0,r0

l00007D49:
	andeq	r0,r0,r0

l00007D4D:
	andeq	r0,r0,r0

l00007D51:
	andeq	r0,r0,r0

l00007D55:
	andeq	r0,r0,r0

l00007D59:
	andeq	r0,r0,r0

l00007D5D:
	andeq	r0,r0,r0

l00007D61:
	andeq	r0,r0,r0

l00007D65:
	andeq	r0,r0,r0

l00007D69:
	andeq	r0,r0,r0

l00007D6D:
	andeq	r0,r0,r0

l00007D71:
	andeq	r0,r0,r0

l00007D75:
	andeq	r0,r0,r0

l00007D79:
	andeq	r0,r0,r0

l00007D7D:
	andeq	r0,r0,r0

l00007D81:
	andeq	r0,r0,r0

l00007D85:
	andeq	r0,r0,r0

l00007D89:
	andeq	r0,r0,r0

l00007D8D:
	andeq	r0,r0,r0

l00007D91:
	andeq	r0,r0,r0

l00007D95:
	andeq	r0,r0,r0

l00007D99:
	andeq	r0,r0,r0

l00007D9D:
	andeq	r0,r0,r0

l00007DA1:
	andeq	r0,r0,r0

l00007DA5:
	andeq	r0,r0,r0

l00007DA9:
	andeq	r0,r0,r0

l00007DAD:
	andeq	r0,r0,r0

l00007DB1:
	andeq	r0,r0,r0

l00007DB5:
	andeq	r0,r0,r0

l00007DB9:
	andeq	r0,r0,r0

l00007DBD:
	andeq	r0,r0,r0

l00007DC1:
	andeq	r0,r0,r0

l00007DC5:
	andeq	r0,r0,r0

l00007DC9:
	andeq	r0,r0,r0

l00007DCD:
	andeq	r0,r0,r0

l00007DD1:
	andeq	r0,r0,r0

l00007DD5:
	andeq	r0,r0,r0

l00007DD9:
	andeq	r0,r0,r0

l00007DDD:
	andeq	r0,r0,r0

l00007DE1:
	andeq	r0,r0,r0

l00007DE5:
	andeq	r0,r0,r0

l00007DE9:
	andeq	r0,r0,r0

l00007DED:
	andeq	r0,r0,r0

l00007DF1:
	andeq	r0,r0,r0

l00007DF5:
	andeq	r0,r0,r0

l00007DF9:
	andeq	r0,r0,r0

l00007DFD:
	andeq	r0,r0,r0

l00007E01:
	andeq	r0,r0,r0

l00007E05:
	andeq	r0,r0,r0

l00007E09:
	andeq	r0,r0,r0

l00007E0D:
	andeq	r0,r0,r0

l00007E11:
	andeq	r0,r0,r0

l00007E15:
	andeq	r0,r0,r0

l00007E19:
	andeq	r0,r0,r0

l00007E1D:
	andeq	r0,r0,r0

l00007E21:
	andeq	r0,r0,r0

l00007E25:
	andeq	r0,r0,r0

l00007E29:
	andeq	r0,r0,r0

l00007E2D:
	andeq	r0,r0,r0

l00007E31:
	andeq	r0,r0,r0

l00007E35:
	andeq	r0,r0,r0

l00007E39:
	andeq	r0,r0,r0

l00007E3D:
	andeq	r0,r0,r0

l00007E41:
	andeq	r0,r0,r0

l00007E45:
	andeq	r0,r0,r0

l00007E49:
	andeq	r0,r0,r0

l00007E4D:
	andeq	r0,r0,r0

l00007E51:
	andeq	r0,r0,r0

l00007E55:
	andeq	r0,r0,r0

l00007E59:
	andeq	r0,r0,r0

l00007E5D:
	andeq	r0,r0,r0

l00007E61:
	andeq	r0,r0,r0

l00007E65:
	andeq	r0,r0,r0

l00007E69:
	andeq	r0,r0,r0

l00007E6D:
	andeq	r0,r0,r0

l00007E71:
	andeq	r0,r0,r0

l00007E75:
	andeq	r0,r0,r0

l00007E79:
	andeq	r0,r0,r0

l00007E7D:
	andeq	r0,r0,r0

l00007E81:
	andeq	r0,r0,r0

l00007E85:
	andeq	r0,r0,r0

l00007E89:
	andeq	r0,r0,r0

l00007E8D:
	andeq	r0,r0,r0

l00007E91:
	andeq	r0,r0,r0

l00007E95:
	andeq	r0,r0,r0

l00007E99:
	andeq	r0,r0,r0

l00007E9D:
	andeq	r0,r0,r0

l00007EA1:
	andeq	r0,r0,r0

l00007EA5:
	andeq	r0,r0,r0

l00007EA9:
	andeq	r0,r0,r0

l00007EAD:
	andeq	r0,r0,r0

l00007EB1:
	andeq	r0,r0,r0

l00007EB5:
	andeq	r0,r0,r0

l00007EB9:
	andeq	r0,r0,r0

l00007EBD:
	andeq	r0,r0,r0

l00007EC1:
	andeq	r0,r0,r0

l00007EC5:
	andeq	r0,r0,r0

l00007EC9:
	andeq	r0,r0,r0

l00007ECD:
	andeq	r0,r0,r0

l00007ED1:
	andeq	r0,r0,r0

l00007ED5:
	andeq	r0,r0,r0

l00007ED9:
	andeq	r0,r0,r0

l00007EDD:
	andeq	r0,r0,r0

l00007EE1:
	andeq	r0,r0,r0

l00007EE5:
	andeq	r0,r0,r0

l00007EE9:
	andeq	r0,r0,r0

l00007EED:
	andeq	r0,r0,r0

l00007EF1:
	andeq	r0,r0,r0

l00007EF5:
	andeq	r0,r0,r0

l00007EF9:
	andeq	r0,r0,r0

l00007EFD:
	andeq	r0,r0,r0

l00007F01:
	andeq	r0,r0,r0

l00007F05:
	andeq	r0,r0,r0

l00007F09:
	andeq	r0,r0,r0

l00007F0D:
	andeq	r0,r0,r0

l00007F11:
	andeq	r0,r0,r0

l00007F15:
	andeq	r0,r0,r0

l00007F19:
	andeq	r0,r0,r0

l00007F1D:
	andeq	r0,r0,r0

l00007F21:
	andeq	r0,r0,r0

l00007F25:
	andeq	r0,r0,r0

l00007F29:
	andeq	r0,r0,r0

l00007F2D:
	andeq	r0,r0,r0

l00007F31:
	andeq	r0,r0,r0

l00007F35:
	andeq	r0,r0,r0

l00007F39:
	andeq	r0,r0,r0

l00007F3D:
	andeq	r0,r0,r0

l00007F41:
	andeq	r0,r0,r0

l00007F45:
	andeq	r0,r0,r0

l00007F49:
	andeq	r0,r0,r0

l00007F4D:
	andeq	r0,r0,r0

l00007F51:
	andeq	r0,r0,r0

l00007F55:
	andeq	r0,r0,r0

l00007F59:
	andeq	r0,r0,r0

l00007F5D:
	andeq	r0,r0,r0

l00007F61:
	andeq	r0,r0,r0

l00007F65:
	andeq	r0,r0,r0

l00007F69:
	andeq	r0,r0,r0

l00007F6D:
	andeq	r0,r0,r0

l00007F71:
	andeq	r0,r0,r0

l00007F75:
	andeq	r0,r0,r0

l00007F79:
	andeq	r0,r0,r0

l00007F7D:
	andeq	r0,r0,r0

l00007F81:
	andeq	r0,r0,r0

l00007F85:
	andeq	r0,r0,r0

l00007F89:
	andeq	r0,r0,r0

l00007F8D:
	andeq	r0,r0,r0

l00007F91:
	andeq	r0,r0,r0

l00007F95:
	andeq	r0,r0,r0

l00007F99:
	andeq	r0,r0,r0

l00007F9D:
	andeq	r0,r0,r0

l00007FA1:
	andeq	r0,r0,r0

l00007FA5:
	andeq	r0,r0,r0

l00007FA9:
	andeq	r0,r0,r0

l00007FAD:
	andeq	r0,r0,r0

l00007FB1:
	andeq	r0,r0,r0

l00007FB5:
	andeq	r0,r0,r0

l00007FB9:
	andeq	r0,r0,r0

l00007FBD:
	andeq	r0,r0,r0

l00007FC1:
	andeq	r0,r0,r0

l00007FC5:
	andeq	r0,r0,r0

l00007FC9:
	andeq	r0,r0,r0

l00007FCD:
	andeq	r0,r0,r0

l00007FD1:
	andeq	r0,r0,r0

l00007FD5:
	andeq	r0,r0,r0

l00007FD9:
	andeq	r0,r0,r0

l00007FDD:
	andeq	r0,r0,r0

l00007FE1:
	andeq	r0,r0,r0

l00007FE5:
	andeq	r0,r0,r0

l00007FE9:
	andeq	r0,r0,r0

l00007FED:
	andeq	r0,r0,r0

l00007FF1:
	andeq	r0,r0,r0

l00007FF5:
	andeq	r0,r0,r0

;; fn00007FF8: 00007FF8
fn00007FF8 proc
	andeq	r0,r0,r0

;; fn00007FF9: 00007FF9
fn00007FF9 proc
	andeq	r0,r0,r0

l00007FFC:
	andeq	r0,r0,r0

;; fn00007FFD: 00007FFD
fn00007FFD proc
	Invalid
;;; Segment .text (00008000)

l00008000:
	svclt	#&E7FE

;; NmiSR: 00008001
NmiSR proc
	Invalid

l00008004:
	svclt	#&E7FE

;; FaultISR: 00008005
FaultISR proc
	ldmeq	pc!,{r0-r2,r5-r7}

l00008008:
	stmdami	r9,{r3,r8-r9,fp,lr}

;; ResetISR: 00008009
ResetISR proc
	movthi	r0,#&894B

;; fn0000800C: 0000800C
fn0000800C proc
	andle	r4,r10,#&30000008

l0000800D:
	ble	$FF48A91D

;; fn00008010: 00008010
fn00008010 proc
	ldrdhs	r4,r5,[r0,-r10]

l00008011:
	eoreq	r0,r1,#&43

;; fn00008014: 00008014
fn00008014 proc
	Invalid

l00008015:
	mvnseq	r2,#&40000004

l00008019:
	bne	$00C89029

l0000801D:
	ldrbteq	r4,[r8],#&344

l00008021:
	blx	$010ACC97
	bllo	$FFC08371

l00008029:
	ldrhtvs	r0,[pc],#&8                                        ; 00008039

l0000802D:
	eorhi	r0,r0,r1

l00008031:
	Invalid

;; raise: 00008035
raise proc
	adcslo	r0,pc,r7,ror #1

;; vPrintTask: 00008039
vPrintTask proc
	stmdbeq	r4!,{r0,r2,r4-r5,r7}

l0000803D:
	asrseq	r8,sp,#6

l00008041:
	eorseq	r0,r4,r9,lsr #3

l00008045:
	svc	#&F04F23
	rsbeq	r2,r8,r2,lsr r8

l0000804D:
	ldrsheq	r8,[sp,#&E0]!

l00008051:
	ldrbteq	r9,[fp],#&6F0

l00008055:
	streq	r0,[r2],-#&1F0

l00008059:
	Invalid

l0000805D:
	ldrblt	r0,[r0,#&198]!

l00008061:
	Invalid

l00008065:
	eorlo	r0,r0,r8

;; vCheckTask: 00008069
vCheckTask proc
	movthi	r0,#&BBB5

l0000806D:
	ldrheq	r0,[r3],r0

l00008071:
	rscseq	r4,ip,#&F00000

l00008075:
	strbmi	r0,[sp],-#&9AC

l00008079:
	strdhs	r0,r1,[sp],-r8

l0000807D:
	ldmdbhi	r2,{r1-r2,r6,r8,lr}^

l00008081:
	Invalid
	svcmi	#&2300FB
	ldrsheq	pc,[r2,-r0]!
	rsbeq	r2,r8,r9,lsr #&11
	Invalid
	adcspl	r0,pc,r7,ror #1
	andhi	r0,r0,r2,lsr #1
	eoreq	r0,r0,r8

;; Main: 000080A1
Main proc
	msrhi	cpsr,#&B5

l000080A5:
	msreq	cpsr,#&4B0

l000080A9:
	eoreq	r0,r4,r0,lsr #&20

l000080AD:
	svceq	#&FCECF0

l000080B1:
	rsbhs	r1,r0,fp,asr #&10

l000080B5:
	blne	$FFC085D5

l000080B9:
	msrhs	cpsr,#&3FC

l000080BD:
	ldceq	p0,c0,[r2],#&118

l000080C1:
	crc32cheq	r3,r2,r9

l000080C5:
	umaaleq	r0,r8,r4,ip

l000080C9:
	rscseq	r9,fp,#&F00

l000080CD:
	movths	r0,#&9B22

l000080D1:
	orrseq	r0,r2,r6,asr #&20

l000080D5:
	stmdbeq	r2!,{r2,r4,r7-r9,fp-sp}

l000080D9:
	ldrbls	r0,[r0,#&48]!

l000080DD:
	Invalid

l000080E1:
	strdhs	r2,r3,[r6,-#&2C]

l000080E5:
	crc32cweq	r0,r8,r6

l000080E9:
	Invalid
	adcshi	r0,pc,r7,ror #1
	stmdapl	r0!,{r3}
	stmdbvs	r0,{r1,r5,r7}
	andvs	r0,r0,r0,lsl #1
	stmdblo	r0,{r1,r5,r7}
	stmdavs	r0,{r7}
	andvc	r0,r0,r2,lsr #1

;; vUART_ISR: 00008109
vUART_ISR proc
	stmdbne	r6!,{r0,r2,r4-r5,r7}

l0000810D:
	asrseq	r8,sp,#4

l00008111:
	Invalid

l00008115:
	ldmible	r0,{r1-r2,r4,r7-r8}^

l00008119:
	strdeq	r0,r1,[r6,-#&4F]

l0000811D:
	crc32weq	r2,r6,r6

l00008121:
	rscs	sp,pc,#&F0000
	bicsne	r0,r5,#&18000000

l00008129:
	blpl	$01A0EE5D

l0000812D:
	sbcsge	r1,r4,r6,lsl #&C

l00008131:
	bicsne	r0,r5,r6,lsl #6

l00008135:
	bvc	$01E0CE65

l00008139:
	bicseq	r0,r9,fp,lsr #&E

l0000813D:
	svcmi	#&B11B9B

l00008141:
	Invalid

l00008145:
	rsbeq	r1,r0,#&4B000

l00008149:
	beq	$FEF64411

l0000814D:
	stmhi	r8,{r0,r3,r6,r8,fp}^

l00008151:
	ldreq	r5,[pc,r6,lsl #24]!                                  ; 00008159

l00008155:
	Invalid

l00008159:
	Invalid
	msrlo	spsr,#&DE7
	crc32weq	r3,r6,r6
	mvnseq	r0,#&2A80
	mvnseq	r8,#&40
	mvnsvc	pc,#&500000
	Invalid
	stmne	r0,{r6-r7}
	mcrrhs	p0,#&C,r0,r0,c0
	strteq	r0,[r0],-#&2
	mvneq	r0,sp,ror #1

;; vSetErrorLED: 00008185
vSetErrorLED proc
	eoreq	r0,r0,r1,lsr #&E

l00008189:
	svcmi	#&BA34F0

;; prvSetAndCheckRegisters: 0000818D
prvSetAndCheckRegisters proc
	bleq	$002CAD55

l00008191:
	bleq	$0000895D

l00008195:
	bleq	$00048D61

l00008199:
	bleq	$00089165

l0000819D:
	bleq	$000C9569

l000081A1:
	bleq	$0010996D

l000081A5:
	bleq	$00149D71

l000081A9:
	bleq	$0018A175

l000081AD:
	bleq	$001CA579

l000081B1:
	bleq	$0020A97D

l000081B5:
	bleq	$0024AD81

l000081B9:
	bleq	$0028B185

l000081BD:
	bllt	$0030B589

l000081C1:
	Invalid

l000081C5:
	bne	$00A0B111

l000081C9:
	stmdane	r9!,{r0,r4,r6-r7,r10-fp}

l000081CD:
	Invalid

l000081D1:
	strtne	r0,[fp],-#&ED1

l000081D5:
	eorne	r0,ip,#&344

l000081D9:
	Invalid

l000081DD:
	Invalid

l000081E1:
	stceq	p2,c1,[pc],-#&344                                    ; 0000852D

l000081E5:
	mvnsne	fp,#&D10000

l000081E9:
	ldmiblt	r1,{r0-r3,r8,fp}^

l000081ED:
	Invalid

l000081F1:
	ldrbne	fp,[r1,#&AD1]!

l000081F5:
	ldclt	p3,c0,[r1],#&3C

l000081F9:
	Invalid

l000081FD:
	Invalid

l00008201:
	stmhi	r9,{r0,r2,r4-r5,r7,r9-r10}

l00008205:
	ldrbteq	r5,[r8],#&D47

l00008209:
	subvc	r7,r7,fp,ror #1

l0000820D:
	ldmeq	pc!,{r0-r2,r6}

;; vApplicationIdleHook: 00008211
vApplicationIdleHook proc
	blhi	$FFC084ED

l00008215:
	ldmiblt	r7,{r1-pc}^

l00008219:
	strbhi	pc,[r7,#&AFF]!

l0000821D:
	andlo	r0,r0,r1,lsl #1

;; PDCInit: 00008221
PDCInit proc
	movthi	r1,#&8AB5

l00008225:
	ldmibge	r0,{r4-r5,r7-r8}^

l00008229:
	Invalid

l0000822D:
	rscseq	r10,ip,#&F000000

l00008231:
	svcmi	#&213422

l00008235:
	Invalid

l00008239:
	Invalid

l0000823D:
	svcmi	#&210822

l00008241:
	Invalid

l00008245:
	beq	$FFFE0E0D

l00008249:
	strteq	r0,[r2],-#&223

l0000824D:
	rscsmi	r4,r0,r1,lsr #&1E

l00008251:
	ldmiblt	r0,{r5}^

l00008255:
	Invalid

l00008259:
	Invalid

l0000825D:
	stmhs	fp,{r1-r2,r6,r9-fp}

l00008261:
	orrseq	r0,r4,r6,asr #&20

l00008265:
	ldmdbhs	fp,{r4-r7,lr-pc}^

l00008269:
	mvns	r0,#&80000011
	strdeq	r2,r3,[r6],-#&1B

l00008271:
	rscsmi	r4,r0,r2,lsr #&1E

l00008275:
	Invalid
	strdhs	r2,r3,[r6,-#&28]
	rscsmi	r4,r0,r6,asr #&1E
	Invalid
	smlaltteq	r3,r0,r8,r0
	ldrshtne	lr,[r8],#&40
	tsteq	r0,r0
	eoreq	r0,r0,r0
	submi	r0,r0,r0,lsl #1
	andlo	r0,r0,r2,asr #&1E

;; PDCWrite: 0000829D
PDCWrite proc
	beq	$0118B979

l000082A1:
	adcseq	r8,r0,ip,asr #6

l000082A5:
	strdhs	r0,r1,[r1],-r0

l000082A9:
	Invalid
	strdhs	r2,r3,[r6],-#&9B
	Invalid
	strdeq	r2,r3,[r6,-#&B]
	Invalid
	Invalid
	Invalid
	ldrshtlo	r0,[r0],#&3B
	ldrhteq	r0,[pc],#&D                                        ; 000082DE
	svcmi	#&400080

;; vListInitialise: 000082D1
vListInitialise proc
	ldrshteq	pc,[r1],-#&F0

l000082D5:
	ldmdbeq	r1,{r1,r5}^

l000082D9:
	rsbhi	r8,r0,r3,lsl #2

l000082DD:
	movgt	r0,#&CE8

l000082E1:
	rsbvc	r0,r1,r0,ror #6

l000082E5:
	adcseq	r0,pc,r7,asr #&20

;; vListInitialiseItem: 000082E9
vListInitialiseItem proc
	rsbvc	r0,r1,r3,lsr #6

l000082ED:
	adcsls	r0,pc,r7,asr #&20

;; vListInsertEnd: 000082F1
vListInsertEnd proc
	andne	r0,r0,r8,ror #&19

l000082F5:
	strheq	r9,[r8,-#&C4]!

l000082F9:
	stcls	p12,c8,[r0],-#&C8

l000082FD:
	eretvs

l00008301:
	rsbne	r9,r0,r0,ror #&12

l00008305:
	rsbeq	r0,r1,#&BC0000

l00008309:
	sublo	r7,r7,r0,rrx

;; vListInsert: 0000830D
vListInsert proc
	blvs	$01A0B9E5

l00008311:
	sbcseq	r1,r0,ip,lsl r1

l00008315:
	strdeq	r0,r1,[r2],-r1

l00008319:
	movtpl	r1,#&6AE0

l0000831D:
	strbge	r1,[r8,-#&C68]!

l00008321:
	ldrbeq	pc,[r2],#&A42

l00008325:
	ereteq

l00008329:
	bhi	$0182E801

l0000832D:
	stmeq	r0!,{r5-r6,r8,ip,lr}

l00008331:
	rsblo	r0,r0,r1,ror #8

l00008335:
	subeq	r7,r7,#&BC

l00008339:
	Invalid
	adcseq	r0,pc,#&E7

;; uxListRemove: 00008341
uxListRemove proc
	erethi

l00008345:
	ldmibls	r4!,{r3,r5-r6,ip}

l00008349:
	erethi

l0000834D:
	blmi	$010B04F5

l00008351:
	rorspl	r0,r0,#&10

l00008355:
	msrne	cpsr,#&60

l00008359:
	stmpl	r1!,{r3,r5-r6,r8}

l0000835D:
	rsbne	r1,r0,lr,lsl r0

l00008361:
	strhvc	r7,[r7],-#&C

;; xQueueCRSend: 00008365
xQueueCRSend proc
	Invalid

l00008369:
	svcmi	#&461446

l0000836D:
	movhi	fp,#&3FF0

l00008371:
	svclt	#&8811F3

l00008375:
	svclt	#&8F6FF3

l00008379:
	strdeq	r4,r5,[pc],r3                                       ; 00008381

l0000837D:
	bge	$FFE47745

l00008381:
	bls	$01B03135

l00008385:
	sbcseq	r1,r0,r2,asr #8

l00008389:
	ldrshteq	r1,[r9],#&20

l0000838D:
	mvnsne	r8,r0,lsr #&20

l00008391:
	svclt	#&F04F88

l00008395:
	mvnsne	r8,r3,lsl #6

l00008399:
	svcvs	#&F3BF88

l0000839D:
	svcmi	#&F3BF8F

l000083A1:
	bl	$01AF2DE5
	beq	$010AED59

l000083A9:
	msrhi	cpsr,#&D3

l000083AD:
	Invalid

l000083B1:
	Invalid
	ldrthi	r7,[r9],#&CF8
	Invalid
	adcseq	r7,sp,#&46
	stmhs	r6,{r1-r2,r6,r8,ip-sp}
	mvnsls	pc,r6,asr #&E
	blvc	$01AA33C9
	strhteq	r0,[r0],-#&19
	mvnsne	r8,r3,lsr #6
	ldreq	r7,[sp,#&88]!
	Invalid
	ldrbhi	r0,[r0,r6,asr #32]!
	msrhi	cpsr,#&FD
	svcvs	#&8811F3
	strdvc	r0,r1,[r0],-r0
	ldrbths	r0,[r1],#&5BD
	svcmi	#&F00000
	stmdb	r8!,{r1-r7}
	ldrbteq	r6,[r0],#&FD0
	stmdblo	r7,{r10,ip,lr-pc}^

;; xQueueCRReceive: 00008401
xQueueCRReceive proc
	svcmi	#&4604B5

l00008405:
	movhi	fp,#&3FF0

l00008409:
	svclt	#&8811F3

l0000840D:
	svclt	#&8F6FF3

l00008411:
	strhi	r4,[pc,#&FF3]                                        ; 0000940C

l00008415:
	adcseq	r2,r9,fp,ror #&1A

l00008419:
	sbcshi	r3,r1,#&2A00000

l0000841D:
	Invalid

l00008421:
	adcseq	r3,sp,r6,asr #&10

l00008425:
	mvnsne	r8,r3,lsr #6

l00008429:
	svclt	#&F04F88

l0000842D:
	mvnsne	r8,r3,lsl #6

l00008431:
	svcvs	#&F3BF88

l00008435:
	svcmi	#&F3BF8F

l00008439:
	rsbhs	r8,fp,#&F0000008

l0000843D:
	strheq	r1,[r6],-#&9

l00008441:
	mvnsne	r8,r3,lsr #6

l00008445:
	popeq	{r3,r7,fp-sp}

l00008449:
	Invalid
	eretne
	movtge	r9,#&2944
	stmhs	r0!,{r0-r1,r3,r5-r6,r8,sp-pc}
	msreq	spsr,#&1BF
	teqge	r3,#&3C4
	rors	r2,r3,#&10
	Invalid
	msrhs	spsr,#&3F8
	strhteq	r0,[r0],-#&19
	mvnsne	r8,r3,lsr #6
	ldrteq	r3,[sp],#&888
	Invalid
	ldrshteq	r0,[lr],#&A0
	svcvs	#&D0F428
	ble	$0000984D
	ldrbths	r0,[r1],#&E7
	subeq	r1,r6,r1
	ldrbhi	r2,[sp,#&EF0]!
	svcvs	#&8811F3
	stmdalo	r0,{r4-r9}
	ldrhtvc	r0,[pc],#&D                                        ; 000084B2

;; xQueueCRSendFromISR: 000084A1
xQueueCRSendFromISR proc
	Invalid

l000084A5:
	Invalid

l000084A9:
	ldmdbhs	r3,{r1,r6,r8}^

l000084AD:
	adcseq	r7,sp,r6,asr #&20

l000084B1:
	Invalid
	ldrshteq	r1,[lr],#&A7
	bicsvs	pc,r1,#&B40000
	Invalid
	ldrbths	r0,[r1],#&4D0
	ldrb	r0,[r0]!
	ldmdane	ip,{r0,r2-r8,r10}
	stc	p1,c0,[r5],-#&2FC
	Invalid

;; xQueueCRReceiveFromISR: 000084D5
xQueueCRReceiveFromISR proc
	msr	spsr,#&3B5
	strhtle	ip,[r8],-#&31

l000084DD:
	strbtmi	r4,[r0],#&F8

l000084E1:
	movtge	r7,#&4368

l000084E5:
	strbeq	r0,[r6],-#&E42

l000084E9:
	strbhi	r1,[r6,-r6,asr #10]

l000084ED:
	stmhs	r0!,{r0-r1,r3,r5-r6,r8-r9,lr-pc}

l000084F1:
	Invalid

l000084F5:
	ldmdahs	r7!,{r0,r4-pc}

l000084F9:
	stmne	r0,{r0-r5,r7-r9,lr-pc}^

l000084FD:
	sublo	r7,r6,r6,asr #4

l00008501:
	rsbeq	r10,r3,#&1180000

l00008505:
	blhs	$FFE200CD

l00008509:
	movshs	r0,#&1A000

l0000850D:
	rorseq	r1,r9,#&16

l00008511:
	popne	{r5,fp-pc}

l00008515:
	ldrteq	pc,[sp],#&846

l00008519:
	Invalid

l0000851D:
	ldrshteq	fp,[sp],#&A0

l00008521:
	bicseq	pc,r0,r8,lsr #&A

l00008525:
	Invalid
	ldmeq	pc!,{r0,r2-r5,r7}

;; prvIdleTask: 0000852D
prvIdleTask proc
	svcvs	#&F7FFB5

l00008531:
	stmdblo	r7,{r1-r7,r10-pc}^

;; xTaskNotifyStateClear: 00008535
xTaskNotifyStateClear proc
	ldrteq	r7,[r1],#&8B5

l00008539:
	Invalid

l0000853D:
	ldrbtvs	r9,[r8],#&4F8

l00008541:
	streq	r0,[fp,-#&230]!

l00008545:
	strheq	r0,[r3,-pc]!

l00008549:
	strthi	r0,[r5],-#&25

l0000854D:
	ldrshteq	r6,[r0],-#&48

l00008551:
	ldmdbhs	r8,{r4-r7,r9-fp,sp}^

l00008555:
	asrseq	r3,r6,#&10

l00008559:
	stc	p12,c5,[r8,-#&12C]!
	ldrtgt	r0,[pc],#&E7                                        ; 0000864C

l00008561:
	svc	#&200000

;; xPortRaisePrivilege: 00008565
xPortRaisePrivilege proc
	Invalid

l00008569:
	bne	$003C8D31

l0000856D:
	eoreq	r0,r0,#&BF

l00008571:
	Invalid

l00008575:
	stmdaeq	r0!,{r0-r2,r6}

;; vPortEnterCritical: 00008579
vPortEnterCritical proc
	Invalid
	svclt	#&F04FFF
	mvnsne	r8,r3,lsl #6
	svcvs	#&F3BF88
	svcmi	#&F3BF8F
	smlalbbeq	r0,r10,pc,r7
	msreq	spsr,#&328
	movne	r0,#&31F1
	svc	#&D00560
	Invalid
	strdhi	r0,r1,[r0],-r0
	stmeq	r8,{r0-r1,r4-r7,r10,ip}
	Invalid
	stmdaeq	r0!,}

;; vPortExitCritical: 000085B1
vPortExitCritical proc
	Invalid

l000085B5:
	movtne	r0,#&A8FF

l000085B9:
	teqne	fp,#&1A

l000085BD:
	movshi	r0,#&18000

l000085C1:
	Invalid

l000085C5:
	svc	#&D00528
	Invalid

l000085CD:
	strdhi	r0,r1,[r0],-r0

l000085D1:
	stmeq	r8,{r0-r1,r4-r7,r10,ip}

l000085D5:
	Invalid

l000085D9:
	stmdaeq	r0!,}

;; vParTestInitialise: 000085DD
vParTestInitialise proc
	svcne	#&F7FFB5

l000085E1:
	strbeq	r0,[fp,-#&3FE]

l000085E5:
	Invalid

l000085E9:
	svc	#&4008E8
	Invalid
	stmdalo	r0!,{r0-r2}

;; vParTestSetLED: 000085F5
vParTestSetLED proc
	stceq	p4,c0,[r6,-#&2D4]

l000085F9:
	mvnsvs	r0,r6,asr #&20

l000085FD:
	stceq	p7,c0,[ip],-#&3E4

l00008601:
	msreq	cpsr,#&1D8

l00008605:
	ldmdbeq	r0,{r1,r3-r7,r10}^

l00008609:
	bne	$FECB873D

l0000860D:
	adcsne	r4,r1,r8,ror sp

l00008611:
	ldmne	r0,{r0-r1,r6,fp-ip}^

l00008615:
	svc	#&200578
	Invalid

l0000861D:
	subeq	r3,r0,r8,ror #&11

l00008621:
	adcshs	r5,r9,#&F00

l00008625:
	stmdane	r0,{r1,r3,r5-r7}

l00008629:
	Invalid
	eorne	r0,r0,r7

;; vParTestToggleLED: 00008631
vParTestToggleLED proc
	strheq	r0,[r6],-#&45

l00008635:
	Invalid

l00008639:
	bicseq	r0,r8,ip,lsr #&1C

l0000863D:
	subeq	r0,fp,#&8800

l00008641:
	ldmibne	r0,{r1,r3-r7,r10}^

l00008645:
	beq	$FECB902D

l00008649:
	ldmibne	r1,{r1,r6,r9,fp}^

l0000864D:
	bne	$010CB035

l00008651:
	ldrbeq	r1,[r8,-#&970]!

l00008655:
	mvnshs	pc,r0,lsr #&1E

l00008659:
	Invalid

l0000865D:
	svclo	#&F00040

l00008661:
	rsbshs	r1,r8,#&B9000

l00008665:
	stmdane	r0,{r1,r3,r5-r7}

l00008669:
	Invalid
	eorvc	r0,r0,r7

;; prvFlashCoRoutine: 00008671
prvFlashCoRoutine proc
	addhi	r8,lr,#&D4000002

l00008675:
	ldrh	fp,[r5,#&30]!
	blne	$0118987D

l0000867D:
	mvnsgt	r4,#&D0

l00008681:
	subeq	r9,r2,#&48000000

l00008685:
	adcseq	r2,r3,#&40000003

l00008689:
	ldrtne	r7,[sp],#&B0

l0000868D:
	asreq	r0,sp,#2

l00008691:
	Invalid

l00008695:
	svc	#&F04FFF
	stmhs	r6,{r1,r4-r5,r8,ip-sp}

l0000869D:
	svcge	#&F7FF68

l000086A1:
	ldmdane	sp,{r1-r7,r9}

l000086A5:
	Invalid

l000086A9:
	Invalid
	stceq	p0,c0,[r2],-#&340
	svc	#&601A4B
	smlaltteq	r0,sp,r7,r10
	smultblo	r8,lr,r8
	svc	#&220046
	mvnsmi	r9,#&F70
	sbcsmi	pc,r1,sp,lsl r0
	tstge	r3,#&C8000003
	adcsvc	r0,r0,r6,lsl #5
	strheq	r0,[sp,-#&3D]
	svcmi	#&E7DEAE
	cmnge	r3,#&3D
	Invalid
	eorgt	r0,r0,r7
	eorne	r0,r0,r0

;; prvFixedDelayCoRoutine: 000086E9
prvFixedDelayCoRoutine proc
	addhi	r8,lr,#&D4000002

l000086ED:
	ldrhgt	fp,[r5,#&30]!

l000086F1:
	hvceq	#&604F

l000086F5:
	Invalid

l000086F9:
	mvnshi	r4,#&D9

l000086FD:
	stmeq	r2,{r1,r4,r8-r9,ip,pc}^

l00008701:
	Invalid

l00008705:
	rscshs	r5,r8,#&68000002

l00008709:
	svcmi	#&BB4000

l0000870D:
	cmnge	r3,#&3D000

l00008711:
	adcsne	r0,r0,r6,lsl #5

l00008715:
	blgt	$FFD75611

l00008719:
	Invalid

l0000871D:
	stmdane	r2!,{r0-r1,r3,r6}

l00008721:
	svc	#&A90168
	rscseq	r1,lr,#&F70

l00008729:
	bicsmi	r2,r0,#&1D

l0000872D:
	bicseq	r1,r0,sp,lsl r10

l00008731:
	sbcseq	lr,r0,r8,lsr #&C

l00008735:
	bne	$012CCFC5

l00008739:
	Invalid

l0000873D:
	rscshs	r5,r8,#&68000002

l00008741:
	eor	r0,r8,#0
	ldrdeq	r0,r1,[r0],#&A0

l00008749:
	sbcseq	lr,r0,#&AC0000

l0000874D:
	beq	$FEF4CA15

l00008751:
	stmdane	r2!,{r0-r1,r3,r6}

l00008755:
	svc	#&A90168
	Invalid
	eoreq	r0,r1,r7,ror #1
	rscsle	ip,fp,#&F000000
	mvnshi	r4,#&E7
	orrle	r10,r6,r3,lsl r3
	mvnsgt	r4,r7,ror #&1F
	Invalid
	ldrthi	r0,[pc],#&E7                                        ; 00008864
	Invalid
	eorgt	r0,r0,r7
	stmdaeq	r0!,}

;; vStartFlashCoRoutines: 00008785
vStartFlashCoRoutines proc
	ldmeq	pc!,{r3,r5,fp,sp}

l00008789:
	adcseq	r7,r5,r0,lsr #&20

l0000878D:
	strbeq	r0,[r6],-#&522

l00008791:
	eoreq	r0,r0,r1,lsr #2

l00008795:
	beq	$FFE66B5D

l00008799:
	stmhi	r0!,{r0-r1,r3,r6,fp-ip}

l0000879D:
	ldrhteq	r4,[r1],#&D1

l000087A1:
	subhs	r0,lr,#&90000

l000087A5:
	crc32heq	r0,r1,r6

l000087A9:
	subeq	r3,r6,r4,lsr r0

l000087AD:
	Invalid

l000087B1:
	sbcseq	pc,r1,r2,asr #&E

l000087B5:
	rscvc	fp,r8,r2,lsr #&1A

l000087B9:
	msreq	cpsr,#&140

l000087BD:
	svclo	#&F00048

l000087C1:
	Invalid
	stmdb	r0!,{r0-r2}
	smlabbvc	r0,r6,r0,r0
	smlabbeq	r0,r6,r0,r0

;; xAreFlashCoRoutinesStillRunning: 000087D1
xAreFlashCoRoutinesStillRunning proc
	rsbvc	r1,r8,fp,asr #&10

l000087D5:
	adcsgt	r0,pc,r7,asr #&20

l000087D9:
	eorvc	r0,r0,r0

;; MPU_xTaskCreateRestricted: 000087DD
MPU_xTaskCreateRestricted proc
	Invalid

l000087E1:
	svclt	#&F7FF46

l000087E5:
	strbeq	r3,[r6],-#&1FE

l000087E9:
	Invalid
	ldrsheq	r9,[r8,#&67]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subvc	r1,r6,r8,lsl #&11
	Invalid

;; MPU_xTaskCreate: 00008809
MPU_xTaskCreate proc
	subhi	pc,r7,#&E9

l0000880D:
	stmhi	r6,{r4-r5,r7-r8,r10}

l00008811:
	bls	$011ACD31

l00008815:
	bleq	$FE7CB135

l00008819:
	mvnsge	pc,#&278

l0000881D:
	strbeq	r5,[r6],-#&3FE

l00008821:
	orrseq	r0,r7,r6,asr #&20

l00008825:
	swpbmi	r4,r6,[r6]

l00008829:
	Invalid
	ldrsheq	r4,[r8,#&27]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subeq	r1,r6,#&880000
	Invalid
	adcsvc	r0,pc,r7,lsl #1

;; MPU_vTaskAllocateMPURegions: 0000884D
MPU_vTaskAllocateMPURegions proc
	Invalid

l00008851:
	ldrbhi	pc,[r7,r6,asr #30]!

l00008855:
	strdlo	r0,r1,[r6,-#&4E]

l00008859:
	Invalid
	ldrsheq	r8,[r8,#&87]!
	svc	#&D0052C
	Invalid
	strdhi	r0,r1,[r0],-r0
	Invalid
	ldrhtvc	r0,[pc],#&D                                        ; 00008886

;; MPU_vTaskDelayUntil: 00008875
MPU_vTaskDelayUntil proc
	Invalid

l00008879:
	mvnsvc	pc,#&118

l0000887D:
	strdlo	r0,r1,[r6,-#&4E]

l00008881:
	Invalid
	ldrsheq	r7,[fp,#&C7]!
	svc	#&D0052C
	Invalid
	strdhi	r0,r1,[r0],-r0
	Invalid
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_vTaskDelay: 0000889D
MPU_vTaskDelay proc
	svc	#&4605B5
	ldrbteq	r6,[lr],#&F7

l000088A5:
	Invalid
	ldrsheq	r4,[fp,#&E7]!
	svc	#&D0052C
	Invalid
	strdhi	r0,r1,[r0],-r0
	stmlo	r8,{r0-r1,r4-r7,r10,ip}
	ldrhtne	r0,[pc],#&D                                        ; 000088D2

;; MPU_vTaskSuspendAll: 000088C1
MPU_vTaskSuspendAll proc
	svcmi	#&F7FFB5

l000088C5:
	Invalid
	ldrsheq	r10,[r8,#&7]!
	svc	#&D0052C
	Invalid
	strdhi	r0,r1,[r0],-r0
	Invalid
	ldrhtne	r0,[pc],#&D                                        ; 000088F2

;; MPU_xTaskResumeAll: 000088E1
MPU_xTaskResumeAll proc
	svclo	#&F7FFB5

l000088E5:
	Invalid
	ldrsheq	ip,[r10,#&7]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subne	r1,r6,r8,lsl #&11
	ldrhtne	r0,[pc],#&D                                        ; 00008916

;; MPU_xTaskGetTickCount: 00008905
MPU_xTaskGetTickCount proc
	Invalid

l00008909:
	Invalid
	ldrsheq	r8,[r8,#&87]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subne	r1,r6,r8,lsl #&11
	ldrhtne	r0,[pc],#&D                                        ; 0000893A

;; MPU_uxTaskGetNumberOfTasks: 00008929
MPU_uxTaskGetNumberOfTasks proc
	blne	$FFE08805

l0000892D:
	Invalid
	ldrsheq	r8,[r8,#&27]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subne	r1,r6,r8,lsl #&11
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_pcTaskGetName: 0000894D
MPU_pcTaskGetName proc
	svc	#&4605B5
	ldrbteq	r0,[lr],#&8F7

l00008955:
	Invalid
	ldrsheq	r7,[r8,#&47]!

;; fn0000895D: 0000895D
fn0000895D proc
	strbeq	r0,[r6,-#&32C]

l00008961:
	ldrbtne	lr,[r3],#&FD0

l00008965:
	mvnseq	r4,r0,lsl #1

l00008969:
	ldrbtne	r8,[r3]

l0000896D:
	stmlo	r6,{r3,r7,fp-ip}

l00008971:
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_vTaskSetTimeOutState: 00008975
MPU_vTaskSetTimeOutState proc
	svc	#&4605B5
	ldrbteq	pc,[sp],#&4F7

l0000897D:
	Invalid
	ldrsheq	lr,[fp,#&7]!
	svc	#&D0052C
	Invalid
	strdhi	r0,r1,[r0],-r0
	stmlo	r8,{r0-r1,r4-r7,r10,ip}
	ldrhtvc	r0,[pc],#&D                                        ; 000089AA

;; MPU_xTaskCheckForTimeOut: 00008999
MPU_xTaskCheckForTimeOut proc
	Invalid

l0000899D:
	mvns	pc,r6,asr #&1E
	strbeq	r3,[r6],-#&1FD

l000089A5:
	Invalid
	ldrsheq	sp,[fp,#&67]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subvc	r1,r6,r8,lsl #&11
	Invalid

;; MPU_xTaskGenericNotify: 000089C5
MPU_xTaskGenericNotify proc
	strbeq	pc,[r1,-#&E9]

l000089C9:
	strbne	r0,[r6,-r6,asr #28]

l000089CD:
	svc	#&469846
	mvnsmi	ip,#&F70000

l000089D5:
	blo	$01189AF5

l000089D9:
	stmhs	r6,{r1-r2,r6,r8,ip-sp}

l000089DD:
	bllo	$FFE06AFD

l000089E1:
	msreq	cpsr,#&1F8

l000089E5:
	svc	#&D00546
	Invalid

l000089ED:
	strdhi	r0,r1,[r0],-r0

l000089F1:
	stmne	r8,{r0-r1,r4-r7,r10,ip}

l000089F5:
	Invalid
	Invalid

;; MPU_xTaskNotifyWait: 000089FD
MPU_xTaskNotifyWait proc
	strbeq	pc,[r1,-#&E9]

l00008A01:
	strbne	r0,[r6,-r6,asr #28]

l00008A05:
	svc	#&469846
	mvnsmi	r10,#&F700

l00008A0D:
	blo	$01189B2D

l00008A11:
	stmhs	r6,{r1-r2,r6,r8,ip-sp}

l00008A15:
	Invalid

l00008A19:
	msreq	cpsr,#&1F8

l00008A1D:
	svc	#&D00546
	Invalid

l00008A25:
	strdhi	r0,r1,[r0],-r0

l00008A29:
	stmne	r8,{r0-r1,r4-r7,r10,ip}

l00008A2D:
	Invalid
	adcsvc	r0,pc,r1,lsl #1

;; MPU_ulTaskNotifyTake: 00008A35
MPU_ulTaskNotifyTake proc
	Invalid

l00008A39:
	mvnsls	pc,#&118

l00008A3D:
	strbeq	r3,[r6],-#&1FD

l00008A41:
	Invalid
	ldrsheq	r5,[r9,#&C7]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subvc	r1,r6,r8,lsl #&11
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_xTaskNotifyStateClear: 00008A61
MPU_xTaskNotifyStateClear proc
	svc	#&4605B5
	ldrbteq	r7,[sp],#&EF7

l00008A69:
	svc	#&462846
	ldrsheq	r6,[sp,#&27]!

l00008A71:
	strbeq	r0,[r6,-#&32C]

l00008A75:
	ldrbtne	lr,[r3],#&FD0

l00008A79:
	mvnseq	r4,r0,lsl #1

l00008A7D:
	ldrbtne	r8,[r3]

l00008A81:
	stmlo	r6,{r3,r7,fp-ip}

l00008A85:
	Invalid

;; MPU_xQueueGenericCreate: 00008A89
MPU_xQueueGenericCreate proc
	Invalid

l00008A8D:
	svc	#&461746
	blo	$FFF62E75

l00008A95:
	crc32wlo	r0,r6,r6

l00008A99:
	Invalid
	ldrsheq	r0,[lr,#&67]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	Invalid
	ldrhtvc	r0,[pc],#&D                                        ; 00008ACA

;; MPU_xQueueGenericReset: 00008AB9
MPU_xQueueGenericReset proc
	Invalid

l00008ABD:
	mvnspl	pc,r6,asr #&1E

l00008AC1:
	strbeq	r3,[r6],-#&1FD

l00008AC5:
	Invalid
	ldrsheq	fp,[sp,#&27]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subvc	r1,r6,r8,lsl #&11
	Invalid

;; MPU_xQueueGenericSend: 00008AE5
MPU_xQueueGenericSend proc
	strbeq	pc,[r1,-#&E9]

l00008AE9:
	strbne	r0,[r6,-r6,asr #28]

l00008AED:
	svc	#&469846
	mvnsmi	r3,#&F70000

l00008AF5:
	blo	$01189C15

l00008AF9:
	stmhs	r6,{r1-r2,r6,r8,ip-sp}

l00008AFD:
	ldrbmi	pc,[r7,r6,asr #14]!

l00008B01:
	msreq	cpsr,#&1FB

l00008B05:
	svc	#&D00546
	Invalid

l00008B0D:
	strdhi	r0,r1,[r0],-r0

l00008B11:
	stmne	r8,{r0-r1,r4-r7,r10,ip}

l00008B15:
	Invalid
	ldmlo	pc!,{r0,r7}

;; MPU_uxQueueMessagesWaiting: 00008B1D
MPU_uxQueueMessagesWaiting proc
	svc	#&4605B5
	ldrbteq	r2,[sp],#&F7

l00008B25:
	Invalid
	ldrsheq	r7,[ip,#&E7]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	stmlo	r6,{r3,r7,fp-ip}
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_uxQueueSpacesAvailable: 00008B45
MPU_uxQueueSpacesAvailable proc
	svc	#&4605B5
	ldrbteq	r0,[sp],#&CF7

l00008B4D:
	Invalid
	ldrsheq	r7,[ip,#&47]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	stmlo	r6,{r3,r7,fp-ip}
	Invalid

;; MPU_xQueueGenericReceive: 00008B6D
MPU_xQueueGenericReceive proc
	strbeq	pc,[r1,-#&E9]

l00008B71:
	strbne	r0,[r6,-r6,asr #28]

l00008B75:
	svc	#&469846
	mvnsmi	pc,#&F7000000

l00008B7D:
	blo	$01189C9D

l00008B81:
	stmhs	r6,{r1-r2,r6,r8,ip-sp}

l00008B85:
	ldrbge	pc,[r7,r6,asr #14]!

l00008B89:
	msreq	cpsr,#&1FB

l00008B8D:
	svc	#&D00546
	Invalid

l00008B95:
	strdhi	r0,r1,[r0],-r0

l00008B99:
	stmne	r8,{r0-r1,r4-r7,r10,ip}

l00008B9D:
	Invalid
	adcsvc	r0,pc,r1,lsl #1

;; MPU_xQueuePeekFromISR: 00008BA5
MPU_xQueuePeekFromISR proc
	Invalid

l00008BA9:
	blle	$FFE088C9

l00008BAD:
	strbeq	r3,[r6],-#&1FC

l00008BB1:
	Invalid
	ldrsheq	r7,[fp,#&67]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subvc	r1,r6,r8,lsl #&11
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_xQueueGetMutexHolder: 00008BD1
MPU_xQueueGetMutexHolder proc
	svc	#&4605B5
	ldrbteq	ip,[ip],#&6F7

l00008BD9:
	Invalid
	ldrsheq	lr,[ip,#&A7]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	stmlo	r6,{r3,r7,fp-ip}
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_xQueueCreateMutex: 00008BF9
MPU_xQueueCreateMutex proc
	svc	#&4605B5
	ldrbteq	fp,[ip],#&2F7

l00008C01:
	Invalid
	ldrsheq	r6,[sp,#&A7]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	stmlo	r6,{r3,r7,fp-ip}
	ldrhtvc	r0,[pc],#&D                                        ; 00008C32

;; MPU_xQueueTakeMutexRecursive: 00008C21
MPU_xQueueTakeMutexRecursive proc
	Invalid

l00008C25:
	Invalid

l00008C29:
	strbeq	r3,[r6],-#&1FC

l00008C2D:
	Invalid
	ldrsheq	sp,[ip,#&7]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subvc	r1,r6,r8,lsl #&11
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_xQueueGiveMutexRecursive: 00008C4D
MPU_xQueueGiveMutexRecursive proc
	svc	#&4605B5
	ldrbteq	r8,[ip],#&8F7

l00008C55:
	Invalid
	ldrsheq	sp,[ip,#&47]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	stmlo	r6,{r3,r7,fp-ip}
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_vQueueDelete: 00008C75
MPU_vQueueDelete proc
	svc	#&4605B5
	ldrbteq	r7,[ip],#&4F7

l00008C7D:
	Invalid
	ldrsheq	lr,[fp,#&87]!
	svc	#&D0052C
	Invalid
	strdhi	r0,r1,[r0],-r0
	stmlo	r8,{r0-r1,r4-r7,r10,ip}
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_pvPortMalloc: 00008C99
MPU_pvPortMalloc proc
	svc	#&4605B5
	ldrbteq	r6,[ip],#&2F7

l00008CA1:
	Invalid
	ldrsheq	r4,[sp,#&27]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	stmlo	r6,{r3,r7,fp-ip}
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_vPortFree: 00008CC1
MPU_vPortFree proc
	svc	#&4605B5
	ldrbteq	r4,[ip],#&EF7

l00008CC9:
	Invalid
	ldrsheq	r5,[sp,#&87]!
	svc	#&D0052C
	Invalid
	strdhi	r0,r1,[r0],-r0
	stmlo	r8,{r0-r1,r4-r7,r10,ip}
	ldrhtne	r0,[pc],#&D                                        ; 00008CF6

;; MPU_vPortInitialiseBlocks: 00008CE5
MPU_vPortInitialiseBlocks proc
	Invalid

l00008CE9:
	Invalid
	ldrsheq	r4,[sp,#&A7]!
	svc	#&D0052C
	Invalid
	strdhi	r0,r1,[r0],-r0
	Invalid
	ldrhtne	r0,[pc],#&D                                        ; 00008D16

;; MPU_xPortGetFreeHeapSize: 00008D05
MPU_xPortGetFreeHeapSize proc
	Invalid

l00008D09:
	Invalid
	ldrsheq	r4,[sp,#&27]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subne	r1,r6,r8,lsl #&11
	ldrhtne	r0,[pc],#&D                                        ; 00008D3A

;; MPU_xEventGroupCreate: 00008D29
MPU_xEventGroupCreate proc
	blne	$FFE08C05

l00008D2D:
	Invalid
	ldrsheq	r3,[sp,#&A7]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subne	r1,r6,r8,lsl #&11
	Invalid

;; MPU_xEventGroupWaitBits: 00008D4D
MPU_xEventGroupWaitBits proc
	movthi	pc,#&30E9

l00008D51:
	Invalid

l00008D55:
	stmls	r6,{r1-r2,r6,ip,pc}^

l00008D59:
	svc	#&9F0A46
	blmi	$FFF09941

l00008D61:
	subeq	r0,r6,r6,asr #8

l00008D65:
	swpblo	r4,r7,[r6]

l00008D69:
	Invalid
	ldrsheq	r2,[sp,#&A7]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	movteq	r1,#&6888
	Invalid
	adcsvc	r0,pc,r3,lsl #1

;; MPU_xEventGroupClearBits: 00008D8D
MPU_xEventGroupClearBits proc
	Invalid

l00008D91:
	ldrb	pc,[r7,r6,asr #30]!
00008D95                FB 31 46 04 46 28 46 F8 F7 6A FD      .1F.F(F..j.
00008DA0 01 2C 03 46 05 D0 EF F3 14 80 40 F0 01 00 80 F3 .,.F......@.....
00008DB0 14 88 18 46 70 BD 00 BF 70                      ...Fp...p      

;; MPU_xEventGroupSetBits: 00008DB9
MPU_xEventGroupSetBits proc
	Invalid

l00008DBD:
	mvnsle	pc,r6,asr #&1E

l00008DC1:
	strbeq	r3,[r6],-#&1FB

l00008DC5:
	Invalid
	ldrsheq	r6,[sp,#&27]!
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3]
	subvc	r1,r6,r8,lsl #&11
	Invalid

;; MPU_xEventGroupSync: 00008DE5
MPU_xEventGroupSync proc
	strbeq	pc,[r1,-#&E9]

l00008DE9:
	strbne	r0,[r6,-r6,asr #28]

l00008DED:
	svc	#&469846
	mvnsmi	fp,#&F70000

l00008DF5:
	blo	$01189F15

l00008DF9:
	stmhs	r6,{r1-r2,r6,r8,ip-sp}

l00008DFD:
	blvc	$FFE06F1D

l00008E01:
	msreq	cpsr,#&1FD

l00008E05:
	svc	#&D00546
	Invalid

l00008E0D:
	strdhi	r0,r1,[r0],-r0

l00008E11:
	stmne	r8,{r0-r1,r4-r7,r10,ip}

l00008E15:
	Invalid
	ldmlo	pc!,{r0,r7}

;; MPU_vEventGroupDelete: 00008E1D
MPU_vEventGroupDelete proc
	svc	#&4605B5
	ldrbteq	r10,[fp],#&F7

l00008E25:
	Invalid
	ldrsheq	fp,[sp,#&C7]!
	svc	#&D0052C
	Invalid
	strdhi	r0,r1,[r0],-r0
	stmlo	r8,{r0-r1,r4-r7,r10,ip}
	Invalid

;; xCoRoutineCreate: 00008E41
xCoRoutineCreate proc
	smlaltthi	pc,pc,r9,r8

l00008E45:
	Invalid

l00008E49:
	Invalid
	ldrshteq	r6,[ip],#&E7
	ldrbhs	r4,[r0,#&728]
	bllo	$01189F99
	ldreq	r3,[r3,r8,ror #22]!
	strdeq	r0,r1,[r8,-r1,ror #9]
	lsrseq	r2,sp,#&10
	strths	r0,[r3],-r5
	str	r10,[r6,#&346]
	rscslo	ip,r8,r2,ror #8
	ldrbteq	r4,[r8],#&6A0
	svc	#&46309B
	ldrbteq	r3,[r10],#&6F7
	svc	#&18F1
	ldrsht	r3,[r10],#&27
	strbgt	r3,[pc,-#&B6A]!                                     ; 000099F7
	stmdals	r5,{r0,r4-r7,r9}
	ldmlo	pc!,{r1,r6,fp,pc}
	rschi	r0,fp,r7,rrx
	rschi	r0,fp,r0,lsl #&10
	strbths	r10,[r1],-#&500
	eretlo
	ldrbhs	pc,[r7,#&F46]!
	stclt	p1,c0,[r0,-#&3E8]!
	stmlt	pc,{r3,r5-r7,fp-pc}
	ldrbteq	r4,[r8],#&846
	svc	#&46400B
	Invalid
	Invalid
	svc	#&18F1
	Invalid
	stmdapl	r6,{r0,r4-r7,lr}
	mvnseq	pc,r6,asr #&1E
	svc	#&4630FA
	Invalid
	svc	#&54F1
	Invalid
	Invalid
	svcmi	#&E7BD66
	ldclt	p15,c15,[r0,-#&3C0]!
	Invalid
	eorvc	r0,r0,r7

;; vCoRoutineAddToDelayedList: 00008EF1
vCoRoutineAddToDelayedList proc
	mcrreq	p14,#&B,r0,r6,c5

l00008EF5:
	strbvs	r2,[r8,-#&34C]!

l00008EF9:
	stmne	r4,{r0-r3,r5-r6,r8,r10}

l00008EFD:
	svcne	#&F7FF1D

l00008F01:
	Invalid

l00008F05:
	stcmi	p13,c9,[r2,-#&1A0]

l00008F09:
	adcs	r3,pc,r0,ror #8
	strbteq	r10,[lr],-#&6E

l00008F11:
	blx	$FFE08BDF
	ldrshhs	r3,[r1,r9]!

l00008F19:
	stclt	p0,c3,[r6,-#&1A0]

l00008F1D:
	stmne	r0,{r3,r5-r7,ip-lr}

l00008F21:
	Invalid
	Invalid
	stchs	p0,c0,[r0,-#&1C]!

;; vCoRoutineSchedule: 00008F2D
vCoRoutineSchedule proc
	strbpl	pc,[r1,-#&E9]

l00008F31:
	blhs	$01B63C6D

l00008F35:
	streq	r0,[r7,-#&B3]!

l00008F39:
	svcmi	#&804F1

l00008F3D:
	movhi	fp,#&3FF0

l00008F41:
	svclt	#&8811F3

l00008F45:
	svclt	#&8F6FF3

l00008F49:
	blhs	$FE3DCF1D

l00008F4D:
	strbteq	sp,[r8],-#&C6E

l00008F51:
	svc	#&18F1
	Invalid

l00008F59:
	Invalid

l00008F5D:
	svc	#&46301D
	mvns	lr,#&F70
	msreq	spsr,#&A6A

l00008F69:
	movls	r8,#&3EB

l00008F6D:
	stmeq	r6,{r1,r6,r8,ip-sp}

l00008F71:
	stmdahi	r0,{r0-r1,r3,r5-r7,pc}

l00008F75:
	svc	#&672BBF
	blvs	$FFE77B5D

l00008F7D:
	stcle	p0,c0,[fp],-#&1B4

l00008F81:
	svclt	#&F7FFD1

l00008F85:
	bge	$009C937D

l00008F89:
	rsbhi	r6,pc,pc,ror #&16

l00008F8D:
	rscseq	sp,r8,r10,lsl pc

l00008F91:
	rsbeq	lr,r7,r1,lsl #&11

l00008F95:
	bicseq	r3,r0,r8,lsr #&1A

l00008F99:
	blvs	$00E0946D

l00008F9D:
	rsbeq	lr,r7,r7,ror #&10

l00008FA1:
	bge	$FF41DC55

l00008FA5:
	rsbeq	r1,r8,lr,ror #2

l00008FA9:
	sbcsle	pc,r0,#&A4000000

l00008FAD:
	rsbvs	sp,r8,#&68000000

l00008FB1:
	strbeq	r9,[r2],-r8

l00008FB5:
	ble	$FFA04705

l00008FB9:
	strbtle	r6,[pc],-#&B68                                     ; 00009B29

l00008FBD:
	bls	$01A21965

l00008FC1:
	svcmi	#&D82442

l00008FC5:
	movhi	fp,#&3FF0

l00008FC9:
	svclt	#&8811F3

l00008FCD:
	svclt	#&8F6FF3

l00008FD1:
	Invalid

l00008FD5:
	svc	#&46301D
	mvnsge	fp,#&7000000F

l00008FDD:
	ldmdbne	r1,{r1,r3,r5-r6,r10}^

l00008FE1:
	svc	#&B10B00
	Invalid

l00008FE9:
	orr	r1,r8,#&C000003C
	msreq	spsr,#&A6A

l00008FF1:
	movls	r8,#&3EB

l00008FF5:
	stmeq	r6,{r1,r6,r8,ip-sp}

l00008FF9:
	stmdahi	r0,{r0-r1,r3,r5-r7,pc}

l00008FFD:
	svc	#&672BBF
	blge	$FFE66BE5

l00009005:
	rsbeq	r1,r8,lr,ror #&14

l00009009:
	blvs	$FF47E4B9

l0000900D:
	rsbeq	lr,pc,pc,ror #&10

l00009011:
	ldmibhs	r1,{r3,r5,r8,lr-pc}^

l00009015:
	blhi	$019F3DD9

l00009019:
	ldreq	r5,[r8,-#&A00]

l0000901D:
	andpl	r8,r2,#&B000000E

l00009021:
	Invalid

l00009025:
	bmi	$FECDF771

l00009029:
	stmdals	r0,{r1-r4,r8-r9,ip,pc}

l0000902D:
	rschi	r0,fp,r8,lsl r5

l00009031:
	stmvc	r8!,{lr}

l00009035:
	bhi	$FEC55B21

l00009039:
	stmdbls	r0,{r1-r4,r8-r9,ip,pc}

l0000903D:
	mvnhi	fp,r8,lsl r5

l00009041:
	stmlo	r8,{r0,r8,fp,lr}^

l00009045:
	Invalid

l00009049:
	stmibge	r1,{r3,r5-r7,ip-pc}

l0000904D:
	stm	lr,{r1-r3,r5-r6,r9,fp,sp-pc}^
	strbge	r10,[r6,-r6,ror #20]!

l00009055:
	msrne	spsr,#&AE7

l00009059:
	stmdb	r0,{r2,r6,r8-r9,fp-ip,pc}
	beq	$01A2B8C5

l00009061:
	msreq	spsr,#&248

l00009065:
	bhi	$010AF97D

l00009069:
	adcspl	r0,pc,#&600000

l0000906D:
	stmeq	r8!,{r3,r5-r6,ip,lr-pc}

l00009071:
	stmhs	r0!,{r0-r5,r7,r9,fp,pc}

l00009075:
	ereteq

l00009079:
	Invalid
	Invalid
	beq	$FE085429
	Invalid
	stmdaeq	r0!,{r0-r2}
	eoreq	r0,r0,r8
	nopgt

;; xCoRoutineRemoveFromEventList: 00009095
xCoRoutineRemoveFromEventList proc
	Invalid

l00009099:
	strbeq	r0,[sp],-#&968

l0000909D:
	Invalid

l000090A1:
	Invalid

l000090A5:
	ldrbtpl	r0,[r1],#&5F9

l000090A9:
	svc	#&463100
	blhs	$FFE51491

l000090B1:
	blle	$01AC1259

l000090B5:
	strblo	r9,[r2],-#&86A

l000090B9:
	strheq	r0,[r0,-pc]!

l000090BD:
	Invalid
	svceq	#&200007

;; GPIOGetIntNumber: 000090C5
GPIOGetIntNumber proc
	stmne	r2,{r0-r1,r3,r6,fp-ip,pc}^

l000090C9:
	ldrsblt	r0,[r8],#&80

l000090CD:
	msrne	cpsr,#&F1

l000090D1:
	ldrsbthi	r10,[r5],#&30

l000090D5:
	beq	$010AF229

l000090D9:
	Invalid

l000090DD:
	stmls	fp,{r0-r2,r6,r9,fp}

l000090E1:
	bicseq	r0,r0,#&420000

l000090E5:
	ldmdals	r3!,{r0,r2,r4-r7,fp,sp-pc}

l000090E9:
	ldrbne	r0,[r1],#&142

l000090ED:
	svcmi	#&477020

l000090F1:
	ldrshtvc	pc,[r0],-#&F0

l000090F5:
	eorvc	r1,r0,r7,asr #6

l000090F9:
	eorvc	r1,r0,r7,asr #&20

l000090FD:
	eorvc	r1,r0,r7,asr #4

l00009101:
	adcseq	r0,pc,r7,asr #&20

l00009105:
	subeq	r0,r0,r0,rrx

l00009109:
	suble	r0,r0,r0,ror r0

;; GPIODirModeSet: 0000910D
GPIODirModeSet proc
	eorsne	r0,r4,#&F8

l00009111:
	strne	r0,[pc],-#&1F0                                       ; 00009309

l00009115:
	blhi	$010CC019

l00009119:
	rscseq	ip,r8,r3,asr #&20

l0000911D:
	rscshs	sp,r8,r4,lsr r0

l00009121:
	Invalid

l00009125:
	movths	r1,#&39BF

l00009129:
	andgt	r0,r1,r10,ror #3

l0000912D:
	ldrshvc	r2,[r4],-r8

l00009131:
	asrseq	r0,r7,#&20

;; GPIODirModeGet: 00009135
GPIODirModeGet proc
	movseq	r1,#&23

l00009139:
	ldrshtle	r0,[r1],#&1A

l0000913D:
	stmgt	r4,{r3-r7}^

l00009141:
	ldrhths	sp,[r8],#&2

l00009145:
	stmeq	r2,{r2,r5,r10-fp}

l00009149:
	beq	$008C944D

l0000914D:
	adcseq	r1,pc,#&42000000

l00009151:
	eorne	r0,r0,r0,lsr #&20

l00009155:
	strhvc	r1,[r3],-#&8C

l00009159:
	adcsle	r0,pc,r7,asr #&20

;; GPIOIntTypeSet: 0000915D
GPIOIntTypeSet proc
	eorsne	r0,r4,#&F80000

l00009161:
	strne	r0,[pc],-#&1F0                                       ; 00009359

l00009165:
	blhi	$010CC069

l00009169:
	ldmdbeq	r8,{r0-r1,r6,lr-pc}^

l0000916D:
	ldrbteq	sp,[r8],#&34

l00009171:
	rscseq	r1,r0,#&40000003

l00009175:
	bleq	$FEFCE1B9

l00009179:
	subgt	r8,r3,r3,asr #&16

l0000917D:
	ldrshtle	r0,[r4],-#&48

l00009181:
	eorspl	r0,r4,#&F800

l00009185:
	ldmibne	pc!,{r0-r2,r10-fp,lr}

l00009189:
	mvneq	r2,r3,asr #6

l0000918D:
	Invalid

l00009191:
	Invalid

;; GPIOIntTypeGet: 00009195
GPIOIntTypeGet proc
	ldmdbeq	r8,{r0-r1,r5,ip,lr-pc}^

l00009199:
	mvnseq	r0,r4,lsr #6

l0000919D:
	ldrshtle	ip,[r2],#&91

l000091A1:
	beq	$00D0A589

l000091A5:
	Invalid

l000091A9:
	lslseq	r1,r4,#8

l000091AD:
	bleq	$0088923D

l000091B1:
	adcseq	r1,pc,#&42000000

l000091B5:
	stmdaeq	r3!,{r0-r1,r5}

l000091B9:
	ldrteq	r1,[pc],#&442                                       ; 00009603

l000091BD:
	nopne

l000091C1:
	subvc	r1,r3,r3,asr #&10

l000091C5:
	adcsne	r0,pc,r7,asr #&20

;; GPIOPadConfigSet: 000091C9
GPIOPadConfigSet proc
	ldrhteq	sp,[r8],#&4

l000091CD:
	mvnseq	r1,r5,asr #4

l000091D1:
	Invalid

l000091D5:
	subgt	r8,r3,r3,asr #&18

l000091D9:
	strdle	r0,r1,[r5],-#&8

l000091DD:
	subne	r0,r5,#&F8000000

l000091E1:
	strne	r0,[pc],-#&2F0                                       ; 000094D9

l000091E5:
	mcrrhi	p12,#&B,r0,r3,c15

l000091E9:
	ldrbteq	ip,[r8],#&43

l000091ED:
	ldmdbeq	r8,{r0,r2,r6,ip,lr-pc}^

l000091F1:
	ldrbteq	r1,[r0],#&245

l000091F5:
	Invalid

l000091F9:
	subgt	r8,r3,r3,asr #&18

l000091FD:
	subne	r0,r5,#&F80000

l00009201:
	strdle	r0,r1,[pc],-r0                                      ; 00009209

l00009205:
	strtne	r1,[r5],-#&8F8

l00009209:
	bhi	$010CBD0D

l0000920D:
	ldmdbne	r8,{r0-r1,r6,lr-pc}^

l00009211:
	Invalid

l00009215:
	Invalid

l00009219:
	bhi	$010CBD1D

l0000921D:
	Invalid

l00009221:
	rscsne	sp,r8,r5,lsr #&20

l00009225:
	Invalid

l00009229:
	bhi	$010CBD2D

l0000922D:
	rscsne	ip,r8,r3,asr #&20

l00009231:
	ldrbtne	sp,[r8],#&25

l00009235:
	Invalid

l00009239:
	bhi	$010CBD3D

l0000923D:
	ldrbtne	ip,[r8],#&43

l00009241:
	ldmdbeq	r0,{r0,r2,r5,r8-r9,ip}^

l00009245:
	Invalid

l00009249:
	ldrtne	r1,[ip],#&35

l0000924D:
	movths	r1,#&39BF

l00009251:
	andgt	r0,r1,r10,ror #3

l00009255:
	ldrshvc	r1,[r5],-r8

l00009259:
	Invalid

;; GPIOPadConfigGet: 0000925D
GPIOPadConfigGet proc
	strhtle	r0,[r4],-#&14

l00009261:
	ldrbeq	r0,[r5],-#&F8

l00009265:
	ldmibgt	r1,{r1,r3-r8}^

l00009269:
	ldrbteq	sp,[r8],#&B2

l0000926D:
	suble	r0,r2,r5,asr #&1A

l00009271:
	ldrbne	r0,[r5],-#&8F8

l00009275:
	strhteq	r0,[r7],-#&1F

l00009279:
	suble	r0,r2,r7,lsr #&18

l0000927D:
	strbne	r1,[r5],-#&8F8

l00009281:
	strhteq	r0,[r6],-#&2F

l00009285:
	strbne	r0,[r2],-#&D26

l00009289:
	strhteq	r0,[r5],-#&4F

l0000928D:
	strbne	r0,[r2],-#&C25

l00009291:
	strhteq	r0,[r4],-#&8F

l00009295:
	strblo	r3,[r3,-#&E24]

l00009299:
	strbne	r2,[r3],-#&C43

l0000929D:
	Invalid

l000092A1:
	rscsne	sp,r8,r5,lsr #&20

l000092A5:
	suble	r1,r2,r5,asr #2

l000092A9:
	stmne	r5!,{r3-r7,r10,ip}

l000092AD:
	strhtle	r0,[r5],-#&1F

l000092B1:
	stmdaeq	r5!,{r3-r7,r10-ip}

l000092B5:
	strhhs	r0,[r5,-pc]!

l000092B9:
	adcseq	r1,pc,#&42000000

l000092BD:
	bxjlo	r4

l000092C1:
	ldrteq	r1,[pc],#&442                                       ; 0000970B

l000092C5:
	bxjne	r0

l000092C9:
	ldmeq	pc!,{r1,r6,r10,ip}

l000092CD:
	strtmi	r0,[r2],-#&22

l000092D1:
	smlatteq	r1,r10,r5,r0

l000092D5:
	bne	$010CBBE9

l000092D9:
	adcsvc	pc,ip,r0,rrx

l000092DD:
	adcsle	r0,pc,r7,asr #&20

;; GPIOPinIntEnable: 000092E1
GPIOPinIntEnable proc
	ldmdbne	r4!,{r3-r7,ip}

l000092E5:
	rscsne	ip,r8,r3,asr #&20

l000092E9:
	suble	r7,r7,r4,lsl r0

;; GPIOPinIntDisable: 000092ED
GPIOPinIntDisable proc
	teqhs	r4,#&F8

l000092F1:
	andgt	r0,r1,r10,ror #3

l000092F5:
	ldrshvc	r1,[r4],-r8

l000092F9:
	asrsne	r0,r7,#&20

;; GPIOPinIntStatus: 000092FD
GPIOPinIntStatus proc
	ldrbtne	sp,[r8],#&B9

l00009301:
	suble	r7,r7,r4

l00009305:
	Invalid

l00009309:
	adcsgt	r0,pc,r7,asr #&20

;; GPIOPinIntClear: 0000930D
GPIOPinIntClear proc
	ldrshvc	r1,[r4],-r8

l00009311:
	ldrths	r0,[pc],#&47                                        ; 00009360

;; GPIOPortIntRegister: 00009315
GPIOPortIntRegister proc
	ldmls	r5!,{r0-r1,r3,r6,ip}

l00009319:
	svceq	#&D03C42

l0000931D:
	ldrsbtmi	fp,[r1],#&8

l00009321:
	bicsge	r2,r0,#&BC

l00009325:
	ldmls	r3,{r0,r2,r4-r7,pc}

l00009329:
	bicsne	r1,r1,r2,asr #&10

l0000932D:
	subeq	r2,r6,r4,lsr #&20

l00009331:
	ldrshths	lr,[r8],#&80

l00009335:
	rscne	fp,r8,r6,asr #&1A

l00009339:
	svcmi	#&F00040

l0000933D:
	stmls	fp,{r0,r3-r5,r7-r9,fp-ip}

l00009341:
	bicseq	r1,r0,#&4200000

l00009345:
	ldmdals	r3!,{r0,r2,r4-r7,fp,sp-pc}

l00009349:
	ldrbne	r0,[r1],#&842

l0000934D:
	subeq	r2,r6,r4,lsr #&20

l00009351:
	ldrshths	sp,[r8],#&80

l00009355:
	rscne	fp,r8,r6,asr #&1A

l00009359:
	svclo	#&F00040

l0000935D:
	svc	#&F04FB9
	subeq	r2,r6,r4,lsr r0

l00009365:
	ldrshths	ip,[r8],#&E0

l00009369:
	rscne	fp,r8,r6,asr #&1A

l0000936D:
	ldrblo	r0,[r0,#&40]!

l00009371:
	strhths	r1,[r4],-#&39

l00009375:
	ldrbgt	r0,[r0,#&46]!

l00009379:
	stclt	p0,c2,[r6,-#&3E0]

l0000937D:
	subeq	r1,r0,r8,ror #1

l00009381:
	ldrshtne	r2,[r9],#&C0

l00009385:
	subeq	r2,r6,r4,lsr #&20

l00009389:
	ldrshths	fp,[r8],#&C0

l0000938D:
	rscne	fp,r8,r6,asr #&1A

l00009391:
	mvnshs	r0,#&40

l00009395:
	strhths	r1,[r4],-#&29

l00009399:
	mvnslt	r0,#&46

l0000939D:
	stclt	p0,c2,[r6,-#&3E0]

l000093A1:
	subeq	r1,r0,r8,ror #1

l000093A5:
	ldrshteq	r1,[r9],#&A0

l000093A9:
	subeq	r0,r0,r0,rrx

l000093AD:
	strbhs	r0,[r0],-#&70

;; GPIOPortIntUnregister: 000093B1
GPIOPortIntUnregister proc
	ldmls	r5!,{r0-r1,r3,r6,ip}

l000093B5:
	svceq	#&D03C42

l000093B9:
	ldrsbtmi	fp,[r1],#&8

l000093BD:
	bicsge	r2,r0,#&BC

l000093C1:
	ldmls	r3,{r0,r2,r4-r7,pc}

l000093C5:
	bicsne	r1,r1,r2,asr #&10

l000093C9:
	subeq	r2,r6,r4,lsr #&20

l000093CD:
	ldrshths	r3,[r9],#&40

l000093D1:
	rscne	fp,r8,r6,asr #&1A

l000093D5:
	svcge	#&F00040

l000093D9:
	stmls	fp,{r3-r5,r7-r9,fp-ip}

l000093DD:
	bicseq	r1,r0,#&4200000

l000093E1:
	ldmdals	r3!,{r0,r2,r4-r7,fp,sp-pc}

l000093E5:
	ldrbne	r0,[r1],#&842

l000093E9:
	subeq	r2,r6,r4,lsr #&20

l000093ED:
	ldrshths	r2,[r9],#&40

l000093F1:
	rscne	fp,r8,r6,asr #&1A

l000093F5:
	svcls	#&F00040

l000093F9:
	svc	#&F04FB8
	subeq	r2,r6,r4,lsr r0

l00009401:
	ldrshths	r1,[r9],#&A0

l00009405:
	rscne	fp,r8,r6,asr #&1A

l00009409:
	ldrbls	r0,[r0,#&40]!

l0000940D:
	strhths	r1,[r4],-#&38

l00009411:
	mvnsne	r0,r6,asr #&20

l00009415:
	stclt	p0,c2,[r6,-#&3E4]

l00009419:
	subeq	r1,r0,r8,ror #1

l0000941D:
	ldrshtne	r8,[r8],#&C0

l00009421:
	subeq	r2,r6,r4,lsr #&20

l00009425:
	ldrshths	r0,[r9],#&80

l00009429:
	rscne	fp,r8,r6,asr #&1A

l0000942D:
	mvnshi	r0,#&40

l00009431:
	strhths	r1,[r4],-#&28

l00009435:
	svc	#&F00046
	stclt	p0,c2,[r6,-#&3E0]

l0000943D:
	subeq	r1,r0,r8,ror #1

l00009441:
	ldrshteq	r7,[r8],#&A0

l00009445:
	subeq	r0,r0,r0,rrx

l00009449:
	subpl	r0,r0,r0,ror r0

;; GPIOPinRead: 0000944D
GPIOPinRead proc
	strdvc	r2,r3,[r0],-r8

l00009451:
	adcsmi	r0,pc,r7,asr #&20

;; GPIOPinWrite: 00009455
GPIOPinWrite proc
	Invalid

l00009459:
	adcsvc	r0,pc,r7,asr #&20

;; GPIOPinTypeComparator: 0000945D
GPIOPinTypeComparator proc
	strhle	ip,[r3],-#&D4

l00009461:
	Invalid

l00009465:
	subgt	r2,r0,r3,lsr #&14

l00009469:
	Invalid

l0000946D:
	ldrsheq	r2,[r4,-#&8]!

l00009471:
	subgt	r3,r0,r2,lsr #&A

l00009475:
	ldrshvc	r2,[r4],-#&8

l00009479:
	ldrbge	pc,[r7,#&FBC]!

l0000947D:
	ldrhtvc	r0,[pc],#&E                                        ; 00009493

;; GPIOPinTypeI2C: 00009481
GPIOPinTypeI2C proc
	strhle	r0,[r6],-#&D4

l00009485:
	bleq	$0090986D

l00009489:
	mvneq	r2,r3,lsr #4

l0000948D:
	rscseq	ip,r8,r2

l00009491:
	rscshs	sp,r8,r4,lsr #&20

l00009495:
	strlo	r0,[r2,-#&164]!

l00009499:
	rscshs	ip,r8,r3,asr #&20

l0000949D:
	svc	#&BC7054
	ldrshtvc	r9,[lr],#&27

;; GPIOPinTypeQEI: 000094A5
GPIOPinTypeQEI proc
	strhle	r0,[r6],-#&D4

l000094A9:
	beq	$00909891

l000094AD:
	mvneq	r2,r3,lsr #4

l000094B1:
	rscseq	ip,r8,r2

l000094B5:
	rscshs	sp,r8,r4,lsr #&20

l000094B9:
	strlo	r0,[r2,-#&164]!

l000094BD:
	rscshs	ip,r8,r3,asr #&20

l000094C1:
	svc	#&BC7054
	ldrshtvc	r8,[lr],#&7

;; GPIOPinTypeUART: 000094C9
GPIOPinTypeUART proc
	strhle	r0,[r6],-#&D4

l000094CD:
	stmdaeq	r4!,{r3-r7}

l000094D1:
	mvneq	r2,r3,lsr #4

l000094D5:
	rscseq	ip,r8,r2

l000094D9:
	rscshs	sp,r8,r4,lsr #&20

l000094DD:
	strlo	r0,[r2,-#&164]!

l000094E1:
	rscshs	ip,r8,r3,asr #&20

l000094E5:
	svc	#&BC7054
	svc	#&BE6EF7

;; GPIOPinTypeTimer: 000094ED
GPIOPinTypeTimer proc
	svc	#&BFECF7

;; GPIOPinTypeSSI: 000094F1
GPIOPinTypeSSI proc
	svc	#&BFEAF7

;; GPIOPinTypePWM: 000094F5
GPIOPinTypePWM proc
	Invalid

;; IntDefaultHandler: 000094F9
IntDefaultHandler proc
	adcseq	r0,pc,r7,ror #1

;; IntMasterEnable: 000094FD
IntMasterEnable proc
	ldrshteq	lr,[sp],#&E0

;; IntMasterDisable: 00009501
IntMasterDisable proc
	beq	$FEF858C9

;; IntRegister: 00009505
IntRegister proc
	blne	$FED15639

l00009509:
	movtge	r0,#&CA68

l0000950D:
	bicshs	r0,r0,#&42000

l00009511:
	ldmdblt	r1,{r1-r2,r6,r10}^

l00009515:
	andsne	r1,fp,#&5000

l00009519:
	ldrbteq	r4,[r8],#&368

l0000951D:
	Invalid
	mcrrne	p3,#&D,r0,fp,c1
	rscshs	r4,r8,r0,ror #8
	adcsvc	r3,ip,r0,lsl r0
	ldmeq	pc!,{r0-r2,r6}
	rsceq	r0,r0,sp,ror #1
	eoreq	r0,r0,#0

;; IntUnregister: 00009539
IntUnregister proc
	movtmi	r0,#&A34B

l0000953D:
	Invalid

l00009541:
	adcseq	r0,pc,r7,asr #&20

l00009545:
	Invalid
	streq	r0,[r0],-#&94

;; IntPriorityGroupingSet: 0000954D
IntPriorityGroupingSet proc
	movtpl	r0,#&A54B

l00009551:
	teqmi	r0,#&F8

l00009555:
	msrmi	spsr,#&FF0

l00009559:
	teqne	r3,#&F4

l0000955D:
	strbge	r7,[r7],-#&60

l00009561:
	Invalid

l00009565:
	svcmi	#&E000ED

;; IntPriorityGroupingGet: 00009569
IntPriorityGroupingGet proc
	uqsub8eq	lr,r3,r4

l0000956D:
	stmdbeq	r0!,{r0,r3,r6}

l00009571:
	stmne	r10,{r3,r5-r6,r9-r10}^

l00009575:
	rscpl	r0,r0,#&10

l00009579:
	blhi	$00ECA961

l0000957D:
	bicseq	r0,r0,r2,asr #4

l00009581:
	Invalid
	mcrreq	p0,#&D,r7,r7,c1
	stmdbge	r0,{r0,r2-r3,r5-r7}^
	svc	#&A2

;; IntPrioritySet: 00009591
IntPrioritySet proc
	subne	r0,fp,r2,lsr #&12

l00009595:
	mvnseq	r2,#&B4

l00009599:
	mcrrne	p3,#0,r2,r4,c4

l0000959D:
	mvnseq	r0,#&6A

l000095A1:
	rsbgt	r2,r8,r0,lsl #6

l000095A5:
	movths	r8,#&200

l000095A9:
	smlatteq	r3,r10,r2,r0

l000095AD:
	ldmdbne	r0,{r1,r3-r7}^

l000095B1:
	rsbne	r2,r0,r3,asr #&20

l000095B5:
	strbge	r7,[r7],-#&BC

l000095B9:
	streq	r0,[r0],-r2

;; IntPriorityGet: 000095BD
IntPriorityGet proc
	mvnseq	r2,#&4B

l000095C1:
	blne	$0110E1D1

l000095C5:
	mvnseq	r0,#&6A

l000095C9:
	rsbgt	r1,r8,r0,lsl #&16

l000095CD:
	rscseq	r2,r10,r0,lsl #6

l000095D1:
	ldrshtvc	ip,[r2]

l000095D5:
	ldrtge	r0,[pc],#&47                                        ; 00009624

l000095D9:
	streq	r0,[r0],-#&A2

;; IntEnable: 000095DD
IntEnable proc
	ldrbeq	r1,[r0,#&328]

l000095E1:
	ldrbeq	r1,[r0],r8

l000095E5:
	svceq	#&D01B28

l000095E9:
	ldrbeq	r0,[r0,#&728]

l000095ED:
	Invalid

l000095F1:
	movteq	r0,#&AE38

l000095F5:
	ldrshtne	r0,[r0],#&A

l000095F9:
	mcrreq	p0,#6,r7,r7,c0

l000095FD:
	msrmi	spsr,#&34A

l00009601:
	movne	r0,#&32F0

l00009605:
	beq	$011E578D

l00009609:
	msrmi	spsr,#&34A

l0000960D:
	teqne	r3,#&F4

l00009611:
	strbeq	r7,[r7,-r0,rrx #1]

l00009615:
	msrmi	spsr,#&34A

l00009619:
	teqne	r3,#&F4

l0000961D:
	strbeq	r7,[r7],-#&60

l00009621:
	msrmi	spsr,#&34A

l00009625:
	msrne	cpsr,#&F4

l00009629:
	subeq	r7,r7,r0,rrx

l0000962D:
	rscne	r0,r0,r1,ror #1

l00009631:
	strbths	r0,[r0],#&E0

l00009635:
	strbteq	r0,[r0],#&ED

;; IntDisable: 00009639
IntDisable proc
	ldrbeq	r1,[r0,#&328]

l0000963D:
	ldrbeq	r1,[r0],r8

l00009641:
	svceq	#&D01B28

l00009645:
	ldrbeq	r0,[r0,#&728]

l00009649:
	Invalid

l0000964D:
	movteq	r0,#&AE38

l00009651:
	ldrshtne	r0,[r0],#&A

l00009655:
	mcrreq	p0,#6,r7,r7,c0

l00009659:
	msrhs	spsr,#&34A

l0000965D:
	movne	r0,#&32F0

l00009661:
	beq	$011E57E9

l00009665:
	msrhs	spsr,#&34A

l00009669:
	teqne	r3,#&F4

l0000966D:
	strbeq	r7,[r7,-r0,rrx #1]

l00009671:
	msrhs	spsr,#&34A

l00009675:
	teqne	r3,#&F4

l00009679:
	strbeq	r7,[r7],-#&60

l0000967D:
	msrhs	spsr,#&34A

l00009681:
	msrne	cpsr,#&F4

l00009685:
	subhi	r7,r7,r0,rrx

l00009689:
	rscne	r0,r0,r1,ror #1

l0000968D:
	strbths	r0,[r0],#&E0

l00009691:
	mvneq	r0,sp,ror #1

;; OSRAMDelay: 00009695
OSRAMDelay proc
	sbcsvc	pc,r1,r8,lsr sp

l00009699:
	ldmlo	pc!,{r0-r2,r6}

;; OSRAMWriteFirst: 0000969D
OSRAMWriteFirst proc
	Invalid

l000096A1:
	eorhs	r0,r2,ip,asr #&20

l000096A5:
	eoreq	r3,r1,r6,asr #&1A

l000096A9:
	ldmibhs	sp,{r4-r7,r9-fp,sp,pc}^

l000096AD:
	subeq	r2,r6,r6,asr #&20

l000096B1:
	ldrshths	ip,[sp],#&40

l000096B5:
	stmdblo	r8,{r1-r2,r6,r8,r10-sp,pc}^

l000096B9:
	eoreq	r0,r1,r0,asr #6

l000096BD:
	ldrshteq	fp,[sp]

l000096C1:
	stmgt	r0,{r9}^

;; OSRAMWriteArray: 000096C5
OSRAMWriteArray proc
	ldreq	pc,[r5,#&8B1]!

l000096C9:
	mcrreq	p12,#4,r0,pc,c6

l000096CD:
	andseq	r4,r8,ip,asr #&C

l000096D1:
	subeq	r2,r6,r1,lsr #&20

l000096D5:
	ldrshteq	r7,[sp],#&80

l000096D9:
	ldmdblo	r0,{r3,r5,r8,fp-pc}^

l000096DD:
	ldmible	r7,{r3,r5-r6,r8-pc}^

l000096E1:
	ldrsheq	r1,[r8,#&5F]!

l000096E5:
	subeq	r2,r6,fp,lsl r0

l000096E9:
	ldrsheq	r10,[sp,#&80]!

l000096ED:
	subeq	r2,r6,r1,lsr #&20

l000096F1:
	Invalid

l000096F5:
	Invalid
	mcrrvc	p0,#&B,r7,r7,c13
	eoreq	r0,r0,r8
	subne	r0,r0,r0,lsl #4

;; OSRAMWriteByte: 00009705
OSRAMWriteByte proc
	strheq	r0,[r6],-#&45

l00009709:
	subeq	r0,r8,r1,lsr #&12

l0000970D:
	ldrshteq	r5,[sp],#&C0

l00009711:
	ldrbeq	pc,[r0,r8,lsr #18]

l00009715:
	svc	#&68184B
	Invalid

l0000971D:
	subeq	r0,r8,r6,asr #8

l00009721:
	Invalid

l00009725:
	smlaltteq	r1,r0,r8,r0

l00009729:
	subeq	r0,r8,r1,lsr #2

l0000972D:
	ldrshteq	r7,[sp],#&80

l00009731:
	mcrrvc	p2,#0,r0,r0,c0

l00009735:
	eorvc	r0,r0,r8

;; OSRAMWriteFinal: 00009739
OSRAMWriteFinal proc
	Invalid

l0000973D:
	eorhs	r0,r1,ip,asr #&20

l00009741:
	mvnsmi	r0,r6,asr #&20

l00009745:
	Invalid
	beq	$0134CA91
	svc	#&68284C
	Invalid
	subeq	r2,r6,r6,asr #&20
	ldrbeq	r7,[sp,#&F0]!
	subeq	r2,r6,r1,lsr #&20
	ldrshteq	r5,[sp],#&E0
	subeq	r2,r6,r1,lsr #&20
	ldrshteq	r2,[sp],#&E0
	ldmdbhs	r0,{r3,r5,r8,fp-pc}^
	rscvc	fp,r8,r8,ror #&1A
	rsceq	r8,r7,r0,asr #&1A
	mcrrvc	p2,#0,r0,r0,c0
	eorne	r0,r0,r8

;; OSRAMClear: 00009781
OSRAMClear proc
	svc	#&2080B5
	Invalid

l00009789:
	svc	#&480E21
	svcpl	#&FF9AF7

l00009791:
	svc	#&200024
	Invalid

l00009799:
	sbcshs	pc,r1,ip,lsr r10

l0000979D:
	blgt	$FFE094BD

l000097A1:
	svc	#&2080FF
	Invalid

l000097A9:
	svc	#&480721
	svcpl	#&FF8AF7

l000097B1:
	svc	#&200024
	Invalid

l000097B9:
	sbcshs	pc,r1,ip,lsr r10

l000097BD:
	rscne	fp,r8,r6,asr #&1A

l000097C1:
	Invalid
	Invalid
	andvc	r0,r0,r2,lsr #1

;; OSRAMStringDraw: 000097CD
OSRAMStringDraw proc
	mcrreq	p6,#&B,r1,r6,c5

l000097D1:
	subhi	r0,r6,r6,asr #&A

l000097D5:
	mvnsvs	pc,r0,lsr #&1E

l000097D9:
	stceq	p0,c0,[lr],-#&3FC

l000097DD:
	strhlt	fp,[r0,-pc]!

l000097E1:
	svchi	#&F7FF20

l000097E5:
	ldrbths	r0,[r1],#&4FF

l000097E9:
	svc	#&208006
	Invalid

l000097F1:
	svc	#&FF0
	ldrshthi	r8,[pc],#&67                                      ; 00009864

l000097F9:
	mvnshi	pc,#&80

l000097FD:
	mvnseq	ip,#&FF00000

l00009801:
	rscsne	r4,r0,r0,lsl r0

l00009805:
	Invalid

l00009809:
	svc	#&2040FF
	blhs	$FFFE83F1

l00009811:
	bpl	$FECEA5F9

l00009815:
	beq	$0138F8CD

l00009819:
	strbne	r1,[r0,#&7D9]!

l0000981D:
	Invalid

l00009821:
	svc	#&B18334
	blhs	$FFFE5409

l00009829:
	bpl	$FECD0611

l0000982D:
	sbcshs	r0,r8,ip,lsr #&1A

l00009831:
	mvnhi	r0,#&EC000000

l00009835:
	ldreq	pc,[r8,-#&3]

l00009839:
	mvnsmi	pc,#&84

l0000983D:
	svcmi	#&2C5AFF

l00009841:
	b	$00009C09
00009845                D1 BD E8 70 40 75 E7 20 3B 03 EB      ...p@u. ;..
00009850 83 03 C4 F1 5F 04 F0 18 21 46 FF F7 33 FF 2B 78 ...._...!F..3.+x
00009860 06 4A 20 3B 03 EB 83 03 13 44 23 44 18 7C BD E8 .J ;.....D#D.|..
00009870 70 40 61 E7 70 BD 70 BD 04 A3 00 00 F4 A2 00 00 p@a.p.p.........
00009880 2D                                              -              

;; OSRAMImageDraw: 00009881
OSRAMImageDraw proc
	stmeq	r7,{r0,r3,r5-r7,ip-pc}

l00009885:
	ldreq	r8,[r3,#&69E]!

l00009889:
	stmls	r6,{r1-r2,r6,r10,ip}^

l0000988D:
	teqgt	r1,r6,asr #8

l00009891:
	Invalid

l00009895:
	rscsne	r4,r0,r4,asr #&10

l00009899:
	svceq	#&F00108

l0000989D:
	svc	#&F10307
	svc	#&20803A
	ldrshteq	pc,[lr],#&A7

l000098A9:
	lsrslt	r1,ip,#8

l000098AD:
	svc	#&20B020
	ldrshthi	r2,[pc],#&87                                      ; 00009940

l000098B5:
	ldrbhs	pc,[r7,#&F20]!

l000098B9:
	svc	#&4638FF
	ldrshthi	r2,[pc],#&27                                      ; 000098EC

l000098C1:
	svcne	#&F7FF20

l000098C5:
	svc	#&4640FF
	ldrshtmi	r1,[pc],#&C7                                      ; 00009998

l000098CD:
	ldmibne	r7,{r5,r8-pc}^

l000098D1:
	strdpl	r2,r3,[r6,-#&8F]

l000098D5:
	svc	#&444D46
	ldrsheq	pc,[lr,#&47]!

l000098DD:
	mvnseq	r8,r4,lsr r5

l000098E1:
	ldmibhs	r7,{r2-r3,r8-pc}^

l000098E5:
	blle	$010B34E9

l000098E9:
	Invalid
	Invalid

;; OSRAMInit: 000098F1
OSRAMInit proc
	strbeq	pc,[r1],-#&E9

l000098F5:
	rscsne	r4,r0,r6,asr #&1E

l000098F9:
	svclo	#&F00020

l000098FD:
	Invalid

l00009901:
	Invalid

l00009905:
	svc	#&481721
	ldrshhs	fp,[sp,#&A7]!

l0000990D:
	subeq	r1,r8,r6,asr #&C

l00009911:
	ldrsheq	pc,[fp]!

l00009915:
	strbne	r1,[fp,-#&522]

l00009919:
	strteq	lr,[r6],-#&34F

l0000991D:
	eoreq	r8,r0,r4,lsr #&20

l00009921:
	strbeq	r1,[r0,-r5,lsr #20]!

l00009925:
	Invalid

l00009929:
	Invalid
	Invalid
	movtls	r2,#&4301
	svc	#&61ECF8
	ldmdbge	lr,{r0-r2,r4-r7,ip-sp,pc}^
	andsmi	r10,lr,ip,lsl r1
	svc	#&340144
	ldrbhs	fp,[lr,#&EF7]!
	svc	#&463044
	ldrshtvc	pc,[lr],#&47
	strbeq	r0,[fp,#&72D]!
	Invalid
	svc	#&41F0E8
	adcseq	r1,pc,#&F7
	eoreq	r0,r0,r0
	subeq	r0,r0,r0,asr r0
	mcrrvc	p2,#0,r0,r0,c0
	Invalid
	stchs	p0,c0,[r0,-#&288]

;; OSRAMDisplayOn: 00009975
OSRAMDisplayOn proc
	subne	pc,r1,r9,ror #1

l00009979:
	strteq	lr,[r6],-#&34F

l0000997D:
	eoreq	r8,r0,r4,lsr #&20

l00009981:
	Invalid
	mvnls	r0,#&7800000
	movtls	lr,#&1CF8
	movhs	lr,#&1DF8
	Invalid
	mvnshi	pc,r1,ror #&1E
	ldrshge	r10,[ip,-lr]
	Invalid
	svchi	#&F7FF34
	strdlo	r2,r3,[r4],-#&5E
	ldrbgt	pc,[r7,#&F46]!
	Invalid
	stmda	r3,{r0-r1,r3,r5-r8,r10}
	Invalid
	Invalid
	stmdaeq	r0,{r1,r5,r7}

;; OSRAMDisplayOff: 000099C1
OSRAMDisplayOff proc
	svc	#&2080B5
	vmrsge	r6,#&E

l000099C9:
	blls	$FFE09651

l000099CD:
	svc	#&2080FE
	Invalid

l000099D5:
	ldrbls	pc,[r7,#&F20]!

l000099D9:
	svc	#&2080FE
	Invalid

l000099E1:
	bhi	$0100BD89

l000099E5:
	Invalid

;; SSIConfig: 000099E9
SSIConfig proc
	strbne	pc,[r1,-r9,ror #1]

l000099ED:
	stmhi	r6,{r1-r2,r6,r9-r10}

l000099F1:
	strbeq	r1,[r6],-r6

l000099F5:
	blx	$FFC09C73
	stmdane	pc!,{r0,r3-r7,r9}

l000099FD:
	stmdane	pc!,{r4,r6-r7}

l00009A01:
	strhtlt	r0,[r7],-#&4F

l00009A05:
	ldrshteq	pc,[r3],#&4B

l00009A09:
	rsbeq	r7,r0,#&900000

l00009A0D:
	Invalid
	svc	#&3A01F2
	ldmdbeq	r8,{r1,r3,r5,r8,fp-pc}^
	Invalid
	stmdbhi	r10,{r0,r2-r5,r8-r9,lr}^
	strbmi	r0,[r3,-#&D11]
	strtlo	r0,[r2],-#&2EA
	stclt	p2,c3,[r0,-#&184]!
	stceq	p0,c15,[r1],#&3A0
	mvnmi	lr,#&9C0000

;; SSIEnable: 00009A35
SSIEnable proc
	rscseq	r4,r0,#&A0000001

l00009A39:
	rsbvc	r4,r0,r3,lsl #6

l00009A3D:
	movsmi	r0,#&47

;; SSIDisable: 00009A41
SSIDisable proc
	rscseq	r2,r0,#&A0000001

l00009A45:
	rsbvc	r4,r0,r3,lsl #6

l00009A49:
	ldmeq	pc!,{r0-r2,r6}

;; SSIIntRegister: 00009A4D
SSIIntRegister proc
	svc	#&2017B5
	Invalid

l00009A55:
	strbne	r0,[r0,-r8,ror #17]

l00009A59:
	svclt	#&F7FF20

l00009A5D:
	ldmeq	pc!,{r0,r2-r5,r7}

;; SSIIntUnregister: 00009A61
SSIIntUnregister proc
	svc	#&2017B5
	Invalid

l00009A69:
	strbne	r0,[r0,-r8,ror #17]

l00009A6D:
	mvnsvs	pc,#&80

l00009A71:
	movsmi	r0,#&BD

;; SSIIntEnable: 00009A75
SSIIntEnable proc
	Invalid

l00009A79:
	movtmi	r7,#&7061

;; SSIIntDisable: 00009A7D
SSIIntDisable proc
	mvneq	r2,r9,ror #6

l00009A81:
	rsbvc	r4,r1,r1,lsl #2

l00009A85:
	ldmibeq	pc!,{r0-r2,r6}

;; SSIIntStatus: 00009A89
SSIIntStatus proc
	strhtvc	r8,[r9],-#&9

l00009A8D:
	rsbvc	ip,r9,r7,asr #&20

l00009A91:
	asrseq	r0,r7,#&20

;; SSIIntClear: 00009A95
SSIIntClear proc
	subeq	r7,r7,r2,rrx

;; SSIDataPut: 00009A99
SSIDataPut proc
	movne	r0,#&2CF1

l00009A9D:
	Invalid
	ldrdvc	r8,r9,[r0],-#&15
	movsgt	r0,#&47

;; SSIDataNonBlockingPut: 00009AA9
SSIDataNonBlockingPut proc
	rscseq	r1,r0,#&A0000001

l00009AAD:
	lslshi	r1,r3,#&14

l00009AB1:
	stmdane	r0!,{r5-r6,r8}

l00009AB5:
	subeq	r7,r7,r6,asr #&20

;; SSIDataGet: 00009AB9
SSIDataGet proc
	movne	r0,#&2CF1

l00009ABD:
	Invalid
	bleq	$01A2AA1D
	movtgt	r7,#&7060

;; SSIDataNonBlockingGet: 00009AC9
SSIDataNonBlockingGet proc
	ldrbteq	r1,[r0],#&368

l00009ACD:
	movshi	r1,#&C0

l00009AD1:
	bleq	$0080A079

l00009AD5:
	subvc	r1,r6,r0,ror #&10

l00009AD9:
	movseq	r0,#&47

;; SysCtlSRAMSizeGet: 00009ADD
SysCtlSRAMSizeGet proc
	blne	$0120AC11

l00009AE1:
	mvnne	r0,#&68

l00009AE5:
	rscshi	r0,r5,r0,lsr #&20

l00009AE9:
	stmeq	r7,{r4-r6,ip-lr}

l00009AED:
	subeq	r0,r0,r0,ror #&1F

l00009AF1:
	moveq	pc,#&FFF

;; SysCtlFlashSizeGet: 00009AF5
SysCtlFlashSizeGet proc
	blne	$0120AC29

l00009AF9:
	mvngt	r0,#&68

l00009AFD:
	rscseq	r0,r5,r0,lsr #&20

l00009B01:
	stmeq	r7,{r5-r6,ip-lr}

l00009B05:
	subeq	r0,r0,r0,ror #&1F

l00009B09:
	moveq	pc,#&7FF8

;; SysCtlPinPresent: 00009B0D
SysCtlPinPresent proc
	msreq	spsr,#&B4B

l00009B11:
	asrseq	r1,r2,#8

l00009B15:
	eorvc	r0,r0,r0,lsr #&20

l00009B19:
	ldmne	pc!,{r0-r2,r6}

l00009B1D:
	strbeq	r0,[r0,-#&FE0]

;; SysCtlPeripheralPresent: 00009B21
SysCtlPeripheralPresent proc
	movpl	r0,#&F24B

l00009B25:
	ldrshths	r2,[r0],-#&28

l00009B29:
	blne	$01025EF1

l00009B2D:
	strbne	r1,[r2],-#&868

l00009B31:
	strhteq	r0,[r0],-#&1F

l00009B35:
	strbpl	r7,[r7],-#&20

l00009B39:
	andeq	r0,r0,r5,lsr #1

;; SysCtlPeripheralReset: 00009B3D
SysCtlPeripheralReset proc
	subeq	r0,fp,#&210

l00009B41:
	movseq	r1,#&F

l00009B45:
	bne	$000EA6F9

l00009B49:
	rscsvc	r2,r0,r9,rrx

l00009B4D:
	msrhi	spsr,#&443

l00009B51:
	movtne	r2,#&33B0

l00009B55:
	orrseq	r0,r1,r0,ror #2

l00009B59:
	streq	r0,[fp,-#&F9B]!

l00009B5D:
	ldrsbeq	r0,[fp,r8]

l00009B61:
	orrseq	r0,r3,r3,lsr r1

l00009B65:
	Invalid
	msrhs	spsr,#&3D9
	andne	r0,r0,r10,ror #1
	adcsne	r0,r0,r0,ror #6
	strbpl	r7,[r7],-#&BC
	streq	r0,[r0,-#&A5]

;; SysCtlPeripheralEnable: 00009B7D
SysCtlPeripheralEnable proc
	moveq	r0,#&F24B

l00009B81:
	blle	$000EA735

l00009B85:
	rscsvc	r2,r0,r9,rrx

l00009B89:
	rsbne	r1,r8,r0,asr #&14

l00009B8D:
	rsbvc	r1,r0,r3,asr #&10

l00009B91:
	ldrtpl	r0,[pc],#&47                                        ; 00009BE0

l00009B95:
	streq	r0,[r0,-#&A5]

;; SysCtlPeripheralDisable: 00009B99
SysCtlPeripheralDisable proc
	moveq	r0,#&F24B

l00009B9D:
	ble	$000EA751

l00009BA1:
	rscsvc	r2,r0,r9,rrx

l00009BA5:
	msrhs	spsr,#&340

l00009BA9:
	andne	r0,r0,r10,ror #1

l00009BAD:
	strbpl	r7,[r7],-#&60

l00009BB1:
	streq	r0,[r0,-#&A5]

;; SysCtlPeripheralSleepEnable: 00009BB5
SysCtlPeripheralSleepEnable proc
	moveq	r0,#&F24B

l00009BB9:
	blls	$000EA76D

l00009BBD:
	rscsvc	r2,r0,r10,rrx

l00009BC1:
	rsbne	r1,r8,r0,asr #&14

l00009BC5:
	rsbvc	r1,r0,r3,asr #&10

l00009BC9:
	ldrtpl	r0,[pc],#&47                                        ; 00009C18

l00009BCD:
	streq	r0,[r0,-#&A5]

;; SysCtlPeripheralSleepDisable: 00009BD1
SysCtlPeripheralSleepDisable proc
	moveq	r0,#&F24B

l00009BD5:
	bls	$000EA789

l00009BD9:
	rscsvc	r2,r0,r10,rrx

l00009BDD:
	msrhs	spsr,#&340

l00009BE1:
	andne	r0,r0,r10,ror #1

l00009BE5:
	strbpl	r7,[r7],-#&60

l00009BE9:
	streq	r0,[r0,-#&A5]

;; SysCtlPeripheralDeepSleepEnable: 00009BED
SysCtlPeripheralDeepSleepEnable proc
	moveq	r0,#&F24B

l00009BF1:
	blpl	$000EA7A5

l00009BF5:
	rscsvc	r2,r0,fp,rrx

l00009BF9:
	rsbne	r1,r8,r0,asr #&14

l00009BFD:
	rsbvc	r1,r0,r3,asr #&10

l00009C01:
	ldrtpl	r0,[pc],#&47                                        ; 00009C50

l00009C05:
	streq	r0,[r0,-#&A5]

;; SysCtlPeripheralDeepSleepDisable: 00009C09
SysCtlPeripheralDeepSleepDisable proc
	moveq	r0,#&F24B

l00009C0D:
	bpl	$000EA7C1

l00009C11:
	rscsvc	r2,r0,fp,rrx

l00009C15:
	msrhs	spsr,#&340

l00009C19:
	andne	r0,r0,r10,ror #1

l00009C1D:
	strbpl	r7,[r7],-#&60

l00009C21:
	streq	r0,[r0,-#&A5]

;; SysCtlPeripheralClockGating: 00009C25
SysCtlPeripheralClockGating proc
	stmne	r8!,{r1,r3,r6,r8-r9,ip}

l00009C29:
	ldrhteq	r2,[r0],#&39

l00009C2D:
	rsbvc	r1,r0,r3,ror #6

l00009C31:
	rscseq	r4,r0,r7,asr #6

l00009C35:
	rsbvc	r1,r0,r3,ror #6

l00009C39:
	adcsvs	r0,pc,r7,asr #&20

l00009C3D:
	stmeq	r0,{r5-fp}

;; SysCtlIntRegister: 00009C41
SysCtlIntRegister proc
	mcrrhs	p1,#&B,r0,r6,c5

l00009C45:
	Invalid

l00009C49:
	stmdbeq	r8,{r2-r8,r10-sp,pc}^

l00009C4D:
	svc	#&202C40
	ldmeq	ip!,{r0-r2,r4-r7,r10,lr-pc}

;; SysCtlIntUnregister: 00009C55
SysCtlIntUnregister proc
	svc	#&202CB5
	Invalid

l00009C5D:
	Invalid

l00009C61:
	ldmibvs	r7,{r5,r8-pc}^

l00009C65:
	adcseq	r0,pc,#&BC

;; SysCtlIntEnable: 00009C69
SysCtlIntEnable proc
	stmne	r8!,{r1,r3,r6,r8-r9,ip}

l00009C6D:
	rsbvc	r1,r0,r3,asr #&20

l00009C71:
	ldrtpl	r0,[pc],#&47                                        ; 00009CC0

l00009C75:
	subeq	r0,r0,#&380

;; SysCtlIntDisable: 00009C79
SysCtlIntDisable proc
	msrhs	spsr,#&34A

l00009C7D:
	andne	r0,r0,r10,ror #1

l00009C81:
	strbpl	r7,[r7],-#&60

l00009C85:
	smlaltteq	r0,r0,r0,pc

;; SysCtlIntClear: 00009C89
SysCtlIntClear proc
	rsbvc	r1,r0,fp,asr #&10

l00009C8D:
	ldmpl	pc!,{r0-r2,r6}

l00009C91:
	subne	r0,r0,r0,ror #&1F

;; SysCtlIntStatus: 00009C95
SysCtlIntStatus proc
	stmne	fp,{r0,r3-r5,r7-r9}

l00009C99:
	subeq	r7,r7,#&68

l00009C9D:
	rsbvc	r1,r8,fp,asr #&10

l00009CA1:
	adcspl	r0,pc,r7,asr #&20

l00009CA5:
	stmpl	r0,{r5-fp}

l00009CA9:
	smlaltteq	r0,r0,r0,pc

;; SysCtlLDOSet: 00009CAD
SysCtlLDOSet proc
	rsbvc	r1,r0,fp,asr #&10

l00009CB1:
	ldrtlo	r0,[pc],#&47                                        ; 00009D00

l00009CB5:
	smlaltteq	r0,r0,r0,pc

;; SysCtlLDOGet: 00009CB9
SysCtlLDOGet proc
	rsbvc	r1,r8,fp,asr #&10

l00009CBD:
	ldrtlo	r0,[pc],#&47                                        ; 00009D0C

l00009CC1:
	smlaltteq	r0,r0,r0,pc

;; SysCtlLDOConfigSet: 00009CC5
SysCtlLDOConfigSet proc
	rsbvc	r1,r0,fp,asr #&10

l00009CC9:
	adcsvs	r0,pc,r7,asr #&20

l00009CCD:
	smlaltteq	r0,r0,r1,pc

;; SysCtlReset: 00009CD1
SysCtlReset proc
	bne	$0128A605

l00009CD5:
	Invalid

l00009CD9:
	strbteq	r0,[r0],#&ED

l00009CDD:
	andeq	pc,r5,r0,lsl #&14

;; SysCtlSleep: 00009CE1
SysCtlSleep proc
	ldrshtne	r0,[r10],#&40

;; SysCtlDeepSleep: 00009CE5
SysCtlDeepSleep proc
	movths	r0,#&C6B5

l00009CE9:
	ldrbteq	r4,[r0],#&368

l00009CED:
	rsbeq	r2,r0,r3,lsl #6

l00009CF1:
	mvnshs	pc,#&F000

l00009CF5:
	ldrbteq	r2,[r0],#&368

l00009CF9:
	rsbne	r2,r0,r3,lsl #6

l00009CFD:
	ldrhtne	r0,[pc],#&D                                        ; 00009D12

l00009D01:
	mvneq	r0,sp,ror #1

;; SysCtlResetCauseGet: 00009D05
SysCtlResetCauseGet proc
	rsbvc	r1,r8,fp,asr #&10

l00009D09:
	Invalid

l00009D0D:
	subeq	r0,r0,#&380

;; SysCtlResetCauseClear: 00009D11
SysCtlResetCauseClear proc
	msrhs	spsr,#&34A

l00009D15:
	andne	r0,r0,r10,ror #1

l00009D19:
	mcrrpl	p0,#6,r7,r7,c0

l00009D1D:
	subeq	r0,r0,#&380

;; SysCtlBrownOutConfigSet: 00009D21
SysCtlBrownOutConfigSet proc
	mvnhi	r4,fp,asr #&20

l00009D25:
	rsbvc	r1,r0,r1,lsl #&12

l00009D29:
	adcslo	r0,pc,r7,asr #&20

l00009D2D:
	movtmi	r0,#&FE0

;; SysCtlClockSet: 00009D31
SysCtlClockSet proc
	Invalid
	strhteq	r4,[r7],-#&4
	stmhs	ip,{r1-r2,r5,r8,fp,sp}^
	rsbvs	r2,r8,r9,asr #6
	stmdbne	r5,{r4-r9}
	rscseq	r4,r4,r0,asr #2
	subeq	r2,r0,#&184000
	rscshi	r2,r4,r0,asr #6
	subhi	r2,sp,#&C00000
	ldrhteq	r4,[r4],#&30
	movths	r0,#&3A63
	rsbhs	r2,r0,#&180
	orrseq	r0,r6,r0,ror #2
	streq	r0,[fp,-#&F9B]!
	ldrsbeq	r0,[fp,r8]
	orrseq	r0,r3,r3,lsr r1
	Invalid
	mvnseq	r0,#&D9
	subhs	r1,ip,#&C000
	bne	$0008AD45
	Invalid
	Invalid
	strbteq	r2,[r0],-#&261
	mvneq	r4,#&40000001
	svcmi	#&D41401
	strdeq	r0,r1,[r3,-#&4]
	blvs	$FE6CA3ED
	movtne	r1,#&A3B1
	moveq	r5,#&6868
	mvnne	r0,#&D50000
	streq	r5,[r6,-#&B68]
	ldrsbeq	r0,[fp,r4]
	orrseq	r0,r3,fp,lsr r1
	Invalid
	ldrsbteq	r2,[r4],#&11
	streq	r0,[r3,-r1,rrx #1]!
	Invalid
	svceq	#&9B0193
	bicseq	r0,r8,fp,lsr #&A
	swpeq	r0,fp,[r3]!
	svceq	#&9B0193
	sbcseq	pc,r9,#&AC000
	ldrhtvc	pc,[ip]
	adcsvs	r0,pc,r7,asr #&20
	svceq	#&400FE0
	ldmdbpl	pc,{r2-r3,r6-sp,pc}^
	subpl	r0,r0,r0,ror #&1F
	stmne	r0,{r5-fp}

;; SysCtlClockGet: 00009DF1
SysCtlClockGet proc
	msreq	spsr,#&B4B

l00009DF5:
	Invalid

l00009DF9:
	sbcshs	r2,r0,r10,lsr #&10

l00009DFD:
	beq	$FF412EAD

l00009E01:
	strhtvc	r0,[r0],-#&1

l00009E05:
	movtgt	r1,#&A447

l00009E09:
	andseq	r8,r1,#&CC000003

l00009E0D:
	andne	r8,r2,fp,ror #3

l00009E11:
	Invalid

l00009E15:
	subne	r1,r10,#&35

l00009E19:
	ldmdbmi	r3,{r3,r5-r6,r9,lr-pc}^

l00009E1D:
	eorseq	r0,r1,r1,lsl r2

l00009E21:
	rscseq	r0,r0,#&C000003E

l00009E25:
	andeq	r1,r1,#&3C0

l00009E29:
	Invalid
	stmdami	r4,{r4-r8,ip,lr}
	strhne	r4,[r8,-pc]
	adcshi	r4,pc,r4,lsl #&10
	and	r5,r2,#&8000
	mvnsgt	ip,#&54000003
	eorslt	r0,r3,r3,asr r1
	ldrshtvc	pc,[r0],#&3B
	crc32w	r0,r8,r7
	svcle	#&4805E7
	adcsvs	r0,pc,r7,ror #1
	strbpl	r0,[r0],-#&FE0
	strvs	r0,[r0],-#&A5
	subvc	r0,r0,r0,ror #&1F
	andgt	r3,r0,r8,lsr r9
	moveq	lr,#&4E1

;; SysCtlPWMClockSet: 00009E69
SysCtlPWMClockSet proc
	msrhs	spsr,#&34A

l00009E6D:
	ldmdane	r3,{r2,r4-r7,ip-pc}

l00009E71:
	rsbvc	r1,r0,r3,asr #&20

l00009E75:
	adcsvs	r0,pc,r7,asr #&20

l00009E79:
	subeq	r0,r0,#&380

;; SysCtlPWMClockGet: 00009E7D
SysCtlPWMClockGet proc
	rsbeq	r1,r8,fp,asr #&10

l00009E81:
	ldrshvc	pc,[r0],-r4

l00009E85:
	adcsvs	r0,pc,r7,asr #&20

l00009E89:
	subne	r0,r0,r0,ror #&1F

;; SysCtlADCSpeedSet: 00009E8D
SysCtlADCSpeedSet proc
	beq	$0130C965

l00009E91:
	beq	$01A12BBD

l00009E95:
	rscsvc	r2,r4,r10,asr #6

l00009E99:
	movths	r0,#&3363

l00009E9D:
	rsbne	r0,r8,r0,ror #&16

l00009EA1:
	ldrhtvc	r2,[r4],#&3C

l00009EA5:
	bleq	$010CAC39

l00009EA9:
	msrhs	spsr,#&360

l00009EAD:
	stmne	r3!,{r2,r4-r7,ip-lr}

l00009EB1:
	rsbvc	r1,r0,r3,asr #&20

l00009EB5:
	adcseq	r0,pc,r7,asr #&20

l00009EB9:
	subne	r0,r0,r1,ror #&1F

l00009EBD:
	subhs	r0,r0,r1,ror #&1F

l00009EC1:
	subeq	r0,r0,#&384

;; SysCtlADCSpeedGet: 00009EC5
SysCtlADCSpeedGet proc
	rsbeq	r1,r8,fp,asr #&10

l00009EC9:
	Invalid

l00009ECD:
	adcseq	r0,pc,r7,asr #&20

l00009ED1:
	strbeq	r0,[r0,-#&FE1]

;; SysCtlIOSCVerificationSet: 00009ED5
SysCtlIOSCVerificationSet proc
	stmne	r8!,{r1,r3,r6,r8-r9,ip}

l00009ED9:
	ldmdbeq	r0,{r0,r3-r5,r7-r9,sp}^

l00009EDD:
	rsbvc	r1,r0,r3,lsl #6

l00009EE1:
	ldmdbeq	r0,{r0-r2,r6,r8-r9,lr}^

l00009EE5:
	rsbvc	r1,r0,r3,lsl #6

l00009EE9:
	adcsvs	r0,pc,r7,asr #&20

l00009EED:
	strbeq	r0,[r0,-#&FE0]

;; SysCtlMOSCVerificationSet: 00009EF1
SysCtlMOSCVerificationSet proc
	stmne	r8!,{r1,r3,r6,r8-r9,ip}

l00009EF5:
	ldrbteq	r2,[r0],#&3B9

l00009EF9:
	rsbvc	r1,r0,r3,lsl #6

l00009EFD:
	ldrbteq	r4,[r0],#&347

l00009F01:
	rsbvc	r1,r0,r3,lsl #6

l00009F05:
	adcsvs	r0,pc,r7,asr #&20

l00009F09:
	strbeq	r0,[r0,-#&FE0]

;; SysCtlPLLVerificationSet: 00009F0D
SysCtlPLLVerificationSet proc
	stmne	r8!,{r1,r3,r6,r8-r9,ip}

l00009F11:
	ldrhthi	r2,[r4],#&39

l00009F15:
	rsbvc	r1,r0,r3,ror #6

l00009F19:
	rscshi	r4,r4,r7,asr #6

l00009F1D:
	rsbvc	r1,r0,r3,ror #6

l00009F21:
	adcsvs	r0,pc,r7,asr #&20

l00009F25:
	smlaltteq	r0,r0,r0,pc

;; SysCtlClkVerificationClear: 00009F29
SysCtlClkVerificationClear proc
	bxjeq	r1

l00009F2D:
	bne	$01810461

l00009F31:
	subpl	r7,r7,r0,rrx

l00009F35:
	movtgt	r0,#&FE1

;; UARTParityModeSet: 00009F39
UARTParityModeSet proc
	strbthi	r2,[r0],r10

l00009F3D:
	mrsgt	r1,spsr

l00009F41:
	subgt	r7,r7,r2,rrx

;; UARTParityModeGet: 00009F45
UARTParityModeGet proc
	strbthi	r0,[r0],r10

l00009F49:
	Invalid

;; UARTConfigSet: 00009F4D
UARTConfigSet proc
	Invalid

l00009F51:
	stmne	r6,{r1-r2,r6,r8,r10}

l00009F55:
	strbtne	r0,[r8],-#&430

l00009F59:
	blx	$0010C323
	msrhs	spsr,#&BD1

l00009F61:
	bl	$000CE329
	rsbhs	r2,fp,#&62000

l00009F69:
	rsbshs	r4,r2,#&F4

l00009F6D:
	bhs	$0008A735

l00009F71:
	Invalid

l00009F75:
	Invalid

l00009F79:
	mvnseq	pc,#&EC000003

l00009F7D:
	blle	$000CEB71

l00009F81:
	Invalid
	blpl	$00CCA759
	blge	$018A47B1
	stcge	p14,c14,[r2],-#&188
	msrmi	spsr,#&B61
	bl	$000CE35D
	msrmi	spsr,#&B62
	cmnmi	r3,#&F4
	blhs	$000CA769
	Invalid

;; UARTConfigGet: 00009FA9
UARTConfigGet proc
	suble	pc,r1,r9,ror #1

l00009FAD:
	streq	r2,[r0],#&4F8

l00009FB1:
	strbne	r0,[r6],-r6

l00009FB5:
	svc	#&6A8546
	ldrbeq	r1,[pc,#&AF7]!                                      ; 0000AAB8

l00009FBD:
	andshi	r8,r5,fp,ror #&11

l00009FC1:
	Invalid
	msr	spsr,#&8F0
	Invalid
	stclt	p3,c3,[r0,-#&C]!
	orrgt	pc,r1,#&E8

;; UARTEnable: 00009FD5
UARTEnable proc
	rscsne	r4,r0,r10,ror #6

l00009FD9:
	msreq	spsr,#&303

l00009FDD:
	rscsmi	r4,r4,fp,ror #6

l00009FE1:
	mvnseq	r0,r3,ror r3

l00009FE5:
	rsbvc	r0,r3,r3,lsl #6

l00009FE9:
	adcseq	r0,pc,r7,asr #&20

;; UARTDisable: 00009FED
UARTDisable proc
	movne	r1,#&28F1

l00009FF1:
	Invalid
	msrhs	spsr,#&3D4
	movgt	r1,#&30F0
	msrhs	spsr,#&362
	cmnhs	r3,#&F4
	moveq	r0,#&31F0
	subhi	r7,r7,r3,rrx

;; UARTCharsAvail: 0000A00D
UARTCharsAvail proc
	rscsne	r8,r0,r9,rrx

l0000A011:
	rscseq	ip,r3,r0

l0000A015:
	subhi	r7,r7,r0,lsl r0

;; UARTSpaceAvail: 0000A019
UARTSpaceAvail proc
	rscshs	r8,r0,r9,rrx

l0000A01D:
	rscsmi	ip,r3,r0

l0000A021:
	movthi	r7,#&7010

;; UARTCharNonBlockingGet: 0000A025
UARTCharNonBlockingGet proc
	strpl	sp,[r6],-#&B69

l0000A029:
	svcmi	#&6800BF

l0000A02D:
	ldrshtvc	pc,[r0],-#&F0

l0000A031:
	adcseq	r0,pc,r7,asr #&20

;; UARTCharGet: 0000A035
UARTCharGet proc
	movne	r1,#&28F1

l0000A039:
	Invalid
	ldrdvc	r0,r1,[r8],-#&4
	movshi	r0,#&47

;; UARTCharNonBlockingPut: 0000A045
UARTCharNonBlockingPut proc
	bpl	$001B0DF1

l0000A049:
	strheq	r0,[r0,-#&1F]!

l0000A04D:
	eorvc	r0,r0,r0,lsr #&20

l0000A051:
	adcseq	r0,pc,r7,asr #&20

;; UARTCharPut: 0000A055
UARTCharPut proc
	movne	r1,#&28F1

l0000A059:
	Invalid
	ldrdvc	r0,r1,[r0],-#&14
	movsgt	r0,#&47

;; UARTBreakCtl: 0000A065
UARTBreakCtl proc
	movshs	r1,#&1A8000

l0000A069:
	movgt	r0,#&31F0

l0000A06D:
	movtmi	r7,#&7062

l0000A071:
	movgt	r0,#&31F0

l0000A075:
	subne	r7,r7,r2,rrx

;; UARTIntRegister: 0000A079
UARTIntRegister proc
	strhge	r0,[ip],-#&65

l0000A07D:
	ldrne	r0,[pc,#&C42]!                                       ; 0000ACC7

l0000A081:
	eorhs	r1,r4,r4,lsr #&C

l0000A085:
	Invalid

l0000A089:
	stclt	p0,c2,[r6,-#&3E8]

l0000A08D:
	svc	#&4010E8
	ldrshteq	r10,[r10],#&47

l0000A095:
	subne	r0,r0,r0,asr #1

;; UARTIntUnregister: 0000A099
UARTIntUnregister proc
	strhge	r0,[ip],-#&65

l0000A09D:
	ldrne	r0,[pc,#&C42]!                                       ; 0000ACE7

l0000A0A1:
	eorhs	r1,r4,r4,lsr #&C

l0000A0A5:
	ldrbgt	pc,[r7,r6,asr #30]!

l0000A0A9:
	stclt	p0,c2,[r6,-#&3E8]

l0000A0AD:
	svc	#&4010E8
	ldrshteq	r4,[r10],#&27

l0000A0B5:
	movthi	r0,#&C0

;; UARTIntEnable: 0000A0B9
UARTIntEnable proc
	Invalid

l0000A0BD:
	movthi	r7,#&7063

;; UARTIntDisable: 0000A0C1
UARTIntDisable proc
	mvneq	r2,fp,ror #6

l0000A0C5:
	rsbvc	r8,r3,r1,lsl #2

l0000A0C9:
	ldmibeq	pc!,{r0-r2,r6}

;; UARTIntStatus: 0000A0CD
UARTIntStatus proc
	strhtvc	ip,[fp],-#&9

l0000A0D1:
	rsbvc	r0,ip,r7,asr #&20

l0000A0D5:
	asrsmi	r0,r7,#&20

;; UARTIntClear: 0000A0D9
UARTIntClear proc
	subvs	r7,r7,#&64

;; CPUcpsie: 0000A0DD
CPUcpsie proc
	strhvc	r7,[r7],-#&6

l0000A0E1:
	adcsvc	r0,pc,#&47

;; CPUcpsid: 0000A0E5
CPUcpsid proc
	strhvc	r7,[r7],-#&6

l0000A0E9:
	adcslo	r0,pc,r7,asr #&20

;; CPUwfi: 0000A0ED
CPUwfi proc
	strhvc	r7,[r7],-#&F

l0000A0F1:
	ldmlo	pc!,{r0-r2,r6}

;; I2CMasterInit: 0000A0F5
I2CMasterInit proc
	subeq	r0,r6,#&2D40

l0000A0F9:
	submi	r0,r6,#&6A000000

l0000A0FD:
	andeq	r1,r2,#&F0

l0000A101:
	ldrbvc	pc,[r7,#&F62]!

l0000A105:
	Invalid

l0000A109:
	teqeq	r8,r10,asr #2

l0000A10D:
	movsne	r0,#&2D0000

l0000A111:
	tstlt	r8,r6,asr #2

l0000A115:
	ldrsheq	pc,[r1,#&3B]!

l0000A119:
	stmlo	r0!,{r0,r3-r5,r8,sp-pc}

l0000A11D:
	ldrhthi	r0,[pc],#&D                                        ; 0000A132

l0000A121:
	andeq	r1,r0,r4,lsl #&1D

l0000A125:
	andne	r7,r0,r2,lsl r10

;; I2CSlaveInit: 0000A129
I2CSlaveInit proc
	strhtge	r0,[r4],-#&14

l0000A12D:
	msrne	spsr,#&CF5

l0000A131:
	rscshs	r4,r0,r8,ror #6

l0000A135:
	strbtmi	r1,[r0],-#&303

l0000A139:
	rsbne	r0,r0,r0,ror #2

l0000A13D:
	movteq	r7,#&70BC

;; I2CMasterEnable: 0000A141
I2CMasterEnable proc
	rscsne	r4,r0,r10,ror #6

l0000A145:
	rsbvc	r0,r2,r3,lsl #6

l0000A149:
	asrseq	r0,r7,#&20

;; I2CSlaveEnable: 0000A14D
I2CSlaveEnable proc
	Invalid
	msrmi	spsr,#&362
	movne	r2,#&30F0
	rsbvc	r4,r0,r0,ror #2
	movseq	r0,#&47

;; I2CMasterDisable: 0000A161
I2CMasterDisable proc
	rscsne	r2,r0,r10,ror #6

l0000A165:
	rsbvc	r0,r2,r3,lsl #6

l0000A169:
	adcseq	r0,pc,r7,asr #&20

;; I2CSlaveDisable: 0000A16D
I2CSlaveDisable proc
	Invalid
	msrne	spsr,#&362
	rscshs	r2,r0,r8,ror #6
	rsbvc	r1,r0,r3,lsl #6
	ldmeq	pc!,{r0-r2,r6}

;; I2CIntRegister: 0000A181
I2CIntRegister proc
	svc	#&2018B5
	Invalid

l0000A189:
	stmne	r0,{r3,r5-r7,fp}

l0000A18D:
	ldrbhs	pc,[r7,#&F20]!

l0000A191:
	ldmeq	pc!,{r1,r3-r5,r7}

;; I2CIntUnregister: 0000A195
I2CIntUnregister proc
	svc	#&2018B5
	Invalid

l0000A19D:
	stmne	r0,{r3,r5-r7,fp}

l0000A1A1:
	ldmibgt	r7,{r5,r8-pc}^

l0000A1A5:
	ldrheq	r0,[pc,r9]!                                         ; 0000A1AD

;; I2CMasterIntEnable: 0000A1A9
I2CMasterIntEnable proc
	rsbvc	r0,r1,r3,lsr #6

l0000A1AD:
	asrseq	r0,r7,#&20

;; I2CSlaveIntEnable: 0000A1B1
I2CSlaveIntEnable proc
	rsbvc	ip,r0,r3,lsr #6

l0000A1B5:
	adcseq	r0,pc,r7,asr #&20

;; I2CMasterIntDisable: 0000A1B9
I2CMasterIntDisable proc
	rsbvc	r0,r1,r3,lsr #6

l0000A1BD:
	adcseq	r0,pc,r7,asr #&20

;; I2CSlaveIntDisable: 0000A1C1
I2CSlaveIntDisable proc
	rsbvc	ip,r0,r3,lsr #6

l0000A1C5:
	asrshs	r0,r7,#&20

;; I2CMasterIntStatus: 0000A1C9
I2CMasterIntStatus proc
	strhteq	r4,[r9],-#&9

l0000A1CD:
	movseq	pc,r0,lsr r8

l0000A1D1:
	subhi	r7,r7,r0,lsr #&20

l0000A1D5:
	ldmdane	r0!,{r0,r3,r5-r6}

l0000A1D9:
	strhtvc	r0,[r0],-#&1F

l0000A1DD:
	asrshs	r0,r7,#&20

;; I2CSlaveIntStatus: 0000A1E1
I2CSlaveIntStatus proc
	strhteq	r0,[r9],-#&9

l0000A1E5:
	movseq	pc,r0,lsr r8

l0000A1E9:
	submi	r7,r7,r0,lsr #&20

l0000A1ED:
	ldmdane	r0!,{r0,r3,r5-r6}

l0000A1F1:
	strhtvc	r0,[r0],-#&1F

l0000A1F5:
	asrseq	r0,r7,#&20

;; I2CMasterIntClear: 0000A1F9
I2CMasterIntClear proc
	msrhi	spsr,#&323

l0000A1FD:
	Invalid

;; I2CSlaveIntClear: 0000A201
I2CSlaveIntClear proc
	rsbvc	r8,r1,r3,lsr #6

l0000A205:
	adcsmi	r0,pc,#&47

;; I2CMasterSlaveAddrSet: 0000A209
I2CMasterSlaveAddrSet proc
	andeq	r4,r2,#&8000003A

l0000A20D:
	submi	r7,r7,r0,rrx

;; I2CMasterBusy: 0000A211
I2CMasterBusy proc
	mvnseq	r0,r8,rrx

l0000A215:
	submi	r7,r7,r0

;; I2CMasterBusBusy: 0000A219
I2CMasterBusBusy proc
	rscshi	ip,r3,r8,rrx

l0000A21D:
	Invalid

;; I2CMasterControl: 0000A221
I2CMasterControl proc
	movtmi	r7,#&7060

;; I2CMasterErr: 0000A225
I2CMasterErr proc
	streq	sp,[r7,-#&A68]

l0000A229:
	rscseq	r1,r0,#&50000003

l0000A22D:
	bicseq	r0,r0,#0

l0000A231:
	Invalid

l0000A235:
	eorvc	r0,r0,r7,asr #&20

l0000A239:
	asrshi	r0,r7,#&20

;; I2CMasterDataPut: 0000A23D
I2CMasterDataPut proc
	subhi	r7,r7,r0,rrx

;; I2CMasterDataGet: 0000A241
I2CMasterDataGet proc
	submi	r7,r7,r8,rrx

;; I2CSlaveStatus: 0000A245
I2CSlaveStatus proc
	Invalid

;; I2CSlaveDataPut: 0000A249
I2CSlaveDataPut proc
	subhi	r7,r7,r0,rrx

;; I2CSlaveDataGet: 0000A24D
I2CSlaveDataGet proc
	stmmi	r7,{r3,r5-r6,ip-lr}

l0000A251:
	svcvs	#&6C6C65

l0000A255:
	movmi	r0,#0

l0000A259:
	blvs	$018E3801

l0000A25D:
	andpl	r0,r0,r0

l0000A261:
	strbtvc	r6,[lr],-#&972

l0000A265:
	movpl	r0,#0

l0000A269:
	Invalid

l0000A26D:
	svcvs	#&6E2064

l0000A271:
	strbvs	r2,[r2,-#&74]!

l0000A275:
	strbvs	r7,[r8,-#&420]!

l0000A279:
	stmdbmi	r0,{r1,r4-r6,r8,r10,sp-lr}

l0000A27D:
	subeq	r4,r5,r4,asr #&18

l0000A281:
	strls	r0,[r0],-r0
xFlashRates.4473		; 0000A284
	db	0x96, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00
	db	0x2C, 0x01, 0x00, 0x00, 0x5E, 0x01, 0x00, 0x00, 0x90, 0x01, 0x00, 0x00, 0xC2, 0x01, 0x00, 0x00
	db	0xF4, 0x01, 0x00, 0x00

l0000A285:
	stmdagt	r0,}

l0000A289:
	blx	$0000A291
	Invalid

;; fn0000A291: 0000A291
fn0000A291 proc
	Invalid

l0000A295:
	andls	r0,r0,r1

l0000A299:
	andgt	r0,r0,#1

l0000A29D:
	Invalid
	andeq	r0,r0,r1
g_pulPriority		; 0000A2A4
	db	0x00, 0x07, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00
	db	0x00, 0x04, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00
g_pulRegs		; 0000A2C4
	db	0x00, 0x00, 0x00, 0x00, 0x18, 0xED, 0x00, 0xE0, 0x1C, 0xED, 0x00, 0xE0
	db	0x20, 0xED, 0x00, 0xE0, 0x00, 0xE4, 0x00, 0xE0, 0x04, 0xE4, 0x00, 0xE0, 0x08, 0xE4, 0x00, 0xE0
	db	0x0C, 0xE4, 0x00, 0xE0, 0x10, 0xE4, 0x00, 0xE0, 0x14, 0xE4, 0x00, 0xE0, 0x18, 0xE4, 0x00, 0xE0
	db	0x1C, 0xE4, 0x00, 0xE0
pucRow1.4380		; 0000A2F4
	db	0xB0, 0x80, 0x04, 0x80, 0x12, 0x40
0000A2FA                               00 00                       ..   
pucRow2.4381		; 0000A2FC
	db	0xB1, 0x80, 0x04, 0x80
	db	0x12, 0x40
0000A302       00 00                                       ..           
g_pucFont		; 0000A304
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4F, 0x00, 0x00, 0x00, 0x07
	db	0x00, 0x07, 0x00, 0x14, 0x7F, 0x14, 0x7F, 0x14, 0x24, 0x2A, 0x7F, 0x2A, 0x12, 0x23, 0x13, 0x08
	db	0x64, 0x62, 0x36, 0x49, 0x55, 0x22, 0x50, 0x00, 0x05, 0x03, 0x00, 0x00, 0x00, 0x1C, 0x22, 0x41
	db	0x00, 0x00, 0x41, 0x22, 0x1C, 0x00, 0x14, 0x08, 0x3E, 0x08, 0x14, 0x08, 0x08, 0x3E, 0x08, 0x08
	db	0x00, 0x50, 0x30, 0x00, 0x00, 0x08, 0x08, 0x08, 0x08, 0x08, 0x00, 0x60, 0x60, 0x00, 0x00, 0x20
	db	0x10, 0x08, 0x04, 0x02, 0x3E, 0x51, 0x49, 0x45, 0x3E, 0x00, 0x42, 0x7F, 0x40, 0x00, 0x42, 0x61
	db	0x51, 0x49, 0x46, 0x21, 0x41, 0x45, 0x4B, 0x31, 0x18, 0x14, 0x12, 0x7F, 0x10, 0x27, 0x45, 0x45
	db	0x45, 0x39, 0x3C, 0x4A, 0x49, 0x49, 0x30, 0x01, 0x71, 0x09, 0x05, 0x03, 0x36, 0x49, 0x49, 0x49
	db	0x36, 0x06, 0x49, 0x49, 0x29, 0x1E, 0x00, 0x36, 0x36, 0x00, 0x00, 0x00, 0x56, 0x36, 0x00, 0x00
	db	0x08, 0x14, 0x22, 0x41, 0x00, 0x14, 0x14, 0x14, 0x14, 0x14, 0x00, 0x41, 0x22, 0x14, 0x08, 0x02
	db	0x01, 0x51, 0x09, 0x06, 0x32, 0x49, 0x79, 0x41, 0x3E, 0x7E, 0x11, 0x11, 0x11, 0x7E, 0x7F, 0x49
	db	0x49, 0x49, 0x36, 0x3E, 0x41, 0x41, 0x41, 0x22, 0x7F, 0x41, 0x41, 0x22, 0x1C, 0x7F, 0x49, 0x49
	db	0x49, 0x41, 0x7F, 0x09, 0x09, 0x09, 0x01, 0x3E, 0x41, 0x49, 0x49, 0x7A, 0x7F, 0x08, 0x08, 0x08
	db	0x7F, 0x00, 0x41, 0x7F, 0x41, 0x00, 0x20, 0x40, 0x41, 0x3F, 0x01, 0x7F, 0x08, 0x14, 0x22, 0x41
	db	0x7F, 0x40, 0x40, 0x40, 0x40, 0x7F, 0x02, 0x0C, 0x02, 0x7F, 0x7F, 0x04, 0x08, 0x10, 0x7F, 0x3E
	db	0x41, 0x41, 0x41, 0x3E, 0x7F, 0x09, 0x09, 0x09, 0x06, 0x3E, 0x41, 0x51, 0x21, 0x5E, 0x7F, 0x09
	db	0x19, 0x29, 0x46, 0x46, 0x49, 0x49, 0x49, 0x31, 0x01, 0x01, 0x7F, 0x01, 0x01, 0x3F, 0x40, 0x40
	db	0x40, 0x3F, 0x1F, 0x20, 0x40, 0x20, 0x1F, 0x3F, 0x40, 0x38, 0x40, 0x3F, 0x63, 0x14, 0x08, 0x14
	db	0x63, 0x07, 0x08, 0x70, 0x08, 0x07, 0x61, 0x51, 0x49, 0x45, 0x43, 0x00, 0x7F, 0x41, 0x41, 0x00
	db	0x02, 0x04, 0x08, 0x10, 0x20, 0x00, 0x41, 0x41, 0x7F, 0x00, 0x04, 0x02, 0x01, 0x02, 0x04, 0x40
	db	0x40, 0x40, 0x40, 0x40, 0x00, 0x01, 0x02, 0x04, 0x00, 0x20, 0x54, 0x54, 0x54, 0x78, 0x7F, 0x48
	db	0x44, 0x44, 0x38, 0x38, 0x44, 0x44, 0x44, 0x20, 0x38, 0x44, 0x44, 0x48, 0x7F, 0x38, 0x54, 0x54
	db	0x54, 0x18, 0x08, 0x7E, 0x09, 0x01, 0x02, 0x0C, 0x52, 0x52, 0x52, 0x3E, 0x7F, 0x08, 0x04, 0x04
	db	0x78, 0x00, 0x44, 0x7D, 0x40, 0x00, 0x20, 0x40, 0x44, 0x3D, 0x00, 0x7F, 0x10, 0x28, 0x44, 0x00
	db	0x00, 0x41, 0x7F, 0x40, 0x00, 0x7C, 0x04, 0x18, 0x04, 0x78, 0x7C, 0x08, 0x04, 0x04, 0x78, 0x38
	db	0x44, 0x44, 0x44, 0x38, 0x7C, 0x14, 0x14, 0x14, 0x08, 0x08, 0x14, 0x14, 0x18, 0x7C, 0x7C, 0x08
	db	0x04, 0x04, 0x08, 0x48, 0x54, 0x54, 0x54, 0x20, 0x04, 0x3F, 0x44, 0x40, 0x20, 0x3C, 0x40, 0x40
	db	0x20, 0x7C, 0x1C, 0x20, 0x40, 0x20, 0x1C, 0x3C, 0x40, 0x30, 0x40, 0x3C, 0x44, 0x28, 0x10, 0x28
	db	0x44, 0x0C, 0x50, 0x50, 0x50, 0x3C, 0x44, 0x64, 0x54, 0x4C, 0x44, 0x00, 0x08, 0x36, 0x41, 0x00
	db	0x00, 0x00, 0x7F, 0x00, 0x00, 0x00, 0x41, 0x36, 0x08, 0x00, 0x02, 0x01, 0x02, 0x04, 0x02
0000A4DF                                              00                .
g_pucOSRAMInit		; 0000A4E0
	db	0x04, 0x80, 0xAE, 0x80, 0xE3, 0x04, 0x80, 0x04, 0x80, 0xE3, 0x04, 0x80, 0x12, 0x80, 0xE3, 0x06
	db	0x80, 0x81, 0x80, 0x2B, 0x80, 0xE3, 0x04, 0x80, 0xA1, 0x80, 0xE3, 0x04, 0x80, 0x40, 0x80, 0xE3
	db	0x06, 0x80, 0xD3, 0x80, 0x00, 0x80, 0xE3, 0x06, 0x80, 0xA8, 0x80, 0x0F, 0x80, 0xE3, 0x04, 0x80
	db	0xA4, 0x80, 0xE3, 0x04, 0x80, 0xA6, 0x80, 0xE3, 0x04, 0x80, 0xB0, 0x80, 0xE3, 0x04, 0x80, 0xC8
	db	0x80, 0xE3, 0x06, 0x80, 0xD5, 0x80, 0x72, 0x80, 0xE3, 0x06, 0x80, 0xD8, 0x80, 0x00, 0x80, 0xE3
	db	0x06, 0x80, 0xD9, 0x80, 0x22, 0x80, 0xE3, 0x06, 0x80, 0xDA, 0x80, 0x12, 0x80, 0xE3, 0x06, 0x80
	db	0xDB, 0x80, 0x0F, 0x80, 0xE3, 0x06, 0x80, 0xAD, 0x80, 0x8B, 0x80, 0xE3, 0x04, 0x80, 0xAF, 0x80
	db	0xE3
0000A551    00 00 00                                      ...           
g_pulDCRegs		; 0000A554
	db	0x10, 0xE0, 0x0F, 0x40, 0x14, 0xE0, 0x0F, 0x40, 0x1C, 0xE0, 0x0F, 0x40
	db	0x10, 0xE0, 0x0F, 0x40
g_pulSRCRRegs		; 0000A564
	db	0x40, 0xE0, 0x0F, 0x40, 0x44, 0xE0, 0x0F, 0x40, 0x48, 0xE0, 0x0F, 0x40
g_pulRCGCRegs		; 0000A570
	db	0x00, 0xE1, 0x0F, 0x40, 0x04, 0xE1, 0x0F, 0x40, 0x08, 0xE1, 0x0F, 0x40
g_pulSCGCRegs		; 0000A57C
	db	0x10, 0xE1, 0x0F, 0x40
	db	0x14, 0xE1, 0x0F, 0x40, 0x18, 0xE1, 0x0F, 0x40
g_pulDCGCRegs		; 0000A588
	db	0x20, 0xE1, 0x0F, 0x40, 0x24, 0xE1, 0x0F, 0x40
	db	0x28, 0xE1, 0x0F, 0x40
g_pulXtals		; 0000A594
	db	0x99, 0x9E, 0x36, 0x00, 0x00, 0x40, 0x38, 0x00, 0x00, 0x09, 0x3D, 0x00
	db	0x00, 0x80, 0x3E, 0x00, 0x00, 0x00, 0x4B, 0x00, 0x40, 0x4B, 0x4C, 0x00, 0x00, 0x20, 0x4E, 0x00
	db	0x80, 0x8D, 0x5B, 0x00, 0x00, 0xC0, 0x5D, 0x00, 0x00, 0x80, 0x70, 0x00, 0x00, 0x12, 0x7A, 0x00
	db	0x00, 0x00, 0x7D, 0x00
;;; Segment .text.memcpy (0000A5C4)
0000A5C4             F0                                      .          

;; memcpy: 0000A5C5
memcpy proc
	svceq	#&5B5

l0000A5C9:
	bleq	$FF656279

l0000A5CD:
	blls	$010CB1D5

l0000A5D1:
	ldrbne	r3,[r1],r7

l0000A5D5:
	moveq	r0,#&C00

l0000A5D9:
	ldrlo	r1,[lr]!

l0000A5DD:
	ldchs	p1,c0,[r5,-#&24]!

l0000A5E1:
	ldrhs	r4,[r9,-r1,lsl #10]

l0000A5E5:
	strbvs	r1,[r0,-r8,ror #30]!

l0000A5E9:
	strbge	r5,[r0,-r8,ror #30]!

l0000A5ED:
	strb	r9,[r0,-r8,ror #30]!
	rsbne	sp,r0,r8,ror #&1E

l0000A5F5:
	ldcls	p0,c1,[r4,-#&CC]!

l0000A5F9:
	svceq	#&D1F342

l0000A5FD:
	subne	r9,r3,r3,lsr #&1C

l0000A601:
	ldmdbhi	r9,{r1-r2,r4-r5,r8,r10,pc}

l0000A605:
	movteq	r1,#&319

l0000A609:
	Invalid

l0000A60D:
	strtlt	r0,[r3],-#&1F

l0000A611:
	ldrtge	r0,[r4],-#&108

l0000A615:
	svc	#&58CF00
	teqge	r3,#&50000000

l0000A61D:
	bicseq	pc,r1,#&42000

l0000A621:
	movtlo	r10,#&3624

l0000A625:
	stmgt	r0,{r0,r2-r4,r9,sp}^

l0000A629:
	andseq	lr,r8,r8,lsl sp

l0000A62D:
	sbcseq	r0,r0,r10,lsr #&A

l0000A631:
	mrrc	p12,#2,ip,ip,c3
	teqls	r3,#&15
	Invalid
	ldmeq	ip!,{r2-r5,r7,r9}
	Invalid
	Invalid
;;; Segment .data (20000000)
g_pfnRAMVectors		; 20000000
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
uxErrorStatus		; 200000B8
	dd	0x00000001
uxCriticalNesting		; 200000BC
	dd	0xAAAAAAAA
xCoRoutineFlashStatus		; 200000C0
	dd	0x00000001
;;; Segment privileged_data (200000C4)
uxCurrentNumberOfTasks		; 200000C4
	dd	0x00000000
pxCurrentTCB		; 200000C8
	dd	0x00000000
pxReadyTasksLists		; 200000CC
	db	0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00
xDelayedTaskList1		; 200000F4
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
xDelayedTaskList2		; 20000108
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
xPendingReadyList		; 2000011C
	db	0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
pxDelayedTaskList		; 20000130
	dd	0x00000000
pxOverflowDelayedTaskList		; 20000134
	dd	0x00000000
xSchedulerRunning		; 20000138
	dd	0x00000000
uxTaskNumber		; 2000013C
	dd	0x00000000
uxTopReadyPriority		; 20000140
	dd	0x00000000
xTickCount		; 20000144
	dd	0x00000000
xNextTaskUnblockTime		; 20000148
	dd	0x00000000
xIdleTaskHandle		; 2000014C
	dd	0x00000000
uxSchedulerSuspended		; 20000150
	dd	0x00000000
xYieldPending		; 20000154
	dd	0x00000000
xNumOfOverflows		; 20000158
	dd	0x00000000
uxPendedTicks		; 2000015C
	dd	0x00000000
;;; Segment .bss (20000160)
pulMainStack		; 20000160
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
cNextChar		; 2000022C
	db	0x00
2000022D                                        00 00 00              ...
pucAlignedHeap.5129		; 20000230
	dd	0x00000000
ucHeap		; 20000234
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
xNextFreeByte		; 200007F0
	dd	0x00000000
ucOutputValue		; 200007F4
	db	0x00
200007F5                00 00 00                              ...       
xFlashQueue		; 200007F8
	dd	0x00000000
pxCurrentCoRoutine		; 200007FC
	dd	0x00000000
pxReadyCoRoutineLists		; 20000800
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
xDelayedCoRoutineList1		; 20000828
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
xDelayedCoRoutineList2		; 2000083C
	db	0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
xPendingReadyCoRoutineList		; 20000850
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00
pxDelayedCoRoutineList		; 20000864
	dd	0x00000000
pxOverflowDelayedCoRoutineList		; 20000868
	dd	0x00000000
uxTopCoRoutineReadyPriority		; 2000086C
	dd	0x00000000
xCoRoutineTickCount		; 20000870
	dd	0x00000000
xLastTickCount		; 20000874
	dd	0x00000000
xPassedTicks		; 20000878
	dd	0x00000000
g_ulDelay		; 2000087C
	dd	0x00000000
xPrintQueue		; 20000880
	dd	0x00000000
