;;; Segment  (00000000)
00000000 2C 02 00 20 09 80 00 00 01 80 00 00 05 80 00 00 ,.. ............
00000010 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000020 00 00 00 00 00 00 00 00 00 00 00 00 15 17 00 00 ................
00000030 00 00 00 00 00 00 00 00 89 16 00 00 E5 16 00 00 ................
00000040 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000050 00 00 00 00 09 81 00 00 70                      ........p      

;; prvUnlockQueue: 00000059
prvUnlockQueue proc
	stmdaeq	r6,r0,r2,r4
	ldrbls	r8,[r10,#&CF0]!
	strbvs	r4,[r0],#&5F8

;; prvCopyDataToQueue: 000000ED
prvCopyDataToQueue proc
	strheq	r0,[r6],#&-45
	stmdahs	fp,r2,r3,r5
	strhteq	r2,[r8],#&69
	bicseq	r3,r0,lr,lsr #2
	rsbvc	r10,r3,r5,lsr r5
	subeq	r1,r6,#&BD00000
	adcsge	r7,r9,r6,asr #&1C
	blpl	$FFC02AB1
	strdhs	r10,fp,[r8,-#&3A]!
	bleq	$01A18AC9
	movtge	r9,#&2344
	bicshs	r1,r3,#&180000
	eorslo	r0,r5,r8,ror #2
	strbge	r10,[r0,-#&346]!
	adcs	r7,sp,r3,rrx
	blmi	$FFC02AD1

;; prvCopyDataFromQueue: 0000016D
prvCopyDataFromQueue proc
	bleq	$FEC5AB25

l00000171:

;; xQueueGenericSend: 00000191
xQueueGenericSend proc
	subeq	pc,r7,r9,ror #1

l00000195:
	ldrteq	r8,[r0],#&425

;; xQueuePeekFromISR: 000002A5
xQueuePeekFromISR proc
	ldrhne	lr,[r3,#&F5]!
	svclt	#&F04F85

;; xQueueGenericReceive: 000002D9
xQueueGenericReceive proc
	subeq	pc,r7,r9,ror #1

l000002DD:
	ldrteq	r8,[r0],#&425

;; uxQueueMessagesWaiting: 00000429
uxQueueMessagesWaiting proc
	stmdaeq	r6,r0,r2,r4
	ldrbtge	r10,[r8],#&4F0
	ldcllt	p8,c0,[r0,#&1AC]!
	strdne	r2,r3,[r6],#-8
	ldmlo	pc!,{r0,r2-r5,r7}

;; uxQueueSpacesAvailable: 0000043D
uxQueueSpacesAvailable proc
	stmdaeq	r6,r0,r2,r4
	ldmge	r8!,{r4-r7,r9,fp-ip,pc}
	strbths	lr,[fp],#&C6B
	mvnslt	r0,r10,lsl r8
	stmdalo	r6,r3,r4,r5
	ldrheq	r0,[pc,sp]!                                         ; 00000459

;; vQueueDelete: 00000455
vQueueDelete proc
	rfeia	#1

;; xQueueGenericSendFromISR: 00000459
xQueueGenericSendFromISR proc
	ldrhne	lr,[r3,#&F5]!
	svclt	#&F04F86

;; xQueueGiveFromISR: 000004C5
xQueueGiveFromISR proc
	ldrhne	lr,[r3,#&F5]!
	svclt	#&F04F84

;; xQueueReceiveFromISR: 00000525
xQueueReceiveFromISR proc
	svc	#&41F0E9
	svcmi	#&8611F3
	strhi	fp,[r4],#&FF0
	svclt	#&8811F3
	svclt	#&8F6FF3
	strhi	r4,[pc],#&FF3                                        ; 00000541
	adcshs	r2,r9,fp,ror #8

l00000541:

;; xQueueIsQueueEmptyFromISR: 00000595
xQueueIsQueueEmptyFromISR proc
	rscshi	fp,r10,fp,rrx
	strdvc	r4,r5,[r9],-r0

;; xQueueIsQueueFullFromISR: 000005A1
xQueueIsQueueFullFromISR proc
	rsbgt	ip,fp,fp,rrx

l000005A5:
	rscshi	fp,r10,r10,lsl r0
	strdvc	r4,r5,[r9],-r0
	adcshi	r0,pc,r7,asr #&20

;; uxQueueMessagesWaitingFromISR: 000005B1
uxQueueMessagesWaitingFromISR proc
	subne	r7,r7,fp,rrx

;; xQueueGetMutexHolder: 000005B5
xQueueGetMutexHolder proc

;; xQueueTakeMutexRecursive: 000005D5
xQueueTakeMutexRecursive proc
	strbteq	r4,[r8],#&5B5
	subeq	r0,r6,r6,asr #&1C
	ldrbhi	r10,[sp,#&CF0]!
	sbcseq	r0,r0,r2,asr #&14
	stmdbne	r6,{r0-r1,r5,r9,ip-sp}

;; xQueueGiveMutexRecursive: 00000605
xQueueGiveMutexRecursive proc
	strbteq	r4,[r8],#&5B5
	ldrbls	r0,[r0,#&46]!
	strdeq	r8,r9,[r2,-#&5D]
	stmdalo	r0,r4,r6,r7
	strheq	lr,[r8,-#&3D]!
	bleq	$0183930D
	stmdalo	r0,r0,r4,r5
	bne	$0118891D

;; xQueueGenericReset: 00000631
xQueueGenericReset proc
	mcreq	p4,#2,r0,c6
	streq	pc,[r5,-r6,asr #&1E]!
	ldrshteq	r9,[pc],#&E0                                      ; 00000641
	rsb	r2,ip,#&84000000
	rsbeq	r2,r8,#&6B

;; xQueueGenericCreate: 000006AD
xQueueGenericCreate proc
	strheq	r0,[r6],#&FFFFFF9B
	ldmmi	r0!,{r0-r1,r3-r8}

l000006B5:

;; xQueueCreateMutex: 000006DD
xQueueCreateMutex proc
	strheq	r0,[r6],#&-25

;; prvInitialiseNewTask: 00000701
prvInitialiseNewTask proc
	mcrreq	p8,#&E,pc,pc
	strbhs	r9,[r6,-#&99C]
	rscshi	r0,r1,sp,ror #4
	teqls	fp,#&C0000010
	ldreq	r0,[r10,#&A46]
	andhi	r8,r5,fp,ror #7
	svcmi	#&1E4B46
	ldrbhs	sp,[r10,-#&2EA]!
	andeq	r0,r5,#&3C00000
	ldrbtpl	r0,[r1],#&431
	rscseq	r2,r0,r0,lsl #4
	rsbseq	r5,r8,r2,asr #&1C

;; prvAddNewTaskToReadyList: 00000799
prvAddNewTaskToReadyList proc
	stclhs	p0,c15,[r1,-#&3A4]
	strbeq	r0,[r6,-ip,asr #&A]

;; prvAddCurrentTaskToDelayedList.isra.0: 0000085D
prvAddCurrentTaskToDelayedList.isra.0 proc
	strbeq	r1,[ip,-#&4B5]
	rscshi	sp,r8,r6,asr #8
	strbths	r6,[r8],#&60
	ldmibvs	r0,r4,r5,r8
	ldrsheq	r3,[r9,sp]!

;; xTaskCreate: 000008B5
xTaskCreate proc
	subhi	pc,r7,r9,ror #1

l000008B9:
	adcsls	r8,r0,r6,asr #8

l000008BD:
	stmdbhi	r6,{r9-r10,ip}
	subeq	r9,r6,r6,asr #&14

l000008C5:
	ldrsht	r3,[pc],#&20                                        ; 000008CD
	stmdavs	r6,r0,r4,r5
	ldclhs	p0,c0,[r0,#&80]!
	stmdale	r6,r0,r1,r2
	streq	r0,[r7,-#&B1]!
	ldrhi	r0,[sp],#&D65
	ldrsheq	r6,[r0,-#&58]!
	orrspl	r0,sp,#&9500
	stmdbmi	r6,{r1-r2,r6,r9,ip-sp}
	movteq	r4,#&6046
	umullseq	r0,r4,r7,r2
	ldrbeq	pc,[r7,#&F95]!

;; xTaskCreateRestricted: 0000091D
xTaskCreateRestricted proc

;; vTaskAllocateMPURegions: 00000971
vTaskAllocateMPURegions proc
	strteq	r0,[r3],#&B1
	subeq	r1,r6,r0,lsr r10

;; vTaskStartScheduler: 00000991
vTaskStartScheduler proc
	strdne	r0,r1,[r3],#0
	subhi	r1,ip,#&5000000B

l00000999:
	ldreq	r0,[r3],#&B0
	strdeq	r8,r9,[r3,-r1]
	mlaeq	r2,r3,fp,r3

l000009A5:
	svceq	#&490F23

;; vTaskEndScheduler: 000009ED
vTaskEndScheduler proc
	movwhi	fp,#&3FF0

l000009F1:
	svclt	#&8811F3
	svclt	#&8F6FF3
	strdeq	r4,r5,[pc],r3                                       ; 00000A01
	bpl	$012C128D

l00000A01:
	ldrbge	r0,[r0,#&67]!
	ldrtgt	r0,[pc],#&BD                                        ; 00000A0D

;; vTaskSuspendAll: 00000A0D
vTaskSuspendAll proc
	ldclhi	p2,c13,[r8]!
	eorsgt	r0,r3,#&C
	ldrshtvc	r8,[r0],r8
	ldrtgt	r0,[pc],#&47                                        ; 00000A21

;; xTaskGetTickCount: 00000A21
xTaskGetTickCount proc
	rscshi	sp,r8,fp,asr #6
	strbgt	r7,[r7],#0

;; xTaskGetTickCountFromISR: 00000A2D
xTaskGetTickCountFromISR proc
	rscshi	sp,r8,fp,asr #6
	strbgt	r7,[r7],#0

;; uxTaskGetNumberOfTasks: 00000A39
uxTaskGetNumberOfTasks proc
	rsbvc	r1,r8,fp,asr #&10

l00000A3D:
	ldrtgt	r0,[pc],#&47                                        ; 00000A45

;; pcTaskGetName: 00000A45
pcTaskGetName proc
	ldrhtvc	r5,[r0],r1
	stmdapl	fp,r0,r1,r2
	eorsvc	r5,r0,r8,ror #8
	ldrtgt	r0,[pc],#&47                                        ; 00000A59

;; xTaskGenericNotify: 00000A59
xTaskGenericNotify proc

;; xTaskGenericNotifyFromISR: 00000B0D
xTaskGenericNotifyFromISR proc
	svc	#&41F0E9
	svcmi	#&8511F3
	strhi	fp,[r4],#&FF0
	svclt	#&8811F3
	svclt	#&8F6FF3
	bleq	$FE3D4AF5

l00000B25:
	stclne	p4,c0,[lr]!
	eorls	r0,r3,r0,ror #4
	strdeq	r6,r7,[r0,-#&48]
	ldrbtvs	r8,[r8],#&3A

;; xTaskNotifyWait: 00000BD5
xTaskNotifyWait proc
	svcne	#&41F0E9
	subhi	r1,r6,ip,asr #&A

l00000BDD:
	svcne	#&460E46
	ldmibgt	r0,r1,r2,r6

l00000BE5:
	rsbls	r6,r8,#&C000000F

l00000BE9:
	eoreq	r6,r0,#&F8000000

l00000BED:
	bicseq	r0,r0,r10,lsr #&12

l00000BF1:
	beq	$01A19079

l00000BF5:
	stmeq	r10!,{r1-r3,r5-r6,r9,sp}

;; vTaskNotifyGiveFromISR: 00000C61
vTaskNotifyGiveFromISR proc
	svc	#&43F8E9
	svcmi	#&8611F3
	movwhi	fp,#&3FF0

l00000C6D:
	svclt	#&8811F3
	svclt	#&8F6FF3
	addeq	r4,pc,#&3CC

l00000C79:
	ldrbtvs	r9,[r8],#&23
	ldrbtvs	r8,[r8],#&50
	stcl	p3,c0,[lr,-#&C0]!
	ldrheq	r0,[r3,-r2]!

;; ulTaskNotifyTake: 00000D01
ulTaskNotifyTake proc

;; xTaskIncrementTick: 00000D6D
xTaskIncrementTick proc
	mcrrlo	p0,#&E,pc,r7
	ldclhi	p4,c13,[r8]!
	mcrpl	p0,#1,r0,c11
	ldrsbthi	sp,[r8],#&41
	ldrtgt	r0,[r7],#&170

;; xTaskResumeAll: 00000E6D
xTaskResumeAll proc
	movtlo	pc,#&10E9

l00000E71:
	mvnshi	r0,ip,asr #&E

l00000E75:
	ldclhi	p4,c13,[r8]!
	ldrtgt	r0,[fp],#&130
	ldrtle	r8,[r0],#&CF8
	ldrsheq	r8,[r0],#&FFFFFF38
	bicshs	r4,r1,#&2D0
	blmi	$00AC1031
	strteq	r0,[r6],#&1D0
	mcrne	p8,#0,r0,c7
	stclle	p3,c6,[lr,-#&380]!
	ldrbths	r0,[r1],#&568
	ldmlo	r1!,{r3,r8,r10}
	ldclmi	p7,c0,[r0]!

;; vTaskDelay: 00000F49
vTaskDelay proc
	svcmi	#&B940B5
	ldmdbeq	r2,r4,r5,r6

l00000F51:
	svclt	#&601A4B
	svclt	#&8F4FF3
	stmeq	pc,{r0-r1,r4-fp,sp-lr}
	suble	r0,r10,#&2F40000

l00000F61:
	ldrsheq	r8,[r0,-r8]!
	ldclhi	p2,c12,[r8]!

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
	vmlseq.f32	s19,s4,s8

;; vTaskPlaceOnEventList: 00000FDD
vTaskPlaceOnEventList proc
	strbeq	r0,[r6],#&CB5
	stmdalo	r8,r0,r1,r3
	mvnsls	r0,r1,lsr r7
	stcllt	p0,c2,[r6,-#&3E4]
	strblo	r1,[r0],#&E8
	ldrtgt	r0,[pc],#&E4                                        ; 00000FF9

;; vTaskPlaceOnUnorderedEventList: 00000FF9
vTaskPlaceOnUnorderedEventList proc

;; xTaskRemoveFromEventList: 0000101D
xTaskRemoveFromEventList proc

;; xTaskRemoveFromUnorderedEventList: 00001081
xTaskRemoveFromUnorderedEventList proc

;; vTaskSwitchContext: 000010D9
vTaskSwitchContext proc
	ldclhi	p2,c13,[r8]!
	adcsgt	ip,r9,#&C0000000
	teqle	r0,#&F8

;; uxTaskResetEventItemValue: 00001121
uxTaskResetEventItemValue proc
	bpl	$01A17655

l00001125:
	stmdahi	r8,r3,r5,r6

;; xTaskGetCurrentTaskHandle: 00001139
xTaskGetCurrentTaskHandle proc
	rsbvc	r5,r8,fp,asr #&10

l0000113D:
	ldrtgt	r0,[pc],#&47                                        ; 00001145

;; vTaskSetTimeOutState: 00001145
vTaskSetTimeOutState proc
	ldrbtls	sp,[r8],#&34B
	rscshi	sp,r8,r0,lsr #6
	stcleq	p0,c8,[r8]!
	strbgt	r7,[r7],#0
	eorvc	r0,r0,r0

;; xTaskCheckForTimeOut: 00001159
xTaskCheckForTimeOut proc
	mcreq	p4,#2,r0,c6
	bleq	$FFC02E7D

l00001161:
	strdhs	r1,r2,[fp,-#&1A]
	rscshi	sp,r8,r8,ror #6
	ldrbtls	sp,[r8],#&350

;; vTaskMissedYield: 000011AD
vTaskMissedYield proc
	movtgt	r0,#&B222

l000011B1:

;; vTaskPriorityInherit: 000011BD
vTaskPriorityInherit proc

;; xTaskPriorityDisinherit: 00001251
xTaskPriorityDisinherit proc

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
	ldrtgt	r0,[pc],#&47                                        ; 000012ED
	eoreq	r0,r0,r0
	svcle	#0

;; prvRestoreContextOfFirstTask: 000012F1
prvRestoreContextOfFirstTask proc
	strdeq	r3,r4,[r4],-r8
	rsbhi	r0,r8,r8,rrx

l000012F9:
	stceq	p8,c0,[r8],#&F3
	stmdaeq	r8,r0,r1,r3
	ldrbteq	r0,[r1],#&168
	rscshs	sp,r8,r1,lsl #&1E

;; prvSVCHandler: 00001335
prvSVCHandler proc
	rscseq	r1,r8,#&A4000001
	eorne	r0,fp,ip,lsr r1
	sbcseq	r0,r3,#&340000
	svc	#&D1062B
	strdhs	r1,r2,[r1,r3]
	strdhi	r0,r1,[r1,-r0]
	strdvc	r1,r2,[r8],r3
	strbeq	r7,[r7,-r7,asr #&20]

;; pxPortInitialiseStack: 0000137D
pxPortInitialiseStack proc
	ldmeq	r4!,{r0-r1,r3,r5,ip-sp}

l00001381:
	svcmi	#&2302BF
	svcmi	#&7580F0
	stmdane	r4,r4,r5,r6

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

;; vPortEndScheduler: 00001551
vPortEndScheduler proc
	adcslo	r0,pc,r7,asr #&20

;; vPortStoreTaskMPUSettings: 00001555
vPortStoreTaskMPUSettings proc
	strhmi	r0,[r9,-r4]!
	ldreq	r4,[fp,#&BD0]!
	stc2l	p12,c4,[r8]!
	strbmi	r0,[r8,-#&BB1]!
	movwne	r1,#&20F0

;; xPortPendSVHandler: 00001689
xPortPendSVHandler proc
	strne	r0,[r0],#&9F3
	svc	#&681A4B
	strdhs	r1,r2,[r1],r3
	andne	pc,pc,r9,ror #5

l00001699:
	stmeq	r9!,{r5-r6,r8,r10-fp,sp}
	svclt	#&F04F40

;; xPortSysTickHandler: 000016E5
xPortSysTickHandler proc
	ldrhne	lr,[r3,#&F5]!
	svclt	#&F04F84

;; vPortSVCHandler: 00001715
vPortSVCHandler proc

;; pvPortMalloc: 0000172D
pvPortMalloc proc
	movtmi	r0,#&64B5

l00001731:
	adcshs	r1,pc,r7,lsl #&18

l00001735:
	stmdaeq	r4,r4,r5,r6

;; vPortFree: 00001781
vPortFree proc
	adcseq	r0,pc,r7,asr #&20

;; vPortInitialiseBlocks: 00001785
vPortInitialiseBlocks proc
	movtgt	r0,#&B222

l00001789:

;; xPortGetFreeHeapSize: 00001795
xPortGetFreeHeapSize proc
	rscsgt	sp,r8,fp,asr #6
	ldrbtlt	ip,[r5],r5
	eorsvc	r0,r0,r0,ror #8
	adcslo	r0,pc,r7,asr #&20
	eorne	r0,r0,r2

;; xEventGroupCreate: 000017A9
xEventGroupCreate proc

;; xEventGroupWaitBits: 000017C5
xEventGroupWaitBits proc
	strbeq	pc,[r1],-r9,ror #1
	stcleq	p15,c1,[r6,-#&118]

;; xEventGroupClearBits: 00001875
xEventGroupClearBits proc
	mcrreq	p6,#&B,r0,r6
	ldclvc	p6,c0,[r0,#&118]!
	strbhs	r3,[r8,-#&5FE]!
	strlo	r0,[r4],#&4EA
	mvnsls	r0,#&6000000
	strdvc	r2,r3,[r6],#&FFFFFF72
	rfeia	#1

;; xEventGroupSetBits: 00001891
xEventGroupSetBits proc
	mcrreq	p5,#&B,r0,r6
	ldmiblt	r7,r1,r2,r6
	stmdahs	r8,r3,r4,r5
	ldcleq	p5,c0,[r1]!
	strbhi	r2,[r3],-r6,lsl #2
	rsbhs	r2,r0,#&108000
	stceq	p0,c0,[r7]!
	strbeq	r0,[r2,-r0,ror #&15]
	ldrdeq	sp,lr,[r1],-r0
	ldrdmi	r1,r2,[r3,-#&75]

;; xEventGroupSync: 000018F9
xEventGroupSync proc
	stmdahi	r1,r0,r3,r5
	strbne	r0,[r6],-r6,asr #&A

;; xEventGroupGetBitsFromISR: 00001989
xEventGroupGetBitsFromISR proc
	svcmi	#&8311F3
	andhi	fp,r2,#&3C0

l00001991:
	svclt	#&8811F3
	svclt	#&8F6FF3
	orrhi	r4,pc,#&3CC

l0000199D:
	strdeq	r1,r2,[r8],r3
	subne	r7,r7,r8,rrx

;; vEventGroupDelete: 000019A5
vEventGroupDelete proc

;; vEventGroupSetBitsCallback: 000019D1
vEventGroupSetBitsCallback proc
	ldmlo	pc!,{r0-r2,r4-r7,r9-ip,lr}

;; vEventGroupClearBitsCallback: 000019D5
vEventGroupClearBitsCallback proc
	stcleq	p4,c0,[r6,-#&2D4]
	ldclgt	p6,c0,[r0,#&118]!
;;; Segment .text (00008000)
00008000 FE                                              .              

;; NmiSR: 00008001
NmiSR proc
	cdp2	p0,#&B,c0,c15

;; FaultISR: 00008005
FaultISR proc
	ldmeq	pc!,{r0-r2,r5-r7}

;; ResetISR: 00008009
ResetISR proc
	movthi	r0,#&894B

l0000800D:
	ble	$FF48A91D

l00008011:
	eoreq	r0,r1,#&43

l00008015:
	mvnseq	r2,#&40000004

l00008019:
	bne	$00C89029

l0000801D:
	ldrbteq	r4,[r8],#&344
	blx	$010ACC97
	bllo	$FFC08371
	ldrhtvs	r0,[pc],r8                                         ; 00008031
	eorhi	r0,r0,r1
	cdp2	p0,#2,c0,c0

;; raise: 00008035
raise proc
	adcslo	r0,pc,r7,ror #1

;; vPrintTask: 00008039
vPrintTask proc
	stmdbeq	r4!,{r0,r2,r4-r5,r7}
	asrseq	r8,sp,asr #6

l00008041:
	eorseq	r0,r4,r9,lsr #3

l00008045:

;; vCheckTask: 00008069
vCheckTask proc
	movthi	r0,#&BBB5

l0000806D:
	ldrheq	r0,[r3],r0
	rscseq	r4,ip,#&F00000
	strbmi	r0,[sp],#&9AC
	strdhs	r0,r1,[sp],-r8
	ldmhi	r2!,{r1-r2,r6,r8,lr}

;; Main: 000080A1
Main proc

;; vUART_ISR: 00008109
vUART_ISR proc
	stmdbne	r6!,{r0,r2,r4-r5,r7}
	asrseq	r8,sp,asr #4

l00008111:

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

;; vApplicationIdleHook: 00008211
vApplicationIdleHook proc
	blhi	$FFC084ED

l00008215:
	ldmiblt	r7,r1,r2,r3

l00008219:
	strbhi	pc,[r7,#&AFF]!
	andlo	r0,r0,r1,lsl #1

;; PDCInit: 00008221
PDCInit proc
	movthi	r1,#&8AB5

l00008225:
	ldmibge	r0,r4,r5,r7

l00008229:
	strdeq	r1,r2,[r8,-#&9C]
	rscseq	r10,ip,#&F000000
	svcmi	#&213422

;; PDCWrite: 0000829D
PDCWrite proc
	beq	$0118B979

l000082A1:
	adcseq	r8,r0,ip,asr #6

l000082A5:
	strdhs	r0,r1,[r1],-r0

;; vListInitialise: 000082D1
vListInitialise proc
	ldrshteq	pc,[r1],r0
	ldmeq	r1!,{r1,r5}
	rsbhi	r8,r0,r3,lsl #2
	movwgt	r0,#&CE8
	rsbvc	r0,r1,r0,ror #6
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
	stclls	p12,c8,[r0]!

;; vListInsert: 0000830D
vListInsert proc
	blvs	$01A0B9E5

l00008311:
	sbcseq	r1,r0,ip,lsl r1

l00008315:
	strdeq	r0,r1,[r2],-r1
	movtpl	r1,#&6AE0

l0000831D:
	strbge	r1,[r8,-#&C68]!
	ldrbeq	pc,[r2],#&A42

l00008325:

;; uxListRemove: 00008341
uxListRemove proc

;; xQueueCRSend: 00008365
xQueueCRSend proc
	mcreq	p5,#2,r0,c6
	svcmi	#&461446
	movwhi	fp,#&3FF0

l00008371:
	svclt	#&8811F3
	svclt	#&8F6FF3
	strdeq	r4,r5,[pc],r3                                       ; 00008381
	bge	$FFE47745

l00008381:
	bls	$01B03135

l00008385:
	sbcseq	r1,r0,r2,asr #8

l00008389:
	ldrshteq	r1,[r9],#&20

;; xQueueCRReceive: 00008401
xQueueCRReceive proc
	svcmi	#&4604B5
	movwhi	fp,#&3FF0

l00008409:
	svclt	#&8811F3
	svclt	#&8F6FF3
	strhi	r4,[pc,#&FF3]                                        ; 0000940C
	adcseq	r2,r9,fp,ror #&1A

l00008419:
	sbcshi	r3,r1,#&2A00000

l0000841D:
	strdne	r1,r2,[r8],r3
	adcseq	r3,sp,r6,asr #&10

l00008425:

;; xQueueCRSendFromISR: 000084A1
xQueueCRSendFromISR proc

;; xQueueCRReceiveFromISR: 000084D5
xQueueCRReceiveFromISR proc

;; prvIdleTask: 0000852D
prvIdleTask proc
	svcvs	#&F7FFB5
	stmlo	r7!,{r1-r7,r10-pc}

;; xTaskNotifyStateClear: 00008535
xTaskNotifyStateClear proc
	ldrteq	r7,[r1],#&8B5
	ldclne	p0,c0,[r0,#&118]!
	ldrbtvs	r9,[r8],#&4F8
	streq	r0,[fp,-#&230]!
	strheq	r0,[r3,-pc]!
	strthi	r0,[r5],#&25
	ldrshteq	r6,[r0],r8
	ldmhs	r8!,{r4-r7,r9-fp,sp}

;; xPortRaisePrivilege: 00008565
xPortRaisePrivilege proc
	strdne	r1,r2,[r0],r3
	bne	$003C8D31

l0000856D:
	eoreq	r0,r0,#&BF

l00008571:

;; vPortEnterCritical: 00008579
vPortEnterCritical proc

;; vPortExitCritical: 000085B1
vPortExitCritical proc

;; vParTestInitialise: 000085DD
vParTestInitialise proc
	svcne	#&F7FFB5
	strbeq	r0,[fp,-#&3FE]
	ldcllt	p9,c1,[r8,-#&80]!

;; vParTestSetLED: 000085F5
vParTestSetLED proc
	stcleq	p4,c0,[r6,-#&2D4]
	mvnsvs	r0,r6,asr #&20
	stceq	p7,c0,[ip]!

;; vParTestToggleLED: 00008631
vParTestToggleLED proc
	strheq	r0,[r6],#&-45

;; prvFlashCoRoutine: 00008671
prvFlashCoRoutine proc
	addhi	r8,lr,#&D4000002

l00008675:
	ldrh	fp,[r5,#&30]!
	blne	$0118987D

l0000867D:

;; prvFixedDelayCoRoutine: 000086E9
prvFixedDelayCoRoutine proc
	addhi	r8,lr,#&D4000002

l000086ED:
	ldrhgt	fp,[r5,#&30]!

;; vStartFlashCoRoutines: 00008785
vStartFlashCoRoutines proc
	ldmeq	pc!,{r3,r5,fp,sp}

l00008789:
	adcseq	r7,r5,r0,lsr #&20

l0000878D:
	strbeq	r0,[r6],#&522
	eoreq	r0,r0,r1,lsr #2

l00008795:
	beq	$FFE66B5D

l00008799:
	stmdahi	r0,r0,r1,r3
	ldrhteq	r4,[r1],r1
	subhs	r0,lr,#&90000

;; xAreFlashCoRoutinesStillRunning: 000087D1
xAreFlashCoRoutinesStillRunning proc
	rsbvc	r1,r8,fp,asr #&10

l000087D5:
	adcsgt	r0,pc,r7,asr #&20

l000087D9:
	eorvc	r0,r0,r0

;; MPU_xTaskCreateRestricted: 000087DD
MPU_xTaskCreateRestricted proc
	mcreq	p5,#2,r0,c6
	svclt	#&F7FF46
	strbeq	r3,[r6],#&1FE

;; MPU_xTaskCreate: 00008809
MPU_xTaskCreate proc
	subhi	pc,r7,#&E9

l0000880D:
	stmdahi	r6,r4,r5,r7
	bls	$011ACD31
	bleq	$FE7CB135

;; MPU_vTaskAllocateMPURegions: 0000884D
MPU_vTaskAllocateMPURegions proc
	mcreq	p5,#2,r0,c6
	ldrbhi	pc,[r7,r6,asr #&1E]!

l00008855:
	strdlo	r0,r1,[r6,-#&4E]

;; MPU_vTaskDelayUntil: 00008875
MPU_vTaskDelayUntil proc
	mcreq	p5,#2,r0,c6

;; MPU_vTaskDelay: 0000889D
MPU_vTaskDelay proc

;; MPU_vTaskSuspendAll: 000088C1
MPU_vTaskSuspendAll proc
	svcmi	#&F7FFB5

;; MPU_xTaskResumeAll: 000088E1
MPU_xTaskResumeAll proc
	svclo	#&F7FFB5

;; MPU_xTaskGetTickCount: 00008905
MPU_xTaskGetTickCount proc
	ldclhs	p15,c15,[r7,#&2D4]!

;; MPU_uxTaskGetNumberOfTasks: 00008929
MPU_uxTaskGetNumberOfTasks proc
	blne	$FFE08805

l0000892D:

;; MPU_pcTaskGetName: 0000894D
MPU_pcTaskGetName proc

;; fn0000895D: 0000895D
fn0000895D proc
	strbeq	r0,[r6,-#&32C]
	ldrbtne	lr,[r3],#&FD0
	mvnseq	r4,r0,lsl #1
	ldrbtne	r8,[r3],#0
	stmdalo	r6,r3,r7,fp
	ldmlo	pc!,{r0,r2-r5,r7}

;; MPU_vTaskSetTimeOutState: 00008975
MPU_vTaskSetTimeOutState proc

;; MPU_xTaskCheckForTimeOut: 00008999
MPU_xTaskCheckForTimeOut proc
	mcreq	p5,#2,r0,c6

;; MPU_xTaskGenericNotify: 000089C5
MPU_xTaskGenericNotify proc
	strbeq	pc,[r1,-#&E9]
	strbne	r0,[r6,-r6,asr #&1C]

;; MPU_xTaskNotifyWait: 000089FD
MPU_xTaskNotifyWait proc
	strbeq	pc,[r1,-#&E9]
	strbne	r0,[r6,-r6,asr #&1C]

;; MPU_ulTaskNotifyTake: 00008A35
MPU_ulTaskNotifyTake proc
	mcreq	p5,#2,r0,c6

;; MPU_xTaskNotifyStateClear: 00008A61
MPU_xTaskNotifyStateClear proc

;; MPU_xQueueGenericCreate: 00008A89
MPU_xQueueGenericCreate proc
	mcreq	p5,#2,r0,c6

;; MPU_xQueueGenericReset: 00008AB9
MPU_xQueueGenericReset proc
	mcreq	p5,#2,r0,c6

;; MPU_xQueueGenericSend: 00008AE5
MPU_xQueueGenericSend proc
	strbeq	pc,[r1,-#&E9]
	strbne	r0,[r6,-r6,asr #&1C]

;; MPU_uxQueueMessagesWaiting: 00008B1D
MPU_uxQueueMessagesWaiting proc

;; MPU_uxQueueSpacesAvailable: 00008B45
MPU_uxQueueSpacesAvailable proc

;; MPU_xQueueGenericReceive: 00008B6D
MPU_xQueueGenericReceive proc
	strbeq	pc,[r1,-#&E9]
	strbne	r0,[r6,-r6,asr #&1C]

;; MPU_xQueuePeekFromISR: 00008BA5
MPU_xQueuePeekFromISR proc
	mcreq	p5,#2,r0,c6
	blle	$FFE088C9

l00008BAD:
	strbeq	r3,[r6],#&1FC

;; MPU_xQueueGetMutexHolder: 00008BD1
MPU_xQueueGetMutexHolder proc

;; MPU_xQueueCreateMutex: 00008BF9
MPU_xQueueCreateMutex proc

;; MPU_xQueueTakeMutexRecursive: 00008C21
MPU_xQueueTakeMutexRecursive proc
	mcreq	p5,#2,r0,c6
	ldclls	p15,c15,[r7,#&118]!
	strbeq	r3,[r6],#&1FC

;; MPU_xQueueGiveMutexRecursive: 00008C4D
MPU_xQueueGiveMutexRecursive proc

;; MPU_vQueueDelete: 00008C75
MPU_vQueueDelete proc

;; MPU_pvPortMalloc: 00008C99
MPU_pvPortMalloc proc

;; MPU_vPortFree: 00008CC1
MPU_vPortFree proc

;; MPU_vPortInitialiseBlocks: 00008CE5
MPU_vPortInitialiseBlocks proc
	ldcllo	p15,c15,[r7,#&2D4]!

;; MPU_xPortGetFreeHeapSize: 00008D05
MPU_xPortGetFreeHeapSize proc
	ldclhs	p15,c15,[r7,#&2D4]!

;; MPU_xEventGroupCreate: 00008D29
MPU_xEventGroupCreate proc
	blne	$FFE08C05

l00008D2D:

;; MPU_xEventGroupWaitBits: 00008D4D
MPU_xEventGroupWaitBits proc
	movthi	pc,#&30E9

l00008D51:
	mcreq	p5,#2,r0,c6
	stmdbls	r6,{r1-r2,r6,ip,pc}

;; MPU_xEventGroupClearBits: 00008D8D
MPU_xEventGroupClearBits proc
	mcreq	p5,#2,r0,c6
	ldrb	pc,[r7,r6,asr #&1E]!
00008D95                FB 31 46 04 46 28 46 F8 F7 6A FD      .1F.F(F..j.
00008DA0 01 2C 03 46 05 D0 EF F3 14 80 40 F0 01 00 80 F3 .,.F......@.....
00008DB0 14 88 18 46 70 BD 00 BF 70                      ...Fp...p      

;; MPU_xEventGroupSetBits: 00008DB9
MPU_xEventGroupSetBits proc
	mcreq	p5,#2,r0,c6

;; MPU_xEventGroupSync: 00008DE5
MPU_xEventGroupSync proc
	strbeq	pc,[r1,-#&E9]
	strbne	r0,[r6,-r6,asr #&1C]

;; MPU_vEventGroupDelete: 00008E1D
MPU_vEventGroupDelete proc

;; xCoRoutineCreate: 00008E41
xCoRoutineCreate proc
	smlaltthi	pc,pc,r9,r8
	stceq	p8,c3,[r0,-#&118]!

;; vCoRoutineAddToDelayedList: 00008EF1
vCoRoutineAddToDelayedList proc
	mcrreq	p14,#&B,r0,r6
	strbvs	r2,[r8,-#&34C]!
	stmdane	r4,r0,r1,r2
	svcne	#&F7FF1D
	strdhs	r6,r7,[pc,-#&3A]!                                   ; 00008ECF
	stclmi	p13,c9,[r2,-#&1A0]
	adcs	r3,pc,r0,ror #8
	strbteq	r10,[lr],#&6E
	blx	$FFE08BDF
	ldrshhs	r3,[r1,r9]!
	stcllt	p0,c3,[r6,-#&1A0]
	stmdane	r0,r3,r5,r6
	vcvt.u32.f32	d31,d17,#9
	ldc2	p0,c7,[sp]!
	stchs	p0,c0,[r0,-#&1C]!

;; vCoRoutineSchedule: 00008F2D
vCoRoutineSchedule proc
	strbpl	pc,[r1,-#&E9]
	blhs	$01B63C6D

l00008F35:
	streq	r0,[r7,-#&B3]!
	svcmi	#&804F1
	movwhi	fp,#&3FF0

l00008F41:
	svclt	#&8811F3
	svclt	#&8F6FF3
	blhs	$FE3DCF1D

l00008F4D:
	strbteq	sp,[r8],#&C6E

;; xCoRoutineRemoveFromEventList: 00009095
xCoRoutineRemoveFromEventList proc
	ldcle	p0,c7,[r5]!
	strbeq	r0,[sp],#&968
	strdlo	r1,r2,[r6],-r1
	ldclmi	p15,c15,[r7,#&118]!
	ldrbtpl	r0,[r1],#&5F9

;; GPIOGetIntNumber: 000090C5
GPIOGetIntNumber proc
	stmdbne	r2,{r0-r1,r3,r6,fp-ip,pc}
	ldrsblt	r0,[r8],#&80

;; GPIODirModeSet: 0000910D
GPIODirModeSet proc
	eorsne	r0,r4,#&F8

l00009111:
	strne	r0,[pc],#&1F0                                        ; 00009119
	blhi	$010CC019

l00009119:
	rscseq	ip,r8,r3,asr #&20
	rscshs	sp,r8,r4,lsr r0

;; GPIODirModeGet: 00009135
GPIODirModeGet proc

;; GPIOIntTypeSet: 0000915D
GPIOIntTypeSet proc
	eorsne	r0,r4,#&F80000

l00009161:
	strne	r0,[pc],#&1F0                                        ; 00009169
	blhi	$010CC069

l00009169:
	ldmeq	r8!,{r0-r1,r6,lr-pc}

l0000916D:
	ldrbteq	sp,[r8],#&34
	rscseq	r1,r0,#&40000003
	bleq	$FEFCE1B9
	subgt	r8,r3,r3,asr #&16
	ldrshtle	r0,[r4],r8
	eorspl	r0,r4,#&F800
	ldmibne	pc,r0,r1,r2

;; GPIOIntTypeGet: 00009195
GPIOIntTypeGet proc
	ldmeq	r8!,{r0-r1,r5,ip,lr-pc}

l00009199:

;; GPIOPadConfigSet: 000091C9
GPIOPadConfigSet proc
	ldrhteq	sp,[r8],#4
	mvnseq	r1,r5,asr #4
	ldceq	p4,c1,[pc]!                                          ; 000091D9
	subgt	r8,r3,r3,asr #&18
	strdle	r0,r1,[r5],#-8
	subne	r0,r5,#&F8000000
	strne	r0,[pc],#&2F0                                        ; 000091E9
	mcrrhi	p12,#&B,r0,r3
	ldrbteq	ip,[r8],#&43
	ldmeq	r8!,{r0,r2,r6,ip,lr-pc}
	ldrbteq	r1,[r0],#&245
	ldceq	p4,c1,[pc]!                                          ; 000091FD
	subgt	r8,r3,r3,asr #&18
	subne	r0,r5,#&F80000
	strdle	r0,r1,[pc],-r0                                      ; 00009209
	strtne	r1,[r5],#&8F8
	bhi	$010CBD0D
	ldmne	r8!,{r0-r1,r6,lr-pc}
	ldcleq	p0,c13,[r8]!

;; GPIOPadConfigGet: 0000925D
GPIOPadConfigGet proc

;; GPIOPinIntEnable: 000092E1
GPIOPinIntEnable proc
	ldmdbne	r4,r3,r4,r5

l000092E5:
	rscsne	ip,r8,r3,asr #&20
	suble	r7,r7,r4,lsl r0

;; GPIOPinIntDisable: 000092ED
GPIOPinIntDisable proc
	teqhs	r4,#&F8
	andgt	r0,r1,r10,ror #3

l000092F5:
	ldrshvc	r1,[r4],-r8

;; GPIOPinIntStatus: 000092FD
GPIOPinIntStatus proc
	ldrbtne	sp,[r8],#&B9
	suble	r7,r7,r4
	strdvc	r1,r2,[r4],-r8
	adcsgt	r0,pc,r7,asr #&20

;; GPIOPinIntClear: 0000930D
GPIOPinIntClear proc
	ldrshvc	r1,[r4],-r8
	ldrths	r0,[pc],#&47                                        ; 00009319

;; GPIOPortIntRegister: 00009315
GPIOPortIntRegister proc
	ldmls	r5!,{r0-r1,r3,r6,ip}

l00009319:
	svceq	#&D03C42
	ldrsbtmi	fp,[r1],#8
	bicsge	r2,r0,#&BC
	ldmdals	r3,r0,r2,r4
	bicsne	r1,r1,r2,asr #&10
	subeq	r2,r6,r4,lsr #&20
	ldrshths	lr,[r8],#&80
	rscne	fp,r8,r6,asr #&1A
	svcmi	#&F00040
	stmdals	fp,r0,r3,r4
	bicseq	r1,r0,#&4200000
	ldmdals	r3,r0,r2,r4
	ldrbne	r0,[r1],#&842
	subeq	r2,r6,r4,lsr #&20
	ldrshths	sp,[r8],#&80
	rscne	fp,r8,r6,asr #&1A
	svclo	#&F00040

;; GPIOPortIntUnregister: 000093B1
GPIOPortIntUnregister proc
	ldmls	r5!,{r0-r1,r3,r6,ip}

l000093B5:
	svceq	#&D03C42
	ldrsbtmi	fp,[r1],#8
	bicsge	r2,r0,#&BC
	ldmdals	r3,r0,r2,r4
	bicsne	r1,r1,r2,asr #&10
	subeq	r2,r6,r4,lsr #&20
	ldrshths	r3,[r9],#&40
	rscne	fp,r8,r6,asr #&1A
	svcge	#&F00040
	stmdals	fp,r3,r4,r5
	bicseq	r1,r0,#&4200000
	ldmdals	r3,r0,r2,r4
	ldrbne	r0,[r1],#&842
	subeq	r2,r6,r4,lsr #&20
	ldrshths	r2,[r9],#&40
	rscne	fp,r8,r6,asr #&1A
	svcls	#&F00040

;; GPIOPinRead: 0000944D
GPIOPinRead proc
	strdvc	r2,r3,[r0],-r8
	adcsmi	r0,pc,r7,asr #&20

;; GPIOPinWrite: 00009455
GPIOPinWrite proc

;; GPIOPinTypeComparator: 0000945D
GPIOPinTypeComparator proc
	strhle	ip,[r3],#&FFFFFF2C

;; GPIOPinTypeI2C: 00009481
GPIOPinTypeI2C proc
	strhle	r0,[r6],#&FFFFFF2C
	bleq	$0090986D

l00009489:

;; GPIOPinTypeQEI: 000094A5
GPIOPinTypeQEI proc
	strhle	r0,[r6],#&FFFFFF2C
	beq	$00909891

l000094AD:

;; GPIOPinTypeUART: 000094C9
GPIOPinTypeUART proc
	strhle	r0,[r6],#&FFFFFF2C
	stmdaeq	r4,r3,r4,r5

;; GPIOPinTypeTimer: 000094ED
GPIOPinTypeTimer proc

;; GPIOPinTypeSSI: 000094F1
GPIOPinTypeSSI proc

;; GPIOPinTypePWM: 000094F5
GPIOPinTypePWM proc
	mrc2	p8,#5,lr,c15

;; IntDefaultHandler: 000094F9
IntDefaultHandler proc
	adcseq	r0,pc,r7,ror #1

;; IntMasterEnable: 000094FD
IntMasterEnable proc
	ldrshteq	lr,[sp],r0

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
	ldmlt	r1!,{r1-r2,r6,r10}

l00009515:
	andsne	r1,fp,#&5000

l00009519:
	ldrbteq	r4,[r8],#&368

;; IntUnregister: 00009539
IntUnregister proc
	movtmi	r0,#&A34B

l0000953D:

;; IntPriorityGroupingSet: 0000954D
IntPriorityGroupingSet proc
	movtpl	r0,#&A54B

l00009551:
	teqmi	r0,#&F8

;; IntPriorityGroupingGet: 00009569
IntPriorityGroupingGet proc
	uqsub8eq	lr,r3,r4
	stmdbeq	r0!,{r0,r3,r6}
	stmdbne	r10,{r3,r5-r6,r9-r10}
	rscpl	r0,r0,#&10
	blhi	$00ECA961
	bicseq	r0,r0,r2,asr #4

;; IntPrioritySet: 00009591
IntPrioritySet proc
	subne	r0,fp,r2,lsr #&12

l00009595:
	mvnseq	r2,#&B4

l00009599:
	mcrrne	p3,#0,r2,r4
	mvnseq	r0,#&6A
	rsbgt	r2,r8,r0,lsl #6
	movths	r8,#1<<9
	smlatteq	r3,r10,r2,r0
	ldmne	r0!,{r1,r3-r7}
	rsbne	r2,r0,r3,asr #&20
	strbge	r7,[r7],#&BC
	streq	r0,[r0],-r2,lsr #1

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
	ldrshtvc	ip,[r2],r0
	ldrtge	r0,[pc],#&47                                        ; 000095DD
	streq	r0,[r0],#&A2

;; IntEnable: 000095DD
IntEnable proc
	ldrbeq	r1,[r0,#&328]
	ldrbeq	r1,[r0],r8,lsr #&E
	svceq	#&D01B28
	ldrbeq	r0,[r0,#&728]

;; IntDisable: 00009639
IntDisable proc
	ldrbeq	r1,[r0,#&328]
	ldrbeq	r1,[r0],r8,lsr #&E
	svceq	#&D01B28
	ldrbeq	r0,[r0,#&728]

;; OSRAMDelay: 00009695
OSRAMDelay proc
	sbcsvc	pc,r1,r8,lsr sp

l00009699:
	ldmlo	pc!,{r0-r2,r6}

;; OSRAMWriteFirst: 0000969D
OSRAMWriteFirst proc

;; OSRAMWriteArray: 000096C5
OSRAMWriteArray proc
	ldreq	pc,[r5,#&8B1]!

l000096C9:
	mcrreq	p12,#4,r0,pc
	andseq	r4,r8,ip,asr #&C
	subeq	r2,r6,r1,lsr #&20
	ldrshteq	r7,[sp],#&80
	ldmlo	r0,{r3,r5,r8,fp-pc}
	ldmible	r7,r3,r5,r6
	ldrsheq	r1,[r8,#&5F]!
	subeq	r2,r6,fp,lsl r0
	ldrsheq	r10,[sp,#&80]!
	subeq	r2,r6,r1,lsr #&20
	mrcge	p6,#7,r9,c13

;; OSRAMWriteByte: 00009705
OSRAMWriteByte proc
	strheq	r0,[r6],#&-45
	subeq	r0,r8,r1,lsr #&12

l0000970D:
	ldrshteq	r5,[sp],#&C0
	ldrbeq	pc,[r0,r8,lsr #&12]

;; OSRAMWriteFinal: 00009739
OSRAMWriteFinal proc
	mcreq	p6,#2,r0,c6
	eorhs	r0,r1,ip,asr #&20

l00009741:
	mvnsmi	r0,r6,asr #&20

l00009745:

;; OSRAMClear: 00009781
OSRAMClear proc

;; OSRAMStringDraw: 000097CD
OSRAMStringDraw proc
	mcrreq	p6,#&B,r1,r6
	subhi	r0,r6,r6,asr #&A

;; OSRAMImageDraw: 00009881
OSRAMImageDraw proc
	stmdaeq	r7,r0,r3,r5
	ldreq	r8,[r3,#&69E]!
	stmdbls	r6,{r1-r2,r6,r10,ip}
	teqgt	r1,r6,asr #8
	ssub8ne	r0,r8,r3
	rscsne	r4,r0,r4,asr #&10
	svceq	#&F00108

;; OSRAMInit: 000098F1
OSRAMInit proc
	strbeq	pc,[r1],#&E9
	rscsne	r4,r0,r6,asr #&1E
	svclo	#&F00020
	strdeq	r1,r2,[r8],#&FFFFFF77
	ldcleq	p12,c3,[r9]!

;; OSRAMDisplayOn: 00009975
OSRAMDisplayOn proc
	subne	pc,r1,r9,ror #1

l00009979:
	strteq	lr,[r6],#&34F
	eoreq	r8,r0,r4,lsr #&20

;; OSRAMDisplayOff: 000099C1
OSRAMDisplayOff proc

;; SSIConfig: 000099E9
SSIConfig proc
	strbne	pc,[r1,-r9,ror #1]
	stmdahi	r6,r1,r2,r6
	strbeq	r1,[r6],-r6,asr #&18
	blx	$FFC09C73
	stmdane	pc,r0,r3,r4
	stmdane	pc,r4,r6,r7

;; SSIEnable: 00009A35
SSIEnable proc
	rscseq	r4,r0,#&A0000001
	rsbvc	r4,r0,r3,lsl #6

;; SSIDisable: 00009A41
SSIDisable proc
	rscseq	r2,r0,#&A0000001
	rsbvc	r4,r0,r3,lsl #6
	ldmeq	pc!,{r0-r2,r6}

;; SSIIntRegister: 00009A4D
SSIIntRegister proc

;; SSIIntUnregister: 00009A61
SSIIntUnregister proc

;; SSIIntEnable: 00009A75
SSIIntEnable proc

;; SSIIntDisable: 00009A7D
SSIIntDisable proc

;; SSIIntStatus: 00009A89
SSIIntStatus proc
	strhtvc	r8,[r9],#9
	rsbvc	ip,r9,r7,asr #&20

;; SSIIntClear: 00009A95
SSIIntClear proc
	subeq	r7,r7,r2,rrx

;; SSIDataPut: 00009A99
SSIDataPut proc
	movwne	r0,#&2CF1

l00009A9D:

;; SSIDataNonBlockingPut: 00009AA9
SSIDataNonBlockingPut proc
	rscseq	r1,r0,#&A0000001

;; SSIDataGet: 00009AB9
SSIDataGet proc
	movwne	r0,#&2CF1

l00009ABD:

;; SSIDataNonBlockingGet: 00009AC9
SSIDataNonBlockingGet proc
	ldrbteq	r1,[r0],#&368

;; SysCtlSRAMSizeGet: 00009ADD
SysCtlSRAMSizeGet proc
	blne	$0120AC11

l00009AE1:

;; SysCtlFlashSizeGet: 00009AF5
SysCtlFlashSizeGet proc
	blne	$0120AC29

l00009AF9:

;; SysCtlPinPresent: 00009B0D
SysCtlPinPresent proc

;; SysCtlPeripheralPresent: 00009B21
SysCtlPeripheralPresent proc
	movwpl	r0,#&F24B

l00009B25:
	ldrshths	r2,[r0],r8
	blne	$01025EF1
	strbne	r1,[r2],#&868

;; SysCtlPeripheralReset: 00009B3D
SysCtlPeripheralReset proc
	subeq	r0,fp,#&210

l00009B41:

;; SysCtlPeripheralEnable: 00009B7D
SysCtlPeripheralEnable proc
	movweq	r0,#&F24B

l00009B81:
	blle	$000EA735

l00009B85:
	rscsvc	r2,r0,r9,rrx
	rsbne	r1,r8,r0,asr #&14
	rsbvc	r1,r0,r3,asr #&10
	ldrtpl	r0,[pc],#&47                                        ; 00009B99
	streq	r0,[r0,-#&A5]

;; SysCtlPeripheralDisable: 00009B99
SysCtlPeripheralDisable proc
	movweq	r0,#&F24B

l00009B9D:
	ble	$000EA751

l00009BA1:
	rscsvc	r2,r0,r9,rrx

;; SysCtlPeripheralSleepEnable: 00009BB5
SysCtlPeripheralSleepEnable proc
	movweq	r0,#&F24B

l00009BB9:
	blls	$000EA76D

l00009BBD:
	rscsvc	r2,r0,r10,rrx
	rsbne	r1,r8,r0,asr #&14
	rsbvc	r1,r0,r3,asr #&10
	ldrtpl	r0,[pc],#&47                                        ; 00009BD1
	streq	r0,[r0,-#&A5]

;; SysCtlPeripheralSleepDisable: 00009BD1
SysCtlPeripheralSleepDisable proc
	movweq	r0,#&F24B

l00009BD5:
	bls	$000EA789

l00009BD9:
	rscsvc	r2,r0,r10,rrx

;; SysCtlPeripheralDeepSleepEnable: 00009BED
SysCtlPeripheralDeepSleepEnable proc
	movweq	r0,#&F24B

l00009BF1:
	blpl	$000EA7A5

l00009BF5:
	rscsvc	r2,r0,fp,rrx
	rsbne	r1,r8,r0,asr #&14
	rsbvc	r1,r0,r3,asr #&10
	ldrtpl	r0,[pc],#&47                                        ; 00009C09
	streq	r0,[r0,-#&A5]

;; SysCtlPeripheralDeepSleepDisable: 00009C09
SysCtlPeripheralDeepSleepDisable proc
	movweq	r0,#&F24B

l00009C0D:
	bpl	$000EA7C1

l00009C11:
	rscsvc	r2,r0,fp,rrx

;; SysCtlPeripheralClockGating: 00009C25
SysCtlPeripheralClockGating proc
	stmdane	r8,r1,r3,r6
	ldrhteq	r2,[r0],#&39
	rsbvc	r1,r0,r3,ror #6
	rscseq	r4,r0,r7,asr #6
	rsbvc	r1,r0,r3,ror #6
	adcsvs	r0,pc,r7,asr #&20
	stmdaeq	r0,r5,r6,r7

;; SysCtlIntRegister: 00009C41
SysCtlIntRegister proc
	mcrrhs	p1,#&B,r0,r6
	ldclpl	p15,c15,[r7,#&80]!
	stmeq	r8!,{r2-r8,r10-sp,pc}

;; SysCtlIntUnregister: 00009C55
SysCtlIntUnregister proc

;; SysCtlIntEnable: 00009C69
SysCtlIntEnable proc
	stmdane	r8,r1,r3,r6
	rsbvc	r1,r0,r3,asr #&20
	ldrtpl	r0,[pc],#&47                                        ; 00009C79
	subeq	r0,r0,#&380

;; SysCtlIntDisable: 00009C79
SysCtlIntDisable proc

;; SysCtlIntClear: 00009C89
SysCtlIntClear proc
	rsbvc	r1,r0,fp,asr #&10

l00009C8D:
	ldmpl	pc!,{r0-r2,r6}

l00009C91:
	subne	r0,r0,r0,ror #&1F

;; SysCtlIntStatus: 00009C95
SysCtlIntStatus proc
	stmdane	fp,r0,r3,r4
	subeq	r7,r7,#&68
	rsbvc	r1,r8,fp,asr #&10
	adcspl	r0,pc,r7,asr #&20
	stmdapl	r0,r5,r6,r7
	smlaltteq	r0,r0,r0,pc

;; SysCtlLDOSet: 00009CAD
SysCtlLDOSet proc
	rsbvc	r1,r0,fp,asr #&10

l00009CB1:
	ldrtlo	r0,[pc],#&47                                        ; 00009CB9
	smlaltteq	r0,r0,r0,pc

;; SysCtlLDOGet: 00009CB9
SysCtlLDOGet proc
	rsbvc	r1,r8,fp,asr #&10

l00009CBD:
	ldrtlo	r0,[pc],#&47                                        ; 00009CC5
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
	stcleq	p14,c15,[r7]!
	strbteq	r0,[r0],#&ED
	andeq	pc,r5,r0,lsl #&14

;; SysCtlSleep: 00009CE1
SysCtlSleep proc
	ldrshtne	r0,[r10],r0

;; SysCtlDeepSleep: 00009CE5
SysCtlDeepSleep proc
	movths	r0,#&C6B5

l00009CE9:
	ldrbteq	r4,[r0],#&368
	rsbeq	r2,r0,r3,lsl #6

;; SysCtlResetCauseGet: 00009D05
SysCtlResetCauseGet proc
	rsbvc	r1,r8,fp,asr #&10

l00009D09:
	ldcpl	p0,c0,[pc]!                                          ; 00009D11
	subeq	r0,r0,#&380

;; SysCtlResetCauseClear: 00009D11
SysCtlResetCauseClear proc

;; SysCtlBrownOutConfigSet: 00009D21
SysCtlBrownOutConfigSet proc

;; SysCtlClockSet: 00009D31
SysCtlClockSet proc

;; SysCtlClockGet: 00009DF1
SysCtlClockGet proc

;; SysCtlPWMClockSet: 00009E69
SysCtlPWMClockSet proc

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
	movths	r0,#&3363
	rsbne	r0,r8,r0,ror #&16
	ldrhtvc	r2,[r4],#&3C
	bleq	$010CAC39

;; SysCtlADCSpeedGet: 00009EC5
SysCtlADCSpeedGet proc
	rsbeq	r1,r8,fp,asr #&10

l00009EC9:

;; SysCtlIOSCVerificationSet: 00009ED5
SysCtlIOSCVerificationSet proc
	stmdane	r8,r1,r3,r6
	ldmeq	r0!,{r0,r3-r5,r7-r9,sp}
	rsbvc	r1,r0,r3,lsl #6
	ldmeq	r0!,{r0-r2,r6,r8-r9,lr}
	rsbvc	r1,r0,r3,lsl #6
	adcsvs	r0,pc,r7,asr #&20
	strbeq	r0,[r0,-#&FE0]

;; SysCtlMOSCVerificationSet: 00009EF1
SysCtlMOSCVerificationSet proc
	stmdane	r8,r1,r3,r6
	ldrbteq	r2,[r0],#&3B9
	rsbvc	r1,r0,r3,lsl #6
	ldrbteq	r4,[r0],#&347
	rsbvc	r1,r0,r3,lsl #6
	adcsvs	r0,pc,r7,asr #&20
	strbeq	r0,[r0,-#&FE0]

;; SysCtlPLLVerificationSet: 00009F0D
SysCtlPLLVerificationSet proc
	stmdane	r8,r1,r3,r6
	ldrhthi	r2,[r4],#&39
	rsbvc	r1,r0,r3,ror #6
	rscshi	r4,r4,r7,asr #6
	rsbvc	r1,r0,r3,ror #6
	adcsvs	r0,pc,r7,asr #&20
	smlaltteq	r0,r0,r0,pc

;; SysCtlClkVerificationClear: 00009F29
SysCtlClkVerificationClear proc

;; UARTParityModeSet: 00009F39
UARTParityModeSet proc
	ldrbthi	r2,[r0],r10,ror #6
	mrsgt	r1,spsr
	subgt	r7,r7,r2,rrx

;; UARTParityModeGet: 00009F45
UARTParityModeGet proc
	ldrbthi	r0,[r0],r10,rrx

;; UARTConfigSet: 00009F4D
UARTConfigSet proc

;; UARTConfigGet: 00009FA9
UARTConfigGet proc
	suble	pc,r1,r9,ror #1

l00009FAD:
	streq	r2,[r0],#&4F8
	strbne	r0,[r6],-r6,asr #&1E

;; UARTEnable: 00009FD5
UARTEnable proc
	rscsne	r4,r0,r10,ror #6

;; UARTDisable: 00009FED
UARTDisable proc
	movwne	r1,#&28F1

l00009FF1:

;; UARTCharsAvail: 0000A00D
UARTCharsAvail proc
	rscsne	r8,r0,r9,rrx
	rscseq	ip,r3,r0
	subhi	r7,r7,r0,lsl r0

;; UARTSpaceAvail: 0000A019
UARTSpaceAvail proc
	rscshs	r8,r0,r9,rrx
	rscsmi	ip,r3,r0
	movthi	r7,#&7010

;; UARTCharNonBlockingGet: 0000A025
UARTCharNonBlockingGet proc
	strpl	sp,[r6],#&B69
	svcmi	#&6800BF
	ldrshtvc	pc,[r0],r0
	adcseq	r0,pc,r7,asr #&20

;; UARTCharGet: 0000A035
UARTCharGet proc
	movwne	r1,#&28F1

l0000A039:

;; UARTCharNonBlockingPut: 0000A045
UARTCharNonBlockingPut proc
	bpl	$001B0DF1

l0000A049:
	strheq	r0,[r0,-#&1F]!
	eorvc	r0,r0,r0,lsr #&20

l0000A051:
	adcseq	r0,pc,r7,asr #&20

;; UARTCharPut: 0000A055
UARTCharPut proc
	movwne	r1,#&28F1

l0000A059:

;; UARTBreakCtl: 0000A065
UARTBreakCtl proc

;; UARTIntRegister: 0000A079
UARTIntRegister proc
	strhge	r0,[ip],#&FFFFFF9B
	ldrne	r0,[pc,#&C42]!                                       ; 0000ACC7
	eorhs	r1,r4,r4,lsr #&C

l0000A085:
	ldcllo	p15,c15,[r7,#&118]!
	stcllt	p0,c2,[r6,-#&3E8]

;; UARTIntUnregister: 0000A099
UARTIntUnregister proc
	strhge	r0,[ip],#&FFFFFF9B
	ldrne	r0,[pc,#&C42]!                                       ; 0000ACE7
	eorhs	r1,r4,r4,lsr #&C

l0000A0A5:
	ldrbgt	pc,[r7,r6,asr #&1E]!

l0000A0A9:
	stcllt	p0,c2,[r6,-#&3E8]

;; UARTIntEnable: 0000A0B9
UARTIntEnable proc

;; UARTIntDisable: 0000A0C1
UARTIntDisable proc

;; UARTIntStatus: 0000A0CD
UARTIntStatus proc
	strhtvc	ip,[fp],#9
	rsbvc	r0,ip,r7,asr #&20

;; UARTIntClear: 0000A0D9
UARTIntClear proc
	subvs	r7,r7,#&64

;; CPUcpsie: 0000A0DD
CPUcpsie proc
	strhvc	r7,[r7],#-6
	adcsvc	r0,pc,#&47

;; CPUcpsid: 0000A0E5
CPUcpsid proc
	strhvc	r7,[r7],#-6
	adcslo	r0,pc,r7,asr #&20

;; CPUwfi: 0000A0ED
CPUwfi proc
	strhvc	r7,[r7],#&-F
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

;; I2CSlaveInit: 0000A129
I2CSlaveInit proc

;; I2CMasterEnable: 0000A141
I2CMasterEnable proc
	rscsne	r4,r0,r10,ror #6
	rsbvc	r0,r2,r3,lsl #6

;; I2CSlaveEnable: 0000A14D
I2CSlaveEnable proc
	ldc2l	p0,c10,[r5]!

;; I2CMasterDisable: 0000A161
I2CMasterDisable proc
	rscsne	r2,r0,r10,ror #6
	rsbvc	r0,r2,r3,lsl #6
	adcseq	r0,pc,r7,asr #&20

;; I2CSlaveDisable: 0000A16D
I2CSlaveDisable proc
	ldc2l	p0,c10,[r5]!

;; I2CIntRegister: 0000A181
I2CIntRegister proc

;; I2CIntUnregister: 0000A195
I2CIntUnregister proc

;; I2CMasterIntEnable: 0000A1A9
I2CMasterIntEnable proc
	rsbvc	r0,r1,r3,lsr #6

l0000A1AD:

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

;; I2CMasterIntStatus: 0000A1C9
I2CMasterIntStatus proc
	strhteq	r4,[r9],#9

;; I2CSlaveIntStatus: 0000A1E1
I2CSlaveIntStatus proc
	strhteq	r0,[r9],#9

;; I2CMasterIntClear: 0000A1F9
I2CMasterIntClear proc

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

;; I2CMasterControl: 0000A221
I2CMasterControl proc
	movtmi	r7,#&7060

;; I2CMasterErr: 0000A225
I2CMasterErr proc
	streq	sp,[r7,-#&A68]
	rscseq	r1,r0,#&50000003
	bicseq	r0,r0,#0
	strdvc	r1,r2,[r0],-r0
	eorvc	r0,r0,r7,asr #&20

;; I2CMasterDataPut: 0000A23D
I2CMasterDataPut proc
	subhi	r7,r7,r0,rrx

;; I2CMasterDataGet: 0000A241
I2CMasterDataGet proc
	submi	r7,r7,r8,rrx

;; I2CSlaveStatus: 0000A245
I2CSlaveStatus proc

;; I2CSlaveDataPut: 0000A249
I2CSlaveDataPut proc
	subhi	r7,r7,r0,rrx

;; I2CSlaveDataGet: 0000A24D
I2CSlaveDataGet proc
	stmdami	r7,r3,r5,r6
	svcvs	#&6C6C65
	movwmi	r0,#0
	blvs	$018E3801
	andpl	r0,r0,r0
	strbtvc	r6,[lr],#&972
	movwpl	r0,#0
	ldclvs	p15,c6,[r5]!
	svcvs	#&6E2064
	strbvs	r2,[r2,-#&74]!
	strbvs	r7,[r8,-#&420]!
	stmdbmi	r0,{r1,r4-r6,r8,r10,sp-lr}
	subeq	r4,r5,r4,asr #&18
	strls	r0,[r0],-r0
0000A284             96 00 00 00 C8 00 00 00 FA 00 00 00     ............
0000A290 2C 01 00 00 5E 01 00 00 90 01 00 00 C2 01 00 00 ,...^...........
0000A2A0 F4 01 00 00                                     ....           
0000A2A4             00 07 00 00 00 06 00 00 00 05 00 00     ............
0000A2B0 00 04 00 00 00 03 00 00 00 02 00 00 00 01 00 00 ................
0000A2C0 00 00 00 00                                     ....           
0000A2C4             00 00 00 00 18 ED 00 E0 1C ED 00 E0     ............
0000A2D0 20 ED 00 E0 00 E4 00 E0 04 E4 00 E0 08 E4 00 E0  ...............
0000A2E0 0C E4 00 E0 10 E4 00 E0 14 E4 00 E0 18 E4 00 E0 ................
0000A2F0 1C E4 00 E0                                     ....           
0000A2F4             B0 80 04 80 12 40                       .....@     
0000A2FA                               00 00                       ..   
0000A2FC                                     B1 80 04 80             ....
0000A300 12 40                                           .@             
0000A302       00 00                                       ..           
0000A304             00 00 00 00 00 00 00 4F 00 00 00 07     .......O....
0000A310 00 07 00 14 7F 14 7F 14 24 2A 7F 2A 12 23 13 08 ........$*.*.#..
0000A320 64 62 36 49 55 22 50 00 05 03 00 00 00 1C 22 41 db6IU"P......."A
0000A330 00 00 41 22 1C 00 14 08 3E 08 14 08 08 3E 08 08 ..A"....>....>..
0000A340 00 50 30 00 00 08 08 08 08 08 00 60 60 00 00 20 .P0........``.. 
0000A350 10 08 04 02 3E 51 49 45 3E 00 42 7F 40 00 42 61 ....>QIE>.B.@.Ba
0000A360 51 49 46 21 41 45 4B 31 18 14 12 7F 10 27 45 45 QIF!AEK1.....'EE
0000A370 45 39 3C 4A 49 49 30 01 71 09 05 03 36 49 49 49 E9<JII0.q...6III
0000A380 36 06 49 49 29 1E 00 36 36 00 00 00 56 36 00 00 6.II)..66...V6..
0000A390 08 14 22 41 00 14 14 14 14 14 00 41 22 14 08 02 .."A.......A"...
0000A3A0 01 51 09 06 32 49 79 41 3E 7E 11 11 11 7E 7F 49 .Q..2IyA>~...~.I
0000A3B0 49 49 36 3E 41 41 41 22 7F 41 41 22 1C 7F 49 49 II6>AAA".AA"..II
0000A3C0 49 41 7F 09 09 09 01 3E 41 49 49 7A 7F 08 08 08 IA.....>AIIz....
0000A3D0 7F 00 41 7F 41 00 20 40 41 3F 01 7F 08 14 22 41 ..A.A. @A?...."A
0000A3E0 7F 40 40 40 40 7F 02 0C 02 7F 7F 04 08 10 7F 3E .@@@@..........>
0000A3F0 41 41 41 3E 7F 09 09 09 06 3E 41 51 21 5E 7F 09 AAA>.....>AQ!^..
0000A400 19 29 46 46 49 49 49 31 01 01 7F 01 01 3F 40 40 .)FFIII1.....?@@
0000A410 40 3F 1F 20 40 20 1F 3F 40 38 40 3F 63 14 08 14 @?. @ .?@8@?c...
0000A420 63 07 08 70 08 07 61 51 49 45 43 00 7F 41 41 00 c..p..aQIEC..AA.
0000A430 02 04 08 10 20 00 41 41 7F 00 04 02 01 02 04 40 .... .AA.......@
0000A440 40 40 40 40 00 01 02 04 00 20 54 54 54 78 7F 48 @@@@..... TTTx.H
0000A450 44 44 38 38 44 44 44 20 38 44 44 48 7F 38 54 54 DD88DDD 8DDH.8TT
0000A460 54 18 08 7E 09 01 02 0C 52 52 52 3E 7F 08 04 04 T..~....RRR>....
0000A470 78 00 44 7D 40 00 20 40 44 3D 00 7F 10 28 44 00 x.D}@. @D=...(D.
0000A480 00 41 7F 40 00 7C 04 18 04 78 7C 08 04 04 78 38 .A.@.|...x|...x8
0000A490 44 44 44 38 7C 14 14 14 08 08 14 14 18 7C 7C 08 DDD8|........||.
0000A4A0 04 04 08 48 54 54 54 20 04 3F 44 40 20 3C 40 40 ...HTTT .?D@ <@@
0000A4B0 20 7C 1C 20 40 20 1C 3C 40 30 40 3C 44 28 10 28  |. @ .<@0@<D(.(
0000A4C0 44 0C 50 50 50 3C 44 64 54 4C 44 00 08 36 41 00 D.PPP<DdTLD..6A.
0000A4D0 00 00 7F 00 00 00 41 36 08 00 02 01 02 04 02    ......A6.......
0000A4DF                                              00                .
0000A4E0 04 80 AE 80 E3 04 80 04 80 E3 04 80 12 80 E3 06 ................
0000A4F0 80 81 80 2B 80 E3 04 80 A1 80 E3 04 80 40 80 E3 ...+.........@..
0000A500 06 80 D3 80 00 80 E3 06 80 A8 80 0F 80 E3 04 80 ................
0000A510 A4 80 E3 04 80 A6 80 E3 04 80 B0 80 E3 04 80 C8 ................
0000A520 80 E3 06 80 D5 80 72 80 E3 06 80 D8 80 00 80 E3 ......r.........
0000A530 06 80 D9 80 22 80 E3 06 80 DA 80 12 80 E3 06 80 ...."...........
0000A540 DB 80 0F 80 E3 06 80 AD 80 8B 80 E3 04 80 AF 80 ................
0000A550 E3                                              .              
0000A551    00 00 00                                      ...           
0000A554             10 E0 0F 40 14 E0 0F 40 1C E0 0F 40     ...@...@...@
0000A560 10 E0 0F 40                                     ...@           
0000A564             40 E0 0F 40 44 E0 0F 40 48 E0 0F 40     @..@D..@H..@
0000A570 00 E1 0F 40 04 E1 0F 40 08 E1 0F 40             ...@...@...@   
0000A57C                                     10 E1 0F 40             ...@
0000A580 14 E1 0F 40 18 E1 0F 40                         ...@...@       
0000A588                         20 E1 0F 40 24 E1 0F 40          ..@$..@
0000A590 28 E1 0F 40                                     (..@           
0000A594             99 9E 36 00 00 40 38 00 00 09 3D 00     ..6..@8...=.
0000A5A0 00 80 3E 00 00 00 4B 00 40 4B 4C 00 00 20 4E 00 ..>...K.@KL.. N.
0000A5B0 80 8D 5B 00 00 C0 5D 00 00 80 70 00 00 12 7A 00 ..[...]...p...z.
0000A5C0 00 00 7D 00                                     ..}.           
;;; Segment .text.memcpy (0000A5C4)
0000A5C4             F0                                      .          

;; memcpy: 0000A5C5
memcpy proc
	svceq	#&5B5
	bleq	$FF656279

l0000A5CD:
	blls	$010CB1D5

l0000A5D1:
	ldrbne	r3,[r1],r7,lsl #&C
	movweq	r0,#&C00

l0000A5D9:
	ldrlo	r1,[lr]!
	ldchs	p1,c0,[r5,-#&24]!
	ldrhs	r4,[r9,-r1,lsl #&A]
	strbvs	r1,[r0,-r8,ror #&1E]!
	strbge	r5,[r0,-r8,ror #&1E]!
	strb	r9,[r0,-r8,ror #&1E]!
	rsbne	sp,r0,r8,ror #&1E
	ldcls	p0,c1,[r4,-#&CC]!
	svceq	#&D1F342
	subne	r9,r3,r3,lsr #&1C
	ldmdbhi	r9,r1,r2,r4
	movteq	r1,#&319
	cdpne	p12,#&D,c1,c9
	strtlt	r0,[r3],#&1F
	ldrtge	r0,[r4],#&108
	svc	#&58CF00
	teqge	r3,#&50000000
	bicseq	pc,r1,#&42000
	movtlo	r10,#&3624
	stmdbgt	r0,{r0,r2-r4,r9,sp}
	andseq	lr,r8,r8,lsl sp
	sbcseq	r0,r0,r10,lsr #&A
	mrrc	p12,#2,ip,ip
	teqls	r3,#&15
;;; Segment .data (20000000)
20000000 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
200000B0 00 00 00 00 00 00 00 00                         ........       
200000B8                         01 00 00 00                     ....   
200000BC                                     AA AA AA AA             ....
200000C0 01 00 00 00                                     ....           
;;; Segment privileged_data (200000C4)
200000C4             00 00 00 00                             ....       
200000C8                         00 00 00 00                     ....   
200000CC                                     00 00 00 00             ....
200000D0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
200000F0 00 00 00 00                                     ....           
200000F4             00 00 00 00 00 00 00 00 00 00 00 00     ............
20000100 00 00 00 00 00 00 00 00                         ........       
20000108                         00 00 00 00 00 00 00 00         ........
20000110 00 00 00 00 00 00 00 00 00 00 00 00             ............   
2000011C                                     00 00 00 00             ....
20000120 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
20000130 00 00 00 00                                     ....           
20000134             00 00 00 00                             ....       
20000138                         00 00 00 00                     ....   
2000013C                                     00 00 00 00             ....
20000140 00 00 00 00                                     ....           
20000144             00 00 00 00                             ....       
20000148                         00 00 00 00                     ....   
2000014C                                     00 00 00 00             ....
20000150 00 00 00 00                                     ....           
20000154             00 00 00 00                             ....       
20000158                         00 00 00 00                     ....   
2000015C                                     00 00 00 00             ....
;;; Segment .bss (20000160)
20000160 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
20000220 00 00 00 00 00 00 00 00 00 00 00 00             ............   
2000022C                                     00                      .  
2000022D                                        00 00 00              ...
20000230 00 00 00 00                                     ....           
20000234             00 00 00 00 00 00 00 00 00 00 00 00     ............
20000240 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
200007F0 00 00 00 00                                     ....           
200007F4             00                                      .          
200007F5                00 00 00                              ...       
200007F8                         00 00 00 00                     ....   
200007FC                                     00 00 00 00             ....
20000800 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
20000820 00 00 00 00 00 00 00 00                         ........       
20000828                         00 00 00 00 00 00 00 00         ........
20000830 00 00 00 00 00 00 00 00 00 00 00 00             ............   
2000083C                                     00 00 00 00             ....
20000840 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
20000850 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
20000860 00 00 00 00                                     ....           
20000864             00 00 00 00                             ....       
20000868                         00 00 00 00                     ....   
2000086C                                     00 00 00 00             ....
20000870 00 00 00 00                                     ....           
20000874             00 00 00 00                             ....       
20000878                         00 00 00 00                     ....   
2000087C                                     00 00 00 00             ....
20000880 00 00 00 00                                     ....           
