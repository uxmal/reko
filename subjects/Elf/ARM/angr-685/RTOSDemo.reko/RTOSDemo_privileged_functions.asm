;;; Segment privileged_functions (00000000)
00000000 2C 02 00 20 09 80 00 00 01 80 00 00 05 80 00 00 ,.. ............
00000010 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000020 00 00 00 00 00 00 00 00 00 00 00 00 15 17 00 00 ................
00000030 00 00 00 00 00 00 00 00 89 16 00 00 E5 16 00 00 ................
00000040 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000050 00 00 00 00 09 81 00 00                         ........       

;; prvUnlockQueue: 00000058
;;   Called from:
;;     000001EE (in xQueueGenericSend)
;;     00000228 (in xQueueGenericSend)
;;     00000280 (in xQueueGenericSend)
;;     00000300 (in xQueueGenericReceive)
;;     00000354 (in xQueueGenericReceive)
;;     0000037E (in xQueueGenericReceive)
prvUnlockQueue proc
	push	{r4-r6,lr}
	mov	r5,r0
	bl	$00008578
	ldrb	r4,[r5,#&45]
	sxtb	r4,r4
	cmps	r4,#0
	ble	$00000098

l0000006A:
	ldr	r3,[r5,#&24]
	cbz	r3,$00000098

l0000006E:
	add	r6,r5,#&24
	b	$00000080

l00000074:
	subs	r4,#1
	uxtb	r3,r4
	sxtb	r4,r3
	cbz	r3,$00000098

l0000007C:
	ldr	r3,[r5,#&24]
	cbz	r3,$00000098

l00000080:
	mov	r0,r6
	bl	$0000101C
	cmps	r0,#0
	beq	$00000074

l0000008A:
	subs	r4,#1
	bl	$000011AC
	uxtb	r3,r4
	sxtb	r4,r3
	cmps	r3,#0
	bne	$0000007C

l00000098:
	mov	r3,#&FF
	strb	r3,[r5,#&45]
	bl	$000085B0
	bl	$00008578
	ldrb	r4,[r5,#&44]
	sxtb	r4,r4
	cmps	r4,#0
	ble	$000000DE

l000000B0:
	ldr	r3,[r5,#&10]
	cbz	r3,$000000DE

l000000B4:
	add	r6,r5,#&10
	b	$000000C6

l000000BA:
	subs	r4,#1
	uxtb	r3,r4
	sxtb	r4,r3
	cbz	r3,$000000DE

l000000C2:
	ldr	r3,[r5,#&10]
	cbz	r3,$000000DE

l000000C6:
	mov	r0,r6
	bl	$0000101C
	cmps	r0,#0
	beq	$000000BA

l000000D0:
	subs	r4,#1
	bl	$000011AC
	uxtb	r3,r4
	sxtb	r4,r3
	cmps	r3,#0
	bne	$000000C2

l000000DE:
	mov	r3,#&FF
	strb	r3,[r5,#&44]
	pop.w	{r4-r6,lr}
	b	$000085B0

;; prvCopyDataToQueue: 000000EC
;;   Called from:
;;     0000024C (in xQueueGenericSend)
;;     0000048E (in xQueueGenericSendFromISR)
;;     000083C6 (in xQueueCRSend)
;;     000084B4 (in xQueueCRSendFromISR)
prvCopyDataToQueue proc
	push	{r4-r6,lr}
	mov	r4,r0
	ldr	r0,[r0,#&40]
	ldr	r5,[r4,#&38]
	cbnz	r0,$00000102

l000000F6:
	ldr	r6,[r4]
	cmps	r6,#0
	beq	$00000160

l000000FC:
	adds	r5,#1

l000000FE:
	str	r5,[r4,#&38]
	pop	{r4-r6,pc}

l00000102:
	mov	r6,r2
	mov	r2,r0
	cbnz	r6,$00000128

l00000108:
	ldr	r0,[r4,#&8]
	bl	$0000A5C4
	ldr	r3,[r4,#&8]
	ldr	r1,[r4,#&40]
	ldr	r2,[r4,#&4]
	adds	r3,r1
	cmps	r3,r2
	str	r3,[r4,#&8]
	blo	$00000150

l0000011C:
	ldr	r3,[r4]
	adds	r5,#1
	mov	r0,r6
	str	r3,[r4,#&8]
	str	r5,[r4,#&38]
	pop	{r4-r6,pc}

l00000128:
	ldr	r0,[r4,#&C]
	bl	$0000A5C4
	ldr	r2,[r4,#&40]
	ldr	r3,[r4,#&C]
	rsbs	r2,r2
	ldr	r1,[r4]
	adds	r3,r2
	cmps	r3,r1
	str	r3,[r4,#&C]
	bhs	$00000144

l0000013E:
	ldr	r3,[r4,#&4]
	adds	r2,r3
	str	r2,[r4,#&C]

l00000144:
	cmps	r6,#2
	beq	$00000158

l00000148:
	adds	r5,#1
	mov	r0,#0
	str	r5,[r4,#&38]
	pop	{r4-r6,pc}

l00000150:
	adds	r5,#1
	mov	r0,r6
	str	r5,[r4,#&38]
	pop	{r4-r6,pc}

l00000158:
	cbnz	r5,$0000015C

l0000015A:
	mov	r5,#1

l0000015C:
	mov	r0,#0
	b	$000000FE

l00000160:
	ldr	r0,[r4,#&4]
	bl	$00001250
	adds	r5,#1
	str	r6,[r4,#&4]
	b	$000000FE

;; prvCopyDataFromQueue: 0000016C
;;   Called from:
;;     000002CA (in xQueuePeekFromISR)
;;     000003B6 (in xQueueGenericReceive)
;;     00000554 (in xQueueReceiveFromISR)
prvCopyDataFromQueue proc
	ldr	r2,[r0,#&40]
	cbz	r2,$0000018C

l00000170:
	mov	r3,r1
	push	{r4}
	ldr	r1,[r0,#&C]
	ldr	r4,[r0,#&4]
	adds	r1,r2
	cmps	r1,r4
	str	r1,[r0,#&C]
	itt	hs
	ldrhs	r1,[r0]

l00000182:
	str	r1,[r0,#&C]
	pop	{r4}
	mov	r0,r3
	b	$0000A5C4

l0000018C:
	bx	lr
0000018E                                           00 BF               ..

;; xQueueGenericSend: 00000190
;;   Called from:
;;     00000628 (in xQueueGiveMutexRecursive)
;;     000006F8 (in xQueueCreateMutex)
;;     00008AFE (in MPU_xQueueGenericSend)
xQueueGenericSend proc
	push.w	{r4-r10,lr}
	mov	r5,#0
	sub	sp,#&10
	mov	r4,r0
	mov	r10,r1
	str	r2,[sp,#&4]
	mov	r7,r3
	mov	r8,r5
	ldr	r9,[000002A0]                                          ; [pc,#&FC]
	b	$000001F8

l000001A8:
	bl	$000085B0
	bl	$00000A0C
	bl	$00008578
	ldrb	r3,[r4,#&44]
	cmps	r3,#&FF
	it	eq
	strbeq	r8,[r4,#&44]

l000001C0:
	ldrb	r3,[r4,#&45]
	cmps	r3,#&FF
	it	eq
	strbeq	r8,[r4,#&45]

l000001CC:
	bl	$000085B0
	add	r1,sp,#4
	add	r0,sp,#8
	bl	$00001158
	cmps	r0,#0
	bne	$0000027E

l000001DC:
	bl	$00008578
	ldr	r2,[r4,#&38]
	ldr	r3,[r4,#&3C]
	cmps	r2,r3
	beq	$00000218

l000001E8:
	bl	$000085B0
	mov	r0,r4
	bl	$00000058
	bl	$00000E6C

l000001F6:
	mov	r5,#1

l000001F8:
	bl	$00008578
	ldr	r2,[r4,#&38]
	ldr	r3,[r4,#&3C]
	cmps	r2,r3
	blo	$00000246

l00000204:
	cmps	r7,#2
	beq	$00000246

l00000208:
	ldr	r6,[sp,#&4]
	cbz	r6,$00000272

l0000020C:
	cmps	r5,#0
	bne	$000001A8

l00000210:
	add	r0,sp,#8
	bl	$00001144
	b	$000001A8

l00000218:
	bl	$000085B0
	ldr	r1,[sp,#&4]
	add	r0,r4,#&10
	bl	$00000FDC
	mov	r0,r4
	bl	$00000058
	bl	$00000E6C
	cmps	r0,#0
	bne	$000001F6

l00000234:
	mov	r3,#&10000000
	str	r3,[r9]
	dsb	sy
	isb	sy
	b	$000001F6

l00000246:
	mov	r2,r7
	mov	r1,r10
	mov	r0,r4
	bl	$000000EC
	ldr	r3,[r4,#&24]
	cbnz	r3,$00000290

l00000254:
	cbz	r0,$00000266

l00000256:
	mov	r2,#&10000000
	ldr	r3,[000002A0]                                          ; [pc,#&44]
	str	r2,[r3]
	dsb	sy
	isb	sy

l00000266:
	bl	$000085B0
	mov	r0,#1
	add	sp,#&10
	pop.w	{r4-r10,pc}

l00000272:
	bl	$000085B0
	mov	r0,r6
	add	sp,#&10
	pop.w	{r4-r10,pc}

l0000027E:
	mov	r0,r4
	bl	$00000058
	bl	$00000E6C
	mov	r0,#0
	add	sp,#&10
	pop.w	{r4-r10,pc}

l00000290:
	add	r0,r4,#&24
	bl	$0000101C
	cmps	r0,#0
	bne	$00000256

l0000029C:
	b	$00000266
0000029E                                           00 BF               ..
000002A0 04 ED 00 E0                                     ....           

;; xQueuePeekFromISR: 000002A4
;;   Called from:
;;     00008BB4 (in MPU_xQueuePeekFromISR)
xQueuePeekFromISR proc
	push	{r4-r6,lr}
	mrs	r5,cpsr
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r3,[r0,#&38]
	cbnz	r3,$000002C6

l000002BE:
	mov	r0,r3
	msr	cpsr,r5
	pop	{r4-r6,pc}

l000002C6:
	mov	r4,r0
	ldr	r6,[r0,#&C]
	bl	$0000016C
	str	r6,[r4,#&C]
	mov	r0,#1
	msr	cpsr,r5
	pop	{r4-r6,pc}

;; xQueueGenericReceive: 000002D8
;;   Called from:
;;     000005EC (in xQueueTakeMutexRecursive)
;;     00008B86 (in MPU_xQueueGenericReceive)
xQueueGenericReceive proc
	push.w	{r4-r10,lr}
	mov	r5,#0
	sub	sp,#&10
	mov	r4,r0
	mov	r10,r1
	str	r2,[sp,#&4]
	mov	r9,r3
	mov	r7,r5
	ldr	r8,[00000424]                                          ; [pc,#&138]
	b	$0000030A

l000002F0:
	bl	$00008578
	ldr	r3,[r4,#&38]
	cmps	r3,#0
	beq	$0000036A

l000002FA:
	bl	$000085B0
	mov	r0,r4
	bl	$00000058
	bl	$00000E6C

l00000308:
	mov	r5,#1

l0000030A:
	bl	$00008578
	ldr	r6,[r4,#&38]
	cmps	r6,#0
	bne	$000003B0

l00000314:
	ldr	r3,[sp,#&4]
	cmps	r3,#0
	beq	$000003A4

l0000031A:
	cmps	r5,#0
	beq	$0000039C

l0000031E:
	bl	$000085B0
	bl	$00000A0C
	bl	$00008578
	ldrb	r3,[r4,#&44]
	cmps	r3,#&FF
	it	eq
	strbeq	r7,[r4,#&44]

l00000336:
	ldrb	r3,[r4,#&45]
	cmps	r3,#&FF
	it	eq
	strbeq	r7,[r4,#&45]

l00000342:
	bl	$000085B0
	add	r1,sp,#4
	add	r0,sp,#8
	bl	$00001158
	cmps	r0,#0
	beq	$000002F0

l00000352:
	mov	r0,r4
	bl	$00000058
	bl	$00000E6C
	bl	$00008578
	ldr	r3,[r4,#&38]
	cbz	r3,$000003A4

l00000364:
	bl	$000085B0
	b	$00000308

l0000036A:
	bl	$000085B0
	ldr	r3,[r4]
	cbz	r3,$000003D8

l00000372:
	ldr	r1,[sp,#&4]
	add	r0,r4,#&24
	bl	$00000FDC
	mov	r0,r4
	bl	$00000058
	bl	$00000E6C
	cmps	r0,#0
	bne	$00000308

l0000038A:
	mov	r3,#&10000000
	str	r3,[r8]
	dsb	sy
	isb	sy
	b	$00000308

l0000039C:
	add	r0,sp,#8
	bl	$00001144
	b	$0000031E

l000003A4:
	bl	$000085B0
	mov	r0,#0
	add	sp,#&10
	pop.w	{r4-r10,pc}

l000003B0:
	mov	r1,r10
	mov	r0,r4
	ldr	r5,[r4,#&C]
	bl	$0000016C
	cmp	r9,#0
	bne	$000003E8

l000003C0:
	ldr	r3,[r4]
	subs	r6,#1
	str	r6,[r4,#&38]
	cbz	r3,$0000041C

l000003C8:
	ldr	r3,[r4,#&10]
	cbnz	r3,$0000040E

l000003CC:
	bl	$000085B0
	mov	r0,#1
	add	sp,#&10
	pop.w	{r4-r10,pc}

l000003D8:
	bl	$00008578
	ldr	r0,[r4,#&4]
	bl	$000011BC
	bl	$000085B0
	b	$00000372

l000003E8:
	ldr	r3,[r4,#&24]
	str	r5,[r4,#&C]
	cmps	r3,#0
	beq	$000003CC

l000003F0:
	add	r0,r4,#&24
	bl	$0000101C
	cmps	r0,#0
	beq	$000003CC

l000003FC:
	mov	r2,#&10000000
	ldr	r3,[00000424]                                          ; [pc,#&20]
	str	r2,[r3]
	dsb	sy
	isb	sy
	b	$000003CC

l0000040E:
	add	r0,r4,#&10
	bl	$0000101C
	cmps	r0,#0
	bne	$000003FC

l0000041A:
	b	$000003CC

l0000041C:
	bl	$000012D4
	str	r0,[r4,#&4]
	b	$000003C8
00000424             04 ED 00 E0                             ....       

;; uxQueueMessagesWaiting: 00000428
;;   Called from:
;;     00008B28 (in MPU_uxQueueMessagesWaiting)
uxQueueMessagesWaiting proc
	push	{r4,lr}
	mov	r4,r0
	bl	$00008578
	ldr	r4,[r4,#&38]
	bl	$000085B0
	mov	r0,r4
	pop	{r4,pc}
0000043A                               00 BF                       ..   

;; uxQueueSpacesAvailable: 0000043C
;;   Called from:
;;     00008B50 (in MPU_uxQueueSpacesAvailable)
uxQueueSpacesAvailable proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008578
	ldr	r0,[r5,#&38]
	ldr	r4,[r5,#&3C]
	sub	r4,r4,r0
	bl	$000085B0
	mov	r0,r4
	pop	{r3-r5,pc}
00000452       00 BF                                       ..           

;; vQueueDelete: 00000454
;;   Called from:
;;     00008C80 (in MPU_vQueueDelete)
vQueueDelete proc
	b	$00001780

;; xQueueGenericSendFromISR: 00000458
;;   Called from:
;;     0000816E (in vUART_ISR)
xQueueGenericSendFromISR proc
	push	{r3-r7,lr}
	mrs	r6,cpsr
	mov	r4,#&BF
	msr	cpsr,r4
	isb	sy
	dsb	sy
	ldr	r5,[r0,#&38]
	ldr	r4,[r0,#&3C]
	cmps	r5,r4
	blo	$00000482

l00000476:
	cmps	r3,#2
	beq	$00000482

l0000047A:
	mov	r0,#0

l0000047C:
	msr	cpsr,r6
	pop	{r3-r7,pc}

l00000482:
	ldrb	r4,[r0,#&45]
	mov	r7,r2
	sxtb	r4,r4
	mov	r2,r3
	mov	r5,r0
	bl	$000000EC
	add	r3,r4,#1
	beq	$000004A6

l00000496:
	adds	r4,#1
	sxtb	r4,r4
	strb	r4,[r5,#&45]

l0000049E:
	mov	r0,#1
	msr	cpsr,r6
	pop	{r3-r7,pc}

l000004A6:
	ldr	r3,[r5,#&24]
	cmps	r3,#0
	beq	$0000049E

l000004AC:
	add	r0,r5,#&24
	bl	$0000101C
	cmps	r0,#0
	beq	$0000049E

l000004B8:
	cmps	r7,#0
	beq	$0000049E

l000004BC:
	mov	r0,#1
	str	r0,[r7]
	b	$0000047C
000004C2       00 BF                                       ..           

;; xQueueGiveFromISR: 000004C4
xQueueGiveFromISR proc
	push	{r3-r5,lr}
	mrs	r4,cpsr
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r2,[r0,#&38]
	ldr	r3,[r0,#&3C]
	cmps	r2,r3
	bhs	$00000500

l000004E2:
	ldrb	r3,[r0,#&45]
	adds	r2,#1
	sxtb	r3,r3
	str	r2,[r0,#&38]
	add	r2,r3,#1
	beq	$00000508

l000004F0:
	adds	r3,#1
	sxtb	r3,r3
	strb	r3,[r0,#&45]

l000004F8:
	mov	r0,#1

l000004FA:
	msr	cpsr,r4
	pop	{r3-r5,pc}

l00000500:
	mov	r0,#0
	msr	cpsr,r4
	pop	{r3-r5,pc}

l00000508:
	ldr	r3,[r0,#&24]
	cmps	r3,#0
	beq	$000004F8

l0000050E:
	adds	r0,#&24
	mov	r5,r1
	bl	$0000101C
	cmps	r0,#0
	beq	$000004F8

l0000051A:
	cmps	r5,#0
	beq	$000004F8

l0000051E:
	mov	r0,#1
	str	r0,[r5]
	b	$000004FA

;; xQueueReceiveFromISR: 00000524
xQueueReceiveFromISR proc
	push.w	{r4-r8,lr}
	mrs	r6,cpsr
	mov	r4,#&BF
	msr	cpsr,r4
	isb	sy
	dsb	sy
	ldr	r4,[r0,#&38]
	cbnz	r4,$0000054A

l00000540:
	mov	r0,r4

l00000542:
	msr	cpsr,r6
	pop.w	{r4-r8,pc}

l0000054A:
	mov	r7,r0
	ldrb	r5,[r0,#&44]
	mov	r8,r2
	sxtb	r5,r5
	bl	$0000016C
	subs	r4,#1
	add	r3,r5,#1
	str	r4,[r7,#&38]
	beq	$00000572

l00000560:
	adds	r5,#1
	sxtb	r5,r5
	strb	r5,[r7,#&44]

l00000568:
	mov	r0,#1
	msr	cpsr,r6
	pop.w	{r4-r8,pc}

l00000572:
	ldr	r3,[r7,#&10]
	cmps	r3,#0
	beq	$00000568

l00000578:
	add	r0,r7,#&10
	bl	$0000101C
	cmps	r0,#0
	beq	$00000568

l00000584:
	cmp	r8,#0
	beq	$00000568

l0000058A:
	mov	r0,#1
	str	r0,[r8]
	b	$00000542
00000592       00 BF                                       ..           

;; xQueueIsQueueEmptyFromISR: 00000594
xQueueIsQueueEmptyFromISR proc
	ldr	r0,[r0,#&38]
	clz	r0,r0
	lsrs	r0,r0,#5
	bx	lr
0000059E                                           00 BF               ..

;; xQueueIsQueueFullFromISR: 000005A0
xQueueIsQueueFullFromISR proc
	ldr	r3,[r0,#&38]
	ldr	r0,[r0,#&3C]
	sub	r0,r0,r3
	clz	r0,r0
	lsrs	r0,r0,#5
	bx	lr
000005AE                                           00 BF               ..

;; uxQueueMessagesWaitingFromISR: 000005B0
uxQueueMessagesWaitingFromISR proc
	ldr	r0,[r0,#&38]
	bx	lr

;; xQueueGetMutexHolder: 000005B4
;;   Called from:
;;     00008BDC (in MPU_xQueueGetMutexHolder)
xQueueGetMutexHolder proc
	push	{r4,lr}
	mov	r4,r0
	bl	$00008578
	ldr	r3,[r4]
	cbnz	r3,$000005CA

l000005C0:
	ldr	r4,[r4,#&4]
	bl	$000085B0
	mov	r0,r4
	pop	{r4,pc}

l000005CA:
	mov	r4,#0
	bl	$000085B0
	mov	r0,r4
	pop	{r4,pc}

;; xQueueTakeMutexRecursive: 000005D4
;;   Called from:
;;     00008C30 (in MPU_xQueueTakeMutexRecursive)
xQueueTakeMutexRecursive proc
	push	{r4-r6,lr}
	ldr	r5,[r0,#&4]
	mov	r4,r0
	mov	r6,r1
	bl	$00001138
	cmps	r5,r0
	beq	$000005FA

l000005E4:
	mov	r3,#0
	mov	r2,r6
	mov	r1,r3
	mov	r0,r4
	bl	$000002D8
	cbz	r0,$000005F8

l000005F2:
	ldr	r3,[r4,#&C]
	adds	r3,#1
	str	r3,[r4,#&C]

l000005F8:
	pop	{r4-r6,pc}

l000005FA:
	mov	r0,#1
	ldr	r3,[r4,#&C]
	adds	r3,r0
	str	r3,[r4,#&C]
	pop	{r4-r6,pc}

;; xQueueGiveMutexRecursive: 00000604
;;   Called from:
;;     00008C58 (in MPU_xQueueGiveMutexRecursive)
xQueueGiveMutexRecursive proc
	push	{r3-r5,lr}
	ldr	r5,[r0,#&4]
	mov	r4,r0
	bl	$00001138
	cmps	r5,r0
	beq	$00000616

l00000612:
	mov	r0,#0
	pop	{r3-r5,pc}

l00000616:
	ldr	r3,[r4,#&C]
	subs	r3,#1
	str	r3,[r4,#&C]
	cbz	r3,$00000622

l0000061E:
	mov	r0,#1
	pop	{r3-r5,pc}

l00000622:
	mov	r0,r4
	mov	r2,r3
	mov	r1,r3
	bl	$00000190
	mov	r0,#1
	pop	{r3-r5,pc}

;; xQueueGenericReset: 00000630
;;   Called from:
;;     000006D0 (in xQueueGenericCreate)
;;     00008AC8 (in MPU_xQueueGenericReset)
xQueueGenericReset proc
	push	{r4-r6,lr}
	mov	r4,r0
	mov	r6,r1
	mov	r5,#&FF
	bl	$00008578
	mov	r1,#0
	ldr	r3,[r4,#&40]
	ldr	r2,[r4,#&3C]
	ldr	r0,[r4]
	mul	r2,r2,r3
	sub	r3,r2,r3
	adds	r3,r0
	adds	r2,r0
	str	r1,[r4,#&38]
	str	r2,[r4,#&4]
	strb	r5,[r4,#&44]
	str	r3,[r4,#&C]
	str	r0,[r4,#&8]
	strb	r5,[r4,#&45]
	cbnz	r6,$00000690

l00000660:
	ldr	r3,[r4,#&10]
	cbnz	r3,$0000066C

l00000664:
	bl	$000085B0
	mov	r0,#1
	pop	{r4-r6,pc}

l0000066C:
	add	r0,r4,#&10
	bl	$0000101C
	cmps	r0,#0
	beq	$00000664

l00000678:
	mov	r2,#&10000000
	ldr	r3,[000006A8]                                          ; [pc,#&28]
	str	r2,[r3]
	dsb	sy
	isb	sy
	bl	$000085B0
	mov	r0,#1
	pop	{r4-r6,pc}

l00000690:
	add	r0,r4,#&10
	bl	$000082D0
	add	r0,r4,#&24
	bl	$000082D0
	bl	$000085B0
	mov	r0,#1
	pop	{r4-r6,pc}
000006A8                         04 ED 00 E0                     ....   

;; xQueueGenericCreate: 000006AC
;;   Called from:
;;     000006E4 (in xQueueCreateMutex)
;;     00008A9C (in MPU_xQueueGenericCreate)
xQueueGenericCreate proc
	push	{r4-r6,lr}
	mov	r6,r0
	mul	r0,r0,r1
	adds	r0,#&48
	mov	r5,r1
	bl	$0000172C
	mov	r4,r0
	cbz	r0,$000006D4

l000006C0:
	cbz	r5,$000006D8

l000006C2:
	add	r3,r0,#&48
	str	r3,[r0]

l000006C8:
	str	r6,[r4,#&3C]
	str	r5,[r4,#&40]
	mov	r1,#1
	mov	r0,r4
	bl	$00000630

l000006D4:
	mov	r0,r4
	pop	{r4-r6,pc}

l000006D8:
	str	r0,[r4]
	b	$000006C8

;; xQueueCreateMutex: 000006DC
;;   Called from:
;;     00008C04 (in MPU_xQueueCreateMutex)
xQueueCreateMutex proc
	push	{r4,lr}
	mov	r2,r0
	mov	r1,#0
	mov	r0,#1
	bl	$000006AC
	mov	r4,r0
	cbz	r0,$000006FC

l000006EC:
	mov	r3,#0
	str	r3,[r0,#&4]
	str	r3,[r0]
	str	r3,[r0,#&C]
	mov	r2,r3
	mov	r1,r3
	bl	$00000190

l000006FC:
	mov	r0,r4
	pop	{r4,pc}

;; prvInitialiseNewTask: 00000700
;;   Called from:
;;     000008F2 (in xTaskCreate)
;;     00000954 (in xTaskCreateRestricted)
prvInitialiseNewTask proc
	push.w	{r3-fp,lr}
	ldr	r4,[sp,#&30]
	mov	r9,r3
	ldr	r5,[r4,#&50]
	add	r3,r2,#&40000000
	subs	r3,#1
	mov	fp,r2
	ldr	r2,[sp,#&28]
	add.w	r5,r5,r3,lsl #2
	mov	r8,r0
	sub	r3,r1,#1
	mov.w	r10,r2,lsr #&1F
	bic	r5,r5,#7
	adds	r1,#2
	add	r0,r4,#&54
	bic	r2,r2,#&80000000

l0000072E:
	ldrb	r6,[r3,#&1]
	strb	r6,[r0],#&1
	ldrb	r6,[r3,#&1]!
	cbz	r6,$0000073E

l0000073A:
	cmps	r3,r1
	bne	$0000072E

l0000073E:
	cmps	r2,#1
	it	hs
	movhs	r2,#1

l00000744:
	mov	r6,#0
	mov	r7,r2
	str	r2,[r4,#&4C]
	str	r2,[r4,#&58]
	add	r0,r4,#&24
	strb	r6,[r4,#&56]
	str	r6,[r4,#&5C]
	bl	$000082E8
	add	r0,r4,#&38
	bl	$000082E8
	rsb	r3,r7,#2
	str	r3,[r4,#&38]
	ldr	r2,[r4,#&50]
	mov	r3,fp
	ldr	r1,[sp,#&34]
	add	r0,r4,#4
	str	r4,[r4,#&30]
	str	r4,[r4,#&44]
	bl	$00001554
	str	r6,[r4,#&60]
	mov	r3,r10
	strb	r6,[r4,#&64]
	mov	r2,r9
	mov	r1,r8
	mov	r0,r5
	bl	$0000137C
	ldr	r3,[sp,#&2C]
	str	r0,[r4]
	cbz	r3,$00000792

l00000790:
	str	r4,[r3]

l00000792:
	pop.w	{r3-fp,pc}
00000796                   00 BF                               ..       

;; prvAddNewTaskToReadyList: 00000798
;;   Called from:
;;     000008F8 (in xTaskCreate)
;;     0000095A (in xTaskCreateRestricted)
prvAddNewTaskToReadyList proc
	push.w	{r4-r8,lr}
	ldr	r4,[00000854]                                          ; [pc,#&B4]
	mov	r5,r0
	bl	$00008578
	ldr	r3,[r4]
	adds	r3,#1
	str	r3,[r4]
	ldr	r3,[r4,#&4]
	cmps	r3,#0
	beq	$00000812

l000007B0:
	ldr	r3,[r4,#&74]
	cbz	r3,$00000800

l000007B4:
	ldr	r0,[r5,#&4C]
	add	r6,r4,#8

l000007BA:
	mov	r3,#1
	ldr	r1,[r4,#&7C]
	ldr	r2,[r4,#&78]
	lsls	r3,r0
	add.w	r0,r0,r0,lsl #2
	orrs	r3,r1
	adds	r2,#1
	add.w	r0,r6,r0,lsl #2
	add	r1,r5,#&24
	str	r3,[r4,#&7C]
	str	r2,[r4,#&78]
	bl	$000082F0
	bl	$000085B0
	ldr	r3,[r4,#&74]
	cbz	r3,$000007FC

l000007E2:
	ldr	r2,[r4,#&4]
	ldr	r3,[r5,#&4C]
	ldr	r2,[r2,#&4C]
	cmps	r2,r3
	bhs	$000007FC

l000007EC:
	mov	r2,#&10000000
	ldr	r3,[00000858]                                          ; [pc,#&64]
	str	r2,[r3]
	dsb	sy
	isb	sy

l000007FC:
	pop.w	{r4-r8,pc}

l00000800:
	ldr	r3,[r4,#&4]
	ldr	r0,[r5,#&4C]
	ldr	r3,[r3,#&4C]
	add	r6,r4,#8
	cmps	r3,r0
	it	ls
	strls	r5,[r4,#&4]

l00000810:
	b	$000007BA

l00000812:
	str	r5,[r4,#&4]
	ldr	r3,[r4]
	cmps	r3,#1
	bne	$000007B4

l0000081A:
	add	r6,r4,#8
	mov	r0,r6
	bl	$000082D0
	add	r8,r4,#&30
	add	r0,r4,#&1C
	bl	$000082D0
	add	r7,r4,#&44
	mov	r0,r8
	bl	$000082D0
	mov	r0,r7
	bl	$000082D0
	add	r0,r4,#&58
	bl	$000082D0
	str	r8,[r4,#&6C]
	ldr	r0,[r5,#&4C]
	str	r7,[r4,#&70]
	b	$000007BA
00000852       00 BF C4 00 00 20 04 ED 00 E0               ..... ....   

;; prvAddCurrentTaskToDelayedList.isra.0: 0000085C
;;   Called from:
;;     00000C3E (in xTaskNotifyWait)
;;     00000D4C (in ulTaskNotifyTake)
;;     00000F6A (in vTaskDelay)
;;     00000FC4 (in vTaskDelayUntil)
;;     00000FF0 (in vTaskPlaceOnEventList)
;;     00001016 (in vTaskPlaceOnUnorderedEventList)
prvAddCurrentTaskToDelayedList.isra.0 proc
	push	{r4-r6,lr}
	ldr	r4,[000008B0]                                          ; [pc,#&50]
	mov	r5,r0
	ldr	r6,[r4,#&80]
	ldr	r0,[r4,#&4]
	adds	r0,#&24
	bl	$00008340
	cbnz	r0,$00000880

l00000870:
	mov	r2,#1
	ldr	r1,[r4,#&4]
	ldr	r3,[r4,#&7C]
	ldr	r1,[r1,#&4C]
	lsls	r2,r1
	bic.w	r3,r3,r2
	str	r3,[r4,#&7C]

l00000880:
	adds	r5,r6
	ldr	r3,[r4,#&4]
	cmps	r6,r5
	str	r5,[r3,#&24]
	bhi	$000008A2

l0000088A:
	ldr	r0,[r4,#&6C]
	ldr	r1,[r4,#&4]
	adds	r1,#&24
	bl	$0000830C
	ldr	r3,[r4,#&84]
	cmps	r5,r3
	it	lo
	strlo	r5,[r4,#&84]

l000008A0:
	pop	{r4-r6,pc}

l000008A2:
	ldr	r0,[r4,#&70]
	ldr	r1,[r4,#&4]
	pop.w	{r4-r6,lr}
	adds	r1,#&24
	b	$0000830C
000008B0 C4 00 00 20                                     ...            

;; xTaskCreate: 000008B4
;;   Called from:
;;     000009AA (in vTaskStartScheduler)
;;     0000882C (in MPU_xTaskCreate)
xTaskCreate proc
	push.w	{r4-r10,lr}
	mov	r8,r0
	sub	sp,#&10
	lsls	r0,r2,#2
	mov	r6,r2
	mov	r9,r1
	mov	r10,r3
	bl	$0000172C
	cbz	r0,$00000904

l000008CA:
	mov	r5,r0
	mov	r0,#&68
	bl	$0000172C
	mov	r4,r0
	cbz	r0,$0000090E

l000008D6:
	mov	r7,#0
	str	r5,[r0,#&50]
	ldr	r5,[sp,#&34]
	strb	r7,[r4,#&65]
	str	r5,[sp,#&4]
	ldr	r5,[sp,#&30]
	mov	r3,r10
	mov	r2,r6
	mov	r1,r9
	mov	r0,r8
	str	r7,[sp,#&C]
	str	r4,[sp,#&8]
	str	r5,[sp]
	bl	$00000700
	mov	r0,r4
	bl	$00000798
	mov	r0,#1

l000008FE:
	add	sp,#&10
	pop.w	{r4-r10,pc}

l00000904:
	mov	r0,#&FFFFFFFF
	add	sp,#&10
	pop.w	{r4-r10,pc}

l0000090E:
	mov	r0,r5
	bl	$00001780
	mov	r0,#&FFFFFFFF
	b	$000008FE
0000091A                               00 BF                       ..   

;; xTaskCreateRestricted: 0000091C
;;   Called from:
;;     000087EC (in MPU_xTaskCreateRestricted)
xTaskCreateRestricted proc
	ldr	r3,[r0,#&14]
	cbz	r3,$0000096A

l00000920:
	push	{r4-r7,lr}
	mov	r4,r0
	sub	sp,#&14
	mov	r0,#&68
	mov	r7,r1
	bl	$0000172C
	mov	r5,r0
	cbz	r0,$00000964

l00000932:
	mov	r6,#1
	ldr	r1,[r4,#&14]
	strb	r6,[r0,#&65]
	ldr	r3,[r4,#&C]
	ldrh	r2,[r4,#&8]
	ldr	lr,[r4,#&10]
	str	r1,[r0,#&50]
	ldr	r1,[r4,#&4]
	str	r0,[sp,#&8]
	str	r7,[sp,#&4]
	ldr	r0,[r4],#&18
	str	lr,[sp]
	str	r4,[sp,#&C]
	bl	$00000700
	mov	r0,r5
	bl	$00000798
	mov	r0,r6

l00000960:
	add	sp,#&14
	pop	{r4-r7,pc}

l00000964:
	mov	r0,#&FFFFFFFF
	b	$00000960

l0000096A:
	mov	r0,#&FFFFFFFF
	bx	lr

;; vTaskAllocateMPURegions: 00000970
;;   Called from:
;;     0000885C (in MPU_vTaskAllocateMPURegions)
vTaskAllocateMPURegions proc
	cbz	r0,$0000097C

l00000972:
	mov	r3,#0
	adds	r0,#4
	mov	r2,r3
	b	$00001554

l0000097C:
	ldr	r3,[0000098C]                                          ; [pc,#&C]
	ldr	r0,[r3,#&4]
	mov	r3,#0
	adds	r0,#4
	mov	r2,r3
	b	$00001554
0000098A                               00 BF C4 00 00 20           ..... 

;; vTaskStartScheduler: 00000990
;;   Called from:
;;     000080DE (in Main)
vTaskStartScheduler proc
	mov	r3,#&80000000
	push	{r4,lr}
	ldr	r4,[000009E0]                                          ; [pc,#&48]
	sub	sp,#8
	str	r3,[sp]
	add	r3,r4,#&88
	str	r3,[sp,#&4]
	mov	r2,#&3B
	mov	r3,#0
	ldr	r1,[000009E4]                                          ; [pc,#&3C]
	ldr	r0,[000009E8]                                          ; [pc,#&3C]
	bl	$000008B4
	cmps	r0,#1
	beq	$000009B6

l000009B2:
	add	sp,#8
	pop	{r4,pc}

l000009B6:
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	mov	r2,#&FFFFFFFF
	mov	r3,#0
	str	r2,[r4,#&84]
	str	r0,[r4,#&74]
	str	r3,[r4,#&80]
	add	sp,#8
	pop.w	{r4,lr}
	b	$000013B0
000009E0 C4 00 00 20 7C A2 00 00 2D 85 00 00             ... |...-...   

;; vTaskEndScheduler: 000009EC
vTaskEndScheduler proc
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	mov	r2,#0
	ldr	r3,[00000A08]                                          ; [pc,#&8]
	str	r2,[r3,#&74]
	b	$00001550
00000A06                   00 BF C4 00 00 20                   .....    

;; vTaskSuspendAll: 00000A0C
;;   Called from:
;;     000001AC (in xQueueGenericSend)
;;     00000322 (in xQueueGenericReceive)
;;     0000173A (in pvPortMalloc)
;;     000017D0 (in xEventGroupWaitBits)
;;     00001896 (in xEventGroupSetBits)
;;     00001904 (in xEventGroupSync)
;;     000019A8 (in vEventGroupDelete)
;;     000088C8 (in MPU_vTaskSuspendAll)
vTaskSuspendAll proc
	ldr	r2,[00000A1C]                                          ; [pc,#&C]
	ldr	r3,[r2,#&8C]
	adds	r3,#1
	str	r3,[r2,#&8C]
	bx	lr
00000A1A                               00 BF C4 00 00 20           ..... 

;; xTaskGetTickCount: 00000A20
;;   Called from:
;;     0000890C (in MPU_xTaskGetTickCount)
xTaskGetTickCount proc
	ldr	r3,[00000A28]                                          ; [pc,#&4]
	ldr	r0,[r3,#&80]
	bx	lr
00000A28                         C4 00 00 20                     ...    

;; xTaskGetTickCountFromISR: 00000A2C
xTaskGetTickCountFromISR proc
	ldr	r3,[00000A34]                                          ; [pc,#&4]
	ldr	r0,[r3,#&80]
	bx	lr
00000A34             C4 00 00 20                             ...        

;; uxTaskGetNumberOfTasks: 00000A38
;;   Called from:
;;     00008930 (in MPU_uxTaskGetNumberOfTasks)
uxTaskGetNumberOfTasks proc
	ldr	r3,[00000A40]                                          ; [pc,#&4]
	ldr	r0,[r3]
	bx	lr
00000A3E                                           00 BF               ..
00000A40 C4 00 00 20                                     ...            

;; pcTaskGetName: 00000A44
;;   Called from:
;;     00008958 (in MPU_pcTaskGetName)
pcTaskGetName proc
	cbz	r0,$00000A4A

l00000A46:
	adds	r0,#&54
	bx	lr

l00000A4A:
	ldr	r3,[00000A54]                                          ; [pc,#&8]
	ldr	r0,[r3,#&4]
	adds	r0,#&54
	bx	lr
00000A52       00 BF C4 00 00 20                           .....        

;; xTaskGenericNotify: 00000A58
;;   Called from:
;;     000089DE (in MPU_xTaskGenericNotify)
xTaskGenericNotify proc
	push	{r3-r7,lr}
	mov	r4,r3
	mov	r6,r0
	mov	r7,r1
	mov	r5,r2
	bl	$00008578
	cbz	r4,$00000A6C

l00000A68:
	ldr	r3,[r6,#&60]
	str	r3,[r4]

l00000A6C:
	mov	r3,#2
	ldrb	r4,[r6,#&64]
	sub	r2,r5,#1
	strb	r3,[r6,#&64]
	uxtb	r4,r4
	cmps	r2,#3
	bhi	$00000A8C

l00000A7E:
	tbb	[pc,r2]                                                ; 00000A80
l00000A82	db	0x3A
l00000A83	db	0x0C
l00000A84	db	0x04
l00000A85	db	0x02

l00000A86:
	cmps	r4,#2
	beq	$00000AFE

l00000A8A:
	str	r7,[r6,#&60]

l00000A8C:
	cmps	r4,#1
	beq	$00000AA6

l00000A90:
	mov	r4,#1

l00000A92:
	bl	$000085B0
	mov	r0,r4
	pop	{r3-r7,pc}

l00000A9A:
	ldr	r3,[r6,#&60]
	cmps	r4,#1
	add	r3,r3,#1
	str	r3,[r6,#&60]
	bne	$00000A90

l00000AA6:
	add	r7,r6,#&24
	ldr	r5,[00000B04]                                          ; [pc,#&58]
	mov	r0,r7
	bl	$00008340
	ldr	r0,[r6,#&4C]
	ldr	lr,[r5,#&7C]
	add	r2,r5,#8
	lsl	r3,r4,r0
	add.w	r0,r0,r0,lsl #2
	orr	r3,r3,lr
	add.w	r0,r2,r0,lsl #2
	mov	r1,r7
	str	r3,[r5,#&7C]
	bl	$000082F0
	ldr	r3,[r5,#&4]
	ldr	r2,[r6,#&4C]
	ldr	r3,[r3,#&4C]
	cmps	r2,r3
	bls	$00000A90

l00000ADE:
	mov	r2,#&10000000
	ldr	r3,[00000B08]                                          ; [pc,#&24]
	str	r2,[r3]
	dsb	sy
	isb	sy
	bl	$000085B0
	mov	r0,r4
	pop	{r3-r7,pc}

l00000AF6:
	ldr	r3,[r6,#&60]
	orrs	r7,r3
	str	r7,[r6,#&60]
	b	$00000A8C

l00000AFE:
	mov	r4,#0
	b	$00000A92
00000B02       00 BF C4 00 00 20 04 ED 00 E0               ..... ....   

;; xTaskGenericNotifyFromISR: 00000B0C
xTaskGenericNotifyFromISR proc
	push.w	{r4-r8,lr}
	mrs	r5,cpsr
	mov	r4,#&BF
	msr	cpsr,r4
	isb	sy
	dsb	sy
	cbz	r3,$00000B2A

l00000B26:
	ldr	r4,[r0,#&60]
	str	r4,[r3]

l00000B2A:
	mov	r3,#2
	ldrb	r4,[r0,#&64]
	subs	r2,#1
	strb	r3,[r0,#&64]
	uxtb	r4,r4
	cmps	r2,#3
	bhi	$00000B4A

l00000B3C:
	tbb	[pc,r2]                                                ; 00000B40
l00000B40	db	0x2A
l00000B41	db	0x0C
l00000B42	db	0x04
l00000B43	db	0x02

l00000B44:
	cmps	r4,#2
	beq	$00000BC4

l00000B48:
	str	r1,[r0,#&60]

l00000B4A:
	cmps	r4,#1
	beq	$00000B64

l00000B4E:
	mov	r0,#1

l00000B50:
	msr	cpsr,r5
	pop.w	{r4-r8,pc}

l00000B58:
	ldr	r3,[r0,#&60]
	cmps	r4,#1
	add	r3,r3,#1
	str	r3,[r0,#&60]
	bne	$00000B4E

l00000B64:
	ldr	r6,[00000BD0]                                          ; [pc,#&68]
	mov	r7,r0
	ldr	r3,[r6,#&8C]
	cbz	r3,$00000B9C

l00000B6E:
	add	r1,r0,#&38
	add	r0,r6,#&58
	bl	$000082F0

l00000B7A:
	ldr	r3,[r6,#&4]
	ldr	r2,[r7,#&4C]
	ldr	r3,[r3,#&4C]
	cmps	r2,r3
	bls	$00000B4E

l00000B84:
	ldr	r3,[sp,#&18]
	mov	r0,#1
	cbz	r3,$00000BC8

l00000B8A:
	str	r0,[r3]
	msr	cpsr,r5
	pop.w	{r4-r8,pc}

l00000B94:
	ldr	r3,[r0,#&60]
	orrs	r1,r3
	str	r1,[r0,#&60]
	b	$00000B4A

l00000B9C:
	add	r8,r0,#&24
	mov	r0,r8
	bl	$00008340
	ldr	r0,[r7,#&4C]
	ldr	r2,[r6,#&7C]
	lsls	r4,r0
	add	r3,r6,#8
	add.w	r0,r0,r0,lsl #2
	orrs	r4,r2
	mov	r1,r8
	add.w	r0,r3,r0,lsl #2
	str	r4,[r6,#&7C]
	bl	$000082F0
	b	$00000B7A

l00000BC4:
	mov	r0,#0
	b	$00000B50

l00000BC8:
	str	r0,[r6,#&90]
	b	$00000B50
00000BCE                                           00 BF               ..
00000BD0 C4 00 00 20                                     ...            

;; xTaskNotifyWait: 00000BD4
;;   Called from:
;;     00008A16 (in MPU_xTaskNotifyWait)
xTaskNotifyWait proc
	push.w	{r4-r8,lr}
	ldr	r4,[00000C58]                                          ; [pc,#&7C]
	mov	r5,r2
	mov	r8,r0
	mov	r6,r1
	mov	r7,r3
	bl	$00008578
	ldr	r2,[r4,#&4]
	ldrb	r2,[r2,#&64]
	cmps	r2,#2
	beq	$00000C04

l00000BF0:
	mov	r0,#1
	ldr	r1,[r4,#&4]
	ldr	r2,[r1,#&60]
	bic.w	r2,r2,r8
	str	r2,[r1,#&60]
	ldr	r3,[r4,#&4]
	strb	r0,[r3,#&64]
	cbnz	r7,$00000C3C

l00000C04:
	bl	$000085B0
	bl	$00008578
	cbz	r5,$00000C14

l00000C0E:
	ldr	r3,[r4,#&4]
	ldr	r3,[r3,#&60]
	str	r3,[r5]

l00000C14:
	ldr	r3,[r4,#&4]
	ldrb	r3,[r3,#&64]
	cmps	r3,#1
	beq	$00000C54

l00000C1E:
	mov	r5,#1
	ldr	r3,[r4,#&4]
	ldr	r1,[r3,#&60]
	bic.w	r1,r1,r6
	str	r1,[r3,#&60]

l00000C2A:
	mov	r2,#0
	ldr	r3,[r4,#&4]
	strb	r2,[r3,#&64]
	bl	$000085B0
	mov	r0,r5
	pop.w	{r4-r8,pc}

l00000C3C:
	mov	r0,r7
	bl	$0000085C
	mov	r2,#&10000000
	ldr	r3,[00000C5C]                                          ; [pc,#&14]
	str	r2,[r3]
	dsb	sy
	isb	sy
	b	$00000C04

l00000C54:
	mov	r5,#0
	b	$00000C2A
00000C58                         C4 00 00 20 04 ED 00 E0         ... ....

;; vTaskNotifyGiveFromISR: 00000C60
vTaskNotifyGiveFromISR proc
	push.w	{r3-r9,lr}
	mrs	r6,cpsr
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	mov	r3,#2
	ldrb	r5,[r0,#&64]
	strb	r3,[r0,#&64]
	ldr	r3,[r0,#&60]
	uxtb	r5,r5
	adds	r3,#1
	cmps	r5,#1
	str	r3,[r0,#&60]
	beq	$00000C96

l00000C8E:
	msr	cpsr,r6
	pop.w	{r3-r9,pc}

l00000C96:
	ldr	r7,[00000CFC]                                          ; [pc,#&64]
	mov	r8,r1
	ldr	r3,[r7,#&8C]
	mov	r4,r0
	cbz	r3,$00000CCC

l00000CA2:
	add	r1,r0,#&38
	add	r0,r7,#&58
	bl	$000082F0

l00000CAE:
	ldr	r3,[r7,#&4]
	ldr	r2,[r4,#&4C]
	ldr	r3,[r3,#&4C]
	cmps	r2,r3
	bls	$00000C8E

l00000CB8:
	mov	r3,#1
	cmp	r8,#0
	beq	$00000CF4

l00000CC0:
	str	r3,[r8]
	msr	cpsr,r6
	pop.w	{r3-r9,pc}

l00000CCC:
	add	r9,r0,#&24
	mov	r0,r9
	bl	$00008340
	ldr	r0,[r4,#&4C]
	ldr	r2,[r7,#&7C]
	lsls	r5,r0
	add	r3,r7,#8
	add.w	r0,r0,r0,lsl #2
	orrs	r5,r2
	mov	r1,r9
	add.w	r0,r3,r0,lsl #2
	str	r5,[r7,#&7C]
	bl	$000082F0
	b	$00000CAE

l00000CF4:
	str	r3,[r7,#&90]
	b	$00000C8E
00000CFA                               00 BF C4 00 00 20           ..... 

;; ulTaskNotifyTake: 00000D00
;;   Called from:
;;     00008A44 (in MPU_ulTaskNotifyTake)
ulTaskNotifyTake proc
	push	{r4-r6,lr}
	ldr	r4,[00000D64]                                          ; [pc,#&60]
	mov	r6,r0
	mov	r5,r1
	bl	$00008578
	ldr	r3,[r4,#&4]
	ldr	r3,[r3,#&60]
	cbnz	r3,$00000D1C

l00000D12:
	mov	r2,#1
	ldr	r3,[r4,#&4]
	strb	r2,[r3,#&64]
	cbnz	r5,$00000D4A

l00000D1C:
	bl	$000085B0
	bl	$00008578
	ldr	r3,[r4,#&4]
	ldr	r5,[r3,#&60]
	cbz	r5,$00000D32

l00000D2A:
	cbnz	r6,$00000D42

l00000D2C:
	ldr	r3,[r4,#&4]
	sub	r2,r5,#1
	str	r2,[r3,#&60]

l00000D32:
	mov	r2,#0
	ldr	r3,[r4,#&4]
	strb	r2,[r3,#&64]
	bl	$000085B0
	mov	r0,r5
	pop	{r4-r6,pc}

l00000D42:
	mov	r2,#0
	ldr	r3,[r4,#&4]
	str	r2,[r3,#&60]
	b	$00000D32

l00000D4A:
	mov	r0,r5
	bl	$0000085C
	mov	r2,#&10000000
	ldr	r3,[00000D68]                                          ; [pc,#&10]
	str	r2,[r3]
	dsb	sy
	isb	sy
	b	$00000D1C
00000D62       00 BF C4 00 00 20 04 ED 00 E0               ..... ....   

;; xTaskIncrementTick: 00000D6C
;;   Called from:
;;     00000EF2 (in xTaskResumeAll)
;;     000016FA (in xPortSysTickHandler)
xTaskIncrementTick proc
	push.w	{r4-r10,lr}
	ldr	r4,[00000E64]                                          ; [pc,#&F0]
	ldr	r3,[r4,#&8C]
	cmps	r3,#0
	bne	$00000E38

l00000D7A:
	ldr	r7,[r4,#&80]
	adds	r7,#1
	str	r7,[r4,#&80]
	cbnz	r7,$00000DA8

l00000D86:
	ldr	r3,[r4,#&6C]
	ldr	r2,[r4,#&70]
	str	r2,[r4,#&6C]
	str	r3,[r4,#&70]
	ldr	r3,[r4,#&94]
	adds	r3,#1
	str	r3,[r4,#&94]
	ldr	r3,[r4,#&6C]
	ldr	r3,[r3]
	cmps	r3,#0
	bne	$00000E46

l00000DA0:
	mov	r3,#&FFFFFFFF
	str	r3,[r4,#&84]

l00000DA8:
	ldr	r3,[r4,#&84]
	mov	r6,#0
	cmps	r7,r3
	blo	$00000E14

l00000DB2:
	mov	r9,#1
	ldr	r8,[00000E68]                                          ; [pc,#&B0]
	b	$00000E04

l00000DBC:
	ldr	r3,[r4,#&6C]
	ldr	r3,[r3,#&C]
	ldr	r5,[r3,#&C]
	ldr	r3,[r5,#&24]
	add	r10,r5,#&24
	cmps	r7,r3
	blo	$00000E5E

l00000DCC:
	mov	r0,r10
	bl	$00008340
	ldr	r3,[r5,#&48]
	add	r0,r5,#&38
	cbz	r3,$00000DDE

l00000DDA:
	bl	$00008340

l00000DDE:
	ldr	r0,[r5,#&4C]
	ldr	r2,[r4,#&7C]
	lsl	r3,r9,r0
	add.w	r0,r0,r0,lsl #2
	orrs	r3,r2
	mov	r1,r10
	add.w	r0,r8,r0,lsl #2
	str	r3,[r4,#&7C]
	bl	$000082F0
	ldr	r3,[r4,#&4]
	ldr	r2,[r5,#&4C]
	ldr	r3,[r3,#&4C]
	cmps	r2,r3
	it	hs
	movhs	r6,#1

l00000E04:
	ldr	r3,[r4,#&6C]
	ldr	r3,[r3]
	cmps	r3,#0
	bne	$00000DBC

l00000E0C:
	mov	r3,#&FFFFFFFF
	str	r3,[r4,#&84]

l00000E14:
	ldr	r3,[r4,#&4]
	ldr	r3,[r3,#&4C]
	add.w	r3,r3,r3,lsl #2
	add.w	r3,r4,r3,lsl #2
	ldr	r3,[r3,#&8]
	cmps	r3,#2
	it	hs
	movhs	r6,#1

l00000E28:
	ldr	r3,[r4,#&90]
	cmps	r3,#0
	it	ne
	movne	r6,#1

l00000E32:
	mov	r0,r6
	pop.w	{r4-r10,pc}

l00000E38:
	ldr	r3,[r4,#&98]
	mov	r6,#0
	adds	r3,#1
	str	r3,[r4,#&98]
	b	$00000E28

l00000E46:
	ldr	r3,[r4,#&6C]
	mov	r6,#0
	ldr	r3,[r3,#&C]
	ldr	r3,[r3,#&C]
	ldr	r3,[r3,#&24]
	str	r3,[r4,#&84]
	ldr	r3,[r4,#&84]
	cmps	r7,r3
	blo	$00000E14

l00000E5C:
	b	$00000DB2

l00000E5E:
	str	r3,[r4,#&84]
	b	$00000E14
00000E64             C4 00 00 20 CC 00 00 20                 ... ...    

;; xTaskResumeAll: 00000E6C
;;   Called from:
;;     000001F2 (in xQueueGenericSend)
;;     0000022C (in xQueueGenericSend)
;;     00000284 (in xQueueGenericSend)
;;     00000304 (in xQueueGenericReceive)
;;     00000358 (in xQueueGenericReceive)
;;     00000382 (in xQueueGenericReceive)
;;     00000F6E (in vTaskDelay)
;;     00000FA0 (in vTaskDelayUntil)
;;     00000FC8 (in vTaskDelayUntil)
;;     0000175E (in pvPortMalloc)
;;     00001768 (in pvPortMalloc)
;;     000017E8 (in xEventGroupWaitBits)
;;     00001816 (in xEventGroupWaitBits)
;;     000018E8 (in xEventGroupSetBits)
;;     0000191E (in xEventGroupSync)
;;     00001934 (in xEventGroupSync)
;;     000019CA (in vEventGroupDelete)
;;     000088E8 (in MPU_xTaskResumeAll)
xTaskResumeAll proc
	push.w	{r4-r8,lr}
	ldr	r4,[00000F40]                                          ; [pc,#&CC]
	bl	$00008578
	ldr	r3,[r4,#&8C]
	subs	r3,#1
	str	r3,[r4,#&8C]
	ldr	r5,[r4,#&8C]
	cmps	r5,#0
	bne	$00000F26

l00000E88:
	ldr	r3,[r4]
	cmps	r3,#0
	beq	$00000F26

l00000E8E:
	mov	r6,#1
	add	r7,r4,#8
	b	$00000ED4

l00000E96:
	ldr	r3,[r4,#&64]
	ldr	r5,[r3,#&C]
	add	r8,r5,#&24
	add	r0,r5,#&38
	bl	$00008340
	mov	r0,r8
	bl	$00008340
	ldr	r0,[r5,#&4C]
	ldr	r2,[r4,#&7C]
	lsl	r3,r6,r0
	add.w	r0,r0,r0,lsl #2
	orrs	r3,r2
	mov	r1,r8
	add.w	r0,r7,r0,lsl #2
	str	r3,[r4,#&7C]
	bl	$000082F0
	ldr	r3,[r4,#&4]
	ldr	r2,[r5,#&4C]
	ldr	r3,[r3,#&4C]
	cmps	r2,r3
	it	hs
	strhs	r6,[r4,#&90]

l00000ED4:
	ldr	r3,[r4,#&58]
	cmps	r3,#0
	bne	$00000E96

l00000EDA:
	cbz	r5,$00000EEA

l00000EDC:
	ldr	r3,[r4,#&6C]
	ldr	r3,[r3]
	cbnz	r3,$00000F32

l00000EE2:
	mov	r3,#&FFFFFFFF
	str	r3,[r4,#&84]

l00000EEA:
	ldr	r5,[r4,#&98]
	cbz	r5,$00000F04

l00000EF0:
	mov	r6,#1

l00000EF2:
	bl	$00000D6C
	cbz	r0,$00000EFC

l00000EF8:
	str	r6,[r4,#&90]

l00000EFC:
	subs	r5,#1
	bne	$00000EF2

l00000F00:
	str	r5,[r4,#&98]

l00000F04:
	ldr	r3,[r4,#&90]
	cbz	r3,$00000F26

l00000F0A:
	mov	r2,#&10000000
	ldr	r3,[00000F44]                                          ; [pc,#&34]
	str	r2,[r3]
	dsb	sy
	isb	sy
	mov	r4,#1
	bl	$000085B0
	mov	r0,r4
	pop.w	{r4-r8,pc}

l00000F26:
	mov	r4,#0
	bl	$000085B0
	mov	r0,r4
	pop.w	{r4-r8,pc}

l00000F32:
	ldr	r3,[r4,#&6C]
	ldr	r3,[r3,#&C]
	ldr	r3,[r3,#&C]
	ldr	r3,[r3,#&24]
	str	r3,[r4,#&84]
	b	$00000EEA
00000F40 C4 00 00 20 04 ED 00 E0                         ... ....       

;; vTaskDelay: 00000F48
;;   Called from:
;;     000088A8 (in MPU_vTaskDelay)
vTaskDelay proc
	push	{r3,lr}
	cbnz	r0,$00000F5E

l00000F4C:
	mov	r2,#&10000000
	ldr	r3,[00000F78]                                          ; [pc,#&24]
	str	r2,[r3]
	dsb	sy
	isb	sy
	pop	{r3,pc}

l00000F5E:
	ldr	r2,[00000F7C]                                          ; [pc,#&1C]
	ldr	r3,[r2,#&8C]
	adds	r3,#1
	str	r3,[r2,#&8C]
	bl	$0000085C
	bl	$00000E6C
	cmps	r0,#0
	beq	$00000F4C

l00000F76:
	pop	{r3,pc}
00000F78                         04 ED 00 E0 C4 00 00 20         ....... 

;; vTaskDelayUntil: 00000F80
;;   Called from:
;;     00008884 (in MPU_vTaskDelayUntil)
vTaskDelayUntil proc
	ldr	r2,[00000FD4]                                          ; [pc,#&50]
	push	{r4,lr}
	ldr	r4,[r2,#&8C]
	ldr	r3,[r0]
	adds	r4,#1
	str	r4,[r2,#&8C]
	ldr	r2,[r2,#&80]
	adds	r1,r3
	cmps	r2,r3
	bhs	$00000FB8

l00000F9A:
	cmps	r3,r1
	bhi	$00000FBC

l00000F9E:
	str	r1,[r0]
	bl	$00000E6C
	cbnz	r0,$00000FD0

l00000FA6:
	mov	r2,#&10000000
	ldr	r3,[00000FD8]                                          ; [pc,#&2C]
	str	r2,[r3]
	dsb	sy
	isb	sy
	pop	{r4,pc}

l00000FB8:
	cmps	r3,r1
	bhi	$00000FC0

l00000FBC:
	cmps	r2,r1
	bhs	$00000F9E

l00000FC0:
	str	r1,[r0]
	sub	r0,r1,r2
	bl	$0000085C
	bl	$00000E6C
	cmps	r0,#0
	beq	$00000FA6

l00000FD0:
	pop	{r4,pc}
00000FD2       00 BF C4 00 00 20 04 ED 00 E0               ..... ....   

;; vTaskPlaceOnEventList: 00000FDC
;;   Called from:
;;     00000222 (in xQueueGenericSend)
;;     00000378 (in xQueueGenericReceive)
vTaskPlaceOnEventList proc
	push	{r4,lr}
	mov	r4,r1
	ldr	r3,[00000FF4]                                          ; [pc,#&10]
	ldr	r1,[r3,#&4]
	adds	r1,#&38
	bl	$0000830C
	mov	r0,r4
	pop.w	{r4,lr}
	b	$0000085C
00000FF2       00 BF C4 00 00 20                           .....        

;; vTaskPlaceOnUnorderedEventList: 00000FF8
;;   Called from:
;;     00001812 (in xEventGroupWaitBits)
;;     00001930 (in xEventGroupSync)
vTaskPlaceOnUnorderedEventList proc
	push	{r3-r5,lr}
	mov	r4,r2
	ldr	r3,[00001018]                                          ; [pc,#&18]
	orr	r1,r1,#&80000000
	ldr	r5,[r3,#&4]
	ldr	r3,[r3,#&4]
	str	r1,[r5,#&38]
	add	r1,r3,#&38
	bl	$000082F0
	mov	r0,r4
	pop.w	{r3-r5,lr}
	b	$0000085C
00001018                         C4 00 00 20                     ...    

;; xTaskRemoveFromEventList: 0000101C
;;   Called from:
;;     00000082 (in prvUnlockQueue)
;;     000000C8 (in prvUnlockQueue)
;;     00000294 (in xQueueGenericSend)
;;     000003F4 (in xQueueGenericReceive)
;;     00000412 (in xQueueGenericReceive)
;;     000004B0 (in xQueueGenericSendFromISR)
;;     00000512 (in xQueueGiveFromISR)
;;     0000057C (in xQueueReceiveFromISR)
;;     00000670 (in xQueueGenericReset)
xTaskRemoveFromEventList proc
	push	{r3-r7,lr}
	ldr	r3,[r0,#&C]
	ldr	r4,[0000107C]                                          ; [pc,#&58]
	ldr	r5,[r3,#&C]
	add	r6,r5,#&38
	mov	r0,r6
	bl	$00008340
	ldr	r3,[r4,#&8C]
	cbnz	r3,$00001070

l00001034:
	add	r6,r5,#&24
	mov	r0,r6
	bl	$00008340
	mov	r3,#1
	ldr	r0,[r5,#&4C]
	ldr	r7,[r4,#&7C]
	lsls	r3,r0
	add	r2,r4,#8
	add.w	r0,r0,r0,lsl #2
	orrs	r3,r7
	mov	r1,r6
	add.w	r0,r2,r0,lsl #2
	str	r3,[r4,#&7C]
	bl	$000082F0

l0000105C:
	ldr	r3,[r4,#&4]
	ldr	r2,[r5,#&4C]
	ldr	r3,[r3,#&4C]
	cmps	r2,r3
	itte	hi
	movhi	r0,#1

l00001068:
	str	r0,[r4,#&90]
	mov	r0,#0
	pop	{r3-r7,pc}

l00001070:
	mov	r1,r6
	add	r0,r4,#&58
	bl	$000082F0
	b	$0000105C
0000107C                                     C4 00 00 20             ... 

;; xTaskRemoveFromUnorderedEventList: 00001080
;;   Called from:
;;     000018BC (in xEventGroupSetBits)
;;     000019B6 (in vEventGroupDelete)
xTaskRemoveFromUnorderedEventList proc
	push	{r3-r7,lr}
	mov	r5,#1
	ldr	r6,[r0,#&C]
	orr	r1,r1,#&80000000
	str	r1,[r0]
	add	r7,r6,#&24
	bl	$00008340
	ldr	r4,[000010D4]                                          ; [pc,#&3C]
	mov	r0,r7
	bl	$00008340
	ldr	r3,[r6,#&4C]
	ldr	lr,[r4,#&7C]
	lsl	r2,r5,r3
	add	r0,r4,#8
	add.w	r3,r3,r3,lsl #2
	add.w	r0,r0,r3,lsl #2
	orr	r2,r2,lr
	mov	r1,r7
	str	r2,[r4,#&7C]
	bl	$000082F0
	ldr	r3,[r4,#&4]
	ldr	r2,[r6,#&4C]
	ldr	r3,[r3,#&4C]
	cmps	r2,r3
	itte	hi
	movhi	r0,r5

l000010CA:
	str	r5,[r4,#&90]
	mov	r0,#0
	pop	{r3-r7,pc}
000010D2       00 BF C4 00 00 20                           .....        

;; vTaskSwitchContext: 000010D8
;;   Called from:
;;     000016A6 (in xPortPendSVHandler)
vTaskSwitchContext proc
	ldr	r2,[0000111C]                                          ; [pc,#&40]
	ldr	r3,[r2,#&8C]
	cbnz	r3,$00001112

l000010E0:
	str	r3,[r2,#&90]
	ldr	r3,[r2,#&7C]
	clz	r3,r3
	uxtb	r3,r3
	rsb	r3,r3,#&1F
	add.w	r3,r3,r3,lsl #2
	lsls	r3,r3,#2
	add	r0,r2,r3
	mov	r3,r0
	ldr	r1,[r0,#&C]
	adds	r3,#&10
	ldr	r1,[r1,#&4]
	cmps	r1,r3
	str	r1,[r0,#&C]
	it	eq
	ldreq	r1,[r1,#&4]

l00001108:
	ldr	r3,[r1,#&C]
	it	eq
	streq	r1,[r0,#&C]

l0000110E:
	str	r3,[r2,#&4]
	bx	lr

l00001112:
	mov	r3,#1
	str	r3,[r2,#&90]
	bx	lr
0000111A                               00 BF C4 00 00 20           ..... 

;; uxTaskResetEventItemValue: 00001120
;;   Called from:
;;     0000182C (in xEventGroupWaitBits)
;;     0000194A (in xEventGroupSync)
uxTaskResetEventItemValue proc
	ldr	r3,[00001134]                                          ; [pc,#&10]
	ldr	r1,[r3,#&4]
	ldr	r2,[r3,#&4]
	ldr	r3,[r3,#&4]
	ldr	r0,[r1,#&38]
	ldr	r3,[r3,#&4C]
	rsb	r3,r3,#2
	str	r3,[r2,#&38]
	bx	lr
00001134             C4 00 00 20                             ...        

;; xTaskGetCurrentTaskHandle: 00001138
;;   Called from:
;;     000005DC (in xQueueTakeMutexRecursive)
;;     0000060A (in xQueueGiveMutexRecursive)
xTaskGetCurrentTaskHandle proc
	ldr	r3,[00001140]                                          ; [pc,#&4]
	ldr	r0,[r3,#&4]
	bx	lr
0000113E                                           00 BF               ..
00001140 C4 00 00 20                                     ...            

;; vTaskSetTimeOutState: 00001144
;;   Called from:
;;     00000212 (in xQueueGenericSend)
;;     0000039E (in xQueueGenericReceive)
;;     00008980 (in MPU_vTaskSetTimeOutState)
vTaskSetTimeOutState proc
	ldr	r3,[00001154]                                          ; [pc,#&C]
	ldr	r2,[r3,#&94]
	ldr	r3,[r3,#&80]
	stm	r0,{r2-r3}
	bx	lr
00001154             C4 00 00 20                             ...        

;; xTaskCheckForTimeOut: 00001158
;;   Called from:
;;     000001D4 (in xQueueGenericSend)
;;     0000034A (in xQueueGenericReceive)
;;     000089A8 (in MPU_xTaskCheckForTimeOut)
xTaskCheckForTimeOut proc
	push	{r4-r6,lr}
	mov	r4,r0
	mov	r6,r1
	bl	$00008578
	ldr	r3,[000011A8]                                          ; [pc,#&44]
	ldr	r1,[r4]
	ldr	r5,[r3,#&80]
	ldr	r2,[r3,#&94]
	ldr	r0,[r4,#&4]
	cmps	r1,r2
	beq	$00001178

l00001174:
	cmps	r5,r0
	bhs	$0000119C

l00001178:
	ldr	r2,[r6]
	sub	r1,r5,r0
	cmps	r1,r2
	bhs	$0000119C

l00001180:
	sub	r2,r2,r5
	mov	r5,#0
	ldr	r1,[r3,#&94]
	ldr	r3,[r3,#&80]
	adds	r2,r0
	str	r2,[r6]
	stm	r4,{r1,r3}
	bl	$000085B0
	mov	r0,r5
	pop	{r4-r6,pc}

l0000119C:
	mov	r5,#1
	bl	$000085B0
	mov	r0,r5
	pop	{r4-r6,pc}
000011A6                   00 BF C4 00 00 20                   .....    

;; vTaskMissedYield: 000011AC
;;   Called from:
;;     0000008C (in prvUnlockQueue)
;;     000000D2 (in prvUnlockQueue)
vTaskMissedYield proc
	mov	r2,#1
	ldr	r3,[000011B8]                                          ; [pc,#&8]
	str	r2,[r3,#&90]
	bx	lr
000011B6                   00 BF C4 00 00 20                   .....    

;; vTaskPriorityInherit: 000011BC
;;   Called from:
;;     000003DE (in xQueueGenericReceive)
vTaskPriorityInherit proc
	cmps	r0,#0
	beq	$00001246

l000011C0:
	push	{r3-r7,lr}
	ldr	r4,[00001248]                                          ; [pc,#&84]
	ldr	r3,[r0,#&4C]
	ldr	r2,[r4,#&4]
	ldr	r2,[r2,#&4C]
	cmps	r3,r2
	bhs	$000011F4

l000011CE:
	ldr	r2,[r0,#&38]
	cmps	r2,#0
	blt	$000011DE

l000011D4:
	ldr	r2,[r4,#&4]
	ldr	r2,[r2,#&4C]
	rsb	r2,r2,#2
	str	r2,[r0,#&38]

l000011DE:
	ldr	r5,[0000124C]                                          ; [pc,#&6C]
	add.w	r3,r3,r3,lsl #2
	ldr	r2,[r0,#&34]
	add.w	r3,r5,r3,lsl #2
	cmps	r2,r3
	beq	$000011F6

l000011EE:
	ldr	r3,[r4,#&4]
	ldr	r3,[r3,#&4C]
	str	r3,[r0,#&4C]

l000011F4:
	pop	{r3-r7,pc}

l000011F6:
	add	r7,r0,#&24
	mov	r6,r0
	mov	r0,r7
	bl	$00008340
	cbnz	r0,$00001220

l00001204:
	ldr	r2,[r6,#&4C]
	add.w	r3,r2,r2,lsl #2
	add.w	r3,r4,r3,lsl #2
	ldr	r3,[r3,#&8]
	cbnz	r3,$00001220

l00001212:
	mov	r1,#1
	ldr	r3,[r4,#&7C]
	lsl	r2,r1,r2
	bic.w	r2,r3,r2
	str	r2,[r4,#&7C]

l00001220:
	mov	r3,#1
	ldr	r2,[r4,#&4]
	ldr	lr,[r4,#&7C]
	ldr	r2,[r2,#&4C]
	mov	r1,r7
	lsls	r3,r2
	orr	r3,r3,lr
	add.w	r0,r2,r2,lsl #2
	str	r2,[r6,#&4C]
	add.w	r0,r5,r0,lsl #2
	str	r3,[r4,#&7C]
	pop.w	{r3-r7,lr}
	b	$000082F0

l00001246:
	bx	lr
00001248                         C4 00 00 20 CC 00 00 20         ... ... 

;; xTaskPriorityDisinherit: 00001250
;;   Called from:
;;     00000162 (in prvCopyDataToQueue)
xTaskPriorityDisinherit proc
	cmps	r0,#0
	beq	$000012C8

l00001254:
	push	{r3-r7,lr}
	ldr	r1,[r0,#&4C]
	ldr	r3,[r0,#&5C]
	ldr	r2,[r0,#&58]
	subs	r3,#1
	cmps	r1,r2
	str	r3,[r0,#&5C]
	beq	$00001266

l00001264:
	cbz	r3,$0000126A

l00001266:
	mov	r0,#0
	pop	{r3-r7,pc}

l0000126A:
	add	r7,r0,#&24
	mov	r4,r0
	mov	r0,r7
	bl	$00008340
	cbnz	r0,$00001298

l00001278:
	ldr	r1,[r4,#&4C]
	ldr	r2,[000012CC]                                          ; [pc,#&50]
	add.w	r3,r1,r1,lsl #2
	add.w	r3,r2,r3,lsl #2
	ldr	r3,[r3,#&8]
	cbnz	r3,$0000129A

l00001288:
	mov	r0,#1
	ldr	r3,[r2,#&7C]
	lsl	r1,r0,r1
	bic.w	r1,r3,r1
	str	r1,[r2,#&7C]
	b	$0000129A

l00001298:
	ldr	r2,[000012CC]                                          ; [pc,#&30]

l0000129A:
	mov	r5,#1
	ldr	r3,[r4,#&58]
	ldr	lr,[r2,#&7C]
	ldr	r0,[000012D0]                                          ; [pc,#&2C]
	lsl	r6,r5,r3
	mov	r1,r7
	str	r3,[r4,#&4C]
	rsb	r7,r3,#2
	add.w	r3,r3,r3,lsl #2
	orr	r6,r6,lr
	add.w	r0,r0,r3,lsl #2
	str	r7,[r4,#&38]
	str	r6,[r2,#&7C]
	bl	$000082F0
	mov	r0,r5
	pop	{r3-r7,pc}

l000012C8:
	mov	r0,#0
	bx	lr
000012CC                                     C4 00 00 20             ... 
000012D0 CC 00 00 20                                     ...            

;; pvTaskIncrementMutexHeldCount: 000012D4
;;   Called from:
;;     0000041C (in xQueueGenericReceive)
pvTaskIncrementMutexHeldCount proc
	ldr	r3,[000012E8]                                          ; [pc,#&10]
	ldr	r2,[r3,#&4]
	cbz	r2,$000012E2

l000012DA:
	ldr	r1,[r3,#&4]
	ldr	r2,[r1,#&5C]
	adds	r2,#1
	str	r2,[r1,#&5C]

l000012E2:
	ldr	r0,[r3,#&4]
	bx	lr
000012E6                   00 BF C4 00 00 20 00 00 00 00       ..... ....

;; prvRestoreContextOfFirstTask: 000012F0
;;   Called from:
;;     0000135E (in prvSVCHandler)
prvRestoreContextOfFirstTask proc
	ldr	r0,[00001724]                                          ; [pc,#&430]
	ldr	r0,[r0]
	ldr	r0,[r0]
	msr	cpsr,r0
	ldr	r3,[00001330]                                          ; [pc,#&30]
	ldr	r1,[r3]
	ldr	r0,[r1]
	add	r1,r1,#4
	ldr	r2,[00001728]                                          ; [pc,#&420]
	ldm	r1!,{r4-fp}
	stm	r2!,{r4-fp}
	ldm	r0!,{r3-fp}
	msr	cpsr,r3
	msr	cpsr,r0
	mov	r0,#0
	msr	cpsr,r0
	mvn	lr,#2
	bx	lr
0000132C                                     AF F3 00 80             ....
00001330 C8 00 00 20                                     ...            

;; prvSVCHandler: 00001334
;;   Called from:
;;     00001722 (in vPortSVCHandler)
prvSVCHandler proc
	ldr	r3,[r0,#&18]
	ldrb.w	r3,[r3,-#&2]
	cmps	r3,#1
	beq	$00001360

l0000133E:
	blo	$00001354

l00001340:
	cmps	r3,#2
	bne	$00001352

l00001344:
	mrs	r1,cpsr
	bic	r1,r1,#1
	msr	cpsr,r1
	bx	lr

l00001352:
	bx	lr

l00001354:
	ldr	r2,[00001374]                                          ; [pc,#&1C]
	ldr	r3,[r2]
	orr	r3,r3,#&BE000000
	str	r3,[r2]
	b	$000012F0

l00001360:
	mov	r2,#&10000000
	ldr	r3,[00001378]                                          ; [pc,#&10]
	str	r2,[r3]
	dsb	sy
	isb	sy
	bx	lr
00001372       00 BF 1C ED 00 E0 04 ED 00 E0               ..........   

;; pxPortInitialiseStack: 0000137C
;;   Called from:
;;     00000786 (in prvInitialiseNewTask)
pxPortInitialiseStack proc
	cmps	r3,#1
	push	{r4-r5}
	it	eq
	moveq	r3,#2

l00001384:
	mov	r5,#&1000000
	mov	r4,#0
	it	ne
	movne	r3,#3

l00001390:
	str.w	r2,[r0,-#&20]
	bic	r1,r1,#1
	sub	r2,r0,#&44
	stmdb	r0,{r1,r5}
	str.w	r4,[r0,-#&C]
	str.w	r3,[r0,-#&44]
	pop	{r4-r5}
	mov	r0,r2
	bx	lr
000013AE                                           00 BF               ..

;; xPortStartScheduler: 000013B0
;;   Called from:
;;     000009DC (in vTaskStartScheduler)
xPortStartScheduler proc
	ldr	r3,[000014E8]                                          ; [pc,#&134]
	push	{r4-r6}
	ldr	r2,[r3]
	ldr	r1,[000014EC]                                          ; [pc,#&134]
	orr	r2,r2,#&FF0000
	str	r2,[r3]
	ldr	r2,[r3]
	orr	r2,r2,#&FF000000
	str	r2,[r3]
	ldr	r3,[r1]
	cmp	r3,#&800
	beq	$00001400

l000013CE:
	mov	r5,#&4E1F
	mov	r1,#7
	mov	r0,#0
	ldr	r4,[000014F0]                                          ; [pc,#&118]
	ldr	r2,[000014F4]                                          ; [pc,#&118]
	ldr	r3,[000014F8]                                          ; [pc,#&11C]
	str	r5,[r4]
	str	r1,[r2]
	str	r0,[r3]
	ldr	r0,[00001724]                                          ; [pc,#&340]
	ldr	r0,[r0]
	ldr	r0,[r0]
	msr	cpsr,r0
	cps	#0
	cps	#0
	dsb	sy
	isb	sy
	svc	#0
	nop
	pop	{r4-r6}
	bx	lr

l00001400:
	ldr	r0,[000014FC]                                          ; [pc,#&F8]
	ldr	r1,[00001500]                                          ; [pc,#&FC]
	ldr	r3,[00001504]                                          ; [pc,#&FC]
	sub	r1,r1,r0
	orr	r2,r0,#&10
	cmps	r1,#&20
	str	r2,[r3]
	bls	$000014DE

l00001412:
	mov	r3,#&40
	mov	r2,#5
	b	$0000141E

l00001418:
	adds	r2,#1
	cmps	r2,#&1F
	beq	$000014CE

l0000141E:
	cmps	r1,r3
	mov.w	r3,r3,lsl #1
	bhi	$00001418

l00001426:
	ldr	r3,[00001508]                                          ; [pc,#&E0]
	orr	r2,r3,r2,lsl #1

l0000142C:
	ldr	r1,[0000150C]                                          ; [pc,#&DC]
	ldr	r4,[00001510]                                          ; [pc,#&E0]
	sub	r1,r1,r0
	ldr	r3,[00001504]                                          ; [pc,#&D0]
	orr	r0,r0,#&11
	cmps	r1,#&20
	str	r2,[r4]
	str	r0,[r3]
	bls	$000014DA

l00001440:
	mov	r3,#&40
	mov	r2,#5
	b	$0000144C

l00001446:
	adds	r2,#1
	cmps	r2,#&1F
	beq	$000014D2

l0000144C:
	cmps	r1,r3
	mov.w	r3,r3,lsl #1
	bhi	$00001446

l00001454:
	ldr	r3,[00001514]                                          ; [pc,#&BC]
	orr	r2,r3,r2,lsl #1

l0000145A:
	ldr	r3,[00001518]                                          ; [pc,#&BC]
	ldr	r1,[0000151C]                                          ; [pc,#&BC]
	ldr	r5,[00001510]                                          ; [pc,#&B0]
	ldr	r0,[00001504]                                          ; [pc,#&A0]
	sub	r1,r1,r3
	orr	r4,r3,#&12
	cmps	r1,#&20
	str	r2,[r5]
	str	r4,[r0]
	bls	$000014E2

l00001470:
	mov	r3,#&40
	mov	r2,#5
	b	$0000147C

l00001476:
	adds	r2,#1
	cmps	r2,#&1F
	beq	$000014D6

l0000147C:
	cmps	r1,r3
	mov.w	r3,r3,lsl #1
	bhi	$00001476

l00001484:
	ldr	r0,[00001520]                                          ; [pc,#&98]
	orr	r0,r0,r2,lsl #1

l0000148A:
	mov	r3,#5
	mov	r2,#&40
	ldr	r6,[00001510]                                          ; [pc,#&80]
	ldr	r4,[00001504]                                          ; [pc,#&70]
	ldr	r5,[00001524]                                          ; [pc,#&90]
	ldr	r1,[00001528]                                          ; [pc,#&90]
	str	r0,[r6]
	str	r5,[r4]

l0000149A:
	adds	r3,#1
	cmps	r3,#&1F
	mov.w	r2,r2,lsl #1
	beq	$000014CA

l000014A4:
	cmps	r2,r1
	bls	$0000149A

l000014A8:
	ldr	r2,[0000152C]                                          ; [pc,#&80]
	orr	r3,r2,r3,lsl #1

l000014AE:
	ldr	r2,[00001510]                                          ; [pc,#&60]
	ldr	r1,[00001530]                                          ; [pc,#&7C]
	str	r3,[r2]
	ldr	r3,[r1]
	orr	r3,r3,#&10000
	str	r3,[r1]
	ldr.w	r3,[r2,-#&C]
	orr	r3,r3,#5
	str.w	r3,[r2,-#&C]
	b	$000013CE

l000014CA:
	ldr	r3,[00001534]                                          ; [pc,#&68]
	b	$000014AE

l000014CE:
	ldr	r2,[00001538]                                          ; [pc,#&68]
	b	$0000142C

l000014D2:
	ldr	r2,[0000153C]                                          ; [pc,#&68]
	b	$0000145A

l000014D6:
	ldr	r0,[00001540]                                          ; [pc,#&68]
	b	$0000148A

l000014DA:
	ldr	r2,[00001544]                                          ; [pc,#&68]
	b	$0000145A

l000014DE:
	ldr	r2,[00001548]                                          ; [pc,#&68]
	b	$0000142C

l000014E2:
	ldr	r0,[0000154C]                                          ; [pc,#&68]
	b	$0000148A
000014E6                   00 BF 20 ED 00 E0 90 ED 00 E0       .. .......
000014F0 14 E0 00 E0 10 E0 00 E0 BC 00 00 20 00 00 00 00 ........... ....
00001500 00 00 02 00 9C ED 00 E0 01 00 07 06 00 80 00 00 ................
00001510 A0 ED 00 E0 01 00 07 05 00 00 00 20 00 02 00 20 ........... ... 
00001520 01 00 07 01 13 00 00 40 FE FF FF 1F 01 00 00 13 .......@........
00001530 24 ED 00 E0 3F 00 00 13 3F 00 07 06 3F 00 07 05 $...?...?...?...
00001540 3F 00 07 01 09 00 07 05 09 00 07 06 09 00 07 01 ?...............

;; vPortEndScheduler: 00001550
;;   Called from:
;;     00000A02 (in vTaskEndScheduler)
vPortEndScheduler proc
	bx	lr
00001552       00 BF                                       ..           

;; vPortStoreTaskMPUSettings: 00001554
;;   Called from:
;;     00000774 (in prvInitialiseNewTask)
;;     00000978 (in vTaskAllocateMPURegions)
;;     00000986 (in vTaskAllocateMPURegions)
vPortStoreTaskMPUSettings proc
	push	{r4-r5}
	cmps	r1,#0
	beq	$000015DE

l0000155A:
	cbnz	r3,$000015B0

l0000155C:
	mov	r5,#5

l0000155E:
	ldr	r4,[r1,#&4]
	cbz	r4,$000015A2

l00001562:
	ldr	r3,[r1]
	orr	r2,r5,#&10
	orrs	r3,r2
	cmps	r4,#&20
	str	r3,[r0,#&8]
	bls	$00001650

l00001570:
	mov	r2,#&40
	mov	r3,#5
	b	$0000157C

l00001576:
	adds	r3,#1
	cmps	r3,#&1F
	beq	$000015AC

l0000157C:
	cmps	r4,r2
	mov.w	r2,r2,lsl #1
	bhi	$00001576

l00001584:
	lsls	r3,r3,#1

l00001586:
	ldr	r2,[r1,#&8]
	orr	r2,r2,#1
	orrs	r3,r2
	str	r3,[r0,#&C]

l00001590:
	adds	r5,#1
	cmps	r5,#8
	add	r1,r1,#&C
	add	r0,r0,#8
	bne	$0000155E

l0000159E:
	pop	{r4-r5}
	bx	lr

l000015A2:
	orr	r3,r5,#&10
	str	r4,[r0,#&C]
	str	r3,[r0,#&8]
	b	$00001590

l000015AC:
	mov	r3,#&3E
	b	$00001586

l000015B0:
	lsls	r3,r3,#2
	orr	r2,r2,#&14
	cmps	r3,#&20
	str	r2,[r0]
	bls	$00001654

l000015BC:
	mov	r2,#&40
	mov	r4,#5
	b	$000015C8

l000015C2:
	adds	r4,#1
	cmps	r4,#&1F
	beq	$000015DA

l000015C8:
	cmps	r3,r2
	mov.w	r2,r2,lsl #1
	bhi	$000015C2

l000015D0:
	ldr	r3,[00001660]                                          ; [pc,#&8C]
	orr	r4,r3,r4,lsl #1

l000015D6:
	str	r4,[r0,#&4]
	b	$0000155C

l000015DA:
	ldr	r4,[00001664]                                          ; [pc,#&88]
	b	$000015D6

l000015DE:
	ldr	r3,[00001668]                                          ; [pc,#&88]
	ldr	r1,[0000166C]                                          ; [pc,#&88]
	orr	r2,r3,#&14
	sub	r1,r1,r3
	cmps	r1,#&20
	str	r2,[r0]
	bls	$0000165C

l000015EE:
	mov	r3,#&40
	mov	r2,#5
	b	$000015FA

l000015F4:
	adds	r2,#1
	cmps	r2,#&1F
	beq	$00001648

l000015FA:
	cmps	r3,r1
	mov.w	r3,r3,lsl #1
	blo	$000015F4

l00001602:
	ldr	r3,[00001660]                                          ; [pc,#&5C]
	orr	r2,r3,r2,lsl #1

l00001608:
	ldr	r3,[00001670]                                          ; [pc,#&64]
	ldr	r1,[00001674]                                          ; [pc,#&68]
	orr	r4,r3,#&15
	sub	r1,r1,r3
	cmps	r1,#&20
	str	r2,[r0,#&4]
	str	r4,[r0,#&8]
	bls	$00001658

l0000161A:
	mov	r2,#5
	mov	r3,#&40
	b	$00001626

l00001620:
	adds	r2,#1
	cmps	r2,#&1F
	beq	$0000164C

l00001626:
	cmps	r1,r3
	mov.w	r3,r3,lsl #1
	bhi	$00001620

l0000162E:
	ldr	r3,[00001678]                                          ; [pc,#&48]
	orr	r2,r3,r2,lsl #1

l00001634:
	mov	r4,#&16
	mov	r3,#0
	mov	r1,#&17
	str	r4,[r0,#&10]
	str	r2,[r0,#&C]
	str	r3,[r0,#&14]
	str	r3,[r0,#&1C]
	str	r1,[r0,#&18]
	pop	{r4-r5}
	bx	lr

l00001648:
	ldr	r2,[00001664]                                          ; [pc,#&18]
	b	$00001608

l0000164C:
	ldr	r2,[0000167C]                                          ; [pc,#&2C]
	b	$00001634

l00001650:
	mov	r3,#8
	b	$00001586

l00001654:
	ldr	r4,[00001680]                                          ; [pc,#&28]
	b	$000015D6

l00001658:
	ldr	r2,[00001684]                                          ; [pc,#&28]
	b	$00001634

l0000165C:
	ldr	r2,[00001680]                                          ; [pc,#&20]
	b	$00001608
00001660 01 00 07 03 3F 00 07 03 00 00 00 20 00 20 00 20 ....?...... . . 
00001670 00 00 00 20 00 02 00 20 01 00 07 01 3F 00 07 01 ... ... ....?...
00001680 09 00 07 03 09 00 07 01                         ........       

;; xPortPendSVHandler: 00001688
xPortPendSVHandler proc
	mrs	r0,cpsr
	ldr	r3,[000016E0]                                          ; [pc,#&50]
	ldr	r2,[r3]
	mrs	r1,cpsr
	stmdb	r0!,{r1,r4-fp}
	str	r0,[r2]
	push.w	{r3,lr}
	mov	r0,#&BF
	msr	cpsr,r0
	bl	$000010D8
	mov	r0,#0
	msr	cpsr,r0
	pop.w	{r3,lr}
	ldr	r1,[r3]
	ldr	r0,[r1]
	add	r1,r1,#4
	ldr	r2,[00001728]                                          ; [pc,#&68]
	ldm	r1!,{r4-fp}
	stm	r2!,{r4-fp}
	ldm	r0!,{r3-fp}
	msr	cpsr,r3
	msr	cpsr,r0
	bx	lr
000016D6                   00 BF AF F3 00 80 AF F3 00 80       ..........
000016E0 C8 00 00 20                                     ...            

;; xPortSysTickHandler: 000016E4
xPortSysTickHandler proc
	push	{r4,lr}
	mrs	r4,cpsr
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	bl	$00000D6C
	cbz	r0,$00001708

l00001700:
	mov	r2,#&10000000
	ldr	r3,[00001710]                                          ; [pc,#&8]
	str	r2,[r3]

l00001708:
	msr	cpsr,r4
	pop	{r4,pc}
0000170E                                           00 BF               ..
00001710 04 ED 00 E0                                     ....           

;; vPortSVCHandler: 00001714
vPortSVCHandler proc
	tst	lr,#4
	ite	eq
	mrseq	r0,cpsr

l0000171E:
	mrs	r0,cpsr
	b	$00001334
00001724             08 ED 00 E0 9C ED 00 E0                 ........   

;; pvPortMalloc: 0000172C
;;   Called from:
;;     000006B8 (in xQueueGenericCreate)
;;     000008C4 (in xTaskCreate)
;;     000008CE (in xTaskCreate)
;;     0000092A (in xTaskCreateRestricted)
;;     000017AC (in xEventGroupCreate)
;;     00008CA4 (in MPU_pvPortMalloc)
;;     00008E4C (in xCoRoutineCreate)
pvPortMalloc proc
	push	{r4,lr}
	mov	r4,r0
	lsls	r3,r0,#&1D
	itt	ne
	bicne	r4,r0,#7

l00001738:
	adds	r4,#8
	bl	$00000A0C
	ldr	r3,[0000177C]                                          ; [pc,#&3C]
	ldr	r2,[r3]
	cbz	r2,$00001770

l00001744:
	mov	r1,#&5B3
	ldr	r2,[r3,#&5C0]
	adds	r4,r2
	cmps	r4,r1
	bhi	$00001766

l00001752:
	cmps	r2,r4
	bhs	$00001766

l00001756:
	ldr	r1,[r3]
	str	r4,[r3,#&5C0]
	add	r4,r1,r2
	bl	$00000E6C
	mov	r0,r4
	pop	{r4,pc}

l00001766:
	mov	r4,#0
	bl	$00000E6C
	mov	r0,r4
	pop	{r4,pc}

l00001770:
	add	r2,r3,#&C
	bic	r2,r2,#7
	str	r2,[r3]
	b	$00001744
0000177C                                     30 02 00 20             0.. 

;; vPortFree: 00001780
;;   Called from:
;;     00000454 (in vQueueDelete)
;;     00000910 (in xTaskCreate)
;;     000019C2 (in vEventGroupDelete)
;;     00008CCC (in MPU_vPortFree)
vPortFree proc
	bx	lr
00001782       00 BF                                       ..           

;; vPortInitialiseBlocks: 00001784
;;   Called from:
;;     00008CEC (in MPU_vPortInitialiseBlocks)
vPortInitialiseBlocks proc
	mov	r2,#0
	ldr	r3,[00001790]                                          ; [pc,#&8]
	str	r2,[r3,#&5C0]
	bx	lr
0000178E                                           00 BF               ..
00001790 30 02 00 20                                     0..            

;; xPortGetFreeHeapSize: 00001794
;;   Called from:
;;     00008D0C (in MPU_xPortGetFreeHeapSize)
xPortGetFreeHeapSize proc
	ldr	r3,[000017A4]                                          ; [pc,#&C]
	ldr	r0,[r3,#&5C0]
	rsb	r0,r0,#&5B0
	adds	r0,#4
	bx	lr
000017A2       00 BF 30 02 00 20                           ..0..        

;; xEventGroupCreate: 000017A8
;;   Called from:
;;     00008D30 (in MPU_xEventGroupCreate)
xEventGroupCreate proc
	push	{r4,lr}
	mov	r0,#&18
	bl	$0000172C
	mov	r4,r0
	cbz	r0,$000017BE

l000017B4:
	mov	r3,#0
	str	r3,[r0],#&4
	bl	$000082D0

l000017BE:
	mov	r0,r4
	pop	{r4,pc}
000017C2       00 BF                                       ..           

;; xEventGroupWaitBits: 000017C4
;;   Called from:
;;     00008D6C (in MPU_xEventGroupWaitBits)
xEventGroupWaitBits proc
	push.w	{r4-r8,lr}
	mov	r6,r0
	mov	r7,r3
	mov	r5,r1
	mov	r8,r2
	bl	$00000A0C
	ldr	r4,[r6]
	cbnz	r7,$000017F2

l000017D8:
	adcs	r4,r5
	beq	$000017F8

l000017DC:
	cmp	r8,#0
	beq	$000017E8

l000017E2:
	bic.w	r5,r4,r5
	str	r5,[r6]

l000017E8:
	bl	$00000E6C
	mov	r0,r4
	pop.w	{r4-r8,pc}

l000017F2:
	bics.w	r3,r5,r4
	beq	$000017DC

l000017F8:
	ldr	r3,[sp,#&18]
	cmps	r3,#0
	beq	$000017E8

l000017FE:
	cmp	r8,#0
	ite	eq
	moveq	r1,#0

l00001806:
	mov	r1,#&1000000
	cbnz	r7,$0000183E

l0000180C:
	orrs	r1,r5
	ldr	r2,[sp,#&18]
	add	r0,r6,#4
	bl	$00000FF8
	bl	$00000E6C
	cbnz	r0,$0000182C

l0000181C:
	mov	r2,#&10000000
	ldr	r3,[00001870]                                          ; [pc,#&4C]
	str	r2,[r3]
	dsb	sy
	isb	sy

l0000182C:
	bl	$00001120
	lsls	r3,r0,#6
	mov	r4,r0
	bpl	$00001844

l00001836:
	bic	r0,r4,#&FF000000
	pop.w	{r4-r8,pc}

l0000183E:
	orr	r1,r1,#&4000000
	b	$0000180C

l00001844:
	bl	$00008578
	ldr	r4,[r6]
	cbnz	r7,$00001868

l0000184C:
	adcs	r5,r4
	beq	$0000185C

l00001850:
	cmp	r8,#0
	beq	$0000185C

l00001856:
	bic.w	r5,r4,r5
	str	r5,[r6]

l0000185C:
	bl	$000085B0
	bic	r0,r4,#&FF000000
	pop.w	{r4-r8,pc}

l00001868:
	bics.w	r3,r5,r4
	bne	$0000185C

l0000186E:
	b	$00001850
00001870 04 ED 00 E0                                     ....           

;; xEventGroupClearBits: 00001874
;;   Called from:
;;     00008D9C (in MPU_xEventGroupClearBits)
xEventGroupClearBits proc
	push	{r4-r6,lr}
	mov	r6,r0
	mov	r4,r1
	bl	$00008578
	ldr	r5,[r6]
	bic.w	r4,r5,r4
	str	r4,[r6]
	bl	$000085B0
	mov	r0,r5
	pop	{r4-r6,pc}
0000188E                                           00 BF               ..

;; xEventGroupSetBits: 00001890
;;   Called from:
;;     00001910 (in xEventGroupSync)
;;     000019D0 (in vEventGroupSetBitsCallback)
;;     00008DC8 (in MPU_xEventGroupSetBits)
xEventGroupSetBits proc
	push	{r3-r7,lr}
	mov	r5,r0
	mov	r4,r1
	bl	$00000A0C
	ldr	r1,[r5]
	ldr	r0,[r5,#&10]
	add	r6,r5,#&C
	orrs	r1,r4
	cmps	r6,r0
	str	r1,[r5]
	beq	$000018F0

l000018AA:
	mov	r7,#0
	b	$000018C8

l000018AE:
	adcs	r2,r1
	beq	$000018C2

l000018B2:
	lsls	r3,r3,#7
	bpl	$000018B8

l000018B6:
	orrs	r7,r2

l000018B8:
	orr	r1,r1,#&2000000
	bl	$00001080
	ldr	r1,[r5]

l000018C2:
	cmps	r6,r4
	mov	r0,r4
	beq	$000018E2

l000018C8:
	ldm	r0,{r3-r4}
	tst	r3,#&4000000
	bic	r2,r3,#&FF000000
	beq	$000018AE

l000018D6:
	bics.w	lr,r2,r1
	beq	$000018B2

l000018DC:
	cmps	r6,r4
	mov	r0,r4
	bne	$000018C8

l000018E2:
	mvns	r7,r7

l000018E4:
	ands	r1,r7
	str	r1,[r5]
	bl	$00000E6C
	ldr	r0,[r5]
	pop	{r3-r7,pc}

l000018F0:
	mov	r7,#&FFFFFFFF
	b	$000018E4
000018F6                   00 BF                               ..       

;; xEventGroupSync: 000018F8
;;   Called from:
;;     00008DFE (in MPU_xEventGroupSync)
xEventGroupSync proc
	push.w	{r4-r8,lr}
	mov	r8,r1
	mov	r5,r0
	mov	r6,r2
	mov	r7,r3
	bl	$00000A0C
	mov	r1,r8
	ldr	r4,[r5]
	mov	r0,r5
	orrs	r4,r1
	bl	$00001890
	bics.w	r3,r6,r4
	beq	$0000195E

l0000191A:
	cbnz	r7,$00001928

l0000191C:
	ldr	r4,[r5]

l0000191E:
	bl	$00000E6C
	mov	r0,r4
	pop.w	{r4-r8,pc}

l00001928:
	mov	r2,r7
	orr	r1,r6,#&5000000
	add	r0,r5,#4
	bl	$00000FF8
	bl	$00000E6C
	cbnz	r0,$0000194A

l0000193A:
	mov	r2,#&10000000
	ldr	r3,[00001984]                                          ; [pc,#&44]
	str	r2,[r3]
	dsb	sy
	isb	sy

l0000194A:
	bl	$00001120
	lsls	r3,r0,#6
	mov	r4,r0
	bpl	$00001968

l00001954:
	bic	r4,r4,#&FF000000

l00001958:
	mov	r0,r4
	pop.w	{r4-r8,pc}

l0000195E:
	ldr	r3,[r5]
	bic.w	r6,r3,r6
	str	r6,[r5]
	b	$0000191E

l00001968:
	bl	$00008578
	ldr	r4,[r5]
	bics.w	r3,r6,r4
	itt	eq
	biceq.w	r6,r4,r6

l00001978:
	str	r6,[r5]
	bl	$000085B0
	bic	r4,r4,#&FF000000
	b	$00001958
00001984             04 ED 00 E0                             ....       

;; xEventGroupGetBitsFromISR: 00001988
xEventGroupGetBitsFromISR proc
	mrs	r3,cpsr
	mov	r2,#&BF
	msr	cpsr,r2
	isb	sy
	dsb	sy
	msr	cpsr,r3
	ldr	r0,[r0]
	bx	lr

;; vEventGroupDelete: 000019A4
;;   Called from:
;;     00008E28 (in MPU_vEventGroupDelete)
vEventGroupDelete proc
	push	{r4,lr}
	mov	r4,r0
	bl	$00000A0C
	ldr	r3,[r4,#&4]
	cbz	r3,$000019C0

l000019B0:
	mov	r1,#&2000000
	ldr	r0,[r4,#&10]
	bl	$00001080
	ldr	r3,[r4,#&4]
	cmps	r3,#0
	bne	$000019B0

l000019C0:
	mov	r0,r4
	bl	$00001780
	pop.w	{r4,lr}
	b	$00000E6C
000019CE                                           00 BF               ..

;; vEventGroupSetBitsCallback: 000019D0
vEventGroupSetBitsCallback proc
	b	$00001890

;; vEventGroupClearBitsCallback: 000019D4
vEventGroupClearBitsCallback proc
	push	{r3-r5,lr}
	mov	r4,r0
	mov	r5,r1
	bl	$00008578
	ldr	r3,[r4]
	bic.w	r3,r3,r5
	str	r3,[r4]
	pop.w	{r3-r5,lr}
	b	$000085B0
000019EE                                           00 BF               ..
000019F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
