;;; Segment .text (00008000)

;; NmiSR: 00008000
NmiSR proc
	b	$00008000
00008002       00 BF                                       ..           

;; FaultISR: 00008004
FaultISR proc
	b	$00008004
00008006                   00 BF                               ..       

;; ResetISR: 00008008
ResetISR proc
	ldr	r3,[0000802C]                                          ; [pc,#&20]
	ldr	r0,[00008030]                                          ; [pc,#&24]
	cmps	r3,r0
	bhs	$00008026

l00008010:
	mvns	r2,r3
	mov	r1,#0
	adds	r2,r0
	bic	r2,r2,#3
	adds	r2,#4
	adds	r2,r3

l0000801E:
	str	r1,[r3],#&4
	cmps	r3,r2
	bne	$0000801E

l00008026:
	b	$000080A0
0000802A                               00 BF 60 01 00 20           ..`.. 
00008030 80 08 00 20                                     ...            

;; raise: 00008034
raise proc
	b	$00008034
00008036                   00 BF                               ..       

;; vPrintTask: 00008038
vPrintTask proc
	push	{r4-r5,lr}
	mov	r4,#0
	ldr	r5,[00008064]                                          ; [pc,#&24]
	sub	sp,#&C

l00008040:
	add	r1,sp,#4
	adds	r4,#1
	mov	r3,#0
	mov	r2,#&FFFFFFFF
	ldr	r0,[r5]
	bl	$00008B6C
	bl	$00009780
	and	r2,r4,#1
	and	r1,r4,#&3F
	ldr	r0,[sp,#&4]
	bl	$000097CC
	b	$00008040
00008064             80 08 00 20                             ...        

;; vCheckTask: 00008068
vCheckTask proc
	push	{r4-r5,lr}
	ldr	r3,[00008098]                                          ; [pc,#&2C]
	sub	sp,#&C
	str	r3,[sp,#&4]
	bl	$00008904
	add	r4,sp,#8
	ldr	r5,[0000809C]                                          ; [pc,#&24]
	str	r0,[r4,-#&8]!

l0000807C:
	mov	r0,r4
	mov	r1,#&1388
	bl	$00008874
	mov	r3,#0
	mov	r2,#&FFFFFFFF
	add	r1,sp,#4
	ldr	r0,[r5]
	bl	$00008AE4
	b	$0000807C
00008096                   00 BF 50 A2 00 00 80 08 00 20       ..P...... 

;; Main: 000080A0
;;   Called from:
;;     00008026 (in ResetISR)
Main proc
	push	{lr}
	mov	r2,#0
	sub	sp,#&C
	mov	r1,#4
	mov	r0,#3
	mov	r4,#0
	bl	$00008A88
	ldr	r3,[000080F0]                                          ; [pc,#&3C]
	str	r0,[r3]
	mov	r0,r4
	bl	$000098F0
	mov	r2,#3
	mov	r3,r4
	str	r2,[sp]
	ldr	r1,[000080F4]                                          ; [pc,#&30]
	mov	r2,#&3B
	str	r4,[sp,#&4]
	ldr	r0,[000080F8]                                          ; [pc,#&30]
	bl	$00008808
	mov	r2,#2
	ldr	r1,[000080FC]                                          ; [pc,#&2C]
	mov	r3,r4
	str	r2,[sp]
	str	r4,[sp,#&4]
	mov	r2,#&3B
	ldr	r0,[00008100]                                          ; [pc,#&24]
	bl	$00008808
	bl	$00000990
	mov	r2,r4
	mov	r1,r4
	ldr	r0,[00008104]                                          ; [pc,#&1C]
	bl	$000097CC

l000080EC:
	b	$000080EC
000080EE                                           00 BF               ..
000080F0 80 08 00 20 58 A2 00 00 69 80 00 00 60 A2 00 00 ... X...i...`...
00008100 39 80 00 00 68 A2 00 00                         9...h...       

;; vUART_ISR: 00008108
vUART_ISR proc
	push	{r4-r6,lr}
	mov	r6,#0
	ldr	r5,[00008174]                                          ; [pc,#&64]
	sub	sp,#8
	mov	r1,#1
	mov	r0,r5
	str	r6,[sp,#&4]
	bl	$0000A0CC
	mov	r4,r0
	mov	r1,r0
	mov	r0,r5
	bl	$0000A0D8
	lsls	r2,r4,#&1B
	bpl	$00008130

l00008128:
	ldr	r3,[00008178]                                          ; [pc,#&4C]
	ldr	r3,[r3]
	lsls	r3,r3,#&19
	bmi	$0000815E

l00008130:
	lsls	r0,r4,#&1A
	bpl	$0000813C

l00008134:
	ldr	r2,[0000817C]                                          ; [pc,#&44]
	ldrb	r3,[r2]
	cmps	r3,#&7A
	bls	$0000814C

l0000813C:
	ldr	r3,[sp,#&4]
	cbz	r3,$00008148

l00008140:
	mov	r2,#&10000000
	ldr	r3,[00008180]                                          ; [pc,#&38]
	str	r2,[r3]

l00008148:
	add	sp,#8
	pop	{r4-r6,pc}

l0000814C:
	ldr	r1,[00008178]                                          ; [pc,#&28]
	ldr	r1,[r1]
	lsls	r1,r1,#&1A
	itt	pl
	ldrpl	r1,[00008174]                                        ; [pc,#&1C]

l00008156:
	str	r3,[r1]
	adds	r3,#1
	strb	r3,[r2]
	b	$0000813C

l0000815E:
	ldr	r5,[r5]
	mov	r3,r6
	mov	r0,r6
	add	r2,sp,#4
	add	r0,sp,#3
	strb	r5,[sp,#&3]
	bl	$00000458
	b	$00008130
00008174             00 C0 00 40 18 C0 00 40 2C 02 00 20     ...@...@,.. 
00008180 04 ED 00 E0                                     ....           

;; vSetErrorLED: 00008184
;;   Called from:
;;     00008204 (in prvSetAndCheckRegisters)
vSetErrorLED proc
	mov	r1,#1
	mov	r0,#7
	b	$000085F4

;; prvSetAndCheckRegisters: 0000818C
;;   Called from:
;;     00008216 (in vApplicationIdleHook)
prvSetAndCheckRegisters proc
	mov	fp,#&A
	add	r0,fp,#1
	add	r1,fp,#2
	add	r2,fp,#3
	add	r3,fp,#4
	add	r4,fp,#5
	add	r5,fp,#6
	add	r6,fp,#7
	add	r7,fp,#8
	add	r8,fp,#9
	add	r9,fp,#&A
	add	r10,fp,#&B
	add	ip,fp,#&C
	cmp	fp,#&A
	bne	$00008200

l000081C6:
	cmps	r0,#&B
	bne	$00008200

l000081CA:
	cmps	r1,#&C
	bne	$00008200

l000081CE:
	cmps	r2,#&D
	bne	$00008200

l000081D2:
	cmps	r3,#&E
	bne	$00008200

l000081D6:
	cmps	r4,#&F
	bne	$00008200

l000081DA:
	cmps	r5,#&10
	bne	$00008200

l000081DE:
	cmps	r6,#&11
	bne	$00008200

l000081E2:
	cmps	r7,#&12
	bne	$00008200

l000081E6:
	cmp	r8,#&13
	bne	$00008200

l000081EC:
	cmp	r9,#&14
	bne	$00008200

l000081F2:
	cmp	r10,#&15
	bne	$00008200

l000081F8:
	cmp	ip,#&16
	bne	$00008200

l000081FE:
	bx	lr

l00008200:
	push	{lr}
	ldr	r1,[0000821C]                                          ; [pc,#&18]
	blx	r1
	pop	lr
	bx	lr
0000820C                                     70 47 00 BF             pG..

;; vApplicationIdleHook: 00008210
;;   Called from:
;;     0000852E (in prvIdleTask)
vApplicationIdleHook proc
	push	{r3,lr}

l00008212:
	bl	$00008F2C
	bl	$0000818C
	b	$00008212
0000821C                                     85 81 00 00             ....

;; PDCInit: 00008220
;;   Called from:
;;     000085DE (in vParTestInitialise)
PDCInit proc
	push	{r4-r5,lr}
	ldr	r0,[0000828C]                                          ; [pc,#&68]
	sub	sp,#&C
	bl	$00009B7C
	ldr	r0,[00008290]                                          ; [pc,#&64]
	bl	$00009B7C
	mov	r2,#2
	mov	r1,#&34
	mov	r0,#&40004000
	bl	$0000910C
	mov	r2,#1
	mov	r1,#8
	mov	r0,#&40004000
	bl	$0000910C
	mov	r3,#&A
	mov	r2,#2
	mov	r1,#4
	mov	r0,#&40004000
	bl	$000091C8
	mov	r4,#8
	mov	r2,#0
	ldr	r5,[00008294]                                          ; [pc,#&38]
	mov	r1,r2
	ldr	r3,[00008298]                                          ; [pc,#&38]
	mov	r0,r5
	str	r4,[sp]
	bl	$000099E8
	mov	r0,r5
	bl	$00009A34
	mov	r1,r4
	mov	r2,#0
	mov	r0,#&40004000
	bl	$00009454
	mov	r2,r4
	mov	r1,r4
	mov	r0,#&40004000
	add	sp,#&C
	pop.w	{r4-r5,lr}
	b	$00009454
0000828C                                     10 00 00 10             ....
00008290 01 00 00 20 00 80 00 40 40 42 0F 00             ... ...@@B..   

;; PDCWrite: 0000829C
;;   Called from:
;;     000085EC (in vParTestInitialise)
;;     00008618 (in vParTestSetLED)
;;     00008656 (in vParTestToggleLED)
PDCWrite proc
	push	{r4-r5,lr}
	mov	r5,r1
	ldr	r4,[000082CC]                                          ; [pc,#&28]
	sub	sp,#&C
	and	r1,r0,#&F
	mov	r0,r4
	bl	$00009A98
	mov	r1,r5
	mov	r0,r4
	bl	$00009A98
	mov	r0,r4
	add	r1,sp,#4
	bl	$00009AB8
	add	r1,sp,#4
	mov	r0,r4
	bl	$00009AB8
	add	sp,#&C
	pop	{r4-r5,pc}
000082CA                               00 BF 00 80 00 40           .....@

;; vListInitialise: 000082D0
;;   Called from:
;;     00000694 (in xQueueGenericReset)
;;     0000069C (in xQueueGenericReset)
;;     00000820 (in prvAddNewTaskToReadyList)
;;     0000082C (in prvAddNewTaskToReadyList)
;;     00000836 (in prvAddNewTaskToReadyList)
;;     0000083C (in prvAddNewTaskToReadyList)
;;     00000844 (in prvAddNewTaskToReadyList)
;;     000017BA (in xEventGroupCreate)
;;     00008EB4 (in xCoRoutineCreate)
;;     00008EC0 (in xCoRoutineCreate)
;;     00008ECA (in xCoRoutineCreate)
;;     00008ED0 (in xCoRoutineCreate)
;;     00008ED8 (in xCoRoutineCreate)
vListInitialise proc
	mov	r1,#&FFFFFFFF
	mov	r2,#0
	add	r3,r0,#8
	str	r1,[r0,#&8]
	stm	r0,{r2-r3}
	str	r3,[r0,#&C]
	str	r3,[r0,#&10]
	bx	lr
000082E6                   00 BF                               ..       

;; vListInitialiseItem: 000082E8
;;   Called from:
;;     00000756 (in prvInitialiseNewTask)
;;     0000075E (in prvInitialiseNewTask)
;;     00008E78 (in xCoRoutineCreate)
;;     00008E80 (in xCoRoutineCreate)
vListInitialiseItem proc
	mov	r3,#0
	str	r3,[r0,#&10]
	bx	lr
000082EE                                           00 BF               ..

;; vListInsertEnd: 000082F0
;;   Called from:
;;     000007D6 (in prvAddNewTaskToReadyList)
;;     00000AD0 (in xTaskGenericNotify)
;;     00000B76 (in xTaskGenericNotifyFromISR)
;;     00000BBE (in xTaskGenericNotifyFromISR)
;;     00000CAA (in vTaskNotifyGiveFromISR)
;;     00000CEE (in vTaskNotifyGiveFromISR)
;;     00000DF4 (in xTaskIncrementTick)
;;     00000EC2 (in xTaskResumeAll)
;;     0000100C (in vTaskPlaceOnUnorderedEventList)
;;     00001058 (in xTaskRemoveFromEventList)
;;     00001076 (in xTaskRemoveFromEventList)
;;     000010BA (in xTaskRemoveFromUnorderedEventList)
;;     00001242 (in vTaskPriorityInherit)
;;     000012C0 (in xTaskPriorityDisinherit)
;;     00008EA2 (in xCoRoutineCreate)
;;     00008F78 (in vCoRoutineSchedule)
;;     00009000 (in vCoRoutineSchedule)
;;     000090AC (in xCoRoutineRemoveFromEventList)
vListInsertEnd proc
	ldm	r0,{r2-r3}
	push	{r4}
	ldr	r4,[r3,#&8]
	adds	r2,#1
	str	r4,[r1,#&8]
	ldr	r4,[r3,#&8]
	str	r3,[r1,#&4]
	str	r1,[r4,#&4]
	str	r1,[r3,#&8]
	pop	{r4}
	str	r0,[r1,#&10]
	str	r2,[r0]
	bx	lr

;; vListInsert: 0000830C
;;   Called from:
;;     00000890 (in prvAddCurrentTaskToDelayedList.isra.0)
;;     000008AC (in prvAddCurrentTaskToDelayedList.isra.0)
;;     00000FE6 (in vTaskPlaceOnEventList)
;;     00008F12 (in vCoRoutineAddToDelayedList)
;;     00008F22 (in vCoRoutineAddToDelayedList)
vListInsert proc
	push	{r4-r5}
	ldr	r5,[r1]
	add	r3,r5,#1
	beq	$00008338

l00008314:
	add	r2,r0,#8
	b	$0000831C

l0000831A:
	mov	r2,r3

l0000831C:
	ldr	r3,[r2,#&4]
	ldr	r4,[r3]
	cmps	r5,r4
	bhs	$0000831A

l00008324:
	ldr	r4,[r0]
	str	r3,[r1,#&4]
	adds	r4,#1
	str	r1,[r3,#&8]
	str	r2,[r1,#&8]
	str	r1,[r2,#&4]
	str	r0,[r1,#&10]
	str	r4,[r0]
	pop	{r4-r5}
	bx	lr

l00008338:
	ldr	r2,[r0,#&10]
	ldr	r3,[r2,#&4]
	b	$00008324
0000833E                                           00 BF               ..

;; uxListRemove: 00008340
;;   Called from:
;;     0000086A (in prvAddCurrentTaskToDelayedList.isra.0)
;;     00000AAE (in xTaskGenericNotify)
;;     00000BA2 (in xTaskGenericNotifyFromISR)
;;     00000CD2 (in vTaskNotifyGiveFromISR)
;;     00000DCE (in xTaskIncrementTick)
;;     00000DDA (in xTaskIncrementTick)
;;     00000EA2 (in xTaskResumeAll)
;;     00000EA8 (in xTaskResumeAll)
;;     0000102A (in xTaskRemoveFromEventList)
;;     0000103A (in xTaskRemoveFromEventList)
;;     00001090 (in xTaskRemoveFromUnorderedEventList)
;;     00001098 (in xTaskRemoveFromUnorderedEventList)
;;     000011FE (in vTaskPriorityInherit)
;;     00001272 (in xTaskPriorityDisinherit)
;;     00008EFE (in vCoRoutineAddToDelayedList)
;;     00008F54 (in vCoRoutineSchedule)
;;     00008F60 (in vCoRoutineSchedule)
;;     00008FD8 (in vCoRoutineSchedule)
;;     00008FE4 (in vCoRoutineSchedule)
;;     000090A2 (in xCoRoutineRemoveFromEventList)
uxListRemove proc
	ldr	r2,[r0,#&10]
	ldr	r3,[r0,#&4]
	ldr	r1,[r0,#&8]
	push	{r4}
	str	r1,[r3,#&8]
	ldr	r4,[r2,#&4]
	ldr	r1,[r0,#&8]
	cmps	r0,r4
	str	r3,[r1,#&4]
	it	eq
	streq	r1,[r2,#&4]

l00008356:
	mov	r1,#0
	ldr	r3,[r2]
	str	r1,[r0,#&10]
	sub	r0,r3,#1
	str	r0,[r2]
	pop	{r4}
	bx	lr

;; xQueueCRSend: 00008364
;;   Called from:
;;     00008724 (in prvFixedDelayCoRoutine)
;;     00008758 (in prvFixedDelayCoRoutine)
xQueueCRSend proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r4,r2
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	bl	$00008578
	ldr	r2,[r5,#&38]
	ldr	r3,[r5,#&3C]
	cmps	r2,r3
	beq	$000083B2

l00008388:
	bl	$000085B0
	mov	r0,#0
	msr	cpsr,r0
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r2,[r5,#&38]
	ldr	r3,[r5,#&3C]
	cmps	r2,r3
	blo	$000083C0

l000083AA:
	mov	r3,#0
	msr	cpsr,r3
	pop	{r4-r6,pc}

l000083B2:
	bl	$000085B0
	cbnz	r4,$000083D8

l000083B8:
	msr	cpsr,r4
	mov	r0,r4
	pop	{r4-r6,pc}

l000083C0:
	mov	r2,r0
	mov	r1,r6
	mov	r0,r5
	bl	$000000EC
	ldr	r3,[r5,#&24]
	cbnz	r3,$000083EE

l000083CE:
	mov	r0,#1
	mov	r3,#0
	msr	cpsr,r3
	pop	{r4-r6,pc}

l000083D8:
	add	r1,r5,#&10
	mov	r0,r4
	bl	$00008EF0
	mov	r3,#0
	msr	cpsr,r3
	mvn	r0,#3
	pop	{r4-r6,pc}

l000083EE:
	add	r0,r5,#&24
	bl	$00009094
	cmps	r0,#0
	beq	$000083CE

l000083FA:
	mvn	r0,#4
	b	$000083AA

;; xQueueCRReceive: 00008400
;;   Called from:
;;     0000869E (in prvFlashCoRoutine)
;;     000086C0 (in prvFlashCoRoutine)
xQueueCRReceive proc
	push	{r3-r5,lr}
	mov	r4,r0
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r5,[r0,#&38]
	cbnz	r5,$00008424

l00008418:
	cmps	r2,#0
	bne	$0000848A

l0000841C:
	msr	cpsr,r2
	mov	r0,r2
	pop	{r3-r5,pc}

l00008424:
	mov	r3,#0
	msr	cpsr,r3
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r2,[r0,#&38]
	cbnz	r2,$00008448

l0000843E:
	mov	r0,r2

l00008440:
	mov	r3,#0
	msr	cpsr,r3
	pop	{r3-r5,pc}

l00008448:
	mov	r0,r1
	ldr	r2,[r4,#&40]
	ldr	r1,[r4,#&C]
	ldr	r3,[r4,#&4]
	adds	r1,r2
	cmps	r1,r3
	ldr	r3,[r4,#&38]
	str	r1,[r4,#&C]
	it	hs
	ldrhs	r1,[r4]

l0000845C:
	add	r3,r3,#&FFFFFFFF
	str	r3,[r4,#&38]
	it	hs
	strhs	r1,[r4,#&C]

l00008466:
	bl	$0000A5C4
	ldr	r3,[r4,#&10]
	cbnz	r3,$00008478

l0000846E:
	mov	r0,#1
	mov	r3,#0
	msr	cpsr,r3
	pop	{r3-r5,pc}

l00008478:
	add	r0,r4,#&10
	bl	$00009094
	cmps	r0,#0
	beq	$0000846E

l00008484:
	mvn	r0,#4
	b	$00008440

l0000848A:
	add	r1,r0,#&24
	mov	r0,r2
	bl	$00008EF0
	msr	cpsr,r5
	mvn	r0,#3
	pop	{r3-r5,pc}
0000849E                                           00 BF               ..

;; xQueueCRSendFromISR: 000084A0
xQueueCRSendFromISR proc
	push	{r4-r6,lr}
	ldr	r3,[r0,#&3C]
	ldr	r6,[r0,#&38]
	mov	r5,r2
	cmps	r6,r3
	blo	$000084B0

l000084AC:
	mov	r0,r5
	pop	{r4-r6,pc}

l000084B0:
	mov	r2,#0
	mov	r4,r0
	bl	$000000EC
	cmps	r5,#0
	bne	$000084AC

l000084BC:
	ldr	r3,[r4,#&24]
	cmps	r3,#0
	beq	$000084AC

l000084C2:
	add	r0,r4,#&24
	bl	$00009094
	add	r5,r0,#0
	it	ne
	movne	r5,#1

l000084D0:
	b	$000084AC
000084D2       00 BF                                       ..           

;; xQueueCRReceiveFromISR: 000084D4
xQueueCRReceiveFromISR proc
	push	{r3-r7,lr}
	ldr	r3,[r0,#&38]
	cbz	r3,$00008514

l000084DA:
	ldr	r3,[r0,#&C]
	ldr	lr,[r0,#&40]
	ldr	r4,[r0,#&4]
	adds	r3,lr
	cmps	r3,r4
	mov	r6,r1
	mov	r4,r0
	mov	r5,r2
	ldr	r7,[r0,#&38]
	str	r3,[r0,#&C]
	it	hs
	ldrhs	r3,[r0]

l000084F4:
	add	r7,r7,#&FFFFFFFF
	it	hs
	strhs	r3,[r0,#&C]

l000084FC:
	mov	r1,r3
	mov	r2,lr
	mov	r0,r6
	str	r7,[r4,#&38]
	bl	$0000A5C4
	ldr	r3,[r5]
	cbnz	r3,$00008510

l0000850C:
	ldr	r3,[r4,#&10]
	cbnz	r3,$00008518

l00008510:
	mov	r0,#1
	pop	{r3-r7,pc}

l00008514:
	mov	r0,r3
	pop	{r3-r7,pc}

l00008518:
	add	r0,r4,#&10
	bl	$00009094
	cmps	r0,#0
	beq	$00008510

l00008524:
	mov	r0,#1
	str	r0,[r5]
	pop	{r3-r7,pc}
0000852A                               00 BF                       ..   

;; prvIdleTask: 0000852C
prvIdleTask proc
	push	{r3,lr}

l0000852E:
	bl	$00008210
	b	$0000852E

;; xTaskNotifyStateClear: 00008534
;;   Called from:
;;     00008A6C (in MPU_xTaskNotifyStateClear)
xTaskNotifyStateClear proc
	push	{r3-r5,lr}
	cbz	r0,$00008558

l00008538:
	mov	r4,r0

l0000853A:
	bl	$00008578
	ldrb	r3,[r4,#&64]
	cmps	r3,#2
	ittet	eq
	moveq	r3,#0

l00008548:
	mov	r5,#1
	mov	r5,#0
	strb	r3,[r4,#&64]
	bl	$000085B0
	mov	r0,r5
	pop	{r3-r5,pc}

l00008558:
	ldr	r3,[00008560]                                          ; [pc,#&4]
	ldr	r4,[r3,#&4]
	b	$0000853A
0000855E                                           00 BF               ..
00008560 C4 00 00 20                                     ...            

;; xPortRaisePrivilege: 00008564
;;   Called from:
;;     0000857A (in vPortEnterCritical)
;;     000085B2 (in vPortExitCritical)
;;     000087E2 (in MPU_xTaskCreateRestricted)
;;     0000881A (in MPU_xTaskCreate)
;;     00008852 (in MPU_vTaskAllocateMPURegions)
;;     0000887A (in MPU_vTaskDelayUntil)
;;     000088A0 (in MPU_vTaskDelay)
;;     000088C2 (in MPU_vTaskSuspendAll)
;;     000088E2 (in MPU_xTaskResumeAll)
;;     00008906 (in MPU_xTaskGetTickCount)
;;     0000892A (in MPU_uxTaskGetNumberOfTasks)
;;     00008950 (in MPU_pcTaskGetName)
;;     00008978 (in MPU_vTaskSetTimeOutState)
;;     0000899E (in MPU_xTaskCheckForTimeOut)
;;     000089D0 (in MPU_xTaskGenericNotify)
;;     00008A08 (in MPU_xTaskNotifyWait)
;;     00008A3A (in MPU_ulTaskNotifyTake)
;;     00008A64 (in MPU_xTaskNotifyStateClear)
;;     00008A90 (in MPU_xQueueGenericCreate)
;;     00008ABE (in MPU_xQueueGenericReset)
;;     00008AF0 (in MPU_xQueueGenericSend)
;;     00008B20 (in MPU_uxQueueMessagesWaiting)
;;     00008B48 (in MPU_uxQueueSpacesAvailable)
;;     00008B78 (in MPU_xQueueGenericReceive)
;;     00008BAA (in MPU_xQueuePeekFromISR)
;;     00008BD4 (in MPU_xQueueGetMutexHolder)
;;     00008BFC (in MPU_xQueueCreateMutex)
;;     00008C26 (in MPU_xQueueTakeMutexRecursive)
;;     00008C50 (in MPU_xQueueGiveMutexRecursive)
;;     00008C78 (in MPU_vQueueDelete)
;;     00008C9C (in MPU_pvPortMalloc)
;;     00008CC4 (in MPU_vPortFree)
;;     00008CE6 (in MPU_vPortInitialiseBlocks)
;;     00008D06 (in MPU_xPortGetFreeHeapSize)
;;     00008D2A (in MPU_xEventGroupCreate)
;;     00008D5C (in MPU_xEventGroupWaitBits)
;;     00008D92 (in MPU_xEventGroupClearBits)
;;     00008DBE (in MPU_xEventGroupSetBits)
;;     00008DF0 (in MPU_xEventGroupSync)
;;     00008E20 (in MPU_vEventGroupDelete)
xPortRaisePrivilege proc
	mrs	r0,cpsr
	tst	r0,#1
	itte	ne
	movne	r0,#0

l00008570:
	svc	#2
	mov	r0,#1
	bx	lr
00008576                   00 20                               .        

;; vPortEnterCritical: 00008578
;;   Called from:
;;     0000005C (in prvUnlockQueue)
;;     000000A2 (in prvUnlockQueue)
;;     000001B0 (in xQueueGenericSend)
;;     000001DC (in xQueueGenericSend)
;;     000001F8 (in xQueueGenericSend)
;;     000002F0 (in xQueueGenericReceive)
;;     0000030A (in xQueueGenericReceive)
;;     00000326 (in xQueueGenericReceive)
;;     0000035C (in xQueueGenericReceive)
;;     000003D8 (in xQueueGenericReceive)
;;     0000042C (in uxQueueMessagesWaiting)
;;     00000440 (in uxQueueSpacesAvailable)
;;     000005B8 (in xQueueGetMutexHolder)
;;     00000638 (in xQueueGenericReset)
;;     000007A0 (in prvAddNewTaskToReadyList)
;;     00000A62 (in xTaskGenericNotify)
;;     00000BE2 (in xTaskNotifyWait)
;;     00000C08 (in xTaskNotifyWait)
;;     00000D08 (in ulTaskNotifyTake)
;;     00000D20 (in ulTaskNotifyTake)
;;     00000E72 (in xTaskResumeAll)
;;     0000115E (in xTaskCheckForTimeOut)
;;     00001844 (in xEventGroupWaitBits)
;;     0000187A (in xEventGroupClearBits)
;;     00001968 (in xEventGroupSync)
;;     000019DA (in vEventGroupClearBitsCallback)
;;     0000837C (in xQueueCRSend)
;;     0000853A (in xTaskNotifyStateClear)
vPortEnterCritical proc
	push	{r3,lr}
	bl	$00008564
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r2,[000085AC]                                          ; [pc,#&1C]
	cmps	r0,#1
	ldr	r3,[r2]
	add	r3,r3,#1
	str	r3,[r2]
	beq	$000085A8

l0000859C:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l000085A8:
	pop	{r3,pc}
000085AA                               00 BF BC 00 00 20           ..... 

;; vPortExitCritical: 000085B0
;;   Called from:
;;     0000009E (in prvUnlockQueue)
;;     000000E8 (in prvUnlockQueue)
;;     000001A8 (in xQueueGenericSend)
;;     000001CC (in xQueueGenericSend)
;;     000001E8 (in xQueueGenericSend)
;;     00000218 (in xQueueGenericSend)
;;     00000266 (in xQueueGenericSend)
;;     00000272 (in xQueueGenericSend)
;;     000002FA (in xQueueGenericReceive)
;;     0000031E (in xQueueGenericReceive)
;;     00000342 (in xQueueGenericReceive)
;;     00000364 (in xQueueGenericReceive)
;;     0000036A (in xQueueGenericReceive)
;;     000003A4 (in xQueueGenericReceive)
;;     000003CC (in xQueueGenericReceive)
;;     000003E2 (in xQueueGenericReceive)
;;     00000432 (in uxQueueMessagesWaiting)
;;     0000044A (in uxQueueSpacesAvailable)
;;     000005C2 (in xQueueGetMutexHolder)
;;     000005CC (in xQueueGetMutexHolder)
;;     00000664 (in xQueueGenericReset)
;;     00000688 (in xQueueGenericReset)
;;     000006A0 (in xQueueGenericReset)
;;     000007DA (in prvAddNewTaskToReadyList)
;;     00000A92 (in xTaskGenericNotify)
;;     00000AEE (in xTaskGenericNotify)
;;     00000C04 (in xTaskNotifyWait)
;;     00000C32 (in xTaskNotifyWait)
;;     00000D1C (in ulTaskNotifyTake)
;;     00000D3A (in ulTaskNotifyTake)
;;     00000F1C (in xTaskResumeAll)
;;     00000F28 (in xTaskResumeAll)
;;     00001194 (in xTaskCheckForTimeOut)
;;     0000119E (in xTaskCheckForTimeOut)
;;     0000185C (in xEventGroupWaitBits)
;;     00001886 (in xEventGroupClearBits)
;;     0000197A (in xEventGroupSync)
;;     000019EA (in vEventGroupClearBitsCallback)
;;     00008388 (in xQueueCRSend)
;;     000083B2 (in xQueueCRSend)
;;     00008550 (in xTaskNotifyStateClear)
vPortExitCritical proc
	push	{r3,lr}
	bl	$00008564
	ldr	r2,[000085D8]                                          ; [pc,#&20]
	ldr	r3,[r2]
	subs	r3,#1
	str	r3,[r2]
	cbnz	r3,$000085C4

l000085C0:
	msr	cpsr,r3

l000085C4:
	cmps	r0,#1
	beq	$000085D4

l000085C8:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l000085D4:
	pop	{r3,pc}
000085D6                   00 BF BC 00 00 20                   .....    

;; vParTestInitialise: 000085DC
vParTestInitialise proc
	push	{r3,lr}
	bl	$00008220
	ldr	r3,[000085F0]                                          ; [pc,#&C]
	mov	r0,#5
	ldrb	r1,[r3]
	pop.w	{r3,lr}
	b	$0000829C
000085F0 F4 07 00 20                                     ...            

;; vParTestSetLED: 000085F4
;;   Called from:
;;     00008188 (in vSetErrorLED)
vParTestSetLED proc
	push	{r3-r5,lr}
	mov	r4,r0
	mov	r5,r1
	bl	$000088C0
	cmps	r4,#7
	bhi	$0000861C

l00008602:
	mov	r3,#1
	lsl	r0,r3,r4
	ldr	r3,[0000862C]                                          ; [pc,#&20]
	uxtb	r0,r0
	ldrb	r2,[r3]
	cbz	r5,$00008624

l00008610:
	orrs	r0,r2
	strb	r0,[r3]

l00008614:
	ldrb	r1,[r3]
	mov	r0,#5
	bl	$0000829C

l0000861C:
	pop.w	{r3-r5,lr}
	b	$000088E0

l00008624:
	bic.w	r0,r2,r0
	strb	r0,[r3]
	b	$00008614
0000862C                                     F4 07 00 20             ... 

;; vParTestToggleLED: 00008630
;;   Called from:
;;     00008692 (in prvFlashCoRoutine)
vParTestToggleLED proc
	push	{r4,lr}
	mov	r4,r0
	bl	$000088C0
	cmps	r4,#7
	bhi	$0000865A

l0000863C:
	mov	r2,#1
	ldr	r3,[0000866C]                                          ; [pc,#&2C]
	lsl	r0,r2,r4
	ldrb	r1,[r3]
	uxtb	r2,r0
	adcs	r2,r1
	bne	$00008662

l0000864C:
	ldrb	r1,[r3]
	orrs	r2,r1
	strb	r2,[r3]

l00008652:
	ldrb	r1,[r3]
	mov	r0,#5
	bl	$0000829C

l0000865A:
	pop.w	{r4,lr}
	b	$000088E0

l00008662:
	ldrb	r2,[r3]
	bic.w	r0,r2,r0
	strb	r0,[r3]
	b	$00008652
0000866C                                     F4 07 00 20             ... 

;; prvFlashCoRoutine: 00008670
prvFlashCoRoutine proc
	push	{r4-r6,lr}
	ldrh	r3,[r0,#&34]
	sub	sp,#8
	cmp	r3,#&1C2
	mov	r4,r0
	beq	$000086B6

l0000867E:
	mov	r2,#&1C3
	cmps	r3,r2
	beq	$0000868C

l00008686:
	cbz	r3,$000086D2

l00008688:
	add	sp,#8
	pop	{r4-r6,pc}

l0000868C:
	ldr	r5,[000086E0]                                          ; [pc,#&50]
	add	r6,sp,#4

l00008690:
	ldr	r0,[sp,#&4]
	bl	$00008630

l00008696:
	mov	r2,#&FFFFFFFF
	mov	r1,r6
	ldr	r0,[r5]
	bl	$00008400
	add	r2,r0,#4
	beq	$000086D8

l000086A6:
	add	r3,r0,#5
	beq	$000086C8

l000086AA:
	cmps	r0,#1
	beq	$00008690

l000086AE:
	mov	r2,#0
	ldr	r3,[000086E4]                                          ; [pc,#&30]
	str	r2,[r3]
	b	$00008696

l000086B6:
	ldr	r5,[000086E0]                                          ; [pc,#&28]
	add	r6,sp,#4
	ldr	r0,[r5]
	mov	r1,r6
	mov	r2,#0
	bl	$00008400
	add	r3,r0,#5
	bne	$000086AA

l000086C8:
	mov	r3,#&1C3
	strh	r3,[r4,#&34]
	add	sp,#8
	pop	{r4-r6,pc}

l000086D2:
	ldr	r5,[000086E0]                                          ; [pc,#&C]
	add	r6,sp,#4
	b	$00008696

l000086D8:
	mov	r3,#&1C2
	strh	r3,[r4,#&34]
	b	$00008688
000086E0 F8 07 00 20 C0 00 00 20                         ... ...        

;; prvFixedDelayCoRoutine: 000086E8
prvFixedDelayCoRoutine proc
	push	{r4,lr}
	ldrh	r3,[r0,#&34]
	sub	sp,#8
	cmp	r3,#&182
	mov	r4,r0
	str	r1,[sp,#&4]
	beq	$00008750

l000086F8:
	bls	$00008748

l000086FA:
	mov	r2,#&183
	cmps	r3,r2
	bne	$00008716

l00008702:
	ldr	r3,[00008778]                                          ; [pc,#&74]
	ldr	r2,[sp,#&4]
	ldr.w	r0,[r3,r2,lsl #2]
	cbnz	r0,$0000875E

l0000870C:
	mov	r3,#&196
	strh	r3,[r4,#&34]

l00008712:
	add	sp,#8
	pop	{r4,pc}

l00008716:
	cmp	r3,#&196
	bne	$00008712

l0000871C:
	ldr	r3,[0000877C]                                          ; [pc,#&5C]
	mov	r2,#0
	ldr	r0,[r3]
	add	r1,sp,#4
	bl	$00008364
	add	r2,r0,#4
	beq	$0000876E

l0000872C:
	add	r3,r0,#5
	beq	$00008766

l00008730:
	cmps	r0,#1
	beq	$00008702

l00008734:
	mov	r2,#0
	ldr	r3,[00008780]                                          ; [pc,#&48]
	str	r2,[r3]
	ldr	r3,[00008778]                                          ; [pc,#&3C]
	ldr	r2,[sp,#&4]
	ldr.w	r0,[r3,r2,lsl #2]
	cmps	r0,#0
	beq	$0000870C

l00008746:
	b	$0000875E

l00008748:
	cmps	r3,#0
	beq	$0000871C

l0000874C:
	add	sp,#8
	pop	{r4,pc}

l00008750:
	ldr	r3,[0000877C]                                          ; [pc,#&28]
	mov	r2,#0
	ldr	r0,[r3]
	add	r1,sp,#4
	bl	$00008364
	b	$0000872C

l0000875E:
	mov	r1,#0
	bl	$00008EF0
	b	$0000870C

l00008766:
	mov	r3,#&183
	strh	r3,[r4,#&34]
	b	$00008712

l0000876E:
	mov	r3,#&182
	strh	r3,[r4,#&34]
	b	$00008712
00008776                   00 BF 84 A2 00 00 F8 07 00 20       ......... 
00008780 C0 00 00 20                                     ...            

;; vStartFlashCoRoutines: 00008784
vStartFlashCoRoutines proc
	cmps	r0,#8
	it	hs
	movhs	r0,#8

l0000878A:
	push	{r4-r6,lr}
	mov	r2,#0
	mov	r5,r0
	mov	r1,#4
	mov	r0,#1
	bl	$00008A88
	ldr	r3,[000087C4]                                          ; [pc,#&28]
	str	r0,[r3]
	cbz	r0,$000087C2

l0000879E:
	cbz	r5,$000087B4

l000087A0:
	mov	r4,#0
	ldr	r6,[000087C8]                                          ; [pc,#&24]

l000087A4:
	mov	r2,r4
	mov	r1,#0
	adds	r4,#1
	mov	r0,r6
	bl	$00008E40
	cmps	r4,r5
	bne	$000087A4

l000087B4:
	mov	r2,#0
	pop.w	{r4-r6,lr}
	mov	r1,#1
	ldr	r0,[000087CC]                                          ; [pc,#&C]
	b	$00008E40

l000087C2:
	pop	{r4-r6,pc}
000087C4             F8 07 00 20 E9 86 00 00 71 86 00 00     ... ....q...

;; xAreFlashCoRoutinesStillRunning: 000087D0
xAreFlashCoRoutinesStillRunning proc
	ldr	r3,[000087D8]                                          ; [pc,#&4]
	ldr	r0,[r3]
	bx	lr
000087D6                   00 BF C0 00 00 20                   .....    

;; MPU_xTaskCreateRestricted: 000087DC
MPU_xTaskCreateRestricted proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008564
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$0000091C
	cmps	r4,#1
	mov	r3,r0
	beq	$00008802

l000087F6:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008802:
	mov	r0,r3
	pop	{r4-r6,pc}
00008806                   00 BF                               ..       

;; MPU_xTaskCreate: 00008808
;;   Called from:
;;     000080C8 (in Main)
;;     000080DA (in Main)
MPU_xTaskCreate proc
	push.w	{r4-r10,lr}
	sub	sp,#8
	mov	r5,r0
	mov	r8,r1
	mov	r9,r2
	mov	r10,r3
	ldr	r7,[sp,#&28]
	ldr	r6,[sp,#&2C]
	bl	$00008564
	mov	r3,r10
	mov	r4,r0
	str	r7,[sp]
	str	r6,[sp,#&4]
	mov	r2,r9
	mov	r1,r8
	mov	r0,r5
	bl	$000008B4
	cmps	r4,#1
	mov	r3,r0
	beq	$00008842

l00008836:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008842:
	mov	r0,r3
	add	sp,#8
	pop.w	{r4-r10,pc}
0000884A                               00 BF                       ..   

;; MPU_vTaskAllocateMPURegions: 0000884C
MPU_vTaskAllocateMPURegions proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008564
	mov	r4,r0
	mov	r1,r6
	mov	r0,r5
	bl	$00000970
	cmps	r4,#1
	beq	$00008870

l00008864:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008870:
	pop	{r4-r6,pc}
00008872       00 BF                                       ..           

;; MPU_vTaskDelayUntil: 00008874
;;   Called from:
;;     00008082 (in vCheckTask)
MPU_vTaskDelayUntil proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008564
	mov	r4,r0
	mov	r1,r6
	mov	r0,r5
	bl	$00000F80
	cmps	r4,#1
	beq	$00008898

l0000888C:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008898:
	pop	{r4-r6,pc}
0000889A                               00 BF                       ..   

;; MPU_vTaskDelay: 0000889C
MPU_vTaskDelay proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$00000F48
	cmps	r4,#1
	beq	$000088BC

l000088B0:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l000088BC:
	pop	{r3-r5,pc}
000088BE                                           00 BF               ..

;; MPU_vTaskSuspendAll: 000088C0
;;   Called from:
;;     000085FA (in vParTestSetLED)
;;     00008634 (in vParTestToggleLED)
MPU_vTaskSuspendAll proc
	push	{r4,lr}
	bl	$00008564
	mov	r4,r0
	bl	$00000A0C
	cmps	r4,#1
	beq	$000088DC

l000088D0:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l000088DC:
	pop	{r4,pc}
000088DE                                           00 BF               ..

;; MPU_xTaskResumeAll: 000088E0
;;   Called from:
;;     00008620 (in vParTestSetLED)
;;     0000865E (in vParTestToggleLED)
MPU_xTaskResumeAll proc
	push	{r4,lr}
	bl	$00008564
	mov	r4,r0
	bl	$00000E6C
	cmps	r4,#1
	mov	r3,r0
	beq	$000088FE

l000088F2:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l000088FE:
	mov	r0,r3
	pop	{r4,pc}
00008902       00 BF                                       ..           

;; MPU_xTaskGetTickCount: 00008904
;;   Called from:
;;     00008070 (in vCheckTask)
;;     00008F82 (in vCoRoutineSchedule)
MPU_xTaskGetTickCount proc
	push	{r4,lr}
	bl	$00008564
	mov	r4,r0
	bl	$00000A20
	cmps	r4,#1
	mov	r3,r0
	beq	$00008922

l00008916:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008922:
	mov	r0,r3
	pop	{r4,pc}
00008926                   00 BF                               ..       

;; MPU_uxTaskGetNumberOfTasks: 00008928
MPU_uxTaskGetNumberOfTasks proc
	push	{r4,lr}
	bl	$00008564
	mov	r4,r0
	bl	$00000A38
	cmps	r4,#1
	mov	r3,r0
	beq	$00008946

l0000893A:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008946:
	mov	r0,r3
	pop	{r4,pc}
0000894A                               00 BF                       ..   

;; MPU_pcTaskGetName: 0000894C
MPU_pcTaskGetName proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$00000A44
	cmps	r4,#1
	mov	r3,r0
	beq	$0000896E

l00008962:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l0000896E:
	mov	r0,r3
	pop	{r3-r5,pc}
00008972       00 BF                                       ..           

;; MPU_vTaskSetTimeOutState: 00008974
MPU_vTaskSetTimeOutState proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$00001144
	cmps	r4,#1
	beq	$00008994

l00008988:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008994:
	pop	{r3-r5,pc}
00008996                   00 BF                               ..       

;; MPU_xTaskCheckForTimeOut: 00008998
MPU_xTaskCheckForTimeOut proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008564
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$00001158
	cmps	r4,#1
	mov	r3,r0
	beq	$000089BE

l000089B2:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l000089BE:
	mov	r0,r3
	pop	{r4-r6,pc}
000089C2       00 BF                                       ..           

;; MPU_xTaskGenericNotify: 000089C4
MPU_xTaskGenericNotify proc
	push.w	{r4-r8,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r7,r2
	mov	r8,r3
	bl	$00008564
	mov	r3,r8
	mov	r4,r0
	mov	r2,r7
	mov	r1,r6
	mov	r0,r5
	bl	$00000A58
	cmps	r4,#1
	mov	r3,r0
	beq	$000089F4

l000089E8:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l000089F4:
	mov	r0,r3
	pop.w	{r4-r8,pc}
000089FA                               00 BF                       ..   

;; MPU_xTaskNotifyWait: 000089FC
MPU_xTaskNotifyWait proc
	push.w	{r4-r8,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r7,r2
	mov	r8,r3
	bl	$00008564
	mov	r3,r8
	mov	r4,r0
	mov	r2,r7
	mov	r1,r6
	mov	r0,r5
	bl	$00000BD4
	cmps	r4,#1
	mov	r3,r0
	beq	$00008A2C

l00008A20:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008A2C:
	mov	r0,r3
	pop.w	{r4-r8,pc}
00008A32       00 BF                                       ..           

;; MPU_ulTaskNotifyTake: 00008A34
MPU_ulTaskNotifyTake proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008564
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$00000D00
	cmps	r4,#1
	mov	r3,r0
	beq	$00008A5A

l00008A4E:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008A5A:
	mov	r0,r3
	pop	{r4-r6,pc}
00008A5E                                           00 BF               ..

;; MPU_xTaskNotifyStateClear: 00008A60
MPU_xTaskNotifyStateClear proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$00008534
	cmps	r4,#1
	mov	r3,r0
	beq	$00008A82

l00008A76:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008A82:
	mov	r0,r3
	pop	{r3-r5,pc}
00008A86                   00 BF                               ..       

;; MPU_xQueueGenericCreate: 00008A88
;;   Called from:
;;     000080AC (in Main)
;;     00008794 (in vStartFlashCoRoutines)
MPU_xQueueGenericCreate proc
	push	{r3-r7,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r7,r2
	bl	$00008564
	mov	r2,r7
	mov	r4,r0
	mov	r1,r6
	mov	r0,r5
	bl	$000006AC
	cmps	r4,#1
	mov	r3,r0
	beq	$00008AB2

l00008AA6:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008AB2:
	mov	r0,r3
	pop	{r3-r7,pc}
00008AB6                   00 BF                               ..       

;; MPU_xQueueGenericReset: 00008AB8
MPU_xQueueGenericReset proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008564
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$00000630
	cmps	r4,#1
	mov	r3,r0
	beq	$00008ADE

l00008AD2:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008ADE:
	mov	r0,r3
	pop	{r4-r6,pc}
00008AE2       00 BF                                       ..           

;; MPU_xQueueGenericSend: 00008AE4
;;   Called from:
;;     00008090 (in vCheckTask)
MPU_xQueueGenericSend proc
	push.w	{r4-r8,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r7,r2
	mov	r8,r3
	bl	$00008564
	mov	r3,r8
	mov	r4,r0
	mov	r2,r7
	mov	r1,r6
	mov	r0,r5
	bl	$00000190
	cmps	r4,#1
	mov	r3,r0
	beq	$00008B14

l00008B08:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008B14:
	mov	r0,r3
	pop.w	{r4-r8,pc}
00008B1A                               00 BF                       ..   

;; MPU_uxQueueMessagesWaiting: 00008B1C
MPU_uxQueueMessagesWaiting proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$00000428
	cmps	r4,#1
	mov	r3,r0
	beq	$00008B3E

l00008B32:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008B3E:
	mov	r0,r3
	pop	{r3-r5,pc}
00008B42       00 BF                                       ..           

;; MPU_uxQueueSpacesAvailable: 00008B44
MPU_uxQueueSpacesAvailable proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$0000043C
	cmps	r4,#1
	mov	r3,r0
	beq	$00008B66

l00008B5A:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008B66:
	mov	r0,r3
	pop	{r3-r5,pc}
00008B6A                               00 BF                       ..   

;; MPU_xQueueGenericReceive: 00008B6C
;;   Called from:
;;     0000804C (in vPrintTask)
MPU_xQueueGenericReceive proc
	push.w	{r4-r8,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r7,r2
	mov	r8,r3
	bl	$00008564
	mov	r3,r8
	mov	r4,r0
	mov	r2,r7
	mov	r1,r6
	mov	r0,r5
	bl	$000002D8
	cmps	r4,#1
	mov	r3,r0
	beq	$00008B9C

l00008B90:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008B9C:
	mov	r0,r3
	pop.w	{r4-r8,pc}
00008BA2       00 BF                                       ..           

;; MPU_xQueuePeekFromISR: 00008BA4
MPU_xQueuePeekFromISR proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008564
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$000002A4
	cmps	r4,#1
	mov	r3,r0
	beq	$00008BCA

l00008BBE:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008BCA:
	mov	r0,r3
	pop	{r4-r6,pc}
00008BCE                                           00 BF               ..

;; MPU_xQueueGetMutexHolder: 00008BD0
MPU_xQueueGetMutexHolder proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$000005B4
	cmps	r4,#1
	mov	r3,r0
	beq	$00008BF2

l00008BE6:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008BF2:
	mov	r0,r3
	pop	{r3-r5,pc}
00008BF6                   00 BF                               ..       

;; MPU_xQueueCreateMutex: 00008BF8
MPU_xQueueCreateMutex proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$000006DC
	cmps	r4,#1
	mov	r3,r0
	beq	$00008C1A

l00008C0E:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008C1A:
	mov	r0,r3
	pop	{r3-r5,pc}
00008C1E                                           00 BF               ..

;; MPU_xQueueTakeMutexRecursive: 00008C20
MPU_xQueueTakeMutexRecursive proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008564
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$000005D4
	cmps	r4,#1
	mov	r3,r0
	beq	$00008C46

l00008C3A:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008C46:
	mov	r0,r3
	pop	{r4-r6,pc}
00008C4A                               00 BF                       ..   

;; MPU_xQueueGiveMutexRecursive: 00008C4C
MPU_xQueueGiveMutexRecursive proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$00000604
	cmps	r4,#1
	mov	r3,r0
	beq	$00008C6E

l00008C62:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008C6E:
	mov	r0,r3
	pop	{r3-r5,pc}
00008C72       00 BF                                       ..           

;; MPU_vQueueDelete: 00008C74
MPU_vQueueDelete proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$00000454
	cmps	r4,#1
	beq	$00008C94

l00008C88:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008C94:
	pop	{r3-r5,pc}
00008C96                   00 BF                               ..       

;; MPU_pvPortMalloc: 00008C98
MPU_pvPortMalloc proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$0000172C
	cmps	r4,#1
	mov	r3,r0
	beq	$00008CBA

l00008CAE:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008CBA:
	mov	r0,r3
	pop	{r3-r5,pc}
00008CBE                                           00 BF               ..

;; MPU_vPortFree: 00008CC0
MPU_vPortFree proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$00001780
	cmps	r4,#1
	beq	$00008CE0

l00008CD4:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008CE0:
	pop	{r3-r5,pc}
00008CE2       00 BF                                       ..           

;; MPU_vPortInitialiseBlocks: 00008CE4
MPU_vPortInitialiseBlocks proc
	push	{r4,lr}
	bl	$00008564
	mov	r4,r0
	bl	$00001784
	cmps	r4,#1
	beq	$00008D00

l00008CF4:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008D00:
	pop	{r4,pc}
00008D02       00 BF                                       ..           

;; MPU_xPortGetFreeHeapSize: 00008D04
MPU_xPortGetFreeHeapSize proc
	push	{r4,lr}
	bl	$00008564
	mov	r4,r0
	bl	$00001794
	cmps	r4,#1
	mov	r3,r0
	beq	$00008D22

l00008D16:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008D22:
	mov	r0,r3
	pop	{r4,pc}
00008D26                   00 BF                               ..       

;; MPU_xEventGroupCreate: 00008D28
MPU_xEventGroupCreate proc
	push	{r4,lr}
	bl	$00008564
	mov	r4,r0
	bl	$000017A8
	cmps	r4,#1
	mov	r3,r0
	beq	$00008D46

l00008D3A:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008D46:
	mov	r0,r3
	pop	{r4,pc}
00008D4A                               00 BF                       ..   

;; MPU_xEventGroupWaitBits: 00008D4C
MPU_xEventGroupWaitBits proc
	push.w	{r4-r9,lr}
	sub	sp,#&C
	mov	r5,r0
	mov	r6,r1
	mov	r8,r2
	mov	r9,r3
	ldr	r7,[sp,#&28]
	bl	$00008564
	mov	r3,r9
	mov	r4,r0
	str	r7,[sp]
	mov	r2,r8
	mov	r1,r6
	mov	r0,r5
	bl	$000017C4
	cmps	r4,#1
	mov	r3,r0
	beq	$00008D82

l00008D76:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008D82:
	mov	r0,r3
	add	sp,#&C
	pop.w	{r4-r9,pc}
00008D8A                               00 BF                       ..   

;; MPU_xEventGroupClearBits: 00008D8C
MPU_xEventGroupClearBits proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008564
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$00001874
	cmps	r4,#1
	mov	r3,r0
	beq	$00008DB2

l00008DA6:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008DB2:
	mov	r0,r3
	pop	{r4-r6,pc}
00008DB6                   00 BF                               ..       

;; MPU_xEventGroupSetBits: 00008DB8
MPU_xEventGroupSetBits proc
	push	{r4-r6,lr}
	mov	r5,r0
	mov	r6,r1
	bl	$00008564
	mov	r1,r6
	mov	r4,r0
	mov	r0,r5
	bl	$00001890
	cmps	r4,#1
	mov	r3,r0
	beq	$00008DDE

l00008DD2:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008DDE:
	mov	r0,r3
	pop	{r4-r6,pc}
00008DE2       00 BF                                       ..           

;; MPU_xEventGroupSync: 00008DE4
MPU_xEventGroupSync proc
	push.w	{r4-r8,lr}
	mov	r5,r0
	mov	r6,r1
	mov	r7,r2
	mov	r8,r3
	bl	$00008564
	mov	r3,r8
	mov	r4,r0
	mov	r2,r7
	mov	r1,r6
	mov	r0,r5
	bl	$000018F8
	cmps	r4,#1
	mov	r3,r0
	beq	$00008E14

l00008E08:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008E14:
	mov	r0,r3
	pop.w	{r4-r8,pc}
00008E1A                               00 BF                       ..   

;; MPU_vEventGroupDelete: 00008E1C
MPU_vEventGroupDelete proc
	push	{r3-r5,lr}
	mov	r5,r0
	bl	$00008564
	mov	r4,r0
	mov	r0,r5
	bl	$000019A4
	cmps	r4,#1
	beq	$00008E3C

l00008E30:
	mrs	r0,cpsr
	orr	r0,r0,#1
	msr	cpsr,r0

l00008E3C:
	pop	{r3-r5,pc}
00008E3E                                           00 BF               ..

;; xCoRoutineCreate: 00008E40
;;   Called from:
;;     000087AC (in vStartFlashCoRoutines)
;;     000087BE (in vStartFlashCoRoutines)
xCoRoutineCreate proc
	push.w	{r3-fp,lr}
	mov	r9,r0
	mov	r0,#&38
	mov	r5,r1
	mov	r10,r2
	bl	$0000172C
	cmps	r0,#0
	beq	$00008EE4

l00008E54:
	ldr	r7,[00008EEC]                                          ; [pc,#&94]
	mov	r4,r0
	ldr	r3,[r7]
	cbz	r3,$00008EAC

l00008E5C:
	add	r8,r7,#4

l00008E60:
	cmps	r5,#1
	it	hs
	movhs	r5,#1

l00008E66:
	mov	r3,#0
	mov	r6,r4
	strh	r3,[r4,#&34]
	str	r5,[r4,#&2C]
	str	r10,[r4,#&30]
	str	r9,[r6],#&4
	mov	r0,r6
	bl	$000082E8
	add	r0,r4,#&18
	bl	$000082E8
	ldr	r0,[r4,#&2C]
	ldr	r3,[r7,#&70]
	rsb	r5,r5,#2
	cmps	r0,r3
	it	hi
	strhi	r0,[r7,#&70]

l00008E92:
	add.w	r0,r0,r0,lsl #2
	add.w	r0,r8,r0,lsl #2
	str	r5,[r4,#&18]
	str	r4,[r4,#&10]
	str	r4,[r4,#&24]
	mov	r1,r6
	bl	$000082F0
	mov	r0,#1
	pop.w	{r3-fp,pc}

l00008EAC:
	mov	r8,r7
	str	r0,[r8],#&4
	mov	r0,r8
	bl	$000082D0
	add	fp,r7,#&2C
	add	r0,r7,#&18
	bl	$000082D0
	add	r6,r7,#&40
	mov	r0,fp
	bl	$000082D0
	mov	r0,r6
	bl	$000082D0
	add	r0,r7,#&54
	bl	$000082D0
	str	fp,[r7,#&68]
	str	r6,[r7,#&6C]
	b	$00008E60

l00008EE4:
	mov	r0,#&FFFFFFFF
	pop.w	{r3-fp,pc}
00008EEC                                     FC 07 00 20             ... 

;; vCoRoutineAddToDelayedList: 00008EF0
;;   Called from:
;;     000083DE (in xQueueCRSend)
;;     00008490 (in xQueueCRReceive)
;;     00008760 (in prvFixedDelayCoRoutine)
vCoRoutineAddToDelayedList proc
	push	{r4-r6,lr}
	mov	r6,r1
	ldr	r4,[00008F28]                                          ; [pc,#&30]
	ldr	r3,[r4]
	ldr	r5,[r4,#&74]
	adds	r5,r0
	add	r0,r3,#4
	bl	$00008340
	ldr	r3,[r4,#&74]
	ldr	r1,[r4]
	cmps	r5,r3
	str	r5,[r1,#&4]
	ite	lo
	ldrlo	r0,[r4,#&6C]

l00008F0E:
	ldr	r0,[r4,#&68]
	adds	r1,#4
	bl	$0000830C
	cbz	r6,$00008F26

l00008F18:
	ldr	r1,[r4]
	mov	r0,r6
	pop.w	{r4-r6,lr}
	adds	r1,#&18
	b	$0000830C

l00008F26:
	pop	{r4-r6,pc}
00008F28                         FC 07 00 20                     ...    

;; vCoRoutineSchedule: 00008F2C
;;   Called from:
;;     00008212 (in vApplicationIdleHook)
vCoRoutineSchedule proc
	push.w	{r4-r8,lr}
	ldr	r5,[00009088]                                          ; [pc,#&154]
	ldr	r3,[r5,#&54]
	cbz	r3,$00008F82

l00008F36:
	mov	r7,#0
	add	r8,r5,#4

l00008F3C:
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	ldr	r3,[r5,#&60]
	ldr	r4,[r3,#&C]
	add	r0,r4,#&18
	bl	$00008340
	msr	cpsr,r7
	add	r6,r4,#4
	mov	r0,r6
	bl	$00008340
	ldr	r3,[r4,#&2C]
	ldr	r2,[r5,#&70]
	add.w	r0,r3,r3,lsl #2
	cmps	r3,r2
	mov	r1,r6
	add.w	r0,r8,r0,lsl #2
	it	hi
	strhi	r3,[r5,#&70]

l00008F78:
	bl	$000082F0
	ldr	r3,[r5,#&54]
	cmps	r3,#0
	bne	$00008F3C

l00008F82:
	bl	$00008904
	mov	r7,#0
	ldr	r2,[r5,#&78]
	ldr	r3,[r5,#&74]
	sub	r0,r0,r2
	ldr	r8,[00009090]                                          ; [pc,#&100]
	str	r0,[r5,#&7C]

l00008F94:
	cmps	r0,#0
	beq	$00009014

l00008F98:
	adds	r3,#1
	subs	r0,#1
	str	r3,[r5,#&74]
	str	r0,[r5,#&7C]
	cmps	r3,#0
	beq	$0000904C

l00008FA4:
	ldr	r2,[r5,#&68]

l00008FA6:
	ldr	r1,[r2]
	cmps	r1,#0
	beq	$00008F94

l00008FAC:
	ldr	r2,[r2,#&C]
	ldr	r4,[r2,#&C]
	ldr	r2,[r4,#&4]
	cmps	r3,r2
	bhs	$00008FC4

l00008FB6:
	b	$00008F94

l00008FB8:
	ldr	r2,[r3,#&C]
	ldr	r3,[r5,#&74]
	ldr	r4,[r2,#&C]
	ldr	r2,[r4,#&4]
	cmps	r2,r3
	bhi	$0000900E

l00008FC4:
	mov	r3,#&BF
	msr	cpsr,r3
	isb	sy
	dsb	sy
	add	r6,r4,#4
	mov	r0,r6
	bl	$00008340
	ldr	r3,[r4,#&28]
	add	r0,r4,#&18
	cbz	r3,$00008FE8

l00008FE4:
	bl	$00008340

l00008FE8:
	msr	cpsr,r7
	ldr	r3,[r4,#&2C]
	ldr	r2,[r5,#&70]
	add.w	r0,r3,r3,lsl #2
	cmps	r3,r2
	mov	r1,r6
	add.w	r0,r8,r0,lsl #2
	it	hi
	strhi	r3,[r5,#&70]

l00009000:
	bl	$000082F0
	ldr	r3,[r5,#&68]
	ldr	r2,[r3]
	cmps	r2,#0
	bne	$00008FB8

l0000900C:
	ldr	r3,[r5,#&74]

l0000900E:
	ldr	r0,[r5,#&7C]
	cmps	r0,#0
	bne	$00008F98

l00009014:
	ldr	r1,[r5,#&70]
	str	r3,[r5,#&78]
	lsls	r3,r1,#2
	add	r2,r3,r1
	add.w	r2,r5,r2,lsl #2
	ldr	r2,[r2,#&4]
	cmps	r2,#0
	bne	$00009084

l00009026:
	cbz	r1,$00009080

l00009028:
	sub	r2,r1,#1
	lsls	r3,r2,#2
	add	r0,r3,r2
	add.w	r0,r5,r0,lsl #2
	ldr	r0,[r0,#&4]
	cbnz	r0,$00009056

l00009036:
	cbz	r2,$00009046

l00009038:
	sub	r2,r1,#2
	lsls	r3,r2,#2
	add	r1,r3,r2
	add.w	r1,r5,r1,lsl #2
	ldr	r1,[r1,#&4]
	cbnz	r1,$00009056

l00009046:
	str	r2,[r5,#&70]
	pop.w	{r4-r8,pc}

l0000904C:
	ldr	r1,[r5,#&68]
	ldr	r2,[r5,#&6C]
	str	r1,[r5,#&6C]
	str	r2,[r5,#&68]
	b	$00008FA6

l00009056:
	str	r2,[r5,#&70]

l00009058:
	adds	r3,r2
	lsls	r3,r3,#2
	add	r1,r5,r3
	ldr	r2,[r1,#&8]
	ldr	r0,[0000908C]                                          ; [pc,#&28]
	ldr	r2,[r2,#&4]
	adds	r3,r0
	cmps	r2,r3
	str	r2,[r1,#&8]
	it	eq
	ldreq	r2,[r2,#&4]

l0000906E:
	ldr	r0,[r2,#&C]
	it	eq
	streq	r2,[r1,#&8]

l00009074:
	str	r0,[r5]
	ldr	r3,[r0]
	ldr	r1,[r0,#&30]
	pop.w	{r4-r8,lr}
	bx	r3

l00009080:
	pop.w	{r4-r8,pc}

l00009084:
	mov	r2,r1
	b	$00009058
00009088                         FC 07 00 20 08 08 00 20         ... ... 
00009090 00 08 00 20                                     ...            

;; xCoRoutineRemoveFromEventList: 00009094
;;   Called from:
;;     000083F2 (in xQueueCRSend)
;;     0000847C (in xQueueCRReceive)
;;     000084C6 (in xQueueCRSendFromISR)
;;     0000851C (in xQueueCRReceiveFromISR)
xCoRoutineRemoveFromEventList proc
	ldr	r3,[r0,#&C]
	push	{r4-r6,lr}
	ldr	r4,[r3,#&C]
	ldr	r5,[000090C0]                                          ; [pc,#&24]
	add	r6,r4,#&18
	mov	r0,r6
	bl	$00008340
	add	r0,r5,#&54
	mov	r1,r6
	bl	$000082F0
	ldr	r3,[r5]
	ldr	r0,[r4,#&2C]
	ldr	r3,[r3,#&2C]
	cmps	r0,r3
	ite	lo
	movlo	r0,#0

l000090BC:
	mov	r0,#1
	pop	{r4-r6,pc}
000090C0 FC 07 00 20                                     ...            

;; GPIOGetIntNumber: 000090C4
GPIOGetIntNumber proc
	ldr	r3,[00009104]                                          ; [pc,#&3C]
	cmps	r0,r3
	beq	$000090FE

l000090CA:
	bhi	$000090DE

l000090CC:
	cmp	r0,#&40004000
	beq	$000090FA

l000090D2:
	sub	r3,r3,#&1000
	cmps	r0,r3
	bne	$000090F0

l000090DA:
	mov	r0,#&11
	bx	lr

l000090DE:
	ldr	r3,[00009108]                                          ; [pc,#&28]
	cmps	r0,r3
	beq	$000090F6

l000090E4:
	add	r3,r3,#&1D000
	cmps	r0,r3
	bne	$000090F0

l000090EC:
	mov	r0,#&14
	bx	lr

l000090F0:
	mov	r0,#&FFFFFFFF
	bx	lr

l000090F6:
	mov	r0,#&13
	bx	lr

l000090FA:
	mov	r0,#&10
	bx	lr

l000090FE:
	mov	r0,#&12
	bx	lr
00009102       00 BF 00 60 00 40 00 70 00 40               ...`.@.p.@   

;; GPIODirModeSet: 0000910C
;;   Called from:
;;     00008238 (in PDCInit)
;;     00008244 (in PDCInit)
GPIODirModeSet proc
	ldr	r3,[r0,#&400]
	tst	r2,#1
	ite	ne
	orrne	r3,r1

l00009118:
	bics	r3,r1
	str	r3,[r0,#&400]
	ldr	r3,[r0,#&420]
	lsls	r2,r2,#&1E
	ite	mi
	orrmi	r1,r3

l00009128:
	bic.w	r1,r3,r1
	str	r1,[r0,#&420]
	bx	lr
00009132       00 BF                                       ..           

;; GPIODirModeGet: 00009134
GPIODirModeGet proc
	mov	r3,#1
	push	{r4}
	lsl	r1,r3,r1
	ldr	r4,[r0,#&400]
	uxtb	r1,r1
	ldr	r2,[r0,#&420]
	adcs	r4,r1
	it	eq
	moveq	r3,#0

l0000914C:
	adcs	r2,r1
	ite	ne
	movne	r0,#2

l00009152:
	mov	r0,#0
	pop	{r4}
	orrs	r0,r3
	bx	lr
0000915A                               00 BF                       ..   

;; GPIOIntTypeSet: 0000915C
GPIOIntTypeSet proc
	ldr	r3,[r0,#&408]
	tst	r2,#1
	ite	ne
	orrne	r3,r1

l00009168:
	bics	r3,r1
	str	r3,[r0,#&408]
	ldr	r3,[r0,#&404]
	tst	r2,#2
	ite	ne
	orrne	r3,r1

l0000917A:
	bics	r3,r1
	str	r3,[r0,#&404]
	ldr	r3,[r0,#&40C]
	lsls	r2,r2,#&1D
	ite	mi
	orrmi	r1,r3

l0000918A:
	bic.w	r1,r3,r1
	str	r1,[r0,#&40C]
	bx	lr

;; GPIOIntTypeGet: 00009194
GPIOIntTypeGet proc
	mov	r3,#1
	ldr	r2,[r0,#&408]
	lsl	r1,r3,r1
	uxtb	r1,r1
	ldr	r3,[r0,#&404]
	adcs	r2,r1
	ldr	r0,[r0,#&40C]
	ite	ne
	movne	r2,#1

l000091AE:
	mov	r2,#0
	adcs	r3,r1
	ite	ne
	movne	r3,#2

l000091B6:
	mov	r3,#0
	adcs	r0,r1
	ite	ne
	movne	r0,#4

l000091BE:
	mov	r0,#0
	orrs	r3,r2
	orrs	r0,r3
	bx	lr
000091C6                   00 BF                               ..       

;; GPIOPadConfigSet: 000091C8
;;   Called from:
;;     00008252 (in PDCInit)
;;     0000947A (in GPIOPinTypeComparator)
;;     000094A0 (in GPIOPinTypeI2C)
;;     000094C4 (in GPIOPinTypeQEI)
;;     000094E8 (in GPIOPinTypeUART)
GPIOPadConfigSet proc
	push	{r4}
	ldr	r4,[r0,#&500]
	tst	r2,#1
	ite	ne
	orrne	r4,r1

l000091D6:
	bics	r4,r1
	str	r4,[r0,#&500]
	ldr	r4,[r0,#&504]
	tst	r2,#2
	ite	ne
	orrne	r4,r1

l000091E8:
	bics	r4,r1
	str	r4,[r0,#&504]
	ldr	r4,[r0,#&508]
	tst	r2,#4
	ite	ne
	orrne	r4,r1

l000091FA:
	bics	r4,r1
	str	r4,[r0,#&508]
	tst	r2,#8
	ldr	r2,[r0,#&518]
	ite	ne
	orrne	r2,r1

l0000920C:
	bics	r2,r1
	str	r2,[r0,#&518]
	ldr	r2,[r0,#&50C]
	lsls	r4,r3,#&1F
	ite	mi
	orrmi	r2,r1

l0000921C:
	bics	r2,r1
	str	r2,[r0,#&50C]
	ldr	r2,[r0,#&510]
	lsls	r4,r3,#&1E
	ite	mi
	orrmi	r2,r1

l0000922C:
	bics	r2,r1
	str	r2,[r0,#&510]
	ldr	r2,[r0,#&514]
	lsls	r4,r3,#&1D
	ite	mi
	orrmi	r2,r1

l0000923C:
	bics	r2,r1
	str	r2,[r0,#&514]
	tst	r3,#8
	ldr	r3,[r0,#&51C]
	pop	{r4}
	ite	ne
	orrne	r1,r3

l00009250:
	bic.w	r1,r3,r1
	str	r1,[r0,#&51C]
	bx	lr
0000925A                               00 BF                       ..   

;; GPIOPadConfigGet: 0000925C
GPIOPadConfigGet proc
	push	{r4-r7}
	mov	r4,#1
	ldr	r5,[r0,#&500]
	lsl	r1,r4,r1
	uxtb	r1,r1
	ldr	r4,[r0,#&504]
	adcs	r5,r1
	ldr	r5,[r0,#&508]
	ite	ne
	movne	r7,#1

l00009278:
	mov	r7,#0
	adcs	r4,r1
	ldr	r4,[r0,#&518]
	ite	ne
	movne	r6,#2

l00009284:
	mov	r6,#0
	adcs	r5,r1
	ite	ne
	movne	r5,#4

l0000928C:
	mov	r5,#0
	adcs	r4,r1
	ite	ne
	movne	r4,#8

l00009294:
	mov	r4,#0
	orrs	r6,r7
	orrs	r5,r6
	orrs	r4,r5
	str	r4,[r2]
	ldr	r2,[r0,#&50C]
	ldr	r4,[r0,#&510]
	adcs	r1,r2
	ldr	r6,[r0,#&514]
	it	ne
	movne	r5,#1

l000092B0:
	ldr	r2,[r0,#&51C]
	it	eq
	moveq	r5,#0

l000092B8:
	adcs	r1,r4
	ite	ne
	movne	r4,#2

l000092BE:
	mov	r4,#0
	adcs	r1,r6
	ite	ne
	movne	r0,#4

l000092C6:
	mov	r0,#0
	adcs	r1,r2
	ite	ne
	movne	r2,#8

l000092CE:
	mov	r2,#0
	orr	r1,r4,r5
	orrs	r1,r0
	orrs	r2,r1
	str	r2,[r3]
	pop	{r4-r7}
	bx	lr
000092DE                                           00 BF               ..

;; GPIOPinIntEnable: 000092E0
GPIOPinIntEnable proc
	ldr	r3,[r0,#&410]
	orrs	r1,r3
	str	r1,[r0,#&410]
	bx	lr

;; GPIOPinIntDisable: 000092EC
GPIOPinIntDisable proc
	ldr	r3,[r0,#&410]
	bic.w	r1,r3,r1
	str	r1,[r0,#&410]
	bx	lr
000092FA                               00 BF                       ..   

;; GPIOPinIntStatus: 000092FC
GPIOPinIntStatus proc
	cbnz	r1,$00009304

l000092FE:
	ldr	r0,[r0,#&414]
	bx	lr

l00009304:
	ldr	r0,[r0,#&418]
	bx	lr
0000930A                               00 BF                       ..   

;; GPIOPinIntClear: 0000930C
GPIOPinIntClear proc
	str	r1,[r0,#&41C]
	bx	lr
00009312       00 BF                                       ..           

;; GPIOPortIntRegister: 00009314
GPIOPortIntRegister proc
	ldr	r3,[000093A8]                                          ; [pc,#&90]
	push	{r4,lr}
	cmps	r0,r3
	beq	$00009396

l0000931C:
	bhi	$0000933E

l0000931E:
	cmp	r0,#&40004000
	beq	$00009384

l00009324:
	sub	r3,r3,#&1000
	cmps	r0,r3
	bne	$0000935E

l0000932C:
	mov	r4,#&11
	mov	r0,r4
	bl	$00009504
	mov	r0,r4
	pop.w	{r4,lr}
	b	$000095DC

l0000933E:
	ldr	r3,[000093AC]                                          ; [pc,#&6C]
	cmps	r0,r3
	beq	$00009372

l00009344:
	add	r3,r3,#&1D000
	cmps	r0,r3
	bne	$0000935E

l0000934C:
	mov	r4,#&14
	mov	r0,r4
	bl	$00009504
	mov	r0,r4
	pop.w	{r4,lr}
	b	$000095DC

l0000935E:
	mov	r4,#&FFFFFFFF
	mov	r0,r4
	bl	$00009504
	mov	r0,r4
	pop.w	{r4,lr}
	b	$000095DC

l00009372:
	mov	r4,#&13
	mov	r0,r4
	bl	$00009504
	mov	r0,r4
	pop.w	{r4,lr}
	b	$000095DC

l00009384:
	mov	r4,#&10
	mov	r0,r4
	bl	$00009504
	mov	r0,r4
	pop.w	{r4,lr}
	b	$000095DC

l00009396:
	mov	r4,#&12
	mov	r0,r4
	bl	$00009504
	mov	r0,r4
	pop.w	{r4,lr}
	b	$000095DC
000093A8                         00 60 00 40 00 70 00 40         .`.@.p.@

;; GPIOPortIntUnregister: 000093B0
GPIOPortIntUnregister proc
	ldr	r3,[00009444]                                          ; [pc,#&90]
	push	{r4,lr}
	cmps	r0,r3
	beq	$00009432

l000093B8:
	bhi	$000093DA

l000093BA:
	cmp	r0,#&40004000
	beq	$00009420

l000093C0:
	sub	r3,r3,#&1000
	cmps	r0,r3
	bne	$000093FA

l000093C8:
	mov	r4,#&11
	mov	r0,r4
	bl	$00009638
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00009538

l000093DA:
	ldr	r3,[00009448]                                          ; [pc,#&6C]
	cmps	r0,r3
	beq	$0000940E

l000093E0:
	add	r3,r3,#&1D000
	cmps	r0,r3
	bne	$000093FA

l000093E8:
	mov	r4,#&14
	mov	r0,r4
	bl	$00009638
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00009538

l000093FA:
	mov	r4,#&FFFFFFFF
	mov	r0,r4
	bl	$00009638
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00009538

l0000940E:
	mov	r4,#&13
	mov	r0,r4
	bl	$00009638
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00009538

l00009420:
	mov	r4,#&10
	mov	r0,r4
	bl	$00009638
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00009538

l00009432:
	mov	r4,#&12
	mov	r0,r4
	bl	$00009638
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00009538
00009444             00 60 00 40 00 70 00 40                 .`.@.p.@   

;; GPIOPinRead: 0000944C
GPIOPinRead proc
	ldr.w	r0,[r0,r1,lsl #2]
	bx	lr
00009452       00 BF                                       ..           

;; GPIOPinWrite: 00009454
;;   Called from:
;;     00008276 (in PDCInit)
;;     00008288 (in PDCInit)
GPIOPinWrite proc
	str.w	r2,[r0,r1,lsl #2]
	bx	lr
0000945A                               00 BF                       ..   

;; GPIOPinTypeComparator: 0000945C
GPIOPinTypeComparator proc
	push	{r4-r6}
	mvns	r5,r1
	ldr	r2,[r0,#&400]
	mov	r3,#0
	ands	r2,r5
	str	r2,[r0,#&400]
	ldr	r6,[r0,#&420]
	mov	r2,#1
	ands	r5,r6
	str	r5,[r0,#&420]
	pop	{r4-r6}
	b	$000091C8
0000947E                                           00 BF               ..

;; GPIOPinTypeI2C: 00009480
;;   Called from:
;;     00009908 (in OSRAMInit)
GPIOPinTypeI2C proc
	push	{r4-r6}
	mov	r5,r1
	ldr	r2,[r0,#&400]
	mov	r3,#&B
	bic.w	r2,r2,r1
	str	r2,[r0,#&400]
	ldr	r6,[r0,#&420]
	mov	r2,#1
	orrs	r5,r6
	str	r5,[r0,#&420]
	pop	{r4-r6}
	b	$000091C8

;; GPIOPinTypeQEI: 000094A4
GPIOPinTypeQEI proc
	push	{r4-r6}
	mov	r5,r1
	ldr	r2,[r0,#&400]
	mov	r3,#&A
	bic.w	r2,r2,r1
	str	r2,[r0,#&400]
	ldr	r6,[r0,#&420]
	mov	r2,#1
	orrs	r5,r6
	str	r5,[r0,#&420]
	pop	{r4-r6}
	b	$000091C8

;; GPIOPinTypeUART: 000094C8
;;   Called from:
;;     000094EC (in GPIOPinTypeTimer)
;;     000094F0 (in GPIOPinTypeSSI)
;;     000094F4 (in GPIOPinTypePWM)
GPIOPinTypeUART proc
	push	{r4-r6}
	mov	r5,r1
	ldr	r2,[r0,#&400]
	mov	r3,#8
	bic.w	r2,r2,r1
	str	r2,[r0,#&400]
	ldr	r6,[r0,#&420]
	mov	r2,#1
	orrs	r5,r6
	str	r5,[r0,#&420]
	pop	{r4-r6}
	b	$000091C8

;; GPIOPinTypeTimer: 000094EC
GPIOPinTypeTimer proc
	b	$000094C8

;; GPIOPinTypeSSI: 000094F0
GPIOPinTypeSSI proc
	b	$000094C8

;; GPIOPinTypePWM: 000094F4
GPIOPinTypePWM proc
	b	$000094C8

;; IntDefaultHandler: 000094F8
IntDefaultHandler proc
	b	$000094F8
000094FA                               00 BF                       ..   

;; IntMasterEnable: 000094FC
IntMasterEnable proc
	b	$0000A0DC

;; IntMasterDisable: 00009500
IntMasterDisable proc
	b	$0000A0E4

;; IntRegister: 00009504
;;   Called from:
;;     00009330 (in GPIOPortIntRegister)
;;     00009350 (in GPIOPortIntRegister)
;;     00009364 (in GPIOPortIntRegister)
;;     00009376 (in GPIOPortIntRegister)
;;     00009388 (in GPIOPortIntRegister)
;;     0000939A (in GPIOPortIntRegister)
;;     00009A50 (in SSIIntRegister)
;;     00009C46 (in SysCtlIntRegister)
;;     0000A086 (in UARTIntRegister)
;;     0000A184 (in I2CIntRegister)
IntRegister proc
	ldr	r3,[00009530]                                          ; [pc,#&28]
	push	{r4-r5}
	ldr	r3,[r3]
	ldr	r4,[00009534]                                          ; [pc,#&28]
	cmps	r3,r4
	beq	$00009526

l00009510:
	mov	r3,r4
	add	r5,r4,#&B8

l00009516:
	sub	r2,r3,r4
	ldr	r2,[r2]
	str	r2,[r3],#&4
	cmps	r3,r5
	bne	$00009516

l00009522:
	ldr	r3,[00009530]                                          ; [pc,#&C]
	str	r4,[r3]

l00009526:
	str.w	r1,[r4,r0,lsl #2]
	pop	{r4-r5}
	bx	lr
0000952E                                           00 BF               ..
00009530 08 ED 00 E0 00 00 00 20                         .......        

;; IntUnregister: 00009538
;;   Called from:
;;     000093D6 (in GPIOPortIntUnregister)
;;     000093F6 (in GPIOPortIntUnregister)
;;     0000940A (in GPIOPortIntUnregister)
;;     0000941C (in GPIOPortIntUnregister)
;;     0000942E (in GPIOPortIntUnregister)
;;     00009440 (in GPIOPortIntUnregister)
;;     00009A6E (in SSIIntUnregister)
;;     00009C62 (in SysCtlIntUnregister)
;;     0000A0B0 (in UARTIntUnregister)
;;     0000A1A2 (in I2CIntUnregister)
IntUnregister proc
	ldr	r3,[00009544]                                          ; [pc,#&8]
	ldr	r2,[00009548]                                          ; [pc,#&C]
	str.w	r2,[r3,r0,lsl #2]
	bx	lr
00009542       00 BF 00 00 00 20 F9 94 00 00               ..... ....   

;; IntPriorityGroupingSet: 0000954C
IntPriorityGroupingSet proc
	ldr	r3,[00009560]                                          ; [pc,#&10]
	ldr	r2,[00009564]                                          ; [pc,#&14]
	ldr.w	r3,[r3,r0,lsl #2]
	orr	r3,r3,#&5F80000
	orr	r3,r3,#&20000
	str	r3,[r2]
	bx	lr
00009560 A4 A2 00 00 0C ED 00 E0                         ........       

;; IntPriorityGroupingGet: 00009568
IntPriorityGroupingGet proc
	mov	r3,#&700
	ldr	r1,[00009588]                                          ; [pc,#&18]
	mov	r0,#0
	ldr	r1,[r1]
	ldr	r2,[0000958C]                                          ; [pc,#&18]
	ands	r1,r3
	b	$0000957C

l00009578:
	ldr	r3,[r2],#&4

l0000957C:
	cmps	r3,r1
	beq	$00009586

l00009580:
	adds	r0,#1
	cmps	r0,#8
	bne	$00009578

l00009586:
	bx	lr
00009588                         0C ED 00 E0 A8 A2 00 00         ........

;; IntPrioritySet: 00009590
IntPrioritySet proc
	mov	r2,#&FF
	ldr	r3,[000095B8]                                          ; [pc,#&24]
	push	{r4}
	bic	r4,r0,#3
	adds	r3,r4
	ldr	r4,[r3,#&20]
	and	r0,r0,#3
	ldr	r3,[r4]
	lsls	r0,r0,#3
	lsls	r2,r0
	bic.w	r3,r3,r2
	lsl	r0,r1,r0
	orrs	r0,r3
	str	r0,[r4]
	pop	{r4}
	bx	lr
000095B8                         A4 A2 00 00                     ....   

;; IntPriorityGet: 000095BC
IntPriorityGet proc
	ldr	r3,[000095D8]                                          ; [pc,#&18]
	bic	r2,r0,#3
	adds	r3,r2
	ldr	r3,[r3,#&20]
	and	r0,r0,#3
	ldr	r3,[r3]
	lsls	r0,r0,#3
	lsr	r0,r3,r0
	uxtb	r0,r0
	bx	lr
000095D6                   00 BF A4 A2 00 00                   ......   

;; IntEnable: 000095DC
;;   Called from:
;;     0000933A (in GPIOPortIntRegister)
;;     0000935A (in GPIOPortIntRegister)
;;     0000936E (in GPIOPortIntRegister)
;;     00009380 (in GPIOPortIntRegister)
;;     00009392 (in GPIOPortIntRegister)
;;     000093A4 (in GPIOPortIntRegister)
;;     00009A5A (in SSIIntRegister)
;;     00009C50 (in SysCtlIntRegister)
;;     0000A090 (in UARTIntRegister)
;;     0000A18E (in I2CIntRegister)
IntEnable proc
	cmps	r0,#4
	beq	$00009608

l000095E0:
	cmps	r0,#5
	beq	$00009614

l000095E4:
	cmps	r0,#6
	beq	$00009620

l000095E8:
	cmps	r0,#&F
	beq	$000095FC

l000095EC:
	bls	$000095FA

l000095EE:
	mov	r3,#1
	subs	r0,#&10
	ldr	r2,[0000962C]                                          ; [pc,#&38]
	lsl	r0,r3,r0
	str	r0,[r2]

l000095FA:
	bx	lr

l000095FC:
	ldr	r2,[00009630]                                          ; [pc,#&30]
	ldr	r3,[r2]
	orr	r3,r3,#2
	str	r3,[r2]
	bx	lr

l00009608:
	ldr	r2,[00009634]                                          ; [pc,#&28]
	ldr	r3,[r2]
	orr	r3,r3,#&10000
	str	r3,[r2]
	bx	lr

l00009614:
	ldr	r2,[00009634]                                          ; [pc,#&1C]
	ldr	r3,[r2]
	orr	r3,r3,#&20000
	str	r3,[r2]
	bx	lr

l00009620:
	ldr	r2,[00009634]                                          ; [pc,#&10]
	ldr	r3,[r2]
	orr	r3,r3,#&40000
	str	r3,[r2]
	bx	lr
0000962C                                     00 E1 00 E0             ....
00009630 10 E0 00 E0 24 ED 00 E0                         ....$...       

;; IntDisable: 00009638
;;   Called from:
;;     000093CC (in GPIOPortIntUnregister)
;;     000093EC (in GPIOPortIntUnregister)
;;     00009400 (in GPIOPortIntUnregister)
;;     00009412 (in GPIOPortIntUnregister)
;;     00009424 (in GPIOPortIntUnregister)
;;     00009436 (in GPIOPortIntUnregister)
;;     00009A64 (in SSIIntUnregister)
;;     00009C58 (in SysCtlIntUnregister)
;;     0000A0A6 (in UARTIntUnregister)
;;     0000A198 (in I2CIntUnregister)
IntDisable proc
	cmps	r0,#4
	beq	$00009664

l0000963C:
	cmps	r0,#5
	beq	$00009670

l00009640:
	cmps	r0,#6
	beq	$0000967C

l00009644:
	cmps	r0,#&F
	beq	$00009658

l00009648:
	bls	$00009656

l0000964A:
	mov	r3,#1
	subs	r0,#&10
	ldr	r2,[00009688]                                          ; [pc,#&38]
	lsl	r0,r3,r0
	str	r0,[r2]

l00009656:
	bx	lr

l00009658:
	ldr	r2,[0000968C]                                          ; [pc,#&30]
	ldr	r3,[r2]
	bic	r3,r3,#2
	str	r3,[r2]
	bx	lr

l00009664:
	ldr	r2,[00009690]                                          ; [pc,#&28]
	ldr	r3,[r2]
	bic	r3,r3,#&10000
	str	r3,[r2]
	bx	lr

l00009670:
	ldr	r2,[00009690]                                          ; [pc,#&1C]
	ldr	r3,[r2]
	bic	r3,r3,#&20000
	str	r3,[r2]
	bx	lr

l0000967C:
	ldr	r2,[00009690]                                          ; [pc,#&10]
	ldr	r3,[r2]
	bic	r3,r3,#&40000
	str	r3,[r2]
	bx	lr
00009688                         80 E1 00 E0 10 E0 00 E0         ........
00009690 24 ED 00 E0                                     $...           

;; OSRAMDelay: 00009694
;;   Called from:
;;     000096DE (in OSRAMWriteArray)
;;     00009718 (in OSRAMWriteByte)
;;     00009750 (in OSRAMWriteFinal)
;;     00009776 (in OSRAMWriteFinal)
OSRAMDelay proc
	subs	r0,#1
	bne	$00009694

l00009698:
	bx	lr
0000969A                               00 BF                       ..   

;; OSRAMWriteFirst: 0000969C
;;   Called from:
;;     00009784 (in OSRAMClear)
;;     000097A4 (in OSRAMClear)
;;     000097D6 (in OSRAMStringDraw)
;;     000098A4 (in OSRAMImageDraw)
;;     00009938 (in OSRAMInit)
;;     00009996 (in OSRAMDisplayOn)
;;     000099C4 (in OSRAMDisplayOff)
OSRAMWriteFirst proc
	push	{r3-r5,lr}
	mov	r5,r0
	ldr	r4,[000096C0]                                          ; [pc,#&1C]
	mov	r2,#0
	mov	r0,r4
	mov	r1,#&3D
	bl	$0000A208
	mov	r1,r5
	mov	r0,r4
	bl	$0000A23C
	mov	r0,r4
	pop.w	{r3-r5,lr}
	mov	r1,#3
	b	$0000A220
000096C0 00 00 02 40                                     ...@           

;; OSRAMWriteArray: 000096C4
;;   Called from:
;;     0000978C (in OSRAMClear)
;;     000097AC (in OSRAMClear)
;;     0000983A (in OSRAMStringDraw)
;;     0000985A (in OSRAMStringDraw)
;;     000098D8 (in OSRAMImageDraw)
;;     00009944 (in OSRAMInit)
;;     000099A2 (in OSRAMDisplayOn)
OSRAMWriteArray proc
	cbz	r1,$000096FA

l000096C6:
	push	{r3-r7,lr}
	mov	r5,r0
	ldr	r7,[000096FC]                                          ; [pc,#&30]
	ldr	r4,[00009700]                                          ; [pc,#&30]
	add	r6,r0,r1

l000096D0:
	mov	r1,#0
	mov	r0,r4
	bl	$0000A1C8
	cmps	r0,#0
	beq	$000096D0

l000096DC:
	ldr	r0,[r7]
	bl	$00009694
	ldrb	r1,[r5],#&1
	mov	r0,r4
	bl	$0000A23C
	mov	r1,#1
	mov	r0,r4
	bl	$0000A220
	cmps	r6,r5
	bne	$000096D0

l000096F8:
	pop	{r3-r7,pc}

l000096FA:
	bx	lr
000096FC                                     7C 08 00 20             |.. 
00009700 00 00 02 40                                     ...@           

;; OSRAMWriteByte: 00009704
;;   Called from:
;;     00009794 (in OSRAMClear)
;;     000097B4 (in OSRAMClear)
;;     000097E2 (in OSRAMStringDraw)
;;     000097EC (in OSRAMStringDraw)
;;     000097F4 (in OSRAMStringDraw)
;;     000097FA (in OSRAMStringDraw)
;;     00009806 (in OSRAMStringDraw)
;;     0000980C (in OSRAMStringDraw)
;;     00009824 (in OSRAMStringDraw)
;;     000098B0 (in OSRAMImageDraw)
;;     000098B6 (in OSRAMImageDraw)
;;     000098BC (in OSRAMImageDraw)
;;     000098C2 (in OSRAMImageDraw)
;;     000098C8 (in OSRAMImageDraw)
;;     000098CE (in OSRAMImageDraw)
;;     000099CA (in OSRAMDisplayOff)
;;     000099D0 (in OSRAMDisplayOff)
;;     000099D6 (in OSRAMDisplayOff)
;;     000099DC (in OSRAMDisplayOff)
OSRAMWriteByte proc
	push	{r4,lr}
	mov	r4,r0

l00009708:
	mov	r1,#0
	ldr	r0,[00009730]                                          ; [pc,#&24]
	bl	$0000A1C8
	cmps	r0,#0
	beq	$00009708

l00009714:
	ldr	r3,[00009734]                                          ; [pc,#&1C]
	ldr	r0,[r3]
	bl	$00009694
	mov	r1,r4
	ldr	r0,[00009730]                                          ; [pc,#&10]
	bl	$0000A23C
	pop.w	{r4,lr}
	mov	r1,#1
	ldr	r0,[00009730]                                          ; [pc,#&4]
	b	$0000A220
00009730 00 00 02 40 7C 08 00 20                         ...@|..        

;; OSRAMWriteFinal: 00009738
;;   Called from:
;;     0000979E (in OSRAMClear)
;;     000097C2 (in OSRAMClear)
;;     0000984A (in OSRAMStringDraw)
;;     00009872 (in OSRAMStringDraw)
;;     000098E2 (in OSRAMImageDraw)
;;     0000994C (in OSRAMInit)
;;     000099AA (in OSRAMDisplayOn)
;;     000099E6 (in OSRAMDisplayOff)
OSRAMWriteFinal proc
	push	{r4-r6,lr}
	mov	r6,r0
	ldr	r4,[00009778]                                          ; [pc,#&38]

l0000973E:
	mov	r1,#0
	mov	r0,r4
	bl	$0000A1C8
	cmps	r0,#0
	beq	$0000973E

l0000974A:
	ldr	r5,[0000977C]                                          ; [pc,#&30]
	ldr	r4,[00009778]                                          ; [pc,#&28]
	ldr	r0,[r5]
	bl	$00009694
	mov	r1,r6
	mov	r0,r4
	bl	$0000A23C
	mov	r1,#5
	mov	r0,r4
	bl	$0000A220

l00009764:
	mov	r1,#0
	mov	r0,r4
	bl	$0000A1C8
	cmps	r0,#0
	beq	$00009764

l00009770:
	ldr	r0,[r5]
	pop.w	{r4-r6,lr}
	b	$00009694
00009778                         00 00 02 40 7C 08 00 20         ...@|.. 

;; OSRAMClear: 00009780
;;   Called from:
;;     00008050 (in vPrintTask)
;;     0000995C (in OSRAMInit)
OSRAMClear proc
	push	{r4,lr}
	mov	r0,#&80
	bl	$0000969C
	mov	r1,#6
	ldr	r0,[000097C4]                                          ; [pc,#&38]
	bl	$000096C4
	mov	r4,#&5F

l00009792:
	mov	r0,#0
	bl	$00009704
	subs	r4,#1
	bne	$00009792

l0000979C:
	mov	r0,r4
	bl	$00009738
	mov	r0,#&80
	bl	$0000969C
	mov	r1,#6
	ldr	r0,[000097C8]                                          ; [pc,#&1C]
	bl	$000096C4
	mov	r4,#&5F

l000097B2:
	mov	r0,#0
	bl	$00009704
	subs	r4,#1
	bne	$000097B2

l000097BC:
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00009738
000097C4             F4 A2 00 00 FC A2 00 00                 ........   

;; OSRAMStringDraw: 000097CC
;;   Called from:
;;     0000805E (in vPrintTask)
;;     000080E8 (in Main)
OSRAMStringDraw proc
	push	{r4-r6,lr}
	mov	r6,r2
	mov	r4,r1
	mov	r5,r0
	mov	r0,#&80
	bl	$0000969C
	cmps	r6,#0
	ite	eq
	moveq	r0,#&B0

l000097E0:
	mov	r0,#&B1
	bl	$00009704
	add	r6,r4,#&24
	mov	r0,#&80
	bl	$00009704
	and	r0,r6,#&F
	bl	$00009704
	mov	r0,#&80
	bl	$00009704
	ubfx	r0,r6,#4,#4
	orr	r0,r0,#&10
	bl	$00009704
	mov	r0,#&40
	bl	$00009704
	ldrb	r3,[r5]
	cbz	r3,$00009876

l00009814:
	cmps	r4,#&5A
	ldr	r6,[00009878]                                          ; [pc,#&60]
	bls	$00009830

l0000981A:
	b	$0000984C

l0000981C:
	ldrb	r3,[r5,#&1]!
	adds	r4,#6
	cbz	r3,$00009846

l00009824:
	bl	$00009704
	ldrb	r3,[r5]
	cbz	r3,$00009874

l0000982C:
	cmps	r4,#&5A
	bhi	$0000984C

l00009830:
	subs	r3,#&20
	add.w	r3,r3,r3,lsl #2
	add	r0,r6,r3
	mov	r1,#5
	bl	$000096C4
	cmps	r4,#&5A
	mov	r0,#0
	bne	$0000981C

l00009846:
	pop.w	{r4-r6,lr}
	b	$00009738

l0000984C:
	subs	r3,#&20
	add.w	r3,r3,r3,lsl #2
	rsb	r4,r4,#&5F
	add	r0,r6,r3
	mov	r1,r4
	bl	$000096C4
	ldrb	r3,[r5]
	ldr	r2,[0000987C]                                          ; [pc,#&18]
	subs	r3,#&20
	add.w	r3,r3,r3,lsl #2
	adds	r3,r2
	adds	r3,r4
	ldrb	r0,[r3,#&10]
	pop.w	{r4-r6,lr}
	b	$00009738

l00009874:
	pop	{r4-r6,pc}

l00009876:
	pop	{r4-r6,pc}
00009878                         04 A3 00 00 F4 A2 00 00         ........

;; OSRAMImageDraw: 00009880
OSRAMImageDraw proc
	push.w	{r4-r10,lr}
	ldr	r6,[sp,#&20]
	cbz	r6,$000098EA

l00009888:
	mov	r5,r0
	mov	r4,r2
	mov	r9,r3
	adds	r1,#&24
	ubfx	r8,r1,#4,#4
	adds	r6,r2
	orr	r8,r8,#&10
	and	r7,r1,#&F
	add	r10,r3,#&FFFFFFFF

l000098A2:
	mov	r0,#&80
	bl	$0000969C
	cmps	r4,#0
	ite	ne
	movne	r0,#&B1

l000098AE:
	mov	r0,#&B0
	bl	$00009704
	mov	r0,#&80
	bl	$00009704
	mov	r0,r7
	bl	$00009704
	mov	r0,#&80
	bl	$00009704
	mov	r0,r8
	bl	$00009704
	mov	r0,#&40
	bl	$00009704
	mov	r0,r5
	mov	r1,r10
	adds	r5,r9
	bl	$000096C4
	adds	r4,#1
	ldrb.w	r0,[r5,-#&1]
	bl	$00009738
	cmps	r6,r4
	bne	$000098A2

l000098EA:
	pop.w	{r4-r10,pc}
000098EE                                           00 BF               ..

;; OSRAMInit: 000098F0
;;   Called from:
;;     000080B6 (in Main)
OSRAMInit proc
	push.w	{r4-r8,lr}
	mov	r4,r0
	mov	r0,#&10001000
	bl	$00009B7C
	ldr	r0,[00009960]                                          ; [pc,#&60]
	bl	$00009B7C
	mov	r1,#&C
	ldr	r0,[00009964]                                          ; [pc,#&5C]
	bl	$00009480
	mov	r1,r4
	ldr	r0,[00009968]                                          ; [pc,#&58]
	bl	$0000A0F4
	mov	r2,#1
	ldr	r3,[0000996C]                                          ; [pc,#&54]
	ldr	r7,[00009970]                                          ; [pc,#&54]
	mov	r6,#&E3
	mov	r4,#4
	mov	r0,#&80
	mov	r5,#0
	str	r2,[r3]
	add	r8,r7,#&1EC
	b	$00009938

l0000992A:
	ldrb	r4,[r3,#&1EC]
	ldrb	r0,[r3,#&1ED]
	adds	r3,r4
	ldrb	r6,[r3,#&1EC]

l00009938:
	bl	$0000969C
	add	r0,r5,#2
	sub	r1,r4,#2
	adds	r0,r8
	adds	r4,#1
	bl	$000096C4
	adds	r5,r4
	mov	r0,r6
	bl	$00009738
	cmps	r5,#&70
	add.w	r3,r7,r5
	bls	$0000992A

l00009958:
	pop.w	{r4-r8,lr}
	b	$00009780
00009960 02 00 00 20 00 50 00 40 00 00 02 40 7C 08 00 20 ... .P.@...@|.. 
00009970 F4 A2 00 00                                     ....           

;; OSRAMDisplayOn: 00009974
OSRAMDisplayOn proc
	push.w	{r4-r8,lr}
	ldr	r7,[000099BC]                                          ; [pc,#&40]
	mov	r6,#&E3
	mov	r4,#4
	mov	r0,#&80
	mov	r5,#0
	add	r8,r7,#&1EC
	b	$00009996

l00009988:
	ldrb	r4,[r3,#&1EC]
	ldrb	r0,[r3,#&1ED]
	adds	r3,r4
	ldrb	r6,[r3,#&1EC]

l00009996:
	bl	$0000969C
	add	r0,r5,#2
	sub	r1,r4,#2
	adds	r0,r8
	adds	r4,#1
	bl	$000096C4
	adds	r5,r4
	mov	r0,r6
	bl	$00009738
	cmps	r5,#&70
	add.w	r3,r7,r5
	bls	$00009988

l000099B6:
	pop.w	{r4-r8,pc}
000099BA                               00 BF F4 A2 00 00           ......

;; OSRAMDisplayOff: 000099C0
OSRAMDisplayOff proc
	push	{r3,lr}
	mov	r0,#&80
	bl	$0000969C
	mov	r0,#&AE
	bl	$00009704
	mov	r0,#&80
	bl	$00009704
	mov	r0,#&AD
	bl	$00009704
	mov	r0,#&80
	bl	$00009704
	pop.w	{r3,lr}
	mov	r0,#&8A
	b	$00009738

;; SSIConfig: 000099E8
;;   Called from:
;;     00008264 (in PDCInit)
SSIConfig proc
	push.w	{r4-r8,lr}
	mov	r7,r2
	mov	r6,r0
	mov	r8,r1
	mov	r4,r3
	ldr	r5,[sp,#&18]
	bl	$00009DF0
	cmps	r7,#2
	beq	$00009A30

l000099FE:
	cmps	r7,#0
	it	ne
	movne	r7,#4

l00009A04:
	udiv	r3,r0,r4
	mov	r4,#0
	str	r7,[r6,#&4]

l00009A0C:
	adds	r4,#2
	udiv	r2,r3,r4
	subs	r2,#1
	cmps	r2,#&FF
	bhi	$00009A0C

l00009A18:
	and	r3,r8,#&30
	subs	r5,#1
	orr	r1,r3,r8,lsl #6
	orrs	r5,r1
	orr	r2,r5,r2,lsl #8
	str	r4,[r6,#&10]
	str	r2,[r6]
	pop.w	{r4-r8,pc}

l00009A30:
	mov	r7,#&C
	b	$00009A04

;; SSIEnable: 00009A34
;;   Called from:
;;     0000826A (in PDCInit)
SSIEnable proc
	ldr	r3,[r0,#&4]
	orr	r3,r3,#2
	str	r3,[r0,#&4]
	bx	lr
00009A3E                                           00 BF               ..

;; SSIDisable: 00009A40
SSIDisable proc
	ldr	r3,[r0,#&4]
	bic	r3,r3,#2
	str	r3,[r0,#&4]
	bx	lr
00009A4A                               00 BF                       ..   

;; SSIIntRegister: 00009A4C
SSIIntRegister proc
	push	{r3,lr}
	mov	r0,#&17
	bl	$00009504
	pop.w	{r3,lr}
	mov	r0,#&17
	b	$000095DC
00009A5E                                           00 BF               ..

;; SSIIntUnregister: 00009A60
SSIIntUnregister proc
	push	{r3,lr}
	mov	r0,#&17
	bl	$00009638
	pop.w	{r3,lr}
	mov	r0,#&17
	b	$00009538
00009A72       00 BF                                       ..           

;; SSIIntEnable: 00009A74
SSIIntEnable proc
	ldr	r3,[r0,#&14]
	orrs	r1,r3
	str	r1,[r0,#&14]
	bx	lr

;; SSIIntDisable: 00009A7C
SSIIntDisable proc
	ldr	r3,[r0,#&14]
	bic.w	r1,r3,r1
	str	r1,[r0,#&14]
	bx	lr
00009A86                   00 BF                               ..       

;; SSIIntStatus: 00009A88
SSIIntStatus proc
	cbnz	r1,$00009A8E

l00009A8A:
	ldr	r0,[r0,#&18]
	bx	lr

l00009A8E:
	ldr	r0,[r0,#&1C]
	bx	lr
00009A92       00 BF                                       ..           

;; SSIIntClear: 00009A94
SSIIntClear proc
	str	r1,[r0,#&20]
	bx	lr

;; SSIDataPut: 00009A98
;;   Called from:
;;     000082AA (in PDCWrite)
;;     000082B2 (in PDCWrite)
SSIDataPut proc
	add	r2,r0,#&C

l00009A9C:
	ldr	r3,[r2]
	lsls	r3,r3,#&1E
	bpl	$00009A9C

l00009AA2:
	str	r1,[r0,#&8]
	bx	lr
00009AA6                   00 BF                               ..       

;; SSIDataNonBlockingPut: 00009AA8
SSIDataNonBlockingPut proc
	ldr	r3,[r0,#&C]
	ands	r3,r3,#2
	itte	ne
	strne	r1,[r0,#&8]

l00009AB2:
	mov	r0,#1
	mov	r0,r3
	bx	lr

;; SSIDataGet: 00009AB8
;;   Called from:
;;     000082BA (in PDCWrite)
;;     000082C2 (in PDCWrite)
SSIDataGet proc
	add	r2,r0,#&C

l00009ABC:
	ldr	r3,[r2]
	lsls	r3,r3,#&1D
	bpl	$00009ABC

l00009AC2:
	ldr	r3,[r0,#&8]
	str	r3,[r1]
	bx	lr

;; SSIDataNonBlockingGet: 00009AC8
SSIDataNonBlockingGet proc
	ldr	r3,[r0,#&C]
	ands	r3,r3,#4
	ittte	ne
	ldrne	r3,[r0,#&8]

l00009AD2:
	mov	r0,#1
	str	r3,[r1]
	mov	r0,r3
	bx	lr
00009ADA                               00 BF                       ..   

;; SysCtlSRAMSizeGet: 00009ADC
SysCtlSRAMSizeGet proc
	ldr	r3,[00009AEC]                                          ; [pc,#&C]
	ldr	r0,[00009AF0]                                          ; [pc,#&10]
	ldr	r3,[r3]
	and.w	r0,r0,r3,lsr #8
	add	r0,r0,#&100
	bx	lr
00009AEC                                     08 E0 0F 40             ...@
00009AF0 00 FF FF 00                                     ....           

;; SysCtlFlashSizeGet: 00009AF4
SysCtlFlashSizeGet proc
	ldr	r3,[00009B04]                                          ; [pc,#&C]
	ldr	r0,[00009B08]                                          ; [pc,#&10]
	ldr	r3,[r3]
	and.w	r0,r0,r3,lsl #&B
	add	r0,r0,#&800
	bx	lr
00009B04             08 E0 0F 40 00 F8 FF 07                 ...@....   

;; SysCtlPinPresent: 00009B0C
SysCtlPinPresent proc
	ldr	r3,[00009B1C]                                          ; [pc,#&C]
	ldr	r3,[r3]
	adcs	r3,r0
	ite	ne
	movne	r0,#1

l00009B16:
	mov	r0,#0
	bx	lr
00009B1A                               00 BF 18 E0 0F 40           .....@

;; SysCtlPeripheralPresent: 00009B20
SysCtlPeripheralPresent proc
	ldr	r3,[00009B38]                                          ; [pc,#&14]
	lsrs	r2,r0,#&1C
	ldr.w	r3,[r3,r2,lsl #2]
	bic	r0,r0,#&F0000000
	ldr	r3,[r3]
	adcs	r0,r3
	ite	ne
	movne	r0,#1

l00009B34:
	mov	r0,#0
	bx	lr
00009B38                         54 A5 00 00                     T...   

;; SysCtlPeripheralReset: 00009B3C
SysCtlPeripheralReset proc
	mov	r1,#0
	ldr	r3,[00009B78]                                          ; [pc,#&38]
	lsrs	r2,r0,#&1C
	push	{r4}
	add.w	r3,r3,r2,lsl #2
	ldr	r2,[r3,#&10]
	bic	r3,r0,#&F0000000
	ldr	r4,[r2]
	sub	sp,#&C
	orrs	r3,r4
	str	r3,[r2]
	str	r1,[sp,#&4]
	ldr	r3,[sp,#&4]
	cmps	r3,#&F
	bhi	$00009B6A

l00009B5E:
	ldr	r3,[sp,#&4]
	adds	r3,#1
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]
	cmps	r3,#&F
	bls	$00009B5E

l00009B6A:
	ldr	r3,[r2]
	bic.w	r0,r3,r0
	str	r0,[r2]
	add	sp,#&C
	pop	{r4}
	bx	lr
00009B78                         54 A5 00 00                     T...   

;; SysCtlPeripheralEnable: 00009B7C
;;   Called from:
;;     00008226 (in PDCInit)
;;     0000822C (in PDCInit)
;;     000098FA (in OSRAMInit)
;;     00009900 (in OSRAMInit)
SysCtlPeripheralEnable proc
	ldr	r3,[00009B94]                                          ; [pc,#&14]
	lsrs	r2,r0,#&1C
	add.w	r3,r3,r2,lsl #2
	ldr	r3,[r3,#&1C]
	bic	r0,r0,#&F0000000
	ldr	r2,[r3]
	orrs	r0,r2
	str	r0,[r3]
	bx	lr
00009B92       00 BF 54 A5 00 00                           ..T...       

;; SysCtlPeripheralDisable: 00009B98
SysCtlPeripheralDisable proc
	ldr	r3,[00009BB0]                                          ; [pc,#&14]
	lsrs	r2,r0,#&1C
	add.w	r3,r3,r2,lsl #2
	ldr	r2,[r3,#&1C]
	bic	r0,r0,#&F0000000
	ldr	r3,[r2]
	bic.w	r0,r3,r0
	str	r0,[r2]
	bx	lr
00009BB0 54 A5 00 00                                     T...           

;; SysCtlPeripheralSleepEnable: 00009BB4
SysCtlPeripheralSleepEnable proc
	ldr	r3,[00009BCC]                                          ; [pc,#&14]
	lsrs	r2,r0,#&1C
	add.w	r3,r3,r2,lsl #2
	ldr	r3,[r3,#&28]
	bic	r0,r0,#&F0000000
	ldr	r2,[r3]
	orrs	r0,r2
	str	r0,[r3]
	bx	lr
00009BCA                               00 BF 54 A5 00 00           ..T...

;; SysCtlPeripheralSleepDisable: 00009BD0
SysCtlPeripheralSleepDisable proc
	ldr	r3,[00009BE8]                                          ; [pc,#&14]
	lsrs	r2,r0,#&1C
	add.w	r3,r3,r2,lsl #2
	ldr	r2,[r3,#&28]
	bic	r0,r0,#&F0000000
	ldr	r3,[r2]
	bic.w	r0,r3,r0
	str	r0,[r2]
	bx	lr
00009BE8                         54 A5 00 00                     T...   

;; SysCtlPeripheralDeepSleepEnable: 00009BEC
SysCtlPeripheralDeepSleepEnable proc
	ldr	r3,[00009C04]                                          ; [pc,#&14]
	lsrs	r2,r0,#&1C
	add.w	r3,r3,r2,lsl #2
	ldr	r3,[r3,#&34]
	bic	r0,r0,#&F0000000
	ldr	r2,[r3]
	orrs	r0,r2
	str	r0,[r3]
	bx	lr
00009C02       00 BF 54 A5 00 00                           ..T...       

;; SysCtlPeripheralDeepSleepDisable: 00009C08
SysCtlPeripheralDeepSleepDisable proc
	ldr	r3,[00009C20]                                          ; [pc,#&14]
	lsrs	r2,r0,#&1C
	add.w	r3,r3,r2,lsl #2
	ldr	r2,[r3,#&34]
	bic	r0,r0,#&F0000000
	ldr	r3,[r2]
	bic.w	r0,r3,r0
	str	r0,[r2]
	bx	lr
00009C20 54 A5 00 00                                     T...           

;; SysCtlPeripheralClockGating: 00009C24
SysCtlPeripheralClockGating proc
	ldr	r2,[00009C3C]                                          ; [pc,#&14]
	ldr	r3,[r2]
	cbnz	r0,$00009C32

l00009C2A:
	bic	r3,r3,#&8000000
	str	r3,[r2]
	bx	lr

l00009C32:
	orr	r3,r3,#&8000000
	str	r3,[r2]
	bx	lr
00009C3A                               00 BF 60 E0 0F 40           ..`..@

;; SysCtlIntRegister: 00009C40
SysCtlIntRegister proc
	push	{r3,lr}
	mov	r1,r0
	mov	r0,#&2C
	bl	$00009504
	pop.w	{r3,lr}
	mov	r0,#&2C
	b	$000095DC

;; SysCtlIntUnregister: 00009C54
SysCtlIntUnregister proc
	push	{r3,lr}
	mov	r0,#&2C
	bl	$00009638
	pop.w	{r3,lr}
	mov	r0,#&2C
	b	$00009538
00009C66                   00 BF                               ..       

;; SysCtlIntEnable: 00009C68
SysCtlIntEnable proc
	ldr	r2,[00009C74]                                          ; [pc,#&8]
	ldr	r3,[r2]
	orrs	r0,r3
	str	r0,[r2]
	bx	lr
00009C72       00 BF 54 E0 0F 40                           ..T..@       

;; SysCtlIntDisable: 00009C78
SysCtlIntDisable proc
	ldr	r2,[00009C84]                                          ; [pc,#&8]
	ldr	r3,[r2]
	bic.w	r0,r3,r0
	str	r0,[r2]
	bx	lr
00009C84             54 E0 0F 40                             T..@       

;; SysCtlIntClear: 00009C88
SysCtlIntClear proc
	ldr	r3,[00009C90]                                          ; [pc,#&4]
	str	r0,[r3]
	bx	lr
00009C8E                                           00 BF               ..
00009C90 58 E0 0F 40                                     X..@           

;; SysCtlIntStatus: 00009C94
SysCtlIntStatus proc
	cbnz	r0,$00009C9C

l00009C96:
	ldr	r3,[00009CA4]                                          ; [pc,#&C]
	ldr	r0,[r3]
	bx	lr

l00009C9C:
	ldr	r3,[00009CA8]                                          ; [pc,#&8]
	ldr	r0,[r3]
	bx	lr
00009CA2       00 BF 50 E0 0F 40 58 E0 0F 40               ..P..@X..@   

;; SysCtlLDOSet: 00009CAC
SysCtlLDOSet proc
	ldr	r3,[00009CB4]                                          ; [pc,#&4]
	str	r0,[r3]
	bx	lr
00009CB2       00 BF 34 E0 0F 40                           ..4..@       

;; SysCtlLDOGet: 00009CB8
SysCtlLDOGet proc
	ldr	r3,[00009CC0]                                          ; [pc,#&4]
	ldr	r0,[r3]
	bx	lr
00009CBE                                           00 BF               ..
00009CC0 34 E0 0F 40                                     4..@           

;; SysCtlLDOConfigSet: 00009CC4
SysCtlLDOConfigSet proc
	ldr	r3,[00009CCC]                                          ; [pc,#&4]
	str	r0,[r3]
	bx	lr
00009CCA                               00 BF 60 E1 0F 40           ..`..@

;; SysCtlReset: 00009CD0
SysCtlReset proc
	ldr	r3,[00009CD8]                                          ; [pc,#&4]
	ldr	r2,[00009CDC]                                          ; [pc,#&8]
	str	r2,[r3]

l00009CD6:
	b	$00009CD6
00009CD8                         0C ED 00 E0 04 00 FA 05         ........

;; SysCtlSleep: 00009CE0
SysCtlSleep proc
	b	$0000A0EC

;; SysCtlDeepSleep: 00009CE4
SysCtlDeepSleep proc
	push	{r4,lr}
	ldr	r4,[00009D00]                                          ; [pc,#&18]
	ldr	r3,[r4]
	orr	r3,r3,#4
	str	r3,[r4]
	bl	$0000A0EC
	ldr	r3,[r4]
	bic	r3,r3,#4
	str	r3,[r4]
	pop	{r4,pc}
00009CFE                                           00 BF               ..
00009D00 10 ED 00 E0                                     ....           

;; SysCtlResetCauseGet: 00009D04
SysCtlResetCauseGet proc
	ldr	r3,[00009D0C]                                          ; [pc,#&4]
	ldr	r0,[r3]
	bx	lr
00009D0A                               00 BF 5C E0 0F 40           ..\..@

;; SysCtlResetCauseClear: 00009D10
SysCtlResetCauseClear proc
	ldr	r2,[00009D1C]                                          ; [pc,#&8]
	ldr	r3,[r2]
	bic.w	r0,r3,r0
	str	r0,[r2]
	bx	lr
00009D1C                                     5C E0 0F 40             \..@

;; SysCtlBrownOutConfigSet: 00009D20
SysCtlBrownOutConfigSet proc
	ldr	r3,[00009D2C]                                          ; [pc,#&8]
	orr	r1,r0,r1,lsl #2
	str	r1,[r3]
	bx	lr
00009D2A                               00 BF 30 E0 0F 40           ..0..@

;; SysCtlClockSet: 00009D30
SysCtlClockSet proc
	mov	r2,#&33F0
	push	{r4-r7}
	mov	r7,#&40
	mov	r6,#0
	ldr	r4,[00009DE0]                                          ; [pc,#&A4]
	ldr	r1,[00009DE4]                                          ; [pc,#&A4]
	ldr	r3,[r4]
	orn	r5,r0,#3
	ands	r1,r3
	orr	r1,r1,#&800
	ands	r1,r5
	ands	r2,r0
	bic	r3,r3,#&400000
	ldr	r5,[00009DE8]                                          ; [pc,#&94]
	sub	sp,#8
	orr	r3,r3,#&800
	orrs	r2,r1
	str	r3,[r4]
	str	r7,[r5]
	str	r2,[r4]
	str	r6,[sp,#&4]
	ldr	r3,[sp,#&4]
	cmps	r3,#&F
	bhi	$00009D76

l00009D6A:
	ldr	r3,[sp,#&4]
	adds	r3,#1
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]
	cmps	r3,#&F
	bls	$00009D6A

l00009D76:
	and	r3,r0,#3
	ldr	r4,[00009DE0]                                          ; [pc,#&64]
	bic	r2,r2,#3
	orrs	r2,r3
	bic	r3,r2,#&7C00000
	and	r1,r0,#&7C00000
	str	r2,[r4]
	lsls	r4,r0,#&14
	orr	r1,r1,r3
	bmi	$00009DBE

l00009D94:
	mov	r3,#&8000
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]
	cbz	r3,$00009DBA

l00009D9E:
	ldr	r2,[00009DEC]                                          ; [pc,#&4C]
	ldr	r3,[r2]
	lsls	r0,r3,#&19
	bpl	$00009DAE

l00009DA6:
	b	$00009DBA

l00009DA8:
	ldr	r3,[r2]
	lsls	r3,r3,#&19
	bmi	$00009DBA

l00009DAE:
	ldr	r3,[sp,#&4]
	subs	r3,#1
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]
	cmps	r3,#0
	bne	$00009DA8

l00009DBA:
	bic	r1,r1,#&800

l00009DBE:
	mov	r3,#0
	ldr	r2,[00009DE0]                                          ; [pc,#&1C]
	str	r1,[r2]
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]
	cmps	r3,#&F
	bhi	$00009DD8

l00009DCC:
	ldr	r3,[sp,#&4]
	adds	r3,#1
	str	r3,[sp,#&4]
	ldr	r3,[sp,#&4]
	cmps	r3,#&F
	bls	$00009DCC

l00009DD8:
	add	sp,#8
	pop	{r4-r7}
	bx	lr
00009DDE                                           00 BF               ..
00009DE0 60 E0 0F 40 0F CC BF FF 58 E0 0F 40 50 E0 0F 40 `..@....X..@P..@

;; SysCtlClockGet: 00009DF0
;;   Called from:
;;     000099F6 (in SSIConfig)
;;     00009F72 (in UARTConfigSet)
;;     00009FB8 (in UARTConfigGet)
;;     0000A102 (in I2CMasterInit)
SysCtlClockGet proc
	ldr	r3,[00009E54]                                          ; [pc,#&60]
	ldr	r3,[r3]
	and	r2,r3,#&30
	cmps	r2,#&10
	beq	$00009E4E

l00009DFC:
	cmps	r2,#&20
	beq	$00009E4A

l00009E00:
	cbz	r2,$00009E06

l00009E02:
	mov	r0,#0

l00009E04:
	bx	lr

l00009E06:
	ldr	r2,[00009E58]                                          ; [pc,#&50]
	ubfx	r1,r3,#6,#4
	add.w	r2,r2,r1,lsl #2
	ldr	r0,[r2,#&30]

l00009E12:
	lsls	r2,r3,#&14
	bmi	$00009E3A

l00009E16:
	ldr	r2,[00009E5C]                                          ; [pc,#&44]
	ldr	r2,[r2]
	ubfx	r1,r2,#5,#9
	adds	r1,#2
	mul	r0,r0,r1
	and	r1,r2,#&1F
	adds	r1,#2
	udiv	r0,r0,r1
	lsls	r1,r2,#&11
	it	mi
	lsrsmi	r0,r0,#1

l00009E34:
	lsls	r1,r2,#&10
	it	mi
	lsrsmi	r0,r0,#2

l00009E3A:
	lsls	r2,r3,#9
	bpl	$00009E04

l00009E3E:
	ubfx	r3,r3,#&17,#4
	adds	r3,#1
	udiv	r0,r0,r3
	bx	lr

l00009E4A:
	ldr	r0,[00009E60]                                          ; [pc,#&14]
	b	$00009E12

l00009E4E:
	ldr	r0,[00009E64]                                          ; [pc,#&14]
	b	$00009E12
00009E52       00 BF 60 E0 0F 40 54 A5 00 00 64 E0 0F 40   ..`..@T...d..@
00009E60 70 38 39 00 C0 E1 E4 00                         p89.....       

;; SysCtlPWMClockSet: 00009E68
SysCtlPWMClockSet proc
	ldr	r2,[00009E78]                                          ; [pc,#&C]
	ldr	r3,[r2]
	bic	r3,r3,#&1E0000
	orrs	r0,r3
	str	r0,[r2]
	bx	lr
00009E76                   00 BF 60 E0 0F 40                   ..`..@   

;; SysCtlPWMClockGet: 00009E7C
SysCtlPWMClockGet proc
	ldr	r3,[00009E88]                                          ; [pc,#&8]
	ldr	r0,[r3]
	and	r0,r0,#&1E0000
	bx	lr
00009E86                   00 BF 60 E0 0F 40                   ..`..@   

;; SysCtlADCSpeedSet: 00009E8C
SysCtlADCSpeedSet proc
	push	{r4}
	ldr	r4,[00009EB8]                                          ; [pc,#&28]
	ldr	r1,[00009EBC]                                          ; [pc,#&28]
	ldr	r3,[r4]
	ldr	r2,[00009EC0]                                          ; [pc,#&28]
	bic	r3,r3,#&F00
	orrs	r3,r0
	str	r3,[r4]
	ldr	r3,[r1]
	pop	{r4}
	bic	r3,r3,#&F00
	orrs	r3,r0
	str	r3,[r1]
	ldr	r3,[r2]
	bic	r3,r3,#&F00
	orrs	r0,r3
	str	r0,[r2]
	bx	lr
00009EB6                   00 BF 00 E1 0F 40 10 E1 0F 40       .....@...@
00009EC0 20 E1 0F 40                                      ..@           

;; SysCtlADCSpeedGet: 00009EC4
SysCtlADCSpeedGet proc
	ldr	r3,[00009ED0]                                          ; [pc,#&8]
	ldr	r0,[r3]
	and	r0,r0,#&F00
	bx	lr
00009ECE                                           00 BF               ..
00009ED0 00 E1 0F 40                                     ...@           

;; SysCtlIOSCVerificationSet: 00009ED4
SysCtlIOSCVerificationSet proc
	ldr	r2,[00009EEC]                                          ; [pc,#&14]
	ldr	r3,[r2]
	cbnz	r0,$00009EE2

l00009EDA:
	bic	r3,r3,#8
	str	r3,[r2]
	bx	lr

l00009EE2:
	orr	r3,r3,#8
	str	r3,[r2]
	bx	lr
00009EEA                               00 BF 60 E0 0F 40           ..`..@

;; SysCtlMOSCVerificationSet: 00009EF0
SysCtlMOSCVerificationSet proc
	ldr	r2,[00009F08]                                          ; [pc,#&14]
	ldr	r3,[r2]
	cbnz	r0,$00009EFE

l00009EF6:
	bic	r3,r3,#4
	str	r3,[r2]
	bx	lr

l00009EFE:
	orr	r3,r3,#4
	str	r3,[r2]
	bx	lr
00009F06                   00 BF 60 E0 0F 40                   ..`..@   

;; SysCtlPLLVerificationSet: 00009F0C
SysCtlPLLVerificationSet proc
	ldr	r2,[00009F24]                                          ; [pc,#&14]
	ldr	r3,[r2]
	cbnz	r0,$00009F1A

l00009F12:
	bic	r3,r3,#&400
	str	r3,[r2]
	bx	lr

l00009F1A:
	orr	r3,r3,#&400
	str	r3,[r2]
	bx	lr
00009F22       00 BF 60 E0 0F 40                           ..`..@       

;; SysCtlClkVerificationClear: 00009F28
SysCtlClkVerificationClear proc
	mov	r1,#1
	mov	r2,#0
	ldr	r3,[00009F34]                                          ; [pc,#&4]
	str	r1,[r3]
	str	r2,[r3]
	bx	lr
00009F34             50 E1 0F 40                             P..@       

;; UARTParityModeSet: 00009F38
UARTParityModeSet proc
	ldr	r3,[r0,#&2C]
	bic	r3,r3,#&86
	orrs	r1,r3
	str	r1,[r0,#&2C]
	bx	lr

;; UARTParityModeGet: 00009F44
UARTParityModeGet proc
	ldr	r0,[r0,#&2C]
	and	r0,r0,#&86
	bx	lr

;; UARTConfigSet: 00009F4C
UARTConfigSet proc
	push	{r3-r7,lr}
	mov	r7,r1
	mov	r6,r2
	mov	r5,r0
	adds	r0,#&18

l00009F56:
	ldr	r4,[r0]
	ands	r4,r4,#8
	bne	$00009F56

l00009F5E:
	ldr	r3,[r5,#&2C]
	bic	r3,r3,#&10
	str	r3,[r5,#&2C]
	ldr	r2,[r5,#&30]
	bic	r2,r2,#&300
	bic	r2,r2,#1
	str	r2,[r5,#&30]
	bl	$00009DF0
	lsls	r3,r7,#4
	udiv	r2,r0,r3
	mls	r3,r3,r2,r0
	lsls	r3,r3,#3
	udiv	r3,r3,r7
	adds	r3,#1
	lsrs	r3,r3,#1
	str	r2,[r5,#&24]
	str	r3,[r5,#&28]
	str	r6,[r5,#&2C]
	str	r4,[r5,#&18]
	ldr	r3,[r5,#&2C]
	orr	r3,r3,#&10
	str	r3,[r5,#&2C]
	ldr	r3,[r5,#&30]
	orr	r3,r3,#&300
	orr	r3,r3,#1
	str	r3,[r5,#&30]
	pop	{r3-r7,pc}

;; UARTConfigGet: 00009FA8
UARTConfigGet proc
	push.w	{r4-r8,lr}
	ldr	r8,[r0,#&24]
	mov	r4,r0
	mov	r7,r1
	mov	r6,r2
	ldr	r5,[r0,#&28]
	bl	$00009DF0
	add.w	r5,r5,r8,lsl #6
	lsls	r0,r0,#2
	udiv	r0,r0,r5
	str	r0,[r7]
	ldr	r3,[r4,#&2C]
	and	r3,r3,#&EE
	str	r3,[r6]
	pop.w	{r4-r8,pc}

;; UARTEnable: 00009FD4
UARTEnable proc
	ldr	r3,[r0,#&2C]
	orr	r3,r3,#&10
	str	r3,[r0,#&2C]
	ldr	r3,[r0,#&30]
	orr	r3,r3,#&300
	orr	r3,r3,#1
	str	r3,[r0,#&30]
	bx	lr
00009FEA                               00 BF                       ..   

;; UARTDisable: 00009FEC
UARTDisable proc
	add	r2,r0,#&18

l00009FF0:
	ldr	r3,[r2]
	lsls	r3,r3,#&1C
	bmi	$00009FF0

l00009FF6:
	ldr	r3,[r0,#&2C]
	bic	r3,r3,#&10
	str	r3,[r0,#&2C]
	ldr	r3,[r0,#&30]
	bic	r3,r3,#&300
	bic	r3,r3,#1
	str	r3,[r0,#&30]
	bx	lr

;; UARTCharsAvail: 0000A00C
UARTCharsAvail proc
	ldr	r0,[r0,#&18]
	eor	r0,r0,#&10
	ubfx	r0,r0,#4,#1
	bx	lr

;; UARTSpaceAvail: 0000A018
UARTSpaceAvail proc
	ldr	r0,[r0,#&18]
	eor	r0,r0,#&20
	ubfx	r0,r0,#5,#1
	bx	lr

;; UARTCharNonBlockingGet: 0000A024
UARTCharNonBlockingGet proc
	ldr	r3,[r0,#&18]
	lsls	r3,r3,#&1B
	ite	pl
	ldrpl	r0,[r0]

l0000A02C:
	mov	r0,#&FFFFFFFF
	bx	lr
0000A032       00 BF                                       ..           

;; UARTCharGet: 0000A034
UARTCharGet proc
	add	r2,r0,#&18

l0000A038:
	ldr	r3,[r2]
	lsls	r3,r3,#&1B
	bmi	$0000A038

l0000A03E:
	ldr	r0,[r0]
	bx	lr
0000A042       00 BF                                       ..           

;; UARTCharNonBlockingPut: 0000A044
UARTCharNonBlockingPut proc
	ldr	r3,[r0,#&18]
	lsls	r3,r3,#&1A
	itte	pl
	strpl	r1,[r0]

l0000A04C:
	mov	r0,#1
	mov	r0,#0
	bx	lr
0000A052       00 BF                                       ..           

;; UARTCharPut: 0000A054
UARTCharPut proc
	add	r2,r0,#&18

l0000A058:
	ldr	r3,[r2]
	lsls	r3,r3,#&1A
	bmi	$0000A058

l0000A05E:
	str	r1,[r0]
	bx	lr
0000A062       00 BF                                       ..           

;; UARTBreakCtl: 0000A064
UARTBreakCtl proc
	ldr	r3,[r0,#&2C]
	cbnz	r1,$0000A070

l0000A068:
	bic	r3,r3,#1
	str	r3,[r0,#&2C]
	bx	lr

l0000A070:
	orr	r3,r3,#1
	str	r3,[r0,#&2C]
	bx	lr

;; UARTIntRegister: 0000A078
UARTIntRegister proc
	push	{r4,lr}
	ldr	r4,[0000A094]                                          ; [pc,#&18]
	cmps	r0,r4
	ite	eq
	moveq	r4,#&15

l0000A082:
	mov	r4,#&16
	mov	r0,r4
	bl	$00009504
	mov	r0,r4
	pop.w	{r4,lr}
	b	$000095DC
0000A094             00 C0 00 40                             ...@       

;; UARTIntUnregister: 0000A098
UARTIntUnregister proc
	push	{r4,lr}
	ldr	r4,[0000A0B4]                                          ; [pc,#&18]
	cmps	r0,r4
	ite	eq
	moveq	r4,#&15

l0000A0A2:
	mov	r4,#&16
	mov	r0,r4
	bl	$00009638
	mov	r0,r4
	pop.w	{r4,lr}
	b	$00009538
0000A0B4             00 C0 00 40                             ...@       

;; UARTIntEnable: 0000A0B8
UARTIntEnable proc
	ldr	r3,[r0,#&38]
	orrs	r1,r3
	str	r1,[r0,#&38]
	bx	lr

;; UARTIntDisable: 0000A0C0
UARTIntDisable proc
	ldr	r3,[r0,#&38]
	bic.w	r1,r3,r1
	str	r1,[r0,#&38]
	bx	lr
0000A0CA                               00 BF                       ..   

;; UARTIntStatus: 0000A0CC
;;   Called from:
;;     00008116 (in vUART_ISR)
UARTIntStatus proc
	cbnz	r1,$0000A0D2

l0000A0CE:
	ldr	r0,[r0,#&3C]
	bx	lr

l0000A0D2:
	ldr	r0,[r0,#&40]
	bx	lr
0000A0D6                   00 BF                               ..       

;; UARTIntClear: 0000A0D8
;;   Called from:
;;     00008120 (in vUART_ISR)
UARTIntClear proc
	str	r1,[r0,#&44]
	bx	lr

;; CPUcpsie: 0000A0DC
;;   Called from:
;;     000094FC (in IntMasterEnable)
CPUcpsie proc
	cps	#0
	bx	lr
0000A0E0 70 47 00 BF                                     pG..           

;; CPUcpsid: 0000A0E4
;;   Called from:
;;     00009500 (in IntMasterDisable)
CPUcpsid proc
	cps	#0
	bx	lr
0000A0E8                         70 47 00 BF                     pG..   

;; CPUwfi: 0000A0EC
;;   Called from:
;;     00009CE0 (in SysCtlSleep)
;;     00009CF0 (in SysCtlDeepSleep)
CPUwfi proc
	wfi
	bx	lr
0000A0F0 70 47 00 BF                                     pG..           

;; I2CMasterInit: 0000A0F4
;;   Called from:
;;     00009910 (in OSRAMInit)
I2CMasterInit proc
	push	{r3-r5,lr}
	mov	r5,r1
	ldr	r2,[r0,#&20]
	mov	r4,r0
	orr	r2,r2,#&10
	str	r2,[r0,#&20]
	bl	$00009DF0
	ldr	r3,[0000A120]                                          ; [pc,#&18]
	ldr	r2,[0000A124]                                          ; [pc,#&18]
	subs	r0,#1
	cmps	r5,#1
	it	eq
	moveq	r3,r2

l0000A112:
	add	r1,r0,r3
	udiv	r1,r1,r3
	subs	r1,#1
	str	r1,[r4,#&C]
	pop	{r3-r5,pc}
0000A11E                                           00 BF               ..
0000A120 80 84 1E 00 00 12 7A 00                         ......z.       

;; I2CSlaveInit: 0000A128
I2CSlaveInit proc
	push	{r4}
	mov	r4,#1
	sub	r2,r0,#&7E0
	ldr	r3,[r2]
	orr	r3,r3,#&20
	str	r3,[r2]
	str	r4,[r0,#&4]
	str	r1,[r0]
	pop	{r4}
	bx	lr

;; I2CMasterEnable: 0000A140
I2CMasterEnable proc
	ldr	r3,[r0,#&20]
	orr	r3,r3,#&10
	str	r3,[r0,#&20]
	bx	lr
0000A14A                               00 BF                       ..   

;; I2CSlaveEnable: 0000A14C
I2CSlaveEnable proc
	mov	r1,#1
	sub	r2,r0,#&7E0
	ldr	r3,[r2]
	orr	r3,r3,#&20
	str	r3,[r2]
	str	r1,[r0,#&4]
	bx	lr
0000A15E                                           00 BF               ..

;; I2CMasterDisable: 0000A160
I2CMasterDisable proc
	ldr	r3,[r0,#&20]
	bic	r3,r3,#&10
	str	r3,[r0,#&20]
	bx	lr
0000A16A                               00 BF                       ..   

;; I2CSlaveDisable: 0000A16C
I2CSlaveDisable proc
	mov	r3,#0
	sub	r2,r0,#&7E0
	str	r3,[r0,#&4]
	ldr	r3,[r2]
	bic	r3,r3,#&20
	str	r3,[r2]
	bx	lr
0000A17E                                           00 BF               ..

;; I2CIntRegister: 0000A180
I2CIntRegister proc
	push	{r3,lr}
	mov	r0,#&18
	bl	$00009504
	pop.w	{r3,lr}
	mov	r0,#&18
	b	$000095DC
0000A192       00 BF                                       ..           

;; I2CIntUnregister: 0000A194
I2CIntUnregister proc
	push	{r3,lr}
	mov	r0,#&18
	bl	$00009638
	pop.w	{r3,lr}
	mov	r0,#&18
	b	$00009538
0000A1A6                   00 BF                               ..       

;; I2CMasterIntEnable: 0000A1A8
I2CMasterIntEnable proc
	mov	r3,#1
	str	r3,[r0,#&10]
	bx	lr
0000A1AE                                           00 BF               ..

;; I2CSlaveIntEnable: 0000A1B0
I2CSlaveIntEnable proc
	mov	r3,#1
	str	r3,[r0,#&C]
	bx	lr
0000A1B6                   00 BF                               ..       

;; I2CMasterIntDisable: 0000A1B8
I2CMasterIntDisable proc
	mov	r3,#0
	str	r3,[r0,#&10]
	bx	lr
0000A1BE                                           00 BF               ..

;; I2CSlaveIntDisable: 0000A1C0
I2CSlaveIntDisable proc
	mov	r3,#0
	str	r3,[r0,#&C]
	bx	lr
0000A1C6                   00 BF                               ..       

;; I2CMasterIntStatus: 0000A1C8
;;   Called from:
;;     000096D4 (in OSRAMWriteArray)
;;     0000970C (in OSRAMWriteByte)
;;     00009742 (in OSRAMWriteFinal)
;;     00009768 (in OSRAMWriteFinal)
I2CMasterIntStatus proc
	cbnz	r1,$0000A1D4

l0000A1CA:
	ldr	r0,[r0,#&14]
	adds	r0,#0
	it	ne
	movne	r0,#1

l0000A1D2:
	bx	lr

l0000A1D4:
	ldr	r0,[r0,#&18]
	adds	r0,#0
	it	ne
	movne	r0,#1

l0000A1DC:
	bx	lr
0000A1DE                                           00 BF               ..

;; I2CSlaveIntStatus: 0000A1E0
I2CSlaveIntStatus proc
	cbnz	r1,$0000A1EC

l0000A1E2:
	ldr	r0,[r0,#&10]
	adds	r0,#0
	it	ne
	movne	r0,#1

l0000A1EA:
	bx	lr

l0000A1EC:
	ldr	r0,[r0,#&14]
	adds	r0,#0
	it	ne
	movne	r0,#1

l0000A1F4:
	bx	lr
0000A1F6                   00 BF                               ..       

;; I2CMasterIntClear: 0000A1F8
I2CMasterIntClear proc
	mov	r3,#1
	str	r3,[r0,#&1C]
	str	r3,[r0,#&18]
	bx	lr

;; I2CSlaveIntClear: 0000A200
I2CSlaveIntClear proc
	mov	r3,#1
	str	r3,[r0,#&18]
	bx	lr
0000A206                   00 BF                               ..       

;; I2CMasterSlaveAddrSet: 0000A208
;;   Called from:
;;     000096A8 (in OSRAMWriteFirst)
I2CMasterSlaveAddrSet proc
	orr	r2,r2,r1,lsl #1
	str	r2,[r0]
	bx	lr

;; I2CMasterBusy: 0000A210
I2CMasterBusy proc
	ldr	r0,[r0,#&4]
	and	r0,r0,#1
	bx	lr

;; I2CMasterBusBusy: 0000A218
I2CMasterBusBusy proc
	ldr	r0,[r0,#&4]
	ubfx	r0,r0,#6,#1
	bx	lr

;; I2CMasterControl: 0000A220
;;   Called from:
;;     000096BC (in OSRAMWriteFirst)
;;     000096F0 (in OSRAMWriteArray)
;;     0000972C (in OSRAMWriteByte)
;;     00009760 (in OSRAMWriteFinal)
I2CMasterControl proc
	str	r1,[r0,#&4]
	bx	lr

;; I2CMasterErr: 0000A224
I2CMasterErr proc
	ldr	r3,[r0,#&4]
	lsls	r2,r3,#&1F
	bmi	$0000A236

l0000A22A:
	ands	r0,r3,#2
	beq	$0000A238

l0000A230:
	and	r0,r3,#&1C
	bx	lr

l0000A236:
	mov	r0,#0

l0000A238:
	bx	lr
0000A23A                               00 BF                       ..   

;; I2CMasterDataPut: 0000A23C
;;   Called from:
;;     000096B0 (in OSRAMWriteFirst)
;;     000096E8 (in OSRAMWriteArray)
;;     00009720 (in OSRAMWriteByte)
;;     00009758 (in OSRAMWriteFinal)
I2CMasterDataPut proc
	str	r1,[r0,#&8]
	bx	lr

;; I2CMasterDataGet: 0000A240
I2CMasterDataGet proc
	ldr	r0,[r0,#&8]
	bx	lr

;; I2CSlaveStatus: 0000A244
I2CSlaveStatus proc
	ldr	r0,[r0,#&4]
	bx	lr

;; I2CSlaveDataPut: 0000A248
I2CSlaveDataPut proc
	str	r1,[r0,#&8]
	bx	lr

;; I2CSlaveDataGet: 0000A24C
I2CSlaveDataGet proc
	ldr	r0,[r0,#&8]
	bx	lr
0000A250 48 65 6C 6C 6F 00 00 00 43 68 65 63 6B 00 00 00 Hello...Check...
0000A260 50 72 69 6E 74 00 00 00 53 68 6F 75 6C 64 20 6E Print...Should n
0000A270 6F 74 20 62 65 20 74 68 65 72 65 00 49 44 4C 45 ot be there.IDLE
0000A280 00 00 00 00                                     ....           
xFlashRates.4473		; 0000A284
	db	0x96, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00
	db	0x2C, 0x01, 0x00, 0x00, 0x5E, 0x01, 0x00, 0x00, 0x90, 0x01, 0x00, 0x00, 0xC2, 0x01, 0x00, 0x00
	db	0xF4, 0x01, 0x00, 0x00
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
