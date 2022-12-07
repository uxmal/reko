;;; Segment .text (00008000)

;; NmiSR: 00008000
NmiSR proc
E7FE           	b	NmiSR
00008002       00 BF                                       ..            

;; FaultISR: 00008004
FaultISR proc
E7FE           	b	FaultISR
00008006                   00 BF                               ..        

;; ResetISR: 00008008
ResetISR proc
4B08           	ldr	r3,[0000802C]                           ; [pc,#&20]
4809           	ldr	r0,[00008030]                           ; [pc,#&24]
4283           	cmps	r3,r0
D20A           	bhs	$00008026

l00008010:
43DA           	mvns	r2,r3
2100           	mov	r1,#0
4402           	adds	r2,r0
F022 0203     	bic	r2,r2,#3
3204           	adds	r2,#4
441A           	adds	r2,r3

l0000801E:
F843 1B04     	str	r1,[r3],#&4
4293           	cmps	r3,r2
D1FB           	bne	$0000801E

l00008026:
F000 B83B     	b	Main
0000802A                               00 BF 60 01 00 20           ..`.. 
00008030 80 08 00 20                                     ...             

;; raise: 00008034
raise proc
E7FE           	b	raise
00008036                   00 BF                               ..        

;; vPrintTask: 00008038
vPrintTask proc
B530           	push	{r4-r5,lr}
2400           	mov	r4,#0
4D09           	ldr	r5,[00008064]                           ; [pc,#&24]
B083           	sub	sp,#&C

l00008040:
A901           	add	r1,sp,#4
3401           	adds	r4,#1
2300           	mov	r3,#0
F04F 32FF     	mov	r2,#&FFFFFFFF
6828           	ldr	r0,[r5]
F000 FD8E     	bl	MPU_xQueueGenericReceive
F001 FB96     	bl	OSRAMClear
F004 0201     	and	r2,r4,#1
F004 013F     	and	r1,r4,#&3F
9801           	ldr	r0,[sp,#&4]
F001 FBB5     	bl	OSRAMStringDraw
E7ED           	b	$00008040
00008064             80 08 00 20                             ...         

;; vCheckTask: 00008068
vCheckTask proc
B530           	push	{r4-r5,lr}
4B0B           	ldr	r3,[00008098]                           ; [pc,#&2C]
B083           	sub	sp,#&C
9301           	str	r3,[sp,#&4]
F000 FC48     	bl	MPU_xTaskGetTickCount
AC02           	add	r4,sp,#8
4D09           	ldr	r5,[0000809C]                           ; [pc,#&24]
F844 0D08     	str	r0,[r4,-#&8]!

l0000807C:
4620           	mov	r0,r4
F241 3188     	mov	r1,#&1388
F000 FBF7     	bl	MPU_vTaskDelayUntil
2300           	mov	r3,#0
F04F 32FF     	mov	r2,#&FFFFFFFF
A901           	add	r1,sp,#4
6828           	ldr	r0,[r5]
F000 FD28     	bl	MPU_xQueueGenericSend
E7F2           	b	$0000807C
00008096                   00 BF 50 A2 00 00 80 08 00 20       ..P...... 

;; Main: 000080A0
;;   Called from:
;;     00008026 (in ResetISR)
Main proc
B500           	push	{lr}
2200           	mov	r2,#0
B083           	sub	sp,#&C
2104           	mov	r1,#4
2003           	mov	r0,#3
2400           	mov	r4,#0
F000 FCEC     	bl	MPU_xQueueGenericCreate
4B0F           	ldr	r3,[000080F0]                           ; [pc,#&3C]
6018           	str	r0,[r3]
4620           	mov	r0,r4
F001 FC1B     	bl	OSRAMInit
2203           	mov	r2,#3
4623           	mov	r3,r4
9200           	str	r2,[sp]
490C           	ldr	r1,[000080F4]                           ; [pc,#&30]
223B           	mov	r2,#&3B
9401           	str	r4,[sp,#&4]
480C           	ldr	r0,[000080F8]                           ; [pc,#&30]
F000 FB9E     	bl	MPU_xTaskCreate
2202           	mov	r2,#2
490B           	ldr	r1,[000080FC]                           ; [pc,#&2C]
4623           	mov	r3,r4
9200           	str	r2,[sp]
9401           	str	r4,[sp,#&4]
223B           	mov	r2,#&3B
4809           	ldr	r0,[00008100]                           ; [pc,#&24]
F000 FB95     	bl	MPU_xTaskCreate
F7F8 FC57     	bl	vTaskStartScheduler
4622           	mov	r2,r4
4621           	mov	r1,r4
4807           	ldr	r0,[00008104]                           ; [pc,#&1C]
F001 FB70     	bl	OSRAMStringDraw

l000080EC:
E7FE           	b	$000080EC
000080EE                                           00 BF               ..
000080F0 80 08 00 20 58 A2 00 00 69 80 00 00 60 A2 00 00 ... X...i...`...
00008100 39 80 00 00 68 A2 00 00                         9...h...        

;; vUART_ISR: 00008108
vUART_ISR proc
B570           	push	{r4-r6,lr}
2600           	mov	r6,#0
4D19           	ldr	r5,[00008174]                           ; [pc,#&64]
B082           	sub	sp,#8
2101           	mov	r1,#1
4628           	mov	r0,r5
9601           	str	r6,[sp,#&4]
F001 FFD9     	bl	UARTIntStatus
4604           	mov	r4,r0
4601           	mov	r1,r0
4628           	mov	r0,r5
F001 FFDA     	bl	UARTIntClear
06E2           	lsls	r2,r4,#&1B
D503           	bpl	$00008130

l00008128:
4B13           	ldr	r3,[00008178]                           ; [pc,#&4C]
681B           	ldr	r3,[r3]
065B           	lsls	r3,r3,#&19
D416           	bmi	$0000815E

l00008130:
06A0           	lsls	r0,r4,#&1A
D503           	bpl	$0000813C

l00008134:
4A11           	ldr	r2,[0000817C]                           ; [pc,#&44]
7813           	ldrb	r3,[r2]
2B7A           	cmps	r3,#&7A
D907           	bls	$0000814C

l0000813C:
9B01           	ldr	r3,[sp,#&4]
B11B           	cbz	r3,$00008148

l00008140:
F04F 5280     	mov	r2,#&10000000
4B0E           	ldr	r3,[00008180]                           ; [pc,#&38]
601A           	str	r2,[r3]

l00008148:
B002           	add	sp,#8
BD70           	pop	{r4-r6,pc}

l0000814C:
490A           	ldr	r1,[00008178]                           ; [pc,#&28]
6809           	ldr	r1,[r1]
0689           	lsls	r1,r1,#&1A
BF5C           	itt	pl
4907           	ldrpl	r1,[00008174]                         ; [pc,#&1C]

l00008156:
600B           	str	r3,[r1]
3301           	adds	r3,#1
7013           	strb	r3,[r2]
E7EE           	b	$0000813C

l0000815E:
682D           	ldr	r5,[r5]
4633           	mov	r3,r6
4630           	mov	r0,r6
AA01           	add	r2,sp,#4
F10D 0103     	add	r0,sp,#3
F88D 5003     	strb	r5,[sp,#&3]
F7F8 F973     	bl	xQueueGenericSendFromISR
E7DD           	b	$00008130
00008174             00 C0 00 40 18 C0 00 40 2C 02 00 20     ...@...@,.. 
00008180 04 ED 00 E0                                     ....            

;; vSetErrorLED: 00008184
;;   Called from:
;;     00008204 (in prvSetAndCheckRegisters)
vSetErrorLED proc
2101           	mov	r1,#1
2007           	mov	r0,#7
F000 BA34     	b	vParTestSetLED

;; prvSetAndCheckRegisters: 0000818C
;;   Called from:
;;     00008216 (in vApplicationIdleHook)
prvSetAndCheckRegisters proc
F04F 0B0A     	mov	fp,#&A
F10B 0001     	add	r0,fp,#1
F10B 0102     	add	r1,fp,#2
F10B 0203     	add	r2,fp,#3
F10B 0304     	add	r3,fp,#4
F10B 0405     	add	r4,fp,#5
F10B 0506     	add	r5,fp,#6
F10B 0607     	add	r6,fp,#7
F10B 0708     	add	r7,fp,#8
F10B 0809     	add	r8,fp,#9
F10B 090A     	add	r9,fp,#&A
F10B 0A0B     	add	r10,fp,#&B
F10B 0C0C     	add	ip,fp,#&C
F1BB 0F0A     	cmp	fp,#&A
D11C           	bne	$00008200

l000081C6:
280B           	cmps	r0,#&B
D11A           	bne	$00008200

l000081CA:
290C           	cmps	r1,#&C
D118           	bne	$00008200

l000081CE:
2A0D           	cmps	r2,#&D
D116           	bne	$00008200

l000081D2:
2B0E           	cmps	r3,#&E
D114           	bne	$00008200

l000081D6:
2C0F           	cmps	r4,#&F
D112           	bne	$00008200

l000081DA:
2D10           	cmps	r5,#&10
D110           	bne	$00008200

l000081DE:
2E11           	cmps	r6,#&11
D10E           	bne	$00008200

l000081E2:
2F12           	cmps	r7,#&12
D10C           	bne	$00008200

l000081E6:
F1B8 0F13     	cmp	r8,#&13
D109           	bne	$00008200

l000081EC:
F1B9 0F14     	cmp	r9,#&14
D106           	bne	$00008200

l000081F2:
F1BA 0F15     	cmp	r10,#&15
D103           	bne	$00008200

l000081F8:
F1BC 0F16     	cmp	ip,#&16
D100           	bne	$00008200

l000081FE:
4770           	bx	lr

l00008200:
B500           	push	{lr}
4906           	ldr	r1,[0000821C]                           ; [pc,#&18]
4788           	blx	r1
F85D EB04     	pop	lr
4770           	bx	lr
0000820C                                     70 47 00 BF             pG..

;; vApplicationIdleHook: 00008210
;;   Called from:
;;     0000852E (in prvIdleTask)
vApplicationIdleHook proc
B508           	push	{r3,lr}

l00008212:
F000 FE8B     	bl	vCoRoutineSchedule
F7FF FFB9     	bl	prvSetAndCheckRegisters
E7FA           	b	$00008212
0000821C                                     85 81 00 00             ....

;; PDCInit: 00008220
;;   Called from:
;;     000085DE (in vParTestInitialise)
PDCInit proc
B530           	push	{r4-r5,lr}
481A           	ldr	r0,[0000828C]                           ; [pc,#&68]
B083           	sub	sp,#&C
F001 FCA9     	bl	SysCtlPeripheralEnable
4819           	ldr	r0,[00008290]                           ; [pc,#&64]
F001 FCA6     	bl	SysCtlPeripheralEnable
2202           	mov	r2,#2
2134           	mov	r1,#&34
F04F 2040     	mov	r0,#&40004000
F000 FF68     	bl	GPIODirModeSet
2201           	mov	r2,#1
2108           	mov	r1,#8
F04F 2040     	mov	r0,#&40004000
F000 FF62     	bl	GPIODirModeSet
230A           	mov	r3,#&A
2202           	mov	r2,#2
2104           	mov	r1,#4
F04F 2040     	mov	r0,#&40004000
F000 FFB9     	bl	GPIOPadConfigSet
2408           	mov	r4,#8
2200           	mov	r2,#0
4D0E           	ldr	r5,[00008294]                           ; [pc,#&38]
4611           	mov	r1,r2
4B0E           	ldr	r3,[00008298]                           ; [pc,#&38]
4628           	mov	r0,r5
9400           	str	r4,[sp]
F001 FBC0     	bl	SSIConfig
4628           	mov	r0,r5
F001 FBE3     	bl	SSIEnable
4621           	mov	r1,r4
2200           	mov	r2,#0
F04F 2040     	mov	r0,#&40004000
F001 F8ED     	bl	GPIOPinWrite
4622           	mov	r2,r4
4621           	mov	r1,r4
F04F 2040     	mov	r0,#&40004000
B003           	add	sp,#&C
E8BD 4030     	pop.w	{r4-r5,lr}
F001 B8E4     	b	GPIOPinWrite
0000828C                                     10 00 00 10             ....
00008290 01 00 00 20 00 80 00 40 40 42 0F 00             ... ...@@B..    

;; PDCWrite: 0000829C
;;   Called from:
;;     000085EC (in vParTestInitialise)
;;     00008618 (in vParTestSetLED)
;;     00008656 (in vParTestToggleLED)
PDCWrite proc
B530           	push	{r4-r5,lr}
460D           	mov	r5,r1
4C0A           	ldr	r4,[000082CC]                           ; [pc,#&28]
B083           	sub	sp,#&C
F000 010F     	and	r1,r0,#&F
4620           	mov	r0,r4
F001 FBF5     	bl	SSIDataPut
4629           	mov	r1,r5
4620           	mov	r0,r4
F001 FBF1     	bl	SSIDataPut
4620           	mov	r0,r4
A901           	add	r1,sp,#4
F001 FBFD     	bl	SSIDataGet
A901           	add	r1,sp,#4
4620           	mov	r0,r4
F001 FBF9     	bl	SSIDataGet
B003           	add	sp,#&C
BD30           	pop	{r4-r5,pc}
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
F04F 31FF     	mov	r1,#&FFFFFFFF
2200           	mov	r2,#0
F100 0308     	add	r3,r0,#8
6081           	str	r1,[r0,#&8]
E880 000C     	stm	r0,{r2-r3}
60C3           	str	r3,[r0,#&C]
6103           	str	r3,[r0,#&10]
4770           	bx	lr
000082E6                   00 BF                               ..        

;; vListInitialiseItem: 000082E8
;;   Called from:
;;     00000756 (in prvInitialiseNewTask)
;;     0000075E (in prvInitialiseNewTask)
;;     00008E78 (in xCoRoutineCreate)
;;     00008E80 (in xCoRoutineCreate)
vListInitialiseItem proc
2300           	mov	r3,#0
6103           	str	r3,[r0,#&10]
4770           	bx	lr
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
E890 000C     	ldm	r0,{r2-r3}
B410           	push	{r4}
689C           	ldr	r4,[r3,#&8]
3201           	adds	r2,#1
608C           	str	r4,[r1,#&8]
689C           	ldr	r4,[r3,#&8]
604B           	str	r3,[r1,#&4]
6061           	str	r1,[r4,#&4]
6099           	str	r1,[r3,#&8]
BC10           	pop	{r4}
6108           	str	r0,[r1,#&10]
6002           	str	r2,[r0]
4770           	bx	lr

;; vListInsert: 0000830C
;;   Called from:
;;     00000890 (in prvAddCurrentTaskToDelayedList.isra.0)
;;     000008AC (in prvAddCurrentTaskToDelayedList.isra.0)
;;     00000FE6 (in vTaskPlaceOnEventList)
;;     00008F12 (in vCoRoutineAddToDelayedList)
;;     00008F22 (in vCoRoutineAddToDelayedList)
vListInsert proc
B430           	push	{r4-r5}
680D           	ldr	r5,[r1]
1C6B           	add	r3,r5,#1
D011           	beq	$00008338

l00008314:
F100 0208     	add	r2,r0,#8
E000           	b	$0000831C

l0000831A:
461A           	mov	r2,r3

l0000831C:
6853           	ldr	r3,[r2,#&4]
681C           	ldr	r4,[r3]
42A5           	cmps	r5,r4
D2FA           	bhs	$0000831A

l00008324:
6804           	ldr	r4,[r0]
604B           	str	r3,[r1,#&4]
3401           	adds	r4,#1
6099           	str	r1,[r3,#&8]
608A           	str	r2,[r1,#&8]
6051           	str	r1,[r2,#&4]
6108           	str	r0,[r1,#&10]
6004           	str	r4,[r0]
BC30           	pop	{r4-r5}
4770           	bx	lr

l00008338:
6902           	ldr	r2,[r0,#&10]
6853           	ldr	r3,[r2,#&4]
E7F2           	b	$00008324
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
6902           	ldr	r2,[r0,#&10]
6843           	ldr	r3,[r0,#&4]
6881           	ldr	r1,[r0,#&8]
B410           	push	{r4}
6099           	str	r1,[r3,#&8]
6854           	ldr	r4,[r2,#&4]
6881           	ldr	r1,[r0,#&8]
42A0           	cmps	r0,r4
604B           	str	r3,[r1,#&4]
BF08           	it	eq
6051           	streq	r1,[r2,#&4]

l00008356:
2100           	mov	r1,#0
6813           	ldr	r3,[r2]
6101           	str	r1,[r0,#&10]
1E58           	sub	r0,r3,#1
6010           	str	r0,[r2]
BC10           	pop	{r4}
4770           	bx	lr

;; xQueueCRSend: 00008364
;;   Called from:
;;     00008724 (in prvFixedDelayCoRoutine)
;;     00008758 (in prvFixedDelayCoRoutine)
xQueueCRSend proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4614           	mov	r4,r2
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
F000 F8FC     	bl	vPortEnterCritical
6BAA           	ldr	r2,[r5,#&38]
6BEB           	ldr	r3,[r5,#&3C]
429A           	cmps	r2,r3
D014           	beq	$000083B2

l00008388:
F000 F912     	bl	vPortExitCritical
2000           	mov	r0,#0
F380 8811     	msr	cpsr,r0
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
6BAA           	ldr	r2,[r5,#&38]
6BEB           	ldr	r3,[r5,#&3C]
429A           	cmps	r2,r3
D30A           	blo	$000083C0

l000083AA:
2300           	mov	r3,#0
F383 8811     	msr	cpsr,r3
BD70           	pop	{r4-r6,pc}

l000083B2:
F000 F8FD     	bl	vPortExitCritical
B97C           	cbnz	r4,$000083D8

l000083B8:
F384 8811     	msr	cpsr,r4
4620           	mov	r0,r4
BD70           	pop	{r4-r6,pc}

l000083C0:
4602           	mov	r2,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FE91     	bl	prvCopyDataToQueue
6A6B           	ldr	r3,[r5,#&24]
B97B           	cbnz	r3,$000083EE

l000083CE:
2001           	mov	r0,#1
2300           	mov	r3,#0
F383 8811     	msr	cpsr,r3
BD70           	pop	{r4-r6,pc}

l000083D8:
F105 0110     	add	r1,r5,#&10
4620           	mov	r0,r4
F000 FD87     	bl	vCoRoutineAddToDelayedList
2300           	mov	r3,#0
F383 8811     	msr	cpsr,r3
F06F 0003     	mvn	r0,#3
BD70           	pop	{r4-r6,pc}

l000083EE:
F105 0024     	add	r0,r5,#&24
F000 FE4F     	bl	xCoRoutineRemoveFromEventList
2800           	cmps	r0,#0
D0E9           	beq	$000083CE

l000083FA:
F06F 0004     	mvn	r0,#4
E7D4           	b	$000083AA

;; xQueueCRReceive: 00008400
;;   Called from:
;;     0000869E (in prvFlashCoRoutine)
;;     000086C0 (in prvFlashCoRoutine)
xQueueCRReceive proc
B538           	push	{r3-r5,lr}
4604           	mov	r4,r0
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
6B85           	ldr	r5,[r0,#&38]
B92D           	cbnz	r5,$00008424

l00008418:
2A00           	cmps	r2,#0
D136           	bne	$0000848A

l0000841C:
F382 8811     	msr	cpsr,r2
4610           	mov	r0,r2
BD38           	pop	{r3-r5,pc}

l00008424:
2300           	mov	r3,#0
F383 8811     	msr	cpsr,r3
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
6B82           	ldr	r2,[r0,#&38]
B922           	cbnz	r2,$00008448

l0000843E:
4610           	mov	r0,r2

l00008440:
2300           	mov	r3,#0
F383 8811     	msr	cpsr,r3
BD38           	pop	{r3-r5,pc}

l00008448:
4608           	mov	r0,r1
6C22           	ldr	r2,[r4,#&40]
68E1           	ldr	r1,[r4,#&C]
6863           	ldr	r3,[r4,#&4]
4411           	adds	r1,r2
4299           	cmps	r1,r3
6BA3           	ldr	r3,[r4,#&38]
60E1           	str	r1,[r4,#&C]
BF28           	it	hs
6821           	ldrhs	r1,[r4]

l0000845C:
F103 33FF     	add	r3,r3,#&FFFFFFFF
63A3           	str	r3,[r4,#&38]
BF28           	it	hs
60E1           	strhs	r1,[r4,#&C]

l00008466:
F002 F8AD     	bl	memcpy
6923           	ldr	r3,[r4,#&10]
B923           	cbnz	r3,$00008478

l0000846E:
2001           	mov	r0,#1
2300           	mov	r3,#0
F383 8811     	msr	cpsr,r3
BD38           	pop	{r3-r5,pc}

l00008478:
F104 0010     	add	r0,r4,#&10
F000 FE0A     	bl	xCoRoutineRemoveFromEventList
2800           	cmps	r0,#0
D0F4           	beq	$0000846E

l00008484:
F06F 0004     	mvn	r0,#4
E7DA           	b	$00008440

l0000848A:
F100 0124     	add	r1,r0,#&24
4610           	mov	r0,r2
F000 FD2E     	bl	vCoRoutineAddToDelayedList
F385 8811     	msr	cpsr,r5
F06F 0003     	mvn	r0,#3
BD38           	pop	{r3-r5,pc}
0000849E                                           00 BF               ..

;; xQueueCRSendFromISR: 000084A0
xQueueCRSendFromISR proc
B570           	push	{r4-r6,lr}
6BC3           	ldr	r3,[r0,#&3C]
6B86           	ldr	r6,[r0,#&38]
4615           	mov	r5,r2
429E           	cmps	r6,r3
D301           	blo	$000084B0

l000084AC:
4628           	mov	r0,r5
BD70           	pop	{r4-r6,pc}

l000084B0:
2200           	mov	r2,#0
4604           	mov	r4,r0
F7F7 FE1A     	bl	prvCopyDataToQueue
2D00           	cmps	r5,#0
D1F7           	bne	$000084AC

l000084BC:
6A63           	ldr	r3,[r4,#&24]
2B00           	cmps	r3,#0
D0F4           	beq	$000084AC

l000084C2:
F104 0024     	add	r0,r4,#&24
F000 FDE5     	bl	xCoRoutineRemoveFromEventList
1C05           	add	r5,r0,#0
BF18           	it	ne
2501           	movne	r5,#1

l000084D0:
E7EC           	b	$000084AC
000084D2       00 BF                                       ..            

;; xQueueCRReceiveFromISR: 000084D4
xQueueCRReceiveFromISR proc
B5F8           	push	{r3-r7,lr}
6B83           	ldr	r3,[r0,#&38]
B1E3           	cbz	r3,$00008514

l000084DA:
68C3           	ldr	r3,[r0,#&C]
F8D0 E040     	ldr	lr,[r0,#&40]
6844           	ldr	r4,[r0,#&4]
4473           	adds	r3,lr
42A3           	cmps	r3,r4
460E           	mov	r6,r1
4604           	mov	r4,r0
4615           	mov	r5,r2
6B87           	ldr	r7,[r0,#&38]
60C3           	str	r3,[r0,#&C]
BF28           	it	hs
6803           	ldrhs	r3,[r0]

l000084F4:
F107 37FF     	add	r7,r7,#&FFFFFFFF
BF28           	it	hs
60C3           	strhs	r3,[r0,#&C]

l000084FC:
4619           	mov	r1,r3
4672           	mov	r2,lr
4630           	mov	r0,r6
63A7           	str	r7,[r4,#&38]
F002 F85E     	bl	memcpy
682B           	ldr	r3,[r5]
B90B           	cbnz	r3,$00008510

l0000850C:
6923           	ldr	r3,[r4,#&10]
B91B           	cbnz	r3,$00008518

l00008510:
2001           	mov	r0,#1
BDF8           	pop	{r3-r7,pc}

l00008514:
4618           	mov	r0,r3
BDF8           	pop	{r3-r7,pc}

l00008518:
F104 0010     	add	r0,r4,#&10
F000 FDBA     	bl	xCoRoutineRemoveFromEventList
2800           	cmps	r0,#0
D0F5           	beq	$00008510

l00008524:
2001           	mov	r0,#1
6028           	str	r0,[r5]
BDF8           	pop	{r3-r7,pc}
0000852A                               00 BF                       ..    

;; prvIdleTask: 0000852C
prvIdleTask proc
B508           	push	{r3,lr}

l0000852E:
F7FF FE6F     	bl	vApplicationIdleHook
E7FC           	b	$0000852E

;; xTaskNotifyStateClear: 00008534
;;   Called from:
;;     00008A6C (in MPU_xTaskNotifyStateClear)
xTaskNotifyStateClear proc
B538           	push	{r3-r5,lr}
B178           	cbz	r0,$00008558

l00008538:
4604           	mov	r4,r0

l0000853A:
F000 F81D     	bl	vPortEnterCritical
F894 3064     	ldrb	r3,[r4,#&64]
2B02           	cmps	r3,#2
BF05           	ittet	eq
2300           	moveq	r3,#0

l00008548:
2501           	mov	r5,#1
2500           	mov	r5,#0
F884 3064     	strb	r3,[r4,#&64]
F000 F82E     	bl	vPortExitCritical
4628           	mov	r0,r5
BD38           	pop	{r3-r5,pc}

l00008558:
4B01           	ldr	r3,[00008560]                           ; [pc,#&4]
685C           	ldr	r4,[r3,#&4]
E7ED           	b	$0000853A
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
F3EF 8014     	mrs	r0,cpsr
F010 0F01     	tst	r0,#1
BF1A           	itte	ne
2000           	movne	r0,#0

l00008570:
DF02           	svc	#2
2001           	mov	r0,#1
4770           	bx	lr
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
B508           	push	{r3,lr}
F7FF FFF3     	bl	xPortRaisePrivilege
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
4A07           	ldr	r2,[000085AC]                           ; [pc,#&1C]
2801           	cmps	r0,#1
6813           	ldr	r3,[r2]
F103 0301     	add	r3,r3,#1
6013           	str	r3,[r2]
D005           	beq	$000085A8

l0000859C:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l000085A8:
BD08           	pop	{r3,pc}
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
B508           	push	{r3,lr}
F7FF FFD7     	bl	xPortRaisePrivilege
4A08           	ldr	r2,[000085D8]                           ; [pc,#&20]
6813           	ldr	r3,[r2]
3B01           	subs	r3,#1
6013           	str	r3,[r2]
B90B           	cbnz	r3,$000085C4

l000085C0:
F383 8811     	msr	cpsr,r3

l000085C4:
2801           	cmps	r0,#1
D005           	beq	$000085D4

l000085C8:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l000085D4:
BD08           	pop	{r3,pc}
000085D6                   00 BF BC 00 00 20                   .....     

;; vParTestInitialise: 000085DC
vParTestInitialise proc
B508           	push	{r3,lr}
F7FF FE1F     	bl	PDCInit
4B03           	ldr	r3,[000085F0]                           ; [pc,#&C]
2005           	mov	r0,#5
7819           	ldrb	r1,[r3]
E8BD 4008     	pop.w	{r3,lr}
F7FF BE56     	b	PDCWrite
000085F0 F4 07 00 20                                     ...             

;; vParTestSetLED: 000085F4
;;   Called from:
;;     00008188 (in vSetErrorLED)
vParTestSetLED proc
B538           	push	{r3-r5,lr}
4604           	mov	r4,r0
460D           	mov	r5,r1
F000 F961     	bl	MPU_vTaskSuspendAll
2C07           	cmps	r4,#7
D80C           	bhi	$0000861C

l00008602:
2301           	mov	r3,#1
FA03 F004     	lsl	r0,r3,r4
4B08           	ldr	r3,[0000862C]                           ; [pc,#&20]
B2C0           	uxtb	r0,r0
781A           	ldrb	r2,[r3]
B14D           	cbz	r5,$00008624

l00008610:
4310           	orrs	r0,r2
7018           	strb	r0,[r3]

l00008614:
7819           	ldrb	r1,[r3]
2005           	mov	r0,#5
F7FF FE40     	bl	PDCWrite

l0000861C:
E8BD 4038     	pop.w	{r3-r5,lr}
F000 B95E     	b	MPU_xTaskResumeAll

l00008624:
EA22 0000     	bic.w	r0,r2,r0
7018           	strb	r0,[r3]
E7F3           	b	$00008614
0000862C                                     F4 07 00 20             ... 

;; vParTestToggleLED: 00008630
;;   Called from:
;;     00008692 (in prvFlashCoRoutine)
vParTestToggleLED proc
B510           	push	{r4,lr}
4604           	mov	r4,r0
F000 F944     	bl	MPU_vTaskSuspendAll
2C07           	cmps	r4,#7
D80E           	bhi	$0000865A

l0000863C:
2201           	mov	r2,#1
4B0B           	ldr	r3,[0000866C]                           ; [pc,#&2C]
FA02 F004     	lsl	r0,r2,r4
7819           	ldrb	r1,[r3]
B2C2           	uxtb	r2,r0
420A           	tst	r2,r1
D10A           	bne	$00008662

l0000864C:
7819           	ldrb	r1,[r3]
430A           	orrs	r2,r1
701A           	strb	r2,[r3]

l00008652:
7819           	ldrb	r1,[r3]
2005           	mov	r0,#5
F7FF FE21     	bl	PDCWrite

l0000865A:
E8BD 4010     	pop.w	{r4,lr}
F000 B93F     	b	MPU_xTaskResumeAll

l00008662:
781A           	ldrb	r2,[r3]
EA22 0000     	bic.w	r0,r2,r0
7018           	strb	r0,[r3]
E7F2           	b	$00008652
0000866C                                     F4 07 00 20             ... 

;; prvFlashCoRoutine: 00008670
prvFlashCoRoutine proc
B570           	push	{r4-r6,lr}
8E83           	ldrh	r3,[r0,#&34]
B082           	sub	sp,#8
F5B3 7FE1     	cmp	r3,#&1C2
4604           	mov	r4,r0
D01B           	beq	$000086B6

l0000867E:
F240 12C3     	mov	r2,#&1C3
4293           	cmps	r3,r2
D002           	beq	$0000868C

l00008686:
B323           	cbz	r3,$000086D2

l00008688:
B002           	add	sp,#8
BD70           	pop	{r4-r6,pc}

l0000868C:
4D14           	ldr	r5,[000086E0]                           ; [pc,#&50]
AE01           	add	r6,sp,#4

l00008690:
9801           	ldr	r0,[sp,#&4]
F7FF FFCD     	bl	vParTestToggleLED

l00008696:
F04F 32FF     	mov	r2,#&FFFFFFFF
4631           	mov	r1,r6
6828           	ldr	r0,[r5]
F7FF FEAF     	bl	xQueueCRReceive
1D02           	add	r2,r0,#4
D018           	beq	$000086D8

l000086A6:
1D43           	add	r3,r0,#5
D00E           	beq	$000086C8

l000086AA:
2801           	cmps	r0,#1
D0F0           	beq	$00008690

l000086AE:
2200           	mov	r2,#0
4B0C           	ldr	r3,[000086E4]                           ; [pc,#&30]
601A           	str	r2,[r3]
E7EF           	b	$00008696

l000086B6:
4D0A           	ldr	r5,[000086E0]                           ; [pc,#&28]
AE01           	add	r6,sp,#4
6828           	ldr	r0,[r5]
4631           	mov	r1,r6
2200           	mov	r2,#0
F7FF FE9E     	bl	xQueueCRReceive
1D43           	add	r3,r0,#5
D1F0           	bne	$000086AA

l000086C8:
F240 13C3     	mov	r3,#&1C3
86A3           	strh	r3,[r4,#&34]
B002           	add	sp,#8
BD70           	pop	{r4-r6,pc}

l000086D2:
4D03           	ldr	r5,[000086E0]                           ; [pc,#&C]
AE01           	add	r6,sp,#4
E7DE           	b	$00008696

l000086D8:
F44F 73E1     	mov	r3,#&1C2
86A3           	strh	r3,[r4,#&34]
E7D3           	b	$00008688
000086E0 F8 07 00 20 C0 00 00 20                         ... ...         

;; prvFixedDelayCoRoutine: 000086E8
prvFixedDelayCoRoutine proc
B510           	push	{r4,lr}
8E83           	ldrh	r3,[r0,#&34]
B082           	sub	sp,#8
F5B3 7FC1     	cmp	r3,#&182
4604           	mov	r4,r0
9101           	str	r1,[sp,#&4]
D02B           	beq	$00008750

l000086F8:
D926           	bls	$00008748

l000086FA:
F240 1283     	mov	r2,#&183
4293           	cmps	r3,r2
D109           	bne	$00008716

l00008702:
4B1D           	ldr	r3,[00008778]                           ; [pc,#&74]
9A01           	ldr	r2,[sp,#&4]
F853 0022     	ldr.w	r0,[r3,r2,lsl #2]
BB40           	cbnz	r0,$0000875E

l0000870C:
F44F 73CB     	mov	r3,#&196
86A3           	strh	r3,[r4,#&34]

l00008712:
B002           	add	sp,#8
BD10           	pop	{r4,pc}

l00008716:
F5B3 7FCB     	cmp	r3,#&196
D1FA           	bne	$00008712

l0000871C:
4B17           	ldr	r3,[0000877C]                           ; [pc,#&5C]
2200           	mov	r2,#0
6818           	ldr	r0,[r3]
A901           	add	r1,sp,#4
F7FF FE1E     	bl	xQueueCRSend
1D02           	add	r2,r0,#4
D020           	beq	$0000876E

l0000872C:
1D43           	add	r3,r0,#5
D01A           	beq	$00008766

l00008730:
2801           	cmps	r0,#1
D0E6           	beq	$00008702

l00008734:
2200           	mov	r2,#0
4B12           	ldr	r3,[00008780]                           ; [pc,#&48]
601A           	str	r2,[r3]
4B0F           	ldr	r3,[00008778]                           ; [pc,#&3C]
9A01           	ldr	r2,[sp,#&4]
F853 0022     	ldr.w	r0,[r3,r2,lsl #2]
2800           	cmps	r0,#0
D0E2           	beq	$0000870C

l00008746:
E00A           	b	$0000875E

l00008748:
2B00           	cmps	r3,#0
D0E7           	beq	$0000871C

l0000874C:
B002           	add	sp,#8
BD10           	pop	{r4,pc}

l00008750:
4B0A           	ldr	r3,[0000877C]                           ; [pc,#&28]
2200           	mov	r2,#0
6818           	ldr	r0,[r3]
A901           	add	r1,sp,#4
F7FF FE04     	bl	xQueueCRSend
E7E6           	b	$0000872C

l0000875E:
2100           	mov	r1,#0
F000 FBC6     	bl	vCoRoutineAddToDelayedList
E7D2           	b	$0000870C

l00008766:
F240 1383     	mov	r3,#&183
86A3           	strh	r3,[r4,#&34]
E7D1           	b	$00008712

l0000876E:
F44F 73C1     	mov	r3,#&182
86A3           	strh	r3,[r4,#&34]
E7CD           	b	$00008712
00008776                   00 BF 84 A2 00 00 F8 07 00 20       ......... 
00008780 C0 00 00 20                                     ...             

;; vStartFlashCoRoutines: 00008784
vStartFlashCoRoutines proc
2808           	cmps	r0,#8
BF28           	it	hs
2008           	movhs	r0,#8

l0000878A:
B570           	push	{r4-r6,lr}
2200           	mov	r2,#0
4605           	mov	r5,r0
2104           	mov	r1,#4
2001           	mov	r0,#1
F000 F978     	bl	MPU_xQueueGenericCreate
4B0A           	ldr	r3,[000087C4]                           ; [pc,#&28]
6018           	str	r0,[r3]
B188           	cbz	r0,$000087C2

l0000879E:
B14D           	cbz	r5,$000087B4

l000087A0:
2400           	mov	r4,#0
4E09           	ldr	r6,[000087C8]                           ; [pc,#&24]

l000087A4:
4622           	mov	r2,r4
2100           	mov	r1,#0
3401           	adds	r4,#1
4630           	mov	r0,r6
F000 FB48     	bl	xCoRoutineCreate
42AC           	cmps	r4,r5
D1F7           	bne	$000087A4

l000087B4:
2200           	mov	r2,#0
E8BD 4070     	pop.w	{r4-r6,lr}
2101           	mov	r1,#1
4803           	ldr	r0,[000087CC]                           ; [pc,#&C]
F000 BB3F     	b	xCoRoutineCreate

l000087C2:
BD70           	pop	{r4-r6,pc}
000087C4             F8 07 00 20 E9 86 00 00 71 86 00 00     ... ....q...

;; xAreFlashCoRoutinesStillRunning: 000087D0
xAreFlashCoRoutinesStillRunning proc
4B01           	ldr	r3,[000087D8]                           ; [pc,#&4]
6818           	ldr	r0,[r3]
4770           	bx	lr
000087D6                   00 BF C0 00 00 20                   .....     

;; MPU_xTaskCreateRestricted: 000087DC
MPU_xTaskCreateRestricted proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FEBF     	bl	xPortRaisePrivilege
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 F896     	bl	xTaskCreateRestricted
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008802

l000087F6:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008802:
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008806                   00 BF                               ..        

;; MPU_xTaskCreate: 00008808
;;   Called from:
;;     000080C8 (in Main)
;;     000080DA (in Main)
MPU_xTaskCreate proc
E92D 47F0     	push.w	{r4-r10,lr}
B082           	sub	sp,#8
4605           	mov	r5,r0
4688           	mov	r8,r1
4691           	mov	r9,r2
469A           	mov	r10,r3
9F0A           	ldr	r7,[sp,#&28]
9E0B           	ldr	r6,[sp,#&2C]
F7FF FEA3     	bl	xPortRaisePrivilege
4653           	mov	r3,r10
4604           	mov	r4,r0
9700           	str	r7,[sp]
9601           	str	r6,[sp,#&4]
464A           	mov	r2,r9
4641           	mov	r1,r8
4628           	mov	r0,r5
F7F8 F842     	bl	xTaskCreate
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008842

l00008836:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008842:
4618           	mov	r0,r3
B002           	add	sp,#8
E8BD 87F0     	pop.w	{r4-r10,pc}
0000884A                               00 BF                       ..    

;; MPU_vTaskAllocateMPURegions: 0000884C
MPU_vTaskAllocateMPURegions proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FE87     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 F888     	bl	vTaskAllocateMPURegions
2C01           	cmps	r4,#1
D005           	beq	$00008870

l00008864:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008870:
BD70           	pop	{r4-r6,pc}
00008872       00 BF                                       ..            

;; MPU_vTaskDelayUntil: 00008874
;;   Called from:
;;     00008082 (in vCheckTask)
MPU_vTaskDelayUntil proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FE73     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 FB7C     	bl	vTaskDelayUntil
2C01           	cmps	r4,#1
D005           	beq	$00008898

l0000888C:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008898:
BD70           	pop	{r4-r6,pc}
0000889A                               00 BF                       ..    

;; MPU_vTaskDelay: 0000889C
MPU_vTaskDelay proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FE60     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FB4E     	bl	vTaskDelay
2C01           	cmps	r4,#1
D005           	beq	$000088BC

l000088B0:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l000088BC:
BD38           	pop	{r3-r5,pc}
000088BE                                           00 BF               ..

;; MPU_vTaskSuspendAll: 000088C0
;;   Called from:
;;     000085FA (in vParTestSetLED)
;;     00008634 (in vParTestToggleLED)
MPU_vTaskSuspendAll proc
B510           	push	{r4,lr}
F7FF FE4F     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
F7F8 F8A0     	bl	vTaskSuspendAll
2C01           	cmps	r4,#1
D005           	beq	$000088DC

l000088D0:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l000088DC:
BD10           	pop	{r4,pc}
000088DE                                           00 BF               ..

;; MPU_xTaskResumeAll: 000088E0
;;   Called from:
;;     00008620 (in vParTestSetLED)
;;     0000865E (in vParTestToggleLED)
MPU_xTaskResumeAll proc
B510           	push	{r4,lr}
F7FF FE3F     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
F7F8 FAC0     	bl	xTaskResumeAll
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$000088FE

l000088F2:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l000088FE:
4618           	mov	r0,r3
BD10           	pop	{r4,pc}
00008902       00 BF                                       ..            

;; MPU_xTaskGetTickCount: 00008904
;;   Called from:
;;     00008070 (in vCheckTask)
;;     00008F82 (in vCoRoutineSchedule)
MPU_xTaskGetTickCount proc
B510           	push	{r4,lr}
F7FF FE2D     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
F7F8 F888     	bl	xTaskGetTickCount
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008922

l00008916:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008922:
4618           	mov	r0,r3
BD10           	pop	{r4,pc}
00008926                   00 BF                               ..        

;; MPU_uxTaskGetNumberOfTasks: 00008928
MPU_uxTaskGetNumberOfTasks proc
B510           	push	{r4,lr}
F7FF FE1B     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
F7F8 F882     	bl	uxTaskGetNumberOfTasks
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008946

l0000893A:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008946:
4618           	mov	r0,r3
BD10           	pop	{r4,pc}
0000894A                               00 BF                       ..    

;; MPU_pcTaskGetName: 0000894C
MPU_pcTaskGetName proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FE08     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 F874     	bl	pcTaskGetName
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$0000896E

l00008962:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l0000896E:
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008972       00 BF                                       ..            

;; MPU_vTaskSetTimeOutState: 00008974
MPU_vTaskSetTimeOutState proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FDF4     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FBE0     	bl	vTaskSetTimeOutState
2C01           	cmps	r4,#1
D005           	beq	$00008994

l00008988:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008994:
BD38           	pop	{r3-r5,pc}
00008996                   00 BF                               ..        

;; MPU_xTaskCheckForTimeOut: 00008998
MPU_xTaskCheckForTimeOut proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FDE1     	bl	xPortRaisePrivilege
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FBD6     	bl	xTaskCheckForTimeOut
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$000089BE

l000089B2:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l000089BE:
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
000089C2       00 BF                                       ..            

;; MPU_xTaskGenericNotify: 000089C4
MPU_xTaskGenericNotify proc
E92D 41F0     	push.w	{r4-r8,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4617           	mov	r7,r2
4698           	mov	r8,r3
F7FF FDC8     	bl	xPortRaisePrivilege
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 F83B     	bl	xTaskGenericNotify
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$000089F4

l000089E8:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l000089F4:
4618           	mov	r0,r3
E8BD 81F0     	pop.w	{r4-r8,pc}
000089FA                               00 BF                       ..    

;; MPU_xTaskNotifyWait: 000089FC
MPU_xTaskNotifyWait proc
E92D 41F0     	push.w	{r4-r8,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4617           	mov	r7,r2
4698           	mov	r8,r3
F7FF FDAC     	bl	xPortRaisePrivilege
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 F8DD     	bl	xTaskNotifyWait
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008A2C

l00008A20:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008A2C:
4618           	mov	r0,r3
E8BD 81F0     	pop.w	{r4-r8,pc}
00008A32       00 BF                                       ..            

;; MPU_ulTaskNotifyTake: 00008A34
MPU_ulTaskNotifyTake proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FD93     	bl	xPortRaisePrivilege
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 F95C     	bl	ulTaskNotifyTake
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008A5A

l00008A4E:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008A5A:
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008A5E                                           00 BF               ..

;; MPU_xTaskNotifyStateClear: 00008A60
MPU_xTaskNotifyStateClear proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FD7E     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7FF FD62     	bl	xTaskNotifyStateClear
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008A82

l00008A76:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008A82:
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008A86                   00 BF                               ..        

;; MPU_xQueueGenericCreate: 00008A88
;;   Called from:
;;     000080AC (in Main)
;;     00008794 (in vStartFlashCoRoutines)
MPU_xQueueGenericCreate proc
B5F8           	push	{r3-r7,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4617           	mov	r7,r2
F7FF FD68     	bl	xPortRaisePrivilege
463A           	mov	r2,r7
4604           	mov	r4,r0
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FE06     	bl	xQueueGenericCreate
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008AB2

l00008AA6:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008AB2:
4618           	mov	r0,r3
BDF8           	pop	{r3-r7,pc}
00008AB6                   00 BF                               ..        

;; MPU_xQueueGenericReset: 00008AB8
MPU_xQueueGenericReset proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FD51     	bl	xPortRaisePrivilege
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FDB2     	bl	xQueueGenericReset
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008ADE

l00008AD2:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008ADE:
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008AE2       00 BF                                       ..            

;; MPU_xQueueGenericSend: 00008AE4
;;   Called from:
;;     00008090 (in vCheckTask)
MPU_xQueueGenericSend proc
E92D 41F0     	push.w	{r4-r8,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4617           	mov	r7,r2
4698           	mov	r8,r3
F7FF FD38     	bl	xPortRaisePrivilege
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FB47     	bl	xQueueGenericSend
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008B14

l00008B08:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008B14:
4618           	mov	r0,r3
E8BD 81F0     	pop.w	{r4-r8,pc}
00008B1A                               00 BF                       ..    

;; MPU_uxQueueMessagesWaiting: 00008B1C
MPU_uxQueueMessagesWaiting proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FD20     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FC7E     	bl	uxQueueMessagesWaiting
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008B3E

l00008B32:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008B3E:
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008B42       00 BF                                       ..            

;; MPU_uxQueueSpacesAvailable: 00008B44
MPU_uxQueueSpacesAvailable proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FD0C     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FC74     	bl	uxQueueSpacesAvailable
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008B66

l00008B5A:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008B66:
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008B6A                               00 BF                       ..    

;; MPU_xQueueGenericReceive: 00008B6C
;;   Called from:
;;     0000804C (in vPrintTask)
MPU_xQueueGenericReceive proc
E92D 41F0     	push.w	{r4-r8,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4617           	mov	r7,r2
4698           	mov	r8,r3
F7FF FCF4     	bl	xPortRaisePrivilege
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F7 FBA7     	bl	xQueueGenericReceive
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008B9C

l00008B90:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008B9C:
4618           	mov	r0,r3
E8BD 81F0     	pop.w	{r4-r8,pc}
00008BA2       00 BF                                       ..            

;; MPU_xQueuePeekFromISR: 00008BA4
MPU_xQueuePeekFromISR proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FCDB     	bl	xPortRaisePrivilege
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FB76     	bl	xQueuePeekFromISR
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008BCA

l00008BBE:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008BCA:
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008BCE                                           00 BF               ..

;; MPU_xQueueGetMutexHolder: 00008BD0
MPU_xQueueGetMutexHolder proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FCC6     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FCEA     	bl	xQueueGetMutexHolder
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008BF2

l00008BE6:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008BF2:
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008BF6                   00 BF                               ..        

;; MPU_xQueueCreateMutex: 00008BF8
MPU_xQueueCreateMutex proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FCB2     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FD6A     	bl	xQueueCreateMutex
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008C1A

l00008C0E:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008C1A:
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008C1E                                           00 BF               ..

;; MPU_xQueueTakeMutexRecursive: 00008C20
MPU_xQueueTakeMutexRecursive proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FC9D     	bl	xPortRaisePrivilege
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FCD0     	bl	xQueueTakeMutexRecursive
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008C46

l00008C3A:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008C46:
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008C4A                               00 BF                       ..    

;; MPU_xQueueGiveMutexRecursive: 00008C4C
MPU_xQueueGiveMutexRecursive proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FC88     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FCD4     	bl	xQueueGiveMutexRecursive
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008C6E

l00008C62:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008C6E:
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008C72       00 BF                                       ..            

;; MPU_vQueueDelete: 00008C74
MPU_vQueueDelete proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FC74     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F7 FBE8     	bl	vQueueDelete
2C01           	cmps	r4,#1
D005           	beq	$00008C94

l00008C88:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008C94:
BD38           	pop	{r3-r5,pc}
00008C96                   00 BF                               ..        

;; MPU_pvPortMalloc: 00008C98
MPU_pvPortMalloc proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FC62     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD42     	bl	pvPortMalloc
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008CBA

l00008CAE:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008CBA:
4618           	mov	r0,r3
BD38           	pop	{r3-r5,pc}
00008CBE                                           00 BF               ..

;; MPU_vPortFree: 00008CC0
MPU_vPortFree proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FC4E     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD58     	bl	vPortFree
2C01           	cmps	r4,#1
D005           	beq	$00008CE0

l00008CD4:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008CE0:
BD38           	pop	{r3-r5,pc}
00008CE2       00 BF                                       ..            

;; MPU_vPortInitialiseBlocks: 00008CE4
MPU_vPortInitialiseBlocks proc
B510           	push	{r4,lr}
F7FF FC3D     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
F7F8 FD4A     	bl	vPortInitialiseBlocks
2C01           	cmps	r4,#1
D005           	beq	$00008D00

l00008CF4:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008D00:
BD10           	pop	{r4,pc}
00008D02       00 BF                                       ..            

;; MPU_xPortGetFreeHeapSize: 00008D04
MPU_xPortGetFreeHeapSize proc
B510           	push	{r4,lr}
F7FF FC2D     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
F7F8 FD42     	bl	xPortGetFreeHeapSize
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008D22

l00008D16:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008D22:
4618           	mov	r0,r3
BD10           	pop	{r4,pc}
00008D26                   00 BF                               ..        

;; MPU_xEventGroupCreate: 00008D28
MPU_xEventGroupCreate proc
B510           	push	{r4,lr}
F7FF FC1B     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
F7F8 FD3A     	bl	xEventGroupCreate
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008D46

l00008D3A:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008D46:
4618           	mov	r0,r3
BD10           	pop	{r4,pc}
00008D4A                               00 BF                       ..    

;; MPU_xEventGroupWaitBits: 00008D4C
MPU_xEventGroupWaitBits proc
E92D 43F0     	push.w	{r4-r9,lr}
B083           	sub	sp,#&C
4605           	mov	r5,r0
460E           	mov	r6,r1
4690           	mov	r8,r2
4699           	mov	r9,r3
9F0A           	ldr	r7,[sp,#&28]
F7FF FC02     	bl	xPortRaisePrivilege
464B           	mov	r3,r9
4604           	mov	r4,r0
9700           	str	r7,[sp]
4642           	mov	r2,r8
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 FD2A     	bl	xEventGroupWaitBits
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008D82

l00008D76:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008D82:
4618           	mov	r0,r3
B003           	add	sp,#&C
E8BD 83F0     	pop.w	{r4-r9,pc}
00008D8A                               00 BF                       ..    

;; MPU_xEventGroupClearBits: 00008D8C
MPU_xEventGroupClearBits proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FBE7     	bl	xPortRaisePrivilege
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD6A     	bl	xEventGroupClearBits
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008DB2

l00008DA6:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008DB2:
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008DB6                   00 BF                               ..        

;; MPU_xEventGroupSetBits: 00008DB8
MPU_xEventGroupSetBits proc
B570           	push	{r4-r6,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
F7FF FBD1     	bl	xPortRaisePrivilege
4631           	mov	r1,r6
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FD62     	bl	xEventGroupSetBits
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008DDE

l00008DD2:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008DDE:
4618           	mov	r0,r3
BD70           	pop	{r4-r6,pc}
00008DE2       00 BF                                       ..            

;; MPU_xEventGroupSync: 00008DE4
MPU_xEventGroupSync proc
E92D 41F0     	push.w	{r4-r8,lr}
4605           	mov	r5,r0
460E           	mov	r6,r1
4617           	mov	r7,r2
4698           	mov	r8,r3
F7FF FBB8     	bl	xPortRaisePrivilege
4643           	mov	r3,r8
4604           	mov	r4,r0
463A           	mov	r2,r7
4631           	mov	r1,r6
4628           	mov	r0,r5
F7F8 FD7B     	bl	xEventGroupSync
2C01           	cmps	r4,#1
4603           	mov	r3,r0
D005           	beq	$00008E14

l00008E08:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008E14:
4618           	mov	r0,r3
E8BD 81F0     	pop.w	{r4-r8,pc}
00008E1A                               00 BF                       ..    

;; MPU_vEventGroupDelete: 00008E1C
MPU_vEventGroupDelete proc
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
F7FF FBA0     	bl	xPortRaisePrivilege
4604           	mov	r4,r0
4628           	mov	r0,r5
F7F8 FDBC     	bl	vEventGroupDelete
2C01           	cmps	r4,#1
D005           	beq	$00008E3C

l00008E30:
F3EF 8014     	mrs	r0,cpsr
F040 0001     	orr	r0,r0,#1
F380 8814     	msr	cpsr,r0

l00008E3C:
BD38           	pop	{r3-r5,pc}
00008E3E                                           00 BF               ..

;; xCoRoutineCreate: 00008E40
;;   Called from:
;;     000087AC (in vStartFlashCoRoutines)
;;     000087BE (in vStartFlashCoRoutines)
xCoRoutineCreate proc
E92D 4FF8     	push.w	{r3-fp,lr}
4681           	mov	r9,r0
2038           	mov	r0,#&38
460D           	mov	r5,r1
4692           	mov	r10,r2
F7F8 FC6E     	bl	pvPortMalloc
2800           	cmps	r0,#0
D047           	beq	$00008EE4

l00008E54:
4F25           	ldr	r7,[00008EEC]                           ; [pc,#&94]
4604           	mov	r4,r0
683B           	ldr	r3,[r7]
B33B           	cbz	r3,$00008EAC

l00008E5C:
F107 0804     	add	r8,r7,#4

l00008E60:
2D01           	cmps	r5,#1
BF28           	it	hs
2501           	movhs	r5,#1

l00008E66:
2300           	mov	r3,#0
4626           	mov	r6,r4
86A3           	strh	r3,[r4,#&34]
62E5           	str	r5,[r4,#&2C]
F8C4 A030     	str	r10,[r4,#&30]
F846 9B04     	str	r9,[r6],#&4
4630           	mov	r0,r6
F7FF FA36     	bl	vListInitialiseItem
F104 0018     	add	r0,r4,#&18
F7FF FA32     	bl	vListInitialiseItem
6AE0           	ldr	r0,[r4,#&2C]
6F3B           	ldr	r3,[r7,#&70]
F1C5 0502     	rsb	r5,r5,#2
4298           	cmps	r0,r3
BF88           	it	hi
6738           	strhi	r0,[r7,#&70]

l00008E92:
EB00 0080     	add.w	r0,r0,r0,lsl #2
EB08 0080     	add.w	r0,r8,r0,lsl #2
61A5           	str	r5,[r4,#&18]
6124           	str	r4,[r4,#&10]
6264           	str	r4,[r4,#&24]
4631           	mov	r1,r6
F7FF FA25     	bl	vListInsertEnd
2001           	mov	r0,#1
E8BD 8FF8     	pop.w	{r3-fp,pc}

l00008EAC:
46B8           	mov	r8,r7
F848 0B04     	str	r0,[r8],#&4
4640           	mov	r0,r8
F7FF FA0C     	bl	vListInitialise
F107 0B2C     	add	fp,r7,#&2C
F107 0018     	add	r0,r7,#&18
F7FF FA06     	bl	vListInitialise
F107 0640     	add	r6,r7,#&40
4658           	mov	r0,fp
F7FF FA01     	bl	vListInitialise
4630           	mov	r0,r6
F7FF F9FE     	bl	vListInitialise
F107 0054     	add	r0,r7,#&54
F7FF F9FA     	bl	vListInitialise
F8C7 B068     	str	fp,[r7,#&68]
66FE           	str	r6,[r7,#&6C]
E7BD           	b	$00008E60

l00008EE4:
F04F 30FF     	mov	r0,#&FFFFFFFF
E8BD 8FF8     	pop.w	{r3-fp,pc}
00008EEC                                     FC 07 00 20             ... 

;; vCoRoutineAddToDelayedList: 00008EF0
;;   Called from:
;;     000083DE (in xQueueCRSend)
;;     00008490 (in xQueueCRReceive)
;;     00008760 (in prvFixedDelayCoRoutine)
vCoRoutineAddToDelayedList proc
B570           	push	{r4-r6,lr}
460E           	mov	r6,r1
4C0C           	ldr	r4,[00008F28]                           ; [pc,#&30]
6823           	ldr	r3,[r4]
6F65           	ldr	r5,[r4,#&74]
4405           	adds	r5,r0
1D18           	add	r0,r3,#4
F7FF FA1F     	bl	uxListRemove
6F63           	ldr	r3,[r4,#&74]
6821           	ldr	r1,[r4]
429D           	cmps	r5,r3
604D           	str	r5,[r1,#&4]
BF34           	ite	lo
6EE0           	ldrlo	r0,[r4,#&6C]

l00008F0E:
6EA0           	ldr	r0,[r4,#&68]
3104           	adds	r1,#4
F7FF F9FB     	bl	vListInsert
B136           	cbz	r6,$00008F26

l00008F18:
6821           	ldr	r1,[r4]
4630           	mov	r0,r6
E8BD 4070     	pop.w	{r4-r6,lr}
3118           	adds	r1,#&18
F7FF B9F3     	b	vListInsert

l00008F26:
BD70           	pop	{r4-r6,pc}
00008F28                         FC 07 00 20                     ...     

;; vCoRoutineSchedule: 00008F2C
;;   Called from:
;;     00008212 (in vApplicationIdleHook)
vCoRoutineSchedule proc
E92D 41F0     	push.w	{r4-r8,lr}
4D55           	ldr	r5,[00009088]                           ; [pc,#&154]
6D6B           	ldr	r3,[r5,#&54]
B32B           	cbz	r3,$00008F82

l00008F36:
2700           	mov	r7,#0
F105 0804     	add	r8,r5,#4

l00008F3C:
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
6E2B           	ldr	r3,[r5,#&60]
68DC           	ldr	r4,[r3,#&C]
F104 0018     	add	r0,r4,#&18
F7FF F9F4     	bl	uxListRemove
F387 8811     	msr	cpsr,r7
1D26           	add	r6,r4,#4
4630           	mov	r0,r6
F7FF F9EE     	bl	uxListRemove
6AE3           	ldr	r3,[r4,#&2C]
6F2A           	ldr	r2,[r5,#&70]
EB03 0083     	add.w	r0,r3,r3,lsl #2
4293           	cmps	r3,r2
4631           	mov	r1,r6
EB08 0080     	add.w	r0,r8,r0,lsl #2
BF88           	it	hi
672B           	strhi	r3,[r5,#&70]

l00008F78:
F7FF F9BA     	bl	vListInsertEnd
6D6B           	ldr	r3,[r5,#&54]
2B00           	cmps	r3,#0
D1DC           	bne	$00008F3C

l00008F82:
F7FF FCBF     	bl	MPU_xTaskGetTickCount
2700           	mov	r7,#0
6FAA           	ldr	r2,[r5,#&78]
6F6B           	ldr	r3,[r5,#&74]
1A80           	sub	r0,r0,r2
F8DF 8100     	ldr	r8,[00009090]                            ; [pc,#&100]
67E8           	str	r0,[r5,#&7C]

l00008F94:
2800           	cmps	r0,#0
D03D           	beq	$00009014

l00008F98:
3301           	adds	r3,#1
3801           	subs	r0,#1
676B           	str	r3,[r5,#&74]
67E8           	str	r0,[r5,#&7C]
2B00           	cmps	r3,#0
D053           	beq	$0000904C

l00008FA4:
6EAA           	ldr	r2,[r5,#&68]

l00008FA6:
6811           	ldr	r1,[r2]
2900           	cmps	r1,#0
D0F3           	beq	$00008F94

l00008FAC:
68D2           	ldr	r2,[r2,#&C]
68D4           	ldr	r4,[r2,#&C]
6862           	ldr	r2,[r4,#&4]
4293           	cmps	r3,r2
D206           	bhs	$00008FC4

l00008FB6:
E7ED           	b	$00008F94

l00008FB8:
68DA           	ldr	r2,[r3,#&C]
6F6B           	ldr	r3,[r5,#&74]
68D4           	ldr	r4,[r2,#&C]
6862           	ldr	r2,[r4,#&4]
429A           	cmps	r2,r3
D824           	bhi	$0000900E

l00008FC4:
F04F 03BF     	mov	r3,#&BF
F383 8811     	msr	cpsr,r3
F3BF 8F6F     	isb	sy
F3BF 8F4F     	dsb	sy
1D26           	add	r6,r4,#4
4630           	mov	r0,r6
F7FF F9B2     	bl	uxListRemove
6AA3           	ldr	r3,[r4,#&28]
F104 0018     	add	r0,r4,#&18
B10B           	cbz	r3,$00008FE8

l00008FE4:
F7FF F9AC     	bl	uxListRemove

l00008FE8:
F387 8811     	msr	cpsr,r7
6AE3           	ldr	r3,[r4,#&2C]
6F2A           	ldr	r2,[r5,#&70]
EB03 0083     	add.w	r0,r3,r3,lsl #2
4293           	cmps	r3,r2
4631           	mov	r1,r6
EB08 0080     	add.w	r0,r8,r0,lsl #2
BF88           	it	hi
672B           	strhi	r3,[r5,#&70]

l00009000:
F7FF F976     	bl	vListInsertEnd
6EAB           	ldr	r3,[r5,#&68]
681A           	ldr	r2,[r3]
2A00           	cmps	r2,#0
D1D5           	bne	$00008FB8

l0000900C:
6F6B           	ldr	r3,[r5,#&74]

l0000900E:
6FE8           	ldr	r0,[r5,#&7C]
2800           	cmps	r0,#0
D1C1           	bne	$00008F98

l00009014:
6F29           	ldr	r1,[r5,#&70]
67AB           	str	r3,[r5,#&78]
008B           	lsls	r3,r1,#2
185A           	add	r2,r3,r1
EB05 0282     	add.w	r2,r5,r2,lsl #2
6852           	ldr	r2,[r2,#&4]
2A00           	cmps	r2,#0
D12E           	bne	$00009084

l00009026:
B359           	cbz	r1,$00009080

l00009028:
1E4A           	sub	r2,r1,#1
0093           	lsls	r3,r2,#2
1898           	add	r0,r3,r2
EB05 0080     	add.w	r0,r5,r0,lsl #2
6840           	ldr	r0,[r0,#&4]
B978           	cbnz	r0,$00009056

l00009036:
B132           	cbz	r2,$00009046

l00009038:
1E8A           	sub	r2,r1,#2
0093           	lsls	r3,r2,#2
1899           	add	r1,r3,r2
EB05 0181     	add.w	r1,r5,r1,lsl #2
6849           	ldr	r1,[r1,#&4]
B939           	cbnz	r1,$00009056

l00009046:
672A           	str	r2,[r5,#&70]
E8BD 81F0     	pop.w	{r4-r8,pc}

l0000904C:
6EA9           	ldr	r1,[r5,#&68]
6EEA           	ldr	r2,[r5,#&6C]
66E9           	str	r1,[r5,#&6C]
66AA           	str	r2,[r5,#&68]
E7A7           	b	$00008FA6

l00009056:
672A           	str	r2,[r5,#&70]

l00009058:
4413           	adds	r3,r2
009B           	lsls	r3,r3,#2
18E9           	add	r1,r5,r3
688A           	ldr	r2,[r1,#&8]
480A           	ldr	r0,[0000908C]                           ; [pc,#&28]
6852           	ldr	r2,[r2,#&4]
4403           	adds	r3,r0
429A           	cmps	r2,r3
608A           	str	r2,[r1,#&8]
BF08           	it	eq
6852           	ldreq	r2,[r2,#&4]

l0000906E:
68D0           	ldr	r0,[r2,#&C]
BF08           	it	eq
608A           	streq	r2,[r1,#&8]

l00009074:
6028           	str	r0,[r5]
6803           	ldr	r3,[r0]
6B01           	ldr	r1,[r0,#&30]
E8BD 41F0     	pop.w	{r4-r8,lr}
4718           	bx	r3

l00009080:
E8BD 81F0     	pop.w	{r4-r8,pc}

l00009084:
460A           	mov	r2,r1
E7E7           	b	$00009058
00009088                         FC 07 00 20 08 08 00 20         ... ... 
00009090 00 08 00 20                                     ...             

;; xCoRoutineRemoveFromEventList: 00009094
;;   Called from:
;;     000083F2 (in xQueueCRSend)
;;     0000847C (in xQueueCRReceive)
;;     000084C6 (in xQueueCRSendFromISR)
;;     0000851C (in xQueueCRReceiveFromISR)
xCoRoutineRemoveFromEventList proc
68C3           	ldr	r3,[r0,#&C]
B570           	push	{r4-r6,lr}
68DC           	ldr	r4,[r3,#&C]
4D09           	ldr	r5,[000090C0]                           ; [pc,#&24]
F104 0618     	add	r6,r4,#&18
4630           	mov	r0,r6
F7FF F94D     	bl	uxListRemove
F105 0054     	add	r0,r5,#&54
4631           	mov	r1,r6
F7FF F920     	bl	vListInsertEnd
682B           	ldr	r3,[r5]
6AE0           	ldr	r0,[r4,#&2C]
6ADB           	ldr	r3,[r3,#&2C]
4298           	cmps	r0,r3
BF34           	ite	lo
2000           	movlo	r0,#0

l000090BC:
2001           	mov	r0,#1
BD70           	pop	{r4-r6,pc}
000090C0 FC 07 00 20                                     ...             

;; GPIOGetIntNumber: 000090C4
GPIOGetIntNumber proc
4B0F           	ldr	r3,[00009104]                           ; [pc,#&3C]
4298           	cmps	r0,r3
D019           	beq	$000090FE

l000090CA:
D808           	bhi	$000090DE

l000090CC:
F1B0 2F40     	cmp	r0,#&40004000
D013           	beq	$000090FA

l000090D2:
F5A3 5380     	sub	r3,r3,#&1000
4298           	cmps	r0,r3
D10A           	bne	$000090F0

l000090DA:
2011           	mov	r0,#&11
4770           	bx	lr

l000090DE:
4B0A           	ldr	r3,[00009108]                           ; [pc,#&28]
4298           	cmps	r0,r3
D008           	beq	$000090F6

l000090E4:
F503 33E8     	add	r3,r3,#&1D000
4298           	cmps	r0,r3
D101           	bne	$000090F0

l000090EC:
2014           	mov	r0,#&14
4770           	bx	lr

l000090F0:
F04F 30FF     	mov	r0,#&FFFFFFFF
4770           	bx	lr

l000090F6:
2013           	mov	r0,#&13
4770           	bx	lr

l000090FA:
2010           	mov	r0,#&10
4770           	bx	lr

l000090FE:
2012           	mov	r0,#&12
4770           	bx	lr
00009102       00 BF 00 60 00 40 00 70 00 40               ...`.@.p.@    

;; GPIODirModeSet: 0000910C
;;   Called from:
;;     00008238 (in PDCInit)
;;     00008244 (in PDCInit)
GPIODirModeSet proc
F8D0 3400     	ldr	r3,[r0,#&400]
F012 0F01     	tst	r2,#1
BF14           	ite	ne
430B           	orrne	r3,r1

l00009118:
438B           	bics	r3,r1
F8C0 3400     	str	r3,[r0,#&400]
F8D0 3420     	ldr	r3,[r0,#&420]
0792           	lsls	r2,r2,#&1E
BF4C           	ite	mi
4319           	orrmi	r1,r3

l00009128:
EA23 0101     	bic.w	r1,r3,r1
F8C0 1420     	str	r1,[r0,#&420]
4770           	bx	lr
00009132       00 BF                                       ..            

;; GPIODirModeGet: 00009134
GPIODirModeGet proc
2301           	mov	r3,#1
B410           	push	{r4}
FA03 F101     	lsl	r1,r3,r1
F8D0 4400     	ldr	r4,[r0,#&400]
B2C9           	uxtb	r1,r1
F8D0 2420     	ldr	r2,[r0,#&420]
420C           	tst	r4,r1
BF08           	it	eq
2300           	moveq	r3,#0

l0000914C:
420A           	tst	r2,r1
BF14           	ite	ne
2002           	movne	r0,#2

l00009152:
2000           	mov	r0,#0
BC10           	pop	{r4}
4318           	orrs	r0,r3
4770           	bx	lr
0000915A                               00 BF                       ..    

;; GPIOIntTypeSet: 0000915C
GPIOIntTypeSet proc
F8D0 3408     	ldr	r3,[r0,#&408]
F012 0F01     	tst	r2,#1
BF14           	ite	ne
430B           	orrne	r3,r1

l00009168:
438B           	bics	r3,r1
F8C0 3408     	str	r3,[r0,#&408]
F8D0 3404     	ldr	r3,[r0,#&404]
F012 0F02     	tst	r2,#2
BF14           	ite	ne
430B           	orrne	r3,r1

l0000917A:
438B           	bics	r3,r1
F8C0 3404     	str	r3,[r0,#&404]
F8D0 340C     	ldr	r3,[r0,#&40C]
0752           	lsls	r2,r2,#&1D
BF4C           	ite	mi
4319           	orrmi	r1,r3

l0000918A:
EA23 0101     	bic.w	r1,r3,r1
F8C0 140C     	str	r1,[r0,#&40C]
4770           	bx	lr

;; GPIOIntTypeGet: 00009194
GPIOIntTypeGet proc
2301           	mov	r3,#1
F8D0 2408     	ldr	r2,[r0,#&408]
FA03 F101     	lsl	r1,r3,r1
B2C9           	uxtb	r1,r1
F8D0 3404     	ldr	r3,[r0,#&404]
420A           	tst	r2,r1
F8D0 040C     	ldr	r0,[r0,#&40C]
BF14           	ite	ne
2201           	movne	r2,#1

l000091AE:
2200           	mov	r2,#0
420B           	tst	r3,r1
BF14           	ite	ne
2302           	movne	r3,#2

l000091B6:
2300           	mov	r3,#0
4208           	tst	r0,r1
BF14           	ite	ne
2004           	movne	r0,#4

l000091BE:
2000           	mov	r0,#0
4313           	orrs	r3,r2
4318           	orrs	r0,r3
4770           	bx	lr
000091C6                   00 BF                               ..        

;; GPIOPadConfigSet: 000091C8
;;   Called from:
;;     00008252 (in PDCInit)
;;     0000947A (in GPIOPinTypeComparator)
;;     000094A0 (in GPIOPinTypeI2C)
;;     000094C4 (in GPIOPinTypeQEI)
;;     000094E8 (in GPIOPinTypeUART)
GPIOPadConfigSet proc
B410           	push	{r4}
F8D0 4500     	ldr	r4,[r0,#&500]
F012 0F01     	tst	r2,#1
BF14           	ite	ne
430C           	orrne	r4,r1

l000091D6:
438C           	bics	r4,r1
F8C0 4500     	str	r4,[r0,#&500]
F8D0 4504     	ldr	r4,[r0,#&504]
F012 0F02     	tst	r2,#2
BF14           	ite	ne
430C           	orrne	r4,r1

l000091E8:
438C           	bics	r4,r1
F8C0 4504     	str	r4,[r0,#&504]
F8D0 4508     	ldr	r4,[r0,#&508]
F012 0F04     	tst	r2,#4
BF14           	ite	ne
430C           	orrne	r4,r1

l000091FA:
438C           	bics	r4,r1
F8C0 4508     	str	r4,[r0,#&508]
F012 0F08     	tst	r2,#8
F8D0 2518     	ldr	r2,[r0,#&518]
BF14           	ite	ne
430A           	orrne	r2,r1

l0000920C:
438A           	bics	r2,r1
F8C0 2518     	str	r2,[r0,#&518]
F8D0 250C     	ldr	r2,[r0,#&50C]
07DC           	lsls	r4,r3,#&1F
BF4C           	ite	mi
430A           	orrmi	r2,r1

l0000921C:
438A           	bics	r2,r1
F8C0 250C     	str	r2,[r0,#&50C]
F8D0 2510     	ldr	r2,[r0,#&510]
079C           	lsls	r4,r3,#&1E
BF4C           	ite	mi
430A           	orrmi	r2,r1

l0000922C:
438A           	bics	r2,r1
F8C0 2510     	str	r2,[r0,#&510]
F8D0 2514     	ldr	r2,[r0,#&514]
075C           	lsls	r4,r3,#&1D
BF4C           	ite	mi
430A           	orrmi	r2,r1

l0000923C:
438A           	bics	r2,r1
F8C0 2514     	str	r2,[r0,#&514]
F013 0F08     	tst	r3,#8
F8D0 351C     	ldr	r3,[r0,#&51C]
BC10           	pop	{r4}
BF14           	ite	ne
4319           	orrne	r1,r3

l00009250:
EA23 0101     	bic.w	r1,r3,r1
F8C0 151C     	str	r1,[r0,#&51C]
4770           	bx	lr
0000925A                               00 BF                       ..    

;; GPIOPadConfigGet: 0000925C
GPIOPadConfigGet proc
B4F0           	push	{r4-r7}
2401           	mov	r4,#1
F8D0 5500     	ldr	r5,[r0,#&500]
FA04 F101     	lsl	r1,r4,r1
B2C9           	uxtb	r1,r1
F8D0 4504     	ldr	r4,[r0,#&504]
420D           	tst	r5,r1
F8D0 5508     	ldr	r5,[r0,#&508]
BF14           	ite	ne
2701           	movne	r7,#1

l00009278:
2700           	mov	r7,#0
420C           	tst	r4,r1
F8D0 4518     	ldr	r4,[r0,#&518]
BF14           	ite	ne
2602           	movne	r6,#2

l00009284:
2600           	mov	r6,#0
420D           	tst	r5,r1
BF14           	ite	ne
2504           	movne	r5,#4

l0000928C:
2500           	mov	r5,#0
420C           	tst	r4,r1
BF14           	ite	ne
2408           	movne	r4,#8

l00009294:
2400           	mov	r4,#0
433E           	orrs	r6,r7
4335           	orrs	r5,r6
432C           	orrs	r4,r5
6014           	str	r4,[r2]
F8D0 250C     	ldr	r2,[r0,#&50C]
F8D0 4510     	ldr	r4,[r0,#&510]
4211           	tst	r1,r2
F8D0 6514     	ldr	r6,[r0,#&514]
BF18           	it	ne
2501           	movne	r5,#1

l000092B0:
F8D0 251C     	ldr	r2,[r0,#&51C]
BF08           	it	eq
2500           	moveq	r5,#0

l000092B8:
4221           	tst	r1,r4
BF14           	ite	ne
2402           	movne	r4,#2

l000092BE:
2400           	mov	r4,#0
4231           	tst	r1,r6
BF14           	ite	ne
2004           	movne	r0,#4

l000092C6:
2000           	mov	r0,#0
4211           	tst	r1,r2
BF14           	ite	ne
2208           	movne	r2,#8

l000092CE:
2200           	mov	r2,#0
EA44 0105     	orr	r1,r4,r5
4301           	orrs	r1,r0
430A           	orrs	r2,r1
601A           	str	r2,[r3]
BCF0           	pop	{r4-r7}
4770           	bx	lr
000092DE                                           00 BF               ..

;; GPIOPinIntEnable: 000092E0
GPIOPinIntEnable proc
F8D0 3410     	ldr	r3,[r0,#&410]
4319           	orrs	r1,r3
F8C0 1410     	str	r1,[r0,#&410]
4770           	bx	lr

;; GPIOPinIntDisable: 000092EC
GPIOPinIntDisable proc
F8D0 3410     	ldr	r3,[r0,#&410]
EA23 0101     	bic.w	r1,r3,r1
F8C0 1410     	str	r1,[r0,#&410]
4770           	bx	lr
000092FA                               00 BF                       ..    

;; GPIOPinIntStatus: 000092FC
GPIOPinIntStatus proc
B911           	cbnz	r1,$00009304

l000092FE:
F8D0 0414     	ldr	r0,[r0,#&414]
4770           	bx	lr

l00009304:
F8D0 0418     	ldr	r0,[r0,#&418]
4770           	bx	lr
0000930A                               00 BF                       ..    

;; GPIOPinIntClear: 0000930C
GPIOPinIntClear proc
F8C0 141C     	str	r1,[r0,#&41C]
4770           	bx	lr
00009312       00 BF                                       ..            

;; GPIOPortIntRegister: 00009314
GPIOPortIntRegister proc
4B24           	ldr	r3,[000093A8]                           ; [pc,#&90]
B510           	push	{r4,lr}
4298           	cmps	r0,r3
D03C           	beq	$00009396

l0000931C:
D80F           	bhi	$0000933E

l0000931E:
F1B0 2F40     	cmp	r0,#&40004000
D02F           	beq	$00009384

l00009324:
F5A3 5380     	sub	r3,r3,#&1000
4298           	cmps	r0,r3
D118           	bne	$0000935E

l0000932C:
2411           	mov	r4,#&11
4620           	mov	r0,r4
F000 F8E8     	bl	IntRegister
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B94F     	b	IntEnable

l0000933E:
4B1B           	ldr	r3,[000093AC]                           ; [pc,#&6C]
4298           	cmps	r0,r3
D016           	beq	$00009372

l00009344:
F503 33E8     	add	r3,r3,#&1D000
4298           	cmps	r0,r3
D108           	bne	$0000935E

l0000934C:
2414           	mov	r4,#&14
4620           	mov	r0,r4
F000 F8D8     	bl	IntRegister
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B93F     	b	IntEnable

l0000935E:
F04F 34FF     	mov	r4,#&FFFFFFFF
4620           	mov	r0,r4
F000 F8CE     	bl	IntRegister
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B935     	b	IntEnable

l00009372:
2413           	mov	r4,#&13
4620           	mov	r0,r4
F000 F8C5     	bl	IntRegister
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B92C     	b	IntEnable

l00009384:
2410           	mov	r4,#&10
4620           	mov	r0,r4
F000 F8BC     	bl	IntRegister
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B923     	b	IntEnable

l00009396:
2412           	mov	r4,#&12
4620           	mov	r0,r4
F000 F8B3     	bl	IntRegister
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B91A     	b	IntEnable
000093A8                         00 60 00 40 00 70 00 40         .`.@.p.@

;; GPIOPortIntUnregister: 000093B0
GPIOPortIntUnregister proc
4B24           	ldr	r3,[00009444]                           ; [pc,#&90]
B510           	push	{r4,lr}
4298           	cmps	r0,r3
D03C           	beq	$00009432

l000093B8:
D80F           	bhi	$000093DA

l000093BA:
F1B0 2F40     	cmp	r0,#&40004000
D02F           	beq	$00009420

l000093C0:
F5A3 5380     	sub	r3,r3,#&1000
4298           	cmps	r0,r3
D118           	bne	$000093FA

l000093C8:
2411           	mov	r4,#&11
4620           	mov	r0,r4
F000 F934     	bl	IntDisable
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B8AF     	b	IntUnregister

l000093DA:
4B1B           	ldr	r3,[00009448]                           ; [pc,#&6C]
4298           	cmps	r0,r3
D016           	beq	$0000940E

l000093E0:
F503 33E8     	add	r3,r3,#&1D000
4298           	cmps	r0,r3
D108           	bne	$000093FA

l000093E8:
2414           	mov	r4,#&14
4620           	mov	r0,r4
F000 F924     	bl	IntDisable
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B89F     	b	IntUnregister

l000093FA:
F04F 34FF     	mov	r4,#&FFFFFFFF
4620           	mov	r0,r4
F000 F91A     	bl	IntDisable
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B895     	b	IntUnregister

l0000940E:
2413           	mov	r4,#&13
4620           	mov	r0,r4
F000 F911     	bl	IntDisable
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B88C     	b	IntUnregister

l00009420:
2410           	mov	r4,#&10
4620           	mov	r0,r4
F000 F908     	bl	IntDisable
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B883     	b	IntUnregister

l00009432:
2412           	mov	r4,#&12
4620           	mov	r0,r4
F000 F8FF     	bl	IntDisable
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F000 B87A     	b	IntUnregister
00009444             00 60 00 40 00 70 00 40                 .`.@.p.@    

;; GPIOPinRead: 0000944C
GPIOPinRead proc
F850 0021     	ldr.w	r0,[r0,r1,lsl #2]
4770           	bx	lr
00009452       00 BF                                       ..            

;; GPIOPinWrite: 00009454
;;   Called from:
;;     00008276 (in PDCInit)
;;     00008288 (in PDCInit)
GPIOPinWrite proc
F840 2021     	str.w	r2,[r0,r1,lsl #2]
4770           	bx	lr
0000945A                               00 BF                       ..    

;; GPIOPinTypeComparator: 0000945C
GPIOPinTypeComparator proc
B470           	push	{r4-r6}
43CD           	mvns	r5,r1
F8D0 2400     	ldr	r2,[r0,#&400]
2300           	mov	r3,#0
402A           	ands	r2,r5
F8C0 2400     	str	r2,[r0,#&400]
F8D0 6420     	ldr	r6,[r0,#&420]
2201           	mov	r2,#1
4035           	ands	r5,r6
F8C0 5420     	str	r5,[r0,#&420]
BC70           	pop	{r4-r6}
F7FF BEA5     	b	GPIOPadConfigSet
0000947E                                           00 BF               ..

;; GPIOPinTypeI2C: 00009480
;;   Called from:
;;     00009908 (in OSRAMInit)
GPIOPinTypeI2C proc
B470           	push	{r4-r6}
460D           	mov	r5,r1
F8D0 2400     	ldr	r2,[r0,#&400]
230B           	mov	r3,#&B
EA22 0201     	bic.w	r2,r2,r1
F8C0 2400     	str	r2,[r0,#&400]
F8D0 6420     	ldr	r6,[r0,#&420]
2201           	mov	r2,#1
4335           	orrs	r5,r6
F8C0 5420     	str	r5,[r0,#&420]
BC70           	pop	{r4-r6}
F7FF BE92     	b	GPIOPadConfigSet

;; GPIOPinTypeQEI: 000094A4
GPIOPinTypeQEI proc
B470           	push	{r4-r6}
460D           	mov	r5,r1
F8D0 2400     	ldr	r2,[r0,#&400]
230A           	mov	r3,#&A
EA22 0201     	bic.w	r2,r2,r1
F8C0 2400     	str	r2,[r0,#&400]
F8D0 6420     	ldr	r6,[r0,#&420]
2201           	mov	r2,#1
4335           	orrs	r5,r6
F8C0 5420     	str	r5,[r0,#&420]
BC70           	pop	{r4-r6}
F7FF BE80     	b	GPIOPadConfigSet

;; GPIOPinTypeUART: 000094C8
;;   Called from:
;;     000094EC (in GPIOPinTypeTimer)
;;     000094F0 (in GPIOPinTypeSSI)
;;     000094F4 (in GPIOPinTypePWM)
GPIOPinTypeUART proc
B470           	push	{r4-r6}
460D           	mov	r5,r1
F8D0 2400     	ldr	r2,[r0,#&400]
2308           	mov	r3,#8
EA22 0201     	bic.w	r2,r2,r1
F8C0 2400     	str	r2,[r0,#&400]
F8D0 6420     	ldr	r6,[r0,#&420]
2201           	mov	r2,#1
4335           	orrs	r5,r6
F8C0 5420     	str	r5,[r0,#&420]
BC70           	pop	{r4-r6}
F7FF BE6E     	b	GPIOPadConfigSet

;; GPIOPinTypeTimer: 000094EC
GPIOPinTypeTimer proc
F7FF BFEC     	b	GPIOPinTypeUART

;; GPIOPinTypeSSI: 000094F0
GPIOPinTypeSSI proc
F7FF BFEA     	b	GPIOPinTypeUART

;; GPIOPinTypePWM: 000094F4
GPIOPinTypePWM proc
F7FF BFE8     	b	GPIOPinTypeUART

;; IntDefaultHandler: 000094F8
IntDefaultHandler proc
E7FE           	b	IntDefaultHandler
000094FA                               00 BF                       ..    

;; IntMasterEnable: 000094FC
IntMasterEnable proc
F000 BDEE     	b	CPUcpsie

;; IntMasterDisable: 00009500
IntMasterDisable proc
F000 BDF0     	b	CPUcpsid

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
4B0A           	ldr	r3,[00009530]                           ; [pc,#&28]
B430           	push	{r4-r5}
681B           	ldr	r3,[r3]
4C0A           	ldr	r4,[00009534]                           ; [pc,#&28]
42A3           	cmps	r3,r4
D00A           	beq	$00009526

l00009510:
4623           	mov	r3,r4
F104 05B8     	add	r5,r4,#&B8

l00009516:
1B1A           	sub	r2,r3,r4
6812           	ldr	r2,[r2]
F843 2B04     	str	r2,[r3],#&4
42AB           	cmps	r3,r5
D1F9           	bne	$00009516

l00009522:
4B03           	ldr	r3,[00009530]                           ; [pc,#&C]
601C           	str	r4,[r3]

l00009526:
F844 1020     	str.w	r1,[r4,r0,lsl #2]
BC30           	pop	{r4-r5}
4770           	bx	lr
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
4B02           	ldr	r3,[00009544]                           ; [pc,#&8]
4A03           	ldr	r2,[00009548]                           ; [pc,#&C]
F843 2020     	str.w	r2,[r3,r0,lsl #2]
4770           	bx	lr
00009542       00 BF 00 00 00 20 F9 94 00 00               ..... ....    

;; IntPriorityGroupingSet: 0000954C
IntPriorityGroupingSet proc
4B04           	ldr	r3,[00009560]                           ; [pc,#&10]
4A05           	ldr	r2,[00009564]                           ; [pc,#&14]
F853 3020     	ldr.w	r3,[r3,r0,lsl #2]
F043 63BF     	orr	r3,r3,#&5F80000
F443 3300     	orr	r3,r3,#&20000
6013           	str	r3,[r2]
4770           	bx	lr
00009560 A4 A2 00 00 0C ED 00 E0                         ........        

;; IntPriorityGroupingGet: 00009568
IntPriorityGroupingGet proc
F44F 63E0     	mov	r3,#&700
4906           	ldr	r1,[00009588]                           ; [pc,#&18]
2000           	mov	r0,#0
6809           	ldr	r1,[r1]
4A06           	ldr	r2,[0000958C]                           ; [pc,#&18]
4019           	ands	r1,r3
E001           	b	$0000957C

l00009578:
F852 3B04     	ldr	r3,[r2],#&4

l0000957C:
428B           	cmps	r3,r1
D002           	beq	$00009586

l00009580:
3001           	adds	r0,#1
2808           	cmps	r0,#8
D1F8           	bne	$00009578

l00009586:
4770           	bx	lr
00009588                         0C ED 00 E0 A8 A2 00 00         ........

;; IntPrioritySet: 00009590
IntPrioritySet proc
22FF           	mov	r2,#&FF
4B09           	ldr	r3,[000095B8]                           ; [pc,#&24]
B410           	push	{r4}
F020 0403     	bic	r4,r0,#3
4423           	adds	r3,r4
6A1C           	ldr	r4,[r3,#&20]
F000 0003     	and	r0,r0,#3
6823           	ldr	r3,[r4]
00C0           	lsls	r0,r0,#3
4082           	lsls	r2,r0
EA23 0302     	bic.w	r3,r3,r2
FA01 F000     	lsl	r0,r1,r0
4318           	orrs	r0,r3
6020           	str	r0,[r4]
BC10           	pop	{r4}
4770           	bx	lr
000095B8                         A4 A2 00 00                     ....    

;; IntPriorityGet: 000095BC
IntPriorityGet proc
4B06           	ldr	r3,[000095D8]                           ; [pc,#&18]
F020 0203     	bic	r2,r0,#3
4413           	adds	r3,r2
6A1B           	ldr	r3,[r3,#&20]
F000 0003     	and	r0,r0,#3
681B           	ldr	r3,[r3]
00C0           	lsls	r0,r0,#3
FA23 F000     	lsr	r0,r3,r0
B2C0           	uxtb	r0,r0
4770           	bx	lr
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
2804           	cmps	r0,#4
D013           	beq	$00009608

l000095E0:
2805           	cmps	r0,#5
D017           	beq	$00009614

l000095E4:
2806           	cmps	r0,#6
D01B           	beq	$00009620

l000095E8:
280F           	cmps	r0,#&F
D007           	beq	$000095FC

l000095EC:
D905           	bls	$000095FA

l000095EE:
2301           	mov	r3,#1
3810           	subs	r0,#&10
4A0E           	ldr	r2,[0000962C]                           ; [pc,#&38]
FA03 F000     	lsl	r0,r3,r0
6010           	str	r0,[r2]

l000095FA:
4770           	bx	lr

l000095FC:
4A0C           	ldr	r2,[00009630]                           ; [pc,#&30]
6813           	ldr	r3,[r2]
F043 0302     	orr	r3,r3,#2
6013           	str	r3,[r2]
4770           	bx	lr

l00009608:
4A0A           	ldr	r2,[00009634]                           ; [pc,#&28]
6813           	ldr	r3,[r2]
F443 3380     	orr	r3,r3,#&10000
6013           	str	r3,[r2]
4770           	bx	lr

l00009614:
4A07           	ldr	r2,[00009634]                           ; [pc,#&1C]
6813           	ldr	r3,[r2]
F443 3300     	orr	r3,r3,#&20000
6013           	str	r3,[r2]
4770           	bx	lr

l00009620:
4A04           	ldr	r2,[00009634]                           ; [pc,#&10]
6813           	ldr	r3,[r2]
F443 2380     	orr	r3,r3,#&40000
6013           	str	r3,[r2]
4770           	bx	lr
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
2804           	cmps	r0,#4
D013           	beq	$00009664

l0000963C:
2805           	cmps	r0,#5
D017           	beq	$00009670

l00009640:
2806           	cmps	r0,#6
D01B           	beq	$0000967C

l00009644:
280F           	cmps	r0,#&F
D007           	beq	$00009658

l00009648:
D905           	bls	$00009656

l0000964A:
2301           	mov	r3,#1
3810           	subs	r0,#&10
4A0E           	ldr	r2,[00009688]                           ; [pc,#&38]
FA03 F000     	lsl	r0,r3,r0
6010           	str	r0,[r2]

l00009656:
4770           	bx	lr

l00009658:
4A0C           	ldr	r2,[0000968C]                           ; [pc,#&30]
6813           	ldr	r3,[r2]
F023 0302     	bic	r3,r3,#2
6013           	str	r3,[r2]
4770           	bx	lr

l00009664:
4A0A           	ldr	r2,[00009690]                           ; [pc,#&28]
6813           	ldr	r3,[r2]
F423 3380     	bic	r3,r3,#&10000
6013           	str	r3,[r2]
4770           	bx	lr

l00009670:
4A07           	ldr	r2,[00009690]                           ; [pc,#&1C]
6813           	ldr	r3,[r2]
F423 3300     	bic	r3,r3,#&20000
6013           	str	r3,[r2]
4770           	bx	lr

l0000967C:
4A04           	ldr	r2,[00009690]                           ; [pc,#&10]
6813           	ldr	r3,[r2]
F423 2380     	bic	r3,r3,#&40000
6013           	str	r3,[r2]
4770           	bx	lr
00009688                         80 E1 00 E0 10 E0 00 E0         ........
00009690 24 ED 00 E0                                     $...            

;; OSRAMDelay: 00009694
;;   Called from:
;;     000096DE (in OSRAMWriteArray)
;;     00009718 (in OSRAMWriteByte)
;;     00009750 (in OSRAMWriteFinal)
;;     00009776 (in OSRAMWriteFinal)
OSRAMDelay proc
3801           	subs	r0,#1
D1FD           	bne	OSRAMDelay

l00009698:
4770           	bx	lr
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
B538           	push	{r3-r5,lr}
4605           	mov	r5,r0
4C07           	ldr	r4,[000096C0]                           ; [pc,#&1C]
2200           	mov	r2,#0
4620           	mov	r0,r4
213D           	mov	r1,#&3D
F000 FDAE     	bl	I2CMasterSlaveAddrSet
4629           	mov	r1,r5
4620           	mov	r0,r4
F000 FDC4     	bl	I2CMasterDataPut
4620           	mov	r0,r4
E8BD 4038     	pop.w	{r3-r5,lr}
2103           	mov	r1,#3
F000 BDB0     	b	I2CMasterControl
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
B1C9           	cbz	r1,$000096FA

l000096C6:
B5F8           	push	{r3-r7,lr}
4605           	mov	r5,r0
4F0C           	ldr	r7,[000096FC]                           ; [pc,#&30]
4C0C           	ldr	r4,[00009700]                           ; [pc,#&30]
1846           	add	r6,r0,r1

l000096D0:
2100           	mov	r1,#0
4620           	mov	r0,r4
F000 FD78     	bl	I2CMasterIntStatus
2800           	cmps	r0,#0
D0F9           	beq	$000096D0

l000096DC:
6838           	ldr	r0,[r7]
F7FF FFD9     	bl	OSRAMDelay
F815 1B01     	ldrb	r1,[r5],#&1
4620           	mov	r0,r4
F000 FDA8     	bl	I2CMasterDataPut
2101           	mov	r1,#1
4620           	mov	r0,r4
F000 FD96     	bl	I2CMasterControl
42AE           	cmps	r6,r5
D1EB           	bne	$000096D0

l000096F8:
BDF8           	pop	{r3-r7,pc}

l000096FA:
4770           	bx	lr
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
B510           	push	{r4,lr}
4604           	mov	r4,r0

l00009708:
2100           	mov	r1,#0
4809           	ldr	r0,[00009730]                           ; [pc,#&24]
F000 FD5C     	bl	I2CMasterIntStatus
2800           	cmps	r0,#0
D0F9           	beq	$00009708

l00009714:
4B07           	ldr	r3,[00009734]                           ; [pc,#&1C]
6818           	ldr	r0,[r3]
F7FF FFBC     	bl	OSRAMDelay
4621           	mov	r1,r4
4804           	ldr	r0,[00009730]                           ; [pc,#&10]
F000 FD8C     	bl	I2CMasterDataPut
E8BD 4010     	pop.w	{r4,lr}
2101           	mov	r1,#1
4801           	ldr	r0,[00009730]                           ; [pc,#&4]
F000 BD78     	b	I2CMasterControl
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
B570           	push	{r4-r6,lr}
4606           	mov	r6,r0
4C0E           	ldr	r4,[00009778]                           ; [pc,#&38]

l0000973E:
2100           	mov	r1,#0
4620           	mov	r0,r4
F000 FD41     	bl	I2CMasterIntStatus
2800           	cmps	r0,#0
D0F9           	beq	$0000973E

l0000974A:
4D0C           	ldr	r5,[0000977C]                           ; [pc,#&30]
4C0A           	ldr	r4,[00009778]                           ; [pc,#&28]
6828           	ldr	r0,[r5]
F7FF FFA0     	bl	OSRAMDelay
4631           	mov	r1,r6
4620           	mov	r0,r4
F000 FD70     	bl	I2CMasterDataPut
2105           	mov	r1,#5
4620           	mov	r0,r4
F000 FD5E     	bl	I2CMasterControl

l00009764:
2100           	mov	r1,#0
4620           	mov	r0,r4
F000 FD2E     	bl	I2CMasterIntStatus
2800           	cmps	r0,#0
D0F9           	beq	$00009764

l00009770:
6828           	ldr	r0,[r5]
E8BD 4070     	pop.w	{r4-r6,lr}
E78D           	b	OSRAMDelay
00009778                         00 00 02 40 7C 08 00 20         ...@|.. 

;; OSRAMClear: 00009780
;;   Called from:
;;     00008050 (in vPrintTask)
;;     0000995C (in OSRAMInit)
OSRAMClear proc
B510           	push	{r4,lr}
2080           	mov	r0,#&80
F7FF FF8A     	bl	OSRAMWriteFirst
2106           	mov	r1,#6
480E           	ldr	r0,[000097C4]                           ; [pc,#&38]
F7FF FF9A     	bl	OSRAMWriteArray
245F           	mov	r4,#&5F

l00009792:
2000           	mov	r0,#0
F7FF FFB6     	bl	OSRAMWriteByte
3C01           	subs	r4,#1
D1FA           	bne	$00009792

l0000979C:
4620           	mov	r0,r4
F7FF FFCB     	bl	OSRAMWriteFinal
2080           	mov	r0,#&80
F7FF FF7A     	bl	OSRAMWriteFirst
2106           	mov	r1,#6
4807           	ldr	r0,[000097C8]                           ; [pc,#&1C]
F7FF FF8A     	bl	OSRAMWriteArray
245F           	mov	r4,#&5F

l000097B2:
2000           	mov	r0,#0
F7FF FFA6     	bl	OSRAMWriteByte
3C01           	subs	r4,#1
D1FA           	bne	$000097B2

l000097BC:
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
E7B9           	b	OSRAMWriteFinal
000097C4             F4 A2 00 00 FC A2 00 00                 ........    

;; OSRAMStringDraw: 000097CC
;;   Called from:
;;     0000805E (in vPrintTask)
;;     000080E8 (in Main)
OSRAMStringDraw proc
B570           	push	{r4-r6,lr}
4616           	mov	r6,r2
460C           	mov	r4,r1
4605           	mov	r5,r0
2080           	mov	r0,#&80
F7FF FF61     	bl	OSRAMWriteFirst
2E00           	cmps	r6,#0
BF0C           	ite	eq
20B0           	moveq	r0,#&B0

l000097E0:
20B1           	mov	r0,#&B1
F7FF FF8F     	bl	OSRAMWriteByte
F104 0624     	add	r6,r4,#&24
2080           	mov	r0,#&80
F7FF FF8A     	bl	OSRAMWriteByte
F006 000F     	and	r0,r6,#&F
F7FF FF86     	bl	OSRAMWriteByte
2080           	mov	r0,#&80
F7FF FF83     	bl	OSRAMWriteByte
F3C6 1003     	ubfx	r0,r6,#4,#4
F040 0010     	orr	r0,r0,#&10
F7FF FF7D     	bl	OSRAMWriteByte
2040           	mov	r0,#&40
F7FF FF7A     	bl	OSRAMWriteByte
782B           	ldrb	r3,[r5]
B383           	cbz	r3,$00009876

l00009814:
2C5A           	cmps	r4,#&5A
4E18           	ldr	r6,[00009878]                           ; [pc,#&60]
D90A           	bls	$00009830

l0000981A:
E017           	b	$0000984C

l0000981C:
F815 3F01     	ldrb	r3,[r5,#&1]!
3406           	adds	r4,#6
B183           	cbz	r3,$00009846

l00009824:
F7FF FF6E     	bl	OSRAMWriteByte
782B           	ldrb	r3,[r5]
B31B           	cbz	r3,$00009874

l0000982C:
2C5A           	cmps	r4,#&5A
D80D           	bhi	$0000984C

l00009830:
3B20           	subs	r3,#&20
EB03 0383     	add.w	r3,r3,r3,lsl #2
18F0           	add	r0,r6,r3
2105           	mov	r1,#5
F7FF FF43     	bl	OSRAMWriteArray
2C5A           	cmps	r4,#&5A
F04F 0000     	mov	r0,#0
D1EA           	bne	$0000981C

l00009846:
E8BD 4070     	pop.w	{r4-r6,lr}
E775           	b	OSRAMWriteFinal

l0000984C:
3B20           	subs	r3,#&20
EB03 0383     	add.w	r3,r3,r3,lsl #2
F1C4 045F     	rsb	r4,r4,#&5F
18F0           	add	r0,r6,r3
4621           	mov	r1,r4
F7FF FF33     	bl	OSRAMWriteArray
782B           	ldrb	r3,[r5]
4A06           	ldr	r2,[0000987C]                           ; [pc,#&18]
3B20           	subs	r3,#&20
EB03 0383     	add.w	r3,r3,r3,lsl #2
4413           	adds	r3,r2
4423           	adds	r3,r4
7C18           	ldrb	r0,[r3,#&10]
E8BD 4070     	pop.w	{r4-r6,lr}
E761           	b	OSRAMWriteFinal

l00009874:
BD70           	pop	{r4-r6,pc}

l00009876:
BD70           	pop	{r4-r6,pc}
00009878                         04 A3 00 00 F4 A2 00 00         ........

;; OSRAMImageDraw: 00009880
OSRAMImageDraw proc
E92D 47F0     	push.w	{r4-r10,lr}
9E08           	ldr	r6,[sp,#&20]
B386           	cbz	r6,$000098EA

l00009888:
4605           	mov	r5,r0
4614           	mov	r4,r2
4699           	mov	r9,r3
3124           	adds	r1,#&24
F3C1 1803     	ubfx	r8,r1,#4,#4
4416           	adds	r6,r2
F048 0810     	orr	r8,r8,#&10
F001 070F     	and	r7,r1,#&F
F103 3AFF     	add	r10,r3,#&FFFFFFFF

l000098A2:
2080           	mov	r0,#&80
F7FF FEFA     	bl	OSRAMWriteFirst
2C00           	cmps	r4,#0
BF14           	ite	ne
20B1           	movne	r0,#&B1

l000098AE:
20B0           	mov	r0,#&B0
F7FF FF28     	bl	OSRAMWriteByte
2080           	mov	r0,#&80
F7FF FF25     	bl	OSRAMWriteByte
4638           	mov	r0,r7
F7FF FF22     	bl	OSRAMWriteByte
2080           	mov	r0,#&80
F7FF FF1F     	bl	OSRAMWriteByte
4640           	mov	r0,r8
F7FF FF1C     	bl	OSRAMWriteByte
2040           	mov	r0,#&40
F7FF FF19     	bl	OSRAMWriteByte
4628           	mov	r0,r5
4651           	mov	r1,r10
444D           	adds	r5,r9
F7FF FEF4     	bl	OSRAMWriteArray
3401           	adds	r4,#1
F815 0C01     	ldrb.w	r0,[r5,-#&1]
F7FF FF29     	bl	OSRAMWriteFinal
42A6           	cmps	r6,r4
D1DB           	bne	$000098A2

l000098EA:
E8BD 87F0     	pop.w	{r4-r10,pc}
000098EE                                           00 BF               ..

;; OSRAMInit: 000098F0
;;   Called from:
;;     000080B6 (in Main)
OSRAMInit proc
E92D 41F0     	push.w	{r4-r8,lr}
4604           	mov	r4,r0
F04F 2010     	mov	r0,#&10001000
F000 F93F     	bl	SysCtlPeripheralEnable
4818           	ldr	r0,[00009960]                           ; [pc,#&60]
F000 F93C     	bl	SysCtlPeripheralEnable
210C           	mov	r1,#&C
4817           	ldr	r0,[00009964]                           ; [pc,#&5C]
F7FF FDBA     	bl	GPIOPinTypeI2C
4621           	mov	r1,r4
4816           	ldr	r0,[00009968]                           ; [pc,#&58]
F000 FBF0     	bl	I2CMasterInit
2201           	mov	r2,#1
4B15           	ldr	r3,[0000996C]                           ; [pc,#&54]
4F15           	ldr	r7,[00009970]                           ; [pc,#&54]
26E3           	mov	r6,#&E3
2404           	mov	r4,#4
2080           	mov	r0,#&80
2500           	mov	r5,#0
601A           	str	r2,[r3]
F507 78F6     	add	r8,r7,#&1EC
E006           	b	$00009938

l0000992A:
F893 41EC     	ldrb	r4,[r3,#&1EC]
F893 01ED     	ldrb	r0,[r3,#&1ED]
4423           	adds	r3,r4
F893 61EC     	ldrb	r6,[r3,#&1EC]

l00009938:
F7FF FEB0     	bl	OSRAMWriteFirst
1CA8           	add	r0,r5,#2
1EA1           	sub	r1,r4,#2
4440           	adds	r0,r8
3401           	adds	r4,#1
F7FF FEBE     	bl	OSRAMWriteArray
4425           	adds	r5,r4
4630           	mov	r0,r6
F7FF FEF4     	bl	OSRAMWriteFinal
2D70           	cmps	r5,#&70
EB07 0305     	add.w	r3,r7,r5
D9E8           	bls	$0000992A

l00009958:
E8BD 41F0     	pop.w	{r4-r8,lr}
F7FF BF10     	b	OSRAMClear
00009960 02 00 00 20 00 50 00 40 00 00 02 40 7C 08 00 20 ... .P.@...@|.. 
00009970 F4 A2 00 00                                     ....            

;; OSRAMDisplayOn: 00009974
OSRAMDisplayOn proc
E92D 41F0     	push.w	{r4-r8,lr}
4F10           	ldr	r7,[000099BC]                           ; [pc,#&40]
26E3           	mov	r6,#&E3
2404           	mov	r4,#4
2080           	mov	r0,#&80
2500           	mov	r5,#0
F507 78F6     	add	r8,r7,#&1EC
E006           	b	$00009996

l00009988:
F893 41EC     	ldrb	r4,[r3,#&1EC]
F893 01ED     	ldrb	r0,[r3,#&1ED]
4423           	adds	r3,r4
F893 61EC     	ldrb	r6,[r3,#&1EC]

l00009996:
F7FF FE81     	bl	OSRAMWriteFirst
1CA8           	add	r0,r5,#2
1EA1           	sub	r1,r4,#2
4440           	adds	r0,r8
3401           	adds	r4,#1
F7FF FE8F     	bl	OSRAMWriteArray
4425           	adds	r5,r4
4630           	mov	r0,r6
F7FF FEC5     	bl	OSRAMWriteFinal
2D70           	cmps	r5,#&70
EB07 0305     	add.w	r3,r7,r5
D9E8           	bls	$00009988

l000099B6:
E8BD 81F0     	pop.w	{r4-r8,pc}
000099BA                               00 BF F4 A2 00 00           ......

;; OSRAMDisplayOff: 000099C0
OSRAMDisplayOff proc
B508           	push	{r3,lr}
2080           	mov	r0,#&80
F7FF FE6A     	bl	OSRAMWriteFirst
20AE           	mov	r0,#&AE
F7FF FE9B     	bl	OSRAMWriteByte
2080           	mov	r0,#&80
F7FF FE98     	bl	OSRAMWriteByte
20AD           	mov	r0,#&AD
F7FF FE95     	bl	OSRAMWriteByte
2080           	mov	r0,#&80
F7FF FE92     	bl	OSRAMWriteByte
E8BD 4008     	pop.w	{r3,lr}
208A           	mov	r0,#&8A
E6A7           	b	OSRAMWriteFinal

;; SSIConfig: 000099E8
;;   Called from:
;;     00008264 (in PDCInit)
SSIConfig proc
E92D 41F0     	push.w	{r4-r8,lr}
4617           	mov	r7,r2
4606           	mov	r6,r0
4688           	mov	r8,r1
461C           	mov	r4,r3
9D06           	ldr	r5,[sp,#&18]
F000 F9FB     	bl	SysCtlClockGet
2F02           	cmps	r7,#2
D018           	beq	$00009A30

l000099FE:
2F00           	cmps	r7,#0
BF18           	it	ne
2704           	movne	r7,#4

l00009A04:
FBB0 F3F4     	udiv	r3,r0,r4
2400           	mov	r4,#0
6077           	str	r7,[r6,#&4]

l00009A0C:
3402           	adds	r4,#2
FBB3 F2F4     	udiv	r2,r3,r4
3A01           	subs	r2,#1
2AFF           	cmps	r2,#&FF
D8F9           	bhi	$00009A0C

l00009A18:
F008 0330     	and	r3,r8,#&30
3D01           	subs	r5,#1
EA43 1188     	orr	r1,r3,r8,lsl #6
430D           	orrs	r5,r1
EA45 2202     	orr	r2,r5,r2,lsl #8
6134           	str	r4,[r6,#&10]
6032           	str	r2,[r6]
E8BD 81F0     	pop.w	{r4-r8,pc}

l00009A30:
270C           	mov	r7,#&C
E7E7           	b	$00009A04

;; SSIEnable: 00009A34
;;   Called from:
;;     0000826A (in PDCInit)
SSIEnable proc
6843           	ldr	r3,[r0,#&4]
F043 0302     	orr	r3,r3,#2
6043           	str	r3,[r0,#&4]
4770           	bx	lr
00009A3E                                           00 BF               ..

;; SSIDisable: 00009A40
SSIDisable proc
6843           	ldr	r3,[r0,#&4]
F023 0302     	bic	r3,r3,#2
6043           	str	r3,[r0,#&4]
4770           	bx	lr
00009A4A                               00 BF                       ..    

;; SSIIntRegister: 00009A4C
SSIIntRegister proc
B508           	push	{r3,lr}
2017           	mov	r0,#&17
F7FF FD58     	bl	IntRegister
E8BD 4008     	pop.w	{r3,lr}
2017           	mov	r0,#&17
F7FF BDBF     	b	IntEnable
00009A5E                                           00 BF               ..

;; SSIIntUnregister: 00009A60
SSIIntUnregister proc
B508           	push	{r3,lr}
2017           	mov	r0,#&17
F7FF FDE8     	bl	IntDisable
E8BD 4008     	pop.w	{r3,lr}
2017           	mov	r0,#&17
F7FF BD63     	b	IntUnregister
00009A72       00 BF                                       ..            

;; SSIIntEnable: 00009A74
SSIIntEnable proc
6943           	ldr	r3,[r0,#&14]
4319           	orrs	r1,r3
6141           	str	r1,[r0,#&14]
4770           	bx	lr

;; SSIIntDisable: 00009A7C
SSIIntDisable proc
6943           	ldr	r3,[r0,#&14]
EA23 0101     	bic.w	r1,r3,r1
6141           	str	r1,[r0,#&14]
4770           	bx	lr
00009A86                   00 BF                               ..        

;; SSIIntStatus: 00009A88
SSIIntStatus proc
B909           	cbnz	r1,$00009A8E

l00009A8A:
6980           	ldr	r0,[r0,#&18]
4770           	bx	lr

l00009A8E:
69C0           	ldr	r0,[r0,#&1C]
4770           	bx	lr
00009A92       00 BF                                       ..            

;; SSIIntClear: 00009A94
SSIIntClear proc
6201           	str	r1,[r0,#&20]
4770           	bx	lr

;; SSIDataPut: 00009A98
;;   Called from:
;;     000082AA (in PDCWrite)
;;     000082B2 (in PDCWrite)
SSIDataPut proc
F100 020C     	add	r2,r0,#&C

l00009A9C:
6813           	ldr	r3,[r2]
079B           	lsls	r3,r3,#&1E
D5FC           	bpl	$00009A9C

l00009AA2:
6081           	str	r1,[r0,#&8]
4770           	bx	lr
00009AA6                   00 BF                               ..        

;; SSIDataNonBlockingPut: 00009AA8
SSIDataNonBlockingPut proc
68C3           	ldr	r3,[r0,#&C]
F013 0302     	ands	r3,r3,#2
BF1A           	itte	ne
6081           	strne	r1,[r0,#&8]

l00009AB2:
2001           	mov	r0,#1
4618           	mov	r0,r3
4770           	bx	lr

;; SSIDataGet: 00009AB8
;;   Called from:
;;     000082BA (in PDCWrite)
;;     000082C2 (in PDCWrite)
SSIDataGet proc
F100 020C     	add	r2,r0,#&C

l00009ABC:
6813           	ldr	r3,[r2]
075B           	lsls	r3,r3,#&1D
D5FC           	bpl	$00009ABC

l00009AC2:
6883           	ldr	r3,[r0,#&8]
600B           	str	r3,[r1]
4770           	bx	lr

;; SSIDataNonBlockingGet: 00009AC8
SSIDataNonBlockingGet proc
68C3           	ldr	r3,[r0,#&C]
F013 0304     	ands	r3,r3,#4
BF1D           	ittte	ne
6883           	ldrne	r3,[r0,#&8]

l00009AD2:
2001           	mov	r0,#1
600B           	str	r3,[r1]
4618           	mov	r0,r3
4770           	bx	lr
00009ADA                               00 BF                       ..    

;; SysCtlSRAMSizeGet: 00009ADC
SysCtlSRAMSizeGet proc
4B03           	ldr	r3,[00009AEC]                           ; [pc,#&C]
4804           	ldr	r0,[00009AF0]                           ; [pc,#&10]
681B           	ldr	r3,[r3]
EA00 2013     	and.w	r0,r0,r3,lsr #8
F500 7080     	add	r0,r0,#&100
4770           	bx	lr
00009AEC                                     08 E0 0F 40             ...@
00009AF0 00 FF FF 00                                     ....            

;; SysCtlFlashSizeGet: 00009AF4
SysCtlFlashSizeGet proc
4B03           	ldr	r3,[00009B04]                           ; [pc,#&C]
4804           	ldr	r0,[00009B08]                           ; [pc,#&10]
681B           	ldr	r3,[r3]
EA00 20C3     	and.w	r0,r0,r3,lsl #&B
F500 6000     	add	r0,r0,#&800
4770           	bx	lr
00009B04             08 E0 0F 40 00 F8 FF 07                 ...@....    

;; SysCtlPinPresent: 00009B0C
SysCtlPinPresent proc
4B03           	ldr	r3,[00009B1C]                           ; [pc,#&C]
681B           	ldr	r3,[r3]
4203           	tst	r3,r0
BF14           	ite	ne
2001           	movne	r0,#1

l00009B16:
2000           	mov	r0,#0
4770           	bx	lr
00009B1A                               00 BF 18 E0 0F 40           .....@

;; SysCtlPeripheralPresent: 00009B20
SysCtlPeripheralPresent proc
4B05           	ldr	r3,[00009B38]                           ; [pc,#&14]
0F02           	lsrs	r2,r0,#&1C
F853 3022     	ldr.w	r3,[r3,r2,lsl #2]
F020 4070     	bic	r0,r0,#&F0000000
681B           	ldr	r3,[r3]
4218           	tst	r0,r3
BF14           	ite	ne
2001           	movne	r0,#1

l00009B34:
2000           	mov	r0,#0
4770           	bx	lr
00009B38                         54 A5 00 00                     T...    

;; SysCtlPeripheralReset: 00009B3C
SysCtlPeripheralReset proc
2100           	mov	r1,#0
4B0E           	ldr	r3,[00009B78]                           ; [pc,#&38]
0F02           	lsrs	r2,r0,#&1C
B410           	push	{r4}
EB03 0382     	add.w	r3,r3,r2,lsl #2
691A           	ldr	r2,[r3,#&10]
F020 4370     	bic	r3,r0,#&F0000000
6814           	ldr	r4,[r2]
B083           	sub	sp,#&C
4323           	orrs	r3,r4
6013           	str	r3,[r2]
9101           	str	r1,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
2B0F           	cmps	r3,#&F
D805           	bhi	$00009B6A

l00009B5E:
9B01           	ldr	r3,[sp,#&4]
3301           	adds	r3,#1
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
2B0F           	cmps	r3,#&F
D9F9           	bls	$00009B5E

l00009B6A:
6813           	ldr	r3,[r2]
EA23 0000     	bic.w	r0,r3,r0
6010           	str	r0,[r2]
B003           	add	sp,#&C
BC10           	pop	{r4}
4770           	bx	lr
00009B78                         54 A5 00 00                     T...    

;; SysCtlPeripheralEnable: 00009B7C
;;   Called from:
;;     00008226 (in PDCInit)
;;     0000822C (in PDCInit)
;;     000098FA (in OSRAMInit)
;;     00009900 (in OSRAMInit)
SysCtlPeripheralEnable proc
4B05           	ldr	r3,[00009B94]                           ; [pc,#&14]
0F02           	lsrs	r2,r0,#&1C
EB03 0382     	add.w	r3,r3,r2,lsl #2
69DB           	ldr	r3,[r3,#&1C]
F020 4070     	bic	r0,r0,#&F0000000
681A           	ldr	r2,[r3]
4310           	orrs	r0,r2
6018           	str	r0,[r3]
4770           	bx	lr
00009B92       00 BF 54 A5 00 00                           ..T...        

;; SysCtlPeripheralDisable: 00009B98
SysCtlPeripheralDisable proc
4B05           	ldr	r3,[00009BB0]                           ; [pc,#&14]
0F02           	lsrs	r2,r0,#&1C
EB03 0382     	add.w	r3,r3,r2,lsl #2
69DA           	ldr	r2,[r3,#&1C]
F020 4070     	bic	r0,r0,#&F0000000
6813           	ldr	r3,[r2]
EA23 0000     	bic.w	r0,r3,r0
6010           	str	r0,[r2]
4770           	bx	lr
00009BB0 54 A5 00 00                                     T...            

;; SysCtlPeripheralSleepEnable: 00009BB4
SysCtlPeripheralSleepEnable proc
4B05           	ldr	r3,[00009BCC]                           ; [pc,#&14]
0F02           	lsrs	r2,r0,#&1C
EB03 0382     	add.w	r3,r3,r2,lsl #2
6A9B           	ldr	r3,[r3,#&28]
F020 4070     	bic	r0,r0,#&F0000000
681A           	ldr	r2,[r3]
4310           	orrs	r0,r2
6018           	str	r0,[r3]
4770           	bx	lr
00009BCA                               00 BF 54 A5 00 00           ..T...

;; SysCtlPeripheralSleepDisable: 00009BD0
SysCtlPeripheralSleepDisable proc
4B05           	ldr	r3,[00009BE8]                           ; [pc,#&14]
0F02           	lsrs	r2,r0,#&1C
EB03 0382     	add.w	r3,r3,r2,lsl #2
6A9A           	ldr	r2,[r3,#&28]
F020 4070     	bic	r0,r0,#&F0000000
6813           	ldr	r3,[r2]
EA23 0000     	bic.w	r0,r3,r0
6010           	str	r0,[r2]
4770           	bx	lr
00009BE8                         54 A5 00 00                     T...    

;; SysCtlPeripheralDeepSleepEnable: 00009BEC
SysCtlPeripheralDeepSleepEnable proc
4B05           	ldr	r3,[00009C04]                           ; [pc,#&14]
0F02           	lsrs	r2,r0,#&1C
EB03 0382     	add.w	r3,r3,r2,lsl #2
6B5B           	ldr	r3,[r3,#&34]
F020 4070     	bic	r0,r0,#&F0000000
681A           	ldr	r2,[r3]
4310           	orrs	r0,r2
6018           	str	r0,[r3]
4770           	bx	lr
00009C02       00 BF 54 A5 00 00                           ..T...        

;; SysCtlPeripheralDeepSleepDisable: 00009C08
SysCtlPeripheralDeepSleepDisable proc
4B05           	ldr	r3,[00009C20]                           ; [pc,#&14]
0F02           	lsrs	r2,r0,#&1C
EB03 0382     	add.w	r3,r3,r2,lsl #2
6B5A           	ldr	r2,[r3,#&34]
F020 4070     	bic	r0,r0,#&F0000000
6813           	ldr	r3,[r2]
EA23 0000     	bic.w	r0,r3,r0
6010           	str	r0,[r2]
4770           	bx	lr
00009C20 54 A5 00 00                                     T...            

;; SysCtlPeripheralClockGating: 00009C24
SysCtlPeripheralClockGating proc
4A05           	ldr	r2,[00009C3C]                           ; [pc,#&14]
6813           	ldr	r3,[r2]
B918           	cbnz	r0,$00009C32

l00009C2A:
F023 6300     	bic	r3,r3,#&8000000
6013           	str	r3,[r2]
4770           	bx	lr

l00009C32:
F043 6300     	orr	r3,r3,#&8000000
6013           	str	r3,[r2]
4770           	bx	lr
00009C3A                               00 BF 60 E0 0F 40           ..`..@

;; SysCtlIntRegister: 00009C40
SysCtlIntRegister proc
B508           	push	{r3,lr}
4601           	mov	r1,r0
202C           	mov	r0,#&2C
F7FF FC5D     	bl	IntRegister
E8BD 4008     	pop.w	{r3,lr}
202C           	mov	r0,#&2C
F7FF BCC4     	b	IntEnable

;; SysCtlIntUnregister: 00009C54
SysCtlIntUnregister proc
B508           	push	{r3,lr}
202C           	mov	r0,#&2C
F7FF FCEE     	bl	IntDisable
E8BD 4008     	pop.w	{r3,lr}
202C           	mov	r0,#&2C
F7FF BC69     	b	IntUnregister
00009C66                   00 BF                               ..        

;; SysCtlIntEnable: 00009C68
SysCtlIntEnable proc
4A02           	ldr	r2,[00009C74]                           ; [pc,#&8]
6813           	ldr	r3,[r2]
4318           	orrs	r0,r3
6010           	str	r0,[r2]
4770           	bx	lr
00009C72       00 BF 54 E0 0F 40                           ..T..@        

;; SysCtlIntDisable: 00009C78
SysCtlIntDisable proc
4A02           	ldr	r2,[00009C84]                           ; [pc,#&8]
6813           	ldr	r3,[r2]
EA23 0000     	bic.w	r0,r3,r0
6010           	str	r0,[r2]
4770           	bx	lr
00009C84             54 E0 0F 40                             T..@        

;; SysCtlIntClear: 00009C88
SysCtlIntClear proc
4B01           	ldr	r3,[00009C90]                           ; [pc,#&4]
6018           	str	r0,[r3]
4770           	bx	lr
00009C8E                                           00 BF               ..
00009C90 58 E0 0F 40                                     X..@            

;; SysCtlIntStatus: 00009C94
SysCtlIntStatus proc
B910           	cbnz	r0,$00009C9C

l00009C96:
4B03           	ldr	r3,[00009CA4]                           ; [pc,#&C]
6818           	ldr	r0,[r3]
4770           	bx	lr

l00009C9C:
4B02           	ldr	r3,[00009CA8]                           ; [pc,#&8]
6818           	ldr	r0,[r3]
4770           	bx	lr
00009CA2       00 BF 50 E0 0F 40 58 E0 0F 40               ..P..@X..@    

;; SysCtlLDOSet: 00009CAC
SysCtlLDOSet proc
4B01           	ldr	r3,[00009CB4]                           ; [pc,#&4]
6018           	str	r0,[r3]
4770           	bx	lr
00009CB2       00 BF 34 E0 0F 40                           ..4..@        

;; SysCtlLDOGet: 00009CB8
SysCtlLDOGet proc
4B01           	ldr	r3,[00009CC0]                           ; [pc,#&4]
6818           	ldr	r0,[r3]
4770           	bx	lr
00009CBE                                           00 BF               ..
00009CC0 34 E0 0F 40                                     4..@            

;; SysCtlLDOConfigSet: 00009CC4
SysCtlLDOConfigSet proc
4B01           	ldr	r3,[00009CCC]                           ; [pc,#&4]
6018           	str	r0,[r3]
4770           	bx	lr
00009CCA                               00 BF 60 E1 0F 40           ..`..@

;; SysCtlReset: 00009CD0
SysCtlReset proc
4B01           	ldr	r3,[00009CD8]                           ; [pc,#&4]
4A02           	ldr	r2,[00009CDC]                           ; [pc,#&8]
601A           	str	r2,[r3]

l00009CD6:
E7FE           	b	$00009CD6
00009CD8                         0C ED 00 E0 04 00 FA 05         ........

;; SysCtlSleep: 00009CE0
SysCtlSleep proc
F000 BA04     	b	CPUwfi

;; SysCtlDeepSleep: 00009CE4
SysCtlDeepSleep proc
B510           	push	{r4,lr}
4C06           	ldr	r4,[00009D00]                           ; [pc,#&18]
6823           	ldr	r3,[r4]
F043 0304     	orr	r3,r3,#4
6023           	str	r3,[r4]
F000 F9FC     	bl	CPUwfi
6823           	ldr	r3,[r4]
F023 0304     	bic	r3,r3,#4
6023           	str	r3,[r4]
BD10           	pop	{r4,pc}
00009CFE                                           00 BF               ..
00009D00 10 ED 00 E0                                     ....            

;; SysCtlResetCauseGet: 00009D04
SysCtlResetCauseGet proc
4B01           	ldr	r3,[00009D0C]                           ; [pc,#&4]
6818           	ldr	r0,[r3]
4770           	bx	lr
00009D0A                               00 BF 5C E0 0F 40           ..\..@

;; SysCtlResetCauseClear: 00009D10
SysCtlResetCauseClear proc
4A02           	ldr	r2,[00009D1C]                           ; [pc,#&8]
6813           	ldr	r3,[r2]
EA23 0000     	bic.w	r0,r3,r0
6010           	str	r0,[r2]
4770           	bx	lr
00009D1C                                     5C E0 0F 40             \..@

;; SysCtlBrownOutConfigSet: 00009D20
SysCtlBrownOutConfigSet proc
4B02           	ldr	r3,[00009D2C]                           ; [pc,#&8]
EA40 0181     	orr	r1,r0,r1,lsl #2
6019           	str	r1,[r3]
4770           	bx	lr
00009D2A                               00 BF 30 E0 0F 40           ..0..@

;; SysCtlClockSet: 00009D30
SysCtlClockSet proc
F243 32F0     	mov	r2,#&33F0
B4F0           	push	{r4-r7}
2740           	mov	r7,#&40
2600           	mov	r6,#0
4C29           	ldr	r4,[00009DE0]                           ; [pc,#&A4]
4929           	ldr	r1,[00009DE4]                           ; [pc,#&A4]
6823           	ldr	r3,[r4]
F060 0503     	orn	r5,r0,#3
4019           	ands	r1,r3
F441 6100     	orr	r1,r1,#&800
4029           	ands	r1,r5
4002           	ands	r2,r0
F423 0380     	bic	r3,r3,#&400000
4D25           	ldr	r5,[00009DE8]                           ; [pc,#&94]
B082           	sub	sp,#8
F443 6300     	orr	r3,r3,#&800
430A           	orrs	r2,r1
6023           	str	r3,[r4]
602F           	str	r7,[r5]
6022           	str	r2,[r4]
9601           	str	r6,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
2B0F           	cmps	r3,#&F
D805           	bhi	$00009D76

l00009D6A:
9B01           	ldr	r3,[sp,#&4]
3301           	adds	r3,#1
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
2B0F           	cmps	r3,#&F
D9F9           	bls	$00009D6A

l00009D76:
F000 0303     	and	r3,r0,#3
4C19           	ldr	r4,[00009DE0]                           ; [pc,#&64]
F022 0203     	bic	r2,r2,#3
431A           	orrs	r2,r3
F022 63F8     	bic	r3,r2,#&7C00000
F000 61F8     	and	r1,r0,#&7C00000
6022           	str	r2,[r4]
0504           	lsls	r4,r0,#&14
EA41 0103     	orr	r1,r1,r3
D414           	bmi	$00009DBE

l00009D94:
F44F 4300     	mov	r3,#&8000
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
B16B           	cbz	r3,$00009DBA

l00009D9E:
4A13           	ldr	r2,[00009DEC]                           ; [pc,#&4C]
6813           	ldr	r3,[r2]
0658           	lsls	r0,r3,#&19
D503           	bpl	$00009DAE

l00009DA6:
E008           	b	$00009DBA

l00009DA8:
6813           	ldr	r3,[r2]
065B           	lsls	r3,r3,#&19
D405           	bmi	$00009DBA

l00009DAE:
9B01           	ldr	r3,[sp,#&4]
3B01           	subs	r3,#1
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
2B00           	cmps	r3,#0
D1F6           	bne	$00009DA8

l00009DBA:
F421 6100     	bic	r1,r1,#&800

l00009DBE:
2300           	mov	r3,#0
4A07           	ldr	r2,[00009DE0]                           ; [pc,#&1C]
6011           	str	r1,[r2]
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
2B0F           	cmps	r3,#&F
D805           	bhi	$00009DD8

l00009DCC:
9B01           	ldr	r3,[sp,#&4]
3301           	adds	r3,#1
9301           	str	r3,[sp,#&4]
9B01           	ldr	r3,[sp,#&4]
2B0F           	cmps	r3,#&F
D9F9           	bls	$00009DCC

l00009DD8:
B002           	add	sp,#8
BCF0           	pop	{r4-r7}
4770           	bx	lr
00009DDE                                           00 BF               ..
00009DE0 60 E0 0F 40 0F CC BF FF 58 E0 0F 40 50 E0 0F 40 `..@....X..@P..@

;; SysCtlClockGet: 00009DF0
;;   Called from:
;;     000099F6 (in SSIConfig)
;;     00009F72 (in UARTConfigSet)
;;     00009FB8 (in UARTConfigGet)
;;     0000A102 (in I2CMasterInit)
SysCtlClockGet proc
4B18           	ldr	r3,[00009E54]                           ; [pc,#&60]
681B           	ldr	r3,[r3]
F003 0230     	and	r2,r3,#&30
2A10           	cmps	r2,#&10
D028           	beq	$00009E4E

l00009DFC:
2A20           	cmps	r2,#&20
D024           	beq	$00009E4A

l00009E00:
B10A           	cbz	r2,$00009E06

l00009E02:
2000           	mov	r0,#0

l00009E04:
4770           	bx	lr

l00009E06:
4A14           	ldr	r2,[00009E58]                           ; [pc,#&50]
F3C3 1183     	ubfx	r1,r3,#6,#4
EB02 0281     	add.w	r2,r2,r1,lsl #2
6B10           	ldr	r0,[r2,#&30]

l00009E12:
051A           	lsls	r2,r3,#&14
D411           	bmi	$00009E3A

l00009E16:
4A11           	ldr	r2,[00009E5C]                           ; [pc,#&44]
6812           	ldr	r2,[r2]
F3C2 1148     	ubfx	r1,r2,#5,#9
3102           	adds	r1,#2
FB00 F001     	mul	r0,r0,r1
F002 011F     	and	r1,r2,#&1F
3102           	adds	r1,#2
FBB0 F0F1     	udiv	r0,r0,r1
0451           	lsls	r1,r2,#&11
BF48           	it	mi
0840           	lsrsmi	r0,r0,#1

l00009E34:
0411           	lsls	r1,r2,#&10
BF48           	it	mi
0880           	lsrsmi	r0,r0,#2

l00009E3A:
025A           	lsls	r2,r3,#9
D5E2           	bpl	$00009E04

l00009E3E:
F3C3 53C3     	ubfx	r3,r3,#&17,#4
3301           	adds	r3,#1
FBB0 F0F3     	udiv	r0,r0,r3
4770           	bx	lr

l00009E4A:
4805           	ldr	r0,[00009E60]                           ; [pc,#&14]
E7E1           	b	$00009E12

l00009E4E:
4805           	ldr	r0,[00009E64]                           ; [pc,#&14]
E7DF           	b	$00009E12
00009E52       00 BF 60 E0 0F 40 54 A5 00 00 64 E0 0F 40   ..`..@T...d..@
00009E60 70 38 39 00 C0 E1 E4 00                         p89.....        

;; SysCtlPWMClockSet: 00009E68
SysCtlPWMClockSet proc
4A03           	ldr	r2,[00009E78]                           ; [pc,#&C]
6813           	ldr	r3,[r2]
F423 13F0     	bic	r3,r3,#&1E0000
4318           	orrs	r0,r3
6010           	str	r0,[r2]
4770           	bx	lr
00009E76                   00 BF 60 E0 0F 40                   ..`..@    

;; SysCtlPWMClockGet: 00009E7C
SysCtlPWMClockGet proc
4B02           	ldr	r3,[00009E88]                           ; [pc,#&8]
6818           	ldr	r0,[r3]
F400 10F0     	and	r0,r0,#&1E0000
4770           	bx	lr
00009E86                   00 BF 60 E0 0F 40                   ..`..@    

;; SysCtlADCSpeedSet: 00009E8C
SysCtlADCSpeedSet proc
B410           	push	{r4}
4C0A           	ldr	r4,[00009EB8]                           ; [pc,#&28]
490A           	ldr	r1,[00009EBC]                           ; [pc,#&28]
6823           	ldr	r3,[r4]
4A0A           	ldr	r2,[00009EC0]                           ; [pc,#&28]
F423 6370     	bic	r3,r3,#&F00
4303           	orrs	r3,r0
6023           	str	r3,[r4]
680B           	ldr	r3,[r1]
BC10           	pop	{r4}
F423 6370     	bic	r3,r3,#&F00
4303           	orrs	r3,r0
600B           	str	r3,[r1]
6813           	ldr	r3,[r2]
F423 6370     	bic	r3,r3,#&F00
4318           	orrs	r0,r3
6010           	str	r0,[r2]
4770           	bx	lr
00009EB6                   00 BF 00 E1 0F 40 10 E1 0F 40       .....@...@
00009EC0 20 E1 0F 40                                      ..@            

;; SysCtlADCSpeedGet: 00009EC4
SysCtlADCSpeedGet proc
4B02           	ldr	r3,[00009ED0]                           ; [pc,#&8]
6818           	ldr	r0,[r3]
F400 6070     	and	r0,r0,#&F00
4770           	bx	lr
00009ECE                                           00 BF               ..
00009ED0 00 E1 0F 40                                     ...@            

;; SysCtlIOSCVerificationSet: 00009ED4
SysCtlIOSCVerificationSet proc
4A05           	ldr	r2,[00009EEC]                           ; [pc,#&14]
6813           	ldr	r3,[r2]
B918           	cbnz	r0,$00009EE2

l00009EDA:
F023 0308     	bic	r3,r3,#8
6013           	str	r3,[r2]
4770           	bx	lr

l00009EE2:
F043 0308     	orr	r3,r3,#8
6013           	str	r3,[r2]
4770           	bx	lr
00009EEA                               00 BF 60 E0 0F 40           ..`..@

;; SysCtlMOSCVerificationSet: 00009EF0
SysCtlMOSCVerificationSet proc
4A05           	ldr	r2,[00009F08]                           ; [pc,#&14]
6813           	ldr	r3,[r2]
B918           	cbnz	r0,$00009EFE

l00009EF6:
F023 0304     	bic	r3,r3,#4
6013           	str	r3,[r2]
4770           	bx	lr

l00009EFE:
F043 0304     	orr	r3,r3,#4
6013           	str	r3,[r2]
4770           	bx	lr
00009F06                   00 BF 60 E0 0F 40                   ..`..@    

;; SysCtlPLLVerificationSet: 00009F0C
SysCtlPLLVerificationSet proc
4A05           	ldr	r2,[00009F24]                           ; [pc,#&14]
6813           	ldr	r3,[r2]
B918           	cbnz	r0,$00009F1A

l00009F12:
F423 6380     	bic	r3,r3,#&400
6013           	str	r3,[r2]
4770           	bx	lr

l00009F1A:
F443 6380     	orr	r3,r3,#&400
6013           	str	r3,[r2]
4770           	bx	lr
00009F22       00 BF 60 E0 0F 40                           ..`..@        

;; SysCtlClkVerificationClear: 00009F28
SysCtlClkVerificationClear proc
2101           	mov	r1,#1
2200           	mov	r2,#0
4B01           	ldr	r3,[00009F34]                           ; [pc,#&4]
6019           	str	r1,[r3]
601A           	str	r2,[r3]
4770           	bx	lr
00009F34             50 E1 0F 40                             P..@        

;; UARTParityModeSet: 00009F38
UARTParityModeSet proc
6AC3           	ldr	r3,[r0,#&2C]
F023 0386     	bic	r3,r3,#&86
4319           	orrs	r1,r3
62C1           	str	r1,[r0,#&2C]
4770           	bx	lr

;; UARTParityModeGet: 00009F44
UARTParityModeGet proc
6AC0           	ldr	r0,[r0,#&2C]
F000 0086     	and	r0,r0,#&86
4770           	bx	lr

;; UARTConfigSet: 00009F4C
UARTConfigSet proc
B5F8           	push	{r3-r7,lr}
460F           	mov	r7,r1
4616           	mov	r6,r2
4605           	mov	r5,r0
3018           	adds	r0,#&18

l00009F56:
6804           	ldr	r4,[r0]
F014 0408     	ands	r4,r4,#8
D1FB           	bne	$00009F56

l00009F5E:
6AEB           	ldr	r3,[r5,#&2C]
F023 0310     	bic	r3,r3,#&10
62EB           	str	r3,[r5,#&2C]
6B2A           	ldr	r2,[r5,#&30]
F422 7240     	bic	r2,r2,#&300
F022 0201     	bic	r2,r2,#1
632A           	str	r2,[r5,#&30]
F7FF FF3D     	bl	SysCtlClockGet
013B           	lsls	r3,r7,#4
FBB0 F2F3     	udiv	r2,r0,r3
FB03 0312     	mls	r3,r3,r2,r0
00DB           	lsls	r3,r3,#3
FBB3 F3F7     	udiv	r3,r3,r7
3301           	adds	r3,#1
085B           	lsrs	r3,r3,#1
626A           	str	r2,[r5,#&24]
62AB           	str	r3,[r5,#&28]
62EE           	str	r6,[r5,#&2C]
61AC           	str	r4,[r5,#&18]
6AEB           	ldr	r3,[r5,#&2C]
F043 0310     	orr	r3,r3,#&10
62EB           	str	r3,[r5,#&2C]
6B2B           	ldr	r3,[r5,#&30]
F443 7340     	orr	r3,r3,#&300
F043 0301     	orr	r3,r3,#1
632B           	str	r3,[r5,#&30]
BDF8           	pop	{r3-r7,pc}

;; UARTConfigGet: 00009FA8
UARTConfigGet proc
E92D 41F0     	push.w	{r4-r8,lr}
F8D0 8024     	ldr	r8,[r0,#&24]
4604           	mov	r4,r0
460F           	mov	r7,r1
4616           	mov	r6,r2
6A85           	ldr	r5,[r0,#&28]
F7FF FF1A     	bl	SysCtlClockGet
EB05 1588     	add.w	r5,r5,r8,lsl #6
0080           	lsls	r0,r0,#2
FBB0 F0F5     	udiv	r0,r0,r5
6038           	str	r0,[r7]
6AE3           	ldr	r3,[r4,#&2C]
F003 03EE     	and	r3,r3,#&EE
6033           	str	r3,[r6]
E8BD 81F0     	pop.w	{r4-r8,pc}

;; UARTEnable: 00009FD4
UARTEnable proc
6AC3           	ldr	r3,[r0,#&2C]
F043 0310     	orr	r3,r3,#&10
62C3           	str	r3,[r0,#&2C]
6B03           	ldr	r3,[r0,#&30]
F443 7340     	orr	r3,r3,#&300
F043 0301     	orr	r3,r3,#1
6303           	str	r3,[r0,#&30]
4770           	bx	lr
00009FEA                               00 BF                       ..    

;; UARTDisable: 00009FEC
UARTDisable proc
F100 0218     	add	r2,r0,#&18

l00009FF0:
6813           	ldr	r3,[r2]
071B           	lsls	r3,r3,#&1C
D4FC           	bmi	$00009FF0

l00009FF6:
6AC3           	ldr	r3,[r0,#&2C]
F023 0310     	bic	r3,r3,#&10
62C3           	str	r3,[r0,#&2C]
6B03           	ldr	r3,[r0,#&30]
F423 7340     	bic	r3,r3,#&300
F023 0301     	bic	r3,r3,#1
6303           	str	r3,[r0,#&30]
4770           	bx	lr

;; UARTCharsAvail: 0000A00C
UARTCharsAvail proc
6980           	ldr	r0,[r0,#&18]
F080 0010     	eor	r0,r0,#&10
F3C0 1000     	ubfx	r0,r0,#4,#1
4770           	bx	lr

;; UARTSpaceAvail: 0000A018
UARTSpaceAvail proc
6980           	ldr	r0,[r0,#&18]
F080 0020     	eor	r0,r0,#&20
F3C0 1040     	ubfx	r0,r0,#5,#1
4770           	bx	lr

;; UARTCharNonBlockingGet: 0000A024
UARTCharNonBlockingGet proc
6983           	ldr	r3,[r0,#&18]
06DB           	lsls	r3,r3,#&1B
BF54           	ite	pl
6800           	ldrpl	r0,[r0]

l0000A02C:
F04F 30FF     	mov	r0,#&FFFFFFFF
4770           	bx	lr
0000A032       00 BF                                       ..            

;; UARTCharGet: 0000A034
UARTCharGet proc
F100 0218     	add	r2,r0,#&18

l0000A038:
6813           	ldr	r3,[r2]
06DB           	lsls	r3,r3,#&1B
D4FC           	bmi	$0000A038

l0000A03E:
6800           	ldr	r0,[r0]
4770           	bx	lr
0000A042       00 BF                                       ..            

;; UARTCharNonBlockingPut: 0000A044
UARTCharNonBlockingPut proc
6983           	ldr	r3,[r0,#&18]
069B           	lsls	r3,r3,#&1A
BF5A           	itte	pl
6001           	strpl	r1,[r0]

l0000A04C:
2001           	mov	r0,#1
2000           	mov	r0,#0
4770           	bx	lr
0000A052       00 BF                                       ..            

;; UARTCharPut: 0000A054
UARTCharPut proc
F100 0218     	add	r2,r0,#&18

l0000A058:
6813           	ldr	r3,[r2]
069B           	lsls	r3,r3,#&1A
D4FC           	bmi	$0000A058

l0000A05E:
6001           	str	r1,[r0]
4770           	bx	lr
0000A062       00 BF                                       ..            

;; UARTBreakCtl: 0000A064
UARTBreakCtl proc
6AC3           	ldr	r3,[r0,#&2C]
B919           	cbnz	r1,$0000A070

l0000A068:
F023 0301     	bic	r3,r3,#1
62C3           	str	r3,[r0,#&2C]
4770           	bx	lr

l0000A070:
F043 0301     	orr	r3,r3,#1
62C3           	str	r3,[r0,#&2C]
4770           	bx	lr

;; UARTIntRegister: 0000A078
UARTIntRegister proc
B510           	push	{r4,lr}
4C06           	ldr	r4,[0000A094]                           ; [pc,#&18]
42A0           	cmps	r0,r4
BF0C           	ite	eq
2415           	moveq	r4,#&15

l0000A082:
2416           	mov	r4,#&16
4620           	mov	r0,r4
F7FF FA3D     	bl	IntRegister
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F7FF BAA4     	b	IntEnable
0000A094             00 C0 00 40                             ...@        

;; UARTIntUnregister: 0000A098
UARTIntUnregister proc
B510           	push	{r4,lr}
4C06           	ldr	r4,[0000A0B4]                           ; [pc,#&18]
42A0           	cmps	r0,r4
BF0C           	ite	eq
2415           	moveq	r4,#&15

l0000A0A2:
2416           	mov	r4,#&16
4620           	mov	r0,r4
F7FF FAC7     	bl	IntDisable
4620           	mov	r0,r4
E8BD 4010     	pop.w	{r4,lr}
F7FF BA42     	b	IntUnregister
0000A0B4             00 C0 00 40                             ...@        

;; UARTIntEnable: 0000A0B8
UARTIntEnable proc
6B83           	ldr	r3,[r0,#&38]
4319           	orrs	r1,r3
6381           	str	r1,[r0,#&38]
4770           	bx	lr

;; UARTIntDisable: 0000A0C0
UARTIntDisable proc
6B83           	ldr	r3,[r0,#&38]
EA23 0101     	bic.w	r1,r3,r1
6381           	str	r1,[r0,#&38]
4770           	bx	lr
0000A0CA                               00 BF                       ..    

;; UARTIntStatus: 0000A0CC
;;   Called from:
;;     00008116 (in vUART_ISR)
UARTIntStatus proc
B909           	cbnz	r1,$0000A0D2

l0000A0CE:
6BC0           	ldr	r0,[r0,#&3C]
4770           	bx	lr

l0000A0D2:
6C00           	ldr	r0,[r0,#&40]
4770           	bx	lr
0000A0D6                   00 BF                               ..        

;; UARTIntClear: 0000A0D8
;;   Called from:
;;     00008120 (in vUART_ISR)
UARTIntClear proc
6441           	str	r1,[r0,#&44]
4770           	bx	lr

;; CPUcpsie: 0000A0DC
;;   Called from:
;;     000094FC (in IntMasterEnable)
CPUcpsie proc
B662           	cps	#0
4770           	bx	lr
0000A0E0 70 47 00 BF                                     pG..            

;; CPUcpsid: 0000A0E4
;;   Called from:
;;     00009500 (in IntMasterDisable)
CPUcpsid proc
B672           	cps	#0
4770           	bx	lr
0000A0E8                         70 47 00 BF                     pG..    

;; CPUwfi: 0000A0EC
;;   Called from:
;;     00009CE0 (in SysCtlSleep)
;;     00009CF0 (in SysCtlDeepSleep)
CPUwfi proc
BF30           	wfi
4770           	bx	lr
0000A0F0 70 47 00 BF                                     pG..            

;; I2CMasterInit: 0000A0F4
;;   Called from:
;;     00009910 (in OSRAMInit)
I2CMasterInit proc
B538           	push	{r3-r5,lr}
460D           	mov	r5,r1
6A02           	ldr	r2,[r0,#&20]
4604           	mov	r4,r0
F042 0210     	orr	r2,r2,#&10
6202           	str	r2,[r0,#&20]
F7FF FE75     	bl	SysCtlClockGet
4B06           	ldr	r3,[0000A120]                           ; [pc,#&18]
4A06           	ldr	r2,[0000A124]                           ; [pc,#&18]
3801           	subs	r0,#1
2D01           	cmps	r5,#1
BF08           	it	eq
4613           	moveq	r3,r2

l0000A112:
18C1           	add	r1,r0,r3
FBB1 F1F3     	udiv	r1,r1,r3
3901           	subs	r1,#1
60E1           	str	r1,[r4,#&C]
BD38           	pop	{r3-r5,pc}
0000A11E                                           00 BF               ..
0000A120 80 84 1E 00 00 12 7A 00                         ......z.        

;; I2CSlaveInit: 0000A128
I2CSlaveInit proc
B410           	push	{r4}
2401           	mov	r4,#1
F5A0 62FC     	sub	r2,r0,#&7E0
6813           	ldr	r3,[r2]
F043 0320     	orr	r3,r3,#&20
6013           	str	r3,[r2]
6044           	str	r4,[r0,#&4]
6001           	str	r1,[r0]
BC10           	pop	{r4}
4770           	bx	lr

;; I2CMasterEnable: 0000A140
I2CMasterEnable proc
6A03           	ldr	r3,[r0,#&20]
F043 0310     	orr	r3,r3,#&10
6203           	str	r3,[r0,#&20]
4770           	bx	lr
0000A14A                               00 BF                       ..    

;; I2CSlaveEnable: 0000A14C
I2CSlaveEnable proc
2101           	mov	r1,#1
F5A0 62FC     	sub	r2,r0,#&7E0
6813           	ldr	r3,[r2]
F043 0320     	orr	r3,r3,#&20
6013           	str	r3,[r2]
6041           	str	r1,[r0,#&4]
4770           	bx	lr
0000A15E                                           00 BF               ..

;; I2CMasterDisable: 0000A160
I2CMasterDisable proc
6A03           	ldr	r3,[r0,#&20]
F023 0310     	bic	r3,r3,#&10
6203           	str	r3,[r0,#&20]
4770           	bx	lr
0000A16A                               00 BF                       ..    

;; I2CSlaveDisable: 0000A16C
I2CSlaveDisable proc
2300           	mov	r3,#0
F5A0 62FC     	sub	r2,r0,#&7E0
6043           	str	r3,[r0,#&4]
6813           	ldr	r3,[r2]
F023 0320     	bic	r3,r3,#&20
6013           	str	r3,[r2]
4770           	bx	lr
0000A17E                                           00 BF               ..

;; I2CIntRegister: 0000A180
I2CIntRegister proc
B508           	push	{r3,lr}
2018           	mov	r0,#&18
F7FF F9BE     	bl	IntRegister
E8BD 4008     	pop.w	{r3,lr}
2018           	mov	r0,#&18
F7FF BA25     	b	IntEnable
0000A192       00 BF                                       ..            

;; I2CIntUnregister: 0000A194
I2CIntUnregister proc
B508           	push	{r3,lr}
2018           	mov	r0,#&18
F7FF FA4E     	bl	IntDisable
E8BD 4008     	pop.w	{r3,lr}
2018           	mov	r0,#&18
F7FF B9C9     	b	IntUnregister
0000A1A6                   00 BF                               ..        

;; I2CMasterIntEnable: 0000A1A8
I2CMasterIntEnable proc
2301           	mov	r3,#1
6103           	str	r3,[r0,#&10]
4770           	bx	lr
0000A1AE                                           00 BF               ..

;; I2CSlaveIntEnable: 0000A1B0
I2CSlaveIntEnable proc
2301           	mov	r3,#1
60C3           	str	r3,[r0,#&C]
4770           	bx	lr
0000A1B6                   00 BF                               ..        

;; I2CMasterIntDisable: 0000A1B8
I2CMasterIntDisable proc
2300           	mov	r3,#0
6103           	str	r3,[r0,#&10]
4770           	bx	lr
0000A1BE                                           00 BF               ..

;; I2CSlaveIntDisable: 0000A1C0
I2CSlaveIntDisable proc
2300           	mov	r3,#0
60C3           	str	r3,[r0,#&C]
4770           	bx	lr
0000A1C6                   00 BF                               ..        

;; I2CMasterIntStatus: 0000A1C8
;;   Called from:
;;     000096D4 (in OSRAMWriteArray)
;;     0000970C (in OSRAMWriteByte)
;;     00009742 (in OSRAMWriteFinal)
;;     00009768 (in OSRAMWriteFinal)
I2CMasterIntStatus proc
B921           	cbnz	r1,$0000A1D4

l0000A1CA:
6940           	ldr	r0,[r0,#&14]
3000           	adds	r0,#0
BF18           	it	ne
2001           	movne	r0,#1

l0000A1D2:
4770           	bx	lr

l0000A1D4:
6980           	ldr	r0,[r0,#&18]
3000           	adds	r0,#0
BF18           	it	ne
2001           	movne	r0,#1

l0000A1DC:
4770           	bx	lr
0000A1DE                                           00 BF               ..

;; I2CSlaveIntStatus: 0000A1E0
I2CSlaveIntStatus proc
B921           	cbnz	r1,$0000A1EC

l0000A1E2:
6900           	ldr	r0,[r0,#&10]
3000           	adds	r0,#0
BF18           	it	ne
2001           	movne	r0,#1

l0000A1EA:
4770           	bx	lr

l0000A1EC:
6940           	ldr	r0,[r0,#&14]
3000           	adds	r0,#0
BF18           	it	ne
2001           	movne	r0,#1

l0000A1F4:
4770           	bx	lr
0000A1F6                   00 BF                               ..        

;; I2CMasterIntClear: 0000A1F8
I2CMasterIntClear proc
2301           	mov	r3,#1
61C3           	str	r3,[r0,#&1C]
6183           	str	r3,[r0,#&18]
4770           	bx	lr

;; I2CSlaveIntClear: 0000A200
I2CSlaveIntClear proc
2301           	mov	r3,#1
6183           	str	r3,[r0,#&18]
4770           	bx	lr
0000A206                   00 BF                               ..        

;; I2CMasterSlaveAddrSet: 0000A208
;;   Called from:
;;     000096A8 (in OSRAMWriteFirst)
I2CMasterSlaveAddrSet proc
EA42 0241     	orr	r2,r2,r1,lsl #1
6002           	str	r2,[r0]
4770           	bx	lr

;; I2CMasterBusy: 0000A210
I2CMasterBusy proc
6840           	ldr	r0,[r0,#&4]
F000 0001     	and	r0,r0,#1
4770           	bx	lr

;; I2CMasterBusBusy: 0000A218
I2CMasterBusBusy proc
6840           	ldr	r0,[r0,#&4]
F3C0 1080     	ubfx	r0,r0,#6,#1
4770           	bx	lr

;; I2CMasterControl: 0000A220
;;   Called from:
;;     000096BC (in OSRAMWriteFirst)
;;     000096F0 (in OSRAMWriteArray)
;;     0000972C (in OSRAMWriteByte)
;;     00009760 (in OSRAMWriteFinal)
I2CMasterControl proc
6041           	str	r1,[r0,#&4]
4770           	bx	lr

;; I2CMasterErr: 0000A224
I2CMasterErr proc
6843           	ldr	r3,[r0,#&4]
07DA           	lsls	r2,r3,#&1F
D405           	bmi	$0000A236

l0000A22A:
F013 0002     	ands	r0,r3,#2
D003           	beq	$0000A238

l0000A230:
F003 001C     	and	r0,r3,#&1C
4770           	bx	lr

l0000A236:
2000           	mov	r0,#0

l0000A238:
4770           	bx	lr
0000A23A                               00 BF                       ..    

;; I2CMasterDataPut: 0000A23C
;;   Called from:
;;     000096B0 (in OSRAMWriteFirst)
;;     000096E8 (in OSRAMWriteArray)
;;     00009720 (in OSRAMWriteByte)
;;     00009758 (in OSRAMWriteFinal)
I2CMasterDataPut proc
6081           	str	r1,[r0,#&8]
4770           	bx	lr

;; I2CMasterDataGet: 0000A240
I2CMasterDataGet proc
6880           	ldr	r0,[r0,#&8]
4770           	bx	lr

;; I2CSlaveStatus: 0000A244
I2CSlaveStatus proc
6840           	ldr	r0,[r0,#&4]
4770           	bx	lr

;; I2CSlaveDataPut: 0000A248
I2CSlaveDataPut proc
6081           	str	r1,[r0,#&8]
4770           	bx	lr

;; I2CSlaveDataGet: 0000A24C
I2CSlaveDataGet proc
6880           	ldr	r0,[r0,#&8]
4770           	bx	lr
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
